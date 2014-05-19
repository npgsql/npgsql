

/* this ALWAYS GENERATED file contains the definitions for the interfaces */


 /* File created by MIDL compiler version 7.00.0499 */
/* Compiler settings for IVsTrackProjectRetargeting.idl:
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

#ifndef COM_NO_WINDOWS_H
#include "windows.h"
#include "ole2.h"
#endif /*COM_NO_WINDOWS_H*/

#ifndef __IVsTrackProjectRetargeting_h__
#define __IVsTrackProjectRetargeting_h__

#if defined(_MSC_VER) && (_MSC_VER >= 1020)
#pragma once
#endif

/* Forward Declarations */ 

#ifndef __IVsSetTargetFrameworkWorkerCallback_FWD_DEFINED__
#define __IVsSetTargetFrameworkWorkerCallback_FWD_DEFINED__
typedef interface IVsSetTargetFrameworkWorkerCallback IVsSetTargetFrameworkWorkerCallback;
#endif 	/* __IVsSetTargetFrameworkWorkerCallback_FWD_DEFINED__ */


#ifndef __IVsTrackProjectRetargeting_FWD_DEFINED__
#define __IVsTrackProjectRetargeting_FWD_DEFINED__
typedef interface IVsTrackProjectRetargeting IVsTrackProjectRetargeting;
#endif 	/* __IVsTrackProjectRetargeting_FWD_DEFINED__ */


#ifndef __SVsTrackProjectRetargeting_FWD_DEFINED__
#define __SVsTrackProjectRetargeting_FWD_DEFINED__
typedef interface SVsTrackProjectRetargeting SVsTrackProjectRetargeting;
#endif 	/* __SVsTrackProjectRetargeting_FWD_DEFINED__ */


/* header files for imported files */
#include "oaidl.h"
#include "IVsTrackProjectRetargetingEvents.h"

#ifdef __cplusplus
extern "C"{
#endif 


/* interface __MIDL_itf_IVsTrackProjectRetargeting_0000_0000 */
/* [local] */ 

#pragma once


extern RPC_IF_HANDLE __MIDL_itf_IVsTrackProjectRetargeting_0000_0000_v0_0_c_ifspec;
extern RPC_IF_HANDLE __MIDL_itf_IVsTrackProjectRetargeting_0000_0000_v0_0_s_ifspec;

#ifndef __IVsSetTargetFrameworkWorkerCallback_INTERFACE_DEFINED__
#define __IVsSetTargetFrameworkWorkerCallback_INTERFACE_DEFINED__

/* interface IVsSetTargetFrameworkWorkerCallback */
/* [object][uuid] */ 


EXTERN_C const IID IID_IVsSetTargetFrameworkWorkerCallback;

#if defined(__cplusplus) && !defined(CINTERFACE)
    
    MIDL_INTERFACE("52B20422-91B2-4fb9-97FE-90D6FA334741")
    IVsSetTargetFrameworkWorkerCallback : public IUnknown
    {
    public:
        virtual HRESULT STDMETHODCALLTYPE UpdateTargetFramework( 
            /* [in] */ __RPC__in_opt IVsHierarchy *pHier,
            /* [in] */ __RPC__in LPCWSTR currentTargetFramework,
            /* [in] */ __RPC__in LPCWSTR newTargetFramework) = 0;
        
    };
    
#else 	/* C style interface */

    typedef struct IVsSetTargetFrameworkWorkerCallbackVtbl
    {
        BEGIN_INTERFACE
        
        HRESULT ( STDMETHODCALLTYPE *QueryInterface )( 
            IVsSetTargetFrameworkWorkerCallback * This,
            /* [in] */ __RPC__in REFIID riid,
            /* [iid_is][out] */ 
            __RPC__deref_out  void **ppvObject);
        
        ULONG ( STDMETHODCALLTYPE *AddRef )( 
            IVsSetTargetFrameworkWorkerCallback * This);
        
        ULONG ( STDMETHODCALLTYPE *Release )( 
            IVsSetTargetFrameworkWorkerCallback * This);
        
        HRESULT ( STDMETHODCALLTYPE *UpdateTargetFramework )( 
            IVsSetTargetFrameworkWorkerCallback * This,
            /* [in] */ __RPC__in_opt IVsHierarchy *pHier,
            /* [in] */ __RPC__in LPCWSTR currentTargetFramework,
            /* [in] */ __RPC__in LPCWSTR newTargetFramework);
        
        END_INTERFACE
    } IVsSetTargetFrameworkWorkerCallbackVtbl;

    interface IVsSetTargetFrameworkWorkerCallback
    {
        CONST_VTBL struct IVsSetTargetFrameworkWorkerCallbackVtbl *lpVtbl;
    };

    

#ifdef COBJMACROS


#define IVsSetTargetFrameworkWorkerCallback_QueryInterface(This,riid,ppvObject)	\
    ( (This)->lpVtbl -> QueryInterface(This,riid,ppvObject) ) 

#define IVsSetTargetFrameworkWorkerCallback_AddRef(This)	\
    ( (This)->lpVtbl -> AddRef(This) ) 

#define IVsSetTargetFrameworkWorkerCallback_Release(This)	\
    ( (This)->lpVtbl -> Release(This) ) 


#define IVsSetTargetFrameworkWorkerCallback_UpdateTargetFramework(This,pHier,currentTargetFramework,newTargetFramework)	\
    ( (This)->lpVtbl -> UpdateTargetFramework(This,pHier,currentTargetFramework,newTargetFramework) ) 

#endif /* COBJMACROS */


#endif 	/* C style interface */




#endif 	/* __IVsSetTargetFrameworkWorkerCallback_INTERFACE_DEFINED__ */


#ifndef __IVsTrackProjectRetargeting_INTERFACE_DEFINED__
#define __IVsTrackProjectRetargeting_INTERFACE_DEFINED__

/* interface IVsTrackProjectRetargeting */
/* [object][uuid] */ 


EXTERN_C const IID IID_IVsTrackProjectRetargeting;

#if defined(__cplusplus) && !defined(CINTERFACE)
    
    MIDL_INTERFACE("D991BC9B-9C68-447f-A3A7-95962AD75DD2")
    IVsTrackProjectRetargeting : public IUnknown
    {
    public:
        virtual HRESULT STDMETHODCALLTYPE OnSetTargetFramework( 
            /* [in] */ __RPC__in_opt IVsHierarchy *pHier,
            /* [in] */ __RPC__in LPCWSTR currentTargetFramework,
            /* [in] */ __RPC__in LPCWSTR newTargetFramework,
            /* [in] */ __RPC__in_opt IVsSetTargetFrameworkWorkerCallback *pWorkerCallback,
            /* [in] */ VARIANT_BOOL reloadProject) = 0;
        
        virtual HRESULT STDMETHODCALLTYPE AdviseTrackProjectRetargetingEvents( 
            /* [in] */ __RPC__in_opt IVsTrackProjectRetargetingEvents *pEventSink,
            /* [out] */ __RPC__out VSCOOKIE *pdwCookie) = 0;
        
        virtual HRESULT STDMETHODCALLTYPE UnadviseTrackProjectRetargetingEvents( 
            /* [in] */ VSCOOKIE dwCookie) = 0;
        
        virtual HRESULT STDMETHODCALLTYPE AdviseTrackBatchRetargetingEvents( 
            /* [in] */ __RPC__in_opt IVsTrackBatchRetargetingEvents *pEventSink,
            /* [out] */ __RPC__out VSCOOKIE *pdwCookie) = 0;
        
        virtual HRESULT STDMETHODCALLTYPE UnadviseTrackBatchRetargetingEvents( 
            /* [in] */ VSCOOKIE dwCookie) = 0;
        
        virtual HRESULT STDMETHODCALLTYPE BeginRetargetingBatch( void) = 0;
        
        virtual HRESULT STDMETHODCALLTYPE BatchRetargetProject( 
            /* [in] */ __RPC__in_opt IVsHierarchy *pHier,
            /* [in] */ __RPC__in LPCWSTR newTargetFramework,
            /* [in] */ VARIANT_BOOL unloadProjectIfErrorOrCancel) = 0;
        
        virtual HRESULT STDMETHODCALLTYPE EndRetargetingBatch( void) = 0;
        
    };
    
#else 	/* C style interface */

    typedef struct IVsTrackProjectRetargetingVtbl
    {
        BEGIN_INTERFACE
        
        HRESULT ( STDMETHODCALLTYPE *QueryInterface )( 
            IVsTrackProjectRetargeting * This,
            /* [in] */ __RPC__in REFIID riid,
            /* [iid_is][out] */ 
            __RPC__deref_out  void **ppvObject);
        
        ULONG ( STDMETHODCALLTYPE *AddRef )( 
            IVsTrackProjectRetargeting * This);
        
        ULONG ( STDMETHODCALLTYPE *Release )( 
            IVsTrackProjectRetargeting * This);
        
        HRESULT ( STDMETHODCALLTYPE *OnSetTargetFramework )( 
            IVsTrackProjectRetargeting * This,
            /* [in] */ __RPC__in_opt IVsHierarchy *pHier,
            /* [in] */ __RPC__in LPCWSTR currentTargetFramework,
            /* [in] */ __RPC__in LPCWSTR newTargetFramework,
            /* [in] */ __RPC__in_opt IVsSetTargetFrameworkWorkerCallback *pWorkerCallback,
            /* [in] */ VARIANT_BOOL reloadProject);
        
        HRESULT ( STDMETHODCALLTYPE *AdviseTrackProjectRetargetingEvents )( 
            IVsTrackProjectRetargeting * This,
            /* [in] */ __RPC__in_opt IVsTrackProjectRetargetingEvents *pEventSink,
            /* [out] */ __RPC__out VSCOOKIE *pdwCookie);
        
        HRESULT ( STDMETHODCALLTYPE *UnadviseTrackProjectRetargetingEvents )( 
            IVsTrackProjectRetargeting * This,
            /* [in] */ VSCOOKIE dwCookie);
        
        HRESULT ( STDMETHODCALLTYPE *AdviseTrackBatchRetargetingEvents )( 
            IVsTrackProjectRetargeting * This,
            /* [in] */ __RPC__in_opt IVsTrackBatchRetargetingEvents *pEventSink,
            /* [out] */ __RPC__out VSCOOKIE *pdwCookie);
        
        HRESULT ( STDMETHODCALLTYPE *UnadviseTrackBatchRetargetingEvents )( 
            IVsTrackProjectRetargeting * This,
            /* [in] */ VSCOOKIE dwCookie);
        
        HRESULT ( STDMETHODCALLTYPE *BeginRetargetingBatch )( 
            IVsTrackProjectRetargeting * This);
        
        HRESULT ( STDMETHODCALLTYPE *BatchRetargetProject )( 
            IVsTrackProjectRetargeting * This,
            /* [in] */ __RPC__in_opt IVsHierarchy *pHier,
            /* [in] */ __RPC__in LPCWSTR newTargetFramework,
            /* [in] */ VARIANT_BOOL unloadProjectIfErrorOrCancel);
        
        HRESULT ( STDMETHODCALLTYPE *EndRetargetingBatch )( 
            IVsTrackProjectRetargeting * This);
        
        END_INTERFACE
    } IVsTrackProjectRetargetingVtbl;

    interface IVsTrackProjectRetargeting
    {
        CONST_VTBL struct IVsTrackProjectRetargetingVtbl *lpVtbl;
    };

    

#ifdef COBJMACROS


#define IVsTrackProjectRetargeting_QueryInterface(This,riid,ppvObject)	\
    ( (This)->lpVtbl -> QueryInterface(This,riid,ppvObject) ) 

#define IVsTrackProjectRetargeting_AddRef(This)	\
    ( (This)->lpVtbl -> AddRef(This) ) 

#define IVsTrackProjectRetargeting_Release(This)	\
    ( (This)->lpVtbl -> Release(This) ) 


#define IVsTrackProjectRetargeting_OnSetTargetFramework(This,pHier,currentTargetFramework,newTargetFramework,pWorkerCallback,reloadProject)	\
    ( (This)->lpVtbl -> OnSetTargetFramework(This,pHier,currentTargetFramework,newTargetFramework,pWorkerCallback,reloadProject) ) 

#define IVsTrackProjectRetargeting_AdviseTrackProjectRetargetingEvents(This,pEventSink,pdwCookie)	\
    ( (This)->lpVtbl -> AdviseTrackProjectRetargetingEvents(This,pEventSink,pdwCookie) ) 

#define IVsTrackProjectRetargeting_UnadviseTrackProjectRetargetingEvents(This,dwCookie)	\
    ( (This)->lpVtbl -> UnadviseTrackProjectRetargetingEvents(This,dwCookie) ) 

#define IVsTrackProjectRetargeting_AdviseTrackBatchRetargetingEvents(This,pEventSink,pdwCookie)	\
    ( (This)->lpVtbl -> AdviseTrackBatchRetargetingEvents(This,pEventSink,pdwCookie) ) 

#define IVsTrackProjectRetargeting_UnadviseTrackBatchRetargetingEvents(This,dwCookie)	\
    ( (This)->lpVtbl -> UnadviseTrackBatchRetargetingEvents(This,dwCookie) ) 

#define IVsTrackProjectRetargeting_BeginRetargetingBatch(This)	\
    ( (This)->lpVtbl -> BeginRetargetingBatch(This) ) 

#define IVsTrackProjectRetargeting_BatchRetargetProject(This,pHier,newTargetFramework,unloadProjectIfErrorOrCancel)	\
    ( (This)->lpVtbl -> BatchRetargetProject(This,pHier,newTargetFramework,unloadProjectIfErrorOrCancel) ) 

#define IVsTrackProjectRetargeting_EndRetargetingBatch(This)	\
    ( (This)->lpVtbl -> EndRetargetingBatch(This) ) 

#endif /* COBJMACROS */


#endif 	/* C style interface */




#endif 	/* __IVsTrackProjectRetargeting_INTERFACE_DEFINED__ */


#ifndef __SVsTrackProjectRetargeting_INTERFACE_DEFINED__
#define __SVsTrackProjectRetargeting_INTERFACE_DEFINED__

/* interface SVsTrackProjectRetargeting */
/* [object][uuid] */ 


EXTERN_C const IID IID_SVsTrackProjectRetargeting;

#if defined(__cplusplus) && !defined(CINTERFACE)
    
    MIDL_INTERFACE("530462A8-1ECB-4f7f-979E-A0237FEA12C3")
    SVsTrackProjectRetargeting : public IUnknown
    {
    public:
    };
    
#else 	/* C style interface */

    typedef struct SVsTrackProjectRetargetingVtbl
    {
        BEGIN_INTERFACE
        
        HRESULT ( STDMETHODCALLTYPE *QueryInterface )( 
            SVsTrackProjectRetargeting * This,
            /* [in] */ __RPC__in REFIID riid,
            /* [iid_is][out] */ 
            __RPC__deref_out  void **ppvObject);
        
        ULONG ( STDMETHODCALLTYPE *AddRef )( 
            SVsTrackProjectRetargeting * This);
        
        ULONG ( STDMETHODCALLTYPE *Release )( 
            SVsTrackProjectRetargeting * This);
        
        END_INTERFACE
    } SVsTrackProjectRetargetingVtbl;

    interface SVsTrackProjectRetargeting
    {
        CONST_VTBL struct SVsTrackProjectRetargetingVtbl *lpVtbl;
    };

    

#ifdef COBJMACROS


#define SVsTrackProjectRetargeting_QueryInterface(This,riid,ppvObject)	\
    ( (This)->lpVtbl -> QueryInterface(This,riid,ppvObject) ) 

#define SVsTrackProjectRetargeting_AddRef(This)	\
    ( (This)->lpVtbl -> AddRef(This) ) 

#define SVsTrackProjectRetargeting_Release(This)	\
    ( (This)->lpVtbl -> Release(This) ) 


#endif /* COBJMACROS */


#endif 	/* C style interface */




#endif 	/* __SVsTrackProjectRetargeting_INTERFACE_DEFINED__ */


/* interface __MIDL_itf_IVsTrackProjectRetargeting_0000_0003 */
/* [local] */ 

#define SID_SVsTrackProjectRetargeting IID_SVsTrackProjectRetargeting


extern RPC_IF_HANDLE __MIDL_itf_IVsTrackProjectRetargeting_0000_0003_v0_0_c_ifspec;
extern RPC_IF_HANDLE __MIDL_itf_IVsTrackProjectRetargeting_0000_0003_v0_0_s_ifspec;

/* Additional Prototypes for ALL interfaces */

/* end of Additional Prototypes */

#ifdef __cplusplus
}
#endif

#endif


