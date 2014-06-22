

/* this ALWAYS GENERATED file contains the definitions for the interfaces */


 /* File created by MIDL compiler version 7.00.0499 */
/* Compiler settings for vbapkg.idl:
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


#ifndef __vbapkg_h__
#define __vbapkg_h__

#if defined(_MSC_VER) && (_MSC_VER >= 1020)
#pragma once
#endif

/* Forward Declarations */ 

#ifndef __IVsVba_FWD_DEFINED__
#define __IVsVba_FWD_DEFINED__
typedef interface IVsVba IVsVba;
#endif 	/* __IVsVba_FWD_DEFINED__ */


#ifndef __IVsMacros_FWD_DEFINED__
#define __IVsMacros_FWD_DEFINED__
typedef interface IVsMacros IVsMacros;
#endif 	/* __IVsMacros_FWD_DEFINED__ */


#ifndef __IVsMacroRecorder_FWD_DEFINED__
#define __IVsMacroRecorder_FWD_DEFINED__
typedef interface IVsMacroRecorder IVsMacroRecorder;
#endif 	/* __IVsMacroRecorder_FWD_DEFINED__ */


#ifndef __VsVbaPackage_FWD_DEFINED__
#define __VsVbaPackage_FWD_DEFINED__

#ifdef __cplusplus
typedef class VsVbaPackage VsVbaPackage;
#else
typedef struct VsVbaPackage VsVbaPackage;
#endif /* __cplusplus */

#endif 	/* __VsVbaPackage_FWD_DEFINED__ */


/* header files for imported files */
#include "oaidl.h"
#include "ocidl.h"
#include "vsshell.h"

#ifdef __cplusplus
extern "C"{
#endif 



#ifndef __VsVbaPackageLib_LIBRARY_DEFINED__
#define __VsVbaPackageLib_LIBRARY_DEFINED__

/* library VsVbaPackageLib */
/* [helpstring][version][uuid] */ 

const GUID GUID_Mode_Recorder = { 0x85a70471, 0x270a, 0x11d2, {0x88, 0xf9, 0x0, 0x60, 0x8, 0x31, 0x96, 0xc6} };
const GUID GUID_Macro = { 0x23162ff2, 0x3c3f, 0x11d2, {0x89, 0xa, 0x0, 0x60, 0x8, 0x31, 0x96, 0xc6} };
const GUID guidVsVbaPkg = { 0xa659f1b3, 0xad34, 0x11d1, {0xab, 0xad, 0x0, 0x80, 0xc7, 0xb8, 0x9c, 0x95} };
const GUID GUID_MacroProject = { 0x23162ff1, 0x3c3f, 0x11d2, {0x89, 0xa, 0x0, 0x60, 0x8, 0x31, 0x96, 0xc6} };
#define SID_SVsVba IID_IVsVba
#define SID_SVsMacros IID_IVsMacros
typedef 
enum _VSRECORDMODE
    {	VSRECORDMODE_ABSOLUTE	= 1,
	VSRECORDMODE_RELATIVE	= 2
    } 	VSRECORDMODE;

#define SID_SVsMacroRecorder IID_IVsMacroRecorder

EXTERN_C const IID LIBID_VsVbaPackageLib;

#ifndef __IVsVba_INTERFACE_DEFINED__
#define __IVsVba_INTERFACE_DEFINED__

/* interface IVsVba */
/* [object][unique][helpstring][uuid] */ 


EXTERN_C const IID IID_IVsVba;

#if defined(__cplusplus) && !defined(CINTERFACE)
    
    MIDL_INTERFACE("A659F1B2-AD34-11d1-ABAD-0080C7B89C95")
    IVsVba : public IUnknown
    {
    public:
        virtual HRESULT STDMETHODCALLTYPE ShowVBA( void) = 0;
        
        virtual HRESULT STDMETHODCALLTYPE GetVbaUnk( 
            __RPC__deref_in_opt IUnknown **pUnknown) = 0;
        
    };
    
#else 	/* C style interface */

    typedef struct IVsVbaVtbl
    {
        BEGIN_INTERFACE
        
        HRESULT ( STDMETHODCALLTYPE *QueryInterface )( 
            IVsVba * This,
            /* [in] */ __RPC__in REFIID riid,
            /* [iid_is][out] */ 
            __RPC__deref_out  void **ppvObject);
        
        ULONG ( STDMETHODCALLTYPE *AddRef )( 
            IVsVba * This);
        
        ULONG ( STDMETHODCALLTYPE *Release )( 
            IVsVba * This);
        
        HRESULT ( STDMETHODCALLTYPE *ShowVBA )( 
            IVsVba * This);
        
        HRESULT ( STDMETHODCALLTYPE *GetVbaUnk )( 
            IVsVba * This,
            __RPC__deref_in_opt IUnknown **pUnknown);
        
        END_INTERFACE
    } IVsVbaVtbl;

    interface IVsVba
    {
        CONST_VTBL struct IVsVbaVtbl *lpVtbl;
    };

    

#ifdef COBJMACROS


#define IVsVba_QueryInterface(This,riid,ppvObject)	\
    ( (This)->lpVtbl -> QueryInterface(This,riid,ppvObject) ) 

#define IVsVba_AddRef(This)	\
    ( (This)->lpVtbl -> AddRef(This) ) 

#define IVsVba_Release(This)	\
    ( (This)->lpVtbl -> Release(This) ) 


#define IVsVba_ShowVBA(This)	\
    ( (This)->lpVtbl -> ShowVBA(This) ) 

#define IVsVba_GetVbaUnk(This,pUnknown)	\
    ( (This)->lpVtbl -> GetVbaUnk(This,pUnknown) ) 

#endif /* COBJMACROS */


#endif 	/* C style interface */




#endif 	/* __IVsVba_INTERFACE_DEFINED__ */


#ifndef __IVsMacros_INTERFACE_DEFINED__
#define __IVsMacros_INTERFACE_DEFINED__

/* interface IVsMacros */
/* [object][unique][helpstring][uuid] */ 


EXTERN_C const IID IID_IVsMacros;

#if defined(__cplusplus) && !defined(CINTERFACE)
    
    MIDL_INTERFACE("55ED27C1-4CE7-11d2-890F-0060083196C6")
    IVsMacros : public IUnknown
    {
    public:
        virtual HRESULT STDMETHODCALLTYPE GetMacroCommands( 
            /* [out] */ __RPC__deref_out_opt SAFEARRAY **ppsaMacroCanonicalNames) = 0;
        
    };
    
#else 	/* C style interface */

    typedef struct IVsMacrosVtbl
    {
        BEGIN_INTERFACE
        
        HRESULT ( STDMETHODCALLTYPE *QueryInterface )( 
            IVsMacros * This,
            /* [in] */ __RPC__in REFIID riid,
            /* [iid_is][out] */ 
            __RPC__deref_out  void **ppvObject);
        
        ULONG ( STDMETHODCALLTYPE *AddRef )( 
            IVsMacros * This);
        
        ULONG ( STDMETHODCALLTYPE *Release )( 
            IVsMacros * This);
        
        HRESULT ( STDMETHODCALLTYPE *GetMacroCommands )( 
            IVsMacros * This,
            /* [out] */ __RPC__deref_out_opt SAFEARRAY **ppsaMacroCanonicalNames);
        
        END_INTERFACE
    } IVsMacrosVtbl;

    interface IVsMacros
    {
        CONST_VTBL struct IVsMacrosVtbl *lpVtbl;
    };

    

#ifdef COBJMACROS


#define IVsMacros_QueryInterface(This,riid,ppvObject)	\
    ( (This)->lpVtbl -> QueryInterface(This,riid,ppvObject) ) 

#define IVsMacros_AddRef(This)	\
    ( (This)->lpVtbl -> AddRef(This) ) 

#define IVsMacros_Release(This)	\
    ( (This)->lpVtbl -> Release(This) ) 


#define IVsMacros_GetMacroCommands(This,ppsaMacroCanonicalNames)	\
    ( (This)->lpVtbl -> GetMacroCommands(This,ppsaMacroCanonicalNames) ) 

#endif /* COBJMACROS */


#endif 	/* C style interface */




#endif 	/* __IVsMacros_INTERFACE_DEFINED__ */


#ifndef __IVsMacroRecorder_INTERFACE_DEFINED__
#define __IVsMacroRecorder_INTERFACE_DEFINED__

/* interface IVsMacroRecorder */
/* [object][unique][helpstring][uuid] */ 


EXTERN_C const IID IID_IVsMacroRecorder;

#if defined(__cplusplus) && !defined(CINTERFACE)
    
    MIDL_INTERFACE("04BBF6A5-4697-11d2-890E-0060083196C6")
    IVsMacroRecorder : public IUnknown
    {
    public:
        virtual HRESULT STDMETHODCALLTYPE RecordStart( 
            /* [in] */ __RPC__in LPCOLESTR pszReserved) = 0;
        
        virtual HRESULT STDMETHODCALLTYPE RecordEnd( void) = 0;
        
        virtual HRESULT STDMETHODCALLTYPE RecordLine( 
            /* [in] */ __RPC__in LPCOLESTR pszLine,
            /* [in] */ __RPC__in REFGUID rguidEmitter) = 0;
        
        virtual HRESULT STDMETHODCALLTYPE GetLastEmitterId( 
            /* [out] */ __RPC__out GUID *pguidEmitter) = 0;
        
        virtual HRESULT STDMETHODCALLTYPE ReplaceLine( 
            /* [in] */ __RPC__in LPCOLESTR pszLine,
            /* [in] */ __RPC__in REFGUID rguidEmitter) = 0;
        
        virtual HRESULT STDMETHODCALLTYPE RecordCancel( void) = 0;
        
        virtual HRESULT STDMETHODCALLTYPE RecordPause( void) = 0;
        
        virtual HRESULT STDMETHODCALLTYPE RecordResume( void) = 0;
        
        virtual HRESULT STDMETHODCALLTYPE SetCodeEmittedFlag( 
            /* [in] */ BOOL fFlag) = 0;
        
        virtual HRESULT STDMETHODCALLTYPE GetCodeEmittedFlag( 
            /* [out] */ __RPC__out BOOL *pfFlag) = 0;
        
        virtual HRESULT STDMETHODCALLTYPE GetKeyWord( 
            /* [in] */ UINT uiKeyWordId,
            /* [out] */ __RPC__deref_out_opt BSTR *pbstrKeyWord) = 0;
        
        virtual HRESULT STDMETHODCALLTYPE IsValidIdentifier( 
            /* [in] */ __RPC__in LPCOLESTR pszIdentifier) = 0;
        
        virtual HRESULT STDMETHODCALLTYPE GetRecordMode( 
            /* [out] */ __RPC__out VSRECORDMODE *peRecordMode) = 0;
        
        virtual HRESULT STDMETHODCALLTYPE SetRecordMode( 
            /* [in] */ VSRECORDMODE eRecordMode) = 0;
        
        virtual HRESULT STDMETHODCALLTYPE GetStringLiteralExpression( 
            /* [in] */ __RPC__in LPCOLESTR pszStringValue,
            /* [out] */ __RPC__deref_out_opt BSTR *pbstrLiteralExpression) = 0;
        
        virtual HRESULT STDMETHODCALLTYPE ExecuteLine( 
            /* [in] */ __RPC__in LPCOLESTR pszLine) = 0;
        
        virtual HRESULT STDMETHODCALLTYPE AddTypeLibRef( 
            /* [in] */ __RPC__in REFGUID guidTypeLib,
            /* [in] */ UINT uVerMaj,
            /* [in] */ UINT uVerMin) = 0;
        
    };
    
#else 	/* C style interface */

    typedef struct IVsMacroRecorderVtbl
    {
        BEGIN_INTERFACE
        
        HRESULT ( STDMETHODCALLTYPE *QueryInterface )( 
            IVsMacroRecorder * This,
            /* [in] */ __RPC__in REFIID riid,
            /* [iid_is][out] */ 
            __RPC__deref_out  void **ppvObject);
        
        ULONG ( STDMETHODCALLTYPE *AddRef )( 
            IVsMacroRecorder * This);
        
        ULONG ( STDMETHODCALLTYPE *Release )( 
            IVsMacroRecorder * This);
        
        HRESULT ( STDMETHODCALLTYPE *RecordStart )( 
            IVsMacroRecorder * This,
            /* [in] */ __RPC__in LPCOLESTR pszReserved);
        
        HRESULT ( STDMETHODCALLTYPE *RecordEnd )( 
            IVsMacroRecorder * This);
        
        HRESULT ( STDMETHODCALLTYPE *RecordLine )( 
            IVsMacroRecorder * This,
            /* [in] */ __RPC__in LPCOLESTR pszLine,
            /* [in] */ __RPC__in REFGUID rguidEmitter);
        
        HRESULT ( STDMETHODCALLTYPE *GetLastEmitterId )( 
            IVsMacroRecorder * This,
            /* [out] */ __RPC__out GUID *pguidEmitter);
        
        HRESULT ( STDMETHODCALLTYPE *ReplaceLine )( 
            IVsMacroRecorder * This,
            /* [in] */ __RPC__in LPCOLESTR pszLine,
            /* [in] */ __RPC__in REFGUID rguidEmitter);
        
        HRESULT ( STDMETHODCALLTYPE *RecordCancel )( 
            IVsMacroRecorder * This);
        
        HRESULT ( STDMETHODCALLTYPE *RecordPause )( 
            IVsMacroRecorder * This);
        
        HRESULT ( STDMETHODCALLTYPE *RecordResume )( 
            IVsMacroRecorder * This);
        
        HRESULT ( STDMETHODCALLTYPE *SetCodeEmittedFlag )( 
            IVsMacroRecorder * This,
            /* [in] */ BOOL fFlag);
        
        HRESULT ( STDMETHODCALLTYPE *GetCodeEmittedFlag )( 
            IVsMacroRecorder * This,
            /* [out] */ __RPC__out BOOL *pfFlag);
        
        HRESULT ( STDMETHODCALLTYPE *GetKeyWord )( 
            IVsMacroRecorder * This,
            /* [in] */ UINT uiKeyWordId,
            /* [out] */ __RPC__deref_out_opt BSTR *pbstrKeyWord);
        
        HRESULT ( STDMETHODCALLTYPE *IsValidIdentifier )( 
            IVsMacroRecorder * This,
            /* [in] */ __RPC__in LPCOLESTR pszIdentifier);
        
        HRESULT ( STDMETHODCALLTYPE *GetRecordMode )( 
            IVsMacroRecorder * This,
            /* [out] */ __RPC__out VSRECORDMODE *peRecordMode);
        
        HRESULT ( STDMETHODCALLTYPE *SetRecordMode )( 
            IVsMacroRecorder * This,
            /* [in] */ VSRECORDMODE eRecordMode);
        
        HRESULT ( STDMETHODCALLTYPE *GetStringLiteralExpression )( 
            IVsMacroRecorder * This,
            /* [in] */ __RPC__in LPCOLESTR pszStringValue,
            /* [out] */ __RPC__deref_out_opt BSTR *pbstrLiteralExpression);
        
        HRESULT ( STDMETHODCALLTYPE *ExecuteLine )( 
            IVsMacroRecorder * This,
            /* [in] */ __RPC__in LPCOLESTR pszLine);
        
        HRESULT ( STDMETHODCALLTYPE *AddTypeLibRef )( 
            IVsMacroRecorder * This,
            /* [in] */ __RPC__in REFGUID guidTypeLib,
            /* [in] */ UINT uVerMaj,
            /* [in] */ UINT uVerMin);
        
        END_INTERFACE
    } IVsMacroRecorderVtbl;

    interface IVsMacroRecorder
    {
        CONST_VTBL struct IVsMacroRecorderVtbl *lpVtbl;
    };

    

#ifdef COBJMACROS


#define IVsMacroRecorder_QueryInterface(This,riid,ppvObject)	\
    ( (This)->lpVtbl -> QueryInterface(This,riid,ppvObject) ) 

#define IVsMacroRecorder_AddRef(This)	\
    ( (This)->lpVtbl -> AddRef(This) ) 

#define IVsMacroRecorder_Release(This)	\
    ( (This)->lpVtbl -> Release(This) ) 


#define IVsMacroRecorder_RecordStart(This,pszReserved)	\
    ( (This)->lpVtbl -> RecordStart(This,pszReserved) ) 

#define IVsMacroRecorder_RecordEnd(This)	\
    ( (This)->lpVtbl -> RecordEnd(This) ) 

#define IVsMacroRecorder_RecordLine(This,pszLine,rguidEmitter)	\
    ( (This)->lpVtbl -> RecordLine(This,pszLine,rguidEmitter) ) 

#define IVsMacroRecorder_GetLastEmitterId(This,pguidEmitter)	\
    ( (This)->lpVtbl -> GetLastEmitterId(This,pguidEmitter) ) 

#define IVsMacroRecorder_ReplaceLine(This,pszLine,rguidEmitter)	\
    ( (This)->lpVtbl -> ReplaceLine(This,pszLine,rguidEmitter) ) 

#define IVsMacroRecorder_RecordCancel(This)	\
    ( (This)->lpVtbl -> RecordCancel(This) ) 

#define IVsMacroRecorder_RecordPause(This)	\
    ( (This)->lpVtbl -> RecordPause(This) ) 

#define IVsMacroRecorder_RecordResume(This)	\
    ( (This)->lpVtbl -> RecordResume(This) ) 

#define IVsMacroRecorder_SetCodeEmittedFlag(This,fFlag)	\
    ( (This)->lpVtbl -> SetCodeEmittedFlag(This,fFlag) ) 

#define IVsMacroRecorder_GetCodeEmittedFlag(This,pfFlag)	\
    ( (This)->lpVtbl -> GetCodeEmittedFlag(This,pfFlag) ) 

#define IVsMacroRecorder_GetKeyWord(This,uiKeyWordId,pbstrKeyWord)	\
    ( (This)->lpVtbl -> GetKeyWord(This,uiKeyWordId,pbstrKeyWord) ) 

#define IVsMacroRecorder_IsValidIdentifier(This,pszIdentifier)	\
    ( (This)->lpVtbl -> IsValidIdentifier(This,pszIdentifier) ) 

#define IVsMacroRecorder_GetRecordMode(This,peRecordMode)	\
    ( (This)->lpVtbl -> GetRecordMode(This,peRecordMode) ) 

#define IVsMacroRecorder_SetRecordMode(This,eRecordMode)	\
    ( (This)->lpVtbl -> SetRecordMode(This,eRecordMode) ) 

#define IVsMacroRecorder_GetStringLiteralExpression(This,pszStringValue,pbstrLiteralExpression)	\
    ( (This)->lpVtbl -> GetStringLiteralExpression(This,pszStringValue,pbstrLiteralExpression) ) 

#define IVsMacroRecorder_ExecuteLine(This,pszLine)	\
    ( (This)->lpVtbl -> ExecuteLine(This,pszLine) ) 

#define IVsMacroRecorder_AddTypeLibRef(This,guidTypeLib,uVerMaj,uVerMin)	\
    ( (This)->lpVtbl -> AddTypeLibRef(This,guidTypeLib,uVerMaj,uVerMin) ) 

#endif /* COBJMACROS */


#endif 	/* C style interface */




#endif 	/* __IVsMacroRecorder_INTERFACE_DEFINED__ */


EXTERN_C const CLSID CLSID_VsVbaPackage;

#ifdef __cplusplus

class DECLSPEC_UUID("A659F1B3-AD34-11d1-ABAD-0080C7B89C95")
VsVbaPackage;
#endif
#endif /* __VsVbaPackageLib_LIBRARY_DEFINED__ */

/* Additional Prototypes for ALL interfaces */

/* end of Additional Prototypes */

#ifdef __cplusplus
}
#endif

#endif


