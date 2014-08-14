

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

#ifndef __IVsGetScciProviderInterface_h__
#define __IVsGetScciProviderInterface_h__

#if defined(_MSC_VER) && (_MSC_VER >= 1020)
#pragma once
#endif

/* Forward Declarations */ 

#ifndef __IVsGetScciProviderInterface_FWD_DEFINED__
#define __IVsGetScciProviderInterface_FWD_DEFINED__
typedef interface IVsGetScciProviderInterface IVsGetScciProviderInterface;

#endif 	/* __IVsGetScciProviderInterface_FWD_DEFINED__ */


/* header files for imported files */
#include "oaidl.h"

#ifdef __cplusplus
extern "C"{
#endif 


#ifndef __IVsGetScciProviderInterface_INTERFACE_DEFINED__
#define __IVsGetScciProviderInterface_INTERFACE_DEFINED__

/* interface IVsGetScciProviderInterface */
/* [unique][helpstring][uuid][object] */ 


EXTERN_C const IID IID_IVsGetScciProviderInterface;

#if defined(__cplusplus) && !defined(CINTERFACE)
    
    MIDL_INTERFACE("53544C4D-C1F3-0ADC-BEA7-EA1A8FECFDD9")
    IVsGetScciProviderInterface : public IUnknown
    {
    public:
        virtual HRESULT STDMETHODCALLTYPE GetSourceControlProviderInterface( 
            /* [in] */ __RPC__in REFIID riid,
            /* [iid_is][out] */ __RPC__deref_out_opt void **ppvObject) = 0;
        
        virtual HRESULT STDMETHODCALLTYPE GetSourceControlProviderID( 
            /* [out] */ __RPC__out GUID *pguidSCCProvider) = 0;
        
    };
    
    
#else 	/* C style interface */

    typedef struct IVsGetScciProviderInterfaceVtbl
    {
        BEGIN_INTERFACE
        
        HRESULT ( STDMETHODCALLTYPE *QueryInterface )( 
            __RPC__in IVsGetScciProviderInterface * This,
            /* [in] */ __RPC__in REFIID riid,
            /* [annotation][iid_is][out] */ 
            _COM_Outptr_  void **ppvObject);
        
        ULONG ( STDMETHODCALLTYPE *AddRef )( 
            __RPC__in IVsGetScciProviderInterface * This);
        
        ULONG ( STDMETHODCALLTYPE *Release )( 
            __RPC__in IVsGetScciProviderInterface * This);
        
        HRESULT ( STDMETHODCALLTYPE *GetSourceControlProviderInterface )( 
            __RPC__in IVsGetScciProviderInterface * This,
            /* [in] */ __RPC__in REFIID riid,
            /* [iid_is][out] */ __RPC__deref_out_opt void **ppvObject);
        
        HRESULT ( STDMETHODCALLTYPE *GetSourceControlProviderID )( 
            __RPC__in IVsGetScciProviderInterface * This,
            /* [out] */ __RPC__out GUID *pguidSCCProvider);
        
        END_INTERFACE
    } IVsGetScciProviderInterfaceVtbl;

    interface IVsGetScciProviderInterface
    {
        CONST_VTBL struct IVsGetScciProviderInterfaceVtbl *lpVtbl;
    };

    

#ifdef COBJMACROS


#define IVsGetScciProviderInterface_QueryInterface(This,riid,ppvObject)	\
    ( (This)->lpVtbl -> QueryInterface(This,riid,ppvObject) ) 

#define IVsGetScciProviderInterface_AddRef(This)	\
    ( (This)->lpVtbl -> AddRef(This) ) 

#define IVsGetScciProviderInterface_Release(This)	\
    ( (This)->lpVtbl -> Release(This) ) 


#define IVsGetScciProviderInterface_GetSourceControlProviderInterface(This,riid,ppvObject)	\
    ( (This)->lpVtbl -> GetSourceControlProviderInterface(This,riid,ppvObject) ) 

#define IVsGetScciProviderInterface_GetSourceControlProviderID(This,pguidSCCProvider)	\
    ( (This)->lpVtbl -> GetSourceControlProviderID(This,pguidSCCProvider) ) 

#endif /* COBJMACROS */


#endif 	/* C style interface */




#endif 	/* __IVsGetScciProviderInterface_INTERFACE_DEFINED__ */


/* Additional Prototypes for ALL interfaces */

/* end of Additional Prototypes */

#ifdef __cplusplus
}
#endif

#endif


