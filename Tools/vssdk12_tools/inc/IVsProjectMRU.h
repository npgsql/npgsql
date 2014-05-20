

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

#ifndef __IVsProjectMRU_h__
#define __IVsProjectMRU_h__

#if defined(_MSC_VER) && (_MSC_VER >= 1020)
#pragma once
#endif

/* Forward Declarations */ 

#ifndef __SVsProjectMRU_FWD_DEFINED__
#define __SVsProjectMRU_FWD_DEFINED__
typedef interface SVsProjectMRU SVsProjectMRU;

#endif 	/* __SVsProjectMRU_FWD_DEFINED__ */


#ifndef __IVsProjectMRU_FWD_DEFINED__
#define __IVsProjectMRU_FWD_DEFINED__
typedef interface IVsProjectMRU IVsProjectMRU;

#endif 	/* __IVsProjectMRU_FWD_DEFINED__ */


/* header files for imported files */
#include "oaidl.h"

#ifdef __cplusplus
extern "C"{
#endif 


#ifndef __SVsProjectMRU_INTERFACE_DEFINED__
#define __SVsProjectMRU_INTERFACE_DEFINED__

/* interface SVsProjectMRU */
/* [object][unique][version][uuid] */ 


EXTERN_C const IID IID_SVsProjectMRU;

#if defined(__cplusplus) && !defined(CINTERFACE)
    
    MIDL_INTERFACE("D8982A22-9CE1-45ED-963C-815854B1CFA2")
    SVsProjectMRU : public IUnknown
    {
    public:
    };
    
    
#else 	/* C style interface */

    typedef struct SVsProjectMRUVtbl
    {
        BEGIN_INTERFACE
        
        HRESULT ( STDMETHODCALLTYPE *QueryInterface )( 
            __RPC__in SVsProjectMRU * This,
            /* [in] */ __RPC__in REFIID riid,
            /* [annotation][iid_is][out] */ 
            _COM_Outptr_  void **ppvObject);
        
        ULONG ( STDMETHODCALLTYPE *AddRef )( 
            __RPC__in SVsProjectMRU * This);
        
        ULONG ( STDMETHODCALLTYPE *Release )( 
            __RPC__in SVsProjectMRU * This);
        
        END_INTERFACE
    } SVsProjectMRUVtbl;

    interface SVsProjectMRU
    {
        CONST_VTBL struct SVsProjectMRUVtbl *lpVtbl;
    };

    

#ifdef COBJMACROS


#define SVsProjectMRU_QueryInterface(This,riid,ppvObject)	\
    ( (This)->lpVtbl -> QueryInterface(This,riid,ppvObject) ) 

#define SVsProjectMRU_AddRef(This)	\
    ( (This)->lpVtbl -> AddRef(This) ) 

#define SVsProjectMRU_Release(This)	\
    ( (This)->lpVtbl -> Release(This) ) 


#endif /* COBJMACROS */


#endif 	/* C style interface */




#endif 	/* __SVsProjectMRU_INTERFACE_DEFINED__ */


/* interface __MIDL_itf_IVsProjectMRU_0000_0001 */
/* [local] */ 

#define SID_SVsProjectMRU IID_SVsProjectMRU


extern RPC_IF_HANDLE __MIDL_itf_IVsProjectMRU_0000_0001_v0_0_c_ifspec;
extern RPC_IF_HANDLE __MIDL_itf_IVsProjectMRU_0000_0001_v0_0_s_ifspec;

#ifndef __IVsProjectMRU_INTERFACE_DEFINED__
#define __IVsProjectMRU_INTERFACE_DEFINED__

/* interface IVsProjectMRU */
/* [object][uuid] */ 


EXTERN_C const IID IID_IVsProjectMRU;

#if defined(__cplusplus) && !defined(CINTERFACE)
    
    MIDL_INTERFACE("D60EDEEA-3629-42CC-BAAF-9AC52E63EEAF")
    IVsProjectMRU : public IUnknown
    {
    public:
        virtual HRESULT STDMETHODCALLTYPE RegisterProjectMRU( 
            /* [in] */ __RPC__in LPCOLESTR szLocalPath,
            /* [in] */ __RPC__in LPCOLESTR szProviderString,
            /* [in] */ __RPC__in LPCOLESTR szDisplayText,
            /* [in] */ __RPC__in LPCOLESTR szToolTipDisplayText,
            /* [in] */ __RPC__in REFGUID providerId) = 0;
        
    };
    
    
#else 	/* C style interface */

    typedef struct IVsProjectMRUVtbl
    {
        BEGIN_INTERFACE
        
        HRESULT ( STDMETHODCALLTYPE *QueryInterface )( 
            __RPC__in IVsProjectMRU * This,
            /* [in] */ __RPC__in REFIID riid,
            /* [annotation][iid_is][out] */ 
            _COM_Outptr_  void **ppvObject);
        
        ULONG ( STDMETHODCALLTYPE *AddRef )( 
            __RPC__in IVsProjectMRU * This);
        
        ULONG ( STDMETHODCALLTYPE *Release )( 
            __RPC__in IVsProjectMRU * This);
        
        HRESULT ( STDMETHODCALLTYPE *RegisterProjectMRU )( 
            __RPC__in IVsProjectMRU * This,
            /* [in] */ __RPC__in LPCOLESTR szLocalPath,
            /* [in] */ __RPC__in LPCOLESTR szProviderString,
            /* [in] */ __RPC__in LPCOLESTR szDisplayText,
            /* [in] */ __RPC__in LPCOLESTR szToolTipDisplayText,
            /* [in] */ __RPC__in REFGUID providerId);
        
        END_INTERFACE
    } IVsProjectMRUVtbl;

    interface IVsProjectMRU
    {
        CONST_VTBL struct IVsProjectMRUVtbl *lpVtbl;
    };

    

#ifdef COBJMACROS


#define IVsProjectMRU_QueryInterface(This,riid,ppvObject)	\
    ( (This)->lpVtbl -> QueryInterface(This,riid,ppvObject) ) 

#define IVsProjectMRU_AddRef(This)	\
    ( (This)->lpVtbl -> AddRef(This) ) 

#define IVsProjectMRU_Release(This)	\
    ( (This)->lpVtbl -> Release(This) ) 


#define IVsProjectMRU_RegisterProjectMRU(This,szLocalPath,szProviderString,szDisplayText,szToolTipDisplayText,providerId)	\
    ( (This)->lpVtbl -> RegisterProjectMRU(This,szLocalPath,szProviderString,szDisplayText,szToolTipDisplayText,providerId) ) 

#endif /* COBJMACROS */


#endif 	/* C style interface */




#endif 	/* __IVsProjectMRU_INTERFACE_DEFINED__ */


/* Additional Prototypes for ALL interfaces */

/* end of Additional Prototypes */

#ifdef __cplusplus
}
#endif

#endif


