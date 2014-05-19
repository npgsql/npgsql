

/* this ALWAYS GENERATED file contains the definitions for the interfaces */


 /* File created by MIDL compiler version 7.00.0499 */
/* Compiler settings for vsbrowse.idl:
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

#ifndef __vsbrowse_h__
#define __vsbrowse_h__

#if defined(_MSC_VER) && (_MSC_VER >= 1020)
#pragma once
#endif

/* Forward Declarations */ 

#ifndef __IVsWebURLMRU_FWD_DEFINED__
#define __IVsWebURLMRU_FWD_DEFINED__
typedef interface IVsWebURLMRU IVsWebURLMRU;
#endif 	/* __IVsWebURLMRU_FWD_DEFINED__ */


#ifndef __IVsWebFavorites_FWD_DEFINED__
#define __IVsWebFavorites_FWD_DEFINED__
typedef interface IVsWebFavorites IVsWebFavorites;
#endif 	/* __IVsWebFavorites_FWD_DEFINED__ */


#ifndef __IVsFavoritesProvider_FWD_DEFINED__
#define __IVsFavoritesProvider_FWD_DEFINED__
typedef interface IVsFavoritesProvider IVsFavoritesProvider;
#endif 	/* __IVsFavoritesProvider_FWD_DEFINED__ */


#ifndef __IVsWebBrowserUser_FWD_DEFINED__
#define __IVsWebBrowserUser_FWD_DEFINED__
typedef interface IVsWebBrowserUser IVsWebBrowserUser;
#endif 	/* __IVsWebBrowserUser_FWD_DEFINED__ */


#ifndef __IVsWebBrowsingService_FWD_DEFINED__
#define __IVsWebBrowsingService_FWD_DEFINED__
typedef interface IVsWebBrowsingService IVsWebBrowsingService;
#endif 	/* __IVsWebBrowsingService_FWD_DEFINED__ */


#ifndef __IVsWebBrowser_FWD_DEFINED__
#define __IVsWebBrowser_FWD_DEFINED__
typedef interface IVsWebBrowser IVsWebBrowser;
#endif 	/* __IVsWebBrowser_FWD_DEFINED__ */


#ifndef __IVsWebPreviewAction_FWD_DEFINED__
#define __IVsWebPreviewAction_FWD_DEFINED__
typedef interface IVsWebPreviewAction IVsWebPreviewAction;
#endif 	/* __IVsWebPreviewAction_FWD_DEFINED__ */


#ifndef __IVsWebPreview_FWD_DEFINED__
#define __IVsWebPreview_FWD_DEFINED__
typedef interface IVsWebPreview IVsWebPreview;
#endif 	/* __IVsWebPreview_FWD_DEFINED__ */


/* header files for imported files */
#include "oaidl.h"
#include "ocidl.h"
#include "vsshell.h"

#ifdef __cplusplus
extern "C"{
#endif 


/* interface __MIDL_itf_vsbrowse_0000_0000 */
/* [local] */ 











extern RPC_IF_HANDLE __MIDL_itf_vsbrowse_0000_0000_v0_0_c_ifspec;
extern RPC_IF_HANDLE __MIDL_itf_vsbrowse_0000_0000_v0_0_s_ifspec;

#ifndef __IVsWebURLMRU_INTERFACE_DEFINED__
#define __IVsWebURLMRU_INTERFACE_DEFINED__

/* interface IVsWebURLMRU */
/* [object][unique][version][uuid] */ 


EXTERN_C const IID IID_IVsWebURLMRU;

#if defined(__cplusplus) && !defined(CINTERFACE)
    
    MIDL_INTERFACE("e8b06f3d-6d01-11d2-aa7d-00c04f990343")
    IVsWebURLMRU : public IUnknown
    {
    public:
        virtual HRESULT STDMETHODCALLTYPE AddURL( 
            /* [in] */ __RPC__in BSTR bstrURL) = 0;
        
        virtual HRESULT STDMETHODCALLTYPE GetURLArray( 
            /* [retval][out] */ __RPC__out VARIANT *pvarURLs) = 0;
        
    };
    
#else 	/* C style interface */

    typedef struct IVsWebURLMRUVtbl
    {
        BEGIN_INTERFACE
        
        HRESULT ( STDMETHODCALLTYPE *QueryInterface )( 
            IVsWebURLMRU * This,
            /* [in] */ __RPC__in REFIID riid,
            /* [iid_is][out] */ 
            __RPC__deref_out  void **ppvObject);
        
        ULONG ( STDMETHODCALLTYPE *AddRef )( 
            IVsWebURLMRU * This);
        
        ULONG ( STDMETHODCALLTYPE *Release )( 
            IVsWebURLMRU * This);
        
        HRESULT ( STDMETHODCALLTYPE *AddURL )( 
            IVsWebURLMRU * This,
            /* [in] */ __RPC__in BSTR bstrURL);
        
        HRESULT ( STDMETHODCALLTYPE *GetURLArray )( 
            IVsWebURLMRU * This,
            /* [retval][out] */ __RPC__out VARIANT *pvarURLs);
        
        END_INTERFACE
    } IVsWebURLMRUVtbl;

    interface IVsWebURLMRU
    {
        CONST_VTBL struct IVsWebURLMRUVtbl *lpVtbl;
    };

    

#ifdef COBJMACROS


#define IVsWebURLMRU_QueryInterface(This,riid,ppvObject)	\
    ( (This)->lpVtbl -> QueryInterface(This,riid,ppvObject) ) 

#define IVsWebURLMRU_AddRef(This)	\
    ( (This)->lpVtbl -> AddRef(This) ) 

#define IVsWebURLMRU_Release(This)	\
    ( (This)->lpVtbl -> Release(This) ) 


#define IVsWebURLMRU_AddURL(This,bstrURL)	\
    ( (This)->lpVtbl -> AddURL(This,bstrURL) ) 

#define IVsWebURLMRU_GetURLArray(This,pvarURLs)	\
    ( (This)->lpVtbl -> GetURLArray(This,pvarURLs) ) 

#endif /* COBJMACROS */


#endif 	/* C style interface */




#endif 	/* __IVsWebURLMRU_INTERFACE_DEFINED__ */


/* interface __MIDL_itf_vsbrowse_0000_0001 */
/* [local] */ 

#define SID_SVsWebURLMRU IID_IVsWebURLMRU


extern RPC_IF_HANDLE __MIDL_itf_vsbrowse_0000_0001_v0_0_c_ifspec;
extern RPC_IF_HANDLE __MIDL_itf_vsbrowse_0000_0001_v0_0_s_ifspec;

#ifndef __IVsWebFavorites_INTERFACE_DEFINED__
#define __IVsWebFavorites_INTERFACE_DEFINED__

/* interface IVsWebFavorites */
/* [object][unique][version][uuid] */ 


EXTERN_C const IID IID_IVsWebFavorites;

#if defined(__cplusplus) && !defined(CINTERFACE)
    
    MIDL_INTERFACE("e8b06f4c-6d01-11d2-aa7d-00c04f990343")
    IVsWebFavorites : public IUnknown
    {
    public:
        virtual HRESULT STDMETHODCALLTYPE AddFavorite( 
            /* [in] */ __RPC__in LPCOLESTR lpszURL,
            /* [in] */ __RPC__in LPCOLESTR lpszName,
            /* [in] */ __RPC__in LPCOLESTR pszIconFileName,
            /* [in] */ int iIconIndex,
            /* [out] */ __RPC__deref_out_opt BSTR *pbstrFileName) = 0;
        
    };
    
#else 	/* C style interface */

    typedef struct IVsWebFavoritesVtbl
    {
        BEGIN_INTERFACE
        
        HRESULT ( STDMETHODCALLTYPE *QueryInterface )( 
            IVsWebFavorites * This,
            /* [in] */ __RPC__in REFIID riid,
            /* [iid_is][out] */ 
            __RPC__deref_out  void **ppvObject);
        
        ULONG ( STDMETHODCALLTYPE *AddRef )( 
            IVsWebFavorites * This);
        
        ULONG ( STDMETHODCALLTYPE *Release )( 
            IVsWebFavorites * This);
        
        HRESULT ( STDMETHODCALLTYPE *AddFavorite )( 
            IVsWebFavorites * This,
            /* [in] */ __RPC__in LPCOLESTR lpszURL,
            /* [in] */ __RPC__in LPCOLESTR lpszName,
            /* [in] */ __RPC__in LPCOLESTR pszIconFileName,
            /* [in] */ int iIconIndex,
            /* [out] */ __RPC__deref_out_opt BSTR *pbstrFileName);
        
        END_INTERFACE
    } IVsWebFavoritesVtbl;

    interface IVsWebFavorites
    {
        CONST_VTBL struct IVsWebFavoritesVtbl *lpVtbl;
    };

    

#ifdef COBJMACROS


#define IVsWebFavorites_QueryInterface(This,riid,ppvObject)	\
    ( (This)->lpVtbl -> QueryInterface(This,riid,ppvObject) ) 

#define IVsWebFavorites_AddRef(This)	\
    ( (This)->lpVtbl -> AddRef(This) ) 

#define IVsWebFavorites_Release(This)	\
    ( (This)->lpVtbl -> Release(This) ) 


#define IVsWebFavorites_AddFavorite(This,lpszURL,lpszName,pszIconFileName,iIconIndex,pbstrFileName)	\
    ( (This)->lpVtbl -> AddFavorite(This,lpszURL,lpszName,pszIconFileName,iIconIndex,pbstrFileName) ) 

#endif /* COBJMACROS */


#endif 	/* C style interface */




#endif 	/* __IVsWebFavorites_INTERFACE_DEFINED__ */


/* interface __MIDL_itf_vsbrowse_0000_0002 */
/* [local] */ 

#define SID_SVsWebFavorites IID_IVsWebFavorites


extern RPC_IF_HANDLE __MIDL_itf_vsbrowse_0000_0002_v0_0_c_ifspec;
extern RPC_IF_HANDLE __MIDL_itf_vsbrowse_0000_0002_v0_0_s_ifspec;

#ifndef __IVsFavoritesProvider_INTERFACE_DEFINED__
#define __IVsFavoritesProvider_INTERFACE_DEFINED__

/* interface IVsFavoritesProvider */
/* [object][unique][version][uuid] */ 


EXTERN_C const IID IID_IVsFavoritesProvider;

#if defined(__cplusplus) && !defined(CINTERFACE)
    
    MIDL_INTERFACE("e8b06f4d-6d01-11d2-aa7d-00c04f990343")
    IVsFavoritesProvider : public IUnknown
    {
    public:
        virtual HRESULT STDMETHODCALLTYPE Navigate( 
            /* [in] */ __RPC__in LPCOLESTR lpszURL,
            /* [in] */ DWORD dwFlags,
            /* [out] */ __RPC__deref_out_opt IVsWindowFrame **ppFrame) = 0;
        
    };
    
#else 	/* C style interface */

    typedef struct IVsFavoritesProviderVtbl
    {
        BEGIN_INTERFACE
        
        HRESULT ( STDMETHODCALLTYPE *QueryInterface )( 
            IVsFavoritesProvider * This,
            /* [in] */ __RPC__in REFIID riid,
            /* [iid_is][out] */ 
            __RPC__deref_out  void **ppvObject);
        
        ULONG ( STDMETHODCALLTYPE *AddRef )( 
            IVsFavoritesProvider * This);
        
        ULONG ( STDMETHODCALLTYPE *Release )( 
            IVsFavoritesProvider * This);
        
        HRESULT ( STDMETHODCALLTYPE *Navigate )( 
            IVsFavoritesProvider * This,
            /* [in] */ __RPC__in LPCOLESTR lpszURL,
            /* [in] */ DWORD dwFlags,
            /* [out] */ __RPC__deref_out_opt IVsWindowFrame **ppFrame);
        
        END_INTERFACE
    } IVsFavoritesProviderVtbl;

    interface IVsFavoritesProvider
    {
        CONST_VTBL struct IVsFavoritesProviderVtbl *lpVtbl;
    };

    

#ifdef COBJMACROS


#define IVsFavoritesProvider_QueryInterface(This,riid,ppvObject)	\
    ( (This)->lpVtbl -> QueryInterface(This,riid,ppvObject) ) 

#define IVsFavoritesProvider_AddRef(This)	\
    ( (This)->lpVtbl -> AddRef(This) ) 

#define IVsFavoritesProvider_Release(This)	\
    ( (This)->lpVtbl -> Release(This) ) 


#define IVsFavoritesProvider_Navigate(This,lpszURL,dwFlags,ppFrame)	\
    ( (This)->lpVtbl -> Navigate(This,lpszURL,dwFlags,ppFrame) ) 

#endif /* COBJMACROS */


#endif 	/* C style interface */




#endif 	/* __IVsFavoritesProvider_INTERFACE_DEFINED__ */


/* interface __MIDL_itf_vsbrowse_0000_0003 */
/* [local] */ 


enum __VSWBCUSTOMURL
    {	VSWBCU_Home	= 0,
	VSWBCU_Search	= 1,
	VSWBCU_Credits	= 2
    } ;
typedef DWORD VSWBCUSTOMURL;



extern RPC_IF_HANDLE __MIDL_itf_vsbrowse_0000_0003_v0_0_c_ifspec;
extern RPC_IF_HANDLE __MIDL_itf_vsbrowse_0000_0003_v0_0_s_ifspec;

#ifndef __IVsWebBrowserUser_INTERFACE_DEFINED__
#define __IVsWebBrowserUser_INTERFACE_DEFINED__

/* interface IVsWebBrowserUser */
/* [object][unique][version][uuid] */ 


EXTERN_C const IID IID_IVsWebBrowserUser;

#if defined(__cplusplus) && !defined(CINTERFACE)
    
    MIDL_INTERFACE("e8b06f4b-6d01-11d2-aa7d-00c04f990343")
    IVsWebBrowserUser : public IUnknown
    {
    public:
        virtual HRESULT STDMETHODCALLTYPE Disconnect( void) = 0;
        
        virtual HRESULT STDMETHODCALLTYPE GetCustomMenuInfo( 
            /* [in] */ __RPC__in_opt IUnknown *pUnkCmdReserved,
            /* [in] */ __RPC__in_opt IDispatch *pDispReserved,
            /* [in] */ DWORD dwType,
            /* [in] */ DWORD dwPosition,
            /* [out] */ __RPC__out GUID *pguidCmdGroup,
            /* [out] */ __RPC__out long *pdwMenuID) = 0;
        
        virtual HRESULT STDMETHODCALLTYPE GetCmdUIGuid( 
            /* [out] */ __RPC__out GUID *pguidCmdUI) = 0;
        
        virtual HRESULT STDMETHODCALLTYPE GetExternalObject( 
            /* [out] */ __RPC__deref_out_opt IDispatch **ppDispObject) = 0;
        
        virtual HRESULT STDMETHODCALLTYPE TranslateUrl( 
            /* [in] */ DWORD dwReserved,
            /* [in] */ __RPC__in LPCOLESTR lpszURLIn,
            /* [out] */ __RPC__deref_out_opt LPOLESTR *lppszURLOut) = 0;
        
        virtual HRESULT STDMETHODCALLTYPE FilterDataObject( 
            /* [in] */ __RPC__in_opt IDataObject *pDataObjIn,
            /* [out] */ __RPC__deref_out_opt IDataObject **ppDataObjOut) = 0;
        
        virtual HRESULT STDMETHODCALLTYPE GetDropTarget( 
            /* [in] */ __RPC__in_opt IDropTarget *pDropTgtIn,
            /* [out] */ __RPC__deref_out_opt IDropTarget **ppDropTgtOut) = 0;
        
        virtual HRESULT STDMETHODCALLTYPE TranslateAccelarator( 
            /* [in] */ __RPC__in LPMSG lpMsg) = 0;
        
        virtual HRESULT STDMETHODCALLTYPE GetCustomURL( 
            /* [in] */ VSWBCUSTOMURL nPage,
            /* [out] */ __RPC__deref_out_opt BSTR *pbstrURL) = 0;
        
        virtual HRESULT STDMETHODCALLTYPE GetOptionKeyPath( 
            /* [in] */ DWORD dwReserved,
            /* [out] */ __RPC__deref_out_opt BSTR *pbstrKey) = 0;
        
        virtual HRESULT STDMETHODCALLTYPE Resize( 
            /* [in] */ int cx,
            /* [in] */ int cy) = 0;
        
    };
    
#else 	/* C style interface */

    typedef struct IVsWebBrowserUserVtbl
    {
        BEGIN_INTERFACE
        
        HRESULT ( STDMETHODCALLTYPE *QueryInterface )( 
            IVsWebBrowserUser * This,
            /* [in] */ __RPC__in REFIID riid,
            /* [iid_is][out] */ 
            __RPC__deref_out  void **ppvObject);
        
        ULONG ( STDMETHODCALLTYPE *AddRef )( 
            IVsWebBrowserUser * This);
        
        ULONG ( STDMETHODCALLTYPE *Release )( 
            IVsWebBrowserUser * This);
        
        HRESULT ( STDMETHODCALLTYPE *Disconnect )( 
            IVsWebBrowserUser * This);
        
        HRESULT ( STDMETHODCALLTYPE *GetCustomMenuInfo )( 
            IVsWebBrowserUser * This,
            /* [in] */ __RPC__in_opt IUnknown *pUnkCmdReserved,
            /* [in] */ __RPC__in_opt IDispatch *pDispReserved,
            /* [in] */ DWORD dwType,
            /* [in] */ DWORD dwPosition,
            /* [out] */ __RPC__out GUID *pguidCmdGroup,
            /* [out] */ __RPC__out long *pdwMenuID);
        
        HRESULT ( STDMETHODCALLTYPE *GetCmdUIGuid )( 
            IVsWebBrowserUser * This,
            /* [out] */ __RPC__out GUID *pguidCmdUI);
        
        HRESULT ( STDMETHODCALLTYPE *GetExternalObject )( 
            IVsWebBrowserUser * This,
            /* [out] */ __RPC__deref_out_opt IDispatch **ppDispObject);
        
        HRESULT ( STDMETHODCALLTYPE *TranslateUrl )( 
            IVsWebBrowserUser * This,
            /* [in] */ DWORD dwReserved,
            /* [in] */ __RPC__in LPCOLESTR lpszURLIn,
            /* [out] */ __RPC__deref_out_opt LPOLESTR *lppszURLOut);
        
        HRESULT ( STDMETHODCALLTYPE *FilterDataObject )( 
            IVsWebBrowserUser * This,
            /* [in] */ __RPC__in_opt IDataObject *pDataObjIn,
            /* [out] */ __RPC__deref_out_opt IDataObject **ppDataObjOut);
        
        HRESULT ( STDMETHODCALLTYPE *GetDropTarget )( 
            IVsWebBrowserUser * This,
            /* [in] */ __RPC__in_opt IDropTarget *pDropTgtIn,
            /* [out] */ __RPC__deref_out_opt IDropTarget **ppDropTgtOut);
        
        HRESULT ( STDMETHODCALLTYPE *TranslateAccelarator )( 
            IVsWebBrowserUser * This,
            /* [in] */ __RPC__in LPMSG lpMsg);
        
        HRESULT ( STDMETHODCALLTYPE *GetCustomURL )( 
            IVsWebBrowserUser * This,
            /* [in] */ VSWBCUSTOMURL nPage,
            /* [out] */ __RPC__deref_out_opt BSTR *pbstrURL);
        
        HRESULT ( STDMETHODCALLTYPE *GetOptionKeyPath )( 
            IVsWebBrowserUser * This,
            /* [in] */ DWORD dwReserved,
            /* [out] */ __RPC__deref_out_opt BSTR *pbstrKey);
        
        HRESULT ( STDMETHODCALLTYPE *Resize )( 
            IVsWebBrowserUser * This,
            /* [in] */ int cx,
            /* [in] */ int cy);
        
        END_INTERFACE
    } IVsWebBrowserUserVtbl;

    interface IVsWebBrowserUser
    {
        CONST_VTBL struct IVsWebBrowserUserVtbl *lpVtbl;
    };

    

#ifdef COBJMACROS


#define IVsWebBrowserUser_QueryInterface(This,riid,ppvObject)	\
    ( (This)->lpVtbl -> QueryInterface(This,riid,ppvObject) ) 

#define IVsWebBrowserUser_AddRef(This)	\
    ( (This)->lpVtbl -> AddRef(This) ) 

#define IVsWebBrowserUser_Release(This)	\
    ( (This)->lpVtbl -> Release(This) ) 


#define IVsWebBrowserUser_Disconnect(This)	\
    ( (This)->lpVtbl -> Disconnect(This) ) 

#define IVsWebBrowserUser_GetCustomMenuInfo(This,pUnkCmdReserved,pDispReserved,dwType,dwPosition,pguidCmdGroup,pdwMenuID)	\
    ( (This)->lpVtbl -> GetCustomMenuInfo(This,pUnkCmdReserved,pDispReserved,dwType,dwPosition,pguidCmdGroup,pdwMenuID) ) 

#define IVsWebBrowserUser_GetCmdUIGuid(This,pguidCmdUI)	\
    ( (This)->lpVtbl -> GetCmdUIGuid(This,pguidCmdUI) ) 

#define IVsWebBrowserUser_GetExternalObject(This,ppDispObject)	\
    ( (This)->lpVtbl -> GetExternalObject(This,ppDispObject) ) 

#define IVsWebBrowserUser_TranslateUrl(This,dwReserved,lpszURLIn,lppszURLOut)	\
    ( (This)->lpVtbl -> TranslateUrl(This,dwReserved,lpszURLIn,lppszURLOut) ) 

#define IVsWebBrowserUser_FilterDataObject(This,pDataObjIn,ppDataObjOut)	\
    ( (This)->lpVtbl -> FilterDataObject(This,pDataObjIn,ppDataObjOut) ) 

#define IVsWebBrowserUser_GetDropTarget(This,pDropTgtIn,ppDropTgtOut)	\
    ( (This)->lpVtbl -> GetDropTarget(This,pDropTgtIn,ppDropTgtOut) ) 

#define IVsWebBrowserUser_TranslateAccelarator(This,lpMsg)	\
    ( (This)->lpVtbl -> TranslateAccelarator(This,lpMsg) ) 

#define IVsWebBrowserUser_GetCustomURL(This,nPage,pbstrURL)	\
    ( (This)->lpVtbl -> GetCustomURL(This,nPage,pbstrURL) ) 

#define IVsWebBrowserUser_GetOptionKeyPath(This,dwReserved,pbstrKey)	\
    ( (This)->lpVtbl -> GetOptionKeyPath(This,dwReserved,pbstrKey) ) 

#define IVsWebBrowserUser_Resize(This,cx,cy)	\
    ( (This)->lpVtbl -> Resize(This,cx,cy) ) 

#endif /* COBJMACROS */


#endif 	/* C style interface */




#endif 	/* __IVsWebBrowserUser_INTERFACE_DEFINED__ */


/* interface __MIDL_itf_vsbrowse_0000_0004 */
/* [local] */ 


enum __VSCREATEWEBBROWSER
    {	VSCWB_AutoShow	= 0x1,
	VSCWB_AddToMRU	= 0x2,
	VSCWB_ReuseExisting	= 0x10,
	VSCWB_ForceNew	= 0x20,
	VSCWB_FrameMdiChild	= 0,
	VSCWB_FrameFloat	= 0x40,
	VSCWB_FrameDock	= 0x80,
	VSCWB_StartHome	= 0x100,
	VSCWB_StartSearch	= 0x200,
	VSCWB_StartCustom	= 0x400,
	VSCWB_NoHistory	= 0x10000,
	VSCWB_NoReadCache	= 0x20000,
	VSCWB_NoWriteToCache	= 0x40000,
	VSCWB_AllowAutosearch	= 0x80000,
	VSCWB_OptionNoDocProps	= 0,
	VSCWB_OptionShowDocProps	= 0x1000000,
	VSCWB_OptionCustomDocProps	= 0x2000000,
	VSCWB_OptionDisableFind	= 0x4000000,
	VSCWB_OptionDisableDockable	= 0x8000000,
	VSCWB_OptionDisableStatusBar	= 0x10000000,
	VSCWB_StartURLMask	= 0xf00,
	VSCWB_NavOptionMask	= 0xf0000,
	VSCWB_OptionsMask	= 0xff000000
    } ;
typedef DWORD VSCREATEWEBBROWSER;


enum __VSWBNAVIGATEFLAGS
    {	VSNWB_ForceNew	= 0x1,
	VSNWB_AddToMRU	= 0x2,
	VSNWB_VsURLOnly	= 0x4,
	VSNWB_WebURLOnly	= 0x8
    } ;
typedef DWORD VSWBNAVIGATEFLAGS;



extern RPC_IF_HANDLE __MIDL_itf_vsbrowse_0000_0004_v0_0_c_ifspec;
extern RPC_IF_HANDLE __MIDL_itf_vsbrowse_0000_0004_v0_0_s_ifspec;

#ifndef __IVsWebBrowsingService_INTERFACE_DEFINED__
#define __IVsWebBrowsingService_INTERFACE_DEFINED__

/* interface IVsWebBrowsingService */
/* [object][unique][version][uuid] */ 


EXTERN_C const IID IID_IVsWebBrowsingService;

#if defined(__cplusplus) && !defined(CINTERFACE)
    
    MIDL_INTERFACE("e8b06f51-6d01-11d2-aa7d-00c04f990343")
    IVsWebBrowsingService : public IUnknown
    {
    public:
        virtual HRESULT STDMETHODCALLTYPE CreateWebBrowser( 
            /* [in] */ VSCREATEWEBBROWSER dwCreateFlags,
            /* [in] */ __RPC__in REFGUID rguidOwner,
            /* [in] */ __RPC__in LPCOLESTR lpszBaseCaption,
            /* [in] */ __RPC__in LPCOLESTR lpszStartURL,
            /* [in] */ __RPC__in_opt IVsWebBrowserUser *pUser,
            /* [out] */ __RPC__deref_out_opt IVsWebBrowser **ppBrowser,
            /* [out] */ __RPC__deref_out_opt IVsWindowFrame **ppFrame) = 0;
        
        virtual HRESULT STDMETHODCALLTYPE GetFirstWebBrowser( 
            /* [in] */ __RPC__in REFGUID rguidPersistenceSlot,
            /* [out] */ __RPC__deref_out_opt IVsWindowFrame **ppFrame,
            /* [out] */ __RPC__deref_out_opt IVsWebBrowser **ppBrowser) = 0;
        
        virtual HRESULT STDMETHODCALLTYPE GetWebBrowserEnum( 
            /* [in] */ __RPC__in REFGUID rguidPersistenceSlot,
            /* [out] */ __RPC__deref_out_opt IEnumWindowFrames **ppEnum) = 0;
        
        virtual HRESULT STDMETHODCALLTYPE CreateExternalWebBrowser( 
            /* [in] */ VSCREATEWEBBROWSER dwCreateFlags,
            /* [in] */ VSPREVIEWRESOLUTION dwResolution,
            /* [in] */ __RPC__in LPCOLESTR lpszURL) = 0;
        
        virtual HRESULT STDMETHODCALLTYPE CreateWebBrowserEx( 
            /* [in] */ VSCREATEWEBBROWSER dwCreateFlags,
            /* [in] */ __RPC__in REFGUID rguidPersistenceSlot,
            /* [in] */ DWORD dwId,
            /* [in] */ __RPC__in LPCOLESTR lpszBaseCaption,
            /* [in] */ __RPC__in LPCOLESTR lpszStartURL,
            /* [in] */ __RPC__in_opt IVsWebBrowserUser *pUser,
            /* [out] */ __RPC__deref_out_opt IVsWebBrowser **ppBrowser,
            /* [out] */ __RPC__deref_out_opt IVsWindowFrame **ppFrame) = 0;
        
        virtual HRESULT STDMETHODCALLTYPE Navigate( 
            /* [in] */ __RPC__in LPCOLESTR lpszURL,
            /* [in] */ VSWBNAVIGATEFLAGS dwNaviageFlags,
            /* [out] */ __RPC__deref_out_opt IVsWindowFrame **ppFrame) = 0;
        
    };
    
#else 	/* C style interface */

    typedef struct IVsWebBrowsingServiceVtbl
    {
        BEGIN_INTERFACE
        
        HRESULT ( STDMETHODCALLTYPE *QueryInterface )( 
            IVsWebBrowsingService * This,
            /* [in] */ __RPC__in REFIID riid,
            /* [iid_is][out] */ 
            __RPC__deref_out  void **ppvObject);
        
        ULONG ( STDMETHODCALLTYPE *AddRef )( 
            IVsWebBrowsingService * This);
        
        ULONG ( STDMETHODCALLTYPE *Release )( 
            IVsWebBrowsingService * This);
        
        HRESULT ( STDMETHODCALLTYPE *CreateWebBrowser )( 
            IVsWebBrowsingService * This,
            /* [in] */ VSCREATEWEBBROWSER dwCreateFlags,
            /* [in] */ __RPC__in REFGUID rguidOwner,
            /* [in] */ __RPC__in LPCOLESTR lpszBaseCaption,
            /* [in] */ __RPC__in LPCOLESTR lpszStartURL,
            /* [in] */ __RPC__in_opt IVsWebBrowserUser *pUser,
            /* [out] */ __RPC__deref_out_opt IVsWebBrowser **ppBrowser,
            /* [out] */ __RPC__deref_out_opt IVsWindowFrame **ppFrame);
        
        HRESULT ( STDMETHODCALLTYPE *GetFirstWebBrowser )( 
            IVsWebBrowsingService * This,
            /* [in] */ __RPC__in REFGUID rguidPersistenceSlot,
            /* [out] */ __RPC__deref_out_opt IVsWindowFrame **ppFrame,
            /* [out] */ __RPC__deref_out_opt IVsWebBrowser **ppBrowser);
        
        HRESULT ( STDMETHODCALLTYPE *GetWebBrowserEnum )( 
            IVsWebBrowsingService * This,
            /* [in] */ __RPC__in REFGUID rguidPersistenceSlot,
            /* [out] */ __RPC__deref_out_opt IEnumWindowFrames **ppEnum);
        
        HRESULT ( STDMETHODCALLTYPE *CreateExternalWebBrowser )( 
            IVsWebBrowsingService * This,
            /* [in] */ VSCREATEWEBBROWSER dwCreateFlags,
            /* [in] */ VSPREVIEWRESOLUTION dwResolution,
            /* [in] */ __RPC__in LPCOLESTR lpszURL);
        
        HRESULT ( STDMETHODCALLTYPE *CreateWebBrowserEx )( 
            IVsWebBrowsingService * This,
            /* [in] */ VSCREATEWEBBROWSER dwCreateFlags,
            /* [in] */ __RPC__in REFGUID rguidPersistenceSlot,
            /* [in] */ DWORD dwId,
            /* [in] */ __RPC__in LPCOLESTR lpszBaseCaption,
            /* [in] */ __RPC__in LPCOLESTR lpszStartURL,
            /* [in] */ __RPC__in_opt IVsWebBrowserUser *pUser,
            /* [out] */ __RPC__deref_out_opt IVsWebBrowser **ppBrowser,
            /* [out] */ __RPC__deref_out_opt IVsWindowFrame **ppFrame);
        
        HRESULT ( STDMETHODCALLTYPE *Navigate )( 
            IVsWebBrowsingService * This,
            /* [in] */ __RPC__in LPCOLESTR lpszURL,
            /* [in] */ VSWBNAVIGATEFLAGS dwNaviageFlags,
            /* [out] */ __RPC__deref_out_opt IVsWindowFrame **ppFrame);
        
        END_INTERFACE
    } IVsWebBrowsingServiceVtbl;

    interface IVsWebBrowsingService
    {
        CONST_VTBL struct IVsWebBrowsingServiceVtbl *lpVtbl;
    };

    

#ifdef COBJMACROS


#define IVsWebBrowsingService_QueryInterface(This,riid,ppvObject)	\
    ( (This)->lpVtbl -> QueryInterface(This,riid,ppvObject) ) 

#define IVsWebBrowsingService_AddRef(This)	\
    ( (This)->lpVtbl -> AddRef(This) ) 

#define IVsWebBrowsingService_Release(This)	\
    ( (This)->lpVtbl -> Release(This) ) 


#define IVsWebBrowsingService_CreateWebBrowser(This,dwCreateFlags,rguidOwner,lpszBaseCaption,lpszStartURL,pUser,ppBrowser,ppFrame)	\
    ( (This)->lpVtbl -> CreateWebBrowser(This,dwCreateFlags,rguidOwner,lpszBaseCaption,lpszStartURL,pUser,ppBrowser,ppFrame) ) 

#define IVsWebBrowsingService_GetFirstWebBrowser(This,rguidPersistenceSlot,ppFrame,ppBrowser)	\
    ( (This)->lpVtbl -> GetFirstWebBrowser(This,rguidPersistenceSlot,ppFrame,ppBrowser) ) 

#define IVsWebBrowsingService_GetWebBrowserEnum(This,rguidPersistenceSlot,ppEnum)	\
    ( (This)->lpVtbl -> GetWebBrowserEnum(This,rguidPersistenceSlot,ppEnum) ) 

#define IVsWebBrowsingService_CreateExternalWebBrowser(This,dwCreateFlags,dwResolution,lpszURL)	\
    ( (This)->lpVtbl -> CreateExternalWebBrowser(This,dwCreateFlags,dwResolution,lpszURL) ) 

#define IVsWebBrowsingService_CreateWebBrowserEx(This,dwCreateFlags,rguidPersistenceSlot,dwId,lpszBaseCaption,lpszStartURL,pUser,ppBrowser,ppFrame)	\
    ( (This)->lpVtbl -> CreateWebBrowserEx(This,dwCreateFlags,rguidPersistenceSlot,dwId,lpszBaseCaption,lpszStartURL,pUser,ppBrowser,ppFrame) ) 

#define IVsWebBrowsingService_Navigate(This,lpszURL,dwNaviageFlags,ppFrame)	\
    ( (This)->lpVtbl -> Navigate(This,lpszURL,dwNaviageFlags,ppFrame) ) 

#endif /* COBJMACROS */


#endif 	/* C style interface */




#endif 	/* __IVsWebBrowsingService_INTERFACE_DEFINED__ */


/* interface __MIDL_itf_vsbrowse_0000_0005 */
/* [local] */ 

#define SID_SVsWebBrowsingService IID_IVsWebBrowsingService

enum __VSWBREFRESHTYPE
    {	VSWBR_Normal	= 0,
	VSWBR_IfExpired	= 1,
	VSWBR_Completely	= 3
    } ;
typedef DWORD VSWBREFRESHTYPE;


enum __VSWBDOCINFOINDEX
    {	VSWBDI_DocDispatch	= 0,
	VSWBDI_DocName	= 1,
	VSWBDI_DocURL	= 2,
	VSWBDI_DocType	= 3,
	VSWBDI_DocStatusText	= 4,
	VSWBDI_DocBusyStatus	= 5,
	VSWBDI_DocReadyState	= 6,
	VSWBDI_DocSize	= 7,
	VSWBDI_DocLastCtxMenuPos	= 8
    } ;
typedef DWORD VSWBDOCINFOINDEX;



extern RPC_IF_HANDLE __MIDL_itf_vsbrowse_0000_0005_v0_0_c_ifspec;
extern RPC_IF_HANDLE __MIDL_itf_vsbrowse_0000_0005_v0_0_s_ifspec;

#ifndef __IVsWebBrowser_INTERFACE_DEFINED__
#define __IVsWebBrowser_INTERFACE_DEFINED__

/* interface IVsWebBrowser */
/* [object][unique][version][uuid] */ 


EXTERN_C const IID IID_IVsWebBrowser;

#if defined(__cplusplus) && !defined(CINTERFACE)
    
    MIDL_INTERFACE("e8b06f50-6d01-11d2-aa7d-00c04f990343")
    IVsWebBrowser : public IUnknown
    {
    public:
        virtual HRESULT STDMETHODCALLTYPE Navigate( 
            /* [in] */ DWORD dwFlags,
            /* [in] */ __RPC__in LPCOLESTR lpszURL) = 0;
        
        virtual HRESULT STDMETHODCALLTYPE NavigateEx( 
            /* [in] */ DWORD dwFlags,
            /* [in] */ __RPC__in LPCOLESTR lpszURL,
            /* [in] */ __RPC__in VARIANT *pvarTargetFrame,
            /* [in] */ __RPC__in VARIANT *pvarPostData,
            /* [in] */ __RPC__in VARIANT *pvarHeaders) = 0;
        
        virtual HRESULT STDMETHODCALLTYPE Stop( void) = 0;
        
        virtual HRESULT STDMETHODCALLTYPE Refresh( 
            /* [in] */ VSWBREFRESHTYPE dwRefreshType) = 0;
        
        virtual HRESULT STDMETHODCALLTYPE GetDocumentInfo( 
            /* [in] */ VSWBDOCINFOINDEX dwInfoIndex,
            /* [out] */ __RPC__out VARIANT *pvarInfo) = 0;
        
    };
    
#else 	/* C style interface */

    typedef struct IVsWebBrowserVtbl
    {
        BEGIN_INTERFACE
        
        HRESULT ( STDMETHODCALLTYPE *QueryInterface )( 
            IVsWebBrowser * This,
            /* [in] */ __RPC__in REFIID riid,
            /* [iid_is][out] */ 
            __RPC__deref_out  void **ppvObject);
        
        ULONG ( STDMETHODCALLTYPE *AddRef )( 
            IVsWebBrowser * This);
        
        ULONG ( STDMETHODCALLTYPE *Release )( 
            IVsWebBrowser * This);
        
        HRESULT ( STDMETHODCALLTYPE *Navigate )( 
            IVsWebBrowser * This,
            /* [in] */ DWORD dwFlags,
            /* [in] */ __RPC__in LPCOLESTR lpszURL);
        
        HRESULT ( STDMETHODCALLTYPE *NavigateEx )( 
            IVsWebBrowser * This,
            /* [in] */ DWORD dwFlags,
            /* [in] */ __RPC__in LPCOLESTR lpszURL,
            /* [in] */ __RPC__in VARIANT *pvarTargetFrame,
            /* [in] */ __RPC__in VARIANT *pvarPostData,
            /* [in] */ __RPC__in VARIANT *pvarHeaders);
        
        HRESULT ( STDMETHODCALLTYPE *Stop )( 
            IVsWebBrowser * This);
        
        HRESULT ( STDMETHODCALLTYPE *Refresh )( 
            IVsWebBrowser * This,
            /* [in] */ VSWBREFRESHTYPE dwRefreshType);
        
        HRESULT ( STDMETHODCALLTYPE *GetDocumentInfo )( 
            IVsWebBrowser * This,
            /* [in] */ VSWBDOCINFOINDEX dwInfoIndex,
            /* [out] */ __RPC__out VARIANT *pvarInfo);
        
        END_INTERFACE
    } IVsWebBrowserVtbl;

    interface IVsWebBrowser
    {
        CONST_VTBL struct IVsWebBrowserVtbl *lpVtbl;
    };

    

#ifdef COBJMACROS


#define IVsWebBrowser_QueryInterface(This,riid,ppvObject)	\
    ( (This)->lpVtbl -> QueryInterface(This,riid,ppvObject) ) 

#define IVsWebBrowser_AddRef(This)	\
    ( (This)->lpVtbl -> AddRef(This) ) 

#define IVsWebBrowser_Release(This)	\
    ( (This)->lpVtbl -> Release(This) ) 


#define IVsWebBrowser_Navigate(This,dwFlags,lpszURL)	\
    ( (This)->lpVtbl -> Navigate(This,dwFlags,lpszURL) ) 

#define IVsWebBrowser_NavigateEx(This,dwFlags,lpszURL,pvarTargetFrame,pvarPostData,pvarHeaders)	\
    ( (This)->lpVtbl -> NavigateEx(This,dwFlags,lpszURL,pvarTargetFrame,pvarPostData,pvarHeaders) ) 

#define IVsWebBrowser_Stop(This)	\
    ( (This)->lpVtbl -> Stop(This) ) 

#define IVsWebBrowser_Refresh(This,dwRefreshType)	\
    ( (This)->lpVtbl -> Refresh(This,dwRefreshType) ) 

#define IVsWebBrowser_GetDocumentInfo(This,dwInfoIndex,pvarInfo)	\
    ( (This)->lpVtbl -> GetDocumentInfo(This,dwInfoIndex,pvarInfo) ) 

#endif /* COBJMACROS */


#endif 	/* C style interface */




#endif 	/* __IVsWebBrowser_INTERFACE_DEFINED__ */


/* interface __MIDL_itf_vsbrowse_0000_0006 */
/* [local] */ 


enum __VSURLZONE
    {	URLZONE_VSAPP	= URLZONE_USER_MIN
    } ;


extern RPC_IF_HANDLE __MIDL_itf_vsbrowse_0000_0006_v0_0_c_ifspec;
extern RPC_IF_HANDLE __MIDL_itf_vsbrowse_0000_0006_v0_0_s_ifspec;

#ifndef __IVsWebPreviewAction_INTERFACE_DEFINED__
#define __IVsWebPreviewAction_INTERFACE_DEFINED__

/* interface IVsWebPreviewAction */
/* [object][helpstring][version][uuid] */ 


EXTERN_C const IID IID_IVsWebPreviewAction;

#if defined(__cplusplus) && !defined(CINTERFACE)
    
    MIDL_INTERFACE("9EC9BA56-B328-11d2-9A98-00C04F79EFC3")
    IVsWebPreviewAction : public IUnknown
    {
    public:
        virtual HRESULT STDMETHODCALLTYPE OnPreviewLoadStart( void) = 0;
        
        virtual HRESULT STDMETHODCALLTYPE OnPreviewClose( void) = 0;
        
        virtual HRESULT STDMETHODCALLTYPE OnPreviewLoaded( 
            /* [in] */ __RPC__in_opt IDispatch *pDispDocument) = 0;
        
    };
    
#else 	/* C style interface */

    typedef struct IVsWebPreviewActionVtbl
    {
        BEGIN_INTERFACE
        
        HRESULT ( STDMETHODCALLTYPE *QueryInterface )( 
            IVsWebPreviewAction * This,
            /* [in] */ __RPC__in REFIID riid,
            /* [iid_is][out] */ 
            __RPC__deref_out  void **ppvObject);
        
        ULONG ( STDMETHODCALLTYPE *AddRef )( 
            IVsWebPreviewAction * This);
        
        ULONG ( STDMETHODCALLTYPE *Release )( 
            IVsWebPreviewAction * This);
        
        HRESULT ( STDMETHODCALLTYPE *OnPreviewLoadStart )( 
            IVsWebPreviewAction * This);
        
        HRESULT ( STDMETHODCALLTYPE *OnPreviewClose )( 
            IVsWebPreviewAction * This);
        
        HRESULT ( STDMETHODCALLTYPE *OnPreviewLoaded )( 
            IVsWebPreviewAction * This,
            /* [in] */ __RPC__in_opt IDispatch *pDispDocument);
        
        END_INTERFACE
    } IVsWebPreviewActionVtbl;

    interface IVsWebPreviewAction
    {
        CONST_VTBL struct IVsWebPreviewActionVtbl *lpVtbl;
    };

    

#ifdef COBJMACROS


#define IVsWebPreviewAction_QueryInterface(This,riid,ppvObject)	\
    ( (This)->lpVtbl -> QueryInterface(This,riid,ppvObject) ) 

#define IVsWebPreviewAction_AddRef(This)	\
    ( (This)->lpVtbl -> AddRef(This) ) 

#define IVsWebPreviewAction_Release(This)	\
    ( (This)->lpVtbl -> Release(This) ) 


#define IVsWebPreviewAction_OnPreviewLoadStart(This)	\
    ( (This)->lpVtbl -> OnPreviewLoadStart(This) ) 

#define IVsWebPreviewAction_OnPreviewClose(This)	\
    ( (This)->lpVtbl -> OnPreviewClose(This) ) 

#define IVsWebPreviewAction_OnPreviewLoaded(This,pDispDocument)	\
    ( (This)->lpVtbl -> OnPreviewLoaded(This,pDispDocument) ) 

#endif /* COBJMACROS */


#endif 	/* C style interface */




#endif 	/* __IVsWebPreviewAction_INTERFACE_DEFINED__ */


/* interface __MIDL_itf_vsbrowse_0000_0007 */
/* [local] */ 


enum __VSWBPREVIEWOPTIONS
    {	VSWBP_FrameMdiChild	= VSCWB_FrameMdiChild,
	VSWBP_FrameFloat	= VSCWB_FrameFloat,
	VSWBP_FrameDock	= VSCWB_FrameDock
    } ;
typedef DWORD VSWBPREVIEWOPTIONS;



extern RPC_IF_HANDLE __MIDL_itf_vsbrowse_0000_0007_v0_0_c_ifspec;
extern RPC_IF_HANDLE __MIDL_itf_vsbrowse_0000_0007_v0_0_s_ifspec;

#ifndef __IVsWebPreview_INTERFACE_DEFINED__
#define __IVsWebPreview_INTERFACE_DEFINED__

/* interface IVsWebPreview */
/* [object][helpstring][version][uuid] */ 


EXTERN_C const IID IID_IVsWebPreview;

#if defined(__cplusplus) && !defined(CINTERFACE)
    
    MIDL_INTERFACE("9EC9BA55-B328-11d2-9A98-00C04F79EFC3")
    IVsWebPreview : public IUnknown
    {
    public:
        virtual HRESULT STDMETHODCALLTYPE PreviewURL( 
            /* [in] */ __RPC__in_opt IVsWebPreviewAction *pAction,
            /* [in] */ __RPC__in LPCOLESTR lpszURL) = 0;
        
        virtual HRESULT STDMETHODCALLTYPE PreviewURLEx( 
            /* [in] */ __RPC__in_opt IVsWebPreviewAction *pAction,
            /* [in] */ __RPC__in LPCOLESTR lpszURL,
            /* [in] */ VSWBPREVIEWOPTIONS opt,
            /* [in] */ int cx,
            /* [in] */ int cy) = 0;
        
        virtual HRESULT STDMETHODCALLTYPE ActivatePreview( void) = 0;
        
        virtual HRESULT STDMETHODCALLTYPE Resize( 
            /* [in] */ int cx,
            /* [in] */ int cy) = 0;
        
    };
    
#else 	/* C style interface */

    typedef struct IVsWebPreviewVtbl
    {
        BEGIN_INTERFACE
        
        HRESULT ( STDMETHODCALLTYPE *QueryInterface )( 
            IVsWebPreview * This,
            /* [in] */ __RPC__in REFIID riid,
            /* [iid_is][out] */ 
            __RPC__deref_out  void **ppvObject);
        
        ULONG ( STDMETHODCALLTYPE *AddRef )( 
            IVsWebPreview * This);
        
        ULONG ( STDMETHODCALLTYPE *Release )( 
            IVsWebPreview * This);
        
        HRESULT ( STDMETHODCALLTYPE *PreviewURL )( 
            IVsWebPreview * This,
            /* [in] */ __RPC__in_opt IVsWebPreviewAction *pAction,
            /* [in] */ __RPC__in LPCOLESTR lpszURL);
        
        HRESULT ( STDMETHODCALLTYPE *PreviewURLEx )( 
            IVsWebPreview * This,
            /* [in] */ __RPC__in_opt IVsWebPreviewAction *pAction,
            /* [in] */ __RPC__in LPCOLESTR lpszURL,
            /* [in] */ VSWBPREVIEWOPTIONS opt,
            /* [in] */ int cx,
            /* [in] */ int cy);
        
        HRESULT ( STDMETHODCALLTYPE *ActivatePreview )( 
            IVsWebPreview * This);
        
        HRESULT ( STDMETHODCALLTYPE *Resize )( 
            IVsWebPreview * This,
            /* [in] */ int cx,
            /* [in] */ int cy);
        
        END_INTERFACE
    } IVsWebPreviewVtbl;

    interface IVsWebPreview
    {
        CONST_VTBL struct IVsWebPreviewVtbl *lpVtbl;
    };

    

#ifdef COBJMACROS


#define IVsWebPreview_QueryInterface(This,riid,ppvObject)	\
    ( (This)->lpVtbl -> QueryInterface(This,riid,ppvObject) ) 

#define IVsWebPreview_AddRef(This)	\
    ( (This)->lpVtbl -> AddRef(This) ) 

#define IVsWebPreview_Release(This)	\
    ( (This)->lpVtbl -> Release(This) ) 


#define IVsWebPreview_PreviewURL(This,pAction,lpszURL)	\
    ( (This)->lpVtbl -> PreviewURL(This,pAction,lpszURL) ) 

#define IVsWebPreview_PreviewURLEx(This,pAction,lpszURL,opt,cx,cy)	\
    ( (This)->lpVtbl -> PreviewURLEx(This,pAction,lpszURL,opt,cx,cy) ) 

#define IVsWebPreview_ActivatePreview(This)	\
    ( (This)->lpVtbl -> ActivatePreview(This) ) 

#define IVsWebPreview_Resize(This,cx,cy)	\
    ( (This)->lpVtbl -> Resize(This,cx,cy) ) 

#endif /* COBJMACROS */


#endif 	/* C style interface */




#endif 	/* __IVsWebPreview_INTERFACE_DEFINED__ */


/* interface __MIDL_itf_vsbrowse_0000_0008 */
/* [local] */ 

#define SID_SVsWebPreview IID_IVsWebPreview


extern RPC_IF_HANDLE __MIDL_itf_vsbrowse_0000_0008_v0_0_c_ifspec;
extern RPC_IF_HANDLE __MIDL_itf_vsbrowse_0000_0008_v0_0_s_ifspec;

/* Additional Prototypes for ALL interfaces */

unsigned long             __RPC_USER  BSTR_UserSize(     unsigned long *, unsigned long            , BSTR * ); 
unsigned char * __RPC_USER  BSTR_UserMarshal(  unsigned long *, unsigned char *, BSTR * ); 
unsigned char * __RPC_USER  BSTR_UserUnmarshal(unsigned long *, unsigned char *, BSTR * ); 
void                      __RPC_USER  BSTR_UserFree(     unsigned long *, BSTR * ); 

unsigned long             __RPC_USER  HWND_UserSize(     unsigned long *, unsigned long            , HWND * ); 
unsigned char * __RPC_USER  HWND_UserMarshal(  unsigned long *, unsigned char *, HWND * ); 
unsigned char * __RPC_USER  HWND_UserUnmarshal(unsigned long *, unsigned char *, HWND * ); 
void                      __RPC_USER  HWND_UserFree(     unsigned long *, HWND * ); 

unsigned long             __RPC_USER  VARIANT_UserSize(     unsigned long *, unsigned long            , VARIANT * ); 
unsigned char * __RPC_USER  VARIANT_UserMarshal(  unsigned long *, unsigned char *, VARIANT * ); 
unsigned char * __RPC_USER  VARIANT_UserUnmarshal(unsigned long *, unsigned char *, VARIANT * ); 
void                      __RPC_USER  VARIANT_UserFree(     unsigned long *, VARIANT * ); 

/* end of Additional Prototypes */

#ifdef __cplusplus
}
#endif

#endif


