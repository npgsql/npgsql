

/* this ALWAYS GENERATED file contains the definitions for the interfaces */


 /* File created by MIDL compiler version 8.00.0603 */
/* @@MIDL_FILE_HEADING(  ) */

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


#ifndef __textmgr110_h__
#define __textmgr110_h__

#if defined(_MSC_VER) && (_MSC_VER >= 1020)
#pragma once
#endif

/* Forward Declarations */ 

#ifndef __IVsTextManager3_FWD_DEFINED__
#define __IVsTextManager3_FWD_DEFINED__
typedef interface IVsTextManager3 IVsTextManager3;

#endif 	/* __IVsTextManager3_FWD_DEFINED__ */


#ifndef __IVsTextManagerEvents3_FWD_DEFINED__
#define __IVsTextManagerEvents3_FWD_DEFINED__
typedef interface IVsTextManagerEvents3 IVsTextManagerEvents3;

#endif 	/* __IVsTextManagerEvents3_FWD_DEFINED__ */


#ifndef __IVsTextView3_FWD_DEFINED__
#define __IVsTextView3_FWD_DEFINED__
typedef interface IVsTextView3 IVsTextView3;

#endif 	/* __IVsTextView3_FWD_DEFINED__ */


#ifndef __IVsLanguageDebugInfo3_FWD_DEFINED__
#define __IVsLanguageDebugInfo3_FWD_DEFINED__
typedef interface IVsLanguageDebugInfo3 IVsLanguageDebugInfo3;

#endif 	/* __IVsLanguageDebugInfo3_FWD_DEFINED__ */


/* header files for imported files */
#include "IVsQueryEditQuerySave2.h"
#include "IVsQueryEditQuerySave80.h"
#include "msxml.h"
#include "context.h"
#include "textmgr.h"
#include "textmgr2.h"

#ifdef __cplusplus
extern "C"{
#endif 


/* interface __MIDL_itf_textmgr110_0000_0000 */
/* [local] */ 

#pragma once
#pragma once


extern RPC_IF_HANDLE __MIDL_itf_textmgr110_0000_0000_v0_0_c_ifspec;
extern RPC_IF_HANDLE __MIDL_itf_textmgr110_0000_0000_v0_0_s_ifspec;


#ifndef __TextMgr110_LIBRARY_DEFINED__
#define __TextMgr110_LIBRARY_DEFINED__

/* library TextMgr110 */
/* [version][uuid] */ 

typedef struct _VIEWPREFERENCES3
    {
    unsigned int fVisibleWhitespace;
    unsigned int fSelectionMargin;
    unsigned int fAutoDelimiterHighlight;
    unsigned int fGoToAnchorAfterEscape;
    unsigned int fDragDropEditing;
    unsigned int fUndoCaretMovements;
    unsigned int fOvertype;
    unsigned int fDragDropMove;
    unsigned int fWidgetMargin;
    unsigned int fReadOnly;
    unsigned int fActiveInModalState;
    unsigned int fClientDragDropFeedback;
    unsigned int fTrackChanges;
    unsigned int uCompletorSize;
    unsigned int fDetectUTF8;
    long lEditorEmulation;
    unsigned int fHighlightCurrentLine;
    } 	VIEWPREFERENCES3;


EXTERN_C const IID LIBID_TextMgr110;

#ifndef __IVsTextManager3_INTERFACE_DEFINED__
#define __IVsTextManager3_INTERFACE_DEFINED__

/* interface IVsTextManager3 */
/* [object][custom][version][uuid] */ 


EXTERN_C const IID IID_IVsTextManager3;

#if defined(__cplusplus) && !defined(CINTERFACE)
    
    MIDL_INTERFACE("F06DF8BD-23F9-49A3-94C3-F3228CF60F2D")
    IVsTextManager3 : public IUnknown
    {
    public:
        virtual /* [local] */ HRESULT STDMETHODCALLTYPE GetUserPreferences3( 
            /* [out] */ VIEWPREFERENCES3 *pViewPrefs,
            /* [out] */ FRAMEPREFERENCES2 *pFramePrefs,
            /* [out][in] */ LANGPREFERENCES2 *pLangPrefs,
            /* [out][in] */ FONTCOLORPREFERENCES2 *pColorPrefs) = 0;
        
        virtual /* [local] */ HRESULT STDMETHODCALLTYPE SetUserPreferences3( 
            /* [in] */ const VIEWPREFERENCES3 *pViewPrefs,
            /* [in] */ const FRAMEPREFERENCES2 *pFramePrefs,
            /* [in] */ const LANGPREFERENCES2 *pLangPrefs,
            /* [in] */ const FONTCOLORPREFERENCES2 *pColorPrefs) = 0;
        
        virtual HRESULT STDMETHODCALLTYPE FindLanguageSIDForExtensionlessFilename( 
            /* [in] */ __RPC__in LPCOLESTR pszFileName,
            /* [out] */ __RPC__out GUID *pguidLangSID) = 0;
        
        virtual HRESULT STDMETHODCALLTYPE PrimeExpansionManager( 
            /* [in] */ __RPC__in REFGUID guidLang) = 0;
        
    };
    
    
#else 	/* C style interface */

    typedef struct IVsTextManager3Vtbl
    {
        BEGIN_INTERFACE
        
        HRESULT ( STDMETHODCALLTYPE *QueryInterface )( 
            __RPC__in IVsTextManager3 * This,
            /* [in] */ __RPC__in REFIID riid,
            /* [annotation][iid_is][out] */ 
            _COM_Outptr_  void **ppvObject);
        
        ULONG ( STDMETHODCALLTYPE *AddRef )( 
            __RPC__in IVsTextManager3 * This);
        
        ULONG ( STDMETHODCALLTYPE *Release )( 
            __RPC__in IVsTextManager3 * This);
        
        /* [local] */ HRESULT ( STDMETHODCALLTYPE *GetUserPreferences3 )( 
            IVsTextManager3 * This,
            /* [out] */ VIEWPREFERENCES3 *pViewPrefs,
            /* [out] */ FRAMEPREFERENCES2 *pFramePrefs,
            /* [out][in] */ LANGPREFERENCES2 *pLangPrefs,
            /* [out][in] */ FONTCOLORPREFERENCES2 *pColorPrefs);
        
        /* [local] */ HRESULT ( STDMETHODCALLTYPE *SetUserPreferences3 )( 
            IVsTextManager3 * This,
            /* [in] */ const VIEWPREFERENCES3 *pViewPrefs,
            /* [in] */ const FRAMEPREFERENCES2 *pFramePrefs,
            /* [in] */ const LANGPREFERENCES2 *pLangPrefs,
            /* [in] */ const FONTCOLORPREFERENCES2 *pColorPrefs);
        
        HRESULT ( STDMETHODCALLTYPE *FindLanguageSIDForExtensionlessFilename )( 
            __RPC__in IVsTextManager3 * This,
            /* [in] */ __RPC__in LPCOLESTR pszFileName,
            /* [out] */ __RPC__out GUID *pguidLangSID);
        
        HRESULT ( STDMETHODCALLTYPE *PrimeExpansionManager )( 
            __RPC__in IVsTextManager3 * This,
            /* [in] */ __RPC__in REFGUID guidLang);
        
        END_INTERFACE
    } IVsTextManager3Vtbl;

    interface IVsTextManager3
    {
        CONST_VTBL struct IVsTextManager3Vtbl *lpVtbl;
    };

    

#ifdef COBJMACROS


#define IVsTextManager3_QueryInterface(This,riid,ppvObject)	\
    ( (This)->lpVtbl -> QueryInterface(This,riid,ppvObject) ) 

#define IVsTextManager3_AddRef(This)	\
    ( (This)->lpVtbl -> AddRef(This) ) 

#define IVsTextManager3_Release(This)	\
    ( (This)->lpVtbl -> Release(This) ) 


#define IVsTextManager3_GetUserPreferences3(This,pViewPrefs,pFramePrefs,pLangPrefs,pColorPrefs)	\
    ( (This)->lpVtbl -> GetUserPreferences3(This,pViewPrefs,pFramePrefs,pLangPrefs,pColorPrefs) ) 

#define IVsTextManager3_SetUserPreferences3(This,pViewPrefs,pFramePrefs,pLangPrefs,pColorPrefs)	\
    ( (This)->lpVtbl -> SetUserPreferences3(This,pViewPrefs,pFramePrefs,pLangPrefs,pColorPrefs) ) 

#define IVsTextManager3_FindLanguageSIDForExtensionlessFilename(This,pszFileName,pguidLangSID)	\
    ( (This)->lpVtbl -> FindLanguageSIDForExtensionlessFilename(This,pszFileName,pguidLangSID) ) 

#define IVsTextManager3_PrimeExpansionManager(This,guidLang)	\
    ( (This)->lpVtbl -> PrimeExpansionManager(This,guidLang) ) 

#endif /* COBJMACROS */


#endif 	/* C style interface */




#endif 	/* __IVsTextManager3_INTERFACE_DEFINED__ */


#ifndef __IVsTextManagerEvents3_INTERFACE_DEFINED__
#define __IVsTextManagerEvents3_INTERFACE_DEFINED__

/* interface IVsTextManagerEvents3 */
/* [object][custom][version][uuid] */ 


EXTERN_C const IID IID_IVsTextManagerEvents3;

#if defined(__cplusplus) && !defined(CINTERFACE)
    
    MIDL_INTERFACE("040929D6-0E89-4987-A450-C0F91D03DFC8")
    IVsTextManagerEvents3 : public IUnknown
    {
    public:
        virtual HRESULT STDMETHODCALLTYPE OnRegisterMarkerType( 
            /* [in] */ long iMarkerType) = 0;
        
        virtual HRESULT STDMETHODCALLTYPE OnRegisterView( 
            /* [in] */ __RPC__in_opt IVsTextView *pView) = 0;
        
        virtual HRESULT STDMETHODCALLTYPE OnUnregisterView( 
            /* [in] */ __RPC__in_opt IVsTextView *pView) = 0;
        
        virtual /* [local] */ HRESULT STDMETHODCALLTYPE OnUserPreferencesChanged3( 
            /* [in] */ const VIEWPREFERENCES3 *pViewPrefs,
            /* [in] */ const FRAMEPREFERENCES2 *pFramePrefs,
            /* [in] */ const LANGPREFERENCES2 *pLangPrefs,
            /* [in] */ const FONTCOLORPREFERENCES2 *pColorPrefs) = 0;
        
        virtual HRESULT STDMETHODCALLTYPE OnReplaceAllInFilesBegin( void) = 0;
        
        virtual HRESULT STDMETHODCALLTYPE OnReplaceAllInFilesEnd( void) = 0;
        
    };
    
    
#else 	/* C style interface */

    typedef struct IVsTextManagerEvents3Vtbl
    {
        BEGIN_INTERFACE
        
        HRESULT ( STDMETHODCALLTYPE *QueryInterface )( 
            __RPC__in IVsTextManagerEvents3 * This,
            /* [in] */ __RPC__in REFIID riid,
            /* [annotation][iid_is][out] */ 
            _COM_Outptr_  void **ppvObject);
        
        ULONG ( STDMETHODCALLTYPE *AddRef )( 
            __RPC__in IVsTextManagerEvents3 * This);
        
        ULONG ( STDMETHODCALLTYPE *Release )( 
            __RPC__in IVsTextManagerEvents3 * This);
        
        HRESULT ( STDMETHODCALLTYPE *OnRegisterMarkerType )( 
            __RPC__in IVsTextManagerEvents3 * This,
            /* [in] */ long iMarkerType);
        
        HRESULT ( STDMETHODCALLTYPE *OnRegisterView )( 
            __RPC__in IVsTextManagerEvents3 * This,
            /* [in] */ __RPC__in_opt IVsTextView *pView);
        
        HRESULT ( STDMETHODCALLTYPE *OnUnregisterView )( 
            __RPC__in IVsTextManagerEvents3 * This,
            /* [in] */ __RPC__in_opt IVsTextView *pView);
        
        /* [local] */ HRESULT ( STDMETHODCALLTYPE *OnUserPreferencesChanged3 )( 
            IVsTextManagerEvents3 * This,
            /* [in] */ const VIEWPREFERENCES3 *pViewPrefs,
            /* [in] */ const FRAMEPREFERENCES2 *pFramePrefs,
            /* [in] */ const LANGPREFERENCES2 *pLangPrefs,
            /* [in] */ const FONTCOLORPREFERENCES2 *pColorPrefs);
        
        HRESULT ( STDMETHODCALLTYPE *OnReplaceAllInFilesBegin )( 
            __RPC__in IVsTextManagerEvents3 * This);
        
        HRESULT ( STDMETHODCALLTYPE *OnReplaceAllInFilesEnd )( 
            __RPC__in IVsTextManagerEvents3 * This);
        
        END_INTERFACE
    } IVsTextManagerEvents3Vtbl;

    interface IVsTextManagerEvents3
    {
        CONST_VTBL struct IVsTextManagerEvents3Vtbl *lpVtbl;
    };

    

#ifdef COBJMACROS


#define IVsTextManagerEvents3_QueryInterface(This,riid,ppvObject)	\
    ( (This)->lpVtbl -> QueryInterface(This,riid,ppvObject) ) 

#define IVsTextManagerEvents3_AddRef(This)	\
    ( (This)->lpVtbl -> AddRef(This) ) 

#define IVsTextManagerEvents3_Release(This)	\
    ( (This)->lpVtbl -> Release(This) ) 


#define IVsTextManagerEvents3_OnRegisterMarkerType(This,iMarkerType)	\
    ( (This)->lpVtbl -> OnRegisterMarkerType(This,iMarkerType) ) 

#define IVsTextManagerEvents3_OnRegisterView(This,pView)	\
    ( (This)->lpVtbl -> OnRegisterView(This,pView) ) 

#define IVsTextManagerEvents3_OnUnregisterView(This,pView)	\
    ( (This)->lpVtbl -> OnUnregisterView(This,pView) ) 

#define IVsTextManagerEvents3_OnUserPreferencesChanged3(This,pViewPrefs,pFramePrefs,pLangPrefs,pColorPrefs)	\
    ( (This)->lpVtbl -> OnUserPreferencesChanged3(This,pViewPrefs,pFramePrefs,pLangPrefs,pColorPrefs) ) 

#define IVsTextManagerEvents3_OnReplaceAllInFilesBegin(This)	\
    ( (This)->lpVtbl -> OnReplaceAllInFilesBegin(This) ) 

#define IVsTextManagerEvents3_OnReplaceAllInFilesEnd(This)	\
    ( (This)->lpVtbl -> OnReplaceAllInFilesEnd(This) ) 

#endif /* COBJMACROS */


#endif 	/* C style interface */




#endif 	/* __IVsTextManagerEvents3_INTERFACE_DEFINED__ */


#ifndef __IVsTextView3_INTERFACE_DEFINED__
#define __IVsTextView3_INTERFACE_DEFINED__

/* interface IVsTextView3 */
/* [object][custom][version][uuid] */ 


EXTERN_C const IID IID_IVsTextView3;

#if defined(__cplusplus) && !defined(CINTERFACE)
    
    MIDL_INTERFACE("DC5CECDF-26BF-4014-BF54-A6D3636A83EF")
    IVsTextView3 : public IUnknown
    {
    public:
        virtual HRESULT STDMETHODCALLTYPE AddProjectionAwareCommandFilter( 
            /* [in] */ __RPC__in_opt IOleCommandTarget *pNewCmdTarg,
            /* [out] */ __RPC__deref_out_opt IOleCommandTarget **ppNextCmdTarg) = 0;
        
        virtual HRESULT STDMETHODCALLTYPE DoesViewSupportRole( 
            /* [in] */ __RPC__in LPCOLESTR pszRole,
            /* [out] */ __RPC__out BOOL *pbContainsRole) = 0;
        
        virtual HRESULT STDMETHODCALLTYPE GetCanCaretAndSelectionMapToSurfaceBuffer( 
            /* [out] */ __RPC__out BOOL *pbCanMap) = 0;
        
    };
    
    
#else 	/* C style interface */

    typedef struct IVsTextView3Vtbl
    {
        BEGIN_INTERFACE
        
        HRESULT ( STDMETHODCALLTYPE *QueryInterface )( 
            __RPC__in IVsTextView3 * This,
            /* [in] */ __RPC__in REFIID riid,
            /* [annotation][iid_is][out] */ 
            _COM_Outptr_  void **ppvObject);
        
        ULONG ( STDMETHODCALLTYPE *AddRef )( 
            __RPC__in IVsTextView3 * This);
        
        ULONG ( STDMETHODCALLTYPE *Release )( 
            __RPC__in IVsTextView3 * This);
        
        HRESULT ( STDMETHODCALLTYPE *AddProjectionAwareCommandFilter )( 
            __RPC__in IVsTextView3 * This,
            /* [in] */ __RPC__in_opt IOleCommandTarget *pNewCmdTarg,
            /* [out] */ __RPC__deref_out_opt IOleCommandTarget **ppNextCmdTarg);
        
        HRESULT ( STDMETHODCALLTYPE *DoesViewSupportRole )( 
            __RPC__in IVsTextView3 * This,
            /* [in] */ __RPC__in LPCOLESTR pszRole,
            /* [out] */ __RPC__out BOOL *pbContainsRole);
        
        HRESULT ( STDMETHODCALLTYPE *GetCanCaretAndSelectionMapToSurfaceBuffer )( 
            __RPC__in IVsTextView3 * This,
            /* [out] */ __RPC__out BOOL *pbCanMap);
        
        END_INTERFACE
    } IVsTextView3Vtbl;

    interface IVsTextView3
    {
        CONST_VTBL struct IVsTextView3Vtbl *lpVtbl;
    };

    

#ifdef COBJMACROS


#define IVsTextView3_QueryInterface(This,riid,ppvObject)	\
    ( (This)->lpVtbl -> QueryInterface(This,riid,ppvObject) ) 

#define IVsTextView3_AddRef(This)	\
    ( (This)->lpVtbl -> AddRef(This) ) 

#define IVsTextView3_Release(This)	\
    ( (This)->lpVtbl -> Release(This) ) 


#define IVsTextView3_AddProjectionAwareCommandFilter(This,pNewCmdTarg,ppNextCmdTarg)	\
    ( (This)->lpVtbl -> AddProjectionAwareCommandFilter(This,pNewCmdTarg,ppNextCmdTarg) ) 

#define IVsTextView3_DoesViewSupportRole(This,pszRole,pbContainsRole)	\
    ( (This)->lpVtbl -> DoesViewSupportRole(This,pszRole,pbContainsRole) ) 

#define IVsTextView3_GetCanCaretAndSelectionMapToSurfaceBuffer(This,pbCanMap)	\
    ( (This)->lpVtbl -> GetCanCaretAndSelectionMapToSurfaceBuffer(This,pbCanMap) ) 

#endif /* COBJMACROS */


#endif 	/* C style interface */




#endif 	/* __IVsTextView3_INTERFACE_DEFINED__ */


#ifndef __IVsLanguageDebugInfo3_INTERFACE_DEFINED__
#define __IVsLanguageDebugInfo3_INTERFACE_DEFINED__

/* interface IVsLanguageDebugInfo3 */
/* [object][uuid] */ 


EXTERN_C const IID IID_IVsLanguageDebugInfo3;

#if defined(__cplusplus) && !defined(CINTERFACE)
    
    MIDL_INTERFACE("37F90B98-666A-452F-91F3-C60266AEB13E")
    IVsLanguageDebugInfo3 : public IUnknown
    {
    public:
        virtual HRESULT STDMETHODCALLTYPE GetValidBreakpointLineVariance( 
            /* [in] */ __RPC__in_opt IVsTextBuffer *pBuffer,
            /* [in] */ long iLine,
            /* [in] */ long iCol,
            /* [out] */ __RPC__out long *piVariance) = 0;
        
    };
    
    
#else 	/* C style interface */

    typedef struct IVsLanguageDebugInfo3Vtbl
    {
        BEGIN_INTERFACE
        
        HRESULT ( STDMETHODCALLTYPE *QueryInterface )( 
            __RPC__in IVsLanguageDebugInfo3 * This,
            /* [in] */ __RPC__in REFIID riid,
            /* [annotation][iid_is][out] */ 
            _COM_Outptr_  void **ppvObject);
        
        ULONG ( STDMETHODCALLTYPE *AddRef )( 
            __RPC__in IVsLanguageDebugInfo3 * This);
        
        ULONG ( STDMETHODCALLTYPE *Release )( 
            __RPC__in IVsLanguageDebugInfo3 * This);
        
        HRESULT ( STDMETHODCALLTYPE *GetValidBreakpointLineVariance )( 
            __RPC__in IVsLanguageDebugInfo3 * This,
            /* [in] */ __RPC__in_opt IVsTextBuffer *pBuffer,
            /* [in] */ long iLine,
            /* [in] */ long iCol,
            /* [out] */ __RPC__out long *piVariance);
        
        END_INTERFACE
    } IVsLanguageDebugInfo3Vtbl;

    interface IVsLanguageDebugInfo3
    {
        CONST_VTBL struct IVsLanguageDebugInfo3Vtbl *lpVtbl;
    };

    

#ifdef COBJMACROS


#define IVsLanguageDebugInfo3_QueryInterface(This,riid,ppvObject)	\
    ( (This)->lpVtbl -> QueryInterface(This,riid,ppvObject) ) 

#define IVsLanguageDebugInfo3_AddRef(This)	\
    ( (This)->lpVtbl -> AddRef(This) ) 

#define IVsLanguageDebugInfo3_Release(This)	\
    ( (This)->lpVtbl -> Release(This) ) 


#define IVsLanguageDebugInfo3_GetValidBreakpointLineVariance(This,pBuffer,iLine,iCol,piVariance)	\
    ( (This)->lpVtbl -> GetValidBreakpointLineVariance(This,pBuffer,iLine,iCol,piVariance) ) 

#endif /* COBJMACROS */


#endif 	/* C style interface */




#endif 	/* __IVsLanguageDebugInfo3_INTERFACE_DEFINED__ */

#endif /* __TextMgr110_LIBRARY_DEFINED__ */

/* Additional Prototypes for ALL interfaces */

/* end of Additional Prototypes */

#ifdef __cplusplus
}
#endif

#endif


