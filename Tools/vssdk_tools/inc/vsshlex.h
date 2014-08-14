

/* this ALWAYS GENERATED file contains the definitions for the interfaces */


 /* File created by MIDL compiler version 7.00.0499 */
/* Compiler settings for vsshlex.idl:
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


#ifndef __vsshlex_h__
#define __vsshlex_h__

#if defined(_MSC_VER) && (_MSC_VER >= 1020)
#pragma once
#endif

/* Forward Declarations */ 

#ifndef __IVsPackageStaticExData_FWD_DEFINED__
#define __IVsPackageStaticExData_FWD_DEFINED__
typedef interface IVsPackageStaticExData IVsPackageStaticExData;
#endif 	/* __IVsPackageStaticExData_FWD_DEFINED__ */


/* header files for imported files */
#include "oaidl.h"
#include "ocidl.h"

#ifdef __cplusplus
extern "C"{
#endif 


/* interface __MIDL_itf_vsshlex_0000_0000 */
/* [local] */ 

DEFINE_GUID(GUID_ExDataEE, 0x64ac2454, 0xbd18, 0x11d1, 0x87, 0xb5, 0x0, 0xa0, 0xc9, 0x1e, 0x2a, 0x46);


extern RPC_IF_HANDLE __MIDL_itf_vsshlex_0000_0000_v0_0_c_ifspec;
extern RPC_IF_HANDLE __MIDL_itf_vsshlex_0000_0000_v0_0_s_ifspec;


#ifndef __VsShellExLib_LIBRARY_DEFINED__
#define __VsShellExLib_LIBRARY_DEFINED__

/* library VsShellExLib */
/* [helpstring][version][uuid] */ 


EXTERN_C const IID LIBID_VsShellExLib;

#ifndef __IVsPackageStaticExData_INTERFACE_DEFINED__
#define __IVsPackageStaticExData_INTERFACE_DEFINED__

/* interface IVsPackageStaticExData */
/* [object][local][unique][helpstring][uuid] */ 


EXTERN_C const IID IID_IVsPackageStaticExData;

#if defined(__cplusplus) && !defined(CINTERFACE)
    
    MIDL_INTERFACE("64AC2451-BD18-11d1-87B5-00A0C91E2A46")
    IVsPackageStaticExData : public IUnknown
    {
    public:
        virtual /* [local] */ HRESULT STDMETHODCALLTYPE GetStaticExData( 
            /* [in] */ REFGUID rguidData,
            /* [out] */ ULONG *pcbData,
            /* [out] */ unsigned short **ppData,
            /* [out] */ ULONG *pcMaxRecords) = 0;
        
    };
    
#else 	/* C style interface */

    typedef struct IVsPackageStaticExDataVtbl
    {
        BEGIN_INTERFACE
        
        HRESULT ( STDMETHODCALLTYPE *QueryInterface )( 
            IVsPackageStaticExData * This,
            /* [in] */ REFIID riid,
            /* [iid_is][out] */ 
            __RPC__deref_out  void **ppvObject);
        
        ULONG ( STDMETHODCALLTYPE *AddRef )( 
            IVsPackageStaticExData * This);
        
        ULONG ( STDMETHODCALLTYPE *Release )( 
            IVsPackageStaticExData * This);
        
        /* [local] */ HRESULT ( STDMETHODCALLTYPE *GetStaticExData )( 
            IVsPackageStaticExData * This,
            /* [in] */ REFGUID rguidData,
            /* [out] */ ULONG *pcbData,
            /* [out] */ unsigned short **ppData,
            /* [out] */ ULONG *pcMaxRecords);
        
        END_INTERFACE
    } IVsPackageStaticExDataVtbl;

    interface IVsPackageStaticExData
    {
        CONST_VTBL struct IVsPackageStaticExDataVtbl *lpVtbl;
    };

    

#ifdef COBJMACROS


#define IVsPackageStaticExData_QueryInterface(This,riid,ppvObject)	\
    ( (This)->lpVtbl -> QueryInterface(This,riid,ppvObject) ) 

#define IVsPackageStaticExData_AddRef(This)	\
    ( (This)->lpVtbl -> AddRef(This) ) 

#define IVsPackageStaticExData_Release(This)	\
    ( (This)->lpVtbl -> Release(This) ) 


#define IVsPackageStaticExData_GetStaticExData(This,rguidData,pcbData,ppData,pcMaxRecords)	\
    ( (This)->lpVtbl -> GetStaticExData(This,rguidData,pcbData,ppData,pcMaxRecords) ) 

#endif /* COBJMACROS */


#endif 	/* C style interface */




#endif 	/* __IVsPackageStaticExData_INTERFACE_DEFINED__ */

#endif /* __VsShellExLib_LIBRARY_DEFINED__ */

/* Additional Prototypes for ALL interfaces */

/* end of Additional Prototypes */

#ifdef __cplusplus
}
#endif

#endif


