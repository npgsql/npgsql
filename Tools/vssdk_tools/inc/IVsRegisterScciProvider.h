

/* this ALWAYS GENERATED file contains the definitions for the interfaces */


 /* File created by MIDL compiler version 7.00.0499 */
/* Compiler settings for IVsRegisterScciProvider.idl:
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

#ifndef __IVsRegisterScciProvider_h__
#define __IVsRegisterScciProvider_h__

#if defined(_MSC_VER) && (_MSC_VER >= 1020)
#pragma once
#endif

/* Forward Declarations */ 

#ifndef __IVsRegisterScciProvider_FWD_DEFINED__
#define __IVsRegisterScciProvider_FWD_DEFINED__
typedef interface IVsRegisterScciProvider IVsRegisterScciProvider;
#endif 	/* __IVsRegisterScciProvider_FWD_DEFINED__ */


#ifndef __ScciRegisterProvider_FWD_DEFINED__
#define __ScciRegisterProvider_FWD_DEFINED__

#ifdef __cplusplus
typedef class ScciRegisterProvider ScciRegisterProvider;
#else
typedef struct ScciRegisterProvider ScciRegisterProvider;
#endif /* __cplusplus */

#endif 	/* __ScciRegisterProvider_FWD_DEFINED__ */


/* header files for imported files */
#include "oaidl.h"
#include "ocidl.h"

#ifdef __cplusplus
extern "C"{
#endif 


#ifndef __IVsRegisterScciProvider_INTERFACE_DEFINED__
#define __IVsRegisterScciProvider_INTERFACE_DEFINED__

/* interface IVsRegisterScciProvider */
/* [unique][helpstring][uuid][object] */ 


EXTERN_C const IID IID_IVsRegisterScciProvider;

#if defined(__cplusplus) && !defined(CINTERFACE)
    
    MIDL_INTERFACE("53544C4D-C1F3-4fa8-BEA7-EA1A8FECFDD9")
    IVsRegisterScciProvider : public IUnknown
    {
    public:
        virtual HRESULT STDMETHODCALLTYPE RegisterSourceControlProvider( 
            /* [in] */ GUID guidProviderService) = 0;
        
    };
    
#else 	/* C style interface */

    typedef struct IVsRegisterScciProviderVtbl
    {
        BEGIN_INTERFACE
        
        HRESULT ( STDMETHODCALLTYPE *QueryInterface )( 
            IVsRegisterScciProvider * This,
            /* [in] */ __RPC__in REFIID riid,
            /* [iid_is][out] */ 
            __RPC__deref_out  void **ppvObject);
        
        ULONG ( STDMETHODCALLTYPE *AddRef )( 
            IVsRegisterScciProvider * This);
        
        ULONG ( STDMETHODCALLTYPE *Release )( 
            IVsRegisterScciProvider * This);
        
        HRESULT ( STDMETHODCALLTYPE *RegisterSourceControlProvider )( 
            IVsRegisterScciProvider * This,
            /* [in] */ GUID guidProviderService);
        
        END_INTERFACE
    } IVsRegisterScciProviderVtbl;

    interface IVsRegisterScciProvider
    {
        CONST_VTBL struct IVsRegisterScciProviderVtbl *lpVtbl;
    };

    

#ifdef COBJMACROS


#define IVsRegisterScciProvider_QueryInterface(This,riid,ppvObject)	\
    ( (This)->lpVtbl -> QueryInterface(This,riid,ppvObject) ) 

#define IVsRegisterScciProvider_AddRef(This)	\
    ( (This)->lpVtbl -> AddRef(This) ) 

#define IVsRegisterScciProvider_Release(This)	\
    ( (This)->lpVtbl -> Release(This) ) 


#define IVsRegisterScciProvider_RegisterSourceControlProvider(This,guidProviderService)	\
    ( (This)->lpVtbl -> RegisterSourceControlProvider(This,guidProviderService) ) 

#endif /* COBJMACROS */


#endif 	/* C style interface */




#endif 	/* __IVsRegisterScciProvider_INTERFACE_DEFINED__ */



#ifndef __TdScciRegisterProvider_LIBRARY_DEFINED__
#define __TdScciRegisterProvider_LIBRARY_DEFINED__

/* library TdScciRegisterProvider */
/* [helpstring][version][uuid] */ 

#define SID_SVsRegisterScciProvider IID_IVsRegisterScciProvider
extern const __declspec(selectany) GUID GUID_SourceControlProviderNone = { 0x53544C4D, 0x0486, 0x4938, {0xa9, 0x02, 0x8e, 0x99, 0xf3, 0xa2, 0xb4, 0x62} };

EXTERN_C const IID LIBID_TdScciRegisterProvider;

EXTERN_C const CLSID CLSID_ScciRegisterProvider;

#ifdef __cplusplus

class DECLSPEC_UUID("53544C4D-C1F3-4fa8-BEA7-EA1A8FECFDDB")
ScciRegisterProvider;
#endif
#endif /* __TdScciRegisterProvider_LIBRARY_DEFINED__ */

/* Additional Prototypes for ALL interfaces */

/* end of Additional Prototypes */

#ifdef __cplusplus
}
#endif

#endif


