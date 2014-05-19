

/* this ALWAYS GENERATED file contains the definitions for the interfaces */


 /* File created by MIDL compiler version 7.00.0499 */
/* Compiler settings for IVsTrackProjectRetargetingEvents.idl:
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

#ifndef __IVsTrackProjectRetargetingEvents_h__
#define __IVsTrackProjectRetargetingEvents_h__

#if defined(_MSC_VER) && (_MSC_VER >= 1020)
#pragma once
#endif

/* Forward Declarations */ 

#ifndef __IVsTrackProjectRetargetingEvents_FWD_DEFINED__
#define __IVsTrackProjectRetargetingEvents_FWD_DEFINED__
typedef interface IVsTrackProjectRetargetingEvents IVsTrackProjectRetargetingEvents;
#endif 	/* __IVsTrackProjectRetargetingEvents_FWD_DEFINED__ */


#ifndef __IVsTrackBatchRetargetingEvents_FWD_DEFINED__
#define __IVsTrackBatchRetargetingEvents_FWD_DEFINED__
typedef interface IVsTrackBatchRetargetingEvents IVsTrackBatchRetargetingEvents;
#endif 	/* __IVsTrackBatchRetargetingEvents_FWD_DEFINED__ */


/* header files for imported files */
#include "oaidl.h"
#include "vsshell.h"

#ifdef __cplusplus
extern "C"{
#endif 


/* interface __MIDL_itf_IVsTrackProjectRetargetingEvents_0000_0000 */
/* [local] */ 

#pragma once


extern RPC_IF_HANDLE __MIDL_itf_IVsTrackProjectRetargetingEvents_0000_0000_v0_0_c_ifspec;
extern RPC_IF_HANDLE __MIDL_itf_IVsTrackProjectRetargetingEvents_0000_0000_v0_0_s_ifspec;

#ifndef __IVsTrackProjectRetargetingEvents_INTERFACE_DEFINED__
#define __IVsTrackProjectRetargetingEvents_INTERFACE_DEFINED__

/* interface IVsTrackProjectRetargetingEvents */
/* [object][uuid] */ 


EXTERN_C const IID IID_IVsTrackProjectRetargetingEvents;

#if defined(__cplusplus) && !defined(CINTERFACE)
    
    MIDL_INTERFACE("60E3F077-6867-4528-96C7-98DD5B541D85")
    IVsTrackProjectRetargetingEvents : public IUnknown
    {
    public:
        virtual HRESULT STDMETHODCALLTYPE OnRetargetingBeforeChange( 
            /* [in] */ __RPC__in LPCWSTR projRef,
            /* [in] */ __RPC__in_opt IVsHierarchy *pBeforeChangeHier,
            /* [in] */ __RPC__in LPCWSTR currentTargetFramework,
            /* [in] */ __RPC__in LPCWSTR newTargetFramework,
            /* [out] */ __RPC__out VARIANT_BOOL *pCanceled,
            /* [out] */ __RPC__deref_out_opt BSTR *ppReasonMsg) = 0;
        
        virtual HRESULT STDMETHODCALLTYPE OnRetargetingCanceledChange( 
            /* [in] */ __RPC__in LPCWSTR projRef,
            /* [in] */ __RPC__in_opt IVsHierarchy *pBeforeChangeHier,
            /* [in] */ __RPC__in LPCWSTR currentTargetFramework,
            /* [in] */ __RPC__in LPCWSTR newTargetFramework) = 0;
        
        virtual HRESULT STDMETHODCALLTYPE OnRetargetingBeforeProjectSave( 
            /* [in] */ __RPC__in LPCWSTR projRef,
            /* [in] */ __RPC__in_opt IVsHierarchy *pBeforeChangeHier,
            /* [in] */ __RPC__in LPCWSTR currentTargetFramework,
            /* [in] */ __RPC__in LPCWSTR newTargetFramework) = 0;
        
        virtual HRESULT STDMETHODCALLTYPE OnRetargetingAfterChange( 
            /* [in] */ __RPC__in LPCWSTR projRef,
            /* [in] */ __RPC__in_opt IVsHierarchy *pAfterChangeHier,
            /* [in] */ __RPC__in LPCWSTR fromTargetFramework,
            /* [in] */ __RPC__in LPCWSTR toTargetFramework) = 0;
        
        virtual HRESULT STDMETHODCALLTYPE OnRetargetingFailure( 
            /* [in] */ __RPC__in LPCWSTR projRef,
            /* [in] */ __RPC__in_opt IVsHierarchy *pHier,
            /* [in] */ __RPC__in LPCWSTR fromTargetFramework,
            /* [in] */ __RPC__in LPCWSTR toTargetFramework) = 0;
        
    };
    
#else 	/* C style interface */

    typedef struct IVsTrackProjectRetargetingEventsVtbl
    {
        BEGIN_INTERFACE
        
        HRESULT ( STDMETHODCALLTYPE *QueryInterface )( 
            IVsTrackProjectRetargetingEvents * This,
            /* [in] */ __RPC__in REFIID riid,
            /* [iid_is][out] */ 
            __RPC__deref_out  void **ppvObject);
        
        ULONG ( STDMETHODCALLTYPE *AddRef )( 
            IVsTrackProjectRetargetingEvents * This);
        
        ULONG ( STDMETHODCALLTYPE *Release )( 
            IVsTrackProjectRetargetingEvents * This);
        
        HRESULT ( STDMETHODCALLTYPE *OnRetargetingBeforeChange )( 
            IVsTrackProjectRetargetingEvents * This,
            /* [in] */ __RPC__in LPCWSTR projRef,
            /* [in] */ __RPC__in_opt IVsHierarchy *pBeforeChangeHier,
            /* [in] */ __RPC__in LPCWSTR currentTargetFramework,
            /* [in] */ __RPC__in LPCWSTR newTargetFramework,
            /* [out] */ __RPC__out VARIANT_BOOL *pCanceled,
            /* [out] */ __RPC__deref_out_opt BSTR *ppReasonMsg);
        
        HRESULT ( STDMETHODCALLTYPE *OnRetargetingCanceledChange )( 
            IVsTrackProjectRetargetingEvents * This,
            /* [in] */ __RPC__in LPCWSTR projRef,
            /* [in] */ __RPC__in_opt IVsHierarchy *pBeforeChangeHier,
            /* [in] */ __RPC__in LPCWSTR currentTargetFramework,
            /* [in] */ __RPC__in LPCWSTR newTargetFramework);
        
        HRESULT ( STDMETHODCALLTYPE *OnRetargetingBeforeProjectSave )( 
            IVsTrackProjectRetargetingEvents * This,
            /* [in] */ __RPC__in LPCWSTR projRef,
            /* [in] */ __RPC__in_opt IVsHierarchy *pBeforeChangeHier,
            /* [in] */ __RPC__in LPCWSTR currentTargetFramework,
            /* [in] */ __RPC__in LPCWSTR newTargetFramework);
        
        HRESULT ( STDMETHODCALLTYPE *OnRetargetingAfterChange )( 
            IVsTrackProjectRetargetingEvents * This,
            /* [in] */ __RPC__in LPCWSTR projRef,
            /* [in] */ __RPC__in_opt IVsHierarchy *pAfterChangeHier,
            /* [in] */ __RPC__in LPCWSTR fromTargetFramework,
            /* [in] */ __RPC__in LPCWSTR toTargetFramework);
        
        HRESULT ( STDMETHODCALLTYPE *OnRetargetingFailure )( 
            IVsTrackProjectRetargetingEvents * This,
            /* [in] */ __RPC__in LPCWSTR projRef,
            /* [in] */ __RPC__in_opt IVsHierarchy *pHier,
            /* [in] */ __RPC__in LPCWSTR fromTargetFramework,
            /* [in] */ __RPC__in LPCWSTR toTargetFramework);
        
        END_INTERFACE
    } IVsTrackProjectRetargetingEventsVtbl;

    interface IVsTrackProjectRetargetingEvents
    {
        CONST_VTBL struct IVsTrackProjectRetargetingEventsVtbl *lpVtbl;
    };

    

#ifdef COBJMACROS


#define IVsTrackProjectRetargetingEvents_QueryInterface(This,riid,ppvObject)	\
    ( (This)->lpVtbl -> QueryInterface(This,riid,ppvObject) ) 

#define IVsTrackProjectRetargetingEvents_AddRef(This)	\
    ( (This)->lpVtbl -> AddRef(This) ) 

#define IVsTrackProjectRetargetingEvents_Release(This)	\
    ( (This)->lpVtbl -> Release(This) ) 


#define IVsTrackProjectRetargetingEvents_OnRetargetingBeforeChange(This,projRef,pBeforeChangeHier,currentTargetFramework,newTargetFramework,pCanceled,ppReasonMsg)	\
    ( (This)->lpVtbl -> OnRetargetingBeforeChange(This,projRef,pBeforeChangeHier,currentTargetFramework,newTargetFramework,pCanceled,ppReasonMsg) ) 

#define IVsTrackProjectRetargetingEvents_OnRetargetingCanceledChange(This,projRef,pBeforeChangeHier,currentTargetFramework,newTargetFramework)	\
    ( (This)->lpVtbl -> OnRetargetingCanceledChange(This,projRef,pBeforeChangeHier,currentTargetFramework,newTargetFramework) ) 

#define IVsTrackProjectRetargetingEvents_OnRetargetingBeforeProjectSave(This,projRef,pBeforeChangeHier,currentTargetFramework,newTargetFramework)	\
    ( (This)->lpVtbl -> OnRetargetingBeforeProjectSave(This,projRef,pBeforeChangeHier,currentTargetFramework,newTargetFramework) ) 

#define IVsTrackProjectRetargetingEvents_OnRetargetingAfterChange(This,projRef,pAfterChangeHier,fromTargetFramework,toTargetFramework)	\
    ( (This)->lpVtbl -> OnRetargetingAfterChange(This,projRef,pAfterChangeHier,fromTargetFramework,toTargetFramework) ) 

#define IVsTrackProjectRetargetingEvents_OnRetargetingFailure(This,projRef,pHier,fromTargetFramework,toTargetFramework)	\
    ( (This)->lpVtbl -> OnRetargetingFailure(This,projRef,pHier,fromTargetFramework,toTargetFramework) ) 

#endif /* COBJMACROS */


#endif 	/* C style interface */




#endif 	/* __IVsTrackProjectRetargetingEvents_INTERFACE_DEFINED__ */


#ifndef __IVsTrackBatchRetargetingEvents_INTERFACE_DEFINED__
#define __IVsTrackBatchRetargetingEvents_INTERFACE_DEFINED__

/* interface IVsTrackBatchRetargetingEvents */
/* [object][uuid] */ 


EXTERN_C const IID IID_IVsTrackBatchRetargetingEvents;

#if defined(__cplusplus) && !defined(CINTERFACE)
    
    MIDL_INTERFACE("B8CB3252-2133-4869-9E34-DBCECD058081")
    IVsTrackBatchRetargetingEvents : public IUnknown
    {
    public:
        virtual HRESULT STDMETHODCALLTYPE OnBatchRetargetingBegin( void) = 0;
        
        virtual HRESULT STDMETHODCALLTYPE OnBatchRetargetingEnd( void) = 0;
        
    };
    
#else 	/* C style interface */

    typedef struct IVsTrackBatchRetargetingEventsVtbl
    {
        BEGIN_INTERFACE
        
        HRESULT ( STDMETHODCALLTYPE *QueryInterface )( 
            IVsTrackBatchRetargetingEvents * This,
            /* [in] */ __RPC__in REFIID riid,
            /* [iid_is][out] */ 
            __RPC__deref_out  void **ppvObject);
        
        ULONG ( STDMETHODCALLTYPE *AddRef )( 
            IVsTrackBatchRetargetingEvents * This);
        
        ULONG ( STDMETHODCALLTYPE *Release )( 
            IVsTrackBatchRetargetingEvents * This);
        
        HRESULT ( STDMETHODCALLTYPE *OnBatchRetargetingBegin )( 
            IVsTrackBatchRetargetingEvents * This);
        
        HRESULT ( STDMETHODCALLTYPE *OnBatchRetargetingEnd )( 
            IVsTrackBatchRetargetingEvents * This);
        
        END_INTERFACE
    } IVsTrackBatchRetargetingEventsVtbl;

    interface IVsTrackBatchRetargetingEvents
    {
        CONST_VTBL struct IVsTrackBatchRetargetingEventsVtbl *lpVtbl;
    };

    

#ifdef COBJMACROS


#define IVsTrackBatchRetargetingEvents_QueryInterface(This,riid,ppvObject)	\
    ( (This)->lpVtbl -> QueryInterface(This,riid,ppvObject) ) 

#define IVsTrackBatchRetargetingEvents_AddRef(This)	\
    ( (This)->lpVtbl -> AddRef(This) ) 

#define IVsTrackBatchRetargetingEvents_Release(This)	\
    ( (This)->lpVtbl -> Release(This) ) 


#define IVsTrackBatchRetargetingEvents_OnBatchRetargetingBegin(This)	\
    ( (This)->lpVtbl -> OnBatchRetargetingBegin(This) ) 

#define IVsTrackBatchRetargetingEvents_OnBatchRetargetingEnd(This)	\
    ( (This)->lpVtbl -> OnBatchRetargetingEnd(This) ) 

#endif /* COBJMACROS */


#endif 	/* C style interface */




#endif 	/* __IVsTrackBatchRetargetingEvents_INTERFACE_DEFINED__ */


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


