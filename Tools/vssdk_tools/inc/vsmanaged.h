

/* this ALWAYS GENERATED file contains the definitions for the interfaces */


 /* File created by MIDL compiler version 7.00.0499 */
/* Compiler settings for vsmanaged.idl:
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


#ifndef __vsmanaged_h__
#define __vsmanaged_h__

#if defined(_MSC_VER) && (_MSC_VER >= 1020)
#pragma once
#endif

/* Forward Declarations */ 

#ifndef __IVSMDCodeDomCreator_FWD_DEFINED__
#define __IVSMDCodeDomCreator_FWD_DEFINED__
typedef interface IVSMDCodeDomCreator IVSMDCodeDomCreator;
#endif 	/* __IVSMDCodeDomCreator_FWD_DEFINED__ */


#ifndef __IVSMDCodeDomProvider_FWD_DEFINED__
#define __IVSMDCodeDomProvider_FWD_DEFINED__
typedef interface IVSMDCodeDomProvider IVSMDCodeDomProvider;
#endif 	/* __IVSMDCodeDomProvider_FWD_DEFINED__ */


#ifndef __IVSMDDesigner_FWD_DEFINED__
#define __IVSMDDesigner_FWD_DEFINED__
typedef interface IVSMDDesigner IVSMDDesigner;
#endif 	/* __IVSMDDesigner_FWD_DEFINED__ */


#ifndef __IVSMDDesignerLoader_FWD_DEFINED__
#define __IVSMDDesignerLoader_FWD_DEFINED__
typedef interface IVSMDDesignerLoader IVSMDDesignerLoader;
#endif 	/* __IVSMDDesignerLoader_FWD_DEFINED__ */


#ifndef __IVSMDDesignerService_FWD_DEFINED__
#define __IVSMDDesignerService_FWD_DEFINED__
typedef interface IVSMDDesignerService IVSMDDesignerService;
#endif 	/* __IVSMDDesignerService_FWD_DEFINED__ */


#ifndef __IVSMDPropertyGrid_FWD_DEFINED__
#define __IVSMDPropertyGrid_FWD_DEFINED__
typedef interface IVSMDPropertyGrid IVSMDPropertyGrid;
#endif 	/* __IVSMDPropertyGrid_FWD_DEFINED__ */


#ifndef __IVSMDPropertyBrowser_FWD_DEFINED__
#define __IVSMDPropertyBrowser_FWD_DEFINED__
typedef interface IVSMDPropertyBrowser IVSMDPropertyBrowser;
#endif 	/* __IVSMDPropertyBrowser_FWD_DEFINED__ */


#ifndef __IVSMDPerPropertyBrowsing_FWD_DEFINED__
#define __IVSMDPerPropertyBrowsing_FWD_DEFINED__
typedef interface IVSMDPerPropertyBrowsing IVSMDPerPropertyBrowsing;
#endif 	/* __IVSMDPerPropertyBrowsing_FWD_DEFINED__ */


/* header files for imported files */
#include "servprov.h"
#include "vsshell.h"

#ifdef __cplusplus
extern "C"{
#endif 


/* interface __MIDL_itf_vsmanaged_0000_0000 */
/* [local] */ 



enum __VSHPROPID_ASPX
    {	VSHPROPID_ProjAspxLanguage	= 5000,
	VSHPROPID_ProjAspxCodeBehindExt	= 5001,
	VSHPROPID_ProjAspxAutoEventWireup	= 5002
    } ;


extern RPC_IF_HANDLE __MIDL_itf_vsmanaged_0000_0000_v0_0_c_ifspec;
extern RPC_IF_HANDLE __MIDL_itf_vsmanaged_0000_0000_v0_0_s_ifspec;


#ifndef __VSManagedDesigner_LIBRARY_DEFINED__
#define __VSManagedDesigner_LIBRARY_DEFINED__

/* library VSManagedDesigner */
/* [helpstring][custom][version][uuid] */ 

#define SID_SVSMDCodeDomProvider IID_IVSMDCodeDomProvider
#define SID_SVSMDDesignerService IID_IVSMDDesignerService
typedef 
enum _PROPERTYGRIDSORT
    {	PGSORT_NOSORT	= 0,
	PGSORT_ALPHABETICAL	= 1,
	PGSORT_CATEGORIZED	= 2
    } 	PROPERTYGRIDSORT;

typedef 
enum _PROPERTYGRIDOPTION
    {	PGOPT_HOTCOMMANDS	= 0,
	PGOPT_HELP	= 1,
	PGOPT_TOOLBAR	= 2
    } 	PROPERTYGRIDOPTION;

#define SID_SVSMDPropertyBrowser IID_IVSMDPropertyBrowser

EXTERN_C const IID LIBID_VSManagedDesigner;

#ifndef __IVSMDCodeDomCreator_INTERFACE_DEFINED__
#define __IVSMDCodeDomCreator_INTERFACE_DEFINED__

/* interface IVSMDCodeDomCreator */
/* [uuid][object] */ 


EXTERN_C const IID IID_IVSMDCodeDomCreator;

#if defined(__cplusplus) && !defined(CINTERFACE)
    
    MIDL_INTERFACE("4CC03BF7-4D89-4198-8E4D-17E217CA07B2")
    IVSMDCodeDomCreator : public IUnknown
    {
    public:
        virtual HRESULT STDMETHODCALLTYPE CreateCodeDomProvider( 
            /* [in] */ __RPC__in_opt IVsHierarchy *pHier,
            /* [in] */ VSITEMID itemid,
            /* [retval][out] */ __RPC__deref_out_opt IVSMDCodeDomProvider **ppProvider) = 0;
        
    };
    
#else 	/* C style interface */

    typedef struct IVSMDCodeDomCreatorVtbl
    {
        BEGIN_INTERFACE
        
        HRESULT ( STDMETHODCALLTYPE *QueryInterface )( 
            IVSMDCodeDomCreator * This,
            /* [in] */ __RPC__in REFIID riid,
            /* [iid_is][out] */ 
            __RPC__deref_out  void **ppvObject);
        
        ULONG ( STDMETHODCALLTYPE *AddRef )( 
            IVSMDCodeDomCreator * This);
        
        ULONG ( STDMETHODCALLTYPE *Release )( 
            IVSMDCodeDomCreator * This);
        
        HRESULT ( STDMETHODCALLTYPE *CreateCodeDomProvider )( 
            IVSMDCodeDomCreator * This,
            /* [in] */ __RPC__in_opt IVsHierarchy *pHier,
            /* [in] */ VSITEMID itemid,
            /* [retval][out] */ __RPC__deref_out_opt IVSMDCodeDomProvider **ppProvider);
        
        END_INTERFACE
    } IVSMDCodeDomCreatorVtbl;

    interface IVSMDCodeDomCreator
    {
        CONST_VTBL struct IVSMDCodeDomCreatorVtbl *lpVtbl;
    };

    

#ifdef COBJMACROS


#define IVSMDCodeDomCreator_QueryInterface(This,riid,ppvObject)	\
    ( (This)->lpVtbl -> QueryInterface(This,riid,ppvObject) ) 

#define IVSMDCodeDomCreator_AddRef(This)	\
    ( (This)->lpVtbl -> AddRef(This) ) 

#define IVSMDCodeDomCreator_Release(This)	\
    ( (This)->lpVtbl -> Release(This) ) 


#define IVSMDCodeDomCreator_CreateCodeDomProvider(This,pHier,itemid,ppProvider)	\
    ( (This)->lpVtbl -> CreateCodeDomProvider(This,pHier,itemid,ppProvider) ) 

#endif /* COBJMACROS */


#endif 	/* C style interface */




#endif 	/* __IVSMDCodeDomCreator_INTERFACE_DEFINED__ */


#ifndef __IVSMDCodeDomProvider_INTERFACE_DEFINED__
#define __IVSMDCodeDomProvider_INTERFACE_DEFINED__

/* interface IVSMDCodeDomProvider */
/* [uuid][object] */ 


EXTERN_C const IID IID_IVSMDCodeDomProvider;

#if defined(__cplusplus) && !defined(CINTERFACE)
    
    MIDL_INTERFACE("73E59688-C7C4-4a85-AF64-A538754784C5")
    IVSMDCodeDomProvider : public IUnknown
    {
    public:
        virtual /* [propget] */ HRESULT STDMETHODCALLTYPE get_CodeDomProvider( 
            /* [retval][out] */ __RPC__deref_out_opt IDispatch **ppProvider) = 0;
        
    };
    
#else 	/* C style interface */

    typedef struct IVSMDCodeDomProviderVtbl
    {
        BEGIN_INTERFACE
        
        HRESULT ( STDMETHODCALLTYPE *QueryInterface )( 
            IVSMDCodeDomProvider * This,
            /* [in] */ __RPC__in REFIID riid,
            /* [iid_is][out] */ 
            __RPC__deref_out  void **ppvObject);
        
        ULONG ( STDMETHODCALLTYPE *AddRef )( 
            IVSMDCodeDomProvider * This);
        
        ULONG ( STDMETHODCALLTYPE *Release )( 
            IVSMDCodeDomProvider * This);
        
        /* [propget] */ HRESULT ( STDMETHODCALLTYPE *get_CodeDomProvider )( 
            IVSMDCodeDomProvider * This,
            /* [retval][out] */ __RPC__deref_out_opt IDispatch **ppProvider);
        
        END_INTERFACE
    } IVSMDCodeDomProviderVtbl;

    interface IVSMDCodeDomProvider
    {
        CONST_VTBL struct IVSMDCodeDomProviderVtbl *lpVtbl;
    };

    

#ifdef COBJMACROS


#define IVSMDCodeDomProvider_QueryInterface(This,riid,ppvObject)	\
    ( (This)->lpVtbl -> QueryInterface(This,riid,ppvObject) ) 

#define IVSMDCodeDomProvider_AddRef(This)	\
    ( (This)->lpVtbl -> AddRef(This) ) 

#define IVSMDCodeDomProvider_Release(This)	\
    ( (This)->lpVtbl -> Release(This) ) 


#define IVSMDCodeDomProvider_get_CodeDomProvider(This,ppProvider)	\
    ( (This)->lpVtbl -> get_CodeDomProvider(This,ppProvider) ) 

#endif /* COBJMACROS */


#endif 	/* C style interface */




#endif 	/* __IVSMDCodeDomProvider_INTERFACE_DEFINED__ */


#ifndef __IVSMDDesigner_INTERFACE_DEFINED__
#define __IVSMDDesigner_INTERFACE_DEFINED__

/* interface IVSMDDesigner */
/* [uuid][object] */ 


EXTERN_C const IID IID_IVSMDDesigner;

#if defined(__cplusplus) && !defined(CINTERFACE)
    
    MIDL_INTERFACE("7494682A-37A0-11d2-A273-00C04F8EF4FF")
    IVSMDDesigner : public IUnknown
    {
    public:
        virtual /* [propget] */ HRESULT STDMETHODCALLTYPE get_CommandGuid( 
            /* [retval][out] */ __RPC__out GUID *pguidCmdId) = 0;
        
        virtual /* [propget] */ HRESULT STDMETHODCALLTYPE get_View( 
            /* [retval][out] */ __RPC__deref_out_opt IUnknown **pView) = 0;
        
        virtual /* [propget] */ HRESULT STDMETHODCALLTYPE get_SelectionContainer( 
            /* [retval][out] */ __RPC__deref_out_opt IUnknown **ppSelCon) = 0;
        
        virtual HRESULT STDMETHODCALLTYPE Dispose( void) = 0;
        
        virtual HRESULT STDMETHODCALLTYPE Flush( void) = 0;
        
        virtual HRESULT STDMETHODCALLTYPE GetLoadError( void) = 0;
        
    };
    
#else 	/* C style interface */

    typedef struct IVSMDDesignerVtbl
    {
        BEGIN_INTERFACE
        
        HRESULT ( STDMETHODCALLTYPE *QueryInterface )( 
            IVSMDDesigner * This,
            /* [in] */ __RPC__in REFIID riid,
            /* [iid_is][out] */ 
            __RPC__deref_out  void **ppvObject);
        
        ULONG ( STDMETHODCALLTYPE *AddRef )( 
            IVSMDDesigner * This);
        
        ULONG ( STDMETHODCALLTYPE *Release )( 
            IVSMDDesigner * This);
        
        /* [propget] */ HRESULT ( STDMETHODCALLTYPE *get_CommandGuid )( 
            IVSMDDesigner * This,
            /* [retval][out] */ __RPC__out GUID *pguidCmdId);
        
        /* [propget] */ HRESULT ( STDMETHODCALLTYPE *get_View )( 
            IVSMDDesigner * This,
            /* [retval][out] */ __RPC__deref_out_opt IUnknown **pView);
        
        /* [propget] */ HRESULT ( STDMETHODCALLTYPE *get_SelectionContainer )( 
            IVSMDDesigner * This,
            /* [retval][out] */ __RPC__deref_out_opt IUnknown **ppSelCon);
        
        HRESULT ( STDMETHODCALLTYPE *Dispose )( 
            IVSMDDesigner * This);
        
        HRESULT ( STDMETHODCALLTYPE *Flush )( 
            IVSMDDesigner * This);
        
        HRESULT ( STDMETHODCALLTYPE *GetLoadError )( 
            IVSMDDesigner * This);
        
        END_INTERFACE
    } IVSMDDesignerVtbl;

    interface IVSMDDesigner
    {
        CONST_VTBL struct IVSMDDesignerVtbl *lpVtbl;
    };

    

#ifdef COBJMACROS


#define IVSMDDesigner_QueryInterface(This,riid,ppvObject)	\
    ( (This)->lpVtbl -> QueryInterface(This,riid,ppvObject) ) 

#define IVSMDDesigner_AddRef(This)	\
    ( (This)->lpVtbl -> AddRef(This) ) 

#define IVSMDDesigner_Release(This)	\
    ( (This)->lpVtbl -> Release(This) ) 


#define IVSMDDesigner_get_CommandGuid(This,pguidCmdId)	\
    ( (This)->lpVtbl -> get_CommandGuid(This,pguidCmdId) ) 

#define IVSMDDesigner_get_View(This,pView)	\
    ( (This)->lpVtbl -> get_View(This,pView) ) 

#define IVSMDDesigner_get_SelectionContainer(This,ppSelCon)	\
    ( (This)->lpVtbl -> get_SelectionContainer(This,ppSelCon) ) 

#define IVSMDDesigner_Dispose(This)	\
    ( (This)->lpVtbl -> Dispose(This) ) 

#define IVSMDDesigner_Flush(This)	\
    ( (This)->lpVtbl -> Flush(This) ) 

#define IVSMDDesigner_GetLoadError(This)	\
    ( (This)->lpVtbl -> GetLoadError(This) ) 

#endif /* COBJMACROS */


#endif 	/* C style interface */




#endif 	/* __IVSMDDesigner_INTERFACE_DEFINED__ */


#ifndef __IVSMDDesignerLoader_INTERFACE_DEFINED__
#define __IVSMDDesignerLoader_INTERFACE_DEFINED__

/* interface IVSMDDesignerLoader */
/* [uuid][object] */ 


EXTERN_C const IID IID_IVSMDDesignerLoader;

#if defined(__cplusplus) && !defined(CINTERFACE)
    
    MIDL_INTERFACE("74946834-37A0-11d2-A273-00C04F8EF4FF")
    IVSMDDesignerLoader : public IUnknown
    {
    public:
        virtual HRESULT STDMETHODCALLTYPE Dispose( void) = 0;
        
        virtual HRESULT STDMETHODCALLTYPE GetEditorCaption( 
            /* [in] */ READONLYSTATUS status,
            /* [retval][out] */ __RPC__deref_out_opt BSTR *pbstrCaption) = 0;
        
        virtual HRESULT STDMETHODCALLTYPE Initialize( 
            /* [in] */ __RPC__in_opt IServiceProvider *pSp,
            /* [in] */ __RPC__in_opt IVsHierarchy *pHier,
            /* [in] */ VSITEMID itemid,
            /* [in] */ __RPC__in_opt IUnknown *pDocData) = 0;
        
        virtual HRESULT STDMETHODCALLTYPE SetBaseEditorCaption( 
            /* [in] */ __RPC__in LPCOLESTR pwszCaption) = 0;
        
    };
    
#else 	/* C style interface */

    typedef struct IVSMDDesignerLoaderVtbl
    {
        BEGIN_INTERFACE
        
        HRESULT ( STDMETHODCALLTYPE *QueryInterface )( 
            IVSMDDesignerLoader * This,
            /* [in] */ __RPC__in REFIID riid,
            /* [iid_is][out] */ 
            __RPC__deref_out  void **ppvObject);
        
        ULONG ( STDMETHODCALLTYPE *AddRef )( 
            IVSMDDesignerLoader * This);
        
        ULONG ( STDMETHODCALLTYPE *Release )( 
            IVSMDDesignerLoader * This);
        
        HRESULT ( STDMETHODCALLTYPE *Dispose )( 
            IVSMDDesignerLoader * This);
        
        HRESULT ( STDMETHODCALLTYPE *GetEditorCaption )( 
            IVSMDDesignerLoader * This,
            /* [in] */ READONLYSTATUS status,
            /* [retval][out] */ __RPC__deref_out_opt BSTR *pbstrCaption);
        
        HRESULT ( STDMETHODCALLTYPE *Initialize )( 
            IVSMDDesignerLoader * This,
            /* [in] */ __RPC__in_opt IServiceProvider *pSp,
            /* [in] */ __RPC__in_opt IVsHierarchy *pHier,
            /* [in] */ VSITEMID itemid,
            /* [in] */ __RPC__in_opt IUnknown *pDocData);
        
        HRESULT ( STDMETHODCALLTYPE *SetBaseEditorCaption )( 
            IVSMDDesignerLoader * This,
            /* [in] */ __RPC__in LPCOLESTR pwszCaption);
        
        END_INTERFACE
    } IVSMDDesignerLoaderVtbl;

    interface IVSMDDesignerLoader
    {
        CONST_VTBL struct IVSMDDesignerLoaderVtbl *lpVtbl;
    };

    

#ifdef COBJMACROS


#define IVSMDDesignerLoader_QueryInterface(This,riid,ppvObject)	\
    ( (This)->lpVtbl -> QueryInterface(This,riid,ppvObject) ) 

#define IVSMDDesignerLoader_AddRef(This)	\
    ( (This)->lpVtbl -> AddRef(This) ) 

#define IVSMDDesignerLoader_Release(This)	\
    ( (This)->lpVtbl -> Release(This) ) 


#define IVSMDDesignerLoader_Dispose(This)	\
    ( (This)->lpVtbl -> Dispose(This) ) 

#define IVSMDDesignerLoader_GetEditorCaption(This,status,pbstrCaption)	\
    ( (This)->lpVtbl -> GetEditorCaption(This,status,pbstrCaption) ) 

#define IVSMDDesignerLoader_Initialize(This,pSp,pHier,itemid,pDocData)	\
    ( (This)->lpVtbl -> Initialize(This,pSp,pHier,itemid,pDocData) ) 

#define IVSMDDesignerLoader_SetBaseEditorCaption(This,pwszCaption)	\
    ( (This)->lpVtbl -> SetBaseEditorCaption(This,pwszCaption) ) 

#endif /* COBJMACROS */


#endif 	/* C style interface */




#endif 	/* __IVSMDDesignerLoader_INTERFACE_DEFINED__ */


#ifndef __IVSMDDesignerService_INTERFACE_DEFINED__
#define __IVSMDDesignerService_INTERFACE_DEFINED__

/* interface IVSMDDesignerService */
/* [uuid][object] */ 


EXTERN_C const IID IID_IVSMDDesignerService;

#if defined(__cplusplus) && !defined(CINTERFACE)
    
    MIDL_INTERFACE("74946829-37A0-11d2-A273-00C04F8EF4FF")
    IVSMDDesignerService : public IUnknown
    {
    public:
        virtual /* [propget] */ HRESULT STDMETHODCALLTYPE get_DesignViewAttribute( 
            /* [retval][out] */ __RPC__deref_out_opt BSTR *pbstrAttribute) = 0;
        
        virtual HRESULT STDMETHODCALLTYPE CreateDesigner( 
            /* [in] */ __RPC__in_opt IServiceProvider *pSp,
            /* [in] */ __RPC__in_opt IUnknown *pDesignerLoader,
            /* [retval][out] */ __RPC__deref_out_opt IVSMDDesigner **ppDesigner) = 0;
        
        virtual HRESULT STDMETHODCALLTYPE CreateDesignerForClass( 
            /* [in] */ __RPC__in_opt IServiceProvider *pSp,
            /* [in] */ __RPC__in LPCOLESTR pwszComponentClass,
            /* [retval][out] */ __RPC__deref_out_opt IVSMDDesigner **ppDesigner) = 0;
        
        virtual HRESULT STDMETHODCALLTYPE CreateDesignerLoader( 
            /* [in] */ __RPC__in LPCOLESTR pwszCodeStreamClass,
            /* [retval][out] */ __RPC__deref_out_opt IUnknown **ppCodeStream) = 0;
        
        virtual HRESULT STDMETHODCALLTYPE GetDesignerLoaderClassForFile( 
            /* [in] */ __RPC__in LPCOLESTR pwszFileName,
            /* [retval][out] */ __RPC__deref_out_opt BSTR *pbstrDesignerLoaderClass) = 0;
        
        virtual HRESULT STDMETHODCALLTYPE RegisterDesignViewAttribute( 
            /* [in] */ __RPC__in_opt IVsHierarchy *pHier,
            /* [in] */ VSITEMID itemid,
            /* [in] */ int dwClass,
            /* [in] */ __RPC__in LPOLESTR pwszAttributeValue) = 0;
        
    };
    
#else 	/* C style interface */

    typedef struct IVSMDDesignerServiceVtbl
    {
        BEGIN_INTERFACE
        
        HRESULT ( STDMETHODCALLTYPE *QueryInterface )( 
            IVSMDDesignerService * This,
            /* [in] */ __RPC__in REFIID riid,
            /* [iid_is][out] */ 
            __RPC__deref_out  void **ppvObject);
        
        ULONG ( STDMETHODCALLTYPE *AddRef )( 
            IVSMDDesignerService * This);
        
        ULONG ( STDMETHODCALLTYPE *Release )( 
            IVSMDDesignerService * This);
        
        /* [propget] */ HRESULT ( STDMETHODCALLTYPE *get_DesignViewAttribute )( 
            IVSMDDesignerService * This,
            /* [retval][out] */ __RPC__deref_out_opt BSTR *pbstrAttribute);
        
        HRESULT ( STDMETHODCALLTYPE *CreateDesigner )( 
            IVSMDDesignerService * This,
            /* [in] */ __RPC__in_opt IServiceProvider *pSp,
            /* [in] */ __RPC__in_opt IUnknown *pDesignerLoader,
            /* [retval][out] */ __RPC__deref_out_opt IVSMDDesigner **ppDesigner);
        
        HRESULT ( STDMETHODCALLTYPE *CreateDesignerForClass )( 
            IVSMDDesignerService * This,
            /* [in] */ __RPC__in_opt IServiceProvider *pSp,
            /* [in] */ __RPC__in LPCOLESTR pwszComponentClass,
            /* [retval][out] */ __RPC__deref_out_opt IVSMDDesigner **ppDesigner);
        
        HRESULT ( STDMETHODCALLTYPE *CreateDesignerLoader )( 
            IVSMDDesignerService * This,
            /* [in] */ __RPC__in LPCOLESTR pwszCodeStreamClass,
            /* [retval][out] */ __RPC__deref_out_opt IUnknown **ppCodeStream);
        
        HRESULT ( STDMETHODCALLTYPE *GetDesignerLoaderClassForFile )( 
            IVSMDDesignerService * This,
            /* [in] */ __RPC__in LPCOLESTR pwszFileName,
            /* [retval][out] */ __RPC__deref_out_opt BSTR *pbstrDesignerLoaderClass);
        
        HRESULT ( STDMETHODCALLTYPE *RegisterDesignViewAttribute )( 
            IVSMDDesignerService * This,
            /* [in] */ __RPC__in_opt IVsHierarchy *pHier,
            /* [in] */ VSITEMID itemid,
            /* [in] */ int dwClass,
            /* [in] */ __RPC__in LPOLESTR pwszAttributeValue);
        
        END_INTERFACE
    } IVSMDDesignerServiceVtbl;

    interface IVSMDDesignerService
    {
        CONST_VTBL struct IVSMDDesignerServiceVtbl *lpVtbl;
    };

    

#ifdef COBJMACROS


#define IVSMDDesignerService_QueryInterface(This,riid,ppvObject)	\
    ( (This)->lpVtbl -> QueryInterface(This,riid,ppvObject) ) 

#define IVSMDDesignerService_AddRef(This)	\
    ( (This)->lpVtbl -> AddRef(This) ) 

#define IVSMDDesignerService_Release(This)	\
    ( (This)->lpVtbl -> Release(This) ) 


#define IVSMDDesignerService_get_DesignViewAttribute(This,pbstrAttribute)	\
    ( (This)->lpVtbl -> get_DesignViewAttribute(This,pbstrAttribute) ) 

#define IVSMDDesignerService_CreateDesigner(This,pSp,pDesignerLoader,ppDesigner)	\
    ( (This)->lpVtbl -> CreateDesigner(This,pSp,pDesignerLoader,ppDesigner) ) 

#define IVSMDDesignerService_CreateDesignerForClass(This,pSp,pwszComponentClass,ppDesigner)	\
    ( (This)->lpVtbl -> CreateDesignerForClass(This,pSp,pwszComponentClass,ppDesigner) ) 

#define IVSMDDesignerService_CreateDesignerLoader(This,pwszCodeStreamClass,ppCodeStream)	\
    ( (This)->lpVtbl -> CreateDesignerLoader(This,pwszCodeStreamClass,ppCodeStream) ) 

#define IVSMDDesignerService_GetDesignerLoaderClassForFile(This,pwszFileName,pbstrDesignerLoaderClass)	\
    ( (This)->lpVtbl -> GetDesignerLoaderClassForFile(This,pwszFileName,pbstrDesignerLoaderClass) ) 

#define IVSMDDesignerService_RegisterDesignViewAttribute(This,pHier,itemid,dwClass,pwszAttributeValue)	\
    ( (This)->lpVtbl -> RegisterDesignViewAttribute(This,pHier,itemid,dwClass,pwszAttributeValue) ) 

#endif /* COBJMACROS */


#endif 	/* C style interface */




#endif 	/* __IVSMDDesignerService_INTERFACE_DEFINED__ */


#ifndef __IVSMDPropertyGrid_INTERFACE_DEFINED__
#define __IVSMDPropertyGrid_INTERFACE_DEFINED__

/* interface IVSMDPropertyGrid */
/* [uuid][object] */ 


EXTERN_C const IID IID_IVSMDPropertyGrid;

#if defined(__cplusplus) && !defined(CINTERFACE)
    
    MIDL_INTERFACE("74946837-37A0-11d2-A273-00C04F8EF4FF")
    IVSMDPropertyGrid : public IUnknown
    {
    public:
        virtual /* [propget] */ HRESULT STDMETHODCALLTYPE get_CommandsVisible( 
            /* [retval][out] */ __RPC__out VARIANT_BOOL *pfVisible) = 0;
        
        virtual /* [propget] */ HRESULT STDMETHODCALLTYPE get_Handle( 
            /* [retval][out] */ __RPC__deref_out_opt HWND *phwnd) = 0;
        
        virtual /* [propget] */ HRESULT STDMETHODCALLTYPE get_GridSort( 
            /* [retval][out] */ __RPC__out PROPERTYGRIDSORT *pSort) = 0;
        
        virtual /* [propput] */ HRESULT STDMETHODCALLTYPE put_GridSort( 
            /* [in] */ PROPERTYGRIDSORT sort) = 0;
        
        virtual /* [propget] */ HRESULT STDMETHODCALLTYPE get_SelectedPropertyName( 
            /* [retval][out] */ __RPC__deref_out_opt BSTR *pbstrName) = 0;
        
        virtual HRESULT STDMETHODCALLTYPE Dispose( void) = 0;
        
        virtual HRESULT STDMETHODCALLTYPE GetOption( 
            /* [in] */ PROPERTYGRIDOPTION option,
            /* [retval][out] */ __RPC__out VARIANT *pvtOption) = 0;
        
        virtual HRESULT STDMETHODCALLTYPE Refresh( void) = 0;
        
        virtual HRESULT STDMETHODCALLTYPE SetOption( 
            /* [in] */ PROPERTYGRIDOPTION option,
            /* [in] */ VARIANT vtOption) = 0;
        
        virtual HRESULT STDMETHODCALLTYPE SetSelectedObjects( 
            int cObjects,
            /* [size_is][in] */ __RPC__in_ecount_full(cObjects) IUnknown **ppUnk) = 0;
        
    };
    
#else 	/* C style interface */

    typedef struct IVSMDPropertyGridVtbl
    {
        BEGIN_INTERFACE
        
        HRESULT ( STDMETHODCALLTYPE *QueryInterface )( 
            IVSMDPropertyGrid * This,
            /* [in] */ __RPC__in REFIID riid,
            /* [iid_is][out] */ 
            __RPC__deref_out  void **ppvObject);
        
        ULONG ( STDMETHODCALLTYPE *AddRef )( 
            IVSMDPropertyGrid * This);
        
        ULONG ( STDMETHODCALLTYPE *Release )( 
            IVSMDPropertyGrid * This);
        
        /* [propget] */ HRESULT ( STDMETHODCALLTYPE *get_CommandsVisible )( 
            IVSMDPropertyGrid * This,
            /* [retval][out] */ __RPC__out VARIANT_BOOL *pfVisible);
        
        /* [propget] */ HRESULT ( STDMETHODCALLTYPE *get_Handle )( 
            IVSMDPropertyGrid * This,
            /* [retval][out] */ __RPC__deref_out_opt HWND *phwnd);
        
        /* [propget] */ HRESULT ( STDMETHODCALLTYPE *get_GridSort )( 
            IVSMDPropertyGrid * This,
            /* [retval][out] */ __RPC__out PROPERTYGRIDSORT *pSort);
        
        /* [propput] */ HRESULT ( STDMETHODCALLTYPE *put_GridSort )( 
            IVSMDPropertyGrid * This,
            /* [in] */ PROPERTYGRIDSORT sort);
        
        /* [propget] */ HRESULT ( STDMETHODCALLTYPE *get_SelectedPropertyName )( 
            IVSMDPropertyGrid * This,
            /* [retval][out] */ __RPC__deref_out_opt BSTR *pbstrName);
        
        HRESULT ( STDMETHODCALLTYPE *Dispose )( 
            IVSMDPropertyGrid * This);
        
        HRESULT ( STDMETHODCALLTYPE *GetOption )( 
            IVSMDPropertyGrid * This,
            /* [in] */ PROPERTYGRIDOPTION option,
            /* [retval][out] */ __RPC__out VARIANT *pvtOption);
        
        HRESULT ( STDMETHODCALLTYPE *Refresh )( 
            IVSMDPropertyGrid * This);
        
        HRESULT ( STDMETHODCALLTYPE *SetOption )( 
            IVSMDPropertyGrid * This,
            /* [in] */ PROPERTYGRIDOPTION option,
            /* [in] */ VARIANT vtOption);
        
        HRESULT ( STDMETHODCALLTYPE *SetSelectedObjects )( 
            IVSMDPropertyGrid * This,
            int cObjects,
            /* [size_is][in] */ __RPC__in_ecount_full(cObjects) IUnknown **ppUnk);
        
        END_INTERFACE
    } IVSMDPropertyGridVtbl;

    interface IVSMDPropertyGrid
    {
        CONST_VTBL struct IVSMDPropertyGridVtbl *lpVtbl;
    };

    

#ifdef COBJMACROS


#define IVSMDPropertyGrid_QueryInterface(This,riid,ppvObject)	\
    ( (This)->lpVtbl -> QueryInterface(This,riid,ppvObject) ) 

#define IVSMDPropertyGrid_AddRef(This)	\
    ( (This)->lpVtbl -> AddRef(This) ) 

#define IVSMDPropertyGrid_Release(This)	\
    ( (This)->lpVtbl -> Release(This) ) 


#define IVSMDPropertyGrid_get_CommandsVisible(This,pfVisible)	\
    ( (This)->lpVtbl -> get_CommandsVisible(This,pfVisible) ) 

#define IVSMDPropertyGrid_get_Handle(This,phwnd)	\
    ( (This)->lpVtbl -> get_Handle(This,phwnd) ) 

#define IVSMDPropertyGrid_get_GridSort(This,pSort)	\
    ( (This)->lpVtbl -> get_GridSort(This,pSort) ) 

#define IVSMDPropertyGrid_put_GridSort(This,sort)	\
    ( (This)->lpVtbl -> put_GridSort(This,sort) ) 

#define IVSMDPropertyGrid_get_SelectedPropertyName(This,pbstrName)	\
    ( (This)->lpVtbl -> get_SelectedPropertyName(This,pbstrName) ) 

#define IVSMDPropertyGrid_Dispose(This)	\
    ( (This)->lpVtbl -> Dispose(This) ) 

#define IVSMDPropertyGrid_GetOption(This,option,pvtOption)	\
    ( (This)->lpVtbl -> GetOption(This,option,pvtOption) ) 

#define IVSMDPropertyGrid_Refresh(This)	\
    ( (This)->lpVtbl -> Refresh(This) ) 

#define IVSMDPropertyGrid_SetOption(This,option,vtOption)	\
    ( (This)->lpVtbl -> SetOption(This,option,vtOption) ) 

#define IVSMDPropertyGrid_SetSelectedObjects(This,cObjects,ppUnk)	\
    ( (This)->lpVtbl -> SetSelectedObjects(This,cObjects,ppUnk) ) 

#endif /* COBJMACROS */


#endif 	/* C style interface */




#endif 	/* __IVSMDPropertyGrid_INTERFACE_DEFINED__ */


#ifndef __IVSMDPropertyBrowser_INTERFACE_DEFINED__
#define __IVSMDPropertyBrowser_INTERFACE_DEFINED__

/* interface IVSMDPropertyBrowser */
/* [uuid][object] */ 


EXTERN_C const IID IID_IVSMDPropertyBrowser;

#if defined(__cplusplus) && !defined(CINTERFACE)
    
    MIDL_INTERFACE("74946810-37A0-11d2-A273-00C04F8EF4FF")
    IVSMDPropertyBrowser : public IUnknown
    {
    public:
        virtual /* [propget] */ HRESULT STDMETHODCALLTYPE get_WindowGlyphResourceID( 
            /* [retval][out] */ __RPC__out DWORD *pdwResID) = 0;
        
        virtual HRESULT STDMETHODCALLTYPE CreatePropertyGrid( 
            /* [retval][out] */ __RPC__deref_out_opt IVSMDPropertyGrid **ppGrid) = 0;
        
        virtual HRESULT STDMETHODCALLTYPE Refresh( void) = 0;
        
    };
    
#else 	/* C style interface */

    typedef struct IVSMDPropertyBrowserVtbl
    {
        BEGIN_INTERFACE
        
        HRESULT ( STDMETHODCALLTYPE *QueryInterface )( 
            IVSMDPropertyBrowser * This,
            /* [in] */ __RPC__in REFIID riid,
            /* [iid_is][out] */ 
            __RPC__deref_out  void **ppvObject);
        
        ULONG ( STDMETHODCALLTYPE *AddRef )( 
            IVSMDPropertyBrowser * This);
        
        ULONG ( STDMETHODCALLTYPE *Release )( 
            IVSMDPropertyBrowser * This);
        
        /* [propget] */ HRESULT ( STDMETHODCALLTYPE *get_WindowGlyphResourceID )( 
            IVSMDPropertyBrowser * This,
            /* [retval][out] */ __RPC__out DWORD *pdwResID);
        
        HRESULT ( STDMETHODCALLTYPE *CreatePropertyGrid )( 
            IVSMDPropertyBrowser * This,
            /* [retval][out] */ __RPC__deref_out_opt IVSMDPropertyGrid **ppGrid);
        
        HRESULT ( STDMETHODCALLTYPE *Refresh )( 
            IVSMDPropertyBrowser * This);
        
        END_INTERFACE
    } IVSMDPropertyBrowserVtbl;

    interface IVSMDPropertyBrowser
    {
        CONST_VTBL struct IVSMDPropertyBrowserVtbl *lpVtbl;
    };

    

#ifdef COBJMACROS


#define IVSMDPropertyBrowser_QueryInterface(This,riid,ppvObject)	\
    ( (This)->lpVtbl -> QueryInterface(This,riid,ppvObject) ) 

#define IVSMDPropertyBrowser_AddRef(This)	\
    ( (This)->lpVtbl -> AddRef(This) ) 

#define IVSMDPropertyBrowser_Release(This)	\
    ( (This)->lpVtbl -> Release(This) ) 


#define IVSMDPropertyBrowser_get_WindowGlyphResourceID(This,pdwResID)	\
    ( (This)->lpVtbl -> get_WindowGlyphResourceID(This,pdwResID) ) 

#define IVSMDPropertyBrowser_CreatePropertyGrid(This,ppGrid)	\
    ( (This)->lpVtbl -> CreatePropertyGrid(This,ppGrid) ) 

#define IVSMDPropertyBrowser_Refresh(This)	\
    ( (This)->lpVtbl -> Refresh(This) ) 

#endif /* COBJMACROS */


#endif 	/* C style interface */




#endif 	/* __IVSMDPropertyBrowser_INTERFACE_DEFINED__ */


#ifndef __IVSMDPerPropertyBrowsing_INTERFACE_DEFINED__
#define __IVSMDPerPropertyBrowsing_INTERFACE_DEFINED__

/* interface IVSMDPerPropertyBrowsing */
/* [unique][uuid][object] */ 


EXTERN_C const IID IID_IVSMDPerPropertyBrowsing;

#if defined(__cplusplus) && !defined(CINTERFACE)
    
    MIDL_INTERFACE("7494683C-37A0-11d2-A273-00C04F8EF4FF")
    IVSMDPerPropertyBrowsing : public IUnknown
    {
    public:
        virtual /* [helpstring][id] */ HRESULT STDMETHODCALLTYPE GetPropertyAttributes( 
            DISPID dispid,
            /* [out] */ __RPC__out UINT *pceltAttrs,
            /* [size_is][size_is][out] */ __RPC__deref_out_ecount_full_opt(*pceltAttrs) BSTR **ppbstrTypeNames,
            /* [size_is][size_is][out] */ __RPC__deref_out_ecount_full_opt(*pceltAttrs) VARIANT **ppvarAttrValues) = 0;
        
    };
    
#else 	/* C style interface */

    typedef struct IVSMDPerPropertyBrowsingVtbl
    {
        BEGIN_INTERFACE
        
        HRESULT ( STDMETHODCALLTYPE *QueryInterface )( 
            IVSMDPerPropertyBrowsing * This,
            /* [in] */ __RPC__in REFIID riid,
            /* [iid_is][out] */ 
            __RPC__deref_out  void **ppvObject);
        
        ULONG ( STDMETHODCALLTYPE *AddRef )( 
            IVSMDPerPropertyBrowsing * This);
        
        ULONG ( STDMETHODCALLTYPE *Release )( 
            IVSMDPerPropertyBrowsing * This);
        
        /* [helpstring][id] */ HRESULT ( STDMETHODCALLTYPE *GetPropertyAttributes )( 
            IVSMDPerPropertyBrowsing * This,
            DISPID dispid,
            /* [out] */ __RPC__out UINT *pceltAttrs,
            /* [size_is][size_is][out] */ __RPC__deref_out_ecount_full_opt(*pceltAttrs) BSTR **ppbstrTypeNames,
            /* [size_is][size_is][out] */ __RPC__deref_out_ecount_full_opt(*pceltAttrs) VARIANT **ppvarAttrValues);
        
        END_INTERFACE
    } IVSMDPerPropertyBrowsingVtbl;

    interface IVSMDPerPropertyBrowsing
    {
        CONST_VTBL struct IVSMDPerPropertyBrowsingVtbl *lpVtbl;
    };

    

#ifdef COBJMACROS


#define IVSMDPerPropertyBrowsing_QueryInterface(This,riid,ppvObject)	\
    ( (This)->lpVtbl -> QueryInterface(This,riid,ppvObject) ) 

#define IVSMDPerPropertyBrowsing_AddRef(This)	\
    ( (This)->lpVtbl -> AddRef(This) ) 

#define IVSMDPerPropertyBrowsing_Release(This)	\
    ( (This)->lpVtbl -> Release(This) ) 


#define IVSMDPerPropertyBrowsing_GetPropertyAttributes(This,dispid,pceltAttrs,ppbstrTypeNames,ppvarAttrValues)	\
    ( (This)->lpVtbl -> GetPropertyAttributes(This,dispid,pceltAttrs,ppbstrTypeNames,ppvarAttrValues) ) 

#endif /* COBJMACROS */


#endif 	/* C style interface */




#endif 	/* __IVSMDPerPropertyBrowsing_INTERFACE_DEFINED__ */

#endif /* __VSManagedDesigner_LIBRARY_DEFINED__ */

/* Additional Prototypes for ALL interfaces */

/* end of Additional Prototypes */

#ifdef __cplusplus
}
#endif

#endif


