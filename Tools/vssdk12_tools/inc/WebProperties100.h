

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


#ifndef __WebProperties100_h__
#define __WebProperties100_h__

#if defined(_MSC_VER) && (_MSC_VER >= 1020)
#pragma once
#endif

/* Forward Declarations */ 

#ifndef __WebSiteProperties4_FWD_DEFINED__
#define __WebSiteProperties4_FWD_DEFINED__
typedef interface WebSiteProperties4 WebSiteProperties4;

#endif 	/* __WebSiteProperties4_FWD_DEFINED__ */


#ifndef __WebSiteProperties5_FWD_DEFINED__
#define __WebSiteProperties5_FWD_DEFINED__
typedef interface WebSiteProperties5 WebSiteProperties5;

#endif 	/* __WebSiteProperties5_FWD_DEFINED__ */


/* header files for imported files */
#include "oaidl.h"

#ifdef __cplusplus
extern "C"{
#endif 


/* interface __MIDL_itf_WebProperties100_0000_0000 */
/* [local] */ 

#pragma once
#define VSWEBSITE100_VER_MAJ    10
#define VSWEBSITE100_VER_MIN    0


extern RPC_IF_HANDLE __MIDL_itf_WebProperties100_0000_0000_v0_0_c_ifspec;
extern RPC_IF_HANDLE __MIDL_itf_WebProperties100_0000_0000_v0_0_s_ifspec;


#ifndef __VSWebSite100_LIBRARY_DEFINED__
#define __VSWebSite100_LIBRARY_DEFINED__

/* library VSWebSite100 */
/* [version][helpstring][uuid] */ 


enum VSWebSitePropID100
    {
        WEBSITEPROPID_TargetFrameworkMoniker	= 3002,
        WEBSITEPROPID_CodeAnalysisRuleSet	= 3003,
        WEBSITEPROPID_CodeAnalysisRuleSetDirectories	= 3004,
        WEBSITEPROPID_CodeAnalysisIgnoreBuiltInRuleSets	= 3005,
        WEBSITEPROPID_CodeAnalysisRuleDirectories	= 3006,
        WEBSITEPROPID_CodeAnalysisIgnoreBuiltInRules	= 3007,
        WEBSITEPROPID_CodeAnalysisFailOnMissingRules	= 3008,
        WEBSITEPROPID_DevelopmentServerCommandLine	= 3009,
        WEBSITEPROPID_SecureUrl	= 3010,
        WEBSITEPROPID_NonSecureUrl	= 3011,
        WEBSITEPROPID_SSLEnabled	= 3012,
        WEBSITEPROPID_ManagedPipelineMode	= 3013,
        WEBSITEPROPID_AnonymousAuthenticationEnabled	= 3014,
        WEBSITEPROPID_WindowsAuthenticationEnabled	= 3015
    } ;
typedef 
enum _IISManagedPipelineMode
    {
        IISManagedPipelineMode_Integrated	= 0,
        IISManagedPipelineMode_ISAPI	= 1
    } 	IISManagedPipelineMode;


EXTERN_C const IID LIBID_VSWebSite100;

#ifndef __WebSiteProperties4_INTERFACE_DEFINED__
#define __WebSiteProperties4_INTERFACE_DEFINED__

/* interface WebSiteProperties4 */
/* [object][dual][unique][helpstring][uuid] */ 


EXTERN_C const IID IID_WebSiteProperties4;

#if defined(__cplusplus) && !defined(CINTERFACE)
    
    MIDL_INTERFACE("102301AB-2290-4067-B0AE-9CB1AF9C14B5")
    WebSiteProperties4 : public WebSiteProperties3
    {
    public:
        virtual /* [helpstring][id][nonbrowsable][propget] */ HRESULT STDMETHODCALLTYPE get_TargetFrameworkMoniker( 
            /* [retval][out] */ __RPC__deref_out_opt BSTR *pbstrTargetFrameworkMoniker) = 0;
        
        virtual /* [helpstring][id][nonbrowsable][propput] */ HRESULT STDMETHODCALLTYPE put_TargetFrameworkMoniker( 
            /* [in] */ __RPC__in BSTR bstrTargetFrameworkMoniker) = 0;
        
        virtual /* [helpstring][id][propget] */ HRESULT STDMETHODCALLTYPE get_CodeAnalysisRuleSet( 
            /* [retval][out] */ __RPC__deref_out_opt BSTR *pbstrRuleSet) = 0;
        
        virtual /* [helpstring][id][propput] */ HRESULT STDMETHODCALLTYPE put_CodeAnalysisRuleSet( 
            /* [in] */ __RPC__in BSTR bstrRuleSet) = 0;
        
        virtual /* [helpstring][id][propget] */ HRESULT STDMETHODCALLTYPE get_CodeAnalysisRuleSetDirectories( 
            /* [retval][out] */ __RPC__deref_out_opt BSTR *pbstrRuleSetDirectories) = 0;
        
        virtual /* [helpstring][id][propput] */ HRESULT STDMETHODCALLTYPE put_CodeAnalysisRuleSetDirectories( 
            /* [in] */ __RPC__in BSTR bstrRuleSetDirectories) = 0;
        
        virtual /* [helpstring][id][propget] */ HRESULT STDMETHODCALLTYPE get_CodeAnalysisIgnoreBuiltInRuleSets( 
            /* [retval][out] */ __RPC__out VARIANT_BOOL *pbIgnoreBuiltInRuleSets) = 0;
        
        virtual /* [helpstring][id][propput] */ HRESULT STDMETHODCALLTYPE put_CodeAnalysisIgnoreBuiltInRuleSets( 
            /* [in] */ VARIANT_BOOL IgnoreBuiltInRuleSets) = 0;
        
        virtual /* [helpstring][id][propget] */ HRESULT STDMETHODCALLTYPE get_CodeAnalysisRuleDirectories( 
            /* [retval][out] */ __RPC__deref_out_opt BSTR *pbstrRuleDirectories) = 0;
        
        virtual /* [helpstring][id][propput] */ HRESULT STDMETHODCALLTYPE put_CodeAnalysisRuleDirectories( 
            /* [in] */ __RPC__in BSTR bstrRuleDirectories) = 0;
        
        virtual /* [helpstring][id][propget] */ HRESULT STDMETHODCALLTYPE get_CodeAnalysisIgnoreBuiltInRules( 
            /* [retval][out] */ __RPC__out VARIANT_BOOL *pbIgnoreBuiltInRules) = 0;
        
        virtual /* [helpstring][id][propput] */ HRESULT STDMETHODCALLTYPE put_CodeAnalysisIgnoreBuiltInRules( 
            /* [in] */ VARIANT_BOOL IgnoreBuiltInRules) = 0;
        
        virtual /* [helpstring][id][propget] */ HRESULT STDMETHODCALLTYPE get_CodeAnalysisFailOnMissingRules( 
            /* [retval][out] */ __RPC__out VARIANT_BOOL *pbFailOnMissingRules) = 0;
        
        virtual /* [helpstring][id][propput] */ HRESULT STDMETHODCALLTYPE put_CodeAnalysisFailOnMissingRules( 
            /* [in] */ VARIANT_BOOL FailOnMissingRules) = 0;
        
        virtual /* [helpstring][id][propget] */ HRESULT STDMETHODCALLTYPE get_DevelopmentServerCommandLine( 
            /* [retval][out] */ __RPC__deref_out_opt BSTR *pbstrCommandLine) = 0;
        
    };
    
    
#else 	/* C style interface */

    typedef struct WebSiteProperties4Vtbl
    {
        BEGIN_INTERFACE
        
        /* [id][restricted][funcdescattr] */ HRESULT ( STDMETHODCALLTYPE *QueryInterface )( 
            __RPC__in WebSiteProperties4 * This,
            /* [in][idldescattr] */ __RPC__in struct GUID *riid,
            /* [out][idldescattr] */ __RPC__deref_out_opt void **ppvObj,
            /* [retval][out] */ __RPC__out void *retval);
        
        /* [id][restricted][funcdescattr] */ HRESULT ( STDMETHODCALLTYPE *AddRef )( 
            __RPC__in WebSiteProperties4 * This,
            /* [retval][out] */ __RPC__out unsigned long *retval);
        
        /* [id][restricted][funcdescattr] */ HRESULT ( STDMETHODCALLTYPE *Release )( 
            __RPC__in WebSiteProperties4 * This,
            /* [retval][out] */ __RPC__out unsigned long *retval);
        
        /* [id][restricted][funcdescattr] */ HRESULT ( STDMETHODCALLTYPE *GetTypeInfoCount )( 
            __RPC__in WebSiteProperties4 * This,
            /* [out][idldescattr] */ __RPC__out unsigned UINT *pctinfo,
            /* [retval][out] */ __RPC__out void *retval);
        
        /* [id][restricted][funcdescattr] */ HRESULT ( STDMETHODCALLTYPE *GetTypeInfo )( 
            __RPC__in WebSiteProperties4 * This,
            /* [in][idldescattr] */ unsigned UINT itinfo,
            /* [in][idldescattr] */ unsigned long lcid,
            /* [out][idldescattr] */ __RPC__deref_out_opt void **pptinfo,
            /* [retval][out] */ __RPC__out void *retval);
        
        /* [id][restricted][funcdescattr] */ HRESULT ( STDMETHODCALLTYPE *GetIDsOfNames )( 
            __RPC__in WebSiteProperties4 * This,
            /* [in][idldescattr] */ __RPC__in struct GUID *riid,
            /* [in][idldescattr] */ __RPC__deref_in_opt signed char **rgszNames,
            /* [in][idldescattr] */ unsigned UINT cNames,
            /* [in][idldescattr] */ unsigned long lcid,
            /* [out][idldescattr] */ __RPC__out signed long *rgdispid,
            /* [retval][out] */ __RPC__out void *retval);
        
        /* [id][restricted][funcdescattr] */ HRESULT ( STDMETHODCALLTYPE *Invoke )( 
            __RPC__in WebSiteProperties4 * This,
            /* [in][idldescattr] */ signed long dispidMember,
            /* [in][idldescattr] */ __RPC__in struct GUID *riid,
            /* [in][idldescattr] */ unsigned long lcid,
            /* [in][idldescattr] */ unsigned short wFlags,
            /* [in][idldescattr] */ __RPC__in struct DISPPARAMS *pdispparams,
            /* [out][idldescattr] */ __RPC__out VARIANT *pvarResult,
            /* [out][idldescattr] */ __RPC__out struct EXCEPINFO *pexcepinfo,
            /* [out][idldescattr] */ __RPC__out unsigned UINT *puArgErr,
            /* [retval][out] */ __RPC__out void *retval);
        
        /* [id][propget][hidden][funcdescattr] */ HRESULT ( STDMETHODCALLTYPE *get___id )( 
            __RPC__in WebSiteProperties4 * This,
            /* [retval][out] */ __RPC__deref_out_opt BSTR *retval);
        
        /* [id][propget][funcdescattr] */ HRESULT ( STDMETHODCALLTYPE *get_OpenedURL )( 
            __RPC__in WebSiteProperties4 * This,
            /* [retval][out] */ __RPC__deref_out_opt BSTR *retval);
        
        /* [id][propget][funcdescattr] */ HRESULT ( STDMETHODCALLTYPE *get_FullPath )( 
            __RPC__in WebSiteProperties4 * This,
            /* [retval][out] */ __RPC__deref_out_opt BSTR *retval);
        
        /* [id][propget][funcdescattr] */ HRESULT ( STDMETHODCALLTYPE *get_BrowseURL )( 
            __RPC__in WebSiteProperties4 * This,
            /* [retval][out] */ __RPC__deref_out_opt BSTR *retval);
        
        /* [id][propput][funcdescattr] */ HRESULT ( STDMETHODCALLTYPE *put_BrowseURL )( 
            __RPC__in WebSiteProperties4 * This,
            /* [in][idldescattr] */ __RPC__in BSTR noname,
            /* [retval][out] */ __RPC__out void *retval);
        
        /* [id][propget][funcdescattr] */ HRESULT ( STDMETHODCALLTYPE *get_EnableVsWebServer )( 
            __RPC__in WebSiteProperties4 * This,
            /* [retval][out] */ __RPC__out BOOLEAN *retval);
        
        /* [id][propput][funcdescattr] */ HRESULT ( STDMETHODCALLTYPE *put_EnableVsWebServer )( 
            __RPC__in WebSiteProperties4 * This,
            /* [in][idldescattr] */ BOOLEAN noname,
            /* [retval][out] */ __RPC__out void *retval);
        
        /* [id][propget][funcdescattr] */ HRESULT ( STDMETHODCALLTYPE *get_WebSiteType )( 
            __RPC__in WebSiteProperties4 * This,
            /* [retval][out] */ __RPC__out enum webType *retval);
        
        /* [id][propget][funcdescattr] */ HRESULT ( STDMETHODCALLTYPE *get_CurrentWebsiteLanguage )( 
            __RPC__in WebSiteProperties4 * This,
            /* [retval][out] */ __RPC__deref_out_opt BSTR *retval);
        
        /* [id][propput][funcdescattr] */ HRESULT ( STDMETHODCALLTYPE *put_CurrentWebsiteLanguage )( 
            __RPC__in WebSiteProperties4 * This,
            /* [in][idldescattr] */ __RPC__in BSTR noname,
            /* [retval][out] */ __RPC__out void *retval);
        
        /* [id][propget][funcdescattr] */ HRESULT ( STDMETHODCALLTYPE *get_StartProgram )( 
            __RPC__in WebSiteProperties4 * This,
            /* [retval][out] */ __RPC__deref_out_opt BSTR *retval);
        
        /* [id][propput][funcdescattr] */ HRESULT ( STDMETHODCALLTYPE *put_StartProgram )( 
            __RPC__in WebSiteProperties4 * This,
            /* [in][idldescattr] */ __RPC__in BSTR noname,
            /* [retval][out] */ __RPC__out void *retval);
        
        /* [id][propget][funcdescattr] */ HRESULT ( STDMETHODCALLTYPE *get_StartWorkingDirectory )( 
            __RPC__in WebSiteProperties4 * This,
            /* [retval][out] */ __RPC__deref_out_opt BSTR *retval);
        
        /* [id][propput][funcdescattr] */ HRESULT ( STDMETHODCALLTYPE *put_StartWorkingDirectory )( 
            __RPC__in WebSiteProperties4 * This,
            /* [in][idldescattr] */ __RPC__in BSTR noname,
            /* [retval][out] */ __RPC__out void *retval);
        
        /* [id][propget][funcdescattr] */ HRESULT ( STDMETHODCALLTYPE *get_StartURL )( 
            __RPC__in WebSiteProperties4 * This,
            /* [retval][out] */ __RPC__deref_out_opt BSTR *retval);
        
        /* [id][propput][funcdescattr] */ HRESULT ( STDMETHODCALLTYPE *put_StartURL )( 
            __RPC__in WebSiteProperties4 * This,
            /* [in][idldescattr] */ __RPC__in BSTR noname,
            /* [retval][out] */ __RPC__out void *retval);
        
        /* [id][propget][funcdescattr] */ HRESULT ( STDMETHODCALLTYPE *get_StartPage )( 
            __RPC__in WebSiteProperties4 * This,
            /* [retval][out] */ __RPC__deref_out_opt BSTR *retval);
        
        /* [id][propput][funcdescattr] */ HRESULT ( STDMETHODCALLTYPE *put_StartPage )( 
            __RPC__in WebSiteProperties4 * This,
            /* [in][idldescattr] */ __RPC__in BSTR noname,
            /* [retval][out] */ __RPC__out void *retval);
        
        /* [id][propget][funcdescattr] */ HRESULT ( STDMETHODCALLTYPE *get_StartArguments )( 
            __RPC__in WebSiteProperties4 * This,
            /* [retval][out] */ __RPC__deref_out_opt BSTR *retval);
        
        /* [id][propput][funcdescattr] */ HRESULT ( STDMETHODCALLTYPE *put_StartArguments )( 
            __RPC__in WebSiteProperties4 * This,
            /* [in][idldescattr] */ __RPC__in BSTR noname,
            /* [retval][out] */ __RPC__out void *retval);
        
        /* [id][propget][funcdescattr] */ HRESULT ( STDMETHODCALLTYPE *get_EnableASPXDebugging )( 
            __RPC__in WebSiteProperties4 * This,
            /* [retval][out] */ __RPC__out BOOLEAN *retval);
        
        /* [id][propput][funcdescattr] */ HRESULT ( STDMETHODCALLTYPE *put_EnableASPXDebugging )( 
            __RPC__in WebSiteProperties4 * This,
            /* [in][idldescattr] */ BOOLEAN noname,
            /* [retval][out] */ __RPC__out void *retval);
        
        /* [id][propget][funcdescattr] */ HRESULT ( STDMETHODCALLTYPE *get_EnableUnmanagedDebugging )( 
            __RPC__in WebSiteProperties4 * This,
            /* [retval][out] */ __RPC__out BOOLEAN *retval);
        
        /* [id][propput][funcdescattr] */ HRESULT ( STDMETHODCALLTYPE *put_EnableUnmanagedDebugging )( 
            __RPC__in WebSiteProperties4 * This,
            /* [in][idldescattr] */ BOOLEAN noname,
            /* [retval][out] */ __RPC__out void *retval);
        
        /* [id][propget][funcdescattr] */ HRESULT ( STDMETHODCALLTYPE *get_StartAction )( 
            __RPC__in WebSiteProperties4 * This,
            /* [retval][out] */ __RPC__out enum webStartAction *retval);
        
        /* [id][propput][funcdescattr] */ HRESULT ( STDMETHODCALLTYPE *put_StartAction )( 
            __RPC__in WebSiteProperties4 * This,
            /* [in][idldescattr] */ enum webStartAction noname,
            /* [retval][out] */ __RPC__out void *retval);
        
        /* [id][propget][funcdescattr] */ HRESULT ( STDMETHODCALLTYPE *get_EnableSQLServerDebugging )( 
            __RPC__in WebSiteProperties4 * This,
            /* [retval][out] */ __RPC__out BOOLEAN *retval);
        
        /* [id][propput][funcdescattr] */ HRESULT ( STDMETHODCALLTYPE *put_EnableSQLServerDebugging )( 
            __RPC__in WebSiteProperties4 * This,
            /* [in][idldescattr] */ BOOLEAN noname,
            /* [retval][out] */ __RPC__out void *retval);
        
        /* [id][propget][funcdescattr] */ HRESULT ( STDMETHODCALLTYPE *get_Extender )( 
            __RPC__in WebSiteProperties4 * This,
            /* [in][idldescattr] */ __RPC__in BSTR ExtenderName,
            /* [retval][out] */ __RPC__deref_out_opt IDispatch **retval);
        
        /* [id][propget][funcdescattr] */ HRESULT ( STDMETHODCALLTYPE *get_ExtenderNames )( 
            __RPC__in WebSiteProperties4 * This,
            /* [retval][out] */ __RPC__out VARIANT *retval);
        
        /* [id][propget][funcdescattr] */ HRESULT ( STDMETHODCALLTYPE *get_ExtenderCATID )( 
            __RPC__in WebSiteProperties4 * This,
            /* [retval][out] */ __RPC__deref_out_opt BSTR *retval);
        
        /* [id][propget][funcdescattr] */ HRESULT ( STDMETHODCALLTYPE *get_EnableFxCop )( 
            __RPC__in WebSiteProperties4 * This,
            /* [retval][out] */ __RPC__out BOOLEAN *retval);
        
        /* [id][propput][funcdescattr] */ HRESULT ( STDMETHODCALLTYPE *put_EnableFxCop )( 
            __RPC__in WebSiteProperties4 * This,
            /* [in][idldescattr] */ BOOLEAN noname,
            /* [retval][out] */ __RPC__out void *retval);
        
        /* [id][propget][funcdescattr] */ HRESULT ( STDMETHODCALLTYPE *get_FxCopRuleAssemblies )( 
            __RPC__in WebSiteProperties4 * This,
            /* [retval][out] */ __RPC__deref_out_opt BSTR *retval);
        
        /* [id][propput][funcdescattr] */ HRESULT ( STDMETHODCALLTYPE *put_FxCopRuleAssemblies )( 
            __RPC__in WebSiteProperties4 * This,
            /* [in][idldescattr] */ __RPC__in BSTR noname,
            /* [retval][out] */ __RPC__out void *retval);
        
        /* [id][propget][funcdescattr] */ HRESULT ( STDMETHODCALLTYPE *get_FxCopRules )( 
            __RPC__in WebSiteProperties4 * This,
            /* [retval][out] */ __RPC__deref_out_opt BSTR *retval);
        
        /* [id][propput][funcdescattr] */ HRESULT ( STDMETHODCALLTYPE *put_FxCopRules )( 
            __RPC__in WebSiteProperties4 * This,
            /* [in][idldescattr] */ __RPC__in BSTR noname,
            /* [retval][out] */ __RPC__out void *retval);
        
        /* [id][propget][funcdescattr] */ HRESULT ( STDMETHODCALLTYPE *get_EnableVsWebServerDynamicPort )( 
            __RPC__in WebSiteProperties4 * This,
            /* [retval][out] */ __RPC__out BOOLEAN *retval);
        
        /* [id][propput][funcdescattr] */ HRESULT ( STDMETHODCALLTYPE *put_EnableVsWebServerDynamicPort )( 
            __RPC__in WebSiteProperties4 * This,
            /* [in][idldescattr] */ BOOLEAN noname,
            /* [retval][out] */ __RPC__out void *retval);
        
        /* [id][propget][funcdescattr] */ HRESULT ( STDMETHODCALLTYPE *get_VsWebServerPort )( 
            __RPC__in WebSiteProperties4 * This,
            /* [retval][out] */ __RPC__out unsigned short *retval);
        
        /* [id][propput][funcdescattr] */ HRESULT ( STDMETHODCALLTYPE *put_VsWebServerPort )( 
            __RPC__in WebSiteProperties4 * This,
            /* [in][idldescattr] */ unsigned short noname,
            /* [retval][out] */ __RPC__out void *retval);
        
        /* [id][propget][funcdescattr] */ HRESULT ( STDMETHODCALLTYPE *get_EnableNTLMAuthentication )( 
            __RPC__in WebSiteProperties4 * This,
            /* [retval][out] */ __RPC__out BOOLEAN *retval);
        
        /* [id][propput][funcdescattr] */ HRESULT ( STDMETHODCALLTYPE *put_EnableNTLMAuthentication )( 
            __RPC__in WebSiteProperties4 * This,
            /* [in][idldescattr] */ BOOLEAN noname,
            /* [retval][out] */ __RPC__out void *retval);
        
        /* [id][propget][funcdescattr] */ HRESULT ( STDMETHODCALLTYPE *get_ProjectDirty )( 
            __RPC__in WebSiteProperties4 * This,
            /* [retval][out] */ __RPC__out BOOLEAN *retval);
        
        /* [id][propget][funcdescattr] */ HRESULT ( STDMETHODCALLTYPE *get_VsWebServerVPath )( 
            __RPC__in WebSiteProperties4 * This,
            /* [retval][out] */ __RPC__deref_out_opt BSTR *retval);
        
        /* [id][propput][funcdescattr] */ HRESULT ( STDMETHODCALLTYPE *put_VsWebServerVPath )( 
            __RPC__in WebSiteProperties4 * This,
            /* [in][idldescattr] */ __RPC__in BSTR noname,
            /* [retval][out] */ __RPC__out void *retval);
        
        /* [id][propget][funcdescattr] */ HRESULT ( STDMETHODCALLTYPE *get_TargetFramework )( 
            __RPC__in WebSiteProperties4 * This,
            /* [retval][out] */ __RPC__out unsigned long *retval);
        
        /* [id][propput][funcdescattr] */ HRESULT ( STDMETHODCALLTYPE *put_TargetFramework )( 
            __RPC__in WebSiteProperties4 * This,
            /* [in][idldescattr] */ unsigned long noname,
            /* [retval][out] */ __RPC__out void *retval);
        
        /* [id][propget][funcdescattr] */ HRESULT ( STDMETHODCALLTYPE *get_StartWebServerOnDebug )( 
            __RPC__in WebSiteProperties4 * This,
            /* [retval][out] */ __RPC__out BOOLEAN *retval);
        
        /* [id][propput][funcdescattr] */ HRESULT ( STDMETHODCALLTYPE *put_StartWebServerOnDebug )( 
            __RPC__in WebSiteProperties4 * This,
            /* [in][idldescattr] */ BOOLEAN noname,
            /* [retval][out] */ __RPC__out void *retval);
        
        /* [helpstring][id][nonbrowsable][propget] */ HRESULT ( STDMETHODCALLTYPE *get_TargetFrameworkMoniker )( 
            __RPC__in WebSiteProperties4 * This,
            /* [retval][out] */ __RPC__deref_out_opt BSTR *pbstrTargetFrameworkMoniker);
        
        /* [helpstring][id][nonbrowsable][propput] */ HRESULT ( STDMETHODCALLTYPE *put_TargetFrameworkMoniker )( 
            __RPC__in WebSiteProperties4 * This,
            /* [in] */ __RPC__in BSTR bstrTargetFrameworkMoniker);
        
        /* [helpstring][id][propget] */ HRESULT ( STDMETHODCALLTYPE *get_CodeAnalysisRuleSet )( 
            __RPC__in WebSiteProperties4 * This,
            /* [retval][out] */ __RPC__deref_out_opt BSTR *pbstrRuleSet);
        
        /* [helpstring][id][propput] */ HRESULT ( STDMETHODCALLTYPE *put_CodeAnalysisRuleSet )( 
            __RPC__in WebSiteProperties4 * This,
            /* [in] */ __RPC__in BSTR bstrRuleSet);
        
        /* [helpstring][id][propget] */ HRESULT ( STDMETHODCALLTYPE *get_CodeAnalysisRuleSetDirectories )( 
            __RPC__in WebSiteProperties4 * This,
            /* [retval][out] */ __RPC__deref_out_opt BSTR *pbstrRuleSetDirectories);
        
        /* [helpstring][id][propput] */ HRESULT ( STDMETHODCALLTYPE *put_CodeAnalysisRuleSetDirectories )( 
            __RPC__in WebSiteProperties4 * This,
            /* [in] */ __RPC__in BSTR bstrRuleSetDirectories);
        
        /* [helpstring][id][propget] */ HRESULT ( STDMETHODCALLTYPE *get_CodeAnalysisIgnoreBuiltInRuleSets )( 
            __RPC__in WebSiteProperties4 * This,
            /* [retval][out] */ __RPC__out VARIANT_BOOL *pbIgnoreBuiltInRuleSets);
        
        /* [helpstring][id][propput] */ HRESULT ( STDMETHODCALLTYPE *put_CodeAnalysisIgnoreBuiltInRuleSets )( 
            __RPC__in WebSiteProperties4 * This,
            /* [in] */ VARIANT_BOOL IgnoreBuiltInRuleSets);
        
        /* [helpstring][id][propget] */ HRESULT ( STDMETHODCALLTYPE *get_CodeAnalysisRuleDirectories )( 
            __RPC__in WebSiteProperties4 * This,
            /* [retval][out] */ __RPC__deref_out_opt BSTR *pbstrRuleDirectories);
        
        /* [helpstring][id][propput] */ HRESULT ( STDMETHODCALLTYPE *put_CodeAnalysisRuleDirectories )( 
            __RPC__in WebSiteProperties4 * This,
            /* [in] */ __RPC__in BSTR bstrRuleDirectories);
        
        /* [helpstring][id][propget] */ HRESULT ( STDMETHODCALLTYPE *get_CodeAnalysisIgnoreBuiltInRules )( 
            __RPC__in WebSiteProperties4 * This,
            /* [retval][out] */ __RPC__out VARIANT_BOOL *pbIgnoreBuiltInRules);
        
        /* [helpstring][id][propput] */ HRESULT ( STDMETHODCALLTYPE *put_CodeAnalysisIgnoreBuiltInRules )( 
            __RPC__in WebSiteProperties4 * This,
            /* [in] */ VARIANT_BOOL IgnoreBuiltInRules);
        
        /* [helpstring][id][propget] */ HRESULT ( STDMETHODCALLTYPE *get_CodeAnalysisFailOnMissingRules )( 
            __RPC__in WebSiteProperties4 * This,
            /* [retval][out] */ __RPC__out VARIANT_BOOL *pbFailOnMissingRules);
        
        /* [helpstring][id][propput] */ HRESULT ( STDMETHODCALLTYPE *put_CodeAnalysisFailOnMissingRules )( 
            __RPC__in WebSiteProperties4 * This,
            /* [in] */ VARIANT_BOOL FailOnMissingRules);
        
        /* [helpstring][id][propget] */ HRESULT ( STDMETHODCALLTYPE *get_DevelopmentServerCommandLine )( 
            __RPC__in WebSiteProperties4 * This,
            /* [retval][out] */ __RPC__deref_out_opt BSTR *pbstrCommandLine);
        
        END_INTERFACE
    } WebSiteProperties4Vtbl;

    interface WebSiteProperties4
    {
        CONST_VTBL struct WebSiteProperties4Vtbl *lpVtbl;
    };

    

#ifdef COBJMACROS


#define WebSiteProperties4_QueryInterface(This,riid,ppvObj,retval)	\
    ( (This)->lpVtbl -> QueryInterface(This,riid,ppvObj,retval) ) 

#define WebSiteProperties4_AddRef(This,retval)	\
    ( (This)->lpVtbl -> AddRef(This,retval) ) 

#define WebSiteProperties4_Release(This,retval)	\
    ( (This)->lpVtbl -> Release(This,retval) ) 

#define WebSiteProperties4_GetTypeInfoCount(This,pctinfo,retval)	\
    ( (This)->lpVtbl -> GetTypeInfoCount(This,pctinfo,retval) ) 

#define WebSiteProperties4_GetTypeInfo(This,itinfo,lcid,pptinfo,retval)	\
    ( (This)->lpVtbl -> GetTypeInfo(This,itinfo,lcid,pptinfo,retval) ) 

#define WebSiteProperties4_GetIDsOfNames(This,riid,rgszNames,cNames,lcid,rgdispid,retval)	\
    ( (This)->lpVtbl -> GetIDsOfNames(This,riid,rgszNames,cNames,lcid,rgdispid,retval) ) 

#define WebSiteProperties4_Invoke(This,dispidMember,riid,lcid,wFlags,pdispparams,pvarResult,pexcepinfo,puArgErr,retval)	\
    ( (This)->lpVtbl -> Invoke(This,dispidMember,riid,lcid,wFlags,pdispparams,pvarResult,pexcepinfo,puArgErr,retval) ) 

#define WebSiteProperties4_get___id(This,retval)	\
    ( (This)->lpVtbl -> get___id(This,retval) ) 

#define WebSiteProperties4_get_OpenedURL(This,retval)	\
    ( (This)->lpVtbl -> get_OpenedURL(This,retval) ) 

#define WebSiteProperties4_get_FullPath(This,retval)	\
    ( (This)->lpVtbl -> get_FullPath(This,retval) ) 

#define WebSiteProperties4_get_BrowseURL(This,retval)	\
    ( (This)->lpVtbl -> get_BrowseURL(This,retval) ) 

#define WebSiteProperties4_put_BrowseURL(This,noname,retval)	\
    ( (This)->lpVtbl -> put_BrowseURL(This,noname,retval) ) 

#define WebSiteProperties4_get_EnableVsWebServer(This,retval)	\
    ( (This)->lpVtbl -> get_EnableVsWebServer(This,retval) ) 

#define WebSiteProperties4_put_EnableVsWebServer(This,noname,retval)	\
    ( (This)->lpVtbl -> put_EnableVsWebServer(This,noname,retval) ) 

#define WebSiteProperties4_get_WebSiteType(This,retval)	\
    ( (This)->lpVtbl -> get_WebSiteType(This,retval) ) 

#define WebSiteProperties4_get_CurrentWebsiteLanguage(This,retval)	\
    ( (This)->lpVtbl -> get_CurrentWebsiteLanguage(This,retval) ) 

#define WebSiteProperties4_put_CurrentWebsiteLanguage(This,noname,retval)	\
    ( (This)->lpVtbl -> put_CurrentWebsiteLanguage(This,noname,retval) ) 

#define WebSiteProperties4_get_StartProgram(This,retval)	\
    ( (This)->lpVtbl -> get_StartProgram(This,retval) ) 

#define WebSiteProperties4_put_StartProgram(This,noname,retval)	\
    ( (This)->lpVtbl -> put_StartProgram(This,noname,retval) ) 

#define WebSiteProperties4_get_StartWorkingDirectory(This,retval)	\
    ( (This)->lpVtbl -> get_StartWorkingDirectory(This,retval) ) 

#define WebSiteProperties4_put_StartWorkingDirectory(This,noname,retval)	\
    ( (This)->lpVtbl -> put_StartWorkingDirectory(This,noname,retval) ) 

#define WebSiteProperties4_get_StartURL(This,retval)	\
    ( (This)->lpVtbl -> get_StartURL(This,retval) ) 

#define WebSiteProperties4_put_StartURL(This,noname,retval)	\
    ( (This)->lpVtbl -> put_StartURL(This,noname,retval) ) 

#define WebSiteProperties4_get_StartPage(This,retval)	\
    ( (This)->lpVtbl -> get_StartPage(This,retval) ) 

#define WebSiteProperties4_put_StartPage(This,noname,retval)	\
    ( (This)->lpVtbl -> put_StartPage(This,noname,retval) ) 

#define WebSiteProperties4_get_StartArguments(This,retval)	\
    ( (This)->lpVtbl -> get_StartArguments(This,retval) ) 

#define WebSiteProperties4_put_StartArguments(This,noname,retval)	\
    ( (This)->lpVtbl -> put_StartArguments(This,noname,retval) ) 

#define WebSiteProperties4_get_EnableASPXDebugging(This,retval)	\
    ( (This)->lpVtbl -> get_EnableASPXDebugging(This,retval) ) 

#define WebSiteProperties4_put_EnableASPXDebugging(This,noname,retval)	\
    ( (This)->lpVtbl -> put_EnableASPXDebugging(This,noname,retval) ) 

#define WebSiteProperties4_get_EnableUnmanagedDebugging(This,retval)	\
    ( (This)->lpVtbl -> get_EnableUnmanagedDebugging(This,retval) ) 

#define WebSiteProperties4_put_EnableUnmanagedDebugging(This,noname,retval)	\
    ( (This)->lpVtbl -> put_EnableUnmanagedDebugging(This,noname,retval) ) 

#define WebSiteProperties4_get_StartAction(This,retval)	\
    ( (This)->lpVtbl -> get_StartAction(This,retval) ) 

#define WebSiteProperties4_put_StartAction(This,noname,retval)	\
    ( (This)->lpVtbl -> put_StartAction(This,noname,retval) ) 

#define WebSiteProperties4_get_EnableSQLServerDebugging(This,retval)	\
    ( (This)->lpVtbl -> get_EnableSQLServerDebugging(This,retval) ) 

#define WebSiteProperties4_put_EnableSQLServerDebugging(This,noname,retval)	\
    ( (This)->lpVtbl -> put_EnableSQLServerDebugging(This,noname,retval) ) 

#define WebSiteProperties4_get_Extender(This,ExtenderName,retval)	\
    ( (This)->lpVtbl -> get_Extender(This,ExtenderName,retval) ) 

#define WebSiteProperties4_get_ExtenderNames(This,retval)	\
    ( (This)->lpVtbl -> get_ExtenderNames(This,retval) ) 

#define WebSiteProperties4_get_ExtenderCATID(This,retval)	\
    ( (This)->lpVtbl -> get_ExtenderCATID(This,retval) ) 

#define WebSiteProperties4_get_EnableFxCop(This,retval)	\
    ( (This)->lpVtbl -> get_EnableFxCop(This,retval) ) 

#define WebSiteProperties4_put_EnableFxCop(This,noname,retval)	\
    ( (This)->lpVtbl -> put_EnableFxCop(This,noname,retval) ) 

#define WebSiteProperties4_get_FxCopRuleAssemblies(This,retval)	\
    ( (This)->lpVtbl -> get_FxCopRuleAssemblies(This,retval) ) 

#define WebSiteProperties4_put_FxCopRuleAssemblies(This,noname,retval)	\
    ( (This)->lpVtbl -> put_FxCopRuleAssemblies(This,noname,retval) ) 

#define WebSiteProperties4_get_FxCopRules(This,retval)	\
    ( (This)->lpVtbl -> get_FxCopRules(This,retval) ) 

#define WebSiteProperties4_put_FxCopRules(This,noname,retval)	\
    ( (This)->lpVtbl -> put_FxCopRules(This,noname,retval) ) 

#define WebSiteProperties4_get_EnableVsWebServerDynamicPort(This,retval)	\
    ( (This)->lpVtbl -> get_EnableVsWebServerDynamicPort(This,retval) ) 

#define WebSiteProperties4_put_EnableVsWebServerDynamicPort(This,noname,retval)	\
    ( (This)->lpVtbl -> put_EnableVsWebServerDynamicPort(This,noname,retval) ) 

#define WebSiteProperties4_get_VsWebServerPort(This,retval)	\
    ( (This)->lpVtbl -> get_VsWebServerPort(This,retval) ) 

#define WebSiteProperties4_put_VsWebServerPort(This,noname,retval)	\
    ( (This)->lpVtbl -> put_VsWebServerPort(This,noname,retval) ) 

#define WebSiteProperties4_get_EnableNTLMAuthentication(This,retval)	\
    ( (This)->lpVtbl -> get_EnableNTLMAuthentication(This,retval) ) 

#define WebSiteProperties4_put_EnableNTLMAuthentication(This,noname,retval)	\
    ( (This)->lpVtbl -> put_EnableNTLMAuthentication(This,noname,retval) ) 

#define WebSiteProperties4_get_ProjectDirty(This,retval)	\
    ( (This)->lpVtbl -> get_ProjectDirty(This,retval) ) 

#define WebSiteProperties4_get_VsWebServerVPath(This,retval)	\
    ( (This)->lpVtbl -> get_VsWebServerVPath(This,retval) ) 

#define WebSiteProperties4_put_VsWebServerVPath(This,noname,retval)	\
    ( (This)->lpVtbl -> put_VsWebServerVPath(This,noname,retval) ) 

#define WebSiteProperties4_get_TargetFramework(This,retval)	\
    ( (This)->lpVtbl -> get_TargetFramework(This,retval) ) 

#define WebSiteProperties4_put_TargetFramework(This,noname,retval)	\
    ( (This)->lpVtbl -> put_TargetFramework(This,noname,retval) ) 

#define WebSiteProperties4_get_StartWebServerOnDebug(This,retval)	\
    ( (This)->lpVtbl -> get_StartWebServerOnDebug(This,retval) ) 

#define WebSiteProperties4_put_StartWebServerOnDebug(This,noname,retval)	\
    ( (This)->lpVtbl -> put_StartWebServerOnDebug(This,noname,retval) ) 


#define WebSiteProperties4_get_TargetFrameworkMoniker(This,pbstrTargetFrameworkMoniker)	\
    ( (This)->lpVtbl -> get_TargetFrameworkMoniker(This,pbstrTargetFrameworkMoniker) ) 

#define WebSiteProperties4_put_TargetFrameworkMoniker(This,bstrTargetFrameworkMoniker)	\
    ( (This)->lpVtbl -> put_TargetFrameworkMoniker(This,bstrTargetFrameworkMoniker) ) 

#define WebSiteProperties4_get_CodeAnalysisRuleSet(This,pbstrRuleSet)	\
    ( (This)->lpVtbl -> get_CodeAnalysisRuleSet(This,pbstrRuleSet) ) 

#define WebSiteProperties4_put_CodeAnalysisRuleSet(This,bstrRuleSet)	\
    ( (This)->lpVtbl -> put_CodeAnalysisRuleSet(This,bstrRuleSet) ) 

#define WebSiteProperties4_get_CodeAnalysisRuleSetDirectories(This,pbstrRuleSetDirectories)	\
    ( (This)->lpVtbl -> get_CodeAnalysisRuleSetDirectories(This,pbstrRuleSetDirectories) ) 

#define WebSiteProperties4_put_CodeAnalysisRuleSetDirectories(This,bstrRuleSetDirectories)	\
    ( (This)->lpVtbl -> put_CodeAnalysisRuleSetDirectories(This,bstrRuleSetDirectories) ) 

#define WebSiteProperties4_get_CodeAnalysisIgnoreBuiltInRuleSets(This,pbIgnoreBuiltInRuleSets)	\
    ( (This)->lpVtbl -> get_CodeAnalysisIgnoreBuiltInRuleSets(This,pbIgnoreBuiltInRuleSets) ) 

#define WebSiteProperties4_put_CodeAnalysisIgnoreBuiltInRuleSets(This,IgnoreBuiltInRuleSets)	\
    ( (This)->lpVtbl -> put_CodeAnalysisIgnoreBuiltInRuleSets(This,IgnoreBuiltInRuleSets) ) 

#define WebSiteProperties4_get_CodeAnalysisRuleDirectories(This,pbstrRuleDirectories)	\
    ( (This)->lpVtbl -> get_CodeAnalysisRuleDirectories(This,pbstrRuleDirectories) ) 

#define WebSiteProperties4_put_CodeAnalysisRuleDirectories(This,bstrRuleDirectories)	\
    ( (This)->lpVtbl -> put_CodeAnalysisRuleDirectories(This,bstrRuleDirectories) ) 

#define WebSiteProperties4_get_CodeAnalysisIgnoreBuiltInRules(This,pbIgnoreBuiltInRules)	\
    ( (This)->lpVtbl -> get_CodeAnalysisIgnoreBuiltInRules(This,pbIgnoreBuiltInRules) ) 

#define WebSiteProperties4_put_CodeAnalysisIgnoreBuiltInRules(This,IgnoreBuiltInRules)	\
    ( (This)->lpVtbl -> put_CodeAnalysisIgnoreBuiltInRules(This,IgnoreBuiltInRules) ) 

#define WebSiteProperties4_get_CodeAnalysisFailOnMissingRules(This,pbFailOnMissingRules)	\
    ( (This)->lpVtbl -> get_CodeAnalysisFailOnMissingRules(This,pbFailOnMissingRules) ) 

#define WebSiteProperties4_put_CodeAnalysisFailOnMissingRules(This,FailOnMissingRules)	\
    ( (This)->lpVtbl -> put_CodeAnalysisFailOnMissingRules(This,FailOnMissingRules) ) 

#define WebSiteProperties4_get_DevelopmentServerCommandLine(This,pbstrCommandLine)	\
    ( (This)->lpVtbl -> get_DevelopmentServerCommandLine(This,pbstrCommandLine) ) 

#endif /* COBJMACROS */


#endif 	/* C style interface */




#endif 	/* __WebSiteProperties4_INTERFACE_DEFINED__ */


#ifndef __WebSiteProperties5_INTERFACE_DEFINED__
#define __WebSiteProperties5_INTERFACE_DEFINED__

/* interface WebSiteProperties5 */
/* [object][dual][unique][helpstring][uuid] */ 


EXTERN_C const IID IID_WebSiteProperties5;

#if defined(__cplusplus) && !defined(CINTERFACE)
    
    MIDL_INTERFACE("856AEFF3-563B-47f6-8F21-C8FA77583EBD")
    WebSiteProperties5 : public WebSiteProperties4
    {
    public:
        virtual /* [helpstring][id][propget] */ HRESULT STDMETHODCALLTYPE get_SecureUrl( 
            /* [retval][out] */ __RPC__deref_out_opt BSTR *pbstrSecureUrl) = 0;
        
        virtual /* [helpstring][id][propget] */ HRESULT STDMETHODCALLTYPE get_NonSecureUrl( 
            /* [retval][out] */ __RPC__deref_out_opt BSTR *pbstrNonSecureUrl) = 0;
        
        virtual /* [helpstring][id][propget] */ HRESULT STDMETHODCALLTYPE get_SSLEnabled( 
            /* [retval][out] */ __RPC__out VARIANT_BOOL *pbSSLEnabled) = 0;
        
        virtual /* [helpstring][id][propput] */ HRESULT STDMETHODCALLTYPE put_SSLEnabled( 
            /* [in] */ VARIANT_BOOL bEnableSSL) = 0;
        
        virtual /* [helpstring][id][propget] */ HRESULT STDMETHODCALLTYPE get_ManagedPipelineMode( 
            /* [retval][out] */ __RPC__out IISManagedPipelineMode *pPipeLineMode) = 0;
        
        virtual /* [helpstring][id][propput] */ HRESULT STDMETHODCALLTYPE put_ManagedPipelineMode( 
            /* [in] */ IISManagedPipelineMode pipeLineMode) = 0;
        
        virtual /* [helpstring][id][propget] */ HRESULT STDMETHODCALLTYPE get_AnonymousAuthenticationEnabled( 
            /* [retval][out] */ __RPC__out VARIANT_BOOL *pEnabled) = 0;
        
        virtual /* [helpstring][id][propput] */ HRESULT STDMETHODCALLTYPE put_AnonymousAuthenticationEnabled( 
            /* [in] */ VARIANT_BOOL bEnable) = 0;
        
        virtual /* [helpstring][id][propget] */ HRESULT STDMETHODCALLTYPE get_WindowsAuthenticationEnabled( 
            /* [retval][out] */ __RPC__out VARIANT_BOOL *pEnabled) = 0;
        
        virtual /* [helpstring][id][propput] */ HRESULT STDMETHODCALLTYPE put_WindowsAuthenticationEnabled( 
            /* [in] */ VARIANT_BOOL bEnable) = 0;
        
    };
    
    
#else 	/* C style interface */

    typedef struct WebSiteProperties5Vtbl
    {
        BEGIN_INTERFACE
        
        /* [id][restricted][funcdescattr] */ HRESULT ( STDMETHODCALLTYPE *QueryInterface )( 
            __RPC__in WebSiteProperties5 * This,
            /* [in][idldescattr] */ __RPC__in struct GUID *riid,
            /* [out][idldescattr] */ __RPC__deref_out_opt void **ppvObj,
            /* [retval][out] */ __RPC__out void *retval);
        
        /* [id][restricted][funcdescattr] */ HRESULT ( STDMETHODCALLTYPE *AddRef )( 
            __RPC__in WebSiteProperties5 * This,
            /* [retval][out] */ __RPC__out unsigned long *retval);
        
        /* [id][restricted][funcdescattr] */ HRESULT ( STDMETHODCALLTYPE *Release )( 
            __RPC__in WebSiteProperties5 * This,
            /* [retval][out] */ __RPC__out unsigned long *retval);
        
        /* [id][restricted][funcdescattr] */ HRESULT ( STDMETHODCALLTYPE *GetTypeInfoCount )( 
            __RPC__in WebSiteProperties5 * This,
            /* [out][idldescattr] */ __RPC__out unsigned UINT *pctinfo,
            /* [retval][out] */ __RPC__out void *retval);
        
        /* [id][restricted][funcdescattr] */ HRESULT ( STDMETHODCALLTYPE *GetTypeInfo )( 
            __RPC__in WebSiteProperties5 * This,
            /* [in][idldescattr] */ unsigned UINT itinfo,
            /* [in][idldescattr] */ unsigned long lcid,
            /* [out][idldescattr] */ __RPC__deref_out_opt void **pptinfo,
            /* [retval][out] */ __RPC__out void *retval);
        
        /* [id][restricted][funcdescattr] */ HRESULT ( STDMETHODCALLTYPE *GetIDsOfNames )( 
            __RPC__in WebSiteProperties5 * This,
            /* [in][idldescattr] */ __RPC__in struct GUID *riid,
            /* [in][idldescattr] */ __RPC__deref_in_opt signed char **rgszNames,
            /* [in][idldescattr] */ unsigned UINT cNames,
            /* [in][idldescattr] */ unsigned long lcid,
            /* [out][idldescattr] */ __RPC__out signed long *rgdispid,
            /* [retval][out] */ __RPC__out void *retval);
        
        /* [id][restricted][funcdescattr] */ HRESULT ( STDMETHODCALLTYPE *Invoke )( 
            __RPC__in WebSiteProperties5 * This,
            /* [in][idldescattr] */ signed long dispidMember,
            /* [in][idldescattr] */ __RPC__in struct GUID *riid,
            /* [in][idldescattr] */ unsigned long lcid,
            /* [in][idldescattr] */ unsigned short wFlags,
            /* [in][idldescattr] */ __RPC__in struct DISPPARAMS *pdispparams,
            /* [out][idldescattr] */ __RPC__out VARIANT *pvarResult,
            /* [out][idldescattr] */ __RPC__out struct EXCEPINFO *pexcepinfo,
            /* [out][idldescattr] */ __RPC__out unsigned UINT *puArgErr,
            /* [retval][out] */ __RPC__out void *retval);
        
        /* [id][propget][hidden][funcdescattr] */ HRESULT ( STDMETHODCALLTYPE *get___id )( 
            __RPC__in WebSiteProperties5 * This,
            /* [retval][out] */ __RPC__deref_out_opt BSTR *retval);
        
        /* [id][propget][funcdescattr] */ HRESULT ( STDMETHODCALLTYPE *get_OpenedURL )( 
            __RPC__in WebSiteProperties5 * This,
            /* [retval][out] */ __RPC__deref_out_opt BSTR *retval);
        
        /* [id][propget][funcdescattr] */ HRESULT ( STDMETHODCALLTYPE *get_FullPath )( 
            __RPC__in WebSiteProperties5 * This,
            /* [retval][out] */ __RPC__deref_out_opt BSTR *retval);
        
        /* [id][propget][funcdescattr] */ HRESULT ( STDMETHODCALLTYPE *get_BrowseURL )( 
            __RPC__in WebSiteProperties5 * This,
            /* [retval][out] */ __RPC__deref_out_opt BSTR *retval);
        
        /* [id][propput][funcdescattr] */ HRESULT ( STDMETHODCALLTYPE *put_BrowseURL )( 
            __RPC__in WebSiteProperties5 * This,
            /* [in][idldescattr] */ __RPC__in BSTR noname,
            /* [retval][out] */ __RPC__out void *retval);
        
        /* [id][propget][funcdescattr] */ HRESULT ( STDMETHODCALLTYPE *get_EnableVsWebServer )( 
            __RPC__in WebSiteProperties5 * This,
            /* [retval][out] */ __RPC__out BOOLEAN *retval);
        
        /* [id][propput][funcdescattr] */ HRESULT ( STDMETHODCALLTYPE *put_EnableVsWebServer )( 
            __RPC__in WebSiteProperties5 * This,
            /* [in][idldescattr] */ BOOLEAN noname,
            /* [retval][out] */ __RPC__out void *retval);
        
        /* [id][propget][funcdescattr] */ HRESULT ( STDMETHODCALLTYPE *get_WebSiteType )( 
            __RPC__in WebSiteProperties5 * This,
            /* [retval][out] */ __RPC__out enum webType *retval);
        
        /* [id][propget][funcdescattr] */ HRESULT ( STDMETHODCALLTYPE *get_CurrentWebsiteLanguage )( 
            __RPC__in WebSiteProperties5 * This,
            /* [retval][out] */ __RPC__deref_out_opt BSTR *retval);
        
        /* [id][propput][funcdescattr] */ HRESULT ( STDMETHODCALLTYPE *put_CurrentWebsiteLanguage )( 
            __RPC__in WebSiteProperties5 * This,
            /* [in][idldescattr] */ __RPC__in BSTR noname,
            /* [retval][out] */ __RPC__out void *retval);
        
        /* [id][propget][funcdescattr] */ HRESULT ( STDMETHODCALLTYPE *get_StartProgram )( 
            __RPC__in WebSiteProperties5 * This,
            /* [retval][out] */ __RPC__deref_out_opt BSTR *retval);
        
        /* [id][propput][funcdescattr] */ HRESULT ( STDMETHODCALLTYPE *put_StartProgram )( 
            __RPC__in WebSiteProperties5 * This,
            /* [in][idldescattr] */ __RPC__in BSTR noname,
            /* [retval][out] */ __RPC__out void *retval);
        
        /* [id][propget][funcdescattr] */ HRESULT ( STDMETHODCALLTYPE *get_StartWorkingDirectory )( 
            __RPC__in WebSiteProperties5 * This,
            /* [retval][out] */ __RPC__deref_out_opt BSTR *retval);
        
        /* [id][propput][funcdescattr] */ HRESULT ( STDMETHODCALLTYPE *put_StartWorkingDirectory )( 
            __RPC__in WebSiteProperties5 * This,
            /* [in][idldescattr] */ __RPC__in BSTR noname,
            /* [retval][out] */ __RPC__out void *retval);
        
        /* [id][propget][funcdescattr] */ HRESULT ( STDMETHODCALLTYPE *get_StartURL )( 
            __RPC__in WebSiteProperties5 * This,
            /* [retval][out] */ __RPC__deref_out_opt BSTR *retval);
        
        /* [id][propput][funcdescattr] */ HRESULT ( STDMETHODCALLTYPE *put_StartURL )( 
            __RPC__in WebSiteProperties5 * This,
            /* [in][idldescattr] */ __RPC__in BSTR noname,
            /* [retval][out] */ __RPC__out void *retval);
        
        /* [id][propget][funcdescattr] */ HRESULT ( STDMETHODCALLTYPE *get_StartPage )( 
            __RPC__in WebSiteProperties5 * This,
            /* [retval][out] */ __RPC__deref_out_opt BSTR *retval);
        
        /* [id][propput][funcdescattr] */ HRESULT ( STDMETHODCALLTYPE *put_StartPage )( 
            __RPC__in WebSiteProperties5 * This,
            /* [in][idldescattr] */ __RPC__in BSTR noname,
            /* [retval][out] */ __RPC__out void *retval);
        
        /* [id][propget][funcdescattr] */ HRESULT ( STDMETHODCALLTYPE *get_StartArguments )( 
            __RPC__in WebSiteProperties5 * This,
            /* [retval][out] */ __RPC__deref_out_opt BSTR *retval);
        
        /* [id][propput][funcdescattr] */ HRESULT ( STDMETHODCALLTYPE *put_StartArguments )( 
            __RPC__in WebSiteProperties5 * This,
            /* [in][idldescattr] */ __RPC__in BSTR noname,
            /* [retval][out] */ __RPC__out void *retval);
        
        /* [id][propget][funcdescattr] */ HRESULT ( STDMETHODCALLTYPE *get_EnableASPXDebugging )( 
            __RPC__in WebSiteProperties5 * This,
            /* [retval][out] */ __RPC__out BOOLEAN *retval);
        
        /* [id][propput][funcdescattr] */ HRESULT ( STDMETHODCALLTYPE *put_EnableASPXDebugging )( 
            __RPC__in WebSiteProperties5 * This,
            /* [in][idldescattr] */ BOOLEAN noname,
            /* [retval][out] */ __RPC__out void *retval);
        
        /* [id][propget][funcdescattr] */ HRESULT ( STDMETHODCALLTYPE *get_EnableUnmanagedDebugging )( 
            __RPC__in WebSiteProperties5 * This,
            /* [retval][out] */ __RPC__out BOOLEAN *retval);
        
        /* [id][propput][funcdescattr] */ HRESULT ( STDMETHODCALLTYPE *put_EnableUnmanagedDebugging )( 
            __RPC__in WebSiteProperties5 * This,
            /* [in][idldescattr] */ BOOLEAN noname,
            /* [retval][out] */ __RPC__out void *retval);
        
        /* [id][propget][funcdescattr] */ HRESULT ( STDMETHODCALLTYPE *get_StartAction )( 
            __RPC__in WebSiteProperties5 * This,
            /* [retval][out] */ __RPC__out enum webStartAction *retval);
        
        /* [id][propput][funcdescattr] */ HRESULT ( STDMETHODCALLTYPE *put_StartAction )( 
            __RPC__in WebSiteProperties5 * This,
            /* [in][idldescattr] */ enum webStartAction noname,
            /* [retval][out] */ __RPC__out void *retval);
        
        /* [id][propget][funcdescattr] */ HRESULT ( STDMETHODCALLTYPE *get_EnableSQLServerDebugging )( 
            __RPC__in WebSiteProperties5 * This,
            /* [retval][out] */ __RPC__out BOOLEAN *retval);
        
        /* [id][propput][funcdescattr] */ HRESULT ( STDMETHODCALLTYPE *put_EnableSQLServerDebugging )( 
            __RPC__in WebSiteProperties5 * This,
            /* [in][idldescattr] */ BOOLEAN noname,
            /* [retval][out] */ __RPC__out void *retval);
        
        /* [id][propget][funcdescattr] */ HRESULT ( STDMETHODCALLTYPE *get_Extender )( 
            __RPC__in WebSiteProperties5 * This,
            /* [in][idldescattr] */ __RPC__in BSTR ExtenderName,
            /* [retval][out] */ __RPC__deref_out_opt IDispatch **retval);
        
        /* [id][propget][funcdescattr] */ HRESULT ( STDMETHODCALLTYPE *get_ExtenderNames )( 
            __RPC__in WebSiteProperties5 * This,
            /* [retval][out] */ __RPC__out VARIANT *retval);
        
        /* [id][propget][funcdescattr] */ HRESULT ( STDMETHODCALLTYPE *get_ExtenderCATID )( 
            __RPC__in WebSiteProperties5 * This,
            /* [retval][out] */ __RPC__deref_out_opt BSTR *retval);
        
        /* [id][propget][funcdescattr] */ HRESULT ( STDMETHODCALLTYPE *get_EnableFxCop )( 
            __RPC__in WebSiteProperties5 * This,
            /* [retval][out] */ __RPC__out BOOLEAN *retval);
        
        /* [id][propput][funcdescattr] */ HRESULT ( STDMETHODCALLTYPE *put_EnableFxCop )( 
            __RPC__in WebSiteProperties5 * This,
            /* [in][idldescattr] */ BOOLEAN noname,
            /* [retval][out] */ __RPC__out void *retval);
        
        /* [id][propget][funcdescattr] */ HRESULT ( STDMETHODCALLTYPE *get_FxCopRuleAssemblies )( 
            __RPC__in WebSiteProperties5 * This,
            /* [retval][out] */ __RPC__deref_out_opt BSTR *retval);
        
        /* [id][propput][funcdescattr] */ HRESULT ( STDMETHODCALLTYPE *put_FxCopRuleAssemblies )( 
            __RPC__in WebSiteProperties5 * This,
            /* [in][idldescattr] */ __RPC__in BSTR noname,
            /* [retval][out] */ __RPC__out void *retval);
        
        /* [id][propget][funcdescattr] */ HRESULT ( STDMETHODCALLTYPE *get_FxCopRules )( 
            __RPC__in WebSiteProperties5 * This,
            /* [retval][out] */ __RPC__deref_out_opt BSTR *retval);
        
        /* [id][propput][funcdescattr] */ HRESULT ( STDMETHODCALLTYPE *put_FxCopRules )( 
            __RPC__in WebSiteProperties5 * This,
            /* [in][idldescattr] */ __RPC__in BSTR noname,
            /* [retval][out] */ __RPC__out void *retval);
        
        /* [id][propget][funcdescattr] */ HRESULT ( STDMETHODCALLTYPE *get_EnableVsWebServerDynamicPort )( 
            __RPC__in WebSiteProperties5 * This,
            /* [retval][out] */ __RPC__out BOOLEAN *retval);
        
        /* [id][propput][funcdescattr] */ HRESULT ( STDMETHODCALLTYPE *put_EnableVsWebServerDynamicPort )( 
            __RPC__in WebSiteProperties5 * This,
            /* [in][idldescattr] */ BOOLEAN noname,
            /* [retval][out] */ __RPC__out void *retval);
        
        /* [id][propget][funcdescattr] */ HRESULT ( STDMETHODCALLTYPE *get_VsWebServerPort )( 
            __RPC__in WebSiteProperties5 * This,
            /* [retval][out] */ __RPC__out unsigned short *retval);
        
        /* [id][propput][funcdescattr] */ HRESULT ( STDMETHODCALLTYPE *put_VsWebServerPort )( 
            __RPC__in WebSiteProperties5 * This,
            /* [in][idldescattr] */ unsigned short noname,
            /* [retval][out] */ __RPC__out void *retval);
        
        /* [id][propget][funcdescattr] */ HRESULT ( STDMETHODCALLTYPE *get_EnableNTLMAuthentication )( 
            __RPC__in WebSiteProperties5 * This,
            /* [retval][out] */ __RPC__out BOOLEAN *retval);
        
        /* [id][propput][funcdescattr] */ HRESULT ( STDMETHODCALLTYPE *put_EnableNTLMAuthentication )( 
            __RPC__in WebSiteProperties5 * This,
            /* [in][idldescattr] */ BOOLEAN noname,
            /* [retval][out] */ __RPC__out void *retval);
        
        /* [id][propget][funcdescattr] */ HRESULT ( STDMETHODCALLTYPE *get_ProjectDirty )( 
            __RPC__in WebSiteProperties5 * This,
            /* [retval][out] */ __RPC__out BOOLEAN *retval);
        
        /* [id][propget][funcdescattr] */ HRESULT ( STDMETHODCALLTYPE *get_VsWebServerVPath )( 
            __RPC__in WebSiteProperties5 * This,
            /* [retval][out] */ __RPC__deref_out_opt BSTR *retval);
        
        /* [id][propput][funcdescattr] */ HRESULT ( STDMETHODCALLTYPE *put_VsWebServerVPath )( 
            __RPC__in WebSiteProperties5 * This,
            /* [in][idldescattr] */ __RPC__in BSTR noname,
            /* [retval][out] */ __RPC__out void *retval);
        
        /* [id][propget][funcdescattr] */ HRESULT ( STDMETHODCALLTYPE *get_TargetFramework )( 
            __RPC__in WebSiteProperties5 * This,
            /* [retval][out] */ __RPC__out unsigned long *retval);
        
        /* [id][propput][funcdescattr] */ HRESULT ( STDMETHODCALLTYPE *put_TargetFramework )( 
            __RPC__in WebSiteProperties5 * This,
            /* [in][idldescattr] */ unsigned long noname,
            /* [retval][out] */ __RPC__out void *retval);
        
        /* [id][propget][funcdescattr] */ HRESULT ( STDMETHODCALLTYPE *get_StartWebServerOnDebug )( 
            __RPC__in WebSiteProperties5 * This,
            /* [retval][out] */ __RPC__out BOOLEAN *retval);
        
        /* [id][propput][funcdescattr] */ HRESULT ( STDMETHODCALLTYPE *put_StartWebServerOnDebug )( 
            __RPC__in WebSiteProperties5 * This,
            /* [in][idldescattr] */ BOOLEAN noname,
            /* [retval][out] */ __RPC__out void *retval);
        
        /* [helpstring][id][nonbrowsable][propget] */ HRESULT ( STDMETHODCALLTYPE *get_TargetFrameworkMoniker )( 
            __RPC__in WebSiteProperties5 * This,
            /* [retval][out] */ __RPC__deref_out_opt BSTR *pbstrTargetFrameworkMoniker);
        
        /* [helpstring][id][nonbrowsable][propput] */ HRESULT ( STDMETHODCALLTYPE *put_TargetFrameworkMoniker )( 
            __RPC__in WebSiteProperties5 * This,
            /* [in] */ __RPC__in BSTR bstrTargetFrameworkMoniker);
        
        /* [helpstring][id][propget] */ HRESULT ( STDMETHODCALLTYPE *get_CodeAnalysisRuleSet )( 
            __RPC__in WebSiteProperties5 * This,
            /* [retval][out] */ __RPC__deref_out_opt BSTR *pbstrRuleSet);
        
        /* [helpstring][id][propput] */ HRESULT ( STDMETHODCALLTYPE *put_CodeAnalysisRuleSet )( 
            __RPC__in WebSiteProperties5 * This,
            /* [in] */ __RPC__in BSTR bstrRuleSet);
        
        /* [helpstring][id][propget] */ HRESULT ( STDMETHODCALLTYPE *get_CodeAnalysisRuleSetDirectories )( 
            __RPC__in WebSiteProperties5 * This,
            /* [retval][out] */ __RPC__deref_out_opt BSTR *pbstrRuleSetDirectories);
        
        /* [helpstring][id][propput] */ HRESULT ( STDMETHODCALLTYPE *put_CodeAnalysisRuleSetDirectories )( 
            __RPC__in WebSiteProperties5 * This,
            /* [in] */ __RPC__in BSTR bstrRuleSetDirectories);
        
        /* [helpstring][id][propget] */ HRESULT ( STDMETHODCALLTYPE *get_CodeAnalysisIgnoreBuiltInRuleSets )( 
            __RPC__in WebSiteProperties5 * This,
            /* [retval][out] */ __RPC__out VARIANT_BOOL *pbIgnoreBuiltInRuleSets);
        
        /* [helpstring][id][propput] */ HRESULT ( STDMETHODCALLTYPE *put_CodeAnalysisIgnoreBuiltInRuleSets )( 
            __RPC__in WebSiteProperties5 * This,
            /* [in] */ VARIANT_BOOL IgnoreBuiltInRuleSets);
        
        /* [helpstring][id][propget] */ HRESULT ( STDMETHODCALLTYPE *get_CodeAnalysisRuleDirectories )( 
            __RPC__in WebSiteProperties5 * This,
            /* [retval][out] */ __RPC__deref_out_opt BSTR *pbstrRuleDirectories);
        
        /* [helpstring][id][propput] */ HRESULT ( STDMETHODCALLTYPE *put_CodeAnalysisRuleDirectories )( 
            __RPC__in WebSiteProperties5 * This,
            /* [in] */ __RPC__in BSTR bstrRuleDirectories);
        
        /* [helpstring][id][propget] */ HRESULT ( STDMETHODCALLTYPE *get_CodeAnalysisIgnoreBuiltInRules )( 
            __RPC__in WebSiteProperties5 * This,
            /* [retval][out] */ __RPC__out VARIANT_BOOL *pbIgnoreBuiltInRules);
        
        /* [helpstring][id][propput] */ HRESULT ( STDMETHODCALLTYPE *put_CodeAnalysisIgnoreBuiltInRules )( 
            __RPC__in WebSiteProperties5 * This,
            /* [in] */ VARIANT_BOOL IgnoreBuiltInRules);
        
        /* [helpstring][id][propget] */ HRESULT ( STDMETHODCALLTYPE *get_CodeAnalysisFailOnMissingRules )( 
            __RPC__in WebSiteProperties5 * This,
            /* [retval][out] */ __RPC__out VARIANT_BOOL *pbFailOnMissingRules);
        
        /* [helpstring][id][propput] */ HRESULT ( STDMETHODCALLTYPE *put_CodeAnalysisFailOnMissingRules )( 
            __RPC__in WebSiteProperties5 * This,
            /* [in] */ VARIANT_BOOL FailOnMissingRules);
        
        /* [helpstring][id][propget] */ HRESULT ( STDMETHODCALLTYPE *get_DevelopmentServerCommandLine )( 
            __RPC__in WebSiteProperties5 * This,
            /* [retval][out] */ __RPC__deref_out_opt BSTR *pbstrCommandLine);
        
        /* [helpstring][id][propget] */ HRESULT ( STDMETHODCALLTYPE *get_SecureUrl )( 
            __RPC__in WebSiteProperties5 * This,
            /* [retval][out] */ __RPC__deref_out_opt BSTR *pbstrSecureUrl);
        
        /* [helpstring][id][propget] */ HRESULT ( STDMETHODCALLTYPE *get_NonSecureUrl )( 
            __RPC__in WebSiteProperties5 * This,
            /* [retval][out] */ __RPC__deref_out_opt BSTR *pbstrNonSecureUrl);
        
        /* [helpstring][id][propget] */ HRESULT ( STDMETHODCALLTYPE *get_SSLEnabled )( 
            __RPC__in WebSiteProperties5 * This,
            /* [retval][out] */ __RPC__out VARIANT_BOOL *pbSSLEnabled);
        
        /* [helpstring][id][propput] */ HRESULT ( STDMETHODCALLTYPE *put_SSLEnabled )( 
            __RPC__in WebSiteProperties5 * This,
            /* [in] */ VARIANT_BOOL bEnableSSL);
        
        /* [helpstring][id][propget] */ HRESULT ( STDMETHODCALLTYPE *get_ManagedPipelineMode )( 
            __RPC__in WebSiteProperties5 * This,
            /* [retval][out] */ __RPC__out IISManagedPipelineMode *pPipeLineMode);
        
        /* [helpstring][id][propput] */ HRESULT ( STDMETHODCALLTYPE *put_ManagedPipelineMode )( 
            __RPC__in WebSiteProperties5 * This,
            /* [in] */ IISManagedPipelineMode pipeLineMode);
        
        /* [helpstring][id][propget] */ HRESULT ( STDMETHODCALLTYPE *get_AnonymousAuthenticationEnabled )( 
            __RPC__in WebSiteProperties5 * This,
            /* [retval][out] */ __RPC__out VARIANT_BOOL *pEnabled);
        
        /* [helpstring][id][propput] */ HRESULT ( STDMETHODCALLTYPE *put_AnonymousAuthenticationEnabled )( 
            __RPC__in WebSiteProperties5 * This,
            /* [in] */ VARIANT_BOOL bEnable);
        
        /* [helpstring][id][propget] */ HRESULT ( STDMETHODCALLTYPE *get_WindowsAuthenticationEnabled )( 
            __RPC__in WebSiteProperties5 * This,
            /* [retval][out] */ __RPC__out VARIANT_BOOL *pEnabled);
        
        /* [helpstring][id][propput] */ HRESULT ( STDMETHODCALLTYPE *put_WindowsAuthenticationEnabled )( 
            __RPC__in WebSiteProperties5 * This,
            /* [in] */ VARIANT_BOOL bEnable);
        
        END_INTERFACE
    } WebSiteProperties5Vtbl;

    interface WebSiteProperties5
    {
        CONST_VTBL struct WebSiteProperties5Vtbl *lpVtbl;
    };

    

#ifdef COBJMACROS


#define WebSiteProperties5_QueryInterface(This,riid,ppvObj,retval)	\
    ( (This)->lpVtbl -> QueryInterface(This,riid,ppvObj,retval) ) 

#define WebSiteProperties5_AddRef(This,retval)	\
    ( (This)->lpVtbl -> AddRef(This,retval) ) 

#define WebSiteProperties5_Release(This,retval)	\
    ( (This)->lpVtbl -> Release(This,retval) ) 

#define WebSiteProperties5_GetTypeInfoCount(This,pctinfo,retval)	\
    ( (This)->lpVtbl -> GetTypeInfoCount(This,pctinfo,retval) ) 

#define WebSiteProperties5_GetTypeInfo(This,itinfo,lcid,pptinfo,retval)	\
    ( (This)->lpVtbl -> GetTypeInfo(This,itinfo,lcid,pptinfo,retval) ) 

#define WebSiteProperties5_GetIDsOfNames(This,riid,rgszNames,cNames,lcid,rgdispid,retval)	\
    ( (This)->lpVtbl -> GetIDsOfNames(This,riid,rgszNames,cNames,lcid,rgdispid,retval) ) 

#define WebSiteProperties5_Invoke(This,dispidMember,riid,lcid,wFlags,pdispparams,pvarResult,pexcepinfo,puArgErr,retval)	\
    ( (This)->lpVtbl -> Invoke(This,dispidMember,riid,lcid,wFlags,pdispparams,pvarResult,pexcepinfo,puArgErr,retval) ) 

#define WebSiteProperties5_get___id(This,retval)	\
    ( (This)->lpVtbl -> get___id(This,retval) ) 

#define WebSiteProperties5_get_OpenedURL(This,retval)	\
    ( (This)->lpVtbl -> get_OpenedURL(This,retval) ) 

#define WebSiteProperties5_get_FullPath(This,retval)	\
    ( (This)->lpVtbl -> get_FullPath(This,retval) ) 

#define WebSiteProperties5_get_BrowseURL(This,retval)	\
    ( (This)->lpVtbl -> get_BrowseURL(This,retval) ) 

#define WebSiteProperties5_put_BrowseURL(This,noname,retval)	\
    ( (This)->lpVtbl -> put_BrowseURL(This,noname,retval) ) 

#define WebSiteProperties5_get_EnableVsWebServer(This,retval)	\
    ( (This)->lpVtbl -> get_EnableVsWebServer(This,retval) ) 

#define WebSiteProperties5_put_EnableVsWebServer(This,noname,retval)	\
    ( (This)->lpVtbl -> put_EnableVsWebServer(This,noname,retval) ) 

#define WebSiteProperties5_get_WebSiteType(This,retval)	\
    ( (This)->lpVtbl -> get_WebSiteType(This,retval) ) 

#define WebSiteProperties5_get_CurrentWebsiteLanguage(This,retval)	\
    ( (This)->lpVtbl -> get_CurrentWebsiteLanguage(This,retval) ) 

#define WebSiteProperties5_put_CurrentWebsiteLanguage(This,noname,retval)	\
    ( (This)->lpVtbl -> put_CurrentWebsiteLanguage(This,noname,retval) ) 

#define WebSiteProperties5_get_StartProgram(This,retval)	\
    ( (This)->lpVtbl -> get_StartProgram(This,retval) ) 

#define WebSiteProperties5_put_StartProgram(This,noname,retval)	\
    ( (This)->lpVtbl -> put_StartProgram(This,noname,retval) ) 

#define WebSiteProperties5_get_StartWorkingDirectory(This,retval)	\
    ( (This)->lpVtbl -> get_StartWorkingDirectory(This,retval) ) 

#define WebSiteProperties5_put_StartWorkingDirectory(This,noname,retval)	\
    ( (This)->lpVtbl -> put_StartWorkingDirectory(This,noname,retval) ) 

#define WebSiteProperties5_get_StartURL(This,retval)	\
    ( (This)->lpVtbl -> get_StartURL(This,retval) ) 

#define WebSiteProperties5_put_StartURL(This,noname,retval)	\
    ( (This)->lpVtbl -> put_StartURL(This,noname,retval) ) 

#define WebSiteProperties5_get_StartPage(This,retval)	\
    ( (This)->lpVtbl -> get_StartPage(This,retval) ) 

#define WebSiteProperties5_put_StartPage(This,noname,retval)	\
    ( (This)->lpVtbl -> put_StartPage(This,noname,retval) ) 

#define WebSiteProperties5_get_StartArguments(This,retval)	\
    ( (This)->lpVtbl -> get_StartArguments(This,retval) ) 

#define WebSiteProperties5_put_StartArguments(This,noname,retval)	\
    ( (This)->lpVtbl -> put_StartArguments(This,noname,retval) ) 

#define WebSiteProperties5_get_EnableASPXDebugging(This,retval)	\
    ( (This)->lpVtbl -> get_EnableASPXDebugging(This,retval) ) 

#define WebSiteProperties5_put_EnableASPXDebugging(This,noname,retval)	\
    ( (This)->lpVtbl -> put_EnableASPXDebugging(This,noname,retval) ) 

#define WebSiteProperties5_get_EnableUnmanagedDebugging(This,retval)	\
    ( (This)->lpVtbl -> get_EnableUnmanagedDebugging(This,retval) ) 

#define WebSiteProperties5_put_EnableUnmanagedDebugging(This,noname,retval)	\
    ( (This)->lpVtbl -> put_EnableUnmanagedDebugging(This,noname,retval) ) 

#define WebSiteProperties5_get_StartAction(This,retval)	\
    ( (This)->lpVtbl -> get_StartAction(This,retval) ) 

#define WebSiteProperties5_put_StartAction(This,noname,retval)	\
    ( (This)->lpVtbl -> put_StartAction(This,noname,retval) ) 

#define WebSiteProperties5_get_EnableSQLServerDebugging(This,retval)	\
    ( (This)->lpVtbl -> get_EnableSQLServerDebugging(This,retval) ) 

#define WebSiteProperties5_put_EnableSQLServerDebugging(This,noname,retval)	\
    ( (This)->lpVtbl -> put_EnableSQLServerDebugging(This,noname,retval) ) 

#define WebSiteProperties5_get_Extender(This,ExtenderName,retval)	\
    ( (This)->lpVtbl -> get_Extender(This,ExtenderName,retval) ) 

#define WebSiteProperties5_get_ExtenderNames(This,retval)	\
    ( (This)->lpVtbl -> get_ExtenderNames(This,retval) ) 

#define WebSiteProperties5_get_ExtenderCATID(This,retval)	\
    ( (This)->lpVtbl -> get_ExtenderCATID(This,retval) ) 

#define WebSiteProperties5_get_EnableFxCop(This,retval)	\
    ( (This)->lpVtbl -> get_EnableFxCop(This,retval) ) 

#define WebSiteProperties5_put_EnableFxCop(This,noname,retval)	\
    ( (This)->lpVtbl -> put_EnableFxCop(This,noname,retval) ) 

#define WebSiteProperties5_get_FxCopRuleAssemblies(This,retval)	\
    ( (This)->lpVtbl -> get_FxCopRuleAssemblies(This,retval) ) 

#define WebSiteProperties5_put_FxCopRuleAssemblies(This,noname,retval)	\
    ( (This)->lpVtbl -> put_FxCopRuleAssemblies(This,noname,retval) ) 

#define WebSiteProperties5_get_FxCopRules(This,retval)	\
    ( (This)->lpVtbl -> get_FxCopRules(This,retval) ) 

#define WebSiteProperties5_put_FxCopRules(This,noname,retval)	\
    ( (This)->lpVtbl -> put_FxCopRules(This,noname,retval) ) 

#define WebSiteProperties5_get_EnableVsWebServerDynamicPort(This,retval)	\
    ( (This)->lpVtbl -> get_EnableVsWebServerDynamicPort(This,retval) ) 

#define WebSiteProperties5_put_EnableVsWebServerDynamicPort(This,noname,retval)	\
    ( (This)->lpVtbl -> put_EnableVsWebServerDynamicPort(This,noname,retval) ) 

#define WebSiteProperties5_get_VsWebServerPort(This,retval)	\
    ( (This)->lpVtbl -> get_VsWebServerPort(This,retval) ) 

#define WebSiteProperties5_put_VsWebServerPort(This,noname,retval)	\
    ( (This)->lpVtbl -> put_VsWebServerPort(This,noname,retval) ) 

#define WebSiteProperties5_get_EnableNTLMAuthentication(This,retval)	\
    ( (This)->lpVtbl -> get_EnableNTLMAuthentication(This,retval) ) 

#define WebSiteProperties5_put_EnableNTLMAuthentication(This,noname,retval)	\
    ( (This)->lpVtbl -> put_EnableNTLMAuthentication(This,noname,retval) ) 

#define WebSiteProperties5_get_ProjectDirty(This,retval)	\
    ( (This)->lpVtbl -> get_ProjectDirty(This,retval) ) 

#define WebSiteProperties5_get_VsWebServerVPath(This,retval)	\
    ( (This)->lpVtbl -> get_VsWebServerVPath(This,retval) ) 

#define WebSiteProperties5_put_VsWebServerVPath(This,noname,retval)	\
    ( (This)->lpVtbl -> put_VsWebServerVPath(This,noname,retval) ) 

#define WebSiteProperties5_get_TargetFramework(This,retval)	\
    ( (This)->lpVtbl -> get_TargetFramework(This,retval) ) 

#define WebSiteProperties5_put_TargetFramework(This,noname,retval)	\
    ( (This)->lpVtbl -> put_TargetFramework(This,noname,retval) ) 

#define WebSiteProperties5_get_StartWebServerOnDebug(This,retval)	\
    ( (This)->lpVtbl -> get_StartWebServerOnDebug(This,retval) ) 

#define WebSiteProperties5_put_StartWebServerOnDebug(This,noname,retval)	\
    ( (This)->lpVtbl -> put_StartWebServerOnDebug(This,noname,retval) ) 


#define WebSiteProperties5_get_TargetFrameworkMoniker(This,pbstrTargetFrameworkMoniker)	\
    ( (This)->lpVtbl -> get_TargetFrameworkMoniker(This,pbstrTargetFrameworkMoniker) ) 

#define WebSiteProperties5_put_TargetFrameworkMoniker(This,bstrTargetFrameworkMoniker)	\
    ( (This)->lpVtbl -> put_TargetFrameworkMoniker(This,bstrTargetFrameworkMoniker) ) 

#define WebSiteProperties5_get_CodeAnalysisRuleSet(This,pbstrRuleSet)	\
    ( (This)->lpVtbl -> get_CodeAnalysisRuleSet(This,pbstrRuleSet) ) 

#define WebSiteProperties5_put_CodeAnalysisRuleSet(This,bstrRuleSet)	\
    ( (This)->lpVtbl -> put_CodeAnalysisRuleSet(This,bstrRuleSet) ) 

#define WebSiteProperties5_get_CodeAnalysisRuleSetDirectories(This,pbstrRuleSetDirectories)	\
    ( (This)->lpVtbl -> get_CodeAnalysisRuleSetDirectories(This,pbstrRuleSetDirectories) ) 

#define WebSiteProperties5_put_CodeAnalysisRuleSetDirectories(This,bstrRuleSetDirectories)	\
    ( (This)->lpVtbl -> put_CodeAnalysisRuleSetDirectories(This,bstrRuleSetDirectories) ) 

#define WebSiteProperties5_get_CodeAnalysisIgnoreBuiltInRuleSets(This,pbIgnoreBuiltInRuleSets)	\
    ( (This)->lpVtbl -> get_CodeAnalysisIgnoreBuiltInRuleSets(This,pbIgnoreBuiltInRuleSets) ) 

#define WebSiteProperties5_put_CodeAnalysisIgnoreBuiltInRuleSets(This,IgnoreBuiltInRuleSets)	\
    ( (This)->lpVtbl -> put_CodeAnalysisIgnoreBuiltInRuleSets(This,IgnoreBuiltInRuleSets) ) 

#define WebSiteProperties5_get_CodeAnalysisRuleDirectories(This,pbstrRuleDirectories)	\
    ( (This)->lpVtbl -> get_CodeAnalysisRuleDirectories(This,pbstrRuleDirectories) ) 

#define WebSiteProperties5_put_CodeAnalysisRuleDirectories(This,bstrRuleDirectories)	\
    ( (This)->lpVtbl -> put_CodeAnalysisRuleDirectories(This,bstrRuleDirectories) ) 

#define WebSiteProperties5_get_CodeAnalysisIgnoreBuiltInRules(This,pbIgnoreBuiltInRules)	\
    ( (This)->lpVtbl -> get_CodeAnalysisIgnoreBuiltInRules(This,pbIgnoreBuiltInRules) ) 

#define WebSiteProperties5_put_CodeAnalysisIgnoreBuiltInRules(This,IgnoreBuiltInRules)	\
    ( (This)->lpVtbl -> put_CodeAnalysisIgnoreBuiltInRules(This,IgnoreBuiltInRules) ) 

#define WebSiteProperties5_get_CodeAnalysisFailOnMissingRules(This,pbFailOnMissingRules)	\
    ( (This)->lpVtbl -> get_CodeAnalysisFailOnMissingRules(This,pbFailOnMissingRules) ) 

#define WebSiteProperties5_put_CodeAnalysisFailOnMissingRules(This,FailOnMissingRules)	\
    ( (This)->lpVtbl -> put_CodeAnalysisFailOnMissingRules(This,FailOnMissingRules) ) 

#define WebSiteProperties5_get_DevelopmentServerCommandLine(This,pbstrCommandLine)	\
    ( (This)->lpVtbl -> get_DevelopmentServerCommandLine(This,pbstrCommandLine) ) 


#define WebSiteProperties5_get_SecureUrl(This,pbstrSecureUrl)	\
    ( (This)->lpVtbl -> get_SecureUrl(This,pbstrSecureUrl) ) 

#define WebSiteProperties5_get_NonSecureUrl(This,pbstrNonSecureUrl)	\
    ( (This)->lpVtbl -> get_NonSecureUrl(This,pbstrNonSecureUrl) ) 

#define WebSiteProperties5_get_SSLEnabled(This,pbSSLEnabled)	\
    ( (This)->lpVtbl -> get_SSLEnabled(This,pbSSLEnabled) ) 

#define WebSiteProperties5_put_SSLEnabled(This,bEnableSSL)	\
    ( (This)->lpVtbl -> put_SSLEnabled(This,bEnableSSL) ) 

#define WebSiteProperties5_get_ManagedPipelineMode(This,pPipeLineMode)	\
    ( (This)->lpVtbl -> get_ManagedPipelineMode(This,pPipeLineMode) ) 

#define WebSiteProperties5_put_ManagedPipelineMode(This,pipeLineMode)	\
    ( (This)->lpVtbl -> put_ManagedPipelineMode(This,pipeLineMode) ) 

#define WebSiteProperties5_get_AnonymousAuthenticationEnabled(This,pEnabled)	\
    ( (This)->lpVtbl -> get_AnonymousAuthenticationEnabled(This,pEnabled) ) 

#define WebSiteProperties5_put_AnonymousAuthenticationEnabled(This,bEnable)	\
    ( (This)->lpVtbl -> put_AnonymousAuthenticationEnabled(This,bEnable) ) 

#define WebSiteProperties5_get_WindowsAuthenticationEnabled(This,pEnabled)	\
    ( (This)->lpVtbl -> get_WindowsAuthenticationEnabled(This,pEnabled) ) 

#define WebSiteProperties5_put_WindowsAuthenticationEnabled(This,bEnable)	\
    ( (This)->lpVtbl -> put_WindowsAuthenticationEnabled(This,bEnable) ) 

#endif /* COBJMACROS */


#endif 	/* C style interface */




#endif 	/* __WebSiteProperties5_INTERFACE_DEFINED__ */

#endif /* __VSWebSite100_LIBRARY_DEFINED__ */

/* Additional Prototypes for ALL interfaces */

/* end of Additional Prototypes */

#ifdef __cplusplus
}
#endif

#endif


