

/* this ALWAYS GENERATED file contains the definitions for the interfaces */


 /* File created by MIDL compiler version 7.00.0499 */
/* Compiler settings for textfind90.idl:
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

#ifndef __textfind90_h__
#define __textfind90_h__

#if defined(_MSC_VER) && (_MSC_VER >= 1020)
#pragma once
#endif

/* Forward Declarations */ 

#ifndef __IVsFindCancelDialog2_FWD_DEFINED__
#define __IVsFindCancelDialog2_FWD_DEFINED__
typedef interface IVsFindCancelDialog2 IVsFindCancelDialog2;
#endif 	/* __IVsFindCancelDialog2_FWD_DEFINED__ */


/* header files for imported files */
#include "textfind2.h"

#ifdef __cplusplus
extern "C"{
#endif 


/* interface __MIDL_itf_textfind90_0000_0000 */
/* [local] */ 

#pragma once


extern RPC_IF_HANDLE __MIDL_itf_textfind90_0000_0000_v0_0_c_ifspec;
extern RPC_IF_HANDLE __MIDL_itf_textfind90_0000_0000_v0_0_s_ifspec;

#ifndef __IVsFindCancelDialog2_INTERFACE_DEFINED__
#define __IVsFindCancelDialog2_INTERFACE_DEFINED__

/* interface IVsFindCancelDialog2 */
/* [object][unique][version][uuid] */ 


EXTERN_C const IID IID_IVsFindCancelDialog2;

#if defined(__cplusplus) && !defined(CINTERFACE)
    
    MIDL_INTERFACE("0C8E8E61-71AF-41dd-B213-B98D042F07FE")
    IVsFindCancelDialog2 : public IUnknown
    {
    public:
        virtual HRESULT STDMETHODCALLTYPE DialogAlreadyLaunched( 
            /* [out] */ __RPC__out BOOL *pfLaunched) = 0;
        
    };
    
#else 	/* C style interface */

    typedef struct IVsFindCancelDialog2Vtbl
    {
        BEGIN_INTERFACE
        
        HRESULT ( STDMETHODCALLTYPE *QueryInterface )( 
            IVsFindCancelDialog2 * This,
            /* [in] */ __RPC__in REFIID riid,
            /* [iid_is][out] */ 
            __RPC__deref_out  void **ppvObject);
        
        ULONG ( STDMETHODCALLTYPE *AddRef )( 
            IVsFindCancelDialog2 * This);
        
        ULONG ( STDMETHODCALLTYPE *Release )( 
            IVsFindCancelDialog2 * This);
        
        HRESULT ( STDMETHODCALLTYPE *DialogAlreadyLaunched )( 
            IVsFindCancelDialog2 * This,
            /* [out] */ __RPC__out BOOL *pfLaunched);
        
        END_INTERFACE
    } IVsFindCancelDialog2Vtbl;

    interface IVsFindCancelDialog2
    {
        CONST_VTBL struct IVsFindCancelDialog2Vtbl *lpVtbl;
    };

    

#ifdef COBJMACROS


#define IVsFindCancelDialog2_QueryInterface(This,riid,ppvObject)	\
    ( (This)->lpVtbl -> QueryInterface(This,riid,ppvObject) ) 

#define IVsFindCancelDialog2_AddRef(This)	\
    ( (This)->lpVtbl -> AddRef(This) ) 

#define IVsFindCancelDialog2_Release(This)	\
    ( (This)->lpVtbl -> Release(This) ) 


#define IVsFindCancelDialog2_DialogAlreadyLaunched(This,pfLaunched)	\
    ( (This)->lpVtbl -> DialogAlreadyLaunched(This,pfLaunched) ) 

#endif /* COBJMACROS */


#endif 	/* C style interface */




#endif 	/* __IVsFindCancelDialog2_INTERFACE_DEFINED__ */


/* Additional Prototypes for ALL interfaces */

/* end of Additional Prototypes */

#ifdef __cplusplus
}
#endif

#endif


