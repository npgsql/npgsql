

/* this ALWAYS GENERATED file contains the definitions for the interfaces */


 /* File created by MIDL compiler version 6.00.0361 */
/* Compiler settings for ivssccenlistmentpathtranslation.idl:
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

#include "rpc.h"
#include "rpcndr.h"

#ifndef __RPCNDR_H_VERSION__
#error this stub requires an updated version of <rpcndr.h>
#endif // __RPCNDR_H_VERSION__

#ifndef COM_NO_WINDOWS_H
#include "windows.h"
#include "ole2.h"
#endif /*COM_NO_WINDOWS_H*/

#ifndef __ivssccenlistmentpathtranslation_h__
#define __ivssccenlistmentpathtranslation_h__

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

void * __RPC_USER MIDL_user_allocate(size_t);
void __RPC_USER MIDL_user_free( void * ); 

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
            /* [in] */ LPCOLESTR lpszProjectPath,
            /* [out] */ BSTR *pbstrEnlistmentPath,
            /* [out] */ BSTR *pbstrEnlistmentPathUNC) = 0;
        
        virtual HRESULT STDMETHODCALLTYPE TranslateEnlistmentPathToProjectPath( 
            /* [in] */ LPCOLESTR lpszEnlistmentPath,
            /* [retval][out] */ BSTR *pbstrProjectPath) = 0;
        
    };
    
#else 	/* C style interface */

    typedef struct IVsSccEnlistmentPathTranslationVtbl
    {
        BEGIN_INTERFACE
        
        HRESULT ( STDMETHODCALLTYPE *QueryInterface )( 
            IVsSccEnlistmentPathTranslation * This,
            /* [in] */ REFIID riid,
            /* [iid_is][out] */ void **ppvObject);
        
        ULONG ( STDMETHODCALLTYPE *AddRef )( 
            IVsSccEnlistmentPathTranslation * This);
        
        ULONG ( STDMETHODCALLTYPE *Release )( 
            IVsSccEnlistmentPathTranslation * This);
        
        HRESULT ( STDMETHODCALLTYPE *TranslateProjectPathToEnlistmentPath )( 
            IVsSccEnlistmentPathTranslation * This,
            /* [in] */ LPCOLESTR lpszProjectPath,
            /* [out] */ BSTR *pbstrEnlistmentPath,
            /* [out] */ BSTR *pbstrEnlistmentPathUNC);
        
        HRESULT ( STDMETHODCALLTYPE *TranslateEnlistmentPathToProjectPath )( 
            IVsSccEnlistmentPathTranslation * This,
            /* [in] */ LPCOLESTR lpszEnlistmentPath,
            /* [retval][out] */ BSTR *pbstrProjectPath);
        
        END_INTERFACE
    } IVsSccEnlistmentPathTranslationVtbl;

    interface IVsSccEnlistmentPathTranslation
    {
        CONST_VTBL struct IVsSccEnlistmentPathTranslationVtbl *lpVtbl;
    };

    

#ifdef COBJMACROS


#define IVsSccEnlistmentPathTranslation_QueryInterface(This,riid,ppvObject)	\
    (This)->lpVtbl -> QueryInterface(This,riid,ppvObject)

#define IVsSccEnlistmentPathTranslation_AddRef(This)	\
    (This)->lpVtbl -> AddRef(This)

#define IVsSccEnlistmentPathTranslation_Release(This)	\
    (This)->lpVtbl -> Release(This)


#define IVsSccEnlistmentPathTranslation_TranslateProjectPathToEnlistmentPath(This,lpszProjectPath,pbstrEnlistmentPath,pbstrEnlistmentPathUNC)	\
    (This)->lpVtbl -> TranslateProjectPathToEnlistmentPath(This,lpszProjectPath,pbstrEnlistmentPath,pbstrEnlistmentPathUNC)

#define IVsSccEnlistmentPathTranslation_TranslateEnlistmentPathToProjectPath(This,lpszEnlistmentPath,pbstrProjectPath)	\
    (This)->lpVtbl -> TranslateEnlistmentPathToProjectPath(This,lpszEnlistmentPath,pbstrProjectPath)

#endif /* COBJMACROS */


#endif 	/* C style interface */



HRESULT STDMETHODCALLTYPE IVsSccEnlistmentPathTranslation_TranslateProjectPathToEnlistmentPath_Proxy( 
    IVsSccEnlistmentPathTranslation * This,
    /* [in] */ LPCOLESTR lpszProjectPath,
    /* [out] */ BSTR *pbstrEnlistmentPath,
    /* [out] */ BSTR *pbstrEnlistmentPathUNC);


void __RPC_STUB IVsSccEnlistmentPathTranslation_TranslateProjectPathToEnlistmentPath_Stub(
    IRpcStubBuffer *This,
    IRpcChannelBuffer *_pRpcChannelBuffer,
    PRPC_MESSAGE _pRpcMessage,
    DWORD *_pdwStubPhase);


HRESULT STDMETHODCALLTYPE IVsSccEnlistmentPathTranslation_TranslateEnlistmentPathToProjectPath_Proxy( 
    IVsSccEnlistmentPathTranslation * This,
    /* [in] */ LPCOLESTR lpszEnlistmentPath,
    /* [retval][out] */ BSTR *pbstrProjectPath);


void __RPC_STUB IVsSccEnlistmentPathTranslation_TranslateEnlistmentPathToProjectPath_Stub(
    IRpcStubBuffer *This,
    IRpcChannelBuffer *_pRpcChannelBuffer,
    PRPC_MESSAGE _pRpcMessage,
    DWORD *_pdwStubPhase);



#endif 	/* __IVsSccEnlistmentPathTranslation_INTERFACE_DEFINED__ */


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


