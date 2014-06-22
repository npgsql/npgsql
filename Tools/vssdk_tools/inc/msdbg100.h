

/* this ALWAYS GENERATED file contains the definitions for the interfaces */


 /* File created by MIDL compiler version 7.00.0499 */
/* Compiler settings for msdbg100.idl:
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

#ifndef __msdbg100_h__
#define __msdbg100_h__

#if defined(_MSC_VER) && (_MSC_VER >= 1020)
#pragma once
#endif

/* Forward Declarations */ 

#ifndef __IDebugThread100_FWD_DEFINED__
#define __IDebugThread100_FWD_DEFINED__
typedef interface IDebugThread100 IDebugThread100;
#endif 	/* __IDebugThread100_FWD_DEFINED__ */


#ifndef __IDebugCodeContext100_FWD_DEFINED__
#define __IDebugCodeContext100_FWD_DEFINED__
typedef interface IDebugCodeContext100 IDebugCodeContext100;
#endif 	/* __IDebugCodeContext100_FWD_DEFINED__ */


#ifndef __IDebugEngineLaunch100_FWD_DEFINED__
#define __IDebugEngineLaunch100_FWD_DEFINED__
typedef interface IDebugEngineLaunch100 IDebugEngineLaunch100;
#endif 	/* __IDebugEngineLaunch100_FWD_DEFINED__ */


#ifndef __IDebugThreadFlagChangeEvent100_FWD_DEFINED__
#define __IDebugThreadFlagChangeEvent100_FWD_DEFINED__
typedef interface IDebugThreadFlagChangeEvent100 IDebugThreadFlagChangeEvent100;
#endif 	/* __IDebugThreadFlagChangeEvent100_FWD_DEFINED__ */


#ifndef __IDebugThreadDisplayNameChangeEvent100_FWD_DEFINED__
#define __IDebugThreadDisplayNameChangeEvent100_FWD_DEFINED__
typedef interface IDebugThreadDisplayNameChangeEvent100 IDebugThreadDisplayNameChangeEvent100;
#endif 	/* __IDebugThreadDisplayNameChangeEvent100_FWD_DEFINED__ */


#ifndef __IDebugThreadSuspendChangeEvent100_FWD_DEFINED__
#define __IDebugThreadSuspendChangeEvent100_FWD_DEFINED__
typedef interface IDebugThreadSuspendChangeEvent100 IDebugThreadSuspendChangeEvent100;
#endif 	/* __IDebugThreadSuspendChangeEvent100_FWD_DEFINED__ */


#ifndef __IDebugProcessContinueEvent100_FWD_DEFINED__
#define __IDebugProcessContinueEvent100_FWD_DEFINED__
typedef interface IDebugProcessContinueEvent100 IDebugProcessContinueEvent100;
#endif 	/* __IDebugProcessContinueEvent100_FWD_DEFINED__ */


#ifndef __IDebugCurrentThreadChangedEvent100_FWD_DEFINED__
#define __IDebugCurrentThreadChangedEvent100_FWD_DEFINED__
typedef interface IDebugCurrentThreadChangedEvent100 IDebugCurrentThreadChangedEvent100;
#endif 	/* __IDebugCurrentThreadChangedEvent100_FWD_DEFINED__ */


#ifndef __IDebugSymbolSettings100_FWD_DEFINED__
#define __IDebugSymbolSettings100_FWD_DEFINED__
typedef interface IDebugSymbolSettings100 IDebugSymbolSettings100;
#endif 	/* __IDebugSymbolSettings100_FWD_DEFINED__ */


#ifndef __IDebugModuleReloadOperationCompleteEvent100_FWD_DEFINED__
#define __IDebugModuleReloadOperationCompleteEvent100_FWD_DEFINED__
typedef interface IDebugModuleReloadOperationCompleteEvent100 IDebugModuleReloadOperationCompleteEvent100;
#endif 	/* __IDebugModuleReloadOperationCompleteEvent100_FWD_DEFINED__ */


#ifndef __IDebugDumpModule100_FWD_DEFINED__
#define __IDebugDumpModule100_FWD_DEFINED__
typedef interface IDebugDumpModule100 IDebugDumpModule100;
#endif 	/* __IDebugDumpModule100_FWD_DEFINED__ */


/* header files for imported files */
#include "oaidl.h"
#include "ocidl.h"
#include "msdbg.h"

#ifdef __cplusplus
extern "C"{
#endif 


/* interface __MIDL_itf_msdbg100_0000_0000 */
/* [local] */ 

extern GUID guidCOMPlusOnlyEng2;
extern GUID guidCOMPlusOnlyEng4;
extern GUID guidFSharpLang;


extern RPC_IF_HANDLE __MIDL_itf_msdbg100_0000_0000_v0_0_c_ifspec;
extern RPC_IF_HANDLE __MIDL_itf_msdbg100_0000_0000_v0_0_s_ifspec;

#ifndef __IDebugThread100_INTERFACE_DEFINED__
#define __IDebugThread100_INTERFACE_DEFINED__

/* interface IDebugThread100 */
/* [unique][uuid][object] */ 


enum enum_THREADPROPERTY_FIELDS100
    {	TPF100_ID	= 0x1,
	TPF100_SUSPENDCOUNT	= 0x2,
	TPF100_STATE	= 0x4,
	TPF100_PRIORITY	= 0x8,
	TPF100_NAME	= 0x10,
	TPF100_LOCATION	= 0x20,
	TPF100_ALLFIELDS	= 0xffffffff,
	TPF100_DISPLAY_NAME	= 0x40,
	TPF100_DISPLAY_NAME_PRIORITY	= 0x80,
	TPF100_CATEGORY	= 0x100,
	TPF100_AFFINITY	= 0x200,
	TPF100_PRIORITY_ID	= 0x400
    } ;
typedef DWORD THREADPROPERTY_FIELDS100;


enum enum_DISPLAY_NAME_PRIORITY100
    {	DISPLAY_NAME_PRI_LOWEST_CONFIDENCY_100	= 0x1,
	DISPLAY_NAME_PRI_LOW_CONFIDENCY_100	= 0x10,
	DISPLAY_NAM_PRI_DEFAULT_100	= 0x100,
	DISPLAY_NAME_PRI_NORMAL_100	= 0x1000,
	DISPLAY_NAME_PRI_HIGH_100	= 0x10000,
	DISPLAY_NAME_PRI_HIGHEST_100	= 0x100000
    } ;
typedef enum enum_DISPLAY_NAME_PRIORITY100 DISPLAY_NAME_PRIORITY100;

typedef struct _tagTHREADPROPERTIES100
    {
    THREADPROPERTY_FIELDS100 dwFields;
    DWORD dwThreadId;
    DWORD dwSuspendCount;
    DWORD dwThreadState;
    BSTR bstrPriority;
    BSTR bstrName;
    BSTR bstrLocation;
    BSTR bstrDisplayName;
    DWORD DisplayNamePriority;
    DWORD dwThreadCategory;
    UINT64 AffinityMask;
    DWORD dwManagedThreadId;
    int priorityId;
    } 	THREADPROPERTIES100;


EXTERN_C const IID IID_IDebugThread100;

#if defined(__cplusplus) && !defined(CINTERFACE)
    
    MIDL_INTERFACE("8590DD4C-BF52-4aef-8348-E0DAC9C65268")
    IDebugThread100 : public IUnknown
    {
    public:
        virtual HRESULT STDMETHODCALLTYPE GetThreadProperties100( 
            /* [in] */ THREADPROPERTY_FIELDS100 dwFields,
            /* [out] */ __RPC__out THREADPROPERTIES100 *ptp) = 0;
        
        virtual HRESULT STDMETHODCALLTYPE GetFlags( 
            /* [out] */ __RPC__out DWORD *pFlags) = 0;
        
        virtual HRESULT STDMETHODCALLTYPE SetFlags( 
            /* [in] */ DWORD flags) = 0;
        
        virtual HRESULT STDMETHODCALLTYPE CanDoFuncEval( void) = 0;
        
        virtual HRESULT STDMETHODCALLTYPE GetThreadDisplayName( 
            /* [out] */ __RPC__deref_out_opt BSTR *bstrDisplayName) = 0;
        
        virtual HRESULT STDMETHODCALLTYPE SetThreadDisplayName( 
            /* [in] */ __RPC__in BSTR bstrDisplayName) = 0;
        
    };
    
#else 	/* C style interface */

    typedef struct IDebugThread100Vtbl
    {
        BEGIN_INTERFACE
        
        HRESULT ( STDMETHODCALLTYPE *QueryInterface )( 
            IDebugThread100 * This,
            /* [in] */ __RPC__in REFIID riid,
            /* [iid_is][out] */ 
            __RPC__deref_out  void **ppvObject);
        
        ULONG ( STDMETHODCALLTYPE *AddRef )( 
            IDebugThread100 * This);
        
        ULONG ( STDMETHODCALLTYPE *Release )( 
            IDebugThread100 * This);
        
        HRESULT ( STDMETHODCALLTYPE *GetThreadProperties100 )( 
            IDebugThread100 * This,
            /* [in] */ THREADPROPERTY_FIELDS100 dwFields,
            /* [out] */ __RPC__out THREADPROPERTIES100 *ptp);
        
        HRESULT ( STDMETHODCALLTYPE *GetFlags )( 
            IDebugThread100 * This,
            /* [out] */ __RPC__out DWORD *pFlags);
        
        HRESULT ( STDMETHODCALLTYPE *SetFlags )( 
            IDebugThread100 * This,
            /* [in] */ DWORD flags);
        
        HRESULT ( STDMETHODCALLTYPE *CanDoFuncEval )( 
            IDebugThread100 * This);
        
        HRESULT ( STDMETHODCALLTYPE *GetThreadDisplayName )( 
            IDebugThread100 * This,
            /* [out] */ __RPC__deref_out_opt BSTR *bstrDisplayName);
        
        HRESULT ( STDMETHODCALLTYPE *SetThreadDisplayName )( 
            IDebugThread100 * This,
            /* [in] */ __RPC__in BSTR bstrDisplayName);
        
        END_INTERFACE
    } IDebugThread100Vtbl;

    interface IDebugThread100
    {
        CONST_VTBL struct IDebugThread100Vtbl *lpVtbl;
    };

    

#ifdef COBJMACROS


#define IDebugThread100_QueryInterface(This,riid,ppvObject)	\
    ( (This)->lpVtbl -> QueryInterface(This,riid,ppvObject) ) 

#define IDebugThread100_AddRef(This)	\
    ( (This)->lpVtbl -> AddRef(This) ) 

#define IDebugThread100_Release(This)	\
    ( (This)->lpVtbl -> Release(This) ) 


#define IDebugThread100_GetThreadProperties100(This,dwFields,ptp)	\
    ( (This)->lpVtbl -> GetThreadProperties100(This,dwFields,ptp) ) 

#define IDebugThread100_GetFlags(This,pFlags)	\
    ( (This)->lpVtbl -> GetFlags(This,pFlags) ) 

#define IDebugThread100_SetFlags(This,flags)	\
    ( (This)->lpVtbl -> SetFlags(This,flags) ) 

#define IDebugThread100_CanDoFuncEval(This)	\
    ( (This)->lpVtbl -> CanDoFuncEval(This) ) 

#define IDebugThread100_GetThreadDisplayName(This,bstrDisplayName)	\
    ( (This)->lpVtbl -> GetThreadDisplayName(This,bstrDisplayName) ) 

#define IDebugThread100_SetThreadDisplayName(This,bstrDisplayName)	\
    ( (This)->lpVtbl -> SetThreadDisplayName(This,bstrDisplayName) ) 

#endif /* COBJMACROS */


#endif 	/* C style interface */




#endif 	/* __IDebugThread100_INTERFACE_DEFINED__ */


#ifndef __IDebugCodeContext100_INTERFACE_DEFINED__
#define __IDebugCodeContext100_INTERFACE_DEFINED__

/* interface IDebugCodeContext100 */
/* [unique][uuid][object] */ 


EXTERN_C const IID IID_IDebugCodeContext100;

#if defined(__cplusplus) && !defined(CINTERFACE)
    
    MIDL_INTERFACE("653f13a7-7e53-4f48-bde8-8ef8d3bc3c57")
    IDebugCodeContext100 : public IUnknown
    {
    public:
        virtual HRESULT STDMETHODCALLTYPE GetProgram( 
            /* [out] */ __RPC__deref_out_opt IDebugProgram2 **ppProgram) = 0;
        
    };
    
#else 	/* C style interface */

    typedef struct IDebugCodeContext100Vtbl
    {
        BEGIN_INTERFACE
        
        HRESULT ( STDMETHODCALLTYPE *QueryInterface )( 
            IDebugCodeContext100 * This,
            /* [in] */ __RPC__in REFIID riid,
            /* [iid_is][out] */ 
            __RPC__deref_out  void **ppvObject);
        
        ULONG ( STDMETHODCALLTYPE *AddRef )( 
            IDebugCodeContext100 * This);
        
        ULONG ( STDMETHODCALLTYPE *Release )( 
            IDebugCodeContext100 * This);
        
        HRESULT ( STDMETHODCALLTYPE *GetProgram )( 
            IDebugCodeContext100 * This,
            /* [out] */ __RPC__deref_out_opt IDebugProgram2 **ppProgram);
        
        END_INTERFACE
    } IDebugCodeContext100Vtbl;

    interface IDebugCodeContext100
    {
        CONST_VTBL struct IDebugCodeContext100Vtbl *lpVtbl;
    };

    

#ifdef COBJMACROS


#define IDebugCodeContext100_QueryInterface(This,riid,ppvObject)	\
    ( (This)->lpVtbl -> QueryInterface(This,riid,ppvObject) ) 

#define IDebugCodeContext100_AddRef(This)	\
    ( (This)->lpVtbl -> AddRef(This) ) 

#define IDebugCodeContext100_Release(This)	\
    ( (This)->lpVtbl -> Release(This) ) 


#define IDebugCodeContext100_GetProgram(This,ppProgram)	\
    ( (This)->lpVtbl -> GetProgram(This,ppProgram) ) 

#endif /* COBJMACROS */


#endif 	/* C style interface */




#endif 	/* __IDebugCodeContext100_INTERFACE_DEFINED__ */


/* interface __MIDL_itf_msdbg100_0000_0002 */
/* [local] */ 


enum _PSI_FLAGS
    {	PSI_USESHOWWINDOW	= 0x1,
	PSI_USESIZE	= 0x2,
	PSI_USEPOSITION	= 0x4,
	PSI_USECOUNTCHARS	= 0x8,
	PSI_USEFILLATTRIBUTE	= 0x10,
	PSI_RUNFULLSCREEN	= 0x20,
	PSI_FORCEONFEEDBACK	= 0x40,
	PSI_FORCEOFFFEEDBACK	= 0x80,
	PSI_USESTDHANDLES	= 0x100,
	PSI_USECREATIONFLAGS	= 0x40000000,
	PSI_INHERITHANDLES	= 0x80000000
    } ;
typedef DWORD PSI_FLAGS;

typedef struct _PROCESS_STARTUP_INFO
    {
    LPCWSTR lpDesktop;
    LPCWSTR lpTitle;
    DWORD dwCreationFlags;
    DWORD dwX;
    DWORD dwY;
    DWORD dwXSize;
    DWORD dwYSize;
    DWORD dwXCountChars;
    DWORD dwYCountChars;
    DWORD dwFillAttribute;
    PSI_FLAGS flags;
    WORD wShowWindow;
    DWORD hStdInput;
    DWORD hStdOutput;
    DWORD hStdError;
    } 	PROCESS_STARTUP_INFO;



extern RPC_IF_HANDLE __MIDL_itf_msdbg100_0000_0002_v0_0_c_ifspec;
extern RPC_IF_HANDLE __MIDL_itf_msdbg100_0000_0002_v0_0_s_ifspec;

#ifndef __IDebugEngineLaunch100_INTERFACE_DEFINED__
#define __IDebugEngineLaunch100_INTERFACE_DEFINED__

/* interface IDebugEngineLaunch100 */
/* [unique][uuid][object] */ 


EXTERN_C const IID IID_IDebugEngineLaunch100;

#if defined(__cplusplus) && !defined(CINTERFACE)
    
    MIDL_INTERFACE("1ac3d265-c50c-4c12-9b1a-c93f7291fc12")
    IDebugEngineLaunch100 : public IUnknown
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
            /* [in] */ __RPC__in PROCESS_STARTUP_INFO *pStartupInfo,
            /* [in] */ __RPC__in_opt IDebugEventCallback2 *pCallback,
            /* [out] */ __RPC__deref_out_opt IDebugProcess2 **ppProcess) = 0;
        
    };
    
#else 	/* C style interface */

    typedef struct IDebugEngineLaunch100Vtbl
    {
        BEGIN_INTERFACE
        
        HRESULT ( STDMETHODCALLTYPE *QueryInterface )( 
            IDebugEngineLaunch100 * This,
            /* [in] */ __RPC__in REFIID riid,
            /* [iid_is][out] */ 
            __RPC__deref_out  void **ppvObject);
        
        ULONG ( STDMETHODCALLTYPE *AddRef )( 
            IDebugEngineLaunch100 * This);
        
        ULONG ( STDMETHODCALLTYPE *Release )( 
            IDebugEngineLaunch100 * This);
        
        HRESULT ( STDMETHODCALLTYPE *LaunchSuspended )( 
            IDebugEngineLaunch100 * This,
            /* [full][in] */ __RPC__in_opt LPCOLESTR pszServer,
            /* [in] */ __RPC__in_opt IDebugPort2 *pPort,
            /* [full][in] */ __RPC__in_opt LPCOLESTR pszExe,
            /* [full][in] */ __RPC__in_opt LPCOLESTR pszArgs,
            /* [full][in] */ __RPC__in_opt LPCOLESTR pszDir,
            /* [full][in] */ __RPC__in_opt BSTR bstrEnv,
            /* [full][in] */ __RPC__in_opt LPCOLESTR pszOptions,
            /* [in] */ LAUNCH_FLAGS dwLaunchFlags,
            /* [in] */ __RPC__in PROCESS_STARTUP_INFO *pStartupInfo,
            /* [in] */ __RPC__in_opt IDebugEventCallback2 *pCallback,
            /* [out] */ __RPC__deref_out_opt IDebugProcess2 **ppProcess);
        
        END_INTERFACE
    } IDebugEngineLaunch100Vtbl;

    interface IDebugEngineLaunch100
    {
        CONST_VTBL struct IDebugEngineLaunch100Vtbl *lpVtbl;
    };

    

#ifdef COBJMACROS


#define IDebugEngineLaunch100_QueryInterface(This,riid,ppvObject)	\
    ( (This)->lpVtbl -> QueryInterface(This,riid,ppvObject) ) 

#define IDebugEngineLaunch100_AddRef(This)	\
    ( (This)->lpVtbl -> AddRef(This) ) 

#define IDebugEngineLaunch100_Release(This)	\
    ( (This)->lpVtbl -> Release(This) ) 


#define IDebugEngineLaunch100_LaunchSuspended(This,pszServer,pPort,pszExe,pszArgs,pszDir,bstrEnv,pszOptions,dwLaunchFlags,pStartupInfo,pCallback,ppProcess)	\
    ( (This)->lpVtbl -> LaunchSuspended(This,pszServer,pPort,pszExe,pszArgs,pszDir,bstrEnv,pszOptions,dwLaunchFlags,pStartupInfo,pCallback,ppProcess) ) 

#endif /* COBJMACROS */


#endif 	/* C style interface */




#endif 	/* __IDebugEngineLaunch100_INTERFACE_DEFINED__ */


/* interface __MIDL_itf_msdbg100_0000_0003 */
/* [local] */ 


enum THREADFLAG
    {	THREADFLAG_None	= 0,
	THREADFLAG_Interesting	= 0x1
    } ;


extern RPC_IF_HANDLE __MIDL_itf_msdbg100_0000_0003_v0_0_c_ifspec;
extern RPC_IF_HANDLE __MIDL_itf_msdbg100_0000_0003_v0_0_s_ifspec;

#ifndef __IDebugThreadFlagChangeEvent100_INTERFACE_DEFINED__
#define __IDebugThreadFlagChangeEvent100_INTERFACE_DEFINED__

/* interface IDebugThreadFlagChangeEvent100 */
/* [unique][uuid][object] */ 


EXTERN_C const IID IID_IDebugThreadFlagChangeEvent100;

#if defined(__cplusplus) && !defined(CINTERFACE)
    
    MIDL_INTERFACE("A4F87F48-D3C4-4694-A1A3-1969E3639EE7")
    IDebugThreadFlagChangeEvent100 : public IUnknown
    {
    public:
        virtual HRESULT STDMETHODCALLTYPE GetThread( 
            /* [out] */ __RPC__deref_out_opt IDebugThread100 **ppThread) = 0;
        
    };
    
#else 	/* C style interface */

    typedef struct IDebugThreadFlagChangeEvent100Vtbl
    {
        BEGIN_INTERFACE
        
        HRESULT ( STDMETHODCALLTYPE *QueryInterface )( 
            IDebugThreadFlagChangeEvent100 * This,
            /* [in] */ __RPC__in REFIID riid,
            /* [iid_is][out] */ 
            __RPC__deref_out  void **ppvObject);
        
        ULONG ( STDMETHODCALLTYPE *AddRef )( 
            IDebugThreadFlagChangeEvent100 * This);
        
        ULONG ( STDMETHODCALLTYPE *Release )( 
            IDebugThreadFlagChangeEvent100 * This);
        
        HRESULT ( STDMETHODCALLTYPE *GetThread )( 
            IDebugThreadFlagChangeEvent100 * This,
            /* [out] */ __RPC__deref_out_opt IDebugThread100 **ppThread);
        
        END_INTERFACE
    } IDebugThreadFlagChangeEvent100Vtbl;

    interface IDebugThreadFlagChangeEvent100
    {
        CONST_VTBL struct IDebugThreadFlagChangeEvent100Vtbl *lpVtbl;
    };

    

#ifdef COBJMACROS


#define IDebugThreadFlagChangeEvent100_QueryInterface(This,riid,ppvObject)	\
    ( (This)->lpVtbl -> QueryInterface(This,riid,ppvObject) ) 

#define IDebugThreadFlagChangeEvent100_AddRef(This)	\
    ( (This)->lpVtbl -> AddRef(This) ) 

#define IDebugThreadFlagChangeEvent100_Release(This)	\
    ( (This)->lpVtbl -> Release(This) ) 


#define IDebugThreadFlagChangeEvent100_GetThread(This,ppThread)	\
    ( (This)->lpVtbl -> GetThread(This,ppThread) ) 

#endif /* COBJMACROS */


#endif 	/* C style interface */




#endif 	/* __IDebugThreadFlagChangeEvent100_INTERFACE_DEFINED__ */


#ifndef __IDebugThreadDisplayNameChangeEvent100_INTERFACE_DEFINED__
#define __IDebugThreadDisplayNameChangeEvent100_INTERFACE_DEFINED__

/* interface IDebugThreadDisplayNameChangeEvent100 */
/* [unique][uuid][object] */ 


EXTERN_C const IID IID_IDebugThreadDisplayNameChangeEvent100;

#if defined(__cplusplus) && !defined(CINTERFACE)
    
    MIDL_INTERFACE("39CBD675-BDF9-423a-884F-7AB36BF51AF9")
    IDebugThreadDisplayNameChangeEvent100 : public IUnknown
    {
    public:
        virtual HRESULT STDMETHODCALLTYPE GetThread( 
            /* [out] */ __RPC__deref_out_opt IDebugThread100 **ppThread) = 0;
        
    };
    
#else 	/* C style interface */

    typedef struct IDebugThreadDisplayNameChangeEvent100Vtbl
    {
        BEGIN_INTERFACE
        
        HRESULT ( STDMETHODCALLTYPE *QueryInterface )( 
            IDebugThreadDisplayNameChangeEvent100 * This,
            /* [in] */ __RPC__in REFIID riid,
            /* [iid_is][out] */ 
            __RPC__deref_out  void **ppvObject);
        
        ULONG ( STDMETHODCALLTYPE *AddRef )( 
            IDebugThreadDisplayNameChangeEvent100 * This);
        
        ULONG ( STDMETHODCALLTYPE *Release )( 
            IDebugThreadDisplayNameChangeEvent100 * This);
        
        HRESULT ( STDMETHODCALLTYPE *GetThread )( 
            IDebugThreadDisplayNameChangeEvent100 * This,
            /* [out] */ __RPC__deref_out_opt IDebugThread100 **ppThread);
        
        END_INTERFACE
    } IDebugThreadDisplayNameChangeEvent100Vtbl;

    interface IDebugThreadDisplayNameChangeEvent100
    {
        CONST_VTBL struct IDebugThreadDisplayNameChangeEvent100Vtbl *lpVtbl;
    };

    

#ifdef COBJMACROS


#define IDebugThreadDisplayNameChangeEvent100_QueryInterface(This,riid,ppvObject)	\
    ( (This)->lpVtbl -> QueryInterface(This,riid,ppvObject) ) 

#define IDebugThreadDisplayNameChangeEvent100_AddRef(This)	\
    ( (This)->lpVtbl -> AddRef(This) ) 

#define IDebugThreadDisplayNameChangeEvent100_Release(This)	\
    ( (This)->lpVtbl -> Release(This) ) 


#define IDebugThreadDisplayNameChangeEvent100_GetThread(This,ppThread)	\
    ( (This)->lpVtbl -> GetThread(This,ppThread) ) 

#endif /* COBJMACROS */


#endif 	/* C style interface */




#endif 	/* __IDebugThreadDisplayNameChangeEvent100_INTERFACE_DEFINED__ */


#ifndef __IDebugThreadSuspendChangeEvent100_INTERFACE_DEFINED__
#define __IDebugThreadSuspendChangeEvent100_INTERFACE_DEFINED__

/* interface IDebugThreadSuspendChangeEvent100 */
/* [unique][uuid][object] */ 


EXTERN_C const IID IID_IDebugThreadSuspendChangeEvent100;

#if defined(__cplusplus) && !defined(CINTERFACE)
    
    MIDL_INTERFACE("BD20393D-6D0C-44FB-A5E3-7BD5D1B640D1")
    IDebugThreadSuspendChangeEvent100 : public IUnknown
    {
    public:
        virtual HRESULT STDMETHODCALLTYPE GetThread( 
            /* [out] */ __RPC__deref_out_opt IDebugThread100 **ppThread) = 0;
        
    };
    
#else 	/* C style interface */

    typedef struct IDebugThreadSuspendChangeEvent100Vtbl
    {
        BEGIN_INTERFACE
        
        HRESULT ( STDMETHODCALLTYPE *QueryInterface )( 
            IDebugThreadSuspendChangeEvent100 * This,
            /* [in] */ __RPC__in REFIID riid,
            /* [iid_is][out] */ 
            __RPC__deref_out  void **ppvObject);
        
        ULONG ( STDMETHODCALLTYPE *AddRef )( 
            IDebugThreadSuspendChangeEvent100 * This);
        
        ULONG ( STDMETHODCALLTYPE *Release )( 
            IDebugThreadSuspendChangeEvent100 * This);
        
        HRESULT ( STDMETHODCALLTYPE *GetThread )( 
            IDebugThreadSuspendChangeEvent100 * This,
            /* [out] */ __RPC__deref_out_opt IDebugThread100 **ppThread);
        
        END_INTERFACE
    } IDebugThreadSuspendChangeEvent100Vtbl;

    interface IDebugThreadSuspendChangeEvent100
    {
        CONST_VTBL struct IDebugThreadSuspendChangeEvent100Vtbl *lpVtbl;
    };

    

#ifdef COBJMACROS


#define IDebugThreadSuspendChangeEvent100_QueryInterface(This,riid,ppvObject)	\
    ( (This)->lpVtbl -> QueryInterface(This,riid,ppvObject) ) 

#define IDebugThreadSuspendChangeEvent100_AddRef(This)	\
    ( (This)->lpVtbl -> AddRef(This) ) 

#define IDebugThreadSuspendChangeEvent100_Release(This)	\
    ( (This)->lpVtbl -> Release(This) ) 


#define IDebugThreadSuspendChangeEvent100_GetThread(This,ppThread)	\
    ( (This)->lpVtbl -> GetThread(This,ppThread) ) 

#endif /* COBJMACROS */


#endif 	/* C style interface */




#endif 	/* __IDebugThreadSuspendChangeEvent100_INTERFACE_DEFINED__ */


#ifndef __IDebugProcessContinueEvent100_INTERFACE_DEFINED__
#define __IDebugProcessContinueEvent100_INTERFACE_DEFINED__

/* interface IDebugProcessContinueEvent100 */
/* [unique][uuid][object] */ 


EXTERN_C const IID IID_IDebugProcessContinueEvent100;

#if defined(__cplusplus) && !defined(CINTERFACE)
    
    MIDL_INTERFACE("C703EBEA-42E7-4768-85A9-692EECBA567B")
    IDebugProcessContinueEvent100 : public IUnknown
    {
    public:
        virtual HRESULT STDMETHODCALLTYPE GetProcess( 
            /* [out] */ __RPC__deref_out_opt IDebugProcess2 **ppProcess) = 0;
        
    };
    
#else 	/* C style interface */

    typedef struct IDebugProcessContinueEvent100Vtbl
    {
        BEGIN_INTERFACE
        
        HRESULT ( STDMETHODCALLTYPE *QueryInterface )( 
            IDebugProcessContinueEvent100 * This,
            /* [in] */ __RPC__in REFIID riid,
            /* [iid_is][out] */ 
            __RPC__deref_out  void **ppvObject);
        
        ULONG ( STDMETHODCALLTYPE *AddRef )( 
            IDebugProcessContinueEvent100 * This);
        
        ULONG ( STDMETHODCALLTYPE *Release )( 
            IDebugProcessContinueEvent100 * This);
        
        HRESULT ( STDMETHODCALLTYPE *GetProcess )( 
            IDebugProcessContinueEvent100 * This,
            /* [out] */ __RPC__deref_out_opt IDebugProcess2 **ppProcess);
        
        END_INTERFACE
    } IDebugProcessContinueEvent100Vtbl;

    interface IDebugProcessContinueEvent100
    {
        CONST_VTBL struct IDebugProcessContinueEvent100Vtbl *lpVtbl;
    };

    

#ifdef COBJMACROS


#define IDebugProcessContinueEvent100_QueryInterface(This,riid,ppvObject)	\
    ( (This)->lpVtbl -> QueryInterface(This,riid,ppvObject) ) 

#define IDebugProcessContinueEvent100_AddRef(This)	\
    ( (This)->lpVtbl -> AddRef(This) ) 

#define IDebugProcessContinueEvent100_Release(This)	\
    ( (This)->lpVtbl -> Release(This) ) 


#define IDebugProcessContinueEvent100_GetProcess(This,ppProcess)	\
    ( (This)->lpVtbl -> GetProcess(This,ppProcess) ) 

#endif /* COBJMACROS */


#endif 	/* C style interface */




#endif 	/* __IDebugProcessContinueEvent100_INTERFACE_DEFINED__ */


#ifndef __IDebugCurrentThreadChangedEvent100_INTERFACE_DEFINED__
#define __IDebugCurrentThreadChangedEvent100_INTERFACE_DEFINED__

/* interface IDebugCurrentThreadChangedEvent100 */
/* [unique][uuid][object] */ 


EXTERN_C const IID IID_IDebugCurrentThreadChangedEvent100;

#if defined(__cplusplus) && !defined(CINTERFACE)
    
    MIDL_INTERFACE("8764364B-0C52-4c7c-AF6A-8B19A8C98226")
    IDebugCurrentThreadChangedEvent100 : public IUnknown
    {
    public:
    };
    
#else 	/* C style interface */

    typedef struct IDebugCurrentThreadChangedEvent100Vtbl
    {
        BEGIN_INTERFACE
        
        HRESULT ( STDMETHODCALLTYPE *QueryInterface )( 
            IDebugCurrentThreadChangedEvent100 * This,
            /* [in] */ __RPC__in REFIID riid,
            /* [iid_is][out] */ 
            __RPC__deref_out  void **ppvObject);
        
        ULONG ( STDMETHODCALLTYPE *AddRef )( 
            IDebugCurrentThreadChangedEvent100 * This);
        
        ULONG ( STDMETHODCALLTYPE *Release )( 
            IDebugCurrentThreadChangedEvent100 * This);
        
        END_INTERFACE
    } IDebugCurrentThreadChangedEvent100Vtbl;

    interface IDebugCurrentThreadChangedEvent100
    {
        CONST_VTBL struct IDebugCurrentThreadChangedEvent100Vtbl *lpVtbl;
    };

    

#ifdef COBJMACROS


#define IDebugCurrentThreadChangedEvent100_QueryInterface(This,riid,ppvObject)	\
    ( (This)->lpVtbl -> QueryInterface(This,riid,ppvObject) ) 

#define IDebugCurrentThreadChangedEvent100_AddRef(This)	\
    ( (This)->lpVtbl -> AddRef(This) ) 

#define IDebugCurrentThreadChangedEvent100_Release(This)	\
    ( (This)->lpVtbl -> Release(This) ) 


#endif /* COBJMACROS */


#endif 	/* C style interface */




#endif 	/* __IDebugCurrentThreadChangedEvent100_INTERFACE_DEFINED__ */


/* interface __MIDL_itf_msdbg100_0000_0008 */
/* [local] */ 


enum enum_EVALFLAGS100
    {	EVAL100_RETURNVALUE	= 0x2,
	EVAL100_NOSIDEEFFECTS	= 0x4,
	EVAL100_ALLOWBPS	= 0x8,
	EVAL100_ALLOWERRORREPORT	= 0x10,
	EVAL100_FUNCTION_AS_ADDRESS	= 0x40,
	EVAL100_NOFUNCEVAL	= 0x80,
	EVAL100_NOEVENTS	= 0x1000,
	EVAL100_DESIGN_TIME_EXPR_EVAL	= 0x2000,
	EVAL100_ALLOW_IMPLICIT_VARS	= 0x4000,
	EVAL100_FORCE_EVALUATION_NOW	= 0x8000,
	EVAL100_NO_IL_INTERPRETER	= 0x10000,
	EVAL100_ALLOW_FUNC_EVALS_EVEN_IF_NO_SIDE_EFFECTS	= 0x20000,
	EVAL100_ALLOW_THREADSLIPPING	= 0x40000
    } ;

enum enum_DEBUGPROP_INFO_FLAGS100
    {	DEBUGPROP100_INFO_FULLNAME	= 0x1,
	DEBUGPROP100_INFO_NAME	= 0x2,
	DEBUGPROP100_INFO_TYPE	= 0x4,
	DEBUGPROP100_INFO_VALUE	= 0x8,
	DEBUGPROP100_INFO_ATTRIB	= 0x10,
	DEBUGPROP100_INFO_PROP	= 0x20,
	DEBUGPROP100_INFO_VALUE_AUTOEXPAND	= 0x10000,
	DEBUGPROP100_INFO_NOFUNCEVAL	= 0x20000,
	DEBUGPROP100_INFO_VALUE_RAW	= 0x40000,
	DEBUGPROP100_INFO_VALUE_NO_TOSTRING	= 0x80000,
	DEBUGPROP100_INFO_NO_NONPUBLIC_MEMBERS	= 0x100000,
	DEBUGPROP100_INFO_NONE	= 0,
	DEBUGPROP100_INFO_STANDARD	= ( ( ( DEBUGPROP100_INFO_ATTRIB | DEBUGPROP100_INFO_NAME )  | DEBUGPROP100_INFO_TYPE )  | DEBUGPROP100_INFO_VALUE ) ,
	DEBUGPROP100_INFO_ALL	= 0xffffffff,
	DEBUGPROP100_INFO_NOSIDEEFFECTS	= 0x200000,
	DEBUGPROP100_INFO_NO_IL_INTERPRETER	= 0x400000,
	DEBUGPROP100_INFO_ALLOW_FUNC_EVALS_EVEN_IF_NO_SIDE_EFFECTS	= 0x800000,
	DEBUGPROP100_INFO_ALLOW_THREADSLIPPING	= 0x1000000
    } ;
typedef DWORD DEBUGPROP100_INFO_FLAGS;


enum enum_MODULE100_FLAGS
    {	MODULE100_FLAG_NONE	= 0,
	MODULE100_FLAG_SYSTEM	= 0x1,
	MODULE100_FLAG_SYMBOLS	= 0x2,
	MODULE100_FLAG_64BIT	= 0x4,
	MODULE100_FLAG_OPTIMIZED	= 0x8,
	MODULE100_FLAG_UNOPTIMIZED	= 0x10,
	MODULE100_FLAG_ENGINEWILLLOADSYMS	= 0x20,
	MODULE100_FLAG_SYMBOLSUNAVAILABLE	= 0x40
    } ;
typedef DWORD MODULE100_FLAGS;

#define DBG_ATTRIB_VALUE_ERROR_THREAD_SLIP_REQUIRED 0x0004000000000000
#define DBG_ATTRIB_VALUE_ILINTERPRETER 0x0008000000000000


extern RPC_IF_HANDLE __MIDL_itf_msdbg100_0000_0008_v0_0_c_ifspec;
extern RPC_IF_HANDLE __MIDL_itf_msdbg100_0000_0008_v0_0_s_ifspec;

#ifndef __IDebugSymbolSettings100_INTERFACE_DEFINED__
#define __IDebugSymbolSettings100_INTERFACE_DEFINED__

/* interface IDebugSymbolSettings100 */
/* [unique][uuid][object] */ 


EXTERN_C const IID IID_IDebugSymbolSettings100;

#if defined(__cplusplus) && !defined(CINTERFACE)
    
    MIDL_INTERFACE("142A0821-8B28-49af-BD41-EABD00A88F57")
    IDebugSymbolSettings100 : public IUnknown
    {
    public:
        virtual HRESULT STDMETHODCALLTYPE SetSymbolLoadState( 
            /* [in] */ BOOL bIsManual,
            /* [in] */ BOOL bLoadAdjacentSymbols,
            /* [in] */ __RPC__in BSTR bstrIncludeList,
            /* [in] */ __RPC__in BSTR bstrExcludeList) = 0;
        
    };
    
#else 	/* C style interface */

    typedef struct IDebugSymbolSettings100Vtbl
    {
        BEGIN_INTERFACE
        
        HRESULT ( STDMETHODCALLTYPE *QueryInterface )( 
            IDebugSymbolSettings100 * This,
            /* [in] */ __RPC__in REFIID riid,
            /* [iid_is][out] */ 
            __RPC__deref_out  void **ppvObject);
        
        ULONG ( STDMETHODCALLTYPE *AddRef )( 
            IDebugSymbolSettings100 * This);
        
        ULONG ( STDMETHODCALLTYPE *Release )( 
            IDebugSymbolSettings100 * This);
        
        HRESULT ( STDMETHODCALLTYPE *SetSymbolLoadState )( 
            IDebugSymbolSettings100 * This,
            /* [in] */ BOOL bIsManual,
            /* [in] */ BOOL bLoadAdjacentSymbols,
            /* [in] */ __RPC__in BSTR bstrIncludeList,
            /* [in] */ __RPC__in BSTR bstrExcludeList);
        
        END_INTERFACE
    } IDebugSymbolSettings100Vtbl;

    interface IDebugSymbolSettings100
    {
        CONST_VTBL struct IDebugSymbolSettings100Vtbl *lpVtbl;
    };

    

#ifdef COBJMACROS


#define IDebugSymbolSettings100_QueryInterface(This,riid,ppvObject)	\
    ( (This)->lpVtbl -> QueryInterface(This,riid,ppvObject) ) 

#define IDebugSymbolSettings100_AddRef(This)	\
    ( (This)->lpVtbl -> AddRef(This) ) 

#define IDebugSymbolSettings100_Release(This)	\
    ( (This)->lpVtbl -> Release(This) ) 


#define IDebugSymbolSettings100_SetSymbolLoadState(This,bIsManual,bLoadAdjacentSymbols,bstrIncludeList,bstrExcludeList)	\
    ( (This)->lpVtbl -> SetSymbolLoadState(This,bIsManual,bLoadAdjacentSymbols,bstrIncludeList,bstrExcludeList) ) 

#endif /* COBJMACROS */


#endif 	/* C style interface */




#endif 	/* __IDebugSymbolSettings100_INTERFACE_DEFINED__ */


#ifndef __IDebugModuleReloadOperationCompleteEvent100_INTERFACE_DEFINED__
#define __IDebugModuleReloadOperationCompleteEvent100_INTERFACE_DEFINED__

/* interface IDebugModuleReloadOperationCompleteEvent100 */
/* [unique][uuid][object] */ 


EXTERN_C const IID IID_IDebugModuleReloadOperationCompleteEvent100;

#if defined(__cplusplus) && !defined(CINTERFACE)
    
    MIDL_INTERFACE("e72b36e9-8ec2-4dee-b96b-c18b752c98c8")
    IDebugModuleReloadOperationCompleteEvent100 : public IUnknown
    {
    public:
    };
    
#else 	/* C style interface */

    typedef struct IDebugModuleReloadOperationCompleteEvent100Vtbl
    {
        BEGIN_INTERFACE
        
        HRESULT ( STDMETHODCALLTYPE *QueryInterface )( 
            IDebugModuleReloadOperationCompleteEvent100 * This,
            /* [in] */ __RPC__in REFIID riid,
            /* [iid_is][out] */ 
            __RPC__deref_out  void **ppvObject);
        
        ULONG ( STDMETHODCALLTYPE *AddRef )( 
            IDebugModuleReloadOperationCompleteEvent100 * This);
        
        ULONG ( STDMETHODCALLTYPE *Release )( 
            IDebugModuleReloadOperationCompleteEvent100 * This);
        
        END_INTERFACE
    } IDebugModuleReloadOperationCompleteEvent100Vtbl;

    interface IDebugModuleReloadOperationCompleteEvent100
    {
        CONST_VTBL struct IDebugModuleReloadOperationCompleteEvent100Vtbl *lpVtbl;
    };

    

#ifdef COBJMACROS


#define IDebugModuleReloadOperationCompleteEvent100_QueryInterface(This,riid,ppvObject)	\
    ( (This)->lpVtbl -> QueryInterface(This,riid,ppvObject) ) 

#define IDebugModuleReloadOperationCompleteEvent100_AddRef(This)	\
    ( (This)->lpVtbl -> AddRef(This) ) 

#define IDebugModuleReloadOperationCompleteEvent100_Release(This)	\
    ( (This)->lpVtbl -> Release(This) ) 


#endif /* COBJMACROS */


#endif 	/* C style interface */




#endif 	/* __IDebugModuleReloadOperationCompleteEvent100_INTERFACE_DEFINED__ */


#ifndef __IDebugDumpModule100_INTERFACE_DEFINED__
#define __IDebugDumpModule100_INTERFACE_DEFINED__

/* interface IDebugDumpModule100 */
/* [unique][uuid][object] */ 


EXTERN_C const IID IID_IDebugDumpModule100;

#if defined(__cplusplus) && !defined(CINTERFACE)
    
    MIDL_INTERFACE("12e0c541-6479-4c3c-a48d-8ffa223208c2")
    IDebugDumpModule100 : public IUnknown
    {
    public:
        virtual HRESULT STDMETHODCALLTYPE IsBinaryLoaded( 
            /* [out] */ __RPC__out BOOL *pbLoaded) = 0;
        
        virtual HRESULT STDMETHODCALLTYPE LoadBinary( 
            /* [out] */ __RPC__deref_out_opt IDebugDumpModule100 **ppModule) = 0;
        
    };
    
#else 	/* C style interface */

    typedef struct IDebugDumpModule100Vtbl
    {
        BEGIN_INTERFACE
        
        HRESULT ( STDMETHODCALLTYPE *QueryInterface )( 
            IDebugDumpModule100 * This,
            /* [in] */ __RPC__in REFIID riid,
            /* [iid_is][out] */ 
            __RPC__deref_out  void **ppvObject);
        
        ULONG ( STDMETHODCALLTYPE *AddRef )( 
            IDebugDumpModule100 * This);
        
        ULONG ( STDMETHODCALLTYPE *Release )( 
            IDebugDumpModule100 * This);
        
        HRESULT ( STDMETHODCALLTYPE *IsBinaryLoaded )( 
            IDebugDumpModule100 * This,
            /* [out] */ __RPC__out BOOL *pbLoaded);
        
        HRESULT ( STDMETHODCALLTYPE *LoadBinary )( 
            IDebugDumpModule100 * This,
            /* [out] */ __RPC__deref_out_opt IDebugDumpModule100 **ppModule);
        
        END_INTERFACE
    } IDebugDumpModule100Vtbl;

    interface IDebugDumpModule100
    {
        CONST_VTBL struct IDebugDumpModule100Vtbl *lpVtbl;
    };

    

#ifdef COBJMACROS


#define IDebugDumpModule100_QueryInterface(This,riid,ppvObject)	\
    ( (This)->lpVtbl -> QueryInterface(This,riid,ppvObject) ) 

#define IDebugDumpModule100_AddRef(This)	\
    ( (This)->lpVtbl -> AddRef(This) ) 

#define IDebugDumpModule100_Release(This)	\
    ( (This)->lpVtbl -> Release(This) ) 


#define IDebugDumpModule100_IsBinaryLoaded(This,pbLoaded)	\
    ( (This)->lpVtbl -> IsBinaryLoaded(This,pbLoaded) ) 

#define IDebugDumpModule100_LoadBinary(This,ppModule)	\
    ( (This)->lpVtbl -> LoadBinary(This,ppModule) ) 

#endif /* COBJMACROS */


#endif 	/* C style interface */




#endif 	/* __IDebugDumpModule100_INTERFACE_DEFINED__ */


/* interface __MIDL_itf_msdbg100_0000_0011 */
/* [local] */ 


enum enum_FRAMEINFO_FLAGS_VALUES100
    {	FIFV_MAXFRAMES_EXCEEDED	= 0x10
    } ;


extern RPC_IF_HANDLE __MIDL_itf_msdbg100_0000_0011_v0_0_c_ifspec;
extern RPC_IF_HANDLE __MIDL_itf_msdbg100_0000_0011_v0_0_s_ifspec;

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


