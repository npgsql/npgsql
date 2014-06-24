

/* this ALWAYS GENERATED file contains the definitions for the interfaces */


 /* File created by MIDL compiler version 7.00.0499 */
/* Compiler settings for IVsSccControlNewSolution.idl:
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

#ifndef __IVsSccControlNewSolution_h__
#define __IVsSccControlNewSolution_h__

#if defined(_MSC_VER) && (_MSC_VER >= 1020)
#pragma once
#endif

/* Forward Declarations */ 

#ifndef __IVsSccControlNewSolution_FWD_DEFINED__
#define __IVsSccControlNewSolution_FWD_DEFINED__
typedef interface IVsSccControlNewSolution IVsSccControlNewSolution;
#endif 	/* __IVsSccControlNewSolution_FWD_DEFINED__ */


/* header files for imported files */
#include "oaidl.h"

#ifdef __cplusplus
extern "C"{
#endif 


/* interface __MIDL_itf_IVsSccControlNewSolution_0000_0000 */
/* [local] */ 

#pragma once


extern RPC_IF_HANDLE __MIDL_itf_IVsSccControlNewSolution_0000_0000_v0_0_c_ifspec;
extern RPC_IF_HANDLE __MIDL_itf_IVsSccControlNewSolution_0000_0000_v0_0_s_ifspec;

#ifndef __IVsSccControlNewSolution_INTERFACE_DEFINED__
#define __IVsSccControlNewSolution_INTERFACE_DEFINED__

/* interface IVsSccControlNewSolution */
/* [object][uuid] */ 


EXTERN_C const IID IID_IVsSccControlNewSolution;

#if defined(__cplusplus) && !defined(CINTERFACE)
    
    MIDL_INTERFACE("753D3585-2077-4e60-8EC5-96EE793F3D1A")
    IVsSccControlNewSolution : public IUnknown
    {
    public:
        virtual HRESULT STDMETHODCALLTYPE AddNewSolutionToSourceControl( void) = 0;
        
        virtual HRESULT STDMETHODCALLTYPE GetDisplayStringForAction( 
            /* [out] */ __RPC__deref_out_opt BSTR *pbstrActionName) = 0;
        
    };
    
#else 	/* C style interface */

    typedef struct IVsSccControlNewSolutionVtbl
    {
        BEGIN_INTERFACE
        
        HRESULT ( STDMETHODCALLTYPE *QueryInterface )( 
            IVsSccControlNewSolution * This,
            /* [in] */ __RPC__in REFIID riid,
            /* [iid_is][out] */ 
            __RPC__deref_out  void **ppvObject);
        
        ULONG ( STDMETHODCALLTYPE *AddRef )( 
            IVsSccControlNewSolution * This);
        
        ULONG ( STDMETHODCALLTYPE *Release )( 
            IVsSccControlNewSolution * This);
        
        HRESULT ( STDMETHODCALLTYPE *AddNewSolutionToSourceControl )( 
            IVsSccControlNewSolution * This);
        
        HRESULT ( STDMETHODCALLTYPE *GetDisplayStringForAction )( 
            IVsSccControlNewSolution * This,
            /* [out] */ __RPC__deref_out_opt BSTR *pbstrActionName);
        
        END_INTERFACE
    } IVsSccControlNewSolutionVtbl;

    interface IVsSccControlNewSolution
    {
        CONST_VTBL struct IVsSccControlNewSolutionVtbl *lpVtbl;
    };

    

#ifdef COBJMACROS


#define IVsSccControlNewSolution_QueryInterface(This,riid,ppvObject)	\
    ( (This)->lpVtbl -> QueryInterface(This,riid,ppvObject) ) 

#define IVsSccControlNewSolution_AddRef(This)	\
    ( (This)->lpVtbl -> AddRef(This) ) 

#define IVsSccControlNewSolution_Release(This)	\
    ( (This)->lpVtbl -> Release(This) ) 


#define IVsSccControlNewSolution_AddNewSolutionToSourceControl(This)	\
    ( (This)->lpVtbl -> AddNewSolutionToSourceControl(This) ) 

#define IVsSccControlNewSolution_GetDisplayStringForAction(This,pbstrActionName)	\
    ( (This)->lpVtbl -> GetDisplayStringForAction(This,pbstrActionName) ) 

#endif /* COBJMACROS */


#endif 	/* C style interface */




#endif 	/* __IVsSccControlNewSolution_INTERFACE_DEFINED__ */


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


