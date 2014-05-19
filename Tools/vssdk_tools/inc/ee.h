

/* this ALWAYS GENERATED file contains the definitions for the interfaces */


 /* File created by MIDL compiler version 7.00.0499 */
/* Compiler settings for ee.idl:
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

#ifndef __ee_h__
#define __ee_h__

#if defined(_MSC_VER) && (_MSC_VER >= 1020)
#pragma once
#endif

/* Forward Declarations */ 

#ifndef __IDebugExpressionEvaluator_FWD_DEFINED__
#define __IDebugExpressionEvaluator_FWD_DEFINED__
typedef interface IDebugExpressionEvaluator IDebugExpressionEvaluator;
#endif 	/* __IDebugExpressionEvaluator_FWD_DEFINED__ */


#ifndef __IDebugExpressionEvaluator2_FWD_DEFINED__
#define __IDebugExpressionEvaluator2_FWD_DEFINED__
typedef interface IDebugExpressionEvaluator2 IDebugExpressionEvaluator2;
#endif 	/* __IDebugExpressionEvaluator2_FWD_DEFINED__ */


#ifndef __IDebugExpressionEvaluator3_FWD_DEFINED__
#define __IDebugExpressionEvaluator3_FWD_DEFINED__
typedef interface IDebugExpressionEvaluator3 IDebugExpressionEvaluator3;
#endif 	/* __IDebugExpressionEvaluator3_FWD_DEFINED__ */


#ifndef __IDebugIDECallback_FWD_DEFINED__
#define __IDebugIDECallback_FWD_DEFINED__
typedef interface IDebugIDECallback IDebugIDECallback;
#endif 	/* __IDebugIDECallback_FWD_DEFINED__ */


#ifndef __IDebugIteratorFrameProvider_FWD_DEFINED__
#define __IDebugIteratorFrameProvider_FWD_DEFINED__
typedef interface IDebugIteratorFrameProvider IDebugIteratorFrameProvider;
#endif 	/* __IDebugIteratorFrameProvider_FWD_DEFINED__ */


#ifndef __IDebugObject_FWD_DEFINED__
#define __IDebugObject_FWD_DEFINED__
typedef interface IDebugObject IDebugObject;
#endif 	/* __IDebugObject_FWD_DEFINED__ */


#ifndef __IDebugObject2_FWD_DEFINED__
#define __IDebugObject2_FWD_DEFINED__
typedef interface IDebugObject2 IDebugObject2;
#endif 	/* __IDebugObject2_FWD_DEFINED__ */


#ifndef __IDebugArrayObject_FWD_DEFINED__
#define __IDebugArrayObject_FWD_DEFINED__
typedef interface IDebugArrayObject IDebugArrayObject;
#endif 	/* __IDebugArrayObject_FWD_DEFINED__ */


#ifndef __IDebugArrayObject2_FWD_DEFINED__
#define __IDebugArrayObject2_FWD_DEFINED__
typedef interface IDebugArrayObject2 IDebugArrayObject2;
#endif 	/* __IDebugArrayObject2_FWD_DEFINED__ */


#ifndef __IDebugFunctionObject_FWD_DEFINED__
#define __IDebugFunctionObject_FWD_DEFINED__
typedef interface IDebugFunctionObject IDebugFunctionObject;
#endif 	/* __IDebugFunctionObject_FWD_DEFINED__ */


#ifndef __IDebugFunctionObject2_FWD_DEFINED__
#define __IDebugFunctionObject2_FWD_DEFINED__
typedef interface IDebugFunctionObject2 IDebugFunctionObject2;
#endif 	/* __IDebugFunctionObject2_FWD_DEFINED__ */


#ifndef __IDebugManagedObject_FWD_DEFINED__
#define __IDebugManagedObject_FWD_DEFINED__
typedef interface IDebugManagedObject IDebugManagedObject;
#endif 	/* __IDebugManagedObject_FWD_DEFINED__ */


#ifndef __IDebugBinder_FWD_DEFINED__
#define __IDebugBinder_FWD_DEFINED__
typedef interface IDebugBinder IDebugBinder;
#endif 	/* __IDebugBinder_FWD_DEFINED__ */


#ifndef __IDebugBinderDirect_FWD_DEFINED__
#define __IDebugBinderDirect_FWD_DEFINED__
typedef interface IDebugBinderDirect IDebugBinderDirect;
#endif 	/* __IDebugBinderDirect_FWD_DEFINED__ */


#ifndef __IDebugBinder2_FWD_DEFINED__
#define __IDebugBinder2_FWD_DEFINED__
typedef interface IDebugBinder2 IDebugBinder2;
#endif 	/* __IDebugBinder2_FWD_DEFINED__ */


#ifndef __IDebugBinder3_FWD_DEFINED__
#define __IDebugBinder3_FWD_DEFINED__
typedef interface IDebugBinder3 IDebugBinder3;
#endif 	/* __IDebugBinder3_FWD_DEFINED__ */


#ifndef __IEEVisualizerDataProvider_FWD_DEFINED__
#define __IEEVisualizerDataProvider_FWD_DEFINED__
typedef interface IEEVisualizerDataProvider IEEVisualizerDataProvider;
#endif 	/* __IEEVisualizerDataProvider_FWD_DEFINED__ */


#ifndef __IEEVisualizerService_FWD_DEFINED__
#define __IEEVisualizerService_FWD_DEFINED__
typedef interface IEEVisualizerService IEEVisualizerService;
#endif 	/* __IEEVisualizerService_FWD_DEFINED__ */


#ifndef __IEEVisualizerServiceProvider_FWD_DEFINED__
#define __IEEVisualizerServiceProvider_FWD_DEFINED__
typedef interface IEEVisualizerServiceProvider IEEVisualizerServiceProvider;
#endif 	/* __IEEVisualizerServiceProvider_FWD_DEFINED__ */


#ifndef __IDebugPointerObject_FWD_DEFINED__
#define __IDebugPointerObject_FWD_DEFINED__
typedef interface IDebugPointerObject IDebugPointerObject;
#endif 	/* __IDebugPointerObject_FWD_DEFINED__ */


#ifndef __IDebugPointerObject2_FWD_DEFINED__
#define __IDebugPointerObject2_FWD_DEFINED__
typedef interface IDebugPointerObject2 IDebugPointerObject2;
#endif 	/* __IDebugPointerObject2_FWD_DEFINED__ */


#ifndef __IDebugPointerObject3_FWD_DEFINED__
#define __IDebugPointerObject3_FWD_DEFINED__
typedef interface IDebugPointerObject3 IDebugPointerObject3;
#endif 	/* __IDebugPointerObject3_FWD_DEFINED__ */


#ifndef __IDebugParsedExpression_FWD_DEFINED__
#define __IDebugParsedExpression_FWD_DEFINED__
typedef interface IDebugParsedExpression IDebugParsedExpression;
#endif 	/* __IDebugParsedExpression_FWD_DEFINED__ */


#ifndef __IEnumDebugObjects_FWD_DEFINED__
#define __IEnumDebugObjects_FWD_DEFINED__
typedef interface IEnumDebugObjects IEnumDebugObjects;
#endif 	/* __IEnumDebugObjects_FWD_DEFINED__ */


#ifndef __IDebugAlias_FWD_DEFINED__
#define __IDebugAlias_FWD_DEFINED__
typedef interface IDebugAlias IDebugAlias;
#endif 	/* __IDebugAlias_FWD_DEFINED__ */


#ifndef __ManagedCppExpressionEvaluator_FWD_DEFINED__
#define __ManagedCppExpressionEvaluator_FWD_DEFINED__

#ifdef __cplusplus
typedef class ManagedCppExpressionEvaluator ManagedCppExpressionEvaluator;
#else
typedef struct ManagedCppExpressionEvaluator ManagedCppExpressionEvaluator;
#endif /* __cplusplus */

#endif 	/* __ManagedCppExpressionEvaluator_FWD_DEFINED__ */


#ifndef __CSharpExpressionEvaluator_FWD_DEFINED__
#define __CSharpExpressionEvaluator_FWD_DEFINED__

#ifdef __cplusplus
typedef class CSharpExpressionEvaluator CSharpExpressionEvaluator;
#else
typedef struct CSharpExpressionEvaluator CSharpExpressionEvaluator;
#endif /* __cplusplus */

#endif 	/* __CSharpExpressionEvaluator_FWD_DEFINED__ */


#ifndef __JSharpExpressionEvaluator_FWD_DEFINED__
#define __JSharpExpressionEvaluator_FWD_DEFINED__

#ifdef __cplusplus
typedef class JSharpExpressionEvaluator JSharpExpressionEvaluator;
#else
typedef struct JSharpExpressionEvaluator JSharpExpressionEvaluator;
#endif /* __cplusplus */

#endif 	/* __JSharpExpressionEvaluator_FWD_DEFINED__ */


/* header files for imported files */
#include "ocidl.h"
#include "msdbg.h"

#ifdef __cplusplus
extern "C"{
#endif 


/* interface __MIDL_itf_ee_0000_0000 */
/* [local] */ 















#define S_EVAL_EXCEPTION                 	MAKE_HRESULT(0, FACILITY_ITF, 0x0002)
#define S_EVAL_ABORTED                   	MAKE_HRESULT(0, FACILITY_ITF, 0x0003)
#define S_EVAL_TIMEDOUT                  	MAKE_HRESULT(0, FACILITY_ITF, 0x0004)
#define S_EVAL_NO_RESULT                 	MAKE_HRESULT(0, FACILITY_ITF, 0x0005)
#define S_EVAL_THREAD_SUSPENDED          	MAKE_HRESULT(0, FACILITY_ITF, 0x0006)
#define S_EVAL_THREAD_SLEEP_WAIT_JOIN    	MAKE_HRESULT(0, FACILITY_ITF, 0x0007)
#define S_EVAL_BAD_THREAD_STATE          	MAKE_HRESULT(0, FACILITY_ITF, 0x0008)
#define S_EVAL_THREAD_NOT_STARTED        	MAKE_HRESULT(0, FACILITY_ITF, 0x0009)
#define S_EVAL_BAD_START_POINT           	MAKE_HRESULT(0, FACILITY_ITF, 0x000A)
#define E_STATIC_VAR_NOT_AVAILABLE       	MAKE_HRESULT(0, FACILITY_ITF, 0x000B)
#define S_EVAL_WEB_METHOD                	MAKE_HRESULT(0, FACILITY_ITF, 0x000C)
#define S_EVAL_STOP_REQUESTED            	MAKE_HRESULT(0, FACILITY_ITF, 0x000D)
#define S_EVAL_SUSPEND_REQUESTED         	MAKE_HRESULT(0, FACILITY_ITF, 0x000E)
#define S_EVAL_UNSCHEDULED_FIBER         	MAKE_HRESULT(0, FACILITY_ITF, 0x000F)
#define E_EVAL_NOT_SUPPORTED_IN_CLR      	MAKE_HRESULT(1, FACILITY_ITF, 0x0010)
#define E_EVAL_OBJECT_ID_NOT_FOUND       	MAKE_HRESULT(1, FACILITY_ITF, 0x0011)
#define E_EVAL_DIFFERENT_APPIDS          	MAKE_HRESULT(1, FACILITY_ITF, 0x0012)
#define E_EVAL_MODULE_NOT_FOUND_IN_APPID 	MAKE_HRESULT(1, FACILITY_ITF, 0x0013)
#define E_EVAL_OVERFLOW_HAS_OCCURRED     	MAKE_HRESULT(1, FACILITY_ITF, 0x0014)
#define E_EVAL_NULL_REFERENCE            	MAKE_HRESULT(1, FACILITY_ITF, 0x0015)
#define S_EVAL_ENC_OUTDATED              	MAKE_HRESULT(0, FACILITY_ITF, 0x0016)
#define S_EVAL_PRIOREVALTIMEDOUT				MAKE_HRESULT(0, FACILITY_ITF, 0x0017)
#define E_EVAL_OBJECT_ID_NOT_IN_APP_DOMAIN	MAKE_HRESULT(0, FACILITY_ITF, 0x0018)


extern RPC_IF_HANDLE __MIDL_itf_ee_0000_0000_v0_0_c_ifspec;
extern RPC_IF_HANDLE __MIDL_itf_ee_0000_0000_v0_0_s_ifspec;

#ifndef __IDebugExpressionEvaluator_INTERFACE_DEFINED__
#define __IDebugExpressionEvaluator_INTERFACE_DEFINED__

/* interface IDebugExpressionEvaluator */
/* [unique][uuid][object] */ 


EXTERN_C const IID IID_IDebugExpressionEvaluator;

#if defined(__cplusplus) && !defined(CINTERFACE)
    
    MIDL_INTERFACE("C077C822-476C-11d2-B73C-0000F87572EF")
    IDebugExpressionEvaluator : public IUnknown
    {
    public:
        virtual HRESULT STDMETHODCALLTYPE Parse( 
            /* [in] */ __RPC__in LPCOLESTR upstrExpression,
            /* [in] */ PARSEFLAGS dwFlags,
            /* [in] */ UINT nRadix,
            /* [out] */ __RPC__deref_out_opt BSTR *pbstrError,
            /* [out] */ __RPC__out UINT *pichError,
            /* [out] */ __RPC__deref_out_opt IDebugParsedExpression **ppParsedExpression) = 0;
        
        virtual HRESULT STDMETHODCALLTYPE GetMethodProperty( 
            /* [in] */ __RPC__in_opt IDebugSymbolProvider *pSymbolProvider,
            /* [in] */ __RPC__in_opt IDebugAddress *pAddress,
            /* [in] */ __RPC__in_opt IDebugBinder *pBinder,
            /* [in] */ BOOL fIncludeHiddenLocals,
            /* [out] */ __RPC__deref_out_opt IDebugProperty2 **ppProperty) = 0;
        
        virtual HRESULT STDMETHODCALLTYPE GetMethodLocationProperty( 
            /* [in] */ __RPC__in LPCOLESTR upstrFullyQualifiedMethodPlusOffset,
            /* [in] */ __RPC__in_opt IDebugSymbolProvider *pSymbolProvider,
            /* [in] */ __RPC__in_opt IDebugAddress *pAddress,
            /* [in] */ __RPC__in_opt IDebugBinder *pBinder,
            /* [out] */ __RPC__deref_out_opt IDebugProperty2 **ppProperty) = 0;
        
        virtual HRESULT STDMETHODCALLTYPE SetLocale( 
            /* [in] */ WORD wLangID) = 0;
        
        virtual HRESULT STDMETHODCALLTYPE SetRegistryRoot( 
            /* [in] */ __RPC__in LPCOLESTR ustrRegistryRoot) = 0;
        
    };
    
#else 	/* C style interface */

    typedef struct IDebugExpressionEvaluatorVtbl
    {
        BEGIN_INTERFACE
        
        HRESULT ( STDMETHODCALLTYPE *QueryInterface )( 
            IDebugExpressionEvaluator * This,
            /* [in] */ __RPC__in REFIID riid,
            /* [iid_is][out] */ 
            __RPC__deref_out  void **ppvObject);
        
        ULONG ( STDMETHODCALLTYPE *AddRef )( 
            IDebugExpressionEvaluator * This);
        
        ULONG ( STDMETHODCALLTYPE *Release )( 
            IDebugExpressionEvaluator * This);
        
        HRESULT ( STDMETHODCALLTYPE *Parse )( 
            IDebugExpressionEvaluator * This,
            /* [in] */ __RPC__in LPCOLESTR upstrExpression,
            /* [in] */ PARSEFLAGS dwFlags,
            /* [in] */ UINT nRadix,
            /* [out] */ __RPC__deref_out_opt BSTR *pbstrError,
            /* [out] */ __RPC__out UINT *pichError,
            /* [out] */ __RPC__deref_out_opt IDebugParsedExpression **ppParsedExpression);
        
        HRESULT ( STDMETHODCALLTYPE *GetMethodProperty )( 
            IDebugExpressionEvaluator * This,
            /* [in] */ __RPC__in_opt IDebugSymbolProvider *pSymbolProvider,
            /* [in] */ __RPC__in_opt IDebugAddress *pAddress,
            /* [in] */ __RPC__in_opt IDebugBinder *pBinder,
            /* [in] */ BOOL fIncludeHiddenLocals,
            /* [out] */ __RPC__deref_out_opt IDebugProperty2 **ppProperty);
        
        HRESULT ( STDMETHODCALLTYPE *GetMethodLocationProperty )( 
            IDebugExpressionEvaluator * This,
            /* [in] */ __RPC__in LPCOLESTR upstrFullyQualifiedMethodPlusOffset,
            /* [in] */ __RPC__in_opt IDebugSymbolProvider *pSymbolProvider,
            /* [in] */ __RPC__in_opt IDebugAddress *pAddress,
            /* [in] */ __RPC__in_opt IDebugBinder *pBinder,
            /* [out] */ __RPC__deref_out_opt IDebugProperty2 **ppProperty);
        
        HRESULT ( STDMETHODCALLTYPE *SetLocale )( 
            IDebugExpressionEvaluator * This,
            /* [in] */ WORD wLangID);
        
        HRESULT ( STDMETHODCALLTYPE *SetRegistryRoot )( 
            IDebugExpressionEvaluator * This,
            /* [in] */ __RPC__in LPCOLESTR ustrRegistryRoot);
        
        END_INTERFACE
    } IDebugExpressionEvaluatorVtbl;

    interface IDebugExpressionEvaluator
    {
        CONST_VTBL struct IDebugExpressionEvaluatorVtbl *lpVtbl;
    };

    

#ifdef COBJMACROS


#define IDebugExpressionEvaluator_QueryInterface(This,riid,ppvObject)	\
    ( (This)->lpVtbl -> QueryInterface(This,riid,ppvObject) ) 

#define IDebugExpressionEvaluator_AddRef(This)	\
    ( (This)->lpVtbl -> AddRef(This) ) 

#define IDebugExpressionEvaluator_Release(This)	\
    ( (This)->lpVtbl -> Release(This) ) 


#define IDebugExpressionEvaluator_Parse(This,upstrExpression,dwFlags,nRadix,pbstrError,pichError,ppParsedExpression)	\
    ( (This)->lpVtbl -> Parse(This,upstrExpression,dwFlags,nRadix,pbstrError,pichError,ppParsedExpression) ) 

#define IDebugExpressionEvaluator_GetMethodProperty(This,pSymbolProvider,pAddress,pBinder,fIncludeHiddenLocals,ppProperty)	\
    ( (This)->lpVtbl -> GetMethodProperty(This,pSymbolProvider,pAddress,pBinder,fIncludeHiddenLocals,ppProperty) ) 

#define IDebugExpressionEvaluator_GetMethodLocationProperty(This,upstrFullyQualifiedMethodPlusOffset,pSymbolProvider,pAddress,pBinder,ppProperty)	\
    ( (This)->lpVtbl -> GetMethodLocationProperty(This,upstrFullyQualifiedMethodPlusOffset,pSymbolProvider,pAddress,pBinder,ppProperty) ) 

#define IDebugExpressionEvaluator_SetLocale(This,wLangID)	\
    ( (This)->lpVtbl -> SetLocale(This,wLangID) ) 

#define IDebugExpressionEvaluator_SetRegistryRoot(This,ustrRegistryRoot)	\
    ( (This)->lpVtbl -> SetRegistryRoot(This,ustrRegistryRoot) ) 

#endif /* COBJMACROS */


#endif 	/* C style interface */




#endif 	/* __IDebugExpressionEvaluator_INTERFACE_DEFINED__ */


#ifndef __IDebugExpressionEvaluator2_INTERFACE_DEFINED__
#define __IDebugExpressionEvaluator2_INTERFACE_DEFINED__

/* interface IDebugExpressionEvaluator2 */
/* [unique][uuid][object] */ 


EXTERN_C const IID IID_IDebugExpressionEvaluator2;

#if defined(__cplusplus) && !defined(CINTERFACE)
    
    MIDL_INTERFACE("2DE1D5E0-CA57-456f-815C-5902825A2795")
    IDebugExpressionEvaluator2 : public IDebugExpressionEvaluator
    {
    public:
        virtual HRESULT STDMETHODCALLTYPE SetCorPath( 
            /* [in] */ __RPC__in LPCOLESTR pcstrCorPath) = 0;
        
        virtual HRESULT STDMETHODCALLTYPE Terminate( void) = 0;
        
        virtual HRESULT STDMETHODCALLTYPE SetCallback( 
            /* [in] */ __RPC__in_opt IDebugSettingsCallback2 *pCallback) = 0;
        
        virtual HRESULT STDMETHODCALLTYPE PreloadModules( 
            /* [in] */ __RPC__in_opt IDebugSymbolProvider *pSym) = 0;
        
        virtual HRESULT STDMETHODCALLTYPE GetService( 
            /* [in] */ GUID uid,
            /* [out] */ __RPC__deref_out_opt IUnknown **ppService) = 0;
        
        virtual HRESULT STDMETHODCALLTYPE SetIDebugIDECallback( 
            /* [in] */ __RPC__in_opt IDebugIDECallback *pCallback) = 0;
        
    };
    
#else 	/* C style interface */

    typedef struct IDebugExpressionEvaluator2Vtbl
    {
        BEGIN_INTERFACE
        
        HRESULT ( STDMETHODCALLTYPE *QueryInterface )( 
            IDebugExpressionEvaluator2 * This,
            /* [in] */ __RPC__in REFIID riid,
            /* [iid_is][out] */ 
            __RPC__deref_out  void **ppvObject);
        
        ULONG ( STDMETHODCALLTYPE *AddRef )( 
            IDebugExpressionEvaluator2 * This);
        
        ULONG ( STDMETHODCALLTYPE *Release )( 
            IDebugExpressionEvaluator2 * This);
        
        HRESULT ( STDMETHODCALLTYPE *Parse )( 
            IDebugExpressionEvaluator2 * This,
            /* [in] */ __RPC__in LPCOLESTR upstrExpression,
            /* [in] */ PARSEFLAGS dwFlags,
            /* [in] */ UINT nRadix,
            /* [out] */ __RPC__deref_out_opt BSTR *pbstrError,
            /* [out] */ __RPC__out UINT *pichError,
            /* [out] */ __RPC__deref_out_opt IDebugParsedExpression **ppParsedExpression);
        
        HRESULT ( STDMETHODCALLTYPE *GetMethodProperty )( 
            IDebugExpressionEvaluator2 * This,
            /* [in] */ __RPC__in_opt IDebugSymbolProvider *pSymbolProvider,
            /* [in] */ __RPC__in_opt IDebugAddress *pAddress,
            /* [in] */ __RPC__in_opt IDebugBinder *pBinder,
            /* [in] */ BOOL fIncludeHiddenLocals,
            /* [out] */ __RPC__deref_out_opt IDebugProperty2 **ppProperty);
        
        HRESULT ( STDMETHODCALLTYPE *GetMethodLocationProperty )( 
            IDebugExpressionEvaluator2 * This,
            /* [in] */ __RPC__in LPCOLESTR upstrFullyQualifiedMethodPlusOffset,
            /* [in] */ __RPC__in_opt IDebugSymbolProvider *pSymbolProvider,
            /* [in] */ __RPC__in_opt IDebugAddress *pAddress,
            /* [in] */ __RPC__in_opt IDebugBinder *pBinder,
            /* [out] */ __RPC__deref_out_opt IDebugProperty2 **ppProperty);
        
        HRESULT ( STDMETHODCALLTYPE *SetLocale )( 
            IDebugExpressionEvaluator2 * This,
            /* [in] */ WORD wLangID);
        
        HRESULT ( STDMETHODCALLTYPE *SetRegistryRoot )( 
            IDebugExpressionEvaluator2 * This,
            /* [in] */ __RPC__in LPCOLESTR ustrRegistryRoot);
        
        HRESULT ( STDMETHODCALLTYPE *SetCorPath )( 
            IDebugExpressionEvaluator2 * This,
            /* [in] */ __RPC__in LPCOLESTR pcstrCorPath);
        
        HRESULT ( STDMETHODCALLTYPE *Terminate )( 
            IDebugExpressionEvaluator2 * This);
        
        HRESULT ( STDMETHODCALLTYPE *SetCallback )( 
            IDebugExpressionEvaluator2 * This,
            /* [in] */ __RPC__in_opt IDebugSettingsCallback2 *pCallback);
        
        HRESULT ( STDMETHODCALLTYPE *PreloadModules )( 
            IDebugExpressionEvaluator2 * This,
            /* [in] */ __RPC__in_opt IDebugSymbolProvider *pSym);
        
        HRESULT ( STDMETHODCALLTYPE *GetService )( 
            IDebugExpressionEvaluator2 * This,
            /* [in] */ GUID uid,
            /* [out] */ __RPC__deref_out_opt IUnknown **ppService);
        
        HRESULT ( STDMETHODCALLTYPE *SetIDebugIDECallback )( 
            IDebugExpressionEvaluator2 * This,
            /* [in] */ __RPC__in_opt IDebugIDECallback *pCallback);
        
        END_INTERFACE
    } IDebugExpressionEvaluator2Vtbl;

    interface IDebugExpressionEvaluator2
    {
        CONST_VTBL struct IDebugExpressionEvaluator2Vtbl *lpVtbl;
    };

    

#ifdef COBJMACROS


#define IDebugExpressionEvaluator2_QueryInterface(This,riid,ppvObject)	\
    ( (This)->lpVtbl -> QueryInterface(This,riid,ppvObject) ) 

#define IDebugExpressionEvaluator2_AddRef(This)	\
    ( (This)->lpVtbl -> AddRef(This) ) 

#define IDebugExpressionEvaluator2_Release(This)	\
    ( (This)->lpVtbl -> Release(This) ) 


#define IDebugExpressionEvaluator2_Parse(This,upstrExpression,dwFlags,nRadix,pbstrError,pichError,ppParsedExpression)	\
    ( (This)->lpVtbl -> Parse(This,upstrExpression,dwFlags,nRadix,pbstrError,pichError,ppParsedExpression) ) 

#define IDebugExpressionEvaluator2_GetMethodProperty(This,pSymbolProvider,pAddress,pBinder,fIncludeHiddenLocals,ppProperty)	\
    ( (This)->lpVtbl -> GetMethodProperty(This,pSymbolProvider,pAddress,pBinder,fIncludeHiddenLocals,ppProperty) ) 

#define IDebugExpressionEvaluator2_GetMethodLocationProperty(This,upstrFullyQualifiedMethodPlusOffset,pSymbolProvider,pAddress,pBinder,ppProperty)	\
    ( (This)->lpVtbl -> GetMethodLocationProperty(This,upstrFullyQualifiedMethodPlusOffset,pSymbolProvider,pAddress,pBinder,ppProperty) ) 

#define IDebugExpressionEvaluator2_SetLocale(This,wLangID)	\
    ( (This)->lpVtbl -> SetLocale(This,wLangID) ) 

#define IDebugExpressionEvaluator2_SetRegistryRoot(This,ustrRegistryRoot)	\
    ( (This)->lpVtbl -> SetRegistryRoot(This,ustrRegistryRoot) ) 


#define IDebugExpressionEvaluator2_SetCorPath(This,pcstrCorPath)	\
    ( (This)->lpVtbl -> SetCorPath(This,pcstrCorPath) ) 

#define IDebugExpressionEvaluator2_Terminate(This)	\
    ( (This)->lpVtbl -> Terminate(This) ) 

#define IDebugExpressionEvaluator2_SetCallback(This,pCallback)	\
    ( (This)->lpVtbl -> SetCallback(This,pCallback) ) 

#define IDebugExpressionEvaluator2_PreloadModules(This,pSym)	\
    ( (This)->lpVtbl -> PreloadModules(This,pSym) ) 

#define IDebugExpressionEvaluator2_GetService(This,uid,ppService)	\
    ( (This)->lpVtbl -> GetService(This,uid,ppService) ) 

#define IDebugExpressionEvaluator2_SetIDebugIDECallback(This,pCallback)	\
    ( (This)->lpVtbl -> SetIDebugIDECallback(This,pCallback) ) 

#endif /* COBJMACROS */


#endif 	/* C style interface */




#endif 	/* __IDebugExpressionEvaluator2_INTERFACE_DEFINED__ */


#ifndef __IDebugExpressionEvaluator3_INTERFACE_DEFINED__
#define __IDebugExpressionEvaluator3_INTERFACE_DEFINED__

/* interface IDebugExpressionEvaluator3 */
/* [unique][uuid][object] */ 


EXTERN_C const IID IID_IDebugExpressionEvaluator3;

#if defined(__cplusplus) && !defined(CINTERFACE)
    
    MIDL_INTERFACE("4C7EC6F5-BB6C-43a2-853C-80FF48B7A8A6")
    IDebugExpressionEvaluator3 : public IDebugExpressionEvaluator2
    {
    public:
        virtual HRESULT STDMETHODCALLTYPE Parse2( 
            /* [in] */ __RPC__in LPCOLESTR upstrExpression,
            /* [in] */ PARSEFLAGS dwFlags,
            /* [in] */ UINT nRadix,
            /* [in] */ __RPC__in_opt IDebugSymbolProvider *pSymbolProvider,
            /* [in] */ __RPC__in_opt IDebugAddress *pAddress,
            /* [out] */ __RPC__deref_out_opt BSTR *pbstrError,
            /* [out] */ __RPC__out UINT *pichError,
            /* [out] */ __RPC__deref_out_opt IDebugParsedExpression **ppParsedExpression) = 0;
        
    };
    
#else 	/* C style interface */

    typedef struct IDebugExpressionEvaluator3Vtbl
    {
        BEGIN_INTERFACE
        
        HRESULT ( STDMETHODCALLTYPE *QueryInterface )( 
            IDebugExpressionEvaluator3 * This,
            /* [in] */ __RPC__in REFIID riid,
            /* [iid_is][out] */ 
            __RPC__deref_out  void **ppvObject);
        
        ULONG ( STDMETHODCALLTYPE *AddRef )( 
            IDebugExpressionEvaluator3 * This);
        
        ULONG ( STDMETHODCALLTYPE *Release )( 
            IDebugExpressionEvaluator3 * This);
        
        HRESULT ( STDMETHODCALLTYPE *Parse )( 
            IDebugExpressionEvaluator3 * This,
            /* [in] */ __RPC__in LPCOLESTR upstrExpression,
            /* [in] */ PARSEFLAGS dwFlags,
            /* [in] */ UINT nRadix,
            /* [out] */ __RPC__deref_out_opt BSTR *pbstrError,
            /* [out] */ __RPC__out UINT *pichError,
            /* [out] */ __RPC__deref_out_opt IDebugParsedExpression **ppParsedExpression);
        
        HRESULT ( STDMETHODCALLTYPE *GetMethodProperty )( 
            IDebugExpressionEvaluator3 * This,
            /* [in] */ __RPC__in_opt IDebugSymbolProvider *pSymbolProvider,
            /* [in] */ __RPC__in_opt IDebugAddress *pAddress,
            /* [in] */ __RPC__in_opt IDebugBinder *pBinder,
            /* [in] */ BOOL fIncludeHiddenLocals,
            /* [out] */ __RPC__deref_out_opt IDebugProperty2 **ppProperty);
        
        HRESULT ( STDMETHODCALLTYPE *GetMethodLocationProperty )( 
            IDebugExpressionEvaluator3 * This,
            /* [in] */ __RPC__in LPCOLESTR upstrFullyQualifiedMethodPlusOffset,
            /* [in] */ __RPC__in_opt IDebugSymbolProvider *pSymbolProvider,
            /* [in] */ __RPC__in_opt IDebugAddress *pAddress,
            /* [in] */ __RPC__in_opt IDebugBinder *pBinder,
            /* [out] */ __RPC__deref_out_opt IDebugProperty2 **ppProperty);
        
        HRESULT ( STDMETHODCALLTYPE *SetLocale )( 
            IDebugExpressionEvaluator3 * This,
            /* [in] */ WORD wLangID);
        
        HRESULT ( STDMETHODCALLTYPE *SetRegistryRoot )( 
            IDebugExpressionEvaluator3 * This,
            /* [in] */ __RPC__in LPCOLESTR ustrRegistryRoot);
        
        HRESULT ( STDMETHODCALLTYPE *SetCorPath )( 
            IDebugExpressionEvaluator3 * This,
            /* [in] */ __RPC__in LPCOLESTR pcstrCorPath);
        
        HRESULT ( STDMETHODCALLTYPE *Terminate )( 
            IDebugExpressionEvaluator3 * This);
        
        HRESULT ( STDMETHODCALLTYPE *SetCallback )( 
            IDebugExpressionEvaluator3 * This,
            /* [in] */ __RPC__in_opt IDebugSettingsCallback2 *pCallback);
        
        HRESULT ( STDMETHODCALLTYPE *PreloadModules )( 
            IDebugExpressionEvaluator3 * This,
            /* [in] */ __RPC__in_opt IDebugSymbolProvider *pSym);
        
        HRESULT ( STDMETHODCALLTYPE *GetService )( 
            IDebugExpressionEvaluator3 * This,
            /* [in] */ GUID uid,
            /* [out] */ __RPC__deref_out_opt IUnknown **ppService);
        
        HRESULT ( STDMETHODCALLTYPE *SetIDebugIDECallback )( 
            IDebugExpressionEvaluator3 * This,
            /* [in] */ __RPC__in_opt IDebugIDECallback *pCallback);
        
        HRESULT ( STDMETHODCALLTYPE *Parse2 )( 
            IDebugExpressionEvaluator3 * This,
            /* [in] */ __RPC__in LPCOLESTR upstrExpression,
            /* [in] */ PARSEFLAGS dwFlags,
            /* [in] */ UINT nRadix,
            /* [in] */ __RPC__in_opt IDebugSymbolProvider *pSymbolProvider,
            /* [in] */ __RPC__in_opt IDebugAddress *pAddress,
            /* [out] */ __RPC__deref_out_opt BSTR *pbstrError,
            /* [out] */ __RPC__out UINT *pichError,
            /* [out] */ __RPC__deref_out_opt IDebugParsedExpression **ppParsedExpression);
        
        END_INTERFACE
    } IDebugExpressionEvaluator3Vtbl;

    interface IDebugExpressionEvaluator3
    {
        CONST_VTBL struct IDebugExpressionEvaluator3Vtbl *lpVtbl;
    };

    

#ifdef COBJMACROS


#define IDebugExpressionEvaluator3_QueryInterface(This,riid,ppvObject)	\
    ( (This)->lpVtbl -> QueryInterface(This,riid,ppvObject) ) 

#define IDebugExpressionEvaluator3_AddRef(This)	\
    ( (This)->lpVtbl -> AddRef(This) ) 

#define IDebugExpressionEvaluator3_Release(This)	\
    ( (This)->lpVtbl -> Release(This) ) 


#define IDebugExpressionEvaluator3_Parse(This,upstrExpression,dwFlags,nRadix,pbstrError,pichError,ppParsedExpression)	\
    ( (This)->lpVtbl -> Parse(This,upstrExpression,dwFlags,nRadix,pbstrError,pichError,ppParsedExpression) ) 

#define IDebugExpressionEvaluator3_GetMethodProperty(This,pSymbolProvider,pAddress,pBinder,fIncludeHiddenLocals,ppProperty)	\
    ( (This)->lpVtbl -> GetMethodProperty(This,pSymbolProvider,pAddress,pBinder,fIncludeHiddenLocals,ppProperty) ) 

#define IDebugExpressionEvaluator3_GetMethodLocationProperty(This,upstrFullyQualifiedMethodPlusOffset,pSymbolProvider,pAddress,pBinder,ppProperty)	\
    ( (This)->lpVtbl -> GetMethodLocationProperty(This,upstrFullyQualifiedMethodPlusOffset,pSymbolProvider,pAddress,pBinder,ppProperty) ) 

#define IDebugExpressionEvaluator3_SetLocale(This,wLangID)	\
    ( (This)->lpVtbl -> SetLocale(This,wLangID) ) 

#define IDebugExpressionEvaluator3_SetRegistryRoot(This,ustrRegistryRoot)	\
    ( (This)->lpVtbl -> SetRegistryRoot(This,ustrRegistryRoot) ) 


#define IDebugExpressionEvaluator3_SetCorPath(This,pcstrCorPath)	\
    ( (This)->lpVtbl -> SetCorPath(This,pcstrCorPath) ) 

#define IDebugExpressionEvaluator3_Terminate(This)	\
    ( (This)->lpVtbl -> Terminate(This) ) 

#define IDebugExpressionEvaluator3_SetCallback(This,pCallback)	\
    ( (This)->lpVtbl -> SetCallback(This,pCallback) ) 

#define IDebugExpressionEvaluator3_PreloadModules(This,pSym)	\
    ( (This)->lpVtbl -> PreloadModules(This,pSym) ) 

#define IDebugExpressionEvaluator3_GetService(This,uid,ppService)	\
    ( (This)->lpVtbl -> GetService(This,uid,ppService) ) 

#define IDebugExpressionEvaluator3_SetIDebugIDECallback(This,pCallback)	\
    ( (This)->lpVtbl -> SetIDebugIDECallback(This,pCallback) ) 


#define IDebugExpressionEvaluator3_Parse2(This,upstrExpression,dwFlags,nRadix,pSymbolProvider,pAddress,pbstrError,pichError,ppParsedExpression)	\
    ( (This)->lpVtbl -> Parse2(This,upstrExpression,dwFlags,nRadix,pSymbolProvider,pAddress,pbstrError,pichError,ppParsedExpression) ) 

#endif /* COBJMACROS */


#endif 	/* C style interface */




#endif 	/* __IDebugExpressionEvaluator3_INTERFACE_DEFINED__ */


#ifndef __IDebugIDECallback_INTERFACE_DEFINED__
#define __IDebugIDECallback_INTERFACE_DEFINED__

/* interface IDebugIDECallback */
/* [unique][uuid][object] */ 


EXTERN_C const IID IID_IDebugIDECallback;

#if defined(__cplusplus) && !defined(CINTERFACE)
    
    MIDL_INTERFACE("B78C9E91-DD39-4e5b-BB7B-30B88149B2FE")
    IDebugIDECallback : public IUnknown
    {
    public:
        virtual HRESULT STDMETHODCALLTYPE DisplayMessage( 
            /* [in] */ __RPC__in LPCOLESTR szMessage) = 0;
        
    };
    
#else 	/* C style interface */

    typedef struct IDebugIDECallbackVtbl
    {
        BEGIN_INTERFACE
        
        HRESULT ( STDMETHODCALLTYPE *QueryInterface )( 
            IDebugIDECallback * This,
            /* [in] */ __RPC__in REFIID riid,
            /* [iid_is][out] */ 
            __RPC__deref_out  void **ppvObject);
        
        ULONG ( STDMETHODCALLTYPE *AddRef )( 
            IDebugIDECallback * This);
        
        ULONG ( STDMETHODCALLTYPE *Release )( 
            IDebugIDECallback * This);
        
        HRESULT ( STDMETHODCALLTYPE *DisplayMessage )( 
            IDebugIDECallback * This,
            /* [in] */ __RPC__in LPCOLESTR szMessage);
        
        END_INTERFACE
    } IDebugIDECallbackVtbl;

    interface IDebugIDECallback
    {
        CONST_VTBL struct IDebugIDECallbackVtbl *lpVtbl;
    };

    

#ifdef COBJMACROS


#define IDebugIDECallback_QueryInterface(This,riid,ppvObject)	\
    ( (This)->lpVtbl -> QueryInterface(This,riid,ppvObject) ) 

#define IDebugIDECallback_AddRef(This)	\
    ( (This)->lpVtbl -> AddRef(This) ) 

#define IDebugIDECallback_Release(This)	\
    ( (This)->lpVtbl -> Release(This) ) 


#define IDebugIDECallback_DisplayMessage(This,szMessage)	\
    ( (This)->lpVtbl -> DisplayMessage(This,szMessage) ) 

#endif /* COBJMACROS */


#endif 	/* C style interface */




#endif 	/* __IDebugIDECallback_INTERFACE_DEFINED__ */


#ifndef __IDebugIteratorFrameProvider_INTERFACE_DEFINED__
#define __IDebugIteratorFrameProvider_INTERFACE_DEFINED__

/* interface IDebugIteratorFrameProvider */
/* [unique][uuid][object] */ 


EXTERN_C const IID IID_IDebugIteratorFrameProvider;

#if defined(__cplusplus) && !defined(CINTERFACE)
    
    MIDL_INTERFACE("A38EF241-AF3E-49a9-8533-0E35B6794D40")
    IDebugIteratorFrameProvider : public IUnknown
    {
    public:
        virtual HRESULT STDMETHODCALLTYPE GetIteratorFrames( 
            /* [in] */ __RPC__in_opt IDebugAddress *pAddress,
            /* [in] */ __RPC__in_opt IDebugBinderDirect *pBinder,
            /* [in] */ __RPC__in_opt IDebugComPlusSymbolProvider *pSym,
            /* [out] */ __RPC__deref_out_opt IDebugProperty2 **ppProperty) = 0;
        
    };
    
#else 	/* C style interface */

    typedef struct IDebugIteratorFrameProviderVtbl
    {
        BEGIN_INTERFACE
        
        HRESULT ( STDMETHODCALLTYPE *QueryInterface )( 
            IDebugIteratorFrameProvider * This,
            /* [in] */ __RPC__in REFIID riid,
            /* [iid_is][out] */ 
            __RPC__deref_out  void **ppvObject);
        
        ULONG ( STDMETHODCALLTYPE *AddRef )( 
            IDebugIteratorFrameProvider * This);
        
        ULONG ( STDMETHODCALLTYPE *Release )( 
            IDebugIteratorFrameProvider * This);
        
        HRESULT ( STDMETHODCALLTYPE *GetIteratorFrames )( 
            IDebugIteratorFrameProvider * This,
            /* [in] */ __RPC__in_opt IDebugAddress *pAddress,
            /* [in] */ __RPC__in_opt IDebugBinderDirect *pBinder,
            /* [in] */ __RPC__in_opt IDebugComPlusSymbolProvider *pSym,
            /* [out] */ __RPC__deref_out_opt IDebugProperty2 **ppProperty);
        
        END_INTERFACE
    } IDebugIteratorFrameProviderVtbl;

    interface IDebugIteratorFrameProvider
    {
        CONST_VTBL struct IDebugIteratorFrameProviderVtbl *lpVtbl;
    };

    

#ifdef COBJMACROS


#define IDebugIteratorFrameProvider_QueryInterface(This,riid,ppvObject)	\
    ( (This)->lpVtbl -> QueryInterface(This,riid,ppvObject) ) 

#define IDebugIteratorFrameProvider_AddRef(This)	\
    ( (This)->lpVtbl -> AddRef(This) ) 

#define IDebugIteratorFrameProvider_Release(This)	\
    ( (This)->lpVtbl -> Release(This) ) 


#define IDebugIteratorFrameProvider_GetIteratorFrames(This,pAddress,pBinder,pSym,ppProperty)	\
    ( (This)->lpVtbl -> GetIteratorFrames(This,pAddress,pBinder,pSym,ppProperty) ) 

#endif /* COBJMACROS */


#endif 	/* C style interface */




#endif 	/* __IDebugIteratorFrameProvider_INTERFACE_DEFINED__ */


#ifndef __IDebugObject_INTERFACE_DEFINED__
#define __IDebugObject_INTERFACE_DEFINED__

/* interface IDebugObject */
/* [unique][uuid][object] */ 


EXTERN_C const IID IID_IDebugObject;

#if defined(__cplusplus) && !defined(CINTERFACE)
    
    MIDL_INTERFACE("C077C823-476C-11d2-B73C-0000F87572EF")
    IDebugObject : public IUnknown
    {
    public:
        virtual HRESULT STDMETHODCALLTYPE GetSize( 
            /* [out] */ __RPC__out UINT *pnSize) = 0;
        
        virtual HRESULT STDMETHODCALLTYPE GetValue( 
            /* [length_is][size_is][out] */ __RPC__out_ecount_part(nSize, nSize) BYTE *pValue,
            /* [in] */ UINT nSize) = 0;
        
        virtual HRESULT STDMETHODCALLTYPE SetValue( 
            /* [size_is][in] */ __RPC__in_ecount_full(nSize) BYTE *pValue,
            /* [in] */ UINT nSize) = 0;
        
        virtual HRESULT STDMETHODCALLTYPE SetReferenceValue( 
            /* [in] */ __RPC__in_opt IDebugObject *pObject) = 0;
        
        virtual HRESULT STDMETHODCALLTYPE GetMemoryContext( 
            __RPC__deref_in_opt IDebugMemoryContext2 **pContext) = 0;
        
        virtual HRESULT STDMETHODCALLTYPE GetManagedDebugObject( 
            /* [out] */ __RPC__deref_out_opt IDebugManagedObject **ppObject) = 0;
        
        virtual HRESULT STDMETHODCALLTYPE IsNullReference( 
            /* [out] */ __RPC__out BOOL *pfIsNull) = 0;
        
        virtual HRESULT STDMETHODCALLTYPE IsEqual( 
            /* [in] */ __RPC__in_opt IDebugObject *pObject,
            /* [out] */ __RPC__out BOOL *pfIsEqual) = 0;
        
        virtual HRESULT STDMETHODCALLTYPE IsReadOnly( 
            /* [out] */ __RPC__out BOOL *pfIsReadOnly) = 0;
        
        virtual HRESULT STDMETHODCALLTYPE IsProxy( 
            /* [out] */ __RPC__out BOOL *pfIsProxy) = 0;
        
    };
    
#else 	/* C style interface */

    typedef struct IDebugObjectVtbl
    {
        BEGIN_INTERFACE
        
        HRESULT ( STDMETHODCALLTYPE *QueryInterface )( 
            IDebugObject * This,
            /* [in] */ __RPC__in REFIID riid,
            /* [iid_is][out] */ 
            __RPC__deref_out  void **ppvObject);
        
        ULONG ( STDMETHODCALLTYPE *AddRef )( 
            IDebugObject * This);
        
        ULONG ( STDMETHODCALLTYPE *Release )( 
            IDebugObject * This);
        
        HRESULT ( STDMETHODCALLTYPE *GetSize )( 
            IDebugObject * This,
            /* [out] */ __RPC__out UINT *pnSize);
        
        HRESULT ( STDMETHODCALLTYPE *GetValue )( 
            IDebugObject * This,
            /* [length_is][size_is][out] */ __RPC__out_ecount_part(nSize, nSize) BYTE *pValue,
            /* [in] */ UINT nSize);
        
        HRESULT ( STDMETHODCALLTYPE *SetValue )( 
            IDebugObject * This,
            /* [size_is][in] */ __RPC__in_ecount_full(nSize) BYTE *pValue,
            /* [in] */ UINT nSize);
        
        HRESULT ( STDMETHODCALLTYPE *SetReferenceValue )( 
            IDebugObject * This,
            /* [in] */ __RPC__in_opt IDebugObject *pObject);
        
        HRESULT ( STDMETHODCALLTYPE *GetMemoryContext )( 
            IDebugObject * This,
            __RPC__deref_in_opt IDebugMemoryContext2 **pContext);
        
        HRESULT ( STDMETHODCALLTYPE *GetManagedDebugObject )( 
            IDebugObject * This,
            /* [out] */ __RPC__deref_out_opt IDebugManagedObject **ppObject);
        
        HRESULT ( STDMETHODCALLTYPE *IsNullReference )( 
            IDebugObject * This,
            /* [out] */ __RPC__out BOOL *pfIsNull);
        
        HRESULT ( STDMETHODCALLTYPE *IsEqual )( 
            IDebugObject * This,
            /* [in] */ __RPC__in_opt IDebugObject *pObject,
            /* [out] */ __RPC__out BOOL *pfIsEqual);
        
        HRESULT ( STDMETHODCALLTYPE *IsReadOnly )( 
            IDebugObject * This,
            /* [out] */ __RPC__out BOOL *pfIsReadOnly);
        
        HRESULT ( STDMETHODCALLTYPE *IsProxy )( 
            IDebugObject * This,
            /* [out] */ __RPC__out BOOL *pfIsProxy);
        
        END_INTERFACE
    } IDebugObjectVtbl;

    interface IDebugObject
    {
        CONST_VTBL struct IDebugObjectVtbl *lpVtbl;
    };

    

#ifdef COBJMACROS


#define IDebugObject_QueryInterface(This,riid,ppvObject)	\
    ( (This)->lpVtbl -> QueryInterface(This,riid,ppvObject) ) 

#define IDebugObject_AddRef(This)	\
    ( (This)->lpVtbl -> AddRef(This) ) 

#define IDebugObject_Release(This)	\
    ( (This)->lpVtbl -> Release(This) ) 


#define IDebugObject_GetSize(This,pnSize)	\
    ( (This)->lpVtbl -> GetSize(This,pnSize) ) 

#define IDebugObject_GetValue(This,pValue,nSize)	\
    ( (This)->lpVtbl -> GetValue(This,pValue,nSize) ) 

#define IDebugObject_SetValue(This,pValue,nSize)	\
    ( (This)->lpVtbl -> SetValue(This,pValue,nSize) ) 

#define IDebugObject_SetReferenceValue(This,pObject)	\
    ( (This)->lpVtbl -> SetReferenceValue(This,pObject) ) 

#define IDebugObject_GetMemoryContext(This,pContext)	\
    ( (This)->lpVtbl -> GetMemoryContext(This,pContext) ) 

#define IDebugObject_GetManagedDebugObject(This,ppObject)	\
    ( (This)->lpVtbl -> GetManagedDebugObject(This,ppObject) ) 

#define IDebugObject_IsNullReference(This,pfIsNull)	\
    ( (This)->lpVtbl -> IsNullReference(This,pfIsNull) ) 

#define IDebugObject_IsEqual(This,pObject,pfIsEqual)	\
    ( (This)->lpVtbl -> IsEqual(This,pObject,pfIsEqual) ) 

#define IDebugObject_IsReadOnly(This,pfIsReadOnly)	\
    ( (This)->lpVtbl -> IsReadOnly(This,pfIsReadOnly) ) 

#define IDebugObject_IsProxy(This,pfIsProxy)	\
    ( (This)->lpVtbl -> IsProxy(This,pfIsProxy) ) 

#endif /* COBJMACROS */


#endif 	/* C style interface */




#endif 	/* __IDebugObject_INTERFACE_DEFINED__ */


#ifndef __IDebugObject2_INTERFACE_DEFINED__
#define __IDebugObject2_INTERFACE_DEFINED__

/* interface IDebugObject2 */
/* [unique][uuid][object] */ 


EXTERN_C const IID IID_IDebugObject2;

#if defined(__cplusplus) && !defined(CINTERFACE)
    
    MIDL_INTERFACE("3FF130FC-B14F-4bae-AE44-46B1CD3928CC")
    IDebugObject2 : public IDebugObject
    {
    public:
        virtual HRESULT STDMETHODCALLTYPE GetBackingFieldForProperty( 
            /* [out] */ __RPC__deref_out_opt IDebugObject2 **ppObject) = 0;
        
        virtual HRESULT STDMETHODCALLTYPE GetICorDebugValue( 
            /* [out] */ __RPC__deref_out_opt IUnknown **ppUnk) = 0;
        
        virtual HRESULT STDMETHODCALLTYPE CreateAlias( 
            /* [out] */ __RPC__deref_out_opt IDebugAlias **ppAlias) = 0;
        
        virtual HRESULT STDMETHODCALLTYPE GetAlias( 
            /* [out] */ __RPC__deref_out_opt IDebugAlias **ppAlias) = 0;
        
        virtual HRESULT STDMETHODCALLTYPE GetField( 
            /* [out] */ __RPC__deref_out_opt IDebugField **ppField) = 0;
        
        virtual HRESULT STDMETHODCALLTYPE IsUserData( 
            /* [out] */ __RPC__out BOOL *pfUser) = 0;
        
        virtual HRESULT STDMETHODCALLTYPE IsEncOutdated( 
            /* [out] */ __RPC__out BOOL *pfEncOutdated) = 0;
        
    };
    
#else 	/* C style interface */

    typedef struct IDebugObject2Vtbl
    {
        BEGIN_INTERFACE
        
        HRESULT ( STDMETHODCALLTYPE *QueryInterface )( 
            IDebugObject2 * This,
            /* [in] */ __RPC__in REFIID riid,
            /* [iid_is][out] */ 
            __RPC__deref_out  void **ppvObject);
        
        ULONG ( STDMETHODCALLTYPE *AddRef )( 
            IDebugObject2 * This);
        
        ULONG ( STDMETHODCALLTYPE *Release )( 
            IDebugObject2 * This);
        
        HRESULT ( STDMETHODCALLTYPE *GetSize )( 
            IDebugObject2 * This,
            /* [out] */ __RPC__out UINT *pnSize);
        
        HRESULT ( STDMETHODCALLTYPE *GetValue )( 
            IDebugObject2 * This,
            /* [length_is][size_is][out] */ __RPC__out_ecount_part(nSize, nSize) BYTE *pValue,
            /* [in] */ UINT nSize);
        
        HRESULT ( STDMETHODCALLTYPE *SetValue )( 
            IDebugObject2 * This,
            /* [size_is][in] */ __RPC__in_ecount_full(nSize) BYTE *pValue,
            /* [in] */ UINT nSize);
        
        HRESULT ( STDMETHODCALLTYPE *SetReferenceValue )( 
            IDebugObject2 * This,
            /* [in] */ __RPC__in_opt IDebugObject *pObject);
        
        HRESULT ( STDMETHODCALLTYPE *GetMemoryContext )( 
            IDebugObject2 * This,
            __RPC__deref_in_opt IDebugMemoryContext2 **pContext);
        
        HRESULT ( STDMETHODCALLTYPE *GetManagedDebugObject )( 
            IDebugObject2 * This,
            /* [out] */ __RPC__deref_out_opt IDebugManagedObject **ppObject);
        
        HRESULT ( STDMETHODCALLTYPE *IsNullReference )( 
            IDebugObject2 * This,
            /* [out] */ __RPC__out BOOL *pfIsNull);
        
        HRESULT ( STDMETHODCALLTYPE *IsEqual )( 
            IDebugObject2 * This,
            /* [in] */ __RPC__in_opt IDebugObject *pObject,
            /* [out] */ __RPC__out BOOL *pfIsEqual);
        
        HRESULT ( STDMETHODCALLTYPE *IsReadOnly )( 
            IDebugObject2 * This,
            /* [out] */ __RPC__out BOOL *pfIsReadOnly);
        
        HRESULT ( STDMETHODCALLTYPE *IsProxy )( 
            IDebugObject2 * This,
            /* [out] */ __RPC__out BOOL *pfIsProxy);
        
        HRESULT ( STDMETHODCALLTYPE *GetBackingFieldForProperty )( 
            IDebugObject2 * This,
            /* [out] */ __RPC__deref_out_opt IDebugObject2 **ppObject);
        
        HRESULT ( STDMETHODCALLTYPE *GetICorDebugValue )( 
            IDebugObject2 * This,
            /* [out] */ __RPC__deref_out_opt IUnknown **ppUnk);
        
        HRESULT ( STDMETHODCALLTYPE *CreateAlias )( 
            IDebugObject2 * This,
            /* [out] */ __RPC__deref_out_opt IDebugAlias **ppAlias);
        
        HRESULT ( STDMETHODCALLTYPE *GetAlias )( 
            IDebugObject2 * This,
            /* [out] */ __RPC__deref_out_opt IDebugAlias **ppAlias);
        
        HRESULT ( STDMETHODCALLTYPE *GetField )( 
            IDebugObject2 * This,
            /* [out] */ __RPC__deref_out_opt IDebugField **ppField);
        
        HRESULT ( STDMETHODCALLTYPE *IsUserData )( 
            IDebugObject2 * This,
            /* [out] */ __RPC__out BOOL *pfUser);
        
        HRESULT ( STDMETHODCALLTYPE *IsEncOutdated )( 
            IDebugObject2 * This,
            /* [out] */ __RPC__out BOOL *pfEncOutdated);
        
        END_INTERFACE
    } IDebugObject2Vtbl;

    interface IDebugObject2
    {
        CONST_VTBL struct IDebugObject2Vtbl *lpVtbl;
    };

    

#ifdef COBJMACROS


#define IDebugObject2_QueryInterface(This,riid,ppvObject)	\
    ( (This)->lpVtbl -> QueryInterface(This,riid,ppvObject) ) 

#define IDebugObject2_AddRef(This)	\
    ( (This)->lpVtbl -> AddRef(This) ) 

#define IDebugObject2_Release(This)	\
    ( (This)->lpVtbl -> Release(This) ) 


#define IDebugObject2_GetSize(This,pnSize)	\
    ( (This)->lpVtbl -> GetSize(This,pnSize) ) 

#define IDebugObject2_GetValue(This,pValue,nSize)	\
    ( (This)->lpVtbl -> GetValue(This,pValue,nSize) ) 

#define IDebugObject2_SetValue(This,pValue,nSize)	\
    ( (This)->lpVtbl -> SetValue(This,pValue,nSize) ) 

#define IDebugObject2_SetReferenceValue(This,pObject)	\
    ( (This)->lpVtbl -> SetReferenceValue(This,pObject) ) 

#define IDebugObject2_GetMemoryContext(This,pContext)	\
    ( (This)->lpVtbl -> GetMemoryContext(This,pContext) ) 

#define IDebugObject2_GetManagedDebugObject(This,ppObject)	\
    ( (This)->lpVtbl -> GetManagedDebugObject(This,ppObject) ) 

#define IDebugObject2_IsNullReference(This,pfIsNull)	\
    ( (This)->lpVtbl -> IsNullReference(This,pfIsNull) ) 

#define IDebugObject2_IsEqual(This,pObject,pfIsEqual)	\
    ( (This)->lpVtbl -> IsEqual(This,pObject,pfIsEqual) ) 

#define IDebugObject2_IsReadOnly(This,pfIsReadOnly)	\
    ( (This)->lpVtbl -> IsReadOnly(This,pfIsReadOnly) ) 

#define IDebugObject2_IsProxy(This,pfIsProxy)	\
    ( (This)->lpVtbl -> IsProxy(This,pfIsProxy) ) 


#define IDebugObject2_GetBackingFieldForProperty(This,ppObject)	\
    ( (This)->lpVtbl -> GetBackingFieldForProperty(This,ppObject) ) 

#define IDebugObject2_GetICorDebugValue(This,ppUnk)	\
    ( (This)->lpVtbl -> GetICorDebugValue(This,ppUnk) ) 

#define IDebugObject2_CreateAlias(This,ppAlias)	\
    ( (This)->lpVtbl -> CreateAlias(This,ppAlias) ) 

#define IDebugObject2_GetAlias(This,ppAlias)	\
    ( (This)->lpVtbl -> GetAlias(This,ppAlias) ) 

#define IDebugObject2_GetField(This,ppField)	\
    ( (This)->lpVtbl -> GetField(This,ppField) ) 

#define IDebugObject2_IsUserData(This,pfUser)	\
    ( (This)->lpVtbl -> IsUserData(This,pfUser) ) 

#define IDebugObject2_IsEncOutdated(This,pfEncOutdated)	\
    ( (This)->lpVtbl -> IsEncOutdated(This,pfEncOutdated) ) 

#endif /* COBJMACROS */


#endif 	/* C style interface */




#endif 	/* __IDebugObject2_INTERFACE_DEFINED__ */


#ifndef __IDebugArrayObject_INTERFACE_DEFINED__
#define __IDebugArrayObject_INTERFACE_DEFINED__

/* interface IDebugArrayObject */
/* [unique][uuid][object] */ 


EXTERN_C const IID IID_IDebugArrayObject;

#if defined(__cplusplus) && !defined(CINTERFACE)
    
    MIDL_INTERFACE("29ECD774-75AE-11d2-B74E-0000F87572EF")
    IDebugArrayObject : public IDebugObject
    {
    public:
        virtual HRESULT STDMETHODCALLTYPE GetCount( 
            /* [out] */ __RPC__out DWORD *pdwElements) = 0;
        
        virtual HRESULT STDMETHODCALLTYPE GetElement( 
            /* [in] */ DWORD dwIndex,
            /* [out] */ __RPC__deref_out_opt IDebugObject **ppElement) = 0;
        
        virtual HRESULT STDMETHODCALLTYPE GetElements( 
            /* [out] */ __RPC__deref_out_opt IEnumDebugObjects **ppEnum) = 0;
        
        virtual HRESULT STDMETHODCALLTYPE GetRank( 
            /* [out] */ __RPC__out DWORD *pdwRank) = 0;
        
        virtual HRESULT STDMETHODCALLTYPE GetDimensions( 
            /* [in] */ DWORD dwCount,
            /* [length_is][size_is][out] */ __RPC__out_ecount_part(dwCount, dwCount) DWORD *dwDimensions) = 0;
        
    };
    
#else 	/* C style interface */

    typedef struct IDebugArrayObjectVtbl
    {
        BEGIN_INTERFACE
        
        HRESULT ( STDMETHODCALLTYPE *QueryInterface )( 
            IDebugArrayObject * This,
            /* [in] */ __RPC__in REFIID riid,
            /* [iid_is][out] */ 
            __RPC__deref_out  void **ppvObject);
        
        ULONG ( STDMETHODCALLTYPE *AddRef )( 
            IDebugArrayObject * This);
        
        ULONG ( STDMETHODCALLTYPE *Release )( 
            IDebugArrayObject * This);
        
        HRESULT ( STDMETHODCALLTYPE *GetSize )( 
            IDebugArrayObject * This,
            /* [out] */ __RPC__out UINT *pnSize);
        
        HRESULT ( STDMETHODCALLTYPE *GetValue )( 
            IDebugArrayObject * This,
            /* [length_is][size_is][out] */ __RPC__out_ecount_part(nSize, nSize) BYTE *pValue,
            /* [in] */ UINT nSize);
        
        HRESULT ( STDMETHODCALLTYPE *SetValue )( 
            IDebugArrayObject * This,
            /* [size_is][in] */ __RPC__in_ecount_full(nSize) BYTE *pValue,
            /* [in] */ UINT nSize);
        
        HRESULT ( STDMETHODCALLTYPE *SetReferenceValue )( 
            IDebugArrayObject * This,
            /* [in] */ __RPC__in_opt IDebugObject *pObject);
        
        HRESULT ( STDMETHODCALLTYPE *GetMemoryContext )( 
            IDebugArrayObject * This,
            __RPC__deref_in_opt IDebugMemoryContext2 **pContext);
        
        HRESULT ( STDMETHODCALLTYPE *GetManagedDebugObject )( 
            IDebugArrayObject * This,
            /* [out] */ __RPC__deref_out_opt IDebugManagedObject **ppObject);
        
        HRESULT ( STDMETHODCALLTYPE *IsNullReference )( 
            IDebugArrayObject * This,
            /* [out] */ __RPC__out BOOL *pfIsNull);
        
        HRESULT ( STDMETHODCALLTYPE *IsEqual )( 
            IDebugArrayObject * This,
            /* [in] */ __RPC__in_opt IDebugObject *pObject,
            /* [out] */ __RPC__out BOOL *pfIsEqual);
        
        HRESULT ( STDMETHODCALLTYPE *IsReadOnly )( 
            IDebugArrayObject * This,
            /* [out] */ __RPC__out BOOL *pfIsReadOnly);
        
        HRESULT ( STDMETHODCALLTYPE *IsProxy )( 
            IDebugArrayObject * This,
            /* [out] */ __RPC__out BOOL *pfIsProxy);
        
        HRESULT ( STDMETHODCALLTYPE *GetCount )( 
            IDebugArrayObject * This,
            /* [out] */ __RPC__out DWORD *pdwElements);
        
        HRESULT ( STDMETHODCALLTYPE *GetElement )( 
            IDebugArrayObject * This,
            /* [in] */ DWORD dwIndex,
            /* [out] */ __RPC__deref_out_opt IDebugObject **ppElement);
        
        HRESULT ( STDMETHODCALLTYPE *GetElements )( 
            IDebugArrayObject * This,
            /* [out] */ __RPC__deref_out_opt IEnumDebugObjects **ppEnum);
        
        HRESULT ( STDMETHODCALLTYPE *GetRank )( 
            IDebugArrayObject * This,
            /* [out] */ __RPC__out DWORD *pdwRank);
        
        HRESULT ( STDMETHODCALLTYPE *GetDimensions )( 
            IDebugArrayObject * This,
            /* [in] */ DWORD dwCount,
            /* [length_is][size_is][out] */ __RPC__out_ecount_part(dwCount, dwCount) DWORD *dwDimensions);
        
        END_INTERFACE
    } IDebugArrayObjectVtbl;

    interface IDebugArrayObject
    {
        CONST_VTBL struct IDebugArrayObjectVtbl *lpVtbl;
    };

    

#ifdef COBJMACROS


#define IDebugArrayObject_QueryInterface(This,riid,ppvObject)	\
    ( (This)->lpVtbl -> QueryInterface(This,riid,ppvObject) ) 

#define IDebugArrayObject_AddRef(This)	\
    ( (This)->lpVtbl -> AddRef(This) ) 

#define IDebugArrayObject_Release(This)	\
    ( (This)->lpVtbl -> Release(This) ) 


#define IDebugArrayObject_GetSize(This,pnSize)	\
    ( (This)->lpVtbl -> GetSize(This,pnSize) ) 

#define IDebugArrayObject_GetValue(This,pValue,nSize)	\
    ( (This)->lpVtbl -> GetValue(This,pValue,nSize) ) 

#define IDebugArrayObject_SetValue(This,pValue,nSize)	\
    ( (This)->lpVtbl -> SetValue(This,pValue,nSize) ) 

#define IDebugArrayObject_SetReferenceValue(This,pObject)	\
    ( (This)->lpVtbl -> SetReferenceValue(This,pObject) ) 

#define IDebugArrayObject_GetMemoryContext(This,pContext)	\
    ( (This)->lpVtbl -> GetMemoryContext(This,pContext) ) 

#define IDebugArrayObject_GetManagedDebugObject(This,ppObject)	\
    ( (This)->lpVtbl -> GetManagedDebugObject(This,ppObject) ) 

#define IDebugArrayObject_IsNullReference(This,pfIsNull)	\
    ( (This)->lpVtbl -> IsNullReference(This,pfIsNull) ) 

#define IDebugArrayObject_IsEqual(This,pObject,pfIsEqual)	\
    ( (This)->lpVtbl -> IsEqual(This,pObject,pfIsEqual) ) 

#define IDebugArrayObject_IsReadOnly(This,pfIsReadOnly)	\
    ( (This)->lpVtbl -> IsReadOnly(This,pfIsReadOnly) ) 

#define IDebugArrayObject_IsProxy(This,pfIsProxy)	\
    ( (This)->lpVtbl -> IsProxy(This,pfIsProxy) ) 


#define IDebugArrayObject_GetCount(This,pdwElements)	\
    ( (This)->lpVtbl -> GetCount(This,pdwElements) ) 

#define IDebugArrayObject_GetElement(This,dwIndex,ppElement)	\
    ( (This)->lpVtbl -> GetElement(This,dwIndex,ppElement) ) 

#define IDebugArrayObject_GetElements(This,ppEnum)	\
    ( (This)->lpVtbl -> GetElements(This,ppEnum) ) 

#define IDebugArrayObject_GetRank(This,pdwRank)	\
    ( (This)->lpVtbl -> GetRank(This,pdwRank) ) 

#define IDebugArrayObject_GetDimensions(This,dwCount,dwDimensions)	\
    ( (This)->lpVtbl -> GetDimensions(This,dwCount,dwDimensions) ) 

#endif /* COBJMACROS */


#endif 	/* C style interface */




#endif 	/* __IDebugArrayObject_INTERFACE_DEFINED__ */


#ifndef __IDebugArrayObject2_INTERFACE_DEFINED__
#define __IDebugArrayObject2_INTERFACE_DEFINED__

/* interface IDebugArrayObject2 */
/* [unique][uuid][object] */ 


EXTERN_C const IID IID_IDebugArrayObject2;

#if defined(__cplusplus) && !defined(CINTERFACE)
    
    MIDL_INTERFACE("41487E33-9A10-42fe-BA3B-15FDE59D09D5")
    IDebugArrayObject2 : public IDebugArrayObject
    {
    public:
        virtual HRESULT STDMETHODCALLTYPE HasBaseIndices( 
            /* [out] */ __RPC__out BOOL *pfHasBaseIndices) = 0;
        
        virtual HRESULT STDMETHODCALLTYPE GetBaseIndices( 
            /* [in] */ DWORD dwRank,
            /* [length_is][size_is][out] */ __RPC__out_ecount_part(dwRank, dwRank) DWORD *dwIndices) = 0;
        
    };
    
#else 	/* C style interface */

    typedef struct IDebugArrayObject2Vtbl
    {
        BEGIN_INTERFACE
        
        HRESULT ( STDMETHODCALLTYPE *QueryInterface )( 
            IDebugArrayObject2 * This,
            /* [in] */ __RPC__in REFIID riid,
            /* [iid_is][out] */ 
            __RPC__deref_out  void **ppvObject);
        
        ULONG ( STDMETHODCALLTYPE *AddRef )( 
            IDebugArrayObject2 * This);
        
        ULONG ( STDMETHODCALLTYPE *Release )( 
            IDebugArrayObject2 * This);
        
        HRESULT ( STDMETHODCALLTYPE *GetSize )( 
            IDebugArrayObject2 * This,
            /* [out] */ __RPC__out UINT *pnSize);
        
        HRESULT ( STDMETHODCALLTYPE *GetValue )( 
            IDebugArrayObject2 * This,
            /* [length_is][size_is][out] */ __RPC__out_ecount_part(nSize, nSize) BYTE *pValue,
            /* [in] */ UINT nSize);
        
        HRESULT ( STDMETHODCALLTYPE *SetValue )( 
            IDebugArrayObject2 * This,
            /* [size_is][in] */ __RPC__in_ecount_full(nSize) BYTE *pValue,
            /* [in] */ UINT nSize);
        
        HRESULT ( STDMETHODCALLTYPE *SetReferenceValue )( 
            IDebugArrayObject2 * This,
            /* [in] */ __RPC__in_opt IDebugObject *pObject);
        
        HRESULT ( STDMETHODCALLTYPE *GetMemoryContext )( 
            IDebugArrayObject2 * This,
            __RPC__deref_in_opt IDebugMemoryContext2 **pContext);
        
        HRESULT ( STDMETHODCALLTYPE *GetManagedDebugObject )( 
            IDebugArrayObject2 * This,
            /* [out] */ __RPC__deref_out_opt IDebugManagedObject **ppObject);
        
        HRESULT ( STDMETHODCALLTYPE *IsNullReference )( 
            IDebugArrayObject2 * This,
            /* [out] */ __RPC__out BOOL *pfIsNull);
        
        HRESULT ( STDMETHODCALLTYPE *IsEqual )( 
            IDebugArrayObject2 * This,
            /* [in] */ __RPC__in_opt IDebugObject *pObject,
            /* [out] */ __RPC__out BOOL *pfIsEqual);
        
        HRESULT ( STDMETHODCALLTYPE *IsReadOnly )( 
            IDebugArrayObject2 * This,
            /* [out] */ __RPC__out BOOL *pfIsReadOnly);
        
        HRESULT ( STDMETHODCALLTYPE *IsProxy )( 
            IDebugArrayObject2 * This,
            /* [out] */ __RPC__out BOOL *pfIsProxy);
        
        HRESULT ( STDMETHODCALLTYPE *GetCount )( 
            IDebugArrayObject2 * This,
            /* [out] */ __RPC__out DWORD *pdwElements);
        
        HRESULT ( STDMETHODCALLTYPE *GetElement )( 
            IDebugArrayObject2 * This,
            /* [in] */ DWORD dwIndex,
            /* [out] */ __RPC__deref_out_opt IDebugObject **ppElement);
        
        HRESULT ( STDMETHODCALLTYPE *GetElements )( 
            IDebugArrayObject2 * This,
            /* [out] */ __RPC__deref_out_opt IEnumDebugObjects **ppEnum);
        
        HRESULT ( STDMETHODCALLTYPE *GetRank )( 
            IDebugArrayObject2 * This,
            /* [out] */ __RPC__out DWORD *pdwRank);
        
        HRESULT ( STDMETHODCALLTYPE *GetDimensions )( 
            IDebugArrayObject2 * This,
            /* [in] */ DWORD dwCount,
            /* [length_is][size_is][out] */ __RPC__out_ecount_part(dwCount, dwCount) DWORD *dwDimensions);
        
        HRESULT ( STDMETHODCALLTYPE *HasBaseIndices )( 
            IDebugArrayObject2 * This,
            /* [out] */ __RPC__out BOOL *pfHasBaseIndices);
        
        HRESULT ( STDMETHODCALLTYPE *GetBaseIndices )( 
            IDebugArrayObject2 * This,
            /* [in] */ DWORD dwRank,
            /* [length_is][size_is][out] */ __RPC__out_ecount_part(dwRank, dwRank) DWORD *dwIndices);
        
        END_INTERFACE
    } IDebugArrayObject2Vtbl;

    interface IDebugArrayObject2
    {
        CONST_VTBL struct IDebugArrayObject2Vtbl *lpVtbl;
    };

    

#ifdef COBJMACROS


#define IDebugArrayObject2_QueryInterface(This,riid,ppvObject)	\
    ( (This)->lpVtbl -> QueryInterface(This,riid,ppvObject) ) 

#define IDebugArrayObject2_AddRef(This)	\
    ( (This)->lpVtbl -> AddRef(This) ) 

#define IDebugArrayObject2_Release(This)	\
    ( (This)->lpVtbl -> Release(This) ) 


#define IDebugArrayObject2_GetSize(This,pnSize)	\
    ( (This)->lpVtbl -> GetSize(This,pnSize) ) 

#define IDebugArrayObject2_GetValue(This,pValue,nSize)	\
    ( (This)->lpVtbl -> GetValue(This,pValue,nSize) ) 

#define IDebugArrayObject2_SetValue(This,pValue,nSize)	\
    ( (This)->lpVtbl -> SetValue(This,pValue,nSize) ) 

#define IDebugArrayObject2_SetReferenceValue(This,pObject)	\
    ( (This)->lpVtbl -> SetReferenceValue(This,pObject) ) 

#define IDebugArrayObject2_GetMemoryContext(This,pContext)	\
    ( (This)->lpVtbl -> GetMemoryContext(This,pContext) ) 

#define IDebugArrayObject2_GetManagedDebugObject(This,ppObject)	\
    ( (This)->lpVtbl -> GetManagedDebugObject(This,ppObject) ) 

#define IDebugArrayObject2_IsNullReference(This,pfIsNull)	\
    ( (This)->lpVtbl -> IsNullReference(This,pfIsNull) ) 

#define IDebugArrayObject2_IsEqual(This,pObject,pfIsEqual)	\
    ( (This)->lpVtbl -> IsEqual(This,pObject,pfIsEqual) ) 

#define IDebugArrayObject2_IsReadOnly(This,pfIsReadOnly)	\
    ( (This)->lpVtbl -> IsReadOnly(This,pfIsReadOnly) ) 

#define IDebugArrayObject2_IsProxy(This,pfIsProxy)	\
    ( (This)->lpVtbl -> IsProxy(This,pfIsProxy) ) 


#define IDebugArrayObject2_GetCount(This,pdwElements)	\
    ( (This)->lpVtbl -> GetCount(This,pdwElements) ) 

#define IDebugArrayObject2_GetElement(This,dwIndex,ppElement)	\
    ( (This)->lpVtbl -> GetElement(This,dwIndex,ppElement) ) 

#define IDebugArrayObject2_GetElements(This,ppEnum)	\
    ( (This)->lpVtbl -> GetElements(This,ppEnum) ) 

#define IDebugArrayObject2_GetRank(This,pdwRank)	\
    ( (This)->lpVtbl -> GetRank(This,pdwRank) ) 

#define IDebugArrayObject2_GetDimensions(This,dwCount,dwDimensions)	\
    ( (This)->lpVtbl -> GetDimensions(This,dwCount,dwDimensions) ) 


#define IDebugArrayObject2_HasBaseIndices(This,pfHasBaseIndices)	\
    ( (This)->lpVtbl -> HasBaseIndices(This,pfHasBaseIndices) ) 

#define IDebugArrayObject2_GetBaseIndices(This,dwRank,dwIndices)	\
    ( (This)->lpVtbl -> GetBaseIndices(This,dwRank,dwIndices) ) 

#endif /* COBJMACROS */


#endif 	/* C style interface */




#endif 	/* __IDebugArrayObject2_INTERFACE_DEFINED__ */


#ifndef __IDebugFunctionObject_INTERFACE_DEFINED__
#define __IDebugFunctionObject_INTERFACE_DEFINED__

/* interface IDebugFunctionObject */
/* [unique][uuid][object] */ 


enum enum_OBJECT_TYPE
    {	OBJECT_TYPE_BOOLEAN	= 0,
	OBJECT_TYPE_CHAR	= 0x1,
	OBJECT_TYPE_I1	= 0x2,
	OBJECT_TYPE_U1	= 0x3,
	OBJECT_TYPE_I2	= 0x4,
	OBJECT_TYPE_U2	= 0x5,
	OBJECT_TYPE_I4	= 0x6,
	OBJECT_TYPE_U4	= 0x7,
	OBJECT_TYPE_I8	= 0x8,
	OBJECT_TYPE_U8	= 0x9,
	OBJECT_TYPE_R4	= 0xa,
	OBJECT_TYPE_R8	= 0xb,
	OBJECT_TYPE_OBJECT	= 0xc,
	OBJECT_TYPE_NULL	= 0xd,
	OBJECT_TYPE_CLASS	= 0xe
    } ;
typedef DWORD OBJECT_TYPE;


EXTERN_C const IID IID_IDebugFunctionObject;

#if defined(__cplusplus) && !defined(CINTERFACE)
    
    MIDL_INTERFACE("F71D9EA0-4269-48dc-9E8D-F86DEFA042B3")
    IDebugFunctionObject : public IDebugObject
    {
    public:
        virtual HRESULT STDMETHODCALLTYPE CreatePrimitiveObject( 
            /* [in] */ OBJECT_TYPE ot,
            /* [out] */ __RPC__deref_out_opt IDebugObject **ppObject) = 0;
        
        virtual HRESULT STDMETHODCALLTYPE CreateObject( 
            /* [in] */ __RPC__in_opt IDebugFunctionObject *pConstructor,
            /* [in] */ DWORD dwArgs,
            /* [size_is][in] */ __RPC__in_ecount_full(dwArgs) IDebugObject *pArgs[  ],
            /* [out] */ __RPC__deref_out_opt IDebugObject **ppObject) = 0;
        
        virtual HRESULT STDMETHODCALLTYPE CreateObjectNoConstructor( 
            /* [in] */ __RPC__in_opt IDebugField *pClassField,
            /* [out] */ __RPC__deref_out_opt IDebugObject **ppObject) = 0;
        
        virtual HRESULT STDMETHODCALLTYPE CreateArrayObject( 
            /* [in] */ OBJECT_TYPE ot,
            /* [in] */ __RPC__in_opt IDebugField *pClassField,
            /* [in] */ DWORD dwRank,
            /* [size_is][in] */ __RPC__in_ecount_full(dwRank) DWORD dwDims[  ],
            /* [size_is][in] */ __RPC__in_ecount_full(dwRank) DWORD dwLowBounds[  ],
            /* [out] */ __RPC__deref_out_opt IDebugObject **ppObject) = 0;
        
        virtual HRESULT STDMETHODCALLTYPE CreateStringObject( 
            /* [in] */ __RPC__in LPCOLESTR pcstrString,
            /* [out] */ __RPC__deref_out_opt IDebugObject **ppOjbect) = 0;
        
        virtual HRESULT STDMETHODCALLTYPE Evaluate( 
            /* [size_is][in] */ __RPC__in_ecount_full(dwParams) IDebugObject **ppParams,
            /* [in] */ DWORD dwParams,
            /* [in] */ DWORD dwTimeout,
            /* [out] */ __RPC__deref_out_opt IDebugObject **ppResult) = 0;
        
    };
    
#else 	/* C style interface */

    typedef struct IDebugFunctionObjectVtbl
    {
        BEGIN_INTERFACE
        
        HRESULT ( STDMETHODCALLTYPE *QueryInterface )( 
            IDebugFunctionObject * This,
            /* [in] */ __RPC__in REFIID riid,
            /* [iid_is][out] */ 
            __RPC__deref_out  void **ppvObject);
        
        ULONG ( STDMETHODCALLTYPE *AddRef )( 
            IDebugFunctionObject * This);
        
        ULONG ( STDMETHODCALLTYPE *Release )( 
            IDebugFunctionObject * This);
        
        HRESULT ( STDMETHODCALLTYPE *GetSize )( 
            IDebugFunctionObject * This,
            /* [out] */ __RPC__out UINT *pnSize);
        
        HRESULT ( STDMETHODCALLTYPE *GetValue )( 
            IDebugFunctionObject * This,
            /* [length_is][size_is][out] */ __RPC__out_ecount_part(nSize, nSize) BYTE *pValue,
            /* [in] */ UINT nSize);
        
        HRESULT ( STDMETHODCALLTYPE *SetValue )( 
            IDebugFunctionObject * This,
            /* [size_is][in] */ __RPC__in_ecount_full(nSize) BYTE *pValue,
            /* [in] */ UINT nSize);
        
        HRESULT ( STDMETHODCALLTYPE *SetReferenceValue )( 
            IDebugFunctionObject * This,
            /* [in] */ __RPC__in_opt IDebugObject *pObject);
        
        HRESULT ( STDMETHODCALLTYPE *GetMemoryContext )( 
            IDebugFunctionObject * This,
            __RPC__deref_in_opt IDebugMemoryContext2 **pContext);
        
        HRESULT ( STDMETHODCALLTYPE *GetManagedDebugObject )( 
            IDebugFunctionObject * This,
            /* [out] */ __RPC__deref_out_opt IDebugManagedObject **ppObject);
        
        HRESULT ( STDMETHODCALLTYPE *IsNullReference )( 
            IDebugFunctionObject * This,
            /* [out] */ __RPC__out BOOL *pfIsNull);
        
        HRESULT ( STDMETHODCALLTYPE *IsEqual )( 
            IDebugFunctionObject * This,
            /* [in] */ __RPC__in_opt IDebugObject *pObject,
            /* [out] */ __RPC__out BOOL *pfIsEqual);
        
        HRESULT ( STDMETHODCALLTYPE *IsReadOnly )( 
            IDebugFunctionObject * This,
            /* [out] */ __RPC__out BOOL *pfIsReadOnly);
        
        HRESULT ( STDMETHODCALLTYPE *IsProxy )( 
            IDebugFunctionObject * This,
            /* [out] */ __RPC__out BOOL *pfIsProxy);
        
        HRESULT ( STDMETHODCALLTYPE *CreatePrimitiveObject )( 
            IDebugFunctionObject * This,
            /* [in] */ OBJECT_TYPE ot,
            /* [out] */ __RPC__deref_out_opt IDebugObject **ppObject);
        
        HRESULT ( STDMETHODCALLTYPE *CreateObject )( 
            IDebugFunctionObject * This,
            /* [in] */ __RPC__in_opt IDebugFunctionObject *pConstructor,
            /* [in] */ DWORD dwArgs,
            /* [size_is][in] */ __RPC__in_ecount_full(dwArgs) IDebugObject *pArgs[  ],
            /* [out] */ __RPC__deref_out_opt IDebugObject **ppObject);
        
        HRESULT ( STDMETHODCALLTYPE *CreateObjectNoConstructor )( 
            IDebugFunctionObject * This,
            /* [in] */ __RPC__in_opt IDebugField *pClassField,
            /* [out] */ __RPC__deref_out_opt IDebugObject **ppObject);
        
        HRESULT ( STDMETHODCALLTYPE *CreateArrayObject )( 
            IDebugFunctionObject * This,
            /* [in] */ OBJECT_TYPE ot,
            /* [in] */ __RPC__in_opt IDebugField *pClassField,
            /* [in] */ DWORD dwRank,
            /* [size_is][in] */ __RPC__in_ecount_full(dwRank) DWORD dwDims[  ],
            /* [size_is][in] */ __RPC__in_ecount_full(dwRank) DWORD dwLowBounds[  ],
            /* [out] */ __RPC__deref_out_opt IDebugObject **ppObject);
        
        HRESULT ( STDMETHODCALLTYPE *CreateStringObject )( 
            IDebugFunctionObject * This,
            /* [in] */ __RPC__in LPCOLESTR pcstrString,
            /* [out] */ __RPC__deref_out_opt IDebugObject **ppOjbect);
        
        HRESULT ( STDMETHODCALLTYPE *Evaluate )( 
            IDebugFunctionObject * This,
            /* [size_is][in] */ __RPC__in_ecount_full(dwParams) IDebugObject **ppParams,
            /* [in] */ DWORD dwParams,
            /* [in] */ DWORD dwTimeout,
            /* [out] */ __RPC__deref_out_opt IDebugObject **ppResult);
        
        END_INTERFACE
    } IDebugFunctionObjectVtbl;

    interface IDebugFunctionObject
    {
        CONST_VTBL struct IDebugFunctionObjectVtbl *lpVtbl;
    };

    

#ifdef COBJMACROS


#define IDebugFunctionObject_QueryInterface(This,riid,ppvObject)	\
    ( (This)->lpVtbl -> QueryInterface(This,riid,ppvObject) ) 

#define IDebugFunctionObject_AddRef(This)	\
    ( (This)->lpVtbl -> AddRef(This) ) 

#define IDebugFunctionObject_Release(This)	\
    ( (This)->lpVtbl -> Release(This) ) 


#define IDebugFunctionObject_GetSize(This,pnSize)	\
    ( (This)->lpVtbl -> GetSize(This,pnSize) ) 

#define IDebugFunctionObject_GetValue(This,pValue,nSize)	\
    ( (This)->lpVtbl -> GetValue(This,pValue,nSize) ) 

#define IDebugFunctionObject_SetValue(This,pValue,nSize)	\
    ( (This)->lpVtbl -> SetValue(This,pValue,nSize) ) 

#define IDebugFunctionObject_SetReferenceValue(This,pObject)	\
    ( (This)->lpVtbl -> SetReferenceValue(This,pObject) ) 

#define IDebugFunctionObject_GetMemoryContext(This,pContext)	\
    ( (This)->lpVtbl -> GetMemoryContext(This,pContext) ) 

#define IDebugFunctionObject_GetManagedDebugObject(This,ppObject)	\
    ( (This)->lpVtbl -> GetManagedDebugObject(This,ppObject) ) 

#define IDebugFunctionObject_IsNullReference(This,pfIsNull)	\
    ( (This)->lpVtbl -> IsNullReference(This,pfIsNull) ) 

#define IDebugFunctionObject_IsEqual(This,pObject,pfIsEqual)	\
    ( (This)->lpVtbl -> IsEqual(This,pObject,pfIsEqual) ) 

#define IDebugFunctionObject_IsReadOnly(This,pfIsReadOnly)	\
    ( (This)->lpVtbl -> IsReadOnly(This,pfIsReadOnly) ) 

#define IDebugFunctionObject_IsProxy(This,pfIsProxy)	\
    ( (This)->lpVtbl -> IsProxy(This,pfIsProxy) ) 


#define IDebugFunctionObject_CreatePrimitiveObject(This,ot,ppObject)	\
    ( (This)->lpVtbl -> CreatePrimitiveObject(This,ot,ppObject) ) 

#define IDebugFunctionObject_CreateObject(This,pConstructor,dwArgs,pArgs,ppObject)	\
    ( (This)->lpVtbl -> CreateObject(This,pConstructor,dwArgs,pArgs,ppObject) ) 

#define IDebugFunctionObject_CreateObjectNoConstructor(This,pClassField,ppObject)	\
    ( (This)->lpVtbl -> CreateObjectNoConstructor(This,pClassField,ppObject) ) 

#define IDebugFunctionObject_CreateArrayObject(This,ot,pClassField,dwRank,dwDims,dwLowBounds,ppObject)	\
    ( (This)->lpVtbl -> CreateArrayObject(This,ot,pClassField,dwRank,dwDims,dwLowBounds,ppObject) ) 

#define IDebugFunctionObject_CreateStringObject(This,pcstrString,ppOjbect)	\
    ( (This)->lpVtbl -> CreateStringObject(This,pcstrString,ppOjbect) ) 

#define IDebugFunctionObject_Evaluate(This,ppParams,dwParams,dwTimeout,ppResult)	\
    ( (This)->lpVtbl -> Evaluate(This,ppParams,dwParams,dwTimeout,ppResult) ) 

#endif /* COBJMACROS */


#endif 	/* C style interface */




#endif 	/* __IDebugFunctionObject_INTERFACE_DEFINED__ */


#ifndef __IDebugFunctionObject2_INTERFACE_DEFINED__
#define __IDebugFunctionObject2_INTERFACE_DEFINED__

/* interface IDebugFunctionObject2 */
/* [unique][uuid][object] */ 


EXTERN_C const IID IID_IDebugFunctionObject2;

#if defined(__cplusplus) && !defined(CINTERFACE)
    
    MIDL_INTERFACE("8E861CC7-D21C-43e7-AB7B-947921689B88")
    IDebugFunctionObject2 : public IUnknown
    {
    public:
        virtual HRESULT STDMETHODCALLTYPE Evaluate( 
            /* [size_is][in] */ __RPC__in_ecount_full(dwParams) IDebugObject **ppParams,
            /* [in] */ DWORD dwParams,
            /* [in] */ DWORD dwEvalFlags,
            /* [in] */ DWORD dwTimeout,
            /* [out] */ __RPC__deref_out_opt IDebugObject **ppResult) = 0;
        
        virtual HRESULT STDMETHODCALLTYPE CreateObject( 
            /* [in] */ __RPC__in_opt IDebugFunctionObject *pConstructor,
            /* [in] */ DWORD dwArgs,
            /* [size_is][in] */ __RPC__in_ecount_full(dwArgs) IDebugObject *pArgs[  ],
            /* [in] */ DWORD dwEvalFlags,
            /* [in] */ DWORD dwTimeout,
            /* [out] */ __RPC__deref_out_opt IDebugObject **ppObject) = 0;
        
        virtual HRESULT STDMETHODCALLTYPE CreateStringObjectWithLength( 
            /* [in] */ __RPC__in LPCOLESTR pcstrString,
            /* [in] */ UINT uiLength,
            /* [out] */ __RPC__deref_out_opt IDebugObject **ppObject) = 0;
        
    };
    
#else 	/* C style interface */

    typedef struct IDebugFunctionObject2Vtbl
    {
        BEGIN_INTERFACE
        
        HRESULT ( STDMETHODCALLTYPE *QueryInterface )( 
            IDebugFunctionObject2 * This,
            /* [in] */ __RPC__in REFIID riid,
            /* [iid_is][out] */ 
            __RPC__deref_out  void **ppvObject);
        
        ULONG ( STDMETHODCALLTYPE *AddRef )( 
            IDebugFunctionObject2 * This);
        
        ULONG ( STDMETHODCALLTYPE *Release )( 
            IDebugFunctionObject2 * This);
        
        HRESULT ( STDMETHODCALLTYPE *Evaluate )( 
            IDebugFunctionObject2 * This,
            /* [size_is][in] */ __RPC__in_ecount_full(dwParams) IDebugObject **ppParams,
            /* [in] */ DWORD dwParams,
            /* [in] */ DWORD dwEvalFlags,
            /* [in] */ DWORD dwTimeout,
            /* [out] */ __RPC__deref_out_opt IDebugObject **ppResult);
        
        HRESULT ( STDMETHODCALLTYPE *CreateObject )( 
            IDebugFunctionObject2 * This,
            /* [in] */ __RPC__in_opt IDebugFunctionObject *pConstructor,
            /* [in] */ DWORD dwArgs,
            /* [size_is][in] */ __RPC__in_ecount_full(dwArgs) IDebugObject *pArgs[  ],
            /* [in] */ DWORD dwEvalFlags,
            /* [in] */ DWORD dwTimeout,
            /* [out] */ __RPC__deref_out_opt IDebugObject **ppObject);
        
        HRESULT ( STDMETHODCALLTYPE *CreateStringObjectWithLength )( 
            IDebugFunctionObject2 * This,
            /* [in] */ __RPC__in LPCOLESTR pcstrString,
            /* [in] */ UINT uiLength,
            /* [out] */ __RPC__deref_out_opt IDebugObject **ppObject);
        
        END_INTERFACE
    } IDebugFunctionObject2Vtbl;

    interface IDebugFunctionObject2
    {
        CONST_VTBL struct IDebugFunctionObject2Vtbl *lpVtbl;
    };

    

#ifdef COBJMACROS


#define IDebugFunctionObject2_QueryInterface(This,riid,ppvObject)	\
    ( (This)->lpVtbl -> QueryInterface(This,riid,ppvObject) ) 

#define IDebugFunctionObject2_AddRef(This)	\
    ( (This)->lpVtbl -> AddRef(This) ) 

#define IDebugFunctionObject2_Release(This)	\
    ( (This)->lpVtbl -> Release(This) ) 


#define IDebugFunctionObject2_Evaluate(This,ppParams,dwParams,dwEvalFlags,dwTimeout,ppResult)	\
    ( (This)->lpVtbl -> Evaluate(This,ppParams,dwParams,dwEvalFlags,dwTimeout,ppResult) ) 

#define IDebugFunctionObject2_CreateObject(This,pConstructor,dwArgs,pArgs,dwEvalFlags,dwTimeout,ppObject)	\
    ( (This)->lpVtbl -> CreateObject(This,pConstructor,dwArgs,pArgs,dwEvalFlags,dwTimeout,ppObject) ) 

#define IDebugFunctionObject2_CreateStringObjectWithLength(This,pcstrString,uiLength,ppObject)	\
    ( (This)->lpVtbl -> CreateStringObjectWithLength(This,pcstrString,uiLength,ppObject) ) 

#endif /* COBJMACROS */


#endif 	/* C style interface */




#endif 	/* __IDebugFunctionObject2_INTERFACE_DEFINED__ */


#ifndef __IDebugManagedObject_INTERFACE_DEFINED__
#define __IDebugManagedObject_INTERFACE_DEFINED__

/* interface IDebugManagedObject */
/* [unique][uuid][object] */ 


EXTERN_C const IID IID_IDebugManagedObject;

#if defined(__cplusplus) && !defined(CINTERFACE)
    
    MIDL_INTERFACE("71AF87C9-66C5-49e4-A602-B9012115AFD5")
    IDebugManagedObject : public IUnknown
    {
    public:
        virtual HRESULT STDMETHODCALLTYPE GetManagedObject( 
            /* [out] */ __RPC__deref_out_opt IUnknown **ppManagedObject) = 0;
        
        virtual HRESULT STDMETHODCALLTYPE SetFromManagedObject( 
            /* [in] */ __RPC__in_opt IUnknown *pManagedObject) = 0;
        
    };
    
#else 	/* C style interface */

    typedef struct IDebugManagedObjectVtbl
    {
        BEGIN_INTERFACE
        
        HRESULT ( STDMETHODCALLTYPE *QueryInterface )( 
            IDebugManagedObject * This,
            /* [in] */ __RPC__in REFIID riid,
            /* [iid_is][out] */ 
            __RPC__deref_out  void **ppvObject);
        
        ULONG ( STDMETHODCALLTYPE *AddRef )( 
            IDebugManagedObject * This);
        
        ULONG ( STDMETHODCALLTYPE *Release )( 
            IDebugManagedObject * This);
        
        HRESULT ( STDMETHODCALLTYPE *GetManagedObject )( 
            IDebugManagedObject * This,
            /* [out] */ __RPC__deref_out_opt IUnknown **ppManagedObject);
        
        HRESULT ( STDMETHODCALLTYPE *SetFromManagedObject )( 
            IDebugManagedObject * This,
            /* [in] */ __RPC__in_opt IUnknown *pManagedObject);
        
        END_INTERFACE
    } IDebugManagedObjectVtbl;

    interface IDebugManagedObject
    {
        CONST_VTBL struct IDebugManagedObjectVtbl *lpVtbl;
    };

    

#ifdef COBJMACROS


#define IDebugManagedObject_QueryInterface(This,riid,ppvObject)	\
    ( (This)->lpVtbl -> QueryInterface(This,riid,ppvObject) ) 

#define IDebugManagedObject_AddRef(This)	\
    ( (This)->lpVtbl -> AddRef(This) ) 

#define IDebugManagedObject_Release(This)	\
    ( (This)->lpVtbl -> Release(This) ) 


#define IDebugManagedObject_GetManagedObject(This,ppManagedObject)	\
    ( (This)->lpVtbl -> GetManagedObject(This,ppManagedObject) ) 

#define IDebugManagedObject_SetFromManagedObject(This,pManagedObject)	\
    ( (This)->lpVtbl -> SetFromManagedObject(This,pManagedObject) ) 

#endif /* COBJMACROS */


#endif 	/* C style interface */




#endif 	/* __IDebugManagedObject_INTERFACE_DEFINED__ */


#ifndef __IDebugBinder_INTERFACE_DEFINED__
#define __IDebugBinder_INTERFACE_DEFINED__

/* interface IDebugBinder */
/* [unique][uuid][object] */ 


EXTERN_C const IID IID_IDebugBinder;

#if defined(__cplusplus) && !defined(CINTERFACE)
    
    MIDL_INTERFACE("C077C833-476C-11d2-B73C-0000F87572EF")
    IDebugBinder : public IUnknown
    {
    public:
        virtual HRESULT STDMETHODCALLTYPE Bind( 
            /* [in] */ __RPC__in_opt IDebugObject *pContainer,
            /* [in] */ __RPC__in_opt IDebugField *pField,
            /* [out] */ __RPC__deref_out_opt IDebugObject **ppObject) = 0;
        
        virtual HRESULT STDMETHODCALLTYPE ResolveDynamicType( 
            /* [in] */ __RPC__in_opt IDebugDynamicField *pDynamic,
            /* [out] */ __RPC__deref_out_opt IDebugField **ppResolved) = 0;
        
        virtual HRESULT STDMETHODCALLTYPE ResolveRuntimeType( 
            /* [in] */ __RPC__in_opt IDebugObject *pObject,
            /* [out] */ __RPC__deref_out_opt IDebugField **ppResolved) = 0;
        
        virtual HRESULT STDMETHODCALLTYPE GetMemoryContext( 
            /* [in] */ __RPC__in_opt IDebugField *pField,
            /* [in] */ DWORD dwConstant,
            /* [out] */ __RPC__deref_out_opt IDebugMemoryContext2 **ppMemCxt) = 0;
        
        virtual HRESULT STDMETHODCALLTYPE GetFunctionObject( 
            /* [out] */ __RPC__deref_out_opt IDebugFunctionObject **ppFunction) = 0;
        
    };
    
#else 	/* C style interface */

    typedef struct IDebugBinderVtbl
    {
        BEGIN_INTERFACE
        
        HRESULT ( STDMETHODCALLTYPE *QueryInterface )( 
            IDebugBinder * This,
            /* [in] */ __RPC__in REFIID riid,
            /* [iid_is][out] */ 
            __RPC__deref_out  void **ppvObject);
        
        ULONG ( STDMETHODCALLTYPE *AddRef )( 
            IDebugBinder * This);
        
        ULONG ( STDMETHODCALLTYPE *Release )( 
            IDebugBinder * This);
        
        HRESULT ( STDMETHODCALLTYPE *Bind )( 
            IDebugBinder * This,
            /* [in] */ __RPC__in_opt IDebugObject *pContainer,
            /* [in] */ __RPC__in_opt IDebugField *pField,
            /* [out] */ __RPC__deref_out_opt IDebugObject **ppObject);
        
        HRESULT ( STDMETHODCALLTYPE *ResolveDynamicType )( 
            IDebugBinder * This,
            /* [in] */ __RPC__in_opt IDebugDynamicField *pDynamic,
            /* [out] */ __RPC__deref_out_opt IDebugField **ppResolved);
        
        HRESULT ( STDMETHODCALLTYPE *ResolveRuntimeType )( 
            IDebugBinder * This,
            /* [in] */ __RPC__in_opt IDebugObject *pObject,
            /* [out] */ __RPC__deref_out_opt IDebugField **ppResolved);
        
        HRESULT ( STDMETHODCALLTYPE *GetMemoryContext )( 
            IDebugBinder * This,
            /* [in] */ __RPC__in_opt IDebugField *pField,
            /* [in] */ DWORD dwConstant,
            /* [out] */ __RPC__deref_out_opt IDebugMemoryContext2 **ppMemCxt);
        
        HRESULT ( STDMETHODCALLTYPE *GetFunctionObject )( 
            IDebugBinder * This,
            /* [out] */ __RPC__deref_out_opt IDebugFunctionObject **ppFunction);
        
        END_INTERFACE
    } IDebugBinderVtbl;

    interface IDebugBinder
    {
        CONST_VTBL struct IDebugBinderVtbl *lpVtbl;
    };

    

#ifdef COBJMACROS


#define IDebugBinder_QueryInterface(This,riid,ppvObject)	\
    ( (This)->lpVtbl -> QueryInterface(This,riid,ppvObject) ) 

#define IDebugBinder_AddRef(This)	\
    ( (This)->lpVtbl -> AddRef(This) ) 

#define IDebugBinder_Release(This)	\
    ( (This)->lpVtbl -> Release(This) ) 


#define IDebugBinder_Bind(This,pContainer,pField,ppObject)	\
    ( (This)->lpVtbl -> Bind(This,pContainer,pField,ppObject) ) 

#define IDebugBinder_ResolveDynamicType(This,pDynamic,ppResolved)	\
    ( (This)->lpVtbl -> ResolveDynamicType(This,pDynamic,ppResolved) ) 

#define IDebugBinder_ResolveRuntimeType(This,pObject,ppResolved)	\
    ( (This)->lpVtbl -> ResolveRuntimeType(This,pObject,ppResolved) ) 

#define IDebugBinder_GetMemoryContext(This,pField,dwConstant,ppMemCxt)	\
    ( (This)->lpVtbl -> GetMemoryContext(This,pField,dwConstant,ppMemCxt) ) 

#define IDebugBinder_GetFunctionObject(This,ppFunction)	\
    ( (This)->lpVtbl -> GetFunctionObject(This,ppFunction) ) 

#endif /* COBJMACROS */


#endif 	/* C style interface */




#endif 	/* __IDebugBinder_INTERFACE_DEFINED__ */


#ifndef __IDebugBinderDirect_INTERFACE_DEFINED__
#define __IDebugBinderDirect_INTERFACE_DEFINED__

/* interface IDebugBinderDirect */
/* [unique][uuid][object] */ 


EXTERN_C const IID IID_IDebugBinderDirect;

#if defined(__cplusplus) && !defined(CINTERFACE)
    
    MIDL_INTERFACE("9DB3E3B8-84F5-488e-93EB-B3CE3E33EDAB")
    IDebugBinderDirect : public IUnknown
    {
    public:
        virtual HRESULT STDMETHODCALLTYPE GetCORDBFrame( 
            /* [out] */ __RPC__deref_out_opt IUnknown **ppFrame) = 0;
        
        virtual HRESULT STDMETHODCALLTYPE GetCORDBModule( 
            /* [in] */ GUID guid,
            /* [in] */ ULONG32 appDomainID,
            /* [out] */ __RPC__deref_out_opt IUnknown **ppModule) = 0;
        
        virtual HRESULT STDMETHODCALLTYPE GetDebugProperty( 
            /* [out] */ __RPC__deref_out_opt IDebugProperty2 **ppProperty) = 0;
        
        virtual HRESULT STDMETHODCALLTYPE GetMemoryContext( 
            /* [in] */ __RPC__in DEBUG_ADDRESS *pda,
            /* [in] */ UINT64 dwConstant,
            /* [out] */ __RPC__deref_out_opt IDebugMemoryContext2 **ppMemCxt) = 0;
        
        virtual HRESULT STDMETHODCALLTYPE GetAlias( 
            /* [in] */ __RPC__in_opt IUnknown *pCorValue,
            /* [out] */ __RPC__deref_out_opt IDebugAlias **ppAlias) = 0;
        
        virtual HRESULT STDMETHODCALLTYPE IsUserData( 
            /* [in] */ __RPC__in DEBUG_ADDRESS *pda,
            /* [out] */ __RPC__out BOOL *pfUser) = 0;
        
        virtual HRESULT STDMETHODCALLTYPE CanDoFuncEval( 
            /* [in] */ __RPC__in DEBUG_ADDRESS *pda) = 0;
        
        virtual HRESULT STDMETHODCALLTYPE ContinueForFuncEval( 
            /* [in] */ __RPC__in_opt IUnknown *pCorEval,
            /* [in] */ DWORD dwEvalFlags,
            /* [in] */ DWORD dwTimeout) = 0;
        
        virtual HRESULT STDMETHODCALLTYPE CreateIDebugObject( 
            /* [in] */ __RPC__in_opt IUnknown *pCorDebugValue,
            /* [out] */ __RPC__deref_out_opt IDebugObject **ppObject) = 0;
        
        virtual HRESULT STDMETHODCALLTYPE IsInSQLCLRMode( 
            /* [out] */ __RPC__out BOOL *pfSQLCLRMode) = 0;
        
        virtual HRESULT STDMETHODCALLTYPE IsInEmbeddedClrMode( 
            /* [out] */ __RPC__out BOOL *pfEmbeddedClrMode) = 0;
        
    };
    
#else 	/* C style interface */

    typedef struct IDebugBinderDirectVtbl
    {
        BEGIN_INTERFACE
        
        HRESULT ( STDMETHODCALLTYPE *QueryInterface )( 
            IDebugBinderDirect * This,
            /* [in] */ __RPC__in REFIID riid,
            /* [iid_is][out] */ 
            __RPC__deref_out  void **ppvObject);
        
        ULONG ( STDMETHODCALLTYPE *AddRef )( 
            IDebugBinderDirect * This);
        
        ULONG ( STDMETHODCALLTYPE *Release )( 
            IDebugBinderDirect * This);
        
        HRESULT ( STDMETHODCALLTYPE *GetCORDBFrame )( 
            IDebugBinderDirect * This,
            /* [out] */ __RPC__deref_out_opt IUnknown **ppFrame);
        
        HRESULT ( STDMETHODCALLTYPE *GetCORDBModule )( 
            IDebugBinderDirect * This,
            /* [in] */ GUID guid,
            /* [in] */ ULONG32 appDomainID,
            /* [out] */ __RPC__deref_out_opt IUnknown **ppModule);
        
        HRESULT ( STDMETHODCALLTYPE *GetDebugProperty )( 
            IDebugBinderDirect * This,
            /* [out] */ __RPC__deref_out_opt IDebugProperty2 **ppProperty);
        
        HRESULT ( STDMETHODCALLTYPE *GetMemoryContext )( 
            IDebugBinderDirect * This,
            /* [in] */ __RPC__in DEBUG_ADDRESS *pda,
            /* [in] */ UINT64 dwConstant,
            /* [out] */ __RPC__deref_out_opt IDebugMemoryContext2 **ppMemCxt);
        
        HRESULT ( STDMETHODCALLTYPE *GetAlias )( 
            IDebugBinderDirect * This,
            /* [in] */ __RPC__in_opt IUnknown *pCorValue,
            /* [out] */ __RPC__deref_out_opt IDebugAlias **ppAlias);
        
        HRESULT ( STDMETHODCALLTYPE *IsUserData )( 
            IDebugBinderDirect * This,
            /* [in] */ __RPC__in DEBUG_ADDRESS *pda,
            /* [out] */ __RPC__out BOOL *pfUser);
        
        HRESULT ( STDMETHODCALLTYPE *CanDoFuncEval )( 
            IDebugBinderDirect * This,
            /* [in] */ __RPC__in DEBUG_ADDRESS *pda);
        
        HRESULT ( STDMETHODCALLTYPE *ContinueForFuncEval )( 
            IDebugBinderDirect * This,
            /* [in] */ __RPC__in_opt IUnknown *pCorEval,
            /* [in] */ DWORD dwEvalFlags,
            /* [in] */ DWORD dwTimeout);
        
        HRESULT ( STDMETHODCALLTYPE *CreateIDebugObject )( 
            IDebugBinderDirect * This,
            /* [in] */ __RPC__in_opt IUnknown *pCorDebugValue,
            /* [out] */ __RPC__deref_out_opt IDebugObject **ppObject);
        
        HRESULT ( STDMETHODCALLTYPE *IsInSQLCLRMode )( 
            IDebugBinderDirect * This,
            /* [out] */ __RPC__out BOOL *pfSQLCLRMode);
        
        HRESULT ( STDMETHODCALLTYPE *IsInEmbeddedClrMode )( 
            IDebugBinderDirect * This,
            /* [out] */ __RPC__out BOOL *pfEmbeddedClrMode);
        
        END_INTERFACE
    } IDebugBinderDirectVtbl;

    interface IDebugBinderDirect
    {
        CONST_VTBL struct IDebugBinderDirectVtbl *lpVtbl;
    };

    

#ifdef COBJMACROS


#define IDebugBinderDirect_QueryInterface(This,riid,ppvObject)	\
    ( (This)->lpVtbl -> QueryInterface(This,riid,ppvObject) ) 

#define IDebugBinderDirect_AddRef(This)	\
    ( (This)->lpVtbl -> AddRef(This) ) 

#define IDebugBinderDirect_Release(This)	\
    ( (This)->lpVtbl -> Release(This) ) 


#define IDebugBinderDirect_GetCORDBFrame(This,ppFrame)	\
    ( (This)->lpVtbl -> GetCORDBFrame(This,ppFrame) ) 

#define IDebugBinderDirect_GetCORDBModule(This,guid,appDomainID,ppModule)	\
    ( (This)->lpVtbl -> GetCORDBModule(This,guid,appDomainID,ppModule) ) 

#define IDebugBinderDirect_GetDebugProperty(This,ppProperty)	\
    ( (This)->lpVtbl -> GetDebugProperty(This,ppProperty) ) 

#define IDebugBinderDirect_GetMemoryContext(This,pda,dwConstant,ppMemCxt)	\
    ( (This)->lpVtbl -> GetMemoryContext(This,pda,dwConstant,ppMemCxt) ) 

#define IDebugBinderDirect_GetAlias(This,pCorValue,ppAlias)	\
    ( (This)->lpVtbl -> GetAlias(This,pCorValue,ppAlias) ) 

#define IDebugBinderDirect_IsUserData(This,pda,pfUser)	\
    ( (This)->lpVtbl -> IsUserData(This,pda,pfUser) ) 

#define IDebugBinderDirect_CanDoFuncEval(This,pda)	\
    ( (This)->lpVtbl -> CanDoFuncEval(This,pda) ) 

#define IDebugBinderDirect_ContinueForFuncEval(This,pCorEval,dwEvalFlags,dwTimeout)	\
    ( (This)->lpVtbl -> ContinueForFuncEval(This,pCorEval,dwEvalFlags,dwTimeout) ) 

#define IDebugBinderDirect_CreateIDebugObject(This,pCorDebugValue,ppObject)	\
    ( (This)->lpVtbl -> CreateIDebugObject(This,pCorDebugValue,ppObject) ) 

#define IDebugBinderDirect_IsInSQLCLRMode(This,pfSQLCLRMode)	\
    ( (This)->lpVtbl -> IsInSQLCLRMode(This,pfSQLCLRMode) ) 

#define IDebugBinderDirect_IsInEmbeddedClrMode(This,pfEmbeddedClrMode)	\
    ( (This)->lpVtbl -> IsInEmbeddedClrMode(This,pfEmbeddedClrMode) ) 

#endif /* COBJMACROS */


#endif 	/* C style interface */




#endif 	/* __IDebugBinderDirect_INTERFACE_DEFINED__ */


#ifndef __IDebugBinder2_INTERFACE_DEFINED__
#define __IDebugBinder2_INTERFACE_DEFINED__

/* interface IDebugBinder2 */
/* [unique][uuid][object] */ 


EXTERN_C const IID IID_IDebugBinder2;

#if defined(__cplusplus) && !defined(CINTERFACE)
    
    MIDL_INTERFACE("DCF3C6EE-7C7D-4e1f-AEEB-646902AF0723")
    IDebugBinder2 : public IUnknown
    {
    public:
        virtual HRESULT STDMETHODCALLTYPE GetMemoryObject( 
            /* [in] */ __RPC__in_opt IDebugField *pField,
            /* [in] */ DWORD dwConstant,
            /* [out] */ __RPC__deref_out_opt IDebugObject **ppObject) = 0;
        
        virtual HRESULT STDMETHODCALLTYPE GetExceptionObjectAndType( 
            /* [out] */ __RPC__deref_out_opt IDebugObject **ppException,
            /* [out] */ __RPC__deref_out_opt IDebugField **ppField) = 0;
        
    };
    
#else 	/* C style interface */

    typedef struct IDebugBinder2Vtbl
    {
        BEGIN_INTERFACE
        
        HRESULT ( STDMETHODCALLTYPE *QueryInterface )( 
            IDebugBinder2 * This,
            /* [in] */ __RPC__in REFIID riid,
            /* [iid_is][out] */ 
            __RPC__deref_out  void **ppvObject);
        
        ULONG ( STDMETHODCALLTYPE *AddRef )( 
            IDebugBinder2 * This);
        
        ULONG ( STDMETHODCALLTYPE *Release )( 
            IDebugBinder2 * This);
        
        HRESULT ( STDMETHODCALLTYPE *GetMemoryObject )( 
            IDebugBinder2 * This,
            /* [in] */ __RPC__in_opt IDebugField *pField,
            /* [in] */ DWORD dwConstant,
            /* [out] */ __RPC__deref_out_opt IDebugObject **ppObject);
        
        HRESULT ( STDMETHODCALLTYPE *GetExceptionObjectAndType )( 
            IDebugBinder2 * This,
            /* [out] */ __RPC__deref_out_opt IDebugObject **ppException,
            /* [out] */ __RPC__deref_out_opt IDebugField **ppField);
        
        END_INTERFACE
    } IDebugBinder2Vtbl;

    interface IDebugBinder2
    {
        CONST_VTBL struct IDebugBinder2Vtbl *lpVtbl;
    };

    

#ifdef COBJMACROS


#define IDebugBinder2_QueryInterface(This,riid,ppvObject)	\
    ( (This)->lpVtbl -> QueryInterface(This,riid,ppvObject) ) 

#define IDebugBinder2_AddRef(This)	\
    ( (This)->lpVtbl -> AddRef(This) ) 

#define IDebugBinder2_Release(This)	\
    ( (This)->lpVtbl -> Release(This) ) 


#define IDebugBinder2_GetMemoryObject(This,pField,dwConstant,ppObject)	\
    ( (This)->lpVtbl -> GetMemoryObject(This,pField,dwConstant,ppObject) ) 

#define IDebugBinder2_GetExceptionObjectAndType(This,ppException,ppField)	\
    ( (This)->lpVtbl -> GetExceptionObjectAndType(This,ppException,ppField) ) 

#endif /* COBJMACROS */


#endif 	/* C style interface */




#endif 	/* __IDebugBinder2_INTERFACE_DEFINED__ */


#ifndef __IDebugBinder3_INTERFACE_DEFINED__
#define __IDebugBinder3_INTERFACE_DEFINED__

/* interface IDebugBinder3 */
/* [unique][uuid][object] */ 


EXTERN_C const IID IID_IDebugBinder3;

#if defined(__cplusplus) && !defined(CINTERFACE)
    
    MIDL_INTERFACE("BBCD7263-B415-40f6-942A-4A9A8599B708")
    IDebugBinder3 : public IDebugBinder
    {
    public:
        virtual HRESULT STDMETHODCALLTYPE GetMemoryObject( 
            /* [in] */ __RPC__in_opt IDebugField *pField,
            /* [in] */ UINT64 uConstant,
            /* [out] */ __RPC__deref_out_opt IDebugObject **ppObject) = 0;
        
        virtual HRESULT STDMETHODCALLTYPE GetExceptionObjectAndType( 
            /* [out] */ __RPC__deref_out_opt IDebugObject **ppException,
            /* [out] */ __RPC__deref_out_opt IDebugField **ppField) = 0;
        
        virtual HRESULT STDMETHODCALLTYPE FindAlias( 
            /* [in] */ __RPC__in LPCOLESTR pcstrName,
            /* [out] */ __RPC__deref_out_opt IDebugAlias **ppAlias) = 0;
        
        virtual HRESULT STDMETHODCALLTYPE GetAllAliases( 
            /* [in] */ UINT uRequest,
            /* [length_is][size_is][full][out][in] */ __RPC__inout_ecount_part_opt(uRequest, *puFetched) IDebugAlias **ppAliases,
            /* [out] */ __RPC__out UINT *puFetched) = 0;
        
        virtual HRESULT STDMETHODCALLTYPE GetTypeArgumentCount( 
            /* [out] */ __RPC__out UINT *uCount) = 0;
        
        virtual HRESULT STDMETHODCALLTYPE GetTypeArguments( 
            /* [in] */ UINT skip,
            /* [in] */ UINT count,
            /* [length_is][size_is][full][out][in] */ __RPC__inout_ecount_part_opt(count, *pFetched) IDebugField **ppFields,
            /* [out] */ __RPC__out UINT *pFetched) = 0;
        
        virtual HRESULT STDMETHODCALLTYPE GetEEService( 
            /* [in] */ GUID vendor,
            /* [in] */ GUID language,
            /* [in] */ GUID iid,
            /* [out] */ __RPC__deref_out_opt IUnknown **ppService) = 0;
        
        virtual HRESULT STDMETHODCALLTYPE GetMemoryContext64( 
            /* [in] */ __RPC__in_opt IDebugField *pField,
            /* [in] */ UINT64 uConstant,
            /* [out] */ __RPC__deref_out_opt IDebugMemoryContext2 **ppMemCxt) = 0;
        
    };
    
#else 	/* C style interface */

    typedef struct IDebugBinder3Vtbl
    {
        BEGIN_INTERFACE
        
        HRESULT ( STDMETHODCALLTYPE *QueryInterface )( 
            IDebugBinder3 * This,
            /* [in] */ __RPC__in REFIID riid,
            /* [iid_is][out] */ 
            __RPC__deref_out  void **ppvObject);
        
        ULONG ( STDMETHODCALLTYPE *AddRef )( 
            IDebugBinder3 * This);
        
        ULONG ( STDMETHODCALLTYPE *Release )( 
            IDebugBinder3 * This);
        
        HRESULT ( STDMETHODCALLTYPE *Bind )( 
            IDebugBinder3 * This,
            /* [in] */ __RPC__in_opt IDebugObject *pContainer,
            /* [in] */ __RPC__in_opt IDebugField *pField,
            /* [out] */ __RPC__deref_out_opt IDebugObject **ppObject);
        
        HRESULT ( STDMETHODCALLTYPE *ResolveDynamicType )( 
            IDebugBinder3 * This,
            /* [in] */ __RPC__in_opt IDebugDynamicField *pDynamic,
            /* [out] */ __RPC__deref_out_opt IDebugField **ppResolved);
        
        HRESULT ( STDMETHODCALLTYPE *ResolveRuntimeType )( 
            IDebugBinder3 * This,
            /* [in] */ __RPC__in_opt IDebugObject *pObject,
            /* [out] */ __RPC__deref_out_opt IDebugField **ppResolved);
        
        HRESULT ( STDMETHODCALLTYPE *GetMemoryContext )( 
            IDebugBinder3 * This,
            /* [in] */ __RPC__in_opt IDebugField *pField,
            /* [in] */ DWORD dwConstant,
            /* [out] */ __RPC__deref_out_opt IDebugMemoryContext2 **ppMemCxt);
        
        HRESULT ( STDMETHODCALLTYPE *GetFunctionObject )( 
            IDebugBinder3 * This,
            /* [out] */ __RPC__deref_out_opt IDebugFunctionObject **ppFunction);
        
        HRESULT ( STDMETHODCALLTYPE *GetMemoryObject )( 
            IDebugBinder3 * This,
            /* [in] */ __RPC__in_opt IDebugField *pField,
            /* [in] */ UINT64 uConstant,
            /* [out] */ __RPC__deref_out_opt IDebugObject **ppObject);
        
        HRESULT ( STDMETHODCALLTYPE *GetExceptionObjectAndType )( 
            IDebugBinder3 * This,
            /* [out] */ __RPC__deref_out_opt IDebugObject **ppException,
            /* [out] */ __RPC__deref_out_opt IDebugField **ppField);
        
        HRESULT ( STDMETHODCALLTYPE *FindAlias )( 
            IDebugBinder3 * This,
            /* [in] */ __RPC__in LPCOLESTR pcstrName,
            /* [out] */ __RPC__deref_out_opt IDebugAlias **ppAlias);
        
        HRESULT ( STDMETHODCALLTYPE *GetAllAliases )( 
            IDebugBinder3 * This,
            /* [in] */ UINT uRequest,
            /* [length_is][size_is][full][out][in] */ __RPC__inout_ecount_part_opt(uRequest, *puFetched) IDebugAlias **ppAliases,
            /* [out] */ __RPC__out UINT *puFetched);
        
        HRESULT ( STDMETHODCALLTYPE *GetTypeArgumentCount )( 
            IDebugBinder3 * This,
            /* [out] */ __RPC__out UINT *uCount);
        
        HRESULT ( STDMETHODCALLTYPE *GetTypeArguments )( 
            IDebugBinder3 * This,
            /* [in] */ UINT skip,
            /* [in] */ UINT count,
            /* [length_is][size_is][full][out][in] */ __RPC__inout_ecount_part_opt(count, *pFetched) IDebugField **ppFields,
            /* [out] */ __RPC__out UINT *pFetched);
        
        HRESULT ( STDMETHODCALLTYPE *GetEEService )( 
            IDebugBinder3 * This,
            /* [in] */ GUID vendor,
            /* [in] */ GUID language,
            /* [in] */ GUID iid,
            /* [out] */ __RPC__deref_out_opt IUnknown **ppService);
        
        HRESULT ( STDMETHODCALLTYPE *GetMemoryContext64 )( 
            IDebugBinder3 * This,
            /* [in] */ __RPC__in_opt IDebugField *pField,
            /* [in] */ UINT64 uConstant,
            /* [out] */ __RPC__deref_out_opt IDebugMemoryContext2 **ppMemCxt);
        
        END_INTERFACE
    } IDebugBinder3Vtbl;

    interface IDebugBinder3
    {
        CONST_VTBL struct IDebugBinder3Vtbl *lpVtbl;
    };

    

#ifdef COBJMACROS


#define IDebugBinder3_QueryInterface(This,riid,ppvObject)	\
    ( (This)->lpVtbl -> QueryInterface(This,riid,ppvObject) ) 

#define IDebugBinder3_AddRef(This)	\
    ( (This)->lpVtbl -> AddRef(This) ) 

#define IDebugBinder3_Release(This)	\
    ( (This)->lpVtbl -> Release(This) ) 


#define IDebugBinder3_Bind(This,pContainer,pField,ppObject)	\
    ( (This)->lpVtbl -> Bind(This,pContainer,pField,ppObject) ) 

#define IDebugBinder3_ResolveDynamicType(This,pDynamic,ppResolved)	\
    ( (This)->lpVtbl -> ResolveDynamicType(This,pDynamic,ppResolved) ) 

#define IDebugBinder3_ResolveRuntimeType(This,pObject,ppResolved)	\
    ( (This)->lpVtbl -> ResolveRuntimeType(This,pObject,ppResolved) ) 

#define IDebugBinder3_GetMemoryContext(This,pField,dwConstant,ppMemCxt)	\
    ( (This)->lpVtbl -> GetMemoryContext(This,pField,dwConstant,ppMemCxt) ) 

#define IDebugBinder3_GetFunctionObject(This,ppFunction)	\
    ( (This)->lpVtbl -> GetFunctionObject(This,ppFunction) ) 


#define IDebugBinder3_GetMemoryObject(This,pField,uConstant,ppObject)	\
    ( (This)->lpVtbl -> GetMemoryObject(This,pField,uConstant,ppObject) ) 

#define IDebugBinder3_GetExceptionObjectAndType(This,ppException,ppField)	\
    ( (This)->lpVtbl -> GetExceptionObjectAndType(This,ppException,ppField) ) 

#define IDebugBinder3_FindAlias(This,pcstrName,ppAlias)	\
    ( (This)->lpVtbl -> FindAlias(This,pcstrName,ppAlias) ) 

#define IDebugBinder3_GetAllAliases(This,uRequest,ppAliases,puFetched)	\
    ( (This)->lpVtbl -> GetAllAliases(This,uRequest,ppAliases,puFetched) ) 

#define IDebugBinder3_GetTypeArgumentCount(This,uCount)	\
    ( (This)->lpVtbl -> GetTypeArgumentCount(This,uCount) ) 

#define IDebugBinder3_GetTypeArguments(This,skip,count,ppFields,pFetched)	\
    ( (This)->lpVtbl -> GetTypeArguments(This,skip,count,ppFields,pFetched) ) 

#define IDebugBinder3_GetEEService(This,vendor,language,iid,ppService)	\
    ( (This)->lpVtbl -> GetEEService(This,vendor,language,iid,ppService) ) 

#define IDebugBinder3_GetMemoryContext64(This,pField,uConstant,ppMemCxt)	\
    ( (This)->lpVtbl -> GetMemoryContext64(This,pField,uConstant,ppMemCxt) ) 

#endif /* COBJMACROS */


#endif 	/* C style interface */




#endif 	/* __IDebugBinder3_INTERFACE_DEFINED__ */


#ifndef __IEEVisualizerDataProvider_INTERFACE_DEFINED__
#define __IEEVisualizerDataProvider_INTERFACE_DEFINED__

/* interface IEEVisualizerDataProvider */
/* [unique][uuid][object] */ 


EXTERN_C const IID IID_IEEVisualizerDataProvider;

#if defined(__cplusplus) && !defined(CINTERFACE)
    
    MIDL_INTERFACE("27ED701A-FA26-406e-AE71-00011B5AE396")
    IEEVisualizerDataProvider : public IUnknown
    {
    public:
        virtual HRESULT STDMETHODCALLTYPE GetNewObjectForVisualizer( 
            /* [out] */ __RPC__deref_out_opt IDebugObject **ppObject) = 0;
        
        virtual HRESULT STDMETHODCALLTYPE GetObjectForVisualizer( 
            /* [out] */ __RPC__deref_out_opt IDebugObject **ppObject) = 0;
        
        virtual HRESULT STDMETHODCALLTYPE CanSetObjectForVisualizer( 
            /* [out] */ __RPC__out BOOL *b) = 0;
        
        virtual HRESULT STDMETHODCALLTYPE SetObjectForVisualizer( 
            /* [in] */ __RPC__in_opt IDebugObject *pNewObject,
            /* [out] */ __RPC__deref_out_opt BSTR *error,
            /* [out] */ __RPC__deref_out_opt IDebugObject **pException) = 0;
        
    };
    
#else 	/* C style interface */

    typedef struct IEEVisualizerDataProviderVtbl
    {
        BEGIN_INTERFACE
        
        HRESULT ( STDMETHODCALLTYPE *QueryInterface )( 
            IEEVisualizerDataProvider * This,
            /* [in] */ __RPC__in REFIID riid,
            /* [iid_is][out] */ 
            __RPC__deref_out  void **ppvObject);
        
        ULONG ( STDMETHODCALLTYPE *AddRef )( 
            IEEVisualizerDataProvider * This);
        
        ULONG ( STDMETHODCALLTYPE *Release )( 
            IEEVisualizerDataProvider * This);
        
        HRESULT ( STDMETHODCALLTYPE *GetNewObjectForVisualizer )( 
            IEEVisualizerDataProvider * This,
            /* [out] */ __RPC__deref_out_opt IDebugObject **ppObject);
        
        HRESULT ( STDMETHODCALLTYPE *GetObjectForVisualizer )( 
            IEEVisualizerDataProvider * This,
            /* [out] */ __RPC__deref_out_opt IDebugObject **ppObject);
        
        HRESULT ( STDMETHODCALLTYPE *CanSetObjectForVisualizer )( 
            IEEVisualizerDataProvider * This,
            /* [out] */ __RPC__out BOOL *b);
        
        HRESULT ( STDMETHODCALLTYPE *SetObjectForVisualizer )( 
            IEEVisualizerDataProvider * This,
            /* [in] */ __RPC__in_opt IDebugObject *pNewObject,
            /* [out] */ __RPC__deref_out_opt BSTR *error,
            /* [out] */ __RPC__deref_out_opt IDebugObject **pException);
        
        END_INTERFACE
    } IEEVisualizerDataProviderVtbl;

    interface IEEVisualizerDataProvider
    {
        CONST_VTBL struct IEEVisualizerDataProviderVtbl *lpVtbl;
    };

    

#ifdef COBJMACROS


#define IEEVisualizerDataProvider_QueryInterface(This,riid,ppvObject)	\
    ( (This)->lpVtbl -> QueryInterface(This,riid,ppvObject) ) 

#define IEEVisualizerDataProvider_AddRef(This)	\
    ( (This)->lpVtbl -> AddRef(This) ) 

#define IEEVisualizerDataProvider_Release(This)	\
    ( (This)->lpVtbl -> Release(This) ) 


#define IEEVisualizerDataProvider_GetNewObjectForVisualizer(This,ppObject)	\
    ( (This)->lpVtbl -> GetNewObjectForVisualizer(This,ppObject) ) 

#define IEEVisualizerDataProvider_GetObjectForVisualizer(This,ppObject)	\
    ( (This)->lpVtbl -> GetObjectForVisualizer(This,ppObject) ) 

#define IEEVisualizerDataProvider_CanSetObjectForVisualizer(This,b)	\
    ( (This)->lpVtbl -> CanSetObjectForVisualizer(This,b) ) 

#define IEEVisualizerDataProvider_SetObjectForVisualizer(This,pNewObject,error,pException)	\
    ( (This)->lpVtbl -> SetObjectForVisualizer(This,pNewObject,error,pException) ) 

#endif /* COBJMACROS */


#endif 	/* C style interface */




#endif 	/* __IEEVisualizerDataProvider_INTERFACE_DEFINED__ */


#ifndef __IEEVisualizerService_INTERFACE_DEFINED__
#define __IEEVisualizerService_INTERFACE_DEFINED__

/* interface IEEVisualizerService */
/* [unique][uuid][object] */ 


enum enum_DisplayKind
    {	DisplayKind_Value	= 0x1,
	DisplayKind_Name	= 0x2,
	DisplayKind_Type	= 0x3
    } ;
typedef DWORD DisplayKind;


enum enum_BrowsableKind
    {	BrowsableKind_None	= 0x1,
	BrowsableKind_Collapsed	= 0x2,
	BrowsableKind_RootHidden	= 0x3,
	BrowsableKind_Never	= 0x4
    } ;
typedef DWORD BrowsableKind;


EXTERN_C const IID IID_IEEVisualizerService;

#if defined(__cplusplus) && !defined(CINTERFACE)
    
    MIDL_INTERFACE("82545B58-F203-4835-ACD6-6D0997AA6F25")
    IEEVisualizerService : public IUnknown
    {
    public:
        virtual HRESULT STDMETHODCALLTYPE GetCustomViewerCount( 
            /* [out] */ __RPC__out ULONG *pcelt) = 0;
        
        virtual HRESULT STDMETHODCALLTYPE GetCustomViewerList( 
            /* [in] */ ULONG celtSkip,
            /* [in] */ ULONG celtRequested,
            /* [length_is][size_is][out] */ __RPC__out_ecount_part(celtRequested, *pceltFetched) DEBUG_CUSTOM_VIEWER *rgViewers,
            /* [out] */ __RPC__out ULONG *pceltFetched) = 0;
        
        virtual HRESULT STDMETHODCALLTYPE GetPropertyProxy( 
            /* [in] */ DWORD dwID,
            /* [out] */ __RPC__deref_out_opt IPropertyProxyEESide **proxy) = 0;
        
        virtual HRESULT STDMETHODCALLTYPE GetValueDisplayStringCount( 
            /* [in] */ DWORD displayKind,
            /* [in] */ __RPC__in_opt IDebugField *propertyOrField,
            /* [out] */ __RPC__out ULONG *pcelt) = 0;
        
        virtual HRESULT STDMETHODCALLTYPE GetValueDisplayStrings( 
            /* [in] */ DisplayKind displayKind,
            /* [in] */ __RPC__in_opt IDebugField *propertyOrField,
            /* [in] */ ULONG celtSkip,
            /* [in] */ ULONG celtRequested,
            /* [length_is][size_is][out] */ __RPC__out_ecount_part(celtRequested, *pceltFetched) BSTR *rgStrings,
            /* [length_is][size_is][out] */ __RPC__out_ecount_part(celtRequested, *pceltFetched) BOOL *rgIsExpression,
            /* [out] */ __RPC__out ULONG *pceltFetched) = 0;
        
        virtual HRESULT STDMETHODCALLTYPE GetBrowsableState( 
            /* [in] */ __RPC__in_opt IDebugField *propertyOrField,
            /* [out] */ __RPC__out BrowsableKind *browsableKind) = 0;
        
        virtual HRESULT STDMETHODCALLTYPE PossiblyHasInlineProxy( 
            /* [out] */ __RPC__out BOOL *mayHaveProxy) = 0;
        
        virtual HRESULT STDMETHODCALLTYPE CreateInlineProxy( 
            /* [out] */ __RPC__deref_out_opt IDebugObject **proxy,
            /* [out] */ __RPC__out BOOL *IsExceptionNotProxy,
            /* [out] */ __RPC__deref_out_opt BSTR *errorString) = 0;
        
    };
    
#else 	/* C style interface */

    typedef struct IEEVisualizerServiceVtbl
    {
        BEGIN_INTERFACE
        
        HRESULT ( STDMETHODCALLTYPE *QueryInterface )( 
            IEEVisualizerService * This,
            /* [in] */ __RPC__in REFIID riid,
            /* [iid_is][out] */ 
            __RPC__deref_out  void **ppvObject);
        
        ULONG ( STDMETHODCALLTYPE *AddRef )( 
            IEEVisualizerService * This);
        
        ULONG ( STDMETHODCALLTYPE *Release )( 
            IEEVisualizerService * This);
        
        HRESULT ( STDMETHODCALLTYPE *GetCustomViewerCount )( 
            IEEVisualizerService * This,
            /* [out] */ __RPC__out ULONG *pcelt);
        
        HRESULT ( STDMETHODCALLTYPE *GetCustomViewerList )( 
            IEEVisualizerService * This,
            /* [in] */ ULONG celtSkip,
            /* [in] */ ULONG celtRequested,
            /* [length_is][size_is][out] */ __RPC__out_ecount_part(celtRequested, *pceltFetched) DEBUG_CUSTOM_VIEWER *rgViewers,
            /* [out] */ __RPC__out ULONG *pceltFetched);
        
        HRESULT ( STDMETHODCALLTYPE *GetPropertyProxy )( 
            IEEVisualizerService * This,
            /* [in] */ DWORD dwID,
            /* [out] */ __RPC__deref_out_opt IPropertyProxyEESide **proxy);
        
        HRESULT ( STDMETHODCALLTYPE *GetValueDisplayStringCount )( 
            IEEVisualizerService * This,
            /* [in] */ DWORD displayKind,
            /* [in] */ __RPC__in_opt IDebugField *propertyOrField,
            /* [out] */ __RPC__out ULONG *pcelt);
        
        HRESULT ( STDMETHODCALLTYPE *GetValueDisplayStrings )( 
            IEEVisualizerService * This,
            /* [in] */ DisplayKind displayKind,
            /* [in] */ __RPC__in_opt IDebugField *propertyOrField,
            /* [in] */ ULONG celtSkip,
            /* [in] */ ULONG celtRequested,
            /* [length_is][size_is][out] */ __RPC__out_ecount_part(celtRequested, *pceltFetched) BSTR *rgStrings,
            /* [length_is][size_is][out] */ __RPC__out_ecount_part(celtRequested, *pceltFetched) BOOL *rgIsExpression,
            /* [out] */ __RPC__out ULONG *pceltFetched);
        
        HRESULT ( STDMETHODCALLTYPE *GetBrowsableState )( 
            IEEVisualizerService * This,
            /* [in] */ __RPC__in_opt IDebugField *propertyOrField,
            /* [out] */ __RPC__out BrowsableKind *browsableKind);
        
        HRESULT ( STDMETHODCALLTYPE *PossiblyHasInlineProxy )( 
            IEEVisualizerService * This,
            /* [out] */ __RPC__out BOOL *mayHaveProxy);
        
        HRESULT ( STDMETHODCALLTYPE *CreateInlineProxy )( 
            IEEVisualizerService * This,
            /* [out] */ __RPC__deref_out_opt IDebugObject **proxy,
            /* [out] */ __RPC__out BOOL *IsExceptionNotProxy,
            /* [out] */ __RPC__deref_out_opt BSTR *errorString);
        
        END_INTERFACE
    } IEEVisualizerServiceVtbl;

    interface IEEVisualizerService
    {
        CONST_VTBL struct IEEVisualizerServiceVtbl *lpVtbl;
    };

    

#ifdef COBJMACROS


#define IEEVisualizerService_QueryInterface(This,riid,ppvObject)	\
    ( (This)->lpVtbl -> QueryInterface(This,riid,ppvObject) ) 

#define IEEVisualizerService_AddRef(This)	\
    ( (This)->lpVtbl -> AddRef(This) ) 

#define IEEVisualizerService_Release(This)	\
    ( (This)->lpVtbl -> Release(This) ) 


#define IEEVisualizerService_GetCustomViewerCount(This,pcelt)	\
    ( (This)->lpVtbl -> GetCustomViewerCount(This,pcelt) ) 

#define IEEVisualizerService_GetCustomViewerList(This,celtSkip,celtRequested,rgViewers,pceltFetched)	\
    ( (This)->lpVtbl -> GetCustomViewerList(This,celtSkip,celtRequested,rgViewers,pceltFetched) ) 

#define IEEVisualizerService_GetPropertyProxy(This,dwID,proxy)	\
    ( (This)->lpVtbl -> GetPropertyProxy(This,dwID,proxy) ) 

#define IEEVisualizerService_GetValueDisplayStringCount(This,displayKind,propertyOrField,pcelt)	\
    ( (This)->lpVtbl -> GetValueDisplayStringCount(This,displayKind,propertyOrField,pcelt) ) 

#define IEEVisualizerService_GetValueDisplayStrings(This,displayKind,propertyOrField,celtSkip,celtRequested,rgStrings,rgIsExpression,pceltFetched)	\
    ( (This)->lpVtbl -> GetValueDisplayStrings(This,displayKind,propertyOrField,celtSkip,celtRequested,rgStrings,rgIsExpression,pceltFetched) ) 

#define IEEVisualizerService_GetBrowsableState(This,propertyOrField,browsableKind)	\
    ( (This)->lpVtbl -> GetBrowsableState(This,propertyOrField,browsableKind) ) 

#define IEEVisualizerService_PossiblyHasInlineProxy(This,mayHaveProxy)	\
    ( (This)->lpVtbl -> PossiblyHasInlineProxy(This,mayHaveProxy) ) 

#define IEEVisualizerService_CreateInlineProxy(This,proxy,IsExceptionNotProxy,errorString)	\
    ( (This)->lpVtbl -> CreateInlineProxy(This,proxy,IsExceptionNotProxy,errorString) ) 

#endif /* COBJMACROS */


#endif 	/* C style interface */




#endif 	/* __IEEVisualizerService_INTERFACE_DEFINED__ */


#ifndef __IEEVisualizerServiceProvider_INTERFACE_DEFINED__
#define __IEEVisualizerServiceProvider_INTERFACE_DEFINED__

/* interface IEEVisualizerServiceProvider */
/* [unique][uuid][object] */ 


EXTERN_C const IID IID_IEEVisualizerServiceProvider;

#if defined(__cplusplus) && !defined(CINTERFACE)
    
    MIDL_INTERFACE("A2F2C782-F929-4ffa-8699-88D4C4C07B17")
    IEEVisualizerServiceProvider : public IUnknown
    {
    public:
        virtual HRESULT STDMETHODCALLTYPE CreateVisualizerService( 
            /* [in] */ __RPC__in_opt IDebugBinder *binder,
            /* [in] */ __RPC__in_opt IDebugSymbolProvider *pSymProv,
            /* [in] */ __RPC__in_opt IDebugAddress *pAddress,
            /* [in] */ __RPC__in_opt IEEVisualizerDataProvider *dataProvider,
            /* [out] */ __RPC__deref_out_opt IEEVisualizerService **ppService) = 0;
        
    };
    
#else 	/* C style interface */

    typedef struct IEEVisualizerServiceProviderVtbl
    {
        BEGIN_INTERFACE
        
        HRESULT ( STDMETHODCALLTYPE *QueryInterface )( 
            IEEVisualizerServiceProvider * This,
            /* [in] */ __RPC__in REFIID riid,
            /* [iid_is][out] */ 
            __RPC__deref_out  void **ppvObject);
        
        ULONG ( STDMETHODCALLTYPE *AddRef )( 
            IEEVisualizerServiceProvider * This);
        
        ULONG ( STDMETHODCALLTYPE *Release )( 
            IEEVisualizerServiceProvider * This);
        
        HRESULT ( STDMETHODCALLTYPE *CreateVisualizerService )( 
            IEEVisualizerServiceProvider * This,
            /* [in] */ __RPC__in_opt IDebugBinder *binder,
            /* [in] */ __RPC__in_opt IDebugSymbolProvider *pSymProv,
            /* [in] */ __RPC__in_opt IDebugAddress *pAddress,
            /* [in] */ __RPC__in_opt IEEVisualizerDataProvider *dataProvider,
            /* [out] */ __RPC__deref_out_opt IEEVisualizerService **ppService);
        
        END_INTERFACE
    } IEEVisualizerServiceProviderVtbl;

    interface IEEVisualizerServiceProvider
    {
        CONST_VTBL struct IEEVisualizerServiceProviderVtbl *lpVtbl;
    };

    

#ifdef COBJMACROS


#define IEEVisualizerServiceProvider_QueryInterface(This,riid,ppvObject)	\
    ( (This)->lpVtbl -> QueryInterface(This,riid,ppvObject) ) 

#define IEEVisualizerServiceProvider_AddRef(This)	\
    ( (This)->lpVtbl -> AddRef(This) ) 

#define IEEVisualizerServiceProvider_Release(This)	\
    ( (This)->lpVtbl -> Release(This) ) 


#define IEEVisualizerServiceProvider_CreateVisualizerService(This,binder,pSymProv,pAddress,dataProvider,ppService)	\
    ( (This)->lpVtbl -> CreateVisualizerService(This,binder,pSymProv,pAddress,dataProvider,ppService) ) 

#endif /* COBJMACROS */


#endif 	/* C style interface */




#endif 	/* __IEEVisualizerServiceProvider_INTERFACE_DEFINED__ */


#ifndef __IDebugPointerObject_INTERFACE_DEFINED__
#define __IDebugPointerObject_INTERFACE_DEFINED__

/* interface IDebugPointerObject */
/* [unique][uuid][object] */ 


EXTERN_C const IID IID_IDebugPointerObject;

#if defined(__cplusplus) && !defined(CINTERFACE)
    
    MIDL_INTERFACE("112756A1-3F04-4ccd-BFD6-ACB4BCA614C9")
    IDebugPointerObject : public IDebugObject
    {
    public:
        virtual HRESULT STDMETHODCALLTYPE Dereference( 
            /* [in] */ DWORD dwIndex,
            /* [out] */ __RPC__deref_out_opt IDebugObject **ppObject) = 0;
        
        virtual HRESULT STDMETHODCALLTYPE GetBytes( 
            /* [in] */ DWORD dwStart,
            /* [in] */ DWORD dwCount,
            /* [length_is][size_is][out] */ __RPC__out_ecount_part(dwCount, dwCount) BYTE *pBytes,
            /* [out] */ __RPC__out DWORD *pdwBytes) = 0;
        
        virtual HRESULT STDMETHODCALLTYPE SetBytes( 
            /* [in] */ DWORD dwStart,
            /* [in] */ DWORD dwCount,
            /* [length_is][size_is][in] */ __RPC__in_ecount_part(dwCount, dwCount) BYTE *pBytes,
            /* [out] */ __RPC__out DWORD *pdwBytes) = 0;
        
    };
    
#else 	/* C style interface */

    typedef struct IDebugPointerObjectVtbl
    {
        BEGIN_INTERFACE
        
        HRESULT ( STDMETHODCALLTYPE *QueryInterface )( 
            IDebugPointerObject * This,
            /* [in] */ __RPC__in REFIID riid,
            /* [iid_is][out] */ 
            __RPC__deref_out  void **ppvObject);
        
        ULONG ( STDMETHODCALLTYPE *AddRef )( 
            IDebugPointerObject * This);
        
        ULONG ( STDMETHODCALLTYPE *Release )( 
            IDebugPointerObject * This);
        
        HRESULT ( STDMETHODCALLTYPE *GetSize )( 
            IDebugPointerObject * This,
            /* [out] */ __RPC__out UINT *pnSize);
        
        HRESULT ( STDMETHODCALLTYPE *GetValue )( 
            IDebugPointerObject * This,
            /* [length_is][size_is][out] */ __RPC__out_ecount_part(nSize, nSize) BYTE *pValue,
            /* [in] */ UINT nSize);
        
        HRESULT ( STDMETHODCALLTYPE *SetValue )( 
            IDebugPointerObject * This,
            /* [size_is][in] */ __RPC__in_ecount_full(nSize) BYTE *pValue,
            /* [in] */ UINT nSize);
        
        HRESULT ( STDMETHODCALLTYPE *SetReferenceValue )( 
            IDebugPointerObject * This,
            /* [in] */ __RPC__in_opt IDebugObject *pObject);
        
        HRESULT ( STDMETHODCALLTYPE *GetMemoryContext )( 
            IDebugPointerObject * This,
            __RPC__deref_in_opt IDebugMemoryContext2 **pContext);
        
        HRESULT ( STDMETHODCALLTYPE *GetManagedDebugObject )( 
            IDebugPointerObject * This,
            /* [out] */ __RPC__deref_out_opt IDebugManagedObject **ppObject);
        
        HRESULT ( STDMETHODCALLTYPE *IsNullReference )( 
            IDebugPointerObject * This,
            /* [out] */ __RPC__out BOOL *pfIsNull);
        
        HRESULT ( STDMETHODCALLTYPE *IsEqual )( 
            IDebugPointerObject * This,
            /* [in] */ __RPC__in_opt IDebugObject *pObject,
            /* [out] */ __RPC__out BOOL *pfIsEqual);
        
        HRESULT ( STDMETHODCALLTYPE *IsReadOnly )( 
            IDebugPointerObject * This,
            /* [out] */ __RPC__out BOOL *pfIsReadOnly);
        
        HRESULT ( STDMETHODCALLTYPE *IsProxy )( 
            IDebugPointerObject * This,
            /* [out] */ __RPC__out BOOL *pfIsProxy);
        
        HRESULT ( STDMETHODCALLTYPE *Dereference )( 
            IDebugPointerObject * This,
            /* [in] */ DWORD dwIndex,
            /* [out] */ __RPC__deref_out_opt IDebugObject **ppObject);
        
        HRESULT ( STDMETHODCALLTYPE *GetBytes )( 
            IDebugPointerObject * This,
            /* [in] */ DWORD dwStart,
            /* [in] */ DWORD dwCount,
            /* [length_is][size_is][out] */ __RPC__out_ecount_part(dwCount, dwCount) BYTE *pBytes,
            /* [out] */ __RPC__out DWORD *pdwBytes);
        
        HRESULT ( STDMETHODCALLTYPE *SetBytes )( 
            IDebugPointerObject * This,
            /* [in] */ DWORD dwStart,
            /* [in] */ DWORD dwCount,
            /* [length_is][size_is][in] */ __RPC__in_ecount_part(dwCount, dwCount) BYTE *pBytes,
            /* [out] */ __RPC__out DWORD *pdwBytes);
        
        END_INTERFACE
    } IDebugPointerObjectVtbl;

    interface IDebugPointerObject
    {
        CONST_VTBL struct IDebugPointerObjectVtbl *lpVtbl;
    };

    

#ifdef COBJMACROS


#define IDebugPointerObject_QueryInterface(This,riid,ppvObject)	\
    ( (This)->lpVtbl -> QueryInterface(This,riid,ppvObject) ) 

#define IDebugPointerObject_AddRef(This)	\
    ( (This)->lpVtbl -> AddRef(This) ) 

#define IDebugPointerObject_Release(This)	\
    ( (This)->lpVtbl -> Release(This) ) 


#define IDebugPointerObject_GetSize(This,pnSize)	\
    ( (This)->lpVtbl -> GetSize(This,pnSize) ) 

#define IDebugPointerObject_GetValue(This,pValue,nSize)	\
    ( (This)->lpVtbl -> GetValue(This,pValue,nSize) ) 

#define IDebugPointerObject_SetValue(This,pValue,nSize)	\
    ( (This)->lpVtbl -> SetValue(This,pValue,nSize) ) 

#define IDebugPointerObject_SetReferenceValue(This,pObject)	\
    ( (This)->lpVtbl -> SetReferenceValue(This,pObject) ) 

#define IDebugPointerObject_GetMemoryContext(This,pContext)	\
    ( (This)->lpVtbl -> GetMemoryContext(This,pContext) ) 

#define IDebugPointerObject_GetManagedDebugObject(This,ppObject)	\
    ( (This)->lpVtbl -> GetManagedDebugObject(This,ppObject) ) 

#define IDebugPointerObject_IsNullReference(This,pfIsNull)	\
    ( (This)->lpVtbl -> IsNullReference(This,pfIsNull) ) 

#define IDebugPointerObject_IsEqual(This,pObject,pfIsEqual)	\
    ( (This)->lpVtbl -> IsEqual(This,pObject,pfIsEqual) ) 

#define IDebugPointerObject_IsReadOnly(This,pfIsReadOnly)	\
    ( (This)->lpVtbl -> IsReadOnly(This,pfIsReadOnly) ) 

#define IDebugPointerObject_IsProxy(This,pfIsProxy)	\
    ( (This)->lpVtbl -> IsProxy(This,pfIsProxy) ) 


#define IDebugPointerObject_Dereference(This,dwIndex,ppObject)	\
    ( (This)->lpVtbl -> Dereference(This,dwIndex,ppObject) ) 

#define IDebugPointerObject_GetBytes(This,dwStart,dwCount,pBytes,pdwBytes)	\
    ( (This)->lpVtbl -> GetBytes(This,dwStart,dwCount,pBytes,pdwBytes) ) 

#define IDebugPointerObject_SetBytes(This,dwStart,dwCount,pBytes,pdwBytes)	\
    ( (This)->lpVtbl -> SetBytes(This,dwStart,dwCount,pBytes,pdwBytes) ) 

#endif /* COBJMACROS */


#endif 	/* C style interface */




#endif 	/* __IDebugPointerObject_INTERFACE_DEFINED__ */


#ifndef __IDebugPointerObject2_INTERFACE_DEFINED__
#define __IDebugPointerObject2_INTERFACE_DEFINED__

/* interface IDebugPointerObject2 */
/* [unique][uuid][object] */ 


EXTERN_C const IID IID_IDebugPointerObject2;

#if defined(__cplusplus) && !defined(CINTERFACE)
    
    MIDL_INTERFACE("BF032216-2C7F-4682-84C1-76EF432D840B")
    IDebugPointerObject2 : public IDebugObject
    {
    public:
        virtual HRESULT STDMETHODCALLTYPE ComputePointerAddress( 
            /* [out] */ __RPC__out DWORD *pdwAddress) = 0;
        
    };
    
#else 	/* C style interface */

    typedef struct IDebugPointerObject2Vtbl
    {
        BEGIN_INTERFACE
        
        HRESULT ( STDMETHODCALLTYPE *QueryInterface )( 
            IDebugPointerObject2 * This,
            /* [in] */ __RPC__in REFIID riid,
            /* [iid_is][out] */ 
            __RPC__deref_out  void **ppvObject);
        
        ULONG ( STDMETHODCALLTYPE *AddRef )( 
            IDebugPointerObject2 * This);
        
        ULONG ( STDMETHODCALLTYPE *Release )( 
            IDebugPointerObject2 * This);
        
        HRESULT ( STDMETHODCALLTYPE *GetSize )( 
            IDebugPointerObject2 * This,
            /* [out] */ __RPC__out UINT *pnSize);
        
        HRESULT ( STDMETHODCALLTYPE *GetValue )( 
            IDebugPointerObject2 * This,
            /* [length_is][size_is][out] */ __RPC__out_ecount_part(nSize, nSize) BYTE *pValue,
            /* [in] */ UINT nSize);
        
        HRESULT ( STDMETHODCALLTYPE *SetValue )( 
            IDebugPointerObject2 * This,
            /* [size_is][in] */ __RPC__in_ecount_full(nSize) BYTE *pValue,
            /* [in] */ UINT nSize);
        
        HRESULT ( STDMETHODCALLTYPE *SetReferenceValue )( 
            IDebugPointerObject2 * This,
            /* [in] */ __RPC__in_opt IDebugObject *pObject);
        
        HRESULT ( STDMETHODCALLTYPE *GetMemoryContext )( 
            IDebugPointerObject2 * This,
            __RPC__deref_in_opt IDebugMemoryContext2 **pContext);
        
        HRESULT ( STDMETHODCALLTYPE *GetManagedDebugObject )( 
            IDebugPointerObject2 * This,
            /* [out] */ __RPC__deref_out_opt IDebugManagedObject **ppObject);
        
        HRESULT ( STDMETHODCALLTYPE *IsNullReference )( 
            IDebugPointerObject2 * This,
            /* [out] */ __RPC__out BOOL *pfIsNull);
        
        HRESULT ( STDMETHODCALLTYPE *IsEqual )( 
            IDebugPointerObject2 * This,
            /* [in] */ __RPC__in_opt IDebugObject *pObject,
            /* [out] */ __RPC__out BOOL *pfIsEqual);
        
        HRESULT ( STDMETHODCALLTYPE *IsReadOnly )( 
            IDebugPointerObject2 * This,
            /* [out] */ __RPC__out BOOL *pfIsReadOnly);
        
        HRESULT ( STDMETHODCALLTYPE *IsProxy )( 
            IDebugPointerObject2 * This,
            /* [out] */ __RPC__out BOOL *pfIsProxy);
        
        HRESULT ( STDMETHODCALLTYPE *ComputePointerAddress )( 
            IDebugPointerObject2 * This,
            /* [out] */ __RPC__out DWORD *pdwAddress);
        
        END_INTERFACE
    } IDebugPointerObject2Vtbl;

    interface IDebugPointerObject2
    {
        CONST_VTBL struct IDebugPointerObject2Vtbl *lpVtbl;
    };

    

#ifdef COBJMACROS


#define IDebugPointerObject2_QueryInterface(This,riid,ppvObject)	\
    ( (This)->lpVtbl -> QueryInterface(This,riid,ppvObject) ) 

#define IDebugPointerObject2_AddRef(This)	\
    ( (This)->lpVtbl -> AddRef(This) ) 

#define IDebugPointerObject2_Release(This)	\
    ( (This)->lpVtbl -> Release(This) ) 


#define IDebugPointerObject2_GetSize(This,pnSize)	\
    ( (This)->lpVtbl -> GetSize(This,pnSize) ) 

#define IDebugPointerObject2_GetValue(This,pValue,nSize)	\
    ( (This)->lpVtbl -> GetValue(This,pValue,nSize) ) 

#define IDebugPointerObject2_SetValue(This,pValue,nSize)	\
    ( (This)->lpVtbl -> SetValue(This,pValue,nSize) ) 

#define IDebugPointerObject2_SetReferenceValue(This,pObject)	\
    ( (This)->lpVtbl -> SetReferenceValue(This,pObject) ) 

#define IDebugPointerObject2_GetMemoryContext(This,pContext)	\
    ( (This)->lpVtbl -> GetMemoryContext(This,pContext) ) 

#define IDebugPointerObject2_GetManagedDebugObject(This,ppObject)	\
    ( (This)->lpVtbl -> GetManagedDebugObject(This,ppObject) ) 

#define IDebugPointerObject2_IsNullReference(This,pfIsNull)	\
    ( (This)->lpVtbl -> IsNullReference(This,pfIsNull) ) 

#define IDebugPointerObject2_IsEqual(This,pObject,pfIsEqual)	\
    ( (This)->lpVtbl -> IsEqual(This,pObject,pfIsEqual) ) 

#define IDebugPointerObject2_IsReadOnly(This,pfIsReadOnly)	\
    ( (This)->lpVtbl -> IsReadOnly(This,pfIsReadOnly) ) 

#define IDebugPointerObject2_IsProxy(This,pfIsProxy)	\
    ( (This)->lpVtbl -> IsProxy(This,pfIsProxy) ) 


#define IDebugPointerObject2_ComputePointerAddress(This,pdwAddress)	\
    ( (This)->lpVtbl -> ComputePointerAddress(This,pdwAddress) ) 

#endif /* COBJMACROS */


#endif 	/* C style interface */




#endif 	/* __IDebugPointerObject2_INTERFACE_DEFINED__ */


#ifndef __IDebugPointerObject3_INTERFACE_DEFINED__
#define __IDebugPointerObject3_INTERFACE_DEFINED__

/* interface IDebugPointerObject3 */
/* [unique][uuid][object] */ 


EXTERN_C const IID IID_IDebugPointerObject3;

#if defined(__cplusplus) && !defined(CINTERFACE)
    
    MIDL_INTERFACE("B69D88F9-BC5A-4eb3-A43C-9AF3155F0632")
    IDebugPointerObject3 : public IDebugPointerObject
    {
    public:
        virtual HRESULT STDMETHODCALLTYPE GetPointerAddress( 
            /* [out] */ __RPC__out UINT64 *puAddress) = 0;
        
    };
    
#else 	/* C style interface */

    typedef struct IDebugPointerObject3Vtbl
    {
        BEGIN_INTERFACE
        
        HRESULT ( STDMETHODCALLTYPE *QueryInterface )( 
            IDebugPointerObject3 * This,
            /* [in] */ __RPC__in REFIID riid,
            /* [iid_is][out] */ 
            __RPC__deref_out  void **ppvObject);
        
        ULONG ( STDMETHODCALLTYPE *AddRef )( 
            IDebugPointerObject3 * This);
        
        ULONG ( STDMETHODCALLTYPE *Release )( 
            IDebugPointerObject3 * This);
        
        HRESULT ( STDMETHODCALLTYPE *GetSize )( 
            IDebugPointerObject3 * This,
            /* [out] */ __RPC__out UINT *pnSize);
        
        HRESULT ( STDMETHODCALLTYPE *GetValue )( 
            IDebugPointerObject3 * This,
            /* [length_is][size_is][out] */ __RPC__out_ecount_part(nSize, nSize) BYTE *pValue,
            /* [in] */ UINT nSize);
        
        HRESULT ( STDMETHODCALLTYPE *SetValue )( 
            IDebugPointerObject3 * This,
            /* [size_is][in] */ __RPC__in_ecount_full(nSize) BYTE *pValue,
            /* [in] */ UINT nSize);
        
        HRESULT ( STDMETHODCALLTYPE *SetReferenceValue )( 
            IDebugPointerObject3 * This,
            /* [in] */ __RPC__in_opt IDebugObject *pObject);
        
        HRESULT ( STDMETHODCALLTYPE *GetMemoryContext )( 
            IDebugPointerObject3 * This,
            __RPC__deref_in_opt IDebugMemoryContext2 **pContext);
        
        HRESULT ( STDMETHODCALLTYPE *GetManagedDebugObject )( 
            IDebugPointerObject3 * This,
            /* [out] */ __RPC__deref_out_opt IDebugManagedObject **ppObject);
        
        HRESULT ( STDMETHODCALLTYPE *IsNullReference )( 
            IDebugPointerObject3 * This,
            /* [out] */ __RPC__out BOOL *pfIsNull);
        
        HRESULT ( STDMETHODCALLTYPE *IsEqual )( 
            IDebugPointerObject3 * This,
            /* [in] */ __RPC__in_opt IDebugObject *pObject,
            /* [out] */ __RPC__out BOOL *pfIsEqual);
        
        HRESULT ( STDMETHODCALLTYPE *IsReadOnly )( 
            IDebugPointerObject3 * This,
            /* [out] */ __RPC__out BOOL *pfIsReadOnly);
        
        HRESULT ( STDMETHODCALLTYPE *IsProxy )( 
            IDebugPointerObject3 * This,
            /* [out] */ __RPC__out BOOL *pfIsProxy);
        
        HRESULT ( STDMETHODCALLTYPE *Dereference )( 
            IDebugPointerObject3 * This,
            /* [in] */ DWORD dwIndex,
            /* [out] */ __RPC__deref_out_opt IDebugObject **ppObject);
        
        HRESULT ( STDMETHODCALLTYPE *GetBytes )( 
            IDebugPointerObject3 * This,
            /* [in] */ DWORD dwStart,
            /* [in] */ DWORD dwCount,
            /* [length_is][size_is][out] */ __RPC__out_ecount_part(dwCount, dwCount) BYTE *pBytes,
            /* [out] */ __RPC__out DWORD *pdwBytes);
        
        HRESULT ( STDMETHODCALLTYPE *SetBytes )( 
            IDebugPointerObject3 * This,
            /* [in] */ DWORD dwStart,
            /* [in] */ DWORD dwCount,
            /* [length_is][size_is][in] */ __RPC__in_ecount_part(dwCount, dwCount) BYTE *pBytes,
            /* [out] */ __RPC__out DWORD *pdwBytes);
        
        HRESULT ( STDMETHODCALLTYPE *GetPointerAddress )( 
            IDebugPointerObject3 * This,
            /* [out] */ __RPC__out UINT64 *puAddress);
        
        END_INTERFACE
    } IDebugPointerObject3Vtbl;

    interface IDebugPointerObject3
    {
        CONST_VTBL struct IDebugPointerObject3Vtbl *lpVtbl;
    };

    

#ifdef COBJMACROS


#define IDebugPointerObject3_QueryInterface(This,riid,ppvObject)	\
    ( (This)->lpVtbl -> QueryInterface(This,riid,ppvObject) ) 

#define IDebugPointerObject3_AddRef(This)	\
    ( (This)->lpVtbl -> AddRef(This) ) 

#define IDebugPointerObject3_Release(This)	\
    ( (This)->lpVtbl -> Release(This) ) 


#define IDebugPointerObject3_GetSize(This,pnSize)	\
    ( (This)->lpVtbl -> GetSize(This,pnSize) ) 

#define IDebugPointerObject3_GetValue(This,pValue,nSize)	\
    ( (This)->lpVtbl -> GetValue(This,pValue,nSize) ) 

#define IDebugPointerObject3_SetValue(This,pValue,nSize)	\
    ( (This)->lpVtbl -> SetValue(This,pValue,nSize) ) 

#define IDebugPointerObject3_SetReferenceValue(This,pObject)	\
    ( (This)->lpVtbl -> SetReferenceValue(This,pObject) ) 

#define IDebugPointerObject3_GetMemoryContext(This,pContext)	\
    ( (This)->lpVtbl -> GetMemoryContext(This,pContext) ) 

#define IDebugPointerObject3_GetManagedDebugObject(This,ppObject)	\
    ( (This)->lpVtbl -> GetManagedDebugObject(This,ppObject) ) 

#define IDebugPointerObject3_IsNullReference(This,pfIsNull)	\
    ( (This)->lpVtbl -> IsNullReference(This,pfIsNull) ) 

#define IDebugPointerObject3_IsEqual(This,pObject,pfIsEqual)	\
    ( (This)->lpVtbl -> IsEqual(This,pObject,pfIsEqual) ) 

#define IDebugPointerObject3_IsReadOnly(This,pfIsReadOnly)	\
    ( (This)->lpVtbl -> IsReadOnly(This,pfIsReadOnly) ) 

#define IDebugPointerObject3_IsProxy(This,pfIsProxy)	\
    ( (This)->lpVtbl -> IsProxy(This,pfIsProxy) ) 


#define IDebugPointerObject3_Dereference(This,dwIndex,ppObject)	\
    ( (This)->lpVtbl -> Dereference(This,dwIndex,ppObject) ) 

#define IDebugPointerObject3_GetBytes(This,dwStart,dwCount,pBytes,pdwBytes)	\
    ( (This)->lpVtbl -> GetBytes(This,dwStart,dwCount,pBytes,pdwBytes) ) 

#define IDebugPointerObject3_SetBytes(This,dwStart,dwCount,pBytes,pdwBytes)	\
    ( (This)->lpVtbl -> SetBytes(This,dwStart,dwCount,pBytes,pdwBytes) ) 


#define IDebugPointerObject3_GetPointerAddress(This,puAddress)	\
    ( (This)->lpVtbl -> GetPointerAddress(This,puAddress) ) 

#endif /* COBJMACROS */


#endif 	/* C style interface */




#endif 	/* __IDebugPointerObject3_INTERFACE_DEFINED__ */


#ifndef __IDebugParsedExpression_INTERFACE_DEFINED__
#define __IDebugParsedExpression_INTERFACE_DEFINED__

/* interface IDebugParsedExpression */
/* [unique][uuid][object] */ 


EXTERN_C const IID IID_IDebugParsedExpression;

#if defined(__cplusplus) && !defined(CINTERFACE)
    
    MIDL_INTERFACE("7895C94C-5A3F-11d2-B742-0000F87572EF")
    IDebugParsedExpression : public IUnknown
    {
    public:
        virtual HRESULT STDMETHODCALLTYPE EvaluateSync( 
            /* [in] */ DWORD dwEvalFlags,
            /* [in] */ DWORD dwTimeout,
            /* [in] */ __RPC__in_opt IDebugSymbolProvider *pSymbolProvider,
            /* [in] */ __RPC__in_opt IDebugAddress *pAddress,
            /* [in] */ __RPC__in_opt IDebugBinder *pBinder,
            /* [in] */ __RPC__in BSTR bstrResultType,
            /* [out] */ __RPC__deref_out_opt IDebugProperty2 **ppResult) = 0;
        
    };
    
#else 	/* C style interface */

    typedef struct IDebugParsedExpressionVtbl
    {
        BEGIN_INTERFACE
        
        HRESULT ( STDMETHODCALLTYPE *QueryInterface )( 
            IDebugParsedExpression * This,
            /* [in] */ __RPC__in REFIID riid,
            /* [iid_is][out] */ 
            __RPC__deref_out  void **ppvObject);
        
        ULONG ( STDMETHODCALLTYPE *AddRef )( 
            IDebugParsedExpression * This);
        
        ULONG ( STDMETHODCALLTYPE *Release )( 
            IDebugParsedExpression * This);
        
        HRESULT ( STDMETHODCALLTYPE *EvaluateSync )( 
            IDebugParsedExpression * This,
            /* [in] */ DWORD dwEvalFlags,
            /* [in] */ DWORD dwTimeout,
            /* [in] */ __RPC__in_opt IDebugSymbolProvider *pSymbolProvider,
            /* [in] */ __RPC__in_opt IDebugAddress *pAddress,
            /* [in] */ __RPC__in_opt IDebugBinder *pBinder,
            /* [in] */ __RPC__in BSTR bstrResultType,
            /* [out] */ __RPC__deref_out_opt IDebugProperty2 **ppResult);
        
        END_INTERFACE
    } IDebugParsedExpressionVtbl;

    interface IDebugParsedExpression
    {
        CONST_VTBL struct IDebugParsedExpressionVtbl *lpVtbl;
    };

    

#ifdef COBJMACROS


#define IDebugParsedExpression_QueryInterface(This,riid,ppvObject)	\
    ( (This)->lpVtbl -> QueryInterface(This,riid,ppvObject) ) 

#define IDebugParsedExpression_AddRef(This)	\
    ( (This)->lpVtbl -> AddRef(This) ) 

#define IDebugParsedExpression_Release(This)	\
    ( (This)->lpVtbl -> Release(This) ) 


#define IDebugParsedExpression_EvaluateSync(This,dwEvalFlags,dwTimeout,pSymbolProvider,pAddress,pBinder,bstrResultType,ppResult)	\
    ( (This)->lpVtbl -> EvaluateSync(This,dwEvalFlags,dwTimeout,pSymbolProvider,pAddress,pBinder,bstrResultType,ppResult) ) 

#endif /* COBJMACROS */


#endif 	/* C style interface */




#endif 	/* __IDebugParsedExpression_INTERFACE_DEFINED__ */


#ifndef __IEnumDebugObjects_INTERFACE_DEFINED__
#define __IEnumDebugObjects_INTERFACE_DEFINED__

/* interface IEnumDebugObjects */
/* [unique][uuid][object] */ 


EXTERN_C const IID IID_IEnumDebugObjects;

#if defined(__cplusplus) && !defined(CINTERFACE)
    
    MIDL_INTERFACE("0881751C-99F4-11d2-B767-0000F87572EF")
    IEnumDebugObjects : public IUnknown
    {
    public:
        virtual HRESULT STDMETHODCALLTYPE Next( 
            /* [in] */ ULONG celt,
            /* [length_is][size_is][out] */ __RPC__out_ecount_part(celt, *pceltFetched) IDebugObject **rgelt,
            /* [out][in] */ __RPC__inout ULONG *pceltFetched) = 0;
        
        virtual HRESULT STDMETHODCALLTYPE Skip( 
            /* [in] */ ULONG celt) = 0;
        
        virtual HRESULT STDMETHODCALLTYPE Reset( void) = 0;
        
        virtual HRESULT STDMETHODCALLTYPE Clone( 
            /* [out] */ __RPC__deref_out_opt IEnumDebugObjects **ppEnum) = 0;
        
        virtual HRESULT STDMETHODCALLTYPE GetCount( 
            /* [out] */ __RPC__out ULONG *pcelt) = 0;
        
    };
    
#else 	/* C style interface */

    typedef struct IEnumDebugObjectsVtbl
    {
        BEGIN_INTERFACE
        
        HRESULT ( STDMETHODCALLTYPE *QueryInterface )( 
            IEnumDebugObjects * This,
            /* [in] */ __RPC__in REFIID riid,
            /* [iid_is][out] */ 
            __RPC__deref_out  void **ppvObject);
        
        ULONG ( STDMETHODCALLTYPE *AddRef )( 
            IEnumDebugObjects * This);
        
        ULONG ( STDMETHODCALLTYPE *Release )( 
            IEnumDebugObjects * This);
        
        HRESULT ( STDMETHODCALLTYPE *Next )( 
            IEnumDebugObjects * This,
            /* [in] */ ULONG celt,
            /* [length_is][size_is][out] */ __RPC__out_ecount_part(celt, *pceltFetched) IDebugObject **rgelt,
            /* [out][in] */ __RPC__inout ULONG *pceltFetched);
        
        HRESULT ( STDMETHODCALLTYPE *Skip )( 
            IEnumDebugObjects * This,
            /* [in] */ ULONG celt);
        
        HRESULT ( STDMETHODCALLTYPE *Reset )( 
            IEnumDebugObjects * This);
        
        HRESULT ( STDMETHODCALLTYPE *Clone )( 
            IEnumDebugObjects * This,
            /* [out] */ __RPC__deref_out_opt IEnumDebugObjects **ppEnum);
        
        HRESULT ( STDMETHODCALLTYPE *GetCount )( 
            IEnumDebugObjects * This,
            /* [out] */ __RPC__out ULONG *pcelt);
        
        END_INTERFACE
    } IEnumDebugObjectsVtbl;

    interface IEnumDebugObjects
    {
        CONST_VTBL struct IEnumDebugObjectsVtbl *lpVtbl;
    };

    

#ifdef COBJMACROS


#define IEnumDebugObjects_QueryInterface(This,riid,ppvObject)	\
    ( (This)->lpVtbl -> QueryInterface(This,riid,ppvObject) ) 

#define IEnumDebugObjects_AddRef(This)	\
    ( (This)->lpVtbl -> AddRef(This) ) 

#define IEnumDebugObjects_Release(This)	\
    ( (This)->lpVtbl -> Release(This) ) 


#define IEnumDebugObjects_Next(This,celt,rgelt,pceltFetched)	\
    ( (This)->lpVtbl -> Next(This,celt,rgelt,pceltFetched) ) 

#define IEnumDebugObjects_Skip(This,celt)	\
    ( (This)->lpVtbl -> Skip(This,celt) ) 

#define IEnumDebugObjects_Reset(This)	\
    ( (This)->lpVtbl -> Reset(This) ) 

#define IEnumDebugObjects_Clone(This,ppEnum)	\
    ( (This)->lpVtbl -> Clone(This,ppEnum) ) 

#define IEnumDebugObjects_GetCount(This,pcelt)	\
    ( (This)->lpVtbl -> GetCount(This,pcelt) ) 

#endif /* COBJMACROS */


#endif 	/* C style interface */




#endif 	/* __IEnumDebugObjects_INTERFACE_DEFINED__ */


#ifndef __IDebugAlias_INTERFACE_DEFINED__
#define __IDebugAlias_INTERFACE_DEFINED__

/* interface IDebugAlias */
/* [unique][uuid][object] */ 


EXTERN_C const IID IID_IDebugAlias;

#if defined(__cplusplus) && !defined(CINTERFACE)
    
    MIDL_INTERFACE("DE7CCB92-94AC-4841-B354-5827B68217E7")
    IDebugAlias : public IUnknown
    {
    public:
        virtual HRESULT STDMETHODCALLTYPE GetObject( 
            /* [out] */ __RPC__deref_out_opt IDebugObject2 **ppObject) = 0;
        
        virtual HRESULT STDMETHODCALLTYPE GetName( 
            /* [out] */ __RPC__deref_out_opt BSTR *pbstrName) = 0;
        
        virtual HRESULT STDMETHODCALLTYPE GetICorDebugValue( 
            /* [out] */ __RPC__deref_out_opt IUnknown **ppUnk) = 0;
        
        virtual HRESULT STDMETHODCALLTYPE Dispose( void) = 0;
        
    };
    
#else 	/* C style interface */

    typedef struct IDebugAliasVtbl
    {
        BEGIN_INTERFACE
        
        HRESULT ( STDMETHODCALLTYPE *QueryInterface )( 
            IDebugAlias * This,
            /* [in] */ __RPC__in REFIID riid,
            /* [iid_is][out] */ 
            __RPC__deref_out  void **ppvObject);
        
        ULONG ( STDMETHODCALLTYPE *AddRef )( 
            IDebugAlias * This);
        
        ULONG ( STDMETHODCALLTYPE *Release )( 
            IDebugAlias * This);
        
        HRESULT ( STDMETHODCALLTYPE *GetObject )( 
            IDebugAlias * This,
            /* [out] */ __RPC__deref_out_opt IDebugObject2 **ppObject);
        
        HRESULT ( STDMETHODCALLTYPE *GetName )( 
            IDebugAlias * This,
            /* [out] */ __RPC__deref_out_opt BSTR *pbstrName);
        
        HRESULT ( STDMETHODCALLTYPE *GetICorDebugValue )( 
            IDebugAlias * This,
            /* [out] */ __RPC__deref_out_opt IUnknown **ppUnk);
        
        HRESULT ( STDMETHODCALLTYPE *Dispose )( 
            IDebugAlias * This);
        
        END_INTERFACE
    } IDebugAliasVtbl;

    interface IDebugAlias
    {
        CONST_VTBL struct IDebugAliasVtbl *lpVtbl;
    };

    

#ifdef COBJMACROS


#define IDebugAlias_QueryInterface(This,riid,ppvObject)	\
    ( (This)->lpVtbl -> QueryInterface(This,riid,ppvObject) ) 

#define IDebugAlias_AddRef(This)	\
    ( (This)->lpVtbl -> AddRef(This) ) 

#define IDebugAlias_Release(This)	\
    ( (This)->lpVtbl -> Release(This) ) 


#define IDebugAlias_GetObject(This,ppObject)	\
    ( (This)->lpVtbl -> GetObject(This,ppObject) ) 

#define IDebugAlias_GetName(This,pbstrName)	\
    ( (This)->lpVtbl -> GetName(This,pbstrName) ) 

#define IDebugAlias_GetICorDebugValue(This,ppUnk)	\
    ( (This)->lpVtbl -> GetICorDebugValue(This,ppUnk) ) 

#define IDebugAlias_Dispose(This)	\
    ( (This)->lpVtbl -> Dispose(This) ) 

#endif /* COBJMACROS */


#endif 	/* C style interface */




#endif 	/* __IDebugAlias_INTERFACE_DEFINED__ */


/* interface __MIDL_itf_ee_0000_0025 */
/* [local] */ 

#define VB_EE_DLL L"vbdebug.dll"
#define MC_EE_DLL L"mcee.dll"
#define CS_EE_DLL L"cscompee.dll"
#define VJS_EE_DLL L"vjscompee.dll"


extern RPC_IF_HANDLE __MIDL_itf_ee_0000_0025_v0_0_c_ifspec;
extern RPC_IF_HANDLE __MIDL_itf_ee_0000_0025_v0_0_s_ifspec;


#ifndef __MicrosoftEELib_LIBRARY_DEFINED__
#define __MicrosoftEELib_LIBRARY_DEFINED__

/* library MicrosoftEELib */
/* [uuid] */ 


EXTERN_C const IID LIBID_MicrosoftEELib;

EXTERN_C const CLSID CLSID_ManagedCppExpressionEvaluator;

#ifdef __cplusplus

class DECLSPEC_UUID("FDDC0D64-0720-11D3-BDA3-00C04FA302E2")
ManagedCppExpressionEvaluator;
#endif

EXTERN_C const CLSID CLSID_CSharpExpressionEvaluator;

#ifdef __cplusplus

class DECLSPEC_UUID("60F5556F-7EBC-4992-8E83-E9B49187FDE3")
CSharpExpressionEvaluator;
#endif

EXTERN_C const CLSID CLSID_JSharpExpressionEvaluator;

#ifdef __cplusplus

class DECLSPEC_UUID("AFFFF3AF-61DC-4859-9799-1E404EF507D4")
JSharpExpressionEvaluator;
#endif
#endif /* __MicrosoftEELib_LIBRARY_DEFINED__ */

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


