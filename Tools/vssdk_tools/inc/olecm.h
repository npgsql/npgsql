

/* this ALWAYS GENERATED file contains the definitions for the interfaces */


 /* File created by MIDL compiler version 7.00.0499 */
/* Compiler settings for olecm.idl:
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

#ifndef __olecm_h__
#define __olecm_h__

#if defined(_MSC_VER) && (_MSC_VER >= 1020)
#pragma once
#endif

/* Forward Declarations */ 

#ifndef __IOleComponent_FWD_DEFINED__
#define __IOleComponent_FWD_DEFINED__
typedef interface IOleComponent IOleComponent;
#endif 	/* __IOleComponent_FWD_DEFINED__ */


#ifndef __IOleComponentManager_FWD_DEFINED__
#define __IOleComponentManager_FWD_DEFINED__
typedef interface IOleComponentManager IOleComponentManager;
#endif 	/* __IOleComponentManager_FWD_DEFINED__ */


/* header files for imported files */
#include "oleidl.h"
#include "servprov.h"
#include "oaidl.h"
#include "docobj.h"
#include "designer.h"
#include "textmgr.h"
#include "oleipc.h"

#ifdef __cplusplus
extern "C"{
#endif 


/* interface __MIDL_itf_olecm_0000_0000 */
/* [local] */ 


enum _OLECRF
    {	olecrfNeedIdleTime	= 1,
	olecrfNeedPeriodicIdleTime	= 2,
	olecrfPreTranslateKeys	= 4,
	olecrfPreTranslateAll	= 8,
	olecrfNeedSpecActiveNotifs	= 16,
	olecrfNeedAllActiveNotifs	= 32,
	olecrfExclusiveBorderSpace	= 64,
	olecrfExclusiveActivation	= 128
    } ;
typedef DWORD OLECRF;


enum _OLECADVF
    {	olecadvfModal	= 1,
	olecadvfRedrawOff	= 2,
	olecadvfWarningsOff	= 4,
	olecadvfRecording	= 8
    } ;
typedef DWORD OLECADVF;

typedef struct _OLECRINFO
    {
    ULONG cbSize;
    ULONG uIdleTimeInterval;
    OLECRF grfcrf;
    OLECADVF grfcadvf;
    } 	OLECRINFO;


enum _OLECHOSTF
    {	olechostfExclusiveBorderSpace	= 1
    } ;
typedef DWORD OLECHOSTF;

typedef struct _OLECHOSTINFO
    {
    ULONG cbSize;
    OLECHOSTF grfchostf;
    } 	OLECHOSTINFO;


enum _OLEIDLEF
    {	oleidlefPeriodic	= 1,
	oleidlefNonPeriodic	= 2,
	oleidlefPriority	= 4,
	oleidlefAll	= -1
    } ;
typedef DWORD OLEIDLEF;


enum _OLELOOP
    {	oleloopFocusWait	= 1,
	oleloopDoEvents	= 2,
	oleloopDebug	= 3,
	oleloopModalForm	= 4
    } ;
typedef ULONG OLELOOP;


enum _OLECSTATE
    {	olecstateModal	= 1,
	olecstateRedrawOff	= 2,
	olecstateWarningsOff	= 3,
	olecstateRecording	= 4
    } ;
typedef ULONG OLECSTATE;


enum _OLECCONTEXT
    {	oleccontextAll	= 0,
	oleccontextMine	= 1,
	oleccontextOthers	= 2
    } ;
typedef ULONG OLECCONTEXT;


enum _OLEGAC
    {	olegacActive	= 0,
	olegacTracking	= 1,
	olegacTrackingOrActive	= 2
    } ;
typedef DWORD OLEGAC;


enum _OLECWINDOW
    {	olecWindowFrameToplevel	= 0,
	olecWindowFrameOwner	= 1,
	olecWindowComponent	= 2,
	olecWindowDlgOwner	= 3
    } ;
typedef DWORD OLECWINDOW;



extern RPC_IF_HANDLE __MIDL_itf_olecm_0000_0000_v0_0_c_ifspec;
extern RPC_IF_HANDLE __MIDL_itf_olecm_0000_0000_v0_0_s_ifspec;

#ifndef __IOleComponent_INTERFACE_DEFINED__
#define __IOleComponent_INTERFACE_DEFINED__

/* interface IOleComponent */
/* [object][local][unique][version][uuid] */ 


EXTERN_C const IID IID_IOleComponent;

#if defined(__cplusplus) && !defined(CINTERFACE)
    
    MIDL_INTERFACE("000C0600-0000-0000-C000-000000000046")
    IOleComponent : public IUnknown
    {
    public:
        virtual BOOL STDMETHODCALLTYPE FReserved1( 
            /* [in] */ DWORD dwReserved,
            /* [in] */ UINT message,
            /* [in] */ WPARAM wParam,
            /* [in] */ LPARAM lParam) = 0;
        
        virtual BOOL STDMETHODCALLTYPE FPreTranslateMessage( 
            /* [out][in] */ MSG *pMsg) = 0;
        
        virtual void STDMETHODCALLTYPE OnEnterState( 
            /* [in] */ OLECSTATE uStateID,
            /* [in] */ BOOL fEnter) = 0;
        
        virtual void STDMETHODCALLTYPE OnAppActivate( 
            /* [in] */ BOOL fActive,
            /* [in] */ DWORD dwOtherThreadID) = 0;
        
        virtual void STDMETHODCALLTYPE OnLoseActivation( void) = 0;
        
        virtual void STDMETHODCALLTYPE OnActivationChange( 
            /* [in] */ IOleComponent *pic,
            /* [in] */ BOOL fSameComponent,
            /* [in] */ const OLECRINFO *pcrinfo,
            /* [in] */ BOOL fHostIsActivating,
            /* [in] */ const OLECHOSTINFO *pchostinfo,
            /* [in] */ DWORD dwReserved) = 0;
        
        virtual BOOL STDMETHODCALLTYPE FDoIdle( 
            /* [in] */ OLEIDLEF grfidlef) = 0;
        
        virtual BOOL STDMETHODCALLTYPE FContinueMessageLoop( 
            /* [in] */ OLELOOP uReason,
            /* [in] */ void *pvLoopData,
            /* [in] */ MSG *pMsgPeeked) = 0;
        
        virtual BOOL STDMETHODCALLTYPE FQueryTerminate( 
            /* [in] */ BOOL fPromptUser) = 0;
        
        virtual void STDMETHODCALLTYPE Terminate( void) = 0;
        
        virtual HWND STDMETHODCALLTYPE HwndGetWindow( 
            /* [in] */ OLECWINDOW dwWhich,
            /* [in] */ DWORD dwReserved) = 0;
        
    };
    
#else 	/* C style interface */

    typedef struct IOleComponentVtbl
    {
        BEGIN_INTERFACE
        
        HRESULT ( STDMETHODCALLTYPE *QueryInterface )( 
            IOleComponent * This,
            /* [in] */ REFIID riid,
            /* [iid_is][out] */ 
            __RPC__deref_out  void **ppvObject);
        
        ULONG ( STDMETHODCALLTYPE *AddRef )( 
            IOleComponent * This);
        
        ULONG ( STDMETHODCALLTYPE *Release )( 
            IOleComponent * This);
        
        BOOL ( STDMETHODCALLTYPE *FReserved1 )( 
            IOleComponent * This,
            /* [in] */ DWORD dwReserved,
            /* [in] */ UINT message,
            /* [in] */ WPARAM wParam,
            /* [in] */ LPARAM lParam);
        
        BOOL ( STDMETHODCALLTYPE *FPreTranslateMessage )( 
            IOleComponent * This,
            /* [out][in] */ MSG *pMsg);
        
        void ( STDMETHODCALLTYPE *OnEnterState )( 
            IOleComponent * This,
            /* [in] */ OLECSTATE uStateID,
            /* [in] */ BOOL fEnter);
        
        void ( STDMETHODCALLTYPE *OnAppActivate )( 
            IOleComponent * This,
            /* [in] */ BOOL fActive,
            /* [in] */ DWORD dwOtherThreadID);
        
        void ( STDMETHODCALLTYPE *OnLoseActivation )( 
            IOleComponent * This);
        
        void ( STDMETHODCALLTYPE *OnActivationChange )( 
            IOleComponent * This,
            /* [in] */ IOleComponent *pic,
            /* [in] */ BOOL fSameComponent,
            /* [in] */ const OLECRINFO *pcrinfo,
            /* [in] */ BOOL fHostIsActivating,
            /* [in] */ const OLECHOSTINFO *pchostinfo,
            /* [in] */ DWORD dwReserved);
        
        BOOL ( STDMETHODCALLTYPE *FDoIdle )( 
            IOleComponent * This,
            /* [in] */ OLEIDLEF grfidlef);
        
        BOOL ( STDMETHODCALLTYPE *FContinueMessageLoop )( 
            IOleComponent * This,
            /* [in] */ OLELOOP uReason,
            /* [in] */ void *pvLoopData,
            /* [in] */ MSG *pMsgPeeked);
        
        BOOL ( STDMETHODCALLTYPE *FQueryTerminate )( 
            IOleComponent * This,
            /* [in] */ BOOL fPromptUser);
        
        void ( STDMETHODCALLTYPE *Terminate )( 
            IOleComponent * This);
        
        HWND ( STDMETHODCALLTYPE *HwndGetWindow )( 
            IOleComponent * This,
            /* [in] */ OLECWINDOW dwWhich,
            /* [in] */ DWORD dwReserved);
        
        END_INTERFACE
    } IOleComponentVtbl;

    interface IOleComponent
    {
        CONST_VTBL struct IOleComponentVtbl *lpVtbl;
    };

    

#ifdef COBJMACROS


#define IOleComponent_QueryInterface(This,riid,ppvObject)	\
    ( (This)->lpVtbl -> QueryInterface(This,riid,ppvObject) ) 

#define IOleComponent_AddRef(This)	\
    ( (This)->lpVtbl -> AddRef(This) ) 

#define IOleComponent_Release(This)	\
    ( (This)->lpVtbl -> Release(This) ) 


#define IOleComponent_FReserved1(This,dwReserved,message,wParam,lParam)	\
    ( (This)->lpVtbl -> FReserved1(This,dwReserved,message,wParam,lParam) ) 

#define IOleComponent_FPreTranslateMessage(This,pMsg)	\
    ( (This)->lpVtbl -> FPreTranslateMessage(This,pMsg) ) 

#define IOleComponent_OnEnterState(This,uStateID,fEnter)	\
    ( (This)->lpVtbl -> OnEnterState(This,uStateID,fEnter) ) 

#define IOleComponent_OnAppActivate(This,fActive,dwOtherThreadID)	\
    ( (This)->lpVtbl -> OnAppActivate(This,fActive,dwOtherThreadID) ) 

#define IOleComponent_OnLoseActivation(This)	\
    ( (This)->lpVtbl -> OnLoseActivation(This) ) 

#define IOleComponent_OnActivationChange(This,pic,fSameComponent,pcrinfo,fHostIsActivating,pchostinfo,dwReserved)	\
    ( (This)->lpVtbl -> OnActivationChange(This,pic,fSameComponent,pcrinfo,fHostIsActivating,pchostinfo,dwReserved) ) 

#define IOleComponent_FDoIdle(This,grfidlef)	\
    ( (This)->lpVtbl -> FDoIdle(This,grfidlef) ) 

#define IOleComponent_FContinueMessageLoop(This,uReason,pvLoopData,pMsgPeeked)	\
    ( (This)->lpVtbl -> FContinueMessageLoop(This,uReason,pvLoopData,pMsgPeeked) ) 

#define IOleComponent_FQueryTerminate(This,fPromptUser)	\
    ( (This)->lpVtbl -> FQueryTerminate(This,fPromptUser) ) 

#define IOleComponent_Terminate(This)	\
    ( (This)->lpVtbl -> Terminate(This) ) 

#define IOleComponent_HwndGetWindow(This,dwWhich,dwReserved)	\
    ( (This)->lpVtbl -> HwndGetWindow(This,dwWhich,dwReserved) ) 

#endif /* COBJMACROS */


#endif 	/* C style interface */




#endif 	/* __IOleComponent_INTERFACE_DEFINED__ */


#ifndef __IOleComponentManager_INTERFACE_DEFINED__
#define __IOleComponentManager_INTERFACE_DEFINED__

/* interface IOleComponentManager */
/* [object][local][unique][version][uuid] */ 


EXTERN_C const IID IID_IOleComponentManager;

#if defined(__cplusplus) && !defined(CINTERFACE)
    
    MIDL_INTERFACE("000C0601-0000-0000-C000-000000000046")
    IOleComponentManager : public IUnknown
    {
    public:
        virtual HRESULT STDMETHODCALLTYPE QueryService( 
            /* [in] */ REFGUID guidService,
            /* [in] */ REFIID iid,
            /* [out] */ void **ppvObj) = 0;
        
        virtual BOOL STDMETHODCALLTYPE FReserved1( 
            /* [in] */ DWORD dwReserved,
            /* [in] */ UINT message,
            /* [in] */ WPARAM wParam,
            /* [in] */ LPARAM lParam) = 0;
        
        virtual BOOL STDMETHODCALLTYPE FRegisterComponent( 
            /* [in] */ IOleComponent *piComponent,
            /* [in] */ const OLECRINFO *pcrinfo,
            /* [out] */ DWORD_PTR *pdwComponentID) = 0;
        
        virtual BOOL STDMETHODCALLTYPE FRevokeComponent( 
            /* [in] */ DWORD_PTR dwComponentID) = 0;
        
        virtual BOOL STDMETHODCALLTYPE FUpdateComponentRegistration( 
            /* [in] */ DWORD_PTR dwComponentID,
            /* [in] */ const OLECRINFO *pcrinfo) = 0;
        
        virtual BOOL STDMETHODCALLTYPE FOnComponentActivate( 
            /* [in] */ DWORD_PTR dwComponentID) = 0;
        
        virtual BOOL STDMETHODCALLTYPE FSetTrackingComponent( 
            /* [in] */ DWORD_PTR dwComponentID,
            /* [in] */ BOOL fTrack) = 0;
        
        virtual void STDMETHODCALLTYPE OnComponentEnterState( 
            /* [in] */ DWORD_PTR dwComponentID,
            /* [in] */ OLECSTATE uStateID,
            /* [in] */ OLECCONTEXT uContext,
            /* [in] */ ULONG cpicmExclude,
            /* [in] */ IOleComponentManager **rgpicmExclude,
            /* [in] */ DWORD dwReserved) = 0;
        
        virtual BOOL STDMETHODCALLTYPE FOnComponentExitState( 
            /* [in] */ DWORD_PTR dwComponentID,
            /* [in] */ OLECSTATE uStateID,
            /* [in] */ OLECCONTEXT uContext,
            /* [in] */ ULONG cpicmExclude,
            /* [in] */ IOleComponentManager **rgpicmExclude) = 0;
        
        virtual BOOL STDMETHODCALLTYPE FInState( 
            /* [in] */ OLECSTATE uStateID,
            /* [in] */ void *pvoid) = 0;
        
        virtual BOOL STDMETHODCALLTYPE FContinueIdle( void) = 0;
        
        virtual BOOL STDMETHODCALLTYPE FPushMessageLoop( 
            /* [in] */ DWORD_PTR dwComponentID,
            /* [in] */ OLELOOP uReason,
            /* [in] */ void *pvLoopData) = 0;
        
        virtual BOOL STDMETHODCALLTYPE FCreateSubComponentManager( 
            /* [in] */ IUnknown *piunkOuter,
            /* [in] */ IUnknown *piunkServProv,
            /* [in] */ REFIID riid,
            /* [out] */ void **ppvObj) = 0;
        
        virtual BOOL STDMETHODCALLTYPE FGetParentComponentManager( 
            /* [out] */ IOleComponentManager **ppicm) = 0;
        
        virtual BOOL STDMETHODCALLTYPE FGetActiveComponent( 
            /* [in] */ OLEGAC dwgac,
            /* [out] */ IOleComponent **ppic,
            /* [out][in] */ OLECRINFO *pcrinfo,
            /* [in] */ DWORD dwReserved) = 0;
        
    };
    
#else 	/* C style interface */

    typedef struct IOleComponentManagerVtbl
    {
        BEGIN_INTERFACE
        
        HRESULT ( STDMETHODCALLTYPE *QueryInterface )( 
            IOleComponentManager * This,
            /* [in] */ REFIID riid,
            /* [iid_is][out] */ 
            __RPC__deref_out  void **ppvObject);
        
        ULONG ( STDMETHODCALLTYPE *AddRef )( 
            IOleComponentManager * This);
        
        ULONG ( STDMETHODCALLTYPE *Release )( 
            IOleComponentManager * This);
        
        HRESULT ( STDMETHODCALLTYPE *QueryService )( 
            IOleComponentManager * This,
            /* [in] */ REFGUID guidService,
            /* [in] */ REFIID iid,
            /* [out] */ void **ppvObj);
        
        BOOL ( STDMETHODCALLTYPE *FReserved1 )( 
            IOleComponentManager * This,
            /* [in] */ DWORD dwReserved,
            /* [in] */ UINT message,
            /* [in] */ WPARAM wParam,
            /* [in] */ LPARAM lParam);
        
        BOOL ( STDMETHODCALLTYPE *FRegisterComponent )( 
            IOleComponentManager * This,
            /* [in] */ IOleComponent *piComponent,
            /* [in] */ const OLECRINFO *pcrinfo,
            /* [out] */ DWORD_PTR *pdwComponentID);
        
        BOOL ( STDMETHODCALLTYPE *FRevokeComponent )( 
            IOleComponentManager * This,
            /* [in] */ DWORD_PTR dwComponentID);
        
        BOOL ( STDMETHODCALLTYPE *FUpdateComponentRegistration )( 
            IOleComponentManager * This,
            /* [in] */ DWORD_PTR dwComponentID,
            /* [in] */ const OLECRINFO *pcrinfo);
        
        BOOL ( STDMETHODCALLTYPE *FOnComponentActivate )( 
            IOleComponentManager * This,
            /* [in] */ DWORD_PTR dwComponentID);
        
        BOOL ( STDMETHODCALLTYPE *FSetTrackingComponent )( 
            IOleComponentManager * This,
            /* [in] */ DWORD_PTR dwComponentID,
            /* [in] */ BOOL fTrack);
        
        void ( STDMETHODCALLTYPE *OnComponentEnterState )( 
            IOleComponentManager * This,
            /* [in] */ DWORD_PTR dwComponentID,
            /* [in] */ OLECSTATE uStateID,
            /* [in] */ OLECCONTEXT uContext,
            /* [in] */ ULONG cpicmExclude,
            /* [in] */ IOleComponentManager **rgpicmExclude,
            /* [in] */ DWORD dwReserved);
        
        BOOL ( STDMETHODCALLTYPE *FOnComponentExitState )( 
            IOleComponentManager * This,
            /* [in] */ DWORD_PTR dwComponentID,
            /* [in] */ OLECSTATE uStateID,
            /* [in] */ OLECCONTEXT uContext,
            /* [in] */ ULONG cpicmExclude,
            /* [in] */ IOleComponentManager **rgpicmExclude);
        
        BOOL ( STDMETHODCALLTYPE *FInState )( 
            IOleComponentManager * This,
            /* [in] */ OLECSTATE uStateID,
            /* [in] */ void *pvoid);
        
        BOOL ( STDMETHODCALLTYPE *FContinueIdle )( 
            IOleComponentManager * This);
        
        BOOL ( STDMETHODCALLTYPE *FPushMessageLoop )( 
            IOleComponentManager * This,
            /* [in] */ DWORD_PTR dwComponentID,
            /* [in] */ OLELOOP uReason,
            /* [in] */ void *pvLoopData);
        
        BOOL ( STDMETHODCALLTYPE *FCreateSubComponentManager )( 
            IOleComponentManager * This,
            /* [in] */ IUnknown *piunkOuter,
            /* [in] */ IUnknown *piunkServProv,
            /* [in] */ REFIID riid,
            /* [out] */ void **ppvObj);
        
        BOOL ( STDMETHODCALLTYPE *FGetParentComponentManager )( 
            IOleComponentManager * This,
            /* [out] */ IOleComponentManager **ppicm);
        
        BOOL ( STDMETHODCALLTYPE *FGetActiveComponent )( 
            IOleComponentManager * This,
            /* [in] */ OLEGAC dwgac,
            /* [out] */ IOleComponent **ppic,
            /* [out][in] */ OLECRINFO *pcrinfo,
            /* [in] */ DWORD dwReserved);
        
        END_INTERFACE
    } IOleComponentManagerVtbl;

    interface IOleComponentManager
    {
        CONST_VTBL struct IOleComponentManagerVtbl *lpVtbl;
    };

    

#ifdef COBJMACROS


#define IOleComponentManager_QueryInterface(This,riid,ppvObject)	\
    ( (This)->lpVtbl -> QueryInterface(This,riid,ppvObject) ) 

#define IOleComponentManager_AddRef(This)	\
    ( (This)->lpVtbl -> AddRef(This) ) 

#define IOleComponentManager_Release(This)	\
    ( (This)->lpVtbl -> Release(This) ) 


#define IOleComponentManager_QueryService(This,guidService,iid,ppvObj)	\
    ( (This)->lpVtbl -> QueryService(This,guidService,iid,ppvObj) ) 

#define IOleComponentManager_FReserved1(This,dwReserved,message,wParam,lParam)	\
    ( (This)->lpVtbl -> FReserved1(This,dwReserved,message,wParam,lParam) ) 

#define IOleComponentManager_FRegisterComponent(This,piComponent,pcrinfo,pdwComponentID)	\
    ( (This)->lpVtbl -> FRegisterComponent(This,piComponent,pcrinfo,pdwComponentID) ) 

#define IOleComponentManager_FRevokeComponent(This,dwComponentID)	\
    ( (This)->lpVtbl -> FRevokeComponent(This,dwComponentID) ) 

#define IOleComponentManager_FUpdateComponentRegistration(This,dwComponentID,pcrinfo)	\
    ( (This)->lpVtbl -> FUpdateComponentRegistration(This,dwComponentID,pcrinfo) ) 

#define IOleComponentManager_FOnComponentActivate(This,dwComponentID)	\
    ( (This)->lpVtbl -> FOnComponentActivate(This,dwComponentID) ) 

#define IOleComponentManager_FSetTrackingComponent(This,dwComponentID,fTrack)	\
    ( (This)->lpVtbl -> FSetTrackingComponent(This,dwComponentID,fTrack) ) 

#define IOleComponentManager_OnComponentEnterState(This,dwComponentID,uStateID,uContext,cpicmExclude,rgpicmExclude,dwReserved)	\
    ( (This)->lpVtbl -> OnComponentEnterState(This,dwComponentID,uStateID,uContext,cpicmExclude,rgpicmExclude,dwReserved) ) 

#define IOleComponentManager_FOnComponentExitState(This,dwComponentID,uStateID,uContext,cpicmExclude,rgpicmExclude)	\
    ( (This)->lpVtbl -> FOnComponentExitState(This,dwComponentID,uStateID,uContext,cpicmExclude,rgpicmExclude) ) 

#define IOleComponentManager_FInState(This,uStateID,pvoid)	\
    ( (This)->lpVtbl -> FInState(This,uStateID,pvoid) ) 

#define IOleComponentManager_FContinueIdle(This)	\
    ( (This)->lpVtbl -> FContinueIdle(This) ) 

#define IOleComponentManager_FPushMessageLoop(This,dwComponentID,uReason,pvLoopData)	\
    ( (This)->lpVtbl -> FPushMessageLoop(This,dwComponentID,uReason,pvLoopData) ) 

#define IOleComponentManager_FCreateSubComponentManager(This,piunkOuter,piunkServProv,riid,ppvObj)	\
    ( (This)->lpVtbl -> FCreateSubComponentManager(This,piunkOuter,piunkServProv,riid,ppvObj) ) 

#define IOleComponentManager_FGetParentComponentManager(This,ppicm)	\
    ( (This)->lpVtbl -> FGetParentComponentManager(This,ppicm) ) 

#define IOleComponentManager_FGetActiveComponent(This,dwgac,ppic,pcrinfo,dwReserved)	\
    ( (This)->lpVtbl -> FGetActiveComponent(This,dwgac,ppic,pcrinfo,dwReserved) ) 

#endif /* COBJMACROS */


#endif 	/* C style interface */




#endif 	/* __IOleComponentManager_INTERFACE_DEFINED__ */


/* interface __MIDL_itf_olecm_0000_0002 */
/* [local] */ 

EXTERN_C const IID SID_SOleComponentManager;


extern RPC_IF_HANDLE __MIDL_itf_olecm_0000_0002_v0_0_c_ifspec;
extern RPC_IF_HANDLE __MIDL_itf_olecm_0000_0002_v0_0_s_ifspec;

/* Additional Prototypes for ALL interfaces */

/* end of Additional Prototypes */

#ifdef __cplusplus
}
#endif

#endif


