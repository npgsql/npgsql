

/* this ALWAYS GENERATED file contains the definitions for the interfaces */


 /* File created by MIDL compiler version 7.00.0499 */
/* Compiler settings for enc.idl:
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

#ifndef __enc_h__
#define __enc_h__

#if defined(_MSC_VER) && (_MSC_VER >= 1020)
#pragma once
#endif

/* Forward Declarations */ 

#ifndef __IDebugENC_FWD_DEFINED__
#define __IDebugENC_FWD_DEFINED__
typedef interface IDebugENC IDebugENC;
#endif 	/* __IDebugENC_FWD_DEFINED__ */


#ifndef __IDebugENC2_FWD_DEFINED__
#define __IDebugENC2_FWD_DEFINED__
typedef interface IDebugENC2 IDebugENC2;
#endif 	/* __IDebugENC2_FWD_DEFINED__ */


#ifndef __IDebugENCLineMap_FWD_DEFINED__
#define __IDebugENCLineMap_FWD_DEFINED__
typedef interface IDebugENCLineMap IDebugENCLineMap;
#endif 	/* __IDebugENCLineMap_FWD_DEFINED__ */


#ifndef __IDebugENCInfo2_FWD_DEFINED__
#define __IDebugENCInfo2_FWD_DEFINED__
typedef interface IDebugENCInfo2 IDebugENCInfo2;
#endif 	/* __IDebugENCInfo2_FWD_DEFINED__ */


#ifndef __IEnumDebugENCInfo2_FWD_DEFINED__
#define __IEnumDebugENCInfo2_FWD_DEFINED__
typedef interface IEnumDebugENCInfo2 IEnumDebugENCInfo2;
#endif 	/* __IEnumDebugENCInfo2_FWD_DEFINED__ */


#ifndef __IDebugENCRelinkInfo2_FWD_DEFINED__
#define __IDebugENCRelinkInfo2_FWD_DEFINED__
typedef interface IDebugENCRelinkInfo2 IDebugENCRelinkInfo2;
#endif 	/* __IDebugENCRelinkInfo2_FWD_DEFINED__ */


#ifndef __IEnumDebugENCRelinkInfo2_FWD_DEFINED__
#define __IEnumDebugENCRelinkInfo2_FWD_DEFINED__
typedef interface IEnumDebugENCRelinkInfo2 IEnumDebugENCRelinkInfo2;
#endif 	/* __IEnumDebugENCRelinkInfo2_FWD_DEFINED__ */


#ifndef __IDebugIDBInfo2_FWD_DEFINED__
#define __IDebugIDBInfo2_FWD_DEFINED__
typedef interface IDebugIDBInfo2 IDebugIDBInfo2;
#endif 	/* __IDebugIDBInfo2_FWD_DEFINED__ */


#ifndef __IEnumDebugIDBInfo2_FWD_DEFINED__
#define __IEnumDebugIDBInfo2_FWD_DEFINED__
typedef interface IEnumDebugIDBInfo2 IEnumDebugIDBInfo2;
#endif 	/* __IEnumDebugIDBInfo2_FWD_DEFINED__ */


#ifndef __IDebugENCBuildInfo2_FWD_DEFINED__
#define __IDebugENCBuildInfo2_FWD_DEFINED__
typedef interface IDebugENCBuildInfo2 IDebugENCBuildInfo2;
#endif 	/* __IDebugENCBuildInfo2_FWD_DEFINED__ */


#ifndef __IDebugENCUpdateOnRelinkEvent2_FWD_DEFINED__
#define __IDebugENCUpdateOnRelinkEvent2_FWD_DEFINED__
typedef interface IDebugENCUpdateOnRelinkEvent2 IDebugENCUpdateOnRelinkEvent2;
#endif 	/* __IDebugENCUpdateOnRelinkEvent2_FWD_DEFINED__ */


#ifndef __IDebugENCUpdateOnStaleCodeEvent2_FWD_DEFINED__
#define __IDebugENCUpdateOnStaleCodeEvent2_FWD_DEFINED__
typedef interface IDebugENCUpdateOnStaleCodeEvent2 IDebugENCUpdateOnStaleCodeEvent2;
#endif 	/* __IDebugENCUpdateOnStaleCodeEvent2_FWD_DEFINED__ */


#ifndef __IDebugENCUpdate_FWD_DEFINED__
#define __IDebugENCUpdate_FWD_DEFINED__
typedef interface IDebugENCUpdate IDebugENCUpdate;
#endif 	/* __IDebugENCUpdate_FWD_DEFINED__ */


#ifndef __IDebugENCSnapshot2_FWD_DEFINED__
#define __IDebugENCSnapshot2_FWD_DEFINED__
typedef interface IDebugENCSnapshot2 IDebugENCSnapshot2;
#endif 	/* __IDebugENCSnapshot2_FWD_DEFINED__ */


#ifndef __IDebugENCSnapshot3_FWD_DEFINED__
#define __IDebugENCSnapshot3_FWD_DEFINED__
typedef interface IDebugENCSnapshot3 IDebugENCSnapshot3;
#endif 	/* __IDebugENCSnapshot3_FWD_DEFINED__ */


#ifndef __IEnumDebugENCSnapshots2_FWD_DEFINED__
#define __IEnumDebugENCSnapshots2_FWD_DEFINED__
typedef interface IEnumDebugENCSnapshots2 IEnumDebugENCSnapshots2;
#endif 	/* __IEnumDebugENCSnapshots2_FWD_DEFINED__ */


#ifndef __IEnumDebugErrorInfos2_FWD_DEFINED__
#define __IEnumDebugErrorInfos2_FWD_DEFINED__
typedef interface IEnumDebugErrorInfos2 IEnumDebugErrorInfos2;
#endif 	/* __IEnumDebugErrorInfos2_FWD_DEFINED__ */


#ifndef __IENCDebugInfo_FWD_DEFINED__
#define __IENCDebugInfo_FWD_DEFINED__
typedef interface IENCDebugInfo IENCDebugInfo;
#endif 	/* __IENCDebugInfo_FWD_DEFINED__ */


#ifndef __IDebugENCSymbolProvider2_FWD_DEFINED__
#define __IDebugENCSymbolProvider2_FWD_DEFINED__
typedef interface IDebugENCSymbolProvider2 IDebugENCSymbolProvider2;
#endif 	/* __IDebugENCSymbolProvider2_FWD_DEFINED__ */


#ifndef __IDebugENCModule_FWD_DEFINED__
#define __IDebugENCModule_FWD_DEFINED__
typedef interface IDebugENCModule IDebugENCModule;
#endif 	/* __IDebugENCModule_FWD_DEFINED__ */


#ifndef __IDebugCustomENCModule100_FWD_DEFINED__
#define __IDebugCustomENCModule100_FWD_DEFINED__
typedef interface IDebugCustomENCModule100 IDebugCustomENCModule100;
#endif 	/* __IDebugCustomENCModule100_FWD_DEFINED__ */


#ifndef __IVsENCRebuildableProjectCfg3_FWD_DEFINED__
#define __IVsENCRebuildableProjectCfg3_FWD_DEFINED__
typedef interface IVsENCRebuildableProjectCfg3 IVsENCRebuildableProjectCfg3;
#endif 	/* __IVsENCRebuildableProjectCfg3_FWD_DEFINED__ */


#ifndef __IDebugManagedENC_FWD_DEFINED__
#define __IDebugManagedENC_FWD_DEFINED__
typedef interface IDebugManagedENC IDebugManagedENC;
#endif 	/* __IDebugManagedENC_FWD_DEFINED__ */


#ifndef __IDebugUpdateInMemoryPE_FWD_DEFINED__
#define __IDebugUpdateInMemoryPE_FWD_DEFINED__
typedef interface IDebugUpdateInMemoryPE IDebugUpdateInMemoryPE;
#endif 	/* __IDebugUpdateInMemoryPE_FWD_DEFINED__ */


#ifndef __IDebugComPlusSnapshot2_FWD_DEFINED__
#define __IDebugComPlusSnapshot2_FWD_DEFINED__
typedef interface IDebugComPlusSnapshot2 IDebugComPlusSnapshot2;
#endif 	/* __IDebugComPlusSnapshot2_FWD_DEFINED__ */


#ifndef __IDebugNativeSnapshot2_FWD_DEFINED__
#define __IDebugNativeSnapshot2_FWD_DEFINED__
typedef interface IDebugNativeSnapshot2 IDebugNativeSnapshot2;
#endif 	/* __IDebugNativeSnapshot2_FWD_DEFINED__ */


#ifndef __IDebugENCStackFrame2_FWD_DEFINED__
#define __IDebugENCStackFrame2_FWD_DEFINED__
typedef interface IDebugENCStackFrame2 IDebugENCStackFrame2;
#endif 	/* __IDebugENCStackFrame2_FWD_DEFINED__ */


#ifndef __IDebugMetaDataEmit2_FWD_DEFINED__
#define __IDebugMetaDataEmit2_FWD_DEFINED__
typedef interface IDebugMetaDataEmit2 IDebugMetaDataEmit2;
#endif 	/* __IDebugMetaDataEmit2_FWD_DEFINED__ */


#ifndef __IDebugMetaDataDebugEmit2_FWD_DEFINED__
#define __IDebugMetaDataDebugEmit2_FWD_DEFINED__
typedef interface IDebugMetaDataDebugEmit2 IDebugMetaDataDebugEmit2;
#endif 	/* __IDebugMetaDataDebugEmit2_FWD_DEFINED__ */


#ifndef __IDebugENCStateEvents_FWD_DEFINED__
#define __IDebugENCStateEvents_FWD_DEFINED__
typedef interface IDebugENCStateEvents IDebugENCStateEvents;
#endif 	/* __IDebugENCStateEvents_FWD_DEFINED__ */


#ifndef __EncMgr_FWD_DEFINED__
#define __EncMgr_FWD_DEFINED__

#ifdef __cplusplus
typedef class EncMgr EncMgr;
#else
typedef struct EncMgr EncMgr;
#endif /* __cplusplus */

#endif 	/* __EncMgr_FWD_DEFINED__ */


/* header files for imported files */
#include "ocidl.h"
#include "msdbg.h"
#include "sh.h"

#ifdef __cplusplus
extern "C"{
#endif 


/* interface __MIDL_itf_enc_0000_0000 */
/* [local] */ 





















static const int E_ENC_REBUILD_FAIL = MAKE_HRESULT(1, FACILITY_ITF, 0x0001);
static const int E_VB_ENC_REBUILD_FAIL = MAKE_HRESULT(1, FACILITY_ITF, 0x0011);
static const int E_ENC_REBUILD_FAIL_MODULE_NOT_LOADED = MAKE_HRESULT(1, FACILITY_ITF, 0x0101);
static const int E_ENC_COMMIT_FAIL = MAKE_HRESULT(1, FACILITY_ITF, 0x0002);
static const int E_ENC_MODIFIED_MODULE_RELOADED = MAKE_HRESULT(1, FACILITY_ITF, 0x0005);
static const int E_ENC_NOT_SUPPORTED_DURING_INTEROP = MAKE_HRESULT(1, FACILITY_ITF, 0x0006);
static const int E_ENC_NOT_SUPPORTED_DURING_SQLCLR = MAKE_HRESULT(1, FACILITY_ITF, 0x0007);
static const int E_ENC_NOT_SUPPORTED_DURING_EMBEDDED = MAKE_HRESULT(1, FACILITY_ITF, 0x0008);
static const int E_ENC_NOT_SUPPORTED_DURING_MINIDUMP = MAKE_HRESULT(1, FACILITY_ITF, 0x0009);
static const int E_ENC_NOT_SUPPORTED_FOR_ATTACH = MAKE_HRESULT(1, FACILITY_ITF, 0x0010);
static const int E_ENC_NOT_SUPPORTED_FOR_WIN64 = MAKE_HRESULT(1, FACILITY_ITF, 0x0011);
static const int E_ENC_NOT_SUPPORTED_UNTIL_MODULE_LOADED = MAKE_HRESULT(1, FACILITY_ITF, 0x0012);
static const int E_ENC_NOT_SUPPORTED_BUILD_FAILED = MAKE_HRESULT(1, FACILITY_ITF, 0x0015);
static const int E_ENC_NOT_SUPPORTED_FOR_SILVERLIGHT = MAKE_HRESULT(1, FACILITY_ITF, 0x0018);
static const int S_ENC_ILREMAP_UNREACHABLE = MAKE_HRESULT(0, FACILITY_ITF, 0x0013);
static const int E_ENC_NOT_SUPPORTED_IN_RUNMODE = MAKE_HRESULT(1, FACILITY_ITF, 0x0014);
static const int E_ENC_NOT_SUPPORTED_IN_STOPONEMODE = MAKE_HRESULT(1, FACILITY_ITF, 0x0016);
static const int E_ENC_NOT_SUPPORTED_FOR_REMOTE = MAKE_HRESULT(1, FACILITY_ITF, 0x0017);

enum tagENCSTATE
    {	ENCSTATE_DISABLED	= 0,
	ENCSTATE_ENABLED	= ( ENCSTATE_DISABLED + 1 ) ,
	ENCSTATE_MANAGED_ENC_NOT_SUPPORTED	= ( ENCSTATE_ENABLED + 1 ) 
    } ;
typedef enum tagENCSTATE ENCSTATE;


enum tagApplyCodeChangesResult
    {	ACCR_SUCCESS	= 0,
	ACCR_BUILDERROR	= ( ACCR_SUCCESS + 1 ) ,
	ACCR_CANCOMMITERROR	= ( ACCR_BUILDERROR + 1 ) ,
	ACCR_COMMITERROR	= ( ACCR_CANCOMMITERROR + 1 ) ,
	ACCR_MODRELOADERROR	= ( ACCR_COMMITERROR + 1 ) ,
	ACCR_ENCNOTSUPPORTED	= ( ACCR_MODRELOADERROR + 1 ) ,
	ACCR_MODNOTLOADEDERROR	= ( ACCR_ENCNOTSUPPORTED + 1 ) 
    } ;
typedef enum tagApplyCodeChangesResult ApplyCodeChangesResult;



extern RPC_IF_HANDLE __MIDL_itf_enc_0000_0000_v0_0_c_ifspec;
extern RPC_IF_HANDLE __MIDL_itf_enc_0000_0000_v0_0_s_ifspec;

#ifndef __IDebugENC_INTERFACE_DEFINED__
#define __IDebugENC_INTERFACE_DEFINED__

/* interface IDebugENC */
/* [unique][uuid][object] */ 


EXTERN_C const IID IID_IDebugENC;

#if defined(__cplusplus) && !defined(CINTERFACE)
    
    MIDL_INTERFACE("B104D8B7-AF19-11d2-922C-00A02448799A")
    IDebugENC : public IUnknown
    {
    public:
        virtual HRESULT STDMETHODCALLTYPE SetRegistryRoot( 
            /* [in] */ __RPC__in LPCOLESTR in_szRegistryRoot) = 0;
        
        virtual HRESULT STDMETHODCALLTYPE EnterDebuggingSession( 
            /* [in] */ __RPC__in_opt IServiceProvider *in_pServiceProvider) = 0;
        
        virtual HRESULT STDMETHODCALLTYPE SetENCProjectBuildOption( 
            /* [in] */ __RPC__in REFGUID in_guidOption,
            /* [in] */ __RPC__in LPCOLESTR in_szOptionValue) = 0;
        
        virtual HRESULT STDMETHODCALLTYPE InquireENCState( 
            /* [in] */ __RPC__in ENCSTATE *in_pENCSTATE,
            /* [in] */ BOOL fOnContinue) = 0;
        
        virtual HRESULT STDMETHODCALLTYPE InquireENCRelinkState( 
            /* [in] */ __RPC__in BOOL *in_pbENCRelinking) = 0;
        
        virtual HRESULT STDMETHODCALLTYPE MapToEdited( 
            /* [in] */ __RPC__in LPCOLESTR in_szFile,
            /* [in] */ ULONG in_LineNo,
            /* [in] */ ULONG in_ColumnNo,
            /* [out] */ __RPC__out ULONG *out_pLineNo,
            /* [out] */ __RPC__out ULONG *out_pColumnNo) = 0;
        
        virtual HRESULT STDMETHODCALLTYPE MapToSuperceded( 
            /* [in] */ __RPC__in LPCOLESTR in_szFile,
            /* [in] */ ULONG in_LineNo,
            /* [in] */ ULONG in_ColumnNo,
            /* [out] */ __RPC__out ULONG *out_pLineNo,
            /* [out] */ __RPC__out ULONG *out_pColumnNo) = 0;
        
        virtual HRESULT STDMETHODCALLTYPE ApplyCodeChanges( 
            /* [in] */ __RPC__in_opt IDebugSession2 *in_pSession,
            /* [in] */ BOOL in_fOnContinue,
            /* [out] */ __RPC__out ApplyCodeChangesResult *result) = 0;
        
        virtual HRESULT STDMETHODCALLTYPE CancelApplyCodeChanges( 
            /* [in] */ __RPC__in_opt IDebugProgram2 *in_pProgram) = 0;
        
        virtual HRESULT STDMETHODCALLTYPE LeaveDebuggingSession( void) = 0;
        
        virtual HRESULT STDMETHODCALLTYPE AdviseENCStateEvents( 
            /* [in] */ __RPC__in_opt IDebugENCStateEvents *in_pENCStateEvents) = 0;
        
        virtual HRESULT STDMETHODCALLTYPE UnadviseENCStateEvents( 
            /* [in] */ __RPC__in_opt IDebugENCStateEvents *in_pENCStateEvents) = 0;
        
        virtual HRESULT STDMETHODCALLTYPE GetFileName( 
            /* [in] */ __RPC__in LPCOLESTR in_szURL,
            /* [out] */ __RPC__deref_out_opt BSTR *out_pbstrFileName) = 0;
        
        virtual HRESULT STDMETHODCALLTYPE GetFileDisplayName( 
            /* [in] */ __RPC__in LPCOLESTR in_szURL,
            /* [out] */ __RPC__deref_out_opt BSTR *out_pbstrDisplayFileName) = 0;
        
        virtual HRESULT STDMETHODCALLTYPE ClearENCState( void) = 0;
        
    };
    
#else 	/* C style interface */

    typedef struct IDebugENCVtbl
    {
        BEGIN_INTERFACE
        
        HRESULT ( STDMETHODCALLTYPE *QueryInterface )( 
            IDebugENC * This,
            /* [in] */ __RPC__in REFIID riid,
            /* [iid_is][out] */ 
            __RPC__deref_out  void **ppvObject);
        
        ULONG ( STDMETHODCALLTYPE *AddRef )( 
            IDebugENC * This);
        
        ULONG ( STDMETHODCALLTYPE *Release )( 
            IDebugENC * This);
        
        HRESULT ( STDMETHODCALLTYPE *SetRegistryRoot )( 
            IDebugENC * This,
            /* [in] */ __RPC__in LPCOLESTR in_szRegistryRoot);
        
        HRESULT ( STDMETHODCALLTYPE *EnterDebuggingSession )( 
            IDebugENC * This,
            /* [in] */ __RPC__in_opt IServiceProvider *in_pServiceProvider);
        
        HRESULT ( STDMETHODCALLTYPE *SetENCProjectBuildOption )( 
            IDebugENC * This,
            /* [in] */ __RPC__in REFGUID in_guidOption,
            /* [in] */ __RPC__in LPCOLESTR in_szOptionValue);
        
        HRESULT ( STDMETHODCALLTYPE *InquireENCState )( 
            IDebugENC * This,
            /* [in] */ __RPC__in ENCSTATE *in_pENCSTATE,
            /* [in] */ BOOL fOnContinue);
        
        HRESULT ( STDMETHODCALLTYPE *InquireENCRelinkState )( 
            IDebugENC * This,
            /* [in] */ __RPC__in BOOL *in_pbENCRelinking);
        
        HRESULT ( STDMETHODCALLTYPE *MapToEdited )( 
            IDebugENC * This,
            /* [in] */ __RPC__in LPCOLESTR in_szFile,
            /* [in] */ ULONG in_LineNo,
            /* [in] */ ULONG in_ColumnNo,
            /* [out] */ __RPC__out ULONG *out_pLineNo,
            /* [out] */ __RPC__out ULONG *out_pColumnNo);
        
        HRESULT ( STDMETHODCALLTYPE *MapToSuperceded )( 
            IDebugENC * This,
            /* [in] */ __RPC__in LPCOLESTR in_szFile,
            /* [in] */ ULONG in_LineNo,
            /* [in] */ ULONG in_ColumnNo,
            /* [out] */ __RPC__out ULONG *out_pLineNo,
            /* [out] */ __RPC__out ULONG *out_pColumnNo);
        
        HRESULT ( STDMETHODCALLTYPE *ApplyCodeChanges )( 
            IDebugENC * This,
            /* [in] */ __RPC__in_opt IDebugSession2 *in_pSession,
            /* [in] */ BOOL in_fOnContinue,
            /* [out] */ __RPC__out ApplyCodeChangesResult *result);
        
        HRESULT ( STDMETHODCALLTYPE *CancelApplyCodeChanges )( 
            IDebugENC * This,
            /* [in] */ __RPC__in_opt IDebugProgram2 *in_pProgram);
        
        HRESULT ( STDMETHODCALLTYPE *LeaveDebuggingSession )( 
            IDebugENC * This);
        
        HRESULT ( STDMETHODCALLTYPE *AdviseENCStateEvents )( 
            IDebugENC * This,
            /* [in] */ __RPC__in_opt IDebugENCStateEvents *in_pENCStateEvents);
        
        HRESULT ( STDMETHODCALLTYPE *UnadviseENCStateEvents )( 
            IDebugENC * This,
            /* [in] */ __RPC__in_opt IDebugENCStateEvents *in_pENCStateEvents);
        
        HRESULT ( STDMETHODCALLTYPE *GetFileName )( 
            IDebugENC * This,
            /* [in] */ __RPC__in LPCOLESTR in_szURL,
            /* [out] */ __RPC__deref_out_opt BSTR *out_pbstrFileName);
        
        HRESULT ( STDMETHODCALLTYPE *GetFileDisplayName )( 
            IDebugENC * This,
            /* [in] */ __RPC__in LPCOLESTR in_szURL,
            /* [out] */ __RPC__deref_out_opt BSTR *out_pbstrDisplayFileName);
        
        HRESULT ( STDMETHODCALLTYPE *ClearENCState )( 
            IDebugENC * This);
        
        END_INTERFACE
    } IDebugENCVtbl;

    interface IDebugENC
    {
        CONST_VTBL struct IDebugENCVtbl *lpVtbl;
    };

    

#ifdef COBJMACROS


#define IDebugENC_QueryInterface(This,riid,ppvObject)	\
    ( (This)->lpVtbl -> QueryInterface(This,riid,ppvObject) ) 

#define IDebugENC_AddRef(This)	\
    ( (This)->lpVtbl -> AddRef(This) ) 

#define IDebugENC_Release(This)	\
    ( (This)->lpVtbl -> Release(This) ) 


#define IDebugENC_SetRegistryRoot(This,in_szRegistryRoot)	\
    ( (This)->lpVtbl -> SetRegistryRoot(This,in_szRegistryRoot) ) 

#define IDebugENC_EnterDebuggingSession(This,in_pServiceProvider)	\
    ( (This)->lpVtbl -> EnterDebuggingSession(This,in_pServiceProvider) ) 

#define IDebugENC_SetENCProjectBuildOption(This,in_guidOption,in_szOptionValue)	\
    ( (This)->lpVtbl -> SetENCProjectBuildOption(This,in_guidOption,in_szOptionValue) ) 

#define IDebugENC_InquireENCState(This,in_pENCSTATE,fOnContinue)	\
    ( (This)->lpVtbl -> InquireENCState(This,in_pENCSTATE,fOnContinue) ) 

#define IDebugENC_InquireENCRelinkState(This,in_pbENCRelinking)	\
    ( (This)->lpVtbl -> InquireENCRelinkState(This,in_pbENCRelinking) ) 

#define IDebugENC_MapToEdited(This,in_szFile,in_LineNo,in_ColumnNo,out_pLineNo,out_pColumnNo)	\
    ( (This)->lpVtbl -> MapToEdited(This,in_szFile,in_LineNo,in_ColumnNo,out_pLineNo,out_pColumnNo) ) 

#define IDebugENC_MapToSuperceded(This,in_szFile,in_LineNo,in_ColumnNo,out_pLineNo,out_pColumnNo)	\
    ( (This)->lpVtbl -> MapToSuperceded(This,in_szFile,in_LineNo,in_ColumnNo,out_pLineNo,out_pColumnNo) ) 

#define IDebugENC_ApplyCodeChanges(This,in_pSession,in_fOnContinue,result)	\
    ( (This)->lpVtbl -> ApplyCodeChanges(This,in_pSession,in_fOnContinue,result) ) 

#define IDebugENC_CancelApplyCodeChanges(This,in_pProgram)	\
    ( (This)->lpVtbl -> CancelApplyCodeChanges(This,in_pProgram) ) 

#define IDebugENC_LeaveDebuggingSession(This)	\
    ( (This)->lpVtbl -> LeaveDebuggingSession(This) ) 

#define IDebugENC_AdviseENCStateEvents(This,in_pENCStateEvents)	\
    ( (This)->lpVtbl -> AdviseENCStateEvents(This,in_pENCStateEvents) ) 

#define IDebugENC_UnadviseENCStateEvents(This,in_pENCStateEvents)	\
    ( (This)->lpVtbl -> UnadviseENCStateEvents(This,in_pENCStateEvents) ) 

#define IDebugENC_GetFileName(This,in_szURL,out_pbstrFileName)	\
    ( (This)->lpVtbl -> GetFileName(This,in_szURL,out_pbstrFileName) ) 

#define IDebugENC_GetFileDisplayName(This,in_szURL,out_pbstrDisplayFileName)	\
    ( (This)->lpVtbl -> GetFileDisplayName(This,in_szURL,out_pbstrDisplayFileName) ) 

#define IDebugENC_ClearENCState(This)	\
    ( (This)->lpVtbl -> ClearENCState(This) ) 

#endif /* COBJMACROS */


#endif 	/* C style interface */




#endif 	/* __IDebugENC_INTERFACE_DEFINED__ */


/* interface __MIDL_itf_enc_0000_0001 */
/* [local] */ 


enum tagEncApplyModel
    {	ENCAM_NONE	= 0,
	ENCAM_ANY	= ( ENCAM_NONE + 1 ) ,
	ENCAM_ONCONTINUE	= ( ENCAM_ANY + 1 ) 
    } ;
typedef enum tagEncApplyModel EncApplyModel;


enum tagEncEditState
    {	ENCES_NO_EDITS_PENDING	= 0,
	ENCES_VALID_EDITS	= ( ENCES_NO_EDITS_PENDING + 1 ) 
    } ;
typedef enum tagEncEditState EncEditState;


enum tagEncBreakReason
    {	ENCBR_NORMAL	= 0,
	ENCBR_EXCEPTION	= ( ENCBR_NORMAL + 1 ) 
    } ;
typedef enum tagEncBreakReason EncBreakReason;



extern RPC_IF_HANDLE __MIDL_itf_enc_0000_0001_v0_0_c_ifspec;
extern RPC_IF_HANDLE __MIDL_itf_enc_0000_0001_v0_0_s_ifspec;

#ifndef __IDebugENC2_INTERFACE_DEFINED__
#define __IDebugENC2_INTERFACE_DEFINED__

/* interface IDebugENC2 */
/* [unique][uuid][object] */ 


EXTERN_C const IID IID_IDebugENC2;

#if defined(__cplusplus) && !defined(CINTERFACE)
    
    MIDL_INTERFACE("54B61A02-4823-42ec-9648-A9AE80CDA270")
    IDebugENC2 : public IDebugENC
    {
    public:
        virtual HRESULT STDMETHODCALLTYPE GetENCApplyModel( 
            /* [in] */ __RPC__in EncApplyModel *pEncApplyModel) = 0;
        
        virtual HRESULT STDMETHODCALLTYPE GetEditState( 
            /* [in] */ __RPC__in EncEditState *pEditState) = 0;
        
        virtual HRESULT STDMETHODCALLTYPE CurrentPositionFromOriginal( 
            /* [in] */ __RPC__in LPCOLESTR in_szFile,
            /* [in] */ ULONG in_LineNo,
            /* [in] */ ULONG in_ColumnNo,
            /* [in] */ ULONG in_LineNoEnd,
            /* [in] */ ULONG in_ColumnNoEnd,
            /* [out] */ __RPC__out ULONG *out_pLineNo,
            /* [out] */ __RPC__out ULONG *out_pColumnNo,
            /* [out] */ __RPC__out ULONG *out_pLineNoEnd,
            /* [out] */ __RPC__out ULONG *out_pColumnNoEnd) = 0;
        
        virtual HRESULT STDMETHODCALLTYPE AreCodeContextsValid( 
            /* [in] */ __RPC__in LPCOLESTR in_szFile) = 0;
        
        virtual HRESULT STDMETHODCALLTYPE WaitForENCRelink( void) = 0;
        
        virtual HRESULT STDMETHODCALLTYPE EncCanIgnoreErrors( 
            /* [out] */ __RPC__out BOOL *pfCanIgnoreErrors) = 0;
        
        virtual HRESULT STDMETHODCALLTYPE SetLocale( 
            /* [in] */ WORD wLangID) = 0;
        
        virtual HRESULT STDMETHODCALLTYPE ApplyCodeChanges2( 
            /* [in] */ __RPC__in_opt IDebugSession2 *in_pSession,
            /* [in] */ BOOL in_fOnContinue,
            /* [in] */ ULONG ulLineHint,
            /* [in] */ ULONG ulColHint,
            /* [out] */ __RPC__out ApplyCodeChangesResult *result) = 0;
        
        virtual HRESULT STDMETHODCALLTYPE EnterBreakState( 
            /* [in] */ __RPC__in_opt IDebugSession2 *pProg,
            /* [in] */ EncBreakReason breakReason) = 0;
        
        virtual HRESULT STDMETHODCALLTYPE ExitBreakState( void) = 0;
        
        virtual HRESULT STDMETHODCALLTYPE SetStoppingModel( 
            /* [in] */ DWORD dwStoppingModel) = 0;
        
        virtual HRESULT STDMETHODCALLTYPE CheckEncAvailableForProject( 
            /* [in] */ __RPC__in_opt IDebugSession2 *pProg,
            /* [in] */ __RPC__in_opt IUnknown *pProject) = 0;
        
        virtual HRESULT STDMETHODCALLTYPE IsLeafRemapPossible( 
            /* [in] */ __RPC__in LPCOLESTR in_szFile,
            /* [in] */ ULONG in_LineNo,
            /* [in] */ ULONG in_ColumnNo,
            /* [in] */ ULONG in_LineNoEnd,
            /* [in] */ ULONG in_ColumnNoEnd) = 0;
        
    };
    
#else 	/* C style interface */

    typedef struct IDebugENC2Vtbl
    {
        BEGIN_INTERFACE
        
        HRESULT ( STDMETHODCALLTYPE *QueryInterface )( 
            IDebugENC2 * This,
            /* [in] */ __RPC__in REFIID riid,
            /* [iid_is][out] */ 
            __RPC__deref_out  void **ppvObject);
        
        ULONG ( STDMETHODCALLTYPE *AddRef )( 
            IDebugENC2 * This);
        
        ULONG ( STDMETHODCALLTYPE *Release )( 
            IDebugENC2 * This);
        
        HRESULT ( STDMETHODCALLTYPE *SetRegistryRoot )( 
            IDebugENC2 * This,
            /* [in] */ __RPC__in LPCOLESTR in_szRegistryRoot);
        
        HRESULT ( STDMETHODCALLTYPE *EnterDebuggingSession )( 
            IDebugENC2 * This,
            /* [in] */ __RPC__in_opt IServiceProvider *in_pServiceProvider);
        
        HRESULT ( STDMETHODCALLTYPE *SetENCProjectBuildOption )( 
            IDebugENC2 * This,
            /* [in] */ __RPC__in REFGUID in_guidOption,
            /* [in] */ __RPC__in LPCOLESTR in_szOptionValue);
        
        HRESULT ( STDMETHODCALLTYPE *InquireENCState )( 
            IDebugENC2 * This,
            /* [in] */ __RPC__in ENCSTATE *in_pENCSTATE,
            /* [in] */ BOOL fOnContinue);
        
        HRESULT ( STDMETHODCALLTYPE *InquireENCRelinkState )( 
            IDebugENC2 * This,
            /* [in] */ __RPC__in BOOL *in_pbENCRelinking);
        
        HRESULT ( STDMETHODCALLTYPE *MapToEdited )( 
            IDebugENC2 * This,
            /* [in] */ __RPC__in LPCOLESTR in_szFile,
            /* [in] */ ULONG in_LineNo,
            /* [in] */ ULONG in_ColumnNo,
            /* [out] */ __RPC__out ULONG *out_pLineNo,
            /* [out] */ __RPC__out ULONG *out_pColumnNo);
        
        HRESULT ( STDMETHODCALLTYPE *MapToSuperceded )( 
            IDebugENC2 * This,
            /* [in] */ __RPC__in LPCOLESTR in_szFile,
            /* [in] */ ULONG in_LineNo,
            /* [in] */ ULONG in_ColumnNo,
            /* [out] */ __RPC__out ULONG *out_pLineNo,
            /* [out] */ __RPC__out ULONG *out_pColumnNo);
        
        HRESULT ( STDMETHODCALLTYPE *ApplyCodeChanges )( 
            IDebugENC2 * This,
            /* [in] */ __RPC__in_opt IDebugSession2 *in_pSession,
            /* [in] */ BOOL in_fOnContinue,
            /* [out] */ __RPC__out ApplyCodeChangesResult *result);
        
        HRESULT ( STDMETHODCALLTYPE *CancelApplyCodeChanges )( 
            IDebugENC2 * This,
            /* [in] */ __RPC__in_opt IDebugProgram2 *in_pProgram);
        
        HRESULT ( STDMETHODCALLTYPE *LeaveDebuggingSession )( 
            IDebugENC2 * This);
        
        HRESULT ( STDMETHODCALLTYPE *AdviseENCStateEvents )( 
            IDebugENC2 * This,
            /* [in] */ __RPC__in_opt IDebugENCStateEvents *in_pENCStateEvents);
        
        HRESULT ( STDMETHODCALLTYPE *UnadviseENCStateEvents )( 
            IDebugENC2 * This,
            /* [in] */ __RPC__in_opt IDebugENCStateEvents *in_pENCStateEvents);
        
        HRESULT ( STDMETHODCALLTYPE *GetFileName )( 
            IDebugENC2 * This,
            /* [in] */ __RPC__in LPCOLESTR in_szURL,
            /* [out] */ __RPC__deref_out_opt BSTR *out_pbstrFileName);
        
        HRESULT ( STDMETHODCALLTYPE *GetFileDisplayName )( 
            IDebugENC2 * This,
            /* [in] */ __RPC__in LPCOLESTR in_szURL,
            /* [out] */ __RPC__deref_out_opt BSTR *out_pbstrDisplayFileName);
        
        HRESULT ( STDMETHODCALLTYPE *ClearENCState )( 
            IDebugENC2 * This);
        
        HRESULT ( STDMETHODCALLTYPE *GetENCApplyModel )( 
            IDebugENC2 * This,
            /* [in] */ __RPC__in EncApplyModel *pEncApplyModel);
        
        HRESULT ( STDMETHODCALLTYPE *GetEditState )( 
            IDebugENC2 * This,
            /* [in] */ __RPC__in EncEditState *pEditState);
        
        HRESULT ( STDMETHODCALLTYPE *CurrentPositionFromOriginal )( 
            IDebugENC2 * This,
            /* [in] */ __RPC__in LPCOLESTR in_szFile,
            /* [in] */ ULONG in_LineNo,
            /* [in] */ ULONG in_ColumnNo,
            /* [in] */ ULONG in_LineNoEnd,
            /* [in] */ ULONG in_ColumnNoEnd,
            /* [out] */ __RPC__out ULONG *out_pLineNo,
            /* [out] */ __RPC__out ULONG *out_pColumnNo,
            /* [out] */ __RPC__out ULONG *out_pLineNoEnd,
            /* [out] */ __RPC__out ULONG *out_pColumnNoEnd);
        
        HRESULT ( STDMETHODCALLTYPE *AreCodeContextsValid )( 
            IDebugENC2 * This,
            /* [in] */ __RPC__in LPCOLESTR in_szFile);
        
        HRESULT ( STDMETHODCALLTYPE *WaitForENCRelink )( 
            IDebugENC2 * This);
        
        HRESULT ( STDMETHODCALLTYPE *EncCanIgnoreErrors )( 
            IDebugENC2 * This,
            /* [out] */ __RPC__out BOOL *pfCanIgnoreErrors);
        
        HRESULT ( STDMETHODCALLTYPE *SetLocale )( 
            IDebugENC2 * This,
            /* [in] */ WORD wLangID);
        
        HRESULT ( STDMETHODCALLTYPE *ApplyCodeChanges2 )( 
            IDebugENC2 * This,
            /* [in] */ __RPC__in_opt IDebugSession2 *in_pSession,
            /* [in] */ BOOL in_fOnContinue,
            /* [in] */ ULONG ulLineHint,
            /* [in] */ ULONG ulColHint,
            /* [out] */ __RPC__out ApplyCodeChangesResult *result);
        
        HRESULT ( STDMETHODCALLTYPE *EnterBreakState )( 
            IDebugENC2 * This,
            /* [in] */ __RPC__in_opt IDebugSession2 *pProg,
            /* [in] */ EncBreakReason breakReason);
        
        HRESULT ( STDMETHODCALLTYPE *ExitBreakState )( 
            IDebugENC2 * This);
        
        HRESULT ( STDMETHODCALLTYPE *SetStoppingModel )( 
            IDebugENC2 * This,
            /* [in] */ DWORD dwStoppingModel);
        
        HRESULT ( STDMETHODCALLTYPE *CheckEncAvailableForProject )( 
            IDebugENC2 * This,
            /* [in] */ __RPC__in_opt IDebugSession2 *pProg,
            /* [in] */ __RPC__in_opt IUnknown *pProject);
        
        HRESULT ( STDMETHODCALLTYPE *IsLeafRemapPossible )( 
            IDebugENC2 * This,
            /* [in] */ __RPC__in LPCOLESTR in_szFile,
            /* [in] */ ULONG in_LineNo,
            /* [in] */ ULONG in_ColumnNo,
            /* [in] */ ULONG in_LineNoEnd,
            /* [in] */ ULONG in_ColumnNoEnd);
        
        END_INTERFACE
    } IDebugENC2Vtbl;

    interface IDebugENC2
    {
        CONST_VTBL struct IDebugENC2Vtbl *lpVtbl;
    };

    

#ifdef COBJMACROS


#define IDebugENC2_QueryInterface(This,riid,ppvObject)	\
    ( (This)->lpVtbl -> QueryInterface(This,riid,ppvObject) ) 

#define IDebugENC2_AddRef(This)	\
    ( (This)->lpVtbl -> AddRef(This) ) 

#define IDebugENC2_Release(This)	\
    ( (This)->lpVtbl -> Release(This) ) 


#define IDebugENC2_SetRegistryRoot(This,in_szRegistryRoot)	\
    ( (This)->lpVtbl -> SetRegistryRoot(This,in_szRegistryRoot) ) 

#define IDebugENC2_EnterDebuggingSession(This,in_pServiceProvider)	\
    ( (This)->lpVtbl -> EnterDebuggingSession(This,in_pServiceProvider) ) 

#define IDebugENC2_SetENCProjectBuildOption(This,in_guidOption,in_szOptionValue)	\
    ( (This)->lpVtbl -> SetENCProjectBuildOption(This,in_guidOption,in_szOptionValue) ) 

#define IDebugENC2_InquireENCState(This,in_pENCSTATE,fOnContinue)	\
    ( (This)->lpVtbl -> InquireENCState(This,in_pENCSTATE,fOnContinue) ) 

#define IDebugENC2_InquireENCRelinkState(This,in_pbENCRelinking)	\
    ( (This)->lpVtbl -> InquireENCRelinkState(This,in_pbENCRelinking) ) 

#define IDebugENC2_MapToEdited(This,in_szFile,in_LineNo,in_ColumnNo,out_pLineNo,out_pColumnNo)	\
    ( (This)->lpVtbl -> MapToEdited(This,in_szFile,in_LineNo,in_ColumnNo,out_pLineNo,out_pColumnNo) ) 

#define IDebugENC2_MapToSuperceded(This,in_szFile,in_LineNo,in_ColumnNo,out_pLineNo,out_pColumnNo)	\
    ( (This)->lpVtbl -> MapToSuperceded(This,in_szFile,in_LineNo,in_ColumnNo,out_pLineNo,out_pColumnNo) ) 

#define IDebugENC2_ApplyCodeChanges(This,in_pSession,in_fOnContinue,result)	\
    ( (This)->lpVtbl -> ApplyCodeChanges(This,in_pSession,in_fOnContinue,result) ) 

#define IDebugENC2_CancelApplyCodeChanges(This,in_pProgram)	\
    ( (This)->lpVtbl -> CancelApplyCodeChanges(This,in_pProgram) ) 

#define IDebugENC2_LeaveDebuggingSession(This)	\
    ( (This)->lpVtbl -> LeaveDebuggingSession(This) ) 

#define IDebugENC2_AdviseENCStateEvents(This,in_pENCStateEvents)	\
    ( (This)->lpVtbl -> AdviseENCStateEvents(This,in_pENCStateEvents) ) 

#define IDebugENC2_UnadviseENCStateEvents(This,in_pENCStateEvents)	\
    ( (This)->lpVtbl -> UnadviseENCStateEvents(This,in_pENCStateEvents) ) 

#define IDebugENC2_GetFileName(This,in_szURL,out_pbstrFileName)	\
    ( (This)->lpVtbl -> GetFileName(This,in_szURL,out_pbstrFileName) ) 

#define IDebugENC2_GetFileDisplayName(This,in_szURL,out_pbstrDisplayFileName)	\
    ( (This)->lpVtbl -> GetFileDisplayName(This,in_szURL,out_pbstrDisplayFileName) ) 

#define IDebugENC2_ClearENCState(This)	\
    ( (This)->lpVtbl -> ClearENCState(This) ) 


#define IDebugENC2_GetENCApplyModel(This,pEncApplyModel)	\
    ( (This)->lpVtbl -> GetENCApplyModel(This,pEncApplyModel) ) 

#define IDebugENC2_GetEditState(This,pEditState)	\
    ( (This)->lpVtbl -> GetEditState(This,pEditState) ) 

#define IDebugENC2_CurrentPositionFromOriginal(This,in_szFile,in_LineNo,in_ColumnNo,in_LineNoEnd,in_ColumnNoEnd,out_pLineNo,out_pColumnNo,out_pLineNoEnd,out_pColumnNoEnd)	\
    ( (This)->lpVtbl -> CurrentPositionFromOriginal(This,in_szFile,in_LineNo,in_ColumnNo,in_LineNoEnd,in_ColumnNoEnd,out_pLineNo,out_pColumnNo,out_pLineNoEnd,out_pColumnNoEnd) ) 

#define IDebugENC2_AreCodeContextsValid(This,in_szFile)	\
    ( (This)->lpVtbl -> AreCodeContextsValid(This,in_szFile) ) 

#define IDebugENC2_WaitForENCRelink(This)	\
    ( (This)->lpVtbl -> WaitForENCRelink(This) ) 

#define IDebugENC2_EncCanIgnoreErrors(This,pfCanIgnoreErrors)	\
    ( (This)->lpVtbl -> EncCanIgnoreErrors(This,pfCanIgnoreErrors) ) 

#define IDebugENC2_SetLocale(This,wLangID)	\
    ( (This)->lpVtbl -> SetLocale(This,wLangID) ) 

#define IDebugENC2_ApplyCodeChanges2(This,in_pSession,in_fOnContinue,ulLineHint,ulColHint,result)	\
    ( (This)->lpVtbl -> ApplyCodeChanges2(This,in_pSession,in_fOnContinue,ulLineHint,ulColHint,result) ) 

#define IDebugENC2_EnterBreakState(This,pProg,breakReason)	\
    ( (This)->lpVtbl -> EnterBreakState(This,pProg,breakReason) ) 

#define IDebugENC2_ExitBreakState(This)	\
    ( (This)->lpVtbl -> ExitBreakState(This) ) 

#define IDebugENC2_SetStoppingModel(This,dwStoppingModel)	\
    ( (This)->lpVtbl -> SetStoppingModel(This,dwStoppingModel) ) 

#define IDebugENC2_CheckEncAvailableForProject(This,pProg,pProject)	\
    ( (This)->lpVtbl -> CheckEncAvailableForProject(This,pProg,pProject) ) 

#define IDebugENC2_IsLeafRemapPossible(This,in_szFile,in_LineNo,in_ColumnNo,in_LineNoEnd,in_ColumnNoEnd)	\
    ( (This)->lpVtbl -> IsLeafRemapPossible(This,in_szFile,in_LineNo,in_ColumnNo,in_LineNoEnd,in_ColumnNoEnd) ) 

#endif /* COBJMACROS */


#endif 	/* C style interface */




#endif 	/* __IDebugENC2_INTERFACE_DEFINED__ */


#ifndef __IDebugENCLineMap_INTERFACE_DEFINED__
#define __IDebugENCLineMap_INTERFACE_DEFINED__

/* interface IDebugENCLineMap */
/* [unique][uuid][object] */ 


EXTERN_C const IID IID_IDebugENCLineMap;

#if defined(__cplusplus) && !defined(CINTERFACE)
    
    MIDL_INTERFACE("8706233B-BD4C-11d2-9238-00A02448799A")
    IDebugENCLineMap : public IUnknown
    {
    public:
        virtual HRESULT STDMETHODCALLTYPE GetEditedSource( 
            /* [out] */ __RPC__deref_out_opt BSTR *out_pbstrEditedSource) = 0;
        
        virtual HRESULT STDMETHODCALLTYPE GetSupercededSource( 
            /* [out] */ __RPC__deref_out_opt BSTR *out_pbstrSupercededSource) = 0;
        
        virtual HRESULT STDMETHODCALLTYPE IsLineModified( 
            /* [in] */ ULONG in_LineNoFromSupercededSource) = 0;
        
        virtual HRESULT STDMETHODCALLTYPE LineMap( 
            /* [in] */ ULONG in_LineNoFromSupercededSource,
            /* [out] */ __RPC__out ULONG *out_pLineNoFromEditedSource) = 0;
        
        virtual HRESULT STDMETHODCALLTYPE ReverseLineMap( 
            /* [in] */ ULONG in_LineNoFromEditedSource,
            /* [out] */ __RPC__out ULONG *out_pLineNoFromSupercededSource) = 0;
        
    };
    
#else 	/* C style interface */

    typedef struct IDebugENCLineMapVtbl
    {
        BEGIN_INTERFACE
        
        HRESULT ( STDMETHODCALLTYPE *QueryInterface )( 
            IDebugENCLineMap * This,
            /* [in] */ __RPC__in REFIID riid,
            /* [iid_is][out] */ 
            __RPC__deref_out  void **ppvObject);
        
        ULONG ( STDMETHODCALLTYPE *AddRef )( 
            IDebugENCLineMap * This);
        
        ULONG ( STDMETHODCALLTYPE *Release )( 
            IDebugENCLineMap * This);
        
        HRESULT ( STDMETHODCALLTYPE *GetEditedSource )( 
            IDebugENCLineMap * This,
            /* [out] */ __RPC__deref_out_opt BSTR *out_pbstrEditedSource);
        
        HRESULT ( STDMETHODCALLTYPE *GetSupercededSource )( 
            IDebugENCLineMap * This,
            /* [out] */ __RPC__deref_out_opt BSTR *out_pbstrSupercededSource);
        
        HRESULT ( STDMETHODCALLTYPE *IsLineModified )( 
            IDebugENCLineMap * This,
            /* [in] */ ULONG in_LineNoFromSupercededSource);
        
        HRESULT ( STDMETHODCALLTYPE *LineMap )( 
            IDebugENCLineMap * This,
            /* [in] */ ULONG in_LineNoFromSupercededSource,
            /* [out] */ __RPC__out ULONG *out_pLineNoFromEditedSource);
        
        HRESULT ( STDMETHODCALLTYPE *ReverseLineMap )( 
            IDebugENCLineMap * This,
            /* [in] */ ULONG in_LineNoFromEditedSource,
            /* [out] */ __RPC__out ULONG *out_pLineNoFromSupercededSource);
        
        END_INTERFACE
    } IDebugENCLineMapVtbl;

    interface IDebugENCLineMap
    {
        CONST_VTBL struct IDebugENCLineMapVtbl *lpVtbl;
    };

    

#ifdef COBJMACROS


#define IDebugENCLineMap_QueryInterface(This,riid,ppvObject)	\
    ( (This)->lpVtbl -> QueryInterface(This,riid,ppvObject) ) 

#define IDebugENCLineMap_AddRef(This)	\
    ( (This)->lpVtbl -> AddRef(This) ) 

#define IDebugENCLineMap_Release(This)	\
    ( (This)->lpVtbl -> Release(This) ) 


#define IDebugENCLineMap_GetEditedSource(This,out_pbstrEditedSource)	\
    ( (This)->lpVtbl -> GetEditedSource(This,out_pbstrEditedSource) ) 

#define IDebugENCLineMap_GetSupercededSource(This,out_pbstrSupercededSource)	\
    ( (This)->lpVtbl -> GetSupercededSource(This,out_pbstrSupercededSource) ) 

#define IDebugENCLineMap_IsLineModified(This,in_LineNoFromSupercededSource)	\
    ( (This)->lpVtbl -> IsLineModified(This,in_LineNoFromSupercededSource) ) 

#define IDebugENCLineMap_LineMap(This,in_LineNoFromSupercededSource,out_pLineNoFromEditedSource)	\
    ( (This)->lpVtbl -> LineMap(This,in_LineNoFromSupercededSource,out_pLineNoFromEditedSource) ) 

#define IDebugENCLineMap_ReverseLineMap(This,in_LineNoFromEditedSource,out_pLineNoFromSupercededSource)	\
    ( (This)->lpVtbl -> ReverseLineMap(This,in_LineNoFromEditedSource,out_pLineNoFromSupercededSource) ) 

#endif /* COBJMACROS */


#endif 	/* C style interface */




#endif 	/* __IDebugENCLineMap_INTERFACE_DEFINED__ */


/* interface __MIDL_itf_enc_0000_0003 */
/* [local] */ 


enum __MIDL___MIDL_itf_enc_0000_0003_0001
    {	ENCINFO_STACKFRAME	= 0x1,
	ENCINFO_HRESULT_FROM_DE	= 0x2,
	ENCINFO_ERROR_NO	= 0x4,
	ENCINFO_ERROR_BSTR	= 0x8,
	ENCINFO_CODE_CONTEXT	= 0x10,
	ENCINFO_EXTENDED_INFO	= 0x20
    } ;
typedef DWORD ENCINFO_FLAGS;

typedef struct tagENCINFO
    {
    ENCINFO_FLAGS m_dwValidFields;
    IDebugStackFrame2 *m_pStackFrame;
    HRESULT m_HRFromDE;
    DWORD m_dwErrorNo;
    BSTR m_bstrError;
    IDebugCodeContext2 *m_pCodeContext;
    IUnknown *m_pExtendedInfo;
    } 	ENCINFO;



extern RPC_IF_HANDLE __MIDL_itf_enc_0000_0003_v0_0_c_ifspec;
extern RPC_IF_HANDLE __MIDL_itf_enc_0000_0003_v0_0_s_ifspec;

#ifndef __IDebugENCInfo2_INTERFACE_DEFINED__
#define __IDebugENCInfo2_INTERFACE_DEFINED__

/* interface IDebugENCInfo2 */
/* [unique][uuid][object] */ 


EXTERN_C const IID IID_IDebugENCInfo2;

#if defined(__cplusplus) && !defined(CINTERFACE)
    
    MIDL_INTERFACE("6B56106F-BD51-11d2-9238-00A02448799A")
    IDebugENCInfo2 : public IUnknown
    {
    public:
        virtual HRESULT STDMETHODCALLTYPE GetInfo( 
            /* [out] */ __RPC__out ENCINFO *out_pENCINFO) = 0;
        
    };
    
#else 	/* C style interface */

    typedef struct IDebugENCInfo2Vtbl
    {
        BEGIN_INTERFACE
        
        HRESULT ( STDMETHODCALLTYPE *QueryInterface )( 
            IDebugENCInfo2 * This,
            /* [in] */ __RPC__in REFIID riid,
            /* [iid_is][out] */ 
            __RPC__deref_out  void **ppvObject);
        
        ULONG ( STDMETHODCALLTYPE *AddRef )( 
            IDebugENCInfo2 * This);
        
        ULONG ( STDMETHODCALLTYPE *Release )( 
            IDebugENCInfo2 * This);
        
        HRESULT ( STDMETHODCALLTYPE *GetInfo )( 
            IDebugENCInfo2 * This,
            /* [out] */ __RPC__out ENCINFO *out_pENCINFO);
        
        END_INTERFACE
    } IDebugENCInfo2Vtbl;

    interface IDebugENCInfo2
    {
        CONST_VTBL struct IDebugENCInfo2Vtbl *lpVtbl;
    };

    

#ifdef COBJMACROS


#define IDebugENCInfo2_QueryInterface(This,riid,ppvObject)	\
    ( (This)->lpVtbl -> QueryInterface(This,riid,ppvObject) ) 

#define IDebugENCInfo2_AddRef(This)	\
    ( (This)->lpVtbl -> AddRef(This) ) 

#define IDebugENCInfo2_Release(This)	\
    ( (This)->lpVtbl -> Release(This) ) 


#define IDebugENCInfo2_GetInfo(This,out_pENCINFO)	\
    ( (This)->lpVtbl -> GetInfo(This,out_pENCINFO) ) 

#endif /* COBJMACROS */


#endif 	/* C style interface */




#endif 	/* __IDebugENCInfo2_INTERFACE_DEFINED__ */


#ifndef __IEnumDebugENCInfo2_INTERFACE_DEFINED__
#define __IEnumDebugENCInfo2_INTERFACE_DEFINED__

/* interface IEnumDebugENCInfo2 */
/* [unique][uuid][object] */ 


EXTERN_C const IID IID_IEnumDebugENCInfo2;

#if defined(__cplusplus) && !defined(CINTERFACE)
    
    MIDL_INTERFACE("7B076AD1-BD51-11d2-9238-00A02448799A")
    IEnumDebugENCInfo2 : public IUnknown
    {
    public:
        virtual HRESULT STDMETHODCALLTYPE Next( 
            /* [in] */ ULONG in_NoOfElementsRequested,
            /* [length_is][size_is][out] */ __RPC__out_ecount_part(in_NoOfElementsRequested, *out_pNoOfElementsFetched) IDebugENCInfo2 **out_ArrayOfpENCInfo,
            /* [out] */ __RPC__out ULONG *out_pNoOfElementsFetched) = 0;
        
        virtual HRESULT STDMETHODCALLTYPE Skip( 
            /* [in] */ ULONG in_NoOfElements) = 0;
        
        virtual HRESULT STDMETHODCALLTYPE Reset( void) = 0;
        
        virtual HRESULT STDMETHODCALLTYPE Clone( 
            /* [out] */ __RPC__deref_out_opt IEnumDebugENCInfo2 **out_ppEnumENCInfo) = 0;
        
        virtual HRESULT STDMETHODCALLTYPE GetCount( 
            /* [out] */ __RPC__out ULONG *out_pCount) = 0;
        
    };
    
#else 	/* C style interface */

    typedef struct IEnumDebugENCInfo2Vtbl
    {
        BEGIN_INTERFACE
        
        HRESULT ( STDMETHODCALLTYPE *QueryInterface )( 
            IEnumDebugENCInfo2 * This,
            /* [in] */ __RPC__in REFIID riid,
            /* [iid_is][out] */ 
            __RPC__deref_out  void **ppvObject);
        
        ULONG ( STDMETHODCALLTYPE *AddRef )( 
            IEnumDebugENCInfo2 * This);
        
        ULONG ( STDMETHODCALLTYPE *Release )( 
            IEnumDebugENCInfo2 * This);
        
        HRESULT ( STDMETHODCALLTYPE *Next )( 
            IEnumDebugENCInfo2 * This,
            /* [in] */ ULONG in_NoOfElementsRequested,
            /* [length_is][size_is][out] */ __RPC__out_ecount_part(in_NoOfElementsRequested, *out_pNoOfElementsFetched) IDebugENCInfo2 **out_ArrayOfpENCInfo,
            /* [out] */ __RPC__out ULONG *out_pNoOfElementsFetched);
        
        HRESULT ( STDMETHODCALLTYPE *Skip )( 
            IEnumDebugENCInfo2 * This,
            /* [in] */ ULONG in_NoOfElements);
        
        HRESULT ( STDMETHODCALLTYPE *Reset )( 
            IEnumDebugENCInfo2 * This);
        
        HRESULT ( STDMETHODCALLTYPE *Clone )( 
            IEnumDebugENCInfo2 * This,
            /* [out] */ __RPC__deref_out_opt IEnumDebugENCInfo2 **out_ppEnumENCInfo);
        
        HRESULT ( STDMETHODCALLTYPE *GetCount )( 
            IEnumDebugENCInfo2 * This,
            /* [out] */ __RPC__out ULONG *out_pCount);
        
        END_INTERFACE
    } IEnumDebugENCInfo2Vtbl;

    interface IEnumDebugENCInfo2
    {
        CONST_VTBL struct IEnumDebugENCInfo2Vtbl *lpVtbl;
    };

    

#ifdef COBJMACROS


#define IEnumDebugENCInfo2_QueryInterface(This,riid,ppvObject)	\
    ( (This)->lpVtbl -> QueryInterface(This,riid,ppvObject) ) 

#define IEnumDebugENCInfo2_AddRef(This)	\
    ( (This)->lpVtbl -> AddRef(This) ) 

#define IEnumDebugENCInfo2_Release(This)	\
    ( (This)->lpVtbl -> Release(This) ) 


#define IEnumDebugENCInfo2_Next(This,in_NoOfElementsRequested,out_ArrayOfpENCInfo,out_pNoOfElementsFetched)	\
    ( (This)->lpVtbl -> Next(This,in_NoOfElementsRequested,out_ArrayOfpENCInfo,out_pNoOfElementsFetched) ) 

#define IEnumDebugENCInfo2_Skip(This,in_NoOfElements)	\
    ( (This)->lpVtbl -> Skip(This,in_NoOfElements) ) 

#define IEnumDebugENCInfo2_Reset(This)	\
    ( (This)->lpVtbl -> Reset(This) ) 

#define IEnumDebugENCInfo2_Clone(This,out_ppEnumENCInfo)	\
    ( (This)->lpVtbl -> Clone(This,out_ppEnumENCInfo) ) 

#define IEnumDebugENCInfo2_GetCount(This,out_pCount)	\
    ( (This)->lpVtbl -> GetCount(This,out_pCount) ) 

#endif /* COBJMACROS */


#endif 	/* C style interface */




#endif 	/* __IEnumDebugENCInfo2_INTERFACE_DEFINED__ */


#ifndef __IDebugENCRelinkInfo2_INTERFACE_DEFINED__
#define __IDebugENCRelinkInfo2_INTERFACE_DEFINED__

/* interface IDebugENCRelinkInfo2 */
/* [unique][uuid][object] */ 


EXTERN_C const IID IID_IDebugENCRelinkInfo2;

#if defined(__cplusplus) && !defined(CINTERFACE)
    
    MIDL_INTERFACE("CBB63A8D-BD57-11d2-9238-00A02448799A")
    IDebugENCRelinkInfo2 : public IUnknown
    {
    public:
        virtual HRESULT STDMETHODCALLTYPE GetInfo( 
            /* [out] */ __RPC__deref_out_opt BSTR *out_pbstrWorkingDir,
            /* [out] */ __RPC__deref_out_opt BSTR *out_pbstrCommand,
            /* [out] */ __RPC__deref_out_opt BSTR *out_pbstrOutFile,
            /* [out] */ __RPC__deref_out_opt BSTR *out_pbstrDebugFile,
            /* [out] */ __RPC__out BOOL *out_pbEditFromLib) = 0;
        
    };
    
#else 	/* C style interface */

    typedef struct IDebugENCRelinkInfo2Vtbl
    {
        BEGIN_INTERFACE
        
        HRESULT ( STDMETHODCALLTYPE *QueryInterface )( 
            IDebugENCRelinkInfo2 * This,
            /* [in] */ __RPC__in REFIID riid,
            /* [iid_is][out] */ 
            __RPC__deref_out  void **ppvObject);
        
        ULONG ( STDMETHODCALLTYPE *AddRef )( 
            IDebugENCRelinkInfo2 * This);
        
        ULONG ( STDMETHODCALLTYPE *Release )( 
            IDebugENCRelinkInfo2 * This);
        
        HRESULT ( STDMETHODCALLTYPE *GetInfo )( 
            IDebugENCRelinkInfo2 * This,
            /* [out] */ __RPC__deref_out_opt BSTR *out_pbstrWorkingDir,
            /* [out] */ __RPC__deref_out_opt BSTR *out_pbstrCommand,
            /* [out] */ __RPC__deref_out_opt BSTR *out_pbstrOutFile,
            /* [out] */ __RPC__deref_out_opt BSTR *out_pbstrDebugFile,
            /* [out] */ __RPC__out BOOL *out_pbEditFromLib);
        
        END_INTERFACE
    } IDebugENCRelinkInfo2Vtbl;

    interface IDebugENCRelinkInfo2
    {
        CONST_VTBL struct IDebugENCRelinkInfo2Vtbl *lpVtbl;
    };

    

#ifdef COBJMACROS


#define IDebugENCRelinkInfo2_QueryInterface(This,riid,ppvObject)	\
    ( (This)->lpVtbl -> QueryInterface(This,riid,ppvObject) ) 

#define IDebugENCRelinkInfo2_AddRef(This)	\
    ( (This)->lpVtbl -> AddRef(This) ) 

#define IDebugENCRelinkInfo2_Release(This)	\
    ( (This)->lpVtbl -> Release(This) ) 


#define IDebugENCRelinkInfo2_GetInfo(This,out_pbstrWorkingDir,out_pbstrCommand,out_pbstrOutFile,out_pbstrDebugFile,out_pbEditFromLib)	\
    ( (This)->lpVtbl -> GetInfo(This,out_pbstrWorkingDir,out_pbstrCommand,out_pbstrOutFile,out_pbstrDebugFile,out_pbEditFromLib) ) 

#endif /* COBJMACROS */


#endif 	/* C style interface */




#endif 	/* __IDebugENCRelinkInfo2_INTERFACE_DEFINED__ */


#ifndef __IEnumDebugENCRelinkInfo2_INTERFACE_DEFINED__
#define __IEnumDebugENCRelinkInfo2_INTERFACE_DEFINED__

/* interface IEnumDebugENCRelinkInfo2 */
/* [unique][uuid][object] */ 


EXTERN_C const IID IID_IEnumDebugENCRelinkInfo2;

#if defined(__cplusplus) && !defined(CINTERFACE)
    
    MIDL_INTERFACE("E51BE743-BD57-11d2-9238-00A02448799A")
    IEnumDebugENCRelinkInfo2 : public IUnknown
    {
    public:
        virtual HRESULT STDMETHODCALLTYPE Next( 
            /* [in] */ ULONG in_NoOfElementsRequested,
            /* [length_is][size_is][out] */ __RPC__out_ecount_part(in_NoOfElementsRequested, *out_pNoOfElementsFetched) IDebugENCRelinkInfo2 **out_ArrayOfpENCInfo,
            /* [out] */ __RPC__out ULONG *out_pNoOfElementsFetched) = 0;
        
        virtual HRESULT STDMETHODCALLTYPE Skip( 
            /* [in] */ ULONG in_NoOfElements) = 0;
        
        virtual HRESULT STDMETHODCALLTYPE Reset( void) = 0;
        
        virtual HRESULT STDMETHODCALLTYPE Clone( 
            /* [out] */ __RPC__deref_out_opt IEnumDebugENCRelinkInfo2 **out_ppEnumENCRelinkInfo) = 0;
        
        virtual HRESULT STDMETHODCALLTYPE GetCount( 
            /* [out] */ __RPC__out ULONG *out_pCount) = 0;
        
    };
    
#else 	/* C style interface */

    typedef struct IEnumDebugENCRelinkInfo2Vtbl
    {
        BEGIN_INTERFACE
        
        HRESULT ( STDMETHODCALLTYPE *QueryInterface )( 
            IEnumDebugENCRelinkInfo2 * This,
            /* [in] */ __RPC__in REFIID riid,
            /* [iid_is][out] */ 
            __RPC__deref_out  void **ppvObject);
        
        ULONG ( STDMETHODCALLTYPE *AddRef )( 
            IEnumDebugENCRelinkInfo2 * This);
        
        ULONG ( STDMETHODCALLTYPE *Release )( 
            IEnumDebugENCRelinkInfo2 * This);
        
        HRESULT ( STDMETHODCALLTYPE *Next )( 
            IEnumDebugENCRelinkInfo2 * This,
            /* [in] */ ULONG in_NoOfElementsRequested,
            /* [length_is][size_is][out] */ __RPC__out_ecount_part(in_NoOfElementsRequested, *out_pNoOfElementsFetched) IDebugENCRelinkInfo2 **out_ArrayOfpENCInfo,
            /* [out] */ __RPC__out ULONG *out_pNoOfElementsFetched);
        
        HRESULT ( STDMETHODCALLTYPE *Skip )( 
            IEnumDebugENCRelinkInfo2 * This,
            /* [in] */ ULONG in_NoOfElements);
        
        HRESULT ( STDMETHODCALLTYPE *Reset )( 
            IEnumDebugENCRelinkInfo2 * This);
        
        HRESULT ( STDMETHODCALLTYPE *Clone )( 
            IEnumDebugENCRelinkInfo2 * This,
            /* [out] */ __RPC__deref_out_opt IEnumDebugENCRelinkInfo2 **out_ppEnumENCRelinkInfo);
        
        HRESULT ( STDMETHODCALLTYPE *GetCount )( 
            IEnumDebugENCRelinkInfo2 * This,
            /* [out] */ __RPC__out ULONG *out_pCount);
        
        END_INTERFACE
    } IEnumDebugENCRelinkInfo2Vtbl;

    interface IEnumDebugENCRelinkInfo2
    {
        CONST_VTBL struct IEnumDebugENCRelinkInfo2Vtbl *lpVtbl;
    };

    

#ifdef COBJMACROS


#define IEnumDebugENCRelinkInfo2_QueryInterface(This,riid,ppvObject)	\
    ( (This)->lpVtbl -> QueryInterface(This,riid,ppvObject) ) 

#define IEnumDebugENCRelinkInfo2_AddRef(This)	\
    ( (This)->lpVtbl -> AddRef(This) ) 

#define IEnumDebugENCRelinkInfo2_Release(This)	\
    ( (This)->lpVtbl -> Release(This) ) 


#define IEnumDebugENCRelinkInfo2_Next(This,in_NoOfElementsRequested,out_ArrayOfpENCInfo,out_pNoOfElementsFetched)	\
    ( (This)->lpVtbl -> Next(This,in_NoOfElementsRequested,out_ArrayOfpENCInfo,out_pNoOfElementsFetched) ) 

#define IEnumDebugENCRelinkInfo2_Skip(This,in_NoOfElements)	\
    ( (This)->lpVtbl -> Skip(This,in_NoOfElements) ) 

#define IEnumDebugENCRelinkInfo2_Reset(This)	\
    ( (This)->lpVtbl -> Reset(This) ) 

#define IEnumDebugENCRelinkInfo2_Clone(This,out_ppEnumENCRelinkInfo)	\
    ( (This)->lpVtbl -> Clone(This,out_ppEnumENCRelinkInfo) ) 

#define IEnumDebugENCRelinkInfo2_GetCount(This,out_pCount)	\
    ( (This)->lpVtbl -> GetCount(This,out_pCount) ) 

#endif /* COBJMACROS */


#endif 	/* C style interface */




#endif 	/* __IEnumDebugENCRelinkInfo2_INTERFACE_DEFINED__ */


#ifndef __IDebugIDBInfo2_INTERFACE_DEFINED__
#define __IDebugIDBInfo2_INTERFACE_DEFINED__

/* interface IDebugIDBInfo2 */
/* [unique][uuid][object] */ 


EXTERN_C const IID IID_IDebugIDBInfo2;

#if defined(__cplusplus) && !defined(CINTERFACE)
    
    MIDL_INTERFACE("9B7DE9A9-BD59-11d2-9238-00A02448799A")
    IDebugIDBInfo2 : public IUnknown
    {
    public:
        virtual HRESULT STDMETHODCALLTYPE GetInfo( 
            /* [out] */ __RPC__deref_out_opt BSTR *out_pbstrIDBFile) = 0;
        
    };
    
#else 	/* C style interface */

    typedef struct IDebugIDBInfo2Vtbl
    {
        BEGIN_INTERFACE
        
        HRESULT ( STDMETHODCALLTYPE *QueryInterface )( 
            IDebugIDBInfo2 * This,
            /* [in] */ __RPC__in REFIID riid,
            /* [iid_is][out] */ 
            __RPC__deref_out  void **ppvObject);
        
        ULONG ( STDMETHODCALLTYPE *AddRef )( 
            IDebugIDBInfo2 * This);
        
        ULONG ( STDMETHODCALLTYPE *Release )( 
            IDebugIDBInfo2 * This);
        
        HRESULT ( STDMETHODCALLTYPE *GetInfo )( 
            IDebugIDBInfo2 * This,
            /* [out] */ __RPC__deref_out_opt BSTR *out_pbstrIDBFile);
        
        END_INTERFACE
    } IDebugIDBInfo2Vtbl;

    interface IDebugIDBInfo2
    {
        CONST_VTBL struct IDebugIDBInfo2Vtbl *lpVtbl;
    };

    

#ifdef COBJMACROS


#define IDebugIDBInfo2_QueryInterface(This,riid,ppvObject)	\
    ( (This)->lpVtbl -> QueryInterface(This,riid,ppvObject) ) 

#define IDebugIDBInfo2_AddRef(This)	\
    ( (This)->lpVtbl -> AddRef(This) ) 

#define IDebugIDBInfo2_Release(This)	\
    ( (This)->lpVtbl -> Release(This) ) 


#define IDebugIDBInfo2_GetInfo(This,out_pbstrIDBFile)	\
    ( (This)->lpVtbl -> GetInfo(This,out_pbstrIDBFile) ) 

#endif /* COBJMACROS */


#endif 	/* C style interface */




#endif 	/* __IDebugIDBInfo2_INTERFACE_DEFINED__ */


#ifndef __IEnumDebugIDBInfo2_INTERFACE_DEFINED__
#define __IEnumDebugIDBInfo2_INTERFACE_DEFINED__

/* interface IEnumDebugIDBInfo2 */
/* [unique][uuid][object] */ 


EXTERN_C const IID IID_IEnumDebugIDBInfo2;

#if defined(__cplusplus) && !defined(CINTERFACE)
    
    MIDL_INTERFACE("B34E469B-BD59-11d2-9238-00A02448799A")
    IEnumDebugIDBInfo2 : public IUnknown
    {
    public:
        virtual HRESULT STDMETHODCALLTYPE Next( 
            /* [in] */ ULONG in_NoOfElementsRequested,
            /* [length_is][size_is][out] */ __RPC__out_ecount_part(in_NoOfElementsRequested, *out_pNoOfElementsFetched) IDebugIDBInfo2 **out_ArrayOfpIDBInfo2,
            /* [out] */ __RPC__out ULONG *out_pNoOfElementsFetched) = 0;
        
        virtual HRESULT STDMETHODCALLTYPE Skip( 
            /* [in] */ ULONG in_NoOfElements) = 0;
        
        virtual HRESULT STDMETHODCALLTYPE Reset( void) = 0;
        
        virtual HRESULT STDMETHODCALLTYPE Clone( 
            /* [out] */ __RPC__deref_out_opt IEnumDebugIDBInfo2 **out_ppEnumIDBInfo2) = 0;
        
        virtual HRESULT STDMETHODCALLTYPE GetCount( 
            /* [out] */ __RPC__out ULONG *out_pCount) = 0;
        
    };
    
#else 	/* C style interface */

    typedef struct IEnumDebugIDBInfo2Vtbl
    {
        BEGIN_INTERFACE
        
        HRESULT ( STDMETHODCALLTYPE *QueryInterface )( 
            IEnumDebugIDBInfo2 * This,
            /* [in] */ __RPC__in REFIID riid,
            /* [iid_is][out] */ 
            __RPC__deref_out  void **ppvObject);
        
        ULONG ( STDMETHODCALLTYPE *AddRef )( 
            IEnumDebugIDBInfo2 * This);
        
        ULONG ( STDMETHODCALLTYPE *Release )( 
            IEnumDebugIDBInfo2 * This);
        
        HRESULT ( STDMETHODCALLTYPE *Next )( 
            IEnumDebugIDBInfo2 * This,
            /* [in] */ ULONG in_NoOfElementsRequested,
            /* [length_is][size_is][out] */ __RPC__out_ecount_part(in_NoOfElementsRequested, *out_pNoOfElementsFetched) IDebugIDBInfo2 **out_ArrayOfpIDBInfo2,
            /* [out] */ __RPC__out ULONG *out_pNoOfElementsFetched);
        
        HRESULT ( STDMETHODCALLTYPE *Skip )( 
            IEnumDebugIDBInfo2 * This,
            /* [in] */ ULONG in_NoOfElements);
        
        HRESULT ( STDMETHODCALLTYPE *Reset )( 
            IEnumDebugIDBInfo2 * This);
        
        HRESULT ( STDMETHODCALLTYPE *Clone )( 
            IEnumDebugIDBInfo2 * This,
            /* [out] */ __RPC__deref_out_opt IEnumDebugIDBInfo2 **out_ppEnumIDBInfo2);
        
        HRESULT ( STDMETHODCALLTYPE *GetCount )( 
            IEnumDebugIDBInfo2 * This,
            /* [out] */ __RPC__out ULONG *out_pCount);
        
        END_INTERFACE
    } IEnumDebugIDBInfo2Vtbl;

    interface IEnumDebugIDBInfo2
    {
        CONST_VTBL struct IEnumDebugIDBInfo2Vtbl *lpVtbl;
    };

    

#ifdef COBJMACROS


#define IEnumDebugIDBInfo2_QueryInterface(This,riid,ppvObject)	\
    ( (This)->lpVtbl -> QueryInterface(This,riid,ppvObject) ) 

#define IEnumDebugIDBInfo2_AddRef(This)	\
    ( (This)->lpVtbl -> AddRef(This) ) 

#define IEnumDebugIDBInfo2_Release(This)	\
    ( (This)->lpVtbl -> Release(This) ) 


#define IEnumDebugIDBInfo2_Next(This,in_NoOfElementsRequested,out_ArrayOfpIDBInfo2,out_pNoOfElementsFetched)	\
    ( (This)->lpVtbl -> Next(This,in_NoOfElementsRequested,out_ArrayOfpIDBInfo2,out_pNoOfElementsFetched) ) 

#define IEnumDebugIDBInfo2_Skip(This,in_NoOfElements)	\
    ( (This)->lpVtbl -> Skip(This,in_NoOfElements) ) 

#define IEnumDebugIDBInfo2_Reset(This)	\
    ( (This)->lpVtbl -> Reset(This) ) 

#define IEnumDebugIDBInfo2_Clone(This,out_ppEnumIDBInfo2)	\
    ( (This)->lpVtbl -> Clone(This,out_ppEnumIDBInfo2) ) 

#define IEnumDebugIDBInfo2_GetCount(This,out_pCount)	\
    ( (This)->lpVtbl -> GetCount(This,out_pCount) ) 

#endif /* COBJMACROS */


#endif 	/* C style interface */




#endif 	/* __IEnumDebugIDBInfo2_INTERFACE_DEFINED__ */


#ifndef __IDebugENCBuildInfo2_INTERFACE_DEFINED__
#define __IDebugENCBuildInfo2_INTERFACE_DEFINED__

/* interface IDebugENCBuildInfo2 */
/* [unique][uuid][object] */ 


EXTERN_C const IID IID_IDebugENCBuildInfo2;

#if defined(__cplusplus) && !defined(CINTERFACE)
    
    MIDL_INTERFACE("EA70281B-BD58-11d2-9238-00A02448799A")
    IDebugENCBuildInfo2 : public IUnknown
    {
    public:
        virtual HRESULT STDMETHODCALLTYPE GetTargetBuildInfo( 
            /* [in] */ __RPC__in LPCOLESTR in_szTargetPath,
            /* [out] */ __RPC__deref_out_opt BSTR *out_pbstrSourcePath,
            /* [out] */ __RPC__deref_out_opt BSTR *out_pbstrCommand,
            /* [out] */ __RPC__deref_out_opt BSTR *out_pbstrCurrentdir) = 0;
        
        virtual HRESULT STDMETHODCALLTYPE IsTargetEligible( 
            /* [in] */ __RPC__in LPCOLESTR in_szTargetPath) = 0;
        
        virtual HRESULT STDMETHODCALLTYPE EnumDebugIDBInfo( 
            /* [out] */ __RPC__deref_out_opt IEnumDebugIDBInfo2 **out_ppEnumIDBInfo) = 0;
        
    };
    
#else 	/* C style interface */

    typedef struct IDebugENCBuildInfo2Vtbl
    {
        BEGIN_INTERFACE
        
        HRESULT ( STDMETHODCALLTYPE *QueryInterface )( 
            IDebugENCBuildInfo2 * This,
            /* [in] */ __RPC__in REFIID riid,
            /* [iid_is][out] */ 
            __RPC__deref_out  void **ppvObject);
        
        ULONG ( STDMETHODCALLTYPE *AddRef )( 
            IDebugENCBuildInfo2 * This);
        
        ULONG ( STDMETHODCALLTYPE *Release )( 
            IDebugENCBuildInfo2 * This);
        
        HRESULT ( STDMETHODCALLTYPE *GetTargetBuildInfo )( 
            IDebugENCBuildInfo2 * This,
            /* [in] */ __RPC__in LPCOLESTR in_szTargetPath,
            /* [out] */ __RPC__deref_out_opt BSTR *out_pbstrSourcePath,
            /* [out] */ __RPC__deref_out_opt BSTR *out_pbstrCommand,
            /* [out] */ __RPC__deref_out_opt BSTR *out_pbstrCurrentdir);
        
        HRESULT ( STDMETHODCALLTYPE *IsTargetEligible )( 
            IDebugENCBuildInfo2 * This,
            /* [in] */ __RPC__in LPCOLESTR in_szTargetPath);
        
        HRESULT ( STDMETHODCALLTYPE *EnumDebugIDBInfo )( 
            IDebugENCBuildInfo2 * This,
            /* [out] */ __RPC__deref_out_opt IEnumDebugIDBInfo2 **out_ppEnumIDBInfo);
        
        END_INTERFACE
    } IDebugENCBuildInfo2Vtbl;

    interface IDebugENCBuildInfo2
    {
        CONST_VTBL struct IDebugENCBuildInfo2Vtbl *lpVtbl;
    };

    

#ifdef COBJMACROS


#define IDebugENCBuildInfo2_QueryInterface(This,riid,ppvObject)	\
    ( (This)->lpVtbl -> QueryInterface(This,riid,ppvObject) ) 

#define IDebugENCBuildInfo2_AddRef(This)	\
    ( (This)->lpVtbl -> AddRef(This) ) 

#define IDebugENCBuildInfo2_Release(This)	\
    ( (This)->lpVtbl -> Release(This) ) 


#define IDebugENCBuildInfo2_GetTargetBuildInfo(This,in_szTargetPath,out_pbstrSourcePath,out_pbstrCommand,out_pbstrCurrentdir)	\
    ( (This)->lpVtbl -> GetTargetBuildInfo(This,in_szTargetPath,out_pbstrSourcePath,out_pbstrCommand,out_pbstrCurrentdir) ) 

#define IDebugENCBuildInfo2_IsTargetEligible(This,in_szTargetPath)	\
    ( (This)->lpVtbl -> IsTargetEligible(This,in_szTargetPath) ) 

#define IDebugENCBuildInfo2_EnumDebugIDBInfo(This,out_ppEnumIDBInfo)	\
    ( (This)->lpVtbl -> EnumDebugIDBInfo(This,out_ppEnumIDBInfo) ) 

#endif /* COBJMACROS */


#endif 	/* C style interface */




#endif 	/* __IDebugENCBuildInfo2_INTERFACE_DEFINED__ */


#ifndef __IDebugENCUpdateOnRelinkEvent2_INTERFACE_DEFINED__
#define __IDebugENCUpdateOnRelinkEvent2_INTERFACE_DEFINED__

/* interface IDebugENCUpdateOnRelinkEvent2 */
/* [unique][uuid][object] */ 


EXTERN_C const IID IID_IDebugENCUpdateOnRelinkEvent2;

#if defined(__cplusplus) && !defined(CINTERFACE)
    
    MIDL_INTERFACE("0EBF1959-BD57-11d2-9238-00A02448799A")
    IDebugENCUpdateOnRelinkEvent2 : public IUnknown
    {
    public:
        virtual HRESULT STDMETHODCALLTYPE GetInfo( 
            /* [out] */ __RPC__deref_out_opt IEnumDebugENCRelinkInfo2 **out_ppEnumENCRelinkInfo) = 0;
        
    };
    
#else 	/* C style interface */

    typedef struct IDebugENCUpdateOnRelinkEvent2Vtbl
    {
        BEGIN_INTERFACE
        
        HRESULT ( STDMETHODCALLTYPE *QueryInterface )( 
            IDebugENCUpdateOnRelinkEvent2 * This,
            /* [in] */ __RPC__in REFIID riid,
            /* [iid_is][out] */ 
            __RPC__deref_out  void **ppvObject);
        
        ULONG ( STDMETHODCALLTYPE *AddRef )( 
            IDebugENCUpdateOnRelinkEvent2 * This);
        
        ULONG ( STDMETHODCALLTYPE *Release )( 
            IDebugENCUpdateOnRelinkEvent2 * This);
        
        HRESULT ( STDMETHODCALLTYPE *GetInfo )( 
            IDebugENCUpdateOnRelinkEvent2 * This,
            /* [out] */ __RPC__deref_out_opt IEnumDebugENCRelinkInfo2 **out_ppEnumENCRelinkInfo);
        
        END_INTERFACE
    } IDebugENCUpdateOnRelinkEvent2Vtbl;

    interface IDebugENCUpdateOnRelinkEvent2
    {
        CONST_VTBL struct IDebugENCUpdateOnRelinkEvent2Vtbl *lpVtbl;
    };

    

#ifdef COBJMACROS


#define IDebugENCUpdateOnRelinkEvent2_QueryInterface(This,riid,ppvObject)	\
    ( (This)->lpVtbl -> QueryInterface(This,riid,ppvObject) ) 

#define IDebugENCUpdateOnRelinkEvent2_AddRef(This)	\
    ( (This)->lpVtbl -> AddRef(This) ) 

#define IDebugENCUpdateOnRelinkEvent2_Release(This)	\
    ( (This)->lpVtbl -> Release(This) ) 


#define IDebugENCUpdateOnRelinkEvent2_GetInfo(This,out_ppEnumENCRelinkInfo)	\
    ( (This)->lpVtbl -> GetInfo(This,out_ppEnumENCRelinkInfo) ) 

#endif /* COBJMACROS */


#endif 	/* C style interface */




#endif 	/* __IDebugENCUpdateOnRelinkEvent2_INTERFACE_DEFINED__ */


#ifndef __IDebugENCUpdateOnStaleCodeEvent2_INTERFACE_DEFINED__
#define __IDebugENCUpdateOnStaleCodeEvent2_INTERFACE_DEFINED__

/* interface IDebugENCUpdateOnStaleCodeEvent2 */
/* [unique][uuid][object] */ 


EXTERN_C const IID IID_IDebugENCUpdateOnStaleCodeEvent2;

#if defined(__cplusplus) && !defined(CINTERFACE)
    
    MIDL_INTERFACE("2F01EB29-BD57-11d2-9238-00A02448799A")
    IDebugENCUpdateOnStaleCodeEvent2 : public IUnknown
    {
    public:
        virtual HRESULT STDMETHODCALLTYPE GetInfo( 
            /* [out] */ __RPC__deref_out_opt IEnumDebugENCInfo2 **out_ppEnumENCInfo) = 0;
        
        virtual HRESULT STDMETHODCALLTYPE GetHResult( 
            /* [out] */ __RPC__out HRESULT *out_pHResult) = 0;
        
    };
    
#else 	/* C style interface */

    typedef struct IDebugENCUpdateOnStaleCodeEvent2Vtbl
    {
        BEGIN_INTERFACE
        
        HRESULT ( STDMETHODCALLTYPE *QueryInterface )( 
            IDebugENCUpdateOnStaleCodeEvent2 * This,
            /* [in] */ __RPC__in REFIID riid,
            /* [iid_is][out] */ 
            __RPC__deref_out  void **ppvObject);
        
        ULONG ( STDMETHODCALLTYPE *AddRef )( 
            IDebugENCUpdateOnStaleCodeEvent2 * This);
        
        ULONG ( STDMETHODCALLTYPE *Release )( 
            IDebugENCUpdateOnStaleCodeEvent2 * This);
        
        HRESULT ( STDMETHODCALLTYPE *GetInfo )( 
            IDebugENCUpdateOnStaleCodeEvent2 * This,
            /* [out] */ __RPC__deref_out_opt IEnumDebugENCInfo2 **out_ppEnumENCInfo);
        
        HRESULT ( STDMETHODCALLTYPE *GetHResult )( 
            IDebugENCUpdateOnStaleCodeEvent2 * This,
            /* [out] */ __RPC__out HRESULT *out_pHResult);
        
        END_INTERFACE
    } IDebugENCUpdateOnStaleCodeEvent2Vtbl;

    interface IDebugENCUpdateOnStaleCodeEvent2
    {
        CONST_VTBL struct IDebugENCUpdateOnStaleCodeEvent2Vtbl *lpVtbl;
    };

    

#ifdef COBJMACROS


#define IDebugENCUpdateOnStaleCodeEvent2_QueryInterface(This,riid,ppvObject)	\
    ( (This)->lpVtbl -> QueryInterface(This,riid,ppvObject) ) 

#define IDebugENCUpdateOnStaleCodeEvent2_AddRef(This)	\
    ( (This)->lpVtbl -> AddRef(This) ) 

#define IDebugENCUpdateOnStaleCodeEvent2_Release(This)	\
    ( (This)->lpVtbl -> Release(This) ) 


#define IDebugENCUpdateOnStaleCodeEvent2_GetInfo(This,out_ppEnumENCInfo)	\
    ( (This)->lpVtbl -> GetInfo(This,out_ppEnumENCInfo) ) 

#define IDebugENCUpdateOnStaleCodeEvent2_GetHResult(This,out_pHResult)	\
    ( (This)->lpVtbl -> GetHResult(This,out_pHResult) ) 

#endif /* COBJMACROS */


#endif 	/* C style interface */




#endif 	/* __IDebugENCUpdateOnStaleCodeEvent2_INTERFACE_DEFINED__ */


#ifndef __IDebugENCUpdate_INTERFACE_DEFINED__
#define __IDebugENCUpdate_INTERFACE_DEFINED__

/* interface IDebugENCUpdate */
/* [unique][uuid][object] */ 


EXTERN_C const IID IID_IDebugENCUpdate;

#if defined(__cplusplus) && !defined(CINTERFACE)
    
    MIDL_INTERFACE("978BAEE7-BD4C-11d2-9238-00A02448799A")
    IDebugENCUpdate : public IUnknown
    {
    public:
        virtual HRESULT STDMETHODCALLTYPE EnumENCSnapshots( 
            /* [in] */ __RPC__in LPCOLESTR pszModule,
            /* [out] */ __RPC__deref_out_opt IEnumDebugENCSnapshots2 **ppEnum) = 0;
        
        virtual HRESULT STDMETHODCALLTYPE EnumENCSnapshotsByGuid( 
            /* [in] */ __RPC__in REFGUID guidModule,
            /* [out] */ __RPC__deref_out_opt IEnumDebugENCSnapshots2 **ppEnum) = 0;
        
        virtual HRESULT STDMETHODCALLTYPE CancelENC( void) = 0;
        
        virtual HRESULT STDMETHODCALLTYPE OnENCAttemptComplete( void) = 0;
        
    };
    
#else 	/* C style interface */

    typedef struct IDebugENCUpdateVtbl
    {
        BEGIN_INTERFACE
        
        HRESULT ( STDMETHODCALLTYPE *QueryInterface )( 
            IDebugENCUpdate * This,
            /* [in] */ __RPC__in REFIID riid,
            /* [iid_is][out] */ 
            __RPC__deref_out  void **ppvObject);
        
        ULONG ( STDMETHODCALLTYPE *AddRef )( 
            IDebugENCUpdate * This);
        
        ULONG ( STDMETHODCALLTYPE *Release )( 
            IDebugENCUpdate * This);
        
        HRESULT ( STDMETHODCALLTYPE *EnumENCSnapshots )( 
            IDebugENCUpdate * This,
            /* [in] */ __RPC__in LPCOLESTR pszModule,
            /* [out] */ __RPC__deref_out_opt IEnumDebugENCSnapshots2 **ppEnum);
        
        HRESULT ( STDMETHODCALLTYPE *EnumENCSnapshotsByGuid )( 
            IDebugENCUpdate * This,
            /* [in] */ __RPC__in REFGUID guidModule,
            /* [out] */ __RPC__deref_out_opt IEnumDebugENCSnapshots2 **ppEnum);
        
        HRESULT ( STDMETHODCALLTYPE *CancelENC )( 
            IDebugENCUpdate * This);
        
        HRESULT ( STDMETHODCALLTYPE *OnENCAttemptComplete )( 
            IDebugENCUpdate * This);
        
        END_INTERFACE
    } IDebugENCUpdateVtbl;

    interface IDebugENCUpdate
    {
        CONST_VTBL struct IDebugENCUpdateVtbl *lpVtbl;
    };

    

#ifdef COBJMACROS


#define IDebugENCUpdate_QueryInterface(This,riid,ppvObject)	\
    ( (This)->lpVtbl -> QueryInterface(This,riid,ppvObject) ) 

#define IDebugENCUpdate_AddRef(This)	\
    ( (This)->lpVtbl -> AddRef(This) ) 

#define IDebugENCUpdate_Release(This)	\
    ( (This)->lpVtbl -> Release(This) ) 


#define IDebugENCUpdate_EnumENCSnapshots(This,pszModule,ppEnum)	\
    ( (This)->lpVtbl -> EnumENCSnapshots(This,pszModule,ppEnum) ) 

#define IDebugENCUpdate_EnumENCSnapshotsByGuid(This,guidModule,ppEnum)	\
    ( (This)->lpVtbl -> EnumENCSnapshotsByGuid(This,guidModule,ppEnum) ) 

#define IDebugENCUpdate_CancelENC(This)	\
    ( (This)->lpVtbl -> CancelENC(This) ) 

#define IDebugENCUpdate_OnENCAttemptComplete(This)	\
    ( (This)->lpVtbl -> OnENCAttemptComplete(This) ) 

#endif /* COBJMACROS */


#endif 	/* C style interface */




#endif 	/* __IDebugENCUpdate_INTERFACE_DEFINED__ */


#ifndef __IDebugENCSnapshot2_INTERFACE_DEFINED__
#define __IDebugENCSnapshot2_INTERFACE_DEFINED__

/* interface IDebugENCSnapshot2 */
/* [unique][uuid][object] */ 


enum __MIDL_IDebugENCSnapshot2_0001
    {	ENC_SNAPSHOT_TYPE_COMPLUS	= 0x1,
	ENC_SNAPSHOT_TYPE_CPP	= 0x2
    } ;
typedef DWORD ENC_SNAPSHOT_TYPE;

typedef struct _ENC_SNAPSHOT_COMPLUS
    {
    IDebugComPlusSnapshot2 *pcpSnapshot;
    } 	ENC_SNAPSHOT_COMPLUS;

typedef struct _ENC_SNAPSHOT_CPP
    {
    DWORD dwNYI;
    } 	ENC_SNAPSHOT_CPP;

typedef struct _ENC_SNAPSHOT
    {
    ENC_SNAPSHOT_TYPE ssType;
    /* [switch_type] */ union __MIDL_IDebugENCSnapshot2_0002
        {
        ENC_SNAPSHOT_COMPLUS encComPlus;
        ENC_SNAPSHOT_CPP encCpp;
        DWORD unused;
        } 	encSnapshot;
    } 	ENC_SNAPSHOT;

typedef struct _ENC_SNAPSHOT_INFO
    {
    IDebugProgram2 *pProgram;
    ENC_SNAPSHOT encSnapshot;
    } 	ENC_SNAPSHOT_INFO;


EXTERN_C const IID IID_IDebugENCSnapshot2;

#if defined(__cplusplus) && !defined(CINTERFACE)
    
    MIDL_INTERFACE("f6f94d0e-78c2-11d2-8ffe-00c04fa38314")
    IDebugENCSnapshot2 : public IUnknown
    {
    public:
        virtual HRESULT STDMETHODCALLTYPE GetENCSnapshotInfo( 
            /* [out] */ __RPC__out ENC_SNAPSHOT_INFO *pSnapshotInfo) = 0;
        
        virtual HRESULT STDMETHODCALLTYPE ApplyCodeChange( 
            /* [in] */ ULONG in_NoOfLineMaps,
            /* [size_is][in] */ __RPC__in_ecount_full(in_NoOfLineMaps) IDebugENCLineMap **in_ArrayOfLineMaps,
            /* [in] */ __RPC__in_opt IServiceProvider *in_pServiceProvider,
            /* [in] */ BOOL in_fOnContinue,
            /* [out] */ __RPC__deref_out_opt IEnumDebugENCInfo2 **out_ppEnumENCInfo) = 0;
        
        virtual HRESULT STDMETHODCALLTYPE CommitChange( 
            /* [out] */ __RPC__deref_out_opt IEnumDebugENCInfo2 **out_ppEnumENCInfo) = 0;
        
    };
    
#else 	/* C style interface */

    typedef struct IDebugENCSnapshot2Vtbl
    {
        BEGIN_INTERFACE
        
        HRESULT ( STDMETHODCALLTYPE *QueryInterface )( 
            IDebugENCSnapshot2 * This,
            /* [in] */ __RPC__in REFIID riid,
            /* [iid_is][out] */ 
            __RPC__deref_out  void **ppvObject);
        
        ULONG ( STDMETHODCALLTYPE *AddRef )( 
            IDebugENCSnapshot2 * This);
        
        ULONG ( STDMETHODCALLTYPE *Release )( 
            IDebugENCSnapshot2 * This);
        
        HRESULT ( STDMETHODCALLTYPE *GetENCSnapshotInfo )( 
            IDebugENCSnapshot2 * This,
            /* [out] */ __RPC__out ENC_SNAPSHOT_INFO *pSnapshotInfo);
        
        HRESULT ( STDMETHODCALLTYPE *ApplyCodeChange )( 
            IDebugENCSnapshot2 * This,
            /* [in] */ ULONG in_NoOfLineMaps,
            /* [size_is][in] */ __RPC__in_ecount_full(in_NoOfLineMaps) IDebugENCLineMap **in_ArrayOfLineMaps,
            /* [in] */ __RPC__in_opt IServiceProvider *in_pServiceProvider,
            /* [in] */ BOOL in_fOnContinue,
            /* [out] */ __RPC__deref_out_opt IEnumDebugENCInfo2 **out_ppEnumENCInfo);
        
        HRESULT ( STDMETHODCALLTYPE *CommitChange )( 
            IDebugENCSnapshot2 * This,
            /* [out] */ __RPC__deref_out_opt IEnumDebugENCInfo2 **out_ppEnumENCInfo);
        
        END_INTERFACE
    } IDebugENCSnapshot2Vtbl;

    interface IDebugENCSnapshot2
    {
        CONST_VTBL struct IDebugENCSnapshot2Vtbl *lpVtbl;
    };

    

#ifdef COBJMACROS


#define IDebugENCSnapshot2_QueryInterface(This,riid,ppvObject)	\
    ( (This)->lpVtbl -> QueryInterface(This,riid,ppvObject) ) 

#define IDebugENCSnapshot2_AddRef(This)	\
    ( (This)->lpVtbl -> AddRef(This) ) 

#define IDebugENCSnapshot2_Release(This)	\
    ( (This)->lpVtbl -> Release(This) ) 


#define IDebugENCSnapshot2_GetENCSnapshotInfo(This,pSnapshotInfo)	\
    ( (This)->lpVtbl -> GetENCSnapshotInfo(This,pSnapshotInfo) ) 

#define IDebugENCSnapshot2_ApplyCodeChange(This,in_NoOfLineMaps,in_ArrayOfLineMaps,in_pServiceProvider,in_fOnContinue,out_ppEnumENCInfo)	\
    ( (This)->lpVtbl -> ApplyCodeChange(This,in_NoOfLineMaps,in_ArrayOfLineMaps,in_pServiceProvider,in_fOnContinue,out_ppEnumENCInfo) ) 

#define IDebugENCSnapshot2_CommitChange(This,out_ppEnumENCInfo)	\
    ( (This)->lpVtbl -> CommitChange(This,out_ppEnumENCInfo) ) 

#endif /* COBJMACROS */


#endif 	/* C style interface */




#endif 	/* __IDebugENCSnapshot2_INTERFACE_DEFINED__ */


#ifndef __IDebugENCSnapshot3_INTERFACE_DEFINED__
#define __IDebugENCSnapshot3_INTERFACE_DEFINED__

/* interface IDebugENCSnapshot3 */
/* [unique][uuid][object] */ 


EXTERN_C const IID IID_IDebugENCSnapshot3;

#if defined(__cplusplus) && !defined(CINTERFACE)
    
    MIDL_INTERFACE("3F50C7D0-D1AF-4a97-AD81-7FDD5934AD32")
    IDebugENCSnapshot3 : public IDebugENCSnapshot2
    {
    public:
        virtual HRESULT STDMETHODCALLTYPE ApplyCodeChange3( 
            /* [in] */ ULONG ulLineHint,
            /* [in] */ ULONG in_NoOfLineMaps,
            /* [size_is][in] */ __RPC__in_ecount_full(in_NoOfLineMaps) IDebugENCLineMap **in_ArrayOfLineMaps,
            /* [in] */ __RPC__in_opt IServiceProvider *in_pServiceProvider,
            /* [in] */ BOOL in_fOnContinue,
            /* [out] */ __RPC__deref_out_opt IEnumDebugENCInfo2 **out_ppEnumENCInfo) = 0;
        
    };
    
#else 	/* C style interface */

    typedef struct IDebugENCSnapshot3Vtbl
    {
        BEGIN_INTERFACE
        
        HRESULT ( STDMETHODCALLTYPE *QueryInterface )( 
            IDebugENCSnapshot3 * This,
            /* [in] */ __RPC__in REFIID riid,
            /* [iid_is][out] */ 
            __RPC__deref_out  void **ppvObject);
        
        ULONG ( STDMETHODCALLTYPE *AddRef )( 
            IDebugENCSnapshot3 * This);
        
        ULONG ( STDMETHODCALLTYPE *Release )( 
            IDebugENCSnapshot3 * This);
        
        HRESULT ( STDMETHODCALLTYPE *GetENCSnapshotInfo )( 
            IDebugENCSnapshot3 * This,
            /* [out] */ __RPC__out ENC_SNAPSHOT_INFO *pSnapshotInfo);
        
        HRESULT ( STDMETHODCALLTYPE *ApplyCodeChange )( 
            IDebugENCSnapshot3 * This,
            /* [in] */ ULONG in_NoOfLineMaps,
            /* [size_is][in] */ __RPC__in_ecount_full(in_NoOfLineMaps) IDebugENCLineMap **in_ArrayOfLineMaps,
            /* [in] */ __RPC__in_opt IServiceProvider *in_pServiceProvider,
            /* [in] */ BOOL in_fOnContinue,
            /* [out] */ __RPC__deref_out_opt IEnumDebugENCInfo2 **out_ppEnumENCInfo);
        
        HRESULT ( STDMETHODCALLTYPE *CommitChange )( 
            IDebugENCSnapshot3 * This,
            /* [out] */ __RPC__deref_out_opt IEnumDebugENCInfo2 **out_ppEnumENCInfo);
        
        HRESULT ( STDMETHODCALLTYPE *ApplyCodeChange3 )( 
            IDebugENCSnapshot3 * This,
            /* [in] */ ULONG ulLineHint,
            /* [in] */ ULONG in_NoOfLineMaps,
            /* [size_is][in] */ __RPC__in_ecount_full(in_NoOfLineMaps) IDebugENCLineMap **in_ArrayOfLineMaps,
            /* [in] */ __RPC__in_opt IServiceProvider *in_pServiceProvider,
            /* [in] */ BOOL in_fOnContinue,
            /* [out] */ __RPC__deref_out_opt IEnumDebugENCInfo2 **out_ppEnumENCInfo);
        
        END_INTERFACE
    } IDebugENCSnapshot3Vtbl;

    interface IDebugENCSnapshot3
    {
        CONST_VTBL struct IDebugENCSnapshot3Vtbl *lpVtbl;
    };

    

#ifdef COBJMACROS


#define IDebugENCSnapshot3_QueryInterface(This,riid,ppvObject)	\
    ( (This)->lpVtbl -> QueryInterface(This,riid,ppvObject) ) 

#define IDebugENCSnapshot3_AddRef(This)	\
    ( (This)->lpVtbl -> AddRef(This) ) 

#define IDebugENCSnapshot3_Release(This)	\
    ( (This)->lpVtbl -> Release(This) ) 


#define IDebugENCSnapshot3_GetENCSnapshotInfo(This,pSnapshotInfo)	\
    ( (This)->lpVtbl -> GetENCSnapshotInfo(This,pSnapshotInfo) ) 

#define IDebugENCSnapshot3_ApplyCodeChange(This,in_NoOfLineMaps,in_ArrayOfLineMaps,in_pServiceProvider,in_fOnContinue,out_ppEnumENCInfo)	\
    ( (This)->lpVtbl -> ApplyCodeChange(This,in_NoOfLineMaps,in_ArrayOfLineMaps,in_pServiceProvider,in_fOnContinue,out_ppEnumENCInfo) ) 

#define IDebugENCSnapshot3_CommitChange(This,out_ppEnumENCInfo)	\
    ( (This)->lpVtbl -> CommitChange(This,out_ppEnumENCInfo) ) 


#define IDebugENCSnapshot3_ApplyCodeChange3(This,ulLineHint,in_NoOfLineMaps,in_ArrayOfLineMaps,in_pServiceProvider,in_fOnContinue,out_ppEnumENCInfo)	\
    ( (This)->lpVtbl -> ApplyCodeChange3(This,ulLineHint,in_NoOfLineMaps,in_ArrayOfLineMaps,in_pServiceProvider,in_fOnContinue,out_ppEnumENCInfo) ) 

#endif /* COBJMACROS */


#endif 	/* C style interface */




#endif 	/* __IDebugENCSnapshot3_INTERFACE_DEFINED__ */


#ifndef __IEnumDebugENCSnapshots2_INTERFACE_DEFINED__
#define __IEnumDebugENCSnapshots2_INTERFACE_DEFINED__

/* interface IEnumDebugENCSnapshots2 */
/* [unique][uuid][object] */ 


EXTERN_C const IID IID_IEnumDebugENCSnapshots2;

#if defined(__cplusplus) && !defined(CINTERFACE)
    
    MIDL_INTERFACE("f6f94d1a-78c2-11d2-8ffe-00c04fa38314")
    IEnumDebugENCSnapshots2 : public IUnknown
    {
    public:
        virtual HRESULT STDMETHODCALLTYPE Next( 
            /* [in] */ ULONG celt,
            /* [length_is][size_is][out] */ __RPC__out_ecount_part(celt, *pceltFetched) IDebugENCSnapshot2 **rgelt,
            /* [out][in] */ __RPC__inout ULONG *pceltFetched) = 0;
        
        virtual HRESULT STDMETHODCALLTYPE Skip( 
            /* [in] */ ULONG celt) = 0;
        
        virtual HRESULT STDMETHODCALLTYPE Reset( void) = 0;
        
        virtual HRESULT STDMETHODCALLTYPE Clone( 
            /* [out] */ __RPC__deref_out_opt IEnumDebugENCSnapshots2 **ppEnum) = 0;
        
        virtual HRESULT STDMETHODCALLTYPE GetCount( 
            /* [out] */ __RPC__out ULONG *pcelt) = 0;
        
    };
    
#else 	/* C style interface */

    typedef struct IEnumDebugENCSnapshots2Vtbl
    {
        BEGIN_INTERFACE
        
        HRESULT ( STDMETHODCALLTYPE *QueryInterface )( 
            IEnumDebugENCSnapshots2 * This,
            /* [in] */ __RPC__in REFIID riid,
            /* [iid_is][out] */ 
            __RPC__deref_out  void **ppvObject);
        
        ULONG ( STDMETHODCALLTYPE *AddRef )( 
            IEnumDebugENCSnapshots2 * This);
        
        ULONG ( STDMETHODCALLTYPE *Release )( 
            IEnumDebugENCSnapshots2 * This);
        
        HRESULT ( STDMETHODCALLTYPE *Next )( 
            IEnumDebugENCSnapshots2 * This,
            /* [in] */ ULONG celt,
            /* [length_is][size_is][out] */ __RPC__out_ecount_part(celt, *pceltFetched) IDebugENCSnapshot2 **rgelt,
            /* [out][in] */ __RPC__inout ULONG *pceltFetched);
        
        HRESULT ( STDMETHODCALLTYPE *Skip )( 
            IEnumDebugENCSnapshots2 * This,
            /* [in] */ ULONG celt);
        
        HRESULT ( STDMETHODCALLTYPE *Reset )( 
            IEnumDebugENCSnapshots2 * This);
        
        HRESULT ( STDMETHODCALLTYPE *Clone )( 
            IEnumDebugENCSnapshots2 * This,
            /* [out] */ __RPC__deref_out_opt IEnumDebugENCSnapshots2 **ppEnum);
        
        HRESULT ( STDMETHODCALLTYPE *GetCount )( 
            IEnumDebugENCSnapshots2 * This,
            /* [out] */ __RPC__out ULONG *pcelt);
        
        END_INTERFACE
    } IEnumDebugENCSnapshots2Vtbl;

    interface IEnumDebugENCSnapshots2
    {
        CONST_VTBL struct IEnumDebugENCSnapshots2Vtbl *lpVtbl;
    };

    

#ifdef COBJMACROS


#define IEnumDebugENCSnapshots2_QueryInterface(This,riid,ppvObject)	\
    ( (This)->lpVtbl -> QueryInterface(This,riid,ppvObject) ) 

#define IEnumDebugENCSnapshots2_AddRef(This)	\
    ( (This)->lpVtbl -> AddRef(This) ) 

#define IEnumDebugENCSnapshots2_Release(This)	\
    ( (This)->lpVtbl -> Release(This) ) 


#define IEnumDebugENCSnapshots2_Next(This,celt,rgelt,pceltFetched)	\
    ( (This)->lpVtbl -> Next(This,celt,rgelt,pceltFetched) ) 

#define IEnumDebugENCSnapshots2_Skip(This,celt)	\
    ( (This)->lpVtbl -> Skip(This,celt) ) 

#define IEnumDebugENCSnapshots2_Reset(This)	\
    ( (This)->lpVtbl -> Reset(This) ) 

#define IEnumDebugENCSnapshots2_Clone(This,ppEnum)	\
    ( (This)->lpVtbl -> Clone(This,ppEnum) ) 

#define IEnumDebugENCSnapshots2_GetCount(This,pcelt)	\
    ( (This)->lpVtbl -> GetCount(This,pcelt) ) 

#endif /* COBJMACROS */


#endif 	/* C style interface */




#endif 	/* __IEnumDebugENCSnapshots2_INTERFACE_DEFINED__ */


#ifndef __IEnumDebugErrorInfos2_INTERFACE_DEFINED__
#define __IEnumDebugErrorInfos2_INTERFACE_DEFINED__

/* interface IEnumDebugErrorInfos2 */
/* [unique][uuid][object] */ 


EXTERN_C const IID IID_IEnumDebugErrorInfos2;

#if defined(__cplusplus) && !defined(CINTERFACE)
    
    MIDL_INTERFACE("f6f94d23-78c2-11d2-8ffe-00c04fa38314")
    IEnumDebugErrorInfos2 : public IUnknown
    {
    public:
        virtual HRESULT STDMETHODCALLTYPE Next( 
            /* [in] */ ULONG celt,
            /* [length_is][size_is][out] */ __RPC__out_ecount_part(celt, *pceltFetched) IErrorInfo **rgelt,
            /* [out][in] */ __RPC__inout ULONG *pceltFetched) = 0;
        
        virtual HRESULT STDMETHODCALLTYPE Skip( 
            /* [in] */ ULONG celt) = 0;
        
        virtual HRESULT STDMETHODCALLTYPE Reset( void) = 0;
        
        virtual HRESULT STDMETHODCALLTYPE Clone( 
            /* [out] */ __RPC__deref_out_opt IEnumDebugErrorInfos2 **ppEnum) = 0;
        
        virtual HRESULT STDMETHODCALLTYPE GetCount( 
            /* [out] */ __RPC__out ULONG *pcelt) = 0;
        
    };
    
#else 	/* C style interface */

    typedef struct IEnumDebugErrorInfos2Vtbl
    {
        BEGIN_INTERFACE
        
        HRESULT ( STDMETHODCALLTYPE *QueryInterface )( 
            IEnumDebugErrorInfos2 * This,
            /* [in] */ __RPC__in REFIID riid,
            /* [iid_is][out] */ 
            __RPC__deref_out  void **ppvObject);
        
        ULONG ( STDMETHODCALLTYPE *AddRef )( 
            IEnumDebugErrorInfos2 * This);
        
        ULONG ( STDMETHODCALLTYPE *Release )( 
            IEnumDebugErrorInfos2 * This);
        
        HRESULT ( STDMETHODCALLTYPE *Next )( 
            IEnumDebugErrorInfos2 * This,
            /* [in] */ ULONG celt,
            /* [length_is][size_is][out] */ __RPC__out_ecount_part(celt, *pceltFetched) IErrorInfo **rgelt,
            /* [out][in] */ __RPC__inout ULONG *pceltFetched);
        
        HRESULT ( STDMETHODCALLTYPE *Skip )( 
            IEnumDebugErrorInfos2 * This,
            /* [in] */ ULONG celt);
        
        HRESULT ( STDMETHODCALLTYPE *Reset )( 
            IEnumDebugErrorInfos2 * This);
        
        HRESULT ( STDMETHODCALLTYPE *Clone )( 
            IEnumDebugErrorInfos2 * This,
            /* [out] */ __RPC__deref_out_opt IEnumDebugErrorInfos2 **ppEnum);
        
        HRESULT ( STDMETHODCALLTYPE *GetCount )( 
            IEnumDebugErrorInfos2 * This,
            /* [out] */ __RPC__out ULONG *pcelt);
        
        END_INTERFACE
    } IEnumDebugErrorInfos2Vtbl;

    interface IEnumDebugErrorInfos2
    {
        CONST_VTBL struct IEnumDebugErrorInfos2Vtbl *lpVtbl;
    };

    

#ifdef COBJMACROS


#define IEnumDebugErrorInfos2_QueryInterface(This,riid,ppvObject)	\
    ( (This)->lpVtbl -> QueryInterface(This,riid,ppvObject) ) 

#define IEnumDebugErrorInfos2_AddRef(This)	\
    ( (This)->lpVtbl -> AddRef(This) ) 

#define IEnumDebugErrorInfos2_Release(This)	\
    ( (This)->lpVtbl -> Release(This) ) 


#define IEnumDebugErrorInfos2_Next(This,celt,rgelt,pceltFetched)	\
    ( (This)->lpVtbl -> Next(This,celt,rgelt,pceltFetched) ) 

#define IEnumDebugErrorInfos2_Skip(This,celt)	\
    ( (This)->lpVtbl -> Skip(This,celt) ) 

#define IEnumDebugErrorInfos2_Reset(This)	\
    ( (This)->lpVtbl -> Reset(This) ) 

#define IEnumDebugErrorInfos2_Clone(This,ppEnum)	\
    ( (This)->lpVtbl -> Clone(This,ppEnum) ) 

#define IEnumDebugErrorInfos2_GetCount(This,pcelt)	\
    ( (This)->lpVtbl -> GetCount(This,pcelt) ) 

#endif /* COBJMACROS */


#endif 	/* C style interface */




#endif 	/* __IEnumDebugErrorInfos2_INTERFACE_DEFINED__ */


#ifndef __IENCDebugInfo_INTERFACE_DEFINED__
#define __IENCDebugInfo_INTERFACE_DEFINED__

/* interface IENCDebugInfo */
/* [unique][uuid][object] */ 

typedef struct _ENC_LOCALINFO
    {
    BSTR bstrLocalName;
    ULONG32 ulAttribute;
    BYTE *pSignature;
    ULONG32 cbSize;
    UINT slot;
    } 	ENC_LOCALINFO;

typedef struct _ENC_LOCAL_EXPR_CONTEXT
    {
    IDebugSymbolProvider *pSymbolProvider;
    IDebugAddress *pAddress;
    IUnknown *pBinder;
    IUnknown *pEE;
    } 	ENC_LOCAL_EXPR_CONTEXT;


EXTERN_C const IID IID_IENCDebugInfo;

#if defined(__cplusplus) && !defined(CINTERFACE)
    
    MIDL_INTERFACE("1DA15C39-7E02-4ee8-8F60-FFF81275EE14")
    IENCDebugInfo : public IUnknown
    {
    public:
        virtual HRESULT STDMETHODCALLTYPE GetLocalVariableCount( 
            /* [in] */ UINT32 MethodToken,
            /* [out] */ __RPC__out ULONG *pcLocals) = 0;
        
        virtual HRESULT STDMETHODCALLTYPE GetLocalVariableLayout( 
            /* [in] */ UINT32 MethodToken,
            /* [in] */ ULONG cLocals,
            /* [length_is][size_is][out] */ __RPC__out_ecount_part(cLocals, *pceltFetched) ENC_LOCALINFO *rgLocalInfo,
            /* [out][in] */ __RPC__inout ULONG *pceltFetched) = 0;
        
        virtual HRESULT STDMETHODCALLTYPE GetCountofExpressionContextsForMethod( 
            /* [in] */ UINT32 MethodToken,
            /* [out] */ __RPC__out ULONG *pcExprContext) = 0;
        
        virtual HRESULT STDMETHODCALLTYPE GetExpressionContextsForMethod( 
            /* [in] */ UINT32 MethodToken,
            /* [in] */ ULONG cExprContext,
            /* [length_is][size_is][out] */ __RPC__out_ecount_part(cExprContext, *pceltFetched) ENC_LOCAL_EXPR_CONTEXT *plocalExprContext,
            /* [out][in] */ __RPC__inout ULONG *pceltFetched) = 0;
        
    };
    
#else 	/* C style interface */

    typedef struct IENCDebugInfoVtbl
    {
        BEGIN_INTERFACE
        
        HRESULT ( STDMETHODCALLTYPE *QueryInterface )( 
            IENCDebugInfo * This,
            /* [in] */ __RPC__in REFIID riid,
            /* [iid_is][out] */ 
            __RPC__deref_out  void **ppvObject);
        
        ULONG ( STDMETHODCALLTYPE *AddRef )( 
            IENCDebugInfo * This);
        
        ULONG ( STDMETHODCALLTYPE *Release )( 
            IENCDebugInfo * This);
        
        HRESULT ( STDMETHODCALLTYPE *GetLocalVariableCount )( 
            IENCDebugInfo * This,
            /* [in] */ UINT32 MethodToken,
            /* [out] */ __RPC__out ULONG *pcLocals);
        
        HRESULT ( STDMETHODCALLTYPE *GetLocalVariableLayout )( 
            IENCDebugInfo * This,
            /* [in] */ UINT32 MethodToken,
            /* [in] */ ULONG cLocals,
            /* [length_is][size_is][out] */ __RPC__out_ecount_part(cLocals, *pceltFetched) ENC_LOCALINFO *rgLocalInfo,
            /* [out][in] */ __RPC__inout ULONG *pceltFetched);
        
        HRESULT ( STDMETHODCALLTYPE *GetCountofExpressionContextsForMethod )( 
            IENCDebugInfo * This,
            /* [in] */ UINT32 MethodToken,
            /* [out] */ __RPC__out ULONG *pcExprContext);
        
        HRESULT ( STDMETHODCALLTYPE *GetExpressionContextsForMethod )( 
            IENCDebugInfo * This,
            /* [in] */ UINT32 MethodToken,
            /* [in] */ ULONG cExprContext,
            /* [length_is][size_is][out] */ __RPC__out_ecount_part(cExprContext, *pceltFetched) ENC_LOCAL_EXPR_CONTEXT *plocalExprContext,
            /* [out][in] */ __RPC__inout ULONG *pceltFetched);
        
        END_INTERFACE
    } IENCDebugInfoVtbl;

    interface IENCDebugInfo
    {
        CONST_VTBL struct IENCDebugInfoVtbl *lpVtbl;
    };

    

#ifdef COBJMACROS


#define IENCDebugInfo_QueryInterface(This,riid,ppvObject)	\
    ( (This)->lpVtbl -> QueryInterface(This,riid,ppvObject) ) 

#define IENCDebugInfo_AddRef(This)	\
    ( (This)->lpVtbl -> AddRef(This) ) 

#define IENCDebugInfo_Release(This)	\
    ( (This)->lpVtbl -> Release(This) ) 


#define IENCDebugInfo_GetLocalVariableCount(This,MethodToken,pcLocals)	\
    ( (This)->lpVtbl -> GetLocalVariableCount(This,MethodToken,pcLocals) ) 

#define IENCDebugInfo_GetLocalVariableLayout(This,MethodToken,cLocals,rgLocalInfo,pceltFetched)	\
    ( (This)->lpVtbl -> GetLocalVariableLayout(This,MethodToken,cLocals,rgLocalInfo,pceltFetched) ) 

#define IENCDebugInfo_GetCountofExpressionContextsForMethod(This,MethodToken,pcExprContext)	\
    ( (This)->lpVtbl -> GetCountofExpressionContextsForMethod(This,MethodToken,pcExprContext) ) 

#define IENCDebugInfo_GetExpressionContextsForMethod(This,MethodToken,cExprContext,plocalExprContext,pceltFetched)	\
    ( (This)->lpVtbl -> GetExpressionContextsForMethod(This,MethodToken,cExprContext,plocalExprContext,pceltFetched) ) 

#endif /* COBJMACROS */


#endif 	/* C style interface */




#endif 	/* __IENCDebugInfo_INTERFACE_DEFINED__ */


/* interface __MIDL_itf_enc_0000_0018 */
/* [local] */ 


enum enum_ENCASINFO
    {	ENCASINFO_NONE	= 0,
	ENCASINFO_LEAF	= 0x1,
	ENCASINFO_MIDSTATEMENT	= 0x2,
	ENCASINFO_NONUSER	= 0x4
    } ;
typedef DWORD ENCASINFO;

typedef struct _ASTMT
    {
    UINT32 id;
    UINT32 methodToken;
    UINT32 methodVersion;
    UINT32 ilOffset;
    UINT32 startLine;
    UINT32 startCol;
    UINT32 endLine;
    UINT32 endCol;
    BSTR bstrfilename;
    ENCASINFO encASinfo;
    BOOL methodUpToDate;
    UINT64 engineInstanceID;
    INT32 delta;
    } 	ENCPROG_ACTIVE_STATEMENT;

typedef struct _EXCPTRANGE
    {
    UINT32 methodToken;
    UINT32 methodVersion;
    UINT32 startLine;
    UINT32 startCol;
    UINT32 endLine;
    UINT32 endCol;
    INT32 delta;
    } 	ENCPROG_EXCEPTION_RANGE;

typedef struct _LINEDELTA
    {
    UINT32 mdMethod;
    INT32 delta;
    } 	LINEDELTA;

typedef struct _LINEUPDATE
    {
    UINT32 line;
    UINT32 updatedLine;
    } 	LINEUPDATE;

typedef struct _FILEUPDATE
    {
    BSTR bstrFileName;
    LINEUPDATE *pLineUpdates;
    UINT32 cLineUpdate;
    } 	FILEUPDATE;



extern RPC_IF_HANDLE __MIDL_itf_enc_0000_0018_v0_0_c_ifspec;
extern RPC_IF_HANDLE __MIDL_itf_enc_0000_0018_v0_0_s_ifspec;

#ifndef __IDebugENCSymbolProvider2_INTERFACE_DEFINED__
#define __IDebugENCSymbolProvider2_INTERFACE_DEFINED__

/* interface IDebugENCSymbolProvider2 */
/* [unique][uuid][object] */ 


EXTERN_C const IID IID_IDebugENCSymbolProvider2;

#if defined(__cplusplus) && !defined(CINTERFACE)
    
    MIDL_INTERFACE("FF6D3520-E8D9-4e8c-BB75-CFFA7B03C633")
    IDebugENCSymbolProvider2 : public IUnknown
    {
    public:
        virtual HRESULT STDMETHODCALLTYPE UpdateSymbols2( 
            /* [in] */ ULONG32 ulAppDomainID,
            /* [in] */ GUID guidModule,
            /* [in] */ __RPC__in_opt IStream *pUpdateStream,
            /* [length_is][size_is][in] */ __RPC__in_ecount_part(cDeltaLines, cDeltaLines) LINEDELTA *pDeltaLines,
            /* [in] */ ULONG cDeltaLines) = 0;
        
        virtual HRESULT STDMETHODCALLTYPE GetFileLineFromOffset( 
            /* [in] */ ULONG32 ulAppDomainID,
            /* [in] */ GUID guidModule,
            /* [in] */ DWORD mdMethod,
            /* [in] */ DWORD dwVersion,
            /* [in] */ DWORD dwOffset,
            /* [out] */ __RPC__out DWORD *pdwStartLine,
            /* [out] */ __RPC__out DWORD *pdwStartCol,
            /* [out] */ __RPC__out DWORD *pdwEndLine,
            /* [out] */ __RPC__out DWORD *pdwEndCol,
            /* [out] */ __RPC__deref_out_opt BSTR *pbstrFileName,
            /* [out] */ __RPC__out DWORD *pdwMappedOffset,
            /* [out] */ __RPC__out BOOL *pfMethodUpToDate) = 0;
        
        virtual HRESULT STDMETHODCALLTYPE Initialize( 
            /* [in] */ ULONG32 ulAppDomainID,
            /* [in] */ GUID guidModule) = 0;
        
    };
    
#else 	/* C style interface */

    typedef struct IDebugENCSymbolProvider2Vtbl
    {
        BEGIN_INTERFACE
        
        HRESULT ( STDMETHODCALLTYPE *QueryInterface )( 
            IDebugENCSymbolProvider2 * This,
            /* [in] */ __RPC__in REFIID riid,
            /* [iid_is][out] */ 
            __RPC__deref_out  void **ppvObject);
        
        ULONG ( STDMETHODCALLTYPE *AddRef )( 
            IDebugENCSymbolProvider2 * This);
        
        ULONG ( STDMETHODCALLTYPE *Release )( 
            IDebugENCSymbolProvider2 * This);
        
        HRESULT ( STDMETHODCALLTYPE *UpdateSymbols2 )( 
            IDebugENCSymbolProvider2 * This,
            /* [in] */ ULONG32 ulAppDomainID,
            /* [in] */ GUID guidModule,
            /* [in] */ __RPC__in_opt IStream *pUpdateStream,
            /* [length_is][size_is][in] */ __RPC__in_ecount_part(cDeltaLines, cDeltaLines) LINEDELTA *pDeltaLines,
            /* [in] */ ULONG cDeltaLines);
        
        HRESULT ( STDMETHODCALLTYPE *GetFileLineFromOffset )( 
            IDebugENCSymbolProvider2 * This,
            /* [in] */ ULONG32 ulAppDomainID,
            /* [in] */ GUID guidModule,
            /* [in] */ DWORD mdMethod,
            /* [in] */ DWORD dwVersion,
            /* [in] */ DWORD dwOffset,
            /* [out] */ __RPC__out DWORD *pdwStartLine,
            /* [out] */ __RPC__out DWORD *pdwStartCol,
            /* [out] */ __RPC__out DWORD *pdwEndLine,
            /* [out] */ __RPC__out DWORD *pdwEndCol,
            /* [out] */ __RPC__deref_out_opt BSTR *pbstrFileName,
            /* [out] */ __RPC__out DWORD *pdwMappedOffset,
            /* [out] */ __RPC__out BOOL *pfMethodUpToDate);
        
        HRESULT ( STDMETHODCALLTYPE *Initialize )( 
            IDebugENCSymbolProvider2 * This,
            /* [in] */ ULONG32 ulAppDomainID,
            /* [in] */ GUID guidModule);
        
        END_INTERFACE
    } IDebugENCSymbolProvider2Vtbl;

    interface IDebugENCSymbolProvider2
    {
        CONST_VTBL struct IDebugENCSymbolProvider2Vtbl *lpVtbl;
    };

    

#ifdef COBJMACROS


#define IDebugENCSymbolProvider2_QueryInterface(This,riid,ppvObject)	\
    ( (This)->lpVtbl -> QueryInterface(This,riid,ppvObject) ) 

#define IDebugENCSymbolProvider2_AddRef(This)	\
    ( (This)->lpVtbl -> AddRef(This) ) 

#define IDebugENCSymbolProvider2_Release(This)	\
    ( (This)->lpVtbl -> Release(This) ) 


#define IDebugENCSymbolProvider2_UpdateSymbols2(This,ulAppDomainID,guidModule,pUpdateStream,pDeltaLines,cDeltaLines)	\
    ( (This)->lpVtbl -> UpdateSymbols2(This,ulAppDomainID,guidModule,pUpdateStream,pDeltaLines,cDeltaLines) ) 

#define IDebugENCSymbolProvider2_GetFileLineFromOffset(This,ulAppDomainID,guidModule,mdMethod,dwVersion,dwOffset,pdwStartLine,pdwStartCol,pdwEndLine,pdwEndCol,pbstrFileName,pdwMappedOffset,pfMethodUpToDate)	\
    ( (This)->lpVtbl -> GetFileLineFromOffset(This,ulAppDomainID,guidModule,mdMethod,dwVersion,dwOffset,pdwStartLine,pdwStartCol,pdwEndLine,pdwEndCol,pbstrFileName,pdwMappedOffset,pfMethodUpToDate) ) 

#define IDebugENCSymbolProvider2_Initialize(This,ulAppDomainID,guidModule)	\
    ( (This)->lpVtbl -> Initialize(This,ulAppDomainID,guidModule) ) 

#endif /* COBJMACROS */


#endif 	/* C style interface */




#endif 	/* __IDebugENCSymbolProvider2_INTERFACE_DEFINED__ */


#ifndef __IDebugENCModule_INTERFACE_DEFINED__
#define __IDebugENCModule_INTERFACE_DEFINED__

/* interface IDebugENCModule */
/* [unique][uuid][object] */ 


EXTERN_C const IID IID_IDebugENCModule;

#if defined(__cplusplus) && !defined(CINTERFACE)
    
    MIDL_INTERFACE("95B8097D-3201-4b21-887C-239EE0A0D589")
    IDebugENCModule : public IUnknown
    {
    public:
        virtual HRESULT STDMETHODCALLTYPE GetActiveStatementCount( 
            /* [out] */ __RPC__out ULONG *cActiveStatements) = 0;
        
        virtual HRESULT STDMETHODCALLTYPE GetActiveStatements( 
            /* [in] */ ULONG celt,
            /* [length_is][size_is][out] */ __RPC__out_ecount_part(celt, *pceltFetched) ENCPROG_ACTIVE_STATEMENT *rgelt,
            /* [out][in] */ __RPC__inout ULONG *pceltFetched) = 0;
        
        virtual HRESULT STDMETHODCALLTYPE GetMetadataByteCount( 
            /* [out] */ __RPC__out ULONG *cb) = 0;
        
        virtual HRESULT STDMETHODCALLTYPE GetMetadataBytes( 
            /* [in] */ ULONG cb,
            /* [length_is][size_is][out] */ __RPC__out_ecount_part(cb, *cbFetched) BYTE *pbMetadata,
            /* [out][in] */ __RPC__inout ULONG *cbFetched) = 0;
        
        virtual HRESULT STDMETHODCALLTYPE GetENCDebugInfo( 
            /* [out] */ __RPC__deref_out_opt IENCDebugInfo **ppENCDebugInfo) = 0;
        
        virtual HRESULT STDMETHODCALLTYPE ApplyENCUpdate( 
            /* [size_is][in] */ __RPC__in_ecount_full(cbDeltaIL) BYTE *pbDeltaIL,
            /* [in] */ ULONG cbDeltaIL,
            /* [size_is][in] */ __RPC__in_ecount_full(cbDeltaMeta) BYTE *pbDeltaMeta,
            /* [in] */ ULONG cbDeltaMeta,
            /* [size_is][in] */ __RPC__in_ecount_full(cbDeltaPdb) BYTE *pbDeltaPdb,
            /* [in] */ ULONG cbDeltaPdb,
            /* [size_is][in] */ __RPC__in_ecount_full(cDeltaLines) LINEDELTA *pDeltaLines,
            /* [in] */ ULONG cDeltaLines,
            /* [size_is][in] */ __RPC__in_ecount_full(cRemapStatements) ENCPROG_ACTIVE_STATEMENT *pRemapStatements,
            /* [in] */ ULONG cRemapStatements,
            /* [size_is][in] */ __RPC__in_ecount_full(cExceptionRanges) ENCPROG_EXCEPTION_RANGE *pExceptionRanges,
            /* [in] */ ULONG cExceptionRanges,
            /* [size_is][in] */ __RPC__in_ecount_full(cRemapMethods) UINT32 *pmdRemapMethods,
            /* [in] */ ULONG cRemapMethods,
            /* [size_is][in] */ __RPC__in_ecount_full(cFileUpdates) FILEUPDATE *pFileUpdates,
            /* [in] */ ULONG32 cFileUpdates) = 0;
        
        virtual HRESULT STDMETHODCALLTYPE Initialize( void) = 0;
        
    };
    
#else 	/* C style interface */

    typedef struct IDebugENCModuleVtbl
    {
        BEGIN_INTERFACE
        
        HRESULT ( STDMETHODCALLTYPE *QueryInterface )( 
            IDebugENCModule * This,
            /* [in] */ __RPC__in REFIID riid,
            /* [iid_is][out] */ 
            __RPC__deref_out  void **ppvObject);
        
        ULONG ( STDMETHODCALLTYPE *AddRef )( 
            IDebugENCModule * This);
        
        ULONG ( STDMETHODCALLTYPE *Release )( 
            IDebugENCModule * This);
        
        HRESULT ( STDMETHODCALLTYPE *GetActiveStatementCount )( 
            IDebugENCModule * This,
            /* [out] */ __RPC__out ULONG *cActiveStatements);
        
        HRESULT ( STDMETHODCALLTYPE *GetActiveStatements )( 
            IDebugENCModule * This,
            /* [in] */ ULONG celt,
            /* [length_is][size_is][out] */ __RPC__out_ecount_part(celt, *pceltFetched) ENCPROG_ACTIVE_STATEMENT *rgelt,
            /* [out][in] */ __RPC__inout ULONG *pceltFetched);
        
        HRESULT ( STDMETHODCALLTYPE *GetMetadataByteCount )( 
            IDebugENCModule * This,
            /* [out] */ __RPC__out ULONG *cb);
        
        HRESULT ( STDMETHODCALLTYPE *GetMetadataBytes )( 
            IDebugENCModule * This,
            /* [in] */ ULONG cb,
            /* [length_is][size_is][out] */ __RPC__out_ecount_part(cb, *cbFetched) BYTE *pbMetadata,
            /* [out][in] */ __RPC__inout ULONG *cbFetched);
        
        HRESULT ( STDMETHODCALLTYPE *GetENCDebugInfo )( 
            IDebugENCModule * This,
            /* [out] */ __RPC__deref_out_opt IENCDebugInfo **ppENCDebugInfo);
        
        HRESULT ( STDMETHODCALLTYPE *ApplyENCUpdate )( 
            IDebugENCModule * This,
            /* [size_is][in] */ __RPC__in_ecount_full(cbDeltaIL) BYTE *pbDeltaIL,
            /* [in] */ ULONG cbDeltaIL,
            /* [size_is][in] */ __RPC__in_ecount_full(cbDeltaMeta) BYTE *pbDeltaMeta,
            /* [in] */ ULONG cbDeltaMeta,
            /* [size_is][in] */ __RPC__in_ecount_full(cbDeltaPdb) BYTE *pbDeltaPdb,
            /* [in] */ ULONG cbDeltaPdb,
            /* [size_is][in] */ __RPC__in_ecount_full(cDeltaLines) LINEDELTA *pDeltaLines,
            /* [in] */ ULONG cDeltaLines,
            /* [size_is][in] */ __RPC__in_ecount_full(cRemapStatements) ENCPROG_ACTIVE_STATEMENT *pRemapStatements,
            /* [in] */ ULONG cRemapStatements,
            /* [size_is][in] */ __RPC__in_ecount_full(cExceptionRanges) ENCPROG_EXCEPTION_RANGE *pExceptionRanges,
            /* [in] */ ULONG cExceptionRanges,
            /* [size_is][in] */ __RPC__in_ecount_full(cRemapMethods) UINT32 *pmdRemapMethods,
            /* [in] */ ULONG cRemapMethods,
            /* [size_is][in] */ __RPC__in_ecount_full(cFileUpdates) FILEUPDATE *pFileUpdates,
            /* [in] */ ULONG32 cFileUpdates);
        
        HRESULT ( STDMETHODCALLTYPE *Initialize )( 
            IDebugENCModule * This);
        
        END_INTERFACE
    } IDebugENCModuleVtbl;

    interface IDebugENCModule
    {
        CONST_VTBL struct IDebugENCModuleVtbl *lpVtbl;
    };

    

#ifdef COBJMACROS


#define IDebugENCModule_QueryInterface(This,riid,ppvObject)	\
    ( (This)->lpVtbl -> QueryInterface(This,riid,ppvObject) ) 

#define IDebugENCModule_AddRef(This)	\
    ( (This)->lpVtbl -> AddRef(This) ) 

#define IDebugENCModule_Release(This)	\
    ( (This)->lpVtbl -> Release(This) ) 


#define IDebugENCModule_GetActiveStatementCount(This,cActiveStatements)	\
    ( (This)->lpVtbl -> GetActiveStatementCount(This,cActiveStatements) ) 

#define IDebugENCModule_GetActiveStatements(This,celt,rgelt,pceltFetched)	\
    ( (This)->lpVtbl -> GetActiveStatements(This,celt,rgelt,pceltFetched) ) 

#define IDebugENCModule_GetMetadataByteCount(This,cb)	\
    ( (This)->lpVtbl -> GetMetadataByteCount(This,cb) ) 

#define IDebugENCModule_GetMetadataBytes(This,cb,pbMetadata,cbFetched)	\
    ( (This)->lpVtbl -> GetMetadataBytes(This,cb,pbMetadata,cbFetched) ) 

#define IDebugENCModule_GetENCDebugInfo(This,ppENCDebugInfo)	\
    ( (This)->lpVtbl -> GetENCDebugInfo(This,ppENCDebugInfo) ) 

#define IDebugENCModule_ApplyENCUpdate(This,pbDeltaIL,cbDeltaIL,pbDeltaMeta,cbDeltaMeta,pbDeltaPdb,cbDeltaPdb,pDeltaLines,cDeltaLines,pRemapStatements,cRemapStatements,pExceptionRanges,cExceptionRanges,pmdRemapMethods,cRemapMethods,pFileUpdates,cFileUpdates)	\
    ( (This)->lpVtbl -> ApplyENCUpdate(This,pbDeltaIL,cbDeltaIL,pbDeltaMeta,cbDeltaMeta,pbDeltaPdb,cbDeltaPdb,pDeltaLines,cDeltaLines,pRemapStatements,cRemapStatements,pExceptionRanges,cExceptionRanges,pmdRemapMethods,cRemapMethods,pFileUpdates,cFileUpdates) ) 

#define IDebugENCModule_Initialize(This)	\
    ( (This)->lpVtbl -> Initialize(This) ) 

#endif /* COBJMACROS */


#endif 	/* C style interface */




#endif 	/* __IDebugENCModule_INTERFACE_DEFINED__ */


#ifndef __IDebugCustomENCModule100_INTERFACE_DEFINED__
#define __IDebugCustomENCModule100_INTERFACE_DEFINED__

/* interface IDebugCustomENCModule100 */
/* [unique][uuid][object] */ 


EXTERN_C const IID IID_IDebugCustomENCModule100;

#if defined(__cplusplus) && !defined(CINTERFACE)
    
    MIDL_INTERFACE("A39A3DDD-AF4D-48FA-BC0D-7AFD3FCDEE9B")
    IDebugCustomENCModule100 : public IUnknown
    {
    public:
        virtual HRESULT STDMETHODCALLTYPE GetEmptyDelta( 
            /* [out] */ __RPC__deref_out_opt IUnknown **uninitializedDelta) = 0;
        
        virtual HRESULT STDMETHODCALLTYPE GetRemapMethodCount( 
            /* [out] */ __RPC__out ULONG *cb) = 0;
        
        virtual HRESULT STDMETHODCALLTYPE GetDeltaForApply( 
            /* [out] */ __RPC__deref_out_opt IUnknown **ppAbstractDelta,
            /* [length_is][size_is][out] */ __RPC__out_ecount_part(cRemapMethods, cRemapMethods) UINT32 *pmdRemapMethods,
            /* [in] */ ULONG cRemapMethods) = 0;
        
        virtual HRESULT STDMETHODCALLTYPE Apply( 
            /* [in] */ __RPC__in_opt IUnknown *pAbstractDelta,
            /* [in] */ __RPC__in ENCPROG_ACTIVE_STATEMENT *pRemapStatements,
            /* [in] */ ULONG cRemapStatements,
            /* [in] */ __RPC__in ENCPROG_EXCEPTION_RANGE *pExceptionRanges,
            /* [in] */ ULONG cExceptionRanges) = 0;
        
    };
    
#else 	/* C style interface */

    typedef struct IDebugCustomENCModule100Vtbl
    {
        BEGIN_INTERFACE
        
        HRESULT ( STDMETHODCALLTYPE *QueryInterface )( 
            IDebugCustomENCModule100 * This,
            /* [in] */ __RPC__in REFIID riid,
            /* [iid_is][out] */ 
            __RPC__deref_out  void **ppvObject);
        
        ULONG ( STDMETHODCALLTYPE *AddRef )( 
            IDebugCustomENCModule100 * This);
        
        ULONG ( STDMETHODCALLTYPE *Release )( 
            IDebugCustomENCModule100 * This);
        
        HRESULT ( STDMETHODCALLTYPE *GetEmptyDelta )( 
            IDebugCustomENCModule100 * This,
            /* [out] */ __RPC__deref_out_opt IUnknown **uninitializedDelta);
        
        HRESULT ( STDMETHODCALLTYPE *GetRemapMethodCount )( 
            IDebugCustomENCModule100 * This,
            /* [out] */ __RPC__out ULONG *cb);
        
        HRESULT ( STDMETHODCALLTYPE *GetDeltaForApply )( 
            IDebugCustomENCModule100 * This,
            /* [out] */ __RPC__deref_out_opt IUnknown **ppAbstractDelta,
            /* [length_is][size_is][out] */ __RPC__out_ecount_part(cRemapMethods, cRemapMethods) UINT32 *pmdRemapMethods,
            /* [in] */ ULONG cRemapMethods);
        
        HRESULT ( STDMETHODCALLTYPE *Apply )( 
            IDebugCustomENCModule100 * This,
            /* [in] */ __RPC__in_opt IUnknown *pAbstractDelta,
            /* [in] */ __RPC__in ENCPROG_ACTIVE_STATEMENT *pRemapStatements,
            /* [in] */ ULONG cRemapStatements,
            /* [in] */ __RPC__in ENCPROG_EXCEPTION_RANGE *pExceptionRanges,
            /* [in] */ ULONG cExceptionRanges);
        
        END_INTERFACE
    } IDebugCustomENCModule100Vtbl;

    interface IDebugCustomENCModule100
    {
        CONST_VTBL struct IDebugCustomENCModule100Vtbl *lpVtbl;
    };

    

#ifdef COBJMACROS


#define IDebugCustomENCModule100_QueryInterface(This,riid,ppvObject)	\
    ( (This)->lpVtbl -> QueryInterface(This,riid,ppvObject) ) 

#define IDebugCustomENCModule100_AddRef(This)	\
    ( (This)->lpVtbl -> AddRef(This) ) 

#define IDebugCustomENCModule100_Release(This)	\
    ( (This)->lpVtbl -> Release(This) ) 


#define IDebugCustomENCModule100_GetEmptyDelta(This,uninitializedDelta)	\
    ( (This)->lpVtbl -> GetEmptyDelta(This,uninitializedDelta) ) 

#define IDebugCustomENCModule100_GetRemapMethodCount(This,cb)	\
    ( (This)->lpVtbl -> GetRemapMethodCount(This,cb) ) 

#define IDebugCustomENCModule100_GetDeltaForApply(This,ppAbstractDelta,pmdRemapMethods,cRemapMethods)	\
    ( (This)->lpVtbl -> GetDeltaForApply(This,ppAbstractDelta,pmdRemapMethods,cRemapMethods) ) 

#define IDebugCustomENCModule100_Apply(This,pAbstractDelta,pRemapStatements,cRemapStatements,pExceptionRanges,cExceptionRanges)	\
    ( (This)->lpVtbl -> Apply(This,pAbstractDelta,pRemapStatements,cRemapStatements,pExceptionRanges,cExceptionRanges) ) 

#endif /* COBJMACROS */


#endif 	/* C style interface */




#endif 	/* __IDebugCustomENCModule100_INTERFACE_DEFINED__ */


#ifndef __IVsENCRebuildableProjectCfg3_INTERFACE_DEFINED__
#define __IVsENCRebuildableProjectCfg3_INTERFACE_DEFINED__

/* interface IVsENCRebuildableProjectCfg3 */
/* [unique][uuid][object][local] */ 


EXTERN_C const IID IID_IVsENCRebuildableProjectCfg3;

#if defined(__cplusplus) && !defined(CINTERFACE)
    
    MIDL_INTERFACE("12342E6D-563A-4195-B267-D080383DD437")
    IVsENCRebuildableProjectCfg3 : public IUnknown
    {
    public:
        virtual HRESULT STDMETHODCALLTYPE BuildForCustomEnc( 
            /* [in] */ IDebugCustomENCModule100 *pModule) = 0;
        
    };
    
#else 	/* C style interface */

    typedef struct IVsENCRebuildableProjectCfg3Vtbl
    {
        BEGIN_INTERFACE
        
        HRESULT ( STDMETHODCALLTYPE *QueryInterface )( 
            IVsENCRebuildableProjectCfg3 * This,
            /* [in] */ REFIID riid,
            /* [iid_is][out] */ 
            __RPC__deref_out  void **ppvObject);
        
        ULONG ( STDMETHODCALLTYPE *AddRef )( 
            IVsENCRebuildableProjectCfg3 * This);
        
        ULONG ( STDMETHODCALLTYPE *Release )( 
            IVsENCRebuildableProjectCfg3 * This);
        
        HRESULT ( STDMETHODCALLTYPE *BuildForCustomEnc )( 
            IVsENCRebuildableProjectCfg3 * This,
            /* [in] */ IDebugCustomENCModule100 *pModule);
        
        END_INTERFACE
    } IVsENCRebuildableProjectCfg3Vtbl;

    interface IVsENCRebuildableProjectCfg3
    {
        CONST_VTBL struct IVsENCRebuildableProjectCfg3Vtbl *lpVtbl;
    };

    

#ifdef COBJMACROS


#define IVsENCRebuildableProjectCfg3_QueryInterface(This,riid,ppvObject)	\
    ( (This)->lpVtbl -> QueryInterface(This,riid,ppvObject) ) 

#define IVsENCRebuildableProjectCfg3_AddRef(This)	\
    ( (This)->lpVtbl -> AddRef(This) ) 

#define IVsENCRebuildableProjectCfg3_Release(This)	\
    ( (This)->lpVtbl -> Release(This) ) 


#define IVsENCRebuildableProjectCfg3_BuildForCustomEnc(This,pModule)	\
    ( (This)->lpVtbl -> BuildForCustomEnc(This,pModule) ) 

#endif /* COBJMACROS */


#endif 	/* C style interface */




#endif 	/* __IVsENCRebuildableProjectCfg3_INTERFACE_DEFINED__ */


#ifndef __IDebugManagedENC_INTERFACE_DEFINED__
#define __IDebugManagedENC_INTERFACE_DEFINED__

/* interface IDebugManagedENC */
/* [unique][uuid][object] */ 


EXTERN_C const IID IID_IDebugManagedENC;

#if defined(__cplusplus) && !defined(CINTERFACE)
    
    MIDL_INTERFACE("12DCD8B7-EBFC-4dbe-A72C-3E44CDD3CBAF")
    IDebugManagedENC : public IUnknown
    {
    public:
        virtual HRESULT STDMETHODCALLTYPE GetENCModule( 
            /* [in] */ GUID moduleID,
            /* [in] */ __RPC__in LPCOLESTR strPEname,
            /* [in] */ __RPC__in FILETIME *pBuiltFileTime,
            /* [out] */ __RPC__deref_out_opt IDebugENCModule **ppENCModule) = 0;
        
    };
    
#else 	/* C style interface */

    typedef struct IDebugManagedENCVtbl
    {
        BEGIN_INTERFACE
        
        HRESULT ( STDMETHODCALLTYPE *QueryInterface )( 
            IDebugManagedENC * This,
            /* [in] */ __RPC__in REFIID riid,
            /* [iid_is][out] */ 
            __RPC__deref_out  void **ppvObject);
        
        ULONG ( STDMETHODCALLTYPE *AddRef )( 
            IDebugManagedENC * This);
        
        ULONG ( STDMETHODCALLTYPE *Release )( 
            IDebugManagedENC * This);
        
        HRESULT ( STDMETHODCALLTYPE *GetENCModule )( 
            IDebugManagedENC * This,
            /* [in] */ GUID moduleID,
            /* [in] */ __RPC__in LPCOLESTR strPEname,
            /* [in] */ __RPC__in FILETIME *pBuiltFileTime,
            /* [out] */ __RPC__deref_out_opt IDebugENCModule **ppENCModule);
        
        END_INTERFACE
    } IDebugManagedENCVtbl;

    interface IDebugManagedENC
    {
        CONST_VTBL struct IDebugManagedENCVtbl *lpVtbl;
    };

    

#ifdef COBJMACROS


#define IDebugManagedENC_QueryInterface(This,riid,ppvObject)	\
    ( (This)->lpVtbl -> QueryInterface(This,riid,ppvObject) ) 

#define IDebugManagedENC_AddRef(This)	\
    ( (This)->lpVtbl -> AddRef(This) ) 

#define IDebugManagedENC_Release(This)	\
    ( (This)->lpVtbl -> Release(This) ) 


#define IDebugManagedENC_GetENCModule(This,moduleID,strPEname,pBuiltFileTime,ppENCModule)	\
    ( (This)->lpVtbl -> GetENCModule(This,moduleID,strPEname,pBuiltFileTime,ppENCModule) ) 

#endif /* COBJMACROS */


#endif 	/* C style interface */




#endif 	/* __IDebugManagedENC_INTERFACE_DEFINED__ */


#ifndef __IDebugUpdateInMemoryPE_INTERFACE_DEFINED__
#define __IDebugUpdateInMemoryPE_INTERFACE_DEFINED__

/* interface IDebugUpdateInMemoryPE */
/* [unique][uuid][object] */ 


EXTERN_C const IID IID_IDebugUpdateInMemoryPE;

#if defined(__cplusplus) && !defined(CINTERFACE)
    
    MIDL_INTERFACE("9E2BD568-7CEE-4166-ABC9-495BA8D3054A")
    IDebugUpdateInMemoryPE : public IUnknown
    {
    public:
        virtual HRESULT STDMETHODCALLTYPE GetMetadataEmit( 
            /* [out] */ __RPC__deref_out_opt IUnknown **ppMetadataEmit) = 0;
        
        virtual HRESULT STDMETHODCALLTYPE SetDeltaIL( 
            /* [size_is][in] */ __RPC__in_ecount_full(cbIL) BYTE *pbIL,
            /* [in] */ ULONG32 cbIL) = 0;
        
        virtual HRESULT STDMETHODCALLTYPE SetDeltaPdb( 
            /* [in] */ __RPC__in_opt IStream *pDeltaPdbStream) = 0;
        
        virtual HRESULT STDMETHODCALLTYPE SetDeltaLines( 
            /* [size_is][in] */ __RPC__in_ecount_full(cLineDeltas) LINEDELTA *pLineDeltas,
            /* [in] */ ULONG32 cLineDeltas) = 0;
        
        virtual HRESULT STDMETHODCALLTYPE GetENCDebugInfo( 
            /* [out] */ __RPC__deref_out_opt IENCDebugInfo **ppDebugInfo) = 0;
        
        virtual HRESULT STDMETHODCALLTYPE SetRemapMethods( 
            /* [size_is][in] */ __RPC__in_ecount_full(cRemapMethods) UINT32 *pmdRemapMethodTokens,
            /* [in] */ ULONG32 cRemapMethods) = 0;
        
        virtual HRESULT STDMETHODCALLTYPE SetFileUpdates( 
            /* [size_is][in] */ __RPC__in_ecount_full(cFileUpdates) FILEUPDATE *pFileUpdates,
            /* [in] */ ULONG32 cFileUpdates) = 0;
        
    };
    
#else 	/* C style interface */

    typedef struct IDebugUpdateInMemoryPEVtbl
    {
        BEGIN_INTERFACE
        
        HRESULT ( STDMETHODCALLTYPE *QueryInterface )( 
            IDebugUpdateInMemoryPE * This,
            /* [in] */ __RPC__in REFIID riid,
            /* [iid_is][out] */ 
            __RPC__deref_out  void **ppvObject);
        
        ULONG ( STDMETHODCALLTYPE *AddRef )( 
            IDebugUpdateInMemoryPE * This);
        
        ULONG ( STDMETHODCALLTYPE *Release )( 
            IDebugUpdateInMemoryPE * This);
        
        HRESULT ( STDMETHODCALLTYPE *GetMetadataEmit )( 
            IDebugUpdateInMemoryPE * This,
            /* [out] */ __RPC__deref_out_opt IUnknown **ppMetadataEmit);
        
        HRESULT ( STDMETHODCALLTYPE *SetDeltaIL )( 
            IDebugUpdateInMemoryPE * This,
            /* [size_is][in] */ __RPC__in_ecount_full(cbIL) BYTE *pbIL,
            /* [in] */ ULONG32 cbIL);
        
        HRESULT ( STDMETHODCALLTYPE *SetDeltaPdb )( 
            IDebugUpdateInMemoryPE * This,
            /* [in] */ __RPC__in_opt IStream *pDeltaPdbStream);
        
        HRESULT ( STDMETHODCALLTYPE *SetDeltaLines )( 
            IDebugUpdateInMemoryPE * This,
            /* [size_is][in] */ __RPC__in_ecount_full(cLineDeltas) LINEDELTA *pLineDeltas,
            /* [in] */ ULONG32 cLineDeltas);
        
        HRESULT ( STDMETHODCALLTYPE *GetENCDebugInfo )( 
            IDebugUpdateInMemoryPE * This,
            /* [out] */ __RPC__deref_out_opt IENCDebugInfo **ppDebugInfo);
        
        HRESULT ( STDMETHODCALLTYPE *SetRemapMethods )( 
            IDebugUpdateInMemoryPE * This,
            /* [size_is][in] */ __RPC__in_ecount_full(cRemapMethods) UINT32 *pmdRemapMethodTokens,
            /* [in] */ ULONG32 cRemapMethods);
        
        HRESULT ( STDMETHODCALLTYPE *SetFileUpdates )( 
            IDebugUpdateInMemoryPE * This,
            /* [size_is][in] */ __RPC__in_ecount_full(cFileUpdates) FILEUPDATE *pFileUpdates,
            /* [in] */ ULONG32 cFileUpdates);
        
        END_INTERFACE
    } IDebugUpdateInMemoryPEVtbl;

    interface IDebugUpdateInMemoryPE
    {
        CONST_VTBL struct IDebugUpdateInMemoryPEVtbl *lpVtbl;
    };

    

#ifdef COBJMACROS


#define IDebugUpdateInMemoryPE_QueryInterface(This,riid,ppvObject)	\
    ( (This)->lpVtbl -> QueryInterface(This,riid,ppvObject) ) 

#define IDebugUpdateInMemoryPE_AddRef(This)	\
    ( (This)->lpVtbl -> AddRef(This) ) 

#define IDebugUpdateInMemoryPE_Release(This)	\
    ( (This)->lpVtbl -> Release(This) ) 


#define IDebugUpdateInMemoryPE_GetMetadataEmit(This,ppMetadataEmit)	\
    ( (This)->lpVtbl -> GetMetadataEmit(This,ppMetadataEmit) ) 

#define IDebugUpdateInMemoryPE_SetDeltaIL(This,pbIL,cbIL)	\
    ( (This)->lpVtbl -> SetDeltaIL(This,pbIL,cbIL) ) 

#define IDebugUpdateInMemoryPE_SetDeltaPdb(This,pDeltaPdbStream)	\
    ( (This)->lpVtbl -> SetDeltaPdb(This,pDeltaPdbStream) ) 

#define IDebugUpdateInMemoryPE_SetDeltaLines(This,pLineDeltas,cLineDeltas)	\
    ( (This)->lpVtbl -> SetDeltaLines(This,pLineDeltas,cLineDeltas) ) 

#define IDebugUpdateInMemoryPE_GetENCDebugInfo(This,ppDebugInfo)	\
    ( (This)->lpVtbl -> GetENCDebugInfo(This,ppDebugInfo) ) 

#define IDebugUpdateInMemoryPE_SetRemapMethods(This,pmdRemapMethodTokens,cRemapMethods)	\
    ( (This)->lpVtbl -> SetRemapMethods(This,pmdRemapMethodTokens,cRemapMethods) ) 

#define IDebugUpdateInMemoryPE_SetFileUpdates(This,pFileUpdates,cFileUpdates)	\
    ( (This)->lpVtbl -> SetFileUpdates(This,pFileUpdates,cFileUpdates) ) 

#endif /* COBJMACROS */


#endif 	/* C style interface */




#endif 	/* __IDebugUpdateInMemoryPE_INTERFACE_DEFINED__ */


#ifndef __IDebugComPlusSnapshot2_INTERFACE_DEFINED__
#define __IDebugComPlusSnapshot2_INTERFACE_DEFINED__

/* interface IDebugComPlusSnapshot2 */
/* [unique][uuid][object] */ 

typedef struct _IL_MAP
    {
    ULONG32 oldOffset;
    ULONG32 newOffset;
    BOOL fAccurate;
    } 	IL_MAP;


EXTERN_C const IID IID_IDebugComPlusSnapshot2;

#if defined(__cplusplus) && !defined(CINTERFACE)
    
    MIDL_INTERFACE("f6f94d20-78c2-11d2-8ffe-00c04fa38314")
    IDebugComPlusSnapshot2 : public IUnknown
    {
    public:
        virtual HRESULT STDMETHODCALLTYPE CopyMetaData( 
            /* [in] */ __RPC__in_opt IStream *pIStream,
            /* [out] */ __RPC__out GUID *pMvid) = 0;
        
        virtual HRESULT STDMETHODCALLTYPE GetMvid( 
            /* [out] */ __RPC__out GUID *pMvid) = 0;
        
        virtual HRESULT STDMETHODCALLTYPE GetRoDataRVA( 
            /* [out] */ __RPC__out ULONG32 *pRoDataRVA) = 0;
        
        virtual HRESULT STDMETHODCALLTYPE GetRwDataRVA( 
            /* [out] */ __RPC__out ULONG32 *pRwDataRVA) = 0;
        
        virtual HRESULT STDMETHODCALLTYPE SetPEBytes( 
            /* [length_is][size_is][in] */ __RPC__in_ecount_part(dwBytes, dwBytes) BYTE *pBytes,
            /* [in] */ DWORD dwBytes) = 0;
        
        virtual HRESULT STDMETHODCALLTYPE SetILMap( 
            /* [in] */ DWORD mdFunction,
            /* [in] */ ULONG cMapSize,
            /* [size_is][in] */ __RPC__in_ecount_full(cMapSize) IL_MAP map[  ]) = 0;
        
        virtual HRESULT STDMETHODCALLTYPE SetSymbolBytes( 
            /* [length_is][size_is][in] */ __RPC__in_ecount_part(dwBytes, dwBytes) BYTE *pBytes,
            /* [in] */ DWORD dwBytes) = 0;
        
        virtual HRESULT STDMETHODCALLTYPE GetSymbolProvider( 
            /* [out] */ __RPC__deref_out_opt IDebugComPlusSymbolProvider **ppSym) = 0;
        
        virtual HRESULT STDMETHODCALLTYPE GetAppDomainAndModuleIDs( 
            /* [out] */ __RPC__out ULONG32 *pulAppDomainID,
            /* [out] */ __RPC__out GUID *pguidModule) = 0;
        
        virtual HRESULT STDMETHODCALLTYPE RequestILMap( 
            /* [in] */ DWORD mdFunction) = 0;
        
        virtual HRESULT STDMETHODCALLTYPE CreateILMaps( 
            /* [in] */ ULONG in_NoOfLineMaps,
            /* [size_is][in] */ __RPC__in_ecount_full(in_NoOfLineMaps) IDebugENCLineMap **in_ArrayOfLineMaps) = 0;
        
    };
    
#else 	/* C style interface */

    typedef struct IDebugComPlusSnapshot2Vtbl
    {
        BEGIN_INTERFACE
        
        HRESULT ( STDMETHODCALLTYPE *QueryInterface )( 
            IDebugComPlusSnapshot2 * This,
            /* [in] */ __RPC__in REFIID riid,
            /* [iid_is][out] */ 
            __RPC__deref_out  void **ppvObject);
        
        ULONG ( STDMETHODCALLTYPE *AddRef )( 
            IDebugComPlusSnapshot2 * This);
        
        ULONG ( STDMETHODCALLTYPE *Release )( 
            IDebugComPlusSnapshot2 * This);
        
        HRESULT ( STDMETHODCALLTYPE *CopyMetaData )( 
            IDebugComPlusSnapshot2 * This,
            /* [in] */ __RPC__in_opt IStream *pIStream,
            /* [out] */ __RPC__out GUID *pMvid);
        
        HRESULT ( STDMETHODCALLTYPE *GetMvid )( 
            IDebugComPlusSnapshot2 * This,
            /* [out] */ __RPC__out GUID *pMvid);
        
        HRESULT ( STDMETHODCALLTYPE *GetRoDataRVA )( 
            IDebugComPlusSnapshot2 * This,
            /* [out] */ __RPC__out ULONG32 *pRoDataRVA);
        
        HRESULT ( STDMETHODCALLTYPE *GetRwDataRVA )( 
            IDebugComPlusSnapshot2 * This,
            /* [out] */ __RPC__out ULONG32 *pRwDataRVA);
        
        HRESULT ( STDMETHODCALLTYPE *SetPEBytes )( 
            IDebugComPlusSnapshot2 * This,
            /* [length_is][size_is][in] */ __RPC__in_ecount_part(dwBytes, dwBytes) BYTE *pBytes,
            /* [in] */ DWORD dwBytes);
        
        HRESULT ( STDMETHODCALLTYPE *SetILMap )( 
            IDebugComPlusSnapshot2 * This,
            /* [in] */ DWORD mdFunction,
            /* [in] */ ULONG cMapSize,
            /* [size_is][in] */ __RPC__in_ecount_full(cMapSize) IL_MAP map[  ]);
        
        HRESULT ( STDMETHODCALLTYPE *SetSymbolBytes )( 
            IDebugComPlusSnapshot2 * This,
            /* [length_is][size_is][in] */ __RPC__in_ecount_part(dwBytes, dwBytes) BYTE *pBytes,
            /* [in] */ DWORD dwBytes);
        
        HRESULT ( STDMETHODCALLTYPE *GetSymbolProvider )( 
            IDebugComPlusSnapshot2 * This,
            /* [out] */ __RPC__deref_out_opt IDebugComPlusSymbolProvider **ppSym);
        
        HRESULT ( STDMETHODCALLTYPE *GetAppDomainAndModuleIDs )( 
            IDebugComPlusSnapshot2 * This,
            /* [out] */ __RPC__out ULONG32 *pulAppDomainID,
            /* [out] */ __RPC__out GUID *pguidModule);
        
        HRESULT ( STDMETHODCALLTYPE *RequestILMap )( 
            IDebugComPlusSnapshot2 * This,
            /* [in] */ DWORD mdFunction);
        
        HRESULT ( STDMETHODCALLTYPE *CreateILMaps )( 
            IDebugComPlusSnapshot2 * This,
            /* [in] */ ULONG in_NoOfLineMaps,
            /* [size_is][in] */ __RPC__in_ecount_full(in_NoOfLineMaps) IDebugENCLineMap **in_ArrayOfLineMaps);
        
        END_INTERFACE
    } IDebugComPlusSnapshot2Vtbl;

    interface IDebugComPlusSnapshot2
    {
        CONST_VTBL struct IDebugComPlusSnapshot2Vtbl *lpVtbl;
    };

    

#ifdef COBJMACROS


#define IDebugComPlusSnapshot2_QueryInterface(This,riid,ppvObject)	\
    ( (This)->lpVtbl -> QueryInterface(This,riid,ppvObject) ) 

#define IDebugComPlusSnapshot2_AddRef(This)	\
    ( (This)->lpVtbl -> AddRef(This) ) 

#define IDebugComPlusSnapshot2_Release(This)	\
    ( (This)->lpVtbl -> Release(This) ) 


#define IDebugComPlusSnapshot2_CopyMetaData(This,pIStream,pMvid)	\
    ( (This)->lpVtbl -> CopyMetaData(This,pIStream,pMvid) ) 

#define IDebugComPlusSnapshot2_GetMvid(This,pMvid)	\
    ( (This)->lpVtbl -> GetMvid(This,pMvid) ) 

#define IDebugComPlusSnapshot2_GetRoDataRVA(This,pRoDataRVA)	\
    ( (This)->lpVtbl -> GetRoDataRVA(This,pRoDataRVA) ) 

#define IDebugComPlusSnapshot2_GetRwDataRVA(This,pRwDataRVA)	\
    ( (This)->lpVtbl -> GetRwDataRVA(This,pRwDataRVA) ) 

#define IDebugComPlusSnapshot2_SetPEBytes(This,pBytes,dwBytes)	\
    ( (This)->lpVtbl -> SetPEBytes(This,pBytes,dwBytes) ) 

#define IDebugComPlusSnapshot2_SetILMap(This,mdFunction,cMapSize,map)	\
    ( (This)->lpVtbl -> SetILMap(This,mdFunction,cMapSize,map) ) 

#define IDebugComPlusSnapshot2_SetSymbolBytes(This,pBytes,dwBytes)	\
    ( (This)->lpVtbl -> SetSymbolBytes(This,pBytes,dwBytes) ) 

#define IDebugComPlusSnapshot2_GetSymbolProvider(This,ppSym)	\
    ( (This)->lpVtbl -> GetSymbolProvider(This,ppSym) ) 

#define IDebugComPlusSnapshot2_GetAppDomainAndModuleIDs(This,pulAppDomainID,pguidModule)	\
    ( (This)->lpVtbl -> GetAppDomainAndModuleIDs(This,pulAppDomainID,pguidModule) ) 

#define IDebugComPlusSnapshot2_RequestILMap(This,mdFunction)	\
    ( (This)->lpVtbl -> RequestILMap(This,mdFunction) ) 

#define IDebugComPlusSnapshot2_CreateILMaps(This,in_NoOfLineMaps,in_ArrayOfLineMaps)	\
    ( (This)->lpVtbl -> CreateILMaps(This,in_NoOfLineMaps,in_ArrayOfLineMaps) ) 

#endif /* COBJMACROS */


#endif 	/* C style interface */




#endif 	/* __IDebugComPlusSnapshot2_INTERFACE_DEFINED__ */


#ifndef __IDebugNativeSnapshot2_INTERFACE_DEFINED__
#define __IDebugNativeSnapshot2_INTERFACE_DEFINED__

/* interface IDebugNativeSnapshot2 */
/* [unique][uuid][object] */ 

typedef 
enum _ENC_NOTIFY
    {	ENC_NOTIFY_COMPILE_START	= 0,
	ENC_NOTIFY_COMPILE_END	= ( ENC_NOTIFY_COMPILE_START + 1 ) 
    } 	ENC_NOTIFY;


EXTERN_C const IID IID_IDebugNativeSnapshot2;

#if defined(__cplusplus) && !defined(CINTERFACE)
    
    MIDL_INTERFACE("461fda3e-bba5-11d2-b10f-00c04f72dc32")
    IDebugNativeSnapshot2 : public IUnknown
    {
    public:
        virtual HRESULT STDMETHODCALLTYPE HasDependentTargets( 
            /* [in] */ __RPC__in LPCOLESTR pszSourcePath) = 0;
        
        virtual HRESULT STDMETHODCALLTYPE EnumDependentImages( 
            /* [in] */ __RPC__in LPCOLESTR pszSourcePath,
            /* [out] */ __RPC__deref_out_opt IEnumString **ppEnum) = 0;
        
        virtual HRESULT STDMETHODCALLTYPE EnumDependentTargets( 
            /* [in] */ ULONG cSrc,
            /* [size_is][in] */ __RPC__in_ecount_full(cSrc) LPCOLESTR pszSourcePath[  ],
            /* [out] */ __RPC__deref_out_opt IEnumString **ppEnum) = 0;
        
        virtual HRESULT STDMETHODCALLTYPE GetTargetBuildInfo( 
            /* [in] */ __RPC__in LPCOLESTR pszTargetPath,
            /* [out] */ __RPC__deref_out_opt BSTR *pbstrSourcePath,
            /* [out] */ __RPC__deref_out_opt BSTR *pbstrCommand,
            /* [out] */ __RPC__deref_out_opt BSTR *pbstrCommandArgs,
            /* [out] */ __RPC__deref_out_opt BSTR *pbstrCurrentDir) = 0;
        
        virtual HRESULT STDMETHODCALLTYPE Notify( 
            /* [in] */ ENC_NOTIFY encnotify) = 0;
        
        virtual HRESULT STDMETHODCALLTYPE IsTargetEligible( 
            /* [in] */ __RPC__in LPCOLESTR pszTargetPath) = 0;
        
        virtual HRESULT STDMETHODCALLTYPE AddRecompiledTarget( 
            /* [in] */ __RPC__in LPCOLESTR in_szTargetPath,
            /* [in] */ __RPC__in LPCOLESTR in_szSavedTargetPath) = 0;
        
    };
    
#else 	/* C style interface */

    typedef struct IDebugNativeSnapshot2Vtbl
    {
        BEGIN_INTERFACE
        
        HRESULT ( STDMETHODCALLTYPE *QueryInterface )( 
            IDebugNativeSnapshot2 * This,
            /* [in] */ __RPC__in REFIID riid,
            /* [iid_is][out] */ 
            __RPC__deref_out  void **ppvObject);
        
        ULONG ( STDMETHODCALLTYPE *AddRef )( 
            IDebugNativeSnapshot2 * This);
        
        ULONG ( STDMETHODCALLTYPE *Release )( 
            IDebugNativeSnapshot2 * This);
        
        HRESULT ( STDMETHODCALLTYPE *HasDependentTargets )( 
            IDebugNativeSnapshot2 * This,
            /* [in] */ __RPC__in LPCOLESTR pszSourcePath);
        
        HRESULT ( STDMETHODCALLTYPE *EnumDependentImages )( 
            IDebugNativeSnapshot2 * This,
            /* [in] */ __RPC__in LPCOLESTR pszSourcePath,
            /* [out] */ __RPC__deref_out_opt IEnumString **ppEnum);
        
        HRESULT ( STDMETHODCALLTYPE *EnumDependentTargets )( 
            IDebugNativeSnapshot2 * This,
            /* [in] */ ULONG cSrc,
            /* [size_is][in] */ __RPC__in_ecount_full(cSrc) LPCOLESTR pszSourcePath[  ],
            /* [out] */ __RPC__deref_out_opt IEnumString **ppEnum);
        
        HRESULT ( STDMETHODCALLTYPE *GetTargetBuildInfo )( 
            IDebugNativeSnapshot2 * This,
            /* [in] */ __RPC__in LPCOLESTR pszTargetPath,
            /* [out] */ __RPC__deref_out_opt BSTR *pbstrSourcePath,
            /* [out] */ __RPC__deref_out_opt BSTR *pbstrCommand,
            /* [out] */ __RPC__deref_out_opt BSTR *pbstrCommandArgs,
            /* [out] */ __RPC__deref_out_opt BSTR *pbstrCurrentDir);
        
        HRESULT ( STDMETHODCALLTYPE *Notify )( 
            IDebugNativeSnapshot2 * This,
            /* [in] */ ENC_NOTIFY encnotify);
        
        HRESULT ( STDMETHODCALLTYPE *IsTargetEligible )( 
            IDebugNativeSnapshot2 * This,
            /* [in] */ __RPC__in LPCOLESTR pszTargetPath);
        
        HRESULT ( STDMETHODCALLTYPE *AddRecompiledTarget )( 
            IDebugNativeSnapshot2 * This,
            /* [in] */ __RPC__in LPCOLESTR in_szTargetPath,
            /* [in] */ __RPC__in LPCOLESTR in_szSavedTargetPath);
        
        END_INTERFACE
    } IDebugNativeSnapshot2Vtbl;

    interface IDebugNativeSnapshot2
    {
        CONST_VTBL struct IDebugNativeSnapshot2Vtbl *lpVtbl;
    };

    

#ifdef COBJMACROS


#define IDebugNativeSnapshot2_QueryInterface(This,riid,ppvObject)	\
    ( (This)->lpVtbl -> QueryInterface(This,riid,ppvObject) ) 

#define IDebugNativeSnapshot2_AddRef(This)	\
    ( (This)->lpVtbl -> AddRef(This) ) 

#define IDebugNativeSnapshot2_Release(This)	\
    ( (This)->lpVtbl -> Release(This) ) 


#define IDebugNativeSnapshot2_HasDependentTargets(This,pszSourcePath)	\
    ( (This)->lpVtbl -> HasDependentTargets(This,pszSourcePath) ) 

#define IDebugNativeSnapshot2_EnumDependentImages(This,pszSourcePath,ppEnum)	\
    ( (This)->lpVtbl -> EnumDependentImages(This,pszSourcePath,ppEnum) ) 

#define IDebugNativeSnapshot2_EnumDependentTargets(This,cSrc,pszSourcePath,ppEnum)	\
    ( (This)->lpVtbl -> EnumDependentTargets(This,cSrc,pszSourcePath,ppEnum) ) 

#define IDebugNativeSnapshot2_GetTargetBuildInfo(This,pszTargetPath,pbstrSourcePath,pbstrCommand,pbstrCommandArgs,pbstrCurrentDir)	\
    ( (This)->lpVtbl -> GetTargetBuildInfo(This,pszTargetPath,pbstrSourcePath,pbstrCommand,pbstrCommandArgs,pbstrCurrentDir) ) 

#define IDebugNativeSnapshot2_Notify(This,encnotify)	\
    ( (This)->lpVtbl -> Notify(This,encnotify) ) 

#define IDebugNativeSnapshot2_IsTargetEligible(This,pszTargetPath)	\
    ( (This)->lpVtbl -> IsTargetEligible(This,pszTargetPath) ) 

#define IDebugNativeSnapshot2_AddRecompiledTarget(This,in_szTargetPath,in_szSavedTargetPath)	\
    ( (This)->lpVtbl -> AddRecompiledTarget(This,in_szTargetPath,in_szSavedTargetPath) ) 

#endif /* COBJMACROS */


#endif 	/* C style interface */




#endif 	/* __IDebugNativeSnapshot2_INTERFACE_DEFINED__ */


#ifndef __IDebugENCStackFrame2_INTERFACE_DEFINED__
#define __IDebugENCStackFrame2_INTERFACE_DEFINED__

/* interface IDebugENCStackFrame2 */
/* [unique][uuid][object] */ 


EXTERN_C const IID IID_IDebugENCStackFrame2;

#if defined(__cplusplus) && !defined(CINTERFACE)
    
    MIDL_INTERFACE("B3C64D7F-DB9D-47c7-B479-C579C7F07103")
    IDebugENCStackFrame2 : public IUnknown
    {
    public:
        virtual HRESULT STDMETHODCALLTYPE GetAllLocalsProperty( 
            /* [out] */ __RPC__deref_out_opt IDebugProperty2 **ppProperty) = 0;
        
    };
    
#else 	/* C style interface */

    typedef struct IDebugENCStackFrame2Vtbl
    {
        BEGIN_INTERFACE
        
        HRESULT ( STDMETHODCALLTYPE *QueryInterface )( 
            IDebugENCStackFrame2 * This,
            /* [in] */ __RPC__in REFIID riid,
            /* [iid_is][out] */ 
            __RPC__deref_out  void **ppvObject);
        
        ULONG ( STDMETHODCALLTYPE *AddRef )( 
            IDebugENCStackFrame2 * This);
        
        ULONG ( STDMETHODCALLTYPE *Release )( 
            IDebugENCStackFrame2 * This);
        
        HRESULT ( STDMETHODCALLTYPE *GetAllLocalsProperty )( 
            IDebugENCStackFrame2 * This,
            /* [out] */ __RPC__deref_out_opt IDebugProperty2 **ppProperty);
        
        END_INTERFACE
    } IDebugENCStackFrame2Vtbl;

    interface IDebugENCStackFrame2
    {
        CONST_VTBL struct IDebugENCStackFrame2Vtbl *lpVtbl;
    };

    

#ifdef COBJMACROS


#define IDebugENCStackFrame2_QueryInterface(This,riid,ppvObject)	\
    ( (This)->lpVtbl -> QueryInterface(This,riid,ppvObject) ) 

#define IDebugENCStackFrame2_AddRef(This)	\
    ( (This)->lpVtbl -> AddRef(This) ) 

#define IDebugENCStackFrame2_Release(This)	\
    ( (This)->lpVtbl -> Release(This) ) 


#define IDebugENCStackFrame2_GetAllLocalsProperty(This,ppProperty)	\
    ( (This)->lpVtbl -> GetAllLocalsProperty(This,ppProperty) ) 

#endif /* COBJMACROS */


#endif 	/* C style interface */




#endif 	/* __IDebugENCStackFrame2_INTERFACE_DEFINED__ */


/* interface __MIDL_itf_enc_0000_0027 */
/* [local] */ 

typedef INT32 _mdToken;

#pragma warning(push)
#pragma warning(disable:28718)


extern RPC_IF_HANDLE __MIDL_itf_enc_0000_0027_v0_0_c_ifspec;
extern RPC_IF_HANDLE __MIDL_itf_enc_0000_0027_v0_0_s_ifspec;

#ifndef __IDebugMetaDataEmit2_INTERFACE_DEFINED__
#define __IDebugMetaDataEmit2_INTERFACE_DEFINED__

/* interface IDebugMetaDataEmit2 */
/* [unique][uuid][object] */ 

typedef struct _FIELD_OFFSET
    {
    _mdToken ridOfField;
    ULONG ulOffset;
    } 	FIELD_OFFSET;

typedef struct _IMAGE_FIXUPENTRY
    {
    ULONG ulRVA;
    ULONG Count;
    } 	IMAGE_FIXUPENTRY;


EXTERN_C const IID IID_IDebugMetaDataEmit2;

#if defined(__cplusplus) && !defined(CINTERFACE)
    
    MIDL_INTERFACE("f6f94d21-78c2-11d2-8ffe-00c04fa38314")
    IDebugMetaDataEmit2 : public IUnknown
    {
    public:
        virtual HRESULT STDMETHODCALLTYPE SetModuleProps( 
            /* [in] */ __RPC__in LPOLESTR szName,
            /* [in] */ __RPC__in GUID *ppid,
            /* [in] */ LCID lcid) = 0;
        
        virtual HRESULT STDMETHODCALLTYPE Save( 
            /* [in] */ __RPC__in LPOLESTR szFile,
            /* [in] */ DWORD dwSaveFlags) = 0;
        
        virtual HRESULT STDMETHODCALLTYPE SaveToStream( 
            /* [in] */ __RPC__in_opt IStream *pIStream,
            /* [in] */ DWORD dwSaveFlags) = 0;
        
        virtual HRESULT STDMETHODCALLTYPE GetSaveSize( 
            /* [in] */ DWORD fSave,
            /* [out] */ __RPC__out DWORD *pdwSaveSize) = 0;
        
        virtual HRESULT STDMETHODCALLTYPE DefineCustomValueAsBlob( 
            /* [in] */ _mdToken tkObj,
            /* [in] */ __RPC__in LPOLESTR szName,
            /* [size_is][in] */ __RPC__in_ecount_full(cbCustomValue) BYTE *pCustomValue,
            /* [in] */ ULONG cbCustomValue,
            /* [in] */ __RPC__in _mdToken *pcv) = 0;
        
        virtual HRESULT STDMETHODCALLTYPE DefineTypeDef( 
            /* [in] */ __RPC__in LPOLESTR szNamespace,
            /* [in] */ __RPC__in LPOLESTR szTypeDef,
            /* [in] */ __RPC__in GUID *pguid,
            /* [in] */ __RPC__in INT64 *pVer,
            /* [in] */ DWORD dwTypeDefFlags,
            /* [in] */ _mdToken tkExtends,
            /* [in] */ DWORD dwExtendsFlags,
            /* [in] */ DWORD dwImplements,
            /* [length_is][size_is][out][in] */ __RPC__inout_ecount_part(dwImplements, dwImplements) _mdToken rtkImplements[  ],
            /* [in] */ DWORD dwEvents,
            /* [length_is][size_is][out][in] */ __RPC__inout_ecount_part(dwEvents, dwEvents) _mdToken rtkEvents[  ],
            /* [out] */ __RPC__out _mdToken *ptd) = 0;
        
        virtual HRESULT STDMETHODCALLTYPE SetTypeDefProps( 
            /* [in] */ _mdToken td,
            /* [in] */ __RPC__in INT64 *pVer,
            /* [in] */ DWORD dwTypeDefFlags,
            /* [in] */ _mdToken tkExtends,
            /* [in] */ DWORD dwExtendsFlags,
            /* [in] */ DWORD dwImplements,
            /* [length_is][size_is][in] */ __RPC__in_ecount_part(dwImplements, dwImplements) _mdToken rtkImplements[  ],
            /* [in] */ DWORD dwEvents,
            /* [length_is][size_is][in] */ __RPC__in_ecount_part(dwEvents, dwEvents) _mdToken rtkEvents[  ]) = 0;
        
        virtual HRESULT STDMETHODCALLTYPE SetClassSvcsContext( 
            /* [in] */ _mdToken td,
            /* [in] */ DWORD dwClassActivateAttr,
            /* [in] */ DWORD dwClassThreadAttr,
            /* [in] */ DWORD dwXactionAttr,
            /* [in] */ DWORD dwSynchAttr) = 0;
        
        virtual HRESULT STDMETHODCALLTYPE DefineTypeRefByGUID( 
            /* [in] */ __RPC__in GUID *pguid,
            /* [out] */ __RPC__out _mdToken *ptr) = 0;
        
        virtual HRESULT STDMETHODCALLTYPE SetModuleReg( 
            /* [in] */ DWORD dwModuleRegAttr,
            /* [in] */ __RPC__in GUID *pguid) = 0;
        
        virtual HRESULT STDMETHODCALLTYPE SetClassReg( 
            /* [in] */ _mdToken td,
            /* [in] */ __RPC__in LPOLESTR szProgID,
            /* [in] */ __RPC__in LPOLESTR szVIProgID,
            /* [in] */ __RPC__in LPOLESTR szIconURL,
            /* [in] */ ULONG ulIconResource,
            /* [in] */ __RPC__in LPOLESTR szSmallIconURL,
            /* [in] */ ULONG ulSmallIconResource,
            /* [in] */ __RPC__in LPOLESTR szDefaultDispName) = 0;
        
        virtual HRESULT STDMETHODCALLTYPE SetIfaceReg( 
            /* [in] */ _mdToken td,
            /* [in] */ DWORD dwIfaceSvcs,
            /* [in] */ __RPC__in GUID *proxyStub) = 0;
        
        virtual HRESULT STDMETHODCALLTYPE SetCategoryImpl( 
            /* [in] */ _mdToken td,
            /* [in] */ DWORD dwImpl,
            /* [length_is][size_is][in] */ __RPC__in_ecount_part(dwImpl, dwImpl) GUID rGuidCoCatImpl[  ],
            /* [in] */ DWORD dwReqd,
            /* [length_is][size_is][in] */ __RPC__in_ecount_part(dwReqd, dwReqd) GUID rGuidCoCatReqd[  ]) = 0;
        
        virtual HRESULT STDMETHODCALLTYPE SetRedirectProgID( 
            /* [in] */ _mdToken td,
            /* [in] */ DWORD dwProgIds,
            /* [length_is][size_is][in] */ __RPC__in_ecount_part(dwProgIds, dwProgIds) LPOLESTR rszRedirectProgID[  ]) = 0;
        
        virtual HRESULT STDMETHODCALLTYPE SetMimeTypeImpl( 
            /* [in] */ _mdToken td,
            /* [in] */ DWORD dwTypes,
            /* [length_is][size_is][in] */ __RPC__in_ecount_part(dwTypes, dwTypes) LPOLESTR rszMimeType[  ]) = 0;
        
        virtual HRESULT STDMETHODCALLTYPE SetFormatImpl( 
            /* [in] */ _mdToken td,
            /* [in] */ DWORD dwSupported,
            /* [length_is][size_is][in] */ __RPC__in_ecount_part(dwSupported, dwSupported) LPOLESTR rszFormatSupported[  ],
            /* [in] */ DWORD dwFrom,
            /* [length_is][size_is][in] */ __RPC__in_ecount_part(dwFrom, dwFrom) LPOLESTR rszFormatConvertsFrom[  ],
            /* [in] */ DWORD dwTo,
            /* [length_is][size_is][in] */ __RPC__in_ecount_part(dwTo, dwTo) LPOLESTR rszFormatConvertsTo[  ],
            /* [in] */ DWORD dwDefault,
            /* [length_is][size_is][in] */ __RPC__in_ecount_part(dwDefault, dwDefault) LPOLESTR rszFormatDefault[  ],
            /* [in] */ DWORD dwExt,
            /* [length_is][size_is][in] */ __RPC__in_ecount_part(dwExt, dwExt) LPOLESTR rszFileExt[  ],
            /* [in] */ DWORD dwType,
            /* [length_is][size_is][in] */ __RPC__in_ecount_part(dwType, dwType) LPOLESTR rszFileType[  ]) = 0;
        
        virtual HRESULT STDMETHODCALLTYPE SetRoleCheck( 
            /* [in] */ _mdToken tk,
            /* [in] */ DWORD dwNames,
            /* [length_is][size_is][in] */ __RPC__in_ecount_part(dwNames, dwNames) LPOLESTR rszName[  ],
            /* [in] */ DWORD dwFlags,
            /* [length_is][size_is][in] */ __RPC__in_ecount_part(dwFlags, dwFlags) DWORD rdwRoleFlags[  ]) = 0;
        
        virtual HRESULT STDMETHODCALLTYPE DefineMethod( 
            /* [in] */ _mdToken td,
            /* [in] */ __RPC__in LPOLESTR szName,
            /* [in] */ DWORD dwMethodFlags,
            /* [size_is][in] */ __RPC__in_ecount_full(cbSigBlob) BYTE *pvSigBlob,
            /* [in] */ ULONG cbSigBlob,
            /* [in] */ ULONG ulSlot,
            /* [in] */ ULONG ulCodeRVA,
            /* [in] */ DWORD dwImplFlags,
            /* [out] */ __RPC__out _mdToken *pmd) = 0;
        
        virtual HRESULT STDMETHODCALLTYPE DefineField( 
            /* [in] */ _mdToken td,
            /* [in] */ __RPC__in LPOLESTR szName,
            /* [in] */ DWORD dwFieldFlags,
            /* [size_is][in] */ __RPC__in_ecount_full(cbSigBlob) BYTE *pvSigBlob,
            /* [in] */ ULONG cbSigBlob,
            /* [in] */ DWORD dwCPlusTypeFlag,
            /* [size_is][in] */ __RPC__in_ecount_full(cbValue) BYTE *pValue,
            /* [in] */ ULONG cbValue,
            /* [out] */ __RPC__out _mdToken *pmd) = 0;
        
        virtual HRESULT STDMETHODCALLTYPE SetParamProps( 
            /* [in] */ _mdToken md,
            /* [in] */ ULONG ulParamSeq,
            /* [in] */ __RPC__in LPOLESTR szName,
            /* [in] */ DWORD dwParamFlags,
            /* [in] */ DWORD dwCPlusTypeFlag,
            /* [size_is][in] */ __RPC__in_ecount_full(cbValue) BYTE *pValue,
            /* [in] */ ULONG cbValue,
            /* [out] */ __RPC__out _mdToken *ppd) = 0;
        
        virtual HRESULT STDMETHODCALLTYPE DefineMethodImpl( 
            /* [in] */ _mdToken td,
            /* [in] */ _mdToken tk,
            /* [in] */ ULONG ulCodeRVA,
            /* [in] */ DWORD dwImplFlags,
            /* [out] */ __RPC__out _mdToken *pmi) = 0;
        
        virtual HRESULT STDMETHODCALLTYPE SetRVA( 
            /* [in] */ _mdToken md,
            /* [in] */ ULONG ulCodeRVA,
            /* [in] */ DWORD dwImplFlags) = 0;
        
        virtual HRESULT STDMETHODCALLTYPE DefineTypeRefByName( 
            /* [in] */ __RPC__in LPOLESTR szNamespace,
            /* [in] */ __RPC__in LPOLESTR szType,
            /* [out] */ __RPC__out _mdToken *ptr) = 0;
        
        virtual HRESULT STDMETHODCALLTYPE SetTypeRefBind( 
            /* [in] */ _mdToken tr,
            /* [in] */ DWORD dwBindFlags,
            /* [in] */ DWORD dwMinVersion,
            /* [in] */ DWORD dwMaxVersion,
            /* [in] */ __RPC__in LPOLESTR szCodebase) = 0;
        
        virtual HRESULT STDMETHODCALLTYPE DefineMemberRef( 
            /* [in] */ _mdToken tkImport,
            /* [in] */ __RPC__in LPOLESTR szName,
            /* [size_is][in] */ __RPC__in_ecount_full(cbSigBlob) BYTE *pvSigBlob,
            /* [in] */ ULONG cbSigBlob,
            /* [out] */ __RPC__out _mdToken *pmr) = 0;
        
        virtual HRESULT STDMETHODCALLTYPE DefineException( 
            /* [in] */ _mdToken mb,
            /* [in] */ _mdToken tk,
            /* [out] */ __RPC__out _mdToken *pex) = 0;
        
        virtual HRESULT STDMETHODCALLTYPE DefineProperty( 
            /* [in] */ _mdToken td,
            /* [in] */ __RPC__in LPOLESTR szProperty,
            /* [in] */ DWORD dwPropFlags,
            /* [size_is][in] */ __RPC__in_ecount_full(cbSig) BYTE *pvSig,
            /* [in] */ ULONG cbSig,
            /* [in] */ DWORD dwCPlusTypeFlag,
            /* [size_is][in] */ __RPC__in_ecount_full(cbValue) BYTE *pValue,
            /* [in] */ ULONG cbValue,
            /* [in] */ _mdToken mdSetter,
            /* [in] */ _mdToken mdGetter,
            /* [in] */ _mdToken mdReset,
            /* [in] */ _mdToken mdTestDefault,
            /* [in] */ DWORD dwOthers,
            /* [length_is][size_is][in] */ __RPC__in_ecount_part(dwOthers, dwOthers) _mdToken rmdOtherMethods[  ],
            /* [in] */ _mdToken evNotifyChanging,
            /* [in] */ _mdToken evNotifyChanged,
            /* [in] */ _mdToken fdBackingField,
            /* [out] */ __RPC__out _mdToken *pmdProp) = 0;
        
        virtual HRESULT STDMETHODCALLTYPE DefineEvent( 
            /* [in] */ _mdToken td,
            /* [in] */ __RPC__in LPOLESTR szEvent,
            /* [in] */ DWORD dwEventFlags,
            /* [in] */ _mdToken tkEventType,
            /* [in] */ _mdToken mdAddOn,
            /* [in] */ _mdToken mdRemoveOn,
            /* [in] */ _mdToken mdFire,
            /* [in] */ DWORD dwOthers,
            /* [length_is][size_is][in] */ __RPC__in_ecount_part(dwOthers, dwOthers) _mdToken rmdOtherMethods[  ],
            /* [out] */ __RPC__out _mdToken *pmdEvent) = 0;
        
        virtual HRESULT STDMETHODCALLTYPE SetFieldMarshal( 
            /* [in] */ _mdToken tk,
            /* [size_is][in] */ __RPC__in_ecount_full(cbNativeType) BYTE *pvNativeType,
            /* [in] */ ULONG cbNativeType) = 0;
        
        virtual HRESULT STDMETHODCALLTYPE DefinePermissionSet( 
            /* [in] */ _mdToken tk,
            /* [in] */ DWORD dwAction,
            /* [size_is][in] */ __RPC__in_ecount_full(cbPermission) BYTE *pvPermission,
            /* [in] */ ULONG cbPermission,
            /* [out] */ __RPC__out _mdToken *ppm) = 0;
        
        virtual HRESULT STDMETHODCALLTYPE SetMemberIndex( 
            /* [in] */ _mdToken md,
            /* [in] */ ULONG ulIndex) = 0;
        
        virtual HRESULT STDMETHODCALLTYPE GetTokenFromSig( 
            /* [size_is][in] */ __RPC__in_ecount_full(cbSig) BYTE *pvSig,
            /* [in] */ ULONG cbSig,
            /* [in] */ __RPC__in _mdToken *pmsig) = 0;
        
        virtual HRESULT STDMETHODCALLTYPE DefineModuleRef( 
            /* [in] */ __RPC__in LPOLESTR szName,
            /* [in] */ __RPC__in GUID *pguid,
            /* [in] */ __RPC__in GUID *pmvid,
            /* [out] */ __RPC__out _mdToken *pmur) = 0;
        
        virtual HRESULT STDMETHODCALLTYPE SetParent( 
            /* [in] */ _mdToken mr,
            /* [in] */ _mdToken tk) = 0;
        
        virtual HRESULT STDMETHODCALLTYPE GetTokenFromArraySpec( 
            /* [size_is][in] */ __RPC__in_ecount_full(cbSig) BYTE *pvSig,
            /* [in] */ ULONG cbSig,
            /* [out] */ __RPC__out _mdToken *parrspec) = 0;
        
    };
    
#else 	/* C style interface */

    typedef struct IDebugMetaDataEmit2Vtbl
    {
        BEGIN_INTERFACE
        
        HRESULT ( STDMETHODCALLTYPE *QueryInterface )( 
            IDebugMetaDataEmit2 * This,
            /* [in] */ __RPC__in REFIID riid,
            /* [iid_is][out] */ 
            __RPC__deref_out  void **ppvObject);
        
        ULONG ( STDMETHODCALLTYPE *AddRef )( 
            IDebugMetaDataEmit2 * This);
        
        ULONG ( STDMETHODCALLTYPE *Release )( 
            IDebugMetaDataEmit2 * This);
        
        HRESULT ( STDMETHODCALLTYPE *SetModuleProps )( 
            IDebugMetaDataEmit2 * This,
            /* [in] */ __RPC__in LPOLESTR szName,
            /* [in] */ __RPC__in GUID *ppid,
            /* [in] */ LCID lcid);
        
        HRESULT ( STDMETHODCALLTYPE *Save )( 
            IDebugMetaDataEmit2 * This,
            /* [in] */ __RPC__in LPOLESTR szFile,
            /* [in] */ DWORD dwSaveFlags);
        
        HRESULT ( STDMETHODCALLTYPE *SaveToStream )( 
            IDebugMetaDataEmit2 * This,
            /* [in] */ __RPC__in_opt IStream *pIStream,
            /* [in] */ DWORD dwSaveFlags);
        
        HRESULT ( STDMETHODCALLTYPE *GetSaveSize )( 
            IDebugMetaDataEmit2 * This,
            /* [in] */ DWORD fSave,
            /* [out] */ __RPC__out DWORD *pdwSaveSize);
        
        HRESULT ( STDMETHODCALLTYPE *DefineCustomValueAsBlob )( 
            IDebugMetaDataEmit2 * This,
            /* [in] */ _mdToken tkObj,
            /* [in] */ __RPC__in LPOLESTR szName,
            /* [size_is][in] */ __RPC__in_ecount_full(cbCustomValue) BYTE *pCustomValue,
            /* [in] */ ULONG cbCustomValue,
            /* [in] */ __RPC__in _mdToken *pcv);
        
        HRESULT ( STDMETHODCALLTYPE *DefineTypeDef )( 
            IDebugMetaDataEmit2 * This,
            /* [in] */ __RPC__in LPOLESTR szNamespace,
            /* [in] */ __RPC__in LPOLESTR szTypeDef,
            /* [in] */ __RPC__in GUID *pguid,
            /* [in] */ __RPC__in INT64 *pVer,
            /* [in] */ DWORD dwTypeDefFlags,
            /* [in] */ _mdToken tkExtends,
            /* [in] */ DWORD dwExtendsFlags,
            /* [in] */ DWORD dwImplements,
            /* [length_is][size_is][out][in] */ __RPC__inout_ecount_part(dwImplements, dwImplements) _mdToken rtkImplements[  ],
            /* [in] */ DWORD dwEvents,
            /* [length_is][size_is][out][in] */ __RPC__inout_ecount_part(dwEvents, dwEvents) _mdToken rtkEvents[  ],
            /* [out] */ __RPC__out _mdToken *ptd);
        
        HRESULT ( STDMETHODCALLTYPE *SetTypeDefProps )( 
            IDebugMetaDataEmit2 * This,
            /* [in] */ _mdToken td,
            /* [in] */ __RPC__in INT64 *pVer,
            /* [in] */ DWORD dwTypeDefFlags,
            /* [in] */ _mdToken tkExtends,
            /* [in] */ DWORD dwExtendsFlags,
            /* [in] */ DWORD dwImplements,
            /* [length_is][size_is][in] */ __RPC__in_ecount_part(dwImplements, dwImplements) _mdToken rtkImplements[  ],
            /* [in] */ DWORD dwEvents,
            /* [length_is][size_is][in] */ __RPC__in_ecount_part(dwEvents, dwEvents) _mdToken rtkEvents[  ]);
        
        HRESULT ( STDMETHODCALLTYPE *SetClassSvcsContext )( 
            IDebugMetaDataEmit2 * This,
            /* [in] */ _mdToken td,
            /* [in] */ DWORD dwClassActivateAttr,
            /* [in] */ DWORD dwClassThreadAttr,
            /* [in] */ DWORD dwXactionAttr,
            /* [in] */ DWORD dwSynchAttr);
        
        HRESULT ( STDMETHODCALLTYPE *DefineTypeRefByGUID )( 
            IDebugMetaDataEmit2 * This,
            /* [in] */ __RPC__in GUID *pguid,
            /* [out] */ __RPC__out _mdToken *ptr);
        
        HRESULT ( STDMETHODCALLTYPE *SetModuleReg )( 
            IDebugMetaDataEmit2 * This,
            /* [in] */ DWORD dwModuleRegAttr,
            /* [in] */ __RPC__in GUID *pguid);
        
        HRESULT ( STDMETHODCALLTYPE *SetClassReg )( 
            IDebugMetaDataEmit2 * This,
            /* [in] */ _mdToken td,
            /* [in] */ __RPC__in LPOLESTR szProgID,
            /* [in] */ __RPC__in LPOLESTR szVIProgID,
            /* [in] */ __RPC__in LPOLESTR szIconURL,
            /* [in] */ ULONG ulIconResource,
            /* [in] */ __RPC__in LPOLESTR szSmallIconURL,
            /* [in] */ ULONG ulSmallIconResource,
            /* [in] */ __RPC__in LPOLESTR szDefaultDispName);
        
        HRESULT ( STDMETHODCALLTYPE *SetIfaceReg )( 
            IDebugMetaDataEmit2 * This,
            /* [in] */ _mdToken td,
            /* [in] */ DWORD dwIfaceSvcs,
            /* [in] */ __RPC__in GUID *proxyStub);
        
        HRESULT ( STDMETHODCALLTYPE *SetCategoryImpl )( 
            IDebugMetaDataEmit2 * This,
            /* [in] */ _mdToken td,
            /* [in] */ DWORD dwImpl,
            /* [length_is][size_is][in] */ __RPC__in_ecount_part(dwImpl, dwImpl) GUID rGuidCoCatImpl[  ],
            /* [in] */ DWORD dwReqd,
            /* [length_is][size_is][in] */ __RPC__in_ecount_part(dwReqd, dwReqd) GUID rGuidCoCatReqd[  ]);
        
        HRESULT ( STDMETHODCALLTYPE *SetRedirectProgID )( 
            IDebugMetaDataEmit2 * This,
            /* [in] */ _mdToken td,
            /* [in] */ DWORD dwProgIds,
            /* [length_is][size_is][in] */ __RPC__in_ecount_part(dwProgIds, dwProgIds) LPOLESTR rszRedirectProgID[  ]);
        
        HRESULT ( STDMETHODCALLTYPE *SetMimeTypeImpl )( 
            IDebugMetaDataEmit2 * This,
            /* [in] */ _mdToken td,
            /* [in] */ DWORD dwTypes,
            /* [length_is][size_is][in] */ __RPC__in_ecount_part(dwTypes, dwTypes) LPOLESTR rszMimeType[  ]);
        
        HRESULT ( STDMETHODCALLTYPE *SetFormatImpl )( 
            IDebugMetaDataEmit2 * This,
            /* [in] */ _mdToken td,
            /* [in] */ DWORD dwSupported,
            /* [length_is][size_is][in] */ __RPC__in_ecount_part(dwSupported, dwSupported) LPOLESTR rszFormatSupported[  ],
            /* [in] */ DWORD dwFrom,
            /* [length_is][size_is][in] */ __RPC__in_ecount_part(dwFrom, dwFrom) LPOLESTR rszFormatConvertsFrom[  ],
            /* [in] */ DWORD dwTo,
            /* [length_is][size_is][in] */ __RPC__in_ecount_part(dwTo, dwTo) LPOLESTR rszFormatConvertsTo[  ],
            /* [in] */ DWORD dwDefault,
            /* [length_is][size_is][in] */ __RPC__in_ecount_part(dwDefault, dwDefault) LPOLESTR rszFormatDefault[  ],
            /* [in] */ DWORD dwExt,
            /* [length_is][size_is][in] */ __RPC__in_ecount_part(dwExt, dwExt) LPOLESTR rszFileExt[  ],
            /* [in] */ DWORD dwType,
            /* [length_is][size_is][in] */ __RPC__in_ecount_part(dwType, dwType) LPOLESTR rszFileType[  ]);
        
        HRESULT ( STDMETHODCALLTYPE *SetRoleCheck )( 
            IDebugMetaDataEmit2 * This,
            /* [in] */ _mdToken tk,
            /* [in] */ DWORD dwNames,
            /* [length_is][size_is][in] */ __RPC__in_ecount_part(dwNames, dwNames) LPOLESTR rszName[  ],
            /* [in] */ DWORD dwFlags,
            /* [length_is][size_is][in] */ __RPC__in_ecount_part(dwFlags, dwFlags) DWORD rdwRoleFlags[  ]);
        
        HRESULT ( STDMETHODCALLTYPE *DefineMethod )( 
            IDebugMetaDataEmit2 * This,
            /* [in] */ _mdToken td,
            /* [in] */ __RPC__in LPOLESTR szName,
            /* [in] */ DWORD dwMethodFlags,
            /* [size_is][in] */ __RPC__in_ecount_full(cbSigBlob) BYTE *pvSigBlob,
            /* [in] */ ULONG cbSigBlob,
            /* [in] */ ULONG ulSlot,
            /* [in] */ ULONG ulCodeRVA,
            /* [in] */ DWORD dwImplFlags,
            /* [out] */ __RPC__out _mdToken *pmd);
        
        HRESULT ( STDMETHODCALLTYPE *DefineField )( 
            IDebugMetaDataEmit2 * This,
            /* [in] */ _mdToken td,
            /* [in] */ __RPC__in LPOLESTR szName,
            /* [in] */ DWORD dwFieldFlags,
            /* [size_is][in] */ __RPC__in_ecount_full(cbSigBlob) BYTE *pvSigBlob,
            /* [in] */ ULONG cbSigBlob,
            /* [in] */ DWORD dwCPlusTypeFlag,
            /* [size_is][in] */ __RPC__in_ecount_full(cbValue) BYTE *pValue,
            /* [in] */ ULONG cbValue,
            /* [out] */ __RPC__out _mdToken *pmd);
        
        HRESULT ( STDMETHODCALLTYPE *SetParamProps )( 
            IDebugMetaDataEmit2 * This,
            /* [in] */ _mdToken md,
            /* [in] */ ULONG ulParamSeq,
            /* [in] */ __RPC__in LPOLESTR szName,
            /* [in] */ DWORD dwParamFlags,
            /* [in] */ DWORD dwCPlusTypeFlag,
            /* [size_is][in] */ __RPC__in_ecount_full(cbValue) BYTE *pValue,
            /* [in] */ ULONG cbValue,
            /* [out] */ __RPC__out _mdToken *ppd);
        
        HRESULT ( STDMETHODCALLTYPE *DefineMethodImpl )( 
            IDebugMetaDataEmit2 * This,
            /* [in] */ _mdToken td,
            /* [in] */ _mdToken tk,
            /* [in] */ ULONG ulCodeRVA,
            /* [in] */ DWORD dwImplFlags,
            /* [out] */ __RPC__out _mdToken *pmi);
        
        HRESULT ( STDMETHODCALLTYPE *SetRVA )( 
            IDebugMetaDataEmit2 * This,
            /* [in] */ _mdToken md,
            /* [in] */ ULONG ulCodeRVA,
            /* [in] */ DWORD dwImplFlags);
        
        HRESULT ( STDMETHODCALLTYPE *DefineTypeRefByName )( 
            IDebugMetaDataEmit2 * This,
            /* [in] */ __RPC__in LPOLESTR szNamespace,
            /* [in] */ __RPC__in LPOLESTR szType,
            /* [out] */ __RPC__out _mdToken *ptr);
        
        HRESULT ( STDMETHODCALLTYPE *SetTypeRefBind )( 
            IDebugMetaDataEmit2 * This,
            /* [in] */ _mdToken tr,
            /* [in] */ DWORD dwBindFlags,
            /* [in] */ DWORD dwMinVersion,
            /* [in] */ DWORD dwMaxVersion,
            /* [in] */ __RPC__in LPOLESTR szCodebase);
        
        HRESULT ( STDMETHODCALLTYPE *DefineMemberRef )( 
            IDebugMetaDataEmit2 * This,
            /* [in] */ _mdToken tkImport,
            /* [in] */ __RPC__in LPOLESTR szName,
            /* [size_is][in] */ __RPC__in_ecount_full(cbSigBlob) BYTE *pvSigBlob,
            /* [in] */ ULONG cbSigBlob,
            /* [out] */ __RPC__out _mdToken *pmr);
        
        HRESULT ( STDMETHODCALLTYPE *DefineException )( 
            IDebugMetaDataEmit2 * This,
            /* [in] */ _mdToken mb,
            /* [in] */ _mdToken tk,
            /* [out] */ __RPC__out _mdToken *pex);
        
        HRESULT ( STDMETHODCALLTYPE *DefineProperty )( 
            IDebugMetaDataEmit2 * This,
            /* [in] */ _mdToken td,
            /* [in] */ __RPC__in LPOLESTR szProperty,
            /* [in] */ DWORD dwPropFlags,
            /* [size_is][in] */ __RPC__in_ecount_full(cbSig) BYTE *pvSig,
            /* [in] */ ULONG cbSig,
            /* [in] */ DWORD dwCPlusTypeFlag,
            /* [size_is][in] */ __RPC__in_ecount_full(cbValue) BYTE *pValue,
            /* [in] */ ULONG cbValue,
            /* [in] */ _mdToken mdSetter,
            /* [in] */ _mdToken mdGetter,
            /* [in] */ _mdToken mdReset,
            /* [in] */ _mdToken mdTestDefault,
            /* [in] */ DWORD dwOthers,
            /* [length_is][size_is][in] */ __RPC__in_ecount_part(dwOthers, dwOthers) _mdToken rmdOtherMethods[  ],
            /* [in] */ _mdToken evNotifyChanging,
            /* [in] */ _mdToken evNotifyChanged,
            /* [in] */ _mdToken fdBackingField,
            /* [out] */ __RPC__out _mdToken *pmdProp);
        
        HRESULT ( STDMETHODCALLTYPE *DefineEvent )( 
            IDebugMetaDataEmit2 * This,
            /* [in] */ _mdToken td,
            /* [in] */ __RPC__in LPOLESTR szEvent,
            /* [in] */ DWORD dwEventFlags,
            /* [in] */ _mdToken tkEventType,
            /* [in] */ _mdToken mdAddOn,
            /* [in] */ _mdToken mdRemoveOn,
            /* [in] */ _mdToken mdFire,
            /* [in] */ DWORD dwOthers,
            /* [length_is][size_is][in] */ __RPC__in_ecount_part(dwOthers, dwOthers) _mdToken rmdOtherMethods[  ],
            /* [out] */ __RPC__out _mdToken *pmdEvent);
        
        HRESULT ( STDMETHODCALLTYPE *SetFieldMarshal )( 
            IDebugMetaDataEmit2 * This,
            /* [in] */ _mdToken tk,
            /* [size_is][in] */ __RPC__in_ecount_full(cbNativeType) BYTE *pvNativeType,
            /* [in] */ ULONG cbNativeType);
        
        HRESULT ( STDMETHODCALLTYPE *DefinePermissionSet )( 
            IDebugMetaDataEmit2 * This,
            /* [in] */ _mdToken tk,
            /* [in] */ DWORD dwAction,
            /* [size_is][in] */ __RPC__in_ecount_full(cbPermission) BYTE *pvPermission,
            /* [in] */ ULONG cbPermission,
            /* [out] */ __RPC__out _mdToken *ppm);
        
        HRESULT ( STDMETHODCALLTYPE *SetMemberIndex )( 
            IDebugMetaDataEmit2 * This,
            /* [in] */ _mdToken md,
            /* [in] */ ULONG ulIndex);
        
        HRESULT ( STDMETHODCALLTYPE *GetTokenFromSig )( 
            IDebugMetaDataEmit2 * This,
            /* [size_is][in] */ __RPC__in_ecount_full(cbSig) BYTE *pvSig,
            /* [in] */ ULONG cbSig,
            /* [in] */ __RPC__in _mdToken *pmsig);
        
        HRESULT ( STDMETHODCALLTYPE *DefineModuleRef )( 
            IDebugMetaDataEmit2 * This,
            /* [in] */ __RPC__in LPOLESTR szName,
            /* [in] */ __RPC__in GUID *pguid,
            /* [in] */ __RPC__in GUID *pmvid,
            /* [out] */ __RPC__out _mdToken *pmur);
        
        HRESULT ( STDMETHODCALLTYPE *SetParent )( 
            IDebugMetaDataEmit2 * This,
            /* [in] */ _mdToken mr,
            /* [in] */ _mdToken tk);
        
        HRESULT ( STDMETHODCALLTYPE *GetTokenFromArraySpec )( 
            IDebugMetaDataEmit2 * This,
            /* [size_is][in] */ __RPC__in_ecount_full(cbSig) BYTE *pvSig,
            /* [in] */ ULONG cbSig,
            /* [out] */ __RPC__out _mdToken *parrspec);
        
        END_INTERFACE
    } IDebugMetaDataEmit2Vtbl;

    interface IDebugMetaDataEmit2
    {
        CONST_VTBL struct IDebugMetaDataEmit2Vtbl *lpVtbl;
    };

    

#ifdef COBJMACROS


#define IDebugMetaDataEmit2_QueryInterface(This,riid,ppvObject)	\
    ( (This)->lpVtbl -> QueryInterface(This,riid,ppvObject) ) 

#define IDebugMetaDataEmit2_AddRef(This)	\
    ( (This)->lpVtbl -> AddRef(This) ) 

#define IDebugMetaDataEmit2_Release(This)	\
    ( (This)->lpVtbl -> Release(This) ) 


#define IDebugMetaDataEmit2_SetModuleProps(This,szName,ppid,lcid)	\
    ( (This)->lpVtbl -> SetModuleProps(This,szName,ppid,lcid) ) 

#define IDebugMetaDataEmit2_Save(This,szFile,dwSaveFlags)	\
    ( (This)->lpVtbl -> Save(This,szFile,dwSaveFlags) ) 

#define IDebugMetaDataEmit2_SaveToStream(This,pIStream,dwSaveFlags)	\
    ( (This)->lpVtbl -> SaveToStream(This,pIStream,dwSaveFlags) ) 

#define IDebugMetaDataEmit2_GetSaveSize(This,fSave,pdwSaveSize)	\
    ( (This)->lpVtbl -> GetSaveSize(This,fSave,pdwSaveSize) ) 

#define IDebugMetaDataEmit2_DefineCustomValueAsBlob(This,tkObj,szName,pCustomValue,cbCustomValue,pcv)	\
    ( (This)->lpVtbl -> DefineCustomValueAsBlob(This,tkObj,szName,pCustomValue,cbCustomValue,pcv) ) 

#define IDebugMetaDataEmit2_DefineTypeDef(This,szNamespace,szTypeDef,pguid,pVer,dwTypeDefFlags,tkExtends,dwExtendsFlags,dwImplements,rtkImplements,dwEvents,rtkEvents,ptd)	\
    ( (This)->lpVtbl -> DefineTypeDef(This,szNamespace,szTypeDef,pguid,pVer,dwTypeDefFlags,tkExtends,dwExtendsFlags,dwImplements,rtkImplements,dwEvents,rtkEvents,ptd) ) 

#define IDebugMetaDataEmit2_SetTypeDefProps(This,td,pVer,dwTypeDefFlags,tkExtends,dwExtendsFlags,dwImplements,rtkImplements,dwEvents,rtkEvents)	\
    ( (This)->lpVtbl -> SetTypeDefProps(This,td,pVer,dwTypeDefFlags,tkExtends,dwExtendsFlags,dwImplements,rtkImplements,dwEvents,rtkEvents) ) 

#define IDebugMetaDataEmit2_SetClassSvcsContext(This,td,dwClassActivateAttr,dwClassThreadAttr,dwXactionAttr,dwSynchAttr)	\
    ( (This)->lpVtbl -> SetClassSvcsContext(This,td,dwClassActivateAttr,dwClassThreadAttr,dwXactionAttr,dwSynchAttr) ) 

#define IDebugMetaDataEmit2_DefineTypeRefByGUID(This,pguid,ptr)	\
    ( (This)->lpVtbl -> DefineTypeRefByGUID(This,pguid,ptr) ) 

#define IDebugMetaDataEmit2_SetModuleReg(This,dwModuleRegAttr,pguid)	\
    ( (This)->lpVtbl -> SetModuleReg(This,dwModuleRegAttr,pguid) ) 

#define IDebugMetaDataEmit2_SetClassReg(This,td,szProgID,szVIProgID,szIconURL,ulIconResource,szSmallIconURL,ulSmallIconResource,szDefaultDispName)	\
    ( (This)->lpVtbl -> SetClassReg(This,td,szProgID,szVIProgID,szIconURL,ulIconResource,szSmallIconURL,ulSmallIconResource,szDefaultDispName) ) 

#define IDebugMetaDataEmit2_SetIfaceReg(This,td,dwIfaceSvcs,proxyStub)	\
    ( (This)->lpVtbl -> SetIfaceReg(This,td,dwIfaceSvcs,proxyStub) ) 

#define IDebugMetaDataEmit2_SetCategoryImpl(This,td,dwImpl,rGuidCoCatImpl,dwReqd,rGuidCoCatReqd)	\
    ( (This)->lpVtbl -> SetCategoryImpl(This,td,dwImpl,rGuidCoCatImpl,dwReqd,rGuidCoCatReqd) ) 

#define IDebugMetaDataEmit2_SetRedirectProgID(This,td,dwProgIds,rszRedirectProgID)	\
    ( (This)->lpVtbl -> SetRedirectProgID(This,td,dwProgIds,rszRedirectProgID) ) 

#define IDebugMetaDataEmit2_SetMimeTypeImpl(This,td,dwTypes,rszMimeType)	\
    ( (This)->lpVtbl -> SetMimeTypeImpl(This,td,dwTypes,rszMimeType) ) 

#define IDebugMetaDataEmit2_SetFormatImpl(This,td,dwSupported,rszFormatSupported,dwFrom,rszFormatConvertsFrom,dwTo,rszFormatConvertsTo,dwDefault,rszFormatDefault,dwExt,rszFileExt,dwType,rszFileType)	\
    ( (This)->lpVtbl -> SetFormatImpl(This,td,dwSupported,rszFormatSupported,dwFrom,rszFormatConvertsFrom,dwTo,rszFormatConvertsTo,dwDefault,rszFormatDefault,dwExt,rszFileExt,dwType,rszFileType) ) 

#define IDebugMetaDataEmit2_SetRoleCheck(This,tk,dwNames,rszName,dwFlags,rdwRoleFlags)	\
    ( (This)->lpVtbl -> SetRoleCheck(This,tk,dwNames,rszName,dwFlags,rdwRoleFlags) ) 

#define IDebugMetaDataEmit2_DefineMethod(This,td,szName,dwMethodFlags,pvSigBlob,cbSigBlob,ulSlot,ulCodeRVA,dwImplFlags,pmd)	\
    ( (This)->lpVtbl -> DefineMethod(This,td,szName,dwMethodFlags,pvSigBlob,cbSigBlob,ulSlot,ulCodeRVA,dwImplFlags,pmd) ) 

#define IDebugMetaDataEmit2_DefineField(This,td,szName,dwFieldFlags,pvSigBlob,cbSigBlob,dwCPlusTypeFlag,pValue,cbValue,pmd)	\
    ( (This)->lpVtbl -> DefineField(This,td,szName,dwFieldFlags,pvSigBlob,cbSigBlob,dwCPlusTypeFlag,pValue,cbValue,pmd) ) 

#define IDebugMetaDataEmit2_SetParamProps(This,md,ulParamSeq,szName,dwParamFlags,dwCPlusTypeFlag,pValue,cbValue,ppd)	\
    ( (This)->lpVtbl -> SetParamProps(This,md,ulParamSeq,szName,dwParamFlags,dwCPlusTypeFlag,pValue,cbValue,ppd) ) 

#define IDebugMetaDataEmit2_DefineMethodImpl(This,td,tk,ulCodeRVA,dwImplFlags,pmi)	\
    ( (This)->lpVtbl -> DefineMethodImpl(This,td,tk,ulCodeRVA,dwImplFlags,pmi) ) 

#define IDebugMetaDataEmit2_SetRVA(This,md,ulCodeRVA,dwImplFlags)	\
    ( (This)->lpVtbl -> SetRVA(This,md,ulCodeRVA,dwImplFlags) ) 

#define IDebugMetaDataEmit2_DefineTypeRefByName(This,szNamespace,szType,ptr)	\
    ( (This)->lpVtbl -> DefineTypeRefByName(This,szNamespace,szType,ptr) ) 

#define IDebugMetaDataEmit2_SetTypeRefBind(This,tr,dwBindFlags,dwMinVersion,dwMaxVersion,szCodebase)	\
    ( (This)->lpVtbl -> SetTypeRefBind(This,tr,dwBindFlags,dwMinVersion,dwMaxVersion,szCodebase) ) 

#define IDebugMetaDataEmit2_DefineMemberRef(This,tkImport,szName,pvSigBlob,cbSigBlob,pmr)	\
    ( (This)->lpVtbl -> DefineMemberRef(This,tkImport,szName,pvSigBlob,cbSigBlob,pmr) ) 

#define IDebugMetaDataEmit2_DefineException(This,mb,tk,pex)	\
    ( (This)->lpVtbl -> DefineException(This,mb,tk,pex) ) 

#define IDebugMetaDataEmit2_DefineProperty(This,td,szProperty,dwPropFlags,pvSig,cbSig,dwCPlusTypeFlag,pValue,cbValue,mdSetter,mdGetter,mdReset,mdTestDefault,dwOthers,rmdOtherMethods,evNotifyChanging,evNotifyChanged,fdBackingField,pmdProp)	\
    ( (This)->lpVtbl -> DefineProperty(This,td,szProperty,dwPropFlags,pvSig,cbSig,dwCPlusTypeFlag,pValue,cbValue,mdSetter,mdGetter,mdReset,mdTestDefault,dwOthers,rmdOtherMethods,evNotifyChanging,evNotifyChanged,fdBackingField,pmdProp) ) 

#define IDebugMetaDataEmit2_DefineEvent(This,td,szEvent,dwEventFlags,tkEventType,mdAddOn,mdRemoveOn,mdFire,dwOthers,rmdOtherMethods,pmdEvent)	\
    ( (This)->lpVtbl -> DefineEvent(This,td,szEvent,dwEventFlags,tkEventType,mdAddOn,mdRemoveOn,mdFire,dwOthers,rmdOtherMethods,pmdEvent) ) 

#define IDebugMetaDataEmit2_SetFieldMarshal(This,tk,pvNativeType,cbNativeType)	\
    ( (This)->lpVtbl -> SetFieldMarshal(This,tk,pvNativeType,cbNativeType) ) 

#define IDebugMetaDataEmit2_DefinePermissionSet(This,tk,dwAction,pvPermission,cbPermission,ppm)	\
    ( (This)->lpVtbl -> DefinePermissionSet(This,tk,dwAction,pvPermission,cbPermission,ppm) ) 

#define IDebugMetaDataEmit2_SetMemberIndex(This,md,ulIndex)	\
    ( (This)->lpVtbl -> SetMemberIndex(This,md,ulIndex) ) 

#define IDebugMetaDataEmit2_GetTokenFromSig(This,pvSig,cbSig,pmsig)	\
    ( (This)->lpVtbl -> GetTokenFromSig(This,pvSig,cbSig,pmsig) ) 

#define IDebugMetaDataEmit2_DefineModuleRef(This,szName,pguid,pmvid,pmur)	\
    ( (This)->lpVtbl -> DefineModuleRef(This,szName,pguid,pmvid,pmur) ) 

#define IDebugMetaDataEmit2_SetParent(This,mr,tk)	\
    ( (This)->lpVtbl -> SetParent(This,mr,tk) ) 

#define IDebugMetaDataEmit2_GetTokenFromArraySpec(This,pvSig,cbSig,parrspec)	\
    ( (This)->lpVtbl -> GetTokenFromArraySpec(This,pvSig,cbSig,parrspec) ) 

#endif /* COBJMACROS */


#endif 	/* C style interface */




#endif 	/* __IDebugMetaDataEmit2_INTERFACE_DEFINED__ */


/* interface __MIDL_itf_enc_0000_0028 */
/* [local] */ 

#pragma warning(pop)
#pragma warning(push)
#pragma warning(disable:28718)	


extern RPC_IF_HANDLE __MIDL_itf_enc_0000_0028_v0_0_c_ifspec;
extern RPC_IF_HANDLE __MIDL_itf_enc_0000_0028_v0_0_s_ifspec;

#ifndef __IDebugMetaDataDebugEmit2_INTERFACE_DEFINED__
#define __IDebugMetaDataDebugEmit2_INTERFACE_DEFINED__

/* interface IDebugMetaDataDebugEmit2 */
/* [unique][uuid][object] */ 


EXTERN_C const IID IID_IDebugMetaDataDebugEmit2;

#if defined(__cplusplus) && !defined(CINTERFACE)
    
    MIDL_INTERFACE("f6f94d22-78c2-11d2-8ffe-00c04fa38314")
    IDebugMetaDataDebugEmit2 : public IUnknown
    {
    public:
        virtual HRESULT STDMETHODCALLTYPE DefineSourceFile( 
            /* [in] */ __RPC__in LPOLESTR szFileName,
            /* [out] */ __RPC__out _mdToken *psourcefile) = 0;
        
        virtual HRESULT STDMETHODCALLTYPE DefineBlock( 
            /* [in] */ _mdToken member,
            /* [in] */ _mdToken sourcefile,
            /* [in] */ __RPC__in BYTE *pAttr,
            /* [in] */ ULONG cbAttr) = 0;
        
        virtual HRESULT STDMETHODCALLTYPE DefineLocalVarScope( 
            /* [in] */ _mdToken scopeParent,
            /* [in] */ ULONG ulStartLine,
            /* [in] */ ULONG ulEndLine,
            /* [in] */ _mdToken member,
            /* [out] */ __RPC__out _mdToken *plocalvarscope) = 0;
        
    };
    
#else 	/* C style interface */

    typedef struct IDebugMetaDataDebugEmit2Vtbl
    {
        BEGIN_INTERFACE
        
        HRESULT ( STDMETHODCALLTYPE *QueryInterface )( 
            IDebugMetaDataDebugEmit2 * This,
            /* [in] */ __RPC__in REFIID riid,
            /* [iid_is][out] */ 
            __RPC__deref_out  void **ppvObject);
        
        ULONG ( STDMETHODCALLTYPE *AddRef )( 
            IDebugMetaDataDebugEmit2 * This);
        
        ULONG ( STDMETHODCALLTYPE *Release )( 
            IDebugMetaDataDebugEmit2 * This);
        
        HRESULT ( STDMETHODCALLTYPE *DefineSourceFile )( 
            IDebugMetaDataDebugEmit2 * This,
            /* [in] */ __RPC__in LPOLESTR szFileName,
            /* [out] */ __RPC__out _mdToken *psourcefile);
        
        HRESULT ( STDMETHODCALLTYPE *DefineBlock )( 
            IDebugMetaDataDebugEmit2 * This,
            /* [in] */ _mdToken member,
            /* [in] */ _mdToken sourcefile,
            /* [in] */ __RPC__in BYTE *pAttr,
            /* [in] */ ULONG cbAttr);
        
        HRESULT ( STDMETHODCALLTYPE *DefineLocalVarScope )( 
            IDebugMetaDataDebugEmit2 * This,
            /* [in] */ _mdToken scopeParent,
            /* [in] */ ULONG ulStartLine,
            /* [in] */ ULONG ulEndLine,
            /* [in] */ _mdToken member,
            /* [out] */ __RPC__out _mdToken *plocalvarscope);
        
        END_INTERFACE
    } IDebugMetaDataDebugEmit2Vtbl;

    interface IDebugMetaDataDebugEmit2
    {
        CONST_VTBL struct IDebugMetaDataDebugEmit2Vtbl *lpVtbl;
    };

    

#ifdef COBJMACROS


#define IDebugMetaDataDebugEmit2_QueryInterface(This,riid,ppvObject)	\
    ( (This)->lpVtbl -> QueryInterface(This,riid,ppvObject) ) 

#define IDebugMetaDataDebugEmit2_AddRef(This)	\
    ( (This)->lpVtbl -> AddRef(This) ) 

#define IDebugMetaDataDebugEmit2_Release(This)	\
    ( (This)->lpVtbl -> Release(This) ) 


#define IDebugMetaDataDebugEmit2_DefineSourceFile(This,szFileName,psourcefile)	\
    ( (This)->lpVtbl -> DefineSourceFile(This,szFileName,psourcefile) ) 

#define IDebugMetaDataDebugEmit2_DefineBlock(This,member,sourcefile,pAttr,cbAttr)	\
    ( (This)->lpVtbl -> DefineBlock(This,member,sourcefile,pAttr,cbAttr) ) 

#define IDebugMetaDataDebugEmit2_DefineLocalVarScope(This,scopeParent,ulStartLine,ulEndLine,member,plocalvarscope)	\
    ( (This)->lpVtbl -> DefineLocalVarScope(This,scopeParent,ulStartLine,ulEndLine,member,plocalvarscope) ) 

#endif /* COBJMACROS */


#endif 	/* C style interface */




#endif 	/* __IDebugMetaDataDebugEmit2_INTERFACE_DEFINED__ */


/* interface __MIDL_itf_enc_0000_0029 */
/* [local] */ 

#pragma warning(pop)


extern RPC_IF_HANDLE __MIDL_itf_enc_0000_0029_v0_0_c_ifspec;
extern RPC_IF_HANDLE __MIDL_itf_enc_0000_0029_v0_0_s_ifspec;

#ifndef __IDebugENCStateEvents_INTERFACE_DEFINED__
#define __IDebugENCStateEvents_INTERFACE_DEFINED__

/* interface IDebugENCStateEvents */
/* [unique][uuid][object] */ 


EXTERN_C const IID IID_IDebugENCStateEvents;

#if defined(__cplusplus) && !defined(CINTERFACE)
    
    MIDL_INTERFACE("ec80d064-102e-435f-aafb-d37e2a4ef654")
    IDebugENCStateEvents : public IUnknown
    {
    public:
        virtual HRESULT STDMETHODCALLTYPE OnStateChange( 
            /* [in] */ __RPC__in ENCSTATE *in_pENCSTATE,
            /* [in] */ BOOL in_fReserved) = 0;
        
    };
    
#else 	/* C style interface */

    typedef struct IDebugENCStateEventsVtbl
    {
        BEGIN_INTERFACE
        
        HRESULT ( STDMETHODCALLTYPE *QueryInterface )( 
            IDebugENCStateEvents * This,
            /* [in] */ __RPC__in REFIID riid,
            /* [iid_is][out] */ 
            __RPC__deref_out  void **ppvObject);
        
        ULONG ( STDMETHODCALLTYPE *AddRef )( 
            IDebugENCStateEvents * This);
        
        ULONG ( STDMETHODCALLTYPE *Release )( 
            IDebugENCStateEvents * This);
        
        HRESULT ( STDMETHODCALLTYPE *OnStateChange )( 
            IDebugENCStateEvents * This,
            /* [in] */ __RPC__in ENCSTATE *in_pENCSTATE,
            /* [in] */ BOOL in_fReserved);
        
        END_INTERFACE
    } IDebugENCStateEventsVtbl;

    interface IDebugENCStateEvents
    {
        CONST_VTBL struct IDebugENCStateEventsVtbl *lpVtbl;
    };

    

#ifdef COBJMACROS


#define IDebugENCStateEvents_QueryInterface(This,riid,ppvObject)	\
    ( (This)->lpVtbl -> QueryInterface(This,riid,ppvObject) ) 

#define IDebugENCStateEvents_AddRef(This)	\
    ( (This)->lpVtbl -> AddRef(This) ) 

#define IDebugENCStateEvents_Release(This)	\
    ( (This)->lpVtbl -> Release(This) ) 


#define IDebugENCStateEvents_OnStateChange(This,in_pENCSTATE,in_fReserved)	\
    ( (This)->lpVtbl -> OnStateChange(This,in_pENCSTATE,in_fReserved) ) 

#endif /* COBJMACROS */


#endif 	/* C style interface */




#endif 	/* __IDebugENCStateEvents_INTERFACE_DEFINED__ */



#ifndef __EncLib_LIBRARY_DEFINED__
#define __EncLib_LIBRARY_DEFINED__

/* library EncLib */
/* [uuid] */ 


EXTERN_C const IID LIBID_EncLib;

EXTERN_C const CLSID CLSID_EncMgr;

#ifdef __cplusplus

class DECLSPEC_UUID("99A426F1-AF1D-11d2-922C-00A02448799A")
EncMgr;
#endif
#endif /* __EncLib_LIBRARY_DEFINED__ */

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


