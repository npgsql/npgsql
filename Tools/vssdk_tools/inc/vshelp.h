

/* this ALWAYS GENERATED file contains the definitions for the interfaces */


 /* File created by MIDL compiler version 7.00.0499 */
/* Compiler settings for vshelp.idl:
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


#ifndef __vshelp_h__
#define __vshelp_h__

#if defined(_MSC_VER) && (_MSC_VER >= 1020)
#pragma once
#endif

/* Forward Declarations */ 

#ifndef __IVsHelpOwner_FWD_DEFINED__
#define __IVsHelpOwner_FWD_DEFINED__
typedef interface IVsHelpOwner IVsHelpOwner;
#endif 	/* __IVsHelpOwner_FWD_DEFINED__ */


#ifndef __IVsHelpTopicShowEvents_FWD_DEFINED__
#define __IVsHelpTopicShowEvents_FWD_DEFINED__
typedef interface IVsHelpTopicShowEvents IVsHelpTopicShowEvents;
#endif 	/* __IVsHelpTopicShowEvents_FWD_DEFINED__ */


#ifndef __Help_FWD_DEFINED__
#define __Help_FWD_DEFINED__
typedef interface Help Help;
#endif 	/* __Help_FWD_DEFINED__ */


#ifndef __IVsHelpEvents_FWD_DEFINED__
#define __IVsHelpEvents_FWD_DEFINED__
typedef interface IVsHelpEvents IVsHelpEvents;
#endif 	/* __IVsHelpEvents_FWD_DEFINED__ */


#ifndef __DExploreAppObj_FWD_DEFINED__
#define __DExploreAppObj_FWD_DEFINED__

#ifdef __cplusplus
typedef class DExploreAppObj DExploreAppObj;
#else
typedef struct DExploreAppObj DExploreAppObj;
#endif /* __cplusplus */

#endif 	/* __DExploreAppObj_FWD_DEFINED__ */


/* header files for imported files */
#include "oaidl.h"
#include "ocidl.h"

#ifdef __cplusplus
extern "C"{
#endif 



#ifndef __VsHelp_LIBRARY_DEFINED__
#define __VsHelp_LIBRARY_DEFINED__

/* library VsHelp */
/* [version][uuid] */ 


EXTERN_C const IID LIBID_VsHelp;

#ifndef __IVsHelpOwner_INTERFACE_DEFINED__
#define __IVsHelpOwner_INTERFACE_DEFINED__

/* interface IVsHelpOwner */
/* [version][helpstring][uuid][object][oleautomation][dual] */ 


EXTERN_C const IID IID_IVsHelpOwner;

#if defined(__cplusplus) && !defined(CINTERFACE)
    
    MIDL_INTERFACE("B9B0983A-364C-4866-873F-D5ED190138FB")
    IVsHelpOwner : public IDispatch
    {
    public:
        virtual /* [id] */ HRESULT STDMETHODCALLTYPE BringHelpToTop( 
            /* [in] */ long hwndHelpApp) = 0;
        
        virtual /* [id][propget] */ HRESULT STDMETHODCALLTYPE get_AutomationObject( 
            /* [retval][out] */ __RPC__deref_out_opt IDispatch **ppObj) = 0;
        
    };
    
#else 	/* C style interface */

    typedef struct IVsHelpOwnerVtbl
    {
        BEGIN_INTERFACE
        
        HRESULT ( STDMETHODCALLTYPE *QueryInterface )( 
            IVsHelpOwner * This,
            /* [in] */ __RPC__in REFIID riid,
            /* [iid_is][out] */ 
            __RPC__deref_out  void **ppvObject);
        
        ULONG ( STDMETHODCALLTYPE *AddRef )( 
            IVsHelpOwner * This);
        
        ULONG ( STDMETHODCALLTYPE *Release )( 
            IVsHelpOwner * This);
        
        HRESULT ( STDMETHODCALLTYPE *GetTypeInfoCount )( 
            IVsHelpOwner * This,
            /* [out] */ __RPC__out UINT *pctinfo);
        
        HRESULT ( STDMETHODCALLTYPE *GetTypeInfo )( 
            IVsHelpOwner * This,
            /* [in] */ UINT iTInfo,
            /* [in] */ LCID lcid,
            /* [out] */ __RPC__deref_out_opt ITypeInfo **ppTInfo);
        
        HRESULT ( STDMETHODCALLTYPE *GetIDsOfNames )( 
            IVsHelpOwner * This,
            /* [in] */ __RPC__in REFIID riid,
            /* [size_is][in] */ __RPC__in_ecount_full(cNames) LPOLESTR *rgszNames,
            /* [range][in] */ UINT cNames,
            /* [in] */ LCID lcid,
            /* [size_is][out] */ __RPC__out_ecount_full(cNames) DISPID *rgDispId);
        
        /* [local] */ HRESULT ( STDMETHODCALLTYPE *Invoke )( 
            IVsHelpOwner * This,
            /* [in] */ DISPID dispIdMember,
            /* [in] */ REFIID riid,
            /* [in] */ LCID lcid,
            /* [in] */ WORD wFlags,
            /* [out][in] */ DISPPARAMS *pDispParams,
            /* [out] */ VARIANT *pVarResult,
            /* [out] */ EXCEPINFO *pExcepInfo,
            /* [out] */ UINT *puArgErr);
        
        /* [id] */ HRESULT ( STDMETHODCALLTYPE *BringHelpToTop )( 
            IVsHelpOwner * This,
            /* [in] */ long hwndHelpApp);
        
        /* [id][propget] */ HRESULT ( STDMETHODCALLTYPE *get_AutomationObject )( 
            IVsHelpOwner * This,
            /* [retval][out] */ __RPC__deref_out_opt IDispatch **ppObj);
        
        END_INTERFACE
    } IVsHelpOwnerVtbl;

    interface IVsHelpOwner
    {
        CONST_VTBL struct IVsHelpOwnerVtbl *lpVtbl;
    };

    

#ifdef COBJMACROS


#define IVsHelpOwner_QueryInterface(This,riid,ppvObject)	\
    ( (This)->lpVtbl -> QueryInterface(This,riid,ppvObject) ) 

#define IVsHelpOwner_AddRef(This)	\
    ( (This)->lpVtbl -> AddRef(This) ) 

#define IVsHelpOwner_Release(This)	\
    ( (This)->lpVtbl -> Release(This) ) 


#define IVsHelpOwner_GetTypeInfoCount(This,pctinfo)	\
    ( (This)->lpVtbl -> GetTypeInfoCount(This,pctinfo) ) 

#define IVsHelpOwner_GetTypeInfo(This,iTInfo,lcid,ppTInfo)	\
    ( (This)->lpVtbl -> GetTypeInfo(This,iTInfo,lcid,ppTInfo) ) 

#define IVsHelpOwner_GetIDsOfNames(This,riid,rgszNames,cNames,lcid,rgDispId)	\
    ( (This)->lpVtbl -> GetIDsOfNames(This,riid,rgszNames,cNames,lcid,rgDispId) ) 

#define IVsHelpOwner_Invoke(This,dispIdMember,riid,lcid,wFlags,pDispParams,pVarResult,pExcepInfo,puArgErr)	\
    ( (This)->lpVtbl -> Invoke(This,dispIdMember,riid,lcid,wFlags,pDispParams,pVarResult,pExcepInfo,puArgErr) ) 


#define IVsHelpOwner_BringHelpToTop(This,hwndHelpApp)	\
    ( (This)->lpVtbl -> BringHelpToTop(This,hwndHelpApp) ) 

#define IVsHelpOwner_get_AutomationObject(This,ppObj)	\
    ( (This)->lpVtbl -> get_AutomationObject(This,ppObj) ) 

#endif /* COBJMACROS */


#endif 	/* C style interface */




#endif 	/* __IVsHelpOwner_INTERFACE_DEFINED__ */


#ifndef __IVsHelpTopicShowEvents_INTERFACE_DEFINED__
#define __IVsHelpTopicShowEvents_INTERFACE_DEFINED__

/* interface IVsHelpTopicShowEvents */
/* [object][helpstring][uuid] */ 


EXTERN_C const IID IID_IVsHelpTopicShowEvents;

#if defined(__cplusplus) && !defined(CINTERFACE)
    
    MIDL_INTERFACE("D1AAC64A-6A25-4274-B2C6-BC3B840B6E54")
    IVsHelpTopicShowEvents : public IDispatch
    {
    public:
        virtual /* [id] */ HRESULT STDMETHODCALLTYPE OnBeforeTopicShow( 
            /* [in] */ __RPC__in BSTR bstrURL,
            /* [in] */ __RPC__in_opt IDispatch *pWB) = 0;
        
        virtual /* [id] */ HRESULT STDMETHODCALLTYPE OnTopicShowComplete( 
            /* [in] */ __RPC__in BSTR bstrURL,
            /* [in] */ __RPC__in_opt IDispatch *pWB) = 0;
        
    };
    
#else 	/* C style interface */

    typedef struct IVsHelpTopicShowEventsVtbl
    {
        BEGIN_INTERFACE
        
        HRESULT ( STDMETHODCALLTYPE *QueryInterface )( 
            IVsHelpTopicShowEvents * This,
            /* [in] */ __RPC__in REFIID riid,
            /* [iid_is][out] */ 
            __RPC__deref_out  void **ppvObject);
        
        ULONG ( STDMETHODCALLTYPE *AddRef )( 
            IVsHelpTopicShowEvents * This);
        
        ULONG ( STDMETHODCALLTYPE *Release )( 
            IVsHelpTopicShowEvents * This);
        
        HRESULT ( STDMETHODCALLTYPE *GetTypeInfoCount )( 
            IVsHelpTopicShowEvents * This,
            /* [out] */ __RPC__out UINT *pctinfo);
        
        HRESULT ( STDMETHODCALLTYPE *GetTypeInfo )( 
            IVsHelpTopicShowEvents * This,
            /* [in] */ UINT iTInfo,
            /* [in] */ LCID lcid,
            /* [out] */ __RPC__deref_out_opt ITypeInfo **ppTInfo);
        
        HRESULT ( STDMETHODCALLTYPE *GetIDsOfNames )( 
            IVsHelpTopicShowEvents * This,
            /* [in] */ __RPC__in REFIID riid,
            /* [size_is][in] */ __RPC__in_ecount_full(cNames) LPOLESTR *rgszNames,
            /* [range][in] */ UINT cNames,
            /* [in] */ LCID lcid,
            /* [size_is][out] */ __RPC__out_ecount_full(cNames) DISPID *rgDispId);
        
        /* [local] */ HRESULT ( STDMETHODCALLTYPE *Invoke )( 
            IVsHelpTopicShowEvents * This,
            /* [in] */ DISPID dispIdMember,
            /* [in] */ REFIID riid,
            /* [in] */ LCID lcid,
            /* [in] */ WORD wFlags,
            /* [out][in] */ DISPPARAMS *pDispParams,
            /* [out] */ VARIANT *pVarResult,
            /* [out] */ EXCEPINFO *pExcepInfo,
            /* [out] */ UINT *puArgErr);
        
        /* [id] */ HRESULT ( STDMETHODCALLTYPE *OnBeforeTopicShow )( 
            IVsHelpTopicShowEvents * This,
            /* [in] */ __RPC__in BSTR bstrURL,
            /* [in] */ __RPC__in_opt IDispatch *pWB);
        
        /* [id] */ HRESULT ( STDMETHODCALLTYPE *OnTopicShowComplete )( 
            IVsHelpTopicShowEvents * This,
            /* [in] */ __RPC__in BSTR bstrURL,
            /* [in] */ __RPC__in_opt IDispatch *pWB);
        
        END_INTERFACE
    } IVsHelpTopicShowEventsVtbl;

    interface IVsHelpTopicShowEvents
    {
        CONST_VTBL struct IVsHelpTopicShowEventsVtbl *lpVtbl;
    };

    

#ifdef COBJMACROS


#define IVsHelpTopicShowEvents_QueryInterface(This,riid,ppvObject)	\
    ( (This)->lpVtbl -> QueryInterface(This,riid,ppvObject) ) 

#define IVsHelpTopicShowEvents_AddRef(This)	\
    ( (This)->lpVtbl -> AddRef(This) ) 

#define IVsHelpTopicShowEvents_Release(This)	\
    ( (This)->lpVtbl -> Release(This) ) 


#define IVsHelpTopicShowEvents_GetTypeInfoCount(This,pctinfo)	\
    ( (This)->lpVtbl -> GetTypeInfoCount(This,pctinfo) ) 

#define IVsHelpTopicShowEvents_GetTypeInfo(This,iTInfo,lcid,ppTInfo)	\
    ( (This)->lpVtbl -> GetTypeInfo(This,iTInfo,lcid,ppTInfo) ) 

#define IVsHelpTopicShowEvents_GetIDsOfNames(This,riid,rgszNames,cNames,lcid,rgDispId)	\
    ( (This)->lpVtbl -> GetIDsOfNames(This,riid,rgszNames,cNames,lcid,rgDispId) ) 

#define IVsHelpTopicShowEvents_Invoke(This,dispIdMember,riid,lcid,wFlags,pDispParams,pVarResult,pExcepInfo,puArgErr)	\
    ( (This)->lpVtbl -> Invoke(This,dispIdMember,riid,lcid,wFlags,pDispParams,pVarResult,pExcepInfo,puArgErr) ) 


#define IVsHelpTopicShowEvents_OnBeforeTopicShow(This,bstrURL,pWB)	\
    ( (This)->lpVtbl -> OnBeforeTopicShow(This,bstrURL,pWB) ) 

#define IVsHelpTopicShowEvents_OnTopicShowComplete(This,bstrURL,pWB)	\
    ( (This)->lpVtbl -> OnTopicShowComplete(This,bstrURL,pWB) ) 

#endif /* COBJMACROS */


#endif 	/* C style interface */




#endif 	/* __IVsHelpTopicShowEvents_INTERFACE_DEFINED__ */


#ifndef __Help_INTERFACE_DEFINED__
#define __Help_INTERFACE_DEFINED__

/* interface Help */
/* [uuid][object][oleautomation][dual] */ 


EXTERN_C const IID IID_Help;

#if defined(__cplusplus) && !defined(CINTERFACE)
    
    MIDL_INTERFACE("4A791148-19E4-11d3-B86B-00C04F79F802")
    Help : public IDispatch
    {
    public:
        virtual /* [id] */ HRESULT STDMETHODCALLTYPE Contents( void) = 0;
        
        virtual /* [id] */ HRESULT STDMETHODCALLTYPE Index( void) = 0;
        
        virtual /* [id] */ HRESULT STDMETHODCALLTYPE Search( void) = 0;
        
        virtual /* [id] */ HRESULT STDMETHODCALLTYPE IndexResults( void) = 0;
        
        virtual /* [id] */ HRESULT STDMETHODCALLTYPE SearchResults( void) = 0;
        
        virtual /* [id] */ HRESULT STDMETHODCALLTYPE DisplayTopicFromId( 
            /* [in] */ __RPC__in BSTR bstrFile,
            /* [in] */ DWORD Id) = 0;
        
        virtual /* [id] */ HRESULT STDMETHODCALLTYPE DisplayTopicFromURL( 
            /* [in] */ __RPC__in BSTR pszURL) = 0;
        
        virtual /* [id] */ HRESULT STDMETHODCALLTYPE DisplayTopicFromURLEx( 
            /* [in] */ __RPC__in BSTR pszURL,
            /* [in] */ __RPC__in_opt IVsHelpTopicShowEvents *pIVsHelpTopicShowEvents) = 0;
        
        virtual /* [id] */ HRESULT STDMETHODCALLTYPE DisplayTopicFromKeyword( 
            /* [in] */ __RPC__in BSTR pszKeyword) = 0;
        
        virtual /* [id] */ HRESULT STDMETHODCALLTYPE DisplayTopicFromF1Keyword( 
            /* [in] */ __RPC__in BSTR pszKeyword) = 0;
        
        virtual /* [id] */ HRESULT STDMETHODCALLTYPE DisplayTopicFrom_OLD_Help( 
            /* [in] */ __RPC__in BSTR bstrFile,
            /* [in] */ DWORD Id) = 0;
        
        virtual /* [id] */ HRESULT STDMETHODCALLTYPE SyncContents( 
            /* [in] */ __RPC__in BSTR bstrURL) = 0;
        
        virtual /* [id] */ HRESULT STDMETHODCALLTYPE CanSyncContents( 
            /* [in] */ __RPC__in BSTR bstrURL) = 0;
        
        virtual /* [id] */ HRESULT STDMETHODCALLTYPE GetNextTopic( 
            /* [in] */ __RPC__in BSTR bstrURL,
            /* [retval][out] */ __RPC__deref_out_opt BSTR *pbstrNext) = 0;
        
        virtual /* [id] */ HRESULT STDMETHODCALLTYPE GetPrevTopic( 
            /* [in] */ __RPC__in BSTR bstrURL,
            /* [retval][out] */ __RPC__deref_out_opt BSTR *pbstrPrev) = 0;
        
        virtual /* [id] */ HRESULT STDMETHODCALLTYPE FilterUI( void) = 0;
        
        virtual /* [id] */ HRESULT STDMETHODCALLTYPE CanShowFilterUI( void) = 0;
        
        virtual /* [id] */ HRESULT STDMETHODCALLTYPE Close( void) = 0;
        
        virtual /* [id] */ HRESULT STDMETHODCALLTYPE SyncIndex( 
            /* [in] */ __RPC__in BSTR bstrKeyword,
            /* [in] */ BOOL fShow) = 0;
        
        virtual /* [id] */ HRESULT STDMETHODCALLTYPE SetCollection( 
            /* [in] */ __RPC__in BSTR bstrCollection,
            /* [in] */ __RPC__in BSTR bstrFilter) = 0;
        
        virtual /* [id][propget] */ HRESULT STDMETHODCALLTYPE get_Collection( 
            /* [retval][out] */ __RPC__deref_out_opt BSTR *pbstrCollection) = 0;
        
        virtual /* [id][propget] */ HRESULT STDMETHODCALLTYPE get_Filter( 
            /* [retval][out] */ __RPC__deref_out_opt BSTR *pbstrFilter) = 0;
        
        virtual /* [id][propput] */ HRESULT STDMETHODCALLTYPE put_Filter( 
            /* [in] */ __RPC__in BSTR bstrFilter) = 0;
        
        virtual /* [id][propget] */ HRESULT STDMETHODCALLTYPE get_FilterQuery( 
            /* [retval][out] */ __RPC__deref_out_opt BSTR *pbstrFilterQuery) = 0;
        
        virtual /* [id][propget] */ HRESULT STDMETHODCALLTYPE get_HelpOwner( 
            /* [retval][out] */ __RPC__deref_out_opt IVsHelpOwner **ppObj) = 0;
        
        virtual /* [id][propput] */ HRESULT STDMETHODCALLTYPE put_HelpOwner( 
            /* [in] */ __RPC__in_opt IVsHelpOwner *pObj) = 0;
        
        virtual /* [id][propget] */ HRESULT STDMETHODCALLTYPE get_HxSession( 
            /* [retval][out] */ __RPC__deref_out_opt IDispatch **ppObj) = 0;
        
        virtual /* [id][propget] */ HRESULT STDMETHODCALLTYPE get_Help( 
            /* [retval][out] */ __RPC__deref_out_opt IDispatch **ppObj) = 0;
        
        virtual /* [id] */ HRESULT STDMETHODCALLTYPE GetObject( 
            /* [in] */ __RPC__in BSTR bstrMoniker,
            /* [in] */ __RPC__in BSTR bstrOptions,
            /* [retval][out] */ __RPC__deref_out_opt IDispatch **ppDisp) = 0;
        
    };
    
#else 	/* C style interface */

    typedef struct HelpVtbl
    {
        BEGIN_INTERFACE
        
        HRESULT ( STDMETHODCALLTYPE *QueryInterface )( 
            Help * This,
            /* [in] */ __RPC__in REFIID riid,
            /* [iid_is][out] */ 
            __RPC__deref_out  void **ppvObject);
        
        ULONG ( STDMETHODCALLTYPE *AddRef )( 
            Help * This);
        
        ULONG ( STDMETHODCALLTYPE *Release )( 
            Help * This);
        
        HRESULT ( STDMETHODCALLTYPE *GetTypeInfoCount )( 
            Help * This,
            /* [out] */ __RPC__out UINT *pctinfo);
        
        HRESULT ( STDMETHODCALLTYPE *GetTypeInfo )( 
            Help * This,
            /* [in] */ UINT iTInfo,
            /* [in] */ LCID lcid,
            /* [out] */ __RPC__deref_out_opt ITypeInfo **ppTInfo);
        
        HRESULT ( STDMETHODCALLTYPE *GetIDsOfNames )( 
            Help * This,
            /* [in] */ __RPC__in REFIID riid,
            /* [size_is][in] */ __RPC__in_ecount_full(cNames) LPOLESTR *rgszNames,
            /* [range][in] */ UINT cNames,
            /* [in] */ LCID lcid,
            /* [size_is][out] */ __RPC__out_ecount_full(cNames) DISPID *rgDispId);
        
        /* [local] */ HRESULT ( STDMETHODCALLTYPE *Invoke )( 
            Help * This,
            /* [in] */ DISPID dispIdMember,
            /* [in] */ REFIID riid,
            /* [in] */ LCID lcid,
            /* [in] */ WORD wFlags,
            /* [out][in] */ DISPPARAMS *pDispParams,
            /* [out] */ VARIANT *pVarResult,
            /* [out] */ EXCEPINFO *pExcepInfo,
            /* [out] */ UINT *puArgErr);
        
        /* [id] */ HRESULT ( STDMETHODCALLTYPE *Contents )( 
            Help * This);
        
        /* [id] */ HRESULT ( STDMETHODCALLTYPE *Index )( 
            Help * This);
        
        /* [id] */ HRESULT ( STDMETHODCALLTYPE *Search )( 
            Help * This);
        
        /* [id] */ HRESULT ( STDMETHODCALLTYPE *IndexResults )( 
            Help * This);
        
        /* [id] */ HRESULT ( STDMETHODCALLTYPE *SearchResults )( 
            Help * This);
        
        /* [id] */ HRESULT ( STDMETHODCALLTYPE *DisplayTopicFromId )( 
            Help * This,
            /* [in] */ __RPC__in BSTR bstrFile,
            /* [in] */ DWORD Id);
        
        /* [id] */ HRESULT ( STDMETHODCALLTYPE *DisplayTopicFromURL )( 
            Help * This,
            /* [in] */ __RPC__in BSTR pszURL);
        
        /* [id] */ HRESULT ( STDMETHODCALLTYPE *DisplayTopicFromURLEx )( 
            Help * This,
            /* [in] */ __RPC__in BSTR pszURL,
            /* [in] */ __RPC__in_opt IVsHelpTopicShowEvents *pIVsHelpTopicShowEvents);
        
        /* [id] */ HRESULT ( STDMETHODCALLTYPE *DisplayTopicFromKeyword )( 
            Help * This,
            /* [in] */ __RPC__in BSTR pszKeyword);
        
        /* [id] */ HRESULT ( STDMETHODCALLTYPE *DisplayTopicFromF1Keyword )( 
            Help * This,
            /* [in] */ __RPC__in BSTR pszKeyword);
        
        /* [id] */ HRESULT ( STDMETHODCALLTYPE *DisplayTopicFrom_OLD_Help )( 
            Help * This,
            /* [in] */ __RPC__in BSTR bstrFile,
            /* [in] */ DWORD Id);
        
        /* [id] */ HRESULT ( STDMETHODCALLTYPE *SyncContents )( 
            Help * This,
            /* [in] */ __RPC__in BSTR bstrURL);
        
        /* [id] */ HRESULT ( STDMETHODCALLTYPE *CanSyncContents )( 
            Help * This,
            /* [in] */ __RPC__in BSTR bstrURL);
        
        /* [id] */ HRESULT ( STDMETHODCALLTYPE *GetNextTopic )( 
            Help * This,
            /* [in] */ __RPC__in BSTR bstrURL,
            /* [retval][out] */ __RPC__deref_out_opt BSTR *pbstrNext);
        
        /* [id] */ HRESULT ( STDMETHODCALLTYPE *GetPrevTopic )( 
            Help * This,
            /* [in] */ __RPC__in BSTR bstrURL,
            /* [retval][out] */ __RPC__deref_out_opt BSTR *pbstrPrev);
        
        /* [id] */ HRESULT ( STDMETHODCALLTYPE *FilterUI )( 
            Help * This);
        
        /* [id] */ HRESULT ( STDMETHODCALLTYPE *CanShowFilterUI )( 
            Help * This);
        
        /* [id] */ HRESULT ( STDMETHODCALLTYPE *Close )( 
            Help * This);
        
        /* [id] */ HRESULT ( STDMETHODCALLTYPE *SyncIndex )( 
            Help * This,
            /* [in] */ __RPC__in BSTR bstrKeyword,
            /* [in] */ BOOL fShow);
        
        /* [id] */ HRESULT ( STDMETHODCALLTYPE *SetCollection )( 
            Help * This,
            /* [in] */ __RPC__in BSTR bstrCollection,
            /* [in] */ __RPC__in BSTR bstrFilter);
        
        /* [id][propget] */ HRESULT ( STDMETHODCALLTYPE *get_Collection )( 
            Help * This,
            /* [retval][out] */ __RPC__deref_out_opt BSTR *pbstrCollection);
        
        /* [id][propget] */ HRESULT ( STDMETHODCALLTYPE *get_Filter )( 
            Help * This,
            /* [retval][out] */ __RPC__deref_out_opt BSTR *pbstrFilter);
        
        /* [id][propput] */ HRESULT ( STDMETHODCALLTYPE *put_Filter )( 
            Help * This,
            /* [in] */ __RPC__in BSTR bstrFilter);
        
        /* [id][propget] */ HRESULT ( STDMETHODCALLTYPE *get_FilterQuery )( 
            Help * This,
            /* [retval][out] */ __RPC__deref_out_opt BSTR *pbstrFilterQuery);
        
        /* [id][propget] */ HRESULT ( STDMETHODCALLTYPE *get_HelpOwner )( 
            Help * This,
            /* [retval][out] */ __RPC__deref_out_opt IVsHelpOwner **ppObj);
        
        /* [id][propput] */ HRESULT ( STDMETHODCALLTYPE *put_HelpOwner )( 
            Help * This,
            /* [in] */ __RPC__in_opt IVsHelpOwner *pObj);
        
        /* [id][propget] */ HRESULT ( STDMETHODCALLTYPE *get_HxSession )( 
            Help * This,
            /* [retval][out] */ __RPC__deref_out_opt IDispatch **ppObj);
        
        /* [id][propget] */ HRESULT ( STDMETHODCALLTYPE *get_Help )( 
            Help * This,
            /* [retval][out] */ __RPC__deref_out_opt IDispatch **ppObj);
        
        /* [id] */ HRESULT ( STDMETHODCALLTYPE *GetObject )( 
            Help * This,
            /* [in] */ __RPC__in BSTR bstrMoniker,
            /* [in] */ __RPC__in BSTR bstrOptions,
            /* [retval][out] */ __RPC__deref_out_opt IDispatch **ppDisp);
        
        END_INTERFACE
    } HelpVtbl;

    interface Help
    {
        CONST_VTBL struct HelpVtbl *lpVtbl;
    };

    

#ifdef COBJMACROS


#define Help_QueryInterface(This,riid,ppvObject)	\
    ( (This)->lpVtbl -> QueryInterface(This,riid,ppvObject) ) 

#define Help_AddRef(This)	\
    ( (This)->lpVtbl -> AddRef(This) ) 

#define Help_Release(This)	\
    ( (This)->lpVtbl -> Release(This) ) 


#define Help_GetTypeInfoCount(This,pctinfo)	\
    ( (This)->lpVtbl -> GetTypeInfoCount(This,pctinfo) ) 

#define Help_GetTypeInfo(This,iTInfo,lcid,ppTInfo)	\
    ( (This)->lpVtbl -> GetTypeInfo(This,iTInfo,lcid,ppTInfo) ) 

#define Help_GetIDsOfNames(This,riid,rgszNames,cNames,lcid,rgDispId)	\
    ( (This)->lpVtbl -> GetIDsOfNames(This,riid,rgszNames,cNames,lcid,rgDispId) ) 

#define Help_Invoke(This,dispIdMember,riid,lcid,wFlags,pDispParams,pVarResult,pExcepInfo,puArgErr)	\
    ( (This)->lpVtbl -> Invoke(This,dispIdMember,riid,lcid,wFlags,pDispParams,pVarResult,pExcepInfo,puArgErr) ) 


#define Help_Contents(This)	\
    ( (This)->lpVtbl -> Contents(This) ) 

#define Help_Index(This)	\
    ( (This)->lpVtbl -> Index(This) ) 

#define Help_Search(This)	\
    ( (This)->lpVtbl -> Search(This) ) 

#define Help_IndexResults(This)	\
    ( (This)->lpVtbl -> IndexResults(This) ) 

#define Help_SearchResults(This)	\
    ( (This)->lpVtbl -> SearchResults(This) ) 

#define Help_DisplayTopicFromId(This,bstrFile,Id)	\
    ( (This)->lpVtbl -> DisplayTopicFromId(This,bstrFile,Id) ) 

#define Help_DisplayTopicFromURL(This,pszURL)	\
    ( (This)->lpVtbl -> DisplayTopicFromURL(This,pszURL) ) 

#define Help_DisplayTopicFromURLEx(This,pszURL,pIVsHelpTopicShowEvents)	\
    ( (This)->lpVtbl -> DisplayTopicFromURLEx(This,pszURL,pIVsHelpTopicShowEvents) ) 

#define Help_DisplayTopicFromKeyword(This,pszKeyword)	\
    ( (This)->lpVtbl -> DisplayTopicFromKeyword(This,pszKeyword) ) 

#define Help_DisplayTopicFromF1Keyword(This,pszKeyword)	\
    ( (This)->lpVtbl -> DisplayTopicFromF1Keyword(This,pszKeyword) ) 

#define Help_DisplayTopicFrom_OLD_Help(This,bstrFile,Id)	\
    ( (This)->lpVtbl -> DisplayTopicFrom_OLD_Help(This,bstrFile,Id) ) 

#define Help_SyncContents(This,bstrURL)	\
    ( (This)->lpVtbl -> SyncContents(This,bstrURL) ) 

#define Help_CanSyncContents(This,bstrURL)	\
    ( (This)->lpVtbl -> CanSyncContents(This,bstrURL) ) 

#define Help_GetNextTopic(This,bstrURL,pbstrNext)	\
    ( (This)->lpVtbl -> GetNextTopic(This,bstrURL,pbstrNext) ) 

#define Help_GetPrevTopic(This,bstrURL,pbstrPrev)	\
    ( (This)->lpVtbl -> GetPrevTopic(This,bstrURL,pbstrPrev) ) 

#define Help_FilterUI(This)	\
    ( (This)->lpVtbl -> FilterUI(This) ) 

#define Help_CanShowFilterUI(This)	\
    ( (This)->lpVtbl -> CanShowFilterUI(This) ) 

#define Help_Close(This)	\
    ( (This)->lpVtbl -> Close(This) ) 

#define Help_SyncIndex(This,bstrKeyword,fShow)	\
    ( (This)->lpVtbl -> SyncIndex(This,bstrKeyword,fShow) ) 

#define Help_SetCollection(This,bstrCollection,bstrFilter)	\
    ( (This)->lpVtbl -> SetCollection(This,bstrCollection,bstrFilter) ) 

#define Help_get_Collection(This,pbstrCollection)	\
    ( (This)->lpVtbl -> get_Collection(This,pbstrCollection) ) 

#define Help_get_Filter(This,pbstrFilter)	\
    ( (This)->lpVtbl -> get_Filter(This,pbstrFilter) ) 

#define Help_put_Filter(This,bstrFilter)	\
    ( (This)->lpVtbl -> put_Filter(This,bstrFilter) ) 

#define Help_get_FilterQuery(This,pbstrFilterQuery)	\
    ( (This)->lpVtbl -> get_FilterQuery(This,pbstrFilterQuery) ) 

#define Help_get_HelpOwner(This,ppObj)	\
    ( (This)->lpVtbl -> get_HelpOwner(This,ppObj) ) 

#define Help_put_HelpOwner(This,pObj)	\
    ( (This)->lpVtbl -> put_HelpOwner(This,pObj) ) 

#define Help_get_HxSession(This,ppObj)	\
    ( (This)->lpVtbl -> get_HxSession(This,ppObj) ) 

#define Help_get_Help(This,ppObj)	\
    ( (This)->lpVtbl -> get_Help(This,ppObj) ) 

#define Help_GetObject(This,bstrMoniker,bstrOptions,ppDisp)	\
    ( (This)->lpVtbl -> GetObject(This,bstrMoniker,bstrOptions,ppDisp) ) 

#endif /* COBJMACROS */


#endif 	/* C style interface */




#endif 	/* __Help_INTERFACE_DEFINED__ */


#ifndef __IVsHelpEvents_INTERFACE_DEFINED__
#define __IVsHelpEvents_INTERFACE_DEFINED__

/* interface IVsHelpEvents */
/* [object][helpstring][uuid] */ 


EXTERN_C const IID IID_IVsHelpEvents;

#if defined(__cplusplus) && !defined(CINTERFACE)
    
    MIDL_INTERFACE("507E4490-5A8C-11d3-B897-00C04F79F802")
    IVsHelpEvents : public IDispatch
    {
    public:
        virtual /* [id] */ HRESULT STDMETHODCALLTYPE OnFilterChanged( 
            /* [in] */ __RPC__in BSTR bstrNewFilter) = 0;
        
        virtual /* [id] */ HRESULT STDMETHODCALLTYPE OnCollectionChanged( 
            /* [in] */ __RPC__in BSTR bstrNewCollection,
            /* [in] */ __RPC__in BSTR bstrNewFilter) = 0;
        
    };
    
#else 	/* C style interface */

    typedef struct IVsHelpEventsVtbl
    {
        BEGIN_INTERFACE
        
        HRESULT ( STDMETHODCALLTYPE *QueryInterface )( 
            IVsHelpEvents * This,
            /* [in] */ __RPC__in REFIID riid,
            /* [iid_is][out] */ 
            __RPC__deref_out  void **ppvObject);
        
        ULONG ( STDMETHODCALLTYPE *AddRef )( 
            IVsHelpEvents * This);
        
        ULONG ( STDMETHODCALLTYPE *Release )( 
            IVsHelpEvents * This);
        
        HRESULT ( STDMETHODCALLTYPE *GetTypeInfoCount )( 
            IVsHelpEvents * This,
            /* [out] */ __RPC__out UINT *pctinfo);
        
        HRESULT ( STDMETHODCALLTYPE *GetTypeInfo )( 
            IVsHelpEvents * This,
            /* [in] */ UINT iTInfo,
            /* [in] */ LCID lcid,
            /* [out] */ __RPC__deref_out_opt ITypeInfo **ppTInfo);
        
        HRESULT ( STDMETHODCALLTYPE *GetIDsOfNames )( 
            IVsHelpEvents * This,
            /* [in] */ __RPC__in REFIID riid,
            /* [size_is][in] */ __RPC__in_ecount_full(cNames) LPOLESTR *rgszNames,
            /* [range][in] */ UINT cNames,
            /* [in] */ LCID lcid,
            /* [size_is][out] */ __RPC__out_ecount_full(cNames) DISPID *rgDispId);
        
        /* [local] */ HRESULT ( STDMETHODCALLTYPE *Invoke )( 
            IVsHelpEvents * This,
            /* [in] */ DISPID dispIdMember,
            /* [in] */ REFIID riid,
            /* [in] */ LCID lcid,
            /* [in] */ WORD wFlags,
            /* [out][in] */ DISPPARAMS *pDispParams,
            /* [out] */ VARIANT *pVarResult,
            /* [out] */ EXCEPINFO *pExcepInfo,
            /* [out] */ UINT *puArgErr);
        
        /* [id] */ HRESULT ( STDMETHODCALLTYPE *OnFilterChanged )( 
            IVsHelpEvents * This,
            /* [in] */ __RPC__in BSTR bstrNewFilter);
        
        /* [id] */ HRESULT ( STDMETHODCALLTYPE *OnCollectionChanged )( 
            IVsHelpEvents * This,
            /* [in] */ __RPC__in BSTR bstrNewCollection,
            /* [in] */ __RPC__in BSTR bstrNewFilter);
        
        END_INTERFACE
    } IVsHelpEventsVtbl;

    interface IVsHelpEvents
    {
        CONST_VTBL struct IVsHelpEventsVtbl *lpVtbl;
    };

    

#ifdef COBJMACROS


#define IVsHelpEvents_QueryInterface(This,riid,ppvObject)	\
    ( (This)->lpVtbl -> QueryInterface(This,riid,ppvObject) ) 

#define IVsHelpEvents_AddRef(This)	\
    ( (This)->lpVtbl -> AddRef(This) ) 

#define IVsHelpEvents_Release(This)	\
    ( (This)->lpVtbl -> Release(This) ) 


#define IVsHelpEvents_GetTypeInfoCount(This,pctinfo)	\
    ( (This)->lpVtbl -> GetTypeInfoCount(This,pctinfo) ) 

#define IVsHelpEvents_GetTypeInfo(This,iTInfo,lcid,ppTInfo)	\
    ( (This)->lpVtbl -> GetTypeInfo(This,iTInfo,lcid,ppTInfo) ) 

#define IVsHelpEvents_GetIDsOfNames(This,riid,rgszNames,cNames,lcid,rgDispId)	\
    ( (This)->lpVtbl -> GetIDsOfNames(This,riid,rgszNames,cNames,lcid,rgDispId) ) 

#define IVsHelpEvents_Invoke(This,dispIdMember,riid,lcid,wFlags,pDispParams,pVarResult,pExcepInfo,puArgErr)	\
    ( (This)->lpVtbl -> Invoke(This,dispIdMember,riid,lcid,wFlags,pDispParams,pVarResult,pExcepInfo,puArgErr) ) 


#define IVsHelpEvents_OnFilterChanged(This,bstrNewFilter)	\
    ( (This)->lpVtbl -> OnFilterChanged(This,bstrNewFilter) ) 

#define IVsHelpEvents_OnCollectionChanged(This,bstrNewCollection,bstrNewFilter)	\
    ( (This)->lpVtbl -> OnCollectionChanged(This,bstrNewCollection,bstrNewFilter) ) 

#endif /* COBJMACROS */


#endif 	/* C style interface */




#endif 	/* __IVsHelpEvents_INTERFACE_DEFINED__ */


EXTERN_C const CLSID CLSID_DExploreAppObj;

#ifdef __cplusplus

class DECLSPEC_UUID("4A79114D-19E4-11d3-B86B-00C04F79F802")
DExploreAppObj;
#endif
#endif /* __VsHelp_LIBRARY_DEFINED__ */

/* interface __MIDL_itf_vshelp_0001_0068 */
/* [local] */ 

#define SID_SVsHelp	IID_Help
#define IID_IVsHelp	IID_Help
#define IVsHelp	    Help
#define CLSID_MsdnAppObj	CLSID_DExploreAppObj
extern const __declspec(selectany) GUID SID_SVsIntegratedHelp = { 0xca8a686a, 0x882, 0x4e79, { 0xbc, 0xa3, 0xaf, 0x3f, 0x3a, 0xb3, 0xeb, 0x8a } };


extern RPC_IF_HANDLE __MIDL_itf_vshelp_0001_0068_v0_0_c_ifspec;
extern RPC_IF_HANDLE __MIDL_itf_vshelp_0001_0068_v0_0_s_ifspec;

/* Additional Prototypes for ALL interfaces */

/* end of Additional Prototypes */

#ifdef __cplusplus
}
#endif

#endif


