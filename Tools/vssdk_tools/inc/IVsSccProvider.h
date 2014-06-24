

/* this ALWAYS GENERATED file contains the definitions for the interfaces */


 /* File created by MIDL compiler version 7.00.0499 */
/* Compiler settings for IVsSccProvider.idl:
    Oicf, W1, Zp8, env=Win32 (32b run)
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

#ifndef __IVsSccProvider_h__
#define __IVsSccProvider_h__

#if defined(_MSC_VER) && (_MSC_VER >= 1020)
#pragma once
#endif

/* Forward Declarations */ 

#ifndef __IVsSccProvider_FWD_DEFINED__
#define __IVsSccProvider_FWD_DEFINED__
typedef interface IVsSccProvider IVsSccProvider;
#endif 	/* __IVsSccProvider_FWD_DEFINED__ */


/* header files for imported files */
#include "oaidl.h"

#ifdef __cplusplus
extern "C"{
#endif 


/* interface __MIDL_itf_IVsSccProvider_0000_0000 */
/* [local] */ 

#pragma once


extern RPC_IF_HANDLE __MIDL_itf_IVsSccProvider_0000_0000_v0_0_c_ifspec;
extern RPC_IF_HANDLE __MIDL_itf_IVsSccProvider_0000_0000_v0_0_s_ifspec;

#ifndef __IVsSccProvider_INTERFACE_DEFINED__
#define __IVsSccProvider_INTERFACE_DEFINED__

/* interface IVsSccProvider */
/* [object][uuid] */ 


EXTERN_C const IID IID_IVsSccProvider;

#if defined(__cplusplus) && !defined(CINTERFACE)
    
    MIDL_INTERFACE("49440575-E33C-4169-9735-F3FD5AE54D8D")
    IVsSccProvider : public IUnknown
    {
    public:
        virtual HRESULT STDMETHODCALLTYPE SetActive( void) = 0;
        
        virtual HRESULT STDMETHODCALLTYPE SetInactive( void) = 0;
        
        virtual HRESULT STDMETHODCALLTYPE AnyItemsUnderSourceControl( 
            /* [out] */ __RPC__out BOOL *pfResult) = 0;
        
    };
    
#else 	/* C style interface */

    typedef struct IVsSccProviderVtbl
    {
        BEGIN_INTERFACE
        
        HRESULT ( STDMETHODCALLTYPE *QueryInterface )( 
            IVsSccProvider * This,
            /* [in] */ __RPC__in REFIID riid,
            /* [iid_is][out] */ 
            __RPC__deref_out  void **ppvObject);
        
        ULONG ( STDMETHODCALLTYPE *AddRef )( 
            IVsSccProvider * This);
        
        ULONG ( STDMETHODCALLTYPE *Release )( 
            IVsSccProvider * This);
        
        HRESULT ( STDMETHODCALLTYPE *SetActive )( 
            IVsSccProvider * This);
        
        HRESULT ( STDMETHODCALLTYPE *SetInactive )( 
            IVsSccProvider * This);
        
        HRESULT ( STDMETHODCALLTYPE *AnyItemsUnderSourceControl )( 
            IVsSccProvider * This,
            /* [out] */ __RPC__out BOOL *pfResult);
        
        END_INTERFACE
    } IVsSccProviderVtbl;

    interface IVsSccProvider
    {
        CONST_VTBL struct IVsSccProviderVtbl *lpVtbl;
    };

    

#ifdef COBJMACROS


#define IVsSccProvider_QueryInterface(This,riid,ppvObject)	\
    ( (This)->lpVtbl -> QueryInterface(This,riid,ppvObject) ) 

#define IVsSccProvider_AddRef(This)	\
    ( (This)->lpVtbl -> AddRef(This) ) 

#define IVsSccProvider_Release(This)	\
    ( (This)->lpVtbl -> Release(This) ) 


#define IVsSccProvider_SetActive(This)	\
    ( (This)->lpVtbl -> SetActive(This) ) 

#define IVsSccProvider_SetInactive(This)	\
    ( (This)->lpVtbl -> SetInactive(This) ) 

#define IVsSccProvider_AnyItemsUnderSourceControl(This,pfResult)	\
    ( (This)->lpVtbl -> AnyItemsUnderSourceControl(This,pfResult) ) 

#endif /* COBJMACROS */


#endif 	/* C style interface */




#endif 	/* __IVsSccProvider_INTERFACE_DEFINED__ */


/* Additional Prototypes for ALL interfaces */

/* end of Additional Prototypes */

#ifdef __cplusplus
}
#endif

#endif


