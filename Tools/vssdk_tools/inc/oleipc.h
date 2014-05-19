

/* this ALWAYS GENERATED file contains the definitions for the interfaces */


 /* File created by MIDL compiler version 7.00.0499 */
/* Compiler settings for oleipc.idl:
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

#ifndef __oleipc_h__
#define __oleipc_h__

#if defined(_MSC_VER) && (_MSC_VER >= 1020)
#pragma once
#endif

/* Forward Declarations */ 

#ifndef __IOleInPlaceComponent_FWD_DEFINED__
#define __IOleInPlaceComponent_FWD_DEFINED__
typedef interface IOleInPlaceComponent IOleInPlaceComponent;
#endif 	/* __IOleInPlaceComponent_FWD_DEFINED__ */


#ifndef __IOleInPlaceComponentSite_FWD_DEFINED__
#define __IOleInPlaceComponentSite_FWD_DEFINED__
typedef interface IOleInPlaceComponentSite IOleInPlaceComponentSite;
#endif 	/* __IOleInPlaceComponentSite_FWD_DEFINED__ */


#ifndef __IOleComponentUIManager_FWD_DEFINED__
#define __IOleComponentUIManager_FWD_DEFINED__
typedef interface IOleComponentUIManager IOleComponentUIManager;
#endif 	/* __IOleComponentUIManager_FWD_DEFINED__ */


#ifndef __IOleInPlaceComponentUIManager_FWD_DEFINED__
#define __IOleInPlaceComponentUIManager_FWD_DEFINED__
typedef interface IOleInPlaceComponentUIManager IOleInPlaceComponentUIManager;
#endif 	/* __IOleInPlaceComponentUIManager_FWD_DEFINED__ */


/* header files for imported files */
#include "oaidl.h"
#include "docobj.h"

#ifdef __cplusplus
extern "C"{
#endif 


/* interface __MIDL_itf_oleipc_0000_0000 */
/* [local] */ 





#define OLECMDSTATE_DISABLED						OLECMDF_SUPPORTED
#define OLECMDSTATE_UP			    (OLECMDF_SUPPORTED | OLECMDF_ENABLED)
#define OLECMDSTATE_DOWN	  (OLECMDF_SUPPORTED | OLECMDF_ENABLED | OLECMDF_LATCHED)
#define OLECMDSTATE_NINCHED 	  (OLECMDF_SUPPORTED | OLECMDF_ENABLED | OLECMDF_NINCHED)
#define OLECMDSTATE_INVISIBLE		  (OLECMDF_SUPPORTED | OLECMDF_INVISIBLE)
typedef 
enum tagOLEROLE
    {	OLEROLE_UNKNOWN	= -1,
	OLEROLE_COMPONENTHOST	= 0,
	OLEROLE_HOSTEXTENSION	= 0,
	OLEROLE_MAINCOMPONENT	= 1,
	OLEROLE_SUBCOMPONENT	= 2,
	OLEROLE_COMPONENTCONTROL	= 3,
	OLEROLE_TOPLEVELCOMPONENT	= 0
    } 	OLEROLE;

typedef 
enum tagOLECOMPFLAG
    {	OLECOMPFLAG_ROUTEACTIVEASCNTRCMD	= 1,
	OLECOMPFLAG_INHIBITNESTEDCOMPUI	= 2
    } 	OLECOMPFLAG;

typedef 
enum tagOLEMENU
    {	OLEMENU_MENUMERGE	= 1,
	OLEMENU_CNTRMENUONLY	= 2,
	OLEMENU_OBJECTMENUONLY	= 4,
	OLEMENU_ROUTEACTIVEASCNTRCMD	= 8
    } 	OLEMENU;

#define OLECONTEXT_NULLMENU 		-1
typedef 
enum tagOLEACTIVATE
    {	OLEACTIVATE_FRAMEWINDOW	= 1,
	OLEACTIVATE_DOCWINDOW	= 2
    } 	OLEACTIVATE;

typedef struct tagOLEMENUID
    {
    long nMenuId;
    ULONG cwBuf;
    LPOLESTR pwszNameBuf;
    } 	OLEMENUID;

#if 0
typedef struct tagPOINTS
    {
    SHORT x;
    SHORT y;
    } 	POINTS;

typedef struct tagPOINTS *PPOINTS;

typedef struct tagPOINTS *LPPOINTS;

typedef POINTS *REFPOINTS;

#endif
#if defined(__cplusplus)
#ifndef _REFPOINTS_DEFINED
#define _REFPOINTS_DEFINED
#define REFPOINTS POINTS &
#endif // !_REFPOINTS_DEFINED
#else // !__cplusplus
#ifndef _REFPOINTS_DEFINED
#define _REFPOINTS_DEFINED
#define REFPOINTS POINTS * const
#endif // !_REFPOINTS_DEFINED
#endif // !__cplusplus
typedef /* [public][public][public] */ 
enum __MIDL___MIDL_itf_oleipc_0000_0000_0001
    {	OLEMSGBUTTON_OK	= 0,
	OLEMSGBUTTON_OKCANCEL	= 1,
	OLEMSGBUTTON_ABORTRETRYIGNORE	= 2,
	OLEMSGBUTTON_YESNOCANCEL	= 3,
	OLEMSGBUTTON_YESNO	= 4,
	OLEMSGBUTTON_RETRYCANCEL	= 5,
	OLEMSGBUTTON_YESALLNOCANCEL	= 6
    } 	OLEMSGBUTTON;

typedef /* [public][public][public] */ 
enum __MIDL___MIDL_itf_oleipc_0000_0000_0002
    {	OLEMSGDEFBUTTON_FIRST	= 0,
	OLEMSGDEFBUTTON_SECOND	= 1,
	OLEMSGDEFBUTTON_THIRD	= 2,
	OLEMSGDEFBUTTON_FOURTH	= 3
    } 	OLEMSGDEFBUTTON;

typedef /* [public][public][public] */ 
enum __MIDL___MIDL_itf_oleipc_0000_0000_0003
    {	OLEMSGICON_NOICON	= 0,
	OLEMSGICON_CRITICAL	= 1,
	OLEMSGICON_QUERY	= 2,
	OLEMSGICON_WARNING	= 3,
	OLEMSGICON_INFO	= 4
    } 	OLEMSGICON;

typedef /* [public] */ 
enum __MIDL___MIDL_itf_oleipc_0000_0000_0004
    {	OLEHELPCMD_CONTEXT	= 0,
	OLEHELPCMD_CONTEXTPOPUP	= 1
    } 	OLEHELPCMD;

typedef 
enum tagOLEUIEVENTSTATUS
    {	OLEUIEVENTSTATUS_OCCURRED	= 0,
	OLEUIEVENTSTATUS_START	= 1,
	OLEUIEVENTSTATUS_STOP	= 2,
	OLEUIEVENTSTATUS_STARTNODIALOG	= 1,
	OLEUIEVENTSTATUS_STARTBEFOREDIALOG	= 3,
	OLEUIEVENTSTATUS_CONTINUEINDIALOG	= 4,
	OLEUIEVENTSTATUS_CONTINUEAFTERDIALOG	= 5
    } 	OLEUIEVENTSTATUS;

typedef 
enum tagOLEUIEVENTFREQ
    {	OLEUIEVENTFREQ_NULL	= 0,
	OLEUIEVENTFREQ_CONSISTENTLY	= 2,
	OLEUIEVENTFREQ_FREQUENTLY	= 4,
	OLEUIEVENTFREQ_OFTEN	= 6,
	OLEUIEVENTFREQ_SOMETIMES	= 8,
	OLEUIEVENTFREQ_SELDOM	= 10
    } 	OLEUIEVENTFREQ;

typedef 
enum tagOLEIPCSTATE
    {	OLEIPCSTATE_MODAL	= 1,
	OLEIPCSTATE_REDRAWOFF	= 2,
	OLEIPCSTATE_WARNINGSOFF	= 3,
	OLEIPCSTATE_RECORDING	= 4
    } 	OLEIPCSTATE;

#define OLEIPCOMPERR_E_FIRST			MAKE_SCODE(SEVERITY_ERROR, FACILITY_ITF, 0x200)
#define OLEIPCOMPERR_E_MUSTBEMAINCOMP				 (OLEIPCOMPERR_E_FIRST)


extern RPC_IF_HANDLE __MIDL_itf_oleipc_0000_0000_v0_0_c_ifspec;
extern RPC_IF_HANDLE __MIDL_itf_oleipc_0000_0000_v0_0_s_ifspec;

#ifndef __IOleInPlaceComponent_INTERFACE_DEFINED__
#define __IOleInPlaceComponent_INTERFACE_DEFINED__

/* interface IOleInPlaceComponent */
/* [object][version][uuid] */ 


EXTERN_C const IID IID_IOleInPlaceComponent;

#if defined(__cplusplus) && !defined(CINTERFACE)
    
    MIDL_INTERFACE("5efc7970-14bc-11cf-9b2b-00aa00573819")
    IOleInPlaceComponent : public IUnknown
    {
    public:
        virtual HRESULT STDMETHODCALLTYPE UseComponentUIManager( 
            /* [in] */ DWORD dwCompRole,
            /* [out] */ __RPC__out DWORD *pgrfCompFlags,
            /* [in] */ __RPC__in_opt IOleComponentUIManager *pCompUIMgr,
            /* [in] */ __RPC__in_opt IOleInPlaceComponentSite *pIPCompSite) = 0;
        
        virtual HRESULT STDMETHODCALLTYPE OnWindowActivate( 
            /* [in] */ DWORD dwWindowType,
            /* [in] */ BOOL fActivate) = 0;
        
        virtual /* [local] */ void STDMETHODCALLTYPE OnEnterState( 
            /* [in] */ DWORD dwStateId,
            /* [in] */ BOOL fEnter) = 0;
        
        virtual /* [local] */ BOOL STDMETHODCALLTYPE FDoIdle( 
            /* [in] */ DWORD grfidlef) = 0;
        
        virtual /* [local] */ BOOL STDMETHODCALLTYPE FQueryClose( 
            /* [in] */ BOOL fPromptUser) = 0;
        
        virtual HRESULT STDMETHODCALLTYPE TranslateCntrAccelerator( 
            /* [in] */ __RPC__in MSG *pMsg) = 0;
        
        virtual HRESULT STDMETHODCALLTYPE GetCntrContextMenu( 
            /* [in] */ DWORD dwRoleActiveObj,
            /* [in] */ __RPC__in REFCLSID rclsidActiveObj,
            /* [in] */ LONG nMenuIdActiveObj,
            /* [in] */ __RPC__in REFPOINTS pos,
            /* [out] */ __RPC__out CLSID *pclsidCntr,
            /* [out] */ __RPC__out OLEMENUID *menuid,
            /* [out] */ __RPC__out DWORD *pgrf) = 0;
        
        virtual HRESULT STDMETHODCALLTYPE GetCntrHelp( 
            /* [out][in] */ __RPC__inout DWORD *pdwRole,
            /* [out][in] */ __RPC__inout CLSID *pclsid,
            /* [in] */ POINT posMouse,
            /* [in] */ DWORD dwHelpCmd,
            /* [in] */ __RPC__in LPOLESTR pszHelpFileIn,
            /* [out] */ __RPC__deref_out_opt LPOLESTR *ppszHelpFileOut,
            /* [in] */ DWORD dwDataIn,
            /* [out] */ __RPC__out DWORD *pdwDataOut) = 0;
        
        virtual HRESULT STDMETHODCALLTYPE GetCntrMessage( 
            /* [out][in] */ __RPC__inout DWORD *pdwRole,
            /* [out][in] */ __RPC__inout CLSID *pclsid,
            /* [in] */ __RPC__in LPOLESTR pszTitleIn,
            /* [in] */ __RPC__in LPOLESTR pszTextIn,
            /* [in] */ __RPC__in LPOLESTR pszHelpFileIn,
            /* [out] */ __RPC__deref_out_opt LPOLESTR *ppszTitleOut,
            /* [out] */ __RPC__deref_out_opt LPOLESTR *ppszTextOut,
            /* [out] */ __RPC__deref_out_opt LPOLESTR *ppszHelpFileOut,
            /* [out][in] */ __RPC__inout DWORD *pdwHelpContextID,
            /* [out][in] */ __RPC__inout OLEMSGBUTTON *pmsgbtn,
            /* [out][in] */ __RPC__inout OLEMSGDEFBUTTON *pmsgdefbtn,
            /* [out][in] */ __RPC__inout OLEMSGICON *pmsgicon,
            /* [out][in] */ __RPC__inout BOOL *pfSysAlert) = 0;
        
    };
    
#else 	/* C style interface */

    typedef struct IOleInPlaceComponentVtbl
    {
        BEGIN_INTERFACE
        
        HRESULT ( STDMETHODCALLTYPE *QueryInterface )( 
            IOleInPlaceComponent * This,
            /* [in] */ __RPC__in REFIID riid,
            /* [iid_is][out] */ 
            __RPC__deref_out  void **ppvObject);
        
        ULONG ( STDMETHODCALLTYPE *AddRef )( 
            IOleInPlaceComponent * This);
        
        ULONG ( STDMETHODCALLTYPE *Release )( 
            IOleInPlaceComponent * This);
        
        HRESULT ( STDMETHODCALLTYPE *UseComponentUIManager )( 
            IOleInPlaceComponent * This,
            /* [in] */ DWORD dwCompRole,
            /* [out] */ __RPC__out DWORD *pgrfCompFlags,
            /* [in] */ __RPC__in_opt IOleComponentUIManager *pCompUIMgr,
            /* [in] */ __RPC__in_opt IOleInPlaceComponentSite *pIPCompSite);
        
        HRESULT ( STDMETHODCALLTYPE *OnWindowActivate )( 
            IOleInPlaceComponent * This,
            /* [in] */ DWORD dwWindowType,
            /* [in] */ BOOL fActivate);
        
        /* [local] */ void ( STDMETHODCALLTYPE *OnEnterState )( 
            IOleInPlaceComponent * This,
            /* [in] */ DWORD dwStateId,
            /* [in] */ BOOL fEnter);
        
        /* [local] */ BOOL ( STDMETHODCALLTYPE *FDoIdle )( 
            IOleInPlaceComponent * This,
            /* [in] */ DWORD grfidlef);
        
        /* [local] */ BOOL ( STDMETHODCALLTYPE *FQueryClose )( 
            IOleInPlaceComponent * This,
            /* [in] */ BOOL fPromptUser);
        
        HRESULT ( STDMETHODCALLTYPE *TranslateCntrAccelerator )( 
            IOleInPlaceComponent * This,
            /* [in] */ __RPC__in MSG *pMsg);
        
        HRESULT ( STDMETHODCALLTYPE *GetCntrContextMenu )( 
            IOleInPlaceComponent * This,
            /* [in] */ DWORD dwRoleActiveObj,
            /* [in] */ __RPC__in REFCLSID rclsidActiveObj,
            /* [in] */ LONG nMenuIdActiveObj,
            /* [in] */ __RPC__in REFPOINTS pos,
            /* [out] */ __RPC__out CLSID *pclsidCntr,
            /* [out] */ __RPC__out OLEMENUID *menuid,
            /* [out] */ __RPC__out DWORD *pgrf);
        
        HRESULT ( STDMETHODCALLTYPE *GetCntrHelp )( 
            IOleInPlaceComponent * This,
            /* [out][in] */ __RPC__inout DWORD *pdwRole,
            /* [out][in] */ __RPC__inout CLSID *pclsid,
            /* [in] */ POINT posMouse,
            /* [in] */ DWORD dwHelpCmd,
            /* [in] */ __RPC__in LPOLESTR pszHelpFileIn,
            /* [out] */ __RPC__deref_out_opt LPOLESTR *ppszHelpFileOut,
            /* [in] */ DWORD dwDataIn,
            /* [out] */ __RPC__out DWORD *pdwDataOut);
        
        HRESULT ( STDMETHODCALLTYPE *GetCntrMessage )( 
            IOleInPlaceComponent * This,
            /* [out][in] */ __RPC__inout DWORD *pdwRole,
            /* [out][in] */ __RPC__inout CLSID *pclsid,
            /* [in] */ __RPC__in LPOLESTR pszTitleIn,
            /* [in] */ __RPC__in LPOLESTR pszTextIn,
            /* [in] */ __RPC__in LPOLESTR pszHelpFileIn,
            /* [out] */ __RPC__deref_out_opt LPOLESTR *ppszTitleOut,
            /* [out] */ __RPC__deref_out_opt LPOLESTR *ppszTextOut,
            /* [out] */ __RPC__deref_out_opt LPOLESTR *ppszHelpFileOut,
            /* [out][in] */ __RPC__inout DWORD *pdwHelpContextID,
            /* [out][in] */ __RPC__inout OLEMSGBUTTON *pmsgbtn,
            /* [out][in] */ __RPC__inout OLEMSGDEFBUTTON *pmsgdefbtn,
            /* [out][in] */ __RPC__inout OLEMSGICON *pmsgicon,
            /* [out][in] */ __RPC__inout BOOL *pfSysAlert);
        
        END_INTERFACE
    } IOleInPlaceComponentVtbl;

    interface IOleInPlaceComponent
    {
        CONST_VTBL struct IOleInPlaceComponentVtbl *lpVtbl;
    };

    

#ifdef COBJMACROS


#define IOleInPlaceComponent_QueryInterface(This,riid,ppvObject)	\
    ( (This)->lpVtbl -> QueryInterface(This,riid,ppvObject) ) 

#define IOleInPlaceComponent_AddRef(This)	\
    ( (This)->lpVtbl -> AddRef(This) ) 

#define IOleInPlaceComponent_Release(This)	\
    ( (This)->lpVtbl -> Release(This) ) 


#define IOleInPlaceComponent_UseComponentUIManager(This,dwCompRole,pgrfCompFlags,pCompUIMgr,pIPCompSite)	\
    ( (This)->lpVtbl -> UseComponentUIManager(This,dwCompRole,pgrfCompFlags,pCompUIMgr,pIPCompSite) ) 

#define IOleInPlaceComponent_OnWindowActivate(This,dwWindowType,fActivate)	\
    ( (This)->lpVtbl -> OnWindowActivate(This,dwWindowType,fActivate) ) 

#define IOleInPlaceComponent_OnEnterState(This,dwStateId,fEnter)	\
    ( (This)->lpVtbl -> OnEnterState(This,dwStateId,fEnter) ) 

#define IOleInPlaceComponent_FDoIdle(This,grfidlef)	\
    ( (This)->lpVtbl -> FDoIdle(This,grfidlef) ) 

#define IOleInPlaceComponent_FQueryClose(This,fPromptUser)	\
    ( (This)->lpVtbl -> FQueryClose(This,fPromptUser) ) 

#define IOleInPlaceComponent_TranslateCntrAccelerator(This,pMsg)	\
    ( (This)->lpVtbl -> TranslateCntrAccelerator(This,pMsg) ) 

#define IOleInPlaceComponent_GetCntrContextMenu(This,dwRoleActiveObj,rclsidActiveObj,nMenuIdActiveObj,pos,pclsidCntr,menuid,pgrf)	\
    ( (This)->lpVtbl -> GetCntrContextMenu(This,dwRoleActiveObj,rclsidActiveObj,nMenuIdActiveObj,pos,pclsidCntr,menuid,pgrf) ) 

#define IOleInPlaceComponent_GetCntrHelp(This,pdwRole,pclsid,posMouse,dwHelpCmd,pszHelpFileIn,ppszHelpFileOut,dwDataIn,pdwDataOut)	\
    ( (This)->lpVtbl -> GetCntrHelp(This,pdwRole,pclsid,posMouse,dwHelpCmd,pszHelpFileIn,ppszHelpFileOut,dwDataIn,pdwDataOut) ) 

#define IOleInPlaceComponent_GetCntrMessage(This,pdwRole,pclsid,pszTitleIn,pszTextIn,pszHelpFileIn,ppszTitleOut,ppszTextOut,ppszHelpFileOut,pdwHelpContextID,pmsgbtn,pmsgdefbtn,pmsgicon,pfSysAlert)	\
    ( (This)->lpVtbl -> GetCntrMessage(This,pdwRole,pclsid,pszTitleIn,pszTextIn,pszHelpFileIn,ppszTitleOut,ppszTextOut,ppszHelpFileOut,pdwHelpContextID,pmsgbtn,pmsgdefbtn,pmsgicon,pfSysAlert) ) 

#endif /* COBJMACROS */


#endif 	/* C style interface */




#endif 	/* __IOleInPlaceComponent_INTERFACE_DEFINED__ */


/* interface __MIDL_itf_oleipc_0000_0001 */
/* [local] */ 

#define SID_SOleInPlaceComponent IID_IOleInPlaceComponent


extern RPC_IF_HANDLE __MIDL_itf_oleipc_0000_0001_v0_0_c_ifspec;
extern RPC_IF_HANDLE __MIDL_itf_oleipc_0000_0001_v0_0_s_ifspec;

#ifndef __IOleInPlaceComponentSite_INTERFACE_DEFINED__
#define __IOleInPlaceComponentSite_INTERFACE_DEFINED__

/* interface IOleInPlaceComponentSite */
/* [object][version][uuid] */ 


EXTERN_C const IID IID_IOleInPlaceComponentSite;

#if defined(__cplusplus) && !defined(CINTERFACE)
    
    MIDL_INTERFACE("5efc7971-14bc-11cf-9b2b-00aa00573819")
    IOleInPlaceComponentSite : public IUnknown
    {
    public:
        virtual HRESULT STDMETHODCALLTYPE SetUIMode( 
            /* [in] */ DWORD dwUIMode) = 0;
        
    };
    
#else 	/* C style interface */

    typedef struct IOleInPlaceComponentSiteVtbl
    {
        BEGIN_INTERFACE
        
        HRESULT ( STDMETHODCALLTYPE *QueryInterface )( 
            IOleInPlaceComponentSite * This,
            /* [in] */ __RPC__in REFIID riid,
            /* [iid_is][out] */ 
            __RPC__deref_out  void **ppvObject);
        
        ULONG ( STDMETHODCALLTYPE *AddRef )( 
            IOleInPlaceComponentSite * This);
        
        ULONG ( STDMETHODCALLTYPE *Release )( 
            IOleInPlaceComponentSite * This);
        
        HRESULT ( STDMETHODCALLTYPE *SetUIMode )( 
            IOleInPlaceComponentSite * This,
            /* [in] */ DWORD dwUIMode);
        
        END_INTERFACE
    } IOleInPlaceComponentSiteVtbl;

    interface IOleInPlaceComponentSite
    {
        CONST_VTBL struct IOleInPlaceComponentSiteVtbl *lpVtbl;
    };

    

#ifdef COBJMACROS


#define IOleInPlaceComponentSite_QueryInterface(This,riid,ppvObject)	\
    ( (This)->lpVtbl -> QueryInterface(This,riid,ppvObject) ) 

#define IOleInPlaceComponentSite_AddRef(This)	\
    ( (This)->lpVtbl -> AddRef(This) ) 

#define IOleInPlaceComponentSite_Release(This)	\
    ( (This)->lpVtbl -> Release(This) ) 


#define IOleInPlaceComponentSite_SetUIMode(This,dwUIMode)	\
    ( (This)->lpVtbl -> SetUIMode(This,dwUIMode) ) 

#endif /* COBJMACROS */


#endif 	/* C style interface */




#endif 	/* __IOleInPlaceComponentSite_INTERFACE_DEFINED__ */


/* interface __MIDL_itf_oleipc_0000_0002 */
/* [local] */ 

#define SID_SOleInPlaceComponentSite IID_IOleInPlaceComponentSite
#pragma warning(push)
#pragma warning(disable:28718)	


extern RPC_IF_HANDLE __MIDL_itf_oleipc_0000_0002_v0_0_c_ifspec;
extern RPC_IF_HANDLE __MIDL_itf_oleipc_0000_0002_v0_0_s_ifspec;

#ifndef __IOleComponentUIManager_INTERFACE_DEFINED__
#define __IOleComponentUIManager_INTERFACE_DEFINED__

/* interface IOleComponentUIManager */
/* [object][version][uuid] */ 


EXTERN_C const IID IID_IOleComponentUIManager;

#if defined(__cplusplus) && !defined(CINTERFACE)
    
    MIDL_INTERFACE("5efc7972-14bc-11cf-9b2b-00aa00573819")
    IOleComponentUIManager : public IUnknown
    {
    public:
        virtual HRESULT STDMETHODCALLTYPE Deleted1( void) = 0;
        
        virtual HRESULT STDMETHODCALLTYPE Deleted2( void) = 0;
        
        virtual /* [local] */ void STDMETHODCALLTYPE OnUIEvent( 
            /* [in] */ DWORD dwCompRole,
            /* [in] */ REFCLSID rclsidComp,
            /* [in] */ const GUID *pguidUIEventGroup,
            /* [in] */ DWORD nUIEventId,
            /* [in] */ DWORD dwUIEventStatus,
            /* [in] */ DWORD dwEventFreq,
            /* [in] */ RECT *prcEventRegion,
            /* [in] */ VARIANT *pvarEventArg) = 0;
        
        virtual HRESULT STDMETHODCALLTYPE OnUIEventProgress( 
            /* [out][in] */ __RPC__inout DWORD_PTR *pdwCookie,
            /* [in] */ BOOL fInProgress,
            /* [in] */ __RPC__in LPOLESTR pwszLabel,
            /* [in] */ ULONG nComplete,
            /* [in] */ ULONG nTotal) = 0;
        
        virtual HRESULT STDMETHODCALLTYPE SetStatus( 
            /* [in] */ __RPC__in LPCOLESTR pwszStatusText,
            /* [in] */ DWORD dwReserved) = 0;
        
        virtual HRESULT STDMETHODCALLTYPE ShowContextMenu( 
            /* [in] */ DWORD dwCompRole,
            /* [in] */ __RPC__in REFCLSID rclsidActive,
            /* [in] */ LONG nMenuId,
            /* [in] */ __RPC__in REFPOINTS pos,
            /* [in] */ __RPC__in_opt IOleCommandTarget *pCmdTrgtActive) = 0;
        
        virtual HRESULT STDMETHODCALLTYPE ShowHelp( 
            /* [in] */ DWORD dwCompRole,
            /* [in] */ __RPC__in REFCLSID rclsidComp,
            /* [in] */ POINT posMouse,
            /* [in] */ DWORD dwHelpCmd,
            /* [in] */ __RPC__in LPOLESTR pszHelpFile,
            /* [in] */ DWORD dwData) = 0;
        
        virtual HRESULT STDMETHODCALLTYPE ShowMessage( 
            /* [in] */ DWORD dwCompRole,
            /* [in] */ __RPC__in REFCLSID rclsidComp,
            /* [in] */ __RPC__in LPOLESTR pszTitle,
            /* [in] */ __RPC__in LPOLESTR pszText,
            /* [in] */ __RPC__in LPOLESTR pszHelpFile,
            /* [in] */ DWORD dwHelpContextID,
            /* [in] */ OLEMSGBUTTON msgbtn,
            /* [in] */ OLEMSGDEFBUTTON msgdefbtn,
            /* [in] */ OLEMSGICON msgicon,
            /* [in] */ BOOL fSysAlert,
            /* [retval][out] */ __RPC__out LONG *pnResult) = 0;
        
    };
    
#else 	/* C style interface */

    typedef struct IOleComponentUIManagerVtbl
    {
        BEGIN_INTERFACE
        
        HRESULT ( STDMETHODCALLTYPE *QueryInterface )( 
            IOleComponentUIManager * This,
            /* [in] */ __RPC__in REFIID riid,
            /* [iid_is][out] */ 
            __RPC__deref_out  void **ppvObject);
        
        ULONG ( STDMETHODCALLTYPE *AddRef )( 
            IOleComponentUIManager * This);
        
        ULONG ( STDMETHODCALLTYPE *Release )( 
            IOleComponentUIManager * This);
        
        HRESULT ( STDMETHODCALLTYPE *Deleted1 )( 
            IOleComponentUIManager * This);
        
        HRESULT ( STDMETHODCALLTYPE *Deleted2 )( 
            IOleComponentUIManager * This);
        
        /* [local] */ void ( STDMETHODCALLTYPE *OnUIEvent )( 
            IOleComponentUIManager * This,
            /* [in] */ DWORD dwCompRole,
            /* [in] */ REFCLSID rclsidComp,
            /* [in] */ const GUID *pguidUIEventGroup,
            /* [in] */ DWORD nUIEventId,
            /* [in] */ DWORD dwUIEventStatus,
            /* [in] */ DWORD dwEventFreq,
            /* [in] */ RECT *prcEventRegion,
            /* [in] */ VARIANT *pvarEventArg);
        
        HRESULT ( STDMETHODCALLTYPE *OnUIEventProgress )( 
            IOleComponentUIManager * This,
            /* [out][in] */ __RPC__inout DWORD_PTR *pdwCookie,
            /* [in] */ BOOL fInProgress,
            /* [in] */ __RPC__in LPOLESTR pwszLabel,
            /* [in] */ ULONG nComplete,
            /* [in] */ ULONG nTotal);
        
        HRESULT ( STDMETHODCALLTYPE *SetStatus )( 
            IOleComponentUIManager * This,
            /* [in] */ __RPC__in LPCOLESTR pwszStatusText,
            /* [in] */ DWORD dwReserved);
        
        HRESULT ( STDMETHODCALLTYPE *ShowContextMenu )( 
            IOleComponentUIManager * This,
            /* [in] */ DWORD dwCompRole,
            /* [in] */ __RPC__in REFCLSID rclsidActive,
            /* [in] */ LONG nMenuId,
            /* [in] */ __RPC__in REFPOINTS pos,
            /* [in] */ __RPC__in_opt IOleCommandTarget *pCmdTrgtActive);
        
        HRESULT ( STDMETHODCALLTYPE *ShowHelp )( 
            IOleComponentUIManager * This,
            /* [in] */ DWORD dwCompRole,
            /* [in] */ __RPC__in REFCLSID rclsidComp,
            /* [in] */ POINT posMouse,
            /* [in] */ DWORD dwHelpCmd,
            /* [in] */ __RPC__in LPOLESTR pszHelpFile,
            /* [in] */ DWORD dwData);
        
        HRESULT ( STDMETHODCALLTYPE *ShowMessage )( 
            IOleComponentUIManager * This,
            /* [in] */ DWORD dwCompRole,
            /* [in] */ __RPC__in REFCLSID rclsidComp,
            /* [in] */ __RPC__in LPOLESTR pszTitle,
            /* [in] */ __RPC__in LPOLESTR pszText,
            /* [in] */ __RPC__in LPOLESTR pszHelpFile,
            /* [in] */ DWORD dwHelpContextID,
            /* [in] */ OLEMSGBUTTON msgbtn,
            /* [in] */ OLEMSGDEFBUTTON msgdefbtn,
            /* [in] */ OLEMSGICON msgicon,
            /* [in] */ BOOL fSysAlert,
            /* [retval][out] */ __RPC__out LONG *pnResult);
        
        END_INTERFACE
    } IOleComponentUIManagerVtbl;

    interface IOleComponentUIManager
    {
        CONST_VTBL struct IOleComponentUIManagerVtbl *lpVtbl;
    };

    

#ifdef COBJMACROS


#define IOleComponentUIManager_QueryInterface(This,riid,ppvObject)	\
    ( (This)->lpVtbl -> QueryInterface(This,riid,ppvObject) ) 

#define IOleComponentUIManager_AddRef(This)	\
    ( (This)->lpVtbl -> AddRef(This) ) 

#define IOleComponentUIManager_Release(This)	\
    ( (This)->lpVtbl -> Release(This) ) 


#define IOleComponentUIManager_Deleted1(This)	\
    ( (This)->lpVtbl -> Deleted1(This) ) 

#define IOleComponentUIManager_Deleted2(This)	\
    ( (This)->lpVtbl -> Deleted2(This) ) 

#define IOleComponentUIManager_OnUIEvent(This,dwCompRole,rclsidComp,pguidUIEventGroup,nUIEventId,dwUIEventStatus,dwEventFreq,prcEventRegion,pvarEventArg)	\
    ( (This)->lpVtbl -> OnUIEvent(This,dwCompRole,rclsidComp,pguidUIEventGroup,nUIEventId,dwUIEventStatus,dwEventFreq,prcEventRegion,pvarEventArg) ) 

#define IOleComponentUIManager_OnUIEventProgress(This,pdwCookie,fInProgress,pwszLabel,nComplete,nTotal)	\
    ( (This)->lpVtbl -> OnUIEventProgress(This,pdwCookie,fInProgress,pwszLabel,nComplete,nTotal) ) 

#define IOleComponentUIManager_SetStatus(This,pwszStatusText,dwReserved)	\
    ( (This)->lpVtbl -> SetStatus(This,pwszStatusText,dwReserved) ) 

#define IOleComponentUIManager_ShowContextMenu(This,dwCompRole,rclsidActive,nMenuId,pos,pCmdTrgtActive)	\
    ( (This)->lpVtbl -> ShowContextMenu(This,dwCompRole,rclsidActive,nMenuId,pos,pCmdTrgtActive) ) 

#define IOleComponentUIManager_ShowHelp(This,dwCompRole,rclsidComp,posMouse,dwHelpCmd,pszHelpFile,dwData)	\
    ( (This)->lpVtbl -> ShowHelp(This,dwCompRole,rclsidComp,posMouse,dwHelpCmd,pszHelpFile,dwData) ) 

#define IOleComponentUIManager_ShowMessage(This,dwCompRole,rclsidComp,pszTitle,pszText,pszHelpFile,dwHelpContextID,msgbtn,msgdefbtn,msgicon,fSysAlert,pnResult)	\
    ( (This)->lpVtbl -> ShowMessage(This,dwCompRole,rclsidComp,pszTitle,pszText,pszHelpFile,dwHelpContextID,msgbtn,msgdefbtn,msgicon,fSysAlert,pnResult) ) 

#endif /* COBJMACROS */


#endif 	/* C style interface */




#endif 	/* __IOleComponentUIManager_INTERFACE_DEFINED__ */


/* interface __MIDL_itf_oleipc_0000_0003 */
/* [local] */ 

#pragma warning(pop)
extern const __declspec(selectany) GUID SID_SOleComponentUIManager = { 0x5efc7974, 0x14bc, 0x11cf, { 0x9b, 0x2b, 0x00, 0xaa, 0x00, 0x57, 0x38, 0x19 } };
#define SID_OleComponentUIManager SID_SOleComponentUIManager
extern const __declspec(selectany) GUID CMDSETID_StandardCommandSet97 = { 0x5efc7975, 0x14bc, 0x11cf, { 0x9b, 0x2b, 0x00, 0xaa, 0x00, 0x57, 0x38, 0x19} };
#define CLSID_StandardCommandSet97 CMDSETID_StandardCommandSet97


extern RPC_IF_HANDLE __MIDL_itf_oleipc_0000_0003_v0_0_c_ifspec;
extern RPC_IF_HANDLE __MIDL_itf_oleipc_0000_0003_v0_0_s_ifspec;

#ifndef __IOleInPlaceComponentUIManager_INTERFACE_DEFINED__
#define __IOleInPlaceComponentUIManager_INTERFACE_DEFINED__

/* interface IOleInPlaceComponentUIManager */
/* [object][version][uuid] */ 


EXTERN_C const IID IID_IOleInPlaceComponentUIManager;

#if defined(__cplusplus) && !defined(CINTERFACE)
    
    MIDL_INTERFACE("5efc7973-14bc-11cf-9b2b-00aa00573819")
    IOleInPlaceComponentUIManager : public IUnknown
    {
    public:
        virtual HRESULT STDMETHODCALLTYPE UIActivateForMe( 
            /* [in] */ DWORD dwCompRole,
            /* [in] */ __RPC__in REFCLSID rclsidActive,
            /* [in] */ __RPC__in_opt IOleInPlaceActiveObject *pIPActObj,
            /* [in] */ __RPC__in_opt IOleCommandTarget *pCmdTrgtActive,
            /* [in] */ ULONG cCmdGrpId,
            /* [in] */ __RPC__in LONG *rgnCmdGrpId) = 0;
        
        virtual HRESULT STDMETHODCALLTYPE UpdateUI( 
            /* [in] */ DWORD dwCompRole,
            /* [in] */ BOOL fImmediateUpdate,
            /* [in] */ DWORD dwReserved) = 0;
        
        virtual HRESULT STDMETHODCALLTYPE SetActiveUI( 
            /* [in] */ DWORD dwCompRole,
            /* [in] */ __RPC__in REFCLSID rclsid,
            /* [in] */ ULONG cCmdGrpId,
            /* [in] */ __RPC__in LONG *rgnCmdGrpId) = 0;
        
        virtual /* [local] */ void STDMETHODCALLTYPE OnUIComponentEnterState( 
            /* [in] */ DWORD dwCompRole,
            /* [in] */ DWORD dwStateId,
            /* [in] */ DWORD dwReserved) = 0;
        
        virtual /* [local] */ BOOL STDMETHODCALLTYPE FOnUIComponentExitState( 
            /* [in] */ DWORD dwCompRole,
            /* [in] */ DWORD dwStateId,
            /* [in] */ DWORD dwReserved) = 0;
        
        virtual /* [local] */ BOOL STDMETHODCALLTYPE FUIComponentInState( 
            /* [in] */ DWORD dwStateId) = 0;
        
        virtual /* [local] */ BOOL STDMETHODCALLTYPE FContinueIdle( void) = 0;
        
    };
    
#else 	/* C style interface */

    typedef struct IOleInPlaceComponentUIManagerVtbl
    {
        BEGIN_INTERFACE
        
        HRESULT ( STDMETHODCALLTYPE *QueryInterface )( 
            IOleInPlaceComponentUIManager * This,
            /* [in] */ __RPC__in REFIID riid,
            /* [iid_is][out] */ 
            __RPC__deref_out  void **ppvObject);
        
        ULONG ( STDMETHODCALLTYPE *AddRef )( 
            IOleInPlaceComponentUIManager * This);
        
        ULONG ( STDMETHODCALLTYPE *Release )( 
            IOleInPlaceComponentUIManager * This);
        
        HRESULT ( STDMETHODCALLTYPE *UIActivateForMe )( 
            IOleInPlaceComponentUIManager * This,
            /* [in] */ DWORD dwCompRole,
            /* [in] */ __RPC__in REFCLSID rclsidActive,
            /* [in] */ __RPC__in_opt IOleInPlaceActiveObject *pIPActObj,
            /* [in] */ __RPC__in_opt IOleCommandTarget *pCmdTrgtActive,
            /* [in] */ ULONG cCmdGrpId,
            /* [in] */ __RPC__in LONG *rgnCmdGrpId);
        
        HRESULT ( STDMETHODCALLTYPE *UpdateUI )( 
            IOleInPlaceComponentUIManager * This,
            /* [in] */ DWORD dwCompRole,
            /* [in] */ BOOL fImmediateUpdate,
            /* [in] */ DWORD dwReserved);
        
        HRESULT ( STDMETHODCALLTYPE *SetActiveUI )( 
            IOleInPlaceComponentUIManager * This,
            /* [in] */ DWORD dwCompRole,
            /* [in] */ __RPC__in REFCLSID rclsid,
            /* [in] */ ULONG cCmdGrpId,
            /* [in] */ __RPC__in LONG *rgnCmdGrpId);
        
        /* [local] */ void ( STDMETHODCALLTYPE *OnUIComponentEnterState )( 
            IOleInPlaceComponentUIManager * This,
            /* [in] */ DWORD dwCompRole,
            /* [in] */ DWORD dwStateId,
            /* [in] */ DWORD dwReserved);
        
        /* [local] */ BOOL ( STDMETHODCALLTYPE *FOnUIComponentExitState )( 
            IOleInPlaceComponentUIManager * This,
            /* [in] */ DWORD dwCompRole,
            /* [in] */ DWORD dwStateId,
            /* [in] */ DWORD dwReserved);
        
        /* [local] */ BOOL ( STDMETHODCALLTYPE *FUIComponentInState )( 
            IOleInPlaceComponentUIManager * This,
            /* [in] */ DWORD dwStateId);
        
        /* [local] */ BOOL ( STDMETHODCALLTYPE *FContinueIdle )( 
            IOleInPlaceComponentUIManager * This);
        
        END_INTERFACE
    } IOleInPlaceComponentUIManagerVtbl;

    interface IOleInPlaceComponentUIManager
    {
        CONST_VTBL struct IOleInPlaceComponentUIManagerVtbl *lpVtbl;
    };

    

#ifdef COBJMACROS


#define IOleInPlaceComponentUIManager_QueryInterface(This,riid,ppvObject)	\
    ( (This)->lpVtbl -> QueryInterface(This,riid,ppvObject) ) 

#define IOleInPlaceComponentUIManager_AddRef(This)	\
    ( (This)->lpVtbl -> AddRef(This) ) 

#define IOleInPlaceComponentUIManager_Release(This)	\
    ( (This)->lpVtbl -> Release(This) ) 


#define IOleInPlaceComponentUIManager_UIActivateForMe(This,dwCompRole,rclsidActive,pIPActObj,pCmdTrgtActive,cCmdGrpId,rgnCmdGrpId)	\
    ( (This)->lpVtbl -> UIActivateForMe(This,dwCompRole,rclsidActive,pIPActObj,pCmdTrgtActive,cCmdGrpId,rgnCmdGrpId) ) 

#define IOleInPlaceComponentUIManager_UpdateUI(This,dwCompRole,fImmediateUpdate,dwReserved)	\
    ( (This)->lpVtbl -> UpdateUI(This,dwCompRole,fImmediateUpdate,dwReserved) ) 

#define IOleInPlaceComponentUIManager_SetActiveUI(This,dwCompRole,rclsid,cCmdGrpId,rgnCmdGrpId)	\
    ( (This)->lpVtbl -> SetActiveUI(This,dwCompRole,rclsid,cCmdGrpId,rgnCmdGrpId) ) 

#define IOleInPlaceComponentUIManager_OnUIComponentEnterState(This,dwCompRole,dwStateId,dwReserved)	\
    ( (This)->lpVtbl -> OnUIComponentEnterState(This,dwCompRole,dwStateId,dwReserved) ) 

#define IOleInPlaceComponentUIManager_FOnUIComponentExitState(This,dwCompRole,dwStateId,dwReserved)	\
    ( (This)->lpVtbl -> FOnUIComponentExitState(This,dwCompRole,dwStateId,dwReserved) ) 

#define IOleInPlaceComponentUIManager_FUIComponentInState(This,dwStateId)	\
    ( (This)->lpVtbl -> FUIComponentInState(This,dwStateId) ) 

#define IOleInPlaceComponentUIManager_FContinueIdle(This)	\
    ( (This)->lpVtbl -> FContinueIdle(This) ) 

#endif /* COBJMACROS */


#endif 	/* C style interface */




#endif 	/* __IOleInPlaceComponentUIManager_INTERFACE_DEFINED__ */


/* interface __MIDL_itf_oleipc_0000_0004 */
/* [local] */ 

#define MSOCMDSTATE_DISABLED	   OLECMDSTATE_DISABLED
#define MSOCMDSTATE_UP 		 OLECMDSTATE_UP
#define MSOCMDSTATE_DOWN	       OLECMDSTATE_DOWN
#define MSOCMDSTATE_NINCHED 	    OLECMDSTATE_NINCHED
#define MSOCMDSTATE_INVISIBLE  OLECMDSTATE_INVISIBLE


extern RPC_IF_HANDLE __MIDL_itf_oleipc_0000_0004_v0_0_c_ifspec;
extern RPC_IF_HANDLE __MIDL_itf_oleipc_0000_0004_v0_0_s_ifspec;

/* Additional Prototypes for ALL interfaces */

unsigned long             __RPC_USER  HWND_UserSize(     unsigned long *, unsigned long            , HWND * ); 
unsigned char * __RPC_USER  HWND_UserMarshal(  unsigned long *, unsigned char *, HWND * ); 
unsigned char * __RPC_USER  HWND_UserUnmarshal(unsigned long *, unsigned char *, HWND * ); 
void                      __RPC_USER  HWND_UserFree(     unsigned long *, HWND * ); 

/* end of Additional Prototypes */

#ifdef __cplusplus
}
#endif

#endif


