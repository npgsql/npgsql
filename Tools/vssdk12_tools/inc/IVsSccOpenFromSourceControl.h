

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

#ifndef __IVsSccOpenFromSourceControl_h__
#define __IVsSccOpenFromSourceControl_h__

#if defined(_MSC_VER) && (_MSC_VER >= 1020)
#pragma once
#endif

/* Forward Declarations */ 

#ifndef __IVsSccOpenFromSourceControl_FWD_DEFINED__
#define __IVsSccOpenFromSourceControl_FWD_DEFINED__
typedef interface IVsSccOpenFromSourceControl IVsSccOpenFromSourceControl;

#endif 	/* __IVsSccOpenFromSourceControl_FWD_DEFINED__ */


/* header files for imported files */
#include "oaidl.h"
#include "vsshell.h"

#ifdef __cplusplus
extern "C"{
#endif 


/* interface __MIDL_itf_IVsSccOpenFromSourceControl_0000_0000 */
/* [local] */ 

#pragma once

enum __VSOPENFROMSCCDLG
    {
        VSOFSD_OPENSOLUTIONORPROJECT	= 1,
        VSOFSD_ADDEXISTINGITEM	= 2
    } ;
typedef LONG VSOPENFROMSCCDLG;



extern RPC_IF_HANDLE __MIDL_itf_IVsSccOpenFromSourceControl_0000_0000_v0_0_c_ifspec;
extern RPC_IF_HANDLE __MIDL_itf_IVsSccOpenFromSourceControl_0000_0000_v0_0_s_ifspec;

#ifndef __IVsSccOpenFromSourceControl_INTERFACE_DEFINED__
#define __IVsSccOpenFromSourceControl_INTERFACE_DEFINED__

/* interface IVsSccOpenFromSourceControl */
/* [object][uuid] */ 


EXTERN_C const IID IID_IVsSccOpenFromSourceControl;

#if defined(__cplusplus) && !defined(CINTERFACE)
    
    MIDL_INTERFACE("A341396A-1B4A-4164-8E6E-BDDC527C861C")
    IVsSccOpenFromSourceControl : public IUnknown
    {
    public:
        virtual HRESULT STDMETHODCALLTYPE OpenSolutionFromSourceControl( 
            /* [in] */ __RPC__in LPCOLESTR pszSolutionStoreUrl) = 0;
        
        virtual HRESULT STDMETHODCALLTYPE AddProjectFromSourceControl( 
            /* [in] */ __RPC__in LPCOLESTR pszProjectStoreUrl) = 0;
        
        virtual HRESULT STDMETHODCALLTYPE AddItemFromSourceControl( 
            /* [in] */ __RPC__in_opt IVsProject *pProject,
            /* [in] */ VSITEMID itemidLoc,
            /* [in] */ ULONG cFilesToAdd,
            /* [size_is][in] */ __RPC__in_ecount_full(cFilesToAdd) LPCOLESTR rgpszFilesToAdd[  ],
            /* [in] */ __RPC__in HWND hwndDlgOwner,
            /* [in] */ VSSPECIFICEDITORFLAGS grfEditorFlags,
            /* [in] */ __RPC__in REFGUID rguidEditorType,
            /* [in] */ __RPC__in LPCOLESTR pszPhysicalView,
            /* [in] */ __RPC__in REFGUID rguidLogicalView,
            /* [retval][out] */ __RPC__out VSADDRESULT *pResult) = 0;
        
        virtual HRESULT STDMETHODCALLTYPE GetNamespaceExtensionInformation( 
            /* [in] */ VSOPENFROMSCCDLG vsofsdDlg,
            /* [out] */ __RPC__deref_out_opt BSTR *pbstrNamespaceGUID,
            /* [out] */ __RPC__deref_out_opt BSTR *pbstrTrayDisplayName,
            /* [out] */ __RPC__deref_out_opt BSTR *pbstrProtocolPrefix) = 0;
        
    };
    
    
#else 	/* C style interface */

    typedef struct IVsSccOpenFromSourceControlVtbl
    {
        BEGIN_INTERFACE
        
        HRESULT ( STDMETHODCALLTYPE *QueryInterface )( 
            __RPC__in IVsSccOpenFromSourceControl * This,
            /* [in] */ __RPC__in REFIID riid,
            /* [annotation][iid_is][out] */ 
            _COM_Outptr_  void **ppvObject);
        
        ULONG ( STDMETHODCALLTYPE *AddRef )( 
            __RPC__in IVsSccOpenFromSourceControl * This);
        
        ULONG ( STDMETHODCALLTYPE *Release )( 
            __RPC__in IVsSccOpenFromSourceControl * This);
        
        HRESULT ( STDMETHODCALLTYPE *OpenSolutionFromSourceControl )( 
            __RPC__in IVsSccOpenFromSourceControl * This,
            /* [in] */ __RPC__in LPCOLESTR pszSolutionStoreUrl);
        
        HRESULT ( STDMETHODCALLTYPE *AddProjectFromSourceControl )( 
            __RPC__in IVsSccOpenFromSourceControl * This,
            /* [in] */ __RPC__in LPCOLESTR pszProjectStoreUrl);
        
        HRESULT ( STDMETHODCALLTYPE *AddItemFromSourceControl )( 
            __RPC__in IVsSccOpenFromSourceControl * This,
            /* [in] */ __RPC__in_opt IVsProject *pProject,
            /* [in] */ VSITEMID itemidLoc,
            /* [in] */ ULONG cFilesToAdd,
            /* [size_is][in] */ __RPC__in_ecount_full(cFilesToAdd) LPCOLESTR rgpszFilesToAdd[  ],
            /* [in] */ __RPC__in HWND hwndDlgOwner,
            /* [in] */ VSSPECIFICEDITORFLAGS grfEditorFlags,
            /* [in] */ __RPC__in REFGUID rguidEditorType,
            /* [in] */ __RPC__in LPCOLESTR pszPhysicalView,
            /* [in] */ __RPC__in REFGUID rguidLogicalView,
            /* [retval][out] */ __RPC__out VSADDRESULT *pResult);
        
        HRESULT ( STDMETHODCALLTYPE *GetNamespaceExtensionInformation )( 
            __RPC__in IVsSccOpenFromSourceControl * This,
            /* [in] */ VSOPENFROMSCCDLG vsofsdDlg,
            /* [out] */ __RPC__deref_out_opt BSTR *pbstrNamespaceGUID,
            /* [out] */ __RPC__deref_out_opt BSTR *pbstrTrayDisplayName,
            /* [out] */ __RPC__deref_out_opt BSTR *pbstrProtocolPrefix);
        
        END_INTERFACE
    } IVsSccOpenFromSourceControlVtbl;

    interface IVsSccOpenFromSourceControl
    {
        CONST_VTBL struct IVsSccOpenFromSourceControlVtbl *lpVtbl;
    };

    

#ifdef COBJMACROS


#define IVsSccOpenFromSourceControl_QueryInterface(This,riid,ppvObject)	\
    ( (This)->lpVtbl -> QueryInterface(This,riid,ppvObject) ) 

#define IVsSccOpenFromSourceControl_AddRef(This)	\
    ( (This)->lpVtbl -> AddRef(This) ) 

#define IVsSccOpenFromSourceControl_Release(This)	\
    ( (This)->lpVtbl -> Release(This) ) 


#define IVsSccOpenFromSourceControl_OpenSolutionFromSourceControl(This,pszSolutionStoreUrl)	\
    ( (This)->lpVtbl -> OpenSolutionFromSourceControl(This,pszSolutionStoreUrl) ) 

#define IVsSccOpenFromSourceControl_AddProjectFromSourceControl(This,pszProjectStoreUrl)	\
    ( (This)->lpVtbl -> AddProjectFromSourceControl(This,pszProjectStoreUrl) ) 

#define IVsSccOpenFromSourceControl_AddItemFromSourceControl(This,pProject,itemidLoc,cFilesToAdd,rgpszFilesToAdd,hwndDlgOwner,grfEditorFlags,rguidEditorType,pszPhysicalView,rguidLogicalView,pResult)	\
    ( (This)->lpVtbl -> AddItemFromSourceControl(This,pProject,itemidLoc,cFilesToAdd,rgpszFilesToAdd,hwndDlgOwner,grfEditorFlags,rguidEditorType,pszPhysicalView,rguidLogicalView,pResult) ) 

#define IVsSccOpenFromSourceControl_GetNamespaceExtensionInformation(This,vsofsdDlg,pbstrNamespaceGUID,pbstrTrayDisplayName,pbstrProtocolPrefix)	\
    ( (This)->lpVtbl -> GetNamespaceExtensionInformation(This,vsofsdDlg,pbstrNamespaceGUID,pbstrTrayDisplayName,pbstrProtocolPrefix) ) 

#endif /* COBJMACROS */


#endif 	/* C style interface */




#endif 	/* __IVsSccOpenFromSourceControl_INTERFACE_DEFINED__ */


/* Additional Prototypes for ALL interfaces */

unsigned long             __RPC_USER  BSTR_UserSize(     __RPC__in unsigned long *, unsigned long            , __RPC__in BSTR * ); 
unsigned char * __RPC_USER  BSTR_UserMarshal(  __RPC__in unsigned long *, __RPC__inout_xcount(0) unsigned char *, __RPC__in BSTR * ); 
unsigned char * __RPC_USER  BSTR_UserUnmarshal(__RPC__in unsigned long *, __RPC__in_xcount(0) unsigned char *, __RPC__out BSTR * ); 
void                      __RPC_USER  BSTR_UserFree(     __RPC__in unsigned long *, __RPC__in BSTR * ); 

unsigned long             __RPC_USER  HWND_UserSize(     __RPC__in unsigned long *, unsigned long            , __RPC__in HWND * ); 
unsigned char * __RPC_USER  HWND_UserMarshal(  __RPC__in unsigned long *, __RPC__inout_xcount(0) unsigned char *, __RPC__in HWND * ); 
unsigned char * __RPC_USER  HWND_UserUnmarshal(__RPC__in unsigned long *, __RPC__in_xcount(0) unsigned char *, __RPC__out HWND * ); 
void                      __RPC_USER  HWND_UserFree(     __RPC__in unsigned long *, __RPC__in HWND * ); 

/* end of Additional Prototypes */

#ifdef __cplusplus
}
#endif

#endif


