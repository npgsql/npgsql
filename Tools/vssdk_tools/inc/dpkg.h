

/* this ALWAYS GENERATED file contains the definitions for the interfaces */


 /* File created by MIDL compiler version 7.00.0499 */
/* Compiler settings for dpkg.idl:
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

#ifndef __dpkg_h__
#define __dpkg_h__

#if defined(_MSC_VER) && (_MSC_VER >= 1020)
#pragma once
#endif

/* Forward Declarations */ 

#ifndef __IVsSolutionDebuggingAssistant2_FWD_DEFINED__
#define __IVsSolutionDebuggingAssistant2_FWD_DEFINED__
typedef interface IVsSolutionDebuggingAssistant2 IVsSolutionDebuggingAssistant2;
#endif 	/* __IVsSolutionDebuggingAssistant2_FWD_DEFINED__ */


#ifndef __IVsProvideDebuggingInfo_FWD_DEFINED__
#define __IVsProvideDebuggingInfo_FWD_DEFINED__
typedef interface IVsProvideDebuggingInfo IVsProvideDebuggingInfo;
#endif 	/* __IVsProvideDebuggingInfo_FWD_DEFINED__ */


/* header files for imported files */
#include "oaidl.h"
#include "ocidl.h"
#include "vsshell.h"

#ifdef __cplusplus
extern "C"{
#endif 


/* interface __MIDL_itf_dpkg_0000_0000 */
/* [local] */ 

#define VS_GUID_DEPLOYMENT_PACKAGE L"{9BBF7AD1-9153-465C-88FD-6AA23494C136}"
#define SID_SVsSolutionDebuggingAssistant2 IID_IVsSolutionDebuggingAssistant2


extern RPC_IF_HANDLE __MIDL_itf_dpkg_0000_0000_v0_0_c_ifspec;
extern RPC_IF_HANDLE __MIDL_itf_dpkg_0000_0000_v0_0_s_ifspec;

#ifndef __IVsSolutionDebuggingAssistant2_INTERFACE_DEFINED__
#define __IVsSolutionDebuggingAssistant2_INTERFACE_DEFINED__

/* interface IVsSolutionDebuggingAssistant2 */
/* [object][unique][version][uuid] */ 


EXTERN_C const IID IID_IVsSolutionDebuggingAssistant2;

#if defined(__cplusplus) && !defined(CINTERFACE)
    
    MIDL_INTERFACE("0A2EE8D5-53EF-410F-8504-3E0AFDAC5995")
    IVsSolutionDebuggingAssistant2 : public IUnknown
    {
    public:
        virtual HRESULT STDMETHODCALLTYPE MapOutputToDeployedURLs( 
            /* [in] */ DWORD dwReserved,
            /* [in] */ __RPC__in_opt IVsProjectCfg *pProjectCfg,
            /* [in] */ __RPC__in LPCOLESTR pszOutputCanonicalName,
            /* [in] */ ULONG celt,
            /* [size_is][out][in] */ __RPC__inout_ecount_full(celt) BSTR rgbstrMachines[  ],
            /* [size_is][out][in] */ __RPC__inout_ecount_full(celt) BSTR rgbstrURLs[  ],
            /* [out] */ __RPC__out ULONG *pcActual) = 0;
        
        virtual HRESULT STDMETHODCALLTYPE MapDeployedURLToProjectItem( 
            /* [in] */ DWORD dwReserved,
            /* [in] */ __RPC__in LPCOLESTR pszDUrl,
            /* [out] */ __RPC__deref_out_opt IVsHierarchy **pphier,
            /* [out] */ __RPC__out VSITEMID *pitemid) = 0;
        
    };
    
#else 	/* C style interface */

    typedef struct IVsSolutionDebuggingAssistant2Vtbl
    {
        BEGIN_INTERFACE
        
        HRESULT ( STDMETHODCALLTYPE *QueryInterface )( 
            IVsSolutionDebuggingAssistant2 * This,
            /* [in] */ __RPC__in REFIID riid,
            /* [iid_is][out] */ 
            __RPC__deref_out  void **ppvObject);
        
        ULONG ( STDMETHODCALLTYPE *AddRef )( 
            IVsSolutionDebuggingAssistant2 * This);
        
        ULONG ( STDMETHODCALLTYPE *Release )( 
            IVsSolutionDebuggingAssistant2 * This);
        
        HRESULT ( STDMETHODCALLTYPE *MapOutputToDeployedURLs )( 
            IVsSolutionDebuggingAssistant2 * This,
            /* [in] */ DWORD dwReserved,
            /* [in] */ __RPC__in_opt IVsProjectCfg *pProjectCfg,
            /* [in] */ __RPC__in LPCOLESTR pszOutputCanonicalName,
            /* [in] */ ULONG celt,
            /* [size_is][out][in] */ __RPC__inout_ecount_full(celt) BSTR rgbstrMachines[  ],
            /* [size_is][out][in] */ __RPC__inout_ecount_full(celt) BSTR rgbstrURLs[  ],
            /* [out] */ __RPC__out ULONG *pcActual);
        
        HRESULT ( STDMETHODCALLTYPE *MapDeployedURLToProjectItem )( 
            IVsSolutionDebuggingAssistant2 * This,
            /* [in] */ DWORD dwReserved,
            /* [in] */ __RPC__in LPCOLESTR pszDUrl,
            /* [out] */ __RPC__deref_out_opt IVsHierarchy **pphier,
            /* [out] */ __RPC__out VSITEMID *pitemid);
        
        END_INTERFACE
    } IVsSolutionDebuggingAssistant2Vtbl;

    interface IVsSolutionDebuggingAssistant2
    {
        CONST_VTBL struct IVsSolutionDebuggingAssistant2Vtbl *lpVtbl;
    };

    

#ifdef COBJMACROS


#define IVsSolutionDebuggingAssistant2_QueryInterface(This,riid,ppvObject)	\
    ( (This)->lpVtbl -> QueryInterface(This,riid,ppvObject) ) 

#define IVsSolutionDebuggingAssistant2_AddRef(This)	\
    ( (This)->lpVtbl -> AddRef(This) ) 

#define IVsSolutionDebuggingAssistant2_Release(This)	\
    ( (This)->lpVtbl -> Release(This) ) 


#define IVsSolutionDebuggingAssistant2_MapOutputToDeployedURLs(This,dwReserved,pProjectCfg,pszOutputCanonicalName,celt,rgbstrMachines,rgbstrURLs,pcActual)	\
    ( (This)->lpVtbl -> MapOutputToDeployedURLs(This,dwReserved,pProjectCfg,pszOutputCanonicalName,celt,rgbstrMachines,rgbstrURLs,pcActual) ) 

#define IVsSolutionDebuggingAssistant2_MapDeployedURLToProjectItem(This,dwReserved,pszDUrl,pphier,pitemid)	\
    ( (This)->lpVtbl -> MapDeployedURLToProjectItem(This,dwReserved,pszDUrl,pphier,pitemid) ) 

#endif /* COBJMACROS */


#endif 	/* C style interface */




#endif 	/* __IVsSolutionDebuggingAssistant2_INTERFACE_DEFINED__ */


#ifndef __IVsProvideDebuggingInfo_INTERFACE_DEFINED__
#define __IVsProvideDebuggingInfo_INTERFACE_DEFINED__

/* interface IVsProvideDebuggingInfo */
/* [object][unique][version][uuid] */ 


EXTERN_C const IID IID_IVsProvideDebuggingInfo;

#if defined(__cplusplus) && !defined(CINTERFACE)
    
    MIDL_INTERFACE("665AADC9-DFF5-4DDF-A119-60D2EEA051C6")
    IVsProvideDebuggingInfo : public IUnknown
    {
    public:
        virtual HRESULT STDMETHODCALLTYPE GetDeployedOutputs( 
            /* [in] */ ULONG celt,
            /* [size_is][out][in] */ __RPC__inout_ecount_full(celt) IVsHierarchy *pIVsHierarchy[  ],
            /* [size_is][out][in] */ __RPC__inout_ecount_full(celt) IVsProjectCfg *pIVsProjectCfg[  ],
            /* [size_is][out][in] */ __RPC__inout_ecount_full(celt) IVsOutput *pIVsOutput[  ],
            /* [size_is][out][in] */ __RPC__inout_ecount_full(celt) BSTR rgbstrMachines[  ],
            /* [size_is][out][in] */ __RPC__inout_ecount_full(celt) BSTR rgbstrURLs[  ],
            /* [out] */ __RPC__out ULONG *pcActual) = 0;
        
    };
    
#else 	/* C style interface */

    typedef struct IVsProvideDebuggingInfoVtbl
    {
        BEGIN_INTERFACE
        
        HRESULT ( STDMETHODCALLTYPE *QueryInterface )( 
            IVsProvideDebuggingInfo * This,
            /* [in] */ __RPC__in REFIID riid,
            /* [iid_is][out] */ 
            __RPC__deref_out  void **ppvObject);
        
        ULONG ( STDMETHODCALLTYPE *AddRef )( 
            IVsProvideDebuggingInfo * This);
        
        ULONG ( STDMETHODCALLTYPE *Release )( 
            IVsProvideDebuggingInfo * This);
        
        HRESULT ( STDMETHODCALLTYPE *GetDeployedOutputs )( 
            IVsProvideDebuggingInfo * This,
            /* [in] */ ULONG celt,
            /* [size_is][out][in] */ __RPC__inout_ecount_full(celt) IVsHierarchy *pIVsHierarchy[  ],
            /* [size_is][out][in] */ __RPC__inout_ecount_full(celt) IVsProjectCfg *pIVsProjectCfg[  ],
            /* [size_is][out][in] */ __RPC__inout_ecount_full(celt) IVsOutput *pIVsOutput[  ],
            /* [size_is][out][in] */ __RPC__inout_ecount_full(celt) BSTR rgbstrMachines[  ],
            /* [size_is][out][in] */ __RPC__inout_ecount_full(celt) BSTR rgbstrURLs[  ],
            /* [out] */ __RPC__out ULONG *pcActual);
        
        END_INTERFACE
    } IVsProvideDebuggingInfoVtbl;

    interface IVsProvideDebuggingInfo
    {
        CONST_VTBL struct IVsProvideDebuggingInfoVtbl *lpVtbl;
    };

    

#ifdef COBJMACROS


#define IVsProvideDebuggingInfo_QueryInterface(This,riid,ppvObject)	\
    ( (This)->lpVtbl -> QueryInterface(This,riid,ppvObject) ) 

#define IVsProvideDebuggingInfo_AddRef(This)	\
    ( (This)->lpVtbl -> AddRef(This) ) 

#define IVsProvideDebuggingInfo_Release(This)	\
    ( (This)->lpVtbl -> Release(This) ) 


#define IVsProvideDebuggingInfo_GetDeployedOutputs(This,celt,pIVsHierarchy,pIVsProjectCfg,pIVsOutput,rgbstrMachines,rgbstrURLs,pcActual)	\
    ( (This)->lpVtbl -> GetDeployedOutputs(This,celt,pIVsHierarchy,pIVsProjectCfg,pIVsOutput,rgbstrMachines,rgbstrURLs,pcActual) ) 

#endif /* COBJMACROS */


#endif 	/* C style interface */




#endif 	/* __IVsProvideDebuggingInfo_INTERFACE_DEFINED__ */


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


