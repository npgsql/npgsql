

/* this ALWAYS GENERATED file contains the definitions for the interfaces */


 /* File created by MIDL compiler version 7.00.0499 */
/* Compiler settings for IVsSccProjectEnlistmentChoice.idl:
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

#ifndef __IVsSccProjectEnlistmentChoice_h__
#define __IVsSccProjectEnlistmentChoice_h__

#if defined(_MSC_VER) && (_MSC_VER >= 1020)
#pragma once
#endif

/* Forward Declarations */ 

#ifndef __IVsSccProjectEnlistmentChoice_FWD_DEFINED__
#define __IVsSccProjectEnlistmentChoice_FWD_DEFINED__
typedef interface IVsSccProjectEnlistmentChoice IVsSccProjectEnlistmentChoice;
#endif 	/* __IVsSccProjectEnlistmentChoice_FWD_DEFINED__ */


/* header files for imported files */
#include "objidl.h"

#ifdef __cplusplus
extern "C"{
#endif 


/* interface __MIDL_itf_IVsSccProjectEnlistmentChoice_0000_0000 */
/* [local] */ 

#pragma once
typedef 
enum __VSSCCENLISTMENTCHOICE
    {	VSSCC_EC_NEVER	= 0,
	VSSCC_EC_OPTIONAL	= 1,
	VSSCC_EC_COMPULSORY	= 2
    } 	VSSCCENLISTMENTCHOICE;



extern RPC_IF_HANDLE __MIDL_itf_IVsSccProjectEnlistmentChoice_0000_0000_v0_0_c_ifspec;
extern RPC_IF_HANDLE __MIDL_itf_IVsSccProjectEnlistmentChoice_0000_0000_v0_0_s_ifspec;

#ifndef __IVsSccProjectEnlistmentChoice_INTERFACE_DEFINED__
#define __IVsSccProjectEnlistmentChoice_INTERFACE_DEFINED__

/* interface IVsSccProjectEnlistmentChoice */
/* [object][unique][version][uuid] */ 


EXTERN_C const IID IID_IVsSccProjectEnlistmentChoice;

#if defined(__cplusplus) && !defined(CINTERFACE)
    
    MIDL_INTERFACE("53544c4d-06f8-11d0-8e5e-00a0c911005a")
    IVsSccProjectEnlistmentChoice : public IUnknown
    {
    public:
        virtual HRESULT STDMETHODCALLTYPE GetEnlistmentChoice( 
            /* [retval][out] */ __RPC__out VSSCCENLISTMENTCHOICE *pvscecEnlistmentChoice) = 0;
        
    };
    
#else 	/* C style interface */

    typedef struct IVsSccProjectEnlistmentChoiceVtbl
    {
        BEGIN_INTERFACE
        
        HRESULT ( STDMETHODCALLTYPE *QueryInterface )( 
            IVsSccProjectEnlistmentChoice * This,
            /* [in] */ __RPC__in REFIID riid,
            /* [iid_is][out] */ 
            __RPC__deref_out  void **ppvObject);
        
        ULONG ( STDMETHODCALLTYPE *AddRef )( 
            IVsSccProjectEnlistmentChoice * This);
        
        ULONG ( STDMETHODCALLTYPE *Release )( 
            IVsSccProjectEnlistmentChoice * This);
        
        HRESULT ( STDMETHODCALLTYPE *GetEnlistmentChoice )( 
            IVsSccProjectEnlistmentChoice * This,
            /* [retval][out] */ __RPC__out VSSCCENLISTMENTCHOICE *pvscecEnlistmentChoice);
        
        END_INTERFACE
    } IVsSccProjectEnlistmentChoiceVtbl;

    interface IVsSccProjectEnlistmentChoice
    {
        CONST_VTBL struct IVsSccProjectEnlistmentChoiceVtbl *lpVtbl;
    };

    

#ifdef COBJMACROS


#define IVsSccProjectEnlistmentChoice_QueryInterface(This,riid,ppvObject)	\
    ( (This)->lpVtbl -> QueryInterface(This,riid,ppvObject) ) 

#define IVsSccProjectEnlistmentChoice_AddRef(This)	\
    ( (This)->lpVtbl -> AddRef(This) ) 

#define IVsSccProjectEnlistmentChoice_Release(This)	\
    ( (This)->lpVtbl -> Release(This) ) 


#define IVsSccProjectEnlistmentChoice_GetEnlistmentChoice(This,pvscecEnlistmentChoice)	\
    ( (This)->lpVtbl -> GetEnlistmentChoice(This,pvscecEnlistmentChoice) ) 

#endif /* COBJMACROS */


#endif 	/* C style interface */




#endif 	/* __IVsSccProjectEnlistmentChoice_INTERFACE_DEFINED__ */


/* Additional Prototypes for ALL interfaces */

/* end of Additional Prototypes */

#ifdef __cplusplus
}
#endif

#endif


