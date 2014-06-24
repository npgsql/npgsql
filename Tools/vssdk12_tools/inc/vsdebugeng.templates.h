/*++

Copyright (c) Microsoft Corporation. All rights reserved.

Module Name:

    vsdebugeng.templates.h

Abstract:

    This file includes templates to make it easier to correctly free resources 
    when using the Visual Sudio Debugger Engine (debugger back end) API.

--*/

#pragma once

#ifndef __cplusplus
    #error This file requires C++ compilation (use a .cpp suffix)
#endif

#if defined(_MANAGED) || defined(__cplusplus_cli)
    #error This file should not be included in code copiled with /clr. Reference the managed assembly instead.
#endif

#include <atlbase.h>

namespace Microsoft {
namespace VisualStudio {
namespace Debugger {

template <class T>
class _NoAddRefReleaseCloseDkmPtr : public T
{
private:
    STDMETHOD_(ULONG, AddRef)()=0;
    STDMETHOD_(ULONG, Release)()=0;

    DECLSPEC_NOTHROW void STDMETHODCALLTYPE Close();   
};

/// ----------------------------------------------------------------------------
/// <summary>
/// CAutoDkmClosePtr holds onto a pointer to a DKM class. This is useful for the
/// owner of a resource to automaticially close the resource.
/// </summary>
template <class T>
class CAutoDkmClosePtr
{
private:
    // Copying a CAutoDkmClosePtr is almost always a bug. To transfer ownership
    // of the resource, use 'pNewOwner.Attach( pOldOwner.Detach() );'
    CAutoDkmClosePtr(const CAutoDkmClosePtr<T>& p);
    CAutoDkmClosePtr<T>& operator=(const CAutoDkmClosePtr<T>& p);

public:
    CAutoDkmClosePtr() : m_p(NULL)
    {}
    explicit CAutoDkmClosePtr(_In_opt_ T* p) : m_p(p)
    {
        if (m_p)
        {
            m_p->AddRef();
        }
    }
    ~CAutoDkmClosePtr()
    {
        if (m_p)
        {
            m_p->Close();
            m_p->Release();
        }
    }
    _Ret_opt_ operator T*() const
    {
        return (T*)m_p;
    }
    _Ret_ T** operator&()
    {
        ATLASSERT(m_p == NULL);
        return &m_p;
    }
    _Ret_ _NoAddRefReleaseCloseDkmPtr<T>* operator->() const
    {
        ATLASSERT(m_p != NULL);
        return (_NoAddRefReleaseCloseDkmPtr<T>*)m_p;
    }
    void Close()
    {
        T* pTemp = m_p;
        if (pTemp)
        {
            m_p = NULL;
            pTemp->Close();
            pTemp->Release();
        }
    }
    // Release reference to the Dkm object without closing it. This is useful when the responsibility
    // for closing the object is now in the hands of a different peice of code.
    void ReleaseWithoutClose()
    {
        T* pTemp = m_p;
        if (pTemp)
        {
            m_p = NULL;
            pTemp->Release();
        }
    }
    _Ret_opt_ T* operator=(_In_opt_ T* p)
    {
        if (p)
            p->AddRef();
        if (m_p)
            m_p->Release();
        m_p = p;
        return m_p;
    }
    bool operator==(_In_opt_ T* p) const
    {
        return m_p == p;
    }
    bool operator!=(_In_opt_ T* p) const
    {
        return m_p != p;
    }
    // Attach to an existing interface (does not AddRef)
    void Attach(_In_opt_ T *p)
    {
        Close();
        m_p = p;
    }
    // Detach from an existing interface (does not Release)
    _Ret_opt_ T* Detach()
    {
        T * p = m_p;
        m_p = NULL;
        return p;
    }
    template <class Q>
    HRESULT QueryInterface(_Deref_out_ Q** pp) const
    {
        ATLASSERT(pp != NULL);
        return m_p->QueryInterface(__uuidof(Q), (void**)pp);        
    }

    HRESULT CopyTo(_Deref_out_opt_ T **ppT) const
    {
        ATLASSERT(ppT != NULL);
        if (ppT == NULL)
            return E_POINTER;
        *ppT = m_p;
        if (m_p)
            m_p->AddRef();
        return S_OK;
    }

    T* m_p;
};

template <class T>
class CAutoDkmArray : public DkmArray<T>
{
private:
    // Copying a DkmArray is not supported by this template. To transfer ownership, consider
    // using Detach/Attach
    CAutoDkmArray(const CAutoDkmArray<T>& p);
    CAutoDkmArray<T>& operator=(const CAutoDkmArray<T>& p);
    CAutoDkmArray(const DkmArray<T>& p);
    CAutoDkmArray<T>& operator=(const DkmArray<T>& p);
public:
    CAutoDkmArray()
    {
        this->Length = 0;
        this->Members = NULL;
    }
    ~CAutoDkmArray()
    {
        if (this->Members != NULL)
        {
            DkmFreeArray(*this);           
        }
    }
    _Ret_ DkmArray<T>* operator&()
    {
        ATLASSERT(this->Length == 0 && this->Members == NULL);
        return this;
    }
    void Free()
    {
        DkmArray<T> Temp = *this;
        
        this->Length = 0;
        this->Members = NULL;

        if (Temp.Members != NULL)
        {            
            DkmFreeArray(Temp);
        }
    }
    bool operator==(_In_opt_ T* p) const
    {
        return this->Members == p;
    }
    bool operator!=(_In_opt_ T* p) const
    {
        return this->Members != p;
    }
    // Attach to an existing array (does not allocate)
    void Attach(const DkmArray<T>& a)
    {
        Free();

        this->Length = a.Length;
        this->Members = a.Members;
    }
    // Detach from an existing array (does not free)
    DkmArray<T> Detach()
    {
        DkmArray<T> Temp = *this;
        
        this->Length = 0;
        this->Members = NULL;

        return Temp;
    }
}; // end of CAutoDkmArray

template <class T>
class CAutoDkmStruct : public T
{
private:
    // Copying a struct is not supported by this template. To transfer ownership, consider
    // using Detach/Attach
    CAutoDkmStruct(const CAutoDkmStruct<T>& p);
    CAutoDkmStruct<T>& operator=(const CAutoDkmStruct<T>& p);
    CAutoDkmStruct(const T& p);
    CAutoDkmStruct<T>& operator=(const T& p);
public:
    CAutoDkmStruct()
    {
        memset(this, 0, sizeof(*this));
    }
    ~CAutoDkmStruct()
    {
        T::Release(this);
    }
    _Ret_ T* operator&()
    {
        ATLASSERT(*this == NULL);
        return this;
    }
    void Release()
    {
        T Temp;

        memcpy(&Temp, this, sizeof(*this));        
        memset(this, 0, sizeof(*this));

        T::Release(&Temp);
    }

    ///<summary>
    ///Compare the struct against zero. Returns true if the structure is still zero.
    ///</summary>
    ///<example>
    ///CAutoDkmStruct<DkmClrLocalConstant> s; 
    ///CallExampleMethod(&s); 
    ///if (s == NULL)
    ///   printf("Example Method did not initialize 's'\n");
    ///</example>
    bool operator==(nullptr_t) const
    {
        T zero; memset(&zero, 0, sizeof(zero));

        return (memcmp(&zero, this, sizeof(zero)) == 0);
    }

    ///<summary>
    ///Compare the struct against zero. Returns true if the structure is no longer zero.
    ///</summary>
    ///<example>
    ///CAutoDkmStruct<DkmClrLocalConstant> s; 
    ///CallExampleMethod(&s); 
    ///if (s != NULL)
    ///   printf("Example method initialized 's'\n");
    ///</example>
    bool operator!=(nullptr_t) const
    {
        return !((*this) == NULL);
    }

    // Attach to an existing structure (does not AddRef)
    void Attach(const T& newValue)
    {
        T oldValue;

        memcpy(&oldValue, this, sizeof(*this));        
        memcpy(this, &newValue, sizeof(*this));

        T::Release(&oldValue);
    }

    // Detach from an existing structure (does not Release)
    T Detach()
    {
        T oldValue;

        memcpy(&oldValue, this, sizeof(*this));        
        memset(this, 0, sizeof(*this));

        return oldValue;
    }
}; // end of CAutoDkmStruct

class _Restricted_DkmWorkList : public DkmWorkList
{
private:
    STDMETHOD_(ULONG, AddRef)()=0;
    STDMETHOD_(ULONG, Release)()=0;

    // Call pWorkList.BeginExecution/Execute/Cancel insteead of pWorkList->BeginExecution/Execute/Cancel
    DECLSPEC_NOTHROW HRESULT STDMETHODCALLTYPE BeginExecution();
    DECLSPEC_NOTHROW HRESULT STDMETHODCALLTYPE Execute();
    DECLSPEC_NOTHROW HRESULT STDMETHODCALLTYPE Cancel();
};

/// ----------------------------------------------------------------------------
/// <summary>
/// CAutoDkmWorkListPtr holds a wrapper around a 'DkmWorkList*' to automaticially cancel the
/// work list if Execute/BeginExecute are not called.
/// </summary>
class CAutoDkmWorkListPtr
{
private:
    // Copying a CAutoDkmWorkListPtr is almost always a bug. To transfer ownership
    // of the resource, use 'pNewOwner.Attach( pOldOwner.Detach() );'
    CAutoDkmWorkListPtr(const CAutoDkmWorkListPtr& p);
    CAutoDkmWorkListPtr& operator=(const CAutoDkmWorkListPtr& p);

public:
    CAutoDkmWorkListPtr()
    {}
    explicit CAutoDkmWorkListPtr(DkmWorkList* p) : m_p(p)
    {
    }
    ~CAutoDkmWorkListPtr()
    {
        Cancel();
    }
    _Ret_opt_ operator DkmWorkList*() const
    {
        return m_p;
    }
    _Ret_ DkmWorkList** operator&()
    {
        return &m_p;
    }
    _Ret_ _Restricted_DkmWorkList* operator->() const
    {
        ATLASSERT(m_p != NULL);
        return (_Restricted_DkmWorkList*)static_cast<DkmWorkList*>(m_p);
    }
    HRESULT Cancel()
    {
        if (m_p != NULL)
        {
            ATL::CComPtr<DkmWorkList> pTemp;
            pTemp.Attach( m_p.Detach() );

            return pTemp->Cancel();
        }
        else
        {
            return S_OK;
        }
    }
    HRESULT BeginExecution()
    {
        ATLASSERT(m_p != NULL);
        if (m_p != NULL)
        {
            ATL::CComPtr<DkmWorkList> pTemp;
            pTemp.Attach( m_p.Detach() );

            return pTemp->BeginExecution();
        }
        else
            return E_UNEXPECTED;
    }
    HRESULT Execute()
    {
        ATLASSERT(m_p != NULL);
        if (m_p != NULL)
        {
            ATL::CComPtr<DkmWorkList> pTemp;
            pTemp.Attach( m_p.Detach() );

            return pTemp->Execute();
        }
        else
            return E_UNEXPECTED;
    }
    _Ret_opt_ DkmWorkList* operator=(_In_opt_ DkmWorkList* p)
    {
        m_p = p;
        return m_p;
    }
    bool operator==(_In_opt_ DkmWorkList* p) const
    {
        return m_p == p;
    }
    bool operator!=(_In_opt_ DkmWorkList* p) const
    {
        return m_p != p;
    }
    // Attach to an existing interface (does not AddRef)
    void Attach(_In_opt_ DkmWorkList *p)
    {
        Cancel();
        m_p.Attach(p);
    }
    // Detach from an existing interface (does not Release)
    _Ret_opt_ DkmWorkList* Detach()
    {
        return m_p.Detach();
    }

    ATL::CComPtr<DkmWorkList> m_p;
};


};
};
};
