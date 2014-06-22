

/* this ALWAYS GENERATED file contains the definitions for the interfaces */


 /* File created by MIDL compiler version 7.00.0499 */
/* Compiler settings for context.idl:
    Oicf, W0, Zp8, env=Win32 (32b run)
    protocol : dce , ms_ext, c_ext, robust
    error checks: allocation ref bounds_check enum stub_data 
    VC __declspec() decoration level: 
         __declspec(uuid()), __declspec(selectany), __declspec(novtable)
         DECLSPEC_UUID(), MIDL_INTERFACE()
*/
//@@MIDL_FILE_HEADING(  )

#pragma warning( disable: 4049 )  /* more than 64k source lines */


/* verify that the <rpcndr.h> version is high enough to compile this file*/
#ifndef __REQUIRED_RPCNDR_H_VERSION__
#define __REQUIRED_RPCNDR_H_VERSION__ 475
#endif

/* verify that the <rpcsal.h> version is high enough to compile this file*/
#ifndef __REQUIRED_RPCSAL_H_VERSION__
#define __REQUIRED_RPCSAL_H_VERSION__ 100
#endif

#include "rpc.h"
#include "rpcndr.h"

#ifndef __RPCNDR_H_VERSION__
#error this stub requires an updated version of <rpcndr.h>
#endif // __RPCNDR_H_VERSION__

#ifndef COM_NO_WINDOWS_H
#include "windows.h"
#include "ole2.h"
#endif /*COM_NO_WINDOWS_H*/

#ifndef __context_h__
#define __context_h__

#if defined(_MSC_VER) && (_MSC_VER >= 1020)
#pragma once
#endif

/* Forward Declarations */ 

#ifndef __IVsUserContext_FWD_DEFINED__
#define __IVsUserContext_FWD_DEFINED__
typedef interface IVsUserContext IVsUserContext;
#endif 	/* __IVsUserContext_FWD_DEFINED__ */


#ifndef __IVsUserContextUpdate_FWD_DEFINED__
#define __IVsUserContextUpdate_FWD_DEFINED__
typedef interface IVsUserContextUpdate IVsUserContextUpdate;
#endif 	/* __IVsUserContextUpdate_FWD_DEFINED__ */


#ifndef __IVsProvideUserContext_FWD_DEFINED__
#define __IVsProvideUserContext_FWD_DEFINED__
typedef interface IVsProvideUserContext IVsProvideUserContext;
#endif 	/* __IVsProvideUserContext_FWD_DEFINED__ */


#ifndef __IVsProvideUserContextForObject_FWD_DEFINED__
#define __IVsProvideUserContextForObject_FWD_DEFINED__
typedef interface IVsProvideUserContextForObject IVsProvideUserContextForObject;
#endif 	/* __IVsProvideUserContextForObject_FWD_DEFINED__ */


#ifndef __IVsUserContextItemCollection_FWD_DEFINED__
#define __IVsUserContextItemCollection_FWD_DEFINED__
typedef interface IVsUserContextItemCollection IVsUserContextItemCollection;
#endif 	/* __IVsUserContextItemCollection_FWD_DEFINED__ */


#ifndef __IVsUserContextItem_FWD_DEFINED__
#define __IVsUserContextItem_FWD_DEFINED__
typedef interface IVsUserContextItem IVsUserContextItem;
#endif 	/* __IVsUserContextItem_FWD_DEFINED__ */


#ifndef __IVsHelpAttributeList_FWD_DEFINED__
#define __IVsHelpAttributeList_FWD_DEFINED__
typedef interface IVsHelpAttributeList IVsHelpAttributeList;
#endif 	/* __IVsHelpAttributeList_FWD_DEFINED__ */


#ifndef __IVsMonitorUserContext_FWD_DEFINED__
#define __IVsMonitorUserContext_FWD_DEFINED__
typedef interface IVsMonitorUserContext IVsMonitorUserContext;
#endif 	/* __IVsMonitorUserContext_FWD_DEFINED__ */


#ifndef __IVsUserContextItemProvider_FWD_DEFINED__
#define __IVsUserContextItemProvider_FWD_DEFINED__
typedef interface IVsUserContextItemProvider IVsUserContextItemProvider;
#endif 	/* __IVsUserContextItemProvider_FWD_DEFINED__ */


#ifndef __IVsUserContextCustomize_FWD_DEFINED__
#define __IVsUserContextCustomize_FWD_DEFINED__
typedef interface IVsUserContextCustomize IVsUserContextCustomize;
#endif 	/* __IVsUserContextCustomize_FWD_DEFINED__ */


#ifndef __IVsUserContextItemEvents_FWD_DEFINED__
#define __IVsUserContextItemEvents_FWD_DEFINED__
typedef interface IVsUserContextItemEvents IVsUserContextItemEvents;
#endif 	/* __IVsUserContextItemEvents_FWD_DEFINED__ */


#ifndef __VsContextClass_FWD_DEFINED__
#define __VsContextClass_FWD_DEFINED__

#ifdef __cplusplus
typedef class VsContextClass VsContextClass;
#else
typedef struct VsContextClass VsContextClass;
#endif /* __cplusplus */

#endif 	/* __VsContextClass_FWD_DEFINED__ */


/* header files for imported files */
#include "oleidl.h"
#include "servprov.h"
#include "oaidl.h"
#include "ocidl.h"

#ifdef __cplusplus
extern "C"{
#endif 


/* interface __MIDL_itf_context_0000_0000 */
/* [local] */ 

#pragma once







typedef DWORD_PTR VSCOOKIE;

typedef DWORD_PTR VSCONTEXTUPDATECOOKIE;

typedef DWORD_PTR VSSUBCONTEXTCOOKIE;

typedef DWORD_PTR VSINFOPROVIDERCOOKIE;

typedef 
enum tagVsUserContextPriority
    {	VSUC_Priority_None	= 0,
	VSUC_Priority_Ambient	= 100,
	VSUC_Priority_State	= 200,
	VSUC_Priority_Project	= 300,
	VSUC_Priority_ProjectItem	= 400,
	VSUC_Priority_Window	= 500,
	VSUC_Priority_Selection	= 600,
	VSUC_Priority_MarkerSel	= 700,
	VSUC_Priority_Enterprise	= 800,
	VSUC_Priority_WindowFrame	= 900,
	VSUC_Priority_ToolWndSel	= 1000,
	VSUC_Priority_Highest	= 1100
    } 	VSUSERCONTEXTPRIORITY;

typedef 
enum tagVsUserContextAttributeUsage
    {	VSUC_Usage_Filter	= 0,
	VSUC_Usage_Lookup	= 1,
	VSUC_Usage_LookupF1	= 2,
	VSUC_Usage_Lookup_CaseSensitive	= 3,
	VSUC_Usage_LookupF1_CaseSensitive	= 4
    } 	VSUSERCONTEXTATTRIBUTEUSAGE;



extern RPC_IF_HANDLE __MIDL_itf_context_0000_0000_v0_0_c_ifspec;
extern RPC_IF_HANDLE __MIDL_itf_context_0000_0000_v0_0_s_ifspec;

#ifndef __IVsUserContext_INTERFACE_DEFINED__
#define __IVsUserContext_INTERFACE_DEFINED__

/* interface IVsUserContext */
/* [version][object][uuid] */ 


EXTERN_C const IID IID_IVsUserContext;

#if defined(__cplusplus) && !defined(CINTERFACE)
    
    MIDL_INTERFACE("761081DF-D45F-4683-9B9E-1B7241E56F5C")
    IVsUserContext : public IUnknown
    {
    public:
        virtual HRESULT STDMETHODCALLTYPE AddAttribute( 
            /* [in] */ VSUSERCONTEXTATTRIBUTEUSAGE usage,
            /* [in] */ __RPC__in LPCOLESTR szName,
            /* [in] */ __RPC__in LPCOLESTR szValue) = 0;
        
        virtual HRESULT STDMETHODCALLTYPE RemoveAttribute( 
            /* [in] */ __RPC__in LPCOLESTR szName,
            /* [in] */ __RPC__in LPCOLESTR szValue) = 0;
        
        virtual HRESULT STDMETHODCALLTYPE AddSubcontext( 
            /* [in] */ __RPC__in_opt IVsUserContext *pSubCtx,
            /* [in] */ int lPriority,
            /* [retval][out] */ __RPC__out VSCOOKIE *pdwCookie) = 0;
        
        virtual HRESULT STDMETHODCALLTYPE RemoveSubcontext( 
            /* [in] */ VSCOOKIE dwcookie) = 0;
        
        virtual HRESULT STDMETHODCALLTYPE CountAttributes( 
            /* [in] */ __RPC__in LPCOLESTR pszName,
            /* [in] */ BOOL fIncludeChildren,
            /* [retval][out] */ __RPC__out int *pc) = 0;
        
        virtual HRESULT STDMETHODCALLTYPE GetAttribute( 
            /* [in] */ int iAttribute,
            /* [in] */ __RPC__in LPCOLESTR pszName,
            /* [in] */ BOOL fIncludeChildren,
            /* [out] */ __RPC__deref_out_opt BSTR *pbstrName,
            /* [retval][out] */ __RPC__deref_out_opt BSTR *pbstrValue) = 0;
        
        virtual HRESULT STDMETHODCALLTYPE CountSubcontexts( 
            /* [retval][out] */ __RPC__out int *pc) = 0;
        
        virtual HRESULT STDMETHODCALLTYPE GetSubcontext( 
            /* [in] */ int i,
            /* [retval][out] */ __RPC__deref_out_opt IVsUserContext **ppSubCtx) = 0;
        
        virtual HRESULT STDMETHODCALLTYPE IsDirty( 
            /* [retval][out] */ __RPC__out BOOL *pfDirty) = 0;
        
        virtual HRESULT STDMETHODCALLTYPE SetDirty( 
            /* [in] */ BOOL fDirty) = 0;
        
        virtual HRESULT STDMETHODCALLTYPE Update( void) = 0;
        
        virtual HRESULT STDMETHODCALLTYPE AdviseUpdate( 
            /* [in] */ __RPC__in_opt IVsUserContextUpdate *pUpdate,
            /* [retval][out] */ __RPC__out VSCOOKIE *pdwCookie) = 0;
        
        virtual HRESULT STDMETHODCALLTYPE UnadviseUpdate( 
            /* [in] */ VSCOOKIE dwCookie) = 0;
        
        virtual HRESULT STDMETHODCALLTYPE GetAttrUsage( 
            /* [in] */ int index,
            /* [in] */ BOOL fIncludeChildren,
            /* [retval][out] */ __RPC__out VSUSERCONTEXTATTRIBUTEUSAGE *pUsage) = 0;
        
        virtual HRESULT STDMETHODCALLTYPE RemoveAllSubcontext( void) = 0;
        
        virtual HRESULT STDMETHODCALLTYPE GetPriority( 
            /* [retval][out] */ __RPC__out int *lPriority) = 0;
        
        virtual HRESULT STDMETHODCALLTYPE RemoveAttributeIncludeChildren( 
            /* [in] */ __RPC__in LPCOLESTR szName,
            /* [in] */ __RPC__in LPCOLESTR szValue) = 0;
        
        virtual HRESULT STDMETHODCALLTYPE GetAttributePri( 
            /* [in] */ int iAttribute,
            /* [in] */ __RPC__in LPCOLESTR pszName,
            /* [in] */ BOOL fIncludeChildren,
            /* [out] */ __RPC__out int *piPriority,
            /* [out] */ __RPC__deref_out_opt BSTR *pbstrName,
            /* [retval][out] */ __RPC__deref_out_opt BSTR *pbstrValue) = 0;
        
    };
    
#else 	/* C style interface */

    typedef struct IVsUserContextVtbl
    {
        BEGIN_INTERFACE
        
        HRESULT ( STDMETHODCALLTYPE *QueryInterface )( 
            IVsUserContext * This,
            /* [in] */ __RPC__in REFIID riid,
            /* [iid_is][out] */ 
            __RPC__deref_out  void **ppvObject);
        
        ULONG ( STDMETHODCALLTYPE *AddRef )( 
            IVsUserContext * This);
        
        ULONG ( STDMETHODCALLTYPE *Release )( 
            IVsUserContext * This);
        
        HRESULT ( STDMETHODCALLTYPE *AddAttribute )( 
            IVsUserContext * This,
            /* [in] */ VSUSERCONTEXTATTRIBUTEUSAGE usage,
            /* [in] */ __RPC__in LPCOLESTR szName,
            /* [in] */ __RPC__in LPCOLESTR szValue);
        
        HRESULT ( STDMETHODCALLTYPE *RemoveAttribute )( 
            IVsUserContext * This,
            /* [in] */ __RPC__in LPCOLESTR szName,
            /* [in] */ __RPC__in LPCOLESTR szValue);
        
        HRESULT ( STDMETHODCALLTYPE *AddSubcontext )( 
            IVsUserContext * This,
            /* [in] */ __RPC__in_opt IVsUserContext *pSubCtx,
            /* [in] */ int lPriority,
            /* [retval][out] */ __RPC__out VSCOOKIE *pdwCookie);
        
        HRESULT ( STDMETHODCALLTYPE *RemoveSubcontext )( 
            IVsUserContext * This,
            /* [in] */ VSCOOKIE dwcookie);
        
        HRESULT ( STDMETHODCALLTYPE *CountAttributes )( 
            IVsUserContext * This,
            /* [in] */ __RPC__in LPCOLESTR pszName,
            /* [in] */ BOOL fIncludeChildren,
            /* [retval][out] */ __RPC__out int *pc);
        
        HRESULT ( STDMETHODCALLTYPE *GetAttribute )( 
            IVsUserContext * This,
            /* [in] */ int iAttribute,
            /* [in] */ __RPC__in LPCOLESTR pszName,
            /* [in] */ BOOL fIncludeChildren,
            /* [out] */ __RPC__deref_out_opt BSTR *pbstrName,
            /* [retval][out] */ __RPC__deref_out_opt BSTR *pbstrValue);
        
        HRESULT ( STDMETHODCALLTYPE *CountSubcontexts )( 
            IVsUserContext * This,
            /* [retval][out] */ __RPC__out int *pc);
        
        HRESULT ( STDMETHODCALLTYPE *GetSubcontext )( 
            IVsUserContext * This,
            /* [in] */ int i,
            /* [retval][out] */ __RPC__deref_out_opt IVsUserContext **ppSubCtx);
        
        HRESULT ( STDMETHODCALLTYPE *IsDirty )( 
            IVsUserContext * This,
            /* [retval][out] */ __RPC__out BOOL *pfDirty);
        
        HRESULT ( STDMETHODCALLTYPE *SetDirty )( 
            IVsUserContext * This,
            /* [in] */ BOOL fDirty);
        
        HRESULT ( STDMETHODCALLTYPE *Update )( 
            IVsUserContext * This);
        
        HRESULT ( STDMETHODCALLTYPE *AdviseUpdate )( 
            IVsUserContext * This,
            /* [in] */ __RPC__in_opt IVsUserContextUpdate *pUpdate,
            /* [retval][out] */ __RPC__out VSCOOKIE *pdwCookie);
        
        HRESULT ( STDMETHODCALLTYPE *UnadviseUpdate )( 
            IVsUserContext * This,
            /* [in] */ VSCOOKIE dwCookie);
        
        HRESULT ( STDMETHODCALLTYPE *GetAttrUsage )( 
            IVsUserContext * This,
            /* [in] */ int index,
            /* [in] */ BOOL fIncludeChildren,
            /* [retval][out] */ __RPC__out VSUSERCONTEXTATTRIBUTEUSAGE *pUsage);
        
        HRESULT ( STDMETHODCALLTYPE *RemoveAllSubcontext )( 
            IVsUserContext * This);
        
        HRESULT ( STDMETHODCALLTYPE *GetPriority )( 
            IVsUserContext * This,
            /* [retval][out] */ __RPC__out int *lPriority);
        
        HRESULT ( STDMETHODCALLTYPE *RemoveAttributeIncludeChildren )( 
            IVsUserContext * This,
            /* [in] */ __RPC__in LPCOLESTR szName,
            /* [in] */ __RPC__in LPCOLESTR szValue);
        
        HRESULT ( STDMETHODCALLTYPE *GetAttributePri )( 
            IVsUserContext * This,
            /* [in] */ int iAttribute,
            /* [in] */ __RPC__in LPCOLESTR pszName,
            /* [in] */ BOOL fIncludeChildren,
            /* [out] */ __RPC__out int *piPriority,
            /* [out] */ __RPC__deref_out_opt BSTR *pbstrName,
            /* [retval][out] */ __RPC__deref_out_opt BSTR *pbstrValue);
        
        END_INTERFACE
    } IVsUserContextVtbl;

    interface IVsUserContext
    {
        CONST_VTBL struct IVsUserContextVtbl *lpVtbl;
    };

    

#ifdef COBJMACROS


#define IVsUserContext_QueryInterface(This,riid,ppvObject)	\
    ( (This)->lpVtbl -> QueryInterface(This,riid,ppvObject) ) 

#define IVsUserContext_AddRef(This)	\
    ( (This)->lpVtbl -> AddRef(This) ) 

#define IVsUserContext_Release(This)	\
    ( (This)->lpVtbl -> Release(This) ) 


#define IVsUserContext_AddAttribute(This,usage,szName,szValue)	\
    ( (This)->lpVtbl -> AddAttribute(This,usage,szName,szValue) ) 

#define IVsUserContext_RemoveAttribute(This,szName,szValue)	\
    ( (This)->lpVtbl -> RemoveAttribute(This,szName,szValue) ) 

#define IVsUserContext_AddSubcontext(This,pSubCtx,lPriority,pdwCookie)	\
    ( (This)->lpVtbl -> AddSubcontext(This,pSubCtx,lPriority,pdwCookie) ) 

#define IVsUserContext_RemoveSubcontext(This,dwcookie)	\
    ( (This)->lpVtbl -> RemoveSubcontext(This,dwcookie) ) 

#define IVsUserContext_CountAttributes(This,pszName,fIncludeChildren,pc)	\
    ( (This)->lpVtbl -> CountAttributes(This,pszName,fIncludeChildren,pc) ) 

#define IVsUserContext_GetAttribute(This,iAttribute,pszName,fIncludeChildren,pbstrName,pbstrValue)	\
    ( (This)->lpVtbl -> GetAttribute(This,iAttribute,pszName,fIncludeChildren,pbstrName,pbstrValue) ) 

#define IVsUserContext_CountSubcontexts(This,pc)	\
    ( (This)->lpVtbl -> CountSubcontexts(This,pc) ) 

#define IVsUserContext_GetSubcontext(This,i,ppSubCtx)	\
    ( (This)->lpVtbl -> GetSubcontext(This,i,ppSubCtx) ) 

#define IVsUserContext_IsDirty(This,pfDirty)	\
    ( (This)->lpVtbl -> IsDirty(This,pfDirty) ) 

#define IVsUserContext_SetDirty(This,fDirty)	\
    ( (This)->lpVtbl -> SetDirty(This,fDirty) ) 

#define IVsUserContext_Update(This)	\
    ( (This)->lpVtbl -> Update(This) ) 

#define IVsUserContext_AdviseUpdate(This,pUpdate,pdwCookie)	\
    ( (This)->lpVtbl -> AdviseUpdate(This,pUpdate,pdwCookie) ) 

#define IVsUserContext_UnadviseUpdate(This,dwCookie)	\
    ( (This)->lpVtbl -> UnadviseUpdate(This,dwCookie) ) 

#define IVsUserContext_GetAttrUsage(This,index,fIncludeChildren,pUsage)	\
    ( (This)->lpVtbl -> GetAttrUsage(This,index,fIncludeChildren,pUsage) ) 

#define IVsUserContext_RemoveAllSubcontext(This)	\
    ( (This)->lpVtbl -> RemoveAllSubcontext(This) ) 

#define IVsUserContext_GetPriority(This,lPriority)	\
    ( (This)->lpVtbl -> GetPriority(This,lPriority) ) 

#define IVsUserContext_RemoveAttributeIncludeChildren(This,szName,szValue)	\
    ( (This)->lpVtbl -> RemoveAttributeIncludeChildren(This,szName,szValue) ) 

#define IVsUserContext_GetAttributePri(This,iAttribute,pszName,fIncludeChildren,piPriority,pbstrName,pbstrValue)	\
    ( (This)->lpVtbl -> GetAttributePri(This,iAttribute,pszName,fIncludeChildren,piPriority,pbstrName,pbstrValue) ) 

#endif /* COBJMACROS */


#endif 	/* C style interface */




#endif 	/* __IVsUserContext_INTERFACE_DEFINED__ */


#ifndef __IVsUserContextUpdate_INTERFACE_DEFINED__
#define __IVsUserContextUpdate_INTERFACE_DEFINED__

/* interface IVsUserContextUpdate */
/* [version][object][uuid] */ 


EXTERN_C const IID IID_IVsUserContextUpdate;

#if defined(__cplusplus) && !defined(CINTERFACE)
    
    MIDL_INTERFACE("F5ED7D1C-61B6-428A-8129-E13B36D9E9A7")
    IVsUserContextUpdate : public IUnknown
    {
    public:
        virtual HRESULT STDMETHODCALLTYPE UpdateUserContext( 
            /* [in] */ __RPC__in_opt IVsUserContext *pCtx,
            /* [in] */ VSCOOKIE dwCookie) = 0;
        
    };
    
#else 	/* C style interface */

    typedef struct IVsUserContextUpdateVtbl
    {
        BEGIN_INTERFACE
        
        HRESULT ( STDMETHODCALLTYPE *QueryInterface )( 
            IVsUserContextUpdate * This,
            /* [in] */ __RPC__in REFIID riid,
            /* [iid_is][out] */ 
            __RPC__deref_out  void **ppvObject);
        
        ULONG ( STDMETHODCALLTYPE *AddRef )( 
            IVsUserContextUpdate * This);
        
        ULONG ( STDMETHODCALLTYPE *Release )( 
            IVsUserContextUpdate * This);
        
        HRESULT ( STDMETHODCALLTYPE *UpdateUserContext )( 
            IVsUserContextUpdate * This,
            /* [in] */ __RPC__in_opt IVsUserContext *pCtx,
            /* [in] */ VSCOOKIE dwCookie);
        
        END_INTERFACE
    } IVsUserContextUpdateVtbl;

    interface IVsUserContextUpdate
    {
        CONST_VTBL struct IVsUserContextUpdateVtbl *lpVtbl;
    };

    

#ifdef COBJMACROS


#define IVsUserContextUpdate_QueryInterface(This,riid,ppvObject)	\
    ( (This)->lpVtbl -> QueryInterface(This,riid,ppvObject) ) 

#define IVsUserContextUpdate_AddRef(This)	\
    ( (This)->lpVtbl -> AddRef(This) ) 

#define IVsUserContextUpdate_Release(This)	\
    ( (This)->lpVtbl -> Release(This) ) 


#define IVsUserContextUpdate_UpdateUserContext(This,pCtx,dwCookie)	\
    ( (This)->lpVtbl -> UpdateUserContext(This,pCtx,dwCookie) ) 

#endif /* COBJMACROS */


#endif 	/* C style interface */




#endif 	/* __IVsUserContextUpdate_INTERFACE_DEFINED__ */


#ifndef __IVsProvideUserContext_INTERFACE_DEFINED__
#define __IVsProvideUserContext_INTERFACE_DEFINED__

/* interface IVsProvideUserContext */
/* [version][object][uuid] */ 


EXTERN_C const IID IID_IVsProvideUserContext;

#if defined(__cplusplus) && !defined(CINTERFACE)
    
    MIDL_INTERFACE("997D7904-D948-4C8B-8BAB-0BDA1E212F6E")
    IVsProvideUserContext : public IUnknown
    {
    public:
        virtual HRESULT STDMETHODCALLTYPE GetUserContext( 
            /* [retval][out] */ __RPC__deref_out_opt IVsUserContext **ppctx) = 0;
        
    };
    
#else 	/* C style interface */

    typedef struct IVsProvideUserContextVtbl
    {
        BEGIN_INTERFACE
        
        HRESULT ( STDMETHODCALLTYPE *QueryInterface )( 
            IVsProvideUserContext * This,
            /* [in] */ __RPC__in REFIID riid,
            /* [iid_is][out] */ 
            __RPC__deref_out  void **ppvObject);
        
        ULONG ( STDMETHODCALLTYPE *AddRef )( 
            IVsProvideUserContext * This);
        
        ULONG ( STDMETHODCALLTYPE *Release )( 
            IVsProvideUserContext * This);
        
        HRESULT ( STDMETHODCALLTYPE *GetUserContext )( 
            IVsProvideUserContext * This,
            /* [retval][out] */ __RPC__deref_out_opt IVsUserContext **ppctx);
        
        END_INTERFACE
    } IVsProvideUserContextVtbl;

    interface IVsProvideUserContext
    {
        CONST_VTBL struct IVsProvideUserContextVtbl *lpVtbl;
    };

    

#ifdef COBJMACROS


#define IVsProvideUserContext_QueryInterface(This,riid,ppvObject)	\
    ( (This)->lpVtbl -> QueryInterface(This,riid,ppvObject) ) 

#define IVsProvideUserContext_AddRef(This)	\
    ( (This)->lpVtbl -> AddRef(This) ) 

#define IVsProvideUserContext_Release(This)	\
    ( (This)->lpVtbl -> Release(This) ) 


#define IVsProvideUserContext_GetUserContext(This,ppctx)	\
    ( (This)->lpVtbl -> GetUserContext(This,ppctx) ) 

#endif /* COBJMACROS */


#endif 	/* C style interface */




#endif 	/* __IVsProvideUserContext_INTERFACE_DEFINED__ */


#ifndef __IVsProvideUserContextForObject_INTERFACE_DEFINED__
#define __IVsProvideUserContextForObject_INTERFACE_DEFINED__

/* interface IVsProvideUserContextForObject */
/* [version][object][uuid] */ 


EXTERN_C const IID IID_IVsProvideUserContextForObject;

#if defined(__cplusplus) && !defined(CINTERFACE)
    
    MIDL_INTERFACE("F98CCC8A-9C5F-41EB-8421-711C0F1880E6")
    IVsProvideUserContextForObject : public IUnknown
    {
    public:
        virtual HRESULT STDMETHODCALLTYPE GetObjectContext( 
            /* [in] */ __RPC__in_opt IUnknown *punk,
            /* [retval][out] */ __RPC__deref_out_opt IVsUserContext **ppctx) = 0;
        
    };
    
#else 	/* C style interface */

    typedef struct IVsProvideUserContextForObjectVtbl
    {
        BEGIN_INTERFACE
        
        HRESULT ( STDMETHODCALLTYPE *QueryInterface )( 
            IVsProvideUserContextForObject * This,
            /* [in] */ __RPC__in REFIID riid,
            /* [iid_is][out] */ 
            __RPC__deref_out  void **ppvObject);
        
        ULONG ( STDMETHODCALLTYPE *AddRef )( 
            IVsProvideUserContextForObject * This);
        
        ULONG ( STDMETHODCALLTYPE *Release )( 
            IVsProvideUserContextForObject * This);
        
        HRESULT ( STDMETHODCALLTYPE *GetObjectContext )( 
            IVsProvideUserContextForObject * This,
            /* [in] */ __RPC__in_opt IUnknown *punk,
            /* [retval][out] */ __RPC__deref_out_opt IVsUserContext **ppctx);
        
        END_INTERFACE
    } IVsProvideUserContextForObjectVtbl;

    interface IVsProvideUserContextForObject
    {
        CONST_VTBL struct IVsProvideUserContextForObjectVtbl *lpVtbl;
    };

    

#ifdef COBJMACROS


#define IVsProvideUserContextForObject_QueryInterface(This,riid,ppvObject)	\
    ( (This)->lpVtbl -> QueryInterface(This,riid,ppvObject) ) 

#define IVsProvideUserContextForObject_AddRef(This)	\
    ( (This)->lpVtbl -> AddRef(This) ) 

#define IVsProvideUserContextForObject_Release(This)	\
    ( (This)->lpVtbl -> Release(This) ) 


#define IVsProvideUserContextForObject_GetObjectContext(This,punk,ppctx)	\
    ( (This)->lpVtbl -> GetObjectContext(This,punk,ppctx) ) 

#endif /* COBJMACROS */


#endif 	/* C style interface */




#endif 	/* __IVsProvideUserContextForObject_INTERFACE_DEFINED__ */


#ifndef __IVsUserContextItemCollection_INTERFACE_DEFINED__
#define __IVsUserContextItemCollection_INTERFACE_DEFINED__

/* interface IVsUserContextItemCollection */
/* [version][object][uuid] */ 


EXTERN_C const IID IID_IVsUserContextItemCollection;

#if defined(__cplusplus) && !defined(CINTERFACE)
    
    MIDL_INTERFACE("2A6DE4A2-5B3D-46EB-A65C-24C4EF4F396F")
    IVsUserContextItemCollection : public IUnknown
    {
    public:
        virtual /* [id][propget] */ HRESULT STDMETHODCALLTYPE get_Item( 
            /* [in] */ VARIANT index,
            /* [retval][out] */ __RPC__deref_out_opt IVsUserContextItem **ppItem) = 0;
        
        virtual /* [id][propget] */ HRESULT STDMETHODCALLTYPE get__NewEnum( 
            /* [retval][out] */ __RPC__deref_out_opt IUnknown **pEnum) = 0;
        
        virtual /* [propget] */ HRESULT STDMETHODCALLTYPE get_Count( 
            /* [retval][out] */ __RPC__out long *pCount) = 0;
        
        virtual /* [propget] */ HRESULT STDMETHODCALLTYPE get_ItemAt( 
            /* [in] */ long index,
            /* [retval][out] */ __RPC__deref_out_opt IVsUserContextItem **ppItem) = 0;
        
    };
    
#else 	/* C style interface */

    typedef struct IVsUserContextItemCollectionVtbl
    {
        BEGIN_INTERFACE
        
        HRESULT ( STDMETHODCALLTYPE *QueryInterface )( 
            IVsUserContextItemCollection * This,
            /* [in] */ __RPC__in REFIID riid,
            /* [iid_is][out] */ 
            __RPC__deref_out  void **ppvObject);
        
        ULONG ( STDMETHODCALLTYPE *AddRef )( 
            IVsUserContextItemCollection * This);
        
        ULONG ( STDMETHODCALLTYPE *Release )( 
            IVsUserContextItemCollection * This);
        
        /* [id][propget] */ HRESULT ( STDMETHODCALLTYPE *get_Item )( 
            IVsUserContextItemCollection * This,
            /* [in] */ VARIANT index,
            /* [retval][out] */ __RPC__deref_out_opt IVsUserContextItem **ppItem);
        
        /* [id][propget] */ HRESULT ( STDMETHODCALLTYPE *get__NewEnum )( 
            IVsUserContextItemCollection * This,
            /* [retval][out] */ __RPC__deref_out_opt IUnknown **pEnum);
        
        /* [propget] */ HRESULT ( STDMETHODCALLTYPE *get_Count )( 
            IVsUserContextItemCollection * This,
            /* [retval][out] */ __RPC__out long *pCount);
        
        /* [propget] */ HRESULT ( STDMETHODCALLTYPE *get_ItemAt )( 
            IVsUserContextItemCollection * This,
            /* [in] */ long index,
            /* [retval][out] */ __RPC__deref_out_opt IVsUserContextItem **ppItem);
        
        END_INTERFACE
    } IVsUserContextItemCollectionVtbl;

    interface IVsUserContextItemCollection
    {
        CONST_VTBL struct IVsUserContextItemCollectionVtbl *lpVtbl;
    };

    

#ifdef COBJMACROS


#define IVsUserContextItemCollection_QueryInterface(This,riid,ppvObject)	\
    ( (This)->lpVtbl -> QueryInterface(This,riid,ppvObject) ) 

#define IVsUserContextItemCollection_AddRef(This)	\
    ( (This)->lpVtbl -> AddRef(This) ) 

#define IVsUserContextItemCollection_Release(This)	\
    ( (This)->lpVtbl -> Release(This) ) 


#define IVsUserContextItemCollection_get_Item(This,index,ppItem)	\
    ( (This)->lpVtbl -> get_Item(This,index,ppItem) ) 

#define IVsUserContextItemCollection_get__NewEnum(This,pEnum)	\
    ( (This)->lpVtbl -> get__NewEnum(This,pEnum) ) 

#define IVsUserContextItemCollection_get_Count(This,pCount)	\
    ( (This)->lpVtbl -> get_Count(This,pCount) ) 

#define IVsUserContextItemCollection_get_ItemAt(This,index,ppItem)	\
    ( (This)->lpVtbl -> get_ItemAt(This,index,ppItem) ) 

#endif /* COBJMACROS */


#endif 	/* C style interface */




#endif 	/* __IVsUserContextItemCollection_INTERFACE_DEFINED__ */


#ifndef __IVsUserContextItem_INTERFACE_DEFINED__
#define __IVsUserContextItem_INTERFACE_DEFINED__

/* interface IVsUserContextItem */
/* [version][object][uuid] */ 


EXTERN_C const IID IID_IVsUserContextItem;

#if defined(__cplusplus) && !defined(CINTERFACE)
    
    MIDL_INTERFACE("720B8500-17B3-4C89-AE84-2CFE7251B4B8")
    IVsUserContextItem : public IUnknown
    {
    public:
        virtual /* [propget] */ HRESULT STDMETHODCALLTYPE get_Name( 
            /* [retval][out] */ __RPC__deref_out_opt BSTR *pbstrName) = 0;
        
        virtual /* [propget] */ HRESULT STDMETHODCALLTYPE get_Command( 
            /* [retval][out] */ __RPC__deref_out_opt BSTR *pbstrCommand) = 0;
        
        virtual HRESULT STDMETHODCALLTYPE CountAttributes( 
            /* [in] */ __RPC__in LPCOLESTR pszAttrName,
            /* [retval][out] */ __RPC__out int *pc) = 0;
        
        virtual HRESULT STDMETHODCALLTYPE GetAttribute( 
            /* [in] */ __RPC__in LPCOLESTR pszAttrName,
            /* [in] */ int index,
            /* [optional][out] */ __RPC__deref_out_opt BSTR *pbstrName,
            /* [retval][optional][out] */ __RPC__deref_out_opt BSTR *pbstrValue) = 0;
        
    };
    
#else 	/* C style interface */

    typedef struct IVsUserContextItemVtbl
    {
        BEGIN_INTERFACE
        
        HRESULT ( STDMETHODCALLTYPE *QueryInterface )( 
            IVsUserContextItem * This,
            /* [in] */ __RPC__in REFIID riid,
            /* [iid_is][out] */ 
            __RPC__deref_out  void **ppvObject);
        
        ULONG ( STDMETHODCALLTYPE *AddRef )( 
            IVsUserContextItem * This);
        
        ULONG ( STDMETHODCALLTYPE *Release )( 
            IVsUserContextItem * This);
        
        /* [propget] */ HRESULT ( STDMETHODCALLTYPE *get_Name )( 
            IVsUserContextItem * This,
            /* [retval][out] */ __RPC__deref_out_opt BSTR *pbstrName);
        
        /* [propget] */ HRESULT ( STDMETHODCALLTYPE *get_Command )( 
            IVsUserContextItem * This,
            /* [retval][out] */ __RPC__deref_out_opt BSTR *pbstrCommand);
        
        HRESULT ( STDMETHODCALLTYPE *CountAttributes )( 
            IVsUserContextItem * This,
            /* [in] */ __RPC__in LPCOLESTR pszAttrName,
            /* [retval][out] */ __RPC__out int *pc);
        
        HRESULT ( STDMETHODCALLTYPE *GetAttribute )( 
            IVsUserContextItem * This,
            /* [in] */ __RPC__in LPCOLESTR pszAttrName,
            /* [in] */ int index,
            /* [optional][out] */ __RPC__deref_out_opt BSTR *pbstrName,
            /* [retval][optional][out] */ __RPC__deref_out_opt BSTR *pbstrValue);
        
        END_INTERFACE
    } IVsUserContextItemVtbl;

    interface IVsUserContextItem
    {
        CONST_VTBL struct IVsUserContextItemVtbl *lpVtbl;
    };

    

#ifdef COBJMACROS


#define IVsUserContextItem_QueryInterface(This,riid,ppvObject)	\
    ( (This)->lpVtbl -> QueryInterface(This,riid,ppvObject) ) 

#define IVsUserContextItem_AddRef(This)	\
    ( (This)->lpVtbl -> AddRef(This) ) 

#define IVsUserContextItem_Release(This)	\
    ( (This)->lpVtbl -> Release(This) ) 


#define IVsUserContextItem_get_Name(This,pbstrName)	\
    ( (This)->lpVtbl -> get_Name(This,pbstrName) ) 

#define IVsUserContextItem_get_Command(This,pbstrCommand)	\
    ( (This)->lpVtbl -> get_Command(This,pbstrCommand) ) 

#define IVsUserContextItem_CountAttributes(This,pszAttrName,pc)	\
    ( (This)->lpVtbl -> CountAttributes(This,pszAttrName,pc) ) 

#define IVsUserContextItem_GetAttribute(This,pszAttrName,index,pbstrName,pbstrValue)	\
    ( (This)->lpVtbl -> GetAttribute(This,pszAttrName,index,pbstrName,pbstrValue) ) 

#endif /* COBJMACROS */


#endif 	/* C style interface */




#endif 	/* __IVsUserContextItem_INTERFACE_DEFINED__ */


/* interface __MIDL_itf_context_0000_0006 */
/* [local] */ 

typedef 
enum tagAttrValueType
    {	VSHAL_Real	= 0,
	VSHAL_Display	= 1
    } 	ATTRVALUETYPE;



extern RPC_IF_HANDLE __MIDL_itf_context_0000_0006_v0_0_c_ifspec;
extern RPC_IF_HANDLE __MIDL_itf_context_0000_0006_v0_0_s_ifspec;

#ifndef __IVsHelpAttributeList_INTERFACE_DEFINED__
#define __IVsHelpAttributeList_INTERFACE_DEFINED__

/* interface IVsHelpAttributeList */
/* [version][object][uuid] */ 


EXTERN_C const IID IID_IVsHelpAttributeList;

#if defined(__cplusplus) && !defined(CINTERFACE)
    
    MIDL_INTERFACE("0A56FB1E-1B2F-4699-8178-63B98E816F35")
    IVsHelpAttributeList : public IUnknown
    {
    public:
        virtual HRESULT STDMETHODCALLTYPE GetAttributeName( 
            /* [out] */ __RPC__deref_out_opt BSTR *bstrName) = 0;
        
        virtual HRESULT STDMETHODCALLTYPE GetCount( 
            /* [out] */ __RPC__out int *pCount) = 0;
        
        virtual HRESULT STDMETHODCALLTYPE UpdateAttributeStatus( 
            /* [in] */ __RPC__in BOOL *afActive) = 0;
        
        virtual HRESULT STDMETHODCALLTYPE GetAttributeStatusVal( 
            /* [in] */ __RPC__in BSTR bstrValue,
            /* [in] */ ATTRVALUETYPE type,
            /* [out] */ __RPC__out BOOL *pfActive) = 0;
        
        virtual HRESULT STDMETHODCALLTYPE GetAttributeStatusIndex( 
            /* [in] */ int index,
            /* [out] */ __RPC__out BOOL *pfActive) = 0;
        
        virtual HRESULT STDMETHODCALLTYPE GetAttributeValue( 
            /* [in] */ int index,
            /* [in] */ ATTRVALUETYPE type,
            /* [out] */ __RPC__deref_out_opt BSTR *pbstrValue) = 0;
        
    };
    
#else 	/* C style interface */

    typedef struct IVsHelpAttributeListVtbl
    {
        BEGIN_INTERFACE
        
        HRESULT ( STDMETHODCALLTYPE *QueryInterface )( 
            IVsHelpAttributeList * This,
            /* [in] */ __RPC__in REFIID riid,
            /* [iid_is][out] */ 
            __RPC__deref_out  void **ppvObject);
        
        ULONG ( STDMETHODCALLTYPE *AddRef )( 
            IVsHelpAttributeList * This);
        
        ULONG ( STDMETHODCALLTYPE *Release )( 
            IVsHelpAttributeList * This);
        
        HRESULT ( STDMETHODCALLTYPE *GetAttributeName )( 
            IVsHelpAttributeList * This,
            /* [out] */ __RPC__deref_out_opt BSTR *bstrName);
        
        HRESULT ( STDMETHODCALLTYPE *GetCount )( 
            IVsHelpAttributeList * This,
            /* [out] */ __RPC__out int *pCount);
        
        HRESULT ( STDMETHODCALLTYPE *UpdateAttributeStatus )( 
            IVsHelpAttributeList * This,
            /* [in] */ __RPC__in BOOL *afActive);
        
        HRESULT ( STDMETHODCALLTYPE *GetAttributeStatusVal )( 
            IVsHelpAttributeList * This,
            /* [in] */ __RPC__in BSTR bstrValue,
            /* [in] */ ATTRVALUETYPE type,
            /* [out] */ __RPC__out BOOL *pfActive);
        
        HRESULT ( STDMETHODCALLTYPE *GetAttributeStatusIndex )( 
            IVsHelpAttributeList * This,
            /* [in] */ int index,
            /* [out] */ __RPC__out BOOL *pfActive);
        
        HRESULT ( STDMETHODCALLTYPE *GetAttributeValue )( 
            IVsHelpAttributeList * This,
            /* [in] */ int index,
            /* [in] */ ATTRVALUETYPE type,
            /* [out] */ __RPC__deref_out_opt BSTR *pbstrValue);
        
        END_INTERFACE
    } IVsHelpAttributeListVtbl;

    interface IVsHelpAttributeList
    {
        CONST_VTBL struct IVsHelpAttributeListVtbl *lpVtbl;
    };

    

#ifdef COBJMACROS


#define IVsHelpAttributeList_QueryInterface(This,riid,ppvObject)	\
    ( (This)->lpVtbl -> QueryInterface(This,riid,ppvObject) ) 

#define IVsHelpAttributeList_AddRef(This)	\
    ( (This)->lpVtbl -> AddRef(This) ) 

#define IVsHelpAttributeList_Release(This)	\
    ( (This)->lpVtbl -> Release(This) ) 


#define IVsHelpAttributeList_GetAttributeName(This,bstrName)	\
    ( (This)->lpVtbl -> GetAttributeName(This,bstrName) ) 

#define IVsHelpAttributeList_GetCount(This,pCount)	\
    ( (This)->lpVtbl -> GetCount(This,pCount) ) 

#define IVsHelpAttributeList_UpdateAttributeStatus(This,afActive)	\
    ( (This)->lpVtbl -> UpdateAttributeStatus(This,afActive) ) 

#define IVsHelpAttributeList_GetAttributeStatusVal(This,bstrValue,type,pfActive)	\
    ( (This)->lpVtbl -> GetAttributeStatusVal(This,bstrValue,type,pfActive) ) 

#define IVsHelpAttributeList_GetAttributeStatusIndex(This,index,pfActive)	\
    ( (This)->lpVtbl -> GetAttributeStatusIndex(This,index,pfActive) ) 

#define IVsHelpAttributeList_GetAttributeValue(This,index,type,pbstrValue)	\
    ( (This)->lpVtbl -> GetAttributeValue(This,index,type,pbstrValue) ) 

#endif /* COBJMACROS */


#endif 	/* C style interface */




#endif 	/* __IVsHelpAttributeList_INTERFACE_DEFINED__ */


/* interface __MIDL_itf_context_0000_0007 */
/* [local] */ 

#define VSUC_CURRENT_F1 ((LPCOLESTR)1)


extern RPC_IF_HANDLE __MIDL_itf_context_0000_0007_v0_0_c_ifspec;
extern RPC_IF_HANDLE __MIDL_itf_context_0000_0007_v0_0_s_ifspec;

#ifndef __IVsMonitorUserContext_INTERFACE_DEFINED__
#define __IVsMonitorUserContext_INTERFACE_DEFINED__

/* interface IVsMonitorUserContext */
/* [version][object][uuid] */ 


EXTERN_C const IID IID_IVsMonitorUserContext;

#if defined(__cplusplus) && !defined(CINTERFACE)
    
    MIDL_INTERFACE("9C074FDB-3D7D-4512-9604-72B3B0A5F609")
    IVsMonitorUserContext : public IUnknown
    {
    public:
        virtual HRESULT STDMETHODCALLTYPE SetSite( 
            /* [in] */ __RPC__in_opt IServiceProvider *pSP) = 0;
        
        virtual HRESULT STDMETHODCALLTYPE get_ApplicationContext( 
            /* [retval][out] */ __RPC__deref_out_opt IVsUserContext **ppContext) = 0;
        
        virtual HRESULT STDMETHODCALLTYPE put_ApplicationContext( 
            /* [in] */ __RPC__in_opt IVsUserContext *pContext) = 0;
        
        virtual HRESULT STDMETHODCALLTYPE CreateEmptyContext( 
            /* [retval][out] */ __RPC__deref_out_opt IVsUserContext **ppContext) = 0;
        
        virtual HRESULT STDMETHODCALLTYPE GetContextItems( 
            /* [out] */ __RPC__deref_out_opt IVsUserContextItemCollection **pplist) = 0;
        
        virtual HRESULT STDMETHODCALLTYPE FindTargetItems( 
            /* [in] */ __RPC__in LPCOLESTR pszTargetAttr,
            /* [in] */ __RPC__in LPCOLESTR pszTargetAttrValue,
            /* [out] */ __RPC__deref_out_opt IVsUserContextItemCollection **ppList,
            /* [out] */ __RPC__out BOOL *pfF1Kwd) = 0;
        
        virtual HRESULT STDMETHODCALLTYPE RegisterItemProvider( 
            /* [in] */ __RPC__in_opt IVsUserContextItemProvider *pProvider,
            /* [retval][out] */ __RPC__out VSCOOKIE *pdwCookie) = 0;
        
        virtual HRESULT STDMETHODCALLTYPE UnregisterItemProvider( 
            /* [in] */ VSCOOKIE dwCookie) = 0;
        
        virtual HRESULT STDMETHODCALLTYPE AdviseContextItemEvents( 
            /* [in] */ __RPC__in_opt IVsUserContextItemEvents *pEvents,
            /* [retval][out] */ __RPC__out VSCOOKIE *pdwCookie) = 0;
        
        virtual HRESULT STDMETHODCALLTYPE UnadviseContextItemEvent( 
            /* [in] */ VSCOOKIE dwCookie) = 0;
        
        virtual HRESULT STDMETHODCALLTYPE GetNextCtxBagAttr( 
            /* [out] */ __RPC__deref_out_opt BSTR *pbstrAttrName,
            /* [out] */ __RPC__deref_out_opt BSTR *pbstrAttrVal) = 0;
        
        virtual HRESULT STDMETHODCALLTYPE ResetNextCtxBagAttr( void) = 0;
        
        virtual HRESULT STDMETHODCALLTYPE GetPrevAttrCache( 
            /* [out] */ __RPC__deref_out_opt BSTR **pbstrCacheArray,
            /* [out] */ __RPC__deref_out_opt int **pnCurrNumStored,
            /* [out] */ __RPC__out int *pnMaxNumStored) = 0;
        
        virtual HRESULT STDMETHODCALLTYPE GetNextCtxBag( 
            /* [out] */ __RPC__deref_out_opt BSTR *pbstrAttrName,
            /* [out] */ __RPC__deref_out_opt BSTR *pbstrAttrVal) = 0;
        
        virtual HRESULT STDMETHODCALLTYPE IsIdleAvailable( 
            /* [out] */ __RPC__out BOOL *pfIdleAvail) = 0;
        
        virtual HRESULT STDMETHODCALLTYPE SetTopicTypeFilter( 
            /* [in] */ __RPC__in_opt IVsHelpAttributeList *pTopicTypeList) = 0;
        
        virtual HRESULT STDMETHODCALLTYPE GetF1Kwd( 
            /* [out] */ __RPC__deref_out_opt BSTR *pbstrKwd,
            /* [out] */ __RPC__out BOOL *fF1Kwd) = 0;
        
        virtual HRESULT STDMETHODCALLTYPE IsF1Lookup( 
            /* [out] */ __RPC__out BOOL *fF1Lookup) = 0;
        
    };
    
#else 	/* C style interface */

    typedef struct IVsMonitorUserContextVtbl
    {
        BEGIN_INTERFACE
        
        HRESULT ( STDMETHODCALLTYPE *QueryInterface )( 
            IVsMonitorUserContext * This,
            /* [in] */ __RPC__in REFIID riid,
            /* [iid_is][out] */ 
            __RPC__deref_out  void **ppvObject);
        
        ULONG ( STDMETHODCALLTYPE *AddRef )( 
            IVsMonitorUserContext * This);
        
        ULONG ( STDMETHODCALLTYPE *Release )( 
            IVsMonitorUserContext * This);
        
        HRESULT ( STDMETHODCALLTYPE *SetSite )( 
            IVsMonitorUserContext * This,
            /* [in] */ __RPC__in_opt IServiceProvider *pSP);
        
        HRESULT ( STDMETHODCALLTYPE *get_ApplicationContext )( 
            IVsMonitorUserContext * This,
            /* [retval][out] */ __RPC__deref_out_opt IVsUserContext **ppContext);
        
        HRESULT ( STDMETHODCALLTYPE *put_ApplicationContext )( 
            IVsMonitorUserContext * This,
            /* [in] */ __RPC__in_opt IVsUserContext *pContext);
        
        HRESULT ( STDMETHODCALLTYPE *CreateEmptyContext )( 
            IVsMonitorUserContext * This,
            /* [retval][out] */ __RPC__deref_out_opt IVsUserContext **ppContext);
        
        HRESULT ( STDMETHODCALLTYPE *GetContextItems )( 
            IVsMonitorUserContext * This,
            /* [out] */ __RPC__deref_out_opt IVsUserContextItemCollection **pplist);
        
        HRESULT ( STDMETHODCALLTYPE *FindTargetItems )( 
            IVsMonitorUserContext * This,
            /* [in] */ __RPC__in LPCOLESTR pszTargetAttr,
            /* [in] */ __RPC__in LPCOLESTR pszTargetAttrValue,
            /* [out] */ __RPC__deref_out_opt IVsUserContextItemCollection **ppList,
            /* [out] */ __RPC__out BOOL *pfF1Kwd);
        
        HRESULT ( STDMETHODCALLTYPE *RegisterItemProvider )( 
            IVsMonitorUserContext * This,
            /* [in] */ __RPC__in_opt IVsUserContextItemProvider *pProvider,
            /* [retval][out] */ __RPC__out VSCOOKIE *pdwCookie);
        
        HRESULT ( STDMETHODCALLTYPE *UnregisterItemProvider )( 
            IVsMonitorUserContext * This,
            /* [in] */ VSCOOKIE dwCookie);
        
        HRESULT ( STDMETHODCALLTYPE *AdviseContextItemEvents )( 
            IVsMonitorUserContext * This,
            /* [in] */ __RPC__in_opt IVsUserContextItemEvents *pEvents,
            /* [retval][out] */ __RPC__out VSCOOKIE *pdwCookie);
        
        HRESULT ( STDMETHODCALLTYPE *UnadviseContextItemEvent )( 
            IVsMonitorUserContext * This,
            /* [in] */ VSCOOKIE dwCookie);
        
        HRESULT ( STDMETHODCALLTYPE *GetNextCtxBagAttr )( 
            IVsMonitorUserContext * This,
            /* [out] */ __RPC__deref_out_opt BSTR *pbstrAttrName,
            /* [out] */ __RPC__deref_out_opt BSTR *pbstrAttrVal);
        
        HRESULT ( STDMETHODCALLTYPE *ResetNextCtxBagAttr )( 
            IVsMonitorUserContext * This);
        
        HRESULT ( STDMETHODCALLTYPE *GetPrevAttrCache )( 
            IVsMonitorUserContext * This,
            /* [out] */ __RPC__deref_out_opt BSTR **pbstrCacheArray,
            /* [out] */ __RPC__deref_out_opt int **pnCurrNumStored,
            /* [out] */ __RPC__out int *pnMaxNumStored);
        
        HRESULT ( STDMETHODCALLTYPE *GetNextCtxBag )( 
            IVsMonitorUserContext * This,
            /* [out] */ __RPC__deref_out_opt BSTR *pbstrAttrName,
            /* [out] */ __RPC__deref_out_opt BSTR *pbstrAttrVal);
        
        HRESULT ( STDMETHODCALLTYPE *IsIdleAvailable )( 
            IVsMonitorUserContext * This,
            /* [out] */ __RPC__out BOOL *pfIdleAvail);
        
        HRESULT ( STDMETHODCALLTYPE *SetTopicTypeFilter )( 
            IVsMonitorUserContext * This,
            /* [in] */ __RPC__in_opt IVsHelpAttributeList *pTopicTypeList);
        
        HRESULT ( STDMETHODCALLTYPE *GetF1Kwd )( 
            IVsMonitorUserContext * This,
            /* [out] */ __RPC__deref_out_opt BSTR *pbstrKwd,
            /* [out] */ __RPC__out BOOL *fF1Kwd);
        
        HRESULT ( STDMETHODCALLTYPE *IsF1Lookup )( 
            IVsMonitorUserContext * This,
            /* [out] */ __RPC__out BOOL *fF1Lookup);
        
        END_INTERFACE
    } IVsMonitorUserContextVtbl;

    interface IVsMonitorUserContext
    {
        CONST_VTBL struct IVsMonitorUserContextVtbl *lpVtbl;
    };

    

#ifdef COBJMACROS


#define IVsMonitorUserContext_QueryInterface(This,riid,ppvObject)	\
    ( (This)->lpVtbl -> QueryInterface(This,riid,ppvObject) ) 

#define IVsMonitorUserContext_AddRef(This)	\
    ( (This)->lpVtbl -> AddRef(This) ) 

#define IVsMonitorUserContext_Release(This)	\
    ( (This)->lpVtbl -> Release(This) ) 


#define IVsMonitorUserContext_SetSite(This,pSP)	\
    ( (This)->lpVtbl -> SetSite(This,pSP) ) 

#define IVsMonitorUserContext_get_ApplicationContext(This,ppContext)	\
    ( (This)->lpVtbl -> get_ApplicationContext(This,ppContext) ) 

#define IVsMonitorUserContext_put_ApplicationContext(This,pContext)	\
    ( (This)->lpVtbl -> put_ApplicationContext(This,pContext) ) 

#define IVsMonitorUserContext_CreateEmptyContext(This,ppContext)	\
    ( (This)->lpVtbl -> CreateEmptyContext(This,ppContext) ) 

#define IVsMonitorUserContext_GetContextItems(This,pplist)	\
    ( (This)->lpVtbl -> GetContextItems(This,pplist) ) 

#define IVsMonitorUserContext_FindTargetItems(This,pszTargetAttr,pszTargetAttrValue,ppList,pfF1Kwd)	\
    ( (This)->lpVtbl -> FindTargetItems(This,pszTargetAttr,pszTargetAttrValue,ppList,pfF1Kwd) ) 

#define IVsMonitorUserContext_RegisterItemProvider(This,pProvider,pdwCookie)	\
    ( (This)->lpVtbl -> RegisterItemProvider(This,pProvider,pdwCookie) ) 

#define IVsMonitorUserContext_UnregisterItemProvider(This,dwCookie)	\
    ( (This)->lpVtbl -> UnregisterItemProvider(This,dwCookie) ) 

#define IVsMonitorUserContext_AdviseContextItemEvents(This,pEvents,pdwCookie)	\
    ( (This)->lpVtbl -> AdviseContextItemEvents(This,pEvents,pdwCookie) ) 

#define IVsMonitorUserContext_UnadviseContextItemEvent(This,dwCookie)	\
    ( (This)->lpVtbl -> UnadviseContextItemEvent(This,dwCookie) ) 

#define IVsMonitorUserContext_GetNextCtxBagAttr(This,pbstrAttrName,pbstrAttrVal)	\
    ( (This)->lpVtbl -> GetNextCtxBagAttr(This,pbstrAttrName,pbstrAttrVal) ) 

#define IVsMonitorUserContext_ResetNextCtxBagAttr(This)	\
    ( (This)->lpVtbl -> ResetNextCtxBagAttr(This) ) 

#define IVsMonitorUserContext_GetPrevAttrCache(This,pbstrCacheArray,pnCurrNumStored,pnMaxNumStored)	\
    ( (This)->lpVtbl -> GetPrevAttrCache(This,pbstrCacheArray,pnCurrNumStored,pnMaxNumStored) ) 

#define IVsMonitorUserContext_GetNextCtxBag(This,pbstrAttrName,pbstrAttrVal)	\
    ( (This)->lpVtbl -> GetNextCtxBag(This,pbstrAttrName,pbstrAttrVal) ) 

#define IVsMonitorUserContext_IsIdleAvailable(This,pfIdleAvail)	\
    ( (This)->lpVtbl -> IsIdleAvailable(This,pfIdleAvail) ) 

#define IVsMonitorUserContext_SetTopicTypeFilter(This,pTopicTypeList)	\
    ( (This)->lpVtbl -> SetTopicTypeFilter(This,pTopicTypeList) ) 

#define IVsMonitorUserContext_GetF1Kwd(This,pbstrKwd,fF1Kwd)	\
    ( (This)->lpVtbl -> GetF1Kwd(This,pbstrKwd,fF1Kwd) ) 

#define IVsMonitorUserContext_IsF1Lookup(This,fF1Lookup)	\
    ( (This)->lpVtbl -> IsF1Lookup(This,fF1Lookup) ) 

#endif /* COBJMACROS */


#endif 	/* C style interface */




#endif 	/* __IVsMonitorUserContext_INTERFACE_DEFINED__ */


/* interface __MIDL_itf_context_0000_0008 */
/* [local] */ 


enum _VSCIPPROPID
    {	VSCIPPROPID_NIL	= -1,
	VSCIPPROPID_LookupType	= 100,
	VSCIPPROPID_Customize	= 200
    } ;
typedef LONG VSCIPPROPID;



extern RPC_IF_HANDLE __MIDL_itf_context_0000_0008_v0_0_c_ifspec;
extern RPC_IF_HANDLE __MIDL_itf_context_0000_0008_v0_0_s_ifspec;

#ifndef __IVsUserContextItemProvider_INTERFACE_DEFINED__
#define __IVsUserContextItemProvider_INTERFACE_DEFINED__

/* interface IVsUserContextItemProvider */
/* [version][object][uuid] */ 


EXTERN_C const IID IID_IVsUserContextItemProvider;

#if defined(__cplusplus) && !defined(CINTERFACE)
    
    MIDL_INTERFACE("715C98B7-05FB-4A1A-86C8-FF00CE2E5D64")
    IVsUserContextItemProvider : public IUnknown
    {
    public:
        virtual HRESULT STDMETHODCALLTYPE GetProperty( 
            /* [in] */ VSCIPPROPID property,
            /* [retval][out] */ __RPC__out VARIANT *pvar) = 0;
        
        virtual HRESULT STDMETHODCALLTYPE SetProperty( 
            /* [in] */ VSCIPPROPID property,
            /* [in] */ VARIANT var) = 0;
        
        virtual HRESULT STDMETHODCALLTYPE KeywordLookup( 
            /* [in] */ __RPC__in LPCOLESTR pwzTargetAttr,
            /* [in] */ __RPC__in LPCOLESTR pwzTargetAttrValue,
            /* [out] */ __RPC__deref_out_opt IVsUserContextItemCollection **ppList,
            /* [in] */ __RPC__in_opt IVsMonitorUserContext *pCMUC,
            /* [in] */ BOOL fCheckIdle,
            /* [in] */ BOOL fContinueInterrupt) = 0;
        
        virtual HRESULT STDMETHODCALLTYPE PackedAttributeLookup( 
            /* [in] */ __RPC__in LPCOLESTR pwzRequired,
            /* [in] */ __RPC__in LPCOLESTR pwzScope,
            /* [out] */ __RPC__deref_out_opt IVsUserContextItemCollection **ppList) = 0;
        
        virtual HRESULT STDMETHODCALLTYPE LookupEnabled( 
            /* [out] */ __RPC__out BOOL *pfLookupEnabled) = 0;
        
    };
    
#else 	/* C style interface */

    typedef struct IVsUserContextItemProviderVtbl
    {
        BEGIN_INTERFACE
        
        HRESULT ( STDMETHODCALLTYPE *QueryInterface )( 
            IVsUserContextItemProvider * This,
            /* [in] */ __RPC__in REFIID riid,
            /* [iid_is][out] */ 
            __RPC__deref_out  void **ppvObject);
        
        ULONG ( STDMETHODCALLTYPE *AddRef )( 
            IVsUserContextItemProvider * This);
        
        ULONG ( STDMETHODCALLTYPE *Release )( 
            IVsUserContextItemProvider * This);
        
        HRESULT ( STDMETHODCALLTYPE *GetProperty )( 
            IVsUserContextItemProvider * This,
            /* [in] */ VSCIPPROPID property,
            /* [retval][out] */ __RPC__out VARIANT *pvar);
        
        HRESULT ( STDMETHODCALLTYPE *SetProperty )( 
            IVsUserContextItemProvider * This,
            /* [in] */ VSCIPPROPID property,
            /* [in] */ VARIANT var);
        
        HRESULT ( STDMETHODCALLTYPE *KeywordLookup )( 
            IVsUserContextItemProvider * This,
            /* [in] */ __RPC__in LPCOLESTR pwzTargetAttr,
            /* [in] */ __RPC__in LPCOLESTR pwzTargetAttrValue,
            /* [out] */ __RPC__deref_out_opt IVsUserContextItemCollection **ppList,
            /* [in] */ __RPC__in_opt IVsMonitorUserContext *pCMUC,
            /* [in] */ BOOL fCheckIdle,
            /* [in] */ BOOL fContinueInterrupt);
        
        HRESULT ( STDMETHODCALLTYPE *PackedAttributeLookup )( 
            IVsUserContextItemProvider * This,
            /* [in] */ __RPC__in LPCOLESTR pwzRequired,
            /* [in] */ __RPC__in LPCOLESTR pwzScope,
            /* [out] */ __RPC__deref_out_opt IVsUserContextItemCollection **ppList);
        
        HRESULT ( STDMETHODCALLTYPE *LookupEnabled )( 
            IVsUserContextItemProvider * This,
            /* [out] */ __RPC__out BOOL *pfLookupEnabled);
        
        END_INTERFACE
    } IVsUserContextItemProviderVtbl;

    interface IVsUserContextItemProvider
    {
        CONST_VTBL struct IVsUserContextItemProviderVtbl *lpVtbl;
    };

    

#ifdef COBJMACROS


#define IVsUserContextItemProvider_QueryInterface(This,riid,ppvObject)	\
    ( (This)->lpVtbl -> QueryInterface(This,riid,ppvObject) ) 

#define IVsUserContextItemProvider_AddRef(This)	\
    ( (This)->lpVtbl -> AddRef(This) ) 

#define IVsUserContextItemProvider_Release(This)	\
    ( (This)->lpVtbl -> Release(This) ) 


#define IVsUserContextItemProvider_GetProperty(This,property,pvar)	\
    ( (This)->lpVtbl -> GetProperty(This,property,pvar) ) 

#define IVsUserContextItemProvider_SetProperty(This,property,var)	\
    ( (This)->lpVtbl -> SetProperty(This,property,var) ) 

#define IVsUserContextItemProvider_KeywordLookup(This,pwzTargetAttr,pwzTargetAttrValue,ppList,pCMUC,fCheckIdle,fContinueInterrupt)	\
    ( (This)->lpVtbl -> KeywordLookup(This,pwzTargetAttr,pwzTargetAttrValue,ppList,pCMUC,fCheckIdle,fContinueInterrupt) ) 

#define IVsUserContextItemProvider_PackedAttributeLookup(This,pwzRequired,pwzScope,ppList)	\
    ( (This)->lpVtbl -> PackedAttributeLookup(This,pwzRequired,pwzScope,ppList) ) 

#define IVsUserContextItemProvider_LookupEnabled(This,pfLookupEnabled)	\
    ( (This)->lpVtbl -> LookupEnabled(This,pfLookupEnabled) ) 

#endif /* COBJMACROS */


#endif 	/* C style interface */




#endif 	/* __IVsUserContextItemProvider_INTERFACE_DEFINED__ */


/* interface __MIDL_itf_context_0000_0009 */
/* [local] */ 


enum _LIMITTOPICSOURCE
    {	CCW_LimKwd_SelOnly	= 0,
	CCW_LimKwd_NoAmbient	= 1,
	CCW_LimKwd_All	= 2,
	CCW_LimKwd_Last	= 3
    } ;
typedef LONG LIMITTOPICSOURCE;



extern RPC_IF_HANDLE __MIDL_itf_context_0000_0009_v0_0_c_ifspec;
extern RPC_IF_HANDLE __MIDL_itf_context_0000_0009_v0_0_s_ifspec;

#ifndef __IVsUserContextCustomize_INTERFACE_DEFINED__
#define __IVsUserContextCustomize_INTERFACE_DEFINED__

/* interface IVsUserContextCustomize */
/* [version][object][uuid] */ 


EXTERN_C const IID IID_IVsUserContextCustomize;

#if defined(__cplusplus) && !defined(CINTERFACE)
    
    MIDL_INTERFACE("0F817159-761D-447e-9600-4C3387F4C0FD")
    IVsUserContextCustomize : public IUnknown
    {
    public:
        virtual HRESULT STDMETHODCALLTYPE GetLimitKeywordSource( 
            /* [retval][out] */ __RPC__out LONG *pLimKwdSrc) = 0;
        
    };
    
#else 	/* C style interface */

    typedef struct IVsUserContextCustomizeVtbl
    {
        BEGIN_INTERFACE
        
        HRESULT ( STDMETHODCALLTYPE *QueryInterface )( 
            IVsUserContextCustomize * This,
            /* [in] */ __RPC__in REFIID riid,
            /* [iid_is][out] */ 
            __RPC__deref_out  void **ppvObject);
        
        ULONG ( STDMETHODCALLTYPE *AddRef )( 
            IVsUserContextCustomize * This);
        
        ULONG ( STDMETHODCALLTYPE *Release )( 
            IVsUserContextCustomize * This);
        
        HRESULT ( STDMETHODCALLTYPE *GetLimitKeywordSource )( 
            IVsUserContextCustomize * This,
            /* [retval][out] */ __RPC__out LONG *pLimKwdSrc);
        
        END_INTERFACE
    } IVsUserContextCustomizeVtbl;

    interface IVsUserContextCustomize
    {
        CONST_VTBL struct IVsUserContextCustomizeVtbl *lpVtbl;
    };

    

#ifdef COBJMACROS


#define IVsUserContextCustomize_QueryInterface(This,riid,ppvObject)	\
    ( (This)->lpVtbl -> QueryInterface(This,riid,ppvObject) ) 

#define IVsUserContextCustomize_AddRef(This)	\
    ( (This)->lpVtbl -> AddRef(This) ) 

#define IVsUserContextCustomize_Release(This)	\
    ( (This)->lpVtbl -> Release(This) ) 


#define IVsUserContextCustomize_GetLimitKeywordSource(This,pLimKwdSrc)	\
    ( (This)->lpVtbl -> GetLimitKeywordSource(This,pLimKwdSrc) ) 

#endif /* COBJMACROS */


#endif 	/* C style interface */




#endif 	/* __IVsUserContextCustomize_INTERFACE_DEFINED__ */


#ifndef __IVsUserContextItemEvents_INTERFACE_DEFINED__
#define __IVsUserContextItemEvents_INTERFACE_DEFINED__

/* interface IVsUserContextItemEvents */
/* [object][version][uuid] */ 


EXTERN_C const IID IID_IVsUserContextItemEvents;

#if defined(__cplusplus) && !defined(CINTERFACE)
    
    MIDL_INTERFACE("A2078F0E-A310-420A-BA27-16531905B88F")
    IVsUserContextItemEvents : public IUnknown
    {
    public:
        virtual HRESULT STDMETHODCALLTYPE OnUserContextItemsAvailable( 
            /* [in] */ __RPC__in_opt IVsUserContextItemCollection *pList) = 0;
        
    };
    
#else 	/* C style interface */

    typedef struct IVsUserContextItemEventsVtbl
    {
        BEGIN_INTERFACE
        
        HRESULT ( STDMETHODCALLTYPE *QueryInterface )( 
            IVsUserContextItemEvents * This,
            /* [in] */ __RPC__in REFIID riid,
            /* [iid_is][out] */ 
            __RPC__deref_out  void **ppvObject);
        
        ULONG ( STDMETHODCALLTYPE *AddRef )( 
            IVsUserContextItemEvents * This);
        
        ULONG ( STDMETHODCALLTYPE *Release )( 
            IVsUserContextItemEvents * This);
        
        HRESULT ( STDMETHODCALLTYPE *OnUserContextItemsAvailable )( 
            IVsUserContextItemEvents * This,
            /* [in] */ __RPC__in_opt IVsUserContextItemCollection *pList);
        
        END_INTERFACE
    } IVsUserContextItemEventsVtbl;

    interface IVsUserContextItemEvents
    {
        CONST_VTBL struct IVsUserContextItemEventsVtbl *lpVtbl;
    };

    

#ifdef COBJMACROS


#define IVsUserContextItemEvents_QueryInterface(This,riid,ppvObject)	\
    ( (This)->lpVtbl -> QueryInterface(This,riid,ppvObject) ) 

#define IVsUserContextItemEvents_AddRef(This)	\
    ( (This)->lpVtbl -> AddRef(This) ) 

#define IVsUserContextItemEvents_Release(This)	\
    ( (This)->lpVtbl -> Release(This) ) 


#define IVsUserContextItemEvents_OnUserContextItemsAvailable(This,pList)	\
    ( (This)->lpVtbl -> OnUserContextItemsAvailable(This,pList) ) 

#endif /* COBJMACROS */


#endif 	/* C style interface */




#endif 	/* __IVsUserContextItemEvents_INTERFACE_DEFINED__ */



#ifndef __VsContext_LIBRARY_DEFINED__
#define __VsContext_LIBRARY_DEFINED__

/* library VsContext */
/* [version][uuid] */ 


EXTERN_C const IID LIBID_VsContext;

EXTERN_C const CLSID CLSID_VsContextClass;

#ifdef __cplusplus

class DECLSPEC_UUID("3c1f59c6-69cf-11d2-aa7c-00c04f990343")
VsContextClass;
#endif
#endif /* __VsContext_LIBRARY_DEFINED__ */

/* interface __MIDL_itf_context_0000_0011 */
/* [local] */ 

#define SID_SVsMonitorUserContext IID_IVsMonitorUserContext
#define HH_1x_ATTR L"HtmlHelp_1.X_LookupInfo"
#define KWD_GUID L"KWD_GUID"
#define KEYWORD_CS L"KEYWORD_CS"


extern RPC_IF_HANDLE __MIDL_itf_context_0000_0011_v0_0_c_ifspec;
extern RPC_IF_HANDLE __MIDL_itf_context_0000_0011_v0_0_s_ifspec;

/* Additional Prototypes for ALL interfaces */

unsigned long             __RPC_USER  BSTR_UserSize(     unsigned long *, unsigned long            , BSTR * ); 
unsigned char * __RPC_USER  BSTR_UserMarshal(  unsigned long *, unsigned char *, BSTR * ); 
unsigned char * __RPC_USER  BSTR_UserUnmarshal(unsigned long *, unsigned char *, BSTR * ); 
void                      __RPC_USER  BSTR_UserFree(     unsigned long *, BSTR * ); 

unsigned long             __RPC_USER  VARIANT_UserSize(     unsigned long *, unsigned long            , VARIANT * ); 
unsigned char * __RPC_USER  VARIANT_UserMarshal(  unsigned long *, unsigned char *, VARIANT * ); 
unsigned char * __RPC_USER  VARIANT_UserUnmarshal(unsigned long *, unsigned char *, VARIANT * ); 
void                      __RPC_USER  VARIANT_UserFree(     unsigned long *, VARIANT * ); 

/* end of Additional Prototypes */

#ifdef __cplusplus
}
#endif

#endif


