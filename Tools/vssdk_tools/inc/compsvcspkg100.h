

/* this ALWAYS GENERATED file contains the definitions for the interfaces */


 /* File created by MIDL compiler version 7.00.0499 */
/* Compiler settings for compsvcspkg100.idl:
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

#ifndef __compsvcspkg100_h__
#define __compsvcspkg100_h__

#if defined(_MSC_VER) && (_MSC_VER >= 1020)
#pragma once
#endif

/* Forward Declarations */ 

#ifndef __IVsFrameworkMultiTargeting_FWD_DEFINED__
#define __IVsFrameworkMultiTargeting_FWD_DEFINED__
typedef interface IVsFrameworkMultiTargeting IVsFrameworkMultiTargeting;
#endif 	/* __IVsFrameworkMultiTargeting_FWD_DEFINED__ */


#ifndef __SVsFrameworkMultiTargeting_FWD_DEFINED__
#define __SVsFrameworkMultiTargeting_FWD_DEFINED__
typedef interface SVsFrameworkMultiTargeting SVsFrameworkMultiTargeting;
#endif 	/* __SVsFrameworkMultiTargeting_FWD_DEFINED__ */


#ifndef __IVsFrameworkRetargetingDlg_FWD_DEFINED__
#define __IVsFrameworkRetargetingDlg_FWD_DEFINED__
typedef interface IVsFrameworkRetargetingDlg IVsFrameworkRetargetingDlg;
#endif 	/* __IVsFrameworkRetargetingDlg_FWD_DEFINED__ */


#ifndef __SVsFrameworkRetargetingDlg_FWD_DEFINED__
#define __SVsFrameworkRetargetingDlg_FWD_DEFINED__
typedef interface SVsFrameworkRetargetingDlg SVsFrameworkRetargetingDlg;
#endif 	/* __SVsFrameworkRetargetingDlg_FWD_DEFINED__ */


#ifndef __IVsComponentEnumeratorFactory4_FWD_DEFINED__
#define __IVsComponentEnumeratorFactory4_FWD_DEFINED__
typedef interface IVsComponentEnumeratorFactory4 IVsComponentEnumeratorFactory4;
#endif 	/* __IVsComponentEnumeratorFactory4_FWD_DEFINED__ */


/* header files for imported files */
#include "oaidl.h"
#include "ocidl.h"
#include "vsshell90.h"
#include "vsshell100.h"
#include "compsvcspkg80.h"
#include "compsvcspkg90.h"

#ifdef __cplusplus
extern "C"{
#endif 


/* interface __MIDL_itf_compsvcspkg100_0000_0000 */
/* [local] */ 

typedef 
enum _tagVSFRAMEWORKASSEMBLYTYPE
    {	VSFRAMEWORKASSEMBLYTYPE_FRAMEWORK	= 0x1,
	VSFRAMEWORKASSEMBLYTYPE_EXTENSIONS	= 0x2,
	VSFRAMEWORKASSEMBLYTYPE_ALL	= 0x8000
    } 	__VSFRAMEWORKASSEMBLYTYPE;

typedef DWORD VSFRAMEWORKASSEMBLYTYPE;

typedef 
enum _tagVSFRAMEWORKCOMPATIBILITY
    {	VSFRAMEWORKCOMPATIBILITY_COMPATIBLE	= 0,
	VSFRAMEWORKCOMPATIBILITY_INCOMPATIBLEIDENTITY	= 0x1,
	VSFRAMEWORKCOMPATIBILITY_INCOMPATIBLEVERSION	= 0x2,
	VSFRAMEWORKCOMPATIBILITY_INCOMPATIBLEPROFILE	= 0x4
    } 	__VSFRAMEWORKCOMPATIBILITY;

typedef DWORD VSFRAMEWORKCOMPATIBILITY;



extern RPC_IF_HANDLE __MIDL_itf_compsvcspkg100_0000_0000_v0_0_c_ifspec;
extern RPC_IF_HANDLE __MIDL_itf_compsvcspkg100_0000_0000_v0_0_s_ifspec;

#ifndef __IVsFrameworkMultiTargeting_INTERFACE_DEFINED__
#define __IVsFrameworkMultiTargeting_INTERFACE_DEFINED__

/* interface IVsFrameworkMultiTargeting */
/* [object][unique][helpstring][uuid] */ 


EXTERN_C const IID IID_IVsFrameworkMultiTargeting;

#if defined(__cplusplus) && !defined(CINTERFACE)
    
    MIDL_INTERFACE("B096B75C-5DF5-42c6-888F-A007CCEB6635")
    IVsFrameworkMultiTargeting : public IUnknown
    {
    public:
        virtual HRESULT STDMETHODCALLTYPE IsReferenceableInTargetFx( 
            /* [in] */ __RPC__in LPCWSTR pwszAssemblySpec,
            /* [in] */ __RPC__in LPCWSTR pwszTargetFrameworkMoniker,
            /* [retval][out] */ __RPC__out VARIANT_BOOL *pbIsReferenceable) = 0;
        
        virtual HRESULT STDMETHODCALLTYPE GetTargetFramework( 
            /* [in] */ __RPC__in LPCWSTR pwszAssemblyPath,
            /* [in] */ __RPC__in LPCWSTR pwszTargetFrameworkIdentifier,
            /* [retval][out] */ __RPC__deref_out_opt BSTR *pbstrTargetFrameworkMoniker) = 0;
        
        virtual HRESULT STDMETHODCALLTYPE GetSupportedFrameworks( 
            /* [retval][out] */ __RPC__deref_out_opt SAFEARRAY * *prgSupportedFrameworks) = 0;
        
        virtual HRESULT STDMETHODCALLTYPE GetFrameworkAssemblies( 
            /* [in] */ __RPC__in LPCWSTR pwszTargetFrameworkMoniker,
            /* [in] */ VSFRAMEWORKASSEMBLYTYPE atAssemblyType,
            /* [retval][out] */ __RPC__deref_out_opt SAFEARRAY * *prgAssemblyPaths) = 0;
        
        virtual HRESULT STDMETHODCALLTYPE CheckFrameworkCompatibility( 
            /* [in] */ __RPC__in LPCWSTR pwszTargetFrameworkMonikerSource,
            /* [in] */ __RPC__in LPCWSTR pwszTargetFrameworkMonikerTarget,
            /* [retval][out] */ __RPC__out VSFRAMEWORKCOMPATIBILITY *pdwCompat) = 0;
        
        virtual HRESULT STDMETHODCALLTYPE ResolveAssemblyPath( 
            /* [in] */ __RPC__in LPCWSTR pwszAssemblySpec,
            /* [in] */ __RPC__in LPCWSTR pwszTargetFrameworkMoniker,
            /* [retval][out] */ __RPC__deref_out_opt BSTR *pbstrResolvedAssemblyPath) = 0;
        
        virtual HRESULT STDMETHODCALLTYPE GetDisplayNameForTargetFx( 
            /* [in] */ __RPC__in LPCWSTR pwszTargetFrameworkMoniker,
            /* [retval][out] */ __RPC__deref_out_opt BSTR *pbstrDisplayName) = 0;
        
        virtual HRESULT STDMETHODCALLTYPE ResolveAssemblyPathsInTargetFx( 
            /* [in] */ __RPC__in LPCWSTR pwszTargetFrameworkMoniker,
            /* [size_is][in] */ __RPC__in_ecount_full(cAssembliesToResolve) SAFEARRAY * prgAssemblySpecs,
            /* [in] */ ULONG cAssembliesToResolve,
            /* [size_is][out][in] */ __RPC__inout_ecount_full(cAssembliesToResolve) PVsResolvedAssemblyPath prgResolvedAssemblyPaths,
            /* [out] */ __RPC__out ULONG *pcResolvedAssemblyPaths) = 0;
        
        virtual HRESULT STDMETHODCALLTYPE GetInstallableFrameworkForTargetFx( 
            /* [in] */ __RPC__in LPCWSTR pwszTargetFrameworkMoniker,
            /* [retval][out] */ __RPC__deref_out_opt BSTR *pbstrInstallableFrameworkMoniker) = 0;
        
    };
    
#else 	/* C style interface */

    typedef struct IVsFrameworkMultiTargetingVtbl
    {
        BEGIN_INTERFACE
        
        HRESULT ( STDMETHODCALLTYPE *QueryInterface )( 
            IVsFrameworkMultiTargeting * This,
            /* [in] */ __RPC__in REFIID riid,
            /* [iid_is][out] */ 
            __RPC__deref_out  void **ppvObject);
        
        ULONG ( STDMETHODCALLTYPE *AddRef )( 
            IVsFrameworkMultiTargeting * This);
        
        ULONG ( STDMETHODCALLTYPE *Release )( 
            IVsFrameworkMultiTargeting * This);
        
        HRESULT ( STDMETHODCALLTYPE *IsReferenceableInTargetFx )( 
            IVsFrameworkMultiTargeting * This,
            /* [in] */ __RPC__in LPCWSTR pwszAssemblySpec,
            /* [in] */ __RPC__in LPCWSTR pwszTargetFrameworkMoniker,
            /* [retval][out] */ __RPC__out VARIANT_BOOL *pbIsReferenceable);
        
        HRESULT ( STDMETHODCALLTYPE *GetTargetFramework )( 
            IVsFrameworkMultiTargeting * This,
            /* [in] */ __RPC__in LPCWSTR pwszAssemblyPath,
            /* [in] */ __RPC__in LPCWSTR pwszTargetFrameworkIdentifier,
            /* [retval][out] */ __RPC__deref_out_opt BSTR *pbstrTargetFrameworkMoniker);
        
        HRESULT ( STDMETHODCALLTYPE *GetSupportedFrameworks )( 
            IVsFrameworkMultiTargeting * This,
            /* [retval][out] */ __RPC__deref_out_opt SAFEARRAY * *prgSupportedFrameworks);
        
        HRESULT ( STDMETHODCALLTYPE *GetFrameworkAssemblies )( 
            IVsFrameworkMultiTargeting * This,
            /* [in] */ __RPC__in LPCWSTR pwszTargetFrameworkMoniker,
            /* [in] */ VSFRAMEWORKASSEMBLYTYPE atAssemblyType,
            /* [retval][out] */ __RPC__deref_out_opt SAFEARRAY * *prgAssemblyPaths);
        
        HRESULT ( STDMETHODCALLTYPE *CheckFrameworkCompatibility )( 
            IVsFrameworkMultiTargeting * This,
            /* [in] */ __RPC__in LPCWSTR pwszTargetFrameworkMonikerSource,
            /* [in] */ __RPC__in LPCWSTR pwszTargetFrameworkMonikerTarget,
            /* [retval][out] */ __RPC__out VSFRAMEWORKCOMPATIBILITY *pdwCompat);
        
        HRESULT ( STDMETHODCALLTYPE *ResolveAssemblyPath )( 
            IVsFrameworkMultiTargeting * This,
            /* [in] */ __RPC__in LPCWSTR pwszAssemblySpec,
            /* [in] */ __RPC__in LPCWSTR pwszTargetFrameworkMoniker,
            /* [retval][out] */ __RPC__deref_out_opt BSTR *pbstrResolvedAssemblyPath);
        
        HRESULT ( STDMETHODCALLTYPE *GetDisplayNameForTargetFx )( 
            IVsFrameworkMultiTargeting * This,
            /* [in] */ __RPC__in LPCWSTR pwszTargetFrameworkMoniker,
            /* [retval][out] */ __RPC__deref_out_opt BSTR *pbstrDisplayName);
        
        HRESULT ( STDMETHODCALLTYPE *ResolveAssemblyPathsInTargetFx )( 
            IVsFrameworkMultiTargeting * This,
            /* [in] */ __RPC__in LPCWSTR pwszTargetFrameworkMoniker,
            /* [size_is][in] */ __RPC__in_ecount_full(cAssembliesToResolve) SAFEARRAY * prgAssemblySpecs,
            /* [in] */ ULONG cAssembliesToResolve,
            /* [size_is][out][in] */ __RPC__inout_ecount_full(cAssembliesToResolve) PVsResolvedAssemblyPath prgResolvedAssemblyPaths,
            /* [out] */ __RPC__out ULONG *pcResolvedAssemblyPaths);
        
        HRESULT ( STDMETHODCALLTYPE *GetInstallableFrameworkForTargetFx )( 
            IVsFrameworkMultiTargeting * This,
            /* [in] */ __RPC__in LPCWSTR pwszTargetFrameworkMoniker,
            /* [retval][out] */ __RPC__deref_out_opt BSTR *pbstrInstallableFrameworkMoniker);
        
        END_INTERFACE
    } IVsFrameworkMultiTargetingVtbl;

    interface IVsFrameworkMultiTargeting
    {
        CONST_VTBL struct IVsFrameworkMultiTargetingVtbl *lpVtbl;
    };

    

#ifdef COBJMACROS


#define IVsFrameworkMultiTargeting_QueryInterface(This,riid,ppvObject)	\
    ( (This)->lpVtbl -> QueryInterface(This,riid,ppvObject) ) 

#define IVsFrameworkMultiTargeting_AddRef(This)	\
    ( (This)->lpVtbl -> AddRef(This) ) 

#define IVsFrameworkMultiTargeting_Release(This)	\
    ( (This)->lpVtbl -> Release(This) ) 


#define IVsFrameworkMultiTargeting_IsReferenceableInTargetFx(This,pwszAssemblySpec,pwszTargetFrameworkMoniker,pbIsReferenceable)	\
    ( (This)->lpVtbl -> IsReferenceableInTargetFx(This,pwszAssemblySpec,pwszTargetFrameworkMoniker,pbIsReferenceable) ) 

#define IVsFrameworkMultiTargeting_GetTargetFramework(This,pwszAssemblyPath,pwszTargetFrameworkIdentifier,pbstrTargetFrameworkMoniker)	\
    ( (This)->lpVtbl -> GetTargetFramework(This,pwszAssemblyPath,pwszTargetFrameworkIdentifier,pbstrTargetFrameworkMoniker) ) 

#define IVsFrameworkMultiTargeting_GetSupportedFrameworks(This,prgSupportedFrameworks)	\
    ( (This)->lpVtbl -> GetSupportedFrameworks(This,prgSupportedFrameworks) ) 

#define IVsFrameworkMultiTargeting_GetFrameworkAssemblies(This,pwszTargetFrameworkMoniker,atAssemblyType,prgAssemblyPaths)	\
    ( (This)->lpVtbl -> GetFrameworkAssemblies(This,pwszTargetFrameworkMoniker,atAssemblyType,prgAssemblyPaths) ) 

#define IVsFrameworkMultiTargeting_CheckFrameworkCompatibility(This,pwszTargetFrameworkMonikerSource,pwszTargetFrameworkMonikerTarget,pdwCompat)	\
    ( (This)->lpVtbl -> CheckFrameworkCompatibility(This,pwszTargetFrameworkMonikerSource,pwszTargetFrameworkMonikerTarget,pdwCompat) ) 

#define IVsFrameworkMultiTargeting_ResolveAssemblyPath(This,pwszAssemblySpec,pwszTargetFrameworkMoniker,pbstrResolvedAssemblyPath)	\
    ( (This)->lpVtbl -> ResolveAssemblyPath(This,pwszAssemblySpec,pwszTargetFrameworkMoniker,pbstrResolvedAssemblyPath) ) 

#define IVsFrameworkMultiTargeting_GetDisplayNameForTargetFx(This,pwszTargetFrameworkMoniker,pbstrDisplayName)	\
    ( (This)->lpVtbl -> GetDisplayNameForTargetFx(This,pwszTargetFrameworkMoniker,pbstrDisplayName) ) 

#define IVsFrameworkMultiTargeting_ResolveAssemblyPathsInTargetFx(This,pwszTargetFrameworkMoniker,prgAssemblySpecs,cAssembliesToResolve,prgResolvedAssemblyPaths,pcResolvedAssemblyPaths)	\
    ( (This)->lpVtbl -> ResolveAssemblyPathsInTargetFx(This,pwszTargetFrameworkMoniker,prgAssemblySpecs,cAssembliesToResolve,prgResolvedAssemblyPaths,pcResolvedAssemblyPaths) ) 

#define IVsFrameworkMultiTargeting_GetInstallableFrameworkForTargetFx(This,pwszTargetFrameworkMoniker,pbstrInstallableFrameworkMoniker)	\
    ( (This)->lpVtbl -> GetInstallableFrameworkForTargetFx(This,pwszTargetFrameworkMoniker,pbstrInstallableFrameworkMoniker) ) 

#endif /* COBJMACROS */


#endif 	/* C style interface */




#endif 	/* __IVsFrameworkMultiTargeting_INTERFACE_DEFINED__ */


#ifndef __SVsFrameworkMultiTargeting_INTERFACE_DEFINED__
#define __SVsFrameworkMultiTargeting_INTERFACE_DEFINED__

/* interface SVsFrameworkMultiTargeting */
/* [object][uuid] */ 


EXTERN_C const IID IID_SVsFrameworkMultiTargeting;

#if defined(__cplusplus) && !defined(CINTERFACE)
    
    MIDL_INTERFACE("6F209208-4D8F-4412-B125-CD839B055D52")
    SVsFrameworkMultiTargeting : public IUnknown
    {
    public:
    };
    
#else 	/* C style interface */

    typedef struct SVsFrameworkMultiTargetingVtbl
    {
        BEGIN_INTERFACE
        
        HRESULT ( STDMETHODCALLTYPE *QueryInterface )( 
            SVsFrameworkMultiTargeting * This,
            /* [in] */ __RPC__in REFIID riid,
            /* [iid_is][out] */ 
            __RPC__deref_out  void **ppvObject);
        
        ULONG ( STDMETHODCALLTYPE *AddRef )( 
            SVsFrameworkMultiTargeting * This);
        
        ULONG ( STDMETHODCALLTYPE *Release )( 
            SVsFrameworkMultiTargeting * This);
        
        END_INTERFACE
    } SVsFrameworkMultiTargetingVtbl;

    interface SVsFrameworkMultiTargeting
    {
        CONST_VTBL struct SVsFrameworkMultiTargetingVtbl *lpVtbl;
    };

    

#ifdef COBJMACROS


#define SVsFrameworkMultiTargeting_QueryInterface(This,riid,ppvObject)	\
    ( (This)->lpVtbl -> QueryInterface(This,riid,ppvObject) ) 

#define SVsFrameworkMultiTargeting_AddRef(This)	\
    ( (This)->lpVtbl -> AddRef(This) ) 

#define SVsFrameworkMultiTargeting_Release(This)	\
    ( (This)->lpVtbl -> Release(This) ) 


#endif /* COBJMACROS */


#endif 	/* C style interface */




#endif 	/* __SVsFrameworkMultiTargeting_INTERFACE_DEFINED__ */


/* interface __MIDL_itf_compsvcspkg100_0000_0002 */
/* [local] */ 

#define SID_SVsFrameworkMultiTargeting IID_SVsFrameworkMultiTargeting
typedef 
enum _tagFRD_FLAGS
    {	FRDF_SUPPORTS_RETARGETING	= 1,
	FRDF_DEFAULT	= FRDF_SUPPORTS_RETARGETING
    } 	__FRD_FLAGS;

typedef DWORD FRD_FLAGS;

typedef 
enum _tagFRD_OUTCOME
    {	FRDO_RETARGET_TO_40	= 1,
	FRDO_LEAVE_UNLOADED	= 2,
	FRDO_GOTO_DOWNLOAD_SITE	= 3
    } 	__FRD_OUTCOME;

typedef DWORD FRD_OUTCOME;



extern RPC_IF_HANDLE __MIDL_itf_compsvcspkg100_0000_0002_v0_0_c_ifspec;
extern RPC_IF_HANDLE __MIDL_itf_compsvcspkg100_0000_0002_v0_0_s_ifspec;

#ifndef __IVsFrameworkRetargetingDlg_INTERFACE_DEFINED__
#define __IVsFrameworkRetargetingDlg_INTERFACE_DEFINED__

/* interface IVsFrameworkRetargetingDlg */
/* [object][unique][helpstring][uuid] */ 


EXTERN_C const IID IID_IVsFrameworkRetargetingDlg;

#if defined(__cplusplus) && !defined(CINTERFACE)
    
    MIDL_INTERFACE("47f60934-4361-443a-9411-020bc2055608")
    IVsFrameworkRetargetingDlg : public IUnknown
    {
    public:
        virtual HRESULT STDMETHODCALLTYPE ShowFrameworkRetargetingDlg( 
            /* [in] */ __RPC__in LPCWSTR pszProjectType,
            /* [in] */ __RPC__in LPCWSTR pszProjectName,
            /* [in] */ __RPC__in LPCWSTR pszTargetedFrameworkMoniker,
            /* [in] */ FRD_FLAGS dwFlags,
            /* [out] */ __RPC__out FRD_OUTCOME *pdwOutcome,
            /* [out] */ __RPC__out BOOL *pbDontShowAgain) = 0;
        
        virtual HRESULT STDMETHODCALLTYPE NavigateToFrameworkDownloadUrl( void) = 0;
        
    };
    
#else 	/* C style interface */

    typedef struct IVsFrameworkRetargetingDlgVtbl
    {
        BEGIN_INTERFACE
        
        HRESULT ( STDMETHODCALLTYPE *QueryInterface )( 
            IVsFrameworkRetargetingDlg * This,
            /* [in] */ __RPC__in REFIID riid,
            /* [iid_is][out] */ 
            __RPC__deref_out  void **ppvObject);
        
        ULONG ( STDMETHODCALLTYPE *AddRef )( 
            IVsFrameworkRetargetingDlg * This);
        
        ULONG ( STDMETHODCALLTYPE *Release )( 
            IVsFrameworkRetargetingDlg * This);
        
        HRESULT ( STDMETHODCALLTYPE *ShowFrameworkRetargetingDlg )( 
            IVsFrameworkRetargetingDlg * This,
            /* [in] */ __RPC__in LPCWSTR pszProjectType,
            /* [in] */ __RPC__in LPCWSTR pszProjectName,
            /* [in] */ __RPC__in LPCWSTR pszTargetedFrameworkMoniker,
            /* [in] */ FRD_FLAGS dwFlags,
            /* [out] */ __RPC__out FRD_OUTCOME *pdwOutcome,
            /* [out] */ __RPC__out BOOL *pbDontShowAgain);
        
        HRESULT ( STDMETHODCALLTYPE *NavigateToFrameworkDownloadUrl )( 
            IVsFrameworkRetargetingDlg * This);
        
        END_INTERFACE
    } IVsFrameworkRetargetingDlgVtbl;

    interface IVsFrameworkRetargetingDlg
    {
        CONST_VTBL struct IVsFrameworkRetargetingDlgVtbl *lpVtbl;
    };

    

#ifdef COBJMACROS


#define IVsFrameworkRetargetingDlg_QueryInterface(This,riid,ppvObject)	\
    ( (This)->lpVtbl -> QueryInterface(This,riid,ppvObject) ) 

#define IVsFrameworkRetargetingDlg_AddRef(This)	\
    ( (This)->lpVtbl -> AddRef(This) ) 

#define IVsFrameworkRetargetingDlg_Release(This)	\
    ( (This)->lpVtbl -> Release(This) ) 


#define IVsFrameworkRetargetingDlg_ShowFrameworkRetargetingDlg(This,pszProjectType,pszProjectName,pszTargetedFrameworkMoniker,dwFlags,pdwOutcome,pbDontShowAgain)	\
    ( (This)->lpVtbl -> ShowFrameworkRetargetingDlg(This,pszProjectType,pszProjectName,pszTargetedFrameworkMoniker,dwFlags,pdwOutcome,pbDontShowAgain) ) 

#define IVsFrameworkRetargetingDlg_NavigateToFrameworkDownloadUrl(This)	\
    ( (This)->lpVtbl -> NavigateToFrameworkDownloadUrl(This) ) 

#endif /* COBJMACROS */


#endif 	/* C style interface */




#endif 	/* __IVsFrameworkRetargetingDlg_INTERFACE_DEFINED__ */


#ifndef __SVsFrameworkRetargetingDlg_INTERFACE_DEFINED__
#define __SVsFrameworkRetargetingDlg_INTERFACE_DEFINED__

/* interface SVsFrameworkRetargetingDlg */
/* [object][uuid] */ 


EXTERN_C const IID IID_SVsFrameworkRetargetingDlg;

#if defined(__cplusplus) && !defined(CINTERFACE)
    
    MIDL_INTERFACE("d4d51b8e-6ecf-4c42-a3e2-e0925e5115d6")
    SVsFrameworkRetargetingDlg : public IUnknown
    {
    public:
    };
    
#else 	/* C style interface */

    typedef struct SVsFrameworkRetargetingDlgVtbl
    {
        BEGIN_INTERFACE
        
        HRESULT ( STDMETHODCALLTYPE *QueryInterface )( 
            SVsFrameworkRetargetingDlg * This,
            /* [in] */ __RPC__in REFIID riid,
            /* [iid_is][out] */ 
            __RPC__deref_out  void **ppvObject);
        
        ULONG ( STDMETHODCALLTYPE *AddRef )( 
            SVsFrameworkRetargetingDlg * This);
        
        ULONG ( STDMETHODCALLTYPE *Release )( 
            SVsFrameworkRetargetingDlg * This);
        
        END_INTERFACE
    } SVsFrameworkRetargetingDlgVtbl;

    interface SVsFrameworkRetargetingDlg
    {
        CONST_VTBL struct SVsFrameworkRetargetingDlgVtbl *lpVtbl;
    };

    

#ifdef COBJMACROS


#define SVsFrameworkRetargetingDlg_QueryInterface(This,riid,ppvObject)	\
    ( (This)->lpVtbl -> QueryInterface(This,riid,ppvObject) ) 

#define SVsFrameworkRetargetingDlg_AddRef(This)	\
    ( (This)->lpVtbl -> AddRef(This) ) 

#define SVsFrameworkRetargetingDlg_Release(This)	\
    ( (This)->lpVtbl -> Release(This) ) 


#endif /* COBJMACROS */


#endif 	/* C style interface */




#endif 	/* __SVsFrameworkRetargetingDlg_INTERFACE_DEFINED__ */


/* interface __MIDL_itf_compsvcspkg100_0000_0004 */
/* [local] */ 

#define SID_SVsFrameworkRetargetingDlg IID_SVsFrameworkRetargetingDlg


extern RPC_IF_HANDLE __MIDL_itf_compsvcspkg100_0000_0004_v0_0_c_ifspec;
extern RPC_IF_HANDLE __MIDL_itf_compsvcspkg100_0000_0004_v0_0_s_ifspec;

#ifndef __IVsComponentEnumeratorFactory4_INTERFACE_DEFINED__
#define __IVsComponentEnumeratorFactory4_INTERFACE_DEFINED__

/* interface IVsComponentEnumeratorFactory4 */
/* [object][unique][helpstring][uuid] */ 


EXTERN_C const IID IID_IVsComponentEnumeratorFactory4;

#if defined(__cplusplus) && !defined(CINTERFACE)
    
    MIDL_INTERFACE("8F18FE7E-ACB7-4031-AAE5-039B49DF5191")
    IVsComponentEnumeratorFactory4 : public IUnknown
    {
    public:
        virtual HRESULT STDMETHODCALLTYPE GetReferencePathsForTargetFramework( 
            /* [in] */ __RPC__in LPCWSTR targetFrameworkMoniker,
            /* [out] */ __RPC__deref_out_opt IEnumComponents **ppEnumerator) = 0;
        
    };
    
#else 	/* C style interface */

    typedef struct IVsComponentEnumeratorFactory4Vtbl
    {
        BEGIN_INTERFACE
        
        HRESULT ( STDMETHODCALLTYPE *QueryInterface )( 
            IVsComponentEnumeratorFactory4 * This,
            /* [in] */ __RPC__in REFIID riid,
            /* [iid_is][out] */ 
            __RPC__deref_out  void **ppvObject);
        
        ULONG ( STDMETHODCALLTYPE *AddRef )( 
            IVsComponentEnumeratorFactory4 * This);
        
        ULONG ( STDMETHODCALLTYPE *Release )( 
            IVsComponentEnumeratorFactory4 * This);
        
        HRESULT ( STDMETHODCALLTYPE *GetReferencePathsForTargetFramework )( 
            IVsComponentEnumeratorFactory4 * This,
            /* [in] */ __RPC__in LPCWSTR targetFrameworkMoniker,
            /* [out] */ __RPC__deref_out_opt IEnumComponents **ppEnumerator);
        
        END_INTERFACE
    } IVsComponentEnumeratorFactory4Vtbl;

    interface IVsComponentEnumeratorFactory4
    {
        CONST_VTBL struct IVsComponentEnumeratorFactory4Vtbl *lpVtbl;
    };

    

#ifdef COBJMACROS


#define IVsComponentEnumeratorFactory4_QueryInterface(This,riid,ppvObject)	\
    ( (This)->lpVtbl -> QueryInterface(This,riid,ppvObject) ) 

#define IVsComponentEnumeratorFactory4_AddRef(This)	\
    ( (This)->lpVtbl -> AddRef(This) ) 

#define IVsComponentEnumeratorFactory4_Release(This)	\
    ( (This)->lpVtbl -> Release(This) ) 


#define IVsComponentEnumeratorFactory4_GetReferencePathsForTargetFramework(This,targetFrameworkMoniker,ppEnumerator)	\
    ( (This)->lpVtbl -> GetReferencePathsForTargetFramework(This,targetFrameworkMoniker,ppEnumerator) ) 

#endif /* COBJMACROS */


#endif 	/* C style interface */




#endif 	/* __IVsComponentEnumeratorFactory4_INTERFACE_DEFINED__ */


/* Additional Prototypes for ALL interfaces */

unsigned long             __RPC_USER  BSTR_UserSize(     unsigned long *, unsigned long            , BSTR * ); 
unsigned char * __RPC_USER  BSTR_UserMarshal(  unsigned long *, unsigned char *, BSTR * ); 
unsigned char * __RPC_USER  BSTR_UserUnmarshal(unsigned long *, unsigned char *, BSTR * ); 
void                      __RPC_USER  BSTR_UserFree(     unsigned long *, BSTR * ); 

unsigned long             __RPC_USER  LPSAFEARRAY_UserSize(     unsigned long *, unsigned long            , LPSAFEARRAY * ); 
unsigned char * __RPC_USER  LPSAFEARRAY_UserMarshal(  unsigned long *, unsigned char *, LPSAFEARRAY * ); 
unsigned char * __RPC_USER  LPSAFEARRAY_UserUnmarshal(unsigned long *, unsigned char *, LPSAFEARRAY * ); 
void                      __RPC_USER  LPSAFEARRAY_UserFree(     unsigned long *, LPSAFEARRAY * ); 

/* end of Additional Prototypes */

#ifdef __cplusplus
}
#endif

#endif


