

/* this ALWAYS GENERATED file contains the definitions for the interfaces */


 /* File created by MIDL compiler version 7.00.0499 */
/* Compiler settings for msdbg90.idl:
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

#ifndef __msdbg90_h__
#define __msdbg90_h__

#if defined(_MSC_VER) && (_MSC_VER >= 1020)
#pragma once
#endif

/* Forward Declarations */ 

#ifndef __IDebugCoreServer90_FWD_DEFINED__
#define __IDebugCoreServer90_FWD_DEFINED__
typedef interface IDebugCoreServer90 IDebugCoreServer90;
#endif 	/* __IDebugCoreServer90_FWD_DEFINED__ */


#ifndef __IDebugThread90_FWD_DEFINED__
#define __IDebugThread90_FWD_DEFINED__
typedef interface IDebugThread90 IDebugThread90;
#endif 	/* __IDebugThread90_FWD_DEFINED__ */


#ifndef __IDebugStepper90_FWD_DEFINED__
#define __IDebugStepper90_FWD_DEFINED__
typedef interface IDebugStepper90 IDebugStepper90;
#endif 	/* __IDebugStepper90_FWD_DEFINED__ */


#ifndef __IDebugCodePath90_FWD_DEFINED__
#define __IDebugCodePath90_FWD_DEFINED__
typedef interface IDebugCodePath90 IDebugCodePath90;
#endif 	/* __IDebugCodePath90_FWD_DEFINED__ */


#ifndef __IEnumDebugCodePaths90_FWD_DEFINED__
#define __IEnumDebugCodePaths90_FWD_DEFINED__
typedef interface IEnumDebugCodePaths90 IEnumDebugCodePaths90;
#endif 	/* __IEnumDebugCodePaths90_FWD_DEFINED__ */


#ifndef __IDebugProgramEnhancedStep90_FWD_DEFINED__
#define __IDebugProgramEnhancedStep90_FWD_DEFINED__
typedef interface IDebugProgramEnhancedStep90 IDebugProgramEnhancedStep90;
#endif 	/* __IDebugProgramEnhancedStep90_FWD_DEFINED__ */


#ifndef __IDebugProcessEnhancedStep90_FWD_DEFINED__
#define __IDebugProcessEnhancedStep90_FWD_DEFINED__
typedef interface IDebugProcessEnhancedStep90 IDebugProcessEnhancedStep90;
#endif 	/* __IDebugProcessEnhancedStep90_FWD_DEFINED__ */


#ifndef __IDebugEngineStepFilterManager90_FWD_DEFINED__
#define __IDebugEngineStepFilterManager90_FWD_DEFINED__
typedef interface IDebugEngineStepFilterManager90 IDebugEngineStepFilterManager90;
#endif 	/* __IDebugEngineStepFilterManager90_FWD_DEFINED__ */


#ifndef __IDebugStepCompleteEvent90_FWD_DEFINED__
#define __IDebugStepCompleteEvent90_FWD_DEFINED__
typedef interface IDebugStepCompleteEvent90 IDebugStepCompleteEvent90;
#endif 	/* __IDebugStepCompleteEvent90_FWD_DEFINED__ */


/* header files for imported files */
#include "oaidl.h"
#include "ocidl.h"
#include "msdbg.h"

#ifdef __cplusplus
extern "C"{
#endif 


/* interface __MIDL_itf_msdbg90_0000_0000 */
/* [local] */ 


enum enum_EVALFLAGS90
    {	EVAL90_RETURNVALUE	= 0x2,
	EVAL90_NOSIDEEFFECTS	= 0x4,
	EVAL90_ALLOWBPS	= 0x8,
	EVAL90_ALLOWERRORREPORT	= 0x10,
	EVAL90_FUNCTION_AS_ADDRESS	= 0x40,
	EVAL90_NOFUNCEVAL	= 0x80,
	EVAL90_NOEVENTS	= 0x1000,
	EVAL90_DESIGN_TIME_EXPR_EVAL	= 0x2000,
	EVAL90_ALLOW_IMPLICIT_VARS	= 0x4000,
	EVAL90_FORCE_EVALUATION_NOW	= 0x8000
    } ;

enum enum_BP_FLAGS90
    {	BP90_FLAG_NONE	= 0,
	BP90_FLAG_MAP_DOCPOSITION	= 0x1,
	BP90_FLAG_DONT_STOP	= 0x2,
	BP90_FLAG_TRACEPOINT_CONTINUE	= 0x4,
	BP90_FLAG_ALLOW_NON_USER	= 0x8,
	BP90_FLAG_USE_REQUEST_LANGUAGE	= 0x10
    } ;

enum enum_BPREQI_FIELDS90
    {	BPREQI90_BPLOCATION	= 0x1,
	BPREQI90_LANGUAGE	= 0x2,
	BPREQI90_PROGRAM	= 0x4,
	BPREQI90_PROGRAMNAME	= 0x8,
	BPREQI90_THREAD	= 0x10,
	BPREQI90_THREADNAME	= 0x20,
	BPREQI90_PASSCOUNT	= 0x40,
	BPREQI90_CONDITION	= 0x80,
	BPREQI90_FLAGS	= 0x100,
	BPREQI90_ALLOLDFIELDS	= 0x1ff,
	BPREQI90_VENDOR	= 0x200,
	BPREQI90_CONSTRAINT	= 0x400,
	BPREQI90_TRACEPOINT	= 0x800,
	BPREQI90_MACROTRACEPOINT	= 0x1000,
	BPREQI90_ALLFIELDS	= 0xffff
    } ;

enum enum_LAUNCH_FLAGS90
    {	LAUNCH_WAIT_FOR_EVENT	= 0x8
    } ;
typedef DWORD LAUNCH_FLAGS90;


enum enum_ENUMERATED_PROCESS_FLAGS90
    {	EPFLAG90_SHOW_SECURITY_WARNING	= 0x1,
	EPFLAG90_SYSTEM_PROCESS	= 0x2,
	EPFLAG90_INTEGRITY_LEVEL_MASK	= ( 7 << 2 ) ,
	EPFLAG90_INTEGRITY_LEVEL_NONE	= ( 0 << 2 ) ,
	EPFLAG90_INTEGRITY_LEVEL_UNTRUSTED	= ( 1 << 2 ) ,
	EPFLAG90_INTEGRITY_LEVEL_LOW	= ( 2 << 2 ) ,
	EPFLAG90_INTEGRITY_LEVEL_MEDIUM	= ( 3 << 2 ) ,
	EPFLAG90_INTEGRITY_LEVEL_HIGH	= ( 4 << 2 ) ,
	EPFLAG90_INTEGRITY_LEVEL_SYSTEM	= ( 5 << 2 ) ,
	EPFLAG90_INTEGRITY_LEVEL_PROTECTED	= ( 6 << 2 ) 
    } ;


extern RPC_IF_HANDLE __MIDL_itf_msdbg90_0000_0000_v0_0_c_ifspec;
extern RPC_IF_HANDLE __MIDL_itf_msdbg90_0000_0000_v0_0_s_ifspec;

#ifndef __IDebugCoreServer90_INTERFACE_DEFINED__
#define __IDebugCoreServer90_INTERFACE_DEFINED__

/* interface IDebugCoreServer90 */
/* [unique][uuid][object] */ 


EXTERN_C const IID IID_IDebugCoreServer90;

#if defined(__cplusplus) && !defined(CINTERFACE)
    
    MIDL_INTERFACE("1a1b5760-fe45-4958-aa3f-819060b16de9")
    IDebugCoreServer90 : public IUnknown
    {
    public:
        virtual HRESULT STDMETHODCALLTYPE CreateManagedInstanceInServer( 
            /* [in] */ __RPC__in LPCWSTR szClass,
            /* [in] */ __RPC__in LPCWSTR szAssembly,
            /* [in] */ WORD wLangId,
            /* [in] */ __RPC__in REFIID riid,
            /* [iid_is][out] */ __RPC__deref_out_opt void **ppvObject) = 0;
        
    };
    
#else 	/* C style interface */

    typedef struct IDebugCoreServer90Vtbl
    {
        BEGIN_INTERFACE
        
        HRESULT ( STDMETHODCALLTYPE *QueryInterface )( 
            IDebugCoreServer90 * This,
            /* [in] */ __RPC__in REFIID riid,
            /* [iid_is][out] */ 
            __RPC__deref_out  void **ppvObject);
        
        ULONG ( STDMETHODCALLTYPE *AddRef )( 
            IDebugCoreServer90 * This);
        
        ULONG ( STDMETHODCALLTYPE *Release )( 
            IDebugCoreServer90 * This);
        
        HRESULT ( STDMETHODCALLTYPE *CreateManagedInstanceInServer )( 
            IDebugCoreServer90 * This,
            /* [in] */ __RPC__in LPCWSTR szClass,
            /* [in] */ __RPC__in LPCWSTR szAssembly,
            /* [in] */ WORD wLangId,
            /* [in] */ __RPC__in REFIID riid,
            /* [iid_is][out] */ __RPC__deref_out_opt void **ppvObject);
        
        END_INTERFACE
    } IDebugCoreServer90Vtbl;

    interface IDebugCoreServer90
    {
        CONST_VTBL struct IDebugCoreServer90Vtbl *lpVtbl;
    };

    

#ifdef COBJMACROS


#define IDebugCoreServer90_QueryInterface(This,riid,ppvObject)	\
    ( (This)->lpVtbl -> QueryInterface(This,riid,ppvObject) ) 

#define IDebugCoreServer90_AddRef(This)	\
    ( (This)->lpVtbl -> AddRef(This) ) 

#define IDebugCoreServer90_Release(This)	\
    ( (This)->lpVtbl -> Release(This) ) 


#define IDebugCoreServer90_CreateManagedInstanceInServer(This,szClass,szAssembly,wLangId,riid,ppvObject)	\
    ( (This)->lpVtbl -> CreateManagedInstanceInServer(This,szClass,szAssembly,wLangId,riid,ppvObject) ) 

#endif /* COBJMACROS */


#endif 	/* C style interface */




#endif 	/* __IDebugCoreServer90_INTERFACE_DEFINED__ */


#ifndef __IDebugThread90_INTERFACE_DEFINED__
#define __IDebugThread90_INTERFACE_DEFINED__

/* interface IDebugThread90 */
/* [unique][uuid][object] */ 


enum enum_THREADPROPERTY_FIELDS90
    {	TPF90_ID	= 0x1,
	TPF90_SUSPENDCOUNT	= 0x2,
	TPF90_STATE	= 0x4,
	TPF90_PRIORITY	= 0x8,
	TPF90_NAME	= 0x10,
	TPF90_LOCATION	= 0x20,
	TPF90_ALLFIELDS	= 0xffffffff,
	TPF90_DISPLAY_NAME	= 0x40,
	TPF90_DISPLAY_NAME_PRIORITY	= 0x80,
	TPF90_CATEGORY	= 0x100
    } ;
typedef DWORD THREADPROPERTY_FIELDS90;


enum enum_DISPLAY_NAME_PRIORITY
    {	DISPLAY_NAME_PRI_LOWEST_CONFIDENCY	= 0x1,
	DISPLAY_NAME_PRI_LOW_CONFIDENCY	= 0x10,
	DISPLAY_NAM_PRI_DEFAULT	= 0x100,
	DISPLAY_NAME_PRI_NORMAL	= 0x1000,
	DISPLAY_NAME_PRI_HIGH	= 0x10000,
	DISPLAY_NAME_PRI_HIGHEST	= 0x100000
    } ;
typedef enum enum_DISPLAY_NAME_PRIORITY DISPLAY_NAME_PRIORITY;

typedef struct _tagTHREADPROPERTIES90
    {
    THREADPROPERTY_FIELDS90 dwFields;
    DWORD dwThreadId;
    DWORD dwSuspendCount;
    DWORD dwThreadState;
    BSTR bstrPriority;
    BSTR bstrName;
    BSTR bstrLocation;
    BSTR bstrDisplayName;
    DWORD DisplayNamePriority;
    DWORD dwThreadCategory;
    } 	THREADPROPERTIES90;


EXTERN_C const IID IID_IDebugThread90;

#if defined(__cplusplus) && !defined(CINTERFACE)
    
    MIDL_INTERFACE("8FD7F3BB-C09E-4c0c-830F-114FFA8BF4F8")
    IDebugThread90 : public IUnknown
    {
    public:
        virtual HRESULT STDMETHODCALLTYPE GetThreadProperties90( 
            /* [in] */ THREADPROPERTY_FIELDS90 dwFields,
            /* [out] */ __RPC__out THREADPROPERTIES90 *ptp) = 0;
        
    };
    
#else 	/* C style interface */

    typedef struct IDebugThread90Vtbl
    {
        BEGIN_INTERFACE
        
        HRESULT ( STDMETHODCALLTYPE *QueryInterface )( 
            IDebugThread90 * This,
            /* [in] */ __RPC__in REFIID riid,
            /* [iid_is][out] */ 
            __RPC__deref_out  void **ppvObject);
        
        ULONG ( STDMETHODCALLTYPE *AddRef )( 
            IDebugThread90 * This);
        
        ULONG ( STDMETHODCALLTYPE *Release )( 
            IDebugThread90 * This);
        
        HRESULT ( STDMETHODCALLTYPE *GetThreadProperties90 )( 
            IDebugThread90 * This,
            /* [in] */ THREADPROPERTY_FIELDS90 dwFields,
            /* [out] */ __RPC__out THREADPROPERTIES90 *ptp);
        
        END_INTERFACE
    } IDebugThread90Vtbl;

    interface IDebugThread90
    {
        CONST_VTBL struct IDebugThread90Vtbl *lpVtbl;
    };

    

#ifdef COBJMACROS


#define IDebugThread90_QueryInterface(This,riid,ppvObject)	\
    ( (This)->lpVtbl -> QueryInterface(This,riid,ppvObject) ) 

#define IDebugThread90_AddRef(This)	\
    ( (This)->lpVtbl -> AddRef(This) ) 

#define IDebugThread90_Release(This)	\
    ( (This)->lpVtbl -> Release(This) ) 


#define IDebugThread90_GetThreadProperties90(This,dwFields,ptp)	\
    ( (This)->lpVtbl -> GetThreadProperties90(This,dwFields,ptp) ) 

#endif /* COBJMACROS */


#endif 	/* C style interface */




#endif 	/* __IDebugThread90_INTERFACE_DEFINED__ */


/* interface __MIDL_itf_msdbg90_0000_0002 */
/* [local] */ 


enum enum_DEBUGPROP_INFO_FLAGS90
    {	DEBUGPROP90_INFO_FULLNAME	= 0x1,
	DEBUGPROP90_INFO_NAME	= 0x2,
	DEBUGPROP90_INFO_TYPE	= 0x4,
	DEBUGPROP90_INFO_VALUE	= 0x8,
	DEBUGPROP90_INFO_ATTRIB	= 0x10,
	DEBUGPROP90_INFO_PROP	= 0x20,
	DEBUGPROP90_INFO_VALUE_AUTOEXPAND	= 0x10000,
	DEBUGPROP90_INFO_NOFUNCEVAL	= 0x20000,
	DEBUGPROP90_INFO_VALUE_RAW	= 0x40000,
	DEBUGPROP90_INFO_VALUE_NO_TOSTRING	= 0x80000,
	DEBUGPROP90_INFO_NO_NONPUBLIC_MEMBERS	= 0x100000,
	DEBUGPROP90_INFO_NONE	= 0,
	DEBUGPROP90_INFO_STANDARD	= ( ( ( DEBUGPROP90_INFO_ATTRIB | DEBUGPROP90_INFO_NAME )  | DEBUGPROP90_INFO_TYPE )  | DEBUGPROP90_INFO_VALUE ) ,
	DEBUGPROP90_INFO_ALL	= 0xffffffff,
	DEBUGPROP90_INFO_NOSIDEEFFECTS	= 0x200000
    } ;
typedef DWORD DEBUGPROP90_INFO_FLAGS;


enum enum_STEPSTATUS
    {	STEPSTATUS_UNKNOWN	= 0,
	STEPSTATUS_IN	= 1,
	STEPSTATUS_NORMAL	= 2,
	STEPSTATUS_OUT	= 4,
	STEPSTATUS_THREAD_EXIT	= 8,
	STEPSTATUS_CUSTOM	= 16
    } ;
typedef DWORD STEPSTATUS;



extern RPC_IF_HANDLE __MIDL_itf_msdbg90_0000_0002_v0_0_c_ifspec;
extern RPC_IF_HANDLE __MIDL_itf_msdbg90_0000_0002_v0_0_s_ifspec;

#ifndef __IDebugStepper90_INTERFACE_DEFINED__
#define __IDebugStepper90_INTERFACE_DEFINED__

/* interface IDebugStepper90 */
/* [unique][uuid][object] */ 


EXTERN_C const IID IID_IDebugStepper90;

#if defined(__cplusplus) && !defined(CINTERFACE)
    
    MIDL_INTERFACE("c0aa96a2-ae7c-4d80-8831-c7c720f14cca")
    IDebugStepper90 : public IUnknown
    {
    public:
        virtual HRESULT STDMETHODCALLTYPE Step( 
            /* [in] */ __RPC__in_opt IDebugProgram2 *pProgram,
            /* [in] */ __RPC__in_opt IDebugThread2 *pThread) = 0;
        
        virtual HRESULT STDMETHODCALLTYPE GetStepKind( 
            /* [out] */ __RPC__out STEPKIND *pSK) = 0;
        
    };
    
#else 	/* C style interface */

    typedef struct IDebugStepper90Vtbl
    {
        BEGIN_INTERFACE
        
        HRESULT ( STDMETHODCALLTYPE *QueryInterface )( 
            IDebugStepper90 * This,
            /* [in] */ __RPC__in REFIID riid,
            /* [iid_is][out] */ 
            __RPC__deref_out  void **ppvObject);
        
        ULONG ( STDMETHODCALLTYPE *AddRef )( 
            IDebugStepper90 * This);
        
        ULONG ( STDMETHODCALLTYPE *Release )( 
            IDebugStepper90 * This);
        
        HRESULT ( STDMETHODCALLTYPE *Step )( 
            IDebugStepper90 * This,
            /* [in] */ __RPC__in_opt IDebugProgram2 *pProgram,
            /* [in] */ __RPC__in_opt IDebugThread2 *pThread);
        
        HRESULT ( STDMETHODCALLTYPE *GetStepKind )( 
            IDebugStepper90 * This,
            /* [out] */ __RPC__out STEPKIND *pSK);
        
        END_INTERFACE
    } IDebugStepper90Vtbl;

    interface IDebugStepper90
    {
        CONST_VTBL struct IDebugStepper90Vtbl *lpVtbl;
    };

    

#ifdef COBJMACROS


#define IDebugStepper90_QueryInterface(This,riid,ppvObject)	\
    ( (This)->lpVtbl -> QueryInterface(This,riid,ppvObject) ) 

#define IDebugStepper90_AddRef(This)	\
    ( (This)->lpVtbl -> AddRef(This) ) 

#define IDebugStepper90_Release(This)	\
    ( (This)->lpVtbl -> Release(This) ) 


#define IDebugStepper90_Step(This,pProgram,pThread)	\
    ( (This)->lpVtbl -> Step(This,pProgram,pThread) ) 

#define IDebugStepper90_GetStepKind(This,pSK)	\
    ( (This)->lpVtbl -> GetStepKind(This,pSK) ) 

#endif /* COBJMACROS */


#endif 	/* C style interface */




#endif 	/* __IDebugStepper90_INTERFACE_DEFINED__ */


#ifndef __IDebugCodePath90_INTERFACE_DEFINED__
#define __IDebugCodePath90_INTERFACE_DEFINED__

/* interface IDebugCodePath90 */
/* [unique][uuid][object] */ 


EXTERN_C const IID IID_IDebugCodePath90;

#if defined(__cplusplus) && !defined(CINTERFACE)
    
    MIDL_INTERFACE("7372dce0-f816-4e35-8b42-64b7f50e6395")
    IDebugCodePath90 : public IUnknown
    {
    public:
        virtual HRESULT STDMETHODCALLTYPE GetName( 
            /* [out] */ __RPC__deref_out_opt BSTR *pbstrName) = 0;
        
        virtual HRESULT STDMETHODCALLTYPE GetStepper( 
            /* [out] */ __RPC__deref_out_opt IDebugStepper90 **ppStepper) = 0;
        
    };
    
#else 	/* C style interface */

    typedef struct IDebugCodePath90Vtbl
    {
        BEGIN_INTERFACE
        
        HRESULT ( STDMETHODCALLTYPE *QueryInterface )( 
            IDebugCodePath90 * This,
            /* [in] */ __RPC__in REFIID riid,
            /* [iid_is][out] */ 
            __RPC__deref_out  void **ppvObject);
        
        ULONG ( STDMETHODCALLTYPE *AddRef )( 
            IDebugCodePath90 * This);
        
        ULONG ( STDMETHODCALLTYPE *Release )( 
            IDebugCodePath90 * This);
        
        HRESULT ( STDMETHODCALLTYPE *GetName )( 
            IDebugCodePath90 * This,
            /* [out] */ __RPC__deref_out_opt BSTR *pbstrName);
        
        HRESULT ( STDMETHODCALLTYPE *GetStepper )( 
            IDebugCodePath90 * This,
            /* [out] */ __RPC__deref_out_opt IDebugStepper90 **ppStepper);
        
        END_INTERFACE
    } IDebugCodePath90Vtbl;

    interface IDebugCodePath90
    {
        CONST_VTBL struct IDebugCodePath90Vtbl *lpVtbl;
    };

    

#ifdef COBJMACROS


#define IDebugCodePath90_QueryInterface(This,riid,ppvObject)	\
    ( (This)->lpVtbl -> QueryInterface(This,riid,ppvObject) ) 

#define IDebugCodePath90_AddRef(This)	\
    ( (This)->lpVtbl -> AddRef(This) ) 

#define IDebugCodePath90_Release(This)	\
    ( (This)->lpVtbl -> Release(This) ) 


#define IDebugCodePath90_GetName(This,pbstrName)	\
    ( (This)->lpVtbl -> GetName(This,pbstrName) ) 

#define IDebugCodePath90_GetStepper(This,ppStepper)	\
    ( (This)->lpVtbl -> GetStepper(This,ppStepper) ) 

#endif /* COBJMACROS */


#endif 	/* C style interface */




#endif 	/* __IDebugCodePath90_INTERFACE_DEFINED__ */


#ifndef __IEnumDebugCodePaths90_INTERFACE_DEFINED__
#define __IEnumDebugCodePaths90_INTERFACE_DEFINED__

/* interface IEnumDebugCodePaths90 */
/* [unique][uuid][object] */ 


EXTERN_C const IID IID_IEnumDebugCodePaths90;

#if defined(__cplusplus) && !defined(CINTERFACE)
    
    MIDL_INTERFACE("0b6520fb-1417-4599-9806-0d254f3c1869")
    IEnumDebugCodePaths90 : public IUnknown
    {
    public:
        virtual HRESULT STDMETHODCALLTYPE Next( 
            /* [in] */ ULONG celt,
            /* [length_is][size_is][out] */ __RPC__out_ecount_part(celt, *pceltFetched) IDebugCodePath90 **rgelt,
            /* [out][in] */ __RPC__inout ULONG *pceltFetched) = 0;
        
        virtual HRESULT STDMETHODCALLTYPE Skip( 
            /* [in] */ ULONG celt) = 0;
        
        virtual HRESULT STDMETHODCALLTYPE Reset( void) = 0;
        
        virtual HRESULT STDMETHODCALLTYPE Clone( 
            /* [out] */ __RPC__deref_out_opt IEnumDebugCodePaths90 **ppEnum) = 0;
        
        virtual HRESULT STDMETHODCALLTYPE GetCount( 
            /* [out] */ __RPC__out ULONG *pcelt) = 0;
        
    };
    
#else 	/* C style interface */

    typedef struct IEnumDebugCodePaths90Vtbl
    {
        BEGIN_INTERFACE
        
        HRESULT ( STDMETHODCALLTYPE *QueryInterface )( 
            IEnumDebugCodePaths90 * This,
            /* [in] */ __RPC__in REFIID riid,
            /* [iid_is][out] */ 
            __RPC__deref_out  void **ppvObject);
        
        ULONG ( STDMETHODCALLTYPE *AddRef )( 
            IEnumDebugCodePaths90 * This);
        
        ULONG ( STDMETHODCALLTYPE *Release )( 
            IEnumDebugCodePaths90 * This);
        
        HRESULT ( STDMETHODCALLTYPE *Next )( 
            IEnumDebugCodePaths90 * This,
            /* [in] */ ULONG celt,
            /* [length_is][size_is][out] */ __RPC__out_ecount_part(celt, *pceltFetched) IDebugCodePath90 **rgelt,
            /* [out][in] */ __RPC__inout ULONG *pceltFetched);
        
        HRESULT ( STDMETHODCALLTYPE *Skip )( 
            IEnumDebugCodePaths90 * This,
            /* [in] */ ULONG celt);
        
        HRESULT ( STDMETHODCALLTYPE *Reset )( 
            IEnumDebugCodePaths90 * This);
        
        HRESULT ( STDMETHODCALLTYPE *Clone )( 
            IEnumDebugCodePaths90 * This,
            /* [out] */ __RPC__deref_out_opt IEnumDebugCodePaths90 **ppEnum);
        
        HRESULT ( STDMETHODCALLTYPE *GetCount )( 
            IEnumDebugCodePaths90 * This,
            /* [out] */ __RPC__out ULONG *pcelt);
        
        END_INTERFACE
    } IEnumDebugCodePaths90Vtbl;

    interface IEnumDebugCodePaths90
    {
        CONST_VTBL struct IEnumDebugCodePaths90Vtbl *lpVtbl;
    };

    

#ifdef COBJMACROS


#define IEnumDebugCodePaths90_QueryInterface(This,riid,ppvObject)	\
    ( (This)->lpVtbl -> QueryInterface(This,riid,ppvObject) ) 

#define IEnumDebugCodePaths90_AddRef(This)	\
    ( (This)->lpVtbl -> AddRef(This) ) 

#define IEnumDebugCodePaths90_Release(This)	\
    ( (This)->lpVtbl -> Release(This) ) 


#define IEnumDebugCodePaths90_Next(This,celt,rgelt,pceltFetched)	\
    ( (This)->lpVtbl -> Next(This,celt,rgelt,pceltFetched) ) 

#define IEnumDebugCodePaths90_Skip(This,celt)	\
    ( (This)->lpVtbl -> Skip(This,celt) ) 

#define IEnumDebugCodePaths90_Reset(This)	\
    ( (This)->lpVtbl -> Reset(This) ) 

#define IEnumDebugCodePaths90_Clone(This,ppEnum)	\
    ( (This)->lpVtbl -> Clone(This,ppEnum) ) 

#define IEnumDebugCodePaths90_GetCount(This,pcelt)	\
    ( (This)->lpVtbl -> GetCount(This,pcelt) ) 

#endif /* COBJMACROS */


#endif 	/* C style interface */




#endif 	/* __IEnumDebugCodePaths90_INTERFACE_DEFINED__ */


#ifndef __IDebugProgramEnhancedStep90_INTERFACE_DEFINED__
#define __IDebugProgramEnhancedStep90_INTERFACE_DEFINED__

/* interface IDebugProgramEnhancedStep90 */
/* [unique][uuid][object] */ 


EXTERN_C const IID IID_IDebugProgramEnhancedStep90;

#if defined(__cplusplus) && !defined(CINTERFACE)
    
    MIDL_INTERFACE("6d07112f-b30d-47b4-b5ce-45b26c29205a")
    IDebugProgramEnhancedStep90 : public IUnknown
    {
    public:
        virtual HRESULT STDMETHODCALLTYPE EnumCodePaths( 
            /* [in] */ __RPC__in_opt IDebugThread2 *pThread,
            /* [in] */ __RPC__in_opt IDebugCodeContext2 *pStart,
            STEPUNIT stepUnit,
            /* [out] */ __RPC__deref_out_opt IEnumDebugCodePaths90 **ppEnum) = 0;
        
    };
    
#else 	/* C style interface */

    typedef struct IDebugProgramEnhancedStep90Vtbl
    {
        BEGIN_INTERFACE
        
        HRESULT ( STDMETHODCALLTYPE *QueryInterface )( 
            IDebugProgramEnhancedStep90 * This,
            /* [in] */ __RPC__in REFIID riid,
            /* [iid_is][out] */ 
            __RPC__deref_out  void **ppvObject);
        
        ULONG ( STDMETHODCALLTYPE *AddRef )( 
            IDebugProgramEnhancedStep90 * This);
        
        ULONG ( STDMETHODCALLTYPE *Release )( 
            IDebugProgramEnhancedStep90 * This);
        
        HRESULT ( STDMETHODCALLTYPE *EnumCodePaths )( 
            IDebugProgramEnhancedStep90 * This,
            /* [in] */ __RPC__in_opt IDebugThread2 *pThread,
            /* [in] */ __RPC__in_opt IDebugCodeContext2 *pStart,
            STEPUNIT stepUnit,
            /* [out] */ __RPC__deref_out_opt IEnumDebugCodePaths90 **ppEnum);
        
        END_INTERFACE
    } IDebugProgramEnhancedStep90Vtbl;

    interface IDebugProgramEnhancedStep90
    {
        CONST_VTBL struct IDebugProgramEnhancedStep90Vtbl *lpVtbl;
    };

    

#ifdef COBJMACROS


#define IDebugProgramEnhancedStep90_QueryInterface(This,riid,ppvObject)	\
    ( (This)->lpVtbl -> QueryInterface(This,riid,ppvObject) ) 

#define IDebugProgramEnhancedStep90_AddRef(This)	\
    ( (This)->lpVtbl -> AddRef(This) ) 

#define IDebugProgramEnhancedStep90_Release(This)	\
    ( (This)->lpVtbl -> Release(This) ) 


#define IDebugProgramEnhancedStep90_EnumCodePaths(This,pThread,pStart,stepUnit,ppEnum)	\
    ( (This)->lpVtbl -> EnumCodePaths(This,pThread,pStart,stepUnit,ppEnum) ) 

#endif /* COBJMACROS */


#endif 	/* C style interface */




#endif 	/* __IDebugProgramEnhancedStep90_INTERFACE_DEFINED__ */


#ifndef __IDebugProcessEnhancedStep90_INTERFACE_DEFINED__
#define __IDebugProcessEnhancedStep90_INTERFACE_DEFINED__

/* interface IDebugProcessEnhancedStep90 */
/* [unique][uuid][object] */ 


EXTERN_C const IID IID_IDebugProcessEnhancedStep90;

#if defined(__cplusplus) && !defined(CINTERFACE)
    
    MIDL_INTERFACE("c446ca51-979a-4096-b996-5934b7ab0455")
    IDebugProcessEnhancedStep90 : public IUnknown
    {
    public:
        virtual HRESULT STDMETHODCALLTYPE Step( 
            __RPC__in_opt IDebugThread2 *pThread,
            __RPC__in_opt IDebugStepper90 *pStepper) = 0;
        
    };
    
#else 	/* C style interface */

    typedef struct IDebugProcessEnhancedStep90Vtbl
    {
        BEGIN_INTERFACE
        
        HRESULT ( STDMETHODCALLTYPE *QueryInterface )( 
            IDebugProcessEnhancedStep90 * This,
            /* [in] */ __RPC__in REFIID riid,
            /* [iid_is][out] */ 
            __RPC__deref_out  void **ppvObject);
        
        ULONG ( STDMETHODCALLTYPE *AddRef )( 
            IDebugProcessEnhancedStep90 * This);
        
        ULONG ( STDMETHODCALLTYPE *Release )( 
            IDebugProcessEnhancedStep90 * This);
        
        HRESULT ( STDMETHODCALLTYPE *Step )( 
            IDebugProcessEnhancedStep90 * This,
            __RPC__in_opt IDebugThread2 *pThread,
            __RPC__in_opt IDebugStepper90 *pStepper);
        
        END_INTERFACE
    } IDebugProcessEnhancedStep90Vtbl;

    interface IDebugProcessEnhancedStep90
    {
        CONST_VTBL struct IDebugProcessEnhancedStep90Vtbl *lpVtbl;
    };

    

#ifdef COBJMACROS


#define IDebugProcessEnhancedStep90_QueryInterface(This,riid,ppvObject)	\
    ( (This)->lpVtbl -> QueryInterface(This,riid,ppvObject) ) 

#define IDebugProcessEnhancedStep90_AddRef(This)	\
    ( (This)->lpVtbl -> AddRef(This) ) 

#define IDebugProcessEnhancedStep90_Release(This)	\
    ( (This)->lpVtbl -> Release(This) ) 


#define IDebugProcessEnhancedStep90_Step(This,pThread,pStepper)	\
    ( (This)->lpVtbl -> Step(This,pThread,pStepper) ) 

#endif /* COBJMACROS */


#endif 	/* C style interface */




#endif 	/* __IDebugProcessEnhancedStep90_INTERFACE_DEFINED__ */


#ifndef __IDebugEngineStepFilterManager90_INTERFACE_DEFINED__
#define __IDebugEngineStepFilterManager90_INTERFACE_DEFINED__

/* interface IDebugEngineStepFilterManager90 */
/* [unique][uuid][object] */ 


EXTERN_C const IID IID_IDebugEngineStepFilterManager90;

#if defined(__cplusplus) && !defined(CINTERFACE)
    
    MIDL_INTERFACE("de74abe1-c625-4768-81b3-8eddab72ca20")
    IDebugEngineStepFilterManager90 : public IUnknown
    {
    public:
        virtual HRESULT STDMETHODCALLTYPE ReceiveStepFilterSettings( 
            __RPC__in BSTR stepFilterSettingsFile) = 0;
        
    };
    
#else 	/* C style interface */

    typedef struct IDebugEngineStepFilterManager90Vtbl
    {
        BEGIN_INTERFACE
        
        HRESULT ( STDMETHODCALLTYPE *QueryInterface )( 
            IDebugEngineStepFilterManager90 * This,
            /* [in] */ __RPC__in REFIID riid,
            /* [iid_is][out] */ 
            __RPC__deref_out  void **ppvObject);
        
        ULONG ( STDMETHODCALLTYPE *AddRef )( 
            IDebugEngineStepFilterManager90 * This);
        
        ULONG ( STDMETHODCALLTYPE *Release )( 
            IDebugEngineStepFilterManager90 * This);
        
        HRESULT ( STDMETHODCALLTYPE *ReceiveStepFilterSettings )( 
            IDebugEngineStepFilterManager90 * This,
            __RPC__in BSTR stepFilterSettingsFile);
        
        END_INTERFACE
    } IDebugEngineStepFilterManager90Vtbl;

    interface IDebugEngineStepFilterManager90
    {
        CONST_VTBL struct IDebugEngineStepFilterManager90Vtbl *lpVtbl;
    };

    

#ifdef COBJMACROS


#define IDebugEngineStepFilterManager90_QueryInterface(This,riid,ppvObject)	\
    ( (This)->lpVtbl -> QueryInterface(This,riid,ppvObject) ) 

#define IDebugEngineStepFilterManager90_AddRef(This)	\
    ( (This)->lpVtbl -> AddRef(This) ) 

#define IDebugEngineStepFilterManager90_Release(This)	\
    ( (This)->lpVtbl -> Release(This) ) 


#define IDebugEngineStepFilterManager90_ReceiveStepFilterSettings(This,stepFilterSettingsFile)	\
    ( (This)->lpVtbl -> ReceiveStepFilterSettings(This,stepFilterSettingsFile) ) 

#endif /* COBJMACROS */


#endif 	/* C style interface */




#endif 	/* __IDebugEngineStepFilterManager90_INTERFACE_DEFINED__ */


#ifndef __IDebugStepCompleteEvent90_INTERFACE_DEFINED__
#define __IDebugStepCompleteEvent90_INTERFACE_DEFINED__

/* interface IDebugStepCompleteEvent90 */
/* [unique][uuid][object] */ 


EXTERN_C const IID IID_IDebugStepCompleteEvent90;

#if defined(__cplusplus) && !defined(CINTERFACE)
    
    MIDL_INTERFACE("82d3f946-c2f8-495b-8b5f-9c30acdc6c81")
    IDebugStepCompleteEvent90 : public IUnknown
    {
    public:
        virtual HRESULT STDMETHODCALLTYPE GetStepStatus( 
            /* [out] */ __RPC__out STEPSTATUS *pStatus) = 0;
        
        virtual HRESULT STDMETHODCALLTYPE EnumBreakpoints( 
            /* [out] */ __RPC__deref_out_opt IEnumDebugBoundBreakpoints2 **ppEnum) = 0;
        
    };
    
#else 	/* C style interface */

    typedef struct IDebugStepCompleteEvent90Vtbl
    {
        BEGIN_INTERFACE
        
        HRESULT ( STDMETHODCALLTYPE *QueryInterface )( 
            IDebugStepCompleteEvent90 * This,
            /* [in] */ __RPC__in REFIID riid,
            /* [iid_is][out] */ 
            __RPC__deref_out  void **ppvObject);
        
        ULONG ( STDMETHODCALLTYPE *AddRef )( 
            IDebugStepCompleteEvent90 * This);
        
        ULONG ( STDMETHODCALLTYPE *Release )( 
            IDebugStepCompleteEvent90 * This);
        
        HRESULT ( STDMETHODCALLTYPE *GetStepStatus )( 
            IDebugStepCompleteEvent90 * This,
            /* [out] */ __RPC__out STEPSTATUS *pStatus);
        
        HRESULT ( STDMETHODCALLTYPE *EnumBreakpoints )( 
            IDebugStepCompleteEvent90 * This,
            /* [out] */ __RPC__deref_out_opt IEnumDebugBoundBreakpoints2 **ppEnum);
        
        END_INTERFACE
    } IDebugStepCompleteEvent90Vtbl;

    interface IDebugStepCompleteEvent90
    {
        CONST_VTBL struct IDebugStepCompleteEvent90Vtbl *lpVtbl;
    };

    

#ifdef COBJMACROS


#define IDebugStepCompleteEvent90_QueryInterface(This,riid,ppvObject)	\
    ( (This)->lpVtbl -> QueryInterface(This,riid,ppvObject) ) 

#define IDebugStepCompleteEvent90_AddRef(This)	\
    ( (This)->lpVtbl -> AddRef(This) ) 

#define IDebugStepCompleteEvent90_Release(This)	\
    ( (This)->lpVtbl -> Release(This) ) 


#define IDebugStepCompleteEvent90_GetStepStatus(This,pStatus)	\
    ( (This)->lpVtbl -> GetStepStatus(This,pStatus) ) 

#define IDebugStepCompleteEvent90_EnumBreakpoints(This,ppEnum)	\
    ( (This)->lpVtbl -> EnumBreakpoints(This,ppEnum) ) 

#endif /* COBJMACROS */


#endif 	/* C style interface */




#endif 	/* __IDebugStepCompleteEvent90_INTERFACE_DEFINED__ */


/* interface __MIDL_itf_msdbg90_0000_0009 */
/* [local] */ 


enum enum_MESSAGETYPE90
    {	MT_REASON_STEP_FILTER	= 0x300,
	MT_REASON_JMC_PROMPT	= 0x400,
	MT_REASON_STEP_FILTER_PROMPT	= 0x500
    } ;


extern RPC_IF_HANDLE __MIDL_itf_msdbg90_0000_0009_v0_0_c_ifspec;
extern RPC_IF_HANDLE __MIDL_itf_msdbg90_0000_0009_v0_0_s_ifspec;

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


