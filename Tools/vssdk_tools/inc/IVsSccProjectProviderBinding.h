

/* this ALWAYS GENERATED file contains the definitions for the interfaces */


 /* File created by MIDL compiler version 7.00.0499 */
/* Compiler settings for IVsSccProjectProviderBinding.idl:
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

#ifndef __IVsSccProjectProviderBinding_h__
#define __IVsSccProjectProviderBinding_h__

#if defined(_MSC_VER) && (_MSC_VER >= 1020)
#pragma once
#endif

/* Forward Declarations */ 

#ifndef __IVsSccProjectProviderBinding_FWD_DEFINED__
#define __IVsSccProjectProviderBinding_FWD_DEFINED__
typedef interface IVsSccProjectProviderBinding IVsSccProjectProviderBinding;
#endif 	/* __IVsSccProjectProviderBinding_FWD_DEFINED__ */


/* header files for imported files */
#include "objidl.h"

#ifdef __cplusplus
extern "C"{
#endif 


/* interface __MIDL_itf_IVsSccProjectProviderBinding_0000_0000 */
/* [local] */ 

#pragma once
typedef 
enum __VSSCCPROVIDERBINDING
    {	VSSCC_PB_STANDARD	= 0,
	VSSCC_PB_CUSTOM_DISABLED	= 1,
	VSSCC_PB_CUSTOM	= 2,
	VSSCC_PB_STANDARD_DISABLED	= 3
    } 	VSSCCPROVIDERBINDING;


enum __VSSCCPROVIDERBINDINGOPTIONS
    {	VSSCC_PBO_CANBROWSESERVERPATH	= 0x1,
	VSSCC_PBO_CANEDITSERVERPATH	= 0x2,
	VSSCC_PBO_CANDISPLAYSERVERPATH	= 0x4
    } ;
typedef DWORD VSSCCPROVIDERBINDINGOPTIONS;



extern RPC_IF_HANDLE __MIDL_itf_IVsSccProjectProviderBinding_0000_0000_v0_0_c_ifspec;
extern RPC_IF_HANDLE __MIDL_itf_IVsSccProjectProviderBinding_0000_0000_v0_0_s_ifspec;

#ifndef __IVsSccProjectProviderBinding_INTERFACE_DEFINED__
#define __IVsSccProjectProviderBinding_INTERFACE_DEFINED__

/* interface IVsSccProjectProviderBinding */
/* [object][unique][version][uuid] */ 


EXTERN_C const IID IID_IVsSccProjectProviderBinding;

#if defined(__cplusplus) && !defined(CINTERFACE)
    
    MIDL_INTERFACE("53544c4d-02f8-11d0-8e5e-00a0c911005a")
    IVsSccProjectProviderBinding : public IUnknown
    {
    public:
        virtual HRESULT STDMETHODCALLTYPE GetProviderBinding( 
            /* [retval][out] */ __RPC__out VSSCCPROVIDERBINDING *pvscpbBinding) = 0;
        
        virtual HRESULT STDMETHODCALLTYPE GetProviderService( 
            /* [retval][out] */ __RPC__out GUID *pguidService) = 0;
        
        virtual HRESULT STDMETHODCALLTYPE GetProviderSession( 
            /* [retval][out] */ __RPC__deref_out_opt IUnknown **punkSession) = 0;
        
        virtual HRESULT STDMETHODCALLTYPE TranslateEnlistmentPath( 
            /* [in] */ __RPC__in LPCOLESTR lpszPath,
            /* [out] */ __RPC__out BOOL *pfAlternateIsDisplay,
            /* [out] */ __RPC__deref_out_opt BSTR *pbstrAlternatePath) = 0;
        
        virtual HRESULT STDMETHODCALLTYPE GetProviderBindingOptions( 
            /* [retval][out] */ __RPC__out VSSCCPROVIDERBINDINGOPTIONS *pvscpboOptions) = 0;
        
        virtual HRESULT STDMETHODCALLTYPE ValidateServerPathEdit( 
            /* [in] */ BOOL fQuick,
            /* [in] */ __RPC__in LPCOLESTR lpszServerPath,
            /* [retval][out] */ __RPC__out BOOL *pfValidServer) = 0;
        
        virtual HRESULT STDMETHODCALLTYPE BrowseServerPath( 
            /* [in] */ __RPC__in LPCOLESTR lpszServerPath,
            /* [retval][out] */ __RPC__deref_out_opt BSTR *pbstrNewServerPath) = 0;
        
    };
    
#else 	/* C style interface */

    typedef struct IVsSccProjectProviderBindingVtbl
    {
        BEGIN_INTERFACE
        
        HRESULT ( STDMETHODCALLTYPE *QueryInterface )( 
            IVsSccProjectProviderBinding * This,
            /* [in] */ __RPC__in REFIID riid,
            /* [iid_is][out] */ 
            __RPC__deref_out  void **ppvObject);
        
        ULONG ( STDMETHODCALLTYPE *AddRef )( 
            IVsSccProjectProviderBinding * This);
        
        ULONG ( STDMETHODCALLTYPE *Release )( 
            IVsSccProjectProviderBinding * This);
        
        HRESULT ( STDMETHODCALLTYPE *GetProviderBinding )( 
            IVsSccProjectProviderBinding * This,
            /* [retval][out] */ __RPC__out VSSCCPROVIDERBINDING *pvscpbBinding);
        
        HRESULT ( STDMETHODCALLTYPE *GetProviderService )( 
            IVsSccProjectProviderBinding * This,
            /* [retval][out] */ __RPC__out GUID *pguidService);
        
        HRESULT ( STDMETHODCALLTYPE *GetProviderSession )( 
            IVsSccProjectProviderBinding * This,
            /* [retval][out] */ __RPC__deref_out_opt IUnknown **punkSession);
        
        HRESULT ( STDMETHODCALLTYPE *TranslateEnlistmentPath )( 
            IVsSccProjectProviderBinding * This,
            /* [in] */ __RPC__in LPCOLESTR lpszPath,
            /* [out] */ __RPC__out BOOL *pfAlternateIsDisplay,
            /* [out] */ __RPC__deref_out_opt BSTR *pbstrAlternatePath);
        
        HRESULT ( STDMETHODCALLTYPE *GetProviderBindingOptions )( 
            IVsSccProjectProviderBinding * This,
            /* [retval][out] */ __RPC__out VSSCCPROVIDERBINDINGOPTIONS *pvscpboOptions);
        
        HRESULT ( STDMETHODCALLTYPE *ValidateServerPathEdit )( 
            IVsSccProjectProviderBinding * This,
            /* [in] */ BOOL fQuick,
            /* [in] */ __RPC__in LPCOLESTR lpszServerPath,
            /* [retval][out] */ __RPC__out BOOL *pfValidServer);
        
        HRESULT ( STDMETHODCALLTYPE *BrowseServerPath )( 
            IVsSccProjectProviderBinding * This,
            /* [in] */ __RPC__in LPCOLESTR lpszServerPath,
            /* [retval][out] */ __RPC__deref_out_opt BSTR *pbstrNewServerPath);
        
        END_INTERFACE
    } IVsSccProjectProviderBindingVtbl;

    interface IVsSccProjectProviderBinding
    {
        CONST_VTBL struct IVsSccProjectProviderBindingVtbl *lpVtbl;
    };

    

#ifdef COBJMACROS


#define IVsSccProjectProviderBinding_QueryInterface(This,riid,ppvObject)	\
    ( (This)->lpVtbl -> QueryInterface(This,riid,ppvObject) ) 

#define IVsSccProjectProviderBinding_AddRef(This)	\
    ( (This)->lpVtbl -> AddRef(This) ) 

#define IVsSccProjectProviderBinding_Release(This)	\
    ( (This)->lpVtbl -> Release(This) ) 


#define IVsSccProjectProviderBinding_GetProviderBinding(This,pvscpbBinding)	\
    ( (This)->lpVtbl -> GetProviderBinding(This,pvscpbBinding) ) 

#define IVsSccProjectProviderBinding_GetProviderService(This,pguidService)	\
    ( (This)->lpVtbl -> GetProviderService(This,pguidService) ) 

#define IVsSccProjectProviderBinding_GetProviderSession(This,punkSession)	\
    ( (This)->lpVtbl -> GetProviderSession(This,punkSession) ) 

#define IVsSccProjectProviderBinding_TranslateEnlistmentPath(This,lpszPath,pfAlternateIsDisplay,pbstrAlternatePath)	\
    ( (This)->lpVtbl -> TranslateEnlistmentPath(This,lpszPath,pfAlternateIsDisplay,pbstrAlternatePath) ) 

#define IVsSccProjectProviderBinding_GetProviderBindingOptions(This,pvscpboOptions)	\
    ( (This)->lpVtbl -> GetProviderBindingOptions(This,pvscpboOptions) ) 

#define IVsSccProjectProviderBinding_ValidateServerPathEdit(This,fQuick,lpszServerPath,pfValidServer)	\
    ( (This)->lpVtbl -> ValidateServerPathEdit(This,fQuick,lpszServerPath,pfValidServer) ) 

#define IVsSccProjectProviderBinding_BrowseServerPath(This,lpszServerPath,pbstrNewServerPath)	\
    ( (This)->lpVtbl -> BrowseServerPath(This,lpszServerPath,pbstrNewServerPath) ) 

#endif /* COBJMACROS */


#endif 	/* C style interface */




#endif 	/* __IVsSccProjectProviderBinding_INTERFACE_DEFINED__ */


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


