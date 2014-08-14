

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

#ifndef __encbuild_h__
#define __encbuild_h__

#if defined(_MSC_VER) && (_MSC_VER >= 1020)
#pragma once
#endif

/* Forward Declarations */ 

#ifndef __IVsENCRebuildableProjectCfg_FWD_DEFINED__
#define __IVsENCRebuildableProjectCfg_FWD_DEFINED__
typedef interface IVsENCRebuildableProjectCfg IVsENCRebuildableProjectCfg;

#endif 	/* __IVsENCRebuildableProjectCfg_FWD_DEFINED__ */


#ifndef __IVsENCRebuildableProjectCfg2_FWD_DEFINED__
#define __IVsENCRebuildableProjectCfg2_FWD_DEFINED__
typedef interface IVsENCRebuildableProjectCfg2 IVsENCRebuildableProjectCfg2;

#endif 	/* __IVsENCRebuildableProjectCfg2_FWD_DEFINED__ */


#ifndef __IEnumVsENCRebuildableProjectCfgs_FWD_DEFINED__
#define __IEnumVsENCRebuildableProjectCfgs_FWD_DEFINED__
typedef interface IEnumVsENCRebuildableProjectCfgs IEnumVsENCRebuildableProjectCfgs;

#endif 	/* __IEnumVsENCRebuildableProjectCfgs_FWD_DEFINED__ */


/* header files for imported files */
#include "oleidl.h"
#include "vsshell.h"

#ifdef __cplusplus
extern "C"{
#endif 


/* interface __MIDL_itf_encbuild_0000_0000 */
/* [local] */ 


enum tagENC_REASON
    {
        ENCReason_Precompile	= 0,
        ENCReason_Rebuild	= ( ENCReason_Precompile + 1 ) 
    } ;
typedef enum tagENC_REASON ENC_REASON;

#define S_ENC_NOT_SUPPORTED                 MAKE_HRESULT(0, FACILITY_ITF, 0x0001)
#define E_STATEMENT_DELETED                 MAKE_HRESULT(1, FACILITY_ITF, 0x0001)


extern RPC_IF_HANDLE __MIDL_itf_encbuild_0000_0000_v0_0_c_ifspec;
extern RPC_IF_HANDLE __MIDL_itf_encbuild_0000_0000_v0_0_s_ifspec;

#ifndef __IVsENCRebuildableProjectCfg_INTERFACE_DEFINED__
#define __IVsENCRebuildableProjectCfg_INTERFACE_DEFINED__

/* interface IVsENCRebuildableProjectCfg */
/* [object][uuid] */ 


EXTERN_C const IID IID_IVsENCRebuildableProjectCfg;

#if defined(__cplusplus) && !defined(CINTERFACE)
    
    MIDL_INTERFACE("BF031840-AFB9-11d2-AE14-00A0C9110055")
    IVsENCRebuildableProjectCfg : public IUnknown
    {
    public:
        virtual HRESULT STDMETHODCALLTYPE ENCRebuild( 
            /* [in] */ __RPC__in_opt IUnknown *in_pProgram,
            /* [out] */ __RPC__deref_out_opt IUnknown **out_ppSnapshot) = 0;
        
        virtual HRESULT STDMETHODCALLTYPE CancelENC( void) = 0;
        
        virtual HRESULT STDMETHODCALLTYPE ENCRelink( 
            /* [in] */ __RPC__in_opt IUnknown *pENCRelinkInfo) = 0;
        
        virtual HRESULT STDMETHODCALLTYPE StartDebugging( void) = 0;
        
        virtual HRESULT STDMETHODCALLTYPE StopDebugging( void) = 0;
        
        virtual HRESULT STDMETHODCALLTYPE BelongToProject( 
            /* [in] */ __RPC__in LPCOLESTR in_szFileName,
            /* [in] */ ENC_REASON in_ENCReason,
            /* [in] */ BOOL in_fOnContinue) = 0;
        
        virtual HRESULT STDMETHODCALLTYPE SetENCProjectBuildOption( 
            /* [in] */ __RPC__in REFGUID in_guidOption,
            /* [in] */ __RPC__in LPCOLESTR in_szOptionValue) = 0;
        
        virtual HRESULT STDMETHODCALLTYPE ENCComplete( 
            /* [in] */ BOOL in_fENCSuccess) = 0;
        
    };
    
    
#else 	/* C style interface */

    typedef struct IVsENCRebuildableProjectCfgVtbl
    {
        BEGIN_INTERFACE
        
        HRESULT ( STDMETHODCALLTYPE *QueryInterface )( 
            __RPC__in IVsENCRebuildableProjectCfg * This,
            /* [in] */ __RPC__in REFIID riid,
            /* [annotation][iid_is][out] */ 
            _COM_Outptr_  void **ppvObject);
        
        ULONG ( STDMETHODCALLTYPE *AddRef )( 
            __RPC__in IVsENCRebuildableProjectCfg * This);
        
        ULONG ( STDMETHODCALLTYPE *Release )( 
            __RPC__in IVsENCRebuildableProjectCfg * This);
        
        HRESULT ( STDMETHODCALLTYPE *ENCRebuild )( 
            __RPC__in IVsENCRebuildableProjectCfg * This,
            /* [in] */ __RPC__in_opt IUnknown *in_pProgram,
            /* [out] */ __RPC__deref_out_opt IUnknown **out_ppSnapshot);
        
        HRESULT ( STDMETHODCALLTYPE *CancelENC )( 
            __RPC__in IVsENCRebuildableProjectCfg * This);
        
        HRESULT ( STDMETHODCALLTYPE *ENCRelink )( 
            __RPC__in IVsENCRebuildableProjectCfg * This,
            /* [in] */ __RPC__in_opt IUnknown *pENCRelinkInfo);
        
        HRESULT ( STDMETHODCALLTYPE *StartDebugging )( 
            __RPC__in IVsENCRebuildableProjectCfg * This);
        
        HRESULT ( STDMETHODCALLTYPE *StopDebugging )( 
            __RPC__in IVsENCRebuildableProjectCfg * This);
        
        HRESULT ( STDMETHODCALLTYPE *BelongToProject )( 
            __RPC__in IVsENCRebuildableProjectCfg * This,
            /* [in] */ __RPC__in LPCOLESTR in_szFileName,
            /* [in] */ ENC_REASON in_ENCReason,
            /* [in] */ BOOL in_fOnContinue);
        
        HRESULT ( STDMETHODCALLTYPE *SetENCProjectBuildOption )( 
            __RPC__in IVsENCRebuildableProjectCfg * This,
            /* [in] */ __RPC__in REFGUID in_guidOption,
            /* [in] */ __RPC__in LPCOLESTR in_szOptionValue);
        
        HRESULT ( STDMETHODCALLTYPE *ENCComplete )( 
            __RPC__in IVsENCRebuildableProjectCfg * This,
            /* [in] */ BOOL in_fENCSuccess);
        
        END_INTERFACE
    } IVsENCRebuildableProjectCfgVtbl;

    interface IVsENCRebuildableProjectCfg
    {
        CONST_VTBL struct IVsENCRebuildableProjectCfgVtbl *lpVtbl;
    };

    

#ifdef COBJMACROS


#define IVsENCRebuildableProjectCfg_QueryInterface(This,riid,ppvObject)	\
    ( (This)->lpVtbl -> QueryInterface(This,riid,ppvObject) ) 

#define IVsENCRebuildableProjectCfg_AddRef(This)	\
    ( (This)->lpVtbl -> AddRef(This) ) 

#define IVsENCRebuildableProjectCfg_Release(This)	\
    ( (This)->lpVtbl -> Release(This) ) 


#define IVsENCRebuildableProjectCfg_ENCRebuild(This,in_pProgram,out_ppSnapshot)	\
    ( (This)->lpVtbl -> ENCRebuild(This,in_pProgram,out_ppSnapshot) ) 

#define IVsENCRebuildableProjectCfg_CancelENC(This)	\
    ( (This)->lpVtbl -> CancelENC(This) ) 

#define IVsENCRebuildableProjectCfg_ENCRelink(This,pENCRelinkInfo)	\
    ( (This)->lpVtbl -> ENCRelink(This,pENCRelinkInfo) ) 

#define IVsENCRebuildableProjectCfg_StartDebugging(This)	\
    ( (This)->lpVtbl -> StartDebugging(This) ) 

#define IVsENCRebuildableProjectCfg_StopDebugging(This)	\
    ( (This)->lpVtbl -> StopDebugging(This) ) 

#define IVsENCRebuildableProjectCfg_BelongToProject(This,in_szFileName,in_ENCReason,in_fOnContinue)	\
    ( (This)->lpVtbl -> BelongToProject(This,in_szFileName,in_ENCReason,in_fOnContinue) ) 

#define IVsENCRebuildableProjectCfg_SetENCProjectBuildOption(This,in_guidOption,in_szOptionValue)	\
    ( (This)->lpVtbl -> SetENCProjectBuildOption(This,in_guidOption,in_szOptionValue) ) 

#define IVsENCRebuildableProjectCfg_ENCComplete(This,in_fENCSuccess)	\
    ( (This)->lpVtbl -> ENCComplete(This,in_fENCSuccess) ) 

#endif /* COBJMACROS */


#endif 	/* C style interface */




#endif 	/* __IVsENCRebuildableProjectCfg_INTERFACE_DEFINED__ */


#ifndef __IVsENCRebuildableProjectCfg2_INTERFACE_DEFINED__
#define __IVsENCRebuildableProjectCfg2_INTERFACE_DEFINED__

/* interface IVsENCRebuildableProjectCfg2 */
/* [unique][uuid][object] */ 


enum enum_ENC_BREAKSTATE_REASON
    {
        ENC_BREAK_NORMAL	= 0,
        ENC_BREAK_EXCEPTION	= ( ENC_BREAK_NORMAL + 1 ) 
    } ;
typedef DWORD ENC_BREAKSTATE_REASON;


enum enum_ASINFO
    {
        ASINFO_NONE	= 0,
        ASINFO_LEAF	= 0x1,
        ASINFO_MIDSTATEMENT	= 0x2,
        ASINFO_NONUSER	= 0x4
    } ;
typedef DWORD ASINFO;


enum enum_POSITION_TYPE
    {
        TEXT_POSITION_ACTIVE_STATEMENT	= 1,
        TEXT_POSITION_NEARBY_STATEMENT	= 2,
        TEXT_POSITION_NONE	= 3
    } ;
typedef DWORD POSITION_TYPE;

typedef struct _ACTVSTMT
    {
    UINT32 id;
    UINT32 methodToken;
    TextSpan tsPosition;
    BSTR filename;
    ASINFO asInfo;
    POSITION_TYPE posType;
    } 	ENC_ACTIVE_STATEMENT;

typedef /* [public][public] */ 
enum __MIDL_IVsENCRebuildableProjectCfg2_0001
    {
        ENC_NOT_MODIFIED	= 0,
        ENC_NONCONTINUABLE_ERRORS	= ( ENC_NOT_MODIFIED + 1 ) ,
        ENC_COMPILE_ERRORS	= ( ENC_NONCONTINUABLE_ERRORS + 1 ) ,
        ENC_APPLY_READY	= ( ENC_COMPILE_ERRORS + 1 ) 
    } 	ENC_BUILD_STATE;

typedef struct _EXCEPTIONSPAN
    {
    UINT32 id;
    UINT32 methodToken;
    TextSpan tsPosition;
    } 	ENC_EXCEPTION_SPAN;


EXTERN_C const IID IID_IVsENCRebuildableProjectCfg2;

#if defined(__cplusplus) && !defined(CINTERFACE)
    
    MIDL_INTERFACE("D13E943A-9EE0-457f-8766-7D8B6BC06565")
    IVsENCRebuildableProjectCfg2 : public IUnknown
    {
    public:
        virtual HRESULT STDMETHODCALLTYPE StartDebuggingPE( void) = 0;
        
        virtual HRESULT STDMETHODCALLTYPE EnterBreakStateOnPE( 
            /* [in] */ ENC_BREAKSTATE_REASON encBreakReason,
            /* [in] */ __RPC__in ENC_ACTIVE_STATEMENT *pActiveStatements,
            /* [in] */ UINT32 cActiveStatements) = 0;
        
        virtual HRESULT STDMETHODCALLTYPE BuildForEnc( 
            /* [in] */ __RPC__in_opt IUnknown *pUpdatePE) = 0;
        
        virtual HRESULT STDMETHODCALLTYPE ExitBreakStateOnPE( void) = 0;
        
        virtual HRESULT STDMETHODCALLTYPE StopDebuggingPE( void) = 0;
        
        virtual HRESULT STDMETHODCALLTYPE GetENCBuildState( 
            /* [out] */ __RPC__out ENC_BUILD_STATE *pENCBuildState) = 0;
        
        virtual HRESULT STDMETHODCALLTYPE GetCurrentActiveStatementPosition( 
            /* [in] */ UINT32 id,
            /* [out] */ __RPC__out TextSpan *ptsNewPosition) = 0;
        
        virtual HRESULT STDMETHODCALLTYPE GetPEidentity( 
            /* [out] */ __RPC__out GUID *pMVID,
            /* [out] */ __RPC__deref_out_opt BSTR *pbstrPEName) = 0;
        
        virtual HRESULT STDMETHODCALLTYPE GetExceptionSpanCount( 
            /* [out] */ __RPC__out UINT32 *pcExceptionSpan) = 0;
        
        virtual HRESULT STDMETHODCALLTYPE GetExceptionSpans( 
            /* [in] */ UINT32 celt,
            /* [length_is][size_is][out] */ __RPC__out_ecount_part(celt, *pceltFetched) ENC_EXCEPTION_SPAN *rgelt,
            /* [out][in] */ __RPC__inout ULONG *pceltFetched) = 0;
        
        virtual HRESULT STDMETHODCALLTYPE GetCurrentExceptionSpanPosition( 
            /* [in] */ UINT32 id,
            /* [out] */ __RPC__out TextSpan *ptsNewPosition) = 0;
        
        virtual HRESULT STDMETHODCALLTYPE EncApplySucceeded( 
            /* [in] */ HRESULT hrApplyResult) = 0;
        
        virtual HRESULT STDMETHODCALLTYPE GetPEBuildTimeStamp( 
            /* [out][in] */ __RPC__inout FILETIME *pTimeStamp) = 0;
        
    };
    
    
#else 	/* C style interface */

    typedef struct IVsENCRebuildableProjectCfg2Vtbl
    {
        BEGIN_INTERFACE
        
        HRESULT ( STDMETHODCALLTYPE *QueryInterface )( 
            __RPC__in IVsENCRebuildableProjectCfg2 * This,
            /* [in] */ __RPC__in REFIID riid,
            /* [annotation][iid_is][out] */ 
            _COM_Outptr_  void **ppvObject);
        
        ULONG ( STDMETHODCALLTYPE *AddRef )( 
            __RPC__in IVsENCRebuildableProjectCfg2 * This);
        
        ULONG ( STDMETHODCALLTYPE *Release )( 
            __RPC__in IVsENCRebuildableProjectCfg2 * This);
        
        HRESULT ( STDMETHODCALLTYPE *StartDebuggingPE )( 
            __RPC__in IVsENCRebuildableProjectCfg2 * This);
        
        HRESULT ( STDMETHODCALLTYPE *EnterBreakStateOnPE )( 
            __RPC__in IVsENCRebuildableProjectCfg2 * This,
            /* [in] */ ENC_BREAKSTATE_REASON encBreakReason,
            /* [in] */ __RPC__in ENC_ACTIVE_STATEMENT *pActiveStatements,
            /* [in] */ UINT32 cActiveStatements);
        
        HRESULT ( STDMETHODCALLTYPE *BuildForEnc )( 
            __RPC__in IVsENCRebuildableProjectCfg2 * This,
            /* [in] */ __RPC__in_opt IUnknown *pUpdatePE);
        
        HRESULT ( STDMETHODCALLTYPE *ExitBreakStateOnPE )( 
            __RPC__in IVsENCRebuildableProjectCfg2 * This);
        
        HRESULT ( STDMETHODCALLTYPE *StopDebuggingPE )( 
            __RPC__in IVsENCRebuildableProjectCfg2 * This);
        
        HRESULT ( STDMETHODCALLTYPE *GetENCBuildState )( 
            __RPC__in IVsENCRebuildableProjectCfg2 * This,
            /* [out] */ __RPC__out ENC_BUILD_STATE *pENCBuildState);
        
        HRESULT ( STDMETHODCALLTYPE *GetCurrentActiveStatementPosition )( 
            __RPC__in IVsENCRebuildableProjectCfg2 * This,
            /* [in] */ UINT32 id,
            /* [out] */ __RPC__out TextSpan *ptsNewPosition);
        
        HRESULT ( STDMETHODCALLTYPE *GetPEidentity )( 
            __RPC__in IVsENCRebuildableProjectCfg2 * This,
            /* [out] */ __RPC__out GUID *pMVID,
            /* [out] */ __RPC__deref_out_opt BSTR *pbstrPEName);
        
        HRESULT ( STDMETHODCALLTYPE *GetExceptionSpanCount )( 
            __RPC__in IVsENCRebuildableProjectCfg2 * This,
            /* [out] */ __RPC__out UINT32 *pcExceptionSpan);
        
        HRESULT ( STDMETHODCALLTYPE *GetExceptionSpans )( 
            __RPC__in IVsENCRebuildableProjectCfg2 * This,
            /* [in] */ UINT32 celt,
            /* [length_is][size_is][out] */ __RPC__out_ecount_part(celt, *pceltFetched) ENC_EXCEPTION_SPAN *rgelt,
            /* [out][in] */ __RPC__inout ULONG *pceltFetched);
        
        HRESULT ( STDMETHODCALLTYPE *GetCurrentExceptionSpanPosition )( 
            __RPC__in IVsENCRebuildableProjectCfg2 * This,
            /* [in] */ UINT32 id,
            /* [out] */ __RPC__out TextSpan *ptsNewPosition);
        
        HRESULT ( STDMETHODCALLTYPE *EncApplySucceeded )( 
            __RPC__in IVsENCRebuildableProjectCfg2 * This,
            /* [in] */ HRESULT hrApplyResult);
        
        HRESULT ( STDMETHODCALLTYPE *GetPEBuildTimeStamp )( 
            __RPC__in IVsENCRebuildableProjectCfg2 * This,
            /* [out][in] */ __RPC__inout FILETIME *pTimeStamp);
        
        END_INTERFACE
    } IVsENCRebuildableProjectCfg2Vtbl;

    interface IVsENCRebuildableProjectCfg2
    {
        CONST_VTBL struct IVsENCRebuildableProjectCfg2Vtbl *lpVtbl;
    };

    

#ifdef COBJMACROS


#define IVsENCRebuildableProjectCfg2_QueryInterface(This,riid,ppvObject)	\
    ( (This)->lpVtbl -> QueryInterface(This,riid,ppvObject) ) 

#define IVsENCRebuildableProjectCfg2_AddRef(This)	\
    ( (This)->lpVtbl -> AddRef(This) ) 

#define IVsENCRebuildableProjectCfg2_Release(This)	\
    ( (This)->lpVtbl -> Release(This) ) 


#define IVsENCRebuildableProjectCfg2_StartDebuggingPE(This)	\
    ( (This)->lpVtbl -> StartDebuggingPE(This) ) 

#define IVsENCRebuildableProjectCfg2_EnterBreakStateOnPE(This,encBreakReason,pActiveStatements,cActiveStatements)	\
    ( (This)->lpVtbl -> EnterBreakStateOnPE(This,encBreakReason,pActiveStatements,cActiveStatements) ) 

#define IVsENCRebuildableProjectCfg2_BuildForEnc(This,pUpdatePE)	\
    ( (This)->lpVtbl -> BuildForEnc(This,pUpdatePE) ) 

#define IVsENCRebuildableProjectCfg2_ExitBreakStateOnPE(This)	\
    ( (This)->lpVtbl -> ExitBreakStateOnPE(This) ) 

#define IVsENCRebuildableProjectCfg2_StopDebuggingPE(This)	\
    ( (This)->lpVtbl -> StopDebuggingPE(This) ) 

#define IVsENCRebuildableProjectCfg2_GetENCBuildState(This,pENCBuildState)	\
    ( (This)->lpVtbl -> GetENCBuildState(This,pENCBuildState) ) 

#define IVsENCRebuildableProjectCfg2_GetCurrentActiveStatementPosition(This,id,ptsNewPosition)	\
    ( (This)->lpVtbl -> GetCurrentActiveStatementPosition(This,id,ptsNewPosition) ) 

#define IVsENCRebuildableProjectCfg2_GetPEidentity(This,pMVID,pbstrPEName)	\
    ( (This)->lpVtbl -> GetPEidentity(This,pMVID,pbstrPEName) ) 

#define IVsENCRebuildableProjectCfg2_GetExceptionSpanCount(This,pcExceptionSpan)	\
    ( (This)->lpVtbl -> GetExceptionSpanCount(This,pcExceptionSpan) ) 

#define IVsENCRebuildableProjectCfg2_GetExceptionSpans(This,celt,rgelt,pceltFetched)	\
    ( (This)->lpVtbl -> GetExceptionSpans(This,celt,rgelt,pceltFetched) ) 

#define IVsENCRebuildableProjectCfg2_GetCurrentExceptionSpanPosition(This,id,ptsNewPosition)	\
    ( (This)->lpVtbl -> GetCurrentExceptionSpanPosition(This,id,ptsNewPosition) ) 

#define IVsENCRebuildableProjectCfg2_EncApplySucceeded(This,hrApplyResult)	\
    ( (This)->lpVtbl -> EncApplySucceeded(This,hrApplyResult) ) 

#define IVsENCRebuildableProjectCfg2_GetPEBuildTimeStamp(This,pTimeStamp)	\
    ( (This)->lpVtbl -> GetPEBuildTimeStamp(This,pTimeStamp) ) 

#endif /* COBJMACROS */


#endif 	/* C style interface */




#endif 	/* __IVsENCRebuildableProjectCfg2_INTERFACE_DEFINED__ */


#ifndef __IEnumVsENCRebuildableProjectCfgs_INTERFACE_DEFINED__
#define __IEnumVsENCRebuildableProjectCfgs_INTERFACE_DEFINED__

/* interface IEnumVsENCRebuildableProjectCfgs */
/* [unique][uuid][object] */ 


EXTERN_C const IID IID_IEnumVsENCRebuildableProjectCfgs;

#if defined(__cplusplus) && !defined(CINTERFACE)
    
    MIDL_INTERFACE("3909921B-BACD-11d2-BD66-00C04FA302E2")
    IEnumVsENCRebuildableProjectCfgs : public IUnknown
    {
    public:
        virtual HRESULT STDMETHODCALLTYPE Next( 
            /* [in] */ ULONG celt,
            /* [length_is][size_is][out] */ __RPC__out_ecount_part(celt, *pceltFetched) IVsENCRebuildableProjectCfg **rgelt,
            /* [out][in] */ __RPC__inout ULONG *pceltFetched) = 0;
        
        virtual HRESULT STDMETHODCALLTYPE Skip( 
            /* [in] */ ULONG celt) = 0;
        
        virtual HRESULT STDMETHODCALLTYPE Reset( void) = 0;
        
        virtual HRESULT STDMETHODCALLTYPE Clone( 
            /* [out] */ __RPC__deref_out_opt IEnumVsENCRebuildableProjectCfgs **ppEnum) = 0;
        
        virtual HRESULT STDMETHODCALLTYPE GetCount( 
            /* [out] */ __RPC__out ULONG *pcelt) = 0;
        
    };
    
    
#else 	/* C style interface */

    typedef struct IEnumVsENCRebuildableProjectCfgsVtbl
    {
        BEGIN_INTERFACE
        
        HRESULT ( STDMETHODCALLTYPE *QueryInterface )( 
            __RPC__in IEnumVsENCRebuildableProjectCfgs * This,
            /* [in] */ __RPC__in REFIID riid,
            /* [annotation][iid_is][out] */ 
            _COM_Outptr_  void **ppvObject);
        
        ULONG ( STDMETHODCALLTYPE *AddRef )( 
            __RPC__in IEnumVsENCRebuildableProjectCfgs * This);
        
        ULONG ( STDMETHODCALLTYPE *Release )( 
            __RPC__in IEnumVsENCRebuildableProjectCfgs * This);
        
        HRESULT ( STDMETHODCALLTYPE *Next )( 
            __RPC__in IEnumVsENCRebuildableProjectCfgs * This,
            /* [in] */ ULONG celt,
            /* [length_is][size_is][out] */ __RPC__out_ecount_part(celt, *pceltFetched) IVsENCRebuildableProjectCfg **rgelt,
            /* [out][in] */ __RPC__inout ULONG *pceltFetched);
        
        HRESULT ( STDMETHODCALLTYPE *Skip )( 
            __RPC__in IEnumVsENCRebuildableProjectCfgs * This,
            /* [in] */ ULONG celt);
        
        HRESULT ( STDMETHODCALLTYPE *Reset )( 
            __RPC__in IEnumVsENCRebuildableProjectCfgs * This);
        
        HRESULT ( STDMETHODCALLTYPE *Clone )( 
            __RPC__in IEnumVsENCRebuildableProjectCfgs * This,
            /* [out] */ __RPC__deref_out_opt IEnumVsENCRebuildableProjectCfgs **ppEnum);
        
        HRESULT ( STDMETHODCALLTYPE *GetCount )( 
            __RPC__in IEnumVsENCRebuildableProjectCfgs * This,
            /* [out] */ __RPC__out ULONG *pcelt);
        
        END_INTERFACE
    } IEnumVsENCRebuildableProjectCfgsVtbl;

    interface IEnumVsENCRebuildableProjectCfgs
    {
        CONST_VTBL struct IEnumVsENCRebuildableProjectCfgsVtbl *lpVtbl;
    };

    

#ifdef COBJMACROS


#define IEnumVsENCRebuildableProjectCfgs_QueryInterface(This,riid,ppvObject)	\
    ( (This)->lpVtbl -> QueryInterface(This,riid,ppvObject) ) 

#define IEnumVsENCRebuildableProjectCfgs_AddRef(This)	\
    ( (This)->lpVtbl -> AddRef(This) ) 

#define IEnumVsENCRebuildableProjectCfgs_Release(This)	\
    ( (This)->lpVtbl -> Release(This) ) 


#define IEnumVsENCRebuildableProjectCfgs_Next(This,celt,rgelt,pceltFetched)	\
    ( (This)->lpVtbl -> Next(This,celt,rgelt,pceltFetched) ) 

#define IEnumVsENCRebuildableProjectCfgs_Skip(This,celt)	\
    ( (This)->lpVtbl -> Skip(This,celt) ) 

#define IEnumVsENCRebuildableProjectCfgs_Reset(This)	\
    ( (This)->lpVtbl -> Reset(This) ) 

#define IEnumVsENCRebuildableProjectCfgs_Clone(This,ppEnum)	\
    ( (This)->lpVtbl -> Clone(This,ppEnum) ) 

#define IEnumVsENCRebuildableProjectCfgs_GetCount(This,pcelt)	\
    ( (This)->lpVtbl -> GetCount(This,pcelt) ) 

#endif /* COBJMACROS */


#endif 	/* C style interface */




#endif 	/* __IEnumVsENCRebuildableProjectCfgs_INTERFACE_DEFINED__ */


/* Additional Prototypes for ALL interfaces */

unsigned long             __RPC_USER  BSTR_UserSize(     __RPC__in unsigned long *, unsigned long            , __RPC__in BSTR * ); 
unsigned char * __RPC_USER  BSTR_UserMarshal(  __RPC__in unsigned long *, __RPC__inout_xcount(0) unsigned char *, __RPC__in BSTR * ); 
unsigned char * __RPC_USER  BSTR_UserUnmarshal(__RPC__in unsigned long *, __RPC__in_xcount(0) unsigned char *, __RPC__out BSTR * ); 
void                      __RPC_USER  BSTR_UserFree(     __RPC__in unsigned long *, __RPC__in BSTR * ); 

/* end of Additional Prototypes */

#ifdef __cplusplus
}
#endif

#endif


