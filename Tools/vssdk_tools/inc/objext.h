

/* this ALWAYS GENERATED file contains the definitions for the interfaces */


 /* File created by MIDL compiler version 7.00.0499 */
/* Compiler settings for objext.idl:
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

#ifndef __objext_h__
#define __objext_h__

#if defined(_MSC_VER) && (_MSC_VER >= 1020)
#pragma once
#endif

/* Forward Declarations */ 

#ifndef __IDocumentSite_FWD_DEFINED__
#define __IDocumentSite_FWD_DEFINED__
typedef interface IDocumentSite IDocumentSite;
#endif 	/* __IDocumentSite_FWD_DEFINED__ */


#ifndef __IDocumentSite2_FWD_DEFINED__
#define __IDocumentSite2_FWD_DEFINED__
typedef interface IDocumentSite2 IDocumentSite2;
#endif 	/* __IDocumentSite2_FWD_DEFINED__ */


#ifndef __IRequireClasses_FWD_DEFINED__
#define __IRequireClasses_FWD_DEFINED__
typedef interface IRequireClasses IRequireClasses;
#endif 	/* __IRequireClasses_FWD_DEFINED__ */


#ifndef __ILicensedClassManager_FWD_DEFINED__
#define __ILicensedClassManager_FWD_DEFINED__
typedef interface ILicensedClassManager ILicensedClassManager;
#endif 	/* __ILicensedClassManager_FWD_DEFINED__ */


#ifndef __IExtendedTypeLib_FWD_DEFINED__
#define __IExtendedTypeLib_FWD_DEFINED__
typedef interface IExtendedTypeLib IExtendedTypeLib;
#endif 	/* __IExtendedTypeLib_FWD_DEFINED__ */


#ifndef __ILocalRegistry_FWD_DEFINED__
#define __ILocalRegistry_FWD_DEFINED__
typedef interface ILocalRegistry ILocalRegistry;
#endif 	/* __ILocalRegistry_FWD_DEFINED__ */


#ifndef __ILocalRegistry2_FWD_DEFINED__
#define __ILocalRegistry2_FWD_DEFINED__
typedef interface ILocalRegistry2 ILocalRegistry2;
#endif 	/* __ILocalRegistry2_FWD_DEFINED__ */


#ifndef __ILocalRegistry3_FWD_DEFINED__
#define __ILocalRegistry3_FWD_DEFINED__
typedef interface ILocalRegistry3 ILocalRegistry3;
#endif 	/* __ILocalRegistry3_FWD_DEFINED__ */


#ifndef __IUIElement_FWD_DEFINED__
#define __IUIElement_FWD_DEFINED__
typedef interface IUIElement IUIElement;
#endif 	/* __IUIElement_FWD_DEFINED__ */


#ifndef __ICategorizeProperties_FWD_DEFINED__
#define __ICategorizeProperties_FWD_DEFINED__
typedef interface ICategorizeProperties ICategorizeProperties;
#endif 	/* __ICategorizeProperties_FWD_DEFINED__ */


#ifndef __IHelp_FWD_DEFINED__
#define __IHelp_FWD_DEFINED__
typedef interface IHelp IHelp;
#endif 	/* __IHelp_FWD_DEFINED__ */


/* header files for imported files */
#include "designer.h"

#ifdef __cplusplus
extern "C"{
#endif 


/* interface __MIDL_itf_objext_0000_0000 */
/* [local] */ 

//+-------------------------------------------------------------------------
//
//  Microsoft Windows
//  Copyright 1995 - 1997 Microsoft Corporation. All Rights Reserved.
//
//  File: objext.h
//
//--------------------------------------------------------------------------
#ifndef _OBJEXT_H_
#define _OBJEXT_H_
#include "proffserv.h"
typedef DWORD ACTFLAG;

#define ACT_DEFAULT 0x00000000
#define ACT_SHOW    0x00000001


extern RPC_IF_HANDLE __MIDL_itf_objext_0000_0000_v0_0_c_ifspec;
extern RPC_IF_HANDLE __MIDL_itf_objext_0000_0000_v0_0_s_ifspec;

#ifndef __IDocumentSite_INTERFACE_DEFINED__
#define __IDocumentSite_INTERFACE_DEFINED__

/* interface IDocumentSite */
/* [unique][uuid][local][object] */ 


EXTERN_C const IID IID_IDocumentSite;

#if defined(__cplusplus) && !defined(CINTERFACE)
    
    MIDL_INTERFACE("94A0F6F1-10BC-11d0-8D09-00A0C90F2732")
    IDocumentSite : public IUnknown
    {
    public:
        virtual HRESULT STDMETHODCALLTYPE SetSite( 
            /* [in] */ IServiceProvider *pSite) = 0;
        
        virtual HRESULT STDMETHODCALLTYPE GetSite( 
            /* [out] */ IServiceProvider **ppSite) = 0;
        
        virtual HRESULT STDMETHODCALLTYPE GetCompiler( 
            /* [in] */ REFIID iid,
            /* [out] */ void **ppvObj) = 0;
        
        virtual HRESULT STDMETHODCALLTYPE ActivateObject( 
            ACTFLAG dwFlags) = 0;
        
        virtual HRESULT STDMETHODCALLTYPE IsObjectShowable( void) = 0;
        
    };
    
#else 	/* C style interface */

    typedef struct IDocumentSiteVtbl
    {
        BEGIN_INTERFACE
        
        HRESULT ( STDMETHODCALLTYPE *QueryInterface )( 
            IDocumentSite * This,
            /* [in] */ REFIID riid,
            /* [iid_is][out] */ 
            __RPC__deref_out  void **ppvObject);
        
        ULONG ( STDMETHODCALLTYPE *AddRef )( 
            IDocumentSite * This);
        
        ULONG ( STDMETHODCALLTYPE *Release )( 
            IDocumentSite * This);
        
        HRESULT ( STDMETHODCALLTYPE *SetSite )( 
            IDocumentSite * This,
            /* [in] */ IServiceProvider *pSite);
        
        HRESULT ( STDMETHODCALLTYPE *GetSite )( 
            IDocumentSite * This,
            /* [out] */ IServiceProvider **ppSite);
        
        HRESULT ( STDMETHODCALLTYPE *GetCompiler )( 
            IDocumentSite * This,
            /* [in] */ REFIID iid,
            /* [out] */ void **ppvObj);
        
        HRESULT ( STDMETHODCALLTYPE *ActivateObject )( 
            IDocumentSite * This,
            ACTFLAG dwFlags);
        
        HRESULT ( STDMETHODCALLTYPE *IsObjectShowable )( 
            IDocumentSite * This);
        
        END_INTERFACE
    } IDocumentSiteVtbl;

    interface IDocumentSite
    {
        CONST_VTBL struct IDocumentSiteVtbl *lpVtbl;
    };

    

#ifdef COBJMACROS


#define IDocumentSite_QueryInterface(This,riid,ppvObject)	\
    ( (This)->lpVtbl -> QueryInterface(This,riid,ppvObject) ) 

#define IDocumentSite_AddRef(This)	\
    ( (This)->lpVtbl -> AddRef(This) ) 

#define IDocumentSite_Release(This)	\
    ( (This)->lpVtbl -> Release(This) ) 


#define IDocumentSite_SetSite(This,pSite)	\
    ( (This)->lpVtbl -> SetSite(This,pSite) ) 

#define IDocumentSite_GetSite(This,ppSite)	\
    ( (This)->lpVtbl -> GetSite(This,ppSite) ) 

#define IDocumentSite_GetCompiler(This,iid,ppvObj)	\
    ( (This)->lpVtbl -> GetCompiler(This,iid,ppvObj) ) 

#define IDocumentSite_ActivateObject(This,dwFlags)	\
    ( (This)->lpVtbl -> ActivateObject(This,dwFlags) ) 

#define IDocumentSite_IsObjectShowable(This)	\
    ( (This)->lpVtbl -> IsObjectShowable(This) ) 

#endif /* COBJMACROS */


#endif 	/* C style interface */




#endif 	/* __IDocumentSite_INTERFACE_DEFINED__ */


/* interface __MIDL_itf_objext_0000_0001 */
/* [local] */ 

#define IClassDesigner IDocumentSite
#define IID_IClassDesigner IID_IDocumentSite


extern RPC_IF_HANDLE __MIDL_itf_objext_0000_0001_v0_0_c_ifspec;
extern RPC_IF_HANDLE __MIDL_itf_objext_0000_0001_v0_0_s_ifspec;

#ifndef __IDocumentSite2_INTERFACE_DEFINED__
#define __IDocumentSite2_INTERFACE_DEFINED__

/* interface IDocumentSite2 */
/* [unique][uuid][local][object] */ 


EXTERN_C const IID IID_IDocumentSite2;

#if defined(__cplusplus) && !defined(CINTERFACE)
    
    MIDL_INTERFACE("61D4A8A1-2C90-11d2-ADE4-00C04F98F417")
    IDocumentSite2 : public IDocumentSite
    {
    public:
        virtual HRESULT STDMETHODCALLTYPE GetObject( 
            /* [out] */ IDispatch **ppDisp) = 0;
        
    };
    
#else 	/* C style interface */

    typedef struct IDocumentSite2Vtbl
    {
        BEGIN_INTERFACE
        
        HRESULT ( STDMETHODCALLTYPE *QueryInterface )( 
            IDocumentSite2 * This,
            /* [in] */ REFIID riid,
            /* [iid_is][out] */ 
            __RPC__deref_out  void **ppvObject);
        
        ULONG ( STDMETHODCALLTYPE *AddRef )( 
            IDocumentSite2 * This);
        
        ULONG ( STDMETHODCALLTYPE *Release )( 
            IDocumentSite2 * This);
        
        HRESULT ( STDMETHODCALLTYPE *SetSite )( 
            IDocumentSite2 * This,
            /* [in] */ IServiceProvider *pSite);
        
        HRESULT ( STDMETHODCALLTYPE *GetSite )( 
            IDocumentSite2 * This,
            /* [out] */ IServiceProvider **ppSite);
        
        HRESULT ( STDMETHODCALLTYPE *GetCompiler )( 
            IDocumentSite2 * This,
            /* [in] */ REFIID iid,
            /* [out] */ void **ppvObj);
        
        HRESULT ( STDMETHODCALLTYPE *ActivateObject )( 
            IDocumentSite2 * This,
            ACTFLAG dwFlags);
        
        HRESULT ( STDMETHODCALLTYPE *IsObjectShowable )( 
            IDocumentSite2 * This);
        
        HRESULT ( STDMETHODCALLTYPE *GetObject )( 
            IDocumentSite2 * This,
            /* [out] */ IDispatch **ppDisp);
        
        END_INTERFACE
    } IDocumentSite2Vtbl;

    interface IDocumentSite2
    {
        CONST_VTBL struct IDocumentSite2Vtbl *lpVtbl;
    };

    

#ifdef COBJMACROS


#define IDocumentSite2_QueryInterface(This,riid,ppvObject)	\
    ( (This)->lpVtbl -> QueryInterface(This,riid,ppvObject) ) 

#define IDocumentSite2_AddRef(This)	\
    ( (This)->lpVtbl -> AddRef(This) ) 

#define IDocumentSite2_Release(This)	\
    ( (This)->lpVtbl -> Release(This) ) 


#define IDocumentSite2_SetSite(This,pSite)	\
    ( (This)->lpVtbl -> SetSite(This,pSite) ) 

#define IDocumentSite2_GetSite(This,ppSite)	\
    ( (This)->lpVtbl -> GetSite(This,ppSite) ) 

#define IDocumentSite2_GetCompiler(This,iid,ppvObj)	\
    ( (This)->lpVtbl -> GetCompiler(This,iid,ppvObj) ) 

#define IDocumentSite2_ActivateObject(This,dwFlags)	\
    ( (This)->lpVtbl -> ActivateObject(This,dwFlags) ) 

#define IDocumentSite2_IsObjectShowable(This)	\
    ( (This)->lpVtbl -> IsObjectShowable(This) ) 


#define IDocumentSite2_GetObject(This,ppDisp)	\
    ( (This)->lpVtbl -> GetObject(This,ppDisp) ) 

#endif /* COBJMACROS */


#endif 	/* C style interface */




#endif 	/* __IDocumentSite2_INTERFACE_DEFINED__ */


#ifndef __IRequireClasses_INTERFACE_DEFINED__
#define __IRequireClasses_INTERFACE_DEFINED__

/* interface IRequireClasses */
/* [unique][uuid][local][object] */ 


EXTERN_C const IID IID_IRequireClasses;

#if defined(__cplusplus) && !defined(CINTERFACE)
    
    MIDL_INTERFACE("6d5140d0-7436-11ce-8034-00aa006009fa")
    IRequireClasses : public IUnknown
    {
    public:
        virtual HRESULT STDMETHODCALLTYPE CountRequiredClasses( 
            /* [out] */ ULONG *pCount) = 0;
        
        virtual HRESULT STDMETHODCALLTYPE GetRequiredClasses( 
            /* [in] */ ULONG index,
            CLSID *pclsid) = 0;
        
    };
    
#else 	/* C style interface */

    typedef struct IRequireClassesVtbl
    {
        BEGIN_INTERFACE
        
        HRESULT ( STDMETHODCALLTYPE *QueryInterface )( 
            IRequireClasses * This,
            /* [in] */ REFIID riid,
            /* [iid_is][out] */ 
            __RPC__deref_out  void **ppvObject);
        
        ULONG ( STDMETHODCALLTYPE *AddRef )( 
            IRequireClasses * This);
        
        ULONG ( STDMETHODCALLTYPE *Release )( 
            IRequireClasses * This);
        
        HRESULT ( STDMETHODCALLTYPE *CountRequiredClasses )( 
            IRequireClasses * This,
            /* [out] */ ULONG *pCount);
        
        HRESULT ( STDMETHODCALLTYPE *GetRequiredClasses )( 
            IRequireClasses * This,
            /* [in] */ ULONG index,
            CLSID *pclsid);
        
        END_INTERFACE
    } IRequireClassesVtbl;

    interface IRequireClasses
    {
        CONST_VTBL struct IRequireClassesVtbl *lpVtbl;
    };

    

#ifdef COBJMACROS


#define IRequireClasses_QueryInterface(This,riid,ppvObject)	\
    ( (This)->lpVtbl -> QueryInterface(This,riid,ppvObject) ) 

#define IRequireClasses_AddRef(This)	\
    ( (This)->lpVtbl -> AddRef(This) ) 

#define IRequireClasses_Release(This)	\
    ( (This)->lpVtbl -> Release(This) ) 


#define IRequireClasses_CountRequiredClasses(This,pCount)	\
    ( (This)->lpVtbl -> CountRequiredClasses(This,pCount) ) 

#define IRequireClasses_GetRequiredClasses(This,index,pclsid)	\
    ( (This)->lpVtbl -> GetRequiredClasses(This,index,pclsid) ) 

#endif /* COBJMACROS */


#endif 	/* C style interface */




#endif 	/* __IRequireClasses_INTERFACE_DEFINED__ */


#ifndef __ILicensedClassManager_INTERFACE_DEFINED__
#define __ILicensedClassManager_INTERFACE_DEFINED__

/* interface ILicensedClassManager */
/* [unique][uuid][local][object] */ 


EXTERN_C const IID IID_ILicensedClassManager;

#if defined(__cplusplus) && !defined(CINTERFACE)
    
    MIDL_INTERFACE("6d5140d4-7436-11ce-8034-00aa006009fa")
    ILicensedClassManager : public IUnknown
    {
    public:
        virtual HRESULT STDMETHODCALLTYPE OnChangeInRequiredClasses( 
            /* [in] */ IRequireClasses *pirc) = 0;
        
    };
    
#else 	/* C style interface */

    typedef struct ILicensedClassManagerVtbl
    {
        BEGIN_INTERFACE
        
        HRESULT ( STDMETHODCALLTYPE *QueryInterface )( 
            ILicensedClassManager * This,
            /* [in] */ REFIID riid,
            /* [iid_is][out] */ 
            __RPC__deref_out  void **ppvObject);
        
        ULONG ( STDMETHODCALLTYPE *AddRef )( 
            ILicensedClassManager * This);
        
        ULONG ( STDMETHODCALLTYPE *Release )( 
            ILicensedClassManager * This);
        
        HRESULT ( STDMETHODCALLTYPE *OnChangeInRequiredClasses )( 
            ILicensedClassManager * This,
            /* [in] */ IRequireClasses *pirc);
        
        END_INTERFACE
    } ILicensedClassManagerVtbl;

    interface ILicensedClassManager
    {
        CONST_VTBL struct ILicensedClassManagerVtbl *lpVtbl;
    };

    

#ifdef COBJMACROS


#define ILicensedClassManager_QueryInterface(This,riid,ppvObject)	\
    ( (This)->lpVtbl -> QueryInterface(This,riid,ppvObject) ) 

#define ILicensedClassManager_AddRef(This)	\
    ( (This)->lpVtbl -> AddRef(This) ) 

#define ILicensedClassManager_Release(This)	\
    ( (This)->lpVtbl -> Release(This) ) 


#define ILicensedClassManager_OnChangeInRequiredClasses(This,pirc)	\
    ( (This)->lpVtbl -> OnChangeInRequiredClasses(This,pirc) ) 

#endif /* COBJMACROS */


#endif 	/* C style interface */




#endif 	/* __ILicensedClassManager_INTERFACE_DEFINED__ */


/* interface __MIDL_itf_objext_0000_0004 */
/* [local] */ 

#define SID_SLicensedClassManager IID_ILicensedClassManager


extern RPC_IF_HANDLE __MIDL_itf_objext_0000_0004_v0_0_c_ifspec;
extern RPC_IF_HANDLE __MIDL_itf_objext_0000_0004_v0_0_s_ifspec;

#ifndef __IExtendedTypeLib_INTERFACE_DEFINED__
#define __IExtendedTypeLib_INTERFACE_DEFINED__

/* interface IExtendedTypeLib */
/* [unique][uuid][local][object] */ 


EXTERN_C const IID IID_IExtendedTypeLib;

#if defined(__cplusplus) && !defined(CINTERFACE)
    
    MIDL_INTERFACE("6d5140d6-7436-11ce-8034-00aa006009fa")
    IExtendedTypeLib : public IUnknown
    {
    public:
        virtual HRESULT STDMETHODCALLTYPE CreateExtendedTypeLib( 
            /* [in] */ LPCOLESTR lpstrCtrlLibFileName,
            /* [in] */ LPCOLESTR lpstrLibNamePrepend,
            /* [in] */ ITypeInfo *ptinfoExtender,
            /* [in] */ DWORD dwReserved,
            /* [in] */ DWORD dwFlags,
            /* [in] */ LPCOLESTR lpstrDirectoryName,
            /* [out] */ ITypeLib **pptLib) = 0;
        
        virtual HRESULT STDMETHODCALLTYPE AddRefExtendedTypeLib( 
            /* [in] */ LPCOLESTR lpstrCtrlLibFileName,
            /* [in] */ LPCOLESTR lpstrLibNamePrepend,
            /* [in] */ ITypeInfo *ptinfoExtender,
            /* [in] */ DWORD dwReserved,
            /* [in] */ DWORD dwFlags,
            /* [in] */ LPCOLESTR lpstrDirectoryName,
            /* [out] */ ITypeLib **pptLib) = 0;
        
        virtual HRESULT STDMETHODCALLTYPE AddRefExtendedTypeLibOfClsid( 
            /* [in] */ REFCLSID rclsidControl,
            /* [in] */ LPCOLESTR lpstrLibNamePrepend,
            /* [in] */ ITypeInfo *ptinfoExtender,
            /* [in] */ DWORD dwReserved,
            /* [in] */ DWORD dwFlags,
            /* [in] */ LPCOLESTR lpstrDirectoryName,
            /* [out] */ ITypeInfo **pptinfo) = 0;
        
        virtual HRESULT STDMETHODCALLTYPE SetExtenderInfo( 
            /* [in] */ LPCOLESTR lpstrDirectoryName,
            /* [in] */ ITypeInfo *ptinfoExtender,
            /* [in] */ DWORD dwReserved) = 0;
        
    };
    
#else 	/* C style interface */

    typedef struct IExtendedTypeLibVtbl
    {
        BEGIN_INTERFACE
        
        HRESULT ( STDMETHODCALLTYPE *QueryInterface )( 
            IExtendedTypeLib * This,
            /* [in] */ REFIID riid,
            /* [iid_is][out] */ 
            __RPC__deref_out  void **ppvObject);
        
        ULONG ( STDMETHODCALLTYPE *AddRef )( 
            IExtendedTypeLib * This);
        
        ULONG ( STDMETHODCALLTYPE *Release )( 
            IExtendedTypeLib * This);
        
        HRESULT ( STDMETHODCALLTYPE *CreateExtendedTypeLib )( 
            IExtendedTypeLib * This,
            /* [in] */ LPCOLESTR lpstrCtrlLibFileName,
            /* [in] */ LPCOLESTR lpstrLibNamePrepend,
            /* [in] */ ITypeInfo *ptinfoExtender,
            /* [in] */ DWORD dwReserved,
            /* [in] */ DWORD dwFlags,
            /* [in] */ LPCOLESTR lpstrDirectoryName,
            /* [out] */ ITypeLib **pptLib);
        
        HRESULT ( STDMETHODCALLTYPE *AddRefExtendedTypeLib )( 
            IExtendedTypeLib * This,
            /* [in] */ LPCOLESTR lpstrCtrlLibFileName,
            /* [in] */ LPCOLESTR lpstrLibNamePrepend,
            /* [in] */ ITypeInfo *ptinfoExtender,
            /* [in] */ DWORD dwReserved,
            /* [in] */ DWORD dwFlags,
            /* [in] */ LPCOLESTR lpstrDirectoryName,
            /* [out] */ ITypeLib **pptLib);
        
        HRESULT ( STDMETHODCALLTYPE *AddRefExtendedTypeLibOfClsid )( 
            IExtendedTypeLib * This,
            /* [in] */ REFCLSID rclsidControl,
            /* [in] */ LPCOLESTR lpstrLibNamePrepend,
            /* [in] */ ITypeInfo *ptinfoExtender,
            /* [in] */ DWORD dwReserved,
            /* [in] */ DWORD dwFlags,
            /* [in] */ LPCOLESTR lpstrDirectoryName,
            /* [out] */ ITypeInfo **pptinfo);
        
        HRESULT ( STDMETHODCALLTYPE *SetExtenderInfo )( 
            IExtendedTypeLib * This,
            /* [in] */ LPCOLESTR lpstrDirectoryName,
            /* [in] */ ITypeInfo *ptinfoExtender,
            /* [in] */ DWORD dwReserved);
        
        END_INTERFACE
    } IExtendedTypeLibVtbl;

    interface IExtendedTypeLib
    {
        CONST_VTBL struct IExtendedTypeLibVtbl *lpVtbl;
    };

    

#ifdef COBJMACROS


#define IExtendedTypeLib_QueryInterface(This,riid,ppvObject)	\
    ( (This)->lpVtbl -> QueryInterface(This,riid,ppvObject) ) 

#define IExtendedTypeLib_AddRef(This)	\
    ( (This)->lpVtbl -> AddRef(This) ) 

#define IExtendedTypeLib_Release(This)	\
    ( (This)->lpVtbl -> Release(This) ) 


#define IExtendedTypeLib_CreateExtendedTypeLib(This,lpstrCtrlLibFileName,lpstrLibNamePrepend,ptinfoExtender,dwReserved,dwFlags,lpstrDirectoryName,pptLib)	\
    ( (This)->lpVtbl -> CreateExtendedTypeLib(This,lpstrCtrlLibFileName,lpstrLibNamePrepend,ptinfoExtender,dwReserved,dwFlags,lpstrDirectoryName,pptLib) ) 

#define IExtendedTypeLib_AddRefExtendedTypeLib(This,lpstrCtrlLibFileName,lpstrLibNamePrepend,ptinfoExtender,dwReserved,dwFlags,lpstrDirectoryName,pptLib)	\
    ( (This)->lpVtbl -> AddRefExtendedTypeLib(This,lpstrCtrlLibFileName,lpstrLibNamePrepend,ptinfoExtender,dwReserved,dwFlags,lpstrDirectoryName,pptLib) ) 

#define IExtendedTypeLib_AddRefExtendedTypeLibOfClsid(This,rclsidControl,lpstrLibNamePrepend,ptinfoExtender,dwReserved,dwFlags,lpstrDirectoryName,pptinfo)	\
    ( (This)->lpVtbl -> AddRefExtendedTypeLibOfClsid(This,rclsidControl,lpstrLibNamePrepend,ptinfoExtender,dwReserved,dwFlags,lpstrDirectoryName,pptinfo) ) 

#define IExtendedTypeLib_SetExtenderInfo(This,lpstrDirectoryName,ptinfoExtender,dwReserved)	\
    ( (This)->lpVtbl -> SetExtenderInfo(This,lpstrDirectoryName,ptinfoExtender,dwReserved) ) 

#endif /* COBJMACROS */


#endif 	/* C style interface */




#endif 	/* __IExtendedTypeLib_INTERFACE_DEFINED__ */


/* interface __MIDL_itf_objext_0000_0005 */
/* [local] */ 

#define SID_SExtendedTypeLib IID_IExtendedTypeLib


extern RPC_IF_HANDLE __MIDL_itf_objext_0000_0005_v0_0_c_ifspec;
extern RPC_IF_HANDLE __MIDL_itf_objext_0000_0005_v0_0_s_ifspec;

#ifndef __ILocalRegistry_INTERFACE_DEFINED__
#define __ILocalRegistry_INTERFACE_DEFINED__

/* interface ILocalRegistry */
/* [unique][uuid][local][object] */ 


EXTERN_C const IID IID_ILocalRegistry;

#if defined(__cplusplus) && !defined(CINTERFACE)
    
    MIDL_INTERFACE("6d5140d3-7436-11ce-8034-00aa006009fa")
    ILocalRegistry : public IUnknown
    {
    public:
        virtual HRESULT STDMETHODCALLTYPE CreateInstance( 
            /* [in] */ CLSID clsid,
            /* [in] */ IUnknown *punkOuter,
            /* [in] */ REFIID riid,
            /* [in] */ DWORD dwFlags,
            /* [out] */ void **ppvObj) = 0;
        
        virtual HRESULT STDMETHODCALLTYPE GetTypeLibOfClsid( 
            /* [in] */ CLSID clsid,
            /* [out] */ ITypeLib **pptlib) = 0;
        
        virtual HRESULT STDMETHODCALLTYPE GetClassObjectOfClsid( 
            /* [in] */ REFCLSID clsid,
            /* [in] */ DWORD dwFlags,
            /* [in] */ LPVOID lpReserved,
            /* [in] */ REFIID riid,
            /* [in] */ void **ppvClassObject) = 0;
        
    };
    
#else 	/* C style interface */

    typedef struct ILocalRegistryVtbl
    {
        BEGIN_INTERFACE
        
        HRESULT ( STDMETHODCALLTYPE *QueryInterface )( 
            ILocalRegistry * This,
            /* [in] */ REFIID riid,
            /* [iid_is][out] */ 
            __RPC__deref_out  void **ppvObject);
        
        ULONG ( STDMETHODCALLTYPE *AddRef )( 
            ILocalRegistry * This);
        
        ULONG ( STDMETHODCALLTYPE *Release )( 
            ILocalRegistry * This);
        
        HRESULT ( STDMETHODCALLTYPE *CreateInstance )( 
            ILocalRegistry * This,
            /* [in] */ CLSID clsid,
            /* [in] */ IUnknown *punkOuter,
            /* [in] */ REFIID riid,
            /* [in] */ DWORD dwFlags,
            /* [out] */ void **ppvObj);
        
        HRESULT ( STDMETHODCALLTYPE *GetTypeLibOfClsid )( 
            ILocalRegistry * This,
            /* [in] */ CLSID clsid,
            /* [out] */ ITypeLib **pptlib);
        
        HRESULT ( STDMETHODCALLTYPE *GetClassObjectOfClsid )( 
            ILocalRegistry * This,
            /* [in] */ REFCLSID clsid,
            /* [in] */ DWORD dwFlags,
            /* [in] */ LPVOID lpReserved,
            /* [in] */ REFIID riid,
            /* [in] */ void **ppvClassObject);
        
        END_INTERFACE
    } ILocalRegistryVtbl;

    interface ILocalRegistry
    {
        CONST_VTBL struct ILocalRegistryVtbl *lpVtbl;
    };

    

#ifdef COBJMACROS


#define ILocalRegistry_QueryInterface(This,riid,ppvObject)	\
    ( (This)->lpVtbl -> QueryInterface(This,riid,ppvObject) ) 

#define ILocalRegistry_AddRef(This)	\
    ( (This)->lpVtbl -> AddRef(This) ) 

#define ILocalRegistry_Release(This)	\
    ( (This)->lpVtbl -> Release(This) ) 


#define ILocalRegistry_CreateInstance(This,clsid,punkOuter,riid,dwFlags,ppvObj)	\
    ( (This)->lpVtbl -> CreateInstance(This,clsid,punkOuter,riid,dwFlags,ppvObj) ) 

#define ILocalRegistry_GetTypeLibOfClsid(This,clsid,pptlib)	\
    ( (This)->lpVtbl -> GetTypeLibOfClsid(This,clsid,pptlib) ) 

#define ILocalRegistry_GetClassObjectOfClsid(This,clsid,dwFlags,lpReserved,riid,ppvClassObject)	\
    ( (This)->lpVtbl -> GetClassObjectOfClsid(This,clsid,dwFlags,lpReserved,riid,ppvClassObject) ) 

#endif /* COBJMACROS */


#endif 	/* C style interface */




#endif 	/* __ILocalRegistry_INTERFACE_DEFINED__ */


#ifndef __ILocalRegistry2_INTERFACE_DEFINED__
#define __ILocalRegistry2_INTERFACE_DEFINED__

/* interface ILocalRegistry2 */
/* [unique][uuid][local][object] */ 


EXTERN_C const IID IID_ILocalRegistry2;

#if defined(__cplusplus) && !defined(CINTERFACE)
    
    MIDL_INTERFACE("77BB19B0-0462-11d1-AAF6-00A0C9055A90")
    ILocalRegistry2 : public ILocalRegistry
    {
    public:
        virtual HRESULT STDMETHODCALLTYPE GetLocalRegistryRoot( 
            /* [out] */ BSTR *pbstrRoot) = 0;
        
    };
    
#else 	/* C style interface */

    typedef struct ILocalRegistry2Vtbl
    {
        BEGIN_INTERFACE
        
        HRESULT ( STDMETHODCALLTYPE *QueryInterface )( 
            ILocalRegistry2 * This,
            /* [in] */ REFIID riid,
            /* [iid_is][out] */ 
            __RPC__deref_out  void **ppvObject);
        
        ULONG ( STDMETHODCALLTYPE *AddRef )( 
            ILocalRegistry2 * This);
        
        ULONG ( STDMETHODCALLTYPE *Release )( 
            ILocalRegistry2 * This);
        
        HRESULT ( STDMETHODCALLTYPE *CreateInstance )( 
            ILocalRegistry2 * This,
            /* [in] */ CLSID clsid,
            /* [in] */ IUnknown *punkOuter,
            /* [in] */ REFIID riid,
            /* [in] */ DWORD dwFlags,
            /* [out] */ void **ppvObj);
        
        HRESULT ( STDMETHODCALLTYPE *GetTypeLibOfClsid )( 
            ILocalRegistry2 * This,
            /* [in] */ CLSID clsid,
            /* [out] */ ITypeLib **pptlib);
        
        HRESULT ( STDMETHODCALLTYPE *GetClassObjectOfClsid )( 
            ILocalRegistry2 * This,
            /* [in] */ REFCLSID clsid,
            /* [in] */ DWORD dwFlags,
            /* [in] */ LPVOID lpReserved,
            /* [in] */ REFIID riid,
            /* [in] */ void **ppvClassObject);
        
        HRESULT ( STDMETHODCALLTYPE *GetLocalRegistryRoot )( 
            ILocalRegistry2 * This,
            /* [out] */ BSTR *pbstrRoot);
        
        END_INTERFACE
    } ILocalRegistry2Vtbl;

    interface ILocalRegistry2
    {
        CONST_VTBL struct ILocalRegistry2Vtbl *lpVtbl;
    };

    

#ifdef COBJMACROS


#define ILocalRegistry2_QueryInterface(This,riid,ppvObject)	\
    ( (This)->lpVtbl -> QueryInterface(This,riid,ppvObject) ) 

#define ILocalRegistry2_AddRef(This)	\
    ( (This)->lpVtbl -> AddRef(This) ) 

#define ILocalRegistry2_Release(This)	\
    ( (This)->lpVtbl -> Release(This) ) 


#define ILocalRegistry2_CreateInstance(This,clsid,punkOuter,riid,dwFlags,ppvObj)	\
    ( (This)->lpVtbl -> CreateInstance(This,clsid,punkOuter,riid,dwFlags,ppvObj) ) 

#define ILocalRegistry2_GetTypeLibOfClsid(This,clsid,pptlib)	\
    ( (This)->lpVtbl -> GetTypeLibOfClsid(This,clsid,pptlib) ) 

#define ILocalRegistry2_GetClassObjectOfClsid(This,clsid,dwFlags,lpReserved,riid,ppvClassObject)	\
    ( (This)->lpVtbl -> GetClassObjectOfClsid(This,clsid,dwFlags,lpReserved,riid,ppvClassObject) ) 


#define ILocalRegistry2_GetLocalRegistryRoot(This,pbstrRoot)	\
    ( (This)->lpVtbl -> GetLocalRegistryRoot(This,pbstrRoot) ) 

#endif /* COBJMACROS */


#endif 	/* C style interface */




#endif 	/* __ILocalRegistry2_INTERFACE_DEFINED__ */


#ifndef __ILocalRegistry3_INTERFACE_DEFINED__
#define __ILocalRegistry3_INTERFACE_DEFINED__

/* interface ILocalRegistry3 */
/* [unique][uuid][local][object] */ 


EXTERN_C const IID IID_ILocalRegistry3;

#if defined(__cplusplus) && !defined(CINTERFACE)
    
    MIDL_INTERFACE("1B01F13F-ABEE-4761-91AF-76CE6B4C9E7A")
    ILocalRegistry3 : public ILocalRegistry2
    {
    public:
        virtual HRESULT STDMETHODCALLTYPE CreateManagedInstance( 
            /* [in] */ LPCWSTR codeBase,
            /* [in] */ LPCWSTR assemblyName,
            /* [in] */ LPCWSTR typeName,
            /* [in] */ REFIID riid,
            /* [out] */ void **ppvObj) = 0;
        
        virtual HRESULT STDMETHODCALLTYPE GetClassObjectOfManagedClass( 
            /* [in] */ LPCWSTR codeBase,
            /* [in] */ LPCWSTR assemblyName,
            /* [in] */ LPCWSTR typeName,
            /* [in] */ REFIID riid,
            /* [out] */ void **ppvClassObject) = 0;
        
    };
    
#else 	/* C style interface */

    typedef struct ILocalRegistry3Vtbl
    {
        BEGIN_INTERFACE
        
        HRESULT ( STDMETHODCALLTYPE *QueryInterface )( 
            ILocalRegistry3 * This,
            /* [in] */ REFIID riid,
            /* [iid_is][out] */ 
            __RPC__deref_out  void **ppvObject);
        
        ULONG ( STDMETHODCALLTYPE *AddRef )( 
            ILocalRegistry3 * This);
        
        ULONG ( STDMETHODCALLTYPE *Release )( 
            ILocalRegistry3 * This);
        
        HRESULT ( STDMETHODCALLTYPE *CreateInstance )( 
            ILocalRegistry3 * This,
            /* [in] */ CLSID clsid,
            /* [in] */ IUnknown *punkOuter,
            /* [in] */ REFIID riid,
            /* [in] */ DWORD dwFlags,
            /* [out] */ void **ppvObj);
        
        HRESULT ( STDMETHODCALLTYPE *GetTypeLibOfClsid )( 
            ILocalRegistry3 * This,
            /* [in] */ CLSID clsid,
            /* [out] */ ITypeLib **pptlib);
        
        HRESULT ( STDMETHODCALLTYPE *GetClassObjectOfClsid )( 
            ILocalRegistry3 * This,
            /* [in] */ REFCLSID clsid,
            /* [in] */ DWORD dwFlags,
            /* [in] */ LPVOID lpReserved,
            /* [in] */ REFIID riid,
            /* [in] */ void **ppvClassObject);
        
        HRESULT ( STDMETHODCALLTYPE *GetLocalRegistryRoot )( 
            ILocalRegistry3 * This,
            /* [out] */ BSTR *pbstrRoot);
        
        HRESULT ( STDMETHODCALLTYPE *CreateManagedInstance )( 
            ILocalRegistry3 * This,
            /* [in] */ LPCWSTR codeBase,
            /* [in] */ LPCWSTR assemblyName,
            /* [in] */ LPCWSTR typeName,
            /* [in] */ REFIID riid,
            /* [out] */ void **ppvObj);
        
        HRESULT ( STDMETHODCALLTYPE *GetClassObjectOfManagedClass )( 
            ILocalRegistry3 * This,
            /* [in] */ LPCWSTR codeBase,
            /* [in] */ LPCWSTR assemblyName,
            /* [in] */ LPCWSTR typeName,
            /* [in] */ REFIID riid,
            /* [out] */ void **ppvClassObject);
        
        END_INTERFACE
    } ILocalRegistry3Vtbl;

    interface ILocalRegistry3
    {
        CONST_VTBL struct ILocalRegistry3Vtbl *lpVtbl;
    };

    

#ifdef COBJMACROS


#define ILocalRegistry3_QueryInterface(This,riid,ppvObject)	\
    ( (This)->lpVtbl -> QueryInterface(This,riid,ppvObject) ) 

#define ILocalRegistry3_AddRef(This)	\
    ( (This)->lpVtbl -> AddRef(This) ) 

#define ILocalRegistry3_Release(This)	\
    ( (This)->lpVtbl -> Release(This) ) 


#define ILocalRegistry3_CreateInstance(This,clsid,punkOuter,riid,dwFlags,ppvObj)	\
    ( (This)->lpVtbl -> CreateInstance(This,clsid,punkOuter,riid,dwFlags,ppvObj) ) 

#define ILocalRegistry3_GetTypeLibOfClsid(This,clsid,pptlib)	\
    ( (This)->lpVtbl -> GetTypeLibOfClsid(This,clsid,pptlib) ) 

#define ILocalRegistry3_GetClassObjectOfClsid(This,clsid,dwFlags,lpReserved,riid,ppvClassObject)	\
    ( (This)->lpVtbl -> GetClassObjectOfClsid(This,clsid,dwFlags,lpReserved,riid,ppvClassObject) ) 


#define ILocalRegistry3_GetLocalRegistryRoot(This,pbstrRoot)	\
    ( (This)->lpVtbl -> GetLocalRegistryRoot(This,pbstrRoot) ) 


#define ILocalRegistry3_CreateManagedInstance(This,codeBase,assemblyName,typeName,riid,ppvObj)	\
    ( (This)->lpVtbl -> CreateManagedInstance(This,codeBase,assemblyName,typeName,riid,ppvObj) ) 

#define ILocalRegistry3_GetClassObjectOfManagedClass(This,codeBase,assemblyName,typeName,riid,ppvClassObject)	\
    ( (This)->lpVtbl -> GetClassObjectOfManagedClass(This,codeBase,assemblyName,typeName,riid,ppvClassObject) ) 

#endif /* COBJMACROS */


#endif 	/* C style interface */




#endif 	/* __ILocalRegistry3_INTERFACE_DEFINED__ */


/* interface __MIDL_itf_objext_0000_0008 */
/* [local] */ 

#define SID_SLocalRegistry IID_ILocalRegistry


extern RPC_IF_HANDLE __MIDL_itf_objext_0000_0008_v0_0_c_ifspec;
extern RPC_IF_HANDLE __MIDL_itf_objext_0000_0008_v0_0_s_ifspec;

#ifndef __IUIElement_INTERFACE_DEFINED__
#define __IUIElement_INTERFACE_DEFINED__

/* interface IUIElement */
/* [unique][uuid][local][object] */ 


EXTERN_C const IID IID_IUIElement;

#if defined(__cplusplus) && !defined(CINTERFACE)
    
    MIDL_INTERFACE("759d0500-d979-11ce-84ec-00aa00614f3e")
    IUIElement : public IUnknown
    {
    public:
        virtual HRESULT STDMETHODCALLTYPE Show( void) = 0;
        
        virtual HRESULT STDMETHODCALLTYPE Hide( void) = 0;
        
        virtual HRESULT STDMETHODCALLTYPE IsVisible( void) = 0;
        
    };
    
#else 	/* C style interface */

    typedef struct IUIElementVtbl
    {
        BEGIN_INTERFACE
        
        HRESULT ( STDMETHODCALLTYPE *QueryInterface )( 
            IUIElement * This,
            /* [in] */ REFIID riid,
            /* [iid_is][out] */ 
            __RPC__deref_out  void **ppvObject);
        
        ULONG ( STDMETHODCALLTYPE *AddRef )( 
            IUIElement * This);
        
        ULONG ( STDMETHODCALLTYPE *Release )( 
            IUIElement * This);
        
        HRESULT ( STDMETHODCALLTYPE *Show )( 
            IUIElement * This);
        
        HRESULT ( STDMETHODCALLTYPE *Hide )( 
            IUIElement * This);
        
        HRESULT ( STDMETHODCALLTYPE *IsVisible )( 
            IUIElement * This);
        
        END_INTERFACE
    } IUIElementVtbl;

    interface IUIElement
    {
        CONST_VTBL struct IUIElementVtbl *lpVtbl;
    };

    

#ifdef COBJMACROS


#define IUIElement_QueryInterface(This,riid,ppvObject)	\
    ( (This)->lpVtbl -> QueryInterface(This,riid,ppvObject) ) 

#define IUIElement_AddRef(This)	\
    ( (This)->lpVtbl -> AddRef(This) ) 

#define IUIElement_Release(This)	\
    ( (This)->lpVtbl -> Release(This) ) 


#define IUIElement_Show(This)	\
    ( (This)->lpVtbl -> Show(This) ) 

#define IUIElement_Hide(This)	\
    ( (This)->lpVtbl -> Hide(This) ) 

#define IUIElement_IsVisible(This)	\
    ( (This)->lpVtbl -> IsVisible(This) ) 

#endif /* COBJMACROS */


#endif 	/* C style interface */




#endif 	/* __IUIElement_INTERFACE_DEFINED__ */


/* interface __MIDL_itf_objext_0000_0009 */
/* [local] */ 

typedef int PROPCAT;



extern RPC_IF_HANDLE __MIDL_itf_objext_0000_0009_v0_0_c_ifspec;
extern RPC_IF_HANDLE __MIDL_itf_objext_0000_0009_v0_0_s_ifspec;

#ifndef __ICategorizeProperties_INTERFACE_DEFINED__
#define __ICategorizeProperties_INTERFACE_DEFINED__

/* interface ICategorizeProperties */
/* [unique][uuid][local][object] */ 


EXTERN_C const IID IID_ICategorizeProperties;

#if defined(__cplusplus) && !defined(CINTERFACE)
    
    MIDL_INTERFACE("4D07FC10-F931-11ce-B001-00AA006884E5")
    ICategorizeProperties : public IUnknown
    {
    public:
        virtual HRESULT STDMETHODCALLTYPE MapPropertyToCategory( 
            /* [in] */ DISPID dispid,
            /* [out] */ PROPCAT *ppropcat) = 0;
        
        virtual HRESULT STDMETHODCALLTYPE GetCategoryName( 
            /* [in] */ PROPCAT propcat,
            /* [in] */ LCID lcid,
            /* [out] */ BSTR *pbstrName) = 0;
        
    };
    
#else 	/* C style interface */

    typedef struct ICategorizePropertiesVtbl
    {
        BEGIN_INTERFACE
        
        HRESULT ( STDMETHODCALLTYPE *QueryInterface )( 
            ICategorizeProperties * This,
            /* [in] */ REFIID riid,
            /* [iid_is][out] */ 
            __RPC__deref_out  void **ppvObject);
        
        ULONG ( STDMETHODCALLTYPE *AddRef )( 
            ICategorizeProperties * This);
        
        ULONG ( STDMETHODCALLTYPE *Release )( 
            ICategorizeProperties * This);
        
        HRESULT ( STDMETHODCALLTYPE *MapPropertyToCategory )( 
            ICategorizeProperties * This,
            /* [in] */ DISPID dispid,
            /* [out] */ PROPCAT *ppropcat);
        
        HRESULT ( STDMETHODCALLTYPE *GetCategoryName )( 
            ICategorizeProperties * This,
            /* [in] */ PROPCAT propcat,
            /* [in] */ LCID lcid,
            /* [out] */ BSTR *pbstrName);
        
        END_INTERFACE
    } ICategorizePropertiesVtbl;

    interface ICategorizeProperties
    {
        CONST_VTBL struct ICategorizePropertiesVtbl *lpVtbl;
    };

    

#ifdef COBJMACROS


#define ICategorizeProperties_QueryInterface(This,riid,ppvObject)	\
    ( (This)->lpVtbl -> QueryInterface(This,riid,ppvObject) ) 

#define ICategorizeProperties_AddRef(This)	\
    ( (This)->lpVtbl -> AddRef(This) ) 

#define ICategorizeProperties_Release(This)	\
    ( (This)->lpVtbl -> Release(This) ) 


#define ICategorizeProperties_MapPropertyToCategory(This,dispid,ppropcat)	\
    ( (This)->lpVtbl -> MapPropertyToCategory(This,dispid,ppropcat) ) 

#define ICategorizeProperties_GetCategoryName(This,propcat,lcid,pbstrName)	\
    ( (This)->lpVtbl -> GetCategoryName(This,propcat,lcid,pbstrName) ) 

#endif /* COBJMACROS */


#endif 	/* C style interface */




#endif 	/* __ICategorizeProperties_INTERFACE_DEFINED__ */


/* interface __MIDL_itf_objext_0000_0010 */
/* [local] */ 

typedef ICategorizeProperties *LPCATEGORIZEPROPERTIES;

#define PROPCAT_Nil -1
#define PROPCAT_Misc -2
#define PROPCAT_Font -3
#define PROPCAT_Position -4
#define PROPCAT_Appearance -5
#define PROPCAT_Behavior -6
#define PROPCAT_Data -7
#define PROPCAT_List -8
#define PROPCAT_Text -9
#define PROPCAT_Scale -10
#define PROPCAT_DDE -11
#define HELPINFO_WHATS_THIS_MODE_ON     1


extern RPC_IF_HANDLE __MIDL_itf_objext_0000_0010_v0_0_c_ifspec;
extern RPC_IF_HANDLE __MIDL_itf_objext_0000_0010_v0_0_s_ifspec;

#ifndef __IHelp_INTERFACE_DEFINED__
#define __IHelp_INTERFACE_DEFINED__

/* interface IHelp */
/* [unique][uuid][local][object] */ 


EXTERN_C const IID IID_IHelp;

#if defined(__cplusplus) && !defined(CINTERFACE)
    
    MIDL_INTERFACE("6d5140c8-7436-11ce-8034-00aa006009fa")
    IHelp : public IUnknown
    {
    public:
        virtual HRESULT STDMETHODCALLTYPE GetHelpFile( 
            /* [out] */ BSTR *pbstr) = 0;
        
        virtual HRESULT STDMETHODCALLTYPE GetHelpInfo( 
            /* [out] */ DWORD *pdwHelpInfo) = 0;
        
        virtual HRESULT STDMETHODCALLTYPE ShowHelp( 
            /* [in] */ LPOLESTR szHelp,
            /* [in] */ UINT fuCommand,
            /* [in] */ DWORD dwHelpContext) = 0;
        
    };
    
#else 	/* C style interface */

    typedef struct IHelpVtbl
    {
        BEGIN_INTERFACE
        
        HRESULT ( STDMETHODCALLTYPE *QueryInterface )( 
            IHelp * This,
            /* [in] */ REFIID riid,
            /* [iid_is][out] */ 
            __RPC__deref_out  void **ppvObject);
        
        ULONG ( STDMETHODCALLTYPE *AddRef )( 
            IHelp * This);
        
        ULONG ( STDMETHODCALLTYPE *Release )( 
            IHelp * This);
        
        HRESULT ( STDMETHODCALLTYPE *GetHelpFile )( 
            IHelp * This,
            /* [out] */ BSTR *pbstr);
        
        HRESULT ( STDMETHODCALLTYPE *GetHelpInfo )( 
            IHelp * This,
            /* [out] */ DWORD *pdwHelpInfo);
        
        HRESULT ( STDMETHODCALLTYPE *ShowHelp )( 
            IHelp * This,
            /* [in] */ LPOLESTR szHelp,
            /* [in] */ UINT fuCommand,
            /* [in] */ DWORD dwHelpContext);
        
        END_INTERFACE
    } IHelpVtbl;

    interface IHelp
    {
        CONST_VTBL struct IHelpVtbl *lpVtbl;
    };

    

#ifdef COBJMACROS


#define IHelp_QueryInterface(This,riid,ppvObject)	\
    ( (This)->lpVtbl -> QueryInterface(This,riid,ppvObject) ) 

#define IHelp_AddRef(This)	\
    ( (This)->lpVtbl -> AddRef(This) ) 

#define IHelp_Release(This)	\
    ( (This)->lpVtbl -> Release(This) ) 


#define IHelp_GetHelpFile(This,pbstr)	\
    ( (This)->lpVtbl -> GetHelpFile(This,pbstr) ) 

#define IHelp_GetHelpInfo(This,pdwHelpInfo)	\
    ( (This)->lpVtbl -> GetHelpInfo(This,pdwHelpInfo) ) 

#define IHelp_ShowHelp(This,szHelp,fuCommand,dwHelpContext)	\
    ( (This)->lpVtbl -> ShowHelp(This,szHelp,fuCommand,dwHelpContext) ) 

#endif /* COBJMACROS */


#endif 	/* C style interface */




#endif 	/* __IHelp_INTERFACE_DEFINED__ */


/* interface __MIDL_itf_objext_0000_0011 */
/* [local] */ 

EXTERN_C const IID SID_SHelp;
#endif


extern RPC_IF_HANDLE __MIDL_itf_objext_0000_0011_v0_0_c_ifspec;
extern RPC_IF_HANDLE __MIDL_itf_objext_0000_0011_v0_0_s_ifspec;

/* Additional Prototypes for ALL interfaces */

/* end of Additional Prototypes */

#ifdef __cplusplus
}
#endif

#endif


