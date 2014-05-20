

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

#ifndef __vssccmgr_h__
#define __vssccmgr_h__

#if defined(_MSC_VER) && (_MSC_VER >= 1020)
#pragma once
#endif

/* Forward Declarations */ 

#ifndef __IVsSccManager_FWD_DEFINED__
#define __IVsSccManager_FWD_DEFINED__
typedef interface IVsSccManager IVsSccManager;

#endif 	/* __IVsSccManager_FWD_DEFINED__ */


#ifndef __IEnableOpenFromScc_FWD_DEFINED__
#define __IEnableOpenFromScc_FWD_DEFINED__
typedef interface IEnableOpenFromScc IEnableOpenFromScc;

#endif 	/* __IEnableOpenFromScc_FWD_DEFINED__ */


/* header files for imported files */
#include "objidl.h"
#include "IVsSccManager2.h"
#include "vsscceng.h"
#include "vssccprj.h"

#ifdef __cplusplus
extern "C"{
#endif 


/* interface __MIDL_itf_vssccmgr_0000_0000 */
/* [local] */ 

#if     _MSC_VER > 1000
#pragma once
#endif
#pragma once

enum tagVSSCCOPS
    {
        VsSccNop	= 0,
        VsSccSyncOp	= ( VsSccNop + 1 ) ,
        VsSccOutOp	= ( VsSccSyncOp + 1 ) ,
        VsSccInOp	= ( VsSccOutOp + 1 ) ,
        VsSccUnOutOp	= ( VsSccInOp + 1 ) ,
        VsSccAddOp	= ( VsSccUnOutOp + 1 ) ,
        VsSccRemoveOp	= ( VsSccAddOp + 1 ) ,
        VsSccDiffOp	= ( VsSccRemoveOp + 1 ) ,
        VsSccHistoryOp	= ( VsSccDiffOp + 1 ) ,
        VsSccRenameOp	= ( VsSccHistoryOp + 1 ) ,
        VsSccPropsOp	= ( VsSccRenameOp + 1 ) ,
        VsSccOptionsOp	= ( VsSccPropsOp + 1 ) ,
        VsSccShareOp	= ( VsSccOptionsOp + 1 ) ,
        VsSccAdminOp	= ( VsSccShareOp + 1 ) ,
        VsSccStatusOp	= ( VsSccAdminOp + 1 ) ,
        VsSccAddOptionsOp	= ( VsSccStatusOp + 1 ) ,
        VsSccInitOp	= ( VsSccAddOptionsOp + 1 ) ,
        VsSccTermOp	= ( VsSccInitOp + 1 ) ,
        VsSccPopOp	= ( VsSccTermOp + 1 ) ,
        VsSccEnumChangedOp	= ( VsSccPopOp + 1 ) ,
        VsSccPopDirOp	= ( VsSccEnumChangedOp + 1 ) ,
        VsSccQueryChangesOp	= ( VsSccPopDirOp + 1 ) ,
        VsSccRemoveDirOp	= ( VsSccQueryChangesOp + 1 ) ,
        VsSccShelveOp	= ( VsSccRemoveDirOp + 1 ) 
    } ;
typedef DWORD VSSCCOPS;


enum tagVSSCCISVALIDOPFLAGS
    {
        VSSCC_VALIDALL	= 0,
        VSSCC_INVALIDANY	= 0,
        VSSCC_VALIDANY	= 1,
        VSSCC_INVALIDALL	= 1
    } ;
typedef DWORD VSSCCISVALIDOPFLAGS;


enum tagVSSCC_MULTIFILEBOOLLOGIC
    {
        VSSCC_RETURNALL	= 1,
        VSSCC_STOPONFIRSTTRUE	= 2,
        VSSCC_STOPONFIRSTFALSE	= 4
    } ;
typedef DWORD VSSCC_MULTIFILEBOOLLOGIC;


enum tagVSSCC_CHECKOUTFORSAVERESULT
    {
        VSSCC_CHECKOUTFORSAVE_OK	= 1,
        VSSCC_CHECKOUTFORSAVE_DONTSAVE_CANCEL	= 2,
        VSSCC_CHECKOUTFORSAVE_DONTSAVE_CONTINUE	= 3,
        VSSCC_CHECKOUTFORSAVE_SAVEAS	= 4
    } ;
typedef DWORD VSSCC_CHECKOUTFORSAVERESULT;



extern RPC_IF_HANDLE __MIDL_itf_vssccmgr_0000_0000_v0_0_c_ifspec;
extern RPC_IF_HANDLE __MIDL_itf_vssccmgr_0000_0000_v0_0_s_ifspec;

#ifndef __IVsSccManager_INTERFACE_DEFINED__
#define __IVsSccManager_INTERFACE_DEFINED__

/* interface IVsSccManager */
/* [object][uuid] */ 


EXTERN_C const IID IID_IVsSccManager;

#if defined(__cplusplus) && !defined(CINTERFACE)
    
    MIDL_INTERFACE("53544C4D-0F05-4735-8AAC-264109CF68AC")
    IVsSccManager : public IUnknown
    {
    public:
        virtual HRESULT STDMETHODCALLTYPE RegisterSccProject( 
            /* [in] */ __RPC__in_opt IVsSccProject *pProject,
            /* [in] */ __RPC__in LPCOLESTR pszSccProjectName,
            /* [in] */ __RPC__in LPCOLESTR pszSccAuxPath,
            /* [in] */ __RPC__in LPCOLESTR pszSccLocalPath,
            /* [in] */ __RPC__in LPCOLESTR pszProvider,
            /* [in] */ __RPC__in_opt IVsSccEngine *piSccEngine,
            /* [out] */ __RPC__out DWORD *pdwCookie) = 0;
        
        virtual HRESULT STDMETHODCALLTYPE UnregisterSccProject( 
            /* [in] */ DWORD dwCookie) = 0;
        
        virtual HRESULT STDMETHODCALLTYPE IsValidOp( 
            /* [in] */ DWORD dwCookie,
            /* [in] */ int cFiles,
            /* [size_is][in] */ __RPC__in_ecount_full(cFiles) const LPCOLESTR rgpszFullpaths[  ],
            /* [in] */ VSSCCOPS sccop,
            /* [in] */ VSSCCISVALIDOPFLAGS dwFlags,
            /* [out] */ __RPC__out BOOL *pfValid) = 0;
        
        virtual HRESULT STDMETHODCALLTYPE FileStatus( 
            /* [in] */ DWORD dwCookie,
            /* [in] */ int cFiles,
            /* [size_is][in] */ __RPC__in_ecount_full(cFiles) const LPCOLESTR rgpszFullpaths[  ],
            /* [out] */ __RPC__out VSSCCSTATUS *pStatus,
            /* [out] */ __RPC__out CADWORD *prgStatus) = 0;
        
        virtual HRESULT STDMETHODCALLTYPE ProjectStatus( 
            /* [in] */ __RPC__in_opt IVsProject *pProject,
            /* [out] */ __RPC__out BOOL *pbStatus) = 0;
        
        virtual HRESULT STDMETHODCALLTYPE PollFileChangeCause( 
            /* [in] */ DWORD dwCookie,
            /* [in] */ __RPC__in LPCOLESTR pszFilename,
            /* [out] */ __RPC__out BOOL *pbBusy) = 0;
        
        virtual HRESULT STDMETHODCALLTYPE CheckoutForEdit( 
            /* [in] */ DWORD dwCookie,
            /* [in] */ int cFiles,
            /* [size_is][in] */ __RPC__in_ecount_full(cFiles) const LPCOLESTR rgpszMkDocuments[  ],
            /* [in] */ BOOL bReloadOK,
            /* [out] */ __RPC__out DWORD *pdw) = 0;
        
        virtual HRESULT STDMETHODCALLTYPE IsInstalled( 
            /* [defaultvalue][in] */ __RPC__in LPCOLESTR pszProviderName,
            /* [retval][out] */ __RPC__out BOOL *pbInstalled) = 0;
        
        virtual HRESULT STDMETHODCALLTYPE GetVsSccEngine( 
            /* [in] */ __RPC__in LPCOLESTR pszProviderName,
            /* [in] */ __RPC__in LPCOLESTR pszFullPathToSccDll,
            /* [out] */ __RPC__deref_out_opt IVsSccEngine **ppEngine) = 0;
        
        virtual HRESULT STDMETHODCALLTYPE BrowseForProject( 
            /* [out] */ __RPC__deref_out_opt BSTR *pbstrDirectory,
            /* [out] */ __RPC__out BOOL *pfOK) = 0;
        
        virtual HRESULT STDMETHODCALLTYPE CancelAfterBrowseForProject( void) = 0;
        
        virtual HRESULT STDMETHODCALLTYPE CheckoutMightChange( 
            /* [in] */ DWORD dwCookie,
            /* [in] */ VSSCC_MULTIFILEBOOLLOGIC multiLogic,
            /* [in] */ int cFiles,
            /* [size_is][in] */ __RPC__in_ecount_full(cFiles) const LPCOLESTR rgpszFullpaths[  ],
            /* [out] */ __RPC__out BOOL *pbSingleResult,
            /* [out] */ __RPC__out CADWORD *prgbMultiResult) = 0;
        
        virtual HRESULT STDMETHODCALLTYPE Checkout( 
            /* [in] */ DWORD dwCookie,
            /* [in] */ DWORD dwReservedFlags,
            /* [in] */ int cFiles,
            /* [size_is][in] */ __RPC__in_ecount_full(cFiles) const LPCOLESTR rgpszMkDocuments[  ],
            /* [out] */ __RPC__out DWORD *pdwReserved) = 0;
        
        virtual HRESULT STDMETHODCALLTYPE CheckoutForSave( 
            /* [in] */ DWORD dwCookie,
            /* [in] */ DWORD dwReservedFlags,
            /* [in] */ int cFiles,
            /* [size_is][in] */ __RPC__in_ecount_full(cFiles) const LPCOLESTR rgpszMkDocuments[  ],
            /* [out] */ __RPC__out VSSCC_CHECKOUTFORSAVERESULT *pdw) = 0;
        
        virtual HRESULT STDMETHODCALLTYPE OnAfterDisconnectProject( 
            /* [in] */ __RPC__in_opt IVsSccProject *pProject,
            /* [in] */ DWORD dwCookie) = 0;
        
        virtual HRESULT STDMETHODCALLTYPE IsTargetOfPendingRename( 
            /* [in] */ __RPC__in LPCOLESTR targetFile,
            /* [out] */ __RPC__out BOOL *pbResult) = 0;
        
    };
    
    
#else 	/* C style interface */

    typedef struct IVsSccManagerVtbl
    {
        BEGIN_INTERFACE
        
        HRESULT ( STDMETHODCALLTYPE *QueryInterface )( 
            __RPC__in IVsSccManager * This,
            /* [in] */ __RPC__in REFIID riid,
            /* [annotation][iid_is][out] */ 
            _COM_Outptr_  void **ppvObject);
        
        ULONG ( STDMETHODCALLTYPE *AddRef )( 
            __RPC__in IVsSccManager * This);
        
        ULONG ( STDMETHODCALLTYPE *Release )( 
            __RPC__in IVsSccManager * This);
        
        HRESULT ( STDMETHODCALLTYPE *RegisterSccProject )( 
            __RPC__in IVsSccManager * This,
            /* [in] */ __RPC__in_opt IVsSccProject *pProject,
            /* [in] */ __RPC__in LPCOLESTR pszSccProjectName,
            /* [in] */ __RPC__in LPCOLESTR pszSccAuxPath,
            /* [in] */ __RPC__in LPCOLESTR pszSccLocalPath,
            /* [in] */ __RPC__in LPCOLESTR pszProvider,
            /* [in] */ __RPC__in_opt IVsSccEngine *piSccEngine,
            /* [out] */ __RPC__out DWORD *pdwCookie);
        
        HRESULT ( STDMETHODCALLTYPE *UnregisterSccProject )( 
            __RPC__in IVsSccManager * This,
            /* [in] */ DWORD dwCookie);
        
        HRESULT ( STDMETHODCALLTYPE *IsValidOp )( 
            __RPC__in IVsSccManager * This,
            /* [in] */ DWORD dwCookie,
            /* [in] */ int cFiles,
            /* [size_is][in] */ __RPC__in_ecount_full(cFiles) const LPCOLESTR rgpszFullpaths[  ],
            /* [in] */ VSSCCOPS sccop,
            /* [in] */ VSSCCISVALIDOPFLAGS dwFlags,
            /* [out] */ __RPC__out BOOL *pfValid);
        
        HRESULT ( STDMETHODCALLTYPE *FileStatus )( 
            __RPC__in IVsSccManager * This,
            /* [in] */ DWORD dwCookie,
            /* [in] */ int cFiles,
            /* [size_is][in] */ __RPC__in_ecount_full(cFiles) const LPCOLESTR rgpszFullpaths[  ],
            /* [out] */ __RPC__out VSSCCSTATUS *pStatus,
            /* [out] */ __RPC__out CADWORD *prgStatus);
        
        HRESULT ( STDMETHODCALLTYPE *ProjectStatus )( 
            __RPC__in IVsSccManager * This,
            /* [in] */ __RPC__in_opt IVsProject *pProject,
            /* [out] */ __RPC__out BOOL *pbStatus);
        
        HRESULT ( STDMETHODCALLTYPE *PollFileChangeCause )( 
            __RPC__in IVsSccManager * This,
            /* [in] */ DWORD dwCookie,
            /* [in] */ __RPC__in LPCOLESTR pszFilename,
            /* [out] */ __RPC__out BOOL *pbBusy);
        
        HRESULT ( STDMETHODCALLTYPE *CheckoutForEdit )( 
            __RPC__in IVsSccManager * This,
            /* [in] */ DWORD dwCookie,
            /* [in] */ int cFiles,
            /* [size_is][in] */ __RPC__in_ecount_full(cFiles) const LPCOLESTR rgpszMkDocuments[  ],
            /* [in] */ BOOL bReloadOK,
            /* [out] */ __RPC__out DWORD *pdw);
        
        HRESULT ( STDMETHODCALLTYPE *IsInstalled )( 
            __RPC__in IVsSccManager * This,
            /* [defaultvalue][in] */ __RPC__in LPCOLESTR pszProviderName,
            /* [retval][out] */ __RPC__out BOOL *pbInstalled);
        
        HRESULT ( STDMETHODCALLTYPE *GetVsSccEngine )( 
            __RPC__in IVsSccManager * This,
            /* [in] */ __RPC__in LPCOLESTR pszProviderName,
            /* [in] */ __RPC__in LPCOLESTR pszFullPathToSccDll,
            /* [out] */ __RPC__deref_out_opt IVsSccEngine **ppEngine);
        
        HRESULT ( STDMETHODCALLTYPE *BrowseForProject )( 
            __RPC__in IVsSccManager * This,
            /* [out] */ __RPC__deref_out_opt BSTR *pbstrDirectory,
            /* [out] */ __RPC__out BOOL *pfOK);
        
        HRESULT ( STDMETHODCALLTYPE *CancelAfterBrowseForProject )( 
            __RPC__in IVsSccManager * This);
        
        HRESULT ( STDMETHODCALLTYPE *CheckoutMightChange )( 
            __RPC__in IVsSccManager * This,
            /* [in] */ DWORD dwCookie,
            /* [in] */ VSSCC_MULTIFILEBOOLLOGIC multiLogic,
            /* [in] */ int cFiles,
            /* [size_is][in] */ __RPC__in_ecount_full(cFiles) const LPCOLESTR rgpszFullpaths[  ],
            /* [out] */ __RPC__out BOOL *pbSingleResult,
            /* [out] */ __RPC__out CADWORD *prgbMultiResult);
        
        HRESULT ( STDMETHODCALLTYPE *Checkout )( 
            __RPC__in IVsSccManager * This,
            /* [in] */ DWORD dwCookie,
            /* [in] */ DWORD dwReservedFlags,
            /* [in] */ int cFiles,
            /* [size_is][in] */ __RPC__in_ecount_full(cFiles) const LPCOLESTR rgpszMkDocuments[  ],
            /* [out] */ __RPC__out DWORD *pdwReserved);
        
        HRESULT ( STDMETHODCALLTYPE *CheckoutForSave )( 
            __RPC__in IVsSccManager * This,
            /* [in] */ DWORD dwCookie,
            /* [in] */ DWORD dwReservedFlags,
            /* [in] */ int cFiles,
            /* [size_is][in] */ __RPC__in_ecount_full(cFiles) const LPCOLESTR rgpszMkDocuments[  ],
            /* [out] */ __RPC__out VSSCC_CHECKOUTFORSAVERESULT *pdw);
        
        HRESULT ( STDMETHODCALLTYPE *OnAfterDisconnectProject )( 
            __RPC__in IVsSccManager * This,
            /* [in] */ __RPC__in_opt IVsSccProject *pProject,
            /* [in] */ DWORD dwCookie);
        
        HRESULT ( STDMETHODCALLTYPE *IsTargetOfPendingRename )( 
            __RPC__in IVsSccManager * This,
            /* [in] */ __RPC__in LPCOLESTR targetFile,
            /* [out] */ __RPC__out BOOL *pbResult);
        
        END_INTERFACE
    } IVsSccManagerVtbl;

    interface IVsSccManager
    {
        CONST_VTBL struct IVsSccManagerVtbl *lpVtbl;
    };

    

#ifdef COBJMACROS


#define IVsSccManager_QueryInterface(This,riid,ppvObject)	\
    ( (This)->lpVtbl -> QueryInterface(This,riid,ppvObject) ) 

#define IVsSccManager_AddRef(This)	\
    ( (This)->lpVtbl -> AddRef(This) ) 

#define IVsSccManager_Release(This)	\
    ( (This)->lpVtbl -> Release(This) ) 


#define IVsSccManager_RegisterSccProject(This,pProject,pszSccProjectName,pszSccAuxPath,pszSccLocalPath,pszProvider,piSccEngine,pdwCookie)	\
    ( (This)->lpVtbl -> RegisterSccProject(This,pProject,pszSccProjectName,pszSccAuxPath,pszSccLocalPath,pszProvider,piSccEngine,pdwCookie) ) 

#define IVsSccManager_UnregisterSccProject(This,dwCookie)	\
    ( (This)->lpVtbl -> UnregisterSccProject(This,dwCookie) ) 

#define IVsSccManager_IsValidOp(This,dwCookie,cFiles,rgpszFullpaths,sccop,dwFlags,pfValid)	\
    ( (This)->lpVtbl -> IsValidOp(This,dwCookie,cFiles,rgpszFullpaths,sccop,dwFlags,pfValid) ) 

#define IVsSccManager_FileStatus(This,dwCookie,cFiles,rgpszFullpaths,pStatus,prgStatus)	\
    ( (This)->lpVtbl -> FileStatus(This,dwCookie,cFiles,rgpszFullpaths,pStatus,prgStatus) ) 

#define IVsSccManager_ProjectStatus(This,pProject,pbStatus)	\
    ( (This)->lpVtbl -> ProjectStatus(This,pProject,pbStatus) ) 

#define IVsSccManager_PollFileChangeCause(This,dwCookie,pszFilename,pbBusy)	\
    ( (This)->lpVtbl -> PollFileChangeCause(This,dwCookie,pszFilename,pbBusy) ) 

#define IVsSccManager_CheckoutForEdit(This,dwCookie,cFiles,rgpszMkDocuments,bReloadOK,pdw)	\
    ( (This)->lpVtbl -> CheckoutForEdit(This,dwCookie,cFiles,rgpszMkDocuments,bReloadOK,pdw) ) 

#define IVsSccManager_IsInstalled(This,pszProviderName,pbInstalled)	\
    ( (This)->lpVtbl -> IsInstalled(This,pszProviderName,pbInstalled) ) 

#define IVsSccManager_GetVsSccEngine(This,pszProviderName,pszFullPathToSccDll,ppEngine)	\
    ( (This)->lpVtbl -> GetVsSccEngine(This,pszProviderName,pszFullPathToSccDll,ppEngine) ) 

#define IVsSccManager_BrowseForProject(This,pbstrDirectory,pfOK)	\
    ( (This)->lpVtbl -> BrowseForProject(This,pbstrDirectory,pfOK) ) 

#define IVsSccManager_CancelAfterBrowseForProject(This)	\
    ( (This)->lpVtbl -> CancelAfterBrowseForProject(This) ) 

#define IVsSccManager_CheckoutMightChange(This,dwCookie,multiLogic,cFiles,rgpszFullpaths,pbSingleResult,prgbMultiResult)	\
    ( (This)->lpVtbl -> CheckoutMightChange(This,dwCookie,multiLogic,cFiles,rgpszFullpaths,pbSingleResult,prgbMultiResult) ) 

#define IVsSccManager_Checkout(This,dwCookie,dwReservedFlags,cFiles,rgpszMkDocuments,pdwReserved)	\
    ( (This)->lpVtbl -> Checkout(This,dwCookie,dwReservedFlags,cFiles,rgpszMkDocuments,pdwReserved) ) 

#define IVsSccManager_CheckoutForSave(This,dwCookie,dwReservedFlags,cFiles,rgpszMkDocuments,pdw)	\
    ( (This)->lpVtbl -> CheckoutForSave(This,dwCookie,dwReservedFlags,cFiles,rgpszMkDocuments,pdw) ) 

#define IVsSccManager_OnAfterDisconnectProject(This,pProject,dwCookie)	\
    ( (This)->lpVtbl -> OnAfterDisconnectProject(This,pProject,dwCookie) ) 

#define IVsSccManager_IsTargetOfPendingRename(This,targetFile,pbResult)	\
    ( (This)->lpVtbl -> IsTargetOfPendingRename(This,targetFile,pbResult) ) 

#endif /* COBJMACROS */


#endif 	/* C style interface */




#endif 	/* __IVsSccManager_INTERFACE_DEFINED__ */


#ifndef __IEnableOpenFromScc_INTERFACE_DEFINED__
#define __IEnableOpenFromScc_INTERFACE_DEFINED__

/* interface IEnableOpenFromScc */
/* [object][uuid] */ 


EXTERN_C const IID IID_IEnableOpenFromScc;

#if defined(__cplusplus) && !defined(CINTERFACE)
    
    MIDL_INTERFACE("795635A1-4522-11d1-8DCE-00AA00A3F593")
    IEnableOpenFromScc : public IUnknown
    {
    public:
    };
    
    
#else 	/* C style interface */

    typedef struct IEnableOpenFromSccVtbl
    {
        BEGIN_INTERFACE
        
        HRESULT ( STDMETHODCALLTYPE *QueryInterface )( 
            __RPC__in IEnableOpenFromScc * This,
            /* [in] */ __RPC__in REFIID riid,
            /* [annotation][iid_is][out] */ 
            _COM_Outptr_  void **ppvObject);
        
        ULONG ( STDMETHODCALLTYPE *AddRef )( 
            __RPC__in IEnableOpenFromScc * This);
        
        ULONG ( STDMETHODCALLTYPE *Release )( 
            __RPC__in IEnableOpenFromScc * This);
        
        END_INTERFACE
    } IEnableOpenFromSccVtbl;

    interface IEnableOpenFromScc
    {
        CONST_VTBL struct IEnableOpenFromSccVtbl *lpVtbl;
    };

    

#ifdef COBJMACROS


#define IEnableOpenFromScc_QueryInterface(This,riid,ppvObject)	\
    ( (This)->lpVtbl -> QueryInterface(This,riid,ppvObject) ) 

#define IEnableOpenFromScc_AddRef(This)	\
    ( (This)->lpVtbl -> AddRef(This) ) 

#define IEnableOpenFromScc_Release(This)	\
    ( (This)->lpVtbl -> Release(This) ) 


#endif /* COBJMACROS */


#endif 	/* C style interface */




#endif 	/* __IEnableOpenFromScc_INTERFACE_DEFINED__ */


/* interface __MIDL_itf_vssccmgr_0000_0002 */
/* [local] */ 

#define UICONTEXT_EnableOpenFromScc IID_IEnableOpenFromScc


extern RPC_IF_HANDLE __MIDL_itf_vssccmgr_0000_0002_v0_0_c_ifspec;
extern RPC_IF_HANDLE __MIDL_itf_vssccmgr_0000_0002_v0_0_s_ifspec;

/* Additional Prototypes for ALL interfaces */

unsigned long             __RPC_USER  BSTR_UserSize(     __RPC__in unsigned long *, unsigned long            , __RPC__in BSTR * ); 
unsigned char * __RPC_USER  BSTR_UserMarshal(  __RPC__in unsigned long *, __RPC__inout_xcount(0) unsigned char *, __RPC__in BSTR * ); 
unsigned char * __RPC_USER  BSTR_UserUnmarshal(__RPC__in unsigned long *, __RPC__in_xcount(0) unsigned char *, __RPC__out BSTR * ); 
void                      __RPC_USER  BSTR_UserFree(     __RPC__in unsigned long *, __RPC__in BSTR * ); 

/* end of Additional Prototypes */

#ifdef __cplusplus
}
#endif

#endif


