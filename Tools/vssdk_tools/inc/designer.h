

/* this ALWAYS GENERATED file contains the definitions for the interfaces */


 /* File created by MIDL compiler version 7.00.0499 */
/* Compiler settings for designer.idl:
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

#ifndef __designer_h__
#define __designer_h__

#if defined(_MSC_VER) && (_MSC_VER >= 1020)
#pragma once
#endif

/* Forward Declarations */ 

#ifndef __IActiveDesigner_FWD_DEFINED__
#define __IActiveDesigner_FWD_DEFINED__
typedef interface IActiveDesigner IActiveDesigner;
#endif 	/* __IActiveDesigner_FWD_DEFINED__ */


#ifndef __ICodeNavigate_FWD_DEFINED__
#define __ICodeNavigate_FWD_DEFINED__
typedef interface ICodeNavigate ICodeNavigate;
#endif 	/* __ICodeNavigate_FWD_DEFINED__ */


#ifndef __ICodeNavigate2_FWD_DEFINED__
#define __ICodeNavigate2_FWD_DEFINED__
typedef interface ICodeNavigate2 ICodeNavigate2;
#endif 	/* __ICodeNavigate2_FWD_DEFINED__ */


#ifndef __ISelectionContainer_FWD_DEFINED__
#define __ISelectionContainer_FWD_DEFINED__
typedef interface ISelectionContainer ISelectionContainer;
#endif 	/* __ISelectionContainer_FWD_DEFINED__ */


#ifndef __ITrackSelection_FWD_DEFINED__
#define __ITrackSelection_FWD_DEFINED__
typedef interface ITrackSelection ITrackSelection;
#endif 	/* __ITrackSelection_FWD_DEFINED__ */


#ifndef __IProfferTypeLib_FWD_DEFINED__
#define __IProfferTypeLib_FWD_DEFINED__
typedef interface IProfferTypeLib IProfferTypeLib;
#endif 	/* __IProfferTypeLib_FWD_DEFINED__ */


#ifndef __IProvideDynamicClassInfo_FWD_DEFINED__
#define __IProvideDynamicClassInfo_FWD_DEFINED__
typedef interface IProvideDynamicClassInfo IProvideDynamicClassInfo;
#endif 	/* __IProvideDynamicClassInfo_FWD_DEFINED__ */


#ifndef __IExtendedObject_FWD_DEFINED__
#define __IExtendedObject_FWD_DEFINED__
typedef interface IExtendedObject IExtendedObject;
#endif 	/* __IExtendedObject_FWD_DEFINED__ */


/* header files for imported files */
#include "oleidl.h"
#include "servprov.h"
#include "oaidl.h"
#include "ocidl.h"

#ifdef __cplusplus
extern "C"{
#endif 


/* interface __MIDL_itf_designer_0000_0000 */
/* [local] */ 

//+-------------------------------------------------------------------------
//
//  Microsoft Windows
//  Copyright 1995 - 1997 Microsoft Corporation. All Rights Reserved.
//
//  File: designer.h
//
//--------------------------------------------------------------------------
#ifndef _DESIGNER_H_
#define _DESIGNER_H_
extern const __declspec(selectany) GUID CATID_Designer = {0x4eb304d0, 0x7555, 0x11cf, 0xa0, 0xc2, 0x00, 0xaa, 0x00, 0x62, 0xbe, 0x57};


extern RPC_IF_HANDLE __MIDL_itf_designer_0000_0000_v0_0_c_ifspec;
extern RPC_IF_HANDLE __MIDL_itf_designer_0000_0000_v0_0_s_ifspec;

#ifndef __IActiveDesigner_INTERFACE_DEFINED__
#define __IActiveDesigner_INTERFACE_DEFINED__

/* interface IActiveDesigner */
/* [unique][uuid][local][object] */ 

typedef /* [unique] */ IActiveDesigner *LPACTIVEDESIGNER;


EXTERN_C const IID IID_IActiveDesigner;

#if defined(__cplusplus) && !defined(CINTERFACE)
    
    MIDL_INTERFACE("51aae3e0-7486-11cf-a0C2-00aa0062be57")
    IActiveDesigner : public IUnknown
    {
    public:
        virtual HRESULT STDMETHODCALLTYPE GetRuntimeClassID( 
            /* [out] */ CLSID *pclsid) = 0;
        
        virtual HRESULT STDMETHODCALLTYPE GetRuntimeMiscStatusFlags( 
            /* [out] */ DWORD *pdwMiscFlags) = 0;
        
        virtual HRESULT STDMETHODCALLTYPE QueryPersistenceInterface( 
            /* [in] */ REFIID riidPersist) = 0;
        
        virtual HRESULT STDMETHODCALLTYPE SaveRuntimeState( 
            /* [in] */ REFIID riidPersist,
            /* [in] */ REFIID riidObjStgMed,
            /* [in] */ void *pObjStgMed) = 0;
        
        virtual HRESULT STDMETHODCALLTYPE GetExtensibilityObject( 
            /* [out] */ IDispatch **ppvObjOut) = 0;
        
    };
    
#else 	/* C style interface */

    typedef struct IActiveDesignerVtbl
    {
        BEGIN_INTERFACE
        
        HRESULT ( STDMETHODCALLTYPE *QueryInterface )( 
            IActiveDesigner * This,
            /* [in] */ REFIID riid,
            /* [iid_is][out] */ 
            __RPC__deref_out  void **ppvObject);
        
        ULONG ( STDMETHODCALLTYPE *AddRef )( 
            IActiveDesigner * This);
        
        ULONG ( STDMETHODCALLTYPE *Release )( 
            IActiveDesigner * This);
        
        HRESULT ( STDMETHODCALLTYPE *GetRuntimeClassID )( 
            IActiveDesigner * This,
            /* [out] */ CLSID *pclsid);
        
        HRESULT ( STDMETHODCALLTYPE *GetRuntimeMiscStatusFlags )( 
            IActiveDesigner * This,
            /* [out] */ DWORD *pdwMiscFlags);
        
        HRESULT ( STDMETHODCALLTYPE *QueryPersistenceInterface )( 
            IActiveDesigner * This,
            /* [in] */ REFIID riidPersist);
        
        HRESULT ( STDMETHODCALLTYPE *SaveRuntimeState )( 
            IActiveDesigner * This,
            /* [in] */ REFIID riidPersist,
            /* [in] */ REFIID riidObjStgMed,
            /* [in] */ void *pObjStgMed);
        
        HRESULT ( STDMETHODCALLTYPE *GetExtensibilityObject )( 
            IActiveDesigner * This,
            /* [out] */ IDispatch **ppvObjOut);
        
        END_INTERFACE
    } IActiveDesignerVtbl;

    interface IActiveDesigner
    {
        CONST_VTBL struct IActiveDesignerVtbl *lpVtbl;
    };

    

#ifdef COBJMACROS


#define IActiveDesigner_QueryInterface(This,riid,ppvObject)	\
    ( (This)->lpVtbl -> QueryInterface(This,riid,ppvObject) ) 

#define IActiveDesigner_AddRef(This)	\
    ( (This)->lpVtbl -> AddRef(This) ) 

#define IActiveDesigner_Release(This)	\
    ( (This)->lpVtbl -> Release(This) ) 


#define IActiveDesigner_GetRuntimeClassID(This,pclsid)	\
    ( (This)->lpVtbl -> GetRuntimeClassID(This,pclsid) ) 

#define IActiveDesigner_GetRuntimeMiscStatusFlags(This,pdwMiscFlags)	\
    ( (This)->lpVtbl -> GetRuntimeMiscStatusFlags(This,pdwMiscFlags) ) 

#define IActiveDesigner_QueryPersistenceInterface(This,riidPersist)	\
    ( (This)->lpVtbl -> QueryPersistenceInterface(This,riidPersist) ) 

#define IActiveDesigner_SaveRuntimeState(This,riidPersist,riidObjStgMed,pObjStgMed)	\
    ( (This)->lpVtbl -> SaveRuntimeState(This,riidPersist,riidObjStgMed,pObjStgMed) ) 

#define IActiveDesigner_GetExtensibilityObject(This,ppvObjOut)	\
    ( (This)->lpVtbl -> GetExtensibilityObject(This,ppvObjOut) ) 

#endif /* COBJMACROS */


#endif 	/* C style interface */




#endif 	/* __IActiveDesigner_INTERFACE_DEFINED__ */


#ifndef __ICodeNavigate_INTERFACE_DEFINED__
#define __ICodeNavigate_INTERFACE_DEFINED__

/* interface ICodeNavigate */
/* [unique][uuid][local][object] */ 

typedef /* [unique] */ ICodeNavigate *LPCODENAVIGATE;


EXTERN_C const IID IID_ICodeNavigate;

#if defined(__cplusplus) && !defined(CINTERFACE)
    
    MIDL_INTERFACE("6d5140c4-7436-11ce-8034-00aa006009fa")
    ICodeNavigate : public IUnknown
    {
    public:
        virtual HRESULT STDMETHODCALLTYPE DisplayDefaultEventHandler( 
            /* [in] */ LPCOLESTR lpstrObjectName) = 0;
        
    };
    
#else 	/* C style interface */

    typedef struct ICodeNavigateVtbl
    {
        BEGIN_INTERFACE
        
        HRESULT ( STDMETHODCALLTYPE *QueryInterface )( 
            ICodeNavigate * This,
            /* [in] */ REFIID riid,
            /* [iid_is][out] */ 
            __RPC__deref_out  void **ppvObject);
        
        ULONG ( STDMETHODCALLTYPE *AddRef )( 
            ICodeNavigate * This);
        
        ULONG ( STDMETHODCALLTYPE *Release )( 
            ICodeNavigate * This);
        
        HRESULT ( STDMETHODCALLTYPE *DisplayDefaultEventHandler )( 
            ICodeNavigate * This,
            /* [in] */ LPCOLESTR lpstrObjectName);
        
        END_INTERFACE
    } ICodeNavigateVtbl;

    interface ICodeNavigate
    {
        CONST_VTBL struct ICodeNavigateVtbl *lpVtbl;
    };

    

#ifdef COBJMACROS


#define ICodeNavigate_QueryInterface(This,riid,ppvObject)	\
    ( (This)->lpVtbl -> QueryInterface(This,riid,ppvObject) ) 

#define ICodeNavigate_AddRef(This)	\
    ( (This)->lpVtbl -> AddRef(This) ) 

#define ICodeNavigate_Release(This)	\
    ( (This)->lpVtbl -> Release(This) ) 


#define ICodeNavigate_DisplayDefaultEventHandler(This,lpstrObjectName)	\
    ( (This)->lpVtbl -> DisplayDefaultEventHandler(This,lpstrObjectName) ) 

#endif /* COBJMACROS */


#endif 	/* C style interface */




#endif 	/* __ICodeNavigate_INTERFACE_DEFINED__ */


/* interface __MIDL_itf_designer_0000_0002 */
/* [local] */ 

#define SID_SCodeNavigate IID_ICodeNavigate


extern RPC_IF_HANDLE __MIDL_itf_designer_0000_0002_v0_0_c_ifspec;
extern RPC_IF_HANDLE __MIDL_itf_designer_0000_0002_v0_0_s_ifspec;

#ifndef __ICodeNavigate2_INTERFACE_DEFINED__
#define __ICodeNavigate2_INTERFACE_DEFINED__

/* interface ICodeNavigate2 */
/* [unique][uuid][local][object] */ 


EXTERN_C const IID IID_ICodeNavigate2;

#if defined(__cplusplus) && !defined(CINTERFACE)
    
    MIDL_INTERFACE("2702ad60-3459-11d1-88fd-00a0c9110049")
    ICodeNavigate2 : public ICodeNavigate
    {
    public:
        virtual HRESULT STDMETHODCALLTYPE DisplayEventHandler( 
            /* [in] */ LPCOLESTR lpstrObjectName,
            /* [in] */ LPCOLESTR lpstrEventName) = 0;
        
    };
    
#else 	/* C style interface */

    typedef struct ICodeNavigate2Vtbl
    {
        BEGIN_INTERFACE
        
        HRESULT ( STDMETHODCALLTYPE *QueryInterface )( 
            ICodeNavigate2 * This,
            /* [in] */ REFIID riid,
            /* [iid_is][out] */ 
            __RPC__deref_out  void **ppvObject);
        
        ULONG ( STDMETHODCALLTYPE *AddRef )( 
            ICodeNavigate2 * This);
        
        ULONG ( STDMETHODCALLTYPE *Release )( 
            ICodeNavigate2 * This);
        
        HRESULT ( STDMETHODCALLTYPE *DisplayDefaultEventHandler )( 
            ICodeNavigate2 * This,
            /* [in] */ LPCOLESTR lpstrObjectName);
        
        HRESULT ( STDMETHODCALLTYPE *DisplayEventHandler )( 
            ICodeNavigate2 * This,
            /* [in] */ LPCOLESTR lpstrObjectName,
            /* [in] */ LPCOLESTR lpstrEventName);
        
        END_INTERFACE
    } ICodeNavigate2Vtbl;

    interface ICodeNavigate2
    {
        CONST_VTBL struct ICodeNavigate2Vtbl *lpVtbl;
    };

    

#ifdef COBJMACROS


#define ICodeNavigate2_QueryInterface(This,riid,ppvObject)	\
    ( (This)->lpVtbl -> QueryInterface(This,riid,ppvObject) ) 

#define ICodeNavigate2_AddRef(This)	\
    ( (This)->lpVtbl -> AddRef(This) ) 

#define ICodeNavigate2_Release(This)	\
    ( (This)->lpVtbl -> Release(This) ) 


#define ICodeNavigate2_DisplayDefaultEventHandler(This,lpstrObjectName)	\
    ( (This)->lpVtbl -> DisplayDefaultEventHandler(This,lpstrObjectName) ) 


#define ICodeNavigate2_DisplayEventHandler(This,lpstrObjectName,lpstrEventName)	\
    ( (This)->lpVtbl -> DisplayEventHandler(This,lpstrObjectName,lpstrEventName) ) 

#endif /* COBJMACROS */


#endif 	/* C style interface */




#endif 	/* __ICodeNavigate2_INTERFACE_DEFINED__ */


/* interface __MIDL_itf_designer_0000_0003 */
/* [local] */ 

#define GETOBJS_ALL         1
#define GETOBJS_SELECTED    2
#define SELOBJS_ACTIVATE_WINDOW   1


extern RPC_IF_HANDLE __MIDL_itf_designer_0000_0003_v0_0_c_ifspec;
extern RPC_IF_HANDLE __MIDL_itf_designer_0000_0003_v0_0_s_ifspec;

#ifndef __ISelectionContainer_INTERFACE_DEFINED__
#define __ISelectionContainer_INTERFACE_DEFINED__

/* interface ISelectionContainer */
/* [unique][uuid][local][object] */ 

typedef /* [unique] */ ISelectionContainer *LPSELECTIONCONTAINER;


EXTERN_C const IID IID_ISelectionContainer;

#if defined(__cplusplus) && !defined(CINTERFACE)
    
    MIDL_INTERFACE("6d5140c6-7436-11ce-8034-00aa006009fa")
    ISelectionContainer : public IUnknown
    {
    public:
        virtual HRESULT STDMETHODCALLTYPE CountObjects( 
            /* [in] */ DWORD dwFlags,
            /* [out] */ ULONG *pc) = 0;
        
        virtual HRESULT STDMETHODCALLTYPE GetObjects( 
            /* [in] */ DWORD dwFlags,
            /* [in] */ ULONG cObjects,
            /* [size_is][out] */ IUnknown **apUnkObjects) = 0;
        
        virtual HRESULT STDMETHODCALLTYPE SelectObjects( 
            /* [in] */ ULONG cSelect,
            /* [size_is][in] */ IUnknown **apUnkSelect,
            /* [in] */ DWORD dwFlags) = 0;
        
    };
    
#else 	/* C style interface */

    typedef struct ISelectionContainerVtbl
    {
        BEGIN_INTERFACE
        
        HRESULT ( STDMETHODCALLTYPE *QueryInterface )( 
            ISelectionContainer * This,
            /* [in] */ REFIID riid,
            /* [iid_is][out] */ 
            __RPC__deref_out  void **ppvObject);
        
        ULONG ( STDMETHODCALLTYPE *AddRef )( 
            ISelectionContainer * This);
        
        ULONG ( STDMETHODCALLTYPE *Release )( 
            ISelectionContainer * This);
        
        HRESULT ( STDMETHODCALLTYPE *CountObjects )( 
            ISelectionContainer * This,
            /* [in] */ DWORD dwFlags,
            /* [out] */ ULONG *pc);
        
        HRESULT ( STDMETHODCALLTYPE *GetObjects )( 
            ISelectionContainer * This,
            /* [in] */ DWORD dwFlags,
            /* [in] */ ULONG cObjects,
            /* [size_is][out] */ IUnknown **apUnkObjects);
        
        HRESULT ( STDMETHODCALLTYPE *SelectObjects )( 
            ISelectionContainer * This,
            /* [in] */ ULONG cSelect,
            /* [size_is][in] */ IUnknown **apUnkSelect,
            /* [in] */ DWORD dwFlags);
        
        END_INTERFACE
    } ISelectionContainerVtbl;

    interface ISelectionContainer
    {
        CONST_VTBL struct ISelectionContainerVtbl *lpVtbl;
    };

    

#ifdef COBJMACROS


#define ISelectionContainer_QueryInterface(This,riid,ppvObject)	\
    ( (This)->lpVtbl -> QueryInterface(This,riid,ppvObject) ) 

#define ISelectionContainer_AddRef(This)	\
    ( (This)->lpVtbl -> AddRef(This) ) 

#define ISelectionContainer_Release(This)	\
    ( (This)->lpVtbl -> Release(This) ) 


#define ISelectionContainer_CountObjects(This,dwFlags,pc)	\
    ( (This)->lpVtbl -> CountObjects(This,dwFlags,pc) ) 

#define ISelectionContainer_GetObjects(This,dwFlags,cObjects,apUnkObjects)	\
    ( (This)->lpVtbl -> GetObjects(This,dwFlags,cObjects,apUnkObjects) ) 

#define ISelectionContainer_SelectObjects(This,cSelect,apUnkSelect,dwFlags)	\
    ( (This)->lpVtbl -> SelectObjects(This,cSelect,apUnkSelect,dwFlags) ) 

#endif /* COBJMACROS */


#endif 	/* C style interface */




#endif 	/* __ISelectionContainer_INTERFACE_DEFINED__ */


#ifndef __ITrackSelection_INTERFACE_DEFINED__
#define __ITrackSelection_INTERFACE_DEFINED__

/* interface ITrackSelection */
/* [unique][uuid][local][object] */ 

typedef /* [unique] */ ITrackSelection *LPTRACKSELECTION;


EXTERN_C const IID IID_ITrackSelection;

#if defined(__cplusplus) && !defined(CINTERFACE)
    
    MIDL_INTERFACE("6d5140c5-7436-11ce-8034-00aa006009fa")
    ITrackSelection : public IUnknown
    {
    public:
        virtual HRESULT STDMETHODCALLTYPE OnSelectChange( 
            /* [in] */ ISelectionContainer *pSC) = 0;
        
    };
    
#else 	/* C style interface */

    typedef struct ITrackSelectionVtbl
    {
        BEGIN_INTERFACE
        
        HRESULT ( STDMETHODCALLTYPE *QueryInterface )( 
            ITrackSelection * This,
            /* [in] */ REFIID riid,
            /* [iid_is][out] */ 
            __RPC__deref_out  void **ppvObject);
        
        ULONG ( STDMETHODCALLTYPE *AddRef )( 
            ITrackSelection * This);
        
        ULONG ( STDMETHODCALLTYPE *Release )( 
            ITrackSelection * This);
        
        HRESULT ( STDMETHODCALLTYPE *OnSelectChange )( 
            ITrackSelection * This,
            /* [in] */ ISelectionContainer *pSC);
        
        END_INTERFACE
    } ITrackSelectionVtbl;

    interface ITrackSelection
    {
        CONST_VTBL struct ITrackSelectionVtbl *lpVtbl;
    };

    

#ifdef COBJMACROS


#define ITrackSelection_QueryInterface(This,riid,ppvObject)	\
    ( (This)->lpVtbl -> QueryInterface(This,riid,ppvObject) ) 

#define ITrackSelection_AddRef(This)	\
    ( (This)->lpVtbl -> AddRef(This) ) 

#define ITrackSelection_Release(This)	\
    ( (This)->lpVtbl -> Release(This) ) 


#define ITrackSelection_OnSelectChange(This,pSC)	\
    ( (This)->lpVtbl -> OnSelectChange(This,pSC) ) 

#endif /* COBJMACROS */


#endif 	/* C style interface */




#endif 	/* __ITrackSelection_INTERFACE_DEFINED__ */


/* interface __MIDL_itf_designer_0000_0005 */
/* [local] */ 

#define SID_STrackSelection IID_ITrackSelection
#define CONTROLTYPELIB       (0x00000001)


extern RPC_IF_HANDLE __MIDL_itf_designer_0000_0005_v0_0_c_ifspec;
extern RPC_IF_HANDLE __MIDL_itf_designer_0000_0005_v0_0_s_ifspec;

#ifndef __IProfferTypeLib_INTERFACE_DEFINED__
#define __IProfferTypeLib_INTERFACE_DEFINED__

/* interface IProfferTypeLib */
/* [unique][uuid][local][object] */ 

typedef /* [unique] */ IProfferTypeLib *LPPROFFERTYPELIB;


EXTERN_C const IID IID_IProfferTypeLib;

#if defined(__cplusplus) && !defined(CINTERFACE)
    
    MIDL_INTERFACE("718cc500-0a76-11cf-8045-00aa006009fa")
    IProfferTypeLib : public IUnknown
    {
    public:
        virtual HRESULT STDMETHODCALLTYPE ProfferTypeLib( 
            /* [in] */ REFGUID guidTypeLib,
            /* [in] */ UINT uVerMaj,
            /* [in] */ UINT uVerMin,
            /* [in] */ DWORD dwFlags) = 0;
        
    };
    
#else 	/* C style interface */

    typedef struct IProfferTypeLibVtbl
    {
        BEGIN_INTERFACE
        
        HRESULT ( STDMETHODCALLTYPE *QueryInterface )( 
            IProfferTypeLib * This,
            /* [in] */ REFIID riid,
            /* [iid_is][out] */ 
            __RPC__deref_out  void **ppvObject);
        
        ULONG ( STDMETHODCALLTYPE *AddRef )( 
            IProfferTypeLib * This);
        
        ULONG ( STDMETHODCALLTYPE *Release )( 
            IProfferTypeLib * This);
        
        HRESULT ( STDMETHODCALLTYPE *ProfferTypeLib )( 
            IProfferTypeLib * This,
            /* [in] */ REFGUID guidTypeLib,
            /* [in] */ UINT uVerMaj,
            /* [in] */ UINT uVerMin,
            /* [in] */ DWORD dwFlags);
        
        END_INTERFACE
    } IProfferTypeLibVtbl;

    interface IProfferTypeLib
    {
        CONST_VTBL struct IProfferTypeLibVtbl *lpVtbl;
    };

    

#ifdef COBJMACROS


#define IProfferTypeLib_QueryInterface(This,riid,ppvObject)	\
    ( (This)->lpVtbl -> QueryInterface(This,riid,ppvObject) ) 

#define IProfferTypeLib_AddRef(This)	\
    ( (This)->lpVtbl -> AddRef(This) ) 

#define IProfferTypeLib_Release(This)	\
    ( (This)->lpVtbl -> Release(This) ) 


#define IProfferTypeLib_ProfferTypeLib(This,guidTypeLib,uVerMaj,uVerMin,dwFlags)	\
    ( (This)->lpVtbl -> ProfferTypeLib(This,guidTypeLib,uVerMaj,uVerMin,dwFlags) ) 

#endif /* COBJMACROS */


#endif 	/* C style interface */




#endif 	/* __IProfferTypeLib_INTERFACE_DEFINED__ */


/* interface __MIDL_itf_designer_0000_0006 */
/* [local] */ 

#define SID_SProfferTypeLib IID_IProfferTypeLib


extern RPC_IF_HANDLE __MIDL_itf_designer_0000_0006_v0_0_c_ifspec;
extern RPC_IF_HANDLE __MIDL_itf_designer_0000_0006_v0_0_s_ifspec;

#ifndef __IProvideDynamicClassInfo_INTERFACE_DEFINED__
#define __IProvideDynamicClassInfo_INTERFACE_DEFINED__

/* interface IProvideDynamicClassInfo */
/* [unique][uuid][local][object] */ 

typedef /* [unique] */ IProvideDynamicClassInfo *LPPROVIDEDYNAMICCLASSINFO;


EXTERN_C const IID IID_IProvideDynamicClassInfo;

#if defined(__cplusplus) && !defined(CINTERFACE)
    
    MIDL_INTERFACE("468cfb80-b4f9-11cf-80dd-00aa00614895")
    IProvideDynamicClassInfo : public IProvideClassInfo
    {
    public:
        virtual HRESULT STDMETHODCALLTYPE GetDynamicClassInfo( 
            /* [out] */ ITypeInfo **ppTI,
            /* [out] */ DWORD_PTR *pdwCookie) = 0;
        
        virtual HRESULT STDMETHODCALLTYPE FreezeShape( void) = 0;
        
    };
    
#else 	/* C style interface */

    typedef struct IProvideDynamicClassInfoVtbl
    {
        BEGIN_INTERFACE
        
        HRESULT ( STDMETHODCALLTYPE *QueryInterface )( 
            IProvideDynamicClassInfo * This,
            /* [in] */ REFIID riid,
            /* [iid_is][out] */ 
            __RPC__deref_out  void **ppvObject);
        
        ULONG ( STDMETHODCALLTYPE *AddRef )( 
            IProvideDynamicClassInfo * This);
        
        ULONG ( STDMETHODCALLTYPE *Release )( 
            IProvideDynamicClassInfo * This);
        
        HRESULT ( STDMETHODCALLTYPE *GetClassInfo )( 
            IProvideDynamicClassInfo * This,
            /* [out] */ ITypeInfo **ppTI);
        
        HRESULT ( STDMETHODCALLTYPE *GetDynamicClassInfo )( 
            IProvideDynamicClassInfo * This,
            /* [out] */ ITypeInfo **ppTI,
            /* [out] */ DWORD_PTR *pdwCookie);
        
        HRESULT ( STDMETHODCALLTYPE *FreezeShape )( 
            IProvideDynamicClassInfo * This);
        
        END_INTERFACE
    } IProvideDynamicClassInfoVtbl;

    interface IProvideDynamicClassInfo
    {
        CONST_VTBL struct IProvideDynamicClassInfoVtbl *lpVtbl;
    };

    

#ifdef COBJMACROS


#define IProvideDynamicClassInfo_QueryInterface(This,riid,ppvObject)	\
    ( (This)->lpVtbl -> QueryInterface(This,riid,ppvObject) ) 

#define IProvideDynamicClassInfo_AddRef(This)	\
    ( (This)->lpVtbl -> AddRef(This) ) 

#define IProvideDynamicClassInfo_Release(This)	\
    ( (This)->lpVtbl -> Release(This) ) 


#define IProvideDynamicClassInfo_GetClassInfo(This,ppTI)	\
    ( (This)->lpVtbl -> GetClassInfo(This,ppTI) ) 


#define IProvideDynamicClassInfo_GetDynamicClassInfo(This,ppTI,pdwCookie)	\
    ( (This)->lpVtbl -> GetDynamicClassInfo(This,ppTI,pdwCookie) ) 

#define IProvideDynamicClassInfo_FreezeShape(This)	\
    ( (This)->lpVtbl -> FreezeShape(This) ) 

#endif /* COBJMACROS */


#endif 	/* C style interface */




#endif 	/* __IProvideDynamicClassInfo_INTERFACE_DEFINED__ */


/* interface __MIDL_itf_designer_0000_0007 */
/* [local] */ 

extern const __declspec(selectany) GUID SID_SApplicationObject = {0x0c539790, 0x12e4, 0x11cf, 0xb6, 0x61, 0x00, 0xaa, 0x00, 0x4c, 0xd6, 0xd8};


extern RPC_IF_HANDLE __MIDL_itf_designer_0000_0007_v0_0_c_ifspec;
extern RPC_IF_HANDLE __MIDL_itf_designer_0000_0007_v0_0_s_ifspec;

#ifndef __IExtendedObject_INTERFACE_DEFINED__
#define __IExtendedObject_INTERFACE_DEFINED__

/* interface IExtendedObject */
/* [object][uuid] */ 


EXTERN_C const IID IID_IExtendedObject;

#if defined(__cplusplus) && !defined(CINTERFACE)
    
    MIDL_INTERFACE("A575C060-5B17-11d1-AB3E-00A0C9055A90")
    IExtendedObject : public IUnknown
    {
    public:
        virtual HRESULT STDMETHODCALLTYPE GetInnerObject( 
            /* [in] */ __RPC__in REFIID iid,
            /* [iid_is][out] */ __RPC__deref_out_opt void **ppvObject) = 0;
        
    };
    
#else 	/* C style interface */

    typedef struct IExtendedObjectVtbl
    {
        BEGIN_INTERFACE
        
        HRESULT ( STDMETHODCALLTYPE *QueryInterface )( 
            IExtendedObject * This,
            /* [in] */ __RPC__in REFIID riid,
            /* [iid_is][out] */ 
            __RPC__deref_out  void **ppvObject);
        
        ULONG ( STDMETHODCALLTYPE *AddRef )( 
            IExtendedObject * This);
        
        ULONG ( STDMETHODCALLTYPE *Release )( 
            IExtendedObject * This);
        
        HRESULT ( STDMETHODCALLTYPE *GetInnerObject )( 
            IExtendedObject * This,
            /* [in] */ __RPC__in REFIID iid,
            /* [iid_is][out] */ __RPC__deref_out_opt void **ppvObject);
        
        END_INTERFACE
    } IExtendedObjectVtbl;

    interface IExtendedObject
    {
        CONST_VTBL struct IExtendedObjectVtbl *lpVtbl;
    };

    

#ifdef COBJMACROS


#define IExtendedObject_QueryInterface(This,riid,ppvObject)	\
    ( (This)->lpVtbl -> QueryInterface(This,riid,ppvObject) ) 

#define IExtendedObject_AddRef(This)	\
    ( (This)->lpVtbl -> AddRef(This) ) 

#define IExtendedObject_Release(This)	\
    ( (This)->lpVtbl -> Release(This) ) 


#define IExtendedObject_GetInnerObject(This,iid,ppvObject)	\
    ( (This)->lpVtbl -> GetInnerObject(This,iid,ppvObject) ) 

#endif /* COBJMACROS */


#endif 	/* C style interface */




#endif 	/* __IExtendedObject_INTERFACE_DEFINED__ */


/* interface __MIDL_itf_designer_0000_0008 */
/* [local] */ 

#endif


extern RPC_IF_HANDLE __MIDL_itf_designer_0000_0008_v0_0_c_ifspec;
extern RPC_IF_HANDLE __MIDL_itf_designer_0000_0008_v0_0_s_ifspec;

/* Additional Prototypes for ALL interfaces */

/* end of Additional Prototypes */

#ifdef __cplusplus
}
#endif

#endif


