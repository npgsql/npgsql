

/* this ALWAYS GENERATED file contains the definitions for the interfaces */


 /* File created by MIDL compiler version 7.00.0499 */
/* Compiler settings for discoveryservice80.idl:
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

#ifndef __discoveryservice80_h__
#define __discoveryservice80_h__

#if defined(_MSC_VER) && (_MSC_VER >= 1020)
#pragma once
#endif

/* Forward Declarations */ 

#ifndef __IDiscoveryResult2_FWD_DEFINED__
#define __IDiscoveryResult2_FWD_DEFINED__
typedef interface IDiscoveryResult2 IDiscoveryResult2;
#endif 	/* __IDiscoveryResult2_FWD_DEFINED__ */


#ifndef __IDiscoveryClientResult_FWD_DEFINED__
#define __IDiscoveryClientResult_FWD_DEFINED__
typedef interface IDiscoveryClientResult IDiscoveryClientResult;
#endif 	/* __IDiscoveryClientResult_FWD_DEFINED__ */


#ifndef __IDiscoveryClientResultCollection_FWD_DEFINED__
#define __IDiscoveryClientResultCollection_FWD_DEFINED__
typedef interface IDiscoveryClientResultCollection IDiscoveryClientResultCollection;
#endif 	/* __IDiscoveryClientResultCollection_FWD_DEFINED__ */


/* header files for imported files */
#include "ocidl.h"
#include "discoveryservice.h"

#ifdef __cplusplus
extern "C"{
#endif 


/* interface __MIDL_itf_discoveryservice80_0000_0000 */
/* [local] */ 






extern RPC_IF_HANDLE __MIDL_itf_discoveryservice80_0000_0000_v0_0_c_ifspec;
extern RPC_IF_HANDLE __MIDL_itf_discoveryservice80_0000_0000_v0_0_s_ifspec;

#ifndef __IDiscoveryResult2_INTERFACE_DEFINED__
#define __IDiscoveryResult2_INTERFACE_DEFINED__

/* interface IDiscoveryResult2 */
/* [uuid][unique][object] */ 


EXTERN_C const IID IID_IDiscoveryResult2;

#if defined(__cplusplus) && !defined(CINTERFACE)
    
    MIDL_INTERFACE("9947783D-B173-4fd9-8E6E-91482CF9E239")
    IDiscoveryResult2 : public IUnknown
    {
    public:
        virtual HRESULT STDMETHODCALLTYPE DownloadServiceDocument( 
            /* [in] */ __RPC__in BSTR bstrDestinationPath,
            /* [in] */ __RPC__in BSTR bstrDiscomapFileName,
            /* [retval][out] */ __RPC__deref_out_opt IDiscoveryClientResultCollection **ppIDiscoveryClientResultCollection) = 0;
        
    };
    
#else 	/* C style interface */

    typedef struct IDiscoveryResult2Vtbl
    {
        BEGIN_INTERFACE
        
        HRESULT ( STDMETHODCALLTYPE *QueryInterface )( 
            IDiscoveryResult2 * This,
            /* [in] */ __RPC__in REFIID riid,
            /* [iid_is][out] */ 
            __RPC__deref_out  void **ppvObject);
        
        ULONG ( STDMETHODCALLTYPE *AddRef )( 
            IDiscoveryResult2 * This);
        
        ULONG ( STDMETHODCALLTYPE *Release )( 
            IDiscoveryResult2 * This);
        
        HRESULT ( STDMETHODCALLTYPE *DownloadServiceDocument )( 
            IDiscoveryResult2 * This,
            /* [in] */ __RPC__in BSTR bstrDestinationPath,
            /* [in] */ __RPC__in BSTR bstrDiscomapFileName,
            /* [retval][out] */ __RPC__deref_out_opt IDiscoveryClientResultCollection **ppIDiscoveryClientResultCollection);
        
        END_INTERFACE
    } IDiscoveryResult2Vtbl;

    interface IDiscoveryResult2
    {
        CONST_VTBL struct IDiscoveryResult2Vtbl *lpVtbl;
    };

    

#ifdef COBJMACROS


#define IDiscoveryResult2_QueryInterface(This,riid,ppvObject)	\
    ( (This)->lpVtbl -> QueryInterface(This,riid,ppvObject) ) 

#define IDiscoveryResult2_AddRef(This)	\
    ( (This)->lpVtbl -> AddRef(This) ) 

#define IDiscoveryResult2_Release(This)	\
    ( (This)->lpVtbl -> Release(This) ) 


#define IDiscoveryResult2_DownloadServiceDocument(This,bstrDestinationPath,bstrDiscomapFileName,ppIDiscoveryClientResultCollection)	\
    ( (This)->lpVtbl -> DownloadServiceDocument(This,bstrDestinationPath,bstrDiscomapFileName,ppIDiscoveryClientResultCollection) ) 

#endif /* COBJMACROS */


#endif 	/* C style interface */




#endif 	/* __IDiscoveryResult2_INTERFACE_DEFINED__ */


#ifndef __IDiscoveryClientResult_INTERFACE_DEFINED__
#define __IDiscoveryClientResult_INTERFACE_DEFINED__

/* interface IDiscoveryClientResult */
/* [uuid][unique][object] */ 


EXTERN_C const IID IID_IDiscoveryClientResult;

#if defined(__cplusplus) && !defined(CINTERFACE)
    
    MIDL_INTERFACE("FC5E8B09-FF17-4da6-BA39-EB8DFC314BC1")
    IDiscoveryClientResult : public IUnknown
    {
    public:
        virtual HRESULT STDMETHODCALLTYPE GetFileName( 
            /* [retval][out] */ __RPC__deref_out_opt BSTR *pbstrFilename) = 0;
        
        virtual HRESULT STDMETHODCALLTYPE GetReferenceTypeName( 
            /* [retval][out] */ __RPC__deref_out_opt BSTR *pbstrReferenceTypeName) = 0;
        
        virtual HRESULT STDMETHODCALLTYPE GetUrl( 
            /* [retval][out] */ __RPC__deref_out_opt BSTR *pbstrUrl) = 0;
        
    };
    
#else 	/* C style interface */

    typedef struct IDiscoveryClientResultVtbl
    {
        BEGIN_INTERFACE
        
        HRESULT ( STDMETHODCALLTYPE *QueryInterface )( 
            IDiscoveryClientResult * This,
            /* [in] */ __RPC__in REFIID riid,
            /* [iid_is][out] */ 
            __RPC__deref_out  void **ppvObject);
        
        ULONG ( STDMETHODCALLTYPE *AddRef )( 
            IDiscoveryClientResult * This);
        
        ULONG ( STDMETHODCALLTYPE *Release )( 
            IDiscoveryClientResult * This);
        
        HRESULT ( STDMETHODCALLTYPE *GetFileName )( 
            IDiscoveryClientResult * This,
            /* [retval][out] */ __RPC__deref_out_opt BSTR *pbstrFilename);
        
        HRESULT ( STDMETHODCALLTYPE *GetReferenceTypeName )( 
            IDiscoveryClientResult * This,
            /* [retval][out] */ __RPC__deref_out_opt BSTR *pbstrReferenceTypeName);
        
        HRESULT ( STDMETHODCALLTYPE *GetUrl )( 
            IDiscoveryClientResult * This,
            /* [retval][out] */ __RPC__deref_out_opt BSTR *pbstrUrl);
        
        END_INTERFACE
    } IDiscoveryClientResultVtbl;

    interface IDiscoveryClientResult
    {
        CONST_VTBL struct IDiscoveryClientResultVtbl *lpVtbl;
    };

    

#ifdef COBJMACROS


#define IDiscoveryClientResult_QueryInterface(This,riid,ppvObject)	\
    ( (This)->lpVtbl -> QueryInterface(This,riid,ppvObject) ) 

#define IDiscoveryClientResult_AddRef(This)	\
    ( (This)->lpVtbl -> AddRef(This) ) 

#define IDiscoveryClientResult_Release(This)	\
    ( (This)->lpVtbl -> Release(This) ) 


#define IDiscoveryClientResult_GetFileName(This,pbstrFilename)	\
    ( (This)->lpVtbl -> GetFileName(This,pbstrFilename) ) 

#define IDiscoveryClientResult_GetReferenceTypeName(This,pbstrReferenceTypeName)	\
    ( (This)->lpVtbl -> GetReferenceTypeName(This,pbstrReferenceTypeName) ) 

#define IDiscoveryClientResult_GetUrl(This,pbstrUrl)	\
    ( (This)->lpVtbl -> GetUrl(This,pbstrUrl) ) 

#endif /* COBJMACROS */


#endif 	/* C style interface */




#endif 	/* __IDiscoveryClientResult_INTERFACE_DEFINED__ */


#ifndef __IDiscoveryClientResultCollection_INTERFACE_DEFINED__
#define __IDiscoveryClientResultCollection_INTERFACE_DEFINED__

/* interface IDiscoveryClientResultCollection */
/* [uuid][unique][object] */ 


EXTERN_C const IID IID_IDiscoveryClientResultCollection;

#if defined(__cplusplus) && !defined(CINTERFACE)
    
    MIDL_INTERFACE("BCEB2C6C-E0D1-4fa5-8DDC-977D68E8D50B")
    IDiscoveryClientResultCollection : public IUnknown
    {
    public:
        virtual HRESULT STDMETHODCALLTYPE GetResultCount( 
            /* [retval][out] */ __RPC__out int *pCount) = 0;
        
        virtual HRESULT STDMETHODCALLTYPE GetResult( 
            /* [in] */ int pIndex,
            /* [retval][out] */ __RPC__deref_out_opt IDiscoveryClientResult **ppIDiscoveryClientResult) = 0;
        
    };
    
#else 	/* C style interface */

    typedef struct IDiscoveryClientResultCollectionVtbl
    {
        BEGIN_INTERFACE
        
        HRESULT ( STDMETHODCALLTYPE *QueryInterface )( 
            IDiscoveryClientResultCollection * This,
            /* [in] */ __RPC__in REFIID riid,
            /* [iid_is][out] */ 
            __RPC__deref_out  void **ppvObject);
        
        ULONG ( STDMETHODCALLTYPE *AddRef )( 
            IDiscoveryClientResultCollection * This);
        
        ULONG ( STDMETHODCALLTYPE *Release )( 
            IDiscoveryClientResultCollection * This);
        
        HRESULT ( STDMETHODCALLTYPE *GetResultCount )( 
            IDiscoveryClientResultCollection * This,
            /* [retval][out] */ __RPC__out int *pCount);
        
        HRESULT ( STDMETHODCALLTYPE *GetResult )( 
            IDiscoveryClientResultCollection * This,
            /* [in] */ int pIndex,
            /* [retval][out] */ __RPC__deref_out_opt IDiscoveryClientResult **ppIDiscoveryClientResult);
        
        END_INTERFACE
    } IDiscoveryClientResultCollectionVtbl;

    interface IDiscoveryClientResultCollection
    {
        CONST_VTBL struct IDiscoveryClientResultCollectionVtbl *lpVtbl;
    };

    

#ifdef COBJMACROS


#define IDiscoveryClientResultCollection_QueryInterface(This,riid,ppvObject)	\
    ( (This)->lpVtbl -> QueryInterface(This,riid,ppvObject) ) 

#define IDiscoveryClientResultCollection_AddRef(This)	\
    ( (This)->lpVtbl -> AddRef(This) ) 

#define IDiscoveryClientResultCollection_Release(This)	\
    ( (This)->lpVtbl -> Release(This) ) 


#define IDiscoveryClientResultCollection_GetResultCount(This,pCount)	\
    ( (This)->lpVtbl -> GetResultCount(This,pCount) ) 

#define IDiscoveryClientResultCollection_GetResult(This,pIndex,ppIDiscoveryClientResult)	\
    ( (This)->lpVtbl -> GetResult(This,pIndex,ppIDiscoveryClientResult) ) 

#endif /* COBJMACROS */


#endif 	/* C style interface */




#endif 	/* __IDiscoveryClientResultCollection_INTERFACE_DEFINED__ */


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


