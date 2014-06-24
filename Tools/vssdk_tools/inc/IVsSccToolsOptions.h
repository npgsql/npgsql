

/* this ALWAYS GENERATED file contains the definitions for the interfaces */


 /* File created by MIDL compiler version 7.00.0499 */
/* Compiler settings for IVsSCCToolsOptions.idl:
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

#ifndef __IVsSCCToolsOptions_h__
#define __IVsSCCToolsOptions_h__

#if defined(_MSC_VER) && (_MSC_VER >= 1020)
#pragma once
#endif

/* Forward Declarations */ 

#ifndef __IVsSccToolsOptions_FWD_DEFINED__
#define __IVsSccToolsOptions_FWD_DEFINED__
typedef interface IVsSccToolsOptions IVsSccToolsOptions;
#endif 	/* __IVsSccToolsOptions_FWD_DEFINED__ */


#ifndef __SVsSccToolsOptions_FWD_DEFINED__
#define __SVsSccToolsOptions_FWD_DEFINED__
typedef interface SVsSccToolsOptions SVsSccToolsOptions;
#endif 	/* __SVsSccToolsOptions_FWD_DEFINED__ */


/* header files for imported files */
#include "oaidl.h"

#ifdef __cplusplus
extern "C"{
#endif 


/* interface __MIDL_itf_IVsSCCToolsOptions_0000_0000 */
/* [local] */ 

#pragma once
#pragma once
typedef /* [helpstring] */ 
enum __SccToolsOptionsEnum
    {	ksctoAllowReadOnlyFilesNotUnderSccToBeEdited	= 1,
	ksctoLast	= ksctoAllowReadOnlyFilesNotUnderSccToBeEdited,
	ksctoBad	= ( ksctoLast + 1 ) 
    } 	SccToolsOptionsEnum;



extern RPC_IF_HANDLE __MIDL_itf_IVsSCCToolsOptions_0000_0000_v0_0_c_ifspec;
extern RPC_IF_HANDLE __MIDL_itf_IVsSCCToolsOptions_0000_0000_v0_0_s_ifspec;

#ifndef __IVsSccToolsOptions_INTERFACE_DEFINED__
#define __IVsSccToolsOptions_INTERFACE_DEFINED__

/* interface IVsSccToolsOptions */
/* [object][uuid] */ 


EXTERN_C const IID IID_IVsSccToolsOptions;

#if defined(__cplusplus) && !defined(CINTERFACE)
    
    MIDL_INTERFACE("53544C4D-304B-4D82-AD93-074816C1A0E5")
    IVsSccToolsOptions : public IUnknown
    {
    public:
        virtual HRESULT STDMETHODCALLTYPE SetSccToolsOption( 
            /* [in] */ SccToolsOptionsEnum sctoOptionToBeSet,
            /* [in] */ VARIANT varValueToBeSet) = 0;
        
        virtual HRESULT STDMETHODCALLTYPE GetSccToolsOption( 
            /* [in] */ SccToolsOptionsEnum sctoOptionToBeSet,
            /* [retval][out] */ __RPC__out VARIANT *pvarValueToGet) = 0;
        
    };
    
#else 	/* C style interface */

    typedef struct IVsSccToolsOptionsVtbl
    {
        BEGIN_INTERFACE
        
        HRESULT ( STDMETHODCALLTYPE *QueryInterface )( 
            IVsSccToolsOptions * This,
            /* [in] */ __RPC__in REFIID riid,
            /* [iid_is][out] */ 
            __RPC__deref_out  void **ppvObject);
        
        ULONG ( STDMETHODCALLTYPE *AddRef )( 
            IVsSccToolsOptions * This);
        
        ULONG ( STDMETHODCALLTYPE *Release )( 
            IVsSccToolsOptions * This);
        
        HRESULT ( STDMETHODCALLTYPE *SetSccToolsOption )( 
            IVsSccToolsOptions * This,
            /* [in] */ SccToolsOptionsEnum sctoOptionToBeSet,
            /* [in] */ VARIANT varValueToBeSet);
        
        HRESULT ( STDMETHODCALLTYPE *GetSccToolsOption )( 
            IVsSccToolsOptions * This,
            /* [in] */ SccToolsOptionsEnum sctoOptionToBeSet,
            /* [retval][out] */ __RPC__out VARIANT *pvarValueToGet);
        
        END_INTERFACE
    } IVsSccToolsOptionsVtbl;

    interface IVsSccToolsOptions
    {
        CONST_VTBL struct IVsSccToolsOptionsVtbl *lpVtbl;
    };

    

#ifdef COBJMACROS


#define IVsSccToolsOptions_QueryInterface(This,riid,ppvObject)	\
    ( (This)->lpVtbl -> QueryInterface(This,riid,ppvObject) ) 

#define IVsSccToolsOptions_AddRef(This)	\
    ( (This)->lpVtbl -> AddRef(This) ) 

#define IVsSccToolsOptions_Release(This)	\
    ( (This)->lpVtbl -> Release(This) ) 


#define IVsSccToolsOptions_SetSccToolsOption(This,sctoOptionToBeSet,varValueToBeSet)	\
    ( (This)->lpVtbl -> SetSccToolsOption(This,sctoOptionToBeSet,varValueToBeSet) ) 

#define IVsSccToolsOptions_GetSccToolsOption(This,sctoOptionToBeSet,pvarValueToGet)	\
    ( (This)->lpVtbl -> GetSccToolsOption(This,sctoOptionToBeSet,pvarValueToGet) ) 

#endif /* COBJMACROS */


#endif 	/* C style interface */




#endif 	/* __IVsSccToolsOptions_INTERFACE_DEFINED__ */


#ifndef __SVsSccToolsOptions_INTERFACE_DEFINED__
#define __SVsSccToolsOptions_INTERFACE_DEFINED__

/* interface SVsSccToolsOptions */
/* [object][uuid] */ 


EXTERN_C const IID IID_SVsSccToolsOptions;

#if defined(__cplusplus) && !defined(CINTERFACE)
    
    MIDL_INTERFACE("53544C4D-104B-4D82-AD93-074816C1A0E5")
    SVsSccToolsOptions : public IUnknown
    {
    public:
    };
    
#else 	/* C style interface */

    typedef struct SVsSccToolsOptionsVtbl
    {
        BEGIN_INTERFACE
        
        HRESULT ( STDMETHODCALLTYPE *QueryInterface )( 
            SVsSccToolsOptions * This,
            /* [in] */ __RPC__in REFIID riid,
            /* [iid_is][out] */ 
            __RPC__deref_out  void **ppvObject);
        
        ULONG ( STDMETHODCALLTYPE *AddRef )( 
            SVsSccToolsOptions * This);
        
        ULONG ( STDMETHODCALLTYPE *Release )( 
            SVsSccToolsOptions * This);
        
        END_INTERFACE
    } SVsSccToolsOptionsVtbl;

    interface SVsSccToolsOptions
    {
        CONST_VTBL struct SVsSccToolsOptionsVtbl *lpVtbl;
    };

    

#ifdef COBJMACROS


#define SVsSccToolsOptions_QueryInterface(This,riid,ppvObject)	\
    ( (This)->lpVtbl -> QueryInterface(This,riid,ppvObject) ) 

#define SVsSccToolsOptions_AddRef(This)	\
    ( (This)->lpVtbl -> AddRef(This) ) 

#define SVsSccToolsOptions_Release(This)	\
    ( (This)->lpVtbl -> Release(This) ) 


#endif /* COBJMACROS */


#endif 	/* C style interface */




#endif 	/* __SVsSccToolsOptions_INTERFACE_DEFINED__ */


/* interface __MIDL_itf_IVsSCCToolsOptions_0000_0002 */
/* [local] */ 

#define SID_SVsSccToolsOptions IID_SVsSccToolsOptions


extern RPC_IF_HANDLE __MIDL_itf_IVsSCCToolsOptions_0000_0002_v0_0_c_ifspec;
extern RPC_IF_HANDLE __MIDL_itf_IVsSCCToolsOptions_0000_0002_v0_0_s_ifspec;

/* Additional Prototypes for ALL interfaces */

unsigned long             __RPC_USER  VARIANT_UserSize(     unsigned long *, unsigned long            , VARIANT * ); 
unsigned char * __RPC_USER  VARIANT_UserMarshal(  unsigned long *, unsigned char *, VARIANT * ); 
unsigned char * __RPC_USER  VARIANT_UserUnmarshal(unsigned long *, unsigned char *, VARIANT * ); 
void                      __RPC_USER  VARIANT_UserFree(     unsigned long *, VARIANT * ); 

/* end of Additional Prototypes */

#ifdef __cplusplus
}
#endif

#endif


