

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

#ifndef COM_NO_WINDOWS_H
#include "windows.h"
#include "ole2.h"
#endif /*COM_NO_WINDOWS_H*/

#ifndef __textmgr121_h__
#define __textmgr121_h__

#if defined(_MSC_VER) && (_MSC_VER >= 1020)
#pragma once
#endif

/* Forward Declarations */ 

#ifndef __IVsDropdownBar2_FWD_DEFINED__
#define __IVsDropdownBar2_FWD_DEFINED__
typedef interface IVsDropdownBar2 IVsDropdownBar2;

#endif 	/* __IVsDropdownBar2_FWD_DEFINED__ */


#ifndef __IVsDropdownBarClient3_FWD_DEFINED__
#define __IVsDropdownBarClient3_FWD_DEFINED__
typedef interface IVsDropdownBarClient3 IVsDropdownBarClient3;

#endif 	/* __IVsDropdownBarClient3_FWD_DEFINED__ */


#ifndef __IVsCompletionSet3_FWD_DEFINED__
#define __IVsCompletionSet3_FWD_DEFINED__
typedef interface IVsCompletionSet3 IVsCompletionSet3;

#endif 	/* __IVsCompletionSet3_FWD_DEFINED__ */


/* header files for imported files */
#include "oaidl.h"
#include "ocidl.h"
#include "IVsQueryEditQuerySave2.h"
#include "IVsQueryEditQuerySave80.h"
#include "context.h"
#include "textmgr.h"
#include "textmgr2.h"
#include "textmgr100.h"
#include "textmgr110.h"
#include "textmgr120.h"

#ifdef __cplusplus
extern "C"{
#endif 


/* interface __MIDL_itf_textmgr121_0000_0000 */
/* [local] */ 

#pragma once
#pragma once


extern RPC_IF_HANDLE __MIDL_itf_textmgr121_0000_0000_v0_0_c_ifspec;
extern RPC_IF_HANDLE __MIDL_itf_textmgr121_0000_0000_v0_0_s_ifspec;

#ifndef __IVsDropdownBar2_INTERFACE_DEFINED__
#define __IVsDropdownBar2_INTERFACE_DEFINED__

/* interface IVsDropdownBar2 */
/* [object][custom][uuid] */ 


EXTERN_C const IID IID_IVsDropdownBar2;

#if defined(__cplusplus) && !defined(CINTERFACE)
    
    MIDL_INTERFACE("BC766334-6E04-442C-92F1-F687ECF8F741")
    IVsDropdownBar2 : public IUnknown
    {
    public:
        virtual /* [local] */ HRESULT STDMETHODCALLTYPE SetComboEnabled( 
            /* [in] */ long iCombo,
            /* [in] */ BOOL bEnabled) = 0;
        
        virtual /* [local] */ HRESULT STDMETHODCALLTYPE GetComboEnabled( 
            /* [in] */ long iCombo,
            /* [out] */ BOOL *pbEnabled) = 0;
        
    };
    
    
#else 	/* C style interface */

    typedef struct IVsDropdownBar2Vtbl
    {
        BEGIN_INTERFACE
        
        HRESULT ( STDMETHODCALLTYPE *QueryInterface )( 
            __RPC__in IVsDropdownBar2 * This,
            /* [in] */ __RPC__in REFIID riid,
            /* [annotation][iid_is][out] */ 
            _COM_Outptr_  void **ppvObject);
        
        ULONG ( STDMETHODCALLTYPE *AddRef )( 
            __RPC__in IVsDropdownBar2 * This);
        
        ULONG ( STDMETHODCALLTYPE *Release )( 
            __RPC__in IVsDropdownBar2 * This);
        
        /* [local] */ HRESULT ( STDMETHODCALLTYPE *SetComboEnabled )( 
            IVsDropdownBar2 * This,
            /* [in] */ long iCombo,
            /* [in] */ BOOL bEnabled);
        
        /* [local] */ HRESULT ( STDMETHODCALLTYPE *GetComboEnabled )( 
            IVsDropdownBar2 * This,
            /* [in] */ long iCombo,
            /* [out] */ BOOL *pbEnabled);
        
        END_INTERFACE
    } IVsDropdownBar2Vtbl;

    interface IVsDropdownBar2
    {
        CONST_VTBL struct IVsDropdownBar2Vtbl *lpVtbl;
    };

    

#ifdef COBJMACROS


#define IVsDropdownBar2_QueryInterface(This,riid,ppvObject)	\
    ( (This)->lpVtbl -> QueryInterface(This,riid,ppvObject) ) 

#define IVsDropdownBar2_AddRef(This)	\
    ( (This)->lpVtbl -> AddRef(This) ) 

#define IVsDropdownBar2_Release(This)	\
    ( (This)->lpVtbl -> Release(This) ) 


#define IVsDropdownBar2_SetComboEnabled(This,iCombo,bEnabled)	\
    ( (This)->lpVtbl -> SetComboEnabled(This,iCombo,bEnabled) ) 

#define IVsDropdownBar2_GetComboEnabled(This,iCombo,pbEnabled)	\
    ( (This)->lpVtbl -> GetComboEnabled(This,iCombo,pbEnabled) ) 

#endif /* COBJMACROS */


#endif 	/* C style interface */




#endif 	/* __IVsDropdownBar2_INTERFACE_DEFINED__ */


#ifndef __IVsDropdownBarClient3_INTERFACE_DEFINED__
#define __IVsDropdownBarClient3_INTERFACE_DEFINED__

/* interface IVsDropdownBarClient3 */
/* [object][custom][uuid] */ 


EXTERN_C const IID IID_IVsDropdownBarClient3;

#if defined(__cplusplus) && !defined(CINTERFACE)
    
    MIDL_INTERFACE("9F0DC18B-2ED7-435E-B0D2-0ED5577B9635")
    IVsDropdownBarClient3 : public IUnknown
    {
    public:
        virtual /* [local] */ HRESULT STDMETHODCALLTYPE GetComboWidth( 
            /* [in] */ long iCombo,
            /* [out] */ long *piWidthPercent) = 0;
        
        virtual /* [local] */ HRESULT STDMETHODCALLTYPE GetAutomationProperties( 
            /* [in] */ long iCombo,
            /* [out] */ BSTR *pbstrName,
            /* [out] */ BSTR *pbstrId) = 0;
        
        virtual /* [local] */ HRESULT STDMETHODCALLTYPE GetEntryImage( 
            /* [in] */ long iCombo,
            /* [in] */ long iIndex,
            /* [out] */ long *piImageIndex,
            /* [out] */ HANDLE *phImageList) = 0;
        
    };
    
    
#else 	/* C style interface */

    typedef struct IVsDropdownBarClient3Vtbl
    {
        BEGIN_INTERFACE
        
        HRESULT ( STDMETHODCALLTYPE *QueryInterface )( 
            __RPC__in IVsDropdownBarClient3 * This,
            /* [in] */ __RPC__in REFIID riid,
            /* [annotation][iid_is][out] */ 
            _COM_Outptr_  void **ppvObject);
        
        ULONG ( STDMETHODCALLTYPE *AddRef )( 
            __RPC__in IVsDropdownBarClient3 * This);
        
        ULONG ( STDMETHODCALLTYPE *Release )( 
            __RPC__in IVsDropdownBarClient3 * This);
        
        /* [local] */ HRESULT ( STDMETHODCALLTYPE *GetComboWidth )( 
            IVsDropdownBarClient3 * This,
            /* [in] */ long iCombo,
            /* [out] */ long *piWidthPercent);
        
        /* [local] */ HRESULT ( STDMETHODCALLTYPE *GetAutomationProperties )( 
            IVsDropdownBarClient3 * This,
            /* [in] */ long iCombo,
            /* [out] */ BSTR *pbstrName,
            /* [out] */ BSTR *pbstrId);
        
        /* [local] */ HRESULT ( STDMETHODCALLTYPE *GetEntryImage )( 
            IVsDropdownBarClient3 * This,
            /* [in] */ long iCombo,
            /* [in] */ long iIndex,
            /* [out] */ long *piImageIndex,
            /* [out] */ HANDLE *phImageList);
        
        END_INTERFACE
    } IVsDropdownBarClient3Vtbl;

    interface IVsDropdownBarClient3
    {
        CONST_VTBL struct IVsDropdownBarClient3Vtbl *lpVtbl;
    };

    

#ifdef COBJMACROS


#define IVsDropdownBarClient3_QueryInterface(This,riid,ppvObject)	\
    ( (This)->lpVtbl -> QueryInterface(This,riid,ppvObject) ) 

#define IVsDropdownBarClient3_AddRef(This)	\
    ( (This)->lpVtbl -> AddRef(This) ) 

#define IVsDropdownBarClient3_Release(This)	\
    ( (This)->lpVtbl -> Release(This) ) 


#define IVsDropdownBarClient3_GetComboWidth(This,iCombo,piWidthPercent)	\
    ( (This)->lpVtbl -> GetComboWidth(This,iCombo,piWidthPercent) ) 

#define IVsDropdownBarClient3_GetAutomationProperties(This,iCombo,pbstrName,pbstrId)	\
    ( (This)->lpVtbl -> GetAutomationProperties(This,iCombo,pbstrName,pbstrId) ) 

#define IVsDropdownBarClient3_GetEntryImage(This,iCombo,iIndex,piImageIndex,phImageList)	\
    ( (This)->lpVtbl -> GetEntryImage(This,iCombo,iIndex,piImageIndex,phImageList) ) 

#endif /* COBJMACROS */


#endif 	/* C style interface */




#endif 	/* __IVsDropdownBarClient3_INTERFACE_DEFINED__ */


#ifndef __IVsCompletionSet3_INTERFACE_DEFINED__
#define __IVsCompletionSet3_INTERFACE_DEFINED__

/* interface IVsCompletionSet3 */
/* [object][custom][uuid] */ 


EXTERN_C const IID IID_IVsCompletionSet3;

#if defined(__cplusplus) && !defined(CINTERFACE)
    
    MIDL_INTERFACE("B30B03B0-DB30-43C7-A959-C81522988F8E")
    IVsCompletionSet3 : public IUnknown
    {
    public:
        virtual /* [local] */ HRESULT STDMETHODCALLTYPE GetContextIcon( 
            /* [in] */ long iIndex,
            /* [out] */ long *piGlyph) = 0;
        
        virtual /* [local] */ HRESULT STDMETHODCALLTYPE GetContextImageList( 
            /* [out] */ HANDLE *phImageList) = 0;
        
    };
    
    
#else 	/* C style interface */

    typedef struct IVsCompletionSet3Vtbl
    {
        BEGIN_INTERFACE
        
        HRESULT ( STDMETHODCALLTYPE *QueryInterface )( 
            __RPC__in IVsCompletionSet3 * This,
            /* [in] */ __RPC__in REFIID riid,
            /* [annotation][iid_is][out] */ 
            _COM_Outptr_  void **ppvObject);
        
        ULONG ( STDMETHODCALLTYPE *AddRef )( 
            __RPC__in IVsCompletionSet3 * This);
        
        ULONG ( STDMETHODCALLTYPE *Release )( 
            __RPC__in IVsCompletionSet3 * This);
        
        /* [local] */ HRESULT ( STDMETHODCALLTYPE *GetContextIcon )( 
            IVsCompletionSet3 * This,
            /* [in] */ long iIndex,
            /* [out] */ long *piGlyph);
        
        /* [local] */ HRESULT ( STDMETHODCALLTYPE *GetContextImageList )( 
            IVsCompletionSet3 * This,
            /* [out] */ HANDLE *phImageList);
        
        END_INTERFACE
    } IVsCompletionSet3Vtbl;

    interface IVsCompletionSet3
    {
        CONST_VTBL struct IVsCompletionSet3Vtbl *lpVtbl;
    };

    

#ifdef COBJMACROS


#define IVsCompletionSet3_QueryInterface(This,riid,ppvObject)	\
    ( (This)->lpVtbl -> QueryInterface(This,riid,ppvObject) ) 

#define IVsCompletionSet3_AddRef(This)	\
    ( (This)->lpVtbl -> AddRef(This) ) 

#define IVsCompletionSet3_Release(This)	\
    ( (This)->lpVtbl -> Release(This) ) 


#define IVsCompletionSet3_GetContextIcon(This,iIndex,piGlyph)	\
    ( (This)->lpVtbl -> GetContextIcon(This,iIndex,piGlyph) ) 

#define IVsCompletionSet3_GetContextImageList(This,phImageList)	\
    ( (This)->lpVtbl -> GetContextImageList(This,phImageList) ) 

#endif /* COBJMACROS */


#endif 	/* C style interface */




#endif 	/* __IVsCompletionSet3_INTERFACE_DEFINED__ */


/* interface __MIDL_itf_textmgr121_0000_0003 */
/* [local] */ 


enum DROPDOWNENTRYTYPE2
    {
        ENTRY_UNINDENT_COLLAPSED	= 8
    } ;


extern RPC_IF_HANDLE __MIDL_itf_textmgr121_0000_0003_v0_0_c_ifspec;
extern RPC_IF_HANDLE __MIDL_itf_textmgr121_0000_0003_v0_0_s_ifspec;

/* Additional Prototypes for ALL interfaces */

/* end of Additional Prototypes */

#ifdef __cplusplus
}
#endif

#endif


