

/* this ALWAYS GENERATED file contains the definitions for the interfaces */


 /* File created by MIDL compiler version 7.00.0499 */
/* Compiler settings for IVsSccProjectEnlistmentFactory.idl:
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

#ifndef __IVsSccProjectEnlistmentFactory_h__
#define __IVsSccProjectEnlistmentFactory_h__

#if defined(_MSC_VER) && (_MSC_VER >= 1020)
#pragma once
#endif

/* Forward Declarations */ 

#ifndef __IVsSccProjectEnlistmentFactory_FWD_DEFINED__
#define __IVsSccProjectEnlistmentFactory_FWD_DEFINED__
typedef interface IVsSccProjectEnlistmentFactory IVsSccProjectEnlistmentFactory;
#endif 	/* __IVsSccProjectEnlistmentFactory_FWD_DEFINED__ */


/* header files for imported files */
#include "objidl.h"

#ifdef __cplusplus
extern "C"{
#endif 


/* interface __MIDL_itf_IVsSccProjectEnlistmentFactory_0000_0000 */
/* [local] */ 


enum __VSSCCENLISTMENTFACTORYOPTIONS
    {	VSSCC_EFO_CANBROWSEENLISTMENTPATH	= 0x1,
	VSSCC_EFO_CANEDITENLISTMENTPATH	= 0x2,
	VSSCC_EFO_CANBROWSEDEBUGGINGPATH	= 0x4,
	VSSCC_EFO_CANEDITDEBUGGINGPATH	= 0x8
    } ;
typedef DWORD VSSCCENLISTMENTFACTORYOPTIONS;



extern RPC_IF_HANDLE __MIDL_itf_IVsSccProjectEnlistmentFactory_0000_0000_v0_0_c_ifspec;
extern RPC_IF_HANDLE __MIDL_itf_IVsSccProjectEnlistmentFactory_0000_0000_v0_0_s_ifspec;

#ifndef __IVsSccProjectEnlistmentFactory_INTERFACE_DEFINED__
#define __IVsSccProjectEnlistmentFactory_INTERFACE_DEFINED__

/* interface IVsSccProjectEnlistmentFactory */
/* [object][unique][version][uuid] */ 


EXTERN_C const IID IID_IVsSccProjectEnlistmentFactory;

#if defined(__cplusplus) && !defined(CINTERFACE)
    
    MIDL_INTERFACE("53544c4d-00f8-11d0-8e5e-00a0c911005a")
    IVsSccProjectEnlistmentFactory : public IUnknown
    {
    public:
        virtual HRESULT STDMETHODCALLTYPE GetDefaultEnlistment( 
            /* [in] */ __RPC__in LPCOLESTR lpszProjectPath,
            /* [out] */ __RPC__deref_out_opt BSTR *pbstrDefaultEnlistment,
            /* [out] */ __RPC__deref_out_opt BSTR *pbstrDefaultEnlistmentUNC) = 0;
        
        virtual HRESULT STDMETHODCALLTYPE GetEnlistmentFactoryOptions( 
            /* [retval][out] */ __RPC__out VSSCCENLISTMENTFACTORYOPTIONS *pvscefoOptions) = 0;
        
        virtual HRESULT STDMETHODCALLTYPE ValidateEnlistmentEdit( 
            /* [in] */ BOOL fQuick,
            /* [in] */ __RPC__in LPCOLESTR lpszProjectPath,
            /* [in] */ __RPC__in LPCOLESTR lpszChosenEnlistment,
            /* [out] */ __RPC__deref_out_opt BSTR *pbstrChosenEnlistmentUNC,
            /* [out] */ __RPC__out BOOL *pfValidEnlistment) = 0;
        
        virtual HRESULT STDMETHODCALLTYPE BrowseEnlistment( 
            /* [in] */ __RPC__in LPCOLESTR lpszProjectPath,
            /* [in] */ __RPC__in LPCOLESTR lpszInitialEnlistment,
            /* [out] */ __RPC__deref_out_opt BSTR *pbstrChosenEnlistment,
            /* [out] */ __RPC__deref_out_opt BSTR *pbstrChosenEnlistmentUNC) = 0;
        
        virtual HRESULT STDMETHODCALLTYPE OnBeforeEnlistmentCreate( 
            /* [in] */ __RPC__in LPCOLESTR lpszProjectPath,
            /* [in] */ __RPC__in LPCOLESTR lpszEnlistment,
            /* [in] */ __RPC__in LPCOLESTR lpszEnlistmentUNC) = 0;
        
        virtual HRESULT STDMETHODCALLTYPE OnAfterEnlistmentCreate( 
            /* [in] */ __RPC__in LPCOLESTR lpszProjectPath,
            /* [in] */ __RPC__in LPCOLESTR lpszEnlistment,
            /* [in] */ __RPC__in LPCOLESTR lpszEnlistmentUNC) = 0;
        
    };
    
#else 	/* C style interface */

    typedef struct IVsSccProjectEnlistmentFactoryVtbl
    {
        BEGIN_INTERFACE
        
        HRESULT ( STDMETHODCALLTYPE *QueryInterface )( 
            IVsSccProjectEnlistmentFactory * This,
            /* [in] */ __RPC__in REFIID riid,
            /* [iid_is][out] */ 
            __RPC__deref_out  void **ppvObject);
        
        ULONG ( STDMETHODCALLTYPE *AddRef )( 
            IVsSccProjectEnlistmentFactory * This);
        
        ULONG ( STDMETHODCALLTYPE *Release )( 
            IVsSccProjectEnlistmentFactory * This);
        
        HRESULT ( STDMETHODCALLTYPE *GetDefaultEnlistment )( 
            IVsSccProjectEnlistmentFactory * This,
            /* [in] */ __RPC__in LPCOLESTR lpszProjectPath,
            /* [out] */ __RPC__deref_out_opt BSTR *pbstrDefaultEnlistment,
            /* [out] */ __RPC__deref_out_opt BSTR *pbstrDefaultEnlistmentUNC);
        
        HRESULT ( STDMETHODCALLTYPE *GetEnlistmentFactoryOptions )( 
            IVsSccProjectEnlistmentFactory * This,
            /* [retval][out] */ __RPC__out VSSCCENLISTMENTFACTORYOPTIONS *pvscefoOptions);
        
        HRESULT ( STDMETHODCALLTYPE *ValidateEnlistmentEdit )( 
            IVsSccProjectEnlistmentFactory * This,
            /* [in] */ BOOL fQuick,
            /* [in] */ __RPC__in LPCOLESTR lpszProjectPath,
            /* [in] */ __RPC__in LPCOLESTR lpszChosenEnlistment,
            /* [out] */ __RPC__deref_out_opt BSTR *pbstrChosenEnlistmentUNC,
            /* [out] */ __RPC__out BOOL *pfValidEnlistment);
        
        HRESULT ( STDMETHODCALLTYPE *BrowseEnlistment )( 
            IVsSccProjectEnlistmentFactory * This,
            /* [in] */ __RPC__in LPCOLESTR lpszProjectPath,
            /* [in] */ __RPC__in LPCOLESTR lpszInitialEnlistment,
            /* [out] */ __RPC__deref_out_opt BSTR *pbstrChosenEnlistment,
            /* [out] */ __RPC__deref_out_opt BSTR *pbstrChosenEnlistmentUNC);
        
        HRESULT ( STDMETHODCALLTYPE *OnBeforeEnlistmentCreate )( 
            IVsSccProjectEnlistmentFactory * This,
            /* [in] */ __RPC__in LPCOLESTR lpszProjectPath,
            /* [in] */ __RPC__in LPCOLESTR lpszEnlistment,
            /* [in] */ __RPC__in LPCOLESTR lpszEnlistmentUNC);
        
        HRESULT ( STDMETHODCALLTYPE *OnAfterEnlistmentCreate )( 
            IVsSccProjectEnlistmentFactory * This,
            /* [in] */ __RPC__in LPCOLESTR lpszProjectPath,
            /* [in] */ __RPC__in LPCOLESTR lpszEnlistment,
            /* [in] */ __RPC__in LPCOLESTR lpszEnlistmentUNC);
        
        END_INTERFACE
    } IVsSccProjectEnlistmentFactoryVtbl;

    interface IVsSccProjectEnlistmentFactory
    {
        CONST_VTBL struct IVsSccProjectEnlistmentFactoryVtbl *lpVtbl;
    };

    

#ifdef COBJMACROS


#define IVsSccProjectEnlistmentFactory_QueryInterface(This,riid,ppvObject)	\
    ( (This)->lpVtbl -> QueryInterface(This,riid,ppvObject) ) 

#define IVsSccProjectEnlistmentFactory_AddRef(This)	\
    ( (This)->lpVtbl -> AddRef(This) ) 

#define IVsSccProjectEnlistmentFactory_Release(This)	\
    ( (This)->lpVtbl -> Release(This) ) 


#define IVsSccProjectEnlistmentFactory_GetDefaultEnlistment(This,lpszProjectPath,pbstrDefaultEnlistment,pbstrDefaultEnlistmentUNC)	\
    ( (This)->lpVtbl -> GetDefaultEnlistment(This,lpszProjectPath,pbstrDefaultEnlistment,pbstrDefaultEnlistmentUNC) ) 

#define IVsSccProjectEnlistmentFactory_GetEnlistmentFactoryOptions(This,pvscefoOptions)	\
    ( (This)->lpVtbl -> GetEnlistmentFactoryOptions(This,pvscefoOptions) ) 

#define IVsSccProjectEnlistmentFactory_ValidateEnlistmentEdit(This,fQuick,lpszProjectPath,lpszChosenEnlistment,pbstrChosenEnlistmentUNC,pfValidEnlistment)	\
    ( (This)->lpVtbl -> ValidateEnlistmentEdit(This,fQuick,lpszProjectPath,lpszChosenEnlistment,pbstrChosenEnlistmentUNC,pfValidEnlistment) ) 

#define IVsSccProjectEnlistmentFactory_BrowseEnlistment(This,lpszProjectPath,lpszInitialEnlistment,pbstrChosenEnlistment,pbstrChosenEnlistmentUNC)	\
    ( (This)->lpVtbl -> BrowseEnlistment(This,lpszProjectPath,lpszInitialEnlistment,pbstrChosenEnlistment,pbstrChosenEnlistmentUNC) ) 

#define IVsSccProjectEnlistmentFactory_OnBeforeEnlistmentCreate(This,lpszProjectPath,lpszEnlistment,lpszEnlistmentUNC)	\
    ( (This)->lpVtbl -> OnBeforeEnlistmentCreate(This,lpszProjectPath,lpszEnlistment,lpszEnlistmentUNC) ) 

#define IVsSccProjectEnlistmentFactory_OnAfterEnlistmentCreate(This,lpszProjectPath,lpszEnlistment,lpszEnlistmentUNC)	\
    ( (This)->lpVtbl -> OnAfterEnlistmentCreate(This,lpszProjectPath,lpszEnlistment,lpszEnlistmentUNC) ) 

#endif /* COBJMACROS */


#endif 	/* C style interface */




#endif 	/* __IVsSccProjectEnlistmentFactory_INTERFACE_DEFINED__ */


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


