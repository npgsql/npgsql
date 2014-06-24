

/* this ALWAYS GENERATED file contains the definitions for the interfaces */


 /* File created by MIDL compiler version 7.00.0499 */
/* Compiler settings for ee90.idl:
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

#ifndef __ee90_h__
#define __ee90_h__

#if defined(_MSC_VER) && (_MSC_VER >= 1020)
#pragma once
#endif

/* Forward Declarations */ 

#ifndef __IDebugObject90_FWD_DEFINED__
#define __IDebugObject90_FWD_DEFINED__
typedef interface IDebugObject90 IDebugObject90;
#endif 	/* __IDebugObject90_FWD_DEFINED__ */


/* header files for imported files */
#include "ocidl.h"

#ifdef __cplusplus
extern "C"{
#endif 


#ifndef __IDebugObject90_INTERFACE_DEFINED__
#define __IDebugObject90_INTERFACE_DEFINED__

/* interface IDebugObject90 */
/* [unique][uuid][object] */ 


EXTERN_C const IID IID_IDebugObject90;

#if defined(__cplusplus) && !defined(CINTERFACE)
    
    MIDL_INTERFACE("2B8F1709-EC30-48AC-9BAA-23867A5076DC")
    IDebugObject90 : public IUnknown
    {
    public:
        virtual HRESULT STDMETHODCALLTYPE CreateStrongAlias( 
            /* [in] */ __RPC__in REFIID riid,
            /* [iid_is][out] */ __RPC__deref_out_opt void **ppvObject) = 0;
        
    };
    
#else 	/* C style interface */

    typedef struct IDebugObject90Vtbl
    {
        BEGIN_INTERFACE
        
        HRESULT ( STDMETHODCALLTYPE *QueryInterface )( 
            IDebugObject90 * This,
            /* [in] */ __RPC__in REFIID riid,
            /* [iid_is][out] */ 
            __RPC__deref_out  void **ppvObject);
        
        ULONG ( STDMETHODCALLTYPE *AddRef )( 
            IDebugObject90 * This);
        
        ULONG ( STDMETHODCALLTYPE *Release )( 
            IDebugObject90 * This);
        
        HRESULT ( STDMETHODCALLTYPE *CreateStrongAlias )( 
            IDebugObject90 * This,
            /* [in] */ __RPC__in REFIID riid,
            /* [iid_is][out] */ __RPC__deref_out_opt void **ppvObject);
        
        END_INTERFACE
    } IDebugObject90Vtbl;

    interface IDebugObject90
    {
        CONST_VTBL struct IDebugObject90Vtbl *lpVtbl;
    };

    

#ifdef COBJMACROS


#define IDebugObject90_QueryInterface(This,riid,ppvObject)	\
    ( (This)->lpVtbl -> QueryInterface(This,riid,ppvObject) ) 

#define IDebugObject90_AddRef(This)	\
    ( (This)->lpVtbl -> AddRef(This) ) 

#define IDebugObject90_Release(This)	\
    ( (This)->lpVtbl -> Release(This) ) 


#define IDebugObject90_CreateStrongAlias(This,riid,ppvObject)	\
    ( (This)->lpVtbl -> CreateStrongAlias(This,riid,ppvObject) ) 

#endif /* COBJMACROS */


#endif 	/* C style interface */




#endif 	/* __IDebugObject90_INTERFACE_DEFINED__ */


/* Additional Prototypes for ALL interfaces */

/* end of Additional Prototypes */

#ifdef __cplusplus
}
#endif

#endif


