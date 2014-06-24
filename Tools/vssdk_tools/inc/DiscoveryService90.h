

/* this ALWAYS GENERATED file contains the definitions for the interfaces */


 /* File created by MIDL compiler version 7.00.0499 */
/* Compiler settings for discoveryservice90.idl:
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

#ifndef __discoveryservice90_h__
#define __discoveryservice90_h__

#if defined(_MSC_VER) && (_MSC_VER >= 1020)
#pragma once
#endif

/* Forward Declarations */ 

#ifndef __IDiscoveryResult3_FWD_DEFINED__
#define __IDiscoveryResult3_FWD_DEFINED__
typedef interface IDiscoveryResult3 IDiscoveryResult3;
#endif 	/* __IDiscoveryResult3_FWD_DEFINED__ */


#ifndef __IDiscoverySession2_FWD_DEFINED__
#define __IDiscoverySession2_FWD_DEFINED__
typedef interface IDiscoverySession2 IDiscoverySession2;
#endif 	/* __IDiscoverySession2_FWD_DEFINED__ */


#ifndef __IReferenceInfo2_FWD_DEFINED__
#define __IReferenceInfo2_FWD_DEFINED__
typedef interface IReferenceInfo2 IReferenceInfo2;
#endif 	/* __IReferenceInfo2_FWD_DEFINED__ */


/* header files for imported files */
#include "oaidl.h"
#include "ocidl.h"
#include "oleipc.h"
#include "vsshell.h"
#include "discoveryservice80.h"

#ifdef __cplusplus
extern "C"{
#endif 


/* interface __MIDL_itf_discoveryservice90_0000_0000 */
/* [local] */ 





enum DiscoveryProtocol
    {	DP_DiscoveryClientProtocol	= 1,
	DP_MetadataExchangeProtocol	= 2,
	DP_DiscoveryDataServiceProtocol	= 3
    } ;
typedef enum DiscoveryProtocol DiscoveryProtocol;



extern RPC_IF_HANDLE __MIDL_itf_discoveryservice90_0000_0000_v0_0_c_ifspec;
extern RPC_IF_HANDLE __MIDL_itf_discoveryservice90_0000_0000_v0_0_s_ifspec;

#ifndef __IDiscoveryResult3_INTERFACE_DEFINED__
#define __IDiscoveryResult3_INTERFACE_DEFINED__

/* interface IDiscoveryResult3 */
/* [object][uuid][unique][version] */ 


EXTERN_C const IID IID_IDiscoveryResult3;

#if defined(__cplusplus) && !defined(CINTERFACE)
    
    MIDL_INTERFACE("6e1073e5-a112-441a-8386-232cecc0c29b")
    IDiscoveryResult3 : public IUnknown
    {
    public:
        virtual HRESULT STDMETHODCALLTYPE GetProtocol( 
            /* [retval][out] */ __RPC__out DiscoveryProtocol *pDiscoveryProtocol) = 0;
        
    };
    
#else 	/* C style interface */

    typedef struct IDiscoveryResult3Vtbl
    {
        BEGIN_INTERFACE
        
        HRESULT ( STDMETHODCALLTYPE *QueryInterface )( 
            IDiscoveryResult3 * This,
            /* [in] */ __RPC__in REFIID riid,
            /* [iid_is][out] */ 
            __RPC__deref_out  void **ppvObject);
        
        ULONG ( STDMETHODCALLTYPE *AddRef )( 
            IDiscoveryResult3 * This);
        
        ULONG ( STDMETHODCALLTYPE *Release )( 
            IDiscoveryResult3 * This);
        
        HRESULT ( STDMETHODCALLTYPE *GetProtocol )( 
            IDiscoveryResult3 * This,
            /* [retval][out] */ __RPC__out DiscoveryProtocol *pDiscoveryProtocol);
        
        END_INTERFACE
    } IDiscoveryResult3Vtbl;

    interface IDiscoveryResult3
    {
        CONST_VTBL struct IDiscoveryResult3Vtbl *lpVtbl;
    };

    

#ifdef COBJMACROS


#define IDiscoveryResult3_QueryInterface(This,riid,ppvObject)	\
    ( (This)->lpVtbl -> QueryInterface(This,riid,ppvObject) ) 

#define IDiscoveryResult3_AddRef(This)	\
    ( (This)->lpVtbl -> AddRef(This) ) 

#define IDiscoveryResult3_Release(This)	\
    ( (This)->lpVtbl -> Release(This) ) 


#define IDiscoveryResult3_GetProtocol(This,pDiscoveryProtocol)	\
    ( (This)->lpVtbl -> GetProtocol(This,pDiscoveryProtocol) ) 

#endif /* COBJMACROS */


#endif 	/* C style interface */




#endif 	/* __IDiscoveryResult3_INTERFACE_DEFINED__ */


/* interface __MIDL_itf_discoveryservice90_0000_0001 */
/* [local] */ 


enum DiscoverySessionProtocolPriority
    {	DSPP_NoPriority	= 0,
	DSPP_DiscoveryProtocolFirst	= 1,
	DSPP_MetadataExchangeFirst	= 2
    } ;
typedef enum DiscoverySessionProtocolPriority DiscoverySessionProtocolPriority;



extern RPC_IF_HANDLE __MIDL_itf_discoveryservice90_0000_0001_v0_0_c_ifspec;
extern RPC_IF_HANDLE __MIDL_itf_discoveryservice90_0000_0001_v0_0_s_ifspec;

#ifndef __IDiscoverySession2_INTERFACE_DEFINED__
#define __IDiscoverySession2_INTERFACE_DEFINED__

/* interface IDiscoverySession2 */
/* [object][uuid][unique][version] */ 


EXTERN_C const IID IID_IDiscoverySession2;

#if defined(__cplusplus) && !defined(CINTERFACE)
    
    MIDL_INTERFACE("182b5eb4-9a58-47b7-a3fe-ac7ed8b891ff")
    IDiscoverySession2 : public IUnknown
    {
    public:
        virtual HRESULT STDMETHODCALLTYPE DiscoverUrlWithMetadataExchange( 
            __RPC__in LPCOLESTR url,
            DiscoverySessionProtocolPriority protocolPriority,
            BOOL resolveAllFiles,
            __RPC__in LPCOLESTR toolConfigurationPath,
            /* [out] */ __RPC__deref_out_opt IDiscoveryResult **ppDiscoverResult) = 0;
        
        virtual HRESULT STDMETHODCALLTYPE DiscoverUrlAsyncWithMetadataExchange( 
            __RPC__in LPCOLESTR url,
            DiscoverySessionProtocolPriority protocolPriority,
            BOOL resolveAllFiles,
            __RPC__in LPCOLESTR toolConfigurationPath,
            __RPC__in_opt IDiscoverUrlCallBack *pDiscoverUrlCallBack,
            /* [out] */ __RPC__out int *pCookieID) = 0;
        
    };
    
#else 	/* C style interface */

    typedef struct IDiscoverySession2Vtbl
    {
        BEGIN_INTERFACE
        
        HRESULT ( STDMETHODCALLTYPE *QueryInterface )( 
            IDiscoverySession2 * This,
            /* [in] */ __RPC__in REFIID riid,
            /* [iid_is][out] */ 
            __RPC__deref_out  void **ppvObject);
        
        ULONG ( STDMETHODCALLTYPE *AddRef )( 
            IDiscoverySession2 * This);
        
        ULONG ( STDMETHODCALLTYPE *Release )( 
            IDiscoverySession2 * This);
        
        HRESULT ( STDMETHODCALLTYPE *DiscoverUrlWithMetadataExchange )( 
            IDiscoverySession2 * This,
            __RPC__in LPCOLESTR url,
            DiscoverySessionProtocolPriority protocolPriority,
            BOOL resolveAllFiles,
            __RPC__in LPCOLESTR toolConfigurationPath,
            /* [out] */ __RPC__deref_out_opt IDiscoveryResult **ppDiscoverResult);
        
        HRESULT ( STDMETHODCALLTYPE *DiscoverUrlAsyncWithMetadataExchange )( 
            IDiscoverySession2 * This,
            __RPC__in LPCOLESTR url,
            DiscoverySessionProtocolPriority protocolPriority,
            BOOL resolveAllFiles,
            __RPC__in LPCOLESTR toolConfigurationPath,
            __RPC__in_opt IDiscoverUrlCallBack *pDiscoverUrlCallBack,
            /* [out] */ __RPC__out int *pCookieID);
        
        END_INTERFACE
    } IDiscoverySession2Vtbl;

    interface IDiscoverySession2
    {
        CONST_VTBL struct IDiscoverySession2Vtbl *lpVtbl;
    };

    

#ifdef COBJMACROS


#define IDiscoverySession2_QueryInterface(This,riid,ppvObject)	\
    ( (This)->lpVtbl -> QueryInterface(This,riid,ppvObject) ) 

#define IDiscoverySession2_AddRef(This)	\
    ( (This)->lpVtbl -> AddRef(This) ) 

#define IDiscoverySession2_Release(This)	\
    ( (This)->lpVtbl -> Release(This) ) 


#define IDiscoverySession2_DiscoverUrlWithMetadataExchange(This,url,protocolPriority,resolveAllFiles,toolConfigurationPath,ppDiscoverResult)	\
    ( (This)->lpVtbl -> DiscoverUrlWithMetadataExchange(This,url,protocolPriority,resolveAllFiles,toolConfigurationPath,ppDiscoverResult) ) 

#define IDiscoverySession2_DiscoverUrlAsyncWithMetadataExchange(This,url,protocolPriority,resolveAllFiles,toolConfigurationPath,pDiscoverUrlCallBack,pCookieID)	\
    ( (This)->lpVtbl -> DiscoverUrlAsyncWithMetadataExchange(This,url,protocolPriority,resolveAllFiles,toolConfigurationPath,pDiscoverUrlCallBack,pCookieID) ) 

#endif /* COBJMACROS */


#endif 	/* C style interface */




#endif 	/* __IDiscoverySession2_INTERFACE_DEFINED__ */


#ifndef __IReferenceInfo2_INTERFACE_DEFINED__
#define __IReferenceInfo2_INTERFACE_DEFINED__

/* interface IReferenceInfo2 */
/* [object][uuid][unique][version] */ 


EXTERN_C const IID IID_IReferenceInfo2;

#if defined(__cplusplus) && !defined(CINTERFACE)
    
    MIDL_INTERFACE("e316ff6d-ca00-49dd-969f-441399132ac7")
    IReferenceInfo2 : public IUnknown
    {
    public:
        virtual HRESULT STDMETHODCALLTYPE GetDefaultFileName( 
            /* [retval][out] */ __RPC__deref_out_opt BSTR *pbstrDefaultName) = 0;
        
        virtual HRESULT STDMETHODCALLTYPE GetDocumentContents( 
            /* [retval][out] */ __RPC__deref_out_opt SAFEARRAY * *pContent) = 0;
        
    };
    
#else 	/* C style interface */

    typedef struct IReferenceInfo2Vtbl
    {
        BEGIN_INTERFACE
        
        HRESULT ( STDMETHODCALLTYPE *QueryInterface )( 
            IReferenceInfo2 * This,
            /* [in] */ __RPC__in REFIID riid,
            /* [iid_is][out] */ 
            __RPC__deref_out  void **ppvObject);
        
        ULONG ( STDMETHODCALLTYPE *AddRef )( 
            IReferenceInfo2 * This);
        
        ULONG ( STDMETHODCALLTYPE *Release )( 
            IReferenceInfo2 * This);
        
        HRESULT ( STDMETHODCALLTYPE *GetDefaultFileName )( 
            IReferenceInfo2 * This,
            /* [retval][out] */ __RPC__deref_out_opt BSTR *pbstrDefaultName);
        
        HRESULT ( STDMETHODCALLTYPE *GetDocumentContents )( 
            IReferenceInfo2 * This,
            /* [retval][out] */ __RPC__deref_out_opt SAFEARRAY * *pContent);
        
        END_INTERFACE
    } IReferenceInfo2Vtbl;

    interface IReferenceInfo2
    {
        CONST_VTBL struct IReferenceInfo2Vtbl *lpVtbl;
    };

    

#ifdef COBJMACROS


#define IReferenceInfo2_QueryInterface(This,riid,ppvObject)	\
    ( (This)->lpVtbl -> QueryInterface(This,riid,ppvObject) ) 

#define IReferenceInfo2_AddRef(This)	\
    ( (This)->lpVtbl -> AddRef(This) ) 

#define IReferenceInfo2_Release(This)	\
    ( (This)->lpVtbl -> Release(This) ) 


#define IReferenceInfo2_GetDefaultFileName(This,pbstrDefaultName)	\
    ( (This)->lpVtbl -> GetDefaultFileName(This,pbstrDefaultName) ) 

#define IReferenceInfo2_GetDocumentContents(This,pContent)	\
    ( (This)->lpVtbl -> GetDocumentContents(This,pContent) ) 

#endif /* COBJMACROS */


#endif 	/* C style interface */




#endif 	/* __IReferenceInfo2_INTERFACE_DEFINED__ */


/* Additional Prototypes for ALL interfaces */

unsigned long             __RPC_USER  BSTR_UserSize(     unsigned long *, unsigned long            , BSTR * ); 
unsigned char * __RPC_USER  BSTR_UserMarshal(  unsigned long *, unsigned char *, BSTR * ); 
unsigned char * __RPC_USER  BSTR_UserUnmarshal(unsigned long *, unsigned char *, BSTR * ); 
void                      __RPC_USER  BSTR_UserFree(     unsigned long *, BSTR * ); 

unsigned long             __RPC_USER  LPSAFEARRAY_UserSize(     unsigned long *, unsigned long            , LPSAFEARRAY * ); 
unsigned char * __RPC_USER  LPSAFEARRAY_UserMarshal(  unsigned long *, unsigned char *, LPSAFEARRAY * ); 
unsigned char * __RPC_USER  LPSAFEARRAY_UserUnmarshal(unsigned long *, unsigned char *, LPSAFEARRAY * ); 
void                      __RPC_USER  LPSAFEARRAY_UserFree(     unsigned long *, LPSAFEARRAY * ); 

/* end of Additional Prototypes */

#ifdef __cplusplus
}
#endif

#endif


