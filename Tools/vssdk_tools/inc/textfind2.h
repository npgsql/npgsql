

/* this ALWAYS GENERATED file contains the definitions for the interfaces */


 /* File created by MIDL compiler version 7.00.0499 */
/* Compiler settings for textfind2.idl:
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

#ifndef __textfind2_h__
#define __textfind2_h__

#if defined(_MSC_VER) && (_MSC_VER >= 1020)
#pragma once
#endif

/* Forward Declarations */ 

#ifndef __IVsFindTarget2_FWD_DEFINED__
#define __IVsFindTarget2_FWD_DEFINED__
typedef interface IVsFindTarget2 IVsFindTarget2;
#endif 	/* __IVsFindTarget2_FWD_DEFINED__ */


#ifndef __IVsFindCancelDialog_FWD_DEFINED__
#define __IVsFindCancelDialog_FWD_DEFINED__
typedef interface IVsFindCancelDialog IVsFindCancelDialog;
#endif 	/* __IVsFindCancelDialog_FWD_DEFINED__ */


/* header files for imported files */
#include "oaidl.h"
#include "textfind.h"

#ifdef __cplusplus
extern "C"{
#endif 


/* interface __MIDL_itf_textfind2_0000_0000 */
/* [local] */ 

#pragma once


extern RPC_IF_HANDLE __MIDL_itf_textfind2_0000_0000_v0_0_c_ifspec;
extern RPC_IF_HANDLE __MIDL_itf_textfind2_0000_0000_v0_0_s_ifspec;

#ifndef __IVsFindTarget2_INTERFACE_DEFINED__
#define __IVsFindTarget2_INTERFACE_DEFINED__

/* interface IVsFindTarget2 */
/* [object][unique][version][uuid] */ 


EXTERN_C const IID IID_IVsFindTarget2;

#if defined(__cplusplus) && !defined(CINTERFACE)
    
    MIDL_INTERFACE("DE311250-1F14-430e-B896-EFBDDD8ABB3E")
    IVsFindTarget2 : public IUnknown
    {
    public:
        virtual HRESULT STDMETHODCALLTYPE NavigateTo2( 
            /* [in] */ __RPC__in_opt IVsTextSpanSet *pSpans,
            /* [in] */ enum _TextSelMode iSelMode) = 0;
        
    };
    
#else 	/* C style interface */

    typedef struct IVsFindTarget2Vtbl
    {
        BEGIN_INTERFACE
        
        HRESULT ( STDMETHODCALLTYPE *QueryInterface )( 
            IVsFindTarget2 * This,
            /* [in] */ __RPC__in REFIID riid,
            /* [iid_is][out] */ 
            __RPC__deref_out  void **ppvObject);
        
        ULONG ( STDMETHODCALLTYPE *AddRef )( 
            IVsFindTarget2 * This);
        
        ULONG ( STDMETHODCALLTYPE *Release )( 
            IVsFindTarget2 * This);
        
        HRESULT ( STDMETHODCALLTYPE *NavigateTo2 )( 
            IVsFindTarget2 * This,
            /* [in] */ __RPC__in_opt IVsTextSpanSet *pSpans,
            /* [in] */ enum _TextSelMode iSelMode);
        
        END_INTERFACE
    } IVsFindTarget2Vtbl;

    interface IVsFindTarget2
    {
        CONST_VTBL struct IVsFindTarget2Vtbl *lpVtbl;
    };

    

#ifdef COBJMACROS


#define IVsFindTarget2_QueryInterface(This,riid,ppvObject)	\
    ( (This)->lpVtbl -> QueryInterface(This,riid,ppvObject) ) 

#define IVsFindTarget2_AddRef(This)	\
    ( (This)->lpVtbl -> AddRef(This) ) 

#define IVsFindTarget2_Release(This)	\
    ( (This)->lpVtbl -> Release(This) ) 


#define IVsFindTarget2_NavigateTo2(This,pSpans,iSelMode)	\
    ( (This)->lpVtbl -> NavigateTo2(This,pSpans,iSelMode) ) 

#endif /* COBJMACROS */


#endif 	/* C style interface */




#endif 	/* __IVsFindTarget2_INTERFACE_DEFINED__ */


/* interface __MIDL_itf_textfind2_0000_0001 */
/* [local] */ 

typedef /* [public] */ 
enum __MIDL___MIDL_itf_textfind2_0000_0001_0001
    {	VSFE_NoErrorAnsiPattern	= 1
    } 	VSFINDERROR2;


enum __VSFINDRESULT2
    {	VSFR_ReplaceIncompleteEOL	= 0x10000000,
	VSFR_CancelledBeforeReplacementsMade	= 0x20000000
    } ;
typedef DWORD VSFINDRESULT2;


enum __VSFINDOPTIONS2
    {	FR_RegExprLineBreaks	= 0x4000,
	FR_BlockThread	= 0x20000000,
	FR_DoNotUpdateUI	= 0x40000000
    } ;
typedef DWORD VSFINDOPTIONS2;


enum __VSFTPROPID2
    {	VSFTPROPID_IsFindInFilesForegroundOnly	= 6
    } ;
typedef DWORD VSFTPROPID2;



extern RPC_IF_HANDLE __MIDL_itf_textfind2_0000_0001_v0_0_c_ifspec;
extern RPC_IF_HANDLE __MIDL_itf_textfind2_0000_0001_v0_0_s_ifspec;

#ifndef __IVsFindCancelDialog_INTERFACE_DEFINED__
#define __IVsFindCancelDialog_INTERFACE_DEFINED__

/* interface IVsFindCancelDialog */
/* [object][unique][version][uuid] */ 


EXTERN_C const IID IID_IVsFindCancelDialog;

#if defined(__cplusplus) && !defined(CINTERFACE)
    
    MIDL_INTERFACE("FE7C62A2-C121-4995-9EC1-561B80D2DA11")
    IVsFindCancelDialog : public IUnknown
    {
    public:
        virtual HRESULT STDMETHODCALLTYPE LaunchDialog( void) = 0;
        
        virtual HRESULT STDMETHODCALLTYPE QueryDialog( 
            /* [out] */ __RPC__out BOOL *pfCancel) = 0;
        
        virtual HRESULT STDMETHODCALLTYPE CloseDialog( void) = 0;
        
    };
    
#else 	/* C style interface */

    typedef struct IVsFindCancelDialogVtbl
    {
        BEGIN_INTERFACE
        
        HRESULT ( STDMETHODCALLTYPE *QueryInterface )( 
            IVsFindCancelDialog * This,
            /* [in] */ __RPC__in REFIID riid,
            /* [iid_is][out] */ 
            __RPC__deref_out  void **ppvObject);
        
        ULONG ( STDMETHODCALLTYPE *AddRef )( 
            IVsFindCancelDialog * This);
        
        ULONG ( STDMETHODCALLTYPE *Release )( 
            IVsFindCancelDialog * This);
        
        HRESULT ( STDMETHODCALLTYPE *LaunchDialog )( 
            IVsFindCancelDialog * This);
        
        HRESULT ( STDMETHODCALLTYPE *QueryDialog )( 
            IVsFindCancelDialog * This,
            /* [out] */ __RPC__out BOOL *pfCancel);
        
        HRESULT ( STDMETHODCALLTYPE *CloseDialog )( 
            IVsFindCancelDialog * This);
        
        END_INTERFACE
    } IVsFindCancelDialogVtbl;

    interface IVsFindCancelDialog
    {
        CONST_VTBL struct IVsFindCancelDialogVtbl *lpVtbl;
    };

    

#ifdef COBJMACROS


#define IVsFindCancelDialog_QueryInterface(This,riid,ppvObject)	\
    ( (This)->lpVtbl -> QueryInterface(This,riid,ppvObject) ) 

#define IVsFindCancelDialog_AddRef(This)	\
    ( (This)->lpVtbl -> AddRef(This) ) 

#define IVsFindCancelDialog_Release(This)	\
    ( (This)->lpVtbl -> Release(This) ) 


#define IVsFindCancelDialog_LaunchDialog(This)	\
    ( (This)->lpVtbl -> LaunchDialog(This) ) 

#define IVsFindCancelDialog_QueryDialog(This,pfCancel)	\
    ( (This)->lpVtbl -> QueryDialog(This,pfCancel) ) 

#define IVsFindCancelDialog_CloseDialog(This)	\
    ( (This)->lpVtbl -> CloseDialog(This) ) 

#endif /* COBJMACROS */


#endif 	/* C style interface */




#endif 	/* __IVsFindCancelDialog_INTERFACE_DEFINED__ */


/* Additional Prototypes for ALL interfaces */

/* end of Additional Prototypes */

#ifdef __cplusplus
}
#endif

#endif


