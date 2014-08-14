

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


#ifndef __textmgr120_h__
#define __textmgr120_h__

#if defined(_MSC_VER) && (_MSC_VER >= 1020)
#pragma once
#endif

/* Forward Declarations */ 

#ifndef __IVsTextManager4_FWD_DEFINED__
#define __IVsTextManager4_FWD_DEFINED__
typedef interface IVsTextManager4 IVsTextManager4;

#endif 	/* __IVsTextManager4_FWD_DEFINED__ */


#ifndef __IVsTextManagerEvents4_FWD_DEFINED__
#define __IVsTextManagerEvents4_FWD_DEFINED__
typedef interface IVsTextManagerEvents4 IVsTextManagerEvents4;

#endif 	/* __IVsTextManagerEvents4_FWD_DEFINED__ */


#ifndef __IVsCodeWindow2_FWD_DEFINED__
#define __IVsCodeWindow2_FWD_DEFINED__
typedef interface IVsCodeWindow2 IVsCodeWindow2;

#endif 	/* __IVsCodeWindow2_FWD_DEFINED__ */


#ifndef __IVsCodeWindowEvents2_FWD_DEFINED__
#define __IVsCodeWindowEvents2_FWD_DEFINED__
typedef interface IVsCodeWindowEvents2 IVsCodeWindowEvents2;

#endif 	/* __IVsCodeWindowEvents2_FWD_DEFINED__ */


/* header files for imported files */
#include "IVsQueryEditQuerySave2.h"
#include "IVsQueryEditQuerySave80.h"
#include "msxml.h"
#include "context.h"
#include "textmgr.h"
#include "textmgr2.h"
#include "textmgr100.h"
#include "textmgr110.h"

#ifdef __cplusplus
extern "C"{
#endif 


/* interface __MIDL_itf_textmgr120_0000_0000 */
/* [local] */ 

#pragma once
#pragma once


extern RPC_IF_HANDLE __MIDL_itf_textmgr120_0000_0000_v0_0_c_ifspec;
extern RPC_IF_HANDLE __MIDL_itf_textmgr120_0000_0000_v0_0_s_ifspec;


#ifndef __TextMgr120_LIBRARY_DEFINED__
#define __TextMgr120_LIBRARY_DEFINED__

/* library TextMgr120 */
/* [version][uuid] */ 

typedef struct _LANGPREFERENCES3
    {
    CHAR szFileType[ 24 ];
    unsigned int fShowCompletion;
    unsigned int fShowSmartIndent;
    unsigned int fHideAdvancedAutoListMembers;
    unsigned int uTabSize;
    unsigned int uIndentSize;
    unsigned int fInsertTabs;
    vsIndentStyle IndentStyle;
    unsigned int fAutoListMembers;
    unsigned int fAutoListParams;
    unsigned int fVirtualSpace;
    unsigned int fWordWrap;
    unsigned int fTwoWayTreeview;
    unsigned int fHotURLs;
    unsigned int fDropdownBar;
    unsigned int fLineNumbers;
    GUID guidLang;
    unsigned int fWordWrapGlyphs;
    unsigned int fCutCopyBlanks;
    unsigned int fShowHorizontalScrollBar;
    unsigned int fShowVerticalScrollBar;
    unsigned int fShowAnnotations;
    unsigned int fShowChanges;
    unsigned int fShowMarks;
    unsigned int fShowErrors;
    unsigned int fShowCaretPosition;
    unsigned int fUseMapMode;
    unsigned int fShowPreview;
    unsigned int uOverviewWidth;
    unsigned int fBraceCompletion;
    } 	LANGPREFERENCES3;


EXTERN_C const IID LIBID_TextMgr120;

#ifndef __IVsTextManager4_INTERFACE_DEFINED__
#define __IVsTextManager4_INTERFACE_DEFINED__

/* interface IVsTextManager4 */
/* [object][custom][version][uuid] */ 


EXTERN_C const IID IID_IVsTextManager4;

#if defined(__cplusplus) && !defined(CINTERFACE)
    
    MIDL_INTERFACE("4125ABF2-621E-4FE8-A4F2-C7A9F5EF2EED")
    IVsTextManager4 : public IUnknown
    {
    public:
        virtual /* [local] */ HRESULT STDMETHODCALLTYPE GetUserPreferences4( 
            /* [out] */ VIEWPREFERENCES3 *pViewPrefs,
            /* [out][in] */ LANGPREFERENCES3 *pLangPrefs,
            /* [out][in] */ FONTCOLORPREFERENCES2 *pColorPrefs) = 0;
        
        virtual /* [local] */ HRESULT STDMETHODCALLTYPE SetUserPreferences4( 
            /* [in] */ const VIEWPREFERENCES3 *pViewPrefs,
            /* [in] */ const LANGPREFERENCES3 *pLangPrefs,
            /* [in] */ const FONTCOLORPREFERENCES2 *pColorPrefs) = 0;
        
    };
    
    
#else 	/* C style interface */

    typedef struct IVsTextManager4Vtbl
    {
        BEGIN_INTERFACE
        
        HRESULT ( STDMETHODCALLTYPE *QueryInterface )( 
            __RPC__in IVsTextManager4 * This,
            /* [in] */ __RPC__in REFIID riid,
            /* [annotation][iid_is][out] */ 
            _COM_Outptr_  void **ppvObject);
        
        ULONG ( STDMETHODCALLTYPE *AddRef )( 
            __RPC__in IVsTextManager4 * This);
        
        ULONG ( STDMETHODCALLTYPE *Release )( 
            __RPC__in IVsTextManager4 * This);
        
        /* [local] */ HRESULT ( STDMETHODCALLTYPE *GetUserPreferences4 )( 
            IVsTextManager4 * This,
            /* [out] */ VIEWPREFERENCES3 *pViewPrefs,
            /* [out][in] */ LANGPREFERENCES3 *pLangPrefs,
            /* [out][in] */ FONTCOLORPREFERENCES2 *pColorPrefs);
        
        /* [local] */ HRESULT ( STDMETHODCALLTYPE *SetUserPreferences4 )( 
            IVsTextManager4 * This,
            /* [in] */ const VIEWPREFERENCES3 *pViewPrefs,
            /* [in] */ const LANGPREFERENCES3 *pLangPrefs,
            /* [in] */ const FONTCOLORPREFERENCES2 *pColorPrefs);
        
        END_INTERFACE
    } IVsTextManager4Vtbl;

    interface IVsTextManager4
    {
        CONST_VTBL struct IVsTextManager4Vtbl *lpVtbl;
    };

    

#ifdef COBJMACROS


#define IVsTextManager4_QueryInterface(This,riid,ppvObject)	\
    ( (This)->lpVtbl -> QueryInterface(This,riid,ppvObject) ) 

#define IVsTextManager4_AddRef(This)	\
    ( (This)->lpVtbl -> AddRef(This) ) 

#define IVsTextManager4_Release(This)	\
    ( (This)->lpVtbl -> Release(This) ) 


#define IVsTextManager4_GetUserPreferences4(This,pViewPrefs,pLangPrefs,pColorPrefs)	\
    ( (This)->lpVtbl -> GetUserPreferences4(This,pViewPrefs,pLangPrefs,pColorPrefs) ) 

#define IVsTextManager4_SetUserPreferences4(This,pViewPrefs,pLangPrefs,pColorPrefs)	\
    ( (This)->lpVtbl -> SetUserPreferences4(This,pViewPrefs,pLangPrefs,pColorPrefs) ) 

#endif /* COBJMACROS */


#endif 	/* C style interface */




#endif 	/* __IVsTextManager4_INTERFACE_DEFINED__ */


#ifndef __IVsTextManagerEvents4_INTERFACE_DEFINED__
#define __IVsTextManagerEvents4_INTERFACE_DEFINED__

/* interface IVsTextManagerEvents4 */
/* [object][custom][version][uuid] */ 


EXTERN_C const IID IID_IVsTextManagerEvents4;

#if defined(__cplusplus) && !defined(CINTERFACE)
    
    MIDL_INTERFACE("0298015A-2C85-4076-9483-F56D91BA14E2")
    IVsTextManagerEvents4 : public IUnknown
    {
    public:
        virtual /* [local] */ HRESULT STDMETHODCALLTYPE OnUserPreferencesChanged4( 
            /* [in] */ const VIEWPREFERENCES3 *pViewPrefs,
            /* [in] */ const LANGPREFERENCES3 *pLangPrefs,
            /* [in] */ const FONTCOLORPREFERENCES2 *pColorPrefs) = 0;
        
    };
    
    
#else 	/* C style interface */

    typedef struct IVsTextManagerEvents4Vtbl
    {
        BEGIN_INTERFACE
        
        HRESULT ( STDMETHODCALLTYPE *QueryInterface )( 
            __RPC__in IVsTextManagerEvents4 * This,
            /* [in] */ __RPC__in REFIID riid,
            /* [annotation][iid_is][out] */ 
            _COM_Outptr_  void **ppvObject);
        
        ULONG ( STDMETHODCALLTYPE *AddRef )( 
            __RPC__in IVsTextManagerEvents4 * This);
        
        ULONG ( STDMETHODCALLTYPE *Release )( 
            __RPC__in IVsTextManagerEvents4 * This);
        
        /* [local] */ HRESULT ( STDMETHODCALLTYPE *OnUserPreferencesChanged4 )( 
            IVsTextManagerEvents4 * This,
            /* [in] */ const VIEWPREFERENCES3 *pViewPrefs,
            /* [in] */ const LANGPREFERENCES3 *pLangPrefs,
            /* [in] */ const FONTCOLORPREFERENCES2 *pColorPrefs);
        
        END_INTERFACE
    } IVsTextManagerEvents4Vtbl;

    interface IVsTextManagerEvents4
    {
        CONST_VTBL struct IVsTextManagerEvents4Vtbl *lpVtbl;
    };

    

#ifdef COBJMACROS


#define IVsTextManagerEvents4_QueryInterface(This,riid,ppvObject)	\
    ( (This)->lpVtbl -> QueryInterface(This,riid,ppvObject) ) 

#define IVsTextManagerEvents4_AddRef(This)	\
    ( (This)->lpVtbl -> AddRef(This) ) 

#define IVsTextManagerEvents4_Release(This)	\
    ( (This)->lpVtbl -> Release(This) ) 


#define IVsTextManagerEvents4_OnUserPreferencesChanged4(This,pViewPrefs,pLangPrefs,pColorPrefs)	\
    ( (This)->lpVtbl -> OnUserPreferencesChanged4(This,pViewPrefs,pLangPrefs,pColorPrefs) ) 

#endif /* COBJMACROS */


#endif 	/* C style interface */




#endif 	/* __IVsTextManagerEvents4_INTERFACE_DEFINED__ */


#ifndef __IVsCodeWindow2_INTERFACE_DEFINED__
#define __IVsCodeWindow2_INTERFACE_DEFINED__

/* interface IVsCodeWindow2 */
/* [object][custom][uuid] */ 


EXTERN_C const IID IID_IVsCodeWindow2;

#if defined(__cplusplus) && !defined(CINTERFACE)
    
    MIDL_INTERFACE("81A91FB7-7625-461A-8A9C-B1CC701ECCC5")
    IVsCodeWindow2 : public IUnknown
    {
    public:
        virtual /* [local] */ HRESULT STDMETHODCALLTYPE GetEmbeddedCodeWindowCount( 
            /* [out] */ long *piCount) = 0;
        
        virtual /* [local] */ HRESULT STDMETHODCALLTYPE GetEmbeddedCodeWindow( 
            /* [in] */ long iIndex,
            /* [out] */ IVsCodeWindow **ppCodeWindow) = 0;
        
        virtual /* [local] */ HRESULT STDMETHODCALLTYPE GetContainingCodeWindow( 
            /* [out] */ IVsCodeWindow **ppCodeWindow) = 0;
        
    };
    
    
#else 	/* C style interface */

    typedef struct IVsCodeWindow2Vtbl
    {
        BEGIN_INTERFACE
        
        HRESULT ( STDMETHODCALLTYPE *QueryInterface )( 
            __RPC__in IVsCodeWindow2 * This,
            /* [in] */ __RPC__in REFIID riid,
            /* [annotation][iid_is][out] */ 
            _COM_Outptr_  void **ppvObject);
        
        ULONG ( STDMETHODCALLTYPE *AddRef )( 
            __RPC__in IVsCodeWindow2 * This);
        
        ULONG ( STDMETHODCALLTYPE *Release )( 
            __RPC__in IVsCodeWindow2 * This);
        
        /* [local] */ HRESULT ( STDMETHODCALLTYPE *GetEmbeddedCodeWindowCount )( 
            IVsCodeWindow2 * This,
            /* [out] */ long *piCount);
        
        /* [local] */ HRESULT ( STDMETHODCALLTYPE *GetEmbeddedCodeWindow )( 
            IVsCodeWindow2 * This,
            /* [in] */ long iIndex,
            /* [out] */ IVsCodeWindow **ppCodeWindow);
        
        /* [local] */ HRESULT ( STDMETHODCALLTYPE *GetContainingCodeWindow )( 
            IVsCodeWindow2 * This,
            /* [out] */ IVsCodeWindow **ppCodeWindow);
        
        END_INTERFACE
    } IVsCodeWindow2Vtbl;

    interface IVsCodeWindow2
    {
        CONST_VTBL struct IVsCodeWindow2Vtbl *lpVtbl;
    };

    

#ifdef COBJMACROS


#define IVsCodeWindow2_QueryInterface(This,riid,ppvObject)	\
    ( (This)->lpVtbl -> QueryInterface(This,riid,ppvObject) ) 

#define IVsCodeWindow2_AddRef(This)	\
    ( (This)->lpVtbl -> AddRef(This) ) 

#define IVsCodeWindow2_Release(This)	\
    ( (This)->lpVtbl -> Release(This) ) 


#define IVsCodeWindow2_GetEmbeddedCodeWindowCount(This,piCount)	\
    ( (This)->lpVtbl -> GetEmbeddedCodeWindowCount(This,piCount) ) 

#define IVsCodeWindow2_GetEmbeddedCodeWindow(This,iIndex,ppCodeWindow)	\
    ( (This)->lpVtbl -> GetEmbeddedCodeWindow(This,iIndex,ppCodeWindow) ) 

#define IVsCodeWindow2_GetContainingCodeWindow(This,ppCodeWindow)	\
    ( (This)->lpVtbl -> GetContainingCodeWindow(This,ppCodeWindow) ) 

#endif /* COBJMACROS */


#endif 	/* C style interface */




#endif 	/* __IVsCodeWindow2_INTERFACE_DEFINED__ */


#ifndef __IVsCodeWindowEvents2_INTERFACE_DEFINED__
#define __IVsCodeWindowEvents2_INTERFACE_DEFINED__

/* interface IVsCodeWindowEvents2 */
/* [object][custom][uuid] */ 


EXTERN_C const IID IID_IVsCodeWindowEvents2;

#if defined(__cplusplus) && !defined(CINTERFACE)
    
    MIDL_INTERFACE("6703F718-2939-40C2-8F8F-AB393378AEC8")
    IVsCodeWindowEvents2 : public IUnknown
    {
    public:
        virtual /* [local] */ HRESULT STDMETHODCALLTYPE OnNewEmbeddedCodeWindow( 
            /* [in] */ IVsCodeWindow *pCodeWindow) = 0;
        
        virtual /* [local] */ HRESULT STDMETHODCALLTYPE OnCloseEmbeddedCodeWindow( 
            /* [in] */ IVsCodeWindow *pCodeWindow) = 0;
        
    };
    
    
#else 	/* C style interface */

    typedef struct IVsCodeWindowEvents2Vtbl
    {
        BEGIN_INTERFACE
        
        HRESULT ( STDMETHODCALLTYPE *QueryInterface )( 
            __RPC__in IVsCodeWindowEvents2 * This,
            /* [in] */ __RPC__in REFIID riid,
            /* [annotation][iid_is][out] */ 
            _COM_Outptr_  void **ppvObject);
        
        ULONG ( STDMETHODCALLTYPE *AddRef )( 
            __RPC__in IVsCodeWindowEvents2 * This);
        
        ULONG ( STDMETHODCALLTYPE *Release )( 
            __RPC__in IVsCodeWindowEvents2 * This);
        
        /* [local] */ HRESULT ( STDMETHODCALLTYPE *OnNewEmbeddedCodeWindow )( 
            IVsCodeWindowEvents2 * This,
            /* [in] */ IVsCodeWindow *pCodeWindow);
        
        /* [local] */ HRESULT ( STDMETHODCALLTYPE *OnCloseEmbeddedCodeWindow )( 
            IVsCodeWindowEvents2 * This,
            /* [in] */ IVsCodeWindow *pCodeWindow);
        
        END_INTERFACE
    } IVsCodeWindowEvents2Vtbl;

    interface IVsCodeWindowEvents2
    {
        CONST_VTBL struct IVsCodeWindowEvents2Vtbl *lpVtbl;
    };

    

#ifdef COBJMACROS


#define IVsCodeWindowEvents2_QueryInterface(This,riid,ppvObject)	\
    ( (This)->lpVtbl -> QueryInterface(This,riid,ppvObject) ) 

#define IVsCodeWindowEvents2_AddRef(This)	\
    ( (This)->lpVtbl -> AddRef(This) ) 

#define IVsCodeWindowEvents2_Release(This)	\
    ( (This)->lpVtbl -> Release(This) ) 


#define IVsCodeWindowEvents2_OnNewEmbeddedCodeWindow(This,pCodeWindow)	\
    ( (This)->lpVtbl -> OnNewEmbeddedCodeWindow(This,pCodeWindow) ) 

#define IVsCodeWindowEvents2_OnCloseEmbeddedCodeWindow(This,pCodeWindow)	\
    ( (This)->lpVtbl -> OnCloseEmbeddedCodeWindow(This,pCodeWindow) ) 

#endif /* COBJMACROS */


#endif 	/* C style interface */




#endif 	/* __IVsCodeWindowEvents2_INTERFACE_DEFINED__ */

#endif /* __TextMgr120_LIBRARY_DEFINED__ */

/* Additional Prototypes for ALL interfaces */

/* end of Additional Prototypes */

#ifdef __cplusplus
}
#endif

#endif


