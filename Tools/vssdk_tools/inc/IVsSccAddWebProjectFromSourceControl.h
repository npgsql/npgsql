

/* this ALWAYS GENERATED file contains the definitions for the interfaces */


 /* File created by MIDL compiler version 7.00.0499 */
/* Compiler settings for IVsSccAddWebProjectFromSourceControl.idl:
    Oicf, W1, Zp8, env=Win32 (32b run)
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

#ifndef __IVsSccAddWebProjectFromSourceControl_h__
#define __IVsSccAddWebProjectFromSourceControl_h__

#if defined(_MSC_VER) && (_MSC_VER >= 1020)
#pragma once
#endif

/* Forward Declarations */ 

#ifndef __IVsSccAddWebProjectFromSourceControl_FWD_DEFINED__
#define __IVsSccAddWebProjectFromSourceControl_FWD_DEFINED__
typedef interface IVsSccAddWebProjectFromSourceControl IVsSccAddWebProjectFromSourceControl;
#endif 	/* __IVsSccAddWebProjectFromSourceControl_FWD_DEFINED__ */


/* header files for imported files */
#include "oaidl.h"

#ifdef __cplusplus
extern "C"{
#endif 


/* interface __MIDL_itf_IVsSccAddWebProjectFromSourceControl_0000_0000 */
/* [local] */ 

#pragma once


extern RPC_IF_HANDLE __MIDL_itf_IVsSccAddWebProjectFromSourceControl_0000_0000_v0_0_c_ifspec;
extern RPC_IF_HANDLE __MIDL_itf_IVsSccAddWebProjectFromSourceControl_0000_0000_v0_0_s_ifspec;

#ifndef __IVsSccAddWebProjectFromSourceControl_INTERFACE_DEFINED__
#define __IVsSccAddWebProjectFromSourceControl_INTERFACE_DEFINED__

/* interface IVsSccAddWebProjectFromSourceControl */
/* [object][uuid] */ 


EXTERN_C const IID IID_IVsSccAddWebProjectFromSourceControl;

#if defined(__cplusplus) && !defined(CINTERFACE)
    
    MIDL_INTERFACE("53544c4d-724A-46C8-8D01-6D4518CA118C")
    IVsSccAddWebProjectFromSourceControl : public IUnknown
    {
    public:
        virtual HRESULT STDMETHODCALLTYPE IsAddWebProjectSupported( 
            /* [out] */ __RPC__out VARIANT_BOOL *pfResult) = 0;
        
        virtual HRESULT STDMETHODCALLTYPE BrowseForServerLocation( 
            /* [out] */ __RPC__deref_out_opt BSTR *pbstrLocationDescription,
            /* [out] */ __RPC__deref_out_opt BSTR *pbstrLocalPath,
            /* [out] */ __RPC__deref_out_opt BSTR *pbstrDatabasePath,
            /* [out] */ __RPC__deref_out_opt BSTR *pbstrAuxiliarPath,
            /* [out] */ __RPC__deref_out_opt BSTR *pbstrProviderName) = 0;
        
        virtual HRESULT STDMETHODCALLTYPE AddWebProjectFromSourceControl( 
            /* [in] */ __RPC__in BSTR bstrLocalPath,
            /* [in] */ __RPC__in BSTR bstrDatabasePath,
            /* [in] */ __RPC__in BSTR bstrAuxiliarPath,
            /* [in] */ __RPC__in BSTR bstrProviderName,
            /* [in] */ __RPC__in BSTR bstrDebuggingPath) = 0;
        
    };
    
#else 	/* C style interface */

    typedef struct IVsSccAddWebProjectFromSourceControlVtbl
    {
        BEGIN_INTERFACE
        
        HRESULT ( STDMETHODCALLTYPE *QueryInterface )( 
            IVsSccAddWebProjectFromSourceControl * This,
            /* [in] */ __RPC__in REFIID riid,
            /* [iid_is][out] */ 
            __RPC__deref_out  void **ppvObject);
        
        ULONG ( STDMETHODCALLTYPE *AddRef )( 
            IVsSccAddWebProjectFromSourceControl * This);
        
        ULONG ( STDMETHODCALLTYPE *Release )( 
            IVsSccAddWebProjectFromSourceControl * This);
        
        HRESULT ( STDMETHODCALLTYPE *IsAddWebProjectSupported )( 
            IVsSccAddWebProjectFromSourceControl * This,
            /* [out] */ __RPC__out VARIANT_BOOL *pfResult);
        
        HRESULT ( STDMETHODCALLTYPE *BrowseForServerLocation )( 
            IVsSccAddWebProjectFromSourceControl * This,
            /* [out] */ __RPC__deref_out_opt BSTR *pbstrLocationDescription,
            /* [out] */ __RPC__deref_out_opt BSTR *pbstrLocalPath,
            /* [out] */ __RPC__deref_out_opt BSTR *pbstrDatabasePath,
            /* [out] */ __RPC__deref_out_opt BSTR *pbstrAuxiliarPath,
            /* [out] */ __RPC__deref_out_opt BSTR *pbstrProviderName);
        
        HRESULT ( STDMETHODCALLTYPE *AddWebProjectFromSourceControl )( 
            IVsSccAddWebProjectFromSourceControl * This,
            /* [in] */ __RPC__in BSTR bstrLocalPath,
            /* [in] */ __RPC__in BSTR bstrDatabasePath,
            /* [in] */ __RPC__in BSTR bstrAuxiliarPath,
            /* [in] */ __RPC__in BSTR bstrProviderName,
            /* [in] */ __RPC__in BSTR bstrDebuggingPath);
        
        END_INTERFACE
    } IVsSccAddWebProjectFromSourceControlVtbl;

    interface IVsSccAddWebProjectFromSourceControl
    {
        CONST_VTBL struct IVsSccAddWebProjectFromSourceControlVtbl *lpVtbl;
    };

    

#ifdef COBJMACROS


#define IVsSccAddWebProjectFromSourceControl_QueryInterface(This,riid,ppvObject)	\
    ( (This)->lpVtbl -> QueryInterface(This,riid,ppvObject) ) 

#define IVsSccAddWebProjectFromSourceControl_AddRef(This)	\
    ( (This)->lpVtbl -> AddRef(This) ) 

#define IVsSccAddWebProjectFromSourceControl_Release(This)	\
    ( (This)->lpVtbl -> Release(This) ) 


#define IVsSccAddWebProjectFromSourceControl_IsAddWebProjectSupported(This,pfResult)	\
    ( (This)->lpVtbl -> IsAddWebProjectSupported(This,pfResult) ) 

#define IVsSccAddWebProjectFromSourceControl_BrowseForServerLocation(This,pbstrLocationDescription,pbstrLocalPath,pbstrDatabasePath,pbstrAuxiliarPath,pbstrProviderName)	\
    ( (This)->lpVtbl -> BrowseForServerLocation(This,pbstrLocationDescription,pbstrLocalPath,pbstrDatabasePath,pbstrAuxiliarPath,pbstrProviderName) ) 

#define IVsSccAddWebProjectFromSourceControl_AddWebProjectFromSourceControl(This,bstrLocalPath,bstrDatabasePath,bstrAuxiliarPath,bstrProviderName,bstrDebuggingPath)	\
    ( (This)->lpVtbl -> AddWebProjectFromSourceControl(This,bstrLocalPath,bstrDatabasePath,bstrAuxiliarPath,bstrProviderName,bstrDebuggingPath) ) 

#endif /* COBJMACROS */


#endif 	/* C style interface */




#endif 	/* __IVsSccAddWebProjectFromSourceControl_INTERFACE_DEFINED__ */


/* Additional Prototypes for ALL interfaces */

unsigned long             __RPC_USER  BSTR_UserSize(     unsigned long *, unsigned long            , BSTR * ); 
unsigned char * __RPC_USER  BSTR_UserMarshal(  unsigned long *, unsigned char *, BSTR * ); 
unsigned char * __RPC_USER  BSTR_UserUnmarshal(unsigned long *, unsigned char *, BSTR * ); 
void                      __RPC_USER  BSTR_UserFree(     unsigned long *, BSTR * ); 

/* end of Additional Prototypes */

#ifdef __cplusplus
}
#endif

#endif


