

/* this ALWAYS GENERATED file contains the definitions for the interfaces */


 /* File created by MIDL compiler version 7.00.0499 */
/* Compiler settings for IVsWebServices.idl:
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


#ifndef __IVsWebServices_h__
#define __IVsWebServices_h__

#if defined(_MSC_VER) && (_MSC_VER >= 1020)
#pragma once
#endif

/* Forward Declarations */ 

#ifndef __IVsWebServiceProvider_FWD_DEFINED__
#define __IVsWebServiceProvider_FWD_DEFINED__
typedef interface IVsWebServiceProvider IVsWebServiceProvider;
#endif 	/* __IVsWebServiceProvider_FWD_DEFINED__ */


#ifndef __IVsWebService_FWD_DEFINED__
#define __IVsWebService_FWD_DEFINED__
typedef interface IVsWebService IVsWebService;
#endif 	/* __IVsWebService_FWD_DEFINED__ */


#ifndef __IVsWebServiceEvents_FWD_DEFINED__
#define __IVsWebServiceEvents_FWD_DEFINED__
typedef interface IVsWebServiceEvents IVsWebServiceEvents;
#endif 	/* __IVsWebServiceEvents_FWD_DEFINED__ */


#ifndef __IVsWebServiceProviderEvents_FWD_DEFINED__
#define __IVsWebServiceProviderEvents_FWD_DEFINED__
typedef interface IVsWebServiceProviderEvents IVsWebServiceProviderEvents;
#endif 	/* __IVsWebServiceProviderEvents_FWD_DEFINED__ */


#ifndef __IEnumWebServices_FWD_DEFINED__
#define __IEnumWebServices_FWD_DEFINED__
typedef interface IEnumWebServices IEnumWebServices;
#endif 	/* __IEnumWebServices_FWD_DEFINED__ */


/* header files for imported files */
#include "oaidl.h"
#include "ocidl.h"
#include "vsshell.h"

#ifdef __cplusplus
extern "C"{
#endif 



#ifndef __VsWebServices_LIBRARY_DEFINED__
#define __VsWebServices_LIBRARY_DEFINED__

/* library VsWebServices */
/* [helpstring][version][uuid] */ 




EXTERN_C const IID LIBID_VsWebServices;

#ifndef __IVsWebServiceProvider_INTERFACE_DEFINED__
#define __IVsWebServiceProvider_INTERFACE_DEFINED__

/* interface IVsWebServiceProvider */
/* [object][unique][helpstring][uuid] */ 


EXTERN_C const IID IID_IVsWebServiceProvider;

#if defined(__cplusplus) && !defined(CINTERFACE)
    
    MIDL_INTERFACE("C736666C-6970-4ae7-9B1F-5C0407A30556")
    IVsWebServiceProvider : public IUnknown
    {
    public:
        virtual HRESULT STDMETHODCALLTYPE WebServices( 
            /* [retval][out] */ __RPC__deref_out_opt IEnumWebServices **ppIEnumWebServices) = 0;
        
        virtual HRESULT STDMETHODCALLTYPE GetWebService( 
            /* [in] */ __RPC__in LPCOLESTR pszUrl,
            /* [retval][out] */ __RPC__deref_out_opt IVsWebService **ppIVsWebService) = 0;
        
        virtual HRESULT STDMETHODCALLTYPE StartServer( void) = 0;
        
        virtual HRESULT STDMETHODCALLTYPE AdviseWebServiceProviderEvents( 
            /* [in] */ __RPC__in_opt IVsWebServiceProviderEvents *pEvents,
            /* [retval][out] */ __RPC__out VSCOOKIE *pdwCookie) = 0;
        
        virtual HRESULT STDMETHODCALLTYPE UnadviseWebServiceProviderEvents( 
            /* [in] */ VSCOOKIE dwCookie) = 0;
        
        virtual HRESULT STDMETHODCALLTYPE EnsureServerRunning( 
            /* [retval][out] */ __RPC__deref_out_opt BSTR *pbstrUrl) = 0;
        
        virtual HRESULT STDMETHODCALLTYPE ApplicationUrl( 
            /* [retval][out] */ __RPC__deref_out_opt BSTR *pbstrUrl) = 0;
        
    };
    
#else 	/* C style interface */

    typedef struct IVsWebServiceProviderVtbl
    {
        BEGIN_INTERFACE
        
        HRESULT ( STDMETHODCALLTYPE *QueryInterface )( 
            IVsWebServiceProvider * This,
            /* [in] */ __RPC__in REFIID riid,
            /* [iid_is][out] */ 
            __RPC__deref_out  void **ppvObject);
        
        ULONG ( STDMETHODCALLTYPE *AddRef )( 
            IVsWebServiceProvider * This);
        
        ULONG ( STDMETHODCALLTYPE *Release )( 
            IVsWebServiceProvider * This);
        
        HRESULT ( STDMETHODCALLTYPE *WebServices )( 
            IVsWebServiceProvider * This,
            /* [retval][out] */ __RPC__deref_out_opt IEnumWebServices **ppIEnumWebServices);
        
        HRESULT ( STDMETHODCALLTYPE *GetWebService )( 
            IVsWebServiceProvider * This,
            /* [in] */ __RPC__in LPCOLESTR pszUrl,
            /* [retval][out] */ __RPC__deref_out_opt IVsWebService **ppIVsWebService);
        
        HRESULT ( STDMETHODCALLTYPE *StartServer )( 
            IVsWebServiceProvider * This);
        
        HRESULT ( STDMETHODCALLTYPE *AdviseWebServiceProviderEvents )( 
            IVsWebServiceProvider * This,
            /* [in] */ __RPC__in_opt IVsWebServiceProviderEvents *pEvents,
            /* [retval][out] */ __RPC__out VSCOOKIE *pdwCookie);
        
        HRESULT ( STDMETHODCALLTYPE *UnadviseWebServiceProviderEvents )( 
            IVsWebServiceProvider * This,
            /* [in] */ VSCOOKIE dwCookie);
        
        HRESULT ( STDMETHODCALLTYPE *EnsureServerRunning )( 
            IVsWebServiceProvider * This,
            /* [retval][out] */ __RPC__deref_out_opt BSTR *pbstrUrl);
        
        HRESULT ( STDMETHODCALLTYPE *ApplicationUrl )( 
            IVsWebServiceProvider * This,
            /* [retval][out] */ __RPC__deref_out_opt BSTR *pbstrUrl);
        
        END_INTERFACE
    } IVsWebServiceProviderVtbl;

    interface IVsWebServiceProvider
    {
        CONST_VTBL struct IVsWebServiceProviderVtbl *lpVtbl;
    };

    

#ifdef COBJMACROS


#define IVsWebServiceProvider_QueryInterface(This,riid,ppvObject)	\
    ( (This)->lpVtbl -> QueryInterface(This,riid,ppvObject) ) 

#define IVsWebServiceProvider_AddRef(This)	\
    ( (This)->lpVtbl -> AddRef(This) ) 

#define IVsWebServiceProvider_Release(This)	\
    ( (This)->lpVtbl -> Release(This) ) 


#define IVsWebServiceProvider_WebServices(This,ppIEnumWebServices)	\
    ( (This)->lpVtbl -> WebServices(This,ppIEnumWebServices) ) 

#define IVsWebServiceProvider_GetWebService(This,pszUrl,ppIVsWebService)	\
    ( (This)->lpVtbl -> GetWebService(This,pszUrl,ppIVsWebService) ) 

#define IVsWebServiceProvider_StartServer(This)	\
    ( (This)->lpVtbl -> StartServer(This) ) 

#define IVsWebServiceProvider_AdviseWebServiceProviderEvents(This,pEvents,pdwCookie)	\
    ( (This)->lpVtbl -> AdviseWebServiceProviderEvents(This,pEvents,pdwCookie) ) 

#define IVsWebServiceProvider_UnadviseWebServiceProviderEvents(This,dwCookie)	\
    ( (This)->lpVtbl -> UnadviseWebServiceProviderEvents(This,dwCookie) ) 

#define IVsWebServiceProvider_EnsureServerRunning(This,pbstrUrl)	\
    ( (This)->lpVtbl -> EnsureServerRunning(This,pbstrUrl) ) 

#define IVsWebServiceProvider_ApplicationUrl(This,pbstrUrl)	\
    ( (This)->lpVtbl -> ApplicationUrl(This,pbstrUrl) ) 

#endif /* COBJMACROS */


#endif 	/* C style interface */




#endif 	/* __IVsWebServiceProvider_INTERFACE_DEFINED__ */


#ifndef __IVsWebService_INTERFACE_DEFINED__
#define __IVsWebService_INTERFACE_DEFINED__

/* interface IVsWebService */
/* [object][unique][helpstring][uuid] */ 


EXTERN_C const IID IID_IVsWebService;

#if defined(__cplusplus) && !defined(CINTERFACE)
    
    MIDL_INTERFACE("D02603F6-BDB0-410c-9BA8-9E34E5FF4BEC")
    IVsWebService : public IUnknown
    {
    public:
        virtual HRESULT STDMETHODCALLTYPE Url( 
            /* [retval][out] */ __RPC__deref_out_opt BSTR *bstrUrl) = 0;
        
        virtual HRESULT STDMETHODCALLTYPE AppRelativeUrl( 
            /* [retval][out] */ __RPC__deref_out_opt BSTR *bstrAppUrl) = 0;
        
        virtual HRESULT STDMETHODCALLTYPE GetProvider( 
            /* [retval][out] */ __RPC__deref_out_opt IVsWebServiceProvider **ppIVsWebServiceProvider) = 0;
        
        virtual HRESULT STDMETHODCALLTYPE AdviseWebServiceEvents( 
            /* [in] */ __RPC__in_opt IVsWebServiceEvents *pEvents,
            /* [retval][out] */ __RPC__out VSCOOKIE *pdwCookie) = 0;
        
        virtual HRESULT STDMETHODCALLTYPE UnadviseWebServiceEvents( 
            /* [in] */ VSCOOKIE dwCookie) = 0;
        
    };
    
#else 	/* C style interface */

    typedef struct IVsWebServiceVtbl
    {
        BEGIN_INTERFACE
        
        HRESULT ( STDMETHODCALLTYPE *QueryInterface )( 
            IVsWebService * This,
            /* [in] */ __RPC__in REFIID riid,
            /* [iid_is][out] */ 
            __RPC__deref_out  void **ppvObject);
        
        ULONG ( STDMETHODCALLTYPE *AddRef )( 
            IVsWebService * This);
        
        ULONG ( STDMETHODCALLTYPE *Release )( 
            IVsWebService * This);
        
        HRESULT ( STDMETHODCALLTYPE *Url )( 
            IVsWebService * This,
            /* [retval][out] */ __RPC__deref_out_opt BSTR *bstrUrl);
        
        HRESULT ( STDMETHODCALLTYPE *AppRelativeUrl )( 
            IVsWebService * This,
            /* [retval][out] */ __RPC__deref_out_opt BSTR *bstrAppUrl);
        
        HRESULT ( STDMETHODCALLTYPE *GetProvider )( 
            IVsWebService * This,
            /* [retval][out] */ __RPC__deref_out_opt IVsWebServiceProvider **ppIVsWebServiceProvider);
        
        HRESULT ( STDMETHODCALLTYPE *AdviseWebServiceEvents )( 
            IVsWebService * This,
            /* [in] */ __RPC__in_opt IVsWebServiceEvents *pEvents,
            /* [retval][out] */ __RPC__out VSCOOKIE *pdwCookie);
        
        HRESULT ( STDMETHODCALLTYPE *UnadviseWebServiceEvents )( 
            IVsWebService * This,
            /* [in] */ VSCOOKIE dwCookie);
        
        END_INTERFACE
    } IVsWebServiceVtbl;

    interface IVsWebService
    {
        CONST_VTBL struct IVsWebServiceVtbl *lpVtbl;
    };

    

#ifdef COBJMACROS


#define IVsWebService_QueryInterface(This,riid,ppvObject)	\
    ( (This)->lpVtbl -> QueryInterface(This,riid,ppvObject) ) 

#define IVsWebService_AddRef(This)	\
    ( (This)->lpVtbl -> AddRef(This) ) 

#define IVsWebService_Release(This)	\
    ( (This)->lpVtbl -> Release(This) ) 


#define IVsWebService_Url(This,bstrUrl)	\
    ( (This)->lpVtbl -> Url(This,bstrUrl) ) 

#define IVsWebService_AppRelativeUrl(This,bstrAppUrl)	\
    ( (This)->lpVtbl -> AppRelativeUrl(This,bstrAppUrl) ) 

#define IVsWebService_GetProvider(This,ppIVsWebServiceProvider)	\
    ( (This)->lpVtbl -> GetProvider(This,ppIVsWebServiceProvider) ) 

#define IVsWebService_AdviseWebServiceEvents(This,pEvents,pdwCookie)	\
    ( (This)->lpVtbl -> AdviseWebServiceEvents(This,pEvents,pdwCookie) ) 

#define IVsWebService_UnadviseWebServiceEvents(This,dwCookie)	\
    ( (This)->lpVtbl -> UnadviseWebServiceEvents(This,dwCookie) ) 

#endif /* COBJMACROS */


#endif 	/* C style interface */




#endif 	/* __IVsWebService_INTERFACE_DEFINED__ */


#ifndef __IVsWebServiceEvents_INTERFACE_DEFINED__
#define __IVsWebServiceEvents_INTERFACE_DEFINED__

/* interface IVsWebServiceEvents */
/* [object][unique][version][uuid] */ 


EXTERN_C const IID IID_IVsWebServiceEvents;

#if defined(__cplusplus) && !defined(CINTERFACE)
    
    MIDL_INTERFACE("F20AB4A1-4BA3-4842-AC78-2550669F751A")
    IVsWebServiceEvents : public IUnknown
    {
    public:
        virtual HRESULT STDMETHODCALLTYPE OnRemoved( 
            /* [in] */ __RPC__in LPCOLESTR pszOldURL) = 0;
        
        virtual HRESULT STDMETHODCALLTYPE OnRenamed( 
            /* [in] */ __RPC__in LPCOLESTR pszOldURL,
            /* [in] */ __RPC__in LPCOLESTR pszNewURL) = 0;
        
        virtual HRESULT STDMETHODCALLTYPE OnChanged( 
            /* [in] */ __RPC__in_opt IVsWebService *pIVsWebReference) = 0;
        
    };
    
#else 	/* C style interface */

    typedef struct IVsWebServiceEventsVtbl
    {
        BEGIN_INTERFACE
        
        HRESULT ( STDMETHODCALLTYPE *QueryInterface )( 
            IVsWebServiceEvents * This,
            /* [in] */ __RPC__in REFIID riid,
            /* [iid_is][out] */ 
            __RPC__deref_out  void **ppvObject);
        
        ULONG ( STDMETHODCALLTYPE *AddRef )( 
            IVsWebServiceEvents * This);
        
        ULONG ( STDMETHODCALLTYPE *Release )( 
            IVsWebServiceEvents * This);
        
        HRESULT ( STDMETHODCALLTYPE *OnRemoved )( 
            IVsWebServiceEvents * This,
            /* [in] */ __RPC__in LPCOLESTR pszOldURL);
        
        HRESULT ( STDMETHODCALLTYPE *OnRenamed )( 
            IVsWebServiceEvents * This,
            /* [in] */ __RPC__in LPCOLESTR pszOldURL,
            /* [in] */ __RPC__in LPCOLESTR pszNewURL);
        
        HRESULT ( STDMETHODCALLTYPE *OnChanged )( 
            IVsWebServiceEvents * This,
            /* [in] */ __RPC__in_opt IVsWebService *pIVsWebReference);
        
        END_INTERFACE
    } IVsWebServiceEventsVtbl;

    interface IVsWebServiceEvents
    {
        CONST_VTBL struct IVsWebServiceEventsVtbl *lpVtbl;
    };

    

#ifdef COBJMACROS


#define IVsWebServiceEvents_QueryInterface(This,riid,ppvObject)	\
    ( (This)->lpVtbl -> QueryInterface(This,riid,ppvObject) ) 

#define IVsWebServiceEvents_AddRef(This)	\
    ( (This)->lpVtbl -> AddRef(This) ) 

#define IVsWebServiceEvents_Release(This)	\
    ( (This)->lpVtbl -> Release(This) ) 


#define IVsWebServiceEvents_OnRemoved(This,pszOldURL)	\
    ( (This)->lpVtbl -> OnRemoved(This,pszOldURL) ) 

#define IVsWebServiceEvents_OnRenamed(This,pszOldURL,pszNewURL)	\
    ( (This)->lpVtbl -> OnRenamed(This,pszOldURL,pszNewURL) ) 

#define IVsWebServiceEvents_OnChanged(This,pIVsWebReference)	\
    ( (This)->lpVtbl -> OnChanged(This,pIVsWebReference) ) 

#endif /* COBJMACROS */


#endif 	/* C style interface */




#endif 	/* __IVsWebServiceEvents_INTERFACE_DEFINED__ */


#ifndef __IVsWebServiceProviderEvents_INTERFACE_DEFINED__
#define __IVsWebServiceProviderEvents_INTERFACE_DEFINED__

/* interface IVsWebServiceProviderEvents */
/* [object][unique][version][uuid] */ 


EXTERN_C const IID IID_IVsWebServiceProviderEvents;

#if defined(__cplusplus) && !defined(CINTERFACE)
    
    MIDL_INTERFACE("8170CB5A-D602-4886-A990-8260A5928133")
    IVsWebServiceProviderEvents : public IUnknown
    {
    public:
        virtual HRESULT STDMETHODCALLTYPE OnAdded( 
            /* [in] */ __RPC__in_opt IVsWebService *pIVsWebReference) = 0;
        
        virtual HRESULT STDMETHODCALLTYPE OnRemoved( 
            /* [in] */ __RPC__in LPCOLESTR pszURL) = 0;
        
    };
    
#else 	/* C style interface */

    typedef struct IVsWebServiceProviderEventsVtbl
    {
        BEGIN_INTERFACE
        
        HRESULT ( STDMETHODCALLTYPE *QueryInterface )( 
            IVsWebServiceProviderEvents * This,
            /* [in] */ __RPC__in REFIID riid,
            /* [iid_is][out] */ 
            __RPC__deref_out  void **ppvObject);
        
        ULONG ( STDMETHODCALLTYPE *AddRef )( 
            IVsWebServiceProviderEvents * This);
        
        ULONG ( STDMETHODCALLTYPE *Release )( 
            IVsWebServiceProviderEvents * This);
        
        HRESULT ( STDMETHODCALLTYPE *OnAdded )( 
            IVsWebServiceProviderEvents * This,
            /* [in] */ __RPC__in_opt IVsWebService *pIVsWebReference);
        
        HRESULT ( STDMETHODCALLTYPE *OnRemoved )( 
            IVsWebServiceProviderEvents * This,
            /* [in] */ __RPC__in LPCOLESTR pszURL);
        
        END_INTERFACE
    } IVsWebServiceProviderEventsVtbl;

    interface IVsWebServiceProviderEvents
    {
        CONST_VTBL struct IVsWebServiceProviderEventsVtbl *lpVtbl;
    };

    

#ifdef COBJMACROS


#define IVsWebServiceProviderEvents_QueryInterface(This,riid,ppvObject)	\
    ( (This)->lpVtbl -> QueryInterface(This,riid,ppvObject) ) 

#define IVsWebServiceProviderEvents_AddRef(This)	\
    ( (This)->lpVtbl -> AddRef(This) ) 

#define IVsWebServiceProviderEvents_Release(This)	\
    ( (This)->lpVtbl -> Release(This) ) 


#define IVsWebServiceProviderEvents_OnAdded(This,pIVsWebReference)	\
    ( (This)->lpVtbl -> OnAdded(This,pIVsWebReference) ) 

#define IVsWebServiceProviderEvents_OnRemoved(This,pszURL)	\
    ( (This)->lpVtbl -> OnRemoved(This,pszURL) ) 

#endif /* COBJMACROS */


#endif 	/* C style interface */




#endif 	/* __IVsWebServiceProviderEvents_INTERFACE_DEFINED__ */


#ifndef __IEnumWebServices_INTERFACE_DEFINED__
#define __IEnumWebServices_INTERFACE_DEFINED__

/* interface IEnumWebServices */
/* [object][unique][helpstring][uuid] */ 


EXTERN_C const IID IID_IEnumWebServices;

#if defined(__cplusplus) && !defined(CINTERFACE)
    
    MIDL_INTERFACE("4DB4E7D3-5550-41ae-A612-3D28CD0C52A8")
    IEnumWebServices : public IUnknown
    {
    public:
        virtual HRESULT STDMETHODCALLTYPE Next( 
            /* [in] */ ULONG celt,
            /* [length_is][size_is][out] */ __RPC__out_ecount_part(celt, *pceltFetched) IVsWebService **rgelt,
            /* [out] */ __RPC__out ULONG *pceltFetched) = 0;
        
        virtual HRESULT STDMETHODCALLTYPE Skip( 
            /* [in] */ ULONG celt) = 0;
        
        virtual HRESULT STDMETHODCALLTYPE Reset( void) = 0;
        
        virtual HRESULT STDMETHODCALLTYPE Clone( 
            /* [out] */ __RPC__deref_out_opt IEnumWebServices **ppenum) = 0;
        
    };
    
#else 	/* C style interface */

    typedef struct IEnumWebServicesVtbl
    {
        BEGIN_INTERFACE
        
        HRESULT ( STDMETHODCALLTYPE *QueryInterface )( 
            IEnumWebServices * This,
            /* [in] */ __RPC__in REFIID riid,
            /* [iid_is][out] */ 
            __RPC__deref_out  void **ppvObject);
        
        ULONG ( STDMETHODCALLTYPE *AddRef )( 
            IEnumWebServices * This);
        
        ULONG ( STDMETHODCALLTYPE *Release )( 
            IEnumWebServices * This);
        
        HRESULT ( STDMETHODCALLTYPE *Next )( 
            IEnumWebServices * This,
            /* [in] */ ULONG celt,
            /* [length_is][size_is][out] */ __RPC__out_ecount_part(celt, *pceltFetched) IVsWebService **rgelt,
            /* [out] */ __RPC__out ULONG *pceltFetched);
        
        HRESULT ( STDMETHODCALLTYPE *Skip )( 
            IEnumWebServices * This,
            /* [in] */ ULONG celt);
        
        HRESULT ( STDMETHODCALLTYPE *Reset )( 
            IEnumWebServices * This);
        
        HRESULT ( STDMETHODCALLTYPE *Clone )( 
            IEnumWebServices * This,
            /* [out] */ __RPC__deref_out_opt IEnumWebServices **ppenum);
        
        END_INTERFACE
    } IEnumWebServicesVtbl;

    interface IEnumWebServices
    {
        CONST_VTBL struct IEnumWebServicesVtbl *lpVtbl;
    };

    

#ifdef COBJMACROS


#define IEnumWebServices_QueryInterface(This,riid,ppvObject)	\
    ( (This)->lpVtbl -> QueryInterface(This,riid,ppvObject) ) 

#define IEnumWebServices_AddRef(This)	\
    ( (This)->lpVtbl -> AddRef(This) ) 

#define IEnumWebServices_Release(This)	\
    ( (This)->lpVtbl -> Release(This) ) 


#define IEnumWebServices_Next(This,celt,rgelt,pceltFetched)	\
    ( (This)->lpVtbl -> Next(This,celt,rgelt,pceltFetched) ) 

#define IEnumWebServices_Skip(This,celt)	\
    ( (This)->lpVtbl -> Skip(This,celt) ) 

#define IEnumWebServices_Reset(This)	\
    ( (This)->lpVtbl -> Reset(This) ) 

#define IEnumWebServices_Clone(This,ppenum)	\
    ( (This)->lpVtbl -> Clone(This,ppenum) ) 

#endif /* COBJMACROS */


#endif 	/* C style interface */




#endif 	/* __IEnumWebServices_INTERFACE_DEFINED__ */

#endif /* __VsWebServices_LIBRARY_DEFINED__ */

/* Additional Prototypes for ALL interfaces */

/* end of Additional Prototypes */

#ifdef __cplusplus
}
#endif

#endif


