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

#pragma once

#include <map>
#include <hash_map>


_STDEXT_BEGIN

/*
 * define overloads of stdext::hash_value(CString) so CStrings can be key values 
 * in hashed collections (stdext::hash_map, stdext::hash_set, etc.)
 */
template<typename BaseType, class StringTraits>
inline size_t hash_value(const CStringT<BaseType, StringTraits>& str)
{
    return (CElementTraits<CStringT<BaseType, StringTraits>>::Hash (str));
}

_STDEXT_END


/*+-------------------------------------------------------------------------*
 * CPooledStringT
 *
 * This class is intended to reduce the memory consumed by multiple, identical
 * CString objects.  The linker will fold identical C-style strings (char* or
 * wchar_t*) into a single instance, but no such mechanism exists for CStrings.
 * 
 * This code:
 * 
 *      CString s1 = "Some text";
 *      CString s2 = "Some text";
 * 
 * will result in a single instance of the string "Some text" embedded in the 
 * binary, but at runtime there will be two dynamically-allocated buffers:
 * 
 *          +-----------+
 *      s1: | m_pszData | ----------+
 *          +-----------+           |
 *                                  V
 *                        +----------------------+
 *                        | header | "Some text" |
 *                        +----------------------+
 *      
 *          +-----------+
 *      s2: | m_pszData | ----------+
 *          +-----------+           |
 *                                  V
 *                        +----------------------+
 *                        | header | "Some text" |
 *                        +----------------------+
 * 
 * CString supports refcounting so if one CString is initialized from another,
 * we end up with a more efficient story:
 * 
 *      CString s1 = "Some text";
 *      CString s2 = s1;
 * 
 * There's still only one copy of "Some text" embedded in the binary, but 
 * now the runtime memory layout looks like this:
 * 
 *          +-----------+
 *      s1: | m_pszData | ----------+
 *          +-----------+           |
 *                                  V
 *                        +----------------------+
 *                        | header | "Some text" |
 *                        +----------------------+
 *                                  ^
 *          +-----------+           |
 *      s2: | m_pszData | ----------+
 *          +-----------+           
 * 
 * CPooledString takes advantage of this.  It maintains a static pool of
 * strings that are contained by CPooledString objects.  Each string in the
 * pool as a reference count, independent of the internal refcount maintained
 * in the header portion of a CString object.  As CPooledStrings are created, 
 * the pool is searched for a CString matching the content of the CPooledString.  
 * If it's in the pool already, the refcount is bumped; if it's not there yet 
 * it's added with a refcount of 1.  As CPooledStrings are destroyed, the 
 * refcount is decremented and if it goes to zero the CString is removed from 
 * the pool.
 * 
 * Each CPooledString contains a CString and achieves its memory savings by 
 * ensuring that its contained CString is initialized from the CString in the
 * pool, taking advantage of CString's internal refcounting system to achieve
 * the memory sharing.
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

    typedef typename stdext::hash_map<StringType, size_t> PoolType;

    /*+-------------------------------------------------------------------------*
     * CPooledStringT
     *
     * Constructs a CPooledStringT object. 
     *-----------------------------------------------------------------(jeffro)-*/
    CPooledStringT (const StringType& s = StringType())
    {
        InitFrom (Lookup (s));
    }

    CPooledStringT (const CPooledStringT& other)
    {
        InitFrom (other.m_s);
    }

    CPooledStringT (PCXSTR psz)
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
        CopyFrom (other.m_s);
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
        return (m_s == s);
    }

    bool operator== (const CPooledStringT& other) const
    {
        /*
         * optimize by comparing pointers, since we know that two idendtical
         * pooled strings will share a string representation
         */
        return (m_s.GetString() == other.GetString());
    }

    bool operator== (PCXSTR psz) const
    {
        return (m_s == psz);
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
        return (m_s < s);
    }

    bool operator< (const CPooledStringT& other) const
    {
        return (m_s < other.m_s);
    }

    bool operator< (PCXSTR psz) const
    {
        return (m_s < psz);
    }


    /*+-------------------------------------------------------------------------*
     * CPooledStringT::operator PCXSTR
     * CPooledStringT::operator const StringType&
     *
     * Casting operators
     *-----------------------------------------------------------------(jeffro)-*/
    operator PCXSTR() const
    {
        return (m_s);
    }

    operator const StringType&() const
    {
        return (m_s);
    }


    /*+-------------------------------------------------------------------------*
     * CPooledStringT::GetString
     * CPooledStringT::GetStringObject
     *
     * Allow access to the contained string as an object or a character pointer.
     *-----------------------------------------------------------------(jeffro)-*/
    PCXSTR GetString() const
    {
        return (m_s);
    }

    const StringType& GetStringObject() const
    {
        return (m_s);
    }

    int GetLength() const throw()
    {
        return (m_s.GetLength());
    }

    bool IsEmpty() const throw()
    {
        return (m_s.IsEmpty());
    }

    /*+-------------------------------------------------------------------------*
     * CPooledStringT::GetPoolSize
     *
     * Returns the number of strings in the string pool.  Mostly useful for
     * debugging purposes.
     *-----------------------------------------------------------------(jeffro)-*/
    static size_t GetPoolSize()
    {
        return (s_pool.size());
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

        for (it = s_pool.begin(); it != s_pool.end(); ++it)
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

        strMessage.Format (strFormat, s_pool.size());
        OutputDebugString (strMessage);

        /*
         * copy from a hash map to a normal map so strings will be sorted
         * alphabetically instead of by hash value
         */
        typedef std::map<StringType, size_t> SortedPoolType;
        SortedPoolType sortedPool;
        std::copy (s_pool.begin(), s_pool.end(), std::inserter(sortedPool, sortedPool.end()));

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
     * CPooledStringT::InitFrom
     *
     *
     *-----------------------------------------------------------------(jeffro)-*/
    void InitFrom (const StringType& s)
    {
        m_s = s;
        AddRef();
    }

    void InitFrom (typename PoolType::iterator it)
    {
        m_s = it->first;
        it->second++;
    }


    /*+-------------------------------------------------------------------------*
     * CPooledStringT::CopyFrom
     *
     *
     *-----------------------------------------------------------------(jeffro)-*/
    void CopyFrom (const StringType& s)
    {
        if (m_s != s)
        {
            Release();
            InitFrom (s);
        }
    }


    /*+-------------------------------------------------------------------------*
     * CPooledStringT::AddRef
     *
     *
     *-----------------------------------------------------------------(jeffro)-*/
    void AddRef()
    {
        s_pool[m_s]++;
    }


    /*+-------------------------------------------------------------------------*
     * CPooledStringT::Release
     *
     *
     *-----------------------------------------------------------------(jeffro)-*/
    void Release()
    {
        PoolType::iterator it = s_pool.find (m_s);

        /*
         * last ref on this pool string?  remove it from the pool
         */
        if (it != s_pool.end())
        {
            if (--it->second == 0)
            {
                VSASSERT (it != s_itEmptyString, "shouldn't erase the empty string (see GetEmptyStringIterator)");
                s_pool.erase (it);
            }
        }
        else
        {
            VSFAIL ("string should be in the pool");
        }

        m_s.Empty();
    }


    /*+-------------------------------------------------------------------------*
     * CPooledStringT::Lookup
     *
     * Finds the given string in the pool (adding if necessary) and returns an
     * iterator to the string's entry in the pool.
     *-----------------------------------------------------------------(jeffro)-*/
    static typename PoolType::iterator Lookup (const StringType& s)
    {
        /*
         * optimze the common case of looking up an empty string
         */
        if (s.IsEmpty())
            return (GetEmptyStringIterator());

        /*
         * Insert the string.  If the string was already in the pool,
         * pr.first will be an iterator to its existing entry; if it's 
         * a new string, pr.first will be an iterator to the new entry
         */
        std::pair<PoolType::iterator, bool> pr = s_pool.insert (std::make_pair(s,0));
        PoolType::iterator itEntry   = pr.first;
        bool               fInserted = pr.second;

        /*
         * make sure that for a new item the refcount in zero and for
         * an existing item the refcount is non-zero
         */
        VSASSERT (( fInserted && (itEntry->second == 0)) ||
                  (!fInserted && (itEntry->second >  0)),
                  "Unexpected refcount for string pool entry");

        return (itEntry);
    }


    /*+-------------------------------------------------------------------------*
     * CPooledStringT::GetEmptyStringIterator
     *
     * Returns an iterator to the entry in the pool for the empty string, inserting
     * an empty string if necessary.  Profiling indicated that we spent a lot of
     * time in Lookup searching for empty strings.  We can avoid this by caching
     * the iterator for the common empty string.
     *-----------------------------------------------------------------(jeffro)-*/
    static typename PoolType::iterator GetEmptyStringIterator()
    {
        /*
         * if we haven't yet inserted an empty string in the pool yet,
         * do so now with a refcount of 1, so it will never be erased
         */
        if (s_itEmptyString == s_pool.end())
            s_itEmptyString = s_pool.insert(std::make_pair(StringType(),1)).first;

        return (s_itEmptyString);
    }


private:
    StringType  m_s;

    static typename PoolType             s_pool;
    static typename PoolType::iterator   s_itEmptyString;


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

#define DECLARE_POOLEDSTRING_STATICS(PooledStringType)  \
    POOLEDSTRINGDECL PooledStringType::PoolType             PooledStringType::s_pool; \
    POOLEDSTRINGDECL PooledStringType::PoolType::iterator   PooledStringType::s_itEmptyString = PooledStringType::s_pool.end(); \
    POOLEDSTRINGDECL PooledStringType::StringType           PooledStringType::CCachedInitString::s_cache;

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
    return (hash_value (s.GetStringObject()));
}

_STDEXT_END
