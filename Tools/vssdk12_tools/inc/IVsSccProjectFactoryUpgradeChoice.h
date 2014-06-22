

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

#ifndef __IVsSccProjectFactoryUpgradeChoice_h__
#define __IVsSccProjectFactoryUpgradeChoice_h__

#if defined(_MSC_VER) && (_MSC_VER >= 1020)
#pragma once
#endif

/* Forward Declarations */ 

#ifndef __IVsSccProjectFactoryUpgradeChoice_FWD_DEFINED__
#define __IVsSccProjectFactoryUpgradeChoice_FWD_DEFINED__
typedef interface IVsSccProjectFactoryUpgradeChoice IVsSccProjectFactoryUpgradeChoice;

#endif 	/* __IVsSccProjectFactoryUpgradeChoice_FWD_DEFINED__ */


/* header files for imported files */
#include "objidl.h"
#include "IVsSccProjectEnlistmentChoice.h"

#ifdef __cplusplus
extern "C"{
#endif 


/* interface __MIDL_itf_IVsSccProjectFactoryUpgradeChoice_0000_0000 */
/* [local] */ 

#pragma once
typedef 
enum __VSSCCPROJECTSTYLE
    {
        VSSCC_PS_USESPROJECTFILE	= 0,
        VSSCC_PS_FOLDERBASED	= 1
    } 	VSSCCPROJECTSTYLE;



extern RPC_IF_HANDLE __MIDL_itf_IVsSccProjectFactoryUpgradeChoice_0000_0000_v0_0_c_ifspec;
extern RPC_IF_HANDLE __MIDL_itf_IVsSccProjectFactoryUpgradeChoice_0000_0000_v0_0_s_ifspec;

#ifndef __IVsSccProjectFactoryUpgradeChoice_INTERFACE_DEFINED__
#define __IVsSccProjectFactoryUpgradeChoice_INTERFACE_DEFINED__

/* interface IVsSccProjectFactoryUpgradeChoice */
/* [object][unique][version][uuid] */ 


EXTERN_C const IID IID_IVsSccProjectFactoryUpgradeChoice;

#if defined(__cplusplus) && !defined(CINTERFACE)
    
    MIDL_INTERFACE("53544c4d-E3A4-4938-A5EC-6593A79CF27C")
    IVsSccProjectFactoryUpgradeChoice : public IUnknown
    {
    public:
        virtual HRESULT STDMETHODCALLTYPE GetSccUpgradeChoice( 
            /* [in] */ __RPC__in LPOLESTR pszProjectFileName,
            /* [out] */ __RPC__out VSSCCPROJECTSTYLE *projectStyle,
            /* [out] */ __RPC__out VSSCCENLISTMENTCHOICE *enlistmentChoice) = 0;
        
    };
    
    
#else 	/* C style interface */

    typedef struct IVsSccProjectFactoryUpgradeChoiceVtbl
    {
        BEGIN_INTERFACE
        
        HRESULT ( STDMETHODCALLTYPE *QueryInterface )( 
            __RPC__in IVsSccProjectFactoryUpgradeChoice * This,
            /* [in] */ __RPC__in REFIID riid,
            /* [annotation][iid_is][out] */ 
            _COM_Outptr_  void **ppvObject);
        
        ULONG ( STDMETHODCALLTYPE *AddRef )( 
            __RPC__in IVsSccProjectFactoryUpgradeChoice * This);
        
        ULONG ( STDMETHODCALLTYPE *Release )( 
            __RPC__in IVsSccProjectFactoryUpgradeChoice * This);
        
        HRESULT ( STDMETHODCALLTYPE *GetSccUpgradeChoice )( 
            __RPC__in IVsSccProjectFactoryUpgradeChoice * This,
            /* [in] */ __RPC__in LPOLESTR pszProjectFileName,
            /* [out] */ __RPC__out VSSCCPROJECTSTYLE *projectStyle,
            /* [out] */ __RPC__out VSSCCENLISTMENTCHOICE *enlistmentChoice);
        
        END_INTERFACE
    } IVsSccProjectFactoryUpgradeChoiceVtbl;

    interface IVsSccProjectFactoryUpgradeChoice
    {
        CONST_VTBL struct IVsSccProjectFactoryUpgradeChoiceVtbl *lpVtbl;
    };

    

#ifdef COBJMACROS


#define IVsSccProjectFactoryUpgradeChoice_QueryInterface(This,riid,ppvObject)	\
    ( (This)->lpVtbl -> QueryInterface(This,riid,ppvObject) ) 

#define IVsSccProjectFactoryUpgradeChoice_AddRef(This)	\
    ( (This)->lpVtbl -> AddRef(This) ) 

#define IVsSccProjectFactoryUpgradeChoice_Release(This)	\
    ( (This)->lpVtbl -> Release(This) ) 


#define IVsSccProjectFactoryUpgradeChoice_GetSccUpgradeChoice(This,pszProjectFileName,projectStyle,enlistmentChoice)	\
    ( (This)->lpVtbl -> GetSccUpgradeChoice(This,pszProjectFileName,projectStyle,enlistmentChoice) ) 

#endif /* COBJMACROS */


#endif 	/* C style interface */




#endif 	/* __IVsSccProjectFactoryUpgradeChoice_INTERFACE_DEFINED__ */


/* Additional Prototypes for ALL interfaces */

/* end of Additional Prototypes */

#ifdef __cplusplus
}
#endif

#endif


