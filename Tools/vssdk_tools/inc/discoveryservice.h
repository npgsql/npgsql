

/* this ALWAYS GENERATED file contains the definitions for the interfaces */


 /* File created by MIDL compiler version 7.00.0499 */
/* Compiler settings for discoveryservice.idl:
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

#ifndef __discoveryservice_h__
#define __discoveryservice_h__

#if defined(_MSC_VER) && (_MSC_VER >= 1020)
#pragma once
#endif

/* Forward Declarations */ 

#ifndef __IVsDiscoveryService_FWD_DEFINED__
#define __IVsDiscoveryService_FWD_DEFINED__
typedef interface IVsDiscoveryService IVsDiscoveryService;
#endif 	/* __IVsDiscoveryService_FWD_DEFINED__ */


#ifndef __IDiscoverUrlCallBack_FWD_DEFINED__
#define __IDiscoverUrlCallBack_FWD_DEFINED__
typedef interface IDiscoverUrlCallBack IDiscoverUrlCallBack;
#endif 	/* __IDiscoverUrlCallBack_FWD_DEFINED__ */


#ifndef __IDiscoveryResult_FWD_DEFINED__
#define __IDiscoveryResult_FWD_DEFINED__
typedef interface IDiscoveryResult IDiscoveryResult;
#endif 	/* __IDiscoveryResult_FWD_DEFINED__ */


#ifndef __IReferenceInfo_FWD_DEFINED__
#define __IReferenceInfo_FWD_DEFINED__
typedef interface IReferenceInfo IReferenceInfo;
#endif 	/* __IReferenceInfo_FWD_DEFINED__ */


#ifndef __ISchemaReferenceInfo_FWD_DEFINED__
#define __ISchemaReferenceInfo_FWD_DEFINED__
typedef interface ISchemaReferenceInfo ISchemaReferenceInfo;
#endif 	/* __ISchemaReferenceInfo_FWD_DEFINED__ */


#ifndef __IDiscoverySession_FWD_DEFINED__
#define __IDiscoverySession_FWD_DEFINED__
typedef interface IDiscoverySession IDiscoverySession;
#endif 	/* __IDiscoverySession_FWD_DEFINED__ */


/* header files for imported files */
#include "ocidl.h"

#ifdef __cplusplus
extern "C"{
#endif 


/* interface __MIDL_itf_discoveryservice_0000_0000 */
/* [local] */ 












#define DISCOVERY_E_PROXYSETTINGINVALID MAKE_HRESULT(SEVERITY_ERROR, FACILITY_ITF, 0x600)

enum tagDiscoveryNodeType
    {	DiscoveryReferenceInfo	= 1,
	ServiceReferenceInfo	= 2,
	SchemaReferenceInfo	= 3,
	UnrecognizedReference	= 4
    } ;
typedef enum tagDiscoveryNodeType DiscoveryNodeType;



extern RPC_IF_HANDLE __MIDL_itf_discoveryservice_0000_0000_v0_0_c_ifspec;
extern RPC_IF_HANDLE __MIDL_itf_discoveryservice_0000_0000_v0_0_s_ifspec;

#ifndef __IVsDiscoveryService_INTERFACE_DEFINED__
#define __IVsDiscoveryService_INTERFACE_DEFINED__

/* interface IVsDiscoveryService */
/* [uuid][unique][object] */ 


EXTERN_C const IID IID_IVsDiscoveryService;

#if defined(__cplusplus) && !defined(CINTERFACE)
    
    MIDL_INTERFACE("B9A32C80-B14D-4ae3-A955-5CBC3E7FAB10")
    IVsDiscoveryService : public IUnknown
    {
    public:
        virtual HRESULT STDMETHODCALLTYPE CreateDiscoverySession( 
            /* [retval][out] */ __RPC__deref_out_opt IDiscoverySession **pSessionObject) = 0;
        
    };
    
#else 	/* C style interface */

    typedef struct IVsDiscoveryServiceVtbl
    {
        BEGIN_INTERFACE
        
        HRESULT ( STDMETHODCALLTYPE *QueryInterface )( 
            IVsDiscoveryService * This,
            /* [in] */ __RPC__in REFIID riid,
            /* [iid_is][out] */ 
            __RPC__deref_out  void **ppvObject);
        
        ULONG ( STDMETHODCALLTYPE *AddRef )( 
            IVsDiscoveryService * This);
        
        ULONG ( STDMETHODCALLTYPE *Release )( 
            IVsDiscoveryService * This);
        
        HRESULT ( STDMETHODCALLTYPE *CreateDiscoverySession )( 
            IVsDiscoveryService * This,
            /* [retval][out] */ __RPC__deref_out_opt IDiscoverySession **pSessionObject);
        
        END_INTERFACE
    } IVsDiscoveryServiceVtbl;

    interface IVsDiscoveryService
    {
        CONST_VTBL struct IVsDiscoveryServiceVtbl *lpVtbl;
    };

    

#ifdef COBJMACROS


#define IVsDiscoveryService_QueryInterface(This,riid,ppvObject)	\
    ( (This)->lpVtbl -> QueryInterface(This,riid,ppvObject) ) 

#define IVsDiscoveryService_AddRef(This)	\
    ( (This)->lpVtbl -> AddRef(This) ) 

#define IVsDiscoveryService_Release(This)	\
    ( (This)->lpVtbl -> Release(This) ) 


#define IVsDiscoveryService_CreateDiscoverySession(This,pSessionObject)	\
    ( (This)->lpVtbl -> CreateDiscoverySession(This,pSessionObject) ) 

#endif /* COBJMACROS */


#endif 	/* C style interface */




#endif 	/* __IVsDiscoveryService_INTERFACE_DEFINED__ */


/* interface __MIDL_itf_discoveryservice_0000_0001 */
/* [local] */ 

#define SID_SVsDiscoveryService IID_IVsDiscoveryService


extern RPC_IF_HANDLE __MIDL_itf_discoveryservice_0000_0001_v0_0_c_ifspec;
extern RPC_IF_HANDLE __MIDL_itf_discoveryservice_0000_0001_v0_0_s_ifspec;

#ifndef __IDiscoverUrlCallBack_INTERFACE_DEFINED__
#define __IDiscoverUrlCallBack_INTERFACE_DEFINED__

/* interface IDiscoverUrlCallBack */
/* [uuid][unique][object] */ 


EXTERN_C const IID IID_IDiscoverUrlCallBack;

#if defined(__cplusplus) && !defined(CINTERFACE)
    
    MIDL_INTERFACE("0EEA651C-B208-4ede-96CE-5194F4DC4E4A")
    IDiscoverUrlCallBack : public IUnknown
    {
    public:
        virtual HRESULT STDMETHODCALLTYPE NotifyDiscoverComplete( 
            /* [in] */ int cookie,
            /* [in] */ __RPC__in_opt IDiscoveryResult *pIDiscoveryResult) = 0;
        
    };
    
#else 	/* C style interface */

    typedef struct IDiscoverUrlCallBackVtbl
    {
        BEGIN_INTERFACE
        
        HRESULT ( STDMETHODCALLTYPE *QueryInterface )( 
            IDiscoverUrlCallBack * This,
            /* [in] */ __RPC__in REFIID riid,
            /* [iid_is][out] */ 
            __RPC__deref_out  void **ppvObject);
        
        ULONG ( STDMETHODCALLTYPE *AddRef )( 
            IDiscoverUrlCallBack * This);
        
        ULONG ( STDMETHODCALLTYPE *Release )( 
            IDiscoverUrlCallBack * This);
        
        HRESULT ( STDMETHODCALLTYPE *NotifyDiscoverComplete )( 
            IDiscoverUrlCallBack * This,
            /* [in] */ int cookie,
            /* [in] */ __RPC__in_opt IDiscoveryResult *pIDiscoveryResult);
        
        END_INTERFACE
    } IDiscoverUrlCallBackVtbl;

    interface IDiscoverUrlCallBack
    {
        CONST_VTBL struct IDiscoverUrlCallBackVtbl *lpVtbl;
    };

    

#ifdef COBJMACROS


#define IDiscoverUrlCallBack_QueryInterface(This,riid,ppvObject)	\
    ( (This)->lpVtbl -> QueryInterface(This,riid,ppvObject) ) 

#define IDiscoverUrlCallBack_AddRef(This)	\
    ( (This)->lpVtbl -> AddRef(This) ) 

#define IDiscoverUrlCallBack_Release(This)	\
    ( (This)->lpVtbl -> Release(This) ) 


#define IDiscoverUrlCallBack_NotifyDiscoverComplete(This,cookie,pIDiscoveryResult)	\
    ( (This)->lpVtbl -> NotifyDiscoverComplete(This,cookie,pIDiscoveryResult) ) 

#endif /* COBJMACROS */


#endif 	/* C style interface */




#endif 	/* __IDiscoverUrlCallBack_INTERFACE_DEFINED__ */


#ifndef __IDiscoveryResult_INTERFACE_DEFINED__
#define __IDiscoveryResult_INTERFACE_DEFINED__

/* interface IDiscoveryResult */
/* [uuid][unique][object] */ 


EXTERN_C const IID IID_IDiscoveryResult;

#if defined(__cplusplus) && !defined(CINTERFACE)
    
    MIDL_INTERFACE("B9A32C91-B14D-4ae3-A955-5CBC3E75FCA5")
    IDiscoveryResult : public IUnknown
    {
    public:
        virtual HRESULT STDMETHODCALLTYPE GetRawXml( 
            /* [retval][out] */ __RPC__deref_out_opt BSTR *pbstrXml) = 0;
        
        virtual HRESULT STDMETHODCALLTYPE GetDocumentXml( 
            /* [in] */ __RPC__in BSTR url,
            /* [retval][out] */ __RPC__deref_out_opt BSTR *pbstrXml) = 0;
        
        virtual HRESULT STDMETHODCALLTYPE GetReferenceCount( 
            /* [retval][out] */ __RPC__out int *pCount) = 0;
        
        virtual HRESULT STDMETHODCALLTYPE GetReferenceInfo( 
            /* [in] */ int pIndex,
            /* [retval][out] */ __RPC__deref_out_opt IReferenceInfo **ppIReferenceInfo) = 0;
        
        virtual HRESULT STDMETHODCALLTYPE GetDiscoverySession( 
            /* [retval][out] */ __RPC__deref_out_opt IDiscoverySession **discoverySession) = 0;
        
        virtual HRESULT STDMETHODCALLTYPE GetUrl( 
            /* [retval][out] */ __RPC__deref_out_opt BSTR *pbstrUrl) = 0;
        
        virtual HRESULT STDMETHODCALLTYPE AddWebReference( 
            /* [in] */ __RPC__in_opt IUnknown *punkWebReferenceFolder,
            /* [in] */ __RPC__in BSTR bstrDestinationPath) = 0;
        
        virtual HRESULT STDMETHODCALLTYPE AddWebReferenceTo( 
            /* [in] */ __RPC__in_opt IUnknown *punkWebReferenceFolder,
            /* [in] */ __RPC__in BSTR bstrDestinationPath,
            /* [in] */ __RPC__in BSTR bstrDiscomapFilename) = 0;
        
    };
    
#else 	/* C style interface */

    typedef struct IDiscoveryResultVtbl
    {
        BEGIN_INTERFACE
        
        HRESULT ( STDMETHODCALLTYPE *QueryInterface )( 
            IDiscoveryResult * This,
            /* [in] */ __RPC__in REFIID riid,
            /* [iid_is][out] */ 
            __RPC__deref_out  void **ppvObject);
        
        ULONG ( STDMETHODCALLTYPE *AddRef )( 
            IDiscoveryResult * This);
        
        ULONG ( STDMETHODCALLTYPE *Release )( 
            IDiscoveryResult * This);
        
        HRESULT ( STDMETHODCALLTYPE *GetRawXml )( 
            IDiscoveryResult * This,
            /* [retval][out] */ __RPC__deref_out_opt BSTR *pbstrXml);
        
        HRESULT ( STDMETHODCALLTYPE *GetDocumentXml )( 
            IDiscoveryResult * This,
            /* [in] */ __RPC__in BSTR url,
            /* [retval][out] */ __RPC__deref_out_opt BSTR *pbstrXml);
        
        HRESULT ( STDMETHODCALLTYPE *GetReferenceCount )( 
            IDiscoveryResult * This,
            /* [retval][out] */ __RPC__out int *pCount);
        
        HRESULT ( STDMETHODCALLTYPE *GetReferenceInfo )( 
            IDiscoveryResult * This,
            /* [in] */ int pIndex,
            /* [retval][out] */ __RPC__deref_out_opt IReferenceInfo **ppIReferenceInfo);
        
        HRESULT ( STDMETHODCALLTYPE *GetDiscoverySession )( 
            IDiscoveryResult * This,
            /* [retval][out] */ __RPC__deref_out_opt IDiscoverySession **discoverySession);
        
        HRESULT ( STDMETHODCALLTYPE *GetUrl )( 
            IDiscoveryResult * This,
            /* [retval][out] */ __RPC__deref_out_opt BSTR *pbstrUrl);
        
        HRESULT ( STDMETHODCALLTYPE *AddWebReference )( 
            IDiscoveryResult * This,
            /* [in] */ __RPC__in_opt IUnknown *punkWebReferenceFolder,
            /* [in] */ __RPC__in BSTR bstrDestinationPath);
        
        HRESULT ( STDMETHODCALLTYPE *AddWebReferenceTo )( 
            IDiscoveryResult * This,
            /* [in] */ __RPC__in_opt IUnknown *punkWebReferenceFolder,
            /* [in] */ __RPC__in BSTR bstrDestinationPath,
            /* [in] */ __RPC__in BSTR bstrDiscomapFilename);
        
        END_INTERFACE
    } IDiscoveryResultVtbl;

    interface IDiscoveryResult
    {
        CONST_VTBL struct IDiscoveryResultVtbl *lpVtbl;
    };

    

#ifdef COBJMACROS


#define IDiscoveryResult_QueryInterface(This,riid,ppvObject)	\
    ( (This)->lpVtbl -> QueryInterface(This,riid,ppvObject) ) 

#define IDiscoveryResult_AddRef(This)	\
    ( (This)->lpVtbl -> AddRef(This) ) 

#define IDiscoveryResult_Release(This)	\
    ( (This)->lpVtbl -> Release(This) ) 


#define IDiscoveryResult_GetRawXml(This,pbstrXml)	\
    ( (This)->lpVtbl -> GetRawXml(This,pbstrXml) ) 

#define IDiscoveryResult_GetDocumentXml(This,url,pbstrXml)	\
    ( (This)->lpVtbl -> GetDocumentXml(This,url,pbstrXml) ) 

#define IDiscoveryResult_GetReferenceCount(This,pCount)	\
    ( (This)->lpVtbl -> GetReferenceCount(This,pCount) ) 

#define IDiscoveryResult_GetReferenceInfo(This,pIndex,ppIReferenceInfo)	\
    ( (This)->lpVtbl -> GetReferenceInfo(This,pIndex,ppIReferenceInfo) ) 

#define IDiscoveryResult_GetDiscoverySession(This,discoverySession)	\
    ( (This)->lpVtbl -> GetDiscoverySession(This,discoverySession) ) 

#define IDiscoveryResult_GetUrl(This,pbstrUrl)	\
    ( (This)->lpVtbl -> GetUrl(This,pbstrUrl) ) 

#define IDiscoveryResult_AddWebReference(This,punkWebReferenceFolder,bstrDestinationPath)	\
    ( (This)->lpVtbl -> AddWebReference(This,punkWebReferenceFolder,bstrDestinationPath) ) 

#define IDiscoveryResult_AddWebReferenceTo(This,punkWebReferenceFolder,bstrDestinationPath,bstrDiscomapFilename)	\
    ( (This)->lpVtbl -> AddWebReferenceTo(This,punkWebReferenceFolder,bstrDestinationPath,bstrDiscomapFilename) ) 

#endif /* COBJMACROS */


#endif 	/* C style interface */




#endif 	/* __IDiscoveryResult_INTERFACE_DEFINED__ */


#ifndef __IReferenceInfo_INTERFACE_DEFINED__
#define __IReferenceInfo_INTERFACE_DEFINED__

/* interface IReferenceInfo */
/* [uuid][unique][object] */ 


EXTERN_C const IID IID_IReferenceInfo;

#if defined(__cplusplus) && !defined(CINTERFACE)
    
    MIDL_INTERFACE("B9A32C92-B14D-4ae3-A955-5CBC3E75FCA5")
    IReferenceInfo : public IUnknown
    {
    public:
        virtual HRESULT STDMETHODCALLTYPE GetNodeType( 
            /* [retval][out] */ __RPC__out DiscoveryNodeType *pType) = 0;
        
        virtual HRESULT STDMETHODCALLTYPE GetUrl( 
            /* [retval][out] */ __RPC__deref_out_opt BSTR *ppbstrUrl) = 0;
        
    };
    
#else 	/* C style interface */

    typedef struct IReferenceInfoVtbl
    {
        BEGIN_INTERFACE
        
        HRESULT ( STDMETHODCALLTYPE *QueryInterface )( 
            IReferenceInfo * This,
            /* [in] */ __RPC__in REFIID riid,
            /* [iid_is][out] */ 
            __RPC__deref_out  void **ppvObject);
        
        ULONG ( STDMETHODCALLTYPE *AddRef )( 
            IReferenceInfo * This);
        
        ULONG ( STDMETHODCALLTYPE *Release )( 
            IReferenceInfo * This);
        
        HRESULT ( STDMETHODCALLTYPE *GetNodeType )( 
            IReferenceInfo * This,
            /* [retval][out] */ __RPC__out DiscoveryNodeType *pType);
        
        HRESULT ( STDMETHODCALLTYPE *GetUrl )( 
            IReferenceInfo * This,
            /* [retval][out] */ __RPC__deref_out_opt BSTR *ppbstrUrl);
        
        END_INTERFACE
    } IReferenceInfoVtbl;

    interface IReferenceInfo
    {
        CONST_VTBL struct IReferenceInfoVtbl *lpVtbl;
    };

    

#ifdef COBJMACROS


#define IReferenceInfo_QueryInterface(This,riid,ppvObject)	\
    ( (This)->lpVtbl -> QueryInterface(This,riid,ppvObject) ) 

#define IReferenceInfo_AddRef(This)	\
    ( (This)->lpVtbl -> AddRef(This) ) 

#define IReferenceInfo_Release(This)	\
    ( (This)->lpVtbl -> Release(This) ) 


#define IReferenceInfo_GetNodeType(This,pType)	\
    ( (This)->lpVtbl -> GetNodeType(This,pType) ) 

#define IReferenceInfo_GetUrl(This,ppbstrUrl)	\
    ( (This)->lpVtbl -> GetUrl(This,ppbstrUrl) ) 

#endif /* COBJMACROS */


#endif 	/* C style interface */




#endif 	/* __IReferenceInfo_INTERFACE_DEFINED__ */


#ifndef __ISchemaReferenceInfo_INTERFACE_DEFINED__
#define __ISchemaReferenceInfo_INTERFACE_DEFINED__

/* interface ISchemaReferenceInfo */
/* [uuid][unique][object] */ 


EXTERN_C const IID IID_ISchemaReferenceInfo;

#if defined(__cplusplus) && !defined(CINTERFACE)
    
    MIDL_INTERFACE("B9A32C92-B14D-4ae3-A955-5CBC3E75FCA8")
    ISchemaReferenceInfo : public IUnknown
    {
    public:
        virtual HRESULT STDMETHODCALLTYPE GetTargetNamespace( 
            /* [retval][out] */ __RPC__deref_out_opt BSTR *pbstrRef) = 0;
        
    };
    
#else 	/* C style interface */

    typedef struct ISchemaReferenceInfoVtbl
    {
        BEGIN_INTERFACE
        
        HRESULT ( STDMETHODCALLTYPE *QueryInterface )( 
            ISchemaReferenceInfo * This,
            /* [in] */ __RPC__in REFIID riid,
            /* [iid_is][out] */ 
            __RPC__deref_out  void **ppvObject);
        
        ULONG ( STDMETHODCALLTYPE *AddRef )( 
            ISchemaReferenceInfo * This);
        
        ULONG ( STDMETHODCALLTYPE *Release )( 
            ISchemaReferenceInfo * This);
        
        HRESULT ( STDMETHODCALLTYPE *GetTargetNamespace )( 
            ISchemaReferenceInfo * This,
            /* [retval][out] */ __RPC__deref_out_opt BSTR *pbstrRef);
        
        END_INTERFACE
    } ISchemaReferenceInfoVtbl;

    interface ISchemaReferenceInfo
    {
        CONST_VTBL struct ISchemaReferenceInfoVtbl *lpVtbl;
    };

    

#ifdef COBJMACROS


#define ISchemaReferenceInfo_QueryInterface(This,riid,ppvObject)	\
    ( (This)->lpVtbl -> QueryInterface(This,riid,ppvObject) ) 

#define ISchemaReferenceInfo_AddRef(This)	\
    ( (This)->lpVtbl -> AddRef(This) ) 

#define ISchemaReferenceInfo_Release(This)	\
    ( (This)->lpVtbl -> Release(This) ) 


#define ISchemaReferenceInfo_GetTargetNamespace(This,pbstrRef)	\
    ( (This)->lpVtbl -> GetTargetNamespace(This,pbstrRef) ) 

#endif /* COBJMACROS */


#endif 	/* C style interface */




#endif 	/* __ISchemaReferenceInfo_INTERFACE_DEFINED__ */


#ifndef __IDiscoverySession_INTERFACE_DEFINED__
#define __IDiscoverySession_INTERFACE_DEFINED__

/* interface IDiscoverySession */
/* [uuid][unique][object] */ 


EXTERN_C const IID IID_IDiscoverySession;

#if defined(__cplusplus) && !defined(CINTERFACE)
    
    MIDL_INTERFACE("D622FE99-2087-4d78-8B49-7B46460475FD")
    IDiscoverySession : public IUnknown
    {
    public:
        virtual HRESULT STDMETHODCALLTYPE DiscoverUrl( 
            /* [in] */ __RPC__in BSTR pbstrUrl,
            /* [retval][out] */ __RPC__deref_out_opt IDiscoveryResult **pIDiscoveryResult) = 0;
        
        virtual HRESULT STDMETHODCALLTYPE DiscoverUrlAsync( 
            /* [in] */ __RPC__in BSTR url,
            /* [in] */ __RPC__in_opt IDiscoverUrlCallBack *pDiscoverUrlCallBack,
            /* [retval][out] */ __RPC__out int *cookie) = 0;
        
        virtual HRESULT STDMETHODCALLTYPE CancelDiscoverUrl( 
            /* [in] */ int cookie) = 0;
        
        virtual HRESULT STDMETHODCALLTYPE GetDiscoverError( 
            /* [in] */ int cookie) = 0;
        
        virtual HRESULT STDMETHODCALLTYPE UpdateWebReference( 
            /* [in] */ __RPC__in_opt IUnknown *punkWebReferenceFolder,
            /* [in] */ __RPC__in BSTR bstrUrl,
            /* [in] */ __RPC__in BSTR bstrDestinationPath) = 0;
        
    };
    
#else 	/* C style interface */

    typedef struct IDiscoverySessionVtbl
    {
        BEGIN_INTERFACE
        
        HRESULT ( STDMETHODCALLTYPE *QueryInterface )( 
            IDiscoverySession * This,
            /* [in] */ __RPC__in REFIID riid,
            /* [iid_is][out] */ 
            __RPC__deref_out  void **ppvObject);
        
        ULONG ( STDMETHODCALLTYPE *AddRef )( 
            IDiscoverySession * This);
        
        ULONG ( STDMETHODCALLTYPE *Release )( 
            IDiscoverySession * This);
        
        HRESULT ( STDMETHODCALLTYPE *DiscoverUrl )( 
            IDiscoverySession * This,
            /* [in] */ __RPC__in BSTR pbstrUrl,
            /* [retval][out] */ __RPC__deref_out_opt IDiscoveryResult **pIDiscoveryResult);
        
        HRESULT ( STDMETHODCALLTYPE *DiscoverUrlAsync )( 
            IDiscoverySession * This,
            /* [in] */ __RPC__in BSTR url,
            /* [in] */ __RPC__in_opt IDiscoverUrlCallBack *pDiscoverUrlCallBack,
            /* [retval][out] */ __RPC__out int *cookie);
        
        HRESULT ( STDMETHODCALLTYPE *CancelDiscoverUrl )( 
            IDiscoverySession * This,
            /* [in] */ int cookie);
        
        HRESULT ( STDMETHODCALLTYPE *GetDiscoverError )( 
            IDiscoverySession * This,
            /* [in] */ int cookie);
        
        HRESULT ( STDMETHODCALLTYPE *UpdateWebReference )( 
            IDiscoverySession * This,
            /* [in] */ __RPC__in_opt IUnknown *punkWebReferenceFolder,
            /* [in] */ __RPC__in BSTR bstrUrl,
            /* [in] */ __RPC__in BSTR bstrDestinationPath);
        
        END_INTERFACE
    } IDiscoverySessionVtbl;

    interface IDiscoverySession
    {
        CONST_VTBL struct IDiscoverySessionVtbl *lpVtbl;
    };

    

#ifdef COBJMACROS


#define IDiscoverySession_QueryInterface(This,riid,ppvObject)	\
    ( (This)->lpVtbl -> QueryInterface(This,riid,ppvObject) ) 

#define IDiscoverySession_AddRef(This)	\
    ( (This)->lpVtbl -> AddRef(This) ) 

#define IDiscoverySession_Release(This)	\
    ( (This)->lpVtbl -> Release(This) ) 


#define IDiscoverySession_DiscoverUrl(This,pbstrUrl,pIDiscoveryResult)	\
    ( (This)->lpVtbl -> DiscoverUrl(This,pbstrUrl,pIDiscoveryResult) ) 

#define IDiscoverySession_DiscoverUrlAsync(This,url,pDiscoverUrlCallBack,cookie)	\
    ( (This)->lpVtbl -> DiscoverUrlAsync(This,url,pDiscoverUrlCallBack,cookie) ) 

#define IDiscoverySession_CancelDiscoverUrl(This,cookie)	\
    ( (This)->lpVtbl -> CancelDiscoverUrl(This,cookie) ) 

#define IDiscoverySession_GetDiscoverError(This,cookie)	\
    ( (This)->lpVtbl -> GetDiscoverError(This,cookie) ) 

#define IDiscoverySession_UpdateWebReference(This,punkWebReferenceFolder,bstrUrl,bstrDestinationPath)	\
    ( (This)->lpVtbl -> UpdateWebReference(This,punkWebReferenceFolder,bstrUrl,bstrDestinationPath) ) 

#endif /* COBJMACROS */


#endif 	/* C style interface */




#endif 	/* __IDiscoverySession_INTERFACE_DEFINED__ */


/* Additional Prototypes for ALL interfaces */

unsigned long             __RPC_USER  BSTR_UserSize(     unsigned long *, unsigned long            , BSTR * ); 
unsigned char * __RPC_USER  BSTR_UserMarshal(  unsigned long *, unsigned char *, BSTR * ); 
unsigned char * __RPC_USER  BSTR_UserUnmarshal(unsigned long *, unsigned char *, BSTR * ); 
void                      __RPC_USER  BSTR_UserFree(     unsigned long *, BSTR * ); 

/* end of Additional Prototypes */

#ifdef __cplusplus
}
#endif

#endif


