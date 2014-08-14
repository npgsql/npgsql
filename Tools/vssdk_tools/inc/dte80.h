

/* this ALWAYS GENERATED file contains the definitions for the interfaces */


 /* File created by MIDL compiler version 7.00.0499 */
/* Compiler settings for dte80.idl:
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


#ifndef __dte80_h__
#define __dte80_h__

#if defined(_MSC_VER) && (_MSC_VER >= 1020)
#pragma once
#endif

/* Forward Declarations */ 

#ifndef __Commands2_FWD_DEFINED__
#define __Commands2_FWD_DEFINED__
typedef interface Commands2 Commands2;
#endif 	/* __Commands2_FWD_DEFINED__ */


#ifndef __SourceControl2_FWD_DEFINED__
#define __SourceControl2_FWD_DEFINED__
typedef interface SourceControl2 SourceControl2;
#endif 	/* __SourceControl2_FWD_DEFINED__ */


#ifndef __ErrorItem_FWD_DEFINED__
#define __ErrorItem_FWD_DEFINED__
typedef interface ErrorItem ErrorItem;
#endif 	/* __ErrorItem_FWD_DEFINED__ */


#ifndef __ErrorList_FWD_DEFINED__
#define __ErrorList_FWD_DEFINED__
typedef interface ErrorList ErrorList;
#endif 	/* __ErrorList_FWD_DEFINED__ */


#ifndef __CodeAttributeArgument_FWD_DEFINED__
#define __CodeAttributeArgument_FWD_DEFINED__
typedef interface CodeAttributeArgument CodeAttributeArgument;
#endif 	/* __CodeAttributeArgument_FWD_DEFINED__ */


#ifndef __CodeEvent_FWD_DEFINED__
#define __CodeEvent_FWD_DEFINED__
typedef interface CodeEvent CodeEvent;
#endif 	/* __CodeEvent_FWD_DEFINED__ */


#ifndef __CodeElement2_FWD_DEFINED__
#define __CodeElement2_FWD_DEFINED__
typedef interface CodeElement2 CodeElement2;
#endif 	/* __CodeElement2_FWD_DEFINED__ */


#ifndef __CodeImport_FWD_DEFINED__
#define __CodeImport_FWD_DEFINED__
typedef interface CodeImport CodeImport;
#endif 	/* __CodeImport_FWD_DEFINED__ */


#ifndef __FileCodeModel2_FWD_DEFINED__
#define __FileCodeModel2_FWD_DEFINED__
typedef interface FileCodeModel2 FileCodeModel2;
#endif 	/* __FileCodeModel2_FWD_DEFINED__ */


#ifndef __CodeModel2_FWD_DEFINED__
#define __CodeModel2_FWD_DEFINED__
typedef interface CodeModel2 CodeModel2;
#endif 	/* __CodeModel2_FWD_DEFINED__ */


#ifndef __CodeClass2_FWD_DEFINED__
#define __CodeClass2_FWD_DEFINED__
typedef interface CodeClass2 CodeClass2;
#endif 	/* __CodeClass2_FWD_DEFINED__ */


#ifndef __CodeParameter2_FWD_DEFINED__
#define __CodeParameter2_FWD_DEFINED__
typedef interface CodeParameter2 CodeParameter2;
#endif 	/* __CodeParameter2_FWD_DEFINED__ */


#ifndef __CodeFunction2_FWD_DEFINED__
#define __CodeFunction2_FWD_DEFINED__
typedef interface CodeFunction2 CodeFunction2;
#endif 	/* __CodeFunction2_FWD_DEFINED__ */


#ifndef __CodeAttribute2_FWD_DEFINED__
#define __CodeAttribute2_FWD_DEFINED__
typedef interface CodeAttribute2 CodeAttribute2;
#endif 	/* __CodeAttribute2_FWD_DEFINED__ */


#ifndef __CodeVariable2_FWD_DEFINED__
#define __CodeVariable2_FWD_DEFINED__
typedef interface CodeVariable2 CodeVariable2;
#endif 	/* __CodeVariable2_FWD_DEFINED__ */


#ifndef __CodeDelegate2_FWD_DEFINED__
#define __CodeDelegate2_FWD_DEFINED__
typedef interface CodeDelegate2 CodeDelegate2;
#endif 	/* __CodeDelegate2_FWD_DEFINED__ */


#ifndef __CodeStruct2_FWD_DEFINED__
#define __CodeStruct2_FWD_DEFINED__
typedef interface CodeStruct2 CodeStruct2;
#endif 	/* __CodeStruct2_FWD_DEFINED__ */


#ifndef __CodeInterface2_FWD_DEFINED__
#define __CodeInterface2_FWD_DEFINED__
typedef interface CodeInterface2 CodeInterface2;
#endif 	/* __CodeInterface2_FWD_DEFINED__ */


#ifndef __CodeTypeRef2_FWD_DEFINED__
#define __CodeTypeRef2_FWD_DEFINED__
typedef interface CodeTypeRef2 CodeTypeRef2;
#endif 	/* __CodeTypeRef2_FWD_DEFINED__ */


#ifndef __CodeProperty2_FWD_DEFINED__
#define __CodeProperty2_FWD_DEFINED__
typedef interface CodeProperty2 CodeProperty2;
#endif 	/* __CodeProperty2_FWD_DEFINED__ */


#ifndef ___dispCodeModelEvents_FWD_DEFINED__
#define ___dispCodeModelEvents_FWD_DEFINED__
typedef interface _dispCodeModelEvents _dispCodeModelEvents;
#endif 	/* ___dispCodeModelEvents_FWD_DEFINED__ */


#ifndef __CodeModelEvents_FWD_DEFINED__
#define __CodeModelEvents_FWD_DEFINED__

#ifdef __cplusplus
typedef class CodeModelEvents CodeModelEvents;
#else
typedef struct CodeModelEvents CodeModelEvents;
#endif /* __cplusplus */

#endif 	/* __CodeModelEvents_FWD_DEFINED__ */


#ifndef ___CodeModelEventsRoot_FWD_DEFINED__
#define ___CodeModelEventsRoot_FWD_DEFINED__
typedef interface _CodeModelEventsRoot _CodeModelEventsRoot;
#endif 	/* ___CodeModelEventsRoot_FWD_DEFINED__ */


#ifndef ___CodeModelEvents_FWD_DEFINED__
#define ___CodeModelEvents_FWD_DEFINED__
typedef interface _CodeModelEvents _CodeModelEvents;
#endif 	/* ___CodeModelEvents_FWD_DEFINED__ */


#ifndef __Debugger2_FWD_DEFINED__
#define __Debugger2_FWD_DEFINED__
typedef interface Debugger2 Debugger2;
#endif 	/* __Debugger2_FWD_DEFINED__ */


#ifndef __Process2_FWD_DEFINED__
#define __Process2_FWD_DEFINED__
typedef interface Process2 Process2;
#endif 	/* __Process2_FWD_DEFINED__ */


#ifndef __Breakpoint2_FWD_DEFINED__
#define __Breakpoint2_FWD_DEFINED__
typedef interface Breakpoint2 Breakpoint2;
#endif 	/* __Breakpoint2_FWD_DEFINED__ */


#ifndef __Engine_FWD_DEFINED__
#define __Engine_FWD_DEFINED__
typedef interface Engine Engine;
#endif 	/* __Engine_FWD_DEFINED__ */


#ifndef __Transport_FWD_DEFINED__
#define __Transport_FWD_DEFINED__
typedef interface Transport Transport;
#endif 	/* __Transport_FWD_DEFINED__ */


#ifndef __Engines_FWD_DEFINED__
#define __Engines_FWD_DEFINED__
typedef interface Engines Engines;
#endif 	/* __Engines_FWD_DEFINED__ */


#ifndef __Transports_FWD_DEFINED__
#define __Transports_FWD_DEFINED__
typedef interface Transports Transports;
#endif 	/* __Transports_FWD_DEFINED__ */


#ifndef ___dispDebuggerProcessEvents_FWD_DEFINED__
#define ___dispDebuggerProcessEvents_FWD_DEFINED__
typedef interface _dispDebuggerProcessEvents _dispDebuggerProcessEvents;
#endif 	/* ___dispDebuggerProcessEvents_FWD_DEFINED__ */


#ifndef ___DebuggerProcessEventsRoot_FWD_DEFINED__
#define ___DebuggerProcessEventsRoot_FWD_DEFINED__
typedef interface _DebuggerProcessEventsRoot _DebuggerProcessEventsRoot;
#endif 	/* ___DebuggerProcessEventsRoot_FWD_DEFINED__ */


#ifndef ___DebuggerProcessEvents_FWD_DEFINED__
#define ___DebuggerProcessEvents_FWD_DEFINED__
typedef interface _DebuggerProcessEvents _DebuggerProcessEvents;
#endif 	/* ___DebuggerProcessEvents_FWD_DEFINED__ */


#ifndef __DebuggerProcessEvents_FWD_DEFINED__
#define __DebuggerProcessEvents_FWD_DEFINED__

#ifdef __cplusplus
typedef class DebuggerProcessEvents DebuggerProcessEvents;
#else
typedef struct DebuggerProcessEvents DebuggerProcessEvents;
#endif /* __cplusplus */

#endif 	/* __DebuggerProcessEvents_FWD_DEFINED__ */


#ifndef ___dispDebuggerExpressionEvaluationEvents_FWD_DEFINED__
#define ___dispDebuggerExpressionEvaluationEvents_FWD_DEFINED__
typedef interface _dispDebuggerExpressionEvaluationEvents _dispDebuggerExpressionEvaluationEvents;
#endif 	/* ___dispDebuggerExpressionEvaluationEvents_FWD_DEFINED__ */


#ifndef ___DebuggerExpressionEvaluationEvents_FWD_DEFINED__
#define ___DebuggerExpressionEvaluationEvents_FWD_DEFINED__
typedef interface _DebuggerExpressionEvaluationEvents _DebuggerExpressionEvaluationEvents;
#endif 	/* ___DebuggerExpressionEvaluationEvents_FWD_DEFINED__ */


#ifndef ___DebuggerExpressionEvaluationEventsRoot_FWD_DEFINED__
#define ___DebuggerExpressionEvaluationEventsRoot_FWD_DEFINED__
typedef interface _DebuggerExpressionEvaluationEventsRoot _DebuggerExpressionEvaluationEventsRoot;
#endif 	/* ___DebuggerExpressionEvaluationEventsRoot_FWD_DEFINED__ */


#ifndef __DebuggerExpressionEvaluationEvents_FWD_DEFINED__
#define __DebuggerExpressionEvaluationEvents_FWD_DEFINED__

#ifdef __cplusplus
typedef class DebuggerExpressionEvaluationEvents DebuggerExpressionEvaluationEvents;
#else
typedef struct DebuggerExpressionEvaluationEvents DebuggerExpressionEvaluationEvents;
#endif /* __cplusplus */

#endif 	/* __DebuggerExpressionEvaluationEvents_FWD_DEFINED__ */


#ifndef __ToolWindows_FWD_DEFINED__
#define __ToolWindows_FWD_DEFINED__
typedef interface ToolWindows ToolWindows;
#endif 	/* __ToolWindows_FWD_DEFINED__ */


#ifndef __Windows2_FWD_DEFINED__
#define __Windows2_FWD_DEFINED__
typedef interface Windows2 Windows2;
#endif 	/* __Windows2_FWD_DEFINED__ */


#ifndef __Window2_FWD_DEFINED__
#define __Window2_FWD_DEFINED__
typedef interface Window2 Window2;
#endif 	/* __Window2_FWD_DEFINED__ */


#ifndef __SourceControlBindings_FWD_DEFINED__
#define __SourceControlBindings_FWD_DEFINED__
typedef interface SourceControlBindings SourceControlBindings;
#endif 	/* __SourceControlBindings_FWD_DEFINED__ */


#ifndef __DTE2_FWD_DEFINED__
#define __DTE2_FWD_DEFINED__
typedef interface DTE2 DTE2;
#endif 	/* __DTE2_FWD_DEFINED__ */


#ifndef __WindowVisibilityEventsRoot_FWD_DEFINED__
#define __WindowVisibilityEventsRoot_FWD_DEFINED__
typedef interface WindowVisibilityEventsRoot WindowVisibilityEventsRoot;
#endif 	/* __WindowVisibilityEventsRoot_FWD_DEFINED__ */


#ifndef ___WindowVisibilityEvents_FWD_DEFINED__
#define ___WindowVisibilityEvents_FWD_DEFINED__
typedef interface _WindowVisibilityEvents _WindowVisibilityEvents;
#endif 	/* ___WindowVisibilityEvents_FWD_DEFINED__ */


#ifndef ___dispWindowVisibilityEvents_FWD_DEFINED__
#define ___dispWindowVisibilityEvents_FWD_DEFINED__
typedef interface _dispWindowVisibilityEvents _dispWindowVisibilityEvents;
#endif 	/* ___dispWindowVisibilityEvents_FWD_DEFINED__ */


#ifndef __WindowVisibilityEvents_FWD_DEFINED__
#define __WindowVisibilityEvents_FWD_DEFINED__

#ifdef __cplusplus
typedef class WindowVisibilityEvents WindowVisibilityEvents;
#else
typedef struct WindowVisibilityEvents WindowVisibilityEvents;
#endif /* __cplusplus */

#endif 	/* __WindowVisibilityEvents_FWD_DEFINED__ */


#ifndef ___TextDocumentKeyPressEventsRoot_FWD_DEFINED__
#define ___TextDocumentKeyPressEventsRoot_FWD_DEFINED__
typedef interface _TextDocumentKeyPressEventsRoot _TextDocumentKeyPressEventsRoot;
#endif 	/* ___TextDocumentKeyPressEventsRoot_FWD_DEFINED__ */


#ifndef ___TextDocumentKeyPressEvents_FWD_DEFINED__
#define ___TextDocumentKeyPressEvents_FWD_DEFINED__
typedef interface _TextDocumentKeyPressEvents _TextDocumentKeyPressEvents;
#endif 	/* ___TextDocumentKeyPressEvents_FWD_DEFINED__ */


#ifndef ___dispTextDocumentKeyPressEvents_FWD_DEFINED__
#define ___dispTextDocumentKeyPressEvents_FWD_DEFINED__
typedef interface _dispTextDocumentKeyPressEvents _dispTextDocumentKeyPressEvents;
#endif 	/* ___dispTextDocumentKeyPressEvents_FWD_DEFINED__ */


#ifndef __TextDocumentKeyPressEvents_FWD_DEFINED__
#define __TextDocumentKeyPressEvents_FWD_DEFINED__

#ifdef __cplusplus
typedef class TextDocumentKeyPressEvents TextDocumentKeyPressEvents;
#else
typedef struct TextDocumentKeyPressEvents TextDocumentKeyPressEvents;
#endif /* __cplusplus */

#endif 	/* __TextDocumentKeyPressEvents_FWD_DEFINED__ */


#ifndef ___PublishEvents_FWD_DEFINED__
#define ___PublishEvents_FWD_DEFINED__
typedef interface _PublishEvents _PublishEvents;
#endif 	/* ___PublishEvents_FWD_DEFINED__ */


#ifndef ___dispPublishEvents_FWD_DEFINED__
#define ___dispPublishEvents_FWD_DEFINED__
typedef interface _dispPublishEvents _dispPublishEvents;
#endif 	/* ___dispPublishEvents_FWD_DEFINED__ */


#ifndef __PublishEvents_FWD_DEFINED__
#define __PublishEvents_FWD_DEFINED__

#ifdef __cplusplus
typedef class PublishEvents PublishEvents;
#else
typedef struct PublishEvents PublishEvents;
#endif /* __cplusplus */

#endif 	/* __PublishEvents_FWD_DEFINED__ */


#ifndef __Events2_FWD_DEFINED__
#define __Events2_FWD_DEFINED__
typedef interface Events2 Events2;
#endif 	/* __Events2_FWD_DEFINED__ */


#ifndef __Solution2_FWD_DEFINED__
#define __Solution2_FWD_DEFINED__
typedef interface Solution2 Solution2;
#endif 	/* __Solution2_FWD_DEFINED__ */


#ifndef __SolutionFolder_FWD_DEFINED__
#define __SolutionFolder_FWD_DEFINED__
typedef interface SolutionFolder SolutionFolder;
#endif 	/* __SolutionFolder_FWD_DEFINED__ */


#ifndef __TaskItems2_FWD_DEFINED__
#define __TaskItems2_FWD_DEFINED__
typedef interface TaskItems2 TaskItems2;
#endif 	/* __TaskItems2_FWD_DEFINED__ */


#ifndef __EditPoint2_FWD_DEFINED__
#define __EditPoint2_FWD_DEFINED__
typedef interface EditPoint2 EditPoint2;
#endif 	/* __EditPoint2_FWD_DEFINED__ */


#ifndef __IVsExtensibility2_FWD_DEFINED__
#define __IVsExtensibility2_FWD_DEFINED__
typedef interface IVsExtensibility2 IVsExtensibility2;
#endif 	/* __IVsExtensibility2_FWD_DEFINED__ */


#ifndef __IInternalExtenderProvider_FWD_DEFINED__
#define __IInternalExtenderProvider_FWD_DEFINED__
typedef interface IInternalExtenderProvider IInternalExtenderProvider;
#endif 	/* __IInternalExtenderProvider_FWD_DEFINED__ */


#ifndef __Find2_FWD_DEFINED__
#define __Find2_FWD_DEFINED__
typedef interface Find2 Find2;
#endif 	/* __Find2_FWD_DEFINED__ */


#ifndef __LifetimeInformation_FWD_DEFINED__
#define __LifetimeInformation_FWD_DEFINED__
typedef interface LifetimeInformation LifetimeInformation;
#endif 	/* __LifetimeInformation_FWD_DEFINED__ */


#ifndef __ToolBoxItem2_FWD_DEFINED__
#define __ToolBoxItem2_FWD_DEFINED__
typedef interface ToolBoxItem2 ToolBoxItem2;
#endif 	/* __ToolBoxItem2_FWD_DEFINED__ */


#ifndef __ToolBoxTab2_FWD_DEFINED__
#define __ToolBoxTab2_FWD_DEFINED__
typedef interface ToolBoxTab2 ToolBoxTab2;
#endif 	/* __ToolBoxTab2_FWD_DEFINED__ */


#ifndef __IncrementalSearch_FWD_DEFINED__
#define __IncrementalSearch_FWD_DEFINED__
typedef interface IncrementalSearch IncrementalSearch;
#endif 	/* __IncrementalSearch_FWD_DEFINED__ */


#ifndef __TextPane2_FWD_DEFINED__
#define __TextPane2_FWD_DEFINED__
typedef interface TextPane2 TextPane2;
#endif 	/* __TextPane2_FWD_DEFINED__ */


#ifndef __SolutionConfiguration2_FWD_DEFINED__
#define __SolutionConfiguration2_FWD_DEFINED__
typedef interface SolutionConfiguration2 SolutionConfiguration2;
#endif 	/* __SolutionConfiguration2_FWD_DEFINED__ */


#ifndef __IVsProfferCommands2_FWD_DEFINED__
#define __IVsProfferCommands2_FWD_DEFINED__
typedef interface IVsProfferCommands2 IVsProfferCommands2;
#endif 	/* __IVsProfferCommands2_FWD_DEFINED__ */


#ifndef __SolutionBuild2_FWD_DEFINED__
#define __SolutionBuild2_FWD_DEFINED__
typedef interface SolutionBuild2 SolutionBuild2;
#endif 	/* __SolutionBuild2_FWD_DEFINED__ */


#ifndef __ErrorItems_FWD_DEFINED__
#define __ErrorItems_FWD_DEFINED__
typedef interface ErrorItems ErrorItems;
#endif 	/* __ErrorItems_FWD_DEFINED__ */


#ifdef __cplusplus
extern "C"{
#endif 


/* interface __MIDL_itf_dte80_0000_0000 */
/* [local] */ 

#pragma once
#ifndef __INDENTSTYLE__
#define __INDENTSTYLE__
typedef /* [uuid] */  DECLSPEC_UUID("BCCEBE05-D29C-11D2-AABD-00C04F688DDE") 
enum _vsIndentStyle
    {	vsIndentStyleNone	= 0,
	vsIndentStyleDefault	= ( vsIndentStyleNone + 1 ) ,
	vsIndentStyleSmart	= ( vsIndentStyleDefault + 1 ) 
    } 	vsIndentStyle;

#endif // __INDENTSTYLE__


extern RPC_IF_HANDLE __MIDL_itf_dte80_0000_0000_v0_0_c_ifspec;
extern RPC_IF_HANDLE __MIDL_itf_dte80_0000_0000_v0_0_s_ifspec;


#ifndef __EnvDTE80_LIBRARY_DEFINED__
#define __EnvDTE80_LIBRARY_DEFINED__

/* library EnvDTE80 */
/* [version][helpstring][uuid] */ 










typedef /* [helpstring][uuid] */  DECLSPEC_UUID("d3c30b7b-11ad-4693-b1a8-e20a36c1c9f1") 
enum vsCMFunction2
    {	vsCMFunctionAddHandler	= 0x20000,
	vsCMFunctionRemoveHandler	= 0x40000,
	vsCMFunctionRaiseEvent	= 0x80000
    } 	vsCMFunction2;

typedef /* [helpstring][uuid] */  DECLSPEC_UUID("7edbc54f-4b70-4b72-a422-5e57555dbd06") 
enum vsCMElement2
    {	vsCMElementUnknown	= 40,
	vsCMElementAttributeArgument	= 41
    } 	vsCMElement2;

typedef /* [helpstring][uuid] */  DECLSPEC_UUID("7fb3c569-7faf-4070-82aa-04b18b8bbad1") 
enum vsCMTypeRef2
    {	vsCMTypeRefUnsignedChar	= 17,
	vsCMTypeRefUnsignedShort	= 18,
	vsCMTypeRefUnsignedInt	= 19,
	vsCMTypeRefUnsignedLong	= 20,
	vsCMTypeRefReference	= 21,
	vsCMTypeRefMCBoxedReference	= 22,
	vsCMTypeRefSByte	= 23
    } 	vsCMTypeRef2;

typedef /* [helpstringcontext][helpstring][helpcontext][uuid] */  DECLSPEC_UUID("A49FB892-EE3A-411E-8BD4-BB4AC6AE6608") 
enum vsCMParseStatus
    {	vsCMParseStatusError	= 1,
	vsCMParseStatusComplete	= 2
    } 	vsCMParseStatus;

typedef /* [helpstringcontext][helpstring][helpcontext][uuid] */  DECLSPEC_UUID("DBDF9319-8FCC-4950-A50D-1E6FB5490869") 
enum vsCMClassKind
    {	vsCMClassKindMainClass	= 1,
	vsCMClassKindBlueprint	= 2,
	vsCMClassKindPartialClass	= 4,
	vsCMClassKindModule	= 8
    } 	vsCMClassKind;

typedef /* [helpstringcontext][helpstring][helpcontext][uuid] */  DECLSPEC_UUID("DBDF931A-8FCC-4950-A50D-1E6FB5490869") 
enum vsCMDataTypeKind
    {	vsCMDataTypeKindMain	= 1,
	vsCMDataTypeKindBlueprint	= 2,
	vsCMDataTypeKindPartial	= 4,
	vsCMDataTypeKindModule	= 8
    } 	vsCMDataTypeKind;

typedef /* [helpstringcontext][helpstring][helpcontext][uuid] */  DECLSPEC_UUID("81DD2950-B7E7-4D51-BFD7-11B086738F6E") 
enum vsCMChangeKind
    {	vsCMChangeKindRename	= 1,
	vsCMChangeKindUnknown	= 2,
	vsCMChangeKindSignatureChange	= 4,
	vsCMChangeKindTypeRefChange	= 8,
	vsCMChangeKindBaseChange	= 16,
	vsCMChangeKindArgumentChange	= 32
    } 	vsCMChangeKind;

typedef /* [helpstringcontext][helpstring][helpcontext][uuid] */  DECLSPEC_UUID("C4541DAB-D314-452D-9760-30A615F0DC26") 
enum vsCMInheritanceKind
    {	vsCMInheritanceKindNone	= 0,
	vsCMInheritanceKindAbstract	= 1,
	vsCMInheritanceKindSealed	= 2,
	vsCMInheritanceKindNew	= 4
    } 	vsCMInheritanceKind;

typedef /* [helpstringcontext][helpstring][helpcontext][uuid] */  DECLSPEC_UUID("2366AEF3-BA49-4A96-B4A1-B8BF1ACB2600") 
enum vsCMParameterKind
    {	vsCMParameterKindNone	= 0,
	vsCMParameterKindIn	= 1,
	vsCMParameterKindRef	= 2,
	vsCMParameterKindOut	= 4,
	vsCMParameterKindOptional	= 8,
	vsCMParameterKindParamArray	= 16
    } 	vsCMParameterKind;

typedef /* [helpstringcontext][helpstring][helpcontext][uuid] */  DECLSPEC_UUID("3E30C32D-9E8D-4919-8CC4-C3D75879BC52") 
enum vsCMOverrideKind
    {	vsCMOverrideKindNone	= 0,
	vsCMOverrideKindAbstract	= 1,
	vsCMOverrideKindVirtual	= 2,
	vsCMOverrideKindOverride	= 4,
	vsCMOverrideKindNew	= 8,
	vsCMOverrideKindSealed	= 16
    } 	vsCMOverrideKind;

typedef /* [helpstringcontext][helpstring][helpcontext][uuid] */  DECLSPEC_UUID("4438EED0-E747-49AC-8D22-00F26B18755D") 
enum vsCMConstKind
    {	vsCMConstKindNone	= 0,
	vsCMConstKindConst	= 1,
	vsCMConstKindReadOnly	= 2
    } 	vsCMConstKind;

typedef /* [uuid] */  DECLSPEC_UUID("e24a10bd-9a40-43a3-9db9-afaf05d74704") 
enum vsCMPropertyKind
    {	vsCMPropertyKindReadWrite	= 0,
	vsCMPropertyKindReadOnly	= 1,
	vsCMPropertyKindWriteOnly	= 2
    } 	vsCMPropertyKind;








typedef /* [uuid] */  DECLSPEC_UUID("4232C43A-589B-44bc-8931-ED79C6A7CA2B") 
enum dbgMinidumpOption
    {	dbgMinidumpNormal	= 1,
	dbgMinidumpFull	= ( dbgMinidumpNormal + 1 ) 
    } 	dbgMinidumpOption;

typedef /* [uuid] */  DECLSPEC_UUID("ECD94EEC-EBF9-45B2-B072-1624241C7C0B") 
enum dbgEventReason2
    {	dbgEventReason2None	= 1,
	dbgEventReason2Go	= ( dbgEventReason2None + 1 ) ,
	dbgEventReason2AttachProgram	= ( dbgEventReason2Go + 1 ) ,
	dbgEventReason2DetachProgram	= ( dbgEventReason2AttachProgram + 1 ) ,
	dbgEventReason2LaunchProgram	= ( dbgEventReason2DetachProgram + 1 ) ,
	dbgEventReason2EndProgram	= ( dbgEventReason2LaunchProgram + 1 ) ,
	dbgEventReason2StopDebugging	= ( dbgEventReason2EndProgram + 1 ) ,
	dbgEventReason2Step	= ( dbgEventReason2StopDebugging + 1 ) ,
	dbgEventReason2Breakpoint	= ( dbgEventReason2Step + 1 ) ,
	dbgEventReason2ExceptionThrown	= ( dbgEventReason2Breakpoint + 1 ) ,
	dbgEventReason2ExceptionNotHandled	= ( dbgEventReason2ExceptionThrown + 1 ) ,
	dbgEventReason2UserBreak	= ( dbgEventReason2ExceptionNotHandled + 1 ) ,
	dbgEventReason2ContextSwitch	= ( dbgEventReason2UserBreak + 1 ) ,
	dbgEventReason2Evaluation	= ( dbgEventReason2ContextSwitch + 1 ) ,
	dbgEventReason2UnwindFromException	= ( dbgEventReason2Evaluation + 1 ) 
    } 	dbgEventReason2;


typedef /* [helpstringcontext][helpstring][helpcontext][uuid] */  DECLSPEC_UUID("0c57dfec-8424-4c1a-a43c-ea7922446232") 
enum dbgProcessState
    {	dbgProcessStateRun	= 1,
	dbgProcessStateStop	= ( dbgProcessStateRun + 1 ) 
    } 	dbgProcessState;

typedef /* [helpstringcontext][helpstring][helpcontext][uuid] */  DECLSPEC_UUID("f8305f32-f656-4353-a49b-6bb20c6fd263") 
enum dbgExpressionEvaluationState
    {	dbgExpressionEvaluationStateStart	= 1,
	dbgExpressionEvaluationStateStop	= ( dbgExpressionEvaluationStateStart + 1 ) 
    } 	dbgExpressionEvaluationState;

typedef /* [helpstringcontext][helpstring][helpcontext][uuid] */  DECLSPEC_UUID("6FB22721-008A-41a0-B4A9-23A7AB2A02B7") 
enum vsCommandStyle
    {	vsCommandStylePict	= 1,
	vsCommandStyleText	= 2,
	vsCommandStylePictAndText	= 3,
	vsCommandStyleComboNoAutoComplete	= 64,
	vsCommandStyleComboCaseSensitive	= 128
    } 	vsCommandStyle;

typedef /* [helpstringcontext][helpstring][helpcontext][uuid] */  DECLSPEC_UUID("765724FE-DDE5-422a-A008-198376C7B641") 
enum vsCommandControlType
    {	vsCommandControlTypeButton	= 2,
	vsCommandControlTypeDropDownCombo	= 32,
	vsCommandControlTypeMRUCombo	= 64,
	vsCommandControlTypeMRUButton	= 8192
    } 	vsCommandControlType;

typedef /* [helpstringcontext][helpstring][helpcontext][uuid] */  DECLSPEC_UUID("4eee3a14-18aa-4244-9745-6e3f35d7fc4b") 
enum vsSourceControlCheckOutOptions
    {	vsSourceControlCheckOutOptionLatestVersion	= 0,
	vsSourceControlCheckOutOptionLocalVersion	= 1
    } 	vsSourceControlCheckOutOptions;

typedef /* [helpstring][uuid] */  DECLSPEC_UUID("2C23EF05-01A5-4684-AC27-4EFD8D840BA9") 
enum vsThemeColors
    {	vsThemeColorAccentBorder	= 5,
	vsThemeColorAccentDark	= 6,
	vsThemeColorAccentLight	= 7,
	vsThemeColorAccentMedium	= 8,
	vsThemeColorAccentPale	= 9,
	vsThemeColorCommandbarBorder	= 10,
	vsThemeColorCommandbarDraghandle	= 11,
	vsThemeColorCommandbarDraghandleShadow	= 12,
	vsThemeColorCommandbarGradientBegin	= 13,
	vsThemeColorCommandbarGradientEnd	= 14,
	vsThemeColorCommandbarGradientMiddle	= 15,
	vsThemeColorCommandbarHover	= 16,
	vsThemeColorCommandbarHoveroverSelected	= 17,
	vsThemeColorCommandbarHoveroverSelectedicon	= 18,
	vsThemeColorCommandbarSelected	= 19,
	vsThemeColorCommandbarShadow	= 20,
	vsThemeColorCommandbarTextActive	= 21,
	vsThemeColorCommandbarTextHover	= 22,
	vsThemeColorCommandbarTextInactive	= 23,
	vsThemeColorCommandbarTextSelected	= 24,
	vsThemeColorControlEditHintText	= 25,
	vsThemeColorControlEditRequiredBackground	= 26,
	vsThemeColorControlEditRequiredHintText	= 27,
	vsThemeColorControlLinkText	= 28,
	vsThemeColorControlLinkTextHover	= 29,
	vsThemeColorControlLinkTextPressed	= 30,
	vsThemeColorControlOutline	= 31,
	vsThemeColorDebuggerDatatipActiveBackground	= 32,
	vsThemeColorDebuggerDatatipActiveBorder	= 33,
	vsThemeColorDebuggerDatatipActiveHighlight	= 34,
	vsThemeColorDebuggerDatatipActiveHighlightText	= 35,
	vsThemeColorDebuggerDatatipActiveSeparator	= 36,
	vsThemeColorDebuggerDatatipActiveText	= 37,
	vsThemeColorDebuggerDatatipInactiveBackground	= 38,
	vsThemeColorDebuggerDatatipInactiveBorder	= 39,
	vsThemeColorDebuggerDatatipInactiveHighlight	= 40,
	vsThemeColorDebuggerDatatipInactiveHighlightText	= 41,
	vsThemeColorDebuggerDatatipInactiveSeparator	= 42,
	vsThemeColorDebuggerDatatipInactiveText	= 43,
	vsThemeColorDesignerBackground	= 44,
	vsThemeColorDesignerSelectionDots	= 45,
	vsThemeColorDesignerTray	= 46,
	vsThemeColorDesignerWatermark	= 47,
	vsThemeColorEnvironmentBackground	= 48,
	vsThemeColorEnvironmentBackgroundGradientBegin	= 49,
	vsThemeColorEnvironmentBackgroundGradientEnd	= 50,
	vsThemeColorFileTabBorder	= 51,
	vsThemeColorFileTabChannelBackground	= 52,
	vsThemeColorFileTabGradientDark	= 53,
	vsThemeColorFileTabGradientLight	= 54,
	vsThemeColorFileTabSelectedBackground	= 55,
	vsThemeColorFileTabSelectedBorder	= 56,
	vsThemeColorFileTabSelectedText	= 57,
	vsThemeColorFileTabText	= 58,
	vsThemeColorFormSmartTagActiontagBorder	= 59,
	vsThemeColorFormSmartTagActiontagFill	= 60,
	vsThemeColorFormSmartTagObjecttagBorder	= 61,
	vsThemeColorFormSmartTagObjecttagFill	= 62,
	vsThemeColorGridHeadingBackground	= 63,
	vsThemeColorGridHeadingText	= 64,
	vsThemeColorGridLine	= 65,
	vsThemeColorHelpHowDoIPaneBackground	= 66,
	vsThemeColorHelpHowDoIPaneBorder	= 67,
	vsThemeColorHelpHowDoIPaneLink	= 68,
	vsThemeColorHelpHowDoIPaneText	= 69,
	vsThemeColorHelpHowDoITaskBackground	= 70,
	vsThemeColorHelpHowDoITaskLink	= 71,
	vsThemeColorHelpHowDoITaskText	= 72,
	vsThemeColorHelpSearchBackground	= 73,
	vsThemeColorHelpSearchBorder	= 74,
	vsThemeColorHelpSearchFitlerBackground	= 75,
	vsThemeColorHelpSearchFitlerBorder	= 76,
	vsThemeColorHelpSearchGradientBegin	= 77,
	vsThemeColorHelpSearchGradientEnd	= 78,
	vsThemeColorHelpSearchNavigationDisabled	= 79,
	vsThemeColorHelpSearchNavigationEnabled	= 80,
	vsThemeColorHelpSearchPanelRules	= 81,
	vsThemeColorHelpSearchProviderBackground	= 82,
	vsThemeColorHelpSearchProviderIcon	= 83,
	vsThemeColorHelpSearchProviderText	= 84,
	vsThemeColorHelpSearchResultLinkSelected	= 85,
	vsThemeColorHelpSearchResultLinkUnselected	= 86,
	vsThemeColorHelpSearchResultSelectedBackground	= 87,
	vsThemeColorHelpSearchResultSelectedText	= 88,
	vsThemeColorHelpSearchText	= 89,
	vsThemeColorPanelBorder	= 90,
	vsThemeColorPanelGradientDark	= 91,
	vsThemeColorPanelGradientLight	= 92,
	vsThemeColorPanelHoveroverCloseBorder	= 93,
	vsThemeColorPanelHoveroverCloseFill	= 94,
	vsThemeColorPanelHyperlink	= 95,
	vsThemeColorPanelHyperlinkHover	= 96,
	vsThemeColorPanelHyperlinkPressed	= 97,
	vsThemeColorPanelSeparator	= 98,
	vsThemeColorPanelSubGroupSeparator	= 99,
	vsThemeColorPanelText	= 100,
	vsThemeColorPanelTitlebar	= 101,
	vsThemeColorPanelTitlebarText	= 102,
	vsThemeColorPanelTitlebarUnselected	= 103,
	vsThemeColorProjectDesignerBackgroundGradientBegin	= 104,
	vsThemeColorProjectDesignerBackgroundGradientEnd	= 105,
	vsThemeColorProjectDesignerBorderOutside	= 106,
	vsThemeColorProjectDesignerBorderInside	= 107,
	vsThemeColorProjectDesignerContentsBackground	= 108,
	vsThemeColorProjectDesignerTabBackgroundGradientBegin	= 109,
	vsThemeColorProjectDesignerTabBackgroundGradientEnd	= 110,
	vsThemeColorProjectDesignerTabSelectedInsideborder	= 111,
	vsThemeColorProjectDesignerTabSelectedBorder	= 112,
	vsThemeColorProjectDesignerTabSelectedHighlight1	= 113,
	vsThemeColorProjectDesignerTabSelectedHighlight2	= 114,
	vsThemeColorProjectDesignerTabSelectedBackground	= 115,
	vsThemeColorProjectDesignerTabSepBottomGradientBegin	= 116,
	vsThemeColorProjectDesignerTabSepBottomGradientEnd	= 117,
	vsThemeColorProjectDesignerTabSepTopGradientBegin	= 118,
	vsThemeColorProjectDesignerTabSepTopGradientEnd	= 119,
	vsThemeColorScreentipBorder	= 120,
	vsThemeColorScreentipBackground	= 121,
	vsThemeColorScreentipText	= 122,
	vsThemeColorSidebarBackground	= 123,
	vsThemeColorSidebarGradientdark	= 124,
	vsThemeColorSidebarGradientlight	= 125,
	vsThemeColorSidebarText	= 126,
	vsThemeColorSmartTagBorder	= 127,
	vsThemeColorSmartTagFill	= 128,
	vsThemeColorSmartTagHoverBorder	= 129,
	vsThemeColorSmartTagHoverFill	= 130,
	vsThemeColorSmartTagHoverText	= 131,
	vsThemeColorSmartTagText	= 132,
	vsThemeColorSnaplines	= 133,
	vsThemeColorSnaplinesTextBaseline	= 134,
	vsThemeColorTasklistGridlines	= 135,
	vsThemeColorTitlebarActive	= 136,
	vsThemeColorTitlebarActiveGradientBegin	= 137,
	vsThemeColorTitlebarActiveGradientEnd	= 138,
	vsThemeColorTitlebarActiveText	= 139,
	vsThemeColorTitlebarInactive	= 140,
	vsThemeColorTitlebarInactiveGradientBegin	= 141,
	vsThemeColorTitlebarInactiveGradientEnd	= 142,
	vsThemeColorTitlebarInactiveText	= 143,
	vsThemeColorToolboxBackground	= 144,
	vsThemeColorToolboxDivider	= 145,
	vsThemeColorToolboxGradientDark	= 146,
	vsThemeColorToolboxGradientLight	= 147,
	vsThemeColorToolboxHeadingAccent	= 148,
	vsThemeColorToolboxHeadingBegin	= 149,
	vsThemeColorToolboxHeadingEnd	= 150,
	vsThemeColorToolboxIconHighlight	= 151,
	vsThemeColorToolboxIconShadow	= 152,
	vsThemeColorToolWindowBackground	= 153,
	vsThemeColorToolWindowBorder	= 154,
	vsThemeColorToolWindowTabSelectedtab	= 155,
	vsThemeColorToolWindowTabBorder	= 156,
	vsThemeColorToolWindowTabGradientBegin	= 157,
	vsThemeColorToolWindowTabGradientEnd	= 158,
	vsThemeColorToolWindowTabText	= 159,
	vsThemeColorToolWindowTabSelectedtext	= 160,
	vsThemeColorWizardOrientationPanelBackground	= 161,
	vsThemeColorWizardOrientationPanelText	= 162
    } 	vsThemeColors;

typedef /* [helpstringcontext][helpstring][helpcontext][uuid] */  DECLSPEC_UUID("A3ACF727-0590-4467-A9C7-5557B9C0F3F6") 
enum vsFindOptions2
    {	vsFindOptionsWaitForFindToComplete	= 0x4000
    } 	vsFindOptions2;

typedef /* [helpstringcontext][helpstring][helpcontext][uuid] */  DECLSPEC_UUID("FD6530D0-1A2C-4073-AAE9-3C2B3AA8BC4D") 
enum vsIncrementalSearchResult
    {	vsIncrementalSearchResultFound	= 1,
	vsIncrementalSearchResultPassedEOB	= 2,
	vsIncrementalSearchResultPassedStart	= 4,
	vsIncrementalSearchResultFailed	= 8
    } 	vsIncrementalSearchResult;

typedef /* [helpstring][uuid] */  DECLSPEC_UUID("861474e5-de58-4924-b6d3-8d48ba712944") 
enum vsPublishState
    {	vsPublishStateDone	= 1,
	vsPublishStateInProgress	= 2,
	vsPublishStateNotStarted	= 4
    } 	vsPublishState;

typedef /* [helpstring][uuid] */  DECLSPEC_UUID("e71fe63b-b6d9-47a0-9577-963c84aecce0") 
enum vsBuildErrorLevel
    {	vsBuildErrorLevelLow	= 1,
	vsBuildErrorLevelMedium	= 2,
	vsBuildErrorLevelHigh	= 4
    } 	vsBuildErrorLevel;


EXTERN_C const IID LIBID_EnvDTE80;

#ifndef __Commands2_INTERFACE_DEFINED__
#define __Commands2_INTERFACE_DEFINED__

/* interface Commands2 */
/* [helpstringcontext][helpstring][helpcontext][uuid][object][oleautomation][dual] */ 


EXTERN_C const IID IID_Commands2;

#if defined(__cplusplus) && !defined(CINTERFACE)
    
    MIDL_INTERFACE("7EAA857B-2356-494B-9E13-0F6EEFA86E43")
    Commands2 : public Commands
    {
    public:
        virtual /* [helpstringcontext][helpstring][helpcontext][id] */ HRESULT STDMETHODCALLTYPE AddNamedCommand2( 
            /* [in] */ __RPC__in /* external definition not present */ AddIn *AddInInstance,
            /* [in] */ __RPC__in BSTR Name,
            /* [in] */ __RPC__in BSTR ButtonText,
            /* [in] */ __RPC__in BSTR Tooltip,
            /* [in] */ VARIANT_BOOL MSOButton,
            /* [optional][in] */ VARIANT Bitmap,
            /* [optional][in] */ __RPC__deref_in_opt SAFEARRAY * *ContextUIGUIDs,
            /* [defaultvalue][in] */ long vsCommandStatusValue,
            /* [defaultvalue][in] */ long CommandStyleFlags,
            /* [defaultvalue][in] */ vsCommandControlType ControlType,
            /* [retval][out] */ __RPC__deref_out_opt /* external definition not present */ Command **pVal) = 0;
        
        virtual /* [helpstringcontext][helpstring][helpcontext][id] */ HRESULT STDMETHODCALLTYPE UpdateCommandUI( 
            VARIANT_BOOL PerformImmediately) = 0;
        
    };
    
#else 	/* C style interface */

    typedef struct Commands2Vtbl
    {
        BEGIN_INTERFACE
        
        /* [id][restricted][funcdescattr] */ HRESULT ( STDMETHODCALLTYPE *QueryInterface )( 
            Commands2 * This,
            /* [in][idldescattr] */ __RPC__in struct GUID *riid,
            /* [out][idldescattr] */ __RPC__deref_out_opt void **ppvObj,
            /* [retval][out] */ __RPC__out void *retval);
        
        /* [id][restricted][funcdescattr] */ HRESULT ( STDMETHODCALLTYPE *AddRef )( 
            Commands2 * This,
            /* [retval][out] */ __RPC__out unsigned long *retval);
        
        /* [id][restricted][funcdescattr] */ HRESULT ( STDMETHODCALLTYPE *Release )( 
            Commands2 * This,
            /* [retval][out] */ __RPC__out unsigned long *retval);
        
        /* [id][restricted][funcdescattr] */ HRESULT ( STDMETHODCALLTYPE *GetTypeInfoCount )( 
            Commands2 * This,
            /* [out][idldescattr] */ __RPC__out unsigned UINT *pctinfo,
            /* [retval][out] */ __RPC__out void *retval);
        
        /* [id][restricted][funcdescattr] */ HRESULT ( STDMETHODCALLTYPE *GetTypeInfo )( 
            Commands2 * This,
            /* [in][idldescattr] */ unsigned UINT itinfo,
            /* [in][idldescattr] */ unsigned long lcid,
            /* [out][idldescattr] */ __RPC__deref_out_opt void **pptinfo,
            /* [retval][out] */ __RPC__out void *retval);
        
        /* [id][restricted][funcdescattr] */ HRESULT ( STDMETHODCALLTYPE *GetIDsOfNames )( 
            Commands2 * This,
            /* [in][idldescattr] */ __RPC__in struct GUID *riid,
            /* [in][idldescattr] */ __RPC__deref_in_opt signed char **rgszNames,
            /* [in][idldescattr] */ unsigned UINT cNames,
            /* [in][idldescattr] */ unsigned long lcid,
            /* [out][idldescattr] */ __RPC__out signed long *rgdispid,
            /* [retval][out] */ __RPC__out void *retval);
        
        /* [id][restricted][funcdescattr] */ HRESULT ( STDMETHODCALLTYPE *Invoke )( 
            Commands2 * This,
            /* [in][idldescattr] */ signed long dispidMember,
            /* [in][idldescattr] */ __RPC__in struct GUID *riid,
            /* [in][idldescattr] */ unsigned long lcid,
            /* [in][idldescattr] */ unsigned short wFlags,
            /* [in][idldescattr] */ __RPC__in struct DISPPARAMS *pdispparams,
            /* [out][idldescattr] */ __RPC__out VARIANT *pvarResult,
            /* [out][idldescattr] */ __RPC__out struct EXCEPINFO *pexcepinfo,
            /* [out][idldescattr] */ __RPC__out unsigned UINT *puArgErr,
            /* [retval][out] */ __RPC__out void *retval);
        
        /* [id][propget][funcdescattr] */ HRESULT ( STDMETHODCALLTYPE *get_DTE )( 
            Commands2 * This,
            /* [retval][out] */ __RPC__deref_out_opt **retval);
        
        /* [id][propget][funcdescattr] */ HRESULT ( STDMETHODCALLTYPE *get_Parent )( 
            Commands2 * This,
            /* [retval][out] */ __RPC__deref_out_opt **retval);
        
        /* [id][hidden][funcdescattr] */ HRESULT ( STDMETHODCALLTYPE *Add )( 
            Commands2 * This,
            /* [in][idldescattr] */ __RPC__in BSTR Guid,
            /* [in][idldescattr] */ signed long ID,
            /* [in][idldescattr] */ __RPC__in VARIANT *Control,
            /* [retval][out] */ __RPC__out void *retval);
        
        /* [id][funcdescattr] */ HRESULT ( STDMETHODCALLTYPE *Raise )( 
            Commands2 * This,
            /* [in][idldescattr] */ __RPC__in BSTR Guid,
            /* [in][idldescattr] */ signed long ID,
            /* [out][in][idldescattr] */ __RPC__inout VARIANT *CustomIn,
            /* [out][in][idldescattr] */ __RPC__inout VARIANT *CustomOut,
            /* [retval][out] */ __RPC__out void *retval);
        
        /* [id][funcdescattr] */ HRESULT ( STDMETHODCALLTYPE *CommandInfo )( 
            Commands2 * This,
            /* [in][idldescattr] */ __RPC__in_opt IDispatch *CommandBarControl,
            /* [out][idldescattr] */ __RPC__deref_out_opt BSTR *Guid,
            /* [out][idldescattr] */ __RPC__out signed long *ID,
            /* [retval][out] */ __RPC__out void *retval);
        
        /* [id][propget][funcdescattr] */ HRESULT ( STDMETHODCALLTYPE *get_Count )( 
            Commands2 * This,
            /* [retval][out] */ __RPC__out signed long *retval);
        
        /* [id][funcdescattr] */ HRESULT ( STDMETHODCALLTYPE *Item )( 
            Commands2 * This,
            /* [in][idldescattr] */ VARIANT index,
            /* [in][idldescattr] */ signed long ID,
            /* [retval][out] */ __RPC__deref_out_opt Command **retval);
        
        /* [id][restricted][funcdescattr] */ HRESULT ( STDMETHODCALLTYPE *_NewEnum )( 
            Commands2 * This,
            /* [retval][out] */ __RPC__deref_out_opt IUnknown **retval);
        
        /* [id][funcdescattr] */ HRESULT ( STDMETHODCALLTYPE *AddNamedCommand )( 
            Commands2 * This,
            /* [in][idldescattr] */ __RPC__in_opt AddIn *AddInInstance,
            /* [in][idldescattr] */ __RPC__in BSTR Name,
            /* [in][idldescattr] */ __RPC__in BSTR ButtonText,
            /* [in][idldescattr] */ __RPC__in BSTR Tooltip,
            /* [in][idldescattr] */ BOOLEAN MSOButton,
            /* [in][idldescattr] */ signed long Bitmap,
            /* [in][idldescattr] */ __RPC__deref_in_opt SAFEARRAY * *ContextUIGUIDs,
            /* [in][idldescattr] */ signed long vsCommandDisabledFlagsValue,
            /* [retval][out] */ __RPC__deref_out_opt Command **retval);
        
        /* [id][funcdescattr] */ HRESULT ( STDMETHODCALLTYPE *AddCommandBar )( 
            Commands2 * This,
            /* [in][idldescattr] */ __RPC__in BSTR Name,
            /* [in][idldescattr] */ enum vsCommandBarType Type,
            /* [in][idldescattr] */ __RPC__in_opt IDispatch *CommandBarParent,
            /* [in][idldescattr] */ signed long Position,
            /* [retval][out] */ __RPC__deref_out_opt IDispatch **retval);
        
        /* [id][funcdescattr] */ HRESULT ( STDMETHODCALLTYPE *RemoveCommandBar )( 
            Commands2 * This,
            /* [in][idldescattr] */ __RPC__in_opt IDispatch *CommandBar,
            /* [retval][out] */ __RPC__out void *retval);
        
        /* [helpstringcontext][helpstring][helpcontext][id] */ HRESULT ( STDMETHODCALLTYPE *AddNamedCommand2 )( 
            Commands2 * This,
            /* [in] */ __RPC__in /* external definition not present */ AddIn *AddInInstance,
            /* [in] */ __RPC__in BSTR Name,
            /* [in] */ __RPC__in BSTR ButtonText,
            /* [in] */ __RPC__in BSTR Tooltip,
            /* [in] */ VARIANT_BOOL MSOButton,
            /* [optional][in] */ VARIANT Bitmap,
            /* [optional][in] */ __RPC__deref_in_opt SAFEARRAY * *ContextUIGUIDs,
            /* [defaultvalue][in] */ long vsCommandStatusValue,
            /* [defaultvalue][in] */ long CommandStyleFlags,
            /* [defaultvalue][in] */ vsCommandControlType ControlType,
            /* [retval][out] */ __RPC__deref_out_opt /* external definition not present */ Command **pVal);
        
        /* [helpstringcontext][helpstring][helpcontext][id] */ HRESULT ( STDMETHODCALLTYPE *UpdateCommandUI )( 
            Commands2 * This,
            VARIANT_BOOL PerformImmediately);
        
        END_INTERFACE
    } Commands2Vtbl;

    interface Commands2
    {
        CONST_VTBL struct Commands2Vtbl *lpVtbl;
    };

    

#ifdef COBJMACROS


#define Commands2_QueryInterface(This,riid,ppvObj,retval)	\
    ( (This)->lpVtbl -> QueryInterface(This,riid,ppvObj,retval) ) 

#define Commands2_AddRef(This,retval)	\
    ( (This)->lpVtbl -> AddRef(This,retval) ) 

#define Commands2_Release(This,retval)	\
    ( (This)->lpVtbl -> Release(This,retval) ) 

#define Commands2_GetTypeInfoCount(This,pctinfo,retval)	\
    ( (This)->lpVtbl -> GetTypeInfoCount(This,pctinfo,retval) ) 

#define Commands2_GetTypeInfo(This,itinfo,lcid,pptinfo,retval)	\
    ( (This)->lpVtbl -> GetTypeInfo(This,itinfo,lcid,pptinfo,retval) ) 

#define Commands2_GetIDsOfNames(This,riid,rgszNames,cNames,lcid,rgdispid,retval)	\
    ( (This)->lpVtbl -> GetIDsOfNames(This,riid,rgszNames,cNames,lcid,rgdispid,retval) ) 

#define Commands2_Invoke(This,dispidMember,riid,lcid,wFlags,pdispparams,pvarResult,pexcepinfo,puArgErr,retval)	\
    ( (This)->lpVtbl -> Invoke(This,dispidMember,riid,lcid,wFlags,pdispparams,pvarResult,pexcepinfo,puArgErr,retval) ) 

#define Commands2_get_DTE(This,retval)	\
    ( (This)->lpVtbl -> get_DTE(This,retval) ) 

#define Commands2_get_Parent(This,retval)	\
    ( (This)->lpVtbl -> get_Parent(This,retval) ) 

#define Commands2_Add(This,Guid,ID,Control,retval)	\
    ( (This)->lpVtbl -> Add(This,Guid,ID,Control,retval) ) 

#define Commands2_Raise(This,Guid,ID,CustomIn,CustomOut,retval)	\
    ( (This)->lpVtbl -> Raise(This,Guid,ID,CustomIn,CustomOut,retval) ) 

#define Commands2_CommandInfo(This,CommandBarControl,Guid,ID,retval)	\
    ( (This)->lpVtbl -> CommandInfo(This,CommandBarControl,Guid,ID,retval) ) 

#define Commands2_get_Count(This,retval)	\
    ( (This)->lpVtbl -> get_Count(This,retval) ) 

#define Commands2_Item(This,index,ID,retval)	\
    ( (This)->lpVtbl -> Item(This,index,ID,retval) ) 

#define Commands2__NewEnum(This,retval)	\
    ( (This)->lpVtbl -> _NewEnum(This,retval) ) 

#define Commands2_AddNamedCommand(This,AddInInstance,Name,ButtonText,Tooltip,MSOButton,Bitmap,ContextUIGUIDs,vsCommandDisabledFlagsValue,retval)	\
    ( (This)->lpVtbl -> AddNamedCommand(This,AddInInstance,Name,ButtonText,Tooltip,MSOButton,Bitmap,ContextUIGUIDs,vsCommandDisabledFlagsValue,retval) ) 

#define Commands2_AddCommandBar(This,Name,Type,CommandBarParent,Position,retval)	\
    ( (This)->lpVtbl -> AddCommandBar(This,Name,Type,CommandBarParent,Position,retval) ) 

#define Commands2_RemoveCommandBar(This,CommandBar,retval)	\
    ( (This)->lpVtbl -> RemoveCommandBar(This,CommandBar,retval) ) 


#define Commands2_AddNamedCommand2(This,AddInInstance,Name,ButtonText,Tooltip,MSOButton,Bitmap,ContextUIGUIDs,vsCommandStatusValue,CommandStyleFlags,ControlType,pVal)	\
    ( (This)->lpVtbl -> AddNamedCommand2(This,AddInInstance,Name,ButtonText,Tooltip,MSOButton,Bitmap,ContextUIGUIDs,vsCommandStatusValue,CommandStyleFlags,ControlType,pVal) ) 

#define Commands2_UpdateCommandUI(This,PerformImmediately)	\
    ( (This)->lpVtbl -> UpdateCommandUI(This,PerformImmediately) ) 

#endif /* COBJMACROS */


#endif 	/* C style interface */




#endif 	/* __Commands2_INTERFACE_DEFINED__ */


#ifndef __SourceControl2_INTERFACE_DEFINED__
#define __SourceControl2_INTERFACE_DEFINED__

/* interface SourceControl2 */
/* [helpstringcontext][helpstring][helpcontext][uuid][object][oleautomation][dual] */ 


EXTERN_C const IID IID_SourceControl2;

#if defined(__cplusplus) && !defined(CINTERFACE)
    
    MIDL_INTERFACE("111522ab-f439-4d85-9a36-1716da4da114")
    SourceControl2 : public SourceControl
    {
    public:
        virtual /* [helpstringcontext][helpstring][helpcontext][id] */ HRESULT STDMETHODCALLTYPE GetBindings( 
            /* [in] */ __RPC__in BSTR ItemPath,
            /* [retval][out] */ __RPC__deref_out_opt SourceControlBindings **ppVal) = 0;
        
        virtual /* [helpstringcontext][helpstring][helpcontext][id] */ HRESULT STDMETHODCALLTYPE CheckOutItem2( 
            /* [in] */ __RPC__in BSTR ItemName,
            /* [in] */ vsSourceControlCheckOutOptions Flags,
            /* [retval][out] */ __RPC__out VARIANT_BOOL *pfCheckedOut) = 0;
        
        virtual /* [helpstringcontext][helpstring][helpcontext][id] */ HRESULT STDMETHODCALLTYPE CheckOutItems2( 
            /* [in] */ __RPC__deref_in_opt SAFEARRAY * *ItemNames,
            /* [in] */ vsSourceControlCheckOutOptions Flags,
            /* [retval][out] */ __RPC__out VARIANT_BOOL *pfCheckedOut) = 0;
        
        virtual /* [helpstringcontext][helpstring][helpcontext][id] */ HRESULT STDMETHODCALLTYPE UndoExcludeItem( 
            /* [in] */ __RPC__in BSTR ProjectFile,
            /* [in] */ __RPC__in BSTR ItemName) = 0;
        
        virtual /* [helpstringcontext][helpstring][helpcontext][id] */ HRESULT STDMETHODCALLTYPE UndoExcludeItems( 
            /* [in] */ __RPC__in BSTR ProjectFile,
            /* [in] */ __RPC__deref_in_opt SAFEARRAY * *ItemNames) = 0;
        
    };
    
#else 	/* C style interface */

    typedef struct SourceControl2Vtbl
    {
        BEGIN_INTERFACE
        
        /* [id][restricted][funcdescattr] */ HRESULT ( STDMETHODCALLTYPE *QueryInterface )( 
            SourceControl2 * This,
            /* [in][idldescattr] */ __RPC__in struct GUID *riid,
            /* [out][idldescattr] */ __RPC__deref_out_opt void **ppvObj,
            /* [retval][out] */ __RPC__out void *retval);
        
        /* [id][restricted][funcdescattr] */ HRESULT ( STDMETHODCALLTYPE *AddRef )( 
            SourceControl2 * This,
            /* [retval][out] */ __RPC__out unsigned long *retval);
        
        /* [id][restricted][funcdescattr] */ HRESULT ( STDMETHODCALLTYPE *Release )( 
            SourceControl2 * This,
            /* [retval][out] */ __RPC__out unsigned long *retval);
        
        /* [id][restricted][funcdescattr] */ HRESULT ( STDMETHODCALLTYPE *GetTypeInfoCount )( 
            SourceControl2 * This,
            /* [out][idldescattr] */ __RPC__out unsigned UINT *pctinfo,
            /* [retval][out] */ __RPC__out void *retval);
        
        /* [id][restricted][funcdescattr] */ HRESULT ( STDMETHODCALLTYPE *GetTypeInfo )( 
            SourceControl2 * This,
            /* [in][idldescattr] */ unsigned UINT itinfo,
            /* [in][idldescattr] */ unsigned long lcid,
            /* [out][idldescattr] */ __RPC__deref_out_opt void **pptinfo,
            /* [retval][out] */ __RPC__out void *retval);
        
        /* [id][restricted][funcdescattr] */ HRESULT ( STDMETHODCALLTYPE *GetIDsOfNames )( 
            SourceControl2 * This,
            /* [in][idldescattr] */ __RPC__in struct GUID *riid,
            /* [in][idldescattr] */ __RPC__deref_in_opt signed char **rgszNames,
            /* [in][idldescattr] */ unsigned UINT cNames,
            /* [in][idldescattr] */ unsigned long lcid,
            /* [out][idldescattr] */ __RPC__out signed long *rgdispid,
            /* [retval][out] */ __RPC__out void *retval);
        
        /* [id][restricted][funcdescattr] */ HRESULT ( STDMETHODCALLTYPE *Invoke )( 
            SourceControl2 * This,
            /* [in][idldescattr] */ signed long dispidMember,
            /* [in][idldescattr] */ __RPC__in struct GUID *riid,
            /* [in][idldescattr] */ unsigned long lcid,
            /* [in][idldescattr] */ unsigned short wFlags,
            /* [in][idldescattr] */ __RPC__in struct DISPPARAMS *pdispparams,
            /* [out][idldescattr] */ __RPC__out VARIANT *pvarResult,
            /* [out][idldescattr] */ __RPC__out struct EXCEPINFO *pexcepinfo,
            /* [out][idldescattr] */ __RPC__out unsigned UINT *puArgErr,
            /* [retval][out] */ __RPC__out void *retval);
        
        /* [id][propget][funcdescattr] */ HRESULT ( STDMETHODCALLTYPE *get_DTE )( 
            SourceControl2 * This,
            /* [retval][out] */ __RPC__deref_out_opt **retval);
        
        /* [id][propget][funcdescattr] */ HRESULT ( STDMETHODCALLTYPE *get_Parent )( 
            SourceControl2 * This,
            /* [retval][out] */ __RPC__deref_out_opt **retval);
        
        /* [id][funcdescattr] */ HRESULT ( STDMETHODCALLTYPE *IsItemUnderSCC )( 
            SourceControl2 * This,
            /* [in][idldescattr] */ __RPC__in BSTR ItemName,
            /* [retval][out] */ __RPC__out BOOLEAN *retval);
        
        /* [id][funcdescattr] */ HRESULT ( STDMETHODCALLTYPE *IsItemCheckedOut )( 
            SourceControl2 * This,
            /* [in][idldescattr] */ __RPC__in BSTR ItemName,
            /* [retval][out] */ __RPC__out BOOLEAN *retval);
        
        /* [id][funcdescattr] */ HRESULT ( STDMETHODCALLTYPE *CheckOutItem )( 
            SourceControl2 * This,
            /* [in][idldescattr] */ __RPC__in BSTR ItemName,
            /* [retval][out] */ __RPC__out BOOLEAN *retval);
        
        /* [id][funcdescattr] */ HRESULT ( STDMETHODCALLTYPE *CheckOutItems )( 
            SourceControl2 * This,
            /* [in][idldescattr] */ __RPC__deref_in_opt SAFEARRAY * *ItemNames,
            /* [retval][out] */ __RPC__out BOOLEAN *retval);
        
        /* [id][funcdescattr] */ HRESULT ( STDMETHODCALLTYPE *ExcludeItem )( 
            SourceControl2 * This,
            /* [in][idldescattr] */ __RPC__in BSTR ProjectFile,
            /* [in][idldescattr] */ __RPC__in BSTR ItemName,
            /* [retval][out] */ __RPC__out void *retval);
        
        /* [id][funcdescattr] */ HRESULT ( STDMETHODCALLTYPE *ExcludeItems )( 
            SourceControl2 * This,
            /* [in][idldescattr] */ __RPC__in BSTR ProjectFile,
            /* [in][idldescattr] */ __RPC__deref_in_opt SAFEARRAY * *ItemNames,
            /* [retval][out] */ __RPC__out void *retval);
        
        /* [helpstringcontext][helpstring][helpcontext][id] */ HRESULT ( STDMETHODCALLTYPE *GetBindings )( 
            SourceControl2 * This,
            /* [in] */ __RPC__in BSTR ItemPath,
            /* [retval][out] */ __RPC__deref_out_opt SourceControlBindings **ppVal);
        
        /* [helpstringcontext][helpstring][helpcontext][id] */ HRESULT ( STDMETHODCALLTYPE *CheckOutItem2 )( 
            SourceControl2 * This,
            /* [in] */ __RPC__in BSTR ItemName,
            /* [in] */ vsSourceControlCheckOutOptions Flags,
            /* [retval][out] */ __RPC__out VARIANT_BOOL *pfCheckedOut);
        
        /* [helpstringcontext][helpstring][helpcontext][id] */ HRESULT ( STDMETHODCALLTYPE *CheckOutItems2 )( 
            SourceControl2 * This,
            /* [in] */ __RPC__deref_in_opt SAFEARRAY * *ItemNames,
            /* [in] */ vsSourceControlCheckOutOptions Flags,
            /* [retval][out] */ __RPC__out VARIANT_BOOL *pfCheckedOut);
        
        /* [helpstringcontext][helpstring][helpcontext][id] */ HRESULT ( STDMETHODCALLTYPE *UndoExcludeItem )( 
            SourceControl2 * This,
            /* [in] */ __RPC__in BSTR ProjectFile,
            /* [in] */ __RPC__in BSTR ItemName);
        
        /* [helpstringcontext][helpstring][helpcontext][id] */ HRESULT ( STDMETHODCALLTYPE *UndoExcludeItems )( 
            SourceControl2 * This,
            /* [in] */ __RPC__in BSTR ProjectFile,
            /* [in] */ __RPC__deref_in_opt SAFEARRAY * *ItemNames);
        
        END_INTERFACE
    } SourceControl2Vtbl;

    interface SourceControl2
    {
        CONST_VTBL struct SourceControl2Vtbl *lpVtbl;
    };

    

#ifdef COBJMACROS


#define SourceControl2_QueryInterface(This,riid,ppvObj,retval)	\
    ( (This)->lpVtbl -> QueryInterface(This,riid,ppvObj,retval) ) 

#define SourceControl2_AddRef(This,retval)	\
    ( (This)->lpVtbl -> AddRef(This,retval) ) 

#define SourceControl2_Release(This,retval)	\
    ( (This)->lpVtbl -> Release(This,retval) ) 

#define SourceControl2_GetTypeInfoCount(This,pctinfo,retval)	\
    ( (This)->lpVtbl -> GetTypeInfoCount(This,pctinfo,retval) ) 

#define SourceControl2_GetTypeInfo(This,itinfo,lcid,pptinfo,retval)	\
    ( (This)->lpVtbl -> GetTypeInfo(This,itinfo,lcid,pptinfo,retval) ) 

#define SourceControl2_GetIDsOfNames(This,riid,rgszNames,cNames,lcid,rgdispid,retval)	\
    ( (This)->lpVtbl -> GetIDsOfNames(This,riid,rgszNames,cNames,lcid,rgdispid,retval) ) 

#define SourceControl2_Invoke(This,dispidMember,riid,lcid,wFlags,pdispparams,pvarResult,pexcepinfo,puArgErr,retval)	\
    ( (This)->lpVtbl -> Invoke(This,dispidMember,riid,lcid,wFlags,pdispparams,pvarResult,pexcepinfo,puArgErr,retval) ) 

#define SourceControl2_get_DTE(This,retval)	\
    ( (This)->lpVtbl -> get_DTE(This,retval) ) 

#define SourceControl2_get_Parent(This,retval)	\
    ( (This)->lpVtbl -> get_Parent(This,retval) ) 

#define SourceControl2_IsItemUnderSCC(This,ItemName,retval)	\
    ( (This)->lpVtbl -> IsItemUnderSCC(This,ItemName,retval) ) 

#define SourceControl2_IsItemCheckedOut(This,ItemName,retval)	\
    ( (This)->lpVtbl -> IsItemCheckedOut(This,ItemName,retval) ) 

#define SourceControl2_CheckOutItem(This,ItemName,retval)	\
    ( (This)->lpVtbl -> CheckOutItem(This,ItemName,retval) ) 

#define SourceControl2_CheckOutItems(This,ItemNames,retval)	\
    ( (This)->lpVtbl -> CheckOutItems(This,ItemNames,retval) ) 

#define SourceControl2_ExcludeItem(This,ProjectFile,ItemName,retval)	\
    ( (This)->lpVtbl -> ExcludeItem(This,ProjectFile,ItemName,retval) ) 

#define SourceControl2_ExcludeItems(This,ProjectFile,ItemNames,retval)	\
    ( (This)->lpVtbl -> ExcludeItems(This,ProjectFile,ItemNames,retval) ) 


#define SourceControl2_GetBindings(This,ItemPath,ppVal)	\
    ( (This)->lpVtbl -> GetBindings(This,ItemPath,ppVal) ) 

#define SourceControl2_CheckOutItem2(This,ItemName,Flags,pfCheckedOut)	\
    ( (This)->lpVtbl -> CheckOutItem2(This,ItemName,Flags,pfCheckedOut) ) 

#define SourceControl2_CheckOutItems2(This,ItemNames,Flags,pfCheckedOut)	\
    ( (This)->lpVtbl -> CheckOutItems2(This,ItemNames,Flags,pfCheckedOut) ) 

#define SourceControl2_UndoExcludeItem(This,ProjectFile,ItemName)	\
    ( (This)->lpVtbl -> UndoExcludeItem(This,ProjectFile,ItemName) ) 

#define SourceControl2_UndoExcludeItems(This,ProjectFile,ItemNames)	\
    ( (This)->lpVtbl -> UndoExcludeItems(This,ProjectFile,ItemNames) ) 

#endif /* COBJMACROS */


#endif 	/* C style interface */




#endif 	/* __SourceControl2_INTERFACE_DEFINED__ */


#ifndef __ErrorItem_INTERFACE_DEFINED__
#define __ErrorItem_INTERFACE_DEFINED__

/* interface ErrorItem */
/* [helpstring][uuid][object][oleautomation][dual] */ 


EXTERN_C const IID IID_ErrorItem;

#if defined(__cplusplus) && !defined(CINTERFACE)
    
    MIDL_INTERFACE("f49a191f-7258-493b-9310-5f7771ddf3d7")
    ErrorItem : public IDispatch
    {
    public:
        virtual /* [helpstringcontext][helpstring][helpcontext][propget][id] */ HRESULT STDMETHODCALLTYPE get_DTE( 
            /* [retval][out] */ __RPC__deref_out_opt /* external definition not present */ DTE **DTEObject) = 0;
        
        virtual /* [helpstringcontext][helpstring][helpcontext][propget][id] */ HRESULT STDMETHODCALLTYPE get_Collection( 
            /* [retval][out] */ __RPC__deref_out_opt ErrorItems **ErrorItems) = 0;
        
        virtual /* [helpstring][propget][id] */ HRESULT STDMETHODCALLTYPE get_ErrorLevel( 
            /* [retval][out] */ __RPC__out vsBuildErrorLevel *ErrorLevel) = 0;
        
        virtual /* [helpstringcontext][helpstring][helpcontext][propget][id] */ HRESULT STDMETHODCALLTYPE get_Description( 
            /* [retval][out] */ __RPC__deref_out_opt BSTR *Description) = 0;
        
        virtual /* [helpstring][propget][id] */ HRESULT STDMETHODCALLTYPE get_FileName( 
            /* [retval][out] */ __RPC__deref_out_opt BSTR *File) = 0;
        
        virtual /* [helpstring][propget][id] */ HRESULT STDMETHODCALLTYPE get_Line( 
            /* [retval][out] */ __RPC__out long *Line) = 0;
        
        virtual /* [helpstring][propget][id] */ HRESULT STDMETHODCALLTYPE get_Column( 
            /* [retval][out] */ __RPC__out long *Column) = 0;
        
        virtual /* [helpstring][propget][id] */ HRESULT STDMETHODCALLTYPE get_Project( 
            /* [retval][out] */ __RPC__deref_out_opt BSTR *ProjectUniqueName) = 0;
        
        virtual /* [helpstring][id] */ HRESULT STDMETHODCALLTYPE Navigate( void) = 0;
        
    };
    
#else 	/* C style interface */

    typedef struct ErrorItemVtbl
    {
        BEGIN_INTERFACE
        
        HRESULT ( STDMETHODCALLTYPE *QueryInterface )( 
            ErrorItem * This,
            /* [in] */ __RPC__in REFIID riid,
            /* [iid_is][out] */ 
            __RPC__deref_out  void **ppvObject);
        
        ULONG ( STDMETHODCALLTYPE *AddRef )( 
            ErrorItem * This);
        
        ULONG ( STDMETHODCALLTYPE *Release )( 
            ErrorItem * This);
        
        HRESULT ( STDMETHODCALLTYPE *GetTypeInfoCount )( 
            ErrorItem * This,
            /* [out] */ __RPC__out UINT *pctinfo);
        
        HRESULT ( STDMETHODCALLTYPE *GetTypeInfo )( 
            ErrorItem * This,
            /* [in] */ UINT iTInfo,
            /* [in] */ LCID lcid,
            /* [out] */ __RPC__deref_out_opt ITypeInfo **ppTInfo);
        
        HRESULT ( STDMETHODCALLTYPE *GetIDsOfNames )( 
            ErrorItem * This,
            /* [in] */ __RPC__in REFIID riid,
            /* [size_is][in] */ __RPC__in_ecount_full(cNames) LPOLESTR *rgszNames,
            /* [range][in] */ UINT cNames,
            /* [in] */ LCID lcid,
            /* [size_is][out] */ __RPC__out_ecount_full(cNames) DISPID *rgDispId);
        
        /* [local] */ HRESULT ( STDMETHODCALLTYPE *Invoke )( 
            ErrorItem * This,
            /* [in] */ DISPID dispIdMember,
            /* [in] */ REFIID riid,
            /* [in] */ LCID lcid,
            /* [in] */ WORD wFlags,
            /* [out][in] */ DISPPARAMS *pDispParams,
            /* [out] */ VARIANT *pVarResult,
            /* [out] */ EXCEPINFO *pExcepInfo,
            /* [out] */ UINT *puArgErr);
        
        /* [helpstringcontext][helpstring][helpcontext][propget][id] */ HRESULT ( STDMETHODCALLTYPE *get_DTE )( 
            ErrorItem * This,
            /* [retval][out] */ __RPC__deref_out_opt /* external definition not present */ DTE **DTEObject);
        
        /* [helpstringcontext][helpstring][helpcontext][propget][id] */ HRESULT ( STDMETHODCALLTYPE *get_Collection )( 
            ErrorItem * This,
            /* [retval][out] */ __RPC__deref_out_opt ErrorItems **ErrorItems);
        
        /* [helpstring][propget][id] */ HRESULT ( STDMETHODCALLTYPE *get_ErrorLevel )( 
            ErrorItem * This,
            /* [retval][out] */ __RPC__out vsBuildErrorLevel *ErrorLevel);
        
        /* [helpstringcontext][helpstring][helpcontext][propget][id] */ HRESULT ( STDMETHODCALLTYPE *get_Description )( 
            ErrorItem * This,
            /* [retval][out] */ __RPC__deref_out_opt BSTR *Description);
        
        /* [helpstring][propget][id] */ HRESULT ( STDMETHODCALLTYPE *get_FileName )( 
            ErrorItem * This,
            /* [retval][out] */ __RPC__deref_out_opt BSTR *File);
        
        /* [helpstring][propget][id] */ HRESULT ( STDMETHODCALLTYPE *get_Line )( 
            ErrorItem * This,
            /* [retval][out] */ __RPC__out long *Line);
        
        /* [helpstring][propget][id] */ HRESULT ( STDMETHODCALLTYPE *get_Column )( 
            ErrorItem * This,
            /* [retval][out] */ __RPC__out long *Column);
        
        /* [helpstring][propget][id] */ HRESULT ( STDMETHODCALLTYPE *get_Project )( 
            ErrorItem * This,
            /* [retval][out] */ __RPC__deref_out_opt BSTR *ProjectUniqueName);
        
        /* [helpstring][id] */ HRESULT ( STDMETHODCALLTYPE *Navigate )( 
            ErrorItem * This);
        
        END_INTERFACE
    } ErrorItemVtbl;

    interface ErrorItem
    {
        CONST_VTBL struct ErrorItemVtbl *lpVtbl;
    };

    

#ifdef COBJMACROS


#define ErrorItem_QueryInterface(This,riid,ppvObject)	\
    ( (This)->lpVtbl -> QueryInterface(This,riid,ppvObject) ) 

#define ErrorItem_AddRef(This)	\
    ( (This)->lpVtbl -> AddRef(This) ) 

#define ErrorItem_Release(This)	\
    ( (This)->lpVtbl -> Release(This) ) 


#define ErrorItem_GetTypeInfoCount(This,pctinfo)	\
    ( (This)->lpVtbl -> GetTypeInfoCount(This,pctinfo) ) 

#define ErrorItem_GetTypeInfo(This,iTInfo,lcid,ppTInfo)	\
    ( (This)->lpVtbl -> GetTypeInfo(This,iTInfo,lcid,ppTInfo) ) 

#define ErrorItem_GetIDsOfNames(This,riid,rgszNames,cNames,lcid,rgDispId)	\
    ( (This)->lpVtbl -> GetIDsOfNames(This,riid,rgszNames,cNames,lcid,rgDispId) ) 

#define ErrorItem_Invoke(This,dispIdMember,riid,lcid,wFlags,pDispParams,pVarResult,pExcepInfo,puArgErr)	\
    ( (This)->lpVtbl -> Invoke(This,dispIdMember,riid,lcid,wFlags,pDispParams,pVarResult,pExcepInfo,puArgErr) ) 


#define ErrorItem_get_DTE(This,DTEObject)	\
    ( (This)->lpVtbl -> get_DTE(This,DTEObject) ) 

#define ErrorItem_get_Collection(This,ErrorItems)	\
    ( (This)->lpVtbl -> get_Collection(This,ErrorItems) ) 

#define ErrorItem_get_ErrorLevel(This,ErrorLevel)	\
    ( (This)->lpVtbl -> get_ErrorLevel(This,ErrorLevel) ) 

#define ErrorItem_get_Description(This,Description)	\
    ( (This)->lpVtbl -> get_Description(This,Description) ) 

#define ErrorItem_get_FileName(This,File)	\
    ( (This)->lpVtbl -> get_FileName(This,File) ) 

#define ErrorItem_get_Line(This,Line)	\
    ( (This)->lpVtbl -> get_Line(This,Line) ) 

#define ErrorItem_get_Column(This,Column)	\
    ( (This)->lpVtbl -> get_Column(This,Column) ) 

#define ErrorItem_get_Project(This,ProjectUniqueName)	\
    ( (This)->lpVtbl -> get_Project(This,ProjectUniqueName) ) 

#define ErrorItem_Navigate(This)	\
    ( (This)->lpVtbl -> Navigate(This) ) 

#endif /* COBJMACROS */


#endif 	/* C style interface */




#endif 	/* __ErrorItem_INTERFACE_DEFINED__ */


#ifndef __ErrorList_INTERFACE_DEFINED__
#define __ErrorList_INTERFACE_DEFINED__

/* interface ErrorList */
/* [helpstring][uuid][object][oleautomation][dual] */ 


EXTERN_C const IID IID_ErrorList;

#if defined(__cplusplus) && !defined(CINTERFACE)
    
    MIDL_INTERFACE("7bf84ef1-8246-498a-a127-2ea37824fda1")
    ErrorList : public IDispatch
    {
    public:
        virtual /* [helpstringcontext][helpstring][helpcontext][propget][id] */ HRESULT STDMETHODCALLTYPE get_DTE( 
            /* [retval][out] */ __RPC__deref_out_opt /* external definition not present */ DTE **ppDTEObject) = 0;
        
        virtual /* [helpstringcontext][helpstring][helpcontext][propget][id] */ HRESULT STDMETHODCALLTYPE get_Parent( 
            /* [retval][out] */ __RPC__deref_out_opt /* external definition not present */ Window **ppWindow) = 0;
        
        virtual /* [helpstring][propget][id] */ HRESULT STDMETHODCALLTYPE get_ShowErrors( 
            /* [retval][out] */ __RPC__out VARIANT_BOOL *pShowErrors) = 0;
        
        virtual /* [helpstring][propput][id] */ HRESULT STDMETHODCALLTYPE put_ShowErrors( 
            VARIANT_BOOL ShowErrors) = 0;
        
        virtual /* [helpstring][propget][id] */ HRESULT STDMETHODCALLTYPE get_ShowWarnings( 
            /* [retval][out] */ __RPC__out VARIANT_BOOL *pShowWarnings) = 0;
        
        virtual /* [helpstring][propput][id] */ HRESULT STDMETHODCALLTYPE put_ShowWarnings( 
            VARIANT_BOOL ShowWarnings) = 0;
        
        virtual /* [helpstring][propget][id] */ HRESULT STDMETHODCALLTYPE get_ShowMessages( 
            /* [retval][out] */ __RPC__out VARIANT_BOOL *pShowMessages) = 0;
        
        virtual /* [helpstring][propput][id] */ HRESULT STDMETHODCALLTYPE put_ShowMessages( 
            VARIANT_BOOL ShowMessages) = 0;
        
        virtual /* [helpstring][propget][id] */ HRESULT STDMETHODCALLTYPE get_ErrorItems( 
            /* [retval][out] */ __RPC__deref_out_opt ErrorItems **ppErrorItems) = 0;
        
        virtual /* [helpstringcontext][helpstring][helpcontext][propget][id] */ HRESULT STDMETHODCALLTYPE get_SelectedItems( 
            /* [retval][out] */ __RPC__out VARIANT *Selections) = 0;
        
    };
    
#else 	/* C style interface */

    typedef struct ErrorListVtbl
    {
        BEGIN_INTERFACE
        
        HRESULT ( STDMETHODCALLTYPE *QueryInterface )( 
            ErrorList * This,
            /* [in] */ __RPC__in REFIID riid,
            /* [iid_is][out] */ 
            __RPC__deref_out  void **ppvObject);
        
        ULONG ( STDMETHODCALLTYPE *AddRef )( 
            ErrorList * This);
        
        ULONG ( STDMETHODCALLTYPE *Release )( 
            ErrorList * This);
        
        HRESULT ( STDMETHODCALLTYPE *GetTypeInfoCount )( 
            ErrorList * This,
            /* [out] */ __RPC__out UINT *pctinfo);
        
        HRESULT ( STDMETHODCALLTYPE *GetTypeInfo )( 
            ErrorList * This,
            /* [in] */ UINT iTInfo,
            /* [in] */ LCID lcid,
            /* [out] */ __RPC__deref_out_opt ITypeInfo **ppTInfo);
        
        HRESULT ( STDMETHODCALLTYPE *GetIDsOfNames )( 
            ErrorList * This,
            /* [in] */ __RPC__in REFIID riid,
            /* [size_is][in] */ __RPC__in_ecount_full(cNames) LPOLESTR *rgszNames,
            /* [range][in] */ UINT cNames,
            /* [in] */ LCID lcid,
            /* [size_is][out] */ __RPC__out_ecount_full(cNames) DISPID *rgDispId);
        
        /* [local] */ HRESULT ( STDMETHODCALLTYPE *Invoke )( 
            ErrorList * This,
            /* [in] */ DISPID dispIdMember,
            /* [in] */ REFIID riid,
            /* [in] */ LCID lcid,
            /* [in] */ WORD wFlags,
            /* [out][in] */ DISPPARAMS *pDispParams,
            /* [out] */ VARIANT *pVarResult,
            /* [out] */ EXCEPINFO *pExcepInfo,
            /* [out] */ UINT *puArgErr);
        
        /* [helpstringcontext][helpstring][helpcontext][propget][id] */ HRESULT ( STDMETHODCALLTYPE *get_DTE )( 
            ErrorList * This,
            /* [retval][out] */ __RPC__deref_out_opt /* external definition not present */ DTE **ppDTEObject);
        
        /* [helpstringcontext][helpstring][helpcontext][propget][id] */ HRESULT ( STDMETHODCALLTYPE *get_Parent )( 
            ErrorList * This,
            /* [retval][out] */ __RPC__deref_out_opt /* external definition not present */ Window **ppWindow);
        
        /* [helpstring][propget][id] */ HRESULT ( STDMETHODCALLTYPE *get_ShowErrors )( 
            ErrorList * This,
            /* [retval][out] */ __RPC__out VARIANT_BOOL *pShowErrors);
        
        /* [helpstring][propput][id] */ HRESULT ( STDMETHODCALLTYPE *put_ShowErrors )( 
            ErrorList * This,
            VARIANT_BOOL ShowErrors);
        
        /* [helpstring][propget][id] */ HRESULT ( STDMETHODCALLTYPE *get_ShowWarnings )( 
            ErrorList * This,
            /* [retval][out] */ __RPC__out VARIANT_BOOL *pShowWarnings);
        
        /* [helpstring][propput][id] */ HRESULT ( STDMETHODCALLTYPE *put_ShowWarnings )( 
            ErrorList * This,
            VARIANT_BOOL ShowWarnings);
        
        /* [helpstring][propget][id] */ HRESULT ( STDMETHODCALLTYPE *get_ShowMessages )( 
            ErrorList * This,
            /* [retval][out] */ __RPC__out VARIANT_BOOL *pShowMessages);
        
        /* [helpstring][propput][id] */ HRESULT ( STDMETHODCALLTYPE *put_ShowMessages )( 
            ErrorList * This,
            VARIANT_BOOL ShowMessages);
        
        /* [helpstring][propget][id] */ HRESULT ( STDMETHODCALLTYPE *get_ErrorItems )( 
            ErrorList * This,
            /* [retval][out] */ __RPC__deref_out_opt ErrorItems **ppErrorItems);
        
        /* [helpstringcontext][helpstring][helpcontext][propget][id] */ HRESULT ( STDMETHODCALLTYPE *get_SelectedItems )( 
            ErrorList * This,
            /* [retval][out] */ __RPC__out VARIANT *Selections);
        
        END_INTERFACE
    } ErrorListVtbl;

    interface ErrorList
    {
        CONST_VTBL struct ErrorListVtbl *lpVtbl;
    };

    

#ifdef COBJMACROS


#define ErrorList_QueryInterface(This,riid,ppvObject)	\
    ( (This)->lpVtbl -> QueryInterface(This,riid,ppvObject) ) 

#define ErrorList_AddRef(This)	\
    ( (This)->lpVtbl -> AddRef(This) ) 

#define ErrorList_Release(This)	\
    ( (This)->lpVtbl -> Release(This) ) 


#define ErrorList_GetTypeInfoCount(This,pctinfo)	\
    ( (This)->lpVtbl -> GetTypeInfoCount(This,pctinfo) ) 

#define ErrorList_GetTypeInfo(This,iTInfo,lcid,ppTInfo)	\
    ( (This)->lpVtbl -> GetTypeInfo(This,iTInfo,lcid,ppTInfo) ) 

#define ErrorList_GetIDsOfNames(This,riid,rgszNames,cNames,lcid,rgDispId)	\
    ( (This)->lpVtbl -> GetIDsOfNames(This,riid,rgszNames,cNames,lcid,rgDispId) ) 

#define ErrorList_Invoke(This,dispIdMember,riid,lcid,wFlags,pDispParams,pVarResult,pExcepInfo,puArgErr)	\
    ( (This)->lpVtbl -> Invoke(This,dispIdMember,riid,lcid,wFlags,pDispParams,pVarResult,pExcepInfo,puArgErr) ) 


#define ErrorList_get_DTE(This,ppDTEObject)	\
    ( (This)->lpVtbl -> get_DTE(This,ppDTEObject) ) 

#define ErrorList_get_Parent(This,ppWindow)	\
    ( (This)->lpVtbl -> get_Parent(This,ppWindow) ) 

#define ErrorList_get_ShowErrors(This,pShowErrors)	\
    ( (This)->lpVtbl -> get_ShowErrors(This,pShowErrors) ) 

#define ErrorList_put_ShowErrors(This,ShowErrors)	\
    ( (This)->lpVtbl -> put_ShowErrors(This,ShowErrors) ) 

#define ErrorList_get_ShowWarnings(This,pShowWarnings)	\
    ( (This)->lpVtbl -> get_ShowWarnings(This,pShowWarnings) ) 

#define ErrorList_put_ShowWarnings(This,ShowWarnings)	\
    ( (This)->lpVtbl -> put_ShowWarnings(This,ShowWarnings) ) 

#define ErrorList_get_ShowMessages(This,pShowMessages)	\
    ( (This)->lpVtbl -> get_ShowMessages(This,pShowMessages) ) 

#define ErrorList_put_ShowMessages(This,ShowMessages)	\
    ( (This)->lpVtbl -> put_ShowMessages(This,ShowMessages) ) 

#define ErrorList_get_ErrorItems(This,ppErrorItems)	\
    ( (This)->lpVtbl -> get_ErrorItems(This,ppErrorItems) ) 

#define ErrorList_get_SelectedItems(This,Selections)	\
    ( (This)->lpVtbl -> get_SelectedItems(This,Selections) ) 

#endif /* COBJMACROS */


#endif 	/* C style interface */




#endif 	/* __ErrorList_INTERFACE_DEFINED__ */


#ifndef __CodeAttributeArgument_INTERFACE_DEFINED__
#define __CodeAttributeArgument_INTERFACE_DEFINED__

/* interface CodeAttributeArgument */
/* [helpstringcontext][helpstring][helpcontext][uuid][object][oleautomation][dual] */ 


EXTERN_C const IID IID_CodeAttributeArgument;

#if defined(__cplusplus) && !defined(CINTERFACE)
    
    MIDL_INTERFACE("80F4779B-835D-4873-8356-2F34A759A514")
    CodeAttributeArgument : public IDispatch
    {
    public:
        virtual /* [nonbrowsable][helpstringcontext][helpstring][helpcontext][propget][id] */ HRESULT STDMETHODCALLTYPE get_DTE( 
            /* [retval][out] */ __RPC__deref_out_opt /* external definition not present */ DTE **DTEObject) = 0;
        
        virtual /* [nonbrowsable][helpstringcontext][helpstring][helpcontext][propget][id] */ HRESULT STDMETHODCALLTYPE get_Collection( 
            /* [retval][out] */ __RPC__deref_out_opt /* external definition not present */ CodeElements **ppCollection) = 0;
        
        virtual /* [helpstringcontext][helpstring][helpcontext][propget][id] */ HRESULT STDMETHODCALLTYPE get_Name( 
            /* [retval][out] */ __RPC__deref_out_opt BSTR *pVal) = 0;
        
        virtual /* [helpstringcontext][helpstring][helpcontext][propput][id] */ HRESULT STDMETHODCALLTYPE put_Name( 
            __RPC__in BSTR NewName) = 0;
        
        virtual /* [helpstringcontext][helpstring][helpcontext][propget][id] */ HRESULT STDMETHODCALLTYPE get_FullName( 
            /* [retval][out] */ __RPC__deref_out_opt BSTR *pVal) = 0;
        
        virtual /* [nonbrowsable][helpstringcontext][helpstring][helpcontext][propget][id] */ HRESULT STDMETHODCALLTYPE get_ProjectItem( 
            /* [retval][out] */ __RPC__deref_out_opt /* external definition not present */ ProjectItem **pProjItem) = 0;
        
        virtual /* [nonbrowsable][helpstringcontext][helpstring][helpcontext][propget][id] */ HRESULT STDMETHODCALLTYPE get_Kind( 
            /* [retval][out] */ __RPC__out enum /* external definition not present */ vsCMElement *pCodeEltKind) = 0;
        
        virtual /* [nonbrowsable][helpstringcontext][helpstring][helpcontext][propget][id] */ HRESULT STDMETHODCALLTYPE get_IsCodeType( 
            /* [retval][out] */ __RPC__out VARIANT_BOOL *pIsCodeType) = 0;
        
        virtual /* [nonbrowsable][helpstringcontext][helpstring][helpcontext][propget][id] */ HRESULT STDMETHODCALLTYPE get_InfoLocation( 
            /* [retval][out] */ __RPC__out enum /* external definition not present */ vsCMInfoLocation *pInfoLocation) = 0;
        
        virtual /* [nonbrowsable][helpstringcontext][helpstring][helpcontext][propget][id] */ HRESULT STDMETHODCALLTYPE get_Children( 
            /* [retval][out] */ __RPC__deref_out_opt /* external definition not present */ CodeElements **ppCodeElements) = 0;
        
        virtual /* [nonbrowsable][helpstringcontext][helpstring][helpcontext][propget][id] */ HRESULT STDMETHODCALLTYPE get_Language( 
            /* [retval][out] */ __RPC__deref_out_opt BSTR *pLanguage) = 0;
        
        virtual /* [nonbrowsable][helpstringcontext][helpstring][helpcontext][propget][id] */ HRESULT STDMETHODCALLTYPE get_StartPoint( 
            /* [retval][out] */ __RPC__deref_out_opt /* external definition not present */ TextPoint **ppTextPoint) = 0;
        
        virtual /* [nonbrowsable][helpstringcontext][helpstring][helpcontext][propget][id] */ HRESULT STDMETHODCALLTYPE get_EndPoint( 
            /* [retval][out] */ __RPC__deref_out_opt /* external definition not present */ TextPoint **ppTextPoint) = 0;
        
        virtual /* [helpstringcontext][helpstring][helpcontext][nonbrowsable][propget][id] */ HRESULT STDMETHODCALLTYPE get_ExtenderNames( 
            /* [retval][out] */ __RPC__out VARIANT *ExtenderNames) = 0;
        
        virtual /* [helpstringcontext][helpstring][helpcontext][nonbrowsable][propget][id] */ HRESULT STDMETHODCALLTYPE get_Extender( 
            __RPC__in BSTR ExtenderName,
            /* [retval][out] */ __RPC__deref_out_opt IDispatch **Extender) = 0;
        
        virtual /* [helpstringcontext][helpstring][helpcontext][nonbrowsable][propget][id] */ HRESULT STDMETHODCALLTYPE get_ExtenderCATID( 
            /* [retval][out] */ __RPC__deref_out_opt BSTR *pRetval) = 0;
        
        virtual /* [nonbrowsable][helpstringcontext][helpstring][helpcontext][id] */ HRESULT STDMETHODCALLTYPE GetStartPoint( 
            /* [defaultvalue][in] */ enum /* external definition not present */ vsCMPart Part,
            /* [retval][out] */ __RPC__deref_out_opt /* external definition not present */ TextPoint **ppTextPoint) = 0;
        
        virtual /* [nonbrowsable][helpstringcontext][helpstring][helpcontext][id] */ HRESULT STDMETHODCALLTYPE GetEndPoint( 
            /* [defaultvalue][in] */ enum /* external definition not present */ vsCMPart Part,
            /* [retval][out] */ __RPC__deref_out_opt /* external definition not present */ TextPoint **ppTextPoint) = 0;
        
        virtual /* [helpstringcontext][helpstring][helpcontext][propget][id] */ HRESULT STDMETHODCALLTYPE get_Value( 
            /* [retval][out] */ __RPC__deref_out_opt BSTR *value) = 0;
        
        virtual /* [helpstringcontext][helpstring][helpcontext][propput][id] */ HRESULT STDMETHODCALLTYPE put_Value( 
            __RPC__in BSTR value) = 0;
        
        virtual /* [helpstring][id] */ HRESULT STDMETHODCALLTYPE Delete( void) = 0;
        
    };
    
#else 	/* C style interface */

    typedef struct CodeAttributeArgumentVtbl
    {
        BEGIN_INTERFACE
        
        HRESULT ( STDMETHODCALLTYPE *QueryInterface )( 
            CodeAttributeArgument * This,
            /* [in] */ __RPC__in REFIID riid,
            /* [iid_is][out] */ 
            __RPC__deref_out  void **ppvObject);
        
        ULONG ( STDMETHODCALLTYPE *AddRef )( 
            CodeAttributeArgument * This);
        
        ULONG ( STDMETHODCALLTYPE *Release )( 
            CodeAttributeArgument * This);
        
        HRESULT ( STDMETHODCALLTYPE *GetTypeInfoCount )( 
            CodeAttributeArgument * This,
            /* [out] */ __RPC__out UINT *pctinfo);
        
        HRESULT ( STDMETHODCALLTYPE *GetTypeInfo )( 
            CodeAttributeArgument * This,
            /* [in] */ UINT iTInfo,
            /* [in] */ LCID lcid,
            /* [out] */ __RPC__deref_out_opt ITypeInfo **ppTInfo);
        
        HRESULT ( STDMETHODCALLTYPE *GetIDsOfNames )( 
            CodeAttributeArgument * This,
            /* [in] */ __RPC__in REFIID riid,
            /* [size_is][in] */ __RPC__in_ecount_full(cNames) LPOLESTR *rgszNames,
            /* [range][in] */ UINT cNames,
            /* [in] */ LCID lcid,
            /* [size_is][out] */ __RPC__out_ecount_full(cNames) DISPID *rgDispId);
        
        /* [local] */ HRESULT ( STDMETHODCALLTYPE *Invoke )( 
            CodeAttributeArgument * This,
            /* [in] */ DISPID dispIdMember,
            /* [in] */ REFIID riid,
            /* [in] */ LCID lcid,
            /* [in] */ WORD wFlags,
            /* [out][in] */ DISPPARAMS *pDispParams,
            /* [out] */ VARIANT *pVarResult,
            /* [out] */ EXCEPINFO *pExcepInfo,
            /* [out] */ UINT *puArgErr);
        
        /* [nonbrowsable][helpstringcontext][helpstring][helpcontext][propget][id] */ HRESULT ( STDMETHODCALLTYPE *get_DTE )( 
            CodeAttributeArgument * This,
            /* [retval][out] */ __RPC__deref_out_opt /* external definition not present */ DTE **DTEObject);
        
        /* [nonbrowsable][helpstringcontext][helpstring][helpcontext][propget][id] */ HRESULT ( STDMETHODCALLTYPE *get_Collection )( 
            CodeAttributeArgument * This,
            /* [retval][out] */ __RPC__deref_out_opt /* external definition not present */ CodeElements **ppCollection);
        
        /* [helpstringcontext][helpstring][helpcontext][propget][id] */ HRESULT ( STDMETHODCALLTYPE *get_Name )( 
            CodeAttributeArgument * This,
            /* [retval][out] */ __RPC__deref_out_opt BSTR *pVal);
        
        /* [helpstringcontext][helpstring][helpcontext][propput][id] */ HRESULT ( STDMETHODCALLTYPE *put_Name )( 
            CodeAttributeArgument * This,
            __RPC__in BSTR NewName);
        
        /* [helpstringcontext][helpstring][helpcontext][propget][id] */ HRESULT ( STDMETHODCALLTYPE *get_FullName )( 
            CodeAttributeArgument * This,
            /* [retval][out] */ __RPC__deref_out_opt BSTR *pVal);
        
        /* [nonbrowsable][helpstringcontext][helpstring][helpcontext][propget][id] */ HRESULT ( STDMETHODCALLTYPE *get_ProjectItem )( 
            CodeAttributeArgument * This,
            /* [retval][out] */ __RPC__deref_out_opt /* external definition not present */ ProjectItem **pProjItem);
        
        /* [nonbrowsable][helpstringcontext][helpstring][helpcontext][propget][id] */ HRESULT ( STDMETHODCALLTYPE *get_Kind )( 
            CodeAttributeArgument * This,
            /* [retval][out] */ __RPC__out enum /* external definition not present */ vsCMElement *pCodeEltKind);
        
        /* [nonbrowsable][helpstringcontext][helpstring][helpcontext][propget][id] */ HRESULT ( STDMETHODCALLTYPE *get_IsCodeType )( 
            CodeAttributeArgument * This,
            /* [retval][out] */ __RPC__out VARIANT_BOOL *pIsCodeType);
        
        /* [nonbrowsable][helpstringcontext][helpstring][helpcontext][propget][id] */ HRESULT ( STDMETHODCALLTYPE *get_InfoLocation )( 
            CodeAttributeArgument * This,
            /* [retval][out] */ __RPC__out enum /* external definition not present */ vsCMInfoLocation *pInfoLocation);
        
        /* [nonbrowsable][helpstringcontext][helpstring][helpcontext][propget][id] */ HRESULT ( STDMETHODCALLTYPE *get_Children )( 
            CodeAttributeArgument * This,
            /* [retval][out] */ __RPC__deref_out_opt /* external definition not present */ CodeElements **ppCodeElements);
        
        /* [nonbrowsable][helpstringcontext][helpstring][helpcontext][propget][id] */ HRESULT ( STDMETHODCALLTYPE *get_Language )( 
            CodeAttributeArgument * This,
            /* [retval][out] */ __RPC__deref_out_opt BSTR *pLanguage);
        
        /* [nonbrowsable][helpstringcontext][helpstring][helpcontext][propget][id] */ HRESULT ( STDMETHODCALLTYPE *get_StartPoint )( 
            CodeAttributeArgument * This,
            /* [retval][out] */ __RPC__deref_out_opt /* external definition not present */ TextPoint **ppTextPoint);
        
        /* [nonbrowsable][helpstringcontext][helpstring][helpcontext][propget][id] */ HRESULT ( STDMETHODCALLTYPE *get_EndPoint )( 
            CodeAttributeArgument * This,
            /* [retval][out] */ __RPC__deref_out_opt /* external definition not present */ TextPoint **ppTextPoint);
        
        /* [helpstringcontext][helpstring][helpcontext][nonbrowsable][propget][id] */ HRESULT ( STDMETHODCALLTYPE *get_ExtenderNames )( 
            CodeAttributeArgument * This,
            /* [retval][out] */ __RPC__out VARIANT *ExtenderNames);
        
        /* [helpstringcontext][helpstring][helpcontext][nonbrowsable][propget][id] */ HRESULT ( STDMETHODCALLTYPE *get_Extender )( 
            CodeAttributeArgument * This,
            __RPC__in BSTR ExtenderName,
            /* [retval][out] */ __RPC__deref_out_opt IDispatch **Extender);
        
        /* [helpstringcontext][helpstring][helpcontext][nonbrowsable][propget][id] */ HRESULT ( STDMETHODCALLTYPE *get_ExtenderCATID )( 
            CodeAttributeArgument * This,
            /* [retval][out] */ __RPC__deref_out_opt BSTR *pRetval);
        
        /* [nonbrowsable][helpstringcontext][helpstring][helpcontext][id] */ HRESULT ( STDMETHODCALLTYPE *GetStartPoint )( 
            CodeAttributeArgument * This,
            /* [defaultvalue][in] */ enum /* external definition not present */ vsCMPart Part,
            /* [retval][out] */ __RPC__deref_out_opt /* external definition not present */ TextPoint **ppTextPoint);
        
        /* [nonbrowsable][helpstringcontext][helpstring][helpcontext][id] */ HRESULT ( STDMETHODCALLTYPE *GetEndPoint )( 
            CodeAttributeArgument * This,
            /* [defaultvalue][in] */ enum /* external definition not present */ vsCMPart Part,
            /* [retval][out] */ __RPC__deref_out_opt /* external definition not present */ TextPoint **ppTextPoint);
        
        /* [helpstringcontext][helpstring][helpcontext][propget][id] */ HRESULT ( STDMETHODCALLTYPE *get_Value )( 
            CodeAttributeArgument * This,
            /* [retval][out] */ __RPC__deref_out_opt BSTR *value);
        
        /* [helpstringcontext][helpstring][helpcontext][propput][id] */ HRESULT ( STDMETHODCALLTYPE *put_Value )( 
            CodeAttributeArgument * This,
            __RPC__in BSTR value);
        
        /* [helpstring][id] */ HRESULT ( STDMETHODCALLTYPE *Delete )( 
            CodeAttributeArgument * This);
        
        END_INTERFACE
    } CodeAttributeArgumentVtbl;

    interface CodeAttributeArgument
    {
        CONST_VTBL struct CodeAttributeArgumentVtbl *lpVtbl;
    };

    

#ifdef COBJMACROS


#define CodeAttributeArgument_QueryInterface(This,riid,ppvObject)	\
    ( (This)->lpVtbl -> QueryInterface(This,riid,ppvObject) ) 

#define CodeAttributeArgument_AddRef(This)	\
    ( (This)->lpVtbl -> AddRef(This) ) 

#define CodeAttributeArgument_Release(This)	\
    ( (This)->lpVtbl -> Release(This) ) 


#define CodeAttributeArgument_GetTypeInfoCount(This,pctinfo)	\
    ( (This)->lpVtbl -> GetTypeInfoCount(This,pctinfo) ) 

#define CodeAttributeArgument_GetTypeInfo(This,iTInfo,lcid,ppTInfo)	\
    ( (This)->lpVtbl -> GetTypeInfo(This,iTInfo,lcid,ppTInfo) ) 

#define CodeAttributeArgument_GetIDsOfNames(This,riid,rgszNames,cNames,lcid,rgDispId)	\
    ( (This)->lpVtbl -> GetIDsOfNames(This,riid,rgszNames,cNames,lcid,rgDispId) ) 

#define CodeAttributeArgument_Invoke(This,dispIdMember,riid,lcid,wFlags,pDispParams,pVarResult,pExcepInfo,puArgErr)	\
    ( (This)->lpVtbl -> Invoke(This,dispIdMember,riid,lcid,wFlags,pDispParams,pVarResult,pExcepInfo,puArgErr) ) 


#define CodeAttributeArgument_get_DTE(This,DTEObject)	\
    ( (This)->lpVtbl -> get_DTE(This,DTEObject) ) 

#define CodeAttributeArgument_get_Collection(This,ppCollection)	\
    ( (This)->lpVtbl -> get_Collection(This,ppCollection) ) 

#define CodeAttributeArgument_get_Name(This,pVal)	\
    ( (This)->lpVtbl -> get_Name(This,pVal) ) 

#define CodeAttributeArgument_put_Name(This,NewName)	\
    ( (This)->lpVtbl -> put_Name(This,NewName) ) 

#define CodeAttributeArgument_get_FullName(This,pVal)	\
    ( (This)->lpVtbl -> get_FullName(This,pVal) ) 

#define CodeAttributeArgument_get_ProjectItem(This,pProjItem)	\
    ( (This)->lpVtbl -> get_ProjectItem(This,pProjItem) ) 

#define CodeAttributeArgument_get_Kind(This,pCodeEltKind)	\
    ( (This)->lpVtbl -> get_Kind(This,pCodeEltKind) ) 

#define CodeAttributeArgument_get_IsCodeType(This,pIsCodeType)	\
    ( (This)->lpVtbl -> get_IsCodeType(This,pIsCodeType) ) 

#define CodeAttributeArgument_get_InfoLocation(This,pInfoLocation)	\
    ( (This)->lpVtbl -> get_InfoLocation(This,pInfoLocation) ) 

#define CodeAttributeArgument_get_Children(This,ppCodeElements)	\
    ( (This)->lpVtbl -> get_Children(This,ppCodeElements) ) 

#define CodeAttributeArgument_get_Language(This,pLanguage)	\
    ( (This)->lpVtbl -> get_Language(This,pLanguage) ) 

#define CodeAttributeArgument_get_StartPoint(This,ppTextPoint)	\
    ( (This)->lpVtbl -> get_StartPoint(This,ppTextPoint) ) 

#define CodeAttributeArgument_get_EndPoint(This,ppTextPoint)	\
    ( (This)->lpVtbl -> get_EndPoint(This,ppTextPoint) ) 

#define CodeAttributeArgument_get_ExtenderNames(This,ExtenderNames)	\
    ( (This)->lpVtbl -> get_ExtenderNames(This,ExtenderNames) ) 

#define CodeAttributeArgument_get_Extender(This,ExtenderName,Extender)	\
    ( (This)->lpVtbl -> get_Extender(This,ExtenderName,Extender) ) 

#define CodeAttributeArgument_get_ExtenderCATID(This,pRetval)	\
    ( (This)->lpVtbl -> get_ExtenderCATID(This,pRetval) ) 

#define CodeAttributeArgument_GetStartPoint(This,Part,ppTextPoint)	\
    ( (This)->lpVtbl -> GetStartPoint(This,Part,ppTextPoint) ) 

#define CodeAttributeArgument_GetEndPoint(This,Part,ppTextPoint)	\
    ( (This)->lpVtbl -> GetEndPoint(This,Part,ppTextPoint) ) 

#define CodeAttributeArgument_get_Value(This,value)	\
    ( (This)->lpVtbl -> get_Value(This,value) ) 

#define CodeAttributeArgument_put_Value(This,value)	\
    ( (This)->lpVtbl -> put_Value(This,value) ) 

#define CodeAttributeArgument_Delete(This)	\
    ( (This)->lpVtbl -> Delete(This) ) 

#endif /* COBJMACROS */


#endif 	/* C style interface */




#endif 	/* __CodeAttributeArgument_INTERFACE_DEFINED__ */



#ifndef __CodeModelLanguageConstants2_MODULE_DEFINED__
#define __CodeModelLanguageConstants2_MODULE_DEFINED__


/* module CodeModelLanguageConstants2 */
/* [dllname][uuid] */ 

const LPSTR vsCMLanguageJSharp	=	"{E6FDF8BF-F3D1-11D4-8576-0002A516ECE8}";

#endif /* __CodeModelLanguageConstants2_MODULE_DEFINED__ */

#ifndef __CodeEvent_INTERFACE_DEFINED__
#define __CodeEvent_INTERFACE_DEFINED__

/* interface CodeEvent */
/* [helpstring][uuid][object][oleautomation][dual] */ 


EXTERN_C const IID IID_CodeEvent;

#if defined(__cplusplus) && !defined(CINTERFACE)
    
    MIDL_INTERFACE("ce615bd2-8ed5-4f0c-a7b6-4a299d8801fd")
    CodeEvent : public IDispatch
    {
    public:
        virtual /* [nonbrowsable][helpstringcontext][helpstring][helpcontext][propget][id] */ HRESULT STDMETHODCALLTYPE get_DTE( 
            /* [retval][out] */ __RPC__deref_out_opt /* external definition not present */ DTE **DTEObject) = 0;
        
        virtual /* [nonbrowsable][helpstringcontext][helpstring][helpcontext][propget][id] */ HRESULT STDMETHODCALLTYPE get_Collection( 
            /* [retval][out] */ __RPC__deref_out_opt /* external definition not present */ CodeElements **ppCollection) = 0;
        
        virtual /* [helpstringcontext][helpstring][helpcontext][propget][id] */ HRESULT STDMETHODCALLTYPE get_Name( 
            /* [retval][out] */ __RPC__deref_out_opt BSTR *pVal) = 0;
        
        virtual /* [helpstringcontext][helpstring][helpcontext][propput][id] */ HRESULT STDMETHODCALLTYPE put_Name( 
            __RPC__in BSTR NewName) = 0;
        
        virtual /* [helpstringcontext][helpstring][helpcontext][propget][id] */ HRESULT STDMETHODCALLTYPE get_FullName( 
            /* [retval][out] */ __RPC__deref_out_opt BSTR *pVal) = 0;
        
        virtual /* [nonbrowsable][helpstringcontext][helpstring][helpcontext][propget][id] */ HRESULT STDMETHODCALLTYPE get_ProjectItem( 
            /* [retval][out] */ __RPC__deref_out_opt /* external definition not present */ ProjectItem **pProjItem) = 0;
        
        virtual /* [nonbrowsable][helpstringcontext][helpstring][helpcontext][propget][id] */ HRESULT STDMETHODCALLTYPE get_Kind( 
            /* [retval][out] */ __RPC__out enum /* external definition not present */ vsCMElement *pCodeEltKind) = 0;
        
        virtual /* [nonbrowsable][helpstringcontext][helpstring][helpcontext][propget][id] */ HRESULT STDMETHODCALLTYPE get_IsCodeType( 
            /* [retval][out] */ __RPC__out VARIANT_BOOL *pIsCodeType) = 0;
        
        virtual /* [nonbrowsable][helpstringcontext][helpstring][helpcontext][propget][id] */ HRESULT STDMETHODCALLTYPE get_InfoLocation( 
            /* [retval][out] */ __RPC__out enum /* external definition not present */ vsCMInfoLocation *pInfoLocation) = 0;
        
        virtual /* [nonbrowsable][helpstringcontext][helpstring][helpcontext][propget][id] */ HRESULT STDMETHODCALLTYPE get_Children( 
            /* [retval][out] */ __RPC__deref_out_opt /* external definition not present */ CodeElements **ppCodeElements) = 0;
        
        virtual /* [nonbrowsable][helpstringcontext][helpstring][helpcontext][propget][id] */ HRESULT STDMETHODCALLTYPE get_Language( 
            /* [retval][out] */ __RPC__deref_out_opt BSTR *pLanguage) = 0;
        
        virtual /* [nonbrowsable][helpstringcontext][helpstring][helpcontext][propget][id] */ HRESULT STDMETHODCALLTYPE get_StartPoint( 
            /* [retval][out] */ __RPC__deref_out_opt /* external definition not present */ TextPoint **ppTextPoint) = 0;
        
        virtual /* [nonbrowsable][helpstringcontext][helpstring][helpcontext][propget][id] */ HRESULT STDMETHODCALLTYPE get_EndPoint( 
            /* [retval][out] */ __RPC__deref_out_opt /* external definition not present */ TextPoint **ppTextPoint) = 0;
        
        virtual /* [helpstringcontext][helpstring][helpcontext][nonbrowsable][propget][id] */ HRESULT STDMETHODCALLTYPE get_ExtenderNames( 
            /* [retval][out] */ __RPC__out VARIANT *ExtenderNames) = 0;
        
        virtual /* [helpstringcontext][helpstring][helpcontext][nonbrowsable][propget][id] */ HRESULT STDMETHODCALLTYPE get_Extender( 
            __RPC__in BSTR ExtenderName,
            /* [retval][out] */ __RPC__deref_out_opt IDispatch **Extender) = 0;
        
        virtual /* [helpstringcontext][helpstring][helpcontext][nonbrowsable][propget][id] */ HRESULT STDMETHODCALLTYPE get_ExtenderCATID( 
            /* [retval][out] */ __RPC__deref_out_opt BSTR *pRetval) = 0;
        
        virtual /* [nonbrowsable][helpstringcontext][helpstring][helpcontext][id] */ HRESULT STDMETHODCALLTYPE GetStartPoint( 
            /* [defaultvalue][in] */ enum /* external definition not present */ vsCMPart Part,
            /* [retval][out] */ __RPC__deref_out_opt /* external definition not present */ TextPoint **ppTextPoint) = 0;
        
        virtual /* [nonbrowsable][helpstringcontext][helpstring][helpcontext][id] */ HRESULT STDMETHODCALLTYPE GetEndPoint( 
            /* [defaultvalue][in] */ enum /* external definition not present */ vsCMPart Part,
            /* [retval][out] */ __RPC__deref_out_opt /* external definition not present */ TextPoint **ppTextPoint) = 0;
        
        virtual /* [helpstringcontext][helpstring][helpcontext][propget][id] */ HRESULT STDMETHODCALLTYPE get_Parent( 
            /* [retval][out] */ __RPC__deref_out_opt IDispatch **ParentObject) = 0;
        
        virtual /* [helpstringcontext][helpstring][helpcontext][propput][id] */ HRESULT STDMETHODCALLTYPE put_Access( 
            enum /* external definition not present */ vsCMAccess Access) = 0;
        
        virtual /* [helpstringcontext][helpstring][helpcontext][propget][id] */ HRESULT STDMETHODCALLTYPE get_Access( 
            /* [retval][out] */ __RPC__out enum /* external definition not present */ vsCMAccess *Access) = 0;
        
        virtual /* [helpstringcontext][helpstring][helpcontext][propget][id] */ HRESULT STDMETHODCALLTYPE get_Attributes( 
            /* [retval][out] */ __RPC__deref_out_opt /* external definition not present */ CodeElements **ppMembers) = 0;
        
        virtual /* [helpstringcontext][helpstring][helpcontext][propget][id] */ HRESULT STDMETHODCALLTYPE get_DocComment( 
            /* [retval][out] */ __RPC__deref_out_opt BSTR *pDocComment) = 0;
        
        virtual /* [helpstringcontext][helpstring][helpcontext][propput][id] */ HRESULT STDMETHODCALLTYPE put_DocComment( 
            __RPC__in BSTR DocComment) = 0;
        
        virtual /* [helpstringcontext][helpstring][helpcontext][propget][id] */ HRESULT STDMETHODCALLTYPE get_Comment( 
            /* [retval][out] */ __RPC__deref_out_opt BSTR *pComment) = 0;
        
        virtual /* [helpstringcontext][helpstring][helpcontext][propput][id] */ HRESULT STDMETHODCALLTYPE put_Comment( 
            __RPC__in BSTR Comment) = 0;
        
        virtual /* [helpstringcontext][helpstring][helpcontext][id] */ HRESULT STDMETHODCALLTYPE AddAttribute( 
            __RPC__in BSTR Name,
            __RPC__in BSTR Value,
            /* [optional] */ VARIANT Position,
            /* [retval][out] */ __RPC__deref_out_opt /* external definition not present */ CodeAttribute **ppCodeAttribute) = 0;
        
        virtual /* [helpstring][propget][id] */ HRESULT STDMETHODCALLTYPE get_Adder( 
            /* [retval][out] */ __RPC__deref_out_opt /* external definition not present */ CodeFunction **ppCodeFunction) = 0;
        
        virtual /* [helpstring][propput][id] */ HRESULT STDMETHODCALLTYPE put_Adder( 
            __RPC__in /* external definition not present */ CodeFunction *codeFunction) = 0;
        
        virtual /* [helpstring][propget][id] */ HRESULT STDMETHODCALLTYPE get_Remover( 
            /* [retval][out] */ __RPC__deref_out_opt /* external definition not present */ CodeFunction **ppCodeFunction) = 0;
        
        virtual /* [helpstring][propput][id] */ HRESULT STDMETHODCALLTYPE put_Remover( 
            __RPC__in /* external definition not present */ CodeFunction *codeFunction) = 0;
        
        virtual /* [helpstring][propget][id] */ HRESULT STDMETHODCALLTYPE get_Thrower( 
            /* [retval][out] */ __RPC__deref_out_opt /* external definition not present */ CodeFunction **ppCodeFunction) = 0;
        
        virtual /* [helpstring][propput][id] */ HRESULT STDMETHODCALLTYPE put_Thrower( 
            __RPC__in /* external definition not present */ CodeFunction *codeFunction) = 0;
        
        virtual /* [helpstring][propget][id] */ HRESULT STDMETHODCALLTYPE get_IsPropertyStyleEvent( 
            /* [retval][out] */ __RPC__out VARIANT_BOOL *vbIsPropertyStyle) = 0;
        
        virtual /* [nonbrowsable][helpstringcontext][helpstring][helpcontext][propget][id] */ HRESULT STDMETHODCALLTYPE get_Prototype( 
            /* [defaultvalue][in] */ long Flags,
            /* [retval][out] */ __RPC__deref_out_opt BSTR *pFullName) = 0;
        
        virtual /* [nonbrowsable][helpstringcontext][helpstring][helpcontext][propget][id] */ HRESULT STDMETHODCALLTYPE get_Type( 
            /* [retval][out] */ __RPC__deref_out_opt /* external definition not present */ CodeTypeRef **ppCodeTypeRef) = 0;
        
        virtual /* [nonbrowsable][helpstringcontext][helpstring][helpcontext][propput][id] */ HRESULT STDMETHODCALLTYPE put_Type( 
            __RPC__in /* external definition not present */ CodeTypeRef *pCodeTypeRef) = 0;
        
        virtual /* [helpstringcontext][helpstring][helpcontext][propget][id] */ HRESULT STDMETHODCALLTYPE get_OverrideKind( 
            /* [retval][out] */ __RPC__out vsCMOverrideKind *Kind) = 0;
        
        virtual /* [helpstringcontext][helpstring][helpcontext][propput][id] */ HRESULT STDMETHODCALLTYPE put_OverrideKind( 
            vsCMOverrideKind Kind) = 0;
        
        virtual /* [helpstringcontext][helpstring][helpcontext][propget][id] */ HRESULT STDMETHODCALLTYPE get_IsShared( 
            /* [retval][out] */ __RPC__out VARIANT_BOOL *pIsShared) = 0;
        
        virtual /* [helpstringcontext][helpstring][helpcontext][propput][id] */ HRESULT STDMETHODCALLTYPE put_IsShared( 
            VARIANT_BOOL Shared) = 0;
        
    };
    
#else 	/* C style interface */

    typedef struct CodeEventVtbl
    {
        BEGIN_INTERFACE
        
        HRESULT ( STDMETHODCALLTYPE *QueryInterface )( 
            CodeEvent * This,
            /* [in] */ __RPC__in REFIID riid,
            /* [iid_is][out] */ 
            __RPC__deref_out  void **ppvObject);
        
        ULONG ( STDMETHODCALLTYPE *AddRef )( 
            CodeEvent * This);
        
        ULONG ( STDMETHODCALLTYPE *Release )( 
            CodeEvent * This);
        
        HRESULT ( STDMETHODCALLTYPE *GetTypeInfoCount )( 
            CodeEvent * This,
            /* [out] */ __RPC__out UINT *pctinfo);
        
        HRESULT ( STDMETHODCALLTYPE *GetTypeInfo )( 
            CodeEvent * This,
            /* [in] */ UINT iTInfo,
            /* [in] */ LCID lcid,
            /* [out] */ __RPC__deref_out_opt ITypeInfo **ppTInfo);
        
        HRESULT ( STDMETHODCALLTYPE *GetIDsOfNames )( 
            CodeEvent * This,
            /* [in] */ __RPC__in REFIID riid,
            /* [size_is][in] */ __RPC__in_ecount_full(cNames) LPOLESTR *rgszNames,
            /* [range][in] */ UINT cNames,
            /* [in] */ LCID lcid,
            /* [size_is][out] */ __RPC__out_ecount_full(cNames) DISPID *rgDispId);
        
        /* [local] */ HRESULT ( STDMETHODCALLTYPE *Invoke )( 
            CodeEvent * This,
            /* [in] */ DISPID dispIdMember,
            /* [in] */ REFIID riid,
            /* [in] */ LCID lcid,
            /* [in] */ WORD wFlags,
            /* [out][in] */ DISPPARAMS *pDispParams,
            /* [out] */ VARIANT *pVarResult,
            /* [out] */ EXCEPINFO *pExcepInfo,
            /* [out] */ UINT *puArgErr);
        
        /* [nonbrowsable][helpstringcontext][helpstring][helpcontext][propget][id] */ HRESULT ( STDMETHODCALLTYPE *get_DTE )( 
            CodeEvent * This,
            /* [retval][out] */ __RPC__deref_out_opt /* external definition not present */ DTE **DTEObject);
        
        /* [nonbrowsable][helpstringcontext][helpstring][helpcontext][propget][id] */ HRESULT ( STDMETHODCALLTYPE *get_Collection )( 
            CodeEvent * This,
            /* [retval][out] */ __RPC__deref_out_opt /* external definition not present */ CodeElements **ppCollection);
        
        /* [helpstringcontext][helpstring][helpcontext][propget][id] */ HRESULT ( STDMETHODCALLTYPE *get_Name )( 
            CodeEvent * This,
            /* [retval][out] */ __RPC__deref_out_opt BSTR *pVal);
        
        /* [helpstringcontext][helpstring][helpcontext][propput][id] */ HRESULT ( STDMETHODCALLTYPE *put_Name )( 
            CodeEvent * This,
            __RPC__in BSTR NewName);
        
        /* [helpstringcontext][helpstring][helpcontext][propget][id] */ HRESULT ( STDMETHODCALLTYPE *get_FullName )( 
            CodeEvent * This,
            /* [retval][out] */ __RPC__deref_out_opt BSTR *pVal);
        
        /* [nonbrowsable][helpstringcontext][helpstring][helpcontext][propget][id] */ HRESULT ( STDMETHODCALLTYPE *get_ProjectItem )( 
            CodeEvent * This,
            /* [retval][out] */ __RPC__deref_out_opt /* external definition not present */ ProjectItem **pProjItem);
        
        /* [nonbrowsable][helpstringcontext][helpstring][helpcontext][propget][id] */ HRESULT ( STDMETHODCALLTYPE *get_Kind )( 
            CodeEvent * This,
            /* [retval][out] */ __RPC__out enum /* external definition not present */ vsCMElement *pCodeEltKind);
        
        /* [nonbrowsable][helpstringcontext][helpstring][helpcontext][propget][id] */ HRESULT ( STDMETHODCALLTYPE *get_IsCodeType )( 
            CodeEvent * This,
            /* [retval][out] */ __RPC__out VARIANT_BOOL *pIsCodeType);
        
        /* [nonbrowsable][helpstringcontext][helpstring][helpcontext][propget][id] */ HRESULT ( STDMETHODCALLTYPE *get_InfoLocation )( 
            CodeEvent * This,
            /* [retval][out] */ __RPC__out enum /* external definition not present */ vsCMInfoLocation *pInfoLocation);
        
        /* [nonbrowsable][helpstringcontext][helpstring][helpcontext][propget][id] */ HRESULT ( STDMETHODCALLTYPE *get_Children )( 
            CodeEvent * This,
            /* [retval][out] */ __RPC__deref_out_opt /* external definition not present */ CodeElements **ppCodeElements);
        
        /* [nonbrowsable][helpstringcontext][helpstring][helpcontext][propget][id] */ HRESULT ( STDMETHODCALLTYPE *get_Language )( 
            CodeEvent * This,
            /* [retval][out] */ __RPC__deref_out_opt BSTR *pLanguage);
        
        /* [nonbrowsable][helpstringcontext][helpstring][helpcontext][propget][id] */ HRESULT ( STDMETHODCALLTYPE *get_StartPoint )( 
            CodeEvent * This,
            /* [retval][out] */ __RPC__deref_out_opt /* external definition not present */ TextPoint **ppTextPoint);
        
        /* [nonbrowsable][helpstringcontext][helpstring][helpcontext][propget][id] */ HRESULT ( STDMETHODCALLTYPE *get_EndPoint )( 
            CodeEvent * This,
            /* [retval][out] */ __RPC__deref_out_opt /* external definition not present */ TextPoint **ppTextPoint);
        
        /* [helpstringcontext][helpstring][helpcontext][nonbrowsable][propget][id] */ HRESULT ( STDMETHODCALLTYPE *get_ExtenderNames )( 
            CodeEvent * This,
            /* [retval][out] */ __RPC__out VARIANT *ExtenderNames);
        
        /* [helpstringcontext][helpstring][helpcontext][nonbrowsable][propget][id] */ HRESULT ( STDMETHODCALLTYPE *get_Extender )( 
            CodeEvent * This,
            __RPC__in BSTR ExtenderName,
            /* [retval][out] */ __RPC__deref_out_opt IDispatch **Extender);
        
        /* [helpstringcontext][helpstring][helpcontext][nonbrowsable][propget][id] */ HRESULT ( STDMETHODCALLTYPE *get_ExtenderCATID )( 
            CodeEvent * This,
            /* [retval][out] */ __RPC__deref_out_opt BSTR *pRetval);
        
        /* [nonbrowsable][helpstringcontext][helpstring][helpcontext][id] */ HRESULT ( STDMETHODCALLTYPE *GetStartPoint )( 
            CodeEvent * This,
            /* [defaultvalue][in] */ enum /* external definition not present */ vsCMPart Part,
            /* [retval][out] */ __RPC__deref_out_opt /* external definition not present */ TextPoint **ppTextPoint);
        
        /* [nonbrowsable][helpstringcontext][helpstring][helpcontext][id] */ HRESULT ( STDMETHODCALLTYPE *GetEndPoint )( 
            CodeEvent * This,
            /* [defaultvalue][in] */ enum /* external definition not present */ vsCMPart Part,
            /* [retval][out] */ __RPC__deref_out_opt /* external definition not present */ TextPoint **ppTextPoint);
        
        /* [helpstringcontext][helpstring][helpcontext][propget][id] */ HRESULT ( STDMETHODCALLTYPE *get_Parent )( 
            CodeEvent * This,
            /* [retval][out] */ __RPC__deref_out_opt IDispatch **ParentObject);
        
        /* [helpstringcontext][helpstring][helpcontext][propput][id] */ HRESULT ( STDMETHODCALLTYPE *put_Access )( 
            CodeEvent * This,
            enum /* external definition not present */ vsCMAccess Access);
        
        /* [helpstringcontext][helpstring][helpcontext][propget][id] */ HRESULT ( STDMETHODCALLTYPE *get_Access )( 
            CodeEvent * This,
            /* [retval][out] */ __RPC__out enum /* external definition not present */ vsCMAccess *Access);
        
        /* [helpstringcontext][helpstring][helpcontext][propget][id] */ HRESULT ( STDMETHODCALLTYPE *get_Attributes )( 
            CodeEvent * This,
            /* [retval][out] */ __RPC__deref_out_opt /* external definition not present */ CodeElements **ppMembers);
        
        /* [helpstringcontext][helpstring][helpcontext][propget][id] */ HRESULT ( STDMETHODCALLTYPE *get_DocComment )( 
            CodeEvent * This,
            /* [retval][out] */ __RPC__deref_out_opt BSTR *pDocComment);
        
        /* [helpstringcontext][helpstring][helpcontext][propput][id] */ HRESULT ( STDMETHODCALLTYPE *put_DocComment )( 
            CodeEvent * This,
            __RPC__in BSTR DocComment);
        
        /* [helpstringcontext][helpstring][helpcontext][propget][id] */ HRESULT ( STDMETHODCALLTYPE *get_Comment )( 
            CodeEvent * This,
            /* [retval][out] */ __RPC__deref_out_opt BSTR *pComment);
        
        /* [helpstringcontext][helpstring][helpcontext][propput][id] */ HRESULT ( STDMETHODCALLTYPE *put_Comment )( 
            CodeEvent * This,
            __RPC__in BSTR Comment);
        
        /* [helpstringcontext][helpstring][helpcontext][id] */ HRESULT ( STDMETHODCALLTYPE *AddAttribute )( 
            CodeEvent * This,
            __RPC__in BSTR Name,
            __RPC__in BSTR Value,
            /* [optional] */ VARIANT Position,
            /* [retval][out] */ __RPC__deref_out_opt /* external definition not present */ CodeAttribute **ppCodeAttribute);
        
        /* [helpstring][propget][id] */ HRESULT ( STDMETHODCALLTYPE *get_Adder )( 
            CodeEvent * This,
            /* [retval][out] */ __RPC__deref_out_opt /* external definition not present */ CodeFunction **ppCodeFunction);
        
        /* [helpstring][propput][id] */ HRESULT ( STDMETHODCALLTYPE *put_Adder )( 
            CodeEvent * This,
            __RPC__in /* external definition not present */ CodeFunction *codeFunction);
        
        /* [helpstring][propget][id] */ HRESULT ( STDMETHODCALLTYPE *get_Remover )( 
            CodeEvent * This,
            /* [retval][out] */ __RPC__deref_out_opt /* external definition not present */ CodeFunction **ppCodeFunction);
        
        /* [helpstring][propput][id] */ HRESULT ( STDMETHODCALLTYPE *put_Remover )( 
            CodeEvent * This,
            __RPC__in /* external definition not present */ CodeFunction *codeFunction);
        
        /* [helpstring][propget][id] */ HRESULT ( STDMETHODCALLTYPE *get_Thrower )( 
            CodeEvent * This,
            /* [retval][out] */ __RPC__deref_out_opt /* external definition not present */ CodeFunction **ppCodeFunction);
        
        /* [helpstring][propput][id] */ HRESULT ( STDMETHODCALLTYPE *put_Thrower )( 
            CodeEvent * This,
            __RPC__in /* external definition not present */ CodeFunction *codeFunction);
        
        /* [helpstring][propget][id] */ HRESULT ( STDMETHODCALLTYPE *get_IsPropertyStyleEvent )( 
            CodeEvent * This,
            /* [retval][out] */ __RPC__out VARIANT_BOOL *vbIsPropertyStyle);
        
        /* [nonbrowsable][helpstringcontext][helpstring][helpcontext][propget][id] */ HRESULT ( STDMETHODCALLTYPE *get_Prototype )( 
            CodeEvent * This,
            /* [defaultvalue][in] */ long Flags,
            /* [retval][out] */ __RPC__deref_out_opt BSTR *pFullName);
        
        /* [nonbrowsable][helpstringcontext][helpstring][helpcontext][propget][id] */ HRESULT ( STDMETHODCALLTYPE *get_Type )( 
            CodeEvent * This,
            /* [retval][out] */ __RPC__deref_out_opt /* external definition not present */ CodeTypeRef **ppCodeTypeRef);
        
        /* [nonbrowsable][helpstringcontext][helpstring][helpcontext][propput][id] */ HRESULT ( STDMETHODCALLTYPE *put_Type )( 
            CodeEvent * This,
            __RPC__in /* external definition not present */ CodeTypeRef *pCodeTypeRef);
        
        /* [helpstringcontext][helpstring][helpcontext][propget][id] */ HRESULT ( STDMETHODCALLTYPE *get_OverrideKind )( 
            CodeEvent * This,
            /* [retval][out] */ __RPC__out vsCMOverrideKind *Kind);
        
        /* [helpstringcontext][helpstring][helpcontext][propput][id] */ HRESULT ( STDMETHODCALLTYPE *put_OverrideKind )( 
            CodeEvent * This,
            vsCMOverrideKind Kind);
        
        /* [helpstringcontext][helpstring][helpcontext][propget][id] */ HRESULT ( STDMETHODCALLTYPE *get_IsShared )( 
            CodeEvent * This,
            /* [retval][out] */ __RPC__out VARIANT_BOOL *pIsShared);
        
        /* [helpstringcontext][helpstring][helpcontext][propput][id] */ HRESULT ( STDMETHODCALLTYPE *put_IsShared )( 
            CodeEvent * This,
            VARIANT_BOOL Shared);
        
        END_INTERFACE
    } CodeEventVtbl;

    interface CodeEvent
    {
        CONST_VTBL struct CodeEventVtbl *lpVtbl;
    };

    

#ifdef COBJMACROS


#define CodeEvent_QueryInterface(This,riid,ppvObject)	\
    ( (This)->lpVtbl -> QueryInterface(This,riid,ppvObject) ) 

#define CodeEvent_AddRef(This)	\
    ( (This)->lpVtbl -> AddRef(This) ) 

#define CodeEvent_Release(This)	\
    ( (This)->lpVtbl -> Release(This) ) 


#define CodeEvent_GetTypeInfoCount(This,pctinfo)	\
    ( (This)->lpVtbl -> GetTypeInfoCount(This,pctinfo) ) 

#define CodeEvent_GetTypeInfo(This,iTInfo,lcid,ppTInfo)	\
    ( (This)->lpVtbl -> GetTypeInfo(This,iTInfo,lcid,ppTInfo) ) 

#define CodeEvent_GetIDsOfNames(This,riid,rgszNames,cNames,lcid,rgDispId)	\
    ( (This)->lpVtbl -> GetIDsOfNames(This,riid,rgszNames,cNames,lcid,rgDispId) ) 

#define CodeEvent_Invoke(This,dispIdMember,riid,lcid,wFlags,pDispParams,pVarResult,pExcepInfo,puArgErr)	\
    ( (This)->lpVtbl -> Invoke(This,dispIdMember,riid,lcid,wFlags,pDispParams,pVarResult,pExcepInfo,puArgErr) ) 


#define CodeEvent_get_DTE(This,DTEObject)	\
    ( (This)->lpVtbl -> get_DTE(This,DTEObject) ) 

#define CodeEvent_get_Collection(This,ppCollection)	\
    ( (This)->lpVtbl -> get_Collection(This,ppCollection) ) 

#define CodeEvent_get_Name(This,pVal)	\
    ( (This)->lpVtbl -> get_Name(This,pVal) ) 

#define CodeEvent_put_Name(This,NewName)	\
    ( (This)->lpVtbl -> put_Name(This,NewName) ) 

#define CodeEvent_get_FullName(This,pVal)	\
    ( (This)->lpVtbl -> get_FullName(This,pVal) ) 

#define CodeEvent_get_ProjectItem(This,pProjItem)	\
    ( (This)->lpVtbl -> get_ProjectItem(This,pProjItem) ) 

#define CodeEvent_get_Kind(This,pCodeEltKind)	\
    ( (This)->lpVtbl -> get_Kind(This,pCodeEltKind) ) 

#define CodeEvent_get_IsCodeType(This,pIsCodeType)	\
    ( (This)->lpVtbl -> get_IsCodeType(This,pIsCodeType) ) 

#define CodeEvent_get_InfoLocation(This,pInfoLocation)	\
    ( (This)->lpVtbl -> get_InfoLocation(This,pInfoLocation) ) 

#define CodeEvent_get_Children(This,ppCodeElements)	\
    ( (This)->lpVtbl -> get_Children(This,ppCodeElements) ) 

#define CodeEvent_get_Language(This,pLanguage)	\
    ( (This)->lpVtbl -> get_Language(This,pLanguage) ) 

#define CodeEvent_get_StartPoint(This,ppTextPoint)	\
    ( (This)->lpVtbl -> get_StartPoint(This,ppTextPoint) ) 

#define CodeEvent_get_EndPoint(This,ppTextPoint)	\
    ( (This)->lpVtbl -> get_EndPoint(This,ppTextPoint) ) 

#define CodeEvent_get_ExtenderNames(This,ExtenderNames)	\
    ( (This)->lpVtbl -> get_ExtenderNames(This,ExtenderNames) ) 

#define CodeEvent_get_Extender(This,ExtenderName,Extender)	\
    ( (This)->lpVtbl -> get_Extender(This,ExtenderName,Extender) ) 

#define CodeEvent_get_ExtenderCATID(This,pRetval)	\
    ( (This)->lpVtbl -> get_ExtenderCATID(This,pRetval) ) 

#define CodeEvent_GetStartPoint(This,Part,ppTextPoint)	\
    ( (This)->lpVtbl -> GetStartPoint(This,Part,ppTextPoint) ) 

#define CodeEvent_GetEndPoint(This,Part,ppTextPoint)	\
    ( (This)->lpVtbl -> GetEndPoint(This,Part,ppTextPoint) ) 

#define CodeEvent_get_Parent(This,ParentObject)	\
    ( (This)->lpVtbl -> get_Parent(This,ParentObject) ) 

#define CodeEvent_put_Access(This,Access)	\
    ( (This)->lpVtbl -> put_Access(This,Access) ) 

#define CodeEvent_get_Access(This,Access)	\
    ( (This)->lpVtbl -> get_Access(This,Access) ) 

#define CodeEvent_get_Attributes(This,ppMembers)	\
    ( (This)->lpVtbl -> get_Attributes(This,ppMembers) ) 

#define CodeEvent_get_DocComment(This,pDocComment)	\
    ( (This)->lpVtbl -> get_DocComment(This,pDocComment) ) 

#define CodeEvent_put_DocComment(This,DocComment)	\
    ( (This)->lpVtbl -> put_DocComment(This,DocComment) ) 

#define CodeEvent_get_Comment(This,pComment)	\
    ( (This)->lpVtbl -> get_Comment(This,pComment) ) 

#define CodeEvent_put_Comment(This,Comment)	\
    ( (This)->lpVtbl -> put_Comment(This,Comment) ) 

#define CodeEvent_AddAttribute(This,Name,Value,Position,ppCodeAttribute)	\
    ( (This)->lpVtbl -> AddAttribute(This,Name,Value,Position,ppCodeAttribute) ) 

#define CodeEvent_get_Adder(This,ppCodeFunction)	\
    ( (This)->lpVtbl -> get_Adder(This,ppCodeFunction) ) 

#define CodeEvent_put_Adder(This,codeFunction)	\
    ( (This)->lpVtbl -> put_Adder(This,codeFunction) ) 

#define CodeEvent_get_Remover(This,ppCodeFunction)	\
    ( (This)->lpVtbl -> get_Remover(This,ppCodeFunction) ) 

#define CodeEvent_put_Remover(This,codeFunction)	\
    ( (This)->lpVtbl -> put_Remover(This,codeFunction) ) 

#define CodeEvent_get_Thrower(This,ppCodeFunction)	\
    ( (This)->lpVtbl -> get_Thrower(This,ppCodeFunction) ) 

#define CodeEvent_put_Thrower(This,codeFunction)	\
    ( (This)->lpVtbl -> put_Thrower(This,codeFunction) ) 

#define CodeEvent_get_IsPropertyStyleEvent(This,vbIsPropertyStyle)	\
    ( (This)->lpVtbl -> get_IsPropertyStyleEvent(This,vbIsPropertyStyle) ) 

#define CodeEvent_get_Prototype(This,Flags,pFullName)	\
    ( (This)->lpVtbl -> get_Prototype(This,Flags,pFullName) ) 

#define CodeEvent_get_Type(This,ppCodeTypeRef)	\
    ( (This)->lpVtbl -> get_Type(This,ppCodeTypeRef) ) 

#define CodeEvent_put_Type(This,pCodeTypeRef)	\
    ( (This)->lpVtbl -> put_Type(This,pCodeTypeRef) ) 

#define CodeEvent_get_OverrideKind(This,Kind)	\
    ( (This)->lpVtbl -> get_OverrideKind(This,Kind) ) 

#define CodeEvent_put_OverrideKind(This,Kind)	\
    ( (This)->lpVtbl -> put_OverrideKind(This,Kind) ) 

#define CodeEvent_get_IsShared(This,pIsShared)	\
    ( (This)->lpVtbl -> get_IsShared(This,pIsShared) ) 

#define CodeEvent_put_IsShared(This,Shared)	\
    ( (This)->lpVtbl -> put_IsShared(This,Shared) ) 

#endif /* COBJMACROS */


#endif 	/* C style interface */




#endif 	/* __CodeEvent_INTERFACE_DEFINED__ */


#ifndef __CodeElement2_INTERFACE_DEFINED__
#define __CodeElement2_INTERFACE_DEFINED__

/* interface CodeElement2 */
/* [helpstringcontext][helpstring][helpcontext][uuid][object][oleautomation][dual] */ 


EXTERN_C const IID IID_CodeElement2;

#if defined(__cplusplus) && !defined(CINTERFACE)
    
    MIDL_INTERFACE("F03DCCE8-233B-43D7-A66B-A66EFC1F85C3")
    CodeElement2 : public CodeElement
    {
    public:
        virtual /* [helpstringcontext][helpstring][helpcontext][propget][id] */ HRESULT STDMETHODCALLTYPE get_ElementID( 
            /* [retval][out] */ __RPC__deref_out_opt BSTR *ID) = 0;
        
        virtual /* [helpstringcontext][helpstring][helpcontext][id] */ HRESULT STDMETHODCALLTYPE RenameSymbol( 
            __RPC__in BSTR NewName) = 0;
        
    };
    
#else 	/* C style interface */

    typedef struct CodeElement2Vtbl
    {
        BEGIN_INTERFACE
        
        /* [id][restricted][funcdescattr] */ HRESULT ( STDMETHODCALLTYPE *QueryInterface )( 
            CodeElement2 * This,
            /* [in][idldescattr] */ __RPC__in struct GUID *riid,
            /* [out][idldescattr] */ __RPC__deref_out_opt void **ppvObj,
            /* [retval][out] */ __RPC__out void *retval);
        
        /* [id][restricted][funcdescattr] */ HRESULT ( STDMETHODCALLTYPE *AddRef )( 
            CodeElement2 * This,
            /* [retval][out] */ __RPC__out unsigned long *retval);
        
        /* [id][restricted][funcdescattr] */ HRESULT ( STDMETHODCALLTYPE *Release )( 
            CodeElement2 * This,
            /* [retval][out] */ __RPC__out unsigned long *retval);
        
        /* [id][restricted][funcdescattr] */ HRESULT ( STDMETHODCALLTYPE *GetTypeInfoCount )( 
            CodeElement2 * This,
            /* [out][idldescattr] */ __RPC__out unsigned UINT *pctinfo,
            /* [retval][out] */ __RPC__out void *retval);
        
        /* [id][restricted][funcdescattr] */ HRESULT ( STDMETHODCALLTYPE *GetTypeInfo )( 
            CodeElement2 * This,
            /* [in][idldescattr] */ unsigned UINT itinfo,
            /* [in][idldescattr] */ unsigned long lcid,
            /* [out][idldescattr] */ __RPC__deref_out_opt void **pptinfo,
            /* [retval][out] */ __RPC__out void *retval);
        
        /* [id][restricted][funcdescattr] */ HRESULT ( STDMETHODCALLTYPE *GetIDsOfNames )( 
            CodeElement2 * This,
            /* [in][idldescattr] */ __RPC__in struct GUID *riid,
            /* [in][idldescattr] */ __RPC__deref_in_opt signed char **rgszNames,
            /* [in][idldescattr] */ unsigned UINT cNames,
            /* [in][idldescattr] */ unsigned long lcid,
            /* [out][idldescattr] */ __RPC__out signed long *rgdispid,
            /* [retval][out] */ __RPC__out void *retval);
        
        /* [id][restricted][funcdescattr] */ HRESULT ( STDMETHODCALLTYPE *Invoke )( 
            CodeElement2 * This,
            /* [in][idldescattr] */ signed long dispidMember,
            /* [in][idldescattr] */ __RPC__in struct GUID *riid,
            /* [in][idldescattr] */ unsigned long lcid,
            /* [in][idldescattr] */ unsigned short wFlags,
            /* [in][idldescattr] */ __RPC__in struct DISPPARAMS *pdispparams,
            /* [out][idldescattr] */ __RPC__out VARIANT *pvarResult,
            /* [out][idldescattr] */ __RPC__out struct EXCEPINFO *pexcepinfo,
            /* [out][idldescattr] */ __RPC__out unsigned UINT *puArgErr,
            /* [retval][out] */ __RPC__out void *retval);
        
        /* [id][propget][funcdescattr] */ HRESULT ( STDMETHODCALLTYPE *get_DTE )( 
            CodeElement2 * This,
            /* [retval][out] */ __RPC__deref_out_opt **retval);
        
        /* [id][propget][funcdescattr] */ HRESULT ( STDMETHODCALLTYPE *get_Collection )( 
            CodeElement2 * This,
            /* [retval][out] */ __RPC__deref_out_opt CodeElements **retval);
        
        /* [id][propget][funcdescattr] */ HRESULT ( STDMETHODCALLTYPE *get_Name )( 
            CodeElement2 * This,
            /* [retval][out] */ __RPC__deref_out_opt BSTR *retval);
        
        /* [id][propput][funcdescattr] */ HRESULT ( STDMETHODCALLTYPE *put_Name )( 
            CodeElement2 * This,
            /* [idldescattr] */ __RPC__in BSTR noname,
            /* [retval][out] */ __RPC__out void *retval);
        
        /* [id][propget][funcdescattr] */ HRESULT ( STDMETHODCALLTYPE *get_FullName )( 
            CodeElement2 * This,
            /* [retval][out] */ __RPC__deref_out_opt BSTR *retval);
        
        /* [id][propget][funcdescattr] */ HRESULT ( STDMETHODCALLTYPE *get_ProjectItem )( 
            CodeElement2 * This,
            /* [retval][out] */ __RPC__deref_out_opt ProjectItem **retval);
        
        /* [id][propget][funcdescattr] */ HRESULT ( STDMETHODCALLTYPE *get_Kind )( 
            CodeElement2 * This,
            /* [retval][out] */ __RPC__out enum vsCMElement *retval);
        
        /* [id][propget][funcdescattr] */ HRESULT ( STDMETHODCALLTYPE *get_IsCodeType )( 
            CodeElement2 * This,
            /* [retval][out] */ __RPC__out BOOLEAN *retval);
        
        /* [id][propget][funcdescattr] */ HRESULT ( STDMETHODCALLTYPE *get_InfoLocation )( 
            CodeElement2 * This,
            /* [retval][out] */ __RPC__out enum vsCMInfoLocation *retval);
        
        /* [id][propget][funcdescattr] */ HRESULT ( STDMETHODCALLTYPE *get_Children )( 
            CodeElement2 * This,
            /* [retval][out] */ __RPC__deref_out_opt CodeElements **retval);
        
        /* [id][propget][funcdescattr] */ HRESULT ( STDMETHODCALLTYPE *get_Language )( 
            CodeElement2 * This,
            /* [retval][out] */ __RPC__deref_out_opt BSTR *retval);
        
        /* [id][propget][funcdescattr] */ HRESULT ( STDMETHODCALLTYPE *get_StartPoint )( 
            CodeElement2 * This,
            /* [retval][out] */ __RPC__deref_out_opt TextPoint **retval);
        
        /* [id][propget][funcdescattr] */ HRESULT ( STDMETHODCALLTYPE *get_EndPoint )( 
            CodeElement2 * This,
            /* [retval][out] */ __RPC__deref_out_opt TextPoint **retval);
        
        /* [id][propget][funcdescattr] */ HRESULT ( STDMETHODCALLTYPE *get_ExtenderNames )( 
            CodeElement2 * This,
            /* [retval][out] */ __RPC__out VARIANT *retval);
        
        /* [id][propget][funcdescattr] */ HRESULT ( STDMETHODCALLTYPE *get_Extender )( 
            CodeElement2 * This,
            /* [idldescattr] */ __RPC__in BSTR ExtenderName,
            /* [retval][out] */ __RPC__deref_out_opt IDispatch **retval);
        
        /* [id][propget][funcdescattr] */ HRESULT ( STDMETHODCALLTYPE *get_ExtenderCATID )( 
            CodeElement2 * This,
            /* [retval][out] */ __RPC__deref_out_opt BSTR *retval);
        
        /* [id][funcdescattr] */ HRESULT ( STDMETHODCALLTYPE *GetStartPoint )( 
            CodeElement2 * This,
            /* [in][idldescattr] */ enum vsCMPart Part,
            /* [retval][out] */ __RPC__deref_out_opt TextPoint **retval);
        
        /* [id][funcdescattr] */ HRESULT ( STDMETHODCALLTYPE *GetEndPoint )( 
            CodeElement2 * This,
            /* [in][idldescattr] */ enum vsCMPart Part,
            /* [retval][out] */ __RPC__deref_out_opt TextPoint **retval);
        
        /* [helpstringcontext][helpstring][helpcontext][propget][id] */ HRESULT ( STDMETHODCALLTYPE *get_ElementID )( 
            CodeElement2 * This,
            /* [retval][out] */ __RPC__deref_out_opt BSTR *ID);
        
        /* [helpstringcontext][helpstring][helpcontext][id] */ HRESULT ( STDMETHODCALLTYPE *RenameSymbol )( 
            CodeElement2 * This,
            __RPC__in BSTR NewName);
        
        END_INTERFACE
    } CodeElement2Vtbl;

    interface CodeElement2
    {
        CONST_VTBL struct CodeElement2Vtbl *lpVtbl;
    };

    

#ifdef COBJMACROS


#define CodeElement2_QueryInterface(This,riid,ppvObj,retval)	\
    ( (This)->lpVtbl -> QueryInterface(This,riid,ppvObj,retval) ) 

#define CodeElement2_AddRef(This,retval)	\
    ( (This)->lpVtbl -> AddRef(This,retval) ) 

#define CodeElement2_Release(This,retval)	\
    ( (This)->lpVtbl -> Release(This,retval) ) 

#define CodeElement2_GetTypeInfoCount(This,pctinfo,retval)	\
    ( (This)->lpVtbl -> GetTypeInfoCount(This,pctinfo,retval) ) 

#define CodeElement2_GetTypeInfo(This,itinfo,lcid,pptinfo,retval)	\
    ( (This)->lpVtbl -> GetTypeInfo(This,itinfo,lcid,pptinfo,retval) ) 

#define CodeElement2_GetIDsOfNames(This,riid,rgszNames,cNames,lcid,rgdispid,retval)	\
    ( (This)->lpVtbl -> GetIDsOfNames(This,riid,rgszNames,cNames,lcid,rgdispid,retval) ) 

#define CodeElement2_Invoke(This,dispidMember,riid,lcid,wFlags,pdispparams,pvarResult,pexcepinfo,puArgErr,retval)	\
    ( (This)->lpVtbl -> Invoke(This,dispidMember,riid,lcid,wFlags,pdispparams,pvarResult,pexcepinfo,puArgErr,retval) ) 

#define CodeElement2_get_DTE(This,retval)	\
    ( (This)->lpVtbl -> get_DTE(This,retval) ) 

#define CodeElement2_get_Collection(This,retval)	\
    ( (This)->lpVtbl -> get_Collection(This,retval) ) 

#define CodeElement2_get_Name(This,retval)	\
    ( (This)->lpVtbl -> get_Name(This,retval) ) 

#define CodeElement2_put_Name(This,noname,retval)	\
    ( (This)->lpVtbl -> put_Name(This,noname,retval) ) 

#define CodeElement2_get_FullName(This,retval)	\
    ( (This)->lpVtbl -> get_FullName(This,retval) ) 

#define CodeElement2_get_ProjectItem(This,retval)	\
    ( (This)->lpVtbl -> get_ProjectItem(This,retval) ) 

#define CodeElement2_get_Kind(This,retval)	\
    ( (This)->lpVtbl -> get_Kind(This,retval) ) 

#define CodeElement2_get_IsCodeType(This,retval)	\
    ( (This)->lpVtbl -> get_IsCodeType(This,retval) ) 

#define CodeElement2_get_InfoLocation(This,retval)	\
    ( (This)->lpVtbl -> get_InfoLocation(This,retval) ) 

#define CodeElement2_get_Children(This,retval)	\
    ( (This)->lpVtbl -> get_Children(This,retval) ) 

#define CodeElement2_get_Language(This,retval)	\
    ( (This)->lpVtbl -> get_Language(This,retval) ) 

#define CodeElement2_get_StartPoint(This,retval)	\
    ( (This)->lpVtbl -> get_StartPoint(This,retval) ) 

#define CodeElement2_get_EndPoint(This,retval)	\
    ( (This)->lpVtbl -> get_EndPoint(This,retval) ) 

#define CodeElement2_get_ExtenderNames(This,retval)	\
    ( (This)->lpVtbl -> get_ExtenderNames(This,retval) ) 

#define CodeElement2_get_Extender(This,ExtenderName,retval)	\
    ( (This)->lpVtbl -> get_Extender(This,ExtenderName,retval) ) 

#define CodeElement2_get_ExtenderCATID(This,retval)	\
    ( (This)->lpVtbl -> get_ExtenderCATID(This,retval) ) 

#define CodeElement2_GetStartPoint(This,Part,retval)	\
    ( (This)->lpVtbl -> GetStartPoint(This,Part,retval) ) 

#define CodeElement2_GetEndPoint(This,Part,retval)	\
    ( (This)->lpVtbl -> GetEndPoint(This,Part,retval) ) 


#define CodeElement2_get_ElementID(This,ID)	\
    ( (This)->lpVtbl -> get_ElementID(This,ID) ) 

#define CodeElement2_RenameSymbol(This,NewName)	\
    ( (This)->lpVtbl -> RenameSymbol(This,NewName) ) 

#endif /* COBJMACROS */


#endif 	/* C style interface */




#endif 	/* __CodeElement2_INTERFACE_DEFINED__ */


#ifndef __CodeImport_INTERFACE_DEFINED__
#define __CodeImport_INTERFACE_DEFINED__

/* interface CodeImport */
/* [helpstringcontext][helpstring][helpcontext][uuid][object][oleautomation][dual] */ 


EXTERN_C const IID IID_CodeImport;

#if defined(__cplusplus) && !defined(CINTERFACE)
    
    MIDL_INTERFACE("0C9A256E-0FF9-4D4A-88E6-304ACF78225F")
    CodeImport : public CodeElement
    {
    public:
        virtual /* [helpstringcontext][helpstring][helpcontext][propget][id] */ HRESULT STDMETHODCALLTYPE get_Namespace( 
            /* [retval][out] */ __RPC__deref_out_opt BSTR *Name) = 0;
        
        virtual /* [helpstringcontext][helpstring][helpcontext][propput][id] */ HRESULT STDMETHODCALLTYPE put_Namespace( 
            __RPC__in BSTR Name) = 0;
        
        virtual /* [helpstringcontext][helpstring][helpcontext][propget][id] */ HRESULT STDMETHODCALLTYPE get_Alias( 
            /* [retval][out] */ __RPC__deref_out_opt BSTR *Alias) = 0;
        
        virtual /* [helpstringcontext][helpstring][helpcontext][propput][id] */ HRESULT STDMETHODCALLTYPE put_Alias( 
            __RPC__in BSTR Alias) = 0;
        
        virtual /* [helpstringcontext][helpstring][helpcontext][propget][id] */ HRESULT STDMETHODCALLTYPE get_Parent( 
            /* [retval][out] */ __RPC__deref_out_opt IDispatch **ParentObject) = 0;
        
    };
    
#else 	/* C style interface */

    typedef struct CodeImportVtbl
    {
        BEGIN_INTERFACE
        
        /* [id][restricted][funcdescattr] */ HRESULT ( STDMETHODCALLTYPE *QueryInterface )( 
            CodeImport * This,
            /* [in][idldescattr] */ __RPC__in struct GUID *riid,
            /* [out][idldescattr] */ __RPC__deref_out_opt void **ppvObj,
            /* [retval][out] */ __RPC__out void *retval);
        
        /* [id][restricted][funcdescattr] */ HRESULT ( STDMETHODCALLTYPE *AddRef )( 
            CodeImport * This,
            /* [retval][out] */ __RPC__out unsigned long *retval);
        
        /* [id][restricted][funcdescattr] */ HRESULT ( STDMETHODCALLTYPE *Release )( 
            CodeImport * This,
            /* [retval][out] */ __RPC__out unsigned long *retval);
        
        /* [id][restricted][funcdescattr] */ HRESULT ( STDMETHODCALLTYPE *GetTypeInfoCount )( 
            CodeImport * This,
            /* [out][idldescattr] */ __RPC__out unsigned UINT *pctinfo,
            /* [retval][out] */ __RPC__out void *retval);
        
        /* [id][restricted][funcdescattr] */ HRESULT ( STDMETHODCALLTYPE *GetTypeInfo )( 
            CodeImport * This,
            /* [in][idldescattr] */ unsigned UINT itinfo,
            /* [in][idldescattr] */ unsigned long lcid,
            /* [out][idldescattr] */ __RPC__deref_out_opt void **pptinfo,
            /* [retval][out] */ __RPC__out void *retval);
        
        /* [id][restricted][funcdescattr] */ HRESULT ( STDMETHODCALLTYPE *GetIDsOfNames )( 
            CodeImport * This,
            /* [in][idldescattr] */ __RPC__in struct GUID *riid,
            /* [in][idldescattr] */ __RPC__deref_in_opt signed char **rgszNames,
            /* [in][idldescattr] */ unsigned UINT cNames,
            /* [in][idldescattr] */ unsigned long lcid,
            /* [out][idldescattr] */ __RPC__out signed long *rgdispid,
            /* [retval][out] */ __RPC__out void *retval);
        
        /* [id][restricted][funcdescattr] */ HRESULT ( STDMETHODCALLTYPE *Invoke )( 
            CodeImport * This,
            /* [in][idldescattr] */ signed long dispidMember,
            /* [in][idldescattr] */ __RPC__in struct GUID *riid,
            /* [in][idldescattr] */ unsigned long lcid,
            /* [in][idldescattr] */ unsigned short wFlags,
            /* [in][idldescattr] */ __RPC__in struct DISPPARAMS *pdispparams,
            /* [out][idldescattr] */ __RPC__out VARIANT *pvarResult,
            /* [out][idldescattr] */ __RPC__out struct EXCEPINFO *pexcepinfo,
            /* [out][idldescattr] */ __RPC__out unsigned UINT *puArgErr,
            /* [retval][out] */ __RPC__out void *retval);
        
        /* [id][propget][funcdescattr] */ HRESULT ( STDMETHODCALLTYPE *get_DTE )( 
            CodeImport * This,
            /* [retval][out] */ __RPC__deref_out_opt **retval);
        
        /* [id][propget][funcdescattr] */ HRESULT ( STDMETHODCALLTYPE *get_Collection )( 
            CodeImport * This,
            /* [retval][out] */ __RPC__deref_out_opt CodeElements **retval);
        
        /* [id][propget][funcdescattr] */ HRESULT ( STDMETHODCALLTYPE *get_Name )( 
            CodeImport * This,
            /* [retval][out] */ __RPC__deref_out_opt BSTR *retval);
        
        /* [id][propput][funcdescattr] */ HRESULT ( STDMETHODCALLTYPE *put_Name )( 
            CodeImport * This,
            /* [idldescattr] */ __RPC__in BSTR noname,
            /* [retval][out] */ __RPC__out void *retval);
        
        /* [id][propget][funcdescattr] */ HRESULT ( STDMETHODCALLTYPE *get_FullName )( 
            CodeImport * This,
            /* [retval][out] */ __RPC__deref_out_opt BSTR *retval);
        
        /* [id][propget][funcdescattr] */ HRESULT ( STDMETHODCALLTYPE *get_ProjectItem )( 
            CodeImport * This,
            /* [retval][out] */ __RPC__deref_out_opt ProjectItem **retval);
        
        /* [id][propget][funcdescattr] */ HRESULT ( STDMETHODCALLTYPE *get_Kind )( 
            CodeImport * This,
            /* [retval][out] */ __RPC__out enum vsCMElement *retval);
        
        /* [id][propget][funcdescattr] */ HRESULT ( STDMETHODCALLTYPE *get_IsCodeType )( 
            CodeImport * This,
            /* [retval][out] */ __RPC__out BOOLEAN *retval);
        
        /* [id][propget][funcdescattr] */ HRESULT ( STDMETHODCALLTYPE *get_InfoLocation )( 
            CodeImport * This,
            /* [retval][out] */ __RPC__out enum vsCMInfoLocation *retval);
        
        /* [id][propget][funcdescattr] */ HRESULT ( STDMETHODCALLTYPE *get_Children )( 
            CodeImport * This,
            /* [retval][out] */ __RPC__deref_out_opt CodeElements **retval);
        
        /* [id][propget][funcdescattr] */ HRESULT ( STDMETHODCALLTYPE *get_Language )( 
            CodeImport * This,
            /* [retval][out] */ __RPC__deref_out_opt BSTR *retval);
        
        /* [id][propget][funcdescattr] */ HRESULT ( STDMETHODCALLTYPE *get_StartPoint )( 
            CodeImport * This,
            /* [retval][out] */ __RPC__deref_out_opt TextPoint **retval);
        
        /* [id][propget][funcdescattr] */ HRESULT ( STDMETHODCALLTYPE *get_EndPoint )( 
            CodeImport * This,
            /* [retval][out] */ __RPC__deref_out_opt TextPoint **retval);
        
        /* [id][propget][funcdescattr] */ HRESULT ( STDMETHODCALLTYPE *get_ExtenderNames )( 
            CodeImport * This,
            /* [retval][out] */ __RPC__out VARIANT *retval);
        
        /* [id][propget][funcdescattr] */ HRESULT ( STDMETHODCALLTYPE *get_Extender )( 
            CodeImport * This,
            /* [idldescattr] */ __RPC__in BSTR ExtenderName,
            /* [retval][out] */ __RPC__deref_out_opt IDispatch **retval);
        
        /* [id][propget][funcdescattr] */ HRESULT ( STDMETHODCALLTYPE *get_ExtenderCATID )( 
            CodeImport * This,
            /* [retval][out] */ __RPC__deref_out_opt BSTR *retval);
        
        /* [id][funcdescattr] */ HRESULT ( STDMETHODCALLTYPE *GetStartPoint )( 
            CodeImport * This,
            /* [in][idldescattr] */ enum vsCMPart Part,
            /* [retval][out] */ __RPC__deref_out_opt TextPoint **retval);
        
        /* [id][funcdescattr] */ HRESULT ( STDMETHODCALLTYPE *GetEndPoint )( 
            CodeImport * This,
            /* [in][idldescattr] */ enum vsCMPart Part,
            /* [retval][out] */ __RPC__deref_out_opt TextPoint **retval);
        
        /* [helpstringcontext][helpstring][helpcontext][propget][id] */ HRESULT ( STDMETHODCALLTYPE *get_Namespace )( 
            CodeImport * This,
            /* [retval][out] */ __RPC__deref_out_opt BSTR *Name);
        
        /* [helpstringcontext][helpstring][helpcontext][propput][id] */ HRESULT ( STDMETHODCALLTYPE *put_Namespace )( 
            CodeImport * This,
            __RPC__in BSTR Name);
        
        /* [helpstringcontext][helpstring][helpcontext][propget][id] */ HRESULT ( STDMETHODCALLTYPE *get_Alias )( 
            CodeImport * This,
            /* [retval][out] */ __RPC__deref_out_opt BSTR *Alias);
        
        /* [helpstringcontext][helpstring][helpcontext][propput][id] */ HRESULT ( STDMETHODCALLTYPE *put_Alias )( 
            CodeImport * This,
            __RPC__in BSTR Alias);
        
        /* [helpstringcontext][helpstring][helpcontext][propget][id] */ HRESULT ( STDMETHODCALLTYPE *get_Parent )( 
            CodeImport * This,
            /* [retval][out] */ __RPC__deref_out_opt IDispatch **ParentObject);
        
        END_INTERFACE
    } CodeImportVtbl;

    interface CodeImport
    {
        CONST_VTBL struct CodeImportVtbl *lpVtbl;
    };

    

#ifdef COBJMACROS


#define CodeImport_QueryInterface(This,riid,ppvObj,retval)	\
    ( (This)->lpVtbl -> QueryInterface(This,riid,ppvObj,retval) ) 

#define CodeImport_AddRef(This,retval)	\
    ( (This)->lpVtbl -> AddRef(This,retval) ) 

#define CodeImport_Release(This,retval)	\
    ( (This)->lpVtbl -> Release(This,retval) ) 

#define CodeImport_GetTypeInfoCount(This,pctinfo,retval)	\
    ( (This)->lpVtbl -> GetTypeInfoCount(This,pctinfo,retval) ) 

#define CodeImport_GetTypeInfo(This,itinfo,lcid,pptinfo,retval)	\
    ( (This)->lpVtbl -> GetTypeInfo(This,itinfo,lcid,pptinfo,retval) ) 

#define CodeImport_GetIDsOfNames(This,riid,rgszNames,cNames,lcid,rgdispid,retval)	\
    ( (This)->lpVtbl -> GetIDsOfNames(This,riid,rgszNames,cNames,lcid,rgdispid,retval) ) 

#define CodeImport_Invoke(This,dispidMember,riid,lcid,wFlags,pdispparams,pvarResult,pexcepinfo,puArgErr,retval)	\
    ( (This)->lpVtbl -> Invoke(This,dispidMember,riid,lcid,wFlags,pdispparams,pvarResult,pexcepinfo,puArgErr,retval) ) 

#define CodeImport_get_DTE(This,retval)	\
    ( (This)->lpVtbl -> get_DTE(This,retval) ) 

#define CodeImport_get_Collection(This,retval)	\
    ( (This)->lpVtbl -> get_Collection(This,retval) ) 

#define CodeImport_get_Name(This,retval)	\
    ( (This)->lpVtbl -> get_Name(This,retval) ) 

#define CodeImport_put_Name(This,noname,retval)	\
    ( (This)->lpVtbl -> put_Name(This,noname,retval) ) 

#define CodeImport_get_FullName(This,retval)	\
    ( (This)->lpVtbl -> get_FullName(This,retval) ) 

#define CodeImport_get_ProjectItem(This,retval)	\
    ( (This)->lpVtbl -> get_ProjectItem(This,retval) ) 

#define CodeImport_get_Kind(This,retval)	\
    ( (This)->lpVtbl -> get_Kind(This,retval) ) 

#define CodeImport_get_IsCodeType(This,retval)	\
    ( (This)->lpVtbl -> get_IsCodeType(This,retval) ) 

#define CodeImport_get_InfoLocation(This,retval)	\
    ( (This)->lpVtbl -> get_InfoLocation(This,retval) ) 

#define CodeImport_get_Children(This,retval)	\
    ( (This)->lpVtbl -> get_Children(This,retval) ) 

#define CodeImport_get_Language(This,retval)	\
    ( (This)->lpVtbl -> get_Language(This,retval) ) 

#define CodeImport_get_StartPoint(This,retval)	\
    ( (This)->lpVtbl -> get_StartPoint(This,retval) ) 

#define CodeImport_get_EndPoint(This,retval)	\
    ( (This)->lpVtbl -> get_EndPoint(This,retval) ) 

#define CodeImport_get_ExtenderNames(This,retval)	\
    ( (This)->lpVtbl -> get_ExtenderNames(This,retval) ) 

#define CodeImport_get_Extender(This,ExtenderName,retval)	\
    ( (This)->lpVtbl -> get_Extender(This,ExtenderName,retval) ) 

#define CodeImport_get_ExtenderCATID(This,retval)	\
    ( (This)->lpVtbl -> get_ExtenderCATID(This,retval) ) 

#define CodeImport_GetStartPoint(This,Part,retval)	\
    ( (This)->lpVtbl -> GetStartPoint(This,Part,retval) ) 

#define CodeImport_GetEndPoint(This,Part,retval)	\
    ( (This)->lpVtbl -> GetEndPoint(This,Part,retval) ) 


#define CodeImport_get_Namespace(This,Name)	\
    ( (This)->lpVtbl -> get_Namespace(This,Name) ) 

#define CodeImport_put_Namespace(This,Name)	\
    ( (This)->lpVtbl -> put_Namespace(This,Name) ) 

#define CodeImport_get_Alias(This,Alias)	\
    ( (This)->lpVtbl -> get_Alias(This,Alias) ) 

#define CodeImport_put_Alias(This,Alias)	\
    ( (This)->lpVtbl -> put_Alias(This,Alias) ) 

#define CodeImport_get_Parent(This,ParentObject)	\
    ( (This)->lpVtbl -> get_Parent(This,ParentObject) ) 

#endif /* COBJMACROS */


#endif 	/* C style interface */




#endif 	/* __CodeImport_INTERFACE_DEFINED__ */


#ifndef __FileCodeModel2_INTERFACE_DEFINED__
#define __FileCodeModel2_INTERFACE_DEFINED__

/* interface FileCodeModel2 */
/* [helpstringcontext][helpstring][helpcontext][uuid][object][oleautomation][dual] */ 


EXTERN_C const IID IID_FileCodeModel2;

#if defined(__cplusplus) && !defined(CINTERFACE)
    
    MIDL_INTERFACE("A65964DF-3A07-45EB-882A-DD04602016B9")
    FileCodeModel2 : public FileCodeModel
    {
    public:
        virtual /* [helpstringcontext][helpstring][helpcontext][id] */ HRESULT STDMETHODCALLTYPE Synchronize( void) = 0;
        
        virtual /* [helpstringcontext][helpstring][helpcontext][propget][id] */ HRESULT STDMETHODCALLTYPE get_ParseStatus( 
            /* [retval][out] */ __RPC__out vsCMParseStatus *Status) = 0;
        
        virtual /* [helpstringcontext][helpstring][helpcontext][id] */ HRESULT STDMETHODCALLTYPE AddImport( 
            __RPC__in BSTR Name,
            /* [optional][in] */ VARIANT Position,
            /* [defaultvalue][in] */ __RPC__in BSTR Alias,
            /* [retval][out] */ __RPC__deref_out_opt CodeImport **ppImport) = 0;
        
        virtual /* [helpstringcontext][helpstring][helpcontext][id] */ HRESULT STDMETHODCALLTYPE BeginBatch( void) = 0;
        
        virtual /* [helpstringcontext][helpstring][helpcontext][id] */ HRESULT STDMETHODCALLTYPE EndBatch( void) = 0;
        
        virtual /* [helpstring][propget][id] */ HRESULT STDMETHODCALLTYPE get_IsBatchOpen( 
            /* [retval][out] */ __RPC__out VARIANT_BOOL *Open) = 0;
        
        virtual /* [helpstringcontext][helpstring][helpcontext][id] */ HRESULT STDMETHODCALLTYPE ElementFromID( 
            __RPC__in BSTR ID,
            /* [retval][out] */ __RPC__deref_out_opt /* external definition not present */ CodeElement **ppElement) = 0;
        
    };
    
#else 	/* C style interface */

    typedef struct FileCodeModel2Vtbl
    {
        BEGIN_INTERFACE
        
        /* [id][restricted][funcdescattr] */ HRESULT ( STDMETHODCALLTYPE *QueryInterface )( 
            FileCodeModel2 * This,
            /* [in][idldescattr] */ __RPC__in struct GUID *riid,
            /* [out][idldescattr] */ __RPC__deref_out_opt void **ppvObj,
            /* [retval][out] */ __RPC__out void *retval);
        
        /* [id][restricted][funcdescattr] */ HRESULT ( STDMETHODCALLTYPE *AddRef )( 
            FileCodeModel2 * This,
            /* [retval][out] */ __RPC__out unsigned long *retval);
        
        /* [id][restricted][funcdescattr] */ HRESULT ( STDMETHODCALLTYPE *Release )( 
            FileCodeModel2 * This,
            /* [retval][out] */ __RPC__out unsigned long *retval);
        
        /* [id][restricted][funcdescattr] */ HRESULT ( STDMETHODCALLTYPE *GetTypeInfoCount )( 
            FileCodeModel2 * This,
            /* [out][idldescattr] */ __RPC__out unsigned UINT *pctinfo,
            /* [retval][out] */ __RPC__out void *retval);
        
        /* [id][restricted][funcdescattr] */ HRESULT ( STDMETHODCALLTYPE *GetTypeInfo )( 
            FileCodeModel2 * This,
            /* [in][idldescattr] */ unsigned UINT itinfo,
            /* [in][idldescattr] */ unsigned long lcid,
            /* [out][idldescattr] */ __RPC__deref_out_opt void **pptinfo,
            /* [retval][out] */ __RPC__out void *retval);
        
        /* [id][restricted][funcdescattr] */ HRESULT ( STDMETHODCALLTYPE *GetIDsOfNames )( 
            FileCodeModel2 * This,
            /* [in][idldescattr] */ __RPC__in struct GUID *riid,
            /* [in][idldescattr] */ __RPC__deref_in_opt signed char **rgszNames,
            /* [in][idldescattr] */ unsigned UINT cNames,
            /* [in][idldescattr] */ unsigned long lcid,
            /* [out][idldescattr] */ __RPC__out signed long *rgdispid,
            /* [retval][out] */ __RPC__out void *retval);
        
        /* [id][restricted][funcdescattr] */ HRESULT ( STDMETHODCALLTYPE *Invoke )( 
            FileCodeModel2 * This,
            /* [in][idldescattr] */ signed long dispidMember,
            /* [in][idldescattr] */ __RPC__in struct GUID *riid,
            /* [in][idldescattr] */ unsigned long lcid,
            /* [in][idldescattr] */ unsigned short wFlags,
            /* [in][idldescattr] */ __RPC__in struct DISPPARAMS *pdispparams,
            /* [out][idldescattr] */ __RPC__out VARIANT *pvarResult,
            /* [out][idldescattr] */ __RPC__out struct EXCEPINFO *pexcepinfo,
            /* [out][idldescattr] */ __RPC__out unsigned UINT *puArgErr,
            /* [retval][out] */ __RPC__out void *retval);
        
        /* [id][propget][funcdescattr] */ HRESULT ( STDMETHODCALLTYPE *get_DTE )( 
            FileCodeModel2 * This,
            /* [retval][out] */ __RPC__deref_out_opt **retval);
        
        /* [id][propget][funcdescattr] */ HRESULT ( STDMETHODCALLTYPE *get_Parent )( 
            FileCodeModel2 * This,
            /* [retval][out] */ __RPC__deref_out_opt ProjectItem **retval);
        
        /* [id][propget][funcdescattr] */ HRESULT ( STDMETHODCALLTYPE *get_Language )( 
            FileCodeModel2 * This,
            /* [retval][out] */ __RPC__deref_out_opt BSTR *retval);
        
        /* [id][propget][funcdescattr] */ HRESULT ( STDMETHODCALLTYPE *get_CodeElements )( 
            FileCodeModel2 * This,
            /* [retval][out] */ __RPC__deref_out_opt CodeElements **retval);
        
        /* [id][funcdescattr] */ HRESULT ( STDMETHODCALLTYPE *CodeElementFromPoint )( 
            FileCodeModel2 * This,
            /* [idldescattr] */ __RPC__in_opt TextPoint *Point,
            /* [idldescattr] */ enum vsCMElement Scope,
            /* [retval][out] */ __RPC__deref_out_opt CodeElement **retval);
        
        /* [id][funcdescattr] */ HRESULT ( STDMETHODCALLTYPE *AddNamespace )( 
            FileCodeModel2 * This,
            /* [idldescattr] */ __RPC__in BSTR Name,
            /* [optional][idldescattr] */ VARIANT Position,
            /* [retval][out] */ __RPC__deref_out_opt CodeNamespace **retval);
        
        /* [id][funcdescattr] */ HRESULT ( STDMETHODCALLTYPE *AddClass )( 
            FileCodeModel2 * This,
            /* [idldescattr] */ __RPC__in BSTR Name,
            /* [idldescattr] */ VARIANT Position,
            /* [idldescattr] */ VARIANT Bases,
            /* [idldescattr] */ VARIANT ImplementedInterfaces,
            /* [idldescattr] */ enum vsCMAccess Access,
            /* [retval][out] */ __RPC__deref_out_opt CodeClass **retval);
        
        /* [id][funcdescattr] */ HRESULT ( STDMETHODCALLTYPE *AddInterface )( 
            FileCodeModel2 * This,
            /* [idldescattr] */ __RPC__in BSTR Name,
            /* [idldescattr] */ VARIANT Position,
            /* [idldescattr] */ VARIANT Bases,
            /* [idldescattr] */ enum vsCMAccess Access,
            /* [retval][out] */ __RPC__deref_out_opt CodeInterface **retval);
        
        /* [id][funcdescattr] */ HRESULT ( STDMETHODCALLTYPE *AddFunction )( 
            FileCodeModel2 * This,
            /* [idldescattr] */ __RPC__in BSTR Name,
            /* [idldescattr] */ enum vsCMFunction Kind,
            /* [idldescattr] */ VARIANT Type,
            /* [idldescattr] */ VARIANT Position,
            /* [idldescattr] */ enum vsCMAccess Access,
            /* [retval][out] */ __RPC__deref_out_opt CodeFunction **retval);
        
        /* [id][funcdescattr] */ HRESULT ( STDMETHODCALLTYPE *AddVariable )( 
            FileCodeModel2 * This,
            /* [idldescattr] */ __RPC__in BSTR Name,
            /* [idldescattr] */ VARIANT Type,
            /* [idldescattr] */ VARIANT Position,
            /* [idldescattr] */ enum vsCMAccess Access,
            /* [retval][out] */ __RPC__deref_out_opt CodeVariable **retval);
        
        /* [id][funcdescattr] */ HRESULT ( STDMETHODCALLTYPE *AddAttribute )( 
            FileCodeModel2 * This,
            /* [idldescattr] */ __RPC__in BSTR Name,
            /* [idldescattr] */ __RPC__in BSTR Value,
            /* [optional][idldescattr] */ VARIANT Position,
            /* [retval][out] */ __RPC__deref_out_opt CodeAttribute **retval);
        
        /* [id][funcdescattr] */ HRESULT ( STDMETHODCALLTYPE *AddStruct )( 
            FileCodeModel2 * This,
            /* [idldescattr] */ __RPC__in BSTR Name,
            /* [idldescattr] */ VARIANT Position,
            /* [idldescattr] */ VARIANT Bases,
            /* [idldescattr] */ VARIANT ImplementedInterfaces,
            /* [idldescattr] */ enum vsCMAccess Access,
            /* [retval][out] */ __RPC__deref_out_opt CodeStruct **retval);
        
        /* [id][funcdescattr] */ HRESULT ( STDMETHODCALLTYPE *AddEnum )( 
            FileCodeModel2 * This,
            /* [idldescattr] */ __RPC__in BSTR Name,
            /* [idldescattr] */ VARIANT Position,
            /* [idldescattr] */ VARIANT Bases,
            /* [idldescattr] */ enum vsCMAccess Access,
            /* [retval][out] */ __RPC__deref_out_opt CodeEnum **retval);
        
        /* [id][funcdescattr] */ HRESULT ( STDMETHODCALLTYPE *AddDelegate )( 
            FileCodeModel2 * This,
            /* [idldescattr] */ __RPC__in BSTR Name,
            /* [idldescattr] */ VARIANT Type,
            /* [idldescattr] */ VARIANT Position,
            /* [idldescattr] */ enum vsCMAccess Access,
            /* [retval][out] */ __RPC__deref_out_opt CodeDelegate **retval);
        
        /* [id][funcdescattr] */ HRESULT ( STDMETHODCALLTYPE *Remove )( 
            FileCodeModel2 * This,
            /* [idldescattr] */ VARIANT Element,
            /* [retval][out] */ __RPC__out void *retval);
        
        /* [helpstringcontext][helpstring][helpcontext][id] */ HRESULT ( STDMETHODCALLTYPE *Synchronize )( 
            FileCodeModel2 * This);
        
        /* [helpstringcontext][helpstring][helpcontext][propget][id] */ HRESULT ( STDMETHODCALLTYPE *get_ParseStatus )( 
            FileCodeModel2 * This,
            /* [retval][out] */ __RPC__out vsCMParseStatus *Status);
        
        /* [helpstringcontext][helpstring][helpcontext][id] */ HRESULT ( STDMETHODCALLTYPE *AddImport )( 
            FileCodeModel2 * This,
            __RPC__in BSTR Name,
            /* [optional][in] */ VARIANT Position,
            /* [defaultvalue][in] */ __RPC__in BSTR Alias,
            /* [retval][out] */ __RPC__deref_out_opt CodeImport **ppImport);
        
        /* [helpstringcontext][helpstring][helpcontext][id] */ HRESULT ( STDMETHODCALLTYPE *BeginBatch )( 
            FileCodeModel2 * This);
        
        /* [helpstringcontext][helpstring][helpcontext][id] */ HRESULT ( STDMETHODCALLTYPE *EndBatch )( 
            FileCodeModel2 * This);
        
        /* [helpstring][propget][id] */ HRESULT ( STDMETHODCALLTYPE *get_IsBatchOpen )( 
            FileCodeModel2 * This,
            /* [retval][out] */ __RPC__out VARIANT_BOOL *Open);
        
        /* [helpstringcontext][helpstring][helpcontext][id] */ HRESULT ( STDMETHODCALLTYPE *ElementFromID )( 
            FileCodeModel2 * This,
            __RPC__in BSTR ID,
            /* [retval][out] */ __RPC__deref_out_opt /* external definition not present */ CodeElement **ppElement);
        
        END_INTERFACE
    } FileCodeModel2Vtbl;

    interface FileCodeModel2
    {
        CONST_VTBL struct FileCodeModel2Vtbl *lpVtbl;
    };

    

#ifdef COBJMACROS


#define FileCodeModel2_QueryInterface(This,riid,ppvObj,retval)	\
    ( (This)->lpVtbl -> QueryInterface(This,riid,ppvObj,retval) ) 

#define FileCodeModel2_AddRef(This,retval)	\
    ( (This)->lpVtbl -> AddRef(This,retval) ) 

#define FileCodeModel2_Release(This,retval)	\
    ( (This)->lpVtbl -> Release(This,retval) ) 

#define FileCodeModel2_GetTypeInfoCount(This,pctinfo,retval)	\
    ( (This)->lpVtbl -> GetTypeInfoCount(This,pctinfo,retval) ) 

#define FileCodeModel2_GetTypeInfo(This,itinfo,lcid,pptinfo,retval)	\
    ( (This)->lpVtbl -> GetTypeInfo(This,itinfo,lcid,pptinfo,retval) ) 

#define FileCodeModel2_GetIDsOfNames(This,riid,rgszNames,cNames,lcid,rgdispid,retval)	\
    ( (This)->lpVtbl -> GetIDsOfNames(This,riid,rgszNames,cNames,lcid,rgdispid,retval) ) 

#define FileCodeModel2_Invoke(This,dispidMember,riid,lcid,wFlags,pdispparams,pvarResult,pexcepinfo,puArgErr,retval)	\
    ( (This)->lpVtbl -> Invoke(This,dispidMember,riid,lcid,wFlags,pdispparams,pvarResult,pexcepinfo,puArgErr,retval) ) 

#define FileCodeModel2_get_DTE(This,retval)	\
    ( (This)->lpVtbl -> get_DTE(This,retval) ) 

#define FileCodeModel2_get_Parent(This,retval)	\
    ( (This)->lpVtbl -> get_Parent(This,retval) ) 

#define FileCodeModel2_get_Language(This,retval)	\
    ( (This)->lpVtbl -> get_Language(This,retval) ) 

#define FileCodeModel2_get_CodeElements(This,retval)	\
    ( (This)->lpVtbl -> get_CodeElements(This,retval) ) 

#define FileCodeModel2_CodeElementFromPoint(This,Point,Scope,retval)	\
    ( (This)->lpVtbl -> CodeElementFromPoint(This,Point,Scope,retval) ) 

#define FileCodeModel2_AddNamespace(This,Name,Position,retval)	\
    ( (This)->lpVtbl -> AddNamespace(This,Name,Position,retval) ) 

#define FileCodeModel2_AddClass(This,Name,Position,Bases,ImplementedInterfaces,Access,retval)	\
    ( (This)->lpVtbl -> AddClass(This,Name,Position,Bases,ImplementedInterfaces,Access,retval) ) 

#define FileCodeModel2_AddInterface(This,Name,Position,Bases,Access,retval)	\
    ( (This)->lpVtbl -> AddInterface(This,Name,Position,Bases,Access,retval) ) 

#define FileCodeModel2_AddFunction(This,Name,Kind,Type,Position,Access,retval)	\
    ( (This)->lpVtbl -> AddFunction(This,Name,Kind,Type,Position,Access,retval) ) 

#define FileCodeModel2_AddVariable(This,Name,Type,Position,Access,retval)	\
    ( (This)->lpVtbl -> AddVariable(This,Name,Type,Position,Access,retval) ) 

#define FileCodeModel2_AddAttribute(This,Name,Value,Position,retval)	\
    ( (This)->lpVtbl -> AddAttribute(This,Name,Value,Position,retval) ) 

#define FileCodeModel2_AddStruct(This,Name,Position,Bases,ImplementedInterfaces,Access,retval)	\
    ( (This)->lpVtbl -> AddStruct(This,Name,Position,Bases,ImplementedInterfaces,Access,retval) ) 

#define FileCodeModel2_AddEnum(This,Name,Position,Bases,Access,retval)	\
    ( (This)->lpVtbl -> AddEnum(This,Name,Position,Bases,Access,retval) ) 

#define FileCodeModel2_AddDelegate(This,Name,Type,Position,Access,retval)	\
    ( (This)->lpVtbl -> AddDelegate(This,Name,Type,Position,Access,retval) ) 

#define FileCodeModel2_Remove(This,Element,retval)	\
    ( (This)->lpVtbl -> Remove(This,Element,retval) ) 


#define FileCodeModel2_Synchronize(This)	\
    ( (This)->lpVtbl -> Synchronize(This) ) 

#define FileCodeModel2_get_ParseStatus(This,Status)	\
    ( (This)->lpVtbl -> get_ParseStatus(This,Status) ) 

#define FileCodeModel2_AddImport(This,Name,Position,Alias,ppImport)	\
    ( (This)->lpVtbl -> AddImport(This,Name,Position,Alias,ppImport) ) 

#define FileCodeModel2_BeginBatch(This)	\
    ( (This)->lpVtbl -> BeginBatch(This) ) 

#define FileCodeModel2_EndBatch(This)	\
    ( (This)->lpVtbl -> EndBatch(This) ) 

#define FileCodeModel2_get_IsBatchOpen(This,Open)	\
    ( (This)->lpVtbl -> get_IsBatchOpen(This,Open) ) 

#define FileCodeModel2_ElementFromID(This,ID,ppElement)	\
    ( (This)->lpVtbl -> ElementFromID(This,ID,ppElement) ) 

#endif /* COBJMACROS */


#endif 	/* C style interface */




#endif 	/* __FileCodeModel2_INTERFACE_DEFINED__ */


#ifndef __CodeModel2_INTERFACE_DEFINED__
#define __CodeModel2_INTERFACE_DEFINED__

/* interface CodeModel2 */
/* [helpstringcontext][helpstring][helpcontext][uuid][object][oleautomation][dual] */ 


EXTERN_C const IID IID_CodeModel2;

#if defined(__cplusplus) && !defined(CINTERFACE)
    
    MIDL_INTERFACE("99B9CD0E-6C89-4BC4-BBA2-FFD3529D3ACB")
    CodeModel2 : public CodeModel
    {
    public:
        virtual /* [helpstringcontext][helpstring][helpcontext][id] */ HRESULT STDMETHODCALLTYPE Synchronize( void) = 0;
        
        virtual /* [helpstringcontext][helpstring][helpcontext][id] */ HRESULT STDMETHODCALLTYPE DotNetNameFromLanguageSpecific( 
            __RPC__in BSTR LanguageName,
            /* [retval][out] */ __RPC__deref_out_opt BSTR *DotNETName) = 0;
        
        virtual /* [helpstringcontext][helpstring][helpcontext][id] */ HRESULT STDMETHODCALLTYPE LanguageSpecificNameFromDotNet( 
            __RPC__in BSTR DotNETName,
            /* [retval][out] */ __RPC__deref_out_opt BSTR *LanguageName) = 0;
        
        virtual /* [helpstringcontext][helpstring][helpcontext][id] */ HRESULT STDMETHODCALLTYPE ElementFromID( 
            __RPC__in BSTR ID,
            /* [retval][out] */ __RPC__deref_out_opt /* external definition not present */ CodeElement **pElement) = 0;
        
    };
    
#else 	/* C style interface */

    typedef struct CodeModel2Vtbl
    {
        BEGIN_INTERFACE
        
        /* [id][restricted][funcdescattr] */ HRESULT ( STDMETHODCALLTYPE *QueryInterface )( 
            CodeModel2 * This,
            /* [in][idldescattr] */ __RPC__in struct GUID *riid,
            /* [out][idldescattr] */ __RPC__deref_out_opt void **ppvObj,
            /* [retval][out] */ __RPC__out void *retval);
        
        /* [id][restricted][funcdescattr] */ HRESULT ( STDMETHODCALLTYPE *AddRef )( 
            CodeModel2 * This,
            /* [retval][out] */ __RPC__out unsigned long *retval);
        
        /* [id][restricted][funcdescattr] */ HRESULT ( STDMETHODCALLTYPE *Release )( 
            CodeModel2 * This,
            /* [retval][out] */ __RPC__out unsigned long *retval);
        
        /* [id][restricted][funcdescattr] */ HRESULT ( STDMETHODCALLTYPE *GetTypeInfoCount )( 
            CodeModel2 * This,
            /* [out][idldescattr] */ __RPC__out unsigned UINT *pctinfo,
            /* [retval][out] */ __RPC__out void *retval);
        
        /* [id][restricted][funcdescattr] */ HRESULT ( STDMETHODCALLTYPE *GetTypeInfo )( 
            CodeModel2 * This,
            /* [in][idldescattr] */ unsigned UINT itinfo,
            /* [in][idldescattr] */ unsigned long lcid,
            /* [out][idldescattr] */ __RPC__deref_out_opt void **pptinfo,
            /* [retval][out] */ __RPC__out void *retval);
        
        /* [id][restricted][funcdescattr] */ HRESULT ( STDMETHODCALLTYPE *GetIDsOfNames )( 
            CodeModel2 * This,
            /* [in][idldescattr] */ __RPC__in struct GUID *riid,
            /* [in][idldescattr] */ __RPC__deref_in_opt signed char **rgszNames,
            /* [in][idldescattr] */ unsigned UINT cNames,
            /* [in][idldescattr] */ unsigned long lcid,
            /* [out][idldescattr] */ __RPC__out signed long *rgdispid,
            /* [retval][out] */ __RPC__out void *retval);
        
        /* [id][restricted][funcdescattr] */ HRESULT ( STDMETHODCALLTYPE *Invoke )( 
            CodeModel2 * This,
            /* [in][idldescattr] */ signed long dispidMember,
            /* [in][idldescattr] */ __RPC__in struct GUID *riid,
            /* [in][idldescattr] */ unsigned long lcid,
            /* [in][idldescattr] */ unsigned short wFlags,
            /* [in][idldescattr] */ __RPC__in struct DISPPARAMS *pdispparams,
            /* [out][idldescattr] */ __RPC__out VARIANT *pvarResult,
            /* [out][idldescattr] */ __RPC__out struct EXCEPINFO *pexcepinfo,
            /* [out][idldescattr] */ __RPC__out unsigned UINT *puArgErr,
            /* [retval][out] */ __RPC__out void *retval);
        
        /* [id][propget][funcdescattr] */ HRESULT ( STDMETHODCALLTYPE *get_DTE )( 
            CodeModel2 * This,
            /* [retval][out] */ __RPC__deref_out_opt **retval);
        
        /* [id][propget][funcdescattr] */ HRESULT ( STDMETHODCALLTYPE *get_Parent )( 
            CodeModel2 * This,
            /* [retval][out] */ __RPC__deref_out_opt Project **retval);
        
        /* [id][propget][funcdescattr] */ HRESULT ( STDMETHODCALLTYPE *get_Language )( 
            CodeModel2 * This,
            /* [retval][out] */ __RPC__deref_out_opt BSTR *retval);
        
        /* [id][propget][funcdescattr] */ HRESULT ( STDMETHODCALLTYPE *get_CodeElements )( 
            CodeModel2 * This,
            /* [retval][out] */ __RPC__deref_out_opt CodeElements **retval);
        
        /* [id][funcdescattr] */ HRESULT ( STDMETHODCALLTYPE *CodeTypeFromFullName )( 
            CodeModel2 * This,
            /* [idldescattr] */ __RPC__in BSTR Name,
            /* [retval][out] */ __RPC__deref_out_opt CodeType **retval);
        
        /* [id][funcdescattr] */ HRESULT ( STDMETHODCALLTYPE *AddNamespace )( 
            CodeModel2 * This,
            /* [idldescattr] */ __RPC__in BSTR Name,
            /* [idldescattr] */ VARIANT Location,
            /* [optional][idldescattr] */ VARIANT Position,
            /* [retval][out] */ __RPC__deref_out_opt CodeNamespace **retval);
        
        /* [id][funcdescattr] */ HRESULT ( STDMETHODCALLTYPE *AddClass )( 
            CodeModel2 * This,
            /* [idldescattr] */ __RPC__in BSTR Name,
            /* [idldescattr] */ VARIANT Location,
            /* [idldescattr] */ VARIANT Position,
            /* [idldescattr] */ VARIANT Bases,
            /* [idldescattr] */ VARIANT ImplementedInterfaces,
            /* [idldescattr] */ enum vsCMAccess Access,
            /* [retval][out] */ __RPC__deref_out_opt CodeClass **retval);
        
        /* [id][funcdescattr] */ HRESULT ( STDMETHODCALLTYPE *AddInterface )( 
            CodeModel2 * This,
            /* [idldescattr] */ __RPC__in BSTR Name,
            /* [idldescattr] */ VARIANT Location,
            /* [idldescattr] */ VARIANT Position,
            /* [idldescattr] */ VARIANT Bases,
            /* [idldescattr] */ enum vsCMAccess Access,
            /* [retval][out] */ __RPC__deref_out_opt CodeInterface **retval);
        
        /* [id][funcdescattr] */ HRESULT ( STDMETHODCALLTYPE *AddFunction )( 
            CodeModel2 * This,
            /* [idldescattr] */ __RPC__in BSTR Name,
            /* [idldescattr] */ VARIANT Location,
            /* [idldescattr] */ enum vsCMFunction Kind,
            /* [idldescattr] */ VARIANT Type,
            /* [idldescattr] */ VARIANT Position,
            /* [idldescattr] */ enum vsCMAccess Access,
            /* [retval][out] */ __RPC__deref_out_opt CodeFunction **retval);
        
        /* [id][funcdescattr] */ HRESULT ( STDMETHODCALLTYPE *AddVariable )( 
            CodeModel2 * This,
            /* [idldescattr] */ __RPC__in BSTR Name,
            /* [idldescattr] */ VARIANT Location,
            /* [idldescattr] */ VARIANT Type,
            /* [idldescattr] */ VARIANT Position,
            /* [idldescattr] */ enum vsCMAccess Access,
            /* [retval][out] */ __RPC__deref_out_opt CodeVariable **retval);
        
        /* [id][funcdescattr] */ HRESULT ( STDMETHODCALLTYPE *AddStruct )( 
            CodeModel2 * This,
            /* [idldescattr] */ __RPC__in BSTR Name,
            /* [idldescattr] */ VARIANT Location,
            /* [idldescattr] */ VARIANT Position,
            /* [idldescattr] */ VARIANT Bases,
            /* [idldescattr] */ VARIANT ImplementedInterfaces,
            /* [idldescattr] */ enum vsCMAccess Access,
            /* [retval][out] */ __RPC__deref_out_opt CodeStruct **retval);
        
        /* [id][funcdescattr] */ HRESULT ( STDMETHODCALLTYPE *AddEnum )( 
            CodeModel2 * This,
            /* [idldescattr] */ __RPC__in BSTR Name,
            /* [idldescattr] */ VARIANT Location,
            /* [idldescattr] */ VARIANT Position,
            /* [idldescattr] */ VARIANT Bases,
            /* [idldescattr] */ enum vsCMAccess Access,
            /* [retval][out] */ __RPC__deref_out_opt CodeEnum **retval);
        
        /* [id][funcdescattr] */ HRESULT ( STDMETHODCALLTYPE *AddDelegate )( 
            CodeModel2 * This,
            /* [idldescattr] */ __RPC__in BSTR Name,
            /* [idldescattr] */ VARIANT Location,
            /* [idldescattr] */ VARIANT Type,
            /* [idldescattr] */ VARIANT Position,
            /* [idldescattr] */ enum vsCMAccess Access,
            /* [retval][out] */ __RPC__deref_out_opt CodeDelegate **retval);
        
        /* [id][funcdescattr] */ HRESULT ( STDMETHODCALLTYPE *AddAttribute )( 
            CodeModel2 * This,
            /* [idldescattr] */ __RPC__in BSTR Name,
            /* [idldescattr] */ VARIANT Location,
            /* [idldescattr] */ __RPC__in BSTR Value,
            /* [optional][idldescattr] */ VARIANT Position,
            /* [retval][out] */ __RPC__deref_out_opt CodeAttribute **retval);
        
        /* [id][funcdescattr] */ HRESULT ( STDMETHODCALLTYPE *Remove )( 
            CodeModel2 * This,
            /* [idldescattr] */ VARIANT Element,
            /* [retval][out] */ __RPC__out void *retval);
        
        /* [id][funcdescattr] */ HRESULT ( STDMETHODCALLTYPE *IsValidID )( 
            CodeModel2 * This,
            /* [idldescattr] */ __RPC__in BSTR Name,
            /* [retval][out] */ __RPC__out BOOLEAN *retval);
        
        /* [id][propget][funcdescattr] */ HRESULT ( STDMETHODCALLTYPE *get_IsCaseSensitive )( 
            CodeModel2 * This,
            /* [retval][out] */ __RPC__out BOOLEAN *retval);
        
        /* [id][funcdescattr] */ HRESULT ( STDMETHODCALLTYPE *CreateCodeTypeRef )( 
            CodeModel2 * This,
            /* [idldescattr] */ VARIANT Type,
            /* [retval][out] */ __RPC__deref_out_opt CodeTypeRef **retval);
        
        /* [helpstringcontext][helpstring][helpcontext][id] */ HRESULT ( STDMETHODCALLTYPE *Synchronize )( 
            CodeModel2 * This);
        
        /* [helpstringcontext][helpstring][helpcontext][id] */ HRESULT ( STDMETHODCALLTYPE *DotNetNameFromLanguageSpecific )( 
            CodeModel2 * This,
            __RPC__in BSTR LanguageName,
            /* [retval][out] */ __RPC__deref_out_opt BSTR *DotNETName);
        
        /* [helpstringcontext][helpstring][helpcontext][id] */ HRESULT ( STDMETHODCALLTYPE *LanguageSpecificNameFromDotNet )( 
            CodeModel2 * This,
            __RPC__in BSTR DotNETName,
            /* [retval][out] */ __RPC__deref_out_opt BSTR *LanguageName);
        
        /* [helpstringcontext][helpstring][helpcontext][id] */ HRESULT ( STDMETHODCALLTYPE *ElementFromID )( 
            CodeModel2 * This,
            __RPC__in BSTR ID,
            /* [retval][out] */ __RPC__deref_out_opt /* external definition not present */ CodeElement **pElement);
        
        END_INTERFACE
    } CodeModel2Vtbl;

    interface CodeModel2
    {
        CONST_VTBL struct CodeModel2Vtbl *lpVtbl;
    };

    

#ifdef COBJMACROS


#define CodeModel2_QueryInterface(This,riid,ppvObj,retval)	\
    ( (This)->lpVtbl -> QueryInterface(This,riid,ppvObj,retval) ) 

#define CodeModel2_AddRef(This,retval)	\
    ( (This)->lpVtbl -> AddRef(This,retval) ) 

#define CodeModel2_Release(This,retval)	\
    ( (This)->lpVtbl -> Release(This,retval) ) 

#define CodeModel2_GetTypeInfoCount(This,pctinfo,retval)	\
    ( (This)->lpVtbl -> GetTypeInfoCount(This,pctinfo,retval) ) 

#define CodeModel2_GetTypeInfo(This,itinfo,lcid,pptinfo,retval)	\
    ( (This)->lpVtbl -> GetTypeInfo(This,itinfo,lcid,pptinfo,retval) ) 

#define CodeModel2_GetIDsOfNames(This,riid,rgszNames,cNames,lcid,rgdispid,retval)	\
    ( (This)->lpVtbl -> GetIDsOfNames(This,riid,rgszNames,cNames,lcid,rgdispid,retval) ) 

#define CodeModel2_Invoke(This,dispidMember,riid,lcid,wFlags,pdispparams,pvarResult,pexcepinfo,puArgErr,retval)	\
    ( (This)->lpVtbl -> Invoke(This,dispidMember,riid,lcid,wFlags,pdispparams,pvarResult,pexcepinfo,puArgErr,retval) ) 

#define CodeModel2_get_DTE(This,retval)	\
    ( (This)->lpVtbl -> get_DTE(This,retval) ) 

#define CodeModel2_get_Parent(This,retval)	\
    ( (This)->lpVtbl -> get_Parent(This,retval) ) 

#define CodeModel2_get_Language(This,retval)	\
    ( (This)->lpVtbl -> get_Language(This,retval) ) 

#define CodeModel2_get_CodeElements(This,retval)	\
    ( (This)->lpVtbl -> get_CodeElements(This,retval) ) 

#define CodeModel2_CodeTypeFromFullName(This,Name,retval)	\
    ( (This)->lpVtbl -> CodeTypeFromFullName(This,Name,retval) ) 

#define CodeModel2_AddNamespace(This,Name,Location,Position,retval)	\
    ( (This)->lpVtbl -> AddNamespace(This,Name,Location,Position,retval) ) 

#define CodeModel2_AddClass(This,Name,Location,Position,Bases,ImplementedInterfaces,Access,retval)	\
    ( (This)->lpVtbl -> AddClass(This,Name,Location,Position,Bases,ImplementedInterfaces,Access,retval) ) 

#define CodeModel2_AddInterface(This,Name,Location,Position,Bases,Access,retval)	\
    ( (This)->lpVtbl -> AddInterface(This,Name,Location,Position,Bases,Access,retval) ) 

#define CodeModel2_AddFunction(This,Name,Location,Kind,Type,Position,Access,retval)	\
    ( (This)->lpVtbl -> AddFunction(This,Name,Location,Kind,Type,Position,Access,retval) ) 

#define CodeModel2_AddVariable(This,Name,Location,Type,Position,Access,retval)	\
    ( (This)->lpVtbl -> AddVariable(This,Name,Location,Type,Position,Access,retval) ) 

#define CodeModel2_AddStruct(This,Name,Location,Position,Bases,ImplementedInterfaces,Access,retval)	\
    ( (This)->lpVtbl -> AddStruct(This,Name,Location,Position,Bases,ImplementedInterfaces,Access,retval) ) 

#define CodeModel2_AddEnum(This,Name,Location,Position,Bases,Access,retval)	\
    ( (This)->lpVtbl -> AddEnum(This,Name,Location,Position,Bases,Access,retval) ) 

#define CodeModel2_AddDelegate(This,Name,Location,Type,Position,Access,retval)	\
    ( (This)->lpVtbl -> AddDelegate(This,Name,Location,Type,Position,Access,retval) ) 

#define CodeModel2_AddAttribute(This,Name,Location,Value,Position,retval)	\
    ( (This)->lpVtbl -> AddAttribute(This,Name,Location,Value,Position,retval) ) 

#define CodeModel2_Remove(This,Element,retval)	\
    ( (This)->lpVtbl -> Remove(This,Element,retval) ) 

#define CodeModel2_IsValidID(This,Name,retval)	\
    ( (This)->lpVtbl -> IsValidID(This,Name,retval) ) 

#define CodeModel2_get_IsCaseSensitive(This,retval)	\
    ( (This)->lpVtbl -> get_IsCaseSensitive(This,retval) ) 

#define CodeModel2_CreateCodeTypeRef(This,Type,retval)	\
    ( (This)->lpVtbl -> CreateCodeTypeRef(This,Type,retval) ) 


#define CodeModel2_Synchronize(This)	\
    ( (This)->lpVtbl -> Synchronize(This) ) 

#define CodeModel2_DotNetNameFromLanguageSpecific(This,LanguageName,DotNETName)	\
    ( (This)->lpVtbl -> DotNetNameFromLanguageSpecific(This,LanguageName,DotNETName) ) 

#define CodeModel2_LanguageSpecificNameFromDotNet(This,DotNETName,LanguageName)	\
    ( (This)->lpVtbl -> LanguageSpecificNameFromDotNet(This,DotNETName,LanguageName) ) 

#define CodeModel2_ElementFromID(This,ID,pElement)	\
    ( (This)->lpVtbl -> ElementFromID(This,ID,pElement) ) 

#endif /* COBJMACROS */


#endif 	/* C style interface */




#endif 	/* __CodeModel2_INTERFACE_DEFINED__ */


#ifndef __CodeClass2_INTERFACE_DEFINED__
#define __CodeClass2_INTERFACE_DEFINED__

/* interface CodeClass2 */
/* [helpstringcontext][helpstring][helpcontext][uuid][object][oleautomation][dual] */ 


EXTERN_C const IID IID_CodeClass2;

#if defined(__cplusplus) && !defined(CINTERFACE)
    
    MIDL_INTERFACE("295ADCD4-B052-49EE-934E-C6B36862A7C6")
    CodeClass2 : public CodeClass
    {
    public:
        virtual /* [helpstringcontext][helpstring][helpcontext][hidden][propget][id] */ HRESULT STDMETHODCALLTYPE get_ClassKind( 
            /* [retval][out] */ __RPC__out vsCMClassKind *Kind) = 0;
        
        virtual /* [helpstringcontext][helpstring][helpcontext][hidden][propput][id] */ HRESULT STDMETHODCALLTYPE put_ClassKind( 
            vsCMClassKind Kind) = 0;
        
        virtual /* [helpstringcontext][helpstring][helpcontext][hidden][propget][id] */ HRESULT STDMETHODCALLTYPE get_PartialClasses( 
            /* [retval][out] */ __RPC__deref_out_opt /* external definition not present */ CodeElements **Elements) = 0;
        
        virtual /* [helpstringcontext][helpstring][helpcontext][propget][id] */ HRESULT STDMETHODCALLTYPE get_DataTypeKind( 
            /* [retval][out] */ __RPC__out vsCMDataTypeKind *Kind) = 0;
        
        virtual /* [helpstringcontext][helpstring][helpcontext][propput][id] */ HRESULT STDMETHODCALLTYPE put_DataTypeKind( 
            vsCMDataTypeKind Kind) = 0;
        
        virtual /* [helpstringcontext][helpstring][helpcontext][propget][id] */ HRESULT STDMETHODCALLTYPE get_Parts( 
            /* [retval][out] */ __RPC__deref_out_opt /* external definition not present */ CodeElements **Elements) = 0;
        
        virtual /* [helpstringcontext][helpstring][helpcontext][propget][id] */ HRESULT STDMETHODCALLTYPE get_InheritanceKind( 
            /* [retval][out] */ __RPC__out vsCMInheritanceKind *Kind) = 0;
        
        virtual /* [helpstringcontext][helpstring][helpcontext][propput][id] */ HRESULT STDMETHODCALLTYPE put_InheritanceKind( 
            vsCMInheritanceKind Kind) = 0;
        
        virtual /* [helpstring][propget][id] */ HRESULT STDMETHODCALLTYPE get_IsGeneric( 
            /* [retval][out] */ __RPC__out VARIANT_BOOL *pIsGeneric) = 0;
        
        virtual /* [helpstring][id] */ HRESULT STDMETHODCALLTYPE AddEvent( 
            __RPC__in BSTR Name,
            __RPC__in BSTR FullDelegateName,
            /* [defaultvalue][in] */ VARIANT_BOOL CreatePropertyStyleEvent,
            /* [optional] */ VARIANT Location,
            /* [defaultvalue] */ enum /* external definition not present */ vsCMAccess Access,
            /* [retval][out] */ __RPC__deref_out_opt CodeEvent **ppEvent) = 0;
        
        virtual /* [helpstringcontext][helpstring][helpcontext][propget][id] */ HRESULT STDMETHODCALLTYPE get_IsShared( 
            /* [retval][out] */ __RPC__out VARIANT_BOOL *pIsShared) = 0;
        
        virtual /* [helpstringcontext][helpstring][helpcontext][propput][id] */ HRESULT STDMETHODCALLTYPE put_IsShared( 
            VARIANT_BOOL Shared) = 0;
        
    };
    
#else 	/* C style interface */

    typedef struct CodeClass2Vtbl
    {
        BEGIN_INTERFACE
        
        /* [id][restricted][funcdescattr] */ HRESULT ( STDMETHODCALLTYPE *QueryInterface )( 
            CodeClass2 * This,
            /* [in][idldescattr] */ __RPC__in struct GUID *riid,
            /* [out][idldescattr] */ __RPC__deref_out_opt void **ppvObj,
            /* [retval][out] */ __RPC__out void *retval);
        
        /* [id][restricted][funcdescattr] */ HRESULT ( STDMETHODCALLTYPE *AddRef )( 
            CodeClass2 * This,
            /* [retval][out] */ __RPC__out unsigned long *retval);
        
        /* [id][restricted][funcdescattr] */ HRESULT ( STDMETHODCALLTYPE *Release )( 
            CodeClass2 * This,
            /* [retval][out] */ __RPC__out unsigned long *retval);
        
        /* [id][restricted][funcdescattr] */ HRESULT ( STDMETHODCALLTYPE *GetTypeInfoCount )( 
            CodeClass2 * This,
            /* [out][idldescattr] */ __RPC__out unsigned UINT *pctinfo,
            /* [retval][out] */ __RPC__out void *retval);
        
        /* [id][restricted][funcdescattr] */ HRESULT ( STDMETHODCALLTYPE *GetTypeInfo )( 
            CodeClass2 * This,
            /* [in][idldescattr] */ unsigned UINT itinfo,
            /* [in][idldescattr] */ unsigned long lcid,
            /* [out][idldescattr] */ __RPC__deref_out_opt void **pptinfo,
            /* [retval][out] */ __RPC__out void *retval);
        
        /* [id][restricted][funcdescattr] */ HRESULT ( STDMETHODCALLTYPE *GetIDsOfNames )( 
            CodeClass2 * This,
            /* [in][idldescattr] */ __RPC__in struct GUID *riid,
            /* [in][idldescattr] */ __RPC__deref_in_opt signed char **rgszNames,
            /* [in][idldescattr] */ unsigned UINT cNames,
            /* [in][idldescattr] */ unsigned long lcid,
            /* [out][idldescattr] */ __RPC__out signed long *rgdispid,
            /* [retval][out] */ __RPC__out void *retval);
        
        /* [id][restricted][funcdescattr] */ HRESULT ( STDMETHODCALLTYPE *Invoke )( 
            CodeClass2 * This,
            /* [in][idldescattr] */ signed long dispidMember,
            /* [in][idldescattr] */ __RPC__in struct GUID *riid,
            /* [in][idldescattr] */ unsigned long lcid,
            /* [in][idldescattr] */ unsigned short wFlags,
            /* [in][idldescattr] */ __RPC__in struct DISPPARAMS *pdispparams,
            /* [out][idldescattr] */ __RPC__out VARIANT *pvarResult,
            /* [out][idldescattr] */ __RPC__out struct EXCEPINFO *pexcepinfo,
            /* [out][idldescattr] */ __RPC__out unsigned UINT *puArgErr,
            /* [retval][out] */ __RPC__out void *retval);
        
        /* [id][propget][funcdescattr] */ HRESULT ( STDMETHODCALLTYPE *get_DTE )( 
            CodeClass2 * This,
            /* [retval][out] */ __RPC__deref_out_opt **retval);
        
        /* [id][propget][funcdescattr] */ HRESULT ( STDMETHODCALLTYPE *get_Collection )( 
            CodeClass2 * This,
            /* [retval][out] */ __RPC__deref_out_opt CodeElements **retval);
        
        /* [id][propget][funcdescattr] */ HRESULT ( STDMETHODCALLTYPE *get_Name )( 
            CodeClass2 * This,
            /* [retval][out] */ __RPC__deref_out_opt BSTR *retval);
        
        /* [id][propput][funcdescattr] */ HRESULT ( STDMETHODCALLTYPE *put_Name )( 
            CodeClass2 * This,
            /* [idldescattr] */ __RPC__in BSTR noname,
            /* [retval][out] */ __RPC__out void *retval);
        
        /* [id][propget][funcdescattr] */ HRESULT ( STDMETHODCALLTYPE *get_FullName )( 
            CodeClass2 * This,
            /* [retval][out] */ __RPC__deref_out_opt BSTR *retval);
        
        /* [id][propget][funcdescattr] */ HRESULT ( STDMETHODCALLTYPE *get_ProjectItem )( 
            CodeClass2 * This,
            /* [retval][out] */ __RPC__deref_out_opt ProjectItem **retval);
        
        /* [id][propget][funcdescattr] */ HRESULT ( STDMETHODCALLTYPE *get_Kind )( 
            CodeClass2 * This,
            /* [retval][out] */ __RPC__out enum vsCMElement *retval);
        
        /* [id][propget][funcdescattr] */ HRESULT ( STDMETHODCALLTYPE *get_IsCodeType )( 
            CodeClass2 * This,
            /* [retval][out] */ __RPC__out BOOLEAN *retval);
        
        /* [id][propget][funcdescattr] */ HRESULT ( STDMETHODCALLTYPE *get_InfoLocation )( 
            CodeClass2 * This,
            /* [retval][out] */ __RPC__out enum vsCMInfoLocation *retval);
        
        /* [id][propget][funcdescattr] */ HRESULT ( STDMETHODCALLTYPE *get_Children )( 
            CodeClass2 * This,
            /* [retval][out] */ __RPC__deref_out_opt CodeElements **retval);
        
        /* [id][propget][funcdescattr] */ HRESULT ( STDMETHODCALLTYPE *get_Language )( 
            CodeClass2 * This,
            /* [retval][out] */ __RPC__deref_out_opt BSTR *retval);
        
        /* [id][propget][funcdescattr] */ HRESULT ( STDMETHODCALLTYPE *get_StartPoint )( 
            CodeClass2 * This,
            /* [retval][out] */ __RPC__deref_out_opt TextPoint **retval);
        
        /* [id][propget][funcdescattr] */ HRESULT ( STDMETHODCALLTYPE *get_EndPoint )( 
            CodeClass2 * This,
            /* [retval][out] */ __RPC__deref_out_opt TextPoint **retval);
        
        /* [id][propget][funcdescattr] */ HRESULT ( STDMETHODCALLTYPE *get_ExtenderNames )( 
            CodeClass2 * This,
            /* [retval][out] */ __RPC__out VARIANT *retval);
        
        /* [id][propget][funcdescattr] */ HRESULT ( STDMETHODCALLTYPE *get_Extender )( 
            CodeClass2 * This,
            /* [idldescattr] */ __RPC__in BSTR ExtenderName,
            /* [retval][out] */ __RPC__deref_out_opt IDispatch **retval);
        
        /* [id][propget][funcdescattr] */ HRESULT ( STDMETHODCALLTYPE *get_ExtenderCATID )( 
            CodeClass2 * This,
            /* [retval][out] */ __RPC__deref_out_opt BSTR *retval);
        
        /* [id][funcdescattr] */ HRESULT ( STDMETHODCALLTYPE *GetStartPoint )( 
            CodeClass2 * This,
            /* [in][idldescattr] */ enum vsCMPart Part,
            /* [retval][out] */ __RPC__deref_out_opt TextPoint **retval);
        
        /* [id][funcdescattr] */ HRESULT ( STDMETHODCALLTYPE *GetEndPoint )( 
            CodeClass2 * This,
            /* [in][idldescattr] */ enum vsCMPart Part,
            /* [retval][out] */ __RPC__deref_out_opt TextPoint **retval);
        
        /* [id][propget][funcdescattr] */ HRESULT ( STDMETHODCALLTYPE *get_Parent )( 
            CodeClass2 * This,
            /* [retval][out] */ __RPC__deref_out_opt IDispatch **retval);
        
        /* [id][propget][funcdescattr] */ HRESULT ( STDMETHODCALLTYPE *get_Namespace )( 
            CodeClass2 * This,
            /* [retval][out] */ __RPC__deref_out_opt CodeNamespace **retval);
        
        /* [id][propget][funcdescattr] */ HRESULT ( STDMETHODCALLTYPE *get_Bases )( 
            CodeClass2 * This,
            /* [retval][out] */ __RPC__deref_out_opt CodeElements **retval);
        
        /* [id][propget][funcdescattr] */ HRESULT ( STDMETHODCALLTYPE *get_Members )( 
            CodeClass2 * This,
            /* [retval][out] */ __RPC__deref_out_opt CodeElements **retval);
        
        /* [id][propput][funcdescattr] */ HRESULT ( STDMETHODCALLTYPE *put_Access )( 
            CodeClass2 * This,
            /* [idldescattr] */ enum vsCMAccess noname,
            /* [retval][out] */ __RPC__out void *retval);
        
        /* [id][propget][funcdescattr] */ HRESULT ( STDMETHODCALLTYPE *get_Access )( 
            CodeClass2 * This,
            /* [retval][out] */ __RPC__out enum vsCMAccess *retval);
        
        /* [id][propget][funcdescattr] */ HRESULT ( STDMETHODCALLTYPE *get_Attributes )( 
            CodeClass2 * This,
            /* [retval][out] */ __RPC__deref_out_opt CodeElements **retval);
        
        /* [id][propget][funcdescattr] */ HRESULT ( STDMETHODCALLTYPE *get_DocComment )( 
            CodeClass2 * This,
            /* [retval][out] */ __RPC__deref_out_opt BSTR *retval);
        
        /* [id][propput][funcdescattr] */ HRESULT ( STDMETHODCALLTYPE *put_DocComment )( 
            CodeClass2 * This,
            /* [idldescattr] */ __RPC__in BSTR noname,
            /* [retval][out] */ __RPC__out void *retval);
        
        /* [id][propget][funcdescattr] */ HRESULT ( STDMETHODCALLTYPE *get_Comment )( 
            CodeClass2 * This,
            /* [retval][out] */ __RPC__deref_out_opt BSTR *retval);
        
        /* [id][propput][funcdescattr] */ HRESULT ( STDMETHODCALLTYPE *put_Comment )( 
            CodeClass2 * This,
            /* [idldescattr] */ __RPC__in BSTR noname,
            /* [retval][out] */ __RPC__out void *retval);
        
        /* [id][funcdescattr] */ HRESULT ( STDMETHODCALLTYPE *AddBase )( 
            CodeClass2 * This,
            /* [idldescattr] */ VARIANT Base,
            /* [optional][idldescattr] */ VARIANT Position,
            /* [retval][out] */ __RPC__deref_out_opt CodeElement **retval);
        
        /* [id][funcdescattr] */ HRESULT ( STDMETHODCALLTYPE *AddAttribute )( 
            CodeClass2 * This,
            /* [idldescattr] */ __RPC__in BSTR Name,
            /* [idldescattr] */ __RPC__in BSTR Value,
            /* [optional][idldescattr] */ VARIANT Position,
            /* [retval][out] */ __RPC__deref_out_opt CodeAttribute **retval);
        
        /* [id][funcdescattr] */ HRESULT ( STDMETHODCALLTYPE *RemoveBase )( 
            CodeClass2 * This,
            /* [idldescattr] */ VARIANT Element,
            /* [retval][out] */ __RPC__out void *retval);
        
        /* [id][funcdescattr] */ HRESULT ( STDMETHODCALLTYPE *RemoveMember )( 
            CodeClass2 * This,
            /* [idldescattr] */ VARIANT Element,
            /* [retval][out] */ __RPC__out void *retval);
        
        /* [id][propget][funcdescattr] */ HRESULT ( STDMETHODCALLTYPE *get_IsDerivedFrom )( 
            CodeClass2 * This,
            /* [idldescattr] */ __RPC__in BSTR FullName,
            /* [retval][out] */ __RPC__out BOOLEAN *retval);
        
        /* [id][propget][funcdescattr] */ HRESULT ( STDMETHODCALLTYPE *get_DerivedTypes )( 
            CodeClass2 * This,
            /* [retval][out] */ __RPC__deref_out_opt CodeElements **retval);
        
        /* [id][propget][funcdescattr] */ HRESULT ( STDMETHODCALLTYPE *get_ImplementedInterfaces )( 
            CodeClass2 * This,
            /* [retval][out] */ __RPC__deref_out_opt CodeElements **retval);
        
        /* [id][propget][funcdescattr] */ HRESULT ( STDMETHODCALLTYPE *get_IsAbstract )( 
            CodeClass2 * This,
            /* [retval][out] */ __RPC__out BOOLEAN *retval);
        
        /* [id][propput][funcdescattr] */ HRESULT ( STDMETHODCALLTYPE *put_IsAbstract )( 
            CodeClass2 * This,
            /* [idldescattr] */ BOOLEAN noname,
            /* [retval][out] */ __RPC__out void *retval);
        
        /* [id][funcdescattr] */ HRESULT ( STDMETHODCALLTYPE *AddImplementedInterface )( 
            CodeClass2 * This,
            /* [idldescattr] */ VARIANT Base,
            /* [optional][idldescattr] */ VARIANT Position,
            /* [retval][out] */ __RPC__deref_out_opt CodeInterface **retval);
        
        /* [id][funcdescattr] */ HRESULT ( STDMETHODCALLTYPE *AddFunction )( 
            CodeClass2 * This,
            /* [idldescattr] */ __RPC__in BSTR Name,
            /* [idldescattr] */ enum vsCMFunction Kind,
            /* [idldescattr] */ VARIANT Type,
            /* [idldescattr] */ VARIANT Position,
            /* [idldescattr] */ enum vsCMAccess Access,
            /* [optional][idldescattr] */ VARIANT Location,
            /* [retval][out] */ __RPC__deref_out_opt CodeFunction **retval);
        
        /* [id][funcdescattr] */ HRESULT ( STDMETHODCALLTYPE *AddVariable )( 
            CodeClass2 * This,
            /* [idldescattr] */ __RPC__in BSTR Name,
            /* [idldescattr] */ VARIANT Type,
            /* [idldescattr] */ VARIANT Position,
            /* [idldescattr] */ enum vsCMAccess Access,
            /* [optional][idldescattr] */ VARIANT Location,
            /* [retval][out] */ __RPC__deref_out_opt CodeVariable **retval);
        
        /* [id][funcdescattr] */ HRESULT ( STDMETHODCALLTYPE *AddProperty )( 
            CodeClass2 * This,
            /* [idldescattr] */ __RPC__in BSTR GetterName,
            /* [idldescattr] */ __RPC__in BSTR PutterName,
            /* [idldescattr] */ VARIANT Type,
            /* [idldescattr] */ VARIANT Position,
            /* [idldescattr] */ enum vsCMAccess Access,
            /* [optional][idldescattr] */ VARIANT Location,
            /* [retval][out] */ __RPC__deref_out_opt CodeProperty **retval);
        
        /* [id][funcdescattr] */ HRESULT ( STDMETHODCALLTYPE *AddClass )( 
            CodeClass2 * This,
            /* [idldescattr] */ __RPC__in BSTR Name,
            /* [idldescattr] */ VARIANT Position,
            /* [idldescattr] */ VARIANT Bases,
            /* [idldescattr] */ VARIANT ImplementedInterfaces,
            /* [idldescattr] */ enum vsCMAccess Access,
            /* [retval][out] */ __RPC__deref_out_opt CodeClass **retval);
        
        /* [id][funcdescattr] */ HRESULT ( STDMETHODCALLTYPE *AddStruct )( 
            CodeClass2 * This,
            /* [idldescattr] */ __RPC__in BSTR Name,
            /* [idldescattr] */ VARIANT Position,
            /* [idldescattr] */ VARIANT Bases,
            /* [idldescattr] */ VARIANT ImplementedInterfaces,
            /* [idldescattr] */ enum vsCMAccess Access,
            /* [retval][out] */ __RPC__deref_out_opt CodeStruct **retval);
        
        /* [id][funcdescattr] */ HRESULT ( STDMETHODCALLTYPE *AddEnum )( 
            CodeClass2 * This,
            /* [idldescattr] */ __RPC__in BSTR Name,
            /* [idldescattr] */ VARIANT Position,
            /* [idldescattr] */ VARIANT Bases,
            /* [idldescattr] */ enum vsCMAccess Access,
            /* [retval][out] */ __RPC__deref_out_opt CodeEnum **retval);
        
        /* [id][funcdescattr] */ HRESULT ( STDMETHODCALLTYPE *AddDelegate )( 
            CodeClass2 * This,
            /* [idldescattr] */ __RPC__in BSTR Name,
            /* [idldescattr] */ VARIANT Type,
            /* [idldescattr] */ VARIANT Position,
            /* [idldescattr] */ enum vsCMAccess Access,
            /* [retval][out] */ __RPC__deref_out_opt CodeDelegate **retval);
        
        /* [id][funcdescattr] */ HRESULT ( STDMETHODCALLTYPE *RemoveInterface )( 
            CodeClass2 * This,
            /* [idldescattr] */ VARIANT Element,
            /* [retval][out] */ __RPC__out void *retval);
        
        /* [helpstringcontext][helpstring][helpcontext][hidden][propget][id] */ HRESULT ( STDMETHODCALLTYPE *get_ClassKind )( 
            CodeClass2 * This,
            /* [retval][out] */ __RPC__out vsCMClassKind *Kind);
        
        /* [helpstringcontext][helpstring][helpcontext][hidden][propput][id] */ HRESULT ( STDMETHODCALLTYPE *put_ClassKind )( 
            CodeClass2 * This,
            vsCMClassKind Kind);
        
        /* [helpstringcontext][helpstring][helpcontext][hidden][propget][id] */ HRESULT ( STDMETHODCALLTYPE *get_PartialClasses )( 
            CodeClass2 * This,
            /* [retval][out] */ __RPC__deref_out_opt /* external definition not present */ CodeElements **Elements);
        
        /* [helpstringcontext][helpstring][helpcontext][propget][id] */ HRESULT ( STDMETHODCALLTYPE *get_DataTypeKind )( 
            CodeClass2 * This,
            /* [retval][out] */ __RPC__out vsCMDataTypeKind *Kind);
        
        /* [helpstringcontext][helpstring][helpcontext][propput][id] */ HRESULT ( STDMETHODCALLTYPE *put_DataTypeKind )( 
            CodeClass2 * This,
            vsCMDataTypeKind Kind);
        
        /* [helpstringcontext][helpstring][helpcontext][propget][id] */ HRESULT ( STDMETHODCALLTYPE *get_Parts )( 
            CodeClass2 * This,
            /* [retval][out] */ __RPC__deref_out_opt /* external definition not present */ CodeElements **Elements);
        
        /* [helpstringcontext][helpstring][helpcontext][propget][id] */ HRESULT ( STDMETHODCALLTYPE *get_InheritanceKind )( 
            CodeClass2 * This,
            /* [retval][out] */ __RPC__out vsCMInheritanceKind *Kind);
        
        /* [helpstringcontext][helpstring][helpcontext][propput][id] */ HRESULT ( STDMETHODCALLTYPE *put_InheritanceKind )( 
            CodeClass2 * This,
            vsCMInheritanceKind Kind);
        
        /* [helpstring][propget][id] */ HRESULT ( STDMETHODCALLTYPE *get_IsGeneric )( 
            CodeClass2 * This,
            /* [retval][out] */ __RPC__out VARIANT_BOOL *pIsGeneric);
        
        /* [helpstring][id] */ HRESULT ( STDMETHODCALLTYPE *AddEvent )( 
            CodeClass2 * This,
            __RPC__in BSTR Name,
            __RPC__in BSTR FullDelegateName,
            /* [defaultvalue][in] */ VARIANT_BOOL CreatePropertyStyleEvent,
            /* [optional] */ VARIANT Location,
            /* [defaultvalue] */ enum /* external definition not present */ vsCMAccess Access,
            /* [retval][out] */ __RPC__deref_out_opt CodeEvent **ppEvent);
        
        /* [helpstringcontext][helpstring][helpcontext][propget][id] */ HRESULT ( STDMETHODCALLTYPE *get_IsShared )( 
            CodeClass2 * This,
            /* [retval][out] */ __RPC__out VARIANT_BOOL *pIsShared);
        
        /* [helpstringcontext][helpstring][helpcontext][propput][id] */ HRESULT ( STDMETHODCALLTYPE *put_IsShared )( 
            CodeClass2 * This,
            VARIANT_BOOL Shared);
        
        END_INTERFACE
    } CodeClass2Vtbl;

    interface CodeClass2
    {
        CONST_VTBL struct CodeClass2Vtbl *lpVtbl;
    };

    

#ifdef COBJMACROS


#define CodeClass2_QueryInterface(This,riid,ppvObj,retval)	\
    ( (This)->lpVtbl -> QueryInterface(This,riid,ppvObj,retval) ) 

#define CodeClass2_AddRef(This,retval)	\
    ( (This)->lpVtbl -> AddRef(This,retval) ) 

#define CodeClass2_Release(This,retval)	\
    ( (This)->lpVtbl -> Release(This,retval) ) 

#define CodeClass2_GetTypeInfoCount(This,pctinfo,retval)	\
    ( (This)->lpVtbl -> GetTypeInfoCount(This,pctinfo,retval) ) 

#define CodeClass2_GetTypeInfo(This,itinfo,lcid,pptinfo,retval)	\
    ( (This)->lpVtbl -> GetTypeInfo(This,itinfo,lcid,pptinfo,retval) ) 

#define CodeClass2_GetIDsOfNames(This,riid,rgszNames,cNames,lcid,rgdispid,retval)	\
    ( (This)->lpVtbl -> GetIDsOfNames(This,riid,rgszNames,cNames,lcid,rgdispid,retval) ) 

#define CodeClass2_Invoke(This,dispidMember,riid,lcid,wFlags,pdispparams,pvarResult,pexcepinfo,puArgErr,retval)	\
    ( (This)->lpVtbl -> Invoke(This,dispidMember,riid,lcid,wFlags,pdispparams,pvarResult,pexcepinfo,puArgErr,retval) ) 

#define CodeClass2_get_DTE(This,retval)	\
    ( (This)->lpVtbl -> get_DTE(This,retval) ) 

#define CodeClass2_get_Collection(This,retval)	\
    ( (This)->lpVtbl -> get_Collection(This,retval) ) 

#define CodeClass2_get_Name(This,retval)	\
    ( (This)->lpVtbl -> get_Name(This,retval) ) 

#define CodeClass2_put_Name(This,noname,retval)	\
    ( (This)->lpVtbl -> put_Name(This,noname,retval) ) 

#define CodeClass2_get_FullName(This,retval)	\
    ( (This)->lpVtbl -> get_FullName(This,retval) ) 

#define CodeClass2_get_ProjectItem(This,retval)	\
    ( (This)->lpVtbl -> get_ProjectItem(This,retval) ) 

#define CodeClass2_get_Kind(This,retval)	\
    ( (This)->lpVtbl -> get_Kind(This,retval) ) 

#define CodeClass2_get_IsCodeType(This,retval)	\
    ( (This)->lpVtbl -> get_IsCodeType(This,retval) ) 

#define CodeClass2_get_InfoLocation(This,retval)	\
    ( (This)->lpVtbl -> get_InfoLocation(This,retval) ) 

#define CodeClass2_get_Children(This,retval)	\
    ( (This)->lpVtbl -> get_Children(This,retval) ) 

#define CodeClass2_get_Language(This,retval)	\
    ( (This)->lpVtbl -> get_Language(This,retval) ) 

#define CodeClass2_get_StartPoint(This,retval)	\
    ( (This)->lpVtbl -> get_StartPoint(This,retval) ) 

#define CodeClass2_get_EndPoint(This,retval)	\
    ( (This)->lpVtbl -> get_EndPoint(This,retval) ) 

#define CodeClass2_get_ExtenderNames(This,retval)	\
    ( (This)->lpVtbl -> get_ExtenderNames(This,retval) ) 

#define CodeClass2_get_Extender(This,ExtenderName,retval)	\
    ( (This)->lpVtbl -> get_Extender(This,ExtenderName,retval) ) 

#define CodeClass2_get_ExtenderCATID(This,retval)	\
    ( (This)->lpVtbl -> get_ExtenderCATID(This,retval) ) 

#define CodeClass2_GetStartPoint(This,Part,retval)	\
    ( (This)->lpVtbl -> GetStartPoint(This,Part,retval) ) 

#define CodeClass2_GetEndPoint(This,Part,retval)	\
    ( (This)->lpVtbl -> GetEndPoint(This,Part,retval) ) 

#define CodeClass2_get_Parent(This,retval)	\
    ( (This)->lpVtbl -> get_Parent(This,retval) ) 

#define CodeClass2_get_Namespace(This,retval)	\
    ( (This)->lpVtbl -> get_Namespace(This,retval) ) 

#define CodeClass2_get_Bases(This,retval)	\
    ( (This)->lpVtbl -> get_Bases(This,retval) ) 

#define CodeClass2_get_Members(This,retval)	\
    ( (This)->lpVtbl -> get_Members(This,retval) ) 

#define CodeClass2_put_Access(This,noname,retval)	\
    ( (This)->lpVtbl -> put_Access(This,noname,retval) ) 

#define CodeClass2_get_Access(This,retval)	\
    ( (This)->lpVtbl -> get_Access(This,retval) ) 

#define CodeClass2_get_Attributes(This,retval)	\
    ( (This)->lpVtbl -> get_Attributes(This,retval) ) 

#define CodeClass2_get_DocComment(This,retval)	\
    ( (This)->lpVtbl -> get_DocComment(This,retval) ) 

#define CodeClass2_put_DocComment(This,noname,retval)	\
    ( (This)->lpVtbl -> put_DocComment(This,noname,retval) ) 

#define CodeClass2_get_Comment(This,retval)	\
    ( (This)->lpVtbl -> get_Comment(This,retval) ) 

#define CodeClass2_put_Comment(This,noname,retval)	\
    ( (This)->lpVtbl -> put_Comment(This,noname,retval) ) 

#define CodeClass2_AddBase(This,Base,Position,retval)	\
    ( (This)->lpVtbl -> AddBase(This,Base,Position,retval) ) 

#define CodeClass2_AddAttribute(This,Name,Value,Position,retval)	\
    ( (This)->lpVtbl -> AddAttribute(This,Name,Value,Position,retval) ) 

#define CodeClass2_RemoveBase(This,Element,retval)	\
    ( (This)->lpVtbl -> RemoveBase(This,Element,retval) ) 

#define CodeClass2_RemoveMember(This,Element,retval)	\
    ( (This)->lpVtbl -> RemoveMember(This,Element,retval) ) 

#define CodeClass2_get_IsDerivedFrom(This,FullName,retval)	\
    ( (This)->lpVtbl -> get_IsDerivedFrom(This,FullName,retval) ) 

#define CodeClass2_get_DerivedTypes(This,retval)	\
    ( (This)->lpVtbl -> get_DerivedTypes(This,retval) ) 

#define CodeClass2_get_ImplementedInterfaces(This,retval)	\
    ( (This)->lpVtbl -> get_ImplementedInterfaces(This,retval) ) 

#define CodeClass2_get_IsAbstract(This,retval)	\
    ( (This)->lpVtbl -> get_IsAbstract(This,retval) ) 

#define CodeClass2_put_IsAbstract(This,noname,retval)	\
    ( (This)->lpVtbl -> put_IsAbstract(This,noname,retval) ) 

#define CodeClass2_AddImplementedInterface(This,Base,Position,retval)	\
    ( (This)->lpVtbl -> AddImplementedInterface(This,Base,Position,retval) ) 

#define CodeClass2_AddFunction(This,Name,Kind,Type,Position,Access,Location,retval)	\
    ( (This)->lpVtbl -> AddFunction(This,Name,Kind,Type,Position,Access,Location,retval) ) 

#define CodeClass2_AddVariable(This,Name,Type,Position,Access,Location,retval)	\
    ( (This)->lpVtbl -> AddVariable(This,Name,Type,Position,Access,Location,retval) ) 

#define CodeClass2_AddProperty(This,GetterName,PutterName,Type,Position,Access,Location,retval)	\
    ( (This)->lpVtbl -> AddProperty(This,GetterName,PutterName,Type,Position,Access,Location,retval) ) 

#define CodeClass2_AddClass(This,Name,Position,Bases,ImplementedInterfaces,Access,retval)	\
    ( (This)->lpVtbl -> AddClass(This,Name,Position,Bases,ImplementedInterfaces,Access,retval) ) 

#define CodeClass2_AddStruct(This,Name,Position,Bases,ImplementedInterfaces,Access,retval)	\
    ( (This)->lpVtbl -> AddStruct(This,Name,Position,Bases,ImplementedInterfaces,Access,retval) ) 

#define CodeClass2_AddEnum(This,Name,Position,Bases,Access,retval)	\
    ( (This)->lpVtbl -> AddEnum(This,Name,Position,Bases,Access,retval) ) 

#define CodeClass2_AddDelegate(This,Name,Type,Position,Access,retval)	\
    ( (This)->lpVtbl -> AddDelegate(This,Name,Type,Position,Access,retval) ) 

#define CodeClass2_RemoveInterface(This,Element,retval)	\
    ( (This)->lpVtbl -> RemoveInterface(This,Element,retval) ) 


#define CodeClass2_get_ClassKind(This,Kind)	\
    ( (This)->lpVtbl -> get_ClassKind(This,Kind) ) 

#define CodeClass2_put_ClassKind(This,Kind)	\
    ( (This)->lpVtbl -> put_ClassKind(This,Kind) ) 

#define CodeClass2_get_PartialClasses(This,Elements)	\
    ( (This)->lpVtbl -> get_PartialClasses(This,Elements) ) 

#define CodeClass2_get_DataTypeKind(This,Kind)	\
    ( (This)->lpVtbl -> get_DataTypeKind(This,Kind) ) 

#define CodeClass2_put_DataTypeKind(This,Kind)	\
    ( (This)->lpVtbl -> put_DataTypeKind(This,Kind) ) 

#define CodeClass2_get_Parts(This,Elements)	\
    ( (This)->lpVtbl -> get_Parts(This,Elements) ) 

#define CodeClass2_get_InheritanceKind(This,Kind)	\
    ( (This)->lpVtbl -> get_InheritanceKind(This,Kind) ) 

#define CodeClass2_put_InheritanceKind(This,Kind)	\
    ( (This)->lpVtbl -> put_InheritanceKind(This,Kind) ) 

#define CodeClass2_get_IsGeneric(This,pIsGeneric)	\
    ( (This)->lpVtbl -> get_IsGeneric(This,pIsGeneric) ) 

#define CodeClass2_AddEvent(This,Name,FullDelegateName,CreatePropertyStyleEvent,Location,Access,ppEvent)	\
    ( (This)->lpVtbl -> AddEvent(This,Name,FullDelegateName,CreatePropertyStyleEvent,Location,Access,ppEvent) ) 

#define CodeClass2_get_IsShared(This,pIsShared)	\
    ( (This)->lpVtbl -> get_IsShared(This,pIsShared) ) 

#define CodeClass2_put_IsShared(This,Shared)	\
    ( (This)->lpVtbl -> put_IsShared(This,Shared) ) 

#endif /* COBJMACROS */


#endif 	/* C style interface */




#endif 	/* __CodeClass2_INTERFACE_DEFINED__ */


#ifndef __CodeParameter2_INTERFACE_DEFINED__
#define __CodeParameter2_INTERFACE_DEFINED__

/* interface CodeParameter2 */
/* [helpstringcontext][helpstring][helpcontext][uuid][object][oleautomation][dual] */ 


EXTERN_C const IID IID_CodeParameter2;

#if defined(__cplusplus) && !defined(CINTERFACE)
    
    MIDL_INTERFACE("35CD9E36-7C96-4429-968F-C0C350CB1A47")
    CodeParameter2 : public CodeParameter
    {
    public:
        virtual /* [helpstringcontext][helpstring][helpcontext][propget][id] */ HRESULT STDMETHODCALLTYPE get_ParameterKind( 
            /* [retval][out] */ __RPC__out vsCMParameterKind *Kind) = 0;
        
        virtual /* [helpstringcontext][helpstring][helpcontext][propput][id] */ HRESULT STDMETHODCALLTYPE put_ParameterKind( 
            vsCMParameterKind Kind) = 0;
        
        virtual /* [helpstringcontext][helpstring][helpcontext][propget][id] */ HRESULT STDMETHODCALLTYPE get_DefaultValue( 
            /* [retval][out] */ __RPC__deref_out_opt BSTR *Value) = 0;
        
        virtual /* [helpstringcontext][helpstring][helpcontext][propput][id] */ HRESULT STDMETHODCALLTYPE put_DefaultValue( 
            __RPC__in BSTR Value) = 0;
        
    };
    
#else 	/* C style interface */

    typedef struct CodeParameter2Vtbl
    {
        BEGIN_INTERFACE
        
        /* [id][restricted][funcdescattr] */ HRESULT ( STDMETHODCALLTYPE *QueryInterface )( 
            CodeParameter2 * This,
            /* [in][idldescattr] */ __RPC__in struct GUID *riid,
            /* [out][idldescattr] */ __RPC__deref_out_opt void **ppvObj,
            /* [retval][out] */ __RPC__out void *retval);
        
        /* [id][restricted][funcdescattr] */ HRESULT ( STDMETHODCALLTYPE *AddRef )( 
            CodeParameter2 * This,
            /* [retval][out] */ __RPC__out unsigned long *retval);
        
        /* [id][restricted][funcdescattr] */ HRESULT ( STDMETHODCALLTYPE *Release )( 
            CodeParameter2 * This,
            /* [retval][out] */ __RPC__out unsigned long *retval);
        
        /* [id][restricted][funcdescattr] */ HRESULT ( STDMETHODCALLTYPE *GetTypeInfoCount )( 
            CodeParameter2 * This,
            /* [out][idldescattr] */ __RPC__out unsigned UINT *pctinfo,
            /* [retval][out] */ __RPC__out void *retval);
        
        /* [id][restricted][funcdescattr] */ HRESULT ( STDMETHODCALLTYPE *GetTypeInfo )( 
            CodeParameter2 * This,
            /* [in][idldescattr] */ unsigned UINT itinfo,
            /* [in][idldescattr] */ unsigned long lcid,
            /* [out][idldescattr] */ __RPC__deref_out_opt void **pptinfo,
            /* [retval][out] */ __RPC__out void *retval);
        
        /* [id][restricted][funcdescattr] */ HRESULT ( STDMETHODCALLTYPE *GetIDsOfNames )( 
            CodeParameter2 * This,
            /* [in][idldescattr] */ __RPC__in struct GUID *riid,
            /* [in][idldescattr] */ __RPC__deref_in_opt signed char **rgszNames,
            /* [in][idldescattr] */ unsigned UINT cNames,
            /* [in][idldescattr] */ unsigned long lcid,
            /* [out][idldescattr] */ __RPC__out signed long *rgdispid,
            /* [retval][out] */ __RPC__out void *retval);
        
        /* [id][restricted][funcdescattr] */ HRESULT ( STDMETHODCALLTYPE *Invoke )( 
            CodeParameter2 * This,
            /* [in][idldescattr] */ signed long dispidMember,
            /* [in][idldescattr] */ __RPC__in struct GUID *riid,
            /* [in][idldescattr] */ unsigned long lcid,
            /* [in][idldescattr] */ unsigned short wFlags,
            /* [in][idldescattr] */ __RPC__in struct DISPPARAMS *pdispparams,
            /* [out][idldescattr] */ __RPC__out VARIANT *pvarResult,
            /* [out][idldescattr] */ __RPC__out struct EXCEPINFO *pexcepinfo,
            /* [out][idldescattr] */ __RPC__out unsigned UINT *puArgErr,
            /* [retval][out] */ __RPC__out void *retval);
        
        /* [id][propget][funcdescattr] */ HRESULT ( STDMETHODCALLTYPE *get_DTE )( 
            CodeParameter2 * This,
            /* [retval][out] */ __RPC__deref_out_opt **retval);
        
        /* [id][propget][funcdescattr] */ HRESULT ( STDMETHODCALLTYPE *get_Collection )( 
            CodeParameter2 * This,
            /* [retval][out] */ __RPC__deref_out_opt CodeElements **retval);
        
        /* [id][propget][funcdescattr] */ HRESULT ( STDMETHODCALLTYPE *get_Name )( 
            CodeParameter2 * This,
            /* [retval][out] */ __RPC__deref_out_opt BSTR *retval);
        
        /* [id][propput][funcdescattr] */ HRESULT ( STDMETHODCALLTYPE *put_Name )( 
            CodeParameter2 * This,
            /* [idldescattr] */ __RPC__in BSTR noname,
            /* [retval][out] */ __RPC__out void *retval);
        
        /* [id][propget][funcdescattr] */ HRESULT ( STDMETHODCALLTYPE *get_FullName )( 
            CodeParameter2 * This,
            /* [retval][out] */ __RPC__deref_out_opt BSTR *retval);
        
        /* [id][propget][funcdescattr] */ HRESULT ( STDMETHODCALLTYPE *get_ProjectItem )( 
            CodeParameter2 * This,
            /* [retval][out] */ __RPC__deref_out_opt ProjectItem **retval);
        
        /* [id][propget][funcdescattr] */ HRESULT ( STDMETHODCALLTYPE *get_Kind )( 
            CodeParameter2 * This,
            /* [retval][out] */ __RPC__out enum vsCMElement *retval);
        
        /* [id][propget][funcdescattr] */ HRESULT ( STDMETHODCALLTYPE *get_IsCodeType )( 
            CodeParameter2 * This,
            /* [retval][out] */ __RPC__out BOOLEAN *retval);
        
        /* [id][propget][funcdescattr] */ HRESULT ( STDMETHODCALLTYPE *get_InfoLocation )( 
            CodeParameter2 * This,
            /* [retval][out] */ __RPC__out enum vsCMInfoLocation *retval);
        
        /* [id][propget][funcdescattr] */ HRESULT ( STDMETHODCALLTYPE *get_Children )( 
            CodeParameter2 * This,
            /* [retval][out] */ __RPC__deref_out_opt CodeElements **retval);
        
        /* [id][propget][funcdescattr] */ HRESULT ( STDMETHODCALLTYPE *get_Language )( 
            CodeParameter2 * This,
            /* [retval][out] */ __RPC__deref_out_opt BSTR *retval);
        
        /* [id][propget][funcdescattr] */ HRESULT ( STDMETHODCALLTYPE *get_StartPoint )( 
            CodeParameter2 * This,
            /* [retval][out] */ __RPC__deref_out_opt TextPoint **retval);
        
        /* [id][propget][funcdescattr] */ HRESULT ( STDMETHODCALLTYPE *get_EndPoint )( 
            CodeParameter2 * This,
            /* [retval][out] */ __RPC__deref_out_opt TextPoint **retval);
        
        /* [id][propget][funcdescattr] */ HRESULT ( STDMETHODCALLTYPE *get_ExtenderNames )( 
            CodeParameter2 * This,
            /* [retval][out] */ __RPC__out VARIANT *retval);
        
        /* [id][propget][funcdescattr] */ HRESULT ( STDMETHODCALLTYPE *get_Extender )( 
            CodeParameter2 * This,
            /* [idldescattr] */ __RPC__in BSTR ExtenderName,
            /* [retval][out] */ __RPC__deref_out_opt IDispatch **retval);
        
        /* [id][propget][funcdescattr] */ HRESULT ( STDMETHODCALLTYPE *get_ExtenderCATID )( 
            CodeParameter2 * This,
            /* [retval][out] */ __RPC__deref_out_opt BSTR *retval);
        
        /* [id][funcdescattr] */ HRESULT ( STDMETHODCALLTYPE *GetStartPoint )( 
            CodeParameter2 * This,
            /* [in][idldescattr] */ enum vsCMPart Part,
            /* [retval][out] */ __RPC__deref_out_opt TextPoint **retval);
        
        /* [id][funcdescattr] */ HRESULT ( STDMETHODCALLTYPE *GetEndPoint )( 
            CodeParameter2 * This,
            /* [in][idldescattr] */ enum vsCMPart Part,
            /* [retval][out] */ __RPC__deref_out_opt TextPoint **retval);
        
        /* [id][propget][funcdescattr] */ HRESULT ( STDMETHODCALLTYPE *get_Parent )( 
            CodeParameter2 * This,
            /* [retval][out] */ __RPC__deref_out_opt CodeElement **retval);
        
        /* [id][propput][funcdescattr] */ HRESULT ( STDMETHODCALLTYPE *put_Type )( 
            CodeParameter2 * This,
            /* [idldescattr] */ __RPC__in_opt CodeTypeRef *noname,
            /* [retval][out] */ __RPC__out void *retval);
        
        /* [id][propget][funcdescattr] */ HRESULT ( STDMETHODCALLTYPE *get_Type )( 
            CodeParameter2 * This,
            /* [retval][out] */ __RPC__deref_out_opt CodeTypeRef **retval);
        
        /* [id][propget][funcdescattr] */ HRESULT ( STDMETHODCALLTYPE *get_Attributes )( 
            CodeParameter2 * This,
            /* [retval][out] */ __RPC__deref_out_opt CodeElements **retval);
        
        /* [id][propget][funcdescattr] */ HRESULT ( STDMETHODCALLTYPE *get_DocComment )( 
            CodeParameter2 * This,
            /* [retval][out] */ __RPC__deref_out_opt BSTR *retval);
        
        /* [id][propput][funcdescattr] */ HRESULT ( STDMETHODCALLTYPE *put_DocComment )( 
            CodeParameter2 * This,
            /* [idldescattr] */ __RPC__in BSTR noname,
            /* [retval][out] */ __RPC__out void *retval);
        
        /* [id][funcdescattr] */ HRESULT ( STDMETHODCALLTYPE *AddAttribute )( 
            CodeParameter2 * This,
            /* [idldescattr] */ __RPC__in BSTR Name,
            /* [idldescattr] */ __RPC__in BSTR Value,
            /* [optional][idldescattr] */ VARIANT Position,
            /* [retval][out] */ __RPC__deref_out_opt CodeAttribute **retval);
        
        /* [helpstringcontext][helpstring][helpcontext][propget][id] */ HRESULT ( STDMETHODCALLTYPE *get_ParameterKind )( 
            CodeParameter2 * This,
            /* [retval][out] */ __RPC__out vsCMParameterKind *Kind);
        
        /* [helpstringcontext][helpstring][helpcontext][propput][id] */ HRESULT ( STDMETHODCALLTYPE *put_ParameterKind )( 
            CodeParameter2 * This,
            vsCMParameterKind Kind);
        
        /* [helpstringcontext][helpstring][helpcontext][propget][id] */ HRESULT ( STDMETHODCALLTYPE *get_DefaultValue )( 
            CodeParameter2 * This,
            /* [retval][out] */ __RPC__deref_out_opt BSTR *Value);
        
        /* [helpstringcontext][helpstring][helpcontext][propput][id] */ HRESULT ( STDMETHODCALLTYPE *put_DefaultValue )( 
            CodeParameter2 * This,
            __RPC__in BSTR Value);
        
        END_INTERFACE
    } CodeParameter2Vtbl;

    interface CodeParameter2
    {
        CONST_VTBL struct CodeParameter2Vtbl *lpVtbl;
    };

    

#ifdef COBJMACROS


#define CodeParameter2_QueryInterface(This,riid,ppvObj,retval)	\
    ( (This)->lpVtbl -> QueryInterface(This,riid,ppvObj,retval) ) 

#define CodeParameter2_AddRef(This,retval)	\
    ( (This)->lpVtbl -> AddRef(This,retval) ) 

#define CodeParameter2_Release(This,retval)	\
    ( (This)->lpVtbl -> Release(This,retval) ) 

#define CodeParameter2_GetTypeInfoCount(This,pctinfo,retval)	\
    ( (This)->lpVtbl -> GetTypeInfoCount(This,pctinfo,retval) ) 

#define CodeParameter2_GetTypeInfo(This,itinfo,lcid,pptinfo,retval)	\
    ( (This)->lpVtbl -> GetTypeInfo(This,itinfo,lcid,pptinfo,retval) ) 

#define CodeParameter2_GetIDsOfNames(This,riid,rgszNames,cNames,lcid,rgdispid,retval)	\
    ( (This)->lpVtbl -> GetIDsOfNames(This,riid,rgszNames,cNames,lcid,rgdispid,retval) ) 

#define CodeParameter2_Invoke(This,dispidMember,riid,lcid,wFlags,pdispparams,pvarResult,pexcepinfo,puArgErr,retval)	\
    ( (This)->lpVtbl -> Invoke(This,dispidMember,riid,lcid,wFlags,pdispparams,pvarResult,pexcepinfo,puArgErr,retval) ) 

#define CodeParameter2_get_DTE(This,retval)	\
    ( (This)->lpVtbl -> get_DTE(This,retval) ) 

#define CodeParameter2_get_Collection(This,retval)	\
    ( (This)->lpVtbl -> get_Collection(This,retval) ) 

#define CodeParameter2_get_Name(This,retval)	\
    ( (This)->lpVtbl -> get_Name(This,retval) ) 

#define CodeParameter2_put_Name(This,noname,retval)	\
    ( (This)->lpVtbl -> put_Name(This,noname,retval) ) 

#define CodeParameter2_get_FullName(This,retval)	\
    ( (This)->lpVtbl -> get_FullName(This,retval) ) 

#define CodeParameter2_get_ProjectItem(This,retval)	\
    ( (This)->lpVtbl -> get_ProjectItem(This,retval) ) 

#define CodeParameter2_get_Kind(This,retval)	\
    ( (This)->lpVtbl -> get_Kind(This,retval) ) 

#define CodeParameter2_get_IsCodeType(This,retval)	\
    ( (This)->lpVtbl -> get_IsCodeType(This,retval) ) 

#define CodeParameter2_get_InfoLocation(This,retval)	\
    ( (This)->lpVtbl -> get_InfoLocation(This,retval) ) 

#define CodeParameter2_get_Children(This,retval)	\
    ( (This)->lpVtbl -> get_Children(This,retval) ) 

#define CodeParameter2_get_Language(This,retval)	\
    ( (This)->lpVtbl -> get_Language(This,retval) ) 

#define CodeParameter2_get_StartPoint(This,retval)	\
    ( (This)->lpVtbl -> get_StartPoint(This,retval) ) 

#define CodeParameter2_get_EndPoint(This,retval)	\
    ( (This)->lpVtbl -> get_EndPoint(This,retval) ) 

#define CodeParameter2_get_ExtenderNames(This,retval)	\
    ( (This)->lpVtbl -> get_ExtenderNames(This,retval) ) 

#define CodeParameter2_get_Extender(This,ExtenderName,retval)	\
    ( (This)->lpVtbl -> get_Extender(This,ExtenderName,retval) ) 

#define CodeParameter2_get_ExtenderCATID(This,retval)	\
    ( (This)->lpVtbl -> get_ExtenderCATID(This,retval) ) 

#define CodeParameter2_GetStartPoint(This,Part,retval)	\
    ( (This)->lpVtbl -> GetStartPoint(This,Part,retval) ) 

#define CodeParameter2_GetEndPoint(This,Part,retval)	\
    ( (This)->lpVtbl -> GetEndPoint(This,Part,retval) ) 

#define CodeParameter2_get_Parent(This,retval)	\
    ( (This)->lpVtbl -> get_Parent(This,retval) ) 

#define CodeParameter2_put_Type(This,noname,retval)	\
    ( (This)->lpVtbl -> put_Type(This,noname,retval) ) 

#define CodeParameter2_get_Type(This,retval)	\
    ( (This)->lpVtbl -> get_Type(This,retval) ) 

#define CodeParameter2_get_Attributes(This,retval)	\
    ( (This)->lpVtbl -> get_Attributes(This,retval) ) 

#define CodeParameter2_get_DocComment(This,retval)	\
    ( (This)->lpVtbl -> get_DocComment(This,retval) ) 

#define CodeParameter2_put_DocComment(This,noname,retval)	\
    ( (This)->lpVtbl -> put_DocComment(This,noname,retval) ) 

#define CodeParameter2_AddAttribute(This,Name,Value,Position,retval)	\
    ( (This)->lpVtbl -> AddAttribute(This,Name,Value,Position,retval) ) 


#define CodeParameter2_get_ParameterKind(This,Kind)	\
    ( (This)->lpVtbl -> get_ParameterKind(This,Kind) ) 

#define CodeParameter2_put_ParameterKind(This,Kind)	\
    ( (This)->lpVtbl -> put_ParameterKind(This,Kind) ) 

#define CodeParameter2_get_DefaultValue(This,Value)	\
    ( (This)->lpVtbl -> get_DefaultValue(This,Value) ) 

#define CodeParameter2_put_DefaultValue(This,Value)	\
    ( (This)->lpVtbl -> put_DefaultValue(This,Value) ) 

#endif /* COBJMACROS */


#endif 	/* C style interface */




#endif 	/* __CodeParameter2_INTERFACE_DEFINED__ */


#ifndef __CodeFunction2_INTERFACE_DEFINED__
#define __CodeFunction2_INTERFACE_DEFINED__

/* interface CodeFunction2 */
/* [helpstringcontext][helpstring][helpcontext][uuid][object][oleautomation][dual] */ 


EXTERN_C const IID IID_CodeFunction2;

#if defined(__cplusplus) && !defined(CINTERFACE)
    
    MIDL_INTERFACE("7E20CD35-8689-466D-8EA0-A863B7EA5DB9")
    CodeFunction2 : public CodeFunction
    {
    public:
        virtual /* [helpstringcontext][helpstring][helpcontext][propget][id] */ HRESULT STDMETHODCALLTYPE get_OverrideKind( 
            /* [retval][out] */ __RPC__out vsCMOverrideKind *Kind) = 0;
        
        virtual /* [helpstringcontext][helpstring][helpcontext][propput][id] */ HRESULT STDMETHODCALLTYPE put_OverrideKind( 
            vsCMOverrideKind Kind) = 0;
        
        virtual /* [helpstring][propget][id] */ HRESULT STDMETHODCALLTYPE get_IsGeneric( 
            /* [retval][out] */ __RPC__out VARIANT_BOOL *pIsGeneric) = 0;
        
    };
    
#else 	/* C style interface */

    typedef struct CodeFunction2Vtbl
    {
        BEGIN_INTERFACE
        
        /* [id][restricted][funcdescattr] */ HRESULT ( STDMETHODCALLTYPE *QueryInterface )( 
            CodeFunction2 * This,
            /* [in][idldescattr] */ __RPC__in struct GUID *riid,
            /* [out][idldescattr] */ __RPC__deref_out_opt void **ppvObj,
            /* [retval][out] */ __RPC__out void *retval);
        
        /* [id][restricted][funcdescattr] */ HRESULT ( STDMETHODCALLTYPE *AddRef )( 
            CodeFunction2 * This,
            /* [retval][out] */ __RPC__out unsigned long *retval);
        
        /* [id][restricted][funcdescattr] */ HRESULT ( STDMETHODCALLTYPE *Release )( 
            CodeFunction2 * This,
            /* [retval][out] */ __RPC__out unsigned long *retval);
        
        /* [id][restricted][funcdescattr] */ HRESULT ( STDMETHODCALLTYPE *GetTypeInfoCount )( 
            CodeFunction2 * This,
            /* [out][idldescattr] */ __RPC__out unsigned UINT *pctinfo,
            /* [retval][out] */ __RPC__out void *retval);
        
        /* [id][restricted][funcdescattr] */ HRESULT ( STDMETHODCALLTYPE *GetTypeInfo )( 
            CodeFunction2 * This,
            /* [in][idldescattr] */ unsigned UINT itinfo,
            /* [in][idldescattr] */ unsigned long lcid,
            /* [out][idldescattr] */ __RPC__deref_out_opt void **pptinfo,
            /* [retval][out] */ __RPC__out void *retval);
        
        /* [id][restricted][funcdescattr] */ HRESULT ( STDMETHODCALLTYPE *GetIDsOfNames )( 
            CodeFunction2 * This,
            /* [in][idldescattr] */ __RPC__in struct GUID *riid,
            /* [in][idldescattr] */ __RPC__deref_in_opt signed char **rgszNames,
            /* [in][idldescattr] */ unsigned UINT cNames,
            /* [in][idldescattr] */ unsigned long lcid,
            /* [out][idldescattr] */ __RPC__out signed long *rgdispid,
            /* [retval][out] */ __RPC__out void *retval);
        
        /* [id][restricted][funcdescattr] */ HRESULT ( STDMETHODCALLTYPE *Invoke )( 
            CodeFunction2 * This,
            /* [in][idldescattr] */ signed long dispidMember,
            /* [in][idldescattr] */ __RPC__in struct GUID *riid,
            /* [in][idldescattr] */ unsigned long lcid,
            /* [in][idldescattr] */ unsigned short wFlags,
            /* [in][idldescattr] */ __RPC__in struct DISPPARAMS *pdispparams,
            /* [out][idldescattr] */ __RPC__out VARIANT *pvarResult,
            /* [out][idldescattr] */ __RPC__out struct EXCEPINFO *pexcepinfo,
            /* [out][idldescattr] */ __RPC__out unsigned UINT *puArgErr,
            /* [retval][out] */ __RPC__out void *retval);
        
        /* [id][propget][funcdescattr] */ HRESULT ( STDMETHODCALLTYPE *get_DTE )( 
            CodeFunction2 * This,
            /* [retval][out] */ __RPC__deref_out_opt **retval);
        
        /* [id][propget][funcdescattr] */ HRESULT ( STDMETHODCALLTYPE *get_Collection )( 
            CodeFunction2 * This,
            /* [retval][out] */ __RPC__deref_out_opt CodeElements **retval);
        
        /* [id][propget][funcdescattr] */ HRESULT ( STDMETHODCALLTYPE *get_Name )( 
            CodeFunction2 * This,
            /* [retval][out] */ __RPC__deref_out_opt BSTR *retval);
        
        /* [id][propput][funcdescattr] */ HRESULT ( STDMETHODCALLTYPE *put_Name )( 
            CodeFunction2 * This,
            /* [idldescattr] */ __RPC__in BSTR noname,
            /* [retval][out] */ __RPC__out void *retval);
        
        /* [id][propget][funcdescattr] */ HRESULT ( STDMETHODCALLTYPE *get_FullName )( 
            CodeFunction2 * This,
            /* [retval][out] */ __RPC__deref_out_opt BSTR *retval);
        
        /* [id][propget][funcdescattr] */ HRESULT ( STDMETHODCALLTYPE *get_ProjectItem )( 
            CodeFunction2 * This,
            /* [retval][out] */ __RPC__deref_out_opt ProjectItem **retval);
        
        /* [id][propget][funcdescattr] */ HRESULT ( STDMETHODCALLTYPE *get_Kind )( 
            CodeFunction2 * This,
            /* [retval][out] */ __RPC__out enum vsCMElement *retval);
        
        /* [id][propget][funcdescattr] */ HRESULT ( STDMETHODCALLTYPE *get_IsCodeType )( 
            CodeFunction2 * This,
            /* [retval][out] */ __RPC__out BOOLEAN *retval);
        
        /* [id][propget][funcdescattr] */ HRESULT ( STDMETHODCALLTYPE *get_InfoLocation )( 
            CodeFunction2 * This,
            /* [retval][out] */ __RPC__out enum vsCMInfoLocation *retval);
        
        /* [id][propget][funcdescattr] */ HRESULT ( STDMETHODCALLTYPE *get_Children )( 
            CodeFunction2 * This,
            /* [retval][out] */ __RPC__deref_out_opt CodeElements **retval);
        
        /* [id][propget][funcdescattr] */ HRESULT ( STDMETHODCALLTYPE *get_Language )( 
            CodeFunction2 * This,
            /* [retval][out] */ __RPC__deref_out_opt BSTR *retval);
        
        /* [id][propget][funcdescattr] */ HRESULT ( STDMETHODCALLTYPE *get_StartPoint )( 
            CodeFunction2 * This,
            /* [retval][out] */ __RPC__deref_out_opt TextPoint **retval);
        
        /* [id][propget][funcdescattr] */ HRESULT ( STDMETHODCALLTYPE *get_EndPoint )( 
            CodeFunction2 * This,
            /* [retval][out] */ __RPC__deref_out_opt TextPoint **retval);
        
        /* [id][propget][funcdescattr] */ HRESULT ( STDMETHODCALLTYPE *get_ExtenderNames )( 
            CodeFunction2 * This,
            /* [retval][out] */ __RPC__out VARIANT *retval);
        
        /* [id][propget][funcdescattr] */ HRESULT ( STDMETHODCALLTYPE *get_Extender )( 
            CodeFunction2 * This,
            /* [idldescattr] */ __RPC__in BSTR ExtenderName,
            /* [retval][out] */ __RPC__deref_out_opt IDispatch **retval);
        
        /* [id][propget][funcdescattr] */ HRESULT ( STDMETHODCALLTYPE *get_ExtenderCATID )( 
            CodeFunction2 * This,
            /* [retval][out] */ __RPC__deref_out_opt BSTR *retval);
        
        /* [id][funcdescattr] */ HRESULT ( STDMETHODCALLTYPE *GetStartPoint )( 
            CodeFunction2 * This,
            /* [in][idldescattr] */ enum vsCMPart Part,
            /* [retval][out] */ __RPC__deref_out_opt TextPoint **retval);
        
        /* [id][funcdescattr] */ HRESULT ( STDMETHODCALLTYPE *GetEndPoint )( 
            CodeFunction2 * This,
            /* [in][idldescattr] */ enum vsCMPart Part,
            /* [retval][out] */ __RPC__deref_out_opt TextPoint **retval);
        
        /* [id][propget][funcdescattr] */ HRESULT ( STDMETHODCALLTYPE *get_Parent )( 
            CodeFunction2 * This,
            /* [retval][out] */ __RPC__deref_out_opt IDispatch **retval);
        
        /* [id][propget][funcdescattr] */ HRESULT ( STDMETHODCALLTYPE *get_FunctionKind )( 
            CodeFunction2 * This,
            /* [retval][out] */ __RPC__out enum vsCMFunction *retval);
        
        /* [id][propget][funcdescattr] */ HRESULT ( STDMETHODCALLTYPE *get_Prototype )( 
            CodeFunction2 * This,
            /* [in][idldescattr] */ signed long Flags,
            /* [retval][out] */ __RPC__deref_out_opt BSTR *retval);
        
        /* [id][propget][funcdescattr] */ HRESULT ( STDMETHODCALLTYPE *get_Type )( 
            CodeFunction2 * This,
            /* [retval][out] */ __RPC__deref_out_opt CodeTypeRef **retval);
        
        /* [id][propput][funcdescattr] */ HRESULT ( STDMETHODCALLTYPE *put_Type )( 
            CodeFunction2 * This,
            /* [idldescattr] */ __RPC__in_opt CodeTypeRef *noname,
            /* [retval][out] */ __RPC__out void *retval);
        
        /* [id][propget][funcdescattr] */ HRESULT ( STDMETHODCALLTYPE *get_Parameters )( 
            CodeFunction2 * This,
            /* [retval][out] */ __RPC__deref_out_opt CodeElements **retval);
        
        /* [id][propput][funcdescattr] */ HRESULT ( STDMETHODCALLTYPE *put_Access )( 
            CodeFunction2 * This,
            /* [idldescattr] */ enum vsCMAccess noname,
            /* [retval][out] */ __RPC__out void *retval);
        
        /* [id][propget][funcdescattr] */ HRESULT ( STDMETHODCALLTYPE *get_Access )( 
            CodeFunction2 * This,
            /* [retval][out] */ __RPC__out enum vsCMAccess *retval);
        
        /* [id][propget][funcdescattr] */ HRESULT ( STDMETHODCALLTYPE *get_IsOverloaded )( 
            CodeFunction2 * This,
            /* [retval][out] */ __RPC__out BOOLEAN *retval);
        
        /* [id][propget][funcdescattr] */ HRESULT ( STDMETHODCALLTYPE *get_IsShared )( 
            CodeFunction2 * This,
            /* [retval][out] */ __RPC__out BOOLEAN *retval);
        
        /* [id][propput][funcdescattr] */ HRESULT ( STDMETHODCALLTYPE *put_IsShared )( 
            CodeFunction2 * This,
            /* [idldescattr] */ BOOLEAN noname,
            /* [retval][out] */ __RPC__out void *retval);
        
        /* [id][propget][funcdescattr] */ HRESULT ( STDMETHODCALLTYPE *get_MustImplement )( 
            CodeFunction2 * This,
            /* [retval][out] */ __RPC__out BOOLEAN *retval);
        
        /* [id][propput][funcdescattr] */ HRESULT ( STDMETHODCALLTYPE *put_MustImplement )( 
            CodeFunction2 * This,
            /* [idldescattr] */ BOOLEAN noname,
            /* [retval][out] */ __RPC__out void *retval);
        
        /* [id][propget][funcdescattr] */ HRESULT ( STDMETHODCALLTYPE *get_Overloads )( 
            CodeFunction2 * This,
            /* [retval][out] */ __RPC__deref_out_opt CodeElements **retval);
        
        /* [id][propget][funcdescattr] */ HRESULT ( STDMETHODCALLTYPE *get_Attributes )( 
            CodeFunction2 * This,
            /* [retval][out] */ __RPC__deref_out_opt CodeElements **retval);
        
        /* [id][propget][funcdescattr] */ HRESULT ( STDMETHODCALLTYPE *get_DocComment )( 
            CodeFunction2 * This,
            /* [retval][out] */ __RPC__deref_out_opt BSTR *retval);
        
        /* [id][propput][funcdescattr] */ HRESULT ( STDMETHODCALLTYPE *put_DocComment )( 
            CodeFunction2 * This,
            /* [idldescattr] */ __RPC__in BSTR noname,
            /* [retval][out] */ __RPC__out void *retval);
        
        /* [id][propget][funcdescattr] */ HRESULT ( STDMETHODCALLTYPE *get_Comment )( 
            CodeFunction2 * This,
            /* [retval][out] */ __RPC__deref_out_opt BSTR *retval);
        
        /* [id][propput][funcdescattr] */ HRESULT ( STDMETHODCALLTYPE *put_Comment )( 
            CodeFunction2 * This,
            /* [idldescattr] */ __RPC__in BSTR noname,
            /* [retval][out] */ __RPC__out void *retval);
        
        /* [id][funcdescattr] */ HRESULT ( STDMETHODCALLTYPE *AddParameter )( 
            CodeFunction2 * This,
            /* [idldescattr] */ __RPC__in BSTR Name,
            /* [idldescattr] */ VARIANT Type,
            /* [optional][idldescattr] */ VARIANT Position,
            /* [retval][out] */ __RPC__deref_out_opt CodeParameter **retval);
        
        /* [id][funcdescattr] */ HRESULT ( STDMETHODCALLTYPE *AddAttribute )( 
            CodeFunction2 * This,
            /* [idldescattr] */ __RPC__in BSTR Name,
            /* [idldescattr] */ __RPC__in BSTR Value,
            /* [optional][idldescattr] */ VARIANT Position,
            /* [retval][out] */ __RPC__deref_out_opt CodeAttribute **retval);
        
        /* [id][funcdescattr] */ HRESULT ( STDMETHODCALLTYPE *RemoveParameter )( 
            CodeFunction2 * This,
            /* [idldescattr] */ VARIANT Element,
            /* [retval][out] */ __RPC__out void *retval);
        
        /* [id][propget][funcdescattr] */ HRESULT ( STDMETHODCALLTYPE *get_CanOverride )( 
            CodeFunction2 * This,
            /* [retval][out] */ __RPC__out BOOLEAN *retval);
        
        /* [id][propput][funcdescattr] */ HRESULT ( STDMETHODCALLTYPE *put_CanOverride )( 
            CodeFunction2 * This,
            /* [idldescattr] */ BOOLEAN noname,
            /* [retval][out] */ __RPC__out void *retval);
        
        /* [helpstringcontext][helpstring][helpcontext][propget][id] */ HRESULT ( STDMETHODCALLTYPE *get_OverrideKind )( 
            CodeFunction2 * This,
            /* [retval][out] */ __RPC__out vsCMOverrideKind *Kind);
        
        /* [helpstringcontext][helpstring][helpcontext][propput][id] */ HRESULT ( STDMETHODCALLTYPE *put_OverrideKind )( 
            CodeFunction2 * This,
            vsCMOverrideKind Kind);
        
        /* [helpstring][propget][id] */ HRESULT ( STDMETHODCALLTYPE *get_IsGeneric )( 
            CodeFunction2 * This,
            /* [retval][out] */ __RPC__out VARIANT_BOOL *pIsGeneric);
        
        END_INTERFACE
    } CodeFunction2Vtbl;

    interface CodeFunction2
    {
        CONST_VTBL struct CodeFunction2Vtbl *lpVtbl;
    };

    

#ifdef COBJMACROS


#define CodeFunction2_QueryInterface(This,riid,ppvObj,retval)	\
    ( (This)->lpVtbl -> QueryInterface(This,riid,ppvObj,retval) ) 

#define CodeFunction2_AddRef(This,retval)	\
    ( (This)->lpVtbl -> AddRef(This,retval) ) 

#define CodeFunction2_Release(This,retval)	\
    ( (This)->lpVtbl -> Release(This,retval) ) 

#define CodeFunction2_GetTypeInfoCount(This,pctinfo,retval)	\
    ( (This)->lpVtbl -> GetTypeInfoCount(This,pctinfo,retval) ) 

#define CodeFunction2_GetTypeInfo(This,itinfo,lcid,pptinfo,retval)	\
    ( (This)->lpVtbl -> GetTypeInfo(This,itinfo,lcid,pptinfo,retval) ) 

#define CodeFunction2_GetIDsOfNames(This,riid,rgszNames,cNames,lcid,rgdispid,retval)	\
    ( (This)->lpVtbl -> GetIDsOfNames(This,riid,rgszNames,cNames,lcid,rgdispid,retval) ) 

#define CodeFunction2_Invoke(This,dispidMember,riid,lcid,wFlags,pdispparams,pvarResult,pexcepinfo,puArgErr,retval)	\
    ( (This)->lpVtbl -> Invoke(This,dispidMember,riid,lcid,wFlags,pdispparams,pvarResult,pexcepinfo,puArgErr,retval) ) 

#define CodeFunction2_get_DTE(This,retval)	\
    ( (This)->lpVtbl -> get_DTE(This,retval) ) 

#define CodeFunction2_get_Collection(This,retval)	\
    ( (This)->lpVtbl -> get_Collection(This,retval) ) 

#define CodeFunction2_get_Name(This,retval)	\
    ( (This)->lpVtbl -> get_Name(This,retval) ) 

#define CodeFunction2_put_Name(This,noname,retval)	\
    ( (This)->lpVtbl -> put_Name(This,noname,retval) ) 

#define CodeFunction2_get_FullName(This,retval)	\
    ( (This)->lpVtbl -> get_FullName(This,retval) ) 

#define CodeFunction2_get_ProjectItem(This,retval)	\
    ( (This)->lpVtbl -> get_ProjectItem(This,retval) ) 

#define CodeFunction2_get_Kind(This,retval)	\
    ( (This)->lpVtbl -> get_Kind(This,retval) ) 

#define CodeFunction2_get_IsCodeType(This,retval)	\
    ( (This)->lpVtbl -> get_IsCodeType(This,retval) ) 

#define CodeFunction2_get_InfoLocation(This,retval)	\
    ( (This)->lpVtbl -> get_InfoLocation(This,retval) ) 

#define CodeFunction2_get_Children(This,retval)	\
    ( (This)->lpVtbl -> get_Children(This,retval) ) 

#define CodeFunction2_get_Language(This,retval)	\
    ( (This)->lpVtbl -> get_Language(This,retval) ) 

#define CodeFunction2_get_StartPoint(This,retval)	\
    ( (This)->lpVtbl -> get_StartPoint(This,retval) ) 

#define CodeFunction2_get_EndPoint(This,retval)	\
    ( (This)->lpVtbl -> get_EndPoint(This,retval) ) 

#define CodeFunction2_get_ExtenderNames(This,retval)	\
    ( (This)->lpVtbl -> get_ExtenderNames(This,retval) ) 

#define CodeFunction2_get_Extender(This,ExtenderName,retval)	\
    ( (This)->lpVtbl -> get_Extender(This,ExtenderName,retval) ) 

#define CodeFunction2_get_ExtenderCATID(This,retval)	\
    ( (This)->lpVtbl -> get_ExtenderCATID(This,retval) ) 

#define CodeFunction2_GetStartPoint(This,Part,retval)	\
    ( (This)->lpVtbl -> GetStartPoint(This,Part,retval) ) 

#define CodeFunction2_GetEndPoint(This,Part,retval)	\
    ( (This)->lpVtbl -> GetEndPoint(This,Part,retval) ) 

#define CodeFunction2_get_Parent(This,retval)	\
    ( (This)->lpVtbl -> get_Parent(This,retval) ) 

#define CodeFunction2_get_FunctionKind(This,retval)	\
    ( (This)->lpVtbl -> get_FunctionKind(This,retval) ) 

#define CodeFunction2_get_Prototype(This,Flags,retval)	\
    ( (This)->lpVtbl -> get_Prototype(This,Flags,retval) ) 

#define CodeFunction2_get_Type(This,retval)	\
    ( (This)->lpVtbl -> get_Type(This,retval) ) 

#define CodeFunction2_put_Type(This,noname,retval)	\
    ( (This)->lpVtbl -> put_Type(This,noname,retval) ) 

#define CodeFunction2_get_Parameters(This,retval)	\
    ( (This)->lpVtbl -> get_Parameters(This,retval) ) 

#define CodeFunction2_put_Access(This,noname,retval)	\
    ( (This)->lpVtbl -> put_Access(This,noname,retval) ) 

#define CodeFunction2_get_Access(This,retval)	\
    ( (This)->lpVtbl -> get_Access(This,retval) ) 

#define CodeFunction2_get_IsOverloaded(This,retval)	\
    ( (This)->lpVtbl -> get_IsOverloaded(This,retval) ) 

#define CodeFunction2_get_IsShared(This,retval)	\
    ( (This)->lpVtbl -> get_IsShared(This,retval) ) 

#define CodeFunction2_put_IsShared(This,noname,retval)	\
    ( (This)->lpVtbl -> put_IsShared(This,noname,retval) ) 

#define CodeFunction2_get_MustImplement(This,retval)	\
    ( (This)->lpVtbl -> get_MustImplement(This,retval) ) 

#define CodeFunction2_put_MustImplement(This,noname,retval)	\
    ( (This)->lpVtbl -> put_MustImplement(This,noname,retval) ) 

#define CodeFunction2_get_Overloads(This,retval)	\
    ( (This)->lpVtbl -> get_Overloads(This,retval) ) 

#define CodeFunction2_get_Attributes(This,retval)	\
    ( (This)->lpVtbl -> get_Attributes(This,retval) ) 

#define CodeFunction2_get_DocComment(This,retval)	\
    ( (This)->lpVtbl -> get_DocComment(This,retval) ) 

#define CodeFunction2_put_DocComment(This,noname,retval)	\
    ( (This)->lpVtbl -> put_DocComment(This,noname,retval) ) 

#define CodeFunction2_get_Comment(This,retval)	\
    ( (This)->lpVtbl -> get_Comment(This,retval) ) 

#define CodeFunction2_put_Comment(This,noname,retval)	\
    ( (This)->lpVtbl -> put_Comment(This,noname,retval) ) 

#define CodeFunction2_AddParameter(This,Name,Type,Position,retval)	\
    ( (This)->lpVtbl -> AddParameter(This,Name,Type,Position,retval) ) 

#define CodeFunction2_AddAttribute(This,Name,Value,Position,retval)	\
    ( (This)->lpVtbl -> AddAttribute(This,Name,Value,Position,retval) ) 

#define CodeFunction2_RemoveParameter(This,Element,retval)	\
    ( (This)->lpVtbl -> RemoveParameter(This,Element,retval) ) 

#define CodeFunction2_get_CanOverride(This,retval)	\
    ( (This)->lpVtbl -> get_CanOverride(This,retval) ) 

#define CodeFunction2_put_CanOverride(This,noname,retval)	\
    ( (This)->lpVtbl -> put_CanOverride(This,noname,retval) ) 


#define CodeFunction2_get_OverrideKind(This,Kind)	\
    ( (This)->lpVtbl -> get_OverrideKind(This,Kind) ) 

#define CodeFunction2_put_OverrideKind(This,Kind)	\
    ( (This)->lpVtbl -> put_OverrideKind(This,Kind) ) 

#define CodeFunction2_get_IsGeneric(This,pIsGeneric)	\
    ( (This)->lpVtbl -> get_IsGeneric(This,pIsGeneric) ) 

#endif /* COBJMACROS */


#endif 	/* C style interface */




#endif 	/* __CodeFunction2_INTERFACE_DEFINED__ */


#ifndef __CodeAttribute2_INTERFACE_DEFINED__
#define __CodeAttribute2_INTERFACE_DEFINED__

/* interface CodeAttribute2 */
/* [helpstringcontext][helpstring][helpcontext][uuid][object][oleautomation][dual] */ 


EXTERN_C const IID IID_CodeAttribute2;

#if defined(__cplusplus) && !defined(CINTERFACE)
    
    MIDL_INTERFACE("35187E2A-E5F6-4F89-A4CE-DA254640855B")
    CodeAttribute2 : public CodeAttribute
    {
    public:
        virtual /* [helpstringcontext][helpstring][helpcontext][propget][id] */ HRESULT STDMETHODCALLTYPE get_Target( 
            /* [retval][out] */ __RPC__deref_out_opt BSTR *Target) = 0;
        
        virtual /* [helpstringcontext][helpstring][helpcontext][propput][id] */ HRESULT STDMETHODCALLTYPE put_Target( 
            __RPC__in BSTR Target) = 0;
        
        virtual /* [helpstringcontext][helpstring][helpcontext][id] */ HRESULT STDMETHODCALLTYPE AddArgument( 
            __RPC__in BSTR Value,
            /* [optional][in] */ VARIANT Name,
            /* [optional][in] */ VARIANT Position,
            /* [retval][out] */ __RPC__deref_out_opt CodeAttributeArgument **Argument) = 0;
        
        virtual /* [helpstring][propget][id] */ HRESULT STDMETHODCALLTYPE get_Arguments( 
            /* [retval][out] */ __RPC__deref_out_opt /* external definition not present */ CodeElements **ppCodeAttributeArguments) = 0;
        
    };
    
#else 	/* C style interface */

    typedef struct CodeAttribute2Vtbl
    {
        BEGIN_INTERFACE
        
        /* [id][restricted][funcdescattr] */ HRESULT ( STDMETHODCALLTYPE *QueryInterface )( 
            CodeAttribute2 * This,
            /* [in][idldescattr] */ __RPC__in struct GUID *riid,
            /* [out][idldescattr] */ __RPC__deref_out_opt void **ppvObj,
            /* [retval][out] */ __RPC__out void *retval);
        
        /* [id][restricted][funcdescattr] */ HRESULT ( STDMETHODCALLTYPE *AddRef )( 
            CodeAttribute2 * This,
            /* [retval][out] */ __RPC__out unsigned long *retval);
        
        /* [id][restricted][funcdescattr] */ HRESULT ( STDMETHODCALLTYPE *Release )( 
            CodeAttribute2 * This,
            /* [retval][out] */ __RPC__out unsigned long *retval);
        
        /* [id][restricted][funcdescattr] */ HRESULT ( STDMETHODCALLTYPE *GetTypeInfoCount )( 
            CodeAttribute2 * This,
            /* [out][idldescattr] */ __RPC__out unsigned UINT *pctinfo,
            /* [retval][out] */ __RPC__out void *retval);
        
        /* [id][restricted][funcdescattr] */ HRESULT ( STDMETHODCALLTYPE *GetTypeInfo )( 
            CodeAttribute2 * This,
            /* [in][idldescattr] */ unsigned UINT itinfo,
            /* [in][idldescattr] */ unsigned long lcid,
            /* [out][idldescattr] */ __RPC__deref_out_opt void **pptinfo,
            /* [retval][out] */ __RPC__out void *retval);
        
        /* [id][restricted][funcdescattr] */ HRESULT ( STDMETHODCALLTYPE *GetIDsOfNames )( 
            CodeAttribute2 * This,
            /* [in][idldescattr] */ __RPC__in struct GUID *riid,
            /* [in][idldescattr] */ __RPC__deref_in_opt signed char **rgszNames,
            /* [in][idldescattr] */ unsigned UINT cNames,
            /* [in][idldescattr] */ unsigned long lcid,
            /* [out][idldescattr] */ __RPC__out signed long *rgdispid,
            /* [retval][out] */ __RPC__out void *retval);
        
        /* [id][restricted][funcdescattr] */ HRESULT ( STDMETHODCALLTYPE *Invoke )( 
            CodeAttribute2 * This,
            /* [in][idldescattr] */ signed long dispidMember,
            /* [in][idldescattr] */ __RPC__in struct GUID *riid,
            /* [in][idldescattr] */ unsigned long lcid,
            /* [in][idldescattr] */ unsigned short wFlags,
            /* [in][idldescattr] */ __RPC__in struct DISPPARAMS *pdispparams,
            /* [out][idldescattr] */ __RPC__out VARIANT *pvarResult,
            /* [out][idldescattr] */ __RPC__out struct EXCEPINFO *pexcepinfo,
            /* [out][idldescattr] */ __RPC__out unsigned UINT *puArgErr,
            /* [retval][out] */ __RPC__out void *retval);
        
        /* [id][propget][funcdescattr] */ HRESULT ( STDMETHODCALLTYPE *get_DTE )( 
            CodeAttribute2 * This,
            /* [retval][out] */ __RPC__deref_out_opt **retval);
        
        /* [id][propget][funcdescattr] */ HRESULT ( STDMETHODCALLTYPE *get_Collection )( 
            CodeAttribute2 * This,
            /* [retval][out] */ __RPC__deref_out_opt CodeElements **retval);
        
        /* [id][propget][funcdescattr] */ HRESULT ( STDMETHODCALLTYPE *get_Name )( 
            CodeAttribute2 * This,
            /* [retval][out] */ __RPC__deref_out_opt BSTR *retval);
        
        /* [id][propput][funcdescattr] */ HRESULT ( STDMETHODCALLTYPE *put_Name )( 
            CodeAttribute2 * This,
            /* [idldescattr] */ __RPC__in BSTR noname,
            /* [retval][out] */ __RPC__out void *retval);
        
        /* [id][propget][funcdescattr] */ HRESULT ( STDMETHODCALLTYPE *get_FullName )( 
            CodeAttribute2 * This,
            /* [retval][out] */ __RPC__deref_out_opt BSTR *retval);
        
        /* [id][propget][funcdescattr] */ HRESULT ( STDMETHODCALLTYPE *get_ProjectItem )( 
            CodeAttribute2 * This,
            /* [retval][out] */ __RPC__deref_out_opt ProjectItem **retval);
        
        /* [id][propget][funcdescattr] */ HRESULT ( STDMETHODCALLTYPE *get_Kind )( 
            CodeAttribute2 * This,
            /* [retval][out] */ __RPC__out enum vsCMElement *retval);
        
        /* [id][propget][funcdescattr] */ HRESULT ( STDMETHODCALLTYPE *get_IsCodeType )( 
            CodeAttribute2 * This,
            /* [retval][out] */ __RPC__out BOOLEAN *retval);
        
        /* [id][propget][funcdescattr] */ HRESULT ( STDMETHODCALLTYPE *get_InfoLocation )( 
            CodeAttribute2 * This,
            /* [retval][out] */ __RPC__out enum vsCMInfoLocation *retval);
        
        /* [id][propget][funcdescattr] */ HRESULT ( STDMETHODCALLTYPE *get_Children )( 
            CodeAttribute2 * This,
            /* [retval][out] */ __RPC__deref_out_opt CodeElements **retval);
        
        /* [id][propget][funcdescattr] */ HRESULT ( STDMETHODCALLTYPE *get_Language )( 
            CodeAttribute2 * This,
            /* [retval][out] */ __RPC__deref_out_opt BSTR *retval);
        
        /* [id][propget][funcdescattr] */ HRESULT ( STDMETHODCALLTYPE *get_StartPoint )( 
            CodeAttribute2 * This,
            /* [retval][out] */ __RPC__deref_out_opt TextPoint **retval);
        
        /* [id][propget][funcdescattr] */ HRESULT ( STDMETHODCALLTYPE *get_EndPoint )( 
            CodeAttribute2 * This,
            /* [retval][out] */ __RPC__deref_out_opt TextPoint **retval);
        
        /* [id][propget][funcdescattr] */ HRESULT ( STDMETHODCALLTYPE *get_ExtenderNames )( 
            CodeAttribute2 * This,
            /* [retval][out] */ __RPC__out VARIANT *retval);
        
        /* [id][propget][funcdescattr] */ HRESULT ( STDMETHODCALLTYPE *get_Extender )( 
            CodeAttribute2 * This,
            /* [idldescattr] */ __RPC__in BSTR ExtenderName,
            /* [retval][out] */ __RPC__deref_out_opt IDispatch **retval);
        
        /* [id][propget][funcdescattr] */ HRESULT ( STDMETHODCALLTYPE *get_ExtenderCATID )( 
            CodeAttribute2 * This,
            /* [retval][out] */ __RPC__deref_out_opt BSTR *retval);
        
        /* [id][funcdescattr] */ HRESULT ( STDMETHODCALLTYPE *GetStartPoint )( 
            CodeAttribute2 * This,
            /* [in][idldescattr] */ enum vsCMPart Part,
            /* [retval][out] */ __RPC__deref_out_opt TextPoint **retval);
        
        /* [id][funcdescattr] */ HRESULT ( STDMETHODCALLTYPE *GetEndPoint )( 
            CodeAttribute2 * This,
            /* [in][idldescattr] */ enum vsCMPart Part,
            /* [retval][out] */ __RPC__deref_out_opt TextPoint **retval);
        
        /* [id][propget][funcdescattr] */ HRESULT ( STDMETHODCALLTYPE *get_Parent )( 
            CodeAttribute2 * This,
            /* [retval][out] */ __RPC__deref_out_opt IDispatch **retval);
        
        /* [id][propget][funcdescattr] */ HRESULT ( STDMETHODCALLTYPE *get_Value )( 
            CodeAttribute2 * This,
            /* [retval][out] */ __RPC__deref_out_opt BSTR *retval);
        
        /* [id][propput][funcdescattr] */ HRESULT ( STDMETHODCALLTYPE *put_Value )( 
            CodeAttribute2 * This,
            /* [idldescattr] */ __RPC__in BSTR noname,
            /* [retval][out] */ __RPC__out void *retval);
        
        /* [id][funcdescattr] */ HRESULT ( STDMETHODCALLTYPE *Delete )( 
            CodeAttribute2 * This,
            /* [retval][out] */ __RPC__out void *retval);
        
        /* [helpstringcontext][helpstring][helpcontext][propget][id] */ HRESULT ( STDMETHODCALLTYPE *get_Target )( 
            CodeAttribute2 * This,
            /* [retval][out] */ __RPC__deref_out_opt BSTR *Target);
        
        /* [helpstringcontext][helpstring][helpcontext][propput][id] */ HRESULT ( STDMETHODCALLTYPE *put_Target )( 
            CodeAttribute2 * This,
            __RPC__in BSTR Target);
        
        /* [helpstringcontext][helpstring][helpcontext][id] */ HRESULT ( STDMETHODCALLTYPE *AddArgument )( 
            CodeAttribute2 * This,
            __RPC__in BSTR Value,
            /* [optional][in] */ VARIANT Name,
            /* [optional][in] */ VARIANT Position,
            /* [retval][out] */ __RPC__deref_out_opt CodeAttributeArgument **Argument);
        
        /* [helpstring][propget][id] */ HRESULT ( STDMETHODCALLTYPE *get_Arguments )( 
            CodeAttribute2 * This,
            /* [retval][out] */ __RPC__deref_out_opt /* external definition not present */ CodeElements **ppCodeAttributeArguments);
        
        END_INTERFACE
    } CodeAttribute2Vtbl;

    interface CodeAttribute2
    {
        CONST_VTBL struct CodeAttribute2Vtbl *lpVtbl;
    };

    

#ifdef COBJMACROS


#define CodeAttribute2_QueryInterface(This,riid,ppvObj,retval)	\
    ( (This)->lpVtbl -> QueryInterface(This,riid,ppvObj,retval) ) 

#define CodeAttribute2_AddRef(This,retval)	\
    ( (This)->lpVtbl -> AddRef(This,retval) ) 

#define CodeAttribute2_Release(This,retval)	\
    ( (This)->lpVtbl -> Release(This,retval) ) 

#define CodeAttribute2_GetTypeInfoCount(This,pctinfo,retval)	\
    ( (This)->lpVtbl -> GetTypeInfoCount(This,pctinfo,retval) ) 

#define CodeAttribute2_GetTypeInfo(This,itinfo,lcid,pptinfo,retval)	\
    ( (This)->lpVtbl -> GetTypeInfo(This,itinfo,lcid,pptinfo,retval) ) 

#define CodeAttribute2_GetIDsOfNames(This,riid,rgszNames,cNames,lcid,rgdispid,retval)	\
    ( (This)->lpVtbl -> GetIDsOfNames(This,riid,rgszNames,cNames,lcid,rgdispid,retval) ) 

#define CodeAttribute2_Invoke(This,dispidMember,riid,lcid,wFlags,pdispparams,pvarResult,pexcepinfo,puArgErr,retval)	\
    ( (This)->lpVtbl -> Invoke(This,dispidMember,riid,lcid,wFlags,pdispparams,pvarResult,pexcepinfo,puArgErr,retval) ) 

#define CodeAttribute2_get_DTE(This,retval)	\
    ( (This)->lpVtbl -> get_DTE(This,retval) ) 

#define CodeAttribute2_get_Collection(This,retval)	\
    ( (This)->lpVtbl -> get_Collection(This,retval) ) 

#define CodeAttribute2_get_Name(This,retval)	\
    ( (This)->lpVtbl -> get_Name(This,retval) ) 

#define CodeAttribute2_put_Name(This,noname,retval)	\
    ( (This)->lpVtbl -> put_Name(This,noname,retval) ) 

#define CodeAttribute2_get_FullName(This,retval)	\
    ( (This)->lpVtbl -> get_FullName(This,retval) ) 

#define CodeAttribute2_get_ProjectItem(This,retval)	\
    ( (This)->lpVtbl -> get_ProjectItem(This,retval) ) 

#define CodeAttribute2_get_Kind(This,retval)	\
    ( (This)->lpVtbl -> get_Kind(This,retval) ) 

#define CodeAttribute2_get_IsCodeType(This,retval)	\
    ( (This)->lpVtbl -> get_IsCodeType(This,retval) ) 

#define CodeAttribute2_get_InfoLocation(This,retval)	\
    ( (This)->lpVtbl -> get_InfoLocation(This,retval) ) 

#define CodeAttribute2_get_Children(This,retval)	\
    ( (This)->lpVtbl -> get_Children(This,retval) ) 

#define CodeAttribute2_get_Language(This,retval)	\
    ( (This)->lpVtbl -> get_Language(This,retval) ) 

#define CodeAttribute2_get_StartPoint(This,retval)	\
    ( (This)->lpVtbl -> get_StartPoint(This,retval) ) 

#define CodeAttribute2_get_EndPoint(This,retval)	\
    ( (This)->lpVtbl -> get_EndPoint(This,retval) ) 

#define CodeAttribute2_get_ExtenderNames(This,retval)	\
    ( (This)->lpVtbl -> get_ExtenderNames(This,retval) ) 

#define CodeAttribute2_get_Extender(This,ExtenderName,retval)	\
    ( (This)->lpVtbl -> get_Extender(This,ExtenderName,retval) ) 

#define CodeAttribute2_get_ExtenderCATID(This,retval)	\
    ( (This)->lpVtbl -> get_ExtenderCATID(This,retval) ) 

#define CodeAttribute2_GetStartPoint(This,Part,retval)	\
    ( (This)->lpVtbl -> GetStartPoint(This,Part,retval) ) 

#define CodeAttribute2_GetEndPoint(This,Part,retval)	\
    ( (This)->lpVtbl -> GetEndPoint(This,Part,retval) ) 

#define CodeAttribute2_get_Parent(This,retval)	\
    ( (This)->lpVtbl -> get_Parent(This,retval) ) 

#define CodeAttribute2_get_Value(This,retval)	\
    ( (This)->lpVtbl -> get_Value(This,retval) ) 

#define CodeAttribute2_put_Value(This,noname,retval)	\
    ( (This)->lpVtbl -> put_Value(This,noname,retval) ) 

#define CodeAttribute2_Delete(This,retval)	\
    ( (This)->lpVtbl -> Delete(This,retval) ) 


#define CodeAttribute2_get_Target(This,Target)	\
    ( (This)->lpVtbl -> get_Target(This,Target) ) 

#define CodeAttribute2_put_Target(This,Target)	\
    ( (This)->lpVtbl -> put_Target(This,Target) ) 

#define CodeAttribute2_AddArgument(This,Value,Name,Position,Argument)	\
    ( (This)->lpVtbl -> AddArgument(This,Value,Name,Position,Argument) ) 

#define CodeAttribute2_get_Arguments(This,ppCodeAttributeArguments)	\
    ( (This)->lpVtbl -> get_Arguments(This,ppCodeAttributeArguments) ) 

#endif /* COBJMACROS */


#endif 	/* C style interface */




#endif 	/* __CodeAttribute2_INTERFACE_DEFINED__ */


#ifndef __CodeVariable2_INTERFACE_DEFINED__
#define __CodeVariable2_INTERFACE_DEFINED__

/* interface CodeVariable2 */
/* [helpstringcontext][helpstring][helpcontext][uuid][object][oleautomation][dual] */ 


EXTERN_C const IID IID_CodeVariable2;

#if defined(__cplusplus) && !defined(CINTERFACE)
    
    MIDL_INTERFACE("F747A8A0-01B1-4DA2-9259-0D5A0CB4C049")
    CodeVariable2 : public CodeVariable
    {
    public:
        virtual /* [helpstringcontext][helpstring][helpcontext][propget][id] */ HRESULT STDMETHODCALLTYPE get_ConstKind( 
            /* [retval][out] */ __RPC__out vsCMConstKind *Kind) = 0;
        
        virtual /* [helpstringcontext][helpstring][helpcontext][propput][id] */ HRESULT STDMETHODCALLTYPE put_ConstKind( 
            vsCMConstKind Kind) = 0;
        
        virtual /* [helpstring][propget][id] */ HRESULT STDMETHODCALLTYPE get_IsGeneric( 
            /* [retval][out] */ __RPC__out VARIANT_BOOL *pIsGeneric) = 0;
        
    };
    
#else 	/* C style interface */

    typedef struct CodeVariable2Vtbl
    {
        BEGIN_INTERFACE
        
        /* [id][restricted][funcdescattr] */ HRESULT ( STDMETHODCALLTYPE *QueryInterface )( 
            CodeVariable2 * This,
            /* [in][idldescattr] */ __RPC__in struct GUID *riid,
            /* [out][idldescattr] */ __RPC__deref_out_opt void **ppvObj,
            /* [retval][out] */ __RPC__out void *retval);
        
        /* [id][restricted][funcdescattr] */ HRESULT ( STDMETHODCALLTYPE *AddRef )( 
            CodeVariable2 * This,
            /* [retval][out] */ __RPC__out unsigned long *retval);
        
        /* [id][restricted][funcdescattr] */ HRESULT ( STDMETHODCALLTYPE *Release )( 
            CodeVariable2 * This,
            /* [retval][out] */ __RPC__out unsigned long *retval);
        
        /* [id][restricted][funcdescattr] */ HRESULT ( STDMETHODCALLTYPE *GetTypeInfoCount )( 
            CodeVariable2 * This,
            /* [out][idldescattr] */ __RPC__out unsigned UINT *pctinfo,
            /* [retval][out] */ __RPC__out void *retval);
        
        /* [id][restricted][funcdescattr] */ HRESULT ( STDMETHODCALLTYPE *GetTypeInfo )( 
            CodeVariable2 * This,
            /* [in][idldescattr] */ unsigned UINT itinfo,
            /* [in][idldescattr] */ unsigned long lcid,
            /* [out][idldescattr] */ __RPC__deref_out_opt void **pptinfo,
            /* [retval][out] */ __RPC__out void *retval);
        
        /* [id][restricted][funcdescattr] */ HRESULT ( STDMETHODCALLTYPE *GetIDsOfNames )( 
            CodeVariable2 * This,
            /* [in][idldescattr] */ __RPC__in struct GUID *riid,
            /* [in][idldescattr] */ __RPC__deref_in_opt signed char **rgszNames,
            /* [in][idldescattr] */ unsigned UINT cNames,
            /* [in][idldescattr] */ unsigned long lcid,
            /* [out][idldescattr] */ __RPC__out signed long *rgdispid,
            /* [retval][out] */ __RPC__out void *retval);
        
        /* [id][restricted][funcdescattr] */ HRESULT ( STDMETHODCALLTYPE *Invoke )( 
            CodeVariable2 * This,
            /* [in][idldescattr] */ signed long dispidMember,
            /* [in][idldescattr] */ __RPC__in struct GUID *riid,
            /* [in][idldescattr] */ unsigned long lcid,
            /* [in][idldescattr] */ unsigned short wFlags,
            /* [in][idldescattr] */ __RPC__in struct DISPPARAMS *pdispparams,
            /* [out][idldescattr] */ __RPC__out VARIANT *pvarResult,
            /* [out][idldescattr] */ __RPC__out struct EXCEPINFO *pexcepinfo,
            /* [out][idldescattr] */ __RPC__out unsigned UINT *puArgErr,
            /* [retval][out] */ __RPC__out void *retval);
        
        /* [id][propget][funcdescattr] */ HRESULT ( STDMETHODCALLTYPE *get_DTE )( 
            CodeVariable2 * This,
            /* [retval][out] */ __RPC__deref_out_opt **retval);
        
        /* [id][propget][funcdescattr] */ HRESULT ( STDMETHODCALLTYPE *get_Collection )( 
            CodeVariable2 * This,
            /* [retval][out] */ __RPC__deref_out_opt CodeElements **retval);
        
        /* [id][propget][funcdescattr] */ HRESULT ( STDMETHODCALLTYPE *get_Name )( 
            CodeVariable2 * This,
            /* [retval][out] */ __RPC__deref_out_opt BSTR *retval);
        
        /* [id][propput][funcdescattr] */ HRESULT ( STDMETHODCALLTYPE *put_Name )( 
            CodeVariable2 * This,
            /* [idldescattr] */ __RPC__in BSTR noname,
            /* [retval][out] */ __RPC__out void *retval);
        
        /* [id][propget][funcdescattr] */ HRESULT ( STDMETHODCALLTYPE *get_FullName )( 
            CodeVariable2 * This,
            /* [retval][out] */ __RPC__deref_out_opt BSTR *retval);
        
        /* [id][propget][funcdescattr] */ HRESULT ( STDMETHODCALLTYPE *get_ProjectItem )( 
            CodeVariable2 * This,
            /* [retval][out] */ __RPC__deref_out_opt ProjectItem **retval);
        
        /* [id][propget][funcdescattr] */ HRESULT ( STDMETHODCALLTYPE *get_Kind )( 
            CodeVariable2 * This,
            /* [retval][out] */ __RPC__out enum vsCMElement *retval);
        
        /* [id][propget][funcdescattr] */ HRESULT ( STDMETHODCALLTYPE *get_IsCodeType )( 
            CodeVariable2 * This,
            /* [retval][out] */ __RPC__out BOOLEAN *retval);
        
        /* [id][propget][funcdescattr] */ HRESULT ( STDMETHODCALLTYPE *get_InfoLocation )( 
            CodeVariable2 * This,
            /* [retval][out] */ __RPC__out enum vsCMInfoLocation *retval);
        
        /* [id][propget][funcdescattr] */ HRESULT ( STDMETHODCALLTYPE *get_Children )( 
            CodeVariable2 * This,
            /* [retval][out] */ __RPC__deref_out_opt CodeElements **retval);
        
        /* [id][propget][funcdescattr] */ HRESULT ( STDMETHODCALLTYPE *get_Language )( 
            CodeVariable2 * This,
            /* [retval][out] */ __RPC__deref_out_opt BSTR *retval);
        
        /* [id][propget][funcdescattr] */ HRESULT ( STDMETHODCALLTYPE *get_StartPoint )( 
            CodeVariable2 * This,
            /* [retval][out] */ __RPC__deref_out_opt TextPoint **retval);
        
        /* [id][propget][funcdescattr] */ HRESULT ( STDMETHODCALLTYPE *get_EndPoint )( 
            CodeVariable2 * This,
            /* [retval][out] */ __RPC__deref_out_opt TextPoint **retval);
        
        /* [id][propget][funcdescattr] */ HRESULT ( STDMETHODCALLTYPE *get_ExtenderNames )( 
            CodeVariable2 * This,
            /* [retval][out] */ __RPC__out VARIANT *retval);
        
        /* [id][propget][funcdescattr] */ HRESULT ( STDMETHODCALLTYPE *get_Extender )( 
            CodeVariable2 * This,
            /* [idldescattr] */ __RPC__in BSTR ExtenderName,
            /* [retval][out] */ __RPC__deref_out_opt IDispatch **retval);
        
        /* [id][propget][funcdescattr] */ HRESULT ( STDMETHODCALLTYPE *get_ExtenderCATID )( 
            CodeVariable2 * This,
            /* [retval][out] */ __RPC__deref_out_opt BSTR *retval);
        
        /* [id][funcdescattr] */ HRESULT ( STDMETHODCALLTYPE *GetStartPoint )( 
            CodeVariable2 * This,
            /* [in][idldescattr] */ enum vsCMPart Part,
            /* [retval][out] */ __RPC__deref_out_opt TextPoint **retval);
        
        /* [id][funcdescattr] */ HRESULT ( STDMETHODCALLTYPE *GetEndPoint )( 
            CodeVariable2 * This,
            /* [in][idldescattr] */ enum vsCMPart Part,
            /* [retval][out] */ __RPC__deref_out_opt TextPoint **retval);
        
        /* [id][propget][funcdescattr] */ HRESULT ( STDMETHODCALLTYPE *get_Parent )( 
            CodeVariable2 * This,
            /* [retval][out] */ __RPC__deref_out_opt IDispatch **retval);
        
        /* [id][propget][funcdescattr] */ HRESULT ( STDMETHODCALLTYPE *get_InitExpression )( 
            CodeVariable2 * This,
            /* [retval][out] */ __RPC__out VARIANT *retval);
        
        /* [id][propput][funcdescattr] */ HRESULT ( STDMETHODCALLTYPE *put_InitExpression )( 
            CodeVariable2 * This,
            /* [idldescattr] */ VARIANT noname,
            /* [retval][out] */ __RPC__out void *retval);
        
        /* [id][propget][funcdescattr] */ HRESULT ( STDMETHODCALLTYPE *get_Prototype )( 
            CodeVariable2 * This,
            /* [in][idldescattr] */ signed long Flags,
            /* [retval][out] */ __RPC__deref_out_opt BSTR *retval);
        
        /* [id][propput][funcdescattr] */ HRESULT ( STDMETHODCALLTYPE *put_Type )( 
            CodeVariable2 * This,
            /* [idldescattr] */ __RPC__in_opt CodeTypeRef *noname,
            /* [retval][out] */ __RPC__out void *retval);
        
        /* [id][propget][funcdescattr] */ HRESULT ( STDMETHODCALLTYPE *get_Type )( 
            CodeVariable2 * This,
            /* [retval][out] */ __RPC__deref_out_opt CodeTypeRef **retval);
        
        /* [id][propput][funcdescattr] */ HRESULT ( STDMETHODCALLTYPE *put_Access )( 
            CodeVariable2 * This,
            /* [idldescattr] */ enum vsCMAccess noname,
            /* [retval][out] */ __RPC__out void *retval);
        
        /* [id][propget][funcdescattr] */ HRESULT ( STDMETHODCALLTYPE *get_Access )( 
            CodeVariable2 * This,
            /* [retval][out] */ __RPC__out enum vsCMAccess *retval);
        
        /* [id][propget][funcdescattr] */ HRESULT ( STDMETHODCALLTYPE *get_IsConstant )( 
            CodeVariable2 * This,
            /* [retval][out] */ __RPC__out BOOLEAN *retval);
        
        /* [id][propput][funcdescattr] */ HRESULT ( STDMETHODCALLTYPE *put_IsConstant )( 
            CodeVariable2 * This,
            /* [idldescattr] */ BOOLEAN noname,
            /* [retval][out] */ __RPC__out void *retval);
        
        /* [id][propget][funcdescattr] */ HRESULT ( STDMETHODCALLTYPE *get_Attributes )( 
            CodeVariable2 * This,
            /* [retval][out] */ __RPC__deref_out_opt CodeElements **retval);
        
        /* [id][propget][funcdescattr] */ HRESULT ( STDMETHODCALLTYPE *get_DocComment )( 
            CodeVariable2 * This,
            /* [retval][out] */ __RPC__deref_out_opt BSTR *retval);
        
        /* [id][propput][funcdescattr] */ HRESULT ( STDMETHODCALLTYPE *put_DocComment )( 
            CodeVariable2 * This,
            /* [idldescattr] */ __RPC__in BSTR noname,
            /* [retval][out] */ __RPC__out void *retval);
        
        /* [id][propget][funcdescattr] */ HRESULT ( STDMETHODCALLTYPE *get_Comment )( 
            CodeVariable2 * This,
            /* [retval][out] */ __RPC__deref_out_opt BSTR *retval);
        
        /* [id][propput][funcdescattr] */ HRESULT ( STDMETHODCALLTYPE *put_Comment )( 
            CodeVariable2 * This,
            /* [idldescattr] */ __RPC__in BSTR noname,
            /* [retval][out] */ __RPC__out void *retval);
        
        /* [id][funcdescattr] */ HRESULT ( STDMETHODCALLTYPE *AddAttribute )( 
            CodeVariable2 * This,
            /* [idldescattr] */ __RPC__in BSTR Name,
            /* [idldescattr] */ __RPC__in BSTR Value,
            /* [optional][idldescattr] */ VARIANT Position,
            /* [retval][out] */ __RPC__deref_out_opt CodeAttribute **retval);
        
        /* [id][propget][funcdescattr] */ HRESULT ( STDMETHODCALLTYPE *get_IsShared )( 
            CodeVariable2 * This,
            /* [retval][out] */ __RPC__out BOOLEAN *retval);
        
        /* [id][propput][funcdescattr] */ HRESULT ( STDMETHODCALLTYPE *put_IsShared )( 
            CodeVariable2 * This,
            /* [idldescattr] */ BOOLEAN noname,
            /* [retval][out] */ __RPC__out void *retval);
        
        /* [helpstringcontext][helpstring][helpcontext][propget][id] */ HRESULT ( STDMETHODCALLTYPE *get_ConstKind )( 
            CodeVariable2 * This,
            /* [retval][out] */ __RPC__out vsCMConstKind *Kind);
        
        /* [helpstringcontext][helpstring][helpcontext][propput][id] */ HRESULT ( STDMETHODCALLTYPE *put_ConstKind )( 
            CodeVariable2 * This,
            vsCMConstKind Kind);
        
        /* [helpstring][propget][id] */ HRESULT ( STDMETHODCALLTYPE *get_IsGeneric )( 
            CodeVariable2 * This,
            /* [retval][out] */ __RPC__out VARIANT_BOOL *pIsGeneric);
        
        END_INTERFACE
    } CodeVariable2Vtbl;

    interface CodeVariable2
    {
        CONST_VTBL struct CodeVariable2Vtbl *lpVtbl;
    };

    

#ifdef COBJMACROS


#define CodeVariable2_QueryInterface(This,riid,ppvObj,retval)	\
    ( (This)->lpVtbl -> QueryInterface(This,riid,ppvObj,retval) ) 

#define CodeVariable2_AddRef(This,retval)	\
    ( (This)->lpVtbl -> AddRef(This,retval) ) 

#define CodeVariable2_Release(This,retval)	\
    ( (This)->lpVtbl -> Release(This,retval) ) 

#define CodeVariable2_GetTypeInfoCount(This,pctinfo,retval)	\
    ( (This)->lpVtbl -> GetTypeInfoCount(This,pctinfo,retval) ) 

#define CodeVariable2_GetTypeInfo(This,itinfo,lcid,pptinfo,retval)	\
    ( (This)->lpVtbl -> GetTypeInfo(This,itinfo,lcid,pptinfo,retval) ) 

#define CodeVariable2_GetIDsOfNames(This,riid,rgszNames,cNames,lcid,rgdispid,retval)	\
    ( (This)->lpVtbl -> GetIDsOfNames(This,riid,rgszNames,cNames,lcid,rgdispid,retval) ) 

#define CodeVariable2_Invoke(This,dispidMember,riid,lcid,wFlags,pdispparams,pvarResult,pexcepinfo,puArgErr,retval)	\
    ( (This)->lpVtbl -> Invoke(This,dispidMember,riid,lcid,wFlags,pdispparams,pvarResult,pexcepinfo,puArgErr,retval) ) 

#define CodeVariable2_get_DTE(This,retval)	\
    ( (This)->lpVtbl -> get_DTE(This,retval) ) 

#define CodeVariable2_get_Collection(This,retval)	\
    ( (This)->lpVtbl -> get_Collection(This,retval) ) 

#define CodeVariable2_get_Name(This,retval)	\
    ( (This)->lpVtbl -> get_Name(This,retval) ) 

#define CodeVariable2_put_Name(This,noname,retval)	\
    ( (This)->lpVtbl -> put_Name(This,noname,retval) ) 

#define CodeVariable2_get_FullName(This,retval)	\
    ( (This)->lpVtbl -> get_FullName(This,retval) ) 

#define CodeVariable2_get_ProjectItem(This,retval)	\
    ( (This)->lpVtbl -> get_ProjectItem(This,retval) ) 

#define CodeVariable2_get_Kind(This,retval)	\
    ( (This)->lpVtbl -> get_Kind(This,retval) ) 

#define CodeVariable2_get_IsCodeType(This,retval)	\
    ( (This)->lpVtbl -> get_IsCodeType(This,retval) ) 

#define CodeVariable2_get_InfoLocation(This,retval)	\
    ( (This)->lpVtbl -> get_InfoLocation(This,retval) ) 

#define CodeVariable2_get_Children(This,retval)	\
    ( (This)->lpVtbl -> get_Children(This,retval) ) 

#define CodeVariable2_get_Language(This,retval)	\
    ( (This)->lpVtbl -> get_Language(This,retval) ) 

#define CodeVariable2_get_StartPoint(This,retval)	\
    ( (This)->lpVtbl -> get_StartPoint(This,retval) ) 

#define CodeVariable2_get_EndPoint(This,retval)	\
    ( (This)->lpVtbl -> get_EndPoint(This,retval) ) 

#define CodeVariable2_get_ExtenderNames(This,retval)	\
    ( (This)->lpVtbl -> get_ExtenderNames(This,retval) ) 

#define CodeVariable2_get_Extender(This,ExtenderName,retval)	\
    ( (This)->lpVtbl -> get_Extender(This,ExtenderName,retval) ) 

#define CodeVariable2_get_ExtenderCATID(This,retval)	\
    ( (This)->lpVtbl -> get_ExtenderCATID(This,retval) ) 

#define CodeVariable2_GetStartPoint(This,Part,retval)	\
    ( (This)->lpVtbl -> GetStartPoint(This,Part,retval) ) 

#define CodeVariable2_GetEndPoint(This,Part,retval)	\
    ( (This)->lpVtbl -> GetEndPoint(This,Part,retval) ) 

#define CodeVariable2_get_Parent(This,retval)	\
    ( (This)->lpVtbl -> get_Parent(This,retval) ) 

#define CodeVariable2_get_InitExpression(This,retval)	\
    ( (This)->lpVtbl -> get_InitExpression(This,retval) ) 

#define CodeVariable2_put_InitExpression(This,noname,retval)	\
    ( (This)->lpVtbl -> put_InitExpression(This,noname,retval) ) 

#define CodeVariable2_get_Prototype(This,Flags,retval)	\
    ( (This)->lpVtbl -> get_Prototype(This,Flags,retval) ) 

#define CodeVariable2_put_Type(This,noname,retval)	\
    ( (This)->lpVtbl -> put_Type(This,noname,retval) ) 

#define CodeVariable2_get_Type(This,retval)	\
    ( (This)->lpVtbl -> get_Type(This,retval) ) 

#define CodeVariable2_put_Access(This,noname,retval)	\
    ( (This)->lpVtbl -> put_Access(This,noname,retval) ) 

#define CodeVariable2_get_Access(This,retval)	\
    ( (This)->lpVtbl -> get_Access(This,retval) ) 

#define CodeVariable2_get_IsConstant(This,retval)	\
    ( (This)->lpVtbl -> get_IsConstant(This,retval) ) 

#define CodeVariable2_put_IsConstant(This,noname,retval)	\
    ( (This)->lpVtbl -> put_IsConstant(This,noname,retval) ) 

#define CodeVariable2_get_Attributes(This,retval)	\
    ( (This)->lpVtbl -> get_Attributes(This,retval) ) 

#define CodeVariable2_get_DocComment(This,retval)	\
    ( (This)->lpVtbl -> get_DocComment(This,retval) ) 

#define CodeVariable2_put_DocComment(This,noname,retval)	\
    ( (This)->lpVtbl -> put_DocComment(This,noname,retval) ) 

#define CodeVariable2_get_Comment(This,retval)	\
    ( (This)->lpVtbl -> get_Comment(This,retval) ) 

#define CodeVariable2_put_Comment(This,noname,retval)	\
    ( (This)->lpVtbl -> put_Comment(This,noname,retval) ) 

#define CodeVariable2_AddAttribute(This,Name,Value,Position,retval)	\
    ( (This)->lpVtbl -> AddAttribute(This,Name,Value,Position,retval) ) 

#define CodeVariable2_get_IsShared(This,retval)	\
    ( (This)->lpVtbl -> get_IsShared(This,retval) ) 

#define CodeVariable2_put_IsShared(This,noname,retval)	\
    ( (This)->lpVtbl -> put_IsShared(This,noname,retval) ) 


#define CodeVariable2_get_ConstKind(This,Kind)	\
    ( (This)->lpVtbl -> get_ConstKind(This,Kind) ) 

#define CodeVariable2_put_ConstKind(This,Kind)	\
    ( (This)->lpVtbl -> put_ConstKind(This,Kind) ) 

#define CodeVariable2_get_IsGeneric(This,pIsGeneric)	\
    ( (This)->lpVtbl -> get_IsGeneric(This,pIsGeneric) ) 

#endif /* COBJMACROS */


#endif 	/* C style interface */




#endif 	/* __CodeVariable2_INTERFACE_DEFINED__ */


#ifndef __CodeDelegate2_INTERFACE_DEFINED__
#define __CodeDelegate2_INTERFACE_DEFINED__

/* interface CodeDelegate2 */
/* [helpstringcontext][helpstring][helpcontext][uuid][object][oleautomation][dual] */ 


EXTERN_C const IID IID_CodeDelegate2;

#if defined(__cplusplus) && !defined(CINTERFACE)
    
    MIDL_INTERFACE("3b1b24ef-dd8e-4c98-8799-4efac80080e9")
    CodeDelegate2 : public CodeDelegate
    {
    public:
        virtual /* [helpstring][propget][id] */ HRESULT STDMETHODCALLTYPE get_IsGeneric( 
            /* [retval][out] */ __RPC__out VARIANT_BOOL *pIsGeneric) = 0;
        
    };
    
#else 	/* C style interface */

    typedef struct CodeDelegate2Vtbl
    {
        BEGIN_INTERFACE
        
        /* [id][restricted][funcdescattr] */ HRESULT ( STDMETHODCALLTYPE *QueryInterface )( 
            CodeDelegate2 * This,
            /* [in][idldescattr] */ __RPC__in struct GUID *riid,
            /* [out][idldescattr] */ __RPC__deref_out_opt void **ppvObj,
            /* [retval][out] */ __RPC__out void *retval);
        
        /* [id][restricted][funcdescattr] */ HRESULT ( STDMETHODCALLTYPE *AddRef )( 
            CodeDelegate2 * This,
            /* [retval][out] */ __RPC__out unsigned long *retval);
        
        /* [id][restricted][funcdescattr] */ HRESULT ( STDMETHODCALLTYPE *Release )( 
            CodeDelegate2 * This,
            /* [retval][out] */ __RPC__out unsigned long *retval);
        
        /* [id][restricted][funcdescattr] */ HRESULT ( STDMETHODCALLTYPE *GetTypeInfoCount )( 
            CodeDelegate2 * This,
            /* [out][idldescattr] */ __RPC__out unsigned UINT *pctinfo,
            /* [retval][out] */ __RPC__out void *retval);
        
        /* [id][restricted][funcdescattr] */ HRESULT ( STDMETHODCALLTYPE *GetTypeInfo )( 
            CodeDelegate2 * This,
            /* [in][idldescattr] */ unsigned UINT itinfo,
            /* [in][idldescattr] */ unsigned long lcid,
            /* [out][idldescattr] */ __RPC__deref_out_opt void **pptinfo,
            /* [retval][out] */ __RPC__out void *retval);
        
        /* [id][restricted][funcdescattr] */ HRESULT ( STDMETHODCALLTYPE *GetIDsOfNames )( 
            CodeDelegate2 * This,
            /* [in][idldescattr] */ __RPC__in struct GUID *riid,
            /* [in][idldescattr] */ __RPC__deref_in_opt signed char **rgszNames,
            /* [in][idldescattr] */ unsigned UINT cNames,
            /* [in][idldescattr] */ unsigned long lcid,
            /* [out][idldescattr] */ __RPC__out signed long *rgdispid,
            /* [retval][out] */ __RPC__out void *retval);
        
        /* [id][restricted][funcdescattr] */ HRESULT ( STDMETHODCALLTYPE *Invoke )( 
            CodeDelegate2 * This,
            /* [in][idldescattr] */ signed long dispidMember,
            /* [in][idldescattr] */ __RPC__in struct GUID *riid,
            /* [in][idldescattr] */ unsigned long lcid,
            /* [in][idldescattr] */ unsigned short wFlags,
            /* [in][idldescattr] */ __RPC__in struct DISPPARAMS *pdispparams,
            /* [out][idldescattr] */ __RPC__out VARIANT *pvarResult,
            /* [out][idldescattr] */ __RPC__out struct EXCEPINFO *pexcepinfo,
            /* [out][idldescattr] */ __RPC__out unsigned UINT *puArgErr,
            /* [retval][out] */ __RPC__out void *retval);
        
        /* [id][propget][funcdescattr] */ HRESULT ( STDMETHODCALLTYPE *get_DTE )( 
            CodeDelegate2 * This,
            /* [retval][out] */ __RPC__deref_out_opt **retval);
        
        /* [id][propget][funcdescattr] */ HRESULT ( STDMETHODCALLTYPE *get_Collection )( 
            CodeDelegate2 * This,
            /* [retval][out] */ __RPC__deref_out_opt CodeElements **retval);
        
        /* [id][propget][funcdescattr] */ HRESULT ( STDMETHODCALLTYPE *get_Name )( 
            CodeDelegate2 * This,
            /* [retval][out] */ __RPC__deref_out_opt BSTR *retval);
        
        /* [id][propput][funcdescattr] */ HRESULT ( STDMETHODCALLTYPE *put_Name )( 
            CodeDelegate2 * This,
            /* [idldescattr] */ __RPC__in BSTR noname,
            /* [retval][out] */ __RPC__out void *retval);
        
        /* [id][propget][funcdescattr] */ HRESULT ( STDMETHODCALLTYPE *get_FullName )( 
            CodeDelegate2 * This,
            /* [retval][out] */ __RPC__deref_out_opt BSTR *retval);
        
        /* [id][propget][funcdescattr] */ HRESULT ( STDMETHODCALLTYPE *get_ProjectItem )( 
            CodeDelegate2 * This,
            /* [retval][out] */ __RPC__deref_out_opt ProjectItem **retval);
        
        /* [id][propget][funcdescattr] */ HRESULT ( STDMETHODCALLTYPE *get_Kind )( 
            CodeDelegate2 * This,
            /* [retval][out] */ __RPC__out enum vsCMElement *retval);
        
        /* [id][propget][funcdescattr] */ HRESULT ( STDMETHODCALLTYPE *get_IsCodeType )( 
            CodeDelegate2 * This,
            /* [retval][out] */ __RPC__out BOOLEAN *retval);
        
        /* [id][propget][funcdescattr] */ HRESULT ( STDMETHODCALLTYPE *get_InfoLocation )( 
            CodeDelegate2 * This,
            /* [retval][out] */ __RPC__out enum vsCMInfoLocation *retval);
        
        /* [id][propget][funcdescattr] */ HRESULT ( STDMETHODCALLTYPE *get_Children )( 
            CodeDelegate2 * This,
            /* [retval][out] */ __RPC__deref_out_opt CodeElements **retval);
        
        /* [id][propget][funcdescattr] */ HRESULT ( STDMETHODCALLTYPE *get_Language )( 
            CodeDelegate2 * This,
            /* [retval][out] */ __RPC__deref_out_opt BSTR *retval);
        
        /* [id][propget][funcdescattr] */ HRESULT ( STDMETHODCALLTYPE *get_StartPoint )( 
            CodeDelegate2 * This,
            /* [retval][out] */ __RPC__deref_out_opt TextPoint **retval);
        
        /* [id][propget][funcdescattr] */ HRESULT ( STDMETHODCALLTYPE *get_EndPoint )( 
            CodeDelegate2 * This,
            /* [retval][out] */ __RPC__deref_out_opt TextPoint **retval);
        
        /* [id][propget][funcdescattr] */ HRESULT ( STDMETHODCALLTYPE *get_ExtenderNames )( 
            CodeDelegate2 * This,
            /* [retval][out] */ __RPC__out VARIANT *retval);
        
        /* [id][propget][funcdescattr] */ HRESULT ( STDMETHODCALLTYPE *get_Extender )( 
            CodeDelegate2 * This,
            /* [idldescattr] */ __RPC__in BSTR ExtenderName,
            /* [retval][out] */ __RPC__deref_out_opt IDispatch **retval);
        
        /* [id][propget][funcdescattr] */ HRESULT ( STDMETHODCALLTYPE *get_ExtenderCATID )( 
            CodeDelegate2 * This,
            /* [retval][out] */ __RPC__deref_out_opt BSTR *retval);
        
        /* [id][funcdescattr] */ HRESULT ( STDMETHODCALLTYPE *GetStartPoint )( 
            CodeDelegate2 * This,
            /* [in][idldescattr] */ enum vsCMPart Part,
            /* [retval][out] */ __RPC__deref_out_opt TextPoint **retval);
        
        /* [id][funcdescattr] */ HRESULT ( STDMETHODCALLTYPE *GetEndPoint )( 
            CodeDelegate2 * This,
            /* [in][idldescattr] */ enum vsCMPart Part,
            /* [retval][out] */ __RPC__deref_out_opt TextPoint **retval);
        
        /* [id][propget][funcdescattr] */ HRESULT ( STDMETHODCALLTYPE *get_Parent )( 
            CodeDelegate2 * This,
            /* [retval][out] */ __RPC__deref_out_opt IDispatch **retval);
        
        /* [id][propget][funcdescattr] */ HRESULT ( STDMETHODCALLTYPE *get_Namespace )( 
            CodeDelegate2 * This,
            /* [retval][out] */ __RPC__deref_out_opt CodeNamespace **retval);
        
        /* [id][propget][funcdescattr] */ HRESULT ( STDMETHODCALLTYPE *get_Bases )( 
            CodeDelegate2 * This,
            /* [retval][out] */ __RPC__deref_out_opt CodeElements **retval);
        
        /* [id][propget][funcdescattr] */ HRESULT ( STDMETHODCALLTYPE *get_Members )( 
            CodeDelegate2 * This,
            /* [retval][out] */ __RPC__deref_out_opt CodeElements **retval);
        
        /* [id][propput][funcdescattr] */ HRESULT ( STDMETHODCALLTYPE *put_Access )( 
            CodeDelegate2 * This,
            /* [idldescattr] */ enum vsCMAccess noname,
            /* [retval][out] */ __RPC__out void *retval);
        
        /* [id][propget][funcdescattr] */ HRESULT ( STDMETHODCALLTYPE *get_Access )( 
            CodeDelegate2 * This,
            /* [retval][out] */ __RPC__out enum vsCMAccess *retval);
        
        /* [id][propget][funcdescattr] */ HRESULT ( STDMETHODCALLTYPE *get_Attributes )( 
            CodeDelegate2 * This,
            /* [retval][out] */ __RPC__deref_out_opt CodeElements **retval);
        
        /* [id][propget][funcdescattr] */ HRESULT ( STDMETHODCALLTYPE *get_DocComment )( 
            CodeDelegate2 * This,
            /* [retval][out] */ __RPC__deref_out_opt BSTR *retval);
        
        /* [id][propput][funcdescattr] */ HRESULT ( STDMETHODCALLTYPE *put_DocComment )( 
            CodeDelegate2 * This,
            /* [idldescattr] */ __RPC__in BSTR noname,
            /* [retval][out] */ __RPC__out void *retval);
        
        /* [id][propget][funcdescattr] */ HRESULT ( STDMETHODCALLTYPE *get_Comment )( 
            CodeDelegate2 * This,
            /* [retval][out] */ __RPC__deref_out_opt BSTR *retval);
        
        /* [id][propput][funcdescattr] */ HRESULT ( STDMETHODCALLTYPE *put_Comment )( 
            CodeDelegate2 * This,
            /* [idldescattr] */ __RPC__in BSTR noname,
            /* [retval][out] */ __RPC__out void *retval);
        
        /* [id][funcdescattr] */ HRESULT ( STDMETHODCALLTYPE *AddBase )( 
            CodeDelegate2 * This,
            /* [idldescattr] */ VARIANT Base,
            /* [optional][idldescattr] */ VARIANT Position,
            /* [retval][out] */ __RPC__deref_out_opt CodeElement **retval);
        
        /* [id][funcdescattr] */ HRESULT ( STDMETHODCALLTYPE *AddAttribute )( 
            CodeDelegate2 * This,
            /* [idldescattr] */ __RPC__in BSTR Name,
            /* [idldescattr] */ __RPC__in BSTR Value,
            /* [optional][idldescattr] */ VARIANT Position,
            /* [retval][out] */ __RPC__deref_out_opt CodeAttribute **retval);
        
        /* [id][funcdescattr] */ HRESULT ( STDMETHODCALLTYPE *RemoveBase )( 
            CodeDelegate2 * This,
            /* [idldescattr] */ VARIANT Element,
            /* [retval][out] */ __RPC__out void *retval);
        
        /* [id][funcdescattr] */ HRESULT ( STDMETHODCALLTYPE *RemoveMember )( 
            CodeDelegate2 * This,
            /* [idldescattr] */ VARIANT Element,
            /* [retval][out] */ __RPC__out void *retval);
        
        /* [id][propget][funcdescattr] */ HRESULT ( STDMETHODCALLTYPE *get_IsDerivedFrom )( 
            CodeDelegate2 * This,
            /* [idldescattr] */ __RPC__in BSTR FullName,
            /* [retval][out] */ __RPC__out BOOLEAN *retval);
        
        /* [id][propget][funcdescattr] */ HRESULT ( STDMETHODCALLTYPE *get_DerivedTypes )( 
            CodeDelegate2 * This,
            /* [retval][out] */ __RPC__deref_out_opt CodeElements **retval);
        
        /* [id][propget][funcdescattr] */ HRESULT ( STDMETHODCALLTYPE *get_BaseClass )( 
            CodeDelegate2 * This,
            /* [retval][out] */ __RPC__deref_out_opt CodeClass **retval);
        
        /* [id][propget][funcdescattr] */ HRESULT ( STDMETHODCALLTYPE *get_Prototype )( 
            CodeDelegate2 * This,
            /* [idldescattr] */ signed long Flags,
            /* [retval][out] */ __RPC__deref_out_opt BSTR *retval);
        
        /* [id][propget][funcdescattr] */ HRESULT ( STDMETHODCALLTYPE *get_Type )( 
            CodeDelegate2 * This,
            /* [retval][out] */ __RPC__deref_out_opt CodeTypeRef **retval);
        
        /* [id][propput][funcdescattr] */ HRESULT ( STDMETHODCALLTYPE *put_Type )( 
            CodeDelegate2 * This,
            /* [idldescattr] */ __RPC__in_opt CodeTypeRef *noname,
            /* [retval][out] */ __RPC__out void *retval);
        
        /* [id][propget][funcdescattr] */ HRESULT ( STDMETHODCALLTYPE *get_Parameters )( 
            CodeDelegate2 * This,
            /* [retval][out] */ __RPC__deref_out_opt CodeElements **retval);
        
        /* [id][funcdescattr] */ HRESULT ( STDMETHODCALLTYPE *AddParameter )( 
            CodeDelegate2 * This,
            /* [idldescattr] */ __RPC__in BSTR Name,
            /* [idldescattr] */ VARIANT Type,
            /* [optional][idldescattr] */ VARIANT Position,
            /* [retval][out] */ __RPC__deref_out_opt CodeParameter **retval);
        
        /* [id][funcdescattr] */ HRESULT ( STDMETHODCALLTYPE *RemoveParameter )( 
            CodeDelegate2 * This,
            /* [idldescattr] */ VARIANT Element,
            /* [retval][out] */ __RPC__out void *retval);
        
        /* [helpstring][propget][id] */ HRESULT ( STDMETHODCALLTYPE *get_IsGeneric )( 
            CodeDelegate2 * This,
            /* [retval][out] */ __RPC__out VARIANT_BOOL *pIsGeneric);
        
        END_INTERFACE
    } CodeDelegate2Vtbl;

    interface CodeDelegate2
    {
        CONST_VTBL struct CodeDelegate2Vtbl *lpVtbl;
    };

    

#ifdef COBJMACROS


#define CodeDelegate2_QueryInterface(This,riid,ppvObj,retval)	\
    ( (This)->lpVtbl -> QueryInterface(This,riid,ppvObj,retval) ) 

#define CodeDelegate2_AddRef(This,retval)	\
    ( (This)->lpVtbl -> AddRef(This,retval) ) 

#define CodeDelegate2_Release(This,retval)	\
    ( (This)->lpVtbl -> Release(This,retval) ) 

#define CodeDelegate2_GetTypeInfoCount(This,pctinfo,retval)	\
    ( (This)->lpVtbl -> GetTypeInfoCount(This,pctinfo,retval) ) 

#define CodeDelegate2_GetTypeInfo(This,itinfo,lcid,pptinfo,retval)	\
    ( (This)->lpVtbl -> GetTypeInfo(This,itinfo,lcid,pptinfo,retval) ) 

#define CodeDelegate2_GetIDsOfNames(This,riid,rgszNames,cNames,lcid,rgdispid,retval)	\
    ( (This)->lpVtbl -> GetIDsOfNames(This,riid,rgszNames,cNames,lcid,rgdispid,retval) ) 

#define CodeDelegate2_Invoke(This,dispidMember,riid,lcid,wFlags,pdispparams,pvarResult,pexcepinfo,puArgErr,retval)	\
    ( (This)->lpVtbl -> Invoke(This,dispidMember,riid,lcid,wFlags,pdispparams,pvarResult,pexcepinfo,puArgErr,retval) ) 

#define CodeDelegate2_get_DTE(This,retval)	\
    ( (This)->lpVtbl -> get_DTE(This,retval) ) 

#define CodeDelegate2_get_Collection(This,retval)	\
    ( (This)->lpVtbl -> get_Collection(This,retval) ) 

#define CodeDelegate2_get_Name(This,retval)	\
    ( (This)->lpVtbl -> get_Name(This,retval) ) 

#define CodeDelegate2_put_Name(This,noname,retval)	\
    ( (This)->lpVtbl -> put_Name(This,noname,retval) ) 

#define CodeDelegate2_get_FullName(This,retval)	\
    ( (This)->lpVtbl -> get_FullName(This,retval) ) 

#define CodeDelegate2_get_ProjectItem(This,retval)	\
    ( (This)->lpVtbl -> get_ProjectItem(This,retval) ) 

#define CodeDelegate2_get_Kind(This,retval)	\
    ( (This)->lpVtbl -> get_Kind(This,retval) ) 

#define CodeDelegate2_get_IsCodeType(This,retval)	\
    ( (This)->lpVtbl -> get_IsCodeType(This,retval) ) 

#define CodeDelegate2_get_InfoLocation(This,retval)	\
    ( (This)->lpVtbl -> get_InfoLocation(This,retval) ) 

#define CodeDelegate2_get_Children(This,retval)	\
    ( (This)->lpVtbl -> get_Children(This,retval) ) 

#define CodeDelegate2_get_Language(This,retval)	\
    ( (This)->lpVtbl -> get_Language(This,retval) ) 

#define CodeDelegate2_get_StartPoint(This,retval)	\
    ( (This)->lpVtbl -> get_StartPoint(This,retval) ) 

#define CodeDelegate2_get_EndPoint(This,retval)	\
    ( (This)->lpVtbl -> get_EndPoint(This,retval) ) 

#define CodeDelegate2_get_ExtenderNames(This,retval)	\
    ( (This)->lpVtbl -> get_ExtenderNames(This,retval) ) 

#define CodeDelegate2_get_Extender(This,ExtenderName,retval)	\
    ( (This)->lpVtbl -> get_Extender(This,ExtenderName,retval) ) 

#define CodeDelegate2_get_ExtenderCATID(This,retval)	\
    ( (This)->lpVtbl -> get_ExtenderCATID(This,retval) ) 

#define CodeDelegate2_GetStartPoint(This,Part,retval)	\
    ( (This)->lpVtbl -> GetStartPoint(This,Part,retval) ) 

#define CodeDelegate2_GetEndPoint(This,Part,retval)	\
    ( (This)->lpVtbl -> GetEndPoint(This,Part,retval) ) 

#define CodeDelegate2_get_Parent(This,retval)	\
    ( (This)->lpVtbl -> get_Parent(This,retval) ) 

#define CodeDelegate2_get_Namespace(This,retval)	\
    ( (This)->lpVtbl -> get_Namespace(This,retval) ) 

#define CodeDelegate2_get_Bases(This,retval)	\
    ( (This)->lpVtbl -> get_Bases(This,retval) ) 

#define CodeDelegate2_get_Members(This,retval)	\
    ( (This)->lpVtbl -> get_Members(This,retval) ) 

#define CodeDelegate2_put_Access(This,noname,retval)	\
    ( (This)->lpVtbl -> put_Access(This,noname,retval) ) 

#define CodeDelegate2_get_Access(This,retval)	\
    ( (This)->lpVtbl -> get_Access(This,retval) ) 

#define CodeDelegate2_get_Attributes(This,retval)	\
    ( (This)->lpVtbl -> get_Attributes(This,retval) ) 

#define CodeDelegate2_get_DocComment(This,retval)	\
    ( (This)->lpVtbl -> get_DocComment(This,retval) ) 

#define CodeDelegate2_put_DocComment(This,noname,retval)	\
    ( (This)->lpVtbl -> put_DocComment(This,noname,retval) ) 

#define CodeDelegate2_get_Comment(This,retval)	\
    ( (This)->lpVtbl -> get_Comment(This,retval) ) 

#define CodeDelegate2_put_Comment(This,noname,retval)	\
    ( (This)->lpVtbl -> put_Comment(This,noname,retval) ) 

#define CodeDelegate2_AddBase(This,Base,Position,retval)	\
    ( (This)->lpVtbl -> AddBase(This,Base,Position,retval) ) 

#define CodeDelegate2_AddAttribute(This,Name,Value,Position,retval)	\
    ( (This)->lpVtbl -> AddAttribute(This,Name,Value,Position,retval) ) 

#define CodeDelegate2_RemoveBase(This,Element,retval)	\
    ( (This)->lpVtbl -> RemoveBase(This,Element,retval) ) 

#define CodeDelegate2_RemoveMember(This,Element,retval)	\
    ( (This)->lpVtbl -> RemoveMember(This,Element,retval) ) 

#define CodeDelegate2_get_IsDerivedFrom(This,FullName,retval)	\
    ( (This)->lpVtbl -> get_IsDerivedFrom(This,FullName,retval) ) 

#define CodeDelegate2_get_DerivedTypes(This,retval)	\
    ( (This)->lpVtbl -> get_DerivedTypes(This,retval) ) 

#define CodeDelegate2_get_BaseClass(This,retval)	\
    ( (This)->lpVtbl -> get_BaseClass(This,retval) ) 

#define CodeDelegate2_get_Prototype(This,Flags,retval)	\
    ( (This)->lpVtbl -> get_Prototype(This,Flags,retval) ) 

#define CodeDelegate2_get_Type(This,retval)	\
    ( (This)->lpVtbl -> get_Type(This,retval) ) 

#define CodeDelegate2_put_Type(This,noname,retval)	\
    ( (This)->lpVtbl -> put_Type(This,noname,retval) ) 

#define CodeDelegate2_get_Parameters(This,retval)	\
    ( (This)->lpVtbl -> get_Parameters(This,retval) ) 

#define CodeDelegate2_AddParameter(This,Name,Type,Position,retval)	\
    ( (This)->lpVtbl -> AddParameter(This,Name,Type,Position,retval) ) 

#define CodeDelegate2_RemoveParameter(This,Element,retval)	\
    ( (This)->lpVtbl -> RemoveParameter(This,Element,retval) ) 


#define CodeDelegate2_get_IsGeneric(This,pIsGeneric)	\
    ( (This)->lpVtbl -> get_IsGeneric(This,pIsGeneric) ) 

#endif /* COBJMACROS */


#endif 	/* C style interface */




#endif 	/* __CodeDelegate2_INTERFACE_DEFINED__ */


#ifndef __CodeStruct2_INTERFACE_DEFINED__
#define __CodeStruct2_INTERFACE_DEFINED__

/* interface CodeStruct2 */
/* [helpstringcontext][helpstring][helpcontext][uuid][object][oleautomation][dual] */ 


EXTERN_C const IID IID_CodeStruct2;

#if defined(__cplusplus) && !defined(CINTERFACE)
    
    MIDL_INTERFACE("f6dc1a01-e65a-404b-8b6d-9cdd603db4ed")
    CodeStruct2 : public CodeStruct
    {
    public:
        virtual /* [helpstring][propget][id] */ HRESULT STDMETHODCALLTYPE get_IsGeneric( 
            /* [retval][out] */ __RPC__out VARIANT_BOOL *pIsGeneric) = 0;
        
        virtual /* [helpstring][id] */ HRESULT STDMETHODCALLTYPE AddEvent( 
            __RPC__in BSTR Name,
            __RPC__in BSTR FullDelegateName,
            /* [defaultvalue][in] */ VARIANT_BOOL CreatePropertyStyleEvent,
            /* [optional] */ VARIANT Position,
            /* [defaultvalue] */ enum /* external definition not present */ vsCMAccess Access,
            /* [retval][out] */ __RPC__deref_out_opt CodeEvent **ppEvent) = 0;
        
        virtual /* [helpstringcontext][helpstring][helpcontext][propget][id] */ HRESULT STDMETHODCALLTYPE get_DataTypeKind( 
            /* [retval][out] */ __RPC__out vsCMDataTypeKind *Kind) = 0;
        
        virtual /* [helpstringcontext][helpstring][helpcontext][propput][id] */ HRESULT STDMETHODCALLTYPE put_DataTypeKind( 
            vsCMDataTypeKind Kind) = 0;
        
        virtual /* [helpstringcontext][helpstring][helpcontext][propget][id] */ HRESULT STDMETHODCALLTYPE get_Parts( 
            /* [retval][out] */ __RPC__deref_out_opt /* external definition not present */ CodeElements **Elements) = 0;
        
    };
    
#else 	/* C style interface */

    typedef struct CodeStruct2Vtbl
    {
        BEGIN_INTERFACE
        
        /* [id][restricted][funcdescattr] */ HRESULT ( STDMETHODCALLTYPE *QueryInterface )( 
            CodeStruct2 * This,
            /* [in][idldescattr] */ __RPC__in struct GUID *riid,
            /* [out][idldescattr] */ __RPC__deref_out_opt void **ppvObj,
            /* [retval][out] */ __RPC__out void *retval);
        
        /* [id][restricted][funcdescattr] */ HRESULT ( STDMETHODCALLTYPE *AddRef )( 
            CodeStruct2 * This,
            /* [retval][out] */ __RPC__out unsigned long *retval);
        
        /* [id][restricted][funcdescattr] */ HRESULT ( STDMETHODCALLTYPE *Release )( 
            CodeStruct2 * This,
            /* [retval][out] */ __RPC__out unsigned long *retval);
        
        /* [id][restricted][funcdescattr] */ HRESULT ( STDMETHODCALLTYPE *GetTypeInfoCount )( 
            CodeStruct2 * This,
            /* [out][idldescattr] */ __RPC__out unsigned UINT *pctinfo,
            /* [retval][out] */ __RPC__out void *retval);
        
        /* [id][restricted][funcdescattr] */ HRESULT ( STDMETHODCALLTYPE *GetTypeInfo )( 
            CodeStruct2 * This,
            /* [in][idldescattr] */ unsigned UINT itinfo,
            /* [in][idldescattr] */ unsigned long lcid,
            /* [out][idldescattr] */ __RPC__deref_out_opt void **pptinfo,
            /* [retval][out] */ __RPC__out void *retval);
        
        /* [id][restricted][funcdescattr] */ HRESULT ( STDMETHODCALLTYPE *GetIDsOfNames )( 
            CodeStruct2 * This,
            /* [in][idldescattr] */ __RPC__in struct GUID *riid,
            /* [in][idldescattr] */ __RPC__deref_in_opt signed char **rgszNames,
            /* [in][idldescattr] */ unsigned UINT cNames,
            /* [in][idldescattr] */ unsigned long lcid,
            /* [out][idldescattr] */ __RPC__out signed long *rgdispid,
            /* [retval][out] */ __RPC__out void *retval);
        
        /* [id][restricted][funcdescattr] */ HRESULT ( STDMETHODCALLTYPE *Invoke )( 
            CodeStruct2 * This,
            /* [in][idldescattr] */ signed long dispidMember,
            /* [in][idldescattr] */ __RPC__in struct GUID *riid,
            /* [in][idldescattr] */ unsigned long lcid,
            /* [in][idldescattr] */ unsigned short wFlags,
            /* [in][idldescattr] */ __RPC__in struct DISPPARAMS *pdispparams,
            /* [out][idldescattr] */ __RPC__out VARIANT *pvarResult,
            /* [out][idldescattr] */ __RPC__out struct EXCEPINFO *pexcepinfo,
            /* [out][idldescattr] */ __RPC__out unsigned UINT *puArgErr,
            /* [retval][out] */ __RPC__out void *retval);
        
        /* [id][propget][funcdescattr] */ HRESULT ( STDMETHODCALLTYPE *get_DTE )( 
            CodeStruct2 * This,
            /* [retval][out] */ __RPC__deref_out_opt **retval);
        
        /* [id][propget][funcdescattr] */ HRESULT ( STDMETHODCALLTYPE *get_Collection )( 
            CodeStruct2 * This,
            /* [retval][out] */ __RPC__deref_out_opt CodeElements **retval);
        
        /* [id][propget][funcdescattr] */ HRESULT ( STDMETHODCALLTYPE *get_Name )( 
            CodeStruct2 * This,
            /* [retval][out] */ __RPC__deref_out_opt BSTR *retval);
        
        /* [id][propput][funcdescattr] */ HRESULT ( STDMETHODCALLTYPE *put_Name )( 
            CodeStruct2 * This,
            /* [idldescattr] */ __RPC__in BSTR noname,
            /* [retval][out] */ __RPC__out void *retval);
        
        /* [id][propget][funcdescattr] */ HRESULT ( STDMETHODCALLTYPE *get_FullName )( 
            CodeStruct2 * This,
            /* [retval][out] */ __RPC__deref_out_opt BSTR *retval);
        
        /* [id][propget][funcdescattr] */ HRESULT ( STDMETHODCALLTYPE *get_ProjectItem )( 
            CodeStruct2 * This,
            /* [retval][out] */ __RPC__deref_out_opt ProjectItem **retval);
        
        /* [id][propget][funcdescattr] */ HRESULT ( STDMETHODCALLTYPE *get_Kind )( 
            CodeStruct2 * This,
            /* [retval][out] */ __RPC__out enum vsCMElement *retval);
        
        /* [id][propget][funcdescattr] */ HRESULT ( STDMETHODCALLTYPE *get_IsCodeType )( 
            CodeStruct2 * This,
            /* [retval][out] */ __RPC__out BOOLEAN *retval);
        
        /* [id][propget][funcdescattr] */ HRESULT ( STDMETHODCALLTYPE *get_InfoLocation )( 
            CodeStruct2 * This,
            /* [retval][out] */ __RPC__out enum vsCMInfoLocation *retval);
        
        /* [id][propget][funcdescattr] */ HRESULT ( STDMETHODCALLTYPE *get_Children )( 
            CodeStruct2 * This,
            /* [retval][out] */ __RPC__deref_out_opt CodeElements **retval);
        
        /* [id][propget][funcdescattr] */ HRESULT ( STDMETHODCALLTYPE *get_Language )( 
            CodeStruct2 * This,
            /* [retval][out] */ __RPC__deref_out_opt BSTR *retval);
        
        /* [id][propget][funcdescattr] */ HRESULT ( STDMETHODCALLTYPE *get_StartPoint )( 
            CodeStruct2 * This,
            /* [retval][out] */ __RPC__deref_out_opt TextPoint **retval);
        
        /* [id][propget][funcdescattr] */ HRESULT ( STDMETHODCALLTYPE *get_EndPoint )( 
            CodeStruct2 * This,
            /* [retval][out] */ __RPC__deref_out_opt TextPoint **retval);
        
        /* [id][propget][funcdescattr] */ HRESULT ( STDMETHODCALLTYPE *get_ExtenderNames )( 
            CodeStruct2 * This,
            /* [retval][out] */ __RPC__out VARIANT *retval);
        
        /* [id][propget][funcdescattr] */ HRESULT ( STDMETHODCALLTYPE *get_Extender )( 
            CodeStruct2 * This,
            /* [idldescattr] */ __RPC__in BSTR ExtenderName,
            /* [retval][out] */ __RPC__deref_out_opt IDispatch **retval);
        
        /* [id][propget][funcdescattr] */ HRESULT ( STDMETHODCALLTYPE *get_ExtenderCATID )( 
            CodeStruct2 * This,
            /* [retval][out] */ __RPC__deref_out_opt BSTR *retval);
        
        /* [id][funcdescattr] */ HRESULT ( STDMETHODCALLTYPE *GetStartPoint )( 
            CodeStruct2 * This,
            /* [in][idldescattr] */ enum vsCMPart Part,
            /* [retval][out] */ __RPC__deref_out_opt TextPoint **retval);
        
        /* [id][funcdescattr] */ HRESULT ( STDMETHODCALLTYPE *GetEndPoint )( 
            CodeStruct2 * This,
            /* [in][idldescattr] */ enum vsCMPart Part,
            /* [retval][out] */ __RPC__deref_out_opt TextPoint **retval);
        
        /* [id][propget][funcdescattr] */ HRESULT ( STDMETHODCALLTYPE *get_Parent )( 
            CodeStruct2 * This,
            /* [retval][out] */ __RPC__deref_out_opt IDispatch **retval);
        
        /* [id][propget][funcdescattr] */ HRESULT ( STDMETHODCALLTYPE *get_Namespace )( 
            CodeStruct2 * This,
            /* [retval][out] */ __RPC__deref_out_opt CodeNamespace **retval);
        
        /* [id][propget][funcdescattr] */ HRESULT ( STDMETHODCALLTYPE *get_Bases )( 
            CodeStruct2 * This,
            /* [retval][out] */ __RPC__deref_out_opt CodeElements **retval);
        
        /* [id][propget][funcdescattr] */ HRESULT ( STDMETHODCALLTYPE *get_Members )( 
            CodeStruct2 * This,
            /* [retval][out] */ __RPC__deref_out_opt CodeElements **retval);
        
        /* [id][propput][funcdescattr] */ HRESULT ( STDMETHODCALLTYPE *put_Access )( 
            CodeStruct2 * This,
            /* [idldescattr] */ enum vsCMAccess noname,
            /* [retval][out] */ __RPC__out void *retval);
        
        /* [id][propget][funcdescattr] */ HRESULT ( STDMETHODCALLTYPE *get_Access )( 
            CodeStruct2 * This,
            /* [retval][out] */ __RPC__out enum vsCMAccess *retval);
        
        /* [id][propget][funcdescattr] */ HRESULT ( STDMETHODCALLTYPE *get_Attributes )( 
            CodeStruct2 * This,
            /* [retval][out] */ __RPC__deref_out_opt CodeElements **retval);
        
        /* [id][propget][funcdescattr] */ HRESULT ( STDMETHODCALLTYPE *get_DocComment )( 
            CodeStruct2 * This,
            /* [retval][out] */ __RPC__deref_out_opt BSTR *retval);
        
        /* [id][propput][funcdescattr] */ HRESULT ( STDMETHODCALLTYPE *put_DocComment )( 
            CodeStruct2 * This,
            /* [idldescattr] */ __RPC__in BSTR noname,
            /* [retval][out] */ __RPC__out void *retval);
        
        /* [id][propget][funcdescattr] */ HRESULT ( STDMETHODCALLTYPE *get_Comment )( 
            CodeStruct2 * This,
            /* [retval][out] */ __RPC__deref_out_opt BSTR *retval);
        
        /* [id][propput][funcdescattr] */ HRESULT ( STDMETHODCALLTYPE *put_Comment )( 
            CodeStruct2 * This,
            /* [idldescattr] */ __RPC__in BSTR noname,
            /* [retval][out] */ __RPC__out void *retval);
        
        /* [id][funcdescattr] */ HRESULT ( STDMETHODCALLTYPE *AddBase )( 
            CodeStruct2 * This,
            /* [idldescattr] */ VARIANT Base,
            /* [optional][idldescattr] */ VARIANT Position,
            /* [retval][out] */ __RPC__deref_out_opt CodeElement **retval);
        
        /* [id][funcdescattr] */ HRESULT ( STDMETHODCALLTYPE *AddAttribute )( 
            CodeStruct2 * This,
            /* [idldescattr] */ __RPC__in BSTR Name,
            /* [idldescattr] */ __RPC__in BSTR Value,
            /* [optional][idldescattr] */ VARIANT Position,
            /* [retval][out] */ __RPC__deref_out_opt CodeAttribute **retval);
        
        /* [id][funcdescattr] */ HRESULT ( STDMETHODCALLTYPE *RemoveBase )( 
            CodeStruct2 * This,
            /* [idldescattr] */ VARIANT Element,
            /* [retval][out] */ __RPC__out void *retval);
        
        /* [id][funcdescattr] */ HRESULT ( STDMETHODCALLTYPE *RemoveMember )( 
            CodeStruct2 * This,
            /* [idldescattr] */ VARIANT Element,
            /* [retval][out] */ __RPC__out void *retval);
        
        /* [id][propget][funcdescattr] */ HRESULT ( STDMETHODCALLTYPE *get_IsDerivedFrom )( 
            CodeStruct2 * This,
            /* [idldescattr] */ __RPC__in BSTR FullName,
            /* [retval][out] */ __RPC__out BOOLEAN *retval);
        
        /* [id][propget][funcdescattr] */ HRESULT ( STDMETHODCALLTYPE *get_DerivedTypes )( 
            CodeStruct2 * This,
            /* [retval][out] */ __RPC__deref_out_opt CodeElements **retval);
        
        /* [id][propget][funcdescattr] */ HRESULT ( STDMETHODCALLTYPE *get_ImplementedInterfaces )( 
            CodeStruct2 * This,
            /* [retval][out] */ __RPC__deref_out_opt CodeElements **retval);
        
        /* [id][propget][funcdescattr] */ HRESULT ( STDMETHODCALLTYPE *get_IsAbstract )( 
            CodeStruct2 * This,
            /* [retval][out] */ __RPC__out BOOLEAN *retval);
        
        /* [id][propput][funcdescattr] */ HRESULT ( STDMETHODCALLTYPE *put_IsAbstract )( 
            CodeStruct2 * This,
            /* [idldescattr] */ BOOLEAN noname,
            /* [retval][out] */ __RPC__out void *retval);
        
        /* [id][funcdescattr] */ HRESULT ( STDMETHODCALLTYPE *AddImplementedInterface )( 
            CodeStruct2 * This,
            /* [idldescattr] */ VARIANT Base,
            /* [optional][idldescattr] */ VARIANT Position,
            /* [retval][out] */ __RPC__deref_out_opt CodeInterface **retval);
        
        /* [id][funcdescattr] */ HRESULT ( STDMETHODCALLTYPE *AddFunction )( 
            CodeStruct2 * This,
            /* [idldescattr] */ __RPC__in BSTR Name,
            /* [idldescattr] */ enum vsCMFunction Kind,
            /* [idldescattr] */ VARIANT Type,
            /* [idldescattr] */ VARIANT Position,
            /* [idldescattr] */ enum vsCMAccess Access,
            /* [optional][idldescattr] */ VARIANT Location,
            /* [retval][out] */ __RPC__deref_out_opt CodeFunction **retval);
        
        /* [id][funcdescattr] */ HRESULT ( STDMETHODCALLTYPE *AddVariable )( 
            CodeStruct2 * This,
            /* [idldescattr] */ __RPC__in BSTR Name,
            /* [idldescattr] */ VARIANT Type,
            /* [idldescattr] */ VARIANT Position,
            /* [idldescattr] */ enum vsCMAccess Access,
            /* [optional][idldescattr] */ VARIANT Location,
            /* [retval][out] */ __RPC__deref_out_opt CodeVariable **retval);
        
        /* [id][funcdescattr] */ HRESULT ( STDMETHODCALLTYPE *AddProperty )( 
            CodeStruct2 * This,
            /* [idldescattr] */ __RPC__in BSTR GetterName,
            /* [idldescattr] */ __RPC__in BSTR PutterName,
            /* [idldescattr] */ VARIANT Type,
            /* [idldescattr] */ VARIANT Position,
            /* [idldescattr] */ enum vsCMAccess Access,
            /* [optional][idldescattr] */ VARIANT Location,
            /* [retval][out] */ __RPC__deref_out_opt CodeProperty **retval);
        
        /* [id][funcdescattr] */ HRESULT ( STDMETHODCALLTYPE *AddClass )( 
            CodeStruct2 * This,
            /* [idldescattr] */ __RPC__in BSTR Name,
            /* [idldescattr] */ VARIANT Position,
            /* [idldescattr] */ VARIANT Bases,
            /* [idldescattr] */ VARIANT ImplementedInterfaces,
            /* [idldescattr] */ enum vsCMAccess Access,
            /* [retval][out] */ __RPC__deref_out_opt CodeClass **retval);
        
        /* [id][funcdescattr] */ HRESULT ( STDMETHODCALLTYPE *AddStruct )( 
            CodeStruct2 * This,
            /* [idldescattr] */ __RPC__in BSTR Name,
            /* [idldescattr] */ VARIANT Position,
            /* [idldescattr] */ VARIANT Bases,
            /* [idldescattr] */ VARIANT ImplementedInterfaces,
            /* [idldescattr] */ enum vsCMAccess Access,
            /* [retval][out] */ __RPC__deref_out_opt CodeStruct **retval);
        
        /* [id][funcdescattr] */ HRESULT ( STDMETHODCALLTYPE *AddEnum )( 
            CodeStruct2 * This,
            /* [idldescattr] */ __RPC__in BSTR Name,
            /* [idldescattr] */ VARIANT Position,
            /* [idldescattr] */ VARIANT Bases,
            /* [idldescattr] */ enum vsCMAccess Access,
            /* [retval][out] */ __RPC__deref_out_opt CodeEnum **retval);
        
        /* [id][funcdescattr] */ HRESULT ( STDMETHODCALLTYPE *AddDelegate )( 
            CodeStruct2 * This,
            /* [idldescattr] */ __RPC__in BSTR Name,
            /* [idldescattr] */ VARIANT Type,
            /* [idldescattr] */ VARIANT Position,
            /* [idldescattr] */ enum vsCMAccess Access,
            /* [retval][out] */ __RPC__deref_out_opt CodeDelegate **retval);
        
        /* [id][funcdescattr] */ HRESULT ( STDMETHODCALLTYPE *RemoveInterface )( 
            CodeStruct2 * This,
            /* [idldescattr] */ VARIANT Element,
            /* [retval][out] */ __RPC__out void *retval);
        
        /* [helpstring][propget][id] */ HRESULT ( STDMETHODCALLTYPE *get_IsGeneric )( 
            CodeStruct2 * This,
            /* [retval][out] */ __RPC__out VARIANT_BOOL *pIsGeneric);
        
        /* [helpstring][id] */ HRESULT ( STDMETHODCALLTYPE *AddEvent )( 
            CodeStruct2 * This,
            __RPC__in BSTR Name,
            __RPC__in BSTR FullDelegateName,
            /* [defaultvalue][in] */ VARIANT_BOOL CreatePropertyStyleEvent,
            /* [optional] */ VARIANT Position,
            /* [defaultvalue] */ enum /* external definition not present */ vsCMAccess Access,
            /* [retval][out] */ __RPC__deref_out_opt CodeEvent **ppEvent);
        
        /* [helpstringcontext][helpstring][helpcontext][propget][id] */ HRESULT ( STDMETHODCALLTYPE *get_DataTypeKind )( 
            CodeStruct2 * This,
            /* [retval][out] */ __RPC__out vsCMDataTypeKind *Kind);
        
        /* [helpstringcontext][helpstring][helpcontext][propput][id] */ HRESULT ( STDMETHODCALLTYPE *put_DataTypeKind )( 
            CodeStruct2 * This,
            vsCMDataTypeKind Kind);
        
        /* [helpstringcontext][helpstring][helpcontext][propget][id] */ HRESULT ( STDMETHODCALLTYPE *get_Parts )( 
            CodeStruct2 * This,
            /* [retval][out] */ __RPC__deref_out_opt /* external definition not present */ CodeElements **Elements);
        
        END_INTERFACE
    } CodeStruct2Vtbl;

    interface CodeStruct2
    {
        CONST_VTBL struct CodeStruct2Vtbl *lpVtbl;
    };

    

#ifdef COBJMACROS


#define CodeStruct2_QueryInterface(This,riid,ppvObj,retval)	\
    ( (This)->lpVtbl -> QueryInterface(This,riid,ppvObj,retval) ) 

#define CodeStruct2_AddRef(This,retval)	\
    ( (This)->lpVtbl -> AddRef(This,retval) ) 

#define CodeStruct2_Release(This,retval)	\
    ( (This)->lpVtbl -> Release(This,retval) ) 

#define CodeStruct2_GetTypeInfoCount(This,pctinfo,retval)	\
    ( (This)->lpVtbl -> GetTypeInfoCount(This,pctinfo,retval) ) 

#define CodeStruct2_GetTypeInfo(This,itinfo,lcid,pptinfo,retval)	\
    ( (This)->lpVtbl -> GetTypeInfo(This,itinfo,lcid,pptinfo,retval) ) 

#define CodeStruct2_GetIDsOfNames(This,riid,rgszNames,cNames,lcid,rgdispid,retval)	\
    ( (This)->lpVtbl -> GetIDsOfNames(This,riid,rgszNames,cNames,lcid,rgdispid,retval) ) 

#define CodeStruct2_Invoke(This,dispidMember,riid,lcid,wFlags,pdispparams,pvarResult,pexcepinfo,puArgErr,retval)	\
    ( (This)->lpVtbl -> Invoke(This,dispidMember,riid,lcid,wFlags,pdispparams,pvarResult,pexcepinfo,puArgErr,retval) ) 

#define CodeStruct2_get_DTE(This,retval)	\
    ( (This)->lpVtbl -> get_DTE(This,retval) ) 

#define CodeStruct2_get_Collection(This,retval)	\
    ( (This)->lpVtbl -> get_Collection(This,retval) ) 

#define CodeStruct2_get_Name(This,retval)	\
    ( (This)->lpVtbl -> get_Name(This,retval) ) 

#define CodeStruct2_put_Name(This,noname,retval)	\
    ( (This)->lpVtbl -> put_Name(This,noname,retval) ) 

#define CodeStruct2_get_FullName(This,retval)	\
    ( (This)->lpVtbl -> get_FullName(This,retval) ) 

#define CodeStruct2_get_ProjectItem(This,retval)	\
    ( (This)->lpVtbl -> get_ProjectItem(This,retval) ) 

#define CodeStruct2_get_Kind(This,retval)	\
    ( (This)->lpVtbl -> get_Kind(This,retval) ) 

#define CodeStruct2_get_IsCodeType(This,retval)	\
    ( (This)->lpVtbl -> get_IsCodeType(This,retval) ) 

#define CodeStruct2_get_InfoLocation(This,retval)	\
    ( (This)->lpVtbl -> get_InfoLocation(This,retval) ) 

#define CodeStruct2_get_Children(This,retval)	\
    ( (This)->lpVtbl -> get_Children(This,retval) ) 

#define CodeStruct2_get_Language(This,retval)	\
    ( (This)->lpVtbl -> get_Language(This,retval) ) 

#define CodeStruct2_get_StartPoint(This,retval)	\
    ( (This)->lpVtbl -> get_StartPoint(This,retval) ) 

#define CodeStruct2_get_EndPoint(This,retval)	\
    ( (This)->lpVtbl -> get_EndPoint(This,retval) ) 

#define CodeStruct2_get_ExtenderNames(This,retval)	\
    ( (This)->lpVtbl -> get_ExtenderNames(This,retval) ) 

#define CodeStruct2_get_Extender(This,ExtenderName,retval)	\
    ( (This)->lpVtbl -> get_Extender(This,ExtenderName,retval) ) 

#define CodeStruct2_get_ExtenderCATID(This,retval)	\
    ( (This)->lpVtbl -> get_ExtenderCATID(This,retval) ) 

#define CodeStruct2_GetStartPoint(This,Part,retval)	\
    ( (This)->lpVtbl -> GetStartPoint(This,Part,retval) ) 

#define CodeStruct2_GetEndPoint(This,Part,retval)	\
    ( (This)->lpVtbl -> GetEndPoint(This,Part,retval) ) 

#define CodeStruct2_get_Parent(This,retval)	\
    ( (This)->lpVtbl -> get_Parent(This,retval) ) 

#define CodeStruct2_get_Namespace(This,retval)	\
    ( (This)->lpVtbl -> get_Namespace(This,retval) ) 

#define CodeStruct2_get_Bases(This,retval)	\
    ( (This)->lpVtbl -> get_Bases(This,retval) ) 

#define CodeStruct2_get_Members(This,retval)	\
    ( (This)->lpVtbl -> get_Members(This,retval) ) 

#define CodeStruct2_put_Access(This,noname,retval)	\
    ( (This)->lpVtbl -> put_Access(This,noname,retval) ) 

#define CodeStruct2_get_Access(This,retval)	\
    ( (This)->lpVtbl -> get_Access(This,retval) ) 

#define CodeStruct2_get_Attributes(This,retval)	\
    ( (This)->lpVtbl -> get_Attributes(This,retval) ) 

#define CodeStruct2_get_DocComment(This,retval)	\
    ( (This)->lpVtbl -> get_DocComment(This,retval) ) 

#define CodeStruct2_put_DocComment(This,noname,retval)	\
    ( (This)->lpVtbl -> put_DocComment(This,noname,retval) ) 

#define CodeStruct2_get_Comment(This,retval)	\
    ( (This)->lpVtbl -> get_Comment(This,retval) ) 

#define CodeStruct2_put_Comment(This,noname,retval)	\
    ( (This)->lpVtbl -> put_Comment(This,noname,retval) ) 

#define CodeStruct2_AddBase(This,Base,Position,retval)	\
    ( (This)->lpVtbl -> AddBase(This,Base,Position,retval) ) 

#define CodeStruct2_AddAttribute(This,Name,Value,Position,retval)	\
    ( (This)->lpVtbl -> AddAttribute(This,Name,Value,Position,retval) ) 

#define CodeStruct2_RemoveBase(This,Element,retval)	\
    ( (This)->lpVtbl -> RemoveBase(This,Element,retval) ) 

#define CodeStruct2_RemoveMember(This,Element,retval)	\
    ( (This)->lpVtbl -> RemoveMember(This,Element,retval) ) 

#define CodeStruct2_get_IsDerivedFrom(This,FullName,retval)	\
    ( (This)->lpVtbl -> get_IsDerivedFrom(This,FullName,retval) ) 

#define CodeStruct2_get_DerivedTypes(This,retval)	\
    ( (This)->lpVtbl -> get_DerivedTypes(This,retval) ) 

#define CodeStruct2_get_ImplementedInterfaces(This,retval)	\
    ( (This)->lpVtbl -> get_ImplementedInterfaces(This,retval) ) 

#define CodeStruct2_get_IsAbstract(This,retval)	\
    ( (This)->lpVtbl -> get_IsAbstract(This,retval) ) 

#define CodeStruct2_put_IsAbstract(This,noname,retval)	\
    ( (This)->lpVtbl -> put_IsAbstract(This,noname,retval) ) 

#define CodeStruct2_AddImplementedInterface(This,Base,Position,retval)	\
    ( (This)->lpVtbl -> AddImplementedInterface(This,Base,Position,retval) ) 

#define CodeStruct2_AddFunction(This,Name,Kind,Type,Position,Access,Location,retval)	\
    ( (This)->lpVtbl -> AddFunction(This,Name,Kind,Type,Position,Access,Location,retval) ) 

#define CodeStruct2_AddVariable(This,Name,Type,Position,Access,Location,retval)	\
    ( (This)->lpVtbl -> AddVariable(This,Name,Type,Position,Access,Location,retval) ) 

#define CodeStruct2_AddProperty(This,GetterName,PutterName,Type,Position,Access,Location,retval)	\
    ( (This)->lpVtbl -> AddProperty(This,GetterName,PutterName,Type,Position,Access,Location,retval) ) 

#define CodeStruct2_AddClass(This,Name,Position,Bases,ImplementedInterfaces,Access,retval)	\
    ( (This)->lpVtbl -> AddClass(This,Name,Position,Bases,ImplementedInterfaces,Access,retval) ) 

#define CodeStruct2_AddStruct(This,Name,Position,Bases,ImplementedInterfaces,Access,retval)	\
    ( (This)->lpVtbl -> AddStruct(This,Name,Position,Bases,ImplementedInterfaces,Access,retval) ) 

#define CodeStruct2_AddEnum(This,Name,Position,Bases,Access,retval)	\
    ( (This)->lpVtbl -> AddEnum(This,Name,Position,Bases,Access,retval) ) 

#define CodeStruct2_AddDelegate(This,Name,Type,Position,Access,retval)	\
    ( (This)->lpVtbl -> AddDelegate(This,Name,Type,Position,Access,retval) ) 

#define CodeStruct2_RemoveInterface(This,Element,retval)	\
    ( (This)->lpVtbl -> RemoveInterface(This,Element,retval) ) 


#define CodeStruct2_get_IsGeneric(This,pIsGeneric)	\
    ( (This)->lpVtbl -> get_IsGeneric(This,pIsGeneric) ) 

#define CodeStruct2_AddEvent(This,Name,FullDelegateName,CreatePropertyStyleEvent,Position,Access,ppEvent)	\
    ( (This)->lpVtbl -> AddEvent(This,Name,FullDelegateName,CreatePropertyStyleEvent,Position,Access,ppEvent) ) 

#define CodeStruct2_get_DataTypeKind(This,Kind)	\
    ( (This)->lpVtbl -> get_DataTypeKind(This,Kind) ) 

#define CodeStruct2_put_DataTypeKind(This,Kind)	\
    ( (This)->lpVtbl -> put_DataTypeKind(This,Kind) ) 

#define CodeStruct2_get_Parts(This,Elements)	\
    ( (This)->lpVtbl -> get_Parts(This,Elements) ) 

#endif /* COBJMACROS */


#endif 	/* C style interface */




#endif 	/* __CodeStruct2_INTERFACE_DEFINED__ */


#ifndef __CodeInterface2_INTERFACE_DEFINED__
#define __CodeInterface2_INTERFACE_DEFINED__

/* interface CodeInterface2 */
/* [helpstringcontext][helpstring][helpcontext][uuid][object][oleautomation][dual] */ 


EXTERN_C const IID IID_CodeInterface2;

#if defined(__cplusplus) && !defined(CINTERFACE)
    
    MIDL_INTERFACE("860ab609-8af2-4434-b4dd-a43dee31b017")
    CodeInterface2 : public CodeInterface
    {
    public:
        virtual /* [helpstring][propget][id] */ HRESULT STDMETHODCALLTYPE get_IsGeneric( 
            /* [retval][out] */ __RPC__out VARIANT_BOOL *pIsGeneric) = 0;
        
        virtual /* [helpstring][id] */ HRESULT STDMETHODCALLTYPE AddEvent( 
            __RPC__in BSTR Name,
            __RPC__in BSTR FullDelegateName,
            /* [defaultvalue][in] */ VARIANT_BOOL CreatePropertyStyleEvent,
            /* [optional] */ VARIANT Position,
            /* [defaultvalue] */ enum /* external definition not present */ vsCMAccess Access,
            /* [retval][out] */ __RPC__deref_out_opt CodeEvent **ppEvent) = 0;
        
        virtual /* [helpstringcontext][helpstring][helpcontext][propget][id] */ HRESULT STDMETHODCALLTYPE get_DataTypeKind( 
            /* [retval][out] */ __RPC__out vsCMDataTypeKind *Kind) = 0;
        
        virtual /* [helpstringcontext][helpstring][helpcontext][propput][id] */ HRESULT STDMETHODCALLTYPE put_DataTypeKind( 
            vsCMDataTypeKind Kind) = 0;
        
        virtual /* [helpstringcontext][helpstring][helpcontext][propget][id] */ HRESULT STDMETHODCALLTYPE get_Parts( 
            /* [retval][out] */ __RPC__deref_out_opt /* external definition not present */ CodeElements **Elements) = 0;
        
    };
    
#else 	/* C style interface */

    typedef struct CodeInterface2Vtbl
    {
        BEGIN_INTERFACE
        
        /* [id][restricted][funcdescattr] */ HRESULT ( STDMETHODCALLTYPE *QueryInterface )( 
            CodeInterface2 * This,
            /* [in][idldescattr] */ __RPC__in struct GUID *riid,
            /* [out][idldescattr] */ __RPC__deref_out_opt void **ppvObj,
            /* [retval][out] */ __RPC__out void *retval);
        
        /* [id][restricted][funcdescattr] */ HRESULT ( STDMETHODCALLTYPE *AddRef )( 
            CodeInterface2 * This,
            /* [retval][out] */ __RPC__out unsigned long *retval);
        
        /* [id][restricted][funcdescattr] */ HRESULT ( STDMETHODCALLTYPE *Release )( 
            CodeInterface2 * This,
            /* [retval][out] */ __RPC__out unsigned long *retval);
        
        /* [id][restricted][funcdescattr] */ HRESULT ( STDMETHODCALLTYPE *GetTypeInfoCount )( 
            CodeInterface2 * This,
            /* [out][idldescattr] */ __RPC__out unsigned UINT *pctinfo,
            /* [retval][out] */ __RPC__out void *retval);
        
        /* [id][restricted][funcdescattr] */ HRESULT ( STDMETHODCALLTYPE *GetTypeInfo )( 
            CodeInterface2 * This,
            /* [in][idldescattr] */ unsigned UINT itinfo,
            /* [in][idldescattr] */ unsigned long lcid,
            /* [out][idldescattr] */ __RPC__deref_out_opt void **pptinfo,
            /* [retval][out] */ __RPC__out void *retval);
        
        /* [id][restricted][funcdescattr] */ HRESULT ( STDMETHODCALLTYPE *GetIDsOfNames )( 
            CodeInterface2 * This,
            /* [in][idldescattr] */ __RPC__in struct GUID *riid,
            /* [in][idldescattr] */ __RPC__deref_in_opt signed char **rgszNames,
            /* [in][idldescattr] */ unsigned UINT cNames,
            /* [in][idldescattr] */ unsigned long lcid,
            /* [out][idldescattr] */ __RPC__out signed long *rgdispid,
            /* [retval][out] */ __RPC__out void *retval);
        
        /* [id][restricted][funcdescattr] */ HRESULT ( STDMETHODCALLTYPE *Invoke )( 
            CodeInterface2 * This,
            /* [in][idldescattr] */ signed long dispidMember,
            /* [in][idldescattr] */ __RPC__in struct GUID *riid,
            /* [in][idldescattr] */ unsigned long lcid,
            /* [in][idldescattr] */ unsigned short wFlags,
            /* [in][idldescattr] */ __RPC__in struct DISPPARAMS *pdispparams,
            /* [out][idldescattr] */ __RPC__out VARIANT *pvarResult,
            /* [out][idldescattr] */ __RPC__out struct EXCEPINFO *pexcepinfo,
            /* [out][idldescattr] */ __RPC__out unsigned UINT *puArgErr,
            /* [retval][out] */ __RPC__out void *retval);
        
        /* [id][propget][funcdescattr] */ HRESULT ( STDMETHODCALLTYPE *get_DTE )( 
            CodeInterface2 * This,
            /* [retval][out] */ __RPC__deref_out_opt **retval);
        
        /* [id][propget][funcdescattr] */ HRESULT ( STDMETHODCALLTYPE *get_Collection )( 
            CodeInterface2 * This,
            /* [retval][out] */ __RPC__deref_out_opt CodeElements **retval);
        
        /* [id][propget][funcdescattr] */ HRESULT ( STDMETHODCALLTYPE *get_Name )( 
            CodeInterface2 * This,
            /* [retval][out] */ __RPC__deref_out_opt BSTR *retval);
        
        /* [id][propput][funcdescattr] */ HRESULT ( STDMETHODCALLTYPE *put_Name )( 
            CodeInterface2 * This,
            /* [idldescattr] */ __RPC__in BSTR noname,
            /* [retval][out] */ __RPC__out void *retval);
        
        /* [id][propget][funcdescattr] */ HRESULT ( STDMETHODCALLTYPE *get_FullName )( 
            CodeInterface2 * This,
            /* [retval][out] */ __RPC__deref_out_opt BSTR *retval);
        
        /* [id][propget][funcdescattr] */ HRESULT ( STDMETHODCALLTYPE *get_ProjectItem )( 
            CodeInterface2 * This,
            /* [retval][out] */ __RPC__deref_out_opt ProjectItem **retval);
        
        /* [id][propget][funcdescattr] */ HRESULT ( STDMETHODCALLTYPE *get_Kind )( 
            CodeInterface2 * This,
            /* [retval][out] */ __RPC__out enum vsCMElement *retval);
        
        /* [id][propget][funcdescattr] */ HRESULT ( STDMETHODCALLTYPE *get_IsCodeType )( 
            CodeInterface2 * This,
            /* [retval][out] */ __RPC__out BOOLEAN *retval);
        
        /* [id][propget][funcdescattr] */ HRESULT ( STDMETHODCALLTYPE *get_InfoLocation )( 
            CodeInterface2 * This,
            /* [retval][out] */ __RPC__out enum vsCMInfoLocation *retval);
        
        /* [id][propget][funcdescattr] */ HRESULT ( STDMETHODCALLTYPE *get_Children )( 
            CodeInterface2 * This,
            /* [retval][out] */ __RPC__deref_out_opt CodeElements **retval);
        
        /* [id][propget][funcdescattr] */ HRESULT ( STDMETHODCALLTYPE *get_Language )( 
            CodeInterface2 * This,
            /* [retval][out] */ __RPC__deref_out_opt BSTR *retval);
        
        /* [id][propget][funcdescattr] */ HRESULT ( STDMETHODCALLTYPE *get_StartPoint )( 
            CodeInterface2 * This,
            /* [retval][out] */ __RPC__deref_out_opt TextPoint **retval);
        
        /* [id][propget][funcdescattr] */ HRESULT ( STDMETHODCALLTYPE *get_EndPoint )( 
            CodeInterface2 * This,
            /* [retval][out] */ __RPC__deref_out_opt TextPoint **retval);
        
        /* [id][propget][funcdescattr] */ HRESULT ( STDMETHODCALLTYPE *get_ExtenderNames )( 
            CodeInterface2 * This,
            /* [retval][out] */ __RPC__out VARIANT *retval);
        
        /* [id][propget][funcdescattr] */ HRESULT ( STDMETHODCALLTYPE *get_Extender )( 
            CodeInterface2 * This,
            /* [idldescattr] */ __RPC__in BSTR ExtenderName,
            /* [retval][out] */ __RPC__deref_out_opt IDispatch **retval);
        
        /* [id][propget][funcdescattr] */ HRESULT ( STDMETHODCALLTYPE *get_ExtenderCATID )( 
            CodeInterface2 * This,
            /* [retval][out] */ __RPC__deref_out_opt BSTR *retval);
        
        /* [id][funcdescattr] */ HRESULT ( STDMETHODCALLTYPE *GetStartPoint )( 
            CodeInterface2 * This,
            /* [in][idldescattr] */ enum vsCMPart Part,
            /* [retval][out] */ __RPC__deref_out_opt TextPoint **retval);
        
        /* [id][funcdescattr] */ HRESULT ( STDMETHODCALLTYPE *GetEndPoint )( 
            CodeInterface2 * This,
            /* [in][idldescattr] */ enum vsCMPart Part,
            /* [retval][out] */ __RPC__deref_out_opt TextPoint **retval);
        
        /* [id][propget][funcdescattr] */ HRESULT ( STDMETHODCALLTYPE *get_Parent )( 
            CodeInterface2 * This,
            /* [retval][out] */ __RPC__deref_out_opt IDispatch **retval);
        
        /* [id][propget][funcdescattr] */ HRESULT ( STDMETHODCALLTYPE *get_Namespace )( 
            CodeInterface2 * This,
            /* [retval][out] */ __RPC__deref_out_opt CodeNamespace **retval);
        
        /* [id][propget][funcdescattr] */ HRESULT ( STDMETHODCALLTYPE *get_Bases )( 
            CodeInterface2 * This,
            /* [retval][out] */ __RPC__deref_out_opt CodeElements **retval);
        
        /* [id][propget][funcdescattr] */ HRESULT ( STDMETHODCALLTYPE *get_Members )( 
            CodeInterface2 * This,
            /* [retval][out] */ __RPC__deref_out_opt CodeElements **retval);
        
        /* [id][propput][funcdescattr] */ HRESULT ( STDMETHODCALLTYPE *put_Access )( 
            CodeInterface2 * This,
            /* [idldescattr] */ enum vsCMAccess noname,
            /* [retval][out] */ __RPC__out void *retval);
        
        /* [id][propget][funcdescattr] */ HRESULT ( STDMETHODCALLTYPE *get_Access )( 
            CodeInterface2 * This,
            /* [retval][out] */ __RPC__out enum vsCMAccess *retval);
        
        /* [id][propget][funcdescattr] */ HRESULT ( STDMETHODCALLTYPE *get_Attributes )( 
            CodeInterface2 * This,
            /* [retval][out] */ __RPC__deref_out_opt CodeElements **retval);
        
        /* [id][propget][funcdescattr] */ HRESULT ( STDMETHODCALLTYPE *get_DocComment )( 
            CodeInterface2 * This,
            /* [retval][out] */ __RPC__deref_out_opt BSTR *retval);
        
        /* [id][propput][funcdescattr] */ HRESULT ( STDMETHODCALLTYPE *put_DocComment )( 
            CodeInterface2 * This,
            /* [idldescattr] */ __RPC__in BSTR noname,
            /* [retval][out] */ __RPC__out void *retval);
        
        /* [id][propget][funcdescattr] */ HRESULT ( STDMETHODCALLTYPE *get_Comment )( 
            CodeInterface2 * This,
            /* [retval][out] */ __RPC__deref_out_opt BSTR *retval);
        
        /* [id][propput][funcdescattr] */ HRESULT ( STDMETHODCALLTYPE *put_Comment )( 
            CodeInterface2 * This,
            /* [idldescattr] */ __RPC__in BSTR noname,
            /* [retval][out] */ __RPC__out void *retval);
        
        /* [id][funcdescattr] */ HRESULT ( STDMETHODCALLTYPE *AddBase )( 
            CodeInterface2 * This,
            /* [idldescattr] */ VARIANT Base,
            /* [optional][idldescattr] */ VARIANT Position,
            /* [retval][out] */ __RPC__deref_out_opt CodeElement **retval);
        
        /* [id][funcdescattr] */ HRESULT ( STDMETHODCALLTYPE *AddAttribute )( 
            CodeInterface2 * This,
            /* [idldescattr] */ __RPC__in BSTR Name,
            /* [idldescattr] */ __RPC__in BSTR Value,
            /* [optional][idldescattr] */ VARIANT Position,
            /* [retval][out] */ __RPC__deref_out_opt CodeAttribute **retval);
        
        /* [id][funcdescattr] */ HRESULT ( STDMETHODCALLTYPE *RemoveBase )( 
            CodeInterface2 * This,
            /* [idldescattr] */ VARIANT Element,
            /* [retval][out] */ __RPC__out void *retval);
        
        /* [id][funcdescattr] */ HRESULT ( STDMETHODCALLTYPE *RemoveMember )( 
            CodeInterface2 * This,
            /* [idldescattr] */ VARIANT Element,
            /* [retval][out] */ __RPC__out void *retval);
        
        /* [id][propget][funcdescattr] */ HRESULT ( STDMETHODCALLTYPE *get_IsDerivedFrom )( 
            CodeInterface2 * This,
            /* [idldescattr] */ __RPC__in BSTR FullName,
            /* [retval][out] */ __RPC__out BOOLEAN *retval);
        
        /* [id][propget][funcdescattr] */ HRESULT ( STDMETHODCALLTYPE *get_DerivedTypes )( 
            CodeInterface2 * This,
            /* [retval][out] */ __RPC__deref_out_opt CodeElements **retval);
        
        /* [id][funcdescattr] */ HRESULT ( STDMETHODCALLTYPE *AddFunction )( 
            CodeInterface2 * This,
            /* [idldescattr] */ __RPC__in BSTR Name,
            /* [idldescattr] */ enum vsCMFunction Kind,
            /* [idldescattr] */ VARIANT Type,
            /* [idldescattr] */ VARIANT Position,
            /* [idldescattr] */ enum vsCMAccess Access,
            /* [retval][out] */ __RPC__deref_out_opt CodeFunction **retval);
        
        /* [id][funcdescattr] */ HRESULT ( STDMETHODCALLTYPE *AddProperty )( 
            CodeInterface2 * This,
            /* [idldescattr] */ __RPC__in BSTR GetterName,
            /* [idldescattr] */ __RPC__in BSTR PutterName,
            /* [idldescattr] */ VARIANT Type,
            /* [idldescattr] */ VARIANT Position,
            /* [idldescattr] */ enum vsCMAccess Access,
            /* [optional][idldescattr] */ VARIANT Location,
            /* [retval][out] */ __RPC__deref_out_opt CodeProperty **retval);
        
        /* [helpstring][propget][id] */ HRESULT ( STDMETHODCALLTYPE *get_IsGeneric )( 
            CodeInterface2 * This,
            /* [retval][out] */ __RPC__out VARIANT_BOOL *pIsGeneric);
        
        /* [helpstring][id] */ HRESULT ( STDMETHODCALLTYPE *AddEvent )( 
            CodeInterface2 * This,
            __RPC__in BSTR Name,
            __RPC__in BSTR FullDelegateName,
            /* [defaultvalue][in] */ VARIANT_BOOL CreatePropertyStyleEvent,
            /* [optional] */ VARIANT Position,
            /* [defaultvalue] */ enum /* external definition not present */ vsCMAccess Access,
            /* [retval][out] */ __RPC__deref_out_opt CodeEvent **ppEvent);
        
        /* [helpstringcontext][helpstring][helpcontext][propget][id] */ HRESULT ( STDMETHODCALLTYPE *get_DataTypeKind )( 
            CodeInterface2 * This,
            /* [retval][out] */ __RPC__out vsCMDataTypeKind *Kind);
        
        /* [helpstringcontext][helpstring][helpcontext][propput][id] */ HRESULT ( STDMETHODCALLTYPE *put_DataTypeKind )( 
            CodeInterface2 * This,
            vsCMDataTypeKind Kind);
        
        /* [helpstringcontext][helpstring][helpcontext][propget][id] */ HRESULT ( STDMETHODCALLTYPE *get_Parts )( 
            CodeInterface2 * This,
            /* [retval][out] */ __RPC__deref_out_opt /* external definition not present */ CodeElements **Elements);
        
        END_INTERFACE
    } CodeInterface2Vtbl;

    interface CodeInterface2
    {
        CONST_VTBL struct CodeInterface2Vtbl *lpVtbl;
    };

    

#ifdef COBJMACROS


#define CodeInterface2_QueryInterface(This,riid,ppvObj,retval)	\
    ( (This)->lpVtbl -> QueryInterface(This,riid,ppvObj,retval) ) 

#define CodeInterface2_AddRef(This,retval)	\
    ( (This)->lpVtbl -> AddRef(This,retval) ) 

#define CodeInterface2_Release(This,retval)	\
    ( (This)->lpVtbl -> Release(This,retval) ) 

#define CodeInterface2_GetTypeInfoCount(This,pctinfo,retval)	\
    ( (This)->lpVtbl -> GetTypeInfoCount(This,pctinfo,retval) ) 

#define CodeInterface2_GetTypeInfo(This,itinfo,lcid,pptinfo,retval)	\
    ( (This)->lpVtbl -> GetTypeInfo(This,itinfo,lcid,pptinfo,retval) ) 

#define CodeInterface2_GetIDsOfNames(This,riid,rgszNames,cNames,lcid,rgdispid,retval)	\
    ( (This)->lpVtbl -> GetIDsOfNames(This,riid,rgszNames,cNames,lcid,rgdispid,retval) ) 

#define CodeInterface2_Invoke(This,dispidMember,riid,lcid,wFlags,pdispparams,pvarResult,pexcepinfo,puArgErr,retval)	\
    ( (This)->lpVtbl -> Invoke(This,dispidMember,riid,lcid,wFlags,pdispparams,pvarResult,pexcepinfo,puArgErr,retval) ) 

#define CodeInterface2_get_DTE(This,retval)	\
    ( (This)->lpVtbl -> get_DTE(This,retval) ) 

#define CodeInterface2_get_Collection(This,retval)	\
    ( (This)->lpVtbl -> get_Collection(This,retval) ) 

#define CodeInterface2_get_Name(This,retval)	\
    ( (This)->lpVtbl -> get_Name(This,retval) ) 

#define CodeInterface2_put_Name(This,noname,retval)	\
    ( (This)->lpVtbl -> put_Name(This,noname,retval) ) 

#define CodeInterface2_get_FullName(This,retval)	\
    ( (This)->lpVtbl -> get_FullName(This,retval) ) 

#define CodeInterface2_get_ProjectItem(This,retval)	\
    ( (This)->lpVtbl -> get_ProjectItem(This,retval) ) 

#define CodeInterface2_get_Kind(This,retval)	\
    ( (This)->lpVtbl -> get_Kind(This,retval) ) 

#define CodeInterface2_get_IsCodeType(This,retval)	\
    ( (This)->lpVtbl -> get_IsCodeType(This,retval) ) 

#define CodeInterface2_get_InfoLocation(This,retval)	\
    ( (This)->lpVtbl -> get_InfoLocation(This,retval) ) 

#define CodeInterface2_get_Children(This,retval)	\
    ( (This)->lpVtbl -> get_Children(This,retval) ) 

#define CodeInterface2_get_Language(This,retval)	\
    ( (This)->lpVtbl -> get_Language(This,retval) ) 

#define CodeInterface2_get_StartPoint(This,retval)	\
    ( (This)->lpVtbl -> get_StartPoint(This,retval) ) 

#define CodeInterface2_get_EndPoint(This,retval)	\
    ( (This)->lpVtbl -> get_EndPoint(This,retval) ) 

#define CodeInterface2_get_ExtenderNames(This,retval)	\
    ( (This)->lpVtbl -> get_ExtenderNames(This,retval) ) 

#define CodeInterface2_get_Extender(This,ExtenderName,retval)	\
    ( (This)->lpVtbl -> get_Extender(This,ExtenderName,retval) ) 

#define CodeInterface2_get_ExtenderCATID(This,retval)	\
    ( (This)->lpVtbl -> get_ExtenderCATID(This,retval) ) 

#define CodeInterface2_GetStartPoint(This,Part,retval)	\
    ( (This)->lpVtbl -> GetStartPoint(This,Part,retval) ) 

#define CodeInterface2_GetEndPoint(This,Part,retval)	\
    ( (This)->lpVtbl -> GetEndPoint(This,Part,retval) ) 

#define CodeInterface2_get_Parent(This,retval)	\
    ( (This)->lpVtbl -> get_Parent(This,retval) ) 

#define CodeInterface2_get_Namespace(This,retval)	\
    ( (This)->lpVtbl -> get_Namespace(This,retval) ) 

#define CodeInterface2_get_Bases(This,retval)	\
    ( (This)->lpVtbl -> get_Bases(This,retval) ) 

#define CodeInterface2_get_Members(This,retval)	\
    ( (This)->lpVtbl -> get_Members(This,retval) ) 

#define CodeInterface2_put_Access(This,noname,retval)	\
    ( (This)->lpVtbl -> put_Access(This,noname,retval) ) 

#define CodeInterface2_get_Access(This,retval)	\
    ( (This)->lpVtbl -> get_Access(This,retval) ) 

#define CodeInterface2_get_Attributes(This,retval)	\
    ( (This)->lpVtbl -> get_Attributes(This,retval) ) 

#define CodeInterface2_get_DocComment(This,retval)	\
    ( (This)->lpVtbl -> get_DocComment(This,retval) ) 

#define CodeInterface2_put_DocComment(This,noname,retval)	\
    ( (This)->lpVtbl -> put_DocComment(This,noname,retval) ) 

#define CodeInterface2_get_Comment(This,retval)	\
    ( (This)->lpVtbl -> get_Comment(This,retval) ) 

#define CodeInterface2_put_Comment(This,noname,retval)	\
    ( (This)->lpVtbl -> put_Comment(This,noname,retval) ) 

#define CodeInterface2_AddBase(This,Base,Position,retval)	\
    ( (This)->lpVtbl -> AddBase(This,Base,Position,retval) ) 

#define CodeInterface2_AddAttribute(This,Name,Value,Position,retval)	\
    ( (This)->lpVtbl -> AddAttribute(This,Name,Value,Position,retval) ) 

#define CodeInterface2_RemoveBase(This,Element,retval)	\
    ( (This)->lpVtbl -> RemoveBase(This,Element,retval) ) 

#define CodeInterface2_RemoveMember(This,Element,retval)	\
    ( (This)->lpVtbl -> RemoveMember(This,Element,retval) ) 

#define CodeInterface2_get_IsDerivedFrom(This,FullName,retval)	\
    ( (This)->lpVtbl -> get_IsDerivedFrom(This,FullName,retval) ) 

#define CodeInterface2_get_DerivedTypes(This,retval)	\
    ( (This)->lpVtbl -> get_DerivedTypes(This,retval) ) 

#define CodeInterface2_AddFunction(This,Name,Kind,Type,Position,Access,retval)	\
    ( (This)->lpVtbl -> AddFunction(This,Name,Kind,Type,Position,Access,retval) ) 

#define CodeInterface2_AddProperty(This,GetterName,PutterName,Type,Position,Access,Location,retval)	\
    ( (This)->lpVtbl -> AddProperty(This,GetterName,PutterName,Type,Position,Access,Location,retval) ) 


#define CodeInterface2_get_IsGeneric(This,pIsGeneric)	\
    ( (This)->lpVtbl -> get_IsGeneric(This,pIsGeneric) ) 

#define CodeInterface2_AddEvent(This,Name,FullDelegateName,CreatePropertyStyleEvent,Position,Access,ppEvent)	\
    ( (This)->lpVtbl -> AddEvent(This,Name,FullDelegateName,CreatePropertyStyleEvent,Position,Access,ppEvent) ) 

#define CodeInterface2_get_DataTypeKind(This,Kind)	\
    ( (This)->lpVtbl -> get_DataTypeKind(This,Kind) ) 

#define CodeInterface2_put_DataTypeKind(This,Kind)	\
    ( (This)->lpVtbl -> put_DataTypeKind(This,Kind) ) 

#define CodeInterface2_get_Parts(This,Elements)	\
    ( (This)->lpVtbl -> get_Parts(This,Elements) ) 

#endif /* COBJMACROS */


#endif 	/* C style interface */




#endif 	/* __CodeInterface2_INTERFACE_DEFINED__ */


#ifndef __CodeTypeRef2_INTERFACE_DEFINED__
#define __CodeTypeRef2_INTERFACE_DEFINED__

/* interface CodeTypeRef2 */
/* [helpstringcontext][helpstring][helpcontext][uuid][object][oleautomation][dual] */ 


EXTERN_C const IID IID_CodeTypeRef2;

#if defined(__cplusplus) && !defined(CINTERFACE)
    
    MIDL_INTERFACE("9883c07a-fa07-4ce7-bd8e-01e1a3f3a3f7")
    CodeTypeRef2 : public CodeTypeRef
    {
    public:
        virtual /* [helpstring][propget][id] */ HRESULT STDMETHODCALLTYPE get_IsGeneric( 
            /* [retval][out] */ __RPC__out VARIANT_BOOL *pIsGeneric) = 0;
        
    };
    
#else 	/* C style interface */

    typedef struct CodeTypeRef2Vtbl
    {
        BEGIN_INTERFACE
        
        /* [id][restricted][funcdescattr] */ HRESULT ( STDMETHODCALLTYPE *QueryInterface )( 
            CodeTypeRef2 * This,
            /* [in][idldescattr] */ __RPC__in struct GUID *riid,
            /* [out][idldescattr] */ __RPC__deref_out_opt void **ppvObj,
            /* [retval][out] */ __RPC__out void *retval);
        
        /* [id][restricted][funcdescattr] */ HRESULT ( STDMETHODCALLTYPE *AddRef )( 
            CodeTypeRef2 * This,
            /* [retval][out] */ __RPC__out unsigned long *retval);
        
        /* [id][restricted][funcdescattr] */ HRESULT ( STDMETHODCALLTYPE *Release )( 
            CodeTypeRef2 * This,
            /* [retval][out] */ __RPC__out unsigned long *retval);
        
        /* [id][restricted][funcdescattr] */ HRESULT ( STDMETHODCALLTYPE *GetTypeInfoCount )( 
            CodeTypeRef2 * This,
            /* [out][idldescattr] */ __RPC__out unsigned UINT *pctinfo,
            /* [retval][out] */ __RPC__out void *retval);
        
        /* [id][restricted][funcdescattr] */ HRESULT ( STDMETHODCALLTYPE *GetTypeInfo )( 
            CodeTypeRef2 * This,
            /* [in][idldescattr] */ unsigned UINT itinfo,
            /* [in][idldescattr] */ unsigned long lcid,
            /* [out][idldescattr] */ __RPC__deref_out_opt void **pptinfo,
            /* [retval][out] */ __RPC__out void *retval);
        
        /* [id][restricted][funcdescattr] */ HRESULT ( STDMETHODCALLTYPE *GetIDsOfNames )( 
            CodeTypeRef2 * This,
            /* [in][idldescattr] */ __RPC__in struct GUID *riid,
            /* [in][idldescattr] */ __RPC__deref_in_opt signed char **rgszNames,
            /* [in][idldescattr] */ unsigned UINT cNames,
            /* [in][idldescattr] */ unsigned long lcid,
            /* [out][idldescattr] */ __RPC__out signed long *rgdispid,
            /* [retval][out] */ __RPC__out void *retval);
        
        /* [id][restricted][funcdescattr] */ HRESULT ( STDMETHODCALLTYPE *Invoke )( 
            CodeTypeRef2 * This,
            /* [in][idldescattr] */ signed long dispidMember,
            /* [in][idldescattr] */ __RPC__in struct GUID *riid,
            /* [in][idldescattr] */ unsigned long lcid,
            /* [in][idldescattr] */ unsigned short wFlags,
            /* [in][idldescattr] */ __RPC__in struct DISPPARAMS *pdispparams,
            /* [out][idldescattr] */ __RPC__out VARIANT *pvarResult,
            /* [out][idldescattr] */ __RPC__out struct EXCEPINFO *pexcepinfo,
            /* [out][idldescattr] */ __RPC__out unsigned UINT *puArgErr,
            /* [retval][out] */ __RPC__out void *retval);
        
        /* [id][propget][funcdescattr] */ HRESULT ( STDMETHODCALLTYPE *get_DTE )( 
            CodeTypeRef2 * This,
            /* [retval][out] */ __RPC__deref_out_opt **retval);
        
        /* [id][propget][funcdescattr] */ HRESULT ( STDMETHODCALLTYPE *get_Parent )( 
            CodeTypeRef2 * This,
            /* [retval][out] */ __RPC__deref_out_opt IDispatch **retval);
        
        /* [id][propget][funcdescattr] */ HRESULT ( STDMETHODCALLTYPE *get_TypeKind )( 
            CodeTypeRef2 * This,
            /* [retval][out] */ __RPC__out enum vsCMTypeRef *retval);
        
        /* [id][propget][funcdescattr] */ HRESULT ( STDMETHODCALLTYPE *get_CodeType )( 
            CodeTypeRef2 * This,
            /* [retval][out] */ __RPC__deref_out_opt CodeType **retval);
        
        /* [id][propput][funcdescattr] */ HRESULT ( STDMETHODCALLTYPE *put_CodeType )( 
            CodeTypeRef2 * This,
            /* [idldescattr] */ __RPC__in_opt CodeType *noname,
            /* [retval][out] */ __RPC__out void *retval);
        
        /* [id][propget][funcdescattr] */ HRESULT ( STDMETHODCALLTYPE *get_ElementType )( 
            CodeTypeRef2 * This,
            /* [retval][out] */ __RPC__deref_out_opt CodeTypeRef **retval);
        
        /* [id][propput][funcdescattr] */ HRESULT ( STDMETHODCALLTYPE *put_ElementType )( 
            CodeTypeRef2 * This,
            /* [idldescattr] */ __RPC__in_opt CodeTypeRef *noname,
            /* [retval][out] */ __RPC__out void *retval);
        
        /* [id][propget][funcdescattr] */ HRESULT ( STDMETHODCALLTYPE *get_AsString )( 
            CodeTypeRef2 * This,
            /* [retval][out] */ __RPC__deref_out_opt BSTR *retval);
        
        /* [id][propget][funcdescattr] */ HRESULT ( STDMETHODCALLTYPE *get_AsFullName )( 
            CodeTypeRef2 * This,
            /* [retval][out] */ __RPC__deref_out_opt BSTR *retval);
        
        /* [id][propget][funcdescattr] */ HRESULT ( STDMETHODCALLTYPE *get_Rank )( 
            CodeTypeRef2 * This,
            /* [retval][out] */ __RPC__out signed long *retval);
        
        /* [id][propput][funcdescattr] */ HRESULT ( STDMETHODCALLTYPE *put_Rank )( 
            CodeTypeRef2 * This,
            /* [idldescattr] */ signed long noname,
            /* [retval][out] */ __RPC__out void *retval);
        
        /* [id][funcdescattr] */ HRESULT ( STDMETHODCALLTYPE *CreateArrayType )( 
            CodeTypeRef2 * This,
            /* [idldescattr] */ signed long Rank,
            /* [retval][out] */ __RPC__deref_out_opt CodeTypeRef **retval);
        
        /* [helpstring][propget][id] */ HRESULT ( STDMETHODCALLTYPE *get_IsGeneric )( 
            CodeTypeRef2 * This,
            /* [retval][out] */ __RPC__out VARIANT_BOOL *pIsGeneric);
        
        END_INTERFACE
    } CodeTypeRef2Vtbl;

    interface CodeTypeRef2
    {
        CONST_VTBL struct CodeTypeRef2Vtbl *lpVtbl;
    };

    

#ifdef COBJMACROS


#define CodeTypeRef2_QueryInterface(This,riid,ppvObj,retval)	\
    ( (This)->lpVtbl -> QueryInterface(This,riid,ppvObj,retval) ) 

#define CodeTypeRef2_AddRef(This,retval)	\
    ( (This)->lpVtbl -> AddRef(This,retval) ) 

#define CodeTypeRef2_Release(This,retval)	\
    ( (This)->lpVtbl -> Release(This,retval) ) 

#define CodeTypeRef2_GetTypeInfoCount(This,pctinfo,retval)	\
    ( (This)->lpVtbl -> GetTypeInfoCount(This,pctinfo,retval) ) 

#define CodeTypeRef2_GetTypeInfo(This,itinfo,lcid,pptinfo,retval)	\
    ( (This)->lpVtbl -> GetTypeInfo(This,itinfo,lcid,pptinfo,retval) ) 

#define CodeTypeRef2_GetIDsOfNames(This,riid,rgszNames,cNames,lcid,rgdispid,retval)	\
    ( (This)->lpVtbl -> GetIDsOfNames(This,riid,rgszNames,cNames,lcid,rgdispid,retval) ) 

#define CodeTypeRef2_Invoke(This,dispidMember,riid,lcid,wFlags,pdispparams,pvarResult,pexcepinfo,puArgErr,retval)	\
    ( (This)->lpVtbl -> Invoke(This,dispidMember,riid,lcid,wFlags,pdispparams,pvarResult,pexcepinfo,puArgErr,retval) ) 

#define CodeTypeRef2_get_DTE(This,retval)	\
    ( (This)->lpVtbl -> get_DTE(This,retval) ) 

#define CodeTypeRef2_get_Parent(This,retval)	\
    ( (This)->lpVtbl -> get_Parent(This,retval) ) 

#define CodeTypeRef2_get_TypeKind(This,retval)	\
    ( (This)->lpVtbl -> get_TypeKind(This,retval) ) 

#define CodeTypeRef2_get_CodeType(This,retval)	\
    ( (This)->lpVtbl -> get_CodeType(This,retval) ) 

#define CodeTypeRef2_put_CodeType(This,noname,retval)	\
    ( (This)->lpVtbl -> put_CodeType(This,noname,retval) ) 

#define CodeTypeRef2_get_ElementType(This,retval)	\
    ( (This)->lpVtbl -> get_ElementType(This,retval) ) 

#define CodeTypeRef2_put_ElementType(This,noname,retval)	\
    ( (This)->lpVtbl -> put_ElementType(This,noname,retval) ) 

#define CodeTypeRef2_get_AsString(This,retval)	\
    ( (This)->lpVtbl -> get_AsString(This,retval) ) 

#define CodeTypeRef2_get_AsFullName(This,retval)	\
    ( (This)->lpVtbl -> get_AsFullName(This,retval) ) 

#define CodeTypeRef2_get_Rank(This,retval)	\
    ( (This)->lpVtbl -> get_Rank(This,retval) ) 

#define CodeTypeRef2_put_Rank(This,noname,retval)	\
    ( (This)->lpVtbl -> put_Rank(This,noname,retval) ) 

#define CodeTypeRef2_CreateArrayType(This,Rank,retval)	\
    ( (This)->lpVtbl -> CreateArrayType(This,Rank,retval) ) 


#define CodeTypeRef2_get_IsGeneric(This,pIsGeneric)	\
    ( (This)->lpVtbl -> get_IsGeneric(This,pIsGeneric) ) 

#endif /* COBJMACROS */


#endif 	/* C style interface */




#endif 	/* __CodeTypeRef2_INTERFACE_DEFINED__ */


#ifndef __CodeProperty2_INTERFACE_DEFINED__
#define __CodeProperty2_INTERFACE_DEFINED__

/* interface CodeProperty2 */
/* [helpstringcontext][helpstring][helpcontext][uuid][object][oleautomation][dual] */ 


EXTERN_C const IID IID_CodeProperty2;

#if defined(__cplusplus) && !defined(CINTERFACE)
    
    MIDL_INTERFACE("33770C02-21B7-4224-A577-6877BDBA60EA")
    CodeProperty2 : public CodeProperty
    {
    public:
        virtual /* [helpstringcontext][helpstring][helpcontext][propget][id] */ HRESULT STDMETHODCALLTYPE get_Parameters( 
            /* [retval][out] */ __RPC__deref_out_opt /* external definition not present */ CodeElements **ppMembers) = 0;
        
        virtual /* [helpstringcontext][helpstring][helpcontext][id] */ HRESULT STDMETHODCALLTYPE AddParameter( 
            __RPC__in BSTR Name,
            VARIANT Type,
            /* [optional] */ VARIANT Position,
            /* [retval][out] */ __RPC__deref_out_opt /* external definition not present */ CodeParameter **ppCodeParameter) = 0;
        
        virtual /* [helpstring][propget][id] */ HRESULT STDMETHODCALLTYPE get_IsGeneric( 
            /* [retval][out] */ __RPC__out VARIANT_BOOL *pIsGeneric) = 0;
        
        virtual /* [helpstringcontext][helpstring][helpcontext][propget][id] */ HRESULT STDMETHODCALLTYPE get_OverrideKind( 
            /* [retval][out] */ __RPC__out vsCMOverrideKind *Kind) = 0;
        
        virtual /* [helpstringcontext][helpstring][helpcontext][propput][id] */ HRESULT STDMETHODCALLTYPE put_OverrideKind( 
            vsCMOverrideKind Kind) = 0;
        
        virtual /* [helpstringcontext][helpstring][helpcontext][propget][id] */ HRESULT STDMETHODCALLTYPE get_IsShared( 
            /* [retval][out] */ __RPC__out VARIANT_BOOL *pIsShared) = 0;
        
        virtual /* [helpstringcontext][helpstring][helpcontext][propput][id] */ HRESULT STDMETHODCALLTYPE put_IsShared( 
            VARIANT_BOOL Shared) = 0;
        
        virtual /* [helpstring][propget][id] */ HRESULT STDMETHODCALLTYPE get_IsDefault( 
            /* [retval][out] */ __RPC__out VARIANT_BOOL *pIsDefault) = 0;
        
        virtual /* [helpstring][propput][id] */ HRESULT STDMETHODCALLTYPE put_IsDefault( 
            VARIANT_BOOL Default) = 0;
        
        virtual /* [helpstringcontext][helpstring][helpcontext][propget][id] */ HRESULT STDMETHODCALLTYPE get_Parent2( 
            /* [retval][out] */ __RPC__deref_out_opt /* external definition not present */ CodeElement **pParent) = 0;
        
        virtual /* [helpstringcontext][helpstring][helpcontext][id] */ HRESULT STDMETHODCALLTYPE RemoveParameter( 
            VARIANT Element) = 0;
        
        virtual /* [helpstring][propget][id] */ HRESULT STDMETHODCALLTYPE get_ReadWrite( 
            /* [retval][out] */ __RPC__out vsCMPropertyKind *pPropertyKind) = 0;
        
    };
    
#else 	/* C style interface */

    typedef struct CodeProperty2Vtbl
    {
        BEGIN_INTERFACE
        
        /* [id][restricted][funcdescattr] */ HRESULT ( STDMETHODCALLTYPE *QueryInterface )( 
            CodeProperty2 * This,
            /* [in][idldescattr] */ __RPC__in struct GUID *riid,
            /* [out][idldescattr] */ __RPC__deref_out_opt void **ppvObj,
            /* [retval][out] */ __RPC__out void *retval);
        
        /* [id][restricted][funcdescattr] */ HRESULT ( STDMETHODCALLTYPE *AddRef )( 
            CodeProperty2 * This,
            /* [retval][out] */ __RPC__out unsigned long *retval);
        
        /* [id][restricted][funcdescattr] */ HRESULT ( STDMETHODCALLTYPE *Release )( 
            CodeProperty2 * This,
            /* [retval][out] */ __RPC__out unsigned long *retval);
        
        /* [id][restricted][funcdescattr] */ HRESULT ( STDMETHODCALLTYPE *GetTypeInfoCount )( 
            CodeProperty2 * This,
            /* [out][idldescattr] */ __RPC__out unsigned UINT *pctinfo,
            /* [retval][out] */ __RPC__out void *retval);
        
        /* [id][restricted][funcdescattr] */ HRESULT ( STDMETHODCALLTYPE *GetTypeInfo )( 
            CodeProperty2 * This,
            /* [in][idldescattr] */ unsigned UINT itinfo,
            /* [in][idldescattr] */ unsigned long lcid,
            /* [out][idldescattr] */ __RPC__deref_out_opt void **pptinfo,
            /* [retval][out] */ __RPC__out void *retval);
        
        /* [id][restricted][funcdescattr] */ HRESULT ( STDMETHODCALLTYPE *GetIDsOfNames )( 
            CodeProperty2 * This,
            /* [in][idldescattr] */ __RPC__in struct GUID *riid,
            /* [in][idldescattr] */ __RPC__deref_in_opt signed char **rgszNames,
            /* [in][idldescattr] */ unsigned UINT cNames,
            /* [in][idldescattr] */ unsigned long lcid,
            /* [out][idldescattr] */ __RPC__out signed long *rgdispid,
            /* [retval][out] */ __RPC__out void *retval);
        
        /* [id][restricted][funcdescattr] */ HRESULT ( STDMETHODCALLTYPE *Invoke )( 
            CodeProperty2 * This,
            /* [in][idldescattr] */ signed long dispidMember,
            /* [in][idldescattr] */ __RPC__in struct GUID *riid,
            /* [in][idldescattr] */ unsigned long lcid,
            /* [in][idldescattr] */ unsigned short wFlags,
            /* [in][idldescattr] */ __RPC__in struct DISPPARAMS *pdispparams,
            /* [out][idldescattr] */ __RPC__out VARIANT *pvarResult,
            /* [out][idldescattr] */ __RPC__out struct EXCEPINFO *pexcepinfo,
            /* [out][idldescattr] */ __RPC__out unsigned UINT *puArgErr,
            /* [retval][out] */ __RPC__out void *retval);
        
        /* [id][propget][funcdescattr] */ HRESULT ( STDMETHODCALLTYPE *get_DTE )( 
            CodeProperty2 * This,
            /* [retval][out] */ __RPC__deref_out_opt **retval);
        
        /* [id][propget][funcdescattr] */ HRESULT ( STDMETHODCALLTYPE *get_Collection )( 
            CodeProperty2 * This,
            /* [retval][out] */ __RPC__deref_out_opt CodeElements **retval);
        
        /* [id][propget][funcdescattr] */ HRESULT ( STDMETHODCALLTYPE *get_Name )( 
            CodeProperty2 * This,
            /* [retval][out] */ __RPC__deref_out_opt BSTR *retval);
        
        /* [id][propput][funcdescattr] */ HRESULT ( STDMETHODCALLTYPE *put_Name )( 
            CodeProperty2 * This,
            /* [idldescattr] */ __RPC__in BSTR noname,
            /* [retval][out] */ __RPC__out void *retval);
        
        /* [id][propget][funcdescattr] */ HRESULT ( STDMETHODCALLTYPE *get_FullName )( 
            CodeProperty2 * This,
            /* [retval][out] */ __RPC__deref_out_opt BSTR *retval);
        
        /* [id][propget][funcdescattr] */ HRESULT ( STDMETHODCALLTYPE *get_ProjectItem )( 
            CodeProperty2 * This,
            /* [retval][out] */ __RPC__deref_out_opt ProjectItem **retval);
        
        /* [id][propget][funcdescattr] */ HRESULT ( STDMETHODCALLTYPE *get_Kind )( 
            CodeProperty2 * This,
            /* [retval][out] */ __RPC__out enum vsCMElement *retval);
        
        /* [id][propget][funcdescattr] */ HRESULT ( STDMETHODCALLTYPE *get_IsCodeType )( 
            CodeProperty2 * This,
            /* [retval][out] */ __RPC__out BOOLEAN *retval);
        
        /* [id][propget][funcdescattr] */ HRESULT ( STDMETHODCALLTYPE *get_InfoLocation )( 
            CodeProperty2 * This,
            /* [retval][out] */ __RPC__out enum vsCMInfoLocation *retval);
        
        /* [id][propget][funcdescattr] */ HRESULT ( STDMETHODCALLTYPE *get_Children )( 
            CodeProperty2 * This,
            /* [retval][out] */ __RPC__deref_out_opt CodeElements **retval);
        
        /* [id][propget][funcdescattr] */ HRESULT ( STDMETHODCALLTYPE *get_Language )( 
            CodeProperty2 * This,
            /* [retval][out] */ __RPC__deref_out_opt BSTR *retval);
        
        /* [id][propget][funcdescattr] */ HRESULT ( STDMETHODCALLTYPE *get_StartPoint )( 
            CodeProperty2 * This,
            /* [retval][out] */ __RPC__deref_out_opt TextPoint **retval);
        
        /* [id][propget][funcdescattr] */ HRESULT ( STDMETHODCALLTYPE *get_EndPoint )( 
            CodeProperty2 * This,
            /* [retval][out] */ __RPC__deref_out_opt TextPoint **retval);
        
        /* [id][propget][funcdescattr] */ HRESULT ( STDMETHODCALLTYPE *get_ExtenderNames )( 
            CodeProperty2 * This,
            /* [retval][out] */ __RPC__out VARIANT *retval);
        
        /* [id][propget][funcdescattr] */ HRESULT ( STDMETHODCALLTYPE *get_Extender )( 
            CodeProperty2 * This,
            /* [idldescattr] */ __RPC__in BSTR ExtenderName,
            /* [retval][out] */ __RPC__deref_out_opt IDispatch **retval);
        
        /* [id][propget][funcdescattr] */ HRESULT ( STDMETHODCALLTYPE *get_ExtenderCATID )( 
            CodeProperty2 * This,
            /* [retval][out] */ __RPC__deref_out_opt BSTR *retval);
        
        /* [id][funcdescattr] */ HRESULT ( STDMETHODCALLTYPE *GetStartPoint )( 
            CodeProperty2 * This,
            /* [in][idldescattr] */ enum vsCMPart Part,
            /* [retval][out] */ __RPC__deref_out_opt TextPoint **retval);
        
        /* [id][funcdescattr] */ HRESULT ( STDMETHODCALLTYPE *GetEndPoint )( 
            CodeProperty2 * This,
            /* [in][idldescattr] */ enum vsCMPart Part,
            /* [retval][out] */ __RPC__deref_out_opt TextPoint **retval);
        
        /* [id][propget][funcdescattr] */ HRESULT ( STDMETHODCALLTYPE *get_Parent )( 
            CodeProperty2 * This,
            /* [retval][out] */ __RPC__deref_out_opt CodeClass **retval);
        
        /* [id][propget][funcdescattr] */ HRESULT ( STDMETHODCALLTYPE *get_Prototype )( 
            CodeProperty2 * This,
            /* [in][idldescattr] */ signed long Flags,
            /* [retval][out] */ __RPC__deref_out_opt BSTR *retval);
        
        /* [id][propput][funcdescattr] */ HRESULT ( STDMETHODCALLTYPE *put_Type )( 
            CodeProperty2 * This,
            /* [idldescattr] */ __RPC__in_opt CodeTypeRef *noname,
            /* [retval][out] */ __RPC__out void *retval);
        
        /* [id][propget][funcdescattr] */ HRESULT ( STDMETHODCALLTYPE *get_Type )( 
            CodeProperty2 * This,
            /* [retval][out] */ __RPC__deref_out_opt CodeTypeRef **retval);
        
        /* [id][propget][funcdescattr] */ HRESULT ( STDMETHODCALLTYPE *get_Getter )( 
            CodeProperty2 * This,
            /* [retval][out] */ __RPC__deref_out_opt CodeFunction **retval);
        
        /* [id][propput][funcdescattr] */ HRESULT ( STDMETHODCALLTYPE *put_Getter )( 
            CodeProperty2 * This,
            /* [idldescattr] */ __RPC__in_opt CodeFunction *noname,
            /* [retval][out] */ __RPC__out void *retval);
        
        /* [id][propget][funcdescattr] */ HRESULT ( STDMETHODCALLTYPE *get_Setter )( 
            CodeProperty2 * This,
            /* [retval][out] */ __RPC__deref_out_opt CodeFunction **retval);
        
        /* [id][propput][funcdescattr] */ HRESULT ( STDMETHODCALLTYPE *put_Setter )( 
            CodeProperty2 * This,
            /* [idldescattr] */ __RPC__in_opt CodeFunction *noname,
            /* [retval][out] */ __RPC__out void *retval);
        
        /* [id][propput][funcdescattr] */ HRESULT ( STDMETHODCALLTYPE *put_Access )( 
            CodeProperty2 * This,
            /* [idldescattr] */ enum vsCMAccess noname,
            /* [retval][out] */ __RPC__out void *retval);
        
        /* [id][propget][funcdescattr] */ HRESULT ( STDMETHODCALLTYPE *get_Access )( 
            CodeProperty2 * This,
            /* [retval][out] */ __RPC__out enum vsCMAccess *retval);
        
        /* [id][propget][funcdescattr] */ HRESULT ( STDMETHODCALLTYPE *get_Attributes )( 
            CodeProperty2 * This,
            /* [retval][out] */ __RPC__deref_out_opt CodeElements **retval);
        
        /* [id][propget][funcdescattr] */ HRESULT ( STDMETHODCALLTYPE *get_DocComment )( 
            CodeProperty2 * This,
            /* [retval][out] */ __RPC__deref_out_opt BSTR *retval);
        
        /* [id][propput][funcdescattr] */ HRESULT ( STDMETHODCALLTYPE *put_DocComment )( 
            CodeProperty2 * This,
            /* [idldescattr] */ __RPC__in BSTR noname,
            /* [retval][out] */ __RPC__out void *retval);
        
        /* [id][propget][funcdescattr] */ HRESULT ( STDMETHODCALLTYPE *get_Comment )( 
            CodeProperty2 * This,
            /* [retval][out] */ __RPC__deref_out_opt BSTR *retval);
        
        /* [id][propput][funcdescattr] */ HRESULT ( STDMETHODCALLTYPE *put_Comment )( 
            CodeProperty2 * This,
            /* [idldescattr] */ __RPC__in BSTR noname,
            /* [retval][out] */ __RPC__out void *retval);
        
        /* [id][funcdescattr] */ HRESULT ( STDMETHODCALLTYPE *AddAttribute )( 
            CodeProperty2 * This,
            /* [idldescattr] */ __RPC__in BSTR Name,
            /* [idldescattr] */ __RPC__in BSTR Value,
            /* [optional][idldescattr] */ VARIANT Position,
            /* [retval][out] */ __RPC__deref_out_opt CodeAttribute **retval);
        
        /* [helpstringcontext][helpstring][helpcontext][propget][id] */ HRESULT ( STDMETHODCALLTYPE *get_Parameters )( 
            CodeProperty2 * This,
            /* [retval][out] */ __RPC__deref_out_opt /* external definition not present */ CodeElements **ppMembers);
        
        /* [helpstringcontext][helpstring][helpcontext][id] */ HRESULT ( STDMETHODCALLTYPE *AddParameter )( 
            CodeProperty2 * This,
            __RPC__in BSTR Name,
            VARIANT Type,
            /* [optional] */ VARIANT Position,
            /* [retval][out] */ __RPC__deref_out_opt /* external definition not present */ CodeParameter **ppCodeParameter);
        
        /* [helpstring][propget][id] */ HRESULT ( STDMETHODCALLTYPE *get_IsGeneric )( 
            CodeProperty2 * This,
            /* [retval][out] */ __RPC__out VARIANT_BOOL *pIsGeneric);
        
        /* [helpstringcontext][helpstring][helpcontext][propget][id] */ HRESULT ( STDMETHODCALLTYPE *get_OverrideKind )( 
            CodeProperty2 * This,
            /* [retval][out] */ __RPC__out vsCMOverrideKind *Kind);
        
        /* [helpstringcontext][helpstring][helpcontext][propput][id] */ HRESULT ( STDMETHODCALLTYPE *put_OverrideKind )( 
            CodeProperty2 * This,
            vsCMOverrideKind Kind);
        
        /* [helpstringcontext][helpstring][helpcontext][propget][id] */ HRESULT ( STDMETHODCALLTYPE *get_IsShared )( 
            CodeProperty2 * This,
            /* [retval][out] */ __RPC__out VARIANT_BOOL *pIsShared);
        
        /* [helpstringcontext][helpstring][helpcontext][propput][id] */ HRESULT ( STDMETHODCALLTYPE *put_IsShared )( 
            CodeProperty2 * This,
            VARIANT_BOOL Shared);
        
        /* [helpstring][propget][id] */ HRESULT ( STDMETHODCALLTYPE *get_IsDefault )( 
            CodeProperty2 * This,
            /* [retval][out] */ __RPC__out VARIANT_BOOL *pIsDefault);
        
        /* [helpstring][propput][id] */ HRESULT ( STDMETHODCALLTYPE *put_IsDefault )( 
            CodeProperty2 * This,
            VARIANT_BOOL Default);
        
        /* [helpstringcontext][helpstring][helpcontext][propget][id] */ HRESULT ( STDMETHODCALLTYPE *get_Parent2 )( 
            CodeProperty2 * This,
            /* [retval][out] */ __RPC__deref_out_opt /* external definition not present */ CodeElement **pParent);
        
        /* [helpstringcontext][helpstring][helpcontext][id] */ HRESULT ( STDMETHODCALLTYPE *RemoveParameter )( 
            CodeProperty2 * This,
            VARIANT Element);
        
        /* [helpstring][propget][id] */ HRESULT ( STDMETHODCALLTYPE *get_ReadWrite )( 
            CodeProperty2 * This,
            /* [retval][out] */ __RPC__out vsCMPropertyKind *pPropertyKind);
        
        END_INTERFACE
    } CodeProperty2Vtbl;

    interface CodeProperty2
    {
        CONST_VTBL struct CodeProperty2Vtbl *lpVtbl;
    };

    

#ifdef COBJMACROS


#define CodeProperty2_QueryInterface(This,riid,ppvObj,retval)	\
    ( (This)->lpVtbl -> QueryInterface(This,riid,ppvObj,retval) ) 

#define CodeProperty2_AddRef(This,retval)	\
    ( (This)->lpVtbl -> AddRef(This,retval) ) 

#define CodeProperty2_Release(This,retval)	\
    ( (This)->lpVtbl -> Release(This,retval) ) 

#define CodeProperty2_GetTypeInfoCount(This,pctinfo,retval)	\
    ( (This)->lpVtbl -> GetTypeInfoCount(This,pctinfo,retval) ) 

#define CodeProperty2_GetTypeInfo(This,itinfo,lcid,pptinfo,retval)	\
    ( (This)->lpVtbl -> GetTypeInfo(This,itinfo,lcid,pptinfo,retval) ) 

#define CodeProperty2_GetIDsOfNames(This,riid,rgszNames,cNames,lcid,rgdispid,retval)	\
    ( (This)->lpVtbl -> GetIDsOfNames(This,riid,rgszNames,cNames,lcid,rgdispid,retval) ) 

#define CodeProperty2_Invoke(This,dispidMember,riid,lcid,wFlags,pdispparams,pvarResult,pexcepinfo,puArgErr,retval)	\
    ( (This)->lpVtbl -> Invoke(This,dispidMember,riid,lcid,wFlags,pdispparams,pvarResult,pexcepinfo,puArgErr,retval) ) 

#define CodeProperty2_get_DTE(This,retval)	\
    ( (This)->lpVtbl -> get_DTE(This,retval) ) 

#define CodeProperty2_get_Collection(This,retval)	\
    ( (This)->lpVtbl -> get_Collection(This,retval) ) 

#define CodeProperty2_get_Name(This,retval)	\
    ( (This)->lpVtbl -> get_Name(This,retval) ) 

#define CodeProperty2_put_Name(This,noname,retval)	\
    ( (This)->lpVtbl -> put_Name(This,noname,retval) ) 

#define CodeProperty2_get_FullName(This,retval)	\
    ( (This)->lpVtbl -> get_FullName(This,retval) ) 

#define CodeProperty2_get_ProjectItem(This,retval)	\
    ( (This)->lpVtbl -> get_ProjectItem(This,retval) ) 

#define CodeProperty2_get_Kind(This,retval)	\
    ( (This)->lpVtbl -> get_Kind(This,retval) ) 

#define CodeProperty2_get_IsCodeType(This,retval)	\
    ( (This)->lpVtbl -> get_IsCodeType(This,retval) ) 

#define CodeProperty2_get_InfoLocation(This,retval)	\
    ( (This)->lpVtbl -> get_InfoLocation(This,retval) ) 

#define CodeProperty2_get_Children(This,retval)	\
    ( (This)->lpVtbl -> get_Children(This,retval) ) 

#define CodeProperty2_get_Language(This,retval)	\
    ( (This)->lpVtbl -> get_Language(This,retval) ) 

#define CodeProperty2_get_StartPoint(This,retval)	\
    ( (This)->lpVtbl -> get_StartPoint(This,retval) ) 

#define CodeProperty2_get_EndPoint(This,retval)	\
    ( (This)->lpVtbl -> get_EndPoint(This,retval) ) 

#define CodeProperty2_get_ExtenderNames(This,retval)	\
    ( (This)->lpVtbl -> get_ExtenderNames(This,retval) ) 

#define CodeProperty2_get_Extender(This,ExtenderName,retval)	\
    ( (This)->lpVtbl -> get_Extender(This,ExtenderName,retval) ) 

#define CodeProperty2_get_ExtenderCATID(This,retval)	\
    ( (This)->lpVtbl -> get_ExtenderCATID(This,retval) ) 

#define CodeProperty2_GetStartPoint(This,Part,retval)	\
    ( (This)->lpVtbl -> GetStartPoint(This,Part,retval) ) 

#define CodeProperty2_GetEndPoint(This,Part,retval)	\
    ( (This)->lpVtbl -> GetEndPoint(This,Part,retval) ) 

#define CodeProperty2_get_Parent(This,retval)	\
    ( (This)->lpVtbl -> get_Parent(This,retval) ) 

#define CodeProperty2_get_Prototype(This,Flags,retval)	\
    ( (This)->lpVtbl -> get_Prototype(This,Flags,retval) ) 

#define CodeProperty2_put_Type(This,noname,retval)	\
    ( (This)->lpVtbl -> put_Type(This,noname,retval) ) 

#define CodeProperty2_get_Type(This,retval)	\
    ( (This)->lpVtbl -> get_Type(This,retval) ) 

#define CodeProperty2_get_Getter(This,retval)	\
    ( (This)->lpVtbl -> get_Getter(This,retval) ) 

#define CodeProperty2_put_Getter(This,noname,retval)	\
    ( (This)->lpVtbl -> put_Getter(This,noname,retval) ) 

#define CodeProperty2_get_Setter(This,retval)	\
    ( (This)->lpVtbl -> get_Setter(This,retval) ) 

#define CodeProperty2_put_Setter(This,noname,retval)	\
    ( (This)->lpVtbl -> put_Setter(This,noname,retval) ) 

#define CodeProperty2_put_Access(This,noname,retval)	\
    ( (This)->lpVtbl -> put_Access(This,noname,retval) ) 

#define CodeProperty2_get_Access(This,retval)	\
    ( (This)->lpVtbl -> get_Access(This,retval) ) 

#define CodeProperty2_get_Attributes(This,retval)	\
    ( (This)->lpVtbl -> get_Attributes(This,retval) ) 

#define CodeProperty2_get_DocComment(This,retval)	\
    ( (This)->lpVtbl -> get_DocComment(This,retval) ) 

#define CodeProperty2_put_DocComment(This,noname,retval)	\
    ( (This)->lpVtbl -> put_DocComment(This,noname,retval) ) 

#define CodeProperty2_get_Comment(This,retval)	\
    ( (This)->lpVtbl -> get_Comment(This,retval) ) 

#define CodeProperty2_put_Comment(This,noname,retval)	\
    ( (This)->lpVtbl -> put_Comment(This,noname,retval) ) 

#define CodeProperty2_AddAttribute(This,Name,Value,Position,retval)	\
    ( (This)->lpVtbl -> AddAttribute(This,Name,Value,Position,retval) ) 


#define CodeProperty2_get_Parameters(This,ppMembers)	\
    ( (This)->lpVtbl -> get_Parameters(This,ppMembers) ) 

#define CodeProperty2_AddParameter(This,Name,Type,Position,ppCodeParameter)	\
    ( (This)->lpVtbl -> AddParameter(This,Name,Type,Position,ppCodeParameter) ) 

#define CodeProperty2_get_IsGeneric(This,pIsGeneric)	\
    ( (This)->lpVtbl -> get_IsGeneric(This,pIsGeneric) ) 

#define CodeProperty2_get_OverrideKind(This,Kind)	\
    ( (This)->lpVtbl -> get_OverrideKind(This,Kind) ) 

#define CodeProperty2_put_OverrideKind(This,Kind)	\
    ( (This)->lpVtbl -> put_OverrideKind(This,Kind) ) 

#define CodeProperty2_get_IsShared(This,pIsShared)	\
    ( (This)->lpVtbl -> get_IsShared(This,pIsShared) ) 

#define CodeProperty2_put_IsShared(This,Shared)	\
    ( (This)->lpVtbl -> put_IsShared(This,Shared) ) 

#define CodeProperty2_get_IsDefault(This,pIsDefault)	\
    ( (This)->lpVtbl -> get_IsDefault(This,pIsDefault) ) 

#define CodeProperty2_put_IsDefault(This,Default)	\
    ( (This)->lpVtbl -> put_IsDefault(This,Default) ) 

#define CodeProperty2_get_Parent2(This,pParent)	\
    ( (This)->lpVtbl -> get_Parent2(This,pParent) ) 

#define CodeProperty2_RemoveParameter(This,Element)	\
    ( (This)->lpVtbl -> RemoveParameter(This,Element) ) 

#define CodeProperty2_get_ReadWrite(This,pPropertyKind)	\
    ( (This)->lpVtbl -> get_ReadWrite(This,pPropertyKind) ) 

#endif /* COBJMACROS */


#endif 	/* C style interface */




#endif 	/* __CodeProperty2_INTERFACE_DEFINED__ */


#ifndef ___dispCodeModelEvents_DISPINTERFACE_DEFINED__
#define ___dispCodeModelEvents_DISPINTERFACE_DEFINED__

/* dispinterface _dispCodeModelEvents */
/* [helpstringcontext][helpstring][helpcontext][uuid] */ 


EXTERN_C const IID DIID__dispCodeModelEvents;

#if defined(__cplusplus) && !defined(CINTERFACE)

    MIDL_INTERFACE("4EA8090E-D289-4D56-98CD-C48DD2853B2E")
    _dispCodeModelEvents : public IDispatch
    {
    };
    
#else 	/* C style interface */

    typedef struct _dispCodeModelEventsVtbl
    {
        BEGIN_INTERFACE
        
        HRESULT ( STDMETHODCALLTYPE *QueryInterface )( 
            _dispCodeModelEvents * This,
            /* [in] */ __RPC__in REFIID riid,
            /* [iid_is][out] */ 
            __RPC__deref_out  void **ppvObject);
        
        ULONG ( STDMETHODCALLTYPE *AddRef )( 
            _dispCodeModelEvents * This);
        
        ULONG ( STDMETHODCALLTYPE *Release )( 
            _dispCodeModelEvents * This);
        
        HRESULT ( STDMETHODCALLTYPE *GetTypeInfoCount )( 
            _dispCodeModelEvents * This,
            /* [out] */ __RPC__out UINT *pctinfo);
        
        HRESULT ( STDMETHODCALLTYPE *GetTypeInfo )( 
            _dispCodeModelEvents * This,
            /* [in] */ UINT iTInfo,
            /* [in] */ LCID lcid,
            /* [out] */ __RPC__deref_out_opt ITypeInfo **ppTInfo);
        
        HRESULT ( STDMETHODCALLTYPE *GetIDsOfNames )( 
            _dispCodeModelEvents * This,
            /* [in] */ __RPC__in REFIID riid,
            /* [size_is][in] */ __RPC__in_ecount_full(cNames) LPOLESTR *rgszNames,
            /* [range][in] */ UINT cNames,
            /* [in] */ LCID lcid,
            /* [size_is][out] */ __RPC__out_ecount_full(cNames) DISPID *rgDispId);
        
        /* [local] */ HRESULT ( STDMETHODCALLTYPE *Invoke )( 
            _dispCodeModelEvents * This,
            /* [in] */ DISPID dispIdMember,
            /* [in] */ REFIID riid,
            /* [in] */ LCID lcid,
            /* [in] */ WORD wFlags,
            /* [out][in] */ DISPPARAMS *pDispParams,
            /* [out] */ VARIANT *pVarResult,
            /* [out] */ EXCEPINFO *pExcepInfo,
            /* [out] */ UINT *puArgErr);
        
        END_INTERFACE
    } _dispCodeModelEventsVtbl;

    interface _dispCodeModelEvents
    {
        CONST_VTBL struct _dispCodeModelEventsVtbl *lpVtbl;
    };

    

#ifdef COBJMACROS


#define _dispCodeModelEvents_QueryInterface(This,riid,ppvObject)	\
    ( (This)->lpVtbl -> QueryInterface(This,riid,ppvObject) ) 

#define _dispCodeModelEvents_AddRef(This)	\
    ( (This)->lpVtbl -> AddRef(This) ) 

#define _dispCodeModelEvents_Release(This)	\
    ( (This)->lpVtbl -> Release(This) ) 


#define _dispCodeModelEvents_GetTypeInfoCount(This,pctinfo)	\
    ( (This)->lpVtbl -> GetTypeInfoCount(This,pctinfo) ) 

#define _dispCodeModelEvents_GetTypeInfo(This,iTInfo,lcid,ppTInfo)	\
    ( (This)->lpVtbl -> GetTypeInfo(This,iTInfo,lcid,ppTInfo) ) 

#define _dispCodeModelEvents_GetIDsOfNames(This,riid,rgszNames,cNames,lcid,rgDispId)	\
    ( (This)->lpVtbl -> GetIDsOfNames(This,riid,rgszNames,cNames,lcid,rgDispId) ) 

#define _dispCodeModelEvents_Invoke(This,dispIdMember,riid,lcid,wFlags,pDispParams,pVarResult,pExcepInfo,puArgErr)	\
    ( (This)->lpVtbl -> Invoke(This,dispIdMember,riid,lcid,wFlags,pDispParams,pVarResult,pExcepInfo,puArgErr) ) 

#endif /* COBJMACROS */


#endif 	/* C style interface */


#endif 	/* ___dispCodeModelEvents_DISPINTERFACE_DEFINED__ */


EXTERN_C const CLSID CLSID_CodeModelEvents;

#ifdef __cplusplus

class DECLSPEC_UUID("B2FA9979-35EB-4CA2-B467-7370152E53B7")
CodeModelEvents;
#endif

#ifndef ___CodeModelEventsRoot_INTERFACE_DEFINED__
#define ___CodeModelEventsRoot_INTERFACE_DEFINED__

/* interface _CodeModelEventsRoot */
/* [hidden][object][dual][uuid] */ 


EXTERN_C const IID IID__CodeModelEventsRoot;

#if defined(__cplusplus) && !defined(CINTERFACE)
    
    MIDL_INTERFACE("c6e28776-b7ff-421f-8c18-57f82e447704")
    _CodeModelEventsRoot : public IDispatch
    {
    public:
        virtual /* [hidden][propget][id] */ HRESULT __stdcall get_CodeModelEvents( 
            /* [in] */ __RPC__in /* external definition not present */ CodeElement *CodeElementFilter,
            /* [retval][out] */ __RPC__deref_out_opt IDispatch **ppdisp) = 0;
        
    };
    
#else 	/* C style interface */

    typedef struct _CodeModelEventsRootVtbl
    {
        BEGIN_INTERFACE
        
        HRESULT ( STDMETHODCALLTYPE *QueryInterface )( 
            _CodeModelEventsRoot * This,
            /* [in] */ __RPC__in REFIID riid,
            /* [iid_is][out] */ 
            __RPC__deref_out  void **ppvObject);
        
        ULONG ( STDMETHODCALLTYPE *AddRef )( 
            _CodeModelEventsRoot * This);
        
        ULONG ( STDMETHODCALLTYPE *Release )( 
            _CodeModelEventsRoot * This);
        
        HRESULT ( STDMETHODCALLTYPE *GetTypeInfoCount )( 
            _CodeModelEventsRoot * This,
            /* [out] */ __RPC__out UINT *pctinfo);
        
        HRESULT ( STDMETHODCALLTYPE *GetTypeInfo )( 
            _CodeModelEventsRoot * This,
            /* [in] */ UINT iTInfo,
            /* [in] */ LCID lcid,
            /* [out] */ __RPC__deref_out_opt ITypeInfo **ppTInfo);
        
        HRESULT ( STDMETHODCALLTYPE *GetIDsOfNames )( 
            _CodeModelEventsRoot * This,
            /* [in] */ __RPC__in REFIID riid,
            /* [size_is][in] */ __RPC__in_ecount_full(cNames) LPOLESTR *rgszNames,
            /* [range][in] */ UINT cNames,
            /* [in] */ LCID lcid,
            /* [size_is][out] */ __RPC__out_ecount_full(cNames) DISPID *rgDispId);
        
        /* [local] */ HRESULT ( STDMETHODCALLTYPE *Invoke )( 
            _CodeModelEventsRoot * This,
            /* [in] */ DISPID dispIdMember,
            /* [in] */ REFIID riid,
            /* [in] */ LCID lcid,
            /* [in] */ WORD wFlags,
            /* [out][in] */ DISPPARAMS *pDispParams,
            /* [out] */ VARIANT *pVarResult,
            /* [out] */ EXCEPINFO *pExcepInfo,
            /* [out] */ UINT *puArgErr);
        
        /* [hidden][propget][id] */ HRESULT ( __stdcall *get_CodeModelEvents )( 
            _CodeModelEventsRoot * This,
            /* [in] */ __RPC__in /* external definition not present */ CodeElement *CodeElementFilter,
            /* [retval][out] */ __RPC__deref_out_opt IDispatch **ppdisp);
        
        END_INTERFACE
    } _CodeModelEventsRootVtbl;

    interface _CodeModelEventsRoot
    {
        CONST_VTBL struct _CodeModelEventsRootVtbl *lpVtbl;
    };

    

#ifdef COBJMACROS


#define _CodeModelEventsRoot_QueryInterface(This,riid,ppvObject)	\
    ( (This)->lpVtbl -> QueryInterface(This,riid,ppvObject) ) 

#define _CodeModelEventsRoot_AddRef(This)	\
    ( (This)->lpVtbl -> AddRef(This) ) 

#define _CodeModelEventsRoot_Release(This)	\
    ( (This)->lpVtbl -> Release(This) ) 


#define _CodeModelEventsRoot_GetTypeInfoCount(This,pctinfo)	\
    ( (This)->lpVtbl -> GetTypeInfoCount(This,pctinfo) ) 

#define _CodeModelEventsRoot_GetTypeInfo(This,iTInfo,lcid,ppTInfo)	\
    ( (This)->lpVtbl -> GetTypeInfo(This,iTInfo,lcid,ppTInfo) ) 

#define _CodeModelEventsRoot_GetIDsOfNames(This,riid,rgszNames,cNames,lcid,rgDispId)	\
    ( (This)->lpVtbl -> GetIDsOfNames(This,riid,rgszNames,cNames,lcid,rgDispId) ) 

#define _CodeModelEventsRoot_Invoke(This,dispIdMember,riid,lcid,wFlags,pDispParams,pVarResult,pExcepInfo,puArgErr)	\
    ( (This)->lpVtbl -> Invoke(This,dispIdMember,riid,lcid,wFlags,pDispParams,pVarResult,pExcepInfo,puArgErr) ) 


#define _CodeModelEventsRoot_get_CodeModelEvents(This,CodeElementFilter,ppdisp)	\
    ( (This)->lpVtbl -> get_CodeModelEvents(This,CodeElementFilter,ppdisp) ) 

#endif /* COBJMACROS */


#endif 	/* C style interface */




#endif 	/* ___CodeModelEventsRoot_INTERFACE_DEFINED__ */


#ifndef ___CodeModelEvents_INTERFACE_DEFINED__
#define ___CodeModelEvents_INTERFACE_DEFINED__

/* interface _CodeModelEvents */
/* [object][oleautomation][uuid] */ 


EXTERN_C const IID IID__CodeModelEvents;

#if defined(__cplusplus) && !defined(CINTERFACE)
    
    MIDL_INTERFACE("66adc510-0ca2-475d-a343-57192bce38bf")
    _CodeModelEvents : public IDispatch
    {
    public:
    };
    
#else 	/* C style interface */

    typedef struct _CodeModelEventsVtbl
    {
        BEGIN_INTERFACE
        
        HRESULT ( STDMETHODCALLTYPE *QueryInterface )( 
            _CodeModelEvents * This,
            /* [in] */ __RPC__in REFIID riid,
            /* [iid_is][out] */ 
            __RPC__deref_out  void **ppvObject);
        
        ULONG ( STDMETHODCALLTYPE *AddRef )( 
            _CodeModelEvents * This);
        
        ULONG ( STDMETHODCALLTYPE *Release )( 
            _CodeModelEvents * This);
        
        HRESULT ( STDMETHODCALLTYPE *GetTypeInfoCount )( 
            _CodeModelEvents * This,
            /* [out] */ __RPC__out UINT *pctinfo);
        
        HRESULT ( STDMETHODCALLTYPE *GetTypeInfo )( 
            _CodeModelEvents * This,
            /* [in] */ UINT iTInfo,
            /* [in] */ LCID lcid,
            /* [out] */ __RPC__deref_out_opt ITypeInfo **ppTInfo);
        
        HRESULT ( STDMETHODCALLTYPE *GetIDsOfNames )( 
            _CodeModelEvents * This,
            /* [in] */ __RPC__in REFIID riid,
            /* [size_is][in] */ __RPC__in_ecount_full(cNames) LPOLESTR *rgszNames,
            /* [range][in] */ UINT cNames,
            /* [in] */ LCID lcid,
            /* [size_is][out] */ __RPC__out_ecount_full(cNames) DISPID *rgDispId);
        
        /* [local] */ HRESULT ( STDMETHODCALLTYPE *Invoke )( 
            _CodeModelEvents * This,
            /* [in] */ DISPID dispIdMember,
            /* [in] */ REFIID riid,
            /* [in] */ LCID lcid,
            /* [in] */ WORD wFlags,
            /* [out][in] */ DISPPARAMS *pDispParams,
            /* [out] */ VARIANT *pVarResult,
            /* [out] */ EXCEPINFO *pExcepInfo,
            /* [out] */ UINT *puArgErr);
        
        END_INTERFACE
    } _CodeModelEventsVtbl;

    interface _CodeModelEvents
    {
        CONST_VTBL struct _CodeModelEventsVtbl *lpVtbl;
    };

    

#ifdef COBJMACROS


#define _CodeModelEvents_QueryInterface(This,riid,ppvObject)	\
    ( (This)->lpVtbl -> QueryInterface(This,riid,ppvObject) ) 

#define _CodeModelEvents_AddRef(This)	\
    ( (This)->lpVtbl -> AddRef(This) ) 

#define _CodeModelEvents_Release(This)	\
    ( (This)->lpVtbl -> Release(This) ) 


#define _CodeModelEvents_GetTypeInfoCount(This,pctinfo)	\
    ( (This)->lpVtbl -> GetTypeInfoCount(This,pctinfo) ) 

#define _CodeModelEvents_GetTypeInfo(This,iTInfo,lcid,ppTInfo)	\
    ( (This)->lpVtbl -> GetTypeInfo(This,iTInfo,lcid,ppTInfo) ) 

#define _CodeModelEvents_GetIDsOfNames(This,riid,rgszNames,cNames,lcid,rgDispId)	\
    ( (This)->lpVtbl -> GetIDsOfNames(This,riid,rgszNames,cNames,lcid,rgDispId) ) 

#define _CodeModelEvents_Invoke(This,dispIdMember,riid,lcid,wFlags,pDispParams,pVarResult,pExcepInfo,puArgErr)	\
    ( (This)->lpVtbl -> Invoke(This,dispIdMember,riid,lcid,wFlags,pDispParams,pVarResult,pExcepInfo,puArgErr) ) 


#endif /* COBJMACROS */


#endif 	/* C style interface */




#endif 	/* ___CodeModelEvents_INTERFACE_DEFINED__ */


#ifndef __Debugger2_INTERFACE_DEFINED__
#define __Debugger2_INTERFACE_DEFINED__

/* interface Debugger2 */
/* [object][version][helpstringcontext][helpstring][helpcontext][dual][uuid] */ 


EXTERN_C const IID IID_Debugger2;

#if defined(__cplusplus) && !defined(CINTERFACE)
    
    MIDL_INTERFACE("8B5E2BFD-4642-4efe-8AF4-0B2DA9AAA23C")
    Debugger2 : public Debugger
    {
    public:
        virtual /* [helpstringcontext][helpstring][helpcontext][id] */ HRESULT STDMETHODCALLTYPE WriteMinidump( 
            /* [in] */ __RPC__in BSTR FileName,
            /* [in] */ dbgMinidumpOption Option) = 0;
        
        virtual /* [helpstringcontext][helpstring][helpcontext][id] */ HRESULT STDMETHODCALLTYPE GetProcesses( 
            /* [in] */ __RPC__in_opt Transport *pTransport,
            /* [in] */ __RPC__in BSTR TransportQualifier,
            /* [retval][out] */ __RPC__deref_out_opt /* external definition not present */ Processes **Processes) = 0;
        
        virtual /* [helpstringcontext][helpstring][helpcontext][id] */ HRESULT STDMETHODCALLTYPE GetExpression2( 
            /* [in] */ __RPC__in BSTR ExpressionText,
            /* [defaultvalue][optional][in] */ VARIANT_BOOL UseAutoExpandRules,
            /* [defaultvalue][optional][in] */ VARIANT_BOOL TreatAsStatement,
            /* [defaultvalue][in] */ long Timeout,
            /* [retval][out] */ __RPC__deref_out_opt /* external definition not present */ Expression **Expression) = 0;
        
        virtual /* [helpstringcontext][helpstring][helpcontext][propget][id] */ HRESULT STDMETHODCALLTYPE get_Transports( 
            /* [retval][out] */ __RPC__deref_out_opt Transports **Transports) = 0;
        
    };
    
#else 	/* C style interface */

    typedef struct Debugger2Vtbl
    {
        BEGIN_INTERFACE
        
        /* [id][restricted][funcdescattr] */ HRESULT ( STDMETHODCALLTYPE *QueryInterface )( 
            Debugger2 * This,
            /* [in][idldescattr] */ __RPC__in struct GUID *riid,
            /* [out][idldescattr] */ __RPC__deref_out_opt void **ppvObj,
            /* [retval][out] */ __RPC__out void *retval);
        
        /* [id][restricted][funcdescattr] */ HRESULT ( STDMETHODCALLTYPE *AddRef )( 
            Debugger2 * This,
            /* [retval][out] */ __RPC__out unsigned long *retval);
        
        /* [id][restricted][funcdescattr] */ HRESULT ( STDMETHODCALLTYPE *Release )( 
            Debugger2 * This,
            /* [retval][out] */ __RPC__out unsigned long *retval);
        
        /* [id][restricted][funcdescattr] */ HRESULT ( STDMETHODCALLTYPE *GetTypeInfoCount )( 
            Debugger2 * This,
            /* [out][idldescattr] */ __RPC__out unsigned UINT *pctinfo,
            /* [retval][out] */ __RPC__out void *retval);
        
        /* [id][restricted][funcdescattr] */ HRESULT ( STDMETHODCALLTYPE *GetTypeInfo )( 
            Debugger2 * This,
            /* [in][idldescattr] */ unsigned UINT itinfo,
            /* [in][idldescattr] */ unsigned long lcid,
            /* [out][idldescattr] */ __RPC__deref_out_opt void **pptinfo,
            /* [retval][out] */ __RPC__out void *retval);
        
        /* [id][restricted][funcdescattr] */ HRESULT ( STDMETHODCALLTYPE *GetIDsOfNames )( 
            Debugger2 * This,
            /* [in][idldescattr] */ __RPC__in struct GUID *riid,
            /* [in][idldescattr] */ __RPC__deref_in_opt signed char **rgszNames,
            /* [in][idldescattr] */ unsigned UINT cNames,
            /* [in][idldescattr] */ unsigned long lcid,
            /* [out][idldescattr] */ __RPC__out signed long *rgdispid,
            /* [retval][out] */ __RPC__out void *retval);
        
        /* [id][restricted][funcdescattr] */ HRESULT ( STDMETHODCALLTYPE *Invoke )( 
            Debugger2 * This,
            /* [in][idldescattr] */ signed long dispidMember,
            /* [in][idldescattr] */ __RPC__in struct GUID *riid,
            /* [in][idldescattr] */ unsigned long lcid,
            /* [in][idldescattr] */ unsigned short wFlags,
            /* [in][idldescattr] */ __RPC__in struct DISPPARAMS *pdispparams,
            /* [out][idldescattr] */ __RPC__out VARIANT *pvarResult,
            /* [out][idldescattr] */ __RPC__out struct EXCEPINFO *pexcepinfo,
            /* [out][idldescattr] */ __RPC__out unsigned UINT *puArgErr,
            /* [retval][out] */ __RPC__out void *retval);
        
        /* [id][funcdescattr] */ HRESULT ( STDMETHODCALLTYPE *GetExpression )( 
            Debugger2 * This,
            /* [in][idldescattr] */ __RPC__in BSTR ExpressionText,
            /* [in][idldescattr] */ BOOLEAN UseAutoExpandRules,
            /* [in][idldescattr] */ signed long Timeout,
            /* [retval][out] */ __RPC__deref_out_opt Expression **retval);
        
        /* [id][funcdescattr] */ HRESULT ( STDMETHODCALLTYPE *DetachAll )( 
            Debugger2 * This,
            /* [retval][out] */ __RPC__out void *retval);
        
        /* [id][funcdescattr] */ HRESULT ( STDMETHODCALLTYPE *StepInto )( 
            Debugger2 * This,
            /* [in][idldescattr] */ BOOLEAN WaitForBreakOrEnd,
            /* [retval][out] */ __RPC__out void *retval);
        
        /* [id][funcdescattr] */ HRESULT ( STDMETHODCALLTYPE *StepOver )( 
            Debugger2 * This,
            /* [in][idldescattr] */ BOOLEAN WaitForBreakOrEnd,
            /* [retval][out] */ __RPC__out void *retval);
        
        /* [id][funcdescattr] */ HRESULT ( STDMETHODCALLTYPE *StepOut )( 
            Debugger2 * This,
            /* [in][idldescattr] */ BOOLEAN WaitForBreakOrEnd,
            /* [retval][out] */ __RPC__out void *retval);
        
        /* [id][funcdescattr] */ HRESULT ( STDMETHODCALLTYPE *Go )( 
            Debugger2 * This,
            /* [in][idldescattr] */ BOOLEAN WaitForBreakOrEnd,
            /* [retval][out] */ __RPC__out void *retval);
        
        /* [id][funcdescattr] */ HRESULT ( STDMETHODCALLTYPE *Break )( 
            Debugger2 * This,
            /* [in][idldescattr] */ BOOLEAN WaitForBreakMode,
            /* [retval][out] */ __RPC__out void *retval);
        
        /* [id][funcdescattr] */ HRESULT ( STDMETHODCALLTYPE *Stop )( 
            Debugger2 * This,
            /* [in][idldescattr] */ BOOLEAN WaitForDesignMode,
            /* [retval][out] */ __RPC__out void *retval);
        
        /* [id][funcdescattr] */ HRESULT ( STDMETHODCALLTYPE *SetNextStatement )( 
            Debugger2 * This,
            /* [retval][out] */ __RPC__out void *retval);
        
        /* [id][funcdescattr] */ HRESULT ( STDMETHODCALLTYPE *RunToCursor )( 
            Debugger2 * This,
            /* [in][idldescattr] */ BOOLEAN WaitForBreakOrEnd,
            /* [retval][out] */ __RPC__out void *retval);
        
        /* [id][funcdescattr] */ HRESULT ( STDMETHODCALLTYPE *ExecuteStatement )( 
            Debugger2 * This,
            /* [in][idldescattr] */ __RPC__in BSTR Statement,
            /* [in][idldescattr] */ signed long Timeout,
            /* [in][idldescattr] */ BOOLEAN TreatAsExpression,
            /* [retval][out] */ __RPC__out void *retval);
        
        /* [id][propget][funcdescattr] */ HRESULT ( STDMETHODCALLTYPE *get_Breakpoints )( 
            Debugger2 * This,
            /* [retval][out] */ __RPC__deref_out_opt Breakpoints **retval);
        
        /* [id][propget][funcdescattr] */ HRESULT ( STDMETHODCALLTYPE *get_Languages )( 
            Debugger2 * This,
            /* [retval][out] */ __RPC__deref_out_opt Languages **retval);
        
        /* [id][propget][funcdescattr] */ HRESULT ( STDMETHODCALLTYPE *get_CurrentMode )( 
            Debugger2 * This,
            /* [retval][out] */ __RPC__out enum dbgDebugMode *retval);
        
        /* [id][propget][funcdescattr] */ HRESULT ( STDMETHODCALLTYPE *get_CurrentProcess )( 
            Debugger2 * This,
            /* [retval][out] */ __RPC__deref_out_opt Process **retval);
        
        /* [id][propput][funcdescattr] */ HRESULT ( STDMETHODCALLTYPE *put_CurrentProcess )( 
            Debugger2 * This,
            /* [in][idldescattr] */ __RPC__in_opt Process *noname,
            /* [retval][out] */ __RPC__out void *retval);
        
        /* [id][propget][funcdescattr] */ HRESULT ( STDMETHODCALLTYPE *get_CurrentProgram )( 
            Debugger2 * This,
            /* [retval][out] */ __RPC__deref_out_opt Program **retval);
        
        /* [id][propput][funcdescattr] */ HRESULT ( STDMETHODCALLTYPE *put_CurrentProgram )( 
            Debugger2 * This,
            /* [in][idldescattr] */ __RPC__in_opt Program *noname,
            /* [retval][out] */ __RPC__out void *retval);
        
        /* [id][propget][funcdescattr] */ HRESULT ( STDMETHODCALLTYPE *get_CurrentThread )( 
            Debugger2 * This,
            /* [retval][out] */ __RPC__deref_out_opt Thread **retval);
        
        /* [id][propput][funcdescattr] */ HRESULT ( STDMETHODCALLTYPE *put_CurrentThread )( 
            Debugger2 * This,
            /* [in][idldescattr] */ __RPC__in_opt Thread *noname,
            /* [retval][out] */ __RPC__out void *retval);
        
        /* [id][propget][funcdescattr] */ HRESULT ( STDMETHODCALLTYPE *get_CurrentStackFrame )( 
            Debugger2 * This,
            /* [retval][out] */ __RPC__deref_out_opt StackFrame **retval);
        
        /* [id][propput][funcdescattr] */ HRESULT ( STDMETHODCALLTYPE *put_CurrentStackFrame )( 
            Debugger2 * This,
            /* [in][idldescattr] */ __RPC__in_opt StackFrame *noname,
            /* [retval][out] */ __RPC__out void *retval);
        
        /* [id][propget][funcdescattr] */ HRESULT ( STDMETHODCALLTYPE *get_HexDisplayMode )( 
            Debugger2 * This,
            /* [retval][out] */ __RPC__out BOOLEAN *retval);
        
        /* [id][propput][funcdescattr] */ HRESULT ( STDMETHODCALLTYPE *put_HexDisplayMode )( 
            Debugger2 * This,
            /* [in][idldescattr] */ BOOLEAN noname,
            /* [retval][out] */ __RPC__out void *retval);
        
        /* [id][propget][funcdescattr] */ HRESULT ( STDMETHODCALLTYPE *get_HexInputMode )( 
            Debugger2 * This,
            /* [retval][out] */ __RPC__out BOOLEAN *retval);
        
        /* [id][propput][funcdescattr] */ HRESULT ( STDMETHODCALLTYPE *put_HexInputMode )( 
            Debugger2 * This,
            /* [in][idldescattr] */ BOOLEAN noname,
            /* [retval][out] */ __RPC__out void *retval);
        
        /* [id][propget][funcdescattr] */ HRESULT ( STDMETHODCALLTYPE *get_LastBreakReason )( 
            Debugger2 * This,
            /* [retval][out] */ __RPC__out enum dbgEventReason *retval);
        
        /* [id][propget][funcdescattr] */ HRESULT ( STDMETHODCALLTYPE *get_BreakpointLastHit )( 
            Debugger2 * This,
            /* [retval][out] */ __RPC__deref_out_opt Breakpoint **retval);
        
        /* [id][propget][funcdescattr] */ HRESULT ( STDMETHODCALLTYPE *get_AllBreakpointsLastHit )( 
            Debugger2 * This,
            /* [retval][out] */ __RPC__deref_out_opt Breakpoints **retval);
        
        /* [id][propget][funcdescattr] */ HRESULT ( STDMETHODCALLTYPE *get_DebuggedProcesses )( 
            Debugger2 * This,
            /* [retval][out] */ __RPC__deref_out_opt Processes **retval);
        
        /* [id][propget][funcdescattr] */ HRESULT ( STDMETHODCALLTYPE *get_LocalProcesses )( 
            Debugger2 * This,
            /* [retval][out] */ __RPC__deref_out_opt Processes **retval);
        
        /* [id][propget][funcdescattr] */ HRESULT ( STDMETHODCALLTYPE *get_DTE )( 
            Debugger2 * This,
            /* [retval][out] */ __RPC__deref_out_opt **retval);
        
        /* [id][propget][funcdescattr] */ HRESULT ( STDMETHODCALLTYPE *get_Parent )( 
            Debugger2 * This,
            /* [retval][out] */ __RPC__deref_out_opt **retval);
        
        /* [id][funcdescattr] */ HRESULT ( STDMETHODCALLTYPE *TerminateAll )( 
            Debugger2 * This,
            /* [retval][out] */ __RPC__out void *retval);
        
        /* [helpstringcontext][helpstring][helpcontext][id] */ HRESULT ( STDMETHODCALLTYPE *WriteMinidump )( 
            Debugger2 * This,
            /* [in] */ __RPC__in BSTR FileName,
            /* [in] */ dbgMinidumpOption Option);
        
        /* [helpstringcontext][helpstring][helpcontext][id] */ HRESULT ( STDMETHODCALLTYPE *GetProcesses )( 
            Debugger2 * This,
            /* [in] */ __RPC__in_opt Transport *pTransport,
            /* [in] */ __RPC__in BSTR TransportQualifier,
            /* [retval][out] */ __RPC__deref_out_opt /* external definition not present */ Processes **Processes);
        
        /* [helpstringcontext][helpstring][helpcontext][id] */ HRESULT ( STDMETHODCALLTYPE *GetExpression2 )( 
            Debugger2 * This,
            /* [in] */ __RPC__in BSTR ExpressionText,
            /* [defaultvalue][optional][in] */ VARIANT_BOOL UseAutoExpandRules,
            /* [defaultvalue][optional][in] */ VARIANT_BOOL TreatAsStatement,
            /* [defaultvalue][in] */ long Timeout,
            /* [retval][out] */ __RPC__deref_out_opt /* external definition not present */ Expression **Expression);
        
        /* [helpstringcontext][helpstring][helpcontext][propget][id] */ HRESULT ( STDMETHODCALLTYPE *get_Transports )( 
            Debugger2 * This,
            /* [retval][out] */ __RPC__deref_out_opt Transports **Transports);
        
        END_INTERFACE
    } Debugger2Vtbl;

    interface Debugger2
    {
        CONST_VTBL struct Debugger2Vtbl *lpVtbl;
    };

    

#ifdef COBJMACROS


#define Debugger2_QueryInterface(This,riid,ppvObj,retval)	\
    ( (This)->lpVtbl -> QueryInterface(This,riid,ppvObj,retval) ) 

#define Debugger2_AddRef(This,retval)	\
    ( (This)->lpVtbl -> AddRef(This,retval) ) 

#define Debugger2_Release(This,retval)	\
    ( (This)->lpVtbl -> Release(This,retval) ) 

#define Debugger2_GetTypeInfoCount(This,pctinfo,retval)	\
    ( (This)->lpVtbl -> GetTypeInfoCount(This,pctinfo,retval) ) 

#define Debugger2_GetTypeInfo(This,itinfo,lcid,pptinfo,retval)	\
    ( (This)->lpVtbl -> GetTypeInfo(This,itinfo,lcid,pptinfo,retval) ) 

#define Debugger2_GetIDsOfNames(This,riid,rgszNames,cNames,lcid,rgdispid,retval)	\
    ( (This)->lpVtbl -> GetIDsOfNames(This,riid,rgszNames,cNames,lcid,rgdispid,retval) ) 

#define Debugger2_Invoke(This,dispidMember,riid,lcid,wFlags,pdispparams,pvarResult,pexcepinfo,puArgErr,retval)	\
    ( (This)->lpVtbl -> Invoke(This,dispidMember,riid,lcid,wFlags,pdispparams,pvarResult,pexcepinfo,puArgErr,retval) ) 

#define Debugger2_GetExpression(This,ExpressionText,UseAutoExpandRules,Timeout,retval)	\
    ( (This)->lpVtbl -> GetExpression(This,ExpressionText,UseAutoExpandRules,Timeout,retval) ) 

#define Debugger2_DetachAll(This,retval)	\
    ( (This)->lpVtbl -> DetachAll(This,retval) ) 

#define Debugger2_StepInto(This,WaitForBreakOrEnd,retval)	\
    ( (This)->lpVtbl -> StepInto(This,WaitForBreakOrEnd,retval) ) 

#define Debugger2_StepOver(This,WaitForBreakOrEnd,retval)	\
    ( (This)->lpVtbl -> StepOver(This,WaitForBreakOrEnd,retval) ) 

#define Debugger2_StepOut(This,WaitForBreakOrEnd,retval)	\
    ( (This)->lpVtbl -> StepOut(This,WaitForBreakOrEnd,retval) ) 

#define Debugger2_Go(This,WaitForBreakOrEnd,retval)	\
    ( (This)->lpVtbl -> Go(This,WaitForBreakOrEnd,retval) ) 

#define Debugger2_Break(This,WaitForBreakMode,retval)	\
    ( (This)->lpVtbl -> Break(This,WaitForBreakMode,retval) ) 

#define Debugger2_Stop(This,WaitForDesignMode,retval)	\
    ( (This)->lpVtbl -> Stop(This,WaitForDesignMode,retval) ) 

#define Debugger2_SetNextStatement(This,retval)	\
    ( (This)->lpVtbl -> SetNextStatement(This,retval) ) 

#define Debugger2_RunToCursor(This,WaitForBreakOrEnd,retval)	\
    ( (This)->lpVtbl -> RunToCursor(This,WaitForBreakOrEnd,retval) ) 

#define Debugger2_ExecuteStatement(This,Statement,Timeout,TreatAsExpression,retval)	\
    ( (This)->lpVtbl -> ExecuteStatement(This,Statement,Timeout,TreatAsExpression,retval) ) 

#define Debugger2_get_Breakpoints(This,retval)	\
    ( (This)->lpVtbl -> get_Breakpoints(This,retval) ) 

#define Debugger2_get_Languages(This,retval)	\
    ( (This)->lpVtbl -> get_Languages(This,retval) ) 

#define Debugger2_get_CurrentMode(This,retval)	\
    ( (This)->lpVtbl -> get_CurrentMode(This,retval) ) 

#define Debugger2_get_CurrentProcess(This,retval)	\
    ( (This)->lpVtbl -> get_CurrentProcess(This,retval) ) 

#define Debugger2_put_CurrentProcess(This,noname,retval)	\
    ( (This)->lpVtbl -> put_CurrentProcess(This,noname,retval) ) 

#define Debugger2_get_CurrentProgram(This,retval)	\
    ( (This)->lpVtbl -> get_CurrentProgram(This,retval) ) 

#define Debugger2_put_CurrentProgram(This,noname,retval)	\
    ( (This)->lpVtbl -> put_CurrentProgram(This,noname,retval) ) 

#define Debugger2_get_CurrentThread(This,retval)	\
    ( (This)->lpVtbl -> get_CurrentThread(This,retval) ) 

#define Debugger2_put_CurrentThread(This,noname,retval)	\
    ( (This)->lpVtbl -> put_CurrentThread(This,noname,retval) ) 

#define Debugger2_get_CurrentStackFrame(This,retval)	\
    ( (This)->lpVtbl -> get_CurrentStackFrame(This,retval) ) 

#define Debugger2_put_CurrentStackFrame(This,noname,retval)	\
    ( (This)->lpVtbl -> put_CurrentStackFrame(This,noname,retval) ) 

#define Debugger2_get_HexDisplayMode(This,retval)	\
    ( (This)->lpVtbl -> get_HexDisplayMode(This,retval) ) 

#define Debugger2_put_HexDisplayMode(This,noname,retval)	\
    ( (This)->lpVtbl -> put_HexDisplayMode(This,noname,retval) ) 

#define Debugger2_get_HexInputMode(This,retval)	\
    ( (This)->lpVtbl -> get_HexInputMode(This,retval) ) 

#define Debugger2_put_HexInputMode(This,noname,retval)	\
    ( (This)->lpVtbl -> put_HexInputMode(This,noname,retval) ) 

#define Debugger2_get_LastBreakReason(This,retval)	\
    ( (This)->lpVtbl -> get_LastBreakReason(This,retval) ) 

#define Debugger2_get_BreakpointLastHit(This,retval)	\
    ( (This)->lpVtbl -> get_BreakpointLastHit(This,retval) ) 

#define Debugger2_get_AllBreakpointsLastHit(This,retval)	\
    ( (This)->lpVtbl -> get_AllBreakpointsLastHit(This,retval) ) 

#define Debugger2_get_DebuggedProcesses(This,retval)	\
    ( (This)->lpVtbl -> get_DebuggedProcesses(This,retval) ) 

#define Debugger2_get_LocalProcesses(This,retval)	\
    ( (This)->lpVtbl -> get_LocalProcesses(This,retval) ) 

#define Debugger2_get_DTE(This,retval)	\
    ( (This)->lpVtbl -> get_DTE(This,retval) ) 

#define Debugger2_get_Parent(This,retval)	\
    ( (This)->lpVtbl -> get_Parent(This,retval) ) 

#define Debugger2_TerminateAll(This,retval)	\
    ( (This)->lpVtbl -> TerminateAll(This,retval) ) 


#define Debugger2_WriteMinidump(This,FileName,Option)	\
    ( (This)->lpVtbl -> WriteMinidump(This,FileName,Option) ) 

#define Debugger2_GetProcesses(This,pTransport,TransportQualifier,Processes)	\
    ( (This)->lpVtbl -> GetProcesses(This,pTransport,TransportQualifier,Processes) ) 

#define Debugger2_GetExpression2(This,ExpressionText,UseAutoExpandRules,TreatAsStatement,Timeout,Expression)	\
    ( (This)->lpVtbl -> GetExpression2(This,ExpressionText,UseAutoExpandRules,TreatAsStatement,Timeout,Expression) ) 

#define Debugger2_get_Transports(This,Transports)	\
    ( (This)->lpVtbl -> get_Transports(This,Transports) ) 

#endif /* COBJMACROS */


#endif 	/* C style interface */




#endif 	/* __Debugger2_INTERFACE_DEFINED__ */


#ifndef __Process2_INTERFACE_DEFINED__
#define __Process2_INTERFACE_DEFINED__

/* interface Process2 */
/* [object][version][helpstringcontext][helpstring][helpcontext][dual][uuid] */ 


EXTERN_C const IID IID_Process2;

#if defined(__cplusplus) && !defined(CINTERFACE)
    
    MIDL_INTERFACE("95AC1923-6EAA-427c-B43E-6274A8CA6C95")
    Process2 : public Process
    {
    public:
        virtual /* [helpstringcontext][helpstring][helpcontext][id] */ HRESULT STDMETHODCALLTYPE Attach2( 
            /* [optional][defaultvalue][in] */ VARIANT Engines) = 0;
        
        virtual /* [helpstringcontext][helpstring][helpcontext][propget][id] */ HRESULT STDMETHODCALLTYPE get_Transport( 
            /* [retval][out] */ __RPC__deref_out_opt Transport **Transport) = 0;
        
        virtual /* [helpstringcontext][helpstring][helpcontext][propget][id] */ HRESULT STDMETHODCALLTYPE get_TransportQualifier( 
            /* [retval][out] */ __RPC__deref_out_opt BSTR *TransportQualifier) = 0;
        
        virtual /* [helpstringcontext][helpstring][helpcontext][propget][id] */ HRESULT STDMETHODCALLTYPE get_Threads( 
            /* [retval][out] */ __RPC__deref_out_opt /* external definition not present */ Threads **Threads) = 0;
        
        virtual /* [helpstringcontext][helpstring][helpcontext][propget][id] */ HRESULT STDMETHODCALLTYPE get_IsBeingDebugged( 
            /* [retval][out] */ __RPC__out VARIANT_BOOL *IsBeingDebugged) = 0;
        
        virtual /* [helpstringcontext][helpstring][helpcontext][propget][id] */ HRESULT STDMETHODCALLTYPE get_UserName( 
            /* [retval][out] */ __RPC__deref_out_opt BSTR *UserName) = 0;
        
    };
    
#else 	/* C style interface */

    typedef struct Process2Vtbl
    {
        BEGIN_INTERFACE
        
        /* [id][restricted][funcdescattr] */ HRESULT ( STDMETHODCALLTYPE *QueryInterface )( 
            Process2 * This,
            /* [in][idldescattr] */ __RPC__in struct GUID *riid,
            /* [out][idldescattr] */ __RPC__deref_out_opt void **ppvObj,
            /* [retval][out] */ __RPC__out void *retval);
        
        /* [id][restricted][funcdescattr] */ HRESULT ( STDMETHODCALLTYPE *AddRef )( 
            Process2 * This,
            /* [retval][out] */ __RPC__out unsigned long *retval);
        
        /* [id][restricted][funcdescattr] */ HRESULT ( STDMETHODCALLTYPE *Release )( 
            Process2 * This,
            /* [retval][out] */ __RPC__out unsigned long *retval);
        
        /* [id][restricted][funcdescattr] */ HRESULT ( STDMETHODCALLTYPE *GetTypeInfoCount )( 
            Process2 * This,
            /* [out][idldescattr] */ __RPC__out unsigned UINT *pctinfo,
            /* [retval][out] */ __RPC__out void *retval);
        
        /* [id][restricted][funcdescattr] */ HRESULT ( STDMETHODCALLTYPE *GetTypeInfo )( 
            Process2 * This,
            /* [in][idldescattr] */ unsigned UINT itinfo,
            /* [in][idldescattr] */ unsigned long lcid,
            /* [out][idldescattr] */ __RPC__deref_out_opt void **pptinfo,
            /* [retval][out] */ __RPC__out void *retval);
        
        /* [id][restricted][funcdescattr] */ HRESULT ( STDMETHODCALLTYPE *GetIDsOfNames )( 
            Process2 * This,
            /* [in][idldescattr] */ __RPC__in struct GUID *riid,
            /* [in][idldescattr] */ __RPC__deref_in_opt signed char **rgszNames,
            /* [in][idldescattr] */ unsigned UINT cNames,
            /* [in][idldescattr] */ unsigned long lcid,
            /* [out][idldescattr] */ __RPC__out signed long *rgdispid,
            /* [retval][out] */ __RPC__out void *retval);
        
        /* [id][restricted][funcdescattr] */ HRESULT ( STDMETHODCALLTYPE *Invoke )( 
            Process2 * This,
            /* [in][idldescattr] */ signed long dispidMember,
            /* [in][idldescattr] */ __RPC__in struct GUID *riid,
            /* [in][idldescattr] */ unsigned long lcid,
            /* [in][idldescattr] */ unsigned short wFlags,
            /* [in][idldescattr] */ __RPC__in struct DISPPARAMS *pdispparams,
            /* [out][idldescattr] */ __RPC__out VARIANT *pvarResult,
            /* [out][idldescattr] */ __RPC__out struct EXCEPINFO *pexcepinfo,
            /* [out][idldescattr] */ __RPC__out unsigned UINT *puArgErr,
            /* [retval][out] */ __RPC__out void *retval);
        
        /* [id][funcdescattr] */ HRESULT ( STDMETHODCALLTYPE *Attach )( 
            Process2 * This,
            /* [retval][out] */ __RPC__out void *retval);
        
        /* [id][funcdescattr] */ HRESULT ( STDMETHODCALLTYPE *Detach )( 
            Process2 * This,
            /* [in][idldescattr] */ BOOLEAN WaitForBreakOrEnd,
            /* [retval][out] */ __RPC__out void *retval);
        
        /* [id][funcdescattr] */ HRESULT ( STDMETHODCALLTYPE *Break )( 
            Process2 * This,
            /* [in][idldescattr] */ BOOLEAN WaitForBreakMode,
            /* [retval][out] */ __RPC__out void *retval);
        
        /* [id][funcdescattr] */ HRESULT ( STDMETHODCALLTYPE *Terminate )( 
            Process2 * This,
            /* [in][idldescattr] */ BOOLEAN WaitForBreakOrEnd,
            /* [retval][out] */ __RPC__out void *retval);
        
        /* [id][propget][funcdescattr] */ HRESULT ( STDMETHODCALLTYPE *get_Name )( 
            Process2 * This,
            /* [retval][out] */ __RPC__deref_out_opt BSTR *retval);
        
        /* [id][propget][funcdescattr] */ HRESULT ( STDMETHODCALLTYPE *get_ProcessID )( 
            Process2 * This,
            /* [retval][out] */ __RPC__out signed long *retval);
        
        /* [id][propget][funcdescattr] */ HRESULT ( STDMETHODCALLTYPE *get_Programs )( 
            Process2 * This,
            /* [retval][out] */ __RPC__deref_out_opt Programs **retval);
        
        /* [id][propget][funcdescattr] */ HRESULT ( STDMETHODCALLTYPE *get_DTE )( 
            Process2 * This,
            /* [retval][out] */ __RPC__deref_out_opt **retval);
        
        /* [id][propget][funcdescattr] */ HRESULT ( STDMETHODCALLTYPE *get_Parent )( 
            Process2 * This,
            /* [retval][out] */ __RPC__deref_out_opt Debugger **retval);
        
        /* [id][propget][funcdescattr] */ HRESULT ( STDMETHODCALLTYPE *get_Collection )( 
            Process2 * This,
            /* [retval][out] */ __RPC__deref_out_opt Processes **retval);
        
        /* [helpstringcontext][helpstring][helpcontext][id] */ HRESULT ( STDMETHODCALLTYPE *Attach2 )( 
            Process2 * This,
            /* [optional][defaultvalue][in] */ VARIANT Engines);
        
        /* [helpstringcontext][helpstring][helpcontext][propget][id] */ HRESULT ( STDMETHODCALLTYPE *get_Transport )( 
            Process2 * This,
            /* [retval][out] */ __RPC__deref_out_opt Transport **Transport);
        
        /* [helpstringcontext][helpstring][helpcontext][propget][id] */ HRESULT ( STDMETHODCALLTYPE *get_TransportQualifier )( 
            Process2 * This,
            /* [retval][out] */ __RPC__deref_out_opt BSTR *TransportQualifier);
        
        /* [helpstringcontext][helpstring][helpcontext][propget][id] */ HRESULT ( STDMETHODCALLTYPE *get_Threads )( 
            Process2 * This,
            /* [retval][out] */ __RPC__deref_out_opt /* external definition not present */ Threads **Threads);
        
        /* [helpstringcontext][helpstring][helpcontext][propget][id] */ HRESULT ( STDMETHODCALLTYPE *get_IsBeingDebugged )( 
            Process2 * This,
            /* [retval][out] */ __RPC__out VARIANT_BOOL *IsBeingDebugged);
        
        /* [helpstringcontext][helpstring][helpcontext][propget][id] */ HRESULT ( STDMETHODCALLTYPE *get_UserName )( 
            Process2 * This,
            /* [retval][out] */ __RPC__deref_out_opt BSTR *UserName);
        
        END_INTERFACE
    } Process2Vtbl;

    interface Process2
    {
        CONST_VTBL struct Process2Vtbl *lpVtbl;
    };

    

#ifdef COBJMACROS


#define Process2_QueryInterface(This,riid,ppvObj,retval)	\
    ( (This)->lpVtbl -> QueryInterface(This,riid,ppvObj,retval) ) 

#define Process2_AddRef(This,retval)	\
    ( (This)->lpVtbl -> AddRef(This,retval) ) 

#define Process2_Release(This,retval)	\
    ( (This)->lpVtbl -> Release(This,retval) ) 

#define Process2_GetTypeInfoCount(This,pctinfo,retval)	\
    ( (This)->lpVtbl -> GetTypeInfoCount(This,pctinfo,retval) ) 

#define Process2_GetTypeInfo(This,itinfo,lcid,pptinfo,retval)	\
    ( (This)->lpVtbl -> GetTypeInfo(This,itinfo,lcid,pptinfo,retval) ) 

#define Process2_GetIDsOfNames(This,riid,rgszNames,cNames,lcid,rgdispid,retval)	\
    ( (This)->lpVtbl -> GetIDsOfNames(This,riid,rgszNames,cNames,lcid,rgdispid,retval) ) 

#define Process2_Invoke(This,dispidMember,riid,lcid,wFlags,pdispparams,pvarResult,pexcepinfo,puArgErr,retval)	\
    ( (This)->lpVtbl -> Invoke(This,dispidMember,riid,lcid,wFlags,pdispparams,pvarResult,pexcepinfo,puArgErr,retval) ) 

#define Process2_Attach(This,retval)	\
    ( (This)->lpVtbl -> Attach(This,retval) ) 

#define Process2_Detach(This,WaitForBreakOrEnd,retval)	\
    ( (This)->lpVtbl -> Detach(This,WaitForBreakOrEnd,retval) ) 

#define Process2_Break(This,WaitForBreakMode,retval)	\
    ( (This)->lpVtbl -> Break(This,WaitForBreakMode,retval) ) 

#define Process2_Terminate(This,WaitForBreakOrEnd,retval)	\
    ( (This)->lpVtbl -> Terminate(This,WaitForBreakOrEnd,retval) ) 

#define Process2_get_Name(This,retval)	\
    ( (This)->lpVtbl -> get_Name(This,retval) ) 

#define Process2_get_ProcessID(This,retval)	\
    ( (This)->lpVtbl -> get_ProcessID(This,retval) ) 

#define Process2_get_Programs(This,retval)	\
    ( (This)->lpVtbl -> get_Programs(This,retval) ) 

#define Process2_get_DTE(This,retval)	\
    ( (This)->lpVtbl -> get_DTE(This,retval) ) 

#define Process2_get_Parent(This,retval)	\
    ( (This)->lpVtbl -> get_Parent(This,retval) ) 

#define Process2_get_Collection(This,retval)	\
    ( (This)->lpVtbl -> get_Collection(This,retval) ) 


#define Process2_Attach2(This,Engines)	\
    ( (This)->lpVtbl -> Attach2(This,Engines) ) 

#define Process2_get_Transport(This,Transport)	\
    ( (This)->lpVtbl -> get_Transport(This,Transport) ) 

#define Process2_get_TransportQualifier(This,TransportQualifier)	\
    ( (This)->lpVtbl -> get_TransportQualifier(This,TransportQualifier) ) 

#define Process2_get_Threads(This,Threads)	\
    ( (This)->lpVtbl -> get_Threads(This,Threads) ) 

#define Process2_get_IsBeingDebugged(This,IsBeingDebugged)	\
    ( (This)->lpVtbl -> get_IsBeingDebugged(This,IsBeingDebugged) ) 

#define Process2_get_UserName(This,UserName)	\
    ( (This)->lpVtbl -> get_UserName(This,UserName) ) 

#endif /* COBJMACROS */


#endif 	/* C style interface */




#endif 	/* __Process2_INTERFACE_DEFINED__ */


#ifndef __Breakpoint2_INTERFACE_DEFINED__
#define __Breakpoint2_INTERFACE_DEFINED__

/* interface Breakpoint2 */
/* [object][version][helpstringcontext][helpstring][helpcontext][dual][uuid] */ 


EXTERN_C const IID IID_Breakpoint2;

#if defined(__cplusplus) && !defined(CINTERFACE)
    
    MIDL_INTERFACE("FBC8D85A-E449-4cb3-B026-F7808DEB7792")
    Breakpoint2 : public Breakpoint
    {
    public:
        virtual /* [helpstringcontext][helpstring][helpcontext][propget][id] */ HRESULT STDMETHODCALLTYPE get_Process( 
            /* [retval][out] */ __RPC__deref_out_opt Process2 **ppProcess) = 0;
        
        virtual /* [helpstringcontext][helpstring][helpcontext][propget][id] */ HRESULT STDMETHODCALLTYPE get_BreakWhenHit( 
            /* [retval][out] */ __RPC__out VARIANT_BOOL *BreakWhenHit) = 0;
        
        virtual /* [helpstringcontext][helpstring][helpcontext][propput][id] */ HRESULT STDMETHODCALLTYPE put_BreakWhenHit( 
            /* [in] */ VARIANT_BOOL BreakWhenHit) = 0;
        
        virtual /* [helpstringcontext][helpstring][helpcontext][propget][id] */ HRESULT STDMETHODCALLTYPE get_Message( 
            /* [retval][out] */ __RPC__deref_out_opt BSTR *Message) = 0;
        
        virtual /* [helpstringcontext][helpstring][helpcontext][propput][id] */ HRESULT STDMETHODCALLTYPE put_Message( 
            /* [in] */ __RPC__in BSTR Message) = 0;
        
        virtual /* [helpstringcontext][helpstring][helpcontext][propget][id] */ HRESULT STDMETHODCALLTYPE get_Macro( 
            /* [retval][out] */ __RPC__deref_out_opt BSTR *Macro) = 0;
        
        virtual /* [helpstringcontext][helpstring][helpcontext][propput][id] */ HRESULT STDMETHODCALLTYPE put_Macro( 
            /* [in] */ __RPC__in BSTR Macro) = 0;
        
        virtual /* [helpstringcontext][helpstring][helpcontext][propget][id] */ HRESULT STDMETHODCALLTYPE get_FilterBy( 
            /* [retval][out] */ __RPC__deref_out_opt BSTR *FilterBy) = 0;
        
        virtual /* [helpstringcontext][helpstring][helpcontext][propput][id] */ HRESULT STDMETHODCALLTYPE put_FilterBy( 
            /* [in] */ __RPC__in BSTR FilterBy) = 0;
        
    };
    
#else 	/* C style interface */

    typedef struct Breakpoint2Vtbl
    {
        BEGIN_INTERFACE
        
        /* [id][restricted][funcdescattr] */ HRESULT ( STDMETHODCALLTYPE *QueryInterface )( 
            Breakpoint2 * This,
            /* [in][idldescattr] */ __RPC__in struct GUID *riid,
            /* [out][idldescattr] */ __RPC__deref_out_opt void **ppvObj,
            /* [retval][out] */ __RPC__out void *retval);
        
        /* [id][restricted][funcdescattr] */ HRESULT ( STDMETHODCALLTYPE *AddRef )( 
            Breakpoint2 * This,
            /* [retval][out] */ __RPC__out unsigned long *retval);
        
        /* [id][restricted][funcdescattr] */ HRESULT ( STDMETHODCALLTYPE *Release )( 
            Breakpoint2 * This,
            /* [retval][out] */ __RPC__out unsigned long *retval);
        
        /* [id][restricted][funcdescattr] */ HRESULT ( STDMETHODCALLTYPE *GetTypeInfoCount )( 
            Breakpoint2 * This,
            /* [out][idldescattr] */ __RPC__out unsigned UINT *pctinfo,
            /* [retval][out] */ __RPC__out void *retval);
        
        /* [id][restricted][funcdescattr] */ HRESULT ( STDMETHODCALLTYPE *GetTypeInfo )( 
            Breakpoint2 * This,
            /* [in][idldescattr] */ unsigned UINT itinfo,
            /* [in][idldescattr] */ unsigned long lcid,
            /* [out][idldescattr] */ __RPC__deref_out_opt void **pptinfo,
            /* [retval][out] */ __RPC__out void *retval);
        
        /* [id][restricted][funcdescattr] */ HRESULT ( STDMETHODCALLTYPE *GetIDsOfNames )( 
            Breakpoint2 * This,
            /* [in][idldescattr] */ __RPC__in struct GUID *riid,
            /* [in][idldescattr] */ __RPC__deref_in_opt signed char **rgszNames,
            /* [in][idldescattr] */ unsigned UINT cNames,
            /* [in][idldescattr] */ unsigned long lcid,
            /* [out][idldescattr] */ __RPC__out signed long *rgdispid,
            /* [retval][out] */ __RPC__out void *retval);
        
        /* [id][restricted][funcdescattr] */ HRESULT ( STDMETHODCALLTYPE *Invoke )( 
            Breakpoint2 * This,
            /* [in][idldescattr] */ signed long dispidMember,
            /* [in][idldescattr] */ __RPC__in struct GUID *riid,
            /* [in][idldescattr] */ unsigned long lcid,
            /* [in][idldescattr] */ unsigned short wFlags,
            /* [in][idldescattr] */ __RPC__in struct DISPPARAMS *pdispparams,
            /* [out][idldescattr] */ __RPC__out VARIANT *pvarResult,
            /* [out][idldescattr] */ __RPC__out struct EXCEPINFO *pexcepinfo,
            /* [out][idldescattr] */ __RPC__out unsigned UINT *puArgErr,
            /* [retval][out] */ __RPC__out void *retval);
        
        /* [id][funcdescattr] */ HRESULT ( STDMETHODCALLTYPE *Delete )( 
            Breakpoint2 * This,
            /* [retval][out] */ __RPC__out void *retval);
        
        /* [id][propget][funcdescattr] */ HRESULT ( STDMETHODCALLTYPE *get_Type )( 
            Breakpoint2 * This,
            /* [retval][out] */ __RPC__out enum dbgBreakpointType *retval);
        
        /* [id][propget][funcdescattr] */ HRESULT ( STDMETHODCALLTYPE *get_Name )( 
            Breakpoint2 * This,
            /* [retval][out] */ __RPC__deref_out_opt BSTR *retval);
        
        /* [id][propput][funcdescattr] */ HRESULT ( STDMETHODCALLTYPE *put_Name )( 
            Breakpoint2 * This,
            /* [in][idldescattr] */ __RPC__in BSTR noname,
            /* [retval][out] */ __RPC__out void *retval);
        
        /* [id][propget][funcdescattr] */ HRESULT ( STDMETHODCALLTYPE *get_LocationType )( 
            Breakpoint2 * This,
            /* [retval][out] */ __RPC__out enum dbgBreakpointLocationType *retval);
        
        /* [id][propget][funcdescattr] */ HRESULT ( STDMETHODCALLTYPE *get_FunctionName )( 
            Breakpoint2 * This,
            /* [retval][out] */ __RPC__deref_out_opt BSTR *retval);
        
        /* [id][propget][funcdescattr] */ HRESULT ( STDMETHODCALLTYPE *get_FunctionLineOffset )( 
            Breakpoint2 * This,
            /* [retval][out] */ __RPC__out signed long *retval);
        
        /* [id][propget][funcdescattr] */ HRESULT ( STDMETHODCALLTYPE *get_FunctionColumnOffset )( 
            Breakpoint2 * This,
            /* [retval][out] */ __RPC__out signed long *retval);
        
        /* [id][propget][funcdescattr] */ HRESULT ( STDMETHODCALLTYPE *get_File )( 
            Breakpoint2 * This,
            /* [retval][out] */ __RPC__deref_out_opt BSTR *retval);
        
        /* [id][propget][funcdescattr] */ HRESULT ( STDMETHODCALLTYPE *get_FileLine )( 
            Breakpoint2 * This,
            /* [retval][out] */ __RPC__out signed long *retval);
        
        /* [id][propget][funcdescattr] */ HRESULT ( STDMETHODCALLTYPE *get_FileColumn )( 
            Breakpoint2 * This,
            /* [retval][out] */ __RPC__out signed long *retval);
        
        /* [id][propget][funcdescattr] */ HRESULT ( STDMETHODCALLTYPE *get_ConditionType )( 
            Breakpoint2 * This,
            /* [retval][out] */ __RPC__out enum dbgBreakpointConditionType *retval);
        
        /* [id][propget][funcdescattr] */ HRESULT ( STDMETHODCALLTYPE *get_Condition )( 
            Breakpoint2 * This,
            /* [retval][out] */ __RPC__deref_out_opt BSTR *retval);
        
        /* [id][propget][funcdescattr] */ HRESULT ( STDMETHODCALLTYPE *get_Language )( 
            Breakpoint2 * This,
            /* [retval][out] */ __RPC__deref_out_opt BSTR *retval);
        
        /* [id][propget][funcdescattr] */ HRESULT ( STDMETHODCALLTYPE *get_HitCountType )( 
            Breakpoint2 * This,
            /* [retval][out] */ __RPC__out enum dbgHitCountType *retval);
        
        /* [id][propget][funcdescattr] */ HRESULT ( STDMETHODCALLTYPE *get_HitCountTarget )( 
            Breakpoint2 * This,
            /* [retval][out] */ __RPC__out signed long *retval);
        
        /* [id][propget][funcdescattr] */ HRESULT ( STDMETHODCALLTYPE *get_Enabled )( 
            Breakpoint2 * This,
            /* [retval][out] */ __RPC__out BOOLEAN *retval);
        
        /* [id][propput][funcdescattr] */ HRESULT ( STDMETHODCALLTYPE *put_Enabled )( 
            Breakpoint2 * This,
            /* [in][idldescattr] */ BOOLEAN noname,
            /* [retval][out] */ __RPC__out void *retval);
        
        /* [id][propget][funcdescattr] */ HRESULT ( STDMETHODCALLTYPE *get_Tag )( 
            Breakpoint2 * This,
            /* [retval][out] */ __RPC__deref_out_opt BSTR *retval);
        
        /* [id][propput][funcdescattr] */ HRESULT ( STDMETHODCALLTYPE *put_Tag )( 
            Breakpoint2 * This,
            /* [in][idldescattr] */ __RPC__in BSTR noname,
            /* [retval][out] */ __RPC__out void *retval);
        
        /* [id][propget][funcdescattr] */ HRESULT ( STDMETHODCALLTYPE *get_CurrentHits )( 
            Breakpoint2 * This,
            /* [retval][out] */ __RPC__out signed long *retval);
        
        /* [id][propget][funcdescattr] */ HRESULT ( STDMETHODCALLTYPE *get_Program )( 
            Breakpoint2 * This,
            /* [retval][out] */ __RPC__deref_out_opt Program **retval);
        
        /* [id][propget][funcdescattr] */ HRESULT ( STDMETHODCALLTYPE *get_DTE )( 
            Breakpoint2 * This,
            /* [retval][out] */ __RPC__deref_out_opt **retval);
        
        /* [id][propget][funcdescattr] */ HRESULT ( STDMETHODCALLTYPE *get_Parent )( 
            Breakpoint2 * This,
            /* [retval][out] */ __RPC__deref_out_opt Breakpoint **retval);
        
        /* [id][propget][funcdescattr] */ HRESULT ( STDMETHODCALLTYPE *get_Collection )( 
            Breakpoint2 * This,
            /* [retval][out] */ __RPC__deref_out_opt Breakpoints **retval);
        
        /* [id][propget][funcdescattr] */ HRESULT ( STDMETHODCALLTYPE *get_Children )( 
            Breakpoint2 * This,
            /* [retval][out] */ __RPC__deref_out_opt Breakpoints **retval);
        
        /* [id][funcdescattr] */ HRESULT ( STDMETHODCALLTYPE *ResetHitCount )( 
            Breakpoint2 * This,
            /* [retval][out] */ __RPC__out void *retval);
        
        /* [helpstringcontext][helpstring][helpcontext][propget][id] */ HRESULT ( STDMETHODCALLTYPE *get_Process )( 
            Breakpoint2 * This,
            /* [retval][out] */ __RPC__deref_out_opt Process2 **ppProcess);
        
        /* [helpstringcontext][helpstring][helpcontext][propget][id] */ HRESULT ( STDMETHODCALLTYPE *get_BreakWhenHit )( 
            Breakpoint2 * This,
            /* [retval][out] */ __RPC__out VARIANT_BOOL *BreakWhenHit);
        
        /* [helpstringcontext][helpstring][helpcontext][propput][id] */ HRESULT ( STDMETHODCALLTYPE *put_BreakWhenHit )( 
            Breakpoint2 * This,
            /* [in] */ VARIANT_BOOL BreakWhenHit);
        
        /* [helpstringcontext][helpstring][helpcontext][propget][id] */ HRESULT ( STDMETHODCALLTYPE *get_Message )( 
            Breakpoint2 * This,
            /* [retval][out] */ __RPC__deref_out_opt BSTR *Message);
        
        /* [helpstringcontext][helpstring][helpcontext][propput][id] */ HRESULT ( STDMETHODCALLTYPE *put_Message )( 
            Breakpoint2 * This,
            /* [in] */ __RPC__in BSTR Message);
        
        /* [helpstringcontext][helpstring][helpcontext][propget][id] */ HRESULT ( STDMETHODCALLTYPE *get_Macro )( 
            Breakpoint2 * This,
            /* [retval][out] */ __RPC__deref_out_opt BSTR *Macro);
        
        /* [helpstringcontext][helpstring][helpcontext][propput][id] */ HRESULT ( STDMETHODCALLTYPE *put_Macro )( 
            Breakpoint2 * This,
            /* [in] */ __RPC__in BSTR Macro);
        
        /* [helpstringcontext][helpstring][helpcontext][propget][id] */ HRESULT ( STDMETHODCALLTYPE *get_FilterBy )( 
            Breakpoint2 * This,
            /* [retval][out] */ __RPC__deref_out_opt BSTR *FilterBy);
        
        /* [helpstringcontext][helpstring][helpcontext][propput][id] */ HRESULT ( STDMETHODCALLTYPE *put_FilterBy )( 
            Breakpoint2 * This,
            /* [in] */ __RPC__in BSTR FilterBy);
        
        END_INTERFACE
    } Breakpoint2Vtbl;

    interface Breakpoint2
    {
        CONST_VTBL struct Breakpoint2Vtbl *lpVtbl;
    };

    

#ifdef COBJMACROS


#define Breakpoint2_QueryInterface(This,riid,ppvObj,retval)	\
    ( (This)->lpVtbl -> QueryInterface(This,riid,ppvObj,retval) ) 

#define Breakpoint2_AddRef(This,retval)	\
    ( (This)->lpVtbl -> AddRef(This,retval) ) 

#define Breakpoint2_Release(This,retval)	\
    ( (This)->lpVtbl -> Release(This,retval) ) 

#define Breakpoint2_GetTypeInfoCount(This,pctinfo,retval)	\
    ( (This)->lpVtbl -> GetTypeInfoCount(This,pctinfo,retval) ) 

#define Breakpoint2_GetTypeInfo(This,itinfo,lcid,pptinfo,retval)	\
    ( (This)->lpVtbl -> GetTypeInfo(This,itinfo,lcid,pptinfo,retval) ) 

#define Breakpoint2_GetIDsOfNames(This,riid,rgszNames,cNames,lcid,rgdispid,retval)	\
    ( (This)->lpVtbl -> GetIDsOfNames(This,riid,rgszNames,cNames,lcid,rgdispid,retval) ) 

#define Breakpoint2_Invoke(This,dispidMember,riid,lcid,wFlags,pdispparams,pvarResult,pexcepinfo,puArgErr,retval)	\
    ( (This)->lpVtbl -> Invoke(This,dispidMember,riid,lcid,wFlags,pdispparams,pvarResult,pexcepinfo,puArgErr,retval) ) 

#define Breakpoint2_Delete(This,retval)	\
    ( (This)->lpVtbl -> Delete(This,retval) ) 

#define Breakpoint2_get_Type(This,retval)	\
    ( (This)->lpVtbl -> get_Type(This,retval) ) 

#define Breakpoint2_get_Name(This,retval)	\
    ( (This)->lpVtbl -> get_Name(This,retval) ) 

#define Breakpoint2_put_Name(This,noname,retval)	\
    ( (This)->lpVtbl -> put_Name(This,noname,retval) ) 

#define Breakpoint2_get_LocationType(This,retval)	\
    ( (This)->lpVtbl -> get_LocationType(This,retval) ) 

#define Breakpoint2_get_FunctionName(This,retval)	\
    ( (This)->lpVtbl -> get_FunctionName(This,retval) ) 

#define Breakpoint2_get_FunctionLineOffset(This,retval)	\
    ( (This)->lpVtbl -> get_FunctionLineOffset(This,retval) ) 

#define Breakpoint2_get_FunctionColumnOffset(This,retval)	\
    ( (This)->lpVtbl -> get_FunctionColumnOffset(This,retval) ) 

#define Breakpoint2_get_File(This,retval)	\
    ( (This)->lpVtbl -> get_File(This,retval) ) 

#define Breakpoint2_get_FileLine(This,retval)	\
    ( (This)->lpVtbl -> get_FileLine(This,retval) ) 

#define Breakpoint2_get_FileColumn(This,retval)	\
    ( (This)->lpVtbl -> get_FileColumn(This,retval) ) 

#define Breakpoint2_get_ConditionType(This,retval)	\
    ( (This)->lpVtbl -> get_ConditionType(This,retval) ) 

#define Breakpoint2_get_Condition(This,retval)	\
    ( (This)->lpVtbl -> get_Condition(This,retval) ) 

#define Breakpoint2_get_Language(This,retval)	\
    ( (This)->lpVtbl -> get_Language(This,retval) ) 

#define Breakpoint2_get_HitCountType(This,retval)	\
    ( (This)->lpVtbl -> get_HitCountType(This,retval) ) 

#define Breakpoint2_get_HitCountTarget(This,retval)	\
    ( (This)->lpVtbl -> get_HitCountTarget(This,retval) ) 

#define Breakpoint2_get_Enabled(This,retval)	\
    ( (This)->lpVtbl -> get_Enabled(This,retval) ) 

#define Breakpoint2_put_Enabled(This,noname,retval)	\
    ( (This)->lpVtbl -> put_Enabled(This,noname,retval) ) 

#define Breakpoint2_get_Tag(This,retval)	\
    ( (This)->lpVtbl -> get_Tag(This,retval) ) 

#define Breakpoint2_put_Tag(This,noname,retval)	\
    ( (This)->lpVtbl -> put_Tag(This,noname,retval) ) 

#define Breakpoint2_get_CurrentHits(This,retval)	\
    ( (This)->lpVtbl -> get_CurrentHits(This,retval) ) 

#define Breakpoint2_get_Program(This,retval)	\
    ( (This)->lpVtbl -> get_Program(This,retval) ) 

#define Breakpoint2_get_DTE(This,retval)	\
    ( (This)->lpVtbl -> get_DTE(This,retval) ) 

#define Breakpoint2_get_Parent(This,retval)	\
    ( (This)->lpVtbl -> get_Parent(This,retval) ) 

#define Breakpoint2_get_Collection(This,retval)	\
    ( (This)->lpVtbl -> get_Collection(This,retval) ) 

#define Breakpoint2_get_Children(This,retval)	\
    ( (This)->lpVtbl -> get_Children(This,retval) ) 

#define Breakpoint2_ResetHitCount(This,retval)	\
    ( (This)->lpVtbl -> ResetHitCount(This,retval) ) 


#define Breakpoint2_get_Process(This,ppProcess)	\
    ( (This)->lpVtbl -> get_Process(This,ppProcess) ) 

#define Breakpoint2_get_BreakWhenHit(This,BreakWhenHit)	\
    ( (This)->lpVtbl -> get_BreakWhenHit(This,BreakWhenHit) ) 

#define Breakpoint2_put_BreakWhenHit(This,BreakWhenHit)	\
    ( (This)->lpVtbl -> put_BreakWhenHit(This,BreakWhenHit) ) 

#define Breakpoint2_get_Message(This,Message)	\
    ( (This)->lpVtbl -> get_Message(This,Message) ) 

#define Breakpoint2_put_Message(This,Message)	\
    ( (This)->lpVtbl -> put_Message(This,Message) ) 

#define Breakpoint2_get_Macro(This,Macro)	\
    ( (This)->lpVtbl -> get_Macro(This,Macro) ) 

#define Breakpoint2_put_Macro(This,Macro)	\
    ( (This)->lpVtbl -> put_Macro(This,Macro) ) 

#define Breakpoint2_get_FilterBy(This,FilterBy)	\
    ( (This)->lpVtbl -> get_FilterBy(This,FilterBy) ) 

#define Breakpoint2_put_FilterBy(This,FilterBy)	\
    ( (This)->lpVtbl -> put_FilterBy(This,FilterBy) ) 

#endif /* COBJMACROS */


#endif 	/* C style interface */




#endif 	/* __Breakpoint2_INTERFACE_DEFINED__ */


#ifndef __Engine_INTERFACE_DEFINED__
#define __Engine_INTERFACE_DEFINED__

/* interface Engine */
/* [object][version][helpstringcontext][helpstring][helpcontext][dual][uuid] */ 


EXTERN_C const IID IID_Engine;

#if defined(__cplusplus) && !defined(CINTERFACE)
    
    MIDL_INTERFACE("8CEA6D39-EBEE-4de9-B282-B5CECE9C9861")
    Engine : public IDispatch
    {
    public:
        virtual /* [helpstringcontext][helpstring][helpcontext][propget][id] */ HRESULT STDMETHODCALLTYPE get_Name( 
            /* [retval][out] */ __RPC__deref_out_opt BSTR *Name) = 0;
        
        virtual /* [helpstringcontext][helpstring][helpcontext][propget][id] */ HRESULT STDMETHODCALLTYPE get_ID( 
            /* [retval][out] */ __RPC__deref_out_opt BSTR *ID) = 0;
        
        virtual /* [helpstringcontext][helpstring][helpcontext][propget][id] */ HRESULT STDMETHODCALLTYPE get_AttachResult( 
            /* [retval][out] */ __RPC__out HRESULT *Result) = 0;
        
        virtual /* [helpstringcontext][helpstring][helpcontext][propget][id] */ HRESULT STDMETHODCALLTYPE get_DTE( 
            /* [retval][out] */ __RPC__deref_out_opt /* external definition not present */ DTE **DTE) = 0;
        
        virtual /* [helpstringcontext][helpstring][helpcontext][propget][id] */ HRESULT STDMETHODCALLTYPE get_Parent( 
            /* [retval][out] */ __RPC__deref_out_opt Transport **Transport) = 0;
        
        virtual /* [helpstringcontext][helpstring][helpcontext][propget][id] */ HRESULT STDMETHODCALLTYPE get_Collection( 
            /* [retval][out] */ __RPC__deref_out_opt Engines **Engines) = 0;
        
    };
    
#else 	/* C style interface */

    typedef struct EngineVtbl
    {
        BEGIN_INTERFACE
        
        HRESULT ( STDMETHODCALLTYPE *QueryInterface )( 
            Engine * This,
            /* [in] */ __RPC__in REFIID riid,
            /* [iid_is][out] */ 
            __RPC__deref_out  void **ppvObject);
        
        ULONG ( STDMETHODCALLTYPE *AddRef )( 
            Engine * This);
        
        ULONG ( STDMETHODCALLTYPE *Release )( 
            Engine * This);
        
        HRESULT ( STDMETHODCALLTYPE *GetTypeInfoCount )( 
            Engine * This,
            /* [out] */ __RPC__out UINT *pctinfo);
        
        HRESULT ( STDMETHODCALLTYPE *GetTypeInfo )( 
            Engine * This,
            /* [in] */ UINT iTInfo,
            /* [in] */ LCID lcid,
            /* [out] */ __RPC__deref_out_opt ITypeInfo **ppTInfo);
        
        HRESULT ( STDMETHODCALLTYPE *GetIDsOfNames )( 
            Engine * This,
            /* [in] */ __RPC__in REFIID riid,
            /* [size_is][in] */ __RPC__in_ecount_full(cNames) LPOLESTR *rgszNames,
            /* [range][in] */ UINT cNames,
            /* [in] */ LCID lcid,
            /* [size_is][out] */ __RPC__out_ecount_full(cNames) DISPID *rgDispId);
        
        /* [local] */ HRESULT ( STDMETHODCALLTYPE *Invoke )( 
            Engine * This,
            /* [in] */ DISPID dispIdMember,
            /* [in] */ REFIID riid,
            /* [in] */ LCID lcid,
            /* [in] */ WORD wFlags,
            /* [out][in] */ DISPPARAMS *pDispParams,
            /* [out] */ VARIANT *pVarResult,
            /* [out] */ EXCEPINFO *pExcepInfo,
            /* [out] */ UINT *puArgErr);
        
        /* [helpstringcontext][helpstring][helpcontext][propget][id] */ HRESULT ( STDMETHODCALLTYPE *get_Name )( 
            Engine * This,
            /* [retval][out] */ __RPC__deref_out_opt BSTR *Name);
        
        /* [helpstringcontext][helpstring][helpcontext][propget][id] */ HRESULT ( STDMETHODCALLTYPE *get_ID )( 
            Engine * This,
            /* [retval][out] */ __RPC__deref_out_opt BSTR *ID);
        
        /* [helpstringcontext][helpstring][helpcontext][propget][id] */ HRESULT ( STDMETHODCALLTYPE *get_AttachResult )( 
            Engine * This,
            /* [retval][out] */ __RPC__out HRESULT *Result);
        
        /* [helpstringcontext][helpstring][helpcontext][propget][id] */ HRESULT ( STDMETHODCALLTYPE *get_DTE )( 
            Engine * This,
            /* [retval][out] */ __RPC__deref_out_opt /* external definition not present */ DTE **DTE);
        
        /* [helpstringcontext][helpstring][helpcontext][propget][id] */ HRESULT ( STDMETHODCALLTYPE *get_Parent )( 
            Engine * This,
            /* [retval][out] */ __RPC__deref_out_opt Transport **Transport);
        
        /* [helpstringcontext][helpstring][helpcontext][propget][id] */ HRESULT ( STDMETHODCALLTYPE *get_Collection )( 
            Engine * This,
            /* [retval][out] */ __RPC__deref_out_opt Engines **Engines);
        
        END_INTERFACE
    } EngineVtbl;

    interface Engine
    {
        CONST_VTBL struct EngineVtbl *lpVtbl;
    };

    

#ifdef COBJMACROS


#define Engine_QueryInterface(This,riid,ppvObject)	\
    ( (This)->lpVtbl -> QueryInterface(This,riid,ppvObject) ) 

#define Engine_AddRef(This)	\
    ( (This)->lpVtbl -> AddRef(This) ) 

#define Engine_Release(This)	\
    ( (This)->lpVtbl -> Release(This) ) 


#define Engine_GetTypeInfoCount(This,pctinfo)	\
    ( (This)->lpVtbl -> GetTypeInfoCount(This,pctinfo) ) 

#define Engine_GetTypeInfo(This,iTInfo,lcid,ppTInfo)	\
    ( (This)->lpVtbl -> GetTypeInfo(This,iTInfo,lcid,ppTInfo) ) 

#define Engine_GetIDsOfNames(This,riid,rgszNames,cNames,lcid,rgDispId)	\
    ( (This)->lpVtbl -> GetIDsOfNames(This,riid,rgszNames,cNames,lcid,rgDispId) ) 

#define Engine_Invoke(This,dispIdMember,riid,lcid,wFlags,pDispParams,pVarResult,pExcepInfo,puArgErr)	\
    ( (This)->lpVtbl -> Invoke(This,dispIdMember,riid,lcid,wFlags,pDispParams,pVarResult,pExcepInfo,puArgErr) ) 


#define Engine_get_Name(This,Name)	\
    ( (This)->lpVtbl -> get_Name(This,Name) ) 

#define Engine_get_ID(This,ID)	\
    ( (This)->lpVtbl -> get_ID(This,ID) ) 

#define Engine_get_AttachResult(This,Result)	\
    ( (This)->lpVtbl -> get_AttachResult(This,Result) ) 

#define Engine_get_DTE(This,DTE)	\
    ( (This)->lpVtbl -> get_DTE(This,DTE) ) 

#define Engine_get_Parent(This,Transport)	\
    ( (This)->lpVtbl -> get_Parent(This,Transport) ) 

#define Engine_get_Collection(This,Engines)	\
    ( (This)->lpVtbl -> get_Collection(This,Engines) ) 

#endif /* COBJMACROS */


#endif 	/* C style interface */




#endif 	/* __Engine_INTERFACE_DEFINED__ */


#ifndef __Transport_INTERFACE_DEFINED__
#define __Transport_INTERFACE_DEFINED__

/* interface Transport */
/* [object][version][helpstringcontext][helpstring][helpcontext][dual][uuid] */ 


EXTERN_C const IID IID_Transport;

#if defined(__cplusplus) && !defined(CINTERFACE)
    
    MIDL_INTERFACE("C977EAED-9E6C-4122-9D28-9D5EAE2BDC33")
    Transport : public IDispatch
    {
    public:
        virtual /* [helpstringcontext][helpstring][helpcontext][propget][id] */ HRESULT STDMETHODCALLTYPE get_Name( 
            /* [retval][out] */ __RPC__deref_out_opt BSTR *Name) = 0;
        
        virtual /* [helpstringcontext][helpstring][helpcontext][propget][id] */ HRESULT STDMETHODCALLTYPE get_ID( 
            /* [retval][out] */ __RPC__deref_out_opt BSTR *ID) = 0;
        
        virtual /* [helpstringcontext][helpstring][helpcontext][propget][id] */ HRESULT STDMETHODCALLTYPE get_Engines( 
            /* [retval][out] */ __RPC__deref_out_opt Engines **Engines) = 0;
        
        virtual /* [helpstringcontext][helpstring][helpcontext][propget][id] */ HRESULT STDMETHODCALLTYPE get_DTE( 
            /* [retval][out] */ __RPC__deref_out_opt /* external definition not present */ DTE **DTE) = 0;
        
        virtual /* [helpstringcontext][helpstring][helpcontext][propget][id] */ HRESULT STDMETHODCALLTYPE get_Parent( 
            /* [retval][out] */ __RPC__deref_out_opt Debugger2 **Debugger) = 0;
        
        virtual /* [helpstringcontext][helpstring][helpcontext][propget][id] */ HRESULT STDMETHODCALLTYPE get_Collection( 
            /* [retval][out] */ __RPC__deref_out_opt Transports **Transports) = 0;
        
    };
    
#else 	/* C style interface */

    typedef struct TransportVtbl
    {
        BEGIN_INTERFACE
        
        HRESULT ( STDMETHODCALLTYPE *QueryInterface )( 
            Transport * This,
            /* [in] */ __RPC__in REFIID riid,
            /* [iid_is][out] */ 
            __RPC__deref_out  void **ppvObject);
        
        ULONG ( STDMETHODCALLTYPE *AddRef )( 
            Transport * This);
        
        ULONG ( STDMETHODCALLTYPE *Release )( 
            Transport * This);
        
        HRESULT ( STDMETHODCALLTYPE *GetTypeInfoCount )( 
            Transport * This,
            /* [out] */ __RPC__out UINT *pctinfo);
        
        HRESULT ( STDMETHODCALLTYPE *GetTypeInfo )( 
            Transport * This,
            /* [in] */ UINT iTInfo,
            /* [in] */ LCID lcid,
            /* [out] */ __RPC__deref_out_opt ITypeInfo **ppTInfo);
        
        HRESULT ( STDMETHODCALLTYPE *GetIDsOfNames )( 
            Transport * This,
            /* [in] */ __RPC__in REFIID riid,
            /* [size_is][in] */ __RPC__in_ecount_full(cNames) LPOLESTR *rgszNames,
            /* [range][in] */ UINT cNames,
            /* [in] */ LCID lcid,
            /* [size_is][out] */ __RPC__out_ecount_full(cNames) DISPID *rgDispId);
        
        /* [local] */ HRESULT ( STDMETHODCALLTYPE *Invoke )( 
            Transport * This,
            /* [in] */ DISPID dispIdMember,
            /* [in] */ REFIID riid,
            /* [in] */ LCID lcid,
            /* [in] */ WORD wFlags,
            /* [out][in] */ DISPPARAMS *pDispParams,
            /* [out] */ VARIANT *pVarResult,
            /* [out] */ EXCEPINFO *pExcepInfo,
            /* [out] */ UINT *puArgErr);
        
        /* [helpstringcontext][helpstring][helpcontext][propget][id] */ HRESULT ( STDMETHODCALLTYPE *get_Name )( 
            Transport * This,
            /* [retval][out] */ __RPC__deref_out_opt BSTR *Name);
        
        /* [helpstringcontext][helpstring][helpcontext][propget][id] */ HRESULT ( STDMETHODCALLTYPE *get_ID )( 
            Transport * This,
            /* [retval][out] */ __RPC__deref_out_opt BSTR *ID);
        
        /* [helpstringcontext][helpstring][helpcontext][propget][id] */ HRESULT ( STDMETHODCALLTYPE *get_Engines )( 
            Transport * This,
            /* [retval][out] */ __RPC__deref_out_opt Engines **Engines);
        
        /* [helpstringcontext][helpstring][helpcontext][propget][id] */ HRESULT ( STDMETHODCALLTYPE *get_DTE )( 
            Transport * This,
            /* [retval][out] */ __RPC__deref_out_opt /* external definition not present */ DTE **DTE);
        
        /* [helpstringcontext][helpstring][helpcontext][propget][id] */ HRESULT ( STDMETHODCALLTYPE *get_Parent )( 
            Transport * This,
            /* [retval][out] */ __RPC__deref_out_opt Debugger2 **Debugger);
        
        /* [helpstringcontext][helpstring][helpcontext][propget][id] */ HRESULT ( STDMETHODCALLTYPE *get_Collection )( 
            Transport * This,
            /* [retval][out] */ __RPC__deref_out_opt Transports **Transports);
        
        END_INTERFACE
    } TransportVtbl;

    interface Transport
    {
        CONST_VTBL struct TransportVtbl *lpVtbl;
    };

    

#ifdef COBJMACROS


#define Transport_QueryInterface(This,riid,ppvObject)	\
    ( (This)->lpVtbl -> QueryInterface(This,riid,ppvObject) ) 

#define Transport_AddRef(This)	\
    ( (This)->lpVtbl -> AddRef(This) ) 

#define Transport_Release(This)	\
    ( (This)->lpVtbl -> Release(This) ) 


#define Transport_GetTypeInfoCount(This,pctinfo)	\
    ( (This)->lpVtbl -> GetTypeInfoCount(This,pctinfo) ) 

#define Transport_GetTypeInfo(This,iTInfo,lcid,ppTInfo)	\
    ( (This)->lpVtbl -> GetTypeInfo(This,iTInfo,lcid,ppTInfo) ) 

#define Transport_GetIDsOfNames(This,riid,rgszNames,cNames,lcid,rgDispId)	\
    ( (This)->lpVtbl -> GetIDsOfNames(This,riid,rgszNames,cNames,lcid,rgDispId) ) 

#define Transport_Invoke(This,dispIdMember,riid,lcid,wFlags,pDispParams,pVarResult,pExcepInfo,puArgErr)	\
    ( (This)->lpVtbl -> Invoke(This,dispIdMember,riid,lcid,wFlags,pDispParams,pVarResult,pExcepInfo,puArgErr) ) 


#define Transport_get_Name(This,Name)	\
    ( (This)->lpVtbl -> get_Name(This,Name) ) 

#define Transport_get_ID(This,ID)	\
    ( (This)->lpVtbl -> get_ID(This,ID) ) 

#define Transport_get_Engines(This,Engines)	\
    ( (This)->lpVtbl -> get_Engines(This,Engines) ) 

#define Transport_get_DTE(This,DTE)	\
    ( (This)->lpVtbl -> get_DTE(This,DTE) ) 

#define Transport_get_Parent(This,Debugger)	\
    ( (This)->lpVtbl -> get_Parent(This,Debugger) ) 

#define Transport_get_Collection(This,Transports)	\
    ( (This)->lpVtbl -> get_Collection(This,Transports) ) 

#endif /* COBJMACROS */


#endif 	/* C style interface */




#endif 	/* __Transport_INTERFACE_DEFINED__ */


#ifndef __Engines_INTERFACE_DEFINED__
#define __Engines_INTERFACE_DEFINED__

/* interface Engines */
/* [object][version][helpstringcontext][helpstring][helpcontext][dual][uuid] */ 


EXTERN_C const IID IID_Engines;

#if defined(__cplusplus) && !defined(CINTERFACE)
    
    MIDL_INTERFACE("9F998C5E-549E-4c74-9FD8-B3A93D85A248")
    Engines : public IDispatch
    {
    public:
        virtual /* [helpstringcontext][helpstring][helpcontext][id] */ HRESULT STDMETHODCALLTYPE Item( 
            /* [in] */ VARIANT Index,
            /* [retval][out] */ __RPC__deref_out_opt Engine **Engine) = 0;
        
        virtual /* [restricted][id] */ HRESULT STDMETHODCALLTYPE _NewEnum( 
            /* [retval][out] */ __RPC__deref_out_opt IUnknown **Enumerator) = 0;
        
        virtual /* [helpstringcontext][helpstring][helpcontext][propget][id] */ HRESULT STDMETHODCALLTYPE get_DTE( 
            /* [retval][out] */ __RPC__deref_out_opt /* external definition not present */ DTE **DTEObject) = 0;
        
        virtual /* [helpstringcontext][helpstring][helpcontext][propget][id] */ HRESULT STDMETHODCALLTYPE get_Parent( 
            /* [retval][out] */ __RPC__deref_out_opt /* external definition not present */ Debugger **Debugger) = 0;
        
        virtual /* [helpstringcontext][helpstring][helpcontext][propget][id] */ HRESULT STDMETHODCALLTYPE get_Count( 
            /* [retval][out] */ __RPC__out long *Count) = 0;
        
    };
    
#else 	/* C style interface */

    typedef struct EnginesVtbl
    {
        BEGIN_INTERFACE
        
        HRESULT ( STDMETHODCALLTYPE *QueryInterface )( 
            Engines * This,
            /* [in] */ __RPC__in REFIID riid,
            /* [iid_is][out] */ 
            __RPC__deref_out  void **ppvObject);
        
        ULONG ( STDMETHODCALLTYPE *AddRef )( 
            Engines * This);
        
        ULONG ( STDMETHODCALLTYPE *Release )( 
            Engines * This);
        
        HRESULT ( STDMETHODCALLTYPE *GetTypeInfoCount )( 
            Engines * This,
            /* [out] */ __RPC__out UINT *pctinfo);
        
        HRESULT ( STDMETHODCALLTYPE *GetTypeInfo )( 
            Engines * This,
            /* [in] */ UINT iTInfo,
            /* [in] */ LCID lcid,
            /* [out] */ __RPC__deref_out_opt ITypeInfo **ppTInfo);
        
        HRESULT ( STDMETHODCALLTYPE *GetIDsOfNames )( 
            Engines * This,
            /* [in] */ __RPC__in REFIID riid,
            /* [size_is][in] */ __RPC__in_ecount_full(cNames) LPOLESTR *rgszNames,
            /* [range][in] */ UINT cNames,
            /* [in] */ LCID lcid,
            /* [size_is][out] */ __RPC__out_ecount_full(cNames) DISPID *rgDispId);
        
        /* [local] */ HRESULT ( STDMETHODCALLTYPE *Invoke )( 
            Engines * This,
            /* [in] */ DISPID dispIdMember,
            /* [in] */ REFIID riid,
            /* [in] */ LCID lcid,
            /* [in] */ WORD wFlags,
            /* [out][in] */ DISPPARAMS *pDispParams,
            /* [out] */ VARIANT *pVarResult,
            /* [out] */ EXCEPINFO *pExcepInfo,
            /* [out] */ UINT *puArgErr);
        
        /* [helpstringcontext][helpstring][helpcontext][id] */ HRESULT ( STDMETHODCALLTYPE *Item )( 
            Engines * This,
            /* [in] */ VARIANT Index,
            /* [retval][out] */ __RPC__deref_out_opt Engine **Engine);
        
        /* [restricted][id] */ HRESULT ( STDMETHODCALLTYPE *_NewEnum )( 
            Engines * This,
            /* [retval][out] */ __RPC__deref_out_opt IUnknown **Enumerator);
        
        /* [helpstringcontext][helpstring][helpcontext][propget][id] */ HRESULT ( STDMETHODCALLTYPE *get_DTE )( 
            Engines * This,
            /* [retval][out] */ __RPC__deref_out_opt /* external definition not present */ DTE **DTEObject);
        
        /* [helpstringcontext][helpstring][helpcontext][propget][id] */ HRESULT ( STDMETHODCALLTYPE *get_Parent )( 
            Engines * This,
            /* [retval][out] */ __RPC__deref_out_opt /* external definition not present */ Debugger **Debugger);
        
        /* [helpstringcontext][helpstring][helpcontext][propget][id] */ HRESULT ( STDMETHODCALLTYPE *get_Count )( 
            Engines * This,
            /* [retval][out] */ __RPC__out long *Count);
        
        END_INTERFACE
    } EnginesVtbl;

    interface Engines
    {
        CONST_VTBL struct EnginesVtbl *lpVtbl;
    };

    

#ifdef COBJMACROS


#define Engines_QueryInterface(This,riid,ppvObject)	\
    ( (This)->lpVtbl -> QueryInterface(This,riid,ppvObject) ) 

#define Engines_AddRef(This)	\
    ( (This)->lpVtbl -> AddRef(This) ) 

#define Engines_Release(This)	\
    ( (This)->lpVtbl -> Release(This) ) 


#define Engines_GetTypeInfoCount(This,pctinfo)	\
    ( (This)->lpVtbl -> GetTypeInfoCount(This,pctinfo) ) 

#define Engines_GetTypeInfo(This,iTInfo,lcid,ppTInfo)	\
    ( (This)->lpVtbl -> GetTypeInfo(This,iTInfo,lcid,ppTInfo) ) 

#define Engines_GetIDsOfNames(This,riid,rgszNames,cNames,lcid,rgDispId)	\
    ( (This)->lpVtbl -> GetIDsOfNames(This,riid,rgszNames,cNames,lcid,rgDispId) ) 

#define Engines_Invoke(This,dispIdMember,riid,lcid,wFlags,pDispParams,pVarResult,pExcepInfo,puArgErr)	\
    ( (This)->lpVtbl -> Invoke(This,dispIdMember,riid,lcid,wFlags,pDispParams,pVarResult,pExcepInfo,puArgErr) ) 


#define Engines_Item(This,Index,Engine)	\
    ( (This)->lpVtbl -> Item(This,Index,Engine) ) 

#define Engines__NewEnum(This,Enumerator)	\
    ( (This)->lpVtbl -> _NewEnum(This,Enumerator) ) 

#define Engines_get_DTE(This,DTEObject)	\
    ( (This)->lpVtbl -> get_DTE(This,DTEObject) ) 

#define Engines_get_Parent(This,Debugger)	\
    ( (This)->lpVtbl -> get_Parent(This,Debugger) ) 

#define Engines_get_Count(This,Count)	\
    ( (This)->lpVtbl -> get_Count(This,Count) ) 

#endif /* COBJMACROS */


#endif 	/* C style interface */




#endif 	/* __Engines_INTERFACE_DEFINED__ */


#ifndef __Transports_INTERFACE_DEFINED__
#define __Transports_INTERFACE_DEFINED__

/* interface Transports */
/* [object][version][helpstringcontext][helpstring][helpcontext][dual][uuid] */ 


EXTERN_C const IID IID_Transports;

#if defined(__cplusplus) && !defined(CINTERFACE)
    
    MIDL_INTERFACE("EA47C3D9-FD41-4402-BDC6-7F07D0C8E3FC")
    Transports : public IDispatch
    {
    public:
        virtual /* [helpstringcontext][helpstring][helpcontext][id] */ HRESULT STDMETHODCALLTYPE Item( 
            /* [in] */ VARIANT Index,
            /* [retval][out] */ __RPC__deref_out_opt Transport **Transport) = 0;
        
        virtual /* [restricted][id] */ HRESULT STDMETHODCALLTYPE _NewEnum( 
            /* [retval][out] */ __RPC__deref_out_opt IUnknown **Enumerator) = 0;
        
        virtual /* [helpstringcontext][helpstring][helpcontext][propget][id] */ HRESULT STDMETHODCALLTYPE get_DTE( 
            /* [retval][out] */ __RPC__deref_out_opt /* external definition not present */ DTE **DTEObject) = 0;
        
        virtual /* [helpstringcontext][helpstring][helpcontext][propget][id] */ HRESULT STDMETHODCALLTYPE get_Parent( 
            /* [retval][out] */ __RPC__deref_out_opt /* external definition not present */ Debugger **Debugger) = 0;
        
        virtual /* [helpstringcontext][helpstring][helpcontext][propget][id] */ HRESULT STDMETHODCALLTYPE get_Count( 
            /* [retval][out] */ __RPC__out long *Count) = 0;
        
    };
    
#else 	/* C style interface */

    typedef struct TransportsVtbl
    {
        BEGIN_INTERFACE
        
        HRESULT ( STDMETHODCALLTYPE *QueryInterface )( 
            Transports * This,
            /* [in] */ __RPC__in REFIID riid,
            /* [iid_is][out] */ 
            __RPC__deref_out  void **ppvObject);
        
        ULONG ( STDMETHODCALLTYPE *AddRef )( 
            Transports * This);
        
        ULONG ( STDMETHODCALLTYPE *Release )( 
            Transports * This);
        
        HRESULT ( STDMETHODCALLTYPE *GetTypeInfoCount )( 
            Transports * This,
            /* [out] */ __RPC__out UINT *pctinfo);
        
        HRESULT ( STDMETHODCALLTYPE *GetTypeInfo )( 
            Transports * This,
            /* [in] */ UINT iTInfo,
            /* [in] */ LCID lcid,
            /* [out] */ __RPC__deref_out_opt ITypeInfo **ppTInfo);
        
        HRESULT ( STDMETHODCALLTYPE *GetIDsOfNames )( 
            Transports * This,
            /* [in] */ __RPC__in REFIID riid,
            /* [size_is][in] */ __RPC__in_ecount_full(cNames) LPOLESTR *rgszNames,
            /* [range][in] */ UINT cNames,
            /* [in] */ LCID lcid,
            /* [size_is][out] */ __RPC__out_ecount_full(cNames) DISPID *rgDispId);
        
        /* [local] */ HRESULT ( STDMETHODCALLTYPE *Invoke )( 
            Transports * This,
            /* [in] */ DISPID dispIdMember,
            /* [in] */ REFIID riid,
            /* [in] */ LCID lcid,
            /* [in] */ WORD wFlags,
            /* [out][in] */ DISPPARAMS *pDispParams,
            /* [out] */ VARIANT *pVarResult,
            /* [out] */ EXCEPINFO *pExcepInfo,
            /* [out] */ UINT *puArgErr);
        
        /* [helpstringcontext][helpstring][helpcontext][id] */ HRESULT ( STDMETHODCALLTYPE *Item )( 
            Transports * This,
            /* [in] */ VARIANT Index,
            /* [retval][out] */ __RPC__deref_out_opt Transport **Transport);
        
        /* [restricted][id] */ HRESULT ( STDMETHODCALLTYPE *_NewEnum )( 
            Transports * This,
            /* [retval][out] */ __RPC__deref_out_opt IUnknown **Enumerator);
        
        /* [helpstringcontext][helpstring][helpcontext][propget][id] */ HRESULT ( STDMETHODCALLTYPE *get_DTE )( 
            Transports * This,
            /* [retval][out] */ __RPC__deref_out_opt /* external definition not present */ DTE **DTEObject);
        
        /* [helpstringcontext][helpstring][helpcontext][propget][id] */ HRESULT ( STDMETHODCALLTYPE *get_Parent )( 
            Transports * This,
            /* [retval][out] */ __RPC__deref_out_opt /* external definition not present */ Debugger **Debugger);
        
        /* [helpstringcontext][helpstring][helpcontext][propget][id] */ HRESULT ( STDMETHODCALLTYPE *get_Count )( 
            Transports * This,
            /* [retval][out] */ __RPC__out long *Count);
        
        END_INTERFACE
    } TransportsVtbl;

    interface Transports
    {
        CONST_VTBL struct TransportsVtbl *lpVtbl;
    };

    

#ifdef COBJMACROS


#define Transports_QueryInterface(This,riid,ppvObject)	\
    ( (This)->lpVtbl -> QueryInterface(This,riid,ppvObject) ) 

#define Transports_AddRef(This)	\
    ( (This)->lpVtbl -> AddRef(This) ) 

#define Transports_Release(This)	\
    ( (This)->lpVtbl -> Release(This) ) 


#define Transports_GetTypeInfoCount(This,pctinfo)	\
    ( (This)->lpVtbl -> GetTypeInfoCount(This,pctinfo) ) 

#define Transports_GetTypeInfo(This,iTInfo,lcid,ppTInfo)	\
    ( (This)->lpVtbl -> GetTypeInfo(This,iTInfo,lcid,ppTInfo) ) 

#define Transports_GetIDsOfNames(This,riid,rgszNames,cNames,lcid,rgDispId)	\
    ( (This)->lpVtbl -> GetIDsOfNames(This,riid,rgszNames,cNames,lcid,rgDispId) ) 

#define Transports_Invoke(This,dispIdMember,riid,lcid,wFlags,pDispParams,pVarResult,pExcepInfo,puArgErr)	\
    ( (This)->lpVtbl -> Invoke(This,dispIdMember,riid,lcid,wFlags,pDispParams,pVarResult,pExcepInfo,puArgErr) ) 


#define Transports_Item(This,Index,Transport)	\
    ( (This)->lpVtbl -> Item(This,Index,Transport) ) 

#define Transports__NewEnum(This,Enumerator)	\
    ( (This)->lpVtbl -> _NewEnum(This,Enumerator) ) 

#define Transports_get_DTE(This,DTEObject)	\
    ( (This)->lpVtbl -> get_DTE(This,DTEObject) ) 

#define Transports_get_Parent(This,Debugger)	\
    ( (This)->lpVtbl -> get_Parent(This,Debugger) ) 

#define Transports_get_Count(This,Count)	\
    ( (This)->lpVtbl -> get_Count(This,Count) ) 

#endif /* COBJMACROS */


#endif 	/* C style interface */




#endif 	/* __Transports_INTERFACE_DEFINED__ */


#ifndef ___dispDebuggerProcessEvents_DISPINTERFACE_DEFINED__
#define ___dispDebuggerProcessEvents_DISPINTERFACE_DEFINED__

/* dispinterface _dispDebuggerProcessEvents */
/* [version][uuid] */ 


EXTERN_C const IID DIID__dispDebuggerProcessEvents;

#if defined(__cplusplus) && !defined(CINTERFACE)

    MIDL_INTERFACE("7d04c01d-bb7a-47e8-92eb-e914cd61366b")
    _dispDebuggerProcessEvents : public IDispatch
    {
    };
    
#else 	/* C style interface */

    typedef struct _dispDebuggerProcessEventsVtbl
    {
        BEGIN_INTERFACE
        
        HRESULT ( STDMETHODCALLTYPE *QueryInterface )( 
            _dispDebuggerProcessEvents * This,
            /* [in] */ __RPC__in REFIID riid,
            /* [iid_is][out] */ 
            __RPC__deref_out  void **ppvObject);
        
        ULONG ( STDMETHODCALLTYPE *AddRef )( 
            _dispDebuggerProcessEvents * This);
        
        ULONG ( STDMETHODCALLTYPE *Release )( 
            _dispDebuggerProcessEvents * This);
        
        HRESULT ( STDMETHODCALLTYPE *GetTypeInfoCount )( 
            _dispDebuggerProcessEvents * This,
            /* [out] */ __RPC__out UINT *pctinfo);
        
        HRESULT ( STDMETHODCALLTYPE *GetTypeInfo )( 
            _dispDebuggerProcessEvents * This,
            /* [in] */ UINT iTInfo,
            /* [in] */ LCID lcid,
            /* [out] */ __RPC__deref_out_opt ITypeInfo **ppTInfo);
        
        HRESULT ( STDMETHODCALLTYPE *GetIDsOfNames )( 
            _dispDebuggerProcessEvents * This,
            /* [in] */ __RPC__in REFIID riid,
            /* [size_is][in] */ __RPC__in_ecount_full(cNames) LPOLESTR *rgszNames,
            /* [range][in] */ UINT cNames,
            /* [in] */ LCID lcid,
            /* [size_is][out] */ __RPC__out_ecount_full(cNames) DISPID *rgDispId);
        
        /* [local] */ HRESULT ( STDMETHODCALLTYPE *Invoke )( 
            _dispDebuggerProcessEvents * This,
            /* [in] */ DISPID dispIdMember,
            /* [in] */ REFIID riid,
            /* [in] */ LCID lcid,
            /* [in] */ WORD wFlags,
            /* [out][in] */ DISPPARAMS *pDispParams,
            /* [out] */ VARIANT *pVarResult,
            /* [out] */ EXCEPINFO *pExcepInfo,
            /* [out] */ UINT *puArgErr);
        
        END_INTERFACE
    } _dispDebuggerProcessEventsVtbl;

    interface _dispDebuggerProcessEvents
    {
        CONST_VTBL struct _dispDebuggerProcessEventsVtbl *lpVtbl;
    };

    

#ifdef COBJMACROS


#define _dispDebuggerProcessEvents_QueryInterface(This,riid,ppvObject)	\
    ( (This)->lpVtbl -> QueryInterface(This,riid,ppvObject) ) 

#define _dispDebuggerProcessEvents_AddRef(This)	\
    ( (This)->lpVtbl -> AddRef(This) ) 

#define _dispDebuggerProcessEvents_Release(This)	\
    ( (This)->lpVtbl -> Release(This) ) 


#define _dispDebuggerProcessEvents_GetTypeInfoCount(This,pctinfo)	\
    ( (This)->lpVtbl -> GetTypeInfoCount(This,pctinfo) ) 

#define _dispDebuggerProcessEvents_GetTypeInfo(This,iTInfo,lcid,ppTInfo)	\
    ( (This)->lpVtbl -> GetTypeInfo(This,iTInfo,lcid,ppTInfo) ) 

#define _dispDebuggerProcessEvents_GetIDsOfNames(This,riid,rgszNames,cNames,lcid,rgDispId)	\
    ( (This)->lpVtbl -> GetIDsOfNames(This,riid,rgszNames,cNames,lcid,rgDispId) ) 

#define _dispDebuggerProcessEvents_Invoke(This,dispIdMember,riid,lcid,wFlags,pDispParams,pVarResult,pExcepInfo,puArgErr)	\
    ( (This)->lpVtbl -> Invoke(This,dispIdMember,riid,lcid,wFlags,pDispParams,pVarResult,pExcepInfo,puArgErr) ) 

#endif /* COBJMACROS */


#endif 	/* C style interface */


#endif 	/* ___dispDebuggerProcessEvents_DISPINTERFACE_DEFINED__ */


#ifndef ___DebuggerProcessEventsRoot_INTERFACE_DEFINED__
#define ___DebuggerProcessEventsRoot_INTERFACE_DEFINED__

/* interface _DebuggerProcessEventsRoot */
/* [version][hidden][object][dual][uuid] */ 


EXTERN_C const IID IID__DebuggerProcessEventsRoot;

#if defined(__cplusplus) && !defined(CINTERFACE)
    
    MIDL_INTERFACE("c2eb9070-38b3-404d-bed8-2d5f5d971df5")
    _DebuggerProcessEventsRoot : public IDispatch
    {
    public:
        virtual /* [hidden][propget][id] */ HRESULT __stdcall get_DebuggerProcessEvents( 
            /* [retval][out] */ __RPC__deref_out_opt IDispatch **disp) = 0;
        
    };
    
#else 	/* C style interface */

    typedef struct _DebuggerProcessEventsRootVtbl
    {
        BEGIN_INTERFACE
        
        HRESULT ( STDMETHODCALLTYPE *QueryInterface )( 
            _DebuggerProcessEventsRoot * This,
            /* [in] */ __RPC__in REFIID riid,
            /* [iid_is][out] */ 
            __RPC__deref_out  void **ppvObject);
        
        ULONG ( STDMETHODCALLTYPE *AddRef )( 
            _DebuggerProcessEventsRoot * This);
        
        ULONG ( STDMETHODCALLTYPE *Release )( 
            _DebuggerProcessEventsRoot * This);
        
        HRESULT ( STDMETHODCALLTYPE *GetTypeInfoCount )( 
            _DebuggerProcessEventsRoot * This,
            /* [out] */ __RPC__out UINT *pctinfo);
        
        HRESULT ( STDMETHODCALLTYPE *GetTypeInfo )( 
            _DebuggerProcessEventsRoot * This,
            /* [in] */ UINT iTInfo,
            /* [in] */ LCID lcid,
            /* [out] */ __RPC__deref_out_opt ITypeInfo **ppTInfo);
        
        HRESULT ( STDMETHODCALLTYPE *GetIDsOfNames )( 
            _DebuggerProcessEventsRoot * This,
            /* [in] */ __RPC__in REFIID riid,
            /* [size_is][in] */ __RPC__in_ecount_full(cNames) LPOLESTR *rgszNames,
            /* [range][in] */ UINT cNames,
            /* [in] */ LCID lcid,
            /* [size_is][out] */ __RPC__out_ecount_full(cNames) DISPID *rgDispId);
        
        /* [local] */ HRESULT ( STDMETHODCALLTYPE *Invoke )( 
            _DebuggerProcessEventsRoot * This,
            /* [in] */ DISPID dispIdMember,
            /* [in] */ REFIID riid,
            /* [in] */ LCID lcid,
            /* [in] */ WORD wFlags,
            /* [out][in] */ DISPPARAMS *pDispParams,
            /* [out] */ VARIANT *pVarResult,
            /* [out] */ EXCEPINFO *pExcepInfo,
            /* [out] */ UINT *puArgErr);
        
        /* [hidden][propget][id] */ HRESULT ( __stdcall *get_DebuggerProcessEvents )( 
            _DebuggerProcessEventsRoot * This,
            /* [retval][out] */ __RPC__deref_out_opt IDispatch **disp);
        
        END_INTERFACE
    } _DebuggerProcessEventsRootVtbl;

    interface _DebuggerProcessEventsRoot
    {
        CONST_VTBL struct _DebuggerProcessEventsRootVtbl *lpVtbl;
    };

    

#ifdef COBJMACROS


#define _DebuggerProcessEventsRoot_QueryInterface(This,riid,ppvObject)	\
    ( (This)->lpVtbl -> QueryInterface(This,riid,ppvObject) ) 

#define _DebuggerProcessEventsRoot_AddRef(This)	\
    ( (This)->lpVtbl -> AddRef(This) ) 

#define _DebuggerProcessEventsRoot_Release(This)	\
    ( (This)->lpVtbl -> Release(This) ) 


#define _DebuggerProcessEventsRoot_GetTypeInfoCount(This,pctinfo)	\
    ( (This)->lpVtbl -> GetTypeInfoCount(This,pctinfo) ) 

#define _DebuggerProcessEventsRoot_GetTypeInfo(This,iTInfo,lcid,ppTInfo)	\
    ( (This)->lpVtbl -> GetTypeInfo(This,iTInfo,lcid,ppTInfo) ) 

#define _DebuggerProcessEventsRoot_GetIDsOfNames(This,riid,rgszNames,cNames,lcid,rgDispId)	\
    ( (This)->lpVtbl -> GetIDsOfNames(This,riid,rgszNames,cNames,lcid,rgDispId) ) 

#define _DebuggerProcessEventsRoot_Invoke(This,dispIdMember,riid,lcid,wFlags,pDispParams,pVarResult,pExcepInfo,puArgErr)	\
    ( (This)->lpVtbl -> Invoke(This,dispIdMember,riid,lcid,wFlags,pDispParams,pVarResult,pExcepInfo,puArgErr) ) 


#define _DebuggerProcessEventsRoot_get_DebuggerProcessEvents(This,disp)	\
    ( (This)->lpVtbl -> get_DebuggerProcessEvents(This,disp) ) 

#endif /* COBJMACROS */


#endif 	/* C style interface */




#endif 	/* ___DebuggerProcessEventsRoot_INTERFACE_DEFINED__ */


#ifndef ___DebuggerProcessEvents_INTERFACE_DEFINED__
#define ___DebuggerProcessEvents_INTERFACE_DEFINED__

/* interface _DebuggerProcessEvents */
/* [object][helpstringcontext][helpstring][helpcontext][oleautomation][uuid] */ 


EXTERN_C const IID IID__DebuggerProcessEvents;

#if defined(__cplusplus) && !defined(CINTERFACE)
    
    MIDL_INTERFACE("ce9ddba3-b23b-4c53-bb3e-5471659289d1")
    _DebuggerProcessEvents : public IDispatch
    {
    public:
    };
    
#else 	/* C style interface */

    typedef struct _DebuggerProcessEventsVtbl
    {
        BEGIN_INTERFACE
        
        HRESULT ( STDMETHODCALLTYPE *QueryInterface )( 
            _DebuggerProcessEvents * This,
            /* [in] */ __RPC__in REFIID riid,
            /* [iid_is][out] */ 
            __RPC__deref_out  void **ppvObject);
        
        ULONG ( STDMETHODCALLTYPE *AddRef )( 
            _DebuggerProcessEvents * This);
        
        ULONG ( STDMETHODCALLTYPE *Release )( 
            _DebuggerProcessEvents * This);
        
        HRESULT ( STDMETHODCALLTYPE *GetTypeInfoCount )( 
            _DebuggerProcessEvents * This,
            /* [out] */ __RPC__out UINT *pctinfo);
        
        HRESULT ( STDMETHODCALLTYPE *GetTypeInfo )( 
            _DebuggerProcessEvents * This,
            /* [in] */ UINT iTInfo,
            /* [in] */ LCID lcid,
            /* [out] */ __RPC__deref_out_opt ITypeInfo **ppTInfo);
        
        HRESULT ( STDMETHODCALLTYPE *GetIDsOfNames )( 
            _DebuggerProcessEvents * This,
            /* [in] */ __RPC__in REFIID riid,
            /* [size_is][in] */ __RPC__in_ecount_full(cNames) LPOLESTR *rgszNames,
            /* [range][in] */ UINT cNames,
            /* [in] */ LCID lcid,
            /* [size_is][out] */ __RPC__out_ecount_full(cNames) DISPID *rgDispId);
        
        /* [local] */ HRESULT ( STDMETHODCALLTYPE *Invoke )( 
            _DebuggerProcessEvents * This,
            /* [in] */ DISPID dispIdMember,
            /* [in] */ REFIID riid,
            /* [in] */ LCID lcid,
            /* [in] */ WORD wFlags,
            /* [out][in] */ DISPPARAMS *pDispParams,
            /* [out] */ VARIANT *pVarResult,
            /* [out] */ EXCEPINFO *pExcepInfo,
            /* [out] */ UINT *puArgErr);
        
        END_INTERFACE
    } _DebuggerProcessEventsVtbl;

    interface _DebuggerProcessEvents
    {
        CONST_VTBL struct _DebuggerProcessEventsVtbl *lpVtbl;
    };

    

#ifdef COBJMACROS


#define _DebuggerProcessEvents_QueryInterface(This,riid,ppvObject)	\
    ( (This)->lpVtbl -> QueryInterface(This,riid,ppvObject) ) 

#define _DebuggerProcessEvents_AddRef(This)	\
    ( (This)->lpVtbl -> AddRef(This) ) 

#define _DebuggerProcessEvents_Release(This)	\
    ( (This)->lpVtbl -> Release(This) ) 


#define _DebuggerProcessEvents_GetTypeInfoCount(This,pctinfo)	\
    ( (This)->lpVtbl -> GetTypeInfoCount(This,pctinfo) ) 

#define _DebuggerProcessEvents_GetTypeInfo(This,iTInfo,lcid,ppTInfo)	\
    ( (This)->lpVtbl -> GetTypeInfo(This,iTInfo,lcid,ppTInfo) ) 

#define _DebuggerProcessEvents_GetIDsOfNames(This,riid,rgszNames,cNames,lcid,rgDispId)	\
    ( (This)->lpVtbl -> GetIDsOfNames(This,riid,rgszNames,cNames,lcid,rgDispId) ) 

#define _DebuggerProcessEvents_Invoke(This,dispIdMember,riid,lcid,wFlags,pDispParams,pVarResult,pExcepInfo,puArgErr)	\
    ( (This)->lpVtbl -> Invoke(This,dispIdMember,riid,lcid,wFlags,pDispParams,pVarResult,pExcepInfo,puArgErr) ) 


#endif /* COBJMACROS */


#endif 	/* C style interface */




#endif 	/* ___DebuggerProcessEvents_INTERFACE_DEFINED__ */


EXTERN_C const CLSID CLSID_DebuggerProcessEvents;

#ifdef __cplusplus

class DECLSPEC_UUID("d14b8b85-e4a8-42f8-b486-8a14dc74adeb")
DebuggerProcessEvents;
#endif

#ifndef ___dispDebuggerExpressionEvaluationEvents_DISPINTERFACE_DEFINED__
#define ___dispDebuggerExpressionEvaluationEvents_DISPINTERFACE_DEFINED__

/* dispinterface _dispDebuggerExpressionEvaluationEvents */
/* [version][uuid] */ 


EXTERN_C const IID DIID__dispDebuggerExpressionEvaluationEvents;

#if defined(__cplusplus) && !defined(CINTERFACE)

    MIDL_INTERFACE("73ffa13f-ad72-4154-be77-d4288f2e4fc5")
    _dispDebuggerExpressionEvaluationEvents : public IDispatch
    {
    };
    
#else 	/* C style interface */

    typedef struct _dispDebuggerExpressionEvaluationEventsVtbl
    {
        BEGIN_INTERFACE
        
        HRESULT ( STDMETHODCALLTYPE *QueryInterface )( 
            _dispDebuggerExpressionEvaluationEvents * This,
            /* [in] */ __RPC__in REFIID riid,
            /* [iid_is][out] */ 
            __RPC__deref_out  void **ppvObject);
        
        ULONG ( STDMETHODCALLTYPE *AddRef )( 
            _dispDebuggerExpressionEvaluationEvents * This);
        
        ULONG ( STDMETHODCALLTYPE *Release )( 
            _dispDebuggerExpressionEvaluationEvents * This);
        
        HRESULT ( STDMETHODCALLTYPE *GetTypeInfoCount )( 
            _dispDebuggerExpressionEvaluationEvents * This,
            /* [out] */ __RPC__out UINT *pctinfo);
        
        HRESULT ( STDMETHODCALLTYPE *GetTypeInfo )( 
            _dispDebuggerExpressionEvaluationEvents * This,
            /* [in] */ UINT iTInfo,
            /* [in] */ LCID lcid,
            /* [out] */ __RPC__deref_out_opt ITypeInfo **ppTInfo);
        
        HRESULT ( STDMETHODCALLTYPE *GetIDsOfNames )( 
            _dispDebuggerExpressionEvaluationEvents * This,
            /* [in] */ __RPC__in REFIID riid,
            /* [size_is][in] */ __RPC__in_ecount_full(cNames) LPOLESTR *rgszNames,
            /* [range][in] */ UINT cNames,
            /* [in] */ LCID lcid,
            /* [size_is][out] */ __RPC__out_ecount_full(cNames) DISPID *rgDispId);
        
        /* [local] */ HRESULT ( STDMETHODCALLTYPE *Invoke )( 
            _dispDebuggerExpressionEvaluationEvents * This,
            /* [in] */ DISPID dispIdMember,
            /* [in] */ REFIID riid,
            /* [in] */ LCID lcid,
            /* [in] */ WORD wFlags,
            /* [out][in] */ DISPPARAMS *pDispParams,
            /* [out] */ VARIANT *pVarResult,
            /* [out] */ EXCEPINFO *pExcepInfo,
            /* [out] */ UINT *puArgErr);
        
        END_INTERFACE
    } _dispDebuggerExpressionEvaluationEventsVtbl;

    interface _dispDebuggerExpressionEvaluationEvents
    {
        CONST_VTBL struct _dispDebuggerExpressionEvaluationEventsVtbl *lpVtbl;
    };

    

#ifdef COBJMACROS


#define _dispDebuggerExpressionEvaluationEvents_QueryInterface(This,riid,ppvObject)	\
    ( (This)->lpVtbl -> QueryInterface(This,riid,ppvObject) ) 

#define _dispDebuggerExpressionEvaluationEvents_AddRef(This)	\
    ( (This)->lpVtbl -> AddRef(This) ) 

#define _dispDebuggerExpressionEvaluationEvents_Release(This)	\
    ( (This)->lpVtbl -> Release(This) ) 


#define _dispDebuggerExpressionEvaluationEvents_GetTypeInfoCount(This,pctinfo)	\
    ( (This)->lpVtbl -> GetTypeInfoCount(This,pctinfo) ) 

#define _dispDebuggerExpressionEvaluationEvents_GetTypeInfo(This,iTInfo,lcid,ppTInfo)	\
    ( (This)->lpVtbl -> GetTypeInfo(This,iTInfo,lcid,ppTInfo) ) 

#define _dispDebuggerExpressionEvaluationEvents_GetIDsOfNames(This,riid,rgszNames,cNames,lcid,rgDispId)	\
    ( (This)->lpVtbl -> GetIDsOfNames(This,riid,rgszNames,cNames,lcid,rgDispId) ) 

#define _dispDebuggerExpressionEvaluationEvents_Invoke(This,dispIdMember,riid,lcid,wFlags,pDispParams,pVarResult,pExcepInfo,puArgErr)	\
    ( (This)->lpVtbl -> Invoke(This,dispIdMember,riid,lcid,wFlags,pDispParams,pVarResult,pExcepInfo,puArgErr) ) 

#endif /* COBJMACROS */


#endif 	/* C style interface */


#endif 	/* ___dispDebuggerExpressionEvaluationEvents_DISPINTERFACE_DEFINED__ */


#ifndef ___DebuggerExpressionEvaluationEvents_INTERFACE_DEFINED__
#define ___DebuggerExpressionEvaluationEvents_INTERFACE_DEFINED__

/* interface _DebuggerExpressionEvaluationEvents */
/* [object][helpstringcontext][helpstring][helpcontext][oleautomation][uuid] */ 


EXTERN_C const IID IID__DebuggerExpressionEvaluationEvents;

#if defined(__cplusplus) && !defined(CINTERFACE)
    
    MIDL_INTERFACE("6973a466-e09c-4195-9ae8-07b7aaff7d85")
    _DebuggerExpressionEvaluationEvents : public IDispatch
    {
    public:
    };
    
#else 	/* C style interface */

    typedef struct _DebuggerExpressionEvaluationEventsVtbl
    {
        BEGIN_INTERFACE
        
        HRESULT ( STDMETHODCALLTYPE *QueryInterface )( 
            _DebuggerExpressionEvaluationEvents * This,
            /* [in] */ __RPC__in REFIID riid,
            /* [iid_is][out] */ 
            __RPC__deref_out  void **ppvObject);
        
        ULONG ( STDMETHODCALLTYPE *AddRef )( 
            _DebuggerExpressionEvaluationEvents * This);
        
        ULONG ( STDMETHODCALLTYPE *Release )( 
            _DebuggerExpressionEvaluationEvents * This);
        
        HRESULT ( STDMETHODCALLTYPE *GetTypeInfoCount )( 
            _DebuggerExpressionEvaluationEvents * This,
            /* [out] */ __RPC__out UINT *pctinfo);
        
        HRESULT ( STDMETHODCALLTYPE *GetTypeInfo )( 
            _DebuggerExpressionEvaluationEvents * This,
            /* [in] */ UINT iTInfo,
            /* [in] */ LCID lcid,
            /* [out] */ __RPC__deref_out_opt ITypeInfo **ppTInfo);
        
        HRESULT ( STDMETHODCALLTYPE *GetIDsOfNames )( 
            _DebuggerExpressionEvaluationEvents * This,
            /* [in] */ __RPC__in REFIID riid,
            /* [size_is][in] */ __RPC__in_ecount_full(cNames) LPOLESTR *rgszNames,
            /* [range][in] */ UINT cNames,
            /* [in] */ LCID lcid,
            /* [size_is][out] */ __RPC__out_ecount_full(cNames) DISPID *rgDispId);
        
        /* [local] */ HRESULT ( STDMETHODCALLTYPE *Invoke )( 
            _DebuggerExpressionEvaluationEvents * This,
            /* [in] */ DISPID dispIdMember,
            /* [in] */ REFIID riid,
            /* [in] */ LCID lcid,
            /* [in] */ WORD wFlags,
            /* [out][in] */ DISPPARAMS *pDispParams,
            /* [out] */ VARIANT *pVarResult,
            /* [out] */ EXCEPINFO *pExcepInfo,
            /* [out] */ UINT *puArgErr);
        
        END_INTERFACE
    } _DebuggerExpressionEvaluationEventsVtbl;

    interface _DebuggerExpressionEvaluationEvents
    {
        CONST_VTBL struct _DebuggerExpressionEvaluationEventsVtbl *lpVtbl;
    };

    

#ifdef COBJMACROS


#define _DebuggerExpressionEvaluationEvents_QueryInterface(This,riid,ppvObject)	\
    ( (This)->lpVtbl -> QueryInterface(This,riid,ppvObject) ) 

#define _DebuggerExpressionEvaluationEvents_AddRef(This)	\
    ( (This)->lpVtbl -> AddRef(This) ) 

#define _DebuggerExpressionEvaluationEvents_Release(This)	\
    ( (This)->lpVtbl -> Release(This) ) 


#define _DebuggerExpressionEvaluationEvents_GetTypeInfoCount(This,pctinfo)	\
    ( (This)->lpVtbl -> GetTypeInfoCount(This,pctinfo) ) 

#define _DebuggerExpressionEvaluationEvents_GetTypeInfo(This,iTInfo,lcid,ppTInfo)	\
    ( (This)->lpVtbl -> GetTypeInfo(This,iTInfo,lcid,ppTInfo) ) 

#define _DebuggerExpressionEvaluationEvents_GetIDsOfNames(This,riid,rgszNames,cNames,lcid,rgDispId)	\
    ( (This)->lpVtbl -> GetIDsOfNames(This,riid,rgszNames,cNames,lcid,rgDispId) ) 

#define _DebuggerExpressionEvaluationEvents_Invoke(This,dispIdMember,riid,lcid,wFlags,pDispParams,pVarResult,pExcepInfo,puArgErr)	\
    ( (This)->lpVtbl -> Invoke(This,dispIdMember,riid,lcid,wFlags,pDispParams,pVarResult,pExcepInfo,puArgErr) ) 


#endif /* COBJMACROS */


#endif 	/* C style interface */




#endif 	/* ___DebuggerExpressionEvaluationEvents_INTERFACE_DEFINED__ */


#ifndef ___DebuggerExpressionEvaluationEventsRoot_INTERFACE_DEFINED__
#define ___DebuggerExpressionEvaluationEventsRoot_INTERFACE_DEFINED__

/* interface _DebuggerExpressionEvaluationEventsRoot */
/* [version][hidden][object][dual][uuid] */ 


EXTERN_C const IID IID__DebuggerExpressionEvaluationEventsRoot;

#if defined(__cplusplus) && !defined(CINTERFACE)
    
    MIDL_INTERFACE("45ae9835-6090-4cca-97e0-5ea9a5608fbe")
    _DebuggerExpressionEvaluationEventsRoot : public IDispatch
    {
    public:
        virtual /* [hidden][propget][id] */ HRESULT __stdcall get_DebuggerExpressionEvaluationEvents( 
            /* [retval][out] */ __RPC__deref_out_opt IDispatch **disp) = 0;
        
    };
    
#else 	/* C style interface */

    typedef struct _DebuggerExpressionEvaluationEventsRootVtbl
    {
        BEGIN_INTERFACE
        
        HRESULT ( STDMETHODCALLTYPE *QueryInterface )( 
            _DebuggerExpressionEvaluationEventsRoot * This,
            /* [in] */ __RPC__in REFIID riid,
            /* [iid_is][out] */ 
            __RPC__deref_out  void **ppvObject);
        
        ULONG ( STDMETHODCALLTYPE *AddRef )( 
            _DebuggerExpressionEvaluationEventsRoot * This);
        
        ULONG ( STDMETHODCALLTYPE *Release )( 
            _DebuggerExpressionEvaluationEventsRoot * This);
        
        HRESULT ( STDMETHODCALLTYPE *GetTypeInfoCount )( 
            _DebuggerExpressionEvaluationEventsRoot * This,
            /* [out] */ __RPC__out UINT *pctinfo);
        
        HRESULT ( STDMETHODCALLTYPE *GetTypeInfo )( 
            _DebuggerExpressionEvaluationEventsRoot * This,
            /* [in] */ UINT iTInfo,
            /* [in] */ LCID lcid,
            /* [out] */ __RPC__deref_out_opt ITypeInfo **ppTInfo);
        
        HRESULT ( STDMETHODCALLTYPE *GetIDsOfNames )( 
            _DebuggerExpressionEvaluationEventsRoot * This,
            /* [in] */ __RPC__in REFIID riid,
            /* [size_is][in] */ __RPC__in_ecount_full(cNames) LPOLESTR *rgszNames,
            /* [range][in] */ UINT cNames,
            /* [in] */ LCID lcid,
            /* [size_is][out] */ __RPC__out_ecount_full(cNames) DISPID *rgDispId);
        
        /* [local] */ HRESULT ( STDMETHODCALLTYPE *Invoke )( 
            _DebuggerExpressionEvaluationEventsRoot * This,
            /* [in] */ DISPID dispIdMember,
            /* [in] */ REFIID riid,
            /* [in] */ LCID lcid,
            /* [in] */ WORD wFlags,
            /* [out][in] */ DISPPARAMS *pDispParams,
            /* [out] */ VARIANT *pVarResult,
            /* [out] */ EXCEPINFO *pExcepInfo,
            /* [out] */ UINT *puArgErr);
        
        /* [hidden][propget][id] */ HRESULT ( __stdcall *get_DebuggerExpressionEvaluationEvents )( 
            _DebuggerExpressionEvaluationEventsRoot * This,
            /* [retval][out] */ __RPC__deref_out_opt IDispatch **disp);
        
        END_INTERFACE
    } _DebuggerExpressionEvaluationEventsRootVtbl;

    interface _DebuggerExpressionEvaluationEventsRoot
    {
        CONST_VTBL struct _DebuggerExpressionEvaluationEventsRootVtbl *lpVtbl;
    };

    

#ifdef COBJMACROS


#define _DebuggerExpressionEvaluationEventsRoot_QueryInterface(This,riid,ppvObject)	\
    ( (This)->lpVtbl -> QueryInterface(This,riid,ppvObject) ) 

#define _DebuggerExpressionEvaluationEventsRoot_AddRef(This)	\
    ( (This)->lpVtbl -> AddRef(This) ) 

#define _DebuggerExpressionEvaluationEventsRoot_Release(This)	\
    ( (This)->lpVtbl -> Release(This) ) 


#define _DebuggerExpressionEvaluationEventsRoot_GetTypeInfoCount(This,pctinfo)	\
    ( (This)->lpVtbl -> GetTypeInfoCount(This,pctinfo) ) 

#define _DebuggerExpressionEvaluationEventsRoot_GetTypeInfo(This,iTInfo,lcid,ppTInfo)	\
    ( (This)->lpVtbl -> GetTypeInfo(This,iTInfo,lcid,ppTInfo) ) 

#define _DebuggerExpressionEvaluationEventsRoot_GetIDsOfNames(This,riid,rgszNames,cNames,lcid,rgDispId)	\
    ( (This)->lpVtbl -> GetIDsOfNames(This,riid,rgszNames,cNames,lcid,rgDispId) ) 

#define _DebuggerExpressionEvaluationEventsRoot_Invoke(This,dispIdMember,riid,lcid,wFlags,pDispParams,pVarResult,pExcepInfo,puArgErr)	\
    ( (This)->lpVtbl -> Invoke(This,dispIdMember,riid,lcid,wFlags,pDispParams,pVarResult,pExcepInfo,puArgErr) ) 


#define _DebuggerExpressionEvaluationEventsRoot_get_DebuggerExpressionEvaluationEvents(This,disp)	\
    ( (This)->lpVtbl -> get_DebuggerExpressionEvaluationEvents(This,disp) ) 

#endif /* COBJMACROS */


#endif 	/* C style interface */




#endif 	/* ___DebuggerExpressionEvaluationEventsRoot_INTERFACE_DEFINED__ */


EXTERN_C const CLSID CLSID_DebuggerExpressionEvaluationEvents;

#ifdef __cplusplus

class DECLSPEC_UUID("1aa6b3e4-42f7-4396-80fa-76728d1b824c")
DebuggerExpressionEvaluationEvents;
#endif


#ifndef __ContextGuids_MODULE_DEFINED__
#define __ContextGuids_MODULE_DEFINED__


/* module ContextGuids */
/* [helpstring][dllname][uuid] */ 

const LPSTR vsContextGuidSolutionBuilding	=	"{ADFC4E60-0397-11D1-9F4E-00A0C911004F}";

const LPSTR vsContextGuidDebugging	=	"{ADFC4E61-0397-11D1-9F4E-00A0C911004F}";

const LPSTR vsContextGuidUIHierarchyDragging	=	"{B706F393-2E5B-49E7-9E2E-B1825F639B63}";

const LPSTR vsContextGuidFullScreenMode	=	"{ADFC4E62-0397-11D1-9F4E-00A0C911004F}";

const LPSTR vsContextGuidDesignMode	=	"{ADFC4E63-0397-11D1-9F4E-00A0C911004F}";

const LPSTR vsContextGuidNoSolution	=	"{ADFC4E64-0397-11D1-9F4E-00A0C911004F}";

const LPSTR vsContextGuidSolutionExists	=	"{F1536EF8-92EC-443C-9ED7-FDADF150DA82}";

const LPSTR vsContextGuidEmptySolution	=	"{ADFC4E65-0397-11D1-9F4E-00A0C911004F}";

const LPSTR vsContextGuidSolutionHasSingleProject	=	"{ADFC4E66-0397-11D1-9F4E-00A0C911004F}";

const LPSTR vsContextGuidSolutionHasMultipleProjects	=	"{93694FA0-0397-11D1-9F4E-00A0C911004F}";

const LPSTR vsContextGuidCodeWindow	=	"{8FE2DF1D-E0DA-4EBE-9D5C-415D40E487B5}";

const LPSTR vsContextGuidNotBuildingAndNotDebugging	=	"{48EA4A80-F14E-4107-88FA-8D0016F30B9C}";

const LPSTR vsContextGuidSolutionOrProjectUpgrading	=	"{EF4F870B-7B85-4F29-9D15-CE1ABFBE733B}";

const LPSTR vsContextGuidDataSourceWindowSupported	=	"{95C314C4-660B-4627-9F82-1BAF1C764BBF}";

const LPSTR vsContextGuidDataSourceWindowAutoVisible	=	"{2E78870D-AC7C-4460-A4A1-3FE37D00EF81}";

const LPSTR vsContextGuidWindowsFormsDesigner	=	"{BA09E2AF-9DF2-4068-B2F0-4C7E5CC19E2F}";

const LPSTR vsContextGuidToolboxInitialized	=	"{DC5DB425-F0FD-4403-96A1-F475CDBA9EE0}";

const LPSTR vsContextGuidSolutionExistsAndNotBuildingAndNotDebugging	=	"{D0E4DEEC-1B53-4CDA-8559-D454583AD23B}";

const LPSTR vsContextGuidTextEditor	=	"{8B382828-6202-11D1-8870-0000F87579D2}";

const LPSTR vsContextGuidXMLTextEditor	=	"{F6819A78-A205-47B5-BE1C-675B3C7F0B8E}";

const LPSTR vsContextGuidCSSTextEditor	=	"{A764E898-518D-11D2-9A89-00C04F79EFC3}";

const LPSTR vsContextGuidHTMLSourceEditor	=	"{58E975A0-F8FE-11D2-A6AE-00104BCC7269}";

const LPSTR vsContextGuidHTMLDesignView	=	"{CB3FCFEA-03DF-11D1-81D2-00A0C91BBEE3}";

const LPSTR vsContextGuidHTMLSourceView	=	"{CB3FCFEB-03DF-11D1-81D2-00A0C91BBEE3}";

const LPSTR vsContextGuidHTMLCodeView	=	"{4C01CBEE-FB8C-4ED0-8EC0-68348C52822E}";

const LPSTR vsContextGuidFrames	=	"{CB3FCFEC-03DF-11D1-81D2-00A0C91BBEE3}";

const LPSTR vsContextGuidSchema	=	"{E6631B5B-2EAB-41E8-82FD-6469645C76C9}";

const LPSTR vsContextGuidData	=	"{F482F8AF-1E66-4760-919E-964707265994}";

const LPSTR vsContextGuidKindStartPage	=	"{387CB18D-6153-4156-9257-9AC3F9207BBE}";

const LPSTR vsContextGuidCommunityWindow	=	"{96DB1F3B-0E7A-4406-B73E-C6F0A2C67B97}";

const LPSTR vsContextGuidDeviceExplorer	=	"{B65E9355-A4C7-4855-96BB-1D3EC8514E8F}";

const LPSTR vsContextGuidBookmarks	=	"{A0C5197D-0AC7-4B63-97CD-8872A789D233}";

const LPSTR vsContextGuidApplicationBrowser	=	"{399832EA-70A8-4AE7-9B99-3C0850DAD152}";

const LPSTR vsContextGuidFavorites	=	"{57DC5D59-11C2-4955-A7B4-D7699D677E93}";

const LPSTR vsContextGuidErrorList	=	"{D78612C7-9962-4B83-95D9-268046DAD23A}";

const LPSTR vsContextGuidHelpSearch	=	"{46C87F81-5A06-43A8-9E25-85D33BAC49F8}";

const LPSTR vsContextGuidHelpIndex	=	"{73F6DD58-437E-11D3-B88E-00C04F79F802}";

const LPSTR vsContextGuidHelpContents	=	"{4A791147-19E4-11D3-B86B-00C04F79F802}";

const LPSTR vsContextGuidCallBrowser	=	"{5415EA3A-D813-4948-B51E-562082CE0887}";

const LPSTR vsContextGuidCodeDefinition	=	"{588470CC-84F8-4A57-9AC4-86BCA0625FF4}";

const LPSTR vsContextGuidTaskList	=	"{4A9B7E51-AA16-11D0-A8C5-00A0C921A4D2}";

const LPSTR vsContextGuidToolbox	=	"{B1E99781-AB81-11D0-B683-00AA00A3EE26}";

const LPSTR vsContextGuidCallStack	=	"{0504FF91-9D61-11D0-A794-00A0C9110051}";

const LPSTR vsContextGuidThread	=	"{E62CE6A0-B439-11D0-A79D-00A0C9110051}";

const LPSTR vsContextGuidLocals	=	"{4A18F9D0-B838-11D0-93EB-00A0C90F2734}";

const LPSTR vsContextGuidAutoLocals	=	"{F2E84780-2AF1-11D1-A7FA-00A0C9110051}";

const LPSTR vsContextGuidWatch	=	"{90243340-BD7A-11D0-93EF-00A0C90F2734}";

const LPSTR vsContextGuidProperties	=	"{EEFA5220-E298-11D0-8F78-00A0C9110057}";

const LPSTR vsContextGuidSolutionExplorer	=	"{3AE79031-E1BC-11D0-8F78-00A0C9110057}";

const LPSTR vsContextGuidOutput	=	"{34E76E81-EE4A-11D0-AE2E-00A0C90FFFC3}";

const LPSTR vsContextGuidObjectBrowser	=	"{269A02DC-6AF8-11D3-BDC4-00C04F688E50}";

const LPSTR vsContextGuidMacroExplorer	=	"{07CD18B4-3BA1-11D2-890A-0060083196C6}";

const LPSTR vsContextGuidDynamicHelp	=	"{66DBA47C-61DF-11D2-AA79-00C04F990343}";

const LPSTR vsContextGuidClassView	=	"{C9C0AE26-AA77-11D2-B3F0-0000F87570EE}";

const LPSTR vsContextGuidResourceView	=	"{2D7728C2-DE0A-45B5-99AA-89B609DFDE73}";

const LPSTR vsContextGuidDocumentOutline	=	"{25F7E850-FFA1-11D0-B63F-00A0C922E851}";

const LPSTR vsContextGuidServerExplorer	=	"{74946827-37A0-11D2-A273-00C04F8EF4FF}";

const LPSTR vsContextGuidCommandWindow	=	"{28836128-FC2C-11D2-A433-00C04F72D18A}";

const LPSTR vsContextGuidFindSymbol	=	"{53024D34-0EF5-11D3-87E0-00C04F7971A5}";

const LPSTR vsContextGuidFindSymbolResults	=	"{68487888-204A-11D3-87EB-00C04F7971A5}";

const LPSTR vsContextGuidFindReplace	=	"{CF2DDC32-8CAD-11D2-9302-005345000000}";

const LPSTR vsContextGuidFindResults1	=	"{0F887920-C2B6-11D2-9375-0080C747D9A0}";

const LPSTR vsContextGuidFindResults2	=	"{0F887921-C2B6-11D2-9375-0080C747D9A0}";

const LPSTR vsContextGuidMainWindow	=	"{9DDABE98-1D02-11D3-89A1-00C04F688DDE}";

const LPSTR vsContextGuidLinkedWindowFrame	=	"{9DDABE99-1D02-11D3-89A1-00C04F688DDE}";

const LPSTR vsContextGuidWebBrowser	=	"{E8B06F52-6D01-11D2-AA7D-00C04F990343}";

#endif /* __ContextGuids_MODULE_DEFINED__ */


#ifndef __WindowKinds_MODULE_DEFINED__
#define __WindowKinds_MODULE_DEFINED__


/* module WindowKinds */
/* [helpstringcontext][helpstring][helpcontext][dllname][uuid] */ 

const LPSTR vsWindowKindKindStartPage	=	"{387CB18D-6153-4156-9257-9AC3F9207BBE}";

const LPSTR vsWindowKindCommunityWindow	=	"{96DB1F3B-0E7A-4406-B73E-C6F0A2C67B97}";

const LPSTR vsWindowKindDeviceExplorer	=	"{B65E9355-A4C7-4855-96BB-1D3EC8514E8F}";

const LPSTR vsWindowKindBookmarks	=	"{A0C5197D-0AC7-4B63-97CD-8872A789D233}";

const LPSTR vsWindowKindApplicationBrowser	=	"{399832EA-70A8-4AE7-9B99-3C0850DAD152}";

const LPSTR vsWindowKindFavorites	=	"{57DC5D59-11C2-4955-A7B4-D7699D677E93}";

const LPSTR vsWindowKindErrorList	=	"{D78612C7-9962-4B83-95D9-268046DAD23A}";

const LPSTR vsWindowKindHelpSearch	=	"{46C87F81-5A06-43A8-9E25-85D33BAC49F8}";

const LPSTR vsWindowKindHelpIndex	=	"{73F6DD58-437E-11D3-B88E-00C04F79F802}";

const LPSTR vsWindowKindHelpContents	=	"{4A791147-19E4-11D3-B86B-00C04F79F802}";

const LPSTR vsWindowKindCallBrowser	=	"{5415EA3A-D813-4948-B51E-562082CE0887}";

const LPSTR vsWindowKindCodeDefinition	=	"{588470CC-84F8-4A57-9AC4-86BCA0625FF4}";

const LPSTR vsWindowKindImmediate	=	"{28836128-FC2C-11D2-A433-00C04F72D18A}";

#endif /* __WindowKinds_MODULE_DEFINED__ */


#ifndef __ProjectKinds_MODULE_DEFINED__
#define __ProjectKinds_MODULE_DEFINED__


/* module ProjectKinds */
/* [helpstringcontext][helpstring][helpcontext][dllname][uuid] */ 

const LPSTR vsProjectKindSolutionFolder	=	"{66A26720-8FB5-11D2-AA7E-00C04F688DDE}";

#endif /* __ProjectKinds_MODULE_DEFINED__ */

#ifndef __ToolWindows_INTERFACE_DEFINED__
#define __ToolWindows_INTERFACE_DEFINED__

/* interface ToolWindows */
/* [helpstringcontext][helpstring][helpcontext][uuid][object][oleautomation][dual] */ 


EXTERN_C const IID IID_ToolWindows;

#if defined(__cplusplus) && !defined(CINTERFACE)
    
    MIDL_INTERFACE("19AC6F68-3019-4D65-8D98-404DFB96B8E2")
    ToolWindows : public IDispatch
    {
    public:
        virtual /* [helpstringcontext][helpstring][helpcontext][propget][id] */ HRESULT STDMETHODCALLTYPE get_DTE( 
            /* [retval][out] */ __RPC__deref_out_opt /* external definition not present */ DTE **DTEObject) = 0;
        
        virtual /* [helpstringcontext][helpstring][helpcontext][id] */ HRESULT STDMETHODCALLTYPE GetToolWindow( 
            __RPC__in BSTR Name,
            /* [retval][out] */ __RPC__deref_out_opt IDispatch **ppObject) = 0;
        
        virtual /* [helpstringcontext][helpstring][helpcontext][propget][id] */ HRESULT STDMETHODCALLTYPE get_ToolBox( 
            /* [retval][out] */ __RPC__deref_out_opt /* external definition not present */ ToolBox **ppToolBox) = 0;
        
        virtual /* [helpstringcontext][helpstring][helpcontext][propget][id] */ HRESULT STDMETHODCALLTYPE get_CommandWindow( 
            /* [retval][out] */ __RPC__deref_out_opt /* external definition not present */ CommandWindow **ppOutputWindow) = 0;
        
        virtual /* [helpstringcontext][helpstring][helpcontext][propget][id] */ HRESULT STDMETHODCALLTYPE get_OutputWindow( 
            /* [retval][out] */ __RPC__deref_out_opt /* external definition not present */ OutputWindow **ppOutputWindow) = 0;
        
        virtual /* [helpstringcontext][helpstring][helpcontext][propget][id] */ HRESULT STDMETHODCALLTYPE get_SolutionExplorer( 
            /* [retval][out] */ __RPC__deref_out_opt /* external definition not present */ UIHierarchy **ppUIHierarchy) = 0;
        
        virtual /* [helpstringcontext][helpstring][helpcontext][propget][id] */ HRESULT STDMETHODCALLTYPE get_TaskList( 
            /* [retval][out] */ __RPC__deref_out_opt /* external definition not present */ TaskList **ppTaskList) = 0;
        
        virtual /* [helpstringcontext][helpstring][helpcontext][propget][id] */ HRESULT STDMETHODCALLTYPE get_ErrorList( 
            /* [retval][out] */ __RPC__deref_out_opt ErrorList **ppErrorList) = 0;
        
    };
    
#else 	/* C style interface */

    typedef struct ToolWindowsVtbl
    {
        BEGIN_INTERFACE
        
        HRESULT ( STDMETHODCALLTYPE *QueryInterface )( 
            ToolWindows * This,
            /* [in] */ __RPC__in REFIID riid,
            /* [iid_is][out] */ 
            __RPC__deref_out  void **ppvObject);
        
        ULONG ( STDMETHODCALLTYPE *AddRef )( 
            ToolWindows * This);
        
        ULONG ( STDMETHODCALLTYPE *Release )( 
            ToolWindows * This);
        
        HRESULT ( STDMETHODCALLTYPE *GetTypeInfoCount )( 
            ToolWindows * This,
            /* [out] */ __RPC__out UINT *pctinfo);
        
        HRESULT ( STDMETHODCALLTYPE *GetTypeInfo )( 
            ToolWindows * This,
            /* [in] */ UINT iTInfo,
            /* [in] */ LCID lcid,
            /* [out] */ __RPC__deref_out_opt ITypeInfo **ppTInfo);
        
        HRESULT ( STDMETHODCALLTYPE *GetIDsOfNames )( 
            ToolWindows * This,
            /* [in] */ __RPC__in REFIID riid,
            /* [size_is][in] */ __RPC__in_ecount_full(cNames) LPOLESTR *rgszNames,
            /* [range][in] */ UINT cNames,
            /* [in] */ LCID lcid,
            /* [size_is][out] */ __RPC__out_ecount_full(cNames) DISPID *rgDispId);
        
        /* [local] */ HRESULT ( STDMETHODCALLTYPE *Invoke )( 
            ToolWindows * This,
            /* [in] */ DISPID dispIdMember,
            /* [in] */ REFIID riid,
            /* [in] */ LCID lcid,
            /* [in] */ WORD wFlags,
            /* [out][in] */ DISPPARAMS *pDispParams,
            /* [out] */ VARIANT *pVarResult,
            /* [out] */ EXCEPINFO *pExcepInfo,
            /* [out] */ UINT *puArgErr);
        
        /* [helpstringcontext][helpstring][helpcontext][propget][id] */ HRESULT ( STDMETHODCALLTYPE *get_DTE )( 
            ToolWindows * This,
            /* [retval][out] */ __RPC__deref_out_opt /* external definition not present */ DTE **DTEObject);
        
        /* [helpstringcontext][helpstring][helpcontext][id] */ HRESULT ( STDMETHODCALLTYPE *GetToolWindow )( 
            ToolWindows * This,
            __RPC__in BSTR Name,
            /* [retval][out] */ __RPC__deref_out_opt IDispatch **ppObject);
        
        /* [helpstringcontext][helpstring][helpcontext][propget][id] */ HRESULT ( STDMETHODCALLTYPE *get_ToolBox )( 
            ToolWindows * This,
            /* [retval][out] */ __RPC__deref_out_opt /* external definition not present */ ToolBox **ppToolBox);
        
        /* [helpstringcontext][helpstring][helpcontext][propget][id] */ HRESULT ( STDMETHODCALLTYPE *get_CommandWindow )( 
            ToolWindows * This,
            /* [retval][out] */ __RPC__deref_out_opt /* external definition not present */ CommandWindow **ppOutputWindow);
        
        /* [helpstringcontext][helpstring][helpcontext][propget][id] */ HRESULT ( STDMETHODCALLTYPE *get_OutputWindow )( 
            ToolWindows * This,
            /* [retval][out] */ __RPC__deref_out_opt /* external definition not present */ OutputWindow **ppOutputWindow);
        
        /* [helpstringcontext][helpstring][helpcontext][propget][id] */ HRESULT ( STDMETHODCALLTYPE *get_SolutionExplorer )( 
            ToolWindows * This,
            /* [retval][out] */ __RPC__deref_out_opt /* external definition not present */ UIHierarchy **ppUIHierarchy);
        
        /* [helpstringcontext][helpstring][helpcontext][propget][id] */ HRESULT ( STDMETHODCALLTYPE *get_TaskList )( 
            ToolWindows * This,
            /* [retval][out] */ __RPC__deref_out_opt /* external definition not present */ TaskList **ppTaskList);
        
        /* [helpstringcontext][helpstring][helpcontext][propget][id] */ HRESULT ( STDMETHODCALLTYPE *get_ErrorList )( 
            ToolWindows * This,
            /* [retval][out] */ __RPC__deref_out_opt ErrorList **ppErrorList);
        
        END_INTERFACE
    } ToolWindowsVtbl;

    interface ToolWindows
    {
        CONST_VTBL struct ToolWindowsVtbl *lpVtbl;
    };

    

#ifdef COBJMACROS


#define ToolWindows_QueryInterface(This,riid,ppvObject)	\
    ( (This)->lpVtbl -> QueryInterface(This,riid,ppvObject) ) 

#define ToolWindows_AddRef(This)	\
    ( (This)->lpVtbl -> AddRef(This) ) 

#define ToolWindows_Release(This)	\
    ( (This)->lpVtbl -> Release(This) ) 


#define ToolWindows_GetTypeInfoCount(This,pctinfo)	\
    ( (This)->lpVtbl -> GetTypeInfoCount(This,pctinfo) ) 

#define ToolWindows_GetTypeInfo(This,iTInfo,lcid,ppTInfo)	\
    ( (This)->lpVtbl -> GetTypeInfo(This,iTInfo,lcid,ppTInfo) ) 

#define ToolWindows_GetIDsOfNames(This,riid,rgszNames,cNames,lcid,rgDispId)	\
    ( (This)->lpVtbl -> GetIDsOfNames(This,riid,rgszNames,cNames,lcid,rgDispId) ) 

#define ToolWindows_Invoke(This,dispIdMember,riid,lcid,wFlags,pDispParams,pVarResult,pExcepInfo,puArgErr)	\
    ( (This)->lpVtbl -> Invoke(This,dispIdMember,riid,lcid,wFlags,pDispParams,pVarResult,pExcepInfo,puArgErr) ) 


#define ToolWindows_get_DTE(This,DTEObject)	\
    ( (This)->lpVtbl -> get_DTE(This,DTEObject) ) 

#define ToolWindows_GetToolWindow(This,Name,ppObject)	\
    ( (This)->lpVtbl -> GetToolWindow(This,Name,ppObject) ) 

#define ToolWindows_get_ToolBox(This,ppToolBox)	\
    ( (This)->lpVtbl -> get_ToolBox(This,ppToolBox) ) 

#define ToolWindows_get_CommandWindow(This,ppOutputWindow)	\
    ( (This)->lpVtbl -> get_CommandWindow(This,ppOutputWindow) ) 

#define ToolWindows_get_OutputWindow(This,ppOutputWindow)	\
    ( (This)->lpVtbl -> get_OutputWindow(This,ppOutputWindow) ) 

#define ToolWindows_get_SolutionExplorer(This,ppUIHierarchy)	\
    ( (This)->lpVtbl -> get_SolutionExplorer(This,ppUIHierarchy) ) 

#define ToolWindows_get_TaskList(This,ppTaskList)	\
    ( (This)->lpVtbl -> get_TaskList(This,ppTaskList) ) 

#define ToolWindows_get_ErrorList(This,ppErrorList)	\
    ( (This)->lpVtbl -> get_ErrorList(This,ppErrorList) ) 

#endif /* COBJMACROS */


#endif 	/* C style interface */




#endif 	/* __ToolWindows_INTERFACE_DEFINED__ */


#ifndef __Windows2_INTERFACE_DEFINED__
#define __Windows2_INTERFACE_DEFINED__

/* interface Windows2 */
/* [helpstringcontext][helpstring][helpcontext][uuid][object][oleautomation][dual] */ 


EXTERN_C const IID IID_Windows2;

#if defined(__cplusplus) && !defined(CINTERFACE)
    
    MIDL_INTERFACE("31EFB5B1-C655-4ada-BB52-3ED87FB2A4AE")
    Windows2 : public Windows
    {
    public:
        virtual /* [helpstringcontext][helpstring][helpcontext][id] */ HRESULT STDMETHODCALLTYPE CreateToolWindow2( 
            __RPC__in /* external definition not present */ AddIn *Addin,
            __RPC__in BSTR Assembly,
            __RPC__in BSTR Class,
            __RPC__in BSTR Caption,
            __RPC__in BSTR GuidPosition,
            /* [out][in] */ __RPC__deref_inout_opt IDispatch **ControlObject,
            /* [retval][out] */ __RPC__deref_out_opt /* external definition not present */ Window **ppwindowOut) = 0;
        
    };
    
#else 	/* C style interface */

    typedef struct Windows2Vtbl
    {
        BEGIN_INTERFACE
        
        /* [id][restricted][funcdescattr] */ HRESULT ( STDMETHODCALLTYPE *QueryInterface )( 
            Windows2 * This,
            /* [in][idldescattr] */ __RPC__in struct GUID *riid,
            /* [out][idldescattr] */ __RPC__deref_out_opt void **ppvObj,
            /* [retval][out] */ __RPC__out void *retval);
        
        /* [id][restricted][funcdescattr] */ HRESULT ( STDMETHODCALLTYPE *AddRef )( 
            Windows2 * This,
            /* [retval][out] */ __RPC__out unsigned long *retval);
        
        /* [id][restricted][funcdescattr] */ HRESULT ( STDMETHODCALLTYPE *Release )( 
            Windows2 * This,
            /* [retval][out] */ __RPC__out unsigned long *retval);
        
        /* [id][restricted][funcdescattr] */ HRESULT ( STDMETHODCALLTYPE *GetTypeInfoCount )( 
            Windows2 * This,
            /* [out][idldescattr] */ __RPC__out unsigned UINT *pctinfo,
            /* [retval][out] */ __RPC__out void *retval);
        
        /* [id][restricted][funcdescattr] */ HRESULT ( STDMETHODCALLTYPE *GetTypeInfo )( 
            Windows2 * This,
            /* [in][idldescattr] */ unsigned UINT itinfo,
            /* [in][idldescattr] */ unsigned long lcid,
            /* [out][idldescattr] */ __RPC__deref_out_opt void **pptinfo,
            /* [retval][out] */ __RPC__out void *retval);
        
        /* [id][restricted][funcdescattr] */ HRESULT ( STDMETHODCALLTYPE *GetIDsOfNames )( 
            Windows2 * This,
            /* [in][idldescattr] */ __RPC__in struct GUID *riid,
            /* [in][idldescattr] */ __RPC__deref_in_opt signed char **rgszNames,
            /* [in][idldescattr] */ unsigned UINT cNames,
            /* [in][idldescattr] */ unsigned long lcid,
            /* [out][idldescattr] */ __RPC__out signed long *rgdispid,
            /* [retval][out] */ __RPC__out void *retval);
        
        /* [id][restricted][funcdescattr] */ HRESULT ( STDMETHODCALLTYPE *Invoke )( 
            Windows2 * This,
            /* [in][idldescattr] */ signed long dispidMember,
            /* [in][idldescattr] */ __RPC__in struct GUID *riid,
            /* [in][idldescattr] */ unsigned long lcid,
            /* [in][idldescattr] */ unsigned short wFlags,
            /* [in][idldescattr] */ __RPC__in struct DISPPARAMS *pdispparams,
            /* [out][idldescattr] */ __RPC__out VARIANT *pvarResult,
            /* [out][idldescattr] */ __RPC__out struct EXCEPINFO *pexcepinfo,
            /* [out][idldescattr] */ __RPC__out unsigned UINT *puArgErr,
            /* [retval][out] */ __RPC__out void *retval);
        
        /* [id][funcdescattr] */ HRESULT ( STDMETHODCALLTYPE *Item )( 
            Windows2 * This,
            /* [in][idldescattr] */ VARIANT index,
            /* [retval][out] */ __RPC__deref_out_opt Window **retval);
        
        /* [id][propget][funcdescattr] */ HRESULT ( STDMETHODCALLTYPE *get_Count )( 
            Windows2 * This,
            /* [retval][out] */ __RPC__out signed long *retval);
        
        /* [id][restricted][funcdescattr] */ HRESULT ( STDMETHODCALLTYPE *_NewEnum )( 
            Windows2 * This,
            /* [retval][out] */ __RPC__deref_out_opt IUnknown **retval);
        
        /* [id][funcdescattr] */ HRESULT ( STDMETHODCALLTYPE *CreateToolWindow )( 
            Windows2 * This,
            /* [in][idldescattr] */ __RPC__in_opt AddIn *AddInInst,
            /* [in][idldescattr] */ __RPC__in BSTR ProgID,
            /* [in][idldescattr] */ __RPC__in BSTR Caption,
            /* [in][idldescattr] */ __RPC__in BSTR GuidPosition,
            /* [out][in][idldescattr] */ __RPC__deref_inout_opt IDispatch **DocObj,
            /* [retval][out] */ __RPC__deref_out_opt Window **retval);
        
        /* [id][propget][funcdescattr] */ HRESULT ( STDMETHODCALLTYPE *get_DTE )( 
            Windows2 * This,
            /* [retval][out] */ __RPC__deref_out_opt **retval);
        
        /* [id][propget][funcdescattr] */ HRESULT ( STDMETHODCALLTYPE *get_Parent )( 
            Windows2 * This,
            /* [retval][out] */ __RPC__deref_out_opt **retval);
        
        /* [id][funcdescattr] */ HRESULT ( STDMETHODCALLTYPE *CreateLinkedWindowFrame )( 
            Windows2 * This,
            /* [in][idldescattr] */ __RPC__in_opt Window *Window1,
            /* [in][idldescattr] */ __RPC__in_opt Window *Window2,
            /* [in][idldescattr] */ enum vsLinkedWindowType Link,
            /* [retval][out] */ __RPC__deref_out_opt Window **retval);
        
        /* [helpstringcontext][helpstring][helpcontext][id] */ HRESULT ( STDMETHODCALLTYPE *CreateToolWindow2 )( 
            Windows2 * This,
            __RPC__in /* external definition not present */ AddIn *Addin,
            __RPC__in BSTR Assembly,
            __RPC__in BSTR Class,
            __RPC__in BSTR Caption,
            __RPC__in BSTR GuidPosition,
            /* [out][in] */ __RPC__deref_inout_opt IDispatch **ControlObject,
            /* [retval][out] */ __RPC__deref_out_opt /* external definition not present */ Window **ppwindowOut);
        
        END_INTERFACE
    } Windows2Vtbl;

    interface Windows2
    {
        CONST_VTBL struct Windows2Vtbl *lpVtbl;
    };

    

#ifdef COBJMACROS


#define Windows2_QueryInterface(This,riid,ppvObj,retval)	\
    ( (This)->lpVtbl -> QueryInterface(This,riid,ppvObj,retval) ) 

#define Windows2_AddRef(This,retval)	\
    ( (This)->lpVtbl -> AddRef(This,retval) ) 

#define Windows2_Release(This,retval)	\
    ( (This)->lpVtbl -> Release(This,retval) ) 

#define Windows2_GetTypeInfoCount(This,pctinfo,retval)	\
    ( (This)->lpVtbl -> GetTypeInfoCount(This,pctinfo,retval) ) 

#define Windows2_GetTypeInfo(This,itinfo,lcid,pptinfo,retval)	\
    ( (This)->lpVtbl -> GetTypeInfo(This,itinfo,lcid,pptinfo,retval) ) 

#define Windows2_GetIDsOfNames(This,riid,rgszNames,cNames,lcid,rgdispid,retval)	\
    ( (This)->lpVtbl -> GetIDsOfNames(This,riid,rgszNames,cNames,lcid,rgdispid,retval) ) 

#define Windows2_Invoke(This,dispidMember,riid,lcid,wFlags,pdispparams,pvarResult,pexcepinfo,puArgErr,retval)	\
    ( (This)->lpVtbl -> Invoke(This,dispidMember,riid,lcid,wFlags,pdispparams,pvarResult,pexcepinfo,puArgErr,retval) ) 

#define Windows2_Item(This,index,retval)	\
    ( (This)->lpVtbl -> Item(This,index,retval) ) 

#define Windows2_get_Count(This,retval)	\
    ( (This)->lpVtbl -> get_Count(This,retval) ) 

#define Windows2__NewEnum(This,retval)	\
    ( (This)->lpVtbl -> _NewEnum(This,retval) ) 

#define Windows2_CreateToolWindow(This,AddInInst,ProgID,Caption,GuidPosition,DocObj,retval)	\
    ( (This)->lpVtbl -> CreateToolWindow(This,AddInInst,ProgID,Caption,GuidPosition,DocObj,retval) ) 

#define Windows2_get_DTE(This,retval)	\
    ( (This)->lpVtbl -> get_DTE(This,retval) ) 

#define Windows2_get_Parent(This,retval)	\
    ( (This)->lpVtbl -> get_Parent(This,retval) ) 

#define Windows2_CreateLinkedWindowFrame(This,Window1,Window2,Link,retval)	\
    ( (This)->lpVtbl -> CreateLinkedWindowFrame(This,Window1,Window2,Link,retval) ) 


#define Windows2_CreateToolWindow2(This,Addin,Assembly,Class,Caption,GuidPosition,ControlObject,ppwindowOut)	\
    ( (This)->lpVtbl -> CreateToolWindow2(This,Addin,Assembly,Class,Caption,GuidPosition,ControlObject,ppwindowOut) ) 

#endif /* COBJMACROS */


#endif 	/* C style interface */




#endif 	/* __Windows2_INTERFACE_DEFINED__ */


#ifndef __Window2_INTERFACE_DEFINED__
#define __Window2_INTERFACE_DEFINED__

/* interface Window2 */
/* [helpstringcontext][helpstring][helpcontext][uuid][object][oleautomation][dual] */ 


EXTERN_C const IID IID_Window2;

#if defined(__cplusplus) && !defined(CINTERFACE)
    
    MIDL_INTERFACE("25731932-3283-4ae0-B7CF-F4691B8BE523")
    Window2 : public Window
    {
    public:
        virtual /* [helpstringcontext][helpstring][helpcontext][propget][id] */ HRESULT STDMETHODCALLTYPE get_CommandBars( 
            /* [retval][out] */ __RPC__deref_out_opt IDispatch **ppcbs) = 0;
        
    };
    
#else 	/* C style interface */

    typedef struct Window2Vtbl
    {
        BEGIN_INTERFACE
        
        /* [id][restricted][funcdescattr] */ HRESULT ( STDMETHODCALLTYPE *QueryInterface )( 
            Window2 * This,
            /* [in][idldescattr] */ __RPC__in struct GUID *riid,
            /* [out][idldescattr] */ __RPC__deref_out_opt void **ppvObj,
            /* [retval][out] */ __RPC__out void *retval);
        
        /* [id][restricted][funcdescattr] */ HRESULT ( STDMETHODCALLTYPE *AddRef )( 
            Window2 * This,
            /* [retval][out] */ __RPC__out unsigned long *retval);
        
        /* [id][restricted][funcdescattr] */ HRESULT ( STDMETHODCALLTYPE *Release )( 
            Window2 * This,
            /* [retval][out] */ __RPC__out unsigned long *retval);
        
        /* [id][restricted][funcdescattr] */ HRESULT ( STDMETHODCALLTYPE *GetTypeInfoCount )( 
            Window2 * This,
            /* [out][idldescattr] */ __RPC__out unsigned UINT *pctinfo,
            /* [retval][out] */ __RPC__out void *retval);
        
        /* [id][restricted][funcdescattr] */ HRESULT ( STDMETHODCALLTYPE *GetTypeInfo )( 
            Window2 * This,
            /* [in][idldescattr] */ unsigned UINT itinfo,
            /* [in][idldescattr] */ unsigned long lcid,
            /* [out][idldescattr] */ __RPC__deref_out_opt void **pptinfo,
            /* [retval][out] */ __RPC__out void *retval);
        
        /* [id][restricted][funcdescattr] */ HRESULT ( STDMETHODCALLTYPE *GetIDsOfNames )( 
            Window2 * This,
            /* [in][idldescattr] */ __RPC__in struct GUID *riid,
            /* [in][idldescattr] */ __RPC__deref_in_opt signed char **rgszNames,
            /* [in][idldescattr] */ unsigned UINT cNames,
            /* [in][idldescattr] */ unsigned long lcid,
            /* [out][idldescattr] */ __RPC__out signed long *rgdispid,
            /* [retval][out] */ __RPC__out void *retval);
        
        /* [id][restricted][funcdescattr] */ HRESULT ( STDMETHODCALLTYPE *Invoke )( 
            Window2 * This,
            /* [in][idldescattr] */ signed long dispidMember,
            /* [in][idldescattr] */ __RPC__in struct GUID *riid,
            /* [in][idldescattr] */ unsigned long lcid,
            /* [in][idldescattr] */ unsigned short wFlags,
            /* [in][idldescattr] */ __RPC__in struct DISPPARAMS *pdispparams,
            /* [out][idldescattr] */ __RPC__out VARIANT *pvarResult,
            /* [out][idldescattr] */ __RPC__out struct EXCEPINFO *pexcepinfo,
            /* [out][idldescattr] */ __RPC__out unsigned UINT *puArgErr,
            /* [retval][out] */ __RPC__out void *retval);
        
        /* [id][propget][funcdescattr] */ HRESULT ( STDMETHODCALLTYPE *get_Collection )( 
            Window2 * This,
            /* [retval][out] */ __RPC__deref_out_opt Windows **retval);
        
        /* [id][propget][funcdescattr] */ HRESULT ( STDMETHODCALLTYPE *get_Visible )( 
            Window2 * This,
            /* [retval][out] */ __RPC__out BOOLEAN *retval);
        
        /* [id][propput][funcdescattr] */ HRESULT ( STDMETHODCALLTYPE *put_Visible )( 
            Window2 * This,
            /* [in][idldescattr] */ BOOLEAN noname,
            /* [retval][out] */ __RPC__out void *retval);
        
        /* [id][propget][funcdescattr] */ HRESULT ( STDMETHODCALLTYPE *get_Left )( 
            Window2 * This,
            /* [retval][out] */ __RPC__out signed long *retval);
        
        /* [id][propput][funcdescattr] */ HRESULT ( STDMETHODCALLTYPE *put_Left )( 
            Window2 * This,
            /* [in][idldescattr] */ signed long noname,
            /* [retval][out] */ __RPC__out void *retval);
        
        /* [id][propget][funcdescattr] */ HRESULT ( STDMETHODCALLTYPE *get_Top )( 
            Window2 * This,
            /* [retval][out] */ __RPC__out signed long *retval);
        
        /* [id][propput][funcdescattr] */ HRESULT ( STDMETHODCALLTYPE *put_Top )( 
            Window2 * This,
            /* [in][idldescattr] */ signed long noname,
            /* [retval][out] */ __RPC__out void *retval);
        
        /* [id][propget][funcdescattr] */ HRESULT ( STDMETHODCALLTYPE *get_Width )( 
            Window2 * This,
            /* [retval][out] */ __RPC__out signed long *retval);
        
        /* [id][propput][funcdescattr] */ HRESULT ( STDMETHODCALLTYPE *put_Width )( 
            Window2 * This,
            /* [in][idldescattr] */ signed long noname,
            /* [retval][out] */ __RPC__out void *retval);
        
        /* [id][propget][funcdescattr] */ HRESULT ( STDMETHODCALLTYPE *get_Height )( 
            Window2 * This,
            /* [retval][out] */ __RPC__out signed long *retval);
        
        /* [id][propput][funcdescattr] */ HRESULT ( STDMETHODCALLTYPE *put_Height )( 
            Window2 * This,
            /* [in][idldescattr] */ signed long noname,
            /* [retval][out] */ __RPC__out void *retval);
        
        /* [id][propget][funcdescattr] */ HRESULT ( STDMETHODCALLTYPE *get_WindowState )( 
            Window2 * This,
            /* [retval][out] */ __RPC__out enum vsWindowState *retval);
        
        /* [id][propput][funcdescattr] */ HRESULT ( STDMETHODCALLTYPE *put_WindowState )( 
            Window2 * This,
            /* [in][idldescattr] */ enum vsWindowState noname,
            /* [retval][out] */ __RPC__out void *retval);
        
        /* [id][hidden][funcdescattr] */ HRESULT ( STDMETHODCALLTYPE *SetFocus )( 
            Window2 * This,
            /* [retval][out] */ __RPC__out void *retval);
        
        /* [id][propget][hidden][funcdescattr] */ HRESULT ( STDMETHODCALLTYPE *get_Type )( 
            Window2 * This,
            /* [retval][out] */ __RPC__out enum vsWindowType *retval);
        
        /* [id][funcdescattr] */ HRESULT ( STDMETHODCALLTYPE *SetKind )( 
            Window2 * This,
            /* [in][idldescattr] */ enum vsWindowType eKind,
            /* [retval][out] */ __RPC__out void *retval);
        
        /* [id][propget][funcdescattr] */ HRESULT ( STDMETHODCALLTYPE *get_LinkedWindows )( 
            Window2 * This,
            /* [retval][out] */ __RPC__deref_out_opt LinkedWindows **retval);
        
        /* [id][propget][funcdescattr] */ HRESULT ( STDMETHODCALLTYPE *get_LinkedWindowFrame )( 
            Window2 * This,
            /* [retval][out] */ __RPC__deref_out_opt Window **retval);
        
        /* [id][funcdescattr] */ HRESULT ( STDMETHODCALLTYPE *Detach )( 
            Window2 * This,
            /* [retval][out] */ __RPC__out void *retval);
        
        /* [id][funcdescattr] */ HRESULT ( STDMETHODCALLTYPE *Attach )( 
            Window2 * This,
            /* [in][idldescattr] */ signed long lWindowHandle,
            /* [retval][out] */ __RPC__out void *retval);
        
        /* [id][propget][hidden][funcdescattr] */ HRESULT ( STDMETHODCALLTYPE *get_HWnd )( 
            Window2 * This,
            /* [retval][out] */ __RPC__out signed long *retval);
        
        /* [id][propget][funcdescattr] */ HRESULT ( STDMETHODCALLTYPE *get_Kind )( 
            Window2 * This,
            /* [retval][out] */ __RPC__deref_out_opt BSTR *retval);
        
        /* [id][propget][funcdescattr] */ HRESULT ( STDMETHODCALLTYPE *get_ObjectKind )( 
            Window2 * This,
            /* [retval][out] */ __RPC__deref_out_opt BSTR *retval);
        
        /* [id][propget][funcdescattr] */ HRESULT ( STDMETHODCALLTYPE *get_Object )( 
            Window2 * This,
            /* [retval][out] */ __RPC__deref_out_opt IDispatch **retval);
        
        /* [id][propget][hidden][funcdescattr] */ HRESULT ( STDMETHODCALLTYPE *get_DocumentData )( 
            Window2 * This,
            /* [in][idldescattr] */ __RPC__in BSTR bstrWhichData,
            /* [retval][out] */ __RPC__deref_out_opt IDispatch **retval);
        
        /* [id][propget][funcdescattr] */ HRESULT ( STDMETHODCALLTYPE *get_ProjectItem )( 
            Window2 * This,
            /* [retval][out] */ __RPC__deref_out_opt ProjectItem **retval);
        
        /* [id][propget][funcdescattr] */ HRESULT ( STDMETHODCALLTYPE *get_Project )( 
            Window2 * This,
            /* [retval][out] */ __RPC__deref_out_opt Project **retval);
        
        /* [id][propget][funcdescattr] */ HRESULT ( STDMETHODCALLTYPE *get_DTE )( 
            Window2 * This,
            /* [retval][out] */ __RPC__deref_out_opt **retval);
        
        /* [id][propget][funcdescattr] */ HRESULT ( STDMETHODCALLTYPE *get_Document )( 
            Window2 * This,
            /* [retval][out] */ __RPC__deref_out_opt Document **retval);
        
        /* [id][propget][funcdescattr] */ HRESULT ( STDMETHODCALLTYPE *get_Selection )( 
            Window2 * This,
            /* [retval][out] */ __RPC__deref_out_opt IDispatch **retval);
        
        /* [id][propget][funcdescattr] */ HRESULT ( STDMETHODCALLTYPE *get_Linkable )( 
            Window2 * This,
            /* [retval][out] */ __RPC__out BOOLEAN *retval);
        
        /* [id][propput][funcdescattr] */ HRESULT ( STDMETHODCALLTYPE *put_Linkable )( 
            Window2 * This,
            /* [in][idldescattr] */ BOOLEAN noname,
            /* [retval][out] */ __RPC__out void *retval);
        
        /* [id][funcdescattr] */ HRESULT ( STDMETHODCALLTYPE *Activate )( 
            Window2 * This,
            /* [retval][out] */ __RPC__out void *retval);
        
        /* [id][funcdescattr] */ HRESULT ( STDMETHODCALLTYPE *Close )( 
            Window2 * This,
            /* [in][idldescattr] */ enum vsSaveChanges SaveChanges,
            /* [retval][out] */ __RPC__out void *retval);
        
        /* [id][propget][funcdescattr] */ HRESULT ( STDMETHODCALLTYPE *get_Caption )( 
            Window2 * This,
            /* [retval][out] */ __RPC__deref_out_opt BSTR *retval);
        
        /* [id][propput][funcdescattr] */ HRESULT ( STDMETHODCALLTYPE *put_Caption )( 
            Window2 * This,
            /* [in][idldescattr] */ __RPC__in BSTR noname,
            /* [retval][out] */ __RPC__out void *retval);
        
        /* [id][funcdescattr] */ HRESULT ( STDMETHODCALLTYPE *SetSelectionContainer )( 
            Window2 * This,
            /* [idldescattr] */ __RPC__deref_in_opt SAFEARRAY * *Objects,
            /* [retval][out] */ __RPC__out void *retval);
        
        /* [id][propget][funcdescattr] */ HRESULT ( STDMETHODCALLTYPE *get_IsFloating )( 
            Window2 * This,
            /* [retval][out] */ __RPC__out BOOLEAN *retval);
        
        /* [id][propput][funcdescattr] */ HRESULT ( STDMETHODCALLTYPE *put_IsFloating )( 
            Window2 * This,
            /* [in][idldescattr] */ BOOLEAN noname,
            /* [retval][out] */ __RPC__out void *retval);
        
        /* [id][propget][funcdescattr] */ HRESULT ( STDMETHODCALLTYPE *get_AutoHides )( 
            Window2 * This,
            /* [retval][out] */ __RPC__out BOOLEAN *retval);
        
        /* [id][propput][funcdescattr] */ HRESULT ( STDMETHODCALLTYPE *put_AutoHides )( 
            Window2 * This,
            /* [in][idldescattr] */ BOOLEAN noname,
            /* [retval][out] */ __RPC__out void *retval);
        
        /* [id][funcdescattr] */ HRESULT ( STDMETHODCALLTYPE *SetTabPicture )( 
            Window2 * This,
            /* [idldescattr] */ VARIANT Picture,
            /* [retval][out] */ __RPC__out void *retval);
        
        /* [id][propget][funcdescattr] */ HRESULT ( STDMETHODCALLTYPE *get_ContextAttributes )( 
            Window2 * This,
            /* [retval][out] */ __RPC__deref_out_opt ContextAttributes **retval);
        
        /* [helpstringcontext][helpstring][helpcontext][propget][id] */ HRESULT ( STDMETHODCALLTYPE *get_CommandBars )( 
            Window2 * This,
            /* [retval][out] */ __RPC__deref_out_opt IDispatch **ppcbs);
        
        END_INTERFACE
    } Window2Vtbl;

    interface Window2
    {
        CONST_VTBL struct Window2Vtbl *lpVtbl;
    };

    

#ifdef COBJMACROS


#define Window2_QueryInterface(This,riid,ppvObj,retval)	\
    ( (This)->lpVtbl -> QueryInterface(This,riid,ppvObj,retval) ) 

#define Window2_AddRef(This,retval)	\
    ( (This)->lpVtbl -> AddRef(This,retval) ) 

#define Window2_Release(This,retval)	\
    ( (This)->lpVtbl -> Release(This,retval) ) 

#define Window2_GetTypeInfoCount(This,pctinfo,retval)	\
    ( (This)->lpVtbl -> GetTypeInfoCount(This,pctinfo,retval) ) 

#define Window2_GetTypeInfo(This,itinfo,lcid,pptinfo,retval)	\
    ( (This)->lpVtbl -> GetTypeInfo(This,itinfo,lcid,pptinfo,retval) ) 

#define Window2_GetIDsOfNames(This,riid,rgszNames,cNames,lcid,rgdispid,retval)	\
    ( (This)->lpVtbl -> GetIDsOfNames(This,riid,rgszNames,cNames,lcid,rgdispid,retval) ) 

#define Window2_Invoke(This,dispidMember,riid,lcid,wFlags,pdispparams,pvarResult,pexcepinfo,puArgErr,retval)	\
    ( (This)->lpVtbl -> Invoke(This,dispidMember,riid,lcid,wFlags,pdispparams,pvarResult,pexcepinfo,puArgErr,retval) ) 

#define Window2_get_Collection(This,retval)	\
    ( (This)->lpVtbl -> get_Collection(This,retval) ) 

#define Window2_get_Visible(This,retval)	\
    ( (This)->lpVtbl -> get_Visible(This,retval) ) 

#define Window2_put_Visible(This,noname,retval)	\
    ( (This)->lpVtbl -> put_Visible(This,noname,retval) ) 

#define Window2_get_Left(This,retval)	\
    ( (This)->lpVtbl -> get_Left(This,retval) ) 

#define Window2_put_Left(This,noname,retval)	\
    ( (This)->lpVtbl -> put_Left(This,noname,retval) ) 

#define Window2_get_Top(This,retval)	\
    ( (This)->lpVtbl -> get_Top(This,retval) ) 

#define Window2_put_Top(This,noname,retval)	\
    ( (This)->lpVtbl -> put_Top(This,noname,retval) ) 

#define Window2_get_Width(This,retval)	\
    ( (This)->lpVtbl -> get_Width(This,retval) ) 

#define Window2_put_Width(This,noname,retval)	\
    ( (This)->lpVtbl -> put_Width(This,noname,retval) ) 

#define Window2_get_Height(This,retval)	\
    ( (This)->lpVtbl -> get_Height(This,retval) ) 

#define Window2_put_Height(This,noname,retval)	\
    ( (This)->lpVtbl -> put_Height(This,noname,retval) ) 

#define Window2_get_WindowState(This,retval)	\
    ( (This)->lpVtbl -> get_WindowState(This,retval) ) 

#define Window2_put_WindowState(This,noname,retval)	\
    ( (This)->lpVtbl -> put_WindowState(This,noname,retval) ) 

#define Window2_SetFocus(This,retval)	\
    ( (This)->lpVtbl -> SetFocus(This,retval) ) 

#define Window2_get_Type(This,retval)	\
    ( (This)->lpVtbl -> get_Type(This,retval) ) 

#define Window2_SetKind(This,eKind,retval)	\
    ( (This)->lpVtbl -> SetKind(This,eKind,retval) ) 

#define Window2_get_LinkedWindows(This,retval)	\
    ( (This)->lpVtbl -> get_LinkedWindows(This,retval) ) 

#define Window2_get_LinkedWindowFrame(This,retval)	\
    ( (This)->lpVtbl -> get_LinkedWindowFrame(This,retval) ) 

#define Window2_Detach(This,retval)	\
    ( (This)->lpVtbl -> Detach(This,retval) ) 

#define Window2_Attach(This,lWindowHandle,retval)	\
    ( (This)->lpVtbl -> Attach(This,lWindowHandle,retval) ) 

#define Window2_get_HWnd(This,retval)	\
    ( (This)->lpVtbl -> get_HWnd(This,retval) ) 

#define Window2_get_Kind(This,retval)	\
    ( (This)->lpVtbl -> get_Kind(This,retval) ) 

#define Window2_get_ObjectKind(This,retval)	\
    ( (This)->lpVtbl -> get_ObjectKind(This,retval) ) 

#define Window2_get_Object(This,retval)	\
    ( (This)->lpVtbl -> get_Object(This,retval) ) 

#define Window2_get_DocumentData(This,bstrWhichData,retval)	\
    ( (This)->lpVtbl -> get_DocumentData(This,bstrWhichData,retval) ) 

#define Window2_get_ProjectItem(This,retval)	\
    ( (This)->lpVtbl -> get_ProjectItem(This,retval) ) 

#define Window2_get_Project(This,retval)	\
    ( (This)->lpVtbl -> get_Project(This,retval) ) 

#define Window2_get_DTE(This,retval)	\
    ( (This)->lpVtbl -> get_DTE(This,retval) ) 

#define Window2_get_Document(This,retval)	\
    ( (This)->lpVtbl -> get_Document(This,retval) ) 

#define Window2_get_Selection(This,retval)	\
    ( (This)->lpVtbl -> get_Selection(This,retval) ) 

#define Window2_get_Linkable(This,retval)	\
    ( (This)->lpVtbl -> get_Linkable(This,retval) ) 

#define Window2_put_Linkable(This,noname,retval)	\
    ( (This)->lpVtbl -> put_Linkable(This,noname,retval) ) 

#define Window2_Activate(This,retval)	\
    ( (This)->lpVtbl -> Activate(This,retval) ) 

#define Window2_Close(This,SaveChanges,retval)	\
    ( (This)->lpVtbl -> Close(This,SaveChanges,retval) ) 

#define Window2_get_Caption(This,retval)	\
    ( (This)->lpVtbl -> get_Caption(This,retval) ) 

#define Window2_put_Caption(This,noname,retval)	\
    ( (This)->lpVtbl -> put_Caption(This,noname,retval) ) 

#define Window2_SetSelectionContainer(This,Objects,retval)	\
    ( (This)->lpVtbl -> SetSelectionContainer(This,Objects,retval) ) 

#define Window2_get_IsFloating(This,retval)	\
    ( (This)->lpVtbl -> get_IsFloating(This,retval) ) 

#define Window2_put_IsFloating(This,noname,retval)	\
    ( (This)->lpVtbl -> put_IsFloating(This,noname,retval) ) 

#define Window2_get_AutoHides(This,retval)	\
    ( (This)->lpVtbl -> get_AutoHides(This,retval) ) 

#define Window2_put_AutoHides(This,noname,retval)	\
    ( (This)->lpVtbl -> put_AutoHides(This,noname,retval) ) 

#define Window2_SetTabPicture(This,Picture,retval)	\
    ( (This)->lpVtbl -> SetTabPicture(This,Picture,retval) ) 

#define Window2_get_ContextAttributes(This,retval)	\
    ( (This)->lpVtbl -> get_ContextAttributes(This,retval) ) 


#define Window2_get_CommandBars(This,ppcbs)	\
    ( (This)->lpVtbl -> get_CommandBars(This,ppcbs) ) 

#endif /* COBJMACROS */


#endif 	/* C style interface */




#endif 	/* __Window2_INTERFACE_DEFINED__ */


#ifndef __SourceControlBindings_INTERFACE_DEFINED__
#define __SourceControlBindings_INTERFACE_DEFINED__

/* interface SourceControlBindings */
/* [helpstringcontext][helpstring][helpcontext][uuid][object][oleautomation][dual] */ 


EXTERN_C const IID IID_SourceControlBindings;

#if defined(__cplusplus) && !defined(CINTERFACE)
    
    MIDL_INTERFACE("544a6477-be0b-42ed-b47b-553ed8c5f22c")
    SourceControlBindings : public IDispatch
    {
    public:
        virtual /* [helpstringcontext][helpstring][helpcontext][id][propget] */ HRESULT STDMETHODCALLTYPE get_DTE( 
            /* [retval][out] */ __RPC__deref_out_opt /* external definition not present */ DTE **pDTE) = 0;
        
        virtual /* [helpstringcontext][helpstring][helpcontext][id][propget] */ HRESULT STDMETHODCALLTYPE get_Parent( 
            /* [retval][out] */ __RPC__deref_out_opt /* external definition not present */ SourceControl **ppSourceControl) = 0;
        
        virtual /* [helpstringcontext][helpstring][helpcontext][id][propget] */ HRESULT STDMETHODCALLTYPE get_ServerName( 
            /* [retval][out] */ __RPC__deref_out_opt BSTR *lpbstrSeverName) = 0;
        
        virtual /* [helpstringcontext][helpstring][helpcontext][id][propget] */ HRESULT STDMETHODCALLTYPE get_LocalBinding( 
            /* [retval][out] */ __RPC__deref_out_opt BSTR *lpbstrLocalPath) = 0;
        
        virtual /* [helpstringcontext][helpstring][helpcontext][id][propget] */ HRESULT STDMETHODCALLTYPE get_ServerBinding( 
            /* [retval][out] */ __RPC__deref_out_opt BSTR *lpbstrServerLocation) = 0;
        
        virtual /* [helpstringcontext][helpstring][helpcontext][id][propget] */ HRESULT STDMETHODCALLTYPE get_ProviderName( 
            /* [retval][out] */ __RPC__deref_out_opt BSTR *lpbstrProviderName) = 0;
        
        virtual /* [helpstringcontext][helpstring][helpcontext][id][propget] */ HRESULT STDMETHODCALLTYPE get_ProviderRegKey( 
            /* [retval][out] */ __RPC__deref_out_opt BSTR *lpbstrProviderRegKey) = 0;
        
    };
    
#else 	/* C style interface */

    typedef struct SourceControlBindingsVtbl
    {
        BEGIN_INTERFACE
        
        HRESULT ( STDMETHODCALLTYPE *QueryInterface )( 
            SourceControlBindings * This,
            /* [in] */ __RPC__in REFIID riid,
            /* [iid_is][out] */ 
            __RPC__deref_out  void **ppvObject);
        
        ULONG ( STDMETHODCALLTYPE *AddRef )( 
            SourceControlBindings * This);
        
        ULONG ( STDMETHODCALLTYPE *Release )( 
            SourceControlBindings * This);
        
        HRESULT ( STDMETHODCALLTYPE *GetTypeInfoCount )( 
            SourceControlBindings * This,
            /* [out] */ __RPC__out UINT *pctinfo);
        
        HRESULT ( STDMETHODCALLTYPE *GetTypeInfo )( 
            SourceControlBindings * This,
            /* [in] */ UINT iTInfo,
            /* [in] */ LCID lcid,
            /* [out] */ __RPC__deref_out_opt ITypeInfo **ppTInfo);
        
        HRESULT ( STDMETHODCALLTYPE *GetIDsOfNames )( 
            SourceControlBindings * This,
            /* [in] */ __RPC__in REFIID riid,
            /* [size_is][in] */ __RPC__in_ecount_full(cNames) LPOLESTR *rgszNames,
            /* [range][in] */ UINT cNames,
            /* [in] */ LCID lcid,
            /* [size_is][out] */ __RPC__out_ecount_full(cNames) DISPID *rgDispId);
        
        /* [local] */ HRESULT ( STDMETHODCALLTYPE *Invoke )( 
            SourceControlBindings * This,
            /* [in] */ DISPID dispIdMember,
            /* [in] */ REFIID riid,
            /* [in] */ LCID lcid,
            /* [in] */ WORD wFlags,
            /* [out][in] */ DISPPARAMS *pDispParams,
            /* [out] */ VARIANT *pVarResult,
            /* [out] */ EXCEPINFO *pExcepInfo,
            /* [out] */ UINT *puArgErr);
        
        /* [helpstringcontext][helpstring][helpcontext][id][propget] */ HRESULT ( STDMETHODCALLTYPE *get_DTE )( 
            SourceControlBindings * This,
            /* [retval][out] */ __RPC__deref_out_opt /* external definition not present */ DTE **pDTE);
        
        /* [helpstringcontext][helpstring][helpcontext][id][propget] */ HRESULT ( STDMETHODCALLTYPE *get_Parent )( 
            SourceControlBindings * This,
            /* [retval][out] */ __RPC__deref_out_opt /* external definition not present */ SourceControl **ppSourceControl);
        
        /* [helpstringcontext][helpstring][helpcontext][id][propget] */ HRESULT ( STDMETHODCALLTYPE *get_ServerName )( 
            SourceControlBindings * This,
            /* [retval][out] */ __RPC__deref_out_opt BSTR *lpbstrSeverName);
        
        /* [helpstringcontext][helpstring][helpcontext][id][propget] */ HRESULT ( STDMETHODCALLTYPE *get_LocalBinding )( 
            SourceControlBindings * This,
            /* [retval][out] */ __RPC__deref_out_opt BSTR *lpbstrLocalPath);
        
        /* [helpstringcontext][helpstring][helpcontext][id][propget] */ HRESULT ( STDMETHODCALLTYPE *get_ServerBinding )( 
            SourceControlBindings * This,
            /* [retval][out] */ __RPC__deref_out_opt BSTR *lpbstrServerLocation);
        
        /* [helpstringcontext][helpstring][helpcontext][id][propget] */ HRESULT ( STDMETHODCALLTYPE *get_ProviderName )( 
            SourceControlBindings * This,
            /* [retval][out] */ __RPC__deref_out_opt BSTR *lpbstrProviderName);
        
        /* [helpstringcontext][helpstring][helpcontext][id][propget] */ HRESULT ( STDMETHODCALLTYPE *get_ProviderRegKey )( 
            SourceControlBindings * This,
            /* [retval][out] */ __RPC__deref_out_opt BSTR *lpbstrProviderRegKey);
        
        END_INTERFACE
    } SourceControlBindingsVtbl;

    interface SourceControlBindings
    {
        CONST_VTBL struct SourceControlBindingsVtbl *lpVtbl;
    };

    

#ifdef COBJMACROS


#define SourceControlBindings_QueryInterface(This,riid,ppvObject)	\
    ( (This)->lpVtbl -> QueryInterface(This,riid,ppvObject) ) 

#define SourceControlBindings_AddRef(This)	\
    ( (This)->lpVtbl -> AddRef(This) ) 

#define SourceControlBindings_Release(This)	\
    ( (This)->lpVtbl -> Release(This) ) 


#define SourceControlBindings_GetTypeInfoCount(This,pctinfo)	\
    ( (This)->lpVtbl -> GetTypeInfoCount(This,pctinfo) ) 

#define SourceControlBindings_GetTypeInfo(This,iTInfo,lcid,ppTInfo)	\
    ( (This)->lpVtbl -> GetTypeInfo(This,iTInfo,lcid,ppTInfo) ) 

#define SourceControlBindings_GetIDsOfNames(This,riid,rgszNames,cNames,lcid,rgDispId)	\
    ( (This)->lpVtbl -> GetIDsOfNames(This,riid,rgszNames,cNames,lcid,rgDispId) ) 

#define SourceControlBindings_Invoke(This,dispIdMember,riid,lcid,wFlags,pDispParams,pVarResult,pExcepInfo,puArgErr)	\
    ( (This)->lpVtbl -> Invoke(This,dispIdMember,riid,lcid,wFlags,pDispParams,pVarResult,pExcepInfo,puArgErr) ) 


#define SourceControlBindings_get_DTE(This,pDTE)	\
    ( (This)->lpVtbl -> get_DTE(This,pDTE) ) 

#define SourceControlBindings_get_Parent(This,ppSourceControl)	\
    ( (This)->lpVtbl -> get_Parent(This,ppSourceControl) ) 

#define SourceControlBindings_get_ServerName(This,lpbstrSeverName)	\
    ( (This)->lpVtbl -> get_ServerName(This,lpbstrSeverName) ) 

#define SourceControlBindings_get_LocalBinding(This,lpbstrLocalPath)	\
    ( (This)->lpVtbl -> get_LocalBinding(This,lpbstrLocalPath) ) 

#define SourceControlBindings_get_ServerBinding(This,lpbstrServerLocation)	\
    ( (This)->lpVtbl -> get_ServerBinding(This,lpbstrServerLocation) ) 

#define SourceControlBindings_get_ProviderName(This,lpbstrProviderName)	\
    ( (This)->lpVtbl -> get_ProviderName(This,lpbstrProviderName) ) 

#define SourceControlBindings_get_ProviderRegKey(This,lpbstrProviderRegKey)	\
    ( (This)->lpVtbl -> get_ProviderRegKey(This,lpbstrProviderRegKey) ) 

#endif /* COBJMACROS */


#endif 	/* C style interface */




#endif 	/* __SourceControlBindings_INTERFACE_DEFINED__ */


#ifndef __DTE2_INTERFACE_DEFINED__
#define __DTE2_INTERFACE_DEFINED__

/* interface DTE2 */
/* [helpstringcontext][helpstring][helpcontext][uuid][object][oleautomation][dual] */ 


EXTERN_C const IID IID_DTE2;

#if defined(__cplusplus) && !defined(CINTERFACE)
    
    MIDL_INTERFACE("2EE1E9FA-0AFE-4348-A89F-ED9CB45C99CF")
    DTE2 : public _DTE
    {
    public:
        virtual /* [helpstringcontext][helpstring][helpcontext][propget][id] */ HRESULT STDMETHODCALLTYPE get_ToolWindows( 
            /* [retval][out] */ __RPC__deref_out_opt ToolWindows **ppcbs) = 0;
        
        virtual /* [helpstring][id] */ HRESULT STDMETHODCALLTYPE GetThemeColor( 
            vsThemeColors Element,
            /* [retval][out] */ __RPC__out /* external definition not present */ OLE_COLOR *pColor) = 0;
        
    };
    
#else 	/* C style interface */

    typedef struct DTE2Vtbl
    {
        BEGIN_INTERFACE
        
        /* [id][restricted][funcdescattr] */ HRESULT ( STDMETHODCALLTYPE *QueryInterface )( 
            DTE2 * This,
            /* [in][idldescattr] */ __RPC__in struct GUID *riid,
            /* [out][idldescattr] */ __RPC__deref_out_opt void **ppvObj,
            /* [retval][out] */ __RPC__out void *retval);
        
        /* [id][restricted][funcdescattr] */ HRESULT ( STDMETHODCALLTYPE *AddRef )( 
            DTE2 * This,
            /* [retval][out] */ __RPC__out unsigned long *retval);
        
        /* [id][restricted][funcdescattr] */ HRESULT ( STDMETHODCALLTYPE *Release )( 
            DTE2 * This,
            /* [retval][out] */ __RPC__out unsigned long *retval);
        
        /* [id][restricted][funcdescattr] */ HRESULT ( STDMETHODCALLTYPE *GetTypeInfoCount )( 
            DTE2 * This,
            /* [out][idldescattr] */ __RPC__out unsigned UINT *pctinfo,
            /* [retval][out] */ __RPC__out void *retval);
        
        /* [id][restricted][funcdescattr] */ HRESULT ( STDMETHODCALLTYPE *GetTypeInfo )( 
            DTE2 * This,
            /* [in][idldescattr] */ unsigned UINT itinfo,
            /* [in][idldescattr] */ unsigned long lcid,
            /* [out][idldescattr] */ __RPC__deref_out_opt void **pptinfo,
            /* [retval][out] */ __RPC__out void *retval);
        
        /* [id][restricted][funcdescattr] */ HRESULT ( STDMETHODCALLTYPE *GetIDsOfNames )( 
            DTE2 * This,
            /* [in][idldescattr] */ __RPC__in struct GUID *riid,
            /* [in][idldescattr] */ __RPC__deref_in_opt signed char **rgszNames,
            /* [in][idldescattr] */ unsigned UINT cNames,
            /* [in][idldescattr] */ unsigned long lcid,
            /* [out][idldescattr] */ __RPC__out signed long *rgdispid,
            /* [retval][out] */ __RPC__out void *retval);
        
        /* [id][restricted][funcdescattr] */ HRESULT ( STDMETHODCALLTYPE *Invoke )( 
            DTE2 * This,
            /* [in][idldescattr] */ signed long dispidMember,
            /* [in][idldescattr] */ __RPC__in struct GUID *riid,
            /* [in][idldescattr] */ unsigned long lcid,
            /* [in][idldescattr] */ unsigned short wFlags,
            /* [in][idldescattr] */ __RPC__in struct DISPPARAMS *pdispparams,
            /* [out][idldescattr] */ __RPC__out VARIANT *pvarResult,
            /* [out][idldescattr] */ __RPC__out struct EXCEPINFO *pexcepinfo,
            /* [out][idldescattr] */ __RPC__out unsigned UINT *puArgErr,
            /* [retval][out] */ __RPC__out void *retval);
        
        /* [id][propget][funcdescattr] */ HRESULT ( STDMETHODCALLTYPE *get_Name )( 
            DTE2 * This,
            /* [retval][out] */ __RPC__deref_out_opt BSTR *retval);
        
        /* [id][propget][hidden][funcdescattr] */ HRESULT ( STDMETHODCALLTYPE *get_FileName )( 
            DTE2 * This,
            /* [retval][out] */ __RPC__deref_out_opt BSTR *retval);
        
        /* [id][propget][funcdescattr] */ HRESULT ( STDMETHODCALLTYPE *get_Version )( 
            DTE2 * This,
            /* [retval][out] */ __RPC__deref_out_opt BSTR *retval);
        
        /* [id][propget][funcdescattr] */ HRESULT ( STDMETHODCALLTYPE *get_CommandBars )( 
            DTE2 * This,
            /* [retval][out] */ __RPC__deref_out_opt IDispatch **retval);
        
        /* [id][propget][funcdescattr] */ HRESULT ( STDMETHODCALLTYPE *get_Windows )( 
            DTE2 * This,
            /* [retval][out] */ __RPC__deref_out_opt Windows **retval);
        
        /* [id][propget][funcdescattr] */ HRESULT ( STDMETHODCALLTYPE *get_Events )( 
            DTE2 * This,
            /* [retval][out] */ __RPC__deref_out_opt Events **retval);
        
        /* [id][propget][funcdescattr] */ HRESULT ( STDMETHODCALLTYPE *get_AddIns )( 
            DTE2 * This,
            /* [retval][out] */ __RPC__deref_out_opt AddIns **retval);
        
        /* [id][propget][funcdescattr] */ HRESULT ( STDMETHODCALLTYPE *get_MainWindow )( 
            DTE2 * This,
            /* [retval][out] */ __RPC__deref_out_opt Window **retval);
        
        /* [id][propget][funcdescattr] */ HRESULT ( STDMETHODCALLTYPE *get_ActiveWindow )( 
            DTE2 * This,
            /* [retval][out] */ __RPC__deref_out_opt Window **retval);
        
        /* [id][funcdescattr] */ HRESULT ( STDMETHODCALLTYPE *Quit )( 
            DTE2 * This,
            /* [retval][out] */ __RPC__out void *retval);
        
        /* [id][propget][funcdescattr] */ HRESULT ( STDMETHODCALLTYPE *get_DisplayMode )( 
            DTE2 * This,
            /* [retval][out] */ __RPC__out enum vsDisplay *retval);
        
        /* [id][propput][funcdescattr] */ HRESULT ( STDMETHODCALLTYPE *put_DisplayMode )( 
            DTE2 * This,
            /* [in][idldescattr] */ enum vsDisplay noname,
            /* [retval][out] */ __RPC__out void *retval);
        
        /* [id][propget][funcdescattr] */ HRESULT ( STDMETHODCALLTYPE *get_Solution )( 
            DTE2 * This,
            /* [retval][out] */ __RPC__deref_out_opt **retval);
        
        /* [id][propget][funcdescattr] */ HRESULT ( STDMETHODCALLTYPE *get_Commands )( 
            DTE2 * This,
            /* [retval][out] */ __RPC__deref_out_opt Commands **retval);
        
        /* [id][funcdescattr] */ HRESULT ( STDMETHODCALLTYPE *GetObject )( 
            DTE2 * This,
            /* [in][idldescattr] */ __RPC__in BSTR Name,
            /* [retval][out] */ __RPC__deref_out_opt IDispatch **retval);
        
        /* [id][propget][funcdescattr] */ HRESULT ( STDMETHODCALLTYPE *get_Properties )( 
            DTE2 * This,
            /* [idldescattr] */ __RPC__in BSTR Category,
            /* [idldescattr] */ __RPC__in BSTR Page,
            /* [retval][out] */ __RPC__deref_out_opt Properties **retval);
        
        /* [id][propget][funcdescattr] */ HRESULT ( STDMETHODCALLTYPE *get_SelectedItems )( 
            DTE2 * This,
            /* [retval][out] */ __RPC__deref_out_opt SelectedItems **retval);
        
        /* [id][propget][funcdescattr] */ HRESULT ( STDMETHODCALLTYPE *get_CommandLineArguments )( 
            DTE2 * This,
            /* [retval][out] */ __RPC__deref_out_opt BSTR *retval);
        
        /* [id][hidden][funcdescattr] */ HRESULT ( STDMETHODCALLTYPE *OpenFile )( 
            DTE2 * This,
            /* [in][idldescattr] */ __RPC__in BSTR ViewKind,
            /* [in][idldescattr] */ __RPC__in BSTR FileName,
            /* [retval][out] */ __RPC__deref_out_opt Window **retval);
        
        /* [id][propget][hidden][funcdescattr] */ HRESULT ( STDMETHODCALLTYPE *get_IsOpenFile )( 
            DTE2 * This,
            /* [in][idldescattr] */ __RPC__in BSTR ViewKind,
            /* [in][idldescattr] */ __RPC__in BSTR FileName,
            /* [retval][out] */ __RPC__out BOOLEAN *retval);
        
        /* [id][propget][funcdescattr] */ HRESULT ( STDMETHODCALLTYPE *get_DTE )( 
            DTE2 * This,
            /* [retval][out] */ __RPC__deref_out_opt **retval);
        
        /* [id][propget][funcdescattr] */ HRESULT ( STDMETHODCALLTYPE *get_LocaleID )( 
            DTE2 * This,
            /* [retval][out] */ __RPC__out signed long *retval);
        
        /* [id][propget][funcdescattr] */ HRESULT ( STDMETHODCALLTYPE *get_WindowConfigurations )( 
            DTE2 * This,
            /* [retval][out] */ __RPC__deref_out_opt WindowConfigurations **retval);
        
        /* [id][propget][funcdescattr] */ HRESULT ( STDMETHODCALLTYPE *get_Documents )( 
            DTE2 * This,
            /* [retval][out] */ __RPC__deref_out_opt Documents **retval);
        
        /* [id][propget][funcdescattr] */ HRESULT ( STDMETHODCALLTYPE *get_ActiveDocument )( 
            DTE2 * This,
            /* [retval][out] */ __RPC__deref_out_opt Document **retval);
        
        /* [id][funcdescattr] */ HRESULT ( STDMETHODCALLTYPE *ExecuteCommand )( 
            DTE2 * This,
            /* [in][idldescattr] */ __RPC__in BSTR CommandName,
            /* [in][idldescattr] */ __RPC__in BSTR CommandArgs,
            /* [retval][out] */ __RPC__out void *retval);
        
        /* [id][propget][funcdescattr] */ HRESULT ( STDMETHODCALLTYPE *get_Globals )( 
            DTE2 * This,
            /* [retval][out] */ __RPC__deref_out_opt Globals **retval);
        
        /* [id][propget][funcdescattr] */ HRESULT ( STDMETHODCALLTYPE *get_StatusBar )( 
            DTE2 * This,
            /* [retval][out] */ __RPC__deref_out_opt StatusBar **retval);
        
        /* [id][propget][funcdescattr] */ HRESULT ( STDMETHODCALLTYPE *get_FullName )( 
            DTE2 * This,
            /* [retval][out] */ __RPC__deref_out_opt BSTR *retval);
        
        /* [id][propget][funcdescattr] */ HRESULT ( STDMETHODCALLTYPE *get_UserControl )( 
            DTE2 * This,
            /* [retval][out] */ __RPC__out BOOLEAN *retval);
        
        /* [id][propput][funcdescattr] */ HRESULT ( STDMETHODCALLTYPE *put_UserControl )( 
            DTE2 * This,
            /* [in][idldescattr] */ BOOLEAN noname,
            /* [retval][out] */ __RPC__out void *retval);
        
        /* [id][propget][funcdescattr] */ HRESULT ( STDMETHODCALLTYPE *get_ObjectExtenders )( 
            DTE2 * This,
            /* [retval][out] */ __RPC__deref_out_opt ObjectExtenders **retval);
        
        /* [id][propget][funcdescattr] */ HRESULT ( STDMETHODCALLTYPE *get_Find )( 
            DTE2 * This,
            /* [retval][out] */ __RPC__deref_out_opt Find **retval);
        
        /* [id][propget][funcdescattr] */ HRESULT ( STDMETHODCALLTYPE *get_Mode )( 
            DTE2 * This,
            /* [retval][out] */ __RPC__out enum vsIDEMode *retval);
        
        /* [id][funcdescattr] */ HRESULT ( STDMETHODCALLTYPE *LaunchWizard )( 
            DTE2 * This,
            /* [in][idldescattr] */ __RPC__in BSTR VSZFile,
            /* [in][idldescattr] */ __RPC__deref_in_opt SAFEARRAY * *ContextParams,
            /* [retval][out] */ __RPC__out enum wizardResult *retval);
        
        /* [id][propget][funcdescattr] */ HRESULT ( STDMETHODCALLTYPE *get_ItemOperations )( 
            DTE2 * This,
            /* [retval][out] */ __RPC__deref_out_opt ItemOperations **retval);
        
        /* [id][propget][funcdescattr] */ HRESULT ( STDMETHODCALLTYPE *get_UndoContext )( 
            DTE2 * This,
            /* [retval][out] */ __RPC__deref_out_opt UndoContext **retval);
        
        /* [id][propget][funcdescattr] */ HRESULT ( STDMETHODCALLTYPE *get_Macros )( 
            DTE2 * This,
            /* [retval][out] */ __RPC__deref_out_opt Macros **retval);
        
        /* [id][propget][funcdescattr] */ HRESULT ( STDMETHODCALLTYPE *get_ActiveSolutionProjects )( 
            DTE2 * This,
            /* [retval][out] */ __RPC__out VARIANT *retval);
        
        /* [id][propget][funcdescattr] */ HRESULT ( STDMETHODCALLTYPE *get_MacrosIDE )( 
            DTE2 * This,
            /* [retval][out] */ __RPC__deref_out_opt **retval);
        
        /* [id][propget][funcdescattr] */ HRESULT ( STDMETHODCALLTYPE *get_RegistryRoot )( 
            DTE2 * This,
            /* [retval][out] */ __RPC__deref_out_opt BSTR *retval);
        
        /* [id][propget][hidden][funcdescattr] */ HRESULT ( STDMETHODCALLTYPE *get_Application )( 
            DTE2 * This,
            /* [retval][out] */ __RPC__deref_out_opt **retval);
        
        /* [id][propget][funcdescattr] */ HRESULT ( STDMETHODCALLTYPE *get_ContextAttributes )( 
            DTE2 * This,
            /* [retval][out] */ __RPC__deref_out_opt ContextAttributes **retval);
        
        /* [id][propget][funcdescattr] */ HRESULT ( STDMETHODCALLTYPE *get_SourceControl )( 
            DTE2 * This,
            /* [retval][out] */ __RPC__deref_out_opt SourceControl **retval);
        
        /* [id][propget][funcdescattr] */ HRESULT ( STDMETHODCALLTYPE *get_SuppressUI )( 
            DTE2 * This,
            /* [retval][out] */ __RPC__out BOOLEAN *retval);
        
        /* [id][propput][funcdescattr] */ HRESULT ( STDMETHODCALLTYPE *put_SuppressUI )( 
            DTE2 * This,
            /* [in][idldescattr] */ BOOLEAN noname,
            /* [retval][out] */ __RPC__out void *retval);
        
        /* [id][propget][funcdescattr] */ HRESULT ( STDMETHODCALLTYPE *get_Debugger )( 
            DTE2 * This,
            /* [retval][out] */ __RPC__deref_out_opt Debugger **retval);
        
        /* [id][funcdescattr] */ HRESULT ( STDMETHODCALLTYPE *SatelliteDllPath )( 
            DTE2 * This,
            /* [idldescattr] */ __RPC__in BSTR Path,
            /* [idldescattr] */ __RPC__in BSTR Name,
            /* [retval][out] */ __RPC__deref_out_opt BSTR *retval);
        
        /* [id][propget][funcdescattr] */ HRESULT ( STDMETHODCALLTYPE *get_Edition )( 
            DTE2 * This,
            /* [retval][out] */ __RPC__deref_out_opt BSTR *retval);
        
        /* [helpstringcontext][helpstring][helpcontext][propget][id] */ HRESULT ( STDMETHODCALLTYPE *get_ToolWindows )( 
            DTE2 * This,
            /* [retval][out] */ __RPC__deref_out_opt ToolWindows **ppcbs);
        
        /* [helpstring][id] */ HRESULT ( STDMETHODCALLTYPE *GetThemeColor )( 
            DTE2 * This,
            vsThemeColors Element,
            /* [retval][out] */ __RPC__out /* external definition not present */ OLE_COLOR *pColor);
        
        END_INTERFACE
    } DTE2Vtbl;

    interface DTE2
    {
        CONST_VTBL struct DTE2Vtbl *lpVtbl;
    };

    

#ifdef COBJMACROS


#define DTE2_QueryInterface(This,riid,ppvObj,retval)	\
    ( (This)->lpVtbl -> QueryInterface(This,riid,ppvObj,retval) ) 

#define DTE2_AddRef(This,retval)	\
    ( (This)->lpVtbl -> AddRef(This,retval) ) 

#define DTE2_Release(This,retval)	\
    ( (This)->lpVtbl -> Release(This,retval) ) 

#define DTE2_GetTypeInfoCount(This,pctinfo,retval)	\
    ( (This)->lpVtbl -> GetTypeInfoCount(This,pctinfo,retval) ) 

#define DTE2_GetTypeInfo(This,itinfo,lcid,pptinfo,retval)	\
    ( (This)->lpVtbl -> GetTypeInfo(This,itinfo,lcid,pptinfo,retval) ) 

#define DTE2_GetIDsOfNames(This,riid,rgszNames,cNames,lcid,rgdispid,retval)	\
    ( (This)->lpVtbl -> GetIDsOfNames(This,riid,rgszNames,cNames,lcid,rgdispid,retval) ) 

#define DTE2_Invoke(This,dispidMember,riid,lcid,wFlags,pdispparams,pvarResult,pexcepinfo,puArgErr,retval)	\
    ( (This)->lpVtbl -> Invoke(This,dispidMember,riid,lcid,wFlags,pdispparams,pvarResult,pexcepinfo,puArgErr,retval) ) 

#define DTE2_get_Name(This,retval)	\
    ( (This)->lpVtbl -> get_Name(This,retval) ) 

#define DTE2_get_FileName(This,retval)	\
    ( (This)->lpVtbl -> get_FileName(This,retval) ) 

#define DTE2_get_Version(This,retval)	\
    ( (This)->lpVtbl -> get_Version(This,retval) ) 

#define DTE2_get_CommandBars(This,retval)	\
    ( (This)->lpVtbl -> get_CommandBars(This,retval) ) 

#define DTE2_get_Windows(This,retval)	\
    ( (This)->lpVtbl -> get_Windows(This,retval) ) 

#define DTE2_get_Events(This,retval)	\
    ( (This)->lpVtbl -> get_Events(This,retval) ) 

#define DTE2_get_AddIns(This,retval)	\
    ( (This)->lpVtbl -> get_AddIns(This,retval) ) 

#define DTE2_get_MainWindow(This,retval)	\
    ( (This)->lpVtbl -> get_MainWindow(This,retval) ) 

#define DTE2_get_ActiveWindow(This,retval)	\
    ( (This)->lpVtbl -> get_ActiveWindow(This,retval) ) 

#define DTE2_Quit(This,retval)	\
    ( (This)->lpVtbl -> Quit(This,retval) ) 

#define DTE2_get_DisplayMode(This,retval)	\
    ( (This)->lpVtbl -> get_DisplayMode(This,retval) ) 

#define DTE2_put_DisplayMode(This,noname,retval)	\
    ( (This)->lpVtbl -> put_DisplayMode(This,noname,retval) ) 

#define DTE2_get_Solution(This,retval)	\
    ( (This)->lpVtbl -> get_Solution(This,retval) ) 

#define DTE2_get_Commands(This,retval)	\
    ( (This)->lpVtbl -> get_Commands(This,retval) ) 

#define DTE2_GetObject(This,Name,retval)	\
    ( (This)->lpVtbl -> GetObject(This,Name,retval) ) 

#define DTE2_get_Properties(This,Category,Page,retval)	\
    ( (This)->lpVtbl -> get_Properties(This,Category,Page,retval) ) 

#define DTE2_get_SelectedItems(This,retval)	\
    ( (This)->lpVtbl -> get_SelectedItems(This,retval) ) 

#define DTE2_get_CommandLineArguments(This,retval)	\
    ( (This)->lpVtbl -> get_CommandLineArguments(This,retval) ) 

#define DTE2_OpenFile(This,ViewKind,FileName,retval)	\
    ( (This)->lpVtbl -> OpenFile(This,ViewKind,FileName,retval) ) 

#define DTE2_get_IsOpenFile(This,ViewKind,FileName,retval)	\
    ( (This)->lpVtbl -> get_IsOpenFile(This,ViewKind,FileName,retval) ) 

#define DTE2_get_DTE(This,retval)	\
    ( (This)->lpVtbl -> get_DTE(This,retval) ) 

#define DTE2_get_LocaleID(This,retval)	\
    ( (This)->lpVtbl -> get_LocaleID(This,retval) ) 

#define DTE2_get_WindowConfigurations(This,retval)	\
    ( (This)->lpVtbl -> get_WindowConfigurations(This,retval) ) 

#define DTE2_get_Documents(This,retval)	\
    ( (This)->lpVtbl -> get_Documents(This,retval) ) 

#define DTE2_get_ActiveDocument(This,retval)	\
    ( (This)->lpVtbl -> get_ActiveDocument(This,retval) ) 

#define DTE2_ExecuteCommand(This,CommandName,CommandArgs,retval)	\
    ( (This)->lpVtbl -> ExecuteCommand(This,CommandName,CommandArgs,retval) ) 

#define DTE2_get_Globals(This,retval)	\
    ( (This)->lpVtbl -> get_Globals(This,retval) ) 

#define DTE2_get_StatusBar(This,retval)	\
    ( (This)->lpVtbl -> get_StatusBar(This,retval) ) 

#define DTE2_get_FullName(This,retval)	\
    ( (This)->lpVtbl -> get_FullName(This,retval) ) 

#define DTE2_get_UserControl(This,retval)	\
    ( (This)->lpVtbl -> get_UserControl(This,retval) ) 

#define DTE2_put_UserControl(This,noname,retval)	\
    ( (This)->lpVtbl -> put_UserControl(This,noname,retval) ) 

#define DTE2_get_ObjectExtenders(This,retval)	\
    ( (This)->lpVtbl -> get_ObjectExtenders(This,retval) ) 

#define DTE2_get_Find(This,retval)	\
    ( (This)->lpVtbl -> get_Find(This,retval) ) 

#define DTE2_get_Mode(This,retval)	\
    ( (This)->lpVtbl -> get_Mode(This,retval) ) 

#define DTE2_LaunchWizard(This,VSZFile,ContextParams,retval)	\
    ( (This)->lpVtbl -> LaunchWizard(This,VSZFile,ContextParams,retval) ) 

#define DTE2_get_ItemOperations(This,retval)	\
    ( (This)->lpVtbl -> get_ItemOperations(This,retval) ) 

#define DTE2_get_UndoContext(This,retval)	\
    ( (This)->lpVtbl -> get_UndoContext(This,retval) ) 

#define DTE2_get_Macros(This,retval)	\
    ( (This)->lpVtbl -> get_Macros(This,retval) ) 

#define DTE2_get_ActiveSolutionProjects(This,retval)	\
    ( (This)->lpVtbl -> get_ActiveSolutionProjects(This,retval) ) 

#define DTE2_get_MacrosIDE(This,retval)	\
    ( (This)->lpVtbl -> get_MacrosIDE(This,retval) ) 

#define DTE2_get_RegistryRoot(This,retval)	\
    ( (This)->lpVtbl -> get_RegistryRoot(This,retval) ) 

#define DTE2_get_Application(This,retval)	\
    ( (This)->lpVtbl -> get_Application(This,retval) ) 

#define DTE2_get_ContextAttributes(This,retval)	\
    ( (This)->lpVtbl -> get_ContextAttributes(This,retval) ) 

#define DTE2_get_SourceControl(This,retval)	\
    ( (This)->lpVtbl -> get_SourceControl(This,retval) ) 

#define DTE2_get_SuppressUI(This,retval)	\
    ( (This)->lpVtbl -> get_SuppressUI(This,retval) ) 

#define DTE2_put_SuppressUI(This,noname,retval)	\
    ( (This)->lpVtbl -> put_SuppressUI(This,noname,retval) ) 

#define DTE2_get_Debugger(This,retval)	\
    ( (This)->lpVtbl -> get_Debugger(This,retval) ) 

#define DTE2_SatelliteDllPath(This,Path,Name,retval)	\
    ( (This)->lpVtbl -> SatelliteDllPath(This,Path,Name,retval) ) 

#define DTE2_get_Edition(This,retval)	\
    ( (This)->lpVtbl -> get_Edition(This,retval) ) 


#define DTE2_get_ToolWindows(This,ppcbs)	\
    ( (This)->lpVtbl -> get_ToolWindows(This,ppcbs) ) 

#define DTE2_GetThemeColor(This,Element,pColor)	\
    ( (This)->lpVtbl -> GetThemeColor(This,Element,pColor) ) 

#endif /* COBJMACROS */


#endif 	/* C style interface */




#endif 	/* __DTE2_INTERFACE_DEFINED__ */


#ifndef __WindowVisibilityEventsRoot_INTERFACE_DEFINED__
#define __WindowVisibilityEventsRoot_INTERFACE_DEFINED__

/* interface WindowVisibilityEventsRoot */
/* [hidden][uuid][object][oleautomation][dual] */ 


EXTERN_C const IID IID_WindowVisibilityEventsRoot;

#if defined(__cplusplus) && !defined(CINTERFACE)
    
    MIDL_INTERFACE("C7B4BCC9-E23E-400C-8821-C85B6B31369E")
    WindowVisibilityEventsRoot : public IDispatch
    {
    public:
        virtual /* [hidden][propget][id] */ HRESULT __stdcall get_WindowVisibilityEvents( 
            /* [in] */ __RPC__in /* external definition not present */ Window *WindowFilter,
            /* [retval][out] */ __RPC__deref_out_opt IDispatch **ppdisp) = 0;
        
    };
    
#else 	/* C style interface */

    typedef struct WindowVisibilityEventsRootVtbl
    {
        BEGIN_INTERFACE
        
        HRESULT ( STDMETHODCALLTYPE *QueryInterface )( 
            WindowVisibilityEventsRoot * This,
            /* [in] */ __RPC__in REFIID riid,
            /* [iid_is][out] */ 
            __RPC__deref_out  void **ppvObject);
        
        ULONG ( STDMETHODCALLTYPE *AddRef )( 
            WindowVisibilityEventsRoot * This);
        
        ULONG ( STDMETHODCALLTYPE *Release )( 
            WindowVisibilityEventsRoot * This);
        
        HRESULT ( STDMETHODCALLTYPE *GetTypeInfoCount )( 
            WindowVisibilityEventsRoot * This,
            /* [out] */ __RPC__out UINT *pctinfo);
        
        HRESULT ( STDMETHODCALLTYPE *GetTypeInfo )( 
            WindowVisibilityEventsRoot * This,
            /* [in] */ UINT iTInfo,
            /* [in] */ LCID lcid,
            /* [out] */ __RPC__deref_out_opt ITypeInfo **ppTInfo);
        
        HRESULT ( STDMETHODCALLTYPE *GetIDsOfNames )( 
            WindowVisibilityEventsRoot * This,
            /* [in] */ __RPC__in REFIID riid,
            /* [size_is][in] */ __RPC__in_ecount_full(cNames) LPOLESTR *rgszNames,
            /* [range][in] */ UINT cNames,
            /* [in] */ LCID lcid,
            /* [size_is][out] */ __RPC__out_ecount_full(cNames) DISPID *rgDispId);
        
        /* [local] */ HRESULT ( STDMETHODCALLTYPE *Invoke )( 
            WindowVisibilityEventsRoot * This,
            /* [in] */ DISPID dispIdMember,
            /* [in] */ REFIID riid,
            /* [in] */ LCID lcid,
            /* [in] */ WORD wFlags,
            /* [out][in] */ DISPPARAMS *pDispParams,
            /* [out] */ VARIANT *pVarResult,
            /* [out] */ EXCEPINFO *pExcepInfo,
            /* [out] */ UINT *puArgErr);
        
        /* [hidden][propget][id] */ HRESULT ( __stdcall *get_WindowVisibilityEvents )( 
            WindowVisibilityEventsRoot * This,
            /* [in] */ __RPC__in /* external definition not present */ Window *WindowFilter,
            /* [retval][out] */ __RPC__deref_out_opt IDispatch **ppdisp);
        
        END_INTERFACE
    } WindowVisibilityEventsRootVtbl;

    interface WindowVisibilityEventsRoot
    {
        CONST_VTBL struct WindowVisibilityEventsRootVtbl *lpVtbl;
    };

    

#ifdef COBJMACROS


#define WindowVisibilityEventsRoot_QueryInterface(This,riid,ppvObject)	\
    ( (This)->lpVtbl -> QueryInterface(This,riid,ppvObject) ) 

#define WindowVisibilityEventsRoot_AddRef(This)	\
    ( (This)->lpVtbl -> AddRef(This) ) 

#define WindowVisibilityEventsRoot_Release(This)	\
    ( (This)->lpVtbl -> Release(This) ) 


#define WindowVisibilityEventsRoot_GetTypeInfoCount(This,pctinfo)	\
    ( (This)->lpVtbl -> GetTypeInfoCount(This,pctinfo) ) 

#define WindowVisibilityEventsRoot_GetTypeInfo(This,iTInfo,lcid,ppTInfo)	\
    ( (This)->lpVtbl -> GetTypeInfo(This,iTInfo,lcid,ppTInfo) ) 

#define WindowVisibilityEventsRoot_GetIDsOfNames(This,riid,rgszNames,cNames,lcid,rgDispId)	\
    ( (This)->lpVtbl -> GetIDsOfNames(This,riid,rgszNames,cNames,lcid,rgDispId) ) 

#define WindowVisibilityEventsRoot_Invoke(This,dispIdMember,riid,lcid,wFlags,pDispParams,pVarResult,pExcepInfo,puArgErr)	\
    ( (This)->lpVtbl -> Invoke(This,dispIdMember,riid,lcid,wFlags,pDispParams,pVarResult,pExcepInfo,puArgErr) ) 


#define WindowVisibilityEventsRoot_get_WindowVisibilityEvents(This,WindowFilter,ppdisp)	\
    ( (This)->lpVtbl -> get_WindowVisibilityEvents(This,WindowFilter,ppdisp) ) 

#endif /* COBJMACROS */


#endif 	/* C style interface */




#endif 	/* __WindowVisibilityEventsRoot_INTERFACE_DEFINED__ */


#ifndef ___WindowVisibilityEvents_INTERFACE_DEFINED__
#define ___WindowVisibilityEvents_INTERFACE_DEFINED__

/* interface _WindowVisibilityEvents */
/* [helpstring][uuid][object][oleautomation][dual] */ 


EXTERN_C const IID IID__WindowVisibilityEvents;

#if defined(__cplusplus) && !defined(CINTERFACE)
    
    MIDL_INTERFACE("84DE07BC-43A2-4275-BCF9-D207D20E49ED")
    _WindowVisibilityEvents : public IDispatch
    {
    public:
    };
    
#else 	/* C style interface */

    typedef struct _WindowVisibilityEventsVtbl
    {
        BEGIN_INTERFACE
        
        HRESULT ( STDMETHODCALLTYPE *QueryInterface )( 
            _WindowVisibilityEvents * This,
            /* [in] */ __RPC__in REFIID riid,
            /* [iid_is][out] */ 
            __RPC__deref_out  void **ppvObject);
        
        ULONG ( STDMETHODCALLTYPE *AddRef )( 
            _WindowVisibilityEvents * This);
        
        ULONG ( STDMETHODCALLTYPE *Release )( 
            _WindowVisibilityEvents * This);
        
        HRESULT ( STDMETHODCALLTYPE *GetTypeInfoCount )( 
            _WindowVisibilityEvents * This,
            /* [out] */ __RPC__out UINT *pctinfo);
        
        HRESULT ( STDMETHODCALLTYPE *GetTypeInfo )( 
            _WindowVisibilityEvents * This,
            /* [in] */ UINT iTInfo,
            /* [in] */ LCID lcid,
            /* [out] */ __RPC__deref_out_opt ITypeInfo **ppTInfo);
        
        HRESULT ( STDMETHODCALLTYPE *GetIDsOfNames )( 
            _WindowVisibilityEvents * This,
            /* [in] */ __RPC__in REFIID riid,
            /* [size_is][in] */ __RPC__in_ecount_full(cNames) LPOLESTR *rgszNames,
            /* [range][in] */ UINT cNames,
            /* [in] */ LCID lcid,
            /* [size_is][out] */ __RPC__out_ecount_full(cNames) DISPID *rgDispId);
        
        /* [local] */ HRESULT ( STDMETHODCALLTYPE *Invoke )( 
            _WindowVisibilityEvents * This,
            /* [in] */ DISPID dispIdMember,
            /* [in] */ REFIID riid,
            /* [in] */ LCID lcid,
            /* [in] */ WORD wFlags,
            /* [out][in] */ DISPPARAMS *pDispParams,
            /* [out] */ VARIANT *pVarResult,
            /* [out] */ EXCEPINFO *pExcepInfo,
            /* [out] */ UINT *puArgErr);
        
        END_INTERFACE
    } _WindowVisibilityEventsVtbl;

    interface _WindowVisibilityEvents
    {
        CONST_VTBL struct _WindowVisibilityEventsVtbl *lpVtbl;
    };

    

#ifdef COBJMACROS


#define _WindowVisibilityEvents_QueryInterface(This,riid,ppvObject)	\
    ( (This)->lpVtbl -> QueryInterface(This,riid,ppvObject) ) 

#define _WindowVisibilityEvents_AddRef(This)	\
    ( (This)->lpVtbl -> AddRef(This) ) 

#define _WindowVisibilityEvents_Release(This)	\
    ( (This)->lpVtbl -> Release(This) ) 


#define _WindowVisibilityEvents_GetTypeInfoCount(This,pctinfo)	\
    ( (This)->lpVtbl -> GetTypeInfoCount(This,pctinfo) ) 

#define _WindowVisibilityEvents_GetTypeInfo(This,iTInfo,lcid,ppTInfo)	\
    ( (This)->lpVtbl -> GetTypeInfo(This,iTInfo,lcid,ppTInfo) ) 

#define _WindowVisibilityEvents_GetIDsOfNames(This,riid,rgszNames,cNames,lcid,rgDispId)	\
    ( (This)->lpVtbl -> GetIDsOfNames(This,riid,rgszNames,cNames,lcid,rgDispId) ) 

#define _WindowVisibilityEvents_Invoke(This,dispIdMember,riid,lcid,wFlags,pDispParams,pVarResult,pExcepInfo,puArgErr)	\
    ( (This)->lpVtbl -> Invoke(This,dispIdMember,riid,lcid,wFlags,pDispParams,pVarResult,pExcepInfo,puArgErr) ) 


#endif /* COBJMACROS */


#endif 	/* C style interface */




#endif 	/* ___WindowVisibilityEvents_INTERFACE_DEFINED__ */


#ifndef ___dispWindowVisibilityEvents_DISPINTERFACE_DEFINED__
#define ___dispWindowVisibilityEvents_DISPINTERFACE_DEFINED__

/* dispinterface _dispWindowVisibilityEvents */
/* [uuid] */ 


EXTERN_C const IID DIID__dispWindowVisibilityEvents;

#if defined(__cplusplus) && !defined(CINTERFACE)

    MIDL_INTERFACE("94259E4B-A44A-4B77-B18F-F2CC9A601D03")
    _dispWindowVisibilityEvents : public IDispatch
    {
    };
    
#else 	/* C style interface */

    typedef struct _dispWindowVisibilityEventsVtbl
    {
        BEGIN_INTERFACE
        
        HRESULT ( STDMETHODCALLTYPE *QueryInterface )( 
            _dispWindowVisibilityEvents * This,
            /* [in] */ __RPC__in REFIID riid,
            /* [iid_is][out] */ 
            __RPC__deref_out  void **ppvObject);
        
        ULONG ( STDMETHODCALLTYPE *AddRef )( 
            _dispWindowVisibilityEvents * This);
        
        ULONG ( STDMETHODCALLTYPE *Release )( 
            _dispWindowVisibilityEvents * This);
        
        HRESULT ( STDMETHODCALLTYPE *GetTypeInfoCount )( 
            _dispWindowVisibilityEvents * This,
            /* [out] */ __RPC__out UINT *pctinfo);
        
        HRESULT ( STDMETHODCALLTYPE *GetTypeInfo )( 
            _dispWindowVisibilityEvents * This,
            /* [in] */ UINT iTInfo,
            /* [in] */ LCID lcid,
            /* [out] */ __RPC__deref_out_opt ITypeInfo **ppTInfo);
        
        HRESULT ( STDMETHODCALLTYPE *GetIDsOfNames )( 
            _dispWindowVisibilityEvents * This,
            /* [in] */ __RPC__in REFIID riid,
            /* [size_is][in] */ __RPC__in_ecount_full(cNames) LPOLESTR *rgszNames,
            /* [range][in] */ UINT cNames,
            /* [in] */ LCID lcid,
            /* [size_is][out] */ __RPC__out_ecount_full(cNames) DISPID *rgDispId);
        
        /* [local] */ HRESULT ( STDMETHODCALLTYPE *Invoke )( 
            _dispWindowVisibilityEvents * This,
            /* [in] */ DISPID dispIdMember,
            /* [in] */ REFIID riid,
            /* [in] */ LCID lcid,
            /* [in] */ WORD wFlags,
            /* [out][in] */ DISPPARAMS *pDispParams,
            /* [out] */ VARIANT *pVarResult,
            /* [out] */ EXCEPINFO *pExcepInfo,
            /* [out] */ UINT *puArgErr);
        
        END_INTERFACE
    } _dispWindowVisibilityEventsVtbl;

    interface _dispWindowVisibilityEvents
    {
        CONST_VTBL struct _dispWindowVisibilityEventsVtbl *lpVtbl;
    };

    

#ifdef COBJMACROS


#define _dispWindowVisibilityEvents_QueryInterface(This,riid,ppvObject)	\
    ( (This)->lpVtbl -> QueryInterface(This,riid,ppvObject) ) 

#define _dispWindowVisibilityEvents_AddRef(This)	\
    ( (This)->lpVtbl -> AddRef(This) ) 

#define _dispWindowVisibilityEvents_Release(This)	\
    ( (This)->lpVtbl -> Release(This) ) 


#define _dispWindowVisibilityEvents_GetTypeInfoCount(This,pctinfo)	\
    ( (This)->lpVtbl -> GetTypeInfoCount(This,pctinfo) ) 

#define _dispWindowVisibilityEvents_GetTypeInfo(This,iTInfo,lcid,ppTInfo)	\
    ( (This)->lpVtbl -> GetTypeInfo(This,iTInfo,lcid,ppTInfo) ) 

#define _dispWindowVisibilityEvents_GetIDsOfNames(This,riid,rgszNames,cNames,lcid,rgDispId)	\
    ( (This)->lpVtbl -> GetIDsOfNames(This,riid,rgszNames,cNames,lcid,rgDispId) ) 

#define _dispWindowVisibilityEvents_Invoke(This,dispIdMember,riid,lcid,wFlags,pDispParams,pVarResult,pExcepInfo,puArgErr)	\
    ( (This)->lpVtbl -> Invoke(This,dispIdMember,riid,lcid,wFlags,pDispParams,pVarResult,pExcepInfo,puArgErr) ) 

#endif /* COBJMACROS */


#endif 	/* C style interface */


#endif 	/* ___dispWindowVisibilityEvents_DISPINTERFACE_DEFINED__ */


EXTERN_C const CLSID CLSID_WindowVisibilityEvents;

#ifdef __cplusplus

class DECLSPEC_UUID("8B7E04AD-B109-4B96-8BA3-2F348813F238")
WindowVisibilityEvents;
#endif

#ifndef ___TextDocumentKeyPressEventsRoot_INTERFACE_DEFINED__
#define ___TextDocumentKeyPressEventsRoot_INTERFACE_DEFINED__

/* interface _TextDocumentKeyPressEventsRoot */
/* [hidden][uuid][object][oleautomation][dual] */ 


EXTERN_C const IID IID__TextDocumentKeyPressEventsRoot;

#if defined(__cplusplus) && !defined(CINTERFACE)
    
    MIDL_INTERFACE("D7EADE3F-BD19-4422-9C4C-85A02535437C")
    _TextDocumentKeyPressEventsRoot : public IDispatch
    {
    public:
        virtual /* [hidden][propget][id] */ HRESULT __stdcall get_TextDocumentKeyPressEvents( 
            /* [in] */ __RPC__in /* external definition not present */ TextDocument *TextDocument,
            /* [retval][out] */ __RPC__deref_out_opt IDispatch **ppdisp) = 0;
        
    };
    
#else 	/* C style interface */

    typedef struct _TextDocumentKeyPressEventsRootVtbl
    {
        BEGIN_INTERFACE
        
        HRESULT ( STDMETHODCALLTYPE *QueryInterface )( 
            _TextDocumentKeyPressEventsRoot * This,
            /* [in] */ __RPC__in REFIID riid,
            /* [iid_is][out] */ 
            __RPC__deref_out  void **ppvObject);
        
        ULONG ( STDMETHODCALLTYPE *AddRef )( 
            _TextDocumentKeyPressEventsRoot * This);
        
        ULONG ( STDMETHODCALLTYPE *Release )( 
            _TextDocumentKeyPressEventsRoot * This);
        
        HRESULT ( STDMETHODCALLTYPE *GetTypeInfoCount )( 
            _TextDocumentKeyPressEventsRoot * This,
            /* [out] */ __RPC__out UINT *pctinfo);
        
        HRESULT ( STDMETHODCALLTYPE *GetTypeInfo )( 
            _TextDocumentKeyPressEventsRoot * This,
            /* [in] */ UINT iTInfo,
            /* [in] */ LCID lcid,
            /* [out] */ __RPC__deref_out_opt ITypeInfo **ppTInfo);
        
        HRESULT ( STDMETHODCALLTYPE *GetIDsOfNames )( 
            _TextDocumentKeyPressEventsRoot * This,
            /* [in] */ __RPC__in REFIID riid,
            /* [size_is][in] */ __RPC__in_ecount_full(cNames) LPOLESTR *rgszNames,
            /* [range][in] */ UINT cNames,
            /* [in] */ LCID lcid,
            /* [size_is][out] */ __RPC__out_ecount_full(cNames) DISPID *rgDispId);
        
        /* [local] */ HRESULT ( STDMETHODCALLTYPE *Invoke )( 
            _TextDocumentKeyPressEventsRoot * This,
            /* [in] */ DISPID dispIdMember,
            /* [in] */ REFIID riid,
            /* [in] */ LCID lcid,
            /* [in] */ WORD wFlags,
            /* [out][in] */ DISPPARAMS *pDispParams,
            /* [out] */ VARIANT *pVarResult,
            /* [out] */ EXCEPINFO *pExcepInfo,
            /* [out] */ UINT *puArgErr);
        
        /* [hidden][propget][id] */ HRESULT ( __stdcall *get_TextDocumentKeyPressEvents )( 
            _TextDocumentKeyPressEventsRoot * This,
            /* [in] */ __RPC__in /* external definition not present */ TextDocument *TextDocument,
            /* [retval][out] */ __RPC__deref_out_opt IDispatch **ppdisp);
        
        END_INTERFACE
    } _TextDocumentKeyPressEventsRootVtbl;

    interface _TextDocumentKeyPressEventsRoot
    {
        CONST_VTBL struct _TextDocumentKeyPressEventsRootVtbl *lpVtbl;
    };

    

#ifdef COBJMACROS


#define _TextDocumentKeyPressEventsRoot_QueryInterface(This,riid,ppvObject)	\
    ( (This)->lpVtbl -> QueryInterface(This,riid,ppvObject) ) 

#define _TextDocumentKeyPressEventsRoot_AddRef(This)	\
    ( (This)->lpVtbl -> AddRef(This) ) 

#define _TextDocumentKeyPressEventsRoot_Release(This)	\
    ( (This)->lpVtbl -> Release(This) ) 


#define _TextDocumentKeyPressEventsRoot_GetTypeInfoCount(This,pctinfo)	\
    ( (This)->lpVtbl -> GetTypeInfoCount(This,pctinfo) ) 

#define _TextDocumentKeyPressEventsRoot_GetTypeInfo(This,iTInfo,lcid,ppTInfo)	\
    ( (This)->lpVtbl -> GetTypeInfo(This,iTInfo,lcid,ppTInfo) ) 

#define _TextDocumentKeyPressEventsRoot_GetIDsOfNames(This,riid,rgszNames,cNames,lcid,rgDispId)	\
    ( (This)->lpVtbl -> GetIDsOfNames(This,riid,rgszNames,cNames,lcid,rgDispId) ) 

#define _TextDocumentKeyPressEventsRoot_Invoke(This,dispIdMember,riid,lcid,wFlags,pDispParams,pVarResult,pExcepInfo,puArgErr)	\
    ( (This)->lpVtbl -> Invoke(This,dispIdMember,riid,lcid,wFlags,pDispParams,pVarResult,pExcepInfo,puArgErr) ) 


#define _TextDocumentKeyPressEventsRoot_get_TextDocumentKeyPressEvents(This,TextDocument,ppdisp)	\
    ( (This)->lpVtbl -> get_TextDocumentKeyPressEvents(This,TextDocument,ppdisp) ) 

#endif /* COBJMACROS */


#endif 	/* C style interface */




#endif 	/* ___TextDocumentKeyPressEventsRoot_INTERFACE_DEFINED__ */


#ifndef ___TextDocumentKeyPressEvents_INTERFACE_DEFINED__
#define ___TextDocumentKeyPressEvents_INTERFACE_DEFINED__

/* interface _TextDocumentKeyPressEvents */
/* [helpstring][uuid][object][oleautomation][dual] */ 


EXTERN_C const IID IID__TextDocumentKeyPressEvents;

#if defined(__cplusplus) && !defined(CINTERFACE)
    
    MIDL_INTERFACE("505B7600-8FCC-487c-9E4F-C7FD0B5FB690")
    _TextDocumentKeyPressEvents : public IDispatch
    {
    public:
    };
    
#else 	/* C style interface */

    typedef struct _TextDocumentKeyPressEventsVtbl
    {
        BEGIN_INTERFACE
        
        HRESULT ( STDMETHODCALLTYPE *QueryInterface )( 
            _TextDocumentKeyPressEvents * This,
            /* [in] */ __RPC__in REFIID riid,
            /* [iid_is][out] */ 
            __RPC__deref_out  void **ppvObject);
        
        ULONG ( STDMETHODCALLTYPE *AddRef )( 
            _TextDocumentKeyPressEvents * This);
        
        ULONG ( STDMETHODCALLTYPE *Release )( 
            _TextDocumentKeyPressEvents * This);
        
        HRESULT ( STDMETHODCALLTYPE *GetTypeInfoCount )( 
            _TextDocumentKeyPressEvents * This,
            /* [out] */ __RPC__out UINT *pctinfo);
        
        HRESULT ( STDMETHODCALLTYPE *GetTypeInfo )( 
            _TextDocumentKeyPressEvents * This,
            /* [in] */ UINT iTInfo,
            /* [in] */ LCID lcid,
            /* [out] */ __RPC__deref_out_opt ITypeInfo **ppTInfo);
        
        HRESULT ( STDMETHODCALLTYPE *GetIDsOfNames )( 
            _TextDocumentKeyPressEvents * This,
            /* [in] */ __RPC__in REFIID riid,
            /* [size_is][in] */ __RPC__in_ecount_full(cNames) LPOLESTR *rgszNames,
            /* [range][in] */ UINT cNames,
            /* [in] */ LCID lcid,
            /* [size_is][out] */ __RPC__out_ecount_full(cNames) DISPID *rgDispId);
        
        /* [local] */ HRESULT ( STDMETHODCALLTYPE *Invoke )( 
            _TextDocumentKeyPressEvents * This,
            /* [in] */ DISPID dispIdMember,
            /* [in] */ REFIID riid,
            /* [in] */ LCID lcid,
            /* [in] */ WORD wFlags,
            /* [out][in] */ DISPPARAMS *pDispParams,
            /* [out] */ VARIANT *pVarResult,
            /* [out] */ EXCEPINFO *pExcepInfo,
            /* [out] */ UINT *puArgErr);
        
        END_INTERFACE
    } _TextDocumentKeyPressEventsVtbl;

    interface _TextDocumentKeyPressEvents
    {
        CONST_VTBL struct _TextDocumentKeyPressEventsVtbl *lpVtbl;
    };

    

#ifdef COBJMACROS


#define _TextDocumentKeyPressEvents_QueryInterface(This,riid,ppvObject)	\
    ( (This)->lpVtbl -> QueryInterface(This,riid,ppvObject) ) 

#define _TextDocumentKeyPressEvents_AddRef(This)	\
    ( (This)->lpVtbl -> AddRef(This) ) 

#define _TextDocumentKeyPressEvents_Release(This)	\
    ( (This)->lpVtbl -> Release(This) ) 


#define _TextDocumentKeyPressEvents_GetTypeInfoCount(This,pctinfo)	\
    ( (This)->lpVtbl -> GetTypeInfoCount(This,pctinfo) ) 

#define _TextDocumentKeyPressEvents_GetTypeInfo(This,iTInfo,lcid,ppTInfo)	\
    ( (This)->lpVtbl -> GetTypeInfo(This,iTInfo,lcid,ppTInfo) ) 

#define _TextDocumentKeyPressEvents_GetIDsOfNames(This,riid,rgszNames,cNames,lcid,rgDispId)	\
    ( (This)->lpVtbl -> GetIDsOfNames(This,riid,rgszNames,cNames,lcid,rgDispId) ) 

#define _TextDocumentKeyPressEvents_Invoke(This,dispIdMember,riid,lcid,wFlags,pDispParams,pVarResult,pExcepInfo,puArgErr)	\
    ( (This)->lpVtbl -> Invoke(This,dispIdMember,riid,lcid,wFlags,pDispParams,pVarResult,pExcepInfo,puArgErr) ) 


#endif /* COBJMACROS */


#endif 	/* C style interface */




#endif 	/* ___TextDocumentKeyPressEvents_INTERFACE_DEFINED__ */


#ifndef ___dispTextDocumentKeyPressEvents_DISPINTERFACE_DEFINED__
#define ___dispTextDocumentKeyPressEvents_DISPINTERFACE_DEFINED__

/* dispinterface _dispTextDocumentKeyPressEvents */
/* [uuid] */ 


EXTERN_C const IID DIID__dispTextDocumentKeyPressEvents;

#if defined(__cplusplus) && !defined(CINTERFACE)

    MIDL_INTERFACE("4B57C85E-5100-4caf-9301-4544B85C7945")
    _dispTextDocumentKeyPressEvents : public IDispatch
    {
    };
    
#else 	/* C style interface */

    typedef struct _dispTextDocumentKeyPressEventsVtbl
    {
        BEGIN_INTERFACE
        
        HRESULT ( STDMETHODCALLTYPE *QueryInterface )( 
            _dispTextDocumentKeyPressEvents * This,
            /* [in] */ __RPC__in REFIID riid,
            /* [iid_is][out] */ 
            __RPC__deref_out  void **ppvObject);
        
        ULONG ( STDMETHODCALLTYPE *AddRef )( 
            _dispTextDocumentKeyPressEvents * This);
        
        ULONG ( STDMETHODCALLTYPE *Release )( 
            _dispTextDocumentKeyPressEvents * This);
        
        HRESULT ( STDMETHODCALLTYPE *GetTypeInfoCount )( 
            _dispTextDocumentKeyPressEvents * This,
            /* [out] */ __RPC__out UINT *pctinfo);
        
        HRESULT ( STDMETHODCALLTYPE *GetTypeInfo )( 
            _dispTextDocumentKeyPressEvents * This,
            /* [in] */ UINT iTInfo,
            /* [in] */ LCID lcid,
            /* [out] */ __RPC__deref_out_opt ITypeInfo **ppTInfo);
        
        HRESULT ( STDMETHODCALLTYPE *GetIDsOfNames )( 
            _dispTextDocumentKeyPressEvents * This,
            /* [in] */ __RPC__in REFIID riid,
            /* [size_is][in] */ __RPC__in_ecount_full(cNames) LPOLESTR *rgszNames,
            /* [range][in] */ UINT cNames,
            /* [in] */ LCID lcid,
            /* [size_is][out] */ __RPC__out_ecount_full(cNames) DISPID *rgDispId);
        
        /* [local] */ HRESULT ( STDMETHODCALLTYPE *Invoke )( 
            _dispTextDocumentKeyPressEvents * This,
            /* [in] */ DISPID dispIdMember,
            /* [in] */ REFIID riid,
            /* [in] */ LCID lcid,
            /* [in] */ WORD wFlags,
            /* [out][in] */ DISPPARAMS *pDispParams,
            /* [out] */ VARIANT *pVarResult,
            /* [out] */ EXCEPINFO *pExcepInfo,
            /* [out] */ UINT *puArgErr);
        
        END_INTERFACE
    } _dispTextDocumentKeyPressEventsVtbl;

    interface _dispTextDocumentKeyPressEvents
    {
        CONST_VTBL struct _dispTextDocumentKeyPressEventsVtbl *lpVtbl;
    };

    

#ifdef COBJMACROS


#define _dispTextDocumentKeyPressEvents_QueryInterface(This,riid,ppvObject)	\
    ( (This)->lpVtbl -> QueryInterface(This,riid,ppvObject) ) 

#define _dispTextDocumentKeyPressEvents_AddRef(This)	\
    ( (This)->lpVtbl -> AddRef(This) ) 

#define _dispTextDocumentKeyPressEvents_Release(This)	\
    ( (This)->lpVtbl -> Release(This) ) 


#define _dispTextDocumentKeyPressEvents_GetTypeInfoCount(This,pctinfo)	\
    ( (This)->lpVtbl -> GetTypeInfoCount(This,pctinfo) ) 

#define _dispTextDocumentKeyPressEvents_GetTypeInfo(This,iTInfo,lcid,ppTInfo)	\
    ( (This)->lpVtbl -> GetTypeInfo(This,iTInfo,lcid,ppTInfo) ) 

#define _dispTextDocumentKeyPressEvents_GetIDsOfNames(This,riid,rgszNames,cNames,lcid,rgDispId)	\
    ( (This)->lpVtbl -> GetIDsOfNames(This,riid,rgszNames,cNames,lcid,rgDispId) ) 

#define _dispTextDocumentKeyPressEvents_Invoke(This,dispIdMember,riid,lcid,wFlags,pDispParams,pVarResult,pExcepInfo,puArgErr)	\
    ( (This)->lpVtbl -> Invoke(This,dispIdMember,riid,lcid,wFlags,pDispParams,pVarResult,pExcepInfo,puArgErr) ) 

#endif /* COBJMACROS */


#endif 	/* C style interface */


#endif 	/* ___dispTextDocumentKeyPressEvents_DISPINTERFACE_DEFINED__ */


EXTERN_C const CLSID CLSID_TextDocumentKeyPressEvents;

#ifdef __cplusplus

class DECLSPEC_UUID("E7532A4E-AB9E-41DF-BB9A-7C764677E5C3")
TextDocumentKeyPressEvents;
#endif

#ifndef ___PublishEvents_INTERFACE_DEFINED__
#define ___PublishEvents_INTERFACE_DEFINED__

/* interface _PublishEvents */
/* [object][helpstring][oleautomation][uuid] */ 


EXTERN_C const IID IID__PublishEvents;

#if defined(__cplusplus) && !defined(CINTERFACE)
    
    MIDL_INTERFACE("827be7dc-1817-4171-a8ff-aab3854913bf")
    _PublishEvents : public IDispatch
    {
    public:
    };
    
#else 	/* C style interface */

    typedef struct _PublishEventsVtbl
    {
        BEGIN_INTERFACE
        
        HRESULT ( STDMETHODCALLTYPE *QueryInterface )( 
            _PublishEvents * This,
            /* [in] */ __RPC__in REFIID riid,
            /* [iid_is][out] */ 
            __RPC__deref_out  void **ppvObject);
        
        ULONG ( STDMETHODCALLTYPE *AddRef )( 
            _PublishEvents * This);
        
        ULONG ( STDMETHODCALLTYPE *Release )( 
            _PublishEvents * This);
        
        HRESULT ( STDMETHODCALLTYPE *GetTypeInfoCount )( 
            _PublishEvents * This,
            /* [out] */ __RPC__out UINT *pctinfo);
        
        HRESULT ( STDMETHODCALLTYPE *GetTypeInfo )( 
            _PublishEvents * This,
            /* [in] */ UINT iTInfo,
            /* [in] */ LCID lcid,
            /* [out] */ __RPC__deref_out_opt ITypeInfo **ppTInfo);
        
        HRESULT ( STDMETHODCALLTYPE *GetIDsOfNames )( 
            _PublishEvents * This,
            /* [in] */ __RPC__in REFIID riid,
            /* [size_is][in] */ __RPC__in_ecount_full(cNames) LPOLESTR *rgszNames,
            /* [range][in] */ UINT cNames,
            /* [in] */ LCID lcid,
            /* [size_is][out] */ __RPC__out_ecount_full(cNames) DISPID *rgDispId);
        
        /* [local] */ HRESULT ( STDMETHODCALLTYPE *Invoke )( 
            _PublishEvents * This,
            /* [in] */ DISPID dispIdMember,
            /* [in] */ REFIID riid,
            /* [in] */ LCID lcid,
            /* [in] */ WORD wFlags,
            /* [out][in] */ DISPPARAMS *pDispParams,
            /* [out] */ VARIANT *pVarResult,
            /* [out] */ EXCEPINFO *pExcepInfo,
            /* [out] */ UINT *puArgErr);
        
        END_INTERFACE
    } _PublishEventsVtbl;

    interface _PublishEvents
    {
        CONST_VTBL struct _PublishEventsVtbl *lpVtbl;
    };

    

#ifdef COBJMACROS


#define _PublishEvents_QueryInterface(This,riid,ppvObject)	\
    ( (This)->lpVtbl -> QueryInterface(This,riid,ppvObject) ) 

#define _PublishEvents_AddRef(This)	\
    ( (This)->lpVtbl -> AddRef(This) ) 

#define _PublishEvents_Release(This)	\
    ( (This)->lpVtbl -> Release(This) ) 


#define _PublishEvents_GetTypeInfoCount(This,pctinfo)	\
    ( (This)->lpVtbl -> GetTypeInfoCount(This,pctinfo) ) 

#define _PublishEvents_GetTypeInfo(This,iTInfo,lcid,ppTInfo)	\
    ( (This)->lpVtbl -> GetTypeInfo(This,iTInfo,lcid,ppTInfo) ) 

#define _PublishEvents_GetIDsOfNames(This,riid,rgszNames,cNames,lcid,rgDispId)	\
    ( (This)->lpVtbl -> GetIDsOfNames(This,riid,rgszNames,cNames,lcid,rgDispId) ) 

#define _PublishEvents_Invoke(This,dispIdMember,riid,lcid,wFlags,pDispParams,pVarResult,pExcepInfo,puArgErr)	\
    ( (This)->lpVtbl -> Invoke(This,dispIdMember,riid,lcid,wFlags,pDispParams,pVarResult,pExcepInfo,puArgErr) ) 


#endif /* COBJMACROS */


#endif 	/* C style interface */




#endif 	/* ___PublishEvents_INTERFACE_DEFINED__ */


#ifndef ___dispPublishEvents_DISPINTERFACE_DEFINED__
#define ___dispPublishEvents_DISPINTERFACE_DEFINED__

/* dispinterface _dispPublishEvents */
/* [uuid] */ 


EXTERN_C const IID DIID__dispPublishEvents;

#if defined(__cplusplus) && !defined(CINTERFACE)

    MIDL_INTERFACE("a0b40b93-9311-410f-b210-1f65bafb0e27")
    _dispPublishEvents : public IDispatch
    {
    };
    
#else 	/* C style interface */

    typedef struct _dispPublishEventsVtbl
    {
        BEGIN_INTERFACE
        
        HRESULT ( STDMETHODCALLTYPE *QueryInterface )( 
            _dispPublishEvents * This,
            /* [in] */ __RPC__in REFIID riid,
            /* [iid_is][out] */ 
            __RPC__deref_out  void **ppvObject);
        
        ULONG ( STDMETHODCALLTYPE *AddRef )( 
            _dispPublishEvents * This);
        
        ULONG ( STDMETHODCALLTYPE *Release )( 
            _dispPublishEvents * This);
        
        HRESULT ( STDMETHODCALLTYPE *GetTypeInfoCount )( 
            _dispPublishEvents * This,
            /* [out] */ __RPC__out UINT *pctinfo);
        
        HRESULT ( STDMETHODCALLTYPE *GetTypeInfo )( 
            _dispPublishEvents * This,
            /* [in] */ UINT iTInfo,
            /* [in] */ LCID lcid,
            /* [out] */ __RPC__deref_out_opt ITypeInfo **ppTInfo);
        
        HRESULT ( STDMETHODCALLTYPE *GetIDsOfNames )( 
            _dispPublishEvents * This,
            /* [in] */ __RPC__in REFIID riid,
            /* [size_is][in] */ __RPC__in_ecount_full(cNames) LPOLESTR *rgszNames,
            /* [range][in] */ UINT cNames,
            /* [in] */ LCID lcid,
            /* [size_is][out] */ __RPC__out_ecount_full(cNames) DISPID *rgDispId);
        
        /* [local] */ HRESULT ( STDMETHODCALLTYPE *Invoke )( 
            _dispPublishEvents * This,
            /* [in] */ DISPID dispIdMember,
            /* [in] */ REFIID riid,
            /* [in] */ LCID lcid,
            /* [in] */ WORD wFlags,
            /* [out][in] */ DISPPARAMS *pDispParams,
            /* [out] */ VARIANT *pVarResult,
            /* [out] */ EXCEPINFO *pExcepInfo,
            /* [out] */ UINT *puArgErr);
        
        END_INTERFACE
    } _dispPublishEventsVtbl;

    interface _dispPublishEvents
    {
        CONST_VTBL struct _dispPublishEventsVtbl *lpVtbl;
    };

    

#ifdef COBJMACROS


#define _dispPublishEvents_QueryInterface(This,riid,ppvObject)	\
    ( (This)->lpVtbl -> QueryInterface(This,riid,ppvObject) ) 

#define _dispPublishEvents_AddRef(This)	\
    ( (This)->lpVtbl -> AddRef(This) ) 

#define _dispPublishEvents_Release(This)	\
    ( (This)->lpVtbl -> Release(This) ) 


#define _dispPublishEvents_GetTypeInfoCount(This,pctinfo)	\
    ( (This)->lpVtbl -> GetTypeInfoCount(This,pctinfo) ) 

#define _dispPublishEvents_GetTypeInfo(This,iTInfo,lcid,ppTInfo)	\
    ( (This)->lpVtbl -> GetTypeInfo(This,iTInfo,lcid,ppTInfo) ) 

#define _dispPublishEvents_GetIDsOfNames(This,riid,rgszNames,cNames,lcid,rgDispId)	\
    ( (This)->lpVtbl -> GetIDsOfNames(This,riid,rgszNames,cNames,lcid,rgDispId) ) 

#define _dispPublishEvents_Invoke(This,dispIdMember,riid,lcid,wFlags,pDispParams,pVarResult,pExcepInfo,puArgErr)	\
    ( (This)->lpVtbl -> Invoke(This,dispIdMember,riid,lcid,wFlags,pDispParams,pVarResult,pExcepInfo,puArgErr) ) 

#endif /* COBJMACROS */


#endif 	/* C style interface */


#endif 	/* ___dispPublishEvents_DISPINTERFACE_DEFINED__ */


EXTERN_C const CLSID CLSID_PublishEvents;

#ifdef __cplusplus

class DECLSPEC_UUID("045448ea-e8c1-4122-ac7b-d96d8e5c6e5b")
PublishEvents;
#endif

#ifndef __Events2_INTERFACE_DEFINED__
#define __Events2_INTERFACE_DEFINED__

/* interface Events2 */
/* [helpstringcontext][helpstring][helpcontext][uuid][object][oleautomation][dual] */ 


EXTERN_C const IID IID_Events2;

#if defined(__cplusplus) && !defined(CINTERFACE)
    
    MIDL_INTERFACE("BED31E8C-F845-4397-AF13-6B82A6996C0D")
    Events2 : public Events
    {
    public:
        virtual /* [helpstringcontext][helpstring][helpcontext][propget][id] */ HRESULT STDMETHODCALLTYPE get_ProjectItemsEvents( 
            /* [retval][out] */ __RPC__deref_out_opt /* external definition not present */ ProjectItemsEvents **ppProjectItemsEvents) = 0;
        
        virtual /* [helpstringcontext][helpstring][helpcontext][propget][id] */ HRESULT STDMETHODCALLTYPE get_ProjectsEvents( 
            /* [retval][out] */ __RPC__deref_out_opt /* external definition not present */ ProjectsEvents **ppProjectsEvents) = 0;
        
        virtual /* [helpstringcontext][helpstring][helpcontext][propget][id] */ HRESULT STDMETHODCALLTYPE get_TextDocumentKeyPressEvents( 
            /* [defaultvalue][in] */ __RPC__in /* external definition not present */ TextDocument *TextDocument,
            /* [retval][out] */ __RPC__deref_out_opt TextDocumentKeyPressEvents	**ppEditorKeyPressEvents) = 0;
        
        virtual /* [helpstringcontext][helpstring][helpcontext][propget][id] */ HRESULT STDMETHODCALLTYPE get_CodeModelEvents( 
            /* [defaultvalue][in] */ __RPC__in /* external definition not present */ CodeElement *Reserved,
            /* [retval][out] */ __RPC__deref_out_opt CodeModelEvents	**ppdisp) = 0;
        
        virtual /* [helpstringcontext][helpstring][helpcontext][propget][id] */ HRESULT STDMETHODCALLTYPE get_WindowVisibilityEvents( 
            /* [defaultvalue][in] */ __RPC__in /* external definition not present */ Window *WindowFilter,
            /* [retval][out] */ __RPC__deref_out_opt WindowVisibilityEvents	**ppdisp) = 0;
        
        virtual /* [helpstringcontext][helpstring][helpcontext][propget][id] */ HRESULT STDMETHODCALLTYPE get_DebuggerProcessEvents( 
            /* [retval][out] */ __RPC__deref_out_opt DebuggerProcessEvents	**ppDebuggerProcessEvents) = 0;
        
        virtual /* [helpstringcontext][helpstring][helpcontext][propget][id] */ HRESULT STDMETHODCALLTYPE get_DebuggerExpressionEvaluationEvents( 
            /* [retval][out] */ __RPC__deref_out_opt DebuggerExpressionEvaluationEvents	**ppDebuggerExpressionEvalutionEvents) = 0;
        
        virtual /* [helpstringcontext][helpstring][helpcontext][propget][id] */ HRESULT STDMETHODCALLTYPE get_PublishEvents( 
            /* [retval][out] */ __RPC__deref_out_opt PublishEvents	**ppPublishEvents) = 0;
        
    };
    
#else 	/* C style interface */

    typedef struct Events2Vtbl
    {
        BEGIN_INTERFACE
        
        /* [id][restricted][funcdescattr] */ HRESULT ( STDMETHODCALLTYPE *QueryInterface )( 
            Events2 * This,
            /* [in][idldescattr] */ __RPC__in struct GUID *riid,
            /* [out][idldescattr] */ __RPC__deref_out_opt void **ppvObj,
            /* [retval][out] */ __RPC__out void *retval);
        
        /* [id][restricted][funcdescattr] */ HRESULT ( STDMETHODCALLTYPE *AddRef )( 
            Events2 * This,
            /* [retval][out] */ __RPC__out unsigned long *retval);
        
        /* [id][restricted][funcdescattr] */ HRESULT ( STDMETHODCALLTYPE *Release )( 
            Events2 * This,
            /* [retval][out] */ __RPC__out unsigned long *retval);
        
        /* [id][restricted][funcdescattr] */ HRESULT ( STDMETHODCALLTYPE *GetTypeInfoCount )( 
            Events2 * This,
            /* [out][idldescattr] */ __RPC__out unsigned UINT *pctinfo,
            /* [retval][out] */ __RPC__out void *retval);
        
        /* [id][restricted][funcdescattr] */ HRESULT ( STDMETHODCALLTYPE *GetTypeInfo )( 
            Events2 * This,
            /* [in][idldescattr] */ unsigned UINT itinfo,
            /* [in][idldescattr] */ unsigned long lcid,
            /* [out][idldescattr] */ __RPC__deref_out_opt void **pptinfo,
            /* [retval][out] */ __RPC__out void *retval);
        
        /* [id][restricted][funcdescattr] */ HRESULT ( STDMETHODCALLTYPE *GetIDsOfNames )( 
            Events2 * This,
            /* [in][idldescattr] */ __RPC__in struct GUID *riid,
            /* [in][idldescattr] */ __RPC__deref_in_opt signed char **rgszNames,
            /* [in][idldescattr] */ unsigned UINT cNames,
            /* [in][idldescattr] */ unsigned long lcid,
            /* [out][idldescattr] */ __RPC__out signed long *rgdispid,
            /* [retval][out] */ __RPC__out void *retval);
        
        /* [id][restricted][funcdescattr] */ HRESULT ( STDMETHODCALLTYPE *Invoke )( 
            Events2 * This,
            /* [in][idldescattr] */ signed long dispidMember,
            /* [in][idldescattr] */ __RPC__in struct GUID *riid,
            /* [in][idldescattr] */ unsigned long lcid,
            /* [in][idldescattr] */ unsigned short wFlags,
            /* [in][idldescattr] */ __RPC__in struct DISPPARAMS *pdispparams,
            /* [out][idldescattr] */ __RPC__out VARIANT *pvarResult,
            /* [out][idldescattr] */ __RPC__out struct EXCEPINFO *pexcepinfo,
            /* [out][idldescattr] */ __RPC__out unsigned UINT *puArgErr,
            /* [retval][out] */ __RPC__out void *retval);
        
        /* [id][propget][funcdescattr] */ HRESULT ( STDMETHODCALLTYPE *get_CommandBarEvents )( 
            Events2 * This,
            /* [in][idldescattr] */ __RPC__in_opt IDispatch *CommandBarControl,
            /* [retval][out] */ __RPC__deref_out_opt IDispatch **retval);
        
        /* [id][propget][funcdescattr] */ HRESULT ( STDMETHODCALLTYPE *get_CommandEvents )( 
            Events2 * This,
            /* [in][idldescattr] */ __RPC__in BSTR Guid,
            /* [in][idldescattr] */ signed long ID,
            /* [retval][out] */ __RPC__deref_out_opt **retval);
        
        /* [id][propget][funcdescattr] */ HRESULT ( STDMETHODCALLTYPE *get_SelectionEvents )( 
            Events2 * This,
            /* [retval][out] */ __RPC__deref_out_opt **retval);
        
        /* [id][propget][funcdescattr] */ HRESULT ( STDMETHODCALLTYPE *get_SolutionEvents )( 
            Events2 * This,
            /* [retval][out] */ __RPC__deref_out_opt **retval);
        
        /* [id][propget][funcdescattr] */ HRESULT ( STDMETHODCALLTYPE *get_BuildEvents )( 
            Events2 * This,
            /* [retval][out] */ __RPC__deref_out_opt **retval);
        
        /* [id][propget][funcdescattr] */ HRESULT ( STDMETHODCALLTYPE *get_WindowEvents )( 
            Events2 * This,
            /* [in][idldescattr] */ __RPC__in_opt Window *WindowFilter,
            /* [retval][out] */ __RPC__deref_out_opt **retval);
        
        /* [id][propget][funcdescattr] */ HRESULT ( STDMETHODCALLTYPE *get_OutputWindowEvents )( 
            Events2 * This,
            /* [in][idldescattr] */ __RPC__in BSTR Pane,
            /* [retval][out] */ __RPC__deref_out_opt **retval);
        
        /* [id][propget][funcdescattr] */ HRESULT ( STDMETHODCALLTYPE *get_FindEvents )( 
            Events2 * This,
            /* [retval][out] */ __RPC__deref_out_opt **retval);
        
        /* [id][propget][funcdescattr] */ HRESULT ( STDMETHODCALLTYPE *get_TaskListEvents )( 
            Events2 * This,
            /* [in][idldescattr] */ __RPC__in BSTR Filter,
            /* [retval][out] */ __RPC__deref_out_opt **retval);
        
        /* [id][propget][funcdescattr] */ HRESULT ( STDMETHODCALLTYPE *get_DTEEvents )( 
            Events2 * This,
            /* [retval][out] */ __RPC__deref_out_opt **retval);
        
        /* [id][propget][funcdescattr] */ HRESULT ( STDMETHODCALLTYPE *get_DocumentEvents )( 
            Events2 * This,
            /* [in][idldescattr] */ __RPC__in_opt Document *Document,
            /* [retval][out] */ __RPC__deref_out_opt **retval);
        
        /* [id][propget][funcdescattr] */ HRESULT ( STDMETHODCALLTYPE *get_SolutionItemsEvents )( 
            Events2 * This,
            /* [retval][out] */ __RPC__deref_out_opt **retval);
        
        /* [id][propget][funcdescattr] */ HRESULT ( STDMETHODCALLTYPE *get_MiscFilesEvents )( 
            Events2 * This,
            /* [retval][out] */ __RPC__deref_out_opt **retval);
        
        /* [id][propget][funcdescattr] */ HRESULT ( STDMETHODCALLTYPE *get_DebuggerEvents )( 
            Events2 * This,
            /* [retval][out] */ __RPC__deref_out_opt **retval);
        
        /* [id][propget][funcdescattr] */ HRESULT ( STDMETHODCALLTYPE *get_TextEditorEvents )( 
            Events2 * This,
            /* [in][idldescattr] */ __RPC__in_opt TextDocument *TextDocumentFilter,
            /* [retval][out] */ __RPC__deref_out_opt **retval);
        
        /* [id][funcdescattr] */ HRESULT ( STDMETHODCALLTYPE *GetObject )( 
            Events2 * This,
            /* [in][idldescattr] */ __RPC__in BSTR Name,
            /* [retval][out] */ __RPC__deref_out_opt IDispatch **retval);
        
        /* [helpstringcontext][helpstring][helpcontext][propget][id] */ HRESULT ( STDMETHODCALLTYPE *get_ProjectItemsEvents )( 
            Events2 * This,
            /* [retval][out] */ __RPC__deref_out_opt /* external definition not present */ ProjectItemsEvents **ppProjectItemsEvents);
        
        /* [helpstringcontext][helpstring][helpcontext][propget][id] */ HRESULT ( STDMETHODCALLTYPE *get_ProjectsEvents )( 
            Events2 * This,
            /* [retval][out] */ __RPC__deref_out_opt /* external definition not present */ ProjectsEvents **ppProjectsEvents);
        
        /* [helpstringcontext][helpstring][helpcontext][propget][id] */ HRESULT ( STDMETHODCALLTYPE *get_TextDocumentKeyPressEvents )( 
            Events2 * This,
            /* [defaultvalue][in] */ __RPC__in /* external definition not present */ TextDocument *TextDocument,
            /* [retval][out] */ __RPC__deref_out_opt TextDocumentKeyPressEvents	**ppEditorKeyPressEvents);
        
        /* [helpstringcontext][helpstring][helpcontext][propget][id] */ HRESULT ( STDMETHODCALLTYPE *get_CodeModelEvents )( 
            Events2 * This,
            /* [defaultvalue][in] */ __RPC__in /* external definition not present */ CodeElement *Reserved,
            /* [retval][out] */ __RPC__deref_out_opt CodeModelEvents	**ppdisp);
        
        /* [helpstringcontext][helpstring][helpcontext][propget][id] */ HRESULT ( STDMETHODCALLTYPE *get_WindowVisibilityEvents )( 
            Events2 * This,
            /* [defaultvalue][in] */ __RPC__in /* external definition not present */ Window *WindowFilter,
            /* [retval][out] */ __RPC__deref_out_opt WindowVisibilityEvents	**ppdisp);
        
        /* [helpstringcontext][helpstring][helpcontext][propget][id] */ HRESULT ( STDMETHODCALLTYPE *get_DebuggerProcessEvents )( 
            Events2 * This,
            /* [retval][out] */ __RPC__deref_out_opt DebuggerProcessEvents	**ppDebuggerProcessEvents);
        
        /* [helpstringcontext][helpstring][helpcontext][propget][id] */ HRESULT ( STDMETHODCALLTYPE *get_DebuggerExpressionEvaluationEvents )( 
            Events2 * This,
            /* [retval][out] */ __RPC__deref_out_opt DebuggerExpressionEvaluationEvents	**ppDebuggerExpressionEvalutionEvents);
        
        /* [helpstringcontext][helpstring][helpcontext][propget][id] */ HRESULT ( STDMETHODCALLTYPE *get_PublishEvents )( 
            Events2 * This,
            /* [retval][out] */ __RPC__deref_out_opt PublishEvents	**ppPublishEvents);
        
        END_INTERFACE
    } Events2Vtbl;

    interface Events2
    {
        CONST_VTBL struct Events2Vtbl *lpVtbl;
    };

    

#ifdef COBJMACROS


#define Events2_QueryInterface(This,riid,ppvObj,retval)	\
    ( (This)->lpVtbl -> QueryInterface(This,riid,ppvObj,retval) ) 

#define Events2_AddRef(This,retval)	\
    ( (This)->lpVtbl -> AddRef(This,retval) ) 

#define Events2_Release(This,retval)	\
    ( (This)->lpVtbl -> Release(This,retval) ) 

#define Events2_GetTypeInfoCount(This,pctinfo,retval)	\
    ( (This)->lpVtbl -> GetTypeInfoCount(This,pctinfo,retval) ) 

#define Events2_GetTypeInfo(This,itinfo,lcid,pptinfo,retval)	\
    ( (This)->lpVtbl -> GetTypeInfo(This,itinfo,lcid,pptinfo,retval) ) 

#define Events2_GetIDsOfNames(This,riid,rgszNames,cNames,lcid,rgdispid,retval)	\
    ( (This)->lpVtbl -> GetIDsOfNames(This,riid,rgszNames,cNames,lcid,rgdispid,retval) ) 

#define Events2_Invoke(This,dispidMember,riid,lcid,wFlags,pdispparams,pvarResult,pexcepinfo,puArgErr,retval)	\
    ( (This)->lpVtbl -> Invoke(This,dispidMember,riid,lcid,wFlags,pdispparams,pvarResult,pexcepinfo,puArgErr,retval) ) 

#define Events2_get_CommandBarEvents(This,CommandBarControl,retval)	\
    ( (This)->lpVtbl -> get_CommandBarEvents(This,CommandBarControl,retval) ) 

#define Events2_get_CommandEvents(This,Guid,ID,retval)	\
    ( (This)->lpVtbl -> get_CommandEvents(This,Guid,ID,retval) ) 

#define Events2_get_SelectionEvents(This,retval)	\
    ( (This)->lpVtbl -> get_SelectionEvents(This,retval) ) 

#define Events2_get_SolutionEvents(This,retval)	\
    ( (This)->lpVtbl -> get_SolutionEvents(This,retval) ) 

#define Events2_get_BuildEvents(This,retval)	\
    ( (This)->lpVtbl -> get_BuildEvents(This,retval) ) 

#define Events2_get_WindowEvents(This,WindowFilter,retval)	\
    ( (This)->lpVtbl -> get_WindowEvents(This,WindowFilter,retval) ) 

#define Events2_get_OutputWindowEvents(This,Pane,retval)	\
    ( (This)->lpVtbl -> get_OutputWindowEvents(This,Pane,retval) ) 

#define Events2_get_FindEvents(This,retval)	\
    ( (This)->lpVtbl -> get_FindEvents(This,retval) ) 

#define Events2_get_TaskListEvents(This,Filter,retval)	\
    ( (This)->lpVtbl -> get_TaskListEvents(This,Filter,retval) ) 

#define Events2_get_DTEEvents(This,retval)	\
    ( (This)->lpVtbl -> get_DTEEvents(This,retval) ) 

#define Events2_get_DocumentEvents(This,Document,retval)	\
    ( (This)->lpVtbl -> get_DocumentEvents(This,Document,retval) ) 

#define Events2_get_SolutionItemsEvents(This,retval)	\
    ( (This)->lpVtbl -> get_SolutionItemsEvents(This,retval) ) 

#define Events2_get_MiscFilesEvents(This,retval)	\
    ( (This)->lpVtbl -> get_MiscFilesEvents(This,retval) ) 

#define Events2_get_DebuggerEvents(This,retval)	\
    ( (This)->lpVtbl -> get_DebuggerEvents(This,retval) ) 

#define Events2_get_TextEditorEvents(This,TextDocumentFilter,retval)	\
    ( (This)->lpVtbl -> get_TextEditorEvents(This,TextDocumentFilter,retval) ) 

#define Events2_GetObject(This,Name,retval)	\
    ( (This)->lpVtbl -> GetObject(This,Name,retval) ) 


#define Events2_get_ProjectItemsEvents(This,ppProjectItemsEvents)	\
    ( (This)->lpVtbl -> get_ProjectItemsEvents(This,ppProjectItemsEvents) ) 

#define Events2_get_ProjectsEvents(This,ppProjectsEvents)	\
    ( (This)->lpVtbl -> get_ProjectsEvents(This,ppProjectsEvents) ) 

#define Events2_get_TextDocumentKeyPressEvents(This,TextDocument,ppEditorKeyPressEvents)	\
    ( (This)->lpVtbl -> get_TextDocumentKeyPressEvents(This,TextDocument,ppEditorKeyPressEvents) ) 

#define Events2_get_CodeModelEvents(This,Reserved,ppdisp)	\
    ( (This)->lpVtbl -> get_CodeModelEvents(This,Reserved,ppdisp) ) 

#define Events2_get_WindowVisibilityEvents(This,WindowFilter,ppdisp)	\
    ( (This)->lpVtbl -> get_WindowVisibilityEvents(This,WindowFilter,ppdisp) ) 

#define Events2_get_DebuggerProcessEvents(This,ppDebuggerProcessEvents)	\
    ( (This)->lpVtbl -> get_DebuggerProcessEvents(This,ppDebuggerProcessEvents) ) 

#define Events2_get_DebuggerExpressionEvaluationEvents(This,ppDebuggerExpressionEvalutionEvents)	\
    ( (This)->lpVtbl -> get_DebuggerExpressionEvaluationEvents(This,ppDebuggerExpressionEvalutionEvents) ) 

#define Events2_get_PublishEvents(This,ppPublishEvents)	\
    ( (This)->lpVtbl -> get_PublishEvents(This,ppPublishEvents) ) 

#endif /* COBJMACROS */


#endif 	/* C style interface */




#endif 	/* __Events2_INTERFACE_DEFINED__ */


#ifndef __Solution2_INTERFACE_DEFINED__
#define __Solution2_INTERFACE_DEFINED__

/* interface Solution2 */
/* [helpstringcontext][helpstring][helpcontext][uuid][object][oleautomation][dual] */ 


EXTERN_C const IID IID_Solution2;

#if defined(__cplusplus) && !defined(CINTERFACE)
    
    MIDL_INTERFACE("FA238614-FBB1-4314-A7F7-49AE8BB6C6BA")
    Solution2 : public _Solution
    {
    public:
        virtual /* [helpstringcontext][helpstring][helpcontext][id] */ HRESULT STDMETHODCALLTYPE AddSolutionFolder( 
            __RPC__in BSTR Name,
            /* [retval][out] */ __RPC__deref_out_opt /* external definition not present */ Project **pProject) = 0;
        
        virtual /* [helpstringcontext][helpstring][helpcontext][id] */ HRESULT STDMETHODCALLTYPE GetProjectTemplate( 
            __RPC__in BSTR TemplateName,
            __RPC__in BSTR Language,
            /* [retval][out] */ __RPC__deref_out_opt BSTR *pFullPath) = 0;
        
        virtual /* [helpstringcontext][helpstring][helpcontext][id] */ HRESULT STDMETHODCALLTYPE GetProjectItemTemplate( 
            __RPC__in BSTR TemplateName,
            __RPC__in BSTR Language,
            /* [retval][out] */ __RPC__deref_out_opt BSTR *pFullPath) = 0;
        
    };
    
#else 	/* C style interface */

    typedef struct Solution2Vtbl
    {
        BEGIN_INTERFACE
        
        /* [id][restricted][funcdescattr] */ HRESULT ( STDMETHODCALLTYPE *QueryInterface )( 
            Solution2 * This,
            /* [in][idldescattr] */ __RPC__in struct GUID *riid,
            /* [out][idldescattr] */ __RPC__deref_out_opt void **ppvObj,
            /* [retval][out] */ __RPC__out void *retval);
        
        /* [id][restricted][funcdescattr] */ HRESULT ( STDMETHODCALLTYPE *AddRef )( 
            Solution2 * This,
            /* [retval][out] */ __RPC__out unsigned long *retval);
        
        /* [id][restricted][funcdescattr] */ HRESULT ( STDMETHODCALLTYPE *Release )( 
            Solution2 * This,
            /* [retval][out] */ __RPC__out unsigned long *retval);
        
        /* [id][restricted][funcdescattr] */ HRESULT ( STDMETHODCALLTYPE *GetTypeInfoCount )( 
            Solution2 * This,
            /* [out][idldescattr] */ __RPC__out unsigned UINT *pctinfo,
            /* [retval][out] */ __RPC__out void *retval);
        
        /* [id][restricted][funcdescattr] */ HRESULT ( STDMETHODCALLTYPE *GetTypeInfo )( 
            Solution2 * This,
            /* [in][idldescattr] */ unsigned UINT itinfo,
            /* [in][idldescattr] */ unsigned long lcid,
            /* [out][idldescattr] */ __RPC__deref_out_opt void **pptinfo,
            /* [retval][out] */ __RPC__out void *retval);
        
        /* [id][restricted][funcdescattr] */ HRESULT ( STDMETHODCALLTYPE *GetIDsOfNames )( 
            Solution2 * This,
            /* [in][idldescattr] */ __RPC__in struct GUID *riid,
            /* [in][idldescattr] */ __RPC__deref_in_opt signed char **rgszNames,
            /* [in][idldescattr] */ unsigned UINT cNames,
            /* [in][idldescattr] */ unsigned long lcid,
            /* [out][idldescattr] */ __RPC__out signed long *rgdispid,
            /* [retval][out] */ __RPC__out void *retval);
        
        /* [id][restricted][funcdescattr] */ HRESULT ( STDMETHODCALLTYPE *Invoke )( 
            Solution2 * This,
            /* [in][idldescattr] */ signed long dispidMember,
            /* [in][idldescattr] */ __RPC__in struct GUID *riid,
            /* [in][idldescattr] */ unsigned long lcid,
            /* [in][idldescattr] */ unsigned short wFlags,
            /* [in][idldescattr] */ __RPC__in struct DISPPARAMS *pdispparams,
            /* [out][idldescattr] */ __RPC__out VARIANT *pvarResult,
            /* [out][idldescattr] */ __RPC__out struct EXCEPINFO *pexcepinfo,
            /* [out][idldescattr] */ __RPC__out unsigned UINT *puArgErr,
            /* [retval][out] */ __RPC__out void *retval);
        
        /* [id][funcdescattr] */ HRESULT ( STDMETHODCALLTYPE *Item )( 
            Solution2 * This,
            /* [in][idldescattr] */ VARIANT index,
            /* [retval][out] */ __RPC__deref_out_opt Project **retval);
        
        /* [id][restricted][funcdescattr] */ HRESULT ( STDMETHODCALLTYPE *_NewEnum )( 
            Solution2 * This,
            /* [retval][out] */ __RPC__deref_out_opt IUnknown **retval);
        
        /* [id][propget][funcdescattr] */ HRESULT ( STDMETHODCALLTYPE *get_DTE )( 
            Solution2 * This,
            /* [retval][out] */ __RPC__deref_out_opt **retval);
        
        /* [id][propget][funcdescattr] */ HRESULT ( STDMETHODCALLTYPE *get_Parent )( 
            Solution2 * This,
            /* [retval][out] */ __RPC__deref_out_opt **retval);
        
        /* [id][propget][funcdescattr] */ HRESULT ( STDMETHODCALLTYPE *get_Count )( 
            Solution2 * This,
            /* [retval][out] */ __RPC__out signed long *retval);
        
        /* [id][propget][hidden][funcdescattr] */ HRESULT ( STDMETHODCALLTYPE *get_FileName )( 
            Solution2 * This,
            /* [retval][out] */ __RPC__deref_out_opt BSTR *retval);
        
        /* [id][funcdescattr] */ HRESULT ( STDMETHODCALLTYPE *SaveAs )( 
            Solution2 * This,
            /* [in][idldescattr] */ __RPC__in BSTR FileName,
            /* [retval][out] */ __RPC__out void *retval);
        
        /* [id][funcdescattr] */ HRESULT ( STDMETHODCALLTYPE *AddFromTemplate )( 
            Solution2 * This,
            /* [in][idldescattr] */ __RPC__in BSTR FileName,
            /* [in][idldescattr] */ __RPC__in BSTR Destination,
            /* [in][idldescattr] */ __RPC__in BSTR ProjectName,
            /* [in][idldescattr] */ BOOLEAN Exclusive,
            /* [retval][out] */ __RPC__deref_out_opt Project **retval);
        
        /* [id][funcdescattr] */ HRESULT ( STDMETHODCALLTYPE *AddFromFile )( 
            Solution2 * This,
            /* [in][idldescattr] */ __RPC__in BSTR FileName,
            /* [in][idldescattr] */ BOOLEAN Exclusive,
            /* [retval][out] */ __RPC__deref_out_opt Project **retval);
        
        /* [id][funcdescattr] */ HRESULT ( STDMETHODCALLTYPE *Open )( 
            Solution2 * This,
            /* [in][idldescattr] */ __RPC__in BSTR FileName,
            /* [retval][out] */ __RPC__out void *retval);
        
        /* [id][funcdescattr] */ HRESULT ( STDMETHODCALLTYPE *Close )( 
            Solution2 * This,
            /* [in][idldescattr] */ BOOLEAN SaveFirst,
            /* [retval][out] */ __RPC__out void *retval);
        
        /* [id][propget][funcdescattr] */ HRESULT ( STDMETHODCALLTYPE *get_Properties )( 
            Solution2 * This,
            /* [retval][out] */ __RPC__deref_out_opt Properties **retval);
        
        /* [id][propget][hidden][funcdescattr] */ HRESULT ( STDMETHODCALLTYPE *get_IsDirty )( 
            Solution2 * This,
            /* [retval][out] */ __RPC__out BOOLEAN *retval);
        
        /* [id][propput][hidden][funcdescattr] */ HRESULT ( STDMETHODCALLTYPE *put_IsDirty )( 
            Solution2 * This,
            /* [in][idldescattr] */ BOOLEAN noname,
            /* [retval][out] */ __RPC__out void *retval);
        
        /* [id][funcdescattr] */ HRESULT ( STDMETHODCALLTYPE *Remove )( 
            Solution2 * This,
            /* [in][idldescattr] */ __RPC__in_opt Project *proj,
            /* [retval][out] */ __RPC__out void *retval);
        
        /* [id][propget][funcdescattr] */ HRESULT ( STDMETHODCALLTYPE *get_TemplatePath )( 
            Solution2 * This,
            /* [in][idldescattr] */ __RPC__in BSTR ProjectType,
            /* [retval][out] */ __RPC__deref_out_opt BSTR *retval);
        
        /* [id][propget][funcdescattr] */ HRESULT ( STDMETHODCALLTYPE *get_FullName )( 
            Solution2 * This,
            /* [retval][out] */ __RPC__deref_out_opt BSTR *retval);
        
        /* [id][propget][funcdescattr] */ HRESULT ( STDMETHODCALLTYPE *get_Saved )( 
            Solution2 * This,
            /* [retval][out] */ __RPC__out BOOLEAN *retval);
        
        /* [id][propput][funcdescattr] */ HRESULT ( STDMETHODCALLTYPE *put_Saved )( 
            Solution2 * This,
            /* [in][idldescattr] */ BOOLEAN noname,
            /* [retval][out] */ __RPC__out void *retval);
        
        /* [id][propget][funcdescattr] */ HRESULT ( STDMETHODCALLTYPE *get_Globals )( 
            Solution2 * This,
            /* [retval][out] */ __RPC__deref_out_opt Globals **retval);
        
        /* [id][propget][funcdescattr] */ HRESULT ( STDMETHODCALLTYPE *get_AddIns )( 
            Solution2 * This,
            /* [retval][out] */ __RPC__deref_out_opt AddIns **retval);
        
        /* [id][propget][funcdescattr] */ HRESULT ( STDMETHODCALLTYPE *get_Extender )( 
            Solution2 * This,
            /* [in][idldescattr] */ __RPC__in BSTR ExtenderName,
            /* [retval][out] */ __RPC__deref_out_opt IDispatch **retval);
        
        /* [id][propget][funcdescattr] */ HRESULT ( STDMETHODCALLTYPE *get_ExtenderNames )( 
            Solution2 * This,
            /* [retval][out] */ __RPC__out VARIANT *retval);
        
        /* [id][propget][funcdescattr] */ HRESULT ( STDMETHODCALLTYPE *get_ExtenderCATID )( 
            Solution2 * This,
            /* [retval][out] */ __RPC__deref_out_opt BSTR *retval);
        
        /* [id][propget][funcdescattr] */ HRESULT ( STDMETHODCALLTYPE *get_IsOpen )( 
            Solution2 * This,
            /* [retval][out] */ __RPC__out BOOLEAN *retval);
        
        /* [id][propget][funcdescattr] */ HRESULT ( STDMETHODCALLTYPE *get_SolutionBuild )( 
            Solution2 * This,
            /* [retval][out] */ __RPC__deref_out_opt SolutionBuild **retval);
        
        /* [id][funcdescattr] */ HRESULT ( STDMETHODCALLTYPE *Create )( 
            Solution2 * This,
            /* [idldescattr] */ __RPC__in BSTR Destination,
            /* [idldescattr] */ __RPC__in BSTR Name,
            /* [retval][out] */ __RPC__out void *retval);
        
        /* [id][propget][funcdescattr] */ HRESULT ( STDMETHODCALLTYPE *get_Projects )( 
            Solution2 * This,
            /* [retval][out] */ __RPC__deref_out_opt Projects **retval);
        
        /* [id][funcdescattr] */ HRESULT ( STDMETHODCALLTYPE *FindProjectItem )( 
            Solution2 * This,
            /* [idldescattr] */ __RPC__in BSTR FileName,
            /* [retval][out] */ __RPC__deref_out_opt ProjectItem **retval);
        
        /* [id][funcdescattr] */ HRESULT ( STDMETHODCALLTYPE *ProjectItemsTemplatePath )( 
            Solution2 * This,
            /* [idldescattr] */ __RPC__in BSTR ProjectKind,
            /* [retval][out] */ __RPC__deref_out_opt BSTR *retval);
        
        /* [helpstringcontext][helpstring][helpcontext][id] */ HRESULT ( STDMETHODCALLTYPE *AddSolutionFolder )( 
            Solution2 * This,
            __RPC__in BSTR Name,
            /* [retval][out] */ __RPC__deref_out_opt /* external definition not present */ Project **pProject);
        
        /* [helpstringcontext][helpstring][helpcontext][id] */ HRESULT ( STDMETHODCALLTYPE *GetProjectTemplate )( 
            Solution2 * This,
            __RPC__in BSTR TemplateName,
            __RPC__in BSTR Language,
            /* [retval][out] */ __RPC__deref_out_opt BSTR *pFullPath);
        
        /* [helpstringcontext][helpstring][helpcontext][id] */ HRESULT ( STDMETHODCALLTYPE *GetProjectItemTemplate )( 
            Solution2 * This,
            __RPC__in BSTR TemplateName,
            __RPC__in BSTR Language,
            /* [retval][out] */ __RPC__deref_out_opt BSTR *pFullPath);
        
        END_INTERFACE
    } Solution2Vtbl;

    interface Solution2
    {
        CONST_VTBL struct Solution2Vtbl *lpVtbl;
    };

    

#ifdef COBJMACROS


#define Solution2_QueryInterface(This,riid,ppvObj,retval)	\
    ( (This)->lpVtbl -> QueryInterface(This,riid,ppvObj,retval) ) 

#define Solution2_AddRef(This,retval)	\
    ( (This)->lpVtbl -> AddRef(This,retval) ) 

#define Solution2_Release(This,retval)	\
    ( (This)->lpVtbl -> Release(This,retval) ) 

#define Solution2_GetTypeInfoCount(This,pctinfo,retval)	\
    ( (This)->lpVtbl -> GetTypeInfoCount(This,pctinfo,retval) ) 

#define Solution2_GetTypeInfo(This,itinfo,lcid,pptinfo,retval)	\
    ( (This)->lpVtbl -> GetTypeInfo(This,itinfo,lcid,pptinfo,retval) ) 

#define Solution2_GetIDsOfNames(This,riid,rgszNames,cNames,lcid,rgdispid,retval)	\
    ( (This)->lpVtbl -> GetIDsOfNames(This,riid,rgszNames,cNames,lcid,rgdispid,retval) ) 

#define Solution2_Invoke(This,dispidMember,riid,lcid,wFlags,pdispparams,pvarResult,pexcepinfo,puArgErr,retval)	\
    ( (This)->lpVtbl -> Invoke(This,dispidMember,riid,lcid,wFlags,pdispparams,pvarResult,pexcepinfo,puArgErr,retval) ) 

#define Solution2_Item(This,index,retval)	\
    ( (This)->lpVtbl -> Item(This,index,retval) ) 

#define Solution2__NewEnum(This,retval)	\
    ( (This)->lpVtbl -> _NewEnum(This,retval) ) 

#define Solution2_get_DTE(This,retval)	\
    ( (This)->lpVtbl -> get_DTE(This,retval) ) 

#define Solution2_get_Parent(This,retval)	\
    ( (This)->lpVtbl -> get_Parent(This,retval) ) 

#define Solution2_get_Count(This,retval)	\
    ( (This)->lpVtbl -> get_Count(This,retval) ) 

#define Solution2_get_FileName(This,retval)	\
    ( (This)->lpVtbl -> get_FileName(This,retval) ) 

#define Solution2_SaveAs(This,FileName,retval)	\
    ( (This)->lpVtbl -> SaveAs(This,FileName,retval) ) 

#define Solution2_AddFromTemplate(This,FileName,Destination,ProjectName,Exclusive,retval)	\
    ( (This)->lpVtbl -> AddFromTemplate(This,FileName,Destination,ProjectName,Exclusive,retval) ) 

#define Solution2_AddFromFile(This,FileName,Exclusive,retval)	\
    ( (This)->lpVtbl -> AddFromFile(This,FileName,Exclusive,retval) ) 

#define Solution2_Open(This,FileName,retval)	\
    ( (This)->lpVtbl -> Open(This,FileName,retval) ) 

#define Solution2_Close(This,SaveFirst,retval)	\
    ( (This)->lpVtbl -> Close(This,SaveFirst,retval) ) 

#define Solution2_get_Properties(This,retval)	\
    ( (This)->lpVtbl -> get_Properties(This,retval) ) 

#define Solution2_get_IsDirty(This,retval)	\
    ( (This)->lpVtbl -> get_IsDirty(This,retval) ) 

#define Solution2_put_IsDirty(This,noname,retval)	\
    ( (This)->lpVtbl -> put_IsDirty(This,noname,retval) ) 

#define Solution2_Remove(This,proj,retval)	\
    ( (This)->lpVtbl -> Remove(This,proj,retval) ) 

#define Solution2_get_TemplatePath(This,ProjectType,retval)	\
    ( (This)->lpVtbl -> get_TemplatePath(This,ProjectType,retval) ) 

#define Solution2_get_FullName(This,retval)	\
    ( (This)->lpVtbl -> get_FullName(This,retval) ) 

#define Solution2_get_Saved(This,retval)	\
    ( (This)->lpVtbl -> get_Saved(This,retval) ) 

#define Solution2_put_Saved(This,noname,retval)	\
    ( (This)->lpVtbl -> put_Saved(This,noname,retval) ) 

#define Solution2_get_Globals(This,retval)	\
    ( (This)->lpVtbl -> get_Globals(This,retval) ) 

#define Solution2_get_AddIns(This,retval)	\
    ( (This)->lpVtbl -> get_AddIns(This,retval) ) 

#define Solution2_get_Extender(This,ExtenderName,retval)	\
    ( (This)->lpVtbl -> get_Extender(This,ExtenderName,retval) ) 

#define Solution2_get_ExtenderNames(This,retval)	\
    ( (This)->lpVtbl -> get_ExtenderNames(This,retval) ) 

#define Solution2_get_ExtenderCATID(This,retval)	\
    ( (This)->lpVtbl -> get_ExtenderCATID(This,retval) ) 

#define Solution2_get_IsOpen(This,retval)	\
    ( (This)->lpVtbl -> get_IsOpen(This,retval) ) 

#define Solution2_get_SolutionBuild(This,retval)	\
    ( (This)->lpVtbl -> get_SolutionBuild(This,retval) ) 

#define Solution2_Create(This,Destination,Name,retval)	\
    ( (This)->lpVtbl -> Create(This,Destination,Name,retval) ) 

#define Solution2_get_Projects(This,retval)	\
    ( (This)->lpVtbl -> get_Projects(This,retval) ) 

#define Solution2_FindProjectItem(This,FileName,retval)	\
    ( (This)->lpVtbl -> FindProjectItem(This,FileName,retval) ) 

#define Solution2_ProjectItemsTemplatePath(This,ProjectKind,retval)	\
    ( (This)->lpVtbl -> ProjectItemsTemplatePath(This,ProjectKind,retval) ) 


#define Solution2_AddSolutionFolder(This,Name,pProject)	\
    ( (This)->lpVtbl -> AddSolutionFolder(This,Name,pProject) ) 

#define Solution2_GetProjectTemplate(This,TemplateName,Language,pFullPath)	\
    ( (This)->lpVtbl -> GetProjectTemplate(This,TemplateName,Language,pFullPath) ) 

#define Solution2_GetProjectItemTemplate(This,TemplateName,Language,pFullPath)	\
    ( (This)->lpVtbl -> GetProjectItemTemplate(This,TemplateName,Language,pFullPath) ) 

#endif /* COBJMACROS */


#endif 	/* C style interface */




#endif 	/* __Solution2_INTERFACE_DEFINED__ */


#ifndef __SolutionFolder_INTERFACE_DEFINED__
#define __SolutionFolder_INTERFACE_DEFINED__

/* interface SolutionFolder */
/* [helpstringcontext][helpstring][helpcontext][uuid][object][oleautomation][dual] */ 


EXTERN_C const IID IID_SolutionFolder;

#if defined(__cplusplus) && !defined(CINTERFACE)
    
    MIDL_INTERFACE("F8F69788-267C-4408-8967-74F26108C438")
    SolutionFolder : public IDispatch
    {
    public:
        virtual /* [helpstringcontext][helpstring][helpcontext][id][propget] */ HRESULT STDMETHODCALLTYPE get_DTE( 
            /* [retval][out] */ __RPC__deref_out_opt /* external definition not present */ DTE **pDTE) = 0;
        
        virtual /* [helpstringcontext][helpstring][helpcontext][id][propget] */ HRESULT STDMETHODCALLTYPE get_Parent( 
            /* [retval][out] */ __RPC__deref_out_opt /* external definition not present */ Project **ppProject) = 0;
        
        virtual /* [helpstringcontext][helpstring][helpcontext][id] */ HRESULT STDMETHODCALLTYPE AddSolutionFolder( 
            __RPC__in BSTR Name,
            /* [retval][out] */ __RPC__deref_out_opt /* external definition not present */ Project **pProject) = 0;
        
        virtual /* [helpstringcontext][helpstring][helpcontext][id] */ HRESULT STDMETHODCALLTYPE AddFromFile( 
            /* [in] */ __RPC__in BSTR FileName,
            /* [retval][out] */ __RPC__deref_out_opt /* external definition not present */ Project **ppProject) = 0;
        
        virtual /* [helpstringcontext][helpstring][helpcontext][id] */ HRESULT STDMETHODCALLTYPE AddFromTemplate( 
            /* [in] */ __RPC__in BSTR FileName,
            /* [in] */ __RPC__in BSTR Destination,
            /* [in] */ __RPC__in BSTR ProjectName,
            /* [retval][out] */ __RPC__deref_out_opt /* external definition not present */ Project **ppProject) = 0;
        
        virtual /* [helpstringcontext][helpstring][helpcontext][id][propget] */ HRESULT STDMETHODCALLTYPE get_Hidden( 
            /* [retval][out] */ __RPC__out VARIANT_BOOL *pHidden) = 0;
        
        virtual /* [helpstringcontext][helpstring][helpcontext][id][propput] */ HRESULT STDMETHODCALLTYPE put_Hidden( 
            VARIANT_BOOL hidden) = 0;
        
    };
    
#else 	/* C style interface */

    typedef struct SolutionFolderVtbl
    {
        BEGIN_INTERFACE
        
        HRESULT ( STDMETHODCALLTYPE *QueryInterface )( 
            SolutionFolder * This,
            /* [in] */ __RPC__in REFIID riid,
            /* [iid_is][out] */ 
            __RPC__deref_out  void **ppvObject);
        
        ULONG ( STDMETHODCALLTYPE *AddRef )( 
            SolutionFolder * This);
        
        ULONG ( STDMETHODCALLTYPE *Release )( 
            SolutionFolder * This);
        
        HRESULT ( STDMETHODCALLTYPE *GetTypeInfoCount )( 
            SolutionFolder * This,
            /* [out] */ __RPC__out UINT *pctinfo);
        
        HRESULT ( STDMETHODCALLTYPE *GetTypeInfo )( 
            SolutionFolder * This,
            /* [in] */ UINT iTInfo,
            /* [in] */ LCID lcid,
            /* [out] */ __RPC__deref_out_opt ITypeInfo **ppTInfo);
        
        HRESULT ( STDMETHODCALLTYPE *GetIDsOfNames )( 
            SolutionFolder * This,
            /* [in] */ __RPC__in REFIID riid,
            /* [size_is][in] */ __RPC__in_ecount_full(cNames) LPOLESTR *rgszNames,
            /* [range][in] */ UINT cNames,
            /* [in] */ LCID lcid,
            /* [size_is][out] */ __RPC__out_ecount_full(cNames) DISPID *rgDispId);
        
        /* [local] */ HRESULT ( STDMETHODCALLTYPE *Invoke )( 
            SolutionFolder * This,
            /* [in] */ DISPID dispIdMember,
            /* [in] */ REFIID riid,
            /* [in] */ LCID lcid,
            /* [in] */ WORD wFlags,
            /* [out][in] */ DISPPARAMS *pDispParams,
            /* [out] */ VARIANT *pVarResult,
            /* [out] */ EXCEPINFO *pExcepInfo,
            /* [out] */ UINT *puArgErr);
        
        /* [helpstringcontext][helpstring][helpcontext][id][propget] */ HRESULT ( STDMETHODCALLTYPE *get_DTE )( 
            SolutionFolder * This,
            /* [retval][out] */ __RPC__deref_out_opt /* external definition not present */ DTE **pDTE);
        
        /* [helpstringcontext][helpstring][helpcontext][id][propget] */ HRESULT ( STDMETHODCALLTYPE *get_Parent )( 
            SolutionFolder * This,
            /* [retval][out] */ __RPC__deref_out_opt /* external definition not present */ Project **ppProject);
        
        /* [helpstringcontext][helpstring][helpcontext][id] */ HRESULT ( STDMETHODCALLTYPE *AddSolutionFolder )( 
            SolutionFolder * This,
            __RPC__in BSTR Name,
            /* [retval][out] */ __RPC__deref_out_opt /* external definition not present */ Project **pProject);
        
        /* [helpstringcontext][helpstring][helpcontext][id] */ HRESULT ( STDMETHODCALLTYPE *AddFromFile )( 
            SolutionFolder * This,
            /* [in] */ __RPC__in BSTR FileName,
            /* [retval][out] */ __RPC__deref_out_opt /* external definition not present */ Project **ppProject);
        
        /* [helpstringcontext][helpstring][helpcontext][id] */ HRESULT ( STDMETHODCALLTYPE *AddFromTemplate )( 
            SolutionFolder * This,
            /* [in] */ __RPC__in BSTR FileName,
            /* [in] */ __RPC__in BSTR Destination,
            /* [in] */ __RPC__in BSTR ProjectName,
            /* [retval][out] */ __RPC__deref_out_opt /* external definition not present */ Project **ppProject);
        
        /* [helpstringcontext][helpstring][helpcontext][id][propget] */ HRESULT ( STDMETHODCALLTYPE *get_Hidden )( 
            SolutionFolder * This,
            /* [retval][out] */ __RPC__out VARIANT_BOOL *pHidden);
        
        /* [helpstringcontext][helpstring][helpcontext][id][propput] */ HRESULT ( STDMETHODCALLTYPE *put_Hidden )( 
            SolutionFolder * This,
            VARIANT_BOOL hidden);
        
        END_INTERFACE
    } SolutionFolderVtbl;

    interface SolutionFolder
    {
        CONST_VTBL struct SolutionFolderVtbl *lpVtbl;
    };

    

#ifdef COBJMACROS


#define SolutionFolder_QueryInterface(This,riid,ppvObject)	\
    ( (This)->lpVtbl -> QueryInterface(This,riid,ppvObject) ) 

#define SolutionFolder_AddRef(This)	\
    ( (This)->lpVtbl -> AddRef(This) ) 

#define SolutionFolder_Release(This)	\
    ( (This)->lpVtbl -> Release(This) ) 


#define SolutionFolder_GetTypeInfoCount(This,pctinfo)	\
    ( (This)->lpVtbl -> GetTypeInfoCount(This,pctinfo) ) 

#define SolutionFolder_GetTypeInfo(This,iTInfo,lcid,ppTInfo)	\
    ( (This)->lpVtbl -> GetTypeInfo(This,iTInfo,lcid,ppTInfo) ) 

#define SolutionFolder_GetIDsOfNames(This,riid,rgszNames,cNames,lcid,rgDispId)	\
    ( (This)->lpVtbl -> GetIDsOfNames(This,riid,rgszNames,cNames,lcid,rgDispId) ) 

#define SolutionFolder_Invoke(This,dispIdMember,riid,lcid,wFlags,pDispParams,pVarResult,pExcepInfo,puArgErr)	\
    ( (This)->lpVtbl -> Invoke(This,dispIdMember,riid,lcid,wFlags,pDispParams,pVarResult,pExcepInfo,puArgErr) ) 


#define SolutionFolder_get_DTE(This,pDTE)	\
    ( (This)->lpVtbl -> get_DTE(This,pDTE) ) 

#define SolutionFolder_get_Parent(This,ppProject)	\
    ( (This)->lpVtbl -> get_Parent(This,ppProject) ) 

#define SolutionFolder_AddSolutionFolder(This,Name,pProject)	\
    ( (This)->lpVtbl -> AddSolutionFolder(This,Name,pProject) ) 

#define SolutionFolder_AddFromFile(This,FileName,ppProject)	\
    ( (This)->lpVtbl -> AddFromFile(This,FileName,ppProject) ) 

#define SolutionFolder_AddFromTemplate(This,FileName,Destination,ProjectName,ppProject)	\
    ( (This)->lpVtbl -> AddFromTemplate(This,FileName,Destination,ProjectName,ppProject) ) 

#define SolutionFolder_get_Hidden(This,pHidden)	\
    ( (This)->lpVtbl -> get_Hidden(This,pHidden) ) 

#define SolutionFolder_put_Hidden(This,hidden)	\
    ( (This)->lpVtbl -> put_Hidden(This,hidden) ) 

#endif /* COBJMACROS */


#endif 	/* C style interface */




#endif 	/* __SolutionFolder_INTERFACE_DEFINED__ */


#ifndef __TaskItems2_INTERFACE_DEFINED__
#define __TaskItems2_INTERFACE_DEFINED__

/* interface TaskItems2 */
/* [helpstringcontext][helpstring][helpcontext][uuid][object][oleautomation][dual] */ 


EXTERN_C const IID IID_TaskItems2;

#if defined(__cplusplus) && !defined(CINTERFACE)
    
    MIDL_INTERFACE("B820F931-645A-473F-8246-922CF069E1FE")
    TaskItems2 : public TaskItems
    {
    public:
        virtual /* [helpstringcontext][helpstring][helpcontext][id] */ HRESULT STDMETHODCALLTYPE Add2( 
            /* [in] */ __RPC__in BSTR Category,
            /* [in] */ __RPC__in BSTR SubCategory,
            /* [in] */ __RPC__in BSTR Description,
            /* [defaultvalue][in] */ long Priority,
            /* [optional][in] */ VARIANT Icon,
            /* [defaultvalue][in] */ VARIANT_BOOL Checkable,
            /* [defaultvalue][in] */ __RPC__in BSTR File,
            /* [defaultvalue][in] */ long Line,
            /* [defaultvalue][in] */ VARIANT_BOOL CanUserDelete,
            /* [defaultvalue][in] */ VARIANT_BOOL FlushItem,
            /* [defaultvalue][in] */ VARIANT_BOOL AutoNavigate,
            /* [retval][out] */ __RPC__deref_out_opt /* external definition not present */ TaskItem **pTaskItem) = 0;
        
    };
    
#else 	/* C style interface */

    typedef struct TaskItems2Vtbl
    {
        BEGIN_INTERFACE
        
        /* [id][restricted][funcdescattr] */ HRESULT ( STDMETHODCALLTYPE *QueryInterface )( 
            TaskItems2 * This,
            /* [in][idldescattr] */ __RPC__in struct GUID *riid,
            /* [out][idldescattr] */ __RPC__deref_out_opt void **ppvObj,
            /* [retval][out] */ __RPC__out void *retval);
        
        /* [id][restricted][funcdescattr] */ HRESULT ( STDMETHODCALLTYPE *AddRef )( 
            TaskItems2 * This,
            /* [retval][out] */ __RPC__out unsigned long *retval);
        
        /* [id][restricted][funcdescattr] */ HRESULT ( STDMETHODCALLTYPE *Release )( 
            TaskItems2 * This,
            /* [retval][out] */ __RPC__out unsigned long *retval);
        
        /* [id][restricted][funcdescattr] */ HRESULT ( STDMETHODCALLTYPE *GetTypeInfoCount )( 
            TaskItems2 * This,
            /* [out][idldescattr] */ __RPC__out unsigned UINT *pctinfo,
            /* [retval][out] */ __RPC__out void *retval);
        
        /* [id][restricted][funcdescattr] */ HRESULT ( STDMETHODCALLTYPE *GetTypeInfo )( 
            TaskItems2 * This,
            /* [in][idldescattr] */ unsigned UINT itinfo,
            /* [in][idldescattr] */ unsigned long lcid,
            /* [out][idldescattr] */ __RPC__deref_out_opt void **pptinfo,
            /* [retval][out] */ __RPC__out void *retval);
        
        /* [id][restricted][funcdescattr] */ HRESULT ( STDMETHODCALLTYPE *GetIDsOfNames )( 
            TaskItems2 * This,
            /* [in][idldescattr] */ __RPC__in struct GUID *riid,
            /* [in][idldescattr] */ __RPC__deref_in_opt signed char **rgszNames,
            /* [in][idldescattr] */ unsigned UINT cNames,
            /* [in][idldescattr] */ unsigned long lcid,
            /* [out][idldescattr] */ __RPC__out signed long *rgdispid,
            /* [retval][out] */ __RPC__out void *retval);
        
        /* [id][restricted][funcdescattr] */ HRESULT ( STDMETHODCALLTYPE *Invoke )( 
            TaskItems2 * This,
            /* [in][idldescattr] */ signed long dispidMember,
            /* [in][idldescattr] */ __RPC__in struct GUID *riid,
            /* [in][idldescattr] */ unsigned long lcid,
            /* [in][idldescattr] */ unsigned short wFlags,
            /* [in][idldescattr] */ __RPC__in struct DISPPARAMS *pdispparams,
            /* [out][idldescattr] */ __RPC__out VARIANT *pvarResult,
            /* [out][idldescattr] */ __RPC__out struct EXCEPINFO *pexcepinfo,
            /* [out][idldescattr] */ __RPC__out unsigned UINT *puArgErr,
            /* [retval][out] */ __RPC__out void *retval);
        
        /* [id][propget][funcdescattr] */ HRESULT ( STDMETHODCALLTYPE *get_DTE )( 
            TaskItems2 * This,
            /* [retval][out] */ __RPC__deref_out_opt **retval);
        
        /* [id][propget][funcdescattr] */ HRESULT ( STDMETHODCALLTYPE *get_Parent )( 
            TaskItems2 * This,
            /* [retval][out] */ __RPC__deref_out_opt TaskList **retval);
        
        /* [id][funcdescattr] */ HRESULT ( STDMETHODCALLTYPE *Item )( 
            TaskItems2 * This,
            /* [in][idldescattr] */ VARIANT index,
            /* [retval][out] */ __RPC__deref_out_opt TaskItem **retval);
        
        /* [id][propget][funcdescattr] */ HRESULT ( STDMETHODCALLTYPE *get_Count )( 
            TaskItems2 * This,
            /* [retval][out] */ __RPC__out signed long *retval);
        
        /* [id][funcdescattr] */ HRESULT ( STDMETHODCALLTYPE *Add )( 
            TaskItems2 * This,
            /* [in][idldescattr] */ __RPC__in BSTR Category,
            /* [in][idldescattr] */ __RPC__in BSTR SubCategory,
            /* [in][idldescattr] */ __RPC__in BSTR Description,
            /* [in][idldescattr] */ enum vsTaskPriority Priority,
            /* [in][idldescattr] */ VARIANT Icon,
            /* [in][idldescattr] */ BOOLEAN Checkable,
            /* [in][idldescattr] */ __RPC__in BSTR File,
            /* [in][idldescattr] */ signed long Line,
            /* [in][idldescattr] */ BOOLEAN CanUserDelete,
            /* [in][idldescattr] */ BOOLEAN FlushItem,
            /* [retval][out] */ __RPC__deref_out_opt TaskItem **retval);
        
        /* [id][restricted][funcdescattr] */ HRESULT ( STDMETHODCALLTYPE *_NewEnum )( 
            TaskItems2 * This,
            /* [retval][out] */ __RPC__deref_out_opt IUnknown **retval);
        
        /* [id][funcdescattr] */ HRESULT ( STDMETHODCALLTYPE *ForceItemsToTaskList )( 
            TaskItems2 * This,
            /* [retval][out] */ __RPC__out void *retval);
        
        /* [helpstringcontext][helpstring][helpcontext][id] */ HRESULT ( STDMETHODCALLTYPE *Add2 )( 
            TaskItems2 * This,
            /* [in] */ __RPC__in BSTR Category,
            /* [in] */ __RPC__in BSTR SubCategory,
            /* [in] */ __RPC__in BSTR Description,
            /* [defaultvalue][in] */ long Priority,
            /* [optional][in] */ VARIANT Icon,
            /* [defaultvalue][in] */ VARIANT_BOOL Checkable,
            /* [defaultvalue][in] */ __RPC__in BSTR File,
            /* [defaultvalue][in] */ long Line,
            /* [defaultvalue][in] */ VARIANT_BOOL CanUserDelete,
            /* [defaultvalue][in] */ VARIANT_BOOL FlushItem,
            /* [defaultvalue][in] */ VARIANT_BOOL AutoNavigate,
            /* [retval][out] */ __RPC__deref_out_opt /* external definition not present */ TaskItem **pTaskItem);
        
        END_INTERFACE
    } TaskItems2Vtbl;

    interface TaskItems2
    {
        CONST_VTBL struct TaskItems2Vtbl *lpVtbl;
    };

    

#ifdef COBJMACROS


#define TaskItems2_QueryInterface(This,riid,ppvObj,retval)	\
    ( (This)->lpVtbl -> QueryInterface(This,riid,ppvObj,retval) ) 

#define TaskItems2_AddRef(This,retval)	\
    ( (This)->lpVtbl -> AddRef(This,retval) ) 

#define TaskItems2_Release(This,retval)	\
    ( (This)->lpVtbl -> Release(This,retval) ) 

#define TaskItems2_GetTypeInfoCount(This,pctinfo,retval)	\
    ( (This)->lpVtbl -> GetTypeInfoCount(This,pctinfo,retval) ) 

#define TaskItems2_GetTypeInfo(This,itinfo,lcid,pptinfo,retval)	\
    ( (This)->lpVtbl -> GetTypeInfo(This,itinfo,lcid,pptinfo,retval) ) 

#define TaskItems2_GetIDsOfNames(This,riid,rgszNames,cNames,lcid,rgdispid,retval)	\
    ( (This)->lpVtbl -> GetIDsOfNames(This,riid,rgszNames,cNames,lcid,rgdispid,retval) ) 

#define TaskItems2_Invoke(This,dispidMember,riid,lcid,wFlags,pdispparams,pvarResult,pexcepinfo,puArgErr,retval)	\
    ( (This)->lpVtbl -> Invoke(This,dispidMember,riid,lcid,wFlags,pdispparams,pvarResult,pexcepinfo,puArgErr,retval) ) 

#define TaskItems2_get_DTE(This,retval)	\
    ( (This)->lpVtbl -> get_DTE(This,retval) ) 

#define TaskItems2_get_Parent(This,retval)	\
    ( (This)->lpVtbl -> get_Parent(This,retval) ) 

#define TaskItems2_Item(This,index,retval)	\
    ( (This)->lpVtbl -> Item(This,index,retval) ) 

#define TaskItems2_get_Count(This,retval)	\
    ( (This)->lpVtbl -> get_Count(This,retval) ) 

#define TaskItems2_Add(This,Category,SubCategory,Description,Priority,Icon,Checkable,File,Line,CanUserDelete,FlushItem,retval)	\
    ( (This)->lpVtbl -> Add(This,Category,SubCategory,Description,Priority,Icon,Checkable,File,Line,CanUserDelete,FlushItem,retval) ) 

#define TaskItems2__NewEnum(This,retval)	\
    ( (This)->lpVtbl -> _NewEnum(This,retval) ) 

#define TaskItems2_ForceItemsToTaskList(This,retval)	\
    ( (This)->lpVtbl -> ForceItemsToTaskList(This,retval) ) 


#define TaskItems2_Add2(This,Category,SubCategory,Description,Priority,Icon,Checkable,File,Line,CanUserDelete,FlushItem,AutoNavigate,pTaskItem)	\
    ( (This)->lpVtbl -> Add2(This,Category,SubCategory,Description,Priority,Icon,Checkable,File,Line,CanUserDelete,FlushItem,AutoNavigate,pTaskItem) ) 

#endif /* COBJMACROS */


#endif 	/* C style interface */




#endif 	/* __TaskItems2_INTERFACE_DEFINED__ */


#ifndef __EditPoint2_INTERFACE_DEFINED__
#define __EditPoint2_INTERFACE_DEFINED__

/* interface EditPoint2 */
/* [helpstringcontext][helpstring][helpcontext][uuid][object][oleautomation][dual] */ 


EXTERN_C const IID IID_EditPoint2;

#if defined(__cplusplus) && !defined(CINTERFACE)
    
    MIDL_INTERFACE("136AFA9F-F243-4abb-A8F8-4C2D26C47163")
    EditPoint2 : public EditPoint
    {
    public:
        virtual /* [helpstringcontext][helpstring][helpcontext][id] */ HRESULT STDMETHODCALLTYPE InsertNewLine( 
            /* [defaultvalue][in] */ long Count = 1) = 0;
        
    };
    
#else 	/* C style interface */

    typedef struct EditPoint2Vtbl
    {
        BEGIN_INTERFACE
        
        /* [id][restricted][funcdescattr] */ HRESULT ( STDMETHODCALLTYPE *QueryInterface )( 
            EditPoint2 * This,
            /* [in][idldescattr] */ __RPC__in struct GUID *riid,
            /* [out][idldescattr] */ __RPC__deref_out_opt void **ppvObj,
            /* [retval][out] */ __RPC__out void *retval);
        
        /* [id][restricted][funcdescattr] */ HRESULT ( STDMETHODCALLTYPE *AddRef )( 
            EditPoint2 * This,
            /* [retval][out] */ __RPC__out unsigned long *retval);
        
        /* [id][restricted][funcdescattr] */ HRESULT ( STDMETHODCALLTYPE *Release )( 
            EditPoint2 * This,
            /* [retval][out] */ __RPC__out unsigned long *retval);
        
        /* [id][restricted][funcdescattr] */ HRESULT ( STDMETHODCALLTYPE *GetTypeInfoCount )( 
            EditPoint2 * This,
            /* [out][idldescattr] */ __RPC__out unsigned UINT *pctinfo,
            /* [retval][out] */ __RPC__out void *retval);
        
        /* [id][restricted][funcdescattr] */ HRESULT ( STDMETHODCALLTYPE *GetTypeInfo )( 
            EditPoint2 * This,
            /* [in][idldescattr] */ unsigned UINT itinfo,
            /* [in][idldescattr] */ unsigned long lcid,
            /* [out][idldescattr] */ __RPC__deref_out_opt void **pptinfo,
            /* [retval][out] */ __RPC__out void *retval);
        
        /* [id][restricted][funcdescattr] */ HRESULT ( STDMETHODCALLTYPE *GetIDsOfNames )( 
            EditPoint2 * This,
            /* [in][idldescattr] */ __RPC__in struct GUID *riid,
            /* [in][idldescattr] */ __RPC__deref_in_opt signed char **rgszNames,
            /* [in][idldescattr] */ unsigned UINT cNames,
            /* [in][idldescattr] */ unsigned long lcid,
            /* [out][idldescattr] */ __RPC__out signed long *rgdispid,
            /* [retval][out] */ __RPC__out void *retval);
        
        /* [id][restricted][funcdescattr] */ HRESULT ( STDMETHODCALLTYPE *Invoke )( 
            EditPoint2 * This,
            /* [in][idldescattr] */ signed long dispidMember,
            /* [in][idldescattr] */ __RPC__in struct GUID *riid,
            /* [in][idldescattr] */ unsigned long lcid,
            /* [in][idldescattr] */ unsigned short wFlags,
            /* [in][idldescattr] */ __RPC__in struct DISPPARAMS *pdispparams,
            /* [out][idldescattr] */ __RPC__out VARIANT *pvarResult,
            /* [out][idldescattr] */ __RPC__out struct EXCEPINFO *pexcepinfo,
            /* [out][idldescattr] */ __RPC__out unsigned UINT *puArgErr,
            /* [retval][out] */ __RPC__out void *retval);
        
        /* [id][propget][funcdescattr] */ HRESULT ( STDMETHODCALLTYPE *get_DTE )( 
            EditPoint2 * This,
            /* [retval][out] */ __RPC__deref_out_opt **retval);
        
        /* [id][propget][funcdescattr] */ HRESULT ( STDMETHODCALLTYPE *get_Parent )( 
            EditPoint2 * This,
            /* [retval][out] */ __RPC__deref_out_opt TextDocument **retval);
        
        /* [id][propget][funcdescattr] */ HRESULT ( STDMETHODCALLTYPE *get_Line )( 
            EditPoint2 * This,
            /* [retval][out] */ __RPC__out signed long *retval);
        
        /* [id][propget][funcdescattr] */ HRESULT ( STDMETHODCALLTYPE *get_LineCharOffset )( 
            EditPoint2 * This,
            /* [retval][out] */ __RPC__out signed long *retval);
        
        /* [id][propget][funcdescattr] */ HRESULT ( STDMETHODCALLTYPE *get_AbsoluteCharOffset )( 
            EditPoint2 * This,
            /* [retval][out] */ __RPC__out signed long *retval);
        
        /* [id][propget][funcdescattr] */ HRESULT ( STDMETHODCALLTYPE *get_DisplayColumn )( 
            EditPoint2 * This,
            /* [retval][out] */ __RPC__out signed long *retval);
        
        /* [id][propget][funcdescattr] */ HRESULT ( STDMETHODCALLTYPE *get_AtEndOfDocument )( 
            EditPoint2 * This,
            /* [retval][out] */ __RPC__out BOOLEAN *retval);
        
        /* [id][propget][funcdescattr] */ HRESULT ( STDMETHODCALLTYPE *get_AtStartOfDocument )( 
            EditPoint2 * This,
            /* [retval][out] */ __RPC__out BOOLEAN *retval);
        
        /* [id][propget][funcdescattr] */ HRESULT ( STDMETHODCALLTYPE *get_AtEndOfLine )( 
            EditPoint2 * This,
            /* [retval][out] */ __RPC__out BOOLEAN *retval);
        
        /* [id][propget][funcdescattr] */ HRESULT ( STDMETHODCALLTYPE *get_AtStartOfLine )( 
            EditPoint2 * This,
            /* [retval][out] */ __RPC__out BOOLEAN *retval);
        
        /* [id][propget][funcdescattr] */ HRESULT ( STDMETHODCALLTYPE *get_LineLength )( 
            EditPoint2 * This,
            /* [retval][out] */ __RPC__out signed long *retval);
        
        /* [id][funcdescattr] */ HRESULT ( STDMETHODCALLTYPE *EqualTo )( 
            EditPoint2 * This,
            /* [in][idldescattr] */ __RPC__in_opt TextPoint *Point,
            /* [retval][out] */ __RPC__out BOOLEAN *retval);
        
        /* [id][funcdescattr] */ HRESULT ( STDMETHODCALLTYPE *LessThan )( 
            EditPoint2 * This,
            /* [in][idldescattr] */ __RPC__in_opt TextPoint *Point,
            /* [retval][out] */ __RPC__out BOOLEAN *retval);
        
        /* [id][funcdescattr] */ HRESULT ( STDMETHODCALLTYPE *GreaterThan )( 
            EditPoint2 * This,
            /* [in][idldescattr] */ __RPC__in_opt TextPoint *Point,
            /* [retval][out] */ __RPC__out BOOLEAN *retval);
        
        /* [id][funcdescattr] */ HRESULT ( STDMETHODCALLTYPE *TryToShow )( 
            EditPoint2 * This,
            /* [in][idldescattr] */ enum vsPaneShowHow How,
            /* [optional][in][idldescattr] */ VARIANT PointOrCount,
            /* [retval][out] */ __RPC__out BOOLEAN *retval);
        
        /* [id][propget][funcdescattr] */ HRESULT ( STDMETHODCALLTYPE *get_CodeElement )( 
            EditPoint2 * This,
            /* [in][idldescattr] */ enum vsCMElement Scope,
            /* [retval][out] */ __RPC__deref_out_opt CodeElement **retval);
        
        /* [id][funcdescattr] */ HRESULT ( STDMETHODCALLTYPE *CreateEditPoint )( 
            EditPoint2 * This,
            /* [retval][out] */ __RPC__deref_out_opt EditPoint **retval);
        
        /* [id][funcdescattr] */ HRESULT ( STDMETHODCALLTYPE *CharLeft )( 
            EditPoint2 * This,
            /* [in][idldescattr] */ signed long Count,
            /* [retval][out] */ __RPC__out void *retval);
        
        /* [id][funcdescattr] */ HRESULT ( STDMETHODCALLTYPE *CharRight )( 
            EditPoint2 * This,
            /* [in][idldescattr] */ signed long Count,
            /* [retval][out] */ __RPC__out void *retval);
        
        /* [id][funcdescattr] */ HRESULT ( STDMETHODCALLTYPE *EndOfLine )( 
            EditPoint2 * This,
            /* [retval][out] */ __RPC__out void *retval);
        
        /* [id][funcdescattr] */ HRESULT ( STDMETHODCALLTYPE *StartOfLine )( 
            EditPoint2 * This,
            /* [retval][out] */ __RPC__out void *retval);
        
        /* [id][funcdescattr] */ HRESULT ( STDMETHODCALLTYPE *EndOfDocument )( 
            EditPoint2 * This,
            /* [retval][out] */ __RPC__out void *retval);
        
        /* [id][funcdescattr] */ HRESULT ( STDMETHODCALLTYPE *StartOfDocument )( 
            EditPoint2 * This,
            /* [retval][out] */ __RPC__out void *retval);
        
        /* [id][funcdescattr] */ HRESULT ( STDMETHODCALLTYPE *WordLeft )( 
            EditPoint2 * This,
            /* [in][idldescattr] */ signed long Count,
            /* [retval][out] */ __RPC__out void *retval);
        
        /* [id][funcdescattr] */ HRESULT ( STDMETHODCALLTYPE *WordRight )( 
            EditPoint2 * This,
            /* [in][idldescattr] */ signed long Count,
            /* [retval][out] */ __RPC__out void *retval);
        
        /* [id][funcdescattr] */ HRESULT ( STDMETHODCALLTYPE *LineUp )( 
            EditPoint2 * This,
            /* [in][idldescattr] */ signed long Count,
            /* [retval][out] */ __RPC__out void *retval);
        
        /* [id][funcdescattr] */ HRESULT ( STDMETHODCALLTYPE *LineDown )( 
            EditPoint2 * This,
            /* [in][idldescattr] */ signed long Count,
            /* [retval][out] */ __RPC__out void *retval);
        
        /* [id][funcdescattr] */ HRESULT ( STDMETHODCALLTYPE *MoveToPoint )( 
            EditPoint2 * This,
            /* [in][idldescattr] */ __RPC__in_opt TextPoint *Point,
            /* [retval][out] */ __RPC__out void *retval);
        
        /* [id][funcdescattr] */ HRESULT ( STDMETHODCALLTYPE *MoveToLineAndOffset )( 
            EditPoint2 * This,
            /* [in][idldescattr] */ signed long Line,
            /* [in][idldescattr] */ signed long Offset,
            /* [retval][out] */ __RPC__out void *retval);
        
        /* [id][funcdescattr] */ HRESULT ( STDMETHODCALLTYPE *MoveToAbsoluteOffset )( 
            EditPoint2 * This,
            /* [in][idldescattr] */ signed long Offset,
            /* [retval][out] */ __RPC__out void *retval);
        
        /* [id][funcdescattr] */ HRESULT ( STDMETHODCALLTYPE *SetBookmark )( 
            EditPoint2 * This,
            /* [retval][out] */ __RPC__out void *retval);
        
        /* [id][funcdescattr] */ HRESULT ( STDMETHODCALLTYPE *ClearBookmark )( 
            EditPoint2 * This,
            /* [retval][out] */ __RPC__out void *retval);
        
        /* [id][funcdescattr] */ HRESULT ( STDMETHODCALLTYPE *NextBookmark )( 
            EditPoint2 * This,
            /* [retval][out] */ __RPC__out BOOLEAN *retval);
        
        /* [id][funcdescattr] */ HRESULT ( STDMETHODCALLTYPE *PreviousBookmark )( 
            EditPoint2 * This,
            /* [retval][out] */ __RPC__out BOOLEAN *retval);
        
        /* [id][funcdescattr] */ HRESULT ( STDMETHODCALLTYPE *PadToColumn )( 
            EditPoint2 * This,
            /* [in][idldescattr] */ signed long Column,
            /* [retval][out] */ __RPC__out void *retval);
        
        /* [id][funcdescattr] */ HRESULT ( STDMETHODCALLTYPE *Insert )( 
            EditPoint2 * This,
            /* [in][idldescattr] */ __RPC__in BSTR Text,
            /* [retval][out] */ __RPC__out void *retval);
        
        /* [id][funcdescattr] */ HRESULT ( STDMETHODCALLTYPE *InsertFromFile )( 
            EditPoint2 * This,
            /* [in][idldescattr] */ __RPC__in BSTR File,
            /* [retval][out] */ __RPC__out void *retval);
        
        /* [id][funcdescattr] */ HRESULT ( STDMETHODCALLTYPE *GetText )( 
            EditPoint2 * This,
            /* [in][idldescattr] */ VARIANT PointOrCount,
            /* [retval][out] */ __RPC__deref_out_opt BSTR *retval);
        
        /* [id][funcdescattr] */ HRESULT ( STDMETHODCALLTYPE *GetLines )( 
            EditPoint2 * This,
            /* [in][idldescattr] */ signed long Start,
            /* [in][idldescattr] */ signed long ExclusiveEnd,
            /* [retval][out] */ __RPC__deref_out_opt BSTR *retval);
        
        /* [id][funcdescattr] */ HRESULT ( STDMETHODCALLTYPE *Copy )( 
            EditPoint2 * This,
            /* [in][idldescattr] */ VARIANT PointOrCount,
            /* [in][idldescattr] */ BOOLEAN Append,
            /* [retval][out] */ __RPC__out void *retval);
        
        /* [id][funcdescattr] */ HRESULT ( STDMETHODCALLTYPE *Cut )( 
            EditPoint2 * This,
            /* [in][idldescattr] */ VARIANT PointOrCount,
            /* [in][idldescattr] */ BOOLEAN Append,
            /* [retval][out] */ __RPC__out void *retval);
        
        /* [id][funcdescattr] */ HRESULT ( STDMETHODCALLTYPE *Delete )( 
            EditPoint2 * This,
            /* [in][idldescattr] */ VARIANT PointOrCount,
            /* [retval][out] */ __RPC__out void *retval);
        
        /* [id][funcdescattr] */ HRESULT ( STDMETHODCALLTYPE *Paste )( 
            EditPoint2 * This,
            /* [retval][out] */ __RPC__out void *retval);
        
        /* [id][funcdescattr] */ HRESULT ( STDMETHODCALLTYPE *ReadOnly )( 
            EditPoint2 * This,
            /* [in][idldescattr] */ VARIANT PointOrCount,
            /* [retval][out] */ __RPC__out BOOLEAN *retval);
        
        /* [id][funcdescattr] */ HRESULT ( STDMETHODCALLTYPE *FindPattern )( 
            EditPoint2 * This,
            /* [in][idldescattr] */ __RPC__in BSTR Pattern,
            /* [in][idldescattr] */ signed long vsFindOptionsValue,
            /* [out][in][idldescattr] */ __RPC__deref_inout_opt EditPoint **EndPoint,
            /* [out][in][idldescattr] */ __RPC__deref_inout_opt TextRanges **Tags,
            /* [retval][out] */ __RPC__out BOOLEAN *retval);
        
        /* [id][funcdescattr] */ HRESULT ( STDMETHODCALLTYPE *ReplacePattern )( 
            EditPoint2 * This,
            /* [in][idldescattr] */ __RPC__in_opt TextPoint *Point,
            /* [in][idldescattr] */ __RPC__in BSTR Pattern,
            /* [in][idldescattr] */ __RPC__in BSTR Replace,
            /* [in][idldescattr] */ signed long vsFindOptionsValue,
            /* [out][in][idldescattr] */ __RPC__deref_inout_opt TextRanges **Tags,
            /* [retval][out] */ __RPC__out BOOLEAN *retval);
        
        /* [id][funcdescattr] */ HRESULT ( STDMETHODCALLTYPE *Indent )( 
            EditPoint2 * This,
            /* [in][idldescattr] */ __RPC__in_opt TextPoint *Point,
            /* [in][idldescattr] */ signed long Count,
            /* [retval][out] */ __RPC__out void *retval);
        
        /* [id][funcdescattr] */ HRESULT ( STDMETHODCALLTYPE *Unindent )( 
            EditPoint2 * This,
            /* [in][idldescattr] */ __RPC__in_opt TextPoint *Point,
            /* [in][idldescattr] */ signed long Count,
            /* [retval][out] */ __RPC__out void *retval);
        
        /* [id][funcdescattr] */ HRESULT ( STDMETHODCALLTYPE *SmartFormat )( 
            EditPoint2 * This,
            /* [in][idldescattr] */ __RPC__in_opt TextPoint *Point,
            /* [retval][out] */ __RPC__out void *retval);
        
        /* [id][funcdescattr] */ HRESULT ( STDMETHODCALLTYPE *OutlineSection )( 
            EditPoint2 * This,
            /* [in][idldescattr] */ VARIANT PointOrCount,
            /* [retval][out] */ __RPC__out void *retval);
        
        /* [id][funcdescattr] */ HRESULT ( STDMETHODCALLTYPE *ReplaceText )( 
            EditPoint2 * This,
            /* [in][idldescattr] */ VARIANT PointOrCount,
            /* [in][idldescattr] */ __RPC__in BSTR Text,
            /* [in][idldescattr] */ signed long Flags,
            /* [retval][out] */ __RPC__out void *retval);
        
        /* [id][funcdescattr] */ HRESULT ( STDMETHODCALLTYPE *ChangeCase )( 
            EditPoint2 * This,
            /* [in][idldescattr] */ VARIANT PointOrCount,
            /* [in][idldescattr] */ enum vsCaseOptions How,
            /* [retval][out] */ __RPC__out void *retval);
        
        /* [id][funcdescattr] */ HRESULT ( STDMETHODCALLTYPE *DeleteWhitespace )( 
            EditPoint2 * This,
            /* [in][idldescattr] */ enum vsWhitespaceOptions Direction,
            /* [retval][out] */ __RPC__out void *retval);
        
        /* [helpstringcontext][helpstring][helpcontext][id] */ HRESULT ( STDMETHODCALLTYPE *InsertNewLine )( 
            EditPoint2 * This,
            /* [defaultvalue][in] */ long Count);
        
        END_INTERFACE
    } EditPoint2Vtbl;

    interface EditPoint2
    {
        CONST_VTBL struct EditPoint2Vtbl *lpVtbl;
    };

    

#ifdef COBJMACROS


#define EditPoint2_QueryInterface(This,riid,ppvObj,retval)	\
    ( (This)->lpVtbl -> QueryInterface(This,riid,ppvObj,retval) ) 

#define EditPoint2_AddRef(This,retval)	\
    ( (This)->lpVtbl -> AddRef(This,retval) ) 

#define EditPoint2_Release(This,retval)	\
    ( (This)->lpVtbl -> Release(This,retval) ) 

#define EditPoint2_GetTypeInfoCount(This,pctinfo,retval)	\
    ( (This)->lpVtbl -> GetTypeInfoCount(This,pctinfo,retval) ) 

#define EditPoint2_GetTypeInfo(This,itinfo,lcid,pptinfo,retval)	\
    ( (This)->lpVtbl -> GetTypeInfo(This,itinfo,lcid,pptinfo,retval) ) 

#define EditPoint2_GetIDsOfNames(This,riid,rgszNames,cNames,lcid,rgdispid,retval)	\
    ( (This)->lpVtbl -> GetIDsOfNames(This,riid,rgszNames,cNames,lcid,rgdispid,retval) ) 

#define EditPoint2_Invoke(This,dispidMember,riid,lcid,wFlags,pdispparams,pvarResult,pexcepinfo,puArgErr,retval)	\
    ( (This)->lpVtbl -> Invoke(This,dispidMember,riid,lcid,wFlags,pdispparams,pvarResult,pexcepinfo,puArgErr,retval) ) 

#define EditPoint2_get_DTE(This,retval)	\
    ( (This)->lpVtbl -> get_DTE(This,retval) ) 

#define EditPoint2_get_Parent(This,retval)	\
    ( (This)->lpVtbl -> get_Parent(This,retval) ) 

#define EditPoint2_get_Line(This,retval)	\
    ( (This)->lpVtbl -> get_Line(This,retval) ) 

#define EditPoint2_get_LineCharOffset(This,retval)	\
    ( (This)->lpVtbl -> get_LineCharOffset(This,retval) ) 

#define EditPoint2_get_AbsoluteCharOffset(This,retval)	\
    ( (This)->lpVtbl -> get_AbsoluteCharOffset(This,retval) ) 

#define EditPoint2_get_DisplayColumn(This,retval)	\
    ( (This)->lpVtbl -> get_DisplayColumn(This,retval) ) 

#define EditPoint2_get_AtEndOfDocument(This,retval)	\
    ( (This)->lpVtbl -> get_AtEndOfDocument(This,retval) ) 

#define EditPoint2_get_AtStartOfDocument(This,retval)	\
    ( (This)->lpVtbl -> get_AtStartOfDocument(This,retval) ) 

#define EditPoint2_get_AtEndOfLine(This,retval)	\
    ( (This)->lpVtbl -> get_AtEndOfLine(This,retval) ) 

#define EditPoint2_get_AtStartOfLine(This,retval)	\
    ( (This)->lpVtbl -> get_AtStartOfLine(This,retval) ) 

#define EditPoint2_get_LineLength(This,retval)	\
    ( (This)->lpVtbl -> get_LineLength(This,retval) ) 

#define EditPoint2_EqualTo(This,Point,retval)	\
    ( (This)->lpVtbl -> EqualTo(This,Point,retval) ) 

#define EditPoint2_LessThan(This,Point,retval)	\
    ( (This)->lpVtbl -> LessThan(This,Point,retval) ) 

#define EditPoint2_GreaterThan(This,Point,retval)	\
    ( (This)->lpVtbl -> GreaterThan(This,Point,retval) ) 

#define EditPoint2_TryToShow(This,How,PointOrCount,retval)	\
    ( (This)->lpVtbl -> TryToShow(This,How,PointOrCount,retval) ) 

#define EditPoint2_get_CodeElement(This,Scope,retval)	\
    ( (This)->lpVtbl -> get_CodeElement(This,Scope,retval) ) 

#define EditPoint2_CreateEditPoint(This,retval)	\
    ( (This)->lpVtbl -> CreateEditPoint(This,retval) ) 

#define EditPoint2_CharLeft(This,Count,retval)	\
    ( (This)->lpVtbl -> CharLeft(This,Count,retval) ) 

#define EditPoint2_CharRight(This,Count,retval)	\
    ( (This)->lpVtbl -> CharRight(This,Count,retval) ) 

#define EditPoint2_EndOfLine(This,retval)	\
    ( (This)->lpVtbl -> EndOfLine(This,retval) ) 

#define EditPoint2_StartOfLine(This,retval)	\
    ( (This)->lpVtbl -> StartOfLine(This,retval) ) 

#define EditPoint2_EndOfDocument(This,retval)	\
    ( (This)->lpVtbl -> EndOfDocument(This,retval) ) 

#define EditPoint2_StartOfDocument(This,retval)	\
    ( (This)->lpVtbl -> StartOfDocument(This,retval) ) 

#define EditPoint2_WordLeft(This,Count,retval)	\
    ( (This)->lpVtbl -> WordLeft(This,Count,retval) ) 

#define EditPoint2_WordRight(This,Count,retval)	\
    ( (This)->lpVtbl -> WordRight(This,Count,retval) ) 

#define EditPoint2_LineUp(This,Count,retval)	\
    ( (This)->lpVtbl -> LineUp(This,Count,retval) ) 

#define EditPoint2_LineDown(This,Count,retval)	\
    ( (This)->lpVtbl -> LineDown(This,Count,retval) ) 

#define EditPoint2_MoveToPoint(This,Point,retval)	\
    ( (This)->lpVtbl -> MoveToPoint(This,Point,retval) ) 

#define EditPoint2_MoveToLineAndOffset(This,Line,Offset,retval)	\
    ( (This)->lpVtbl -> MoveToLineAndOffset(This,Line,Offset,retval) ) 

#define EditPoint2_MoveToAbsoluteOffset(This,Offset,retval)	\
    ( (This)->lpVtbl -> MoveToAbsoluteOffset(This,Offset,retval) ) 

#define EditPoint2_SetBookmark(This,retval)	\
    ( (This)->lpVtbl -> SetBookmark(This,retval) ) 

#define EditPoint2_ClearBookmark(This,retval)	\
    ( (This)->lpVtbl -> ClearBookmark(This,retval) ) 

#define EditPoint2_NextBookmark(This,retval)	\
    ( (This)->lpVtbl -> NextBookmark(This,retval) ) 

#define EditPoint2_PreviousBookmark(This,retval)	\
    ( (This)->lpVtbl -> PreviousBookmark(This,retval) ) 

#define EditPoint2_PadToColumn(This,Column,retval)	\
    ( (This)->lpVtbl -> PadToColumn(This,Column,retval) ) 

#define EditPoint2_Insert(This,Text,retval)	\
    ( (This)->lpVtbl -> Insert(This,Text,retval) ) 

#define EditPoint2_InsertFromFile(This,File,retval)	\
    ( (This)->lpVtbl -> InsertFromFile(This,File,retval) ) 

#define EditPoint2_GetText(This,PointOrCount,retval)	\
    ( (This)->lpVtbl -> GetText(This,PointOrCount,retval) ) 

#define EditPoint2_GetLines(This,Start,ExclusiveEnd,retval)	\
    ( (This)->lpVtbl -> GetLines(This,Start,ExclusiveEnd,retval) ) 

#define EditPoint2_Copy(This,PointOrCount,Append,retval)	\
    ( (This)->lpVtbl -> Copy(This,PointOrCount,Append,retval) ) 

#define EditPoint2_Cut(This,PointOrCount,Append,retval)	\
    ( (This)->lpVtbl -> Cut(This,PointOrCount,Append,retval) ) 

#define EditPoint2_Delete(This,PointOrCount,retval)	\
    ( (This)->lpVtbl -> Delete(This,PointOrCount,retval) ) 

#define EditPoint2_Paste(This,retval)	\
    ( (This)->lpVtbl -> Paste(This,retval) ) 

#define EditPoint2_ReadOnly(This,PointOrCount,retval)	\
    ( (This)->lpVtbl -> ReadOnly(This,PointOrCount,retval) ) 

#define EditPoint2_FindPattern(This,Pattern,vsFindOptionsValue,EndPoint,Tags,retval)	\
    ( (This)->lpVtbl -> FindPattern(This,Pattern,vsFindOptionsValue,EndPoint,Tags,retval) ) 

#define EditPoint2_ReplacePattern(This,Point,Pattern,Replace,vsFindOptionsValue,Tags,retval)	\
    ( (This)->lpVtbl -> ReplacePattern(This,Point,Pattern,Replace,vsFindOptionsValue,Tags,retval) ) 

#define EditPoint2_Indent(This,Point,Count,retval)	\
    ( (This)->lpVtbl -> Indent(This,Point,Count,retval) ) 

#define EditPoint2_Unindent(This,Point,Count,retval)	\
    ( (This)->lpVtbl -> Unindent(This,Point,Count,retval) ) 

#define EditPoint2_SmartFormat(This,Point,retval)	\
    ( (This)->lpVtbl -> SmartFormat(This,Point,retval) ) 

#define EditPoint2_OutlineSection(This,PointOrCount,retval)	\
    ( (This)->lpVtbl -> OutlineSection(This,PointOrCount,retval) ) 

#define EditPoint2_ReplaceText(This,PointOrCount,Text,Flags,retval)	\
    ( (This)->lpVtbl -> ReplaceText(This,PointOrCount,Text,Flags,retval) ) 

#define EditPoint2_ChangeCase(This,PointOrCount,How,retval)	\
    ( (This)->lpVtbl -> ChangeCase(This,PointOrCount,How,retval) ) 

#define EditPoint2_DeleteWhitespace(This,Direction,retval)	\
    ( (This)->lpVtbl -> DeleteWhitespace(This,Direction,retval) ) 


#define EditPoint2_InsertNewLine(This,Count)	\
    ( (This)->lpVtbl -> InsertNewLine(This,Count) ) 

#endif /* COBJMACROS */


#endif 	/* C style interface */




#endif 	/* __EditPoint2_INTERFACE_DEFINED__ */


#ifndef __IVsExtensibility2_INTERFACE_DEFINED__
#define __IVsExtensibility2_INTERFACE_DEFINED__

/* interface IVsExtensibility2 */
/* [restricted][hidden][uuid][object][oleautomation] */ 


EXTERN_C const IID IID_IVsExtensibility2;

#if defined(__cplusplus) && !defined(CINTERFACE)
    
    MIDL_INTERFACE("12133BC2-390A-4b30-AD5B-504C1D1C89E7")
    IVsExtensibility2 : public IVsExtensibility
    {
    public:
        virtual /* [id] */ HRESULT STDMETHODCALLTYPE FireProjectsEvent_ItemAdded( 
            /* [in] */ __RPC__in /* external definition not present */ Project *Project) = 0;
        
        virtual /* [id] */ HRESULT STDMETHODCALLTYPE FireProjectsEvent_ItemRemoved( 
            /* [in] */ __RPC__in /* external definition not present */ Project *Project) = 0;
        
        virtual /* [id] */ HRESULT STDMETHODCALLTYPE FireProjectsEvent_ItemRenamed( 
            /* [in] */ __RPC__in /* external definition not present */ Project *Project,
            /* [in] */ __RPC__in BSTR OldName) = 0;
        
        virtual /* [id] */ HRESULT STDMETHODCALLTYPE FireProjectItemsEvent_ItemAdded( 
            /* [in] */ __RPC__in /* external definition not present */ ProjectItem *ProjectItem) = 0;
        
        virtual /* [id] */ HRESULT STDMETHODCALLTYPE FireProjectItemsEvent_ItemRemoved( 
            /* [in] */ __RPC__in /* external definition not present */ ProjectItem *ProjectItem) = 0;
        
        virtual /* [id] */ HRESULT STDMETHODCALLTYPE FireProjectItemsEvent_ItemRenamed( 
            /* [in] */ __RPC__in /* external definition not present */ ProjectItem *ProjectItem,
            /* [in] */ __RPC__in BSTR OldName) = 0;
        
        virtual /* [id] */ HRESULT STDMETHODCALLTYPE BuildUIHierarchyFromTree( 
            /* external definition not present */ OLE_HANDLE hwnd,
            __RPC__in /* external definition not present */ Window *pParent,
            /* [retval][out] */ __RPC__deref_out_opt /* external definition not present */ UIHierarchy **ppUIHierarchy) = 0;
        
        virtual /* [id] */ HRESULT STDMETHODCALLTYPE FireCodeModelEvent( 
            DISPID dispid,
            __RPC__in /* external definition not present */ CodeElement *pElement,
            vsCMChangeKind changeKind) = 0;
        
        virtual /* [id] */ HRESULT STDMETHODCALLTYPE IsFireCodeModelEventNeeded( 
            __RPC__in VARIANT_BOOL *vbNeeded) = 0;
        
        virtual /* [id] */ HRESULT STDMETHODCALLTYPE RunWizardFileEx( 
            /* [in] */ __RPC__in BSTR bstrWizFilename,
            /* [in] */ long hwndOwner,
            /* [in] */ __RPC__deref_in_opt SAFEARRAY * *vContextParams,
            /* [in] */ __RPC__deref_in_opt SAFEARRAY * *vCustomParams,
            /* [retval][out] */ __RPC__out long *pResult) = 0;
        
        virtual /* [id] */ HRESULT STDMETHODCALLTYPE FireCodeModelEvent3( 
            DISPID dispid,
            __RPC__in_opt IDispatch *pParent,
            __RPC__in /* external definition not present */ CodeElement *pElement,
            vsCMChangeKind changeKind) = 0;
        
    };
    
#else 	/* C style interface */

    typedef struct IVsExtensibility2Vtbl
    {
        BEGIN_INTERFACE
        
        /* [id][restricted][funcdescattr] */ HRESULT ( STDMETHODCALLTYPE *QueryInterface )( 
            IVsExtensibility2 * This,
            /* [in][idldescattr] */ __RPC__in struct GUID *riid,
            /* [out][idldescattr] */ __RPC__deref_out_opt void **ppvObj);
        
        /* [id][restricted][funcdescattr] */ unsigned long ( STDMETHODCALLTYPE *AddRef )( 
            IVsExtensibility2 * This);
        
        /* [id][restricted][funcdescattr] */ unsigned long ( STDMETHODCALLTYPE *Release )( 
            IVsExtensibility2 * This);
        
        /* [id][funcdescattr] */ HRESULT ( STDMETHODCALLTYPE *get_Properties )( 
            IVsExtensibility2 * This,
            /* [in][idldescattr] */ __RPC__in_opt ISupportVSProperties *pParent,
            /* [in][idldescattr] */ __RPC__in_opt IDispatch *pdispPropObj,
            /* [out][idldescattr] */ __RPC__deref_out_opt Properties **ppProperties);
        
        /* [id][funcdescattr] */ HRESULT ( STDMETHODCALLTYPE *RunWizardFile )( 
            IVsExtensibility2 * This,
            /* [in][idldescattr] */ __RPC__in BSTR bstrWizFilename,
            /* [in][idldescattr] */ signed long hwndOwner,
            /* [in][idldescattr] */ __RPC__deref_in_opt SAFEARRAY * *vContextParams,
            /* [out][idldescattr] */ __RPC__out enum wizardResult *pResult);
        
        /* [id][funcdescattr] */ HRESULT ( STDMETHODCALLTYPE *Get_TextBuffer )( 
            IVsExtensibility2 * This,
            /* [in][idldescattr] */ __RPC__in_opt IUnknown *pVsTextStream,
            /* [in][idldescattr] */ __RPC__in_opt IExtensibleObjectSite *pParent,
            /* [out][idldescattr] */ __RPC__deref_out_opt TextBuffer **ppTextBuffer);
        
        /* [id][funcdescattr] */ HRESULT ( STDMETHODCALLTYPE *EnterAutomationFunction )( 
            IVsExtensibility2 * This);
        
        /* [id][funcdescattr] */ HRESULT ( STDMETHODCALLTYPE *ExitAutomationFunction )( 
            IVsExtensibility2 * This);
        
        /* [id][funcdescattr] */ HRESULT ( STDMETHODCALLTYPE *IsInAutomationFunction )( 
            IVsExtensibility2 * This,
            /* [out][idldescattr] */ __RPC__out signed long *pfInAutoFunc);
        
        /* [id][funcdescattr] */ HRESULT ( STDMETHODCALLTYPE *GetUserControl )( 
            IVsExtensibility2 * This,
            /* [out][idldescattr] */ __RPC__out BOOLEAN *fUserControl);
        
        /* [id][funcdescattr] */ HRESULT ( STDMETHODCALLTYPE *SetUserControl )( 
            IVsExtensibility2 * This,
            /* [in][idldescattr] */ BOOLEAN fUserControl);
        
        /* [id][funcdescattr] */ HRESULT ( STDMETHODCALLTYPE *SetUserControlUnlatched )( 
            IVsExtensibility2 * This,
            /* [in][idldescattr] */ BOOLEAN fUserControl);
        
        /* [id][funcdescattr] */ HRESULT ( STDMETHODCALLTYPE *LockServer )( 
            IVsExtensibility2 * This,
            /* [in][idldescattr] */ BOOLEAN __MIDL__IVsExtensibility0000);
        
        /* [id][funcdescattr] */ HRESULT ( STDMETHODCALLTYPE *GetLockCount )( 
            IVsExtensibility2 * This,
            /* [out][idldescattr] */ __RPC__out signed long *pCount);
        
        /* [id][funcdescattr] */ HRESULT ( STDMETHODCALLTYPE *TestForShutdown )( 
            IVsExtensibility2 * This,
            /* [out][idldescattr] */ __RPC__out BOOLEAN *fShutdown);
        
        /* [id][funcdescattr] */ HRESULT ( STDMETHODCALLTYPE *GetGlobalsObject )( 
            IVsExtensibility2 * This,
            /* [in][idldescattr] */ VARIANT ExtractFrom,
            /* [out][idldescattr] */ __RPC__deref_out_opt Globals **ppGlobals);
        
        /* [id][funcdescattr] */ HRESULT ( STDMETHODCALLTYPE *GetConfigMgr )( 
            IVsExtensibility2 * This,
            /* [in][idldescattr] */ __RPC__in_opt IUnknown *pIVsProject,
            /* [idldescattr] */ ULONG_PTR itemid,
            /* [out][idldescattr] */ __RPC__deref_out_opt ConfigurationManager **ppCfgMgr);
        
        /* [id][funcdescattr] */ HRESULT ( STDMETHODCALLTYPE *FireMacroReset )( 
            IVsExtensibility2 * This);
        
        /* [id][funcdescattr] */ HRESULT ( STDMETHODCALLTYPE *GetDocumentFromDocCookie )( 
            IVsExtensibility2 * This,
            /* [idldescattr] */ LONG_PTR lDocCookie,
            /* [out][idldescattr] */ __RPC__deref_out_opt Document **ppDoc);
        
        /* [id][funcdescattr] */ HRESULT ( STDMETHODCALLTYPE *IsMethodDisabled )( 
            IVsExtensibility2 * This,
            /* [idldescattr] */ __RPC__in struct GUID *pGUID,
            /* [idldescattr] */ signed long dispid);
        
        /* [id][funcdescattr] */ HRESULT ( STDMETHODCALLTYPE *SetSuppressUI )( 
            IVsExtensibility2 * This,
            /* [idldescattr] */ BOOLEAN In);
        
        /* [id][funcdescattr] */ HRESULT ( STDMETHODCALLTYPE *GetSuppressUI )( 
            IVsExtensibility2 * This,
            /* [idldescattr] */ __RPC__in BOOLEAN *pOut);
        
        /* [id] */ HRESULT ( STDMETHODCALLTYPE *FireProjectsEvent_ItemAdded )( 
            IVsExtensibility2 * This,
            /* [in] */ __RPC__in /* external definition not present */ Project *Project);
        
        /* [id] */ HRESULT ( STDMETHODCALLTYPE *FireProjectsEvent_ItemRemoved )( 
            IVsExtensibility2 * This,
            /* [in] */ __RPC__in /* external definition not present */ Project *Project);
        
        /* [id] */ HRESULT ( STDMETHODCALLTYPE *FireProjectsEvent_ItemRenamed )( 
            IVsExtensibility2 * This,
            /* [in] */ __RPC__in /* external definition not present */ Project *Project,
            /* [in] */ __RPC__in BSTR OldName);
        
        /* [id] */ HRESULT ( STDMETHODCALLTYPE *FireProjectItemsEvent_ItemAdded )( 
            IVsExtensibility2 * This,
            /* [in] */ __RPC__in /* external definition not present */ ProjectItem *ProjectItem);
        
        /* [id] */ HRESULT ( STDMETHODCALLTYPE *FireProjectItemsEvent_ItemRemoved )( 
            IVsExtensibility2 * This,
            /* [in] */ __RPC__in /* external definition not present */ ProjectItem *ProjectItem);
        
        /* [id] */ HRESULT ( STDMETHODCALLTYPE *FireProjectItemsEvent_ItemRenamed )( 
            IVsExtensibility2 * This,
            /* [in] */ __RPC__in /* external definition not present */ ProjectItem *ProjectItem,
            /* [in] */ __RPC__in BSTR OldName);
        
        /* [id] */ HRESULT ( STDMETHODCALLTYPE *BuildUIHierarchyFromTree )( 
            IVsExtensibility2 * This,
            /* external definition not present */ OLE_HANDLE hwnd,
            __RPC__in /* external definition not present */ Window *pParent,
            /* [retval][out] */ __RPC__deref_out_opt /* external definition not present */ UIHierarchy **ppUIHierarchy);
        
        /* [id] */ HRESULT ( STDMETHODCALLTYPE *FireCodeModelEvent )( 
            IVsExtensibility2 * This,
            DISPID dispid,
            __RPC__in /* external definition not present */ CodeElement *pElement,
            vsCMChangeKind changeKind);
        
        /* [id] */ HRESULT ( STDMETHODCALLTYPE *IsFireCodeModelEventNeeded )( 
            IVsExtensibility2 * This,
            __RPC__in VARIANT_BOOL *vbNeeded);
        
        /* [id] */ HRESULT ( STDMETHODCALLTYPE *RunWizardFileEx )( 
            IVsExtensibility2 * This,
            /* [in] */ __RPC__in BSTR bstrWizFilename,
            /* [in] */ long hwndOwner,
            /* [in] */ __RPC__deref_in_opt SAFEARRAY * *vContextParams,
            /* [in] */ __RPC__deref_in_opt SAFEARRAY * *vCustomParams,
            /* [retval][out] */ __RPC__out long *pResult);
        
        /* [id] */ HRESULT ( STDMETHODCALLTYPE *FireCodeModelEvent3 )( 
            IVsExtensibility2 * This,
            DISPID dispid,
            __RPC__in_opt IDispatch *pParent,
            __RPC__in /* external definition not present */ CodeElement *pElement,
            vsCMChangeKind changeKind);
        
        END_INTERFACE
    } IVsExtensibility2Vtbl;

    interface IVsExtensibility2
    {
        CONST_VTBL struct IVsExtensibility2Vtbl *lpVtbl;
    };

    

#ifdef COBJMACROS


#define IVsExtensibility2_QueryInterface(This,riid,ppvObj)	\
    ( (This)->lpVtbl -> QueryInterface(This,riid,ppvObj) ) 

#define IVsExtensibility2_AddRef(This)	\
    ( (This)->lpVtbl -> AddRef(This) ) 

#define IVsExtensibility2_Release(This)	\
    ( (This)->lpVtbl -> Release(This) ) 


#define IVsExtensibility2_get_Properties(This,pParent,pdispPropObj,ppProperties)	\
    ( (This)->lpVtbl -> get_Properties(This,pParent,pdispPropObj,ppProperties) ) 

#define IVsExtensibility2_RunWizardFile(This,bstrWizFilename,hwndOwner,vContextParams,pResult)	\
    ( (This)->lpVtbl -> RunWizardFile(This,bstrWizFilename,hwndOwner,vContextParams,pResult) ) 

#define IVsExtensibility2_Get_TextBuffer(This,pVsTextStream,pParent,ppTextBuffer)	\
    ( (This)->lpVtbl -> Get_TextBuffer(This,pVsTextStream,pParent,ppTextBuffer) ) 

#define IVsExtensibility2_EnterAutomationFunction(This)	\
    ( (This)->lpVtbl -> EnterAutomationFunction(This) ) 

#define IVsExtensibility2_ExitAutomationFunction(This)	\
    ( (This)->lpVtbl -> ExitAutomationFunction(This) ) 

#define IVsExtensibility2_IsInAutomationFunction(This,pfInAutoFunc)	\
    ( (This)->lpVtbl -> IsInAutomationFunction(This,pfInAutoFunc) ) 

#define IVsExtensibility2_GetUserControl(This,fUserControl)	\
    ( (This)->lpVtbl -> GetUserControl(This,fUserControl) ) 

#define IVsExtensibility2_SetUserControl(This,fUserControl)	\
    ( (This)->lpVtbl -> SetUserControl(This,fUserControl) ) 

#define IVsExtensibility2_SetUserControlUnlatched(This,fUserControl)	\
    ( (This)->lpVtbl -> SetUserControlUnlatched(This,fUserControl) ) 

#define IVsExtensibility2_LockServer(This,__MIDL__IVsExtensibility0000)	\
    ( (This)->lpVtbl -> LockServer(This,__MIDL__IVsExtensibility0000) ) 

#define IVsExtensibility2_GetLockCount(This,pCount)	\
    ( (This)->lpVtbl -> GetLockCount(This,pCount) ) 

#define IVsExtensibility2_TestForShutdown(This,fShutdown)	\
    ( (This)->lpVtbl -> TestForShutdown(This,fShutdown) ) 

#define IVsExtensibility2_GetGlobalsObject(This,ExtractFrom,ppGlobals)	\
    ( (This)->lpVtbl -> GetGlobalsObject(This,ExtractFrom,ppGlobals) ) 

#define IVsExtensibility2_GetConfigMgr(This,pIVsProject,itemid,ppCfgMgr)	\
    ( (This)->lpVtbl -> GetConfigMgr(This,pIVsProject,itemid,ppCfgMgr) ) 

#define IVsExtensibility2_FireMacroReset(This)	\
    ( (This)->lpVtbl -> FireMacroReset(This) ) 

#define IVsExtensibility2_GetDocumentFromDocCookie(This,lDocCookie,ppDoc)	\
    ( (This)->lpVtbl -> GetDocumentFromDocCookie(This,lDocCookie,ppDoc) ) 

#define IVsExtensibility2_IsMethodDisabled(This,pGUID,dispid)	\
    ( (This)->lpVtbl -> IsMethodDisabled(This,pGUID,dispid) ) 

#define IVsExtensibility2_SetSuppressUI(This,In)	\
    ( (This)->lpVtbl -> SetSuppressUI(This,In) ) 

#define IVsExtensibility2_GetSuppressUI(This,pOut)	\
    ( (This)->lpVtbl -> GetSuppressUI(This,pOut) ) 


#define IVsExtensibility2_FireProjectsEvent_ItemAdded(This,Project)	\
    ( (This)->lpVtbl -> FireProjectsEvent_ItemAdded(This,Project) ) 

#define IVsExtensibility2_FireProjectsEvent_ItemRemoved(This,Project)	\
    ( (This)->lpVtbl -> FireProjectsEvent_ItemRemoved(This,Project) ) 

#define IVsExtensibility2_FireProjectsEvent_ItemRenamed(This,Project,OldName)	\
    ( (This)->lpVtbl -> FireProjectsEvent_ItemRenamed(This,Project,OldName) ) 

#define IVsExtensibility2_FireProjectItemsEvent_ItemAdded(This,ProjectItem)	\
    ( (This)->lpVtbl -> FireProjectItemsEvent_ItemAdded(This,ProjectItem) ) 

#define IVsExtensibility2_FireProjectItemsEvent_ItemRemoved(This,ProjectItem)	\
    ( (This)->lpVtbl -> FireProjectItemsEvent_ItemRemoved(This,ProjectItem) ) 

#define IVsExtensibility2_FireProjectItemsEvent_ItemRenamed(This,ProjectItem,OldName)	\
    ( (This)->lpVtbl -> FireProjectItemsEvent_ItemRenamed(This,ProjectItem,OldName) ) 

#define IVsExtensibility2_BuildUIHierarchyFromTree(This,hwnd,pParent,ppUIHierarchy)	\
    ( (This)->lpVtbl -> BuildUIHierarchyFromTree(This,hwnd,pParent,ppUIHierarchy) ) 

#define IVsExtensibility2_FireCodeModelEvent(This,dispid,pElement,changeKind)	\
    ( (This)->lpVtbl -> FireCodeModelEvent(This,dispid,pElement,changeKind) ) 

#define IVsExtensibility2_IsFireCodeModelEventNeeded(This,vbNeeded)	\
    ( (This)->lpVtbl -> IsFireCodeModelEventNeeded(This,vbNeeded) ) 

#define IVsExtensibility2_RunWizardFileEx(This,bstrWizFilename,hwndOwner,vContextParams,vCustomParams,pResult)	\
    ( (This)->lpVtbl -> RunWizardFileEx(This,bstrWizFilename,hwndOwner,vContextParams,vCustomParams,pResult) ) 

#define IVsExtensibility2_FireCodeModelEvent3(This,dispid,pParent,pElement,changeKind)	\
    ( (This)->lpVtbl -> FireCodeModelEvent3(This,dispid,pParent,pElement,changeKind) ) 

#endif /* COBJMACROS */


#endif 	/* C style interface */




#endif 	/* __IVsExtensibility2_INTERFACE_DEFINED__ */


#ifndef __IInternalExtenderProvider_INTERFACE_DEFINED__
#define __IInternalExtenderProvider_INTERFACE_DEFINED__

/* interface IInternalExtenderProvider */
/* [uuid][restricted][hidden][object][oleautomation][dual] */ 


EXTERN_C const IID IID_IInternalExtenderProvider;

#if defined(__cplusplus) && !defined(CINTERFACE)
    
    MIDL_INTERFACE("B85F43C4-C765-4984-AE3D-695E8CD8E992")
    IInternalExtenderProvider : public IDispatch
    {
    public:
        virtual /* [id] */ HRESULT STDMETHODCALLTYPE GetExtenderNames( 
            /* [in] */ __RPC__in BSTR ExtenderCATID,
            /* [in] */ __RPC__in_opt IUnknown *ExtendeeObject,
            /* [retval][out] */ __RPC__out VARIANT *ExtenderNames) = 0;
        
        virtual /* [id] */ HRESULT STDMETHODCALLTYPE GetExtender( 
            /* [in] */ __RPC__in BSTR ExtenderCATID,
            /* [in] */ __RPC__in BSTR ExtenderName,
            /* [in] */ __RPC__in_opt IDispatch *ExtendeeObject,
            /* [in] */ __RPC__in /* external definition not present */ IExtenderSite *ExtenderSite,
            /* [in] */ long Cookie,
            /* [retval][out] */ __RPC__deref_out_opt IDispatch **Extender) = 0;
        
        virtual /* [id] */ HRESULT STDMETHODCALLTYPE CanExtend( 
            /* [in] */ __RPC__in BSTR ExtenderCATID,
            /* [in] */ __RPC__in BSTR ExtenderName,
            /* [in] */ __RPC__in_opt IDispatch *ExtendeeObject,
            /* [retval][out] */ __RPC__out VARIANT_BOOL *fRetval) = 0;
        
    };
    
#else 	/* C style interface */

    typedef struct IInternalExtenderProviderVtbl
    {
        BEGIN_INTERFACE
        
        HRESULT ( STDMETHODCALLTYPE *QueryInterface )( 
            IInternalExtenderProvider * This,
            /* [in] */ __RPC__in REFIID riid,
            /* [iid_is][out] */ 
            __RPC__deref_out  void **ppvObject);
        
        ULONG ( STDMETHODCALLTYPE *AddRef )( 
            IInternalExtenderProvider * This);
        
        ULONG ( STDMETHODCALLTYPE *Release )( 
            IInternalExtenderProvider * This);
        
        HRESULT ( STDMETHODCALLTYPE *GetTypeInfoCount )( 
            IInternalExtenderProvider * This,
            /* [out] */ __RPC__out UINT *pctinfo);
        
        HRESULT ( STDMETHODCALLTYPE *GetTypeInfo )( 
            IInternalExtenderProvider * This,
            /* [in] */ UINT iTInfo,
            /* [in] */ LCID lcid,
            /* [out] */ __RPC__deref_out_opt ITypeInfo **ppTInfo);
        
        HRESULT ( STDMETHODCALLTYPE *GetIDsOfNames )( 
            IInternalExtenderProvider * This,
            /* [in] */ __RPC__in REFIID riid,
            /* [size_is][in] */ __RPC__in_ecount_full(cNames) LPOLESTR *rgszNames,
            /* [range][in] */ UINT cNames,
            /* [in] */ LCID lcid,
            /* [size_is][out] */ __RPC__out_ecount_full(cNames) DISPID *rgDispId);
        
        /* [local] */ HRESULT ( STDMETHODCALLTYPE *Invoke )( 
            IInternalExtenderProvider * This,
            /* [in] */ DISPID dispIdMember,
            /* [in] */ REFIID riid,
            /* [in] */ LCID lcid,
            /* [in] */ WORD wFlags,
            /* [out][in] */ DISPPARAMS *pDispParams,
            /* [out] */ VARIANT *pVarResult,
            /* [out] */ EXCEPINFO *pExcepInfo,
            /* [out] */ UINT *puArgErr);
        
        /* [id] */ HRESULT ( STDMETHODCALLTYPE *GetExtenderNames )( 
            IInternalExtenderProvider * This,
            /* [in] */ __RPC__in BSTR ExtenderCATID,
            /* [in] */ __RPC__in_opt IUnknown *ExtendeeObject,
            /* [retval][out] */ __RPC__out VARIANT *ExtenderNames);
        
        /* [id] */ HRESULT ( STDMETHODCALLTYPE *GetExtender )( 
            IInternalExtenderProvider * This,
            /* [in] */ __RPC__in BSTR ExtenderCATID,
            /* [in] */ __RPC__in BSTR ExtenderName,
            /* [in] */ __RPC__in_opt IDispatch *ExtendeeObject,
            /* [in] */ __RPC__in /* external definition not present */ IExtenderSite *ExtenderSite,
            /* [in] */ long Cookie,
            /* [retval][out] */ __RPC__deref_out_opt IDispatch **Extender);
        
        /* [id] */ HRESULT ( STDMETHODCALLTYPE *CanExtend )( 
            IInternalExtenderProvider * This,
            /* [in] */ __RPC__in BSTR ExtenderCATID,
            /* [in] */ __RPC__in BSTR ExtenderName,
            /* [in] */ __RPC__in_opt IDispatch *ExtendeeObject,
            /* [retval][out] */ __RPC__out VARIANT_BOOL *fRetval);
        
        END_INTERFACE
    } IInternalExtenderProviderVtbl;

    interface IInternalExtenderProvider
    {
        CONST_VTBL struct IInternalExtenderProviderVtbl *lpVtbl;
    };

    

#ifdef COBJMACROS


#define IInternalExtenderProvider_QueryInterface(This,riid,ppvObject)	\
    ( (This)->lpVtbl -> QueryInterface(This,riid,ppvObject) ) 

#define IInternalExtenderProvider_AddRef(This)	\
    ( (This)->lpVtbl -> AddRef(This) ) 

#define IInternalExtenderProvider_Release(This)	\
    ( (This)->lpVtbl -> Release(This) ) 


#define IInternalExtenderProvider_GetTypeInfoCount(This,pctinfo)	\
    ( (This)->lpVtbl -> GetTypeInfoCount(This,pctinfo) ) 

#define IInternalExtenderProvider_GetTypeInfo(This,iTInfo,lcid,ppTInfo)	\
    ( (This)->lpVtbl -> GetTypeInfo(This,iTInfo,lcid,ppTInfo) ) 

#define IInternalExtenderProvider_GetIDsOfNames(This,riid,rgszNames,cNames,lcid,rgDispId)	\
    ( (This)->lpVtbl -> GetIDsOfNames(This,riid,rgszNames,cNames,lcid,rgDispId) ) 

#define IInternalExtenderProvider_Invoke(This,dispIdMember,riid,lcid,wFlags,pDispParams,pVarResult,pExcepInfo,puArgErr)	\
    ( (This)->lpVtbl -> Invoke(This,dispIdMember,riid,lcid,wFlags,pDispParams,pVarResult,pExcepInfo,puArgErr) ) 


#define IInternalExtenderProvider_GetExtenderNames(This,ExtenderCATID,ExtendeeObject,ExtenderNames)	\
    ( (This)->lpVtbl -> GetExtenderNames(This,ExtenderCATID,ExtendeeObject,ExtenderNames) ) 

#define IInternalExtenderProvider_GetExtender(This,ExtenderCATID,ExtenderName,ExtendeeObject,ExtenderSite,Cookie,Extender)	\
    ( (This)->lpVtbl -> GetExtender(This,ExtenderCATID,ExtenderName,ExtendeeObject,ExtenderSite,Cookie,Extender) ) 

#define IInternalExtenderProvider_CanExtend(This,ExtenderCATID,ExtenderName,ExtendeeObject,fRetval)	\
    ( (This)->lpVtbl -> CanExtend(This,ExtenderCATID,ExtenderName,ExtendeeObject,fRetval) ) 

#endif /* COBJMACROS */


#endif 	/* C style interface */




#endif 	/* __IInternalExtenderProvider_INTERFACE_DEFINED__ */


#ifndef __Find2_INTERFACE_DEFINED__
#define __Find2_INTERFACE_DEFINED__

/* interface Find2 */
/* [helpstringcontext][helpstring][helpcontext][uuid][object][oleautomation][dual] */ 


EXTERN_C const IID IID_Find2;

#if defined(__cplusplus) && !defined(CINTERFACE)
    
    MIDL_INTERFACE("01568308-5b2a-4f30-8d0a-e10ee0f28f4a")
    Find2 : public Find
    {
    public:
        virtual /* [helpstringcontext][helpstring][helpcontext][propget][id] */ HRESULT STDMETHODCALLTYPE get_WaitForFindToComplete( 
            /* [retval][out] */ __RPC__out VARIANT_BOOL *pWait) = 0;
        
        virtual /* [helpstringcontext][helpstring][helpcontext][propput][id] */ HRESULT STDMETHODCALLTYPE put_WaitForFindToComplete( 
            VARIANT_BOOL Wait) = 0;
        
    };
    
#else 	/* C style interface */

    typedef struct Find2Vtbl
    {
        BEGIN_INTERFACE
        
        /* [id][restricted][funcdescattr] */ HRESULT ( STDMETHODCALLTYPE *QueryInterface )( 
            Find2 * This,
            /* [in][idldescattr] */ __RPC__in struct GUID *riid,
            /* [out][idldescattr] */ __RPC__deref_out_opt void **ppvObj,
            /* [retval][out] */ __RPC__out void *retval);
        
        /* [id][restricted][funcdescattr] */ HRESULT ( STDMETHODCALLTYPE *AddRef )( 
            Find2 * This,
            /* [retval][out] */ __RPC__out unsigned long *retval);
        
        /* [id][restricted][funcdescattr] */ HRESULT ( STDMETHODCALLTYPE *Release )( 
            Find2 * This,
            /* [retval][out] */ __RPC__out unsigned long *retval);
        
        /* [id][restricted][funcdescattr] */ HRESULT ( STDMETHODCALLTYPE *GetTypeInfoCount )( 
            Find2 * This,
            /* [out][idldescattr] */ __RPC__out unsigned UINT *pctinfo,
            /* [retval][out] */ __RPC__out void *retval);
        
        /* [id][restricted][funcdescattr] */ HRESULT ( STDMETHODCALLTYPE *GetTypeInfo )( 
            Find2 * This,
            /* [in][idldescattr] */ unsigned UINT itinfo,
            /* [in][idldescattr] */ unsigned long lcid,
            /* [out][idldescattr] */ __RPC__deref_out_opt void **pptinfo,
            /* [retval][out] */ __RPC__out void *retval);
        
        /* [id][restricted][funcdescattr] */ HRESULT ( STDMETHODCALLTYPE *GetIDsOfNames )( 
            Find2 * This,
            /* [in][idldescattr] */ __RPC__in struct GUID *riid,
            /* [in][idldescattr] */ __RPC__deref_in_opt signed char **rgszNames,
            /* [in][idldescattr] */ unsigned UINT cNames,
            /* [in][idldescattr] */ unsigned long lcid,
            /* [out][idldescattr] */ __RPC__out signed long *rgdispid,
            /* [retval][out] */ __RPC__out void *retval);
        
        /* [id][restricted][funcdescattr] */ HRESULT ( STDMETHODCALLTYPE *Invoke )( 
            Find2 * This,
            /* [in][idldescattr] */ signed long dispidMember,
            /* [in][idldescattr] */ __RPC__in struct GUID *riid,
            /* [in][idldescattr] */ unsigned long lcid,
            /* [in][idldescattr] */ unsigned short wFlags,
            /* [in][idldescattr] */ __RPC__in struct DISPPARAMS *pdispparams,
            /* [out][idldescattr] */ __RPC__out VARIANT *pvarResult,
            /* [out][idldescattr] */ __RPC__out struct EXCEPINFO *pexcepinfo,
            /* [out][idldescattr] */ __RPC__out unsigned UINT *puArgErr,
            /* [retval][out] */ __RPC__out void *retval);
        
        /* [id][propget][funcdescattr] */ HRESULT ( STDMETHODCALLTYPE *get_DTE )( 
            Find2 * This,
            /* [retval][out] */ __RPC__deref_out_opt **retval);
        
        /* [id][propget][funcdescattr] */ HRESULT ( STDMETHODCALLTYPE *get_Parent )( 
            Find2 * This,
            /* [retval][out] */ __RPC__deref_out_opt **retval);
        
        /* [id][propget][funcdescattr] */ HRESULT ( STDMETHODCALLTYPE *get_Action )( 
            Find2 * This,
            /* [retval][out] */ __RPC__out enum vsFindAction *retval);
        
        /* [id][propput][funcdescattr] */ HRESULT ( STDMETHODCALLTYPE *put_Action )( 
            Find2 * This,
            /* [in][idldescattr] */ enum vsFindAction noname,
            /* [retval][out] */ __RPC__out void *retval);
        
        /* [id][propget][funcdescattr] */ HRESULT ( STDMETHODCALLTYPE *get_FindWhat )( 
            Find2 * This,
            /* [retval][out] */ __RPC__deref_out_opt BSTR *retval);
        
        /* [id][propput][funcdescattr] */ HRESULT ( STDMETHODCALLTYPE *put_FindWhat )( 
            Find2 * This,
            /* [in][idldescattr] */ __RPC__in BSTR noname,
            /* [retval][out] */ __RPC__out void *retval);
        
        /* [id][propget][funcdescattr] */ HRESULT ( STDMETHODCALLTYPE *get_MatchCase )( 
            Find2 * This,
            /* [retval][out] */ __RPC__out BOOLEAN *retval);
        
        /* [id][propput][funcdescattr] */ HRESULT ( STDMETHODCALLTYPE *put_MatchCase )( 
            Find2 * This,
            /* [in][idldescattr] */ BOOLEAN noname,
            /* [retval][out] */ __RPC__out void *retval);
        
        /* [id][propget][funcdescattr] */ HRESULT ( STDMETHODCALLTYPE *get_MatchWholeWord )( 
            Find2 * This,
            /* [retval][out] */ __RPC__out BOOLEAN *retval);
        
        /* [id][propput][funcdescattr] */ HRESULT ( STDMETHODCALLTYPE *put_MatchWholeWord )( 
            Find2 * This,
            /* [in][idldescattr] */ BOOLEAN noname,
            /* [retval][out] */ __RPC__out void *retval);
        
        /* [id][propget][funcdescattr] */ HRESULT ( STDMETHODCALLTYPE *get_MatchInHiddenText )( 
            Find2 * This,
            /* [retval][out] */ __RPC__out BOOLEAN *retval);
        
        /* [id][propput][funcdescattr] */ HRESULT ( STDMETHODCALLTYPE *put_MatchInHiddenText )( 
            Find2 * This,
            /* [in][idldescattr] */ BOOLEAN noname,
            /* [retval][out] */ __RPC__out void *retval);
        
        /* [id][propget][funcdescattr] */ HRESULT ( STDMETHODCALLTYPE *get_Backwards )( 
            Find2 * This,
            /* [retval][out] */ __RPC__out BOOLEAN *retval);
        
        /* [id][propput][funcdescattr] */ HRESULT ( STDMETHODCALLTYPE *put_Backwards )( 
            Find2 * This,
            /* [in][idldescattr] */ BOOLEAN noname,
            /* [retval][out] */ __RPC__out void *retval);
        
        /* [id][propget][funcdescattr] */ HRESULT ( STDMETHODCALLTYPE *get_SearchSubfolders )( 
            Find2 * This,
            /* [retval][out] */ __RPC__out BOOLEAN *retval);
        
        /* [id][propput][funcdescattr] */ HRESULT ( STDMETHODCALLTYPE *put_SearchSubfolders )( 
            Find2 * This,
            /* [in][idldescattr] */ BOOLEAN noname,
            /* [retval][out] */ __RPC__out void *retval);
        
        /* [id][propget][funcdescattr] */ HRESULT ( STDMETHODCALLTYPE *get_KeepModifiedDocumentsOpen )( 
            Find2 * This,
            /* [retval][out] */ __RPC__out BOOLEAN *retval);
        
        /* [id][propput][funcdescattr] */ HRESULT ( STDMETHODCALLTYPE *put_KeepModifiedDocumentsOpen )( 
            Find2 * This,
            /* [in][idldescattr] */ BOOLEAN noname,
            /* [retval][out] */ __RPC__out void *retval);
        
        /* [id][propget][funcdescattr] */ HRESULT ( STDMETHODCALLTYPE *get_PatternSyntax )( 
            Find2 * This,
            /* [retval][out] */ __RPC__out enum vsFindPatternSyntax *retval);
        
        /* [id][propput][funcdescattr] */ HRESULT ( STDMETHODCALLTYPE *put_PatternSyntax )( 
            Find2 * This,
            /* [in][idldescattr] */ enum vsFindPatternSyntax noname,
            /* [retval][out] */ __RPC__out void *retval);
        
        /* [id][propget][funcdescattr] */ HRESULT ( STDMETHODCALLTYPE *get_ReplaceWith )( 
            Find2 * This,
            /* [retval][out] */ __RPC__deref_out_opt BSTR *retval);
        
        /* [id][propput][funcdescattr] */ HRESULT ( STDMETHODCALLTYPE *put_ReplaceWith )( 
            Find2 * This,
            /* [in][idldescattr] */ __RPC__in BSTR noname,
            /* [retval][out] */ __RPC__out void *retval);
        
        /* [id][propget][funcdescattr] */ HRESULT ( STDMETHODCALLTYPE *get_Target )( 
            Find2 * This,
            /* [retval][out] */ __RPC__out enum vsFindTarget *retval);
        
        /* [id][propput][funcdescattr] */ HRESULT ( STDMETHODCALLTYPE *put_Target )( 
            Find2 * This,
            /* [in][idldescattr] */ enum vsFindTarget noname,
            /* [retval][out] */ __RPC__out void *retval);
        
        /* [id][propget][funcdescattr] */ HRESULT ( STDMETHODCALLTYPE *get_SearchPath )( 
            Find2 * This,
            /* [retval][out] */ __RPC__deref_out_opt BSTR *retval);
        
        /* [id][propput][funcdescattr] */ HRESULT ( STDMETHODCALLTYPE *put_SearchPath )( 
            Find2 * This,
            /* [in][idldescattr] */ __RPC__in BSTR noname,
            /* [retval][out] */ __RPC__out void *retval);
        
        /* [id][propget][funcdescattr] */ HRESULT ( STDMETHODCALLTYPE *get_FilesOfType )( 
            Find2 * This,
            /* [retval][out] */ __RPC__deref_out_opt BSTR *retval);
        
        /* [id][propput][funcdescattr] */ HRESULT ( STDMETHODCALLTYPE *put_FilesOfType )( 
            Find2 * This,
            /* [in][idldescattr] */ __RPC__in BSTR noname,
            /* [retval][out] */ __RPC__out void *retval);
        
        /* [id][propget][funcdescattr] */ HRESULT ( STDMETHODCALLTYPE *get_ResultsLocation )( 
            Find2 * This,
            /* [retval][out] */ __RPC__out enum vsFindResultsLocation *retval);
        
        /* [id][propput][funcdescattr] */ HRESULT ( STDMETHODCALLTYPE *put_ResultsLocation )( 
            Find2 * This,
            /* [in][idldescattr] */ enum vsFindResultsLocation noname,
            /* [retval][out] */ __RPC__out void *retval);
        
        /* [id][funcdescattr] */ HRESULT ( STDMETHODCALLTYPE *Execute )( 
            Find2 * This,
            /* [retval][out] */ __RPC__out enum vsFindResult *retval);
        
        /* [id][funcdescattr] */ HRESULT ( STDMETHODCALLTYPE *FindReplace )( 
            Find2 * This,
            /* [in][idldescattr] */ enum vsFindAction Action,
            /* [in][idldescattr] */ __RPC__in BSTR FindWhat,
            /* [in][idldescattr] */ signed long vsFindOptionsValue,
            /* [in][idldescattr] */ __RPC__in BSTR ReplaceWith,
            /* [in][idldescattr] */ enum vsFindTarget Target,
            /* [in][idldescattr] */ __RPC__in BSTR SearchPath,
            /* [in][idldescattr] */ __RPC__in BSTR FilesOfType,
            /* [in][idldescattr] */ enum vsFindResultsLocation ResultsLocation,
            /* [retval][out] */ __RPC__out enum vsFindResult *retval);
        
        /* [helpstringcontext][helpstring][helpcontext][propget][id] */ HRESULT ( STDMETHODCALLTYPE *get_WaitForFindToComplete )( 
            Find2 * This,
            /* [retval][out] */ __RPC__out VARIANT_BOOL *pWait);
        
        /* [helpstringcontext][helpstring][helpcontext][propput][id] */ HRESULT ( STDMETHODCALLTYPE *put_WaitForFindToComplete )( 
            Find2 * This,
            VARIANT_BOOL Wait);
        
        END_INTERFACE
    } Find2Vtbl;

    interface Find2
    {
        CONST_VTBL struct Find2Vtbl *lpVtbl;
    };

    

#ifdef COBJMACROS


#define Find2_QueryInterface(This,riid,ppvObj,retval)	\
    ( (This)->lpVtbl -> QueryInterface(This,riid,ppvObj,retval) ) 

#define Find2_AddRef(This,retval)	\
    ( (This)->lpVtbl -> AddRef(This,retval) ) 

#define Find2_Release(This,retval)	\
    ( (This)->lpVtbl -> Release(This,retval) ) 

#define Find2_GetTypeInfoCount(This,pctinfo,retval)	\
    ( (This)->lpVtbl -> GetTypeInfoCount(This,pctinfo,retval) ) 

#define Find2_GetTypeInfo(This,itinfo,lcid,pptinfo,retval)	\
    ( (This)->lpVtbl -> GetTypeInfo(This,itinfo,lcid,pptinfo,retval) ) 

#define Find2_GetIDsOfNames(This,riid,rgszNames,cNames,lcid,rgdispid,retval)	\
    ( (This)->lpVtbl -> GetIDsOfNames(This,riid,rgszNames,cNames,lcid,rgdispid,retval) ) 

#define Find2_Invoke(This,dispidMember,riid,lcid,wFlags,pdispparams,pvarResult,pexcepinfo,puArgErr,retval)	\
    ( (This)->lpVtbl -> Invoke(This,dispidMember,riid,lcid,wFlags,pdispparams,pvarResult,pexcepinfo,puArgErr,retval) ) 

#define Find2_get_DTE(This,retval)	\
    ( (This)->lpVtbl -> get_DTE(This,retval) ) 

#define Find2_get_Parent(This,retval)	\
    ( (This)->lpVtbl -> get_Parent(This,retval) ) 

#define Find2_get_Action(This,retval)	\
    ( (This)->lpVtbl -> get_Action(This,retval) ) 

#define Find2_put_Action(This,noname,retval)	\
    ( (This)->lpVtbl -> put_Action(This,noname,retval) ) 

#define Find2_get_FindWhat(This,retval)	\
    ( (This)->lpVtbl -> get_FindWhat(This,retval) ) 

#define Find2_put_FindWhat(This,noname,retval)	\
    ( (This)->lpVtbl -> put_FindWhat(This,noname,retval) ) 

#define Find2_get_MatchCase(This,retval)	\
    ( (This)->lpVtbl -> get_MatchCase(This,retval) ) 

#define Find2_put_MatchCase(This,noname,retval)	\
    ( (This)->lpVtbl -> put_MatchCase(This,noname,retval) ) 

#define Find2_get_MatchWholeWord(This,retval)	\
    ( (This)->lpVtbl -> get_MatchWholeWord(This,retval) ) 

#define Find2_put_MatchWholeWord(This,noname,retval)	\
    ( (This)->lpVtbl -> put_MatchWholeWord(This,noname,retval) ) 

#define Find2_get_MatchInHiddenText(This,retval)	\
    ( (This)->lpVtbl -> get_MatchInHiddenText(This,retval) ) 

#define Find2_put_MatchInHiddenText(This,noname,retval)	\
    ( (This)->lpVtbl -> put_MatchInHiddenText(This,noname,retval) ) 

#define Find2_get_Backwards(This,retval)	\
    ( (This)->lpVtbl -> get_Backwards(This,retval) ) 

#define Find2_put_Backwards(This,noname,retval)	\
    ( (This)->lpVtbl -> put_Backwards(This,noname,retval) ) 

#define Find2_get_SearchSubfolders(This,retval)	\
    ( (This)->lpVtbl -> get_SearchSubfolders(This,retval) ) 

#define Find2_put_SearchSubfolders(This,noname,retval)	\
    ( (This)->lpVtbl -> put_SearchSubfolders(This,noname,retval) ) 

#define Find2_get_KeepModifiedDocumentsOpen(This,retval)	\
    ( (This)->lpVtbl -> get_KeepModifiedDocumentsOpen(This,retval) ) 

#define Find2_put_KeepModifiedDocumentsOpen(This,noname,retval)	\
    ( (This)->lpVtbl -> put_KeepModifiedDocumentsOpen(This,noname,retval) ) 

#define Find2_get_PatternSyntax(This,retval)	\
    ( (This)->lpVtbl -> get_PatternSyntax(This,retval) ) 

#define Find2_put_PatternSyntax(This,noname,retval)	\
    ( (This)->lpVtbl -> put_PatternSyntax(This,noname,retval) ) 

#define Find2_get_ReplaceWith(This,retval)	\
    ( (This)->lpVtbl -> get_ReplaceWith(This,retval) ) 

#define Find2_put_ReplaceWith(This,noname,retval)	\
    ( (This)->lpVtbl -> put_ReplaceWith(This,noname,retval) ) 

#define Find2_get_Target(This,retval)	\
    ( (This)->lpVtbl -> get_Target(This,retval) ) 

#define Find2_put_Target(This,noname,retval)	\
    ( (This)->lpVtbl -> put_Target(This,noname,retval) ) 

#define Find2_get_SearchPath(This,retval)	\
    ( (This)->lpVtbl -> get_SearchPath(This,retval) ) 

#define Find2_put_SearchPath(This,noname,retval)	\
    ( (This)->lpVtbl -> put_SearchPath(This,noname,retval) ) 

#define Find2_get_FilesOfType(This,retval)	\
    ( (This)->lpVtbl -> get_FilesOfType(This,retval) ) 

#define Find2_put_FilesOfType(This,noname,retval)	\
    ( (This)->lpVtbl -> put_FilesOfType(This,noname,retval) ) 

#define Find2_get_ResultsLocation(This,retval)	\
    ( (This)->lpVtbl -> get_ResultsLocation(This,retval) ) 

#define Find2_put_ResultsLocation(This,noname,retval)	\
    ( (This)->lpVtbl -> put_ResultsLocation(This,noname,retval) ) 

#define Find2_Execute(This,retval)	\
    ( (This)->lpVtbl -> Execute(This,retval) ) 

#define Find2_FindReplace(This,Action,FindWhat,vsFindOptionsValue,ReplaceWith,Target,SearchPath,FilesOfType,ResultsLocation,retval)	\
    ( (This)->lpVtbl -> FindReplace(This,Action,FindWhat,vsFindOptionsValue,ReplaceWith,Target,SearchPath,FilesOfType,ResultsLocation,retval) ) 


#define Find2_get_WaitForFindToComplete(This,pWait)	\
    ( (This)->lpVtbl -> get_WaitForFindToComplete(This,pWait) ) 

#define Find2_put_WaitForFindToComplete(This,Wait)	\
    ( (This)->lpVtbl -> put_WaitForFindToComplete(This,Wait) ) 

#endif /* COBJMACROS */


#endif 	/* C style interface */




#endif 	/* __Find2_INTERFACE_DEFINED__ */


#ifndef __LifetimeInformation_INTERFACE_DEFINED__
#define __LifetimeInformation_INTERFACE_DEFINED__

/* interface LifetimeInformation */
/* [helpstringcontext][helpstring][helpcontext][uuid][object][oleautomation][dual] */ 


EXTERN_C const IID IID_LifetimeInformation;

#if defined(__cplusplus) && !defined(CINTERFACE)
    
    MIDL_INTERFACE("C8F4F9CC-B7E5-4458-BCE3-E1542468F26B")
    LifetimeInformation : public IDispatch
    {
    public:
        virtual /* [helpstringcontext][helpstring][helpcontext][propget][id] */ HRESULT STDMETHODCALLTYPE get_HasBeenDeleted( 
            /* [retval][out] */ __RPC__out VARIANT_BOOL *pDeleted) = 0;
        
    };
    
#else 	/* C style interface */

    typedef struct LifetimeInformationVtbl
    {
        BEGIN_INTERFACE
        
        HRESULT ( STDMETHODCALLTYPE *QueryInterface )( 
            LifetimeInformation * This,
            /* [in] */ __RPC__in REFIID riid,
            /* [iid_is][out] */ 
            __RPC__deref_out  void **ppvObject);
        
        ULONG ( STDMETHODCALLTYPE *AddRef )( 
            LifetimeInformation * This);
        
        ULONG ( STDMETHODCALLTYPE *Release )( 
            LifetimeInformation * This);
        
        HRESULT ( STDMETHODCALLTYPE *GetTypeInfoCount )( 
            LifetimeInformation * This,
            /* [out] */ __RPC__out UINT *pctinfo);
        
        HRESULT ( STDMETHODCALLTYPE *GetTypeInfo )( 
            LifetimeInformation * This,
            /* [in] */ UINT iTInfo,
            /* [in] */ LCID lcid,
            /* [out] */ __RPC__deref_out_opt ITypeInfo **ppTInfo);
        
        HRESULT ( STDMETHODCALLTYPE *GetIDsOfNames )( 
            LifetimeInformation * This,
            /* [in] */ __RPC__in REFIID riid,
            /* [size_is][in] */ __RPC__in_ecount_full(cNames) LPOLESTR *rgszNames,
            /* [range][in] */ UINT cNames,
            /* [in] */ LCID lcid,
            /* [size_is][out] */ __RPC__out_ecount_full(cNames) DISPID *rgDispId);
        
        /* [local] */ HRESULT ( STDMETHODCALLTYPE *Invoke )( 
            LifetimeInformation * This,
            /* [in] */ DISPID dispIdMember,
            /* [in] */ REFIID riid,
            /* [in] */ LCID lcid,
            /* [in] */ WORD wFlags,
            /* [out][in] */ DISPPARAMS *pDispParams,
            /* [out] */ VARIANT *pVarResult,
            /* [out] */ EXCEPINFO *pExcepInfo,
            /* [out] */ UINT *puArgErr);
        
        /* [helpstringcontext][helpstring][helpcontext][propget][id] */ HRESULT ( STDMETHODCALLTYPE *get_HasBeenDeleted )( 
            LifetimeInformation * This,
            /* [retval][out] */ __RPC__out VARIANT_BOOL *pDeleted);
        
        END_INTERFACE
    } LifetimeInformationVtbl;

    interface LifetimeInformation
    {
        CONST_VTBL struct LifetimeInformationVtbl *lpVtbl;
    };

    

#ifdef COBJMACROS


#define LifetimeInformation_QueryInterface(This,riid,ppvObject)	\
    ( (This)->lpVtbl -> QueryInterface(This,riid,ppvObject) ) 

#define LifetimeInformation_AddRef(This)	\
    ( (This)->lpVtbl -> AddRef(This) ) 

#define LifetimeInformation_Release(This)	\
    ( (This)->lpVtbl -> Release(This) ) 


#define LifetimeInformation_GetTypeInfoCount(This,pctinfo)	\
    ( (This)->lpVtbl -> GetTypeInfoCount(This,pctinfo) ) 

#define LifetimeInformation_GetTypeInfo(This,iTInfo,lcid,ppTInfo)	\
    ( (This)->lpVtbl -> GetTypeInfo(This,iTInfo,lcid,ppTInfo) ) 

#define LifetimeInformation_GetIDsOfNames(This,riid,rgszNames,cNames,lcid,rgDispId)	\
    ( (This)->lpVtbl -> GetIDsOfNames(This,riid,rgszNames,cNames,lcid,rgDispId) ) 

#define LifetimeInformation_Invoke(This,dispIdMember,riid,lcid,wFlags,pDispParams,pVarResult,pExcepInfo,puArgErr)	\
    ( (This)->lpVtbl -> Invoke(This,dispIdMember,riid,lcid,wFlags,pDispParams,pVarResult,pExcepInfo,puArgErr) ) 


#define LifetimeInformation_get_HasBeenDeleted(This,pDeleted)	\
    ( (This)->lpVtbl -> get_HasBeenDeleted(This,pDeleted) ) 

#endif /* COBJMACROS */


#endif 	/* C style interface */




#endif 	/* __LifetimeInformation_INTERFACE_DEFINED__ */


#ifndef __ToolBoxItem2_INTERFACE_DEFINED__
#define __ToolBoxItem2_INTERFACE_DEFINED__

/* interface ToolBoxItem2 */
/* [helpstringcontext][helpstring][helpcontext][uuid][object][oleautomation][dual] */ 


EXTERN_C const IID IID_ToolBoxItem2;

#if defined(__cplusplus) && !defined(CINTERFACE)
    
    MIDL_INTERFACE("82C9DBF2-1DA8-4ED6-A5EC-8B876B46317C")
    ToolBoxItem2 : public ToolBoxItem
    {
    public:
        virtual /* [helpstringcontext][helpstring][helpcontext][propget][id] */ HRESULT STDMETHODCALLTYPE get_Data( 
            /* [retval][out] */ __RPC__out VARIANT *pData) = 0;
        
    };
    
#else 	/* C style interface */

    typedef struct ToolBoxItem2Vtbl
    {
        BEGIN_INTERFACE
        
        /* [id][restricted][funcdescattr] */ HRESULT ( STDMETHODCALLTYPE *QueryInterface )( 
            ToolBoxItem2 * This,
            /* [in][idldescattr] */ __RPC__in struct GUID *riid,
            /* [out][idldescattr] */ __RPC__deref_out_opt void **ppvObj,
            /* [retval][out] */ __RPC__out void *retval);
        
        /* [id][restricted][funcdescattr] */ HRESULT ( STDMETHODCALLTYPE *AddRef )( 
            ToolBoxItem2 * This,
            /* [retval][out] */ __RPC__out unsigned long *retval);
        
        /* [id][restricted][funcdescattr] */ HRESULT ( STDMETHODCALLTYPE *Release )( 
            ToolBoxItem2 * This,
            /* [retval][out] */ __RPC__out unsigned long *retval);
        
        /* [id][restricted][funcdescattr] */ HRESULT ( STDMETHODCALLTYPE *GetTypeInfoCount )( 
            ToolBoxItem2 * This,
            /* [out][idldescattr] */ __RPC__out unsigned UINT *pctinfo,
            /* [retval][out] */ __RPC__out void *retval);
        
        /* [id][restricted][funcdescattr] */ HRESULT ( STDMETHODCALLTYPE *GetTypeInfo )( 
            ToolBoxItem2 * This,
            /* [in][idldescattr] */ unsigned UINT itinfo,
            /* [in][idldescattr] */ unsigned long lcid,
            /* [out][idldescattr] */ __RPC__deref_out_opt void **pptinfo,
            /* [retval][out] */ __RPC__out void *retval);
        
        /* [id][restricted][funcdescattr] */ HRESULT ( STDMETHODCALLTYPE *GetIDsOfNames )( 
            ToolBoxItem2 * This,
            /* [in][idldescattr] */ __RPC__in struct GUID *riid,
            /* [in][idldescattr] */ __RPC__deref_in_opt signed char **rgszNames,
            /* [in][idldescattr] */ unsigned UINT cNames,
            /* [in][idldescattr] */ unsigned long lcid,
            /* [out][idldescattr] */ __RPC__out signed long *rgdispid,
            /* [retval][out] */ __RPC__out void *retval);
        
        /* [id][restricted][funcdescattr] */ HRESULT ( STDMETHODCALLTYPE *Invoke )( 
            ToolBoxItem2 * This,
            /* [in][idldescattr] */ signed long dispidMember,
            /* [in][idldescattr] */ __RPC__in struct GUID *riid,
            /* [in][idldescattr] */ unsigned long lcid,
            /* [in][idldescattr] */ unsigned short wFlags,
            /* [in][idldescattr] */ __RPC__in struct DISPPARAMS *pdispparams,
            /* [out][idldescattr] */ __RPC__out VARIANT *pvarResult,
            /* [out][idldescattr] */ __RPC__out struct EXCEPINFO *pexcepinfo,
            /* [out][idldescattr] */ __RPC__out unsigned UINT *puArgErr,
            /* [retval][out] */ __RPC__out void *retval);
        
        /* [id][propget][funcdescattr] */ HRESULT ( STDMETHODCALLTYPE *get_Name )( 
            ToolBoxItem2 * This,
            /* [retval][out] */ __RPC__deref_out_opt BSTR *retval);
        
        /* [id][propput][funcdescattr] */ HRESULT ( STDMETHODCALLTYPE *put_Name )( 
            ToolBoxItem2 * This,
            /* [in][idldescattr] */ __RPC__in BSTR noname,
            /* [retval][out] */ __RPC__out void *retval);
        
        /* [id][propget][funcdescattr] */ HRESULT ( STDMETHODCALLTYPE *get_Collection )( 
            ToolBoxItem2 * This,
            /* [retval][out] */ __RPC__deref_out_opt ToolBoxItems **retval);
        
        /* [id][propget][funcdescattr] */ HRESULT ( STDMETHODCALLTYPE *get_DTE )( 
            ToolBoxItem2 * This,
            /* [retval][out] */ __RPC__deref_out_opt **retval);
        
        /* [id][funcdescattr] */ HRESULT ( STDMETHODCALLTYPE *Delete )( 
            ToolBoxItem2 * This,
            /* [retval][out] */ __RPC__out void *retval);
        
        /* [id][funcdescattr] */ HRESULT ( STDMETHODCALLTYPE *Select )( 
            ToolBoxItem2 * This,
            /* [retval][out] */ __RPC__out void *retval);
        
        /* [helpstringcontext][helpstring][helpcontext][propget][id] */ HRESULT ( STDMETHODCALLTYPE *get_Data )( 
            ToolBoxItem2 * This,
            /* [retval][out] */ __RPC__out VARIANT *pData);
        
        END_INTERFACE
    } ToolBoxItem2Vtbl;

    interface ToolBoxItem2
    {
        CONST_VTBL struct ToolBoxItem2Vtbl *lpVtbl;
    };

    

#ifdef COBJMACROS


#define ToolBoxItem2_QueryInterface(This,riid,ppvObj,retval)	\
    ( (This)->lpVtbl -> QueryInterface(This,riid,ppvObj,retval) ) 

#define ToolBoxItem2_AddRef(This,retval)	\
    ( (This)->lpVtbl -> AddRef(This,retval) ) 

#define ToolBoxItem2_Release(This,retval)	\
    ( (This)->lpVtbl -> Release(This,retval) ) 

#define ToolBoxItem2_GetTypeInfoCount(This,pctinfo,retval)	\
    ( (This)->lpVtbl -> GetTypeInfoCount(This,pctinfo,retval) ) 

#define ToolBoxItem2_GetTypeInfo(This,itinfo,lcid,pptinfo,retval)	\
    ( (This)->lpVtbl -> GetTypeInfo(This,itinfo,lcid,pptinfo,retval) ) 

#define ToolBoxItem2_GetIDsOfNames(This,riid,rgszNames,cNames,lcid,rgdispid,retval)	\
    ( (This)->lpVtbl -> GetIDsOfNames(This,riid,rgszNames,cNames,lcid,rgdispid,retval) ) 

#define ToolBoxItem2_Invoke(This,dispidMember,riid,lcid,wFlags,pdispparams,pvarResult,pexcepinfo,puArgErr,retval)	\
    ( (This)->lpVtbl -> Invoke(This,dispidMember,riid,lcid,wFlags,pdispparams,pvarResult,pexcepinfo,puArgErr,retval) ) 

#define ToolBoxItem2_get_Name(This,retval)	\
    ( (This)->lpVtbl -> get_Name(This,retval) ) 

#define ToolBoxItem2_put_Name(This,noname,retval)	\
    ( (This)->lpVtbl -> put_Name(This,noname,retval) ) 

#define ToolBoxItem2_get_Collection(This,retval)	\
    ( (This)->lpVtbl -> get_Collection(This,retval) ) 

#define ToolBoxItem2_get_DTE(This,retval)	\
    ( (This)->lpVtbl -> get_DTE(This,retval) ) 

#define ToolBoxItem2_Delete(This,retval)	\
    ( (This)->lpVtbl -> Delete(This,retval) ) 

#define ToolBoxItem2_Select(This,retval)	\
    ( (This)->lpVtbl -> Select(This,retval) ) 


#define ToolBoxItem2_get_Data(This,pData)	\
    ( (This)->lpVtbl -> get_Data(This,pData) ) 

#endif /* COBJMACROS */


#endif 	/* C style interface */




#endif 	/* __ToolBoxItem2_INTERFACE_DEFINED__ */


#ifndef __ToolBoxTab2_INTERFACE_DEFINED__
#define __ToolBoxTab2_INTERFACE_DEFINED__

/* interface ToolBoxTab2 */
/* [helpstringcontext][helpstring][helpcontext][uuid][object][oleautomation][dual] */ 


EXTERN_C const IID IID_ToolBoxTab2;

#if defined(__cplusplus) && !defined(CINTERFACE)
    
    MIDL_INTERFACE("A64715CB-85D7-41c3-8E71-2302D4EEBC34")
    ToolBoxTab2 : public ToolBoxTab
    {
    public:
        virtual /* [helpstringcontext][helpstring][helpcontext][propget][id] */ HRESULT STDMETHODCALLTYPE get_UniqueID( 
            /* [retval][out] */ __RPC__deref_out_opt BSTR *pbstrID) = 0;
        
        virtual /* [helpstringcontext][helpstring][helpcontext][propput][id] */ HRESULT STDMETHODCALLTYPE put_UniqueID( 
            /* [in] */ __RPC__in BSTR bstrID) = 0;
        
    };
    
#else 	/* C style interface */

    typedef struct ToolBoxTab2Vtbl
    {
        BEGIN_INTERFACE
        
        /* [id][restricted][funcdescattr] */ HRESULT ( STDMETHODCALLTYPE *QueryInterface )( 
            ToolBoxTab2 * This,
            /* [in][idldescattr] */ __RPC__in struct GUID *riid,
            /* [out][idldescattr] */ __RPC__deref_out_opt void **ppvObj,
            /* [retval][out] */ __RPC__out void *retval);
        
        /* [id][restricted][funcdescattr] */ HRESULT ( STDMETHODCALLTYPE *AddRef )( 
            ToolBoxTab2 * This,
            /* [retval][out] */ __RPC__out unsigned long *retval);
        
        /* [id][restricted][funcdescattr] */ HRESULT ( STDMETHODCALLTYPE *Release )( 
            ToolBoxTab2 * This,
            /* [retval][out] */ __RPC__out unsigned long *retval);
        
        /* [id][restricted][funcdescattr] */ HRESULT ( STDMETHODCALLTYPE *GetTypeInfoCount )( 
            ToolBoxTab2 * This,
            /* [out][idldescattr] */ __RPC__out unsigned UINT *pctinfo,
            /* [retval][out] */ __RPC__out void *retval);
        
        /* [id][restricted][funcdescattr] */ HRESULT ( STDMETHODCALLTYPE *GetTypeInfo )( 
            ToolBoxTab2 * This,
            /* [in][idldescattr] */ unsigned UINT itinfo,
            /* [in][idldescattr] */ unsigned long lcid,
            /* [out][idldescattr] */ __RPC__deref_out_opt void **pptinfo,
            /* [retval][out] */ __RPC__out void *retval);
        
        /* [id][restricted][funcdescattr] */ HRESULT ( STDMETHODCALLTYPE *GetIDsOfNames )( 
            ToolBoxTab2 * This,
            /* [in][idldescattr] */ __RPC__in struct GUID *riid,
            /* [in][idldescattr] */ __RPC__deref_in_opt signed char **rgszNames,
            /* [in][idldescattr] */ unsigned UINT cNames,
            /* [in][idldescattr] */ unsigned long lcid,
            /* [out][idldescattr] */ __RPC__out signed long *rgdispid,
            /* [retval][out] */ __RPC__out void *retval);
        
        /* [id][restricted][funcdescattr] */ HRESULT ( STDMETHODCALLTYPE *Invoke )( 
            ToolBoxTab2 * This,
            /* [in][idldescattr] */ signed long dispidMember,
            /* [in][idldescattr] */ __RPC__in struct GUID *riid,
            /* [in][idldescattr] */ unsigned long lcid,
            /* [in][idldescattr] */ unsigned short wFlags,
            /* [in][idldescattr] */ __RPC__in struct DISPPARAMS *pdispparams,
            /* [out][idldescattr] */ __RPC__out VARIANT *pvarResult,
            /* [out][idldescattr] */ __RPC__out struct EXCEPINFO *pexcepinfo,
            /* [out][idldescattr] */ __RPC__out unsigned UINT *puArgErr,
            /* [retval][out] */ __RPC__out void *retval);
        
        /* [id][propget][funcdescattr] */ HRESULT ( STDMETHODCALLTYPE *get_Name )( 
            ToolBoxTab2 * This,
            /* [retval][out] */ __RPC__deref_out_opt BSTR *retval);
        
        /* [id][propput][funcdescattr] */ HRESULT ( STDMETHODCALLTYPE *put_Name )( 
            ToolBoxTab2 * This,
            /* [in][idldescattr] */ __RPC__in BSTR noname,
            /* [retval][out] */ __RPC__out void *retval);
        
        /* [id][propget][funcdescattr] */ HRESULT ( STDMETHODCALLTYPE *get_Collection )( 
            ToolBoxTab2 * This,
            /* [retval][out] */ __RPC__deref_out_opt ToolBoxTabs **retval);
        
        /* [id][propget][funcdescattr] */ HRESULT ( STDMETHODCALLTYPE *get_DTE )( 
            ToolBoxTab2 * This,
            /* [retval][out] */ __RPC__deref_out_opt **retval);
        
        /* [id][funcdescattr] */ HRESULT ( STDMETHODCALLTYPE *Activate )( 
            ToolBoxTab2 * This,
            /* [retval][out] */ __RPC__out void *retval);
        
        /* [id][funcdescattr] */ HRESULT ( STDMETHODCALLTYPE *Delete )( 
            ToolBoxTab2 * This,
            /* [retval][out] */ __RPC__out void *retval);
        
        /* [id][propget][funcdescattr] */ HRESULT ( STDMETHODCALLTYPE *get_ToolBoxItems )( 
            ToolBoxTab2 * This,
            /* [retval][out] */ __RPC__deref_out_opt ToolBoxItems **retval);
        
        /* [id][propget][funcdescattr] */ HRESULT ( STDMETHODCALLTYPE *get_ListView )( 
            ToolBoxTab2 * This,
            /* [retval][out] */ __RPC__out BOOLEAN *retval);
        
        /* [id][propput][funcdescattr] */ HRESULT ( STDMETHODCALLTYPE *put_ListView )( 
            ToolBoxTab2 * This,
            /* [in][idldescattr] */ BOOLEAN noname,
            /* [retval][out] */ __RPC__out void *retval);
        
        /* [helpstringcontext][helpstring][helpcontext][propget][id] */ HRESULT ( STDMETHODCALLTYPE *get_UniqueID )( 
            ToolBoxTab2 * This,
            /* [retval][out] */ __RPC__deref_out_opt BSTR *pbstrID);
        
        /* [helpstringcontext][helpstring][helpcontext][propput][id] */ HRESULT ( STDMETHODCALLTYPE *put_UniqueID )( 
            ToolBoxTab2 * This,
            /* [in] */ __RPC__in BSTR bstrID);
        
        END_INTERFACE
    } ToolBoxTab2Vtbl;

    interface ToolBoxTab2
    {
        CONST_VTBL struct ToolBoxTab2Vtbl *lpVtbl;
    };

    

#ifdef COBJMACROS


#define ToolBoxTab2_QueryInterface(This,riid,ppvObj,retval)	\
    ( (This)->lpVtbl -> QueryInterface(This,riid,ppvObj,retval) ) 

#define ToolBoxTab2_AddRef(This,retval)	\
    ( (This)->lpVtbl -> AddRef(This,retval) ) 

#define ToolBoxTab2_Release(This,retval)	\
    ( (This)->lpVtbl -> Release(This,retval) ) 

#define ToolBoxTab2_GetTypeInfoCount(This,pctinfo,retval)	\
    ( (This)->lpVtbl -> GetTypeInfoCount(This,pctinfo,retval) ) 

#define ToolBoxTab2_GetTypeInfo(This,itinfo,lcid,pptinfo,retval)	\
    ( (This)->lpVtbl -> GetTypeInfo(This,itinfo,lcid,pptinfo,retval) ) 

#define ToolBoxTab2_GetIDsOfNames(This,riid,rgszNames,cNames,lcid,rgdispid,retval)	\
    ( (This)->lpVtbl -> GetIDsOfNames(This,riid,rgszNames,cNames,lcid,rgdispid,retval) ) 

#define ToolBoxTab2_Invoke(This,dispidMember,riid,lcid,wFlags,pdispparams,pvarResult,pexcepinfo,puArgErr,retval)	\
    ( (This)->lpVtbl -> Invoke(This,dispidMember,riid,lcid,wFlags,pdispparams,pvarResult,pexcepinfo,puArgErr,retval) ) 

#define ToolBoxTab2_get_Name(This,retval)	\
    ( (This)->lpVtbl -> get_Name(This,retval) ) 

#define ToolBoxTab2_put_Name(This,noname,retval)	\
    ( (This)->lpVtbl -> put_Name(This,noname,retval) ) 

#define ToolBoxTab2_get_Collection(This,retval)	\
    ( (This)->lpVtbl -> get_Collection(This,retval) ) 

#define ToolBoxTab2_get_DTE(This,retval)	\
    ( (This)->lpVtbl -> get_DTE(This,retval) ) 

#define ToolBoxTab2_Activate(This,retval)	\
    ( (This)->lpVtbl -> Activate(This,retval) ) 

#define ToolBoxTab2_Delete(This,retval)	\
    ( (This)->lpVtbl -> Delete(This,retval) ) 

#define ToolBoxTab2_get_ToolBoxItems(This,retval)	\
    ( (This)->lpVtbl -> get_ToolBoxItems(This,retval) ) 

#define ToolBoxTab2_get_ListView(This,retval)	\
    ( (This)->lpVtbl -> get_ListView(This,retval) ) 

#define ToolBoxTab2_put_ListView(This,noname,retval)	\
    ( (This)->lpVtbl -> put_ListView(This,noname,retval) ) 


#define ToolBoxTab2_get_UniqueID(This,pbstrID)	\
    ( (This)->lpVtbl -> get_UniqueID(This,pbstrID) ) 

#define ToolBoxTab2_put_UniqueID(This,bstrID)	\
    ( (This)->lpVtbl -> put_UniqueID(This,bstrID) ) 

#endif /* COBJMACROS */


#endif 	/* C style interface */




#endif 	/* __ToolBoxTab2_INTERFACE_DEFINED__ */


#ifndef __IncrementalSearch_INTERFACE_DEFINED__
#define __IncrementalSearch_INTERFACE_DEFINED__

/* interface IncrementalSearch */
/* [helpstringcontext][helpstring][helpcontext][uuid][object][oleautomation][dual] */ 


EXTERN_C const IID IID_IncrementalSearch;

#if defined(__cplusplus) && !defined(CINTERFACE)
    
    MIDL_INTERFACE("C5BEE6D8-ED45-4317-96BF-97EB88EA3A07")
    IncrementalSearch : public IDispatch
    {
    public:
        virtual /* [helpstringcontext][helpstring][helpcontext][propget][id] */ HRESULT STDMETHODCALLTYPE get_DTE( 
            /* [retval][out] */ __RPC__deref_out_opt /* external definition not present */ DTE **DTEObject) = 0;
        
        virtual /* [helpstringcontext][helpstring][helpcontext][id] */ HRESULT STDMETHODCALLTYPE StartForward( void) = 0;
        
        virtual /* [helpstringcontext][helpstring][helpcontext][id] */ HRESULT STDMETHODCALLTYPE StartBackward( void) = 0;
        
        virtual /* [helpstringcontext][helpstring][helpcontext][propget][id] */ HRESULT STDMETHODCALLTYPE get_IncrementalSearchModeOn( 
            /* [retval][out] */ __RPC__out VARIANT_BOOL *pOn) = 0;
        
        virtual /* [helpstringcontext][helpstring][helpcontext][id] */ HRESULT STDMETHODCALLTYPE SearchWithLastPattern( 
            /* [retval][out] */ __RPC__out vsIncrementalSearchResult *pResult) = 0;
        
        virtual /* [helpstringcontext][helpstring][helpcontext][propget][id] */ HRESULT STDMETHODCALLTYPE get_Pattern( 
            /* [retval][out] */ __RPC__deref_out_opt BSTR *pPattern) = 0;
        
        virtual /* [helpstringcontext][helpstring][helpcontext][id] */ HRESULT STDMETHODCALLTYPE AppendCharAndSearch( 
            short Character,
            /* [retval][out] */ __RPC__out vsIncrementalSearchResult *pResult) = 0;
        
        virtual /* [helpstringcontext][helpstring][helpcontext][id] */ HRESULT STDMETHODCALLTYPE DeleteCharAndBackup( 
            /* [retval][out] */ __RPC__out vsIncrementalSearchResult *pResult) = 0;
        
        virtual /* [helpstringcontext][helpstring][helpcontext][id] */ HRESULT STDMETHODCALLTYPE Exit( void) = 0;
        
        virtual /* [helpstringcontext][helpstring][helpcontext][id] */ HRESULT STDMETHODCALLTYPE SearchForward( 
            /* [retval][out] */ __RPC__out vsIncrementalSearchResult *pResult) = 0;
        
        virtual /* [helpstringcontext][helpstring][helpcontext][id] */ HRESULT STDMETHODCALLTYPE SearchBackward( 
            /* [retval][out] */ __RPC__out vsIncrementalSearchResult *pResult) = 0;
        
    };
    
#else 	/* C style interface */

    typedef struct IncrementalSearchVtbl
    {
        BEGIN_INTERFACE
        
        HRESULT ( STDMETHODCALLTYPE *QueryInterface )( 
            IncrementalSearch * This,
            /* [in] */ __RPC__in REFIID riid,
            /* [iid_is][out] */ 
            __RPC__deref_out  void **ppvObject);
        
        ULONG ( STDMETHODCALLTYPE *AddRef )( 
            IncrementalSearch * This);
        
        ULONG ( STDMETHODCALLTYPE *Release )( 
            IncrementalSearch * This);
        
        HRESULT ( STDMETHODCALLTYPE *GetTypeInfoCount )( 
            IncrementalSearch * This,
            /* [out] */ __RPC__out UINT *pctinfo);
        
        HRESULT ( STDMETHODCALLTYPE *GetTypeInfo )( 
            IncrementalSearch * This,
            /* [in] */ UINT iTInfo,
            /* [in] */ LCID lcid,
            /* [out] */ __RPC__deref_out_opt ITypeInfo **ppTInfo);
        
        HRESULT ( STDMETHODCALLTYPE *GetIDsOfNames )( 
            IncrementalSearch * This,
            /* [in] */ __RPC__in REFIID riid,
            /* [size_is][in] */ __RPC__in_ecount_full(cNames) LPOLESTR *rgszNames,
            /* [range][in] */ UINT cNames,
            /* [in] */ LCID lcid,
            /* [size_is][out] */ __RPC__out_ecount_full(cNames) DISPID *rgDispId);
        
        /* [local] */ HRESULT ( STDMETHODCALLTYPE *Invoke )( 
            IncrementalSearch * This,
            /* [in] */ DISPID dispIdMember,
            /* [in] */ REFIID riid,
            /* [in] */ LCID lcid,
            /* [in] */ WORD wFlags,
            /* [out][in] */ DISPPARAMS *pDispParams,
            /* [out] */ VARIANT *pVarResult,
            /* [out] */ EXCEPINFO *pExcepInfo,
            /* [out] */ UINT *puArgErr);
        
        /* [helpstringcontext][helpstring][helpcontext][propget][id] */ HRESULT ( STDMETHODCALLTYPE *get_DTE )( 
            IncrementalSearch * This,
            /* [retval][out] */ __RPC__deref_out_opt /* external definition not present */ DTE **DTEObject);
        
        /* [helpstringcontext][helpstring][helpcontext][id] */ HRESULT ( STDMETHODCALLTYPE *StartForward )( 
            IncrementalSearch * This);
        
        /* [helpstringcontext][helpstring][helpcontext][id] */ HRESULT ( STDMETHODCALLTYPE *StartBackward )( 
            IncrementalSearch * This);
        
        /* [helpstringcontext][helpstring][helpcontext][propget][id] */ HRESULT ( STDMETHODCALLTYPE *get_IncrementalSearchModeOn )( 
            IncrementalSearch * This,
            /* [retval][out] */ __RPC__out VARIANT_BOOL *pOn);
        
        /* [helpstringcontext][helpstring][helpcontext][id] */ HRESULT ( STDMETHODCALLTYPE *SearchWithLastPattern )( 
            IncrementalSearch * This,
            /* [retval][out] */ __RPC__out vsIncrementalSearchResult *pResult);
        
        /* [helpstringcontext][helpstring][helpcontext][propget][id] */ HRESULT ( STDMETHODCALLTYPE *get_Pattern )( 
            IncrementalSearch * This,
            /* [retval][out] */ __RPC__deref_out_opt BSTR *pPattern);
        
        /* [helpstringcontext][helpstring][helpcontext][id] */ HRESULT ( STDMETHODCALLTYPE *AppendCharAndSearch )( 
            IncrementalSearch * This,
            short Character,
            /* [retval][out] */ __RPC__out vsIncrementalSearchResult *pResult);
        
        /* [helpstringcontext][helpstring][helpcontext][id] */ HRESULT ( STDMETHODCALLTYPE *DeleteCharAndBackup )( 
            IncrementalSearch * This,
            /* [retval][out] */ __RPC__out vsIncrementalSearchResult *pResult);
        
        /* [helpstringcontext][helpstring][helpcontext][id] */ HRESULT ( STDMETHODCALLTYPE *Exit )( 
            IncrementalSearch * This);
        
        /* [helpstringcontext][helpstring][helpcontext][id] */ HRESULT ( STDMETHODCALLTYPE *SearchForward )( 
            IncrementalSearch * This,
            /* [retval][out] */ __RPC__out vsIncrementalSearchResult *pResult);
        
        /* [helpstringcontext][helpstring][helpcontext][id] */ HRESULT ( STDMETHODCALLTYPE *SearchBackward )( 
            IncrementalSearch * This,
            /* [retval][out] */ __RPC__out vsIncrementalSearchResult *pResult);
        
        END_INTERFACE
    } IncrementalSearchVtbl;

    interface IncrementalSearch
    {
        CONST_VTBL struct IncrementalSearchVtbl *lpVtbl;
    };

    

#ifdef COBJMACROS


#define IncrementalSearch_QueryInterface(This,riid,ppvObject)	\
    ( (This)->lpVtbl -> QueryInterface(This,riid,ppvObject) ) 

#define IncrementalSearch_AddRef(This)	\
    ( (This)->lpVtbl -> AddRef(This) ) 

#define IncrementalSearch_Release(This)	\
    ( (This)->lpVtbl -> Release(This) ) 


#define IncrementalSearch_GetTypeInfoCount(This,pctinfo)	\
    ( (This)->lpVtbl -> GetTypeInfoCount(This,pctinfo) ) 

#define IncrementalSearch_GetTypeInfo(This,iTInfo,lcid,ppTInfo)	\
    ( (This)->lpVtbl -> GetTypeInfo(This,iTInfo,lcid,ppTInfo) ) 

#define IncrementalSearch_GetIDsOfNames(This,riid,rgszNames,cNames,lcid,rgDispId)	\
    ( (This)->lpVtbl -> GetIDsOfNames(This,riid,rgszNames,cNames,lcid,rgDispId) ) 

#define IncrementalSearch_Invoke(This,dispIdMember,riid,lcid,wFlags,pDispParams,pVarResult,pExcepInfo,puArgErr)	\
    ( (This)->lpVtbl -> Invoke(This,dispIdMember,riid,lcid,wFlags,pDispParams,pVarResult,pExcepInfo,puArgErr) ) 


#define IncrementalSearch_get_DTE(This,DTEObject)	\
    ( (This)->lpVtbl -> get_DTE(This,DTEObject) ) 

#define IncrementalSearch_StartForward(This)	\
    ( (This)->lpVtbl -> StartForward(This) ) 

#define IncrementalSearch_StartBackward(This)	\
    ( (This)->lpVtbl -> StartBackward(This) ) 

#define IncrementalSearch_get_IncrementalSearchModeOn(This,pOn)	\
    ( (This)->lpVtbl -> get_IncrementalSearchModeOn(This,pOn) ) 

#define IncrementalSearch_SearchWithLastPattern(This,pResult)	\
    ( (This)->lpVtbl -> SearchWithLastPattern(This,pResult) ) 

#define IncrementalSearch_get_Pattern(This,pPattern)	\
    ( (This)->lpVtbl -> get_Pattern(This,pPattern) ) 

#define IncrementalSearch_AppendCharAndSearch(This,Character,pResult)	\
    ( (This)->lpVtbl -> AppendCharAndSearch(This,Character,pResult) ) 

#define IncrementalSearch_DeleteCharAndBackup(This,pResult)	\
    ( (This)->lpVtbl -> DeleteCharAndBackup(This,pResult) ) 

#define IncrementalSearch_Exit(This)	\
    ( (This)->lpVtbl -> Exit(This) ) 

#define IncrementalSearch_SearchForward(This,pResult)	\
    ( (This)->lpVtbl -> SearchForward(This,pResult) ) 

#define IncrementalSearch_SearchBackward(This,pResult)	\
    ( (This)->lpVtbl -> SearchBackward(This,pResult) ) 

#endif /* COBJMACROS */


#endif 	/* C style interface */




#endif 	/* __IncrementalSearch_INTERFACE_DEFINED__ */


#ifndef __TextPane2_INTERFACE_DEFINED__
#define __TextPane2_INTERFACE_DEFINED__

/* interface TextPane2 */
/* [helpstringcontext][helpstring][helpcontext][uuid][object][oleautomation][dual] */ 


EXTERN_C const IID IID_TextPane2;

#if defined(__cplusplus) && !defined(CINTERFACE)
    
    MIDL_INTERFACE("ACE19C7B-A0AC-4089-94FD-749CF4380E1F")
    TextPane2 : public TextPane
    {
    public:
        virtual /* [helpstringcontext][helpstring][helpcontext][propget][id] */ HRESULT STDMETHODCALLTYPE get_IncrementalSearch( 
            /* [retval][out] */ __RPC__deref_out_opt IncrementalSearch **ppIncrementalSearch) = 0;
        
    };
    
#else 	/* C style interface */

    typedef struct TextPane2Vtbl
    {
        BEGIN_INTERFACE
        
        /* [id][restricted][funcdescattr] */ HRESULT ( STDMETHODCALLTYPE *QueryInterface )( 
            TextPane2 * This,
            /* [in][idldescattr] */ __RPC__in struct GUID *riid,
            /* [out][idldescattr] */ __RPC__deref_out_opt void **ppvObj,
            /* [retval][out] */ __RPC__out void *retval);
        
        /* [id][restricted][funcdescattr] */ HRESULT ( STDMETHODCALLTYPE *AddRef )( 
            TextPane2 * This,
            /* [retval][out] */ __RPC__out unsigned long *retval);
        
        /* [id][restricted][funcdescattr] */ HRESULT ( STDMETHODCALLTYPE *Release )( 
            TextPane2 * This,
            /* [retval][out] */ __RPC__out unsigned long *retval);
        
        /* [id][restricted][funcdescattr] */ HRESULT ( STDMETHODCALLTYPE *GetTypeInfoCount )( 
            TextPane2 * This,
            /* [out][idldescattr] */ __RPC__out unsigned UINT *pctinfo,
            /* [retval][out] */ __RPC__out void *retval);
        
        /* [id][restricted][funcdescattr] */ HRESULT ( STDMETHODCALLTYPE *GetTypeInfo )( 
            TextPane2 * This,
            /* [in][idldescattr] */ unsigned UINT itinfo,
            /* [in][idldescattr] */ unsigned long lcid,
            /* [out][idldescattr] */ __RPC__deref_out_opt void **pptinfo,
            /* [retval][out] */ __RPC__out void *retval);
        
        /* [id][restricted][funcdescattr] */ HRESULT ( STDMETHODCALLTYPE *GetIDsOfNames )( 
            TextPane2 * This,
            /* [in][idldescattr] */ __RPC__in struct GUID *riid,
            /* [in][idldescattr] */ __RPC__deref_in_opt signed char **rgszNames,
            /* [in][idldescattr] */ unsigned UINT cNames,
            /* [in][idldescattr] */ unsigned long lcid,
            /* [out][idldescattr] */ __RPC__out signed long *rgdispid,
            /* [retval][out] */ __RPC__out void *retval);
        
        /* [id][restricted][funcdescattr] */ HRESULT ( STDMETHODCALLTYPE *Invoke )( 
            TextPane2 * This,
            /* [in][idldescattr] */ signed long dispidMember,
            /* [in][idldescattr] */ __RPC__in struct GUID *riid,
            /* [in][idldescattr] */ unsigned long lcid,
            /* [in][idldescattr] */ unsigned short wFlags,
            /* [in][idldescattr] */ __RPC__in struct DISPPARAMS *pdispparams,
            /* [out][idldescattr] */ __RPC__out VARIANT *pvarResult,
            /* [out][idldescattr] */ __RPC__out struct EXCEPINFO *pexcepinfo,
            /* [out][idldescattr] */ __RPC__out unsigned UINT *puArgErr,
            /* [retval][out] */ __RPC__out void *retval);
        
        /* [id][propget][funcdescattr] */ HRESULT ( STDMETHODCALLTYPE *get_DTE )( 
            TextPane2 * This,
            /* [retval][out] */ __RPC__deref_out_opt **retval);
        
        /* [id][propget][funcdescattr] */ HRESULT ( STDMETHODCALLTYPE *get_Collection )( 
            TextPane2 * This,
            /* [retval][out] */ __RPC__deref_out_opt TextPanes **retval);
        
        /* [id][propget][funcdescattr] */ HRESULT ( STDMETHODCALLTYPE *get_Window )( 
            TextPane2 * This,
            /* [retval][out] */ __RPC__deref_out_opt Window **retval);
        
        /* [id][propget][funcdescattr] */ HRESULT ( STDMETHODCALLTYPE *get_Height )( 
            TextPane2 * This,
            /* [retval][out] */ __RPC__out signed long *retval);
        
        /* [id][propget][funcdescattr] */ HRESULT ( STDMETHODCALLTYPE *get_Width )( 
            TextPane2 * This,
            /* [retval][out] */ __RPC__out signed long *retval);
        
        /* [id][propget][funcdescattr] */ HRESULT ( STDMETHODCALLTYPE *get_Selection )( 
            TextPane2 * This,
            /* [retval][out] */ __RPC__deref_out_opt TextSelection **retval);
        
        /* [id][propget][funcdescattr] */ HRESULT ( STDMETHODCALLTYPE *get_StartPoint )( 
            TextPane2 * This,
            /* [retval][out] */ __RPC__deref_out_opt TextPoint **retval);
        
        /* [id][funcdescattr] */ HRESULT ( STDMETHODCALLTYPE *Activate )( 
            TextPane2 * This,
            /* [retval][out] */ __RPC__out void *retval);
        
        /* [id][funcdescattr] */ HRESULT ( STDMETHODCALLTYPE *IsVisible )( 
            TextPane2 * This,
            /* [in][idldescattr] */ __RPC__in_opt TextPoint *Point,
            /* [optional][in][idldescattr] */ VARIANT PointOrCount,
            /* [retval][out] */ __RPC__out BOOLEAN *retval);
        
        /* [id][funcdescattr] */ HRESULT ( STDMETHODCALLTYPE *TryToShow )( 
            TextPane2 * This,
            /* [in][idldescattr] */ __RPC__in_opt TextPoint *Point,
            /* [in][idldescattr] */ enum vsPaneShowHow How,
            /* [optional][in][idldescattr] */ VARIANT PointOrCount,
            /* [retval][out] */ __RPC__out BOOLEAN *retval);
        
        /* [helpstringcontext][helpstring][helpcontext][propget][id] */ HRESULT ( STDMETHODCALLTYPE *get_IncrementalSearch )( 
            TextPane2 * This,
            /* [retval][out] */ __RPC__deref_out_opt IncrementalSearch **ppIncrementalSearch);
        
        END_INTERFACE
    } TextPane2Vtbl;

    interface TextPane2
    {
        CONST_VTBL struct TextPane2Vtbl *lpVtbl;
    };

    

#ifdef COBJMACROS


#define TextPane2_QueryInterface(This,riid,ppvObj,retval)	\
    ( (This)->lpVtbl -> QueryInterface(This,riid,ppvObj,retval) ) 

#define TextPane2_AddRef(This,retval)	\
    ( (This)->lpVtbl -> AddRef(This,retval) ) 

#define TextPane2_Release(This,retval)	\
    ( (This)->lpVtbl -> Release(This,retval) ) 

#define TextPane2_GetTypeInfoCount(This,pctinfo,retval)	\
    ( (This)->lpVtbl -> GetTypeInfoCount(This,pctinfo,retval) ) 

#define TextPane2_GetTypeInfo(This,itinfo,lcid,pptinfo,retval)	\
    ( (This)->lpVtbl -> GetTypeInfo(This,itinfo,lcid,pptinfo,retval) ) 

#define TextPane2_GetIDsOfNames(This,riid,rgszNames,cNames,lcid,rgdispid,retval)	\
    ( (This)->lpVtbl -> GetIDsOfNames(This,riid,rgszNames,cNames,lcid,rgdispid,retval) ) 

#define TextPane2_Invoke(This,dispidMember,riid,lcid,wFlags,pdispparams,pvarResult,pexcepinfo,puArgErr,retval)	\
    ( (This)->lpVtbl -> Invoke(This,dispidMember,riid,lcid,wFlags,pdispparams,pvarResult,pexcepinfo,puArgErr,retval) ) 

#define TextPane2_get_DTE(This,retval)	\
    ( (This)->lpVtbl -> get_DTE(This,retval) ) 

#define TextPane2_get_Collection(This,retval)	\
    ( (This)->lpVtbl -> get_Collection(This,retval) ) 

#define TextPane2_get_Window(This,retval)	\
    ( (This)->lpVtbl -> get_Window(This,retval) ) 

#define TextPane2_get_Height(This,retval)	\
    ( (This)->lpVtbl -> get_Height(This,retval) ) 

#define TextPane2_get_Width(This,retval)	\
    ( (This)->lpVtbl -> get_Width(This,retval) ) 

#define TextPane2_get_Selection(This,retval)	\
    ( (This)->lpVtbl -> get_Selection(This,retval) ) 

#define TextPane2_get_StartPoint(This,retval)	\
    ( (This)->lpVtbl -> get_StartPoint(This,retval) ) 

#define TextPane2_Activate(This,retval)	\
    ( (This)->lpVtbl -> Activate(This,retval) ) 

#define TextPane2_IsVisible(This,Point,PointOrCount,retval)	\
    ( (This)->lpVtbl -> IsVisible(This,Point,PointOrCount,retval) ) 

#define TextPane2_TryToShow(This,Point,How,PointOrCount,retval)	\
    ( (This)->lpVtbl -> TryToShow(This,Point,How,PointOrCount,retval) ) 


#define TextPane2_get_IncrementalSearch(This,ppIncrementalSearch)	\
    ( (This)->lpVtbl -> get_IncrementalSearch(This,ppIncrementalSearch) ) 

#endif /* COBJMACROS */


#endif 	/* C style interface */




#endif 	/* __TextPane2_INTERFACE_DEFINED__ */


#ifndef __SolutionConfiguration2_INTERFACE_DEFINED__
#define __SolutionConfiguration2_INTERFACE_DEFINED__

/* interface SolutionConfiguration2 */
/* [helpstringcontext][helpstring][helpcontext][uuid][object][oleautomation][dual] */ 


EXTERN_C const IID IID_SolutionConfiguration2;

#if defined(__cplusplus) && !defined(CINTERFACE)
    
    MIDL_INTERFACE("1099AAA6-4169-430D-9F57-0B4C76624B3B")
    SolutionConfiguration2 : public SolutionConfiguration
    {
    public:
        virtual /* [helpstring][propget][id] */ HRESULT STDMETHODCALLTYPE get_PlatformName( 
            /* [retval][out] */ __RPC__deref_out_opt BSTR *pName) = 0;
        
    };
    
#else 	/* C style interface */

    typedef struct SolutionConfiguration2Vtbl
    {
        BEGIN_INTERFACE
        
        /* [id][restricted][funcdescattr] */ HRESULT ( STDMETHODCALLTYPE *QueryInterface )( 
            SolutionConfiguration2 * This,
            /* [in][idldescattr] */ __RPC__in struct GUID *riid,
            /* [out][idldescattr] */ __RPC__deref_out_opt void **ppvObj,
            /* [retval][out] */ __RPC__out void *retval);
        
        /* [id][restricted][funcdescattr] */ HRESULT ( STDMETHODCALLTYPE *AddRef )( 
            SolutionConfiguration2 * This,
            /* [retval][out] */ __RPC__out unsigned long *retval);
        
        /* [id][restricted][funcdescattr] */ HRESULT ( STDMETHODCALLTYPE *Release )( 
            SolutionConfiguration2 * This,
            /* [retval][out] */ __RPC__out unsigned long *retval);
        
        /* [id][restricted][funcdescattr] */ HRESULT ( STDMETHODCALLTYPE *GetTypeInfoCount )( 
            SolutionConfiguration2 * This,
            /* [out][idldescattr] */ __RPC__out unsigned UINT *pctinfo,
            /* [retval][out] */ __RPC__out void *retval);
        
        /* [id][restricted][funcdescattr] */ HRESULT ( STDMETHODCALLTYPE *GetTypeInfo )( 
            SolutionConfiguration2 * This,
            /* [in][idldescattr] */ unsigned UINT itinfo,
            /* [in][idldescattr] */ unsigned long lcid,
            /* [out][idldescattr] */ __RPC__deref_out_opt void **pptinfo,
            /* [retval][out] */ __RPC__out void *retval);
        
        /* [id][restricted][funcdescattr] */ HRESULT ( STDMETHODCALLTYPE *GetIDsOfNames )( 
            SolutionConfiguration2 * This,
            /* [in][idldescattr] */ __RPC__in struct GUID *riid,
            /* [in][idldescattr] */ __RPC__deref_in_opt signed char **rgszNames,
            /* [in][idldescattr] */ unsigned UINT cNames,
            /* [in][idldescattr] */ unsigned long lcid,
            /* [out][idldescattr] */ __RPC__out signed long *rgdispid,
            /* [retval][out] */ __RPC__out void *retval);
        
        /* [id][restricted][funcdescattr] */ HRESULT ( STDMETHODCALLTYPE *Invoke )( 
            SolutionConfiguration2 * This,
            /* [in][idldescattr] */ signed long dispidMember,
            /* [in][idldescattr] */ __RPC__in struct GUID *riid,
            /* [in][idldescattr] */ unsigned long lcid,
            /* [in][idldescattr] */ unsigned short wFlags,
            /* [in][idldescattr] */ __RPC__in struct DISPPARAMS *pdispparams,
            /* [out][idldescattr] */ __RPC__out VARIANT *pvarResult,
            /* [out][idldescattr] */ __RPC__out struct EXCEPINFO *pexcepinfo,
            /* [out][idldescattr] */ __RPC__out unsigned UINT *puArgErr,
            /* [retval][out] */ __RPC__out void *retval);
        
        /* [id][propget][funcdescattr] */ HRESULT ( STDMETHODCALLTYPE *get_DTE )( 
            SolutionConfiguration2 * This,
            /* [retval][out] */ __RPC__deref_out_opt **retval);
        
        /* [id][propget][funcdescattr] */ HRESULT ( STDMETHODCALLTYPE *get_Collection )( 
            SolutionConfiguration2 * This,
            /* [retval][out] */ __RPC__deref_out_opt SolutionConfigurations **retval);
        
        /* [id][propget][funcdescattr] */ HRESULT ( STDMETHODCALLTYPE *get_Name )( 
            SolutionConfiguration2 * This,
            /* [retval][out] */ __RPC__deref_out_opt BSTR *retval);
        
        /* [id][propget][funcdescattr] */ HRESULT ( STDMETHODCALLTYPE *get_SolutionContexts )( 
            SolutionConfiguration2 * This,
            /* [retval][out] */ __RPC__deref_out_opt SolutionContexts **retval);
        
        /* [id][funcdescattr] */ HRESULT ( STDMETHODCALLTYPE *Delete )( 
            SolutionConfiguration2 * This,
            /* [retval][out] */ __RPC__out void *retval);
        
        /* [id][funcdescattr] */ HRESULT ( STDMETHODCALLTYPE *Activate )( 
            SolutionConfiguration2 * This,
            /* [retval][out] */ __RPC__out void *retval);
        
        /* [helpstring][propget][id] */ HRESULT ( STDMETHODCALLTYPE *get_PlatformName )( 
            SolutionConfiguration2 * This,
            /* [retval][out] */ __RPC__deref_out_opt BSTR *pName);
        
        END_INTERFACE
    } SolutionConfiguration2Vtbl;

    interface SolutionConfiguration2
    {
        CONST_VTBL struct SolutionConfiguration2Vtbl *lpVtbl;
    };

    

#ifdef COBJMACROS


#define SolutionConfiguration2_QueryInterface(This,riid,ppvObj,retval)	\
    ( (This)->lpVtbl -> QueryInterface(This,riid,ppvObj,retval) ) 

#define SolutionConfiguration2_AddRef(This,retval)	\
    ( (This)->lpVtbl -> AddRef(This,retval) ) 

#define SolutionConfiguration2_Release(This,retval)	\
    ( (This)->lpVtbl -> Release(This,retval) ) 

#define SolutionConfiguration2_GetTypeInfoCount(This,pctinfo,retval)	\
    ( (This)->lpVtbl -> GetTypeInfoCount(This,pctinfo,retval) ) 

#define SolutionConfiguration2_GetTypeInfo(This,itinfo,lcid,pptinfo,retval)	\
    ( (This)->lpVtbl -> GetTypeInfo(This,itinfo,lcid,pptinfo,retval) ) 

#define SolutionConfiguration2_GetIDsOfNames(This,riid,rgszNames,cNames,lcid,rgdispid,retval)	\
    ( (This)->lpVtbl -> GetIDsOfNames(This,riid,rgszNames,cNames,lcid,rgdispid,retval) ) 

#define SolutionConfiguration2_Invoke(This,dispidMember,riid,lcid,wFlags,pdispparams,pvarResult,pexcepinfo,puArgErr,retval)	\
    ( (This)->lpVtbl -> Invoke(This,dispidMember,riid,lcid,wFlags,pdispparams,pvarResult,pexcepinfo,puArgErr,retval) ) 

#define SolutionConfiguration2_get_DTE(This,retval)	\
    ( (This)->lpVtbl -> get_DTE(This,retval) ) 

#define SolutionConfiguration2_get_Collection(This,retval)	\
    ( (This)->lpVtbl -> get_Collection(This,retval) ) 

#define SolutionConfiguration2_get_Name(This,retval)	\
    ( (This)->lpVtbl -> get_Name(This,retval) ) 

#define SolutionConfiguration2_get_SolutionContexts(This,retval)	\
    ( (This)->lpVtbl -> get_SolutionContexts(This,retval) ) 

#define SolutionConfiguration2_Delete(This,retval)	\
    ( (This)->lpVtbl -> Delete(This,retval) ) 

#define SolutionConfiguration2_Activate(This,retval)	\
    ( (This)->lpVtbl -> Activate(This,retval) ) 


#define SolutionConfiguration2_get_PlatformName(This,pName)	\
    ( (This)->lpVtbl -> get_PlatformName(This,pName) ) 

#endif /* COBJMACROS */


#endif 	/* C style interface */




#endif 	/* __SolutionConfiguration2_INTERFACE_DEFINED__ */


#ifndef __IVsProfferCommands2_INTERFACE_DEFINED__
#define __IVsProfferCommands2_INTERFACE_DEFINED__

/* interface IVsProfferCommands2 */
/* [object][restricted][hidden][version][uuid] */ 


EXTERN_C const IID IID_IVsProfferCommands2;

#if defined(__cplusplus) && !defined(CINTERFACE)
    
    MIDL_INTERFACE("c508b13c-06c5-40c7-b405-d327d4f8e268")
    IVsProfferCommands2 : public IVsProfferCommands
    {
    public:
        virtual HRESULT STDMETHODCALLTYPE AddNamedCommand2( 
            /* [in] */ __RPC__in const GUID *pguidPackage,
            /* [in] */ __RPC__in const GUID *pguidCmdGroup,
            /* [string][in] */ __RPC__in const LPCOLESTR pszCmdNameCanonical,
            /* [out] */ __RPC__out DWORD *pdwCmdId,
            /* [string][in] */ __RPC__in const LPCOLESTR pszCmdNameLocalized,
            /* [string][in] */ __RPC__in const LPCOLESTR pszBtnText,
            /* [string][in] */ __RPC__in const LPCOLESTR pszCmdTooltip,
            /* [string][in] */ __RPC__in const LPCOLESTR pszSatelliteDLL,
            /* [in] */ DWORD dwBitmapResourceId,
            /* [in] */ DWORD dwBitmapImageIndex,
            /* [in] */ DWORD dwCmdFlagsDefault,
            /* [in] */ DWORD cUIContexts,
            /* [size_is][in] */ __RPC__in_ecount_full(cUIContexts) const GUID *rgguidUIContexts,
            /* [in] */ DWORD dwUIElementType) = 0;
        
    };
    
#else 	/* C style interface */

    typedef struct IVsProfferCommands2Vtbl
    {
        BEGIN_INTERFACE
        
        /* [id][restricted][funcdescattr] */ HRESULT ( STDMETHODCALLTYPE *QueryInterface )( 
            IVsProfferCommands2 * This,
            /* [in][idldescattr] */ __RPC__in struct GUID *riid,
            /* [out][idldescattr] */ __RPC__deref_out_opt void **ppvObj);
        
        /* [id][restricted][funcdescattr] */ unsigned long ( STDMETHODCALLTYPE *AddRef )( 
            IVsProfferCommands2 * This);
        
        /* [id][restricted][funcdescattr] */ unsigned long ( STDMETHODCALLTYPE *Release )( 
            IVsProfferCommands2 * This);
        
        /* [id][funcdescattr] */ HRESULT ( STDMETHODCALLTYPE *AddNamedCommand )( 
            IVsProfferCommands2 * This,
            /* [in][idldescattr] */ __RPC__in struct GUID *pguidPackage,
            /* [in][idldescattr] */ __RPC__in struct GUID *pguidCmdGroup,
            /* [in][idldescattr] */ LPWSTR pszCmdNameCanonical,
            /* [out][idldescattr] */ __RPC__out unsigned long *pdwCmdId,
            /* [in][idldescattr] */ LPWSTR pszCmdNameLocalized,
            /* [in][idldescattr] */ LPWSTR pszBtnText,
            /* [in][idldescattr] */ LPWSTR pszCmdTooltip,
            /* [in][idldescattr] */ LPWSTR pszSatelliteDLL,
            /* [in][idldescattr] */ unsigned long dwBitmapResourceId,
            /* [in][idldescattr] */ unsigned long dwBitmapImageIndex,
            /* [in][idldescattr] */ unsigned long dwCmdFlagsDefault,
            /* [in][idldescattr] */ unsigned long cUIContexts,
            /* [in][idldescattr] */ __RPC__in struct GUID *rgguidUIContexts);
        
        /* [id][funcdescattr] */ HRESULT ( STDMETHODCALLTYPE *RemoveNamedCommand )( 
            IVsProfferCommands2 * This,
            /* [in][idldescattr] */ LPWSTR pszCmdNameCanonical);
        
        /* [id][funcdescattr] */ HRESULT ( STDMETHODCALLTYPE *RenameNamedCommand )( 
            IVsProfferCommands2 * This,
            /* [in][idldescattr] */ LPWSTR pszCmdNameCanonical,
            /* [in][idldescattr] */ LPWSTR pszCmdNameCanonicalNew,
            /* [in][idldescattr] */ LPWSTR pszCmdNameLocalizedNew);
        
        /* [id][funcdescattr] */ HRESULT ( STDMETHODCALLTYPE *AddCommandBarControl )( 
            IVsProfferCommands2 * This,
            /* [in][idldescattr] */ LPWSTR pszCmdNameCanonical,
            /* [in][idldescattr] */ __RPC__in_opt IDispatch *pCmdBarParent,
            /* [in][idldescattr] */ unsigned long dwIndex,
            /* [in][idldescattr] */ unsigned long dwCmdType,
            /* [out][idldescattr] */ __RPC__deref_out_opt IDispatch **ppCmdBarCtrl);
        
        /* [id][funcdescattr] */ HRESULT ( STDMETHODCALLTYPE *RemoveCommandBarControl )( 
            IVsProfferCommands2 * This,
            /* [in][idldescattr] */ __RPC__in_opt IDispatch *pCmdBarCtrl);
        
        /* [id][funcdescattr] */ HRESULT ( STDMETHODCALLTYPE *AddCommandBar )( 
            IVsProfferCommands2 * This,
            /* [in][idldescattr] */ LPWSTR pszCmdBarName,
            /* [in][idldescattr] */ enum vsCommandBarType dwType,
            /* [in][idldescattr] */ __RPC__in_opt IDispatch *pCmdBarParent,
            /* [in][idldescattr] */ unsigned long dwIndex,
            /* [out][idldescattr] */ __RPC__deref_out_opt IDispatch **ppCmdBar);
        
        /* [id][funcdescattr] */ HRESULT ( STDMETHODCALLTYPE *RemoveCommandBar )( 
            IVsProfferCommands2 * This,
            /* [in][idldescattr] */ __RPC__in_opt IDispatch *pCmdBar);
        
        /* [id][funcdescattr] */ HRESULT ( STDMETHODCALLTYPE *FindCommandBar )( 
            IVsProfferCommands2 * This,
            /* [in][idldescattr] */ __RPC__in void *pToolbarSet,
            /* [in][idldescattr] */ __RPC__in struct GUID *pguidCmdGroup,
            /* [in][idldescattr] */ unsigned long dwMenuId,
            /* [out][idldescattr] */ __RPC__deref_out_opt IDispatch **ppdispCmdBar);
        
        HRESULT ( STDMETHODCALLTYPE *AddNamedCommand2 )( 
            IVsProfferCommands2 * This,
            /* [in] */ __RPC__in const GUID *pguidPackage,
            /* [in] */ __RPC__in const GUID *pguidCmdGroup,
            /* [string][in] */ __RPC__in const LPCOLESTR pszCmdNameCanonical,
            /* [out] */ __RPC__out DWORD *pdwCmdId,
            /* [string][in] */ __RPC__in const LPCOLESTR pszCmdNameLocalized,
            /* [string][in] */ __RPC__in const LPCOLESTR pszBtnText,
            /* [string][in] */ __RPC__in const LPCOLESTR pszCmdTooltip,
            /* [string][in] */ __RPC__in const LPCOLESTR pszSatelliteDLL,
            /* [in] */ DWORD dwBitmapResourceId,
            /* [in] */ DWORD dwBitmapImageIndex,
            /* [in] */ DWORD dwCmdFlagsDefault,
            /* [in] */ DWORD cUIContexts,
            /* [size_is][in] */ __RPC__in_ecount_full(cUIContexts) const GUID *rgguidUIContexts,
            /* [in] */ DWORD dwUIElementType);
        
        END_INTERFACE
    } IVsProfferCommands2Vtbl;

    interface IVsProfferCommands2
    {
        CONST_VTBL struct IVsProfferCommands2Vtbl *lpVtbl;
    };

    

#ifdef COBJMACROS


#define IVsProfferCommands2_QueryInterface(This,riid,ppvObj)	\
    ( (This)->lpVtbl -> QueryInterface(This,riid,ppvObj) ) 

#define IVsProfferCommands2_AddRef(This)	\
    ( (This)->lpVtbl -> AddRef(This) ) 

#define IVsProfferCommands2_Release(This)	\
    ( (This)->lpVtbl -> Release(This) ) 


#define IVsProfferCommands2_AddNamedCommand(This,pguidPackage,pguidCmdGroup,pszCmdNameCanonical,pdwCmdId,pszCmdNameLocalized,pszBtnText,pszCmdTooltip,pszSatelliteDLL,dwBitmapResourceId,dwBitmapImageIndex,dwCmdFlagsDefault,cUIContexts,rgguidUIContexts)	\
    ( (This)->lpVtbl -> AddNamedCommand(This,pguidPackage,pguidCmdGroup,pszCmdNameCanonical,pdwCmdId,pszCmdNameLocalized,pszBtnText,pszCmdTooltip,pszSatelliteDLL,dwBitmapResourceId,dwBitmapImageIndex,dwCmdFlagsDefault,cUIContexts,rgguidUIContexts) ) 

#define IVsProfferCommands2_RemoveNamedCommand(This,pszCmdNameCanonical)	\
    ( (This)->lpVtbl -> RemoveNamedCommand(This,pszCmdNameCanonical) ) 

#define IVsProfferCommands2_RenameNamedCommand(This,pszCmdNameCanonical,pszCmdNameCanonicalNew,pszCmdNameLocalizedNew)	\
    ( (This)->lpVtbl -> RenameNamedCommand(This,pszCmdNameCanonical,pszCmdNameCanonicalNew,pszCmdNameLocalizedNew) ) 

#define IVsProfferCommands2_AddCommandBarControl(This,pszCmdNameCanonical,pCmdBarParent,dwIndex,dwCmdType,ppCmdBarCtrl)	\
    ( (This)->lpVtbl -> AddCommandBarControl(This,pszCmdNameCanonical,pCmdBarParent,dwIndex,dwCmdType,ppCmdBarCtrl) ) 

#define IVsProfferCommands2_RemoveCommandBarControl(This,pCmdBarCtrl)	\
    ( (This)->lpVtbl -> RemoveCommandBarControl(This,pCmdBarCtrl) ) 

#define IVsProfferCommands2_AddCommandBar(This,pszCmdBarName,dwType,pCmdBarParent,dwIndex,ppCmdBar)	\
    ( (This)->lpVtbl -> AddCommandBar(This,pszCmdBarName,dwType,pCmdBarParent,dwIndex,ppCmdBar) ) 

#define IVsProfferCommands2_RemoveCommandBar(This,pCmdBar)	\
    ( (This)->lpVtbl -> RemoveCommandBar(This,pCmdBar) ) 

#define IVsProfferCommands2_FindCommandBar(This,pToolbarSet,pguidCmdGroup,dwMenuId,ppdispCmdBar)	\
    ( (This)->lpVtbl -> FindCommandBar(This,pToolbarSet,pguidCmdGroup,dwMenuId,ppdispCmdBar) ) 


#define IVsProfferCommands2_AddNamedCommand2(This,pguidPackage,pguidCmdGroup,pszCmdNameCanonical,pdwCmdId,pszCmdNameLocalized,pszBtnText,pszCmdTooltip,pszSatelliteDLL,dwBitmapResourceId,dwBitmapImageIndex,dwCmdFlagsDefault,cUIContexts,rgguidUIContexts,dwUIElementType)	\
    ( (This)->lpVtbl -> AddNamedCommand2(This,pguidPackage,pguidCmdGroup,pszCmdNameCanonical,pdwCmdId,pszCmdNameLocalized,pszBtnText,pszCmdTooltip,pszSatelliteDLL,dwBitmapResourceId,dwBitmapImageIndex,dwCmdFlagsDefault,cUIContexts,rgguidUIContexts,dwUIElementType) ) 

#endif /* COBJMACROS */


#endif 	/* C style interface */




#endif 	/* __IVsProfferCommands2_INTERFACE_DEFINED__ */


#ifndef __SolutionBuild2_INTERFACE_DEFINED__
#define __SolutionBuild2_INTERFACE_DEFINED__

/* interface SolutionBuild2 */
/* [helpstringcontext][helpstring][helpcontext][uuid][object][oleautomation][dual] */ 


EXTERN_C const IID IID_SolutionBuild2;

#if defined(__cplusplus) && !defined(CINTERFACE)
    
    MIDL_INTERFACE("c2516e4b-5d69-459d-b539-c95a71c4fa3d")
    SolutionBuild2 : public SolutionBuild
    {
    public:
        virtual /* [helpstring][id] */ HRESULT STDMETHODCALLTYPE Publish( 
            /* [defaultvalue] */ VARIANT_BOOL WaitForPublishToFinish = 0) = 0;
        
        virtual /* [helpstring][id] */ HRESULT STDMETHODCALLTYPE PublishProject( 
            __RPC__in BSTR SolutionConfiguration,
            __RPC__in BSTR ProjectUniqueName,
            /* [defaultvalue] */ VARIANT_BOOL WaitForPublishToFinish = 0) = 0;
        
        virtual /* [helpstring][propget][id] */ HRESULT STDMETHODCALLTYPE get_LastPublishInfo( 
            /* [retval][out] */ __RPC__out long *Info) = 0;
        
        virtual /* [helpstring][propget][id] */ HRESULT STDMETHODCALLTYPE get_PublishState( 
            /* [retval][out] */ __RPC__out vsPublishState *State) = 0;
        
        virtual /* [helpstring][id] */ HRESULT STDMETHODCALLTYPE DeployProject( 
            __RPC__in BSTR SolutionConfiguration,
            __RPC__in BSTR ProjectUniqueName,
            /* [defaultvalue] */ VARIANT_BOOL WaitForDeployToFinish = 0) = 0;
        
    };
    
#else 	/* C style interface */

    typedef struct SolutionBuild2Vtbl
    {
        BEGIN_INTERFACE
        
        /* [id][restricted][funcdescattr] */ HRESULT ( STDMETHODCALLTYPE *QueryInterface )( 
            SolutionBuild2 * This,
            /* [in][idldescattr] */ __RPC__in struct GUID *riid,
            /* [out][idldescattr] */ __RPC__deref_out_opt void **ppvObj,
            /* [retval][out] */ __RPC__out void *retval);
        
        /* [id][restricted][funcdescattr] */ HRESULT ( STDMETHODCALLTYPE *AddRef )( 
            SolutionBuild2 * This,
            /* [retval][out] */ __RPC__out unsigned long *retval);
        
        /* [id][restricted][funcdescattr] */ HRESULT ( STDMETHODCALLTYPE *Release )( 
            SolutionBuild2 * This,
            /* [retval][out] */ __RPC__out unsigned long *retval);
        
        /* [id][restricted][funcdescattr] */ HRESULT ( STDMETHODCALLTYPE *GetTypeInfoCount )( 
            SolutionBuild2 * This,
            /* [out][idldescattr] */ __RPC__out unsigned UINT *pctinfo,
            /* [retval][out] */ __RPC__out void *retval);
        
        /* [id][restricted][funcdescattr] */ HRESULT ( STDMETHODCALLTYPE *GetTypeInfo )( 
            SolutionBuild2 * This,
            /* [in][idldescattr] */ unsigned UINT itinfo,
            /* [in][idldescattr] */ unsigned long lcid,
            /* [out][idldescattr] */ __RPC__deref_out_opt void **pptinfo,
            /* [retval][out] */ __RPC__out void *retval);
        
        /* [id][restricted][funcdescattr] */ HRESULT ( STDMETHODCALLTYPE *GetIDsOfNames )( 
            SolutionBuild2 * This,
            /* [in][idldescattr] */ __RPC__in struct GUID *riid,
            /* [in][idldescattr] */ __RPC__deref_in_opt signed char **rgszNames,
            /* [in][idldescattr] */ unsigned UINT cNames,
            /* [in][idldescattr] */ unsigned long lcid,
            /* [out][idldescattr] */ __RPC__out signed long *rgdispid,
            /* [retval][out] */ __RPC__out void *retval);
        
        /* [id][restricted][funcdescattr] */ HRESULT ( STDMETHODCALLTYPE *Invoke )( 
            SolutionBuild2 * This,
            /* [in][idldescattr] */ signed long dispidMember,
            /* [in][idldescattr] */ __RPC__in struct GUID *riid,
            /* [in][idldescattr] */ unsigned long lcid,
            /* [in][idldescattr] */ unsigned short wFlags,
            /* [in][idldescattr] */ __RPC__in struct DISPPARAMS *pdispparams,
            /* [out][idldescattr] */ __RPC__out VARIANT *pvarResult,
            /* [out][idldescattr] */ __RPC__out struct EXCEPINFO *pexcepinfo,
            /* [out][idldescattr] */ __RPC__out unsigned UINT *puArgErr,
            /* [retval][out] */ __RPC__out void *retval);
        
        /* [id][propget][funcdescattr] */ HRESULT ( STDMETHODCALLTYPE *get_DTE )( 
            SolutionBuild2 * This,
            /* [retval][out] */ __RPC__deref_out_opt **retval);
        
        /* [id][propget][funcdescattr] */ HRESULT ( STDMETHODCALLTYPE *get_Parent )( 
            SolutionBuild2 * This,
            /* [retval][out] */ __RPC__deref_out_opt **retval);
        
        /* [id][propget][funcdescattr] */ HRESULT ( STDMETHODCALLTYPE *get_ActiveConfiguration )( 
            SolutionBuild2 * This,
            /* [retval][out] */ __RPC__deref_out_opt SolutionConfiguration **retval);
        
        /* [id][propget][funcdescattr] */ HRESULT ( STDMETHODCALLTYPE *get_BuildDependencies )( 
            SolutionBuild2 * This,
            /* [retval][out] */ __RPC__deref_out_opt BuildDependencies **retval);
        
        /* [id][propget][funcdescattr] */ HRESULT ( STDMETHODCALLTYPE *get_BuildState )( 
            SolutionBuild2 * This,
            /* [retval][out] */ __RPC__out enum vsBuildState *retval);
        
        /* [id][propget][funcdescattr] */ HRESULT ( STDMETHODCALLTYPE *get_LastBuildInfo )( 
            SolutionBuild2 * This,
            /* [retval][out] */ __RPC__out signed long *retval);
        
        /* [id][propput][funcdescattr] */ HRESULT ( STDMETHODCALLTYPE *put_StartupProjects )( 
            SolutionBuild2 * This,
            /* [in][idldescattr] */ VARIANT noname,
            /* [retval][out] */ __RPC__out void *retval);
        
        /* [id][propget][funcdescattr] */ HRESULT ( STDMETHODCALLTYPE *get_StartupProjects )( 
            SolutionBuild2 * This,
            /* [retval][out] */ __RPC__out VARIANT *retval);
        
        /* [id][funcdescattr] */ HRESULT ( STDMETHODCALLTYPE *Build )( 
            SolutionBuild2 * This,
            /* [in][idldescattr] */ BOOLEAN WaitForBuildToFinish,
            /* [retval][out] */ __RPC__out void *retval);
        
        /* [id][funcdescattr] */ HRESULT ( STDMETHODCALLTYPE *Debug )( 
            SolutionBuild2 * This,
            /* [retval][out] */ __RPC__out void *retval);
        
        /* [id][funcdescattr] */ HRESULT ( STDMETHODCALLTYPE *Deploy )( 
            SolutionBuild2 * This,
            /* [in][idldescattr] */ BOOLEAN WaitForDeployToFinish,
            /* [retval][out] */ __RPC__out void *retval);
        
        /* [id][funcdescattr] */ HRESULT ( STDMETHODCALLTYPE *Clean )( 
            SolutionBuild2 * This,
            /* [in][idldescattr] */ BOOLEAN WaitForCleanToFinish,
            /* [retval][out] */ __RPC__out void *retval);
        
        /* [id][funcdescattr] */ HRESULT ( STDMETHODCALLTYPE *Run )( 
            SolutionBuild2 * This,
            /* [retval][out] */ __RPC__out void *retval);
        
        /* [id][propget][funcdescattr] */ HRESULT ( STDMETHODCALLTYPE *get_SolutionConfigurations )( 
            SolutionBuild2 * This,
            /* [retval][out] */ __RPC__deref_out_opt SolutionConfigurations **retval);
        
        /* [id][funcdescattr] */ HRESULT ( STDMETHODCALLTYPE *BuildProject )( 
            SolutionBuild2 * This,
            /* [idldescattr] */ __RPC__in BSTR SolutionConfiguration,
            /* [idldescattr] */ __RPC__in BSTR ProjectUniqueName,
            /* [idldescattr] */ BOOLEAN WaitForBuildToFinish,
            /* [retval][out] */ __RPC__out void *retval);
        
        /* [helpstring][id] */ HRESULT ( STDMETHODCALLTYPE *Publish )( 
            SolutionBuild2 * This,
            /* [defaultvalue] */ VARIANT_BOOL WaitForPublishToFinish);
        
        /* [helpstring][id] */ HRESULT ( STDMETHODCALLTYPE *PublishProject )( 
            SolutionBuild2 * This,
            __RPC__in BSTR SolutionConfiguration,
            __RPC__in BSTR ProjectUniqueName,
            /* [defaultvalue] */ VARIANT_BOOL WaitForPublishToFinish);
        
        /* [helpstring][propget][id] */ HRESULT ( STDMETHODCALLTYPE *get_LastPublishInfo )( 
            SolutionBuild2 * This,
            /* [retval][out] */ __RPC__out long *Info);
        
        /* [helpstring][propget][id] */ HRESULT ( STDMETHODCALLTYPE *get_PublishState )( 
            SolutionBuild2 * This,
            /* [retval][out] */ __RPC__out vsPublishState *State);
        
        /* [helpstring][id] */ HRESULT ( STDMETHODCALLTYPE *DeployProject )( 
            SolutionBuild2 * This,
            __RPC__in BSTR SolutionConfiguration,
            __RPC__in BSTR ProjectUniqueName,
            /* [defaultvalue] */ VARIANT_BOOL WaitForDeployToFinish);
        
        END_INTERFACE
    } SolutionBuild2Vtbl;

    interface SolutionBuild2
    {
        CONST_VTBL struct SolutionBuild2Vtbl *lpVtbl;
    };

    

#ifdef COBJMACROS


#define SolutionBuild2_QueryInterface(This,riid,ppvObj,retval)	\
    ( (This)->lpVtbl -> QueryInterface(This,riid,ppvObj,retval) ) 

#define SolutionBuild2_AddRef(This,retval)	\
    ( (This)->lpVtbl -> AddRef(This,retval) ) 

#define SolutionBuild2_Release(This,retval)	\
    ( (This)->lpVtbl -> Release(This,retval) ) 

#define SolutionBuild2_GetTypeInfoCount(This,pctinfo,retval)	\
    ( (This)->lpVtbl -> GetTypeInfoCount(This,pctinfo,retval) ) 

#define SolutionBuild2_GetTypeInfo(This,itinfo,lcid,pptinfo,retval)	\
    ( (This)->lpVtbl -> GetTypeInfo(This,itinfo,lcid,pptinfo,retval) ) 

#define SolutionBuild2_GetIDsOfNames(This,riid,rgszNames,cNames,lcid,rgdispid,retval)	\
    ( (This)->lpVtbl -> GetIDsOfNames(This,riid,rgszNames,cNames,lcid,rgdispid,retval) ) 

#define SolutionBuild2_Invoke(This,dispidMember,riid,lcid,wFlags,pdispparams,pvarResult,pexcepinfo,puArgErr,retval)	\
    ( (This)->lpVtbl -> Invoke(This,dispidMember,riid,lcid,wFlags,pdispparams,pvarResult,pexcepinfo,puArgErr,retval) ) 

#define SolutionBuild2_get_DTE(This,retval)	\
    ( (This)->lpVtbl -> get_DTE(This,retval) ) 

#define SolutionBuild2_get_Parent(This,retval)	\
    ( (This)->lpVtbl -> get_Parent(This,retval) ) 

#define SolutionBuild2_get_ActiveConfiguration(This,retval)	\
    ( (This)->lpVtbl -> get_ActiveConfiguration(This,retval) ) 

#define SolutionBuild2_get_BuildDependencies(This,retval)	\
    ( (This)->lpVtbl -> get_BuildDependencies(This,retval) ) 

#define SolutionBuild2_get_BuildState(This,retval)	\
    ( (This)->lpVtbl -> get_BuildState(This,retval) ) 

#define SolutionBuild2_get_LastBuildInfo(This,retval)	\
    ( (This)->lpVtbl -> get_LastBuildInfo(This,retval) ) 

#define SolutionBuild2_put_StartupProjects(This,noname,retval)	\
    ( (This)->lpVtbl -> put_StartupProjects(This,noname,retval) ) 

#define SolutionBuild2_get_StartupProjects(This,retval)	\
    ( (This)->lpVtbl -> get_StartupProjects(This,retval) ) 

#define SolutionBuild2_Build(This,WaitForBuildToFinish,retval)	\
    ( (This)->lpVtbl -> Build(This,WaitForBuildToFinish,retval) ) 

#define SolutionBuild2_Debug(This,retval)	\
    ( (This)->lpVtbl -> Debug(This,retval) ) 

#define SolutionBuild2_Deploy(This,WaitForDeployToFinish,retval)	\
    ( (This)->lpVtbl -> Deploy(This,WaitForDeployToFinish,retval) ) 

#define SolutionBuild2_Clean(This,WaitForCleanToFinish,retval)	\
    ( (This)->lpVtbl -> Clean(This,WaitForCleanToFinish,retval) ) 

#define SolutionBuild2_Run(This,retval)	\
    ( (This)->lpVtbl -> Run(This,retval) ) 

#define SolutionBuild2_get_SolutionConfigurations(This,retval)	\
    ( (This)->lpVtbl -> get_SolutionConfigurations(This,retval) ) 

#define SolutionBuild2_BuildProject(This,SolutionConfiguration,ProjectUniqueName,WaitForBuildToFinish,retval)	\
    ( (This)->lpVtbl -> BuildProject(This,SolutionConfiguration,ProjectUniqueName,WaitForBuildToFinish,retval) ) 


#define SolutionBuild2_Publish(This,WaitForPublishToFinish)	\
    ( (This)->lpVtbl -> Publish(This,WaitForPublishToFinish) ) 

#define SolutionBuild2_PublishProject(This,SolutionConfiguration,ProjectUniqueName,WaitForPublishToFinish)	\
    ( (This)->lpVtbl -> PublishProject(This,SolutionConfiguration,ProjectUniqueName,WaitForPublishToFinish) ) 

#define SolutionBuild2_get_LastPublishInfo(This,Info)	\
    ( (This)->lpVtbl -> get_LastPublishInfo(This,Info) ) 

#define SolutionBuild2_get_PublishState(This,State)	\
    ( (This)->lpVtbl -> get_PublishState(This,State) ) 

#define SolutionBuild2_DeployProject(This,SolutionConfiguration,ProjectUniqueName,WaitForDeployToFinish)	\
    ( (This)->lpVtbl -> DeployProject(This,SolutionConfiguration,ProjectUniqueName,WaitForDeployToFinish) ) 

#endif /* COBJMACROS */


#endif 	/* C style interface */




#endif 	/* __SolutionBuild2_INTERFACE_DEFINED__ */


#ifndef __ErrorItems_INTERFACE_DEFINED__
#define __ErrorItems_INTERFACE_DEFINED__

/* interface ErrorItems */
/* [helpstring][uuid][object][oleautomation][dual] */ 


EXTERN_C const IID IID_ErrorItems;

#if defined(__cplusplus) && !defined(CINTERFACE)
    
    MIDL_INTERFACE("dcf93a30-d013-42f4-8aee-9f5ba215fb8b")
    ErrorItems : public IDispatch
    {
    public:
        virtual /* [helpstringcontext][helpstring][helpcontext][propget][id] */ HRESULT STDMETHODCALLTYPE get_DTE( 
            /* [retval][out] */ __RPC__deref_out_opt /* external definition not present */ DTE **DTEObject) = 0;
        
        virtual /* [helpstringcontext][helpstring][helpcontext][propget][id] */ HRESULT STDMETHODCALLTYPE get_Parent( 
            /* [retval][out] */ __RPC__deref_out_opt ErrorList **ErrorList) = 0;
        
        virtual /* [helpstringcontext][helpstring][helpcontext][id] */ HRESULT STDMETHODCALLTYPE Item( 
            /* [in] */ VARIANT index,
            /* [retval][out] */ __RPC__deref_out_opt ErrorItem **ErrorItem) = 0;
        
        virtual /* [helpstringcontext][helpstring][helpcontext][propget][id] */ HRESULT STDMETHODCALLTYPE get_Count( 
            /* [retval][out] */ __RPC__out long *Count) = 0;
        
    };
    
#else 	/* C style interface */

    typedef struct ErrorItemsVtbl
    {
        BEGIN_INTERFACE
        
        HRESULT ( STDMETHODCALLTYPE *QueryInterface )( 
            ErrorItems * This,
            /* [in] */ __RPC__in REFIID riid,
            /* [iid_is][out] */ 
            __RPC__deref_out  void **ppvObject);
        
        ULONG ( STDMETHODCALLTYPE *AddRef )( 
            ErrorItems * This);
        
        ULONG ( STDMETHODCALLTYPE *Release )( 
            ErrorItems * This);
        
        HRESULT ( STDMETHODCALLTYPE *GetTypeInfoCount )( 
            ErrorItems * This,
            /* [out] */ __RPC__out UINT *pctinfo);
        
        HRESULT ( STDMETHODCALLTYPE *GetTypeInfo )( 
            ErrorItems * This,
            /* [in] */ UINT iTInfo,
            /* [in] */ LCID lcid,
            /* [out] */ __RPC__deref_out_opt ITypeInfo **ppTInfo);
        
        HRESULT ( STDMETHODCALLTYPE *GetIDsOfNames )( 
            ErrorItems * This,
            /* [in] */ __RPC__in REFIID riid,
            /* [size_is][in] */ __RPC__in_ecount_full(cNames) LPOLESTR *rgszNames,
            /* [range][in] */ UINT cNames,
            /* [in] */ LCID lcid,
            /* [size_is][out] */ __RPC__out_ecount_full(cNames) DISPID *rgDispId);
        
        /* [local] */ HRESULT ( STDMETHODCALLTYPE *Invoke )( 
            ErrorItems * This,
            /* [in] */ DISPID dispIdMember,
            /* [in] */ REFIID riid,
            /* [in] */ LCID lcid,
            /* [in] */ WORD wFlags,
            /* [out][in] */ DISPPARAMS *pDispParams,
            /* [out] */ VARIANT *pVarResult,
            /* [out] */ EXCEPINFO *pExcepInfo,
            /* [out] */ UINT *puArgErr);
        
        /* [helpstringcontext][helpstring][helpcontext][propget][id] */ HRESULT ( STDMETHODCALLTYPE *get_DTE )( 
            ErrorItems * This,
            /* [retval][out] */ __RPC__deref_out_opt /* external definition not present */ DTE **DTEObject);
        
        /* [helpstringcontext][helpstring][helpcontext][propget][id] */ HRESULT ( STDMETHODCALLTYPE *get_Parent )( 
            ErrorItems * This,
            /* [retval][out] */ __RPC__deref_out_opt ErrorList **ErrorList);
        
        /* [helpstringcontext][helpstring][helpcontext][id] */ HRESULT ( STDMETHODCALLTYPE *Item )( 
            ErrorItems * This,
            /* [in] */ VARIANT index,
            /* [retval][out] */ __RPC__deref_out_opt ErrorItem **ErrorItem);
        
        /* [helpstringcontext][helpstring][helpcontext][propget][id] */ HRESULT ( STDMETHODCALLTYPE *get_Count )( 
            ErrorItems * This,
            /* [retval][out] */ __RPC__out long *Count);
        
        END_INTERFACE
    } ErrorItemsVtbl;

    interface ErrorItems
    {
        CONST_VTBL struct ErrorItemsVtbl *lpVtbl;
    };

    

#ifdef COBJMACROS


#define ErrorItems_QueryInterface(This,riid,ppvObject)	\
    ( (This)->lpVtbl -> QueryInterface(This,riid,ppvObject) ) 

#define ErrorItems_AddRef(This)	\
    ( (This)->lpVtbl -> AddRef(This) ) 

#define ErrorItems_Release(This)	\
    ( (This)->lpVtbl -> Release(This) ) 


#define ErrorItems_GetTypeInfoCount(This,pctinfo)	\
    ( (This)->lpVtbl -> GetTypeInfoCount(This,pctinfo) ) 

#define ErrorItems_GetTypeInfo(This,iTInfo,lcid,ppTInfo)	\
    ( (This)->lpVtbl -> GetTypeInfo(This,iTInfo,lcid,ppTInfo) ) 

#define ErrorItems_GetIDsOfNames(This,riid,rgszNames,cNames,lcid,rgDispId)	\
    ( (This)->lpVtbl -> GetIDsOfNames(This,riid,rgszNames,cNames,lcid,rgDispId) ) 

#define ErrorItems_Invoke(This,dispIdMember,riid,lcid,wFlags,pDispParams,pVarResult,pExcepInfo,puArgErr)	\
    ( (This)->lpVtbl -> Invoke(This,dispIdMember,riid,lcid,wFlags,pDispParams,pVarResult,pExcepInfo,puArgErr) ) 


#define ErrorItems_get_DTE(This,DTEObject)	\
    ( (This)->lpVtbl -> get_DTE(This,DTEObject) ) 

#define ErrorItems_get_Parent(This,ErrorList)	\
    ( (This)->lpVtbl -> get_Parent(This,ErrorList) ) 

#define ErrorItems_Item(This,index,ErrorItem)	\
    ( (This)->lpVtbl -> Item(This,index,ErrorItem) ) 

#define ErrorItems_get_Count(This,Count)	\
    ( (This)->lpVtbl -> get_Count(This,Count) ) 

#endif /* COBJMACROS */


#endif 	/* C style interface */




#endif 	/* __ErrorItems_INTERFACE_DEFINED__ */

#endif /* __EnvDTE80_LIBRARY_DEFINED__ */

/* Additional Prototypes for ALL interfaces */

/* end of Additional Prototypes */

#ifdef __cplusplus
}
#endif

#endif


