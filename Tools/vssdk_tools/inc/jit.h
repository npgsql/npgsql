

/* this ALWAYS GENERATED file contains the definitions for the interfaces */


 /* File created by MIDL compiler version 7.00.0499 */
/* Compiler settings for jit.idl:
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

#ifndef __jit_h__
#define __jit_h__

#if defined(_MSC_VER) && (_MSC_VER >= 1020)
#pragma once
#endif

/* Forward Declarations */ 

#ifndef __IDebugIDEJIT2_FWD_DEFINED__
#define __IDebugIDEJIT2_FWD_DEFINED__
typedef interface IDebugIDEJIT2 IDebugIDEJIT2;
#endif 	/* __IDebugIDEJIT2_FWD_DEFINED__ */


#ifndef __IDebugJIT2_FWD_DEFINED__
#define __IDebugJIT2_FWD_DEFINED__
typedef interface IDebugJIT2 IDebugJIT2;
#endif 	/* __IDebugJIT2_FWD_DEFINED__ */


#ifndef __IDebugRuntimeJITServerProvider2_FWD_DEFINED__
#define __IDebugRuntimeJITServerProvider2_FWD_DEFINED__
typedef interface IDebugRuntimeJITServerProvider2 IDebugRuntimeJITServerProvider2;
#endif 	/* __IDebugRuntimeJITServerProvider2_FWD_DEFINED__ */


#ifndef __IDebugSetJITEventCallback2_FWD_DEFINED__
#define __IDebugSetJITEventCallback2_FWD_DEFINED__
typedef interface IDebugSetJITEventCallback2 IDebugSetJITEventCallback2;
#endif 	/* __IDebugSetJITEventCallback2_FWD_DEFINED__ */


#ifndef __IDebugProgramJIT2_FWD_DEFINED__
#define __IDebugProgramJIT2_FWD_DEFINED__
typedef interface IDebugProgramJIT2 IDebugProgramJIT2;
#endif 	/* __IDebugProgramJIT2_FWD_DEFINED__ */


#ifndef __IJITDebuggingHost2_FWD_DEFINED__
#define __IJITDebuggingHost2_FWD_DEFINED__
typedef interface IJITDebuggingHost2 IJITDebuggingHost2;
#endif 	/* __IJITDebuggingHost2_FWD_DEFINED__ */


#ifndef __IDebugWrappedJITDebugger2_FWD_DEFINED__
#define __IDebugWrappedJITDebugger2_FWD_DEFINED__
typedef interface IDebugWrappedJITDebugger2 IDebugWrappedJITDebugger2;
#endif 	/* __IDebugWrappedJITDebugger2_FWD_DEFINED__ */


#ifndef __IDebugEngineJITSettings2_FWD_DEFINED__
#define __IDebugEngineJITSettings2_FWD_DEFINED__
typedef interface IDebugEngineJITSettings2 IDebugEngineJITSettings2;
#endif 	/* __IDebugEngineJITSettings2_FWD_DEFINED__ */


#ifndef __IDEJITServer_FWD_DEFINED__
#define __IDEJITServer_FWD_DEFINED__

#ifdef __cplusplus
typedef class IDEJITServer IDEJITServer;
#else
typedef struct IDEJITServer IDEJITServer;
#endif /* __cplusplus */

#endif 	/* __IDEJITServer_FWD_DEFINED__ */


#ifndef __JITDebuggingHost_FWD_DEFINED__
#define __JITDebuggingHost_FWD_DEFINED__

#ifdef __cplusplus
typedef class JITDebuggingHost JITDebuggingHost;
#else
typedef struct JITDebuggingHost JITDebuggingHost;
#endif /* __cplusplus */

#endif 	/* __JITDebuggingHost_FWD_DEFINED__ */


/* header files for imported files */
#include "msdbg.h"

#ifdef __cplusplus
extern "C"{
#endif 


/* interface __MIDL_itf_jit_0000_0000 */
/* [local] */ 

#define JIT_DEUBUGGER_SHARED_MEMORY_PREFIX L"Local\\Microsoft_VS80_JIT_Debugger-"
#define MAX_JIT_DEUBUGGER_SHARED_MEMORY_SIZE 0x4000
#define MAX_MARSHALLED_DEBUGGER_SIZE (0x4000 - offsetof(JIT_DEUBUGGER_SHARED_MEMORY, MarshalledDebuggerData))
#define JIT_DEBUGGER_MAGIC_NUMBER ((DWORD)'JIT8')
struct JIT_DEUBUGGER_SHARED_MEMORY {
		DWORD dwMagicNumber;
		CLSID clsidDebugger;
		DWORD dwMarshalledDebuggerSize;
     BYTE  MarshalledDebuggerData[ANYSIZE_ARRAY];
};


enum enum_JIT_FLAGS
    {	JIT_FLAG_RPC	= 0x1,
	JIT_FLAG_NOCRASH	= 0x2,
	JIT_FLAG_DEBUGEXE	= 0x4,
	JIT_FLAG_SELECT_ENGINES	= 0x8
    } ;
typedef DWORD JIT_FLAGS;

typedef struct tagCRASHING_PROGRAM_INFO
    {
    GUID guidEngine;
    DWORD dwProcessId;
    UINT64 ProgramId;
    LPCOLESTR szExceptionText;
    JIT_FLAGS JitFlags;
    IDebugSetJITEventCallback2 *pSetEventCallback;
    } 	CRASHING_PROGRAM_INFO;



extern RPC_IF_HANDLE __MIDL_itf_jit_0000_0000_v0_0_c_ifspec;
extern RPC_IF_HANDLE __MIDL_itf_jit_0000_0000_v0_0_s_ifspec;

#ifndef __IDebugIDEJIT2_INTERFACE_DEFINED__
#define __IDebugIDEJIT2_INTERFACE_DEFINED__

/* interface IDebugIDEJIT2 */
/* [unique][uuid][object] */ 


EXTERN_C const IID IID_IDebugIDEJIT2;

#if defined(__cplusplus) && !defined(CINTERFACE)
    
    MIDL_INTERFACE("B2F73449-98EA-4866-90A0-425837FC5E23")
    IDebugIDEJIT2 : public IUnknown
    {
    public:
        virtual HRESULT STDMETHODCALLTYPE AttachJITDebugger( 
            /* [in] */ __RPC__in_opt IDebugProcess2 *pProcess,
            /* [in] */ __RPC__in_opt IDebugProgram2 *pJITProgram,
            /* [in] */ JIT_FLAGS JitFlags) = 0;
        
    };
    
#else 	/* C style interface */

    typedef struct IDebugIDEJIT2Vtbl
    {
        BEGIN_INTERFACE
        
        HRESULT ( STDMETHODCALLTYPE *QueryInterface )( 
            IDebugIDEJIT2 * This,
            /* [in] */ __RPC__in REFIID riid,
            /* [iid_is][out] */ 
            __RPC__deref_out  void **ppvObject);
        
        ULONG ( STDMETHODCALLTYPE *AddRef )( 
            IDebugIDEJIT2 * This);
        
        ULONG ( STDMETHODCALLTYPE *Release )( 
            IDebugIDEJIT2 * This);
        
        HRESULT ( STDMETHODCALLTYPE *AttachJITDebugger )( 
            IDebugIDEJIT2 * This,
            /* [in] */ __RPC__in_opt IDebugProcess2 *pProcess,
            /* [in] */ __RPC__in_opt IDebugProgram2 *pJITProgram,
            /* [in] */ JIT_FLAGS JitFlags);
        
        END_INTERFACE
    } IDebugIDEJIT2Vtbl;

    interface IDebugIDEJIT2
    {
        CONST_VTBL struct IDebugIDEJIT2Vtbl *lpVtbl;
    };

    

#ifdef COBJMACROS


#define IDebugIDEJIT2_QueryInterface(This,riid,ppvObject)	\
    ( (This)->lpVtbl -> QueryInterface(This,riid,ppvObject) ) 

#define IDebugIDEJIT2_AddRef(This)	\
    ( (This)->lpVtbl -> AddRef(This) ) 

#define IDebugIDEJIT2_Release(This)	\
    ( (This)->lpVtbl -> Release(This) ) 


#define IDebugIDEJIT2_AttachJITDebugger(This,pProcess,pJITProgram,JitFlags)	\
    ( (This)->lpVtbl -> AttachJITDebugger(This,pProcess,pJITProgram,JitFlags) ) 

#endif /* COBJMACROS */


#endif 	/* C style interface */




#endif 	/* __IDebugIDEJIT2_INTERFACE_DEFINED__ */


#ifndef __IDebugJIT2_INTERFACE_DEFINED__
#define __IDebugJIT2_INTERFACE_DEFINED__

/* interface IDebugJIT2 */
/* [unique][uuid][object] */ 


EXTERN_C const IID IID_IDebugJIT2;

#if defined(__cplusplus) && !defined(CINTERFACE)
    
    MIDL_INTERFACE("2ea4c00d-7156-4f8e-b990-fb4271147617")
    IDebugJIT2 : public IUnknown
    {
    public:
        virtual HRESULT STDMETHODCALLTYPE JITDebug( 
            /* [in] */ CRASHING_PROGRAM_INFO CrashingProgram) = 0;
        
    };
    
#else 	/* C style interface */

    typedef struct IDebugJIT2Vtbl
    {
        BEGIN_INTERFACE
        
        HRESULT ( STDMETHODCALLTYPE *QueryInterface )( 
            IDebugJIT2 * This,
            /* [in] */ __RPC__in REFIID riid,
            /* [iid_is][out] */ 
            __RPC__deref_out  void **ppvObject);
        
        ULONG ( STDMETHODCALLTYPE *AddRef )( 
            IDebugJIT2 * This);
        
        ULONG ( STDMETHODCALLTYPE *Release )( 
            IDebugJIT2 * This);
        
        HRESULT ( STDMETHODCALLTYPE *JITDebug )( 
            IDebugJIT2 * This,
            /* [in] */ CRASHING_PROGRAM_INFO CrashingProgram);
        
        END_INTERFACE
    } IDebugJIT2Vtbl;

    interface IDebugJIT2
    {
        CONST_VTBL struct IDebugJIT2Vtbl *lpVtbl;
    };

    

#ifdef COBJMACROS


#define IDebugJIT2_QueryInterface(This,riid,ppvObject)	\
    ( (This)->lpVtbl -> QueryInterface(This,riid,ppvObject) ) 

#define IDebugJIT2_AddRef(This)	\
    ( (This)->lpVtbl -> AddRef(This) ) 

#define IDebugJIT2_Release(This)	\
    ( (This)->lpVtbl -> Release(This) ) 


#define IDebugJIT2_JITDebug(This,CrashingProgram)	\
    ( (This)->lpVtbl -> JITDebug(This,CrashingProgram) ) 

#endif /* COBJMACROS */


#endif 	/* C style interface */




#endif 	/* __IDebugJIT2_INTERFACE_DEFINED__ */


#ifndef __IDebugRuntimeJITServerProvider2_INTERFACE_DEFINED__
#define __IDebugRuntimeJITServerProvider2_INTERFACE_DEFINED__

/* interface IDebugRuntimeJITServerProvider2 */
/* [unique][uuid][object] */ 


EXTERN_C const IID IID_IDebugRuntimeJITServerProvider2;

#if defined(__cplusplus) && !defined(CINTERFACE)
    
    MIDL_INTERFACE("e7f35f78-362c-4d8a-8f76-3a7dbae0a237")
    IDebugRuntimeJITServerProvider2 : public IUnknown
    {
    public:
        virtual HRESULT STDMETHODCALLTYPE GetRuntimeJITSession( 
            /* [out] */ __RPC__deref_out_opt IDebugJIT2 **ppJITServer) = 0;
        
        virtual HRESULT STDMETHODCALLTYPE IsAvailable( void) = 0;
        
    };
    
#else 	/* C style interface */

    typedef struct IDebugRuntimeJITServerProvider2Vtbl
    {
        BEGIN_INTERFACE
        
        HRESULT ( STDMETHODCALLTYPE *QueryInterface )( 
            IDebugRuntimeJITServerProvider2 * This,
            /* [in] */ __RPC__in REFIID riid,
            /* [iid_is][out] */ 
            __RPC__deref_out  void **ppvObject);
        
        ULONG ( STDMETHODCALLTYPE *AddRef )( 
            IDebugRuntimeJITServerProvider2 * This);
        
        ULONG ( STDMETHODCALLTYPE *Release )( 
            IDebugRuntimeJITServerProvider2 * This);
        
        HRESULT ( STDMETHODCALLTYPE *GetRuntimeJITSession )( 
            IDebugRuntimeJITServerProvider2 * This,
            /* [out] */ __RPC__deref_out_opt IDebugJIT2 **ppJITServer);
        
        HRESULT ( STDMETHODCALLTYPE *IsAvailable )( 
            IDebugRuntimeJITServerProvider2 * This);
        
        END_INTERFACE
    } IDebugRuntimeJITServerProvider2Vtbl;

    interface IDebugRuntimeJITServerProvider2
    {
        CONST_VTBL struct IDebugRuntimeJITServerProvider2Vtbl *lpVtbl;
    };

    

#ifdef COBJMACROS


#define IDebugRuntimeJITServerProvider2_QueryInterface(This,riid,ppvObject)	\
    ( (This)->lpVtbl -> QueryInterface(This,riid,ppvObject) ) 

#define IDebugRuntimeJITServerProvider2_AddRef(This)	\
    ( (This)->lpVtbl -> AddRef(This) ) 

#define IDebugRuntimeJITServerProvider2_Release(This)	\
    ( (This)->lpVtbl -> Release(This) ) 


#define IDebugRuntimeJITServerProvider2_GetRuntimeJITSession(This,ppJITServer)	\
    ( (This)->lpVtbl -> GetRuntimeJITSession(This,ppJITServer) ) 

#define IDebugRuntimeJITServerProvider2_IsAvailable(This)	\
    ( (This)->lpVtbl -> IsAvailable(This) ) 

#endif /* COBJMACROS */


#endif 	/* C style interface */




#endif 	/* __IDebugRuntimeJITServerProvider2_INTERFACE_DEFINED__ */


#ifndef __IDebugSetJITEventCallback2_INTERFACE_DEFINED__
#define __IDebugSetJITEventCallback2_INTERFACE_DEFINED__

/* interface IDebugSetJITEventCallback2 */
/* [unique][uuid][object] */ 


EXTERN_C const IID IID_IDebugSetJITEventCallback2;

#if defined(__cplusplus) && !defined(CINTERFACE)
    
    MIDL_INTERFACE("92DCA9C3-37A3-4498-8466-4C57EA885E49")
    IDebugSetJITEventCallback2 : public IUnknown
    {
    public:
        virtual HRESULT STDMETHODCALLTYPE SetJITEvent( void) = 0;
        
    };
    
#else 	/* C style interface */

    typedef struct IDebugSetJITEventCallback2Vtbl
    {
        BEGIN_INTERFACE
        
        HRESULT ( STDMETHODCALLTYPE *QueryInterface )( 
            IDebugSetJITEventCallback2 * This,
            /* [in] */ __RPC__in REFIID riid,
            /* [iid_is][out] */ 
            __RPC__deref_out  void **ppvObject);
        
        ULONG ( STDMETHODCALLTYPE *AddRef )( 
            IDebugSetJITEventCallback2 * This);
        
        ULONG ( STDMETHODCALLTYPE *Release )( 
            IDebugSetJITEventCallback2 * This);
        
        HRESULT ( STDMETHODCALLTYPE *SetJITEvent )( 
            IDebugSetJITEventCallback2 * This);
        
        END_INTERFACE
    } IDebugSetJITEventCallback2Vtbl;

    interface IDebugSetJITEventCallback2
    {
        CONST_VTBL struct IDebugSetJITEventCallback2Vtbl *lpVtbl;
    };

    

#ifdef COBJMACROS


#define IDebugSetJITEventCallback2_QueryInterface(This,riid,ppvObject)	\
    ( (This)->lpVtbl -> QueryInterface(This,riid,ppvObject) ) 

#define IDebugSetJITEventCallback2_AddRef(This)	\
    ( (This)->lpVtbl -> AddRef(This) ) 

#define IDebugSetJITEventCallback2_Release(This)	\
    ( (This)->lpVtbl -> Release(This) ) 


#define IDebugSetJITEventCallback2_SetJITEvent(This)	\
    ( (This)->lpVtbl -> SetJITEvent(This) ) 

#endif /* COBJMACROS */


#endif 	/* C style interface */




#endif 	/* __IDebugSetJITEventCallback2_INTERFACE_DEFINED__ */


#ifndef __IDebugProgramJIT2_INTERFACE_DEFINED__
#define __IDebugProgramJIT2_INTERFACE_DEFINED__

/* interface IDebugProgramJIT2 */
/* [unique][uuid][object] */ 


EXTERN_C const IID IID_IDebugProgramJIT2;

#if defined(__cplusplus) && !defined(CINTERFACE)
    
    MIDL_INTERFACE("87379803-2fad-4801-abdf-218b5d2f076f")
    IDebugProgramJIT2 : public IUnknown
    {
    public:
        virtual HRESULT STDMETHODCALLTYPE GetJITCallback( 
            /* [out] */ __RPC__deref_out_opt IDebugSetJITEventCallback2 **ppCallback) = 0;
        
    };
    
#else 	/* C style interface */

    typedef struct IDebugProgramJIT2Vtbl
    {
        BEGIN_INTERFACE
        
        HRESULT ( STDMETHODCALLTYPE *QueryInterface )( 
            IDebugProgramJIT2 * This,
            /* [in] */ __RPC__in REFIID riid,
            /* [iid_is][out] */ 
            __RPC__deref_out  void **ppvObject);
        
        ULONG ( STDMETHODCALLTYPE *AddRef )( 
            IDebugProgramJIT2 * This);
        
        ULONG ( STDMETHODCALLTYPE *Release )( 
            IDebugProgramJIT2 * This);
        
        HRESULT ( STDMETHODCALLTYPE *GetJITCallback )( 
            IDebugProgramJIT2 * This,
            /* [out] */ __RPC__deref_out_opt IDebugSetJITEventCallback2 **ppCallback);
        
        END_INTERFACE
    } IDebugProgramJIT2Vtbl;

    interface IDebugProgramJIT2
    {
        CONST_VTBL struct IDebugProgramJIT2Vtbl *lpVtbl;
    };

    

#ifdef COBJMACROS


#define IDebugProgramJIT2_QueryInterface(This,riid,ppvObject)	\
    ( (This)->lpVtbl -> QueryInterface(This,riid,ppvObject) ) 

#define IDebugProgramJIT2_AddRef(This)	\
    ( (This)->lpVtbl -> AddRef(This) ) 

#define IDebugProgramJIT2_Release(This)	\
    ( (This)->lpVtbl -> Release(This) ) 


#define IDebugProgramJIT2_GetJITCallback(This,ppCallback)	\
    ( (This)->lpVtbl -> GetJITCallback(This,ppCallback) ) 

#endif /* COBJMACROS */


#endif 	/* C style interface */




#endif 	/* __IDebugProgramJIT2_INTERFACE_DEFINED__ */


#ifndef __IJITDebuggingHost2_INTERFACE_DEFINED__
#define __IJITDebuggingHost2_INTERFACE_DEFINED__

/* interface IJITDebuggingHost2 */
/* [unique][uuid][object] */ 


EXTERN_C const IID IID_IJITDebuggingHost2;

#if defined(__cplusplus) && !defined(CINTERFACE)
    
    MIDL_INTERFACE("42ef9f29-7777-42ed-ba74-944aefd663da")
    IJITDebuggingHost2 : public IUnknown
    {
    public:
        virtual HRESULT STDMETHODCALLTYPE JITAsLoggedInUser( 
            /* [in] */ CRASHING_PROGRAM_INFO CrashingProgram) = 0;
        
    };
    
#else 	/* C style interface */

    typedef struct IJITDebuggingHost2Vtbl
    {
        BEGIN_INTERFACE
        
        HRESULT ( STDMETHODCALLTYPE *QueryInterface )( 
            IJITDebuggingHost2 * This,
            /* [in] */ __RPC__in REFIID riid,
            /* [iid_is][out] */ 
            __RPC__deref_out  void **ppvObject);
        
        ULONG ( STDMETHODCALLTYPE *AddRef )( 
            IJITDebuggingHost2 * This);
        
        ULONG ( STDMETHODCALLTYPE *Release )( 
            IJITDebuggingHost2 * This);
        
        HRESULT ( STDMETHODCALLTYPE *JITAsLoggedInUser )( 
            IJITDebuggingHost2 * This,
            /* [in] */ CRASHING_PROGRAM_INFO CrashingProgram);
        
        END_INTERFACE
    } IJITDebuggingHost2Vtbl;

    interface IJITDebuggingHost2
    {
        CONST_VTBL struct IJITDebuggingHost2Vtbl *lpVtbl;
    };

    

#ifdef COBJMACROS


#define IJITDebuggingHost2_QueryInterface(This,riid,ppvObject)	\
    ( (This)->lpVtbl -> QueryInterface(This,riid,ppvObject) ) 

#define IJITDebuggingHost2_AddRef(This)	\
    ( (This)->lpVtbl -> AddRef(This) ) 

#define IJITDebuggingHost2_Release(This)	\
    ( (This)->lpVtbl -> Release(This) ) 


#define IJITDebuggingHost2_JITAsLoggedInUser(This,CrashingProgram)	\
    ( (This)->lpVtbl -> JITAsLoggedInUser(This,CrashingProgram) ) 

#endif /* COBJMACROS */


#endif 	/* C style interface */




#endif 	/* __IJITDebuggingHost2_INTERFACE_DEFINED__ */


#ifndef __IDebugWrappedJITDebugger2_INTERFACE_DEFINED__
#define __IDebugWrappedJITDebugger2_INTERFACE_DEFINED__

/* interface IDebugWrappedJITDebugger2 */
/* [unique][uuid][object] */ 


EXTERN_C const IID IID_IDebugWrappedJITDebugger2;

#if defined(__cplusplus) && !defined(CINTERFACE)
    
    MIDL_INTERFACE("4dfa78ac-43c1-4de5-8179-3c3ec9010a31")
    IDebugWrappedJITDebugger2 : public IDebugJIT2
    {
    public:
        virtual HRESULT STDMETHODCALLTYPE GetName( 
            /* [out] */ __RPC__deref_out_opt BSTR *pbstrName) = 0;
        
    };
    
#else 	/* C style interface */

    typedef struct IDebugWrappedJITDebugger2Vtbl
    {
        BEGIN_INTERFACE
        
        HRESULT ( STDMETHODCALLTYPE *QueryInterface )( 
            IDebugWrappedJITDebugger2 * This,
            /* [in] */ __RPC__in REFIID riid,
            /* [iid_is][out] */ 
            __RPC__deref_out  void **ppvObject);
        
        ULONG ( STDMETHODCALLTYPE *AddRef )( 
            IDebugWrappedJITDebugger2 * This);
        
        ULONG ( STDMETHODCALLTYPE *Release )( 
            IDebugWrappedJITDebugger2 * This);
        
        HRESULT ( STDMETHODCALLTYPE *JITDebug )( 
            IDebugWrappedJITDebugger2 * This,
            /* [in] */ CRASHING_PROGRAM_INFO CrashingProgram);
        
        HRESULT ( STDMETHODCALLTYPE *GetName )( 
            IDebugWrappedJITDebugger2 * This,
            /* [out] */ __RPC__deref_out_opt BSTR *pbstrName);
        
        END_INTERFACE
    } IDebugWrappedJITDebugger2Vtbl;

    interface IDebugWrappedJITDebugger2
    {
        CONST_VTBL struct IDebugWrappedJITDebugger2Vtbl *lpVtbl;
    };

    

#ifdef COBJMACROS


#define IDebugWrappedJITDebugger2_QueryInterface(This,riid,ppvObject)	\
    ( (This)->lpVtbl -> QueryInterface(This,riid,ppvObject) ) 

#define IDebugWrappedJITDebugger2_AddRef(This)	\
    ( (This)->lpVtbl -> AddRef(This) ) 

#define IDebugWrappedJITDebugger2_Release(This)	\
    ( (This)->lpVtbl -> Release(This) ) 


#define IDebugWrappedJITDebugger2_JITDebug(This,CrashingProgram)	\
    ( (This)->lpVtbl -> JITDebug(This,CrashingProgram) ) 


#define IDebugWrappedJITDebugger2_GetName(This,pbstrName)	\
    ( (This)->lpVtbl -> GetName(This,pbstrName) ) 

#endif /* COBJMACROS */


#endif 	/* C style interface */




#endif 	/* __IDebugWrappedJITDebugger2_INTERFACE_DEFINED__ */


#ifndef __IDebugEngineJITSettings2_INTERFACE_DEFINED__
#define __IDebugEngineJITSettings2_INTERFACE_DEFINED__

/* interface IDebugEngineJITSettings2 */
/* [unique][uuid][object] */ 

typedef struct tagWRAPPED_DEBUGGER_ARRAY
    {
    DWORD dwCount;
    IDebugWrappedJITDebugger2 **Members;
    } 	WRAPPED_DEBUGGER_ARRAY;


EXTERN_C const IID IID_IDebugEngineJITSettings2;

#if defined(__cplusplus) && !defined(CINTERFACE)
    
    MIDL_INTERFACE("b9f3fdf1-7b6d-4899-bd94-72e4d4acd2e1")
    IDebugEngineJITSettings2 : public IUnknown
    {
    public:
        virtual HRESULT STDMETHODCALLTYPE QueryIsEnabled( void) = 0;
        
        virtual HRESULT STDMETHODCALLTYPE Enable( void) = 0;
        
        virtual HRESULT STDMETHODCALLTYPE GetWrappedDebuggers( 
            /* [out] */ __RPC__out WRAPPED_DEBUGGER_ARRAY *pWrappedDebuggers) = 0;
        
    };
    
#else 	/* C style interface */

    typedef struct IDebugEngineJITSettings2Vtbl
    {
        BEGIN_INTERFACE
        
        HRESULT ( STDMETHODCALLTYPE *QueryInterface )( 
            IDebugEngineJITSettings2 * This,
            /* [in] */ __RPC__in REFIID riid,
            /* [iid_is][out] */ 
            __RPC__deref_out  void **ppvObject);
        
        ULONG ( STDMETHODCALLTYPE *AddRef )( 
            IDebugEngineJITSettings2 * This);
        
        ULONG ( STDMETHODCALLTYPE *Release )( 
            IDebugEngineJITSettings2 * This);
        
        HRESULT ( STDMETHODCALLTYPE *QueryIsEnabled )( 
            IDebugEngineJITSettings2 * This);
        
        HRESULT ( STDMETHODCALLTYPE *Enable )( 
            IDebugEngineJITSettings2 * This);
        
        HRESULT ( STDMETHODCALLTYPE *GetWrappedDebuggers )( 
            IDebugEngineJITSettings2 * This,
            /* [out] */ __RPC__out WRAPPED_DEBUGGER_ARRAY *pWrappedDebuggers);
        
        END_INTERFACE
    } IDebugEngineJITSettings2Vtbl;

    interface IDebugEngineJITSettings2
    {
        CONST_VTBL struct IDebugEngineJITSettings2Vtbl *lpVtbl;
    };

    

#ifdef COBJMACROS


#define IDebugEngineJITSettings2_QueryInterface(This,riid,ppvObject)	\
    ( (This)->lpVtbl -> QueryInterface(This,riid,ppvObject) ) 

#define IDebugEngineJITSettings2_AddRef(This)	\
    ( (This)->lpVtbl -> AddRef(This) ) 

#define IDebugEngineJITSettings2_Release(This)	\
    ( (This)->lpVtbl -> Release(This) ) 


#define IDebugEngineJITSettings2_QueryIsEnabled(This)	\
    ( (This)->lpVtbl -> QueryIsEnabled(This) ) 

#define IDebugEngineJITSettings2_Enable(This)	\
    ( (This)->lpVtbl -> Enable(This) ) 

#define IDebugEngineJITSettings2_GetWrappedDebuggers(This,pWrappedDebuggers)	\
    ( (This)->lpVtbl -> GetWrappedDebuggers(This,pWrappedDebuggers) ) 

#endif /* COBJMACROS */


#endif 	/* C style interface */




#endif 	/* __IDebugEngineJITSettings2_INTERFACE_DEFINED__ */



#ifndef __JITLib_LIBRARY_DEFINED__
#define __JITLib_LIBRARY_DEFINED__

/* library JITLib */
/* [uuid] */ 


EXTERN_C const IID LIBID_JITLib;

EXTERN_C const CLSID CLSID_IDEJITServer;

#ifdef __cplusplus

class DECLSPEC_UUID("46D26D39-F692-4f6c-8153-086E6DA9F059")
IDEJITServer;
#endif

EXTERN_C const CLSID CLSID_JITDebuggingHost;

#ifdef __cplusplus

class DECLSPEC_UUID("36bbb745-0999-4fd8-a538-4d4d84e4bd09")
JITDebuggingHost;
#endif
#endif /* __JITLib_LIBRARY_DEFINED__ */

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


