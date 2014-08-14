

/* this ALWAYS GENERATED file contains the definitions for the interfaces */


 /* File created by MIDL compiler version 7.00.0499 */
/* Compiler settings for textmgr90.idl:
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


#ifndef __textmgr90_h__
#define __textmgr90_h__

#if defined(_MSC_VER) && (_MSC_VER >= 1020)
#pragma once
#endif

/* Forward Declarations */ 

#ifndef __IVsLanguageDebugInfoRemap_FWD_DEFINED__
#define __IVsLanguageDebugInfoRemap_FWD_DEFINED__
typedef interface IVsLanguageDebugInfoRemap IVsLanguageDebugInfoRemap;
#endif 	/* __IVsLanguageDebugInfoRemap_FWD_DEFINED__ */


#ifndef __IVsHiddenColorableItemInTextEditorCategory_FWD_DEFINED__
#define __IVsHiddenColorableItemInTextEditorCategory_FWD_DEFINED__
typedef interface IVsHiddenColorableItemInTextEditorCategory IVsHiddenColorableItemInTextEditorCategory;
#endif 	/* __IVsHiddenColorableItemInTextEditorCategory_FWD_DEFINED__ */


/* header files for imported files */
#include "oaidl.h"
#include "ocidl.h"

#ifdef __cplusplus
extern "C"{
#endif 


/* interface __MIDL_itf_textmgr90_0000_0000 */
/* [local] */ 

#pragma once



extern RPC_IF_HANDLE __MIDL_itf_textmgr90_0000_0000_v0_0_c_ifspec;
extern RPC_IF_HANDLE __MIDL_itf_textmgr90_0000_0000_v0_0_s_ifspec;


#ifndef __TextMgr90_LIBRARY_DEFINED__
#define __TextMgr90_LIBRARY_DEFINED__

/* library TextMgr90 */
/* [version][uuid] */ 

typedef 
enum _tag_DATA_OBJECT_RENDER_HINT90
    {	DORH_COPY_IN_SAME_DOC	= 0x20
    } 	DATA_OBJECT_RENDER_HINT90;

typedef 
enum _markerbehaviorflags3
    {	MB_HIDE_IN_FONTCOLOR_TEXTEDITOR_CATEGORY	= 0x200
    } 	MARKERBEHAVIORFLAGS3;

typedef 
enum _CompletionStatusFlags2
    {	CSF_DONTSELECTONTAB	= 0x100
    } 	UpdateCompletionFlags2;


EXTERN_C const IID LIBID_TextMgr90;

#ifndef __IVsLanguageDebugInfoRemap_INTERFACE_DEFINED__
#define __IVsLanguageDebugInfoRemap_INTERFACE_DEFINED__

/* interface IVsLanguageDebugInfoRemap */
/* [local][unique][uuid][object] */ 


EXTERN_C const IID IID_IVsLanguageDebugInfoRemap;

#if defined(__cplusplus) && !defined(CINTERFACE)
    
    MIDL_INTERFACE("7E3FAAE7-F89E-448b-A5EB-C7D73E0685F5")
    IVsLanguageDebugInfoRemap : public IUnknown
    {
    public:
        virtual HRESULT STDMETHODCALLTYPE RemapBreakpoint( 
            /* [in] */ IUnknown *pUserBreakpointRequest,
            /* [out] */ IUnknown **ppMappedBreakpointRequest) = 0;
        
    };
    
#else 	/* C style interface */

    typedef struct IVsLanguageDebugInfoRemapVtbl
    {
        BEGIN_INTERFACE
        
        HRESULT ( STDMETHODCALLTYPE *QueryInterface )( 
            IVsLanguageDebugInfoRemap * This,
            /* [in] */ REFIID riid,
            /* [iid_is][out] */ 
            __RPC__deref_out  void **ppvObject);
        
        ULONG ( STDMETHODCALLTYPE *AddRef )( 
            IVsLanguageDebugInfoRemap * This);
        
        ULONG ( STDMETHODCALLTYPE *Release )( 
            IVsLanguageDebugInfoRemap * This);
        
        HRESULT ( STDMETHODCALLTYPE *RemapBreakpoint )( 
            IVsLanguageDebugInfoRemap * This,
            /* [in] */ IUnknown *pUserBreakpointRequest,
            /* [out] */ IUnknown **ppMappedBreakpointRequest);
        
        END_INTERFACE
    } IVsLanguageDebugInfoRemapVtbl;

    interface IVsLanguageDebugInfoRemap
    {
        CONST_VTBL struct IVsLanguageDebugInfoRemapVtbl *lpVtbl;
    };

    

#ifdef COBJMACROS


#define IVsLanguageDebugInfoRemap_QueryInterface(This,riid,ppvObject)	\
    ( (This)->lpVtbl -> QueryInterface(This,riid,ppvObject) ) 

#define IVsLanguageDebugInfoRemap_AddRef(This)	\
    ( (This)->lpVtbl -> AddRef(This) ) 

#define IVsLanguageDebugInfoRemap_Release(This)	\
    ( (This)->lpVtbl -> Release(This) ) 


#define IVsLanguageDebugInfoRemap_RemapBreakpoint(This,pUserBreakpointRequest,ppMappedBreakpointRequest)	\
    ( (This)->lpVtbl -> RemapBreakpoint(This,pUserBreakpointRequest,ppMappedBreakpointRequest) ) 

#endif /* COBJMACROS */


#endif 	/* C style interface */




#endif 	/* __IVsLanguageDebugInfoRemap_INTERFACE_DEFINED__ */


#ifndef __IVsHiddenColorableItemInTextEditorCategory_INTERFACE_DEFINED__
#define __IVsHiddenColorableItemInTextEditorCategory_INTERFACE_DEFINED__

/* interface IVsHiddenColorableItemInTextEditorCategory */
/* [object][unique][version][uuid] */ 


EXTERN_C const IID IID_IVsHiddenColorableItemInTextEditorCategory;

#if defined(__cplusplus) && !defined(CINTERFACE)
    
    MIDL_INTERFACE("FAE9C935-0DA2-476b-AD3C-D73FB69432D7")
    IVsHiddenColorableItemInTextEditorCategory : public IUnknown
    {
    public:
    };
    
#else 	/* C style interface */

    typedef struct IVsHiddenColorableItemInTextEditorCategoryVtbl
    {
        BEGIN_INTERFACE
        
        HRESULT ( STDMETHODCALLTYPE *QueryInterface )( 
            IVsHiddenColorableItemInTextEditorCategory * This,
            /* [in] */ __RPC__in REFIID riid,
            /* [iid_is][out] */ 
            __RPC__deref_out  void **ppvObject);
        
        ULONG ( STDMETHODCALLTYPE *AddRef )( 
            IVsHiddenColorableItemInTextEditorCategory * This);
        
        ULONG ( STDMETHODCALLTYPE *Release )( 
            IVsHiddenColorableItemInTextEditorCategory * This);
        
        END_INTERFACE
    } IVsHiddenColorableItemInTextEditorCategoryVtbl;

    interface IVsHiddenColorableItemInTextEditorCategory
    {
        CONST_VTBL struct IVsHiddenColorableItemInTextEditorCategoryVtbl *lpVtbl;
    };

    

#ifdef COBJMACROS


#define IVsHiddenColorableItemInTextEditorCategory_QueryInterface(This,riid,ppvObject)	\
    ( (This)->lpVtbl -> QueryInterface(This,riid,ppvObject) ) 

#define IVsHiddenColorableItemInTextEditorCategory_AddRef(This)	\
    ( (This)->lpVtbl -> AddRef(This) ) 

#define IVsHiddenColorableItemInTextEditorCategory_Release(This)	\
    ( (This)->lpVtbl -> Release(This) ) 


#endif /* COBJMACROS */


#endif 	/* C style interface */




#endif 	/* __IVsHiddenColorableItemInTextEditorCategory_INTERFACE_DEFINED__ */

#endif /* __TextMgr90_LIBRARY_DEFINED__ */

/* Additional Prototypes for ALL interfaces */

/* end of Additional Prototypes */

#ifdef __cplusplus
}
#endif

#endif


