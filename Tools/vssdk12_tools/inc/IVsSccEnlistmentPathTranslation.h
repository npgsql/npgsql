

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

#ifndef __IVsSccEnlistmentPathTranslation_h__
#define __IVsSccEnlistmentPathTranslation_h__

#if defined(_MSC_VER) && (_MSC_VER >= 1020)
#pragma once
#endif

/* Forward Declarations */ 

#ifndef __IVsSccEnlistmentPathTranslation_FWD_DEFINED__
#define __IVsSccEnlistmentPathTranslation_FWD_DEFINED__
typedef interface IVsSccEnlistmentPathTranslation IVsSccEnlistmentPathTranslation;

#endif 	/* __IVsSccEnlistmentPathTranslation_FWD_DEFINED__ */


/* header files for imported files */
#include "objidl.h"

#ifdef __cplusplus
extern "C"{
#endif 


/* interface __MIDL_itf_IVsSccEnlistmentPathTranslation_0000_0000 */
/* [local] */ 

#pragma once


extern RPC_IF_HANDLE __MIDL_itf_IVsSccEnlistmentPathTranslation_0000_0000_v0_0_c_ifspec;
extern RPC_IF_HANDLE __MIDL_itf_IVsSccEnlistmentPathTranslation_0000_0000_v0_0_s_ifspec;

#ifndef __IVsSccEnlistmentPathTranslation_INTERFACE_DEFINED__
#define __IVsSccEnlistmentPathTranslation_INTERFACE_DEFINED__

/* interface IVsSccEnlistmentPathTranslation */
/* [object][unique][version][uuid] */ 


EXTERN_C const IID IID_IVsSccEnlistmentPathTranslation;

#if defined(__cplusplus) && !defined(CINTERFACE)
    
    MIDL_INTERFACE("53544c4d-01f8-11d0-8e5e-00a0c911005a")
    IVsSccEnlistmentPathTranslation : public IUnknown
    {
    public:
        virtual HRESULT STDMETHODCALLTYPE TranslateProjectPathToEnlistmentPath( 
            /* [in] */ __RPC__in LPCOLESTR lpszProjectPath,
            /* [out] */ __RPC__deref_out_opt BSTR *pbstrEnlistmentPath,
            /* [out] */ __RPC__deref_out_opt BSTR *pbstrEnlistmentPathUNC) = 0;
        
        virtual HRESULT STDMETHODCALLTYPE TranslateEnlistmentPathToProjectPath( 
            /* [in] */ __RPC__in LPCOLESTR lpszEnlistmentPath,
            /* [retval][out] */ __RPC__deref_out_opt BSTR *pbstrProjectPath) = 0;
        
    };
    
    
#else 	/* C style interface */

    typedef struct IVsSccEnlistmentPathTranslationVtbl
    {
        BEGIN_INTERFACE
        
        HRESULT ( STDMETHODCALLTYPE *QueryInterface )( 
            __RPC__in IVsSccEnlistmentPathTranslation * This,
            /* [in] */ __RPC__in REFIID riid,
            /* [annotation][iid_is][out] */ 
            _COM_Outptr_  void **ppvObject);
        
        ULONG ( STDMETHODCALLTYPE *AddRef )( 
            __RPC__in IVsSccEnlistmentPathTranslation * This);
        
        ULONG ( STDMETHODCALLTYPE *Release )( 
            __RPC__in IVsSccEnlistmentPathTranslation * This);
        
        HRESULT ( STDMETHODCALLTYPE *TranslateProjectPathToEnlistmentPath )( 
            __RPC__in IVsSccEnlistmentPathTranslation * This,
            /* [in] */ __RPC__in LPCOLESTR lpszProjectPath,
            /* [out] */ __RPC__deref_out_opt BSTR *pbstrEnlistmentPath,
            /* [out] */ __RPC__deref_out_opt BSTR *pbstrEnlistmentPathUNC);
        
        HRESULT ( STDMETHODCALLTYPE *TranslateEnlistmentPathToProjectPath )( 
            __RPC__in IVsSccEnlistmentPathTranslation * This,
            /* [in] */ __RPC__in LPCOLESTR lpszEnlistmentPath,
            /* [retval][out] */ __RPC__deref_out_opt BSTR *pbstrProjectPath);
        
        END_INTERFACE
    } IVsSccEnlistmentPathTranslationVtbl;

    interface IVsSccEnlistmentPathTranslation
    {
        CONST_VTBL struct IVsSccEnlistmentPathTranslationVtbl *lpVtbl;
    };

    

#ifdef COBJMACROS


#define IVsSccEnlistmentPathTranslation_QueryInterface(This,riid,ppvObject)	\
    ( (This)->lpVtbl -> QueryInterface(This,riid,ppvObject) ) 

#define IVsSccEnlistmentPathTranslation_AddRef(This)	\
    ( (This)->lpVtbl -> AddRef(This) ) 

#define IVsSccEnlistmentPathTranslation_Release(This)	\
    ( (This)->lpVtbl -> Release(This) ) 


#define IVsSccEnlistmentPathTranslation_TranslateProjectPathToEnlistmentPath(This,lpszProjectPath,pbstrEnlistmentPath,pbstrEnlistmentPathUNC)	\
    ( (This)->lpVtbl -> TranslateProjectPathToEnlistmentPath(This,lpszProjectPath,pbstrEnlistmentPath,pbstrEnlistmentPathUNC) ) 

#define IVsSccEnlistmentPathTranslation_TranslateEnlistmentPathToProjectPath(This,lpszEnlistmentPath,pbstrProjectPath)	\
    ( (This)->lpVtbl -> TranslateEnlistmentPathToProjectPath(This,lpszEnlistmentPath,pbstrProjectPath) ) 

#endif /* COBJMACROS */


#endif 	/* C style interface */




#endif 	/* __IVsSccEnlistmentPathTranslation_INTERFACE_DEFINED__ */


/* Additional Prototypes for ALL interfaces */

unsigned long             __RPC_USER  BSTR_UserSize(     __RPC__in unsigned long *, unsigned long            , __RPC__in BSTR * ); 
unsigned char * __RPC_USER  BSTR_UserMarshal(  __RPC__in unsigned long *, __RPC__inout_xcount(0) unsigned char *, __RPC__in BSTR * ); 
unsigned char * __RPC_USER  BSTR_UserUnmarshal(__RPC__in unsigned long *, __RPC__in_xcount(0) unsigned char *, __RPC__out BSTR * ); 
void                      __RPC_USER  BSTR_UserFree(     __RPC__in unsigned long *, __RPC__in BSTR * ); 

/* end of Additional Prototypes */

#ifdef __cplusplus
}
#endif

#endif


