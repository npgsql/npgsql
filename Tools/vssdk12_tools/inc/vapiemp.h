

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

#ifndef __vapiemp_h__
#define __vapiemp_h__

#if defined(_MSC_VER) && (_MSC_VER >= 1020)
#pragma once
#endif

/* Forward Declarations */ 

#ifndef __ISetVsWeb_FWD_DEFINED__
#define __ISetVsWeb_FWD_DEFINED__
typedef interface ISetVsWeb ISetVsWeb;

#endif 	/* __ISetVsWeb_FWD_DEFINED__ */


#ifndef __CVapiEMPDataSource_FWD_DEFINED__
#define __CVapiEMPDataSource_FWD_DEFINED__

#ifdef __cplusplus
typedef class CVapiEMPDataSource CVapiEMPDataSource;
#else
typedef struct CVapiEMPDataSource CVapiEMPDataSource;
#endif /* __cplusplus */

#endif 	/* __CVapiEMPDataSource_FWD_DEFINED__ */


/* header files for imported files */
#include "objidl.h"

#ifdef __cplusplus
extern "C"{
#endif 


/* interface __MIDL_itf_vapiemp_0000_0000 */
/* [local] */ 

#pragma once


extern RPC_IF_HANDLE __MIDL_itf_vapiemp_0000_0000_v0_0_c_ifspec;
extern RPC_IF_HANDLE __MIDL_itf_vapiemp_0000_0000_v0_0_s_ifspec;

#ifndef __ISetVsWeb_INTERFACE_DEFINED__
#define __ISetVsWeb_INTERFACE_DEFINED__

/* interface ISetVsWeb */
/* [unique][uuid][object] */ 


EXTERN_C const IID IID_ISetVsWeb;

#if defined(__cplusplus) && !defined(CINTERFACE)
    
    MIDL_INTERFACE("6B33DBD9-1DDD-11d3-85CF-00A0C9CFCC16")
    ISetVsWeb : public IUnknown
    {
    public:
        virtual HRESULT STDMETHODCALLTYPE SetVsWeb( 
            /* [in] */ __RPC__in_opt IUnknown *pIUnknown) = 0;
        
    };
    
    
#else 	/* C style interface */

    typedef struct ISetVsWebVtbl
    {
        BEGIN_INTERFACE
        
        HRESULT ( STDMETHODCALLTYPE *QueryInterface )( 
            __RPC__in ISetVsWeb * This,
            /* [in] */ __RPC__in REFIID riid,
            /* [annotation][iid_is][out] */ 
            _COM_Outptr_  void **ppvObject);
        
        ULONG ( STDMETHODCALLTYPE *AddRef )( 
            __RPC__in ISetVsWeb * This);
        
        ULONG ( STDMETHODCALLTYPE *Release )( 
            __RPC__in ISetVsWeb * This);
        
        HRESULT ( STDMETHODCALLTYPE *SetVsWeb )( 
            __RPC__in ISetVsWeb * This,
            /* [in] */ __RPC__in_opt IUnknown *pIUnknown);
        
        END_INTERFACE
    } ISetVsWebVtbl;

    interface ISetVsWeb
    {
        CONST_VTBL struct ISetVsWebVtbl *lpVtbl;
    };

    

#ifdef COBJMACROS


#define ISetVsWeb_QueryInterface(This,riid,ppvObject)	\
    ( (This)->lpVtbl -> QueryInterface(This,riid,ppvObject) ) 

#define ISetVsWeb_AddRef(This)	\
    ( (This)->lpVtbl -> AddRef(This) ) 

#define ISetVsWeb_Release(This)	\
    ( (This)->lpVtbl -> Release(This) ) 


#define ISetVsWeb_SetVsWeb(This,pIUnknown)	\
    ( (This)->lpVtbl -> SetVsWeb(This,pIUnknown) ) 

#endif /* COBJMACROS */


#endif 	/* C style interface */




#endif 	/* __ISetVsWeb_INTERFACE_DEFINED__ */



#ifndef __VseeVersioningEnlistmentManagerProxy_LIBRARY_DEFINED__
#define __VseeVersioningEnlistmentManagerProxy_LIBRARY_DEFINED__

/* library VseeVersioningEnlistmentManagerProxy */
/* [uuid] */ 


EXTERN_C const IID LIBID_VseeVersioningEnlistmentManagerProxy;

EXTERN_C const CLSID CLSID_CVapiEMPDataSource;

#ifdef __cplusplus

class DECLSPEC_UUID("6856262f-19d4-4090-a0cd-a486a148709c")
CVapiEMPDataSource;
#endif
#endif /* __VseeVersioningEnlistmentManagerProxy_LIBRARY_DEFINED__ */

/* Additional Prototypes for ALL interfaces */

/* end of Additional Prototypes */

#ifdef __cplusplus
}
#endif

#endif


