/*--------------------------------------------------------------------------*
 *
 *  Microsoft Visual Studio
 *  Copyright (C) Microsoft Corporation, 1995 - 2008
 *
 *  File:       CookieTable.h
 *
 *  Contents:   Interface file for CCookieTable
 *
 *  History:    20-Jan-2008 jeffro    Created
 *
 *--------------------------------------------------------------------------*/

#pragma once

#include <map>
#include <algorithm>
#include <iterator>
#include <atlcomcli.h>              // for CComPtr, CAdapt
#include "vsshell.h"                // for VSCOOKIE, VSCOOKIE_NIL

#pragma push_macro("min")
#undef min

/*+-------------------------------------------------------------------------*
 * DefaultCookieTraits
 *
 * This class defines the default type, range, and issuance policy for
 * cookies handed out by CCookieTable.
 *-----------------------------------------------------------------(jeffro)-*/
template<class CookieType         = VSCOOKIE,
         CookieType Min           = 1,
         CookieType Max           = 0xffffffff,
         CookieType Invalid       = VSCOOKIE_NIL,
         bool       AllowRollover = true>
struct DefaultCookieTraits
{
    typedef CookieType cookie_type;

    static const cookie_type MinCookie     = Min;
    static const cookie_type MaxCookie     = Max;
    static const cookie_type InvalidCookie = Invalid;

    /*+-------------------------------------------------------------------------*
     * state_type
     *
     * This structure contains the state used by this type when generating a 
     * new cookie.
     *-----------------------------------------------------------------(jeffro)-*/
    struct state_type
    {
        state_type() 
            : lastCookieUsed (InvalidCookie)
            , fRolledOver    (false)
        {}
        
        cookie_type lastCookieUsed;
        bool        fRolledOver;
    };


    /*+-------------------------------------------------------------------------*
     * UniqueCookieCount
     *
     * Returns the maximum number of unique cookies that this traits class
     * can issue.
     *-----------------------------------------------------------------(jeffro)-*/
    static size_t UniqueCookieCount()
    {
        return (MaxCookie - MinCookie + 1);
    }


    /*+-------------------------------------------------------------------------*
     * NextCookie
     *
     * Generates a new cookie for the given table.
     *-----------------------------------------------------------------(jeffro)-*/
    template<class T>
    static cookie_type NextCookie (const T& table, state_type& state)
    {
        switch (state.lastCookieUsed)
        {
            case InvalidCookie:
                state.lastCookieUsed = MinCookie;
                break;

            case MaxCookie:
                state.lastCookieUsed = MinCookie;
                state.fRolledOver    = true;
                // fall through to default
                __fallthrough;
            default:
                /*
                 * if we haven't rolled over yet (common case), things are 
                 * easy -- just increment the cookie
                 */
                if (!state.fRolledOver)
                    state.lastCookieUsed++;

                /*
                 * if we have rolled over but the class doesn't support
                 * issuing cookies beyond that, throw an error
                 */
                else if (!AllowRollover)
                    throw (std::bad_alloc());

                /*
                 * otherwise iterate until we find one that's not in use
                 */
                else
                {
                    /*
                     * Do a quick test to see if the table is full.  This
                     * is highly unlikely unless cookie_type is a narrow
                     * data type or UniqueCookieCount is small.
                     */
                    if (table.pending_size() == table.max_size())
                        throw (std::bad_alloc());

                    /*
                     * Loop until we find a cookie that's not in use.  This is
                     * O(n), but it's highly unlikely we'll ever get this far.
                     */
                    while (table.pending_find (state.lastCookieUsed))
                    {
                        state.lastCookieUsed++;

                        /*
                         * we should never get to MaxCookie (pending_size() == max_size())
                         * protects us
                         */
                        if (state.lastCookieUsed == MaxCookie)
                        {
                            VSASSERT (false, "Should have found a cookie");
                            throw (std::bad_alloc());
                        }
                    }
                }
                break;
        }

        return (state.lastCookieUsed);
    }
};


/*+-------------------------------------------------------------------------*
 * DefaultValueTraits
 *
 * This class defines what an invalid value for a CCookieTable is, and the
 * method of invalidating values.
 *-----------------------------------------------------------------(jeffro)-*/
template<class T>
struct DefaultValueTraits
{
    /*
     * implement Invalidate if you need to detect an element
     * that has been erased during iteration
     */
    static void Invalidate (T&)
        {}

    /*
     * if you implement Invalidate, you should also implement
     * IsValid to identify an element that was invalidated
     */
    static bool IsValid (const T&)
        { return (true); }
};

// specialization for CComPtr<T>
template<class T>
struct DefaultValueTraits<CComPtr<T>>
{
    static void Invalidate (CComPtr<T>& spT)
    {
        spT.Release();
    }

    static bool IsValid (const CComPtr<T>& spT)
    {
        return (spT != NULL);
    }
};

// specialization for CAdapt<CComPtr<T>>
template<class T>
struct DefaultValueTraits<CAdapt<CComPtr<T>>>
{
    static void Invalidate (CAdapt<CComPtr<T>>& adapt)
    {
        CComPtr<T>& spT = adapt;
        spT.Release();
    }

    static bool IsValid (const CAdapt<CComPtr<T>>& adapt)
    {
        const CComPtr<T>& spT = adapt;
        return (spT != NULL);
    }
};


/*+-------------------------------------------------------------------------*
 * CCookieTable
 *
 * This class is intended to be used to support a registration model where
 * a client registers an object (commonly a COM interface) with a service
 * and receives a unique cookie for the registration in return.  The cookie
 * is subsequently used as a key when the object is unregistered.
 * 
 * This is an STL-style collection, exposing begin() and end() methods to
 * provide boundaries for iteration.  If there is a possiblity that the
 * table could be modified during the iteration, it is important to lock
 * the table before acquiring iterators.  The table can be locked with the
 * lock() method (and unlocked with unlock()), or by using the CCookieTable::Lock
 * RAII helper.
 * 
 * While the table is locked, the range of items covered by begin()/end()
 * is a snapshot representing the state of the table at the time it was
 * locked.  Additions or deletions from the table while it is locked are
 * held in a pending modifications list, which are applied to the table
 * when it is unlocked.  If an element is erased from the table while it
 * is locked, it is invalidated by ValueTraits::Invalidate; you can use
 * ValueTraits::IsValid during iteration to avoid using elements that were
 * erased during iteration.
 * 
 * NOTE:  This class provides the same thread-safety guarantees of standard 
 * STL collections:  it is thread-safe only in the sense that simultaneous 
 * accesses to distinct instances are safe, and simultaneous read accesses 
 * to to shared shared are safe. If multiple threads access a single 
 * instance and at least one thread may potentially write, then the user 
 * is responsible for ensuring mutual exclusion between the threads during
 * the accesses. 
 *-----------------------------------------------------------------(jeffro)-*/
template<class T, 
         class CookieTraits = DefaultCookieTraits<>,
         class ValueTraits  = DefaultValueTraits<T> >
class CCookieTable
{
public:
    typedef CCookieTable<T, CookieTraits, ValueTraits>  this_type;
    typedef CookieTraits                        CookieTraits;
    typedef ValueTraits                         ValueTraits;
    typedef typename CookieTraits::cookie_type  cookie_type;

private:
    typedef std::map<cookie_type, T>            map_type;

public:
    typedef typename map_type::mapped_type      mapped_type;
    typedef typename map_type::value_type       value_type;
    typedef typename map_type::size_type        size_type;
    typedef typename map_type::iterator         iterator;
    typedef typename map_type::const_iterator   const_iterator;

    static const cookie_type InvalidCookie = CookieTraits::InvalidCookie;

    class Lock
    {
    public:
        Lock (this_type& table)  : m_table (table)
            { m_table.lock(); }

        Lock (this_type* pTable) : m_table (*pTable)
            { m_table.lock(); }

        ~Lock()
            { m_table.unlock(); }

    private:
        this_type&  m_table;

        Lock            (const Lock& other);  // not copy-assignable
        Lock& operator= (const Lock& other);  // not assignable
    };

public:
    CCookieTable()
        : m_cLocks (0)
    {
        VSCASSERT (CookieTraits::MinCookie < CookieTraits::MaxCookie,
                  Min_max_cookies_in_CookeTraits_are_out_of_range);

        VSCASSERT ((CookieTraits::InvalidCookie < CookieTraits::MinCookie) ||
                   (CookieTraits::InvalidCookie > CookieTraits::MaxCookie),
                   CookieTraits_defines_the_invalid_cookie_between_min_max_cookies);
    }


    /*+-------------------------------------------------------------------------*
     * CCookieTable::begin
     *
     * Returns a bi-directional iterator addressing the first element in the 
     * table.  If the table is empty, the iterator addresses the location 
     * following the empty table.
     *-----------------------------------------------------------------(jeffro)-*/
    iterator begin()
        { return (m_map.begin()); }

    const_iterator begin() const
        { return (m_map.begin()); }


    /*+-------------------------------------------------------------------------*
     * CCookieTable::end
     *
     * Returns a bi-directional iterator that addresses the location following 
     * the last element in the table.
     *-----------------------------------------------------------------(jeffro)-*/
    iterator end()
        { return (m_map.end()); }

    const_iterator end() const
        { return (m_map.end()); }


    /*+-------------------------------------------------------------------------*
     * CCookieTable::insert
     *
     * Adds an element to the table.  If the table is locked when this function
     * is called, the element won't be physically added to the table until the
     * lock count goes to zero.
     *
     * Returns the cookie assigned to the new element.
     *-----------------------------------------------------------------(jeffro)-*/
    cookie_type insert (const mapped_type& element)
    { 
        /*
         * Lock the table before insertion.  Doing this now means we only have
         * a single code path for inserting items, regardless of whether the
         * table is externally locked or not.  The new element will actually
         * be added to the table once its lock count goes to zero.  If the
         * table isn't locked with this function is called, that time will be
         * when this function returns.
         */
        Lock lock(this);

        cookie_type cookie = CookieTraits::NextCookie (*this, m_CookieTraitsState);
        VSASSERT (!pending_find (cookie), "Reissuing a cookie that's in use");

        add_pending_insert (cookie, element);

        return (cookie);
    }


    /*+-------------------------------------------------------------------------*
     * CCookieTable::erase
     *
     * Removes the element matching the specified cookie.  If the table is 
     * locked when this function is called, the removal won't take effect 
     * until the table's lock count goes to zero.
     *
     * Returns a boolean indicating success.
     *-----------------------------------------------------------------(jeffro)-*/
    bool erase (const cookie_type& cookie)
    {
        bool fErased = false;

        /*
         * Lock the table before deletion.  Doing this now means we only have
         * a single code path for deleting items, regardless of whether the
         * table is externally locked or not.  The element will actually be
         * be removed from the table once its lock count goes to zero.  If the
         * table isn't locked with this function is called, that time will be
         * when this function returns.
         */
        Lock lock(this);
        VSASSERT (locked(), "Table should be locked");

        /*
         * look for an item in the table proper; if found and it hasn't already
         * been previously erased, invalidate it and add the cookie to the set of 
         * cookies to erase when unlocked
         */
        iterator it = find (cookie);
        if ((it != end()) && !find_pending_erasure (cookie))
        {
            ValueTraits::Invalidate (it->second);
            add_pending_erasure (cookie);
            fErased = true;
        }

        /*
         * if we didn't find the item in the table proper, see if it's in the
         * pending inserts
         */
        if (!fErased && find_pending_insert (cookie))
        {
            m_spPendingMods->m_inserts.erase (cookie);
            fErased = true;
        }

        return (fErased);
    }


    /*+-------------------------------------------------------------------------*
     * CCookieTable::clear
     *
     * Removes all elements from the table.  If the table is locked when this
     * function is called, the clear won't take effect until the table's lock
     * count goes to zero.
     *-----------------------------------------------------------------(jeffro)-*/
    void clear()
    { 
        /*
         * Lock the table before clearing.  Doing this now means we only have
         * a single code path for clearing items, regardless of whether the
         * table is externally locked or not.  The table will actually be 
         * cleared once its lock count goes to zero.  If the table isn't 
         * locked with this function is called, that time will be when this
         * function returns.
         */
        Lock lock(this);

        /*
         * invalidate all of the items in the table
         */
        std::for_each (begin(), end(), Invalidate());

        ensure_pending_mods();
        m_spPendingMods->m_clear = true;

        /*
         * now that the table has been cleared, we no longer have any pending
         * inserts/erasures
         */
        m_spPendingMods->m_inserts.clear();
        m_spPendingMods->m_erasures.clear();
    }

    struct Invalidate : public std::unary_function<value_type, void>
    {
        void operator()(value_type& pair) const
        {
            ValueTraits::Invalidate (pair.second);
        }
    };


    /*+-------------------------------------------------------------------------*
     * CCookieTable::find
     *
     * Returns an iterator addressing the location of an element with the 
     * specified cookie.  If no such element exists, the iterator addresses the 
     * location following the last element in the table.
     *
     * If this function is called when the table is locked, the find operation
     * will not take into account any changes made to the table while locked
     * (see pending_find()).
     *-----------------------------------------------------------------(jeffro)-*/
    iterator find (const cookie_type& cookie)
        { return (m_map.find (cookie)); }

    const_iterator find (const cookie_type& cookie) const
        { return (m_map.find (cookie)); }


    /*+-------------------------------------------------------------------------*
     * CCookieTable::pending_find
     *
     * Returns a boolean indicating whether the given cookie is in use in the 
     * table, including all pending modifications to the table.  
     * 
     * If the table is unlocked with this function is called then the return 
     * value for this function is identical to (find(cookie) != end()).
     *
     * If the table is locked, the return value indicates the what the value
     * of (find(cookie) != end()) will be when all pending changes are applied.
     *-----------------------------------------------------------------(jeffro)-*/
    bool pending_find (const cookie_type& cookie) const
    { 
        bool found = (m_map.find (cookie) != end());       // assume unlocked

        /*
         * if we have pending modifications to the table, there's more work to do
         */
        if (have_pending_mods())
        {
            /*
             * if we found the cookie in the table proper, then its pending-
             * find state will only be true if there's not a pending clear
             * or a pending erasure
             */
            if (found)
                found = !m_spPendingMods->m_clear && !find_pending_erasure(cookie);

            /* 
             * otherwise, the cookie wasn't found in the table proper; its
             * pending-find state is only true if there's a pending insert
             * for the item
             */
            else
                found = find_pending_insert (cookie);
        }

        return (found);
    }


    /*+-------------------------------------------------------------------------*
     * CCookieTable::empty
     *
     * Returns a boolean indicating whether the table is empty.  If the table is 
     * locked when this function is called, the value returned represents the
     * table's state at the time it was locked.
     *-----------------------------------------------------------------(jeffro)-*/
    bool empty() const
        { return (m_map.empty()); }


    /*+-------------------------------------------------------------------------*
     * CCookieTable::size
     *
     * Returns the number of elements in the table.  If the table is locked
     * when this function is called, the value returned is the size of the
     * table at the time it was locked.
     *-----------------------------------------------------------------(jeffro)-*/
    size_type size() const
        { return (m_map.size()); }


    /*+-------------------------------------------------------------------------*
     * CCookieTable::pending_size
     *
     * Returns the number of elements that will be in the table when it is
     * unlocked, including all pending modifications to the table.  
     *
     * If the table is unlocked when this function is called, this is identical
     * to the value returned by size().
     *-----------------------------------------------------------------(jeffro)-*/
    size_type pending_size() const
    {
        size_type count = size();       // assume unlocked

        if (have_pending_mods())
        {
            /*
             * if the table was cleared while it was locked, the size when
             * unlocked will be the number of elements added to the table
             * since it was last cleared (to-erase collection should be empty)
             */
            if (m_spPendingMods->m_clear)
            {
                VSASSERT (m_spPendingMods->m_erasures.size() == 0, "Unexpected pending erasures");
                count =   m_spPendingMods->m_inserts.size();
            }
            else
            {
                /*
                 * decrement the count before incrementing to avoid overflow 
                 * (underflow isn't a concern)
                 */
                VSASSERT (m_spPendingMods->m_erasures.size() <= count, "Too many pending erasures");
                count -=  m_spPendingMods->m_erasures.size();
                count +=  m_spPendingMods->m_inserts.size();
            }
        }

        return (count); 
    }


    /*+-------------------------------------------------------------------------*
     * CCookieTable::for_each
     *
     * Calls a function for each elements in the table, optionally skipping 
     * invalid elements.
     *-----------------------------------------------------------------(jeffro)-*/
    template<class Func>
    Func for_each (Func func, bool fIncludeInvalidElements = false)
    {
        /*
         * micro-optimization: short out without locking if the table is empty
         */
        if (empty())
            return (func);

        /*
         * lock the table so changes potentially made by the callback function 
         * don't invalidate our iterators
         */
        Lock lock(this);

        for (const_iterator it = begin(); it != end(); ++it)
        {
            const cookie_type& cookie = it->first;
            const mapped_type& value  = it->second;

            if (fIncludeInvalidElements || ValueTraits::IsValid(value))
            {
                func (cookie, value);
            }
        }

        return (func);
    }


    /*+-------------------------------------------------------------------------*
     * CCookieTable::max_size
     *
     * Returns the largest number of elements this table can hold.  This value
     * will be limited by the lesser of the maximum number of elements that 
     * the underlying map can hold and the cookie range specified by the 
     * CookieTraits class.
     *-----------------------------------------------------------------(jeffro)-*/
    size_type max_size() const
    { 
        size_type cookie_count = CookieTraits::UniqueCookieCount();
        return (std::min (m_map.max_size(), cookie_count));
    }


    /*+-------------------------------------------------------------------------*
     * CCookieTable::lock
     *
     * Locks the table so subsequent insertions and deletions won't invalidate
     * iterators into the table.
     * 
     * NOTE: Locking the table DOES NOT provide exclusive access to the locking
     * thread.  There is NO thread safety implied by this function.  If exclusive
     * access to an instance of the table is required, it is the user's 
     * responsibility to provide it.
     *-----------------------------------------------------------------(jeffro)-*/
    size_type lock()
    {
        /*
         * bump the lock count
         */
        m_cLocks++;

        VSASSERT (locked(), "Table should be locked");
        return (m_cLocks);
    }


    /*+-------------------------------------------------------------------------*
     * CCookieTable::locked
     *
     * Returns a boolean indicating whether the table is locked or not.
     *-----------------------------------------------------------------(jeffro)-*/
    bool locked() const
    {
        /*
         * If we're not locked, we shouldn't have a modification tracking structure.
         * Note that the converse is not true:  If we're locked, we may or may not
         * have a modification tracking structure, depending on whether insert,
         * erase, or clear has been called while locked.
         */
        VSASSERT ((m_cLocks > 0) || !have_pending_mods(),
                  "Shouldn't have pending mods if we're not locked");

        return (m_cLocks > 0);
    }


    /*+-------------------------------------------------------------------------*
     * CCookieTable::unlock
     *
     * Decrements the lock count on the table.  If the lock count goes to zero,
     * any changes to the table made while the table was locked are applied.
     *-----------------------------------------------------------------(jeffro)-*/
    size_type unlock()
    {
        /*
         * Shouldn't be unlocking if not locked.  Consider using the Lock class
         * above to automate locking semantics.
         */
        VSASSERT (locked(), "Table should be locked");

        /*
         * if this is the final unlock, apply the pending changes
         */
        if (locked() && (--m_cLocks == 0) && have_pending_mods())
        {
            /*
             * apply pending clear/erasures
             */
            if (m_spPendingMods->m_clear)
            {
                m_map.clear();
            }
            else
            {
                std::for_each (m_spPendingMods->m_erasures.begin(), 
                               m_spPendingMods->m_erasures.end(),
                               Erase (m_map));
            }

            /*
             * apply pending inserts
             */
            std::copy (m_spPendingMods->m_inserts.begin(), 
                       m_spPendingMods->m_inserts.end(),
                       std::inserter (m_map, m_map.end()));

            /*
             * let go of the tracking structure
             */
            m_spPendingMods.reset();
            VSASSERT (!locked(), "Table should be unlocked");
        }

        return (m_cLocks);
    }

    struct Erase : public std::unary_function<cookie_type, void>
    {
        Erase (map_type& table) : m_pTable (&table) 
            {}

        void operator()(const cookie_type& cookie) const
            { m_pTable->erase (cookie); }

    private:
        map_type*  m_pTable;
    };


private:
    /*+-------------------------------------------------------------------------*
     * CCookieTable::find_pending_insert
     *
     * Returns a boolean indicating whether the given cookie is in the pending 
     * add collection.
     *-----------------------------------------------------------------(jeffro)-*/
    bool find_pending_insert (cookie_type cookie) const
    {
        if (!have_pending_mods())
            return (false);

        return (m_spPendingMods->m_inserts.find(cookie) != m_spPendingMods->m_inserts.end());
    }


    /*+-------------------------------------------------------------------------*
     * CCookieTable::add_pending_insert
     *
     * Adds an entry to the pending inserts collection, creating the PendingMods
     * structure if necessary.
     *-----------------------------------------------------------------(jeffro)-*/
    void add_pending_insert (cookie_type cookie, const mapped_type& element)
    {
        ensure_pending_mods();
        m_spPendingMods->m_inserts[cookie] = element;
    }


    /*+-------------------------------------------------------------------------*
     * CCookieTable::find_pending_erasure
     *
     * Returns a boolean indicating whether the given cookie is in the pending 
     * erasure collection.
     *-----------------------------------------------------------------(jeffro)-*/
    bool find_pending_erasure (cookie_type cookie) const
    {
        if (!have_pending_mods())
            return (false);

        return (std::find (m_spPendingMods->m_erasures.begin(),
                           m_spPendingMods->m_erasures.end(),
                           cookie) != m_spPendingMods->m_erasures.end());
    }


    /*+-------------------------------------------------------------------------*
     * CCookieTable::add_pending_erasure
     *
     * Adds an entry to the pending erasures collection, creating the PendingMods
     * structure if necessary.
     *-----------------------------------------------------------------(jeffro)-*/
    void add_pending_erasure (cookie_type cookie)
    {
        ensure_pending_mods();
        m_spPendingMods->m_erasures.push_back (cookie);
    }


    /*+-------------------------------------------------------------------------*
     * CCookieTable::ensure_pending_mods
     *
     * Creates the PendingMods structure if we don't have one.
     *-----------------------------------------------------------------(jeffro)-*/
    void ensure_pending_mods()
    {
        VSASSERT (locked(), "Shouldn't need a PendingMods structure unless we're locked");

        if (!have_pending_mods())
            m_spPendingMods = std::auto_ptr<PendingMods>(new PendingMods);
    }


    /*+-------------------------------------------------------------------------*
     * CCookieTable::have_pending_mods
     *
     * Returns a boolean indicating whether we have pending modifications.
     *-----------------------------------------------------------------(jeffro)-*/
    bool have_pending_mods() const
    {
        return (m_spPendingMods.get() != NULL);
    }


private:
    /*+-------------------------------------------------------------------------*
     * PendingMods
     *
     * This data structure tracks changes made to the table while locked.
     *-----------------------------------------------------------------(jeffro)-*/
    struct PendingMods
    {
        PendingMods() : m_clear (false) {}

        typedef map_type                    Inserts;
        typedef std::vector<cookie_type>    Erasures;

        Inserts     m_inserts;
        Erasures    m_erasures;
        bool        m_clear;
    };

    map_type                            m_map;
    std::auto_ptr<PendingMods>          m_spPendingMods;    // used when locked and mods are pending
    size_type                           m_cLocks;
    typename CookieTraits::state_type   m_CookieTraitsState;

    CCookieTable         (const this_type& other);  // not copy-assignable
    this_type& operator= (const this_type& other);  // not assignable
};

#pragma pop_macro("min")