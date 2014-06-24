

/* this ALWAYS GENERATED file contains the definitions for the interfaces */


 /* File created by MIDL compiler version 7.00.0499 */
/* Compiler settings for completion.idl:
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

#ifndef __completion_h__
#define __completion_h__

#if defined(_MSC_VER) && (_MSC_VER >= 1020)
#pragma once
#endif

/* Forward Declarations */ 

#ifndef __IVsCommandWindowCompletion_FWD_DEFINED__
#define __IVsCommandWindowCompletion_FWD_DEFINED__
typedef interface IVsCommandWindowCompletion IVsCommandWindowCompletion;
#endif 	/* __IVsCommandWindowCompletion_FWD_DEFINED__ */


#ifndef __IVsImmediateStatementCompletion_FWD_DEFINED__
#define __IVsImmediateStatementCompletion_FWD_DEFINED__
typedef interface IVsImmediateStatementCompletion IVsImmediateStatementCompletion;
#endif 	/* __IVsImmediateStatementCompletion_FWD_DEFINED__ */


#ifndef __IVsImmediateStatementCompletion2_FWD_DEFINED__
#define __IVsImmediateStatementCompletion2_FWD_DEFINED__
typedef interface IVsImmediateStatementCompletion2 IVsImmediateStatementCompletion2;
#endif 	/* __IVsImmediateStatementCompletion2_FWD_DEFINED__ */


/* header files for imported files */
#include "textmgr.h"

#ifdef __cplusplus
extern "C"{
#endif 


/* interface __MIDL_itf_completion_0000_0000 */
/* [local] */ 

#pragma once


extern RPC_IF_HANDLE __MIDL_itf_completion_0000_0000_v0_0_c_ifspec;
extern RPC_IF_HANDLE __MIDL_itf_completion_0000_0000_v0_0_s_ifspec;

#ifndef __IVsCommandWindowCompletion_INTERFACE_DEFINED__
#define __IVsCommandWindowCompletion_INTERFACE_DEFINED__

/* interface IVsCommandWindowCompletion */
/* [object][version][uuid] */ 


EXTERN_C const IID IID_IVsCommandWindowCompletion;

#if defined(__cplusplus) && !defined(CINTERFACE)
    
    MIDL_INTERFACE("34896BBB-A3D5-4c80-BCCE-E9271BEEDC11")
    IVsCommandWindowCompletion : public IUnknown
    {
    public:
        virtual HRESULT STDMETHODCALLTYPE SetCompletionContext( 
            /* [in] */ __RPC__in LPCOLESTR pszFilePath,
            /* [in] */ __RPC__in_opt IVsTextLines *pBuffer,
            /* [in] */ __RPC__in const TextSpan *ptsCurStatement,
            /* [in] */ __RPC__in_opt IUnknown *punkContext) = 0;
        
    };
    
#else 	/* C style interface */

    typedef struct IVsCommandWindowCompletionVtbl
    {
        BEGIN_INTERFACE
        
        HRESULT ( STDMETHODCALLTYPE *QueryInterface )( 
            IVsCommandWindowCompletion * This,
            /* [in] */ __RPC__in REFIID riid,
            /* [iid_is][out] */ 
            __RPC__deref_out  void **ppvObject);
        
        ULONG ( STDMETHODCALLTYPE *AddRef )( 
            IVsCommandWindowCompletion * This);
        
        ULONG ( STDMETHODCALLTYPE *Release )( 
            IVsCommandWindowCompletion * This);
        
        HRESULT ( STDMETHODCALLTYPE *SetCompletionContext )( 
            IVsCommandWindowCompletion * This,
            /* [in] */ __RPC__in LPCOLESTR pszFilePath,
            /* [in] */ __RPC__in_opt IVsTextLines *pBuffer,
            /* [in] */ __RPC__in const TextSpan *ptsCurStatement,
            /* [in] */ __RPC__in_opt IUnknown *punkContext);
        
        END_INTERFACE
    } IVsCommandWindowCompletionVtbl;

    interface IVsCommandWindowCompletion
    {
        CONST_VTBL struct IVsCommandWindowCompletionVtbl *lpVtbl;
    };

    

#ifdef COBJMACROS


#define IVsCommandWindowCompletion_QueryInterface(This,riid,ppvObject)	\
    ( (This)->lpVtbl -> QueryInterface(This,riid,ppvObject) ) 

#define IVsCommandWindowCompletion_AddRef(This)	\
    ( (This)->lpVtbl -> AddRef(This) ) 

#define IVsCommandWindowCompletion_Release(This)	\
    ( (This)->lpVtbl -> Release(This) ) 


#define IVsCommandWindowCompletion_SetCompletionContext(This,pszFilePath,pBuffer,ptsCurStatement,punkContext)	\
    ( (This)->lpVtbl -> SetCompletionContext(This,pszFilePath,pBuffer,ptsCurStatement,punkContext) ) 

#endif /* COBJMACROS */


#endif 	/* C style interface */




#endif 	/* __IVsCommandWindowCompletion_INTERFACE_DEFINED__ */


#ifndef __IVsImmediateStatementCompletion_INTERFACE_DEFINED__
#define __IVsImmediateStatementCompletion_INTERFACE_DEFINED__

/* interface IVsImmediateStatementCompletion */
/* [object][version][uuid] */ 


EXTERN_C const IID IID_IVsImmediateStatementCompletion;

#if defined(__cplusplus) && !defined(CINTERFACE)
    
    MIDL_INTERFACE("5CE7AE60-7B66-4301-8892-90BC0B49A89B")
    IVsImmediateStatementCompletion : public IUnknown
    {
    public:
        virtual HRESULT STDMETHODCALLTYPE InstallStatementCompletion( 
            /* [in] */ BOOL fInstall,
            /* [in] */ __RPC__in_opt IVsTextView *pCmdWinView,
            /* [in] */ BOOL fInitialEnable) = 0;
        
        virtual /* [restricted][hidden] */ HRESULT STDMETHODCALLTYPE EnableStatementCompletion_Deprecated( 
            /* [in] */ BOOL fEnable,
            /* [in] */ CharIndex iStartIndex,
            /* [in] */ CharIndex iEndIndex) = 0;
        
        virtual /* [restricted][hidden] */ HRESULT STDMETHODCALLTYPE SetCompletionContext_Deprecated( 
            /* [in] */ __RPC__in LPCOLESTR pszFilePath,
            /* [in] */ __RPC__in_opt IVsTextLines *pBuffer,
            /* [in] */ __RPC__in const TextSpan *ptsCurStatement,
            /* [in] */ __RPC__in_opt IUnknown *punkContext) = 0;
        
    };
    
#else 	/* C style interface */

    typedef struct IVsImmediateStatementCompletionVtbl
    {
        BEGIN_INTERFACE
        
        HRESULT ( STDMETHODCALLTYPE *QueryInterface )( 
            IVsImmediateStatementCompletion * This,
            /* [in] */ __RPC__in REFIID riid,
            /* [iid_is][out] */ 
            __RPC__deref_out  void **ppvObject);
        
        ULONG ( STDMETHODCALLTYPE *AddRef )( 
            IVsImmediateStatementCompletion * This);
        
        ULONG ( STDMETHODCALLTYPE *Release )( 
            IVsImmediateStatementCompletion * This);
        
        HRESULT ( STDMETHODCALLTYPE *InstallStatementCompletion )( 
            IVsImmediateStatementCompletion * This,
            /* [in] */ BOOL fInstall,
            /* [in] */ __RPC__in_opt IVsTextView *pCmdWinView,
            /* [in] */ BOOL fInitialEnable);
        
        /* [restricted][hidden] */ HRESULT ( STDMETHODCALLTYPE *EnableStatementCompletion_Deprecated )( 
            IVsImmediateStatementCompletion * This,
            /* [in] */ BOOL fEnable,
            /* [in] */ CharIndex iStartIndex,
            /* [in] */ CharIndex iEndIndex);
        
        /* [restricted][hidden] */ HRESULT ( STDMETHODCALLTYPE *SetCompletionContext_Deprecated )( 
            IVsImmediateStatementCompletion * This,
            /* [in] */ __RPC__in LPCOLESTR pszFilePath,
            /* [in] */ __RPC__in_opt IVsTextLines *pBuffer,
            /* [in] */ __RPC__in const TextSpan *ptsCurStatement,
            /* [in] */ __RPC__in_opt IUnknown *punkContext);
        
        END_INTERFACE
    } IVsImmediateStatementCompletionVtbl;

    interface IVsImmediateStatementCompletion
    {
        CONST_VTBL struct IVsImmediateStatementCompletionVtbl *lpVtbl;
    };

    

#ifdef COBJMACROS


#define IVsImmediateStatementCompletion_QueryInterface(This,riid,ppvObject)	\
    ( (This)->lpVtbl -> QueryInterface(This,riid,ppvObject) ) 

#define IVsImmediateStatementCompletion_AddRef(This)	\
    ( (This)->lpVtbl -> AddRef(This) ) 

#define IVsImmediateStatementCompletion_Release(This)	\
    ( (This)->lpVtbl -> Release(This) ) 


#define IVsImmediateStatementCompletion_InstallStatementCompletion(This,fInstall,pCmdWinView,fInitialEnable)	\
    ( (This)->lpVtbl -> InstallStatementCompletion(This,fInstall,pCmdWinView,fInitialEnable) ) 

#define IVsImmediateStatementCompletion_EnableStatementCompletion_Deprecated(This,fEnable,iStartIndex,iEndIndex)	\
    ( (This)->lpVtbl -> EnableStatementCompletion_Deprecated(This,fEnable,iStartIndex,iEndIndex) ) 

#define IVsImmediateStatementCompletion_SetCompletionContext_Deprecated(This,pszFilePath,pBuffer,ptsCurStatement,punkContext)	\
    ( (This)->lpVtbl -> SetCompletionContext_Deprecated(This,pszFilePath,pBuffer,ptsCurStatement,punkContext) ) 

#endif /* COBJMACROS */


#endif 	/* C style interface */




#endif 	/* __IVsImmediateStatementCompletion_INTERFACE_DEFINED__ */


#ifndef __IVsImmediateStatementCompletion2_INTERFACE_DEFINED__
#define __IVsImmediateStatementCompletion2_INTERFACE_DEFINED__

/* interface IVsImmediateStatementCompletion2 */
/* [object][version][uuid] */ 


EXTERN_C const IID IID_IVsImmediateStatementCompletion2;

#if defined(__cplusplus) && !defined(CINTERFACE)
    
    MIDL_INTERFACE("58F03D6E-F781-4e44-BD88-3BDE817CBDCD")
    IVsImmediateStatementCompletion2 : public IVsImmediateStatementCompletion
    {
    public:
        virtual HRESULT STDMETHODCALLTYPE EnableStatementCompletion( 
            /* [in] */ BOOL fEnable,
            /* [in] */ CharIndex iStartIndex,
            /* [in] */ CharIndex iEndIndex,
            /* [in] */ __RPC__in_opt IVsTextView *pTextView) = 0;
        
        virtual HRESULT STDMETHODCALLTYPE SetCompletionContext( 
            /* [in] */ __RPC__in LPCOLESTR pszFilePath,
            /* [in] */ __RPC__in_opt IVsTextLines *pBuffer,
            /* [in] */ __RPC__in const TextSpan *ptsCurStatement,
            /* [in] */ __RPC__in_opt IUnknown *punkContext,
            /* [in] */ __RPC__in_opt IVsTextView *pTextView) = 0;
        
        virtual HRESULT STDMETHODCALLTYPE GetFilter( 
            /* [in] */ __RPC__in_opt IVsTextView *pTextView,
            /* [out] */ __RPC__deref_out_opt IVsTextViewFilter **ppFilter) = 0;
        
    };
    
#else 	/* C style interface */

    typedef struct IVsImmediateStatementCompletion2Vtbl
    {
        BEGIN_INTERFACE
        
        HRESULT ( STDMETHODCALLTYPE *QueryInterface )( 
            IVsImmediateStatementCompletion2 * This,
            /* [in] */ __RPC__in REFIID riid,
            /* [iid_is][out] */ 
            __RPC__deref_out  void **ppvObject);
        
        ULONG ( STDMETHODCALLTYPE *AddRef )( 
            IVsImmediateStatementCompletion2 * This);
        
        ULONG ( STDMETHODCALLTYPE *Release )( 
            IVsImmediateStatementCompletion2 * This);
        
        HRESULT ( STDMETHODCALLTYPE *InstallStatementCompletion )( 
            IVsImmediateStatementCompletion2 * This,
            /* [in] */ BOOL fInstall,
            /* [in] */ __RPC__in_opt IVsTextView *pCmdWinView,
            /* [in] */ BOOL fInitialEnable);
        
        /* [restricted][hidden] */ HRESULT ( STDMETHODCALLTYPE *EnableStatementCompletion_Deprecated )( 
            IVsImmediateStatementCompletion2 * This,
            /* [in] */ BOOL fEnable,
            /* [in] */ CharIndex iStartIndex,
            /* [in] */ CharIndex iEndIndex);
        
        /* [restricted][hidden] */ HRESULT ( STDMETHODCALLTYPE *SetCompletionContext_Deprecated )( 
            IVsImmediateStatementCompletion2 * This,
            /* [in] */ __RPC__in LPCOLESTR pszFilePath,
            /* [in] */ __RPC__in_opt IVsTextLines *pBuffer,
            /* [in] */ __RPC__in const TextSpan *ptsCurStatement,
            /* [in] */ __RPC__in_opt IUnknown *punkContext);
        
        HRESULT ( STDMETHODCALLTYPE *EnableStatementCompletion )( 
            IVsImmediateStatementCompletion2 * This,
            /* [in] */ BOOL fEnable,
            /* [in] */ CharIndex iStartIndex,
            /* [in] */ CharIndex iEndIndex,
            /* [in] */ __RPC__in_opt IVsTextView *pTextView);
        
        HRESULT ( STDMETHODCALLTYPE *SetCompletionContext )( 
            IVsImmediateStatementCompletion2 * This,
            /* [in] */ __RPC__in LPCOLESTR pszFilePath,
            /* [in] */ __RPC__in_opt IVsTextLines *pBuffer,
            /* [in] */ __RPC__in const TextSpan *ptsCurStatement,
            /* [in] */ __RPC__in_opt IUnknown *punkContext,
            /* [in] */ __RPC__in_opt IVsTextView *pTextView);
        
        HRESULT ( STDMETHODCALLTYPE *GetFilter )( 
            IVsImmediateStatementCompletion2 * This,
            /* [in] */ __RPC__in_opt IVsTextView *pTextView,
            /* [out] */ __RPC__deref_out_opt IVsTextViewFilter **ppFilter);
        
        END_INTERFACE
    } IVsImmediateStatementCompletion2Vtbl;

    interface IVsImmediateStatementCompletion2
    {
        CONST_VTBL struct IVsImmediateStatementCompletion2Vtbl *lpVtbl;
    };

    

#ifdef COBJMACROS


#define IVsImmediateStatementCompletion2_QueryInterface(This,riid,ppvObject)	\
    ( (This)->lpVtbl -> QueryInterface(This,riid,ppvObject) ) 

#define IVsImmediateStatementCompletion2_AddRef(This)	\
    ( (This)->lpVtbl -> AddRef(This) ) 

#define IVsImmediateStatementCompletion2_Release(This)	\
    ( (This)->lpVtbl -> Release(This) ) 


#define IVsImmediateStatementCompletion2_InstallStatementCompletion(This,fInstall,pCmdWinView,fInitialEnable)	\
    ( (This)->lpVtbl -> InstallStatementCompletion(This,fInstall,pCmdWinView,fInitialEnable) ) 

#define IVsImmediateStatementCompletion2_EnableStatementCompletion_Deprecated(This,fEnable,iStartIndex,iEndIndex)	\
    ( (This)->lpVtbl -> EnableStatementCompletion_Deprecated(This,fEnable,iStartIndex,iEndIndex) ) 

#define IVsImmediateStatementCompletion2_SetCompletionContext_Deprecated(This,pszFilePath,pBuffer,ptsCurStatement,punkContext)	\
    ( (This)->lpVtbl -> SetCompletionContext_Deprecated(This,pszFilePath,pBuffer,ptsCurStatement,punkContext) ) 


#define IVsImmediateStatementCompletion2_EnableStatementCompletion(This,fEnable,iStartIndex,iEndIndex,pTextView)	\
    ( (This)->lpVtbl -> EnableStatementCompletion(This,fEnable,iStartIndex,iEndIndex,pTextView) ) 

#define IVsImmediateStatementCompletion2_SetCompletionContext(This,pszFilePath,pBuffer,ptsCurStatement,punkContext,pTextView)	\
    ( (This)->lpVtbl -> SetCompletionContext(This,pszFilePath,pBuffer,ptsCurStatement,punkContext,pTextView) ) 

#define IVsImmediateStatementCompletion2_GetFilter(This,pTextView,ppFilter)	\
    ( (This)->lpVtbl -> GetFilter(This,pTextView,ppFilter) ) 

#endif /* COBJMACROS */


#endif 	/* C style interface */




#endif 	/* __IVsImmediateStatementCompletion2_INTERFACE_DEFINED__ */


/* Additional Prototypes for ALL interfaces */

/* end of Additional Prototypes */

#ifdef __cplusplus
}
#endif

#endif


