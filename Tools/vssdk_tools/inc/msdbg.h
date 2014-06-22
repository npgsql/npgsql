

/* this ALWAYS GENERATED file contains the definitions for the interfaces */


 /* File created by MIDL compiler version 7.00.0499 */
/* Compiler settings for msdbg.idl:
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

#ifndef __msdbg_h__
#define __msdbg_h__

#if defined(_MSC_VER) && (_MSC_VER >= 1020)
#pragma once
#endif

/* Forward Declarations */ 

#ifndef __IDebugCoreServer2_FWD_DEFINED__
#define __IDebugCoreServer2_FWD_DEFINED__
typedef interface IDebugCoreServer2 IDebugCoreServer2;
#endif 	/* __IDebugCoreServer2_FWD_DEFINED__ */


#ifndef __IDebugCoreServer3_FWD_DEFINED__
#define __IDebugCoreServer3_FWD_DEFINED__
typedef interface IDebugCoreServer3 IDebugCoreServer3;
#endif 	/* __IDebugCoreServer3_FWD_DEFINED__ */


#ifndef __IDebugMachineEx2_V7_FWD_DEFINED__
#define __IDebugMachineEx2_V7_FWD_DEFINED__
typedef interface IDebugMachineEx2_V7 IDebugMachineEx2_V7;
#endif 	/* __IDebugMachineEx2_V7_FWD_DEFINED__ */


#ifndef __IDebugPortSupplier2_FWD_DEFINED__
#define __IDebugPortSupplier2_FWD_DEFINED__
typedef interface IDebugPortSupplier2 IDebugPortSupplier2;
#endif 	/* __IDebugPortSupplier2_FWD_DEFINED__ */


#ifndef __IDebugPortSupplier3_FWD_DEFINED__
#define __IDebugPortSupplier3_FWD_DEFINED__
typedef interface IDebugPortSupplier3 IDebugPortSupplier3;
#endif 	/* __IDebugPortSupplier3_FWD_DEFINED__ */


#ifndef __IDebugPortPicker_FWD_DEFINED__
#define __IDebugPortPicker_FWD_DEFINED__
typedef interface IDebugPortPicker IDebugPortPicker;
#endif 	/* __IDebugPortPicker_FWD_DEFINED__ */


#ifndef __IDebugPortSupplierDescription2_FWD_DEFINED__
#define __IDebugPortSupplierDescription2_FWD_DEFINED__
typedef interface IDebugPortSupplierDescription2 IDebugPortSupplierDescription2;
#endif 	/* __IDebugPortSupplierDescription2_FWD_DEFINED__ */


#ifndef __IDebugPort2_FWD_DEFINED__
#define __IDebugPort2_FWD_DEFINED__
typedef interface IDebugPort2 IDebugPort2;
#endif 	/* __IDebugPort2_FWD_DEFINED__ */


#ifndef __IDebugDefaultPort2_FWD_DEFINED__
#define __IDebugDefaultPort2_FWD_DEFINED__
typedef interface IDebugDefaultPort2 IDebugDefaultPort2;
#endif 	/* __IDebugDefaultPort2_FWD_DEFINED__ */


#ifndef __IDebugWindowsComputerPort2_FWD_DEFINED__
#define __IDebugWindowsComputerPort2_FWD_DEFINED__
typedef interface IDebugWindowsComputerPort2 IDebugWindowsComputerPort2;
#endif 	/* __IDebugWindowsComputerPort2_FWD_DEFINED__ */


#ifndef __IDebugPortRequest2_FWD_DEFINED__
#define __IDebugPortRequest2_FWD_DEFINED__
typedef interface IDebugPortRequest2 IDebugPortRequest2;
#endif 	/* __IDebugPortRequest2_FWD_DEFINED__ */


#ifndef __IDebugPortNotify2_FWD_DEFINED__
#define __IDebugPortNotify2_FWD_DEFINED__
typedef interface IDebugPortNotify2 IDebugPortNotify2;
#endif 	/* __IDebugPortNotify2_FWD_DEFINED__ */


#ifndef __IDebugPortEvents2_FWD_DEFINED__
#define __IDebugPortEvents2_FWD_DEFINED__
typedef interface IDebugPortEvents2 IDebugPortEvents2;
#endif 	/* __IDebugPortEvents2_FWD_DEFINED__ */


#ifndef __IDebugMDMUtil2_V7_FWD_DEFINED__
#define __IDebugMDMUtil2_V7_FWD_DEFINED__
typedef interface IDebugMDMUtil2_V7 IDebugMDMUtil2_V7;
#endif 	/* __IDebugMDMUtil2_V7_FWD_DEFINED__ */


#ifndef __IDebugMDMUtil3_V7_FWD_DEFINED__
#define __IDebugMDMUtil3_V7_FWD_DEFINED__
typedef interface IDebugMDMUtil3_V7 IDebugMDMUtil3_V7;
#endif 	/* __IDebugMDMUtil3_V7_FWD_DEFINED__ */


#ifndef __IDebugSession2_FWD_DEFINED__
#define __IDebugSession2_FWD_DEFINED__
typedef interface IDebugSession2 IDebugSession2;
#endif 	/* __IDebugSession2_FWD_DEFINED__ */


#ifndef __IDebugSession3_FWD_DEFINED__
#define __IDebugSession3_FWD_DEFINED__
typedef interface IDebugSession3 IDebugSession3;
#endif 	/* __IDebugSession3_FWD_DEFINED__ */


#ifndef __IDebugEngine2_FWD_DEFINED__
#define __IDebugEngine2_FWD_DEFINED__
typedef interface IDebugEngine2 IDebugEngine2;
#endif 	/* __IDebugEngine2_FWD_DEFINED__ */


#ifndef __IDebugEngineLaunch2_FWD_DEFINED__
#define __IDebugEngineLaunch2_FWD_DEFINED__
typedef interface IDebugEngineLaunch2 IDebugEngineLaunch2;
#endif 	/* __IDebugEngineLaunch2_FWD_DEFINED__ */


#ifndef __IDebugEngine3_FWD_DEFINED__
#define __IDebugEngine3_FWD_DEFINED__
typedef interface IDebugEngine3 IDebugEngine3;
#endif 	/* __IDebugEngine3_FWD_DEFINED__ */


#ifndef __IDebugEventCallback2_FWD_DEFINED__
#define __IDebugEventCallback2_FWD_DEFINED__
typedef interface IDebugEventCallback2 IDebugEventCallback2;
#endif 	/* __IDebugEventCallback2_FWD_DEFINED__ */


#ifndef __IDebugSettingsCallback2_FWD_DEFINED__
#define __IDebugSettingsCallback2_FWD_DEFINED__
typedef interface IDebugSettingsCallback2 IDebugSettingsCallback2;
#endif 	/* __IDebugSettingsCallback2_FWD_DEFINED__ */


#ifndef __IDebugEvent2_FWD_DEFINED__
#define __IDebugEvent2_FWD_DEFINED__
typedef interface IDebugEvent2 IDebugEvent2;
#endif 	/* __IDebugEvent2_FWD_DEFINED__ */


#ifndef __IDebugSessionCreateEvent2_FWD_DEFINED__
#define __IDebugSessionCreateEvent2_FWD_DEFINED__
typedef interface IDebugSessionCreateEvent2 IDebugSessionCreateEvent2;
#endif 	/* __IDebugSessionCreateEvent2_FWD_DEFINED__ */


#ifndef __IDebugSessionDestroyEvent2_FWD_DEFINED__
#define __IDebugSessionDestroyEvent2_FWD_DEFINED__
typedef interface IDebugSessionDestroyEvent2 IDebugSessionDestroyEvent2;
#endif 	/* __IDebugSessionDestroyEvent2_FWD_DEFINED__ */


#ifndef __IDebugEngineCreateEvent2_FWD_DEFINED__
#define __IDebugEngineCreateEvent2_FWD_DEFINED__
typedef interface IDebugEngineCreateEvent2 IDebugEngineCreateEvent2;
#endif 	/* __IDebugEngineCreateEvent2_FWD_DEFINED__ */


#ifndef __IDebugProcessCreateEvent2_FWD_DEFINED__
#define __IDebugProcessCreateEvent2_FWD_DEFINED__
typedef interface IDebugProcessCreateEvent2 IDebugProcessCreateEvent2;
#endif 	/* __IDebugProcessCreateEvent2_FWD_DEFINED__ */


#ifndef __IDebugProcessDestroyEvent2_FWD_DEFINED__
#define __IDebugProcessDestroyEvent2_FWD_DEFINED__
typedef interface IDebugProcessDestroyEvent2 IDebugProcessDestroyEvent2;
#endif 	/* __IDebugProcessDestroyEvent2_FWD_DEFINED__ */


#ifndef __IDebugProgramCreateEvent2_FWD_DEFINED__
#define __IDebugProgramCreateEvent2_FWD_DEFINED__
typedef interface IDebugProgramCreateEvent2 IDebugProgramCreateEvent2;
#endif 	/* __IDebugProgramCreateEvent2_FWD_DEFINED__ */


#ifndef __IDebugProgramDestroyEvent2_FWD_DEFINED__
#define __IDebugProgramDestroyEvent2_FWD_DEFINED__
typedef interface IDebugProgramDestroyEvent2 IDebugProgramDestroyEvent2;
#endif 	/* __IDebugProgramDestroyEvent2_FWD_DEFINED__ */


#ifndef __IDebugProgramDestroyEventFlags2_FWD_DEFINED__
#define __IDebugProgramDestroyEventFlags2_FWD_DEFINED__
typedef interface IDebugProgramDestroyEventFlags2 IDebugProgramDestroyEventFlags2;
#endif 	/* __IDebugProgramDestroyEventFlags2_FWD_DEFINED__ */


#ifndef __IDebugThreadCreateEvent2_FWD_DEFINED__
#define __IDebugThreadCreateEvent2_FWD_DEFINED__
typedef interface IDebugThreadCreateEvent2 IDebugThreadCreateEvent2;
#endif 	/* __IDebugThreadCreateEvent2_FWD_DEFINED__ */


#ifndef __IDebugThreadDestroyEvent2_FWD_DEFINED__
#define __IDebugThreadDestroyEvent2_FWD_DEFINED__
typedef interface IDebugThreadDestroyEvent2 IDebugThreadDestroyEvent2;
#endif 	/* __IDebugThreadDestroyEvent2_FWD_DEFINED__ */


#ifndef __IDebugLoadCompleteEvent2_FWD_DEFINED__
#define __IDebugLoadCompleteEvent2_FWD_DEFINED__
typedef interface IDebugLoadCompleteEvent2 IDebugLoadCompleteEvent2;
#endif 	/* __IDebugLoadCompleteEvent2_FWD_DEFINED__ */


#ifndef __IDebugEntryPointEvent2_FWD_DEFINED__
#define __IDebugEntryPointEvent2_FWD_DEFINED__
typedef interface IDebugEntryPointEvent2 IDebugEntryPointEvent2;
#endif 	/* __IDebugEntryPointEvent2_FWD_DEFINED__ */


#ifndef __IDebugStepCompleteEvent2_FWD_DEFINED__
#define __IDebugStepCompleteEvent2_FWD_DEFINED__
typedef interface IDebugStepCompleteEvent2 IDebugStepCompleteEvent2;
#endif 	/* __IDebugStepCompleteEvent2_FWD_DEFINED__ */


#ifndef __IDebugCanStopEvent2_FWD_DEFINED__
#define __IDebugCanStopEvent2_FWD_DEFINED__
typedef interface IDebugCanStopEvent2 IDebugCanStopEvent2;
#endif 	/* __IDebugCanStopEvent2_FWD_DEFINED__ */


#ifndef __IDebugBreakEvent2_FWD_DEFINED__
#define __IDebugBreakEvent2_FWD_DEFINED__
typedef interface IDebugBreakEvent2 IDebugBreakEvent2;
#endif 	/* __IDebugBreakEvent2_FWD_DEFINED__ */


#ifndef __IDebugBreakpointEvent2_FWD_DEFINED__
#define __IDebugBreakpointEvent2_FWD_DEFINED__
typedef interface IDebugBreakpointEvent2 IDebugBreakpointEvent2;
#endif 	/* __IDebugBreakpointEvent2_FWD_DEFINED__ */


#ifndef __IDebugExceptionEvent2_FWD_DEFINED__
#define __IDebugExceptionEvent2_FWD_DEFINED__
typedef interface IDebugExceptionEvent2 IDebugExceptionEvent2;
#endif 	/* __IDebugExceptionEvent2_FWD_DEFINED__ */


#ifndef __IDebugNativeExceptionInfo_FWD_DEFINED__
#define __IDebugNativeExceptionInfo_FWD_DEFINED__
typedef interface IDebugNativeExceptionInfo IDebugNativeExceptionInfo;
#endif 	/* __IDebugNativeExceptionInfo_FWD_DEFINED__ */


#ifndef __IDebugManagedExceptionInfo2_FWD_DEFINED__
#define __IDebugManagedExceptionInfo2_FWD_DEFINED__
typedef interface IDebugManagedExceptionInfo2 IDebugManagedExceptionInfo2;
#endif 	/* __IDebugManagedExceptionInfo2_FWD_DEFINED__ */


#ifndef __IDebugOutputStringEvent2_FWD_DEFINED__
#define __IDebugOutputStringEvent2_FWD_DEFINED__
typedef interface IDebugOutputStringEvent2 IDebugOutputStringEvent2;
#endif 	/* __IDebugOutputStringEvent2_FWD_DEFINED__ */


#ifndef __IDebugModuleLoadEvent2_FWD_DEFINED__
#define __IDebugModuleLoadEvent2_FWD_DEFINED__
typedef interface IDebugModuleLoadEvent2 IDebugModuleLoadEvent2;
#endif 	/* __IDebugModuleLoadEvent2_FWD_DEFINED__ */


#ifndef __IDebugSymbolSearchEvent2_FWD_DEFINED__
#define __IDebugSymbolSearchEvent2_FWD_DEFINED__
typedef interface IDebugSymbolSearchEvent2 IDebugSymbolSearchEvent2;
#endif 	/* __IDebugSymbolSearchEvent2_FWD_DEFINED__ */


#ifndef __IDebugBeforeSymbolSearchEvent2_FWD_DEFINED__
#define __IDebugBeforeSymbolSearchEvent2_FWD_DEFINED__
typedef interface IDebugBeforeSymbolSearchEvent2 IDebugBeforeSymbolSearchEvent2;
#endif 	/* __IDebugBeforeSymbolSearchEvent2_FWD_DEFINED__ */


#ifndef __IDebugPropertyCreateEvent2_FWD_DEFINED__
#define __IDebugPropertyCreateEvent2_FWD_DEFINED__
typedef interface IDebugPropertyCreateEvent2 IDebugPropertyCreateEvent2;
#endif 	/* __IDebugPropertyCreateEvent2_FWD_DEFINED__ */


#ifndef __IDebugPropertyDestroyEvent2_FWD_DEFINED__
#define __IDebugPropertyDestroyEvent2_FWD_DEFINED__
typedef interface IDebugPropertyDestroyEvent2 IDebugPropertyDestroyEvent2;
#endif 	/* __IDebugPropertyDestroyEvent2_FWD_DEFINED__ */


#ifndef __IDebugBreakpointBoundEvent2_FWD_DEFINED__
#define __IDebugBreakpointBoundEvent2_FWD_DEFINED__
typedef interface IDebugBreakpointBoundEvent2 IDebugBreakpointBoundEvent2;
#endif 	/* __IDebugBreakpointBoundEvent2_FWD_DEFINED__ */


#ifndef __IDebugBreakpointUnboundEvent2_FWD_DEFINED__
#define __IDebugBreakpointUnboundEvent2_FWD_DEFINED__
typedef interface IDebugBreakpointUnboundEvent2 IDebugBreakpointUnboundEvent2;
#endif 	/* __IDebugBreakpointUnboundEvent2_FWD_DEFINED__ */


#ifndef __IDebugBreakpointErrorEvent2_FWD_DEFINED__
#define __IDebugBreakpointErrorEvent2_FWD_DEFINED__
typedef interface IDebugBreakpointErrorEvent2 IDebugBreakpointErrorEvent2;
#endif 	/* __IDebugBreakpointErrorEvent2_FWD_DEFINED__ */


#ifndef __IDebugExpressionEvaluationCompleteEvent2_FWD_DEFINED__
#define __IDebugExpressionEvaluationCompleteEvent2_FWD_DEFINED__
typedef interface IDebugExpressionEvaluationCompleteEvent2 IDebugExpressionEvaluationCompleteEvent2;
#endif 	/* __IDebugExpressionEvaluationCompleteEvent2_FWD_DEFINED__ */


#ifndef __IDebugReturnValueEvent2_FWD_DEFINED__
#define __IDebugReturnValueEvent2_FWD_DEFINED__
typedef interface IDebugReturnValueEvent2 IDebugReturnValueEvent2;
#endif 	/* __IDebugReturnValueEvent2_FWD_DEFINED__ */


#ifndef __IDebugNoSymbolsEvent2_FWD_DEFINED__
#define __IDebugNoSymbolsEvent2_FWD_DEFINED__
typedef interface IDebugNoSymbolsEvent2 IDebugNoSymbolsEvent2;
#endif 	/* __IDebugNoSymbolsEvent2_FWD_DEFINED__ */


#ifndef __IDebugProgramNameChangedEvent2_FWD_DEFINED__
#define __IDebugProgramNameChangedEvent2_FWD_DEFINED__
typedef interface IDebugProgramNameChangedEvent2 IDebugProgramNameChangedEvent2;
#endif 	/* __IDebugProgramNameChangedEvent2_FWD_DEFINED__ */


#ifndef __IDebugThreadNameChangedEvent2_FWD_DEFINED__
#define __IDebugThreadNameChangedEvent2_FWD_DEFINED__
typedef interface IDebugThreadNameChangedEvent2 IDebugThreadNameChangedEvent2;
#endif 	/* __IDebugThreadNameChangedEvent2_FWD_DEFINED__ */


#ifndef __IDebugMessageEvent2_FWD_DEFINED__
#define __IDebugMessageEvent2_FWD_DEFINED__
typedef interface IDebugMessageEvent2 IDebugMessageEvent2;
#endif 	/* __IDebugMessageEvent2_FWD_DEFINED__ */


#ifndef __IDebugErrorEvent2_FWD_DEFINED__
#define __IDebugErrorEvent2_FWD_DEFINED__
typedef interface IDebugErrorEvent2 IDebugErrorEvent2;
#endif 	/* __IDebugErrorEvent2_FWD_DEFINED__ */


#ifndef __IDebugActivateDocumentEvent2_FWD_DEFINED__
#define __IDebugActivateDocumentEvent2_FWD_DEFINED__
typedef interface IDebugActivateDocumentEvent2 IDebugActivateDocumentEvent2;
#endif 	/* __IDebugActivateDocumentEvent2_FWD_DEFINED__ */


#ifndef __IDebugInterceptExceptionCompleteEvent2_FWD_DEFINED__
#define __IDebugInterceptExceptionCompleteEvent2_FWD_DEFINED__
typedef interface IDebugInterceptExceptionCompleteEvent2 IDebugInterceptExceptionCompleteEvent2;
#endif 	/* __IDebugInterceptExceptionCompleteEvent2_FWD_DEFINED__ */


#ifndef __IDebugAttachCompleteEvent2_FWD_DEFINED__
#define __IDebugAttachCompleteEvent2_FWD_DEFINED__
typedef interface IDebugAttachCompleteEvent2 IDebugAttachCompleteEvent2;
#endif 	/* __IDebugAttachCompleteEvent2_FWD_DEFINED__ */


#ifndef __IDebugFuncEvalAbortedEvent2_FWD_DEFINED__
#define __IDebugFuncEvalAbortedEvent2_FWD_DEFINED__
typedef interface IDebugFuncEvalAbortedEvent2 IDebugFuncEvalAbortedEvent2;
#endif 	/* __IDebugFuncEvalAbortedEvent2_FWD_DEFINED__ */


#ifndef __IDebugStopCompleteEvent2_FWD_DEFINED__
#define __IDebugStopCompleteEvent2_FWD_DEFINED__
typedef interface IDebugStopCompleteEvent2 IDebugStopCompleteEvent2;
#endif 	/* __IDebugStopCompleteEvent2_FWD_DEFINED__ */


#ifndef __IDebugEncNotify_FWD_DEFINED__
#define __IDebugEncNotify_FWD_DEFINED__
typedef interface IDebugEncNotify IDebugEncNotify;
#endif 	/* __IDebugEncNotify_FWD_DEFINED__ */


#ifndef __IDebugSessionEvent2_FWD_DEFINED__
#define __IDebugSessionEvent2_FWD_DEFINED__
typedef interface IDebugSessionEvent2 IDebugSessionEvent2;
#endif 	/* __IDebugSessionEvent2_FWD_DEFINED__ */


#ifndef __IDebugProcess2_FWD_DEFINED__
#define __IDebugProcess2_FWD_DEFINED__
typedef interface IDebugProcess2 IDebugProcess2;
#endif 	/* __IDebugProcess2_FWD_DEFINED__ */


#ifndef __IDebugProcess3_FWD_DEFINED__
#define __IDebugProcess3_FWD_DEFINED__
typedef interface IDebugProcess3 IDebugProcess3;
#endif 	/* __IDebugProcess3_FWD_DEFINED__ */


#ifndef __IDebugProcessSecurity2_FWD_DEFINED__
#define __IDebugProcessSecurity2_FWD_DEFINED__
typedef interface IDebugProcessSecurity2 IDebugProcessSecurity2;
#endif 	/* __IDebugProcessSecurity2_FWD_DEFINED__ */


#ifndef __IDebugProgram2_FWD_DEFINED__
#define __IDebugProgram2_FWD_DEFINED__
typedef interface IDebugProgram2 IDebugProgram2;
#endif 	/* __IDebugProgram2_FWD_DEFINED__ */


#ifndef __IDebugProgram3_FWD_DEFINED__
#define __IDebugProgram3_FWD_DEFINED__
typedef interface IDebugProgram3 IDebugProgram3;
#endif 	/* __IDebugProgram3_FWD_DEFINED__ */


#ifndef __IDebugEngineProgram2_FWD_DEFINED__
#define __IDebugEngineProgram2_FWD_DEFINED__
typedef interface IDebugEngineProgram2 IDebugEngineProgram2;
#endif 	/* __IDebugEngineProgram2_FWD_DEFINED__ */


#ifndef __IDebugProgramHost2_FWD_DEFINED__
#define __IDebugProgramHost2_FWD_DEFINED__
typedef interface IDebugProgramHost2 IDebugProgramHost2;
#endif 	/* __IDebugProgramHost2_FWD_DEFINED__ */


#ifndef __IDebugProgramNode2_FWD_DEFINED__
#define __IDebugProgramNode2_FWD_DEFINED__
typedef interface IDebugProgramNode2 IDebugProgramNode2;
#endif 	/* __IDebugProgramNode2_FWD_DEFINED__ */


#ifndef __IDebugProgramNodeAttach2_FWD_DEFINED__
#define __IDebugProgramNodeAttach2_FWD_DEFINED__
typedef interface IDebugProgramNodeAttach2 IDebugProgramNodeAttach2;
#endif 	/* __IDebugProgramNodeAttach2_FWD_DEFINED__ */


#ifndef __IDebugProgramEngines2_FWD_DEFINED__
#define __IDebugProgramEngines2_FWD_DEFINED__
typedef interface IDebugProgramEngines2 IDebugProgramEngines2;
#endif 	/* __IDebugProgramEngines2_FWD_DEFINED__ */


#ifndef __IDebugCOMPlusProgramNode2_FWD_DEFINED__
#define __IDebugCOMPlusProgramNode2_FWD_DEFINED__
typedef interface IDebugCOMPlusProgramNode2 IDebugCOMPlusProgramNode2;
#endif 	/* __IDebugCOMPlusProgramNode2_FWD_DEFINED__ */


#ifndef __IDebugSQLCLRProgramNode2_FWD_DEFINED__
#define __IDebugSQLCLRProgramNode2_FWD_DEFINED__
typedef interface IDebugSQLCLRProgramNode2 IDebugSQLCLRProgramNode2;
#endif 	/* __IDebugSQLCLRProgramNode2_FWD_DEFINED__ */


#ifndef __IDebugThread2_FWD_DEFINED__
#define __IDebugThread2_FWD_DEFINED__
typedef interface IDebugThread2 IDebugThread2;
#endif 	/* __IDebugThread2_FWD_DEFINED__ */


#ifndef __IDebugLogicalThread2_FWD_DEFINED__
#define __IDebugLogicalThread2_FWD_DEFINED__
typedef interface IDebugLogicalThread2 IDebugLogicalThread2;
#endif 	/* __IDebugLogicalThread2_FWD_DEFINED__ */


#ifndef __IDebugThread3_FWD_DEFINED__
#define __IDebugThread3_FWD_DEFINED__
typedef interface IDebugThread3 IDebugThread3;
#endif 	/* __IDebugThread3_FWD_DEFINED__ */


#ifndef __IDebugProperty2_FWD_DEFINED__
#define __IDebugProperty2_FWD_DEFINED__
typedef interface IDebugProperty2 IDebugProperty2;
#endif 	/* __IDebugProperty2_FWD_DEFINED__ */


#ifndef __IDebugProperty3_FWD_DEFINED__
#define __IDebugProperty3_FWD_DEFINED__
typedef interface IDebugProperty3 IDebugProperty3;
#endif 	/* __IDebugProperty3_FWD_DEFINED__ */


#ifndef __IDebugSessionProperty2_FWD_DEFINED__
#define __IDebugSessionProperty2_FWD_DEFINED__
typedef interface IDebugSessionProperty2 IDebugSessionProperty2;
#endif 	/* __IDebugSessionProperty2_FWD_DEFINED__ */


#ifndef __IDebugPropertyClose2_FWD_DEFINED__
#define __IDebugPropertyClose2_FWD_DEFINED__
typedef interface IDebugPropertyClose2 IDebugPropertyClose2;
#endif 	/* __IDebugPropertyClose2_FWD_DEFINED__ */


#ifndef __IDebugDataGrid_FWD_DEFINED__
#define __IDebugDataGrid_FWD_DEFINED__
typedef interface IDebugDataGrid IDebugDataGrid;
#endif 	/* __IDebugDataGrid_FWD_DEFINED__ */


#ifndef __IDebugPropertySafetyWrapper_FWD_DEFINED__
#define __IDebugPropertySafetyWrapper_FWD_DEFINED__
typedef interface IDebugPropertySafetyWrapper IDebugPropertySafetyWrapper;
#endif 	/* __IDebugPropertySafetyWrapper_FWD_DEFINED__ */


#ifndef __IDebugReference2_FWD_DEFINED__
#define __IDebugReference2_FWD_DEFINED__
typedef interface IDebugReference2 IDebugReference2;
#endif 	/* __IDebugReference2_FWD_DEFINED__ */


#ifndef __IDebugStackFrame2_FWD_DEFINED__
#define __IDebugStackFrame2_FWD_DEFINED__
typedef interface IDebugStackFrame2 IDebugStackFrame2;
#endif 	/* __IDebugStackFrame2_FWD_DEFINED__ */


#ifndef __IDebugStackFrame3_FWD_DEFINED__
#define __IDebugStackFrame3_FWD_DEFINED__
typedef interface IDebugStackFrame3 IDebugStackFrame3;
#endif 	/* __IDebugStackFrame3_FWD_DEFINED__ */


#ifndef __IDebugMemoryContext2_FWD_DEFINED__
#define __IDebugMemoryContext2_FWD_DEFINED__
typedef interface IDebugMemoryContext2 IDebugMemoryContext2;
#endif 	/* __IDebugMemoryContext2_FWD_DEFINED__ */


#ifndef __IDebugCodeContext2_FWD_DEFINED__
#define __IDebugCodeContext2_FWD_DEFINED__
typedef interface IDebugCodeContext2 IDebugCodeContext2;
#endif 	/* __IDebugCodeContext2_FWD_DEFINED__ */


#ifndef __IDebugCodeContext3_FWD_DEFINED__
#define __IDebugCodeContext3_FWD_DEFINED__
typedef interface IDebugCodeContext3 IDebugCodeContext3;
#endif 	/* __IDebugCodeContext3_FWD_DEFINED__ */


#ifndef __IDebugMemoryBytes2_FWD_DEFINED__
#define __IDebugMemoryBytes2_FWD_DEFINED__
typedef interface IDebugMemoryBytes2 IDebugMemoryBytes2;
#endif 	/* __IDebugMemoryBytes2_FWD_DEFINED__ */


#ifndef __IDebugDisassemblyStream2_FWD_DEFINED__
#define __IDebugDisassemblyStream2_FWD_DEFINED__
typedef interface IDebugDisassemblyStream2 IDebugDisassemblyStream2;
#endif 	/* __IDebugDisassemblyStream2_FWD_DEFINED__ */


#ifndef __IDebugDocumentContext2_FWD_DEFINED__
#define __IDebugDocumentContext2_FWD_DEFINED__
typedef interface IDebugDocumentContext2 IDebugDocumentContext2;
#endif 	/* __IDebugDocumentContext2_FWD_DEFINED__ */


#ifndef __IDebugDocumentChecksum2_FWD_DEFINED__
#define __IDebugDocumentChecksum2_FWD_DEFINED__
typedef interface IDebugDocumentChecksum2 IDebugDocumentChecksum2;
#endif 	/* __IDebugDocumentChecksum2_FWD_DEFINED__ */


#ifndef __IDebugENCDocumentContextUpdate_FWD_DEFINED__
#define __IDebugENCDocumentContextUpdate_FWD_DEFINED__
typedef interface IDebugENCDocumentContextUpdate IDebugENCDocumentContextUpdate;
#endif 	/* __IDebugENCDocumentContextUpdate_FWD_DEFINED__ */


#ifndef __IDebugExpressionContext2_FWD_DEFINED__
#define __IDebugExpressionContext2_FWD_DEFINED__
typedef interface IDebugExpressionContext2 IDebugExpressionContext2;
#endif 	/* __IDebugExpressionContext2_FWD_DEFINED__ */


#ifndef __IDebugBreakpointRequest2_FWD_DEFINED__
#define __IDebugBreakpointRequest2_FWD_DEFINED__
typedef interface IDebugBreakpointRequest2 IDebugBreakpointRequest2;
#endif 	/* __IDebugBreakpointRequest2_FWD_DEFINED__ */


#ifndef __IDebugBreakpointChecksumRequest2_FWD_DEFINED__
#define __IDebugBreakpointChecksumRequest2_FWD_DEFINED__
typedef interface IDebugBreakpointChecksumRequest2 IDebugBreakpointChecksumRequest2;
#endif 	/* __IDebugBreakpointChecksumRequest2_FWD_DEFINED__ */


#ifndef __IDebugBreakpointRequest3_FWD_DEFINED__
#define __IDebugBreakpointRequest3_FWD_DEFINED__
typedef interface IDebugBreakpointRequest3 IDebugBreakpointRequest3;
#endif 	/* __IDebugBreakpointRequest3_FWD_DEFINED__ */


#ifndef __IDebugBreakpointResolution2_FWD_DEFINED__
#define __IDebugBreakpointResolution2_FWD_DEFINED__
typedef interface IDebugBreakpointResolution2 IDebugBreakpointResolution2;
#endif 	/* __IDebugBreakpointResolution2_FWD_DEFINED__ */


#ifndef __IDebugErrorBreakpointResolution2_FWD_DEFINED__
#define __IDebugErrorBreakpointResolution2_FWD_DEFINED__
typedef interface IDebugErrorBreakpointResolution2 IDebugErrorBreakpointResolution2;
#endif 	/* __IDebugErrorBreakpointResolution2_FWD_DEFINED__ */


#ifndef __IDebugBoundBreakpoint2_FWD_DEFINED__
#define __IDebugBoundBreakpoint2_FWD_DEFINED__
typedef interface IDebugBoundBreakpoint2 IDebugBoundBreakpoint2;
#endif 	/* __IDebugBoundBreakpoint2_FWD_DEFINED__ */


#ifndef __IDebugBoundBreakpoint3_FWD_DEFINED__
#define __IDebugBoundBreakpoint3_FWD_DEFINED__
typedef interface IDebugBoundBreakpoint3 IDebugBoundBreakpoint3;
#endif 	/* __IDebugBoundBreakpoint3_FWD_DEFINED__ */


#ifndef __IDebugPendingBreakpoint2_FWD_DEFINED__
#define __IDebugPendingBreakpoint2_FWD_DEFINED__
typedef interface IDebugPendingBreakpoint2 IDebugPendingBreakpoint2;
#endif 	/* __IDebugPendingBreakpoint2_FWD_DEFINED__ */


#ifndef __IDebugPendingBreakpoint3_FWD_DEFINED__
#define __IDebugPendingBreakpoint3_FWD_DEFINED__
typedef interface IDebugPendingBreakpoint3 IDebugPendingBreakpoint3;
#endif 	/* __IDebugPendingBreakpoint3_FWD_DEFINED__ */


#ifndef __IDebugErrorBreakpoint2_FWD_DEFINED__
#define __IDebugErrorBreakpoint2_FWD_DEFINED__
typedef interface IDebugErrorBreakpoint2 IDebugErrorBreakpoint2;
#endif 	/* __IDebugErrorBreakpoint2_FWD_DEFINED__ */


#ifndef __IDebugExpression2_FWD_DEFINED__
#define __IDebugExpression2_FWD_DEFINED__
typedef interface IDebugExpression2 IDebugExpression2;
#endif 	/* __IDebugExpression2_FWD_DEFINED__ */


#ifndef __IDebugModule2_FWD_DEFINED__
#define __IDebugModule2_FWD_DEFINED__
typedef interface IDebugModule2 IDebugModule2;
#endif 	/* __IDebugModule2_FWD_DEFINED__ */


#ifndef __IDebugModule3_FWD_DEFINED__
#define __IDebugModule3_FWD_DEFINED__
typedef interface IDebugModule3 IDebugModule3;
#endif 	/* __IDebugModule3_FWD_DEFINED__ */


#ifndef __IDebugSourceServerModule_FWD_DEFINED__
#define __IDebugSourceServerModule_FWD_DEFINED__
typedef interface IDebugSourceServerModule IDebugSourceServerModule;
#endif 	/* __IDebugSourceServerModule_FWD_DEFINED__ */


#ifndef __IDebugModuleManaged_FWD_DEFINED__
#define __IDebugModuleManaged_FWD_DEFINED__
typedef interface IDebugModuleManaged IDebugModuleManaged;
#endif 	/* __IDebugModuleManaged_FWD_DEFINED__ */


#ifndef __IDebugDocument2_FWD_DEFINED__
#define __IDebugDocument2_FWD_DEFINED__
typedef interface IDebugDocument2 IDebugDocument2;
#endif 	/* __IDebugDocument2_FWD_DEFINED__ */


#ifndef __IDebugDocumentText2_FWD_DEFINED__
#define __IDebugDocumentText2_FWD_DEFINED__
typedef interface IDebugDocumentText2 IDebugDocumentText2;
#endif 	/* __IDebugDocumentText2_FWD_DEFINED__ */


#ifndef __IDebugDocumentPosition2_FWD_DEFINED__
#define __IDebugDocumentPosition2_FWD_DEFINED__
typedef interface IDebugDocumentPosition2 IDebugDocumentPosition2;
#endif 	/* __IDebugDocumentPosition2_FWD_DEFINED__ */


#ifndef __IDebugDocumentPositionOffset2_FWD_DEFINED__
#define __IDebugDocumentPositionOffset2_FWD_DEFINED__
typedef interface IDebugDocumentPositionOffset2 IDebugDocumentPositionOffset2;
#endif 	/* __IDebugDocumentPositionOffset2_FWD_DEFINED__ */


#ifndef __IDebugFunctionPosition2_FWD_DEFINED__
#define __IDebugFunctionPosition2_FWD_DEFINED__
typedef interface IDebugFunctionPosition2 IDebugFunctionPosition2;
#endif 	/* __IDebugFunctionPosition2_FWD_DEFINED__ */


#ifndef __IDebugDocumentTextEvents2_FWD_DEFINED__
#define __IDebugDocumentTextEvents2_FWD_DEFINED__
typedef interface IDebugDocumentTextEvents2 IDebugDocumentTextEvents2;
#endif 	/* __IDebugDocumentTextEvents2_FWD_DEFINED__ */


#ifndef __IDebugQueryEngine2_FWD_DEFINED__
#define __IDebugQueryEngine2_FWD_DEFINED__
typedef interface IDebugQueryEngine2 IDebugQueryEngine2;
#endif 	/* __IDebugQueryEngine2_FWD_DEFINED__ */


#ifndef __IEEHostServices_FWD_DEFINED__
#define __IEEHostServices_FWD_DEFINED__
typedef interface IEEHostServices IEEHostServices;
#endif 	/* __IEEHostServices_FWD_DEFINED__ */


#ifndef __IDebugCustomViewer_FWD_DEFINED__
#define __IDebugCustomViewer_FWD_DEFINED__
typedef interface IDebugCustomViewer IDebugCustomViewer;
#endif 	/* __IDebugCustomViewer_FWD_DEFINED__ */


#ifndef __IEEDataStorage_FWD_DEFINED__
#define __IEEDataStorage_FWD_DEFINED__
typedef interface IEEDataStorage IEEDataStorage;
#endif 	/* __IEEDataStorage_FWD_DEFINED__ */


#ifndef __IPropertyProxyEESide_FWD_DEFINED__
#define __IPropertyProxyEESide_FWD_DEFINED__
typedef interface IPropertyProxyEESide IPropertyProxyEESide;
#endif 	/* __IPropertyProxyEESide_FWD_DEFINED__ */


#ifndef __IPropertyProxyProvider_FWD_DEFINED__
#define __IPropertyProxyProvider_FWD_DEFINED__
typedef interface IPropertyProxyProvider IPropertyProxyProvider;
#endif 	/* __IPropertyProxyProvider_FWD_DEFINED__ */


#ifndef __IManagedViewerHost_FWD_DEFINED__
#define __IManagedViewerHost_FWD_DEFINED__
typedef interface IManagedViewerHost IManagedViewerHost;
#endif 	/* __IManagedViewerHost_FWD_DEFINED__ */


#ifndef __IEELocalObject_FWD_DEFINED__
#define __IEELocalObject_FWD_DEFINED__
typedef interface IEELocalObject IEELocalObject;
#endif 	/* __IEELocalObject_FWD_DEFINED__ */


#ifndef __IEEAssemblyRefResolveComparer_FWD_DEFINED__
#define __IEEAssemblyRefResolveComparer_FWD_DEFINED__
typedef interface IEEAssemblyRefResolveComparer IEEAssemblyRefResolveComparer;
#endif 	/* __IEEAssemblyRefResolveComparer_FWD_DEFINED__ */


#ifndef __IEEAssemblyRef_FWD_DEFINED__
#define __IEEAssemblyRef_FWD_DEFINED__
typedef interface IEEAssemblyRef IEEAssemblyRef;
#endif 	/* __IEEAssemblyRef_FWD_DEFINED__ */


#ifndef __IEEHelperObject_FWD_DEFINED__
#define __IEEHelperObject_FWD_DEFINED__
typedef interface IEEHelperObject IEEHelperObject;
#endif 	/* __IEEHelperObject_FWD_DEFINED__ */


#ifndef __IDebugExceptionCallback2_FWD_DEFINED__
#define __IDebugExceptionCallback2_FWD_DEFINED__
typedef interface IDebugExceptionCallback2 IDebugExceptionCallback2;
#endif 	/* __IDebugExceptionCallback2_FWD_DEFINED__ */


#ifndef __IEnumDebugProcesses2_FWD_DEFINED__
#define __IEnumDebugProcesses2_FWD_DEFINED__
typedef interface IEnumDebugProcesses2 IEnumDebugProcesses2;
#endif 	/* __IEnumDebugProcesses2_FWD_DEFINED__ */


#ifndef __IEnumDebugPrograms2_FWD_DEFINED__
#define __IEnumDebugPrograms2_FWD_DEFINED__
typedef interface IEnumDebugPrograms2 IEnumDebugPrograms2;
#endif 	/* __IEnumDebugPrograms2_FWD_DEFINED__ */


#ifndef __IEnumDebugThreads2_FWD_DEFINED__
#define __IEnumDebugThreads2_FWD_DEFINED__
typedef interface IEnumDebugThreads2 IEnumDebugThreads2;
#endif 	/* __IEnumDebugThreads2_FWD_DEFINED__ */


#ifndef __IEnumDebugStackFrames2_FWD_DEFINED__
#define __IEnumDebugStackFrames2_FWD_DEFINED__
typedef interface IEnumDebugStackFrames2 IEnumDebugStackFrames2;
#endif 	/* __IEnumDebugStackFrames2_FWD_DEFINED__ */


#ifndef __IEnumDebugCodeContexts2_FWD_DEFINED__
#define __IEnumDebugCodeContexts2_FWD_DEFINED__
typedef interface IEnumDebugCodeContexts2 IEnumDebugCodeContexts2;
#endif 	/* __IEnumDebugCodeContexts2_FWD_DEFINED__ */


#ifndef __IEnumDebugBoundBreakpoints2_FWD_DEFINED__
#define __IEnumDebugBoundBreakpoints2_FWD_DEFINED__
typedef interface IEnumDebugBoundBreakpoints2 IEnumDebugBoundBreakpoints2;
#endif 	/* __IEnumDebugBoundBreakpoints2_FWD_DEFINED__ */


#ifndef __IEnumDebugPendingBreakpoints2_FWD_DEFINED__
#define __IEnumDebugPendingBreakpoints2_FWD_DEFINED__
typedef interface IEnumDebugPendingBreakpoints2 IEnumDebugPendingBreakpoints2;
#endif 	/* __IEnumDebugPendingBreakpoints2_FWD_DEFINED__ */


#ifndef __IEnumDebugErrorBreakpoints2_FWD_DEFINED__
#define __IEnumDebugErrorBreakpoints2_FWD_DEFINED__
typedef interface IEnumDebugErrorBreakpoints2 IEnumDebugErrorBreakpoints2;
#endif 	/* __IEnumDebugErrorBreakpoints2_FWD_DEFINED__ */


#ifndef __IEnumDebugMachines2__deprecated_FWD_DEFINED__
#define __IEnumDebugMachines2__deprecated_FWD_DEFINED__
typedef interface IEnumDebugMachines2__deprecated IEnumDebugMachines2__deprecated;
#endif 	/* __IEnumDebugMachines2__deprecated_FWD_DEFINED__ */


#ifndef __IEnumDebugExceptionInfo2_FWD_DEFINED__
#define __IEnumDebugExceptionInfo2_FWD_DEFINED__
typedef interface IEnumDebugExceptionInfo2 IEnumDebugExceptionInfo2;
#endif 	/* __IEnumDebugExceptionInfo2_FWD_DEFINED__ */


#ifndef __IEnumDebugFrameInfo2_FWD_DEFINED__
#define __IEnumDebugFrameInfo2_FWD_DEFINED__
typedef interface IEnumDebugFrameInfo2 IEnumDebugFrameInfo2;
#endif 	/* __IEnumDebugFrameInfo2_FWD_DEFINED__ */


#ifndef __IEnumDebugSessionFrameInfo2_FWD_DEFINED__
#define __IEnumDebugSessionFrameInfo2_FWD_DEFINED__
typedef interface IEnumDebugSessionFrameInfo2 IEnumDebugSessionFrameInfo2;
#endif 	/* __IEnumDebugSessionFrameInfo2_FWD_DEFINED__ */


#ifndef __IEnumDebugFrameInfoFilter2_FWD_DEFINED__
#define __IEnumDebugFrameInfoFilter2_FWD_DEFINED__
typedef interface IEnumDebugFrameInfoFilter2 IEnumDebugFrameInfoFilter2;
#endif 	/* __IEnumDebugFrameInfoFilter2_FWD_DEFINED__ */


#ifndef __IEnumCodePaths2_FWD_DEFINED__
#define __IEnumCodePaths2_FWD_DEFINED__
typedef interface IEnumCodePaths2 IEnumCodePaths2;
#endif 	/* __IEnumCodePaths2_FWD_DEFINED__ */


#ifndef __IEnumDebugModules2_FWD_DEFINED__
#define __IEnumDebugModules2_FWD_DEFINED__
typedef interface IEnumDebugModules2 IEnumDebugModules2;
#endif 	/* __IEnumDebugModules2_FWD_DEFINED__ */


#ifndef __IEnumDebugPortSuppliers2_FWD_DEFINED__
#define __IEnumDebugPortSuppliers2_FWD_DEFINED__
typedef interface IEnumDebugPortSuppliers2 IEnumDebugPortSuppliers2;
#endif 	/* __IEnumDebugPortSuppliers2_FWD_DEFINED__ */


#ifndef __IEnumDebugPorts2_FWD_DEFINED__
#define __IEnumDebugPorts2_FWD_DEFINED__
typedef interface IEnumDebugPorts2 IEnumDebugPorts2;
#endif 	/* __IEnumDebugPorts2_FWD_DEFINED__ */


#ifndef __IEnumDebugPropertyInfo2_FWD_DEFINED__
#define __IEnumDebugPropertyInfo2_FWD_DEFINED__
typedef interface IEnumDebugPropertyInfo2 IEnumDebugPropertyInfo2;
#endif 	/* __IEnumDebugPropertyInfo2_FWD_DEFINED__ */


#ifndef __IEnumDebugReferenceInfo2_FWD_DEFINED__
#define __IEnumDebugReferenceInfo2_FWD_DEFINED__
typedef interface IEnumDebugReferenceInfo2 IEnumDebugReferenceInfo2;
#endif 	/* __IEnumDebugReferenceInfo2_FWD_DEFINED__ */


#ifndef __IDebugProcessQueryProperties_FWD_DEFINED__
#define __IDebugProcessQueryProperties_FWD_DEFINED__
typedef interface IDebugProcessQueryProperties IDebugProcessQueryProperties;
#endif 	/* __IDebugProcessQueryProperties_FWD_DEFINED__ */


#ifndef __IDebugRemoteServer2_FWD_DEFINED__
#define __IDebugRemoteServer2_FWD_DEFINED__
typedef interface IDebugRemoteServer2 IDebugRemoteServer2;
#endif 	/* __IDebugRemoteServer2_FWD_DEFINED__ */


#ifndef __IDebugRemoteServerFactory2_FWD_DEFINED__
#define __IDebugRemoteServerFactory2_FWD_DEFINED__
typedef interface IDebugRemoteServerFactory2 IDebugRemoteServerFactory2;
#endif 	/* __IDebugRemoteServerFactory2_FWD_DEFINED__ */


#ifndef __IDebugProgramPublisher2_FWD_DEFINED__
#define __IDebugProgramPublisher2_FWD_DEFINED__
typedef interface IDebugProgramPublisher2 IDebugProgramPublisher2;
#endif 	/* __IDebugProgramPublisher2_FWD_DEFINED__ */


#ifndef __IDebugProgramProvider2_FWD_DEFINED__
#define __IDebugProgramProvider2_FWD_DEFINED__
typedef interface IDebugProgramProvider2 IDebugProgramProvider2;
#endif 	/* __IDebugProgramProvider2_FWD_DEFINED__ */


#ifndef __IDebugProviderProgramNode2_FWD_DEFINED__
#define __IDebugProviderProgramNode2_FWD_DEFINED__
typedef interface IDebugProviderProgramNode2 IDebugProviderProgramNode2;
#endif 	/* __IDebugProviderProgramNode2_FWD_DEFINED__ */


#ifndef __IDebugFirewallConfigurationCallback2_FWD_DEFINED__
#define __IDebugFirewallConfigurationCallback2_FWD_DEFINED__
typedef interface IDebugFirewallConfigurationCallback2 IDebugFirewallConfigurationCallback2;
#endif 	/* __IDebugFirewallConfigurationCallback2_FWD_DEFINED__ */


#ifndef __IDebugAttachSecurityCallback2_FWD_DEFINED__
#define __IDebugAttachSecurityCallback2_FWD_DEFINED__
typedef interface IDebugAttachSecurityCallback2 IDebugAttachSecurityCallback2;
#endif 	/* __IDebugAttachSecurityCallback2_FWD_DEFINED__ */


#ifndef __SDMServer_FWD_DEFINED__
#define __SDMServer_FWD_DEFINED__

#ifdef __cplusplus
typedef class SDMServer SDMServer;
#else
typedef struct SDMServer SDMServer;
#endif /* __cplusplus */

#endif 	/* __SDMServer_FWD_DEFINED__ */


#ifndef __MsMachineDebugManager_V7_RETAIL_FWD_DEFINED__
#define __MsMachineDebugManager_V7_RETAIL_FWD_DEFINED__

#ifdef __cplusplus
typedef class MsMachineDebugManager_V7_RETAIL MsMachineDebugManager_V7_RETAIL;
#else
typedef struct MsMachineDebugManager_V7_RETAIL MsMachineDebugManager_V7_RETAIL;
#endif /* __cplusplus */

#endif 	/* __MsMachineDebugManager_V7_RETAIL_FWD_DEFINED__ */


#ifndef __MsMachineDebugManager_V7_DEBUG_FWD_DEFINED__
#define __MsMachineDebugManager_V7_DEBUG_FWD_DEFINED__

#ifdef __cplusplus
typedef class MsMachineDebugManager_V7_DEBUG MsMachineDebugManager_V7_DEBUG;
#else
typedef struct MsMachineDebugManager_V7_DEBUG MsMachineDebugManager_V7_DEBUG;
#endif /* __cplusplus */

#endif 	/* __MsMachineDebugManager_V7_DEBUG_FWD_DEFINED__ */


#ifndef __MDMUtilServer_V7_RETAIL_FWD_DEFINED__
#define __MDMUtilServer_V7_RETAIL_FWD_DEFINED__

#ifdef __cplusplus
typedef class MDMUtilServer_V7_RETAIL MDMUtilServer_V7_RETAIL;
#else
typedef struct MDMUtilServer_V7_RETAIL MDMUtilServer_V7_RETAIL;
#endif /* __cplusplus */

#endif 	/* __MDMUtilServer_V7_RETAIL_FWD_DEFINED__ */


#ifndef __MDMUtilServer_V7_DEBUG_FWD_DEFINED__
#define __MDMUtilServer_V7_DEBUG_FWD_DEFINED__

#ifdef __cplusplus
typedef class MDMUtilServer_V7_DEBUG MDMUtilServer_V7_DEBUG;
#else
typedef struct MDMUtilServer_V7_DEBUG MDMUtilServer_V7_DEBUG;
#endif /* __cplusplus */

#endif 	/* __MDMUtilServer_V7_DEBUG_FWD_DEFINED__ */


#ifndef __ProgramPublisher_FWD_DEFINED__
#define __ProgramPublisher_FWD_DEFINED__

#ifdef __cplusplus
typedef class ProgramPublisher ProgramPublisher;
#else
typedef struct ProgramPublisher ProgramPublisher;
#endif /* __cplusplus */

#endif 	/* __ProgramPublisher_FWD_DEFINED__ */


#ifndef __MsProgramProvider_FWD_DEFINED__
#define __MsProgramProvider_FWD_DEFINED__

#ifdef __cplusplus
typedef class MsProgramProvider MsProgramProvider;
#else
typedef struct MsProgramProvider MsProgramProvider;
#endif /* __cplusplus */

#endif 	/* __MsProgramProvider_FWD_DEFINED__ */


/* header files for imported files */
#include "ocidl.h"
#include "enc.h"

#ifdef __cplusplus
extern "C"{
#endif 


/* interface __MIDL_itf_msdbg_0000_0000 */
/* [local] */ 

#ifndef _BASETSD_H_
typedef unsigned long POINTER_64_INT;

#pragma once
typedef signed char INT8;

typedef signed char *PINT8;

typedef short INT16;

typedef short *PINT16;

typedef int INT32;

typedef int *PINT32;

typedef __int64 INT64;

typedef __int64 *PINT64;

typedef unsigned char UINT8;

typedef unsigned char *PUINT8;

typedef unsigned short UINT16;

typedef unsigned short *PUINT16;

typedef unsigned int UINT32;

typedef unsigned int *PUINT32;

typedef unsigned __int64 UINT64;

typedef unsigned __int64 *PUINT64;

typedef int LONG32;

typedef int *PLONG32;

typedef unsigned int ULONG32;

typedef unsigned int *PULONG32;

typedef unsigned int DWORD32;

typedef unsigned int *PDWORD32;

typedef /* [custom][public] */ __int3264 INT_PTR;

typedef /* [public] */ __int3264 *PINT_PTR;

typedef /* [custom][public] */ unsigned __int3264 UINT_PTR;

typedef /* [public] */ unsigned __int3264 *PUINT_PTR;

typedef /* [custom][public] */ __int3264 LONG_PTR;

typedef /* [public] */ __int3264 *PLONG_PTR;

typedef /* [custom][public] */ unsigned __int3264 ULONG_PTR;

typedef /* [public] */ unsigned __int3264 *PULONG_PTR;

typedef unsigned short UHALF_PTR;

typedef unsigned short *PUHALF_PTR;

typedef short HALF_PTR;

typedef short *PHALF_PTR;

typedef long SHANDLE_PTR;

typedef unsigned long HANDLE_PTR;

typedef ULONG_PTR SIZE_T;

typedef ULONG_PTR *PSIZE_T;

typedef LONG_PTR SSIZE_T;

typedef LONG_PTR *PSSIZE_T;

typedef ULONG_PTR DWORD_PTR;

typedef ULONG_PTR *PDWORD_PTR;

typedef __int64 LONG64;

typedef __int64 *PLONG64;

typedef unsigned __int64 ULONG64;

typedef unsigned __int64 *PULONG64;

typedef unsigned __int64 DWORD64;

typedef unsigned __int64 *PDWORD64;

typedef ULONG_PTR KAFFINITY;

typedef KAFFINITY *PKAFFINITY;

#endif // _BASETSD_H_
#define	MSDBG_VERSION	( 7126 )

enum AD7_HRESULT
{
    S_ATTACH_DEFERRED__ = 0x40004,
    S_ATTACH_IGNORED__ = 0x40005,
    S_JIT_USERCANCELLED__ = 0x400B0,
    S_JIT_NOT_REG_FOR_ENGINE__ = 0x400B5,
    S_TERMINATE_PROCESSES_STILL_DETACHING__ = 0x400C0,
    S_TERMINATE_PROCESSES_STILL_TERMINATING__ = 0x400C1,
    S_ENC_SETIP_REQUIRES_CONTINUE__ = 0x40106,
    S_WEBDBG_UNABLE_TO_DIAGNOSE__ = 0x40120,
    S_WEBDBG_DEBUG_VERB_BLOCKED__ = 0x40121,
    S_ASP_USER_ACCESS_DENIED__ = 0x40125,
    S_JMC_LIMITED_SUPPORT__ = 0x40146,
    S_CANNOT_REMAP_IN_EXCEPTION__ = 0x40150,
    S_CANNOT_REMAP_NOT_AT_SEQUENCE_POINT__ = 0x40151,
    S_GETPARENT_NO_PARENT__ = 0x40531,
    S_GETDERIVEDMOST_NO_DERIVED_MOST__ = 0x40541,
    S_GETMEMORYBYTES_NO_MEMORY_BYTES__ = 0x40551,
    S_GETMEMORYCONTEXT_NO_MEMORY_CONTEXT__ = 0x40561,
    S_GETSIZE_NO_SIZE__ = 0x40571,
    S_GETEXTENDEDINFO_NO_EXTENDEDINFO__ = 0x40591,
    S_ASYNC_STOP__ = 0x40B02,
    E_ATTACH_DEBUGGER_ALREADY_ATTACHED__ = 0x80040001,
    E_ATTACH_DEBUGGEE_PROCESS_SECURITY_VIOLATION__ = 0x80040002,
    E_ATTACH_CANNOT_ATTACH_TO_DESKTOP__ = 0x80040003,
    E_LAUNCH_NO_INTEROP__ = 0x80040005,
    E_LAUNCH_DEBUGGING_NOT_POSSIBLE__ = 0x80040006,
    E_LAUNCH_KERNEL_DEBUGGER_ENABLED__ = 0x80040007,
    E_LAUNCH_KERNEL_DEBUGGER_PRESENT__ = 0x80040008,
    E_INTEROP_NOT_SUPPORTED__ = 0x80040009,
    E_TOO_MANY_PROCESSES__ = 0x8004000A,
    E_MSHTML_SCRIPT_DEBUGGING_DISABLED__ = 0x8004000B,
    E_SCRIPT_PDM_NOT_REGISTERED__ = 0x8004000C,
    E_DE_CLR_DBG_SERVICES_NOT_INSTALLED__ = 0x8004000D,
    E_ATTACH_NO_CLR_PROGRAMS__ = 0x8004000E,
    E_REMOTE_SERVER_CLOSED__ = 0x80040010,
    E_CLR_NOT_SUPPORTED__ = 0x80040016,
    E_64BIT_CLR_NOT_SUPPORTED__ = 0x80040017,
    E_CANNOT_MIX_MINDUMP_DEBUGGING__ = 0x80040018,
    E_DEBUG_ENGINE_NOT_REGISTERED__ = 0x80040019,
    E_LAUNCH_SXS_ERROR__ = 0x8004001A,
    E_FAILED_TO_INITIALIZE_SCRIPT_PROXY__ = 0x8004001B,
    E_REMOTE_SERVER_DOES_NOT_EXIST__ = 0x80040020,
    E_REMOTE_SERVER_ACCESS_DENIED__ = 0x80040021,
    E_REMOTE_SERVER_MACHINE_DOES_NOT_EXIST__ = 0x80040022,
    E_DEBUGGER_NOT_REGISTERED_PROPERLY__ = 0x80040023,
    E_FORCE_GUEST_MODE_ENABLED__ = 0x80040024,
    E_GET_IWAM_USER_FAILURE__ = 0x80040025,
    E_REMOTE_SERVER_INVALID_NAME__ = 0x80040026,
    E_REMOTE_SERVER_MACHINE_NO_DEFAULT__ = 0x80040027,
    E_AUTO_LAUNCH_EXEC_FAILURE__ = 0x80040028,
    E_SERVICE_ACCESS_DENIED__ = 0x80040029,
    E_SERVICE_ACCESS_DENIED_ON_CALLBACK__ = 0x8004002A,
    E_REMOTE_COMPONENTS_NOT_REGISTERED__ = 0x8004002B,
    E_DCOM_ACCESS_DENIED__ = 0x8004002C,
    E_SHARE_LEVEL_ACCESS_CONTROL_ENABLED__ = 0x8004002D,
    E_WORKGROUP_REMOTE_LOGON_FAILURE__ = 0x8004002E,
    E_WINAUTH_CONNECT_NOT_SUPPORTED__ = 0x8004002F,
    E_EVALUATE_BUSY_WITH_EVALUATION__ = 0x80040030,
    E_EVALUATE_TIMEOUT__ = 0x80040031,
    E_INTEROP_CLR_TOO_OLD__ = 0x80040032,
    E_CLR_INCOMPATIBLE_PROTOCOL__ = 0x80040033,
    E_CLR_CANNOT_DEBUG_FIBER_PROCESS__ = 0x80040034,
    E_PROCESS_OBJECT_ACCESS_DENIED__ = 0x80040035,
    E_PROCESS_TOKEN_ACCESS_DENIED__ = 0x80040036,
    E_PROCESS_TOKEN_ACCESS_DENIED_NO_TS__ = 0x80040037,
    E_OPERATION_REQUIRES_ELEVATION__ = 0x80040038,
    E_ATTACH_REQUIRES_ELEVATION__ = 0x80040039,
    E_MEMORY_NOTSUPPORTED__ = 0x80040040,
    E_DISASM_NOTSUPPORTED__ = 0x80040041,
    E_DISASM_BADADDRESS__ = 0x80040042,
    E_DISASM_NOTAVAILABLE__ = 0x80040043,
    E_BP_DELETED__ = 0x80040060,
    E_PROCESS_DESTROYED__ = 0x80040070,
    E_PROCESS_DEBUGGER_IS_DEBUGGEE__ = 0x80040071,
    E_TERMINATE_FORBIDDEN__ = 0x80040072,
    E_THREAD_DESTROYED__ = 0x80040075,
    E_PORTSUPPLIER_NO_PORT__ = 0x80040080,
    E_PORT_NO_REQUEST__ = 0x80040090,
    E_COMPARE_CANNOT_COMPARE__ = 0x800400A0,
    E_JIT_INVALID_PID__ = 0x800400B1,
    E_JIT_VSJITDEBUGGER_NOT_REGISTERED__ = 0x800400B3,
    E_JIT_APPID_NOT_REGISTERED__ = 0x800400B4,
    E_JIT_RUNTIME_VERSION_UNSUPPORTED__ = 0x800400B6,
    E_SESSION_TERMINATE_DETACH_FAILED__ = 0x800400C2,
    E_SESSION_TERMINATE_FAILED__ = 0x800400C3,
    E_DETACH_NO_PROXY__ = 0x800400D0,
    E_DETACH_TS_UNSUPPORTED__ = 0x800400E0,
    E_DETACH_IMPERSONATE_FAILURE__ = 0x800400F0,
    E_CANNOT_SET_NEXT_STATEMENT_ON_NONLEAF_FRAME__ = 0x80040100,
    E_TARGET_FILE_MISMATCH__ = 0x80040101,
    E_IMAGE_NOT_LOADED__ = 0x80040102,
    E_FIBER_NOT_SUPPORTED__ = 0x80040103,
    E_CANNOT_SETIP_TO_DIFFERENT_FUNCTION__ = 0x80040104,
    E_CANNOT_SET_NEXT_STATEMENT_ON_EXCEPTION__ = 0x80040105,
    E_ENC_SETIP_REQUIRES_CONTINUE__ = 0x80040107,
    E_CANNOT_SET_NEXT_STATEMENT_INTO_FINALLY__ = 0x80040108,
    E_CANNOT_SET_NEXT_STATEMENT_OUT_OF_FINALLY__ = 0x80040109,
    E_CANNOT_SET_NEXT_STATEMENT_INTO_CATCH__ = 0x8004010A,
    E_CANNOT_SET_NEXT_STATEMENT_GENERAL__ = 0x8004010B,
    E_CANNOT_SET_NEXT_STATEMENT_INTO_OR_OUT_OF_FILTER__ = 0x8004010C,
    E_ASYNCBREAK_NO_PROGRAMS__ = 0x80040110,
    E_ASYNCBREAK_DEBUGGEE_NOT_INITIALIZED__ = 0x80040111,
    E_ASYNCBREAK_UNABLE_TO_PROCESS__ = 0x80040112,
    E_WEBDBG_DEBUG_VERB_BLOCKED__ = 0x80040121,
    E_ASP_USER_ACCESS_DENIED__ = 0x80040125,
    E_AUTO_ATTACH_NOT_REGISTERED__ = 0x80040126,
    E_AUTO_ATTACH_DCOM_ERROR__ = 0x80040127,
    E_AUTO_ATTACH_NOT_SUPPORTED__ = 0x80040128,
    E_AUTO_ATTACH_CLASSNOTREG__ = 0x80040129,
    E_CANNOT_CONTINUE_DURING_PENDING_EXPR_EVAL__ = 0x80040130,
    E_REMOTE_REDIRECTION_UNSUPPORTED__ = 0x80040135,
    E_INVALID_WORKING_DIRECTORY__ = 0x80040136,
    E_LAUNCH_FAILED_WITH_ELEVATION__ = 0x80040137,
    E_LAUNCH_ELEVATION_REQUIRED__ = 0x80040138,
    E_CANNOT_FIND_INTERNET_EXPLORER__ = 0x80040139,
    E_REMOTE_PROCESS_OBJECT_ACCESS_DENIED__ = 0x8004013A,
    E_REMOTE_ATTACH_REQUIRES_ELEVATION__ = 0x8004013B,
    E_REMOTE_LAUNCH_ELEVATION_REQUIRED__ = 0x8004013C,
    E_EXCEPTION_CANNOT_BE_INTERCEPTED__ = 0x80040140,
    E_EXCEPTION_CANNOT_UNWIND_ABOVE_CALLBACK__ = 0x80040141,
    E_INTERCEPT_CURRENT_EXCEPTION_NOT_SUPPORTED__ = 0x80040142,
    E_INTERCEPT_CANNOT_UNWIND_LASTCHANCE_INTEROP__ = 0x80040143,
    E_JMC_CANNOT_SET_STATUS__ = 0x80040145,
    E_DESTROYED__ = 0x80040201,
    E_REMOTE_NOMSVCMON__ = 0x80040202,
    E_REMOTE_BADIPADDRESS__ = 0x80040203,
    E_REMOTE_MACHINEDOWN__ = 0x80040204,
    E_REMOTE_MACHINEUNSPECIFIED__ = 0x80040205,
    E_CRASHDUMP_ACTIVE__ = 0x80040206,
    E_ALL_THREADS_SUSPENDED__ = 0x80040207,
    E_LOAD_DLL_TL__ = 0x80040208,
    E_LOAD_DLL_SH__ = 0x80040209,
    E_LOAD_DLL_EM__ = 0x8004020A,
    E_LOAD_DLL_EE__ = 0x8004020B,
    E_LOAD_DLL_DM__ = 0x8004020C,
    E_LOAD_DLL_MD__ = 0x8004020D,
    E_IOREDIR_BADFILE__ = 0x8004020E,
    E_IOREDIR_BADSYNTAX__ = 0x8004020F,
    E_REMOTE_BADVERSION__ = 0x80040210,
    E_CRASHDUMP_UNSUPPORTED__ = 0x80040211,
    E_REMOTE_BAD_CLR_VERSION__ = 0x80040212,
    E_UNSUPPORTED_BINARY__ = 0x80040215,
    E_DEBUGGEE_BLOCKED__ = 0x80040216,
    E_REMOTE_NOUSERMSVCMON__ = 0x80040217,
    E_STEP_WIN9xSYSCODE__ = 0x80040218,
    E_INTEROP_ORPC_INIT__ = 0x80040219,
    E_CANNOT_DEBUG_WIN32__ = 0x8004021B,
    E_CANNOT_DEBUG_WIN64__ = 0x8004021C,
    E_MINIDUMP_READ_WIN9X__ = 0x8004021D,
    E_CROSS_TSSESSION_ATTACH__ = 0x8004021E,
    E_STEP_BP_SET_FAILED__ = 0x8004021F,
    E_LOAD_DLL_TL_INCORRECT_VERSION__ = 0x80040220,
    E_LOAD_DLL_DM_INCORRECT_VERSION__ = 0x80040221,
    E_REMOTE_NOMSVCMON_PIPE__ = 0x80040222,
    E_LOAD_DLL_DIA__ = 0x80040223,
    E_DUMP_CORRUPTED__ = 0x80040224,
    E_INTEROP_X64__ = 0x80040225,
    E_CRASHDUMP_DEPRECATED__ = 0x80040227,
    E_LAUNCH_MANAGEDONLYMINIDUMP_UNSUPPORTED__ = 0x80040228,
    E_LAUNCH_64BIT_MANAGEDMINIDUMP_UNSUPPORTED__ = 0x80040229,
    E_DEVICEBITS_NOT_SIGNED__ = 0x80040401,
    E_ATTACH_NOT_ENABLED__ = 0x80040402,
    E_REMOTE_DISCONNECT__ = 0x80040403,
    E_BREAK_ALL_FAILED__ = 0x80040404,
    E_DEVICE_ACCESS_DENIED_SELECT_YES__ = 0x80040405,
    E_DEVICE_ACCESS_DENIED__ = 0x80040406,
    E_DEVICE_CONNRESET__ = 0x80040407,
    E_BAD_NETCF_VERSION__ = 0x80040408,
    E_REFERENCE_NOT_VALID__ = 0x80040501,
    E_PROPERTY_NOT_VALID__ = 0x80040511,
    E_SETVALUE_VALUE_CANNOT_BE_SET__ = 0x80040521,
    E_SETVALUE_VALUE_IS_READONLY__ = 0x80040522,
    E_SETVALUEASREFERENCE_NOTSUPPORTED__ = 0x80040523,
    E_CANNOT_GET_UNMANAGED_MEMORY_CONTEXT__ = 0x80040561,
    E_GETREFERENCE_NO_REFERENCE__ = 0x80040581,
    E_CODE_CONTEXT_OUT_OF_SCOPE__ = 0x800405A1,
    E_INVALID_SESSIONID__ = 0x800405A2,
    E_SERVER_UNAVAILABLE_ON_CALLBACK__ = 0x800405A3,
    E_ACCESS_DENIED_ON_CALLBACK__ = 0x800405A4,
    E_UNKNOWN_AUTHN_SERVICE_ON_CALLBACK__ = 0x800405A5,
    E_NO_SESSION_AVAILABLE__ = 0x800405A6,
    E_CLIENT_NOT_LOGGED_ON__ = 0x800405A7,
    E_OTHER_USERS_SESSION__ = 0x800405A8,
    E_USER_LEVEL_ACCESS_CONTROL_REQUIRED__ = 0x800405A9,
    E_KERBEROS_ACCESS_DENIED_ON_CALLBACK__ = 0x800405AA,
    E_DNS_FAILURE_ON_CALLBACK__ = 0x800405AB,
    E_SCRIPT_CLR_EE_DISABLED__ = 0x800405B0,
    E_HTTP_SERVERERROR__ = 0x80040700,
    E_HTTP_UNAUTHORIZED__ = 0x80040701,
    E_HTTP_SENDREQUEST_FAILED__ = 0x80040702,
    E_HTTP_FORBIDDEN__ = 0x80040703,
    E_HTTP_NOT_SUPPORTED__ = 0x80040704,
    E_HTTP_NO_CONTENT__ = 0x80040705,
    E_HTTP_NOT_FOUND__ = 0x80040706,
    E_HTTP_BAD_REQUEST__ = 0x80040707,
    E_HTTP_ACCESS_DENIED__ = 0x80040708,
    E_HTTP_CONNECT_FAILED__ = 0x80040709,
    E_HTTP_EXCEPTION__ = 0x8004070A,
    E_HTTP_TIMEOUT__ = 0x8004070B,
    E_HTTP_SITE_NOT_FOUND__ = 0x8004070C,
    E_HTTP_APP_NOT_FOUND__ = 0x8004070D,
    E_HTTP_MANAGEMENT_API_MISSING__ = 0x8004070E,
    E_HTTP_NO_PROCESS__ = 0x8004070F,
    E_64BIT_COMPONENTS_NOT_INSTALLED__ = 0x80040750,
    E_UNMARSHAL_SERVER_FAILED__ = 0x80040751,
    E_UNMARSHAL_CALLBACK_FAILED__ = 0x80040752,
    E_RPC_REQUIRES_AUTHENTICATION__ = 0x80040755,
    E_LOGON_FAILURE_ON_CALLBACK__ = 0x80040756,
    E_REMOTE_SERVER_UNAVAILABLE__ = 0x80040757,
    E_FIREWALL_USER_CANCELED__ = 0x80040758,
    E_REMOTE_CREDENTIALS_PROHIBITED__ = 0x80040759,
    E_FIREWALL_NO_EXCEPTIONS__ = 0x8004075A,
    E_FIREWALL_CANNOT_OPEN_APPLICATION__ = 0x8004075B,
    E_FIREWALL_CANNOT_OPEN_PORT__ = 0x8004075C,
    E_FIREWALL_CANNOT_OPEN_FILE_SHARING__ = 0x8004075D,
    E_REMOTE_DEBUGGING_UNSUPPORTED__ = 0x8004075E,
    E_REMOTE_BAD_MSDBG2__ = 0x8004075F,
    E_ATTACH_USER_CANCELED__ = 0x80040760,
    E_REMOTE_PACKET_TOO_BIG__ = 0x80040761,
    E_UNSUPPORTED_FUTURE_CLR_VERSION__ = 0x80040762,
    E_UNSUPPORTED_CLR_V1__ = 0x80040763,
    E_INTEROP_IA64__ = 0x80040764,
    E_HTTP_GENERAL__ = 0x80040765,
    E_FUNCTION_NOT_JITTED__ = 0x80040800,
    E_NO_CODE_CONTEXT__ = 0x80040801,
    E_BAD_CLR_DIASYMREADER__ = 0x80040802,
    E_CLR_SHIM_ERROR__ = 0x80040803,
    E_AUTOATTACH_WEBSERVER_NOT_FOUND__ = 0x80040901,
    E_DBGEXTENSION_NOT_FOUND__ = 0x80040910,
    E_DBGEXTENSION_FUNCTION_NOT_FOUND__ = 0x80040911,
    E_DBGEXTENSION_FAULTED__ = 0x80040912,
    E_DBGEXTENSION_RESULT_INVALID__ = 0x80040913,
    E_PROGRAM_IN_RUNMODE__ = 0x80040914,
    E_CAUSALITY_NO_SERVER_RESPONSE__ = 0x80040920,
    E_CAUSALITY_REMOTE_NOT_REGISTERED__ = 0x80040921,
    E_CAUSALITY_BREAKPOINT_NOT_HIT__ = 0x80040922,
    E_CAUSALITY_BREAKPOINT_BIND_ERROR__ = 0x80040923,
    E_CAUSALITY_PROJECT_DISABLED__ = 0x80040924,
    E_NO_ATTACH_WHILE_DDD__ = 0x80040A00,
    E_SQLLE_ACCESSDENIED__ = 0x80040A01,
    E_SQL_SP_ENABLE_PERMISSION_DENIED__ = 0x80040A02,
    E_SQL_DEBUGGING_NOT_ENABLED_ON_SERVER__ = 0x80040A03,
    E_SQL_CANT_FIND_SSDEBUGPS_ON_CLIENT__ = 0x80040A04,
    E_SQL_EXECUTED_BUT_NOT_DEBUGGED__ = 0x80040A05,
    E_SQL_VDT_INIT_RETURNED_SQL_ERROR__ = 0x80040A06,
    E_ATTACH_FAILED_ABORT_SILENTLY__ = 0x80040A07,
    E_SQL_REGISTER_FAILED__ = 0x80040A08,
    E_DE_NOT_SUPPORTED_PRE_8_0__ = 0x80040B00,
    E_PROGRAM_DESTROY_PENDING__ = 0x80040B01,
    E_MANAGED_FEATURE_NOTSUPPORTED__ = 0x80040BAD,
    E_OS_PERSONAL__ = 0x80040C00,
    E_SOURCE_SERVER_DISABLE_PARTIAL_TRUST__ = 0x80040C01,
    E_TRACE_DETACH_UNSUPPORTED__ = 0x80040D00,
    E_THREAD_NOT_FOUND__ = 0x80040D01,
    E_CANNOT_AUTOATTACH_TO_SQLSERVER__ = 0x80040D02,
    E_OBJECT_OUT_OF_SYNC__ = 0x80040D03
};
#define S_ATTACH_DEFERRED _HRESULT_TYPEDEF_(S_ATTACH_DEFERRED__)
#define S_ATTACH_IGNORED _HRESULT_TYPEDEF_(S_ATTACH_IGNORED__)
#define S_JIT_USERCANCELLED _HRESULT_TYPEDEF_(S_JIT_USERCANCELLED__)
#define S_JIT_NOT_REG_FOR_ENGINE _HRESULT_TYPEDEF_(S_JIT_NOT_REG_FOR_ENGINE__)
#define S_TERMINATE_PROCESSES_STILL_DETACHING _HRESULT_TYPEDEF_(S_TERMINATE_PROCESSES_STILL_DETACHING__)
#define S_TERMINATE_PROCESSES_STILL_TERMINATING _HRESULT_TYPEDEF_(S_TERMINATE_PROCESSES_STILL_TERMINATING__)
#define S_ENC_SETIP_REQUIRES_CONTINUE _HRESULT_TYPEDEF_(S_ENC_SETIP_REQUIRES_CONTINUE__)
#define S_WEBDBG_UNABLE_TO_DIAGNOSE _HRESULT_TYPEDEF_(S_WEBDBG_UNABLE_TO_DIAGNOSE__)
#define S_WEBDBG_DEBUG_VERB_BLOCKED _HRESULT_TYPEDEF_(S_WEBDBG_DEBUG_VERB_BLOCKED__)
#define S_ASP_USER_ACCESS_DENIED _HRESULT_TYPEDEF_(S_ASP_USER_ACCESS_DENIED__)
#define S_JMC_LIMITED_SUPPORT _HRESULT_TYPEDEF_(S_JMC_LIMITED_SUPPORT__)
#define S_CANNOT_REMAP_IN_EXCEPTION _HRESULT_TYPEDEF_(S_CANNOT_REMAP_IN_EXCEPTION__)
#define S_CANNOT_REMAP_NOT_AT_SEQUENCE_POINT _HRESULT_TYPEDEF_(S_CANNOT_REMAP_NOT_AT_SEQUENCE_POINT__)
#define S_GETPARENT_NO_PARENT _HRESULT_TYPEDEF_(S_GETPARENT_NO_PARENT__)
#define S_GETDERIVEDMOST_NO_DERIVED_MOST _HRESULT_TYPEDEF_(S_GETDERIVEDMOST_NO_DERIVED_MOST__)
#define S_GETMEMORYBYTES_NO_MEMORY_BYTES _HRESULT_TYPEDEF_(S_GETMEMORYBYTES_NO_MEMORY_BYTES__)
#define S_GETMEMORYCONTEXT_NO_MEMORY_CONTEXT _HRESULT_TYPEDEF_(S_GETMEMORYCONTEXT_NO_MEMORY_CONTEXT__)
#define S_GETSIZE_NO_SIZE _HRESULT_TYPEDEF_(S_GETSIZE_NO_SIZE__)
#define S_GETEXTENDEDINFO_NO_EXTENDEDINFO _HRESULT_TYPEDEF_(S_GETEXTENDEDINFO_NO_EXTENDEDINFO__)
#define S_ASYNC_STOP _HRESULT_TYPEDEF_(S_ASYNC_STOP__)
#define E_ATTACH_DEBUGGER_ALREADY_ATTACHED _HRESULT_TYPEDEF_(E_ATTACH_DEBUGGER_ALREADY_ATTACHED__)
#define E_ATTACH_DEBUGGEE_PROCESS_SECURITY_VIOLATION _HRESULT_TYPEDEF_(E_ATTACH_DEBUGGEE_PROCESS_SECURITY_VIOLATION__)
#define E_ATTACH_CANNOT_ATTACH_TO_DESKTOP _HRESULT_TYPEDEF_(E_ATTACH_CANNOT_ATTACH_TO_DESKTOP__)
#define E_LAUNCH_NO_INTEROP _HRESULT_TYPEDEF_(E_LAUNCH_NO_INTEROP__)
#define E_LAUNCH_DEBUGGING_NOT_POSSIBLE _HRESULT_TYPEDEF_(E_LAUNCH_DEBUGGING_NOT_POSSIBLE__)
#define E_LAUNCH_KERNEL_DEBUGGER_ENABLED _HRESULT_TYPEDEF_(E_LAUNCH_KERNEL_DEBUGGER_ENABLED__)
#define E_LAUNCH_KERNEL_DEBUGGER_PRESENT _HRESULT_TYPEDEF_(E_LAUNCH_KERNEL_DEBUGGER_PRESENT__)
#define E_INTEROP_NOT_SUPPORTED _HRESULT_TYPEDEF_(E_INTEROP_NOT_SUPPORTED__)
#define E_TOO_MANY_PROCESSES _HRESULT_TYPEDEF_(E_TOO_MANY_PROCESSES__)
#define E_MSHTML_SCRIPT_DEBUGGING_DISABLED _HRESULT_TYPEDEF_(E_MSHTML_SCRIPT_DEBUGGING_DISABLED__)
#define E_SCRIPT_PDM_NOT_REGISTERED _HRESULT_TYPEDEF_(E_SCRIPT_PDM_NOT_REGISTERED__)
#define E_DE_CLR_DBG_SERVICES_NOT_INSTALLED _HRESULT_TYPEDEF_(E_DE_CLR_DBG_SERVICES_NOT_INSTALLED__)
#define E_ATTACH_NO_CLR_PROGRAMS _HRESULT_TYPEDEF_(E_ATTACH_NO_CLR_PROGRAMS__)
#define E_REMOTE_SERVER_CLOSED _HRESULT_TYPEDEF_(E_REMOTE_SERVER_CLOSED__)
#define E_CLR_NOT_SUPPORTED _HRESULT_TYPEDEF_(E_CLR_NOT_SUPPORTED__)
#define E_64BIT_CLR_NOT_SUPPORTED _HRESULT_TYPEDEF_(E_64BIT_CLR_NOT_SUPPORTED__)
#define E_CANNOT_MIX_MINDUMP_DEBUGGING _HRESULT_TYPEDEF_(E_CANNOT_MIX_MINDUMP_DEBUGGING__)
#define E_DEBUG_ENGINE_NOT_REGISTERED _HRESULT_TYPEDEF_(E_DEBUG_ENGINE_NOT_REGISTERED__)
#define E_LAUNCH_SXS_ERROR _HRESULT_TYPEDEF_(E_LAUNCH_SXS_ERROR__)
#define E_FAILED_TO_INITIALIZE_SCRIPT_PROXY _HRESULT_TYPEDEF_(E_FAILED_TO_INITIALIZE_SCRIPT_PROXY__)
#define E_REMOTE_SERVER_DOES_NOT_EXIST _HRESULT_TYPEDEF_(E_REMOTE_SERVER_DOES_NOT_EXIST__)
#define E_REMOTE_SERVER_ACCESS_DENIED _HRESULT_TYPEDEF_(E_REMOTE_SERVER_ACCESS_DENIED__)
#define E_REMOTE_SERVER_MACHINE_DOES_NOT_EXIST _HRESULT_TYPEDEF_(E_REMOTE_SERVER_MACHINE_DOES_NOT_EXIST__)
#define E_DEBUGGER_NOT_REGISTERED_PROPERLY _HRESULT_TYPEDEF_(E_DEBUGGER_NOT_REGISTERED_PROPERLY__)
#define E_FORCE_GUEST_MODE_ENABLED _HRESULT_TYPEDEF_(E_FORCE_GUEST_MODE_ENABLED__)
#define E_GET_IWAM_USER_FAILURE _HRESULT_TYPEDEF_(E_GET_IWAM_USER_FAILURE__)
#define E_REMOTE_SERVER_INVALID_NAME _HRESULT_TYPEDEF_(E_REMOTE_SERVER_INVALID_NAME__)
#define E_REMOTE_SERVER_MACHINE_NO_DEFAULT _HRESULT_TYPEDEF_(E_REMOTE_SERVER_MACHINE_NO_DEFAULT__)
#define E_AUTO_LAUNCH_EXEC_FAILURE _HRESULT_TYPEDEF_(E_AUTO_LAUNCH_EXEC_FAILURE__)
#define E_SERVICE_ACCESS_DENIED _HRESULT_TYPEDEF_(E_SERVICE_ACCESS_DENIED__)
#define E_SERVICE_ACCESS_DENIED_ON_CALLBACK _HRESULT_TYPEDEF_(E_SERVICE_ACCESS_DENIED_ON_CALLBACK__)
#define E_REMOTE_COMPONENTS_NOT_REGISTERED _HRESULT_TYPEDEF_(E_REMOTE_COMPONENTS_NOT_REGISTERED__)
#define E_DCOM_ACCESS_DENIED _HRESULT_TYPEDEF_(E_DCOM_ACCESS_DENIED__)
#define E_SHARE_LEVEL_ACCESS_CONTROL_ENABLED _HRESULT_TYPEDEF_(E_SHARE_LEVEL_ACCESS_CONTROL_ENABLED__)
#define E_WORKGROUP_REMOTE_LOGON_FAILURE _HRESULT_TYPEDEF_(E_WORKGROUP_REMOTE_LOGON_FAILURE__)
#define E_WINAUTH_CONNECT_NOT_SUPPORTED _HRESULT_TYPEDEF_(E_WINAUTH_CONNECT_NOT_SUPPORTED__)
#define E_EVALUATE_BUSY_WITH_EVALUATION _HRESULT_TYPEDEF_(E_EVALUATE_BUSY_WITH_EVALUATION__)
#define E_EVALUATE_TIMEOUT _HRESULT_TYPEDEF_(E_EVALUATE_TIMEOUT__)
#define E_INTEROP_CLR_TOO_OLD _HRESULT_TYPEDEF_(E_INTEROP_CLR_TOO_OLD__)
#define E_CLR_INCOMPATIBLE_PROTOCOL _HRESULT_TYPEDEF_(E_CLR_INCOMPATIBLE_PROTOCOL__)
#define E_CLR_CANNOT_DEBUG_FIBER_PROCESS _HRESULT_TYPEDEF_(E_CLR_CANNOT_DEBUG_FIBER_PROCESS__)
#define E_PROCESS_OBJECT_ACCESS_DENIED _HRESULT_TYPEDEF_(E_PROCESS_OBJECT_ACCESS_DENIED__)
#define E_PROCESS_TOKEN_ACCESS_DENIED _HRESULT_TYPEDEF_(E_PROCESS_TOKEN_ACCESS_DENIED__)
#define E_PROCESS_TOKEN_ACCESS_DENIED_NO_TS _HRESULT_TYPEDEF_(E_PROCESS_TOKEN_ACCESS_DENIED_NO_TS__)
#define E_OPERATION_REQUIRES_ELEVATION _HRESULT_TYPEDEF_(E_OPERATION_REQUIRES_ELEVATION__)
#define E_ATTACH_REQUIRES_ELEVATION _HRESULT_TYPEDEF_(E_ATTACH_REQUIRES_ELEVATION__)
#define E_MEMORY_NOTSUPPORTED _HRESULT_TYPEDEF_(E_MEMORY_NOTSUPPORTED__)
#define E_DISASM_NOTSUPPORTED _HRESULT_TYPEDEF_(E_DISASM_NOTSUPPORTED__)
#define E_DISASM_BADADDRESS _HRESULT_TYPEDEF_(E_DISASM_BADADDRESS__)
#define E_DISASM_NOTAVAILABLE _HRESULT_TYPEDEF_(E_DISASM_NOTAVAILABLE__)
#define E_BP_DELETED _HRESULT_TYPEDEF_(E_BP_DELETED__)
#define E_PROCESS_DESTROYED _HRESULT_TYPEDEF_(E_PROCESS_DESTROYED__)
#define E_PROCESS_DEBUGGER_IS_DEBUGGEE _HRESULT_TYPEDEF_(E_PROCESS_DEBUGGER_IS_DEBUGGEE__)
#define E_TERMINATE_FORBIDDEN _HRESULT_TYPEDEF_(E_TERMINATE_FORBIDDEN__)
#define E_THREAD_DESTROYED _HRESULT_TYPEDEF_(E_THREAD_DESTROYED__)
#define E_PORTSUPPLIER_NO_PORT _HRESULT_TYPEDEF_(E_PORTSUPPLIER_NO_PORT__)
#define E_PORT_NO_REQUEST _HRESULT_TYPEDEF_(E_PORT_NO_REQUEST__)
#define E_COMPARE_CANNOT_COMPARE _HRESULT_TYPEDEF_(E_COMPARE_CANNOT_COMPARE__)
#define E_JIT_INVALID_PID _HRESULT_TYPEDEF_(E_JIT_INVALID_PID__)
#define E_JIT_VSJITDEBUGGER_NOT_REGISTERED _HRESULT_TYPEDEF_(E_JIT_VSJITDEBUGGER_NOT_REGISTERED__)
#define E_JIT_APPID_NOT_REGISTERED _HRESULT_TYPEDEF_(E_JIT_APPID_NOT_REGISTERED__)
#define E_JIT_RUNTIME_VERSION_UNSUPPORTED _HRESULT_TYPEDEF_(E_JIT_RUNTIME_VERSION_UNSUPPORTED__)
#define E_SESSION_TERMINATE_DETACH_FAILED _HRESULT_TYPEDEF_(E_SESSION_TERMINATE_DETACH_FAILED__)
#define E_SESSION_TERMINATE_FAILED _HRESULT_TYPEDEF_(E_SESSION_TERMINATE_FAILED__)
#define E_DETACH_NO_PROXY _HRESULT_TYPEDEF_(E_DETACH_NO_PROXY__)
#define E_DETACH_TS_UNSUPPORTED _HRESULT_TYPEDEF_(E_DETACH_TS_UNSUPPORTED__)
#define E_DETACH_IMPERSONATE_FAILURE _HRESULT_TYPEDEF_(E_DETACH_IMPERSONATE_FAILURE__)
#define E_CANNOT_SET_NEXT_STATEMENT_ON_NONLEAF_FRAME _HRESULT_TYPEDEF_(E_CANNOT_SET_NEXT_STATEMENT_ON_NONLEAF_FRAME__)
#define E_TARGET_FILE_MISMATCH _HRESULT_TYPEDEF_(E_TARGET_FILE_MISMATCH__)
#define E_IMAGE_NOT_LOADED _HRESULT_TYPEDEF_(E_IMAGE_NOT_LOADED__)
#define E_FIBER_NOT_SUPPORTED _HRESULT_TYPEDEF_(E_FIBER_NOT_SUPPORTED__)
#define E_CANNOT_SETIP_TO_DIFFERENT_FUNCTION _HRESULT_TYPEDEF_(E_CANNOT_SETIP_TO_DIFFERENT_FUNCTION__)
#define E_CANNOT_SET_NEXT_STATEMENT_ON_EXCEPTION _HRESULT_TYPEDEF_(E_CANNOT_SET_NEXT_STATEMENT_ON_EXCEPTION__)
#define E_ENC_SETIP_REQUIRES_CONTINUE _HRESULT_TYPEDEF_(E_ENC_SETIP_REQUIRES_CONTINUE__)
#define E_CANNOT_SET_NEXT_STATEMENT_INTO_FINALLY _HRESULT_TYPEDEF_(E_CANNOT_SET_NEXT_STATEMENT_INTO_FINALLY__)
#define E_CANNOT_SET_NEXT_STATEMENT_OUT_OF_FINALLY _HRESULT_TYPEDEF_(E_CANNOT_SET_NEXT_STATEMENT_OUT_OF_FINALLY__)
#define E_CANNOT_SET_NEXT_STATEMENT_INTO_CATCH _HRESULT_TYPEDEF_(E_CANNOT_SET_NEXT_STATEMENT_INTO_CATCH__)
#define E_CANNOT_SET_NEXT_STATEMENT_GENERAL _HRESULT_TYPEDEF_(E_CANNOT_SET_NEXT_STATEMENT_GENERAL__)
#define E_CANNOT_SET_NEXT_STATEMENT_INTO_OR_OUT_OF_FILTER _HRESULT_TYPEDEF_(E_CANNOT_SET_NEXT_STATEMENT_INTO_OR_OUT_OF_FILTER__)
#define E_ASYNCBREAK_NO_PROGRAMS _HRESULT_TYPEDEF_(E_ASYNCBREAK_NO_PROGRAMS__)
#define E_ASYNCBREAK_DEBUGGEE_NOT_INITIALIZED _HRESULT_TYPEDEF_(E_ASYNCBREAK_DEBUGGEE_NOT_INITIALIZED__)
#define E_ASYNCBREAK_UNABLE_TO_PROCESS _HRESULT_TYPEDEF_(E_ASYNCBREAK_UNABLE_TO_PROCESS__)
#define E_WEBDBG_DEBUG_VERB_BLOCKED _HRESULT_TYPEDEF_(E_WEBDBG_DEBUG_VERB_BLOCKED__)
#define E_ASP_USER_ACCESS_DENIED _HRESULT_TYPEDEF_(E_ASP_USER_ACCESS_DENIED__)
#define E_AUTO_ATTACH_NOT_REGISTERED _HRESULT_TYPEDEF_(E_AUTO_ATTACH_NOT_REGISTERED__)
#define E_AUTO_ATTACH_DCOM_ERROR _HRESULT_TYPEDEF_(E_AUTO_ATTACH_DCOM_ERROR__)
#define E_AUTO_ATTACH_NOT_SUPPORTED _HRESULT_TYPEDEF_(E_AUTO_ATTACH_NOT_SUPPORTED__)
#define E_AUTO_ATTACH_CLASSNOTREG _HRESULT_TYPEDEF_(E_AUTO_ATTACH_CLASSNOTREG__)
#define E_CANNOT_CONTINUE_DURING_PENDING_EXPR_EVAL _HRESULT_TYPEDEF_(E_CANNOT_CONTINUE_DURING_PENDING_EXPR_EVAL__)
#define E_REMOTE_REDIRECTION_UNSUPPORTED _HRESULT_TYPEDEF_(E_REMOTE_REDIRECTION_UNSUPPORTED__)
#define E_INVALID_WORKING_DIRECTORY _HRESULT_TYPEDEF_(E_INVALID_WORKING_DIRECTORY__)
#define E_LAUNCH_FAILED_WITH_ELEVATION _HRESULT_TYPEDEF_(E_LAUNCH_FAILED_WITH_ELEVATION__)
#define E_LAUNCH_ELEVATION_REQUIRED _HRESULT_TYPEDEF_(E_LAUNCH_ELEVATION_REQUIRED__)
#define E_CANNOT_FIND_INTERNET_EXPLORER _HRESULT_TYPEDEF_(E_CANNOT_FIND_INTERNET_EXPLORER__)
#define E_REMOTE_PROCESS_OBJECT_ACCESS_DENIED _HRESULT_TYPEDEF_(E_REMOTE_PROCESS_OBJECT_ACCESS_DENIED__)
#define E_REMOTE_ATTACH_REQUIRES_ELEVATION _HRESULT_TYPEDEF_(E_REMOTE_ATTACH_REQUIRES_ELEVATION__)
#define E_REMOTE_LAUNCH_ELEVATION_REQUIRED _HRESULT_TYPEDEF_(E_REMOTE_LAUNCH_ELEVATION_REQUIRED__)
#define E_EXCEPTION_CANNOT_BE_INTERCEPTED _HRESULT_TYPEDEF_(E_EXCEPTION_CANNOT_BE_INTERCEPTED__)
#define E_EXCEPTION_CANNOT_UNWIND_ABOVE_CALLBACK _HRESULT_TYPEDEF_(E_EXCEPTION_CANNOT_UNWIND_ABOVE_CALLBACK__)
#define E_INTERCEPT_CURRENT_EXCEPTION_NOT_SUPPORTED _HRESULT_TYPEDEF_(E_INTERCEPT_CURRENT_EXCEPTION_NOT_SUPPORTED__)
#define E_INTERCEPT_CANNOT_UNWIND_LASTCHANCE_INTEROP _HRESULT_TYPEDEF_(E_INTERCEPT_CANNOT_UNWIND_LASTCHANCE_INTEROP__)
#define E_JMC_CANNOT_SET_STATUS _HRESULT_TYPEDEF_(E_JMC_CANNOT_SET_STATUS__)
#define E_DESTROYED _HRESULT_TYPEDEF_(E_DESTROYED__)
#define E_REMOTE_NOMSVCMON _HRESULT_TYPEDEF_(E_REMOTE_NOMSVCMON__)
#define E_REMOTE_BADIPADDRESS _HRESULT_TYPEDEF_(E_REMOTE_BADIPADDRESS__)
#define E_REMOTE_MACHINEDOWN _HRESULT_TYPEDEF_(E_REMOTE_MACHINEDOWN__)
#define E_REMOTE_MACHINEUNSPECIFIED _HRESULT_TYPEDEF_(E_REMOTE_MACHINEUNSPECIFIED__)
#define E_CRASHDUMP_ACTIVE _HRESULT_TYPEDEF_(E_CRASHDUMP_ACTIVE__)
#define E_ALL_THREADS_SUSPENDED _HRESULT_TYPEDEF_(E_ALL_THREADS_SUSPENDED__)
#define E_LOAD_DLL_TL _HRESULT_TYPEDEF_(E_LOAD_DLL_TL__)
#define E_LOAD_DLL_SH _HRESULT_TYPEDEF_(E_LOAD_DLL_SH__)
#define E_LOAD_DLL_EM _HRESULT_TYPEDEF_(E_LOAD_DLL_EM__)
#define E_LOAD_DLL_EE _HRESULT_TYPEDEF_(E_LOAD_DLL_EE__)
#define E_LOAD_DLL_DM _HRESULT_TYPEDEF_(E_LOAD_DLL_DM__)
#define E_LOAD_DLL_MD _HRESULT_TYPEDEF_(E_LOAD_DLL_MD__)
#define E_IOREDIR_BADFILE _HRESULT_TYPEDEF_(E_IOREDIR_BADFILE__)
#define E_IOREDIR_BADSYNTAX _HRESULT_TYPEDEF_(E_IOREDIR_BADSYNTAX__)
#define E_REMOTE_BADVERSION _HRESULT_TYPEDEF_(E_REMOTE_BADVERSION__)
#define E_CRASHDUMP_UNSUPPORTED _HRESULT_TYPEDEF_(E_CRASHDUMP_UNSUPPORTED__)
#define E_REMOTE_BAD_CLR_VERSION _HRESULT_TYPEDEF_(E_REMOTE_BAD_CLR_VERSION__)
#define E_UNSUPPORTED_BINARY _HRESULT_TYPEDEF_(E_UNSUPPORTED_BINARY__)
#define E_DEBUGGEE_BLOCKED _HRESULT_TYPEDEF_(E_DEBUGGEE_BLOCKED__)
#define E_REMOTE_NOUSERMSVCMON _HRESULT_TYPEDEF_(E_REMOTE_NOUSERMSVCMON__)
#define E_STEP_WIN9xSYSCODE _HRESULT_TYPEDEF_(E_STEP_WIN9xSYSCODE__)
#define E_INTEROP_ORPC_INIT _HRESULT_TYPEDEF_(E_INTEROP_ORPC_INIT__)
#define E_CANNOT_DEBUG_WIN32 _HRESULT_TYPEDEF_(E_CANNOT_DEBUG_WIN32__)
#define E_CANNOT_DEBUG_WIN64 _HRESULT_TYPEDEF_(E_CANNOT_DEBUG_WIN64__)
#define E_MINIDUMP_READ_WIN9X _HRESULT_TYPEDEF_(E_MINIDUMP_READ_WIN9X__)
#define E_CROSS_TSSESSION_ATTACH _HRESULT_TYPEDEF_(E_CROSS_TSSESSION_ATTACH__)
#define E_STEP_BP_SET_FAILED _HRESULT_TYPEDEF_(E_STEP_BP_SET_FAILED__)
#define E_LOAD_DLL_TL_INCORRECT_VERSION _HRESULT_TYPEDEF_(E_LOAD_DLL_TL_INCORRECT_VERSION__)
#define E_LOAD_DLL_DM_INCORRECT_VERSION _HRESULT_TYPEDEF_(E_LOAD_DLL_DM_INCORRECT_VERSION__)
#define E_REMOTE_NOMSVCMON_PIPE _HRESULT_TYPEDEF_(E_REMOTE_NOMSVCMON_PIPE__)
#define E_LOAD_DLL_DIA _HRESULT_TYPEDEF_(E_LOAD_DLL_DIA__)
#define E_DUMP_CORRUPTED _HRESULT_TYPEDEF_(E_DUMP_CORRUPTED__)
#define E_INTEROP_X64 _HRESULT_TYPEDEF_(E_INTEROP_X64__)
#define E_CRASHDUMP_DEPRECATED _HRESULT_TYPEDEF_(E_CRASHDUMP_DEPRECATED__)
#define E_LAUNCH_MANAGEDONLYMINIDUMP_UNSUPPORTED _HRESULT_TYPEDEF_(E_LAUNCH_MANAGEDONLYMINIDUMP_UNSUPPORTED__)
#define E_LAUNCH_64BIT_MANAGEDMINIDUMP_UNSUPPORTED _HRESULT_TYPEDEF_(E_LAUNCH_64BIT_MANAGEDMINIDUMP_UNSUPPORTED__)
#define E_DEVICEBITS_NOT_SIGNED _HRESULT_TYPEDEF_(E_DEVICEBITS_NOT_SIGNED__)
#define E_ATTACH_NOT_ENABLED _HRESULT_TYPEDEF_(E_ATTACH_NOT_ENABLED__)
#define E_REMOTE_DISCONNECT _HRESULT_TYPEDEF_(E_REMOTE_DISCONNECT__)
#define E_BREAK_ALL_FAILED _HRESULT_TYPEDEF_(E_BREAK_ALL_FAILED__)
#define E_DEVICE_ACCESS_DENIED_SELECT_YES _HRESULT_TYPEDEF_(E_DEVICE_ACCESS_DENIED_SELECT_YES__)
#define E_DEVICE_ACCESS_DENIED _HRESULT_TYPEDEF_(E_DEVICE_ACCESS_DENIED__)
#define E_DEVICE_CONNRESET _HRESULT_TYPEDEF_(E_DEVICE_CONNRESET__)
#define E_BAD_NETCF_VERSION _HRESULT_TYPEDEF_(E_BAD_NETCF_VERSION__)
#define E_REFERENCE_NOT_VALID _HRESULT_TYPEDEF_(E_REFERENCE_NOT_VALID__)
#define E_PROPERTY_NOT_VALID _HRESULT_TYPEDEF_(E_PROPERTY_NOT_VALID__)
#define E_SETVALUE_VALUE_CANNOT_BE_SET _HRESULT_TYPEDEF_(E_SETVALUE_VALUE_CANNOT_BE_SET__)
#define E_SETVALUE_VALUE_IS_READONLY _HRESULT_TYPEDEF_(E_SETVALUE_VALUE_IS_READONLY__)
#define E_SETVALUEASREFERENCE_NOTSUPPORTED _HRESULT_TYPEDEF_(E_SETVALUEASREFERENCE_NOTSUPPORTED__)
#define E_CANNOT_GET_UNMANAGED_MEMORY_CONTEXT _HRESULT_TYPEDEF_(E_CANNOT_GET_UNMANAGED_MEMORY_CONTEXT__)
#define E_GETREFERENCE_NO_REFERENCE _HRESULT_TYPEDEF_(E_GETREFERENCE_NO_REFERENCE__)
#define E_CODE_CONTEXT_OUT_OF_SCOPE _HRESULT_TYPEDEF_(E_CODE_CONTEXT_OUT_OF_SCOPE__)
#define E_INVALID_SESSIONID _HRESULT_TYPEDEF_(E_INVALID_SESSIONID__)
#define E_SERVER_UNAVAILABLE_ON_CALLBACK _HRESULT_TYPEDEF_(E_SERVER_UNAVAILABLE_ON_CALLBACK__)
#define E_ACCESS_DENIED_ON_CALLBACK _HRESULT_TYPEDEF_(E_ACCESS_DENIED_ON_CALLBACK__)
#define E_UNKNOWN_AUTHN_SERVICE_ON_CALLBACK _HRESULT_TYPEDEF_(E_UNKNOWN_AUTHN_SERVICE_ON_CALLBACK__)
#define E_NO_SESSION_AVAILABLE _HRESULT_TYPEDEF_(E_NO_SESSION_AVAILABLE__)
#define E_CLIENT_NOT_LOGGED_ON _HRESULT_TYPEDEF_(E_CLIENT_NOT_LOGGED_ON__)
#define E_OTHER_USERS_SESSION _HRESULT_TYPEDEF_(E_OTHER_USERS_SESSION__)
#define E_USER_LEVEL_ACCESS_CONTROL_REQUIRED _HRESULT_TYPEDEF_(E_USER_LEVEL_ACCESS_CONTROL_REQUIRED__)
#define E_KERBEROS_ACCESS_DENIED_ON_CALLBACK _HRESULT_TYPEDEF_(E_KERBEROS_ACCESS_DENIED_ON_CALLBACK__)
#define E_DNS_FAILURE_ON_CALLBACK _HRESULT_TYPEDEF_(E_DNS_FAILURE_ON_CALLBACK__)
#define E_SCRIPT_CLR_EE_DISABLED _HRESULT_TYPEDEF_(E_SCRIPT_CLR_EE_DISABLED__)
#define E_HTTP_SERVERERROR _HRESULT_TYPEDEF_(E_HTTP_SERVERERROR__)
#define E_HTTP_UNAUTHORIZED _HRESULT_TYPEDEF_(E_HTTP_UNAUTHORIZED__)
#define E_HTTP_SENDREQUEST_FAILED _HRESULT_TYPEDEF_(E_HTTP_SENDREQUEST_FAILED__)
#define E_HTTP_FORBIDDEN _HRESULT_TYPEDEF_(E_HTTP_FORBIDDEN__)
#define E_HTTP_NOT_SUPPORTED _HRESULT_TYPEDEF_(E_HTTP_NOT_SUPPORTED__)
#define E_HTTP_NO_CONTENT _HRESULT_TYPEDEF_(E_HTTP_NO_CONTENT__)
#define E_HTTP_NOT_FOUND _HRESULT_TYPEDEF_(E_HTTP_NOT_FOUND__)
#define E_HTTP_BAD_REQUEST _HRESULT_TYPEDEF_(E_HTTP_BAD_REQUEST__)
#define E_HTTP_ACCESS_DENIED _HRESULT_TYPEDEF_(E_HTTP_ACCESS_DENIED__)
#define E_HTTP_CONNECT_FAILED _HRESULT_TYPEDEF_(E_HTTP_CONNECT_FAILED__)
#define E_HTTP_EXCEPTION _HRESULT_TYPEDEF_(E_HTTP_EXCEPTION__)
#define E_HTTP_TIMEOUT _HRESULT_TYPEDEF_(E_HTTP_TIMEOUT__)
#define E_HTTP_SITE_NOT_FOUND _HRESULT_TYPEDEF_(E_HTTP_SITE_NOT_FOUND__)
#define E_HTTP_APP_NOT_FOUND _HRESULT_TYPEDEF_(E_HTTP_APP_NOT_FOUND__)
#define E_HTTP_MANAGEMENT_API_MISSING _HRESULT_TYPEDEF_(E_HTTP_MANAGEMENT_API_MISSING__)
#define E_HTTP_NO_PROCESS _HRESULT_TYPEDEF_(E_HTTP_NO_PROCESS__)
#define E_64BIT_COMPONENTS_NOT_INSTALLED _HRESULT_TYPEDEF_(E_64BIT_COMPONENTS_NOT_INSTALLED__)
#define E_UNMARSHAL_SERVER_FAILED _HRESULT_TYPEDEF_(E_UNMARSHAL_SERVER_FAILED__)
#define E_UNMARSHAL_CALLBACK_FAILED _HRESULT_TYPEDEF_(E_UNMARSHAL_CALLBACK_FAILED__)
#define E_RPC_REQUIRES_AUTHENTICATION _HRESULT_TYPEDEF_(E_RPC_REQUIRES_AUTHENTICATION__)
#define E_LOGON_FAILURE_ON_CALLBACK _HRESULT_TYPEDEF_(E_LOGON_FAILURE_ON_CALLBACK__)
#define E_REMOTE_SERVER_UNAVAILABLE _HRESULT_TYPEDEF_(E_REMOTE_SERVER_UNAVAILABLE__)
#define E_FIREWALL_USER_CANCELED _HRESULT_TYPEDEF_(E_FIREWALL_USER_CANCELED__)
#define E_REMOTE_CREDENTIALS_PROHIBITED _HRESULT_TYPEDEF_(E_REMOTE_CREDENTIALS_PROHIBITED__)
#define E_FIREWALL_NO_EXCEPTIONS _HRESULT_TYPEDEF_(E_FIREWALL_NO_EXCEPTIONS__)
#define E_FIREWALL_CANNOT_OPEN_APPLICATION _HRESULT_TYPEDEF_(E_FIREWALL_CANNOT_OPEN_APPLICATION__)
#define E_FIREWALL_CANNOT_OPEN_PORT _HRESULT_TYPEDEF_(E_FIREWALL_CANNOT_OPEN_PORT__)
#define E_FIREWALL_CANNOT_OPEN_FILE_SHARING _HRESULT_TYPEDEF_(E_FIREWALL_CANNOT_OPEN_FILE_SHARING__)
#define E_REMOTE_DEBUGGING_UNSUPPORTED _HRESULT_TYPEDEF_(E_REMOTE_DEBUGGING_UNSUPPORTED__)
#define E_REMOTE_BAD_MSDBG2 _HRESULT_TYPEDEF_(E_REMOTE_BAD_MSDBG2__)
#define E_ATTACH_USER_CANCELED _HRESULT_TYPEDEF_(E_ATTACH_USER_CANCELED__)
#define E_REMOTE_PACKET_TOO_BIG _HRESULT_TYPEDEF_(E_REMOTE_PACKET_TOO_BIG__)
#define E_UNSUPPORTED_FUTURE_CLR_VERSION _HRESULT_TYPEDEF_(E_UNSUPPORTED_FUTURE_CLR_VERSION__)
#define E_UNSUPPORTED_CLR_V1 _HRESULT_TYPEDEF_(E_UNSUPPORTED_CLR_V1__)
#define E_INTEROP_IA64 _HRESULT_TYPEDEF_(E_INTEROP_IA64__)
#define E_HTTP_GENERAL _HRESULT_TYPEDEF_(E_HTTP_GENERAL__)
#define E_FUNCTION_NOT_JITTED _HRESULT_TYPEDEF_(E_FUNCTION_NOT_JITTED__)
#define E_NO_CODE_CONTEXT _HRESULT_TYPEDEF_(E_NO_CODE_CONTEXT__)
#define E_BAD_CLR_DIASYMREADER _HRESULT_TYPEDEF_(E_BAD_CLR_DIASYMREADER__)
#define E_CLR_SHIM_ERROR _HRESULT_TYPEDEF_(E_CLR_SHIM_ERROR__)
#define E_AUTOATTACH_WEBSERVER_NOT_FOUND _HRESULT_TYPEDEF_(E_AUTOATTACH_WEBSERVER_NOT_FOUND__)
#define E_DBGEXTENSION_NOT_FOUND _HRESULT_TYPEDEF_(E_DBGEXTENSION_NOT_FOUND__)
#define E_DBGEXTENSION_FUNCTION_NOT_FOUND _HRESULT_TYPEDEF_(E_DBGEXTENSION_FUNCTION_NOT_FOUND__)
#define E_DBGEXTENSION_FAULTED _HRESULT_TYPEDEF_(E_DBGEXTENSION_FAULTED__)
#define E_DBGEXTENSION_RESULT_INVALID _HRESULT_TYPEDEF_(E_DBGEXTENSION_RESULT_INVALID__)
#define E_PROGRAM_IN_RUNMODE _HRESULT_TYPEDEF_(E_PROGRAM_IN_RUNMODE__)
#define E_CAUSALITY_NO_SERVER_RESPONSE _HRESULT_TYPEDEF_(E_CAUSALITY_NO_SERVER_RESPONSE__)
#define E_CAUSALITY_REMOTE_NOT_REGISTERED _HRESULT_TYPEDEF_(E_CAUSALITY_REMOTE_NOT_REGISTERED__)
#define E_CAUSALITY_BREAKPOINT_NOT_HIT _HRESULT_TYPEDEF_(E_CAUSALITY_BREAKPOINT_NOT_HIT__)
#define E_CAUSALITY_BREAKPOINT_BIND_ERROR _HRESULT_TYPEDEF_(E_CAUSALITY_BREAKPOINT_BIND_ERROR__)
#define E_CAUSALITY_PROJECT_DISABLED _HRESULT_TYPEDEF_(E_CAUSALITY_PROJECT_DISABLED__)
#define E_NO_ATTACH_WHILE_DDD _HRESULT_TYPEDEF_(E_NO_ATTACH_WHILE_DDD__)
#define E_SQLLE_ACCESSDENIED _HRESULT_TYPEDEF_(E_SQLLE_ACCESSDENIED__)
#define E_SQL_SP_ENABLE_PERMISSION_DENIED _HRESULT_TYPEDEF_(E_SQL_SP_ENABLE_PERMISSION_DENIED__)
#define E_SQL_DEBUGGING_NOT_ENABLED_ON_SERVER _HRESULT_TYPEDEF_(E_SQL_DEBUGGING_NOT_ENABLED_ON_SERVER__)
#define E_SQL_CANT_FIND_SSDEBUGPS_ON_CLIENT _HRESULT_TYPEDEF_(E_SQL_CANT_FIND_SSDEBUGPS_ON_CLIENT__)
#define E_SQL_EXECUTED_BUT_NOT_DEBUGGED _HRESULT_TYPEDEF_(E_SQL_EXECUTED_BUT_NOT_DEBUGGED__)
#define E_SQL_VDT_INIT_RETURNED_SQL_ERROR _HRESULT_TYPEDEF_(E_SQL_VDT_INIT_RETURNED_SQL_ERROR__)
#define E_ATTACH_FAILED_ABORT_SILENTLY _HRESULT_TYPEDEF_(E_ATTACH_FAILED_ABORT_SILENTLY__)
#define E_SQL_REGISTER_FAILED _HRESULT_TYPEDEF_(E_SQL_REGISTER_FAILED__)
#define E_DE_NOT_SUPPORTED_PRE_8_0 _HRESULT_TYPEDEF_(E_DE_NOT_SUPPORTED_PRE_8_0__)
#define E_PROGRAM_DESTROY_PENDING _HRESULT_TYPEDEF_(E_PROGRAM_DESTROY_PENDING__)
#define E_MANAGED_FEATURE_NOTSUPPORTED _HRESULT_TYPEDEF_(E_MANAGED_FEATURE_NOTSUPPORTED__)
#define E_OS_PERSONAL _HRESULT_TYPEDEF_(E_OS_PERSONAL__)
#define E_SOURCE_SERVER_DISABLE_PARTIAL_TRUST _HRESULT_TYPEDEF_(E_SOURCE_SERVER_DISABLE_PARTIAL_TRUST__)
#define E_TRACE_DETACH_UNSUPPORTED _HRESULT_TYPEDEF_(E_TRACE_DETACH_UNSUPPORTED__)
#define E_THREAD_NOT_FOUND _HRESULT_TYPEDEF_(E_THREAD_NOT_FOUND__)
#define E_CANNOT_AUTOATTACH_TO_SQLSERVER _HRESULT_TYPEDEF_(E_CANNOT_AUTOATTACH_TO_SQLSERVER__)
#define E_OBJECT_OUT_OF_SYNC _HRESULT_TYPEDEF_(E_OBJECT_OUT_OF_SYNC__)
































































extern GUID guidVBLang;
extern GUID guidVBScriptLang;
extern GUID guidJScriptLang;
extern GUID guidCLang;
extern GUID guidCPPLang;
extern GUID guidSQLLang;
extern GUID guidScriptLang;
extern GUID guidSafeCLang;
extern GUID guidJSharpLang;
extern GUID guidManagedCLang;
extern GUID guidManagedCPPLang;
extern GUID guidCausalityBreakpointLang;
extern GUID guidFortranLang;
extern GUID guidMethodIdLang;
extern GUID guidClientScriptLang;
extern GUID guidScriptEng;
extern GUID guidSQLEng;
extern GUID guidCOMPlusNativeEng;
extern GUID guidCOMPlusOnlyEng;
extern GUID guidNativeOnlyEng;
extern GUID guidMsOrclEng;
extern GUID guidEmbeddedCLREng;
extern GUID guidSQLEng2;
extern GUID guidCOMPlusSQLLocalEng;
extern GUID guidCOMPlusSQLRemoteEng;
extern GUID guidSilverlightEng;
extern GUID GUID_WorkflowDebugEngine;
extern GUID guidMACSilverlightEng;
extern GUID guidMicrosoftVendor;
extern GUID guidLocalPortSupplier;
extern GUID guidNativePortSupplier;
extern GUID guidNativePipePortSupplier;
extern GUID guidEmbeddedCLRPortSupplier;
extern GUID guidFilterLocals;
extern GUID guidFilterAllLocals;
extern GUID guidFilterArgs;
extern GUID guidFilterLocalsPlusArgs;
extern GUID guidFilterAllLocalsPlusArgs;
extern GUID guidFilterRegisters;
extern GUID guidFilterThis;
extern GUID guidFilterAutoRegisters;
// GUIDs for GetExtendedInfo
extern GUID guidDocument;
extern GUID guidCodeContext;
extern GUID guidCustomViewerSupported;
extern GUID guidSimpleGridViewer;
extern GUID guidExtendedInfoSlot;
extern GUID guidExtendedInfoSignature;
extern GUID guidSourceHashMD5;
extern GUID guidSourceHashSHA1;
extern GUID guidMDANotification;

enum enum_GETNAME_TYPE
    {	GN_NAME	= 0,
	GN_FILENAME	= ( GN_NAME + 1 ) ,
	GN_BASENAME	= ( GN_FILENAME + 1 ) ,
	GN_MONIKERNAME	= ( GN_BASENAME + 1 ) ,
	GN_URL	= ( GN_MONIKERNAME + 1 ) ,
	GN_TITLE	= ( GN_URL + 1 ) ,
	GN_STARTPAGEURL	= ( GN_TITLE + 1 ) 
    } ;
typedef DWORD GETNAME_TYPE;


enum enum_TEXT_POSITION_MAX
    {	TEXT_POSITION_MAX_LINE	= 0xffffffff,
	TEXT_POSITION_MAX_COLUMN	= 0xffffffff
    } ;
typedef struct _tagTEXT_POSITION
    {
    DWORD dwLine;
    DWORD dwColumn;
    } 	TEXT_POSITION;

typedef struct tagBSTR_ARRAY
    {
    DWORD dwCount;
    BSTR *Members;
    } 	BSTR_ARRAY;

typedef struct tagCONST_GUID_ARRAY
    {
    DWORD dwCount;
    const GUID *Members;
    } 	CONST_GUID_ARRAY;

typedef struct tagGUID_ARRAY
    {
    DWORD dwCount;
    GUID *Members;
    } 	GUID_ARRAY;



extern RPC_IF_HANDLE __MIDL_itf_msdbg_0000_0000_v0_0_c_ifspec;
extern RPC_IF_HANDLE __MIDL_itf_msdbg_0000_0000_v0_0_s_ifspec;

#ifndef __IDebugCoreServer2_INTERFACE_DEFINED__
#define __IDebugCoreServer2_INTERFACE_DEFINED__

/* interface IDebugCoreServer2 */
/* [unique][uuid][object] */ 


enum enum_MACHINE_INFO_FLAGS
    {	MCIFLAG_TERMINAL_SERVICES_AVAILABLE	= 0x1
    } ;
typedef DWORD MACHINE_INFO_FLAGS;


enum enum_MACHINE_INFO_FIELDS
    {	MCIF_NAME	= 0x1,
	MCIF_FLAGS	= 0x2,
	MCIF_ALL	= 0x3
    } ;
typedef DWORD MACHINE_INFO_FIELDS;

typedef struct tagMACHINE_INFO
    {
    MACHINE_INFO_FIELDS Fields;
    BSTR bstrName;
    MACHINE_INFO_FLAGS Flags;
    } 	MACHINE_INFO;


EXTERN_C const IID IID_IDebugCoreServer2;

#if defined(__cplusplus) && !defined(CINTERFACE)
    
    MIDL_INTERFACE("17bf8fa3-4c5a-49a3-b2f8-5942e1ea287e")
    IDebugCoreServer2 : public IUnknown
    {
    public:
        virtual HRESULT STDMETHODCALLTYPE GetMachineInfo( 
            /* [in] */ MACHINE_INFO_FIELDS Fields,
            /* [out] */ __RPC__out MACHINE_INFO *pMachineInfo) = 0;
        
        virtual HRESULT STDMETHODCALLTYPE GetMachineName( 
            /* [out] */ __RPC__deref_out_opt BSTR *pbstrName) = 0;
        
        virtual HRESULT STDMETHODCALLTYPE GetPortSupplier( 
            /* [in] */ __RPC__in REFGUID guidPortSupplier,
            /* [out] */ __RPC__deref_out_opt IDebugPortSupplier2 **ppPortSupplier) = 0;
        
        virtual HRESULT STDMETHODCALLTYPE GetPort( 
            /* [in] */ __RPC__in REFGUID guidPort,
            /* [out] */ __RPC__deref_out_opt IDebugPort2 **ppPort) = 0;
        
        virtual HRESULT STDMETHODCALLTYPE EnumPorts( 
            /* [out] */ __RPC__deref_out_opt IEnumDebugPorts2 **ppEnum) = 0;
        
        virtual HRESULT STDMETHODCALLTYPE EnumPortSuppliers( 
            /* [out] */ __RPC__deref_out_opt IEnumDebugPortSuppliers2 **ppEnum) = 0;
        
        virtual HRESULT STDMETHODCALLTYPE GetMachineUtilities_V7( 
            /* [out] */ __RPC__deref_out_opt IDebugMDMUtil2_V7 **ppUtil) = 0;
        
    };
    
#else 	/* C style interface */

    typedef struct IDebugCoreServer2Vtbl
    {
        BEGIN_INTERFACE
        
        HRESULT ( STDMETHODCALLTYPE *QueryInterface )( 
            IDebugCoreServer2 * This,
            /* [in] */ __RPC__in REFIID riid,
            /* [iid_is][out] */ 
            __RPC__deref_out  void **ppvObject);
        
        ULONG ( STDMETHODCALLTYPE *AddRef )( 
            IDebugCoreServer2 * This);
        
        ULONG ( STDMETHODCALLTYPE *Release )( 
            IDebugCoreServer2 * This);
        
        HRESULT ( STDMETHODCALLTYPE *GetMachineInfo )( 
            IDebugCoreServer2 * This,
            /* [in] */ MACHINE_INFO_FIELDS Fields,
            /* [out] */ __RPC__out MACHINE_INFO *pMachineInfo);
        
        HRESULT ( STDMETHODCALLTYPE *GetMachineName )( 
            IDebugCoreServer2 * This,
            /* [out] */ __RPC__deref_out_opt BSTR *pbstrName);
        
        HRESULT ( STDMETHODCALLTYPE *GetPortSupplier )( 
            IDebugCoreServer2 * This,
            /* [in] */ __RPC__in REFGUID guidPortSupplier,
            /* [out] */ __RPC__deref_out_opt IDebugPortSupplier2 **ppPortSupplier);
        
        HRESULT ( STDMETHODCALLTYPE *GetPort )( 
            IDebugCoreServer2 * This,
            /* [in] */ __RPC__in REFGUID guidPort,
            /* [out] */ __RPC__deref_out_opt IDebugPort2 **ppPort);
        
        HRESULT ( STDMETHODCALLTYPE *EnumPorts )( 
            IDebugCoreServer2 * This,
            /* [out] */ __RPC__deref_out_opt IEnumDebugPorts2 **ppEnum);
        
        HRESULT ( STDMETHODCALLTYPE *EnumPortSuppliers )( 
            IDebugCoreServer2 * This,
            /* [out] */ __RPC__deref_out_opt IEnumDebugPortSuppliers2 **ppEnum);
        
        HRESULT ( STDMETHODCALLTYPE *GetMachineUtilities_V7 )( 
            IDebugCoreServer2 * This,
            /* [out] */ __RPC__deref_out_opt IDebugMDMUtil2_V7 **ppUtil);
        
        END_INTERFACE
    } IDebugCoreServer2Vtbl;

    interface IDebugCoreServer2
    {
        CONST_VTBL struct IDebugCoreServer2Vtbl *lpVtbl;
    };

    

#ifdef COBJMACROS


#define IDebugCoreServer2_QueryInterface(This,riid,ppvObject)	\
    ( (This)->lpVtbl -> QueryInterface(This,riid,ppvObject) ) 

#define IDebugCoreServer2_AddRef(This)	\
    ( (This)->lpVtbl -> AddRef(This) ) 

#define IDebugCoreServer2_Release(This)	\
    ( (This)->lpVtbl -> Release(This) ) 


#define IDebugCoreServer2_GetMachineInfo(This,Fields,pMachineInfo)	\
    ( (This)->lpVtbl -> GetMachineInfo(This,Fields,pMachineInfo) ) 

#define IDebugCoreServer2_GetMachineName(This,pbstrName)	\
    ( (This)->lpVtbl -> GetMachineName(This,pbstrName) ) 

#define IDebugCoreServer2_GetPortSupplier(This,guidPortSupplier,ppPortSupplier)	\
    ( (This)->lpVtbl -> GetPortSupplier(This,guidPortSupplier,ppPortSupplier) ) 

#define IDebugCoreServer2_GetPort(This,guidPort,ppPort)	\
    ( (This)->lpVtbl -> GetPort(This,guidPort,ppPort) ) 

#define IDebugCoreServer2_EnumPorts(This,ppEnum)	\
    ( (This)->lpVtbl -> EnumPorts(This,ppEnum) ) 

#define IDebugCoreServer2_EnumPortSuppliers(This,ppEnum)	\
    ( (This)->lpVtbl -> EnumPortSuppliers(This,ppEnum) ) 

#define IDebugCoreServer2_GetMachineUtilities_V7(This,ppUtil)	\
    ( (This)->lpVtbl -> GetMachineUtilities_V7(This,ppUtil) ) 

#endif /* COBJMACROS */


#endif 	/* C style interface */




#endif 	/* __IDebugCoreServer2_INTERFACE_DEFINED__ */


#ifndef __IDebugCoreServer3_INTERFACE_DEFINED__
#define __IDebugCoreServer3_INTERFACE_DEFINED__

/* interface IDebugCoreServer3 */
/* [unique][uuid][object] */ 

typedef 
enum tagCONNECTION_PROTOCOL
    {	CONNECTION_NONE	= 0,
	CONNECTION_UNKNOWN	= 1,
	CONNECTION_LOCAL	= 2,
	CONNECTION_PIPE	= 3,
	CONNECTION_TCPIP	= 4,
	CONNECTION_HTTP	= 5,
	CONNECTION_OTHER	= 6
    } 	CONNECTION_PROTOCOL;


EXTERN_C const IID IID_IDebugCoreServer3;

#if defined(__cplusplus) && !defined(CINTERFACE)
    
    MIDL_INTERFACE("12c1180e-c257-4485-9800-af484b699713")
    IDebugCoreServer3 : public IDebugCoreServer2
    {
    public:
        virtual HRESULT STDMETHODCALLTYPE GetServerName( 
            /* [out] */ __RPC__deref_out_opt BSTR *pbstrName) = 0;
        
        virtual HRESULT STDMETHODCALLTYPE GetServerFriendlyName( 
            /* [out] */ __RPC__deref_out_opt BSTR *pbstrName) = 0;
        
        virtual HRESULT STDMETHODCALLTYPE EnableAutoAttach( 
            /* [full][size_is][in] */ __RPC__in_ecount_full_opt(celtSpecificEngines) GUID *rgguidSpecificEngines,
            /* [in] */ DWORD celtSpecificEngines,
            /* [full][in] */ __RPC__in_opt LPCOLESTR pszStartPageUrl,
            /* [out] */ __RPC__deref_out_opt BSTR *pbstrSessionId) = 0;
        
        virtual HRESULT STDMETHODCALLTYPE DiagnoseWebDebuggingError( 
            /* [full][in] */ __RPC__in_opt LPCWSTR pszUrl) = 0;
        
        virtual HRESULT STDMETHODCALLTYPE CreateInstanceInServer( 
            /* [full][in] */ __RPC__in_opt LPCWSTR szDll,
            /* [in] */ WORD wLangId,
            /* [in] */ __RPC__in REFCLSID clsidObject,
            /* [in] */ __RPC__in REFIID riid,
            /* [iid_is][out] */ __RPC__deref_out_opt void **ppvObject) = 0;
        
        virtual HRESULT STDMETHODCALLTYPE QueryIsLocal( void) = 0;
        
        virtual HRESULT STDMETHODCALLTYPE GetConnectionProtocol( 
            /* [out] */ __RPC__out CONNECTION_PROTOCOL *pProtocol) = 0;
        
        virtual HRESULT STDMETHODCALLTYPE DisableAutoAttach( void) = 0;
        
    };
    
#else 	/* C style interface */

    typedef struct IDebugCoreServer3Vtbl
    {
        BEGIN_INTERFACE
        
        HRESULT ( STDMETHODCALLTYPE *QueryInterface )( 
            IDebugCoreServer3 * This,
            /* [in] */ __RPC__in REFIID riid,
            /* [iid_is][out] */ 
            __RPC__deref_out  void **ppvObject);
        
        ULONG ( STDMETHODCALLTYPE *AddRef )( 
            IDebugCoreServer3 * This);
        
        ULONG ( STDMETHODCALLTYPE *Release )( 
            IDebugCoreServer3 * This);
        
        HRESULT ( STDMETHODCALLTYPE *GetMachineInfo )( 
            IDebugCoreServer3 * This,
            /* [in] */ MACHINE_INFO_FIELDS Fields,
            /* [out] */ __RPC__out MACHINE_INFO *pMachineInfo);
        
        HRESULT ( STDMETHODCALLTYPE *GetMachineName )( 
            IDebugCoreServer3 * This,
            /* [out] */ __RPC__deref_out_opt BSTR *pbstrName);
        
        HRESULT ( STDMETHODCALLTYPE *GetPortSupplier )( 
            IDebugCoreServer3 * This,
            /* [in] */ __RPC__in REFGUID guidPortSupplier,
            /* [out] */ __RPC__deref_out_opt IDebugPortSupplier2 **ppPortSupplier);
        
        HRESULT ( STDMETHODCALLTYPE *GetPort )( 
            IDebugCoreServer3 * This,
            /* [in] */ __RPC__in REFGUID guidPort,
            /* [out] */ __RPC__deref_out_opt IDebugPort2 **ppPort);
        
        HRESULT ( STDMETHODCALLTYPE *EnumPorts )( 
            IDebugCoreServer3 * This,
            /* [out] */ __RPC__deref_out_opt IEnumDebugPorts2 **ppEnum);
        
        HRESULT ( STDMETHODCALLTYPE *EnumPortSuppliers )( 
            IDebugCoreServer3 * This,
            /* [out] */ __RPC__deref_out_opt IEnumDebugPortSuppliers2 **ppEnum);
        
        HRESULT ( STDMETHODCALLTYPE *GetMachineUtilities_V7 )( 
            IDebugCoreServer3 * This,
            /* [out] */ __RPC__deref_out_opt IDebugMDMUtil2_V7 **ppUtil);
        
        HRESULT ( STDMETHODCALLTYPE *GetServerName )( 
            IDebugCoreServer3 * This,
            /* [out] */ __RPC__deref_out_opt BSTR *pbstrName);
        
        HRESULT ( STDMETHODCALLTYPE *GetServerFriendlyName )( 
            IDebugCoreServer3 * This,
            /* [out] */ __RPC__deref_out_opt BSTR *pbstrName);
        
        HRESULT ( STDMETHODCALLTYPE *EnableAutoAttach )( 
            IDebugCoreServer3 * This,
            /* [full][size_is][in] */ __RPC__in_ecount_full_opt(celtSpecificEngines) GUID *rgguidSpecificEngines,
            /* [in] */ DWORD celtSpecificEngines,
            /* [full][in] */ __RPC__in_opt LPCOLESTR pszStartPageUrl,
            /* [out] */ __RPC__deref_out_opt BSTR *pbstrSessionId);
        
        HRESULT ( STDMETHODCALLTYPE *DiagnoseWebDebuggingError )( 
            IDebugCoreServer3 * This,
            /* [full][in] */ __RPC__in_opt LPCWSTR pszUrl);
        
        HRESULT ( STDMETHODCALLTYPE *CreateInstanceInServer )( 
            IDebugCoreServer3 * This,
            /* [full][in] */ __RPC__in_opt LPCWSTR szDll,
            /* [in] */ WORD wLangId,
            /* [in] */ __RPC__in REFCLSID clsidObject,
            /* [in] */ __RPC__in REFIID riid,
            /* [iid_is][out] */ __RPC__deref_out_opt void **ppvObject);
        
        HRESULT ( STDMETHODCALLTYPE *QueryIsLocal )( 
            IDebugCoreServer3 * This);
        
        HRESULT ( STDMETHODCALLTYPE *GetConnectionProtocol )( 
            IDebugCoreServer3 * This,
            /* [out] */ __RPC__out CONNECTION_PROTOCOL *pProtocol);
        
        HRESULT ( STDMETHODCALLTYPE *DisableAutoAttach )( 
            IDebugCoreServer3 * This);
        
        END_INTERFACE
    } IDebugCoreServer3Vtbl;

    interface IDebugCoreServer3
    {
        CONST_VTBL struct IDebugCoreServer3Vtbl *lpVtbl;
    };

    

#ifdef COBJMACROS


#define IDebugCoreServer3_QueryInterface(This,riid,ppvObject)	\
    ( (This)->lpVtbl -> QueryInterface(This,riid,ppvObject) ) 

#define IDebugCoreServer3_AddRef(This)	\
    ( (This)->lpVtbl -> AddRef(This) ) 

#define IDebugCoreServer3_Release(This)	\
    ( (This)->lpVtbl -> Release(This) ) 


#define IDebugCoreServer3_GetMachineInfo(This,Fields,pMachineInfo)	\
    ( (This)->lpVtbl -> GetMachineInfo(This,Fields,pMachineInfo) ) 

#define IDebugCoreServer3_GetMachineName(This,pbstrName)	\
    ( (This)->lpVtbl -> GetMachineName(This,pbstrName) ) 

#define IDebugCoreServer3_GetPortSupplier(This,guidPortSupplier,ppPortSupplier)	\
    ( (This)->lpVtbl -> GetPortSupplier(This,guidPortSupplier,ppPortSupplier) ) 

#define IDebugCoreServer3_GetPort(This,guidPort,ppPort)	\
    ( (This)->lpVtbl -> GetPort(This,guidPort,ppPort) ) 

#define IDebugCoreServer3_EnumPorts(This,ppEnum)	\
    ( (This)->lpVtbl -> EnumPorts(This,ppEnum) ) 

#define IDebugCoreServer3_EnumPortSuppliers(This,ppEnum)	\
    ( (This)->lpVtbl -> EnumPortSuppliers(This,ppEnum) ) 

#define IDebugCoreServer3_GetMachineUtilities_V7(This,ppUtil)	\
    ( (This)->lpVtbl -> GetMachineUtilities_V7(This,ppUtil) ) 


#define IDebugCoreServer3_GetServerName(This,pbstrName)	\
    ( (This)->lpVtbl -> GetServerName(This,pbstrName) ) 

#define IDebugCoreServer3_GetServerFriendlyName(This,pbstrName)	\
    ( (This)->lpVtbl -> GetServerFriendlyName(This,pbstrName) ) 

#define IDebugCoreServer3_EnableAutoAttach(This,rgguidSpecificEngines,celtSpecificEngines,pszStartPageUrl,pbstrSessionId)	\
    ( (This)->lpVtbl -> EnableAutoAttach(This,rgguidSpecificEngines,celtSpecificEngines,pszStartPageUrl,pbstrSessionId) ) 

#define IDebugCoreServer3_DiagnoseWebDebuggingError(This,pszUrl)	\
    ( (This)->lpVtbl -> DiagnoseWebDebuggingError(This,pszUrl) ) 

#define IDebugCoreServer3_CreateInstanceInServer(This,szDll,wLangId,clsidObject,riid,ppvObject)	\
    ( (This)->lpVtbl -> CreateInstanceInServer(This,szDll,wLangId,clsidObject,riid,ppvObject) ) 

#define IDebugCoreServer3_QueryIsLocal(This)	\
    ( (This)->lpVtbl -> QueryIsLocal(This) ) 

#define IDebugCoreServer3_GetConnectionProtocol(This,pProtocol)	\
    ( (This)->lpVtbl -> GetConnectionProtocol(This,pProtocol) ) 

#define IDebugCoreServer3_DisableAutoAttach(This)	\
    ( (This)->lpVtbl -> DisableAutoAttach(This) ) 

#endif /* COBJMACROS */


#endif 	/* C style interface */




#endif 	/* __IDebugCoreServer3_INTERFACE_DEFINED__ */


/* interface __MIDL_itf_msdbg_0000_0002 */
/* [local] */ 

#define IDebugMachine2_V7 IDebugCoreServer2
#define IID_IDebugMachine2_V7 IID_IDebugCoreServer2


extern RPC_IF_HANDLE __MIDL_itf_msdbg_0000_0002_v0_0_c_ifspec;
extern RPC_IF_HANDLE __MIDL_itf_msdbg_0000_0002_v0_0_s_ifspec;

#ifndef __IDebugMachineEx2_V7_INTERFACE_DEFINED__
#define __IDebugMachineEx2_V7_INTERFACE_DEFINED__

/* interface IDebugMachineEx2_V7 */
/* [unique][uuid][object] */ 


EXTERN_C const IID IID_IDebugMachineEx2_V7;

#if defined(__cplusplus) && !defined(CINTERFACE)
    
    MIDL_INTERFACE("ae27b230-a0bf-47ff-a2d1-22c29a178eac")
    IDebugMachineEx2_V7 : public IUnknown
    {
    public:
        virtual HRESULT STDMETHODCALLTYPE EnableAutoAttachOnProgramCreate( 
            /* [in] */ __RPC__in LPCWSTR pszProcessNames,
            /* [in] */ __RPC__in REFGUID guidEngine,
            /* [in] */ __RPC__in LPCWSTR pszSessionId,
            /* [out] */ __RPC__out DWORD *pdwCookie) = 0;
        
        virtual HRESULT STDMETHODCALLTYPE DisableAutoAttachOnEvent( 
            /* [in] */ DWORD dwCookie) = 0;
        
        virtual HRESULT STDMETHODCALLTYPE GetPortSupplierEx( 
            /* [in] */ __RPC__in LPCOLESTR wstrRegistryRoot,
            /* [in] */ __RPC__in REFGUID guidPortSupplier,
            /* [out] */ __RPC__deref_out_opt IDebugPortSupplier2 **ppPortSupplier) = 0;
        
        virtual HRESULT STDMETHODCALLTYPE GetPortEx( 
            /* [in] */ __RPC__in LPCOLESTR wstrRegistryRoot,
            /* [in] */ __RPC__in REFGUID guidPort,
            /* [out] */ __RPC__deref_out_opt IDebugPort2 **ppPort) = 0;
        
        virtual HRESULT STDMETHODCALLTYPE EnumPortsEx( 
            /* [in] */ __RPC__in LPCOLESTR wstrRegistryRoot,
            /* [out] */ __RPC__deref_out_opt IEnumDebugPorts2 **ppEnum) = 0;
        
        virtual HRESULT STDMETHODCALLTYPE EnumPortSuppliersEx( 
            /* [in] */ __RPC__in LPCOLESTR wstrRegistryRoot,
            /* [out] */ __RPC__deref_out_opt IEnumDebugPortSuppliers2 **ppEnum) = 0;
        
    };
    
#else 	/* C style interface */

    typedef struct IDebugMachineEx2_V7Vtbl
    {
        BEGIN_INTERFACE
        
        HRESULT ( STDMETHODCALLTYPE *QueryInterface )( 
            IDebugMachineEx2_V7 * This,
            /* [in] */ __RPC__in REFIID riid,
            /* [iid_is][out] */ 
            __RPC__deref_out  void **ppvObject);
        
        ULONG ( STDMETHODCALLTYPE *AddRef )( 
            IDebugMachineEx2_V7 * This);
        
        ULONG ( STDMETHODCALLTYPE *Release )( 
            IDebugMachineEx2_V7 * This);
        
        HRESULT ( STDMETHODCALLTYPE *EnableAutoAttachOnProgramCreate )( 
            IDebugMachineEx2_V7 * This,
            /* [in] */ __RPC__in LPCWSTR pszProcessNames,
            /* [in] */ __RPC__in REFGUID guidEngine,
            /* [in] */ __RPC__in LPCWSTR pszSessionId,
            /* [out] */ __RPC__out DWORD *pdwCookie);
        
        HRESULT ( STDMETHODCALLTYPE *DisableAutoAttachOnEvent )( 
            IDebugMachineEx2_V7 * This,
            /* [in] */ DWORD dwCookie);
        
        HRESULT ( STDMETHODCALLTYPE *GetPortSupplierEx )( 
            IDebugMachineEx2_V7 * This,
            /* [in] */ __RPC__in LPCOLESTR wstrRegistryRoot,
            /* [in] */ __RPC__in REFGUID guidPortSupplier,
            /* [out] */ __RPC__deref_out_opt IDebugPortSupplier2 **ppPortSupplier);
        
        HRESULT ( STDMETHODCALLTYPE *GetPortEx )( 
            IDebugMachineEx2_V7 * This,
            /* [in] */ __RPC__in LPCOLESTR wstrRegistryRoot,
            /* [in] */ __RPC__in REFGUID guidPort,
            /* [out] */ __RPC__deref_out_opt IDebugPort2 **ppPort);
        
        HRESULT ( STDMETHODCALLTYPE *EnumPortsEx )( 
            IDebugMachineEx2_V7 * This,
            /* [in] */ __RPC__in LPCOLESTR wstrRegistryRoot,
            /* [out] */ __RPC__deref_out_opt IEnumDebugPorts2 **ppEnum);
        
        HRESULT ( STDMETHODCALLTYPE *EnumPortSuppliersEx )( 
            IDebugMachineEx2_V7 * This,
            /* [in] */ __RPC__in LPCOLESTR wstrRegistryRoot,
            /* [out] */ __RPC__deref_out_opt IEnumDebugPortSuppliers2 **ppEnum);
        
        END_INTERFACE
    } IDebugMachineEx2_V7Vtbl;

    interface IDebugMachineEx2_V7
    {
        CONST_VTBL struct IDebugMachineEx2_V7Vtbl *lpVtbl;
    };

    

#ifdef COBJMACROS


#define IDebugMachineEx2_V7_QueryInterface(This,riid,ppvObject)	\
    ( (This)->lpVtbl -> QueryInterface(This,riid,ppvObject) ) 

#define IDebugMachineEx2_V7_AddRef(This)	\
    ( (This)->lpVtbl -> AddRef(This) ) 

#define IDebugMachineEx2_V7_Release(This)	\
    ( (This)->lpVtbl -> Release(This) ) 


#define IDebugMachineEx2_V7_EnableAutoAttachOnProgramCreate(This,pszProcessNames,guidEngine,pszSessionId,pdwCookie)	\
    ( (This)->lpVtbl -> EnableAutoAttachOnProgramCreate(This,pszProcessNames,guidEngine,pszSessionId,pdwCookie) ) 

#define IDebugMachineEx2_V7_DisableAutoAttachOnEvent(This,dwCookie)	\
    ( (This)->lpVtbl -> DisableAutoAttachOnEvent(This,dwCookie) ) 

#define IDebugMachineEx2_V7_GetPortSupplierEx(This,wstrRegistryRoot,guidPortSupplier,ppPortSupplier)	\
    ( (This)->lpVtbl -> GetPortSupplierEx(This,wstrRegistryRoot,guidPortSupplier,ppPortSupplier) ) 

#define IDebugMachineEx2_V7_GetPortEx(This,wstrRegistryRoot,guidPort,ppPort)	\
    ( (This)->lpVtbl -> GetPortEx(This,wstrRegistryRoot,guidPort,ppPort) ) 

#define IDebugMachineEx2_V7_EnumPortsEx(This,wstrRegistryRoot,ppEnum)	\
    ( (This)->lpVtbl -> EnumPortsEx(This,wstrRegistryRoot,ppEnum) ) 

#define IDebugMachineEx2_V7_EnumPortSuppliersEx(This,wstrRegistryRoot,ppEnum)	\
    ( (This)->lpVtbl -> EnumPortSuppliersEx(This,wstrRegistryRoot,ppEnum) ) 

#endif /* COBJMACROS */


#endif 	/* C style interface */




#endif 	/* __IDebugMachineEx2_V7_INTERFACE_DEFINED__ */


#ifndef __IDebugPortSupplier2_INTERFACE_DEFINED__
#define __IDebugPortSupplier2_INTERFACE_DEFINED__

/* interface IDebugPortSupplier2 */
/* [unique][uuid][object] */ 


EXTERN_C const IID IID_IDebugPortSupplier2;

#if defined(__cplusplus) && !defined(CINTERFACE)
    
    MIDL_INTERFACE("53f68191-7b2f-4f14-8e55-40b1b6e5df66")
    IDebugPortSupplier2 : public IUnknown
    {
    public:
        virtual HRESULT STDMETHODCALLTYPE GetPortSupplierName( 
            /* [out] */ __RPC__deref_out_opt BSTR *pbstrName) = 0;
        
        virtual HRESULT STDMETHODCALLTYPE GetPortSupplierId( 
            /* [out] */ __RPC__out GUID *pguidPortSupplier) = 0;
        
        virtual HRESULT STDMETHODCALLTYPE GetPort( 
            /* [in] */ __RPC__in REFGUID guidPort,
            /* [out] */ __RPC__deref_out_opt IDebugPort2 **ppPort) = 0;
        
        virtual HRESULT STDMETHODCALLTYPE EnumPorts( 
            /* [out] */ __RPC__deref_out_opt IEnumDebugPorts2 **ppEnum) = 0;
        
        virtual HRESULT STDMETHODCALLTYPE CanAddPort( void) = 0;
        
        virtual HRESULT STDMETHODCALLTYPE AddPort( 
            /* [in] */ __RPC__in_opt IDebugPortRequest2 *pRequest,
            /* [out] */ __RPC__deref_out_opt IDebugPort2 **ppPort) = 0;
        
        virtual HRESULT STDMETHODCALLTYPE RemovePort( 
            /* [in] */ __RPC__in_opt IDebugPort2 *pPort) = 0;
        
    };
    
#else 	/* C style interface */

    typedef struct IDebugPortSupplier2Vtbl
    {
        BEGIN_INTERFACE
        
        HRESULT ( STDMETHODCALLTYPE *QueryInterface )( 
            IDebugPortSupplier2 * This,
            /* [in] */ __RPC__in REFIID riid,
            /* [iid_is][out] */ 
            __RPC__deref_out  void **ppvObject);
        
        ULONG ( STDMETHODCALLTYPE *AddRef )( 
            IDebugPortSupplier2 * This);
        
        ULONG ( STDMETHODCALLTYPE *Release )( 
            IDebugPortSupplier2 * This);
        
        HRESULT ( STDMETHODCALLTYPE *GetPortSupplierName )( 
            IDebugPortSupplier2 * This,
            /* [out] */ __RPC__deref_out_opt BSTR *pbstrName);
        
        HRESULT ( STDMETHODCALLTYPE *GetPortSupplierId )( 
            IDebugPortSupplier2 * This,
            /* [out] */ __RPC__out GUID *pguidPortSupplier);
        
        HRESULT ( STDMETHODCALLTYPE *GetPort )( 
            IDebugPortSupplier2 * This,
            /* [in] */ __RPC__in REFGUID guidPort,
            /* [out] */ __RPC__deref_out_opt IDebugPort2 **ppPort);
        
        HRESULT ( STDMETHODCALLTYPE *EnumPorts )( 
            IDebugPortSupplier2 * This,
            /* [out] */ __RPC__deref_out_opt IEnumDebugPorts2 **ppEnum);
        
        HRESULT ( STDMETHODCALLTYPE *CanAddPort )( 
            IDebugPortSupplier2 * This);
        
        HRESULT ( STDMETHODCALLTYPE *AddPort )( 
            IDebugPortSupplier2 * This,
            /* [in] */ __RPC__in_opt IDebugPortRequest2 *pRequest,
            /* [out] */ __RPC__deref_out_opt IDebugPort2 **ppPort);
        
        HRESULT ( STDMETHODCALLTYPE *RemovePort )( 
            IDebugPortSupplier2 * This,
            /* [in] */ __RPC__in_opt IDebugPort2 *pPort);
        
        END_INTERFACE
    } IDebugPortSupplier2Vtbl;

    interface IDebugPortSupplier2
    {
        CONST_VTBL struct IDebugPortSupplier2Vtbl *lpVtbl;
    };

    

#ifdef COBJMACROS


#define IDebugPortSupplier2_QueryInterface(This,riid,ppvObject)	\
    ( (This)->lpVtbl -> QueryInterface(This,riid,ppvObject) ) 

#define IDebugPortSupplier2_AddRef(This)	\
    ( (This)->lpVtbl -> AddRef(This) ) 

#define IDebugPortSupplier2_Release(This)	\
    ( (This)->lpVtbl -> Release(This) ) 


#define IDebugPortSupplier2_GetPortSupplierName(This,pbstrName)	\
    ( (This)->lpVtbl -> GetPortSupplierName(This,pbstrName) ) 

#define IDebugPortSupplier2_GetPortSupplierId(This,pguidPortSupplier)	\
    ( (This)->lpVtbl -> GetPortSupplierId(This,pguidPortSupplier) ) 

#define IDebugPortSupplier2_GetPort(This,guidPort,ppPort)	\
    ( (This)->lpVtbl -> GetPort(This,guidPort,ppPort) ) 

#define IDebugPortSupplier2_EnumPorts(This,ppEnum)	\
    ( (This)->lpVtbl -> EnumPorts(This,ppEnum) ) 

#define IDebugPortSupplier2_CanAddPort(This)	\
    ( (This)->lpVtbl -> CanAddPort(This) ) 

#define IDebugPortSupplier2_AddPort(This,pRequest,ppPort)	\
    ( (This)->lpVtbl -> AddPort(This,pRequest,ppPort) ) 

#define IDebugPortSupplier2_RemovePort(This,pPort)	\
    ( (This)->lpVtbl -> RemovePort(This,pPort) ) 

#endif /* COBJMACROS */


#endif 	/* C style interface */




#endif 	/* __IDebugPortSupplier2_INTERFACE_DEFINED__ */


#ifndef __IDebugPortSupplier3_INTERFACE_DEFINED__
#define __IDebugPortSupplier3_INTERFACE_DEFINED__

/* interface IDebugPortSupplier3 */
/* [unique][uuid][object] */ 


EXTERN_C const IID IID_IDebugPortSupplier3;

#if defined(__cplusplus) && !defined(CINTERFACE)
    
    MIDL_INTERFACE("5b5eec44-51aa-4210-b84f-1938b8576d8d")
    IDebugPortSupplier3 : public IDebugPortSupplier2
    {
    public:
        virtual HRESULT STDMETHODCALLTYPE CanPersistPorts( void) = 0;
        
        virtual HRESULT STDMETHODCALLTYPE EnumPersistedPorts( 
            /* [in] */ BSTR_ARRAY PortNames,
            /* [out] */ __RPC__deref_out_opt IEnumDebugPorts2 **ppEnum) = 0;
        
    };
    
#else 	/* C style interface */

    typedef struct IDebugPortSupplier3Vtbl
    {
        BEGIN_INTERFACE
        
        HRESULT ( STDMETHODCALLTYPE *QueryInterface )( 
            IDebugPortSupplier3 * This,
            /* [in] */ __RPC__in REFIID riid,
            /* [iid_is][out] */ 
            __RPC__deref_out  void **ppvObject);
        
        ULONG ( STDMETHODCALLTYPE *AddRef )( 
            IDebugPortSupplier3 * This);
        
        ULONG ( STDMETHODCALLTYPE *Release )( 
            IDebugPortSupplier3 * This);
        
        HRESULT ( STDMETHODCALLTYPE *GetPortSupplierName )( 
            IDebugPortSupplier3 * This,
            /* [out] */ __RPC__deref_out_opt BSTR *pbstrName);
        
        HRESULT ( STDMETHODCALLTYPE *GetPortSupplierId )( 
            IDebugPortSupplier3 * This,
            /* [out] */ __RPC__out GUID *pguidPortSupplier);
        
        HRESULT ( STDMETHODCALLTYPE *GetPort )( 
            IDebugPortSupplier3 * This,
            /* [in] */ __RPC__in REFGUID guidPort,
            /* [out] */ __RPC__deref_out_opt IDebugPort2 **ppPort);
        
        HRESULT ( STDMETHODCALLTYPE *EnumPorts )( 
            IDebugPortSupplier3 * This,
            /* [out] */ __RPC__deref_out_opt IEnumDebugPorts2 **ppEnum);
        
        HRESULT ( STDMETHODCALLTYPE *CanAddPort )( 
            IDebugPortSupplier3 * This);
        
        HRESULT ( STDMETHODCALLTYPE *AddPort )( 
            IDebugPortSupplier3 * This,
            /* [in] */ __RPC__in_opt IDebugPortRequest2 *pRequest,
            /* [out] */ __RPC__deref_out_opt IDebugPort2 **ppPort);
        
        HRESULT ( STDMETHODCALLTYPE *RemovePort )( 
            IDebugPortSupplier3 * This,
            /* [in] */ __RPC__in_opt IDebugPort2 *pPort);
        
        HRESULT ( STDMETHODCALLTYPE *CanPersistPorts )( 
            IDebugPortSupplier3 * This);
        
        HRESULT ( STDMETHODCALLTYPE *EnumPersistedPorts )( 
            IDebugPortSupplier3 * This,
            /* [in] */ BSTR_ARRAY PortNames,
            /* [out] */ __RPC__deref_out_opt IEnumDebugPorts2 **ppEnum);
        
        END_INTERFACE
    } IDebugPortSupplier3Vtbl;

    interface IDebugPortSupplier3
    {
        CONST_VTBL struct IDebugPortSupplier3Vtbl *lpVtbl;
    };

    

#ifdef COBJMACROS


#define IDebugPortSupplier3_QueryInterface(This,riid,ppvObject)	\
    ( (This)->lpVtbl -> QueryInterface(This,riid,ppvObject) ) 

#define IDebugPortSupplier3_AddRef(This)	\
    ( (This)->lpVtbl -> AddRef(This) ) 

#define IDebugPortSupplier3_Release(This)	\
    ( (This)->lpVtbl -> Release(This) ) 


#define IDebugPortSupplier3_GetPortSupplierName(This,pbstrName)	\
    ( (This)->lpVtbl -> GetPortSupplierName(This,pbstrName) ) 

#define IDebugPortSupplier3_GetPortSupplierId(This,pguidPortSupplier)	\
    ( (This)->lpVtbl -> GetPortSupplierId(This,pguidPortSupplier) ) 

#define IDebugPortSupplier3_GetPort(This,guidPort,ppPort)	\
    ( (This)->lpVtbl -> GetPort(This,guidPort,ppPort) ) 

#define IDebugPortSupplier3_EnumPorts(This,ppEnum)	\
    ( (This)->lpVtbl -> EnumPorts(This,ppEnum) ) 

#define IDebugPortSupplier3_CanAddPort(This)	\
    ( (This)->lpVtbl -> CanAddPort(This) ) 

#define IDebugPortSupplier3_AddPort(This,pRequest,ppPort)	\
    ( (This)->lpVtbl -> AddPort(This,pRequest,ppPort) ) 

#define IDebugPortSupplier3_RemovePort(This,pPort)	\
    ( (This)->lpVtbl -> RemovePort(This,pPort) ) 


#define IDebugPortSupplier3_CanPersistPorts(This)	\
    ( (This)->lpVtbl -> CanPersistPorts(This) ) 

#define IDebugPortSupplier3_EnumPersistedPorts(This,PortNames,ppEnum)	\
    ( (This)->lpVtbl -> EnumPersistedPorts(This,PortNames,ppEnum) ) 

#endif /* COBJMACROS */


#endif 	/* C style interface */




#endif 	/* __IDebugPortSupplier3_INTERFACE_DEFINED__ */


#ifndef __IDebugPortPicker_INTERFACE_DEFINED__
#define __IDebugPortPicker_INTERFACE_DEFINED__

/* interface IDebugPortPicker */
/* [unique][uuid][object] */ 


EXTERN_C const IID IID_IDebugPortPicker;

#if defined(__cplusplus) && !defined(CINTERFACE)
    
    MIDL_INTERFACE("8d6eed60-2737-4425-b38a-490ef273acbb")
    IDebugPortPicker : public IUnknown
    {
    public:
        virtual HRESULT STDMETHODCALLTYPE SetSite( 
            /* [in] */ __RPC__in_opt IServiceProvider *pSP) = 0;
        
        virtual HRESULT STDMETHODCALLTYPE DisplayPortPicker( 
            /* [in] */ __RPC__in HWND hwndParentDialog,
            /* [out] */ __RPC__deref_out_opt BSTR *pbstrPortId) = 0;
        
    };
    
#else 	/* C style interface */

    typedef struct IDebugPortPickerVtbl
    {
        BEGIN_INTERFACE
        
        HRESULT ( STDMETHODCALLTYPE *QueryInterface )( 
            IDebugPortPicker * This,
            /* [in] */ __RPC__in REFIID riid,
            /* [iid_is][out] */ 
            __RPC__deref_out  void **ppvObject);
        
        ULONG ( STDMETHODCALLTYPE *AddRef )( 
            IDebugPortPicker * This);
        
        ULONG ( STDMETHODCALLTYPE *Release )( 
            IDebugPortPicker * This);
        
        HRESULT ( STDMETHODCALLTYPE *SetSite )( 
            IDebugPortPicker * This,
            /* [in] */ __RPC__in_opt IServiceProvider *pSP);
        
        HRESULT ( STDMETHODCALLTYPE *DisplayPortPicker )( 
            IDebugPortPicker * This,
            /* [in] */ __RPC__in HWND hwndParentDialog,
            /* [out] */ __RPC__deref_out_opt BSTR *pbstrPortId);
        
        END_INTERFACE
    } IDebugPortPickerVtbl;

    interface IDebugPortPicker
    {
        CONST_VTBL struct IDebugPortPickerVtbl *lpVtbl;
    };

    

#ifdef COBJMACROS


#define IDebugPortPicker_QueryInterface(This,riid,ppvObject)	\
    ( (This)->lpVtbl -> QueryInterface(This,riid,ppvObject) ) 

#define IDebugPortPicker_AddRef(This)	\
    ( (This)->lpVtbl -> AddRef(This) ) 

#define IDebugPortPicker_Release(This)	\
    ( (This)->lpVtbl -> Release(This) ) 


#define IDebugPortPicker_SetSite(This,pSP)	\
    ( (This)->lpVtbl -> SetSite(This,pSP) ) 

#define IDebugPortPicker_DisplayPortPicker(This,hwndParentDialog,pbstrPortId)	\
    ( (This)->lpVtbl -> DisplayPortPicker(This,hwndParentDialog,pbstrPortId) ) 

#endif /* COBJMACROS */


#endif 	/* C style interface */




#endif 	/* __IDebugPortPicker_INTERFACE_DEFINED__ */


#ifndef __IDebugPortSupplierDescription2_INTERFACE_DEFINED__
#define __IDebugPortSupplierDescription2_INTERFACE_DEFINED__

/* interface IDebugPortSupplierDescription2 */
/* [unique][uuid][object] */ 


enum enum_PORT_SUPPLIER_DESCRIPTION_FLAGS
    {	PSDFLAG_SHOW_WARNING_ICON	= 0x1
    } ;
typedef DWORD PORT_SUPPLIER_DESCRIPTION_FLAGS;


EXTERN_C const IID IID_IDebugPortSupplierDescription2;

#if defined(__cplusplus) && !defined(CINTERFACE)
    
    MIDL_INTERFACE("d0785faa-91d7-4ca2-a302-6555487719f7")
    IDebugPortSupplierDescription2 : public IUnknown
    {
    public:
        virtual HRESULT STDMETHODCALLTYPE GetDescription( 
            /* [out] */ __RPC__out PORT_SUPPLIER_DESCRIPTION_FLAGS *pdwFlags,
            /* [out] */ __RPC__deref_out_opt BSTR *pbstrText) = 0;
        
    };
    
#else 	/* C style interface */

    typedef struct IDebugPortSupplierDescription2Vtbl
    {
        BEGIN_INTERFACE
        
        HRESULT ( STDMETHODCALLTYPE *QueryInterface )( 
            IDebugPortSupplierDescription2 * This,
            /* [in] */ __RPC__in REFIID riid,
            /* [iid_is][out] */ 
            __RPC__deref_out  void **ppvObject);
        
        ULONG ( STDMETHODCALLTYPE *AddRef )( 
            IDebugPortSupplierDescription2 * This);
        
        ULONG ( STDMETHODCALLTYPE *Release )( 
            IDebugPortSupplierDescription2 * This);
        
        HRESULT ( STDMETHODCALLTYPE *GetDescription )( 
            IDebugPortSupplierDescription2 * This,
            /* [out] */ __RPC__out PORT_SUPPLIER_DESCRIPTION_FLAGS *pdwFlags,
            /* [out] */ __RPC__deref_out_opt BSTR *pbstrText);
        
        END_INTERFACE
    } IDebugPortSupplierDescription2Vtbl;

    interface IDebugPortSupplierDescription2
    {
        CONST_VTBL struct IDebugPortSupplierDescription2Vtbl *lpVtbl;
    };

    

#ifdef COBJMACROS


#define IDebugPortSupplierDescription2_QueryInterface(This,riid,ppvObject)	\
    ( (This)->lpVtbl -> QueryInterface(This,riid,ppvObject) ) 

#define IDebugPortSupplierDescription2_AddRef(This)	\
    ( (This)->lpVtbl -> AddRef(This) ) 

#define IDebugPortSupplierDescription2_Release(This)	\
    ( (This)->lpVtbl -> Release(This) ) 


#define IDebugPortSupplierDescription2_GetDescription(This,pdwFlags,pbstrText)	\
    ( (This)->lpVtbl -> GetDescription(This,pdwFlags,pbstrText) ) 

#endif /* COBJMACROS */


#endif 	/* C style interface */




#endif 	/* __IDebugPortSupplierDescription2_INTERFACE_DEFINED__ */


#ifndef __IDebugPort2_INTERFACE_DEFINED__
#define __IDebugPort2_INTERFACE_DEFINED__

/* interface IDebugPort2 */
/* [unique][uuid][object] */ 


enum enum_AD_PROCESS_ID
    {	AD_PROCESS_ID_SYSTEM	= 0,
	AD_PROCESS_ID_GUID	= ( AD_PROCESS_ID_SYSTEM + 1 ) 
    } ;
typedef DWORD AD_PROCESS_ID_TYPE;

typedef struct _AD_PROCESS_ID
    {
    AD_PROCESS_ID_TYPE ProcessIdType;
    /* [switch_type] */ union __MIDL_IDebugPort2_0001
        {
        DWORD dwProcessId;
        GUID guidProcessId;
        DWORD dwUnused;
        } 	ProcessId;
    } 	AD_PROCESS_ID;


EXTERN_C const IID IID_IDebugPort2;

#if defined(__cplusplus) && !defined(CINTERFACE)
    
    MIDL_INTERFACE("79293cc8-d9d9-43f5-97ad-0bcc5a688776")
    IDebugPort2 : public IUnknown
    {
    public:
        virtual HRESULT STDMETHODCALLTYPE GetPortName( 
            /* [out] */ __RPC__deref_out_opt BSTR *pbstrName) = 0;
        
        virtual HRESULT STDMETHODCALLTYPE GetPortId( 
            /* [out] */ __RPC__out GUID *pguidPort) = 0;
        
        virtual HRESULT STDMETHODCALLTYPE GetPortRequest( 
            /* [out] */ __RPC__deref_out_opt IDebugPortRequest2 **ppRequest) = 0;
        
        virtual HRESULT STDMETHODCALLTYPE GetPortSupplier( 
            /* [out] */ __RPC__deref_out_opt IDebugPortSupplier2 **ppSupplier) = 0;
        
        virtual HRESULT STDMETHODCALLTYPE GetProcess( 
            /* [in] */ AD_PROCESS_ID ProcessId,
            /* [out] */ __RPC__deref_out_opt IDebugProcess2 **ppProcess) = 0;
        
        virtual HRESULT STDMETHODCALLTYPE EnumProcesses( 
            /* [out] */ __RPC__deref_out_opt IEnumDebugProcesses2 **ppEnum) = 0;
        
    };
    
#else 	/* C style interface */

    typedef struct IDebugPort2Vtbl
    {
        BEGIN_INTERFACE
        
        HRESULT ( STDMETHODCALLTYPE *QueryInterface )( 
            IDebugPort2 * This,
            /* [in] */ __RPC__in REFIID riid,
            /* [iid_is][out] */ 
            __RPC__deref_out  void **ppvObject);
        
        ULONG ( STDMETHODCALLTYPE *AddRef )( 
            IDebugPort2 * This);
        
        ULONG ( STDMETHODCALLTYPE *Release )( 
            IDebugPort2 * This);
        
        HRESULT ( STDMETHODCALLTYPE *GetPortName )( 
            IDebugPort2 * This,
            /* [out] */ __RPC__deref_out_opt BSTR *pbstrName);
        
        HRESULT ( STDMETHODCALLTYPE *GetPortId )( 
            IDebugPort2 * This,
            /* [out] */ __RPC__out GUID *pguidPort);
        
        HRESULT ( STDMETHODCALLTYPE *GetPortRequest )( 
            IDebugPort2 * This,
            /* [out] */ __RPC__deref_out_opt IDebugPortRequest2 **ppRequest);
        
        HRESULT ( STDMETHODCALLTYPE *GetPortSupplier )( 
            IDebugPort2 * This,
            /* [out] */ __RPC__deref_out_opt IDebugPortSupplier2 **ppSupplier);
        
        HRESULT ( STDMETHODCALLTYPE *GetProcess )( 
            IDebugPort2 * This,
            /* [in] */ AD_PROCESS_ID ProcessId,
            /* [out] */ __RPC__deref_out_opt IDebugProcess2 **ppProcess);
        
        HRESULT ( STDMETHODCALLTYPE *EnumProcesses )( 
            IDebugPort2 * This,
            /* [out] */ __RPC__deref_out_opt IEnumDebugProcesses2 **ppEnum);
        
        END_INTERFACE
    } IDebugPort2Vtbl;

    interface IDebugPort2
    {
        CONST_VTBL struct IDebugPort2Vtbl *lpVtbl;
    };

    

#ifdef COBJMACROS


#define IDebugPort2_QueryInterface(This,riid,ppvObject)	\
    ( (This)->lpVtbl -> QueryInterface(This,riid,ppvObject) ) 

#define IDebugPort2_AddRef(This)	\
    ( (This)->lpVtbl -> AddRef(This) ) 

#define IDebugPort2_Release(This)	\
    ( (This)->lpVtbl -> Release(This) ) 


#define IDebugPort2_GetPortName(This,pbstrName)	\
    ( (This)->lpVtbl -> GetPortName(This,pbstrName) ) 

#define IDebugPort2_GetPortId(This,pguidPort)	\
    ( (This)->lpVtbl -> GetPortId(This,pguidPort) ) 

#define IDebugPort2_GetPortRequest(This,ppRequest)	\
    ( (This)->lpVtbl -> GetPortRequest(This,ppRequest) ) 

#define IDebugPort2_GetPortSupplier(This,ppSupplier)	\
    ( (This)->lpVtbl -> GetPortSupplier(This,ppSupplier) ) 

#define IDebugPort2_GetProcess(This,ProcessId,ppProcess)	\
    ( (This)->lpVtbl -> GetProcess(This,ProcessId,ppProcess) ) 

#define IDebugPort2_EnumProcesses(This,ppEnum)	\
    ( (This)->lpVtbl -> EnumProcesses(This,ppEnum) ) 

#endif /* COBJMACROS */


#endif 	/* C style interface */




#endif 	/* __IDebugPort2_INTERFACE_DEFINED__ */


#ifndef __IDebugDefaultPort2_INTERFACE_DEFINED__
#define __IDebugDefaultPort2_INTERFACE_DEFINED__

/* interface IDebugDefaultPort2 */
/* [unique][uuid][object] */ 


EXTERN_C const IID IID_IDebugDefaultPort2;

#if defined(__cplusplus) && !defined(CINTERFACE)
    
    MIDL_INTERFACE("302f0f55-1ede-4777-9b38-115e1f229d56")
    IDebugDefaultPort2 : public IDebugPort2
    {
    public:
        virtual HRESULT STDMETHODCALLTYPE GetPortNotify( 
            /* [out] */ __RPC__deref_out_opt IDebugPortNotify2 **ppPortNotify) = 0;
        
        virtual HRESULT STDMETHODCALLTYPE GetServer( 
            /* [out] */ __RPC__deref_out_opt IDebugCoreServer3 **ppServer) = 0;
        
        virtual HRESULT STDMETHODCALLTYPE QueryIsLocal( void) = 0;
        
    };
    
#else 	/* C style interface */

    typedef struct IDebugDefaultPort2Vtbl
    {
        BEGIN_INTERFACE
        
        HRESULT ( STDMETHODCALLTYPE *QueryInterface )( 
            IDebugDefaultPort2 * This,
            /* [in] */ __RPC__in REFIID riid,
            /* [iid_is][out] */ 
            __RPC__deref_out  void **ppvObject);
        
        ULONG ( STDMETHODCALLTYPE *AddRef )( 
            IDebugDefaultPort2 * This);
        
        ULONG ( STDMETHODCALLTYPE *Release )( 
            IDebugDefaultPort2 * This);
        
        HRESULT ( STDMETHODCALLTYPE *GetPortName )( 
            IDebugDefaultPort2 * This,
            /* [out] */ __RPC__deref_out_opt BSTR *pbstrName);
        
        HRESULT ( STDMETHODCALLTYPE *GetPortId )( 
            IDebugDefaultPort2 * This,
            /* [out] */ __RPC__out GUID *pguidPort);
        
        HRESULT ( STDMETHODCALLTYPE *GetPortRequest )( 
            IDebugDefaultPort2 * This,
            /* [out] */ __RPC__deref_out_opt IDebugPortRequest2 **ppRequest);
        
        HRESULT ( STDMETHODCALLTYPE *GetPortSupplier )( 
            IDebugDefaultPort2 * This,
            /* [out] */ __RPC__deref_out_opt IDebugPortSupplier2 **ppSupplier);
        
        HRESULT ( STDMETHODCALLTYPE *GetProcess )( 
            IDebugDefaultPort2 * This,
            /* [in] */ AD_PROCESS_ID ProcessId,
            /* [out] */ __RPC__deref_out_opt IDebugProcess2 **ppProcess);
        
        HRESULT ( STDMETHODCALLTYPE *EnumProcesses )( 
            IDebugDefaultPort2 * This,
            /* [out] */ __RPC__deref_out_opt IEnumDebugProcesses2 **ppEnum);
        
        HRESULT ( STDMETHODCALLTYPE *GetPortNotify )( 
            IDebugDefaultPort2 * This,
            /* [out] */ __RPC__deref_out_opt IDebugPortNotify2 **ppPortNotify);
        
        HRESULT ( STDMETHODCALLTYPE *GetServer )( 
            IDebugDefaultPort2 * This,
            /* [out] */ __RPC__deref_out_opt IDebugCoreServer3 **ppServer);
        
        HRESULT ( STDMETHODCALLTYPE *QueryIsLocal )( 
            IDebugDefaultPort2 * This);
        
        END_INTERFACE
    } IDebugDefaultPort2Vtbl;

    interface IDebugDefaultPort2
    {
        CONST_VTBL struct IDebugDefaultPort2Vtbl *lpVtbl;
    };

    

#ifdef COBJMACROS


#define IDebugDefaultPort2_QueryInterface(This,riid,ppvObject)	\
    ( (This)->lpVtbl -> QueryInterface(This,riid,ppvObject) ) 

#define IDebugDefaultPort2_AddRef(This)	\
    ( (This)->lpVtbl -> AddRef(This) ) 

#define IDebugDefaultPort2_Release(This)	\
    ( (This)->lpVtbl -> Release(This) ) 


#define IDebugDefaultPort2_GetPortName(This,pbstrName)	\
    ( (This)->lpVtbl -> GetPortName(This,pbstrName) ) 

#define IDebugDefaultPort2_GetPortId(This,pguidPort)	\
    ( (This)->lpVtbl -> GetPortId(This,pguidPort) ) 

#define IDebugDefaultPort2_GetPortRequest(This,ppRequest)	\
    ( (This)->lpVtbl -> GetPortRequest(This,ppRequest) ) 

#define IDebugDefaultPort2_GetPortSupplier(This,ppSupplier)	\
    ( (This)->lpVtbl -> GetPortSupplier(This,ppSupplier) ) 

#define IDebugDefaultPort2_GetProcess(This,ProcessId,ppProcess)	\
    ( (This)->lpVtbl -> GetProcess(This,ProcessId,ppProcess) ) 

#define IDebugDefaultPort2_EnumProcesses(This,ppEnum)	\
    ( (This)->lpVtbl -> EnumProcesses(This,ppEnum) ) 


#define IDebugDefaultPort2_GetPortNotify(This,ppPortNotify)	\
    ( (This)->lpVtbl -> GetPortNotify(This,ppPortNotify) ) 

#define IDebugDefaultPort2_GetServer(This,ppServer)	\
    ( (This)->lpVtbl -> GetServer(This,ppServer) ) 

#define IDebugDefaultPort2_QueryIsLocal(This)	\
    ( (This)->lpVtbl -> QueryIsLocal(This) ) 

#endif /* COBJMACROS */


#endif 	/* C style interface */




#endif 	/* __IDebugDefaultPort2_INTERFACE_DEFINED__ */


#ifndef __IDebugWindowsComputerPort2_INTERFACE_DEFINED__
#define __IDebugWindowsComputerPort2_INTERFACE_DEFINED__

/* interface IDebugWindowsComputerPort2 */
/* [unique][uuid][object] */ 

typedef struct tagCOMPUTER_INFO
    {
    WORD wProcessorArchitecture;
    WORD wSuiteMask;
    DWORD dwOperatingSystemVersion;
    } 	COMPUTER_INFO;


EXTERN_C const IID IID_IDebugWindowsComputerPort2;

#if defined(__cplusplus) && !defined(CINTERFACE)
    
    MIDL_INTERFACE("5fbb8ed3-ecdb-412a-bfa3-3a54beb5b2d1")
    IDebugWindowsComputerPort2 : public IUnknown
    {
    public:
        virtual HRESULT STDMETHODCALLTYPE GetComputerInfo( 
            /* [out] */ __RPC__out COMPUTER_INFO *pInfo) = 0;
        
    };
    
#else 	/* C style interface */

    typedef struct IDebugWindowsComputerPort2Vtbl
    {
        BEGIN_INTERFACE
        
        HRESULT ( STDMETHODCALLTYPE *QueryInterface )( 
            IDebugWindowsComputerPort2 * This,
            /* [in] */ __RPC__in REFIID riid,
            /* [iid_is][out] */ 
            __RPC__deref_out  void **ppvObject);
        
        ULONG ( STDMETHODCALLTYPE *AddRef )( 
            IDebugWindowsComputerPort2 * This);
        
        ULONG ( STDMETHODCALLTYPE *Release )( 
            IDebugWindowsComputerPort2 * This);
        
        HRESULT ( STDMETHODCALLTYPE *GetComputerInfo )( 
            IDebugWindowsComputerPort2 * This,
            /* [out] */ __RPC__out COMPUTER_INFO *pInfo);
        
        END_INTERFACE
    } IDebugWindowsComputerPort2Vtbl;

    interface IDebugWindowsComputerPort2
    {
        CONST_VTBL struct IDebugWindowsComputerPort2Vtbl *lpVtbl;
    };

    

#ifdef COBJMACROS


#define IDebugWindowsComputerPort2_QueryInterface(This,riid,ppvObject)	\
    ( (This)->lpVtbl -> QueryInterface(This,riid,ppvObject) ) 

#define IDebugWindowsComputerPort2_AddRef(This)	\
    ( (This)->lpVtbl -> AddRef(This) ) 

#define IDebugWindowsComputerPort2_Release(This)	\
    ( (This)->lpVtbl -> Release(This) ) 


#define IDebugWindowsComputerPort2_GetComputerInfo(This,pInfo)	\
    ( (This)->lpVtbl -> GetComputerInfo(This,pInfo) ) 

#endif /* COBJMACROS */


#endif 	/* C style interface */




#endif 	/* __IDebugWindowsComputerPort2_INTERFACE_DEFINED__ */


#ifndef __IDebugPortRequest2_INTERFACE_DEFINED__
#define __IDebugPortRequest2_INTERFACE_DEFINED__

/* interface IDebugPortRequest2 */
/* [unique][uuid][object] */ 


EXTERN_C const IID IID_IDebugPortRequest2;

#if defined(__cplusplus) && !defined(CINTERFACE)
    
    MIDL_INTERFACE("8d36beb8-9bfe-47dd-a11b-7ba1de18e449")
    IDebugPortRequest2 : public IUnknown
    {
    public:
        virtual HRESULT STDMETHODCALLTYPE GetPortName( 
            /* [out] */ __RPC__deref_out_opt BSTR *pbstrPortName) = 0;
        
    };
    
#else 	/* C style interface */

    typedef struct IDebugPortRequest2Vtbl
    {
        BEGIN_INTERFACE
        
        HRESULT ( STDMETHODCALLTYPE *QueryInterface )( 
            IDebugPortRequest2 * This,
            /* [in] */ __RPC__in REFIID riid,
            /* [iid_is][out] */ 
            __RPC__deref_out  void **ppvObject);
        
        ULONG ( STDMETHODCALLTYPE *AddRef )( 
            IDebugPortRequest2 * This);
        
        ULONG ( STDMETHODCALLTYPE *Release )( 
            IDebugPortRequest2 * This);
        
        HRESULT ( STDMETHODCALLTYPE *GetPortName )( 
            IDebugPortRequest2 * This,
            /* [out] */ __RPC__deref_out_opt BSTR *pbstrPortName);
        
        END_INTERFACE
    } IDebugPortRequest2Vtbl;

    interface IDebugPortRequest2
    {
        CONST_VTBL struct IDebugPortRequest2Vtbl *lpVtbl;
    };

    

#ifdef COBJMACROS


#define IDebugPortRequest2_QueryInterface(This,riid,ppvObject)	\
    ( (This)->lpVtbl -> QueryInterface(This,riid,ppvObject) ) 

#define IDebugPortRequest2_AddRef(This)	\
    ( (This)->lpVtbl -> AddRef(This) ) 

#define IDebugPortRequest2_Release(This)	\
    ( (This)->lpVtbl -> Release(This) ) 


#define IDebugPortRequest2_GetPortName(This,pbstrPortName)	\
    ( (This)->lpVtbl -> GetPortName(This,pbstrPortName) ) 

#endif /* COBJMACROS */


#endif 	/* C style interface */




#endif 	/* __IDebugPortRequest2_INTERFACE_DEFINED__ */


#ifndef __IDebugPortNotify2_INTERFACE_DEFINED__
#define __IDebugPortNotify2_INTERFACE_DEFINED__

/* interface IDebugPortNotify2 */
/* [unique][uuid][object] */ 


EXTERN_C const IID IID_IDebugPortNotify2;

#if defined(__cplusplus) && !defined(CINTERFACE)
    
    MIDL_INTERFACE("fb8d2032-2858-414c-83d9-f732664e0c7a")
    IDebugPortNotify2 : public IUnknown
    {
    public:
        virtual HRESULT STDMETHODCALLTYPE AddProgramNode( 
            /* [in] */ __RPC__in_opt IDebugProgramNode2 *pProgramNode) = 0;
        
        virtual HRESULT STDMETHODCALLTYPE RemoveProgramNode( 
            /* [in] */ __RPC__in_opt IDebugProgramNode2 *pProgramNode) = 0;
        
    };
    
#else 	/* C style interface */

    typedef struct IDebugPortNotify2Vtbl
    {
        BEGIN_INTERFACE
        
        HRESULT ( STDMETHODCALLTYPE *QueryInterface )( 
            IDebugPortNotify2 * This,
            /* [in] */ __RPC__in REFIID riid,
            /* [iid_is][out] */ 
            __RPC__deref_out  void **ppvObject);
        
        ULONG ( STDMETHODCALLTYPE *AddRef )( 
            IDebugPortNotify2 * This);
        
        ULONG ( STDMETHODCALLTYPE *Release )( 
            IDebugPortNotify2 * This);
        
        HRESULT ( STDMETHODCALLTYPE *AddProgramNode )( 
            IDebugPortNotify2 * This,
            /* [in] */ __RPC__in_opt IDebugProgramNode2 *pProgramNode);
        
        HRESULT ( STDMETHODCALLTYPE *RemoveProgramNode )( 
            IDebugPortNotify2 * This,
            /* [in] */ __RPC__in_opt IDebugProgramNode2 *pProgramNode);
        
        END_INTERFACE
    } IDebugPortNotify2Vtbl;

    interface IDebugPortNotify2
    {
        CONST_VTBL struct IDebugPortNotify2Vtbl *lpVtbl;
    };

    

#ifdef COBJMACROS


#define IDebugPortNotify2_QueryInterface(This,riid,ppvObject)	\
    ( (This)->lpVtbl -> QueryInterface(This,riid,ppvObject) ) 

#define IDebugPortNotify2_AddRef(This)	\
    ( (This)->lpVtbl -> AddRef(This) ) 

#define IDebugPortNotify2_Release(This)	\
    ( (This)->lpVtbl -> Release(This) ) 


#define IDebugPortNotify2_AddProgramNode(This,pProgramNode)	\
    ( (This)->lpVtbl -> AddProgramNode(This,pProgramNode) ) 

#define IDebugPortNotify2_RemoveProgramNode(This,pProgramNode)	\
    ( (This)->lpVtbl -> RemoveProgramNode(This,pProgramNode) ) 

#endif /* COBJMACROS */


#endif 	/* C style interface */




#endif 	/* __IDebugPortNotify2_INTERFACE_DEFINED__ */


#ifndef __IDebugPortEvents2_INTERFACE_DEFINED__
#define __IDebugPortEvents2_INTERFACE_DEFINED__

/* interface IDebugPortEvents2 */
/* [unique][uuid][object] */ 


EXTERN_C const IID IID_IDebugPortEvents2;

#if defined(__cplusplus) && !defined(CINTERFACE)
    
    MIDL_INTERFACE("564fa275-12e1-4b5f-8316-4d79bcef7246")
    IDebugPortEvents2 : public IUnknown
    {
    public:
        virtual HRESULT STDMETHODCALLTYPE Event( 
            /* [in] */ __RPC__in_opt IDebugCoreServer2 *pServer,
            /* [in] */ __RPC__in_opt IDebugPort2 *pPort,
            /* [in] */ __RPC__in_opt IDebugProcess2 *pProcess,
            /* [in] */ __RPC__in_opt IDebugProgram2 *pProgram,
            /* [in] */ __RPC__in_opt IDebugEvent2 *pEvent,
            /* [in] */ __RPC__in REFIID riidEvent) = 0;
        
    };
    
#else 	/* C style interface */

    typedef struct IDebugPortEvents2Vtbl
    {
        BEGIN_INTERFACE
        
        HRESULT ( STDMETHODCALLTYPE *QueryInterface )( 
            IDebugPortEvents2 * This,
            /* [in] */ __RPC__in REFIID riid,
            /* [iid_is][out] */ 
            __RPC__deref_out  void **ppvObject);
        
        ULONG ( STDMETHODCALLTYPE *AddRef )( 
            IDebugPortEvents2 * This);
        
        ULONG ( STDMETHODCALLTYPE *Release )( 
            IDebugPortEvents2 * This);
        
        HRESULT ( STDMETHODCALLTYPE *Event )( 
            IDebugPortEvents2 * This,
            /* [in] */ __RPC__in_opt IDebugCoreServer2 *pServer,
            /* [in] */ __RPC__in_opt IDebugPort2 *pPort,
            /* [in] */ __RPC__in_opt IDebugProcess2 *pProcess,
            /* [in] */ __RPC__in_opt IDebugProgram2 *pProgram,
            /* [in] */ __RPC__in_opt IDebugEvent2 *pEvent,
            /* [in] */ __RPC__in REFIID riidEvent);
        
        END_INTERFACE
    } IDebugPortEvents2Vtbl;

    interface IDebugPortEvents2
    {
        CONST_VTBL struct IDebugPortEvents2Vtbl *lpVtbl;
    };

    

#ifdef COBJMACROS


#define IDebugPortEvents2_QueryInterface(This,riid,ppvObject)	\
    ( (This)->lpVtbl -> QueryInterface(This,riid,ppvObject) ) 

#define IDebugPortEvents2_AddRef(This)	\
    ( (This)->lpVtbl -> AddRef(This) ) 

#define IDebugPortEvents2_Release(This)	\
    ( (This)->lpVtbl -> Release(This) ) 


#define IDebugPortEvents2_Event(This,pServer,pPort,pProcess,pProgram,pEvent,riidEvent)	\
    ( (This)->lpVtbl -> Event(This,pServer,pPort,pProcess,pProgram,pEvent,riidEvent) ) 

#endif /* COBJMACROS */


#endif 	/* C style interface */




#endif 	/* __IDebugPortEvents2_INTERFACE_DEFINED__ */


#ifndef __IDebugMDMUtil2_V7_INTERFACE_DEFINED__
#define __IDebugMDMUtil2_V7_INTERFACE_DEFINED__

/* interface IDebugMDMUtil2_V7 */
/* [unique][uuid][object] */ 

typedef DWORD DYNDEBUGFLAGS;


enum enum_DYNDEBUGFLAGS
    {	DYNDEBUG_ATTACH	= 1,
	DYNDEBUG_JIT	= 2,
	DYNDEBUG_REMOTEJIT	= 4
    } ;
#define	S_UNKNOWN	( 0x3 )


EXTERN_C const IID IID_IDebugMDMUtil2_V7;

#if defined(__cplusplus) && !defined(CINTERFACE)
    
    MIDL_INTERFACE("f3062547-43d8-4dc2-b18e-e1460ff2c422")
    IDebugMDMUtil2_V7 : public IUnknown
    {
    public:
        virtual HRESULT STDMETHODCALLTYPE AddPIDToIgnore( 
            /* [in] */ __RPC__in REFGUID guidEngine,
            /* [in] */ DWORD dwPid) = 0;
        
        virtual HRESULT STDMETHODCALLTYPE RemovePIDToIgnore( 
            /* [in] */ __RPC__in REFGUID guidEngine,
            /* [in] */ DWORD dwPid) = 0;
        
        virtual HRESULT STDMETHODCALLTYPE AddPIDToDebug( 
            /* [in] */ __RPC__in REFGUID guidEngine,
            /* [in] */ DWORD dwPid) = 0;
        
        virtual HRESULT STDMETHODCALLTYPE RemovePIDToDebug( 
            /* [in] */ __RPC__in REFGUID guidEngine,
            /* [in] */ DWORD dwPid) = 0;
        
        virtual HRESULT STDMETHODCALLTYPE SetDynamicDebuggingFlags( 
            /* [in] */ __RPC__in REFGUID guidEngine,
            /* [in] */ DYNDEBUGFLAGS dwFlags) = 0;
        
        virtual HRESULT STDMETHODCALLTYPE GetDynamicDebuggingFlags( 
            /* [in] */ __RPC__in REFGUID guidEngine,
            /* [out] */ __RPC__out DYNDEBUGFLAGS *pdwFlags) = 0;
        
        virtual HRESULT STDMETHODCALLTYPE SetDefaultJITServer( 
            /* [in] */ __RPC__in REFCLSID clsidJITServer) = 0;
        
        virtual HRESULT STDMETHODCALLTYPE GetDefaultJITServer( 
            /* [out] */ __RPC__out CLSID *pClsidJITServer) = 0;
        
        virtual HRESULT STDMETHODCALLTYPE RegisterJITDebugEngines( 
            /* [in] */ __RPC__in REFCLSID clsidJITServer,
            /* [size_is][in] */ __RPC__in_ecount_full(celtEngs) GUID *arrguidEngines,
            /* [size_is][full][in] */ __RPC__in_ecount_full_opt(celtEngs) BOOL *arrRemoteFlags,
            /* [in] */ DWORD celtEngs,
            /* [in] */ BOOL fRegister) = 0;
        
        virtual HRESULT STDMETHODCALLTYPE CanDebugPID( 
            /* [in] */ __RPC__in REFGUID guidEngine,
            /* [in] */ DWORD pid) = 0;
        
    };
    
#else 	/* C style interface */

    typedef struct IDebugMDMUtil2_V7Vtbl
    {
        BEGIN_INTERFACE
        
        HRESULT ( STDMETHODCALLTYPE *QueryInterface )( 
            IDebugMDMUtil2_V7 * This,
            /* [in] */ __RPC__in REFIID riid,
            /* [iid_is][out] */ 
            __RPC__deref_out  void **ppvObject);
        
        ULONG ( STDMETHODCALLTYPE *AddRef )( 
            IDebugMDMUtil2_V7 * This);
        
        ULONG ( STDMETHODCALLTYPE *Release )( 
            IDebugMDMUtil2_V7 * This);
        
        HRESULT ( STDMETHODCALLTYPE *AddPIDToIgnore )( 
            IDebugMDMUtil2_V7 * This,
            /* [in] */ __RPC__in REFGUID guidEngine,
            /* [in] */ DWORD dwPid);
        
        HRESULT ( STDMETHODCALLTYPE *RemovePIDToIgnore )( 
            IDebugMDMUtil2_V7 * This,
            /* [in] */ __RPC__in REFGUID guidEngine,
            /* [in] */ DWORD dwPid);
        
        HRESULT ( STDMETHODCALLTYPE *AddPIDToDebug )( 
            IDebugMDMUtil2_V7 * This,
            /* [in] */ __RPC__in REFGUID guidEngine,
            /* [in] */ DWORD dwPid);
        
        HRESULT ( STDMETHODCALLTYPE *RemovePIDToDebug )( 
            IDebugMDMUtil2_V7 * This,
            /* [in] */ __RPC__in REFGUID guidEngine,
            /* [in] */ DWORD dwPid);
        
        HRESULT ( STDMETHODCALLTYPE *SetDynamicDebuggingFlags )( 
            IDebugMDMUtil2_V7 * This,
            /* [in] */ __RPC__in REFGUID guidEngine,
            /* [in] */ DYNDEBUGFLAGS dwFlags);
        
        HRESULT ( STDMETHODCALLTYPE *GetDynamicDebuggingFlags )( 
            IDebugMDMUtil2_V7 * This,
            /* [in] */ __RPC__in REFGUID guidEngine,
            /* [out] */ __RPC__out DYNDEBUGFLAGS *pdwFlags);
        
        HRESULT ( STDMETHODCALLTYPE *SetDefaultJITServer )( 
            IDebugMDMUtil2_V7 * This,
            /* [in] */ __RPC__in REFCLSID clsidJITServer);
        
        HRESULT ( STDMETHODCALLTYPE *GetDefaultJITServer )( 
            IDebugMDMUtil2_V7 * This,
            /* [out] */ __RPC__out CLSID *pClsidJITServer);
        
        HRESULT ( STDMETHODCALLTYPE *RegisterJITDebugEngines )( 
            IDebugMDMUtil2_V7 * This,
            /* [in] */ __RPC__in REFCLSID clsidJITServer,
            /* [size_is][in] */ __RPC__in_ecount_full(celtEngs) GUID *arrguidEngines,
            /* [size_is][full][in] */ __RPC__in_ecount_full_opt(celtEngs) BOOL *arrRemoteFlags,
            /* [in] */ DWORD celtEngs,
            /* [in] */ BOOL fRegister);
        
        HRESULT ( STDMETHODCALLTYPE *CanDebugPID )( 
            IDebugMDMUtil2_V7 * This,
            /* [in] */ __RPC__in REFGUID guidEngine,
            /* [in] */ DWORD pid);
        
        END_INTERFACE
    } IDebugMDMUtil2_V7Vtbl;

    interface IDebugMDMUtil2_V7
    {
        CONST_VTBL struct IDebugMDMUtil2_V7Vtbl *lpVtbl;
    };

    

#ifdef COBJMACROS


#define IDebugMDMUtil2_V7_QueryInterface(This,riid,ppvObject)	\
    ( (This)->lpVtbl -> QueryInterface(This,riid,ppvObject) ) 

#define IDebugMDMUtil2_V7_AddRef(This)	\
    ( (This)->lpVtbl -> AddRef(This) ) 

#define IDebugMDMUtil2_V7_Release(This)	\
    ( (This)->lpVtbl -> Release(This) ) 


#define IDebugMDMUtil2_V7_AddPIDToIgnore(This,guidEngine,dwPid)	\
    ( (This)->lpVtbl -> AddPIDToIgnore(This,guidEngine,dwPid) ) 

#define IDebugMDMUtil2_V7_RemovePIDToIgnore(This,guidEngine,dwPid)	\
    ( (This)->lpVtbl -> RemovePIDToIgnore(This,guidEngine,dwPid) ) 

#define IDebugMDMUtil2_V7_AddPIDToDebug(This,guidEngine,dwPid)	\
    ( (This)->lpVtbl -> AddPIDToDebug(This,guidEngine,dwPid) ) 

#define IDebugMDMUtil2_V7_RemovePIDToDebug(This,guidEngine,dwPid)	\
    ( (This)->lpVtbl -> RemovePIDToDebug(This,guidEngine,dwPid) ) 

#define IDebugMDMUtil2_V7_SetDynamicDebuggingFlags(This,guidEngine,dwFlags)	\
    ( (This)->lpVtbl -> SetDynamicDebuggingFlags(This,guidEngine,dwFlags) ) 

#define IDebugMDMUtil2_V7_GetDynamicDebuggingFlags(This,guidEngine,pdwFlags)	\
    ( (This)->lpVtbl -> GetDynamicDebuggingFlags(This,guidEngine,pdwFlags) ) 

#define IDebugMDMUtil2_V7_SetDefaultJITServer(This,clsidJITServer)	\
    ( (This)->lpVtbl -> SetDefaultJITServer(This,clsidJITServer) ) 

#define IDebugMDMUtil2_V7_GetDefaultJITServer(This,pClsidJITServer)	\
    ( (This)->lpVtbl -> GetDefaultJITServer(This,pClsidJITServer) ) 

#define IDebugMDMUtil2_V7_RegisterJITDebugEngines(This,clsidJITServer,arrguidEngines,arrRemoteFlags,celtEngs,fRegister)	\
    ( (This)->lpVtbl -> RegisterJITDebugEngines(This,clsidJITServer,arrguidEngines,arrRemoteFlags,celtEngs,fRegister) ) 

#define IDebugMDMUtil2_V7_CanDebugPID(This,guidEngine,pid)	\
    ( (This)->lpVtbl -> CanDebugPID(This,guidEngine,pid) ) 

#endif /* COBJMACROS */


#endif 	/* C style interface */




#endif 	/* __IDebugMDMUtil2_V7_INTERFACE_DEFINED__ */


#ifndef __IDebugMDMUtil3_V7_INTERFACE_DEFINED__
#define __IDebugMDMUtil3_V7_INTERFACE_DEFINED__

/* interface IDebugMDMUtil3_V7 */
/* [unique][uuid][object] */ 


enum __MIDL_IDebugMDMUtil3_V7_0001
    {	WEB_DEBUG_ASP_NET	= 0x1
    } ;
typedef DWORD WEB_DEBUG_TYPE;


EXTERN_C const IID IID_IDebugMDMUtil3_V7;

#if defined(__cplusplus) && !defined(CINTERFACE)
    
    MIDL_INTERFACE("af598dea-ce92-443b-a0b5-9992ff660bc4")
    IDebugMDMUtil3_V7 : public IDebugMDMUtil2_V7
    {
    public:
        virtual HRESULT STDMETHODCALLTYPE DiagnoseScriptDebuggingError( 
            /* [in] */ DWORD dwDebuggeeProcessId) = 0;
        
        virtual HRESULT STDMETHODCALLTYPE DiagnoseWebDebuggingError( 
            /* [in] */ WEB_DEBUG_TYPE dwWebType,
            /* [full][in] */ __RPC__in_opt LPCWSTR pszUrl) = 0;
        
        virtual HRESULT STDMETHODCALLTYPE DiagnoseASPDebugging( 
            /* [full][in] */ __RPC__in_opt LPCWSTR szASPUserAccount) = 0;
        
    };
    
#else 	/* C style interface */

    typedef struct IDebugMDMUtil3_V7Vtbl
    {
        BEGIN_INTERFACE
        
        HRESULT ( STDMETHODCALLTYPE *QueryInterface )( 
            IDebugMDMUtil3_V7 * This,
            /* [in] */ __RPC__in REFIID riid,
            /* [iid_is][out] */ 
            __RPC__deref_out  void **ppvObject);
        
        ULONG ( STDMETHODCALLTYPE *AddRef )( 
            IDebugMDMUtil3_V7 * This);
        
        ULONG ( STDMETHODCALLTYPE *Release )( 
            IDebugMDMUtil3_V7 * This);
        
        HRESULT ( STDMETHODCALLTYPE *AddPIDToIgnore )( 
            IDebugMDMUtil3_V7 * This,
            /* [in] */ __RPC__in REFGUID guidEngine,
            /* [in] */ DWORD dwPid);
        
        HRESULT ( STDMETHODCALLTYPE *RemovePIDToIgnore )( 
            IDebugMDMUtil3_V7 * This,
            /* [in] */ __RPC__in REFGUID guidEngine,
            /* [in] */ DWORD dwPid);
        
        HRESULT ( STDMETHODCALLTYPE *AddPIDToDebug )( 
            IDebugMDMUtil3_V7 * This,
            /* [in] */ __RPC__in REFGUID guidEngine,
            /* [in] */ DWORD dwPid);
        
        HRESULT ( STDMETHODCALLTYPE *RemovePIDToDebug )( 
            IDebugMDMUtil3_V7 * This,
            /* [in] */ __RPC__in REFGUID guidEngine,
            /* [in] */ DWORD dwPid);
        
        HRESULT ( STDMETHODCALLTYPE *SetDynamicDebuggingFlags )( 
            IDebugMDMUtil3_V7 * This,
            /* [in] */ __RPC__in REFGUID guidEngine,
            /* [in] */ DYNDEBUGFLAGS dwFlags);
        
        HRESULT ( STDMETHODCALLTYPE *GetDynamicDebuggingFlags )( 
            IDebugMDMUtil3_V7 * This,
            /* [in] */ __RPC__in REFGUID guidEngine,
            /* [out] */ __RPC__out DYNDEBUGFLAGS *pdwFlags);
        
        HRESULT ( STDMETHODCALLTYPE *SetDefaultJITServer )( 
            IDebugMDMUtil3_V7 * This,
            /* [in] */ __RPC__in REFCLSID clsidJITServer);
        
        HRESULT ( STDMETHODCALLTYPE *GetDefaultJITServer )( 
            IDebugMDMUtil3_V7 * This,
            /* [out] */ __RPC__out CLSID *pClsidJITServer);
        
        HRESULT ( STDMETHODCALLTYPE *RegisterJITDebugEngines )( 
            IDebugMDMUtil3_V7 * This,
            /* [in] */ __RPC__in REFCLSID clsidJITServer,
            /* [size_is][in] */ __RPC__in_ecount_full(celtEngs) GUID *arrguidEngines,
            /* [size_is][full][in] */ __RPC__in_ecount_full_opt(celtEngs) BOOL *arrRemoteFlags,
            /* [in] */ DWORD celtEngs,
            /* [in] */ BOOL fRegister);
        
        HRESULT ( STDMETHODCALLTYPE *CanDebugPID )( 
            IDebugMDMUtil3_V7 * This,
            /* [in] */ __RPC__in REFGUID guidEngine,
            /* [in] */ DWORD pid);
        
        HRESULT ( STDMETHODCALLTYPE *DiagnoseScriptDebuggingError )( 
            IDebugMDMUtil3_V7 * This,
            /* [in] */ DWORD dwDebuggeeProcessId);
        
        HRESULT ( STDMETHODCALLTYPE *DiagnoseWebDebuggingError )( 
            IDebugMDMUtil3_V7 * This,
            /* [in] */ WEB_DEBUG_TYPE dwWebType,
            /* [full][in] */ __RPC__in_opt LPCWSTR pszUrl);
        
        HRESULT ( STDMETHODCALLTYPE *DiagnoseASPDebugging )( 
            IDebugMDMUtil3_V7 * This,
            /* [full][in] */ __RPC__in_opt LPCWSTR szASPUserAccount);
        
        END_INTERFACE
    } IDebugMDMUtil3_V7Vtbl;

    interface IDebugMDMUtil3_V7
    {
        CONST_VTBL struct IDebugMDMUtil3_V7Vtbl *lpVtbl;
    };

    

#ifdef COBJMACROS


#define IDebugMDMUtil3_V7_QueryInterface(This,riid,ppvObject)	\
    ( (This)->lpVtbl -> QueryInterface(This,riid,ppvObject) ) 

#define IDebugMDMUtil3_V7_AddRef(This)	\
    ( (This)->lpVtbl -> AddRef(This) ) 

#define IDebugMDMUtil3_V7_Release(This)	\
    ( (This)->lpVtbl -> Release(This) ) 


#define IDebugMDMUtil3_V7_AddPIDToIgnore(This,guidEngine,dwPid)	\
    ( (This)->lpVtbl -> AddPIDToIgnore(This,guidEngine,dwPid) ) 

#define IDebugMDMUtil3_V7_RemovePIDToIgnore(This,guidEngine,dwPid)	\
    ( (This)->lpVtbl -> RemovePIDToIgnore(This,guidEngine,dwPid) ) 

#define IDebugMDMUtil3_V7_AddPIDToDebug(This,guidEngine,dwPid)	\
    ( (This)->lpVtbl -> AddPIDToDebug(This,guidEngine,dwPid) ) 

#define IDebugMDMUtil3_V7_RemovePIDToDebug(This,guidEngine,dwPid)	\
    ( (This)->lpVtbl -> RemovePIDToDebug(This,guidEngine,dwPid) ) 

#define IDebugMDMUtil3_V7_SetDynamicDebuggingFlags(This,guidEngine,dwFlags)	\
    ( (This)->lpVtbl -> SetDynamicDebuggingFlags(This,guidEngine,dwFlags) ) 

#define IDebugMDMUtil3_V7_GetDynamicDebuggingFlags(This,guidEngine,pdwFlags)	\
    ( (This)->lpVtbl -> GetDynamicDebuggingFlags(This,guidEngine,pdwFlags) ) 

#define IDebugMDMUtil3_V7_SetDefaultJITServer(This,clsidJITServer)	\
    ( (This)->lpVtbl -> SetDefaultJITServer(This,clsidJITServer) ) 

#define IDebugMDMUtil3_V7_GetDefaultJITServer(This,pClsidJITServer)	\
    ( (This)->lpVtbl -> GetDefaultJITServer(This,pClsidJITServer) ) 

#define IDebugMDMUtil3_V7_RegisterJITDebugEngines(This,clsidJITServer,arrguidEngines,arrRemoteFlags,celtEngs,fRegister)	\
    ( (This)->lpVtbl -> RegisterJITDebugEngines(This,clsidJITServer,arrguidEngines,arrRemoteFlags,celtEngs,fRegister) ) 

#define IDebugMDMUtil3_V7_CanDebugPID(This,guidEngine,pid)	\
    ( (This)->lpVtbl -> CanDebugPID(This,guidEngine,pid) ) 


#define IDebugMDMUtil3_V7_DiagnoseScriptDebuggingError(This,dwDebuggeeProcessId)	\
    ( (This)->lpVtbl -> DiagnoseScriptDebuggingError(This,dwDebuggeeProcessId) ) 

#define IDebugMDMUtil3_V7_DiagnoseWebDebuggingError(This,dwWebType,pszUrl)	\
    ( (This)->lpVtbl -> DiagnoseWebDebuggingError(This,dwWebType,pszUrl) ) 

#define IDebugMDMUtil3_V7_DiagnoseASPDebugging(This,szASPUserAccount)	\
    ( (This)->lpVtbl -> DiagnoseASPDebugging(This,szASPUserAccount) ) 

#endif /* COBJMACROS */


#endif 	/* C style interface */




#endif 	/* __IDebugMDMUtil3_V7_INTERFACE_DEFINED__ */


#ifndef __IDebugSession2_INTERFACE_DEFINED__
#define __IDebugSession2_INTERFACE_DEFINED__

/* interface IDebugSession2 */
/* [unique][uuid][object] */ 


enum enum_LAUNCH_FLAGS
    {	LAUNCH_DEBUG	= 0,
	LAUNCH_NODEBUG	= 0x1,
	LAUNCH_ENABLE_ENC	= 0x2,
	LAUNCH_MERGE_ENV	= 0x4
    } ;
typedef DWORD LAUNCH_FLAGS;


enum enum_EXCEPTION_STATE
    {	EXCEPTION_NONE	= 0,
	EXCEPTION_STOP_FIRST_CHANCE	= 0x1,
	EXCEPTION_STOP_SECOND_CHANCE	= 0x2,
	EXCEPTION_STOP_USER_FIRST_CHANCE	= 0x10,
	EXCEPTION_STOP_USER_UNCAUGHT	= 0x20,
	EXCEPTION_STOP_FIRST_CHANCE_USE_PARENT	= 0x4,
	EXCEPTION_STOP_SECOND_CHANCE_USE_PARENT	= 0x8,
	EXCEPTION_STOP_USER_FIRST_CHANCE_USE_PARENT	= 0x40,
	EXCEPTION_STOP_USER_UNCAUGHT_USE_PARENT	= 0x80,
	EXCEPTION_STOP_ALL	= 0xff,
	EXCEPTION_CANNOT_BE_CONTINUED	= 0x100,
	EXCEPTION_CODE_SUPPORTED	= 0x1000,
	EXCEPTION_CODE_DISPLAY_IN_HEX	= 0x2000,
	EXCEPTION_JUST_MY_CODE_SUPPORTED	= 0x4000,
	EXCEPTION_MANAGED_DEBUG_ASSISTANT	= 0x8000
    } ;
typedef DWORD EXCEPTION_STATE;

typedef struct tagEXCEPTION_INFO
    {
    IDebugProgram2 *pProgram;
    BSTR bstrProgramName;
    BSTR bstrExceptionName;
    DWORD dwCode;
    EXCEPTION_STATE dwState;
    GUID guidType;
    } 	EXCEPTION_INFO;


enum enum_STOPPING_MODEL
    {	STOPPING_MODEL_STOP_ALL	= 0x1,
	STOPPING_MODEL_STOP_ONE	= 0x2
    } ;
typedef DWORD STOPPING_MODEL;


EXTERN_C const IID IID_IDebugSession2;

#if defined(__cplusplus) && !defined(CINTERFACE)
    
    MIDL_INTERFACE("8948300f-8bd5-4728-a1d8-83d172295a9d")
    IDebugSession2 : public IUnknown
    {
    public:
        virtual HRESULT STDMETHODCALLTYPE GetName( 
            /* [out] */ __RPC__deref_out_opt BSTR *pbstrName) = 0;
        
        virtual HRESULT STDMETHODCALLTYPE SetName( 
            /* [in] */ __RPC__in LPCOLESTR pszName) = 0;
        
        virtual HRESULT STDMETHODCALLTYPE EnumProcesses( 
            /* [out] */ __RPC__deref_out_opt IEnumDebugProcesses2 **ppEnum) = 0;
        
        virtual HRESULT STDMETHODCALLTYPE Launch( 
            /* [full][in] */ __RPC__in_opt LPCOLESTR pszMachine,
            /* [in] */ __RPC__in_opt IDebugPort2 *pPort,
            /* [full][in] */ __RPC__in_opt LPCOLESTR pszExe,
            /* [full][in] */ __RPC__in_opt LPCOLESTR pszArgs,
            /* [full][in] */ __RPC__in_opt LPCOLESTR pszDir,
            /* [full][in] */ __RPC__in_opt BSTR bstrEnv,
            /* [full][in] */ __RPC__in_opt LPCOLESTR pszOptions,
            /* [in] */ LAUNCH_FLAGS dwLaunchFlags,
            /* [in] */ DWORD hStdInput,
            /* [in] */ DWORD hStdOutput,
            /* [in] */ DWORD hStdError,
            /* [in] */ __RPC__in REFGUID guidLaunchingEngine,
            /* [in] */ __RPC__in_opt IDebugEventCallback2 *pCallback,
            /* [size_is][in] */ __RPC__in_ecount_full(celtSpecificEngines) GUID *rgguidSpecificEngines,
            /* [in] */ DWORD celtSpecificEngines,
            /* [out] */ __RPC__deref_out_opt IDebugProcess2 **ppProcess) = 0;
        
        virtual HRESULT STDMETHODCALLTYPE RegisterJITServer( 
            /* [in] */ __RPC__in REFCLSID clsidJITServer) = 0;
        
        virtual HRESULT STDMETHODCALLTYPE Terminate( 
            /* [in] */ BOOL fForce) = 0;
        
        virtual HRESULT STDMETHODCALLTYPE Detach( void) = 0;
        
        virtual HRESULT STDMETHODCALLTYPE CauseBreak( void) = 0;
        
        virtual HRESULT STDMETHODCALLTYPE CreatePendingBreakpoint( 
            /* [in] */ __RPC__in_opt IDebugBreakpointRequest2 *pBPRequest,
            /* [out] */ __RPC__deref_out_opt IDebugPendingBreakpoint2 **ppPendingBP) = 0;
        
        virtual HRESULT STDMETHODCALLTYPE EnumPendingBreakpoints( 
            /* [in] */ __RPC__in_opt IDebugProgram2 *pProgram,
            /* [full][in] */ __RPC__in_opt LPCOLESTR pszProgram,
            /* [out] */ __RPC__deref_out_opt IEnumDebugPendingBreakpoints2 **ppEnumBPs) = 0;
        
        virtual HRESULT STDMETHODCALLTYPE EnumMachines__deprecated( 
            /* [out] */ __RPC__deref_out_opt IEnumDebugMachines2__deprecated **ppEnum) = 0;
        
        virtual HRESULT STDMETHODCALLTYPE ConnectToServer( 
            /* [full][in] */ __RPC__in_opt LPCOLESTR szServerName,
            /* [out] */ __RPC__deref_out_opt IDebugCoreServer2 **ppServer) = 0;
        
        virtual HRESULT STDMETHODCALLTYPE DisconnectServer( 
            /* [in] */ __RPC__in_opt IDebugCoreServer2 *pServer) = 0;
        
        virtual HRESULT STDMETHODCALLTYPE ShutdownSession( void) = 0;
        
        virtual HRESULT STDMETHODCALLTYPE EnumCodeContexts( 
            /* [in] */ __RPC__in_opt IDebugProgram2 *pProgram,
            /* [in] */ __RPC__in_opt IDebugDocumentPosition2 *pDocPos,
            /* [out] */ __RPC__deref_out_opt IEnumDebugCodeContexts2 **ppEnum) = 0;
        
        virtual HRESULT STDMETHODCALLTYPE SetException( 
            /* [in] */ __RPC__in EXCEPTION_INFO *pException) = 0;
        
        virtual HRESULT STDMETHODCALLTYPE EnumSetExceptions( 
            /* [in] */ __RPC__in_opt IDebugProgram2 *pProgram,
            /* [full][in] */ __RPC__in_opt LPCOLESTR pszProgram,
            /* [in] */ __RPC__in REFGUID guidType,
            /* [out] */ __RPC__deref_out_opt IEnumDebugExceptionInfo2 **ppEnum) = 0;
        
        virtual HRESULT STDMETHODCALLTYPE RemoveSetException( 
            /* [in] */ __RPC__in EXCEPTION_INFO *pException) = 0;
        
        virtual HRESULT STDMETHODCALLTYPE RemoveAllSetExceptions( 
            /* [in] */ __RPC__in REFGUID guidType) = 0;
        
        virtual HRESULT STDMETHODCALLTYPE EnumDefaultExceptions( 
            /* [full][in] */ __RPC__in_opt EXCEPTION_INFO *pParentException,
            /* [out] */ __RPC__deref_out_opt IEnumDebugExceptionInfo2 **ppEnum) = 0;
        
        virtual HRESULT STDMETHODCALLTYPE GetENCUpdate( 
            /* [in] */ __RPC__in_opt IDebugProgram2 *pProgram,
            /* [out] */ __RPC__deref_out_opt IDebugENCUpdate **ppUpdate) = 0;
        
        virtual HRESULT STDMETHODCALLTYPE SetLocale( 
            /* [in] */ WORD wLangID) = 0;
        
        virtual HRESULT STDMETHODCALLTYPE SetRegistryRoot( 
            /* [full][in] */ __RPC__in_opt LPCOLESTR pszRegistryRoot) = 0;
        
        virtual HRESULT STDMETHODCALLTYPE IsAlive( void) = 0;
        
        virtual HRESULT STDMETHODCALLTYPE ClearAllSessionThreadStackFrames( void) = 0;
        
        virtual HRESULT STDMETHODCALLTYPE __deprecated_GetSessionId( 
            /* [in] */ __RPC__in_opt IDebugEventCallback2 *pCallback,
            /* [full][size_is][in] */ __RPC__in_ecount_full_opt(celtSpecificEngines) GUID *rgguidSpecificEngines,
            /* [in] */ DWORD celtSpecificEngines,
            /* [full][in] */ __RPC__in_opt LPCOLESTR pszStartPageUrl,
            /* [out] */ __RPC__deref_out_opt BSTR *pbstrSessionId) = 0;
        
        virtual HRESULT STDMETHODCALLTYPE SetEngineMetric( 
            /* [in] */ __RPC__in REFGUID guidEngine,
            /* [in] */ __RPC__in LPCOLESTR pszMetric,
            /* [in] */ VARIANT varValue) = 0;
        
        virtual HRESULT STDMETHODCALLTYPE SetStoppingModel( 
            /* [in] */ STOPPING_MODEL dwStoppingModel) = 0;
        
        virtual HRESULT STDMETHODCALLTYPE GetStoppingModel( 
            /* [out] */ __RPC__out STOPPING_MODEL *pdwStoppingModel) = 0;
        
        virtual HRESULT STDMETHODCALLTYPE __deprecated_RegisterSessionWithServer( 
            /* [in] */ __RPC__in LPCOLESTR pwszServerName) = 0;
        
    };
    
#else 	/* C style interface */

    typedef struct IDebugSession2Vtbl
    {
        BEGIN_INTERFACE
        
        HRESULT ( STDMETHODCALLTYPE *QueryInterface )( 
            IDebugSession2 * This,
            /* [in] */ __RPC__in REFIID riid,
            /* [iid_is][out] */ 
            __RPC__deref_out  void **ppvObject);
        
        ULONG ( STDMETHODCALLTYPE *AddRef )( 
            IDebugSession2 * This);
        
        ULONG ( STDMETHODCALLTYPE *Release )( 
            IDebugSession2 * This);
        
        HRESULT ( STDMETHODCALLTYPE *GetName )( 
            IDebugSession2 * This,
            /* [out] */ __RPC__deref_out_opt BSTR *pbstrName);
        
        HRESULT ( STDMETHODCALLTYPE *SetName )( 
            IDebugSession2 * This,
            /* [in] */ __RPC__in LPCOLESTR pszName);
        
        HRESULT ( STDMETHODCALLTYPE *EnumProcesses )( 
            IDebugSession2 * This,
            /* [out] */ __RPC__deref_out_opt IEnumDebugProcesses2 **ppEnum);
        
        HRESULT ( STDMETHODCALLTYPE *Launch )( 
            IDebugSession2 * This,
            /* [full][in] */ __RPC__in_opt LPCOLESTR pszMachine,
            /* [in] */ __RPC__in_opt IDebugPort2 *pPort,
            /* [full][in] */ __RPC__in_opt LPCOLESTR pszExe,
            /* [full][in] */ __RPC__in_opt LPCOLESTR pszArgs,
            /* [full][in] */ __RPC__in_opt LPCOLESTR pszDir,
            /* [full][in] */ __RPC__in_opt BSTR bstrEnv,
            /* [full][in] */ __RPC__in_opt LPCOLESTR pszOptions,
            /* [in] */ LAUNCH_FLAGS dwLaunchFlags,
            /* [in] */ DWORD hStdInput,
            /* [in] */ DWORD hStdOutput,
            /* [in] */ DWORD hStdError,
            /* [in] */ __RPC__in REFGUID guidLaunchingEngine,
            /* [in] */ __RPC__in_opt IDebugEventCallback2 *pCallback,
            /* [size_is][in] */ __RPC__in_ecount_full(celtSpecificEngines) GUID *rgguidSpecificEngines,
            /* [in] */ DWORD celtSpecificEngines,
            /* [out] */ __RPC__deref_out_opt IDebugProcess2 **ppProcess);
        
        HRESULT ( STDMETHODCALLTYPE *RegisterJITServer )( 
            IDebugSession2 * This,
            /* [in] */ __RPC__in REFCLSID clsidJITServer);
        
        HRESULT ( STDMETHODCALLTYPE *Terminate )( 
            IDebugSession2 * This,
            /* [in] */ BOOL fForce);
        
        HRESULT ( STDMETHODCALLTYPE *Detach )( 
            IDebugSession2 * This);
        
        HRESULT ( STDMETHODCALLTYPE *CauseBreak )( 
            IDebugSession2 * This);
        
        HRESULT ( STDMETHODCALLTYPE *CreatePendingBreakpoint )( 
            IDebugSession2 * This,
            /* [in] */ __RPC__in_opt IDebugBreakpointRequest2 *pBPRequest,
            /* [out] */ __RPC__deref_out_opt IDebugPendingBreakpoint2 **ppPendingBP);
        
        HRESULT ( STDMETHODCALLTYPE *EnumPendingBreakpoints )( 
            IDebugSession2 * This,
            /* [in] */ __RPC__in_opt IDebugProgram2 *pProgram,
            /* [full][in] */ __RPC__in_opt LPCOLESTR pszProgram,
            /* [out] */ __RPC__deref_out_opt IEnumDebugPendingBreakpoints2 **ppEnumBPs);
        
        HRESULT ( STDMETHODCALLTYPE *EnumMachines__deprecated )( 
            IDebugSession2 * This,
            /* [out] */ __RPC__deref_out_opt IEnumDebugMachines2__deprecated **ppEnum);
        
        HRESULT ( STDMETHODCALLTYPE *ConnectToServer )( 
            IDebugSession2 * This,
            /* [full][in] */ __RPC__in_opt LPCOLESTR szServerName,
            /* [out] */ __RPC__deref_out_opt IDebugCoreServer2 **ppServer);
        
        HRESULT ( STDMETHODCALLTYPE *DisconnectServer )( 
            IDebugSession2 * This,
            /* [in] */ __RPC__in_opt IDebugCoreServer2 *pServer);
        
        HRESULT ( STDMETHODCALLTYPE *ShutdownSession )( 
            IDebugSession2 * This);
        
        HRESULT ( STDMETHODCALLTYPE *EnumCodeContexts )( 
            IDebugSession2 * This,
            /* [in] */ __RPC__in_opt IDebugProgram2 *pProgram,
            /* [in] */ __RPC__in_opt IDebugDocumentPosition2 *pDocPos,
            /* [out] */ __RPC__deref_out_opt IEnumDebugCodeContexts2 **ppEnum);
        
        HRESULT ( STDMETHODCALLTYPE *SetException )( 
            IDebugSession2 * This,
            /* [in] */ __RPC__in EXCEPTION_INFO *pException);
        
        HRESULT ( STDMETHODCALLTYPE *EnumSetExceptions )( 
            IDebugSession2 * This,
            /* [in] */ __RPC__in_opt IDebugProgram2 *pProgram,
            /* [full][in] */ __RPC__in_opt LPCOLESTR pszProgram,
            /* [in] */ __RPC__in REFGUID guidType,
            /* [out] */ __RPC__deref_out_opt IEnumDebugExceptionInfo2 **ppEnum);
        
        HRESULT ( STDMETHODCALLTYPE *RemoveSetException )( 
            IDebugSession2 * This,
            /* [in] */ __RPC__in EXCEPTION_INFO *pException);
        
        HRESULT ( STDMETHODCALLTYPE *RemoveAllSetExceptions )( 
            IDebugSession2 * This,
            /* [in] */ __RPC__in REFGUID guidType);
        
        HRESULT ( STDMETHODCALLTYPE *EnumDefaultExceptions )( 
            IDebugSession2 * This,
            /* [full][in] */ __RPC__in_opt EXCEPTION_INFO *pParentException,
            /* [out] */ __RPC__deref_out_opt IEnumDebugExceptionInfo2 **ppEnum);
        
        HRESULT ( STDMETHODCALLTYPE *GetENCUpdate )( 
            IDebugSession2 * This,
            /* [in] */ __RPC__in_opt IDebugProgram2 *pProgram,
            /* [out] */ __RPC__deref_out_opt IDebugENCUpdate **ppUpdate);
        
        HRESULT ( STDMETHODCALLTYPE *SetLocale )( 
            IDebugSession2 * This,
            /* [in] */ WORD wLangID);
        
        HRESULT ( STDMETHODCALLTYPE *SetRegistryRoot )( 
            IDebugSession2 * This,
            /* [full][in] */ __RPC__in_opt LPCOLESTR pszRegistryRoot);
        
        HRESULT ( STDMETHODCALLTYPE *IsAlive )( 
            IDebugSession2 * This);
        
        HRESULT ( STDMETHODCALLTYPE *ClearAllSessionThreadStackFrames )( 
            IDebugSession2 * This);
        
        HRESULT ( STDMETHODCALLTYPE *__deprecated_GetSessionId )( 
            IDebugSession2 * This,
            /* [in] */ __RPC__in_opt IDebugEventCallback2 *pCallback,
            /* [full][size_is][in] */ __RPC__in_ecount_full_opt(celtSpecificEngines) GUID *rgguidSpecificEngines,
            /* [in] */ DWORD celtSpecificEngines,
            /* [full][in] */ __RPC__in_opt LPCOLESTR pszStartPageUrl,
            /* [out] */ __RPC__deref_out_opt BSTR *pbstrSessionId);
        
        HRESULT ( STDMETHODCALLTYPE *SetEngineMetric )( 
            IDebugSession2 * This,
            /* [in] */ __RPC__in REFGUID guidEngine,
            /* [in] */ __RPC__in LPCOLESTR pszMetric,
            /* [in] */ VARIANT varValue);
        
        HRESULT ( STDMETHODCALLTYPE *SetStoppingModel )( 
            IDebugSession2 * This,
            /* [in] */ STOPPING_MODEL dwStoppingModel);
        
        HRESULT ( STDMETHODCALLTYPE *GetStoppingModel )( 
            IDebugSession2 * This,
            /* [out] */ __RPC__out STOPPING_MODEL *pdwStoppingModel);
        
        HRESULT ( STDMETHODCALLTYPE *__deprecated_RegisterSessionWithServer )( 
            IDebugSession2 * This,
            /* [in] */ __RPC__in LPCOLESTR pwszServerName);
        
        END_INTERFACE
    } IDebugSession2Vtbl;

    interface IDebugSession2
    {
        CONST_VTBL struct IDebugSession2Vtbl *lpVtbl;
    };

    

#ifdef COBJMACROS


#define IDebugSession2_QueryInterface(This,riid,ppvObject)	\
    ( (This)->lpVtbl -> QueryInterface(This,riid,ppvObject) ) 

#define IDebugSession2_AddRef(This)	\
    ( (This)->lpVtbl -> AddRef(This) ) 

#define IDebugSession2_Release(This)	\
    ( (This)->lpVtbl -> Release(This) ) 


#define IDebugSession2_GetName(This,pbstrName)	\
    ( (This)->lpVtbl -> GetName(This,pbstrName) ) 

#define IDebugSession2_SetName(This,pszName)	\
    ( (This)->lpVtbl -> SetName(This,pszName) ) 

#define IDebugSession2_EnumProcesses(This,ppEnum)	\
    ( (This)->lpVtbl -> EnumProcesses(This,ppEnum) ) 

#define IDebugSession2_Launch(This,pszMachine,pPort,pszExe,pszArgs,pszDir,bstrEnv,pszOptions,dwLaunchFlags,hStdInput,hStdOutput,hStdError,guidLaunchingEngine,pCallback,rgguidSpecificEngines,celtSpecificEngines,ppProcess)	\
    ( (This)->lpVtbl -> Launch(This,pszMachine,pPort,pszExe,pszArgs,pszDir,bstrEnv,pszOptions,dwLaunchFlags,hStdInput,hStdOutput,hStdError,guidLaunchingEngine,pCallback,rgguidSpecificEngines,celtSpecificEngines,ppProcess) ) 

#define IDebugSession2_RegisterJITServer(This,clsidJITServer)	\
    ( (This)->lpVtbl -> RegisterJITServer(This,clsidJITServer) ) 

#define IDebugSession2_Terminate(This,fForce)	\
    ( (This)->lpVtbl -> Terminate(This,fForce) ) 

#define IDebugSession2_Detach(This)	\
    ( (This)->lpVtbl -> Detach(This) ) 

#define IDebugSession2_CauseBreak(This)	\
    ( (This)->lpVtbl -> CauseBreak(This) ) 

#define IDebugSession2_CreatePendingBreakpoint(This,pBPRequest,ppPendingBP)	\
    ( (This)->lpVtbl -> CreatePendingBreakpoint(This,pBPRequest,ppPendingBP) ) 

#define IDebugSession2_EnumPendingBreakpoints(This,pProgram,pszProgram,ppEnumBPs)	\
    ( (This)->lpVtbl -> EnumPendingBreakpoints(This,pProgram,pszProgram,ppEnumBPs) ) 

#define IDebugSession2_EnumMachines__deprecated(This,ppEnum)	\
    ( (This)->lpVtbl -> EnumMachines__deprecated(This,ppEnum) ) 

#define IDebugSession2_ConnectToServer(This,szServerName,ppServer)	\
    ( (This)->lpVtbl -> ConnectToServer(This,szServerName,ppServer) ) 

#define IDebugSession2_DisconnectServer(This,pServer)	\
    ( (This)->lpVtbl -> DisconnectServer(This,pServer) ) 

#define IDebugSession2_ShutdownSession(This)	\
    ( (This)->lpVtbl -> ShutdownSession(This) ) 

#define IDebugSession2_EnumCodeContexts(This,pProgram,pDocPos,ppEnum)	\
    ( (This)->lpVtbl -> EnumCodeContexts(This,pProgram,pDocPos,ppEnum) ) 

#define IDebugSession2_SetException(This,pException)	\
    ( (This)->lpVtbl -> SetException(This,pException) ) 

#define IDebugSession2_EnumSetExceptions(This,pProgram,pszProgram,guidType,ppEnum)	\
    ( (This)->lpVtbl -> EnumSetExceptions(This,pProgram,pszProgram,guidType,ppEnum) ) 

#define IDebugSession2_RemoveSetException(This,pException)	\
    ( (This)->lpVtbl -> RemoveSetException(This,pException) ) 

#define IDebugSession2_RemoveAllSetExceptions(This,guidType)	\
    ( (This)->lpVtbl -> RemoveAllSetExceptions(This,guidType) ) 

#define IDebugSession2_EnumDefaultExceptions(This,pParentException,ppEnum)	\
    ( (This)->lpVtbl -> EnumDefaultExceptions(This,pParentException,ppEnum) ) 

#define IDebugSession2_GetENCUpdate(This,pProgram,ppUpdate)	\
    ( (This)->lpVtbl -> GetENCUpdate(This,pProgram,ppUpdate) ) 

#define IDebugSession2_SetLocale(This,wLangID)	\
    ( (This)->lpVtbl -> SetLocale(This,wLangID) ) 

#define IDebugSession2_SetRegistryRoot(This,pszRegistryRoot)	\
    ( (This)->lpVtbl -> SetRegistryRoot(This,pszRegistryRoot) ) 

#define IDebugSession2_IsAlive(This)	\
    ( (This)->lpVtbl -> IsAlive(This) ) 

#define IDebugSession2_ClearAllSessionThreadStackFrames(This)	\
    ( (This)->lpVtbl -> ClearAllSessionThreadStackFrames(This) ) 

#define IDebugSession2___deprecated_GetSessionId(This,pCallback,rgguidSpecificEngines,celtSpecificEngines,pszStartPageUrl,pbstrSessionId)	\
    ( (This)->lpVtbl -> __deprecated_GetSessionId(This,pCallback,rgguidSpecificEngines,celtSpecificEngines,pszStartPageUrl,pbstrSessionId) ) 

#define IDebugSession2_SetEngineMetric(This,guidEngine,pszMetric,varValue)	\
    ( (This)->lpVtbl -> SetEngineMetric(This,guidEngine,pszMetric,varValue) ) 

#define IDebugSession2_SetStoppingModel(This,dwStoppingModel)	\
    ( (This)->lpVtbl -> SetStoppingModel(This,dwStoppingModel) ) 

#define IDebugSession2_GetStoppingModel(This,pdwStoppingModel)	\
    ( (This)->lpVtbl -> GetStoppingModel(This,pdwStoppingModel) ) 

#define IDebugSession2___deprecated_RegisterSessionWithServer(This,pwszServerName)	\
    ( (This)->lpVtbl -> __deprecated_RegisterSessionWithServer(This,pwszServerName) ) 

#endif /* COBJMACROS */


#endif 	/* C style interface */




#endif 	/* __IDebugSession2_INTERFACE_DEFINED__ */


/* interface __MIDL_itf_msdbg_0000_0016 */
/* [local] */ 

typedef struct _JMC_CODE_SPEC
    {
    BOOL fIsUserCode;
    BSTR bstrModuleName;
    } 	JMC_CODE_SPEC;



extern RPC_IF_HANDLE __MIDL_itf_msdbg_0000_0016_v0_0_c_ifspec;
extern RPC_IF_HANDLE __MIDL_itf_msdbg_0000_0016_v0_0_s_ifspec;

#ifndef __IDebugSession3_INTERFACE_DEFINED__
#define __IDebugSession3_INTERFACE_DEFINED__

/* interface IDebugSession3 */
/* [unique][uuid][object] */ 

typedef /* [public][public] */ 
enum __MIDL_IDebugSession3_0001
    {	CONNECT_LOCAL	= 0,
	CONNECT_ATTACH	= ( CONNECT_LOCAL + 1 ) ,
	CONNECT_LAUNCH	= ( CONNECT_ATTACH + 1 ) ,
	CONNECT_WEB_AUTO_ATTACH	= ( CONNECT_LAUNCH + 1 ) ,
	CONNECT_SQL_AUTO_ATTACH	= ( CONNECT_WEB_AUTO_ATTACH + 1 ) ,
	CONNECT_CAUSALITY	= ( CONNECT_SQL_AUTO_ATTACH + 1 ) ,
	CONNECT_DIAGNOSE_WEB_ERROR	= ( CONNECT_CAUSALITY + 1 ) 
    } 	CONNECT_REASON;


enum enum_SESSION_FEATURES
    {	FEATURE_REMOTE_DEBUGGING	= 0x1,
	FEATURE_CAUSALITY	= 0x2
    } ;
typedef DWORD SESSION_FEATURES;

typedef DWORD LOAD_SYMBOLS_FLAGS;


EXTERN_C const IID IID_IDebugSession3;

#if defined(__cplusplus) && !defined(CINTERFACE)
    
    MIDL_INTERFACE("BBF74DB9-39D5-406e-8BC3-3BA9DD34C02E")
    IDebugSession3 : public IDebugSession2
    {
    public:
        virtual HRESULT STDMETHODCALLTYPE SetSymbolPath( 
            /* [full][in] */ __RPC__in_opt LPCOLESTR szSymbolSearchPath,
            /* [full][in] */ __RPC__in_opt LPCOLESTR szSymbolCachePath,
            /* [in] */ LOAD_SYMBOLS_FLAGS Flags) = 0;
        
        virtual HRESULT STDMETHODCALLTYPE LoadSymbols( void) = 0;
        
        virtual HRESULT STDMETHODCALLTYPE RegisterCallback( 
            /* [in] */ __RPC__in_opt IDebugEventCallback2 *pCallback) = 0;
        
        virtual HRESULT STDMETHODCALLTYPE ConnectToServerEx( 
            /* [full][in] */ __RPC__in_opt LPCOLESTR szServerName,
            /* [in] */ CONNECT_REASON ConnectReason,
            /* [out] */ __RPC__deref_out_opt IDebugCoreServer3 **ppServer) = 0;
        
        virtual HRESULT STDMETHODCALLTYPE SetJustMyCodeState( 
            /* [in] */ BOOL fUpdate,
            /* [in] */ DWORD dwModules,
            /* [full][size_is][in] */ __RPC__in_ecount_full_opt(dwModules) JMC_CODE_SPEC *rgJMCSpec) = 0;
        
        virtual HRESULT STDMETHODCALLTYPE GetRecentServerNames( 
            /* [out] */ __RPC__out BSTR_ARRAY *pServers) = 0;
        
        virtual HRESULT STDMETHODCALLTYPE SetMaxRecentServerNames( 
            /* [in] */ DWORD dwNewMax) = 0;
        
        virtual HRESULT STDMETHODCALLTYPE InitializeFeatures( 
            /* [in] */ SESSION_FEATURES EnabledFeatures) = 0;
        
        virtual HRESULT STDMETHODCALLTYPE SetAllExceptions( 
            /* [in] */ EXCEPTION_STATE dwState) = 0;
        
        virtual HRESULT STDMETHODCALLTYPE GetStateForAllExceptions( 
            /* [out] */ __RPC__out EXCEPTION_STATE *pdwState) = 0;
        
        virtual HRESULT STDMETHODCALLTYPE AddExceptionCallback( 
            /* [in] */ __RPC__in EXCEPTION_INFO *pException,
            /* [in] */ __RPC__in_opt IDebugExceptionCallback2 *pCallback) = 0;
        
        virtual HRESULT STDMETHODCALLTYPE RemoveExceptionCallback( 
            /* [in] */ __RPC__in EXCEPTION_INFO *pException,
            /* [in] */ __RPC__in_opt IDebugExceptionCallback2 *pCallback) = 0;
        
        virtual HRESULT STDMETHODCALLTYPE BlockingShutdownSession( 
            /* [in] */ DWORD dwTimeout) = 0;
        
    };
    
#else 	/* C style interface */

    typedef struct IDebugSession3Vtbl
    {
        BEGIN_INTERFACE
        
        HRESULT ( STDMETHODCALLTYPE *QueryInterface )( 
            IDebugSession3 * This,
            /* [in] */ __RPC__in REFIID riid,
            /* [iid_is][out] */ 
            __RPC__deref_out  void **ppvObject);
        
        ULONG ( STDMETHODCALLTYPE *AddRef )( 
            IDebugSession3 * This);
        
        ULONG ( STDMETHODCALLTYPE *Release )( 
            IDebugSession3 * This);
        
        HRESULT ( STDMETHODCALLTYPE *GetName )( 
            IDebugSession3 * This,
            /* [out] */ __RPC__deref_out_opt BSTR *pbstrName);
        
        HRESULT ( STDMETHODCALLTYPE *SetName )( 
            IDebugSession3 * This,
            /* [in] */ __RPC__in LPCOLESTR pszName);
        
        HRESULT ( STDMETHODCALLTYPE *EnumProcesses )( 
            IDebugSession3 * This,
            /* [out] */ __RPC__deref_out_opt IEnumDebugProcesses2 **ppEnum);
        
        HRESULT ( STDMETHODCALLTYPE *Launch )( 
            IDebugSession3 * This,
            /* [full][in] */ __RPC__in_opt LPCOLESTR pszMachine,
            /* [in] */ __RPC__in_opt IDebugPort2 *pPort,
            /* [full][in] */ __RPC__in_opt LPCOLESTR pszExe,
            /* [full][in] */ __RPC__in_opt LPCOLESTR pszArgs,
            /* [full][in] */ __RPC__in_opt LPCOLESTR pszDir,
            /* [full][in] */ __RPC__in_opt BSTR bstrEnv,
            /* [full][in] */ __RPC__in_opt LPCOLESTR pszOptions,
            /* [in] */ LAUNCH_FLAGS dwLaunchFlags,
            /* [in] */ DWORD hStdInput,
            /* [in] */ DWORD hStdOutput,
            /* [in] */ DWORD hStdError,
            /* [in] */ __RPC__in REFGUID guidLaunchingEngine,
            /* [in] */ __RPC__in_opt IDebugEventCallback2 *pCallback,
            /* [size_is][in] */ __RPC__in_ecount_full(celtSpecificEngines) GUID *rgguidSpecificEngines,
            /* [in] */ DWORD celtSpecificEngines,
            /* [out] */ __RPC__deref_out_opt IDebugProcess2 **ppProcess);
        
        HRESULT ( STDMETHODCALLTYPE *RegisterJITServer )( 
            IDebugSession3 * This,
            /* [in] */ __RPC__in REFCLSID clsidJITServer);
        
        HRESULT ( STDMETHODCALLTYPE *Terminate )( 
            IDebugSession3 * This,
            /* [in] */ BOOL fForce);
        
        HRESULT ( STDMETHODCALLTYPE *Detach )( 
            IDebugSession3 * This);
        
        HRESULT ( STDMETHODCALLTYPE *CauseBreak )( 
            IDebugSession3 * This);
        
        HRESULT ( STDMETHODCALLTYPE *CreatePendingBreakpoint )( 
            IDebugSession3 * This,
            /* [in] */ __RPC__in_opt IDebugBreakpointRequest2 *pBPRequest,
            /* [out] */ __RPC__deref_out_opt IDebugPendingBreakpoint2 **ppPendingBP);
        
        HRESULT ( STDMETHODCALLTYPE *EnumPendingBreakpoints )( 
            IDebugSession3 * This,
            /* [in] */ __RPC__in_opt IDebugProgram2 *pProgram,
            /* [full][in] */ __RPC__in_opt LPCOLESTR pszProgram,
            /* [out] */ __RPC__deref_out_opt IEnumDebugPendingBreakpoints2 **ppEnumBPs);
        
        HRESULT ( STDMETHODCALLTYPE *EnumMachines__deprecated )( 
            IDebugSession3 * This,
            /* [out] */ __RPC__deref_out_opt IEnumDebugMachines2__deprecated **ppEnum);
        
        HRESULT ( STDMETHODCALLTYPE *ConnectToServer )( 
            IDebugSession3 * This,
            /* [full][in] */ __RPC__in_opt LPCOLESTR szServerName,
            /* [out] */ __RPC__deref_out_opt IDebugCoreServer2 **ppServer);
        
        HRESULT ( STDMETHODCALLTYPE *DisconnectServer )( 
            IDebugSession3 * This,
            /* [in] */ __RPC__in_opt IDebugCoreServer2 *pServer);
        
        HRESULT ( STDMETHODCALLTYPE *ShutdownSession )( 
            IDebugSession3 * This);
        
        HRESULT ( STDMETHODCALLTYPE *EnumCodeContexts )( 
            IDebugSession3 * This,
            /* [in] */ __RPC__in_opt IDebugProgram2 *pProgram,
            /* [in] */ __RPC__in_opt IDebugDocumentPosition2 *pDocPos,
            /* [out] */ __RPC__deref_out_opt IEnumDebugCodeContexts2 **ppEnum);
        
        HRESULT ( STDMETHODCALLTYPE *SetException )( 
            IDebugSession3 * This,
            /* [in] */ __RPC__in EXCEPTION_INFO *pException);
        
        HRESULT ( STDMETHODCALLTYPE *EnumSetExceptions )( 
            IDebugSession3 * This,
            /* [in] */ __RPC__in_opt IDebugProgram2 *pProgram,
            /* [full][in] */ __RPC__in_opt LPCOLESTR pszProgram,
            /* [in] */ __RPC__in REFGUID guidType,
            /* [out] */ __RPC__deref_out_opt IEnumDebugExceptionInfo2 **ppEnum);
        
        HRESULT ( STDMETHODCALLTYPE *RemoveSetException )( 
            IDebugSession3 * This,
            /* [in] */ __RPC__in EXCEPTION_INFO *pException);
        
        HRESULT ( STDMETHODCALLTYPE *RemoveAllSetExceptions )( 
            IDebugSession3 * This,
            /* [in] */ __RPC__in REFGUID guidType);
        
        HRESULT ( STDMETHODCALLTYPE *EnumDefaultExceptions )( 
            IDebugSession3 * This,
            /* [full][in] */ __RPC__in_opt EXCEPTION_INFO *pParentException,
            /* [out] */ __RPC__deref_out_opt IEnumDebugExceptionInfo2 **ppEnum);
        
        HRESULT ( STDMETHODCALLTYPE *GetENCUpdate )( 
            IDebugSession3 * This,
            /* [in] */ __RPC__in_opt IDebugProgram2 *pProgram,
            /* [out] */ __RPC__deref_out_opt IDebugENCUpdate **ppUpdate);
        
        HRESULT ( STDMETHODCALLTYPE *SetLocale )( 
            IDebugSession3 * This,
            /* [in] */ WORD wLangID);
        
        HRESULT ( STDMETHODCALLTYPE *SetRegistryRoot )( 
            IDebugSession3 * This,
            /* [full][in] */ __RPC__in_opt LPCOLESTR pszRegistryRoot);
        
        HRESULT ( STDMETHODCALLTYPE *IsAlive )( 
            IDebugSession3 * This);
        
        HRESULT ( STDMETHODCALLTYPE *ClearAllSessionThreadStackFrames )( 
            IDebugSession3 * This);
        
        HRESULT ( STDMETHODCALLTYPE *__deprecated_GetSessionId )( 
            IDebugSession3 * This,
            /* [in] */ __RPC__in_opt IDebugEventCallback2 *pCallback,
            /* [full][size_is][in] */ __RPC__in_ecount_full_opt(celtSpecificEngines) GUID *rgguidSpecificEngines,
            /* [in] */ DWORD celtSpecificEngines,
            /* [full][in] */ __RPC__in_opt LPCOLESTR pszStartPageUrl,
            /* [out] */ __RPC__deref_out_opt BSTR *pbstrSessionId);
        
        HRESULT ( STDMETHODCALLTYPE *SetEngineMetric )( 
            IDebugSession3 * This,
            /* [in] */ __RPC__in REFGUID guidEngine,
            /* [in] */ __RPC__in LPCOLESTR pszMetric,
            /* [in] */ VARIANT varValue);
        
        HRESULT ( STDMETHODCALLTYPE *SetStoppingModel )( 
            IDebugSession3 * This,
            /* [in] */ STOPPING_MODEL dwStoppingModel);
        
        HRESULT ( STDMETHODCALLTYPE *GetStoppingModel )( 
            IDebugSession3 * This,
            /* [out] */ __RPC__out STOPPING_MODEL *pdwStoppingModel);
        
        HRESULT ( STDMETHODCALLTYPE *__deprecated_RegisterSessionWithServer )( 
            IDebugSession3 * This,
            /* [in] */ __RPC__in LPCOLESTR pwszServerName);
        
        HRESULT ( STDMETHODCALLTYPE *SetSymbolPath )( 
            IDebugSession3 * This,
            /* [full][in] */ __RPC__in_opt LPCOLESTR szSymbolSearchPath,
            /* [full][in] */ __RPC__in_opt LPCOLESTR szSymbolCachePath,
            /* [in] */ LOAD_SYMBOLS_FLAGS Flags);
        
        HRESULT ( STDMETHODCALLTYPE *LoadSymbols )( 
            IDebugSession3 * This);
        
        HRESULT ( STDMETHODCALLTYPE *RegisterCallback )( 
            IDebugSession3 * This,
            /* [in] */ __RPC__in_opt IDebugEventCallback2 *pCallback);
        
        HRESULT ( STDMETHODCALLTYPE *ConnectToServerEx )( 
            IDebugSession3 * This,
            /* [full][in] */ __RPC__in_opt LPCOLESTR szServerName,
            /* [in] */ CONNECT_REASON ConnectReason,
            /* [out] */ __RPC__deref_out_opt IDebugCoreServer3 **ppServer);
        
        HRESULT ( STDMETHODCALLTYPE *SetJustMyCodeState )( 
            IDebugSession3 * This,
            /* [in] */ BOOL fUpdate,
            /* [in] */ DWORD dwModules,
            /* [full][size_is][in] */ __RPC__in_ecount_full_opt(dwModules) JMC_CODE_SPEC *rgJMCSpec);
        
        HRESULT ( STDMETHODCALLTYPE *GetRecentServerNames )( 
            IDebugSession3 * This,
            /* [out] */ __RPC__out BSTR_ARRAY *pServers);
        
        HRESULT ( STDMETHODCALLTYPE *SetMaxRecentServerNames )( 
            IDebugSession3 * This,
            /* [in] */ DWORD dwNewMax);
        
        HRESULT ( STDMETHODCALLTYPE *InitializeFeatures )( 
            IDebugSession3 * This,
            /* [in] */ SESSION_FEATURES EnabledFeatures);
        
        HRESULT ( STDMETHODCALLTYPE *SetAllExceptions )( 
            IDebugSession3 * This,
            /* [in] */ EXCEPTION_STATE dwState);
        
        HRESULT ( STDMETHODCALLTYPE *GetStateForAllExceptions )( 
            IDebugSession3 * This,
            /* [out] */ __RPC__out EXCEPTION_STATE *pdwState);
        
        HRESULT ( STDMETHODCALLTYPE *AddExceptionCallback )( 
            IDebugSession3 * This,
            /* [in] */ __RPC__in EXCEPTION_INFO *pException,
            /* [in] */ __RPC__in_opt IDebugExceptionCallback2 *pCallback);
        
        HRESULT ( STDMETHODCALLTYPE *RemoveExceptionCallback )( 
            IDebugSession3 * This,
            /* [in] */ __RPC__in EXCEPTION_INFO *pException,
            /* [in] */ __RPC__in_opt IDebugExceptionCallback2 *pCallback);
        
        HRESULT ( STDMETHODCALLTYPE *BlockingShutdownSession )( 
            IDebugSession3 * This,
            /* [in] */ DWORD dwTimeout);
        
        END_INTERFACE
    } IDebugSession3Vtbl;

    interface IDebugSession3
    {
        CONST_VTBL struct IDebugSession3Vtbl *lpVtbl;
    };

    

#ifdef COBJMACROS


#define IDebugSession3_QueryInterface(This,riid,ppvObject)	\
    ( (This)->lpVtbl -> QueryInterface(This,riid,ppvObject) ) 

#define IDebugSession3_AddRef(This)	\
    ( (This)->lpVtbl -> AddRef(This) ) 

#define IDebugSession3_Release(This)	\
    ( (This)->lpVtbl -> Release(This) ) 


#define IDebugSession3_GetName(This,pbstrName)	\
    ( (This)->lpVtbl -> GetName(This,pbstrName) ) 

#define IDebugSession3_SetName(This,pszName)	\
    ( (This)->lpVtbl -> SetName(This,pszName) ) 

#define IDebugSession3_EnumProcesses(This,ppEnum)	\
    ( (This)->lpVtbl -> EnumProcesses(This,ppEnum) ) 

#define IDebugSession3_Launch(This,pszMachine,pPort,pszExe,pszArgs,pszDir,bstrEnv,pszOptions,dwLaunchFlags,hStdInput,hStdOutput,hStdError,guidLaunchingEngine,pCallback,rgguidSpecificEngines,celtSpecificEngines,ppProcess)	\
    ( (This)->lpVtbl -> Launch(This,pszMachine,pPort,pszExe,pszArgs,pszDir,bstrEnv,pszOptions,dwLaunchFlags,hStdInput,hStdOutput,hStdError,guidLaunchingEngine,pCallback,rgguidSpecificEngines,celtSpecificEngines,ppProcess) ) 

#define IDebugSession3_RegisterJITServer(This,clsidJITServer)	\
    ( (This)->lpVtbl -> RegisterJITServer(This,clsidJITServer) ) 

#define IDebugSession3_Terminate(This,fForce)	\
    ( (This)->lpVtbl -> Terminate(This,fForce) ) 

#define IDebugSession3_Detach(This)	\
    ( (This)->lpVtbl -> Detach(This) ) 

#define IDebugSession3_CauseBreak(This)	\
    ( (This)->lpVtbl -> CauseBreak(This) ) 

#define IDebugSession3_CreatePendingBreakpoint(This,pBPRequest,ppPendingBP)	\
    ( (This)->lpVtbl -> CreatePendingBreakpoint(This,pBPRequest,ppPendingBP) ) 

#define IDebugSession3_EnumPendingBreakpoints(This,pProgram,pszProgram,ppEnumBPs)	\
    ( (This)->lpVtbl -> EnumPendingBreakpoints(This,pProgram,pszProgram,ppEnumBPs) ) 

#define IDebugSession3_EnumMachines__deprecated(This,ppEnum)	\
    ( (This)->lpVtbl -> EnumMachines__deprecated(This,ppEnum) ) 

#define IDebugSession3_ConnectToServer(This,szServerName,ppServer)	\
    ( (This)->lpVtbl -> ConnectToServer(This,szServerName,ppServer) ) 

#define IDebugSession3_DisconnectServer(This,pServer)	\
    ( (This)->lpVtbl -> DisconnectServer(This,pServer) ) 

#define IDebugSession3_ShutdownSession(This)	\
    ( (This)->lpVtbl -> ShutdownSession(This) ) 

#define IDebugSession3_EnumCodeContexts(This,pProgram,pDocPos,ppEnum)	\
    ( (This)->lpVtbl -> EnumCodeContexts(This,pProgram,pDocPos,ppEnum) ) 

#define IDebugSession3_SetException(This,pException)	\
    ( (This)->lpVtbl -> SetException(This,pException) ) 

#define IDebugSession3_EnumSetExceptions(This,pProgram,pszProgram,guidType,ppEnum)	\
    ( (This)->lpVtbl -> EnumSetExceptions(This,pProgram,pszProgram,guidType,ppEnum) ) 

#define IDebugSession3_RemoveSetException(This,pException)	\
    ( (This)->lpVtbl -> RemoveSetException(This,pException) ) 

#define IDebugSession3_RemoveAllSetExceptions(This,guidType)	\
    ( (This)->lpVtbl -> RemoveAllSetExceptions(This,guidType) ) 

#define IDebugSession3_EnumDefaultExceptions(This,pParentException,ppEnum)	\
    ( (This)->lpVtbl -> EnumDefaultExceptions(This,pParentException,ppEnum) ) 

#define IDebugSession3_GetENCUpdate(This,pProgram,ppUpdate)	\
    ( (This)->lpVtbl -> GetENCUpdate(This,pProgram,ppUpdate) ) 

#define IDebugSession3_SetLocale(This,wLangID)	\
    ( (This)->lpVtbl -> SetLocale(This,wLangID) ) 

#define IDebugSession3_SetRegistryRoot(This,pszRegistryRoot)	\
    ( (This)->lpVtbl -> SetRegistryRoot(This,pszRegistryRoot) ) 

#define IDebugSession3_IsAlive(This)	\
    ( (This)->lpVtbl -> IsAlive(This) ) 

#define IDebugSession3_ClearAllSessionThreadStackFrames(This)	\
    ( (This)->lpVtbl -> ClearAllSessionThreadStackFrames(This) ) 

#define IDebugSession3___deprecated_GetSessionId(This,pCallback,rgguidSpecificEngines,celtSpecificEngines,pszStartPageUrl,pbstrSessionId)	\
    ( (This)->lpVtbl -> __deprecated_GetSessionId(This,pCallback,rgguidSpecificEngines,celtSpecificEngines,pszStartPageUrl,pbstrSessionId) ) 

#define IDebugSession3_SetEngineMetric(This,guidEngine,pszMetric,varValue)	\
    ( (This)->lpVtbl -> SetEngineMetric(This,guidEngine,pszMetric,varValue) ) 

#define IDebugSession3_SetStoppingModel(This,dwStoppingModel)	\
    ( (This)->lpVtbl -> SetStoppingModel(This,dwStoppingModel) ) 

#define IDebugSession3_GetStoppingModel(This,pdwStoppingModel)	\
    ( (This)->lpVtbl -> GetStoppingModel(This,pdwStoppingModel) ) 

#define IDebugSession3___deprecated_RegisterSessionWithServer(This,pwszServerName)	\
    ( (This)->lpVtbl -> __deprecated_RegisterSessionWithServer(This,pwszServerName) ) 


#define IDebugSession3_SetSymbolPath(This,szSymbolSearchPath,szSymbolCachePath,Flags)	\
    ( (This)->lpVtbl -> SetSymbolPath(This,szSymbolSearchPath,szSymbolCachePath,Flags) ) 

#define IDebugSession3_LoadSymbols(This)	\
    ( (This)->lpVtbl -> LoadSymbols(This) ) 

#define IDebugSession3_RegisterCallback(This,pCallback)	\
    ( (This)->lpVtbl -> RegisterCallback(This,pCallback) ) 

#define IDebugSession3_ConnectToServerEx(This,szServerName,ConnectReason,ppServer)	\
    ( (This)->lpVtbl -> ConnectToServerEx(This,szServerName,ConnectReason,ppServer) ) 

#define IDebugSession3_SetJustMyCodeState(This,fUpdate,dwModules,rgJMCSpec)	\
    ( (This)->lpVtbl -> SetJustMyCodeState(This,fUpdate,dwModules,rgJMCSpec) ) 

#define IDebugSession3_GetRecentServerNames(This,pServers)	\
    ( (This)->lpVtbl -> GetRecentServerNames(This,pServers) ) 

#define IDebugSession3_SetMaxRecentServerNames(This,dwNewMax)	\
    ( (This)->lpVtbl -> SetMaxRecentServerNames(This,dwNewMax) ) 

#define IDebugSession3_InitializeFeatures(This,EnabledFeatures)	\
    ( (This)->lpVtbl -> InitializeFeatures(This,EnabledFeatures) ) 

#define IDebugSession3_SetAllExceptions(This,dwState)	\
    ( (This)->lpVtbl -> SetAllExceptions(This,dwState) ) 

#define IDebugSession3_GetStateForAllExceptions(This,pdwState)	\
    ( (This)->lpVtbl -> GetStateForAllExceptions(This,pdwState) ) 

#define IDebugSession3_AddExceptionCallback(This,pException,pCallback)	\
    ( (This)->lpVtbl -> AddExceptionCallback(This,pException,pCallback) ) 

#define IDebugSession3_RemoveExceptionCallback(This,pException,pCallback)	\
    ( (This)->lpVtbl -> RemoveExceptionCallback(This,pException,pCallback) ) 

#define IDebugSession3_BlockingShutdownSession(This,dwTimeout)	\
    ( (This)->lpVtbl -> BlockingShutdownSession(This,dwTimeout) ) 

#endif /* COBJMACROS */


#endif 	/* C style interface */




#endif 	/* __IDebugSession3_INTERFACE_DEFINED__ */


#ifndef __IDebugEngine2_INTERFACE_DEFINED__
#define __IDebugEngine2_INTERFACE_DEFINED__

/* interface IDebugEngine2 */
/* [unique][uuid][object] */ 


enum enum_ATTACH_REASON
    {	ATTACH_REASON_LAUNCH	= 0x1,
	ATTACH_REASON_USER	= 0x2,
	ATTACH_REASON_AUTO	= 0x3
    } ;
typedef DWORD ATTACH_REASON;


EXTERN_C const IID IID_IDebugEngine2;

#if defined(__cplusplus) && !defined(CINTERFACE)
    
    MIDL_INTERFACE("ba105b52-12f1-4038-ae64-d95785874c47")
    IDebugEngine2 : public IUnknown
    {
    public:
        virtual HRESULT STDMETHODCALLTYPE EnumPrograms( 
            /* [out] */ __RPC__deref_out_opt IEnumDebugPrograms2 **ppEnum) = 0;
        
        virtual HRESULT STDMETHODCALLTYPE Attach( 
            /* [length_is][size_is][in] */ __RPC__in_ecount_part(celtPrograms, celtPrograms) IDebugProgram2 **rgpPrograms,
            /* [length_is][size_is][in] */ __RPC__in_ecount_part(celtPrograms, celtPrograms) IDebugProgramNode2 **rgpProgramNodes,
            /* [in] */ DWORD celtPrograms,
            /* [in] */ __RPC__in_opt IDebugEventCallback2 *pCallback,
            /* [in] */ ATTACH_REASON dwReason) = 0;
        
        virtual HRESULT STDMETHODCALLTYPE CreatePendingBreakpoint( 
            /* [in] */ __RPC__in_opt IDebugBreakpointRequest2 *pBPRequest,
            /* [out] */ __RPC__deref_out_opt IDebugPendingBreakpoint2 **ppPendingBP) = 0;
        
        virtual HRESULT STDMETHODCALLTYPE SetException( 
            /* [in] */ __RPC__in EXCEPTION_INFO *pException) = 0;
        
        virtual HRESULT STDMETHODCALLTYPE RemoveSetException( 
            /* [in] */ __RPC__in EXCEPTION_INFO *pException) = 0;
        
        virtual HRESULT STDMETHODCALLTYPE RemoveAllSetExceptions( 
            /* [in] */ __RPC__in REFGUID guidType) = 0;
        
        virtual HRESULT STDMETHODCALLTYPE GetEngineId( 
            /* [out] */ __RPC__out GUID *pguidEngine) = 0;
        
        virtual HRESULT STDMETHODCALLTYPE DestroyProgram( 
            /* [in] */ __RPC__in_opt IDebugProgram2 *pProgram) = 0;
        
        virtual HRESULT STDMETHODCALLTYPE ContinueFromSynchronousEvent( 
            /* [in] */ __RPC__in_opt IDebugEvent2 *pEvent) = 0;
        
        virtual HRESULT STDMETHODCALLTYPE SetLocale( 
            /* [in] */ WORD wLangID) = 0;
        
        virtual HRESULT STDMETHODCALLTYPE SetRegistryRoot( 
            /* [full][in] */ __RPC__in_opt LPCOLESTR pszRegistryRoot) = 0;
        
        virtual HRESULT STDMETHODCALLTYPE SetMetric( 
            /* [in] */ __RPC__in LPCOLESTR pszMetric,
            /* [in] */ VARIANT varValue) = 0;
        
        virtual HRESULT STDMETHODCALLTYPE CauseBreak( void) = 0;
        
    };
    
#else 	/* C style interface */

    typedef struct IDebugEngine2Vtbl
    {
        BEGIN_INTERFACE
        
        HRESULT ( STDMETHODCALLTYPE *QueryInterface )( 
            IDebugEngine2 * This,
            /* [in] */ __RPC__in REFIID riid,
            /* [iid_is][out] */ 
            __RPC__deref_out  void **ppvObject);
        
        ULONG ( STDMETHODCALLTYPE *AddRef )( 
            IDebugEngine2 * This);
        
        ULONG ( STDMETHODCALLTYPE *Release )( 
            IDebugEngine2 * This);
        
        HRESULT ( STDMETHODCALLTYPE *EnumPrograms )( 
            IDebugEngine2 * This,
            /* [out] */ __RPC__deref_out_opt IEnumDebugPrograms2 **ppEnum);
        
        HRESULT ( STDMETHODCALLTYPE *Attach )( 
            IDebugEngine2 * This,
            /* [length_is][size_is][in] */ __RPC__in_ecount_part(celtPrograms, celtPrograms) IDebugProgram2 **rgpPrograms,
            /* [length_is][size_is][in] */ __RPC__in_ecount_part(celtPrograms, celtPrograms) IDebugProgramNode2 **rgpProgramNodes,
            /* [in] */ DWORD celtPrograms,
            /* [in] */ __RPC__in_opt IDebugEventCallback2 *pCallback,
            /* [in] */ ATTACH_REASON dwReason);
        
        HRESULT ( STDMETHODCALLTYPE *CreatePendingBreakpoint )( 
            IDebugEngine2 * This,
            /* [in] */ __RPC__in_opt IDebugBreakpointRequest2 *pBPRequest,
            /* [out] */ __RPC__deref_out_opt IDebugPendingBreakpoint2 **ppPendingBP);
        
        HRESULT ( STDMETHODCALLTYPE *SetException )( 
            IDebugEngine2 * This,
            /* [in] */ __RPC__in EXCEPTION_INFO *pException);
        
        HRESULT ( STDMETHODCALLTYPE *RemoveSetException )( 
            IDebugEngine2 * This,
            /* [in] */ __RPC__in EXCEPTION_INFO *pException);
        
        HRESULT ( STDMETHODCALLTYPE *RemoveAllSetExceptions )( 
            IDebugEngine2 * This,
            /* [in] */ __RPC__in REFGUID guidType);
        
        HRESULT ( STDMETHODCALLTYPE *GetEngineId )( 
            IDebugEngine2 * This,
            /* [out] */ __RPC__out GUID *pguidEngine);
        
        HRESULT ( STDMETHODCALLTYPE *DestroyProgram )( 
            IDebugEngine2 * This,
            /* [in] */ __RPC__in_opt IDebugProgram2 *pProgram);
        
        HRESULT ( STDMETHODCALLTYPE *ContinueFromSynchronousEvent )( 
            IDebugEngine2 * This,
            /* [in] */ __RPC__in_opt IDebugEvent2 *pEvent);
        
        HRESULT ( STDMETHODCALLTYPE *SetLocale )( 
            IDebugEngine2 * This,
            /* [in] */ WORD wLangID);
        
        HRESULT ( STDMETHODCALLTYPE *SetRegistryRoot )( 
            IDebugEngine2 * This,
            /* [full][in] */ __RPC__in_opt LPCOLESTR pszRegistryRoot);
        
        HRESULT ( STDMETHODCALLTYPE *SetMetric )( 
            IDebugEngine2 * This,
            /* [in] */ __RPC__in LPCOLESTR pszMetric,
            /* [in] */ VARIANT varValue);
        
        HRESULT ( STDMETHODCALLTYPE *CauseBreak )( 
            IDebugEngine2 * This);
        
        END_INTERFACE
    } IDebugEngine2Vtbl;

    interface IDebugEngine2
    {
        CONST_VTBL struct IDebugEngine2Vtbl *lpVtbl;
    };

    

#ifdef COBJMACROS


#define IDebugEngine2_QueryInterface(This,riid,ppvObject)	\
    ( (This)->lpVtbl -> QueryInterface(This,riid,ppvObject) ) 

#define IDebugEngine2_AddRef(This)	\
    ( (This)->lpVtbl -> AddRef(This) ) 

#define IDebugEngine2_Release(This)	\
    ( (This)->lpVtbl -> Release(This) ) 


#define IDebugEngine2_EnumPrograms(This,ppEnum)	\
    ( (This)->lpVtbl -> EnumPrograms(This,ppEnum) ) 

#define IDebugEngine2_Attach(This,rgpPrograms,rgpProgramNodes,celtPrograms,pCallback,dwReason)	\
    ( (This)->lpVtbl -> Attach(This,rgpPrograms,rgpProgramNodes,celtPrograms,pCallback,dwReason) ) 

#define IDebugEngine2_CreatePendingBreakpoint(This,pBPRequest,ppPendingBP)	\
    ( (This)->lpVtbl -> CreatePendingBreakpoint(This,pBPRequest,ppPendingBP) ) 

#define IDebugEngine2_SetException(This,pException)	\
    ( (This)->lpVtbl -> SetException(This,pException) ) 

#define IDebugEngine2_RemoveSetException(This,pException)	\
    ( (This)->lpVtbl -> RemoveSetException(This,pException) ) 

#define IDebugEngine2_RemoveAllSetExceptions(This,guidType)	\
    ( (This)->lpVtbl -> RemoveAllSetExceptions(This,guidType) ) 

#define IDebugEngine2_GetEngineId(This,pguidEngine)	\
    ( (This)->lpVtbl -> GetEngineId(This,pguidEngine) ) 

#define IDebugEngine2_DestroyProgram(This,pProgram)	\
    ( (This)->lpVtbl -> DestroyProgram(This,pProgram) ) 

#define IDebugEngine2_ContinueFromSynchronousEvent(This,pEvent)	\
    ( (This)->lpVtbl -> ContinueFromSynchronousEvent(This,pEvent) ) 

#define IDebugEngine2_SetLocale(This,wLangID)	\
    ( (This)->lpVtbl -> SetLocale(This,wLangID) ) 

#define IDebugEngine2_SetRegistryRoot(This,pszRegistryRoot)	\
    ( (This)->lpVtbl -> SetRegistryRoot(This,pszRegistryRoot) ) 

#define IDebugEngine2_SetMetric(This,pszMetric,varValue)	\
    ( (This)->lpVtbl -> SetMetric(This,pszMetric,varValue) ) 

#define IDebugEngine2_CauseBreak(This)	\
    ( (This)->lpVtbl -> CauseBreak(This) ) 

#endif /* COBJMACROS */


#endif 	/* C style interface */




#endif 	/* __IDebugEngine2_INTERFACE_DEFINED__ */


#ifndef __IDebugEngineLaunch2_INTERFACE_DEFINED__
#define __IDebugEngineLaunch2_INTERFACE_DEFINED__

/* interface IDebugEngineLaunch2 */
/* [unique][uuid][object] */ 


EXTERN_C const IID IID_IDebugEngineLaunch2;

#if defined(__cplusplus) && !defined(CINTERFACE)
    
    MIDL_INTERFACE("c7c1462f-9736-466c-b2c1-b6b2dedbf4a7")
    IDebugEngineLaunch2 : public IUnknown
    {
    public:
        virtual HRESULT STDMETHODCALLTYPE LaunchSuspended( 
            /* [full][in] */ __RPC__in_opt LPCOLESTR pszServer,
            /* [in] */ __RPC__in_opt IDebugPort2 *pPort,
            /* [full][in] */ __RPC__in_opt LPCOLESTR pszExe,
            /* [full][in] */ __RPC__in_opt LPCOLESTR pszArgs,
            /* [full][in] */ __RPC__in_opt LPCOLESTR pszDir,
            /* [full][in] */ __RPC__in_opt BSTR bstrEnv,
            /* [full][in] */ __RPC__in_opt LPCOLESTR pszOptions,
            /* [in] */ LAUNCH_FLAGS dwLaunchFlags,
            /* [in] */ DWORD hStdInput,
            /* [in] */ DWORD hStdOutput,
            /* [in] */ DWORD hStdError,
            /* [in] */ __RPC__in_opt IDebugEventCallback2 *pCallback,
            /* [out] */ __RPC__deref_out_opt IDebugProcess2 **ppProcess) = 0;
        
        virtual HRESULT STDMETHODCALLTYPE ResumeProcess( 
            /* [in] */ __RPC__in_opt IDebugProcess2 *pProcess) = 0;
        
        virtual HRESULT STDMETHODCALLTYPE CanTerminateProcess( 
            /* [in] */ __RPC__in_opt IDebugProcess2 *pProcess) = 0;
        
        virtual HRESULT STDMETHODCALLTYPE TerminateProcess( 
            /* [in] */ __RPC__in_opt IDebugProcess2 *pProcess) = 0;
        
    };
    
#else 	/* C style interface */

    typedef struct IDebugEngineLaunch2Vtbl
    {
        BEGIN_INTERFACE
        
        HRESULT ( STDMETHODCALLTYPE *QueryInterface )( 
            IDebugEngineLaunch2 * This,
            /* [in] */ __RPC__in REFIID riid,
            /* [iid_is][out] */ 
            __RPC__deref_out  void **ppvObject);
        
        ULONG ( STDMETHODCALLTYPE *AddRef )( 
            IDebugEngineLaunch2 * This);
        
        ULONG ( STDMETHODCALLTYPE *Release )( 
            IDebugEngineLaunch2 * This);
        
        HRESULT ( STDMETHODCALLTYPE *LaunchSuspended )( 
            IDebugEngineLaunch2 * This,
            /* [full][in] */ __RPC__in_opt LPCOLESTR pszServer,
            /* [in] */ __RPC__in_opt IDebugPort2 *pPort,
            /* [full][in] */ __RPC__in_opt LPCOLESTR pszExe,
            /* [full][in] */ __RPC__in_opt LPCOLESTR pszArgs,
            /* [full][in] */ __RPC__in_opt LPCOLESTR pszDir,
            /* [full][in] */ __RPC__in_opt BSTR bstrEnv,
            /* [full][in] */ __RPC__in_opt LPCOLESTR pszOptions,
            /* [in] */ LAUNCH_FLAGS dwLaunchFlags,
            /* [in] */ DWORD hStdInput,
            /* [in] */ DWORD hStdOutput,
            /* [in] */ DWORD hStdError,
            /* [in] */ __RPC__in_opt IDebugEventCallback2 *pCallback,
            /* [out] */ __RPC__deref_out_opt IDebugProcess2 **ppProcess);
        
        HRESULT ( STDMETHODCALLTYPE *ResumeProcess )( 
            IDebugEngineLaunch2 * This,
            /* [in] */ __RPC__in_opt IDebugProcess2 *pProcess);
        
        HRESULT ( STDMETHODCALLTYPE *CanTerminateProcess )( 
            IDebugEngineLaunch2 * This,
            /* [in] */ __RPC__in_opt IDebugProcess2 *pProcess);
        
        HRESULT ( STDMETHODCALLTYPE *TerminateProcess )( 
            IDebugEngineLaunch2 * This,
            /* [in] */ __RPC__in_opt IDebugProcess2 *pProcess);
        
        END_INTERFACE
    } IDebugEngineLaunch2Vtbl;

    interface IDebugEngineLaunch2
    {
        CONST_VTBL struct IDebugEngineLaunch2Vtbl *lpVtbl;
    };

    

#ifdef COBJMACROS


#define IDebugEngineLaunch2_QueryInterface(This,riid,ppvObject)	\
    ( (This)->lpVtbl -> QueryInterface(This,riid,ppvObject) ) 

#define IDebugEngineLaunch2_AddRef(This)	\
    ( (This)->lpVtbl -> AddRef(This) ) 

#define IDebugEngineLaunch2_Release(This)	\
    ( (This)->lpVtbl -> Release(This) ) 


#define IDebugEngineLaunch2_LaunchSuspended(This,pszServer,pPort,pszExe,pszArgs,pszDir,bstrEnv,pszOptions,dwLaunchFlags,hStdInput,hStdOutput,hStdError,pCallback,ppProcess)	\
    ( (This)->lpVtbl -> LaunchSuspended(This,pszServer,pPort,pszExe,pszArgs,pszDir,bstrEnv,pszOptions,dwLaunchFlags,hStdInput,hStdOutput,hStdError,pCallback,ppProcess) ) 

#define IDebugEngineLaunch2_ResumeProcess(This,pProcess)	\
    ( (This)->lpVtbl -> ResumeProcess(This,pProcess) ) 

#define IDebugEngineLaunch2_CanTerminateProcess(This,pProcess)	\
    ( (This)->lpVtbl -> CanTerminateProcess(This,pProcess) ) 

#define IDebugEngineLaunch2_TerminateProcess(This,pProcess)	\
    ( (This)->lpVtbl -> TerminateProcess(This,pProcess) ) 

#endif /* COBJMACROS */


#endif 	/* C style interface */




#endif 	/* __IDebugEngineLaunch2_INTERFACE_DEFINED__ */


#ifndef __IDebugEngine3_INTERFACE_DEFINED__
#define __IDebugEngine3_INTERFACE_DEFINED__

/* interface IDebugEngine3 */
/* [unique][uuid][object] */ 


EXTERN_C const IID IID_IDebugEngine3;

#if defined(__cplusplus) && !defined(CINTERFACE)
    
    MIDL_INTERFACE("A60384F6-3712-4cb3-BC46-81E6402FEE99")
    IDebugEngine3 : public IDebugEngine2
    {
    public:
        virtual HRESULT STDMETHODCALLTYPE SetSymbolPath( 
            /* [in] */ __RPC__in LPCOLESTR szSymbolSearchPath,
            /* [in] */ __RPC__in LPCOLESTR szSymbolCachePath,
            /* [in] */ LOAD_SYMBOLS_FLAGS Flags) = 0;
        
        virtual HRESULT STDMETHODCALLTYPE LoadSymbols( void) = 0;
        
        virtual HRESULT STDMETHODCALLTYPE SetJustMyCodeState( 
            /* [in] */ BOOL fUpdate,
            /* [in] */ DWORD dwModules,
            /* [full][size_is][in] */ __RPC__in_ecount_full_opt(dwModules) JMC_CODE_SPEC *rgJMCSpec) = 0;
        
        virtual HRESULT STDMETHODCALLTYPE SetEngineGuid( 
            /* [in] */ __RPC__in GUID *guidEngine) = 0;
        
        virtual HRESULT STDMETHODCALLTYPE SetAllExceptions( 
            /* [in] */ EXCEPTION_STATE dwState) = 0;
        
    };
    
#else 	/* C style interface */

    typedef struct IDebugEngine3Vtbl
    {
        BEGIN_INTERFACE
        
        HRESULT ( STDMETHODCALLTYPE *QueryInterface )( 
            IDebugEngine3 * This,
            /* [in] */ __RPC__in REFIID riid,
            /* [iid_is][out] */ 
            __RPC__deref_out  void **ppvObject);
        
        ULONG ( STDMETHODCALLTYPE *AddRef )( 
            IDebugEngine3 * This);
        
        ULONG ( STDMETHODCALLTYPE *Release )( 
            IDebugEngine3 * This);
        
        HRESULT ( STDMETHODCALLTYPE *EnumPrograms )( 
            IDebugEngine3 * This,
            /* [out] */ __RPC__deref_out_opt IEnumDebugPrograms2 **ppEnum);
        
        HRESULT ( STDMETHODCALLTYPE *Attach )( 
            IDebugEngine3 * This,
            /* [length_is][size_is][in] */ __RPC__in_ecount_part(celtPrograms, celtPrograms) IDebugProgram2 **rgpPrograms,
            /* [length_is][size_is][in] */ __RPC__in_ecount_part(celtPrograms, celtPrograms) IDebugProgramNode2 **rgpProgramNodes,
            /* [in] */ DWORD celtPrograms,
            /* [in] */ __RPC__in_opt IDebugEventCallback2 *pCallback,
            /* [in] */ ATTACH_REASON dwReason);
        
        HRESULT ( STDMETHODCALLTYPE *CreatePendingBreakpoint )( 
            IDebugEngine3 * This,
            /* [in] */ __RPC__in_opt IDebugBreakpointRequest2 *pBPRequest,
            /* [out] */ __RPC__deref_out_opt IDebugPendingBreakpoint2 **ppPendingBP);
        
        HRESULT ( STDMETHODCALLTYPE *SetException )( 
            IDebugEngine3 * This,
            /* [in] */ __RPC__in EXCEPTION_INFO *pException);
        
        HRESULT ( STDMETHODCALLTYPE *RemoveSetException )( 
            IDebugEngine3 * This,
            /* [in] */ __RPC__in EXCEPTION_INFO *pException);
        
        HRESULT ( STDMETHODCALLTYPE *RemoveAllSetExceptions )( 
            IDebugEngine3 * This,
            /* [in] */ __RPC__in REFGUID guidType);
        
        HRESULT ( STDMETHODCALLTYPE *GetEngineId )( 
            IDebugEngine3 * This,
            /* [out] */ __RPC__out GUID *pguidEngine);
        
        HRESULT ( STDMETHODCALLTYPE *DestroyProgram )( 
            IDebugEngine3 * This,
            /* [in] */ __RPC__in_opt IDebugProgram2 *pProgram);
        
        HRESULT ( STDMETHODCALLTYPE *ContinueFromSynchronousEvent )( 
            IDebugEngine3 * This,
            /* [in] */ __RPC__in_opt IDebugEvent2 *pEvent);
        
        HRESULT ( STDMETHODCALLTYPE *SetLocale )( 
            IDebugEngine3 * This,
            /* [in] */ WORD wLangID);
        
        HRESULT ( STDMETHODCALLTYPE *SetRegistryRoot )( 
            IDebugEngine3 * This,
            /* [full][in] */ __RPC__in_opt LPCOLESTR pszRegistryRoot);
        
        HRESULT ( STDMETHODCALLTYPE *SetMetric )( 
            IDebugEngine3 * This,
            /* [in] */ __RPC__in LPCOLESTR pszMetric,
            /* [in] */ VARIANT varValue);
        
        HRESULT ( STDMETHODCALLTYPE *CauseBreak )( 
            IDebugEngine3 * This);
        
        HRESULT ( STDMETHODCALLTYPE *SetSymbolPath )( 
            IDebugEngine3 * This,
            /* [in] */ __RPC__in LPCOLESTR szSymbolSearchPath,
            /* [in] */ __RPC__in LPCOLESTR szSymbolCachePath,
            /* [in] */ LOAD_SYMBOLS_FLAGS Flags);
        
        HRESULT ( STDMETHODCALLTYPE *LoadSymbols )( 
            IDebugEngine3 * This);
        
        HRESULT ( STDMETHODCALLTYPE *SetJustMyCodeState )( 
            IDebugEngine3 * This,
            /* [in] */ BOOL fUpdate,
            /* [in] */ DWORD dwModules,
            /* [full][size_is][in] */ __RPC__in_ecount_full_opt(dwModules) JMC_CODE_SPEC *rgJMCSpec);
        
        HRESULT ( STDMETHODCALLTYPE *SetEngineGuid )( 
            IDebugEngine3 * This,
            /* [in] */ __RPC__in GUID *guidEngine);
        
        HRESULT ( STDMETHODCALLTYPE *SetAllExceptions )( 
            IDebugEngine3 * This,
            /* [in] */ EXCEPTION_STATE dwState);
        
        END_INTERFACE
    } IDebugEngine3Vtbl;

    interface IDebugEngine3
    {
        CONST_VTBL struct IDebugEngine3Vtbl *lpVtbl;
    };

    

#ifdef COBJMACROS


#define IDebugEngine3_QueryInterface(This,riid,ppvObject)	\
    ( (This)->lpVtbl -> QueryInterface(This,riid,ppvObject) ) 

#define IDebugEngine3_AddRef(This)	\
    ( (This)->lpVtbl -> AddRef(This) ) 

#define IDebugEngine3_Release(This)	\
    ( (This)->lpVtbl -> Release(This) ) 


#define IDebugEngine3_EnumPrograms(This,ppEnum)	\
    ( (This)->lpVtbl -> EnumPrograms(This,ppEnum) ) 

#define IDebugEngine3_Attach(This,rgpPrograms,rgpProgramNodes,celtPrograms,pCallback,dwReason)	\
    ( (This)->lpVtbl -> Attach(This,rgpPrograms,rgpProgramNodes,celtPrograms,pCallback,dwReason) ) 

#define IDebugEngine3_CreatePendingBreakpoint(This,pBPRequest,ppPendingBP)	\
    ( (This)->lpVtbl -> CreatePendingBreakpoint(This,pBPRequest,ppPendingBP) ) 

#define IDebugEngine3_SetException(This,pException)	\
    ( (This)->lpVtbl -> SetException(This,pException) ) 

#define IDebugEngine3_RemoveSetException(This,pException)	\
    ( (This)->lpVtbl -> RemoveSetException(This,pException) ) 

#define IDebugEngine3_RemoveAllSetExceptions(This,guidType)	\
    ( (This)->lpVtbl -> RemoveAllSetExceptions(This,guidType) ) 

#define IDebugEngine3_GetEngineId(This,pguidEngine)	\
    ( (This)->lpVtbl -> GetEngineId(This,pguidEngine) ) 

#define IDebugEngine3_DestroyProgram(This,pProgram)	\
    ( (This)->lpVtbl -> DestroyProgram(This,pProgram) ) 

#define IDebugEngine3_ContinueFromSynchronousEvent(This,pEvent)	\
    ( (This)->lpVtbl -> ContinueFromSynchronousEvent(This,pEvent) ) 

#define IDebugEngine3_SetLocale(This,wLangID)	\
    ( (This)->lpVtbl -> SetLocale(This,wLangID) ) 

#define IDebugEngine3_SetRegistryRoot(This,pszRegistryRoot)	\
    ( (This)->lpVtbl -> SetRegistryRoot(This,pszRegistryRoot) ) 

#define IDebugEngine3_SetMetric(This,pszMetric,varValue)	\
    ( (This)->lpVtbl -> SetMetric(This,pszMetric,varValue) ) 

#define IDebugEngine3_CauseBreak(This)	\
    ( (This)->lpVtbl -> CauseBreak(This) ) 


#define IDebugEngine3_SetSymbolPath(This,szSymbolSearchPath,szSymbolCachePath,Flags)	\
    ( (This)->lpVtbl -> SetSymbolPath(This,szSymbolSearchPath,szSymbolCachePath,Flags) ) 

#define IDebugEngine3_LoadSymbols(This)	\
    ( (This)->lpVtbl -> LoadSymbols(This) ) 

#define IDebugEngine3_SetJustMyCodeState(This,fUpdate,dwModules,rgJMCSpec)	\
    ( (This)->lpVtbl -> SetJustMyCodeState(This,fUpdate,dwModules,rgJMCSpec) ) 

#define IDebugEngine3_SetEngineGuid(This,guidEngine)	\
    ( (This)->lpVtbl -> SetEngineGuid(This,guidEngine) ) 

#define IDebugEngine3_SetAllExceptions(This,dwState)	\
    ( (This)->lpVtbl -> SetAllExceptions(This,dwState) ) 

#endif /* COBJMACROS */


#endif 	/* C style interface */




#endif 	/* __IDebugEngine3_INTERFACE_DEFINED__ */


#ifndef __IDebugEventCallback2_INTERFACE_DEFINED__
#define __IDebugEventCallback2_INTERFACE_DEFINED__

/* interface IDebugEventCallback2 */
/* [unique][uuid][object] */ 


enum enum_EVENTATTRIBUTES
    {	EVENT_ASYNCHRONOUS	= 0,
	EVENT_SYNCHRONOUS	= 0x1,
	EVENT_STOPPING	= 0x2,
	EVENT_ASYNC_STOP	= 0x2,
	EVENT_SYNC_STOP	= 0x3,
	EVENT_IMMEDIATE	= 0x4,
	EVENT_EXPRESSION_EVALUATION	= 0x8
    } ;
typedef DWORD EVENTATTRIBUTES;


EXTERN_C const IID IID_IDebugEventCallback2;

#if defined(__cplusplus) && !defined(CINTERFACE)
    
    MIDL_INTERFACE("ade2eeb9-fc85-4f5b-b5d9-d431b4aac31a")
    IDebugEventCallback2 : public IUnknown
    {
    public:
        virtual HRESULT STDMETHODCALLTYPE Event( 
            /* [in] */ __RPC__in_opt IDebugEngine2 *pEngine,
            /* [in] */ __RPC__in_opt IDebugProcess2 *pProcess,
            /* [in] */ __RPC__in_opt IDebugProgram2 *pProgram,
            /* [in] */ __RPC__in_opt IDebugThread2 *pThread,
            /* [in] */ __RPC__in_opt IDebugEvent2 *pEvent,
            /* [in] */ __RPC__in REFIID riidEvent,
            /* [in] */ DWORD dwAttrib) = 0;
        
    };
    
#else 	/* C style interface */

    typedef struct IDebugEventCallback2Vtbl
    {
        BEGIN_INTERFACE
        
        HRESULT ( STDMETHODCALLTYPE *QueryInterface )( 
            IDebugEventCallback2 * This,
            /* [in] */ __RPC__in REFIID riid,
            /* [iid_is][out] */ 
            __RPC__deref_out  void **ppvObject);
        
        ULONG ( STDMETHODCALLTYPE *AddRef )( 
            IDebugEventCallback2 * This);
        
        ULONG ( STDMETHODCALLTYPE *Release )( 
            IDebugEventCallback2 * This);
        
        HRESULT ( STDMETHODCALLTYPE *Event )( 
            IDebugEventCallback2 * This,
            /* [in] */ __RPC__in_opt IDebugEngine2 *pEngine,
            /* [in] */ __RPC__in_opt IDebugProcess2 *pProcess,
            /* [in] */ __RPC__in_opt IDebugProgram2 *pProgram,
            /* [in] */ __RPC__in_opt IDebugThread2 *pThread,
            /* [in] */ __RPC__in_opt IDebugEvent2 *pEvent,
            /* [in] */ __RPC__in REFIID riidEvent,
            /* [in] */ DWORD dwAttrib);
        
        END_INTERFACE
    } IDebugEventCallback2Vtbl;

    interface IDebugEventCallback2
    {
        CONST_VTBL struct IDebugEventCallback2Vtbl *lpVtbl;
    };

    

#ifdef COBJMACROS


#define IDebugEventCallback2_QueryInterface(This,riid,ppvObject)	\
    ( (This)->lpVtbl -> QueryInterface(This,riid,ppvObject) ) 

#define IDebugEventCallback2_AddRef(This)	\
    ( (This)->lpVtbl -> AddRef(This) ) 

#define IDebugEventCallback2_Release(This)	\
    ( (This)->lpVtbl -> Release(This) ) 


#define IDebugEventCallback2_Event(This,pEngine,pProcess,pProgram,pThread,pEvent,riidEvent,dwAttrib)	\
    ( (This)->lpVtbl -> Event(This,pEngine,pProcess,pProgram,pThread,pEvent,riidEvent,dwAttrib) ) 

#endif /* COBJMACROS */


#endif 	/* C style interface */




#endif 	/* __IDebugEventCallback2_INTERFACE_DEFINED__ */


#ifndef __IDebugSettingsCallback2_INTERFACE_DEFINED__
#define __IDebugSettingsCallback2_INTERFACE_DEFINED__

/* interface IDebugSettingsCallback2 */
/* [unique][uuid][object] */ 


EXTERN_C const IID IID_IDebugSettingsCallback2;

#if defined(__cplusplus) && !defined(CINTERFACE)
    
    MIDL_INTERFACE("050b1813-91db-47a0-8987-fc55bdd6362b")
    IDebugSettingsCallback2 : public IUnknown
    {
    public:
        virtual HRESULT STDMETHODCALLTYPE GetMetricGuid( 
            /* [in] */ __RPC__in LPCWSTR pszType,
            /* [in] */ __RPC__in REFGUID guidSection,
            /* [in] */ __RPC__in LPCWSTR pszMetric,
            /* [out] */ __RPC__out GUID *pguidValue) = 0;
        
        virtual HRESULT STDMETHODCALLTYPE GetMetricDword( 
            /* [in] */ __RPC__in LPCWSTR pszType,
            /* [in] */ __RPC__in REFGUID guidSection,
            /* [in] */ __RPC__in LPCWSTR pszMetric,
            /* [out] */ __RPC__out DWORD *pdwValue) = 0;
        
        virtual HRESULT STDMETHODCALLTYPE GetEEMetricString( 
            /* [in] */ __RPC__in REFGUID guidLang,
            /* [in] */ __RPC__in REFGUID guidVendor,
            /* [in] */ __RPC__in LPCWSTR pszMetric,
            /* [out] */ __RPC__deref_out_opt BSTR *pbstrValue) = 0;
        
        virtual HRESULT STDMETHODCALLTYPE GetEEMetricGuid( 
            /* [in] */ __RPC__in REFGUID guidLang,
            /* [in] */ __RPC__in REFGUID guidVendor,
            /* [in] */ __RPC__in LPCWSTR pszMetric,
            /* [out] */ __RPC__out GUID *pguidValue) = 0;
        
        virtual HRESULT STDMETHODCALLTYPE GetEEMetricFile( 
            /* [in] */ __RPC__in REFGUID guidLang,
            /* [in] */ __RPC__in REFGUID guidVendor,
            /* [in] */ __RPC__in LPCWSTR pszMetric,
            /* [out] */ __RPC__deref_out_opt BSTR *pbstrValue) = 0;
        
        virtual HRESULT STDMETHODCALLTYPE EnumEEs( 
            /* [in] */ DWORD celtBuffer,
            /* [length_is][size_is][full][out][in] */ __RPC__inout_ecount_part_opt(celtBuffer, *pceltEEs) GUID *rgguidLang,
            /* [length_is][size_is][full][out][in] */ __RPC__inout_ecount_part_opt(celtBuffer, *pceltEEs) GUID *rgguidVendor,
            /* [out][in] */ __RPC__inout DWORD *pceltEEs) = 0;
        
        virtual HRESULT STDMETHODCALLTYPE GetEEMetricDword( 
            /* [in] */ __RPC__in REFGUID guidLang,
            /* [in] */ __RPC__in REFGUID guidVendor,
            /* [in] */ __RPC__in LPCWSTR pszMetric,
            /* [out] */ __RPC__out DWORD *pdwValue) = 0;
        
        virtual HRESULT STDMETHODCALLTYPE GetEELocalObject( 
            /* [in] */ __RPC__in REFGUID guidLang,
            /* [in] */ __RPC__in REFGUID guidVendor,
            /* [in] */ __RPC__in LPCWSTR pszMetric,
            /* [out] */ __RPC__deref_out_opt IUnknown **ppUnk) = 0;
        
        virtual HRESULT STDMETHODCALLTYPE GetMetricString( 
            /* [in] */ __RPC__in LPCWSTR pszType,
            /* [in] */ __RPC__in REFGUID guidSection,
            /* [in] */ __RPC__in LPCWSTR pszMetric,
            /* [out] */ __RPC__deref_out_opt BSTR *pbstrValue) = 0;
        
    };
    
#else 	/* C style interface */

    typedef struct IDebugSettingsCallback2Vtbl
    {
        BEGIN_INTERFACE
        
        HRESULT ( STDMETHODCALLTYPE *QueryInterface )( 
            IDebugSettingsCallback2 * This,
            /* [in] */ __RPC__in REFIID riid,
            /* [iid_is][out] */ 
            __RPC__deref_out  void **ppvObject);
        
        ULONG ( STDMETHODCALLTYPE *AddRef )( 
            IDebugSettingsCallback2 * This);
        
        ULONG ( STDMETHODCALLTYPE *Release )( 
            IDebugSettingsCallback2 * This);
        
        HRESULT ( STDMETHODCALLTYPE *GetMetricGuid )( 
            IDebugSettingsCallback2 * This,
            /* [in] */ __RPC__in LPCWSTR pszType,
            /* [in] */ __RPC__in REFGUID guidSection,
            /* [in] */ __RPC__in LPCWSTR pszMetric,
            /* [out] */ __RPC__out GUID *pguidValue);
        
        HRESULT ( STDMETHODCALLTYPE *GetMetricDword )( 
            IDebugSettingsCallback2 * This,
            /* [in] */ __RPC__in LPCWSTR pszType,
            /* [in] */ __RPC__in REFGUID guidSection,
            /* [in] */ __RPC__in LPCWSTR pszMetric,
            /* [out] */ __RPC__out DWORD *pdwValue);
        
        HRESULT ( STDMETHODCALLTYPE *GetEEMetricString )( 
            IDebugSettingsCallback2 * This,
            /* [in] */ __RPC__in REFGUID guidLang,
            /* [in] */ __RPC__in REFGUID guidVendor,
            /* [in] */ __RPC__in LPCWSTR pszMetric,
            /* [out] */ __RPC__deref_out_opt BSTR *pbstrValue);
        
        HRESULT ( STDMETHODCALLTYPE *GetEEMetricGuid )( 
            IDebugSettingsCallback2 * This,
            /* [in] */ __RPC__in REFGUID guidLang,
            /* [in] */ __RPC__in REFGUID guidVendor,
            /* [in] */ __RPC__in LPCWSTR pszMetric,
            /* [out] */ __RPC__out GUID *pguidValue);
        
        HRESULT ( STDMETHODCALLTYPE *GetEEMetricFile )( 
            IDebugSettingsCallback2 * This,
            /* [in] */ __RPC__in REFGUID guidLang,
            /* [in] */ __RPC__in REFGUID guidVendor,
            /* [in] */ __RPC__in LPCWSTR pszMetric,
            /* [out] */ __RPC__deref_out_opt BSTR *pbstrValue);
        
        HRESULT ( STDMETHODCALLTYPE *EnumEEs )( 
            IDebugSettingsCallback2 * This,
            /* [in] */ DWORD celtBuffer,
            /* [length_is][size_is][full][out][in] */ __RPC__inout_ecount_part_opt(celtBuffer, *pceltEEs) GUID *rgguidLang,
            /* [length_is][size_is][full][out][in] */ __RPC__inout_ecount_part_opt(celtBuffer, *pceltEEs) GUID *rgguidVendor,
            /* [out][in] */ __RPC__inout DWORD *pceltEEs);
        
        HRESULT ( STDMETHODCALLTYPE *GetEEMetricDword )( 
            IDebugSettingsCallback2 * This,
            /* [in] */ __RPC__in REFGUID guidLang,
            /* [in] */ __RPC__in REFGUID guidVendor,
            /* [in] */ __RPC__in LPCWSTR pszMetric,
            /* [out] */ __RPC__out DWORD *pdwValue);
        
        HRESULT ( STDMETHODCALLTYPE *GetEELocalObject )( 
            IDebugSettingsCallback2 * This,
            /* [in] */ __RPC__in REFGUID guidLang,
            /* [in] */ __RPC__in REFGUID guidVendor,
            /* [in] */ __RPC__in LPCWSTR pszMetric,
            /* [out] */ __RPC__deref_out_opt IUnknown **ppUnk);
        
        HRESULT ( STDMETHODCALLTYPE *GetMetricString )( 
            IDebugSettingsCallback2 * This,
            /* [in] */ __RPC__in LPCWSTR pszType,
            /* [in] */ __RPC__in REFGUID guidSection,
            /* [in] */ __RPC__in LPCWSTR pszMetric,
            /* [out] */ __RPC__deref_out_opt BSTR *pbstrValue);
        
        END_INTERFACE
    } IDebugSettingsCallback2Vtbl;

    interface IDebugSettingsCallback2
    {
        CONST_VTBL struct IDebugSettingsCallback2Vtbl *lpVtbl;
    };

    

#ifdef COBJMACROS


#define IDebugSettingsCallback2_QueryInterface(This,riid,ppvObject)	\
    ( (This)->lpVtbl -> QueryInterface(This,riid,ppvObject) ) 

#define IDebugSettingsCallback2_AddRef(This)	\
    ( (This)->lpVtbl -> AddRef(This) ) 

#define IDebugSettingsCallback2_Release(This)	\
    ( (This)->lpVtbl -> Release(This) ) 


#define IDebugSettingsCallback2_GetMetricGuid(This,pszType,guidSection,pszMetric,pguidValue)	\
    ( (This)->lpVtbl -> GetMetricGuid(This,pszType,guidSection,pszMetric,pguidValue) ) 

#define IDebugSettingsCallback2_GetMetricDword(This,pszType,guidSection,pszMetric,pdwValue)	\
    ( (This)->lpVtbl -> GetMetricDword(This,pszType,guidSection,pszMetric,pdwValue) ) 

#define IDebugSettingsCallback2_GetEEMetricString(This,guidLang,guidVendor,pszMetric,pbstrValue)	\
    ( (This)->lpVtbl -> GetEEMetricString(This,guidLang,guidVendor,pszMetric,pbstrValue) ) 

#define IDebugSettingsCallback2_GetEEMetricGuid(This,guidLang,guidVendor,pszMetric,pguidValue)	\
    ( (This)->lpVtbl -> GetEEMetricGuid(This,guidLang,guidVendor,pszMetric,pguidValue) ) 

#define IDebugSettingsCallback2_GetEEMetricFile(This,guidLang,guidVendor,pszMetric,pbstrValue)	\
    ( (This)->lpVtbl -> GetEEMetricFile(This,guidLang,guidVendor,pszMetric,pbstrValue) ) 

#define IDebugSettingsCallback2_EnumEEs(This,celtBuffer,rgguidLang,rgguidVendor,pceltEEs)	\
    ( (This)->lpVtbl -> EnumEEs(This,celtBuffer,rgguidLang,rgguidVendor,pceltEEs) ) 

#define IDebugSettingsCallback2_GetEEMetricDword(This,guidLang,guidVendor,pszMetric,pdwValue)	\
    ( (This)->lpVtbl -> GetEEMetricDword(This,guidLang,guidVendor,pszMetric,pdwValue) ) 

#define IDebugSettingsCallback2_GetEELocalObject(This,guidLang,guidVendor,pszMetric,ppUnk)	\
    ( (This)->lpVtbl -> GetEELocalObject(This,guidLang,guidVendor,pszMetric,ppUnk) ) 

#define IDebugSettingsCallback2_GetMetricString(This,pszType,guidSection,pszMetric,pbstrValue)	\
    ( (This)->lpVtbl -> GetMetricString(This,pszType,guidSection,pszMetric,pbstrValue) ) 

#endif /* COBJMACROS */


#endif 	/* C style interface */




#endif 	/* __IDebugSettingsCallback2_INTERFACE_DEFINED__ */


#ifndef __IDebugEvent2_INTERFACE_DEFINED__
#define __IDebugEvent2_INTERFACE_DEFINED__

/* interface IDebugEvent2 */
/* [unique][uuid][object] */ 


EXTERN_C const IID IID_IDebugEvent2;

#if defined(__cplusplus) && !defined(CINTERFACE)
    
    MIDL_INTERFACE("423238d6-da42-4989-96fb-6bba26e72e09")
    IDebugEvent2 : public IUnknown
    {
    public:
        virtual HRESULT STDMETHODCALLTYPE GetAttributes( 
            /* [out] */ __RPC__out DWORD *pdwAttrib) = 0;
        
    };
    
#else 	/* C style interface */

    typedef struct IDebugEvent2Vtbl
    {
        BEGIN_INTERFACE
        
        HRESULT ( STDMETHODCALLTYPE *QueryInterface )( 
            IDebugEvent2 * This,
            /* [in] */ __RPC__in REFIID riid,
            /* [iid_is][out] */ 
            __RPC__deref_out  void **ppvObject);
        
        ULONG ( STDMETHODCALLTYPE *AddRef )( 
            IDebugEvent2 * This);
        
        ULONG ( STDMETHODCALLTYPE *Release )( 
            IDebugEvent2 * This);
        
        HRESULT ( STDMETHODCALLTYPE *GetAttributes )( 
            IDebugEvent2 * This,
            /* [out] */ __RPC__out DWORD *pdwAttrib);
        
        END_INTERFACE
    } IDebugEvent2Vtbl;

    interface IDebugEvent2
    {
        CONST_VTBL struct IDebugEvent2Vtbl *lpVtbl;
    };

    

#ifdef COBJMACROS


#define IDebugEvent2_QueryInterface(This,riid,ppvObject)	\
    ( (This)->lpVtbl -> QueryInterface(This,riid,ppvObject) ) 

#define IDebugEvent2_AddRef(This)	\
    ( (This)->lpVtbl -> AddRef(This) ) 

#define IDebugEvent2_Release(This)	\
    ( (This)->lpVtbl -> Release(This) ) 


#define IDebugEvent2_GetAttributes(This,pdwAttrib)	\
    ( (This)->lpVtbl -> GetAttributes(This,pdwAttrib) ) 

#endif /* COBJMACROS */


#endif 	/* C style interface */




#endif 	/* __IDebugEvent2_INTERFACE_DEFINED__ */


#ifndef __IDebugSessionCreateEvent2_INTERFACE_DEFINED__
#define __IDebugSessionCreateEvent2_INTERFACE_DEFINED__

/* interface IDebugSessionCreateEvent2 */
/* [unique][uuid][object] */ 


EXTERN_C const IID IID_IDebugSessionCreateEvent2;

#if defined(__cplusplus) && !defined(CINTERFACE)
    
    MIDL_INTERFACE("2c2b15b7-fc6d-45b3-9622-29665d964a76")
    IDebugSessionCreateEvent2 : public IUnknown
    {
    public:
    };
    
#else 	/* C style interface */

    typedef struct IDebugSessionCreateEvent2Vtbl
    {
        BEGIN_INTERFACE
        
        HRESULT ( STDMETHODCALLTYPE *QueryInterface )( 
            IDebugSessionCreateEvent2 * This,
            /* [in] */ __RPC__in REFIID riid,
            /* [iid_is][out] */ 
            __RPC__deref_out  void **ppvObject);
        
        ULONG ( STDMETHODCALLTYPE *AddRef )( 
            IDebugSessionCreateEvent2 * This);
        
        ULONG ( STDMETHODCALLTYPE *Release )( 
            IDebugSessionCreateEvent2 * This);
        
        END_INTERFACE
    } IDebugSessionCreateEvent2Vtbl;

    interface IDebugSessionCreateEvent2
    {
        CONST_VTBL struct IDebugSessionCreateEvent2Vtbl *lpVtbl;
    };

    

#ifdef COBJMACROS


#define IDebugSessionCreateEvent2_QueryInterface(This,riid,ppvObject)	\
    ( (This)->lpVtbl -> QueryInterface(This,riid,ppvObject) ) 

#define IDebugSessionCreateEvent2_AddRef(This)	\
    ( (This)->lpVtbl -> AddRef(This) ) 

#define IDebugSessionCreateEvent2_Release(This)	\
    ( (This)->lpVtbl -> Release(This) ) 


#endif /* COBJMACROS */


#endif 	/* C style interface */




#endif 	/* __IDebugSessionCreateEvent2_INTERFACE_DEFINED__ */


#ifndef __IDebugSessionDestroyEvent2_INTERFACE_DEFINED__
#define __IDebugSessionDestroyEvent2_INTERFACE_DEFINED__

/* interface IDebugSessionDestroyEvent2 */
/* [unique][uuid][object] */ 


EXTERN_C const IID IID_IDebugSessionDestroyEvent2;

#if defined(__cplusplus) && !defined(CINTERFACE)
    
    MIDL_INTERFACE("f199b2c2-88fe-4c5d-a0fd-aa046b0dc0dc")
    IDebugSessionDestroyEvent2 : public IUnknown
    {
    public:
    };
    
#else 	/* C style interface */

    typedef struct IDebugSessionDestroyEvent2Vtbl
    {
        BEGIN_INTERFACE
        
        HRESULT ( STDMETHODCALLTYPE *QueryInterface )( 
            IDebugSessionDestroyEvent2 * This,
            /* [in] */ __RPC__in REFIID riid,
            /* [iid_is][out] */ 
            __RPC__deref_out  void **ppvObject);
        
        ULONG ( STDMETHODCALLTYPE *AddRef )( 
            IDebugSessionDestroyEvent2 * This);
        
        ULONG ( STDMETHODCALLTYPE *Release )( 
            IDebugSessionDestroyEvent2 * This);
        
        END_INTERFACE
    } IDebugSessionDestroyEvent2Vtbl;

    interface IDebugSessionDestroyEvent2
    {
        CONST_VTBL struct IDebugSessionDestroyEvent2Vtbl *lpVtbl;
    };

    

#ifdef COBJMACROS


#define IDebugSessionDestroyEvent2_QueryInterface(This,riid,ppvObject)	\
    ( (This)->lpVtbl -> QueryInterface(This,riid,ppvObject) ) 

#define IDebugSessionDestroyEvent2_AddRef(This)	\
    ( (This)->lpVtbl -> AddRef(This) ) 

#define IDebugSessionDestroyEvent2_Release(This)	\
    ( (This)->lpVtbl -> Release(This) ) 


#endif /* COBJMACROS */


#endif 	/* C style interface */




#endif 	/* __IDebugSessionDestroyEvent2_INTERFACE_DEFINED__ */


#ifndef __IDebugEngineCreateEvent2_INTERFACE_DEFINED__
#define __IDebugEngineCreateEvent2_INTERFACE_DEFINED__

/* interface IDebugEngineCreateEvent2 */
/* [unique][uuid][object] */ 


EXTERN_C const IID IID_IDebugEngineCreateEvent2;

#if defined(__cplusplus) && !defined(CINTERFACE)
    
    MIDL_INTERFACE("fe5b734c-759d-4e59-ab04-f103343bdd06")
    IDebugEngineCreateEvent2 : public IUnknown
    {
    public:
        virtual HRESULT STDMETHODCALLTYPE GetEngine( 
            /* [out] */ __RPC__deref_out_opt IDebugEngine2 **pEngine) = 0;
        
    };
    
#else 	/* C style interface */

    typedef struct IDebugEngineCreateEvent2Vtbl
    {
        BEGIN_INTERFACE
        
        HRESULT ( STDMETHODCALLTYPE *QueryInterface )( 
            IDebugEngineCreateEvent2 * This,
            /* [in] */ __RPC__in REFIID riid,
            /* [iid_is][out] */ 
            __RPC__deref_out  void **ppvObject);
        
        ULONG ( STDMETHODCALLTYPE *AddRef )( 
            IDebugEngineCreateEvent2 * This);
        
        ULONG ( STDMETHODCALLTYPE *Release )( 
            IDebugEngineCreateEvent2 * This);
        
        HRESULT ( STDMETHODCALLTYPE *GetEngine )( 
            IDebugEngineCreateEvent2 * This,
            /* [out] */ __RPC__deref_out_opt IDebugEngine2 **pEngine);
        
        END_INTERFACE
    } IDebugEngineCreateEvent2Vtbl;

    interface IDebugEngineCreateEvent2
    {
        CONST_VTBL struct IDebugEngineCreateEvent2Vtbl *lpVtbl;
    };

    

#ifdef COBJMACROS


#define IDebugEngineCreateEvent2_QueryInterface(This,riid,ppvObject)	\
    ( (This)->lpVtbl -> QueryInterface(This,riid,ppvObject) ) 

#define IDebugEngineCreateEvent2_AddRef(This)	\
    ( (This)->lpVtbl -> AddRef(This) ) 

#define IDebugEngineCreateEvent2_Release(This)	\
    ( (This)->lpVtbl -> Release(This) ) 


#define IDebugEngineCreateEvent2_GetEngine(This,pEngine)	\
    ( (This)->lpVtbl -> GetEngine(This,pEngine) ) 

#endif /* COBJMACROS */


#endif 	/* C style interface */




#endif 	/* __IDebugEngineCreateEvent2_INTERFACE_DEFINED__ */


#ifndef __IDebugProcessCreateEvent2_INTERFACE_DEFINED__
#define __IDebugProcessCreateEvent2_INTERFACE_DEFINED__

/* interface IDebugProcessCreateEvent2 */
/* [unique][uuid][object] */ 


EXTERN_C const IID IID_IDebugProcessCreateEvent2;

#if defined(__cplusplus) && !defined(CINTERFACE)
    
    MIDL_INTERFACE("bac3780f-04da-4726-901c-ba6a4633e1ca")
    IDebugProcessCreateEvent2 : public IUnknown
    {
    public:
    };
    
#else 	/* C style interface */

    typedef struct IDebugProcessCreateEvent2Vtbl
    {
        BEGIN_INTERFACE
        
        HRESULT ( STDMETHODCALLTYPE *QueryInterface )( 
            IDebugProcessCreateEvent2 * This,
            /* [in] */ __RPC__in REFIID riid,
            /* [iid_is][out] */ 
            __RPC__deref_out  void **ppvObject);
        
        ULONG ( STDMETHODCALLTYPE *AddRef )( 
            IDebugProcessCreateEvent2 * This);
        
        ULONG ( STDMETHODCALLTYPE *Release )( 
            IDebugProcessCreateEvent2 * This);
        
        END_INTERFACE
    } IDebugProcessCreateEvent2Vtbl;

    interface IDebugProcessCreateEvent2
    {
        CONST_VTBL struct IDebugProcessCreateEvent2Vtbl *lpVtbl;
    };

    

#ifdef COBJMACROS


#define IDebugProcessCreateEvent2_QueryInterface(This,riid,ppvObject)	\
    ( (This)->lpVtbl -> QueryInterface(This,riid,ppvObject) ) 

#define IDebugProcessCreateEvent2_AddRef(This)	\
    ( (This)->lpVtbl -> AddRef(This) ) 

#define IDebugProcessCreateEvent2_Release(This)	\
    ( (This)->lpVtbl -> Release(This) ) 


#endif /* COBJMACROS */


#endif 	/* C style interface */




#endif 	/* __IDebugProcessCreateEvent2_INTERFACE_DEFINED__ */


#ifndef __IDebugProcessDestroyEvent2_INTERFACE_DEFINED__
#define __IDebugProcessDestroyEvent2_INTERFACE_DEFINED__

/* interface IDebugProcessDestroyEvent2 */
/* [unique][uuid][object] */ 


EXTERN_C const IID IID_IDebugProcessDestroyEvent2;

#if defined(__cplusplus) && !defined(CINTERFACE)
    
    MIDL_INTERFACE("3e2a0832-17e1-4886-8c0e-204da242995f")
    IDebugProcessDestroyEvent2 : public IUnknown
    {
    public:
    };
    
#else 	/* C style interface */

    typedef struct IDebugProcessDestroyEvent2Vtbl
    {
        BEGIN_INTERFACE
        
        HRESULT ( STDMETHODCALLTYPE *QueryInterface )( 
            IDebugProcessDestroyEvent2 * This,
            /* [in] */ __RPC__in REFIID riid,
            /* [iid_is][out] */ 
            __RPC__deref_out  void **ppvObject);
        
        ULONG ( STDMETHODCALLTYPE *AddRef )( 
            IDebugProcessDestroyEvent2 * This);
        
        ULONG ( STDMETHODCALLTYPE *Release )( 
            IDebugProcessDestroyEvent2 * This);
        
        END_INTERFACE
    } IDebugProcessDestroyEvent2Vtbl;

    interface IDebugProcessDestroyEvent2
    {
        CONST_VTBL struct IDebugProcessDestroyEvent2Vtbl *lpVtbl;
    };

    

#ifdef COBJMACROS


#define IDebugProcessDestroyEvent2_QueryInterface(This,riid,ppvObject)	\
    ( (This)->lpVtbl -> QueryInterface(This,riid,ppvObject) ) 

#define IDebugProcessDestroyEvent2_AddRef(This)	\
    ( (This)->lpVtbl -> AddRef(This) ) 

#define IDebugProcessDestroyEvent2_Release(This)	\
    ( (This)->lpVtbl -> Release(This) ) 


#endif /* COBJMACROS */


#endif 	/* C style interface */




#endif 	/* __IDebugProcessDestroyEvent2_INTERFACE_DEFINED__ */


#ifndef __IDebugProgramCreateEvent2_INTERFACE_DEFINED__
#define __IDebugProgramCreateEvent2_INTERFACE_DEFINED__

/* interface IDebugProgramCreateEvent2 */
/* [unique][uuid][object] */ 


EXTERN_C const IID IID_IDebugProgramCreateEvent2;

#if defined(__cplusplus) && !defined(CINTERFACE)
    
    MIDL_INTERFACE("96cd11ee-ecd4-4e89-957e-b5d496fc4139")
    IDebugProgramCreateEvent2 : public IUnknown
    {
    public:
    };
    
#else 	/* C style interface */

    typedef struct IDebugProgramCreateEvent2Vtbl
    {
        BEGIN_INTERFACE
        
        HRESULT ( STDMETHODCALLTYPE *QueryInterface )( 
            IDebugProgramCreateEvent2 * This,
            /* [in] */ __RPC__in REFIID riid,
            /* [iid_is][out] */ 
            __RPC__deref_out  void **ppvObject);
        
        ULONG ( STDMETHODCALLTYPE *AddRef )( 
            IDebugProgramCreateEvent2 * This);
        
        ULONG ( STDMETHODCALLTYPE *Release )( 
            IDebugProgramCreateEvent2 * This);
        
        END_INTERFACE
    } IDebugProgramCreateEvent2Vtbl;

    interface IDebugProgramCreateEvent2
    {
        CONST_VTBL struct IDebugProgramCreateEvent2Vtbl *lpVtbl;
    };

    

#ifdef COBJMACROS


#define IDebugProgramCreateEvent2_QueryInterface(This,riid,ppvObject)	\
    ( (This)->lpVtbl -> QueryInterface(This,riid,ppvObject) ) 

#define IDebugProgramCreateEvent2_AddRef(This)	\
    ( (This)->lpVtbl -> AddRef(This) ) 

#define IDebugProgramCreateEvent2_Release(This)	\
    ( (This)->lpVtbl -> Release(This) ) 


#endif /* COBJMACROS */


#endif 	/* C style interface */




#endif 	/* __IDebugProgramCreateEvent2_INTERFACE_DEFINED__ */


#ifndef __IDebugProgramDestroyEvent2_INTERFACE_DEFINED__
#define __IDebugProgramDestroyEvent2_INTERFACE_DEFINED__

/* interface IDebugProgramDestroyEvent2 */
/* [unique][uuid][object] */ 


EXTERN_C const IID IID_IDebugProgramDestroyEvent2;

#if defined(__cplusplus) && !defined(CINTERFACE)
    
    MIDL_INTERFACE("e147e9e3-6440-4073-a7b7-a65592c714b5")
    IDebugProgramDestroyEvent2 : public IUnknown
    {
    public:
        virtual HRESULT STDMETHODCALLTYPE GetExitCode( 
            /* [out] */ __RPC__out DWORD *pdwExit) = 0;
        
    };
    
#else 	/* C style interface */

    typedef struct IDebugProgramDestroyEvent2Vtbl
    {
        BEGIN_INTERFACE
        
        HRESULT ( STDMETHODCALLTYPE *QueryInterface )( 
            IDebugProgramDestroyEvent2 * This,
            /* [in] */ __RPC__in REFIID riid,
            /* [iid_is][out] */ 
            __RPC__deref_out  void **ppvObject);
        
        ULONG ( STDMETHODCALLTYPE *AddRef )( 
            IDebugProgramDestroyEvent2 * This);
        
        ULONG ( STDMETHODCALLTYPE *Release )( 
            IDebugProgramDestroyEvent2 * This);
        
        HRESULT ( STDMETHODCALLTYPE *GetExitCode )( 
            IDebugProgramDestroyEvent2 * This,
            /* [out] */ __RPC__out DWORD *pdwExit);
        
        END_INTERFACE
    } IDebugProgramDestroyEvent2Vtbl;

    interface IDebugProgramDestroyEvent2
    {
        CONST_VTBL struct IDebugProgramDestroyEvent2Vtbl *lpVtbl;
    };

    

#ifdef COBJMACROS


#define IDebugProgramDestroyEvent2_QueryInterface(This,riid,ppvObject)	\
    ( (This)->lpVtbl -> QueryInterface(This,riid,ppvObject) ) 

#define IDebugProgramDestroyEvent2_AddRef(This)	\
    ( (This)->lpVtbl -> AddRef(This) ) 

#define IDebugProgramDestroyEvent2_Release(This)	\
    ( (This)->lpVtbl -> Release(This) ) 


#define IDebugProgramDestroyEvent2_GetExitCode(This,pdwExit)	\
    ( (This)->lpVtbl -> GetExitCode(This,pdwExit) ) 

#endif /* COBJMACROS */


#endif 	/* C style interface */




#endif 	/* __IDebugProgramDestroyEvent2_INTERFACE_DEFINED__ */


#ifndef __IDebugProgramDestroyEventFlags2_INTERFACE_DEFINED__
#define __IDebugProgramDestroyEventFlags2_INTERFACE_DEFINED__

/* interface IDebugProgramDestroyEventFlags2 */
/* [unique][uuid][object] */ 


enum enum_PROGRAM_DESTROY_FLAGS
    {	PROGRAM_DESTROY_CONTINUE_DEBUGGING	= 0x1
    } ;
typedef DWORD PROGRAM_DESTROY_FLAGS;


EXTERN_C const IID IID_IDebugProgramDestroyEventFlags2;

#if defined(__cplusplus) && !defined(CINTERFACE)
    
    MIDL_INTERFACE("7e072bee-24e7-43eb-9bce-06402c70e018")
    IDebugProgramDestroyEventFlags2 : public IUnknown
    {
    public:
        virtual HRESULT STDMETHODCALLTYPE GetFlags( 
            /* [out] */ __RPC__out PROGRAM_DESTROY_FLAGS *pdwFlags) = 0;
        
    };
    
#else 	/* C style interface */

    typedef struct IDebugProgramDestroyEventFlags2Vtbl
    {
        BEGIN_INTERFACE
        
        HRESULT ( STDMETHODCALLTYPE *QueryInterface )( 
            IDebugProgramDestroyEventFlags2 * This,
            /* [in] */ __RPC__in REFIID riid,
            /* [iid_is][out] */ 
            __RPC__deref_out  void **ppvObject);
        
        ULONG ( STDMETHODCALLTYPE *AddRef )( 
            IDebugProgramDestroyEventFlags2 * This);
        
        ULONG ( STDMETHODCALLTYPE *Release )( 
            IDebugProgramDestroyEventFlags2 * This);
        
        HRESULT ( STDMETHODCALLTYPE *GetFlags )( 
            IDebugProgramDestroyEventFlags2 * This,
            /* [out] */ __RPC__out PROGRAM_DESTROY_FLAGS *pdwFlags);
        
        END_INTERFACE
    } IDebugProgramDestroyEventFlags2Vtbl;

    interface IDebugProgramDestroyEventFlags2
    {
        CONST_VTBL struct IDebugProgramDestroyEventFlags2Vtbl *lpVtbl;
    };

    

#ifdef COBJMACROS


#define IDebugProgramDestroyEventFlags2_QueryInterface(This,riid,ppvObject)	\
    ( (This)->lpVtbl -> QueryInterface(This,riid,ppvObject) ) 

#define IDebugProgramDestroyEventFlags2_AddRef(This)	\
    ( (This)->lpVtbl -> AddRef(This) ) 

#define IDebugProgramDestroyEventFlags2_Release(This)	\
    ( (This)->lpVtbl -> Release(This) ) 


#define IDebugProgramDestroyEventFlags2_GetFlags(This,pdwFlags)	\
    ( (This)->lpVtbl -> GetFlags(This,pdwFlags) ) 

#endif /* COBJMACROS */


#endif 	/* C style interface */




#endif 	/* __IDebugProgramDestroyEventFlags2_INTERFACE_DEFINED__ */


#ifndef __IDebugThreadCreateEvent2_INTERFACE_DEFINED__
#define __IDebugThreadCreateEvent2_INTERFACE_DEFINED__

/* interface IDebugThreadCreateEvent2 */
/* [unique][uuid][object] */ 


EXTERN_C const IID IID_IDebugThreadCreateEvent2;

#if defined(__cplusplus) && !defined(CINTERFACE)
    
    MIDL_INTERFACE("2090ccfc-70c5-491d-a5e8-bad2dd9ee3ea")
    IDebugThreadCreateEvent2 : public IUnknown
    {
    public:
    };
    
#else 	/* C style interface */

    typedef struct IDebugThreadCreateEvent2Vtbl
    {
        BEGIN_INTERFACE
        
        HRESULT ( STDMETHODCALLTYPE *QueryInterface )( 
            IDebugThreadCreateEvent2 * This,
            /* [in] */ __RPC__in REFIID riid,
            /* [iid_is][out] */ 
            __RPC__deref_out  void **ppvObject);
        
        ULONG ( STDMETHODCALLTYPE *AddRef )( 
            IDebugThreadCreateEvent2 * This);
        
        ULONG ( STDMETHODCALLTYPE *Release )( 
            IDebugThreadCreateEvent2 * This);
        
        END_INTERFACE
    } IDebugThreadCreateEvent2Vtbl;

    interface IDebugThreadCreateEvent2
    {
        CONST_VTBL struct IDebugThreadCreateEvent2Vtbl *lpVtbl;
    };

    

#ifdef COBJMACROS


#define IDebugThreadCreateEvent2_QueryInterface(This,riid,ppvObject)	\
    ( (This)->lpVtbl -> QueryInterface(This,riid,ppvObject) ) 

#define IDebugThreadCreateEvent2_AddRef(This)	\
    ( (This)->lpVtbl -> AddRef(This) ) 

#define IDebugThreadCreateEvent2_Release(This)	\
    ( (This)->lpVtbl -> Release(This) ) 


#endif /* COBJMACROS */


#endif 	/* C style interface */




#endif 	/* __IDebugThreadCreateEvent2_INTERFACE_DEFINED__ */


#ifndef __IDebugThreadDestroyEvent2_INTERFACE_DEFINED__
#define __IDebugThreadDestroyEvent2_INTERFACE_DEFINED__

/* interface IDebugThreadDestroyEvent2 */
/* [unique][uuid][object] */ 


EXTERN_C const IID IID_IDebugThreadDestroyEvent2;

#if defined(__cplusplus) && !defined(CINTERFACE)
    
    MIDL_INTERFACE("2c3b7532-a36f-4a6e-9072-49be649b8541")
    IDebugThreadDestroyEvent2 : public IUnknown
    {
    public:
        virtual HRESULT STDMETHODCALLTYPE GetExitCode( 
            /* [out] */ __RPC__out DWORD *pdwExit) = 0;
        
    };
    
#else 	/* C style interface */

    typedef struct IDebugThreadDestroyEvent2Vtbl
    {
        BEGIN_INTERFACE
        
        HRESULT ( STDMETHODCALLTYPE *QueryInterface )( 
            IDebugThreadDestroyEvent2 * This,
            /* [in] */ __RPC__in REFIID riid,
            /* [iid_is][out] */ 
            __RPC__deref_out  void **ppvObject);
        
        ULONG ( STDMETHODCALLTYPE *AddRef )( 
            IDebugThreadDestroyEvent2 * This);
        
        ULONG ( STDMETHODCALLTYPE *Release )( 
            IDebugThreadDestroyEvent2 * This);
        
        HRESULT ( STDMETHODCALLTYPE *GetExitCode )( 
            IDebugThreadDestroyEvent2 * This,
            /* [out] */ __RPC__out DWORD *pdwExit);
        
        END_INTERFACE
    } IDebugThreadDestroyEvent2Vtbl;

    interface IDebugThreadDestroyEvent2
    {
        CONST_VTBL struct IDebugThreadDestroyEvent2Vtbl *lpVtbl;
    };

    

#ifdef COBJMACROS


#define IDebugThreadDestroyEvent2_QueryInterface(This,riid,ppvObject)	\
    ( (This)->lpVtbl -> QueryInterface(This,riid,ppvObject) ) 

#define IDebugThreadDestroyEvent2_AddRef(This)	\
    ( (This)->lpVtbl -> AddRef(This) ) 

#define IDebugThreadDestroyEvent2_Release(This)	\
    ( (This)->lpVtbl -> Release(This) ) 


#define IDebugThreadDestroyEvent2_GetExitCode(This,pdwExit)	\
    ( (This)->lpVtbl -> GetExitCode(This,pdwExit) ) 

#endif /* COBJMACROS */


#endif 	/* C style interface */




#endif 	/* __IDebugThreadDestroyEvent2_INTERFACE_DEFINED__ */


#ifndef __IDebugLoadCompleteEvent2_INTERFACE_DEFINED__
#define __IDebugLoadCompleteEvent2_INTERFACE_DEFINED__

/* interface IDebugLoadCompleteEvent2 */
/* [unique][uuid][object] */ 


EXTERN_C const IID IID_IDebugLoadCompleteEvent2;

#if defined(__cplusplus) && !defined(CINTERFACE)
    
    MIDL_INTERFACE("b1844850-1349-45d4-9f12-495212f5eb0b")
    IDebugLoadCompleteEvent2 : public IUnknown
    {
    public:
    };
    
#else 	/* C style interface */

    typedef struct IDebugLoadCompleteEvent2Vtbl
    {
        BEGIN_INTERFACE
        
        HRESULT ( STDMETHODCALLTYPE *QueryInterface )( 
            IDebugLoadCompleteEvent2 * This,
            /* [in] */ __RPC__in REFIID riid,
            /* [iid_is][out] */ 
            __RPC__deref_out  void **ppvObject);
        
        ULONG ( STDMETHODCALLTYPE *AddRef )( 
            IDebugLoadCompleteEvent2 * This);
        
        ULONG ( STDMETHODCALLTYPE *Release )( 
            IDebugLoadCompleteEvent2 * This);
        
        END_INTERFACE
    } IDebugLoadCompleteEvent2Vtbl;

    interface IDebugLoadCompleteEvent2
    {
        CONST_VTBL struct IDebugLoadCompleteEvent2Vtbl *lpVtbl;
    };

    

#ifdef COBJMACROS


#define IDebugLoadCompleteEvent2_QueryInterface(This,riid,ppvObject)	\
    ( (This)->lpVtbl -> QueryInterface(This,riid,ppvObject) ) 

#define IDebugLoadCompleteEvent2_AddRef(This)	\
    ( (This)->lpVtbl -> AddRef(This) ) 

#define IDebugLoadCompleteEvent2_Release(This)	\
    ( (This)->lpVtbl -> Release(This) ) 


#endif /* COBJMACROS */


#endif 	/* C style interface */




#endif 	/* __IDebugLoadCompleteEvent2_INTERFACE_DEFINED__ */


#ifndef __IDebugEntryPointEvent2_INTERFACE_DEFINED__
#define __IDebugEntryPointEvent2_INTERFACE_DEFINED__

/* interface IDebugEntryPointEvent2 */
/* [unique][uuid][object] */ 


EXTERN_C const IID IID_IDebugEntryPointEvent2;

#if defined(__cplusplus) && !defined(CINTERFACE)
    
    MIDL_INTERFACE("e8414a3e-1642-48ec-829e-5f4040e16da9")
    IDebugEntryPointEvent2 : public IUnknown
    {
    public:
    };
    
#else 	/* C style interface */

    typedef struct IDebugEntryPointEvent2Vtbl
    {
        BEGIN_INTERFACE
        
        HRESULT ( STDMETHODCALLTYPE *QueryInterface )( 
            IDebugEntryPointEvent2 * This,
            /* [in] */ __RPC__in REFIID riid,
            /* [iid_is][out] */ 
            __RPC__deref_out  void **ppvObject);
        
        ULONG ( STDMETHODCALLTYPE *AddRef )( 
            IDebugEntryPointEvent2 * This);
        
        ULONG ( STDMETHODCALLTYPE *Release )( 
            IDebugEntryPointEvent2 * This);
        
        END_INTERFACE
    } IDebugEntryPointEvent2Vtbl;

    interface IDebugEntryPointEvent2
    {
        CONST_VTBL struct IDebugEntryPointEvent2Vtbl *lpVtbl;
    };

    

#ifdef COBJMACROS


#define IDebugEntryPointEvent2_QueryInterface(This,riid,ppvObject)	\
    ( (This)->lpVtbl -> QueryInterface(This,riid,ppvObject) ) 

#define IDebugEntryPointEvent2_AddRef(This)	\
    ( (This)->lpVtbl -> AddRef(This) ) 

#define IDebugEntryPointEvent2_Release(This)	\
    ( (This)->lpVtbl -> Release(This) ) 


#endif /* COBJMACROS */


#endif 	/* C style interface */




#endif 	/* __IDebugEntryPointEvent2_INTERFACE_DEFINED__ */


#ifndef __IDebugStepCompleteEvent2_INTERFACE_DEFINED__
#define __IDebugStepCompleteEvent2_INTERFACE_DEFINED__

/* interface IDebugStepCompleteEvent2 */
/* [unique][uuid][object] */ 


EXTERN_C const IID IID_IDebugStepCompleteEvent2;

#if defined(__cplusplus) && !defined(CINTERFACE)
    
    MIDL_INTERFACE("0f7f24c1-74d9-4ea6-a3ea-7edb2d81441d")
    IDebugStepCompleteEvent2 : public IUnknown
    {
    public:
    };
    
#else 	/* C style interface */

    typedef struct IDebugStepCompleteEvent2Vtbl
    {
        BEGIN_INTERFACE
        
        HRESULT ( STDMETHODCALLTYPE *QueryInterface )( 
            IDebugStepCompleteEvent2 * This,
            /* [in] */ __RPC__in REFIID riid,
            /* [iid_is][out] */ 
            __RPC__deref_out  void **ppvObject);
        
        ULONG ( STDMETHODCALLTYPE *AddRef )( 
            IDebugStepCompleteEvent2 * This);
        
        ULONG ( STDMETHODCALLTYPE *Release )( 
            IDebugStepCompleteEvent2 * This);
        
        END_INTERFACE
    } IDebugStepCompleteEvent2Vtbl;

    interface IDebugStepCompleteEvent2
    {
        CONST_VTBL struct IDebugStepCompleteEvent2Vtbl *lpVtbl;
    };

    

#ifdef COBJMACROS


#define IDebugStepCompleteEvent2_QueryInterface(This,riid,ppvObject)	\
    ( (This)->lpVtbl -> QueryInterface(This,riid,ppvObject) ) 

#define IDebugStepCompleteEvent2_AddRef(This)	\
    ( (This)->lpVtbl -> AddRef(This) ) 

#define IDebugStepCompleteEvent2_Release(This)	\
    ( (This)->lpVtbl -> Release(This) ) 


#endif /* COBJMACROS */


#endif 	/* C style interface */




#endif 	/* __IDebugStepCompleteEvent2_INTERFACE_DEFINED__ */


#ifndef __IDebugCanStopEvent2_INTERFACE_DEFINED__
#define __IDebugCanStopEvent2_INTERFACE_DEFINED__

/* interface IDebugCanStopEvent2 */
/* [unique][uuid][object] */ 


enum enum_CANSTOP_REASON
    {	CANSTOP_ENTRYPOINT	= 0,
	CANSTOP_STEPIN	= 0x1
    } ;
typedef DWORD CANSTOP_REASON;


EXTERN_C const IID IID_IDebugCanStopEvent2;

#if defined(__cplusplus) && !defined(CINTERFACE)
    
    MIDL_INTERFACE("b5b0d747-d4d2-4e2d-872d-74da22037826")
    IDebugCanStopEvent2 : public IUnknown
    {
    public:
        virtual HRESULT STDMETHODCALLTYPE GetReason( 
            /* [out] */ __RPC__out CANSTOP_REASON *pcr) = 0;
        
        virtual HRESULT STDMETHODCALLTYPE CanStop( 
            /* [in] */ BOOL fCanStop) = 0;
        
        virtual HRESULT STDMETHODCALLTYPE GetDocumentContext( 
            /* [out] */ __RPC__deref_out_opt IDebugDocumentContext2 **ppDocCxt) = 0;
        
        virtual HRESULT STDMETHODCALLTYPE GetCodeContext( 
            /* [out] */ __RPC__deref_out_opt IDebugCodeContext2 **ppCodeContext) = 0;
        
    };
    
#else 	/* C style interface */

    typedef struct IDebugCanStopEvent2Vtbl
    {
        BEGIN_INTERFACE
        
        HRESULT ( STDMETHODCALLTYPE *QueryInterface )( 
            IDebugCanStopEvent2 * This,
            /* [in] */ __RPC__in REFIID riid,
            /* [iid_is][out] */ 
            __RPC__deref_out  void **ppvObject);
        
        ULONG ( STDMETHODCALLTYPE *AddRef )( 
            IDebugCanStopEvent2 * This);
        
        ULONG ( STDMETHODCALLTYPE *Release )( 
            IDebugCanStopEvent2 * This);
        
        HRESULT ( STDMETHODCALLTYPE *GetReason )( 
            IDebugCanStopEvent2 * This,
            /* [out] */ __RPC__out CANSTOP_REASON *pcr);
        
        HRESULT ( STDMETHODCALLTYPE *CanStop )( 
            IDebugCanStopEvent2 * This,
            /* [in] */ BOOL fCanStop);
        
        HRESULT ( STDMETHODCALLTYPE *GetDocumentContext )( 
            IDebugCanStopEvent2 * This,
            /* [out] */ __RPC__deref_out_opt IDebugDocumentContext2 **ppDocCxt);
        
        HRESULT ( STDMETHODCALLTYPE *GetCodeContext )( 
            IDebugCanStopEvent2 * This,
            /* [out] */ __RPC__deref_out_opt IDebugCodeContext2 **ppCodeContext);
        
        END_INTERFACE
    } IDebugCanStopEvent2Vtbl;

    interface IDebugCanStopEvent2
    {
        CONST_VTBL struct IDebugCanStopEvent2Vtbl *lpVtbl;
    };

    

#ifdef COBJMACROS


#define IDebugCanStopEvent2_QueryInterface(This,riid,ppvObject)	\
    ( (This)->lpVtbl -> QueryInterface(This,riid,ppvObject) ) 

#define IDebugCanStopEvent2_AddRef(This)	\
    ( (This)->lpVtbl -> AddRef(This) ) 

#define IDebugCanStopEvent2_Release(This)	\
    ( (This)->lpVtbl -> Release(This) ) 


#define IDebugCanStopEvent2_GetReason(This,pcr)	\
    ( (This)->lpVtbl -> GetReason(This,pcr) ) 

#define IDebugCanStopEvent2_CanStop(This,fCanStop)	\
    ( (This)->lpVtbl -> CanStop(This,fCanStop) ) 

#define IDebugCanStopEvent2_GetDocumentContext(This,ppDocCxt)	\
    ( (This)->lpVtbl -> GetDocumentContext(This,ppDocCxt) ) 

#define IDebugCanStopEvent2_GetCodeContext(This,ppCodeContext)	\
    ( (This)->lpVtbl -> GetCodeContext(This,ppCodeContext) ) 

#endif /* COBJMACROS */


#endif 	/* C style interface */




#endif 	/* __IDebugCanStopEvent2_INTERFACE_DEFINED__ */


#ifndef __IDebugBreakEvent2_INTERFACE_DEFINED__
#define __IDebugBreakEvent2_INTERFACE_DEFINED__

/* interface IDebugBreakEvent2 */
/* [unique][uuid][object] */ 


EXTERN_C const IID IID_IDebugBreakEvent2;

#if defined(__cplusplus) && !defined(CINTERFACE)
    
    MIDL_INTERFACE("c7405d1d-e24b-44e0-b707-d8a5a4e1641b")
    IDebugBreakEvent2 : public IUnknown
    {
    public:
    };
    
#else 	/* C style interface */

    typedef struct IDebugBreakEvent2Vtbl
    {
        BEGIN_INTERFACE
        
        HRESULT ( STDMETHODCALLTYPE *QueryInterface )( 
            IDebugBreakEvent2 * This,
            /* [in] */ __RPC__in REFIID riid,
            /* [iid_is][out] */ 
            __RPC__deref_out  void **ppvObject);
        
        ULONG ( STDMETHODCALLTYPE *AddRef )( 
            IDebugBreakEvent2 * This);
        
        ULONG ( STDMETHODCALLTYPE *Release )( 
            IDebugBreakEvent2 * This);
        
        END_INTERFACE
    } IDebugBreakEvent2Vtbl;

    interface IDebugBreakEvent2
    {
        CONST_VTBL struct IDebugBreakEvent2Vtbl *lpVtbl;
    };

    

#ifdef COBJMACROS


#define IDebugBreakEvent2_QueryInterface(This,riid,ppvObject)	\
    ( (This)->lpVtbl -> QueryInterface(This,riid,ppvObject) ) 

#define IDebugBreakEvent2_AddRef(This)	\
    ( (This)->lpVtbl -> AddRef(This) ) 

#define IDebugBreakEvent2_Release(This)	\
    ( (This)->lpVtbl -> Release(This) ) 


#endif /* COBJMACROS */


#endif 	/* C style interface */




#endif 	/* __IDebugBreakEvent2_INTERFACE_DEFINED__ */


#ifndef __IDebugBreakpointEvent2_INTERFACE_DEFINED__
#define __IDebugBreakpointEvent2_INTERFACE_DEFINED__

/* interface IDebugBreakpointEvent2 */
/* [unique][uuid][object] */ 


EXTERN_C const IID IID_IDebugBreakpointEvent2;

#if defined(__cplusplus) && !defined(CINTERFACE)
    
    MIDL_INTERFACE("501c1e21-c557-48b8-ba30-a1eab0bc4a74")
    IDebugBreakpointEvent2 : public IUnknown
    {
    public:
        virtual HRESULT STDMETHODCALLTYPE EnumBreakpoints( 
            /* [out] */ __RPC__deref_out_opt IEnumDebugBoundBreakpoints2 **ppEnum) = 0;
        
    };
    
#else 	/* C style interface */

    typedef struct IDebugBreakpointEvent2Vtbl
    {
        BEGIN_INTERFACE
        
        HRESULT ( STDMETHODCALLTYPE *QueryInterface )( 
            IDebugBreakpointEvent2 * This,
            /* [in] */ __RPC__in REFIID riid,
            /* [iid_is][out] */ 
            __RPC__deref_out  void **ppvObject);
        
        ULONG ( STDMETHODCALLTYPE *AddRef )( 
            IDebugBreakpointEvent2 * This);
        
        ULONG ( STDMETHODCALLTYPE *Release )( 
            IDebugBreakpointEvent2 * This);
        
        HRESULT ( STDMETHODCALLTYPE *EnumBreakpoints )( 
            IDebugBreakpointEvent2 * This,
            /* [out] */ __RPC__deref_out_opt IEnumDebugBoundBreakpoints2 **ppEnum);
        
        END_INTERFACE
    } IDebugBreakpointEvent2Vtbl;

    interface IDebugBreakpointEvent2
    {
        CONST_VTBL struct IDebugBreakpointEvent2Vtbl *lpVtbl;
    };

    

#ifdef COBJMACROS


#define IDebugBreakpointEvent2_QueryInterface(This,riid,ppvObject)	\
    ( (This)->lpVtbl -> QueryInterface(This,riid,ppvObject) ) 

#define IDebugBreakpointEvent2_AddRef(This)	\
    ( (This)->lpVtbl -> AddRef(This) ) 

#define IDebugBreakpointEvent2_Release(This)	\
    ( (This)->lpVtbl -> Release(This) ) 


#define IDebugBreakpointEvent2_EnumBreakpoints(This,ppEnum)	\
    ( (This)->lpVtbl -> EnumBreakpoints(This,ppEnum) ) 

#endif /* COBJMACROS */


#endif 	/* C style interface */




#endif 	/* __IDebugBreakpointEvent2_INTERFACE_DEFINED__ */


#ifndef __IDebugExceptionEvent2_INTERFACE_DEFINED__
#define __IDebugExceptionEvent2_INTERFACE_DEFINED__

/* interface IDebugExceptionEvent2 */
/* [unique][uuid][object] */ 


EXTERN_C const IID IID_IDebugExceptionEvent2;

#if defined(__cplusplus) && !defined(CINTERFACE)
    
    MIDL_INTERFACE("51a94113-8788-4a54-ae15-08b74ff922d0")
    IDebugExceptionEvent2 : public IUnknown
    {
    public:
        virtual HRESULT STDMETHODCALLTYPE GetException( 
            /* [out] */ __RPC__out EXCEPTION_INFO *pExceptionInfo) = 0;
        
        virtual HRESULT STDMETHODCALLTYPE GetExceptionDescription( 
            /* [out] */ __RPC__deref_out_opt BSTR *pbstrDescription) = 0;
        
        virtual HRESULT STDMETHODCALLTYPE CanPassToDebuggee( void) = 0;
        
        virtual HRESULT STDMETHODCALLTYPE PassToDebuggee( 
            /* [in] */ BOOL fPass) = 0;
        
    };
    
#else 	/* C style interface */

    typedef struct IDebugExceptionEvent2Vtbl
    {
        BEGIN_INTERFACE
        
        HRESULT ( STDMETHODCALLTYPE *QueryInterface )( 
            IDebugExceptionEvent2 * This,
            /* [in] */ __RPC__in REFIID riid,
            /* [iid_is][out] */ 
            __RPC__deref_out  void **ppvObject);
        
        ULONG ( STDMETHODCALLTYPE *AddRef )( 
            IDebugExceptionEvent2 * This);
        
        ULONG ( STDMETHODCALLTYPE *Release )( 
            IDebugExceptionEvent2 * This);
        
        HRESULT ( STDMETHODCALLTYPE *GetException )( 
            IDebugExceptionEvent2 * This,
            /* [out] */ __RPC__out EXCEPTION_INFO *pExceptionInfo);
        
        HRESULT ( STDMETHODCALLTYPE *GetExceptionDescription )( 
            IDebugExceptionEvent2 * This,
            /* [out] */ __RPC__deref_out_opt BSTR *pbstrDescription);
        
        HRESULT ( STDMETHODCALLTYPE *CanPassToDebuggee )( 
            IDebugExceptionEvent2 * This);
        
        HRESULT ( STDMETHODCALLTYPE *PassToDebuggee )( 
            IDebugExceptionEvent2 * This,
            /* [in] */ BOOL fPass);
        
        END_INTERFACE
    } IDebugExceptionEvent2Vtbl;

    interface IDebugExceptionEvent2
    {
        CONST_VTBL struct IDebugExceptionEvent2Vtbl *lpVtbl;
    };

    

#ifdef COBJMACROS


#define IDebugExceptionEvent2_QueryInterface(This,riid,ppvObject)	\
    ( (This)->lpVtbl -> QueryInterface(This,riid,ppvObject) ) 

#define IDebugExceptionEvent2_AddRef(This)	\
    ( (This)->lpVtbl -> AddRef(This) ) 

#define IDebugExceptionEvent2_Release(This)	\
    ( (This)->lpVtbl -> Release(This) ) 


#define IDebugExceptionEvent2_GetException(This,pExceptionInfo)	\
    ( (This)->lpVtbl -> GetException(This,pExceptionInfo) ) 

#define IDebugExceptionEvent2_GetExceptionDescription(This,pbstrDescription)	\
    ( (This)->lpVtbl -> GetExceptionDescription(This,pbstrDescription) ) 

#define IDebugExceptionEvent2_CanPassToDebuggee(This)	\
    ( (This)->lpVtbl -> CanPassToDebuggee(This) ) 

#define IDebugExceptionEvent2_PassToDebuggee(This,fPass)	\
    ( (This)->lpVtbl -> PassToDebuggee(This,fPass) ) 

#endif /* COBJMACROS */


#endif 	/* C style interface */




#endif 	/* __IDebugExceptionEvent2_INTERFACE_DEFINED__ */


/* interface __MIDL_itf_msdbg_0000_0040 */
/* [local] */ 

typedef struct _NATIVE_EXCEPTION_INFO
    {
    DWORD ExceptionCode;
    DWORD ExceptionFlags;
    DWORD NumberOfParameters;
    UINT64 ExceptionInformation[ 15 ];
    } 	NATIVE_EXCEPTION_INFO;



extern RPC_IF_HANDLE __MIDL_itf_msdbg_0000_0040_v0_0_c_ifspec;
extern RPC_IF_HANDLE __MIDL_itf_msdbg_0000_0040_v0_0_s_ifspec;

#ifndef __IDebugNativeExceptionInfo_INTERFACE_DEFINED__
#define __IDebugNativeExceptionInfo_INTERFACE_DEFINED__

/* interface IDebugNativeExceptionInfo */
/* [unique][uuid][object] */ 


EXTERN_C const IID IID_IDebugNativeExceptionInfo;

#if defined(__cplusplus) && !defined(CINTERFACE)
    
    MIDL_INTERFACE("3d320710-06c0-437b-a55f-826f48cc7ee7")
    IDebugNativeExceptionInfo : public IUnknown
    {
    public:
        virtual HRESULT STDMETHODCALLTYPE GetNativeException( 
            /* [out] */ __RPC__out NATIVE_EXCEPTION_INFO *pExceptionInfo) = 0;
        
    };
    
#else 	/* C style interface */

    typedef struct IDebugNativeExceptionInfoVtbl
    {
        BEGIN_INTERFACE
        
        HRESULT ( STDMETHODCALLTYPE *QueryInterface )( 
            IDebugNativeExceptionInfo * This,
            /* [in] */ __RPC__in REFIID riid,
            /* [iid_is][out] */ 
            __RPC__deref_out  void **ppvObject);
        
        ULONG ( STDMETHODCALLTYPE *AddRef )( 
            IDebugNativeExceptionInfo * This);
        
        ULONG ( STDMETHODCALLTYPE *Release )( 
            IDebugNativeExceptionInfo * This);
        
        HRESULT ( STDMETHODCALLTYPE *GetNativeException )( 
            IDebugNativeExceptionInfo * This,
            /* [out] */ __RPC__out NATIVE_EXCEPTION_INFO *pExceptionInfo);
        
        END_INTERFACE
    } IDebugNativeExceptionInfoVtbl;

    interface IDebugNativeExceptionInfo
    {
        CONST_VTBL struct IDebugNativeExceptionInfoVtbl *lpVtbl;
    };

    

#ifdef COBJMACROS


#define IDebugNativeExceptionInfo_QueryInterface(This,riid,ppvObject)	\
    ( (This)->lpVtbl -> QueryInterface(This,riid,ppvObject) ) 

#define IDebugNativeExceptionInfo_AddRef(This)	\
    ( (This)->lpVtbl -> AddRef(This) ) 

#define IDebugNativeExceptionInfo_Release(This)	\
    ( (This)->lpVtbl -> Release(This) ) 


#define IDebugNativeExceptionInfo_GetNativeException(This,pExceptionInfo)	\
    ( (This)->lpVtbl -> GetNativeException(This,pExceptionInfo) ) 

#endif /* COBJMACROS */


#endif 	/* C style interface */




#endif 	/* __IDebugNativeExceptionInfo_INTERFACE_DEFINED__ */


#ifndef __IDebugManagedExceptionInfo2_INTERFACE_DEFINED__
#define __IDebugManagedExceptionInfo2_INTERFACE_DEFINED__

/* interface IDebugManagedExceptionInfo2 */
/* [unique][uuid][object] */ 


enum tagEXCEPTION_BOUNDARY_TYPE
    {	EXCEPTION_BOUNDARY_NONE	= 0,
	EXCEPTION_BOUNDARY_APPDOMAIN	= ( EXCEPTION_BOUNDARY_NONE + 1 ) ,
	EXCEPTION_BOUNDARY_UNMANAGED	= ( EXCEPTION_BOUNDARY_APPDOMAIN + 1 ) 
    } ;
typedef enum tagEXCEPTION_BOUNDARY_TYPE EXCEPTION_BOUNDARY_TYPE;


EXTERN_C const IID IID_IDebugManagedExceptionInfo2;

#if defined(__cplusplus) && !defined(CINTERFACE)
    
    MIDL_INTERFACE("d288564a-edb2-4214-8690-ff9a82870379")
    IDebugManagedExceptionInfo2 : public IUnknown
    {
    public:
        virtual HRESULT STDMETHODCALLTYPE GetExceptionMessage( 
            /* [out] */ __RPC__deref_out_opt BSTR *pbstrExceptionMessage) = 0;
        
        virtual HRESULT STDMETHODCALLTYPE GetExceptionBoundaryType( 
            /* [out] */ __RPC__out EXCEPTION_BOUNDARY_TYPE *pType) = 0;
        
    };
    
#else 	/* C style interface */

    typedef struct IDebugManagedExceptionInfo2Vtbl
    {
        BEGIN_INTERFACE
        
        HRESULT ( STDMETHODCALLTYPE *QueryInterface )( 
            IDebugManagedExceptionInfo2 * This,
            /* [in] */ __RPC__in REFIID riid,
            /* [iid_is][out] */ 
            __RPC__deref_out  void **ppvObject);
        
        ULONG ( STDMETHODCALLTYPE *AddRef )( 
            IDebugManagedExceptionInfo2 * This);
        
        ULONG ( STDMETHODCALLTYPE *Release )( 
            IDebugManagedExceptionInfo2 * This);
        
        HRESULT ( STDMETHODCALLTYPE *GetExceptionMessage )( 
            IDebugManagedExceptionInfo2 * This,
            /* [out] */ __RPC__deref_out_opt BSTR *pbstrExceptionMessage);
        
        HRESULT ( STDMETHODCALLTYPE *GetExceptionBoundaryType )( 
            IDebugManagedExceptionInfo2 * This,
            /* [out] */ __RPC__out EXCEPTION_BOUNDARY_TYPE *pType);
        
        END_INTERFACE
    } IDebugManagedExceptionInfo2Vtbl;

    interface IDebugManagedExceptionInfo2
    {
        CONST_VTBL struct IDebugManagedExceptionInfo2Vtbl *lpVtbl;
    };

    

#ifdef COBJMACROS


#define IDebugManagedExceptionInfo2_QueryInterface(This,riid,ppvObject)	\
    ( (This)->lpVtbl -> QueryInterface(This,riid,ppvObject) ) 

#define IDebugManagedExceptionInfo2_AddRef(This)	\
    ( (This)->lpVtbl -> AddRef(This) ) 

#define IDebugManagedExceptionInfo2_Release(This)	\
    ( (This)->lpVtbl -> Release(This) ) 


#define IDebugManagedExceptionInfo2_GetExceptionMessage(This,pbstrExceptionMessage)	\
    ( (This)->lpVtbl -> GetExceptionMessage(This,pbstrExceptionMessage) ) 

#define IDebugManagedExceptionInfo2_GetExceptionBoundaryType(This,pType)	\
    ( (This)->lpVtbl -> GetExceptionBoundaryType(This,pType) ) 

#endif /* COBJMACROS */


#endif 	/* C style interface */




#endif 	/* __IDebugManagedExceptionInfo2_INTERFACE_DEFINED__ */


#ifndef __IDebugOutputStringEvent2_INTERFACE_DEFINED__
#define __IDebugOutputStringEvent2_INTERFACE_DEFINED__

/* interface IDebugOutputStringEvent2 */
/* [unique][uuid][object] */ 


EXTERN_C const IID IID_IDebugOutputStringEvent2;

#if defined(__cplusplus) && !defined(CINTERFACE)
    
    MIDL_INTERFACE("569c4bb1-7b82-46fc-ae28-4536ddad753e")
    IDebugOutputStringEvent2 : public IUnknown
    {
    public:
        virtual HRESULT STDMETHODCALLTYPE GetString( 
            /* [out] */ __RPC__deref_out_opt BSTR *pbstrString) = 0;
        
    };
    
#else 	/* C style interface */

    typedef struct IDebugOutputStringEvent2Vtbl
    {
        BEGIN_INTERFACE
        
        HRESULT ( STDMETHODCALLTYPE *QueryInterface )( 
            IDebugOutputStringEvent2 * This,
            /* [in] */ __RPC__in REFIID riid,
            /* [iid_is][out] */ 
            __RPC__deref_out  void **ppvObject);
        
        ULONG ( STDMETHODCALLTYPE *AddRef )( 
            IDebugOutputStringEvent2 * This);
        
        ULONG ( STDMETHODCALLTYPE *Release )( 
            IDebugOutputStringEvent2 * This);
        
        HRESULT ( STDMETHODCALLTYPE *GetString )( 
            IDebugOutputStringEvent2 * This,
            /* [out] */ __RPC__deref_out_opt BSTR *pbstrString);
        
        END_INTERFACE
    } IDebugOutputStringEvent2Vtbl;

    interface IDebugOutputStringEvent2
    {
        CONST_VTBL struct IDebugOutputStringEvent2Vtbl *lpVtbl;
    };

    

#ifdef COBJMACROS


#define IDebugOutputStringEvent2_QueryInterface(This,riid,ppvObject)	\
    ( (This)->lpVtbl -> QueryInterface(This,riid,ppvObject) ) 

#define IDebugOutputStringEvent2_AddRef(This)	\
    ( (This)->lpVtbl -> AddRef(This) ) 

#define IDebugOutputStringEvent2_Release(This)	\
    ( (This)->lpVtbl -> Release(This) ) 


#define IDebugOutputStringEvent2_GetString(This,pbstrString)	\
    ( (This)->lpVtbl -> GetString(This,pbstrString) ) 

#endif /* COBJMACROS */


#endif 	/* C style interface */




#endif 	/* __IDebugOutputStringEvent2_INTERFACE_DEFINED__ */


#ifndef __IDebugModuleLoadEvent2_INTERFACE_DEFINED__
#define __IDebugModuleLoadEvent2_INTERFACE_DEFINED__

/* interface IDebugModuleLoadEvent2 */
/* [unique][uuid][object] */ 


EXTERN_C const IID IID_IDebugModuleLoadEvent2;

#if defined(__cplusplus) && !defined(CINTERFACE)
    
    MIDL_INTERFACE("989db083-0d7c-40d1-a9d9-921bf611a4b2")
    IDebugModuleLoadEvent2 : public IUnknown
    {
    public:
        virtual HRESULT STDMETHODCALLTYPE GetModule( 
            /* [out] */ __RPC__deref_out_opt IDebugModule2 **pModule,
            /* [out][in] */ __RPC__deref_inout_opt BSTR *pbstrDebugMessage,
            /* [out][in] */ __RPC__inout BOOL *pbLoad) = 0;
        
    };
    
#else 	/* C style interface */

    typedef struct IDebugModuleLoadEvent2Vtbl
    {
        BEGIN_INTERFACE
        
        HRESULT ( STDMETHODCALLTYPE *QueryInterface )( 
            IDebugModuleLoadEvent2 * This,
            /* [in] */ __RPC__in REFIID riid,
            /* [iid_is][out] */ 
            __RPC__deref_out  void **ppvObject);
        
        ULONG ( STDMETHODCALLTYPE *AddRef )( 
            IDebugModuleLoadEvent2 * This);
        
        ULONG ( STDMETHODCALLTYPE *Release )( 
            IDebugModuleLoadEvent2 * This);
        
        HRESULT ( STDMETHODCALLTYPE *GetModule )( 
            IDebugModuleLoadEvent2 * This,
            /* [out] */ __RPC__deref_out_opt IDebugModule2 **pModule,
            /* [out][in] */ __RPC__deref_inout_opt BSTR *pbstrDebugMessage,
            /* [out][in] */ __RPC__inout BOOL *pbLoad);
        
        END_INTERFACE
    } IDebugModuleLoadEvent2Vtbl;

    interface IDebugModuleLoadEvent2
    {
        CONST_VTBL struct IDebugModuleLoadEvent2Vtbl *lpVtbl;
    };

    

#ifdef COBJMACROS


#define IDebugModuleLoadEvent2_QueryInterface(This,riid,ppvObject)	\
    ( (This)->lpVtbl -> QueryInterface(This,riid,ppvObject) ) 

#define IDebugModuleLoadEvent2_AddRef(This)	\
    ( (This)->lpVtbl -> AddRef(This) ) 

#define IDebugModuleLoadEvent2_Release(This)	\
    ( (This)->lpVtbl -> Release(This) ) 


#define IDebugModuleLoadEvent2_GetModule(This,pModule,pbstrDebugMessage,pbLoad)	\
    ( (This)->lpVtbl -> GetModule(This,pModule,pbstrDebugMessage,pbLoad) ) 

#endif /* COBJMACROS */


#endif 	/* C style interface */




#endif 	/* __IDebugModuleLoadEvent2_INTERFACE_DEFINED__ */


#ifndef __IDebugSymbolSearchEvent2_INTERFACE_DEFINED__
#define __IDebugSymbolSearchEvent2_INTERFACE_DEFINED__

/* interface IDebugSymbolSearchEvent2 */
/* [unique][uuid][object] */ 


enum enum_MODULE_INFO_FLAGS
    {	MIF_SYMBOLS_LOADED	= 0x1
    } ;
typedef DWORD MODULE_INFO_FLAGS;


EXTERN_C const IID IID_IDebugSymbolSearchEvent2;

#if defined(__cplusplus) && !defined(CINTERFACE)
    
    MIDL_INTERFACE("638F7C54-C160-4c7b-B2D0-E0337BC61F8C")
    IDebugSymbolSearchEvent2 : public IUnknown
    {
    public:
        virtual HRESULT STDMETHODCALLTYPE GetSymbolSearchInfo( 
            /* [out] */ __RPC__deref_out_opt IDebugModule3 **pModule,
            /* [out][in] */ __RPC__deref_inout_opt BSTR *pbstrDebugMessage,
            /* [out] */ __RPC__out MODULE_INFO_FLAGS *pdwModuleInfoFlags) = 0;
        
    };
    
#else 	/* C style interface */

    typedef struct IDebugSymbolSearchEvent2Vtbl
    {
        BEGIN_INTERFACE
        
        HRESULT ( STDMETHODCALLTYPE *QueryInterface )( 
            IDebugSymbolSearchEvent2 * This,
            /* [in] */ __RPC__in REFIID riid,
            /* [iid_is][out] */ 
            __RPC__deref_out  void **ppvObject);
        
        ULONG ( STDMETHODCALLTYPE *AddRef )( 
            IDebugSymbolSearchEvent2 * This);
        
        ULONG ( STDMETHODCALLTYPE *Release )( 
            IDebugSymbolSearchEvent2 * This);
        
        HRESULT ( STDMETHODCALLTYPE *GetSymbolSearchInfo )( 
            IDebugSymbolSearchEvent2 * This,
            /* [out] */ __RPC__deref_out_opt IDebugModule3 **pModule,
            /* [out][in] */ __RPC__deref_inout_opt BSTR *pbstrDebugMessage,
            /* [out] */ __RPC__out MODULE_INFO_FLAGS *pdwModuleInfoFlags);
        
        END_INTERFACE
    } IDebugSymbolSearchEvent2Vtbl;

    interface IDebugSymbolSearchEvent2
    {
        CONST_VTBL struct IDebugSymbolSearchEvent2Vtbl *lpVtbl;
    };

    

#ifdef COBJMACROS


#define IDebugSymbolSearchEvent2_QueryInterface(This,riid,ppvObject)	\
    ( (This)->lpVtbl -> QueryInterface(This,riid,ppvObject) ) 

#define IDebugSymbolSearchEvent2_AddRef(This)	\
    ( (This)->lpVtbl -> AddRef(This) ) 

#define IDebugSymbolSearchEvent2_Release(This)	\
    ( (This)->lpVtbl -> Release(This) ) 


#define IDebugSymbolSearchEvent2_GetSymbolSearchInfo(This,pModule,pbstrDebugMessage,pdwModuleInfoFlags)	\
    ( (This)->lpVtbl -> GetSymbolSearchInfo(This,pModule,pbstrDebugMessage,pdwModuleInfoFlags) ) 

#endif /* COBJMACROS */


#endif 	/* C style interface */




#endif 	/* __IDebugSymbolSearchEvent2_INTERFACE_DEFINED__ */


#ifndef __IDebugBeforeSymbolSearchEvent2_INTERFACE_DEFINED__
#define __IDebugBeforeSymbolSearchEvent2_INTERFACE_DEFINED__

/* interface IDebugBeforeSymbolSearchEvent2 */
/* [unique][uuid][object] */ 


EXTERN_C const IID IID_IDebugBeforeSymbolSearchEvent2;

#if defined(__cplusplus) && !defined(CINTERFACE)
    
    MIDL_INTERFACE("B15C8149-2B81-40ae-9388-62FA276AE14C")
    IDebugBeforeSymbolSearchEvent2 : public IUnknown
    {
    public:
        virtual HRESULT STDMETHODCALLTYPE GetModuleName( 
            /* [out] */ __RPC__deref_out_opt BSTR *pbstrModuleName) = 0;
        
    };
    
#else 	/* C style interface */

    typedef struct IDebugBeforeSymbolSearchEvent2Vtbl
    {
        BEGIN_INTERFACE
        
        HRESULT ( STDMETHODCALLTYPE *QueryInterface )( 
            IDebugBeforeSymbolSearchEvent2 * This,
            /* [in] */ __RPC__in REFIID riid,
            /* [iid_is][out] */ 
            __RPC__deref_out  void **ppvObject);
        
        ULONG ( STDMETHODCALLTYPE *AddRef )( 
            IDebugBeforeSymbolSearchEvent2 * This);
        
        ULONG ( STDMETHODCALLTYPE *Release )( 
            IDebugBeforeSymbolSearchEvent2 * This);
        
        HRESULT ( STDMETHODCALLTYPE *GetModuleName )( 
            IDebugBeforeSymbolSearchEvent2 * This,
            /* [out] */ __RPC__deref_out_opt BSTR *pbstrModuleName);
        
        END_INTERFACE
    } IDebugBeforeSymbolSearchEvent2Vtbl;

    interface IDebugBeforeSymbolSearchEvent2
    {
        CONST_VTBL struct IDebugBeforeSymbolSearchEvent2Vtbl *lpVtbl;
    };

    

#ifdef COBJMACROS


#define IDebugBeforeSymbolSearchEvent2_QueryInterface(This,riid,ppvObject)	\
    ( (This)->lpVtbl -> QueryInterface(This,riid,ppvObject) ) 

#define IDebugBeforeSymbolSearchEvent2_AddRef(This)	\
    ( (This)->lpVtbl -> AddRef(This) ) 

#define IDebugBeforeSymbolSearchEvent2_Release(This)	\
    ( (This)->lpVtbl -> Release(This) ) 


#define IDebugBeforeSymbolSearchEvent2_GetModuleName(This,pbstrModuleName)	\
    ( (This)->lpVtbl -> GetModuleName(This,pbstrModuleName) ) 

#endif /* COBJMACROS */


#endif 	/* C style interface */




#endif 	/* __IDebugBeforeSymbolSearchEvent2_INTERFACE_DEFINED__ */


#ifndef __IDebugPropertyCreateEvent2_INTERFACE_DEFINED__
#define __IDebugPropertyCreateEvent2_INTERFACE_DEFINED__

/* interface IDebugPropertyCreateEvent2 */
/* [unique][uuid][object] */ 


EXTERN_C const IID IID_IDebugPropertyCreateEvent2;

#if defined(__cplusplus) && !defined(CINTERFACE)
    
    MIDL_INTERFACE("ded6d613-a3db-4e35-bb5b-a92391133f03")
    IDebugPropertyCreateEvent2 : public IUnknown
    {
    public:
        virtual HRESULT STDMETHODCALLTYPE GetDebugProperty( 
            /* [out] */ __RPC__deref_out_opt IDebugProperty2 **ppProperty) = 0;
        
    };
    
#else 	/* C style interface */

    typedef struct IDebugPropertyCreateEvent2Vtbl
    {
        BEGIN_INTERFACE
        
        HRESULT ( STDMETHODCALLTYPE *QueryInterface )( 
            IDebugPropertyCreateEvent2 * This,
            /* [in] */ __RPC__in REFIID riid,
            /* [iid_is][out] */ 
            __RPC__deref_out  void **ppvObject);
        
        ULONG ( STDMETHODCALLTYPE *AddRef )( 
            IDebugPropertyCreateEvent2 * This);
        
        ULONG ( STDMETHODCALLTYPE *Release )( 
            IDebugPropertyCreateEvent2 * This);
        
        HRESULT ( STDMETHODCALLTYPE *GetDebugProperty )( 
            IDebugPropertyCreateEvent2 * This,
            /* [out] */ __RPC__deref_out_opt IDebugProperty2 **ppProperty);
        
        END_INTERFACE
    } IDebugPropertyCreateEvent2Vtbl;

    interface IDebugPropertyCreateEvent2
    {
        CONST_VTBL struct IDebugPropertyCreateEvent2Vtbl *lpVtbl;
    };

    

#ifdef COBJMACROS


#define IDebugPropertyCreateEvent2_QueryInterface(This,riid,ppvObject)	\
    ( (This)->lpVtbl -> QueryInterface(This,riid,ppvObject) ) 

#define IDebugPropertyCreateEvent2_AddRef(This)	\
    ( (This)->lpVtbl -> AddRef(This) ) 

#define IDebugPropertyCreateEvent2_Release(This)	\
    ( (This)->lpVtbl -> Release(This) ) 


#define IDebugPropertyCreateEvent2_GetDebugProperty(This,ppProperty)	\
    ( (This)->lpVtbl -> GetDebugProperty(This,ppProperty) ) 

#endif /* COBJMACROS */


#endif 	/* C style interface */




#endif 	/* __IDebugPropertyCreateEvent2_INTERFACE_DEFINED__ */


#ifndef __IDebugPropertyDestroyEvent2_INTERFACE_DEFINED__
#define __IDebugPropertyDestroyEvent2_INTERFACE_DEFINED__

/* interface IDebugPropertyDestroyEvent2 */
/* [unique][uuid][object] */ 


EXTERN_C const IID IID_IDebugPropertyDestroyEvent2;

#if defined(__cplusplus) && !defined(CINTERFACE)
    
    MIDL_INTERFACE("f3765f18-f395-4b8c-8e95-dcb3fe8e7ec8")
    IDebugPropertyDestroyEvent2 : public IUnknown
    {
    public:
        virtual HRESULT STDMETHODCALLTYPE GetDebugProperty( 
            /* [out] */ __RPC__deref_out_opt IDebugProperty2 **ppProperty) = 0;
        
    };
    
#else 	/* C style interface */

    typedef struct IDebugPropertyDestroyEvent2Vtbl
    {
        BEGIN_INTERFACE
        
        HRESULT ( STDMETHODCALLTYPE *QueryInterface )( 
            IDebugPropertyDestroyEvent2 * This,
            /* [in] */ __RPC__in REFIID riid,
            /* [iid_is][out] */ 
            __RPC__deref_out  void **ppvObject);
        
        ULONG ( STDMETHODCALLTYPE *AddRef )( 
            IDebugPropertyDestroyEvent2 * This);
        
        ULONG ( STDMETHODCALLTYPE *Release )( 
            IDebugPropertyDestroyEvent2 * This);
        
        HRESULT ( STDMETHODCALLTYPE *GetDebugProperty )( 
            IDebugPropertyDestroyEvent2 * This,
            /* [out] */ __RPC__deref_out_opt IDebugProperty2 **ppProperty);
        
        END_INTERFACE
    } IDebugPropertyDestroyEvent2Vtbl;

    interface IDebugPropertyDestroyEvent2
    {
        CONST_VTBL struct IDebugPropertyDestroyEvent2Vtbl *lpVtbl;
    };

    

#ifdef COBJMACROS


#define IDebugPropertyDestroyEvent2_QueryInterface(This,riid,ppvObject)	\
    ( (This)->lpVtbl -> QueryInterface(This,riid,ppvObject) ) 

#define IDebugPropertyDestroyEvent2_AddRef(This)	\
    ( (This)->lpVtbl -> AddRef(This) ) 

#define IDebugPropertyDestroyEvent2_Release(This)	\
    ( (This)->lpVtbl -> Release(This) ) 


#define IDebugPropertyDestroyEvent2_GetDebugProperty(This,ppProperty)	\
    ( (This)->lpVtbl -> GetDebugProperty(This,ppProperty) ) 

#endif /* COBJMACROS */


#endif 	/* C style interface */




#endif 	/* __IDebugPropertyDestroyEvent2_INTERFACE_DEFINED__ */


#ifndef __IDebugBreakpointBoundEvent2_INTERFACE_DEFINED__
#define __IDebugBreakpointBoundEvent2_INTERFACE_DEFINED__

/* interface IDebugBreakpointBoundEvent2 */
/* [unique][uuid][object] */ 


EXTERN_C const IID IID_IDebugBreakpointBoundEvent2;

#if defined(__cplusplus) && !defined(CINTERFACE)
    
    MIDL_INTERFACE("1dddb704-cf99-4b8a-b746-dabb01dd13a0")
    IDebugBreakpointBoundEvent2 : public IUnknown
    {
    public:
        virtual HRESULT STDMETHODCALLTYPE GetPendingBreakpoint( 
            /* [out] */ __RPC__deref_out_opt IDebugPendingBreakpoint2 **ppPendingBP) = 0;
        
        virtual HRESULT STDMETHODCALLTYPE EnumBoundBreakpoints( 
            /* [out] */ __RPC__deref_out_opt IEnumDebugBoundBreakpoints2 **ppEnum) = 0;
        
    };
    
#else 	/* C style interface */

    typedef struct IDebugBreakpointBoundEvent2Vtbl
    {
        BEGIN_INTERFACE
        
        HRESULT ( STDMETHODCALLTYPE *QueryInterface )( 
            IDebugBreakpointBoundEvent2 * This,
            /* [in] */ __RPC__in REFIID riid,
            /* [iid_is][out] */ 
            __RPC__deref_out  void **ppvObject);
        
        ULONG ( STDMETHODCALLTYPE *AddRef )( 
            IDebugBreakpointBoundEvent2 * This);
        
        ULONG ( STDMETHODCALLTYPE *Release )( 
            IDebugBreakpointBoundEvent2 * This);
        
        HRESULT ( STDMETHODCALLTYPE *GetPendingBreakpoint )( 
            IDebugBreakpointBoundEvent2 * This,
            /* [out] */ __RPC__deref_out_opt IDebugPendingBreakpoint2 **ppPendingBP);
        
        HRESULT ( STDMETHODCALLTYPE *EnumBoundBreakpoints )( 
            IDebugBreakpointBoundEvent2 * This,
            /* [out] */ __RPC__deref_out_opt IEnumDebugBoundBreakpoints2 **ppEnum);
        
        END_INTERFACE
    } IDebugBreakpointBoundEvent2Vtbl;

    interface IDebugBreakpointBoundEvent2
    {
        CONST_VTBL struct IDebugBreakpointBoundEvent2Vtbl *lpVtbl;
    };

    

#ifdef COBJMACROS


#define IDebugBreakpointBoundEvent2_QueryInterface(This,riid,ppvObject)	\
    ( (This)->lpVtbl -> QueryInterface(This,riid,ppvObject) ) 

#define IDebugBreakpointBoundEvent2_AddRef(This)	\
    ( (This)->lpVtbl -> AddRef(This) ) 

#define IDebugBreakpointBoundEvent2_Release(This)	\
    ( (This)->lpVtbl -> Release(This) ) 


#define IDebugBreakpointBoundEvent2_GetPendingBreakpoint(This,ppPendingBP)	\
    ( (This)->lpVtbl -> GetPendingBreakpoint(This,ppPendingBP) ) 

#define IDebugBreakpointBoundEvent2_EnumBoundBreakpoints(This,ppEnum)	\
    ( (This)->lpVtbl -> EnumBoundBreakpoints(This,ppEnum) ) 

#endif /* COBJMACROS */


#endif 	/* C style interface */




#endif 	/* __IDebugBreakpointBoundEvent2_INTERFACE_DEFINED__ */


#ifndef __IDebugBreakpointUnboundEvent2_INTERFACE_DEFINED__
#define __IDebugBreakpointUnboundEvent2_INTERFACE_DEFINED__

/* interface IDebugBreakpointUnboundEvent2 */
/* [unique][uuid][object] */ 


enum enum_BP_UNBOUND_REASON
    {	BPUR_UNKNOWN	= 0x1,
	BPUR_CODE_UNLOADED	= 0x2,
	BPUR_BREAKPOINT_REBIND	= 0x3,
	BPUR_BREAKPOINT_ERROR	= 0x4
    } ;
typedef DWORD BP_UNBOUND_REASON;


EXTERN_C const IID IID_IDebugBreakpointUnboundEvent2;

#if defined(__cplusplus) && !defined(CINTERFACE)
    
    MIDL_INTERFACE("78d1db4f-c557-4dc5-a2dd-5369d21b1c8c")
    IDebugBreakpointUnboundEvent2 : public IUnknown
    {
    public:
        virtual HRESULT STDMETHODCALLTYPE GetBreakpoint( 
            /* [out] */ __RPC__deref_out_opt IDebugBoundBreakpoint2 **ppBP) = 0;
        
        virtual HRESULT STDMETHODCALLTYPE GetReason( 
            /* [out] */ __RPC__out BP_UNBOUND_REASON *pdwUnboundReason) = 0;
        
    };
    
#else 	/* C style interface */

    typedef struct IDebugBreakpointUnboundEvent2Vtbl
    {
        BEGIN_INTERFACE
        
        HRESULT ( STDMETHODCALLTYPE *QueryInterface )( 
            IDebugBreakpointUnboundEvent2 * This,
            /* [in] */ __RPC__in REFIID riid,
            /* [iid_is][out] */ 
            __RPC__deref_out  void **ppvObject);
        
        ULONG ( STDMETHODCALLTYPE *AddRef )( 
            IDebugBreakpointUnboundEvent2 * This);
        
        ULONG ( STDMETHODCALLTYPE *Release )( 
            IDebugBreakpointUnboundEvent2 * This);
        
        HRESULT ( STDMETHODCALLTYPE *GetBreakpoint )( 
            IDebugBreakpointUnboundEvent2 * This,
            /* [out] */ __RPC__deref_out_opt IDebugBoundBreakpoint2 **ppBP);
        
        HRESULT ( STDMETHODCALLTYPE *GetReason )( 
            IDebugBreakpointUnboundEvent2 * This,
            /* [out] */ __RPC__out BP_UNBOUND_REASON *pdwUnboundReason);
        
        END_INTERFACE
    } IDebugBreakpointUnboundEvent2Vtbl;

    interface IDebugBreakpointUnboundEvent2
    {
        CONST_VTBL struct IDebugBreakpointUnboundEvent2Vtbl *lpVtbl;
    };

    

#ifdef COBJMACROS


#define IDebugBreakpointUnboundEvent2_QueryInterface(This,riid,ppvObject)	\
    ( (This)->lpVtbl -> QueryInterface(This,riid,ppvObject) ) 

#define IDebugBreakpointUnboundEvent2_AddRef(This)	\
    ( (This)->lpVtbl -> AddRef(This) ) 

#define IDebugBreakpointUnboundEvent2_Release(This)	\
    ( (This)->lpVtbl -> Release(This) ) 


#define IDebugBreakpointUnboundEvent2_GetBreakpoint(This,ppBP)	\
    ( (This)->lpVtbl -> GetBreakpoint(This,ppBP) ) 

#define IDebugBreakpointUnboundEvent2_GetReason(This,pdwUnboundReason)	\
    ( (This)->lpVtbl -> GetReason(This,pdwUnboundReason) ) 

#endif /* COBJMACROS */


#endif 	/* C style interface */




#endif 	/* __IDebugBreakpointUnboundEvent2_INTERFACE_DEFINED__ */


#ifndef __IDebugBreakpointErrorEvent2_INTERFACE_DEFINED__
#define __IDebugBreakpointErrorEvent2_INTERFACE_DEFINED__

/* interface IDebugBreakpointErrorEvent2 */
/* [unique][uuid][object] */ 


EXTERN_C const IID IID_IDebugBreakpointErrorEvent2;

#if defined(__cplusplus) && !defined(CINTERFACE)
    
    MIDL_INTERFACE("abb0ca42-f82b-4622-84e4-6903ae90f210")
    IDebugBreakpointErrorEvent2 : public IUnknown
    {
    public:
        virtual HRESULT STDMETHODCALLTYPE GetErrorBreakpoint( 
            /* [out] */ __RPC__deref_out_opt IDebugErrorBreakpoint2 **ppErrorBP) = 0;
        
    };
    
#else 	/* C style interface */

    typedef struct IDebugBreakpointErrorEvent2Vtbl
    {
        BEGIN_INTERFACE
        
        HRESULT ( STDMETHODCALLTYPE *QueryInterface )( 
            IDebugBreakpointErrorEvent2 * This,
            /* [in] */ __RPC__in REFIID riid,
            /* [iid_is][out] */ 
            __RPC__deref_out  void **ppvObject);
        
        ULONG ( STDMETHODCALLTYPE *AddRef )( 
            IDebugBreakpointErrorEvent2 * This);
        
        ULONG ( STDMETHODCALLTYPE *Release )( 
            IDebugBreakpointErrorEvent2 * This);
        
        HRESULT ( STDMETHODCALLTYPE *GetErrorBreakpoint )( 
            IDebugBreakpointErrorEvent2 * This,
            /* [out] */ __RPC__deref_out_opt IDebugErrorBreakpoint2 **ppErrorBP);
        
        END_INTERFACE
    } IDebugBreakpointErrorEvent2Vtbl;

    interface IDebugBreakpointErrorEvent2
    {
        CONST_VTBL struct IDebugBreakpointErrorEvent2Vtbl *lpVtbl;
    };

    

#ifdef COBJMACROS


#define IDebugBreakpointErrorEvent2_QueryInterface(This,riid,ppvObject)	\
    ( (This)->lpVtbl -> QueryInterface(This,riid,ppvObject) ) 

#define IDebugBreakpointErrorEvent2_AddRef(This)	\
    ( (This)->lpVtbl -> AddRef(This) ) 

#define IDebugBreakpointErrorEvent2_Release(This)	\
    ( (This)->lpVtbl -> Release(This) ) 


#define IDebugBreakpointErrorEvent2_GetErrorBreakpoint(This,ppErrorBP)	\
    ( (This)->lpVtbl -> GetErrorBreakpoint(This,ppErrorBP) ) 

#endif /* COBJMACROS */


#endif 	/* C style interface */




#endif 	/* __IDebugBreakpointErrorEvent2_INTERFACE_DEFINED__ */


#ifndef __IDebugExpressionEvaluationCompleteEvent2_INTERFACE_DEFINED__
#define __IDebugExpressionEvaluationCompleteEvent2_INTERFACE_DEFINED__

/* interface IDebugExpressionEvaluationCompleteEvent2 */
/* [unique][uuid][object] */ 


EXTERN_C const IID IID_IDebugExpressionEvaluationCompleteEvent2;

#if defined(__cplusplus) && !defined(CINTERFACE)
    
    MIDL_INTERFACE("c0e13a85-238a-4800-8315-d947c960a843")
    IDebugExpressionEvaluationCompleteEvent2 : public IUnknown
    {
    public:
        virtual HRESULT STDMETHODCALLTYPE GetExpression( 
            /* [out] */ __RPC__deref_out_opt IDebugExpression2 **ppExpr) = 0;
        
        virtual HRESULT STDMETHODCALLTYPE GetResult( 
            /* [out] */ __RPC__deref_out_opt IDebugProperty2 **ppResult) = 0;
        
    };
    
#else 	/* C style interface */

    typedef struct IDebugExpressionEvaluationCompleteEvent2Vtbl
    {
        BEGIN_INTERFACE
        
        HRESULT ( STDMETHODCALLTYPE *QueryInterface )( 
            IDebugExpressionEvaluationCompleteEvent2 * This,
            /* [in] */ __RPC__in REFIID riid,
            /* [iid_is][out] */ 
            __RPC__deref_out  void **ppvObject);
        
        ULONG ( STDMETHODCALLTYPE *AddRef )( 
            IDebugExpressionEvaluationCompleteEvent2 * This);
        
        ULONG ( STDMETHODCALLTYPE *Release )( 
            IDebugExpressionEvaluationCompleteEvent2 * This);
        
        HRESULT ( STDMETHODCALLTYPE *GetExpression )( 
            IDebugExpressionEvaluationCompleteEvent2 * This,
            /* [out] */ __RPC__deref_out_opt IDebugExpression2 **ppExpr);
        
        HRESULT ( STDMETHODCALLTYPE *GetResult )( 
            IDebugExpressionEvaluationCompleteEvent2 * This,
            /* [out] */ __RPC__deref_out_opt IDebugProperty2 **ppResult);
        
        END_INTERFACE
    } IDebugExpressionEvaluationCompleteEvent2Vtbl;

    interface IDebugExpressionEvaluationCompleteEvent2
    {
        CONST_VTBL struct IDebugExpressionEvaluationCompleteEvent2Vtbl *lpVtbl;
    };

    

#ifdef COBJMACROS


#define IDebugExpressionEvaluationCompleteEvent2_QueryInterface(This,riid,ppvObject)	\
    ( (This)->lpVtbl -> QueryInterface(This,riid,ppvObject) ) 

#define IDebugExpressionEvaluationCompleteEvent2_AddRef(This)	\
    ( (This)->lpVtbl -> AddRef(This) ) 

#define IDebugExpressionEvaluationCompleteEvent2_Release(This)	\
    ( (This)->lpVtbl -> Release(This) ) 


#define IDebugExpressionEvaluationCompleteEvent2_GetExpression(This,ppExpr)	\
    ( (This)->lpVtbl -> GetExpression(This,ppExpr) ) 

#define IDebugExpressionEvaluationCompleteEvent2_GetResult(This,ppResult)	\
    ( (This)->lpVtbl -> GetResult(This,ppResult) ) 

#endif /* COBJMACROS */


#endif 	/* C style interface */




#endif 	/* __IDebugExpressionEvaluationCompleteEvent2_INTERFACE_DEFINED__ */


#ifndef __IDebugReturnValueEvent2_INTERFACE_DEFINED__
#define __IDebugReturnValueEvent2_INTERFACE_DEFINED__

/* interface IDebugReturnValueEvent2 */
/* [unique][uuid][object] */ 


EXTERN_C const IID IID_IDebugReturnValueEvent2;

#if defined(__cplusplus) && !defined(CINTERFACE)
    
    MIDL_INTERFACE("0da4d4cc-2d0b-410f-8d5d-b6b73a5d35d8")
    IDebugReturnValueEvent2 : public IUnknown
    {
    public:
        virtual HRESULT STDMETHODCALLTYPE GetReturnValue( 
            /* [out] */ __RPC__deref_out_opt IDebugProperty2 **ppReturnValue) = 0;
        
    };
    
#else 	/* C style interface */

    typedef struct IDebugReturnValueEvent2Vtbl
    {
        BEGIN_INTERFACE
        
        HRESULT ( STDMETHODCALLTYPE *QueryInterface )( 
            IDebugReturnValueEvent2 * This,
            /* [in] */ __RPC__in REFIID riid,
            /* [iid_is][out] */ 
            __RPC__deref_out  void **ppvObject);
        
        ULONG ( STDMETHODCALLTYPE *AddRef )( 
            IDebugReturnValueEvent2 * This);
        
        ULONG ( STDMETHODCALLTYPE *Release )( 
            IDebugReturnValueEvent2 * This);
        
        HRESULT ( STDMETHODCALLTYPE *GetReturnValue )( 
            IDebugReturnValueEvent2 * This,
            /* [out] */ __RPC__deref_out_opt IDebugProperty2 **ppReturnValue);
        
        END_INTERFACE
    } IDebugReturnValueEvent2Vtbl;

    interface IDebugReturnValueEvent2
    {
        CONST_VTBL struct IDebugReturnValueEvent2Vtbl *lpVtbl;
    };

    

#ifdef COBJMACROS


#define IDebugReturnValueEvent2_QueryInterface(This,riid,ppvObject)	\
    ( (This)->lpVtbl -> QueryInterface(This,riid,ppvObject) ) 

#define IDebugReturnValueEvent2_AddRef(This)	\
    ( (This)->lpVtbl -> AddRef(This) ) 

#define IDebugReturnValueEvent2_Release(This)	\
    ( (This)->lpVtbl -> Release(This) ) 


#define IDebugReturnValueEvent2_GetReturnValue(This,ppReturnValue)	\
    ( (This)->lpVtbl -> GetReturnValue(This,ppReturnValue) ) 

#endif /* COBJMACROS */


#endif 	/* C style interface */




#endif 	/* __IDebugReturnValueEvent2_INTERFACE_DEFINED__ */


#ifndef __IDebugNoSymbolsEvent2_INTERFACE_DEFINED__
#define __IDebugNoSymbolsEvent2_INTERFACE_DEFINED__

/* interface IDebugNoSymbolsEvent2 */
/* [unique][uuid][object] */ 


EXTERN_C const IID IID_IDebugNoSymbolsEvent2;

#if defined(__cplusplus) && !defined(CINTERFACE)
    
    MIDL_INTERFACE("3ad4fb48-647e-4b03-9c1e-52754e80c880")
    IDebugNoSymbolsEvent2 : public IUnknown
    {
    public:
    };
    
#else 	/* C style interface */

    typedef struct IDebugNoSymbolsEvent2Vtbl
    {
        BEGIN_INTERFACE
        
        HRESULT ( STDMETHODCALLTYPE *QueryInterface )( 
            IDebugNoSymbolsEvent2 * This,
            /* [in] */ __RPC__in REFIID riid,
            /* [iid_is][out] */ 
            __RPC__deref_out  void **ppvObject);
        
        ULONG ( STDMETHODCALLTYPE *AddRef )( 
            IDebugNoSymbolsEvent2 * This);
        
        ULONG ( STDMETHODCALLTYPE *Release )( 
            IDebugNoSymbolsEvent2 * This);
        
        END_INTERFACE
    } IDebugNoSymbolsEvent2Vtbl;

    interface IDebugNoSymbolsEvent2
    {
        CONST_VTBL struct IDebugNoSymbolsEvent2Vtbl *lpVtbl;
    };

    

#ifdef COBJMACROS


#define IDebugNoSymbolsEvent2_QueryInterface(This,riid,ppvObject)	\
    ( (This)->lpVtbl -> QueryInterface(This,riid,ppvObject) ) 

#define IDebugNoSymbolsEvent2_AddRef(This)	\
    ( (This)->lpVtbl -> AddRef(This) ) 

#define IDebugNoSymbolsEvent2_Release(This)	\
    ( (This)->lpVtbl -> Release(This) ) 


#endif /* COBJMACROS */


#endif 	/* C style interface */




#endif 	/* __IDebugNoSymbolsEvent2_INTERFACE_DEFINED__ */


#ifndef __IDebugProgramNameChangedEvent2_INTERFACE_DEFINED__
#define __IDebugProgramNameChangedEvent2_INTERFACE_DEFINED__

/* interface IDebugProgramNameChangedEvent2 */
/* [unique][uuid][object] */ 


EXTERN_C const IID IID_IDebugProgramNameChangedEvent2;

#if defined(__cplusplus) && !defined(CINTERFACE)
    
    MIDL_INTERFACE("e05c2dfd-59d5-46d3-a71c-5d07665d85af")
    IDebugProgramNameChangedEvent2 : public IUnknown
    {
    public:
    };
    
#else 	/* C style interface */

    typedef struct IDebugProgramNameChangedEvent2Vtbl
    {
        BEGIN_INTERFACE
        
        HRESULT ( STDMETHODCALLTYPE *QueryInterface )( 
            IDebugProgramNameChangedEvent2 * This,
            /* [in] */ __RPC__in REFIID riid,
            /* [iid_is][out] */ 
            __RPC__deref_out  void **ppvObject);
        
        ULONG ( STDMETHODCALLTYPE *AddRef )( 
            IDebugProgramNameChangedEvent2 * This);
        
        ULONG ( STDMETHODCALLTYPE *Release )( 
            IDebugProgramNameChangedEvent2 * This);
        
        END_INTERFACE
    } IDebugProgramNameChangedEvent2Vtbl;

    interface IDebugProgramNameChangedEvent2
    {
        CONST_VTBL struct IDebugProgramNameChangedEvent2Vtbl *lpVtbl;
    };

    

#ifdef COBJMACROS


#define IDebugProgramNameChangedEvent2_QueryInterface(This,riid,ppvObject)	\
    ( (This)->lpVtbl -> QueryInterface(This,riid,ppvObject) ) 

#define IDebugProgramNameChangedEvent2_AddRef(This)	\
    ( (This)->lpVtbl -> AddRef(This) ) 

#define IDebugProgramNameChangedEvent2_Release(This)	\
    ( (This)->lpVtbl -> Release(This) ) 


#endif /* COBJMACROS */


#endif 	/* C style interface */




#endif 	/* __IDebugProgramNameChangedEvent2_INTERFACE_DEFINED__ */


#ifndef __IDebugThreadNameChangedEvent2_INTERFACE_DEFINED__
#define __IDebugThreadNameChangedEvent2_INTERFACE_DEFINED__

/* interface IDebugThreadNameChangedEvent2 */
/* [unique][uuid][object] */ 


EXTERN_C const IID IID_IDebugThreadNameChangedEvent2;

#if defined(__cplusplus) && !defined(CINTERFACE)
    
    MIDL_INTERFACE("1ef4ef78-2c44-4b7a-8473-8f4357611729")
    IDebugThreadNameChangedEvent2 : public IUnknown
    {
    public:
    };
    
#else 	/* C style interface */

    typedef struct IDebugThreadNameChangedEvent2Vtbl
    {
        BEGIN_INTERFACE
        
        HRESULT ( STDMETHODCALLTYPE *QueryInterface )( 
            IDebugThreadNameChangedEvent2 * This,
            /* [in] */ __RPC__in REFIID riid,
            /* [iid_is][out] */ 
            __RPC__deref_out  void **ppvObject);
        
        ULONG ( STDMETHODCALLTYPE *AddRef )( 
            IDebugThreadNameChangedEvent2 * This);
        
        ULONG ( STDMETHODCALLTYPE *Release )( 
            IDebugThreadNameChangedEvent2 * This);
        
        END_INTERFACE
    } IDebugThreadNameChangedEvent2Vtbl;

    interface IDebugThreadNameChangedEvent2
    {
        CONST_VTBL struct IDebugThreadNameChangedEvent2Vtbl *lpVtbl;
    };

    

#ifdef COBJMACROS


#define IDebugThreadNameChangedEvent2_QueryInterface(This,riid,ppvObject)	\
    ( (This)->lpVtbl -> QueryInterface(This,riid,ppvObject) ) 

#define IDebugThreadNameChangedEvent2_AddRef(This)	\
    ( (This)->lpVtbl -> AddRef(This) ) 

#define IDebugThreadNameChangedEvent2_Release(This)	\
    ( (This)->lpVtbl -> Release(This) ) 


#endif /* COBJMACROS */


#endif 	/* C style interface */




#endif 	/* __IDebugThreadNameChangedEvent2_INTERFACE_DEFINED__ */


#ifndef __IDebugMessageEvent2_INTERFACE_DEFINED__
#define __IDebugMessageEvent2_INTERFACE_DEFINED__

/* interface IDebugMessageEvent2 */
/* [unique][uuid][object] */ 


enum enum_MESSAGETYPE
    {	MT_OUTPUTSTRING	= 0x1,
	MT_MESSAGEBOX	= 0x2,
	MT_TYPE_MASK	= 0xff,
	MT_REASON_EXCEPTION	= 0x100,
	MT_REASON_TRACEPOINT	= 0x200,
	MT_REASON_MASK	= 0xff00
    } ;
typedef DWORD MESSAGETYPE;


EXTERN_C const IID IID_IDebugMessageEvent2;

#if defined(__cplusplus) && !defined(CINTERFACE)
    
    MIDL_INTERFACE("3bdb28cf-dbd2-4d24-af03-01072b67eb9e")
    IDebugMessageEvent2 : public IUnknown
    {
    public:
        virtual HRESULT STDMETHODCALLTYPE GetMessage( 
            /* [out] */ __RPC__out MESSAGETYPE *pMessageType,
            /* [out] */ __RPC__deref_out_opt BSTR *pbstrMessage,
            /* [out] */ __RPC__out DWORD *pdwType,
            /* [out] */ __RPC__deref_out_opt BSTR *pbstrHelpFileName,
            /* [out] */ __RPC__out DWORD *pdwHelpId) = 0;
        
        virtual HRESULT STDMETHODCALLTYPE SetResponse( 
            /* [in] */ DWORD dwResponse) = 0;
        
    };
    
#else 	/* C style interface */

    typedef struct IDebugMessageEvent2Vtbl
    {
        BEGIN_INTERFACE
        
        HRESULT ( STDMETHODCALLTYPE *QueryInterface )( 
            IDebugMessageEvent2 * This,
            /* [in] */ __RPC__in REFIID riid,
            /* [iid_is][out] */ 
            __RPC__deref_out  void **ppvObject);
        
        ULONG ( STDMETHODCALLTYPE *AddRef )( 
            IDebugMessageEvent2 * This);
        
        ULONG ( STDMETHODCALLTYPE *Release )( 
            IDebugMessageEvent2 * This);
        
        HRESULT ( STDMETHODCALLTYPE *GetMessage )( 
            IDebugMessageEvent2 * This,
            /* [out] */ __RPC__out MESSAGETYPE *pMessageType,
            /* [out] */ __RPC__deref_out_opt BSTR *pbstrMessage,
            /* [out] */ __RPC__out DWORD *pdwType,
            /* [out] */ __RPC__deref_out_opt BSTR *pbstrHelpFileName,
            /* [out] */ __RPC__out DWORD *pdwHelpId);
        
        HRESULT ( STDMETHODCALLTYPE *SetResponse )( 
            IDebugMessageEvent2 * This,
            /* [in] */ DWORD dwResponse);
        
        END_INTERFACE
    } IDebugMessageEvent2Vtbl;

    interface IDebugMessageEvent2
    {
        CONST_VTBL struct IDebugMessageEvent2Vtbl *lpVtbl;
    };

    

#ifdef COBJMACROS


#define IDebugMessageEvent2_QueryInterface(This,riid,ppvObject)	\
    ( (This)->lpVtbl -> QueryInterface(This,riid,ppvObject) ) 

#define IDebugMessageEvent2_AddRef(This)	\
    ( (This)->lpVtbl -> AddRef(This) ) 

#define IDebugMessageEvent2_Release(This)	\
    ( (This)->lpVtbl -> Release(This) ) 


#define IDebugMessageEvent2_GetMessage(This,pMessageType,pbstrMessage,pdwType,pbstrHelpFileName,pdwHelpId)	\
    ( (This)->lpVtbl -> GetMessage(This,pMessageType,pbstrMessage,pdwType,pbstrHelpFileName,pdwHelpId) ) 

#define IDebugMessageEvent2_SetResponse(This,dwResponse)	\
    ( (This)->lpVtbl -> SetResponse(This,dwResponse) ) 

#endif /* COBJMACROS */


#endif 	/* C style interface */




#endif 	/* __IDebugMessageEvent2_INTERFACE_DEFINED__ */


#ifndef __IDebugErrorEvent2_INTERFACE_DEFINED__
#define __IDebugErrorEvent2_INTERFACE_DEFINED__

/* interface IDebugErrorEvent2 */
/* [unique][uuid][object] */ 


EXTERN_C const IID IID_IDebugErrorEvent2;

#if defined(__cplusplus) && !defined(CINTERFACE)
    
    MIDL_INTERFACE("fdb7a36c-8c53-41da-a337-8bd86b14d5cb")
    IDebugErrorEvent2 : public IUnknown
    {
    public:
        virtual HRESULT STDMETHODCALLTYPE GetErrorMessage( 
            /* [out] */ __RPC__out MESSAGETYPE *pMessageType,
            /* [out] */ __RPC__deref_out_opt BSTR *pbstrErrorFormat,
            /* [out] */ __RPC__out HRESULT *phrErrorReason,
            /* [out] */ __RPC__out DWORD *pdwType,
            /* [out] */ __RPC__deref_out_opt BSTR *pbstrHelpFileName,
            /* [out] */ __RPC__out DWORD *pdwHelpId) = 0;
        
    };
    
#else 	/* C style interface */

    typedef struct IDebugErrorEvent2Vtbl
    {
        BEGIN_INTERFACE
        
        HRESULT ( STDMETHODCALLTYPE *QueryInterface )( 
            IDebugErrorEvent2 * This,
            /* [in] */ __RPC__in REFIID riid,
            /* [iid_is][out] */ 
            __RPC__deref_out  void **ppvObject);
        
        ULONG ( STDMETHODCALLTYPE *AddRef )( 
            IDebugErrorEvent2 * This);
        
        ULONG ( STDMETHODCALLTYPE *Release )( 
            IDebugErrorEvent2 * This);
        
        HRESULT ( STDMETHODCALLTYPE *GetErrorMessage )( 
            IDebugErrorEvent2 * This,
            /* [out] */ __RPC__out MESSAGETYPE *pMessageType,
            /* [out] */ __RPC__deref_out_opt BSTR *pbstrErrorFormat,
            /* [out] */ __RPC__out HRESULT *phrErrorReason,
            /* [out] */ __RPC__out DWORD *pdwType,
            /* [out] */ __RPC__deref_out_opt BSTR *pbstrHelpFileName,
            /* [out] */ __RPC__out DWORD *pdwHelpId);
        
        END_INTERFACE
    } IDebugErrorEvent2Vtbl;

    interface IDebugErrorEvent2
    {
        CONST_VTBL struct IDebugErrorEvent2Vtbl *lpVtbl;
    };

    

#ifdef COBJMACROS


#define IDebugErrorEvent2_QueryInterface(This,riid,ppvObject)	\
    ( (This)->lpVtbl -> QueryInterface(This,riid,ppvObject) ) 

#define IDebugErrorEvent2_AddRef(This)	\
    ( (This)->lpVtbl -> AddRef(This) ) 

#define IDebugErrorEvent2_Release(This)	\
    ( (This)->lpVtbl -> Release(This) ) 


#define IDebugErrorEvent2_GetErrorMessage(This,pMessageType,pbstrErrorFormat,phrErrorReason,pdwType,pbstrHelpFileName,pdwHelpId)	\
    ( (This)->lpVtbl -> GetErrorMessage(This,pMessageType,pbstrErrorFormat,phrErrorReason,pdwType,pbstrHelpFileName,pdwHelpId) ) 

#endif /* COBJMACROS */


#endif 	/* C style interface */




#endif 	/* __IDebugErrorEvent2_INTERFACE_DEFINED__ */


#ifndef __IDebugActivateDocumentEvent2_INTERFACE_DEFINED__
#define __IDebugActivateDocumentEvent2_INTERFACE_DEFINED__

/* interface IDebugActivateDocumentEvent2 */
/* [unique][uuid][object] */ 


EXTERN_C const IID IID_IDebugActivateDocumentEvent2;

#if defined(__cplusplus) && !defined(CINTERFACE)
    
    MIDL_INTERFACE("58f36c3d-7d07-4eba-a041-62f63e188037")
    IDebugActivateDocumentEvent2 : public IUnknown
    {
    public:
        virtual HRESULT STDMETHODCALLTYPE GetDocument( 
            /* [out] */ __RPC__deref_out_opt IDebugDocument2 **ppDoc) = 0;
        
        virtual HRESULT STDMETHODCALLTYPE GetDocumentContext( 
            /* [out] */ __RPC__deref_out_opt IDebugDocumentContext2 **ppDocContext) = 0;
        
    };
    
#else 	/* C style interface */

    typedef struct IDebugActivateDocumentEvent2Vtbl
    {
        BEGIN_INTERFACE
        
        HRESULT ( STDMETHODCALLTYPE *QueryInterface )( 
            IDebugActivateDocumentEvent2 * This,
            /* [in] */ __RPC__in REFIID riid,
            /* [iid_is][out] */ 
            __RPC__deref_out  void **ppvObject);
        
        ULONG ( STDMETHODCALLTYPE *AddRef )( 
            IDebugActivateDocumentEvent2 * This);
        
        ULONG ( STDMETHODCALLTYPE *Release )( 
            IDebugActivateDocumentEvent2 * This);
        
        HRESULT ( STDMETHODCALLTYPE *GetDocument )( 
            IDebugActivateDocumentEvent2 * This,
            /* [out] */ __RPC__deref_out_opt IDebugDocument2 **ppDoc);
        
        HRESULT ( STDMETHODCALLTYPE *GetDocumentContext )( 
            IDebugActivateDocumentEvent2 * This,
            /* [out] */ __RPC__deref_out_opt IDebugDocumentContext2 **ppDocContext);
        
        END_INTERFACE
    } IDebugActivateDocumentEvent2Vtbl;

    interface IDebugActivateDocumentEvent2
    {
        CONST_VTBL struct IDebugActivateDocumentEvent2Vtbl *lpVtbl;
    };

    

#ifdef COBJMACROS


#define IDebugActivateDocumentEvent2_QueryInterface(This,riid,ppvObject)	\
    ( (This)->lpVtbl -> QueryInterface(This,riid,ppvObject) ) 

#define IDebugActivateDocumentEvent2_AddRef(This)	\
    ( (This)->lpVtbl -> AddRef(This) ) 

#define IDebugActivateDocumentEvent2_Release(This)	\
    ( (This)->lpVtbl -> Release(This) ) 


#define IDebugActivateDocumentEvent2_GetDocument(This,ppDoc)	\
    ( (This)->lpVtbl -> GetDocument(This,ppDoc) ) 

#define IDebugActivateDocumentEvent2_GetDocumentContext(This,ppDocContext)	\
    ( (This)->lpVtbl -> GetDocumentContext(This,ppDocContext) ) 

#endif /* COBJMACROS */


#endif 	/* C style interface */




#endif 	/* __IDebugActivateDocumentEvent2_INTERFACE_DEFINED__ */


#ifndef __IDebugInterceptExceptionCompleteEvent2_INTERFACE_DEFINED__
#define __IDebugInterceptExceptionCompleteEvent2_INTERFACE_DEFINED__

/* interface IDebugInterceptExceptionCompleteEvent2 */
/* [unique][uuid][object] */ 


EXTERN_C const IID IID_IDebugInterceptExceptionCompleteEvent2;

#if defined(__cplusplus) && !defined(CINTERFACE)
    
    MIDL_INTERFACE("44FCEACA-7F56-4d2c-A637-60052B1B9CBE")
    IDebugInterceptExceptionCompleteEvent2 : public IUnknown
    {
    public:
        virtual HRESULT STDMETHODCALLTYPE GetInterceptCookie( 
            /* [out] */ __RPC__out UINT64 *pqwCookie) = 0;
        
    };
    
#else 	/* C style interface */

    typedef struct IDebugInterceptExceptionCompleteEvent2Vtbl
    {
        BEGIN_INTERFACE
        
        HRESULT ( STDMETHODCALLTYPE *QueryInterface )( 
            IDebugInterceptExceptionCompleteEvent2 * This,
            /* [in] */ __RPC__in REFIID riid,
            /* [iid_is][out] */ 
            __RPC__deref_out  void **ppvObject);
        
        ULONG ( STDMETHODCALLTYPE *AddRef )( 
            IDebugInterceptExceptionCompleteEvent2 * This);
        
        ULONG ( STDMETHODCALLTYPE *Release )( 
            IDebugInterceptExceptionCompleteEvent2 * This);
        
        HRESULT ( STDMETHODCALLTYPE *GetInterceptCookie )( 
            IDebugInterceptExceptionCompleteEvent2 * This,
            /* [out] */ __RPC__out UINT64 *pqwCookie);
        
        END_INTERFACE
    } IDebugInterceptExceptionCompleteEvent2Vtbl;

    interface IDebugInterceptExceptionCompleteEvent2
    {
        CONST_VTBL struct IDebugInterceptExceptionCompleteEvent2Vtbl *lpVtbl;
    };

    

#ifdef COBJMACROS


#define IDebugInterceptExceptionCompleteEvent2_QueryInterface(This,riid,ppvObject)	\
    ( (This)->lpVtbl -> QueryInterface(This,riid,ppvObject) ) 

#define IDebugInterceptExceptionCompleteEvent2_AddRef(This)	\
    ( (This)->lpVtbl -> AddRef(This) ) 

#define IDebugInterceptExceptionCompleteEvent2_Release(This)	\
    ( (This)->lpVtbl -> Release(This) ) 


#define IDebugInterceptExceptionCompleteEvent2_GetInterceptCookie(This,pqwCookie)	\
    ( (This)->lpVtbl -> GetInterceptCookie(This,pqwCookie) ) 

#endif /* COBJMACROS */


#endif 	/* C style interface */




#endif 	/* __IDebugInterceptExceptionCompleteEvent2_INTERFACE_DEFINED__ */


#ifndef __IDebugAttachCompleteEvent2_INTERFACE_DEFINED__
#define __IDebugAttachCompleteEvent2_INTERFACE_DEFINED__

/* interface IDebugAttachCompleteEvent2 */
/* [unique][uuid][object] */ 


EXTERN_C const IID IID_IDebugAttachCompleteEvent2;

#if defined(__cplusplus) && !defined(CINTERFACE)
    
    MIDL_INTERFACE("fe1fd9ea-6413-4183-a67d-588870014e97")
    IDebugAttachCompleteEvent2 : public IUnknown
    {
    public:
    };
    
#else 	/* C style interface */

    typedef struct IDebugAttachCompleteEvent2Vtbl
    {
        BEGIN_INTERFACE
        
        HRESULT ( STDMETHODCALLTYPE *QueryInterface )( 
            IDebugAttachCompleteEvent2 * This,
            /* [in] */ __RPC__in REFIID riid,
            /* [iid_is][out] */ 
            __RPC__deref_out  void **ppvObject);
        
        ULONG ( STDMETHODCALLTYPE *AddRef )( 
            IDebugAttachCompleteEvent2 * This);
        
        ULONG ( STDMETHODCALLTYPE *Release )( 
            IDebugAttachCompleteEvent2 * This);
        
        END_INTERFACE
    } IDebugAttachCompleteEvent2Vtbl;

    interface IDebugAttachCompleteEvent2
    {
        CONST_VTBL struct IDebugAttachCompleteEvent2Vtbl *lpVtbl;
    };

    

#ifdef COBJMACROS


#define IDebugAttachCompleteEvent2_QueryInterface(This,riid,ppvObject)	\
    ( (This)->lpVtbl -> QueryInterface(This,riid,ppvObject) ) 

#define IDebugAttachCompleteEvent2_AddRef(This)	\
    ( (This)->lpVtbl -> AddRef(This) ) 

#define IDebugAttachCompleteEvent2_Release(This)	\
    ( (This)->lpVtbl -> Release(This) ) 


#endif /* COBJMACROS */


#endif 	/* C style interface */




#endif 	/* __IDebugAttachCompleteEvent2_INTERFACE_DEFINED__ */


#ifndef __IDebugFuncEvalAbortedEvent2_INTERFACE_DEFINED__
#define __IDebugFuncEvalAbortedEvent2_INTERFACE_DEFINED__

/* interface IDebugFuncEvalAbortedEvent2 */
/* [unique][uuid][object] */ 


enum tagFUNC_EVAL_ABORT_RESULT
    {	ABORT_SUCCEEDED	= 0,
	RUDE_ABORT_SUCCEEDED	= 1,
	ABORT_FAILED	= 2,
	ABORT_HUNG	= 3,
	PROCESS_TERMINATED	= 4
    } ;
typedef enum tagFUNC_EVAL_ABORT_RESULT FUNC_EVAL_ABORT_RESULT;


EXTERN_C const IID IID_IDebugFuncEvalAbortedEvent2;

#if defined(__cplusplus) && !defined(CINTERFACE)
    
    MIDL_INTERFACE("3f3be369-0b78-4511-91e5-08f9fc5cae0d")
    IDebugFuncEvalAbortedEvent2 : public IUnknown
    {
    public:
        virtual HRESULT STDMETHODCALLTYPE GetAbortResult( 
            /* [out] */ __RPC__out FUNC_EVAL_ABORT_RESULT *pResult) = 0;
        
        virtual HRESULT STDMETHODCALLTYPE GetFunctionName( 
            /* [out] */ __RPC__deref_out_opt BSTR *pbstrFunctionName) = 0;
        
    };
    
#else 	/* C style interface */

    typedef struct IDebugFuncEvalAbortedEvent2Vtbl
    {
        BEGIN_INTERFACE
        
        HRESULT ( STDMETHODCALLTYPE *QueryInterface )( 
            IDebugFuncEvalAbortedEvent2 * This,
            /* [in] */ __RPC__in REFIID riid,
            /* [iid_is][out] */ 
            __RPC__deref_out  void **ppvObject);
        
        ULONG ( STDMETHODCALLTYPE *AddRef )( 
            IDebugFuncEvalAbortedEvent2 * This);
        
        ULONG ( STDMETHODCALLTYPE *Release )( 
            IDebugFuncEvalAbortedEvent2 * This);
        
        HRESULT ( STDMETHODCALLTYPE *GetAbortResult )( 
            IDebugFuncEvalAbortedEvent2 * This,
            /* [out] */ __RPC__out FUNC_EVAL_ABORT_RESULT *pResult);
        
        HRESULT ( STDMETHODCALLTYPE *GetFunctionName )( 
            IDebugFuncEvalAbortedEvent2 * This,
            /* [out] */ __RPC__deref_out_opt BSTR *pbstrFunctionName);
        
        END_INTERFACE
    } IDebugFuncEvalAbortedEvent2Vtbl;

    interface IDebugFuncEvalAbortedEvent2
    {
        CONST_VTBL struct IDebugFuncEvalAbortedEvent2Vtbl *lpVtbl;
    };

    

#ifdef COBJMACROS


#define IDebugFuncEvalAbortedEvent2_QueryInterface(This,riid,ppvObject)	\
    ( (This)->lpVtbl -> QueryInterface(This,riid,ppvObject) ) 

#define IDebugFuncEvalAbortedEvent2_AddRef(This)	\
    ( (This)->lpVtbl -> AddRef(This) ) 

#define IDebugFuncEvalAbortedEvent2_Release(This)	\
    ( (This)->lpVtbl -> Release(This) ) 


#define IDebugFuncEvalAbortedEvent2_GetAbortResult(This,pResult)	\
    ( (This)->lpVtbl -> GetAbortResult(This,pResult) ) 

#define IDebugFuncEvalAbortedEvent2_GetFunctionName(This,pbstrFunctionName)	\
    ( (This)->lpVtbl -> GetFunctionName(This,pbstrFunctionName) ) 

#endif /* COBJMACROS */


#endif 	/* C style interface */




#endif 	/* __IDebugFuncEvalAbortedEvent2_INTERFACE_DEFINED__ */


#ifndef __IDebugStopCompleteEvent2_INTERFACE_DEFINED__
#define __IDebugStopCompleteEvent2_INTERFACE_DEFINED__

/* interface IDebugStopCompleteEvent2 */
/* [unique][uuid][object] */ 


EXTERN_C const IID IID_IDebugStopCompleteEvent2;

#if defined(__cplusplus) && !defined(CINTERFACE)
    
    MIDL_INTERFACE("3dca9dcd-fb09-4af1-a926-45f293d48b2d")
    IDebugStopCompleteEvent2 : public IUnknown
    {
    public:
    };
    
#else 	/* C style interface */

    typedef struct IDebugStopCompleteEvent2Vtbl
    {
        BEGIN_INTERFACE
        
        HRESULT ( STDMETHODCALLTYPE *QueryInterface )( 
            IDebugStopCompleteEvent2 * This,
            /* [in] */ __RPC__in REFIID riid,
            /* [iid_is][out] */ 
            __RPC__deref_out  void **ppvObject);
        
        ULONG ( STDMETHODCALLTYPE *AddRef )( 
            IDebugStopCompleteEvent2 * This);
        
        ULONG ( STDMETHODCALLTYPE *Release )( 
            IDebugStopCompleteEvent2 * This);
        
        END_INTERFACE
    } IDebugStopCompleteEvent2Vtbl;

    interface IDebugStopCompleteEvent2
    {
        CONST_VTBL struct IDebugStopCompleteEvent2Vtbl *lpVtbl;
    };

    

#ifdef COBJMACROS


#define IDebugStopCompleteEvent2_QueryInterface(This,riid,ppvObject)	\
    ( (This)->lpVtbl -> QueryInterface(This,riid,ppvObject) ) 

#define IDebugStopCompleteEvent2_AddRef(This)	\
    ( (This)->lpVtbl -> AddRef(This) ) 

#define IDebugStopCompleteEvent2_Release(This)	\
    ( (This)->lpVtbl -> Release(This) ) 


#endif /* COBJMACROS */


#endif 	/* C style interface */




#endif 	/* __IDebugStopCompleteEvent2_INTERFACE_DEFINED__ */


/* interface __MIDL_itf_msdbg_0000_0063 */
/* [local] */ 


enum tagEncUnavailableReason
    {	ENCUN_NONE	= 0,
	ENCUN_INTEROP	= ( ENCUN_NONE + 1 ) ,
	ENCUN_SQLCLR	= ( ENCUN_INTEROP + 1 ) ,
	ENCUN_MINIDUMP	= ( ENCUN_SQLCLR + 1 ) ,
	ENCUN_EMBEDDED	= ( ENCUN_MINIDUMP + 1 ) ,
	ENCUN_ATTACH	= ( ENCUN_EMBEDDED + 1 ) ,
	ENCUN_WIN64	= ( ENCUN_ATTACH + 1 ) ,
	ENCUN_STOPONEMODE	= ( ENCUN_WIN64 + 1 ) ,
	ENCUN_MODULENOTLOADED	= ( ENCUN_STOPONEMODE + 1 ) ,
	ENCUN_MODULERELOADED	= ( ENCUN_MODULENOTLOADED + 1 ) ,
	ENCUN_INRUNMODE	= ( ENCUN_MODULERELOADED + 1 ) ,
	ENCUN_NOTBUILT	= ( ENCUN_INRUNMODE + 1 ) ,
	ENCUN_REMOTE	= ( ENCUN_NOTBUILT + 1 ) ,
	ENCUN_SILVERLIGHT	= ( ENCUN_REMOTE + 1 ) 
    } ;
typedef enum tagEncUnavailableReason EncUnavailableReason;



extern RPC_IF_HANDLE __MIDL_itf_msdbg_0000_0063_v0_0_c_ifspec;
extern RPC_IF_HANDLE __MIDL_itf_msdbg_0000_0063_v0_0_s_ifspec;

#ifndef __IDebugEncNotify_INTERFACE_DEFINED__
#define __IDebugEncNotify_INTERFACE_DEFINED__

/* interface IDebugEncNotify */
/* [unique][uuid][object] */ 


EXTERN_C const IID IID_IDebugEncNotify;

#if defined(__cplusplus) && !defined(CINTERFACE)
    
    MIDL_INTERFACE("347C45E1-5C42-4e0e-9E15-DEFF9CFC7841")
    IDebugEncNotify : public IUnknown
    {
    public:
        virtual HRESULT STDMETHODCALLTYPE NotifyEncIsUnavailable( 
            /* [in] */ EncUnavailableReason reason,
            /* [in] */ BOOL fEditWasApplied) = 0;
        
        virtual HRESULT STDMETHODCALLTYPE NotifyEncUpdateCurrentStatement( void) = 0;
        
        virtual HRESULT STDMETHODCALLTYPE NotifyEncEditAttemptedAtInvalidStopState( void) = 0;
        
        virtual HRESULT STDMETHODCALLTYPE NotifyEncEditDisallowedByProject( 
            /* [in] */ __RPC__in_opt IUnknown *pProject) = 0;
        
    };
    
#else 	/* C style interface */

    typedef struct IDebugEncNotifyVtbl
    {
        BEGIN_INTERFACE
        
        HRESULT ( STDMETHODCALLTYPE *QueryInterface )( 
            IDebugEncNotify * This,
            /* [in] */ __RPC__in REFIID riid,
            /* [iid_is][out] */ 
            __RPC__deref_out  void **ppvObject);
        
        ULONG ( STDMETHODCALLTYPE *AddRef )( 
            IDebugEncNotify * This);
        
        ULONG ( STDMETHODCALLTYPE *Release )( 
            IDebugEncNotify * This);
        
        HRESULT ( STDMETHODCALLTYPE *NotifyEncIsUnavailable )( 
            IDebugEncNotify * This,
            /* [in] */ EncUnavailableReason reason,
            /* [in] */ BOOL fEditWasApplied);
        
        HRESULT ( STDMETHODCALLTYPE *NotifyEncUpdateCurrentStatement )( 
            IDebugEncNotify * This);
        
        HRESULT ( STDMETHODCALLTYPE *NotifyEncEditAttemptedAtInvalidStopState )( 
            IDebugEncNotify * This);
        
        HRESULT ( STDMETHODCALLTYPE *NotifyEncEditDisallowedByProject )( 
            IDebugEncNotify * This,
            /* [in] */ __RPC__in_opt IUnknown *pProject);
        
        END_INTERFACE
    } IDebugEncNotifyVtbl;

    interface IDebugEncNotify
    {
        CONST_VTBL struct IDebugEncNotifyVtbl *lpVtbl;
    };

    

#ifdef COBJMACROS


#define IDebugEncNotify_QueryInterface(This,riid,ppvObject)	\
    ( (This)->lpVtbl -> QueryInterface(This,riid,ppvObject) ) 

#define IDebugEncNotify_AddRef(This)	\
    ( (This)->lpVtbl -> AddRef(This) ) 

#define IDebugEncNotify_Release(This)	\
    ( (This)->lpVtbl -> Release(This) ) 


#define IDebugEncNotify_NotifyEncIsUnavailable(This,reason,fEditWasApplied)	\
    ( (This)->lpVtbl -> NotifyEncIsUnavailable(This,reason,fEditWasApplied) ) 

#define IDebugEncNotify_NotifyEncUpdateCurrentStatement(This)	\
    ( (This)->lpVtbl -> NotifyEncUpdateCurrentStatement(This) ) 

#define IDebugEncNotify_NotifyEncEditAttemptedAtInvalidStopState(This)	\
    ( (This)->lpVtbl -> NotifyEncEditAttemptedAtInvalidStopState(This) ) 

#define IDebugEncNotify_NotifyEncEditDisallowedByProject(This,pProject)	\
    ( (This)->lpVtbl -> NotifyEncEditDisallowedByProject(This,pProject) ) 

#endif /* COBJMACROS */


#endif 	/* C style interface */




#endif 	/* __IDebugEncNotify_INTERFACE_DEFINED__ */


#ifndef __IDebugSessionEvent2_INTERFACE_DEFINED__
#define __IDebugSessionEvent2_INTERFACE_DEFINED__

/* interface IDebugSessionEvent2 */
/* [unique][uuid][object] */ 


EXTERN_C const IID IID_IDebugSessionEvent2;

#if defined(__cplusplus) && !defined(CINTERFACE)
    
    MIDL_INTERFACE("fd1a378c-f117-4f43-917c-dadca1308606")
    IDebugSessionEvent2 : public IUnknown
    {
    public:
        virtual HRESULT STDMETHODCALLTYPE GetSession( 
            /* [out] */ __RPC__deref_out_opt IDebugSession2 **ppSession) = 0;
        
    };
    
#else 	/* C style interface */

    typedef struct IDebugSessionEvent2Vtbl
    {
        BEGIN_INTERFACE
        
        HRESULT ( STDMETHODCALLTYPE *QueryInterface )( 
            IDebugSessionEvent2 * This,
            /* [in] */ __RPC__in REFIID riid,
            /* [iid_is][out] */ 
            __RPC__deref_out  void **ppvObject);
        
        ULONG ( STDMETHODCALLTYPE *AddRef )( 
            IDebugSessionEvent2 * This);
        
        ULONG ( STDMETHODCALLTYPE *Release )( 
            IDebugSessionEvent2 * This);
        
        HRESULT ( STDMETHODCALLTYPE *GetSession )( 
            IDebugSessionEvent2 * This,
            /* [out] */ __RPC__deref_out_opt IDebugSession2 **ppSession);
        
        END_INTERFACE
    } IDebugSessionEvent2Vtbl;

    interface IDebugSessionEvent2
    {
        CONST_VTBL struct IDebugSessionEvent2Vtbl *lpVtbl;
    };

    

#ifdef COBJMACROS


#define IDebugSessionEvent2_QueryInterface(This,riid,ppvObject)	\
    ( (This)->lpVtbl -> QueryInterface(This,riid,ppvObject) ) 

#define IDebugSessionEvent2_AddRef(This)	\
    ( (This)->lpVtbl -> AddRef(This) ) 

#define IDebugSessionEvent2_Release(This)	\
    ( (This)->lpVtbl -> Release(This) ) 


#define IDebugSessionEvent2_GetSession(This,ppSession)	\
    ( (This)->lpVtbl -> GetSession(This,ppSession) ) 

#endif /* COBJMACROS */


#endif 	/* C style interface */




#endif 	/* __IDebugSessionEvent2_INTERFACE_DEFINED__ */


#ifndef __IDebugProcess2_INTERFACE_DEFINED__
#define __IDebugProcess2_INTERFACE_DEFINED__

/* interface IDebugProcess2 */
/* [unique][uuid][object] */ 


enum enum_PROCESS_INFO_FLAGS
    {	PIFLAG_SYSTEM_PROCESS	= 0x1,
	PIFLAG_DEBUGGER_ATTACHED	= 0x2,
	PIFLAG_PROCESS_STOPPED	= 0x4,
	PIFLAG_PROCESS_RUNNING	= 0x8
    } ;
typedef DWORD PROCESS_INFO_FLAGS;


enum enum_PROCESS_INFO_FIELDS
    {	PIF_FILE_NAME	= 0x1,
	PIF_BASE_NAME	= 0x2,
	PIF_TITLE	= 0x4,
	PIF_PROCESS_ID	= 0x8,
	PIF_SESSION_ID	= 0x10,
	PIF_ATTACHED_SESSION_NAME	= 0x20,
	PIF_CREATION_TIME	= 0x40,
	PIF_FLAGS	= 0x80,
	PIF_ALL	= 0xff
    } ;
typedef DWORD PROCESS_INFO_FIELDS;

typedef struct tagPROCESS_INFO
    {
    PROCESS_INFO_FIELDS Fields;
    BSTR bstrFileName;
    BSTR bstrBaseName;
    BSTR bstrTitle;
    AD_PROCESS_ID ProcessId;
    DWORD dwSessionId;
    BSTR bstrAttachedSessionName;
    FILETIME CreationTime;
    PROCESS_INFO_FLAGS Flags;
    } 	PROCESS_INFO;


EXTERN_C const IID IID_IDebugProcess2;

#if defined(__cplusplus) && !defined(CINTERFACE)
    
    MIDL_INTERFACE("43286fea-6997-4543-803e-60a20c473de5")
    IDebugProcess2 : public IUnknown
    {
    public:
        virtual HRESULT STDMETHODCALLTYPE GetInfo( 
            /* [in] */ PROCESS_INFO_FIELDS Fields,
            /* [out] */ __RPC__out PROCESS_INFO *pProcessInfo) = 0;
        
        virtual HRESULT STDMETHODCALLTYPE EnumPrograms( 
            /* [out] */ __RPC__deref_out_opt IEnumDebugPrograms2 **ppEnum) = 0;
        
        virtual HRESULT STDMETHODCALLTYPE GetName( 
            /* [in] */ GETNAME_TYPE gnType,
            /* [out] */ __RPC__deref_out_opt BSTR *pbstrName) = 0;
        
        virtual HRESULT STDMETHODCALLTYPE GetServer( 
            /* [out] */ __RPC__deref_out_opt IDebugCoreServer2 **ppServer) = 0;
        
        virtual HRESULT STDMETHODCALLTYPE Terminate( void) = 0;
        
        virtual HRESULT STDMETHODCALLTYPE Attach( 
            /* [in] */ __RPC__in_opt IDebugEventCallback2 *pCallback,
            /* [size_is][in] */ __RPC__in_ecount_full(celtSpecificEngines) GUID *rgguidSpecificEngines,
            /* [in] */ DWORD celtSpecificEngines,
            /* [length_is][size_is][out] */ __RPC__out_ecount_part(celtSpecificEngines, celtSpecificEngines) HRESULT *rghrEngineAttach) = 0;
        
        virtual HRESULT STDMETHODCALLTYPE CanDetach( void) = 0;
        
        virtual HRESULT STDMETHODCALLTYPE Detach( void) = 0;
        
        virtual HRESULT STDMETHODCALLTYPE GetPhysicalProcessId( 
            /* [out] */ __RPC__out AD_PROCESS_ID *pProcessId) = 0;
        
        virtual HRESULT STDMETHODCALLTYPE GetProcessId( 
            /* [out] */ __RPC__out GUID *pguidProcessId) = 0;
        
        virtual HRESULT STDMETHODCALLTYPE GetAttachedSessionName( 
            /* [out] */ __RPC__deref_out_opt BSTR *pbstrSessionName) = 0;
        
        virtual HRESULT STDMETHODCALLTYPE EnumThreads( 
            /* [out] */ __RPC__deref_out_opt IEnumDebugThreads2 **ppEnum) = 0;
        
        virtual HRESULT STDMETHODCALLTYPE CauseBreak( void) = 0;
        
        virtual HRESULT STDMETHODCALLTYPE GetPort( 
            /* [out] */ __RPC__deref_out_opt IDebugPort2 **ppPort) = 0;
        
    };
    
#else 	/* C style interface */

    typedef struct IDebugProcess2Vtbl
    {
        BEGIN_INTERFACE
        
        HRESULT ( STDMETHODCALLTYPE *QueryInterface )( 
            IDebugProcess2 * This,
            /* [in] */ __RPC__in REFIID riid,
            /* [iid_is][out] */ 
            __RPC__deref_out  void **ppvObject);
        
        ULONG ( STDMETHODCALLTYPE *AddRef )( 
            IDebugProcess2 * This);
        
        ULONG ( STDMETHODCALLTYPE *Release )( 
            IDebugProcess2 * This);
        
        HRESULT ( STDMETHODCALLTYPE *GetInfo )( 
            IDebugProcess2 * This,
            /* [in] */ PROCESS_INFO_FIELDS Fields,
            /* [out] */ __RPC__out PROCESS_INFO *pProcessInfo);
        
        HRESULT ( STDMETHODCALLTYPE *EnumPrograms )( 
            IDebugProcess2 * This,
            /* [out] */ __RPC__deref_out_opt IEnumDebugPrograms2 **ppEnum);
        
        HRESULT ( STDMETHODCALLTYPE *GetName )( 
            IDebugProcess2 * This,
            /* [in] */ GETNAME_TYPE gnType,
            /* [out] */ __RPC__deref_out_opt BSTR *pbstrName);
        
        HRESULT ( STDMETHODCALLTYPE *GetServer )( 
            IDebugProcess2 * This,
            /* [out] */ __RPC__deref_out_opt IDebugCoreServer2 **ppServer);
        
        HRESULT ( STDMETHODCALLTYPE *Terminate )( 
            IDebugProcess2 * This);
        
        HRESULT ( STDMETHODCALLTYPE *Attach )( 
            IDebugProcess2 * This,
            /* [in] */ __RPC__in_opt IDebugEventCallback2 *pCallback,
            /* [size_is][in] */ __RPC__in_ecount_full(celtSpecificEngines) GUID *rgguidSpecificEngines,
            /* [in] */ DWORD celtSpecificEngines,
            /* [length_is][size_is][out] */ __RPC__out_ecount_part(celtSpecificEngines, celtSpecificEngines) HRESULT *rghrEngineAttach);
        
        HRESULT ( STDMETHODCALLTYPE *CanDetach )( 
            IDebugProcess2 * This);
        
        HRESULT ( STDMETHODCALLTYPE *Detach )( 
            IDebugProcess2 * This);
        
        HRESULT ( STDMETHODCALLTYPE *GetPhysicalProcessId )( 
            IDebugProcess2 * This,
            /* [out] */ __RPC__out AD_PROCESS_ID *pProcessId);
        
        HRESULT ( STDMETHODCALLTYPE *GetProcessId )( 
            IDebugProcess2 * This,
            /* [out] */ __RPC__out GUID *pguidProcessId);
        
        HRESULT ( STDMETHODCALLTYPE *GetAttachedSessionName )( 
            IDebugProcess2 * This,
            /* [out] */ __RPC__deref_out_opt BSTR *pbstrSessionName);
        
        HRESULT ( STDMETHODCALLTYPE *EnumThreads )( 
            IDebugProcess2 * This,
            /* [out] */ __RPC__deref_out_opt IEnumDebugThreads2 **ppEnum);
        
        HRESULT ( STDMETHODCALLTYPE *CauseBreak )( 
            IDebugProcess2 * This);
        
        HRESULT ( STDMETHODCALLTYPE *GetPort )( 
            IDebugProcess2 * This,
            /* [out] */ __RPC__deref_out_opt IDebugPort2 **ppPort);
        
        END_INTERFACE
    } IDebugProcess2Vtbl;

    interface IDebugProcess2
    {
        CONST_VTBL struct IDebugProcess2Vtbl *lpVtbl;
    };

    

#ifdef COBJMACROS


#define IDebugProcess2_QueryInterface(This,riid,ppvObject)	\
    ( (This)->lpVtbl -> QueryInterface(This,riid,ppvObject) ) 

#define IDebugProcess2_AddRef(This)	\
    ( (This)->lpVtbl -> AddRef(This) ) 

#define IDebugProcess2_Release(This)	\
    ( (This)->lpVtbl -> Release(This) ) 


#define IDebugProcess2_GetInfo(This,Fields,pProcessInfo)	\
    ( (This)->lpVtbl -> GetInfo(This,Fields,pProcessInfo) ) 

#define IDebugProcess2_EnumPrograms(This,ppEnum)	\
    ( (This)->lpVtbl -> EnumPrograms(This,ppEnum) ) 

#define IDebugProcess2_GetName(This,gnType,pbstrName)	\
    ( (This)->lpVtbl -> GetName(This,gnType,pbstrName) ) 

#define IDebugProcess2_GetServer(This,ppServer)	\
    ( (This)->lpVtbl -> GetServer(This,ppServer) ) 

#define IDebugProcess2_Terminate(This)	\
    ( (This)->lpVtbl -> Terminate(This) ) 

#define IDebugProcess2_Attach(This,pCallback,rgguidSpecificEngines,celtSpecificEngines,rghrEngineAttach)	\
    ( (This)->lpVtbl -> Attach(This,pCallback,rgguidSpecificEngines,celtSpecificEngines,rghrEngineAttach) ) 

#define IDebugProcess2_CanDetach(This)	\
    ( (This)->lpVtbl -> CanDetach(This) ) 

#define IDebugProcess2_Detach(This)	\
    ( (This)->lpVtbl -> Detach(This) ) 

#define IDebugProcess2_GetPhysicalProcessId(This,pProcessId)	\
    ( (This)->lpVtbl -> GetPhysicalProcessId(This,pProcessId) ) 

#define IDebugProcess2_GetProcessId(This,pguidProcessId)	\
    ( (This)->lpVtbl -> GetProcessId(This,pguidProcessId) ) 

#define IDebugProcess2_GetAttachedSessionName(This,pbstrSessionName)	\
    ( (This)->lpVtbl -> GetAttachedSessionName(This,pbstrSessionName) ) 

#define IDebugProcess2_EnumThreads(This,ppEnum)	\
    ( (This)->lpVtbl -> EnumThreads(This,ppEnum) ) 

#define IDebugProcess2_CauseBreak(This)	\
    ( (This)->lpVtbl -> CauseBreak(This) ) 

#define IDebugProcess2_GetPort(This,ppPort)	\
    ( (This)->lpVtbl -> GetPort(This,ppPort) ) 

#endif /* COBJMACROS */


#endif 	/* C style interface */




#endif 	/* __IDebugProcess2_INTERFACE_DEFINED__ */


#ifndef __IDebugProcess3_INTERFACE_DEFINED__
#define __IDebugProcess3_INTERFACE_DEFINED__

/* interface IDebugProcess3 */
/* [unique][uuid][object] */ 


enum enum_STEPKIND
    {	STEP_INTO	= 0,
	STEP_OVER	= ( STEP_INTO + 1 ) ,
	STEP_OUT	= ( STEP_OVER + 1 ) ,
	STEP_BACKWARDS	= ( STEP_OUT + 1 ) 
    } ;
typedef DWORD STEPKIND;


enum enum_STEPUNIT
    {	STEP_STATEMENT	= 0,
	STEP_LINE	= ( STEP_STATEMENT + 1 ) ,
	STEP_INSTRUCTION	= ( STEP_LINE + 1 ) 
    } ;
typedef DWORD STEPUNIT;


enum enum_DEBUG_REASON
    {	DEBUG_REASON_ERROR	= 0,
	DEBUG_REASON_USER_LAUNCHED	= ( DEBUG_REASON_ERROR + 1 ) ,
	DEBUG_REASON_USER_ATTACHED	= ( DEBUG_REASON_USER_LAUNCHED + 1 ) ,
	DEBUG_REASON_AUTO_ATTACHED	= ( DEBUG_REASON_USER_ATTACHED + 1 ) ,
	DEBUG_REASON_CAUSALITY	= ( DEBUG_REASON_AUTO_ATTACHED + 1 ) 
    } ;
typedef DWORD DEBUG_REASON;


EXTERN_C const IID IID_IDebugProcess3;

#if defined(__cplusplus) && !defined(CINTERFACE)
    
    MIDL_INTERFACE("83ab1712-18a6-47a1-8da6-8c7b0f96092e")
    IDebugProcess3 : public IDebugProcess2
    {
    public:
        virtual HRESULT STDMETHODCALLTYPE Execute( 
            /* [in] */ __RPC__in_opt IDebugThread2 *pThread) = 0;
        
        virtual HRESULT STDMETHODCALLTYPE Continue( 
            /* [in] */ __RPC__in_opt IDebugThread2 *pThread) = 0;
        
        virtual HRESULT STDMETHODCALLTYPE Step( 
            /* [in] */ __RPC__in_opt IDebugThread2 *pThread,
            /* [in] */ STEPKIND sk,
            /* [in] */ STEPUNIT step) = 0;
        
        virtual HRESULT STDMETHODCALLTYPE GetDebugReason( 
            /* [out] */ __RPC__out DEBUG_REASON *pReason) = 0;
        
        virtual HRESULT STDMETHODCALLTYPE SetHostingProcessLanguage( 
            /* [in] */ __RPC__in REFGUID guidLang) = 0;
        
        virtual HRESULT STDMETHODCALLTYPE GetHostingProcessLanguage( 
            /* [out] */ __RPC__out GUID *pguidLang) = 0;
        
        virtual HRESULT STDMETHODCALLTYPE DisableENC( 
            /* [in] */ EncUnavailableReason reason) = 0;
        
        virtual HRESULT STDMETHODCALLTYPE GetENCAvailableState( 
            /* [out] */ __RPC__out EncUnavailableReason *preason) = 0;
        
        virtual HRESULT STDMETHODCALLTYPE GetEngineFilter( 
            /* [out] */ __RPC__out GUID_ARRAY *pEngineArray) = 0;
        
    };
    
#else 	/* C style interface */

    typedef struct IDebugProcess3Vtbl
    {
        BEGIN_INTERFACE
        
        HRESULT ( STDMETHODCALLTYPE *QueryInterface )( 
            IDebugProcess3 * This,
            /* [in] */ __RPC__in REFIID riid,
            /* [iid_is][out] */ 
            __RPC__deref_out  void **ppvObject);
        
        ULONG ( STDMETHODCALLTYPE *AddRef )( 
            IDebugProcess3 * This);
        
        ULONG ( STDMETHODCALLTYPE *Release )( 
            IDebugProcess3 * This);
        
        HRESULT ( STDMETHODCALLTYPE *GetInfo )( 
            IDebugProcess3 * This,
            /* [in] */ PROCESS_INFO_FIELDS Fields,
            /* [out] */ __RPC__out PROCESS_INFO *pProcessInfo);
        
        HRESULT ( STDMETHODCALLTYPE *EnumPrograms )( 
            IDebugProcess3 * This,
            /* [out] */ __RPC__deref_out_opt IEnumDebugPrograms2 **ppEnum);
        
        HRESULT ( STDMETHODCALLTYPE *GetName )( 
            IDebugProcess3 * This,
            /* [in] */ GETNAME_TYPE gnType,
            /* [out] */ __RPC__deref_out_opt BSTR *pbstrName);
        
        HRESULT ( STDMETHODCALLTYPE *GetServer )( 
            IDebugProcess3 * This,
            /* [out] */ __RPC__deref_out_opt IDebugCoreServer2 **ppServer);
        
        HRESULT ( STDMETHODCALLTYPE *Terminate )( 
            IDebugProcess3 * This);
        
        HRESULT ( STDMETHODCALLTYPE *Attach )( 
            IDebugProcess3 * This,
            /* [in] */ __RPC__in_opt IDebugEventCallback2 *pCallback,
            /* [size_is][in] */ __RPC__in_ecount_full(celtSpecificEngines) GUID *rgguidSpecificEngines,
            /* [in] */ DWORD celtSpecificEngines,
            /* [length_is][size_is][out] */ __RPC__out_ecount_part(celtSpecificEngines, celtSpecificEngines) HRESULT *rghrEngineAttach);
        
        HRESULT ( STDMETHODCALLTYPE *CanDetach )( 
            IDebugProcess3 * This);
        
        HRESULT ( STDMETHODCALLTYPE *Detach )( 
            IDebugProcess3 * This);
        
        HRESULT ( STDMETHODCALLTYPE *GetPhysicalProcessId )( 
            IDebugProcess3 * This,
            /* [out] */ __RPC__out AD_PROCESS_ID *pProcessId);
        
        HRESULT ( STDMETHODCALLTYPE *GetProcessId )( 
            IDebugProcess3 * This,
            /* [out] */ __RPC__out GUID *pguidProcessId);
        
        HRESULT ( STDMETHODCALLTYPE *GetAttachedSessionName )( 
            IDebugProcess3 * This,
            /* [out] */ __RPC__deref_out_opt BSTR *pbstrSessionName);
        
        HRESULT ( STDMETHODCALLTYPE *EnumThreads )( 
            IDebugProcess3 * This,
            /* [out] */ __RPC__deref_out_opt IEnumDebugThreads2 **ppEnum);
        
        HRESULT ( STDMETHODCALLTYPE *CauseBreak )( 
            IDebugProcess3 * This);
        
        HRESULT ( STDMETHODCALLTYPE *GetPort )( 
            IDebugProcess3 * This,
            /* [out] */ __RPC__deref_out_opt IDebugPort2 **ppPort);
        
        HRESULT ( STDMETHODCALLTYPE *Execute )( 
            IDebugProcess3 * This,
            /* [in] */ __RPC__in_opt IDebugThread2 *pThread);
        
        HRESULT ( STDMETHODCALLTYPE *Continue )( 
            IDebugProcess3 * This,
            /* [in] */ __RPC__in_opt IDebugThread2 *pThread);
        
        HRESULT ( STDMETHODCALLTYPE *Step )( 
            IDebugProcess3 * This,
            /* [in] */ __RPC__in_opt IDebugThread2 *pThread,
            /* [in] */ STEPKIND sk,
            /* [in] */ STEPUNIT step);
        
        HRESULT ( STDMETHODCALLTYPE *GetDebugReason )( 
            IDebugProcess3 * This,
            /* [out] */ __RPC__out DEBUG_REASON *pReason);
        
        HRESULT ( STDMETHODCALLTYPE *SetHostingProcessLanguage )( 
            IDebugProcess3 * This,
            /* [in] */ __RPC__in REFGUID guidLang);
        
        HRESULT ( STDMETHODCALLTYPE *GetHostingProcessLanguage )( 
            IDebugProcess3 * This,
            /* [out] */ __RPC__out GUID *pguidLang);
        
        HRESULT ( STDMETHODCALLTYPE *DisableENC )( 
            IDebugProcess3 * This,
            /* [in] */ EncUnavailableReason reason);
        
        HRESULT ( STDMETHODCALLTYPE *GetENCAvailableState )( 
            IDebugProcess3 * This,
            /* [out] */ __RPC__out EncUnavailableReason *preason);
        
        HRESULT ( STDMETHODCALLTYPE *GetEngineFilter )( 
            IDebugProcess3 * This,
            /* [out] */ __RPC__out GUID_ARRAY *pEngineArray);
        
        END_INTERFACE
    } IDebugProcess3Vtbl;

    interface IDebugProcess3
    {
        CONST_VTBL struct IDebugProcess3Vtbl *lpVtbl;
    };

    

#ifdef COBJMACROS


#define IDebugProcess3_QueryInterface(This,riid,ppvObject)	\
    ( (This)->lpVtbl -> QueryInterface(This,riid,ppvObject) ) 

#define IDebugProcess3_AddRef(This)	\
    ( (This)->lpVtbl -> AddRef(This) ) 

#define IDebugProcess3_Release(This)	\
    ( (This)->lpVtbl -> Release(This) ) 


#define IDebugProcess3_GetInfo(This,Fields,pProcessInfo)	\
    ( (This)->lpVtbl -> GetInfo(This,Fields,pProcessInfo) ) 

#define IDebugProcess3_EnumPrograms(This,ppEnum)	\
    ( (This)->lpVtbl -> EnumPrograms(This,ppEnum) ) 

#define IDebugProcess3_GetName(This,gnType,pbstrName)	\
    ( (This)->lpVtbl -> GetName(This,gnType,pbstrName) ) 

#define IDebugProcess3_GetServer(This,ppServer)	\
    ( (This)->lpVtbl -> GetServer(This,ppServer) ) 

#define IDebugProcess3_Terminate(This)	\
    ( (This)->lpVtbl -> Terminate(This) ) 

#define IDebugProcess3_Attach(This,pCallback,rgguidSpecificEngines,celtSpecificEngines,rghrEngineAttach)	\
    ( (This)->lpVtbl -> Attach(This,pCallback,rgguidSpecificEngines,celtSpecificEngines,rghrEngineAttach) ) 

#define IDebugProcess3_CanDetach(This)	\
    ( (This)->lpVtbl -> CanDetach(This) ) 

#define IDebugProcess3_Detach(This)	\
    ( (This)->lpVtbl -> Detach(This) ) 

#define IDebugProcess3_GetPhysicalProcessId(This,pProcessId)	\
    ( (This)->lpVtbl -> GetPhysicalProcessId(This,pProcessId) ) 

#define IDebugProcess3_GetProcessId(This,pguidProcessId)	\
    ( (This)->lpVtbl -> GetProcessId(This,pguidProcessId) ) 

#define IDebugProcess3_GetAttachedSessionName(This,pbstrSessionName)	\
    ( (This)->lpVtbl -> GetAttachedSessionName(This,pbstrSessionName) ) 

#define IDebugProcess3_EnumThreads(This,ppEnum)	\
    ( (This)->lpVtbl -> EnumThreads(This,ppEnum) ) 

#define IDebugProcess3_CauseBreak(This)	\
    ( (This)->lpVtbl -> CauseBreak(This) ) 

#define IDebugProcess3_GetPort(This,ppPort)	\
    ( (This)->lpVtbl -> GetPort(This,ppPort) ) 


#define IDebugProcess3_Execute(This,pThread)	\
    ( (This)->lpVtbl -> Execute(This,pThread) ) 

#define IDebugProcess3_Continue(This,pThread)	\
    ( (This)->lpVtbl -> Continue(This,pThread) ) 

#define IDebugProcess3_Step(This,pThread,sk,step)	\
    ( (This)->lpVtbl -> Step(This,pThread,sk,step) ) 

#define IDebugProcess3_GetDebugReason(This,pReason)	\
    ( (This)->lpVtbl -> GetDebugReason(This,pReason) ) 

#define IDebugProcess3_SetHostingProcessLanguage(This,guidLang)	\
    ( (This)->lpVtbl -> SetHostingProcessLanguage(This,guidLang) ) 

#define IDebugProcess3_GetHostingProcessLanguage(This,pguidLang)	\
    ( (This)->lpVtbl -> GetHostingProcessLanguage(This,pguidLang) ) 

#define IDebugProcess3_DisableENC(This,reason)	\
    ( (This)->lpVtbl -> DisableENC(This,reason) ) 

#define IDebugProcess3_GetENCAvailableState(This,preason)	\
    ( (This)->lpVtbl -> GetENCAvailableState(This,preason) ) 

#define IDebugProcess3_GetEngineFilter(This,pEngineArray)	\
    ( (This)->lpVtbl -> GetEngineFilter(This,pEngineArray) ) 

#endif /* COBJMACROS */


#endif 	/* C style interface */




#endif 	/* __IDebugProcess3_INTERFACE_DEFINED__ */


#ifndef __IDebugProcessSecurity2_INTERFACE_DEFINED__
#define __IDebugProcessSecurity2_INTERFACE_DEFINED__

/* interface IDebugProcessSecurity2 */
/* [unique][uuid][object] */ 


EXTERN_C const IID IID_IDebugProcessSecurity2;

#if defined(__cplusplus) && !defined(CINTERFACE)
    
    MIDL_INTERFACE("fca0c35c-4c02-432b-88f7-eb277be2ba55")
    IDebugProcessSecurity2 : public IUnknown
    {
    public:
        virtual HRESULT STDMETHODCALLTYPE QueryCanSafelyAttach( void) = 0;
        
        virtual HRESULT STDMETHODCALLTYPE GetUserName( 
            /* [out] */ __RPC__deref_out_opt BSTR *pbstrUserName) = 0;
        
    };
    
#else 	/* C style interface */

    typedef struct IDebugProcessSecurity2Vtbl
    {
        BEGIN_INTERFACE
        
        HRESULT ( STDMETHODCALLTYPE *QueryInterface )( 
            IDebugProcessSecurity2 * This,
            /* [in] */ __RPC__in REFIID riid,
            /* [iid_is][out] */ 
            __RPC__deref_out  void **ppvObject);
        
        ULONG ( STDMETHODCALLTYPE *AddRef )( 
            IDebugProcessSecurity2 * This);
        
        ULONG ( STDMETHODCALLTYPE *Release )( 
            IDebugProcessSecurity2 * This);
        
        HRESULT ( STDMETHODCALLTYPE *QueryCanSafelyAttach )( 
            IDebugProcessSecurity2 * This);
        
        HRESULT ( STDMETHODCALLTYPE *GetUserName )( 
            IDebugProcessSecurity2 * This,
            /* [out] */ __RPC__deref_out_opt BSTR *pbstrUserName);
        
        END_INTERFACE
    } IDebugProcessSecurity2Vtbl;

    interface IDebugProcessSecurity2
    {
        CONST_VTBL struct IDebugProcessSecurity2Vtbl *lpVtbl;
    };

    

#ifdef COBJMACROS


#define IDebugProcessSecurity2_QueryInterface(This,riid,ppvObject)	\
    ( (This)->lpVtbl -> QueryInterface(This,riid,ppvObject) ) 

#define IDebugProcessSecurity2_AddRef(This)	\
    ( (This)->lpVtbl -> AddRef(This) ) 

#define IDebugProcessSecurity2_Release(This)	\
    ( (This)->lpVtbl -> Release(This) ) 


#define IDebugProcessSecurity2_QueryCanSafelyAttach(This)	\
    ( (This)->lpVtbl -> QueryCanSafelyAttach(This) ) 

#define IDebugProcessSecurity2_GetUserName(This,pbstrUserName)	\
    ( (This)->lpVtbl -> GetUserName(This,pbstrUserName) ) 

#endif /* COBJMACROS */


#endif 	/* C style interface */




#endif 	/* __IDebugProcessSecurity2_INTERFACE_DEFINED__ */


#ifndef __IDebugProgram2_INTERFACE_DEFINED__
#define __IDebugProgram2_INTERFACE_DEFINED__

/* interface IDebugProgram2 */
/* [unique][uuid][object] */ 


enum enum_DISASSEMBLY_STREAM_SCOPE
    {	DSS_HUGE	= 0x10000000,
	DSS_FUNCTION	= 0x1,
	DSS_MODULE	= ( DSS_HUGE | 0x2 ) ,
	DSS_ALL	= ( DSS_HUGE | 0x3 ) 
    } ;
typedef DWORD DISASSEMBLY_STREAM_SCOPE;


enum enum_DUMPTYPE
    {	DUMP_MINIDUMP	= 0,
	DUMP_FULLDUMP	= ( DUMP_MINIDUMP + 1 ) 
    } ;
typedef DWORD DUMPTYPE;

typedef struct tagCODE_PATH
    {
    BSTR bstrName;
    IDebugCodeContext2 *pCode;
    } 	CODE_PATH;


EXTERN_C const IID IID_IDebugProgram2;

#if defined(__cplusplus) && !defined(CINTERFACE)
    
    MIDL_INTERFACE("69d172ef-f2c4-44e1-89f7-c86231e706e9")
    IDebugProgram2 : public IUnknown
    {
    public:
        virtual HRESULT STDMETHODCALLTYPE EnumThreads( 
            /* [out] */ __RPC__deref_out_opt IEnumDebugThreads2 **ppEnum) = 0;
        
        virtual HRESULT STDMETHODCALLTYPE GetName( 
            /* [out] */ __RPC__deref_out_opt BSTR *pbstrName) = 0;
        
        virtual HRESULT STDMETHODCALLTYPE GetProcess( 
            /* [out] */ __RPC__deref_out_opt IDebugProcess2 **ppProcess) = 0;
        
        virtual HRESULT STDMETHODCALLTYPE Terminate( void) = 0;
        
        virtual HRESULT STDMETHODCALLTYPE Attach( 
            /* [in] */ __RPC__in_opt IDebugEventCallback2 *pCallback) = 0;
        
        virtual HRESULT STDMETHODCALLTYPE CanDetach( void) = 0;
        
        virtual HRESULT STDMETHODCALLTYPE Detach( void) = 0;
        
        virtual HRESULT STDMETHODCALLTYPE GetProgramId( 
            /* [out] */ __RPC__out GUID *pguidProgramId) = 0;
        
        virtual HRESULT STDMETHODCALLTYPE GetDebugProperty( 
            /* [out] */ __RPC__deref_out_opt IDebugProperty2 **ppProperty) = 0;
        
        virtual HRESULT STDMETHODCALLTYPE Execute( void) = 0;
        
        virtual HRESULT STDMETHODCALLTYPE Continue( 
            /* [in] */ __RPC__in_opt IDebugThread2 *pThread) = 0;
        
        virtual HRESULT STDMETHODCALLTYPE Step( 
            /* [in] */ __RPC__in_opt IDebugThread2 *pThread,
            /* [in] */ STEPKIND sk,
            /* [in] */ STEPUNIT step) = 0;
        
        virtual HRESULT STDMETHODCALLTYPE CauseBreak( void) = 0;
        
        virtual HRESULT STDMETHODCALLTYPE GetEngineInfo( 
            /* [out] */ __RPC__deref_out_opt BSTR *pbstrEngine,
            /* [out] */ __RPC__out GUID *pguidEngine) = 0;
        
        virtual HRESULT STDMETHODCALLTYPE EnumCodeContexts( 
            /* [in] */ __RPC__in_opt IDebugDocumentPosition2 *pDocPos,
            /* [out] */ __RPC__deref_out_opt IEnumDebugCodeContexts2 **ppEnum) = 0;
        
        virtual HRESULT STDMETHODCALLTYPE GetMemoryBytes( 
            /* [out] */ __RPC__deref_out_opt IDebugMemoryBytes2 **ppMemoryBytes) = 0;
        
        virtual HRESULT STDMETHODCALLTYPE GetDisassemblyStream( 
            /* [in] */ DISASSEMBLY_STREAM_SCOPE dwScope,
            /* [in] */ __RPC__in_opt IDebugCodeContext2 *pCodeContext,
            /* [out] */ __RPC__deref_out_opt IDebugDisassemblyStream2 **ppDisassemblyStream) = 0;
        
        virtual HRESULT STDMETHODCALLTYPE EnumModules( 
            /* [out] */ __RPC__deref_out_opt IEnumDebugModules2 **ppEnum) = 0;
        
        virtual HRESULT STDMETHODCALLTYPE GetENCUpdate( 
            /* [out] */ __RPC__deref_out_opt IDebugENCUpdate **ppUpdate) = 0;
        
        virtual HRESULT STDMETHODCALLTYPE EnumCodePaths( 
            /* [full][in] */ __RPC__in_opt LPCOLESTR pszHint,
            /* [in] */ __RPC__in_opt IDebugCodeContext2 *pStart,
            /* [in] */ __RPC__in_opt IDebugStackFrame2 *pFrame,
            /* [in] */ BOOL fSource,
            /* [out] */ __RPC__deref_out_opt IEnumCodePaths2 **ppEnum,
            /* [out] */ __RPC__deref_out_opt IDebugCodeContext2 **ppSafety) = 0;
        
        virtual HRESULT STDMETHODCALLTYPE WriteDump( 
            /* [in] */ DUMPTYPE DumpType,
            /* [in] */ __RPC__in LPCOLESTR pszDumpUrl) = 0;
        
    };
    
#else 	/* C style interface */

    typedef struct IDebugProgram2Vtbl
    {
        BEGIN_INTERFACE
        
        HRESULT ( STDMETHODCALLTYPE *QueryInterface )( 
            IDebugProgram2 * This,
            /* [in] */ __RPC__in REFIID riid,
            /* [iid_is][out] */ 
            __RPC__deref_out  void **ppvObject);
        
        ULONG ( STDMETHODCALLTYPE *AddRef )( 
            IDebugProgram2 * This);
        
        ULONG ( STDMETHODCALLTYPE *Release )( 
            IDebugProgram2 * This);
        
        HRESULT ( STDMETHODCALLTYPE *EnumThreads )( 
            IDebugProgram2 * This,
            /* [out] */ __RPC__deref_out_opt IEnumDebugThreads2 **ppEnum);
        
        HRESULT ( STDMETHODCALLTYPE *GetName )( 
            IDebugProgram2 * This,
            /* [out] */ __RPC__deref_out_opt BSTR *pbstrName);
        
        HRESULT ( STDMETHODCALLTYPE *GetProcess )( 
            IDebugProgram2 * This,
            /* [out] */ __RPC__deref_out_opt IDebugProcess2 **ppProcess);
        
        HRESULT ( STDMETHODCALLTYPE *Terminate )( 
            IDebugProgram2 * This);
        
        HRESULT ( STDMETHODCALLTYPE *Attach )( 
            IDebugProgram2 * This,
            /* [in] */ __RPC__in_opt IDebugEventCallback2 *pCallback);
        
        HRESULT ( STDMETHODCALLTYPE *CanDetach )( 
            IDebugProgram2 * This);
        
        HRESULT ( STDMETHODCALLTYPE *Detach )( 
            IDebugProgram2 * This);
        
        HRESULT ( STDMETHODCALLTYPE *GetProgramId )( 
            IDebugProgram2 * This,
            /* [out] */ __RPC__out GUID *pguidProgramId);
        
        HRESULT ( STDMETHODCALLTYPE *GetDebugProperty )( 
            IDebugProgram2 * This,
            /* [out] */ __RPC__deref_out_opt IDebugProperty2 **ppProperty);
        
        HRESULT ( STDMETHODCALLTYPE *Execute )( 
            IDebugProgram2 * This);
        
        HRESULT ( STDMETHODCALLTYPE *Continue )( 
            IDebugProgram2 * This,
            /* [in] */ __RPC__in_opt IDebugThread2 *pThread);
        
        HRESULT ( STDMETHODCALLTYPE *Step )( 
            IDebugProgram2 * This,
            /* [in] */ __RPC__in_opt IDebugThread2 *pThread,
            /* [in] */ STEPKIND sk,
            /* [in] */ STEPUNIT step);
        
        HRESULT ( STDMETHODCALLTYPE *CauseBreak )( 
            IDebugProgram2 * This);
        
        HRESULT ( STDMETHODCALLTYPE *GetEngineInfo )( 
            IDebugProgram2 * This,
            /* [out] */ __RPC__deref_out_opt BSTR *pbstrEngine,
            /* [out] */ __RPC__out GUID *pguidEngine);
        
        HRESULT ( STDMETHODCALLTYPE *EnumCodeContexts )( 
            IDebugProgram2 * This,
            /* [in] */ __RPC__in_opt IDebugDocumentPosition2 *pDocPos,
            /* [out] */ __RPC__deref_out_opt IEnumDebugCodeContexts2 **ppEnum);
        
        HRESULT ( STDMETHODCALLTYPE *GetMemoryBytes )( 
            IDebugProgram2 * This,
            /* [out] */ __RPC__deref_out_opt IDebugMemoryBytes2 **ppMemoryBytes);
        
        HRESULT ( STDMETHODCALLTYPE *GetDisassemblyStream )( 
            IDebugProgram2 * This,
            /* [in] */ DISASSEMBLY_STREAM_SCOPE dwScope,
            /* [in] */ __RPC__in_opt IDebugCodeContext2 *pCodeContext,
            /* [out] */ __RPC__deref_out_opt IDebugDisassemblyStream2 **ppDisassemblyStream);
        
        HRESULT ( STDMETHODCALLTYPE *EnumModules )( 
            IDebugProgram2 * This,
            /* [out] */ __RPC__deref_out_opt IEnumDebugModules2 **ppEnum);
        
        HRESULT ( STDMETHODCALLTYPE *GetENCUpdate )( 
            IDebugProgram2 * This,
            /* [out] */ __RPC__deref_out_opt IDebugENCUpdate **ppUpdate);
        
        HRESULT ( STDMETHODCALLTYPE *EnumCodePaths )( 
            IDebugProgram2 * This,
            /* [full][in] */ __RPC__in_opt LPCOLESTR pszHint,
            /* [in] */ __RPC__in_opt IDebugCodeContext2 *pStart,
            /* [in] */ __RPC__in_opt IDebugStackFrame2 *pFrame,
            /* [in] */ BOOL fSource,
            /* [out] */ __RPC__deref_out_opt IEnumCodePaths2 **ppEnum,
            /* [out] */ __RPC__deref_out_opt IDebugCodeContext2 **ppSafety);
        
        HRESULT ( STDMETHODCALLTYPE *WriteDump )( 
            IDebugProgram2 * This,
            /* [in] */ DUMPTYPE DumpType,
            /* [in] */ __RPC__in LPCOLESTR pszDumpUrl);
        
        END_INTERFACE
    } IDebugProgram2Vtbl;

    interface IDebugProgram2
    {
        CONST_VTBL struct IDebugProgram2Vtbl *lpVtbl;
    };

    

#ifdef COBJMACROS


#define IDebugProgram2_QueryInterface(This,riid,ppvObject)	\
    ( (This)->lpVtbl -> QueryInterface(This,riid,ppvObject) ) 

#define IDebugProgram2_AddRef(This)	\
    ( (This)->lpVtbl -> AddRef(This) ) 

#define IDebugProgram2_Release(This)	\
    ( (This)->lpVtbl -> Release(This) ) 


#define IDebugProgram2_EnumThreads(This,ppEnum)	\
    ( (This)->lpVtbl -> EnumThreads(This,ppEnum) ) 

#define IDebugProgram2_GetName(This,pbstrName)	\
    ( (This)->lpVtbl -> GetName(This,pbstrName) ) 

#define IDebugProgram2_GetProcess(This,ppProcess)	\
    ( (This)->lpVtbl -> GetProcess(This,ppProcess) ) 

#define IDebugProgram2_Terminate(This)	\
    ( (This)->lpVtbl -> Terminate(This) ) 

#define IDebugProgram2_Attach(This,pCallback)	\
    ( (This)->lpVtbl -> Attach(This,pCallback) ) 

#define IDebugProgram2_CanDetach(This)	\
    ( (This)->lpVtbl -> CanDetach(This) ) 

#define IDebugProgram2_Detach(This)	\
    ( (This)->lpVtbl -> Detach(This) ) 

#define IDebugProgram2_GetProgramId(This,pguidProgramId)	\
    ( (This)->lpVtbl -> GetProgramId(This,pguidProgramId) ) 

#define IDebugProgram2_GetDebugProperty(This,ppProperty)	\
    ( (This)->lpVtbl -> GetDebugProperty(This,ppProperty) ) 

#define IDebugProgram2_Execute(This)	\
    ( (This)->lpVtbl -> Execute(This) ) 

#define IDebugProgram2_Continue(This,pThread)	\
    ( (This)->lpVtbl -> Continue(This,pThread) ) 

#define IDebugProgram2_Step(This,pThread,sk,step)	\
    ( (This)->lpVtbl -> Step(This,pThread,sk,step) ) 

#define IDebugProgram2_CauseBreak(This)	\
    ( (This)->lpVtbl -> CauseBreak(This) ) 

#define IDebugProgram2_GetEngineInfo(This,pbstrEngine,pguidEngine)	\
    ( (This)->lpVtbl -> GetEngineInfo(This,pbstrEngine,pguidEngine) ) 

#define IDebugProgram2_EnumCodeContexts(This,pDocPos,ppEnum)	\
    ( (This)->lpVtbl -> EnumCodeContexts(This,pDocPos,ppEnum) ) 

#define IDebugProgram2_GetMemoryBytes(This,ppMemoryBytes)	\
    ( (This)->lpVtbl -> GetMemoryBytes(This,ppMemoryBytes) ) 

#define IDebugProgram2_GetDisassemblyStream(This,dwScope,pCodeContext,ppDisassemblyStream)	\
    ( (This)->lpVtbl -> GetDisassemblyStream(This,dwScope,pCodeContext,ppDisassemblyStream) ) 

#define IDebugProgram2_EnumModules(This,ppEnum)	\
    ( (This)->lpVtbl -> EnumModules(This,ppEnum) ) 

#define IDebugProgram2_GetENCUpdate(This,ppUpdate)	\
    ( (This)->lpVtbl -> GetENCUpdate(This,ppUpdate) ) 

#define IDebugProgram2_EnumCodePaths(This,pszHint,pStart,pFrame,fSource,ppEnum,ppSafety)	\
    ( (This)->lpVtbl -> EnumCodePaths(This,pszHint,pStart,pFrame,fSource,ppEnum,ppSafety) ) 

#define IDebugProgram2_WriteDump(This,DumpType,pszDumpUrl)	\
    ( (This)->lpVtbl -> WriteDump(This,DumpType,pszDumpUrl) ) 

#endif /* COBJMACROS */


#endif 	/* C style interface */




#endif 	/* __IDebugProgram2_INTERFACE_DEFINED__ */


#ifndef __IDebugProgram3_INTERFACE_DEFINED__
#define __IDebugProgram3_INTERFACE_DEFINED__

/* interface IDebugProgram3 */
/* [unique][uuid][object] */ 


EXTERN_C const IID IID_IDebugProgram3;

#if defined(__cplusplus) && !defined(CINTERFACE)
    
    MIDL_INTERFACE("7CF3EC7F-AC62-4cd6-BB30-39A464CB52CB")
    IDebugProgram3 : public IDebugProgram2
    {
    public:
        virtual HRESULT STDMETHODCALLTYPE ExecuteOnThread( 
            /* [in] */ __RPC__in_opt IDebugThread2 *pThread) = 0;
        
    };
    
#else 	/* C style interface */

    typedef struct IDebugProgram3Vtbl
    {
        BEGIN_INTERFACE
        
        HRESULT ( STDMETHODCALLTYPE *QueryInterface )( 
            IDebugProgram3 * This,
            /* [in] */ __RPC__in REFIID riid,
            /* [iid_is][out] */ 
            __RPC__deref_out  void **ppvObject);
        
        ULONG ( STDMETHODCALLTYPE *AddRef )( 
            IDebugProgram3 * This);
        
        ULONG ( STDMETHODCALLTYPE *Release )( 
            IDebugProgram3 * This);
        
        HRESULT ( STDMETHODCALLTYPE *EnumThreads )( 
            IDebugProgram3 * This,
            /* [out] */ __RPC__deref_out_opt IEnumDebugThreads2 **ppEnum);
        
        HRESULT ( STDMETHODCALLTYPE *GetName )( 
            IDebugProgram3 * This,
            /* [out] */ __RPC__deref_out_opt BSTR *pbstrName);
        
        HRESULT ( STDMETHODCALLTYPE *GetProcess )( 
            IDebugProgram3 * This,
            /* [out] */ __RPC__deref_out_opt IDebugProcess2 **ppProcess);
        
        HRESULT ( STDMETHODCALLTYPE *Terminate )( 
            IDebugProgram3 * This);
        
        HRESULT ( STDMETHODCALLTYPE *Attach )( 
            IDebugProgram3 * This,
            /* [in] */ __RPC__in_opt IDebugEventCallback2 *pCallback);
        
        HRESULT ( STDMETHODCALLTYPE *CanDetach )( 
            IDebugProgram3 * This);
        
        HRESULT ( STDMETHODCALLTYPE *Detach )( 
            IDebugProgram3 * This);
        
        HRESULT ( STDMETHODCALLTYPE *GetProgramId )( 
            IDebugProgram3 * This,
            /* [out] */ __RPC__out GUID *pguidProgramId);
        
        HRESULT ( STDMETHODCALLTYPE *GetDebugProperty )( 
            IDebugProgram3 * This,
            /* [out] */ __RPC__deref_out_opt IDebugProperty2 **ppProperty);
        
        HRESULT ( STDMETHODCALLTYPE *Execute )( 
            IDebugProgram3 * This);
        
        HRESULT ( STDMETHODCALLTYPE *Continue )( 
            IDebugProgram3 * This,
            /* [in] */ __RPC__in_opt IDebugThread2 *pThread);
        
        HRESULT ( STDMETHODCALLTYPE *Step )( 
            IDebugProgram3 * This,
            /* [in] */ __RPC__in_opt IDebugThread2 *pThread,
            /* [in] */ STEPKIND sk,
            /* [in] */ STEPUNIT step);
        
        HRESULT ( STDMETHODCALLTYPE *CauseBreak )( 
            IDebugProgram3 * This);
        
        HRESULT ( STDMETHODCALLTYPE *GetEngineInfo )( 
            IDebugProgram3 * This,
            /* [out] */ __RPC__deref_out_opt BSTR *pbstrEngine,
            /* [out] */ __RPC__out GUID *pguidEngine);
        
        HRESULT ( STDMETHODCALLTYPE *EnumCodeContexts )( 
            IDebugProgram3 * This,
            /* [in] */ __RPC__in_opt IDebugDocumentPosition2 *pDocPos,
            /* [out] */ __RPC__deref_out_opt IEnumDebugCodeContexts2 **ppEnum);
        
        HRESULT ( STDMETHODCALLTYPE *GetMemoryBytes )( 
            IDebugProgram3 * This,
            /* [out] */ __RPC__deref_out_opt IDebugMemoryBytes2 **ppMemoryBytes);
        
        HRESULT ( STDMETHODCALLTYPE *GetDisassemblyStream )( 
            IDebugProgram3 * This,
            /* [in] */ DISASSEMBLY_STREAM_SCOPE dwScope,
            /* [in] */ __RPC__in_opt IDebugCodeContext2 *pCodeContext,
            /* [out] */ __RPC__deref_out_opt IDebugDisassemblyStream2 **ppDisassemblyStream);
        
        HRESULT ( STDMETHODCALLTYPE *EnumModules )( 
            IDebugProgram3 * This,
            /* [out] */ __RPC__deref_out_opt IEnumDebugModules2 **ppEnum);
        
        HRESULT ( STDMETHODCALLTYPE *GetENCUpdate )( 
            IDebugProgram3 * This,
            /* [out] */ __RPC__deref_out_opt IDebugENCUpdate **ppUpdate);
        
        HRESULT ( STDMETHODCALLTYPE *EnumCodePaths )( 
            IDebugProgram3 * This,
            /* [full][in] */ __RPC__in_opt LPCOLESTR pszHint,
            /* [in] */ __RPC__in_opt IDebugCodeContext2 *pStart,
            /* [in] */ __RPC__in_opt IDebugStackFrame2 *pFrame,
            /* [in] */ BOOL fSource,
            /* [out] */ __RPC__deref_out_opt IEnumCodePaths2 **ppEnum,
            /* [out] */ __RPC__deref_out_opt IDebugCodeContext2 **ppSafety);
        
        HRESULT ( STDMETHODCALLTYPE *WriteDump )( 
            IDebugProgram3 * This,
            /* [in] */ DUMPTYPE DumpType,
            /* [in] */ __RPC__in LPCOLESTR pszDumpUrl);
        
        HRESULT ( STDMETHODCALLTYPE *ExecuteOnThread )( 
            IDebugProgram3 * This,
            /* [in] */ __RPC__in_opt IDebugThread2 *pThread);
        
        END_INTERFACE
    } IDebugProgram3Vtbl;

    interface IDebugProgram3
    {
        CONST_VTBL struct IDebugProgram3Vtbl *lpVtbl;
    };

    

#ifdef COBJMACROS


#define IDebugProgram3_QueryInterface(This,riid,ppvObject)	\
    ( (This)->lpVtbl -> QueryInterface(This,riid,ppvObject) ) 

#define IDebugProgram3_AddRef(This)	\
    ( (This)->lpVtbl -> AddRef(This) ) 

#define IDebugProgram3_Release(This)	\
    ( (This)->lpVtbl -> Release(This) ) 


#define IDebugProgram3_EnumThreads(This,ppEnum)	\
    ( (This)->lpVtbl -> EnumThreads(This,ppEnum) ) 

#define IDebugProgram3_GetName(This,pbstrName)	\
    ( (This)->lpVtbl -> GetName(This,pbstrName) ) 

#define IDebugProgram3_GetProcess(This,ppProcess)	\
    ( (This)->lpVtbl -> GetProcess(This,ppProcess) ) 

#define IDebugProgram3_Terminate(This)	\
    ( (This)->lpVtbl -> Terminate(This) ) 

#define IDebugProgram3_Attach(This,pCallback)	\
    ( (This)->lpVtbl -> Attach(This,pCallback) ) 

#define IDebugProgram3_CanDetach(This)	\
    ( (This)->lpVtbl -> CanDetach(This) ) 

#define IDebugProgram3_Detach(This)	\
    ( (This)->lpVtbl -> Detach(This) ) 

#define IDebugProgram3_GetProgramId(This,pguidProgramId)	\
    ( (This)->lpVtbl -> GetProgramId(This,pguidProgramId) ) 

#define IDebugProgram3_GetDebugProperty(This,ppProperty)	\
    ( (This)->lpVtbl -> GetDebugProperty(This,ppProperty) ) 

#define IDebugProgram3_Execute(This)	\
    ( (This)->lpVtbl -> Execute(This) ) 

#define IDebugProgram3_Continue(This,pThread)	\
    ( (This)->lpVtbl -> Continue(This,pThread) ) 

#define IDebugProgram3_Step(This,pThread,sk,step)	\
    ( (This)->lpVtbl -> Step(This,pThread,sk,step) ) 

#define IDebugProgram3_CauseBreak(This)	\
    ( (This)->lpVtbl -> CauseBreak(This) ) 

#define IDebugProgram3_GetEngineInfo(This,pbstrEngine,pguidEngine)	\
    ( (This)->lpVtbl -> GetEngineInfo(This,pbstrEngine,pguidEngine) ) 

#define IDebugProgram3_EnumCodeContexts(This,pDocPos,ppEnum)	\
    ( (This)->lpVtbl -> EnumCodeContexts(This,pDocPos,ppEnum) ) 

#define IDebugProgram3_GetMemoryBytes(This,ppMemoryBytes)	\
    ( (This)->lpVtbl -> GetMemoryBytes(This,ppMemoryBytes) ) 

#define IDebugProgram3_GetDisassemblyStream(This,dwScope,pCodeContext,ppDisassemblyStream)	\
    ( (This)->lpVtbl -> GetDisassemblyStream(This,dwScope,pCodeContext,ppDisassemblyStream) ) 

#define IDebugProgram3_EnumModules(This,ppEnum)	\
    ( (This)->lpVtbl -> EnumModules(This,ppEnum) ) 

#define IDebugProgram3_GetENCUpdate(This,ppUpdate)	\
    ( (This)->lpVtbl -> GetENCUpdate(This,ppUpdate) ) 

#define IDebugProgram3_EnumCodePaths(This,pszHint,pStart,pFrame,fSource,ppEnum,ppSafety)	\
    ( (This)->lpVtbl -> EnumCodePaths(This,pszHint,pStart,pFrame,fSource,ppEnum,ppSafety) ) 

#define IDebugProgram3_WriteDump(This,DumpType,pszDumpUrl)	\
    ( (This)->lpVtbl -> WriteDump(This,DumpType,pszDumpUrl) ) 


#define IDebugProgram3_ExecuteOnThread(This,pThread)	\
    ( (This)->lpVtbl -> ExecuteOnThread(This,pThread) ) 

#endif /* COBJMACROS */


#endif 	/* C style interface */




#endif 	/* __IDebugProgram3_INTERFACE_DEFINED__ */


#ifndef __IDebugEngineProgram2_INTERFACE_DEFINED__
#define __IDebugEngineProgram2_INTERFACE_DEFINED__

/* interface IDebugEngineProgram2 */
/* [unique][uuid][object] */ 


enum enum_WATCHFOREVAL
    {	WATCHFOREVAL_LEAF_PROGRAM	= 0x10000000
    } ;

EXTERN_C const IID IID_IDebugEngineProgram2;

#if defined(__cplusplus) && !defined(CINTERFACE)
    
    MIDL_INTERFACE("7ce3e768-654d-4ba7-8d95-cdaac642b141")
    IDebugEngineProgram2 : public IUnknown
    {
    public:
        virtual HRESULT STDMETHODCALLTYPE Stop( void) = 0;
        
        virtual HRESULT STDMETHODCALLTYPE WatchForThreadStep( 
            /* [in] */ __RPC__in_opt IDebugProgram2 *pOriginatingProgram,
            /* [in] */ DWORD dwTid,
            /* [in] */ BOOL fWatch,
            /* [in] */ DWORD dwFrame) = 0;
        
        virtual HRESULT STDMETHODCALLTYPE WatchForExpressionEvaluationOnThread( 
            /* [in] */ __RPC__in_opt IDebugProgram2 *pOriginatingProgram,
            /* [in] */ DWORD dwTid,
            /* [in] */ DWORD dwEvalFlags,
            /* [in] */ __RPC__in_opt IDebugEventCallback2 *pExprCallback,
            /* [in] */ BOOL fWatch) = 0;
        
    };
    
#else 	/* C style interface */

    typedef struct IDebugEngineProgram2Vtbl
    {
        BEGIN_INTERFACE
        
        HRESULT ( STDMETHODCALLTYPE *QueryInterface )( 
            IDebugEngineProgram2 * This,
            /* [in] */ __RPC__in REFIID riid,
            /* [iid_is][out] */ 
            __RPC__deref_out  void **ppvObject);
        
        ULONG ( STDMETHODCALLTYPE *AddRef )( 
            IDebugEngineProgram2 * This);
        
        ULONG ( STDMETHODCALLTYPE *Release )( 
            IDebugEngineProgram2 * This);
        
        HRESULT ( STDMETHODCALLTYPE *Stop )( 
            IDebugEngineProgram2 * This);
        
        HRESULT ( STDMETHODCALLTYPE *WatchForThreadStep )( 
            IDebugEngineProgram2 * This,
            /* [in] */ __RPC__in_opt IDebugProgram2 *pOriginatingProgram,
            /* [in] */ DWORD dwTid,
            /* [in] */ BOOL fWatch,
            /* [in] */ DWORD dwFrame);
        
        HRESULT ( STDMETHODCALLTYPE *WatchForExpressionEvaluationOnThread )( 
            IDebugEngineProgram2 * This,
            /* [in] */ __RPC__in_opt IDebugProgram2 *pOriginatingProgram,
            /* [in] */ DWORD dwTid,
            /* [in] */ DWORD dwEvalFlags,
            /* [in] */ __RPC__in_opt IDebugEventCallback2 *pExprCallback,
            /* [in] */ BOOL fWatch);
        
        END_INTERFACE
    } IDebugEngineProgram2Vtbl;

    interface IDebugEngineProgram2
    {
        CONST_VTBL struct IDebugEngineProgram2Vtbl *lpVtbl;
    };

    

#ifdef COBJMACROS


#define IDebugEngineProgram2_QueryInterface(This,riid,ppvObject)	\
    ( (This)->lpVtbl -> QueryInterface(This,riid,ppvObject) ) 

#define IDebugEngineProgram2_AddRef(This)	\
    ( (This)->lpVtbl -> AddRef(This) ) 

#define IDebugEngineProgram2_Release(This)	\
    ( (This)->lpVtbl -> Release(This) ) 


#define IDebugEngineProgram2_Stop(This)	\
    ( (This)->lpVtbl -> Stop(This) ) 

#define IDebugEngineProgram2_WatchForThreadStep(This,pOriginatingProgram,dwTid,fWatch,dwFrame)	\
    ( (This)->lpVtbl -> WatchForThreadStep(This,pOriginatingProgram,dwTid,fWatch,dwFrame) ) 

#define IDebugEngineProgram2_WatchForExpressionEvaluationOnThread(This,pOriginatingProgram,dwTid,dwEvalFlags,pExprCallback,fWatch)	\
    ( (This)->lpVtbl -> WatchForExpressionEvaluationOnThread(This,pOriginatingProgram,dwTid,dwEvalFlags,pExprCallback,fWatch) ) 

#endif /* COBJMACROS */


#endif 	/* C style interface */




#endif 	/* __IDebugEngineProgram2_INTERFACE_DEFINED__ */


#ifndef __IDebugProgramHost2_INTERFACE_DEFINED__
#define __IDebugProgramHost2_INTERFACE_DEFINED__

/* interface IDebugProgramHost2 */
/* [unique][uuid][object] */ 


EXTERN_C const IID IID_IDebugProgramHost2;

#if defined(__cplusplus) && !defined(CINTERFACE)
    
    MIDL_INTERFACE("c99d588f-778c-44fe-8b2e-40124a738891")
    IDebugProgramHost2 : public IUnknown
    {
    public:
        virtual HRESULT STDMETHODCALLTYPE GetHostName( 
            /* [in] */ DWORD dwType,
            /* [out] */ __RPC__deref_out_opt BSTR *pbstrHostName) = 0;
        
        virtual HRESULT STDMETHODCALLTYPE GetHostId( 
            /* [out] */ __RPC__out AD_PROCESS_ID *pProcessId) = 0;
        
        virtual HRESULT STDMETHODCALLTYPE GetHostMachineName( 
            /* [out] */ __RPC__deref_out_opt BSTR *pbstrHostMachineName) = 0;
        
    };
    
#else 	/* C style interface */

    typedef struct IDebugProgramHost2Vtbl
    {
        BEGIN_INTERFACE
        
        HRESULT ( STDMETHODCALLTYPE *QueryInterface )( 
            IDebugProgramHost2 * This,
            /* [in] */ __RPC__in REFIID riid,
            /* [iid_is][out] */ 
            __RPC__deref_out  void **ppvObject);
        
        ULONG ( STDMETHODCALLTYPE *AddRef )( 
            IDebugProgramHost2 * This);
        
        ULONG ( STDMETHODCALLTYPE *Release )( 
            IDebugProgramHost2 * This);
        
        HRESULT ( STDMETHODCALLTYPE *GetHostName )( 
            IDebugProgramHost2 * This,
            /* [in] */ DWORD dwType,
            /* [out] */ __RPC__deref_out_opt BSTR *pbstrHostName);
        
        HRESULT ( STDMETHODCALLTYPE *GetHostId )( 
            IDebugProgramHost2 * This,
            /* [out] */ __RPC__out AD_PROCESS_ID *pProcessId);
        
        HRESULT ( STDMETHODCALLTYPE *GetHostMachineName )( 
            IDebugProgramHost2 * This,
            /* [out] */ __RPC__deref_out_opt BSTR *pbstrHostMachineName);
        
        END_INTERFACE
    } IDebugProgramHost2Vtbl;

    interface IDebugProgramHost2
    {
        CONST_VTBL struct IDebugProgramHost2Vtbl *lpVtbl;
    };

    

#ifdef COBJMACROS


#define IDebugProgramHost2_QueryInterface(This,riid,ppvObject)	\
    ( (This)->lpVtbl -> QueryInterface(This,riid,ppvObject) ) 

#define IDebugProgramHost2_AddRef(This)	\
    ( (This)->lpVtbl -> AddRef(This) ) 

#define IDebugProgramHost2_Release(This)	\
    ( (This)->lpVtbl -> Release(This) ) 


#define IDebugProgramHost2_GetHostName(This,dwType,pbstrHostName)	\
    ( (This)->lpVtbl -> GetHostName(This,dwType,pbstrHostName) ) 

#define IDebugProgramHost2_GetHostId(This,pProcessId)	\
    ( (This)->lpVtbl -> GetHostId(This,pProcessId) ) 

#define IDebugProgramHost2_GetHostMachineName(This,pbstrHostMachineName)	\
    ( (This)->lpVtbl -> GetHostMachineName(This,pbstrHostMachineName) ) 

#endif /* COBJMACROS */


#endif 	/* C style interface */




#endif 	/* __IDebugProgramHost2_INTERFACE_DEFINED__ */


#ifndef __IDebugProgramNode2_INTERFACE_DEFINED__
#define __IDebugProgramNode2_INTERFACE_DEFINED__

/* interface IDebugProgramNode2 */
/* [unique][uuid][object] */ 


enum enum_GETHOSTNAME_TYPE
    {	GHN_FRIENDLY_NAME	= 0,
	GHN_FILE_NAME	= ( GHN_FRIENDLY_NAME + 1 ) 
    } ;
typedef DWORD GETHOSTNAME_TYPE;


EXTERN_C const IID IID_IDebugProgramNode2;

#if defined(__cplusplus) && !defined(CINTERFACE)
    
    MIDL_INTERFACE("426e255c-f1ce-4d02-a931-f9a254bf7f0f")
    IDebugProgramNode2 : public IUnknown
    {
    public:
        virtual HRESULT STDMETHODCALLTYPE GetProgramName( 
            /* [out] */ __RPC__deref_out_opt BSTR *pbstrProgramName) = 0;
        
        virtual HRESULT STDMETHODCALLTYPE GetHostName( 
            /* [in] */ GETHOSTNAME_TYPE dwHostNameType,
            /* [out] */ __RPC__deref_out_opt BSTR *pbstrHostName) = 0;
        
        virtual HRESULT STDMETHODCALLTYPE GetHostPid( 
            /* [out] */ __RPC__out AD_PROCESS_ID *pHostProcessId) = 0;
        
        virtual HRESULT STDMETHODCALLTYPE GetHostMachineName_V7( 
            /* [out] */ __RPC__deref_out_opt BSTR *pbstrHostMachineName) = 0;
        
        virtual HRESULT STDMETHODCALLTYPE Attach_V7( 
            /* [in] */ __RPC__in_opt IDebugProgram2 *pMDMProgram,
            /* [in] */ __RPC__in_opt IDebugEventCallback2 *pCallback,
            /* [in] */ DWORD dwReason) = 0;
        
        virtual HRESULT STDMETHODCALLTYPE GetEngineInfo( 
            /* [out] */ __RPC__deref_out_opt BSTR *pbstrEngine,
            /* [out] */ __RPC__out GUID *pguidEngine) = 0;
        
        virtual HRESULT STDMETHODCALLTYPE DetachDebugger_V7( void) = 0;
        
    };
    
#else 	/* C style interface */

    typedef struct IDebugProgramNode2Vtbl
    {
        BEGIN_INTERFACE
        
        HRESULT ( STDMETHODCALLTYPE *QueryInterface )( 
            IDebugProgramNode2 * This,
            /* [in] */ __RPC__in REFIID riid,
            /* [iid_is][out] */ 
            __RPC__deref_out  void **ppvObject);
        
        ULONG ( STDMETHODCALLTYPE *AddRef )( 
            IDebugProgramNode2 * This);
        
        ULONG ( STDMETHODCALLTYPE *Release )( 
            IDebugProgramNode2 * This);
        
        HRESULT ( STDMETHODCALLTYPE *GetProgramName )( 
            IDebugProgramNode2 * This,
            /* [out] */ __RPC__deref_out_opt BSTR *pbstrProgramName);
        
        HRESULT ( STDMETHODCALLTYPE *GetHostName )( 
            IDebugProgramNode2 * This,
            /* [in] */ GETHOSTNAME_TYPE dwHostNameType,
            /* [out] */ __RPC__deref_out_opt BSTR *pbstrHostName);
        
        HRESULT ( STDMETHODCALLTYPE *GetHostPid )( 
            IDebugProgramNode2 * This,
            /* [out] */ __RPC__out AD_PROCESS_ID *pHostProcessId);
        
        HRESULT ( STDMETHODCALLTYPE *GetHostMachineName_V7 )( 
            IDebugProgramNode2 * This,
            /* [out] */ __RPC__deref_out_opt BSTR *pbstrHostMachineName);
        
        HRESULT ( STDMETHODCALLTYPE *Attach_V7 )( 
            IDebugProgramNode2 * This,
            /* [in] */ __RPC__in_opt IDebugProgram2 *pMDMProgram,
            /* [in] */ __RPC__in_opt IDebugEventCallback2 *pCallback,
            /* [in] */ DWORD dwReason);
        
        HRESULT ( STDMETHODCALLTYPE *GetEngineInfo )( 
            IDebugProgramNode2 * This,
            /* [out] */ __RPC__deref_out_opt BSTR *pbstrEngine,
            /* [out] */ __RPC__out GUID *pguidEngine);
        
        HRESULT ( STDMETHODCALLTYPE *DetachDebugger_V7 )( 
            IDebugProgramNode2 * This);
        
        END_INTERFACE
    } IDebugProgramNode2Vtbl;

    interface IDebugProgramNode2
    {
        CONST_VTBL struct IDebugProgramNode2Vtbl *lpVtbl;
    };

    

#ifdef COBJMACROS


#define IDebugProgramNode2_QueryInterface(This,riid,ppvObject)	\
    ( (This)->lpVtbl -> QueryInterface(This,riid,ppvObject) ) 

#define IDebugProgramNode2_AddRef(This)	\
    ( (This)->lpVtbl -> AddRef(This) ) 

#define IDebugProgramNode2_Release(This)	\
    ( (This)->lpVtbl -> Release(This) ) 


#define IDebugProgramNode2_GetProgramName(This,pbstrProgramName)	\
    ( (This)->lpVtbl -> GetProgramName(This,pbstrProgramName) ) 

#define IDebugProgramNode2_GetHostName(This,dwHostNameType,pbstrHostName)	\
    ( (This)->lpVtbl -> GetHostName(This,dwHostNameType,pbstrHostName) ) 

#define IDebugProgramNode2_GetHostPid(This,pHostProcessId)	\
    ( (This)->lpVtbl -> GetHostPid(This,pHostProcessId) ) 

#define IDebugProgramNode2_GetHostMachineName_V7(This,pbstrHostMachineName)	\
    ( (This)->lpVtbl -> GetHostMachineName_V7(This,pbstrHostMachineName) ) 

#define IDebugProgramNode2_Attach_V7(This,pMDMProgram,pCallback,dwReason)	\
    ( (This)->lpVtbl -> Attach_V7(This,pMDMProgram,pCallback,dwReason) ) 

#define IDebugProgramNode2_GetEngineInfo(This,pbstrEngine,pguidEngine)	\
    ( (This)->lpVtbl -> GetEngineInfo(This,pbstrEngine,pguidEngine) ) 

#define IDebugProgramNode2_DetachDebugger_V7(This)	\
    ( (This)->lpVtbl -> DetachDebugger_V7(This) ) 

#endif /* COBJMACROS */


#endif 	/* C style interface */




#endif 	/* __IDebugProgramNode2_INTERFACE_DEFINED__ */


#ifndef __IDebugProgramNodeAttach2_INTERFACE_DEFINED__
#define __IDebugProgramNodeAttach2_INTERFACE_DEFINED__

/* interface IDebugProgramNodeAttach2 */
/* [unique][uuid][object] */ 


EXTERN_C const IID IID_IDebugProgramNodeAttach2;

#if defined(__cplusplus) && !defined(CINTERFACE)
    
    MIDL_INTERFACE("73faa608-5f87-4d2b-9551-8440b1cbf54c")
    IDebugProgramNodeAttach2 : public IUnknown
    {
    public:
        virtual HRESULT STDMETHODCALLTYPE OnAttach( 
            /* [in] */ __RPC__in REFGUID guidProgramId) = 0;
        
    };
    
#else 	/* C style interface */

    typedef struct IDebugProgramNodeAttach2Vtbl
    {
        BEGIN_INTERFACE
        
        HRESULT ( STDMETHODCALLTYPE *QueryInterface )( 
            IDebugProgramNodeAttach2 * This,
            /* [in] */ __RPC__in REFIID riid,
            /* [iid_is][out] */ 
            __RPC__deref_out  void **ppvObject);
        
        ULONG ( STDMETHODCALLTYPE *AddRef )( 
            IDebugProgramNodeAttach2 * This);
        
        ULONG ( STDMETHODCALLTYPE *Release )( 
            IDebugProgramNodeAttach2 * This);
        
        HRESULT ( STDMETHODCALLTYPE *OnAttach )( 
            IDebugProgramNodeAttach2 * This,
            /* [in] */ __RPC__in REFGUID guidProgramId);
        
        END_INTERFACE
    } IDebugProgramNodeAttach2Vtbl;

    interface IDebugProgramNodeAttach2
    {
        CONST_VTBL struct IDebugProgramNodeAttach2Vtbl *lpVtbl;
    };

    

#ifdef COBJMACROS


#define IDebugProgramNodeAttach2_QueryInterface(This,riid,ppvObject)	\
    ( (This)->lpVtbl -> QueryInterface(This,riid,ppvObject) ) 

#define IDebugProgramNodeAttach2_AddRef(This)	\
    ( (This)->lpVtbl -> AddRef(This) ) 

#define IDebugProgramNodeAttach2_Release(This)	\
    ( (This)->lpVtbl -> Release(This) ) 


#define IDebugProgramNodeAttach2_OnAttach(This,guidProgramId)	\
    ( (This)->lpVtbl -> OnAttach(This,guidProgramId) ) 

#endif /* COBJMACROS */


#endif 	/* C style interface */




#endif 	/* __IDebugProgramNodeAttach2_INTERFACE_DEFINED__ */


#ifndef __IDebugProgramEngines2_INTERFACE_DEFINED__
#define __IDebugProgramEngines2_INTERFACE_DEFINED__

/* interface IDebugProgramEngines2 */
/* [unique][uuid][object] */ 


EXTERN_C const IID IID_IDebugProgramEngines2;

#if defined(__cplusplus) && !defined(CINTERFACE)
    
    MIDL_INTERFACE("fda24a6b-b142-447d-bbbc-8654a3d84f80")
    IDebugProgramEngines2 : public IUnknown
    {
    public:
        virtual HRESULT STDMETHODCALLTYPE EnumPossibleEngines( 
            /* [in] */ DWORD celtBuffer,
            /* [length_is][size_is][full][out][in] */ __RPC__inout_ecount_part_opt(celtBuffer, *pceltEngines) GUID *rgguidEngines,
            /* [out][in] */ __RPC__inout DWORD *pceltEngines) = 0;
        
        virtual HRESULT STDMETHODCALLTYPE SetEngine( 
            /* [in] */ __RPC__in REFGUID guidEngine) = 0;
        
    };
    
#else 	/* C style interface */

    typedef struct IDebugProgramEngines2Vtbl
    {
        BEGIN_INTERFACE
        
        HRESULT ( STDMETHODCALLTYPE *QueryInterface )( 
            IDebugProgramEngines2 * This,
            /* [in] */ __RPC__in REFIID riid,
            /* [iid_is][out] */ 
            __RPC__deref_out  void **ppvObject);
        
        ULONG ( STDMETHODCALLTYPE *AddRef )( 
            IDebugProgramEngines2 * This);
        
        ULONG ( STDMETHODCALLTYPE *Release )( 
            IDebugProgramEngines2 * This);
        
        HRESULT ( STDMETHODCALLTYPE *EnumPossibleEngines )( 
            IDebugProgramEngines2 * This,
            /* [in] */ DWORD celtBuffer,
            /* [length_is][size_is][full][out][in] */ __RPC__inout_ecount_part_opt(celtBuffer, *pceltEngines) GUID *rgguidEngines,
            /* [out][in] */ __RPC__inout DWORD *pceltEngines);
        
        HRESULT ( STDMETHODCALLTYPE *SetEngine )( 
            IDebugProgramEngines2 * This,
            /* [in] */ __RPC__in REFGUID guidEngine);
        
        END_INTERFACE
    } IDebugProgramEngines2Vtbl;

    interface IDebugProgramEngines2
    {
        CONST_VTBL struct IDebugProgramEngines2Vtbl *lpVtbl;
    };

    

#ifdef COBJMACROS


#define IDebugProgramEngines2_QueryInterface(This,riid,ppvObject)	\
    ( (This)->lpVtbl -> QueryInterface(This,riid,ppvObject) ) 

#define IDebugProgramEngines2_AddRef(This)	\
    ( (This)->lpVtbl -> AddRef(This) ) 

#define IDebugProgramEngines2_Release(This)	\
    ( (This)->lpVtbl -> Release(This) ) 


#define IDebugProgramEngines2_EnumPossibleEngines(This,celtBuffer,rgguidEngines,pceltEngines)	\
    ( (This)->lpVtbl -> EnumPossibleEngines(This,celtBuffer,rgguidEngines,pceltEngines) ) 

#define IDebugProgramEngines2_SetEngine(This,guidEngine)	\
    ( (This)->lpVtbl -> SetEngine(This,guidEngine) ) 

#endif /* COBJMACROS */


#endif 	/* C style interface */




#endif 	/* __IDebugProgramEngines2_INTERFACE_DEFINED__ */


#ifndef __IDebugCOMPlusProgramNode2_INTERFACE_DEFINED__
#define __IDebugCOMPlusProgramNode2_INTERFACE_DEFINED__

/* interface IDebugCOMPlusProgramNode2 */
/* [unique][uuid][object] */ 


EXTERN_C const IID IID_IDebugCOMPlusProgramNode2;

#if defined(__cplusplus) && !defined(CINTERFACE)
    
    MIDL_INTERFACE("d6f7d3d0-506a-448f-8702-46eb2745e4fc")
    IDebugCOMPlusProgramNode2 : public IUnknown
    {
    public:
        virtual HRESULT STDMETHODCALLTYPE GetAppDomainId( 
            /* [out] */ __RPC__out ULONG32 *pul32Id) = 0;
        
    };
    
#else 	/* C style interface */

    typedef struct IDebugCOMPlusProgramNode2Vtbl
    {
        BEGIN_INTERFACE
        
        HRESULT ( STDMETHODCALLTYPE *QueryInterface )( 
            IDebugCOMPlusProgramNode2 * This,
            /* [in] */ __RPC__in REFIID riid,
            /* [iid_is][out] */ 
            __RPC__deref_out  void **ppvObject);
        
        ULONG ( STDMETHODCALLTYPE *AddRef )( 
            IDebugCOMPlusProgramNode2 * This);
        
        ULONG ( STDMETHODCALLTYPE *Release )( 
            IDebugCOMPlusProgramNode2 * This);
        
        HRESULT ( STDMETHODCALLTYPE *GetAppDomainId )( 
            IDebugCOMPlusProgramNode2 * This,
            /* [out] */ __RPC__out ULONG32 *pul32Id);
        
        END_INTERFACE
    } IDebugCOMPlusProgramNode2Vtbl;

    interface IDebugCOMPlusProgramNode2
    {
        CONST_VTBL struct IDebugCOMPlusProgramNode2Vtbl *lpVtbl;
    };

    

#ifdef COBJMACROS


#define IDebugCOMPlusProgramNode2_QueryInterface(This,riid,ppvObject)	\
    ( (This)->lpVtbl -> QueryInterface(This,riid,ppvObject) ) 

#define IDebugCOMPlusProgramNode2_AddRef(This)	\
    ( (This)->lpVtbl -> AddRef(This) ) 

#define IDebugCOMPlusProgramNode2_Release(This)	\
    ( (This)->lpVtbl -> Release(This) ) 


#define IDebugCOMPlusProgramNode2_GetAppDomainId(This,pul32Id)	\
    ( (This)->lpVtbl -> GetAppDomainId(This,pul32Id) ) 

#endif /* COBJMACROS */


#endif 	/* C style interface */




#endif 	/* __IDebugCOMPlusProgramNode2_INTERFACE_DEFINED__ */


#ifndef __IDebugSQLCLRProgramNode2_INTERFACE_DEFINED__
#define __IDebugSQLCLRProgramNode2_INTERFACE_DEFINED__

/* interface IDebugSQLCLRProgramNode2 */
/* [unique][uuid][object] */ 


EXTERN_C const IID IID_IDebugSQLCLRProgramNode2;

#if defined(__cplusplus) && !defined(CINTERFACE)
    
    MIDL_INTERFACE("F617DFCB-0045-4024-837B-7ACAD8F4D67B")
    IDebugSQLCLRProgramNode2 : public IUnknown
    {
    public:
        virtual HRESULT STDMETHODCALLTYPE GetConnectionId( 
            /* [out] */ __RPC__out DWORD *pdwId) = 0;
        
    };
    
#else 	/* C style interface */

    typedef struct IDebugSQLCLRProgramNode2Vtbl
    {
        BEGIN_INTERFACE
        
        HRESULT ( STDMETHODCALLTYPE *QueryInterface )( 
            IDebugSQLCLRProgramNode2 * This,
            /* [in] */ __RPC__in REFIID riid,
            /* [iid_is][out] */ 
            __RPC__deref_out  void **ppvObject);
        
        ULONG ( STDMETHODCALLTYPE *AddRef )( 
            IDebugSQLCLRProgramNode2 * This);
        
        ULONG ( STDMETHODCALLTYPE *Release )( 
            IDebugSQLCLRProgramNode2 * This);
        
        HRESULT ( STDMETHODCALLTYPE *GetConnectionId )( 
            IDebugSQLCLRProgramNode2 * This,
            /* [out] */ __RPC__out DWORD *pdwId);
        
        END_INTERFACE
    } IDebugSQLCLRProgramNode2Vtbl;

    interface IDebugSQLCLRProgramNode2
    {
        CONST_VTBL struct IDebugSQLCLRProgramNode2Vtbl *lpVtbl;
    };

    

#ifdef COBJMACROS


#define IDebugSQLCLRProgramNode2_QueryInterface(This,riid,ppvObject)	\
    ( (This)->lpVtbl -> QueryInterface(This,riid,ppvObject) ) 

#define IDebugSQLCLRProgramNode2_AddRef(This)	\
    ( (This)->lpVtbl -> AddRef(This) ) 

#define IDebugSQLCLRProgramNode2_Release(This)	\
    ( (This)->lpVtbl -> Release(This) ) 


#define IDebugSQLCLRProgramNode2_GetConnectionId(This,pdwId)	\
    ( (This)->lpVtbl -> GetConnectionId(This,pdwId) ) 

#endif /* COBJMACROS */


#endif 	/* C style interface */




#endif 	/* __IDebugSQLCLRProgramNode2_INTERFACE_DEFINED__ */


#ifndef __IDebugThread2_INTERFACE_DEFINED__
#define __IDebugThread2_INTERFACE_DEFINED__

/* interface IDebugThread2 */
/* [unique][uuid][object] */ 


enum enum_THREADSTATE
    {	THREADSTATE_RUNNING	= 0x1,
	THREADSTATE_STOPPED	= 0x2,
	THREADSTATE_FRESH	= 0x3,
	THREADSTATE_DEAD	= 0x4,
	THREADSTATE_FROZEN	= 0x5
    } ;
typedef DWORD THREADSTATE;


enum enum_THREADPROPERTY_FIELDS
    {	TPF_ID	= 0x1,
	TPF_SUSPENDCOUNT	= 0x2,
	TPF_STATE	= 0x4,
	TPF_PRIORITY	= 0x8,
	TPF_NAME	= 0x10,
	TPF_LOCATION	= 0x20,
	TPF_ALLFIELDS	= 0xffffffff
    } ;
typedef DWORD THREADPROPERTY_FIELDS;

typedef struct _tagTHREADPROPERTIES
    {
    THREADPROPERTY_FIELDS dwFields;
    DWORD dwThreadId;
    DWORD dwSuspendCount;
    DWORD dwThreadState;
    BSTR bstrPriority;
    BSTR bstrName;
    BSTR bstrLocation;
    } 	THREADPROPERTIES;


enum enum_FRAMEINFO_FLAGS
    {	FIF_FUNCNAME	= 0x1,
	FIF_RETURNTYPE	= 0x2,
	FIF_ARGS	= 0x4,
	FIF_LANGUAGE	= 0x8,
	FIF_MODULE	= 0x10,
	FIF_STACKRANGE	= 0x20,
	FIF_FRAME	= 0x40,
	FIF_DEBUGINFO	= 0x80,
	FIF_STALECODE	= 0x100,
	FIF_FLAGS	= 0x200,
	FIF_DEBUG_MODULEP	= 0x400,
	FIF_FUNCNAME_FORMAT	= 0x1000,
	FIF_FUNCNAME_RETURNTYPE	= 0x2000,
	FIF_FUNCNAME_ARGS	= 0x4000,
	FIF_FUNCNAME_LANGUAGE	= 0x8000,
	FIF_FUNCNAME_MODULE	= 0x10000,
	FIF_FUNCNAME_LINES	= 0x20000,
	FIF_FUNCNAME_OFFSET	= 0x40000,
	FIF_FUNCNAME_ARGS_TYPES	= 0x100000,
	FIF_FUNCNAME_ARGS_NAMES	= 0x200000,
	FIF_FUNCNAME_ARGS_VALUES	= 0x400000,
	FIF_FUNCNAME_ARGS_ALL	= 0x700000,
	FIF_ARGS_TYPES	= 0x1000000,
	FIF_ARGS_NAMES	= 0x2000000,
	FIF_ARGS_VALUES	= 0x4000000,
	FIF_ARGS_ALL	= 0x7000000,
	FIF_ARGS_NOFORMAT	= 0x8000000,
	FIF_ARGS_NO_FUNC_EVAL	= 0x10000000,
	FIF_FILTER_NON_USER_CODE	= 0x20000000,
	FIF_ARGS_NO_TOSTRING	= 0x40000000,
	FIF_DESIGN_TIME_EXPR_EVAL	= 0x80000000,
	FIF_FILTER_INCLUDE_ALL	= 0x80000
    } ;
typedef DWORD FRAMEINFO_FLAGS;


enum enum_FRAMEINFO_FLAGS_VALUES
    {	FIFV_ANNOTATEDFRAME	= 0x1,
	FIFV_NON_USER_CODE	= 0x2,
	FIFV_CANINTERCEPT_EXCEPTION	= 0x4,
	FIFV_FUNCEVALFRAME	= 0x8
    } ;
typedef DWORD FRAMEINFO_FLAGS_VALUES;

typedef struct tagFRAMEINFO
    {
    FRAMEINFO_FLAGS m_dwValidFields;
    BSTR m_bstrFuncName;
    BSTR m_bstrReturnType;
    BSTR m_bstrArgs;
    BSTR m_bstrLanguage;
    BSTR m_bstrModule;
    UINT64 m_addrMin;
    UINT64 m_addrMax;
    IDebugStackFrame2 *m_pFrame;
    IDebugModule2 *m_pModule;
    BOOL m_fHasDebugInfo;
    BOOL m_fStaleCode;
    DWORD m_dwFlags;
    } 	FRAMEINFO;


EXTERN_C const IID IID_IDebugThread2;

#if defined(__cplusplus) && !defined(CINTERFACE)
    
    MIDL_INTERFACE("d5168050-a57a-465c-bea9-974f405eba13")
    IDebugThread2 : public IUnknown
    {
    public:
        virtual HRESULT STDMETHODCALLTYPE EnumFrameInfo( 
            /* [in] */ FRAMEINFO_FLAGS dwFieldSpec,
            /* [in] */ UINT nRadix,
            /* [out] */ __RPC__deref_out_opt IEnumDebugFrameInfo2 **ppEnum) = 0;
        
        virtual HRESULT STDMETHODCALLTYPE GetName( 
            /* [out] */ __RPC__deref_out_opt BSTR *pbstrName) = 0;
        
        virtual HRESULT STDMETHODCALLTYPE SetThreadName( 
            /* [in] */ __RPC__in LPCOLESTR pszName) = 0;
        
        virtual HRESULT STDMETHODCALLTYPE GetProgram( 
            /* [out] */ __RPC__deref_out_opt IDebugProgram2 **ppProgram) = 0;
        
        virtual HRESULT STDMETHODCALLTYPE CanSetNextStatement( 
            /* [in] */ __RPC__in_opt IDebugStackFrame2 *pStackFrame,
            /* [in] */ __RPC__in_opt IDebugCodeContext2 *pCodeContext) = 0;
        
        virtual HRESULT STDMETHODCALLTYPE SetNextStatement( 
            /* [in] */ __RPC__in_opt IDebugStackFrame2 *pStackFrame,
            /* [in] */ __RPC__in_opt IDebugCodeContext2 *pCodeContext) = 0;
        
        virtual HRESULT STDMETHODCALLTYPE GetThreadId( 
            /* [out] */ __RPC__out DWORD *pdwThreadId) = 0;
        
        virtual HRESULT STDMETHODCALLTYPE Suspend( 
            /* [out] */ __RPC__out DWORD *pdwSuspendCount) = 0;
        
        virtual HRESULT STDMETHODCALLTYPE Resume( 
            /* [out] */ __RPC__out DWORD *pdwSuspendCount) = 0;
        
        virtual HRESULT STDMETHODCALLTYPE GetThreadProperties( 
            /* [in] */ THREADPROPERTY_FIELDS dwFields,
            /* [out] */ __RPC__out THREADPROPERTIES *ptp) = 0;
        
        virtual HRESULT STDMETHODCALLTYPE GetLogicalThread( 
            /* [in] */ __RPC__in_opt IDebugStackFrame2 *pStackFrame,
            /* [out] */ __RPC__deref_out_opt IDebugLogicalThread2 **ppLogicalThread) = 0;
        
    };
    
#else 	/* C style interface */

    typedef struct IDebugThread2Vtbl
    {
        BEGIN_INTERFACE
        
        HRESULT ( STDMETHODCALLTYPE *QueryInterface )( 
            IDebugThread2 * This,
            /* [in] */ __RPC__in REFIID riid,
            /* [iid_is][out] */ 
            __RPC__deref_out  void **ppvObject);
        
        ULONG ( STDMETHODCALLTYPE *AddRef )( 
            IDebugThread2 * This);
        
        ULONG ( STDMETHODCALLTYPE *Release )( 
            IDebugThread2 * This);
        
        HRESULT ( STDMETHODCALLTYPE *EnumFrameInfo )( 
            IDebugThread2 * This,
            /* [in] */ FRAMEINFO_FLAGS dwFieldSpec,
            /* [in] */ UINT nRadix,
            /* [out] */ __RPC__deref_out_opt IEnumDebugFrameInfo2 **ppEnum);
        
        HRESULT ( STDMETHODCALLTYPE *GetName )( 
            IDebugThread2 * This,
            /* [out] */ __RPC__deref_out_opt BSTR *pbstrName);
        
        HRESULT ( STDMETHODCALLTYPE *SetThreadName )( 
            IDebugThread2 * This,
            /* [in] */ __RPC__in LPCOLESTR pszName);
        
        HRESULT ( STDMETHODCALLTYPE *GetProgram )( 
            IDebugThread2 * This,
            /* [out] */ __RPC__deref_out_opt IDebugProgram2 **ppProgram);
        
        HRESULT ( STDMETHODCALLTYPE *CanSetNextStatement )( 
            IDebugThread2 * This,
            /* [in] */ __RPC__in_opt IDebugStackFrame2 *pStackFrame,
            /* [in] */ __RPC__in_opt IDebugCodeContext2 *pCodeContext);
        
        HRESULT ( STDMETHODCALLTYPE *SetNextStatement )( 
            IDebugThread2 * This,
            /* [in] */ __RPC__in_opt IDebugStackFrame2 *pStackFrame,
            /* [in] */ __RPC__in_opt IDebugCodeContext2 *pCodeContext);
        
        HRESULT ( STDMETHODCALLTYPE *GetThreadId )( 
            IDebugThread2 * This,
            /* [out] */ __RPC__out DWORD *pdwThreadId);
        
        HRESULT ( STDMETHODCALLTYPE *Suspend )( 
            IDebugThread2 * This,
            /* [out] */ __RPC__out DWORD *pdwSuspendCount);
        
        HRESULT ( STDMETHODCALLTYPE *Resume )( 
            IDebugThread2 * This,
            /* [out] */ __RPC__out DWORD *pdwSuspendCount);
        
        HRESULT ( STDMETHODCALLTYPE *GetThreadProperties )( 
            IDebugThread2 * This,
            /* [in] */ THREADPROPERTY_FIELDS dwFields,
            /* [out] */ __RPC__out THREADPROPERTIES *ptp);
        
        HRESULT ( STDMETHODCALLTYPE *GetLogicalThread )( 
            IDebugThread2 * This,
            /* [in] */ __RPC__in_opt IDebugStackFrame2 *pStackFrame,
            /* [out] */ __RPC__deref_out_opt IDebugLogicalThread2 **ppLogicalThread);
        
        END_INTERFACE
    } IDebugThread2Vtbl;

    interface IDebugThread2
    {
        CONST_VTBL struct IDebugThread2Vtbl *lpVtbl;
    };

    

#ifdef COBJMACROS


#define IDebugThread2_QueryInterface(This,riid,ppvObject)	\
    ( (This)->lpVtbl -> QueryInterface(This,riid,ppvObject) ) 

#define IDebugThread2_AddRef(This)	\
    ( (This)->lpVtbl -> AddRef(This) ) 

#define IDebugThread2_Release(This)	\
    ( (This)->lpVtbl -> Release(This) ) 


#define IDebugThread2_EnumFrameInfo(This,dwFieldSpec,nRadix,ppEnum)	\
    ( (This)->lpVtbl -> EnumFrameInfo(This,dwFieldSpec,nRadix,ppEnum) ) 

#define IDebugThread2_GetName(This,pbstrName)	\
    ( (This)->lpVtbl -> GetName(This,pbstrName) ) 

#define IDebugThread2_SetThreadName(This,pszName)	\
    ( (This)->lpVtbl -> SetThreadName(This,pszName) ) 

#define IDebugThread2_GetProgram(This,ppProgram)	\
    ( (This)->lpVtbl -> GetProgram(This,ppProgram) ) 

#define IDebugThread2_CanSetNextStatement(This,pStackFrame,pCodeContext)	\
    ( (This)->lpVtbl -> CanSetNextStatement(This,pStackFrame,pCodeContext) ) 

#define IDebugThread2_SetNextStatement(This,pStackFrame,pCodeContext)	\
    ( (This)->lpVtbl -> SetNextStatement(This,pStackFrame,pCodeContext) ) 

#define IDebugThread2_GetThreadId(This,pdwThreadId)	\
    ( (This)->lpVtbl -> GetThreadId(This,pdwThreadId) ) 

#define IDebugThread2_Suspend(This,pdwSuspendCount)	\
    ( (This)->lpVtbl -> Suspend(This,pdwSuspendCount) ) 

#define IDebugThread2_Resume(This,pdwSuspendCount)	\
    ( (This)->lpVtbl -> Resume(This,pdwSuspendCount) ) 

#define IDebugThread2_GetThreadProperties(This,dwFields,ptp)	\
    ( (This)->lpVtbl -> GetThreadProperties(This,dwFields,ptp) ) 

#define IDebugThread2_GetLogicalThread(This,pStackFrame,ppLogicalThread)	\
    ( (This)->lpVtbl -> GetLogicalThread(This,pStackFrame,ppLogicalThread) ) 

#endif /* COBJMACROS */


#endif 	/* C style interface */




#endif 	/* __IDebugThread2_INTERFACE_DEFINED__ */


#ifndef __IDebugLogicalThread2_INTERFACE_DEFINED__
#define __IDebugLogicalThread2_INTERFACE_DEFINED__

/* interface IDebugLogicalThread2 */
/* [unique][uuid][object] */ 


EXTERN_C const IID IID_IDebugLogicalThread2;

#if defined(__cplusplus) && !defined(CINTERFACE)
    
    MIDL_INTERFACE("88d2f75b-d329-4e03-9b75-201f7782d8bd")
    IDebugLogicalThread2 : public IUnknown
    {
    public:
        virtual HRESULT STDMETHODCALLTYPE EnumFrameInfo( 
            /* [in] */ FRAMEINFO_FLAGS dwFieldSpec,
            /* [in] */ UINT nRadix,
            /* [out] */ __RPC__deref_out_opt IEnumDebugFrameInfo2 **ppEnum) = 0;
        
    };
    
#else 	/* C style interface */

    typedef struct IDebugLogicalThread2Vtbl
    {
        BEGIN_INTERFACE
        
        HRESULT ( STDMETHODCALLTYPE *QueryInterface )( 
            IDebugLogicalThread2 * This,
            /* [in] */ __RPC__in REFIID riid,
            /* [iid_is][out] */ 
            __RPC__deref_out  void **ppvObject);
        
        ULONG ( STDMETHODCALLTYPE *AddRef )( 
            IDebugLogicalThread2 * This);
        
        ULONG ( STDMETHODCALLTYPE *Release )( 
            IDebugLogicalThread2 * This);
        
        HRESULT ( STDMETHODCALLTYPE *EnumFrameInfo )( 
            IDebugLogicalThread2 * This,
            /* [in] */ FRAMEINFO_FLAGS dwFieldSpec,
            /* [in] */ UINT nRadix,
            /* [out] */ __RPC__deref_out_opt IEnumDebugFrameInfo2 **ppEnum);
        
        END_INTERFACE
    } IDebugLogicalThread2Vtbl;

    interface IDebugLogicalThread2
    {
        CONST_VTBL struct IDebugLogicalThread2Vtbl *lpVtbl;
    };

    

#ifdef COBJMACROS


#define IDebugLogicalThread2_QueryInterface(This,riid,ppvObject)	\
    ( (This)->lpVtbl -> QueryInterface(This,riid,ppvObject) ) 

#define IDebugLogicalThread2_AddRef(This)	\
    ( (This)->lpVtbl -> AddRef(This) ) 

#define IDebugLogicalThread2_Release(This)	\
    ( (This)->lpVtbl -> Release(This) ) 


#define IDebugLogicalThread2_EnumFrameInfo(This,dwFieldSpec,nRadix,ppEnum)	\
    ( (This)->lpVtbl -> EnumFrameInfo(This,dwFieldSpec,nRadix,ppEnum) ) 

#endif /* COBJMACROS */


#endif 	/* C style interface */




#endif 	/* __IDebugLogicalThread2_INTERFACE_DEFINED__ */


#ifndef __IDebugThread3_INTERFACE_DEFINED__
#define __IDebugThread3_INTERFACE_DEFINED__

/* interface IDebugThread3 */
/* [unique][uuid][object] */ 


EXTERN_C const IID IID_IDebugThread3;

#if defined(__cplusplus) && !defined(CINTERFACE)
    
    MIDL_INTERFACE("43D24196-0000-467f-8C6B-9C006922D02F")
    IDebugThread3 : public IDebugThread2
    {
    public:
        virtual HRESULT STDMETHODCALLTYPE IsCurrentException( void) = 0;
        
        virtual HRESULT STDMETHODCALLTYPE CanRemapLeafFrame( void) = 0;
        
        virtual HRESULT STDMETHODCALLTYPE RemapLeafFrame( void) = 0;
        
    };
    
#else 	/* C style interface */

    typedef struct IDebugThread3Vtbl
    {
        BEGIN_INTERFACE
        
        HRESULT ( STDMETHODCALLTYPE *QueryInterface )( 
            IDebugThread3 * This,
            /* [in] */ __RPC__in REFIID riid,
            /* [iid_is][out] */ 
            __RPC__deref_out  void **ppvObject);
        
        ULONG ( STDMETHODCALLTYPE *AddRef )( 
            IDebugThread3 * This);
        
        ULONG ( STDMETHODCALLTYPE *Release )( 
            IDebugThread3 * This);
        
        HRESULT ( STDMETHODCALLTYPE *EnumFrameInfo )( 
            IDebugThread3 * This,
            /* [in] */ FRAMEINFO_FLAGS dwFieldSpec,
            /* [in] */ UINT nRadix,
            /* [out] */ __RPC__deref_out_opt IEnumDebugFrameInfo2 **ppEnum);
        
        HRESULT ( STDMETHODCALLTYPE *GetName )( 
            IDebugThread3 * This,
            /* [out] */ __RPC__deref_out_opt BSTR *pbstrName);
        
        HRESULT ( STDMETHODCALLTYPE *SetThreadName )( 
            IDebugThread3 * This,
            /* [in] */ __RPC__in LPCOLESTR pszName);
        
        HRESULT ( STDMETHODCALLTYPE *GetProgram )( 
            IDebugThread3 * This,
            /* [out] */ __RPC__deref_out_opt IDebugProgram2 **ppProgram);
        
        HRESULT ( STDMETHODCALLTYPE *CanSetNextStatement )( 
            IDebugThread3 * This,
            /* [in] */ __RPC__in_opt IDebugStackFrame2 *pStackFrame,
            /* [in] */ __RPC__in_opt IDebugCodeContext2 *pCodeContext);
        
        HRESULT ( STDMETHODCALLTYPE *SetNextStatement )( 
            IDebugThread3 * This,
            /* [in] */ __RPC__in_opt IDebugStackFrame2 *pStackFrame,
            /* [in] */ __RPC__in_opt IDebugCodeContext2 *pCodeContext);
        
        HRESULT ( STDMETHODCALLTYPE *GetThreadId )( 
            IDebugThread3 * This,
            /* [out] */ __RPC__out DWORD *pdwThreadId);
        
        HRESULT ( STDMETHODCALLTYPE *Suspend )( 
            IDebugThread3 * This,
            /* [out] */ __RPC__out DWORD *pdwSuspendCount);
        
        HRESULT ( STDMETHODCALLTYPE *Resume )( 
            IDebugThread3 * This,
            /* [out] */ __RPC__out DWORD *pdwSuspendCount);
        
        HRESULT ( STDMETHODCALLTYPE *GetThreadProperties )( 
            IDebugThread3 * This,
            /* [in] */ THREADPROPERTY_FIELDS dwFields,
            /* [out] */ __RPC__out THREADPROPERTIES *ptp);
        
        HRESULT ( STDMETHODCALLTYPE *GetLogicalThread )( 
            IDebugThread3 * This,
            /* [in] */ __RPC__in_opt IDebugStackFrame2 *pStackFrame,
            /* [out] */ __RPC__deref_out_opt IDebugLogicalThread2 **ppLogicalThread);
        
        HRESULT ( STDMETHODCALLTYPE *IsCurrentException )( 
            IDebugThread3 * This);
        
        HRESULT ( STDMETHODCALLTYPE *CanRemapLeafFrame )( 
            IDebugThread3 * This);
        
        HRESULT ( STDMETHODCALLTYPE *RemapLeafFrame )( 
            IDebugThread3 * This);
        
        END_INTERFACE
    } IDebugThread3Vtbl;

    interface IDebugThread3
    {
        CONST_VTBL struct IDebugThread3Vtbl *lpVtbl;
    };

    

#ifdef COBJMACROS


#define IDebugThread3_QueryInterface(This,riid,ppvObject)	\
    ( (This)->lpVtbl -> QueryInterface(This,riid,ppvObject) ) 

#define IDebugThread3_AddRef(This)	\
    ( (This)->lpVtbl -> AddRef(This) ) 

#define IDebugThread3_Release(This)	\
    ( (This)->lpVtbl -> Release(This) ) 


#define IDebugThread3_EnumFrameInfo(This,dwFieldSpec,nRadix,ppEnum)	\
    ( (This)->lpVtbl -> EnumFrameInfo(This,dwFieldSpec,nRadix,ppEnum) ) 

#define IDebugThread3_GetName(This,pbstrName)	\
    ( (This)->lpVtbl -> GetName(This,pbstrName) ) 

#define IDebugThread3_SetThreadName(This,pszName)	\
    ( (This)->lpVtbl -> SetThreadName(This,pszName) ) 

#define IDebugThread3_GetProgram(This,ppProgram)	\
    ( (This)->lpVtbl -> GetProgram(This,ppProgram) ) 

#define IDebugThread3_CanSetNextStatement(This,pStackFrame,pCodeContext)	\
    ( (This)->lpVtbl -> CanSetNextStatement(This,pStackFrame,pCodeContext) ) 

#define IDebugThread3_SetNextStatement(This,pStackFrame,pCodeContext)	\
    ( (This)->lpVtbl -> SetNextStatement(This,pStackFrame,pCodeContext) ) 

#define IDebugThread3_GetThreadId(This,pdwThreadId)	\
    ( (This)->lpVtbl -> GetThreadId(This,pdwThreadId) ) 

#define IDebugThread3_Suspend(This,pdwSuspendCount)	\
    ( (This)->lpVtbl -> Suspend(This,pdwSuspendCount) ) 

#define IDebugThread3_Resume(This,pdwSuspendCount)	\
    ( (This)->lpVtbl -> Resume(This,pdwSuspendCount) ) 

#define IDebugThread3_GetThreadProperties(This,dwFields,ptp)	\
    ( (This)->lpVtbl -> GetThreadProperties(This,dwFields,ptp) ) 

#define IDebugThread3_GetLogicalThread(This,pStackFrame,ppLogicalThread)	\
    ( (This)->lpVtbl -> GetLogicalThread(This,pStackFrame,ppLogicalThread) ) 


#define IDebugThread3_IsCurrentException(This)	\
    ( (This)->lpVtbl -> IsCurrentException(This) ) 

#define IDebugThread3_CanRemapLeafFrame(This)	\
    ( (This)->lpVtbl -> CanRemapLeafFrame(This) ) 

#define IDebugThread3_RemapLeafFrame(This)	\
    ( (This)->lpVtbl -> RemapLeafFrame(This) ) 

#endif /* COBJMACROS */


#endif 	/* C style interface */




#endif 	/* __IDebugThread3_INTERFACE_DEFINED__ */


/* interface __MIDL_itf_msdbg_0000_0080 */
/* [local] */ 

#define DBG_ATTRIB_NONE					0x0000000000000000
#define DBG_ATTRIB_ALL					0xffffffffffffffff
#define DBG_ATTRIB_OBJ_IS_EXPANDABLE		0x0000000000000001
#define DBG_ATTRIB_OBJ_HAS_ID            0x0000000000000002
#define DBG_ATTRIB_OBJ_CAN_HAVE_ID            0x0000000000000004
#define DBG_ATTRIB_VALUE_READONLY		0x0000000000000010
#define DBG_ATTRIB_VALUE_ERROR			0x0000000000000020
#define DBG_ATTRIB_VALUE_SIDE_EFFECT		0x0000000000000040
#define DBG_ATTRIB_OVERLOADED_CONTAINER	0x0000000000000080
#define DBG_ATTRIB_VALUE_BOOLEAN			0x0000000000000100
#define DBG_ATTRIB_VALUE_BOOLEAN_TRUE	0x0000000000000200
#define DBG_ATTRIB_VALUE_INVALID			0x0000000000000400
#define DBG_ATTRIB_VALUE_NAT				0x0000000000000800
#define DBG_ATTRIB_VALUE_AUTOEXPANDED	0x0000000000001000
#define DBG_ATTRIB_VALUE_TIMEOUT     	0x0000000000002000
#define DBG_ATTRIB_VALUE_RAW_STRING      0x0000000000004000
#define DBG_ATTRIB_VALUE_CUSTOM_VIEWER   0x0000000000008000
#define DBG_ATTRIB_ACCESS_NONE			0x0000000000010000
#define DBG_ATTRIB_ACCESS_PUBLIC			0x0000000000020000
#define DBG_ATTRIB_ACCESS_PRIVATE		0x0000000000040000
#define DBG_ATTRIB_ACCESS_PROTECTED		0x0000000000080000
#define DBG_ATTRIB_ACCESS_FINAL			0x0000000000100000
#define DBG_ATTRIB_ACCESS_ALL			0x00000000001f0000
#define DBG_ATTRIB_STORAGE_NONE			0x0000000001000000
#define DBG_ATTRIB_STORAGE_GLOBAL		0x0000000002000000
#define DBG_ATTRIB_STORAGE_STATIC		0x0000000004000000
#define DBG_ATTRIB_STORAGE_REGISTER		0x0000000008000000
#define DBG_ATTRIB_STORAGE_ALL			0x000000000f000000
#define DBG_ATTRIB_TYPE_NONE				0x0000000100000000
#define DBG_ATTRIB_TYPE_VIRTUAL			0x0000000200000000
#define DBG_ATTRIB_TYPE_CONSTANT			0x0000000400000000
#define DBG_ATTRIB_TYPE_SYNCHRONIZED		0x0000000800000000
#define DBG_ATTRIB_TYPE_VOLATILE			0x0000001000000000
#define DBG_ATTRIB_TYPE_ALL				0x0000001f00000000
#define DBG_ATTRIB_DATA					0x0000010000000000
#define DBG_ATTRIB_METHOD				0x0000020000000000
#define DBG_ATTRIB_PROPERTY				0x0000040000000000
#define DBG_ATTRIB_CLASS					0x0000080000000000
#define DBG_ATTRIB_BASECLASS				0x0000100000000000
#define DBG_ATTRIB_INTERFACE				0x0000200000000000
#define DBG_ATTRIB_INNERCLASS			0x0000400000000000
#define DBG_ATTRIB_MOSTDERIVEDCLASS		0x0000800000000000
#define DBG_ATTRIB_CHILD_ALL				0x0000ff0000000000
#define DBG_ATTRIB_MULTI_CUSTOM_VIEWERS  0x0001000000000000
#define DBG_ATTRIB_EVENT                 0x0002000000000000
typedef UINT64 DBG_ATTRIB_FLAGS;


enum enum_DEBUGPROP_INFO_FLAGS
    {	DEBUGPROP_INFO_FULLNAME	= 0x1,
	DEBUGPROP_INFO_NAME	= 0x2,
	DEBUGPROP_INFO_TYPE	= 0x4,
	DEBUGPROP_INFO_VALUE	= 0x8,
	DEBUGPROP_INFO_ATTRIB	= 0x10,
	DEBUGPROP_INFO_PROP	= 0x20,
	DEBUGPROP_INFO_VALUE_AUTOEXPAND	= 0x10000,
	DEBUGPROP_INFO_NOFUNCEVAL	= 0x20000,
	DEBUGPROP_INFO_VALUE_RAW	= 0x40000,
	DEBUGPROP_INFO_VALUE_NO_TOSTRING	= 0x80000,
	DEBUGPROP_INFO_NO_NONPUBLIC_MEMBERS	= 0x100000,
	DEBUGPROP_INFO_NONE	= 0,
	DEBUGPROP_INFO_STANDARD	= ( ( ( DEBUGPROP_INFO_ATTRIB | DEBUGPROP_INFO_NAME )  | DEBUGPROP_INFO_TYPE )  | DEBUGPROP_INFO_VALUE ) ,
	DEBUGPROP_INFO_ALL	= 0xffffffff
    } ;
typedef DWORD DEBUGPROP_INFO_FLAGS;

typedef struct tagDEBUG_PROPERTY_INFO
    {
    DEBUGPROP_INFO_FLAGS dwFields;
    BSTR bstrFullName;
    BSTR bstrName;
    BSTR bstrType;
    BSTR bstrValue;
    IDebugProperty2 *pProperty;
    DBG_ATTRIB_FLAGS dwAttrib;
    } 	DEBUG_PROPERTY_INFO;

typedef struct tagDEBUG_CUSTOM_VIEWER
    {
    DWORD dwID;
    BSTR bstrMenuName;
    BSTR bstrDescription;
    GUID guidLang;
    GUID guidVendor;
    BSTR bstrMetric;
    } 	DEBUG_CUSTOM_VIEWER;



extern RPC_IF_HANDLE __MIDL_itf_msdbg_0000_0080_v0_0_c_ifspec;
extern RPC_IF_HANDLE __MIDL_itf_msdbg_0000_0080_v0_0_s_ifspec;

#ifndef __IDebugProperty2_INTERFACE_DEFINED__
#define __IDebugProperty2_INTERFACE_DEFINED__

/* interface IDebugProperty2 */
/* [unique][uuid][object] */ 


EXTERN_C const IID IID_IDebugProperty2;

#if defined(__cplusplus) && !defined(CINTERFACE)
    
    MIDL_INTERFACE("a7ee3e7e-2dd2-4ad7-9697-f4aae3427762")
    IDebugProperty2 : public IUnknown
    {
    public:
        virtual HRESULT STDMETHODCALLTYPE GetPropertyInfo( 
            /* [in] */ DEBUGPROP_INFO_FLAGS dwFields,
            /* [in] */ DWORD dwRadix,
            /* [in] */ DWORD dwTimeout,
            /* [length_is][size_is][full][in] */ __RPC__in_ecount_part_opt(dwArgCount, dwArgCount) IDebugReference2 **rgpArgs,
            /* [in] */ DWORD dwArgCount,
            /* [out] */ __RPC__out DEBUG_PROPERTY_INFO *pPropertyInfo) = 0;
        
        virtual HRESULT STDMETHODCALLTYPE SetValueAsString( 
            /* [in] */ __RPC__in LPCOLESTR pszValue,
            /* [in] */ DWORD dwRadix,
            /* [in] */ DWORD dwTimeout) = 0;
        
        virtual HRESULT STDMETHODCALLTYPE SetValueAsReference( 
            /* [length_is][size_is][full][in] */ __RPC__in_ecount_part_opt(dwArgCount, dwArgCount) IDebugReference2 **rgpArgs,
            /* [in] */ DWORD dwArgCount,
            /* [in] */ __RPC__in_opt IDebugReference2 *pValue,
            /* [in] */ DWORD dwTimeout) = 0;
        
        virtual HRESULT STDMETHODCALLTYPE EnumChildren( 
            /* [in] */ DEBUGPROP_INFO_FLAGS dwFields,
            /* [in] */ DWORD dwRadix,
            /* [in] */ __RPC__in REFGUID guidFilter,
            /* [in] */ DBG_ATTRIB_FLAGS dwAttribFilter,
            /* [full][in] */ __RPC__in_opt LPCOLESTR pszNameFilter,
            /* [in] */ DWORD dwTimeout,
            /* [out] */ __RPC__deref_out_opt IEnumDebugPropertyInfo2 **ppEnum) = 0;
        
        virtual HRESULT STDMETHODCALLTYPE GetParent( 
            /* [out] */ __RPC__deref_out_opt IDebugProperty2 **ppParent) = 0;
        
        virtual HRESULT STDMETHODCALLTYPE GetDerivedMostProperty( 
            /* [out] */ __RPC__deref_out_opt IDebugProperty2 **ppDerivedMost) = 0;
        
        virtual HRESULT STDMETHODCALLTYPE GetMemoryBytes( 
            /* [out] */ __RPC__deref_out_opt IDebugMemoryBytes2 **ppMemoryBytes) = 0;
        
        virtual HRESULT STDMETHODCALLTYPE GetMemoryContext( 
            /* [out] */ __RPC__deref_out_opt IDebugMemoryContext2 **ppMemory) = 0;
        
        virtual HRESULT STDMETHODCALLTYPE GetSize( 
            /* [out] */ __RPC__out DWORD *pdwSize) = 0;
        
        virtual HRESULT STDMETHODCALLTYPE GetReference( 
            /* [out] */ __RPC__deref_out_opt IDebugReference2 **ppReference) = 0;
        
        virtual HRESULT STDMETHODCALLTYPE GetExtendedInfo( 
            /* [in] */ __RPC__in REFGUID guidExtendedInfo,
            /* [out] */ __RPC__out VARIANT *pExtendedInfo) = 0;
        
    };
    
#else 	/* C style interface */

    typedef struct IDebugProperty2Vtbl
    {
        BEGIN_INTERFACE
        
        HRESULT ( STDMETHODCALLTYPE *QueryInterface )( 
            IDebugProperty2 * This,
            /* [in] */ __RPC__in REFIID riid,
            /* [iid_is][out] */ 
            __RPC__deref_out  void **ppvObject);
        
        ULONG ( STDMETHODCALLTYPE *AddRef )( 
            IDebugProperty2 * This);
        
        ULONG ( STDMETHODCALLTYPE *Release )( 
            IDebugProperty2 * This);
        
        HRESULT ( STDMETHODCALLTYPE *GetPropertyInfo )( 
            IDebugProperty2 * This,
            /* [in] */ DEBUGPROP_INFO_FLAGS dwFields,
            /* [in] */ DWORD dwRadix,
            /* [in] */ DWORD dwTimeout,
            /* [length_is][size_is][full][in] */ __RPC__in_ecount_part_opt(dwArgCount, dwArgCount) IDebugReference2 **rgpArgs,
            /* [in] */ DWORD dwArgCount,
            /* [out] */ __RPC__out DEBUG_PROPERTY_INFO *pPropertyInfo);
        
        HRESULT ( STDMETHODCALLTYPE *SetValueAsString )( 
            IDebugProperty2 * This,
            /* [in] */ __RPC__in LPCOLESTR pszValue,
            /* [in] */ DWORD dwRadix,
            /* [in] */ DWORD dwTimeout);
        
        HRESULT ( STDMETHODCALLTYPE *SetValueAsReference )( 
            IDebugProperty2 * This,
            /* [length_is][size_is][full][in] */ __RPC__in_ecount_part_opt(dwArgCount, dwArgCount) IDebugReference2 **rgpArgs,
            /* [in] */ DWORD dwArgCount,
            /* [in] */ __RPC__in_opt IDebugReference2 *pValue,
            /* [in] */ DWORD dwTimeout);
        
        HRESULT ( STDMETHODCALLTYPE *EnumChildren )( 
            IDebugProperty2 * This,
            /* [in] */ DEBUGPROP_INFO_FLAGS dwFields,
            /* [in] */ DWORD dwRadix,
            /* [in] */ __RPC__in REFGUID guidFilter,
            /* [in] */ DBG_ATTRIB_FLAGS dwAttribFilter,
            /* [full][in] */ __RPC__in_opt LPCOLESTR pszNameFilter,
            /* [in] */ DWORD dwTimeout,
            /* [out] */ __RPC__deref_out_opt IEnumDebugPropertyInfo2 **ppEnum);
        
        HRESULT ( STDMETHODCALLTYPE *GetParent )( 
            IDebugProperty2 * This,
            /* [out] */ __RPC__deref_out_opt IDebugProperty2 **ppParent);
        
        HRESULT ( STDMETHODCALLTYPE *GetDerivedMostProperty )( 
            IDebugProperty2 * This,
            /* [out] */ __RPC__deref_out_opt IDebugProperty2 **ppDerivedMost);
        
        HRESULT ( STDMETHODCALLTYPE *GetMemoryBytes )( 
            IDebugProperty2 * This,
            /* [out] */ __RPC__deref_out_opt IDebugMemoryBytes2 **ppMemoryBytes);
        
        HRESULT ( STDMETHODCALLTYPE *GetMemoryContext )( 
            IDebugProperty2 * This,
            /* [out] */ __RPC__deref_out_opt IDebugMemoryContext2 **ppMemory);
        
        HRESULT ( STDMETHODCALLTYPE *GetSize )( 
            IDebugProperty2 * This,
            /* [out] */ __RPC__out DWORD *pdwSize);
        
        HRESULT ( STDMETHODCALLTYPE *GetReference )( 
            IDebugProperty2 * This,
            /* [out] */ __RPC__deref_out_opt IDebugReference2 **ppReference);
        
        HRESULT ( STDMETHODCALLTYPE *GetExtendedInfo )( 
            IDebugProperty2 * This,
            /* [in] */ __RPC__in REFGUID guidExtendedInfo,
            /* [out] */ __RPC__out VARIANT *pExtendedInfo);
        
        END_INTERFACE
    } IDebugProperty2Vtbl;

    interface IDebugProperty2
    {
        CONST_VTBL struct IDebugProperty2Vtbl *lpVtbl;
    };

    

#ifdef COBJMACROS


#define IDebugProperty2_QueryInterface(This,riid,ppvObject)	\
    ( (This)->lpVtbl -> QueryInterface(This,riid,ppvObject) ) 

#define IDebugProperty2_AddRef(This)	\
    ( (This)->lpVtbl -> AddRef(This) ) 

#define IDebugProperty2_Release(This)	\
    ( (This)->lpVtbl -> Release(This) ) 


#define IDebugProperty2_GetPropertyInfo(This,dwFields,dwRadix,dwTimeout,rgpArgs,dwArgCount,pPropertyInfo)	\
    ( (This)->lpVtbl -> GetPropertyInfo(This,dwFields,dwRadix,dwTimeout,rgpArgs,dwArgCount,pPropertyInfo) ) 

#define IDebugProperty2_SetValueAsString(This,pszValue,dwRadix,dwTimeout)	\
    ( (This)->lpVtbl -> SetValueAsString(This,pszValue,dwRadix,dwTimeout) ) 

#define IDebugProperty2_SetValueAsReference(This,rgpArgs,dwArgCount,pValue,dwTimeout)	\
    ( (This)->lpVtbl -> SetValueAsReference(This,rgpArgs,dwArgCount,pValue,dwTimeout) ) 

#define IDebugProperty2_EnumChildren(This,dwFields,dwRadix,guidFilter,dwAttribFilter,pszNameFilter,dwTimeout,ppEnum)	\
    ( (This)->lpVtbl -> EnumChildren(This,dwFields,dwRadix,guidFilter,dwAttribFilter,pszNameFilter,dwTimeout,ppEnum) ) 

#define IDebugProperty2_GetParent(This,ppParent)	\
    ( (This)->lpVtbl -> GetParent(This,ppParent) ) 

#define IDebugProperty2_GetDerivedMostProperty(This,ppDerivedMost)	\
    ( (This)->lpVtbl -> GetDerivedMostProperty(This,ppDerivedMost) ) 

#define IDebugProperty2_GetMemoryBytes(This,ppMemoryBytes)	\
    ( (This)->lpVtbl -> GetMemoryBytes(This,ppMemoryBytes) ) 

#define IDebugProperty2_GetMemoryContext(This,ppMemory)	\
    ( (This)->lpVtbl -> GetMemoryContext(This,ppMemory) ) 

#define IDebugProperty2_GetSize(This,pdwSize)	\
    ( (This)->lpVtbl -> GetSize(This,pdwSize) ) 

#define IDebugProperty2_GetReference(This,ppReference)	\
    ( (This)->lpVtbl -> GetReference(This,ppReference) ) 

#define IDebugProperty2_GetExtendedInfo(This,guidExtendedInfo,pExtendedInfo)	\
    ( (This)->lpVtbl -> GetExtendedInfo(This,guidExtendedInfo,pExtendedInfo) ) 

#endif /* COBJMACROS */


#endif 	/* C style interface */




#endif 	/* __IDebugProperty2_INTERFACE_DEFINED__ */


/* interface __MIDL_itf_msdbg_0000_0081 */
/* [local] */ 

#pragma warning(push)
#pragma warning(disable:28718)


extern RPC_IF_HANDLE __MIDL_itf_msdbg_0000_0081_v0_0_c_ifspec;
extern RPC_IF_HANDLE __MIDL_itf_msdbg_0000_0081_v0_0_s_ifspec;

#ifndef __IDebugProperty3_INTERFACE_DEFINED__
#define __IDebugProperty3_INTERFACE_DEFINED__

/* interface IDebugProperty3 */
/* [unique][uuid][object] */ 


EXTERN_C const IID IID_IDebugProperty3;

#if defined(__cplusplus) && !defined(CINTERFACE)
    
    MIDL_INTERFACE("94E1E004-0672-423d-AD62-78783DEF1E76")
    IDebugProperty3 : public IDebugProperty2
    {
    public:
        virtual HRESULT STDMETHODCALLTYPE GetStringCharLength( 
            /* [out] */ __RPC__out ULONG *pLen) = 0;
        
        virtual HRESULT STDMETHODCALLTYPE GetStringChars( 
            /* [in] */ ULONG buflen,
            /* [length_is][size_is][out] */ __RPC__out_ecount_part(buflen, *pceltFetched) WCHAR *rgString,
            /* [out] */ __RPC__out ULONG *pceltFetched) = 0;
        
        virtual HRESULT STDMETHODCALLTYPE CreateObjectID( void) = 0;
        
        virtual HRESULT STDMETHODCALLTYPE DestroyObjectID( void) = 0;
        
        virtual HRESULT STDMETHODCALLTYPE GetCustomViewerCount( 
            /* [out] */ __RPC__out ULONG *pcelt) = 0;
        
        virtual HRESULT STDMETHODCALLTYPE GetCustomViewerList( 
            /* [in] */ ULONG celtSkip,
            /* [in] */ ULONG celtRequested,
            /* [length_is][size_is][out] */ __RPC__out_ecount_part(celtRequested, *pceltFetched) DEBUG_CUSTOM_VIEWER *rgViewers,
            /* [out] */ __RPC__out ULONG *pceltFetched) = 0;
        
        virtual HRESULT STDMETHODCALLTYPE SetValueAsStringWithError( 
            /* [in] */ __RPC__in LPCOLESTR pszValue,
            /* [in] */ DWORD dwRadix,
            /* [in] */ DWORD dwTimeout,
            /* [out] */ __RPC__deref_out_opt BSTR *errorString) = 0;
        
    };
    
#else 	/* C style interface */

    typedef struct IDebugProperty3Vtbl
    {
        BEGIN_INTERFACE
        
        HRESULT ( STDMETHODCALLTYPE *QueryInterface )( 
            IDebugProperty3 * This,
            /* [in] */ __RPC__in REFIID riid,
            /* [iid_is][out] */ 
            __RPC__deref_out  void **ppvObject);
        
        ULONG ( STDMETHODCALLTYPE *AddRef )( 
            IDebugProperty3 * This);
        
        ULONG ( STDMETHODCALLTYPE *Release )( 
            IDebugProperty3 * This);
        
        HRESULT ( STDMETHODCALLTYPE *GetPropertyInfo )( 
            IDebugProperty3 * This,
            /* [in] */ DEBUGPROP_INFO_FLAGS dwFields,
            /* [in] */ DWORD dwRadix,
            /* [in] */ DWORD dwTimeout,
            /* [length_is][size_is][full][in] */ __RPC__in_ecount_part_opt(dwArgCount, dwArgCount) IDebugReference2 **rgpArgs,
            /* [in] */ DWORD dwArgCount,
            /* [out] */ __RPC__out DEBUG_PROPERTY_INFO *pPropertyInfo);
        
        HRESULT ( STDMETHODCALLTYPE *SetValueAsString )( 
            IDebugProperty3 * This,
            /* [in] */ __RPC__in LPCOLESTR pszValue,
            /* [in] */ DWORD dwRadix,
            /* [in] */ DWORD dwTimeout);
        
        HRESULT ( STDMETHODCALLTYPE *SetValueAsReference )( 
            IDebugProperty3 * This,
            /* [length_is][size_is][full][in] */ __RPC__in_ecount_part_opt(dwArgCount, dwArgCount) IDebugReference2 **rgpArgs,
            /* [in] */ DWORD dwArgCount,
            /* [in] */ __RPC__in_opt IDebugReference2 *pValue,
            /* [in] */ DWORD dwTimeout);
        
        HRESULT ( STDMETHODCALLTYPE *EnumChildren )( 
            IDebugProperty3 * This,
            /* [in] */ DEBUGPROP_INFO_FLAGS dwFields,
            /* [in] */ DWORD dwRadix,
            /* [in] */ __RPC__in REFGUID guidFilter,
            /* [in] */ DBG_ATTRIB_FLAGS dwAttribFilter,
            /* [full][in] */ __RPC__in_opt LPCOLESTR pszNameFilter,
            /* [in] */ DWORD dwTimeout,
            /* [out] */ __RPC__deref_out_opt IEnumDebugPropertyInfo2 **ppEnum);
        
        HRESULT ( STDMETHODCALLTYPE *GetParent )( 
            IDebugProperty3 * This,
            /* [out] */ __RPC__deref_out_opt IDebugProperty2 **ppParent);
        
        HRESULT ( STDMETHODCALLTYPE *GetDerivedMostProperty )( 
            IDebugProperty3 * This,
            /* [out] */ __RPC__deref_out_opt IDebugProperty2 **ppDerivedMost);
        
        HRESULT ( STDMETHODCALLTYPE *GetMemoryBytes )( 
            IDebugProperty3 * This,
            /* [out] */ __RPC__deref_out_opt IDebugMemoryBytes2 **ppMemoryBytes);
        
        HRESULT ( STDMETHODCALLTYPE *GetMemoryContext )( 
            IDebugProperty3 * This,
            /* [out] */ __RPC__deref_out_opt IDebugMemoryContext2 **ppMemory);
        
        HRESULT ( STDMETHODCALLTYPE *GetSize )( 
            IDebugProperty3 * This,
            /* [out] */ __RPC__out DWORD *pdwSize);
        
        HRESULT ( STDMETHODCALLTYPE *GetReference )( 
            IDebugProperty3 * This,
            /* [out] */ __RPC__deref_out_opt IDebugReference2 **ppReference);
        
        HRESULT ( STDMETHODCALLTYPE *GetExtendedInfo )( 
            IDebugProperty3 * This,
            /* [in] */ __RPC__in REFGUID guidExtendedInfo,
            /* [out] */ __RPC__out VARIANT *pExtendedInfo);
        
        HRESULT ( STDMETHODCALLTYPE *GetStringCharLength )( 
            IDebugProperty3 * This,
            /* [out] */ __RPC__out ULONG *pLen);
        
        HRESULT ( STDMETHODCALLTYPE *GetStringChars )( 
            IDebugProperty3 * This,
            /* [in] */ ULONG buflen,
            /* [length_is][size_is][out] */ __RPC__out_ecount_part(buflen, *pceltFetched) WCHAR *rgString,
            /* [out] */ __RPC__out ULONG *pceltFetched);
        
        HRESULT ( STDMETHODCALLTYPE *CreateObjectID )( 
            IDebugProperty3 * This);
        
        HRESULT ( STDMETHODCALLTYPE *DestroyObjectID )( 
            IDebugProperty3 * This);
        
        HRESULT ( STDMETHODCALLTYPE *GetCustomViewerCount )( 
            IDebugProperty3 * This,
            /* [out] */ __RPC__out ULONG *pcelt);
        
        HRESULT ( STDMETHODCALLTYPE *GetCustomViewerList )( 
            IDebugProperty3 * This,
            /* [in] */ ULONG celtSkip,
            /* [in] */ ULONG celtRequested,
            /* [length_is][size_is][out] */ __RPC__out_ecount_part(celtRequested, *pceltFetched) DEBUG_CUSTOM_VIEWER *rgViewers,
            /* [out] */ __RPC__out ULONG *pceltFetched);
        
        HRESULT ( STDMETHODCALLTYPE *SetValueAsStringWithError )( 
            IDebugProperty3 * This,
            /* [in] */ __RPC__in LPCOLESTR pszValue,
            /* [in] */ DWORD dwRadix,
            /* [in] */ DWORD dwTimeout,
            /* [out] */ __RPC__deref_out_opt BSTR *errorString);
        
        END_INTERFACE
    } IDebugProperty3Vtbl;

    interface IDebugProperty3
    {
        CONST_VTBL struct IDebugProperty3Vtbl *lpVtbl;
    };

    

#ifdef COBJMACROS


#define IDebugProperty3_QueryInterface(This,riid,ppvObject)	\
    ( (This)->lpVtbl -> QueryInterface(This,riid,ppvObject) ) 

#define IDebugProperty3_AddRef(This)	\
    ( (This)->lpVtbl -> AddRef(This) ) 

#define IDebugProperty3_Release(This)	\
    ( (This)->lpVtbl -> Release(This) ) 


#define IDebugProperty3_GetPropertyInfo(This,dwFields,dwRadix,dwTimeout,rgpArgs,dwArgCount,pPropertyInfo)	\
    ( (This)->lpVtbl -> GetPropertyInfo(This,dwFields,dwRadix,dwTimeout,rgpArgs,dwArgCount,pPropertyInfo) ) 

#define IDebugProperty3_SetValueAsString(This,pszValue,dwRadix,dwTimeout)	\
    ( (This)->lpVtbl -> SetValueAsString(This,pszValue,dwRadix,dwTimeout) ) 

#define IDebugProperty3_SetValueAsReference(This,rgpArgs,dwArgCount,pValue,dwTimeout)	\
    ( (This)->lpVtbl -> SetValueAsReference(This,rgpArgs,dwArgCount,pValue,dwTimeout) ) 

#define IDebugProperty3_EnumChildren(This,dwFields,dwRadix,guidFilter,dwAttribFilter,pszNameFilter,dwTimeout,ppEnum)	\
    ( (This)->lpVtbl -> EnumChildren(This,dwFields,dwRadix,guidFilter,dwAttribFilter,pszNameFilter,dwTimeout,ppEnum) ) 

#define IDebugProperty3_GetParent(This,ppParent)	\
    ( (This)->lpVtbl -> GetParent(This,ppParent) ) 

#define IDebugProperty3_GetDerivedMostProperty(This,ppDerivedMost)	\
    ( (This)->lpVtbl -> GetDerivedMostProperty(This,ppDerivedMost) ) 

#define IDebugProperty3_GetMemoryBytes(This,ppMemoryBytes)	\
    ( (This)->lpVtbl -> GetMemoryBytes(This,ppMemoryBytes) ) 

#define IDebugProperty3_GetMemoryContext(This,ppMemory)	\
    ( (This)->lpVtbl -> GetMemoryContext(This,ppMemory) ) 

#define IDebugProperty3_GetSize(This,pdwSize)	\
    ( (This)->lpVtbl -> GetSize(This,pdwSize) ) 

#define IDebugProperty3_GetReference(This,ppReference)	\
    ( (This)->lpVtbl -> GetReference(This,ppReference) ) 

#define IDebugProperty3_GetExtendedInfo(This,guidExtendedInfo,pExtendedInfo)	\
    ( (This)->lpVtbl -> GetExtendedInfo(This,guidExtendedInfo,pExtendedInfo) ) 


#define IDebugProperty3_GetStringCharLength(This,pLen)	\
    ( (This)->lpVtbl -> GetStringCharLength(This,pLen) ) 

#define IDebugProperty3_GetStringChars(This,buflen,rgString,pceltFetched)	\
    ( (This)->lpVtbl -> GetStringChars(This,buflen,rgString,pceltFetched) ) 

#define IDebugProperty3_CreateObjectID(This)	\
    ( (This)->lpVtbl -> CreateObjectID(This) ) 

#define IDebugProperty3_DestroyObjectID(This)	\
    ( (This)->lpVtbl -> DestroyObjectID(This) ) 

#define IDebugProperty3_GetCustomViewerCount(This,pcelt)	\
    ( (This)->lpVtbl -> GetCustomViewerCount(This,pcelt) ) 

#define IDebugProperty3_GetCustomViewerList(This,celtSkip,celtRequested,rgViewers,pceltFetched)	\
    ( (This)->lpVtbl -> GetCustomViewerList(This,celtSkip,celtRequested,rgViewers,pceltFetched) ) 

#define IDebugProperty3_SetValueAsStringWithError(This,pszValue,dwRadix,dwTimeout,errorString)	\
    ( (This)->lpVtbl -> SetValueAsStringWithError(This,pszValue,dwRadix,dwTimeout,errorString) ) 

#endif /* COBJMACROS */


#endif 	/* C style interface */




#endif 	/* __IDebugProperty3_INTERFACE_DEFINED__ */


#ifndef __IDebugSessionProperty2_INTERFACE_DEFINED__
#define __IDebugSessionProperty2_INTERFACE_DEFINED__

/* interface IDebugSessionProperty2 */
/* [unique][uuid][object] */ 


EXTERN_C const IID IID_IDebugSessionProperty2;

#if defined(__cplusplus) && !defined(CINTERFACE)
    
    MIDL_INTERFACE("72ff2712-0bc3-4308-a99d-26ac7ec68c5f")
    IDebugSessionProperty2 : public IDebugProperty3
    {
    public:
        virtual HRESULT STDMETHODCALLTYPE Close( void) = 0;
        
        virtual HRESULT STDMETHODCALLTYPE GetThread( 
            /* [out] */ __RPC__deref_out_opt IDebugThread3 **ppThread) = 0;
        
    };
    
#else 	/* C style interface */

    typedef struct IDebugSessionProperty2Vtbl
    {
        BEGIN_INTERFACE
        
        HRESULT ( STDMETHODCALLTYPE *QueryInterface )( 
            IDebugSessionProperty2 * This,
            /* [in] */ __RPC__in REFIID riid,
            /* [iid_is][out] */ 
            __RPC__deref_out  void **ppvObject);
        
        ULONG ( STDMETHODCALLTYPE *AddRef )( 
            IDebugSessionProperty2 * This);
        
        ULONG ( STDMETHODCALLTYPE *Release )( 
            IDebugSessionProperty2 * This);
        
        HRESULT ( STDMETHODCALLTYPE *GetPropertyInfo )( 
            IDebugSessionProperty2 * This,
            /* [in] */ DEBUGPROP_INFO_FLAGS dwFields,
            /* [in] */ DWORD dwRadix,
            /* [in] */ DWORD dwTimeout,
            /* [length_is][size_is][full][in] */ __RPC__in_ecount_part_opt(dwArgCount, dwArgCount) IDebugReference2 **rgpArgs,
            /* [in] */ DWORD dwArgCount,
            /* [out] */ __RPC__out DEBUG_PROPERTY_INFO *pPropertyInfo);
        
        HRESULT ( STDMETHODCALLTYPE *SetValueAsString )( 
            IDebugSessionProperty2 * This,
            /* [in] */ __RPC__in LPCOLESTR pszValue,
            /* [in] */ DWORD dwRadix,
            /* [in] */ DWORD dwTimeout);
        
        HRESULT ( STDMETHODCALLTYPE *SetValueAsReference )( 
            IDebugSessionProperty2 * This,
            /* [length_is][size_is][full][in] */ __RPC__in_ecount_part_opt(dwArgCount, dwArgCount) IDebugReference2 **rgpArgs,
            /* [in] */ DWORD dwArgCount,
            /* [in] */ __RPC__in_opt IDebugReference2 *pValue,
            /* [in] */ DWORD dwTimeout);
        
        HRESULT ( STDMETHODCALLTYPE *EnumChildren )( 
            IDebugSessionProperty2 * This,
            /* [in] */ DEBUGPROP_INFO_FLAGS dwFields,
            /* [in] */ DWORD dwRadix,
            /* [in] */ __RPC__in REFGUID guidFilter,
            /* [in] */ DBG_ATTRIB_FLAGS dwAttribFilter,
            /* [full][in] */ __RPC__in_opt LPCOLESTR pszNameFilter,
            /* [in] */ DWORD dwTimeout,
            /* [out] */ __RPC__deref_out_opt IEnumDebugPropertyInfo2 **ppEnum);
        
        HRESULT ( STDMETHODCALLTYPE *GetParent )( 
            IDebugSessionProperty2 * This,
            /* [out] */ __RPC__deref_out_opt IDebugProperty2 **ppParent);
        
        HRESULT ( STDMETHODCALLTYPE *GetDerivedMostProperty )( 
            IDebugSessionProperty2 * This,
            /* [out] */ __RPC__deref_out_opt IDebugProperty2 **ppDerivedMost);
        
        HRESULT ( STDMETHODCALLTYPE *GetMemoryBytes )( 
            IDebugSessionProperty2 * This,
            /* [out] */ __RPC__deref_out_opt IDebugMemoryBytes2 **ppMemoryBytes);
        
        HRESULT ( STDMETHODCALLTYPE *GetMemoryContext )( 
            IDebugSessionProperty2 * This,
            /* [out] */ __RPC__deref_out_opt IDebugMemoryContext2 **ppMemory);
        
        HRESULT ( STDMETHODCALLTYPE *GetSize )( 
            IDebugSessionProperty2 * This,
            /* [out] */ __RPC__out DWORD *pdwSize);
        
        HRESULT ( STDMETHODCALLTYPE *GetReference )( 
            IDebugSessionProperty2 * This,
            /* [out] */ __RPC__deref_out_opt IDebugReference2 **ppReference);
        
        HRESULT ( STDMETHODCALLTYPE *GetExtendedInfo )( 
            IDebugSessionProperty2 * This,
            /* [in] */ __RPC__in REFGUID guidExtendedInfo,
            /* [out] */ __RPC__out VARIANT *pExtendedInfo);
        
        HRESULT ( STDMETHODCALLTYPE *GetStringCharLength )( 
            IDebugSessionProperty2 * This,
            /* [out] */ __RPC__out ULONG *pLen);
        
        HRESULT ( STDMETHODCALLTYPE *GetStringChars )( 
            IDebugSessionProperty2 * This,
            /* [in] */ ULONG buflen,
            /* [length_is][size_is][out] */ __RPC__out_ecount_part(buflen, *pceltFetched) WCHAR *rgString,
            /* [out] */ __RPC__out ULONG *pceltFetched);
        
        HRESULT ( STDMETHODCALLTYPE *CreateObjectID )( 
            IDebugSessionProperty2 * This);
        
        HRESULT ( STDMETHODCALLTYPE *DestroyObjectID )( 
            IDebugSessionProperty2 * This);
        
        HRESULT ( STDMETHODCALLTYPE *GetCustomViewerCount )( 
            IDebugSessionProperty2 * This,
            /* [out] */ __RPC__out ULONG *pcelt);
        
        HRESULT ( STDMETHODCALLTYPE *GetCustomViewerList )( 
            IDebugSessionProperty2 * This,
            /* [in] */ ULONG celtSkip,
            /* [in] */ ULONG celtRequested,
            /* [length_is][size_is][out] */ __RPC__out_ecount_part(celtRequested, *pceltFetched) DEBUG_CUSTOM_VIEWER *rgViewers,
            /* [out] */ __RPC__out ULONG *pceltFetched);
        
        HRESULT ( STDMETHODCALLTYPE *SetValueAsStringWithError )( 
            IDebugSessionProperty2 * This,
            /* [in] */ __RPC__in LPCOLESTR pszValue,
            /* [in] */ DWORD dwRadix,
            /* [in] */ DWORD dwTimeout,
            /* [out] */ __RPC__deref_out_opt BSTR *errorString);
        
        HRESULT ( STDMETHODCALLTYPE *Close )( 
            IDebugSessionProperty2 * This);
        
        HRESULT ( STDMETHODCALLTYPE *GetThread )( 
            IDebugSessionProperty2 * This,
            /* [out] */ __RPC__deref_out_opt IDebugThread3 **ppThread);
        
        END_INTERFACE
    } IDebugSessionProperty2Vtbl;

    interface IDebugSessionProperty2
    {
        CONST_VTBL struct IDebugSessionProperty2Vtbl *lpVtbl;
    };

    

#ifdef COBJMACROS


#define IDebugSessionProperty2_QueryInterface(This,riid,ppvObject)	\
    ( (This)->lpVtbl -> QueryInterface(This,riid,ppvObject) ) 

#define IDebugSessionProperty2_AddRef(This)	\
    ( (This)->lpVtbl -> AddRef(This) ) 

#define IDebugSessionProperty2_Release(This)	\
    ( (This)->lpVtbl -> Release(This) ) 


#define IDebugSessionProperty2_GetPropertyInfo(This,dwFields,dwRadix,dwTimeout,rgpArgs,dwArgCount,pPropertyInfo)	\
    ( (This)->lpVtbl -> GetPropertyInfo(This,dwFields,dwRadix,dwTimeout,rgpArgs,dwArgCount,pPropertyInfo) ) 

#define IDebugSessionProperty2_SetValueAsString(This,pszValue,dwRadix,dwTimeout)	\
    ( (This)->lpVtbl -> SetValueAsString(This,pszValue,dwRadix,dwTimeout) ) 

#define IDebugSessionProperty2_SetValueAsReference(This,rgpArgs,dwArgCount,pValue,dwTimeout)	\
    ( (This)->lpVtbl -> SetValueAsReference(This,rgpArgs,dwArgCount,pValue,dwTimeout) ) 

#define IDebugSessionProperty2_EnumChildren(This,dwFields,dwRadix,guidFilter,dwAttribFilter,pszNameFilter,dwTimeout,ppEnum)	\
    ( (This)->lpVtbl -> EnumChildren(This,dwFields,dwRadix,guidFilter,dwAttribFilter,pszNameFilter,dwTimeout,ppEnum) ) 

#define IDebugSessionProperty2_GetParent(This,ppParent)	\
    ( (This)->lpVtbl -> GetParent(This,ppParent) ) 

#define IDebugSessionProperty2_GetDerivedMostProperty(This,ppDerivedMost)	\
    ( (This)->lpVtbl -> GetDerivedMostProperty(This,ppDerivedMost) ) 

#define IDebugSessionProperty2_GetMemoryBytes(This,ppMemoryBytes)	\
    ( (This)->lpVtbl -> GetMemoryBytes(This,ppMemoryBytes) ) 

#define IDebugSessionProperty2_GetMemoryContext(This,ppMemory)	\
    ( (This)->lpVtbl -> GetMemoryContext(This,ppMemory) ) 

#define IDebugSessionProperty2_GetSize(This,pdwSize)	\
    ( (This)->lpVtbl -> GetSize(This,pdwSize) ) 

#define IDebugSessionProperty2_GetReference(This,ppReference)	\
    ( (This)->lpVtbl -> GetReference(This,ppReference) ) 

#define IDebugSessionProperty2_GetExtendedInfo(This,guidExtendedInfo,pExtendedInfo)	\
    ( (This)->lpVtbl -> GetExtendedInfo(This,guidExtendedInfo,pExtendedInfo) ) 


#define IDebugSessionProperty2_GetStringCharLength(This,pLen)	\
    ( (This)->lpVtbl -> GetStringCharLength(This,pLen) ) 

#define IDebugSessionProperty2_GetStringChars(This,buflen,rgString,pceltFetched)	\
    ( (This)->lpVtbl -> GetStringChars(This,buflen,rgString,pceltFetched) ) 

#define IDebugSessionProperty2_CreateObjectID(This)	\
    ( (This)->lpVtbl -> CreateObjectID(This) ) 

#define IDebugSessionProperty2_DestroyObjectID(This)	\
    ( (This)->lpVtbl -> DestroyObjectID(This) ) 

#define IDebugSessionProperty2_GetCustomViewerCount(This,pcelt)	\
    ( (This)->lpVtbl -> GetCustomViewerCount(This,pcelt) ) 

#define IDebugSessionProperty2_GetCustomViewerList(This,celtSkip,celtRequested,rgViewers,pceltFetched)	\
    ( (This)->lpVtbl -> GetCustomViewerList(This,celtSkip,celtRequested,rgViewers,pceltFetched) ) 

#define IDebugSessionProperty2_SetValueAsStringWithError(This,pszValue,dwRadix,dwTimeout,errorString)	\
    ( (This)->lpVtbl -> SetValueAsStringWithError(This,pszValue,dwRadix,dwTimeout,errorString) ) 


#define IDebugSessionProperty2_Close(This)	\
    ( (This)->lpVtbl -> Close(This) ) 

#define IDebugSessionProperty2_GetThread(This,ppThread)	\
    ( (This)->lpVtbl -> GetThread(This,ppThread) ) 

#endif /* COBJMACROS */


#endif 	/* C style interface */




#endif 	/* __IDebugSessionProperty2_INTERFACE_DEFINED__ */


/* interface __MIDL_itf_msdbg_0000_0082 */
/* [local] */ 

#pragma warning(pop)


extern RPC_IF_HANDLE __MIDL_itf_msdbg_0000_0082_v0_0_c_ifspec;
extern RPC_IF_HANDLE __MIDL_itf_msdbg_0000_0082_v0_0_s_ifspec;

#ifndef __IDebugPropertyClose2_INTERFACE_DEFINED__
#define __IDebugPropertyClose2_INTERFACE_DEFINED__

/* interface IDebugPropertyClose2 */
/* [unique][uuid][object] */ 


EXTERN_C const IID IID_IDebugPropertyClose2;

#if defined(__cplusplus) && !defined(CINTERFACE)
    
    MIDL_INTERFACE("852c7d42-794f-43cd-a18f-cd40e83e67cd")
    IDebugPropertyClose2 : public IDebugProperty2
    {
    public:
        virtual HRESULT STDMETHODCALLTYPE Close( void) = 0;
        
    };
    
#else 	/* C style interface */

    typedef struct IDebugPropertyClose2Vtbl
    {
        BEGIN_INTERFACE
        
        HRESULT ( STDMETHODCALLTYPE *QueryInterface )( 
            IDebugPropertyClose2 * This,
            /* [in] */ __RPC__in REFIID riid,
            /* [iid_is][out] */ 
            __RPC__deref_out  void **ppvObject);
        
        ULONG ( STDMETHODCALLTYPE *AddRef )( 
            IDebugPropertyClose2 * This);
        
        ULONG ( STDMETHODCALLTYPE *Release )( 
            IDebugPropertyClose2 * This);
        
        HRESULT ( STDMETHODCALLTYPE *GetPropertyInfo )( 
            IDebugPropertyClose2 * This,
            /* [in] */ DEBUGPROP_INFO_FLAGS dwFields,
            /* [in] */ DWORD dwRadix,
            /* [in] */ DWORD dwTimeout,
            /* [length_is][size_is][full][in] */ __RPC__in_ecount_part_opt(dwArgCount, dwArgCount) IDebugReference2 **rgpArgs,
            /* [in] */ DWORD dwArgCount,
            /* [out] */ __RPC__out DEBUG_PROPERTY_INFO *pPropertyInfo);
        
        HRESULT ( STDMETHODCALLTYPE *SetValueAsString )( 
            IDebugPropertyClose2 * This,
            /* [in] */ __RPC__in LPCOLESTR pszValue,
            /* [in] */ DWORD dwRadix,
            /* [in] */ DWORD dwTimeout);
        
        HRESULT ( STDMETHODCALLTYPE *SetValueAsReference )( 
            IDebugPropertyClose2 * This,
            /* [length_is][size_is][full][in] */ __RPC__in_ecount_part_opt(dwArgCount, dwArgCount) IDebugReference2 **rgpArgs,
            /* [in] */ DWORD dwArgCount,
            /* [in] */ __RPC__in_opt IDebugReference2 *pValue,
            /* [in] */ DWORD dwTimeout);
        
        HRESULT ( STDMETHODCALLTYPE *EnumChildren )( 
            IDebugPropertyClose2 * This,
            /* [in] */ DEBUGPROP_INFO_FLAGS dwFields,
            /* [in] */ DWORD dwRadix,
            /* [in] */ __RPC__in REFGUID guidFilter,
            /* [in] */ DBG_ATTRIB_FLAGS dwAttribFilter,
            /* [full][in] */ __RPC__in_opt LPCOLESTR pszNameFilter,
            /* [in] */ DWORD dwTimeout,
            /* [out] */ __RPC__deref_out_opt IEnumDebugPropertyInfo2 **ppEnum);
        
        HRESULT ( STDMETHODCALLTYPE *GetParent )( 
            IDebugPropertyClose2 * This,
            /* [out] */ __RPC__deref_out_opt IDebugProperty2 **ppParent);
        
        HRESULT ( STDMETHODCALLTYPE *GetDerivedMostProperty )( 
            IDebugPropertyClose2 * This,
            /* [out] */ __RPC__deref_out_opt IDebugProperty2 **ppDerivedMost);
        
        HRESULT ( STDMETHODCALLTYPE *GetMemoryBytes )( 
            IDebugPropertyClose2 * This,
            /* [out] */ __RPC__deref_out_opt IDebugMemoryBytes2 **ppMemoryBytes);
        
        HRESULT ( STDMETHODCALLTYPE *GetMemoryContext )( 
            IDebugPropertyClose2 * This,
            /* [out] */ __RPC__deref_out_opt IDebugMemoryContext2 **ppMemory);
        
        HRESULT ( STDMETHODCALLTYPE *GetSize )( 
            IDebugPropertyClose2 * This,
            /* [out] */ __RPC__out DWORD *pdwSize);
        
        HRESULT ( STDMETHODCALLTYPE *GetReference )( 
            IDebugPropertyClose2 * This,
            /* [out] */ __RPC__deref_out_opt IDebugReference2 **ppReference);
        
        HRESULT ( STDMETHODCALLTYPE *GetExtendedInfo )( 
            IDebugPropertyClose2 * This,
            /* [in] */ __RPC__in REFGUID guidExtendedInfo,
            /* [out] */ __RPC__out VARIANT *pExtendedInfo);
        
        HRESULT ( STDMETHODCALLTYPE *Close )( 
            IDebugPropertyClose2 * This);
        
        END_INTERFACE
    } IDebugPropertyClose2Vtbl;

    interface IDebugPropertyClose2
    {
        CONST_VTBL struct IDebugPropertyClose2Vtbl *lpVtbl;
    };

    

#ifdef COBJMACROS


#define IDebugPropertyClose2_QueryInterface(This,riid,ppvObject)	\
    ( (This)->lpVtbl -> QueryInterface(This,riid,ppvObject) ) 

#define IDebugPropertyClose2_AddRef(This)	\
    ( (This)->lpVtbl -> AddRef(This) ) 

#define IDebugPropertyClose2_Release(This)	\
    ( (This)->lpVtbl -> Release(This) ) 


#define IDebugPropertyClose2_GetPropertyInfo(This,dwFields,dwRadix,dwTimeout,rgpArgs,dwArgCount,pPropertyInfo)	\
    ( (This)->lpVtbl -> GetPropertyInfo(This,dwFields,dwRadix,dwTimeout,rgpArgs,dwArgCount,pPropertyInfo) ) 

#define IDebugPropertyClose2_SetValueAsString(This,pszValue,dwRadix,dwTimeout)	\
    ( (This)->lpVtbl -> SetValueAsString(This,pszValue,dwRadix,dwTimeout) ) 

#define IDebugPropertyClose2_SetValueAsReference(This,rgpArgs,dwArgCount,pValue,dwTimeout)	\
    ( (This)->lpVtbl -> SetValueAsReference(This,rgpArgs,dwArgCount,pValue,dwTimeout) ) 

#define IDebugPropertyClose2_EnumChildren(This,dwFields,dwRadix,guidFilter,dwAttribFilter,pszNameFilter,dwTimeout,ppEnum)	\
    ( (This)->lpVtbl -> EnumChildren(This,dwFields,dwRadix,guidFilter,dwAttribFilter,pszNameFilter,dwTimeout,ppEnum) ) 

#define IDebugPropertyClose2_GetParent(This,ppParent)	\
    ( (This)->lpVtbl -> GetParent(This,ppParent) ) 

#define IDebugPropertyClose2_GetDerivedMostProperty(This,ppDerivedMost)	\
    ( (This)->lpVtbl -> GetDerivedMostProperty(This,ppDerivedMost) ) 

#define IDebugPropertyClose2_GetMemoryBytes(This,ppMemoryBytes)	\
    ( (This)->lpVtbl -> GetMemoryBytes(This,ppMemoryBytes) ) 

#define IDebugPropertyClose2_GetMemoryContext(This,ppMemory)	\
    ( (This)->lpVtbl -> GetMemoryContext(This,ppMemory) ) 

#define IDebugPropertyClose2_GetSize(This,pdwSize)	\
    ( (This)->lpVtbl -> GetSize(This,pdwSize) ) 

#define IDebugPropertyClose2_GetReference(This,ppReference)	\
    ( (This)->lpVtbl -> GetReference(This,ppReference) ) 

#define IDebugPropertyClose2_GetExtendedInfo(This,guidExtendedInfo,pExtendedInfo)	\
    ( (This)->lpVtbl -> GetExtendedInfo(This,guidExtendedInfo,pExtendedInfo) ) 


#define IDebugPropertyClose2_Close(This)	\
    ( (This)->lpVtbl -> Close(This) ) 

#endif /* COBJMACROS */


#endif 	/* C style interface */




#endif 	/* __IDebugPropertyClose2_INTERFACE_DEFINED__ */


/* interface __MIDL_itf_msdbg_0000_0083 */
/* [local] */ 

#pragma warning(push)
#pragma warning(disable:28718)


extern RPC_IF_HANDLE __MIDL_itf_msdbg_0000_0083_v0_0_c_ifspec;
extern RPC_IF_HANDLE __MIDL_itf_msdbg_0000_0083_v0_0_s_ifspec;

/* interface __MIDL_itf_msdbg_0000_0084 */
/* [local] */ 

#pragma warning(pop)


extern RPC_IF_HANDLE __MIDL_itf_msdbg_0000_0084_v0_0_c_ifspec;
extern RPC_IF_HANDLE __MIDL_itf_msdbg_0000_0084_v0_0_s_ifspec;

#ifndef __IDebugDataGrid_INTERFACE_DEFINED__
#define __IDebugDataGrid_INTERFACE_DEFINED__

/* interface IDebugDataGrid */
/* [unique][uuid][object] */ 


EXTERN_C const IID IID_IDebugDataGrid;

#if defined(__cplusplus) && !defined(CINTERFACE)
    
    MIDL_INTERFACE("411F3E08-E6B1-4789-AB29-755C52E52AC4")
    IDebugDataGrid : public IUnknown
    {
    public:
        virtual HRESULT STDMETHODCALLTYPE GetGridInfo( 
            /* [out] */ __RPC__out ULONG *pX,
            /* [out] */ __RPC__out ULONG *pY,
            /* [out] */ __RPC__deref_out_opt BSTR *bpstrTitle) = 0;
        
        virtual HRESULT STDMETHODCALLTYPE GetGridPropertyInfo( 
            ULONG x,
            ULONG y,
            ULONG celtX,
            ULONG celtY,
            ULONG celtXtimesY,
            DEBUGPROP_INFO_FLAGS dwFields,
            DWORD dwRadix,
            /* [length_is][size_is][out] */ __RPC__out_ecount_part(celtXtimesY, *pceltFetched) DEBUG_PROPERTY_INFO *rgelt,
            /* [out] */ __RPC__out ULONG *pceltFetched) = 0;
        
    };
    
#else 	/* C style interface */

    typedef struct IDebugDataGridVtbl
    {
        BEGIN_INTERFACE
        
        HRESULT ( STDMETHODCALLTYPE *QueryInterface )( 
            IDebugDataGrid * This,
            /* [in] */ __RPC__in REFIID riid,
            /* [iid_is][out] */ 
            __RPC__deref_out  void **ppvObject);
        
        ULONG ( STDMETHODCALLTYPE *AddRef )( 
            IDebugDataGrid * This);
        
        ULONG ( STDMETHODCALLTYPE *Release )( 
            IDebugDataGrid * This);
        
        HRESULT ( STDMETHODCALLTYPE *GetGridInfo )( 
            IDebugDataGrid * This,
            /* [out] */ __RPC__out ULONG *pX,
            /* [out] */ __RPC__out ULONG *pY,
            /* [out] */ __RPC__deref_out_opt BSTR *bpstrTitle);
        
        HRESULT ( STDMETHODCALLTYPE *GetGridPropertyInfo )( 
            IDebugDataGrid * This,
            ULONG x,
            ULONG y,
            ULONG celtX,
            ULONG celtY,
            ULONG celtXtimesY,
            DEBUGPROP_INFO_FLAGS dwFields,
            DWORD dwRadix,
            /* [length_is][size_is][out] */ __RPC__out_ecount_part(celtXtimesY, *pceltFetched) DEBUG_PROPERTY_INFO *rgelt,
            /* [out] */ __RPC__out ULONG *pceltFetched);
        
        END_INTERFACE
    } IDebugDataGridVtbl;

    interface IDebugDataGrid
    {
        CONST_VTBL struct IDebugDataGridVtbl *lpVtbl;
    };

    

#ifdef COBJMACROS


#define IDebugDataGrid_QueryInterface(This,riid,ppvObject)	\
    ( (This)->lpVtbl -> QueryInterface(This,riid,ppvObject) ) 

#define IDebugDataGrid_AddRef(This)	\
    ( (This)->lpVtbl -> AddRef(This) ) 

#define IDebugDataGrid_Release(This)	\
    ( (This)->lpVtbl -> Release(This) ) 


#define IDebugDataGrid_GetGridInfo(This,pX,pY,bpstrTitle)	\
    ( (This)->lpVtbl -> GetGridInfo(This,pX,pY,bpstrTitle) ) 

#define IDebugDataGrid_GetGridPropertyInfo(This,x,y,celtX,celtY,celtXtimesY,dwFields,dwRadix,rgelt,pceltFetched)	\
    ( (This)->lpVtbl -> GetGridPropertyInfo(This,x,y,celtX,celtY,celtXtimesY,dwFields,dwRadix,rgelt,pceltFetched) ) 

#endif /* COBJMACROS */


#endif 	/* C style interface */




#endif 	/* __IDebugDataGrid_INTERFACE_DEFINED__ */


#ifndef __IDebugPropertySafetyWrapper_INTERFACE_DEFINED__
#define __IDebugPropertySafetyWrapper_INTERFACE_DEFINED__

/* interface IDebugPropertySafetyWrapper */
/* [unique][uuid][object] */ 


EXTERN_C const IID IID_IDebugPropertySafetyWrapper;

#if defined(__cplusplus) && !defined(CINTERFACE)
    
    MIDL_INTERFACE("7031886B-61D2-4cb5-B909-00386090733B")
    IDebugPropertySafetyWrapper : public IUnknown
    {
    public:
        virtual HRESULT STDMETHODCALLTYPE BeforePropertyCall( void) = 0;
        
        virtual HRESULT STDMETHODCALLTYPE AfterPropertyCall( void) = 0;
        
        virtual HRESULT STDMETHODCALLTYPE GetRawProperty( 
            /* [out] */ __RPC__deref_out_opt IDebugProperty3 **ppProperty) = 0;
        
    };
    
#else 	/* C style interface */

    typedef struct IDebugPropertySafetyWrapperVtbl
    {
        BEGIN_INTERFACE
        
        HRESULT ( STDMETHODCALLTYPE *QueryInterface )( 
            IDebugPropertySafetyWrapper * This,
            /* [in] */ __RPC__in REFIID riid,
            /* [iid_is][out] */ 
            __RPC__deref_out  void **ppvObject);
        
        ULONG ( STDMETHODCALLTYPE *AddRef )( 
            IDebugPropertySafetyWrapper * This);
        
        ULONG ( STDMETHODCALLTYPE *Release )( 
            IDebugPropertySafetyWrapper * This);
        
        HRESULT ( STDMETHODCALLTYPE *BeforePropertyCall )( 
            IDebugPropertySafetyWrapper * This);
        
        HRESULT ( STDMETHODCALLTYPE *AfterPropertyCall )( 
            IDebugPropertySafetyWrapper * This);
        
        HRESULT ( STDMETHODCALLTYPE *GetRawProperty )( 
            IDebugPropertySafetyWrapper * This,
            /* [out] */ __RPC__deref_out_opt IDebugProperty3 **ppProperty);
        
        END_INTERFACE
    } IDebugPropertySafetyWrapperVtbl;

    interface IDebugPropertySafetyWrapper
    {
        CONST_VTBL struct IDebugPropertySafetyWrapperVtbl *lpVtbl;
    };

    

#ifdef COBJMACROS


#define IDebugPropertySafetyWrapper_QueryInterface(This,riid,ppvObject)	\
    ( (This)->lpVtbl -> QueryInterface(This,riid,ppvObject) ) 

#define IDebugPropertySafetyWrapper_AddRef(This)	\
    ( (This)->lpVtbl -> AddRef(This) ) 

#define IDebugPropertySafetyWrapper_Release(This)	\
    ( (This)->lpVtbl -> Release(This) ) 


#define IDebugPropertySafetyWrapper_BeforePropertyCall(This)	\
    ( (This)->lpVtbl -> BeforePropertyCall(This) ) 

#define IDebugPropertySafetyWrapper_AfterPropertyCall(This)	\
    ( (This)->lpVtbl -> AfterPropertyCall(This) ) 

#define IDebugPropertySafetyWrapper_GetRawProperty(This,ppProperty)	\
    ( (This)->lpVtbl -> GetRawProperty(This,ppProperty) ) 

#endif /* COBJMACROS */


#endif 	/* C style interface */




#endif 	/* __IDebugPropertySafetyWrapper_INTERFACE_DEFINED__ */


/* interface __MIDL_itf_msdbg_0000_0086 */
/* [local] */ 


enum enum_REFERENCE_TYPE
    {	REF_TYPE_WEAK	= 0x1,
	REF_TYPE_STRONG	= 0x2
    } ;
typedef DWORD REFERENCE_TYPE;


enum enum_DEBUGREF_INFO_FLAGS
    {	DEBUGREF_INFO_NAME	= 0x1,
	DEBUGREF_INFO_TYPE	= 0x2,
	DEBUGREF_INFO_VALUE	= 0x4,
	DEBUGREF_INFO_ATTRIB	= 0x8,
	DEBUGREF_INFO_REFTYPE	= 0x10,
	DEBUGREF_INFO_REF	= 0x20,
	DEBUGREF_INFO_VALUE_AUTOEXPAND	= 0x10000,
	DEBUGREF_INFO_NONE	= 0,
	DEBUGREF_INFO_ALL	= 0xffffffff
    } ;
typedef DWORD DEBUGREF_INFO_FLAGS;

typedef struct tagDEBUG_REFERENCE_INFO
    {
    DEBUGREF_INFO_FLAGS dwFields;
    BSTR bstrName;
    BSTR bstrType;
    BSTR bstrValue;
    DBG_ATTRIB_FLAGS dwAttrib;
    REFERENCE_TYPE dwRefType;
    IDebugReference2 *pReference;
    } 	DEBUG_REFERENCE_INFO;



extern RPC_IF_HANDLE __MIDL_itf_msdbg_0000_0086_v0_0_c_ifspec;
extern RPC_IF_HANDLE __MIDL_itf_msdbg_0000_0086_v0_0_s_ifspec;

#ifndef __IDebugReference2_INTERFACE_DEFINED__
#define __IDebugReference2_INTERFACE_DEFINED__

/* interface IDebugReference2 */
/* [unique][uuid][object] */ 


enum enum_REFERENCE_COMPARE
    {	REF_COMPARE_EQUAL	= 0x1,
	REF_COMPARE_LESS_THAN	= 0x2,
	REF_COMPARE_GREATER_THAN	= 0x3
    } ;
typedef DWORD REFERENCE_COMPARE;


EXTERN_C const IID IID_IDebugReference2;

#if defined(__cplusplus) && !defined(CINTERFACE)
    
    MIDL_INTERFACE("10b793ac-0c47-4679-8454-adb36f29f802")
    IDebugReference2 : public IUnknown
    {
    public:
        virtual HRESULT STDMETHODCALLTYPE GetReferenceInfo( 
            /* [in] */ DEBUGREF_INFO_FLAGS dwFields,
            /* [in] */ DWORD dwRadix,
            /* [in] */ DWORD dwTimeout,
            /* [length_is][size_is][in] */ __RPC__in_ecount_part(dwArgCount, dwArgCount) IDebugReference2 **rgpArgs,
            /* [in] */ DWORD dwArgCount,
            /* [out] */ __RPC__out DEBUG_REFERENCE_INFO *pReferenceInfo) = 0;
        
        virtual HRESULT STDMETHODCALLTYPE SetValueAsString( 
            /* [in] */ __RPC__in LPCOLESTR pszValue,
            /* [in] */ DWORD dwRadix,
            /* [in] */ DWORD dwTimeout) = 0;
        
        virtual HRESULT STDMETHODCALLTYPE SetValueAsReference( 
            /* [length_is][size_is][in] */ __RPC__in_ecount_part(dwArgCount, dwArgCount) IDebugReference2 **rgpArgs,
            /* [in] */ DWORD dwArgCount,
            /* [in] */ __RPC__in_opt IDebugReference2 *pValue,
            /* [in] */ DWORD dwTimeout) = 0;
        
        virtual HRESULT STDMETHODCALLTYPE EnumChildren( 
            /* [in] */ DEBUGREF_INFO_FLAGS dwFields,
            /* [in] */ DWORD dwRadix,
            /* [in] */ DBG_ATTRIB_FLAGS dwAttribFilter,
            /* [full][in] */ __RPC__in_opt LPCOLESTR pszNameFilter,
            /* [in] */ DWORD dwTimeout,
            /* [out] */ __RPC__deref_out_opt IEnumDebugReferenceInfo2 **ppEnum) = 0;
        
        virtual HRESULT STDMETHODCALLTYPE GetParent( 
            /* [out] */ __RPC__deref_out_opt IDebugReference2 **ppParent) = 0;
        
        virtual HRESULT STDMETHODCALLTYPE GetDerivedMostReference( 
            /* [out] */ __RPC__deref_out_opt IDebugReference2 **ppDerivedMost) = 0;
        
        virtual HRESULT STDMETHODCALLTYPE GetMemoryBytes( 
            /* [out] */ __RPC__deref_out_opt IDebugMemoryBytes2 **ppMemoryBytes) = 0;
        
        virtual HRESULT STDMETHODCALLTYPE GetMemoryContext( 
            /* [out] */ __RPC__deref_out_opt IDebugMemoryContext2 **ppMemory) = 0;
        
        virtual HRESULT STDMETHODCALLTYPE GetSize( 
            /* [out] */ __RPC__out DWORD *pdwSize) = 0;
        
        virtual HRESULT STDMETHODCALLTYPE SetReferenceType( 
            /* [in] */ REFERENCE_TYPE dwRefType) = 0;
        
        virtual HRESULT STDMETHODCALLTYPE Compare( 
            /* [in] */ REFERENCE_COMPARE dwCompare,
            /* [in] */ __RPC__in_opt IDebugReference2 *pReference) = 0;
        
    };
    
#else 	/* C style interface */

    typedef struct IDebugReference2Vtbl
    {
        BEGIN_INTERFACE
        
        HRESULT ( STDMETHODCALLTYPE *QueryInterface )( 
            IDebugReference2 * This,
            /* [in] */ __RPC__in REFIID riid,
            /* [iid_is][out] */ 
            __RPC__deref_out  void **ppvObject);
        
        ULONG ( STDMETHODCALLTYPE *AddRef )( 
            IDebugReference2 * This);
        
        ULONG ( STDMETHODCALLTYPE *Release )( 
            IDebugReference2 * This);
        
        HRESULT ( STDMETHODCALLTYPE *GetReferenceInfo )( 
            IDebugReference2 * This,
            /* [in] */ DEBUGREF_INFO_FLAGS dwFields,
            /* [in] */ DWORD dwRadix,
            /* [in] */ DWORD dwTimeout,
            /* [length_is][size_is][in] */ __RPC__in_ecount_part(dwArgCount, dwArgCount) IDebugReference2 **rgpArgs,
            /* [in] */ DWORD dwArgCount,
            /* [out] */ __RPC__out DEBUG_REFERENCE_INFO *pReferenceInfo);
        
        HRESULT ( STDMETHODCALLTYPE *SetValueAsString )( 
            IDebugReference2 * This,
            /* [in] */ __RPC__in LPCOLESTR pszValue,
            /* [in] */ DWORD dwRadix,
            /* [in] */ DWORD dwTimeout);
        
        HRESULT ( STDMETHODCALLTYPE *SetValueAsReference )( 
            IDebugReference2 * This,
            /* [length_is][size_is][in] */ __RPC__in_ecount_part(dwArgCount, dwArgCount) IDebugReference2 **rgpArgs,
            /* [in] */ DWORD dwArgCount,
            /* [in] */ __RPC__in_opt IDebugReference2 *pValue,
            /* [in] */ DWORD dwTimeout);
        
        HRESULT ( STDMETHODCALLTYPE *EnumChildren )( 
            IDebugReference2 * This,
            /* [in] */ DEBUGREF_INFO_FLAGS dwFields,
            /* [in] */ DWORD dwRadix,
            /* [in] */ DBG_ATTRIB_FLAGS dwAttribFilter,
            /* [full][in] */ __RPC__in_opt LPCOLESTR pszNameFilter,
            /* [in] */ DWORD dwTimeout,
            /* [out] */ __RPC__deref_out_opt IEnumDebugReferenceInfo2 **ppEnum);
        
        HRESULT ( STDMETHODCALLTYPE *GetParent )( 
            IDebugReference2 * This,
            /* [out] */ __RPC__deref_out_opt IDebugReference2 **ppParent);
        
        HRESULT ( STDMETHODCALLTYPE *GetDerivedMostReference )( 
            IDebugReference2 * This,
            /* [out] */ __RPC__deref_out_opt IDebugReference2 **ppDerivedMost);
        
        HRESULT ( STDMETHODCALLTYPE *GetMemoryBytes )( 
            IDebugReference2 * This,
            /* [out] */ __RPC__deref_out_opt IDebugMemoryBytes2 **ppMemoryBytes);
        
        HRESULT ( STDMETHODCALLTYPE *GetMemoryContext )( 
            IDebugReference2 * This,
            /* [out] */ __RPC__deref_out_opt IDebugMemoryContext2 **ppMemory);
        
        HRESULT ( STDMETHODCALLTYPE *GetSize )( 
            IDebugReference2 * This,
            /* [out] */ __RPC__out DWORD *pdwSize);
        
        HRESULT ( STDMETHODCALLTYPE *SetReferenceType )( 
            IDebugReference2 * This,
            /* [in] */ REFERENCE_TYPE dwRefType);
        
        HRESULT ( STDMETHODCALLTYPE *Compare )( 
            IDebugReference2 * This,
            /* [in] */ REFERENCE_COMPARE dwCompare,
            /* [in] */ __RPC__in_opt IDebugReference2 *pReference);
        
        END_INTERFACE
    } IDebugReference2Vtbl;

    interface IDebugReference2
    {
        CONST_VTBL struct IDebugReference2Vtbl *lpVtbl;
    };

    

#ifdef COBJMACROS


#define IDebugReference2_QueryInterface(This,riid,ppvObject)	\
    ( (This)->lpVtbl -> QueryInterface(This,riid,ppvObject) ) 

#define IDebugReference2_AddRef(This)	\
    ( (This)->lpVtbl -> AddRef(This) ) 

#define IDebugReference2_Release(This)	\
    ( (This)->lpVtbl -> Release(This) ) 


#define IDebugReference2_GetReferenceInfo(This,dwFields,dwRadix,dwTimeout,rgpArgs,dwArgCount,pReferenceInfo)	\
    ( (This)->lpVtbl -> GetReferenceInfo(This,dwFields,dwRadix,dwTimeout,rgpArgs,dwArgCount,pReferenceInfo) ) 

#define IDebugReference2_SetValueAsString(This,pszValue,dwRadix,dwTimeout)	\
    ( (This)->lpVtbl -> SetValueAsString(This,pszValue,dwRadix,dwTimeout) ) 

#define IDebugReference2_SetValueAsReference(This,rgpArgs,dwArgCount,pValue,dwTimeout)	\
    ( (This)->lpVtbl -> SetValueAsReference(This,rgpArgs,dwArgCount,pValue,dwTimeout) ) 

#define IDebugReference2_EnumChildren(This,dwFields,dwRadix,dwAttribFilter,pszNameFilter,dwTimeout,ppEnum)	\
    ( (This)->lpVtbl -> EnumChildren(This,dwFields,dwRadix,dwAttribFilter,pszNameFilter,dwTimeout,ppEnum) ) 

#define IDebugReference2_GetParent(This,ppParent)	\
    ( (This)->lpVtbl -> GetParent(This,ppParent) ) 

#define IDebugReference2_GetDerivedMostReference(This,ppDerivedMost)	\
    ( (This)->lpVtbl -> GetDerivedMostReference(This,ppDerivedMost) ) 

#define IDebugReference2_GetMemoryBytes(This,ppMemoryBytes)	\
    ( (This)->lpVtbl -> GetMemoryBytes(This,ppMemoryBytes) ) 

#define IDebugReference2_GetMemoryContext(This,ppMemory)	\
    ( (This)->lpVtbl -> GetMemoryContext(This,ppMemory) ) 

#define IDebugReference2_GetSize(This,pdwSize)	\
    ( (This)->lpVtbl -> GetSize(This,pdwSize) ) 

#define IDebugReference2_SetReferenceType(This,dwRefType)	\
    ( (This)->lpVtbl -> SetReferenceType(This,dwRefType) ) 

#define IDebugReference2_Compare(This,dwCompare,pReference)	\
    ( (This)->lpVtbl -> Compare(This,dwCompare,pReference) ) 

#endif /* COBJMACROS */


#endif 	/* C style interface */




#endif 	/* __IDebugReference2_INTERFACE_DEFINED__ */


#ifndef __IDebugStackFrame2_INTERFACE_DEFINED__
#define __IDebugStackFrame2_INTERFACE_DEFINED__

/* interface IDebugStackFrame2 */
/* [unique][uuid][object] */ 


EXTERN_C const IID IID_IDebugStackFrame2;

#if defined(__cplusplus) && !defined(CINTERFACE)
    
    MIDL_INTERFACE("1412926f-5dd6-4e58-b648-e1c63e013d51")
    IDebugStackFrame2 : public IUnknown
    {
    public:
        virtual HRESULT STDMETHODCALLTYPE GetCodeContext( 
            /* [out] */ __RPC__deref_out_opt IDebugCodeContext2 **ppCodeCxt) = 0;
        
        virtual HRESULT STDMETHODCALLTYPE GetDocumentContext( 
            /* [out] */ __RPC__deref_out_opt IDebugDocumentContext2 **ppCxt) = 0;
        
        virtual HRESULT STDMETHODCALLTYPE GetName( 
            /* [out] */ __RPC__deref_out_opt BSTR *pbstrName) = 0;
        
        virtual HRESULT STDMETHODCALLTYPE GetInfo( 
            /* [in] */ FRAMEINFO_FLAGS dwFieldSpec,
            /* [in] */ UINT nRadix,
            /* [out] */ __RPC__out FRAMEINFO *pFrameInfo) = 0;
        
        virtual HRESULT STDMETHODCALLTYPE GetPhysicalStackRange( 
            /* [out] */ __RPC__out UINT64 *paddrMin,
            /* [out] */ __RPC__out UINT64 *paddrMax) = 0;
        
        virtual HRESULT STDMETHODCALLTYPE GetExpressionContext( 
            /* [out] */ __RPC__deref_out_opt IDebugExpressionContext2 **ppExprCxt) = 0;
        
        virtual HRESULT STDMETHODCALLTYPE GetLanguageInfo( 
            /* [full][out][in] */ __RPC__deref_opt_inout_opt BSTR *pbstrLanguage,
            /* [full][out][in] */ __RPC__inout_opt GUID *pguidLanguage) = 0;
        
        virtual HRESULT STDMETHODCALLTYPE GetDebugProperty( 
            /* [out] */ __RPC__deref_out_opt IDebugProperty2 **ppProperty) = 0;
        
        virtual HRESULT STDMETHODCALLTYPE EnumProperties( 
            /* [in] */ DEBUGPROP_INFO_FLAGS dwFields,
            /* [in] */ UINT nRadix,
            /* [in] */ __RPC__in REFGUID guidFilter,
            /* [in] */ DWORD dwTimeout,
            /* [out] */ __RPC__out ULONG *pcelt,
            /* [out] */ __RPC__deref_out_opt IEnumDebugPropertyInfo2 **ppEnum) = 0;
        
        virtual HRESULT STDMETHODCALLTYPE GetThread( 
            /* [out] */ __RPC__deref_out_opt IDebugThread2 **ppThread) = 0;
        
    };
    
#else 	/* C style interface */

    typedef struct IDebugStackFrame2Vtbl
    {
        BEGIN_INTERFACE
        
        HRESULT ( STDMETHODCALLTYPE *QueryInterface )( 
            IDebugStackFrame2 * This,
            /* [in] */ __RPC__in REFIID riid,
            /* [iid_is][out] */ 
            __RPC__deref_out  void **ppvObject);
        
        ULONG ( STDMETHODCALLTYPE *AddRef )( 
            IDebugStackFrame2 * This);
        
        ULONG ( STDMETHODCALLTYPE *Release )( 
            IDebugStackFrame2 * This);
        
        HRESULT ( STDMETHODCALLTYPE *GetCodeContext )( 
            IDebugStackFrame2 * This,
            /* [out] */ __RPC__deref_out_opt IDebugCodeContext2 **ppCodeCxt);
        
        HRESULT ( STDMETHODCALLTYPE *GetDocumentContext )( 
            IDebugStackFrame2 * This,
            /* [out] */ __RPC__deref_out_opt IDebugDocumentContext2 **ppCxt);
        
        HRESULT ( STDMETHODCALLTYPE *GetName )( 
            IDebugStackFrame2 * This,
            /* [out] */ __RPC__deref_out_opt BSTR *pbstrName);
        
        HRESULT ( STDMETHODCALLTYPE *GetInfo )( 
            IDebugStackFrame2 * This,
            /* [in] */ FRAMEINFO_FLAGS dwFieldSpec,
            /* [in] */ UINT nRadix,
            /* [out] */ __RPC__out FRAMEINFO *pFrameInfo);
        
        HRESULT ( STDMETHODCALLTYPE *GetPhysicalStackRange )( 
            IDebugStackFrame2 * This,
            /* [out] */ __RPC__out UINT64 *paddrMin,
            /* [out] */ __RPC__out UINT64 *paddrMax);
        
        HRESULT ( STDMETHODCALLTYPE *GetExpressionContext )( 
            IDebugStackFrame2 * This,
            /* [out] */ __RPC__deref_out_opt IDebugExpressionContext2 **ppExprCxt);
        
        HRESULT ( STDMETHODCALLTYPE *GetLanguageInfo )( 
            IDebugStackFrame2 * This,
            /* [full][out][in] */ __RPC__deref_opt_inout_opt BSTR *pbstrLanguage,
            /* [full][out][in] */ __RPC__inout_opt GUID *pguidLanguage);
        
        HRESULT ( STDMETHODCALLTYPE *GetDebugProperty )( 
            IDebugStackFrame2 * This,
            /* [out] */ __RPC__deref_out_opt IDebugProperty2 **ppProperty);
        
        HRESULT ( STDMETHODCALLTYPE *EnumProperties )( 
            IDebugStackFrame2 * This,
            /* [in] */ DEBUGPROP_INFO_FLAGS dwFields,
            /* [in] */ UINT nRadix,
            /* [in] */ __RPC__in REFGUID guidFilter,
            /* [in] */ DWORD dwTimeout,
            /* [out] */ __RPC__out ULONG *pcelt,
            /* [out] */ __RPC__deref_out_opt IEnumDebugPropertyInfo2 **ppEnum);
        
        HRESULT ( STDMETHODCALLTYPE *GetThread )( 
            IDebugStackFrame2 * This,
            /* [out] */ __RPC__deref_out_opt IDebugThread2 **ppThread);
        
        END_INTERFACE
    } IDebugStackFrame2Vtbl;

    interface IDebugStackFrame2
    {
        CONST_VTBL struct IDebugStackFrame2Vtbl *lpVtbl;
    };

    

#ifdef COBJMACROS


#define IDebugStackFrame2_QueryInterface(This,riid,ppvObject)	\
    ( (This)->lpVtbl -> QueryInterface(This,riid,ppvObject) ) 

#define IDebugStackFrame2_AddRef(This)	\
    ( (This)->lpVtbl -> AddRef(This) ) 

#define IDebugStackFrame2_Release(This)	\
    ( (This)->lpVtbl -> Release(This) ) 


#define IDebugStackFrame2_GetCodeContext(This,ppCodeCxt)	\
    ( (This)->lpVtbl -> GetCodeContext(This,ppCodeCxt) ) 

#define IDebugStackFrame2_GetDocumentContext(This,ppCxt)	\
    ( (This)->lpVtbl -> GetDocumentContext(This,ppCxt) ) 

#define IDebugStackFrame2_GetName(This,pbstrName)	\
    ( (This)->lpVtbl -> GetName(This,pbstrName) ) 

#define IDebugStackFrame2_GetInfo(This,dwFieldSpec,nRadix,pFrameInfo)	\
    ( (This)->lpVtbl -> GetInfo(This,dwFieldSpec,nRadix,pFrameInfo) ) 

#define IDebugStackFrame2_GetPhysicalStackRange(This,paddrMin,paddrMax)	\
    ( (This)->lpVtbl -> GetPhysicalStackRange(This,paddrMin,paddrMax) ) 

#define IDebugStackFrame2_GetExpressionContext(This,ppExprCxt)	\
    ( (This)->lpVtbl -> GetExpressionContext(This,ppExprCxt) ) 

#define IDebugStackFrame2_GetLanguageInfo(This,pbstrLanguage,pguidLanguage)	\
    ( (This)->lpVtbl -> GetLanguageInfo(This,pbstrLanguage,pguidLanguage) ) 

#define IDebugStackFrame2_GetDebugProperty(This,ppProperty)	\
    ( (This)->lpVtbl -> GetDebugProperty(This,ppProperty) ) 

#define IDebugStackFrame2_EnumProperties(This,dwFields,nRadix,guidFilter,dwTimeout,pcelt,ppEnum)	\
    ( (This)->lpVtbl -> EnumProperties(This,dwFields,nRadix,guidFilter,dwTimeout,pcelt,ppEnum) ) 

#define IDebugStackFrame2_GetThread(This,ppThread)	\
    ( (This)->lpVtbl -> GetThread(This,ppThread) ) 

#endif /* COBJMACROS */


#endif 	/* C style interface */




#endif 	/* __IDebugStackFrame2_INTERFACE_DEFINED__ */


#ifndef __IDebugStackFrame3_INTERFACE_DEFINED__
#define __IDebugStackFrame3_INTERFACE_DEFINED__

/* interface IDebugStackFrame3 */
/* [unique][uuid][object] */ 


enum enum_INTERCEPT_EXCEPTION_ACTION
    {	IEA_INTERCEPT	= 0x1,
	IEA_CANCEL_INTERCEPT	= 0
    } ;
typedef DWORD INTERCEPT_EXCEPTION_ACTION;


EXTERN_C const IID IID_IDebugStackFrame3;

#if defined(__cplusplus) && !defined(CINTERFACE)
    
    MIDL_INTERFACE("60DE844B-38B1-4d87-AFE1-8CF49677D3B0")
    IDebugStackFrame3 : public IDebugStackFrame2
    {
    public:
        virtual HRESULT STDMETHODCALLTYPE InterceptCurrentException( 
            /* [in] */ INTERCEPT_EXCEPTION_ACTION dwFlags,
            /* [out] */ __RPC__out UINT64 *pqwCookie) = 0;
        
        virtual HRESULT STDMETHODCALLTYPE GetUnwindCodeContext( 
            /* [out] */ __RPC__deref_out_opt IDebugCodeContext2 **ppCodeContext) = 0;
        
    };
    
#else 	/* C style interface */

    typedef struct IDebugStackFrame3Vtbl
    {
        BEGIN_INTERFACE
        
        HRESULT ( STDMETHODCALLTYPE *QueryInterface )( 
            IDebugStackFrame3 * This,
            /* [in] */ __RPC__in REFIID riid,
            /* [iid_is][out] */ 
            __RPC__deref_out  void **ppvObject);
        
        ULONG ( STDMETHODCALLTYPE *AddRef )( 
            IDebugStackFrame3 * This);
        
        ULONG ( STDMETHODCALLTYPE *Release )( 
            IDebugStackFrame3 * This);
        
        HRESULT ( STDMETHODCALLTYPE *GetCodeContext )( 
            IDebugStackFrame3 * This,
            /* [out] */ __RPC__deref_out_opt IDebugCodeContext2 **ppCodeCxt);
        
        HRESULT ( STDMETHODCALLTYPE *GetDocumentContext )( 
            IDebugStackFrame3 * This,
            /* [out] */ __RPC__deref_out_opt IDebugDocumentContext2 **ppCxt);
        
        HRESULT ( STDMETHODCALLTYPE *GetName )( 
            IDebugStackFrame3 * This,
            /* [out] */ __RPC__deref_out_opt BSTR *pbstrName);
        
        HRESULT ( STDMETHODCALLTYPE *GetInfo )( 
            IDebugStackFrame3 * This,
            /* [in] */ FRAMEINFO_FLAGS dwFieldSpec,
            /* [in] */ UINT nRadix,
            /* [out] */ __RPC__out FRAMEINFO *pFrameInfo);
        
        HRESULT ( STDMETHODCALLTYPE *GetPhysicalStackRange )( 
            IDebugStackFrame3 * This,
            /* [out] */ __RPC__out UINT64 *paddrMin,
            /* [out] */ __RPC__out UINT64 *paddrMax);
        
        HRESULT ( STDMETHODCALLTYPE *GetExpressionContext )( 
            IDebugStackFrame3 * This,
            /* [out] */ __RPC__deref_out_opt IDebugExpressionContext2 **ppExprCxt);
        
        HRESULT ( STDMETHODCALLTYPE *GetLanguageInfo )( 
            IDebugStackFrame3 * This,
            /* [full][out][in] */ __RPC__deref_opt_inout_opt BSTR *pbstrLanguage,
            /* [full][out][in] */ __RPC__inout_opt GUID *pguidLanguage);
        
        HRESULT ( STDMETHODCALLTYPE *GetDebugProperty )( 
            IDebugStackFrame3 * This,
            /* [out] */ __RPC__deref_out_opt IDebugProperty2 **ppProperty);
        
        HRESULT ( STDMETHODCALLTYPE *EnumProperties )( 
            IDebugStackFrame3 * This,
            /* [in] */ DEBUGPROP_INFO_FLAGS dwFields,
            /* [in] */ UINT nRadix,
            /* [in] */ __RPC__in REFGUID guidFilter,
            /* [in] */ DWORD dwTimeout,
            /* [out] */ __RPC__out ULONG *pcelt,
            /* [out] */ __RPC__deref_out_opt IEnumDebugPropertyInfo2 **ppEnum);
        
        HRESULT ( STDMETHODCALLTYPE *GetThread )( 
            IDebugStackFrame3 * This,
            /* [out] */ __RPC__deref_out_opt IDebugThread2 **ppThread);
        
        HRESULT ( STDMETHODCALLTYPE *InterceptCurrentException )( 
            IDebugStackFrame3 * This,
            /* [in] */ INTERCEPT_EXCEPTION_ACTION dwFlags,
            /* [out] */ __RPC__out UINT64 *pqwCookie);
        
        HRESULT ( STDMETHODCALLTYPE *GetUnwindCodeContext )( 
            IDebugStackFrame3 * This,
            /* [out] */ __RPC__deref_out_opt IDebugCodeContext2 **ppCodeContext);
        
        END_INTERFACE
    } IDebugStackFrame3Vtbl;

    interface IDebugStackFrame3
    {
        CONST_VTBL struct IDebugStackFrame3Vtbl *lpVtbl;
    };

    

#ifdef COBJMACROS


#define IDebugStackFrame3_QueryInterface(This,riid,ppvObject)	\
    ( (This)->lpVtbl -> QueryInterface(This,riid,ppvObject) ) 

#define IDebugStackFrame3_AddRef(This)	\
    ( (This)->lpVtbl -> AddRef(This) ) 

#define IDebugStackFrame3_Release(This)	\
    ( (This)->lpVtbl -> Release(This) ) 


#define IDebugStackFrame3_GetCodeContext(This,ppCodeCxt)	\
    ( (This)->lpVtbl -> GetCodeContext(This,ppCodeCxt) ) 

#define IDebugStackFrame3_GetDocumentContext(This,ppCxt)	\
    ( (This)->lpVtbl -> GetDocumentContext(This,ppCxt) ) 

#define IDebugStackFrame3_GetName(This,pbstrName)	\
    ( (This)->lpVtbl -> GetName(This,pbstrName) ) 

#define IDebugStackFrame3_GetInfo(This,dwFieldSpec,nRadix,pFrameInfo)	\
    ( (This)->lpVtbl -> GetInfo(This,dwFieldSpec,nRadix,pFrameInfo) ) 

#define IDebugStackFrame3_GetPhysicalStackRange(This,paddrMin,paddrMax)	\
    ( (This)->lpVtbl -> GetPhysicalStackRange(This,paddrMin,paddrMax) ) 

#define IDebugStackFrame3_GetExpressionContext(This,ppExprCxt)	\
    ( (This)->lpVtbl -> GetExpressionContext(This,ppExprCxt) ) 

#define IDebugStackFrame3_GetLanguageInfo(This,pbstrLanguage,pguidLanguage)	\
    ( (This)->lpVtbl -> GetLanguageInfo(This,pbstrLanguage,pguidLanguage) ) 

#define IDebugStackFrame3_GetDebugProperty(This,ppProperty)	\
    ( (This)->lpVtbl -> GetDebugProperty(This,ppProperty) ) 

#define IDebugStackFrame3_EnumProperties(This,dwFields,nRadix,guidFilter,dwTimeout,pcelt,ppEnum)	\
    ( (This)->lpVtbl -> EnumProperties(This,dwFields,nRadix,guidFilter,dwTimeout,pcelt,ppEnum) ) 

#define IDebugStackFrame3_GetThread(This,ppThread)	\
    ( (This)->lpVtbl -> GetThread(This,ppThread) ) 


#define IDebugStackFrame3_InterceptCurrentException(This,dwFlags,pqwCookie)	\
    ( (This)->lpVtbl -> InterceptCurrentException(This,dwFlags,pqwCookie) ) 

#define IDebugStackFrame3_GetUnwindCodeContext(This,ppCodeContext)	\
    ( (This)->lpVtbl -> GetUnwindCodeContext(This,ppCodeContext) ) 

#endif /* COBJMACROS */


#endif 	/* C style interface */




#endif 	/* __IDebugStackFrame3_INTERFACE_DEFINED__ */


#ifndef __IDebugMemoryContext2_INTERFACE_DEFINED__
#define __IDebugMemoryContext2_INTERFACE_DEFINED__

/* interface IDebugMemoryContext2 */
/* [unique][uuid][object] */ 


enum enum_CONTEXT_COMPARE
    {	CONTEXT_EQUAL	= 0x1,
	CONTEXT_LESS_THAN	= 0x2,
	CONTEXT_GREATER_THAN	= 0x3,
	CONTEXT_LESS_THAN_OR_EQUAL	= 0x4,
	CONTEXT_GREATER_THAN_OR_EQUAL	= 0x5,
	CONTEXT_SAME_SCOPE	= 0x6,
	CONTEXT_SAME_FUNCTION	= 0x7,
	CONTEXT_SAME_MODULE	= 0x8,
	CONTEXT_SAME_PROCESS	= 0x9
    } ;
typedef DWORD CONTEXT_COMPARE;


enum enum_CONTEXT_INFO_FIELDS
    {	CIF_MODULEURL	= 0x1,
	CIF_FUNCTION	= 0x2,
	CIF_FUNCTIONOFFSET	= 0x4,
	CIF_ADDRESS	= 0x8,
	CIF_ADDRESSOFFSET	= 0x10,
	CIF_ADDRESSABSOLUTE	= 0x20,
	CIF_ALLFIELDS	= 0x3f
    } ;
typedef DWORD CONTEXT_INFO_FIELDS;

typedef struct _tagCONTEXT_INFO
    {
    CONTEXT_INFO_FIELDS dwFields;
    BSTR bstrModuleUrl;
    BSTR bstrFunction;
    TEXT_POSITION posFunctionOffset;
    BSTR bstrAddress;
    BSTR bstrAddressOffset;
    BSTR bstrAddressAbsolute;
    } 	CONTEXT_INFO;


EXTERN_C const IID IID_IDebugMemoryContext2;

#if defined(__cplusplus) && !defined(CINTERFACE)
    
    MIDL_INTERFACE("1ab276dd-f27b-4445-825d-5df0b4a04a3a")
    IDebugMemoryContext2 : public IUnknown
    {
    public:
        virtual HRESULT STDMETHODCALLTYPE GetName( 
            /* [out] */ __RPC__deref_out_opt BSTR *pbstrName) = 0;
        
        virtual HRESULT STDMETHODCALLTYPE GetInfo( 
            /* [in] */ CONTEXT_INFO_FIELDS dwFields,
            /* [out] */ __RPC__out CONTEXT_INFO *pInfo) = 0;
        
        virtual HRESULT STDMETHODCALLTYPE Add( 
            /* [in] */ UINT64 dwCount,
            /* [out] */ __RPC__deref_out_opt IDebugMemoryContext2 **ppMemCxt) = 0;
        
        virtual HRESULT STDMETHODCALLTYPE Subtract( 
            /* [in] */ UINT64 dwCount,
            /* [out] */ __RPC__deref_out_opt IDebugMemoryContext2 **ppMemCxt) = 0;
        
        virtual HRESULT STDMETHODCALLTYPE Compare( 
            /* [in] */ CONTEXT_COMPARE compare,
            /* [length_is][size_is][in] */ __RPC__in_ecount_part(dwMemoryContextSetLen, dwMemoryContextSetLen) IDebugMemoryContext2 **rgpMemoryContextSet,
            /* [in] */ DWORD dwMemoryContextSetLen,
            /* [out] */ __RPC__out DWORD *pdwMemoryContext) = 0;
        
    };
    
#else 	/* C style interface */

    typedef struct IDebugMemoryContext2Vtbl
    {
        BEGIN_INTERFACE
        
        HRESULT ( STDMETHODCALLTYPE *QueryInterface )( 
            IDebugMemoryContext2 * This,
            /* [in] */ __RPC__in REFIID riid,
            /* [iid_is][out] */ 
            __RPC__deref_out  void **ppvObject);
        
        ULONG ( STDMETHODCALLTYPE *AddRef )( 
            IDebugMemoryContext2 * This);
        
        ULONG ( STDMETHODCALLTYPE *Release )( 
            IDebugMemoryContext2 * This);
        
        HRESULT ( STDMETHODCALLTYPE *GetName )( 
            IDebugMemoryContext2 * This,
            /* [out] */ __RPC__deref_out_opt BSTR *pbstrName);
        
        HRESULT ( STDMETHODCALLTYPE *GetInfo )( 
            IDebugMemoryContext2 * This,
            /* [in] */ CONTEXT_INFO_FIELDS dwFields,
            /* [out] */ __RPC__out CONTEXT_INFO *pInfo);
        
        HRESULT ( STDMETHODCALLTYPE *Add )( 
            IDebugMemoryContext2 * This,
            /* [in] */ UINT64 dwCount,
            /* [out] */ __RPC__deref_out_opt IDebugMemoryContext2 **ppMemCxt);
        
        HRESULT ( STDMETHODCALLTYPE *Subtract )( 
            IDebugMemoryContext2 * This,
            /* [in] */ UINT64 dwCount,
            /* [out] */ __RPC__deref_out_opt IDebugMemoryContext2 **ppMemCxt);
        
        HRESULT ( STDMETHODCALLTYPE *Compare )( 
            IDebugMemoryContext2 * This,
            /* [in] */ CONTEXT_COMPARE compare,
            /* [length_is][size_is][in] */ __RPC__in_ecount_part(dwMemoryContextSetLen, dwMemoryContextSetLen) IDebugMemoryContext2 **rgpMemoryContextSet,
            /* [in] */ DWORD dwMemoryContextSetLen,
            /* [out] */ __RPC__out DWORD *pdwMemoryContext);
        
        END_INTERFACE
    } IDebugMemoryContext2Vtbl;

    interface IDebugMemoryContext2
    {
        CONST_VTBL struct IDebugMemoryContext2Vtbl *lpVtbl;
    };

    

#ifdef COBJMACROS


#define IDebugMemoryContext2_QueryInterface(This,riid,ppvObject)	\
    ( (This)->lpVtbl -> QueryInterface(This,riid,ppvObject) ) 

#define IDebugMemoryContext2_AddRef(This)	\
    ( (This)->lpVtbl -> AddRef(This) ) 

#define IDebugMemoryContext2_Release(This)	\
    ( (This)->lpVtbl -> Release(This) ) 


#define IDebugMemoryContext2_GetName(This,pbstrName)	\
    ( (This)->lpVtbl -> GetName(This,pbstrName) ) 

#define IDebugMemoryContext2_GetInfo(This,dwFields,pInfo)	\
    ( (This)->lpVtbl -> GetInfo(This,dwFields,pInfo) ) 

#define IDebugMemoryContext2_Add(This,dwCount,ppMemCxt)	\
    ( (This)->lpVtbl -> Add(This,dwCount,ppMemCxt) ) 

#define IDebugMemoryContext2_Subtract(This,dwCount,ppMemCxt)	\
    ( (This)->lpVtbl -> Subtract(This,dwCount,ppMemCxt) ) 

#define IDebugMemoryContext2_Compare(This,compare,rgpMemoryContextSet,dwMemoryContextSetLen,pdwMemoryContext)	\
    ( (This)->lpVtbl -> Compare(This,compare,rgpMemoryContextSet,dwMemoryContextSetLen,pdwMemoryContext) ) 

#endif /* COBJMACROS */


#endif 	/* C style interface */




#endif 	/* __IDebugMemoryContext2_INTERFACE_DEFINED__ */


#ifndef __IDebugCodeContext2_INTERFACE_DEFINED__
#define __IDebugCodeContext2_INTERFACE_DEFINED__

/* interface IDebugCodeContext2 */
/* [unique][uuid][object] */ 


EXTERN_C const IID IID_IDebugCodeContext2;

#if defined(__cplusplus) && !defined(CINTERFACE)
    
    MIDL_INTERFACE("ac17b76b-2b09-419a-ad5f-7d7402da8875")
    IDebugCodeContext2 : public IDebugMemoryContext2
    {
    public:
        virtual HRESULT STDMETHODCALLTYPE GetDocumentContext( 
            /* [out] */ __RPC__deref_out_opt IDebugDocumentContext2 **ppSrcCxt) = 0;
        
        virtual HRESULT STDMETHODCALLTYPE GetLanguageInfo( 
            /* [full][out][in] */ __RPC__deref_opt_inout_opt BSTR *pbstrLanguage,
            /* [full][out][in] */ __RPC__inout_opt GUID *pguidLanguage) = 0;
        
    };
    
#else 	/* C style interface */

    typedef struct IDebugCodeContext2Vtbl
    {
        BEGIN_INTERFACE
        
        HRESULT ( STDMETHODCALLTYPE *QueryInterface )( 
            IDebugCodeContext2 * This,
            /* [in] */ __RPC__in REFIID riid,
            /* [iid_is][out] */ 
            __RPC__deref_out  void **ppvObject);
        
        ULONG ( STDMETHODCALLTYPE *AddRef )( 
            IDebugCodeContext2 * This);
        
        ULONG ( STDMETHODCALLTYPE *Release )( 
            IDebugCodeContext2 * This);
        
        HRESULT ( STDMETHODCALLTYPE *GetName )( 
            IDebugCodeContext2 * This,
            /* [out] */ __RPC__deref_out_opt BSTR *pbstrName);
        
        HRESULT ( STDMETHODCALLTYPE *GetInfo )( 
            IDebugCodeContext2 * This,
            /* [in] */ CONTEXT_INFO_FIELDS dwFields,
            /* [out] */ __RPC__out CONTEXT_INFO *pInfo);
        
        HRESULT ( STDMETHODCALLTYPE *Add )( 
            IDebugCodeContext2 * This,
            /* [in] */ UINT64 dwCount,
            /* [out] */ __RPC__deref_out_opt IDebugMemoryContext2 **ppMemCxt);
        
        HRESULT ( STDMETHODCALLTYPE *Subtract )( 
            IDebugCodeContext2 * This,
            /* [in] */ UINT64 dwCount,
            /* [out] */ __RPC__deref_out_opt IDebugMemoryContext2 **ppMemCxt);
        
        HRESULT ( STDMETHODCALLTYPE *Compare )( 
            IDebugCodeContext2 * This,
            /* [in] */ CONTEXT_COMPARE compare,
            /* [length_is][size_is][in] */ __RPC__in_ecount_part(dwMemoryContextSetLen, dwMemoryContextSetLen) IDebugMemoryContext2 **rgpMemoryContextSet,
            /* [in] */ DWORD dwMemoryContextSetLen,
            /* [out] */ __RPC__out DWORD *pdwMemoryContext);
        
        HRESULT ( STDMETHODCALLTYPE *GetDocumentContext )( 
            IDebugCodeContext2 * This,
            /* [out] */ __RPC__deref_out_opt IDebugDocumentContext2 **ppSrcCxt);
        
        HRESULT ( STDMETHODCALLTYPE *GetLanguageInfo )( 
            IDebugCodeContext2 * This,
            /* [full][out][in] */ __RPC__deref_opt_inout_opt BSTR *pbstrLanguage,
            /* [full][out][in] */ __RPC__inout_opt GUID *pguidLanguage);
        
        END_INTERFACE
    } IDebugCodeContext2Vtbl;

    interface IDebugCodeContext2
    {
        CONST_VTBL struct IDebugCodeContext2Vtbl *lpVtbl;
    };

    

#ifdef COBJMACROS


#define IDebugCodeContext2_QueryInterface(This,riid,ppvObject)	\
    ( (This)->lpVtbl -> QueryInterface(This,riid,ppvObject) ) 

#define IDebugCodeContext2_AddRef(This)	\
    ( (This)->lpVtbl -> AddRef(This) ) 

#define IDebugCodeContext2_Release(This)	\
    ( (This)->lpVtbl -> Release(This) ) 


#define IDebugCodeContext2_GetName(This,pbstrName)	\
    ( (This)->lpVtbl -> GetName(This,pbstrName) ) 

#define IDebugCodeContext2_GetInfo(This,dwFields,pInfo)	\
    ( (This)->lpVtbl -> GetInfo(This,dwFields,pInfo) ) 

#define IDebugCodeContext2_Add(This,dwCount,ppMemCxt)	\
    ( (This)->lpVtbl -> Add(This,dwCount,ppMemCxt) ) 

#define IDebugCodeContext2_Subtract(This,dwCount,ppMemCxt)	\
    ( (This)->lpVtbl -> Subtract(This,dwCount,ppMemCxt) ) 

#define IDebugCodeContext2_Compare(This,compare,rgpMemoryContextSet,dwMemoryContextSetLen,pdwMemoryContext)	\
    ( (This)->lpVtbl -> Compare(This,compare,rgpMemoryContextSet,dwMemoryContextSetLen,pdwMemoryContext) ) 


#define IDebugCodeContext2_GetDocumentContext(This,ppSrcCxt)	\
    ( (This)->lpVtbl -> GetDocumentContext(This,ppSrcCxt) ) 

#define IDebugCodeContext2_GetLanguageInfo(This,pbstrLanguage,pguidLanguage)	\
    ( (This)->lpVtbl -> GetLanguageInfo(This,pbstrLanguage,pguidLanguage) ) 

#endif /* COBJMACROS */


#endif 	/* C style interface */




#endif 	/* __IDebugCodeContext2_INTERFACE_DEFINED__ */


#ifndef __IDebugCodeContext3_INTERFACE_DEFINED__
#define __IDebugCodeContext3_INTERFACE_DEFINED__

/* interface IDebugCodeContext3 */
/* [unique][uuid][object] */ 


EXTERN_C const IID IID_IDebugCodeContext3;

#if defined(__cplusplus) && !defined(CINTERFACE)
    
    MIDL_INTERFACE("17c106b9-0925-42f5-ae32-1fc019649c10")
    IDebugCodeContext3 : public IDebugCodeContext2
    {
    public:
        virtual HRESULT STDMETHODCALLTYPE GetModule( 
            /* [out] */ __RPC__deref_out_opt IDebugModule2 **ppModule) = 0;
        
        virtual HRESULT STDMETHODCALLTYPE GetProcess( 
            /* [out] */ __RPC__deref_out_opt IDebugProcess2 **ppProcess) = 0;
        
    };
    
#else 	/* C style interface */

    typedef struct IDebugCodeContext3Vtbl
    {
        BEGIN_INTERFACE
        
        HRESULT ( STDMETHODCALLTYPE *QueryInterface )( 
            IDebugCodeContext3 * This,
            /* [in] */ __RPC__in REFIID riid,
            /* [iid_is][out] */ 
            __RPC__deref_out  void **ppvObject);
        
        ULONG ( STDMETHODCALLTYPE *AddRef )( 
            IDebugCodeContext3 * This);
        
        ULONG ( STDMETHODCALLTYPE *Release )( 
            IDebugCodeContext3 * This);
        
        HRESULT ( STDMETHODCALLTYPE *GetName )( 
            IDebugCodeContext3 * This,
            /* [out] */ __RPC__deref_out_opt BSTR *pbstrName);
        
        HRESULT ( STDMETHODCALLTYPE *GetInfo )( 
            IDebugCodeContext3 * This,
            /* [in] */ CONTEXT_INFO_FIELDS dwFields,
            /* [out] */ __RPC__out CONTEXT_INFO *pInfo);
        
        HRESULT ( STDMETHODCALLTYPE *Add )( 
            IDebugCodeContext3 * This,
            /* [in] */ UINT64 dwCount,
            /* [out] */ __RPC__deref_out_opt IDebugMemoryContext2 **ppMemCxt);
        
        HRESULT ( STDMETHODCALLTYPE *Subtract )( 
            IDebugCodeContext3 * This,
            /* [in] */ UINT64 dwCount,
            /* [out] */ __RPC__deref_out_opt IDebugMemoryContext2 **ppMemCxt);
        
        HRESULT ( STDMETHODCALLTYPE *Compare )( 
            IDebugCodeContext3 * This,
            /* [in] */ CONTEXT_COMPARE compare,
            /* [length_is][size_is][in] */ __RPC__in_ecount_part(dwMemoryContextSetLen, dwMemoryContextSetLen) IDebugMemoryContext2 **rgpMemoryContextSet,
            /* [in] */ DWORD dwMemoryContextSetLen,
            /* [out] */ __RPC__out DWORD *pdwMemoryContext);
        
        HRESULT ( STDMETHODCALLTYPE *GetDocumentContext )( 
            IDebugCodeContext3 * This,
            /* [out] */ __RPC__deref_out_opt IDebugDocumentContext2 **ppSrcCxt);
        
        HRESULT ( STDMETHODCALLTYPE *GetLanguageInfo )( 
            IDebugCodeContext3 * This,
            /* [full][out][in] */ __RPC__deref_opt_inout_opt BSTR *pbstrLanguage,
            /* [full][out][in] */ __RPC__inout_opt GUID *pguidLanguage);
        
        HRESULT ( STDMETHODCALLTYPE *GetModule )( 
            IDebugCodeContext3 * This,
            /* [out] */ __RPC__deref_out_opt IDebugModule2 **ppModule);
        
        HRESULT ( STDMETHODCALLTYPE *GetProcess )( 
            IDebugCodeContext3 * This,
            /* [out] */ __RPC__deref_out_opt IDebugProcess2 **ppProcess);
        
        END_INTERFACE
    } IDebugCodeContext3Vtbl;

    interface IDebugCodeContext3
    {
        CONST_VTBL struct IDebugCodeContext3Vtbl *lpVtbl;
    };

    

#ifdef COBJMACROS


#define IDebugCodeContext3_QueryInterface(This,riid,ppvObject)	\
    ( (This)->lpVtbl -> QueryInterface(This,riid,ppvObject) ) 

#define IDebugCodeContext3_AddRef(This)	\
    ( (This)->lpVtbl -> AddRef(This) ) 

#define IDebugCodeContext3_Release(This)	\
    ( (This)->lpVtbl -> Release(This) ) 


#define IDebugCodeContext3_GetName(This,pbstrName)	\
    ( (This)->lpVtbl -> GetName(This,pbstrName) ) 

#define IDebugCodeContext3_GetInfo(This,dwFields,pInfo)	\
    ( (This)->lpVtbl -> GetInfo(This,dwFields,pInfo) ) 

#define IDebugCodeContext3_Add(This,dwCount,ppMemCxt)	\
    ( (This)->lpVtbl -> Add(This,dwCount,ppMemCxt) ) 

#define IDebugCodeContext3_Subtract(This,dwCount,ppMemCxt)	\
    ( (This)->lpVtbl -> Subtract(This,dwCount,ppMemCxt) ) 

#define IDebugCodeContext3_Compare(This,compare,rgpMemoryContextSet,dwMemoryContextSetLen,pdwMemoryContext)	\
    ( (This)->lpVtbl -> Compare(This,compare,rgpMemoryContextSet,dwMemoryContextSetLen,pdwMemoryContext) ) 


#define IDebugCodeContext3_GetDocumentContext(This,ppSrcCxt)	\
    ( (This)->lpVtbl -> GetDocumentContext(This,ppSrcCxt) ) 

#define IDebugCodeContext3_GetLanguageInfo(This,pbstrLanguage,pguidLanguage)	\
    ( (This)->lpVtbl -> GetLanguageInfo(This,pbstrLanguage,pguidLanguage) ) 


#define IDebugCodeContext3_GetModule(This,ppModule)	\
    ( (This)->lpVtbl -> GetModule(This,ppModule) ) 

#define IDebugCodeContext3_GetProcess(This,ppProcess)	\
    ( (This)->lpVtbl -> GetProcess(This,ppProcess) ) 

#endif /* COBJMACROS */


#endif 	/* C style interface */




#endif 	/* __IDebugCodeContext3_INTERFACE_DEFINED__ */


#ifndef __IDebugMemoryBytes2_INTERFACE_DEFINED__
#define __IDebugMemoryBytes2_INTERFACE_DEFINED__

/* interface IDebugMemoryBytes2 */
/* [unique][uuid][object] */ 


EXTERN_C const IID IID_IDebugMemoryBytes2;

#if defined(__cplusplus) && !defined(CINTERFACE)
    
    MIDL_INTERFACE("925837d1-3aa1-451a-b7fe-cc04bb42cfb8")
    IDebugMemoryBytes2 : public IUnknown
    {
    public:
        virtual HRESULT STDMETHODCALLTYPE ReadAt( 
            /* [in] */ __RPC__in_opt IDebugMemoryContext2 *pStartContext,
            /* [in] */ DWORD dwCount,
            /* [length_is][size_is][out] */ __RPC__out_ecount_part(dwCount, *pdwRead) BYTE *rgbMemory,
            /* [out] */ __RPC__out DWORD *pdwRead,
            /* [full][out][in] */ __RPC__inout_opt DWORD *pdwUnreadable) = 0;
        
        virtual HRESULT STDMETHODCALLTYPE WriteAt( 
            /* [in] */ __RPC__in_opt IDebugMemoryContext2 *pStartContext,
            /* [in] */ DWORD dwCount,
            /* [length_is][size_is][in] */ __RPC__in_ecount_part(dwCount, dwCount) BYTE *rgbMemory) = 0;
        
        virtual HRESULT STDMETHODCALLTYPE GetSize( 
            /* [out] */ __RPC__out UINT64 *pqwSize) = 0;
        
    };
    
#else 	/* C style interface */

    typedef struct IDebugMemoryBytes2Vtbl
    {
        BEGIN_INTERFACE
        
        HRESULT ( STDMETHODCALLTYPE *QueryInterface )( 
            IDebugMemoryBytes2 * This,
            /* [in] */ __RPC__in REFIID riid,
            /* [iid_is][out] */ 
            __RPC__deref_out  void **ppvObject);
        
        ULONG ( STDMETHODCALLTYPE *AddRef )( 
            IDebugMemoryBytes2 * This);
        
        ULONG ( STDMETHODCALLTYPE *Release )( 
            IDebugMemoryBytes2 * This);
        
        HRESULT ( STDMETHODCALLTYPE *ReadAt )( 
            IDebugMemoryBytes2 * This,
            /* [in] */ __RPC__in_opt IDebugMemoryContext2 *pStartContext,
            /* [in] */ DWORD dwCount,
            /* [length_is][size_is][out] */ __RPC__out_ecount_part(dwCount, *pdwRead) BYTE *rgbMemory,
            /* [out] */ __RPC__out DWORD *pdwRead,
            /* [full][out][in] */ __RPC__inout_opt DWORD *pdwUnreadable);
        
        HRESULT ( STDMETHODCALLTYPE *WriteAt )( 
            IDebugMemoryBytes2 * This,
            /* [in] */ __RPC__in_opt IDebugMemoryContext2 *pStartContext,
            /* [in] */ DWORD dwCount,
            /* [length_is][size_is][in] */ __RPC__in_ecount_part(dwCount, dwCount) BYTE *rgbMemory);
        
        HRESULT ( STDMETHODCALLTYPE *GetSize )( 
            IDebugMemoryBytes2 * This,
            /* [out] */ __RPC__out UINT64 *pqwSize);
        
        END_INTERFACE
    } IDebugMemoryBytes2Vtbl;

    interface IDebugMemoryBytes2
    {
        CONST_VTBL struct IDebugMemoryBytes2Vtbl *lpVtbl;
    };

    

#ifdef COBJMACROS


#define IDebugMemoryBytes2_QueryInterface(This,riid,ppvObject)	\
    ( (This)->lpVtbl -> QueryInterface(This,riid,ppvObject) ) 

#define IDebugMemoryBytes2_AddRef(This)	\
    ( (This)->lpVtbl -> AddRef(This) ) 

#define IDebugMemoryBytes2_Release(This)	\
    ( (This)->lpVtbl -> Release(This) ) 


#define IDebugMemoryBytes2_ReadAt(This,pStartContext,dwCount,rgbMemory,pdwRead,pdwUnreadable)	\
    ( (This)->lpVtbl -> ReadAt(This,pStartContext,dwCount,rgbMemory,pdwRead,pdwUnreadable) ) 

#define IDebugMemoryBytes2_WriteAt(This,pStartContext,dwCount,rgbMemory)	\
    ( (This)->lpVtbl -> WriteAt(This,pStartContext,dwCount,rgbMemory) ) 

#define IDebugMemoryBytes2_GetSize(This,pqwSize)	\
    ( (This)->lpVtbl -> GetSize(This,pqwSize) ) 

#endif /* COBJMACROS */


#endif 	/* C style interface */




#endif 	/* __IDebugMemoryBytes2_INTERFACE_DEFINED__ */


#ifndef __IDebugDisassemblyStream2_INTERFACE_DEFINED__
#define __IDebugDisassemblyStream2_INTERFACE_DEFINED__

/* interface IDebugDisassemblyStream2 */
/* [unique][uuid][object] */ 


enum enum_DISASSEMBLY_STREAM_FIELDS
    {	DSF_ADDRESS	= 0x1,
	DSF_ADDRESSOFFSET	= 0x2,
	DSF_CODEBYTES	= 0x4,
	DSF_OPCODE	= 0x8,
	DSF_OPERANDS	= 0x10,
	DSF_SYMBOL	= 0x20,
	DSF_CODELOCATIONID	= 0x40,
	DSF_POSITION	= 0x80,
	DSF_DOCUMENTURL	= 0x100,
	DSF_BYTEOFFSET	= 0x200,
	DSF_FLAGS	= 0x400,
	DSF_OPERANDS_SYMBOLS	= 0x10000,
	DSF_ALL	= 0x107ff
    } ;
typedef DWORD DISASSEMBLY_STREAM_FIELDS;


enum enum_DISASSEMBLY_FLAGS
    {	DF_DOCUMENTCHANGE	= 0x1,
	DF_DISABLED	= 0x2,
	DF_INSTRUCTION_ACTIVE	= 0x4,
	DF_DATA	= 0x8,
	DF_HASSOURCE	= 0x10,
	DF_DOCUMENT_CHECKSUM	= 0x20
    } ;
typedef DWORD DISASSEMBLY_FLAGS;

typedef struct tagDisassemblyData
    {
    DISASSEMBLY_STREAM_FIELDS dwFields;
    BSTR bstrAddress;
    BSTR bstrAddressOffset;
    BSTR bstrCodeBytes;
    BSTR bstrOpcode;
    BSTR bstrOperands;
    BSTR bstrSymbol;
    UINT64 uCodeLocationId;
    TEXT_POSITION posBeg;
    TEXT_POSITION posEnd;
    BSTR bstrDocumentUrl;
    DWORD dwByteOffset;
    DISASSEMBLY_FLAGS dwFlags;
    } 	DisassemblyData;


enum enum_SEEK_START
    {	SEEK_START_BEGIN	= 0x1,
	SEEK_START_END	= 0x2,
	SEEK_START_CURRENT	= 0x3,
	SEEK_START_CODECONTEXT	= 0x4,
	SEEK_START_CODELOCID	= 0x5
    } ;
typedef DWORD SEEK_START;


EXTERN_C const IID IID_IDebugDisassemblyStream2;

#if defined(__cplusplus) && !defined(CINTERFACE)
    
    MIDL_INTERFACE("e5b017fe-dfb0-411c-8266-7c64d6f519f8")
    IDebugDisassemblyStream2 : public IUnknown
    {
    public:
        virtual HRESULT STDMETHODCALLTYPE Read( 
            /* [in] */ DWORD dwInstructions,
            /* [in] */ DISASSEMBLY_STREAM_FIELDS dwFields,
            /* [out] */ __RPC__out DWORD *pdwInstructionsRead,
            /* [length_is][size_is][out] */ __RPC__out_ecount_part(dwInstructions, *pdwInstructionsRead) DisassemblyData *prgDisassembly) = 0;
        
        virtual HRESULT STDMETHODCALLTYPE Seek( 
            /* [in] */ SEEK_START dwSeekStart,
            /* [in] */ __RPC__in_opt IDebugCodeContext2 *pCodeContext,
            /* [in] */ UINT64 uCodeLocationId,
            /* [in] */ INT64 iInstructions) = 0;
        
        virtual HRESULT STDMETHODCALLTYPE GetCodeLocationId( 
            /* [in] */ __RPC__in_opt IDebugCodeContext2 *pCodeContext,
            /* [out] */ __RPC__out UINT64 *puCodeLocationId) = 0;
        
        virtual HRESULT STDMETHODCALLTYPE GetCodeContext( 
            /* [in] */ UINT64 uCodeLocationId,
            /* [out] */ __RPC__deref_out_opt IDebugCodeContext2 **ppCodeContext) = 0;
        
        virtual HRESULT STDMETHODCALLTYPE GetCurrentLocation( 
            /* [out] */ __RPC__out UINT64 *puCodeLocationId) = 0;
        
        virtual HRESULT STDMETHODCALLTYPE GetDocument( 
            /* [in] */ __RPC__in BSTR bstrDocumentUrl,
            /* [out] */ __RPC__deref_out_opt IDebugDocument2 **ppDocument) = 0;
        
        virtual HRESULT STDMETHODCALLTYPE GetScope( 
            /* [out] */ __RPC__out DISASSEMBLY_STREAM_SCOPE *pdwScope) = 0;
        
        virtual HRESULT STDMETHODCALLTYPE GetSize( 
            /* [out] */ __RPC__out UINT64 *pnSize) = 0;
        
    };
    
#else 	/* C style interface */

    typedef struct IDebugDisassemblyStream2Vtbl
    {
        BEGIN_INTERFACE
        
        HRESULT ( STDMETHODCALLTYPE *QueryInterface )( 
            IDebugDisassemblyStream2 * This,
            /* [in] */ __RPC__in REFIID riid,
            /* [iid_is][out] */ 
            __RPC__deref_out  void **ppvObject);
        
        ULONG ( STDMETHODCALLTYPE *AddRef )( 
            IDebugDisassemblyStream2 * This);
        
        ULONG ( STDMETHODCALLTYPE *Release )( 
            IDebugDisassemblyStream2 * This);
        
        HRESULT ( STDMETHODCALLTYPE *Read )( 
            IDebugDisassemblyStream2 * This,
            /* [in] */ DWORD dwInstructions,
            /* [in] */ DISASSEMBLY_STREAM_FIELDS dwFields,
            /* [out] */ __RPC__out DWORD *pdwInstructionsRead,
            /* [length_is][size_is][out] */ __RPC__out_ecount_part(dwInstructions, *pdwInstructionsRead) DisassemblyData *prgDisassembly);
        
        HRESULT ( STDMETHODCALLTYPE *Seek )( 
            IDebugDisassemblyStream2 * This,
            /* [in] */ SEEK_START dwSeekStart,
            /* [in] */ __RPC__in_opt IDebugCodeContext2 *pCodeContext,
            /* [in] */ UINT64 uCodeLocationId,
            /* [in] */ INT64 iInstructions);
        
        HRESULT ( STDMETHODCALLTYPE *GetCodeLocationId )( 
            IDebugDisassemblyStream2 * This,
            /* [in] */ __RPC__in_opt IDebugCodeContext2 *pCodeContext,
            /* [out] */ __RPC__out UINT64 *puCodeLocationId);
        
        HRESULT ( STDMETHODCALLTYPE *GetCodeContext )( 
            IDebugDisassemblyStream2 * This,
            /* [in] */ UINT64 uCodeLocationId,
            /* [out] */ __RPC__deref_out_opt IDebugCodeContext2 **ppCodeContext);
        
        HRESULT ( STDMETHODCALLTYPE *GetCurrentLocation )( 
            IDebugDisassemblyStream2 * This,
            /* [out] */ __RPC__out UINT64 *puCodeLocationId);
        
        HRESULT ( STDMETHODCALLTYPE *GetDocument )( 
            IDebugDisassemblyStream2 * This,
            /* [in] */ __RPC__in BSTR bstrDocumentUrl,
            /* [out] */ __RPC__deref_out_opt IDebugDocument2 **ppDocument);
        
        HRESULT ( STDMETHODCALLTYPE *GetScope )( 
            IDebugDisassemblyStream2 * This,
            /* [out] */ __RPC__out DISASSEMBLY_STREAM_SCOPE *pdwScope);
        
        HRESULT ( STDMETHODCALLTYPE *GetSize )( 
            IDebugDisassemblyStream2 * This,
            /* [out] */ __RPC__out UINT64 *pnSize);
        
        END_INTERFACE
    } IDebugDisassemblyStream2Vtbl;

    interface IDebugDisassemblyStream2
    {
        CONST_VTBL struct IDebugDisassemblyStream2Vtbl *lpVtbl;
    };

    

#ifdef COBJMACROS


#define IDebugDisassemblyStream2_QueryInterface(This,riid,ppvObject)	\
    ( (This)->lpVtbl -> QueryInterface(This,riid,ppvObject) ) 

#define IDebugDisassemblyStream2_AddRef(This)	\
    ( (This)->lpVtbl -> AddRef(This) ) 

#define IDebugDisassemblyStream2_Release(This)	\
    ( (This)->lpVtbl -> Release(This) ) 


#define IDebugDisassemblyStream2_Read(This,dwInstructions,dwFields,pdwInstructionsRead,prgDisassembly)	\
    ( (This)->lpVtbl -> Read(This,dwInstructions,dwFields,pdwInstructionsRead,prgDisassembly) ) 

#define IDebugDisassemblyStream2_Seek(This,dwSeekStart,pCodeContext,uCodeLocationId,iInstructions)	\
    ( (This)->lpVtbl -> Seek(This,dwSeekStart,pCodeContext,uCodeLocationId,iInstructions) ) 

#define IDebugDisassemblyStream2_GetCodeLocationId(This,pCodeContext,puCodeLocationId)	\
    ( (This)->lpVtbl -> GetCodeLocationId(This,pCodeContext,puCodeLocationId) ) 

#define IDebugDisassemblyStream2_GetCodeContext(This,uCodeLocationId,ppCodeContext)	\
    ( (This)->lpVtbl -> GetCodeContext(This,uCodeLocationId,ppCodeContext) ) 

#define IDebugDisassemblyStream2_GetCurrentLocation(This,puCodeLocationId)	\
    ( (This)->lpVtbl -> GetCurrentLocation(This,puCodeLocationId) ) 

#define IDebugDisassemblyStream2_GetDocument(This,bstrDocumentUrl,ppDocument)	\
    ( (This)->lpVtbl -> GetDocument(This,bstrDocumentUrl,ppDocument) ) 

#define IDebugDisassemblyStream2_GetScope(This,pdwScope)	\
    ( (This)->lpVtbl -> GetScope(This,pdwScope) ) 

#define IDebugDisassemblyStream2_GetSize(This,pnSize)	\
    ( (This)->lpVtbl -> GetSize(This,pnSize) ) 

#endif /* COBJMACROS */


#endif 	/* C style interface */




#endif 	/* __IDebugDisassemblyStream2_INTERFACE_DEFINED__ */


#ifndef __IDebugDocumentContext2_INTERFACE_DEFINED__
#define __IDebugDocumentContext2_INTERFACE_DEFINED__

/* interface IDebugDocumentContext2 */
/* [unique][uuid][object] */ 


enum enum_DOCCONTEXT_COMPARE
    {	DOCCONTEXT_EQUAL	= 0x1,
	DOCCONTEXT_LESS_THAN	= 0x2,
	DOCCONTEXT_GREATER_THAN	= 0x3,
	DOCCONTEXT_SAME_DOCUMENT	= 0x4
    } ;
typedef DWORD DOCCONTEXT_COMPARE;


EXTERN_C const IID IID_IDebugDocumentContext2;

#if defined(__cplusplus) && !defined(CINTERFACE)
    
    MIDL_INTERFACE("931516ad-b600-419c-88fc-dcf5183b5fa9")
    IDebugDocumentContext2 : public IUnknown
    {
    public:
        virtual HRESULT STDMETHODCALLTYPE GetDocument( 
            /* [out] */ __RPC__deref_out_opt IDebugDocument2 **ppDocument) = 0;
        
        virtual HRESULT STDMETHODCALLTYPE GetName( 
            /* [in] */ GETNAME_TYPE gnType,
            /* [out] */ __RPC__deref_out_opt BSTR *pbstrFileName) = 0;
        
        virtual HRESULT STDMETHODCALLTYPE EnumCodeContexts( 
            /* [out] */ __RPC__deref_out_opt IEnumDebugCodeContexts2 **ppEnumCodeCxts) = 0;
        
        virtual HRESULT STDMETHODCALLTYPE GetLanguageInfo( 
            /* [full][out][in] */ __RPC__deref_opt_inout_opt BSTR *pbstrLanguage,
            /* [full][out][in] */ __RPC__inout_opt GUID *pguidLanguage) = 0;
        
        virtual HRESULT STDMETHODCALLTYPE GetStatementRange( 
            /* [full][out][in] */ __RPC__inout_opt TEXT_POSITION *pBegPosition,
            /* [full][out][in] */ __RPC__inout_opt TEXT_POSITION *pEndPosition) = 0;
        
        virtual HRESULT STDMETHODCALLTYPE GetSourceRange( 
            /* [full][out][in] */ __RPC__inout_opt TEXT_POSITION *pBegPosition,
            /* [full][out][in] */ __RPC__inout_opt TEXT_POSITION *pEndPosition) = 0;
        
        virtual HRESULT STDMETHODCALLTYPE Compare( 
            /* [in] */ DOCCONTEXT_COMPARE compare,
            /* [length_is][size_is][in] */ __RPC__in_ecount_part(dwDocContextSetLen, dwDocContextSetLen) IDebugDocumentContext2 **rgpDocContextSet,
            /* [in] */ DWORD dwDocContextSetLen,
            /* [out] */ __RPC__out DWORD *pdwDocContext) = 0;
        
        virtual HRESULT STDMETHODCALLTYPE Seek( 
            /* [in] */ int nCount,
            /* [out] */ __RPC__deref_out_opt IDebugDocumentContext2 **ppDocContext) = 0;
        
    };
    
#else 	/* C style interface */

    typedef struct IDebugDocumentContext2Vtbl
    {
        BEGIN_INTERFACE
        
        HRESULT ( STDMETHODCALLTYPE *QueryInterface )( 
            IDebugDocumentContext2 * This,
            /* [in] */ __RPC__in REFIID riid,
            /* [iid_is][out] */ 
            __RPC__deref_out  void **ppvObject);
        
        ULONG ( STDMETHODCALLTYPE *AddRef )( 
            IDebugDocumentContext2 * This);
        
        ULONG ( STDMETHODCALLTYPE *Release )( 
            IDebugDocumentContext2 * This);
        
        HRESULT ( STDMETHODCALLTYPE *GetDocument )( 
            IDebugDocumentContext2 * This,
            /* [out] */ __RPC__deref_out_opt IDebugDocument2 **ppDocument);
        
        HRESULT ( STDMETHODCALLTYPE *GetName )( 
            IDebugDocumentContext2 * This,
            /* [in] */ GETNAME_TYPE gnType,
            /* [out] */ __RPC__deref_out_opt BSTR *pbstrFileName);
        
        HRESULT ( STDMETHODCALLTYPE *EnumCodeContexts )( 
            IDebugDocumentContext2 * This,
            /* [out] */ __RPC__deref_out_opt IEnumDebugCodeContexts2 **ppEnumCodeCxts);
        
        HRESULT ( STDMETHODCALLTYPE *GetLanguageInfo )( 
            IDebugDocumentContext2 * This,
            /* [full][out][in] */ __RPC__deref_opt_inout_opt BSTR *pbstrLanguage,
            /* [full][out][in] */ __RPC__inout_opt GUID *pguidLanguage);
        
        HRESULT ( STDMETHODCALLTYPE *GetStatementRange )( 
            IDebugDocumentContext2 * This,
            /* [full][out][in] */ __RPC__inout_opt TEXT_POSITION *pBegPosition,
            /* [full][out][in] */ __RPC__inout_opt TEXT_POSITION *pEndPosition);
        
        HRESULT ( STDMETHODCALLTYPE *GetSourceRange )( 
            IDebugDocumentContext2 * This,
            /* [full][out][in] */ __RPC__inout_opt TEXT_POSITION *pBegPosition,
            /* [full][out][in] */ __RPC__inout_opt TEXT_POSITION *pEndPosition);
        
        HRESULT ( STDMETHODCALLTYPE *Compare )( 
            IDebugDocumentContext2 * This,
            /* [in] */ DOCCONTEXT_COMPARE compare,
            /* [length_is][size_is][in] */ __RPC__in_ecount_part(dwDocContextSetLen, dwDocContextSetLen) IDebugDocumentContext2 **rgpDocContextSet,
            /* [in] */ DWORD dwDocContextSetLen,
            /* [out] */ __RPC__out DWORD *pdwDocContext);
        
        HRESULT ( STDMETHODCALLTYPE *Seek )( 
            IDebugDocumentContext2 * This,
            /* [in] */ int nCount,
            /* [out] */ __RPC__deref_out_opt IDebugDocumentContext2 **ppDocContext);
        
        END_INTERFACE
    } IDebugDocumentContext2Vtbl;

    interface IDebugDocumentContext2
    {
        CONST_VTBL struct IDebugDocumentContext2Vtbl *lpVtbl;
    };

    

#ifdef COBJMACROS


#define IDebugDocumentContext2_QueryInterface(This,riid,ppvObject)	\
    ( (This)->lpVtbl -> QueryInterface(This,riid,ppvObject) ) 

#define IDebugDocumentContext2_AddRef(This)	\
    ( (This)->lpVtbl -> AddRef(This) ) 

#define IDebugDocumentContext2_Release(This)	\
    ( (This)->lpVtbl -> Release(This) ) 


#define IDebugDocumentContext2_GetDocument(This,ppDocument)	\
    ( (This)->lpVtbl -> GetDocument(This,ppDocument) ) 

#define IDebugDocumentContext2_GetName(This,gnType,pbstrFileName)	\
    ( (This)->lpVtbl -> GetName(This,gnType,pbstrFileName) ) 

#define IDebugDocumentContext2_EnumCodeContexts(This,ppEnumCodeCxts)	\
    ( (This)->lpVtbl -> EnumCodeContexts(This,ppEnumCodeCxts) ) 

#define IDebugDocumentContext2_GetLanguageInfo(This,pbstrLanguage,pguidLanguage)	\
    ( (This)->lpVtbl -> GetLanguageInfo(This,pbstrLanguage,pguidLanguage) ) 

#define IDebugDocumentContext2_GetStatementRange(This,pBegPosition,pEndPosition)	\
    ( (This)->lpVtbl -> GetStatementRange(This,pBegPosition,pEndPosition) ) 

#define IDebugDocumentContext2_GetSourceRange(This,pBegPosition,pEndPosition)	\
    ( (This)->lpVtbl -> GetSourceRange(This,pBegPosition,pEndPosition) ) 

#define IDebugDocumentContext2_Compare(This,compare,rgpDocContextSet,dwDocContextSetLen,pdwDocContext)	\
    ( (This)->lpVtbl -> Compare(This,compare,rgpDocContextSet,dwDocContextSetLen,pdwDocContext) ) 

#define IDebugDocumentContext2_Seek(This,nCount,ppDocContext)	\
    ( (This)->lpVtbl -> Seek(This,nCount,ppDocContext) ) 

#endif /* COBJMACROS */


#endif 	/* C style interface */




#endif 	/* __IDebugDocumentContext2_INTERFACE_DEFINED__ */


#ifndef __IDebugDocumentChecksum2_INTERFACE_DEFINED__
#define __IDebugDocumentChecksum2_INTERFACE_DEFINED__

/* interface IDebugDocumentChecksum2 */
/* [unique][uuid][object] */ 


EXTERN_C const IID IID_IDebugDocumentChecksum2;

#if defined(__cplusplus) && !defined(CINTERFACE)
    
    MIDL_INTERFACE("c1c74db7-a3a7-40a2-a279-a63ba756b8b0")
    IDebugDocumentChecksum2 : public IUnknown
    {
    public:
        virtual HRESULT STDMETHODCALLTYPE GetChecksumAndAlgorithmId( 
            /* [out] */ __RPC__out GUID *pRetVal,
            /* [in] */ ULONG cMaxBytes,
            /* [size_is][length_is][out] */ __RPC__out_ecount_part(cMaxBytes, *pcNumBytes) BYTE *pChecksum,
            /* [out] */ __RPC__out ULONG *pcNumBytes) = 0;
        
    };
    
#else 	/* C style interface */

    typedef struct IDebugDocumentChecksum2Vtbl
    {
        BEGIN_INTERFACE
        
        HRESULT ( STDMETHODCALLTYPE *QueryInterface )( 
            IDebugDocumentChecksum2 * This,
            /* [in] */ __RPC__in REFIID riid,
            /* [iid_is][out] */ 
            __RPC__deref_out  void **ppvObject);
        
        ULONG ( STDMETHODCALLTYPE *AddRef )( 
            IDebugDocumentChecksum2 * This);
        
        ULONG ( STDMETHODCALLTYPE *Release )( 
            IDebugDocumentChecksum2 * This);
        
        HRESULT ( STDMETHODCALLTYPE *GetChecksumAndAlgorithmId )( 
            IDebugDocumentChecksum2 * This,
            /* [out] */ __RPC__out GUID *pRetVal,
            /* [in] */ ULONG cMaxBytes,
            /* [size_is][length_is][out] */ __RPC__out_ecount_part(cMaxBytes, *pcNumBytes) BYTE *pChecksum,
            /* [out] */ __RPC__out ULONG *pcNumBytes);
        
        END_INTERFACE
    } IDebugDocumentChecksum2Vtbl;

    interface IDebugDocumentChecksum2
    {
        CONST_VTBL struct IDebugDocumentChecksum2Vtbl *lpVtbl;
    };

    

#ifdef COBJMACROS


#define IDebugDocumentChecksum2_QueryInterface(This,riid,ppvObject)	\
    ( (This)->lpVtbl -> QueryInterface(This,riid,ppvObject) ) 

#define IDebugDocumentChecksum2_AddRef(This)	\
    ( (This)->lpVtbl -> AddRef(This) ) 

#define IDebugDocumentChecksum2_Release(This)	\
    ( (This)->lpVtbl -> Release(This) ) 


#define IDebugDocumentChecksum2_GetChecksumAndAlgorithmId(This,pRetVal,cMaxBytes,pChecksum,pcNumBytes)	\
    ( (This)->lpVtbl -> GetChecksumAndAlgorithmId(This,pRetVal,cMaxBytes,pChecksum,pcNumBytes) ) 

#endif /* COBJMACROS */


#endif 	/* C style interface */




#endif 	/* __IDebugDocumentChecksum2_INTERFACE_DEFINED__ */


#ifndef __IDebugENCDocumentContextUpdate_INTERFACE_DEFINED__
#define __IDebugENCDocumentContextUpdate_INTERFACE_DEFINED__

/* interface IDebugENCDocumentContextUpdate */
/* [unique][uuid][object] */ 


EXTERN_C const IID IID_IDebugENCDocumentContextUpdate;

#if defined(__cplusplus) && !defined(CINTERFACE)
    
    MIDL_INTERFACE("F5637291-D779-4580-A82C-0D523E7FDCF0")
    IDebugENCDocumentContextUpdate : public IUnknown
    {
    public:
        virtual HRESULT STDMETHODCALLTYPE UpdateDocumentContext( 
            /* [in] */ __RPC__in_opt IDebugCodeContext2 *pContext,
            /* [in] */ __RPC__in_opt IDebugDocumentContext2 *pDocContext) = 0;
        
        virtual HRESULT STDMETHODCALLTYPE UpdateStatementPosition( 
            /* [in] */ TEXT_POSITION posBegStatement,
            /* [in] */ TEXT_POSITION posEndStatement) = 0;
        
    };
    
#else 	/* C style interface */

    typedef struct IDebugENCDocumentContextUpdateVtbl
    {
        BEGIN_INTERFACE
        
        HRESULT ( STDMETHODCALLTYPE *QueryInterface )( 
            IDebugENCDocumentContextUpdate * This,
            /* [in] */ __RPC__in REFIID riid,
            /* [iid_is][out] */ 
            __RPC__deref_out  void **ppvObject);
        
        ULONG ( STDMETHODCALLTYPE *AddRef )( 
            IDebugENCDocumentContextUpdate * This);
        
        ULONG ( STDMETHODCALLTYPE *Release )( 
            IDebugENCDocumentContextUpdate * This);
        
        HRESULT ( STDMETHODCALLTYPE *UpdateDocumentContext )( 
            IDebugENCDocumentContextUpdate * This,
            /* [in] */ __RPC__in_opt IDebugCodeContext2 *pContext,
            /* [in] */ __RPC__in_opt IDebugDocumentContext2 *pDocContext);
        
        HRESULT ( STDMETHODCALLTYPE *UpdateStatementPosition )( 
            IDebugENCDocumentContextUpdate * This,
            /* [in] */ TEXT_POSITION posBegStatement,
            /* [in] */ TEXT_POSITION posEndStatement);
        
        END_INTERFACE
    } IDebugENCDocumentContextUpdateVtbl;

    interface IDebugENCDocumentContextUpdate
    {
        CONST_VTBL struct IDebugENCDocumentContextUpdateVtbl *lpVtbl;
    };

    

#ifdef COBJMACROS


#define IDebugENCDocumentContextUpdate_QueryInterface(This,riid,ppvObject)	\
    ( (This)->lpVtbl -> QueryInterface(This,riid,ppvObject) ) 

#define IDebugENCDocumentContextUpdate_AddRef(This)	\
    ( (This)->lpVtbl -> AddRef(This) ) 

#define IDebugENCDocumentContextUpdate_Release(This)	\
    ( (This)->lpVtbl -> Release(This) ) 


#define IDebugENCDocumentContextUpdate_UpdateDocumentContext(This,pContext,pDocContext)	\
    ( (This)->lpVtbl -> UpdateDocumentContext(This,pContext,pDocContext) ) 

#define IDebugENCDocumentContextUpdate_UpdateStatementPosition(This,posBegStatement,posEndStatement)	\
    ( (This)->lpVtbl -> UpdateStatementPosition(This,posBegStatement,posEndStatement) ) 

#endif /* COBJMACROS */


#endif 	/* C style interface */




#endif 	/* __IDebugENCDocumentContextUpdate_INTERFACE_DEFINED__ */


#ifndef __IDebugExpressionContext2_INTERFACE_DEFINED__
#define __IDebugExpressionContext2_INTERFACE_DEFINED__

/* interface IDebugExpressionContext2 */
/* [unique][uuid][object] */ 


enum enum_PARSEFLAGS
    {	PARSE_EXPRESSION	= 0x1,
	PARSE_FUNCTION_AS_ADDRESS	= 0x2,
	PARSE_DESIGN_TIME_EXPR_EVAL	= 0x1000
    } ;
typedef DWORD PARSEFLAGS;


EXTERN_C const IID IID_IDebugExpressionContext2;

#if defined(__cplusplus) && !defined(CINTERFACE)
    
    MIDL_INTERFACE("37a44580-d5fc-473e-a048-21702ebfc466")
    IDebugExpressionContext2 : public IUnknown
    {
    public:
        virtual HRESULT STDMETHODCALLTYPE GetName( 
            /* [out] */ __RPC__deref_out_opt BSTR *pbstrName) = 0;
        
        virtual HRESULT STDMETHODCALLTYPE ParseText( 
            /* [in] */ __RPC__in LPCOLESTR pszCode,
            /* [in] */ PARSEFLAGS dwFlags,
            /* [in] */ UINT nRadix,
            /* [out] */ __RPC__deref_out_opt IDebugExpression2 **ppExpr,
            /* [out] */ __RPC__deref_out_opt BSTR *pbstrError,
            /* [out] */ __RPC__out UINT *pichError) = 0;
        
    };
    
#else 	/* C style interface */

    typedef struct IDebugExpressionContext2Vtbl
    {
        BEGIN_INTERFACE
        
        HRESULT ( STDMETHODCALLTYPE *QueryInterface )( 
            IDebugExpressionContext2 * This,
            /* [in] */ __RPC__in REFIID riid,
            /* [iid_is][out] */ 
            __RPC__deref_out  void **ppvObject);
        
        ULONG ( STDMETHODCALLTYPE *AddRef )( 
            IDebugExpressionContext2 * This);
        
        ULONG ( STDMETHODCALLTYPE *Release )( 
            IDebugExpressionContext2 * This);
        
        HRESULT ( STDMETHODCALLTYPE *GetName )( 
            IDebugExpressionContext2 * This,
            /* [out] */ __RPC__deref_out_opt BSTR *pbstrName);
        
        HRESULT ( STDMETHODCALLTYPE *ParseText )( 
            IDebugExpressionContext2 * This,
            /* [in] */ __RPC__in LPCOLESTR pszCode,
            /* [in] */ PARSEFLAGS dwFlags,
            /* [in] */ UINT nRadix,
            /* [out] */ __RPC__deref_out_opt IDebugExpression2 **ppExpr,
            /* [out] */ __RPC__deref_out_opt BSTR *pbstrError,
            /* [out] */ __RPC__out UINT *pichError);
        
        END_INTERFACE
    } IDebugExpressionContext2Vtbl;

    interface IDebugExpressionContext2
    {
        CONST_VTBL struct IDebugExpressionContext2Vtbl *lpVtbl;
    };

    

#ifdef COBJMACROS


#define IDebugExpressionContext2_QueryInterface(This,riid,ppvObject)	\
    ( (This)->lpVtbl -> QueryInterface(This,riid,ppvObject) ) 

#define IDebugExpressionContext2_AddRef(This)	\
    ( (This)->lpVtbl -> AddRef(This) ) 

#define IDebugExpressionContext2_Release(This)	\
    ( (This)->lpVtbl -> Release(This) ) 


#define IDebugExpressionContext2_GetName(This,pbstrName)	\
    ( (This)->lpVtbl -> GetName(This,pbstrName) ) 

#define IDebugExpressionContext2_ParseText(This,pszCode,dwFlags,nRadix,ppExpr,pbstrError,pichError)	\
    ( (This)->lpVtbl -> ParseText(This,pszCode,dwFlags,nRadix,ppExpr,pbstrError,pichError) ) 

#endif /* COBJMACROS */


#endif 	/* C style interface */




#endif 	/* __IDebugExpressionContext2_INTERFACE_DEFINED__ */


/* interface __MIDL_itf_msdbg_0000_0098 */
/* [local] */ 


enum enum_BP_TYPE
    {	BPT_NONE	= 0,
	BPT_CODE	= 0x1,
	BPT_DATA	= 0x2,
	BPT_SPECIAL	= 0x3
    } ;
typedef DWORD BP_TYPE;


enum enum_BP_LOCATION_TYPE
    {	BPLT_NONE	= 0,
	BPLT_FILE_LINE	= 0x10000,
	BPLT_FUNC_OFFSET	= 0x20000,
	BPLT_CONTEXT	= 0x30000,
	BPLT_STRING	= 0x40000,
	BPLT_ADDRESS	= 0x50000,
	BPLT_RESOLUTION	= 0x60000,
	BPLT_CODE_FILE_LINE	= ( BPT_CODE | BPLT_FILE_LINE ) ,
	BPLT_CODE_FUNC_OFFSET	= ( BPT_CODE | BPLT_FUNC_OFFSET ) ,
	BPLT_CODE_CONTEXT	= ( BPT_CODE | BPLT_CONTEXT ) ,
	BPLT_CODE_STRING	= ( BPT_CODE | BPLT_STRING ) ,
	BPLT_CODE_ADDRESS	= ( BPT_CODE | BPLT_ADDRESS ) ,
	BPLT_DATA_STRING	= ( BPT_DATA | BPLT_STRING ) ,
	BPLT_TYPE_MASK	= 0xffff,
	BPLT_LOCATION_TYPE_MASK	= 0xffff0000
    } ;
typedef DWORD BP_LOCATION_TYPE;

typedef struct _BP_LOCATION_CODE_FILE_LINE
    {
    BSTR bstrContext;
    IDebugDocumentPosition2 *pDocPos;
    } 	BP_LOCATION_CODE_FILE_LINE;

typedef struct _BP_LOCATION_CODE_FUNC_OFFSET
    {
    BSTR bstrContext;
    IDebugFunctionPosition2 *pFuncPos;
    } 	BP_LOCATION_CODE_FUNC_OFFSET;

typedef struct _BP_LOCATION_CODE_CONTEXT
    {
    IDebugCodeContext2 *pCodeContext;
    } 	BP_LOCATION_CODE_CONTEXT;

typedef struct _BP_LOCATION_CODE_STRING
    {
    BSTR bstrContext;
    BSTR bstrCodeExpr;
    } 	BP_LOCATION_CODE_STRING;

typedef struct _BP_LOCATION_CODE_ADDRESS
    {
    BSTR bstrContext;
    BSTR bstrModuleUrl;
    BSTR bstrFunction;
    BSTR bstrAddress;
    } 	BP_LOCATION_CODE_ADDRESS;

typedef struct _BP_LOCATION_DATA_STRING
    {
    IDebugThread2 *pThread;
    BSTR bstrContext;
    BSTR bstrDataExpr;
    DWORD dwNumElements;
    } 	BP_LOCATION_DATA_STRING;

typedef struct _BP_LOCATION_RESOLUTION
    {
    IDebugBreakpointResolution2 *pResolution;
    } 	BP_LOCATION_RESOLUTION;

typedef struct _BP_LOCATION
    {
    BP_LOCATION_TYPE bpLocationType;
    /* [switch_type] */ union __MIDL___MIDL_itf_msdbg_0000_0098_0001
        {
        BP_LOCATION_CODE_FILE_LINE bplocCodeFileLine;
        BP_LOCATION_CODE_FUNC_OFFSET bplocCodeFuncOffset;
        BP_LOCATION_CODE_CONTEXT bplocCodeContext;
        BP_LOCATION_CODE_STRING bplocCodeString;
        BP_LOCATION_CODE_ADDRESS bplocCodeAddress;
        BP_LOCATION_DATA_STRING bplocDataString;
        BP_LOCATION_RESOLUTION bplocResolution;
        DWORD unused;
        } 	bpLocation;
    } 	BP_LOCATION;


enum enum_BP_PASSCOUNT_STYLE
    {	BP_PASSCOUNT_NONE	= 0,
	BP_PASSCOUNT_EQUAL	= 0x1,
	BP_PASSCOUNT_EQUAL_OR_GREATER	= 0x2,
	BP_PASSCOUNT_MOD	= 0x3
    } ;
typedef DWORD BP_PASSCOUNT_STYLE;

typedef struct _BP_PASSCOUNT
    {
    DWORD dwPassCount;
    BP_PASSCOUNT_STYLE stylePassCount;
    } 	BP_PASSCOUNT;


enum enum_BP_COND_STYLE
    {	BP_COND_NONE	= 0,
	BP_COND_WHEN_TRUE	= 0x1,
	BP_COND_WHEN_CHANGED	= 0x2
    } ;
typedef DWORD BP_COND_STYLE;

typedef struct _BP_CONDITION
    {
    IDebugThread2 *pThread;
    BP_COND_STYLE styleCondition;
    BSTR bstrContext;
    BSTR bstrCondition;
    UINT nRadix;
    } 	BP_CONDITION;


enum enum_BP_FLAGS
    {	BP_FLAG_NONE	= 0,
	BP_FLAG_MAP_DOCPOSITION	= 0x1,
	BP_FLAG_DONT_STOP	= 0x2
    } ;
typedef DWORD BP_FLAGS;


enum enum_BPREQI_FIELDS
    {	BPREQI_BPLOCATION	= 0x1,
	BPREQI_LANGUAGE	= 0x2,
	BPREQI_PROGRAM	= 0x4,
	BPREQI_PROGRAMNAME	= 0x8,
	BPREQI_THREAD	= 0x10,
	BPREQI_THREADNAME	= 0x20,
	BPREQI_PASSCOUNT	= 0x40,
	BPREQI_CONDITION	= 0x80,
	BPREQI_FLAGS	= 0x100,
	BPREQI_ALLOLDFIELDS	= 0x1ff,
	BPREQI_VENDOR	= 0x200,
	BPREQI_CONSTRAINT	= 0x400,
	BPREQI_TRACEPOINT	= 0x800,
	BPREQI_ALLFIELDS	= 0xfff
    } ;
typedef DWORD BPREQI_FIELDS;

typedef struct _BP_REQUEST_INFO
    {
    BPREQI_FIELDS dwFields;
    GUID guidLanguage;
    BP_LOCATION bpLocation;
    IDebugProgram2 *pProgram;
    BSTR bstrProgramName;
    IDebugThread2 *pThread;
    BSTR bstrThreadName;
    BP_CONDITION bpCondition;
    BP_PASSCOUNT bpPassCount;
    BP_FLAGS dwFlags;
    } 	BP_REQUEST_INFO;

typedef struct _BP_REQUEST_INFO2
    {
    BPREQI_FIELDS dwFields;
    GUID guidLanguage;
    BP_LOCATION bpLocation;
    IDebugProgram2 *pProgram;
    BSTR bstrProgramName;
    IDebugThread2 *pThread;
    BSTR bstrThreadName;
    BP_CONDITION bpCondition;
    BP_PASSCOUNT bpPassCount;
    BP_FLAGS dwFlags;
    GUID guidVendor;
    BSTR bstrConstraint;
    BSTR bstrTracepoint;
    } 	BP_REQUEST_INFO2;



extern RPC_IF_HANDLE __MIDL_itf_msdbg_0000_0098_v0_0_c_ifspec;
extern RPC_IF_HANDLE __MIDL_itf_msdbg_0000_0098_v0_0_s_ifspec;

#ifndef __IDebugBreakpointRequest2_INTERFACE_DEFINED__
#define __IDebugBreakpointRequest2_INTERFACE_DEFINED__

/* interface IDebugBreakpointRequest2 */
/* [unique][uuid][object] */ 


EXTERN_C const IID IID_IDebugBreakpointRequest2;

#if defined(__cplusplus) && !defined(CINTERFACE)
    
    MIDL_INTERFACE("6015fd18-8257-4df3-ac42-f074dedd4cbd")
    IDebugBreakpointRequest2 : public IUnknown
    {
    public:
        virtual HRESULT STDMETHODCALLTYPE GetLocationType( 
            /* [out] */ __RPC__out BP_LOCATION_TYPE *pBPLocationType) = 0;
        
        virtual HRESULT STDMETHODCALLTYPE GetRequestInfo( 
            /* [in] */ BPREQI_FIELDS dwFields,
            /* [out] */ __RPC__out BP_REQUEST_INFO *pBPRequestInfo) = 0;
        
    };
    
#else 	/* C style interface */

    typedef struct IDebugBreakpointRequest2Vtbl
    {
        BEGIN_INTERFACE
        
        HRESULT ( STDMETHODCALLTYPE *QueryInterface )( 
            IDebugBreakpointRequest2 * This,
            /* [in] */ __RPC__in REFIID riid,
            /* [iid_is][out] */ 
            __RPC__deref_out  void **ppvObject);
        
        ULONG ( STDMETHODCALLTYPE *AddRef )( 
            IDebugBreakpointRequest2 * This);
        
        ULONG ( STDMETHODCALLTYPE *Release )( 
            IDebugBreakpointRequest2 * This);
        
        HRESULT ( STDMETHODCALLTYPE *GetLocationType )( 
            IDebugBreakpointRequest2 * This,
            /* [out] */ __RPC__out BP_LOCATION_TYPE *pBPLocationType);
        
        HRESULT ( STDMETHODCALLTYPE *GetRequestInfo )( 
            IDebugBreakpointRequest2 * This,
            /* [in] */ BPREQI_FIELDS dwFields,
            /* [out] */ __RPC__out BP_REQUEST_INFO *pBPRequestInfo);
        
        END_INTERFACE
    } IDebugBreakpointRequest2Vtbl;

    interface IDebugBreakpointRequest2
    {
        CONST_VTBL struct IDebugBreakpointRequest2Vtbl *lpVtbl;
    };

    

#ifdef COBJMACROS


#define IDebugBreakpointRequest2_QueryInterface(This,riid,ppvObject)	\
    ( (This)->lpVtbl -> QueryInterface(This,riid,ppvObject) ) 

#define IDebugBreakpointRequest2_AddRef(This)	\
    ( (This)->lpVtbl -> AddRef(This) ) 

#define IDebugBreakpointRequest2_Release(This)	\
    ( (This)->lpVtbl -> Release(This) ) 


#define IDebugBreakpointRequest2_GetLocationType(This,pBPLocationType)	\
    ( (This)->lpVtbl -> GetLocationType(This,pBPLocationType) ) 

#define IDebugBreakpointRequest2_GetRequestInfo(This,dwFields,pBPRequestInfo)	\
    ( (This)->lpVtbl -> GetRequestInfo(This,dwFields,pBPRequestInfo) ) 

#endif /* COBJMACROS */


#endif 	/* C style interface */




#endif 	/* __IDebugBreakpointRequest2_INTERFACE_DEFINED__ */


/* interface __MIDL_itf_msdbg_0000_0099 */
/* [local] */ 

typedef struct tagCHECKSUM_DATA
    {
    DWORD ByteCount;
    BYTE *pBytes;
    } 	CHECKSUM_DATA;



extern RPC_IF_HANDLE __MIDL_itf_msdbg_0000_0099_v0_0_c_ifspec;
extern RPC_IF_HANDLE __MIDL_itf_msdbg_0000_0099_v0_0_s_ifspec;

#ifndef __IDebugBreakpointChecksumRequest2_INTERFACE_DEFINED__
#define __IDebugBreakpointChecksumRequest2_INTERFACE_DEFINED__

/* interface IDebugBreakpointChecksumRequest2 */
/* [unique][uuid][object] */ 


EXTERN_C const IID IID_IDebugBreakpointChecksumRequest2;

#if defined(__cplusplus) && !defined(CINTERFACE)
    
    MIDL_INTERFACE("0EA91CF7-8542-4780-8D6B-7BD686CD2471")
    IDebugBreakpointChecksumRequest2 : public IUnknown
    {
    public:
        virtual HRESULT STDMETHODCALLTYPE GetChecksum( 
            /* [in] */ __RPC__in REFGUID guidAlgorithm,
            /* [out] */ __RPC__out CHECKSUM_DATA *pChecksumData) = 0;
        
        virtual HRESULT STDMETHODCALLTYPE IsChecksumEnabled( 
            /* [out] */ __RPC__out BOOL *pfChecksumEnabled) = 0;
        
    };
    
#else 	/* C style interface */

    typedef struct IDebugBreakpointChecksumRequest2Vtbl
    {
        BEGIN_INTERFACE
        
        HRESULT ( STDMETHODCALLTYPE *QueryInterface )( 
            IDebugBreakpointChecksumRequest2 * This,
            /* [in] */ __RPC__in REFIID riid,
            /* [iid_is][out] */ 
            __RPC__deref_out  void **ppvObject);
        
        ULONG ( STDMETHODCALLTYPE *AddRef )( 
            IDebugBreakpointChecksumRequest2 * This);
        
        ULONG ( STDMETHODCALLTYPE *Release )( 
            IDebugBreakpointChecksumRequest2 * This);
        
        HRESULT ( STDMETHODCALLTYPE *GetChecksum )( 
            IDebugBreakpointChecksumRequest2 * This,
            /* [in] */ __RPC__in REFGUID guidAlgorithm,
            /* [out] */ __RPC__out CHECKSUM_DATA *pChecksumData);
        
        HRESULT ( STDMETHODCALLTYPE *IsChecksumEnabled )( 
            IDebugBreakpointChecksumRequest2 * This,
            /* [out] */ __RPC__out BOOL *pfChecksumEnabled);
        
        END_INTERFACE
    } IDebugBreakpointChecksumRequest2Vtbl;

    interface IDebugBreakpointChecksumRequest2
    {
        CONST_VTBL struct IDebugBreakpointChecksumRequest2Vtbl *lpVtbl;
    };

    

#ifdef COBJMACROS


#define IDebugBreakpointChecksumRequest2_QueryInterface(This,riid,ppvObject)	\
    ( (This)->lpVtbl -> QueryInterface(This,riid,ppvObject) ) 

#define IDebugBreakpointChecksumRequest2_AddRef(This)	\
    ( (This)->lpVtbl -> AddRef(This) ) 

#define IDebugBreakpointChecksumRequest2_Release(This)	\
    ( (This)->lpVtbl -> Release(This) ) 


#define IDebugBreakpointChecksumRequest2_GetChecksum(This,guidAlgorithm,pChecksumData)	\
    ( (This)->lpVtbl -> GetChecksum(This,guidAlgorithm,pChecksumData) ) 

#define IDebugBreakpointChecksumRequest2_IsChecksumEnabled(This,pfChecksumEnabled)	\
    ( (This)->lpVtbl -> IsChecksumEnabled(This,pfChecksumEnabled) ) 

#endif /* COBJMACROS */


#endif 	/* C style interface */




#endif 	/* __IDebugBreakpointChecksumRequest2_INTERFACE_DEFINED__ */


#ifndef __IDebugBreakpointRequest3_INTERFACE_DEFINED__
#define __IDebugBreakpointRequest3_INTERFACE_DEFINED__

/* interface IDebugBreakpointRequest3 */
/* [unique][uuid][object] */ 


EXTERN_C const IID IID_IDebugBreakpointRequest3;

#if defined(__cplusplus) && !defined(CINTERFACE)
    
    MIDL_INTERFACE("5C18A5FE-7150-4e66-8246-27BFB0E7BFD9")
    IDebugBreakpointRequest3 : public IDebugBreakpointRequest2
    {
    public:
        virtual HRESULT STDMETHODCALLTYPE GetRequestInfo2( 
            /* [in] */ BPREQI_FIELDS dwFields,
            /* [out] */ __RPC__out BP_REQUEST_INFO2 *bBPRequestInfo) = 0;
        
    };
    
#else 	/* C style interface */

    typedef struct IDebugBreakpointRequest3Vtbl
    {
        BEGIN_INTERFACE
        
        HRESULT ( STDMETHODCALLTYPE *QueryInterface )( 
            IDebugBreakpointRequest3 * This,
            /* [in] */ __RPC__in REFIID riid,
            /* [iid_is][out] */ 
            __RPC__deref_out  void **ppvObject);
        
        ULONG ( STDMETHODCALLTYPE *AddRef )( 
            IDebugBreakpointRequest3 * This);
        
        ULONG ( STDMETHODCALLTYPE *Release )( 
            IDebugBreakpointRequest3 * This);
        
        HRESULT ( STDMETHODCALLTYPE *GetLocationType )( 
            IDebugBreakpointRequest3 * This,
            /* [out] */ __RPC__out BP_LOCATION_TYPE *pBPLocationType);
        
        HRESULT ( STDMETHODCALLTYPE *GetRequestInfo )( 
            IDebugBreakpointRequest3 * This,
            /* [in] */ BPREQI_FIELDS dwFields,
            /* [out] */ __RPC__out BP_REQUEST_INFO *pBPRequestInfo);
        
        HRESULT ( STDMETHODCALLTYPE *GetRequestInfo2 )( 
            IDebugBreakpointRequest3 * This,
            /* [in] */ BPREQI_FIELDS dwFields,
            /* [out] */ __RPC__out BP_REQUEST_INFO2 *bBPRequestInfo);
        
        END_INTERFACE
    } IDebugBreakpointRequest3Vtbl;

    interface IDebugBreakpointRequest3
    {
        CONST_VTBL struct IDebugBreakpointRequest3Vtbl *lpVtbl;
    };

    

#ifdef COBJMACROS


#define IDebugBreakpointRequest3_QueryInterface(This,riid,ppvObject)	\
    ( (This)->lpVtbl -> QueryInterface(This,riid,ppvObject) ) 

#define IDebugBreakpointRequest3_AddRef(This)	\
    ( (This)->lpVtbl -> AddRef(This) ) 

#define IDebugBreakpointRequest3_Release(This)	\
    ( (This)->lpVtbl -> Release(This) ) 


#define IDebugBreakpointRequest3_GetLocationType(This,pBPLocationType)	\
    ( (This)->lpVtbl -> GetLocationType(This,pBPLocationType) ) 

#define IDebugBreakpointRequest3_GetRequestInfo(This,dwFields,pBPRequestInfo)	\
    ( (This)->lpVtbl -> GetRequestInfo(This,dwFields,pBPRequestInfo) ) 


#define IDebugBreakpointRequest3_GetRequestInfo2(This,dwFields,bBPRequestInfo)	\
    ( (This)->lpVtbl -> GetRequestInfo2(This,dwFields,bBPRequestInfo) ) 

#endif /* COBJMACROS */


#endif 	/* C style interface */




#endif 	/* __IDebugBreakpointRequest3_INTERFACE_DEFINED__ */


/* interface __MIDL_itf_msdbg_0000_0101 */
/* [local] */ 

typedef struct _BP_RESOLUTION_CODE
    {
    IDebugCodeContext2 *pCodeContext;
    } 	BP_RESOLUTION_CODE;


enum enum_BP_RES_DATA_FLAGS
    {	BP_RES_DATA_EMULATED	= 0x1
    } ;
typedef DWORD BP_RES_DATA_FLAGS;

typedef struct _BP_RESOLUTION_DATA
    {
    BSTR bstrDataExpr;
    BSTR bstrFunc;
    BSTR bstrImage;
    BP_RES_DATA_FLAGS dwFlags;
    } 	BP_RESOLUTION_DATA;

typedef struct _BP_RESOLUTION_LOCATION
    {
    BP_TYPE bpType;
    /* [switch_type] */ union __MIDL___MIDL_itf_msdbg_0000_0101_0001
        {
        BP_RESOLUTION_CODE bpresCode;
        BP_RESOLUTION_DATA bpresData;
        int unused;
        } 	bpResLocation;
    } 	BP_RESOLUTION_LOCATION;


enum enum_BPRESI_FIELDS
    {	BPRESI_BPRESLOCATION	= 0x1,
	BPRESI_PROGRAM	= 0x2,
	BPRESI_THREAD	= 0x4,
	BPRESI_ALLFIELDS	= 0xffffffff
    } ;
typedef DWORD BPRESI_FIELDS;

typedef struct _BP_RESOLUTION_INFO
    {
    BPRESI_FIELDS dwFields;
    BP_RESOLUTION_LOCATION bpResLocation;
    IDebugProgram2 *pProgram;
    IDebugThread2 *pThread;
    } 	BP_RESOLUTION_INFO;



extern RPC_IF_HANDLE __MIDL_itf_msdbg_0000_0101_v0_0_c_ifspec;
extern RPC_IF_HANDLE __MIDL_itf_msdbg_0000_0101_v0_0_s_ifspec;

#ifndef __IDebugBreakpointResolution2_INTERFACE_DEFINED__
#define __IDebugBreakpointResolution2_INTERFACE_DEFINED__

/* interface IDebugBreakpointResolution2 */
/* [unique][uuid][object] */ 


EXTERN_C const IID IID_IDebugBreakpointResolution2;

#if defined(__cplusplus) && !defined(CINTERFACE)
    
    MIDL_INTERFACE("b7e66f28-035a-401a-afc7-2e300bd29711")
    IDebugBreakpointResolution2 : public IUnknown
    {
    public:
        virtual HRESULT STDMETHODCALLTYPE GetBreakpointType( 
            /* [out] */ __RPC__out BP_TYPE *pBPType) = 0;
        
        virtual HRESULT STDMETHODCALLTYPE GetResolutionInfo( 
            /* [in] */ BPRESI_FIELDS dwFields,
            /* [out] */ __RPC__out BP_RESOLUTION_INFO *pBPResolutionInfo) = 0;
        
    };
    
#else 	/* C style interface */

    typedef struct IDebugBreakpointResolution2Vtbl
    {
        BEGIN_INTERFACE
        
        HRESULT ( STDMETHODCALLTYPE *QueryInterface )( 
            IDebugBreakpointResolution2 * This,
            /* [in] */ __RPC__in REFIID riid,
            /* [iid_is][out] */ 
            __RPC__deref_out  void **ppvObject);
        
        ULONG ( STDMETHODCALLTYPE *AddRef )( 
            IDebugBreakpointResolution2 * This);
        
        ULONG ( STDMETHODCALLTYPE *Release )( 
            IDebugBreakpointResolution2 * This);
        
        HRESULT ( STDMETHODCALLTYPE *GetBreakpointType )( 
            IDebugBreakpointResolution2 * This,
            /* [out] */ __RPC__out BP_TYPE *pBPType);
        
        HRESULT ( STDMETHODCALLTYPE *GetResolutionInfo )( 
            IDebugBreakpointResolution2 * This,
            /* [in] */ BPRESI_FIELDS dwFields,
            /* [out] */ __RPC__out BP_RESOLUTION_INFO *pBPResolutionInfo);
        
        END_INTERFACE
    } IDebugBreakpointResolution2Vtbl;

    interface IDebugBreakpointResolution2
    {
        CONST_VTBL struct IDebugBreakpointResolution2Vtbl *lpVtbl;
    };

    

#ifdef COBJMACROS


#define IDebugBreakpointResolution2_QueryInterface(This,riid,ppvObject)	\
    ( (This)->lpVtbl -> QueryInterface(This,riid,ppvObject) ) 

#define IDebugBreakpointResolution2_AddRef(This)	\
    ( (This)->lpVtbl -> AddRef(This) ) 

#define IDebugBreakpointResolution2_Release(This)	\
    ( (This)->lpVtbl -> Release(This) ) 


#define IDebugBreakpointResolution2_GetBreakpointType(This,pBPType)	\
    ( (This)->lpVtbl -> GetBreakpointType(This,pBPType) ) 

#define IDebugBreakpointResolution2_GetResolutionInfo(This,dwFields,pBPResolutionInfo)	\
    ( (This)->lpVtbl -> GetResolutionInfo(This,dwFields,pBPResolutionInfo) ) 

#endif /* COBJMACROS */


#endif 	/* C style interface */




#endif 	/* __IDebugBreakpointResolution2_INTERFACE_DEFINED__ */


/* interface __MIDL_itf_msdbg_0000_0102 */
/* [local] */ 


enum enum_BP_ERROR_TYPE
    {	BPET_NONE	= 0,
	BPET_TYPE_WARNING	= 0x1,
	BPET_TYPE_ERROR	= 0x2,
	BPET_SEV_HIGH	= 0xf000000,
	BPET_SEV_GENERAL	= 0x7000000,
	BPET_SEV_LOW	= 0x1000000,
	BPET_TYPE_MASK	= 0xffff,
	BPET_SEV_MASK	= 0xffff0000,
	BPET_GENERAL_WARNING	= ( BPET_SEV_GENERAL | BPET_TYPE_WARNING ) ,
	BPET_GENERAL_ERROR	= ( BPET_SEV_GENERAL | BPET_TYPE_ERROR ) ,
	BPET_ALL	= 0xffffffff
    } ;
typedef DWORD BP_ERROR_TYPE;


enum enum_BPERESI_FIELDS
    {	BPERESI_BPRESLOCATION	= 0x1,
	BPERESI_PROGRAM	= 0x2,
	BPERESI_THREAD	= 0x4,
	BPERESI_MESSAGE	= 0x8,
	BPERESI_TYPE	= 0x10,
	BPERESI_ALLFIELDS	= 0xffffffff
    } ;
typedef DWORD BPERESI_FIELDS;

typedef struct _BP_ERROR_RESOLUTION_INFO
    {
    BPERESI_FIELDS dwFields;
    BP_RESOLUTION_LOCATION bpResLocation;
    IDebugProgram2 *pProgram;
    IDebugThread2 *pThread;
    BSTR bstrMessage;
    BP_ERROR_TYPE dwType;
    } 	BP_ERROR_RESOLUTION_INFO;



extern RPC_IF_HANDLE __MIDL_itf_msdbg_0000_0102_v0_0_c_ifspec;
extern RPC_IF_HANDLE __MIDL_itf_msdbg_0000_0102_v0_0_s_ifspec;

#ifndef __IDebugErrorBreakpointResolution2_INTERFACE_DEFINED__
#define __IDebugErrorBreakpointResolution2_INTERFACE_DEFINED__

/* interface IDebugErrorBreakpointResolution2 */
/* [unique][uuid][object] */ 


EXTERN_C const IID IID_IDebugErrorBreakpointResolution2;

#if defined(__cplusplus) && !defined(CINTERFACE)
    
    MIDL_INTERFACE("603aedf8-9575-4d30-b8ca-124d1c98ebd8")
    IDebugErrorBreakpointResolution2 : public IUnknown
    {
    public:
        virtual HRESULT STDMETHODCALLTYPE GetBreakpointType( 
            /* [out] */ __RPC__out BP_TYPE *pBPType) = 0;
        
        virtual HRESULT STDMETHODCALLTYPE GetResolutionInfo( 
            /* [in] */ BPERESI_FIELDS dwFields,
            /* [out] */ __RPC__out BP_ERROR_RESOLUTION_INFO *pErrorResolutionInfo) = 0;
        
    };
    
#else 	/* C style interface */

    typedef struct IDebugErrorBreakpointResolution2Vtbl
    {
        BEGIN_INTERFACE
        
        HRESULT ( STDMETHODCALLTYPE *QueryInterface )( 
            IDebugErrorBreakpointResolution2 * This,
            /* [in] */ __RPC__in REFIID riid,
            /* [iid_is][out] */ 
            __RPC__deref_out  void **ppvObject);
        
        ULONG ( STDMETHODCALLTYPE *AddRef )( 
            IDebugErrorBreakpointResolution2 * This);
        
        ULONG ( STDMETHODCALLTYPE *Release )( 
            IDebugErrorBreakpointResolution2 * This);
        
        HRESULT ( STDMETHODCALLTYPE *GetBreakpointType )( 
            IDebugErrorBreakpointResolution2 * This,
            /* [out] */ __RPC__out BP_TYPE *pBPType);
        
        HRESULT ( STDMETHODCALLTYPE *GetResolutionInfo )( 
            IDebugErrorBreakpointResolution2 * This,
            /* [in] */ BPERESI_FIELDS dwFields,
            /* [out] */ __RPC__out BP_ERROR_RESOLUTION_INFO *pErrorResolutionInfo);
        
        END_INTERFACE
    } IDebugErrorBreakpointResolution2Vtbl;

    interface IDebugErrorBreakpointResolution2
    {
        CONST_VTBL struct IDebugErrorBreakpointResolution2Vtbl *lpVtbl;
    };

    

#ifdef COBJMACROS


#define IDebugErrorBreakpointResolution2_QueryInterface(This,riid,ppvObject)	\
    ( (This)->lpVtbl -> QueryInterface(This,riid,ppvObject) ) 

#define IDebugErrorBreakpointResolution2_AddRef(This)	\
    ( (This)->lpVtbl -> AddRef(This) ) 

#define IDebugErrorBreakpointResolution2_Release(This)	\
    ( (This)->lpVtbl -> Release(This) ) 


#define IDebugErrorBreakpointResolution2_GetBreakpointType(This,pBPType)	\
    ( (This)->lpVtbl -> GetBreakpointType(This,pBPType) ) 

#define IDebugErrorBreakpointResolution2_GetResolutionInfo(This,dwFields,pErrorResolutionInfo)	\
    ( (This)->lpVtbl -> GetResolutionInfo(This,dwFields,pErrorResolutionInfo) ) 

#endif /* COBJMACROS */


#endif 	/* C style interface */




#endif 	/* __IDebugErrorBreakpointResolution2_INTERFACE_DEFINED__ */


#ifndef __IDebugBoundBreakpoint2_INTERFACE_DEFINED__
#define __IDebugBoundBreakpoint2_INTERFACE_DEFINED__

/* interface IDebugBoundBreakpoint2 */
/* [unique][uuid][object] */ 


enum enum_BP_STATE
    {	BPS_NONE	= 0,
	BPS_DELETED	= 0x1,
	BPS_DISABLED	= 0x2,
	BPS_ENABLED	= 0x3
    } ;
typedef DWORD BP_STATE;


EXTERN_C const IID IID_IDebugBoundBreakpoint2;

#if defined(__cplusplus) && !defined(CINTERFACE)
    
    MIDL_INTERFACE("d533d975-3f32-4876-abd0-6d37fda563e7")
    IDebugBoundBreakpoint2 : public IUnknown
    {
    public:
        virtual HRESULT STDMETHODCALLTYPE GetPendingBreakpoint( 
            /* [out] */ __RPC__deref_out_opt IDebugPendingBreakpoint2 **ppPendingBreakpoint) = 0;
        
        virtual HRESULT STDMETHODCALLTYPE GetState( 
            /* [out] */ __RPC__out BP_STATE *pState) = 0;
        
        virtual HRESULT STDMETHODCALLTYPE GetHitCount( 
            /* [out] */ __RPC__out DWORD *pdwHitCount) = 0;
        
        virtual HRESULT STDMETHODCALLTYPE GetBreakpointResolution( 
            /* [out] */ __RPC__deref_out_opt IDebugBreakpointResolution2 **ppBPResolution) = 0;
        
        virtual HRESULT STDMETHODCALLTYPE Enable( 
            /* [in] */ BOOL fEnable) = 0;
        
        virtual HRESULT STDMETHODCALLTYPE SetHitCount( 
            /* [in] */ DWORD dwHitCount) = 0;
        
        virtual HRESULT STDMETHODCALLTYPE SetCondition( 
            /* [in] */ BP_CONDITION bpCondition) = 0;
        
        virtual HRESULT STDMETHODCALLTYPE SetPassCount( 
            /* [in] */ BP_PASSCOUNT bpPassCount) = 0;
        
        virtual HRESULT STDMETHODCALLTYPE Delete( void) = 0;
        
    };
    
#else 	/* C style interface */

    typedef struct IDebugBoundBreakpoint2Vtbl
    {
        BEGIN_INTERFACE
        
        HRESULT ( STDMETHODCALLTYPE *QueryInterface )( 
            IDebugBoundBreakpoint2 * This,
            /* [in] */ __RPC__in REFIID riid,
            /* [iid_is][out] */ 
            __RPC__deref_out  void **ppvObject);
        
        ULONG ( STDMETHODCALLTYPE *AddRef )( 
            IDebugBoundBreakpoint2 * This);
        
        ULONG ( STDMETHODCALLTYPE *Release )( 
            IDebugBoundBreakpoint2 * This);
        
        HRESULT ( STDMETHODCALLTYPE *GetPendingBreakpoint )( 
            IDebugBoundBreakpoint2 * This,
            /* [out] */ __RPC__deref_out_opt IDebugPendingBreakpoint2 **ppPendingBreakpoint);
        
        HRESULT ( STDMETHODCALLTYPE *GetState )( 
            IDebugBoundBreakpoint2 * This,
            /* [out] */ __RPC__out BP_STATE *pState);
        
        HRESULT ( STDMETHODCALLTYPE *GetHitCount )( 
            IDebugBoundBreakpoint2 * This,
            /* [out] */ __RPC__out DWORD *pdwHitCount);
        
        HRESULT ( STDMETHODCALLTYPE *GetBreakpointResolution )( 
            IDebugBoundBreakpoint2 * This,
            /* [out] */ __RPC__deref_out_opt IDebugBreakpointResolution2 **ppBPResolution);
        
        HRESULT ( STDMETHODCALLTYPE *Enable )( 
            IDebugBoundBreakpoint2 * This,
            /* [in] */ BOOL fEnable);
        
        HRESULT ( STDMETHODCALLTYPE *SetHitCount )( 
            IDebugBoundBreakpoint2 * This,
            /* [in] */ DWORD dwHitCount);
        
        HRESULT ( STDMETHODCALLTYPE *SetCondition )( 
            IDebugBoundBreakpoint2 * This,
            /* [in] */ BP_CONDITION bpCondition);
        
        HRESULT ( STDMETHODCALLTYPE *SetPassCount )( 
            IDebugBoundBreakpoint2 * This,
            /* [in] */ BP_PASSCOUNT bpPassCount);
        
        HRESULT ( STDMETHODCALLTYPE *Delete )( 
            IDebugBoundBreakpoint2 * This);
        
        END_INTERFACE
    } IDebugBoundBreakpoint2Vtbl;

    interface IDebugBoundBreakpoint2
    {
        CONST_VTBL struct IDebugBoundBreakpoint2Vtbl *lpVtbl;
    };

    

#ifdef COBJMACROS


#define IDebugBoundBreakpoint2_QueryInterface(This,riid,ppvObject)	\
    ( (This)->lpVtbl -> QueryInterface(This,riid,ppvObject) ) 

#define IDebugBoundBreakpoint2_AddRef(This)	\
    ( (This)->lpVtbl -> AddRef(This) ) 

#define IDebugBoundBreakpoint2_Release(This)	\
    ( (This)->lpVtbl -> Release(This) ) 


#define IDebugBoundBreakpoint2_GetPendingBreakpoint(This,ppPendingBreakpoint)	\
    ( (This)->lpVtbl -> GetPendingBreakpoint(This,ppPendingBreakpoint) ) 

#define IDebugBoundBreakpoint2_GetState(This,pState)	\
    ( (This)->lpVtbl -> GetState(This,pState) ) 

#define IDebugBoundBreakpoint2_GetHitCount(This,pdwHitCount)	\
    ( (This)->lpVtbl -> GetHitCount(This,pdwHitCount) ) 

#define IDebugBoundBreakpoint2_GetBreakpointResolution(This,ppBPResolution)	\
    ( (This)->lpVtbl -> GetBreakpointResolution(This,ppBPResolution) ) 

#define IDebugBoundBreakpoint2_Enable(This,fEnable)	\
    ( (This)->lpVtbl -> Enable(This,fEnable) ) 

#define IDebugBoundBreakpoint2_SetHitCount(This,dwHitCount)	\
    ( (This)->lpVtbl -> SetHitCount(This,dwHitCount) ) 

#define IDebugBoundBreakpoint2_SetCondition(This,bpCondition)	\
    ( (This)->lpVtbl -> SetCondition(This,bpCondition) ) 

#define IDebugBoundBreakpoint2_SetPassCount(This,bpPassCount)	\
    ( (This)->lpVtbl -> SetPassCount(This,bpPassCount) ) 

#define IDebugBoundBreakpoint2_Delete(This)	\
    ( (This)->lpVtbl -> Delete(This) ) 

#endif /* COBJMACROS */


#endif 	/* C style interface */




#endif 	/* __IDebugBoundBreakpoint2_INTERFACE_DEFINED__ */


#ifndef __IDebugBoundBreakpoint3_INTERFACE_DEFINED__
#define __IDebugBoundBreakpoint3_INTERFACE_DEFINED__

/* interface IDebugBoundBreakpoint3 */
/* [unique][uuid][object] */ 


EXTERN_C const IID IID_IDebugBoundBreakpoint3;

#if defined(__cplusplus) && !defined(CINTERFACE)
    
    MIDL_INTERFACE("60f49115-ce92-4f96-8d0a-81cccae4ab77")
    IDebugBoundBreakpoint3 : public IUnknown
    {
    public:
        virtual HRESULT STDMETHODCALLTYPE SetTracepoint( 
            /* [in] */ __RPC__in LPCOLESTR bpBstrTracepoint,
            /* [in] */ BP_FLAGS bpFlags) = 0;
        
    };
    
#else 	/* C style interface */

    typedef struct IDebugBoundBreakpoint3Vtbl
    {
        BEGIN_INTERFACE
        
        HRESULT ( STDMETHODCALLTYPE *QueryInterface )( 
            IDebugBoundBreakpoint3 * This,
            /* [in] */ __RPC__in REFIID riid,
            /* [iid_is][out] */ 
            __RPC__deref_out  void **ppvObject);
        
        ULONG ( STDMETHODCALLTYPE *AddRef )( 
            IDebugBoundBreakpoint3 * This);
        
        ULONG ( STDMETHODCALLTYPE *Release )( 
            IDebugBoundBreakpoint3 * This);
        
        HRESULT ( STDMETHODCALLTYPE *SetTracepoint )( 
            IDebugBoundBreakpoint3 * This,
            /* [in] */ __RPC__in LPCOLESTR bpBstrTracepoint,
            /* [in] */ BP_FLAGS bpFlags);
        
        END_INTERFACE
    } IDebugBoundBreakpoint3Vtbl;

    interface IDebugBoundBreakpoint3
    {
        CONST_VTBL struct IDebugBoundBreakpoint3Vtbl *lpVtbl;
    };

    

#ifdef COBJMACROS


#define IDebugBoundBreakpoint3_QueryInterface(This,riid,ppvObject)	\
    ( (This)->lpVtbl -> QueryInterface(This,riid,ppvObject) ) 

#define IDebugBoundBreakpoint3_AddRef(This)	\
    ( (This)->lpVtbl -> AddRef(This) ) 

#define IDebugBoundBreakpoint3_Release(This)	\
    ( (This)->lpVtbl -> Release(This) ) 


#define IDebugBoundBreakpoint3_SetTracepoint(This,bpBstrTracepoint,bpFlags)	\
    ( (This)->lpVtbl -> SetTracepoint(This,bpBstrTracepoint,bpFlags) ) 

#endif /* COBJMACROS */


#endif 	/* C style interface */




#endif 	/* __IDebugBoundBreakpoint3_INTERFACE_DEFINED__ */


#ifndef __IDebugPendingBreakpoint2_INTERFACE_DEFINED__
#define __IDebugPendingBreakpoint2_INTERFACE_DEFINED__

/* interface IDebugPendingBreakpoint2 */
/* [unique][uuid][object] */ 


enum enum_PENDING_BP_STATE
    {	PBPS_NONE	= 0,
	PBPS_DELETED	= 0x1,
	PBPS_DISABLED	= 0x2,
	PBPS_ENABLED	= 0x3
    } ;
typedef DWORD PENDING_BP_STATE;


enum enum_PENDING_BP_STATE_FLAGS
    {	PBPSF_NONE	= 0,
	PBPSF_VIRTUALIZED	= 0x1
    } ;
typedef DWORD PENDING_BP_STATE_FLAGS;

typedef struct _tagPENDING_BP_STATE_INFO
    {
    PENDING_BP_STATE state;
    PENDING_BP_STATE_FLAGS flags;
    } 	PENDING_BP_STATE_INFO;


EXTERN_C const IID IID_IDebugPendingBreakpoint2;

#if defined(__cplusplus) && !defined(CINTERFACE)
    
    MIDL_INTERFACE("6e215ef3-e44c-44d1-b7ba-b2401f7dc23d")
    IDebugPendingBreakpoint2 : public IUnknown
    {
    public:
        virtual HRESULT STDMETHODCALLTYPE CanBind( 
            /* [out] */ __RPC__deref_out_opt IEnumDebugErrorBreakpoints2 **ppErrorEnum) = 0;
        
        virtual HRESULT STDMETHODCALLTYPE Bind( void) = 0;
        
        virtual HRESULT STDMETHODCALLTYPE GetState( 
            /* [out] */ __RPC__out PENDING_BP_STATE_INFO *pState) = 0;
        
        virtual HRESULT STDMETHODCALLTYPE GetBreakpointRequest( 
            /* [out] */ __RPC__deref_out_opt IDebugBreakpointRequest2 **ppBPRequest) = 0;
        
        virtual HRESULT STDMETHODCALLTYPE Virtualize( 
            /* [in] */ BOOL fVirtualize) = 0;
        
        virtual HRESULT STDMETHODCALLTYPE Enable( 
            /* [in] */ BOOL fEnable) = 0;
        
        virtual HRESULT STDMETHODCALLTYPE SetCondition( 
            /* [in] */ BP_CONDITION bpCondition) = 0;
        
        virtual HRESULT STDMETHODCALLTYPE SetPassCount( 
            /* [in] */ BP_PASSCOUNT bpPassCount) = 0;
        
        virtual HRESULT STDMETHODCALLTYPE EnumBoundBreakpoints( 
            /* [out] */ __RPC__deref_out_opt IEnumDebugBoundBreakpoints2 **ppEnum) = 0;
        
        virtual HRESULT STDMETHODCALLTYPE EnumErrorBreakpoints( 
            /* [in] */ BP_ERROR_TYPE bpErrorType,
            /* [out] */ __RPC__deref_out_opt IEnumDebugErrorBreakpoints2 **ppEnum) = 0;
        
        virtual HRESULT STDMETHODCALLTYPE Delete( void) = 0;
        
    };
    
#else 	/* C style interface */

    typedef struct IDebugPendingBreakpoint2Vtbl
    {
        BEGIN_INTERFACE
        
        HRESULT ( STDMETHODCALLTYPE *QueryInterface )( 
            IDebugPendingBreakpoint2 * This,
            /* [in] */ __RPC__in REFIID riid,
            /* [iid_is][out] */ 
            __RPC__deref_out  void **ppvObject);
        
        ULONG ( STDMETHODCALLTYPE *AddRef )( 
            IDebugPendingBreakpoint2 * This);
        
        ULONG ( STDMETHODCALLTYPE *Release )( 
            IDebugPendingBreakpoint2 * This);
        
        HRESULT ( STDMETHODCALLTYPE *CanBind )( 
            IDebugPendingBreakpoint2 * This,
            /* [out] */ __RPC__deref_out_opt IEnumDebugErrorBreakpoints2 **ppErrorEnum);
        
        HRESULT ( STDMETHODCALLTYPE *Bind )( 
            IDebugPendingBreakpoint2 * This);
        
        HRESULT ( STDMETHODCALLTYPE *GetState )( 
            IDebugPendingBreakpoint2 * This,
            /* [out] */ __RPC__out PENDING_BP_STATE_INFO *pState);
        
        HRESULT ( STDMETHODCALLTYPE *GetBreakpointRequest )( 
            IDebugPendingBreakpoint2 * This,
            /* [out] */ __RPC__deref_out_opt IDebugBreakpointRequest2 **ppBPRequest);
        
        HRESULT ( STDMETHODCALLTYPE *Virtualize )( 
            IDebugPendingBreakpoint2 * This,
            /* [in] */ BOOL fVirtualize);
        
        HRESULT ( STDMETHODCALLTYPE *Enable )( 
            IDebugPendingBreakpoint2 * This,
            /* [in] */ BOOL fEnable);
        
        HRESULT ( STDMETHODCALLTYPE *SetCondition )( 
            IDebugPendingBreakpoint2 * This,
            /* [in] */ BP_CONDITION bpCondition);
        
        HRESULT ( STDMETHODCALLTYPE *SetPassCount )( 
            IDebugPendingBreakpoint2 * This,
            /* [in] */ BP_PASSCOUNT bpPassCount);
        
        HRESULT ( STDMETHODCALLTYPE *EnumBoundBreakpoints )( 
            IDebugPendingBreakpoint2 * This,
            /* [out] */ __RPC__deref_out_opt IEnumDebugBoundBreakpoints2 **ppEnum);
        
        HRESULT ( STDMETHODCALLTYPE *EnumErrorBreakpoints )( 
            IDebugPendingBreakpoint2 * This,
            /* [in] */ BP_ERROR_TYPE bpErrorType,
            /* [out] */ __RPC__deref_out_opt IEnumDebugErrorBreakpoints2 **ppEnum);
        
        HRESULT ( STDMETHODCALLTYPE *Delete )( 
            IDebugPendingBreakpoint2 * This);
        
        END_INTERFACE
    } IDebugPendingBreakpoint2Vtbl;

    interface IDebugPendingBreakpoint2
    {
        CONST_VTBL struct IDebugPendingBreakpoint2Vtbl *lpVtbl;
    };

    

#ifdef COBJMACROS


#define IDebugPendingBreakpoint2_QueryInterface(This,riid,ppvObject)	\
    ( (This)->lpVtbl -> QueryInterface(This,riid,ppvObject) ) 

#define IDebugPendingBreakpoint2_AddRef(This)	\
    ( (This)->lpVtbl -> AddRef(This) ) 

#define IDebugPendingBreakpoint2_Release(This)	\
    ( (This)->lpVtbl -> Release(This) ) 


#define IDebugPendingBreakpoint2_CanBind(This,ppErrorEnum)	\
    ( (This)->lpVtbl -> CanBind(This,ppErrorEnum) ) 

#define IDebugPendingBreakpoint2_Bind(This)	\
    ( (This)->lpVtbl -> Bind(This) ) 

#define IDebugPendingBreakpoint2_GetState(This,pState)	\
    ( (This)->lpVtbl -> GetState(This,pState) ) 

#define IDebugPendingBreakpoint2_GetBreakpointRequest(This,ppBPRequest)	\
    ( (This)->lpVtbl -> GetBreakpointRequest(This,ppBPRequest) ) 

#define IDebugPendingBreakpoint2_Virtualize(This,fVirtualize)	\
    ( (This)->lpVtbl -> Virtualize(This,fVirtualize) ) 

#define IDebugPendingBreakpoint2_Enable(This,fEnable)	\
    ( (This)->lpVtbl -> Enable(This,fEnable) ) 

#define IDebugPendingBreakpoint2_SetCondition(This,bpCondition)	\
    ( (This)->lpVtbl -> SetCondition(This,bpCondition) ) 

#define IDebugPendingBreakpoint2_SetPassCount(This,bpPassCount)	\
    ( (This)->lpVtbl -> SetPassCount(This,bpPassCount) ) 

#define IDebugPendingBreakpoint2_EnumBoundBreakpoints(This,ppEnum)	\
    ( (This)->lpVtbl -> EnumBoundBreakpoints(This,ppEnum) ) 

#define IDebugPendingBreakpoint2_EnumErrorBreakpoints(This,bpErrorType,ppEnum)	\
    ( (This)->lpVtbl -> EnumErrorBreakpoints(This,bpErrorType,ppEnum) ) 

#define IDebugPendingBreakpoint2_Delete(This)	\
    ( (This)->lpVtbl -> Delete(This) ) 

#endif /* COBJMACROS */


#endif 	/* C style interface */




#endif 	/* __IDebugPendingBreakpoint2_INTERFACE_DEFINED__ */


#ifndef __IDebugPendingBreakpoint3_INTERFACE_DEFINED__
#define __IDebugPendingBreakpoint3_INTERFACE_DEFINED__

/* interface IDebugPendingBreakpoint3 */
/* [unique][uuid][object] */ 


EXTERN_C const IID IID_IDebugPendingBreakpoint3;

#if defined(__cplusplus) && !defined(CINTERFACE)
    
    MIDL_INTERFACE("96643d32-2624-479a-9f1a-25d02030dd3b")
    IDebugPendingBreakpoint3 : public IDebugPendingBreakpoint2
    {
    public:
        virtual HRESULT STDMETHODCALLTYPE GetErrorResolutionInfo( 
            /* [in] */ BPERESI_FIELDS dwFields,
            /* [out] */ __RPC__out BP_ERROR_RESOLUTION_INFO *pErrorResolutionInfo) = 0;
        
    };
    
#else 	/* C style interface */

    typedef struct IDebugPendingBreakpoint3Vtbl
    {
        BEGIN_INTERFACE
        
        HRESULT ( STDMETHODCALLTYPE *QueryInterface )( 
            IDebugPendingBreakpoint3 * This,
            /* [in] */ __RPC__in REFIID riid,
            /* [iid_is][out] */ 
            __RPC__deref_out  void **ppvObject);
        
        ULONG ( STDMETHODCALLTYPE *AddRef )( 
            IDebugPendingBreakpoint3 * This);
        
        ULONG ( STDMETHODCALLTYPE *Release )( 
            IDebugPendingBreakpoint3 * This);
        
        HRESULT ( STDMETHODCALLTYPE *CanBind )( 
            IDebugPendingBreakpoint3 * This,
            /* [out] */ __RPC__deref_out_opt IEnumDebugErrorBreakpoints2 **ppErrorEnum);
        
        HRESULT ( STDMETHODCALLTYPE *Bind )( 
            IDebugPendingBreakpoint3 * This);
        
        HRESULT ( STDMETHODCALLTYPE *GetState )( 
            IDebugPendingBreakpoint3 * This,
            /* [out] */ __RPC__out PENDING_BP_STATE_INFO *pState);
        
        HRESULT ( STDMETHODCALLTYPE *GetBreakpointRequest )( 
            IDebugPendingBreakpoint3 * This,
            /* [out] */ __RPC__deref_out_opt IDebugBreakpointRequest2 **ppBPRequest);
        
        HRESULT ( STDMETHODCALLTYPE *Virtualize )( 
            IDebugPendingBreakpoint3 * This,
            /* [in] */ BOOL fVirtualize);
        
        HRESULT ( STDMETHODCALLTYPE *Enable )( 
            IDebugPendingBreakpoint3 * This,
            /* [in] */ BOOL fEnable);
        
        HRESULT ( STDMETHODCALLTYPE *SetCondition )( 
            IDebugPendingBreakpoint3 * This,
            /* [in] */ BP_CONDITION bpCondition);
        
        HRESULT ( STDMETHODCALLTYPE *SetPassCount )( 
            IDebugPendingBreakpoint3 * This,
            /* [in] */ BP_PASSCOUNT bpPassCount);
        
        HRESULT ( STDMETHODCALLTYPE *EnumBoundBreakpoints )( 
            IDebugPendingBreakpoint3 * This,
            /* [out] */ __RPC__deref_out_opt IEnumDebugBoundBreakpoints2 **ppEnum);
        
        HRESULT ( STDMETHODCALLTYPE *EnumErrorBreakpoints )( 
            IDebugPendingBreakpoint3 * This,
            /* [in] */ BP_ERROR_TYPE bpErrorType,
            /* [out] */ __RPC__deref_out_opt IEnumDebugErrorBreakpoints2 **ppEnum);
        
        HRESULT ( STDMETHODCALLTYPE *Delete )( 
            IDebugPendingBreakpoint3 * This);
        
        HRESULT ( STDMETHODCALLTYPE *GetErrorResolutionInfo )( 
            IDebugPendingBreakpoint3 * This,
            /* [in] */ BPERESI_FIELDS dwFields,
            /* [out] */ __RPC__out BP_ERROR_RESOLUTION_INFO *pErrorResolutionInfo);
        
        END_INTERFACE
    } IDebugPendingBreakpoint3Vtbl;

    interface IDebugPendingBreakpoint3
    {
        CONST_VTBL struct IDebugPendingBreakpoint3Vtbl *lpVtbl;
    };

    

#ifdef COBJMACROS


#define IDebugPendingBreakpoint3_QueryInterface(This,riid,ppvObject)	\
    ( (This)->lpVtbl -> QueryInterface(This,riid,ppvObject) ) 

#define IDebugPendingBreakpoint3_AddRef(This)	\
    ( (This)->lpVtbl -> AddRef(This) ) 

#define IDebugPendingBreakpoint3_Release(This)	\
    ( (This)->lpVtbl -> Release(This) ) 


#define IDebugPendingBreakpoint3_CanBind(This,ppErrorEnum)	\
    ( (This)->lpVtbl -> CanBind(This,ppErrorEnum) ) 

#define IDebugPendingBreakpoint3_Bind(This)	\
    ( (This)->lpVtbl -> Bind(This) ) 

#define IDebugPendingBreakpoint3_GetState(This,pState)	\
    ( (This)->lpVtbl -> GetState(This,pState) ) 

#define IDebugPendingBreakpoint3_GetBreakpointRequest(This,ppBPRequest)	\
    ( (This)->lpVtbl -> GetBreakpointRequest(This,ppBPRequest) ) 

#define IDebugPendingBreakpoint3_Virtualize(This,fVirtualize)	\
    ( (This)->lpVtbl -> Virtualize(This,fVirtualize) ) 

#define IDebugPendingBreakpoint3_Enable(This,fEnable)	\
    ( (This)->lpVtbl -> Enable(This,fEnable) ) 

#define IDebugPendingBreakpoint3_SetCondition(This,bpCondition)	\
    ( (This)->lpVtbl -> SetCondition(This,bpCondition) ) 

#define IDebugPendingBreakpoint3_SetPassCount(This,bpPassCount)	\
    ( (This)->lpVtbl -> SetPassCount(This,bpPassCount) ) 

#define IDebugPendingBreakpoint3_EnumBoundBreakpoints(This,ppEnum)	\
    ( (This)->lpVtbl -> EnumBoundBreakpoints(This,ppEnum) ) 

#define IDebugPendingBreakpoint3_EnumErrorBreakpoints(This,bpErrorType,ppEnum)	\
    ( (This)->lpVtbl -> EnumErrorBreakpoints(This,bpErrorType,ppEnum) ) 

#define IDebugPendingBreakpoint3_Delete(This)	\
    ( (This)->lpVtbl -> Delete(This) ) 


#define IDebugPendingBreakpoint3_GetErrorResolutionInfo(This,dwFields,pErrorResolutionInfo)	\
    ( (This)->lpVtbl -> GetErrorResolutionInfo(This,dwFields,pErrorResolutionInfo) ) 

#endif /* COBJMACROS */


#endif 	/* C style interface */




#endif 	/* __IDebugPendingBreakpoint3_INTERFACE_DEFINED__ */


#ifndef __IDebugErrorBreakpoint2_INTERFACE_DEFINED__
#define __IDebugErrorBreakpoint2_INTERFACE_DEFINED__

/* interface IDebugErrorBreakpoint2 */
/* [unique][uuid][object] */ 


EXTERN_C const IID IID_IDebugErrorBreakpoint2;

#if defined(__cplusplus) && !defined(CINTERFACE)
    
    MIDL_INTERFACE("74570ef7-2486-4089-800c-56e3829b5ca4")
    IDebugErrorBreakpoint2 : public IUnknown
    {
    public:
        virtual HRESULT STDMETHODCALLTYPE GetPendingBreakpoint( 
            /* [out] */ __RPC__deref_out_opt IDebugPendingBreakpoint2 **ppPendingBreakpoint) = 0;
        
        virtual HRESULT STDMETHODCALLTYPE GetBreakpointResolution( 
            /* [out] */ __RPC__deref_out_opt IDebugErrorBreakpointResolution2 **ppErrorResolution) = 0;
        
    };
    
#else 	/* C style interface */

    typedef struct IDebugErrorBreakpoint2Vtbl
    {
        BEGIN_INTERFACE
        
        HRESULT ( STDMETHODCALLTYPE *QueryInterface )( 
            IDebugErrorBreakpoint2 * This,
            /* [in] */ __RPC__in REFIID riid,
            /* [iid_is][out] */ 
            __RPC__deref_out  void **ppvObject);
        
        ULONG ( STDMETHODCALLTYPE *AddRef )( 
            IDebugErrorBreakpoint2 * This);
        
        ULONG ( STDMETHODCALLTYPE *Release )( 
            IDebugErrorBreakpoint2 * This);
        
        HRESULT ( STDMETHODCALLTYPE *GetPendingBreakpoint )( 
            IDebugErrorBreakpoint2 * This,
            /* [out] */ __RPC__deref_out_opt IDebugPendingBreakpoint2 **ppPendingBreakpoint);
        
        HRESULT ( STDMETHODCALLTYPE *GetBreakpointResolution )( 
            IDebugErrorBreakpoint2 * This,
            /* [out] */ __RPC__deref_out_opt IDebugErrorBreakpointResolution2 **ppErrorResolution);
        
        END_INTERFACE
    } IDebugErrorBreakpoint2Vtbl;

    interface IDebugErrorBreakpoint2
    {
        CONST_VTBL struct IDebugErrorBreakpoint2Vtbl *lpVtbl;
    };

    

#ifdef COBJMACROS


#define IDebugErrorBreakpoint2_QueryInterface(This,riid,ppvObject)	\
    ( (This)->lpVtbl -> QueryInterface(This,riid,ppvObject) ) 

#define IDebugErrorBreakpoint2_AddRef(This)	\
    ( (This)->lpVtbl -> AddRef(This) ) 

#define IDebugErrorBreakpoint2_Release(This)	\
    ( (This)->lpVtbl -> Release(This) ) 


#define IDebugErrorBreakpoint2_GetPendingBreakpoint(This,ppPendingBreakpoint)	\
    ( (This)->lpVtbl -> GetPendingBreakpoint(This,ppPendingBreakpoint) ) 

#define IDebugErrorBreakpoint2_GetBreakpointResolution(This,ppErrorResolution)	\
    ( (This)->lpVtbl -> GetBreakpointResolution(This,ppErrorResolution) ) 

#endif /* COBJMACROS */


#endif 	/* C style interface */




#endif 	/* __IDebugErrorBreakpoint2_INTERFACE_DEFINED__ */


#ifndef __IDebugExpression2_INTERFACE_DEFINED__
#define __IDebugExpression2_INTERFACE_DEFINED__

/* interface IDebugExpression2 */
/* [unique][uuid][object] */ 


enum enum_EVALFLAGS
    {	EVAL_RETURNVALUE	= 0x2,
	EVAL_NOSIDEEFFECTS	= 0x4,
	EVAL_ALLOWBPS	= 0x8,
	EVAL_ALLOWERRORREPORT	= 0x10,
	EVAL_FUNCTION_AS_ADDRESS	= 0x40,
	EVAL_NOFUNCEVAL	= 0x80,
	EVAL_NOEVENTS	= 0x1000,
	EVAL_DESIGN_TIME_EXPR_EVAL	= 0x2000,
	EVAL_ALLOW_IMPLICIT_VARS	= 0x4000
    } ;
typedef DWORD EVALFLAGS;


EXTERN_C const IID IID_IDebugExpression2;

#if defined(__cplusplus) && !defined(CINTERFACE)
    
    MIDL_INTERFACE("f7473fd0-7f75-478d-8d85-a485204e7a2d")
    IDebugExpression2 : public IUnknown
    {
    public:
        virtual HRESULT STDMETHODCALLTYPE EvaluateAsync( 
            /* [in] */ EVALFLAGS dwFlags,
            /* [in] */ __RPC__in_opt IDebugEventCallback2 *pExprCallback) = 0;
        
        virtual HRESULT STDMETHODCALLTYPE Abort( void) = 0;
        
        virtual HRESULT STDMETHODCALLTYPE EvaluateSync( 
            /* [in] */ EVALFLAGS dwFlags,
            /* [in] */ DWORD dwTimeout,
            /* [in] */ __RPC__in_opt IDebugEventCallback2 *pExprCallback,
            /* [out] */ __RPC__deref_out_opt IDebugProperty2 **ppResult) = 0;
        
    };
    
#else 	/* C style interface */

    typedef struct IDebugExpression2Vtbl
    {
        BEGIN_INTERFACE
        
        HRESULT ( STDMETHODCALLTYPE *QueryInterface )( 
            IDebugExpression2 * This,
            /* [in] */ __RPC__in REFIID riid,
            /* [iid_is][out] */ 
            __RPC__deref_out  void **ppvObject);
        
        ULONG ( STDMETHODCALLTYPE *AddRef )( 
            IDebugExpression2 * This);
        
        ULONG ( STDMETHODCALLTYPE *Release )( 
            IDebugExpression2 * This);
        
        HRESULT ( STDMETHODCALLTYPE *EvaluateAsync )( 
            IDebugExpression2 * This,
            /* [in] */ EVALFLAGS dwFlags,
            /* [in] */ __RPC__in_opt IDebugEventCallback2 *pExprCallback);
        
        HRESULT ( STDMETHODCALLTYPE *Abort )( 
            IDebugExpression2 * This);
        
        HRESULT ( STDMETHODCALLTYPE *EvaluateSync )( 
            IDebugExpression2 * This,
            /* [in] */ EVALFLAGS dwFlags,
            /* [in] */ DWORD dwTimeout,
            /* [in] */ __RPC__in_opt IDebugEventCallback2 *pExprCallback,
            /* [out] */ __RPC__deref_out_opt IDebugProperty2 **ppResult);
        
        END_INTERFACE
    } IDebugExpression2Vtbl;

    interface IDebugExpression2
    {
        CONST_VTBL struct IDebugExpression2Vtbl *lpVtbl;
    };

    

#ifdef COBJMACROS


#define IDebugExpression2_QueryInterface(This,riid,ppvObject)	\
    ( (This)->lpVtbl -> QueryInterface(This,riid,ppvObject) ) 

#define IDebugExpression2_AddRef(This)	\
    ( (This)->lpVtbl -> AddRef(This) ) 

#define IDebugExpression2_Release(This)	\
    ( (This)->lpVtbl -> Release(This) ) 


#define IDebugExpression2_EvaluateAsync(This,dwFlags,pExprCallback)	\
    ( (This)->lpVtbl -> EvaluateAsync(This,dwFlags,pExprCallback) ) 

#define IDebugExpression2_Abort(This)	\
    ( (This)->lpVtbl -> Abort(This) ) 

#define IDebugExpression2_EvaluateSync(This,dwFlags,dwTimeout,pExprCallback,ppResult)	\
    ( (This)->lpVtbl -> EvaluateSync(This,dwFlags,dwTimeout,pExprCallback,ppResult) ) 

#endif /* COBJMACROS */


#endif 	/* C style interface */




#endif 	/* __IDebugExpression2_INTERFACE_DEFINED__ */


#ifndef __IDebugModule2_INTERFACE_DEFINED__
#define __IDebugModule2_INTERFACE_DEFINED__

/* interface IDebugModule2 */
/* [unique][uuid][object] */ 


enum enum_MODULE_FLAGS
    {	MODULE_FLAG_NONE	= 0,
	MODULE_FLAG_SYSTEM	= 0x1,
	MODULE_FLAG_SYMBOLS	= 0x2,
	MODULE_FLAG_64BIT	= 0x4,
	MODULE_FLAG_OPTIMIZED	= 0x8,
	MODULE_FLAG_UNOPTIMIZED	= 0x10
    } ;
typedef DWORD MODULE_FLAGS;


enum enum_MODULE_INFO_FIELDS
    {	MIF_NONE	= 0,
	MIF_NAME	= 0x1,
	MIF_URL	= 0x2,
	MIF_VERSION	= 0x4,
	MIF_DEBUGMESSAGE	= 0x8,
	MIF_LOADADDRESS	= 0x10,
	MIF_PREFFEREDADDRESS	= 0x20,
	MIF_SIZE	= 0x40,
	MIF_LOADORDER	= 0x80,
	MIF_TIMESTAMP	= 0x100,
	MIF_URLSYMBOLLOCATION	= 0x200,
	MIF_FLAGS	= 0x400,
	MIF_ALLFIELDS	= 0x7ff
    } ;
typedef DWORD MODULE_INFO_FIELDS;

typedef struct _tagMODULE_INFO
    {
    MODULE_INFO_FIELDS dwValidFields;
    BSTR m_bstrName;
    BSTR m_bstrUrl;
    BSTR m_bstrVersion;
    BSTR m_bstrDebugMessage;
    UINT64 m_addrLoadAddress;
    UINT64 m_addrPreferredLoadAddress;
    DWORD m_dwSize;
    DWORD m_dwLoadOrder;
    FILETIME m_TimeStamp;
    BSTR m_bstrUrlSymbolLocation;
    MODULE_FLAGS m_dwModuleFlags;
    } 	MODULE_INFO;


EXTERN_C const IID IID_IDebugModule2;

#if defined(__cplusplus) && !defined(CINTERFACE)
    
    MIDL_INTERFACE("0fc1cd9a-b912-405c-a04c-43ce02cd7df2")
    IDebugModule2 : public IUnknown
    {
    public:
        virtual HRESULT STDMETHODCALLTYPE GetInfo( 
            /* [in] */ MODULE_INFO_FIELDS dwFields,
            /* [out] */ __RPC__out MODULE_INFO *pInfo) = 0;
        
        virtual HRESULT STDMETHODCALLTYPE ReloadSymbols_Deprecated( 
            /* [full][in] */ __RPC__in_opt LPCOLESTR pszUrlToSymbols,
            /* [out] */ __RPC__deref_out_opt BSTR *pbstrDebugMessage) = 0;
        
    };
    
#else 	/* C style interface */

    typedef struct IDebugModule2Vtbl
    {
        BEGIN_INTERFACE
        
        HRESULT ( STDMETHODCALLTYPE *QueryInterface )( 
            IDebugModule2 * This,
            /* [in] */ __RPC__in REFIID riid,
            /* [iid_is][out] */ 
            __RPC__deref_out  void **ppvObject);
        
        ULONG ( STDMETHODCALLTYPE *AddRef )( 
            IDebugModule2 * This);
        
        ULONG ( STDMETHODCALLTYPE *Release )( 
            IDebugModule2 * This);
        
        HRESULT ( STDMETHODCALLTYPE *GetInfo )( 
            IDebugModule2 * This,
            /* [in] */ MODULE_INFO_FIELDS dwFields,
            /* [out] */ __RPC__out MODULE_INFO *pInfo);
        
        HRESULT ( STDMETHODCALLTYPE *ReloadSymbols_Deprecated )( 
            IDebugModule2 * This,
            /* [full][in] */ __RPC__in_opt LPCOLESTR pszUrlToSymbols,
            /* [out] */ __RPC__deref_out_opt BSTR *pbstrDebugMessage);
        
        END_INTERFACE
    } IDebugModule2Vtbl;

    interface IDebugModule2
    {
        CONST_VTBL struct IDebugModule2Vtbl *lpVtbl;
    };

    

#ifdef COBJMACROS


#define IDebugModule2_QueryInterface(This,riid,ppvObject)	\
    ( (This)->lpVtbl -> QueryInterface(This,riid,ppvObject) ) 

#define IDebugModule2_AddRef(This)	\
    ( (This)->lpVtbl -> AddRef(This) ) 

#define IDebugModule2_Release(This)	\
    ( (This)->lpVtbl -> Release(This) ) 


#define IDebugModule2_GetInfo(This,dwFields,pInfo)	\
    ( (This)->lpVtbl -> GetInfo(This,dwFields,pInfo) ) 

#define IDebugModule2_ReloadSymbols_Deprecated(This,pszUrlToSymbols,pbstrDebugMessage)	\
    ( (This)->lpVtbl -> ReloadSymbols_Deprecated(This,pszUrlToSymbols,pbstrDebugMessage) ) 

#endif /* COBJMACROS */


#endif 	/* C style interface */




#endif 	/* __IDebugModule2_INTERFACE_DEFINED__ */


#ifndef __IDebugModule3_INTERFACE_DEFINED__
#define __IDebugModule3_INTERFACE_DEFINED__

/* interface IDebugModule3 */
/* [unique][uuid][object] */ 


enum enum_SYMBOL_SEARCH_INFO_FIELDS
    {	SSIF_NONE	= 0,
	SSIF_VERBOSE_SEARCH_INFO	= 0x1
    } ;
typedef DWORD SYMBOL_SEARCH_INFO_FIELDS;

typedef struct _tagSYMBOL_SEARCH_INFO
    {
    SYMBOL_SEARCH_INFO_FIELDS dwValidFields;
    BSTR bstrVerboseSearchInfo;
    } 	MODULE_SYMBOL_SEARCH_INFO;


EXTERN_C const IID IID_IDebugModule3;

#if defined(__cplusplus) && !defined(CINTERFACE)
    
    MIDL_INTERFACE("245F9D6A-E550-404d-82F1-FDB68281607A")
    IDebugModule3 : public IDebugModule2
    {
    public:
        virtual HRESULT STDMETHODCALLTYPE GetSymbolInfo( 
            /* [in] */ SYMBOL_SEARCH_INFO_FIELDS dwFields,
            /* [out] */ __RPC__out MODULE_SYMBOL_SEARCH_INFO *pInfo) = 0;
        
        virtual HRESULT STDMETHODCALLTYPE LoadSymbols( void) = 0;
        
        virtual HRESULT STDMETHODCALLTYPE IsUserCode( 
            /* [out] */ __RPC__out BOOL *pfUser) = 0;
        
        virtual HRESULT STDMETHODCALLTYPE SetJustMyCodeState( 
            /* [in] */ BOOL fIsUserCode) = 0;
        
    };
    
#else 	/* C style interface */

    typedef struct IDebugModule3Vtbl
    {
        BEGIN_INTERFACE
        
        HRESULT ( STDMETHODCALLTYPE *QueryInterface )( 
            IDebugModule3 * This,
            /* [in] */ __RPC__in REFIID riid,
            /* [iid_is][out] */ 
            __RPC__deref_out  void **ppvObject);
        
        ULONG ( STDMETHODCALLTYPE *AddRef )( 
            IDebugModule3 * This);
        
        ULONG ( STDMETHODCALLTYPE *Release )( 
            IDebugModule3 * This);
        
        HRESULT ( STDMETHODCALLTYPE *GetInfo )( 
            IDebugModule3 * This,
            /* [in] */ MODULE_INFO_FIELDS dwFields,
            /* [out] */ __RPC__out MODULE_INFO *pInfo);
        
        HRESULT ( STDMETHODCALLTYPE *ReloadSymbols_Deprecated )( 
            IDebugModule3 * This,
            /* [full][in] */ __RPC__in_opt LPCOLESTR pszUrlToSymbols,
            /* [out] */ __RPC__deref_out_opt BSTR *pbstrDebugMessage);
        
        HRESULT ( STDMETHODCALLTYPE *GetSymbolInfo )( 
            IDebugModule3 * This,
            /* [in] */ SYMBOL_SEARCH_INFO_FIELDS dwFields,
            /* [out] */ __RPC__out MODULE_SYMBOL_SEARCH_INFO *pInfo);
        
        HRESULT ( STDMETHODCALLTYPE *LoadSymbols )( 
            IDebugModule3 * This);
        
        HRESULT ( STDMETHODCALLTYPE *IsUserCode )( 
            IDebugModule3 * This,
            /* [out] */ __RPC__out BOOL *pfUser);
        
        HRESULT ( STDMETHODCALLTYPE *SetJustMyCodeState )( 
            IDebugModule3 * This,
            /* [in] */ BOOL fIsUserCode);
        
        END_INTERFACE
    } IDebugModule3Vtbl;

    interface IDebugModule3
    {
        CONST_VTBL struct IDebugModule3Vtbl *lpVtbl;
    };

    

#ifdef COBJMACROS


#define IDebugModule3_QueryInterface(This,riid,ppvObject)	\
    ( (This)->lpVtbl -> QueryInterface(This,riid,ppvObject) ) 

#define IDebugModule3_AddRef(This)	\
    ( (This)->lpVtbl -> AddRef(This) ) 

#define IDebugModule3_Release(This)	\
    ( (This)->lpVtbl -> Release(This) ) 


#define IDebugModule3_GetInfo(This,dwFields,pInfo)	\
    ( (This)->lpVtbl -> GetInfo(This,dwFields,pInfo) ) 

#define IDebugModule3_ReloadSymbols_Deprecated(This,pszUrlToSymbols,pbstrDebugMessage)	\
    ( (This)->lpVtbl -> ReloadSymbols_Deprecated(This,pszUrlToSymbols,pbstrDebugMessage) ) 


#define IDebugModule3_GetSymbolInfo(This,dwFields,pInfo)	\
    ( (This)->lpVtbl -> GetSymbolInfo(This,dwFields,pInfo) ) 

#define IDebugModule3_LoadSymbols(This)	\
    ( (This)->lpVtbl -> LoadSymbols(This) ) 

#define IDebugModule3_IsUserCode(This,pfUser)	\
    ( (This)->lpVtbl -> IsUserCode(This,pfUser) ) 

#define IDebugModule3_SetJustMyCodeState(This,fIsUserCode)	\
    ( (This)->lpVtbl -> SetJustMyCodeState(This,fIsUserCode) ) 

#endif /* COBJMACROS */


#endif 	/* C style interface */




#endif 	/* __IDebugModule3_INTERFACE_DEFINED__ */


#ifndef __IDebugSourceServerModule_INTERFACE_DEFINED__
#define __IDebugSourceServerModule_INTERFACE_DEFINED__

/* interface IDebugSourceServerModule */
/* [unique][uuid][object] */ 


EXTERN_C const IID IID_IDebugSourceServerModule;

#if defined(__cplusplus) && !defined(CINTERFACE)
    
    MIDL_INTERFACE("492e5541-215b-4f67-ad73-20f48614912e")
    IDebugSourceServerModule : public IUnknown
    {
    public:
        virtual HRESULT STDMETHODCALLTYPE GetSourceServerData( 
            /* [out] */ __RPC__out ULONG *pDataByteCount,
            /* [size_is][size_is][out] */ __RPC__deref_out_ecount_full_opt(*pDataByteCount) BYTE **ppData) = 0;
        
    };
    
#else 	/* C style interface */

    typedef struct IDebugSourceServerModuleVtbl
    {
        BEGIN_INTERFACE
        
        HRESULT ( STDMETHODCALLTYPE *QueryInterface )( 
            IDebugSourceServerModule * This,
            /* [in] */ __RPC__in REFIID riid,
            /* [iid_is][out] */ 
            __RPC__deref_out  void **ppvObject);
        
        ULONG ( STDMETHODCALLTYPE *AddRef )( 
            IDebugSourceServerModule * This);
        
        ULONG ( STDMETHODCALLTYPE *Release )( 
            IDebugSourceServerModule * This);
        
        HRESULT ( STDMETHODCALLTYPE *GetSourceServerData )( 
            IDebugSourceServerModule * This,
            /* [out] */ __RPC__out ULONG *pDataByteCount,
            /* [size_is][size_is][out] */ __RPC__deref_out_ecount_full_opt(*pDataByteCount) BYTE **ppData);
        
        END_INTERFACE
    } IDebugSourceServerModuleVtbl;

    interface IDebugSourceServerModule
    {
        CONST_VTBL struct IDebugSourceServerModuleVtbl *lpVtbl;
    };

    

#ifdef COBJMACROS


#define IDebugSourceServerModule_QueryInterface(This,riid,ppvObject)	\
    ( (This)->lpVtbl -> QueryInterface(This,riid,ppvObject) ) 

#define IDebugSourceServerModule_AddRef(This)	\
    ( (This)->lpVtbl -> AddRef(This) ) 

#define IDebugSourceServerModule_Release(This)	\
    ( (This)->lpVtbl -> Release(This) ) 


#define IDebugSourceServerModule_GetSourceServerData(This,pDataByteCount,ppData)	\
    ( (This)->lpVtbl -> GetSourceServerData(This,pDataByteCount,ppData) ) 

#endif /* COBJMACROS */


#endif 	/* C style interface */




#endif 	/* __IDebugSourceServerModule_INTERFACE_DEFINED__ */


#ifndef __IDebugModuleManaged_INTERFACE_DEFINED__
#define __IDebugModuleManaged_INTERFACE_DEFINED__

/* interface IDebugModuleManaged */
/* [unique][uuid][object] */ 


EXTERN_C const IID IID_IDebugModuleManaged;

#if defined(__cplusplus) && !defined(CINTERFACE)
    
    MIDL_INTERFACE("232397F8-B232-479d-B1BB-2F044C70A0F9")
    IDebugModuleManaged : public IUnknown
    {
    public:
        virtual HRESULT STDMETHODCALLTYPE GetMvid( 
            /* [out] */ __RPC__out GUID *mvid) = 0;
        
    };
    
#else 	/* C style interface */

    typedef struct IDebugModuleManagedVtbl
    {
        BEGIN_INTERFACE
        
        HRESULT ( STDMETHODCALLTYPE *QueryInterface )( 
            IDebugModuleManaged * This,
            /* [in] */ __RPC__in REFIID riid,
            /* [iid_is][out] */ 
            __RPC__deref_out  void **ppvObject);
        
        ULONG ( STDMETHODCALLTYPE *AddRef )( 
            IDebugModuleManaged * This);
        
        ULONG ( STDMETHODCALLTYPE *Release )( 
            IDebugModuleManaged * This);
        
        HRESULT ( STDMETHODCALLTYPE *GetMvid )( 
            IDebugModuleManaged * This,
            /* [out] */ __RPC__out GUID *mvid);
        
        END_INTERFACE
    } IDebugModuleManagedVtbl;

    interface IDebugModuleManaged
    {
        CONST_VTBL struct IDebugModuleManagedVtbl *lpVtbl;
    };

    

#ifdef COBJMACROS


#define IDebugModuleManaged_QueryInterface(This,riid,ppvObject)	\
    ( (This)->lpVtbl -> QueryInterface(This,riid,ppvObject) ) 

#define IDebugModuleManaged_AddRef(This)	\
    ( (This)->lpVtbl -> AddRef(This) ) 

#define IDebugModuleManaged_Release(This)	\
    ( (This)->lpVtbl -> Release(This) ) 


#define IDebugModuleManaged_GetMvid(This,mvid)	\
    ( (This)->lpVtbl -> GetMvid(This,mvid) ) 

#endif /* COBJMACROS */


#endif 	/* C style interface */




#endif 	/* __IDebugModuleManaged_INTERFACE_DEFINED__ */


#ifndef __IDebugDocument2_INTERFACE_DEFINED__
#define __IDebugDocument2_INTERFACE_DEFINED__

/* interface IDebugDocument2 */
/* [unique][uuid][object] */ 


EXTERN_C const IID IID_IDebugDocument2;

#if defined(__cplusplus) && !defined(CINTERFACE)
    
    MIDL_INTERFACE("1606dd73-5d5f-405c-b4f4-ce32baba2501")
    IDebugDocument2 : public IUnknown
    {
    public:
        virtual HRESULT STDMETHODCALLTYPE GetName( 
            /* [in] */ GETNAME_TYPE gnType,
            /* [out] */ __RPC__deref_out_opt BSTR *pbstrFileName) = 0;
        
        virtual HRESULT STDMETHODCALLTYPE GetDocumentClassId( 
            /* [out] */ __RPC__out CLSID *pclsid) = 0;
        
    };
    
#else 	/* C style interface */

    typedef struct IDebugDocument2Vtbl
    {
        BEGIN_INTERFACE
        
        HRESULT ( STDMETHODCALLTYPE *QueryInterface )( 
            IDebugDocument2 * This,
            /* [in] */ __RPC__in REFIID riid,
            /* [iid_is][out] */ 
            __RPC__deref_out  void **ppvObject);
        
        ULONG ( STDMETHODCALLTYPE *AddRef )( 
            IDebugDocument2 * This);
        
        ULONG ( STDMETHODCALLTYPE *Release )( 
            IDebugDocument2 * This);
        
        HRESULT ( STDMETHODCALLTYPE *GetName )( 
            IDebugDocument2 * This,
            /* [in] */ GETNAME_TYPE gnType,
            /* [out] */ __RPC__deref_out_opt BSTR *pbstrFileName);
        
        HRESULT ( STDMETHODCALLTYPE *GetDocumentClassId )( 
            IDebugDocument2 * This,
            /* [out] */ __RPC__out CLSID *pclsid);
        
        END_INTERFACE
    } IDebugDocument2Vtbl;

    interface IDebugDocument2
    {
        CONST_VTBL struct IDebugDocument2Vtbl *lpVtbl;
    };

    

#ifdef COBJMACROS


#define IDebugDocument2_QueryInterface(This,riid,ppvObject)	\
    ( (This)->lpVtbl -> QueryInterface(This,riid,ppvObject) ) 

#define IDebugDocument2_AddRef(This)	\
    ( (This)->lpVtbl -> AddRef(This) ) 

#define IDebugDocument2_Release(This)	\
    ( (This)->lpVtbl -> Release(This) ) 


#define IDebugDocument2_GetName(This,gnType,pbstrFileName)	\
    ( (This)->lpVtbl -> GetName(This,gnType,pbstrFileName) ) 

#define IDebugDocument2_GetDocumentClassId(This,pclsid)	\
    ( (This)->lpVtbl -> GetDocumentClassId(This,pclsid) ) 

#endif /* COBJMACROS */


#endif 	/* C style interface */




#endif 	/* __IDebugDocument2_INTERFACE_DEFINED__ */


/* interface __MIDL_itf_msdbg_0000_0114 */
/* [local] */ 

#pragma warning(push)
#pragma warning(disable:28718)


extern RPC_IF_HANDLE __MIDL_itf_msdbg_0000_0114_v0_0_c_ifspec;
extern RPC_IF_HANDLE __MIDL_itf_msdbg_0000_0114_v0_0_s_ifspec;

#ifndef __IDebugDocumentText2_INTERFACE_DEFINED__
#define __IDebugDocumentText2_INTERFACE_DEFINED__

/* interface IDebugDocumentText2 */
/* [unique][uuid][object] */ 


EXTERN_C const IID IID_IDebugDocumentText2;

#if defined(__cplusplus) && !defined(CINTERFACE)
    
    MIDL_INTERFACE("4b0645aa-08ef-4cb9-adb9-0395d6edad35")
    IDebugDocumentText2 : public IDebugDocument2
    {
    public:
        virtual HRESULT STDMETHODCALLTYPE GetSize( 
            /* [full][out][in] */ __RPC__inout_opt ULONG *pcNumLines,
            /* [full][out][in] */ __RPC__inout_opt ULONG *pcNumChars) = 0;
        
        virtual HRESULT STDMETHODCALLTYPE GetText( 
            /* [in] */ TEXT_POSITION pos,
            /* [in] */ ULONG cMaxChars,
            /* [size_is][length_is][out] */ __RPC__out_ecount_part(cMaxChars, *pcNumChars) WCHAR *pText,
            /* [out] */ __RPC__out ULONG *pcNumChars) = 0;
        
    };
    
#else 	/* C style interface */

    typedef struct IDebugDocumentText2Vtbl
    {
        BEGIN_INTERFACE
        
        HRESULT ( STDMETHODCALLTYPE *QueryInterface )( 
            IDebugDocumentText2 * This,
            /* [in] */ __RPC__in REFIID riid,
            /* [iid_is][out] */ 
            __RPC__deref_out  void **ppvObject);
        
        ULONG ( STDMETHODCALLTYPE *AddRef )( 
            IDebugDocumentText2 * This);
        
        ULONG ( STDMETHODCALLTYPE *Release )( 
            IDebugDocumentText2 * This);
        
        HRESULT ( STDMETHODCALLTYPE *GetName )( 
            IDebugDocumentText2 * This,
            /* [in] */ GETNAME_TYPE gnType,
            /* [out] */ __RPC__deref_out_opt BSTR *pbstrFileName);
        
        HRESULT ( STDMETHODCALLTYPE *GetDocumentClassId )( 
            IDebugDocumentText2 * This,
            /* [out] */ __RPC__out CLSID *pclsid);
        
        HRESULT ( STDMETHODCALLTYPE *GetSize )( 
            IDebugDocumentText2 * This,
            /* [full][out][in] */ __RPC__inout_opt ULONG *pcNumLines,
            /* [full][out][in] */ __RPC__inout_opt ULONG *pcNumChars);
        
        HRESULT ( STDMETHODCALLTYPE *GetText )( 
            IDebugDocumentText2 * This,
            /* [in] */ TEXT_POSITION pos,
            /* [in] */ ULONG cMaxChars,
            /* [size_is][length_is][out] */ __RPC__out_ecount_part(cMaxChars, *pcNumChars) WCHAR *pText,
            /* [out] */ __RPC__out ULONG *pcNumChars);
        
        END_INTERFACE
    } IDebugDocumentText2Vtbl;

    interface IDebugDocumentText2
    {
        CONST_VTBL struct IDebugDocumentText2Vtbl *lpVtbl;
    };

    

#ifdef COBJMACROS


#define IDebugDocumentText2_QueryInterface(This,riid,ppvObject)	\
    ( (This)->lpVtbl -> QueryInterface(This,riid,ppvObject) ) 

#define IDebugDocumentText2_AddRef(This)	\
    ( (This)->lpVtbl -> AddRef(This) ) 

#define IDebugDocumentText2_Release(This)	\
    ( (This)->lpVtbl -> Release(This) ) 


#define IDebugDocumentText2_GetName(This,gnType,pbstrFileName)	\
    ( (This)->lpVtbl -> GetName(This,gnType,pbstrFileName) ) 

#define IDebugDocumentText2_GetDocumentClassId(This,pclsid)	\
    ( (This)->lpVtbl -> GetDocumentClassId(This,pclsid) ) 


#define IDebugDocumentText2_GetSize(This,pcNumLines,pcNumChars)	\
    ( (This)->lpVtbl -> GetSize(This,pcNumLines,pcNumChars) ) 

#define IDebugDocumentText2_GetText(This,pos,cMaxChars,pText,pcNumChars)	\
    ( (This)->lpVtbl -> GetText(This,pos,cMaxChars,pText,pcNumChars) ) 

#endif /* COBJMACROS */


#endif 	/* C style interface */




#endif 	/* __IDebugDocumentText2_INTERFACE_DEFINED__ */


/* interface __MIDL_itf_msdbg_0000_0115 */
/* [local] */ 

#pragma warning(pop)


extern RPC_IF_HANDLE __MIDL_itf_msdbg_0000_0115_v0_0_c_ifspec;
extern RPC_IF_HANDLE __MIDL_itf_msdbg_0000_0115_v0_0_s_ifspec;

#ifndef __IDebugDocumentPosition2_INTERFACE_DEFINED__
#define __IDebugDocumentPosition2_INTERFACE_DEFINED__

/* interface IDebugDocumentPosition2 */
/* [unique][uuid][object] */ 


EXTERN_C const IID IID_IDebugDocumentPosition2;

#if defined(__cplusplus) && !defined(CINTERFACE)
    
    MIDL_INTERFACE("bdde0eee-3b8d-4c82-b529-33f16b42832e")
    IDebugDocumentPosition2 : public IUnknown
    {
    public:
        virtual HRESULT STDMETHODCALLTYPE GetFileName( 
            /* [out] */ __RPC__deref_out_opt BSTR *pbstrFileName) = 0;
        
        virtual HRESULT STDMETHODCALLTYPE GetDocument( 
            /* [out] */ __RPC__deref_out_opt IDebugDocument2 **ppDoc) = 0;
        
        virtual HRESULT STDMETHODCALLTYPE IsPositionInDocument( 
            /* [in] */ __RPC__in_opt IDebugDocument2 *pDoc) = 0;
        
        virtual HRESULT STDMETHODCALLTYPE GetRange( 
            /* [full][out][in] */ __RPC__inout_opt TEXT_POSITION *pBegPosition,
            /* [full][out][in] */ __RPC__inout_opt TEXT_POSITION *pEndPosition) = 0;
        
    };
    
#else 	/* C style interface */

    typedef struct IDebugDocumentPosition2Vtbl
    {
        BEGIN_INTERFACE
        
        HRESULT ( STDMETHODCALLTYPE *QueryInterface )( 
            IDebugDocumentPosition2 * This,
            /* [in] */ __RPC__in REFIID riid,
            /* [iid_is][out] */ 
            __RPC__deref_out  void **ppvObject);
        
        ULONG ( STDMETHODCALLTYPE *AddRef )( 
            IDebugDocumentPosition2 * This);
        
        ULONG ( STDMETHODCALLTYPE *Release )( 
            IDebugDocumentPosition2 * This);
        
        HRESULT ( STDMETHODCALLTYPE *GetFileName )( 
            IDebugDocumentPosition2 * This,
            /* [out] */ __RPC__deref_out_opt BSTR *pbstrFileName);
        
        HRESULT ( STDMETHODCALLTYPE *GetDocument )( 
            IDebugDocumentPosition2 * This,
            /* [out] */ __RPC__deref_out_opt IDebugDocument2 **ppDoc);
        
        HRESULT ( STDMETHODCALLTYPE *IsPositionInDocument )( 
            IDebugDocumentPosition2 * This,
            /* [in] */ __RPC__in_opt IDebugDocument2 *pDoc);
        
        HRESULT ( STDMETHODCALLTYPE *GetRange )( 
            IDebugDocumentPosition2 * This,
            /* [full][out][in] */ __RPC__inout_opt TEXT_POSITION *pBegPosition,
            /* [full][out][in] */ __RPC__inout_opt TEXT_POSITION *pEndPosition);
        
        END_INTERFACE
    } IDebugDocumentPosition2Vtbl;

    interface IDebugDocumentPosition2
    {
        CONST_VTBL struct IDebugDocumentPosition2Vtbl *lpVtbl;
    };

    

#ifdef COBJMACROS


#define IDebugDocumentPosition2_QueryInterface(This,riid,ppvObject)	\
    ( (This)->lpVtbl -> QueryInterface(This,riid,ppvObject) ) 

#define IDebugDocumentPosition2_AddRef(This)	\
    ( (This)->lpVtbl -> AddRef(This) ) 

#define IDebugDocumentPosition2_Release(This)	\
    ( (This)->lpVtbl -> Release(This) ) 


#define IDebugDocumentPosition2_GetFileName(This,pbstrFileName)	\
    ( (This)->lpVtbl -> GetFileName(This,pbstrFileName) ) 

#define IDebugDocumentPosition2_GetDocument(This,ppDoc)	\
    ( (This)->lpVtbl -> GetDocument(This,ppDoc) ) 

#define IDebugDocumentPosition2_IsPositionInDocument(This,pDoc)	\
    ( (This)->lpVtbl -> IsPositionInDocument(This,pDoc) ) 

#define IDebugDocumentPosition2_GetRange(This,pBegPosition,pEndPosition)	\
    ( (This)->lpVtbl -> GetRange(This,pBegPosition,pEndPosition) ) 

#endif /* COBJMACROS */


#endif 	/* C style interface */




#endif 	/* __IDebugDocumentPosition2_INTERFACE_DEFINED__ */


#ifndef __IDebugDocumentPositionOffset2_INTERFACE_DEFINED__
#define __IDebugDocumentPositionOffset2_INTERFACE_DEFINED__

/* interface IDebugDocumentPositionOffset2 */
/* [unique][uuid][object] */ 


EXTERN_C const IID IID_IDebugDocumentPositionOffset2;

#if defined(__cplusplus) && !defined(CINTERFACE)
    
    MIDL_INTERFACE("037edd0f-8551-4f7f-8ca0-04d9e29f532d")
    IDebugDocumentPositionOffset2 : public IUnknown
    {
    public:
        virtual HRESULT STDMETHODCALLTYPE GetRange( 
            /* [full][out][in] */ __RPC__inout_opt DWORD *pdwBegOffset,
            /* [full][out][in] */ __RPC__inout_opt DWORD *pdwEndOffset) = 0;
        
    };
    
#else 	/* C style interface */

    typedef struct IDebugDocumentPositionOffset2Vtbl
    {
        BEGIN_INTERFACE
        
        HRESULT ( STDMETHODCALLTYPE *QueryInterface )( 
            IDebugDocumentPositionOffset2 * This,
            /* [in] */ __RPC__in REFIID riid,
            /* [iid_is][out] */ 
            __RPC__deref_out  void **ppvObject);
        
        ULONG ( STDMETHODCALLTYPE *AddRef )( 
            IDebugDocumentPositionOffset2 * This);
        
        ULONG ( STDMETHODCALLTYPE *Release )( 
            IDebugDocumentPositionOffset2 * This);
        
        HRESULT ( STDMETHODCALLTYPE *GetRange )( 
            IDebugDocumentPositionOffset2 * This,
            /* [full][out][in] */ __RPC__inout_opt DWORD *pdwBegOffset,
            /* [full][out][in] */ __RPC__inout_opt DWORD *pdwEndOffset);
        
        END_INTERFACE
    } IDebugDocumentPositionOffset2Vtbl;

    interface IDebugDocumentPositionOffset2
    {
        CONST_VTBL struct IDebugDocumentPositionOffset2Vtbl *lpVtbl;
    };

    

#ifdef COBJMACROS


#define IDebugDocumentPositionOffset2_QueryInterface(This,riid,ppvObject)	\
    ( (This)->lpVtbl -> QueryInterface(This,riid,ppvObject) ) 

#define IDebugDocumentPositionOffset2_AddRef(This)	\
    ( (This)->lpVtbl -> AddRef(This) ) 

#define IDebugDocumentPositionOffset2_Release(This)	\
    ( (This)->lpVtbl -> Release(This) ) 


#define IDebugDocumentPositionOffset2_GetRange(This,pdwBegOffset,pdwEndOffset)	\
    ( (This)->lpVtbl -> GetRange(This,pdwBegOffset,pdwEndOffset) ) 

#endif /* COBJMACROS */


#endif 	/* C style interface */




#endif 	/* __IDebugDocumentPositionOffset2_INTERFACE_DEFINED__ */


#ifndef __IDebugFunctionPosition2_INTERFACE_DEFINED__
#define __IDebugFunctionPosition2_INTERFACE_DEFINED__

/* interface IDebugFunctionPosition2 */
/* [unique][uuid][object] */ 


EXTERN_C const IID IID_IDebugFunctionPosition2;

#if defined(__cplusplus) && !defined(CINTERFACE)
    
    MIDL_INTERFACE("1ede3b4b-35e7-4b97-8133-02845d600174")
    IDebugFunctionPosition2 : public IUnknown
    {
    public:
        virtual HRESULT STDMETHODCALLTYPE GetFunctionName( 
            /* [out] */ __RPC__deref_out_opt BSTR *pbstrFunctionName) = 0;
        
        virtual HRESULT STDMETHODCALLTYPE GetOffset( 
            /* [full][out][in] */ __RPC__inout_opt TEXT_POSITION *pPosition) = 0;
        
    };
    
#else 	/* C style interface */

    typedef struct IDebugFunctionPosition2Vtbl
    {
        BEGIN_INTERFACE
        
        HRESULT ( STDMETHODCALLTYPE *QueryInterface )( 
            IDebugFunctionPosition2 * This,
            /* [in] */ __RPC__in REFIID riid,
            /* [iid_is][out] */ 
            __RPC__deref_out  void **ppvObject);
        
        ULONG ( STDMETHODCALLTYPE *AddRef )( 
            IDebugFunctionPosition2 * This);
        
        ULONG ( STDMETHODCALLTYPE *Release )( 
            IDebugFunctionPosition2 * This);
        
        HRESULT ( STDMETHODCALLTYPE *GetFunctionName )( 
            IDebugFunctionPosition2 * This,
            /* [out] */ __RPC__deref_out_opt BSTR *pbstrFunctionName);
        
        HRESULT ( STDMETHODCALLTYPE *GetOffset )( 
            IDebugFunctionPosition2 * This,
            /* [full][out][in] */ __RPC__inout_opt TEXT_POSITION *pPosition);
        
        END_INTERFACE
    } IDebugFunctionPosition2Vtbl;

    interface IDebugFunctionPosition2
    {
        CONST_VTBL struct IDebugFunctionPosition2Vtbl *lpVtbl;
    };

    

#ifdef COBJMACROS


#define IDebugFunctionPosition2_QueryInterface(This,riid,ppvObject)	\
    ( (This)->lpVtbl -> QueryInterface(This,riid,ppvObject) ) 

#define IDebugFunctionPosition2_AddRef(This)	\
    ( (This)->lpVtbl -> AddRef(This) ) 

#define IDebugFunctionPosition2_Release(This)	\
    ( (This)->lpVtbl -> Release(This) ) 


#define IDebugFunctionPosition2_GetFunctionName(This,pbstrFunctionName)	\
    ( (This)->lpVtbl -> GetFunctionName(This,pbstrFunctionName) ) 

#define IDebugFunctionPosition2_GetOffset(This,pPosition)	\
    ( (This)->lpVtbl -> GetOffset(This,pPosition) ) 

#endif /* COBJMACROS */


#endif 	/* C style interface */




#endif 	/* __IDebugFunctionPosition2_INTERFACE_DEFINED__ */


#ifndef __IDebugDocumentTextEvents2_INTERFACE_DEFINED__
#define __IDebugDocumentTextEvents2_INTERFACE_DEFINED__

/* interface IDebugDocumentTextEvents2 */
/* [unique][uuid][object] */ 

typedef DWORD TEXT_DOC_ATTR_2;

#define	TEXT_DOC_ATTR_READONLY_2	( 0x1 )


EXTERN_C const IID IID_IDebugDocumentTextEvents2;

#if defined(__cplusplus) && !defined(CINTERFACE)
    
    MIDL_INTERFACE("33ec72e3-002f-4966-b91c-5ce2f7ba5124")
    IDebugDocumentTextEvents2 : public IUnknown
    {
    public:
        virtual HRESULT STDMETHODCALLTYPE onDestroy( void) = 0;
        
        virtual HRESULT STDMETHODCALLTYPE onInsertText( 
            /* [in] */ TEXT_POSITION pos,
            /* [in] */ DWORD dwNumToInsert) = 0;
        
        virtual HRESULT STDMETHODCALLTYPE onRemoveText( 
            /* [in] */ TEXT_POSITION pos,
            /* [in] */ DWORD dwNumToRemove) = 0;
        
        virtual HRESULT STDMETHODCALLTYPE onReplaceText( 
            /* [in] */ TEXT_POSITION pos,
            /* [in] */ DWORD dwNumToReplace) = 0;
        
        virtual HRESULT STDMETHODCALLTYPE onUpdateTextAttributes( 
            /* [in] */ TEXT_POSITION pos,
            /* [in] */ DWORD dwNumToUpdate) = 0;
        
        virtual HRESULT STDMETHODCALLTYPE onUpdateDocumentAttributes( 
            /* [in] */ TEXT_DOC_ATTR_2 textdocattr) = 0;
        
    };
    
#else 	/* C style interface */

    typedef struct IDebugDocumentTextEvents2Vtbl
    {
        BEGIN_INTERFACE
        
        HRESULT ( STDMETHODCALLTYPE *QueryInterface )( 
            IDebugDocumentTextEvents2 * This,
            /* [in] */ __RPC__in REFIID riid,
            /* [iid_is][out] */ 
            __RPC__deref_out  void **ppvObject);
        
        ULONG ( STDMETHODCALLTYPE *AddRef )( 
            IDebugDocumentTextEvents2 * This);
        
        ULONG ( STDMETHODCALLTYPE *Release )( 
            IDebugDocumentTextEvents2 * This);
        
        HRESULT ( STDMETHODCALLTYPE *onDestroy )( 
            IDebugDocumentTextEvents2 * This);
        
        HRESULT ( STDMETHODCALLTYPE *onInsertText )( 
            IDebugDocumentTextEvents2 * This,
            /* [in] */ TEXT_POSITION pos,
            /* [in] */ DWORD dwNumToInsert);
        
        HRESULT ( STDMETHODCALLTYPE *onRemoveText )( 
            IDebugDocumentTextEvents2 * This,
            /* [in] */ TEXT_POSITION pos,
            /* [in] */ DWORD dwNumToRemove);
        
        HRESULT ( STDMETHODCALLTYPE *onReplaceText )( 
            IDebugDocumentTextEvents2 * This,
            /* [in] */ TEXT_POSITION pos,
            /* [in] */ DWORD dwNumToReplace);
        
        HRESULT ( STDMETHODCALLTYPE *onUpdateTextAttributes )( 
            IDebugDocumentTextEvents2 * This,
            /* [in] */ TEXT_POSITION pos,
            /* [in] */ DWORD dwNumToUpdate);
        
        HRESULT ( STDMETHODCALLTYPE *onUpdateDocumentAttributes )( 
            IDebugDocumentTextEvents2 * This,
            /* [in] */ TEXT_DOC_ATTR_2 textdocattr);
        
        END_INTERFACE
    } IDebugDocumentTextEvents2Vtbl;

    interface IDebugDocumentTextEvents2
    {
        CONST_VTBL struct IDebugDocumentTextEvents2Vtbl *lpVtbl;
    };

    

#ifdef COBJMACROS


#define IDebugDocumentTextEvents2_QueryInterface(This,riid,ppvObject)	\
    ( (This)->lpVtbl -> QueryInterface(This,riid,ppvObject) ) 

#define IDebugDocumentTextEvents2_AddRef(This)	\
    ( (This)->lpVtbl -> AddRef(This) ) 

#define IDebugDocumentTextEvents2_Release(This)	\
    ( (This)->lpVtbl -> Release(This) ) 


#define IDebugDocumentTextEvents2_onDestroy(This)	\
    ( (This)->lpVtbl -> onDestroy(This) ) 

#define IDebugDocumentTextEvents2_onInsertText(This,pos,dwNumToInsert)	\
    ( (This)->lpVtbl -> onInsertText(This,pos,dwNumToInsert) ) 

#define IDebugDocumentTextEvents2_onRemoveText(This,pos,dwNumToRemove)	\
    ( (This)->lpVtbl -> onRemoveText(This,pos,dwNumToRemove) ) 

#define IDebugDocumentTextEvents2_onReplaceText(This,pos,dwNumToReplace)	\
    ( (This)->lpVtbl -> onReplaceText(This,pos,dwNumToReplace) ) 

#define IDebugDocumentTextEvents2_onUpdateTextAttributes(This,pos,dwNumToUpdate)	\
    ( (This)->lpVtbl -> onUpdateTextAttributes(This,pos,dwNumToUpdate) ) 

#define IDebugDocumentTextEvents2_onUpdateDocumentAttributes(This,textdocattr)	\
    ( (This)->lpVtbl -> onUpdateDocumentAttributes(This,textdocattr) ) 

#endif /* COBJMACROS */


#endif 	/* C style interface */




#endif 	/* __IDebugDocumentTextEvents2_INTERFACE_DEFINED__ */


#ifndef __IDebugQueryEngine2_INTERFACE_DEFINED__
#define __IDebugQueryEngine2_INTERFACE_DEFINED__

/* interface IDebugQueryEngine2 */
/* [unique][uuid][object] */ 


EXTERN_C const IID IID_IDebugQueryEngine2;

#if defined(__cplusplus) && !defined(CINTERFACE)
    
    MIDL_INTERFACE("c989adc9-f305-4ef5-8ca2-20898e8d0e28")
    IDebugQueryEngine2 : public IUnknown
    {
    public:
        virtual HRESULT STDMETHODCALLTYPE GetEngineInterface( 
            /* [out] */ __RPC__deref_out_opt IUnknown **ppUnk) = 0;
        
    };
    
#else 	/* C style interface */

    typedef struct IDebugQueryEngine2Vtbl
    {
        BEGIN_INTERFACE
        
        HRESULT ( STDMETHODCALLTYPE *QueryInterface )( 
            IDebugQueryEngine2 * This,
            /* [in] */ __RPC__in REFIID riid,
            /* [iid_is][out] */ 
            __RPC__deref_out  void **ppvObject);
        
        ULONG ( STDMETHODCALLTYPE *AddRef )( 
            IDebugQueryEngine2 * This);
        
        ULONG ( STDMETHODCALLTYPE *Release )( 
            IDebugQueryEngine2 * This);
        
        HRESULT ( STDMETHODCALLTYPE *GetEngineInterface )( 
            IDebugQueryEngine2 * This,
            /* [out] */ __RPC__deref_out_opt IUnknown **ppUnk);
        
        END_INTERFACE
    } IDebugQueryEngine2Vtbl;

    interface IDebugQueryEngine2
    {
        CONST_VTBL struct IDebugQueryEngine2Vtbl *lpVtbl;
    };

    

#ifdef COBJMACROS


#define IDebugQueryEngine2_QueryInterface(This,riid,ppvObject)	\
    ( (This)->lpVtbl -> QueryInterface(This,riid,ppvObject) ) 

#define IDebugQueryEngine2_AddRef(This)	\
    ( (This)->lpVtbl -> AddRef(This) ) 

#define IDebugQueryEngine2_Release(This)	\
    ( (This)->lpVtbl -> Release(This) ) 


#define IDebugQueryEngine2_GetEngineInterface(This,ppUnk)	\
    ( (This)->lpVtbl -> GetEngineInterface(This,ppUnk) ) 

#endif /* COBJMACROS */


#endif 	/* C style interface */




#endif 	/* __IDebugQueryEngine2_INTERFACE_DEFINED__ */


#ifndef __IEEHostServices_INTERFACE_DEFINED__
#define __IEEHostServices_INTERFACE_DEFINED__

/* interface IEEHostServices */
/* [unique][uuid][object] */ 


EXTERN_C const IID IID_IEEHostServices;

#if defined(__cplusplus) && !defined(CINTERFACE)
    
    MIDL_INTERFACE("BB7BE481-DA8F-4b9e-89CB-0A8DDE6BC5D7")
    IEEHostServices : public IUnknown
    {
    public:
        virtual HRESULT STDMETHODCALLTYPE GetHostValue( 
            /* [in] */ __RPC__in LPCOLESTR valueCatagory,
            /* [in] */ __RPC__in LPCOLESTR valueKind,
            /* [out] */ __RPC__out VARIANT *result) = 0;
        
        virtual HRESULT STDMETHODCALLTYPE SetHostValue( 
            /* [in] */ __RPC__in LPCOLESTR valueCatagory,
            /* [in] */ __RPC__in LPCOLESTR valueKind,
            /* [in] */ VARIANT newValue) = 0;
        
    };
    
#else 	/* C style interface */

    typedef struct IEEHostServicesVtbl
    {
        BEGIN_INTERFACE
        
        HRESULT ( STDMETHODCALLTYPE *QueryInterface )( 
            IEEHostServices * This,
            /* [in] */ __RPC__in REFIID riid,
            /* [iid_is][out] */ 
            __RPC__deref_out  void **ppvObject);
        
        ULONG ( STDMETHODCALLTYPE *AddRef )( 
            IEEHostServices * This);
        
        ULONG ( STDMETHODCALLTYPE *Release )( 
            IEEHostServices * This);
        
        HRESULT ( STDMETHODCALLTYPE *GetHostValue )( 
            IEEHostServices * This,
            /* [in] */ __RPC__in LPCOLESTR valueCatagory,
            /* [in] */ __RPC__in LPCOLESTR valueKind,
            /* [out] */ __RPC__out VARIANT *result);
        
        HRESULT ( STDMETHODCALLTYPE *SetHostValue )( 
            IEEHostServices * This,
            /* [in] */ __RPC__in LPCOLESTR valueCatagory,
            /* [in] */ __RPC__in LPCOLESTR valueKind,
            /* [in] */ VARIANT newValue);
        
        END_INTERFACE
    } IEEHostServicesVtbl;

    interface IEEHostServices
    {
        CONST_VTBL struct IEEHostServicesVtbl *lpVtbl;
    };

    

#ifdef COBJMACROS


#define IEEHostServices_QueryInterface(This,riid,ppvObject)	\
    ( (This)->lpVtbl -> QueryInterface(This,riid,ppvObject) ) 

#define IEEHostServices_AddRef(This)	\
    ( (This)->lpVtbl -> AddRef(This) ) 

#define IEEHostServices_Release(This)	\
    ( (This)->lpVtbl -> Release(This) ) 


#define IEEHostServices_GetHostValue(This,valueCatagory,valueKind,result)	\
    ( (This)->lpVtbl -> GetHostValue(This,valueCatagory,valueKind,result) ) 

#define IEEHostServices_SetHostValue(This,valueCatagory,valueKind,newValue)	\
    ( (This)->lpVtbl -> SetHostValue(This,valueCatagory,valueKind,newValue) ) 

#endif /* COBJMACROS */


#endif 	/* C style interface */




#endif 	/* __IEEHostServices_INTERFACE_DEFINED__ */


#ifndef __IDebugCustomViewer_INTERFACE_DEFINED__
#define __IDebugCustomViewer_INTERFACE_DEFINED__

/* interface IDebugCustomViewer */
/* [unique][uuid][object] */ 


EXTERN_C const IID IID_IDebugCustomViewer;

#if defined(__cplusplus) && !defined(CINTERFACE)
    
    MIDL_INTERFACE("6306E526-9E02-4696-BFF9-48338A27F8AF")
    IDebugCustomViewer : public IUnknown
    {
    public:
        virtual HRESULT STDMETHODCALLTYPE DisplayValue( 
            /* [in] */ __RPC__in HWND hwnd,
            /* [in] */ DWORD dwID,
            /* [in] */ __RPC__in_opt IUnknown *pHostServices,
            /* [in] */ __RPC__in_opt IDebugProperty3 *pDebugProperty) = 0;
        
    };
    
#else 	/* C style interface */

    typedef struct IDebugCustomViewerVtbl
    {
        BEGIN_INTERFACE
        
        HRESULT ( STDMETHODCALLTYPE *QueryInterface )( 
            IDebugCustomViewer * This,
            /* [in] */ __RPC__in REFIID riid,
            /* [iid_is][out] */ 
            __RPC__deref_out  void **ppvObject);
        
        ULONG ( STDMETHODCALLTYPE *AddRef )( 
            IDebugCustomViewer * This);
        
        ULONG ( STDMETHODCALLTYPE *Release )( 
            IDebugCustomViewer * This);
        
        HRESULT ( STDMETHODCALLTYPE *DisplayValue )( 
            IDebugCustomViewer * This,
            /* [in] */ __RPC__in HWND hwnd,
            /* [in] */ DWORD dwID,
            /* [in] */ __RPC__in_opt IUnknown *pHostServices,
            /* [in] */ __RPC__in_opt IDebugProperty3 *pDebugProperty);
        
        END_INTERFACE
    } IDebugCustomViewerVtbl;

    interface IDebugCustomViewer
    {
        CONST_VTBL struct IDebugCustomViewerVtbl *lpVtbl;
    };

    

#ifdef COBJMACROS


#define IDebugCustomViewer_QueryInterface(This,riid,ppvObject)	\
    ( (This)->lpVtbl -> QueryInterface(This,riid,ppvObject) ) 

#define IDebugCustomViewer_AddRef(This)	\
    ( (This)->lpVtbl -> AddRef(This) ) 

#define IDebugCustomViewer_Release(This)	\
    ( (This)->lpVtbl -> Release(This) ) 


#define IDebugCustomViewer_DisplayValue(This,hwnd,dwID,pHostServices,pDebugProperty)	\
    ( (This)->lpVtbl -> DisplayValue(This,hwnd,dwID,pHostServices,pDebugProperty) ) 

#endif /* COBJMACROS */


#endif 	/* C style interface */




#endif 	/* __IDebugCustomViewer_INTERFACE_DEFINED__ */


#ifndef __IEEDataStorage_INTERFACE_DEFINED__
#define __IEEDataStorage_INTERFACE_DEFINED__

/* interface IEEDataStorage */
/* [unique][uuid][object] */ 


EXTERN_C const IID IID_IEEDataStorage;

#if defined(__cplusplus) && !defined(CINTERFACE)
    
    MIDL_INTERFACE("DCF1F227-EC51-4680-8722-C8796A5F3483")
    IEEDataStorage : public IUnknown
    {
    public:
        virtual HRESULT STDMETHODCALLTYPE GetSize( 
            /* [out] */ __RPC__out ULONG *size) = 0;
        
        virtual HRESULT STDMETHODCALLTYPE GetData( 
            /* [in] */ ULONG dataSize,
            /* [out] */ __RPC__out ULONG *sizeGotten,
            /* [length_is][size_is][out] */ __RPC__out_ecount_part(dataSize, *sizeGotten) BYTE *data) = 0;
        
    };
    
#else 	/* C style interface */

    typedef struct IEEDataStorageVtbl
    {
        BEGIN_INTERFACE
        
        HRESULT ( STDMETHODCALLTYPE *QueryInterface )( 
            IEEDataStorage * This,
            /* [in] */ __RPC__in REFIID riid,
            /* [iid_is][out] */ 
            __RPC__deref_out  void **ppvObject);
        
        ULONG ( STDMETHODCALLTYPE *AddRef )( 
            IEEDataStorage * This);
        
        ULONG ( STDMETHODCALLTYPE *Release )( 
            IEEDataStorage * This);
        
        HRESULT ( STDMETHODCALLTYPE *GetSize )( 
            IEEDataStorage * This,
            /* [out] */ __RPC__out ULONG *size);
        
        HRESULT ( STDMETHODCALLTYPE *GetData )( 
            IEEDataStorage * This,
            /* [in] */ ULONG dataSize,
            /* [out] */ __RPC__out ULONG *sizeGotten,
            /* [length_is][size_is][out] */ __RPC__out_ecount_part(dataSize, *sizeGotten) BYTE *data);
        
        END_INTERFACE
    } IEEDataStorageVtbl;

    interface IEEDataStorage
    {
        CONST_VTBL struct IEEDataStorageVtbl *lpVtbl;
    };

    

#ifdef COBJMACROS


#define IEEDataStorage_QueryInterface(This,riid,ppvObject)	\
    ( (This)->lpVtbl -> QueryInterface(This,riid,ppvObject) ) 

#define IEEDataStorage_AddRef(This)	\
    ( (This)->lpVtbl -> AddRef(This) ) 

#define IEEDataStorage_Release(This)	\
    ( (This)->lpVtbl -> Release(This) ) 


#define IEEDataStorage_GetSize(This,size)	\
    ( (This)->lpVtbl -> GetSize(This,size) ) 

#define IEEDataStorage_GetData(This,dataSize,sizeGotten,data)	\
    ( (This)->lpVtbl -> GetData(This,dataSize,sizeGotten,data) ) 

#endif /* COBJMACROS */


#endif 	/* C style interface */




#endif 	/* __IEEDataStorage_INTERFACE_DEFINED__ */


#ifndef __IPropertyProxyEESide_INTERFACE_DEFINED__
#define __IPropertyProxyEESide_INTERFACE_DEFINED__

/* interface IPropertyProxyEESide */
/* [unique][uuid][object] */ 


enum enum_ASSEMBLYLOCRESOLUTION
    {	ALR_NAME	= 0,
	ALR_USERDIR	= 0x1,
	ALR_SHAREDDIR	= 0x2,
	ALR_REMOTEDIR	= 0x4,
	ALR_ERROR	= 0x8,
	ALR_BYTES	= 0x10
    } ;
typedef DWORD ASSEMBLYLOCRESOLUTION;


enum enum_GETASSEMBLY
    {	GA_BYTES	= 0x1,
	GA_PDBBYTES	= 0x2,
	GA_NAME	= 0x4,
	GA_FLAGS	= 0x8
    } ;
typedef DWORD GETASSEMBLY;


EXTERN_C const IID IID_IPropertyProxyEESide;

#if defined(__cplusplus) && !defined(CINTERFACE)
    
    MIDL_INTERFACE("579919D2-1B10-4584-969C-3E065BD3E22D")
    IPropertyProxyEESide : public IUnknown
    {
    public:
        virtual HRESULT STDMETHODCALLTYPE InitSourceDataProvider( 
            /* [out] */ __RPC__deref_out_opt IEEDataStorage **dataOut) = 0;
        
        virtual HRESULT STDMETHODCALLTYPE GetManagedViewerCreationData( 
            /* [out] */ __RPC__deref_out_opt BSTR *assemName,
            /* [out] */ __RPC__deref_out_opt IEEDataStorage **assemBytes,
            /* [out] */ __RPC__deref_out_opt IEEDataStorage **assemPdb,
            /* [out] */ __RPC__deref_out_opt BSTR *className,
            /* [out] */ __RPC__out ASSEMBLYLOCRESOLUTION *alr,
            /* [out] */ __RPC__out BOOL *replacementOk) = 0;
        
        virtual HRESULT STDMETHODCALLTYPE GetInitialData( 
            /* [out] */ __RPC__deref_out_opt IEEDataStorage **dataOut) = 0;
        
        virtual HRESULT STDMETHODCALLTYPE CreateReplacementObject( 
            /* [in] */ __RPC__in_opt IEEDataStorage *dataIn,
            /* [out] */ __RPC__deref_out_opt IEEDataStorage **dataOut) = 0;
        
        virtual HRESULT STDMETHODCALLTYPE InPlaceUpdateObject( 
            /* [in] */ __RPC__in_opt IEEDataStorage *dataIn,
            /* [out] */ __RPC__deref_out_opt IEEDataStorage **dataOut) = 0;
        
        virtual HRESULT STDMETHODCALLTYPE ResolveAssemblyReference( 
            /* [in] */ __RPC__in LPCOLESTR assemName,
            /* [in] */ GETASSEMBLY flags,
            /* [out] */ __RPC__deref_out_opt IEEDataStorage **assemBytes,
            /* [out] */ __RPC__deref_out_opt IEEDataStorage **assemPdb,
            /* [out] */ __RPC__deref_out_opt BSTR *assemLocation,
            /* [out] */ __RPC__out ASSEMBLYLOCRESOLUTION *alr) = 0;
        
    };
    
#else 	/* C style interface */

    typedef struct IPropertyProxyEESideVtbl
    {
        BEGIN_INTERFACE
        
        HRESULT ( STDMETHODCALLTYPE *QueryInterface )( 
            IPropertyProxyEESide * This,
            /* [in] */ __RPC__in REFIID riid,
            /* [iid_is][out] */ 
            __RPC__deref_out  void **ppvObject);
        
        ULONG ( STDMETHODCALLTYPE *AddRef )( 
            IPropertyProxyEESide * This);
        
        ULONG ( STDMETHODCALLTYPE *Release )( 
            IPropertyProxyEESide * This);
        
        HRESULT ( STDMETHODCALLTYPE *InitSourceDataProvider )( 
            IPropertyProxyEESide * This,
            /* [out] */ __RPC__deref_out_opt IEEDataStorage **dataOut);
        
        HRESULT ( STDMETHODCALLTYPE *GetManagedViewerCreationData )( 
            IPropertyProxyEESide * This,
            /* [out] */ __RPC__deref_out_opt BSTR *assemName,
            /* [out] */ __RPC__deref_out_opt IEEDataStorage **assemBytes,
            /* [out] */ __RPC__deref_out_opt IEEDataStorage **assemPdb,
            /* [out] */ __RPC__deref_out_opt BSTR *className,
            /* [out] */ __RPC__out ASSEMBLYLOCRESOLUTION *alr,
            /* [out] */ __RPC__out BOOL *replacementOk);
        
        HRESULT ( STDMETHODCALLTYPE *GetInitialData )( 
            IPropertyProxyEESide * This,
            /* [out] */ __RPC__deref_out_opt IEEDataStorage **dataOut);
        
        HRESULT ( STDMETHODCALLTYPE *CreateReplacementObject )( 
            IPropertyProxyEESide * This,
            /* [in] */ __RPC__in_opt IEEDataStorage *dataIn,
            /* [out] */ __RPC__deref_out_opt IEEDataStorage **dataOut);
        
        HRESULT ( STDMETHODCALLTYPE *InPlaceUpdateObject )( 
            IPropertyProxyEESide * This,
            /* [in] */ __RPC__in_opt IEEDataStorage *dataIn,
            /* [out] */ __RPC__deref_out_opt IEEDataStorage **dataOut);
        
        HRESULT ( STDMETHODCALLTYPE *ResolveAssemblyReference )( 
            IPropertyProxyEESide * This,
            /* [in] */ __RPC__in LPCOLESTR assemName,
            /* [in] */ GETASSEMBLY flags,
            /* [out] */ __RPC__deref_out_opt IEEDataStorage **assemBytes,
            /* [out] */ __RPC__deref_out_opt IEEDataStorage **assemPdb,
            /* [out] */ __RPC__deref_out_opt BSTR *assemLocation,
            /* [out] */ __RPC__out ASSEMBLYLOCRESOLUTION *alr);
        
        END_INTERFACE
    } IPropertyProxyEESideVtbl;

    interface IPropertyProxyEESide
    {
        CONST_VTBL struct IPropertyProxyEESideVtbl *lpVtbl;
    };

    

#ifdef COBJMACROS


#define IPropertyProxyEESide_QueryInterface(This,riid,ppvObject)	\
    ( (This)->lpVtbl -> QueryInterface(This,riid,ppvObject) ) 

#define IPropertyProxyEESide_AddRef(This)	\
    ( (This)->lpVtbl -> AddRef(This) ) 

#define IPropertyProxyEESide_Release(This)	\
    ( (This)->lpVtbl -> Release(This) ) 


#define IPropertyProxyEESide_InitSourceDataProvider(This,dataOut)	\
    ( (This)->lpVtbl -> InitSourceDataProvider(This,dataOut) ) 

#define IPropertyProxyEESide_GetManagedViewerCreationData(This,assemName,assemBytes,assemPdb,className,alr,replacementOk)	\
    ( (This)->lpVtbl -> GetManagedViewerCreationData(This,assemName,assemBytes,assemPdb,className,alr,replacementOk) ) 

#define IPropertyProxyEESide_GetInitialData(This,dataOut)	\
    ( (This)->lpVtbl -> GetInitialData(This,dataOut) ) 

#define IPropertyProxyEESide_CreateReplacementObject(This,dataIn,dataOut)	\
    ( (This)->lpVtbl -> CreateReplacementObject(This,dataIn,dataOut) ) 

#define IPropertyProxyEESide_InPlaceUpdateObject(This,dataIn,dataOut)	\
    ( (This)->lpVtbl -> InPlaceUpdateObject(This,dataIn,dataOut) ) 

#define IPropertyProxyEESide_ResolveAssemblyReference(This,assemName,flags,assemBytes,assemPdb,assemLocation,alr)	\
    ( (This)->lpVtbl -> ResolveAssemblyReference(This,assemName,flags,assemBytes,assemPdb,assemLocation,alr) ) 

#endif /* COBJMACROS */


#endif 	/* C style interface */




#endif 	/* __IPropertyProxyEESide_INTERFACE_DEFINED__ */


#ifndef __IPropertyProxyProvider_INTERFACE_DEFINED__
#define __IPropertyProxyProvider_INTERFACE_DEFINED__

/* interface IPropertyProxyProvider */
/* [unique][uuid][object] */ 


EXTERN_C const IID IID_IPropertyProxyProvider;

#if defined(__cplusplus) && !defined(CINTERFACE)
    
    MIDL_INTERFACE("30E6C90E-757E-48cf-8DB8-20B061AFBBAE")
    IPropertyProxyProvider : public IUnknown
    {
    public:
        virtual HRESULT STDMETHODCALLTYPE GetPropertyProxy( 
            /* [in] */ DWORD dwID,
            /* [out] */ __RPC__deref_out_opt IPropertyProxyEESide **proxy) = 0;
        
    };
    
#else 	/* C style interface */

    typedef struct IPropertyProxyProviderVtbl
    {
        BEGIN_INTERFACE
        
        HRESULT ( STDMETHODCALLTYPE *QueryInterface )( 
            IPropertyProxyProvider * This,
            /* [in] */ __RPC__in REFIID riid,
            /* [iid_is][out] */ 
            __RPC__deref_out  void **ppvObject);
        
        ULONG ( STDMETHODCALLTYPE *AddRef )( 
            IPropertyProxyProvider * This);
        
        ULONG ( STDMETHODCALLTYPE *Release )( 
            IPropertyProxyProvider * This);
        
        HRESULT ( STDMETHODCALLTYPE *GetPropertyProxy )( 
            IPropertyProxyProvider * This,
            /* [in] */ DWORD dwID,
            /* [out] */ __RPC__deref_out_opt IPropertyProxyEESide **proxy);
        
        END_INTERFACE
    } IPropertyProxyProviderVtbl;

    interface IPropertyProxyProvider
    {
        CONST_VTBL struct IPropertyProxyProviderVtbl *lpVtbl;
    };

    

#ifdef COBJMACROS


#define IPropertyProxyProvider_QueryInterface(This,riid,ppvObject)	\
    ( (This)->lpVtbl -> QueryInterface(This,riid,ppvObject) ) 

#define IPropertyProxyProvider_AddRef(This)	\
    ( (This)->lpVtbl -> AddRef(This) ) 

#define IPropertyProxyProvider_Release(This)	\
    ( (This)->lpVtbl -> Release(This) ) 


#define IPropertyProxyProvider_GetPropertyProxy(This,dwID,proxy)	\
    ( (This)->lpVtbl -> GetPropertyProxy(This,dwID,proxy) ) 

#endif /* COBJMACROS */


#endif 	/* C style interface */




#endif 	/* __IPropertyProxyProvider_INTERFACE_DEFINED__ */


#ifndef __IManagedViewerHost_INTERFACE_DEFINED__
#define __IManagedViewerHost_INTERFACE_DEFINED__

/* interface IManagedViewerHost */
/* [unique][uuid][object] */ 


EXTERN_C const IID IID_IManagedViewerHost;

#if defined(__cplusplus) && !defined(CINTERFACE)
    
    MIDL_INTERFACE("5968D43D-D21E-437c-9C71-77C52C3E287A")
    IManagedViewerHost : public IUnknown
    {
    public:
        virtual HRESULT STDMETHODCALLTYPE CreateViewer( 
            /* [in] */ ULONG hwnd,
            /* [in] */ __RPC__in_opt IUnknown *hostServices,
            /* [in] */ __RPC__in_opt IPropertyProxyEESide *property) = 0;
        
    };
    
#else 	/* C style interface */

    typedef struct IManagedViewerHostVtbl
    {
        BEGIN_INTERFACE
        
        HRESULT ( STDMETHODCALLTYPE *QueryInterface )( 
            IManagedViewerHost * This,
            /* [in] */ __RPC__in REFIID riid,
            /* [iid_is][out] */ 
            __RPC__deref_out  void **ppvObject);
        
        ULONG ( STDMETHODCALLTYPE *AddRef )( 
            IManagedViewerHost * This);
        
        ULONG ( STDMETHODCALLTYPE *Release )( 
            IManagedViewerHost * This);
        
        HRESULT ( STDMETHODCALLTYPE *CreateViewer )( 
            IManagedViewerHost * This,
            /* [in] */ ULONG hwnd,
            /* [in] */ __RPC__in_opt IUnknown *hostServices,
            /* [in] */ __RPC__in_opt IPropertyProxyEESide *property);
        
        END_INTERFACE
    } IManagedViewerHostVtbl;

    interface IManagedViewerHost
    {
        CONST_VTBL struct IManagedViewerHostVtbl *lpVtbl;
    };

    

#ifdef COBJMACROS


#define IManagedViewerHost_QueryInterface(This,riid,ppvObject)	\
    ( (This)->lpVtbl -> QueryInterface(This,riid,ppvObject) ) 

#define IManagedViewerHost_AddRef(This)	\
    ( (This)->lpVtbl -> AddRef(This) ) 

#define IManagedViewerHost_Release(This)	\
    ( (This)->lpVtbl -> Release(This) ) 


#define IManagedViewerHost_CreateViewer(This,hwnd,hostServices,property)	\
    ( (This)->lpVtbl -> CreateViewer(This,hwnd,hostServices,property) ) 

#endif /* COBJMACROS */


#endif 	/* C style interface */




#endif 	/* __IManagedViewerHost_INTERFACE_DEFINED__ */


#ifndef __IEELocalObject_INTERFACE_DEFINED__
#define __IEELocalObject_INTERFACE_DEFINED__

/* interface IEELocalObject */
/* [unique][uuid][object] */ 


EXTERN_C const IID IID_IEELocalObject;

#if defined(__cplusplus) && !defined(CINTERFACE)
    
    MIDL_INTERFACE("44F8F85F-5514-49a3-8173-6F9C9F1C4832")
    IEELocalObject : public IUnknown
    {
    public:
        virtual HRESULT STDMETHODCALLTYPE SetCallback( 
            __RPC__in_opt IDebugSettingsCallback2 *pCallback) = 0;
        
    };
    
#else 	/* C style interface */

    typedef struct IEELocalObjectVtbl
    {
        BEGIN_INTERFACE
        
        HRESULT ( STDMETHODCALLTYPE *QueryInterface )( 
            IEELocalObject * This,
            /* [in] */ __RPC__in REFIID riid,
            /* [iid_is][out] */ 
            __RPC__deref_out  void **ppvObject);
        
        ULONG ( STDMETHODCALLTYPE *AddRef )( 
            IEELocalObject * This);
        
        ULONG ( STDMETHODCALLTYPE *Release )( 
            IEELocalObject * This);
        
        HRESULT ( STDMETHODCALLTYPE *SetCallback )( 
            IEELocalObject * This,
            __RPC__in_opt IDebugSettingsCallback2 *pCallback);
        
        END_INTERFACE
    } IEELocalObjectVtbl;

    interface IEELocalObject
    {
        CONST_VTBL struct IEELocalObjectVtbl *lpVtbl;
    };

    

#ifdef COBJMACROS


#define IEELocalObject_QueryInterface(This,riid,ppvObject)	\
    ( (This)->lpVtbl -> QueryInterface(This,riid,ppvObject) ) 

#define IEELocalObject_AddRef(This)	\
    ( (This)->lpVtbl -> AddRef(This) ) 

#define IEELocalObject_Release(This)	\
    ( (This)->lpVtbl -> Release(This) ) 


#define IEELocalObject_SetCallback(This,pCallback)	\
    ( (This)->lpVtbl -> SetCallback(This,pCallback) ) 

#endif /* COBJMACROS */


#endif 	/* C style interface */




#endif 	/* __IEELocalObject_INTERFACE_DEFINED__ */


#ifndef __IEEAssemblyRefResolveComparer_INTERFACE_DEFINED__
#define __IEEAssemblyRefResolveComparer_INTERFACE_DEFINED__

/* interface IEEAssemblyRefResolveComparer */
/* [unique][uuid][object] */ 


EXTERN_C const IID IID_IEEAssemblyRefResolveComparer;

#if defined(__cplusplus) && !defined(CINTERFACE)
    
    MIDL_INTERFACE("6F1A544C-E69E-4a52-9EA1-25C897B05BEF")
    IEEAssemblyRefResolveComparer : public IUnknown
    {
    public:
        virtual HRESULT STDMETHODCALLTYPE CompareRef( 
            /* [in] */ DWORD cookieFirst,
            /* [in] */ DWORD cookieSecond,
            /* [in] */ DWORD cookieTarget,
            /* [out] */ __RPC__out BOOL *firstIsBetter) = 0;
        
    };
    
#else 	/* C style interface */

    typedef struct IEEAssemblyRefResolveComparerVtbl
    {
        BEGIN_INTERFACE
        
        HRESULT ( STDMETHODCALLTYPE *QueryInterface )( 
            IEEAssemblyRefResolveComparer * This,
            /* [in] */ __RPC__in REFIID riid,
            /* [iid_is][out] */ 
            __RPC__deref_out  void **ppvObject);
        
        ULONG ( STDMETHODCALLTYPE *AddRef )( 
            IEEAssemblyRefResolveComparer * This);
        
        ULONG ( STDMETHODCALLTYPE *Release )( 
            IEEAssemblyRefResolveComparer * This);
        
        HRESULT ( STDMETHODCALLTYPE *CompareRef )( 
            IEEAssemblyRefResolveComparer * This,
            /* [in] */ DWORD cookieFirst,
            /* [in] */ DWORD cookieSecond,
            /* [in] */ DWORD cookieTarget,
            /* [out] */ __RPC__out BOOL *firstIsBetter);
        
        END_INTERFACE
    } IEEAssemblyRefResolveComparerVtbl;

    interface IEEAssemblyRefResolveComparer
    {
        CONST_VTBL struct IEEAssemblyRefResolveComparerVtbl *lpVtbl;
    };

    

#ifdef COBJMACROS


#define IEEAssemblyRefResolveComparer_QueryInterface(This,riid,ppvObject)	\
    ( (This)->lpVtbl -> QueryInterface(This,riid,ppvObject) ) 

#define IEEAssemblyRefResolveComparer_AddRef(This)	\
    ( (This)->lpVtbl -> AddRef(This) ) 

#define IEEAssemblyRefResolveComparer_Release(This)	\
    ( (This)->lpVtbl -> Release(This) ) 


#define IEEAssemblyRefResolveComparer_CompareRef(This,cookieFirst,cookieSecond,cookieTarget,firstIsBetter)	\
    ( (This)->lpVtbl -> CompareRef(This,cookieFirst,cookieSecond,cookieTarget,firstIsBetter) ) 

#endif /* COBJMACROS */


#endif 	/* C style interface */




#endif 	/* __IEEAssemblyRefResolveComparer_INTERFACE_DEFINED__ */


#ifndef __IEEAssemblyRef_INTERFACE_DEFINED__
#define __IEEAssemblyRef_INTERFACE_DEFINED__

/* interface IEEAssemblyRef */
/* [unique][uuid][object] */ 


EXTERN_C const IID IID_IEEAssemblyRef;

#if defined(__cplusplus) && !defined(CINTERFACE)
    
    MIDL_INTERFACE("AAD20A0E-9CD9-40ab-91B9-3C1943562C84")
    IEEAssemblyRef : public IUnknown
    {
    public:
        virtual HRESULT STDMETHODCALLTYPE GetName( 
            /* [out] */ __RPC__deref_out_opt BSTR *bstr) = 0;
        
        virtual HRESULT STDMETHODCALLTYPE GetVersion( 
            /* [out] */ __RPC__out USHORT *major,
            /* [out] */ __RPC__out USHORT *minor,
            /* [out] */ __RPC__out USHORT *build,
            /* [out] */ __RPC__out USHORT *revision) = 0;
        
        virtual HRESULT STDMETHODCALLTYPE GetCulture( 
            /* [out] */ __RPC__deref_out_opt BSTR *bstr) = 0;
        
        virtual HRESULT STDMETHODCALLTYPE GetPublicKey( 
            /* [out] */ __RPC__deref_out_opt BSTR *key) = 0;
        
    };
    
#else 	/* C style interface */

    typedef struct IEEAssemblyRefVtbl
    {
        BEGIN_INTERFACE
        
        HRESULT ( STDMETHODCALLTYPE *QueryInterface )( 
            IEEAssemblyRef * This,
            /* [in] */ __RPC__in REFIID riid,
            /* [iid_is][out] */ 
            __RPC__deref_out  void **ppvObject);
        
        ULONG ( STDMETHODCALLTYPE *AddRef )( 
            IEEAssemblyRef * This);
        
        ULONG ( STDMETHODCALLTYPE *Release )( 
            IEEAssemblyRef * This);
        
        HRESULT ( STDMETHODCALLTYPE *GetName )( 
            IEEAssemblyRef * This,
            /* [out] */ __RPC__deref_out_opt BSTR *bstr);
        
        HRESULT ( STDMETHODCALLTYPE *GetVersion )( 
            IEEAssemblyRef * This,
            /* [out] */ __RPC__out USHORT *major,
            /* [out] */ __RPC__out USHORT *minor,
            /* [out] */ __RPC__out USHORT *build,
            /* [out] */ __RPC__out USHORT *revision);
        
        HRESULT ( STDMETHODCALLTYPE *GetCulture )( 
            IEEAssemblyRef * This,
            /* [out] */ __RPC__deref_out_opt BSTR *bstr);
        
        HRESULT ( STDMETHODCALLTYPE *GetPublicKey )( 
            IEEAssemblyRef * This,
            /* [out] */ __RPC__deref_out_opt BSTR *key);
        
        END_INTERFACE
    } IEEAssemblyRefVtbl;

    interface IEEAssemblyRef
    {
        CONST_VTBL struct IEEAssemblyRefVtbl *lpVtbl;
    };

    

#ifdef COBJMACROS


#define IEEAssemblyRef_QueryInterface(This,riid,ppvObject)	\
    ( (This)->lpVtbl -> QueryInterface(This,riid,ppvObject) ) 

#define IEEAssemblyRef_AddRef(This)	\
    ( (This)->lpVtbl -> AddRef(This) ) 

#define IEEAssemblyRef_Release(This)	\
    ( (This)->lpVtbl -> Release(This) ) 


#define IEEAssemblyRef_GetName(This,bstr)	\
    ( (This)->lpVtbl -> GetName(This,bstr) ) 

#define IEEAssemblyRef_GetVersion(This,major,minor,build,revision)	\
    ( (This)->lpVtbl -> GetVersion(This,major,minor,build,revision) ) 

#define IEEAssemblyRef_GetCulture(This,bstr)	\
    ( (This)->lpVtbl -> GetCulture(This,bstr) ) 

#define IEEAssemblyRef_GetPublicKey(This,key)	\
    ( (This)->lpVtbl -> GetPublicKey(This,key) ) 

#endif /* COBJMACROS */


#endif 	/* C style interface */




#endif 	/* __IEEAssemblyRef_INTERFACE_DEFINED__ */


#ifndef __IEEHelperObject_INTERFACE_DEFINED__
#define __IEEHelperObject_INTERFACE_DEFINED__

/* interface IEEHelperObject */
/* [unique][uuid][object] */ 


enum enum_ASSEMBLYFLAGS
    {	ASMF_USERDIR	= 0x1,
	ASMF_SHAREDDIR	= 0x2
    } ;
typedef DWORD ASSEMBLYFLAGS;


EXTERN_C const IID IID_IEEHelperObject;

#if defined(__cplusplus) && !defined(CINTERFACE)
    
    MIDL_INTERFACE("4A3BCDE5-5F66-4cc8-9FA0-14275CCEE688")
    IEEHelperObject : public IUnknown
    {
    public:
        virtual HRESULT STDMETHODCALLTYPE InitCache( 
            __RPC__in_opt IEEAssemblyRefResolveComparer *pResolver) = 0;
        
        virtual HRESULT STDMETHODCALLTYPE GetTargetClass( 
            /* [in] */ __RPC__in LPCOLESTR name,
            /* [in] */ DWORD assemblyCookie,
            /* [out] */ __RPC__out DWORD *cookie,
            /* [out] */ __RPC__out ULONG *valueAttrCount,
            /* [out] */ __RPC__out ULONG *viewerAttrCount,
            /* [out] */ __RPC__out ULONG *visualizerAttrCount) = 0;
        
        virtual HRESULT STDMETHODCALLTYPE GetTargetAssembly( 
            /* [in] */ __RPC__in LPCOLESTR name,
            /* [out] */ __RPC__out DWORD *cookie) = 0;
        
        virtual HRESULT STDMETHODCALLTYPE GetAssembly( 
            /* [in] */ DWORD assemblyCookie,
            GETASSEMBLY flags,
            /* [out] */ __RPC__out ASSEMBLYFLAGS *flagsOut,
            /* [out] */ __RPC__deref_out_opt BSTR *name,
            /* [out] */ __RPC__deref_out_opt IEEDataStorage **assemBytes,
            /* [out] */ __RPC__deref_out_opt IEEDataStorage **pdbBytes) = 0;
        
        virtual HRESULT STDMETHODCALLTYPE GetHostAssembly( 
            GETASSEMBLY flags,
            /* [out] */ __RPC__deref_out_opt IEEDataStorage **assemBytes,
            /* [out] */ __RPC__deref_out_opt IEEDataStorage **pdbBytes) = 0;
        
        virtual HRESULT STDMETHODCALLTYPE GetValueAttributeProps( 
            /* [in] */ DWORD classCookie,
            /* [in] */ ULONG ordinal,
            /* [out] */ __RPC__deref_out_opt BSTR *targetedAssembly,
            /* [out] */ __RPC__out DWORD *assemLocation,
            /* [out] */ __RPC__deref_out_opt BSTR *name,
            /* [out] */ __RPC__deref_out_opt BSTR *value,
            /* [out] */ __RPC__deref_out_opt BSTR *type) = 0;
        
        virtual HRESULT STDMETHODCALLTYPE GetViewerAttributeProps( 
            /* [in] */ DWORD classCookie,
            /* [in] */ ULONG ordinal,
            /* [out] */ __RPC__deref_out_opt BSTR *targetedAssembly,
            /* [out] */ __RPC__out DWORD *assemLocation,
            /* [out] */ __RPC__deref_out_opt BSTR *className,
            /* [out] */ __RPC__out DWORD *classAssemLocation) = 0;
        
        virtual HRESULT STDMETHODCALLTYPE GetVisualizerAttributeProps( 
            /* [in] */ DWORD classCookie,
            /* [in] */ ULONG ordinal,
            /* [out] */ __RPC__deref_out_opt BSTR *targetedAssembly,
            /* [out] */ __RPC__out DWORD *assemLocation,
            /* [out] */ __RPC__deref_out_opt BSTR *displayClassName,
            /* [out] */ __RPC__out DWORD *displayClassAssemLocation,
            /* [out] */ __RPC__deref_out_opt BSTR *proxyClassName,
            /* [out] */ __RPC__out DWORD *proxyClassAssemLocation,
            /* [out] */ __RPC__deref_out_opt BSTR *description,
            /* [out] */ __RPC__out ULONG *uiType) = 0;
        
        virtual HRESULT STDMETHODCALLTYPE GetAssemblyRefForCookie( 
            /* [in] */ DWORD cookie,
            /* [out] */ __RPC__deref_out_opt IEEAssemblyRef **ppAssemRef) = 0;
        
    };
    
#else 	/* C style interface */

    typedef struct IEEHelperObjectVtbl
    {
        BEGIN_INTERFACE
        
        HRESULT ( STDMETHODCALLTYPE *QueryInterface )( 
            IEEHelperObject * This,
            /* [in] */ __RPC__in REFIID riid,
            /* [iid_is][out] */ 
            __RPC__deref_out  void **ppvObject);
        
        ULONG ( STDMETHODCALLTYPE *AddRef )( 
            IEEHelperObject * This);
        
        ULONG ( STDMETHODCALLTYPE *Release )( 
            IEEHelperObject * This);
        
        HRESULT ( STDMETHODCALLTYPE *InitCache )( 
            IEEHelperObject * This,
            __RPC__in_opt IEEAssemblyRefResolveComparer *pResolver);
        
        HRESULT ( STDMETHODCALLTYPE *GetTargetClass )( 
            IEEHelperObject * This,
            /* [in] */ __RPC__in LPCOLESTR name,
            /* [in] */ DWORD assemblyCookie,
            /* [out] */ __RPC__out DWORD *cookie,
            /* [out] */ __RPC__out ULONG *valueAttrCount,
            /* [out] */ __RPC__out ULONG *viewerAttrCount,
            /* [out] */ __RPC__out ULONG *visualizerAttrCount);
        
        HRESULT ( STDMETHODCALLTYPE *GetTargetAssembly )( 
            IEEHelperObject * This,
            /* [in] */ __RPC__in LPCOLESTR name,
            /* [out] */ __RPC__out DWORD *cookie);
        
        HRESULT ( STDMETHODCALLTYPE *GetAssembly )( 
            IEEHelperObject * This,
            /* [in] */ DWORD assemblyCookie,
            GETASSEMBLY flags,
            /* [out] */ __RPC__out ASSEMBLYFLAGS *flagsOut,
            /* [out] */ __RPC__deref_out_opt BSTR *name,
            /* [out] */ __RPC__deref_out_opt IEEDataStorage **assemBytes,
            /* [out] */ __RPC__deref_out_opt IEEDataStorage **pdbBytes);
        
        HRESULT ( STDMETHODCALLTYPE *GetHostAssembly )( 
            IEEHelperObject * This,
            GETASSEMBLY flags,
            /* [out] */ __RPC__deref_out_opt IEEDataStorage **assemBytes,
            /* [out] */ __RPC__deref_out_opt IEEDataStorage **pdbBytes);
        
        HRESULT ( STDMETHODCALLTYPE *GetValueAttributeProps )( 
            IEEHelperObject * This,
            /* [in] */ DWORD classCookie,
            /* [in] */ ULONG ordinal,
            /* [out] */ __RPC__deref_out_opt BSTR *targetedAssembly,
            /* [out] */ __RPC__out DWORD *assemLocation,
            /* [out] */ __RPC__deref_out_opt BSTR *name,
            /* [out] */ __RPC__deref_out_opt BSTR *value,
            /* [out] */ __RPC__deref_out_opt BSTR *type);
        
        HRESULT ( STDMETHODCALLTYPE *GetViewerAttributeProps )( 
            IEEHelperObject * This,
            /* [in] */ DWORD classCookie,
            /* [in] */ ULONG ordinal,
            /* [out] */ __RPC__deref_out_opt BSTR *targetedAssembly,
            /* [out] */ __RPC__out DWORD *assemLocation,
            /* [out] */ __RPC__deref_out_opt BSTR *className,
            /* [out] */ __RPC__out DWORD *classAssemLocation);
        
        HRESULT ( STDMETHODCALLTYPE *GetVisualizerAttributeProps )( 
            IEEHelperObject * This,
            /* [in] */ DWORD classCookie,
            /* [in] */ ULONG ordinal,
            /* [out] */ __RPC__deref_out_opt BSTR *targetedAssembly,
            /* [out] */ __RPC__out DWORD *assemLocation,
            /* [out] */ __RPC__deref_out_opt BSTR *displayClassName,
            /* [out] */ __RPC__out DWORD *displayClassAssemLocation,
            /* [out] */ __RPC__deref_out_opt BSTR *proxyClassName,
            /* [out] */ __RPC__out DWORD *proxyClassAssemLocation,
            /* [out] */ __RPC__deref_out_opt BSTR *description,
            /* [out] */ __RPC__out ULONG *uiType);
        
        HRESULT ( STDMETHODCALLTYPE *GetAssemblyRefForCookie )( 
            IEEHelperObject * This,
            /* [in] */ DWORD cookie,
            /* [out] */ __RPC__deref_out_opt IEEAssemblyRef **ppAssemRef);
        
        END_INTERFACE
    } IEEHelperObjectVtbl;

    interface IEEHelperObject
    {
        CONST_VTBL struct IEEHelperObjectVtbl *lpVtbl;
    };

    

#ifdef COBJMACROS


#define IEEHelperObject_QueryInterface(This,riid,ppvObject)	\
    ( (This)->lpVtbl -> QueryInterface(This,riid,ppvObject) ) 

#define IEEHelperObject_AddRef(This)	\
    ( (This)->lpVtbl -> AddRef(This) ) 

#define IEEHelperObject_Release(This)	\
    ( (This)->lpVtbl -> Release(This) ) 


#define IEEHelperObject_InitCache(This,pResolver)	\
    ( (This)->lpVtbl -> InitCache(This,pResolver) ) 

#define IEEHelperObject_GetTargetClass(This,name,assemblyCookie,cookie,valueAttrCount,viewerAttrCount,visualizerAttrCount)	\
    ( (This)->lpVtbl -> GetTargetClass(This,name,assemblyCookie,cookie,valueAttrCount,viewerAttrCount,visualizerAttrCount) ) 

#define IEEHelperObject_GetTargetAssembly(This,name,cookie)	\
    ( (This)->lpVtbl -> GetTargetAssembly(This,name,cookie) ) 

#define IEEHelperObject_GetAssembly(This,assemblyCookie,flags,flagsOut,name,assemBytes,pdbBytes)	\
    ( (This)->lpVtbl -> GetAssembly(This,assemblyCookie,flags,flagsOut,name,assemBytes,pdbBytes) ) 

#define IEEHelperObject_GetHostAssembly(This,flags,assemBytes,pdbBytes)	\
    ( (This)->lpVtbl -> GetHostAssembly(This,flags,assemBytes,pdbBytes) ) 

#define IEEHelperObject_GetValueAttributeProps(This,classCookie,ordinal,targetedAssembly,assemLocation,name,value,type)	\
    ( (This)->lpVtbl -> GetValueAttributeProps(This,classCookie,ordinal,targetedAssembly,assemLocation,name,value,type) ) 

#define IEEHelperObject_GetViewerAttributeProps(This,classCookie,ordinal,targetedAssembly,assemLocation,className,classAssemLocation)	\
    ( (This)->lpVtbl -> GetViewerAttributeProps(This,classCookie,ordinal,targetedAssembly,assemLocation,className,classAssemLocation) ) 

#define IEEHelperObject_GetVisualizerAttributeProps(This,classCookie,ordinal,targetedAssembly,assemLocation,displayClassName,displayClassAssemLocation,proxyClassName,proxyClassAssemLocation,description,uiType)	\
    ( (This)->lpVtbl -> GetVisualizerAttributeProps(This,classCookie,ordinal,targetedAssembly,assemLocation,displayClassName,displayClassAssemLocation,proxyClassName,proxyClassAssemLocation,description,uiType) ) 

#define IEEHelperObject_GetAssemblyRefForCookie(This,cookie,ppAssemRef)	\
    ( (This)->lpVtbl -> GetAssemblyRefForCookie(This,cookie,ppAssemRef) ) 

#endif /* COBJMACROS */


#endif 	/* C style interface */




#endif 	/* __IEEHelperObject_INTERFACE_DEFINED__ */


#ifndef __IDebugExceptionCallback2_INTERFACE_DEFINED__
#define __IDebugExceptionCallback2_INTERFACE_DEFINED__

/* interface IDebugExceptionCallback2 */
/* [unique][uuid][object] */ 


EXTERN_C const IID IID_IDebugExceptionCallback2;

#if defined(__cplusplus) && !defined(CINTERFACE)
    
    MIDL_INTERFACE("6f5cfda4-47d3-4a90-a882-14427237bcee")
    IDebugExceptionCallback2 : public IUnknown
    {
    public:
        virtual HRESULT STDMETHODCALLTYPE QueryStopOnException( 
            /* [in] */ __RPC__in_opt IDebugProcess2 *pProcess,
            /* [in] */ __RPC__in_opt IDebugProgram2 *pProgram,
            /* [in] */ __RPC__in_opt IDebugThread2 *pThread,
            /* [in] */ __RPC__in_opt IDebugExceptionEvent2 *pEvent) = 0;
        
    };
    
#else 	/* C style interface */

    typedef struct IDebugExceptionCallback2Vtbl
    {
        BEGIN_INTERFACE
        
        HRESULT ( STDMETHODCALLTYPE *QueryInterface )( 
            IDebugExceptionCallback2 * This,
            /* [in] */ __RPC__in REFIID riid,
            /* [iid_is][out] */ 
            __RPC__deref_out  void **ppvObject);
        
        ULONG ( STDMETHODCALLTYPE *AddRef )( 
            IDebugExceptionCallback2 * This);
        
        ULONG ( STDMETHODCALLTYPE *Release )( 
            IDebugExceptionCallback2 * This);
        
        HRESULT ( STDMETHODCALLTYPE *QueryStopOnException )( 
            IDebugExceptionCallback2 * This,
            /* [in] */ __RPC__in_opt IDebugProcess2 *pProcess,
            /* [in] */ __RPC__in_opt IDebugProgram2 *pProgram,
            /* [in] */ __RPC__in_opt IDebugThread2 *pThread,
            /* [in] */ __RPC__in_opt IDebugExceptionEvent2 *pEvent);
        
        END_INTERFACE
    } IDebugExceptionCallback2Vtbl;

    interface IDebugExceptionCallback2
    {
        CONST_VTBL struct IDebugExceptionCallback2Vtbl *lpVtbl;
    };

    

#ifdef COBJMACROS


#define IDebugExceptionCallback2_QueryInterface(This,riid,ppvObject)	\
    ( (This)->lpVtbl -> QueryInterface(This,riid,ppvObject) ) 

#define IDebugExceptionCallback2_AddRef(This)	\
    ( (This)->lpVtbl -> AddRef(This) ) 

#define IDebugExceptionCallback2_Release(This)	\
    ( (This)->lpVtbl -> Release(This) ) 


#define IDebugExceptionCallback2_QueryStopOnException(This,pProcess,pProgram,pThread,pEvent)	\
    ( (This)->lpVtbl -> QueryStopOnException(This,pProcess,pProgram,pThread,pEvent) ) 

#endif /* COBJMACROS */


#endif 	/* C style interface */




#endif 	/* __IDebugExceptionCallback2_INTERFACE_DEFINED__ */


#ifndef __IEnumDebugProcesses2_INTERFACE_DEFINED__
#define __IEnumDebugProcesses2_INTERFACE_DEFINED__

/* interface IEnumDebugProcesses2 */
/* [unique][uuid][object] */ 


EXTERN_C const IID IID_IEnumDebugProcesses2;

#if defined(__cplusplus) && !defined(CINTERFACE)
    
    MIDL_INTERFACE("96c74ef4-185d-4f9a-8a43-4d2723758e0a")
    IEnumDebugProcesses2 : public IUnknown
    {
    public:
        virtual HRESULT STDMETHODCALLTYPE Next( 
            /* [in] */ ULONG celt,
            /* [length_is][size_is][out] */ __RPC__out_ecount_part(celt, *pceltFetched) IDebugProcess2 **rgelt,
            /* [out][in] */ __RPC__inout ULONG *pceltFetched) = 0;
        
        virtual HRESULT STDMETHODCALLTYPE Skip( 
            /* [in] */ ULONG celt) = 0;
        
        virtual HRESULT STDMETHODCALLTYPE Reset( void) = 0;
        
        virtual HRESULT STDMETHODCALLTYPE Clone( 
            /* [out] */ __RPC__deref_out_opt IEnumDebugProcesses2 **ppEnum) = 0;
        
        virtual HRESULT STDMETHODCALLTYPE GetCount( 
            /* [out] */ __RPC__out ULONG *pcelt) = 0;
        
    };
    
#else 	/* C style interface */

    typedef struct IEnumDebugProcesses2Vtbl
    {
        BEGIN_INTERFACE
        
        HRESULT ( STDMETHODCALLTYPE *QueryInterface )( 
            IEnumDebugProcesses2 * This,
            /* [in] */ __RPC__in REFIID riid,
            /* [iid_is][out] */ 
            __RPC__deref_out  void **ppvObject);
        
        ULONG ( STDMETHODCALLTYPE *AddRef )( 
            IEnumDebugProcesses2 * This);
        
        ULONG ( STDMETHODCALLTYPE *Release )( 
            IEnumDebugProcesses2 * This);
        
        HRESULT ( STDMETHODCALLTYPE *Next )( 
            IEnumDebugProcesses2 * This,
            /* [in] */ ULONG celt,
            /* [length_is][size_is][out] */ __RPC__out_ecount_part(celt, *pceltFetched) IDebugProcess2 **rgelt,
            /* [out][in] */ __RPC__inout ULONG *pceltFetched);
        
        HRESULT ( STDMETHODCALLTYPE *Skip )( 
            IEnumDebugProcesses2 * This,
            /* [in] */ ULONG celt);
        
        HRESULT ( STDMETHODCALLTYPE *Reset )( 
            IEnumDebugProcesses2 * This);
        
        HRESULT ( STDMETHODCALLTYPE *Clone )( 
            IEnumDebugProcesses2 * This,
            /* [out] */ __RPC__deref_out_opt IEnumDebugProcesses2 **ppEnum);
        
        HRESULT ( STDMETHODCALLTYPE *GetCount )( 
            IEnumDebugProcesses2 * This,
            /* [out] */ __RPC__out ULONG *pcelt);
        
        END_INTERFACE
    } IEnumDebugProcesses2Vtbl;

    interface IEnumDebugProcesses2
    {
        CONST_VTBL struct IEnumDebugProcesses2Vtbl *lpVtbl;
    };

    

#ifdef COBJMACROS


#define IEnumDebugProcesses2_QueryInterface(This,riid,ppvObject)	\
    ( (This)->lpVtbl -> QueryInterface(This,riid,ppvObject) ) 

#define IEnumDebugProcesses2_AddRef(This)	\
    ( (This)->lpVtbl -> AddRef(This) ) 

#define IEnumDebugProcesses2_Release(This)	\
    ( (This)->lpVtbl -> Release(This) ) 


#define IEnumDebugProcesses2_Next(This,celt,rgelt,pceltFetched)	\
    ( (This)->lpVtbl -> Next(This,celt,rgelt,pceltFetched) ) 

#define IEnumDebugProcesses2_Skip(This,celt)	\
    ( (This)->lpVtbl -> Skip(This,celt) ) 

#define IEnumDebugProcesses2_Reset(This)	\
    ( (This)->lpVtbl -> Reset(This) ) 

#define IEnumDebugProcesses2_Clone(This,ppEnum)	\
    ( (This)->lpVtbl -> Clone(This,ppEnum) ) 

#define IEnumDebugProcesses2_GetCount(This,pcelt)	\
    ( (This)->lpVtbl -> GetCount(This,pcelt) ) 

#endif /* COBJMACROS */


#endif 	/* C style interface */




#endif 	/* __IEnumDebugProcesses2_INTERFACE_DEFINED__ */


#ifndef __IEnumDebugPrograms2_INTERFACE_DEFINED__
#define __IEnumDebugPrograms2_INTERFACE_DEFINED__

/* interface IEnumDebugPrograms2 */
/* [unique][uuid][object] */ 


EXTERN_C const IID IID_IEnumDebugPrograms2;

#if defined(__cplusplus) && !defined(CINTERFACE)
    
    MIDL_INTERFACE("8d14bca6-34ce-4efe-ac7e-0abc61dadb20")
    IEnumDebugPrograms2 : public IUnknown
    {
    public:
        virtual HRESULT STDMETHODCALLTYPE Next( 
            /* [in] */ ULONG celt,
            /* [length_is][size_is][out] */ __RPC__out_ecount_part(celt, *pceltFetched) IDebugProgram2 **rgelt,
            /* [out][in] */ __RPC__inout ULONG *pceltFetched) = 0;
        
        virtual HRESULT STDMETHODCALLTYPE Skip( 
            /* [in] */ ULONG celt) = 0;
        
        virtual HRESULT STDMETHODCALLTYPE Reset( void) = 0;
        
        virtual HRESULT STDMETHODCALLTYPE Clone( 
            /* [out] */ __RPC__deref_out_opt IEnumDebugPrograms2 **ppEnum) = 0;
        
        virtual HRESULT STDMETHODCALLTYPE GetCount( 
            /* [out] */ __RPC__out ULONG *pcelt) = 0;
        
    };
    
#else 	/* C style interface */

    typedef struct IEnumDebugPrograms2Vtbl
    {
        BEGIN_INTERFACE
        
        HRESULT ( STDMETHODCALLTYPE *QueryInterface )( 
            IEnumDebugPrograms2 * This,
            /* [in] */ __RPC__in REFIID riid,
            /* [iid_is][out] */ 
            __RPC__deref_out  void **ppvObject);
        
        ULONG ( STDMETHODCALLTYPE *AddRef )( 
            IEnumDebugPrograms2 * This);
        
        ULONG ( STDMETHODCALLTYPE *Release )( 
            IEnumDebugPrograms2 * This);
        
        HRESULT ( STDMETHODCALLTYPE *Next )( 
            IEnumDebugPrograms2 * This,
            /* [in] */ ULONG celt,
            /* [length_is][size_is][out] */ __RPC__out_ecount_part(celt, *pceltFetched) IDebugProgram2 **rgelt,
            /* [out][in] */ __RPC__inout ULONG *pceltFetched);
        
        HRESULT ( STDMETHODCALLTYPE *Skip )( 
            IEnumDebugPrograms2 * This,
            /* [in] */ ULONG celt);
        
        HRESULT ( STDMETHODCALLTYPE *Reset )( 
            IEnumDebugPrograms2 * This);
        
        HRESULT ( STDMETHODCALLTYPE *Clone )( 
            IEnumDebugPrograms2 * This,
            /* [out] */ __RPC__deref_out_opt IEnumDebugPrograms2 **ppEnum);
        
        HRESULT ( STDMETHODCALLTYPE *GetCount )( 
            IEnumDebugPrograms2 * This,
            /* [out] */ __RPC__out ULONG *pcelt);
        
        END_INTERFACE
    } IEnumDebugPrograms2Vtbl;

    interface IEnumDebugPrograms2
    {
        CONST_VTBL struct IEnumDebugPrograms2Vtbl *lpVtbl;
    };

    

#ifdef COBJMACROS


#define IEnumDebugPrograms2_QueryInterface(This,riid,ppvObject)	\
    ( (This)->lpVtbl -> QueryInterface(This,riid,ppvObject) ) 

#define IEnumDebugPrograms2_AddRef(This)	\
    ( (This)->lpVtbl -> AddRef(This) ) 

#define IEnumDebugPrograms2_Release(This)	\
    ( (This)->lpVtbl -> Release(This) ) 


#define IEnumDebugPrograms2_Next(This,celt,rgelt,pceltFetched)	\
    ( (This)->lpVtbl -> Next(This,celt,rgelt,pceltFetched) ) 

#define IEnumDebugPrograms2_Skip(This,celt)	\
    ( (This)->lpVtbl -> Skip(This,celt) ) 

#define IEnumDebugPrograms2_Reset(This)	\
    ( (This)->lpVtbl -> Reset(This) ) 

#define IEnumDebugPrograms2_Clone(This,ppEnum)	\
    ( (This)->lpVtbl -> Clone(This,ppEnum) ) 

#define IEnumDebugPrograms2_GetCount(This,pcelt)	\
    ( (This)->lpVtbl -> GetCount(This,pcelt) ) 

#endif /* COBJMACROS */


#endif 	/* C style interface */




#endif 	/* __IEnumDebugPrograms2_INTERFACE_DEFINED__ */


#ifndef __IEnumDebugThreads2_INTERFACE_DEFINED__
#define __IEnumDebugThreads2_INTERFACE_DEFINED__

/* interface IEnumDebugThreads2 */
/* [unique][uuid][object] */ 


EXTERN_C const IID IID_IEnumDebugThreads2;

#if defined(__cplusplus) && !defined(CINTERFACE)
    
    MIDL_INTERFACE("0d30dc12-c4f8-433d-9fcc-9ff117e5e5f4")
    IEnumDebugThreads2 : public IUnknown
    {
    public:
        virtual HRESULT STDMETHODCALLTYPE Next( 
            /* [in] */ ULONG celt,
            /* [length_is][size_is][out] */ __RPC__out_ecount_part(celt, *pceltFetched) IDebugThread2 **rgelt,
            /* [out][in] */ __RPC__inout ULONG *pceltFetched) = 0;
        
        virtual HRESULT STDMETHODCALLTYPE Skip( 
            /* [in] */ ULONG celt) = 0;
        
        virtual HRESULT STDMETHODCALLTYPE Reset( void) = 0;
        
        virtual HRESULT STDMETHODCALLTYPE Clone( 
            /* [out] */ __RPC__deref_out_opt IEnumDebugThreads2 **ppEnum) = 0;
        
        virtual HRESULT STDMETHODCALLTYPE GetCount( 
            /* [out] */ __RPC__out ULONG *pcelt) = 0;
        
    };
    
#else 	/* C style interface */

    typedef struct IEnumDebugThreads2Vtbl
    {
        BEGIN_INTERFACE
        
        HRESULT ( STDMETHODCALLTYPE *QueryInterface )( 
            IEnumDebugThreads2 * This,
            /* [in] */ __RPC__in REFIID riid,
            /* [iid_is][out] */ 
            __RPC__deref_out  void **ppvObject);
        
        ULONG ( STDMETHODCALLTYPE *AddRef )( 
            IEnumDebugThreads2 * This);
        
        ULONG ( STDMETHODCALLTYPE *Release )( 
            IEnumDebugThreads2 * This);
        
        HRESULT ( STDMETHODCALLTYPE *Next )( 
            IEnumDebugThreads2 * This,
            /* [in] */ ULONG celt,
            /* [length_is][size_is][out] */ __RPC__out_ecount_part(celt, *pceltFetched) IDebugThread2 **rgelt,
            /* [out][in] */ __RPC__inout ULONG *pceltFetched);
        
        HRESULT ( STDMETHODCALLTYPE *Skip )( 
            IEnumDebugThreads2 * This,
            /* [in] */ ULONG celt);
        
        HRESULT ( STDMETHODCALLTYPE *Reset )( 
            IEnumDebugThreads2 * This);
        
        HRESULT ( STDMETHODCALLTYPE *Clone )( 
            IEnumDebugThreads2 * This,
            /* [out] */ __RPC__deref_out_opt IEnumDebugThreads2 **ppEnum);
        
        HRESULT ( STDMETHODCALLTYPE *GetCount )( 
            IEnumDebugThreads2 * This,
            /* [out] */ __RPC__out ULONG *pcelt);
        
        END_INTERFACE
    } IEnumDebugThreads2Vtbl;

    interface IEnumDebugThreads2
    {
        CONST_VTBL struct IEnumDebugThreads2Vtbl *lpVtbl;
    };

    

#ifdef COBJMACROS


#define IEnumDebugThreads2_QueryInterface(This,riid,ppvObject)	\
    ( (This)->lpVtbl -> QueryInterface(This,riid,ppvObject) ) 

#define IEnumDebugThreads2_AddRef(This)	\
    ( (This)->lpVtbl -> AddRef(This) ) 

#define IEnumDebugThreads2_Release(This)	\
    ( (This)->lpVtbl -> Release(This) ) 


#define IEnumDebugThreads2_Next(This,celt,rgelt,pceltFetched)	\
    ( (This)->lpVtbl -> Next(This,celt,rgelt,pceltFetched) ) 

#define IEnumDebugThreads2_Skip(This,celt)	\
    ( (This)->lpVtbl -> Skip(This,celt) ) 

#define IEnumDebugThreads2_Reset(This)	\
    ( (This)->lpVtbl -> Reset(This) ) 

#define IEnumDebugThreads2_Clone(This,ppEnum)	\
    ( (This)->lpVtbl -> Clone(This,ppEnum) ) 

#define IEnumDebugThreads2_GetCount(This,pcelt)	\
    ( (This)->lpVtbl -> GetCount(This,pcelt) ) 

#endif /* COBJMACROS */


#endif 	/* C style interface */




#endif 	/* __IEnumDebugThreads2_INTERFACE_DEFINED__ */


#ifndef __IEnumDebugStackFrames2_INTERFACE_DEFINED__
#define __IEnumDebugStackFrames2_INTERFACE_DEFINED__

/* interface IEnumDebugStackFrames2 */
/* [unique][uuid][object] */ 


EXTERN_C const IID IID_IEnumDebugStackFrames2;

#if defined(__cplusplus) && !defined(CINTERFACE)
    
    MIDL_INTERFACE("cd39102b-4b69-4495-8f29-e0b25c4a8855")
    IEnumDebugStackFrames2 : public IUnknown
    {
    public:
        virtual HRESULT STDMETHODCALLTYPE Next( 
            /* [in] */ ULONG celt,
            /* [length_is][size_is][out] */ __RPC__out_ecount_part(celt, *pceltFetched) IDebugStackFrame2 **rgelt,
            /* [out][in] */ __RPC__inout ULONG *pceltFetched) = 0;
        
        virtual HRESULT STDMETHODCALLTYPE Skip( 
            /* [in] */ ULONG celt) = 0;
        
        virtual HRESULT STDMETHODCALLTYPE Reset( void) = 0;
        
        virtual HRESULT STDMETHODCALLTYPE Clone( 
            /* [out] */ __RPC__deref_out_opt IEnumDebugStackFrames2 **ppEnum) = 0;
        
        virtual HRESULT STDMETHODCALLTYPE GetCount( 
            /* [out] */ __RPC__out ULONG *pcelt) = 0;
        
        virtual HRESULT STDMETHODCALLTYPE GetIndex( 
            /* [in] */ __RPC__in_opt IDebugStackFrame2 *pStackFrame,
            /* [out][in] */ __RPC__inout ULONG *pIndex) = 0;
        
    };
    
#else 	/* C style interface */

    typedef struct IEnumDebugStackFrames2Vtbl
    {
        BEGIN_INTERFACE
        
        HRESULT ( STDMETHODCALLTYPE *QueryInterface )( 
            IEnumDebugStackFrames2 * This,
            /* [in] */ __RPC__in REFIID riid,
            /* [iid_is][out] */ 
            __RPC__deref_out  void **ppvObject);
        
        ULONG ( STDMETHODCALLTYPE *AddRef )( 
            IEnumDebugStackFrames2 * This);
        
        ULONG ( STDMETHODCALLTYPE *Release )( 
            IEnumDebugStackFrames2 * This);
        
        HRESULT ( STDMETHODCALLTYPE *Next )( 
            IEnumDebugStackFrames2 * This,
            /* [in] */ ULONG celt,
            /* [length_is][size_is][out] */ __RPC__out_ecount_part(celt, *pceltFetched) IDebugStackFrame2 **rgelt,
            /* [out][in] */ __RPC__inout ULONG *pceltFetched);
        
        HRESULT ( STDMETHODCALLTYPE *Skip )( 
            IEnumDebugStackFrames2 * This,
            /* [in] */ ULONG celt);
        
        HRESULT ( STDMETHODCALLTYPE *Reset )( 
            IEnumDebugStackFrames2 * This);
        
        HRESULT ( STDMETHODCALLTYPE *Clone )( 
            IEnumDebugStackFrames2 * This,
            /* [out] */ __RPC__deref_out_opt IEnumDebugStackFrames2 **ppEnum);
        
        HRESULT ( STDMETHODCALLTYPE *GetCount )( 
            IEnumDebugStackFrames2 * This,
            /* [out] */ __RPC__out ULONG *pcelt);
        
        HRESULT ( STDMETHODCALLTYPE *GetIndex )( 
            IEnumDebugStackFrames2 * This,
            /* [in] */ __RPC__in_opt IDebugStackFrame2 *pStackFrame,
            /* [out][in] */ __RPC__inout ULONG *pIndex);
        
        END_INTERFACE
    } IEnumDebugStackFrames2Vtbl;

    interface IEnumDebugStackFrames2
    {
        CONST_VTBL struct IEnumDebugStackFrames2Vtbl *lpVtbl;
    };

    

#ifdef COBJMACROS


#define IEnumDebugStackFrames2_QueryInterface(This,riid,ppvObject)	\
    ( (This)->lpVtbl -> QueryInterface(This,riid,ppvObject) ) 

#define IEnumDebugStackFrames2_AddRef(This)	\
    ( (This)->lpVtbl -> AddRef(This) ) 

#define IEnumDebugStackFrames2_Release(This)	\
    ( (This)->lpVtbl -> Release(This) ) 


#define IEnumDebugStackFrames2_Next(This,celt,rgelt,pceltFetched)	\
    ( (This)->lpVtbl -> Next(This,celt,rgelt,pceltFetched) ) 

#define IEnumDebugStackFrames2_Skip(This,celt)	\
    ( (This)->lpVtbl -> Skip(This,celt) ) 

#define IEnumDebugStackFrames2_Reset(This)	\
    ( (This)->lpVtbl -> Reset(This) ) 

#define IEnumDebugStackFrames2_Clone(This,ppEnum)	\
    ( (This)->lpVtbl -> Clone(This,ppEnum) ) 

#define IEnumDebugStackFrames2_GetCount(This,pcelt)	\
    ( (This)->lpVtbl -> GetCount(This,pcelt) ) 

#define IEnumDebugStackFrames2_GetIndex(This,pStackFrame,pIndex)	\
    ( (This)->lpVtbl -> GetIndex(This,pStackFrame,pIndex) ) 

#endif /* COBJMACROS */


#endif 	/* C style interface */




#endif 	/* __IEnumDebugStackFrames2_INTERFACE_DEFINED__ */


#ifndef __IEnumDebugCodeContexts2_INTERFACE_DEFINED__
#define __IEnumDebugCodeContexts2_INTERFACE_DEFINED__

/* interface IEnumDebugCodeContexts2 */
/* [unique][uuid][object] */ 


EXTERN_C const IID IID_IEnumDebugCodeContexts2;

#if defined(__cplusplus) && !defined(CINTERFACE)
    
    MIDL_INTERFACE("ad47a80b-eda7-459e-af82-647cc9fbaa50")
    IEnumDebugCodeContexts2 : public IUnknown
    {
    public:
        virtual HRESULT STDMETHODCALLTYPE Next( 
            /* [in] */ ULONG celt,
            /* [length_is][size_is][out] */ __RPC__out_ecount_part(celt, *pceltFetched) IDebugCodeContext2 **rgelt,
            /* [out][in] */ __RPC__inout ULONG *pceltFetched) = 0;
        
        virtual HRESULT STDMETHODCALLTYPE Skip( 
            /* [in] */ ULONG celt) = 0;
        
        virtual HRESULT STDMETHODCALLTYPE Reset( void) = 0;
        
        virtual HRESULT STDMETHODCALLTYPE Clone( 
            /* [out] */ __RPC__deref_out_opt IEnumDebugCodeContexts2 **ppEnum) = 0;
        
        virtual HRESULT STDMETHODCALLTYPE GetCount( 
            /* [out] */ __RPC__out ULONG *pcelt) = 0;
        
    };
    
#else 	/* C style interface */

    typedef struct IEnumDebugCodeContexts2Vtbl
    {
        BEGIN_INTERFACE
        
        HRESULT ( STDMETHODCALLTYPE *QueryInterface )( 
            IEnumDebugCodeContexts2 * This,
            /* [in] */ __RPC__in REFIID riid,
            /* [iid_is][out] */ 
            __RPC__deref_out  void **ppvObject);
        
        ULONG ( STDMETHODCALLTYPE *AddRef )( 
            IEnumDebugCodeContexts2 * This);
        
        ULONG ( STDMETHODCALLTYPE *Release )( 
            IEnumDebugCodeContexts2 * This);
        
        HRESULT ( STDMETHODCALLTYPE *Next )( 
            IEnumDebugCodeContexts2 * This,
            /* [in] */ ULONG celt,
            /* [length_is][size_is][out] */ __RPC__out_ecount_part(celt, *pceltFetched) IDebugCodeContext2 **rgelt,
            /* [out][in] */ __RPC__inout ULONG *pceltFetched);
        
        HRESULT ( STDMETHODCALLTYPE *Skip )( 
            IEnumDebugCodeContexts2 * This,
            /* [in] */ ULONG celt);
        
        HRESULT ( STDMETHODCALLTYPE *Reset )( 
            IEnumDebugCodeContexts2 * This);
        
        HRESULT ( STDMETHODCALLTYPE *Clone )( 
            IEnumDebugCodeContexts2 * This,
            /* [out] */ __RPC__deref_out_opt IEnumDebugCodeContexts2 **ppEnum);
        
        HRESULT ( STDMETHODCALLTYPE *GetCount )( 
            IEnumDebugCodeContexts2 * This,
            /* [out] */ __RPC__out ULONG *pcelt);
        
        END_INTERFACE
    } IEnumDebugCodeContexts2Vtbl;

    interface IEnumDebugCodeContexts2
    {
        CONST_VTBL struct IEnumDebugCodeContexts2Vtbl *lpVtbl;
    };

    

#ifdef COBJMACROS


#define IEnumDebugCodeContexts2_QueryInterface(This,riid,ppvObject)	\
    ( (This)->lpVtbl -> QueryInterface(This,riid,ppvObject) ) 

#define IEnumDebugCodeContexts2_AddRef(This)	\
    ( (This)->lpVtbl -> AddRef(This) ) 

#define IEnumDebugCodeContexts2_Release(This)	\
    ( (This)->lpVtbl -> Release(This) ) 


#define IEnumDebugCodeContexts2_Next(This,celt,rgelt,pceltFetched)	\
    ( (This)->lpVtbl -> Next(This,celt,rgelt,pceltFetched) ) 

#define IEnumDebugCodeContexts2_Skip(This,celt)	\
    ( (This)->lpVtbl -> Skip(This,celt) ) 

#define IEnumDebugCodeContexts2_Reset(This)	\
    ( (This)->lpVtbl -> Reset(This) ) 

#define IEnumDebugCodeContexts2_Clone(This,ppEnum)	\
    ( (This)->lpVtbl -> Clone(This,ppEnum) ) 

#define IEnumDebugCodeContexts2_GetCount(This,pcelt)	\
    ( (This)->lpVtbl -> GetCount(This,pcelt) ) 

#endif /* COBJMACROS */


#endif 	/* C style interface */




#endif 	/* __IEnumDebugCodeContexts2_INTERFACE_DEFINED__ */


#ifndef __IEnumDebugBoundBreakpoints2_INTERFACE_DEFINED__
#define __IEnumDebugBoundBreakpoints2_INTERFACE_DEFINED__

/* interface IEnumDebugBoundBreakpoints2 */
/* [unique][uuid][object] */ 


EXTERN_C const IID IID_IEnumDebugBoundBreakpoints2;

#if defined(__cplusplus) && !defined(CINTERFACE)
    
    MIDL_INTERFACE("0f6b37e0-fcfe-44d9-9112-394ca9b92114")
    IEnumDebugBoundBreakpoints2 : public IUnknown
    {
    public:
        virtual HRESULT STDMETHODCALLTYPE Next( 
            /* [in] */ ULONG celt,
            /* [length_is][size_is][out] */ __RPC__out_ecount_part(celt, *pceltFetched) IDebugBoundBreakpoint2 **rgelt,
            /* [out][in] */ __RPC__inout ULONG *pceltFetched) = 0;
        
        virtual HRESULT STDMETHODCALLTYPE Skip( 
            /* [in] */ ULONG celt) = 0;
        
        virtual HRESULT STDMETHODCALLTYPE Reset( void) = 0;
        
        virtual HRESULT STDMETHODCALLTYPE Clone( 
            /* [out] */ __RPC__deref_out_opt IEnumDebugBoundBreakpoints2 **ppEnum) = 0;
        
        virtual HRESULT STDMETHODCALLTYPE GetCount( 
            /* [out] */ __RPC__out ULONG *pcelt) = 0;
        
    };
    
#else 	/* C style interface */

    typedef struct IEnumDebugBoundBreakpoints2Vtbl
    {
        BEGIN_INTERFACE
        
        HRESULT ( STDMETHODCALLTYPE *QueryInterface )( 
            IEnumDebugBoundBreakpoints2 * This,
            /* [in] */ __RPC__in REFIID riid,
            /* [iid_is][out] */ 
            __RPC__deref_out  void **ppvObject);
        
        ULONG ( STDMETHODCALLTYPE *AddRef )( 
            IEnumDebugBoundBreakpoints2 * This);
        
        ULONG ( STDMETHODCALLTYPE *Release )( 
            IEnumDebugBoundBreakpoints2 * This);
        
        HRESULT ( STDMETHODCALLTYPE *Next )( 
            IEnumDebugBoundBreakpoints2 * This,
            /* [in] */ ULONG celt,
            /* [length_is][size_is][out] */ __RPC__out_ecount_part(celt, *pceltFetched) IDebugBoundBreakpoint2 **rgelt,
            /* [out][in] */ __RPC__inout ULONG *pceltFetched);
        
        HRESULT ( STDMETHODCALLTYPE *Skip )( 
            IEnumDebugBoundBreakpoints2 * This,
            /* [in] */ ULONG celt);
        
        HRESULT ( STDMETHODCALLTYPE *Reset )( 
            IEnumDebugBoundBreakpoints2 * This);
        
        HRESULT ( STDMETHODCALLTYPE *Clone )( 
            IEnumDebugBoundBreakpoints2 * This,
            /* [out] */ __RPC__deref_out_opt IEnumDebugBoundBreakpoints2 **ppEnum);
        
        HRESULT ( STDMETHODCALLTYPE *GetCount )( 
            IEnumDebugBoundBreakpoints2 * This,
            /* [out] */ __RPC__out ULONG *pcelt);
        
        END_INTERFACE
    } IEnumDebugBoundBreakpoints2Vtbl;

    interface IEnumDebugBoundBreakpoints2
    {
        CONST_VTBL struct IEnumDebugBoundBreakpoints2Vtbl *lpVtbl;
    };

    

#ifdef COBJMACROS


#define IEnumDebugBoundBreakpoints2_QueryInterface(This,riid,ppvObject)	\
    ( (This)->lpVtbl -> QueryInterface(This,riid,ppvObject) ) 

#define IEnumDebugBoundBreakpoints2_AddRef(This)	\
    ( (This)->lpVtbl -> AddRef(This) ) 

#define IEnumDebugBoundBreakpoints2_Release(This)	\
    ( (This)->lpVtbl -> Release(This) ) 


#define IEnumDebugBoundBreakpoints2_Next(This,celt,rgelt,pceltFetched)	\
    ( (This)->lpVtbl -> Next(This,celt,rgelt,pceltFetched) ) 

#define IEnumDebugBoundBreakpoints2_Skip(This,celt)	\
    ( (This)->lpVtbl -> Skip(This,celt) ) 

#define IEnumDebugBoundBreakpoints2_Reset(This)	\
    ( (This)->lpVtbl -> Reset(This) ) 

#define IEnumDebugBoundBreakpoints2_Clone(This,ppEnum)	\
    ( (This)->lpVtbl -> Clone(This,ppEnum) ) 

#define IEnumDebugBoundBreakpoints2_GetCount(This,pcelt)	\
    ( (This)->lpVtbl -> GetCount(This,pcelt) ) 

#endif /* COBJMACROS */


#endif 	/* C style interface */




#endif 	/* __IEnumDebugBoundBreakpoints2_INTERFACE_DEFINED__ */


#ifndef __IEnumDebugPendingBreakpoints2_INTERFACE_DEFINED__
#define __IEnumDebugPendingBreakpoints2_INTERFACE_DEFINED__

/* interface IEnumDebugPendingBreakpoints2 */
/* [unique][uuid][object] */ 


EXTERN_C const IID IID_IEnumDebugPendingBreakpoints2;

#if defined(__cplusplus) && !defined(CINTERFACE)
    
    MIDL_INTERFACE("70d2dc1e-4dcc-4786-a072-9a3b600c216b")
    IEnumDebugPendingBreakpoints2 : public IUnknown
    {
    public:
        virtual HRESULT STDMETHODCALLTYPE Next( 
            /* [in] */ ULONG celt,
            /* [length_is][size_is][out] */ __RPC__out_ecount_part(celt, *pceltFetched) IDebugPendingBreakpoint2 **rgelt,
            /* [out][in] */ __RPC__inout ULONG *pceltFetched) = 0;
        
        virtual HRESULT STDMETHODCALLTYPE Skip( 
            /* [in] */ ULONG celt) = 0;
        
        virtual HRESULT STDMETHODCALLTYPE Reset( void) = 0;
        
        virtual HRESULT STDMETHODCALLTYPE Clone( 
            /* [out] */ __RPC__deref_out_opt IEnumDebugPendingBreakpoints2 **ppEnum) = 0;
        
        virtual HRESULT STDMETHODCALLTYPE GetCount( 
            /* [out] */ __RPC__out ULONG *pcelt) = 0;
        
    };
    
#else 	/* C style interface */

    typedef struct IEnumDebugPendingBreakpoints2Vtbl
    {
        BEGIN_INTERFACE
        
        HRESULT ( STDMETHODCALLTYPE *QueryInterface )( 
            IEnumDebugPendingBreakpoints2 * This,
            /* [in] */ __RPC__in REFIID riid,
            /* [iid_is][out] */ 
            __RPC__deref_out  void **ppvObject);
        
        ULONG ( STDMETHODCALLTYPE *AddRef )( 
            IEnumDebugPendingBreakpoints2 * This);
        
        ULONG ( STDMETHODCALLTYPE *Release )( 
            IEnumDebugPendingBreakpoints2 * This);
        
        HRESULT ( STDMETHODCALLTYPE *Next )( 
            IEnumDebugPendingBreakpoints2 * This,
            /* [in] */ ULONG celt,
            /* [length_is][size_is][out] */ __RPC__out_ecount_part(celt, *pceltFetched) IDebugPendingBreakpoint2 **rgelt,
            /* [out][in] */ __RPC__inout ULONG *pceltFetched);
        
        HRESULT ( STDMETHODCALLTYPE *Skip )( 
            IEnumDebugPendingBreakpoints2 * This,
            /* [in] */ ULONG celt);
        
        HRESULT ( STDMETHODCALLTYPE *Reset )( 
            IEnumDebugPendingBreakpoints2 * This);
        
        HRESULT ( STDMETHODCALLTYPE *Clone )( 
            IEnumDebugPendingBreakpoints2 * This,
            /* [out] */ __RPC__deref_out_opt IEnumDebugPendingBreakpoints2 **ppEnum);
        
        HRESULT ( STDMETHODCALLTYPE *GetCount )( 
            IEnumDebugPendingBreakpoints2 * This,
            /* [out] */ __RPC__out ULONG *pcelt);
        
        END_INTERFACE
    } IEnumDebugPendingBreakpoints2Vtbl;

    interface IEnumDebugPendingBreakpoints2
    {
        CONST_VTBL struct IEnumDebugPendingBreakpoints2Vtbl *lpVtbl;
    };

    

#ifdef COBJMACROS


#define IEnumDebugPendingBreakpoints2_QueryInterface(This,riid,ppvObject)	\
    ( (This)->lpVtbl -> QueryInterface(This,riid,ppvObject) ) 

#define IEnumDebugPendingBreakpoints2_AddRef(This)	\
    ( (This)->lpVtbl -> AddRef(This) ) 

#define IEnumDebugPendingBreakpoints2_Release(This)	\
    ( (This)->lpVtbl -> Release(This) ) 


#define IEnumDebugPendingBreakpoints2_Next(This,celt,rgelt,pceltFetched)	\
    ( (This)->lpVtbl -> Next(This,celt,rgelt,pceltFetched) ) 

#define IEnumDebugPendingBreakpoints2_Skip(This,celt)	\
    ( (This)->lpVtbl -> Skip(This,celt) ) 

#define IEnumDebugPendingBreakpoints2_Reset(This)	\
    ( (This)->lpVtbl -> Reset(This) ) 

#define IEnumDebugPendingBreakpoints2_Clone(This,ppEnum)	\
    ( (This)->lpVtbl -> Clone(This,ppEnum) ) 

#define IEnumDebugPendingBreakpoints2_GetCount(This,pcelt)	\
    ( (This)->lpVtbl -> GetCount(This,pcelt) ) 

#endif /* COBJMACROS */


#endif 	/* C style interface */




#endif 	/* __IEnumDebugPendingBreakpoints2_INTERFACE_DEFINED__ */


#ifndef __IEnumDebugErrorBreakpoints2_INTERFACE_DEFINED__
#define __IEnumDebugErrorBreakpoints2_INTERFACE_DEFINED__

/* interface IEnumDebugErrorBreakpoints2 */
/* [unique][uuid][object] */ 


EXTERN_C const IID IID_IEnumDebugErrorBreakpoints2;

#if defined(__cplusplus) && !defined(CINTERFACE)
    
    MIDL_INTERFACE("e158f5aa-31fe-491b-a9f6-cff934b03a01")
    IEnumDebugErrorBreakpoints2 : public IUnknown
    {
    public:
        virtual HRESULT STDMETHODCALLTYPE Next( 
            /* [in] */ ULONG celt,
            /* [length_is][size_is][out] */ __RPC__out_ecount_part(celt, *pceltFetched) IDebugErrorBreakpoint2 **rgelt,
            /* [out][in] */ __RPC__inout ULONG *pceltFetched) = 0;
        
        virtual HRESULT STDMETHODCALLTYPE Skip( 
            /* [in] */ ULONG celt) = 0;
        
        virtual HRESULT STDMETHODCALLTYPE Reset( void) = 0;
        
        virtual HRESULT STDMETHODCALLTYPE Clone( 
            /* [out] */ __RPC__deref_out_opt IEnumDebugErrorBreakpoints2 **ppEnum) = 0;
        
        virtual HRESULT STDMETHODCALLTYPE GetCount( 
            /* [out] */ __RPC__out ULONG *pcelt) = 0;
        
    };
    
#else 	/* C style interface */

    typedef struct IEnumDebugErrorBreakpoints2Vtbl
    {
        BEGIN_INTERFACE
        
        HRESULT ( STDMETHODCALLTYPE *QueryInterface )( 
            IEnumDebugErrorBreakpoints2 * This,
            /* [in] */ __RPC__in REFIID riid,
            /* [iid_is][out] */ 
            __RPC__deref_out  void **ppvObject);
        
        ULONG ( STDMETHODCALLTYPE *AddRef )( 
            IEnumDebugErrorBreakpoints2 * This);
        
        ULONG ( STDMETHODCALLTYPE *Release )( 
            IEnumDebugErrorBreakpoints2 * This);
        
        HRESULT ( STDMETHODCALLTYPE *Next )( 
            IEnumDebugErrorBreakpoints2 * This,
            /* [in] */ ULONG celt,
            /* [length_is][size_is][out] */ __RPC__out_ecount_part(celt, *pceltFetched) IDebugErrorBreakpoint2 **rgelt,
            /* [out][in] */ __RPC__inout ULONG *pceltFetched);
        
        HRESULT ( STDMETHODCALLTYPE *Skip )( 
            IEnumDebugErrorBreakpoints2 * This,
            /* [in] */ ULONG celt);
        
        HRESULT ( STDMETHODCALLTYPE *Reset )( 
            IEnumDebugErrorBreakpoints2 * This);
        
        HRESULT ( STDMETHODCALLTYPE *Clone )( 
            IEnumDebugErrorBreakpoints2 * This,
            /* [out] */ __RPC__deref_out_opt IEnumDebugErrorBreakpoints2 **ppEnum);
        
        HRESULT ( STDMETHODCALLTYPE *GetCount )( 
            IEnumDebugErrorBreakpoints2 * This,
            /* [out] */ __RPC__out ULONG *pcelt);
        
        END_INTERFACE
    } IEnumDebugErrorBreakpoints2Vtbl;

    interface IEnumDebugErrorBreakpoints2
    {
        CONST_VTBL struct IEnumDebugErrorBreakpoints2Vtbl *lpVtbl;
    };

    

#ifdef COBJMACROS


#define IEnumDebugErrorBreakpoints2_QueryInterface(This,riid,ppvObject)	\
    ( (This)->lpVtbl -> QueryInterface(This,riid,ppvObject) ) 

#define IEnumDebugErrorBreakpoints2_AddRef(This)	\
    ( (This)->lpVtbl -> AddRef(This) ) 

#define IEnumDebugErrorBreakpoints2_Release(This)	\
    ( (This)->lpVtbl -> Release(This) ) 


#define IEnumDebugErrorBreakpoints2_Next(This,celt,rgelt,pceltFetched)	\
    ( (This)->lpVtbl -> Next(This,celt,rgelt,pceltFetched) ) 

#define IEnumDebugErrorBreakpoints2_Skip(This,celt)	\
    ( (This)->lpVtbl -> Skip(This,celt) ) 

#define IEnumDebugErrorBreakpoints2_Reset(This)	\
    ( (This)->lpVtbl -> Reset(This) ) 

#define IEnumDebugErrorBreakpoints2_Clone(This,ppEnum)	\
    ( (This)->lpVtbl -> Clone(This,ppEnum) ) 

#define IEnumDebugErrorBreakpoints2_GetCount(This,pcelt)	\
    ( (This)->lpVtbl -> GetCount(This,pcelt) ) 

#endif /* COBJMACROS */


#endif 	/* C style interface */




#endif 	/* __IEnumDebugErrorBreakpoints2_INTERFACE_DEFINED__ */


#ifndef __IEnumDebugMachines2__deprecated_INTERFACE_DEFINED__
#define __IEnumDebugMachines2__deprecated_INTERFACE_DEFINED__

/* interface IEnumDebugMachines2__deprecated */
/* [unique][uuid][object] */ 


EXTERN_C const IID IID_IEnumDebugMachines2__deprecated;

#if defined(__cplusplus) && !defined(CINTERFACE)
    
    MIDL_INTERFACE("61d986ec-1eac-46b6-90ff-402a008f15d1")
    IEnumDebugMachines2__deprecated : public IUnknown
    {
    public:
        virtual HRESULT STDMETHODCALLTYPE Next( 
            /* [in] */ ULONG celt,
            /* [length_is][size_is][out] */ __RPC__out_ecount_part(celt, *pceltFetched) IDebugCoreServer2 **rgelt,
            /* [out][in] */ __RPC__inout ULONG *pceltFetched) = 0;
        
        virtual HRESULT STDMETHODCALLTYPE Skip( 
            /* [in] */ ULONG celt) = 0;
        
        virtual HRESULT STDMETHODCALLTYPE Reset( void) = 0;
        
        virtual HRESULT STDMETHODCALLTYPE Clone( 
            /* [out] */ __RPC__deref_out_opt IEnumDebugMachines2__deprecated **ppEnum) = 0;
        
        virtual HRESULT STDMETHODCALLTYPE GetCount( 
            /* [out] */ __RPC__out ULONG *pcelt) = 0;
        
    };
    
#else 	/* C style interface */

    typedef struct IEnumDebugMachines2__deprecatedVtbl
    {
        BEGIN_INTERFACE
        
        HRESULT ( STDMETHODCALLTYPE *QueryInterface )( 
            IEnumDebugMachines2__deprecated * This,
            /* [in] */ __RPC__in REFIID riid,
            /* [iid_is][out] */ 
            __RPC__deref_out  void **ppvObject);
        
        ULONG ( STDMETHODCALLTYPE *AddRef )( 
            IEnumDebugMachines2__deprecated * This);
        
        ULONG ( STDMETHODCALLTYPE *Release )( 
            IEnumDebugMachines2__deprecated * This);
        
        HRESULT ( STDMETHODCALLTYPE *Next )( 
            IEnumDebugMachines2__deprecated * This,
            /* [in] */ ULONG celt,
            /* [length_is][size_is][out] */ __RPC__out_ecount_part(celt, *pceltFetched) IDebugCoreServer2 **rgelt,
            /* [out][in] */ __RPC__inout ULONG *pceltFetched);
        
        HRESULT ( STDMETHODCALLTYPE *Skip )( 
            IEnumDebugMachines2__deprecated * This,
            /* [in] */ ULONG celt);
        
        HRESULT ( STDMETHODCALLTYPE *Reset )( 
            IEnumDebugMachines2__deprecated * This);
        
        HRESULT ( STDMETHODCALLTYPE *Clone )( 
            IEnumDebugMachines2__deprecated * This,
            /* [out] */ __RPC__deref_out_opt IEnumDebugMachines2__deprecated **ppEnum);
        
        HRESULT ( STDMETHODCALLTYPE *GetCount )( 
            IEnumDebugMachines2__deprecated * This,
            /* [out] */ __RPC__out ULONG *pcelt);
        
        END_INTERFACE
    } IEnumDebugMachines2__deprecatedVtbl;

    interface IEnumDebugMachines2__deprecated
    {
        CONST_VTBL struct IEnumDebugMachines2__deprecatedVtbl *lpVtbl;
    };

    

#ifdef COBJMACROS


#define IEnumDebugMachines2__deprecated_QueryInterface(This,riid,ppvObject)	\
    ( (This)->lpVtbl -> QueryInterface(This,riid,ppvObject) ) 

#define IEnumDebugMachines2__deprecated_AddRef(This)	\
    ( (This)->lpVtbl -> AddRef(This) ) 

#define IEnumDebugMachines2__deprecated_Release(This)	\
    ( (This)->lpVtbl -> Release(This) ) 


#define IEnumDebugMachines2__deprecated_Next(This,celt,rgelt,pceltFetched)	\
    ( (This)->lpVtbl -> Next(This,celt,rgelt,pceltFetched) ) 

#define IEnumDebugMachines2__deprecated_Skip(This,celt)	\
    ( (This)->lpVtbl -> Skip(This,celt) ) 

#define IEnumDebugMachines2__deprecated_Reset(This)	\
    ( (This)->lpVtbl -> Reset(This) ) 

#define IEnumDebugMachines2__deprecated_Clone(This,ppEnum)	\
    ( (This)->lpVtbl -> Clone(This,ppEnum) ) 

#define IEnumDebugMachines2__deprecated_GetCount(This,pcelt)	\
    ( (This)->lpVtbl -> GetCount(This,pcelt) ) 

#endif /* COBJMACROS */


#endif 	/* C style interface */




#endif 	/* __IEnumDebugMachines2__deprecated_INTERFACE_DEFINED__ */


/* interface __MIDL_itf_msdbg_0000_0140 */
/* [local] */ 

#define EnumMachines_V7 EnumMachines__deprecated
#define IEnumDebugMachines2_V7 IEnumDebugMachines2__deprecated
#define IID_IEnumDebugMachines2_V7 IID_IEnumDebugMachines2__deprecated


extern RPC_IF_HANDLE __MIDL_itf_msdbg_0000_0140_v0_0_c_ifspec;
extern RPC_IF_HANDLE __MIDL_itf_msdbg_0000_0140_v0_0_s_ifspec;

#ifndef __IEnumDebugExceptionInfo2_INTERFACE_DEFINED__
#define __IEnumDebugExceptionInfo2_INTERFACE_DEFINED__

/* interface IEnumDebugExceptionInfo2 */
/* [unique][uuid][object] */ 


EXTERN_C const IID IID_IEnumDebugExceptionInfo2;

#if defined(__cplusplus) && !defined(CINTERFACE)
    
    MIDL_INTERFACE("8e4bbd34-a2f4-41ef-87b5-c563b4ad6ee7")
    IEnumDebugExceptionInfo2 : public IUnknown
    {
    public:
        virtual HRESULT STDMETHODCALLTYPE Next( 
            /* [in] */ ULONG celt,
            /* [length_is][size_is][out] */ __RPC__out_ecount_part(celt, *pceltFetched) EXCEPTION_INFO *rgelt,
            /* [out][in] */ __RPC__inout ULONG *pceltFetched) = 0;
        
        virtual HRESULT STDMETHODCALLTYPE Skip( 
            /* [in] */ ULONG celt) = 0;
        
        virtual HRESULT STDMETHODCALLTYPE Reset( void) = 0;
        
        virtual HRESULT STDMETHODCALLTYPE Clone( 
            /* [out] */ __RPC__deref_out_opt IEnumDebugExceptionInfo2 **ppEnum) = 0;
        
        virtual HRESULT STDMETHODCALLTYPE GetCount( 
            /* [out] */ __RPC__out ULONG *pcelt) = 0;
        
    };
    
#else 	/* C style interface */

    typedef struct IEnumDebugExceptionInfo2Vtbl
    {
        BEGIN_INTERFACE
        
        HRESULT ( STDMETHODCALLTYPE *QueryInterface )( 
            IEnumDebugExceptionInfo2 * This,
            /* [in] */ __RPC__in REFIID riid,
            /* [iid_is][out] */ 
            __RPC__deref_out  void **ppvObject);
        
        ULONG ( STDMETHODCALLTYPE *AddRef )( 
            IEnumDebugExceptionInfo2 * This);
        
        ULONG ( STDMETHODCALLTYPE *Release )( 
            IEnumDebugExceptionInfo2 * This);
        
        HRESULT ( STDMETHODCALLTYPE *Next )( 
            IEnumDebugExceptionInfo2 * This,
            /* [in] */ ULONG celt,
            /* [length_is][size_is][out] */ __RPC__out_ecount_part(celt, *pceltFetched) EXCEPTION_INFO *rgelt,
            /* [out][in] */ __RPC__inout ULONG *pceltFetched);
        
        HRESULT ( STDMETHODCALLTYPE *Skip )( 
            IEnumDebugExceptionInfo2 * This,
            /* [in] */ ULONG celt);
        
        HRESULT ( STDMETHODCALLTYPE *Reset )( 
            IEnumDebugExceptionInfo2 * This);
        
        HRESULT ( STDMETHODCALLTYPE *Clone )( 
            IEnumDebugExceptionInfo2 * This,
            /* [out] */ __RPC__deref_out_opt IEnumDebugExceptionInfo2 **ppEnum);
        
        HRESULT ( STDMETHODCALLTYPE *GetCount )( 
            IEnumDebugExceptionInfo2 * This,
            /* [out] */ __RPC__out ULONG *pcelt);
        
        END_INTERFACE
    } IEnumDebugExceptionInfo2Vtbl;

    interface IEnumDebugExceptionInfo2
    {
        CONST_VTBL struct IEnumDebugExceptionInfo2Vtbl *lpVtbl;
    };

    

#ifdef COBJMACROS


#define IEnumDebugExceptionInfo2_QueryInterface(This,riid,ppvObject)	\
    ( (This)->lpVtbl -> QueryInterface(This,riid,ppvObject) ) 

#define IEnumDebugExceptionInfo2_AddRef(This)	\
    ( (This)->lpVtbl -> AddRef(This) ) 

#define IEnumDebugExceptionInfo2_Release(This)	\
    ( (This)->lpVtbl -> Release(This) ) 


#define IEnumDebugExceptionInfo2_Next(This,celt,rgelt,pceltFetched)	\
    ( (This)->lpVtbl -> Next(This,celt,rgelt,pceltFetched) ) 

#define IEnumDebugExceptionInfo2_Skip(This,celt)	\
    ( (This)->lpVtbl -> Skip(This,celt) ) 

#define IEnumDebugExceptionInfo2_Reset(This)	\
    ( (This)->lpVtbl -> Reset(This) ) 

#define IEnumDebugExceptionInfo2_Clone(This,ppEnum)	\
    ( (This)->lpVtbl -> Clone(This,ppEnum) ) 

#define IEnumDebugExceptionInfo2_GetCount(This,pcelt)	\
    ( (This)->lpVtbl -> GetCount(This,pcelt) ) 

#endif /* COBJMACROS */


#endif 	/* C style interface */




#endif 	/* __IEnumDebugExceptionInfo2_INTERFACE_DEFINED__ */


#ifndef __IEnumDebugFrameInfo2_INTERFACE_DEFINED__
#define __IEnumDebugFrameInfo2_INTERFACE_DEFINED__

/* interface IEnumDebugFrameInfo2 */
/* [unique][uuid][object] */ 


EXTERN_C const IID IID_IEnumDebugFrameInfo2;

#if defined(__cplusplus) && !defined(CINTERFACE)
    
    MIDL_INTERFACE("98bbba48-4c4d-4fff-8340-6097bec9c894")
    IEnumDebugFrameInfo2 : public IUnknown
    {
    public:
        virtual HRESULT STDMETHODCALLTYPE Next( 
            /* [in] */ ULONG celt,
            /* [length_is][size_is][out] */ __RPC__out_ecount_part(celt, *pceltFetched) FRAMEINFO *rgelt,
            /* [out][in] */ __RPC__inout ULONG *pceltFetched) = 0;
        
        virtual HRESULT STDMETHODCALLTYPE Skip( 
            /* [in] */ ULONG celt) = 0;
        
        virtual HRESULT STDMETHODCALLTYPE Reset( void) = 0;
        
        virtual HRESULT STDMETHODCALLTYPE Clone( 
            /* [out] */ __RPC__deref_out_opt IEnumDebugFrameInfo2 **ppEnum) = 0;
        
        virtual HRESULT STDMETHODCALLTYPE GetCount( 
            /* [out] */ __RPC__out ULONG *pcelt) = 0;
        
    };
    
#else 	/* C style interface */

    typedef struct IEnumDebugFrameInfo2Vtbl
    {
        BEGIN_INTERFACE
        
        HRESULT ( STDMETHODCALLTYPE *QueryInterface )( 
            IEnumDebugFrameInfo2 * This,
            /* [in] */ __RPC__in REFIID riid,
            /* [iid_is][out] */ 
            __RPC__deref_out  void **ppvObject);
        
        ULONG ( STDMETHODCALLTYPE *AddRef )( 
            IEnumDebugFrameInfo2 * This);
        
        ULONG ( STDMETHODCALLTYPE *Release )( 
            IEnumDebugFrameInfo2 * This);
        
        HRESULT ( STDMETHODCALLTYPE *Next )( 
            IEnumDebugFrameInfo2 * This,
            /* [in] */ ULONG celt,
            /* [length_is][size_is][out] */ __RPC__out_ecount_part(celt, *pceltFetched) FRAMEINFO *rgelt,
            /* [out][in] */ __RPC__inout ULONG *pceltFetched);
        
        HRESULT ( STDMETHODCALLTYPE *Skip )( 
            IEnumDebugFrameInfo2 * This,
            /* [in] */ ULONG celt);
        
        HRESULT ( STDMETHODCALLTYPE *Reset )( 
            IEnumDebugFrameInfo2 * This);
        
        HRESULT ( STDMETHODCALLTYPE *Clone )( 
            IEnumDebugFrameInfo2 * This,
            /* [out] */ __RPC__deref_out_opt IEnumDebugFrameInfo2 **ppEnum);
        
        HRESULT ( STDMETHODCALLTYPE *GetCount )( 
            IEnumDebugFrameInfo2 * This,
            /* [out] */ __RPC__out ULONG *pcelt);
        
        END_INTERFACE
    } IEnumDebugFrameInfo2Vtbl;

    interface IEnumDebugFrameInfo2
    {
        CONST_VTBL struct IEnumDebugFrameInfo2Vtbl *lpVtbl;
    };

    

#ifdef COBJMACROS


#define IEnumDebugFrameInfo2_QueryInterface(This,riid,ppvObject)	\
    ( (This)->lpVtbl -> QueryInterface(This,riid,ppvObject) ) 

#define IEnumDebugFrameInfo2_AddRef(This)	\
    ( (This)->lpVtbl -> AddRef(This) ) 

#define IEnumDebugFrameInfo2_Release(This)	\
    ( (This)->lpVtbl -> Release(This) ) 


#define IEnumDebugFrameInfo2_Next(This,celt,rgelt,pceltFetched)	\
    ( (This)->lpVtbl -> Next(This,celt,rgelt,pceltFetched) ) 

#define IEnumDebugFrameInfo2_Skip(This,celt)	\
    ( (This)->lpVtbl -> Skip(This,celt) ) 

#define IEnumDebugFrameInfo2_Reset(This)	\
    ( (This)->lpVtbl -> Reset(This) ) 

#define IEnumDebugFrameInfo2_Clone(This,ppEnum)	\
    ( (This)->lpVtbl -> Clone(This,ppEnum) ) 

#define IEnumDebugFrameInfo2_GetCount(This,pcelt)	\
    ( (This)->lpVtbl -> GetCount(This,pcelt) ) 

#endif /* COBJMACROS */


#endif 	/* C style interface */




#endif 	/* __IEnumDebugFrameInfo2_INTERFACE_DEFINED__ */


#ifndef __IEnumDebugSessionFrameInfo2_INTERFACE_DEFINED__
#define __IEnumDebugSessionFrameInfo2_INTERFACE_DEFINED__

/* interface IEnumDebugSessionFrameInfo2 */
/* [unique][uuid][object] */ 

typedef 
enum enum_SESSION_CACHE_PRIORITY
    {	NORMAL_CACHE_PRIORITY	= 0,
	HIGH_CACHE_PRIORITY	= 1
    } 	SESSION_CACHE_PRIORITY;


EXTERN_C const IID IID_IEnumDebugSessionFrameInfo2;

#if defined(__cplusplus) && !defined(CINTERFACE)
    
    MIDL_INTERFACE("ef7262c4-4a01-42a0-8658-932667b27555")
    IEnumDebugSessionFrameInfo2 : public IEnumDebugFrameInfo2
    {
    public:
        virtual HRESULT STDMETHODCALLTYPE SetCachePriority( 
            /* [in] */ SESSION_CACHE_PRIORITY cachePriority) = 0;
        
    };
    
#else 	/* C style interface */

    typedef struct IEnumDebugSessionFrameInfo2Vtbl
    {
        BEGIN_INTERFACE
        
        HRESULT ( STDMETHODCALLTYPE *QueryInterface )( 
            IEnumDebugSessionFrameInfo2 * This,
            /* [in] */ __RPC__in REFIID riid,
            /* [iid_is][out] */ 
            __RPC__deref_out  void **ppvObject);
        
        ULONG ( STDMETHODCALLTYPE *AddRef )( 
            IEnumDebugSessionFrameInfo2 * This);
        
        ULONG ( STDMETHODCALLTYPE *Release )( 
            IEnumDebugSessionFrameInfo2 * This);
        
        HRESULT ( STDMETHODCALLTYPE *Next )( 
            IEnumDebugSessionFrameInfo2 * This,
            /* [in] */ ULONG celt,
            /* [length_is][size_is][out] */ __RPC__out_ecount_part(celt, *pceltFetched) FRAMEINFO *rgelt,
            /* [out][in] */ __RPC__inout ULONG *pceltFetched);
        
        HRESULT ( STDMETHODCALLTYPE *Skip )( 
            IEnumDebugSessionFrameInfo2 * This,
            /* [in] */ ULONG celt);
        
        HRESULT ( STDMETHODCALLTYPE *Reset )( 
            IEnumDebugSessionFrameInfo2 * This);
        
        HRESULT ( STDMETHODCALLTYPE *Clone )( 
            IEnumDebugSessionFrameInfo2 * This,
            /* [out] */ __RPC__deref_out_opt IEnumDebugFrameInfo2 **ppEnum);
        
        HRESULT ( STDMETHODCALLTYPE *GetCount )( 
            IEnumDebugSessionFrameInfo2 * This,
            /* [out] */ __RPC__out ULONG *pcelt);
        
        HRESULT ( STDMETHODCALLTYPE *SetCachePriority )( 
            IEnumDebugSessionFrameInfo2 * This,
            /* [in] */ SESSION_CACHE_PRIORITY cachePriority);
        
        END_INTERFACE
    } IEnumDebugSessionFrameInfo2Vtbl;

    interface IEnumDebugSessionFrameInfo2
    {
        CONST_VTBL struct IEnumDebugSessionFrameInfo2Vtbl *lpVtbl;
    };

    

#ifdef COBJMACROS


#define IEnumDebugSessionFrameInfo2_QueryInterface(This,riid,ppvObject)	\
    ( (This)->lpVtbl -> QueryInterface(This,riid,ppvObject) ) 

#define IEnumDebugSessionFrameInfo2_AddRef(This)	\
    ( (This)->lpVtbl -> AddRef(This) ) 

#define IEnumDebugSessionFrameInfo2_Release(This)	\
    ( (This)->lpVtbl -> Release(This) ) 


#define IEnumDebugSessionFrameInfo2_Next(This,celt,rgelt,pceltFetched)	\
    ( (This)->lpVtbl -> Next(This,celt,rgelt,pceltFetched) ) 

#define IEnumDebugSessionFrameInfo2_Skip(This,celt)	\
    ( (This)->lpVtbl -> Skip(This,celt) ) 

#define IEnumDebugSessionFrameInfo2_Reset(This)	\
    ( (This)->lpVtbl -> Reset(This) ) 

#define IEnumDebugSessionFrameInfo2_Clone(This,ppEnum)	\
    ( (This)->lpVtbl -> Clone(This,ppEnum) ) 

#define IEnumDebugSessionFrameInfo2_GetCount(This,pcelt)	\
    ( (This)->lpVtbl -> GetCount(This,pcelt) ) 


#define IEnumDebugSessionFrameInfo2_SetCachePriority(This,cachePriority)	\
    ( (This)->lpVtbl -> SetCachePriority(This,cachePriority) ) 

#endif /* COBJMACROS */


#endif 	/* C style interface */




#endif 	/* __IEnumDebugSessionFrameInfo2_INTERFACE_DEFINED__ */


#ifndef __IEnumDebugFrameInfoFilter2_INTERFACE_DEFINED__
#define __IEnumDebugFrameInfoFilter2_INTERFACE_DEFINED__

/* interface IEnumDebugFrameInfoFilter2 */
/* [unique][uuid][object] */ 


EXTERN_C const IID IID_IEnumDebugFrameInfoFilter2;

#if defined(__cplusplus) && !defined(CINTERFACE)
    
    MIDL_INTERFACE("6CD4FB40-F954-44e0-B8A5-A614481E0831")
    IEnumDebugFrameInfoFilter2 : public IEnumDebugFrameInfo2
    {
    public:
        virtual HRESULT STDMETHODCALLTYPE CanFilter( 
            /* [out] */ __RPC__out BOOL *pfCanFilter) = 0;
        
        virtual HRESULT STDMETHODCALLTYPE IsFiltered( 
            /* [out] */ __RPC__out BOOL *pfIsFiltered) = 0;
        
    };
    
#else 	/* C style interface */

    typedef struct IEnumDebugFrameInfoFilter2Vtbl
    {
        BEGIN_INTERFACE
        
        HRESULT ( STDMETHODCALLTYPE *QueryInterface )( 
            IEnumDebugFrameInfoFilter2 * This,
            /* [in] */ __RPC__in REFIID riid,
            /* [iid_is][out] */ 
            __RPC__deref_out  void **ppvObject);
        
        ULONG ( STDMETHODCALLTYPE *AddRef )( 
            IEnumDebugFrameInfoFilter2 * This);
        
        ULONG ( STDMETHODCALLTYPE *Release )( 
            IEnumDebugFrameInfoFilter2 * This);
        
        HRESULT ( STDMETHODCALLTYPE *Next )( 
            IEnumDebugFrameInfoFilter2 * This,
            /* [in] */ ULONG celt,
            /* [length_is][size_is][out] */ __RPC__out_ecount_part(celt, *pceltFetched) FRAMEINFO *rgelt,
            /* [out][in] */ __RPC__inout ULONG *pceltFetched);
        
        HRESULT ( STDMETHODCALLTYPE *Skip )( 
            IEnumDebugFrameInfoFilter2 * This,
            /* [in] */ ULONG celt);
        
        HRESULT ( STDMETHODCALLTYPE *Reset )( 
            IEnumDebugFrameInfoFilter2 * This);
        
        HRESULT ( STDMETHODCALLTYPE *Clone )( 
            IEnumDebugFrameInfoFilter2 * This,
            /* [out] */ __RPC__deref_out_opt IEnumDebugFrameInfo2 **ppEnum);
        
        HRESULT ( STDMETHODCALLTYPE *GetCount )( 
            IEnumDebugFrameInfoFilter2 * This,
            /* [out] */ __RPC__out ULONG *pcelt);
        
        HRESULT ( STDMETHODCALLTYPE *CanFilter )( 
            IEnumDebugFrameInfoFilter2 * This,
            /* [out] */ __RPC__out BOOL *pfCanFilter);
        
        HRESULT ( STDMETHODCALLTYPE *IsFiltered )( 
            IEnumDebugFrameInfoFilter2 * This,
            /* [out] */ __RPC__out BOOL *pfIsFiltered);
        
        END_INTERFACE
    } IEnumDebugFrameInfoFilter2Vtbl;

    interface IEnumDebugFrameInfoFilter2
    {
        CONST_VTBL struct IEnumDebugFrameInfoFilter2Vtbl *lpVtbl;
    };

    

#ifdef COBJMACROS


#define IEnumDebugFrameInfoFilter2_QueryInterface(This,riid,ppvObject)	\
    ( (This)->lpVtbl -> QueryInterface(This,riid,ppvObject) ) 

#define IEnumDebugFrameInfoFilter2_AddRef(This)	\
    ( (This)->lpVtbl -> AddRef(This) ) 

#define IEnumDebugFrameInfoFilter2_Release(This)	\
    ( (This)->lpVtbl -> Release(This) ) 


#define IEnumDebugFrameInfoFilter2_Next(This,celt,rgelt,pceltFetched)	\
    ( (This)->lpVtbl -> Next(This,celt,rgelt,pceltFetched) ) 

#define IEnumDebugFrameInfoFilter2_Skip(This,celt)	\
    ( (This)->lpVtbl -> Skip(This,celt) ) 

#define IEnumDebugFrameInfoFilter2_Reset(This)	\
    ( (This)->lpVtbl -> Reset(This) ) 

#define IEnumDebugFrameInfoFilter2_Clone(This,ppEnum)	\
    ( (This)->lpVtbl -> Clone(This,ppEnum) ) 

#define IEnumDebugFrameInfoFilter2_GetCount(This,pcelt)	\
    ( (This)->lpVtbl -> GetCount(This,pcelt) ) 


#define IEnumDebugFrameInfoFilter2_CanFilter(This,pfCanFilter)	\
    ( (This)->lpVtbl -> CanFilter(This,pfCanFilter) ) 

#define IEnumDebugFrameInfoFilter2_IsFiltered(This,pfIsFiltered)	\
    ( (This)->lpVtbl -> IsFiltered(This,pfIsFiltered) ) 

#endif /* COBJMACROS */


#endif 	/* C style interface */




#endif 	/* __IEnumDebugFrameInfoFilter2_INTERFACE_DEFINED__ */


#ifndef __IEnumCodePaths2_INTERFACE_DEFINED__
#define __IEnumCodePaths2_INTERFACE_DEFINED__

/* interface IEnumCodePaths2 */
/* [unique][uuid][object] */ 


EXTERN_C const IID IID_IEnumCodePaths2;

#if defined(__cplusplus) && !defined(CINTERFACE)
    
    MIDL_INTERFACE("9b13f80d-cfc6-4b78-81ef-1f7cc33f7639")
    IEnumCodePaths2 : public IUnknown
    {
    public:
        virtual HRESULT STDMETHODCALLTYPE Next( 
            /* [in] */ ULONG celt,
            /* [length_is][size_is][out] */ __RPC__out_ecount_part(celt, *pceltFetched) CODE_PATH *rgelt,
            /* [out][in] */ __RPC__inout ULONG *pceltFetched) = 0;
        
        virtual HRESULT STDMETHODCALLTYPE Skip( 
            /* [in] */ ULONG celt) = 0;
        
        virtual HRESULT STDMETHODCALLTYPE Reset( void) = 0;
        
        virtual HRESULT STDMETHODCALLTYPE Clone( 
            /* [out] */ __RPC__deref_out_opt IEnumCodePaths2 **ppEnum) = 0;
        
        virtual HRESULT STDMETHODCALLTYPE GetCount( 
            /* [out] */ __RPC__out ULONG *pcelt) = 0;
        
    };
    
#else 	/* C style interface */

    typedef struct IEnumCodePaths2Vtbl
    {
        BEGIN_INTERFACE
        
        HRESULT ( STDMETHODCALLTYPE *QueryInterface )( 
            IEnumCodePaths2 * This,
            /* [in] */ __RPC__in REFIID riid,
            /* [iid_is][out] */ 
            __RPC__deref_out  void **ppvObject);
        
        ULONG ( STDMETHODCALLTYPE *AddRef )( 
            IEnumCodePaths2 * This);
        
        ULONG ( STDMETHODCALLTYPE *Release )( 
            IEnumCodePaths2 * This);
        
        HRESULT ( STDMETHODCALLTYPE *Next )( 
            IEnumCodePaths2 * This,
            /* [in] */ ULONG celt,
            /* [length_is][size_is][out] */ __RPC__out_ecount_part(celt, *pceltFetched) CODE_PATH *rgelt,
            /* [out][in] */ __RPC__inout ULONG *pceltFetched);
        
        HRESULT ( STDMETHODCALLTYPE *Skip )( 
            IEnumCodePaths2 * This,
            /* [in] */ ULONG celt);
        
        HRESULT ( STDMETHODCALLTYPE *Reset )( 
            IEnumCodePaths2 * This);
        
        HRESULT ( STDMETHODCALLTYPE *Clone )( 
            IEnumCodePaths2 * This,
            /* [out] */ __RPC__deref_out_opt IEnumCodePaths2 **ppEnum);
        
        HRESULT ( STDMETHODCALLTYPE *GetCount )( 
            IEnumCodePaths2 * This,
            /* [out] */ __RPC__out ULONG *pcelt);
        
        END_INTERFACE
    } IEnumCodePaths2Vtbl;

    interface IEnumCodePaths2
    {
        CONST_VTBL struct IEnumCodePaths2Vtbl *lpVtbl;
    };

    

#ifdef COBJMACROS


#define IEnumCodePaths2_QueryInterface(This,riid,ppvObject)	\
    ( (This)->lpVtbl -> QueryInterface(This,riid,ppvObject) ) 

#define IEnumCodePaths2_AddRef(This)	\
    ( (This)->lpVtbl -> AddRef(This) ) 

#define IEnumCodePaths2_Release(This)	\
    ( (This)->lpVtbl -> Release(This) ) 


#define IEnumCodePaths2_Next(This,celt,rgelt,pceltFetched)	\
    ( (This)->lpVtbl -> Next(This,celt,rgelt,pceltFetched) ) 

#define IEnumCodePaths2_Skip(This,celt)	\
    ( (This)->lpVtbl -> Skip(This,celt) ) 

#define IEnumCodePaths2_Reset(This)	\
    ( (This)->lpVtbl -> Reset(This) ) 

#define IEnumCodePaths2_Clone(This,ppEnum)	\
    ( (This)->lpVtbl -> Clone(This,ppEnum) ) 

#define IEnumCodePaths2_GetCount(This,pcelt)	\
    ( (This)->lpVtbl -> GetCount(This,pcelt) ) 

#endif /* COBJMACROS */


#endif 	/* C style interface */




#endif 	/* __IEnumCodePaths2_INTERFACE_DEFINED__ */


#ifndef __IEnumDebugModules2_INTERFACE_DEFINED__
#define __IEnumDebugModules2_INTERFACE_DEFINED__

/* interface IEnumDebugModules2 */
/* [unique][uuid][object] */ 


EXTERN_C const IID IID_IEnumDebugModules2;

#if defined(__cplusplus) && !defined(CINTERFACE)
    
    MIDL_INTERFACE("4c4a2835-682e-4ce1-aebc-1e6b3a165b44")
    IEnumDebugModules2 : public IUnknown
    {
    public:
        virtual HRESULT STDMETHODCALLTYPE Next( 
            /* [in] */ ULONG celt,
            /* [length_is][size_is][out] */ __RPC__out_ecount_part(celt, *pceltFetched) IDebugModule2 **rgelt,
            /* [out][in] */ __RPC__inout ULONG *pceltFetched) = 0;
        
        virtual HRESULT STDMETHODCALLTYPE Skip( 
            /* [in] */ ULONG celt) = 0;
        
        virtual HRESULT STDMETHODCALLTYPE Reset( void) = 0;
        
        virtual HRESULT STDMETHODCALLTYPE Clone( 
            /* [out] */ __RPC__deref_out_opt IEnumDebugModules2 **ppEnum) = 0;
        
        virtual HRESULT STDMETHODCALLTYPE GetCount( 
            /* [out] */ __RPC__out ULONG *pcelt) = 0;
        
    };
    
#else 	/* C style interface */

    typedef struct IEnumDebugModules2Vtbl
    {
        BEGIN_INTERFACE
        
        HRESULT ( STDMETHODCALLTYPE *QueryInterface )( 
            IEnumDebugModules2 * This,
            /* [in] */ __RPC__in REFIID riid,
            /* [iid_is][out] */ 
            __RPC__deref_out  void **ppvObject);
        
        ULONG ( STDMETHODCALLTYPE *AddRef )( 
            IEnumDebugModules2 * This);
        
        ULONG ( STDMETHODCALLTYPE *Release )( 
            IEnumDebugModules2 * This);
        
        HRESULT ( STDMETHODCALLTYPE *Next )( 
            IEnumDebugModules2 * This,
            /* [in] */ ULONG celt,
            /* [length_is][size_is][out] */ __RPC__out_ecount_part(celt, *pceltFetched) IDebugModule2 **rgelt,
            /* [out][in] */ __RPC__inout ULONG *pceltFetched);
        
        HRESULT ( STDMETHODCALLTYPE *Skip )( 
            IEnumDebugModules2 * This,
            /* [in] */ ULONG celt);
        
        HRESULT ( STDMETHODCALLTYPE *Reset )( 
            IEnumDebugModules2 * This);
        
        HRESULT ( STDMETHODCALLTYPE *Clone )( 
            IEnumDebugModules2 * This,
            /* [out] */ __RPC__deref_out_opt IEnumDebugModules2 **ppEnum);
        
        HRESULT ( STDMETHODCALLTYPE *GetCount )( 
            IEnumDebugModules2 * This,
            /* [out] */ __RPC__out ULONG *pcelt);
        
        END_INTERFACE
    } IEnumDebugModules2Vtbl;

    interface IEnumDebugModules2
    {
        CONST_VTBL struct IEnumDebugModules2Vtbl *lpVtbl;
    };

    

#ifdef COBJMACROS


#define IEnumDebugModules2_QueryInterface(This,riid,ppvObject)	\
    ( (This)->lpVtbl -> QueryInterface(This,riid,ppvObject) ) 

#define IEnumDebugModules2_AddRef(This)	\
    ( (This)->lpVtbl -> AddRef(This) ) 

#define IEnumDebugModules2_Release(This)	\
    ( (This)->lpVtbl -> Release(This) ) 


#define IEnumDebugModules2_Next(This,celt,rgelt,pceltFetched)	\
    ( (This)->lpVtbl -> Next(This,celt,rgelt,pceltFetched) ) 

#define IEnumDebugModules2_Skip(This,celt)	\
    ( (This)->lpVtbl -> Skip(This,celt) ) 

#define IEnumDebugModules2_Reset(This)	\
    ( (This)->lpVtbl -> Reset(This) ) 

#define IEnumDebugModules2_Clone(This,ppEnum)	\
    ( (This)->lpVtbl -> Clone(This,ppEnum) ) 

#define IEnumDebugModules2_GetCount(This,pcelt)	\
    ( (This)->lpVtbl -> GetCount(This,pcelt) ) 

#endif /* COBJMACROS */


#endif 	/* C style interface */




#endif 	/* __IEnumDebugModules2_INTERFACE_DEFINED__ */


#ifndef __IEnumDebugPortSuppliers2_INTERFACE_DEFINED__
#define __IEnumDebugPortSuppliers2_INTERFACE_DEFINED__

/* interface IEnumDebugPortSuppliers2 */
/* [unique][uuid][object] */ 


EXTERN_C const IID IID_IEnumDebugPortSuppliers2;

#if defined(__cplusplus) && !defined(CINTERFACE)
    
    MIDL_INTERFACE("59c9dc99-3eff-4ff3-b201-98acd01b0d87")
    IEnumDebugPortSuppliers2 : public IUnknown
    {
    public:
        virtual HRESULT STDMETHODCALLTYPE Next( 
            /* [in] */ ULONG celt,
            /* [length_is][size_is][out] */ __RPC__out_ecount_part(celt, *pceltFetched) IDebugPortSupplier2 **rgelt,
            /* [out][in] */ __RPC__inout ULONG *pceltFetched) = 0;
        
        virtual HRESULT STDMETHODCALLTYPE Skip( 
            /* [in] */ ULONG celt) = 0;
        
        virtual HRESULT STDMETHODCALLTYPE Reset( void) = 0;
        
        virtual HRESULT STDMETHODCALLTYPE Clone( 
            /* [out] */ __RPC__deref_out_opt IEnumDebugPortSuppliers2 **ppEnum) = 0;
        
        virtual HRESULT STDMETHODCALLTYPE GetCount( 
            /* [out] */ __RPC__out ULONG *pcelt) = 0;
        
    };
    
#else 	/* C style interface */

    typedef struct IEnumDebugPortSuppliers2Vtbl
    {
        BEGIN_INTERFACE
        
        HRESULT ( STDMETHODCALLTYPE *QueryInterface )( 
            IEnumDebugPortSuppliers2 * This,
            /* [in] */ __RPC__in REFIID riid,
            /* [iid_is][out] */ 
            __RPC__deref_out  void **ppvObject);
        
        ULONG ( STDMETHODCALLTYPE *AddRef )( 
            IEnumDebugPortSuppliers2 * This);
        
        ULONG ( STDMETHODCALLTYPE *Release )( 
            IEnumDebugPortSuppliers2 * This);
        
        HRESULT ( STDMETHODCALLTYPE *Next )( 
            IEnumDebugPortSuppliers2 * This,
            /* [in] */ ULONG celt,
            /* [length_is][size_is][out] */ __RPC__out_ecount_part(celt, *pceltFetched) IDebugPortSupplier2 **rgelt,
            /* [out][in] */ __RPC__inout ULONG *pceltFetched);
        
        HRESULT ( STDMETHODCALLTYPE *Skip )( 
            IEnumDebugPortSuppliers2 * This,
            /* [in] */ ULONG celt);
        
        HRESULT ( STDMETHODCALLTYPE *Reset )( 
            IEnumDebugPortSuppliers2 * This);
        
        HRESULT ( STDMETHODCALLTYPE *Clone )( 
            IEnumDebugPortSuppliers2 * This,
            /* [out] */ __RPC__deref_out_opt IEnumDebugPortSuppliers2 **ppEnum);
        
        HRESULT ( STDMETHODCALLTYPE *GetCount )( 
            IEnumDebugPortSuppliers2 * This,
            /* [out] */ __RPC__out ULONG *pcelt);
        
        END_INTERFACE
    } IEnumDebugPortSuppliers2Vtbl;

    interface IEnumDebugPortSuppliers2
    {
        CONST_VTBL struct IEnumDebugPortSuppliers2Vtbl *lpVtbl;
    };

    

#ifdef COBJMACROS


#define IEnumDebugPortSuppliers2_QueryInterface(This,riid,ppvObject)	\
    ( (This)->lpVtbl -> QueryInterface(This,riid,ppvObject) ) 

#define IEnumDebugPortSuppliers2_AddRef(This)	\
    ( (This)->lpVtbl -> AddRef(This) ) 

#define IEnumDebugPortSuppliers2_Release(This)	\
    ( (This)->lpVtbl -> Release(This) ) 


#define IEnumDebugPortSuppliers2_Next(This,celt,rgelt,pceltFetched)	\
    ( (This)->lpVtbl -> Next(This,celt,rgelt,pceltFetched) ) 

#define IEnumDebugPortSuppliers2_Skip(This,celt)	\
    ( (This)->lpVtbl -> Skip(This,celt) ) 

#define IEnumDebugPortSuppliers2_Reset(This)	\
    ( (This)->lpVtbl -> Reset(This) ) 

#define IEnumDebugPortSuppliers2_Clone(This,ppEnum)	\
    ( (This)->lpVtbl -> Clone(This,ppEnum) ) 

#define IEnumDebugPortSuppliers2_GetCount(This,pcelt)	\
    ( (This)->lpVtbl -> GetCount(This,pcelt) ) 

#endif /* COBJMACROS */


#endif 	/* C style interface */




#endif 	/* __IEnumDebugPortSuppliers2_INTERFACE_DEFINED__ */


#ifndef __IEnumDebugPorts2_INTERFACE_DEFINED__
#define __IEnumDebugPorts2_INTERFACE_DEFINED__

/* interface IEnumDebugPorts2 */
/* [unique][uuid][object] */ 


EXTERN_C const IID IID_IEnumDebugPorts2;

#if defined(__cplusplus) && !defined(CINTERFACE)
    
    MIDL_INTERFACE("bc827c5e-99ae-4ac8-83ad-2ea5c2034333")
    IEnumDebugPorts2 : public IUnknown
    {
    public:
        virtual HRESULT STDMETHODCALLTYPE Next( 
            /* [in] */ ULONG celt,
            /* [length_is][size_is][out] */ __RPC__out_ecount_part(celt, *pceltFetched) IDebugPort2 **rgelt,
            /* [out][in] */ __RPC__inout ULONG *pceltFetched) = 0;
        
        virtual HRESULT STDMETHODCALLTYPE Skip( 
            /* [in] */ ULONG celt) = 0;
        
        virtual HRESULT STDMETHODCALLTYPE Reset( void) = 0;
        
        virtual HRESULT STDMETHODCALLTYPE Clone( 
            /* [out] */ __RPC__deref_out_opt IEnumDebugPorts2 **ppEnum) = 0;
        
        virtual HRESULT STDMETHODCALLTYPE GetCount( 
            /* [out] */ __RPC__out ULONG *pcelt) = 0;
        
    };
    
#else 	/* C style interface */

    typedef struct IEnumDebugPorts2Vtbl
    {
        BEGIN_INTERFACE
        
        HRESULT ( STDMETHODCALLTYPE *QueryInterface )( 
            IEnumDebugPorts2 * This,
            /* [in] */ __RPC__in REFIID riid,
            /* [iid_is][out] */ 
            __RPC__deref_out  void **ppvObject);
        
        ULONG ( STDMETHODCALLTYPE *AddRef )( 
            IEnumDebugPorts2 * This);
        
        ULONG ( STDMETHODCALLTYPE *Release )( 
            IEnumDebugPorts2 * This);
        
        HRESULT ( STDMETHODCALLTYPE *Next )( 
            IEnumDebugPorts2 * This,
            /* [in] */ ULONG celt,
            /* [length_is][size_is][out] */ __RPC__out_ecount_part(celt, *pceltFetched) IDebugPort2 **rgelt,
            /* [out][in] */ __RPC__inout ULONG *pceltFetched);
        
        HRESULT ( STDMETHODCALLTYPE *Skip )( 
            IEnumDebugPorts2 * This,
            /* [in] */ ULONG celt);
        
        HRESULT ( STDMETHODCALLTYPE *Reset )( 
            IEnumDebugPorts2 * This);
        
        HRESULT ( STDMETHODCALLTYPE *Clone )( 
            IEnumDebugPorts2 * This,
            /* [out] */ __RPC__deref_out_opt IEnumDebugPorts2 **ppEnum);
        
        HRESULT ( STDMETHODCALLTYPE *GetCount )( 
            IEnumDebugPorts2 * This,
            /* [out] */ __RPC__out ULONG *pcelt);
        
        END_INTERFACE
    } IEnumDebugPorts2Vtbl;

    interface IEnumDebugPorts2
    {
        CONST_VTBL struct IEnumDebugPorts2Vtbl *lpVtbl;
    };

    

#ifdef COBJMACROS


#define IEnumDebugPorts2_QueryInterface(This,riid,ppvObject)	\
    ( (This)->lpVtbl -> QueryInterface(This,riid,ppvObject) ) 

#define IEnumDebugPorts2_AddRef(This)	\
    ( (This)->lpVtbl -> AddRef(This) ) 

#define IEnumDebugPorts2_Release(This)	\
    ( (This)->lpVtbl -> Release(This) ) 


#define IEnumDebugPorts2_Next(This,celt,rgelt,pceltFetched)	\
    ( (This)->lpVtbl -> Next(This,celt,rgelt,pceltFetched) ) 

#define IEnumDebugPorts2_Skip(This,celt)	\
    ( (This)->lpVtbl -> Skip(This,celt) ) 

#define IEnumDebugPorts2_Reset(This)	\
    ( (This)->lpVtbl -> Reset(This) ) 

#define IEnumDebugPorts2_Clone(This,ppEnum)	\
    ( (This)->lpVtbl -> Clone(This,ppEnum) ) 

#define IEnumDebugPorts2_GetCount(This,pcelt)	\
    ( (This)->lpVtbl -> GetCount(This,pcelt) ) 

#endif /* COBJMACROS */


#endif 	/* C style interface */




#endif 	/* __IEnumDebugPorts2_INTERFACE_DEFINED__ */


#ifndef __IEnumDebugPropertyInfo2_INTERFACE_DEFINED__
#define __IEnumDebugPropertyInfo2_INTERFACE_DEFINED__

/* interface IEnumDebugPropertyInfo2 */
/* [unique][uuid][object] */ 


EXTERN_C const IID IID_IEnumDebugPropertyInfo2;

#if defined(__cplusplus) && !defined(CINTERFACE)
    
    MIDL_INTERFACE("6c7072c3-3ac4-408f-a680-fc5a2f96903e")
    IEnumDebugPropertyInfo2 : public IUnknown
    {
    public:
        virtual HRESULT STDMETHODCALLTYPE Next( 
            /* [in] */ ULONG celt,
            /* [length_is][size_is][out] */ __RPC__out_ecount_part(celt, *pceltFetched) DEBUG_PROPERTY_INFO *rgelt,
            /* [out] */ __RPC__out ULONG *pceltFetched) = 0;
        
        virtual HRESULT STDMETHODCALLTYPE Skip( 
            /* [in] */ ULONG celt) = 0;
        
        virtual HRESULT STDMETHODCALLTYPE Reset( void) = 0;
        
        virtual HRESULT STDMETHODCALLTYPE Clone( 
            /* [out] */ __RPC__deref_out_opt IEnumDebugPropertyInfo2 **ppEnum) = 0;
        
        virtual HRESULT STDMETHODCALLTYPE GetCount( 
            /* [out] */ __RPC__out ULONG *pcelt) = 0;
        
    };
    
#else 	/* C style interface */

    typedef struct IEnumDebugPropertyInfo2Vtbl
    {
        BEGIN_INTERFACE
        
        HRESULT ( STDMETHODCALLTYPE *QueryInterface )( 
            IEnumDebugPropertyInfo2 * This,
            /* [in] */ __RPC__in REFIID riid,
            /* [iid_is][out] */ 
            __RPC__deref_out  void **ppvObject);
        
        ULONG ( STDMETHODCALLTYPE *AddRef )( 
            IEnumDebugPropertyInfo2 * This);
        
        ULONG ( STDMETHODCALLTYPE *Release )( 
            IEnumDebugPropertyInfo2 * This);
        
        HRESULT ( STDMETHODCALLTYPE *Next )( 
            IEnumDebugPropertyInfo2 * This,
            /* [in] */ ULONG celt,
            /* [length_is][size_is][out] */ __RPC__out_ecount_part(celt, *pceltFetched) DEBUG_PROPERTY_INFO *rgelt,
            /* [out] */ __RPC__out ULONG *pceltFetched);
        
        HRESULT ( STDMETHODCALLTYPE *Skip )( 
            IEnumDebugPropertyInfo2 * This,
            /* [in] */ ULONG celt);
        
        HRESULT ( STDMETHODCALLTYPE *Reset )( 
            IEnumDebugPropertyInfo2 * This);
        
        HRESULT ( STDMETHODCALLTYPE *Clone )( 
            IEnumDebugPropertyInfo2 * This,
            /* [out] */ __RPC__deref_out_opt IEnumDebugPropertyInfo2 **ppEnum);
        
        HRESULT ( STDMETHODCALLTYPE *GetCount )( 
            IEnumDebugPropertyInfo2 * This,
            /* [out] */ __RPC__out ULONG *pcelt);
        
        END_INTERFACE
    } IEnumDebugPropertyInfo2Vtbl;

    interface IEnumDebugPropertyInfo2
    {
        CONST_VTBL struct IEnumDebugPropertyInfo2Vtbl *lpVtbl;
    };

    

#ifdef COBJMACROS


#define IEnumDebugPropertyInfo2_QueryInterface(This,riid,ppvObject)	\
    ( (This)->lpVtbl -> QueryInterface(This,riid,ppvObject) ) 

#define IEnumDebugPropertyInfo2_AddRef(This)	\
    ( (This)->lpVtbl -> AddRef(This) ) 

#define IEnumDebugPropertyInfo2_Release(This)	\
    ( (This)->lpVtbl -> Release(This) ) 


#define IEnumDebugPropertyInfo2_Next(This,celt,rgelt,pceltFetched)	\
    ( (This)->lpVtbl -> Next(This,celt,rgelt,pceltFetched) ) 

#define IEnumDebugPropertyInfo2_Skip(This,celt)	\
    ( (This)->lpVtbl -> Skip(This,celt) ) 

#define IEnumDebugPropertyInfo2_Reset(This)	\
    ( (This)->lpVtbl -> Reset(This) ) 

#define IEnumDebugPropertyInfo2_Clone(This,ppEnum)	\
    ( (This)->lpVtbl -> Clone(This,ppEnum) ) 

#define IEnumDebugPropertyInfo2_GetCount(This,pcelt)	\
    ( (This)->lpVtbl -> GetCount(This,pcelt) ) 

#endif /* COBJMACROS */


#endif 	/* C style interface */




#endif 	/* __IEnumDebugPropertyInfo2_INTERFACE_DEFINED__ */


#ifndef __IEnumDebugReferenceInfo2_INTERFACE_DEFINED__
#define __IEnumDebugReferenceInfo2_INTERFACE_DEFINED__

/* interface IEnumDebugReferenceInfo2 */
/* [unique][uuid][object] */ 


EXTERN_C const IID IID_IEnumDebugReferenceInfo2;

#if defined(__cplusplus) && !defined(CINTERFACE)
    
    MIDL_INTERFACE("e459dd12-864f-4aaa-abc1-dcecbc267f04")
    IEnumDebugReferenceInfo2 : public IUnknown
    {
    public:
        virtual HRESULT STDMETHODCALLTYPE Next( 
            /* [in] */ ULONG celt,
            /* [length_is][size_is][out] */ __RPC__out_ecount_part(celt, *pceltFetched) DEBUG_REFERENCE_INFO *rgelt,
            /* [out] */ __RPC__out ULONG *pceltFetched) = 0;
        
        virtual HRESULT STDMETHODCALLTYPE Skip( 
            /* [in] */ ULONG celt) = 0;
        
        virtual HRESULT STDMETHODCALLTYPE Reset( void) = 0;
        
        virtual HRESULT STDMETHODCALLTYPE Clone( 
            /* [out] */ __RPC__deref_out_opt IEnumDebugReferenceInfo2 **ppEnum) = 0;
        
        virtual HRESULT STDMETHODCALLTYPE GetCount( 
            /* [out] */ __RPC__out ULONG *pcelt) = 0;
        
    };
    
#else 	/* C style interface */

    typedef struct IEnumDebugReferenceInfo2Vtbl
    {
        BEGIN_INTERFACE
        
        HRESULT ( STDMETHODCALLTYPE *QueryInterface )( 
            IEnumDebugReferenceInfo2 * This,
            /* [in] */ __RPC__in REFIID riid,
            /* [iid_is][out] */ 
            __RPC__deref_out  void **ppvObject);
        
        ULONG ( STDMETHODCALLTYPE *AddRef )( 
            IEnumDebugReferenceInfo2 * This);
        
        ULONG ( STDMETHODCALLTYPE *Release )( 
            IEnumDebugReferenceInfo2 * This);
        
        HRESULT ( STDMETHODCALLTYPE *Next )( 
            IEnumDebugReferenceInfo2 * This,
            /* [in] */ ULONG celt,
            /* [length_is][size_is][out] */ __RPC__out_ecount_part(celt, *pceltFetched) DEBUG_REFERENCE_INFO *rgelt,
            /* [out] */ __RPC__out ULONG *pceltFetched);
        
        HRESULT ( STDMETHODCALLTYPE *Skip )( 
            IEnumDebugReferenceInfo2 * This,
            /* [in] */ ULONG celt);
        
        HRESULT ( STDMETHODCALLTYPE *Reset )( 
            IEnumDebugReferenceInfo2 * This);
        
        HRESULT ( STDMETHODCALLTYPE *Clone )( 
            IEnumDebugReferenceInfo2 * This,
            /* [out] */ __RPC__deref_out_opt IEnumDebugReferenceInfo2 **ppEnum);
        
        HRESULT ( STDMETHODCALLTYPE *GetCount )( 
            IEnumDebugReferenceInfo2 * This,
            /* [out] */ __RPC__out ULONG *pcelt);
        
        END_INTERFACE
    } IEnumDebugReferenceInfo2Vtbl;

    interface IEnumDebugReferenceInfo2
    {
        CONST_VTBL struct IEnumDebugReferenceInfo2Vtbl *lpVtbl;
    };

    

#ifdef COBJMACROS


#define IEnumDebugReferenceInfo2_QueryInterface(This,riid,ppvObject)	\
    ( (This)->lpVtbl -> QueryInterface(This,riid,ppvObject) ) 

#define IEnumDebugReferenceInfo2_AddRef(This)	\
    ( (This)->lpVtbl -> AddRef(This) ) 

#define IEnumDebugReferenceInfo2_Release(This)	\
    ( (This)->lpVtbl -> Release(This) ) 


#define IEnumDebugReferenceInfo2_Next(This,celt,rgelt,pceltFetched)	\
    ( (This)->lpVtbl -> Next(This,celt,rgelt,pceltFetched) ) 

#define IEnumDebugReferenceInfo2_Skip(This,celt)	\
    ( (This)->lpVtbl -> Skip(This,celt) ) 

#define IEnumDebugReferenceInfo2_Reset(This)	\
    ( (This)->lpVtbl -> Reset(This) ) 

#define IEnumDebugReferenceInfo2_Clone(This,ppEnum)	\
    ( (This)->lpVtbl -> Clone(This,ppEnum) ) 

#define IEnumDebugReferenceInfo2_GetCount(This,pcelt)	\
    ( (This)->lpVtbl -> GetCount(This,pcelt) ) 

#endif /* COBJMACROS */


#endif 	/* C style interface */




#endif 	/* __IEnumDebugReferenceInfo2_INTERFACE_DEFINED__ */


#ifndef __IDebugProcessQueryProperties_INTERFACE_DEFINED__
#define __IDebugProcessQueryProperties_INTERFACE_DEFINED__

/* interface IDebugProcessQueryProperties */
/* [unique][uuid][object] */ 


enum enum_PROCESS_PROPERTY_TYPE
    {	PROCESS_PROPERTY_COMMAND_LINE	= 1,
	PROCESS_PROPERTY_CURRENT_DIRECTORY	= 2,
	PROCESS_PROPERTY_ENVIRONMENT_VARIABLES	= 3
    } ;
typedef DWORD PROCESS_PROPERTY_TYPE;


EXTERN_C const IID IID_IDebugProcessQueryProperties;

#if defined(__cplusplus) && !defined(CINTERFACE)
    
    MIDL_INTERFACE("230A0071-62EF-4cae-AAC0-8988C37024BF")
    IDebugProcessQueryProperties : public IUnknown
    {
    public:
        virtual HRESULT STDMETHODCALLTYPE QueryProperty( 
            /* [in] */ PROCESS_PROPERTY_TYPE dwPropType,
            /* [out] */ __RPC__out VARIANT *pvarPropValue) = 0;
        
        virtual HRESULT STDMETHODCALLTYPE QueryProperties( 
            /* [in] */ ULONG celt,
            /* [size_is][in] */ __RPC__in_ecount_full(celt) PROCESS_PROPERTY_TYPE *rgdwPropTypes,
            /* [size_is][out] */ __RPC__out_ecount_full(celt) VARIANT *rgtPropValues) = 0;
        
    };
    
#else 	/* C style interface */

    typedef struct IDebugProcessQueryPropertiesVtbl
    {
        BEGIN_INTERFACE
        
        HRESULT ( STDMETHODCALLTYPE *QueryInterface )( 
            IDebugProcessQueryProperties * This,
            /* [in] */ __RPC__in REFIID riid,
            /* [iid_is][out] */ 
            __RPC__deref_out  void **ppvObject);
        
        ULONG ( STDMETHODCALLTYPE *AddRef )( 
            IDebugProcessQueryProperties * This);
        
        ULONG ( STDMETHODCALLTYPE *Release )( 
            IDebugProcessQueryProperties * This);
        
        HRESULT ( STDMETHODCALLTYPE *QueryProperty )( 
            IDebugProcessQueryProperties * This,
            /* [in] */ PROCESS_PROPERTY_TYPE dwPropType,
            /* [out] */ __RPC__out VARIANT *pvarPropValue);
        
        HRESULT ( STDMETHODCALLTYPE *QueryProperties )( 
            IDebugProcessQueryProperties * This,
            /* [in] */ ULONG celt,
            /* [size_is][in] */ __RPC__in_ecount_full(celt) PROCESS_PROPERTY_TYPE *rgdwPropTypes,
            /* [size_is][out] */ __RPC__out_ecount_full(celt) VARIANT *rgtPropValues);
        
        END_INTERFACE
    } IDebugProcessQueryPropertiesVtbl;

    interface IDebugProcessQueryProperties
    {
        CONST_VTBL struct IDebugProcessQueryPropertiesVtbl *lpVtbl;
    };

    

#ifdef COBJMACROS


#define IDebugProcessQueryProperties_QueryInterface(This,riid,ppvObject)	\
    ( (This)->lpVtbl -> QueryInterface(This,riid,ppvObject) ) 

#define IDebugProcessQueryProperties_AddRef(This)	\
    ( (This)->lpVtbl -> AddRef(This) ) 

#define IDebugProcessQueryProperties_Release(This)	\
    ( (This)->lpVtbl -> Release(This) ) 


#define IDebugProcessQueryProperties_QueryProperty(This,dwPropType,pvarPropValue)	\
    ( (This)->lpVtbl -> QueryProperty(This,dwPropType,pvarPropValue) ) 

#define IDebugProcessQueryProperties_QueryProperties(This,celt,rgdwPropTypes,rgtPropValues)	\
    ( (This)->lpVtbl -> QueryProperties(This,celt,rgdwPropTypes,rgtPropValues) ) 

#endif /* COBJMACROS */


#endif 	/* C style interface */




#endif 	/* __IDebugProcessQueryProperties_INTERFACE_DEFINED__ */


#ifndef __IDebugRemoteServer2_INTERFACE_DEFINED__
#define __IDebugRemoteServer2_INTERFACE_DEFINED__

/* interface IDebugRemoteServer2 */
/* [unique][uuid][object] */ 


enum enum_ENUMERATED_PROCESS_FLAGS
    {	EPFLAG_SHOW_SECURITY_WARNING	= 0x1,
	EPFLAG_SYSTEM_PROCESS	= 0x2
    } ;
typedef DWORD ENUMERATED_PROCESS_FLAGS;


enum enum_REMOTE_PROCESS_FLAGS
    {	RPFLAG_DEBUGGER_ATTACH	= 0x100,
	RPFLAG_SQL_LOADED	= 0x200,
	RPFLAG_CLR_LOADED	= 0x400,
	RPFLAG_PROCESS_WOW64	= 0x800
    } ;
typedef DWORD REMOTE_PROCESS_FLAGS;


enum enum_REMOTE_PROCESS_INFO_FIELDS
    {	RPIF_TITLE	= 0x1,
	RPIF_MODULE_PATH	= 0x2,
	RPIF_COMMAND_LINE	= 0x4,
	RPIF_CURRENT_DIRECTORY	= 0x8,
	RPIF_ENVIRONMENT_VARIABLES	= 0x10,
	RPIF_USER_NAME	= 0x20,
	RPIF_SESSION_ID	= 0x40,
	RPIF_ENUMERATED_FLAGS	= 0x80,
	RPIF_DEBUGGER_PRESENT_FLAGS	= 0x100,
	RPIF_PROGRAM_TYPE_FLAGS	= 0x200
    } ;
typedef DWORD REMOTE_PROCESS_INFO_FIELDS;

typedef struct tagREMOTE_PROCESS_INFO
    {
    REMOTE_PROCESS_INFO_FIELDS Fields;
    BSTR bstrTitle;
    BSTR bstrModulePath;
    BSTR bstrCommandLine;
    BSTR bstrCurrentDirectory;
    BSTR bstrEnvironmentVariables;
    BSTR bstrUserName;
    DWORD dwSessionId;
    REMOTE_PROCESS_FLAGS Flags;
    } 	REMOTE_PROCESS_INFO;

typedef struct tagENUMERATED_PROCESS
    {
    DWORD dwProcessId;
    DWORD dwSessionId;
    BSTR bstrUserName;
    ENUMERATED_PROCESS_FLAGS dwProcessFlags;
    } 	ENUMERATED_PROCESS;

typedef struct tagENUMERATED_PROCESS_ARRAY
    {
    DWORD dwCount;
    ENUMERATED_PROCESS *Members;
    } 	ENUMERATED_PROCESS_ARRAY;

typedef struct tagPROCESS_LAUNCH_INFO
    {
    LPCOLESTR pszExe;
    LPCOLESTR pszArgs;
    LPCOLESTR pszDir;
    BSTR bstrEnv;
    BOOL fLaunchSuspended;
    } 	PROCESS_LAUNCH_INFO;

typedef struct tagWATCH_COOKIE
    {
    UINT64 val;
    } 	WATCH_COOKIE;

typedef struct tagRESUME_COOKIE
    {
    UINT64 val;
    } 	RESUME_COOKIE;


EXTERN_C const IID IID_IDebugRemoteServer2;

#if defined(__cplusplus) && !defined(CINTERFACE)
    
    MIDL_INTERFACE("3d3ce5c4-1508-4711-a5eb-f848f6e10072")
    IDebugRemoteServer2 : public IUnknown
    {
    public:
        virtual HRESULT STDMETHODCALLTYPE GetRemoteServerName( 
            /* [out] */ __RPC__deref_out_opt BSTR *pbstrName) = 0;
        
        virtual HRESULT STDMETHODCALLTYPE GetRemoteComputerInfo( 
            /* [out] */ __RPC__out COMPUTER_INFO *pInfo) = 0;
        
        virtual HRESULT STDMETHODCALLTYPE EnumRemoteProcesses( 
            /* [out] */ __RPC__out ENUMERATED_PROCESS_ARRAY *pProcessArray) = 0;
        
        virtual HRESULT STDMETHODCALLTYPE GetRemoteProcessInfo( 
            /* [in] */ DWORD dwProcessId,
            /* [in] */ REMOTE_PROCESS_INFO_FIELDS Fields,
            /* [out] */ __RPC__out REMOTE_PROCESS_INFO *pInfo) = 0;
        
        virtual HRESULT STDMETHODCALLTYPE CreateRemoteInstance( 
            /* [full][in] */ __RPC__in_opt LPCWSTR szDll,
            /* [in] */ WORD wLangId,
            /* [in] */ __RPC__in REFCLSID clsidObject,
            /* [in] */ __RPC__in REFIID riid,
            /* [iid_is][out] */ __RPC__deref_out_opt void **ppvObject) = 0;
        
        virtual HRESULT STDMETHODCALLTYPE WatchForRemoteProcessDestroy( 
            /* [in] */ __RPC__in_opt IDebugPortEvents2 *pCallback,
            /* [in] */ __RPC__in_opt IDebugProcess2 *pProcess,
            /* [out] */ __RPC__out WATCH_COOKIE *pWatchCookie) = 0;
        
        virtual HRESULT STDMETHODCALLTYPE CloseRemoteWatchCookie( 
            /* [in] */ WATCH_COOKIE WatchCookie) = 0;
        
        virtual HRESULT STDMETHODCALLTYPE TerminateRemoteProcess( 
            /* [in] */ DWORD dwProcessId) = 0;
        
        virtual HRESULT STDMETHODCALLTYPE LaunchRemoteProcess( 
            /* [in] */ PROCESS_LAUNCH_INFO LaunchInfo,
            /* [out] */ __RPC__out DWORD *pdwProcessId,
            /* [out] */ __RPC__out RESUME_COOKIE *pResumeCookie) = 0;
        
        virtual HRESULT STDMETHODCALLTYPE CloseRemoteResumeCookie( 
            /* [in] */ RESUME_COOKIE ResumeCookie) = 0;
        
        virtual HRESULT STDMETHODCALLTYPE DiagnoseRemoteWebDebuggingError( 
            /* [full][in] */ __RPC__in_opt LPCWSTR szUrl) = 0;
        
    };
    
#else 	/* C style interface */

    typedef struct IDebugRemoteServer2Vtbl
    {
        BEGIN_INTERFACE
        
        HRESULT ( STDMETHODCALLTYPE *QueryInterface )( 
            IDebugRemoteServer2 * This,
            /* [in] */ __RPC__in REFIID riid,
            /* [iid_is][out] */ 
            __RPC__deref_out  void **ppvObject);
        
        ULONG ( STDMETHODCALLTYPE *AddRef )( 
            IDebugRemoteServer2 * This);
        
        ULONG ( STDMETHODCALLTYPE *Release )( 
            IDebugRemoteServer2 * This);
        
        HRESULT ( STDMETHODCALLTYPE *GetRemoteServerName )( 
            IDebugRemoteServer2 * This,
            /* [out] */ __RPC__deref_out_opt BSTR *pbstrName);
        
        HRESULT ( STDMETHODCALLTYPE *GetRemoteComputerInfo )( 
            IDebugRemoteServer2 * This,
            /* [out] */ __RPC__out COMPUTER_INFO *pInfo);
        
        HRESULT ( STDMETHODCALLTYPE *EnumRemoteProcesses )( 
            IDebugRemoteServer2 * This,
            /* [out] */ __RPC__out ENUMERATED_PROCESS_ARRAY *pProcessArray);
        
        HRESULT ( STDMETHODCALLTYPE *GetRemoteProcessInfo )( 
            IDebugRemoteServer2 * This,
            /* [in] */ DWORD dwProcessId,
            /* [in] */ REMOTE_PROCESS_INFO_FIELDS Fields,
            /* [out] */ __RPC__out REMOTE_PROCESS_INFO *pInfo);
        
        HRESULT ( STDMETHODCALLTYPE *CreateRemoteInstance )( 
            IDebugRemoteServer2 * This,
            /* [full][in] */ __RPC__in_opt LPCWSTR szDll,
            /* [in] */ WORD wLangId,
            /* [in] */ __RPC__in REFCLSID clsidObject,
            /* [in] */ __RPC__in REFIID riid,
            /* [iid_is][out] */ __RPC__deref_out_opt void **ppvObject);
        
        HRESULT ( STDMETHODCALLTYPE *WatchForRemoteProcessDestroy )( 
            IDebugRemoteServer2 * This,
            /* [in] */ __RPC__in_opt IDebugPortEvents2 *pCallback,
            /* [in] */ __RPC__in_opt IDebugProcess2 *pProcess,
            /* [out] */ __RPC__out WATCH_COOKIE *pWatchCookie);
        
        HRESULT ( STDMETHODCALLTYPE *CloseRemoteWatchCookie )( 
            IDebugRemoteServer2 * This,
            /* [in] */ WATCH_COOKIE WatchCookie);
        
        HRESULT ( STDMETHODCALLTYPE *TerminateRemoteProcess )( 
            IDebugRemoteServer2 * This,
            /* [in] */ DWORD dwProcessId);
        
        HRESULT ( STDMETHODCALLTYPE *LaunchRemoteProcess )( 
            IDebugRemoteServer2 * This,
            /* [in] */ PROCESS_LAUNCH_INFO LaunchInfo,
            /* [out] */ __RPC__out DWORD *pdwProcessId,
            /* [out] */ __RPC__out RESUME_COOKIE *pResumeCookie);
        
        HRESULT ( STDMETHODCALLTYPE *CloseRemoteResumeCookie )( 
            IDebugRemoteServer2 * This,
            /* [in] */ RESUME_COOKIE ResumeCookie);
        
        HRESULT ( STDMETHODCALLTYPE *DiagnoseRemoteWebDebuggingError )( 
            IDebugRemoteServer2 * This,
            /* [full][in] */ __RPC__in_opt LPCWSTR szUrl);
        
        END_INTERFACE
    } IDebugRemoteServer2Vtbl;

    interface IDebugRemoteServer2
    {
        CONST_VTBL struct IDebugRemoteServer2Vtbl *lpVtbl;
    };

    

#ifdef COBJMACROS


#define IDebugRemoteServer2_QueryInterface(This,riid,ppvObject)	\
    ( (This)->lpVtbl -> QueryInterface(This,riid,ppvObject) ) 

#define IDebugRemoteServer2_AddRef(This)	\
    ( (This)->lpVtbl -> AddRef(This) ) 

#define IDebugRemoteServer2_Release(This)	\
    ( (This)->lpVtbl -> Release(This) ) 


#define IDebugRemoteServer2_GetRemoteServerName(This,pbstrName)	\
    ( (This)->lpVtbl -> GetRemoteServerName(This,pbstrName) ) 

#define IDebugRemoteServer2_GetRemoteComputerInfo(This,pInfo)	\
    ( (This)->lpVtbl -> GetRemoteComputerInfo(This,pInfo) ) 

#define IDebugRemoteServer2_EnumRemoteProcesses(This,pProcessArray)	\
    ( (This)->lpVtbl -> EnumRemoteProcesses(This,pProcessArray) ) 

#define IDebugRemoteServer2_GetRemoteProcessInfo(This,dwProcessId,Fields,pInfo)	\
    ( (This)->lpVtbl -> GetRemoteProcessInfo(This,dwProcessId,Fields,pInfo) ) 

#define IDebugRemoteServer2_CreateRemoteInstance(This,szDll,wLangId,clsidObject,riid,ppvObject)	\
    ( (This)->lpVtbl -> CreateRemoteInstance(This,szDll,wLangId,clsidObject,riid,ppvObject) ) 

#define IDebugRemoteServer2_WatchForRemoteProcessDestroy(This,pCallback,pProcess,pWatchCookie)	\
    ( (This)->lpVtbl -> WatchForRemoteProcessDestroy(This,pCallback,pProcess,pWatchCookie) ) 

#define IDebugRemoteServer2_CloseRemoteWatchCookie(This,WatchCookie)	\
    ( (This)->lpVtbl -> CloseRemoteWatchCookie(This,WatchCookie) ) 

#define IDebugRemoteServer2_TerminateRemoteProcess(This,dwProcessId)	\
    ( (This)->lpVtbl -> TerminateRemoteProcess(This,dwProcessId) ) 

#define IDebugRemoteServer2_LaunchRemoteProcess(This,LaunchInfo,pdwProcessId,pResumeCookie)	\
    ( (This)->lpVtbl -> LaunchRemoteProcess(This,LaunchInfo,pdwProcessId,pResumeCookie) ) 

#define IDebugRemoteServer2_CloseRemoteResumeCookie(This,ResumeCookie)	\
    ( (This)->lpVtbl -> CloseRemoteResumeCookie(This,ResumeCookie) ) 

#define IDebugRemoteServer2_DiagnoseRemoteWebDebuggingError(This,szUrl)	\
    ( (This)->lpVtbl -> DiagnoseRemoteWebDebuggingError(This,szUrl) ) 

#endif /* COBJMACROS */


#endif 	/* C style interface */




#endif 	/* __IDebugRemoteServer2_INTERFACE_DEFINED__ */


#ifndef __IDebugRemoteServerFactory2_INTERFACE_DEFINED__
#define __IDebugRemoteServerFactory2_INTERFACE_DEFINED__

/* interface IDebugRemoteServerFactory2 */
/* [unique][uuid][object] */ 


EXTERN_C const IID IID_IDebugRemoteServerFactory2;

#if defined(__cplusplus) && !defined(CINTERFACE)
    
    MIDL_INTERFACE("4a5af829-ca32-4b01-aae4-4c53d260e75c")
    IDebugRemoteServerFactory2 : public IUnknown
    {
    public:
        virtual HRESULT STDMETHODCALLTYPE CreateServer( 
            /* [in] */ __RPC__in_opt IDebugSession2 *pSession,
            /* [out] */ __RPC__deref_out_opt IDebugRemoteServer2 **ppRemoteServer) = 0;
        
    };
    
#else 	/* C style interface */

    typedef struct IDebugRemoteServerFactory2Vtbl
    {
        BEGIN_INTERFACE
        
        HRESULT ( STDMETHODCALLTYPE *QueryInterface )( 
            IDebugRemoteServerFactory2 * This,
            /* [in] */ __RPC__in REFIID riid,
            /* [iid_is][out] */ 
            __RPC__deref_out  void **ppvObject);
        
        ULONG ( STDMETHODCALLTYPE *AddRef )( 
            IDebugRemoteServerFactory2 * This);
        
        ULONG ( STDMETHODCALLTYPE *Release )( 
            IDebugRemoteServerFactory2 * This);
        
        HRESULT ( STDMETHODCALLTYPE *CreateServer )( 
            IDebugRemoteServerFactory2 * This,
            /* [in] */ __RPC__in_opt IDebugSession2 *pSession,
            /* [out] */ __RPC__deref_out_opt IDebugRemoteServer2 **ppRemoteServer);
        
        END_INTERFACE
    } IDebugRemoteServerFactory2Vtbl;

    interface IDebugRemoteServerFactory2
    {
        CONST_VTBL struct IDebugRemoteServerFactory2Vtbl *lpVtbl;
    };

    

#ifdef COBJMACROS


#define IDebugRemoteServerFactory2_QueryInterface(This,riid,ppvObject)	\
    ( (This)->lpVtbl -> QueryInterface(This,riid,ppvObject) ) 

#define IDebugRemoteServerFactory2_AddRef(This)	\
    ( (This)->lpVtbl -> AddRef(This) ) 

#define IDebugRemoteServerFactory2_Release(This)	\
    ( (This)->lpVtbl -> Release(This) ) 


#define IDebugRemoteServerFactory2_CreateServer(This,pSession,ppRemoteServer)	\
    ( (This)->lpVtbl -> CreateServer(This,pSession,ppRemoteServer) ) 

#endif /* COBJMACROS */


#endif 	/* C style interface */




#endif 	/* __IDebugRemoteServerFactory2_INTERFACE_DEFINED__ */


#ifndef __IDebugProgramPublisher2_INTERFACE_DEFINED__
#define __IDebugProgramPublisher2_INTERFACE_DEFINED__

/* interface IDebugProgramPublisher2 */
/* [unique][uuid][object] */ 


EXTERN_C const IID IID_IDebugProgramPublisher2;

#if defined(__cplusplus) && !defined(CINTERFACE)
    
    MIDL_INTERFACE("a3dddf26-7792-4544-a9a4-d4dfb11cd8f3")
    IDebugProgramPublisher2 : public IUnknown
    {
    public:
        virtual HRESULT STDMETHODCALLTYPE PublishProgramNode( 
            /* [in] */ __RPC__in_opt IDebugProgramNode2 *pProgramNode) = 0;
        
        virtual HRESULT STDMETHODCALLTYPE UnpublishProgramNode( 
            /* [in] */ __RPC__in_opt IDebugProgramNode2 *pProgramNode) = 0;
        
        virtual HRESULT STDMETHODCALLTYPE PublishProgram( 
            /* [in] */ CONST_GUID_ARRAY Engines,
            /* [full][in] */ __RPC__in_opt LPCOLESTR szFriendlyName,
            /* [in] */ __RPC__in_opt IUnknown *pDebuggeeInterface) = 0;
        
        virtual HRESULT STDMETHODCALLTYPE UnpublishProgram( 
            /* [in] */ __RPC__in_opt IUnknown *pDebuggeeInterface) = 0;
        
        virtual HRESULT STDMETHODCALLTYPE SetDebuggerPresent( 
            /* [in] */ BOOL fDebuggerPresent) = 0;
        
    };
    
#else 	/* C style interface */

    typedef struct IDebugProgramPublisher2Vtbl
    {
        BEGIN_INTERFACE
        
        HRESULT ( STDMETHODCALLTYPE *QueryInterface )( 
            IDebugProgramPublisher2 * This,
            /* [in] */ __RPC__in REFIID riid,
            /* [iid_is][out] */ 
            __RPC__deref_out  void **ppvObject);
        
        ULONG ( STDMETHODCALLTYPE *AddRef )( 
            IDebugProgramPublisher2 * This);
        
        ULONG ( STDMETHODCALLTYPE *Release )( 
            IDebugProgramPublisher2 * This);
        
        HRESULT ( STDMETHODCALLTYPE *PublishProgramNode )( 
            IDebugProgramPublisher2 * This,
            /* [in] */ __RPC__in_opt IDebugProgramNode2 *pProgramNode);
        
        HRESULT ( STDMETHODCALLTYPE *UnpublishProgramNode )( 
            IDebugProgramPublisher2 * This,
            /* [in] */ __RPC__in_opt IDebugProgramNode2 *pProgramNode);
        
        HRESULT ( STDMETHODCALLTYPE *PublishProgram )( 
            IDebugProgramPublisher2 * This,
            /* [in] */ CONST_GUID_ARRAY Engines,
            /* [full][in] */ __RPC__in_opt LPCOLESTR szFriendlyName,
            /* [in] */ __RPC__in_opt IUnknown *pDebuggeeInterface);
        
        HRESULT ( STDMETHODCALLTYPE *UnpublishProgram )( 
            IDebugProgramPublisher2 * This,
            /* [in] */ __RPC__in_opt IUnknown *pDebuggeeInterface);
        
        HRESULT ( STDMETHODCALLTYPE *SetDebuggerPresent )( 
            IDebugProgramPublisher2 * This,
            /* [in] */ BOOL fDebuggerPresent);
        
        END_INTERFACE
    } IDebugProgramPublisher2Vtbl;

    interface IDebugProgramPublisher2
    {
        CONST_VTBL struct IDebugProgramPublisher2Vtbl *lpVtbl;
    };

    

#ifdef COBJMACROS


#define IDebugProgramPublisher2_QueryInterface(This,riid,ppvObject)	\
    ( (This)->lpVtbl -> QueryInterface(This,riid,ppvObject) ) 

#define IDebugProgramPublisher2_AddRef(This)	\
    ( (This)->lpVtbl -> AddRef(This) ) 

#define IDebugProgramPublisher2_Release(This)	\
    ( (This)->lpVtbl -> Release(This) ) 


#define IDebugProgramPublisher2_PublishProgramNode(This,pProgramNode)	\
    ( (This)->lpVtbl -> PublishProgramNode(This,pProgramNode) ) 

#define IDebugProgramPublisher2_UnpublishProgramNode(This,pProgramNode)	\
    ( (This)->lpVtbl -> UnpublishProgramNode(This,pProgramNode) ) 

#define IDebugProgramPublisher2_PublishProgram(This,Engines,szFriendlyName,pDebuggeeInterface)	\
    ( (This)->lpVtbl -> PublishProgram(This,Engines,szFriendlyName,pDebuggeeInterface) ) 

#define IDebugProgramPublisher2_UnpublishProgram(This,pDebuggeeInterface)	\
    ( (This)->lpVtbl -> UnpublishProgram(This,pDebuggeeInterface) ) 

#define IDebugProgramPublisher2_SetDebuggerPresent(This,fDebuggerPresent)	\
    ( (This)->lpVtbl -> SetDebuggerPresent(This,fDebuggerPresent) ) 

#endif /* COBJMACROS */


#endif 	/* C style interface */




#endif 	/* __IDebugProgramPublisher2_INTERFACE_DEFINED__ */


#ifndef __IDebugProgramProvider2_INTERFACE_DEFINED__
#define __IDebugProgramProvider2_INTERFACE_DEFINED__

/* interface IDebugProgramProvider2 */
/* [unique][uuid][object] */ 


enum enum_PROVIDER_FLAGS
    {	PFLAG_NONE	= 0,
	PFLAG_REMOTE_PORT	= 0x1,
	PFLAG_DEBUGGEE	= 0x2,
	PFLAG_ATTACHED_TO_DEBUGGEE	= 0x4,
	PFLAG_REASON_WATCH	= 0x8,
	PFLAG_GET_PROGRAM_NODES	= 0x10,
	PFLAG_GET_IS_DEBUGGER_PRESENT	= 0x20
    } ;
typedef DWORD PROVIDER_FLAGS;


enum enum_PROVIDER_FIELDS
    {	PFIELD_PROGRAM_NODES	= 0x1,
	PFIELD_IS_DEBUGGER_PRESENT	= 0x2
    } ;
typedef DWORD PROVIDER_FIELDS;

typedef struct tagPROGRAM_NODE_ARRAY
    {
    DWORD dwCount;
    IDebugProgramNode2 **Members;
    } 	PROGRAM_NODE_ARRAY;

typedef struct tagPROVIDER_PROCESS_DATA
    {
    PROVIDER_FIELDS Fields;
    PROGRAM_NODE_ARRAY ProgramNodes;
    BOOL fIsDebuggerPresent;
    } 	PROVIDER_PROCESS_DATA;


EXTERN_C const IID IID_IDebugProgramProvider2;

#if defined(__cplusplus) && !defined(CINTERFACE)
    
    MIDL_INTERFACE("1959530a-8e53-4e09-ad11-1b7334811cad")
    IDebugProgramProvider2 : public IUnknown
    {
    public:
        virtual HRESULT STDMETHODCALLTYPE GetProviderProcessData( 
            /* [in] */ PROVIDER_FLAGS Flags,
            /* [in] */ __RPC__in_opt IDebugDefaultPort2 *pPort,
            /* [in] */ AD_PROCESS_ID processId,
            /* [in] */ CONST_GUID_ARRAY EngineFilter,
            /* [out] */ __RPC__out PROVIDER_PROCESS_DATA *pProcess) = 0;
        
        virtual HRESULT STDMETHODCALLTYPE GetProviderProgramNode( 
            /* [in] */ PROVIDER_FLAGS Flags,
            /* [in] */ __RPC__in_opt IDebugDefaultPort2 *pPort,
            /* [in] */ AD_PROCESS_ID processId,
            /* [in] */ __RPC__in REFGUID guidEngine,
            /* [in] */ UINT64 programId,
            /* [out] */ __RPC__deref_out_opt IDebugProgramNode2 **ppProgramNode) = 0;
        
        virtual HRESULT STDMETHODCALLTYPE WatchForProviderEvents( 
            /* [in] */ PROVIDER_FLAGS Flags,
            /* [in] */ __RPC__in_opt IDebugDefaultPort2 *pPort,
            /* [in] */ AD_PROCESS_ID processId,
            /* [in] */ CONST_GUID_ARRAY EngineFilter,
            /* [in] */ __RPC__in REFGUID guidLaunchingEngine,
            /* [in] */ __RPC__in_opt IDebugPortNotify2 *pEventCallback) = 0;
        
        virtual HRESULT STDMETHODCALLTYPE SetLocale( 
            /* [in] */ WORD wLangID) = 0;
        
    };
    
#else 	/* C style interface */

    typedef struct IDebugProgramProvider2Vtbl
    {
        BEGIN_INTERFACE
        
        HRESULT ( STDMETHODCALLTYPE *QueryInterface )( 
            IDebugProgramProvider2 * This,
            /* [in] */ __RPC__in REFIID riid,
            /* [iid_is][out] */ 
            __RPC__deref_out  void **ppvObject);
        
        ULONG ( STDMETHODCALLTYPE *AddRef )( 
            IDebugProgramProvider2 * This);
        
        ULONG ( STDMETHODCALLTYPE *Release )( 
            IDebugProgramProvider2 * This);
        
        HRESULT ( STDMETHODCALLTYPE *GetProviderProcessData )( 
            IDebugProgramProvider2 * This,
            /* [in] */ PROVIDER_FLAGS Flags,
            /* [in] */ __RPC__in_opt IDebugDefaultPort2 *pPort,
            /* [in] */ AD_PROCESS_ID processId,
            /* [in] */ CONST_GUID_ARRAY EngineFilter,
            /* [out] */ __RPC__out PROVIDER_PROCESS_DATA *pProcess);
        
        HRESULT ( STDMETHODCALLTYPE *GetProviderProgramNode )( 
            IDebugProgramProvider2 * This,
            /* [in] */ PROVIDER_FLAGS Flags,
            /* [in] */ __RPC__in_opt IDebugDefaultPort2 *pPort,
            /* [in] */ AD_PROCESS_ID processId,
            /* [in] */ __RPC__in REFGUID guidEngine,
            /* [in] */ UINT64 programId,
            /* [out] */ __RPC__deref_out_opt IDebugProgramNode2 **ppProgramNode);
        
        HRESULT ( STDMETHODCALLTYPE *WatchForProviderEvents )( 
            IDebugProgramProvider2 * This,
            /* [in] */ PROVIDER_FLAGS Flags,
            /* [in] */ __RPC__in_opt IDebugDefaultPort2 *pPort,
            /* [in] */ AD_PROCESS_ID processId,
            /* [in] */ CONST_GUID_ARRAY EngineFilter,
            /* [in] */ __RPC__in REFGUID guidLaunchingEngine,
            /* [in] */ __RPC__in_opt IDebugPortNotify2 *pEventCallback);
        
        HRESULT ( STDMETHODCALLTYPE *SetLocale )( 
            IDebugProgramProvider2 * This,
            /* [in] */ WORD wLangID);
        
        END_INTERFACE
    } IDebugProgramProvider2Vtbl;

    interface IDebugProgramProvider2
    {
        CONST_VTBL struct IDebugProgramProvider2Vtbl *lpVtbl;
    };

    

#ifdef COBJMACROS


#define IDebugProgramProvider2_QueryInterface(This,riid,ppvObject)	\
    ( (This)->lpVtbl -> QueryInterface(This,riid,ppvObject) ) 

#define IDebugProgramProvider2_AddRef(This)	\
    ( (This)->lpVtbl -> AddRef(This) ) 

#define IDebugProgramProvider2_Release(This)	\
    ( (This)->lpVtbl -> Release(This) ) 


#define IDebugProgramProvider2_GetProviderProcessData(This,Flags,pPort,processId,EngineFilter,pProcess)	\
    ( (This)->lpVtbl -> GetProviderProcessData(This,Flags,pPort,processId,EngineFilter,pProcess) ) 

#define IDebugProgramProvider2_GetProviderProgramNode(This,Flags,pPort,processId,guidEngine,programId,ppProgramNode)	\
    ( (This)->lpVtbl -> GetProviderProgramNode(This,Flags,pPort,processId,guidEngine,programId,ppProgramNode) ) 

#define IDebugProgramProvider2_WatchForProviderEvents(This,Flags,pPort,processId,EngineFilter,guidLaunchingEngine,pEventCallback)	\
    ( (This)->lpVtbl -> WatchForProviderEvents(This,Flags,pPort,processId,EngineFilter,guidLaunchingEngine,pEventCallback) ) 

#define IDebugProgramProvider2_SetLocale(This,wLangID)	\
    ( (This)->lpVtbl -> SetLocale(This,wLangID) ) 

#endif /* COBJMACROS */


#endif 	/* C style interface */




#endif 	/* __IDebugProgramProvider2_INTERFACE_DEFINED__ */


#ifndef __IDebugProviderProgramNode2_INTERFACE_DEFINED__
#define __IDebugProviderProgramNode2_INTERFACE_DEFINED__

/* interface IDebugProviderProgramNode2 */
/* [unique][uuid][object] */ 


EXTERN_C const IID IID_IDebugProviderProgramNode2;

#if defined(__cplusplus) && !defined(CINTERFACE)
    
    MIDL_INTERFACE("afdba726-047a-4b83-b8c7-d812fe9caa5c")
    IDebugProviderProgramNode2 : public IUnknown
    {
    public:
        virtual HRESULT STDMETHODCALLTYPE UnmarshalDebuggeeInterface( 
            /* [in] */ __RPC__in REFIID riid,
            /* [iid_is][out] */ __RPC__deref_out_opt void **ppvObject) = 0;
        
    };
    
#else 	/* C style interface */

    typedef struct IDebugProviderProgramNode2Vtbl
    {
        BEGIN_INTERFACE
        
        HRESULT ( STDMETHODCALLTYPE *QueryInterface )( 
            IDebugProviderProgramNode2 * This,
            /* [in] */ __RPC__in REFIID riid,
            /* [iid_is][out] */ 
            __RPC__deref_out  void **ppvObject);
        
        ULONG ( STDMETHODCALLTYPE *AddRef )( 
            IDebugProviderProgramNode2 * This);
        
        ULONG ( STDMETHODCALLTYPE *Release )( 
            IDebugProviderProgramNode2 * This);
        
        HRESULT ( STDMETHODCALLTYPE *UnmarshalDebuggeeInterface )( 
            IDebugProviderProgramNode2 * This,
            /* [in] */ __RPC__in REFIID riid,
            /* [iid_is][out] */ __RPC__deref_out_opt void **ppvObject);
        
        END_INTERFACE
    } IDebugProviderProgramNode2Vtbl;

    interface IDebugProviderProgramNode2
    {
        CONST_VTBL struct IDebugProviderProgramNode2Vtbl *lpVtbl;
    };

    

#ifdef COBJMACROS


#define IDebugProviderProgramNode2_QueryInterface(This,riid,ppvObject)	\
    ( (This)->lpVtbl -> QueryInterface(This,riid,ppvObject) ) 

#define IDebugProviderProgramNode2_AddRef(This)	\
    ( (This)->lpVtbl -> AddRef(This) ) 

#define IDebugProviderProgramNode2_Release(This)	\
    ( (This)->lpVtbl -> Release(This) ) 


#define IDebugProviderProgramNode2_UnmarshalDebuggeeInterface(This,riid,ppvObject)	\
    ( (This)->lpVtbl -> UnmarshalDebuggeeInterface(This,riid,ppvObject) ) 

#endif /* COBJMACROS */


#endif 	/* C style interface */




#endif 	/* __IDebugProviderProgramNode2_INTERFACE_DEFINED__ */


#ifndef __IDebugFirewallConfigurationCallback2_INTERFACE_DEFINED__
#define __IDebugFirewallConfigurationCallback2_INTERFACE_DEFINED__

/* interface IDebugFirewallConfigurationCallback2 */
/* [unique][uuid][object] */ 


EXTERN_C const IID IID_IDebugFirewallConfigurationCallback2;

#if defined(__cplusplus) && !defined(CINTERFACE)
    
    MIDL_INTERFACE("ba3288db-224a-4fd6-a37e-64e7abe9c4a1")
    IDebugFirewallConfigurationCallback2 : public IUnknown
    {
    public:
        virtual HRESULT STDMETHODCALLTYPE EnsureDCOMUnblocked( void) = 0;
        
    };
    
#else 	/* C style interface */

    typedef struct IDebugFirewallConfigurationCallback2Vtbl
    {
        BEGIN_INTERFACE
        
        HRESULT ( STDMETHODCALLTYPE *QueryInterface )( 
            IDebugFirewallConfigurationCallback2 * This,
            /* [in] */ __RPC__in REFIID riid,
            /* [iid_is][out] */ 
            __RPC__deref_out  void **ppvObject);
        
        ULONG ( STDMETHODCALLTYPE *AddRef )( 
            IDebugFirewallConfigurationCallback2 * This);
        
        ULONG ( STDMETHODCALLTYPE *Release )( 
            IDebugFirewallConfigurationCallback2 * This);
        
        HRESULT ( STDMETHODCALLTYPE *EnsureDCOMUnblocked )( 
            IDebugFirewallConfigurationCallback2 * This);
        
        END_INTERFACE
    } IDebugFirewallConfigurationCallback2Vtbl;

    interface IDebugFirewallConfigurationCallback2
    {
        CONST_VTBL struct IDebugFirewallConfigurationCallback2Vtbl *lpVtbl;
    };

    

#ifdef COBJMACROS


#define IDebugFirewallConfigurationCallback2_QueryInterface(This,riid,ppvObject)	\
    ( (This)->lpVtbl -> QueryInterface(This,riid,ppvObject) ) 

#define IDebugFirewallConfigurationCallback2_AddRef(This)	\
    ( (This)->lpVtbl -> AddRef(This) ) 

#define IDebugFirewallConfigurationCallback2_Release(This)	\
    ( (This)->lpVtbl -> Release(This) ) 


#define IDebugFirewallConfigurationCallback2_EnsureDCOMUnblocked(This)	\
    ( (This)->lpVtbl -> EnsureDCOMUnblocked(This) ) 

#endif /* COBJMACROS */


#endif 	/* C style interface */




#endif 	/* __IDebugFirewallConfigurationCallback2_INTERFACE_DEFINED__ */


#ifndef __IDebugAttachSecurityCallback2_INTERFACE_DEFINED__
#define __IDebugAttachSecurityCallback2_INTERFACE_DEFINED__

/* interface IDebugAttachSecurityCallback2 */
/* [unique][uuid][object] */ 


EXTERN_C const IID IID_IDebugAttachSecurityCallback2;

#if defined(__cplusplus) && !defined(CINTERFACE)
    
    MIDL_INTERFACE("a19e7faf-cb6f-43ba-ac16-bde9823d6dd1")
    IDebugAttachSecurityCallback2 : public IUnknown
    {
    public:
        virtual HRESULT STDMETHODCALLTYPE OnUnsafeAttach( 
            /* [in] */ __RPC__in_opt IDebugProcess2 *pProcess) = 0;
        
    };
    
#else 	/* C style interface */

    typedef struct IDebugAttachSecurityCallback2Vtbl
    {
        BEGIN_INTERFACE
        
        HRESULT ( STDMETHODCALLTYPE *QueryInterface )( 
            IDebugAttachSecurityCallback2 * This,
            /* [in] */ __RPC__in REFIID riid,
            /* [iid_is][out] */ 
            __RPC__deref_out  void **ppvObject);
        
        ULONG ( STDMETHODCALLTYPE *AddRef )( 
            IDebugAttachSecurityCallback2 * This);
        
        ULONG ( STDMETHODCALLTYPE *Release )( 
            IDebugAttachSecurityCallback2 * This);
        
        HRESULT ( STDMETHODCALLTYPE *OnUnsafeAttach )( 
            IDebugAttachSecurityCallback2 * This,
            /* [in] */ __RPC__in_opt IDebugProcess2 *pProcess);
        
        END_INTERFACE
    } IDebugAttachSecurityCallback2Vtbl;

    interface IDebugAttachSecurityCallback2
    {
        CONST_VTBL struct IDebugAttachSecurityCallback2Vtbl *lpVtbl;
    };

    

#ifdef COBJMACROS


#define IDebugAttachSecurityCallback2_QueryInterface(This,riid,ppvObject)	\
    ( (This)->lpVtbl -> QueryInterface(This,riid,ppvObject) ) 

#define IDebugAttachSecurityCallback2_AddRef(This)	\
    ( (This)->lpVtbl -> AddRef(This) ) 

#define IDebugAttachSecurityCallback2_Release(This)	\
    ( (This)->lpVtbl -> Release(This) ) 


#define IDebugAttachSecurityCallback2_OnUnsafeAttach(This,pProcess)	\
    ( (This)->lpVtbl -> OnUnsafeAttach(This,pProcess) ) 

#endif /* COBJMACROS */


#endif 	/* C style interface */




#endif 	/* __IDebugAttachSecurityCallback2_INTERFACE_DEFINED__ */



#ifndef __AD2Lib_LIBRARY_DEFINED__
#define __AD2Lib_LIBRARY_DEFINED__

/* library AD2Lib */
/* [uuid] */ 

#ifdef DEBUG
#define MsMachineDebugManager_V7 MsMachineDebugManager_V7_DEBUG
#define CLSID_MsMachineDebugManager_V7 CLSID_MsMachineDebugManager_V7_DEBUG
#else
#define MsMachineDebugManager_V7 MsMachineDebugManager_V7_RETAIL
#define CLSID_MsMachineDebugManager_V7 CLSID_MsMachineDebugManager_V7_RETAIL
#endif
#ifdef DEBUG
#define MDMUtilServer_V7 MDMUtilServer_V7_DEBUG
#define CLSID_MDMUtilServer_V7 CLSID_MDMUtilServer_V7_DEBUG
#else
#define MDMUtilServer_V7 MDMUtilServer_V7_RETAIL
#define CLSID_MDMUtilServer_V7 CLSID_MDMUtilServer_V7_RETAIL
#endif

EXTERN_C const IID LIBID_AD2Lib;

EXTERN_C const CLSID CLSID_SDMServer;

#ifdef __cplusplus

class DECLSPEC_UUID("5eb7d9f7-af21-400e-a2c4-7fd6396f8641")
SDMServer;
#endif

EXTERN_C const CLSID CLSID_MsMachineDebugManager_V7_RETAIL;

#ifdef __cplusplus

class DECLSPEC_UUID("73b25ffd-f501-437b-8b11-7f0de383964f")
MsMachineDebugManager_V7_RETAIL;
#endif

EXTERN_C const CLSID CLSID_MsMachineDebugManager_V7_DEBUG;

#ifdef __cplusplus

class DECLSPEC_UUID("05e1b201-493d-4678-bbcb-18d9caf5c0a9")
MsMachineDebugManager_V7_DEBUG;
#endif

EXTERN_C const CLSID CLSID_MDMUtilServer_V7_RETAIL;

#ifdef __cplusplus

class DECLSPEC_UUID("b20e899d-b079-479d-a4dc-10f758d9cd9a")
MDMUtilServer_V7_RETAIL;
#endif

EXTERN_C const CLSID CLSID_MDMUtilServer_V7_DEBUG;

#ifdef __cplusplus

class DECLSPEC_UUID("89370a13-3977-4e7d-aea0-0a9751ae596b")
MDMUtilServer_V7_DEBUG;
#endif

EXTERN_C const CLSID CLSID_ProgramPublisher;

#ifdef __cplusplus

class DECLSPEC_UUID("d04d550d-1ea8-4e37-830e-700fea447688")
ProgramPublisher;
#endif

EXTERN_C const CLSID CLSID_MsProgramProvider;

#ifdef __cplusplus

class DECLSPEC_UUID("170ec3fc-4e80-40ab-a85a-55900c7c70de")
MsProgramProvider;
#endif
#endif /* __AD2Lib_LIBRARY_DEFINED__ */

/* Additional Prototypes for ALL interfaces */

unsigned long             __RPC_USER  BSTR_UserSize(     unsigned long *, unsigned long            , BSTR * ); 
unsigned char * __RPC_USER  BSTR_UserMarshal(  unsigned long *, unsigned char *, BSTR * ); 
unsigned char * __RPC_USER  BSTR_UserUnmarshal(unsigned long *, unsigned char *, BSTR * ); 
void                      __RPC_USER  BSTR_UserFree(     unsigned long *, BSTR * ); 

unsigned long             __RPC_USER  HWND_UserSize(     unsigned long *, unsigned long            , HWND * ); 
unsigned char * __RPC_USER  HWND_UserMarshal(  unsigned long *, unsigned char *, HWND * ); 
unsigned char * __RPC_USER  HWND_UserUnmarshal(unsigned long *, unsigned char *, HWND * ); 
void                      __RPC_USER  HWND_UserFree(     unsigned long *, HWND * ); 

unsigned long             __RPC_USER  VARIANT_UserSize(     unsigned long *, unsigned long            , VARIANT * ); 
unsigned char * __RPC_USER  VARIANT_UserMarshal(  unsigned long *, unsigned char *, VARIANT * ); 
unsigned char * __RPC_USER  VARIANT_UserUnmarshal(unsigned long *, unsigned char *, VARIANT * ); 
void                      __RPC_USER  VARIANT_UserFree(     unsigned long *, VARIANT * ); 

/* end of Additional Prototypes */

#ifdef __cplusplus
}
#endif

#endif


