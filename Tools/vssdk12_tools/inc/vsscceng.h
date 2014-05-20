

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

#ifndef __vsscceng_h__
#define __vsscceng_h__

#if defined(_MSC_VER) && (_MSC_VER >= 1020)
#pragma once
#endif

/* Forward Declarations */ 

#ifndef __IVsSccPopulateList_FWD_DEFINED__
#define __IVsSccPopulateList_FWD_DEFINED__
typedef interface IVsSccPopulateList IVsSccPopulateList;

#endif 	/* __IVsSccPopulateList_FWD_DEFINED__ */


#ifndef __IVsSccEngine_FWD_DEFINED__
#define __IVsSccEngine_FWD_DEFINED__
typedef interface IVsSccEngine IVsSccEngine;

#endif 	/* __IVsSccEngine_FWD_DEFINED__ */


/* header files for imported files */
#include "objidl.h"
#include "vsshell.h"

#ifdef __cplusplus
extern "C"{
#endif 


/* interface __MIDL_itf_vsscceng_0000_0000 */
/* [local] */ 

#pragma once

enum tagVSSCC_CAP
    {
        VSSCC_CAP_CACHED_QUERYINFO	= 0x1,
        VSSCC_CAP_DIRECTORY_BASED_PROJECT	= 0x2,
        VSSCC_CAP_ENABLE_WHEN_SLOW	= 0x4,
        VSSCC_CAP_READONLY_LOGINID	= 0x8,
        VSSCC_CAP_NO_REFRESH	= 0x10
    } ;
typedef DWORD VSSCC_CAP;

#if defined(__cplusplus)
extern "C++" {
inline tagVSSCC_CAP operator|(tagVSSCC_CAP x, tagVSSCC_CAP y)
{
 typedef unsigned long ulong;
 return tagVSSCC_CAP(ulong(x)|ulong(y));
}
inline tagVSSCC_CAP operator&(tagVSSCC_CAP x, tagVSSCC_CAP y)
{
 typedef unsigned long ulong;
 return tagVSSCC_CAP(ulong(x)&ulong(y));
}
} // extern C++
#endif
#ifndef __cplusplus
#ifndef SCCCOMMAND_DEFINED
#define SCCCOMMAND_DEFINED
typedef 
enum SCCCOMMAND
    {
        SCCCOMMAND_PAD	= 0
    } 	SCCCOMMAND;

#endif
#endif
#if defined(_MSC_VER) || defined(__cplusplus) || defined(__STDC__) /* C or C++ */
#include "scc.h"
#endif


extern RPC_IF_HANDLE __MIDL_itf_vsscceng_0000_0000_v0_0_c_ifspec;
extern RPC_IF_HANDLE __MIDL_itf_vsscceng_0000_0000_v0_0_s_ifspec;

#ifndef __IVsSccPopulateList_INTERFACE_DEFINED__
#define __IVsSccPopulateList_INTERFACE_DEFINED__

/* interface IVsSccPopulateList */
/* [object][uuid] */ 


EXTERN_C const IID IID_IVsSccPopulateList;

#if defined(__cplusplus) && !defined(CINTERFACE)
    
    MIDL_INTERFACE("53544C4D-F8CF-11d0-8D84-00AA00A3F593")
    IVsSccPopulateList : public IUnknown
    {
    public:
        virtual HRESULT STDMETHODCALLTYPE CallbackPopulateList( 
            /* [in] */ BOOL bAddKeep,
            /* [in] */ DWORD dwStatus,
            /* [in] */ __RPC__in LPCOLESTR pszFile,
            /* [out] */ __RPC__out BOOL *pbResult) = 0;
        
        virtual HRESULT STDMETHODCALLTYPE CallbackPopulateDirList( 
            /* [in] */ BOOL bFolder,
            /* [in] */ __RPC__in LPCOLESTR pszDirectoryOrFileName,
            /* [out] */ __RPC__out BOOL *pbResult) = 0;
        
    };
    
    
#else 	/* C style interface */

    typedef struct IVsSccPopulateListVtbl
    {
        BEGIN_INTERFACE
        
        HRESULT ( STDMETHODCALLTYPE *QueryInterface )( 
            __RPC__in IVsSccPopulateList * This,
            /* [in] */ __RPC__in REFIID riid,
            /* [annotation][iid_is][out] */ 
            _COM_Outptr_  void **ppvObject);
        
        ULONG ( STDMETHODCALLTYPE *AddRef )( 
            __RPC__in IVsSccPopulateList * This);
        
        ULONG ( STDMETHODCALLTYPE *Release )( 
            __RPC__in IVsSccPopulateList * This);
        
        HRESULT ( STDMETHODCALLTYPE *CallbackPopulateList )( 
            __RPC__in IVsSccPopulateList * This,
            /* [in] */ BOOL bAddKeep,
            /* [in] */ DWORD dwStatus,
            /* [in] */ __RPC__in LPCOLESTR pszFile,
            /* [out] */ __RPC__out BOOL *pbResult);
        
        HRESULT ( STDMETHODCALLTYPE *CallbackPopulateDirList )( 
            __RPC__in IVsSccPopulateList * This,
            /* [in] */ BOOL bFolder,
            /* [in] */ __RPC__in LPCOLESTR pszDirectoryOrFileName,
            /* [out] */ __RPC__out BOOL *pbResult);
        
        END_INTERFACE
    } IVsSccPopulateListVtbl;

    interface IVsSccPopulateList
    {
        CONST_VTBL struct IVsSccPopulateListVtbl *lpVtbl;
    };

    

#ifdef COBJMACROS


#define IVsSccPopulateList_QueryInterface(This,riid,ppvObject)	\
    ( (This)->lpVtbl -> QueryInterface(This,riid,ppvObject) ) 

#define IVsSccPopulateList_AddRef(This)	\
    ( (This)->lpVtbl -> AddRef(This) ) 

#define IVsSccPopulateList_Release(This)	\
    ( (This)->lpVtbl -> Release(This) ) 


#define IVsSccPopulateList_CallbackPopulateList(This,bAddKeep,dwStatus,pszFile,pbResult)	\
    ( (This)->lpVtbl -> CallbackPopulateList(This,bAddKeep,dwStatus,pszFile,pbResult) ) 

#define IVsSccPopulateList_CallbackPopulateDirList(This,bFolder,pszDirectoryOrFileName,pbResult)	\
    ( (This)->lpVtbl -> CallbackPopulateDirList(This,bFolder,pszDirectoryOrFileName,pbResult) ) 

#endif /* COBJMACROS */


#endif 	/* C style interface */




#endif 	/* __IVsSccPopulateList_INTERFACE_DEFINED__ */


/* interface __MIDL_itf_vsscceng_0000_0001 */
/* [local] */ 

typedef struct __VSITEMSELECTION PAIR_IVSHIERARCHY_VSITEMID;



extern RPC_IF_HANDLE __MIDL_itf_vsscceng_0000_0001_v0_0_c_ifspec;
extern RPC_IF_HANDLE __MIDL_itf_vsscceng_0000_0001_v0_0_s_ifspec;

#ifndef __IVsSccEngine_INTERFACE_DEFINED__
#define __IVsSccEngine_INTERFACE_DEFINED__

/* interface IVsSccEngine */
/* [object][uuid] */ 


EXTERN_C const IID IID_IVsSccEngine;

#if defined(__cplusplus) && !defined(CINTERFACE)
    
    MIDL_INTERFACE("53544C4D-F82C-11d0-8D84-00AA00A3F593")
    IVsSccEngine : public IUnknown
    {
    public:
        virtual HRESULT STDMETHODCALLTYPE GetVersion( 
            /* [out] */ __RPC__out LONG *pnVersion) = 0;
        
        virtual HRESULT STDMETHODCALLTYPE Initialize( 
            /* [out] */ __RPC__out VSCOOKIE *pdwCookie,
            /* [in] */ __RPC__in HWND hWnd,
            /* [in] */ __RPC__in LPCOLESTR pszCallerName,
            /* [out] */ __RPC__deref_out_opt BSTR *pbstrSccName,
            /* [out] */ __RPC__out DWORD *pdwCapabilityBits,
            /* [out] */ __RPC__out VSSCC_CAP *pdwVSCapabilityBits,
            /* [out] */ __RPC__deref_out_opt BSTR *pbstrAuxPathLabel,
            /* [out] */ __RPC__out LONG *pnCheckoutCommentLen,
            /* [out] */ __RPC__out LONG *pnCommentLen) = 0;
        
        virtual HRESULT STDMETHODCALLTYPE Uninitialize( 
            /* [in] */ VSCOOKIE cookie) = 0;
        
        virtual HRESULT STDMETHODCALLTYPE OpenProject( 
            /* [in] */ VSCOOKIE dwCookie,
            /* [in] */ __RPC__in HWND hWnd,
            /* [out][in] */ __RPC__deref_inout_opt BSTR *pbstrUser,
            /* [out][in] */ __RPC__deref_inout_opt BSTR *pbstrProjName,
            /* [in] */ __RPC__in LPCOLESTR pszLocalProjPath,
            /* [out][in] */ __RPC__deref_inout_opt BSTR *pbstrAuxProjPath,
            /* [in] */ __RPC__in LPCOLESTR pszComment,
            /* [in] */ DWORD dwFlags) = 0;
        
        virtual HRESULT STDMETHODCALLTYPE CloseProject( 
            VSCOOKIE dwCookie) = 0;
        
        virtual HRESULT STDMETHODCALLTYPE GetProjPath( 
            /* [in] */ VSCOOKIE dwCookie,
            /* [in] */ __RPC__in HWND hWnd,
            /* [out][in] */ __RPC__deref_inout_opt BSTR *pbstrUser,
            /* [out][in] */ __RPC__deref_inout_opt BSTR *pbstrProjName,
            /* [out][in] */ __RPC__deref_inout_opt BSTR *pbstrLocalPath,
            /* [out][in] */ __RPC__deref_inout_opt BSTR *pbstrAuxProjPath,
            /* [in] */ BOOL bAllowChangePath,
            /* [out][in] */ __RPC__inout BOOL *pbInNew) = 0;
        
        virtual HRESULT STDMETHODCALLTYPE Get( 
            /* [in] */ VSCOOKIE dwCookie,
            /* [in] */ __RPC__in HWND hWnd,
            /* [in] */ LONG cFiles,
            /* [size_is][in] */ __RPC__in_ecount_full(cFiles) const LPCOLESTR rgpszFileNames[  ],
            /* [size_is][in] */ __RPC__in_ecount_full(cFiles) const PAIR_IVSHIERARCHY_VSITEMID rgHierarchyItemIDPairs[  ],
            /* [in] */ DWORD dwFlags,
            /* [in] */ DWORD dwOptions) = 0;
        
        virtual HRESULT STDMETHODCALLTYPE Checkout( 
            /* [in] */ VSCOOKIE dwCookie,
            /* [in] */ __RPC__in HWND hWnd,
            /* [in] */ LONG cFiles,
            /* [size_is][in] */ __RPC__in_ecount_full(cFiles) const LPCOLESTR rgpszFileNames[  ],
            /* [size_is][in] */ __RPC__in_ecount_full(cFiles) const PAIR_IVSHIERARCHY_VSITEMID rgHierarchyItemIDPairs[  ],
            /* [in] */ __RPC__in LPCOLESTR pszComment,
            /* [in] */ DWORD dwFlags,
            /* [in] */ DWORD dwOptions) = 0;
        
        virtual HRESULT STDMETHODCALLTYPE Uncheckout( 
            /* [in] */ VSCOOKIE dwCookie,
            /* [in] */ __RPC__in HWND hWnd,
            /* [in] */ LONG cFiles,
            /* [size_is][in] */ __RPC__in_ecount_full(cFiles) const LPCOLESTR rgpszFileNames[  ],
            /* [size_is][in] */ __RPC__in_ecount_full(cFiles) const PAIR_IVSHIERARCHY_VSITEMID rgHierarchyItemIDPairs[  ],
            /* [in] */ DWORD dwFlags,
            /* [in] */ DWORD dwOptions) = 0;
        
        virtual HRESULT STDMETHODCALLTYPE Checkin( 
            /* [in] */ VSCOOKIE dwCookie,
            /* [in] */ __RPC__in HWND hWnd,
            /* [in] */ LONG cFiles,
            /* [size_is][in] */ __RPC__in_ecount_full(cFiles) const LPCOLESTR rgpszFileNames[  ],
            /* [size_is][in] */ __RPC__in_ecount_full(cFiles) const PAIR_IVSHIERARCHY_VSITEMID rgHierarchyItemIDPairs[  ],
            /* [in] */ __RPC__in LPCOLESTR pszComment,
            /* [in] */ DWORD dwFlags,
            /* [in] */ DWORD dwOptions) = 0;
        
        virtual HRESULT STDMETHODCALLTYPE Add( 
            /* [in] */ VSCOOKIE dwCookie,
            /* [in] */ __RPC__in HWND hWnd,
            /* [in] */ LONG cFiles,
            /* [size_is][in] */ __RPC__in_ecount_full(cFiles) const LPCOLESTR rgpszFileNames[  ],
            /* [size_is][in] */ __RPC__in_ecount_full(cFiles) const PAIR_IVSHIERARCHY_VSITEMID rgHierarchyItemIDPairs[  ],
            /* [in] */ __RPC__in LPCOLESTR pszComment,
            /* [size_is][in] */ __RPC__in_ecount_full(cFiles) const DWORD rgdwFlags[  ],
            /* [in] */ DWORD dwOptions) = 0;
        
        virtual HRESULT STDMETHODCALLTYPE Remove( 
            /* [in] */ VSCOOKIE dwCookie,
            /* [in] */ __RPC__in HWND hWnd,
            /* [in] */ LONG cFiles,
            /* [size_is][in] */ __RPC__in_ecount_full(cFiles) const LPCOLESTR rgpszFileNames[  ],
            /* [size_is][in] */ __RPC__in_ecount_full(cFiles) const PAIR_IVSHIERARCHY_VSITEMID rgHierarchyItemIDPairs[  ],
            /* [in] */ __RPC__in LPCOLESTR pszComment,
            /* [in] */ DWORD dwFlags,
            /* [in] */ DWORD dwOptions) = 0;
        
        virtual HRESULT STDMETHODCALLTYPE Rename( 
            /* [in] */ VSCOOKIE dwCookie,
            /* [in] */ __RPC__in HWND hWnd,
            /* [in] */ __RPC__in LPCOLESTR pszFrom,
            /* [in] */ __RPC__in LPCOLESTR pszTo) = 0;
        
        virtual HRESULT STDMETHODCALLTYPE Diff( 
            /* [in] */ VSCOOKIE dwCookie,
            /* [in] */ __RPC__in HWND hWnd,
            /* [in] */ __RPC__in LPCOLESTR pszFilename,
            /* [in] */ __RPC__in const PAIR_IVSHIERARCHY_VSITEMID *pHierarchyItemIDPair,
            /* [in] */ DWORD dwFlags,
            /* [in] */ DWORD dwOptions) = 0;
        
        virtual HRESULT STDMETHODCALLTYPE History( 
            /* [in] */ VSCOOKIE dwCookie,
            /* [in] */ __RPC__in HWND hWnd,
            /* [in] */ LONG cFiles,
            /* [size_is][in] */ __RPC__in_ecount_full(cFiles) const LPCOLESTR rgpszFileNames[  ],
            /* [size_is][in] */ __RPC__in_ecount_full(cFiles) const PAIR_IVSHIERARCHY_VSITEMID rgHierarchyItemIDPairs[  ],
            /* [in] */ DWORD dwFlags,
            /* [in] */ DWORD dwOptions) = 0;
        
        virtual HRESULT STDMETHODCALLTYPE Properties( 
            /* [in] */ VSCOOKIE dwCookie,
            /* [in] */ __RPC__in HWND hWnd,
            /* [in] */ __RPC__in LPCOLESTR pszFileName,
            /* [in] */ __RPC__in const PAIR_IVSHIERARCHY_VSITEMID *pHierarchyItemIDPair) = 0;
        
        virtual HRESULT STDMETHODCALLTYPE QueryInfo( 
            /* [in] */ VSCOOKIE dwCookie,
            /* [in] */ LONG cFiles,
            /* [size_is][in] */ __RPC__in_ecount_full(cFiles) const LPCOLESTR rgpszFileNames[  ],
            /* [size_is][in] */ __RPC__in_ecount_full(cFiles) const PAIR_IVSHIERARCHY_VSITEMID rgHierarchyItemIDPairs[  ],
            /* [size_is][out] */ __RPC__out_ecount_full(cFiles) DWORD rgStatus[  ]) = 0;
        
        virtual HRESULT STDMETHODCALLTYPE PopulateList( 
            /* [in] */ VSCOOKIE dwCookie,
            /* [in] */ SCCCOMMAND nCommand,
            /* [in] */ LONG cFiles,
            /* [size_is][in] */ __RPC__in_ecount_full(cFiles) const LPCOLESTR rgpszFileNames[  ],
            /* [size_is][in] */ __RPC__in_ecount_full(cFiles) const PAIR_IVSHIERARCHY_VSITEMID rgHierarchyItemIDPairs[  ],
            /* [in] */ __RPC__in_opt IVsSccPopulateList *pfnPopulate,
            /* [size_is][out] */ __RPC__out_ecount_full(cFiles) DWORD rgStatus[  ],
            /* [in] */ DWORD dwFlags) = 0;
        
        virtual HRESULT STDMETHODCALLTYPE GetEvents( 
            /* [in] */ VSCOOKIE dwCookie,
            /* [out] */ __RPC__deref_out_opt BSTR *pbstrFileName,
            /* [out] */ __RPC__out DWORD *dwStatus,
            /* [out] */ __RPC__out LONG *pnEventsRemaining) = 0;
        
        virtual HRESULT STDMETHODCALLTYPE RunScc( 
            /* [in] */ VSCOOKIE dwCookie,
            /* [in] */ __RPC__in HWND hWnd,
            /* [in] */ LONG cFiles,
            /* [size_is][in] */ __RPC__in_ecount_full(cFiles) const LPCOLESTR rgpszFileNames[  ],
            /* [size_is][in] */ __RPC__in_ecount_full(cFiles) const PAIR_IVSHIERARCHY_VSITEMID rgHierarchyItemIDPairs[  ]) = 0;
        
        virtual HRESULT STDMETHODCALLTYPE GetCommandOptions( 
            /* [in] */ VSCOOKIE dwCookie,
            /* [in] */ __RPC__in HWND hWnd,
            /* [in] */ SCCCOMMAND nCommand,
            /* [out][in] */ __RPC__inout DWORD *pdwOptions) = 0;
        
        virtual HRESULT STDMETHODCALLTYPE AddFromScc( 
            /* [in] */ VSCOOKIE dwCookie,
            /* [in] */ __RPC__in HWND hWnd,
            /* [out] */ __RPC__out CALPOLESTR *pCaFileNames) = 0;
        
        virtual HRESULT STDMETHODCALLTYPE SetOption( 
            /* [in] */ VSCOOKIE dwCookie,
            /* [in] */ LONG nOption,
            /* [in] */ DWORD dwVal) = 0;
        
    };
    
    
#else 	/* C style interface */

    typedef struct IVsSccEngineVtbl
    {
        BEGIN_INTERFACE
        
        HRESULT ( STDMETHODCALLTYPE *QueryInterface )( 
            __RPC__in IVsSccEngine * This,
            /* [in] */ __RPC__in REFIID riid,
            /* [annotation][iid_is][out] */ 
            _COM_Outptr_  void **ppvObject);
        
        ULONG ( STDMETHODCALLTYPE *AddRef )( 
            __RPC__in IVsSccEngine * This);
        
        ULONG ( STDMETHODCALLTYPE *Release )( 
            __RPC__in IVsSccEngine * This);
        
        HRESULT ( STDMETHODCALLTYPE *GetVersion )( 
            __RPC__in IVsSccEngine * This,
            /* [out] */ __RPC__out LONG *pnVersion);
        
        HRESULT ( STDMETHODCALLTYPE *Initialize )( 
            __RPC__in IVsSccEngine * This,
            /* [out] */ __RPC__out VSCOOKIE *pdwCookie,
            /* [in] */ __RPC__in HWND hWnd,
            /* [in] */ __RPC__in LPCOLESTR pszCallerName,
            /* [out] */ __RPC__deref_out_opt BSTR *pbstrSccName,
            /* [out] */ __RPC__out DWORD *pdwCapabilityBits,
            /* [out] */ __RPC__out VSSCC_CAP *pdwVSCapabilityBits,
            /* [out] */ __RPC__deref_out_opt BSTR *pbstrAuxPathLabel,
            /* [out] */ __RPC__out LONG *pnCheckoutCommentLen,
            /* [out] */ __RPC__out LONG *pnCommentLen);
        
        HRESULT ( STDMETHODCALLTYPE *Uninitialize )( 
            __RPC__in IVsSccEngine * This,
            /* [in] */ VSCOOKIE cookie);
        
        HRESULT ( STDMETHODCALLTYPE *OpenProject )( 
            __RPC__in IVsSccEngine * This,
            /* [in] */ VSCOOKIE dwCookie,
            /* [in] */ __RPC__in HWND hWnd,
            /* [out][in] */ __RPC__deref_inout_opt BSTR *pbstrUser,
            /* [out][in] */ __RPC__deref_inout_opt BSTR *pbstrProjName,
            /* [in] */ __RPC__in LPCOLESTR pszLocalProjPath,
            /* [out][in] */ __RPC__deref_inout_opt BSTR *pbstrAuxProjPath,
            /* [in] */ __RPC__in LPCOLESTR pszComment,
            /* [in] */ DWORD dwFlags);
        
        HRESULT ( STDMETHODCALLTYPE *CloseProject )( 
            __RPC__in IVsSccEngine * This,
            VSCOOKIE dwCookie);
        
        HRESULT ( STDMETHODCALLTYPE *GetProjPath )( 
            __RPC__in IVsSccEngine * This,
            /* [in] */ VSCOOKIE dwCookie,
            /* [in] */ __RPC__in HWND hWnd,
            /* [out][in] */ __RPC__deref_inout_opt BSTR *pbstrUser,
            /* [out][in] */ __RPC__deref_inout_opt BSTR *pbstrProjName,
            /* [out][in] */ __RPC__deref_inout_opt BSTR *pbstrLocalPath,
            /* [out][in] */ __RPC__deref_inout_opt BSTR *pbstrAuxProjPath,
            /* [in] */ BOOL bAllowChangePath,
            /* [out][in] */ __RPC__inout BOOL *pbInNew);
        
        HRESULT ( STDMETHODCALLTYPE *Get )( 
            __RPC__in IVsSccEngine * This,
            /* [in] */ VSCOOKIE dwCookie,
            /* [in] */ __RPC__in HWND hWnd,
            /* [in] */ LONG cFiles,
            /* [size_is][in] */ __RPC__in_ecount_full(cFiles) const LPCOLESTR rgpszFileNames[  ],
            /* [size_is][in] */ __RPC__in_ecount_full(cFiles) const PAIR_IVSHIERARCHY_VSITEMID rgHierarchyItemIDPairs[  ],
            /* [in] */ DWORD dwFlags,
            /* [in] */ DWORD dwOptions);
        
        HRESULT ( STDMETHODCALLTYPE *Checkout )( 
            __RPC__in IVsSccEngine * This,
            /* [in] */ VSCOOKIE dwCookie,
            /* [in] */ __RPC__in HWND hWnd,
            /* [in] */ LONG cFiles,
            /* [size_is][in] */ __RPC__in_ecount_full(cFiles) const LPCOLESTR rgpszFileNames[  ],
            /* [size_is][in] */ __RPC__in_ecount_full(cFiles) const PAIR_IVSHIERARCHY_VSITEMID rgHierarchyItemIDPairs[  ],
            /* [in] */ __RPC__in LPCOLESTR pszComment,
            /* [in] */ DWORD dwFlags,
            /* [in] */ DWORD dwOptions);
        
        HRESULT ( STDMETHODCALLTYPE *Uncheckout )( 
            __RPC__in IVsSccEngine * This,
            /* [in] */ VSCOOKIE dwCookie,
            /* [in] */ __RPC__in HWND hWnd,
            /* [in] */ LONG cFiles,
            /* [size_is][in] */ __RPC__in_ecount_full(cFiles) const LPCOLESTR rgpszFileNames[  ],
            /* [size_is][in] */ __RPC__in_ecount_full(cFiles) const PAIR_IVSHIERARCHY_VSITEMID rgHierarchyItemIDPairs[  ],
            /* [in] */ DWORD dwFlags,
            /* [in] */ DWORD dwOptions);
        
        HRESULT ( STDMETHODCALLTYPE *Checkin )( 
            __RPC__in IVsSccEngine * This,
            /* [in] */ VSCOOKIE dwCookie,
            /* [in] */ __RPC__in HWND hWnd,
            /* [in] */ LONG cFiles,
            /* [size_is][in] */ __RPC__in_ecount_full(cFiles) const LPCOLESTR rgpszFileNames[  ],
            /* [size_is][in] */ __RPC__in_ecount_full(cFiles) const PAIR_IVSHIERARCHY_VSITEMID rgHierarchyItemIDPairs[  ],
            /* [in] */ __RPC__in LPCOLESTR pszComment,
            /* [in] */ DWORD dwFlags,
            /* [in] */ DWORD dwOptions);
        
        HRESULT ( STDMETHODCALLTYPE *Add )( 
            __RPC__in IVsSccEngine * This,
            /* [in] */ VSCOOKIE dwCookie,
            /* [in] */ __RPC__in HWND hWnd,
            /* [in] */ LONG cFiles,
            /* [size_is][in] */ __RPC__in_ecount_full(cFiles) const LPCOLESTR rgpszFileNames[  ],
            /* [size_is][in] */ __RPC__in_ecount_full(cFiles) const PAIR_IVSHIERARCHY_VSITEMID rgHierarchyItemIDPairs[  ],
            /* [in] */ __RPC__in LPCOLESTR pszComment,
            /* [size_is][in] */ __RPC__in_ecount_full(cFiles) const DWORD rgdwFlags[  ],
            /* [in] */ DWORD dwOptions);
        
        HRESULT ( STDMETHODCALLTYPE *Remove )( 
            __RPC__in IVsSccEngine * This,
            /* [in] */ VSCOOKIE dwCookie,
            /* [in] */ __RPC__in HWND hWnd,
            /* [in] */ LONG cFiles,
            /* [size_is][in] */ __RPC__in_ecount_full(cFiles) const LPCOLESTR rgpszFileNames[  ],
            /* [size_is][in] */ __RPC__in_ecount_full(cFiles) const PAIR_IVSHIERARCHY_VSITEMID rgHierarchyItemIDPairs[  ],
            /* [in] */ __RPC__in LPCOLESTR pszComment,
            /* [in] */ DWORD dwFlags,
            /* [in] */ DWORD dwOptions);
        
        HRESULT ( STDMETHODCALLTYPE *Rename )( 
            __RPC__in IVsSccEngine * This,
            /* [in] */ VSCOOKIE dwCookie,
            /* [in] */ __RPC__in HWND hWnd,
            /* [in] */ __RPC__in LPCOLESTR pszFrom,
            /* [in] */ __RPC__in LPCOLESTR pszTo);
        
        HRESULT ( STDMETHODCALLTYPE *Diff )( 
            __RPC__in IVsSccEngine * This,
            /* [in] */ VSCOOKIE dwCookie,
            /* [in] */ __RPC__in HWND hWnd,
            /* [in] */ __RPC__in LPCOLESTR pszFilename,
            /* [in] */ __RPC__in const PAIR_IVSHIERARCHY_VSITEMID *pHierarchyItemIDPair,
            /* [in] */ DWORD dwFlags,
            /* [in] */ DWORD dwOptions);
        
        HRESULT ( STDMETHODCALLTYPE *History )( 
            __RPC__in IVsSccEngine * This,
            /* [in] */ VSCOOKIE dwCookie,
            /* [in] */ __RPC__in HWND hWnd,
            /* [in] */ LONG cFiles,
            /* [size_is][in] */ __RPC__in_ecount_full(cFiles) const LPCOLESTR rgpszFileNames[  ],
            /* [size_is][in] */ __RPC__in_ecount_full(cFiles) const PAIR_IVSHIERARCHY_VSITEMID rgHierarchyItemIDPairs[  ],
            /* [in] */ DWORD dwFlags,
            /* [in] */ DWORD dwOptions);
        
        HRESULT ( STDMETHODCALLTYPE *Properties )( 
            __RPC__in IVsSccEngine * This,
            /* [in] */ VSCOOKIE dwCookie,
            /* [in] */ __RPC__in HWND hWnd,
            /* [in] */ __RPC__in LPCOLESTR pszFileName,
            /* [in] */ __RPC__in const PAIR_IVSHIERARCHY_VSITEMID *pHierarchyItemIDPair);
        
        HRESULT ( STDMETHODCALLTYPE *QueryInfo )( 
            __RPC__in IVsSccEngine * This,
            /* [in] */ VSCOOKIE dwCookie,
            /* [in] */ LONG cFiles,
            /* [size_is][in] */ __RPC__in_ecount_full(cFiles) const LPCOLESTR rgpszFileNames[  ],
            /* [size_is][in] */ __RPC__in_ecount_full(cFiles) const PAIR_IVSHIERARCHY_VSITEMID rgHierarchyItemIDPairs[  ],
            /* [size_is][out] */ __RPC__out_ecount_full(cFiles) DWORD rgStatus[  ]);
        
        HRESULT ( STDMETHODCALLTYPE *PopulateList )( 
            __RPC__in IVsSccEngine * This,
            /* [in] */ VSCOOKIE dwCookie,
            /* [in] */ SCCCOMMAND nCommand,
            /* [in] */ LONG cFiles,
            /* [size_is][in] */ __RPC__in_ecount_full(cFiles) const LPCOLESTR rgpszFileNames[  ],
            /* [size_is][in] */ __RPC__in_ecount_full(cFiles) const PAIR_IVSHIERARCHY_VSITEMID rgHierarchyItemIDPairs[  ],
            /* [in] */ __RPC__in_opt IVsSccPopulateList *pfnPopulate,
            /* [size_is][out] */ __RPC__out_ecount_full(cFiles) DWORD rgStatus[  ],
            /* [in] */ DWORD dwFlags);
        
        HRESULT ( STDMETHODCALLTYPE *GetEvents )( 
            __RPC__in IVsSccEngine * This,
            /* [in] */ VSCOOKIE dwCookie,
            /* [out] */ __RPC__deref_out_opt BSTR *pbstrFileName,
            /* [out] */ __RPC__out DWORD *dwStatus,
            /* [out] */ __RPC__out LONG *pnEventsRemaining);
        
        HRESULT ( STDMETHODCALLTYPE *RunScc )( 
            __RPC__in IVsSccEngine * This,
            /* [in] */ VSCOOKIE dwCookie,
            /* [in] */ __RPC__in HWND hWnd,
            /* [in] */ LONG cFiles,
            /* [size_is][in] */ __RPC__in_ecount_full(cFiles) const LPCOLESTR rgpszFileNames[  ],
            /* [size_is][in] */ __RPC__in_ecount_full(cFiles) const PAIR_IVSHIERARCHY_VSITEMID rgHierarchyItemIDPairs[  ]);
        
        HRESULT ( STDMETHODCALLTYPE *GetCommandOptions )( 
            __RPC__in IVsSccEngine * This,
            /* [in] */ VSCOOKIE dwCookie,
            /* [in] */ __RPC__in HWND hWnd,
            /* [in] */ SCCCOMMAND nCommand,
            /* [out][in] */ __RPC__inout DWORD *pdwOptions);
        
        HRESULT ( STDMETHODCALLTYPE *AddFromScc )( 
            __RPC__in IVsSccEngine * This,
            /* [in] */ VSCOOKIE dwCookie,
            /* [in] */ __RPC__in HWND hWnd,
            /* [out] */ __RPC__out CALPOLESTR *pCaFileNames);
        
        HRESULT ( STDMETHODCALLTYPE *SetOption )( 
            __RPC__in IVsSccEngine * This,
            /* [in] */ VSCOOKIE dwCookie,
            /* [in] */ LONG nOption,
            /* [in] */ DWORD dwVal);
        
        END_INTERFACE
    } IVsSccEngineVtbl;

    interface IVsSccEngine
    {
        CONST_VTBL struct IVsSccEngineVtbl *lpVtbl;
    };

    

#ifdef COBJMACROS


#define IVsSccEngine_QueryInterface(This,riid,ppvObject)	\
    ( (This)->lpVtbl -> QueryInterface(This,riid,ppvObject) ) 

#define IVsSccEngine_AddRef(This)	\
    ( (This)->lpVtbl -> AddRef(This) ) 

#define IVsSccEngine_Release(This)	\
    ( (This)->lpVtbl -> Release(This) ) 


#define IVsSccEngine_GetVersion(This,pnVersion)	\
    ( (This)->lpVtbl -> GetVersion(This,pnVersion) ) 

#define IVsSccEngine_Initialize(This,pdwCookie,hWnd,pszCallerName,pbstrSccName,pdwCapabilityBits,pdwVSCapabilityBits,pbstrAuxPathLabel,pnCheckoutCommentLen,pnCommentLen)	\
    ( (This)->lpVtbl -> Initialize(This,pdwCookie,hWnd,pszCallerName,pbstrSccName,pdwCapabilityBits,pdwVSCapabilityBits,pbstrAuxPathLabel,pnCheckoutCommentLen,pnCommentLen) ) 

#define IVsSccEngine_Uninitialize(This,cookie)	\
    ( (This)->lpVtbl -> Uninitialize(This,cookie) ) 

#define IVsSccEngine_OpenProject(This,dwCookie,hWnd,pbstrUser,pbstrProjName,pszLocalProjPath,pbstrAuxProjPath,pszComment,dwFlags)	\
    ( (This)->lpVtbl -> OpenProject(This,dwCookie,hWnd,pbstrUser,pbstrProjName,pszLocalProjPath,pbstrAuxProjPath,pszComment,dwFlags) ) 

#define IVsSccEngine_CloseProject(This,dwCookie)	\
    ( (This)->lpVtbl -> CloseProject(This,dwCookie) ) 

#define IVsSccEngine_GetProjPath(This,dwCookie,hWnd,pbstrUser,pbstrProjName,pbstrLocalPath,pbstrAuxProjPath,bAllowChangePath,pbInNew)	\
    ( (This)->lpVtbl -> GetProjPath(This,dwCookie,hWnd,pbstrUser,pbstrProjName,pbstrLocalPath,pbstrAuxProjPath,bAllowChangePath,pbInNew) ) 

#define IVsSccEngine_Get(This,dwCookie,hWnd,cFiles,rgpszFileNames,rgHierarchyItemIDPairs,dwFlags,dwOptions)	\
    ( (This)->lpVtbl -> Get(This,dwCookie,hWnd,cFiles,rgpszFileNames,rgHierarchyItemIDPairs,dwFlags,dwOptions) ) 

#define IVsSccEngine_Checkout(This,dwCookie,hWnd,cFiles,rgpszFileNames,rgHierarchyItemIDPairs,pszComment,dwFlags,dwOptions)	\
    ( (This)->lpVtbl -> Checkout(This,dwCookie,hWnd,cFiles,rgpszFileNames,rgHierarchyItemIDPairs,pszComment,dwFlags,dwOptions) ) 

#define IVsSccEngine_Uncheckout(This,dwCookie,hWnd,cFiles,rgpszFileNames,rgHierarchyItemIDPairs,dwFlags,dwOptions)	\
    ( (This)->lpVtbl -> Uncheckout(This,dwCookie,hWnd,cFiles,rgpszFileNames,rgHierarchyItemIDPairs,dwFlags,dwOptions) ) 

#define IVsSccEngine_Checkin(This,dwCookie,hWnd,cFiles,rgpszFileNames,rgHierarchyItemIDPairs,pszComment,dwFlags,dwOptions)	\
    ( (This)->lpVtbl -> Checkin(This,dwCookie,hWnd,cFiles,rgpszFileNames,rgHierarchyItemIDPairs,pszComment,dwFlags,dwOptions) ) 

#define IVsSccEngine_Add(This,dwCookie,hWnd,cFiles,rgpszFileNames,rgHierarchyItemIDPairs,pszComment,rgdwFlags,dwOptions)	\
    ( (This)->lpVtbl -> Add(This,dwCookie,hWnd,cFiles,rgpszFileNames,rgHierarchyItemIDPairs,pszComment,rgdwFlags,dwOptions) ) 

#define IVsSccEngine_Remove(This,dwCookie,hWnd,cFiles,rgpszFileNames,rgHierarchyItemIDPairs,pszComment,dwFlags,dwOptions)	\
    ( (This)->lpVtbl -> Remove(This,dwCookie,hWnd,cFiles,rgpszFileNames,rgHierarchyItemIDPairs,pszComment,dwFlags,dwOptions) ) 

#define IVsSccEngine_Rename(This,dwCookie,hWnd,pszFrom,pszTo)	\
    ( (This)->lpVtbl -> Rename(This,dwCookie,hWnd,pszFrom,pszTo) ) 

#define IVsSccEngine_Diff(This,dwCookie,hWnd,pszFilename,pHierarchyItemIDPair,dwFlags,dwOptions)	\
    ( (This)->lpVtbl -> Diff(This,dwCookie,hWnd,pszFilename,pHierarchyItemIDPair,dwFlags,dwOptions) ) 

#define IVsSccEngine_History(This,dwCookie,hWnd,cFiles,rgpszFileNames,rgHierarchyItemIDPairs,dwFlags,dwOptions)	\
    ( (This)->lpVtbl -> History(This,dwCookie,hWnd,cFiles,rgpszFileNames,rgHierarchyItemIDPairs,dwFlags,dwOptions) ) 

#define IVsSccEngine_Properties(This,dwCookie,hWnd,pszFileName,pHierarchyItemIDPair)	\
    ( (This)->lpVtbl -> Properties(This,dwCookie,hWnd,pszFileName,pHierarchyItemIDPair) ) 

#define IVsSccEngine_QueryInfo(This,dwCookie,cFiles,rgpszFileNames,rgHierarchyItemIDPairs,rgStatus)	\
    ( (This)->lpVtbl -> QueryInfo(This,dwCookie,cFiles,rgpszFileNames,rgHierarchyItemIDPairs,rgStatus) ) 

#define IVsSccEngine_PopulateList(This,dwCookie,nCommand,cFiles,rgpszFileNames,rgHierarchyItemIDPairs,pfnPopulate,rgStatus,dwFlags)	\
    ( (This)->lpVtbl -> PopulateList(This,dwCookie,nCommand,cFiles,rgpszFileNames,rgHierarchyItemIDPairs,pfnPopulate,rgStatus,dwFlags) ) 

#define IVsSccEngine_GetEvents(This,dwCookie,pbstrFileName,dwStatus,pnEventsRemaining)	\
    ( (This)->lpVtbl -> GetEvents(This,dwCookie,pbstrFileName,dwStatus,pnEventsRemaining) ) 

#define IVsSccEngine_RunScc(This,dwCookie,hWnd,cFiles,rgpszFileNames,rgHierarchyItemIDPairs)	\
    ( (This)->lpVtbl -> RunScc(This,dwCookie,hWnd,cFiles,rgpszFileNames,rgHierarchyItemIDPairs) ) 

#define IVsSccEngine_GetCommandOptions(This,dwCookie,hWnd,nCommand,pdwOptions)	\
    ( (This)->lpVtbl -> GetCommandOptions(This,dwCookie,hWnd,nCommand,pdwOptions) ) 

#define IVsSccEngine_AddFromScc(This,dwCookie,hWnd,pCaFileNames)	\
    ( (This)->lpVtbl -> AddFromScc(This,dwCookie,hWnd,pCaFileNames) ) 

#define IVsSccEngine_SetOption(This,dwCookie,nOption,dwVal)	\
    ( (This)->lpVtbl -> SetOption(This,dwCookie,nOption,dwVal) ) 

#endif /* COBJMACROS */


#endif 	/* C style interface */




#endif 	/* __IVsSccEngine_INTERFACE_DEFINED__ */


/* interface __MIDL_itf_vsscceng_0000_0002 */
/* [local] */ 

#if defined(_MSC_VER) || defined(__cplusplus) || defined(__STDC__) /* C or C++ */
static const HRESULT HRSCC_I_SHARESUBPROJOK = MAKE_HRESULT(SEVERITY_SUCCESS, FACILITY_ITF, 0x1000+(SCC_I_SHARESUBPROJOK));
static const HRESULT HRSCC_I_FILEDIFFERS = MAKE_HRESULT(SEVERITY_SUCCESS, FACILITY_ITF, 0x1000+(SCC_I_FILEDIFFERS));
static const HRESULT HRSCC_I_RELOADFILE = MAKE_HRESULT(SEVERITY_SUCCESS, FACILITY_ITF, 0x1000+(SCC_I_RELOADFILE));
static const HRESULT HRSCC_I_FILENOTAFFECTED = MAKE_HRESULT(SEVERITY_SUCCESS, FACILITY_ITF, 0x1000+(SCC_I_FILENOTAFFECTED));
static const HRESULT HRSCC_I_PROJECTCREATED = MAKE_HRESULT(SEVERITY_SUCCESS, FACILITY_ITF, 0x1000+(SCC_I_PROJECTCREATED));
static const HRESULT HRSCC_I_OPERATIONCANCELED = MAKE_HRESULT(SEVERITY_SUCCESS, FACILITY_ITF, 0x1000+(SCC_I_OPERATIONCANCELED));
static const HRESULT HRSCC_I_ADV_SUPPORT = MAKE_HRESULT(SEVERITY_SUCCESS, FACILITY_ITF, 0x1000+(SCC_I_ADV_SUPPORT));
static const HRESULT HRSCC_E_INITIALIZEFAILED = MAKE_HRESULT(SEVERITY_ERROR, FACILITY_ITF, 0x1000-(SCC_E_INITIALIZEFAILED));
static const HRESULT HRSCC_E_UNKNOWNPROJECT = MAKE_HRESULT(SEVERITY_ERROR, FACILITY_ITF, 0x1000-(SCC_E_UNKNOWNPROJECT));
static const HRESULT HRSCC_E_COULDNOTCREATEPROJECT = MAKE_HRESULT(SEVERITY_ERROR, FACILITY_ITF, 0x1000-(SCC_E_COULDNOTCREATEPROJECT));
static const HRESULT HRSCC_E_NOTCHECKEDOUT = MAKE_HRESULT(SEVERITY_ERROR, FACILITY_ITF, 0x1000-(SCC_E_NOTCHECKEDOUT));
static const HRESULT HRSCC_E_ALREADYCHECKEDOUT = MAKE_HRESULT(SEVERITY_ERROR, FACILITY_ITF, 0x1000-(SCC_E_ALREADYCHECKEDOUT));
static const HRESULT HRSCC_E_FILEISLOCKED = MAKE_HRESULT(SEVERITY_ERROR, FACILITY_ITF, 0x1000-(SCC_E_FILEISLOCKED));
static const HRESULT HRSCC_E_FILEOUTEXCLUSIVE = MAKE_HRESULT(SEVERITY_ERROR, FACILITY_ITF, 0x1000-(SCC_E_FILEOUTEXCLUSIVE));
static const HRESULT HRSCC_E_ACCESSFAILURE = MAKE_HRESULT(SEVERITY_ERROR, FACILITY_ITF, 0x1000-(SCC_E_ACCESSFAILURE));
static const HRESULT HRSCC_E_CHECKINCONFLICT = MAKE_HRESULT(SEVERITY_ERROR, FACILITY_ITF, 0x1000-(SCC_E_CHECKINCONFLICT));
static const HRESULT HRSCC_E_FILEALREADYEXISTS = MAKE_HRESULT(SEVERITY_ERROR, FACILITY_ITF, 0x1000-(SCC_E_FILEALREADYEXISTS));
static const HRESULT HRSCC_E_FILENOTCONTROLLED = MAKE_HRESULT(SEVERITY_ERROR, FACILITY_ITF, 0x1000-(SCC_E_FILENOTCONTROLLED));
static const HRESULT HRSCC_E_FILEISCHECKEDOUT = MAKE_HRESULT(SEVERITY_ERROR, FACILITY_ITF, 0x1000-(SCC_E_FILEISCHECKEDOUT));
static const HRESULT HRSCC_E_NOSPECIFIEDVERSION = MAKE_HRESULT(SEVERITY_ERROR, FACILITY_ITF, 0x1000-(SCC_E_NOSPECIFIEDVERSION));
static const HRESULT HRSCC_E_OPNOTSUPPORTED = MAKE_HRESULT(SEVERITY_ERROR, FACILITY_ITF, 0x1000-(SCC_E_OPNOTSUPPORTED));
static const HRESULT HRSCC_E_NONSPECIFICERROR = MAKE_HRESULT(SEVERITY_ERROR, FACILITY_ITF, 0x1000-(SCC_E_NONSPECIFICERROR));
static const HRESULT HRSCC_E_OPNOTPERFORMED = MAKE_HRESULT(SEVERITY_ERROR, FACILITY_ITF, 0x1000-(SCC_E_OPNOTPERFORMED));
static const HRESULT HRSCC_E_TYPENOTSUPPORTED = MAKE_HRESULT(SEVERITY_ERROR, FACILITY_ITF, 0x1000-(SCC_E_TYPENOTSUPPORTED));
static const HRESULT HRSCC_E_VERIFYMERGE = MAKE_HRESULT(SEVERITY_ERROR, FACILITY_ITF, 0x1000-(SCC_E_VERIFYMERGE));
static const HRESULT HRSCC_E_FIXMERGE = MAKE_HRESULT(SEVERITY_ERROR, FACILITY_ITF, 0x1000-(SCC_E_FIXMERGE));
static const HRESULT HRSCC_E_SHELLFAILURE = MAKE_HRESULT(SEVERITY_ERROR, FACILITY_ITF, 0x1000-(SCC_E_SHELLFAILURE));
static const HRESULT HRSCC_E_INVALIDUSER = MAKE_HRESULT(SEVERITY_ERROR, FACILITY_ITF, 0x1000-(SCC_E_INVALIDUSER));
static const HRESULT HRSCC_E_PROJECTALREADYOPEN = MAKE_HRESULT(SEVERITY_ERROR, FACILITY_ITF, 0x1000-(SCC_E_PROJECTALREADYOPEN));
static const HRESULT HRSCC_E_PROJSYNTAXERR = MAKE_HRESULT(SEVERITY_ERROR, FACILITY_ITF, 0x1000-(SCC_E_PROJSYNTAXERR));
static const HRESULT HRSCC_E_INVALIDFILEPATH = MAKE_HRESULT(SEVERITY_ERROR, FACILITY_ITF, 0x1000-(SCC_E_INVALIDFILEPATH));
static const HRESULT HRSCC_E_PROJNOTOPEN = MAKE_HRESULT(SEVERITY_ERROR, FACILITY_ITF, 0x1000-(SCC_E_PROJNOTOPEN));
static const HRESULT HRSCC_E_NOTAUTHORIZED = MAKE_HRESULT(SEVERITY_ERROR, FACILITY_ITF, 0x1000-(SCC_E_NOTAUTHORIZED));
static const HRESULT HRSCC_E_FILESYNTAXERR = MAKE_HRESULT(SEVERITY_ERROR, FACILITY_ITF, 0x1000-(SCC_E_FILESYNTAXERR));
static const HRESULT HRSCC_E_FILENOTEXIST = MAKE_HRESULT(SEVERITY_ERROR, FACILITY_ITF, 0x1000-(SCC_E_FILENOTEXIST));
static const HRESULT HRSCC_E_CONNECTIONFAILURE = MAKE_HRESULT(SEVERITY_ERROR, FACILITY_ITF, 0x1000-(SCC_E_CONNECTIONFAILURE));
static const HRESULT HRSCC_E_UNKNOWNERROR = MAKE_HRESULT(SEVERITY_ERROR, FACILITY_ITF, 0x1000-(SCC_E_UNKNOWNERROR));
static const HRESULT HRSCC_E_BACKGROUNDGETINPROGRESS = MAKE_HRESULT(SEVERITY_ERROR, FACILITY_ITF, 0x1000-(SCC_E_BACKGROUNDGETINPROGRESS));
static const HRESULT HRSCC_E_W2A_CONVERSION_FILES	= MAKE_HRESULT(SEVERITY_ERROR,	FACILITY_ITF, 0x1500);
static const HRESULT HRSCC_I_W2A_CONVERSION_FILES	= MAKE_HRESULT(SEVERITY_SUCCESS,FACILITY_ITF, 0x1500);
static const HRESULT HRSCC_E_W2A_CONVERSION_FOLDERS	= MAKE_HRESULT(SEVERITY_ERROR,	FACILITY_ITF, 0x1501);
static const HRESULT HRSCC_I_W2A_CONVERSION_FOLDERS	= MAKE_HRESULT(SEVERITY_SUCCESS,FACILITY_ITF, 0x1501);
static const HRESULT HRSCC_E_W2A_CONVERSION_PROJECT	= MAKE_HRESULT(SEVERITY_ERROR,	FACILITY_ITF, 0x1502);
static const HRESULT HRSCC_E_W2A_CONVERSION_PROJECT_OPENFROMSCC = MAKE_HRESULT(SEVERITY_ERROR, FACILITY_ITF, 0x1503);
#endif


extern RPC_IF_HANDLE __MIDL_itf_vsscceng_0000_0002_v0_0_c_ifspec;
extern RPC_IF_HANDLE __MIDL_itf_vsscceng_0000_0002_v0_0_s_ifspec;

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


