

/* this ALWAYS GENERATED file contains the definitions for the interfaces */


 /* File created by MIDL compiler version 7.00.0499 */
/* Compiler settings for vsshell2.idl:
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

#ifndef __vsshell2_h__
#define __vsshell2_h__

#if defined(_MSC_VER) && (_MSC_VER >= 1020)
#pragma once
#endif

/* Forward Declarations */ 

#ifndef __IVsProjectUpgrade_FWD_DEFINED__
#define __IVsProjectUpgrade_FWD_DEFINED__
typedef interface IVsProjectUpgrade IVsProjectUpgrade;
#endif 	/* __IVsProjectUpgrade_FWD_DEFINED__ */


/* header files for imported files */
#include "oaidl.h"
#include "vsshell.h"

#ifdef __cplusplus
extern "C"{
#endif 


/* interface __MIDL_itf_vsshell2_0000_0000 */
/* [local] */ 

#pragma once

enum __VSUPGRADEPROJFLAGS
    {	UPF_SILENTMIGRATE	= 0x1
    } ;
typedef DWORD VSUPGRADEPROJFLAGS;



extern RPC_IF_HANDLE __MIDL_itf_vsshell2_0000_0000_v0_0_c_ifspec;
extern RPC_IF_HANDLE __MIDL_itf_vsshell2_0000_0000_v0_0_s_ifspec;

#ifndef __IVsProjectUpgrade_INTERFACE_DEFINED__
#define __IVsProjectUpgrade_INTERFACE_DEFINED__

/* interface IVsProjectUpgrade */
/* [object][unique][version][uuid] */ 


EXTERN_C const IID IID_IVsProjectUpgrade;

#if defined(__cplusplus) && !defined(CINTERFACE)
    
    MIDL_INTERFACE("75661D39-F5DA-41b9-ABDA-9CF54C6B1AC9")
    IVsProjectUpgrade : public IUnknown
    {
    public:
        virtual HRESULT STDMETHODCALLTYPE UpgradeProject( 
            /* [in] */ VSUPGRADEPROJFLAGS grfUpgradeFlags) = 0;
        
    };
    
#else 	/* C style interface */

    typedef struct IVsProjectUpgradeVtbl
    {
        BEGIN_INTERFACE
        
        HRESULT ( STDMETHODCALLTYPE *QueryInterface )( 
            IVsProjectUpgrade * This,
            /* [in] */ __RPC__in REFIID riid,
            /* [iid_is][out] */ 
            __RPC__deref_out  void **ppvObject);
        
        ULONG ( STDMETHODCALLTYPE *AddRef )( 
            IVsProjectUpgrade * This);
        
        ULONG ( STDMETHODCALLTYPE *Release )( 
            IVsProjectUpgrade * This);
        
        HRESULT ( STDMETHODCALLTYPE *UpgradeProject )( 
            IVsProjectUpgrade * This,
            /* [in] */ VSUPGRADEPROJFLAGS grfUpgradeFlags);
        
        END_INTERFACE
    } IVsProjectUpgradeVtbl;

    interface IVsProjectUpgrade
    {
        CONST_VTBL struct IVsProjectUpgradeVtbl *lpVtbl;
    };

    

#ifdef COBJMACROS


#define IVsProjectUpgrade_QueryInterface(This,riid,ppvObject)	\
    ( (This)->lpVtbl -> QueryInterface(This,riid,ppvObject) ) 

#define IVsProjectUpgrade_AddRef(This)	\
    ( (This)->lpVtbl -> AddRef(This) ) 

#define IVsProjectUpgrade_Release(This)	\
    ( (This)->lpVtbl -> Release(This) ) 


#define IVsProjectUpgrade_UpgradeProject(This,grfUpgradeFlags)	\
    ( (This)->lpVtbl -> UpgradeProject(This,grfUpgradeFlags) ) 

#endif /* COBJMACROS */


#endif 	/* C style interface */




#endif 	/* __IVsProjectUpgrade_INTERFACE_DEFINED__ */


/* Additional Prototypes for ALL interfaces */

/* end of Additional Prototypes */

#ifdef __cplusplus
}
#endif

#endif


