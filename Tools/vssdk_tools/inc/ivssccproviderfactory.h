

/* this ALWAYS GENERATED file contains the definitions for the interfaces */


 /* File created by MIDL compiler version 7.00.0499 */
/* Compiler settings for IVsSccProviderFactory.idl:
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

#ifndef __IVsSccProviderFactory_h__
#define __IVsSccProviderFactory_h__

#if defined(_MSC_VER) && (_MSC_VER >= 1020)
#pragma once
#endif

/* Forward Declarations */ 

#ifndef __IVsSccProviderFactory_FWD_DEFINED__
#define __IVsSccProviderFactory_FWD_DEFINED__
typedef interface IVsSccProviderFactory IVsSccProviderFactory;
#endif 	/* __IVsSccProviderFactory_FWD_DEFINED__ */


/* header files for imported files */
#include "objidl.h"

#ifdef __cplusplus
extern "C"{
#endif 


/* interface __MIDL_itf_IVsSccProviderFactory_0000_0000 */
/* [local] */ 

#pragma once


extern RPC_IF_HANDLE __MIDL_itf_IVsSccProviderFactory_0000_0000_v0_0_c_ifspec;
extern RPC_IF_HANDLE __MIDL_itf_IVsSccProviderFactory_0000_0000_v0_0_s_ifspec;

#ifndef __IVsSccProviderFactory_INTERFACE_DEFINED__
#define __IVsSccProviderFactory_INTERFACE_DEFINED__

/* interface IVsSccProviderFactory */
/* [object][unique][version][uuid] */ 


EXTERN_C const IID IID_IVsSccProviderFactory;

#if defined(__cplusplus) && !defined(CINTERFACE)
    
    MIDL_INTERFACE("53544c4d-03f8-11d0-8e5e-00a0c911005a")
    IVsSccProviderFactory : public IUnknown
    {
    public:
        virtual HRESULT STDMETHODCALLTYPE CreateProvider( 
            /* [in] */ __RPC__in LPCOLESTR lpszProjectServerPath,
            /* [in] */ __RPC__in LPCOLESTR lpszProjectLocalPath,
            /* [retval][out] */ __RPC__deref_out_opt IUnknown **punkSession) = 0;
        
    };
    
#else 	/* C style interface */

    typedef struct IVsSccProviderFactoryVtbl
    {
        BEGIN_INTERFACE
        
        HRESULT ( STDMETHODCALLTYPE *QueryInterface )( 
            IVsSccProviderFactory * This,
            /* [in] */ __RPC__in REFIID riid,
            /* [iid_is][out] */ 
            __RPC__deref_out  void **ppvObject);
        
        ULONG ( STDMETHODCALLTYPE *AddRef )( 
            IVsSccProviderFactory * This);
        
        ULONG ( STDMETHODCALLTYPE *Release )( 
            IVsSccProviderFactory * This);
        
        HRESULT ( STDMETHODCALLTYPE *CreateProvider )( 
            IVsSccProviderFactory * This,
            /* [in] */ __RPC__in LPCOLESTR lpszProjectServerPath,
            /* [in] */ __RPC__in LPCOLESTR lpszProjectLocalPath,
            /* [retval][out] */ __RPC__deref_out_opt IUnknown **punkSession);
        
        END_INTERFACE
    } IVsSccProviderFactoryVtbl;

    interface IVsSccProviderFactory
    {
        CONST_VTBL struct IVsSccProviderFactoryVtbl *lpVtbl;
    };

    

#ifdef COBJMACROS


#define IVsSccProviderFactory_QueryInterface(This,riid,ppvObject)	\
    ( (This)->lpVtbl -> QueryInterface(This,riid,ppvObject) ) 

#define IVsSccProviderFactory_AddRef(This)	\
    ( (This)->lpVtbl -> AddRef(This) ) 

#define IVsSccProviderFactory_Release(This)	\
    ( (This)->lpVtbl -> Release(This) ) 


#define IVsSccProviderFactory_CreateProvider(This,lpszProjectServerPath,lpszProjectLocalPath,punkSession)	\
    ( (This)->lpVtbl -> CreateProvider(This,lpszProjectServerPath,lpszProjectLocalPath,punkSession) ) 

#endif /* COBJMACROS */


#endif 	/* C style interface */




#endif 	/* __IVsSccProviderFactory_INTERFACE_DEFINED__ */


/* Additional Prototypes for ALL interfaces */

/* end of Additional Prototypes */

#ifdef __cplusplus
}
#endif

#endif


