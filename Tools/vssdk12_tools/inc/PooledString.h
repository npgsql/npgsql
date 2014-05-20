/*--------------------------------------------------------------------------*
 *
 *  Microsoft Visual Studio
 *  Copyright (C) Microsoft Corporation, 1995 - 2008
 *
 *  File:       PooledString.h
 *
 *  Contents:   Implementation file for CPooledString
 *
 *  History:    8-Feb-2008 jeffro    Created
 *
 *--------------------------------------------------------------------------*/
#ifndef _POOLEDSTRING_H_
#define _POOLEDSTRING_H_

#pragma once

#include <map>
#include <unordered_map>
#include "optpragmas.h"


class CStringHash
{
public:
    template<typename BaseType, class StringTraits>
    size_t operator() (const CStringT<BaseType, StringTraits>& str)
    {
        return (CElementTraits<CStringT<BaseType, StringTraits>>::Hash (str));
    }
};


/*+-------------------------------------------------------------------------*
 * CPooledStringT
 *
 * This class is intended to reduce the memory consumed by multiple, identical
 * CString objects.  The linker will fold identical C-style strings (char* or
 * wchar_t*) into a single instance, but no such mechanism exists for CStrings.
 * 
 * CPooledString maintains a static pool of strings that are represented by
 * CPooledString objects.  Each string in the pool as a reference count
 * (unrelated to the internal refcount maintained in the header portion of a
 * CString object).  As CPooledStrings are created, the pool is searched for
 * a CString matching the content of the CPooledString.  If it's in the pool
 * already, the refcount is bumped; if it's not there yet  it's added with a
 * refcount of 1.  As CPooledStrings are destroyed, the refcount is
 * decremented and if it goes to zero the CString is removed from the pool.
 * 
 * Each CPooledString contains a pointer to a CString and its refcount in the
 * pool; its public API hides the fact that the string may be shared among
 * multiple CPooledString objects.
 * 
 * CPooledString isn't intended to be a full-service replacement for CString.
 * Specifically, CPooledStrings are immutable.  (Well, almost immutable. 
 * They can be changed by assignment (i.e. by completely replacing the 
 * contained CString), but not by mutating the contained CString in place.)
 * It defines a set of constructors and assignment operators, a set of
 * comparison operators, and implicit conversions to const CString& and
 * LPCxSTR.  If you want to do other CString-like things (find its length,
 * tokenize it, extract a substring, etc.), you can obtain a const reference
 * to the inner string by using the implicit conversion or by using 
 * GetStringObject.
 *-----------------------------------------------------------------(jeffro)-*/
template<class StringType>
class CPooledStringT
{
public:
    typedef typename StringType                 StringType;
    typedef typename StringType::XCHAR          XCHAR;
    typedef typename StringType::PXSTR          PXSTR;
    typedef typename StringType::PCXSTR         PCXSTR;

    typedef typename std::unordered_map<StringType, size_t, CStringHash> PoolType;
    typedef typename PoolType::value_type PairType;

    /*+-------------------------------------------------------------------------*
     * CPooledStringT
     *
     * Constructs a CPooledStringT object. 
     *-----------------------------------------------------------------(jeffro)-*/
    CPooledStringT (const StringType& s = StringType()) : m_pPoolElement(nullptr)
    {
        InitFrom (Lookup (s));
    }

    CPooledStringT (const CPooledStringT& other) : m_pPoolElement(nullptr)
    {
        InitFrom (other);
    }

    CPooledStringT (PCXSTR psz) : m_pPoolElement(nullptr)
    {
        InitFrom (Lookup (CCachedInitString(psz)));
    }


    /*+-------------------------------------------------------------------------*
     * CPooledStringT::~CPooledStringT
     *
     * Destroys a CPooledStringT object.
     *-----------------------------------------------------------------(jeffro)-*/
    ~CPooledStringT()
    {
        Release();
    }


    /*+-------------------------------------------------------------------------*
     * CPooledStringT::operator=
     *
     * Assignment operators.
     *-----------------------------------------------------------------(jeffro)-*/
    CPooledStringT& operator= (const StringType& s) 
    {
        CopyFrom (s);
        return (*this);
    }

    CPooledStringT& operator= (const CPooledStringT& other)
    {
        CopyFrom (other);
        return (*this);
    }

    CPooledStringT& operator= (PCXSTR psz)
    {
        CopyFrom (CCachedInitString(psz));
        return (*this);
    }


    /*+-------------------------------------------------------------------------*
     * CPooledStringT::operator==
     * CPooledStringT::operator!=
     *
     * (In-) equality operators.
     *-----------------------------------------------------------------(jeffro)-*/
    bool operator== (const StringType& s) const
    {
        return (InnerString == s);
    }

    bool operator== (const CPooledStringT& other) const
    {
        /*
         * optimize by comparing pointers, since we know that two identical
         * pooled strings will share a string representation
         */
        return (m_pPoolElement == other.m_pPoolElement);
    }

    bool operator== (PCXSTR psz) const
    {
        return (InnerString == psz);
    }

    bool operator!= (const StringType& s) const
    {
        return !(*this == s);
    }

    bool operator!= (const CPooledStringT& other) const
    {
        return !(*this == other);
    }

    bool operator!= (PCXSTR psz) const
    {
        return !(*this == psz);
    }


    /*+-------------------------------------------------------------------------*
     * CPooledStringT::operator<
     *
     * This operator is required for sorting by STL collections and algorithms.
     *-----------------------------------------------------------------(jeffro)-*/
    bool operator< (const StringType& s) const
    {
        return (InnerString < s);
    }

    bool operator< (const CPooledStringT& other) const
    {
        return (InnerString < other.InnerString);
    }

    bool operator< (PCXSTR psz) const
    {
        return (InnerString < psz);
    }


    /*+-------------------------------------------------------------------------*
     * CPooledStringT::operator PCXSTR
     * CPooledStringT::operator const StringType&
     *
     * Casting operators
     *-----------------------------------------------------------------(jeffro)-*/
    operator PCXSTR() const
    {
        return (InnerString);
    }

    operator const StringType&() const
    {
        return (InnerString);
    }


    /*+-------------------------------------------------------------------------*
     * CPooledStringT::GetString
     * CPooledStringT::GetStringObject
     *
     * Allow access to the contained string as an object or a character pointer.
     *-----------------------------------------------------------------(jeffro)-*/
    PCXSTR GetString() const
    {
        return (InnerString);
    }

    const StringType& GetStringObject() const
    {
        return (InnerString);
    }

    int GetLength() const throw()
    {
        return (InnerString.GetLength());
    }

    bool IsEmpty() const throw()
    {
        return (InnerString.IsEmpty());
    }

    /*+-------------------------------------------------------------------------*
     * CPooledStringT::GetPoolSize
     *
     * Returns the number of strings in the string pool.  Mostly useful for
     * debugging purposes.
     *-----------------------------------------------------------------(jeffro)-*/
    static size_t GetPoolSize()
    {
        return (GetPool().size());
    }


    /*+-------------------------------------------------------------------------*
     * CPooledStringT::GetPoolMemUsage
     *
     * Returns the memory occupied by the strings in the pool (not including
     * the overhead of the pool itself).  If the optional pcbWouldBe parameter
     * is passed, then the amount of memory that the strings would occupy if
     * not pooled.
     * 
     * This function is useful for diagnostics only.
     *-----------------------------------------------------------------(jeffro)-*/
    static size_t GetPoolMemUsage (size_t* pcbWouldBe = NULL)
    {
        size_t cbTotal = 0;

        if (pcbWouldBe != NULL)
            *pcbWouldBe = 0;

        PoolType::const_iterator it;

        for (it = GetPool().begin(); it != GetPool().end(); ++it)
        {
            const StringType& s     = it->first;
            const size_t&     cRefs = it->second;

            /*
             * ATL allocates memory blocks for strings in multiples of 8 characters
             */
            const size_t cb = sizeof(XCHAR) * AtlAlignUp (s.GetLength()+1, 8);

            cbTotal += cb;

            if (pcbWouldBe != NULL)
                *pcbWouldBe += cb * cRefs;
        }

        return (cbTotal);
    }

#ifdef DEBUG
    /*+-------------------------------------------------------------------------*
     * CPooledStringT::ShowPoolStats
     *
     * Shows statistics for the string pool, optionally dumping the pool contents
     * to the debugger.
     *-----------------------------------------------------------------(jeffro)-*/
    static void ShowPoolStats (bool fDumpPool)
    {
        if (fDumpPool)
        {  
            CWaitCursor wait;
            DumpPool();
        }

        size_t cbWouldBe;
        size_t cbTotal   = GetPoolMemUsage (&cbWouldBe);
        size_t cbSavings = cbWouldBe - cbTotal;

        StringType strFormat = L"Pooled string count:\t\t\t%d\n"
                               L"Pool memory usage:  \t\t%d bytes\n"
                               L"Individual strings would have used:\t%d bytes\n"
                               L"Memory savings:\t\t\t%d bytes (%d%%)";

        StringType strMessage;
        strMessage.Format (strFormat,
                           GetPoolSize(),
                           cbTotal,
                           cbWouldBe,
                           cbSavings,
                           (cbSavings * 100) / cbWouldBe);

        if (fDumpPool)
        {
            strMessage += L"\n\nSee debugger Output window for string pool contents.";
        }

        MessageBox (Main_hwndMainFrame, strMessage, L"String Pool Stats", MB_OK);
    }

    static void DumpPool()
    {
        StringType strMessage;
        StringType strFormat = L"String pool (%d elements)\n"
                               L"Refs String\n"
                               L"---- ---------------------\n";

        strMessage.Format (strFormat, GetPool().size());
        OutputDebugString (strMessage);

        /*
         * copy from a hash map to a normal map so strings will be sorted
         * alphabetically instead of by hash value
         */
        typedef std::map<StringType, size_t> SortedPoolType;
        SortedPoolType sortedPool;
        std::copy (GetPool().begin(), GetPool().end(), std::inserter(sortedPool, sortedPool.end()));

        strFormat = L"%4d %s\n";

        SortedPoolType::const_iterator it;
        for (it = sortedPool.begin(); it != sortedPool.end(); ++it)
        {
            const StringType& s     = it->first;
            const size_t&     cRefs = it->second;

            strMessage.Format (strFormat, cRefs, s);
            OutputDebugString (strMessage);
        }
    }
#endif

    
private:
    /*+-------------------------------------------------------------------------*
     * CPooledStringT::InnerString
     *
     *
     *-----------------------------------------------------------------(joshs)-*/
    __declspec(property(get=get_InnerString)) const StringType& InnerString;

    const StringType& get_InnerString() const
    {
        if (!m_pPoolElement)
        {
            VSFAIL("m_pPoolElement was not initialized");
            return s_emptyStringPair.first;
        }
        return m_pPoolElement->first;
    }


    /*+-------------------------------------------------------------------------*
     * CPooledStringT::RefCount
     *
     *
     *-----------------------------------------------------------------(joshs)-*/
    __declspec(property(get=get_RefCount, put=put_RefCount)) size_t RefCount;

    size_t get_RefCount() const
    {
        if (!m_pPoolElement)
        {
            VSFAIL("m_pPoolElement was not initialized");
            return 0;
        }
        return m_pPoolElement->second;
    }

    size_t put_RefCount(size_t count) const
    {
        if (!m_pPoolElement)
        {
            VSFAIL("m_pPoolElement was not initialized");
            return 0;
        }
        m_pPoolElement->second = count;
        return count;
    }


    /*+-------------------------------------------------------------------------*
     * CPooledStringT::InitFrom
     *
     *
     *-----------------------------------------------------------------(jeffro)-*/
    void InitFrom (const CPooledStringT& other)
    {
        m_pPoolElement = other.m_pPoolElement;
        AddRef();
    }

    void InitFrom (PairType& pair)
    {
        m_pPoolElement = &pair;
        AddRef();
    }


    /*+-------------------------------------------------------------------------*
     * CPooledStringT::CopyFrom
     *
     *
     *-----------------------------------------------------------------(jeffro)-*/
    void CopyFrom (const CPooledStringT& s)
    {
        if (*this != s)
        {
            Release();
            InitFrom (s);
        }
    }

    void CopyFrom (const StringType& s)
    {
        if (*this != s)
        {
            Release();
            InitFrom (Lookup (s));
        }
    }


#pragma VS_OPTIMIZE_FAVOR_SPEED

    /*+-------------------------------------------------------------------------*
     * CPooledStringT::AddRef
     *
     *
     *-----------------------------------------------------------------(jeffro)-*/
    void AddRef()
    {
        ++RefCount;
    }


    /*+-------------------------------------------------------------------------*
     * CPooledStringT::Release
     *
     *
     *-----------------------------------------------------------------(jeffro)-*/
    void Release()
    {
        /*
         * last ref on this pool string?  remove it from the pool
         */
        if (--RefCount == 0)
        {
            PoolType::iterator it = GetPool().find (InnerString);

            if (it != GetPool().end())
            {
                GetPool().erase (it);
            }
            else
            {
                VSFAIL ("string should be in the pool");
            }
        }

        m_pPoolElement = nullptr;
    }

#pragma VS_OPTIMIZE_DEFAULT


    /*+-------------------------------------------------------------------------*
     * CPooledStringT::Lookup
     *
     * Finds the given string in the pool (adding if necessary) and returns an
     * iterator to the string's entry in the pool.
     *-----------------------------------------------------------------(jeffro)-*/
    static PairType& Lookup (const StringType& s)
    {
        /*
         * optimize the common case of looking up an empty string
         */
        if (s.IsEmpty())
            return s_emptyStringPair;

        /*
         * Insert the string.  If the string was already in the pool,
         * pr.first will be an iterator to its existing entry; if it's 
         * a new string, pr.first will be an iterator to the new entry
         */
        std::pair<PoolType::iterator, bool> pr = GetPool().insert (std::make_pair(s,0));
        PoolType::iterator itEntry   = pr.first;
        bool               fInserted = pr.second;

        /*
         * make sure that for a new item the refcount in zero and for
         * an existing item the refcount is non-zero
         */
        VSASSERT (( fInserted && (itEntry->second == 0)) ||
                  (!fInserted && (itEntry->second >  0)),
                  "Unexpected refcount for string pool entry");

        return (*itEntry);
    }


    /*+-------------------------------------------------------------------------*
     * CPooledStringT::GetPool
     *
     *
     *-----------------------------------------------------------------(joshs)-*/
    static PoolType& GetPool()
    {
        VSASSERT(s_dwInitialThreadId == GetCurrentThreadId(), "String pool accessed on multiple threads -- potential for data corruption unless locks are added.");
        return s_pool;
    }


private:
    // Store a pointer to our pair<StringType, size_t> in the pool so AddRef and Release can avoid
    // doing a lookup operation.  unordered_map iterators may be invalidated by insertion into the
    // map, so we can't store an iterator.
    typename PairType* m_pPoolElement;

    static typename PoolType s_pool;
    static typename PairType s_emptyStringPair;
    static const DWORD s_dwInitialThreadId;


    /*+-------------------------------------------------------------------------*
     * CPooledStringT::CCachedInitString
     *
     * This class serves as a cached location for initializing CPooledStringTs.
     * CPooledStringT's PCXSTR constructor must create a temporary StringType to
     * use as a key for use in Lookup.  Profiling indicated that the PCXSTR
     * constructor was called very frequently, so the alloc/copy/free overhead
     * for the temporary StringType was significant.
     * 
     * By using this cache we can remove the most expensive portion of that
     * overhead, namely the alloc/free.
     *-----------------------------------------------------------------(jeffro)-*/
    class CCachedInitString
    {
    public:
        CCachedInitString (PCXSTR psz) 
        {
            s_cache = psz;
        }

        ~CCachedInitString()
        {
            /*
             * if our cached string has grown larger that we want to keep
             * around long-term, empty it out
             */
            if (s_cache.GetLength() > cchMaxCacheThreshold)
            {
                s_cache.Empty();
            }
        }

        operator const StringType&() const
        {
            return (s_cache);
        }

    private:
        static const size_t cchMaxCacheThreshold = 64;
        static StringType s_cache;
    };
};


typedef CPooledStringT<CStringW> CPooledStringW;
typedef CPooledStringT<CStringA> CPooledStringA;
typedef CPooledStringT<CString>  CPooledString;

#ifdef _MANAGED
#define POOLEDSTRINGDECL __declspec(selectany) __declspec(process)
#else
#define POOLEDSTRINGDECL __declspec(selectany)
#endif

/*
 * Profiling indicated that we spent a lot of time in Lookup searching for
 * empty strings.  We can avoid this by pointing empty CPooledStrings at the
 * static s_emptyStringPair member rather than pooling them.  We initialize it
 * with a refcount of one (not zero) so that we'll never try to remove it from
 * the pool (which would fail), as we would if its refcount reached zero.
 */
#define DECLARE_POOLEDSTRING_STATICS(PooledStringType)  \
    POOLEDSTRINGDECL PooledStringType::PoolType             PooledStringType::s_pool; \
    POOLEDSTRINGDECL PooledStringType::StringType           PooledStringType::CCachedInitString::s_cache; \
    POOLEDSTRINGDECL PooledStringType::PairType             PooledStringType::s_emptyStringPair(PooledStringType::StringType(), 1); \
    POOLEDSTRINGDECL const DWORD                            PooledStringType::s_dwInitialThreadId = GetCurrentThreadId();

DECLARE_POOLEDSTRING_STATICS (CPooledStringW);
DECLARE_POOLEDSTRING_STATICS (CPooledStringA);

_STDEXT_BEGIN

/*
 * define overloads of stdext::hash_value(CPooledString) so CPooledStrings can be 
 * key values in hashed collections (stdext::hash_map, stdext::hash_set, etc.)
 */
template<class StringType>
inline size_t hash_value(const CPooledStringT<StringType>& s)
{   
    return (CStringHash() (s.GetStringObject()));
}

_STDEXT_END

#endif
