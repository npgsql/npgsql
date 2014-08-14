

/* this ALWAYS GENERATED file contains the definitions for the interfaces */


 /* File created by MIDL compiler version 7.00.0499 */
/* Compiler settings for ee100.idl:
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

#ifndef __ee100_h__
#define __ee100_h__

#if defined(_MSC_VER) && (_MSC_VER >= 1020)
#pragma once
#endif

/* Forward Declarations */ 

#ifndef __IDebugBinderDirect100_FWD_DEFINED__
#define __IDebugBinderDirect100_FWD_DEFINED__
typedef interface IDebugBinderDirect100 IDebugBinderDirect100;
#endif 	/* __IDebugBinderDirect100_FWD_DEFINED__ */


#ifndef __IDebugExpressionEvaluator100_FWD_DEFINED__
#define __IDebugExpressionEvaluator100_FWD_DEFINED__
typedef interface IDebugExpressionEvaluator100 IDebugExpressionEvaluator100;
#endif 	/* __IDebugExpressionEvaluator100_FWD_DEFINED__ */


/* header files for imported files */
#include "ocidl.h"

#ifdef __cplusplus
extern "C"{
#endif 


/* interface __MIDL_itf_ee100_0000_0000 */
/* [local] */ 

#define E_EVAL_ILLEGAL_SIDE_EFFECT            	MAKE_HRESULT(1, FACILITY_ITF, 0x0019)
#define S_EVAL_THREADSLIP_REQUIRED            	MAKE_HRESULT(0, FACILITY_ITF, 0x0020)
#define E_EVAL_INTERPRETER_ERROR                 MAKE_HRESULT(1, FACILITY_ITF, 0x0021)
#define E_EVAL_FUNCEVAL_IN_MINIDUMP              MAKE_HRESULT(1, FACILITY_ITF, 0x0022)


extern RPC_IF_HANDLE __MIDL_itf_ee100_0000_0000_v0_0_c_ifspec;
extern RPC_IF_HANDLE __MIDL_itf_ee100_0000_0000_v0_0_s_ifspec;

#ifndef __IDebugBinderDirect100_INTERFACE_DEFINED__
#define __IDebugBinderDirect100_INTERFACE_DEFINED__

/* interface IDebugBinderDirect100 */
/* [unique][uuid][object] */ 


EXTERN_C const IID IID_IDebugBinderDirect100;

#if defined(__cplusplus) && !defined(CINTERFACE)
    
    MIDL_INTERFACE("00ca5803-6adc-49b3-adcb-a2b538b5665e")
    IDebugBinderDirect100 : public IUnknown
    {
    public:
        virtual HRESULT STDMETHODCALLTYPE GetErrorMessage( 
            HRESULT hrError,
            /* [out] */ __RPC__deref_out_opt BSTR *pbstrError) = 0;
        
        virtual HRESULT STDMETHODCALLTYPE IsDump( 
            /* [out] */ __RPC__out BOOL *pfDump) = 0;
        
    };
    
#else 	/* C style interface */

    typedef struct IDebugBinderDirect100Vtbl
    {
        BEGIN_INTERFACE
        
        HRESULT ( STDMETHODCALLTYPE *QueryInterface )( 
            IDebugBinderDirect100 * This,
            /* [in] */ __RPC__in REFIID riid,
            /* [iid_is][out] */ 
            __RPC__deref_out  void **ppvObject);
        
        ULONG ( STDMETHODCALLTYPE *AddRef )( 
            IDebugBinderDirect100 * This);
        
        ULONG ( STDMETHODCALLTYPE *Release )( 
            IDebugBinderDirect100 * This);
        
        HRESULT ( STDMETHODCALLTYPE *GetErrorMessage )( 
            IDebugBinderDirect100 * This,
            HRESULT hrError,
            /* [out] */ __RPC__deref_out_opt BSTR *pbstrError);
        
        HRESULT ( STDMETHODCALLTYPE *IsDump )( 
            IDebugBinderDirect100 * This,
            /* [out] */ __RPC__out BOOL *pfDump);
        
        END_INTERFACE
    } IDebugBinderDirect100Vtbl;

    interface IDebugBinderDirect100
    {
        CONST_VTBL struct IDebugBinderDirect100Vtbl *lpVtbl;
    };

    

#ifdef COBJMACROS


#define IDebugBinderDirect100_QueryInterface(This,riid,ppvObject)	\
    ( (This)->lpVtbl -> QueryInterface(This,riid,ppvObject) ) 

#define IDebugBinderDirect100_AddRef(This)	\
    ( (This)->lpVtbl -> AddRef(This) ) 

#define IDebugBinderDirect100_Release(This)	\
    ( (This)->lpVtbl -> Release(This) ) 


#define IDebugBinderDirect100_GetErrorMessage(This,hrError,pbstrError)	\
    ( (This)->lpVtbl -> GetErrorMessage(This,hrError,pbstrError) ) 

#define IDebugBinderDirect100_IsDump(This,pfDump)	\
    ( (This)->lpVtbl -> IsDump(This,pfDump) ) 

#endif /* COBJMACROS */


#endif 	/* C style interface */




#endif 	/* __IDebugBinderDirect100_INTERFACE_DEFINED__ */


#ifndef __IDebugExpressionEvaluator100_INTERFACE_DEFINED__
#define __IDebugExpressionEvaluator100_INTERFACE_DEFINED__

/* interface IDebugExpressionEvaluator100 */
/* [unique][uuid][object] */ 


EXTERN_C const IID IID_IDebugExpressionEvaluator100;

#if defined(__cplusplus) && !defined(CINTERFACE)
    
    MIDL_INTERFACE("643CD3D4-043D-410C-8201-7E83C8918A91")
    IDebugExpressionEvaluator100 : public IUnknown
    {
    public:
        virtual HRESULT STDMETHODCALLTYPE SetAttachedCLRMajorMinorVersion( 
            /* [in] */ __RPC__in BSTR bstrCLRVersion) = 0;
        
        virtual HRESULT STDMETHODCALLTYPE NotifyFuncEval( void) = 0;
        
    };
    
#else 	/* C style interface */

    typedef struct IDebugExpressionEvaluator100Vtbl
    {
        BEGIN_INTERFACE
        
        HRESULT ( STDMETHODCALLTYPE *QueryInterface )( 
            IDebugExpressionEvaluator100 * This,
            /* [in] */ __RPC__in REFIID riid,
            /* [iid_is][out] */ 
            __RPC__deref_out  void **ppvObject);
        
        ULONG ( STDMETHODCALLTYPE *AddRef )( 
            IDebugExpressionEvaluator100 * This);
        
        ULONG ( STDMETHODCALLTYPE *Release )( 
            IDebugExpressionEvaluator100 * This);
        
        HRESULT ( STDMETHODCALLTYPE *SetAttachedCLRMajorMinorVersion )( 
            IDebugExpressionEvaluator100 * This,
            /* [in] */ __RPC__in BSTR bstrCLRVersion);
        
        HRESULT ( STDMETHODCALLTYPE *NotifyFuncEval )( 
            IDebugExpressionEvaluator100 * This);
        
        END_INTERFACE
    } IDebugExpressionEvaluator100Vtbl;

    interface IDebugExpressionEvaluator100
    {
        CONST_VTBL struct IDebugExpressionEvaluator100Vtbl *lpVtbl;
    };

    

#ifdef COBJMACROS


#define IDebugExpressionEvaluator100_QueryInterface(This,riid,ppvObject)	\
    ( (This)->lpVtbl -> QueryInterface(This,riid,ppvObject) ) 

#define IDebugExpressionEvaluator100_AddRef(This)	\
    ( (This)->lpVtbl -> AddRef(This) ) 

#define IDebugExpressionEvaluator100_Release(This)	\
    ( (This)->lpVtbl -> Release(This) ) 


#define IDebugExpressionEvaluator100_SetAttachedCLRMajorMinorVersion(This,bstrCLRVersion)	\
    ( (This)->lpVtbl -> SetAttachedCLRMajorMinorVersion(This,bstrCLRVersion) ) 

#define IDebugExpressionEvaluator100_NotifyFuncEval(This)	\
    ( (This)->lpVtbl -> NotifyFuncEval(This) ) 

#endif /* COBJMACROS */


#endif 	/* C style interface */




#endif 	/* __IDebugExpressionEvaluator100_INTERFACE_DEFINED__ */


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


