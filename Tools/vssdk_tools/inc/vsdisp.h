

/* this ALWAYS GENERATED file contains the definitions for the interfaces */


 /* File created by MIDL compiler version 7.00.0499 */
/* Compiler settings for vsdisp.idl:
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

#ifndef __vsdisp_h__
#define __vsdisp_h__

#if defined(_MSC_VER) && (_MSC_VER >= 1020)
#pragma once
#endif

/* Forward Declarations */ 

#ifndef __IVsDispatch_FWD_DEFINED__
#define __IVsDispatch_FWD_DEFINED__
typedef interface IVsDispatch IVsDispatch;
#endif 	/* __IVsDispatch_FWD_DEFINED__ */


/* header files for imported files */
#include "oleidl.h"
#include "oaidl.h"

#ifdef __cplusplus
extern "C"{
#endif 


/* interface __MIDL_itf_vsdisp_0000_0000 */
/* [local] */ 

typedef LONG VSDISPID;



extern RPC_IF_HANDLE __MIDL_itf_vsdisp_0000_0000_v0_0_c_ifspec;
extern RPC_IF_HANDLE __MIDL_itf_vsdisp_0000_0000_v0_0_s_ifspec;

#ifndef __IVsDispatch_INTERFACE_DEFINED__
#define __IVsDispatch_INTERFACE_DEFINED__

/* interface IVsDispatch */
/* [object][oleautomation][uuid] */ 


EXTERN_C const IID IID_IVsDispatch;

#if defined(__cplusplus) && !defined(CINTERFACE)
    
    MIDL_INTERFACE("09CCD272-5FA0-11d2-B1F8-0080C747D9A0")
    IVsDispatch : public IUnknown
    {
    public:
        virtual HRESULT STDMETHODCALLTYPE Do( 
            /* [in] */ VSDISPID vsdispid,
            /* [in] */ long celIn,
            /* [size_is][in] */ __RPC__in_ecount_full(celIn) VARIANT *rgvaIn,
            /* [in] */ long celOut,
            /* [size_is][out][in] */ __RPC__inout_ecount_full(celOut) VARIANT *rgvaOut) = 0;
        
    };
    
#else 	/* C style interface */

    typedef struct IVsDispatchVtbl
    {
        BEGIN_INTERFACE
        
        HRESULT ( STDMETHODCALLTYPE *QueryInterface )( 
            IVsDispatch * This,
            /* [in] */ __RPC__in REFIID riid,
            /* [iid_is][out] */ 
            __RPC__deref_out  void **ppvObject);
        
        ULONG ( STDMETHODCALLTYPE *AddRef )( 
            IVsDispatch * This);
        
        ULONG ( STDMETHODCALLTYPE *Release )( 
            IVsDispatch * This);
        
        HRESULT ( STDMETHODCALLTYPE *Do )( 
            IVsDispatch * This,
            /* [in] */ VSDISPID vsdispid,
            /* [in] */ long celIn,
            /* [size_is][in] */ __RPC__in_ecount_full(celIn) VARIANT *rgvaIn,
            /* [in] */ long celOut,
            /* [size_is][out][in] */ __RPC__inout_ecount_full(celOut) VARIANT *rgvaOut);
        
        END_INTERFACE
    } IVsDispatchVtbl;

    interface IVsDispatch
    {
        CONST_VTBL struct IVsDispatchVtbl *lpVtbl;
    };

    

#ifdef COBJMACROS


#define IVsDispatch_QueryInterface(This,riid,ppvObject)	\
    ( (This)->lpVtbl -> QueryInterface(This,riid,ppvObject) ) 

#define IVsDispatch_AddRef(This)	\
    ( (This)->lpVtbl -> AddRef(This) ) 

#define IVsDispatch_Release(This)	\
    ( (This)->lpVtbl -> Release(This) ) 


#define IVsDispatch_Do(This,vsdispid,celIn,rgvaIn,celOut,rgvaOut)	\
    ( (This)->lpVtbl -> Do(This,vsdispid,celIn,rgvaIn,celOut,rgvaOut) ) 

#endif /* COBJMACROS */


#endif 	/* C style interface */




#endif 	/* __IVsDispatch_INTERFACE_DEFINED__ */


/* Additional Prototypes for ALL interfaces */

unsigned long             __RPC_USER  VARIANT_UserSize(     unsigned long *, unsigned long            , VARIANT * ); 
unsigned char * __RPC_USER  VARIANT_UserMarshal(  unsigned long *, unsigned char *, VARIANT * ); 
unsigned char * __RPC_USER  VARIANT_UserUnmarshal(unsigned long *, unsigned char *, VARIANT * ); 
void                      __RPC_USER  VARIANT_UserFree(     unsigned long *, VARIANT * ); 

/* end of Additional Prototypes */

#ifdef __cplusplus
}
#endif

#endif


