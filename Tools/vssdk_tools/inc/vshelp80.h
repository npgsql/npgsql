

/* this ALWAYS GENERATED file contains the definitions for the interfaces */


 /* File created by MIDL compiler version 7.00.0499 */
/* Compiler settings for vshelp80.idl:
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


#ifndef __vshelp80_h__
#define __vshelp80_h__

#if defined(_MSC_VER) && (_MSC_VER >= 1020)
#pragma once
#endif

/* Forward Declarations */ 

#ifndef __Help2_FWD_DEFINED__
#define __Help2_FWD_DEFINED__
typedef interface Help2 Help2;
#endif 	/* __Help2_FWD_DEFINED__ */


#ifndef __IVsHelpFavorites_FWD_DEFINED__
#define __IVsHelpFavorites_FWD_DEFINED__
typedef interface IVsHelpFavorites IVsHelpFavorites;
#endif 	/* __IVsHelpFavorites_FWD_DEFINED__ */


#ifndef __SVsHelpFavorites_FWD_DEFINED__
#define __SVsHelpFavorites_FWD_DEFINED__
typedef interface SVsHelpFavorites SVsHelpFavorites;
#endif 	/* __SVsHelpFavorites_FWD_DEFINED__ */


/* header files for imported files */
#include "oaidl.h"
#include "ocidl.h"

#ifdef __cplusplus
extern "C"{
#endif 



#ifndef __VsHelp80_LIBRARY_DEFINED__
#define __VsHelp80_LIBRARY_DEFINED__

/* library VsHelp80 */
/* [version][uuid] */ 




enum __vsSearchFlags
    {	vsSearchFlagsNone	= 0,
	vsSearchFlagsExecuteSearch	= 0x1,
	vsSearchFlagsAddToExistingQueryString	= 0x2,
	vsSearchFlagsFilterTransformSpecified	= 0x4
    } ;
typedef DWORD vsSearchFlags;


enum __vsAskQuestionFlags
    {	vsAskQuestionFlagsAskNew	= 0x1,
	vsAskQuestionFlagsCheckStatus	= 0x2,
	vsAskQuestionFlagsSendFeedback	= 0x4
    } ;
typedef DWORD vsAskQuestionFlags;


enum __vsHelpDisplayUrlFlags
    {	vsHelpDisplayUrlFlagsNone	= 0,
	vsHelpDisplayUrlFlagsHighlightTerm	= 0x1,
	vsHelpDisplayUrlFlagsOpenNewWindow	= 0x2,
	vsHelpDisplayUrlFlagsGuidLocal	= 0x10,
	vsHelpDisplayUrlFlagsGuidOnline	= 0x20,
	vsHelpDisplayUrlFlagsGuidLocale	= 0x40,
	vsHelpDisplayUrlFlagsGuidFailover	= 0x80,
	vsHelpDisplayUrlFlagsNamedUrl	= 0x100,
	vsHelpDisplayUrlFlagsNoHistory	= 0x200,
	vsHelpDisplayUrlFlagsNoHistoryThisPage	= 0x400
    } ;
typedef DWORD vsHelpDisplayUrlFlags;


enum __vsHelpUrlFromTopicIDFlags
    {	HUFTID_Default	= 0,
	HUFTID_Local	= 0x1,
	HUFTID_Online	= 0x2,
	HUFTID_Locale	= 0x4
    } ;
typedef DWORD vsHelpUrlFromTopicIDFlags;


EXTERN_C const IID LIBID_VsHelp80;

#ifndef __Help2_INTERFACE_DEFINED__
#define __Help2_INTERFACE_DEFINED__

/* interface Help2 */
/* [uuid][object][oleautomation][dual] */ 


EXTERN_C const IID IID_Help2;

#if defined(__cplusplus) && !defined(CINTERFACE)
    
    MIDL_INTERFACE("78413D2D-0492-4a9b-AB25-730633679977")
    Help2 : public Help
    {
    public:
        virtual /* [id] */ HRESULT STDMETHODCALLTYPE SearchEx( 
            /* [in] */ __RPC__in BSTR bstrSearchFilterTransform,
            /* [in] */ __RPC__in BSTR pszSearchTerm,
            /* [in] */ vsSearchFlags vssfSearchFlags) = 0;
        
        virtual /* [id] */ HRESULT STDMETHODCALLTYPE HowDoI( void) = 0;
        
        virtual /* [id] */ HRESULT STDMETHODCALLTYPE Favorites( void) = 0;
        
        virtual /* [id] */ HRESULT STDMETHODCALLTYPE AskAQuestion( 
            /* [in] */ vsAskQuestionFlags askQuestionFlags) = 0;
        
        virtual /* [id] */ HRESULT STDMETHODCALLTYPE DisplayTopicFromURLEx2( 
            /* [in] */ __RPC__in BSTR bstrURL,
            /* [in] */ vsHelpDisplayUrlFlags displayUrlFlags,
            /* [in] */ __RPC__in BSTR bstrParam) = 0;
        
        virtual /* [id] */ HRESULT STDMETHODCALLTYPE InitializeSettingsToken( 
            /* [in] */ __RPC__in BSTR bstrSettingsToken) = 0;
        
    };
    
#else 	/* C style interface */

    typedef struct Help2Vtbl
    {
        BEGIN_INTERFACE
        
        /* [id][restricted][funcdescattr] */ HRESULT ( STDMETHODCALLTYPE *QueryInterface )( 
            Help2 * This,
            /* [in][idldescattr] */ __RPC__in struct GUID *riid,
            /* [out][idldescattr] */ __RPC__deref_out_opt void **ppvObj,
            /* [retval][out] */ __RPC__out void *retval);
        
        /* [id][restricted][funcdescattr] */ HRESULT ( STDMETHODCALLTYPE *AddRef )( 
            Help2 * This,
            /* [retval][out] */ __RPC__out unsigned long *retval);
        
        /* [id][restricted][funcdescattr] */ HRESULT ( STDMETHODCALLTYPE *Release )( 
            Help2 * This,
            /* [retval][out] */ __RPC__out unsigned long *retval);
        
        /* [id][restricted][funcdescattr] */ HRESULT ( STDMETHODCALLTYPE *GetTypeInfoCount )( 
            Help2 * This,
            /* [out][idldescattr] */ __RPC__out unsigned int *pctinfo,
            /* [retval][out] */ __RPC__out void *retval);
        
        /* [id][restricted][funcdescattr] */ HRESULT ( STDMETHODCALLTYPE *GetTypeInfo )( 
            Help2 * This,
            /* [in][idldescattr] */ unsigned int itinfo,
            /* [in][idldescattr] */ unsigned long lcid,
            /* [out][idldescattr] */ __RPC__deref_out_opt void **pptinfo,
            /* [retval][out] */ __RPC__out void *retval);
        
        /* [id][restricted][funcdescattr] */ HRESULT ( STDMETHODCALLTYPE *GetIDsOfNames )( 
            Help2 * This,
            /* [in][idldescattr] */ __RPC__in struct GUID *riid,
            /* [in][idldescattr] */ __RPC__deref_in_opt signed char **rgszNames,
            /* [in][idldescattr] */ unsigned int cNames,
            /* [in][idldescattr] */ unsigned long lcid,
            /* [out][idldescattr] */ __RPC__out signed long *rgdispid,
            /* [retval][out] */ __RPC__out void *retval);
        
        /* [id][restricted][funcdescattr] */ HRESULT ( STDMETHODCALLTYPE *Invoke )( 
            Help2 * This,
            /* [in][idldescattr] */ signed long dispidMember,
            /* [in][idldescattr] */ __RPC__in struct GUID *riid,
            /* [in][idldescattr] */ unsigned long lcid,
            /* [in][idldescattr] */ unsigned short wFlags,
            /* [in][idldescattr] */ __RPC__in struct DISPPARAMS *pdispparams,
            /* [out][idldescattr] */ __RPC__out VARIANT *pvarResult,
            /* [out][idldescattr] */ __RPC__out struct EXCEPINFO *pexcepinfo,
            /* [out][idldescattr] */ __RPC__out unsigned int *puArgErr,
            /* [retval][out] */ __RPC__out void *retval);
        
        /* [id][funcdescattr] */ HRESULT ( STDMETHODCALLTYPE *Contents )( 
            Help2 * This,
            /* [retval][out] */ __RPC__out void *retval);
        
        /* [id][funcdescattr] */ HRESULT ( STDMETHODCALLTYPE *Index )( 
            Help2 * This,
            /* [retval][out] */ __RPC__out void *retval);
        
        /* [id][funcdescattr] */ HRESULT ( STDMETHODCALLTYPE *Search )( 
            Help2 * This,
            /* [retval][out] */ __RPC__out void *retval);
        
        /* [id][funcdescattr] */ HRESULT ( STDMETHODCALLTYPE *IndexResults )( 
            Help2 * This,
            /* [retval][out] */ __RPC__out void *retval);
        
        /* [id][funcdescattr] */ HRESULT ( STDMETHODCALLTYPE *SearchResults )( 
            Help2 * This,
            /* [retval][out] */ __RPC__out void *retval);
        
        /* [id][funcdescattr] */ HRESULT ( STDMETHODCALLTYPE *DisplayTopicFromId )( 
            Help2 * This,
            /* [in][idldescattr] */ __RPC__in BSTR bstrFile,
            /* [in][idldescattr] */ unsigned long Id,
            /* [retval][out] */ __RPC__out void *retval);
        
        /* [id][funcdescattr] */ HRESULT ( STDMETHODCALLTYPE *DisplayTopicFromURL )( 
            Help2 * This,
            /* [in][idldescattr] */ __RPC__in BSTR pszURL,
            /* [retval][out] */ __RPC__out void *retval);
        
        /* [id][funcdescattr] */ HRESULT ( STDMETHODCALLTYPE *DisplayTopicFromURLEx )( 
            Help2 * This,
            /* [in][idldescattr] */ __RPC__in BSTR pszURL,
            /* [in][idldescattr] */ __RPC__in_opt IVsHelpTopicShowEvents *pIVsHelpTopicShowEvents,
            /* [retval][out] */ __RPC__out void *retval);
        
        /* [id][funcdescattr] */ HRESULT ( STDMETHODCALLTYPE *DisplayTopicFromKeyword )( 
            Help2 * This,
            /* [in][idldescattr] */ __RPC__in BSTR pszKeyword,
            /* [retval][out] */ __RPC__out void *retval);
        
        /* [id][funcdescattr] */ HRESULT ( STDMETHODCALLTYPE *DisplayTopicFromF1Keyword )( 
            Help2 * This,
            /* [in][idldescattr] */ __RPC__in BSTR pszKeyword,
            /* [retval][out] */ __RPC__out void *retval);
        
        /* [id][funcdescattr] */ HRESULT ( STDMETHODCALLTYPE *DisplayTopicFrom_OLD_Help )( 
            Help2 * This,
            /* [in][idldescattr] */ __RPC__in BSTR bstrFile,
            /* [in][idldescattr] */ unsigned long Id,
            /* [retval][out] */ __RPC__out void *retval);
        
        /* [id][funcdescattr] */ HRESULT ( STDMETHODCALLTYPE *SyncContents )( 
            Help2 * This,
            /* [in][idldescattr] */ __RPC__in BSTR bstrURL,
            /* [retval][out] */ __RPC__out void *retval);
        
        /* [id][funcdescattr] */ HRESULT ( STDMETHODCALLTYPE *CanSyncContents )( 
            Help2 * This,
            /* [in][idldescattr] */ __RPC__in BSTR bstrURL,
            /* [retval][out] */ __RPC__out void *retval);
        
        /* [id][funcdescattr] */ HRESULT ( STDMETHODCALLTYPE *GetNextTopic )( 
            Help2 * This,
            /* [in][idldescattr] */ __RPC__in BSTR bstrURL,
            /* [retval][out] */ __RPC__deref_out_opt BSTR *retval);
        
        /* [id][funcdescattr] */ HRESULT ( STDMETHODCALLTYPE *GetPrevTopic )( 
            Help2 * This,
            /* [in][idldescattr] */ __RPC__in BSTR bstrURL,
            /* [retval][out] */ __RPC__deref_out_opt BSTR *retval);
        
        /* [id][funcdescattr] */ HRESULT ( STDMETHODCALLTYPE *FilterUI )( 
            Help2 * This,
            /* [retval][out] */ __RPC__out void *retval);
        
        /* [id][funcdescattr] */ HRESULT ( STDMETHODCALLTYPE *CanShowFilterUI )( 
            Help2 * This,
            /* [retval][out] */ __RPC__out void *retval);
        
        /* [id][funcdescattr] */ HRESULT ( STDMETHODCALLTYPE *Close )( 
            Help2 * This,
            /* [retval][out] */ __RPC__out void *retval);
        
        /* [id][funcdescattr] */ HRESULT ( STDMETHODCALLTYPE *SyncIndex )( 
            Help2 * This,
            /* [in][idldescattr] */ __RPC__in BSTR bstrKeyword,
            /* [in][idldescattr] */ signed long fShow,
            /* [retval][out] */ __RPC__out void *retval);
        
        /* [id][funcdescattr] */ HRESULT ( STDMETHODCALLTYPE *SetCollection )( 
            Help2 * This,
            /* [in][idldescattr] */ __RPC__in BSTR bstrCollection,
            /* [in][idldescattr] */ __RPC__in BSTR bstrFilter,
            /* [retval][out] */ __RPC__out void *retval);
        
        /* [id][propget][funcdescattr] */ HRESULT ( STDMETHODCALLTYPE *get_Collection )( 
            Help2 * This,
            /* [retval][out] */ __RPC__deref_out_opt BSTR *retval);
        
        /* [id][propget][funcdescattr] */ HRESULT ( STDMETHODCALLTYPE *get_Filter )( 
            Help2 * This,
            /* [retval][out] */ __RPC__deref_out_opt BSTR *retval);
        
        /* [id][propput][funcdescattr] */ HRESULT ( STDMETHODCALLTYPE *put_Filter )( 
            Help2 * This,
            /* [in][idldescattr] */ __RPC__in BSTR noname,
            /* [retval][out] */ __RPC__out void *retval);
        
        /* [id][propget][funcdescattr] */ HRESULT ( STDMETHODCALLTYPE *get_FilterQuery )( 
            Help2 * This,
            /* [retval][out] */ __RPC__deref_out_opt BSTR *retval);
        
        /* [id][propget][funcdescattr] */ HRESULT ( STDMETHODCALLTYPE *get_HelpOwner )( 
            Help2 * This,
            /* [retval][out] */ __RPC__deref_out_opt IVsHelpOwner **retval);
        
        /* [id][propput][funcdescattr] */ HRESULT ( STDMETHODCALLTYPE *put_HelpOwner )( 
            Help2 * This,
            /* [in][idldescattr] */ __RPC__in_opt IVsHelpOwner *noname,
            /* [retval][out] */ __RPC__out void *retval);
        
        /* [id][propget][funcdescattr] */ HRESULT ( STDMETHODCALLTYPE *get_HxSession )( 
            Help2 * This,
            /* [retval][out] */ __RPC__deref_out_opt IDispatch **retval);
        
        /* [id][propget][funcdescattr] */ HRESULT ( STDMETHODCALLTYPE *get_Help )( 
            Help2 * This,
            /* [retval][out] */ __RPC__deref_out_opt IDispatch **retval);
        
        /* [id][funcdescattr] */ HRESULT ( STDMETHODCALLTYPE *GetObject )( 
            Help2 * This,
            /* [in][idldescattr] */ __RPC__in BSTR bstrMoniker,
            /* [in][idldescattr] */ __RPC__in BSTR bstrOptions,
            /* [retval][out] */ __RPC__deref_out_opt IDispatch **retval);
        
        /* [id] */ HRESULT ( STDMETHODCALLTYPE *SearchEx )( 
            Help2 * This,
            /* [in] */ __RPC__in BSTR bstrSearchFilterTransform,
            /* [in] */ __RPC__in BSTR pszSearchTerm,
            /* [in] */ vsSearchFlags vssfSearchFlags);
        
        /* [id] */ HRESULT ( STDMETHODCALLTYPE *HowDoI )( 
            Help2 * This);
        
        /* [id] */ HRESULT ( STDMETHODCALLTYPE *Favorites )( 
            Help2 * This);
        
        /* [id] */ HRESULT ( STDMETHODCALLTYPE *AskAQuestion )( 
            Help2 * This,
            /* [in] */ vsAskQuestionFlags askQuestionFlags);
        
        /* [id] */ HRESULT ( STDMETHODCALLTYPE *DisplayTopicFromURLEx2 )( 
            Help2 * This,
            /* [in] */ __RPC__in BSTR bstrURL,
            /* [in] */ vsHelpDisplayUrlFlags displayUrlFlags,
            /* [in] */ __RPC__in BSTR bstrParam);
        
        /* [id] */ HRESULT ( STDMETHODCALLTYPE *InitializeSettingsToken )( 
            Help2 * This,
            /* [in] */ __RPC__in BSTR bstrSettingsToken);
        
        END_INTERFACE
    } Help2Vtbl;

    interface Help2
    {
        CONST_VTBL struct Help2Vtbl *lpVtbl;
    };

    

#ifdef COBJMACROS


#define Help2_QueryInterface(This,riid,ppvObj,retval)	\
    ( (This)->lpVtbl -> QueryInterface(This,riid,ppvObj,retval) ) 

#define Help2_AddRef(This,retval)	\
    ( (This)->lpVtbl -> AddRef(This,retval) ) 

#define Help2_Release(This,retval)	\
    ( (This)->lpVtbl -> Release(This,retval) ) 

#define Help2_GetTypeInfoCount(This,pctinfo,retval)	\
    ( (This)->lpVtbl -> GetTypeInfoCount(This,pctinfo,retval) ) 

#define Help2_GetTypeInfo(This,itinfo,lcid,pptinfo,retval)	\
    ( (This)->lpVtbl -> GetTypeInfo(This,itinfo,lcid,pptinfo,retval) ) 

#define Help2_GetIDsOfNames(This,riid,rgszNames,cNames,lcid,rgdispid,retval)	\
    ( (This)->lpVtbl -> GetIDsOfNames(This,riid,rgszNames,cNames,lcid,rgdispid,retval) ) 

#define Help2_Invoke(This,dispidMember,riid,lcid,wFlags,pdispparams,pvarResult,pexcepinfo,puArgErr,retval)	\
    ( (This)->lpVtbl -> Invoke(This,dispidMember,riid,lcid,wFlags,pdispparams,pvarResult,pexcepinfo,puArgErr,retval) ) 

#define Help2_Contents(This,retval)	\
    ( (This)->lpVtbl -> Contents(This,retval) ) 

#define Help2_Index(This,retval)	\
    ( (This)->lpVtbl -> Index(This,retval) ) 

#define Help2_Search(This,retval)	\
    ( (This)->lpVtbl -> Search(This,retval) ) 

#define Help2_IndexResults(This,retval)	\
    ( (This)->lpVtbl -> IndexResults(This,retval) ) 

#define Help2_SearchResults(This,retval)	\
    ( (This)->lpVtbl -> SearchResults(This,retval) ) 

#define Help2_DisplayTopicFromId(This,bstrFile,Id,retval)	\
    ( (This)->lpVtbl -> DisplayTopicFromId(This,bstrFile,Id,retval) ) 

#define Help2_DisplayTopicFromURL(This,pszURL,retval)	\
    ( (This)->lpVtbl -> DisplayTopicFromURL(This,pszURL,retval) ) 

#define Help2_DisplayTopicFromURLEx(This,pszURL,pIVsHelpTopicShowEvents,retval)	\
    ( (This)->lpVtbl -> DisplayTopicFromURLEx(This,pszURL,pIVsHelpTopicShowEvents,retval) ) 

#define Help2_DisplayTopicFromKeyword(This,pszKeyword,retval)	\
    ( (This)->lpVtbl -> DisplayTopicFromKeyword(This,pszKeyword,retval) ) 

#define Help2_DisplayTopicFromF1Keyword(This,pszKeyword,retval)	\
    ( (This)->lpVtbl -> DisplayTopicFromF1Keyword(This,pszKeyword,retval) ) 

#define Help2_DisplayTopicFrom_OLD_Help(This,bstrFile,Id,retval)	\
    ( (This)->lpVtbl -> DisplayTopicFrom_OLD_Help(This,bstrFile,Id,retval) ) 

#define Help2_SyncContents(This,bstrURL,retval)	\
    ( (This)->lpVtbl -> SyncContents(This,bstrURL,retval) ) 

#define Help2_CanSyncContents(This,bstrURL,retval)	\
    ( (This)->lpVtbl -> CanSyncContents(This,bstrURL,retval) ) 

#define Help2_GetNextTopic(This,bstrURL,retval)	\
    ( (This)->lpVtbl -> GetNextTopic(This,bstrURL,retval) ) 

#define Help2_GetPrevTopic(This,bstrURL,retval)	\
    ( (This)->lpVtbl -> GetPrevTopic(This,bstrURL,retval) ) 

#define Help2_FilterUI(This,retval)	\
    ( (This)->lpVtbl -> FilterUI(This,retval) ) 

#define Help2_CanShowFilterUI(This,retval)	\
    ( (This)->lpVtbl -> CanShowFilterUI(This,retval) ) 

#define Help2_Close(This,retval)	\
    ( (This)->lpVtbl -> Close(This,retval) ) 

#define Help2_SyncIndex(This,bstrKeyword,fShow,retval)	\
    ( (This)->lpVtbl -> SyncIndex(This,bstrKeyword,fShow,retval) ) 

#define Help2_SetCollection(This,bstrCollection,bstrFilter,retval)	\
    ( (This)->lpVtbl -> SetCollection(This,bstrCollection,bstrFilter,retval) ) 

#define Help2_get_Collection(This,retval)	\
    ( (This)->lpVtbl -> get_Collection(This,retval) ) 

#define Help2_get_Filter(This,retval)	\
    ( (This)->lpVtbl -> get_Filter(This,retval) ) 

#define Help2_put_Filter(This,noname,retval)	\
    ( (This)->lpVtbl -> put_Filter(This,noname,retval) ) 

#define Help2_get_FilterQuery(This,retval)	\
    ( (This)->lpVtbl -> get_FilterQuery(This,retval) ) 

#define Help2_get_HelpOwner(This,retval)	\
    ( (This)->lpVtbl -> get_HelpOwner(This,retval) ) 

#define Help2_put_HelpOwner(This,noname,retval)	\
    ( (This)->lpVtbl -> put_HelpOwner(This,noname,retval) ) 

#define Help2_get_HxSession(This,retval)	\
    ( (This)->lpVtbl -> get_HxSession(This,retval) ) 

#define Help2_get_Help(This,retval)	\
    ( (This)->lpVtbl -> get_Help(This,retval) ) 

#define Help2_GetObject(This,bstrMoniker,bstrOptions,retval)	\
    ( (This)->lpVtbl -> GetObject(This,bstrMoniker,bstrOptions,retval) ) 


#define Help2_SearchEx(This,bstrSearchFilterTransform,pszSearchTerm,vssfSearchFlags)	\
    ( (This)->lpVtbl -> SearchEx(This,bstrSearchFilterTransform,pszSearchTerm,vssfSearchFlags) ) 

#define Help2_HowDoI(This)	\
    ( (This)->lpVtbl -> HowDoI(This) ) 

#define Help2_Favorites(This)	\
    ( (This)->lpVtbl -> Favorites(This) ) 

#define Help2_AskAQuestion(This,askQuestionFlags)	\
    ( (This)->lpVtbl -> AskAQuestion(This,askQuestionFlags) ) 

#define Help2_DisplayTopicFromURLEx2(This,bstrURL,displayUrlFlags,bstrParam)	\
    ( (This)->lpVtbl -> DisplayTopicFromURLEx2(This,bstrURL,displayUrlFlags,bstrParam) ) 

#define Help2_InitializeSettingsToken(This,bstrSettingsToken)	\
    ( (This)->lpVtbl -> InitializeSettingsToken(This,bstrSettingsToken) ) 

#endif /* COBJMACROS */


#endif 	/* C style interface */




#endif 	/* __Help2_INTERFACE_DEFINED__ */


#ifndef __IVsHelpFavorites_INTERFACE_DEFINED__
#define __IVsHelpFavorites_INTERFACE_DEFINED__

/* interface IVsHelpFavorites */
/* [object][oleautomation][uuid] */ 


EXTERN_C const IID IID_IVsHelpFavorites;

#if defined(__cplusplus) && !defined(CINTERFACE)
    
    MIDL_INTERFACE("D1333514-2B6B-4479-8135-A39E286E146D")
    IVsHelpFavorites : public IUnknown
    {
    public:
        virtual /* [id] */ HRESULT STDMETHODCALLTYPE ShowFavorites( void) = 0;
        
        virtual /* [id] */ HRESULT STDMETHODCALLTYPE AddFavoriteTopic( 
            /* [in] */ __RPC__in BSTR strTitle,
            /* [in] */ __RPC__in BSTR strUrl,
            /* [in] */ __RPC__in BSTR topicKeyword,
            /* [in] */ __RPC__in BSTR strTopicLocale) = 0;
        
    };
    
#else 	/* C style interface */

    typedef struct IVsHelpFavoritesVtbl
    {
        BEGIN_INTERFACE
        
        HRESULT ( STDMETHODCALLTYPE *QueryInterface )( 
            IVsHelpFavorites * This,
            /* [in] */ __RPC__in REFIID riid,
            /* [iid_is][out] */ 
            __RPC__deref_out  void **ppvObject);
        
        ULONG ( STDMETHODCALLTYPE *AddRef )( 
            IVsHelpFavorites * This);
        
        ULONG ( STDMETHODCALLTYPE *Release )( 
            IVsHelpFavorites * This);
        
        /* [id] */ HRESULT ( STDMETHODCALLTYPE *ShowFavorites )( 
            IVsHelpFavorites * This);
        
        /* [id] */ HRESULT ( STDMETHODCALLTYPE *AddFavoriteTopic )( 
            IVsHelpFavorites * This,
            /* [in] */ __RPC__in BSTR strTitle,
            /* [in] */ __RPC__in BSTR strUrl,
            /* [in] */ __RPC__in BSTR topicKeyword,
            /* [in] */ __RPC__in BSTR strTopicLocale);
        
        END_INTERFACE
    } IVsHelpFavoritesVtbl;

    interface IVsHelpFavorites
    {
        CONST_VTBL struct IVsHelpFavoritesVtbl *lpVtbl;
    };

    

#ifdef COBJMACROS


#define IVsHelpFavorites_QueryInterface(This,riid,ppvObject)	\
    ( (This)->lpVtbl -> QueryInterface(This,riid,ppvObject) ) 

#define IVsHelpFavorites_AddRef(This)	\
    ( (This)->lpVtbl -> AddRef(This) ) 

#define IVsHelpFavorites_Release(This)	\
    ( (This)->lpVtbl -> Release(This) ) 


#define IVsHelpFavorites_ShowFavorites(This)	\
    ( (This)->lpVtbl -> ShowFavorites(This) ) 

#define IVsHelpFavorites_AddFavoriteTopic(This,strTitle,strUrl,topicKeyword,strTopicLocale)	\
    ( (This)->lpVtbl -> AddFavoriteTopic(This,strTitle,strUrl,topicKeyword,strTopicLocale) ) 

#endif /* COBJMACROS */


#endif 	/* C style interface */




#endif 	/* __IVsHelpFavorites_INTERFACE_DEFINED__ */



#ifndef __vsHelpSearchFilterTransforms_MODULE_DEFINED__
#define __vsHelpSearchFilterTransforms_MODULE_DEFINED__


/* module vsHelpSearchFilterTransforms */


const LPWSTR vsHelpSearchFilterTransformsHelp	=	L"Help";

const LPWSTR vsHelpSearchFilterTransformsControls	=	L"Controls";

const LPWSTR vsHelpSearchFilterTransformsSamples	=	L"Samples";

const LPWSTR vsHelpSearchFilterTransformsSnippets	=	L"Snippets";

const LPWSTR vsHelpSearchFilterTransformsStarterKits	=	L"StarterKits";

const LPWSTR vsHelpSearchFilterTransformsAddins	=	L"Addins";

const LPWSTR vsHelpSearchFilterTransformsUnfiltered	=	L"Unfiltered";

#endif /* __vsHelpSearchFilterTransforms_MODULE_DEFINED__ */

#ifndef __SVsHelpFavorites_INTERFACE_DEFINED__
#define __SVsHelpFavorites_INTERFACE_DEFINED__

/* interface SVsHelpFavorites */
/* [object][uuid] */ 


EXTERN_C const IID IID_SVsHelpFavorites;

#if defined(__cplusplus) && !defined(CINTERFACE)
    
    MIDL_INTERFACE("4A81432B-BFCC-4832-9BE9-15977DA82072")
    SVsHelpFavorites : public IUnknown
    {
    public:
    };
    
#else 	/* C style interface */

    typedef struct SVsHelpFavoritesVtbl
    {
        BEGIN_INTERFACE
        
        HRESULT ( STDMETHODCALLTYPE *QueryInterface )( 
            SVsHelpFavorites * This,
            /* [in] */ __RPC__in REFIID riid,
            /* [iid_is][out] */ 
            __RPC__deref_out  void **ppvObject);
        
        ULONG ( STDMETHODCALLTYPE *AddRef )( 
            SVsHelpFavorites * This);
        
        ULONG ( STDMETHODCALLTYPE *Release )( 
            SVsHelpFavorites * This);
        
        END_INTERFACE
    } SVsHelpFavoritesVtbl;

    interface SVsHelpFavorites
    {
        CONST_VTBL struct SVsHelpFavoritesVtbl *lpVtbl;
    };

    

#ifdef COBJMACROS


#define SVsHelpFavorites_QueryInterface(This,riid,ppvObject)	\
    ( (This)->lpVtbl -> QueryInterface(This,riid,ppvObject) ) 

#define SVsHelpFavorites_AddRef(This)	\
    ( (This)->lpVtbl -> AddRef(This) ) 

#define SVsHelpFavorites_Release(This)	\
    ( (This)->lpVtbl -> Release(This) ) 


#endif /* COBJMACROS */


#endif 	/* C style interface */




#endif 	/* __SVsHelpFavorites_INTERFACE_DEFINED__ */

#endif /* __VsHelp80_LIBRARY_DEFINED__ */

/* interface __MIDL_itf_vshelp80_0001_0068 */
/* [local] */ 

#define IVsHelp2     Help2
#define IID_IVsHelp2 IID_Help2
#define SID_SVsHelpFavorites IID_SVsHelpFavorites


extern RPC_IF_HANDLE __MIDL_itf_vshelp80_0001_0068_v0_0_c_ifspec;
extern RPC_IF_HANDLE __MIDL_itf_vshelp80_0001_0068_v0_0_s_ifspec;

/* Additional Prototypes for ALL interfaces */

/* end of Additional Prototypes */

#ifdef __cplusplus
}
#endif

#endif


