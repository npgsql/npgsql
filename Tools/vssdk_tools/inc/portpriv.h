

/* this ALWAYS GENERATED file contains the definitions for the interfaces */


 /* File created by MIDL compiler version 7.00.0499 */
/* Compiler settings for portpriv.idl:
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

#ifndef __portpriv_h__
#define __portpriv_h__

#if defined(_MSC_VER) && (_MSC_VER >= 1020)
#pragma once
#endif

/* Forward Declarations */ 

#ifndef __IDebugPortSupplierEx2_FWD_DEFINED__
#define __IDebugPortSupplierEx2_FWD_DEFINED__
typedef interface IDebugPortSupplierEx2 IDebugPortSupplierEx2;
#endif 	/* __IDebugPortSupplierEx2_FWD_DEFINED__ */


#ifndef __IDebugPortSupplierLocale2_FWD_DEFINED__
#define __IDebugPortSupplierLocale2_FWD_DEFINED__
typedef interface IDebugPortSupplierLocale2 IDebugPortSupplierLocale2;
#endif 	/* __IDebugPortSupplierLocale2_FWD_DEFINED__ */


#ifndef __IDebugNativePort2_FWD_DEFINED__
#define __IDebugNativePort2_FWD_DEFINED__
typedef interface IDebugNativePort2 IDebugNativePort2;
#endif 	/* __IDebugNativePort2_FWD_DEFINED__ */


#ifndef __IDebugPortEx2_FWD_DEFINED__
#define __IDebugPortEx2_FWD_DEFINED__
typedef interface IDebugPortEx2 IDebugPortEx2;
#endif 	/* __IDebugPortEx2_FWD_DEFINED__ */


#ifndef __IDebugPortEventsEx2_FWD_DEFINED__
#define __IDebugPortEventsEx2_FWD_DEFINED__
typedef interface IDebugPortEventsEx2 IDebugPortEventsEx2;
#endif 	/* __IDebugPortEventsEx2_FWD_DEFINED__ */


#ifndef __IDebugProcessEx2_FWD_DEFINED__
#define __IDebugProcessEx2_FWD_DEFINED__
typedef interface IDebugProcessEx2 IDebugProcessEx2;
#endif 	/* __IDebugProcessEx2_FWD_DEFINED__ */


#ifndef __IDebugProgramEx2_FWD_DEFINED__
#define __IDebugProgramEx2_FWD_DEFINED__
typedef interface IDebugProgramEx2 IDebugProgramEx2;
#endif 	/* __IDebugProgramEx2_FWD_DEFINED__ */


#ifndef __IDebugAD1Program2_V7_FWD_DEFINED__
#define __IDebugAD1Program2_V7_FWD_DEFINED__
typedef interface IDebugAD1Program2_V7 IDebugAD1Program2_V7;
#endif 	/* __IDebugAD1Program2_V7_FWD_DEFINED__ */


/* header files for imported files */
#include "oaidl.h"
#include "ocidl.h"
#include "msdbg.h"
#include "activdbg.h"

#ifdef __cplusplus
extern "C"{
#endif 


#ifndef __IDebugPortSupplierEx2_INTERFACE_DEFINED__
#define __IDebugPortSupplierEx2_INTERFACE_DEFINED__

/* interface IDebugPortSupplierEx2 */
/* [unique][uuid][object] */ 


EXTERN_C const IID IID_IDebugPortSupplierEx2;

#if defined(__cplusplus) && !defined(CINTERFACE)
    
    MIDL_INTERFACE("c1f9ba1d-f70f-49f8-839e-5e0caa230306")
    IDebugPortSupplierEx2 : public IUnknown
    {
    public:
        virtual HRESULT STDMETHODCALLTYPE SetServer( 
            /* [in] */ __RPC__in_opt IDebugCoreServer2 *pServer) = 0;
        
    };
    
#else 	/* C style interface */

    typedef struct IDebugPortSupplierEx2Vtbl
    {
        BEGIN_INTERFACE
        
        HRESULT ( STDMETHODCALLTYPE *QueryInterface )( 
            IDebugPortSupplierEx2 * This,
            /* [in] */ __RPC__in REFIID riid,
            /* [iid_is][out] */ 
            __RPC__deref_out  void **ppvObject);
        
        ULONG ( STDMETHODCALLTYPE *AddRef )( 
            IDebugPortSupplierEx2 * This);
        
        ULONG ( STDMETHODCALLTYPE *Release )( 
            IDebugPortSupplierEx2 * This);
        
        HRESULT ( STDMETHODCALLTYPE *SetServer )( 
            IDebugPortSupplierEx2 * This,
            /* [in] */ __RPC__in_opt IDebugCoreServer2 *pServer);
        
        END_INTERFACE
    } IDebugPortSupplierEx2Vtbl;

    interface IDebugPortSupplierEx2
    {
        CONST_VTBL struct IDebugPortSupplierEx2Vtbl *lpVtbl;
    };

    

#ifdef COBJMACROS


#define IDebugPortSupplierEx2_QueryInterface(This,riid,ppvObject)	\
    ( (This)->lpVtbl -> QueryInterface(This,riid,ppvObject) ) 

#define IDebugPortSupplierEx2_AddRef(This)	\
    ( (This)->lpVtbl -> AddRef(This) ) 

#define IDebugPortSupplierEx2_Release(This)	\
    ( (This)->lpVtbl -> Release(This) ) 


#define IDebugPortSupplierEx2_SetServer(This,pServer)	\
    ( (This)->lpVtbl -> SetServer(This,pServer) ) 

#endif /* COBJMACROS */


#endif 	/* C style interface */




#endif 	/* __IDebugPortSupplierEx2_INTERFACE_DEFINED__ */


#ifndef __IDebugPortSupplierLocale2_INTERFACE_DEFINED__
#define __IDebugPortSupplierLocale2_INTERFACE_DEFINED__

/* interface IDebugPortSupplierLocale2 */
/* [unique][uuid][object] */ 


EXTERN_C const IID IID_IDebugPortSupplierLocale2;

#if defined(__cplusplus) && !defined(CINTERFACE)
    
    MIDL_INTERFACE("1bbab55e-6d13-4a5e-8c81-34ab2a3a6269")
    IDebugPortSupplierLocale2 : public IUnknown
    {
    public:
        virtual HRESULT STDMETHODCALLTYPE SetLocale( 
            /* [in] */ WORD wLangID) = 0;
        
    };
    
#else 	/* C style interface */

    typedef struct IDebugPortSupplierLocale2Vtbl
    {
        BEGIN_INTERFACE
        
        HRESULT ( STDMETHODCALLTYPE *QueryInterface )( 
            IDebugPortSupplierLocale2 * This,
            /* [in] */ __RPC__in REFIID riid,
            /* [iid_is][out] */ 
            __RPC__deref_out  void **ppvObject);
        
        ULONG ( STDMETHODCALLTYPE *AddRef )( 
            IDebugPortSupplierLocale2 * This);
        
        ULONG ( STDMETHODCALLTYPE *Release )( 
            IDebugPortSupplierLocale2 * This);
        
        HRESULT ( STDMETHODCALLTYPE *SetLocale )( 
            IDebugPortSupplierLocale2 * This,
            /* [in] */ WORD wLangID);
        
        END_INTERFACE
    } IDebugPortSupplierLocale2Vtbl;

    interface IDebugPortSupplierLocale2
    {
        CONST_VTBL struct IDebugPortSupplierLocale2Vtbl *lpVtbl;
    };

    

#ifdef COBJMACROS


#define IDebugPortSupplierLocale2_QueryInterface(This,riid,ppvObject)	\
    ( (This)->lpVtbl -> QueryInterface(This,riid,ppvObject) ) 

#define IDebugPortSupplierLocale2_AddRef(This)	\
    ( (This)->lpVtbl -> AddRef(This) ) 

#define IDebugPortSupplierLocale2_Release(This)	\
    ( (This)->lpVtbl -> Release(This) ) 


#define IDebugPortSupplierLocale2_SetLocale(This,wLangID)	\
    ( (This)->lpVtbl -> SetLocale(This,wLangID) ) 

#endif /* COBJMACROS */


#endif 	/* C style interface */




#endif 	/* __IDebugPortSupplierLocale2_INTERFACE_DEFINED__ */


#ifndef __IDebugNativePort2_INTERFACE_DEFINED__
#define __IDebugNativePort2_INTERFACE_DEFINED__

/* interface IDebugNativePort2 */
/* [unique][uuid][object] */ 


EXTERN_C const IID IID_IDebugNativePort2;

#if defined(__cplusplus) && !defined(CINTERFACE)
    
    MIDL_INTERFACE("521b4726-04e9-47e7-b3a5-cd93a7f74f5b")
    IDebugNativePort2 : public IUnknown
    {
    public:
        virtual HRESULT STDMETHODCALLTYPE AddProcess( 
            /* [in] */ AD_PROCESS_ID processId,
            /* [full][in] */ __RPC__in_opt LPCOLESTR pszProcessName,
            /* [in] */ BOOL fCanDetach,
            /* [out] */ __RPC__deref_out_opt IDebugProcess2 **ppPortProcess) = 0;
        
    };
    
#else 	/* C style interface */

    typedef struct IDebugNativePort2Vtbl
    {
        BEGIN_INTERFACE
        
        HRESULT ( STDMETHODCALLTYPE *QueryInterface )( 
            IDebugNativePort2 * This,
            /* [in] */ __RPC__in REFIID riid,
            /* [iid_is][out] */ 
            __RPC__deref_out  void **ppvObject);
        
        ULONG ( STDMETHODCALLTYPE *AddRef )( 
            IDebugNativePort2 * This);
        
        ULONG ( STDMETHODCALLTYPE *Release )( 
            IDebugNativePort2 * This);
        
        HRESULT ( STDMETHODCALLTYPE *AddProcess )( 
            IDebugNativePort2 * This,
            /* [in] */ AD_PROCESS_ID processId,
            /* [full][in] */ __RPC__in_opt LPCOLESTR pszProcessName,
            /* [in] */ BOOL fCanDetach,
            /* [out] */ __RPC__deref_out_opt IDebugProcess2 **ppPortProcess);
        
        END_INTERFACE
    } IDebugNativePort2Vtbl;

    interface IDebugNativePort2
    {
        CONST_VTBL struct IDebugNativePort2Vtbl *lpVtbl;
    };

    

#ifdef COBJMACROS


#define IDebugNativePort2_QueryInterface(This,riid,ppvObject)	\
    ( (This)->lpVtbl -> QueryInterface(This,riid,ppvObject) ) 

#define IDebugNativePort2_AddRef(This)	\
    ( (This)->lpVtbl -> AddRef(This) ) 

#define IDebugNativePort2_Release(This)	\
    ( (This)->lpVtbl -> Release(This) ) 


#define IDebugNativePort2_AddProcess(This,processId,pszProcessName,fCanDetach,ppPortProcess)	\
    ( (This)->lpVtbl -> AddProcess(This,processId,pszProcessName,fCanDetach,ppPortProcess) ) 

#endif /* COBJMACROS */


#endif 	/* C style interface */




#endif 	/* __IDebugNativePort2_INTERFACE_DEFINED__ */


#ifndef __IDebugPortEx2_INTERFACE_DEFINED__
#define __IDebugPortEx2_INTERFACE_DEFINED__

/* interface IDebugPortEx2 */
/* [unique][uuid][object] */ 


EXTERN_C const IID IID_IDebugPortEx2;

#if defined(__cplusplus) && !defined(CINTERFACE)
    
    MIDL_INTERFACE("e2314ee1-5c8c-4a9d-ad32-0c9a3574f685")
    IDebugPortEx2 : public IUnknown
    {
    public:
        virtual HRESULT STDMETHODCALLTYPE LaunchSuspended( 
            /* [full][in] */ __RPC__in_opt LPCOLESTR pszExe,
            /* [full][in] */ __RPC__in_opt LPCOLESTR pszArgs,
            /* [full][in] */ __RPC__in_opt LPCOLESTR pszDir,
            /* [full][in] */ __RPC__in_opt BSTR bstrEnv,
            /* [in] */ DWORD hStdInput,
            /* [in] */ DWORD hStdOutput,
            /* [in] */ DWORD hStdError,
            /* [out] */ __RPC__deref_out_opt IDebugProcess2 **ppPortProcess) = 0;
        
        virtual HRESULT STDMETHODCALLTYPE ResumeProcess( 
            /* [in] */ __RPC__in_opt IDebugProcess2 *pPortProcess) = 0;
        
        virtual HRESULT STDMETHODCALLTYPE CanTerminateProcess( 
            /* [in] */ __RPC__in_opt IDebugProcess2 *pPortProcess) = 0;
        
        virtual HRESULT STDMETHODCALLTYPE TerminateProcess( 
            /* [in] */ __RPC__in_opt IDebugProcess2 *pPortProcess) = 0;
        
        virtual HRESULT STDMETHODCALLTYPE GetPortProcessId( 
            /* [out] */ __RPC__out DWORD *pdwProcessId) = 0;
        
        virtual HRESULT STDMETHODCALLTYPE GetProgram( 
            /* [in] */ __RPC__in_opt IDebugProgramNode2 *pProgramNode,
            /* [out] */ __RPC__deref_out_opt IDebugProgram2 **ppProgram) = 0;
        
    };
    
#else 	/* C style interface */

    typedef struct IDebugPortEx2Vtbl
    {
        BEGIN_INTERFACE
        
        HRESULT ( STDMETHODCALLTYPE *QueryInterface )( 
            IDebugPortEx2 * This,
            /* [in] */ __RPC__in REFIID riid,
            /* [iid_is][out] */ 
            __RPC__deref_out  void **ppvObject);
        
        ULONG ( STDMETHODCALLTYPE *AddRef )( 
            IDebugPortEx2 * This);
        
        ULONG ( STDMETHODCALLTYPE *Release )( 
            IDebugPortEx2 * This);
        
        HRESULT ( STDMETHODCALLTYPE *LaunchSuspended )( 
            IDebugPortEx2 * This,
            /* [full][in] */ __RPC__in_opt LPCOLESTR pszExe,
            /* [full][in] */ __RPC__in_opt LPCOLESTR pszArgs,
            /* [full][in] */ __RPC__in_opt LPCOLESTR pszDir,
            /* [full][in] */ __RPC__in_opt BSTR bstrEnv,
            /* [in] */ DWORD hStdInput,
            /* [in] */ DWORD hStdOutput,
            /* [in] */ DWORD hStdError,
            /* [out] */ __RPC__deref_out_opt IDebugProcess2 **ppPortProcess);
        
        HRESULT ( STDMETHODCALLTYPE *ResumeProcess )( 
            IDebugPortEx2 * This,
            /* [in] */ __RPC__in_opt IDebugProcess2 *pPortProcess);
        
        HRESULT ( STDMETHODCALLTYPE *CanTerminateProcess )( 
            IDebugPortEx2 * This,
            /* [in] */ __RPC__in_opt IDebugProcess2 *pPortProcess);
        
        HRESULT ( STDMETHODCALLTYPE *TerminateProcess )( 
            IDebugPortEx2 * This,
            /* [in] */ __RPC__in_opt IDebugProcess2 *pPortProcess);
        
        HRESULT ( STDMETHODCALLTYPE *GetPortProcessId )( 
            IDebugPortEx2 * This,
            /* [out] */ __RPC__out DWORD *pdwProcessId);
        
        HRESULT ( STDMETHODCALLTYPE *GetProgram )( 
            IDebugPortEx2 * This,
            /* [in] */ __RPC__in_opt IDebugProgramNode2 *pProgramNode,
            /* [out] */ __RPC__deref_out_opt IDebugProgram2 **ppProgram);
        
        END_INTERFACE
    } IDebugPortEx2Vtbl;

    interface IDebugPortEx2
    {
        CONST_VTBL struct IDebugPortEx2Vtbl *lpVtbl;
    };

    

#ifdef COBJMACROS


#define IDebugPortEx2_QueryInterface(This,riid,ppvObject)	\
    ( (This)->lpVtbl -> QueryInterface(This,riid,ppvObject) ) 

#define IDebugPortEx2_AddRef(This)	\
    ( (This)->lpVtbl -> AddRef(This) ) 

#define IDebugPortEx2_Release(This)	\
    ( (This)->lpVtbl -> Release(This) ) 


#define IDebugPortEx2_LaunchSuspended(This,pszExe,pszArgs,pszDir,bstrEnv,hStdInput,hStdOutput,hStdError,ppPortProcess)	\
    ( (This)->lpVtbl -> LaunchSuspended(This,pszExe,pszArgs,pszDir,bstrEnv,hStdInput,hStdOutput,hStdError,ppPortProcess) ) 

#define IDebugPortEx2_ResumeProcess(This,pPortProcess)	\
    ( (This)->lpVtbl -> ResumeProcess(This,pPortProcess) ) 

#define IDebugPortEx2_CanTerminateProcess(This,pPortProcess)	\
    ( (This)->lpVtbl -> CanTerminateProcess(This,pPortProcess) ) 

#define IDebugPortEx2_TerminateProcess(This,pPortProcess)	\
    ( (This)->lpVtbl -> TerminateProcess(This,pPortProcess) ) 

#define IDebugPortEx2_GetPortProcessId(This,pdwProcessId)	\
    ( (This)->lpVtbl -> GetPortProcessId(This,pdwProcessId) ) 

#define IDebugPortEx2_GetProgram(This,pProgramNode,ppProgram)	\
    ( (This)->lpVtbl -> GetProgram(This,pProgramNode,ppProgram) ) 

#endif /* COBJMACROS */


#endif 	/* C style interface */




#endif 	/* __IDebugPortEx2_INTERFACE_DEFINED__ */


#ifndef __IDebugPortEventsEx2_INTERFACE_DEFINED__
#define __IDebugPortEventsEx2_INTERFACE_DEFINED__

/* interface IDebugPortEventsEx2 */
/* [unique][uuid][object] */ 


EXTERN_C const IID IID_IDebugPortEventsEx2;

#if defined(__cplusplus) && !defined(CINTERFACE)
    
    MIDL_INTERFACE("608a5912-e66b-4278-b6ed-847ac9318405")
    IDebugPortEventsEx2 : public IUnknown
    {
    public:
        virtual HRESULT STDMETHODCALLTYPE GetSession( 
            /* [out] */ __RPC__deref_out_opt IDebugSession2 **ppSession) = 0;
        
    };
    
#else 	/* C style interface */

    typedef struct IDebugPortEventsEx2Vtbl
    {
        BEGIN_INTERFACE
        
        HRESULT ( STDMETHODCALLTYPE *QueryInterface )( 
            IDebugPortEventsEx2 * This,
            /* [in] */ __RPC__in REFIID riid,
            /* [iid_is][out] */ 
            __RPC__deref_out  void **ppvObject);
        
        ULONG ( STDMETHODCALLTYPE *AddRef )( 
            IDebugPortEventsEx2 * This);
        
        ULONG ( STDMETHODCALLTYPE *Release )( 
            IDebugPortEventsEx2 * This);
        
        HRESULT ( STDMETHODCALLTYPE *GetSession )( 
            IDebugPortEventsEx2 * This,
            /* [out] */ __RPC__deref_out_opt IDebugSession2 **ppSession);
        
        END_INTERFACE
    } IDebugPortEventsEx2Vtbl;

    interface IDebugPortEventsEx2
    {
        CONST_VTBL struct IDebugPortEventsEx2Vtbl *lpVtbl;
    };

    

#ifdef COBJMACROS


#define IDebugPortEventsEx2_QueryInterface(This,riid,ppvObject)	\
    ( (This)->lpVtbl -> QueryInterface(This,riid,ppvObject) ) 

#define IDebugPortEventsEx2_AddRef(This)	\
    ( (This)->lpVtbl -> AddRef(This) ) 

#define IDebugPortEventsEx2_Release(This)	\
    ( (This)->lpVtbl -> Release(This) ) 


#define IDebugPortEventsEx2_GetSession(This,ppSession)	\
    ( (This)->lpVtbl -> GetSession(This,ppSession) ) 

#endif /* COBJMACROS */


#endif 	/* C style interface */




#endif 	/* __IDebugPortEventsEx2_INTERFACE_DEFINED__ */


#ifndef __IDebugProcessEx2_INTERFACE_DEFINED__
#define __IDebugProcessEx2_INTERFACE_DEFINED__

/* interface IDebugProcessEx2 */
/* [unique][uuid][object] */ 


EXTERN_C const IID IID_IDebugProcessEx2;

#if defined(__cplusplus) && !defined(CINTERFACE)
    
    MIDL_INTERFACE("82d71df5-6597-48c4-b5d5-b8b697fa36b5")
    IDebugProcessEx2 : public IUnknown
    {
    public:
        virtual HRESULT STDMETHODCALLTYPE Attach( 
            /* [in] */ __RPC__in_opt IDebugSession2 *pSession) = 0;
        
        virtual HRESULT STDMETHODCALLTYPE Detach( 
            /* [in] */ __RPC__in_opt IDebugSession2 *pSession) = 0;
        
        virtual HRESULT STDMETHODCALLTYPE AddImplicitProgramNodes( 
            /* [in] */ __RPC__in REFGUID guidLaunchingEngine,
            /* [size_is][in][full] */ __RPC__in_ecount_full_opt(celtSpecificEngines) GUID *rgguidSpecificEngines,
            /* [in] */ DWORD celtSpecificEngines) = 0;
        
    };
    
#else 	/* C style interface */

    typedef struct IDebugProcessEx2Vtbl
    {
        BEGIN_INTERFACE
        
        HRESULT ( STDMETHODCALLTYPE *QueryInterface )( 
            IDebugProcessEx2 * This,
            /* [in] */ __RPC__in REFIID riid,
            /* [iid_is][out] */ 
            __RPC__deref_out  void **ppvObject);
        
        ULONG ( STDMETHODCALLTYPE *AddRef )( 
            IDebugProcessEx2 * This);
        
        ULONG ( STDMETHODCALLTYPE *Release )( 
            IDebugProcessEx2 * This);
        
        HRESULT ( STDMETHODCALLTYPE *Attach )( 
            IDebugProcessEx2 * This,
            /* [in] */ __RPC__in_opt IDebugSession2 *pSession);
        
        HRESULT ( STDMETHODCALLTYPE *Detach )( 
            IDebugProcessEx2 * This,
            /* [in] */ __RPC__in_opt IDebugSession2 *pSession);
        
        HRESULT ( STDMETHODCALLTYPE *AddImplicitProgramNodes )( 
            IDebugProcessEx2 * This,
            /* [in] */ __RPC__in REFGUID guidLaunchingEngine,
            /* [size_is][in][full] */ __RPC__in_ecount_full_opt(celtSpecificEngines) GUID *rgguidSpecificEngines,
            /* [in] */ DWORD celtSpecificEngines);
        
        END_INTERFACE
    } IDebugProcessEx2Vtbl;

    interface IDebugProcessEx2
    {
        CONST_VTBL struct IDebugProcessEx2Vtbl *lpVtbl;
    };

    

#ifdef COBJMACROS


#define IDebugProcessEx2_QueryInterface(This,riid,ppvObject)	\
    ( (This)->lpVtbl -> QueryInterface(This,riid,ppvObject) ) 

#define IDebugProcessEx2_AddRef(This)	\
    ( (This)->lpVtbl -> AddRef(This) ) 

#define IDebugProcessEx2_Release(This)	\
    ( (This)->lpVtbl -> Release(This) ) 


#define IDebugProcessEx2_Attach(This,pSession)	\
    ( (This)->lpVtbl -> Attach(This,pSession) ) 

#define IDebugProcessEx2_Detach(This,pSession)	\
    ( (This)->lpVtbl -> Detach(This,pSession) ) 

#define IDebugProcessEx2_AddImplicitProgramNodes(This,guidLaunchingEngine,rgguidSpecificEngines,celtSpecificEngines)	\
    ( (This)->lpVtbl -> AddImplicitProgramNodes(This,guidLaunchingEngine,rgguidSpecificEngines,celtSpecificEngines) ) 

#endif /* COBJMACROS */


#endif 	/* C style interface */




#endif 	/* __IDebugProcessEx2_INTERFACE_DEFINED__ */


#ifndef __IDebugProgramEx2_INTERFACE_DEFINED__
#define __IDebugProgramEx2_INTERFACE_DEFINED__

/* interface IDebugProgramEx2 */
/* [unique][uuid][object] */ 


EXTERN_C const IID IID_IDebugProgramEx2;

#if defined(__cplusplus) && !defined(CINTERFACE)
    
    MIDL_INTERFACE("2959618a-a692-48ff-8cef-7a28a4f50954")
    IDebugProgramEx2 : public IUnknown
    {
    public:
        virtual HRESULT STDMETHODCALLTYPE Attach( 
            /* [in] */ __RPC__in_opt IDebugEventCallback2 *pCallback,
            /* [in] */ DWORD dwReason,
            /* [in] */ __RPC__in_opt IDebugSession2 *pSession) = 0;
        
        virtual HRESULT STDMETHODCALLTYPE GetProgramNode( 
            /* [out] */ __RPC__deref_out_opt IDebugProgramNode2 **ppProgramNode) = 0;
        
    };
    
#else 	/* C style interface */

    typedef struct IDebugProgramEx2Vtbl
    {
        BEGIN_INTERFACE
        
        HRESULT ( STDMETHODCALLTYPE *QueryInterface )( 
            IDebugProgramEx2 * This,
            /* [in] */ __RPC__in REFIID riid,
            /* [iid_is][out] */ 
            __RPC__deref_out  void **ppvObject);
        
        ULONG ( STDMETHODCALLTYPE *AddRef )( 
            IDebugProgramEx2 * This);
        
        ULONG ( STDMETHODCALLTYPE *Release )( 
            IDebugProgramEx2 * This);
        
        HRESULT ( STDMETHODCALLTYPE *Attach )( 
            IDebugProgramEx2 * This,
            /* [in] */ __RPC__in_opt IDebugEventCallback2 *pCallback,
            /* [in] */ DWORD dwReason,
            /* [in] */ __RPC__in_opt IDebugSession2 *pSession);
        
        HRESULT ( STDMETHODCALLTYPE *GetProgramNode )( 
            IDebugProgramEx2 * This,
            /* [out] */ __RPC__deref_out_opt IDebugProgramNode2 **ppProgramNode);
        
        END_INTERFACE
    } IDebugProgramEx2Vtbl;

    interface IDebugProgramEx2
    {
        CONST_VTBL struct IDebugProgramEx2Vtbl *lpVtbl;
    };

    

#ifdef COBJMACROS


#define IDebugProgramEx2_QueryInterface(This,riid,ppvObject)	\
    ( (This)->lpVtbl -> QueryInterface(This,riid,ppvObject) ) 

#define IDebugProgramEx2_AddRef(This)	\
    ( (This)->lpVtbl -> AddRef(This) ) 

#define IDebugProgramEx2_Release(This)	\
    ( (This)->lpVtbl -> Release(This) ) 


#define IDebugProgramEx2_Attach(This,pCallback,dwReason,pSession)	\
    ( (This)->lpVtbl -> Attach(This,pCallback,dwReason,pSession) ) 

#define IDebugProgramEx2_GetProgramNode(This,ppProgramNode)	\
    ( (This)->lpVtbl -> GetProgramNode(This,ppProgramNode) ) 

#endif /* COBJMACROS */


#endif 	/* C style interface */




#endif 	/* __IDebugProgramEx2_INTERFACE_DEFINED__ */


#ifndef __IDebugAD1Program2_V7_INTERFACE_DEFINED__
#define __IDebugAD1Program2_V7_INTERFACE_DEFINED__

/* interface IDebugAD1Program2_V7 */
/* [unique][uuid][object] */ 


EXTERN_C const IID IID_IDebugAD1Program2_V7;

#if defined(__cplusplus) && !defined(CINTERFACE)
    
    MIDL_INTERFACE("b7bdab6c-9077-43d0-87c4-96d1fd851446")
    IDebugAD1Program2_V7 : public IUnknown
    {
    public:
        virtual HRESULT STDMETHODCALLTYPE GetApplication( 
            /* [out] */ __RPC__deref_out_opt IRemoteDebugApplication **ppApp) = 0;
        
    };
    
#else 	/* C style interface */

    typedef struct IDebugAD1Program2_V7Vtbl
    {
        BEGIN_INTERFACE
        
        HRESULT ( STDMETHODCALLTYPE *QueryInterface )( 
            IDebugAD1Program2_V7 * This,
            /* [in] */ __RPC__in REFIID riid,
            /* [iid_is][out] */ 
            __RPC__deref_out  void **ppvObject);
        
        ULONG ( STDMETHODCALLTYPE *AddRef )( 
            IDebugAD1Program2_V7 * This);
        
        ULONG ( STDMETHODCALLTYPE *Release )( 
            IDebugAD1Program2_V7 * This);
        
        HRESULT ( STDMETHODCALLTYPE *GetApplication )( 
            IDebugAD1Program2_V7 * This,
            /* [out] */ __RPC__deref_out_opt IRemoteDebugApplication **ppApp);
        
        END_INTERFACE
    } IDebugAD1Program2_V7Vtbl;

    interface IDebugAD1Program2_V7
    {
        CONST_VTBL struct IDebugAD1Program2_V7Vtbl *lpVtbl;
    };

    

#ifdef COBJMACROS


#define IDebugAD1Program2_V7_QueryInterface(This,riid,ppvObject)	\
    ( (This)->lpVtbl -> QueryInterface(This,riid,ppvObject) ) 

#define IDebugAD1Program2_V7_AddRef(This)	\
    ( (This)->lpVtbl -> AddRef(This) ) 

#define IDebugAD1Program2_V7_Release(This)	\
    ( (This)->lpVtbl -> Release(This) ) 


#define IDebugAD1Program2_V7_GetApplication(This,ppApp)	\
    ( (This)->lpVtbl -> GetApplication(This,ppApp) ) 

#endif /* COBJMACROS */


#endif 	/* C style interface */




#endif 	/* __IDebugAD1Program2_V7_INTERFACE_DEFINED__ */


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


