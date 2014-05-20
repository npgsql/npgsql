

/* this ALWAYS GENERATED file contains the definitions for the interfaces */


 /* File created by MIDL compiler version 8.00.0603 */
/* @@MIDL_FILE_HEADING(  ) */

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

#ifndef __msdbg110_h__
#define __msdbg110_h__

#if defined(_MSC_VER) && (_MSC_VER >= 1020)
#pragma once
#endif

/* Forward Declarations */ 

#ifndef __IQueryComputeThreadInfo110_FWD_DEFINED__
#define __IQueryComputeThreadInfo110_FWD_DEFINED__
typedef interface IQueryComputeThreadInfo110 IQueryComputeThreadInfo110;

#endif 	/* __IQueryComputeThreadInfo110_FWD_DEFINED__ */


#ifndef __IEnumComputeThreadInfo110_FWD_DEFINED__
#define __IEnumComputeThreadInfo110_FWD_DEFINED__
typedef interface IEnumComputeThreadInfo110 IEnumComputeThreadInfo110;

#endif 	/* __IEnumComputeThreadInfo110_FWD_DEFINED__ */


#ifndef __IEnumMatchedStackFrameInfo110_FWD_DEFINED__
#define __IEnumMatchedStackFrameInfo110_FWD_DEFINED__
typedef interface IEnumMatchedStackFrameInfo110 IEnumMatchedStackFrameInfo110;

#endif 	/* __IEnumMatchedStackFrameInfo110_FWD_DEFINED__ */


#ifndef __IDebugComputeThread110_FWD_DEFINED__
#define __IDebugComputeThread110_FWD_DEFINED__
typedef interface IDebugComputeThread110 IDebugComputeThread110;

#endif 	/* __IDebugComputeThread110_FWD_DEFINED__ */


#ifndef __IDebugComputeKernel110_FWD_DEFINED__
#define __IDebugComputeKernel110_FWD_DEFINED__
typedef interface IDebugComputeKernel110 IDebugComputeKernel110;

#endif 	/* __IDebugComputeKernel110_FWD_DEFINED__ */


#ifndef __IDebugProgram110_FWD_DEFINED__
#define __IDebugProgram110_FWD_DEFINED__
typedef interface IDebugProgram110 IDebugProgram110;

#endif 	/* __IDebugProgram110_FWD_DEFINED__ */


#ifndef __IDebugPropertyBag110_FWD_DEFINED__
#define __IDebugPropertyBag110_FWD_DEFINED__
typedef interface IDebugPropertyBag110 IDebugPropertyBag110;

#endif 	/* __IDebugPropertyBag110_FWD_DEFINED__ */


#ifndef __IEnumDebugComputeKernel110_FWD_DEFINED__
#define __IEnumDebugComputeKernel110_FWD_DEFINED__
typedef interface IEnumDebugComputeKernel110 IEnumDebugComputeKernel110;

#endif 	/* __IEnumDebugComputeKernel110_FWD_DEFINED__ */


#ifndef __IDebugComputeThreadPropertyChangedEvent110_FWD_DEFINED__
#define __IDebugComputeThreadPropertyChangedEvent110_FWD_DEFINED__
typedef interface IDebugComputeThreadPropertyChangedEvent110 IDebugComputeThreadPropertyChangedEvent110;

#endif 	/* __IDebugComputeThreadPropertyChangedEvent110_FWD_DEFINED__ */


#ifndef __IDebugComputeKernelDispatchEvent110_FWD_DEFINED__
#define __IDebugComputeKernelDispatchEvent110_FWD_DEFINED__
typedef interface IDebugComputeKernelDispatchEvent110 IDebugComputeKernelDispatchEvent110;

#endif 	/* __IDebugComputeKernelDispatchEvent110_FWD_DEFINED__ */


#ifndef __IDebugStackFrame110_FWD_DEFINED__
#define __IDebugStackFrame110_FWD_DEFINED__
typedef interface IDebugStackFrame110 IDebugStackFrame110;

#endif 	/* __IDebugStackFrame110_FWD_DEFINED__ */


#ifndef __IDebugExpressionCompilationContext110_FWD_DEFINED__
#define __IDebugExpressionCompilationContext110_FWD_DEFINED__
typedef interface IDebugExpressionCompilationContext110 IDebugExpressionCompilationContext110;

#endif 	/* __IDebugExpressionCompilationContext110_FWD_DEFINED__ */


#ifndef __IDebugCompiledExpression110_FWD_DEFINED__
#define __IDebugCompiledExpression110_FWD_DEFINED__
typedef interface IDebugCompiledExpression110 IDebugCompiledExpression110;

#endif 	/* __IDebugCompiledExpression110_FWD_DEFINED__ */


#ifndef __IEnumGroupEvaluationResultContext110_FWD_DEFINED__
#define __IEnumGroupEvaluationResultContext110_FWD_DEFINED__
typedef interface IEnumGroupEvaluationResultContext110 IEnumGroupEvaluationResultContext110;

#endif 	/* __IEnumGroupEvaluationResultContext110_FWD_DEFINED__ */


#ifndef __IEnumGroupEvaluationResults110_FWD_DEFINED__
#define __IEnumGroupEvaluationResults110_FWD_DEFINED__
typedef interface IEnumGroupEvaluationResults110 IEnumGroupEvaluationResults110;

#endif 	/* __IEnumGroupEvaluationResults110_FWD_DEFINED__ */


#ifndef __IDebugGroupEvaluationCompleteEvent110_FWD_DEFINED__
#define __IDebugGroupEvaluationCompleteEvent110_FWD_DEFINED__
typedef interface IDebugGroupEvaluationCompleteEvent110 IDebugGroupEvaluationCompleteEvent110;

#endif 	/* __IDebugGroupEvaluationCompleteEvent110_FWD_DEFINED__ */


#ifndef __IDebugBreakpointRequest110_FWD_DEFINED__
#define __IDebugBreakpointRequest110_FWD_DEFINED__
typedef interface IDebugBreakpointRequest110 IDebugBreakpointRequest110;

#endif 	/* __IDebugBreakpointRequest110_FWD_DEFINED__ */


#ifndef __IDebugBoundBreakpoint110_FWD_DEFINED__
#define __IDebugBoundBreakpoint110_FWD_DEFINED__
typedef interface IDebugBoundBreakpoint110 IDebugBoundBreakpoint110;

#endif 	/* __IDebugBoundBreakpoint110_FWD_DEFINED__ */


#ifndef __IDebugDocumentPosition110_FWD_DEFINED__
#define __IDebugDocumentPosition110_FWD_DEFINED__
typedef interface IDebugDocumentPosition110 IDebugDocumentPosition110;

#endif 	/* __IDebugDocumentPosition110_FWD_DEFINED__ */


#ifndef __IDebugUserNotificationUI110_FWD_DEFINED__
#define __IDebugUserNotificationUI110_FWD_DEFINED__
typedef interface IDebugUserNotificationUI110 IDebugUserNotificationUI110;

#endif 	/* __IDebugUserNotificationUI110_FWD_DEFINED__ */


#ifndef __IDebugDefaultPort110_FWD_DEFINED__
#define __IDebugDefaultPort110_FWD_DEFINED__
typedef interface IDebugDefaultPort110 IDebugDefaultPort110;

#endif 	/* __IDebugDefaultPort110_FWD_DEFINED__ */


#ifndef __IDebugSettingsCallback110_FWD_DEFINED__
#define __IDebugSettingsCallback110_FWD_DEFINED__
typedef interface IDebugSettingsCallback110 IDebugSettingsCallback110;

#endif 	/* __IDebugSettingsCallback110_FWD_DEFINED__ */


#ifndef __IDebugEngine110_FWD_DEFINED__
#define __IDebugEngine110_FWD_DEFINED__
typedef interface IDebugEngine110 IDebugEngine110;

#endif 	/* __IDebugEngine110_FWD_DEFINED__ */


#ifndef __IVsCustomDebuggerEventHandler110_FWD_DEFINED__
#define __IVsCustomDebuggerEventHandler110_FWD_DEFINED__
typedef interface IVsCustomDebuggerEventHandler110 IVsCustomDebuggerEventHandler110;

#endif 	/* __IVsCustomDebuggerEventHandler110_FWD_DEFINED__ */


#ifndef __IVsCustomDebuggerStoppingEventHandler110_FWD_DEFINED__
#define __IVsCustomDebuggerStoppingEventHandler110_FWD_DEFINED__
typedef interface IVsCustomDebuggerStoppingEventHandler110 IVsCustomDebuggerStoppingEventHandler110;

#endif 	/* __IVsCustomDebuggerStoppingEventHandler110_FWD_DEFINED__ */


#ifndef __IDebugCustomEvent110_FWD_DEFINED__
#define __IDebugCustomEvent110_FWD_DEFINED__
typedef interface IDebugCustomEvent110 IDebugCustomEvent110;

#endif 	/* __IDebugCustomEvent110_FWD_DEFINED__ */


#ifndef __IVsDebugLaunchNotifyListenerFactory110_FWD_DEFINED__
#define __IVsDebugLaunchNotifyListenerFactory110_FWD_DEFINED__
typedef interface IVsDebugLaunchNotifyListenerFactory110 IVsDebugLaunchNotifyListenerFactory110;

#endif 	/* __IVsDebugLaunchNotifyListenerFactory110_FWD_DEFINED__ */


#ifndef __IVsDebugLaunchNotifyListener110_FWD_DEFINED__
#define __IVsDebugLaunchNotifyListener110_FWD_DEFINED__
typedef interface IVsDebugLaunchNotifyListener110 IVsDebugLaunchNotifyListener110;

#endif 	/* __IVsDebugLaunchNotifyListener110_FWD_DEFINED__ */


#ifndef __IDebugLaunchedProcessAttachRequestEvent110_FWD_DEFINED__
#define __IDebugLaunchedProcessAttachRequestEvent110_FWD_DEFINED__
typedef interface IDebugLaunchedProcessAttachRequestEvent110 IDebugLaunchedProcessAttachRequestEvent110;

#endif 	/* __IDebugLaunchedProcessAttachRequestEvent110_FWD_DEFINED__ */


#ifndef __IAppDomainInfo110_FWD_DEFINED__
#define __IAppDomainInfo110_FWD_DEFINED__
typedef interface IAppDomainInfo110 IAppDomainInfo110;

#endif 	/* __IAppDomainInfo110_FWD_DEFINED__ */


#ifndef __IVsCppDebugUIVisualizer_FWD_DEFINED__
#define __IVsCppDebugUIVisualizer_FWD_DEFINED__
typedef interface IVsCppDebugUIVisualizer IVsCppDebugUIVisualizer;

#endif 	/* __IVsCppDebugUIVisualizer_FWD_DEFINED__ */


#ifndef __DebugUserNotificationUI110_FWD_DEFINED__
#define __DebugUserNotificationUI110_FWD_DEFINED__

#ifdef __cplusplus
typedef class DebugUserNotificationUI110 DebugUserNotificationUI110;
#else
typedef struct DebugUserNotificationUI110 DebugUserNotificationUI110;
#endif /* __cplusplus */

#endif 	/* __DebugUserNotificationUI110_FWD_DEFINED__ */


#ifndef __IDebugStackFrame110_FWD_DEFINED__
#define __IDebugStackFrame110_FWD_DEFINED__
typedef interface IDebugStackFrame110 IDebugStackFrame110;

#endif 	/* __IDebugStackFrame110_FWD_DEFINED__ */


#ifndef __IDebugComputeThread110_FWD_DEFINED__
#define __IDebugComputeThread110_FWD_DEFINED__
typedef interface IDebugComputeThread110 IDebugComputeThread110;

#endif 	/* __IDebugComputeThread110_FWD_DEFINED__ */


#ifndef __IDebugComputeKernel110_FWD_DEFINED__
#define __IDebugComputeKernel110_FWD_DEFINED__
typedef interface IDebugComputeKernel110 IDebugComputeKernel110;

#endif 	/* __IDebugComputeKernel110_FWD_DEFINED__ */


#ifndef __IQueryComputeThreadInfo110_FWD_DEFINED__
#define __IQueryComputeThreadInfo110_FWD_DEFINED__
typedef interface IQueryComputeThreadInfo110 IQueryComputeThreadInfo110;

#endif 	/* __IQueryComputeThreadInfo110_FWD_DEFINED__ */


#ifndef __IEnumComputeThreadInfo110_FWD_DEFINED__
#define __IEnumComputeThreadInfo110_FWD_DEFINED__
typedef interface IEnumComputeThreadInfo110 IEnumComputeThreadInfo110;

#endif 	/* __IEnumComputeThreadInfo110_FWD_DEFINED__ */


#ifndef __IEnumMatchedStackFrameInfo110_FWD_DEFINED__
#define __IEnumMatchedStackFrameInfo110_FWD_DEFINED__
typedef interface IEnumMatchedStackFrameInfo110 IEnumMatchedStackFrameInfo110;

#endif 	/* __IEnumMatchedStackFrameInfo110_FWD_DEFINED__ */


#ifndef __IDebugProgram110_FWD_DEFINED__
#define __IDebugProgram110_FWD_DEFINED__
typedef interface IDebugProgram110 IDebugProgram110;

#endif 	/* __IDebugProgram110_FWD_DEFINED__ */


#ifndef __IDebugPropertyBag110_FWD_DEFINED__
#define __IDebugPropertyBag110_FWD_DEFINED__
typedef interface IDebugPropertyBag110 IDebugPropertyBag110;

#endif 	/* __IDebugPropertyBag110_FWD_DEFINED__ */


#ifndef __IEnumDebugComputeKernel110_FWD_DEFINED__
#define __IEnumDebugComputeKernel110_FWD_DEFINED__
typedef interface IEnumDebugComputeKernel110 IEnumDebugComputeKernel110;

#endif 	/* __IEnumDebugComputeKernel110_FWD_DEFINED__ */


#ifndef __IDebugComputeKernelDispatchEvent110_FWD_DEFINED__
#define __IDebugComputeKernelDispatchEvent110_FWD_DEFINED__
typedef interface IDebugComputeKernelDispatchEvent110 IDebugComputeKernelDispatchEvent110;

#endif 	/* __IDebugComputeKernelDispatchEvent110_FWD_DEFINED__ */


#ifndef __IDebugExpressionCompilationContext110_FWD_DEFINED__
#define __IDebugExpressionCompilationContext110_FWD_DEFINED__
typedef interface IDebugExpressionCompilationContext110 IDebugExpressionCompilationContext110;

#endif 	/* __IDebugExpressionCompilationContext110_FWD_DEFINED__ */


#ifndef __IDebugCompiledExpression110_FWD_DEFINED__
#define __IDebugCompiledExpression110_FWD_DEFINED__
typedef interface IDebugCompiledExpression110 IDebugCompiledExpression110;

#endif 	/* __IDebugCompiledExpression110_FWD_DEFINED__ */


#ifndef __IEnumGroupEvaluationResults110_FWD_DEFINED__
#define __IEnumGroupEvaluationResults110_FWD_DEFINED__
typedef interface IEnumGroupEvaluationResults110 IEnumGroupEvaluationResults110;

#endif 	/* __IEnumGroupEvaluationResults110_FWD_DEFINED__ */


#ifndef __IDebugGroupEvaluationCompleteEvent110_FWD_DEFINED__
#define __IDebugGroupEvaluationCompleteEvent110_FWD_DEFINED__
typedef interface IDebugGroupEvaluationCompleteEvent110 IDebugGroupEvaluationCompleteEvent110;

#endif 	/* __IDebugGroupEvaluationCompleteEvent110_FWD_DEFINED__ */


#ifndef __IVsCppDebugUIVisualizer_FWD_DEFINED__
#define __IVsCppDebugUIVisualizer_FWD_DEFINED__
typedef interface IVsCppDebugUIVisualizer IVsCppDebugUIVisualizer;

#endif 	/* __IVsCppDebugUIVisualizer_FWD_DEFINED__ */


#ifndef __IDebugRestrictedExceptionInfo110_FWD_DEFINED__
#define __IDebugRestrictedExceptionInfo110_FWD_DEFINED__
typedef interface IDebugRestrictedExceptionInfo110 IDebugRestrictedExceptionInfo110;

#endif 	/* __IDebugRestrictedExceptionInfo110_FWD_DEFINED__ */


#ifndef __IDebugSessionProcess110_FWD_DEFINED__
#define __IDebugSessionProcess110_FWD_DEFINED__
typedef interface IDebugSessionProcess110 IDebugSessionProcess110;

#endif 	/* __IDebugSessionProcess110_FWD_DEFINED__ */


#ifndef __IDebugBreakpointFileUpdateNotification110_FWD_DEFINED__
#define __IDebugBreakpointFileUpdateNotification110_FWD_DEFINED__
typedef interface IDebugBreakpointFileUpdateNotification110 IDebugBreakpointFileUpdateNotification110;

#endif 	/* __IDebugBreakpointFileUpdateNotification110_FWD_DEFINED__ */


#ifndef __IDebugProperty110_FWD_DEFINED__
#define __IDebugProperty110_FWD_DEFINED__
typedef interface IDebugProperty110 IDebugProperty110;

#endif 	/* __IDebugProperty110_FWD_DEFINED__ */


/* header files for imported files */
#include "oaidl.h"
#include "ocidl.h"
#include "msdbg.h"

#ifdef __cplusplus
extern "C"{
#endif 


/* interface __MIDL_itf_msdbg110_0000_0000 */
/* [local] */ 

typedef DWORD_PTR WIN32_HANDLE;

// Sentinel engine guid value which may be used to force usage of legacy (cpde.dll) remote managed debugging
// {351668CC-8477-4fbf-BFE3-5F1006E4DB1F}
extern GUID guidCOMPlusLegacyRemoteEngine;
// Sentinel engine guid value which may be used to force usage of new architecture (concord) local managed debugging
// {97552AEF-4F41-447a-BCC3-802EAA377343}
extern GUID guidCOMPlusNewArchEngine;
// {F4453496-1DB8-47F8-A7D5-31EBDDC2EC96}
extern GUID guidConcordGpuEng;















#define	MAX_DIM	( 10 )


enum enum_COMPUTE_THREAD_STATE
    {
        CTS_UNKNOWN	= 0,
        CTS_ACTIVE	= 0x1,
        CTS_DIVERGENT	= 0x2,
        CTS_BLOCKED	= 0x4,
        CTS_UNUSED	= 0x8,
        CTS_NOTSTARTED	= 0x10,
        CTS_COMPLETED	= 0x20
    } ;
typedef DWORD GPU_THREAD_STATE;


enum enum_QUERY_COMPUTETHREAD_INFO_FLAGS
    {
        QCTIF_NONE	= 0,
        QCTIF_THREAD_GROUP_ID	= 0x1,
        QCTIF_VECTOR_ID	= 0x2,
        QCTIF_THREAD_ID	= 0x4,
        QCTIF_THREAD_STATE	= 0x8,
        QCTIF_FLAG_STATE	= 0x10,
        QCTIF_INSTRUCTION_POINTER	= 0x20,
        QCTIF_FROZEN	= 0x40,
        QCTIF_ALL	= 0xffff
    } ;
typedef DWORD QUERY_COMPUTETHREAD_INFO_FLAGS;

typedef struct tagCOMPUTE_THREAD_INFO
    {
    UINT32 threadCount;
    UINT32 vectorId;
    UINT64 groupId;
    UINT64 threadId;
    UINT64 instructionPointer;
    GPU_THREAD_STATE threadState;
    BOOL flagged;
    BOOL frozen;
    } 	COMPUTE_THREAD_INFO;

typedef struct tagWHERE_CLAUSE
    {
    QUERY_COMPUTETHREAD_INFO_FLAGS columnFlags;
    COMPUTE_THREAD_INFO values;
    } 	WHERE_CLAUSE;



extern RPC_IF_HANDLE __MIDL_itf_msdbg110_0000_0000_v0_0_c_ifspec;
extern RPC_IF_HANDLE __MIDL_itf_msdbg110_0000_0000_v0_0_s_ifspec;

#ifndef __IQueryComputeThreadInfo110_INTERFACE_DEFINED__
#define __IQueryComputeThreadInfo110_INTERFACE_DEFINED__

/* interface IQueryComputeThreadInfo110 */
/* [unique][uuid][object] */ 


EXTERN_C const IID IID_IQueryComputeThreadInfo110;

#if defined(__cplusplus) && !defined(CINTERFACE)
    
    MIDL_INTERFACE("14A2ADDE-1F63-41B5-AD3E-06F6F8E4C0D5")
    IQueryComputeThreadInfo110 : public IUnknown
    {
    public:
        virtual HRESULT STDMETHODCALLTYPE Select( 
            /* [in] */ __RPC__in WHERE_CLAUSE *pWhereClause,
            /* [out] */ __RPC__deref_out_opt IEnumComputeThreadInfo110 **ppEnum) = 0;
        
        virtual HRESULT STDMETHODCALLTYPE GroupBy( 
            /* [in] */ QUERY_COMPUTETHREAD_INFO_FLAGS dwGroupByFlags,
            /* [in] */ __RPC__in WHERE_CLAUSE *pWhereClause,
            /* [out] */ __RPC__deref_out_opt IEnumComputeThreadInfo110 **ppEnum) = 0;
        
        virtual HRESULT STDMETHODCALLTYPE FindThreadsWithFrame( 
            /* [in] */ __RPC__in_opt IDebugStackFrame2 *pMatchThisFrame,
            /* [out] */ __RPC__deref_out_opt IEnumMatchedStackFrameInfo110 **ppEnum) = 0;
        
        virtual HRESULT STDMETHODCALLTYPE UpdateFlaggedState( 
            /* [in] */ __RPC__in WHERE_CLAUSE *pWhereClause,
            /* [in] */ BOOL fFlagged) = 0;
        
        virtual HRESULT STDMETHODCALLTYPE UpdateFrozenState( 
            /* [in] */ __RPC__in WHERE_CLAUSE *pWhereClause,
            /* [in] */ BOOL fFrozen) = 0;
        
        virtual HRESULT STDMETHODCALLTYPE UpdateThreadsBegin( void) = 0;
        
        virtual HRESULT STDMETHODCALLTYPE UpdateThreadsEnd( void) = 0;
        
    };
    
    
#else 	/* C style interface */

    typedef struct IQueryComputeThreadInfo110Vtbl
    {
        BEGIN_INTERFACE
        
        HRESULT ( STDMETHODCALLTYPE *QueryInterface )( 
            __RPC__in IQueryComputeThreadInfo110 * This,
            /* [in] */ __RPC__in REFIID riid,
            /* [annotation][iid_is][out] */ 
            _COM_Outptr_  void **ppvObject);
        
        ULONG ( STDMETHODCALLTYPE *AddRef )( 
            __RPC__in IQueryComputeThreadInfo110 * This);
        
        ULONG ( STDMETHODCALLTYPE *Release )( 
            __RPC__in IQueryComputeThreadInfo110 * This);
        
        HRESULT ( STDMETHODCALLTYPE *Select )( 
            __RPC__in IQueryComputeThreadInfo110 * This,
            /* [in] */ __RPC__in WHERE_CLAUSE *pWhereClause,
            /* [out] */ __RPC__deref_out_opt IEnumComputeThreadInfo110 **ppEnum);
        
        HRESULT ( STDMETHODCALLTYPE *GroupBy )( 
            __RPC__in IQueryComputeThreadInfo110 * This,
            /* [in] */ QUERY_COMPUTETHREAD_INFO_FLAGS dwGroupByFlags,
            /* [in] */ __RPC__in WHERE_CLAUSE *pWhereClause,
            /* [out] */ __RPC__deref_out_opt IEnumComputeThreadInfo110 **ppEnum);
        
        HRESULT ( STDMETHODCALLTYPE *FindThreadsWithFrame )( 
            __RPC__in IQueryComputeThreadInfo110 * This,
            /* [in] */ __RPC__in_opt IDebugStackFrame2 *pMatchThisFrame,
            /* [out] */ __RPC__deref_out_opt IEnumMatchedStackFrameInfo110 **ppEnum);
        
        HRESULT ( STDMETHODCALLTYPE *UpdateFlaggedState )( 
            __RPC__in IQueryComputeThreadInfo110 * This,
            /* [in] */ __RPC__in WHERE_CLAUSE *pWhereClause,
            /* [in] */ BOOL fFlagged);
        
        HRESULT ( STDMETHODCALLTYPE *UpdateFrozenState )( 
            __RPC__in IQueryComputeThreadInfo110 * This,
            /* [in] */ __RPC__in WHERE_CLAUSE *pWhereClause,
            /* [in] */ BOOL fFrozen);
        
        HRESULT ( STDMETHODCALLTYPE *UpdateThreadsBegin )( 
            __RPC__in IQueryComputeThreadInfo110 * This);
        
        HRESULT ( STDMETHODCALLTYPE *UpdateThreadsEnd )( 
            __RPC__in IQueryComputeThreadInfo110 * This);
        
        END_INTERFACE
    } IQueryComputeThreadInfo110Vtbl;

    interface IQueryComputeThreadInfo110
    {
        CONST_VTBL struct IQueryComputeThreadInfo110Vtbl *lpVtbl;
    };

    

#ifdef COBJMACROS


#define IQueryComputeThreadInfo110_QueryInterface(This,riid,ppvObject)	\
    ( (This)->lpVtbl -> QueryInterface(This,riid,ppvObject) ) 

#define IQueryComputeThreadInfo110_AddRef(This)	\
    ( (This)->lpVtbl -> AddRef(This) ) 

#define IQueryComputeThreadInfo110_Release(This)	\
    ( (This)->lpVtbl -> Release(This) ) 


#define IQueryComputeThreadInfo110_Select(This,pWhereClause,ppEnum)	\
    ( (This)->lpVtbl -> Select(This,pWhereClause,ppEnum) ) 

#define IQueryComputeThreadInfo110_GroupBy(This,dwGroupByFlags,pWhereClause,ppEnum)	\
    ( (This)->lpVtbl -> GroupBy(This,dwGroupByFlags,pWhereClause,ppEnum) ) 

#define IQueryComputeThreadInfo110_FindThreadsWithFrame(This,pMatchThisFrame,ppEnum)	\
    ( (This)->lpVtbl -> FindThreadsWithFrame(This,pMatchThisFrame,ppEnum) ) 

#define IQueryComputeThreadInfo110_UpdateFlaggedState(This,pWhereClause,fFlagged)	\
    ( (This)->lpVtbl -> UpdateFlaggedState(This,pWhereClause,fFlagged) ) 

#define IQueryComputeThreadInfo110_UpdateFrozenState(This,pWhereClause,fFrozen)	\
    ( (This)->lpVtbl -> UpdateFrozenState(This,pWhereClause,fFrozen) ) 

#define IQueryComputeThreadInfo110_UpdateThreadsBegin(This)	\
    ( (This)->lpVtbl -> UpdateThreadsBegin(This) ) 

#define IQueryComputeThreadInfo110_UpdateThreadsEnd(This)	\
    ( (This)->lpVtbl -> UpdateThreadsEnd(This) ) 

#endif /* COBJMACROS */


#endif 	/* C style interface */




#endif 	/* __IQueryComputeThreadInfo110_INTERFACE_DEFINED__ */


#ifndef __IEnumComputeThreadInfo110_INTERFACE_DEFINED__
#define __IEnumComputeThreadInfo110_INTERFACE_DEFINED__

/* interface IEnumComputeThreadInfo110 */
/* [unique][uuid][object] */ 


EXTERN_C const IID IID_IEnumComputeThreadInfo110;

#if defined(__cplusplus) && !defined(CINTERFACE)
    
    MIDL_INTERFACE("983C0EF8-F4F3-432C-8D4A-0866C55D4B79")
    IEnumComputeThreadInfo110 : public IUnknown
    {
    public:
        virtual HRESULT STDMETHODCALLTYPE Next( 
            /* [in] */ ULONG celt,
            /* [length_is][size_is][out] */ __RPC__out_ecount_part(celt, *pceltFetched) COMPUTE_THREAD_INFO *rgelt,
            /* [out][in] */ __RPC__inout ULONG *pceltFetched) = 0;
        
        virtual HRESULT STDMETHODCALLTYPE Skip( 
            /* [in] */ ULONG celt) = 0;
        
        virtual HRESULT STDMETHODCALLTYPE Reset( void) = 0;
        
        virtual HRESULT STDMETHODCALLTYPE Clone( 
            /* [out] */ __RPC__deref_out_opt IEnumComputeThreadInfo110 **ppEnum) = 0;
        
        virtual HRESULT STDMETHODCALLTYPE GetCount( 
            /* [out] */ __RPC__out ULONG *pcelt) = 0;
        
    };
    
    
#else 	/* C style interface */

    typedef struct IEnumComputeThreadInfo110Vtbl
    {
        BEGIN_INTERFACE
        
        HRESULT ( STDMETHODCALLTYPE *QueryInterface )( 
            __RPC__in IEnumComputeThreadInfo110 * This,
            /* [in] */ __RPC__in REFIID riid,
            /* [annotation][iid_is][out] */ 
            _COM_Outptr_  void **ppvObject);
        
        ULONG ( STDMETHODCALLTYPE *AddRef )( 
            __RPC__in IEnumComputeThreadInfo110 * This);
        
        ULONG ( STDMETHODCALLTYPE *Release )( 
            __RPC__in IEnumComputeThreadInfo110 * This);
        
        HRESULT ( STDMETHODCALLTYPE *Next )( 
            __RPC__in IEnumComputeThreadInfo110 * This,
            /* [in] */ ULONG celt,
            /* [length_is][size_is][out] */ __RPC__out_ecount_part(celt, *pceltFetched) COMPUTE_THREAD_INFO *rgelt,
            /* [out][in] */ __RPC__inout ULONG *pceltFetched);
        
        HRESULT ( STDMETHODCALLTYPE *Skip )( 
            __RPC__in IEnumComputeThreadInfo110 * This,
            /* [in] */ ULONG celt);
        
        HRESULT ( STDMETHODCALLTYPE *Reset )( 
            __RPC__in IEnumComputeThreadInfo110 * This);
        
        HRESULT ( STDMETHODCALLTYPE *Clone )( 
            __RPC__in IEnumComputeThreadInfo110 * This,
            /* [out] */ __RPC__deref_out_opt IEnumComputeThreadInfo110 **ppEnum);
        
        HRESULT ( STDMETHODCALLTYPE *GetCount )( 
            __RPC__in IEnumComputeThreadInfo110 * This,
            /* [out] */ __RPC__out ULONG *pcelt);
        
        END_INTERFACE
    } IEnumComputeThreadInfo110Vtbl;

    interface IEnumComputeThreadInfo110
    {
        CONST_VTBL struct IEnumComputeThreadInfo110Vtbl *lpVtbl;
    };

    

#ifdef COBJMACROS


#define IEnumComputeThreadInfo110_QueryInterface(This,riid,ppvObject)	\
    ( (This)->lpVtbl -> QueryInterface(This,riid,ppvObject) ) 

#define IEnumComputeThreadInfo110_AddRef(This)	\
    ( (This)->lpVtbl -> AddRef(This) ) 

#define IEnumComputeThreadInfo110_Release(This)	\
    ( (This)->lpVtbl -> Release(This) ) 


#define IEnumComputeThreadInfo110_Next(This,celt,rgelt,pceltFetched)	\
    ( (This)->lpVtbl -> Next(This,celt,rgelt,pceltFetched) ) 

#define IEnumComputeThreadInfo110_Skip(This,celt)	\
    ( (This)->lpVtbl -> Skip(This,celt) ) 

#define IEnumComputeThreadInfo110_Reset(This)	\
    ( (This)->lpVtbl -> Reset(This) ) 

#define IEnumComputeThreadInfo110_Clone(This,ppEnum)	\
    ( (This)->lpVtbl -> Clone(This,ppEnum) ) 

#define IEnumComputeThreadInfo110_GetCount(This,pcelt)	\
    ( (This)->lpVtbl -> GetCount(This,pcelt) ) 

#endif /* COBJMACROS */


#endif 	/* C style interface */




#endif 	/* __IEnumComputeThreadInfo110_INTERFACE_DEFINED__ */


/* interface __MIDL_itf_msdbg110_0000_0002 */
/* [local] */ 

typedef struct tagMATCHED_STACK_FRAME_INFO
    {
    UINT64 threadId;
    INT32 stackFrameIndex;
    } 	MATCHED_STACK_FRAME_INFO;



extern RPC_IF_HANDLE __MIDL_itf_msdbg110_0000_0002_v0_0_c_ifspec;
extern RPC_IF_HANDLE __MIDL_itf_msdbg110_0000_0002_v0_0_s_ifspec;

#ifndef __IEnumMatchedStackFrameInfo110_INTERFACE_DEFINED__
#define __IEnumMatchedStackFrameInfo110_INTERFACE_DEFINED__

/* interface IEnumMatchedStackFrameInfo110 */
/* [unique][uuid][object] */ 


EXTERN_C const IID IID_IEnumMatchedStackFrameInfo110;

#if defined(__cplusplus) && !defined(CINTERFACE)
    
    MIDL_INTERFACE("7007BEF7-8A7D-4539-BD92-9B177B75AEC9")
    IEnumMatchedStackFrameInfo110 : public IUnknown
    {
    public:
        virtual HRESULT STDMETHODCALLTYPE Next( 
            /* [in] */ ULONG celt,
            /* [length_is][size_is][out] */ __RPC__out_ecount_part(celt, *pceltFetched) MATCHED_STACK_FRAME_INFO *rgelt,
            /* [out][in] */ __RPC__inout ULONG *pceltFetched) = 0;
        
        virtual HRESULT STDMETHODCALLTYPE Skip( 
            /* [in] */ ULONG celt) = 0;
        
        virtual HRESULT STDMETHODCALLTYPE Reset( void) = 0;
        
        virtual HRESULT STDMETHODCALLTYPE Clone( 
            /* [out] */ __RPC__deref_out_opt IEnumMatchedStackFrameInfo110 **ppEnum) = 0;
        
        virtual HRESULT STDMETHODCALLTYPE GetCount( 
            /* [out] */ __RPC__out ULONG *pcelt) = 0;
        
    };
    
    
#else 	/* C style interface */

    typedef struct IEnumMatchedStackFrameInfo110Vtbl
    {
        BEGIN_INTERFACE
        
        HRESULT ( STDMETHODCALLTYPE *QueryInterface )( 
            __RPC__in IEnumMatchedStackFrameInfo110 * This,
            /* [in] */ __RPC__in REFIID riid,
            /* [annotation][iid_is][out] */ 
            _COM_Outptr_  void **ppvObject);
        
        ULONG ( STDMETHODCALLTYPE *AddRef )( 
            __RPC__in IEnumMatchedStackFrameInfo110 * This);
        
        ULONG ( STDMETHODCALLTYPE *Release )( 
            __RPC__in IEnumMatchedStackFrameInfo110 * This);
        
        HRESULT ( STDMETHODCALLTYPE *Next )( 
            __RPC__in IEnumMatchedStackFrameInfo110 * This,
            /* [in] */ ULONG celt,
            /* [length_is][size_is][out] */ __RPC__out_ecount_part(celt, *pceltFetched) MATCHED_STACK_FRAME_INFO *rgelt,
            /* [out][in] */ __RPC__inout ULONG *pceltFetched);
        
        HRESULT ( STDMETHODCALLTYPE *Skip )( 
            __RPC__in IEnumMatchedStackFrameInfo110 * This,
            /* [in] */ ULONG celt);
        
        HRESULT ( STDMETHODCALLTYPE *Reset )( 
            __RPC__in IEnumMatchedStackFrameInfo110 * This);
        
        HRESULT ( STDMETHODCALLTYPE *Clone )( 
            __RPC__in IEnumMatchedStackFrameInfo110 * This,
            /* [out] */ __RPC__deref_out_opt IEnumMatchedStackFrameInfo110 **ppEnum);
        
        HRESULT ( STDMETHODCALLTYPE *GetCount )( 
            __RPC__in IEnumMatchedStackFrameInfo110 * This,
            /* [out] */ __RPC__out ULONG *pcelt);
        
        END_INTERFACE
    } IEnumMatchedStackFrameInfo110Vtbl;

    interface IEnumMatchedStackFrameInfo110
    {
        CONST_VTBL struct IEnumMatchedStackFrameInfo110Vtbl *lpVtbl;
    };

    

#ifdef COBJMACROS


#define IEnumMatchedStackFrameInfo110_QueryInterface(This,riid,ppvObject)	\
    ( (This)->lpVtbl -> QueryInterface(This,riid,ppvObject) ) 

#define IEnumMatchedStackFrameInfo110_AddRef(This)	\
    ( (This)->lpVtbl -> AddRef(This) ) 

#define IEnumMatchedStackFrameInfo110_Release(This)	\
    ( (This)->lpVtbl -> Release(This) ) 


#define IEnumMatchedStackFrameInfo110_Next(This,celt,rgelt,pceltFetched)	\
    ( (This)->lpVtbl -> Next(This,celt,rgelt,pceltFetched) ) 

#define IEnumMatchedStackFrameInfo110_Skip(This,celt)	\
    ( (This)->lpVtbl -> Skip(This,celt) ) 

#define IEnumMatchedStackFrameInfo110_Reset(This)	\
    ( (This)->lpVtbl -> Reset(This) ) 

#define IEnumMatchedStackFrameInfo110_Clone(This,ppEnum)	\
    ( (This)->lpVtbl -> Clone(This,ppEnum) ) 

#define IEnumMatchedStackFrameInfo110_GetCount(This,pcelt)	\
    ( (This)->lpVtbl -> GetCount(This,pcelt) ) 

#endif /* COBJMACROS */


#endif 	/* C style interface */




#endif 	/* __IEnumMatchedStackFrameInfo110_INTERFACE_DEFINED__ */


#ifndef __IDebugComputeThread110_INTERFACE_DEFINED__
#define __IDebugComputeThread110_INTERFACE_DEFINED__

/* interface IDebugComputeThread110 */
/* [unique][uuid][object] */ 


EXTERN_C const IID IID_IDebugComputeThread110;

#if defined(__cplusplus) && !defined(CINTERFACE)
    
    MIDL_INTERFACE("E50A6E8D-74BA-4F69-956D-9C27D86F5922")
    IDebugComputeThread110 : public IUnknown
    {
    public:
        virtual HRESULT STDMETHODCALLTYPE GetComputeThreadInfo( 
            /* [out] */ __RPC__out COMPUTE_THREAD_INFO *pInfo) = 0;
        
        virtual HRESULT STDMETHODCALLTYPE GetComputeKernel( 
            /* [out] */ __RPC__deref_out_opt IDebugComputeKernel110 **ppComputeKernel) = 0;
        
    };
    
    
#else 	/* C style interface */

    typedef struct IDebugComputeThread110Vtbl
    {
        BEGIN_INTERFACE
        
        HRESULT ( STDMETHODCALLTYPE *QueryInterface )( 
            __RPC__in IDebugComputeThread110 * This,
            /* [in] */ __RPC__in REFIID riid,
            /* [annotation][iid_is][out] */ 
            _COM_Outptr_  void **ppvObject);
        
        ULONG ( STDMETHODCALLTYPE *AddRef )( 
            __RPC__in IDebugComputeThread110 * This);
        
        ULONG ( STDMETHODCALLTYPE *Release )( 
            __RPC__in IDebugComputeThread110 * This);
        
        HRESULT ( STDMETHODCALLTYPE *GetComputeThreadInfo )( 
            __RPC__in IDebugComputeThread110 * This,
            /* [out] */ __RPC__out COMPUTE_THREAD_INFO *pInfo);
        
        HRESULT ( STDMETHODCALLTYPE *GetComputeKernel )( 
            __RPC__in IDebugComputeThread110 * This,
            /* [out] */ __RPC__deref_out_opt IDebugComputeKernel110 **ppComputeKernel);
        
        END_INTERFACE
    } IDebugComputeThread110Vtbl;

    interface IDebugComputeThread110
    {
        CONST_VTBL struct IDebugComputeThread110Vtbl *lpVtbl;
    };

    

#ifdef COBJMACROS


#define IDebugComputeThread110_QueryInterface(This,riid,ppvObject)	\
    ( (This)->lpVtbl -> QueryInterface(This,riid,ppvObject) ) 

#define IDebugComputeThread110_AddRef(This)	\
    ( (This)->lpVtbl -> AddRef(This) ) 

#define IDebugComputeThread110_Release(This)	\
    ( (This)->lpVtbl -> Release(This) ) 


#define IDebugComputeThread110_GetComputeThreadInfo(This,pInfo)	\
    ( (This)->lpVtbl -> GetComputeThreadInfo(This,pInfo) ) 

#define IDebugComputeThread110_GetComputeKernel(This,ppComputeKernel)	\
    ( (This)->lpVtbl -> GetComputeKernel(This,ppComputeKernel) ) 

#endif /* COBJMACROS */


#endif 	/* C style interface */




#endif 	/* __IDebugComputeThread110_INTERFACE_DEFINED__ */


#ifndef __IDebugComputeKernel110_INTERFACE_DEFINED__
#define __IDebugComputeKernel110_INTERFACE_DEFINED__

/* interface IDebugComputeKernel110 */
/* [unique][uuid][object] */ 

#define	MAX_COMPUTE_DIM	( 10 )

typedef struct tagCOMPUTE_INFO
    {
    UINT32 numGroupDims;
    UINT32 groupDims[ 10 ];
    UINT32 numThreadDims;
    UINT32 threadDims[ 10 ];
    UINT32 numFlatThreadDims;
    UINT32 flatThreadDims[ 10 ];
    INT32 flatIndexBase[ 10 ];
    BOOL FlatModel;
    BSTR computeKernelName;
    BSTR groupName;
    } 	COMPUTE_INFO;

typedef struct tagPAIR
    {
    BSTR name;
    BSTR value;
    } 	Pair;


EXTERN_C const IID IID_IDebugComputeKernel110;

#if defined(__cplusplus) && !defined(CINTERFACE)
    
    MIDL_INTERFACE("ED1847BC-EF75-459F-A1DA-5042CF83F229")
    IDebugComputeKernel110 : public IUnknown
    {
    public:
        virtual HRESULT STDMETHODCALLTYPE GetComputeParameters( 
            /* [out] */ __RPC__out COMPUTE_INFO *pComputeInfo) = 0;
        
        virtual HRESULT STDMETHODCALLTYPE GetKernelProperties( 
            /* [in] */ ULONG cProperties,
            /* [length_is][size_is][out] */ __RPC__out_ecount_part(cProperties, *pcProperties) Pair *KernelProperties,
            /* [out][in] */ __RPC__inout ULONG *pcProperties) = 0;
        
        virtual HRESULT STDMETHODCALLTYPE GetProgram( 
            /* [out] */ __RPC__deref_out_opt IDebugProgram2 **ppProgram) = 0;
        
        virtual HRESULT STDMETHODCALLTYPE GetModule( 
            /* [out] */ __RPC__deref_out_opt IDebugModule2 **ppModule) = 0;
        
        virtual HRESULT STDMETHODCALLTYPE QueryComputeThreadInfo( 
            /* [out] */ __RPC__deref_out_opt IQueryComputeThreadInfo110 **ppEnum) = 0;
        
        virtual HRESULT STDMETHODCALLTYPE GetThreadForComputeThread( 
            /* [in] */ UINT64 nThreadId,
            /* [out] */ __RPC__deref_out_opt IDebugThread2 **ppThread) = 0;
        
        virtual HRESULT STDMETHODCALLTYPE GetComputeKernelId( 
            /* [out] */ __RPC__out UINT32 *pnKernelId) = 0;
        
    };
    
    
#else 	/* C style interface */

    typedef struct IDebugComputeKernel110Vtbl
    {
        BEGIN_INTERFACE
        
        HRESULT ( STDMETHODCALLTYPE *QueryInterface )( 
            __RPC__in IDebugComputeKernel110 * This,
            /* [in] */ __RPC__in REFIID riid,
            /* [annotation][iid_is][out] */ 
            _COM_Outptr_  void **ppvObject);
        
        ULONG ( STDMETHODCALLTYPE *AddRef )( 
            __RPC__in IDebugComputeKernel110 * This);
        
        ULONG ( STDMETHODCALLTYPE *Release )( 
            __RPC__in IDebugComputeKernel110 * This);
        
        HRESULT ( STDMETHODCALLTYPE *GetComputeParameters )( 
            __RPC__in IDebugComputeKernel110 * This,
            /* [out] */ __RPC__out COMPUTE_INFO *pComputeInfo);
        
        HRESULT ( STDMETHODCALLTYPE *GetKernelProperties )( 
            __RPC__in IDebugComputeKernel110 * This,
            /* [in] */ ULONG cProperties,
            /* [length_is][size_is][out] */ __RPC__out_ecount_part(cProperties, *pcProperties) Pair *KernelProperties,
            /* [out][in] */ __RPC__inout ULONG *pcProperties);
        
        HRESULT ( STDMETHODCALLTYPE *GetProgram )( 
            __RPC__in IDebugComputeKernel110 * This,
            /* [out] */ __RPC__deref_out_opt IDebugProgram2 **ppProgram);
        
        HRESULT ( STDMETHODCALLTYPE *GetModule )( 
            __RPC__in IDebugComputeKernel110 * This,
            /* [out] */ __RPC__deref_out_opt IDebugModule2 **ppModule);
        
        HRESULT ( STDMETHODCALLTYPE *QueryComputeThreadInfo )( 
            __RPC__in IDebugComputeKernel110 * This,
            /* [out] */ __RPC__deref_out_opt IQueryComputeThreadInfo110 **ppEnum);
        
        HRESULT ( STDMETHODCALLTYPE *GetThreadForComputeThread )( 
            __RPC__in IDebugComputeKernel110 * This,
            /* [in] */ UINT64 nThreadId,
            /* [out] */ __RPC__deref_out_opt IDebugThread2 **ppThread);
        
        HRESULT ( STDMETHODCALLTYPE *GetComputeKernelId )( 
            __RPC__in IDebugComputeKernel110 * This,
            /* [out] */ __RPC__out UINT32 *pnKernelId);
        
        END_INTERFACE
    } IDebugComputeKernel110Vtbl;

    interface IDebugComputeKernel110
    {
        CONST_VTBL struct IDebugComputeKernel110Vtbl *lpVtbl;
    };

    

#ifdef COBJMACROS


#define IDebugComputeKernel110_QueryInterface(This,riid,ppvObject)	\
    ( (This)->lpVtbl -> QueryInterface(This,riid,ppvObject) ) 

#define IDebugComputeKernel110_AddRef(This)	\
    ( (This)->lpVtbl -> AddRef(This) ) 

#define IDebugComputeKernel110_Release(This)	\
    ( (This)->lpVtbl -> Release(This) ) 


#define IDebugComputeKernel110_GetComputeParameters(This,pComputeInfo)	\
    ( (This)->lpVtbl -> GetComputeParameters(This,pComputeInfo) ) 

#define IDebugComputeKernel110_GetKernelProperties(This,cProperties,KernelProperties,pcProperties)	\
    ( (This)->lpVtbl -> GetKernelProperties(This,cProperties,KernelProperties,pcProperties) ) 

#define IDebugComputeKernel110_GetProgram(This,ppProgram)	\
    ( (This)->lpVtbl -> GetProgram(This,ppProgram) ) 

#define IDebugComputeKernel110_GetModule(This,ppModule)	\
    ( (This)->lpVtbl -> GetModule(This,ppModule) ) 

#define IDebugComputeKernel110_QueryComputeThreadInfo(This,ppEnum)	\
    ( (This)->lpVtbl -> QueryComputeThreadInfo(This,ppEnum) ) 

#define IDebugComputeKernel110_GetThreadForComputeThread(This,nThreadId,ppThread)	\
    ( (This)->lpVtbl -> GetThreadForComputeThread(This,nThreadId,ppThread) ) 

#define IDebugComputeKernel110_GetComputeKernelId(This,pnKernelId)	\
    ( (This)->lpVtbl -> GetComputeKernelId(This,pnKernelId) ) 

#endif /* COBJMACROS */


#endif 	/* C style interface */




#endif 	/* __IDebugComputeKernel110_INTERFACE_DEFINED__ */


#ifndef __IDebugProgram110_INTERFACE_DEFINED__
#define __IDebugProgram110_INTERFACE_DEFINED__

/* interface IDebugProgram110 */
/* [unique][uuid][object] */ 


EXTERN_C const IID IID_IDebugProgram110;

#if defined(__cplusplus) && !defined(CINTERFACE)
    
    MIDL_INTERFACE("007D8D53-ACBE-4FC6-99BE-6BBF13D9F8D3")
    IDebugProgram110 : public IUnknown
    {
    public:
        virtual HRESULT STDMETHODCALLTYPE EnumDebugComputeKernels( 
            /* [out] */ __RPC__deref_out_opt IEnumDebugComputeKernel110 **ppEnum) = 0;
        
    };
    
    
#else 	/* C style interface */

    typedef struct IDebugProgram110Vtbl
    {
        BEGIN_INTERFACE
        
        HRESULT ( STDMETHODCALLTYPE *QueryInterface )( 
            __RPC__in IDebugProgram110 * This,
            /* [in] */ __RPC__in REFIID riid,
            /* [annotation][iid_is][out] */ 
            _COM_Outptr_  void **ppvObject);
        
        ULONG ( STDMETHODCALLTYPE *AddRef )( 
            __RPC__in IDebugProgram110 * This);
        
        ULONG ( STDMETHODCALLTYPE *Release )( 
            __RPC__in IDebugProgram110 * This);
        
        HRESULT ( STDMETHODCALLTYPE *EnumDebugComputeKernels )( 
            __RPC__in IDebugProgram110 * This,
            /* [out] */ __RPC__deref_out_opt IEnumDebugComputeKernel110 **ppEnum);
        
        END_INTERFACE
    } IDebugProgram110Vtbl;

    interface IDebugProgram110
    {
        CONST_VTBL struct IDebugProgram110Vtbl *lpVtbl;
    };

    

#ifdef COBJMACROS


#define IDebugProgram110_QueryInterface(This,riid,ppvObject)	\
    ( (This)->lpVtbl -> QueryInterface(This,riid,ppvObject) ) 

#define IDebugProgram110_AddRef(This)	\
    ( (This)->lpVtbl -> AddRef(This) ) 

#define IDebugProgram110_Release(This)	\
    ( (This)->lpVtbl -> Release(This) ) 


#define IDebugProgram110_EnumDebugComputeKernels(This,ppEnum)	\
    ( (This)->lpVtbl -> EnumDebugComputeKernels(This,ppEnum) ) 

#endif /* COBJMACROS */


#endif 	/* C style interface */




#endif 	/* __IDebugProgram110_INTERFACE_DEFINED__ */


#ifndef __IDebugPropertyBag110_INTERFACE_DEFINED__
#define __IDebugPropertyBag110_INTERFACE_DEFINED__

/* interface IDebugPropertyBag110 */
/* [unique][uuid][object] */ 


EXTERN_C const IID IID_IDebugPropertyBag110;

#if defined(__cplusplus) && !defined(CINTERFACE)
    
    MIDL_INTERFACE("574F2363-BD5B-43D4-A30F-3A0C562D5BEF")
    IDebugPropertyBag110 : public IUnknown
    {
    public:
        virtual HRESULT STDMETHODCALLTYPE GetProperty( 
            /* [in] */ __RPC__in BSTR bstrKey,
            /* [out] */ __RPC__deref_out_opt BSTR *pbstrValue) = 0;
        
        virtual HRESULT STDMETHODCALLTYPE SetProperty( 
            /* [in] */ __RPC__in BSTR bstrKey,
            /* [in] */ __RPC__in BSTR bstrValue) = 0;
        
    };
    
    
#else 	/* C style interface */

    typedef struct IDebugPropertyBag110Vtbl
    {
        BEGIN_INTERFACE
        
        HRESULT ( STDMETHODCALLTYPE *QueryInterface )( 
            __RPC__in IDebugPropertyBag110 * This,
            /* [in] */ __RPC__in REFIID riid,
            /* [annotation][iid_is][out] */ 
            _COM_Outptr_  void **ppvObject);
        
        ULONG ( STDMETHODCALLTYPE *AddRef )( 
            __RPC__in IDebugPropertyBag110 * This);
        
        ULONG ( STDMETHODCALLTYPE *Release )( 
            __RPC__in IDebugPropertyBag110 * This);
        
        HRESULT ( STDMETHODCALLTYPE *GetProperty )( 
            __RPC__in IDebugPropertyBag110 * This,
            /* [in] */ __RPC__in BSTR bstrKey,
            /* [out] */ __RPC__deref_out_opt BSTR *pbstrValue);
        
        HRESULT ( STDMETHODCALLTYPE *SetProperty )( 
            __RPC__in IDebugPropertyBag110 * This,
            /* [in] */ __RPC__in BSTR bstrKey,
            /* [in] */ __RPC__in BSTR bstrValue);
        
        END_INTERFACE
    } IDebugPropertyBag110Vtbl;

    interface IDebugPropertyBag110
    {
        CONST_VTBL struct IDebugPropertyBag110Vtbl *lpVtbl;
    };

    

#ifdef COBJMACROS


#define IDebugPropertyBag110_QueryInterface(This,riid,ppvObject)	\
    ( (This)->lpVtbl -> QueryInterface(This,riid,ppvObject) ) 

#define IDebugPropertyBag110_AddRef(This)	\
    ( (This)->lpVtbl -> AddRef(This) ) 

#define IDebugPropertyBag110_Release(This)	\
    ( (This)->lpVtbl -> Release(This) ) 


#define IDebugPropertyBag110_GetProperty(This,bstrKey,pbstrValue)	\
    ( (This)->lpVtbl -> GetProperty(This,bstrKey,pbstrValue) ) 

#define IDebugPropertyBag110_SetProperty(This,bstrKey,bstrValue)	\
    ( (This)->lpVtbl -> SetProperty(This,bstrKey,bstrValue) ) 

#endif /* COBJMACROS */


#endif 	/* C style interface */




#endif 	/* __IDebugPropertyBag110_INTERFACE_DEFINED__ */


#ifndef __IEnumDebugComputeKernel110_INTERFACE_DEFINED__
#define __IEnumDebugComputeKernel110_INTERFACE_DEFINED__

/* interface IEnumDebugComputeKernel110 */
/* [unique][uuid][object] */ 


EXTERN_C const IID IID_IEnumDebugComputeKernel110;

#if defined(__cplusplus) && !defined(CINTERFACE)
    
    MIDL_INTERFACE("9A663B05-DC59-41D4-BEBF-A6A574D1D3EC")
    IEnumDebugComputeKernel110 : public IUnknown
    {
    public:
        virtual HRESULT STDMETHODCALLTYPE Next( 
            /* [in] */ ULONG celt,
            /* [length_is][size_is][out] */ __RPC__out_ecount_part(celt, *pceltFetched) IDebugComputeKernel110 **rgelt,
            /* [out][in] */ __RPC__inout ULONG *pceltFetched) = 0;
        
        virtual HRESULT STDMETHODCALLTYPE Skip( 
            /* [in] */ ULONG celt) = 0;
        
        virtual HRESULT STDMETHODCALLTYPE Reset( void) = 0;
        
        virtual HRESULT STDMETHODCALLTYPE Clone( 
            /* [out] */ __RPC__deref_out_opt IEnumDebugComputeKernel110 **ppEnum) = 0;
        
        virtual HRESULT STDMETHODCALLTYPE GetCount( 
            /* [out] */ __RPC__out ULONG *pcelt) = 0;
        
    };
    
    
#else 	/* C style interface */

    typedef struct IEnumDebugComputeKernel110Vtbl
    {
        BEGIN_INTERFACE
        
        HRESULT ( STDMETHODCALLTYPE *QueryInterface )( 
            __RPC__in IEnumDebugComputeKernel110 * This,
            /* [in] */ __RPC__in REFIID riid,
            /* [annotation][iid_is][out] */ 
            _COM_Outptr_  void **ppvObject);
        
        ULONG ( STDMETHODCALLTYPE *AddRef )( 
            __RPC__in IEnumDebugComputeKernel110 * This);
        
        ULONG ( STDMETHODCALLTYPE *Release )( 
            __RPC__in IEnumDebugComputeKernel110 * This);
        
        HRESULT ( STDMETHODCALLTYPE *Next )( 
            __RPC__in IEnumDebugComputeKernel110 * This,
            /* [in] */ ULONG celt,
            /* [length_is][size_is][out] */ __RPC__out_ecount_part(celt, *pceltFetched) IDebugComputeKernel110 **rgelt,
            /* [out][in] */ __RPC__inout ULONG *pceltFetched);
        
        HRESULT ( STDMETHODCALLTYPE *Skip )( 
            __RPC__in IEnumDebugComputeKernel110 * This,
            /* [in] */ ULONG celt);
        
        HRESULT ( STDMETHODCALLTYPE *Reset )( 
            __RPC__in IEnumDebugComputeKernel110 * This);
        
        HRESULT ( STDMETHODCALLTYPE *Clone )( 
            __RPC__in IEnumDebugComputeKernel110 * This,
            /* [out] */ __RPC__deref_out_opt IEnumDebugComputeKernel110 **ppEnum);
        
        HRESULT ( STDMETHODCALLTYPE *GetCount )( 
            __RPC__in IEnumDebugComputeKernel110 * This,
            /* [out] */ __RPC__out ULONG *pcelt);
        
        END_INTERFACE
    } IEnumDebugComputeKernel110Vtbl;

    interface IEnumDebugComputeKernel110
    {
        CONST_VTBL struct IEnumDebugComputeKernel110Vtbl *lpVtbl;
    };

    

#ifdef COBJMACROS


#define IEnumDebugComputeKernel110_QueryInterface(This,riid,ppvObject)	\
    ( (This)->lpVtbl -> QueryInterface(This,riid,ppvObject) ) 

#define IEnumDebugComputeKernel110_AddRef(This)	\
    ( (This)->lpVtbl -> AddRef(This) ) 

#define IEnumDebugComputeKernel110_Release(This)	\
    ( (This)->lpVtbl -> Release(This) ) 


#define IEnumDebugComputeKernel110_Next(This,celt,rgelt,pceltFetched)	\
    ( (This)->lpVtbl -> Next(This,celt,rgelt,pceltFetched) ) 

#define IEnumDebugComputeKernel110_Skip(This,celt)	\
    ( (This)->lpVtbl -> Skip(This,celt) ) 

#define IEnumDebugComputeKernel110_Reset(This)	\
    ( (This)->lpVtbl -> Reset(This) ) 

#define IEnumDebugComputeKernel110_Clone(This,ppEnum)	\
    ( (This)->lpVtbl -> Clone(This,ppEnum) ) 

#define IEnumDebugComputeKernel110_GetCount(This,pcelt)	\
    ( (This)->lpVtbl -> GetCount(This,pcelt) ) 

#endif /* COBJMACROS */


#endif 	/* C style interface */




#endif 	/* __IEnumDebugComputeKernel110_INTERFACE_DEFINED__ */


#ifndef __IDebugComputeThreadPropertyChangedEvent110_INTERFACE_DEFINED__
#define __IDebugComputeThreadPropertyChangedEvent110_INTERFACE_DEFINED__

/* interface IDebugComputeThreadPropertyChangedEvent110 */
/* [unique][uuid][object] */ 


EXTERN_C const IID IID_IDebugComputeThreadPropertyChangedEvent110;

#if defined(__cplusplus) && !defined(CINTERFACE)
    
    MIDL_INTERFACE("9DAACCD8-4368-4391-B8D5-3E4A6B794A20")
    IDebugComputeThreadPropertyChangedEvent110 : public IUnknown
    {
    public:
    };
    
    
#else 	/* C style interface */

    typedef struct IDebugComputeThreadPropertyChangedEvent110Vtbl
    {
        BEGIN_INTERFACE
        
        HRESULT ( STDMETHODCALLTYPE *QueryInterface )( 
            __RPC__in IDebugComputeThreadPropertyChangedEvent110 * This,
            /* [in] */ __RPC__in REFIID riid,
            /* [annotation][iid_is][out] */ 
            _COM_Outptr_  void **ppvObject);
        
        ULONG ( STDMETHODCALLTYPE *AddRef )( 
            __RPC__in IDebugComputeThreadPropertyChangedEvent110 * This);
        
        ULONG ( STDMETHODCALLTYPE *Release )( 
            __RPC__in IDebugComputeThreadPropertyChangedEvent110 * This);
        
        END_INTERFACE
    } IDebugComputeThreadPropertyChangedEvent110Vtbl;

    interface IDebugComputeThreadPropertyChangedEvent110
    {
        CONST_VTBL struct IDebugComputeThreadPropertyChangedEvent110Vtbl *lpVtbl;
    };

    

#ifdef COBJMACROS


#define IDebugComputeThreadPropertyChangedEvent110_QueryInterface(This,riid,ppvObject)	\
    ( (This)->lpVtbl -> QueryInterface(This,riid,ppvObject) ) 

#define IDebugComputeThreadPropertyChangedEvent110_AddRef(This)	\
    ( (This)->lpVtbl -> AddRef(This) ) 

#define IDebugComputeThreadPropertyChangedEvent110_Release(This)	\
    ( (This)->lpVtbl -> Release(This) ) 


#endif /* COBJMACROS */


#endif 	/* C style interface */




#endif 	/* __IDebugComputeThreadPropertyChangedEvent110_INTERFACE_DEFINED__ */


#ifndef __IDebugComputeKernelDispatchEvent110_INTERFACE_DEFINED__
#define __IDebugComputeKernelDispatchEvent110_INTERFACE_DEFINED__

/* interface IDebugComputeKernelDispatchEvent110 */
/* [unique][uuid][object] */ 


EXTERN_C const IID IID_IDebugComputeKernelDispatchEvent110;

#if defined(__cplusplus) && !defined(CINTERFACE)
    
    MIDL_INTERFACE("E157B5E4-7380-4516-9624-C5BA188D8431")
    IDebugComputeKernelDispatchEvent110 : public IUnknown
    {
    public:
        virtual HRESULT STDMETHODCALLTYPE GetComputeKernel( 
            /* [out] */ __RPC__deref_out_opt IDebugComputeKernel110 **pComputeKernel,
            /* [out][in] */ __RPC__inout BOOL *pbDispatch) = 0;
        
    };
    
    
#else 	/* C style interface */

    typedef struct IDebugComputeKernelDispatchEvent110Vtbl
    {
        BEGIN_INTERFACE
        
        HRESULT ( STDMETHODCALLTYPE *QueryInterface )( 
            __RPC__in IDebugComputeKernelDispatchEvent110 * This,
            /* [in] */ __RPC__in REFIID riid,
            /* [annotation][iid_is][out] */ 
            _COM_Outptr_  void **ppvObject);
        
        ULONG ( STDMETHODCALLTYPE *AddRef )( 
            __RPC__in IDebugComputeKernelDispatchEvent110 * This);
        
        ULONG ( STDMETHODCALLTYPE *Release )( 
            __RPC__in IDebugComputeKernelDispatchEvent110 * This);
        
        HRESULT ( STDMETHODCALLTYPE *GetComputeKernel )( 
            __RPC__in IDebugComputeKernelDispatchEvent110 * This,
            /* [out] */ __RPC__deref_out_opt IDebugComputeKernel110 **pComputeKernel,
            /* [out][in] */ __RPC__inout BOOL *pbDispatch);
        
        END_INTERFACE
    } IDebugComputeKernelDispatchEvent110Vtbl;

    interface IDebugComputeKernelDispatchEvent110
    {
        CONST_VTBL struct IDebugComputeKernelDispatchEvent110Vtbl *lpVtbl;
    };

    

#ifdef COBJMACROS


#define IDebugComputeKernelDispatchEvent110_QueryInterface(This,riid,ppvObject)	\
    ( (This)->lpVtbl -> QueryInterface(This,riid,ppvObject) ) 

#define IDebugComputeKernelDispatchEvent110_AddRef(This)	\
    ( (This)->lpVtbl -> AddRef(This) ) 

#define IDebugComputeKernelDispatchEvent110_Release(This)	\
    ( (This)->lpVtbl -> Release(This) ) 


#define IDebugComputeKernelDispatchEvent110_GetComputeKernel(This,pComputeKernel,pbDispatch)	\
    ( (This)->lpVtbl -> GetComputeKernel(This,pComputeKernel,pbDispatch) ) 

#endif /* COBJMACROS */


#endif 	/* C style interface */




#endif 	/* __IDebugComputeKernelDispatchEvent110_INTERFACE_DEFINED__ */


#ifndef __IDebugStackFrame110_INTERFACE_DEFINED__
#define __IDebugStackFrame110_INTERFACE_DEFINED__

/* interface IDebugStackFrame110 */
/* [unique][uuid][object] */ 


EXTERN_C const IID IID_IDebugStackFrame110;

#if defined(__cplusplus) && !defined(CINTERFACE)
    
    MIDL_INTERFACE("5CFCAF33-9B58-49B3-9BE3-E47BEC9FA20C")
    IDebugStackFrame110 : public IUnknown
    {
    public:
        virtual HRESULT STDMETHODCALLTYPE ShouldShowPropagateSideEffectsIcon( 
            /* [out] */ __RPC__out BOOL *pfShowIcon) = 0;
        
        virtual HRESULT STDMETHODCALLTYPE GetExpressionCompilationContext( 
            /* [out] */ __RPC__deref_out_opt IDebugExpressionCompilationContext110 **ppExprCxt) = 0;
        
    };
    
    
#else 	/* C style interface */

    typedef struct IDebugStackFrame110Vtbl
    {
        BEGIN_INTERFACE
        
        HRESULT ( STDMETHODCALLTYPE *QueryInterface )( 
            __RPC__in IDebugStackFrame110 * This,
            /* [in] */ __RPC__in REFIID riid,
            /* [annotation][iid_is][out] */ 
            _COM_Outptr_  void **ppvObject);
        
        ULONG ( STDMETHODCALLTYPE *AddRef )( 
            __RPC__in IDebugStackFrame110 * This);
        
        ULONG ( STDMETHODCALLTYPE *Release )( 
            __RPC__in IDebugStackFrame110 * This);
        
        HRESULT ( STDMETHODCALLTYPE *ShouldShowPropagateSideEffectsIcon )( 
            __RPC__in IDebugStackFrame110 * This,
            /* [out] */ __RPC__out BOOL *pfShowIcon);
        
        HRESULT ( STDMETHODCALLTYPE *GetExpressionCompilationContext )( 
            __RPC__in IDebugStackFrame110 * This,
            /* [out] */ __RPC__deref_out_opt IDebugExpressionCompilationContext110 **ppExprCxt);
        
        END_INTERFACE
    } IDebugStackFrame110Vtbl;

    interface IDebugStackFrame110
    {
        CONST_VTBL struct IDebugStackFrame110Vtbl *lpVtbl;
    };

    

#ifdef COBJMACROS


#define IDebugStackFrame110_QueryInterface(This,riid,ppvObject)	\
    ( (This)->lpVtbl -> QueryInterface(This,riid,ppvObject) ) 

#define IDebugStackFrame110_AddRef(This)	\
    ( (This)->lpVtbl -> AddRef(This) ) 

#define IDebugStackFrame110_Release(This)	\
    ( (This)->lpVtbl -> Release(This) ) 


#define IDebugStackFrame110_ShouldShowPropagateSideEffectsIcon(This,pfShowIcon)	\
    ( (This)->lpVtbl -> ShouldShowPropagateSideEffectsIcon(This,pfShowIcon) ) 

#define IDebugStackFrame110_GetExpressionCompilationContext(This,ppExprCxt)	\
    ( (This)->lpVtbl -> GetExpressionCompilationContext(This,ppExprCxt) ) 

#endif /* COBJMACROS */


#endif 	/* C style interface */




#endif 	/* __IDebugStackFrame110_INTERFACE_DEFINED__ */


#ifndef __IDebugExpressionCompilationContext110_INTERFACE_DEFINED__
#define __IDebugExpressionCompilationContext110_INTERFACE_DEFINED__

/* interface IDebugExpressionCompilationContext110 */
/* [unique][uuid][object] */ 


EXTERN_C const IID IID_IDebugExpressionCompilationContext110;

#if defined(__cplusplus) && !defined(CINTERFACE)
    
    MIDL_INTERFACE("3DE09ACE-D2B6-4054-B17B-1120B0160283")
    IDebugExpressionCompilationContext110 : public IUnknown
    {
    public:
        virtual HRESULT STDMETHODCALLTYPE CompileText( 
            /* [in] */ __RPC__in LPCOLESTR pszCode,
            /* [in] */ DWORD evalFlags,
            /* [in] */ UINT32 nRadix,
            /* [out] */ __RPC__deref_out_opt IDebugCompiledExpression110 **ppExpr,
            /* [out] */ __RPC__deref_out_opt BSTR *pbstrError,
            /* [out] */ __RPC__out UINT32 *pichError) = 0;
        
    };
    
    
#else 	/* C style interface */

    typedef struct IDebugExpressionCompilationContext110Vtbl
    {
        BEGIN_INTERFACE
        
        HRESULT ( STDMETHODCALLTYPE *QueryInterface )( 
            __RPC__in IDebugExpressionCompilationContext110 * This,
            /* [in] */ __RPC__in REFIID riid,
            /* [annotation][iid_is][out] */ 
            _COM_Outptr_  void **ppvObject);
        
        ULONG ( STDMETHODCALLTYPE *AddRef )( 
            __RPC__in IDebugExpressionCompilationContext110 * This);
        
        ULONG ( STDMETHODCALLTYPE *Release )( 
            __RPC__in IDebugExpressionCompilationContext110 * This);
        
        HRESULT ( STDMETHODCALLTYPE *CompileText )( 
            __RPC__in IDebugExpressionCompilationContext110 * This,
            /* [in] */ __RPC__in LPCOLESTR pszCode,
            /* [in] */ DWORD evalFlags,
            /* [in] */ UINT32 nRadix,
            /* [out] */ __RPC__deref_out_opt IDebugCompiledExpression110 **ppExpr,
            /* [out] */ __RPC__deref_out_opt BSTR *pbstrError,
            /* [out] */ __RPC__out UINT32 *pichError);
        
        END_INTERFACE
    } IDebugExpressionCompilationContext110Vtbl;

    interface IDebugExpressionCompilationContext110
    {
        CONST_VTBL struct IDebugExpressionCompilationContext110Vtbl *lpVtbl;
    };

    

#ifdef COBJMACROS


#define IDebugExpressionCompilationContext110_QueryInterface(This,riid,ppvObject)	\
    ( (This)->lpVtbl -> QueryInterface(This,riid,ppvObject) ) 

#define IDebugExpressionCompilationContext110_AddRef(This)	\
    ( (This)->lpVtbl -> AddRef(This) ) 

#define IDebugExpressionCompilationContext110_Release(This)	\
    ( (This)->lpVtbl -> Release(This) ) 


#define IDebugExpressionCompilationContext110_CompileText(This,pszCode,evalFlags,nRadix,ppExpr,pbstrError,pichError)	\
    ( (This)->lpVtbl -> CompileText(This,pszCode,evalFlags,nRadix,ppExpr,pbstrError,pichError) ) 

#endif /* COBJMACROS */


#endif 	/* C style interface */




#endif 	/* __IDebugExpressionCompilationContext110_INTERFACE_DEFINED__ */


#ifndef __IDebugCompiledExpression110_INTERFACE_DEFINED__
#define __IDebugCompiledExpression110_INTERFACE_DEFINED__

/* interface IDebugCompiledExpression110 */
/* [unique][uuid][object] */ 


EXTERN_C const IID IID_IDebugCompiledExpression110;

#if defined(__cplusplus) && !defined(CINTERFACE)
    
    MIDL_INTERFACE("A528D84A-06D7-48B0-8825-614F4C5C1F68")
    IDebugCompiledExpression110 : public IUnknown
    {
    public:
        virtual HRESULT STDMETHODCALLTYPE GetExpressionContext( 
            /* [out] */ __RPC__deref_out_opt IDebugExpressionCompilationContext110 **ppExpressionContext) = 0;
        
        virtual HRESULT STDMETHODCALLTYPE GroupEvaluateAsync( 
            /* [in] */ ULONG cNumThreads,
            /* [size_is][in] */ __RPC__in_ecount_full(cNumThreads) UINT64 *rgComputeThreadIds) = 0;
        
        virtual HRESULT STDMETHODCALLTYPE Abort( void) = 0;
        
    };
    
    
#else 	/* C style interface */

    typedef struct IDebugCompiledExpression110Vtbl
    {
        BEGIN_INTERFACE
        
        HRESULT ( STDMETHODCALLTYPE *QueryInterface )( 
            __RPC__in IDebugCompiledExpression110 * This,
            /* [in] */ __RPC__in REFIID riid,
            /* [annotation][iid_is][out] */ 
            _COM_Outptr_  void **ppvObject);
        
        ULONG ( STDMETHODCALLTYPE *AddRef )( 
            __RPC__in IDebugCompiledExpression110 * This);
        
        ULONG ( STDMETHODCALLTYPE *Release )( 
            __RPC__in IDebugCompiledExpression110 * This);
        
        HRESULT ( STDMETHODCALLTYPE *GetExpressionContext )( 
            __RPC__in IDebugCompiledExpression110 * This,
            /* [out] */ __RPC__deref_out_opt IDebugExpressionCompilationContext110 **ppExpressionContext);
        
        HRESULT ( STDMETHODCALLTYPE *GroupEvaluateAsync )( 
            __RPC__in IDebugCompiledExpression110 * This,
            /* [in] */ ULONG cNumThreads,
            /* [size_is][in] */ __RPC__in_ecount_full(cNumThreads) UINT64 *rgComputeThreadIds);
        
        HRESULT ( STDMETHODCALLTYPE *Abort )( 
            __RPC__in IDebugCompiledExpression110 * This);
        
        END_INTERFACE
    } IDebugCompiledExpression110Vtbl;

    interface IDebugCompiledExpression110
    {
        CONST_VTBL struct IDebugCompiledExpression110Vtbl *lpVtbl;
    };

    

#ifdef COBJMACROS


#define IDebugCompiledExpression110_QueryInterface(This,riid,ppvObject)	\
    ( (This)->lpVtbl -> QueryInterface(This,riid,ppvObject) ) 

#define IDebugCompiledExpression110_AddRef(This)	\
    ( (This)->lpVtbl -> AddRef(This) ) 

#define IDebugCompiledExpression110_Release(This)	\
    ( (This)->lpVtbl -> Release(This) ) 


#define IDebugCompiledExpression110_GetExpressionContext(This,ppExpressionContext)	\
    ( (This)->lpVtbl -> GetExpressionContext(This,ppExpressionContext) ) 

#define IDebugCompiledExpression110_GroupEvaluateAsync(This,cNumThreads,rgComputeThreadIds)	\
    ( (This)->lpVtbl -> GroupEvaluateAsync(This,cNumThreads,rgComputeThreadIds) ) 

#define IDebugCompiledExpression110_Abort(This)	\
    ( (This)->lpVtbl -> Abort(This) ) 

#endif /* COBJMACROS */


#endif 	/* C style interface */




#endif 	/* __IDebugCompiledExpression110_INTERFACE_DEFINED__ */


#ifndef __IEnumGroupEvaluationResultContext110_INTERFACE_DEFINED__
#define __IEnumGroupEvaluationResultContext110_INTERFACE_DEFINED__

/* interface IEnumGroupEvaluationResultContext110 */
/* [unique][uuid][object] */ 


EXTERN_C const IID IID_IEnumGroupEvaluationResultContext110;

#if defined(__cplusplus) && !defined(CINTERFACE)
    
    MIDL_INTERFACE("CB9D13F7-B6EC-4971-8075-7A21554D0F4B")
    IEnumGroupEvaluationResultContext110 : public IUnknown
    {
    public:
        virtual HRESULT STDMETHODCALLTYPE Next( 
            /* [in] */ ULONG cResultBufferSize,
            /* [length_is][size_is][out] */ __RPC__out_ecount_part(cResultBufferSize, *pcResultsFetched) UINT64 *rgComputeThreadIds,
            /* [length_is][size_is][out] */ __RPC__out_ecount_part(cResultBufferSize, *pcResultsFetched) BSTR *rgEvaluationValues,
            /* [out] */ __RPC__out ULONG *pcResultsFetched) = 0;
        
        virtual HRESULT STDMETHODCALLTYPE Skip( 
            /* [in] */ ULONG cResultsToSkip) = 0;
        
        virtual HRESULT STDMETHODCALLTYPE Reset( void) = 0;
        
        virtual HRESULT STDMETHODCALLTYPE Clone( 
            /* [out] */ __RPC__deref_out_opt IEnumGroupEvaluationResultContext110 **ppEnum) = 0;
        
        virtual HRESULT STDMETHODCALLTYPE GetCount( 
            /* [out] */ __RPC__out ULONG *pcTotalResults) = 0;
        
        virtual HRESULT STDMETHODCALLTYPE GetCanonicalProperty( 
            /* [out] */ __RPC__deref_out_opt IDebugProperty2 **ppCanonicalProperty) = 0;
        
        virtual HRESULT STDMETHODCALLTYPE GetEvaluationResult( void) = 0;
        
    };
    
    
#else 	/* C style interface */

    typedef struct IEnumGroupEvaluationResultContext110Vtbl
    {
        BEGIN_INTERFACE
        
        HRESULT ( STDMETHODCALLTYPE *QueryInterface )( 
            __RPC__in IEnumGroupEvaluationResultContext110 * This,
            /* [in] */ __RPC__in REFIID riid,
            /* [annotation][iid_is][out] */ 
            _COM_Outptr_  void **ppvObject);
        
        ULONG ( STDMETHODCALLTYPE *AddRef )( 
            __RPC__in IEnumGroupEvaluationResultContext110 * This);
        
        ULONG ( STDMETHODCALLTYPE *Release )( 
            __RPC__in IEnumGroupEvaluationResultContext110 * This);
        
        HRESULT ( STDMETHODCALLTYPE *Next )( 
            __RPC__in IEnumGroupEvaluationResultContext110 * This,
            /* [in] */ ULONG cResultBufferSize,
            /* [length_is][size_is][out] */ __RPC__out_ecount_part(cResultBufferSize, *pcResultsFetched) UINT64 *rgComputeThreadIds,
            /* [length_is][size_is][out] */ __RPC__out_ecount_part(cResultBufferSize, *pcResultsFetched) BSTR *rgEvaluationValues,
            /* [out] */ __RPC__out ULONG *pcResultsFetched);
        
        HRESULT ( STDMETHODCALLTYPE *Skip )( 
            __RPC__in IEnumGroupEvaluationResultContext110 * This,
            /* [in] */ ULONG cResultsToSkip);
        
        HRESULT ( STDMETHODCALLTYPE *Reset )( 
            __RPC__in IEnumGroupEvaluationResultContext110 * This);
        
        HRESULT ( STDMETHODCALLTYPE *Clone )( 
            __RPC__in IEnumGroupEvaluationResultContext110 * This,
            /* [out] */ __RPC__deref_out_opt IEnumGroupEvaluationResultContext110 **ppEnum);
        
        HRESULT ( STDMETHODCALLTYPE *GetCount )( 
            __RPC__in IEnumGroupEvaluationResultContext110 * This,
            /* [out] */ __RPC__out ULONG *pcTotalResults);
        
        HRESULT ( STDMETHODCALLTYPE *GetCanonicalProperty )( 
            __RPC__in IEnumGroupEvaluationResultContext110 * This,
            /* [out] */ __RPC__deref_out_opt IDebugProperty2 **ppCanonicalProperty);
        
        HRESULT ( STDMETHODCALLTYPE *GetEvaluationResult )( 
            __RPC__in IEnumGroupEvaluationResultContext110 * This);
        
        END_INTERFACE
    } IEnumGroupEvaluationResultContext110Vtbl;

    interface IEnumGroupEvaluationResultContext110
    {
        CONST_VTBL struct IEnumGroupEvaluationResultContext110Vtbl *lpVtbl;
    };

    

#ifdef COBJMACROS


#define IEnumGroupEvaluationResultContext110_QueryInterface(This,riid,ppvObject)	\
    ( (This)->lpVtbl -> QueryInterface(This,riid,ppvObject) ) 

#define IEnumGroupEvaluationResultContext110_AddRef(This)	\
    ( (This)->lpVtbl -> AddRef(This) ) 

#define IEnumGroupEvaluationResultContext110_Release(This)	\
    ( (This)->lpVtbl -> Release(This) ) 


#define IEnumGroupEvaluationResultContext110_Next(This,cResultBufferSize,rgComputeThreadIds,rgEvaluationValues,pcResultsFetched)	\
    ( (This)->lpVtbl -> Next(This,cResultBufferSize,rgComputeThreadIds,rgEvaluationValues,pcResultsFetched) ) 

#define IEnumGroupEvaluationResultContext110_Skip(This,cResultsToSkip)	\
    ( (This)->lpVtbl -> Skip(This,cResultsToSkip) ) 

#define IEnumGroupEvaluationResultContext110_Reset(This)	\
    ( (This)->lpVtbl -> Reset(This) ) 

#define IEnumGroupEvaluationResultContext110_Clone(This,ppEnum)	\
    ( (This)->lpVtbl -> Clone(This,ppEnum) ) 

#define IEnumGroupEvaluationResultContext110_GetCount(This,pcTotalResults)	\
    ( (This)->lpVtbl -> GetCount(This,pcTotalResults) ) 

#define IEnumGroupEvaluationResultContext110_GetCanonicalProperty(This,ppCanonicalProperty)	\
    ( (This)->lpVtbl -> GetCanonicalProperty(This,ppCanonicalProperty) ) 

#define IEnumGroupEvaluationResultContext110_GetEvaluationResult(This)	\
    ( (This)->lpVtbl -> GetEvaluationResult(This) ) 

#endif /* COBJMACROS */


#endif 	/* C style interface */




#endif 	/* __IEnumGroupEvaluationResultContext110_INTERFACE_DEFINED__ */


#ifndef __IEnumGroupEvaluationResults110_INTERFACE_DEFINED__
#define __IEnumGroupEvaluationResults110_INTERFACE_DEFINED__

/* interface IEnumGroupEvaluationResults110 */
/* [unique][uuid][object] */ 


EXTERN_C const IID IID_IEnumGroupEvaluationResults110;

#if defined(__cplusplus) && !defined(CINTERFACE)
    
    MIDL_INTERFACE("B9AB98D4-0454-4093-8793-2EBA79E8C66E")
    IEnumGroupEvaluationResults110 : public IUnknown
    {
    public:
        virtual HRESULT STDMETHODCALLTYPE Next( 
            /* [in] */ ULONG cResultBufferSize,
            /* [length_is][size_is][out] */ __RPC__out_ecount_part(cResultBufferSize, *pcResultsFetched) IEnumGroupEvaluationResultContext110 **ppResultContexts,
            /* [out] */ __RPC__out ULONG *pcResultsFetched) = 0;
        
        virtual HRESULT STDMETHODCALLTYPE Skip( 
            /* [in] */ ULONG cResultsToSkip) = 0;
        
        virtual HRESULT STDMETHODCALLTYPE Reset( void) = 0;
        
        virtual HRESULT STDMETHODCALLTYPE Clone( 
            /* [out] */ __RPC__deref_out_opt IEnumGroupEvaluationResults110 **ppEnum) = 0;
        
        virtual HRESULT STDMETHODCALLTYPE GetCount( 
            /* [out] */ __RPC__out ULONG *pcTotalResults) = 0;
        
    };
    
    
#else 	/* C style interface */

    typedef struct IEnumGroupEvaluationResults110Vtbl
    {
        BEGIN_INTERFACE
        
        HRESULT ( STDMETHODCALLTYPE *QueryInterface )( 
            __RPC__in IEnumGroupEvaluationResults110 * This,
            /* [in] */ __RPC__in REFIID riid,
            /* [annotation][iid_is][out] */ 
            _COM_Outptr_  void **ppvObject);
        
        ULONG ( STDMETHODCALLTYPE *AddRef )( 
            __RPC__in IEnumGroupEvaluationResults110 * This);
        
        ULONG ( STDMETHODCALLTYPE *Release )( 
            __RPC__in IEnumGroupEvaluationResults110 * This);
        
        HRESULT ( STDMETHODCALLTYPE *Next )( 
            __RPC__in IEnumGroupEvaluationResults110 * This,
            /* [in] */ ULONG cResultBufferSize,
            /* [length_is][size_is][out] */ __RPC__out_ecount_part(cResultBufferSize, *pcResultsFetched) IEnumGroupEvaluationResultContext110 **ppResultContexts,
            /* [out] */ __RPC__out ULONG *pcResultsFetched);
        
        HRESULT ( STDMETHODCALLTYPE *Skip )( 
            __RPC__in IEnumGroupEvaluationResults110 * This,
            /* [in] */ ULONG cResultsToSkip);
        
        HRESULT ( STDMETHODCALLTYPE *Reset )( 
            __RPC__in IEnumGroupEvaluationResults110 * This);
        
        HRESULT ( STDMETHODCALLTYPE *Clone )( 
            __RPC__in IEnumGroupEvaluationResults110 * This,
            /* [out] */ __RPC__deref_out_opt IEnumGroupEvaluationResults110 **ppEnum);
        
        HRESULT ( STDMETHODCALLTYPE *GetCount )( 
            __RPC__in IEnumGroupEvaluationResults110 * This,
            /* [out] */ __RPC__out ULONG *pcTotalResults);
        
        END_INTERFACE
    } IEnumGroupEvaluationResults110Vtbl;

    interface IEnumGroupEvaluationResults110
    {
        CONST_VTBL struct IEnumGroupEvaluationResults110Vtbl *lpVtbl;
    };

    

#ifdef COBJMACROS


#define IEnumGroupEvaluationResults110_QueryInterface(This,riid,ppvObject)	\
    ( (This)->lpVtbl -> QueryInterface(This,riid,ppvObject) ) 

#define IEnumGroupEvaluationResults110_AddRef(This)	\
    ( (This)->lpVtbl -> AddRef(This) ) 

#define IEnumGroupEvaluationResults110_Release(This)	\
    ( (This)->lpVtbl -> Release(This) ) 


#define IEnumGroupEvaluationResults110_Next(This,cResultBufferSize,ppResultContexts,pcResultsFetched)	\
    ( (This)->lpVtbl -> Next(This,cResultBufferSize,ppResultContexts,pcResultsFetched) ) 

#define IEnumGroupEvaluationResults110_Skip(This,cResultsToSkip)	\
    ( (This)->lpVtbl -> Skip(This,cResultsToSkip) ) 

#define IEnumGroupEvaluationResults110_Reset(This)	\
    ( (This)->lpVtbl -> Reset(This) ) 

#define IEnumGroupEvaluationResults110_Clone(This,ppEnum)	\
    ( (This)->lpVtbl -> Clone(This,ppEnum) ) 

#define IEnumGroupEvaluationResults110_GetCount(This,pcTotalResults)	\
    ( (This)->lpVtbl -> GetCount(This,pcTotalResults) ) 

#endif /* COBJMACROS */


#endif 	/* C style interface */




#endif 	/* __IEnumGroupEvaluationResults110_INTERFACE_DEFINED__ */


#ifndef __IDebugGroupEvaluationCompleteEvent110_INTERFACE_DEFINED__
#define __IDebugGroupEvaluationCompleteEvent110_INTERFACE_DEFINED__

/* interface IDebugGroupEvaluationCompleteEvent110 */
/* [unique][uuid][object] */ 


EXTERN_C const IID IID_IDebugGroupEvaluationCompleteEvent110;

#if defined(__cplusplus) && !defined(CINTERFACE)
    
    MIDL_INTERFACE("BA675ACF-E798-4B06-A843-C0ED2631E015")
    IDebugGroupEvaluationCompleteEvent110 : public IUnknown
    {
    public:
        virtual HRESULT STDMETHODCALLTYPE GetExpression( 
            /* [out] */ __RPC__deref_out_opt IDebugCompiledExpression110 **ppExpression) = 0;
        
        virtual HRESULT STDMETHODCALLTYPE GetEvaluationResult( void) = 0;
        
        virtual HRESULT STDMETHODCALLTYPE GetResults( 
            /* [out] */ __RPC__deref_out_opt IEnumGroupEvaluationResults110 **ppResultsEnum) = 0;
        
    };
    
    
#else 	/* C style interface */

    typedef struct IDebugGroupEvaluationCompleteEvent110Vtbl
    {
        BEGIN_INTERFACE
        
        HRESULT ( STDMETHODCALLTYPE *QueryInterface )( 
            __RPC__in IDebugGroupEvaluationCompleteEvent110 * This,
            /* [in] */ __RPC__in REFIID riid,
            /* [annotation][iid_is][out] */ 
            _COM_Outptr_  void **ppvObject);
        
        ULONG ( STDMETHODCALLTYPE *AddRef )( 
            __RPC__in IDebugGroupEvaluationCompleteEvent110 * This);
        
        ULONG ( STDMETHODCALLTYPE *Release )( 
            __RPC__in IDebugGroupEvaluationCompleteEvent110 * This);
        
        HRESULT ( STDMETHODCALLTYPE *GetExpression )( 
            __RPC__in IDebugGroupEvaluationCompleteEvent110 * This,
            /* [out] */ __RPC__deref_out_opt IDebugCompiledExpression110 **ppExpression);
        
        HRESULT ( STDMETHODCALLTYPE *GetEvaluationResult )( 
            __RPC__in IDebugGroupEvaluationCompleteEvent110 * This);
        
        HRESULT ( STDMETHODCALLTYPE *GetResults )( 
            __RPC__in IDebugGroupEvaluationCompleteEvent110 * This,
            /* [out] */ __RPC__deref_out_opt IEnumGroupEvaluationResults110 **ppResultsEnum);
        
        END_INTERFACE
    } IDebugGroupEvaluationCompleteEvent110Vtbl;

    interface IDebugGroupEvaluationCompleteEvent110
    {
        CONST_VTBL struct IDebugGroupEvaluationCompleteEvent110Vtbl *lpVtbl;
    };

    

#ifdef COBJMACROS


#define IDebugGroupEvaluationCompleteEvent110_QueryInterface(This,riid,ppvObject)	\
    ( (This)->lpVtbl -> QueryInterface(This,riid,ppvObject) ) 

#define IDebugGroupEvaluationCompleteEvent110_AddRef(This)	\
    ( (This)->lpVtbl -> AddRef(This) ) 

#define IDebugGroupEvaluationCompleteEvent110_Release(This)	\
    ( (This)->lpVtbl -> Release(This) ) 


#define IDebugGroupEvaluationCompleteEvent110_GetExpression(This,ppExpression)	\
    ( (This)->lpVtbl -> GetExpression(This,ppExpression) ) 

#define IDebugGroupEvaluationCompleteEvent110_GetEvaluationResult(This)	\
    ( (This)->lpVtbl -> GetEvaluationResult(This) ) 

#define IDebugGroupEvaluationCompleteEvent110_GetResults(This,ppResultsEnum)	\
    ( (This)->lpVtbl -> GetResults(This,ppResultsEnum) ) 

#endif /* COBJMACROS */


#endif 	/* C style interface */




#endif 	/* __IDebugGroupEvaluationCompleteEvent110_INTERFACE_DEFINED__ */


#ifndef __IDebugBreakpointRequest110_INTERFACE_DEFINED__
#define __IDebugBreakpointRequest110_INTERFACE_DEFINED__

/* interface IDebugBreakpointRequest110 */
/* [unique][uuid][object] */ 


EXTERN_C const IID IID_IDebugBreakpointRequest110;

#if defined(__cplusplus) && !defined(CINTERFACE)
    
    MIDL_INTERFACE("8AB88B04-2586-47CE-8A23-E3130F86296F")
    IDebugBreakpointRequest110 : public IUnknown
    {
    public:
        virtual HRESULT STDMETHODCALLTYPE GetAutomationObject( 
            /* [out] */ __RPC__deref_out_opt IUnknown **ppAutomationObject) = 0;
        
        virtual HRESULT STDMETHODCALLTYPE GetIsBarrier( 
            /* [out] */ __RPC__out BOOL *isBarrier) = 0;
        
    };
    
    
#else 	/* C style interface */

    typedef struct IDebugBreakpointRequest110Vtbl
    {
        BEGIN_INTERFACE
        
        HRESULT ( STDMETHODCALLTYPE *QueryInterface )( 
            __RPC__in IDebugBreakpointRequest110 * This,
            /* [in] */ __RPC__in REFIID riid,
            /* [annotation][iid_is][out] */ 
            _COM_Outptr_  void **ppvObject);
        
        ULONG ( STDMETHODCALLTYPE *AddRef )( 
            __RPC__in IDebugBreakpointRequest110 * This);
        
        ULONG ( STDMETHODCALLTYPE *Release )( 
            __RPC__in IDebugBreakpointRequest110 * This);
        
        HRESULT ( STDMETHODCALLTYPE *GetAutomationObject )( 
            __RPC__in IDebugBreakpointRequest110 * This,
            /* [out] */ __RPC__deref_out_opt IUnknown **ppAutomationObject);
        
        HRESULT ( STDMETHODCALLTYPE *GetIsBarrier )( 
            __RPC__in IDebugBreakpointRequest110 * This,
            /* [out] */ __RPC__out BOOL *isBarrier);
        
        END_INTERFACE
    } IDebugBreakpointRequest110Vtbl;

    interface IDebugBreakpointRequest110
    {
        CONST_VTBL struct IDebugBreakpointRequest110Vtbl *lpVtbl;
    };

    

#ifdef COBJMACROS


#define IDebugBreakpointRequest110_QueryInterface(This,riid,ppvObject)	\
    ( (This)->lpVtbl -> QueryInterface(This,riid,ppvObject) ) 

#define IDebugBreakpointRequest110_AddRef(This)	\
    ( (This)->lpVtbl -> AddRef(This) ) 

#define IDebugBreakpointRequest110_Release(This)	\
    ( (This)->lpVtbl -> Release(This) ) 


#define IDebugBreakpointRequest110_GetAutomationObject(This,ppAutomationObject)	\
    ( (This)->lpVtbl -> GetAutomationObject(This,ppAutomationObject) ) 

#define IDebugBreakpointRequest110_GetIsBarrier(This,isBarrier)	\
    ( (This)->lpVtbl -> GetIsBarrier(This,isBarrier) ) 

#endif /* COBJMACROS */


#endif 	/* C style interface */




#endif 	/* __IDebugBreakpointRequest110_INTERFACE_DEFINED__ */


#ifndef __IDebugBoundBreakpoint110_INTERFACE_DEFINED__
#define __IDebugBoundBreakpoint110_INTERFACE_DEFINED__

/* interface IDebugBoundBreakpoint110 */
/* [unique][uuid][object] */ 

typedef 
enum enum_TARGET_CODE_TYPE
    {
        TCT_UNKNOWN	= 0,
        TCT_CPU_CODE	= ( TCT_UNKNOWN + 1 ) ,
        TCT_GPU_CODE	= ( TCT_CPU_CODE + 1 ) 
    } 	TARGET_CODE_TYPE;


EXTERN_C const IID IID_IDebugBoundBreakpoint110;

#if defined(__cplusplus) && !defined(CINTERFACE)
    
    MIDL_INTERFACE("F7D19258-7AED-45C5-BB5A-1657B035360D")
    IDebugBoundBreakpoint110 : public IUnknown
    {
    public:
        virtual HRESULT STDMETHODCALLTYPE GetTargetCodeType( 
            /* [out] */ __RPC__out TARGET_CODE_TYPE *pTargetCodeType) = 0;
        
    };
    
    
#else 	/* C style interface */

    typedef struct IDebugBoundBreakpoint110Vtbl
    {
        BEGIN_INTERFACE
        
        HRESULT ( STDMETHODCALLTYPE *QueryInterface )( 
            __RPC__in IDebugBoundBreakpoint110 * This,
            /* [in] */ __RPC__in REFIID riid,
            /* [annotation][iid_is][out] */ 
            _COM_Outptr_  void **ppvObject);
        
        ULONG ( STDMETHODCALLTYPE *AddRef )( 
            __RPC__in IDebugBoundBreakpoint110 * This);
        
        ULONG ( STDMETHODCALLTYPE *Release )( 
            __RPC__in IDebugBoundBreakpoint110 * This);
        
        HRESULT ( STDMETHODCALLTYPE *GetTargetCodeType )( 
            __RPC__in IDebugBoundBreakpoint110 * This,
            /* [out] */ __RPC__out TARGET_CODE_TYPE *pTargetCodeType);
        
        END_INTERFACE
    } IDebugBoundBreakpoint110Vtbl;

    interface IDebugBoundBreakpoint110
    {
        CONST_VTBL struct IDebugBoundBreakpoint110Vtbl *lpVtbl;
    };

    

#ifdef COBJMACROS


#define IDebugBoundBreakpoint110_QueryInterface(This,riid,ppvObject)	\
    ( (This)->lpVtbl -> QueryInterface(This,riid,ppvObject) ) 

#define IDebugBoundBreakpoint110_AddRef(This)	\
    ( (This)->lpVtbl -> AddRef(This) ) 

#define IDebugBoundBreakpoint110_Release(This)	\
    ( (This)->lpVtbl -> Release(This) ) 


#define IDebugBoundBreakpoint110_GetTargetCodeType(This,pTargetCodeType)	\
    ( (This)->lpVtbl -> GetTargetCodeType(This,pTargetCodeType) ) 

#endif /* COBJMACROS */


#endif 	/* C style interface */




#endif 	/* __IDebugBoundBreakpoint110_INTERFACE_DEFINED__ */


#ifndef __IDebugDocumentPosition110_INTERFACE_DEFINED__
#define __IDebugDocumentPosition110_INTERFACE_DEFINED__

/* interface IDebugDocumentPosition110 */
/* [unique][uuid][object] */ 


EXTERN_C const IID IID_IDebugDocumentPosition110;

#if defined(__cplusplus) && !defined(CINTERFACE)
    
    MIDL_INTERFACE("d3376546-5c4b-46f4-9e38-2cc7c0c2eb41")
    IDebugDocumentPosition110 : public IUnknown
    {
    public:
        virtual HRESULT STDMETHODCALLTYPE GetChecksum( 
            /* [in] */ __RPC__in REFGUID guidAlgorithm,
            /* [out] */ __RPC__out CHECKSUM_DATA *pChecksumData) = 0;
        
        virtual HRESULT STDMETHODCALLTYPE IsChecksumEnabled( 
            /* [out] */ __RPC__out BOOL *pfChecksumEnabled) = 0;
        
        virtual HRESULT STDMETHODCALLTYPE GetLanguage( 
            /* [out] */ __RPC__out GUID *pguidLanguage) = 0;
        
        virtual HRESULT STDMETHODCALLTYPE GetText( 
            /* [out] */ __RPC__deref_out_opt BSTR *pbstrText) = 0;
        
    };
    
    
#else 	/* C style interface */

    typedef struct IDebugDocumentPosition110Vtbl
    {
        BEGIN_INTERFACE
        
        HRESULT ( STDMETHODCALLTYPE *QueryInterface )( 
            __RPC__in IDebugDocumentPosition110 * This,
            /* [in] */ __RPC__in REFIID riid,
            /* [annotation][iid_is][out] */ 
            _COM_Outptr_  void **ppvObject);
        
        ULONG ( STDMETHODCALLTYPE *AddRef )( 
            __RPC__in IDebugDocumentPosition110 * This);
        
        ULONG ( STDMETHODCALLTYPE *Release )( 
            __RPC__in IDebugDocumentPosition110 * This);
        
        HRESULT ( STDMETHODCALLTYPE *GetChecksum )( 
            __RPC__in IDebugDocumentPosition110 * This,
            /* [in] */ __RPC__in REFGUID guidAlgorithm,
            /* [out] */ __RPC__out CHECKSUM_DATA *pChecksumData);
        
        HRESULT ( STDMETHODCALLTYPE *IsChecksumEnabled )( 
            __RPC__in IDebugDocumentPosition110 * This,
            /* [out] */ __RPC__out BOOL *pfChecksumEnabled);
        
        HRESULT ( STDMETHODCALLTYPE *GetLanguage )( 
            __RPC__in IDebugDocumentPosition110 * This,
            /* [out] */ __RPC__out GUID *pguidLanguage);
        
        HRESULT ( STDMETHODCALLTYPE *GetText )( 
            __RPC__in IDebugDocumentPosition110 * This,
            /* [out] */ __RPC__deref_out_opt BSTR *pbstrText);
        
        END_INTERFACE
    } IDebugDocumentPosition110Vtbl;

    interface IDebugDocumentPosition110
    {
        CONST_VTBL struct IDebugDocumentPosition110Vtbl *lpVtbl;
    };

    

#ifdef COBJMACROS


#define IDebugDocumentPosition110_QueryInterface(This,riid,ppvObject)	\
    ( (This)->lpVtbl -> QueryInterface(This,riid,ppvObject) ) 

#define IDebugDocumentPosition110_AddRef(This)	\
    ( (This)->lpVtbl -> AddRef(This) ) 

#define IDebugDocumentPosition110_Release(This)	\
    ( (This)->lpVtbl -> Release(This) ) 


#define IDebugDocumentPosition110_GetChecksum(This,guidAlgorithm,pChecksumData)	\
    ( (This)->lpVtbl -> GetChecksum(This,guidAlgorithm,pChecksumData) ) 

#define IDebugDocumentPosition110_IsChecksumEnabled(This,pfChecksumEnabled)	\
    ( (This)->lpVtbl -> IsChecksumEnabled(This,pfChecksumEnabled) ) 

#define IDebugDocumentPosition110_GetLanguage(This,pguidLanguage)	\
    ( (This)->lpVtbl -> GetLanguage(This,pguidLanguage) ) 

#define IDebugDocumentPosition110_GetText(This,pbstrText)	\
    ( (This)->lpVtbl -> GetText(This,pbstrText) ) 

#endif /* COBJMACROS */


#endif 	/* C style interface */




#endif 	/* __IDebugDocumentPosition110_INTERFACE_DEFINED__ */


#ifndef __IDebugUserNotificationUI110_INTERFACE_DEFINED__
#define __IDebugUserNotificationUI110_INTERFACE_DEFINED__

/* interface IDebugUserNotificationUI110 */
/* [unique][uuid][object] */ 


EXTERN_C const IID IID_IDebugUserNotificationUI110;

#if defined(__cplusplus) && !defined(CINTERFACE)
    
    MIDL_INTERFACE("43fee995-53cd-41aa-9ec9-ca1281378788")
    IDebugUserNotificationUI110 : public IUnknown
    {
    public:
        virtual HRESULT STDMETHODCALLTYPE WaitOnSlowRemoteDebuggingOperation( 
            /* [in] */ WIN32_HANDLE waitHandle,
            /* [in] */ BOOL isCancelableOperation) = 0;
        
        virtual HRESULT STDMETHODCALLTYPE BeginWaitOnSlowSymbolLoad( 
            /* [in] */ __RPC__in LPCWSTR moduleName,
            /* [out] */ __RPC__out WIN32_HANDLE *pCancelWaitHandle) = 0;
        
        virtual HRESULT STDMETHODCALLTYPE EndWaitOnSlowSymbolLoad( void) = 0;
        
        virtual HRESULT STDMETHODCALLTYPE UpdateModuleNameForCurrentSymbolLoad( 
            /* [in] */ __RPC__in LPCWSTR newModuleName) = 0;
        
        virtual HRESULT STDMETHODCALLTYPE WaitOnSlowGroupEEOperation( 
            /* [in] */ WIN32_HANDLE waitHandle) = 0;
        
    };
    
    
#else 	/* C style interface */

    typedef struct IDebugUserNotificationUI110Vtbl
    {
        BEGIN_INTERFACE
        
        HRESULT ( STDMETHODCALLTYPE *QueryInterface )( 
            __RPC__in IDebugUserNotificationUI110 * This,
            /* [in] */ __RPC__in REFIID riid,
            /* [annotation][iid_is][out] */ 
            _COM_Outptr_  void **ppvObject);
        
        ULONG ( STDMETHODCALLTYPE *AddRef )( 
            __RPC__in IDebugUserNotificationUI110 * This);
        
        ULONG ( STDMETHODCALLTYPE *Release )( 
            __RPC__in IDebugUserNotificationUI110 * This);
        
        HRESULT ( STDMETHODCALLTYPE *WaitOnSlowRemoteDebuggingOperation )( 
            __RPC__in IDebugUserNotificationUI110 * This,
            /* [in] */ WIN32_HANDLE waitHandle,
            /* [in] */ BOOL isCancelableOperation);
        
        HRESULT ( STDMETHODCALLTYPE *BeginWaitOnSlowSymbolLoad )( 
            __RPC__in IDebugUserNotificationUI110 * This,
            /* [in] */ __RPC__in LPCWSTR moduleName,
            /* [out] */ __RPC__out WIN32_HANDLE *pCancelWaitHandle);
        
        HRESULT ( STDMETHODCALLTYPE *EndWaitOnSlowSymbolLoad )( 
            __RPC__in IDebugUserNotificationUI110 * This);
        
        HRESULT ( STDMETHODCALLTYPE *UpdateModuleNameForCurrentSymbolLoad )( 
            __RPC__in IDebugUserNotificationUI110 * This,
            /* [in] */ __RPC__in LPCWSTR newModuleName);
        
        HRESULT ( STDMETHODCALLTYPE *WaitOnSlowGroupEEOperation )( 
            __RPC__in IDebugUserNotificationUI110 * This,
            /* [in] */ WIN32_HANDLE waitHandle);
        
        END_INTERFACE
    } IDebugUserNotificationUI110Vtbl;

    interface IDebugUserNotificationUI110
    {
        CONST_VTBL struct IDebugUserNotificationUI110Vtbl *lpVtbl;
    };

    

#ifdef COBJMACROS


#define IDebugUserNotificationUI110_QueryInterface(This,riid,ppvObject)	\
    ( (This)->lpVtbl -> QueryInterface(This,riid,ppvObject) ) 

#define IDebugUserNotificationUI110_AddRef(This)	\
    ( (This)->lpVtbl -> AddRef(This) ) 

#define IDebugUserNotificationUI110_Release(This)	\
    ( (This)->lpVtbl -> Release(This) ) 


#define IDebugUserNotificationUI110_WaitOnSlowRemoteDebuggingOperation(This,waitHandle,isCancelableOperation)	\
    ( (This)->lpVtbl -> WaitOnSlowRemoteDebuggingOperation(This,waitHandle,isCancelableOperation) ) 

#define IDebugUserNotificationUI110_BeginWaitOnSlowSymbolLoad(This,moduleName,pCancelWaitHandle)	\
    ( (This)->lpVtbl -> BeginWaitOnSlowSymbolLoad(This,moduleName,pCancelWaitHandle) ) 

#define IDebugUserNotificationUI110_EndWaitOnSlowSymbolLoad(This)	\
    ( (This)->lpVtbl -> EndWaitOnSlowSymbolLoad(This) ) 

#define IDebugUserNotificationUI110_UpdateModuleNameForCurrentSymbolLoad(This,newModuleName)	\
    ( (This)->lpVtbl -> UpdateModuleNameForCurrentSymbolLoad(This,newModuleName) ) 

#define IDebugUserNotificationUI110_WaitOnSlowGroupEEOperation(This,waitHandle)	\
    ( (This)->lpVtbl -> WaitOnSlowGroupEEOperation(This,waitHandle) ) 

#endif /* COBJMACROS */


#endif 	/* C style interface */




#endif 	/* __IDebugUserNotificationUI110_INTERFACE_DEFINED__ */


#ifndef __IDebugDefaultPort110_INTERFACE_DEFINED__
#define __IDebugDefaultPort110_INTERFACE_DEFINED__

/* interface IDebugDefaultPort110 */
/* [unique][uuid][object] */ 


EXTERN_C const IID IID_IDebugDefaultPort110;

#if defined(__cplusplus) && !defined(CINTERFACE)
    
    MIDL_INTERFACE("690ffd4f-6336-4689-9bc6-c8f87c1d333c")
    IDebugDefaultPort110 : public IUnknown
    {
    public:
        virtual HRESULT STDMETHODCALLTYPE EnumProcesses( 
            /* [in] */ BOOL fIncludeFromAllUsers,
            /* [out] */ __RPC__deref_out_opt IEnumDebugProcesses2 **ppEnum) = 0;
        
    };
    
    
#else 	/* C style interface */

    typedef struct IDebugDefaultPort110Vtbl
    {
        BEGIN_INTERFACE
        
        HRESULT ( STDMETHODCALLTYPE *QueryInterface )( 
            __RPC__in IDebugDefaultPort110 * This,
            /* [in] */ __RPC__in REFIID riid,
            /* [annotation][iid_is][out] */ 
            _COM_Outptr_  void **ppvObject);
        
        ULONG ( STDMETHODCALLTYPE *AddRef )( 
            __RPC__in IDebugDefaultPort110 * This);
        
        ULONG ( STDMETHODCALLTYPE *Release )( 
            __RPC__in IDebugDefaultPort110 * This);
        
        HRESULT ( STDMETHODCALLTYPE *EnumProcesses )( 
            __RPC__in IDebugDefaultPort110 * This,
            /* [in] */ BOOL fIncludeFromAllUsers,
            /* [out] */ __RPC__deref_out_opt IEnumDebugProcesses2 **ppEnum);
        
        END_INTERFACE
    } IDebugDefaultPort110Vtbl;

    interface IDebugDefaultPort110
    {
        CONST_VTBL struct IDebugDefaultPort110Vtbl *lpVtbl;
    };

    

#ifdef COBJMACROS


#define IDebugDefaultPort110_QueryInterface(This,riid,ppvObject)	\
    ( (This)->lpVtbl -> QueryInterface(This,riid,ppvObject) ) 

#define IDebugDefaultPort110_AddRef(This)	\
    ( (This)->lpVtbl -> AddRef(This) ) 

#define IDebugDefaultPort110_Release(This)	\
    ( (This)->lpVtbl -> Release(This) ) 


#define IDebugDefaultPort110_EnumProcesses(This,fIncludeFromAllUsers,ppEnum)	\
    ( (This)->lpVtbl -> EnumProcesses(This,fIncludeFromAllUsers,ppEnum) ) 

#endif /* COBJMACROS */


#endif 	/* C style interface */




#endif 	/* __IDebugDefaultPort110_INTERFACE_DEFINED__ */


#ifndef __IDebugSettingsCallback110_INTERFACE_DEFINED__
#define __IDebugSettingsCallback110_INTERFACE_DEFINED__

/* interface IDebugSettingsCallback110 */
/* [unique][uuid][object] */ 


EXTERN_C const IID IID_IDebugSettingsCallback110;

#if defined(__cplusplus) && !defined(CINTERFACE)
    
    MIDL_INTERFACE("712b9487-3492-42a4-87f4-a676ed3707ce")
    IDebugSettingsCallback110 : public IUnknown
    {
    public:
        virtual HRESULT STDMETHODCALLTYPE GetDisplayRadix( 
            /* [out] */ __RPC__out UINT32 *pdwRadix) = 0;
        
        virtual HRESULT STDMETHODCALLTYPE GetUserDocumentPath( 
            /* [out] */ __RPC__deref_out_opt BSTR *pbstrUserDocumentPath) = 0;
        
        virtual HRESULT STDMETHODCALLTYPE ShouldHideNonPublicMembers( 
            /* [out] */ __RPC__out BOOL *pfHideNonPublicMembers) = 0;
        
        virtual HRESULT STDMETHODCALLTYPE ShouldSuppressImplicitToStringCalls( 
            /* [out] */ __RPC__out BOOL *pfSuppressImplicitToStringCalls) = 0;
        
    };
    
    
#else 	/* C style interface */

    typedef struct IDebugSettingsCallback110Vtbl
    {
        BEGIN_INTERFACE
        
        HRESULT ( STDMETHODCALLTYPE *QueryInterface )( 
            __RPC__in IDebugSettingsCallback110 * This,
            /* [in] */ __RPC__in REFIID riid,
            /* [annotation][iid_is][out] */ 
            _COM_Outptr_  void **ppvObject);
        
        ULONG ( STDMETHODCALLTYPE *AddRef )( 
            __RPC__in IDebugSettingsCallback110 * This);
        
        ULONG ( STDMETHODCALLTYPE *Release )( 
            __RPC__in IDebugSettingsCallback110 * This);
        
        HRESULT ( STDMETHODCALLTYPE *GetDisplayRadix )( 
            __RPC__in IDebugSettingsCallback110 * This,
            /* [out] */ __RPC__out UINT32 *pdwRadix);
        
        HRESULT ( STDMETHODCALLTYPE *GetUserDocumentPath )( 
            __RPC__in IDebugSettingsCallback110 * This,
            /* [out] */ __RPC__deref_out_opt BSTR *pbstrUserDocumentPath);
        
        HRESULT ( STDMETHODCALLTYPE *ShouldHideNonPublicMembers )( 
            __RPC__in IDebugSettingsCallback110 * This,
            /* [out] */ __RPC__out BOOL *pfHideNonPublicMembers);
        
        HRESULT ( STDMETHODCALLTYPE *ShouldSuppressImplicitToStringCalls )( 
            __RPC__in IDebugSettingsCallback110 * This,
            /* [out] */ __RPC__out BOOL *pfSuppressImplicitToStringCalls);
        
        END_INTERFACE
    } IDebugSettingsCallback110Vtbl;

    interface IDebugSettingsCallback110
    {
        CONST_VTBL struct IDebugSettingsCallback110Vtbl *lpVtbl;
    };

    

#ifdef COBJMACROS


#define IDebugSettingsCallback110_QueryInterface(This,riid,ppvObject)	\
    ( (This)->lpVtbl -> QueryInterface(This,riid,ppvObject) ) 

#define IDebugSettingsCallback110_AddRef(This)	\
    ( (This)->lpVtbl -> AddRef(This) ) 

#define IDebugSettingsCallback110_Release(This)	\
    ( (This)->lpVtbl -> Release(This) ) 


#define IDebugSettingsCallback110_GetDisplayRadix(This,pdwRadix)	\
    ( (This)->lpVtbl -> GetDisplayRadix(This,pdwRadix) ) 

#define IDebugSettingsCallback110_GetUserDocumentPath(This,pbstrUserDocumentPath)	\
    ( (This)->lpVtbl -> GetUserDocumentPath(This,pbstrUserDocumentPath) ) 

#define IDebugSettingsCallback110_ShouldHideNonPublicMembers(This,pfHideNonPublicMembers)	\
    ( (This)->lpVtbl -> ShouldHideNonPublicMembers(This,pfHideNonPublicMembers) ) 

#define IDebugSettingsCallback110_ShouldSuppressImplicitToStringCalls(This,pfSuppressImplicitToStringCalls)	\
    ( (This)->lpVtbl -> ShouldSuppressImplicitToStringCalls(This,pfSuppressImplicitToStringCalls) ) 

#endif /* COBJMACROS */


#endif 	/* C style interface */




#endif 	/* __IDebugSettingsCallback110_INTERFACE_DEFINED__ */


#ifndef __IDebugEngine110_INTERFACE_DEFINED__
#define __IDebugEngine110_INTERFACE_DEFINED__

/* interface IDebugEngine110 */
/* [unique][uuid][object] */ 


EXTERN_C const IID IID_IDebugEngine110;

#if defined(__cplusplus) && !defined(CINTERFACE)
    
    MIDL_INTERFACE("6f274ec0-3772-415c-969f-58bac0e2c5a9")
    IDebugEngine110 : public IUnknown
    {
    public:
        virtual HRESULT STDMETHODCALLTYPE SetMainThreadSettingsCallback110( 
            __RPC__in_opt IDebugSettingsCallback110 *pCallback) = 0;
        
    };
    
    
#else 	/* C style interface */

    typedef struct IDebugEngine110Vtbl
    {
        BEGIN_INTERFACE
        
        HRESULT ( STDMETHODCALLTYPE *QueryInterface )( 
            __RPC__in IDebugEngine110 * This,
            /* [in] */ __RPC__in REFIID riid,
            /* [annotation][iid_is][out] */ 
            _COM_Outptr_  void **ppvObject);
        
        ULONG ( STDMETHODCALLTYPE *AddRef )( 
            __RPC__in IDebugEngine110 * This);
        
        ULONG ( STDMETHODCALLTYPE *Release )( 
            __RPC__in IDebugEngine110 * This);
        
        HRESULT ( STDMETHODCALLTYPE *SetMainThreadSettingsCallback110 )( 
            __RPC__in IDebugEngine110 * This,
            __RPC__in_opt IDebugSettingsCallback110 *pCallback);
        
        END_INTERFACE
    } IDebugEngine110Vtbl;

    interface IDebugEngine110
    {
        CONST_VTBL struct IDebugEngine110Vtbl *lpVtbl;
    };

    

#ifdef COBJMACROS


#define IDebugEngine110_QueryInterface(This,riid,ppvObject)	\
    ( (This)->lpVtbl -> QueryInterface(This,riid,ppvObject) ) 

#define IDebugEngine110_AddRef(This)	\
    ( (This)->lpVtbl -> AddRef(This) ) 

#define IDebugEngine110_Release(This)	\
    ( (This)->lpVtbl -> Release(This) ) 


#define IDebugEngine110_SetMainThreadSettingsCallback110(This,pCallback)	\
    ( (This)->lpVtbl -> SetMainThreadSettingsCallback110(This,pCallback) ) 

#endif /* COBJMACROS */


#endif 	/* C style interface */




#endif 	/* __IDebugEngine110_INTERFACE_DEFINED__ */


/* interface __MIDL_itf_msdbg110_0000_0023 */
/* [local] */ 

struct VsComponentMessage
    {
    GUID SourceId;
    DWORD MessageCode;
    VARIANT Parameter1;
    VARIANT Parameter2;
    } ;


extern RPC_IF_HANDLE __MIDL_itf_msdbg110_0000_0023_v0_0_c_ifspec;
extern RPC_IF_HANDLE __MIDL_itf_msdbg110_0000_0023_v0_0_s_ifspec;

#ifndef __IVsCustomDebuggerEventHandler110_INTERFACE_DEFINED__
#define __IVsCustomDebuggerEventHandler110_INTERFACE_DEFINED__

/* interface IVsCustomDebuggerEventHandler110 */
/* [unique][uuid][object][local] */ 


EXTERN_C const IID IID_IVsCustomDebuggerEventHandler110;

#if defined(__cplusplus) && !defined(CINTERFACE)
    
    MIDL_INTERFACE("b704f6b1-1339-4c29-b402-ddd5a7f6f82a")
    IVsCustomDebuggerEventHandler110 : public IUnknown
    {
    public:
        virtual HRESULT STDMETHODCALLTYPE OnCustomDebugEvent( 
            /* [in] */ REFGUID ProcessId,
            /* [in] */ struct VsComponentMessage message) = 0;
        
    };
    
    
#else 	/* C style interface */

    typedef struct IVsCustomDebuggerEventHandler110Vtbl
    {
        BEGIN_INTERFACE
        
        HRESULT ( STDMETHODCALLTYPE *QueryInterface )( 
            IVsCustomDebuggerEventHandler110 * This,
            /* [in] */ REFIID riid,
            /* [annotation][iid_is][out] */ 
            _COM_Outptr_  void **ppvObject);
        
        ULONG ( STDMETHODCALLTYPE *AddRef )( 
            IVsCustomDebuggerEventHandler110 * This);
        
        ULONG ( STDMETHODCALLTYPE *Release )( 
            IVsCustomDebuggerEventHandler110 * This);
        
        HRESULT ( STDMETHODCALLTYPE *OnCustomDebugEvent )( 
            IVsCustomDebuggerEventHandler110 * This,
            /* [in] */ REFGUID ProcessId,
            /* [in] */ struct VsComponentMessage message);
        
        END_INTERFACE
    } IVsCustomDebuggerEventHandler110Vtbl;

    interface IVsCustomDebuggerEventHandler110
    {
        CONST_VTBL struct IVsCustomDebuggerEventHandler110Vtbl *lpVtbl;
    };

    

#ifdef COBJMACROS


#define IVsCustomDebuggerEventHandler110_QueryInterface(This,riid,ppvObject)	\
    ( (This)->lpVtbl -> QueryInterface(This,riid,ppvObject) ) 

#define IVsCustomDebuggerEventHandler110_AddRef(This)	\
    ( (This)->lpVtbl -> AddRef(This) ) 

#define IVsCustomDebuggerEventHandler110_Release(This)	\
    ( (This)->lpVtbl -> Release(This) ) 


#define IVsCustomDebuggerEventHandler110_OnCustomDebugEvent(This,ProcessId,message)	\
    ( (This)->lpVtbl -> OnCustomDebugEvent(This,ProcessId,message) ) 

#endif /* COBJMACROS */


#endif 	/* C style interface */




#endif 	/* __IVsCustomDebuggerEventHandler110_INTERFACE_DEFINED__ */


#ifndef __IVsCustomDebuggerStoppingEventHandler110_INTERFACE_DEFINED__
#define __IVsCustomDebuggerStoppingEventHandler110_INTERFACE_DEFINED__

/* interface IVsCustomDebuggerStoppingEventHandler110 */
/* [unique][uuid][object][local] */ 


EXTERN_C const IID IID_IVsCustomDebuggerStoppingEventHandler110;

#if defined(__cplusplus) && !defined(CINTERFACE)
    
    MIDL_INTERFACE("f5cd73fa-13b2-4166-87dc-562ea46e0075")
    IVsCustomDebuggerStoppingEventHandler110 : public IUnknown
    {
    public:
        virtual HRESULT STDMETHODCALLTYPE OnCustomStoppingDebugEvent( 
            /* [in] */ REFGUID ProcessId,
            /* [in] */ struct VsComponentMessage message,
            /* [out] */ BOOL *pContinueExecution) = 0;
        
    };
    
    
#else 	/* C style interface */

    typedef struct IVsCustomDebuggerStoppingEventHandler110Vtbl
    {
        BEGIN_INTERFACE
        
        HRESULT ( STDMETHODCALLTYPE *QueryInterface )( 
            IVsCustomDebuggerStoppingEventHandler110 * This,
            /* [in] */ REFIID riid,
            /* [annotation][iid_is][out] */ 
            _COM_Outptr_  void **ppvObject);
        
        ULONG ( STDMETHODCALLTYPE *AddRef )( 
            IVsCustomDebuggerStoppingEventHandler110 * This);
        
        ULONG ( STDMETHODCALLTYPE *Release )( 
            IVsCustomDebuggerStoppingEventHandler110 * This);
        
        HRESULT ( STDMETHODCALLTYPE *OnCustomStoppingDebugEvent )( 
            IVsCustomDebuggerStoppingEventHandler110 * This,
            /* [in] */ REFGUID ProcessId,
            /* [in] */ struct VsComponentMessage message,
            /* [out] */ BOOL *pContinueExecution);
        
        END_INTERFACE
    } IVsCustomDebuggerStoppingEventHandler110Vtbl;

    interface IVsCustomDebuggerStoppingEventHandler110
    {
        CONST_VTBL struct IVsCustomDebuggerStoppingEventHandler110Vtbl *lpVtbl;
    };

    

#ifdef COBJMACROS


#define IVsCustomDebuggerStoppingEventHandler110_QueryInterface(This,riid,ppvObject)	\
    ( (This)->lpVtbl -> QueryInterface(This,riid,ppvObject) ) 

#define IVsCustomDebuggerStoppingEventHandler110_AddRef(This)	\
    ( (This)->lpVtbl -> AddRef(This) ) 

#define IVsCustomDebuggerStoppingEventHandler110_Release(This)	\
    ( (This)->lpVtbl -> Release(This) ) 


#define IVsCustomDebuggerStoppingEventHandler110_OnCustomStoppingDebugEvent(This,ProcessId,message,pContinueExecution)	\
    ( (This)->lpVtbl -> OnCustomStoppingDebugEvent(This,ProcessId,message,pContinueExecution) ) 

#endif /* COBJMACROS */


#endif 	/* C style interface */




#endif 	/* __IVsCustomDebuggerStoppingEventHandler110_INTERFACE_DEFINED__ */


#ifndef __IDebugCustomEvent110_INTERFACE_DEFINED__
#define __IDebugCustomEvent110_INTERFACE_DEFINED__

/* interface IDebugCustomEvent110 */
/* [unique][uuid][object] */ 


EXTERN_C const IID IID_IDebugCustomEvent110;

#if defined(__cplusplus) && !defined(CINTERFACE)
    
    MIDL_INTERFACE("2615d9bc-1948-4d21-81ee-7a963f20cf59")
    IDebugCustomEvent110 : public IUnknown
    {
    public:
        virtual HRESULT STDMETHODCALLTYPE GetCustomEventInfo( 
            /* [out] */ __RPC__out GUID *guidVSService,
            /* [out] */ __RPC__out struct VsComponentMessage *message) = 0;
        
    };
    
    
#else 	/* C style interface */

    typedef struct IDebugCustomEvent110Vtbl
    {
        BEGIN_INTERFACE
        
        HRESULT ( STDMETHODCALLTYPE *QueryInterface )( 
            __RPC__in IDebugCustomEvent110 * This,
            /* [in] */ __RPC__in REFIID riid,
            /* [annotation][iid_is][out] */ 
            _COM_Outptr_  void **ppvObject);
        
        ULONG ( STDMETHODCALLTYPE *AddRef )( 
            __RPC__in IDebugCustomEvent110 * This);
        
        ULONG ( STDMETHODCALLTYPE *Release )( 
            __RPC__in IDebugCustomEvent110 * This);
        
        HRESULT ( STDMETHODCALLTYPE *GetCustomEventInfo )( 
            __RPC__in IDebugCustomEvent110 * This,
            /* [out] */ __RPC__out GUID *guidVSService,
            /* [out] */ __RPC__out struct VsComponentMessage *message);
        
        END_INTERFACE
    } IDebugCustomEvent110Vtbl;

    interface IDebugCustomEvent110
    {
        CONST_VTBL struct IDebugCustomEvent110Vtbl *lpVtbl;
    };

    

#ifdef COBJMACROS


#define IDebugCustomEvent110_QueryInterface(This,riid,ppvObject)	\
    ( (This)->lpVtbl -> QueryInterface(This,riid,ppvObject) ) 

#define IDebugCustomEvent110_AddRef(This)	\
    ( (This)->lpVtbl -> AddRef(This) ) 

#define IDebugCustomEvent110_Release(This)	\
    ( (This)->lpVtbl -> Release(This) ) 


#define IDebugCustomEvent110_GetCustomEventInfo(This,guidVSService,message)	\
    ( (This)->lpVtbl -> GetCustomEventInfo(This,guidVSService,message) ) 

#endif /* COBJMACROS */


#endif 	/* C style interface */




#endif 	/* __IDebugCustomEvent110_INTERFACE_DEFINED__ */


/* interface __MIDL_itf_msdbg110_0000_0026 */
/* [local] */ 


enum VsDebugLaunchNotifyListenerFlags
    {
        VsDebugLaunchNotify_Default	= 0,
        VsDebugLaunchNotify_RegisterForAppPackage	= 0x1,
        VsDebugLaunchNotify_DetachOnStopDebugging	= 0x2,
        VsDebugLaunchNotify_DebugWithZeroProcesses	= 0x4
    } ;
DEFINE_ENUM_FLAG_OPERATORS(VsDebugLaunchNotifyListenerFlags)
struct VsDebugLaunchNotifyListenerProperties
    {
    /* [size_is] */ GUID *EngineFilterArray;
    DWORD EngineFilterCount;
    DWORD ExpectedSessionId;
    LPCWSTR PackagedAppFullName;
    DWORD PackagedAppPlatform;
    BSTR AdditionalEnvironmentVariables;
    enum VsDebugLaunchNotifyListenerFlags Flags;
    BSTR Options;
    IUnknown *Project;
    IUnknown *LaunchSettingsContainer;
    } ;


extern RPC_IF_HANDLE __MIDL_itf_msdbg110_0000_0026_v0_0_c_ifspec;
extern RPC_IF_HANDLE __MIDL_itf_msdbg110_0000_0026_v0_0_s_ifspec;

#ifndef __IVsDebugLaunchNotifyListenerFactory110_INTERFACE_DEFINED__
#define __IVsDebugLaunchNotifyListenerFactory110_INTERFACE_DEFINED__

/* interface IVsDebugLaunchNotifyListenerFactory110 */
/* [unique][uuid][object] */ 


EXTERN_C const IID IID_IVsDebugLaunchNotifyListenerFactory110;

#if defined(__cplusplus) && !defined(CINTERFACE)
    
    MIDL_INTERFACE("a8ee3398-a74d-429c-8d1c-e3ad96637649")
    IVsDebugLaunchNotifyListenerFactory110 : public IUnknown
    {
    public:
        virtual HRESULT STDMETHODCALLTYPE StartListener( 
            /* [in] */ __RPC__in LPCWSTR transportQualifier,
            /* [in] */ __RPC__in REFGUID guidPortSupplier,
            /* [in] */ __RPC__in struct VsDebugLaunchNotifyListenerProperties *pProperties,
            /* [out] */ __RPC__deref_out_opt IVsDebugLaunchNotifyListener110 **ppListener) = 0;
        
    };
    
    
#else 	/* C style interface */

    typedef struct IVsDebugLaunchNotifyListenerFactory110Vtbl
    {
        BEGIN_INTERFACE
        
        HRESULT ( STDMETHODCALLTYPE *QueryInterface )( 
            __RPC__in IVsDebugLaunchNotifyListenerFactory110 * This,
            /* [in] */ __RPC__in REFIID riid,
            /* [annotation][iid_is][out] */ 
            _COM_Outptr_  void **ppvObject);
        
        ULONG ( STDMETHODCALLTYPE *AddRef )( 
            __RPC__in IVsDebugLaunchNotifyListenerFactory110 * This);
        
        ULONG ( STDMETHODCALLTYPE *Release )( 
            __RPC__in IVsDebugLaunchNotifyListenerFactory110 * This);
        
        HRESULT ( STDMETHODCALLTYPE *StartListener )( 
            __RPC__in IVsDebugLaunchNotifyListenerFactory110 * This,
            /* [in] */ __RPC__in LPCWSTR transportQualifier,
            /* [in] */ __RPC__in REFGUID guidPortSupplier,
            /* [in] */ __RPC__in struct VsDebugLaunchNotifyListenerProperties *pProperties,
            /* [out] */ __RPC__deref_out_opt IVsDebugLaunchNotifyListener110 **ppListener);
        
        END_INTERFACE
    } IVsDebugLaunchNotifyListenerFactory110Vtbl;

    interface IVsDebugLaunchNotifyListenerFactory110
    {
        CONST_VTBL struct IVsDebugLaunchNotifyListenerFactory110Vtbl *lpVtbl;
    };

    

#ifdef COBJMACROS


#define IVsDebugLaunchNotifyListenerFactory110_QueryInterface(This,riid,ppvObject)	\
    ( (This)->lpVtbl -> QueryInterface(This,riid,ppvObject) ) 

#define IVsDebugLaunchNotifyListenerFactory110_AddRef(This)	\
    ( (This)->lpVtbl -> AddRef(This) ) 

#define IVsDebugLaunchNotifyListenerFactory110_Release(This)	\
    ( (This)->lpVtbl -> Release(This) ) 


#define IVsDebugLaunchNotifyListenerFactory110_StartListener(This,transportQualifier,guidPortSupplier,pProperties,ppListener)	\
    ( (This)->lpVtbl -> StartListener(This,transportQualifier,guidPortSupplier,pProperties,ppListener) ) 

#endif /* COBJMACROS */


#endif 	/* C style interface */




#endif 	/* __IVsDebugLaunchNotifyListenerFactory110_INTERFACE_DEFINED__ */


#ifndef __IVsDebugLaunchNotifyListener110_INTERFACE_DEFINED__
#define __IVsDebugLaunchNotifyListener110_INTERFACE_DEFINED__

/* interface IVsDebugLaunchNotifyListener110 */
/* [unique][uuid][object] */ 


EXTERN_C const IID IID_IVsDebugLaunchNotifyListener110;

#if defined(__cplusplus) && !defined(CINTERFACE)
    
    MIDL_INTERFACE("4a991d8c-4711-448f-90a9-0d962667799b")
    IVsDebugLaunchNotifyListener110 : public IUnknown
    {
    public:
        virtual HRESULT STDMETHODCALLTYPE Close( void) = 0;
        
        virtual HRESULT STDMETHODCALLTYPE GetTargetStartInfo( 
            /* [out] */ __RPC__deref_out_opt BSTR *pNotifyCommandLine,
            /* [out] */ __RPC__deref_out_opt BSTR *pTargetProcessEnvironment) = 0;
        
        virtual HRESULT STDMETHODCALLTYPE GetServer( 
            /* [out] */ __RPC__deref_out_opt IDebugCoreServer2 **ppServer) = 0;
        
    };
    
    
#else 	/* C style interface */

    typedef struct IVsDebugLaunchNotifyListener110Vtbl
    {
        BEGIN_INTERFACE
        
        HRESULT ( STDMETHODCALLTYPE *QueryInterface )( 
            __RPC__in IVsDebugLaunchNotifyListener110 * This,
            /* [in] */ __RPC__in REFIID riid,
            /* [annotation][iid_is][out] */ 
            _COM_Outptr_  void **ppvObject);
        
        ULONG ( STDMETHODCALLTYPE *AddRef )( 
            __RPC__in IVsDebugLaunchNotifyListener110 * This);
        
        ULONG ( STDMETHODCALLTYPE *Release )( 
            __RPC__in IVsDebugLaunchNotifyListener110 * This);
        
        HRESULT ( STDMETHODCALLTYPE *Close )( 
            __RPC__in IVsDebugLaunchNotifyListener110 * This);
        
        HRESULT ( STDMETHODCALLTYPE *GetTargetStartInfo )( 
            __RPC__in IVsDebugLaunchNotifyListener110 * This,
            /* [out] */ __RPC__deref_out_opt BSTR *pNotifyCommandLine,
            /* [out] */ __RPC__deref_out_opt BSTR *pTargetProcessEnvironment);
        
        HRESULT ( STDMETHODCALLTYPE *GetServer )( 
            __RPC__in IVsDebugLaunchNotifyListener110 * This,
            /* [out] */ __RPC__deref_out_opt IDebugCoreServer2 **ppServer);
        
        END_INTERFACE
    } IVsDebugLaunchNotifyListener110Vtbl;

    interface IVsDebugLaunchNotifyListener110
    {
        CONST_VTBL struct IVsDebugLaunchNotifyListener110Vtbl *lpVtbl;
    };

    

#ifdef COBJMACROS


#define IVsDebugLaunchNotifyListener110_QueryInterface(This,riid,ppvObject)	\
    ( (This)->lpVtbl -> QueryInterface(This,riid,ppvObject) ) 

#define IVsDebugLaunchNotifyListener110_AddRef(This)	\
    ( (This)->lpVtbl -> AddRef(This) ) 

#define IVsDebugLaunchNotifyListener110_Release(This)	\
    ( (This)->lpVtbl -> Release(This) ) 


#define IVsDebugLaunchNotifyListener110_Close(This)	\
    ( (This)->lpVtbl -> Close(This) ) 

#define IVsDebugLaunchNotifyListener110_GetTargetStartInfo(This,pNotifyCommandLine,pTargetProcessEnvironment)	\
    ( (This)->lpVtbl -> GetTargetStartInfo(This,pNotifyCommandLine,pTargetProcessEnvironment) ) 

#define IVsDebugLaunchNotifyListener110_GetServer(This,ppServer)	\
    ( (This)->lpVtbl -> GetServer(This,ppServer) ) 

#endif /* COBJMACROS */


#endif 	/* C style interface */




#endif 	/* __IVsDebugLaunchNotifyListener110_INTERFACE_DEFINED__ */


#ifndef __IDebugLaunchedProcessAttachRequestEvent110_INTERFACE_DEFINED__
#define __IDebugLaunchedProcessAttachRequestEvent110_INTERFACE_DEFINED__

/* interface IDebugLaunchedProcessAttachRequestEvent110 */
/* [unique][uuid][object] */ 

struct ProcessAttachRequestEventInfo
    {
    IDebugPort2 *pPort;
    DWORD ProcessId;
    /* [size_is] */ GUID *EngineFilterArray;
    DWORD EngineFilterCount;
    BSTR ProcessName;
    enum VsDebugLaunchNotifyListenerFlags Flags;
    } ;

EXTERN_C const IID IID_IDebugLaunchedProcessAttachRequestEvent110;

#if defined(__cplusplus) && !defined(CINTERFACE)
    
    MIDL_INTERFACE("7a529a0a-d8ed-4abd-8218-f216b081ef3e")
    IDebugLaunchedProcessAttachRequestEvent110 : public IUnknown
    {
    public:
        virtual HRESULT STDMETHODCALLTYPE GetInfo( 
            /* [out] */ __RPC__out struct ProcessAttachRequestEventInfo *pInfo) = 0;
        
        virtual HRESULT STDMETHODCALLTYPE OnAttachComplete( 
            /* [in] */ BOOL fAttachSucceeded) = 0;
        
    };
    
    
#else 	/* C style interface */

    typedef struct IDebugLaunchedProcessAttachRequestEvent110Vtbl
    {
        BEGIN_INTERFACE
        
        HRESULT ( STDMETHODCALLTYPE *QueryInterface )( 
            __RPC__in IDebugLaunchedProcessAttachRequestEvent110 * This,
            /* [in] */ __RPC__in REFIID riid,
            /* [annotation][iid_is][out] */ 
            _COM_Outptr_  void **ppvObject);
        
        ULONG ( STDMETHODCALLTYPE *AddRef )( 
            __RPC__in IDebugLaunchedProcessAttachRequestEvent110 * This);
        
        ULONG ( STDMETHODCALLTYPE *Release )( 
            __RPC__in IDebugLaunchedProcessAttachRequestEvent110 * This);
        
        HRESULT ( STDMETHODCALLTYPE *GetInfo )( 
            __RPC__in IDebugLaunchedProcessAttachRequestEvent110 * This,
            /* [out] */ __RPC__out struct ProcessAttachRequestEventInfo *pInfo);
        
        HRESULT ( STDMETHODCALLTYPE *OnAttachComplete )( 
            __RPC__in IDebugLaunchedProcessAttachRequestEvent110 * This,
            /* [in] */ BOOL fAttachSucceeded);
        
        END_INTERFACE
    } IDebugLaunchedProcessAttachRequestEvent110Vtbl;

    interface IDebugLaunchedProcessAttachRequestEvent110
    {
        CONST_VTBL struct IDebugLaunchedProcessAttachRequestEvent110Vtbl *lpVtbl;
    };

    

#ifdef COBJMACROS


#define IDebugLaunchedProcessAttachRequestEvent110_QueryInterface(This,riid,ppvObject)	\
    ( (This)->lpVtbl -> QueryInterface(This,riid,ppvObject) ) 

#define IDebugLaunchedProcessAttachRequestEvent110_AddRef(This)	\
    ( (This)->lpVtbl -> AddRef(This) ) 

#define IDebugLaunchedProcessAttachRequestEvent110_Release(This)	\
    ( (This)->lpVtbl -> Release(This) ) 


#define IDebugLaunchedProcessAttachRequestEvent110_GetInfo(This,pInfo)	\
    ( (This)->lpVtbl -> GetInfo(This,pInfo) ) 

#define IDebugLaunchedProcessAttachRequestEvent110_OnAttachComplete(This,fAttachSucceeded)	\
    ( (This)->lpVtbl -> OnAttachComplete(This,fAttachSucceeded) ) 

#endif /* COBJMACROS */


#endif 	/* C style interface */




#endif 	/* __IDebugLaunchedProcessAttachRequestEvent110_INTERFACE_DEFINED__ */


#ifndef __IAppDomainInfo110_INTERFACE_DEFINED__
#define __IAppDomainInfo110_INTERFACE_DEFINED__

/* interface IAppDomainInfo110 */
/* [unique][uuid][object] */ 


EXTERN_C const IID IID_IAppDomainInfo110;

#if defined(__cplusplus) && !defined(CINTERFACE)
    
    MIDL_INTERFACE("585dbb5b-a542-4296-bec6-29d12a7b016c")
    IAppDomainInfo110 : public IUnknown
    {
    public:
        virtual HRESULT STDMETHODCALLTYPE GetAppDomainInfo( 
            /* [out] */ __RPC__out UINT32 *pAppDomainId,
            /* [out] */ __RPC__deref_out_opt BSTR *pbstrAppDomainName) = 0;
        
    };
    
    
#else 	/* C style interface */

    typedef struct IAppDomainInfo110Vtbl
    {
        BEGIN_INTERFACE
        
        HRESULT ( STDMETHODCALLTYPE *QueryInterface )( 
            __RPC__in IAppDomainInfo110 * This,
            /* [in] */ __RPC__in REFIID riid,
            /* [annotation][iid_is][out] */ 
            _COM_Outptr_  void **ppvObject);
        
        ULONG ( STDMETHODCALLTYPE *AddRef )( 
            __RPC__in IAppDomainInfo110 * This);
        
        ULONG ( STDMETHODCALLTYPE *Release )( 
            __RPC__in IAppDomainInfo110 * This);
        
        HRESULT ( STDMETHODCALLTYPE *GetAppDomainInfo )( 
            __RPC__in IAppDomainInfo110 * This,
            /* [out] */ __RPC__out UINT32 *pAppDomainId,
            /* [out] */ __RPC__deref_out_opt BSTR *pbstrAppDomainName);
        
        END_INTERFACE
    } IAppDomainInfo110Vtbl;

    interface IAppDomainInfo110
    {
        CONST_VTBL struct IAppDomainInfo110Vtbl *lpVtbl;
    };

    

#ifdef COBJMACROS


#define IAppDomainInfo110_QueryInterface(This,riid,ppvObject)	\
    ( (This)->lpVtbl -> QueryInterface(This,riid,ppvObject) ) 

#define IAppDomainInfo110_AddRef(This)	\
    ( (This)->lpVtbl -> AddRef(This) ) 

#define IAppDomainInfo110_Release(This)	\
    ( (This)->lpVtbl -> Release(This) ) 


#define IAppDomainInfo110_GetAppDomainInfo(This,pAppDomainId,pbstrAppDomainName)	\
    ( (This)->lpVtbl -> GetAppDomainInfo(This,pAppDomainId,pbstrAppDomainName) ) 

#endif /* COBJMACROS */


#endif 	/* C style interface */




#endif 	/* __IAppDomainInfo110_INTERFACE_DEFINED__ */


#ifndef __IVsCppDebugUIVisualizer_INTERFACE_DEFINED__
#define __IVsCppDebugUIVisualizer_INTERFACE_DEFINED__

/* interface IVsCppDebugUIVisualizer */
/* [local][unique][uuid][object] */ 


EXTERN_C const IID IID_IVsCppDebugUIVisualizer;

#if defined(__cplusplus) && !defined(CINTERFACE)
    
    MIDL_INTERFACE("E8EB78C4-0569-4B6A-BC28-4D5E1B0A350A")
    IVsCppDebugUIVisualizer : public IUnknown
    {
    public:
        virtual HRESULT STDMETHODCALLTYPE DisplayValue( 
            /* [in] */ DWORD_PTR ownerHwnd,
            /* [in] */ DWORD visualizerId,
            /* [in] */ IDebugProperty3 *pDebugProperty) = 0;
        
    };
    
    
#else 	/* C style interface */

    typedef struct IVsCppDebugUIVisualizerVtbl
    {
        BEGIN_INTERFACE
        
        HRESULT ( STDMETHODCALLTYPE *QueryInterface )( 
            IVsCppDebugUIVisualizer * This,
            /* [in] */ REFIID riid,
            /* [annotation][iid_is][out] */ 
            _COM_Outptr_  void **ppvObject);
        
        ULONG ( STDMETHODCALLTYPE *AddRef )( 
            IVsCppDebugUIVisualizer * This);
        
        ULONG ( STDMETHODCALLTYPE *Release )( 
            IVsCppDebugUIVisualizer * This);
        
        HRESULT ( STDMETHODCALLTYPE *DisplayValue )( 
            IVsCppDebugUIVisualizer * This,
            /* [in] */ DWORD_PTR ownerHwnd,
            /* [in] */ DWORD visualizerId,
            /* [in] */ IDebugProperty3 *pDebugProperty);
        
        END_INTERFACE
    } IVsCppDebugUIVisualizerVtbl;

    interface IVsCppDebugUIVisualizer
    {
        CONST_VTBL struct IVsCppDebugUIVisualizerVtbl *lpVtbl;
    };

    

#ifdef COBJMACROS


#define IVsCppDebugUIVisualizer_QueryInterface(This,riid,ppvObject)	\
    ( (This)->lpVtbl -> QueryInterface(This,riid,ppvObject) ) 

#define IVsCppDebugUIVisualizer_AddRef(This)	\
    ( (This)->lpVtbl -> AddRef(This) ) 

#define IVsCppDebugUIVisualizer_Release(This)	\
    ( (This)->lpVtbl -> Release(This) ) 


#define IVsCppDebugUIVisualizer_DisplayValue(This,ownerHwnd,visualizerId,pDebugProperty)	\
    ( (This)->lpVtbl -> DisplayValue(This,ownerHwnd,visualizerId,pDebugProperty) ) 

#endif /* COBJMACROS */


#endif 	/* C style interface */




#endif 	/* __IVsCppDebugUIVisualizer_INTERFACE_DEFINED__ */


/* interface __MIDL_itf_msdbg110_0000_0031 */
/* [local] */ 


enum enum_EVALFLAGS110
    {
        EVAL110_RETURNVALUE	= 0x2,
        EVAL110_NOSIDEEFFECTS	= 0x4,
        EVAL110_ALLOWBPS	= 0x8,
        EVAL110_ALLOWERRORREPORT	= 0x10,
        EVAL110_FUNCTION_AS_ADDRESS	= 0x40,
        EVAL110_NOFUNCEVAL	= 0x80,
        EVAL110_NOEVENTS	= 0x1000,
        EVAL110_DESIGN_TIME_EXPR_EVAL	= 0x2000,
        EVAL110_ALLOW_IMPLICIT_VARS	= 0x4000,
        EVAL110_FORCE_EVALUATION_NOW	= 0x8000,
        EVAL110_NO_IL_INTERPRETER_DEPRECATED	= 0x10000,
        EVAL110_ALLOW_FUNC_EVALS_EVEN_IF_NO_SIDE_EFFECTS	= 0x20000,
        EVAL110_ALLOW_THREADSLIPPING	= 0x40000,
        EVAL110_SHOW_VALUERAW	= 0x80000,
        EVAL110_FORCE_REAL_FUNCEVAL	= 0x100000,
        EVAL110_ILINTERPRETER_BEING_USED	= 0x200000
    } ;

enum enum_DEBUGPROP_INFO_FLAGS110
    {
        DEBUGPROP110_INFO_FULLNAME	= 0x1,
        DEBUGPROP110_INFO_NAME	= 0x2,
        DEBUGPROP110_INFO_TYPE	= 0x4,
        DEBUGPROP110_INFO_VALUE	= 0x8,
        DEBUGPROP110_INFO_ATTRIB	= 0x10,
        DEBUGPROP110_INFO_PROP	= 0x20,
        DEBUGPROP110_INFO_VALUE_AUTOEXPAND	= 0x10000,
        DEBUGPROP110_INFO_NOFUNCEVAL	= 0x20000,
        DEBUGPROP110_INFO_VALUE_RAW	= 0x40000,
        DEBUGPROP110_INFO_VALUE_NO_TOSTRING	= 0x80000,
        DEBUGPROP110_INFO_NO_NONPUBLIC_MEMBERS	= 0x100000,
        DEBUGPROP110_INFO_NONE	= 0,
        DEBUGPROP110_INFO_STANDARD	= ( ( ( DEBUGPROP110_INFO_ATTRIB | DEBUGPROP110_INFO_NAME )  | DEBUGPROP110_INFO_TYPE )  | DEBUGPROP110_INFO_VALUE ) ,
        DEBUGPROP110_INFO_ALL	= 0xffffffff,
        DEBUGPROP110_INFO_NOSIDEEFFECTS	= 0x200000,
        DEBUGPROP110_INFO_NO_IL_INTERPRETER	= 0x400000,
        DEBUGPROP110_INFO_ALLOW_FUNC_EVALS_EVEN_IF_NO_SIDE_EFFECTS	= 0x800000,
        DEBUGPROP110_INFO_ALLOW_THREADSLIPPING	= 0x1000000,
        DEBUGPROP110_INFO_FORCE_REAL_FUNCEVAL	= 0x2000000,
        DEBUGPROP110_INFO_ILINTERPRETER_BEING_USED	= 0x4000000
    } ;
typedef DWORD DEBUGPROP110_INFO_FLAGS;

#define DBG_ATTRIB_VALUE_UNFLUSHED_SIDEEFFECTS 0x0010000000000000
#define DBG_ATTRIB_MODULE                      0x0020000000000000


extern RPC_IF_HANDLE __MIDL_itf_msdbg110_0000_0031_v0_0_c_ifspec;
extern RPC_IF_HANDLE __MIDL_itf_msdbg110_0000_0031_v0_0_s_ifspec;


#ifndef __msdbg110_LIBRARY_DEFINED__
#define __msdbg110_LIBRARY_DEFINED__

/* library msdbg110 */
/* [uuid] */ 




















EXTERN_C const IID LIBID_msdbg110;

EXTERN_C const CLSID CLSID_DebugUserNotificationUI110;

#ifdef __cplusplus

class DECLSPEC_UUID("7199ed2c-0b58-4d00-bc75-bec5a3b46d5d")
DebugUserNotificationUI110;
#endif
#endif /* __msdbg110_LIBRARY_DEFINED__ */

#ifndef __IDebugRestrictedExceptionInfo110_INTERFACE_DEFINED__
#define __IDebugRestrictedExceptionInfo110_INTERFACE_DEFINED__

/* interface IDebugRestrictedExceptionInfo110 */
/* [unique][uuid][object] */ 


EXTERN_C const IID IID_IDebugRestrictedExceptionInfo110;

#if defined(__cplusplus) && !defined(CINTERFACE)
    
    MIDL_INTERFACE("24EF7776-1EB8-42f6-8146-EC673FBCE5B4")
    IDebugRestrictedExceptionInfo110 : public IUnknown
    {
    public:
        virtual HRESULT STDMETHODCALLTYPE GetInfo( 
            /* [out] */ __RPC__deref_out_opt BSTR *pbstrCapabilitySid,
            /* [out] */ __RPC__deref_out_opt BSTR *pbstrRestrictedReference) = 0;
        
    };
    
    
#else 	/* C style interface */

    typedef struct IDebugRestrictedExceptionInfo110Vtbl
    {
        BEGIN_INTERFACE
        
        HRESULT ( STDMETHODCALLTYPE *QueryInterface )( 
            __RPC__in IDebugRestrictedExceptionInfo110 * This,
            /* [in] */ __RPC__in REFIID riid,
            /* [annotation][iid_is][out] */ 
            _COM_Outptr_  void **ppvObject);
        
        ULONG ( STDMETHODCALLTYPE *AddRef )( 
            __RPC__in IDebugRestrictedExceptionInfo110 * This);
        
        ULONG ( STDMETHODCALLTYPE *Release )( 
            __RPC__in IDebugRestrictedExceptionInfo110 * This);
        
        HRESULT ( STDMETHODCALLTYPE *GetInfo )( 
            __RPC__in IDebugRestrictedExceptionInfo110 * This,
            /* [out] */ __RPC__deref_out_opt BSTR *pbstrCapabilitySid,
            /* [out] */ __RPC__deref_out_opt BSTR *pbstrRestrictedReference);
        
        END_INTERFACE
    } IDebugRestrictedExceptionInfo110Vtbl;

    interface IDebugRestrictedExceptionInfo110
    {
        CONST_VTBL struct IDebugRestrictedExceptionInfo110Vtbl *lpVtbl;
    };

    

#ifdef COBJMACROS


#define IDebugRestrictedExceptionInfo110_QueryInterface(This,riid,ppvObject)	\
    ( (This)->lpVtbl -> QueryInterface(This,riid,ppvObject) ) 

#define IDebugRestrictedExceptionInfo110_AddRef(This)	\
    ( (This)->lpVtbl -> AddRef(This) ) 

#define IDebugRestrictedExceptionInfo110_Release(This)	\
    ( (This)->lpVtbl -> Release(This) ) 


#define IDebugRestrictedExceptionInfo110_GetInfo(This,pbstrCapabilitySid,pbstrRestrictedReference)	\
    ( (This)->lpVtbl -> GetInfo(This,pbstrCapabilitySid,pbstrRestrictedReference) ) 

#endif /* COBJMACROS */


#endif 	/* C style interface */




#endif 	/* __IDebugRestrictedExceptionInfo110_INTERFACE_DEFINED__ */


#ifndef __IDebugSessionProcess110_INTERFACE_DEFINED__
#define __IDebugSessionProcess110_INTERFACE_DEFINED__

/* interface IDebugSessionProcess110 */
/* [unique][uuid][object] */ 


EXTERN_C const IID IID_IDebugSessionProcess110;

#if defined(__cplusplus) && !defined(CINTERFACE)
    
    MIDL_INTERFACE("e0b1eeac-4cb1-4421-a46b-c7a42f89ab67")
    IDebugSessionProcess110 : public IUnknown
    {
    public:
        virtual HRESULT STDMETHODCALLTYPE GetProcessorArchitecture( 
            /* [out] */ __RPC__out UINT16 *wProcessorArchitecture) = 0;
        
        virtual HRESULT STDMETHODCALLTYPE GetLaunchOptionsString( 
            /* [out] */ __RPC__deref_out_opt BSTR *bstrOptions) = 0;
        
    };
    
    
#else 	/* C style interface */

    typedef struct IDebugSessionProcess110Vtbl
    {
        BEGIN_INTERFACE
        
        HRESULT ( STDMETHODCALLTYPE *QueryInterface )( 
            __RPC__in IDebugSessionProcess110 * This,
            /* [in] */ __RPC__in REFIID riid,
            /* [annotation][iid_is][out] */ 
            _COM_Outptr_  void **ppvObject);
        
        ULONG ( STDMETHODCALLTYPE *AddRef )( 
            __RPC__in IDebugSessionProcess110 * This);
        
        ULONG ( STDMETHODCALLTYPE *Release )( 
            __RPC__in IDebugSessionProcess110 * This);
        
        HRESULT ( STDMETHODCALLTYPE *GetProcessorArchitecture )( 
            __RPC__in IDebugSessionProcess110 * This,
            /* [out] */ __RPC__out UINT16 *wProcessorArchitecture);
        
        HRESULT ( STDMETHODCALLTYPE *GetLaunchOptionsString )( 
            __RPC__in IDebugSessionProcess110 * This,
            /* [out] */ __RPC__deref_out_opt BSTR *bstrOptions);
        
        END_INTERFACE
    } IDebugSessionProcess110Vtbl;

    interface IDebugSessionProcess110
    {
        CONST_VTBL struct IDebugSessionProcess110Vtbl *lpVtbl;
    };

    

#ifdef COBJMACROS


#define IDebugSessionProcess110_QueryInterface(This,riid,ppvObject)	\
    ( (This)->lpVtbl -> QueryInterface(This,riid,ppvObject) ) 

#define IDebugSessionProcess110_AddRef(This)	\
    ( (This)->lpVtbl -> AddRef(This) ) 

#define IDebugSessionProcess110_Release(This)	\
    ( (This)->lpVtbl -> Release(This) ) 


#define IDebugSessionProcess110_GetProcessorArchitecture(This,wProcessorArchitecture)	\
    ( (This)->lpVtbl -> GetProcessorArchitecture(This,wProcessorArchitecture) ) 

#define IDebugSessionProcess110_GetLaunchOptionsString(This,bstrOptions)	\
    ( (This)->lpVtbl -> GetLaunchOptionsString(This,bstrOptions) ) 

#endif /* COBJMACROS */


#endif 	/* C style interface */




#endif 	/* __IDebugSessionProcess110_INTERFACE_DEFINED__ */


#ifndef __IDebugBreakpointFileUpdateNotification110_INTERFACE_DEFINED__
#define __IDebugBreakpointFileUpdateNotification110_INTERFACE_DEFINED__

/* interface IDebugBreakpointFileUpdateNotification110 */
/* [unique][uuid][object] */ 


EXTERN_C const IID IID_IDebugBreakpointFileUpdateNotification110;

#if defined(__cplusplus) && !defined(CINTERFACE)
    
    MIDL_INTERFACE("A87AB572-49C1-4BF5-9F1B-E0A3F94CA27B")
    IDebugBreakpointFileUpdateNotification110 : public IUnknown
    {
    public:
        virtual HRESULT STDMETHODCALLTYPE OnBreakpointFilesUpdated( 
            /* [in] */ DWORD filePathCount,
            /* [size_is][in] */ __RPC__in_ecount_full(filePathCount) LPCWSTR *filePathArray) = 0;
        
        virtual HRESULT STDMETHODCALLTYPE WaitForBreakpointFileUpdateProcessingComplete( void) = 0;
        
    };
    
    
#else 	/* C style interface */

    typedef struct IDebugBreakpointFileUpdateNotification110Vtbl
    {
        BEGIN_INTERFACE
        
        HRESULT ( STDMETHODCALLTYPE *QueryInterface )( 
            __RPC__in IDebugBreakpointFileUpdateNotification110 * This,
            /* [in] */ __RPC__in REFIID riid,
            /* [annotation][iid_is][out] */ 
            _COM_Outptr_  void **ppvObject);
        
        ULONG ( STDMETHODCALLTYPE *AddRef )( 
            __RPC__in IDebugBreakpointFileUpdateNotification110 * This);
        
        ULONG ( STDMETHODCALLTYPE *Release )( 
            __RPC__in IDebugBreakpointFileUpdateNotification110 * This);
        
        HRESULT ( STDMETHODCALLTYPE *OnBreakpointFilesUpdated )( 
            __RPC__in IDebugBreakpointFileUpdateNotification110 * This,
            /* [in] */ DWORD filePathCount,
            /* [size_is][in] */ __RPC__in_ecount_full(filePathCount) LPCWSTR *filePathArray);
        
        HRESULT ( STDMETHODCALLTYPE *WaitForBreakpointFileUpdateProcessingComplete )( 
            __RPC__in IDebugBreakpointFileUpdateNotification110 * This);
        
        END_INTERFACE
    } IDebugBreakpointFileUpdateNotification110Vtbl;

    interface IDebugBreakpointFileUpdateNotification110
    {
        CONST_VTBL struct IDebugBreakpointFileUpdateNotification110Vtbl *lpVtbl;
    };

    

#ifdef COBJMACROS


#define IDebugBreakpointFileUpdateNotification110_QueryInterface(This,riid,ppvObject)	\
    ( (This)->lpVtbl -> QueryInterface(This,riid,ppvObject) ) 

#define IDebugBreakpointFileUpdateNotification110_AddRef(This)	\
    ( (This)->lpVtbl -> AddRef(This) ) 

#define IDebugBreakpointFileUpdateNotification110_Release(This)	\
    ( (This)->lpVtbl -> Release(This) ) 


#define IDebugBreakpointFileUpdateNotification110_OnBreakpointFilesUpdated(This,filePathCount,filePathArray)	\
    ( (This)->lpVtbl -> OnBreakpointFilesUpdated(This,filePathCount,filePathArray) ) 

#define IDebugBreakpointFileUpdateNotification110_WaitForBreakpointFileUpdateProcessingComplete(This)	\
    ( (This)->lpVtbl -> WaitForBreakpointFileUpdateProcessingComplete(This) ) 

#endif /* COBJMACROS */


#endif 	/* C style interface */




#endif 	/* __IDebugBreakpointFileUpdateNotification110_INTERFACE_DEFINED__ */


#ifndef __IDebugProperty110_INTERFACE_DEFINED__
#define __IDebugProperty110_INTERFACE_DEFINED__

/* interface IDebugProperty110 */
/* [unique][uuid][object] */ 


EXTERN_C const IID IID_IDebugProperty110;

#if defined(__cplusplus) && !defined(CINTERFACE)
    
    MIDL_INTERFACE("9119092a-be6b-44f0-914a-c9e83b9223cf")
    IDebugProperty110 : public IUnknown
    {
    public:
        virtual HRESULT STDMETHODCALLTYPE GetExternalModules( 
            /* [out] */ __RPC__deref_out_opt IEnumDebugModules2 **ppEnum) = 0;
        
    };
    
    
#else 	/* C style interface */

    typedef struct IDebugProperty110Vtbl
    {
        BEGIN_INTERFACE
        
        HRESULT ( STDMETHODCALLTYPE *QueryInterface )( 
            __RPC__in IDebugProperty110 * This,
            /* [in] */ __RPC__in REFIID riid,
            /* [annotation][iid_is][out] */ 
            _COM_Outptr_  void **ppvObject);
        
        ULONG ( STDMETHODCALLTYPE *AddRef )( 
            __RPC__in IDebugProperty110 * This);
        
        ULONG ( STDMETHODCALLTYPE *Release )( 
            __RPC__in IDebugProperty110 * This);
        
        HRESULT ( STDMETHODCALLTYPE *GetExternalModules )( 
            __RPC__in IDebugProperty110 * This,
            /* [out] */ __RPC__deref_out_opt IEnumDebugModules2 **ppEnum);
        
        END_INTERFACE
    } IDebugProperty110Vtbl;

    interface IDebugProperty110
    {
        CONST_VTBL struct IDebugProperty110Vtbl *lpVtbl;
    };

    

#ifdef COBJMACROS


#define IDebugProperty110_QueryInterface(This,riid,ppvObject)	\
    ( (This)->lpVtbl -> QueryInterface(This,riid,ppvObject) ) 

#define IDebugProperty110_AddRef(This)	\
    ( (This)->lpVtbl -> AddRef(This) ) 

#define IDebugProperty110_Release(This)	\
    ( (This)->lpVtbl -> Release(This) ) 


#define IDebugProperty110_GetExternalModules(This,ppEnum)	\
    ( (This)->lpVtbl -> GetExternalModules(This,ppEnum) ) 

#endif /* COBJMACROS */


#endif 	/* C style interface */




#endif 	/* __IDebugProperty110_INTERFACE_DEFINED__ */


/* Additional Prototypes for ALL interfaces */

unsigned long             __RPC_USER  BSTR_UserSize(     __RPC__in unsigned long *, unsigned long            , __RPC__in BSTR * ); 
unsigned char * __RPC_USER  BSTR_UserMarshal(  __RPC__in unsigned long *, __RPC__inout_xcount(0) unsigned char *, __RPC__in BSTR * ); 
unsigned char * __RPC_USER  BSTR_UserUnmarshal(__RPC__in unsigned long *, __RPC__in_xcount(0) unsigned char *, __RPC__out BSTR * ); 
void                      __RPC_USER  BSTR_UserFree(     __RPC__in unsigned long *, __RPC__in BSTR * ); 

unsigned long             __RPC_USER  VARIANT_UserSize(     __RPC__in unsigned long *, unsigned long            , __RPC__in VARIANT * ); 
unsigned char * __RPC_USER  VARIANT_UserMarshal(  __RPC__in unsigned long *, __RPC__inout_xcount(0) unsigned char *, __RPC__in VARIANT * ); 
unsigned char * __RPC_USER  VARIANT_UserUnmarshal(__RPC__in unsigned long *, __RPC__in_xcount(0) unsigned char *, __RPC__out VARIANT * ); 
void                      __RPC_USER  VARIANT_UserFree(     __RPC__in unsigned long *, __RPC__in VARIANT * ); 

/* end of Additional Prototypes */

#ifdef __cplusplus
}
#endif

#endif


