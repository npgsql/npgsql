

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


#ifndef __vslangproj90_h__
#define __vslangproj90_h__

#if defined(_MSC_VER) && (_MSC_VER >= 1020)
#pragma once
#endif

/* Forward Declarations */ 

#ifndef __VBProjectProperties4_FWD_DEFINED__
#define __VBProjectProperties4_FWD_DEFINED__
typedef interface VBProjectProperties4 VBProjectProperties4;

#endif 	/* __VBProjectProperties4_FWD_DEFINED__ */


#ifndef __VBPackageSettings2_FWD_DEFINED__
#define __VBPackageSettings2_FWD_DEFINED__
typedef interface VBPackageSettings2 VBPackageSettings2;

#endif 	/* __VBPackageSettings2_FWD_DEFINED__ */


#ifndef __CSharpProjectProperties4_FWD_DEFINED__
#define __CSharpProjectProperties4_FWD_DEFINED__
typedef interface CSharpProjectProperties4 CSharpProjectProperties4;

#endif 	/* __CSharpProjectProperties4_FWD_DEFINED__ */


#ifndef __CSharpProjectConfigurationProperties4_FWD_DEFINED__
#define __CSharpProjectConfigurationProperties4_FWD_DEFINED__
typedef interface CSharpProjectConfigurationProperties4 CSharpProjectConfigurationProperties4;

#endif 	/* __CSharpProjectConfigurationProperties4_FWD_DEFINED__ */


#ifndef __VBProjectConfigurationProperties4_FWD_DEFINED__
#define __VBProjectConfigurationProperties4_FWD_DEFINED__
typedef interface VBProjectConfigurationProperties4 VBProjectConfigurationProperties4;

#endif 	/* __VBProjectConfigurationProperties4_FWD_DEFINED__ */


/* header files for imported files */
#include "oaidl.h"

#ifdef __cplusplus
extern "C"{
#endif 


/* interface __MIDL_itf_vslangproj90_0000_0000 */
/* [local] */ 

#define VSLANGPROJ90_VER_MAJ    9
#define VSLANGPROJ90_VER_MIN    0


extern RPC_IF_HANDLE __MIDL_itf_vslangproj90_0000_0000_v0_0_c_ifspec;
extern RPC_IF_HANDLE __MIDL_itf_vslangproj90_0000_0000_v0_0_s_ifspec;


#ifndef __VSLangProj90_LIBRARY_DEFINED__
#define __VSLangProj90_LIBRARY_DEFINED__

/* library VSLangProj90 */
/* [version][helpstring][uuid] */ 


enum VsProjPropId90
    {
        VBPROJPROPID_TargetFramework	= 12200,
        VBPROJPROPID_ApplicationManifest	= 12201
    } ;

enum VBProjPropId90
    {
        VBPROJPROPID_OptionInfer	= 15101,
        VBPROJPROPID_CodeAnalysisIgnoreGeneratedCode	= 15208,
        VBPROJPROPID_CodeAnalysisOverrideRuleVisibilities	= 15209,
        VBPROJPROPID_CodeAnalysisCulture	= 15210,
        VBPROJPROPID_CodeAnalysisDictionaries	= 15211
    } ;
typedef /* [uuid] */  DECLSPEC_UUID("39d4f2ae-776e-4948-bda3-8dee4c5d4abd") 
enum pkgOptionInfer
    {
        pkgOptionInferOff	= 0,
        pkgOptionInferOn	= ( pkgOptionInferOff + 1 ) 
    } 	pkgOptionInfer;

#define pkgOptionInferMin  pkgOptionInferOff
#define pkgOptionInferMax  pkgOptionInferOn
typedef /* [uuid] */  DECLSPEC_UUID("e629a10f-3e9c-4f54-a281-fab74b25ad7c") 
enum prjOptionInfer
    {
        prjOptionInferOff	= 0,
        prjOptionInferOn	= ( prjOptionInferOff + 1 ) 
    } 	prjOptionInfer;

#define prjOptionInferMin  prjOptionInferOff
#define prjOptionInferMax  prjOptionInferOn

EXTERN_C const IID LIBID_VSLangProj90;


#ifndef __prjApplicationManifestValues_MODULE_DEFINED__
#define __prjApplicationManifestValues_MODULE_DEFINED__


/* module prjApplicationManifestValues */
/* [helpstring][dllname][uuid] */ 

const LPWSTR prjApplicationManifest_Default	=	L"DefaultManifest";

const LPWSTR prjApplicationManifest_NoManifest	=	L"NoManifest";

#endif /* __prjApplicationManifestValues_MODULE_DEFINED__ */

#ifndef __VBProjectProperties4_INTERFACE_DEFINED__
#define __VBProjectProperties4_INTERFACE_DEFINED__

/* interface VBProjectProperties4 */
/* [object][dual][unique][helpstring][uuid] */ 


EXTERN_C const IID IID_VBProjectProperties4;

#if defined(__cplusplus) && !defined(CINTERFACE)
    
    MIDL_INTERFACE("c530b098-acc9-434e-9671-124eaa73fa00")
    VBProjectProperties4 : public VBProjectProperties3
    {
    public:
        virtual /* [helpstring][id][nonbrowsable][propget] */ HRESULT STDMETHODCALLTYPE get_TargetFramework( 
            /* [retval][out] */ __RPC__out DWORD *pdwTargetFramework) = 0;
        
        virtual /* [helpstring][id][nonbrowsable][propput] */ HRESULT STDMETHODCALLTYPE put_TargetFramework( 
            /* [in] */ DWORD dwTargetFramework) = 0;
        
        virtual /* [helpstring][id][propget] */ HRESULT STDMETHODCALLTYPE get_ApplicationManifest( 
            /* [retval][out] */ __RPC__deref_out_opt BSTR *pbstrApplicationManifest) = 0;
        
        virtual /* [helpstring][id][propput] */ HRESULT STDMETHODCALLTYPE put_ApplicationManifest( 
            /* [in] */ __RPC__in BSTR ApplicationManifest) = 0;
        
        virtual /* [helpstring][id][propget] */ HRESULT STDMETHODCALLTYPE get_OptionInfer( 
            /* [retval][out] */ __RPC__out prjOptionInfer *pOptionInfer) = 0;
        
        virtual /* [helpstring][id][propput] */ HRESULT STDMETHODCALLTYPE put_OptionInfer( 
            /* [in] */ prjOptionInfer optionInfer) = 0;
        
    };
    
    
#else 	/* C style interface */

    typedef struct VBProjectProperties4Vtbl
    {
        BEGIN_INTERFACE
        
        /* [id][restricted][funcdescattr] */ HRESULT ( STDMETHODCALLTYPE *QueryInterface )( 
            __RPC__in VBProjectProperties4 * This,
            /* [in][idldescattr] */ __RPC__in struct GUID *riid,
            /* [out][idldescattr] */ __RPC__deref_out_opt void **ppvObj,
            /* [retval][out] */ __RPC__out void *retval);
        
        /* [id][restricted][funcdescattr] */ HRESULT ( STDMETHODCALLTYPE *AddRef )( 
            __RPC__in VBProjectProperties4 * This,
            /* [retval][out] */ __RPC__out unsigned long *retval);
        
        /* [id][restricted][funcdescattr] */ HRESULT ( STDMETHODCALLTYPE *Release )( 
            __RPC__in VBProjectProperties4 * This,
            /* [retval][out] */ __RPC__out unsigned long *retval);
        
        /* [id][restricted][funcdescattr] */ HRESULT ( STDMETHODCALLTYPE *GetTypeInfoCount )( 
            __RPC__in VBProjectProperties4 * This,
            /* [out][idldescattr] */ __RPC__out unsigned UINT *pctinfo,
            /* [retval][out] */ __RPC__out void *retval);
        
        /* [id][restricted][funcdescattr] */ HRESULT ( STDMETHODCALLTYPE *GetTypeInfo )( 
            __RPC__in VBProjectProperties4 * This,
            /* [in][idldescattr] */ unsigned UINT itinfo,
            /* [in][idldescattr] */ unsigned long lcid,
            /* [out][idldescattr] */ __RPC__deref_out_opt void **pptinfo,
            /* [retval][out] */ __RPC__out void *retval);
        
        /* [id][restricted][funcdescattr] */ HRESULT ( STDMETHODCALLTYPE *GetIDsOfNames )( 
            __RPC__in VBProjectProperties4 * This,
            /* [in][idldescattr] */ __RPC__in struct GUID *riid,
            /* [in][idldescattr] */ __RPC__deref_in_opt signed char **rgszNames,
            /* [in][idldescattr] */ unsigned UINT cNames,
            /* [in][idldescattr] */ unsigned long lcid,
            /* [out][idldescattr] */ __RPC__out signed long *rgdispid,
            /* [retval][out] */ __RPC__out void *retval);
        
        /* [id][restricted][funcdescattr] */ HRESULT ( STDMETHODCALLTYPE *Invoke )( 
            __RPC__in VBProjectProperties4 * This,
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
            __RPC__in VBProjectProperties4 * This,
            /* [retval][out] */ __RPC__deref_out_opt BSTR *retval);
        
        /* [id][propget][hidden][funcdescattr] */ HRESULT ( STDMETHODCALLTYPE *get___project )( 
            __RPC__in VBProjectProperties4 * This,
            /* [retval][out] */ __RPC__deref_out_opt IUnknown **retval);
        
        /* [id][propget][funcdescattr] */ HRESULT ( STDMETHODCALLTYPE *get_StartupObject )( 
            __RPC__in VBProjectProperties4 * This,
            /* [retval][out] */ __RPC__deref_out_opt BSTR *retval);
        
        /* [id][propput][funcdescattr] */ HRESULT ( STDMETHODCALLTYPE *put_StartupObject )( 
            __RPC__in VBProjectProperties4 * This,
            /* [in][idldescattr] */ __RPC__in BSTR noname,
            /* [retval][out] */ __RPC__out void *retval);
        
        /* [id][propget][funcdescattr] */ HRESULT ( STDMETHODCALLTYPE *get_OutputType )( 
            __RPC__in VBProjectProperties4 * This,
            /* [retval][out] */ __RPC__out enum prjOutputType *retval);
        
        /* [id][propput][funcdescattr] */ HRESULT ( STDMETHODCALLTYPE *put_OutputType )( 
            __RPC__in VBProjectProperties4 * This,
            /* [in][idldescattr] */ enum prjOutputType noname,
            /* [retval][out] */ __RPC__out void *retval);
        
        /* [id][propget][funcdescattr] */ HRESULT ( STDMETHODCALLTYPE *get_RootNamespace )( 
            __RPC__in VBProjectProperties4 * This,
            /* [retval][out] */ __RPC__deref_out_opt BSTR *retval);
        
        /* [id][propput][funcdescattr] */ HRESULT ( STDMETHODCALLTYPE *put_RootNamespace )( 
            __RPC__in VBProjectProperties4 * This,
            /* [in][idldescattr] */ __RPC__in BSTR noname,
            /* [retval][out] */ __RPC__out void *retval);
        
        /* [id][propget][funcdescattr] */ HRESULT ( STDMETHODCALLTYPE *get_AssemblyName )( 
            __RPC__in VBProjectProperties4 * This,
            /* [retval][out] */ __RPC__deref_out_opt BSTR *retval);
        
        /* [id][propput][funcdescattr] */ HRESULT ( STDMETHODCALLTYPE *put_AssemblyName )( 
            __RPC__in VBProjectProperties4 * This,
            /* [in][idldescattr] */ __RPC__in BSTR noname,
            /* [retval][out] */ __RPC__out void *retval);
        
        /* [id][propget][funcdescattr] */ HRESULT ( STDMETHODCALLTYPE *get_AssemblyOriginatorKeyFile )( 
            __RPC__in VBProjectProperties4 * This,
            /* [retval][out] */ __RPC__deref_out_opt BSTR *retval);
        
        /* [id][propput][funcdescattr] */ HRESULT ( STDMETHODCALLTYPE *put_AssemblyOriginatorKeyFile )( 
            __RPC__in VBProjectProperties4 * This,
            /* [in][idldescattr] */ __RPC__in BSTR noname,
            /* [retval][out] */ __RPC__out void *retval);
        
        /* [id][propget][funcdescattr] */ HRESULT ( STDMETHODCALLTYPE *get_AssemblyKeyContainerName )( 
            __RPC__in VBProjectProperties4 * This,
            /* [retval][out] */ __RPC__deref_out_opt BSTR *retval);
        
        /* [id][propput][funcdescattr] */ HRESULT ( STDMETHODCALLTYPE *put_AssemblyKeyContainerName )( 
            __RPC__in VBProjectProperties4 * This,
            /* [in][idldescattr] */ __RPC__in BSTR noname,
            /* [retval][out] */ __RPC__out void *retval);
        
        /* [id][propget][funcdescattr] */ HRESULT ( STDMETHODCALLTYPE *get_AssemblyOriginatorKeyMode )( 
            __RPC__in VBProjectProperties4 * This,
            /* [retval][out] */ __RPC__out enum prjOriginatorKeyMode *retval);
        
        /* [id][propput][funcdescattr] */ HRESULT ( STDMETHODCALLTYPE *put_AssemblyOriginatorKeyMode )( 
            __RPC__in VBProjectProperties4 * This,
            /* [in][idldescattr] */ enum prjOriginatorKeyMode noname,
            /* [retval][out] */ __RPC__out void *retval);
        
        /* [id][propget][funcdescattr] */ HRESULT ( STDMETHODCALLTYPE *get_DelaySign )( 
            __RPC__in VBProjectProperties4 * This,
            /* [retval][out] */ __RPC__out BOOLEAN *retval);
        
        /* [id][propput][funcdescattr] */ HRESULT ( STDMETHODCALLTYPE *put_DelaySign )( 
            __RPC__in VBProjectProperties4 * This,
            /* [in][idldescattr] */ BOOLEAN noname,
            /* [retval][out] */ __RPC__out void *retval);
        
        /* [id][propget][funcdescattr] */ HRESULT ( STDMETHODCALLTYPE *get_WebServer )( 
            __RPC__in VBProjectProperties4 * This,
            /* [retval][out] */ __RPC__deref_out_opt BSTR *retval);
        
        /* [id][propget][funcdescattr] */ HRESULT ( STDMETHODCALLTYPE *get_WebServerVersion )( 
            __RPC__in VBProjectProperties4 * This,
            /* [retval][out] */ __RPC__deref_out_opt BSTR *retval);
        
        /* [id][propget][funcdescattr] */ HRESULT ( STDMETHODCALLTYPE *get_ServerExtensionsVersion )( 
            __RPC__in VBProjectProperties4 * This,
            /* [retval][out] */ __RPC__deref_out_opt BSTR *retval);
        
        /* [id][propget][funcdescattr] */ HRESULT ( STDMETHODCALLTYPE *get_LinkRepair )( 
            __RPC__in VBProjectProperties4 * This,
            /* [retval][out] */ __RPC__out BOOLEAN *retval);
        
        /* [id][propput][funcdescattr] */ HRESULT ( STDMETHODCALLTYPE *put_LinkRepair )( 
            __RPC__in VBProjectProperties4 * This,
            /* [in][idldescattr] */ BOOLEAN noname,
            /* [retval][out] */ __RPC__out void *retval);
        
        /* [id][propget][funcdescattr] */ HRESULT ( STDMETHODCALLTYPE *get_OfflineURL )( 
            __RPC__in VBProjectProperties4 * This,
            /* [retval][out] */ __RPC__deref_out_opt BSTR *retval);
        
        /* [id][propget][funcdescattr] */ HRESULT ( STDMETHODCALLTYPE *get_FileSharePath )( 
            __RPC__in VBProjectProperties4 * This,
            /* [retval][out] */ __RPC__deref_out_opt BSTR *retval);
        
        /* [id][propput][funcdescattr] */ HRESULT ( STDMETHODCALLTYPE *put_FileSharePath )( 
            __RPC__in VBProjectProperties4 * This,
            /* [in][idldescattr] */ __RPC__in BSTR noname,
            /* [retval][out] */ __RPC__out void *retval);
        
        /* [id][propget][funcdescattr] */ HRESULT ( STDMETHODCALLTYPE *get_ActiveFileSharePath )( 
            __RPC__in VBProjectProperties4 * This,
            /* [retval][out] */ __RPC__deref_out_opt BSTR *retval);
        
        /* [id][propget][funcdescattr] */ HRESULT ( STDMETHODCALLTYPE *get_WebAccessMethod )( 
            __RPC__in VBProjectProperties4 * This,
            /* [retval][out] */ __RPC__out enum prjWebAccessMethod *retval);
        
        /* [id][propput][funcdescattr] */ HRESULT ( STDMETHODCALLTYPE *put_WebAccessMethod )( 
            __RPC__in VBProjectProperties4 * This,
            /* [in][idldescattr] */ enum prjWebAccessMethod noname,
            /* [retval][out] */ __RPC__out void *retval);
        
        /* [id][propget][funcdescattr] */ HRESULT ( STDMETHODCALLTYPE *get_ActiveWebAccessMethod )( 
            __RPC__in VBProjectProperties4 * This,
            /* [retval][out] */ __RPC__out enum prjWebAccessMethod *retval);
        
        /* [id][propget][funcdescattr] */ HRESULT ( STDMETHODCALLTYPE *get_DefaultClientScript )( 
            __RPC__in VBProjectProperties4 * This,
            /* [retval][out] */ __RPC__out enum prjScriptLanguage *retval);
        
        /* [id][propput][funcdescattr] */ HRESULT ( STDMETHODCALLTYPE *put_DefaultClientScript )( 
            __RPC__in VBProjectProperties4 * This,
            /* [in][idldescattr] */ enum prjScriptLanguage noname,
            /* [retval][out] */ __RPC__out void *retval);
        
        /* [id][propget][funcdescattr] */ HRESULT ( STDMETHODCALLTYPE *get_DefaultTargetSchema )( 
            __RPC__in VBProjectProperties4 * This,
            /* [retval][out] */ __RPC__out enum prjTargetSchema *retval);
        
        /* [id][propput][funcdescattr] */ HRESULT ( STDMETHODCALLTYPE *put_DefaultTargetSchema )( 
            __RPC__in VBProjectProperties4 * This,
            /* [in][idldescattr] */ enum prjTargetSchema noname,
            /* [retval][out] */ __RPC__out void *retval);
        
        /* [id][propget][funcdescattr] */ HRESULT ( STDMETHODCALLTYPE *get_DefaultHTMLPageLayout )( 
            __RPC__in VBProjectProperties4 * This,
            /* [retval][out] */ __RPC__out enum prjHTMLPageLayout *retval);
        
        /* [id][propput][funcdescattr] */ HRESULT ( STDMETHODCALLTYPE *put_DefaultHTMLPageLayout )( 
            __RPC__in VBProjectProperties4 * This,
            /* [in][idldescattr] */ enum prjHTMLPageLayout noname,
            /* [retval][out] */ __RPC__out void *retval);
        
        /* [id][propget][funcdescattr] */ HRESULT ( STDMETHODCALLTYPE *get_FileName )( 
            __RPC__in VBProjectProperties4 * This,
            /* [retval][out] */ __RPC__deref_out_opt BSTR *retval);
        
        /* [id][propput][funcdescattr] */ HRESULT ( STDMETHODCALLTYPE *put_FileName )( 
            __RPC__in VBProjectProperties4 * This,
            /* [in][idldescattr] */ __RPC__in BSTR noname,
            /* [retval][out] */ __RPC__out void *retval);
        
        /* [id][propget][funcdescattr] */ HRESULT ( STDMETHODCALLTYPE *get_FullPath )( 
            __RPC__in VBProjectProperties4 * This,
            /* [retval][out] */ __RPC__deref_out_opt BSTR *retval);
        
        /* [id][propget][funcdescattr] */ HRESULT ( STDMETHODCALLTYPE *get_LocalPath )( 
            __RPC__in VBProjectProperties4 * This,
            /* [retval][out] */ __RPC__deref_out_opt BSTR *retval);
        
        /* [id][propget][funcdescattr] */ HRESULT ( STDMETHODCALLTYPE *get_URL )( 
            __RPC__in VBProjectProperties4 * This,
            /* [retval][out] */ __RPC__deref_out_opt BSTR *retval);
        
        /* [id][propget][funcdescattr] */ HRESULT ( STDMETHODCALLTYPE *get_ActiveConfigurationSettings )( 
            __RPC__in VBProjectProperties4 * This,
            /* [retval][out] */ __RPC__deref_out_opt ProjectConfigurationProperties **retval);
        
        /* [id][propget][funcdescattr] */ HRESULT ( STDMETHODCALLTYPE *get_Extender )( 
            __RPC__in VBProjectProperties4 * This,
            /* [in][idldescattr] */ __RPC__in BSTR ExtenderName,
            /* [retval][out] */ __RPC__deref_out_opt IDispatch **retval);
        
        /* [id][propget][funcdescattr] */ HRESULT ( STDMETHODCALLTYPE *get_ExtenderNames )( 
            __RPC__in VBProjectProperties4 * This,
            /* [retval][out] */ __RPC__out VARIANT *retval);
        
        /* [id][propget][funcdescattr] */ HRESULT ( STDMETHODCALLTYPE *get_ExtenderCATID )( 
            __RPC__in VBProjectProperties4 * This,
            /* [retval][out] */ __RPC__deref_out_opt BSTR *retval);
        
        /* [id][propget][funcdescattr] */ HRESULT ( STDMETHODCALLTYPE *get_ApplicationIcon )( 
            __RPC__in VBProjectProperties4 * This,
            /* [retval][out] */ __RPC__deref_out_opt BSTR *retval);
        
        /* [id][propput][funcdescattr] */ HRESULT ( STDMETHODCALLTYPE *put_ApplicationIcon )( 
            __RPC__in VBProjectProperties4 * This,
            /* [in][idldescattr] */ __RPC__in BSTR noname,
            /* [retval][out] */ __RPC__out void *retval);
        
        /* [id][propget][funcdescattr] */ HRESULT ( STDMETHODCALLTYPE *get_OptionStrict )( 
            __RPC__in VBProjectProperties4 * This,
            /* [retval][out] */ __RPC__out enum prjOptionStrict *retval);
        
        /* [id][propput][funcdescattr] */ HRESULT ( STDMETHODCALLTYPE *put_OptionStrict )( 
            __RPC__in VBProjectProperties4 * This,
            /* [in][idldescattr] */ enum prjOptionStrict noname,
            /* [retval][out] */ __RPC__out void *retval);
        
        /* [id][propget][funcdescattr] */ HRESULT ( STDMETHODCALLTYPE *get_ReferencePath )( 
            __RPC__in VBProjectProperties4 * This,
            /* [retval][out] */ __RPC__deref_out_opt BSTR *retval);
        
        /* [id][propput][funcdescattr] */ HRESULT ( STDMETHODCALLTYPE *put_ReferencePath )( 
            __RPC__in VBProjectProperties4 * This,
            /* [in][idldescattr] */ __RPC__in BSTR noname,
            /* [retval][out] */ __RPC__out void *retval);
        
        /* [id][propget][funcdescattr] */ HRESULT ( STDMETHODCALLTYPE *get_OutputFileName )( 
            __RPC__in VBProjectProperties4 * This,
            /* [retval][out] */ __RPC__deref_out_opt BSTR *retval);
        
        /* [id][propget][hidden][funcdescattr] */ HRESULT ( STDMETHODCALLTYPE *get_AbsoluteProjectDirectory )( 
            __RPC__in VBProjectProperties4 * This,
            /* [retval][out] */ __RPC__deref_out_opt BSTR *retval);
        
        /* [id][propget][funcdescattr] */ HRESULT ( STDMETHODCALLTYPE *get_OptionExplicit )( 
            __RPC__in VBProjectProperties4 * This,
            /* [retval][out] */ __RPC__out enum prjOptionExplicit *retval);
        
        /* [id][propput][funcdescattr] */ HRESULT ( STDMETHODCALLTYPE *put_OptionExplicit )( 
            __RPC__in VBProjectProperties4 * This,
            /* [in][idldescattr] */ enum prjOptionExplicit noname,
            /* [retval][out] */ __RPC__out void *retval);
        
        /* [id][propget][funcdescattr] */ HRESULT ( STDMETHODCALLTYPE *get_OptionCompare )( 
            __RPC__in VBProjectProperties4 * This,
            /* [retval][out] */ __RPC__out enum prjCompare *retval);
        
        /* [id][propput][funcdescattr] */ HRESULT ( STDMETHODCALLTYPE *put_OptionCompare )( 
            __RPC__in VBProjectProperties4 * This,
            /* [in][idldescattr] */ enum prjCompare noname,
            /* [retval][out] */ __RPC__out void *retval);
        
        /* [id][propget][funcdescattr] */ HRESULT ( STDMETHODCALLTYPE *get_ProjectType )( 
            __RPC__in VBProjectProperties4 * This,
            /* [retval][out] */ __RPC__out enum prjProjectType *retval);
        
        /* [id][propget][funcdescattr] */ HRESULT ( STDMETHODCALLTYPE *get_DefaultNamespace )( 
            __RPC__in VBProjectProperties4 * This,
            /* [retval][out] */ __RPC__deref_out_opt BSTR *retval);
        
        /* [id][propput][funcdescattr] */ HRESULT ( STDMETHODCALLTYPE *put_DefaultNamespace )( 
            __RPC__in VBProjectProperties4 * This,
            /* [in][idldescattr] */ __RPC__in BSTR noname,
            /* [retval][out] */ __RPC__out void *retval);
        
        /* [id][propget][funcdescattr] */ HRESULT ( STDMETHODCALLTYPE *get_PreBuildEvent )( 
            __RPC__in VBProjectProperties4 * This,
            /* [retval][out] */ __RPC__deref_out_opt BSTR *retval);
        
        /* [id][propput][funcdescattr] */ HRESULT ( STDMETHODCALLTYPE *put_PreBuildEvent )( 
            __RPC__in VBProjectProperties4 * This,
            /* [in][idldescattr] */ __RPC__in BSTR noname,
            /* [retval][out] */ __RPC__out void *retval);
        
        /* [id][propget][funcdescattr] */ HRESULT ( STDMETHODCALLTYPE *get_PostBuildEvent )( 
            __RPC__in VBProjectProperties4 * This,
            /* [retval][out] */ __RPC__deref_out_opt BSTR *retval);
        
        /* [id][propput][funcdescattr] */ HRESULT ( STDMETHODCALLTYPE *put_PostBuildEvent )( 
            __RPC__in VBProjectProperties4 * This,
            /* [in][idldescattr] */ __RPC__in BSTR noname,
            /* [retval][out] */ __RPC__out void *retval);
        
        /* [id][propget][funcdescattr] */ HRESULT ( STDMETHODCALLTYPE *get_RunPostBuildEvent )( 
            __RPC__in VBProjectProperties4 * This,
            /* [retval][out] */ __RPC__out enum prjRunPostBuildEvent *retval);
        
        /* [id][propput][funcdescattr] */ HRESULT ( STDMETHODCALLTYPE *put_RunPostBuildEvent )( 
            __RPC__in VBProjectProperties4 * This,
            /* [in][idldescattr] */ enum prjRunPostBuildEvent noname,
            /* [retval][out] */ __RPC__out void *retval);
        
        /* [id][propget][funcdescattr] */ HRESULT ( STDMETHODCALLTYPE *get_AspnetVersion )( 
            __RPC__in VBProjectProperties4 * This,
            /* [retval][out] */ __RPC__deref_out_opt BSTR *retval);
        
        /* [id][propget][funcdescattr] */ HRESULT ( STDMETHODCALLTYPE *get_Title )( 
            __RPC__in VBProjectProperties4 * This,
            /* [retval][out] */ __RPC__deref_out_opt BSTR *retval);
        
        /* [id][propput][funcdescattr] */ HRESULT ( STDMETHODCALLTYPE *put_Title )( 
            __RPC__in VBProjectProperties4 * This,
            /* [in][idldescattr] */ __RPC__in BSTR noname,
            /* [retval][out] */ __RPC__out void *retval);
        
        /* [id][propget][funcdescattr] */ HRESULT ( STDMETHODCALLTYPE *get_Description )( 
            __RPC__in VBProjectProperties4 * This,
            /* [retval][out] */ __RPC__deref_out_opt BSTR *retval);
        
        /* [id][propput][funcdescattr] */ HRESULT ( STDMETHODCALLTYPE *put_Description )( 
            __RPC__in VBProjectProperties4 * This,
            /* [in][idldescattr] */ __RPC__in BSTR noname,
            /* [retval][out] */ __RPC__out void *retval);
        
        /* [id][propget][funcdescattr] */ HRESULT ( STDMETHODCALLTYPE *get_Company )( 
            __RPC__in VBProjectProperties4 * This,
            /* [retval][out] */ __RPC__deref_out_opt BSTR *retval);
        
        /* [id][propput][funcdescattr] */ HRESULT ( STDMETHODCALLTYPE *put_Company )( 
            __RPC__in VBProjectProperties4 * This,
            /* [in][idldescattr] */ __RPC__in BSTR noname,
            /* [retval][out] */ __RPC__out void *retval);
        
        /* [id][propget][funcdescattr] */ HRESULT ( STDMETHODCALLTYPE *get_Product )( 
            __RPC__in VBProjectProperties4 * This,
            /* [retval][out] */ __RPC__deref_out_opt BSTR *retval);
        
        /* [id][propput][funcdescattr] */ HRESULT ( STDMETHODCALLTYPE *put_Product )( 
            __RPC__in VBProjectProperties4 * This,
            /* [in][idldescattr] */ __RPC__in BSTR noname,
            /* [retval][out] */ __RPC__out void *retval);
        
        /* [id][propget][funcdescattr] */ HRESULT ( STDMETHODCALLTYPE *get_Copyright )( 
            __RPC__in VBProjectProperties4 * This,
            /* [retval][out] */ __RPC__deref_out_opt BSTR *retval);
        
        /* [id][propput][funcdescattr] */ HRESULT ( STDMETHODCALLTYPE *put_Copyright )( 
            __RPC__in VBProjectProperties4 * This,
            /* [in][idldescattr] */ __RPC__in BSTR noname,
            /* [retval][out] */ __RPC__out void *retval);
        
        /* [id][propget][funcdescattr] */ HRESULT ( STDMETHODCALLTYPE *get_Trademark )( 
            __RPC__in VBProjectProperties4 * This,
            /* [retval][out] */ __RPC__deref_out_opt BSTR *retval);
        
        /* [id][propput][funcdescattr] */ HRESULT ( STDMETHODCALLTYPE *put_Trademark )( 
            __RPC__in VBProjectProperties4 * This,
            /* [in][idldescattr] */ __RPC__in BSTR noname,
            /* [retval][out] */ __RPC__out void *retval);
        
        /* [id][propget][funcdescattr] */ HRESULT ( STDMETHODCALLTYPE *get_AssemblyType )( 
            __RPC__in VBProjectProperties4 * This,
            /* [retval][out] */ __RPC__out enum prjAssemblyType *retval);
        
        /* [id][propput][funcdescattr] */ HRESULT ( STDMETHODCALLTYPE *put_AssemblyType )( 
            __RPC__in VBProjectProperties4 * This,
            /* [in][idldescattr] */ enum prjAssemblyType noname,
            /* [retval][out] */ __RPC__out void *retval);
        
        /* [id][propget][funcdescattr] */ HRESULT ( STDMETHODCALLTYPE *get_TypeComplianceDiagnostics )( 
            __RPC__in VBProjectProperties4 * This,
            /* [retval][out] */ __RPC__out BOOLEAN *retval);
        
        /* [id][propput][funcdescattr] */ HRESULT ( STDMETHODCALLTYPE *put_TypeComplianceDiagnostics )( 
            __RPC__in VBProjectProperties4 * This,
            /* [in][idldescattr] */ BOOLEAN noname,
            /* [retval][out] */ __RPC__out void *retval);
        
        /* [id][propget][funcdescattr] */ HRESULT ( STDMETHODCALLTYPE *get_Win32ResourceFile )( 
            __RPC__in VBProjectProperties4 * This,
            /* [retval][out] */ __RPC__deref_out_opt BSTR *retval);
        
        /* [id][propput][funcdescattr] */ HRESULT ( STDMETHODCALLTYPE *put_Win32ResourceFile )( 
            __RPC__in VBProjectProperties4 * This,
            /* [in][idldescattr] */ __RPC__in BSTR noname,
            /* [retval][out] */ __RPC__out void *retval);
        
        /* [id][propget][funcdescattr] */ HRESULT ( STDMETHODCALLTYPE *get_AssemblyKeyProviderName )( 
            __RPC__in VBProjectProperties4 * This,
            /* [retval][out] */ __RPC__deref_out_opt BSTR *retval);
        
        /* [id][propput][funcdescattr] */ HRESULT ( STDMETHODCALLTYPE *put_AssemblyKeyProviderName )( 
            __RPC__in VBProjectProperties4 * This,
            /* [in][idldescattr] */ __RPC__in BSTR noname,
            /* [retval][out] */ __RPC__out void *retval);
        
        /* [id][propget][funcdescattr] */ HRESULT ( STDMETHODCALLTYPE *get_AssemblyOriginatorKeyFileType )( 
            __RPC__in VBProjectProperties4 * This,
            /* [retval][out] */ __RPC__out unsigned long *retval);
        
        /* [id][propput][funcdescattr] */ HRESULT ( STDMETHODCALLTYPE *put_AssemblyOriginatorKeyFileType )( 
            __RPC__in VBProjectProperties4 * This,
            /* [in][idldescattr] */ unsigned long noname,
            /* [retval][out] */ __RPC__out void *retval);
        
        /* [id][propget][funcdescattr] */ HRESULT ( STDMETHODCALLTYPE *get_AssemblyVersion )( 
            __RPC__in VBProjectProperties4 * This,
            /* [retval][out] */ __RPC__deref_out_opt BSTR *retval);
        
        /* [id][propput][funcdescattr] */ HRESULT ( STDMETHODCALLTYPE *put_AssemblyVersion )( 
            __RPC__in VBProjectProperties4 * This,
            /* [in][idldescattr] */ __RPC__in BSTR noname,
            /* [retval][out] */ __RPC__out void *retval);
        
        /* [id][propget][funcdescattr] */ HRESULT ( STDMETHODCALLTYPE *get_AssemblyFileVersion )( 
            __RPC__in VBProjectProperties4 * This,
            /* [retval][out] */ __RPC__deref_out_opt BSTR *retval);
        
        /* [id][propput][funcdescattr] */ HRESULT ( STDMETHODCALLTYPE *put_AssemblyFileVersion )( 
            __RPC__in VBProjectProperties4 * This,
            /* [in][idldescattr] */ __RPC__in BSTR noname,
            /* [retval][out] */ __RPC__out void *retval);
        
        /* [id][propget][funcdescattr] */ HRESULT ( STDMETHODCALLTYPE *get_GenerateManifests )( 
            __RPC__in VBProjectProperties4 * This,
            /* [retval][out] */ __RPC__out BOOLEAN *retval);
        
        /* [id][propput][funcdescattr] */ HRESULT ( STDMETHODCALLTYPE *put_GenerateManifests )( 
            __RPC__in VBProjectProperties4 * This,
            /* [in][idldescattr] */ BOOLEAN noname,
            /* [retval][out] */ __RPC__out void *retval);
        
        /* [id][propget][funcdescattr] */ HRESULT ( STDMETHODCALLTYPE *get_EnableSecurityDebugging )( 
            __RPC__in VBProjectProperties4 * This,
            /* [retval][out] */ __RPC__out BOOLEAN *retval);
        
        /* [id][propput][funcdescattr] */ HRESULT ( STDMETHODCALLTYPE *put_EnableSecurityDebugging )( 
            __RPC__in VBProjectProperties4 * This,
            /* [in][idldescattr] */ BOOLEAN noname,
            /* [retval][out] */ __RPC__out void *retval);
        
        /* [id][propget][funcdescattr] */ HRESULT ( STDMETHODCALLTYPE *get_DebugSecurityZoneURL )( 
            __RPC__in VBProjectProperties4 * This,
            /* [retval][out] */ __RPC__deref_out_opt BSTR *retval);
        
        /* [id][propput][funcdescattr] */ HRESULT ( STDMETHODCALLTYPE *put_DebugSecurityZoneURL )( 
            __RPC__in VBProjectProperties4 * This,
            /* [in][idldescattr] */ __RPC__in BSTR noname,
            /* [retval][out] */ __RPC__out void *retval);
        
        /* [id][propget][funcdescattr] */ HRESULT ( STDMETHODCALLTYPE *get_Publish )( 
            __RPC__in VBProjectProperties4 * This,
            /* [retval][out] */ __RPC__deref_out_opt IDispatch **retval);
        
        /* [id][propget][funcdescattr] */ HRESULT ( STDMETHODCALLTYPE *get_ComVisible )( 
            __RPC__in VBProjectProperties4 * This,
            /* [retval][out] */ __RPC__out BOOLEAN *retval);
        
        /* [id][propput][funcdescattr] */ HRESULT ( STDMETHODCALLTYPE *put_ComVisible )( 
            __RPC__in VBProjectProperties4 * This,
            /* [in][idldescattr] */ BOOLEAN noname,
            /* [retval][out] */ __RPC__out void *retval);
        
        /* [id][propget][funcdescattr] */ HRESULT ( STDMETHODCALLTYPE *get_AssemblyGuid )( 
            __RPC__in VBProjectProperties4 * This,
            /* [retval][out] */ __RPC__deref_out_opt BSTR *retval);
        
        /* [id][propput][funcdescattr] */ HRESULT ( STDMETHODCALLTYPE *put_AssemblyGuid )( 
            __RPC__in VBProjectProperties4 * This,
            /* [in][idldescattr] */ __RPC__in BSTR noname,
            /* [retval][out] */ __RPC__out void *retval);
        
        /* [id][propget][funcdescattr] */ HRESULT ( STDMETHODCALLTYPE *get_NeutralResourcesLanguage )( 
            __RPC__in VBProjectProperties4 * This,
            /* [retval][out] */ __RPC__deref_out_opt BSTR *retval);
        
        /* [id][propput][funcdescattr] */ HRESULT ( STDMETHODCALLTYPE *put_NeutralResourcesLanguage )( 
            __RPC__in VBProjectProperties4 * This,
            /* [in][idldescattr] */ __RPC__in BSTR noname,
            /* [retval][out] */ __RPC__out void *retval);
        
        /* [id][propget][funcdescattr] */ HRESULT ( STDMETHODCALLTYPE *get_SignAssembly )( 
            __RPC__in VBProjectProperties4 * This,
            /* [retval][out] */ __RPC__out BOOLEAN *retval);
        
        /* [id][propput][funcdescattr] */ HRESULT ( STDMETHODCALLTYPE *put_SignAssembly )( 
            __RPC__in VBProjectProperties4 * This,
            /* [in][idldescattr] */ BOOLEAN noname,
            /* [retval][out] */ __RPC__out void *retval);
        
        /* [id][propget][funcdescattr] */ HRESULT ( STDMETHODCALLTYPE *get_SignManifests )( 
            __RPC__in VBProjectProperties4 * This,
            /* [retval][out] */ __RPC__out BOOLEAN *retval);
        
        /* [id][propput][funcdescattr] */ HRESULT ( STDMETHODCALLTYPE *put_SignManifests )( 
            __RPC__in VBProjectProperties4 * This,
            /* [in][idldescattr] */ BOOLEAN noname,
            /* [retval][out] */ __RPC__out void *retval);
        
        /* [id][propget][funcdescattr] */ HRESULT ( STDMETHODCALLTYPE *get_TargetZone )( 
            __RPC__in VBProjectProperties4 * This,
            /* [retval][out] */ __RPC__deref_out_opt BSTR *retval);
        
        /* [id][propput][funcdescattr] */ HRESULT ( STDMETHODCALLTYPE *put_TargetZone )( 
            __RPC__in VBProjectProperties4 * This,
            /* [in][idldescattr] */ __RPC__in BSTR noname,
            /* [retval][out] */ __RPC__out void *retval);
        
        /* [id][propget][funcdescattr] */ HRESULT ( STDMETHODCALLTYPE *get_ExcludedPermissions )( 
            __RPC__in VBProjectProperties4 * This,
            /* [retval][out] */ __RPC__deref_out_opt BSTR *retval);
        
        /* [id][propput][funcdescattr] */ HRESULT ( STDMETHODCALLTYPE *put_ExcludedPermissions )( 
            __RPC__in VBProjectProperties4 * This,
            /* [in][idldescattr] */ __RPC__in BSTR noname,
            /* [retval][out] */ __RPC__out void *retval);
        
        /* [id][propget][funcdescattr] */ HRESULT ( STDMETHODCALLTYPE *get_ManifestCertificateThumbprint )( 
            __RPC__in VBProjectProperties4 * This,
            /* [retval][out] */ __RPC__deref_out_opt BSTR *retval);
        
        /* [id][propput][funcdescattr] */ HRESULT ( STDMETHODCALLTYPE *put_ManifestCertificateThumbprint )( 
            __RPC__in VBProjectProperties4 * This,
            /* [in][idldescattr] */ __RPC__in BSTR noname,
            /* [retval][out] */ __RPC__out void *retval);
        
        /* [id][propget][funcdescattr] */ HRESULT ( STDMETHODCALLTYPE *get_ManifestKeyFile )( 
            __RPC__in VBProjectProperties4 * This,
            /* [retval][out] */ __RPC__deref_out_opt BSTR *retval);
        
        /* [id][propput][funcdescattr] */ HRESULT ( STDMETHODCALLTYPE *put_ManifestKeyFile )( 
            __RPC__in VBProjectProperties4 * This,
            /* [in][idldescattr] */ __RPC__in BSTR noname,
            /* [retval][out] */ __RPC__out void *retval);
        
        /* [id][propget][funcdescattr] */ HRESULT ( STDMETHODCALLTYPE *get_ManifestTimestampUrl )( 
            __RPC__in VBProjectProperties4 * This,
            /* [retval][out] */ __RPC__deref_out_opt BSTR *retval);
        
        /* [id][propput][funcdescattr] */ HRESULT ( STDMETHODCALLTYPE *put_ManifestTimestampUrl )( 
            __RPC__in VBProjectProperties4 * This,
            /* [in][idldescattr] */ __RPC__in BSTR noname,
            /* [retval][out] */ __RPC__out void *retval);
        
        /* [id][propget][funcdescattr] */ HRESULT ( STDMETHODCALLTYPE *get_MyApplication )( 
            __RPC__in VBProjectProperties4 * This,
            /* [retval][out] */ __RPC__deref_out_opt IDispatch **retval);
        
        /* [id][propget][funcdescattr] */ HRESULT ( STDMETHODCALLTYPE *get_MyType )( 
            __RPC__in VBProjectProperties4 * This,
            /* [retval][out] */ __RPC__deref_out_opt BSTR *retval);
        
        /* [id][propput][funcdescattr] */ HRESULT ( STDMETHODCALLTYPE *put_MyType )( 
            __RPC__in VBProjectProperties4 * This,
            /* [in][idldescattr] */ __RPC__in BSTR noname,
            /* [retval][out] */ __RPC__out void *retval);
        
        /* [helpstring][id][nonbrowsable][propget] */ HRESULT ( STDMETHODCALLTYPE *get_TargetFramework )( 
            __RPC__in VBProjectProperties4 * This,
            /* [retval][out] */ __RPC__out DWORD *pdwTargetFramework);
        
        /* [helpstring][id][nonbrowsable][propput] */ HRESULT ( STDMETHODCALLTYPE *put_TargetFramework )( 
            __RPC__in VBProjectProperties4 * This,
            /* [in] */ DWORD dwTargetFramework);
        
        /* [helpstring][id][propget] */ HRESULT ( STDMETHODCALLTYPE *get_ApplicationManifest )( 
            __RPC__in VBProjectProperties4 * This,
            /* [retval][out] */ __RPC__deref_out_opt BSTR *pbstrApplicationManifest);
        
        /* [helpstring][id][propput] */ HRESULT ( STDMETHODCALLTYPE *put_ApplicationManifest )( 
            __RPC__in VBProjectProperties4 * This,
            /* [in] */ __RPC__in BSTR ApplicationManifest);
        
        /* [helpstring][id][propget] */ HRESULT ( STDMETHODCALLTYPE *get_OptionInfer )( 
            __RPC__in VBProjectProperties4 * This,
            /* [retval][out] */ __RPC__out prjOptionInfer *pOptionInfer);
        
        /* [helpstring][id][propput] */ HRESULT ( STDMETHODCALLTYPE *put_OptionInfer )( 
            __RPC__in VBProjectProperties4 * This,
            /* [in] */ prjOptionInfer optionInfer);
        
        END_INTERFACE
    } VBProjectProperties4Vtbl;

    interface VBProjectProperties4
    {
        CONST_VTBL struct VBProjectProperties4Vtbl *lpVtbl;
    };

    

#ifdef COBJMACROS


#define VBProjectProperties4_QueryInterface(This,riid,ppvObj,retval)	\
    ( (This)->lpVtbl -> QueryInterface(This,riid,ppvObj,retval) ) 

#define VBProjectProperties4_AddRef(This,retval)	\
    ( (This)->lpVtbl -> AddRef(This,retval) ) 

#define VBProjectProperties4_Release(This,retval)	\
    ( (This)->lpVtbl -> Release(This,retval) ) 

#define VBProjectProperties4_GetTypeInfoCount(This,pctinfo,retval)	\
    ( (This)->lpVtbl -> GetTypeInfoCount(This,pctinfo,retval) ) 

#define VBProjectProperties4_GetTypeInfo(This,itinfo,lcid,pptinfo,retval)	\
    ( (This)->lpVtbl -> GetTypeInfo(This,itinfo,lcid,pptinfo,retval) ) 

#define VBProjectProperties4_GetIDsOfNames(This,riid,rgszNames,cNames,lcid,rgdispid,retval)	\
    ( (This)->lpVtbl -> GetIDsOfNames(This,riid,rgszNames,cNames,lcid,rgdispid,retval) ) 

#define VBProjectProperties4_Invoke(This,dispidMember,riid,lcid,wFlags,pdispparams,pvarResult,pexcepinfo,puArgErr,retval)	\
    ( (This)->lpVtbl -> Invoke(This,dispidMember,riid,lcid,wFlags,pdispparams,pvarResult,pexcepinfo,puArgErr,retval) ) 

#define VBProjectProperties4_get___id(This,retval)	\
    ( (This)->lpVtbl -> get___id(This,retval) ) 

#define VBProjectProperties4_get___project(This,retval)	\
    ( (This)->lpVtbl -> get___project(This,retval) ) 

#define VBProjectProperties4_get_StartupObject(This,retval)	\
    ( (This)->lpVtbl -> get_StartupObject(This,retval) ) 

#define VBProjectProperties4_put_StartupObject(This,noname,retval)	\
    ( (This)->lpVtbl -> put_StartupObject(This,noname,retval) ) 

#define VBProjectProperties4_get_OutputType(This,retval)	\
    ( (This)->lpVtbl -> get_OutputType(This,retval) ) 

#define VBProjectProperties4_put_OutputType(This,noname,retval)	\
    ( (This)->lpVtbl -> put_OutputType(This,noname,retval) ) 

#define VBProjectProperties4_get_RootNamespace(This,retval)	\
    ( (This)->lpVtbl -> get_RootNamespace(This,retval) ) 

#define VBProjectProperties4_put_RootNamespace(This,noname,retval)	\
    ( (This)->lpVtbl -> put_RootNamespace(This,noname,retval) ) 

#define VBProjectProperties4_get_AssemblyName(This,retval)	\
    ( (This)->lpVtbl -> get_AssemblyName(This,retval) ) 

#define VBProjectProperties4_put_AssemblyName(This,noname,retval)	\
    ( (This)->lpVtbl -> put_AssemblyName(This,noname,retval) ) 

#define VBProjectProperties4_get_AssemblyOriginatorKeyFile(This,retval)	\
    ( (This)->lpVtbl -> get_AssemblyOriginatorKeyFile(This,retval) ) 

#define VBProjectProperties4_put_AssemblyOriginatorKeyFile(This,noname,retval)	\
    ( (This)->lpVtbl -> put_AssemblyOriginatorKeyFile(This,noname,retval) ) 

#define VBProjectProperties4_get_AssemblyKeyContainerName(This,retval)	\
    ( (This)->lpVtbl -> get_AssemblyKeyContainerName(This,retval) ) 

#define VBProjectProperties4_put_AssemblyKeyContainerName(This,noname,retval)	\
    ( (This)->lpVtbl -> put_AssemblyKeyContainerName(This,noname,retval) ) 

#define VBProjectProperties4_get_AssemblyOriginatorKeyMode(This,retval)	\
    ( (This)->lpVtbl -> get_AssemblyOriginatorKeyMode(This,retval) ) 

#define VBProjectProperties4_put_AssemblyOriginatorKeyMode(This,noname,retval)	\
    ( (This)->lpVtbl -> put_AssemblyOriginatorKeyMode(This,noname,retval) ) 

#define VBProjectProperties4_get_DelaySign(This,retval)	\
    ( (This)->lpVtbl -> get_DelaySign(This,retval) ) 

#define VBProjectProperties4_put_DelaySign(This,noname,retval)	\
    ( (This)->lpVtbl -> put_DelaySign(This,noname,retval) ) 

#define VBProjectProperties4_get_WebServer(This,retval)	\
    ( (This)->lpVtbl -> get_WebServer(This,retval) ) 

#define VBProjectProperties4_get_WebServerVersion(This,retval)	\
    ( (This)->lpVtbl -> get_WebServerVersion(This,retval) ) 

#define VBProjectProperties4_get_ServerExtensionsVersion(This,retval)	\
    ( (This)->lpVtbl -> get_ServerExtensionsVersion(This,retval) ) 

#define VBProjectProperties4_get_LinkRepair(This,retval)	\
    ( (This)->lpVtbl -> get_LinkRepair(This,retval) ) 

#define VBProjectProperties4_put_LinkRepair(This,noname,retval)	\
    ( (This)->lpVtbl -> put_LinkRepair(This,noname,retval) ) 

#define VBProjectProperties4_get_OfflineURL(This,retval)	\
    ( (This)->lpVtbl -> get_OfflineURL(This,retval) ) 

#define VBProjectProperties4_get_FileSharePath(This,retval)	\
    ( (This)->lpVtbl -> get_FileSharePath(This,retval) ) 

#define VBProjectProperties4_put_FileSharePath(This,noname,retval)	\
    ( (This)->lpVtbl -> put_FileSharePath(This,noname,retval) ) 

#define VBProjectProperties4_get_ActiveFileSharePath(This,retval)	\
    ( (This)->lpVtbl -> get_ActiveFileSharePath(This,retval) ) 

#define VBProjectProperties4_get_WebAccessMethod(This,retval)	\
    ( (This)->lpVtbl -> get_WebAccessMethod(This,retval) ) 

#define VBProjectProperties4_put_WebAccessMethod(This,noname,retval)	\
    ( (This)->lpVtbl -> put_WebAccessMethod(This,noname,retval) ) 

#define VBProjectProperties4_get_ActiveWebAccessMethod(This,retval)	\
    ( (This)->lpVtbl -> get_ActiveWebAccessMethod(This,retval) ) 

#define VBProjectProperties4_get_DefaultClientScript(This,retval)	\
    ( (This)->lpVtbl -> get_DefaultClientScript(This,retval) ) 

#define VBProjectProperties4_put_DefaultClientScript(This,noname,retval)	\
    ( (This)->lpVtbl -> put_DefaultClientScript(This,noname,retval) ) 

#define VBProjectProperties4_get_DefaultTargetSchema(This,retval)	\
    ( (This)->lpVtbl -> get_DefaultTargetSchema(This,retval) ) 

#define VBProjectProperties4_put_DefaultTargetSchema(This,noname,retval)	\
    ( (This)->lpVtbl -> put_DefaultTargetSchema(This,noname,retval) ) 

#define VBProjectProperties4_get_DefaultHTMLPageLayout(This,retval)	\
    ( (This)->lpVtbl -> get_DefaultHTMLPageLayout(This,retval) ) 

#define VBProjectProperties4_put_DefaultHTMLPageLayout(This,noname,retval)	\
    ( (This)->lpVtbl -> put_DefaultHTMLPageLayout(This,noname,retval) ) 

#define VBProjectProperties4_get_FileName(This,retval)	\
    ( (This)->lpVtbl -> get_FileName(This,retval) ) 

#define VBProjectProperties4_put_FileName(This,noname,retval)	\
    ( (This)->lpVtbl -> put_FileName(This,noname,retval) ) 

#define VBProjectProperties4_get_FullPath(This,retval)	\
    ( (This)->lpVtbl -> get_FullPath(This,retval) ) 

#define VBProjectProperties4_get_LocalPath(This,retval)	\
    ( (This)->lpVtbl -> get_LocalPath(This,retval) ) 

#define VBProjectProperties4_get_URL(This,retval)	\
    ( (This)->lpVtbl -> get_URL(This,retval) ) 

#define VBProjectProperties4_get_ActiveConfigurationSettings(This,retval)	\
    ( (This)->lpVtbl -> get_ActiveConfigurationSettings(This,retval) ) 

#define VBProjectProperties4_get_Extender(This,ExtenderName,retval)	\
    ( (This)->lpVtbl -> get_Extender(This,ExtenderName,retval) ) 

#define VBProjectProperties4_get_ExtenderNames(This,retval)	\
    ( (This)->lpVtbl -> get_ExtenderNames(This,retval) ) 

#define VBProjectProperties4_get_ExtenderCATID(This,retval)	\
    ( (This)->lpVtbl -> get_ExtenderCATID(This,retval) ) 

#define VBProjectProperties4_get_ApplicationIcon(This,retval)	\
    ( (This)->lpVtbl -> get_ApplicationIcon(This,retval) ) 

#define VBProjectProperties4_put_ApplicationIcon(This,noname,retval)	\
    ( (This)->lpVtbl -> put_ApplicationIcon(This,noname,retval) ) 

#define VBProjectProperties4_get_OptionStrict(This,retval)	\
    ( (This)->lpVtbl -> get_OptionStrict(This,retval) ) 

#define VBProjectProperties4_put_OptionStrict(This,noname,retval)	\
    ( (This)->lpVtbl -> put_OptionStrict(This,noname,retval) ) 

#define VBProjectProperties4_get_ReferencePath(This,retval)	\
    ( (This)->lpVtbl -> get_ReferencePath(This,retval) ) 

#define VBProjectProperties4_put_ReferencePath(This,noname,retval)	\
    ( (This)->lpVtbl -> put_ReferencePath(This,noname,retval) ) 

#define VBProjectProperties4_get_OutputFileName(This,retval)	\
    ( (This)->lpVtbl -> get_OutputFileName(This,retval) ) 

#define VBProjectProperties4_get_AbsoluteProjectDirectory(This,retval)	\
    ( (This)->lpVtbl -> get_AbsoluteProjectDirectory(This,retval) ) 

#define VBProjectProperties4_get_OptionExplicit(This,retval)	\
    ( (This)->lpVtbl -> get_OptionExplicit(This,retval) ) 

#define VBProjectProperties4_put_OptionExplicit(This,noname,retval)	\
    ( (This)->lpVtbl -> put_OptionExplicit(This,noname,retval) ) 

#define VBProjectProperties4_get_OptionCompare(This,retval)	\
    ( (This)->lpVtbl -> get_OptionCompare(This,retval) ) 

#define VBProjectProperties4_put_OptionCompare(This,noname,retval)	\
    ( (This)->lpVtbl -> put_OptionCompare(This,noname,retval) ) 

#define VBProjectProperties4_get_ProjectType(This,retval)	\
    ( (This)->lpVtbl -> get_ProjectType(This,retval) ) 

#define VBProjectProperties4_get_DefaultNamespace(This,retval)	\
    ( (This)->lpVtbl -> get_DefaultNamespace(This,retval) ) 

#define VBProjectProperties4_put_DefaultNamespace(This,noname,retval)	\
    ( (This)->lpVtbl -> put_DefaultNamespace(This,noname,retval) ) 

#define VBProjectProperties4_get_PreBuildEvent(This,retval)	\
    ( (This)->lpVtbl -> get_PreBuildEvent(This,retval) ) 

#define VBProjectProperties4_put_PreBuildEvent(This,noname,retval)	\
    ( (This)->lpVtbl -> put_PreBuildEvent(This,noname,retval) ) 

#define VBProjectProperties4_get_PostBuildEvent(This,retval)	\
    ( (This)->lpVtbl -> get_PostBuildEvent(This,retval) ) 

#define VBProjectProperties4_put_PostBuildEvent(This,noname,retval)	\
    ( (This)->lpVtbl -> put_PostBuildEvent(This,noname,retval) ) 

#define VBProjectProperties4_get_RunPostBuildEvent(This,retval)	\
    ( (This)->lpVtbl -> get_RunPostBuildEvent(This,retval) ) 

#define VBProjectProperties4_put_RunPostBuildEvent(This,noname,retval)	\
    ( (This)->lpVtbl -> put_RunPostBuildEvent(This,noname,retval) ) 

#define VBProjectProperties4_get_AspnetVersion(This,retval)	\
    ( (This)->lpVtbl -> get_AspnetVersion(This,retval) ) 

#define VBProjectProperties4_get_Title(This,retval)	\
    ( (This)->lpVtbl -> get_Title(This,retval) ) 

#define VBProjectProperties4_put_Title(This,noname,retval)	\
    ( (This)->lpVtbl -> put_Title(This,noname,retval) ) 

#define VBProjectProperties4_get_Description(This,retval)	\
    ( (This)->lpVtbl -> get_Description(This,retval) ) 

#define VBProjectProperties4_put_Description(This,noname,retval)	\
    ( (This)->lpVtbl -> put_Description(This,noname,retval) ) 

#define VBProjectProperties4_get_Company(This,retval)	\
    ( (This)->lpVtbl -> get_Company(This,retval) ) 

#define VBProjectProperties4_put_Company(This,noname,retval)	\
    ( (This)->lpVtbl -> put_Company(This,noname,retval) ) 

#define VBProjectProperties4_get_Product(This,retval)	\
    ( (This)->lpVtbl -> get_Product(This,retval) ) 

#define VBProjectProperties4_put_Product(This,noname,retval)	\
    ( (This)->lpVtbl -> put_Product(This,noname,retval) ) 

#define VBProjectProperties4_get_Copyright(This,retval)	\
    ( (This)->lpVtbl -> get_Copyright(This,retval) ) 

#define VBProjectProperties4_put_Copyright(This,noname,retval)	\
    ( (This)->lpVtbl -> put_Copyright(This,noname,retval) ) 

#define VBProjectProperties4_get_Trademark(This,retval)	\
    ( (This)->lpVtbl -> get_Trademark(This,retval) ) 

#define VBProjectProperties4_put_Trademark(This,noname,retval)	\
    ( (This)->lpVtbl -> put_Trademark(This,noname,retval) ) 

#define VBProjectProperties4_get_AssemblyType(This,retval)	\
    ( (This)->lpVtbl -> get_AssemblyType(This,retval) ) 

#define VBProjectProperties4_put_AssemblyType(This,noname,retval)	\
    ( (This)->lpVtbl -> put_AssemblyType(This,noname,retval) ) 

#define VBProjectProperties4_get_TypeComplianceDiagnostics(This,retval)	\
    ( (This)->lpVtbl -> get_TypeComplianceDiagnostics(This,retval) ) 

#define VBProjectProperties4_put_TypeComplianceDiagnostics(This,noname,retval)	\
    ( (This)->lpVtbl -> put_TypeComplianceDiagnostics(This,noname,retval) ) 

#define VBProjectProperties4_get_Win32ResourceFile(This,retval)	\
    ( (This)->lpVtbl -> get_Win32ResourceFile(This,retval) ) 

#define VBProjectProperties4_put_Win32ResourceFile(This,noname,retval)	\
    ( (This)->lpVtbl -> put_Win32ResourceFile(This,noname,retval) ) 

#define VBProjectProperties4_get_AssemblyKeyProviderName(This,retval)	\
    ( (This)->lpVtbl -> get_AssemblyKeyProviderName(This,retval) ) 

#define VBProjectProperties4_put_AssemblyKeyProviderName(This,noname,retval)	\
    ( (This)->lpVtbl -> put_AssemblyKeyProviderName(This,noname,retval) ) 

#define VBProjectProperties4_get_AssemblyOriginatorKeyFileType(This,retval)	\
    ( (This)->lpVtbl -> get_AssemblyOriginatorKeyFileType(This,retval) ) 

#define VBProjectProperties4_put_AssemblyOriginatorKeyFileType(This,noname,retval)	\
    ( (This)->lpVtbl -> put_AssemblyOriginatorKeyFileType(This,noname,retval) ) 

#define VBProjectProperties4_get_AssemblyVersion(This,retval)	\
    ( (This)->lpVtbl -> get_AssemblyVersion(This,retval) ) 

#define VBProjectProperties4_put_AssemblyVersion(This,noname,retval)	\
    ( (This)->lpVtbl -> put_AssemblyVersion(This,noname,retval) ) 

#define VBProjectProperties4_get_AssemblyFileVersion(This,retval)	\
    ( (This)->lpVtbl -> get_AssemblyFileVersion(This,retval) ) 

#define VBProjectProperties4_put_AssemblyFileVersion(This,noname,retval)	\
    ( (This)->lpVtbl -> put_AssemblyFileVersion(This,noname,retval) ) 

#define VBProjectProperties4_get_GenerateManifests(This,retval)	\
    ( (This)->lpVtbl -> get_GenerateManifests(This,retval) ) 

#define VBProjectProperties4_put_GenerateManifests(This,noname,retval)	\
    ( (This)->lpVtbl -> put_GenerateManifests(This,noname,retval) ) 

#define VBProjectProperties4_get_EnableSecurityDebugging(This,retval)	\
    ( (This)->lpVtbl -> get_EnableSecurityDebugging(This,retval) ) 

#define VBProjectProperties4_put_EnableSecurityDebugging(This,noname,retval)	\
    ( (This)->lpVtbl -> put_EnableSecurityDebugging(This,noname,retval) ) 

#define VBProjectProperties4_get_DebugSecurityZoneURL(This,retval)	\
    ( (This)->lpVtbl -> get_DebugSecurityZoneURL(This,retval) ) 

#define VBProjectProperties4_put_DebugSecurityZoneURL(This,noname,retval)	\
    ( (This)->lpVtbl -> put_DebugSecurityZoneURL(This,noname,retval) ) 

#define VBProjectProperties4_get_Publish(This,retval)	\
    ( (This)->lpVtbl -> get_Publish(This,retval) ) 

#define VBProjectProperties4_get_ComVisible(This,retval)	\
    ( (This)->lpVtbl -> get_ComVisible(This,retval) ) 

#define VBProjectProperties4_put_ComVisible(This,noname,retval)	\
    ( (This)->lpVtbl -> put_ComVisible(This,noname,retval) ) 

#define VBProjectProperties4_get_AssemblyGuid(This,retval)	\
    ( (This)->lpVtbl -> get_AssemblyGuid(This,retval) ) 

#define VBProjectProperties4_put_AssemblyGuid(This,noname,retval)	\
    ( (This)->lpVtbl -> put_AssemblyGuid(This,noname,retval) ) 

#define VBProjectProperties4_get_NeutralResourcesLanguage(This,retval)	\
    ( (This)->lpVtbl -> get_NeutralResourcesLanguage(This,retval) ) 

#define VBProjectProperties4_put_NeutralResourcesLanguage(This,noname,retval)	\
    ( (This)->lpVtbl -> put_NeutralResourcesLanguage(This,noname,retval) ) 

#define VBProjectProperties4_get_SignAssembly(This,retval)	\
    ( (This)->lpVtbl -> get_SignAssembly(This,retval) ) 

#define VBProjectProperties4_put_SignAssembly(This,noname,retval)	\
    ( (This)->lpVtbl -> put_SignAssembly(This,noname,retval) ) 

#define VBProjectProperties4_get_SignManifests(This,retval)	\
    ( (This)->lpVtbl -> get_SignManifests(This,retval) ) 

#define VBProjectProperties4_put_SignManifests(This,noname,retval)	\
    ( (This)->lpVtbl -> put_SignManifests(This,noname,retval) ) 

#define VBProjectProperties4_get_TargetZone(This,retval)	\
    ( (This)->lpVtbl -> get_TargetZone(This,retval) ) 

#define VBProjectProperties4_put_TargetZone(This,noname,retval)	\
    ( (This)->lpVtbl -> put_TargetZone(This,noname,retval) ) 

#define VBProjectProperties4_get_ExcludedPermissions(This,retval)	\
    ( (This)->lpVtbl -> get_ExcludedPermissions(This,retval) ) 

#define VBProjectProperties4_put_ExcludedPermissions(This,noname,retval)	\
    ( (This)->lpVtbl -> put_ExcludedPermissions(This,noname,retval) ) 

#define VBProjectProperties4_get_ManifestCertificateThumbprint(This,retval)	\
    ( (This)->lpVtbl -> get_ManifestCertificateThumbprint(This,retval) ) 

#define VBProjectProperties4_put_ManifestCertificateThumbprint(This,noname,retval)	\
    ( (This)->lpVtbl -> put_ManifestCertificateThumbprint(This,noname,retval) ) 

#define VBProjectProperties4_get_ManifestKeyFile(This,retval)	\
    ( (This)->lpVtbl -> get_ManifestKeyFile(This,retval) ) 

#define VBProjectProperties4_put_ManifestKeyFile(This,noname,retval)	\
    ( (This)->lpVtbl -> put_ManifestKeyFile(This,noname,retval) ) 

#define VBProjectProperties4_get_ManifestTimestampUrl(This,retval)	\
    ( (This)->lpVtbl -> get_ManifestTimestampUrl(This,retval) ) 

#define VBProjectProperties4_put_ManifestTimestampUrl(This,noname,retval)	\
    ( (This)->lpVtbl -> put_ManifestTimestampUrl(This,noname,retval) ) 

#define VBProjectProperties4_get_MyApplication(This,retval)	\
    ( (This)->lpVtbl -> get_MyApplication(This,retval) ) 

#define VBProjectProperties4_get_MyType(This,retval)	\
    ( (This)->lpVtbl -> get_MyType(This,retval) ) 

#define VBProjectProperties4_put_MyType(This,noname,retval)	\
    ( (This)->lpVtbl -> put_MyType(This,noname,retval) ) 


#define VBProjectProperties4_get_TargetFramework(This,pdwTargetFramework)	\
    ( (This)->lpVtbl -> get_TargetFramework(This,pdwTargetFramework) ) 

#define VBProjectProperties4_put_TargetFramework(This,dwTargetFramework)	\
    ( (This)->lpVtbl -> put_TargetFramework(This,dwTargetFramework) ) 

#define VBProjectProperties4_get_ApplicationManifest(This,pbstrApplicationManifest)	\
    ( (This)->lpVtbl -> get_ApplicationManifest(This,pbstrApplicationManifest) ) 

#define VBProjectProperties4_put_ApplicationManifest(This,ApplicationManifest)	\
    ( (This)->lpVtbl -> put_ApplicationManifest(This,ApplicationManifest) ) 

#define VBProjectProperties4_get_OptionInfer(This,pOptionInfer)	\
    ( (This)->lpVtbl -> get_OptionInfer(This,pOptionInfer) ) 

#define VBProjectProperties4_put_OptionInfer(This,optionInfer)	\
    ( (This)->lpVtbl -> put_OptionInfer(This,optionInfer) ) 

#endif /* COBJMACROS */


#endif 	/* C style interface */



/* [helpstring][id][nonbrowsable][propget] */ HRESULT STDMETHODCALLTYPE VBProjectProperties4_get_TargetFramework_Proxy( 
    __RPC__in VBProjectProperties4 * This,
    /* [retval][out] */ __RPC__out DWORD *pdwTargetFramework);


void __RPC_STUB VBProjectProperties4_get_TargetFramework_Stub(
    IRpcStubBuffer *This,
    IRpcChannelBuffer *_pRpcChannelBuffer,
    PRPC_MESSAGE _pRpcMessage,
    DWORD *_pdwStubPhase);


/* [helpstring][id][nonbrowsable][propput] */ HRESULT STDMETHODCALLTYPE VBProjectProperties4_put_TargetFramework_Proxy( 
    __RPC__in VBProjectProperties4 * This,
    /* [in] */ DWORD dwTargetFramework);


void __RPC_STUB VBProjectProperties4_put_TargetFramework_Stub(
    IRpcStubBuffer *This,
    IRpcChannelBuffer *_pRpcChannelBuffer,
    PRPC_MESSAGE _pRpcMessage,
    DWORD *_pdwStubPhase);


/* [helpstring][id][propget] */ HRESULT STDMETHODCALLTYPE VBProjectProperties4_get_ApplicationManifest_Proxy( 
    __RPC__in VBProjectProperties4 * This,
    /* [retval][out] */ __RPC__deref_out_opt BSTR *pbstrApplicationManifest);


void __RPC_STUB VBProjectProperties4_get_ApplicationManifest_Stub(
    IRpcStubBuffer *This,
    IRpcChannelBuffer *_pRpcChannelBuffer,
    PRPC_MESSAGE _pRpcMessage,
    DWORD *_pdwStubPhase);


/* [helpstring][id][propput] */ HRESULT STDMETHODCALLTYPE VBProjectProperties4_put_ApplicationManifest_Proxy( 
    __RPC__in VBProjectProperties4 * This,
    /* [in] */ __RPC__in BSTR ApplicationManifest);


void __RPC_STUB VBProjectProperties4_put_ApplicationManifest_Stub(
    IRpcStubBuffer *This,
    IRpcChannelBuffer *_pRpcChannelBuffer,
    PRPC_MESSAGE _pRpcMessage,
    DWORD *_pdwStubPhase);


/* [helpstring][id][propget] */ HRESULT STDMETHODCALLTYPE VBProjectProperties4_get_OptionInfer_Proxy( 
    __RPC__in VBProjectProperties4 * This,
    /* [retval][out] */ __RPC__out prjOptionInfer *pOptionInfer);


void __RPC_STUB VBProjectProperties4_get_OptionInfer_Stub(
    IRpcStubBuffer *This,
    IRpcChannelBuffer *_pRpcChannelBuffer,
    PRPC_MESSAGE _pRpcMessage,
    DWORD *_pdwStubPhase);


/* [helpstring][id][propput] */ HRESULT STDMETHODCALLTYPE VBProjectProperties4_put_OptionInfer_Proxy( 
    __RPC__in VBProjectProperties4 * This,
    /* [in] */ prjOptionInfer optionInfer);


void __RPC_STUB VBProjectProperties4_put_OptionInfer_Stub(
    IRpcStubBuffer *This,
    IRpcChannelBuffer *_pRpcChannelBuffer,
    PRPC_MESSAGE _pRpcMessage,
    DWORD *_pdwStubPhase);



#endif 	/* __VBProjectProperties4_INTERFACE_DEFINED__ */


#ifndef __VBPackageSettings2_INTERFACE_DEFINED__
#define __VBPackageSettings2_INTERFACE_DEFINED__

/* interface VBPackageSettings2 */
/* [object][dual][unique][helpstring][uuid] */ 


EXTERN_C const IID IID_VBPackageSettings2;

#if defined(__cplusplus) && !defined(CINTERFACE)
    
    MIDL_INTERFACE("471f93fa-0d7a-44fe-ac48-190cbb8b84c7")
    VBPackageSettings2 : public VBPackageSettings
    {
    public:
        virtual /* [helpstring][id][propget] */ HRESULT STDMETHODCALLTYPE get_OptionInfer( 
            /* [retval][out] */ __RPC__out pkgOptionInfer *pOptionInfer) = 0;
        
        virtual /* [helpstring][id][propput] */ HRESULT STDMETHODCALLTYPE put_OptionInfer( 
            /* [in] */ pkgOptionInfer optionInfer) = 0;
        
    };
    
    
#else 	/* C style interface */

    typedef struct VBPackageSettings2Vtbl
    {
        BEGIN_INTERFACE
        
        /* [id][restricted][funcdescattr] */ HRESULT ( STDMETHODCALLTYPE *QueryInterface )( 
            __RPC__in VBPackageSettings2 * This,
            /* [in][idldescattr] */ __RPC__in struct GUID *riid,
            /* [out][idldescattr] */ __RPC__deref_out_opt void **ppvObj,
            /* [retval][out] */ __RPC__out void *retval);
        
        /* [id][restricted][funcdescattr] */ HRESULT ( STDMETHODCALLTYPE *AddRef )( 
            __RPC__in VBPackageSettings2 * This,
            /* [retval][out] */ __RPC__out unsigned long *retval);
        
        /* [id][restricted][funcdescattr] */ HRESULT ( STDMETHODCALLTYPE *Release )( 
            __RPC__in VBPackageSettings2 * This,
            /* [retval][out] */ __RPC__out unsigned long *retval);
        
        /* [id][restricted][funcdescattr] */ HRESULT ( STDMETHODCALLTYPE *GetTypeInfoCount )( 
            __RPC__in VBPackageSettings2 * This,
            /* [out][idldescattr] */ __RPC__out unsigned UINT *pctinfo,
            /* [retval][out] */ __RPC__out void *retval);
        
        /* [id][restricted][funcdescattr] */ HRESULT ( STDMETHODCALLTYPE *GetTypeInfo )( 
            __RPC__in VBPackageSettings2 * This,
            /* [in][idldescattr] */ unsigned UINT itinfo,
            /* [in][idldescattr] */ unsigned long lcid,
            /* [out][idldescattr] */ __RPC__deref_out_opt void **pptinfo,
            /* [retval][out] */ __RPC__out void *retval);
        
        /* [id][restricted][funcdescattr] */ HRESULT ( STDMETHODCALLTYPE *GetIDsOfNames )( 
            __RPC__in VBPackageSettings2 * This,
            /* [in][idldescattr] */ __RPC__in struct GUID *riid,
            /* [in][idldescattr] */ __RPC__deref_in_opt signed char **rgszNames,
            /* [in][idldescattr] */ unsigned UINT cNames,
            /* [in][idldescattr] */ unsigned long lcid,
            /* [out][idldescattr] */ __RPC__out signed long *rgdispid,
            /* [retval][out] */ __RPC__out void *retval);
        
        /* [id][restricted][funcdescattr] */ HRESULT ( STDMETHODCALLTYPE *Invoke )( 
            __RPC__in VBPackageSettings2 * This,
            /* [in][idldescattr] */ signed long dispidMember,
            /* [in][idldescattr] */ __RPC__in struct GUID *riid,
            /* [in][idldescattr] */ unsigned long lcid,
            /* [in][idldescattr] */ unsigned short wFlags,
            /* [in][idldescattr] */ __RPC__in struct DISPPARAMS *pdispparams,
            /* [out][idldescattr] */ __RPC__out VARIANT *pvarResult,
            /* [out][idldescattr] */ __RPC__out struct EXCEPINFO *pexcepinfo,
            /* [out][idldescattr] */ __RPC__out unsigned UINT *puArgErr,
            /* [retval][out] */ __RPC__out void *retval);
        
        /* [id][propget][funcdescattr] */ HRESULT ( STDMETHODCALLTYPE *get_OptionExplicit )( 
            __RPC__in VBPackageSettings2 * This,
            /* [retval][out] */ __RPC__out enum pkgOptionExplicit *retval);
        
        /* [id][propput][funcdescattr] */ HRESULT ( STDMETHODCALLTYPE *put_OptionExplicit )( 
            __RPC__in VBPackageSettings2 * This,
            /* [in][idldescattr] */ enum pkgOptionExplicit noname,
            /* [retval][out] */ __RPC__out void *retval);
        
        /* [id][propget][funcdescattr] */ HRESULT ( STDMETHODCALLTYPE *get_OptionCompare )( 
            __RPC__in VBPackageSettings2 * This,
            /* [retval][out] */ __RPC__out enum pkgCompare *retval);
        
        /* [id][propput][funcdescattr] */ HRESULT ( STDMETHODCALLTYPE *put_OptionCompare )( 
            __RPC__in VBPackageSettings2 * This,
            /* [in][idldescattr] */ enum pkgCompare noname,
            /* [retval][out] */ __RPC__out void *retval);
        
        /* [id][propget][funcdescattr] */ HRESULT ( STDMETHODCALLTYPE *get_OptionStrict )( 
            __RPC__in VBPackageSettings2 * This,
            /* [retval][out] */ __RPC__out enum pkgOptionStrict *retval);
        
        /* [id][propput][funcdescattr] */ HRESULT ( STDMETHODCALLTYPE *put_OptionStrict )( 
            __RPC__in VBPackageSettings2 * This,
            /* [in][idldescattr] */ enum pkgOptionStrict noname,
            /* [retval][out] */ __RPC__out void *retval);
        
        /* [helpstring][id][propget] */ HRESULT ( STDMETHODCALLTYPE *get_OptionInfer )( 
            __RPC__in VBPackageSettings2 * This,
            /* [retval][out] */ __RPC__out pkgOptionInfer *pOptionInfer);
        
        /* [helpstring][id][propput] */ HRESULT ( STDMETHODCALLTYPE *put_OptionInfer )( 
            __RPC__in VBPackageSettings2 * This,
            /* [in] */ pkgOptionInfer optionInfer);
        
        END_INTERFACE
    } VBPackageSettings2Vtbl;

    interface VBPackageSettings2
    {
        CONST_VTBL struct VBPackageSettings2Vtbl *lpVtbl;
    };

    

#ifdef COBJMACROS


#define VBPackageSettings2_QueryInterface(This,riid,ppvObj,retval)	\
    ( (This)->lpVtbl -> QueryInterface(This,riid,ppvObj,retval) ) 

#define VBPackageSettings2_AddRef(This,retval)	\
    ( (This)->lpVtbl -> AddRef(This,retval) ) 

#define VBPackageSettings2_Release(This,retval)	\
    ( (This)->lpVtbl -> Release(This,retval) ) 

#define VBPackageSettings2_GetTypeInfoCount(This,pctinfo,retval)	\
    ( (This)->lpVtbl -> GetTypeInfoCount(This,pctinfo,retval) ) 

#define VBPackageSettings2_GetTypeInfo(This,itinfo,lcid,pptinfo,retval)	\
    ( (This)->lpVtbl -> GetTypeInfo(This,itinfo,lcid,pptinfo,retval) ) 

#define VBPackageSettings2_GetIDsOfNames(This,riid,rgszNames,cNames,lcid,rgdispid,retval)	\
    ( (This)->lpVtbl -> GetIDsOfNames(This,riid,rgszNames,cNames,lcid,rgdispid,retval) ) 

#define VBPackageSettings2_Invoke(This,dispidMember,riid,lcid,wFlags,pdispparams,pvarResult,pexcepinfo,puArgErr,retval)	\
    ( (This)->lpVtbl -> Invoke(This,dispidMember,riid,lcid,wFlags,pdispparams,pvarResult,pexcepinfo,puArgErr,retval) ) 

#define VBPackageSettings2_get_OptionExplicit(This,retval)	\
    ( (This)->lpVtbl -> get_OptionExplicit(This,retval) ) 

#define VBPackageSettings2_put_OptionExplicit(This,noname,retval)	\
    ( (This)->lpVtbl -> put_OptionExplicit(This,noname,retval) ) 

#define VBPackageSettings2_get_OptionCompare(This,retval)	\
    ( (This)->lpVtbl -> get_OptionCompare(This,retval) ) 

#define VBPackageSettings2_put_OptionCompare(This,noname,retval)	\
    ( (This)->lpVtbl -> put_OptionCompare(This,noname,retval) ) 

#define VBPackageSettings2_get_OptionStrict(This,retval)	\
    ( (This)->lpVtbl -> get_OptionStrict(This,retval) ) 

#define VBPackageSettings2_put_OptionStrict(This,noname,retval)	\
    ( (This)->lpVtbl -> put_OptionStrict(This,noname,retval) ) 


#define VBPackageSettings2_get_OptionInfer(This,pOptionInfer)	\
    ( (This)->lpVtbl -> get_OptionInfer(This,pOptionInfer) ) 

#define VBPackageSettings2_put_OptionInfer(This,optionInfer)	\
    ( (This)->lpVtbl -> put_OptionInfer(This,optionInfer) ) 

#endif /* COBJMACROS */


#endif 	/* C style interface */




#endif 	/* __VBPackageSettings2_INTERFACE_DEFINED__ */


#ifndef __CSharpProjectProperties4_INTERFACE_DEFINED__
#define __CSharpProjectProperties4_INTERFACE_DEFINED__

/* interface CSharpProjectProperties4 */
/* [object][dual][unique][helpstring][uuid] */ 


EXTERN_C const IID IID_CSharpProjectProperties4;

#if defined(__cplusplus) && !defined(CINTERFACE)
    
    MIDL_INTERFACE("9C52F596-60FB-4144-BE49-ADCA8F044790")
    CSharpProjectProperties4 : public ProjectProperties3
    {
    public:
        virtual /* [helpstring][id][nonbrowsable][propget] */ HRESULT STDMETHODCALLTYPE get_TargetFramework( 
            /* [retval][out] */ __RPC__out DWORD *pdwTargetFramework) = 0;
        
        virtual /* [helpstring][id][nonbrowsable][propput] */ HRESULT STDMETHODCALLTYPE put_TargetFramework( 
            /* [in] */ DWORD dwTargetFramework) = 0;
        
        virtual /* [helpstring][id][propget] */ HRESULT STDMETHODCALLTYPE get_ApplicationManifest( 
            /* [retval][out] */ __RPC__deref_out_opt BSTR *pbstrApplicationManifest) = 0;
        
        virtual /* [helpstring][id][propput] */ HRESULT STDMETHODCALLTYPE put_ApplicationManifest( 
            /* [in] */ __RPC__in BSTR ApplicationManifest) = 0;
        
    };
    
    
#else 	/* C style interface */

    typedef struct CSharpProjectProperties4Vtbl
    {
        BEGIN_INTERFACE
        
        /* [id][restricted][funcdescattr] */ HRESULT ( STDMETHODCALLTYPE *QueryInterface )( 
            __RPC__in CSharpProjectProperties4 * This,
            /* [in][idldescattr] */ __RPC__in struct GUID *riid,
            /* [out][idldescattr] */ __RPC__deref_out_opt void **ppvObj,
            /* [retval][out] */ __RPC__out void *retval);
        
        /* [id][restricted][funcdescattr] */ HRESULT ( STDMETHODCALLTYPE *AddRef )( 
            __RPC__in CSharpProjectProperties4 * This,
            /* [retval][out] */ __RPC__out unsigned long *retval);
        
        /* [id][restricted][funcdescattr] */ HRESULT ( STDMETHODCALLTYPE *Release )( 
            __RPC__in CSharpProjectProperties4 * This,
            /* [retval][out] */ __RPC__out unsigned long *retval);
        
        /* [id][restricted][funcdescattr] */ HRESULT ( STDMETHODCALLTYPE *GetTypeInfoCount )( 
            __RPC__in CSharpProjectProperties4 * This,
            /* [out][idldescattr] */ __RPC__out unsigned UINT *pctinfo,
            /* [retval][out] */ __RPC__out void *retval);
        
        /* [id][restricted][funcdescattr] */ HRESULT ( STDMETHODCALLTYPE *GetTypeInfo )( 
            __RPC__in CSharpProjectProperties4 * This,
            /* [in][idldescattr] */ unsigned UINT itinfo,
            /* [in][idldescattr] */ unsigned long lcid,
            /* [out][idldescattr] */ __RPC__deref_out_opt void **pptinfo,
            /* [retval][out] */ __RPC__out void *retval);
        
        /* [id][restricted][funcdescattr] */ HRESULT ( STDMETHODCALLTYPE *GetIDsOfNames )( 
            __RPC__in CSharpProjectProperties4 * This,
            /* [in][idldescattr] */ __RPC__in struct GUID *riid,
            /* [in][idldescattr] */ __RPC__deref_in_opt signed char **rgszNames,
            /* [in][idldescattr] */ unsigned UINT cNames,
            /* [in][idldescattr] */ unsigned long lcid,
            /* [out][idldescattr] */ __RPC__out signed long *rgdispid,
            /* [retval][out] */ __RPC__out void *retval);
        
        /* [id][restricted][funcdescattr] */ HRESULT ( STDMETHODCALLTYPE *Invoke )( 
            __RPC__in CSharpProjectProperties4 * This,
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
            __RPC__in CSharpProjectProperties4 * This,
            /* [retval][out] */ __RPC__deref_out_opt BSTR *retval);
        
        /* [id][propget][hidden][funcdescattr] */ HRESULT ( STDMETHODCALLTYPE *get___project )( 
            __RPC__in CSharpProjectProperties4 * This,
            /* [retval][out] */ __RPC__deref_out_opt IUnknown **retval);
        
        /* [id][propget][funcdescattr] */ HRESULT ( STDMETHODCALLTYPE *get_StartupObject )( 
            __RPC__in CSharpProjectProperties4 * This,
            /* [retval][out] */ __RPC__deref_out_opt BSTR *retval);
        
        /* [id][propput][funcdescattr] */ HRESULT ( STDMETHODCALLTYPE *put_StartupObject )( 
            __RPC__in CSharpProjectProperties4 * This,
            /* [in][idldescattr] */ __RPC__in BSTR noname,
            /* [retval][out] */ __RPC__out void *retval);
        
        /* [id][propget][funcdescattr] */ HRESULT ( STDMETHODCALLTYPE *get_OutputType )( 
            __RPC__in CSharpProjectProperties4 * This,
            /* [retval][out] */ __RPC__out enum prjOutputType *retval);
        
        /* [id][propput][funcdescattr] */ HRESULT ( STDMETHODCALLTYPE *put_OutputType )( 
            __RPC__in CSharpProjectProperties4 * This,
            /* [in][idldescattr] */ enum prjOutputType noname,
            /* [retval][out] */ __RPC__out void *retval);
        
        /* [id][propget][funcdescattr] */ HRESULT ( STDMETHODCALLTYPE *get_RootNamespace )( 
            __RPC__in CSharpProjectProperties4 * This,
            /* [retval][out] */ __RPC__deref_out_opt BSTR *retval);
        
        /* [id][propput][funcdescattr] */ HRESULT ( STDMETHODCALLTYPE *put_RootNamespace )( 
            __RPC__in CSharpProjectProperties4 * This,
            /* [in][idldescattr] */ __RPC__in BSTR noname,
            /* [retval][out] */ __RPC__out void *retval);
        
        /* [id][propget][funcdescattr] */ HRESULT ( STDMETHODCALLTYPE *get_AssemblyName )( 
            __RPC__in CSharpProjectProperties4 * This,
            /* [retval][out] */ __RPC__deref_out_opt BSTR *retval);
        
        /* [id][propput][funcdescattr] */ HRESULT ( STDMETHODCALLTYPE *put_AssemblyName )( 
            __RPC__in CSharpProjectProperties4 * This,
            /* [in][idldescattr] */ __RPC__in BSTR noname,
            /* [retval][out] */ __RPC__out void *retval);
        
        /* [id][propget][funcdescattr] */ HRESULT ( STDMETHODCALLTYPE *get_AssemblyOriginatorKeyFile )( 
            __RPC__in CSharpProjectProperties4 * This,
            /* [retval][out] */ __RPC__deref_out_opt BSTR *retval);
        
        /* [id][propput][funcdescattr] */ HRESULT ( STDMETHODCALLTYPE *put_AssemblyOriginatorKeyFile )( 
            __RPC__in CSharpProjectProperties4 * This,
            /* [in][idldescattr] */ __RPC__in BSTR noname,
            /* [retval][out] */ __RPC__out void *retval);
        
        /* [id][propget][funcdescattr] */ HRESULT ( STDMETHODCALLTYPE *get_AssemblyKeyContainerName )( 
            __RPC__in CSharpProjectProperties4 * This,
            /* [retval][out] */ __RPC__deref_out_opt BSTR *retval);
        
        /* [id][propput][funcdescattr] */ HRESULT ( STDMETHODCALLTYPE *put_AssemblyKeyContainerName )( 
            __RPC__in CSharpProjectProperties4 * This,
            /* [in][idldescattr] */ __RPC__in BSTR noname,
            /* [retval][out] */ __RPC__out void *retval);
        
        /* [id][propget][funcdescattr] */ HRESULT ( STDMETHODCALLTYPE *get_AssemblyOriginatorKeyMode )( 
            __RPC__in CSharpProjectProperties4 * This,
            /* [retval][out] */ __RPC__out enum prjOriginatorKeyMode *retval);
        
        /* [id][propput][funcdescattr] */ HRESULT ( STDMETHODCALLTYPE *put_AssemblyOriginatorKeyMode )( 
            __RPC__in CSharpProjectProperties4 * This,
            /* [in][idldescattr] */ enum prjOriginatorKeyMode noname,
            /* [retval][out] */ __RPC__out void *retval);
        
        /* [id][propget][funcdescattr] */ HRESULT ( STDMETHODCALLTYPE *get_DelaySign )( 
            __RPC__in CSharpProjectProperties4 * This,
            /* [retval][out] */ __RPC__out BOOLEAN *retval);
        
        /* [id][propput][funcdescattr] */ HRESULT ( STDMETHODCALLTYPE *put_DelaySign )( 
            __RPC__in CSharpProjectProperties4 * This,
            /* [in][idldescattr] */ BOOLEAN noname,
            /* [retval][out] */ __RPC__out void *retval);
        
        /* [id][propget][funcdescattr] */ HRESULT ( STDMETHODCALLTYPE *get_WebServer )( 
            __RPC__in CSharpProjectProperties4 * This,
            /* [retval][out] */ __RPC__deref_out_opt BSTR *retval);
        
        /* [id][propget][funcdescattr] */ HRESULT ( STDMETHODCALLTYPE *get_WebServerVersion )( 
            __RPC__in CSharpProjectProperties4 * This,
            /* [retval][out] */ __RPC__deref_out_opt BSTR *retval);
        
        /* [id][propget][funcdescattr] */ HRESULT ( STDMETHODCALLTYPE *get_ServerExtensionsVersion )( 
            __RPC__in CSharpProjectProperties4 * This,
            /* [retval][out] */ __RPC__deref_out_opt BSTR *retval);
        
        /* [id][propget][funcdescattr] */ HRESULT ( STDMETHODCALLTYPE *get_LinkRepair )( 
            __RPC__in CSharpProjectProperties4 * This,
            /* [retval][out] */ __RPC__out BOOLEAN *retval);
        
        /* [id][propput][funcdescattr] */ HRESULT ( STDMETHODCALLTYPE *put_LinkRepair )( 
            __RPC__in CSharpProjectProperties4 * This,
            /* [in][idldescattr] */ BOOLEAN noname,
            /* [retval][out] */ __RPC__out void *retval);
        
        /* [id][propget][funcdescattr] */ HRESULT ( STDMETHODCALLTYPE *get_OfflineURL )( 
            __RPC__in CSharpProjectProperties4 * This,
            /* [retval][out] */ __RPC__deref_out_opt BSTR *retval);
        
        /* [id][propget][funcdescattr] */ HRESULT ( STDMETHODCALLTYPE *get_FileSharePath )( 
            __RPC__in CSharpProjectProperties4 * This,
            /* [retval][out] */ __RPC__deref_out_opt BSTR *retval);
        
        /* [id][propput][funcdescattr] */ HRESULT ( STDMETHODCALLTYPE *put_FileSharePath )( 
            __RPC__in CSharpProjectProperties4 * This,
            /* [in][idldescattr] */ __RPC__in BSTR noname,
            /* [retval][out] */ __RPC__out void *retval);
        
        /* [id][propget][funcdescattr] */ HRESULT ( STDMETHODCALLTYPE *get_ActiveFileSharePath )( 
            __RPC__in CSharpProjectProperties4 * This,
            /* [retval][out] */ __RPC__deref_out_opt BSTR *retval);
        
        /* [id][propget][funcdescattr] */ HRESULT ( STDMETHODCALLTYPE *get_WebAccessMethod )( 
            __RPC__in CSharpProjectProperties4 * This,
            /* [retval][out] */ __RPC__out enum prjWebAccessMethod *retval);
        
        /* [id][propput][funcdescattr] */ HRESULT ( STDMETHODCALLTYPE *put_WebAccessMethod )( 
            __RPC__in CSharpProjectProperties4 * This,
            /* [in][idldescattr] */ enum prjWebAccessMethod noname,
            /* [retval][out] */ __RPC__out void *retval);
        
        /* [id][propget][funcdescattr] */ HRESULT ( STDMETHODCALLTYPE *get_ActiveWebAccessMethod )( 
            __RPC__in CSharpProjectProperties4 * This,
            /* [retval][out] */ __RPC__out enum prjWebAccessMethod *retval);
        
        /* [id][propget][funcdescattr] */ HRESULT ( STDMETHODCALLTYPE *get_DefaultClientScript )( 
            __RPC__in CSharpProjectProperties4 * This,
            /* [retval][out] */ __RPC__out enum prjScriptLanguage *retval);
        
        /* [id][propput][funcdescattr] */ HRESULT ( STDMETHODCALLTYPE *put_DefaultClientScript )( 
            __RPC__in CSharpProjectProperties4 * This,
            /* [in][idldescattr] */ enum prjScriptLanguage noname,
            /* [retval][out] */ __RPC__out void *retval);
        
        /* [id][propget][funcdescattr] */ HRESULT ( STDMETHODCALLTYPE *get_DefaultTargetSchema )( 
            __RPC__in CSharpProjectProperties4 * This,
            /* [retval][out] */ __RPC__out enum prjTargetSchema *retval);
        
        /* [id][propput][funcdescattr] */ HRESULT ( STDMETHODCALLTYPE *put_DefaultTargetSchema )( 
            __RPC__in CSharpProjectProperties4 * This,
            /* [in][idldescattr] */ enum prjTargetSchema noname,
            /* [retval][out] */ __RPC__out void *retval);
        
        /* [id][propget][funcdescattr] */ HRESULT ( STDMETHODCALLTYPE *get_DefaultHTMLPageLayout )( 
            __RPC__in CSharpProjectProperties4 * This,
            /* [retval][out] */ __RPC__out enum prjHTMLPageLayout *retval);
        
        /* [id][propput][funcdescattr] */ HRESULT ( STDMETHODCALLTYPE *put_DefaultHTMLPageLayout )( 
            __RPC__in CSharpProjectProperties4 * This,
            /* [in][idldescattr] */ enum prjHTMLPageLayout noname,
            /* [retval][out] */ __RPC__out void *retval);
        
        /* [id][propget][funcdescattr] */ HRESULT ( STDMETHODCALLTYPE *get_FileName )( 
            __RPC__in CSharpProjectProperties4 * This,
            /* [retval][out] */ __RPC__deref_out_opt BSTR *retval);
        
        /* [id][propput][funcdescattr] */ HRESULT ( STDMETHODCALLTYPE *put_FileName )( 
            __RPC__in CSharpProjectProperties4 * This,
            /* [in][idldescattr] */ __RPC__in BSTR noname,
            /* [retval][out] */ __RPC__out void *retval);
        
        /* [id][propget][funcdescattr] */ HRESULT ( STDMETHODCALLTYPE *get_FullPath )( 
            __RPC__in CSharpProjectProperties4 * This,
            /* [retval][out] */ __RPC__deref_out_opt BSTR *retval);
        
        /* [id][propget][funcdescattr] */ HRESULT ( STDMETHODCALLTYPE *get_LocalPath )( 
            __RPC__in CSharpProjectProperties4 * This,
            /* [retval][out] */ __RPC__deref_out_opt BSTR *retval);
        
        /* [id][propget][funcdescattr] */ HRESULT ( STDMETHODCALLTYPE *get_URL )( 
            __RPC__in CSharpProjectProperties4 * This,
            /* [retval][out] */ __RPC__deref_out_opt BSTR *retval);
        
        /* [id][propget][funcdescattr] */ HRESULT ( STDMETHODCALLTYPE *get_ActiveConfigurationSettings )( 
            __RPC__in CSharpProjectProperties4 * This,
            /* [retval][out] */ __RPC__deref_out_opt ProjectConfigurationProperties **retval);
        
        /* [id][propget][funcdescattr] */ HRESULT ( STDMETHODCALLTYPE *get_Extender )( 
            __RPC__in CSharpProjectProperties4 * This,
            /* [in][idldescattr] */ __RPC__in BSTR ExtenderName,
            /* [retval][out] */ __RPC__deref_out_opt IDispatch **retval);
        
        /* [id][propget][funcdescattr] */ HRESULT ( STDMETHODCALLTYPE *get_ExtenderNames )( 
            __RPC__in CSharpProjectProperties4 * This,
            /* [retval][out] */ __RPC__out VARIANT *retval);
        
        /* [id][propget][funcdescattr] */ HRESULT ( STDMETHODCALLTYPE *get_ExtenderCATID )( 
            __RPC__in CSharpProjectProperties4 * This,
            /* [retval][out] */ __RPC__deref_out_opt BSTR *retval);
        
        /* [id][propget][funcdescattr] */ HRESULT ( STDMETHODCALLTYPE *get_ApplicationIcon )( 
            __RPC__in CSharpProjectProperties4 * This,
            /* [retval][out] */ __RPC__deref_out_opt BSTR *retval);
        
        /* [id][propput][funcdescattr] */ HRESULT ( STDMETHODCALLTYPE *put_ApplicationIcon )( 
            __RPC__in CSharpProjectProperties4 * This,
            /* [in][idldescattr] */ __RPC__in BSTR noname,
            /* [retval][out] */ __RPC__out void *retval);
        
        /* [id][propget][funcdescattr] */ HRESULT ( STDMETHODCALLTYPE *get_OptionStrict )( 
            __RPC__in CSharpProjectProperties4 * This,
            /* [retval][out] */ __RPC__out enum prjOptionStrict *retval);
        
        /* [id][propput][funcdescattr] */ HRESULT ( STDMETHODCALLTYPE *put_OptionStrict )( 
            __RPC__in CSharpProjectProperties4 * This,
            /* [in][idldescattr] */ enum prjOptionStrict noname,
            /* [retval][out] */ __RPC__out void *retval);
        
        /* [id][propget][funcdescattr] */ HRESULT ( STDMETHODCALLTYPE *get_ReferencePath )( 
            __RPC__in CSharpProjectProperties4 * This,
            /* [retval][out] */ __RPC__deref_out_opt BSTR *retval);
        
        /* [id][propput][funcdescattr] */ HRESULT ( STDMETHODCALLTYPE *put_ReferencePath )( 
            __RPC__in CSharpProjectProperties4 * This,
            /* [in][idldescattr] */ __RPC__in BSTR noname,
            /* [retval][out] */ __RPC__out void *retval);
        
        /* [id][propget][funcdescattr] */ HRESULT ( STDMETHODCALLTYPE *get_OutputFileName )( 
            __RPC__in CSharpProjectProperties4 * This,
            /* [retval][out] */ __RPC__deref_out_opt BSTR *retval);
        
        /* [id][propget][hidden][funcdescattr] */ HRESULT ( STDMETHODCALLTYPE *get_AbsoluteProjectDirectory )( 
            __RPC__in CSharpProjectProperties4 * This,
            /* [retval][out] */ __RPC__deref_out_opt BSTR *retval);
        
        /* [id][propget][funcdescattr] */ HRESULT ( STDMETHODCALLTYPE *get_OptionExplicit )( 
            __RPC__in CSharpProjectProperties4 * This,
            /* [retval][out] */ __RPC__out enum prjOptionExplicit *retval);
        
        /* [id][propput][funcdescattr] */ HRESULT ( STDMETHODCALLTYPE *put_OptionExplicit )( 
            __RPC__in CSharpProjectProperties4 * This,
            /* [in][idldescattr] */ enum prjOptionExplicit noname,
            /* [retval][out] */ __RPC__out void *retval);
        
        /* [id][propget][funcdescattr] */ HRESULT ( STDMETHODCALLTYPE *get_OptionCompare )( 
            __RPC__in CSharpProjectProperties4 * This,
            /* [retval][out] */ __RPC__out enum prjCompare *retval);
        
        /* [id][propput][funcdescattr] */ HRESULT ( STDMETHODCALLTYPE *put_OptionCompare )( 
            __RPC__in CSharpProjectProperties4 * This,
            /* [in][idldescattr] */ enum prjCompare noname,
            /* [retval][out] */ __RPC__out void *retval);
        
        /* [id][propget][funcdescattr] */ HRESULT ( STDMETHODCALLTYPE *get_ProjectType )( 
            __RPC__in CSharpProjectProperties4 * This,
            /* [retval][out] */ __RPC__out enum prjProjectType *retval);
        
        /* [id][propget][funcdescattr] */ HRESULT ( STDMETHODCALLTYPE *get_DefaultNamespace )( 
            __RPC__in CSharpProjectProperties4 * This,
            /* [retval][out] */ __RPC__deref_out_opt BSTR *retval);
        
        /* [id][propput][funcdescattr] */ HRESULT ( STDMETHODCALLTYPE *put_DefaultNamespace )( 
            __RPC__in CSharpProjectProperties4 * This,
            /* [in][idldescattr] */ __RPC__in BSTR noname,
            /* [retval][out] */ __RPC__out void *retval);
        
        /* [id][propget][funcdescattr] */ HRESULT ( STDMETHODCALLTYPE *get_PreBuildEvent )( 
            __RPC__in CSharpProjectProperties4 * This,
            /* [retval][out] */ __RPC__deref_out_opt BSTR *retval);
        
        /* [id][propput][funcdescattr] */ HRESULT ( STDMETHODCALLTYPE *put_PreBuildEvent )( 
            __RPC__in CSharpProjectProperties4 * This,
            /* [in][idldescattr] */ __RPC__in BSTR noname,
            /* [retval][out] */ __RPC__out void *retval);
        
        /* [id][propget][funcdescattr] */ HRESULT ( STDMETHODCALLTYPE *get_PostBuildEvent )( 
            __RPC__in CSharpProjectProperties4 * This,
            /* [retval][out] */ __RPC__deref_out_opt BSTR *retval);
        
        /* [id][propput][funcdescattr] */ HRESULT ( STDMETHODCALLTYPE *put_PostBuildEvent )( 
            __RPC__in CSharpProjectProperties4 * This,
            /* [in][idldescattr] */ __RPC__in BSTR noname,
            /* [retval][out] */ __RPC__out void *retval);
        
        /* [id][propget][funcdescattr] */ HRESULT ( STDMETHODCALLTYPE *get_RunPostBuildEvent )( 
            __RPC__in CSharpProjectProperties4 * This,
            /* [retval][out] */ __RPC__out enum prjRunPostBuildEvent *retval);
        
        /* [id][propput][funcdescattr] */ HRESULT ( STDMETHODCALLTYPE *put_RunPostBuildEvent )( 
            __RPC__in CSharpProjectProperties4 * This,
            /* [in][idldescattr] */ enum prjRunPostBuildEvent noname,
            /* [retval][out] */ __RPC__out void *retval);
        
        /* [id][propget][funcdescattr] */ HRESULT ( STDMETHODCALLTYPE *get_AspnetVersion )( 
            __RPC__in CSharpProjectProperties4 * This,
            /* [retval][out] */ __RPC__deref_out_opt BSTR *retval);
        
        /* [id][propget][funcdescattr] */ HRESULT ( STDMETHODCALLTYPE *get_Title )( 
            __RPC__in CSharpProjectProperties4 * This,
            /* [retval][out] */ __RPC__deref_out_opt BSTR *retval);
        
        /* [id][propput][funcdescattr] */ HRESULT ( STDMETHODCALLTYPE *put_Title )( 
            __RPC__in CSharpProjectProperties4 * This,
            /* [in][idldescattr] */ __RPC__in BSTR noname,
            /* [retval][out] */ __RPC__out void *retval);
        
        /* [id][propget][funcdescattr] */ HRESULT ( STDMETHODCALLTYPE *get_Description )( 
            __RPC__in CSharpProjectProperties4 * This,
            /* [retval][out] */ __RPC__deref_out_opt BSTR *retval);
        
        /* [id][propput][funcdescattr] */ HRESULT ( STDMETHODCALLTYPE *put_Description )( 
            __RPC__in CSharpProjectProperties4 * This,
            /* [in][idldescattr] */ __RPC__in BSTR noname,
            /* [retval][out] */ __RPC__out void *retval);
        
        /* [id][propget][funcdescattr] */ HRESULT ( STDMETHODCALLTYPE *get_Company )( 
            __RPC__in CSharpProjectProperties4 * This,
            /* [retval][out] */ __RPC__deref_out_opt BSTR *retval);
        
        /* [id][propput][funcdescattr] */ HRESULT ( STDMETHODCALLTYPE *put_Company )( 
            __RPC__in CSharpProjectProperties4 * This,
            /* [in][idldescattr] */ __RPC__in BSTR noname,
            /* [retval][out] */ __RPC__out void *retval);
        
        /* [id][propget][funcdescattr] */ HRESULT ( STDMETHODCALLTYPE *get_Product )( 
            __RPC__in CSharpProjectProperties4 * This,
            /* [retval][out] */ __RPC__deref_out_opt BSTR *retval);
        
        /* [id][propput][funcdescattr] */ HRESULT ( STDMETHODCALLTYPE *put_Product )( 
            __RPC__in CSharpProjectProperties4 * This,
            /* [in][idldescattr] */ __RPC__in BSTR noname,
            /* [retval][out] */ __RPC__out void *retval);
        
        /* [id][propget][funcdescattr] */ HRESULT ( STDMETHODCALLTYPE *get_Copyright )( 
            __RPC__in CSharpProjectProperties4 * This,
            /* [retval][out] */ __RPC__deref_out_opt BSTR *retval);
        
        /* [id][propput][funcdescattr] */ HRESULT ( STDMETHODCALLTYPE *put_Copyright )( 
            __RPC__in CSharpProjectProperties4 * This,
            /* [in][idldescattr] */ __RPC__in BSTR noname,
            /* [retval][out] */ __RPC__out void *retval);
        
        /* [id][propget][funcdescattr] */ HRESULT ( STDMETHODCALLTYPE *get_Trademark )( 
            __RPC__in CSharpProjectProperties4 * This,
            /* [retval][out] */ __RPC__deref_out_opt BSTR *retval);
        
        /* [id][propput][funcdescattr] */ HRESULT ( STDMETHODCALLTYPE *put_Trademark )( 
            __RPC__in CSharpProjectProperties4 * This,
            /* [in][idldescattr] */ __RPC__in BSTR noname,
            /* [retval][out] */ __RPC__out void *retval);
        
        /* [id][propget][funcdescattr] */ HRESULT ( STDMETHODCALLTYPE *get_AssemblyType )( 
            __RPC__in CSharpProjectProperties4 * This,
            /* [retval][out] */ __RPC__out enum prjAssemblyType *retval);
        
        /* [id][propput][funcdescattr] */ HRESULT ( STDMETHODCALLTYPE *put_AssemblyType )( 
            __RPC__in CSharpProjectProperties4 * This,
            /* [in][idldescattr] */ enum prjAssemblyType noname,
            /* [retval][out] */ __RPC__out void *retval);
        
        /* [id][propget][funcdescattr] */ HRESULT ( STDMETHODCALLTYPE *get_TypeComplianceDiagnostics )( 
            __RPC__in CSharpProjectProperties4 * This,
            /* [retval][out] */ __RPC__out BOOLEAN *retval);
        
        /* [id][propput][funcdescattr] */ HRESULT ( STDMETHODCALLTYPE *put_TypeComplianceDiagnostics )( 
            __RPC__in CSharpProjectProperties4 * This,
            /* [in][idldescattr] */ BOOLEAN noname,
            /* [retval][out] */ __RPC__out void *retval);
        
        /* [id][propget][funcdescattr] */ HRESULT ( STDMETHODCALLTYPE *get_Win32ResourceFile )( 
            __RPC__in CSharpProjectProperties4 * This,
            /* [retval][out] */ __RPC__deref_out_opt BSTR *retval);
        
        /* [id][propput][funcdescattr] */ HRESULT ( STDMETHODCALLTYPE *put_Win32ResourceFile )( 
            __RPC__in CSharpProjectProperties4 * This,
            /* [in][idldescattr] */ __RPC__in BSTR noname,
            /* [retval][out] */ __RPC__out void *retval);
        
        /* [id][propget][funcdescattr] */ HRESULT ( STDMETHODCALLTYPE *get_AssemblyKeyProviderName )( 
            __RPC__in CSharpProjectProperties4 * This,
            /* [retval][out] */ __RPC__deref_out_opt BSTR *retval);
        
        /* [id][propput][funcdescattr] */ HRESULT ( STDMETHODCALLTYPE *put_AssemblyKeyProviderName )( 
            __RPC__in CSharpProjectProperties4 * This,
            /* [in][idldescattr] */ __RPC__in BSTR noname,
            /* [retval][out] */ __RPC__out void *retval);
        
        /* [id][propget][funcdescattr] */ HRESULT ( STDMETHODCALLTYPE *get_AssemblyOriginatorKeyFileType )( 
            __RPC__in CSharpProjectProperties4 * This,
            /* [retval][out] */ __RPC__out unsigned long *retval);
        
        /* [id][propput][funcdescattr] */ HRESULT ( STDMETHODCALLTYPE *put_AssemblyOriginatorKeyFileType )( 
            __RPC__in CSharpProjectProperties4 * This,
            /* [in][idldescattr] */ unsigned long noname,
            /* [retval][out] */ __RPC__out void *retval);
        
        /* [id][propget][funcdescattr] */ HRESULT ( STDMETHODCALLTYPE *get_AssemblyVersion )( 
            __RPC__in CSharpProjectProperties4 * This,
            /* [retval][out] */ __RPC__deref_out_opt BSTR *retval);
        
        /* [id][propput][funcdescattr] */ HRESULT ( STDMETHODCALLTYPE *put_AssemblyVersion )( 
            __RPC__in CSharpProjectProperties4 * This,
            /* [in][idldescattr] */ __RPC__in BSTR noname,
            /* [retval][out] */ __RPC__out void *retval);
        
        /* [id][propget][funcdescattr] */ HRESULT ( STDMETHODCALLTYPE *get_AssemblyFileVersion )( 
            __RPC__in CSharpProjectProperties4 * This,
            /* [retval][out] */ __RPC__deref_out_opt BSTR *retval);
        
        /* [id][propput][funcdescattr] */ HRESULT ( STDMETHODCALLTYPE *put_AssemblyFileVersion )( 
            __RPC__in CSharpProjectProperties4 * This,
            /* [in][idldescattr] */ __RPC__in BSTR noname,
            /* [retval][out] */ __RPC__out void *retval);
        
        /* [id][propget][funcdescattr] */ HRESULT ( STDMETHODCALLTYPE *get_GenerateManifests )( 
            __RPC__in CSharpProjectProperties4 * This,
            /* [retval][out] */ __RPC__out BOOLEAN *retval);
        
        /* [id][propput][funcdescattr] */ HRESULT ( STDMETHODCALLTYPE *put_GenerateManifests )( 
            __RPC__in CSharpProjectProperties4 * This,
            /* [in][idldescattr] */ BOOLEAN noname,
            /* [retval][out] */ __RPC__out void *retval);
        
        /* [id][propget][funcdescattr] */ HRESULT ( STDMETHODCALLTYPE *get_EnableSecurityDebugging )( 
            __RPC__in CSharpProjectProperties4 * This,
            /* [retval][out] */ __RPC__out BOOLEAN *retval);
        
        /* [id][propput][funcdescattr] */ HRESULT ( STDMETHODCALLTYPE *put_EnableSecurityDebugging )( 
            __RPC__in CSharpProjectProperties4 * This,
            /* [in][idldescattr] */ BOOLEAN noname,
            /* [retval][out] */ __RPC__out void *retval);
        
        /* [id][propget][funcdescattr] */ HRESULT ( STDMETHODCALLTYPE *get_DebugSecurityZoneURL )( 
            __RPC__in CSharpProjectProperties4 * This,
            /* [retval][out] */ __RPC__deref_out_opt BSTR *retval);
        
        /* [id][propput][funcdescattr] */ HRESULT ( STDMETHODCALLTYPE *put_DebugSecurityZoneURL )( 
            __RPC__in CSharpProjectProperties4 * This,
            /* [in][idldescattr] */ __RPC__in BSTR noname,
            /* [retval][out] */ __RPC__out void *retval);
        
        /* [id][propget][funcdescattr] */ HRESULT ( STDMETHODCALLTYPE *get_Publish )( 
            __RPC__in CSharpProjectProperties4 * This,
            /* [retval][out] */ __RPC__deref_out_opt IDispatch **retval);
        
        /* [id][propget][funcdescattr] */ HRESULT ( STDMETHODCALLTYPE *get_ComVisible )( 
            __RPC__in CSharpProjectProperties4 * This,
            /* [retval][out] */ __RPC__out BOOLEAN *retval);
        
        /* [id][propput][funcdescattr] */ HRESULT ( STDMETHODCALLTYPE *put_ComVisible )( 
            __RPC__in CSharpProjectProperties4 * This,
            /* [in][idldescattr] */ BOOLEAN noname,
            /* [retval][out] */ __RPC__out void *retval);
        
        /* [id][propget][funcdescattr] */ HRESULT ( STDMETHODCALLTYPE *get_AssemblyGuid )( 
            __RPC__in CSharpProjectProperties4 * This,
            /* [retval][out] */ __RPC__deref_out_opt BSTR *retval);
        
        /* [id][propput][funcdescattr] */ HRESULT ( STDMETHODCALLTYPE *put_AssemblyGuid )( 
            __RPC__in CSharpProjectProperties4 * This,
            /* [in][idldescattr] */ __RPC__in BSTR noname,
            /* [retval][out] */ __RPC__out void *retval);
        
        /* [id][propget][funcdescattr] */ HRESULT ( STDMETHODCALLTYPE *get_NeutralResourcesLanguage )( 
            __RPC__in CSharpProjectProperties4 * This,
            /* [retval][out] */ __RPC__deref_out_opt BSTR *retval);
        
        /* [id][propput][funcdescattr] */ HRESULT ( STDMETHODCALLTYPE *put_NeutralResourcesLanguage )( 
            __RPC__in CSharpProjectProperties4 * This,
            /* [in][idldescattr] */ __RPC__in BSTR noname,
            /* [retval][out] */ __RPC__out void *retval);
        
        /* [id][propget][funcdescattr] */ HRESULT ( STDMETHODCALLTYPE *get_SignAssembly )( 
            __RPC__in CSharpProjectProperties4 * This,
            /* [retval][out] */ __RPC__out BOOLEAN *retval);
        
        /* [id][propput][funcdescattr] */ HRESULT ( STDMETHODCALLTYPE *put_SignAssembly )( 
            __RPC__in CSharpProjectProperties4 * This,
            /* [in][idldescattr] */ BOOLEAN noname,
            /* [retval][out] */ __RPC__out void *retval);
        
        /* [id][propget][funcdescattr] */ HRESULT ( STDMETHODCALLTYPE *get_SignManifests )( 
            __RPC__in CSharpProjectProperties4 * This,
            /* [retval][out] */ __RPC__out BOOLEAN *retval);
        
        /* [id][propput][funcdescattr] */ HRESULT ( STDMETHODCALLTYPE *put_SignManifests )( 
            __RPC__in CSharpProjectProperties4 * This,
            /* [in][idldescattr] */ BOOLEAN noname,
            /* [retval][out] */ __RPC__out void *retval);
        
        /* [id][propget][funcdescattr] */ HRESULT ( STDMETHODCALLTYPE *get_TargetZone )( 
            __RPC__in CSharpProjectProperties4 * This,
            /* [retval][out] */ __RPC__deref_out_opt BSTR *retval);
        
        /* [id][propput][funcdescattr] */ HRESULT ( STDMETHODCALLTYPE *put_TargetZone )( 
            __RPC__in CSharpProjectProperties4 * This,
            /* [in][idldescattr] */ __RPC__in BSTR noname,
            /* [retval][out] */ __RPC__out void *retval);
        
        /* [id][propget][funcdescattr] */ HRESULT ( STDMETHODCALLTYPE *get_ExcludedPermissions )( 
            __RPC__in CSharpProjectProperties4 * This,
            /* [retval][out] */ __RPC__deref_out_opt BSTR *retval);
        
        /* [id][propput][funcdescattr] */ HRESULT ( STDMETHODCALLTYPE *put_ExcludedPermissions )( 
            __RPC__in CSharpProjectProperties4 * This,
            /* [in][idldescattr] */ __RPC__in BSTR noname,
            /* [retval][out] */ __RPC__out void *retval);
        
        /* [id][propget][funcdescattr] */ HRESULT ( STDMETHODCALLTYPE *get_ManifestCertificateThumbprint )( 
            __RPC__in CSharpProjectProperties4 * This,
            /* [retval][out] */ __RPC__deref_out_opt BSTR *retval);
        
        /* [id][propput][funcdescattr] */ HRESULT ( STDMETHODCALLTYPE *put_ManifestCertificateThumbprint )( 
            __RPC__in CSharpProjectProperties4 * This,
            /* [in][idldescattr] */ __RPC__in BSTR noname,
            /* [retval][out] */ __RPC__out void *retval);
        
        /* [id][propget][funcdescattr] */ HRESULT ( STDMETHODCALLTYPE *get_ManifestKeyFile )( 
            __RPC__in CSharpProjectProperties4 * This,
            /* [retval][out] */ __RPC__deref_out_opt BSTR *retval);
        
        /* [id][propput][funcdescattr] */ HRESULT ( STDMETHODCALLTYPE *put_ManifestKeyFile )( 
            __RPC__in CSharpProjectProperties4 * This,
            /* [in][idldescattr] */ __RPC__in BSTR noname,
            /* [retval][out] */ __RPC__out void *retval);
        
        /* [id][propget][funcdescattr] */ HRESULT ( STDMETHODCALLTYPE *get_ManifestTimestampUrl )( 
            __RPC__in CSharpProjectProperties4 * This,
            /* [retval][out] */ __RPC__deref_out_opt BSTR *retval);
        
        /* [id][propput][funcdescattr] */ HRESULT ( STDMETHODCALLTYPE *put_ManifestTimestampUrl )( 
            __RPC__in CSharpProjectProperties4 * This,
            /* [in][idldescattr] */ __RPC__in BSTR noname,
            /* [retval][out] */ __RPC__out void *retval);
        
        /* [helpstring][id][nonbrowsable][propget] */ HRESULT ( STDMETHODCALLTYPE *get_TargetFramework )( 
            __RPC__in CSharpProjectProperties4 * This,
            /* [retval][out] */ __RPC__out DWORD *pdwTargetFramework);
        
        /* [helpstring][id][nonbrowsable][propput] */ HRESULT ( STDMETHODCALLTYPE *put_TargetFramework )( 
            __RPC__in CSharpProjectProperties4 * This,
            /* [in] */ DWORD dwTargetFramework);
        
        /* [helpstring][id][propget] */ HRESULT ( STDMETHODCALLTYPE *get_ApplicationManifest )( 
            __RPC__in CSharpProjectProperties4 * This,
            /* [retval][out] */ __RPC__deref_out_opt BSTR *pbstrApplicationManifest);
        
        /* [helpstring][id][propput] */ HRESULT ( STDMETHODCALLTYPE *put_ApplicationManifest )( 
            __RPC__in CSharpProjectProperties4 * This,
            /* [in] */ __RPC__in BSTR ApplicationManifest);
        
        END_INTERFACE
    } CSharpProjectProperties4Vtbl;

    interface CSharpProjectProperties4
    {
        CONST_VTBL struct CSharpProjectProperties4Vtbl *lpVtbl;
    };

    

#ifdef COBJMACROS


#define CSharpProjectProperties4_QueryInterface(This,riid,ppvObj,retval)	\
    ( (This)->lpVtbl -> QueryInterface(This,riid,ppvObj,retval) ) 

#define CSharpProjectProperties4_AddRef(This,retval)	\
    ( (This)->lpVtbl -> AddRef(This,retval) ) 

#define CSharpProjectProperties4_Release(This,retval)	\
    ( (This)->lpVtbl -> Release(This,retval) ) 

#define CSharpProjectProperties4_GetTypeInfoCount(This,pctinfo,retval)	\
    ( (This)->lpVtbl -> GetTypeInfoCount(This,pctinfo,retval) ) 

#define CSharpProjectProperties4_GetTypeInfo(This,itinfo,lcid,pptinfo,retval)	\
    ( (This)->lpVtbl -> GetTypeInfo(This,itinfo,lcid,pptinfo,retval) ) 

#define CSharpProjectProperties4_GetIDsOfNames(This,riid,rgszNames,cNames,lcid,rgdispid,retval)	\
    ( (This)->lpVtbl -> GetIDsOfNames(This,riid,rgszNames,cNames,lcid,rgdispid,retval) ) 

#define CSharpProjectProperties4_Invoke(This,dispidMember,riid,lcid,wFlags,pdispparams,pvarResult,pexcepinfo,puArgErr,retval)	\
    ( (This)->lpVtbl -> Invoke(This,dispidMember,riid,lcid,wFlags,pdispparams,pvarResult,pexcepinfo,puArgErr,retval) ) 

#define CSharpProjectProperties4_get___id(This,retval)	\
    ( (This)->lpVtbl -> get___id(This,retval) ) 

#define CSharpProjectProperties4_get___project(This,retval)	\
    ( (This)->lpVtbl -> get___project(This,retval) ) 

#define CSharpProjectProperties4_get_StartupObject(This,retval)	\
    ( (This)->lpVtbl -> get_StartupObject(This,retval) ) 

#define CSharpProjectProperties4_put_StartupObject(This,noname,retval)	\
    ( (This)->lpVtbl -> put_StartupObject(This,noname,retval) ) 

#define CSharpProjectProperties4_get_OutputType(This,retval)	\
    ( (This)->lpVtbl -> get_OutputType(This,retval) ) 

#define CSharpProjectProperties4_put_OutputType(This,noname,retval)	\
    ( (This)->lpVtbl -> put_OutputType(This,noname,retval) ) 

#define CSharpProjectProperties4_get_RootNamespace(This,retval)	\
    ( (This)->lpVtbl -> get_RootNamespace(This,retval) ) 

#define CSharpProjectProperties4_put_RootNamespace(This,noname,retval)	\
    ( (This)->lpVtbl -> put_RootNamespace(This,noname,retval) ) 

#define CSharpProjectProperties4_get_AssemblyName(This,retval)	\
    ( (This)->lpVtbl -> get_AssemblyName(This,retval) ) 

#define CSharpProjectProperties4_put_AssemblyName(This,noname,retval)	\
    ( (This)->lpVtbl -> put_AssemblyName(This,noname,retval) ) 

#define CSharpProjectProperties4_get_AssemblyOriginatorKeyFile(This,retval)	\
    ( (This)->lpVtbl -> get_AssemblyOriginatorKeyFile(This,retval) ) 

#define CSharpProjectProperties4_put_AssemblyOriginatorKeyFile(This,noname,retval)	\
    ( (This)->lpVtbl -> put_AssemblyOriginatorKeyFile(This,noname,retval) ) 

#define CSharpProjectProperties4_get_AssemblyKeyContainerName(This,retval)	\
    ( (This)->lpVtbl -> get_AssemblyKeyContainerName(This,retval) ) 

#define CSharpProjectProperties4_put_AssemblyKeyContainerName(This,noname,retval)	\
    ( (This)->lpVtbl -> put_AssemblyKeyContainerName(This,noname,retval) ) 

#define CSharpProjectProperties4_get_AssemblyOriginatorKeyMode(This,retval)	\
    ( (This)->lpVtbl -> get_AssemblyOriginatorKeyMode(This,retval) ) 

#define CSharpProjectProperties4_put_AssemblyOriginatorKeyMode(This,noname,retval)	\
    ( (This)->lpVtbl -> put_AssemblyOriginatorKeyMode(This,noname,retval) ) 

#define CSharpProjectProperties4_get_DelaySign(This,retval)	\
    ( (This)->lpVtbl -> get_DelaySign(This,retval) ) 

#define CSharpProjectProperties4_put_DelaySign(This,noname,retval)	\
    ( (This)->lpVtbl -> put_DelaySign(This,noname,retval) ) 

#define CSharpProjectProperties4_get_WebServer(This,retval)	\
    ( (This)->lpVtbl -> get_WebServer(This,retval) ) 

#define CSharpProjectProperties4_get_WebServerVersion(This,retval)	\
    ( (This)->lpVtbl -> get_WebServerVersion(This,retval) ) 

#define CSharpProjectProperties4_get_ServerExtensionsVersion(This,retval)	\
    ( (This)->lpVtbl -> get_ServerExtensionsVersion(This,retval) ) 

#define CSharpProjectProperties4_get_LinkRepair(This,retval)	\
    ( (This)->lpVtbl -> get_LinkRepair(This,retval) ) 

#define CSharpProjectProperties4_put_LinkRepair(This,noname,retval)	\
    ( (This)->lpVtbl -> put_LinkRepair(This,noname,retval) ) 

#define CSharpProjectProperties4_get_OfflineURL(This,retval)	\
    ( (This)->lpVtbl -> get_OfflineURL(This,retval) ) 

#define CSharpProjectProperties4_get_FileSharePath(This,retval)	\
    ( (This)->lpVtbl -> get_FileSharePath(This,retval) ) 

#define CSharpProjectProperties4_put_FileSharePath(This,noname,retval)	\
    ( (This)->lpVtbl -> put_FileSharePath(This,noname,retval) ) 

#define CSharpProjectProperties4_get_ActiveFileSharePath(This,retval)	\
    ( (This)->lpVtbl -> get_ActiveFileSharePath(This,retval) ) 

#define CSharpProjectProperties4_get_WebAccessMethod(This,retval)	\
    ( (This)->lpVtbl -> get_WebAccessMethod(This,retval) ) 

#define CSharpProjectProperties4_put_WebAccessMethod(This,noname,retval)	\
    ( (This)->lpVtbl -> put_WebAccessMethod(This,noname,retval) ) 

#define CSharpProjectProperties4_get_ActiveWebAccessMethod(This,retval)	\
    ( (This)->lpVtbl -> get_ActiveWebAccessMethod(This,retval) ) 

#define CSharpProjectProperties4_get_DefaultClientScript(This,retval)	\
    ( (This)->lpVtbl -> get_DefaultClientScript(This,retval) ) 

#define CSharpProjectProperties4_put_DefaultClientScript(This,noname,retval)	\
    ( (This)->lpVtbl -> put_DefaultClientScript(This,noname,retval) ) 

#define CSharpProjectProperties4_get_DefaultTargetSchema(This,retval)	\
    ( (This)->lpVtbl -> get_DefaultTargetSchema(This,retval) ) 

#define CSharpProjectProperties4_put_DefaultTargetSchema(This,noname,retval)	\
    ( (This)->lpVtbl -> put_DefaultTargetSchema(This,noname,retval) ) 

#define CSharpProjectProperties4_get_DefaultHTMLPageLayout(This,retval)	\
    ( (This)->lpVtbl -> get_DefaultHTMLPageLayout(This,retval) ) 

#define CSharpProjectProperties4_put_DefaultHTMLPageLayout(This,noname,retval)	\
    ( (This)->lpVtbl -> put_DefaultHTMLPageLayout(This,noname,retval) ) 

#define CSharpProjectProperties4_get_FileName(This,retval)	\
    ( (This)->lpVtbl -> get_FileName(This,retval) ) 

#define CSharpProjectProperties4_put_FileName(This,noname,retval)	\
    ( (This)->lpVtbl -> put_FileName(This,noname,retval) ) 

#define CSharpProjectProperties4_get_FullPath(This,retval)	\
    ( (This)->lpVtbl -> get_FullPath(This,retval) ) 

#define CSharpProjectProperties4_get_LocalPath(This,retval)	\
    ( (This)->lpVtbl -> get_LocalPath(This,retval) ) 

#define CSharpProjectProperties4_get_URL(This,retval)	\
    ( (This)->lpVtbl -> get_URL(This,retval) ) 

#define CSharpProjectProperties4_get_ActiveConfigurationSettings(This,retval)	\
    ( (This)->lpVtbl -> get_ActiveConfigurationSettings(This,retval) ) 

#define CSharpProjectProperties4_get_Extender(This,ExtenderName,retval)	\
    ( (This)->lpVtbl -> get_Extender(This,ExtenderName,retval) ) 

#define CSharpProjectProperties4_get_ExtenderNames(This,retval)	\
    ( (This)->lpVtbl -> get_ExtenderNames(This,retval) ) 

#define CSharpProjectProperties4_get_ExtenderCATID(This,retval)	\
    ( (This)->lpVtbl -> get_ExtenderCATID(This,retval) ) 

#define CSharpProjectProperties4_get_ApplicationIcon(This,retval)	\
    ( (This)->lpVtbl -> get_ApplicationIcon(This,retval) ) 

#define CSharpProjectProperties4_put_ApplicationIcon(This,noname,retval)	\
    ( (This)->lpVtbl -> put_ApplicationIcon(This,noname,retval) ) 

#define CSharpProjectProperties4_get_OptionStrict(This,retval)	\
    ( (This)->lpVtbl -> get_OptionStrict(This,retval) ) 

#define CSharpProjectProperties4_put_OptionStrict(This,noname,retval)	\
    ( (This)->lpVtbl -> put_OptionStrict(This,noname,retval) ) 

#define CSharpProjectProperties4_get_ReferencePath(This,retval)	\
    ( (This)->lpVtbl -> get_ReferencePath(This,retval) ) 

#define CSharpProjectProperties4_put_ReferencePath(This,noname,retval)	\
    ( (This)->lpVtbl -> put_ReferencePath(This,noname,retval) ) 

#define CSharpProjectProperties4_get_OutputFileName(This,retval)	\
    ( (This)->lpVtbl -> get_OutputFileName(This,retval) ) 

#define CSharpProjectProperties4_get_AbsoluteProjectDirectory(This,retval)	\
    ( (This)->lpVtbl -> get_AbsoluteProjectDirectory(This,retval) ) 

#define CSharpProjectProperties4_get_OptionExplicit(This,retval)	\
    ( (This)->lpVtbl -> get_OptionExplicit(This,retval) ) 

#define CSharpProjectProperties4_put_OptionExplicit(This,noname,retval)	\
    ( (This)->lpVtbl -> put_OptionExplicit(This,noname,retval) ) 

#define CSharpProjectProperties4_get_OptionCompare(This,retval)	\
    ( (This)->lpVtbl -> get_OptionCompare(This,retval) ) 

#define CSharpProjectProperties4_put_OptionCompare(This,noname,retval)	\
    ( (This)->lpVtbl -> put_OptionCompare(This,noname,retval) ) 

#define CSharpProjectProperties4_get_ProjectType(This,retval)	\
    ( (This)->lpVtbl -> get_ProjectType(This,retval) ) 

#define CSharpProjectProperties4_get_DefaultNamespace(This,retval)	\
    ( (This)->lpVtbl -> get_DefaultNamespace(This,retval) ) 

#define CSharpProjectProperties4_put_DefaultNamespace(This,noname,retval)	\
    ( (This)->lpVtbl -> put_DefaultNamespace(This,noname,retval) ) 

#define CSharpProjectProperties4_get_PreBuildEvent(This,retval)	\
    ( (This)->lpVtbl -> get_PreBuildEvent(This,retval) ) 

#define CSharpProjectProperties4_put_PreBuildEvent(This,noname,retval)	\
    ( (This)->lpVtbl -> put_PreBuildEvent(This,noname,retval) ) 

#define CSharpProjectProperties4_get_PostBuildEvent(This,retval)	\
    ( (This)->lpVtbl -> get_PostBuildEvent(This,retval) ) 

#define CSharpProjectProperties4_put_PostBuildEvent(This,noname,retval)	\
    ( (This)->lpVtbl -> put_PostBuildEvent(This,noname,retval) ) 

#define CSharpProjectProperties4_get_RunPostBuildEvent(This,retval)	\
    ( (This)->lpVtbl -> get_RunPostBuildEvent(This,retval) ) 

#define CSharpProjectProperties4_put_RunPostBuildEvent(This,noname,retval)	\
    ( (This)->lpVtbl -> put_RunPostBuildEvent(This,noname,retval) ) 

#define CSharpProjectProperties4_get_AspnetVersion(This,retval)	\
    ( (This)->lpVtbl -> get_AspnetVersion(This,retval) ) 

#define CSharpProjectProperties4_get_Title(This,retval)	\
    ( (This)->lpVtbl -> get_Title(This,retval) ) 

#define CSharpProjectProperties4_put_Title(This,noname,retval)	\
    ( (This)->lpVtbl -> put_Title(This,noname,retval) ) 

#define CSharpProjectProperties4_get_Description(This,retval)	\
    ( (This)->lpVtbl -> get_Description(This,retval) ) 

#define CSharpProjectProperties4_put_Description(This,noname,retval)	\
    ( (This)->lpVtbl -> put_Description(This,noname,retval) ) 

#define CSharpProjectProperties4_get_Company(This,retval)	\
    ( (This)->lpVtbl -> get_Company(This,retval) ) 

#define CSharpProjectProperties4_put_Company(This,noname,retval)	\
    ( (This)->lpVtbl -> put_Company(This,noname,retval) ) 

#define CSharpProjectProperties4_get_Product(This,retval)	\
    ( (This)->lpVtbl -> get_Product(This,retval) ) 

#define CSharpProjectProperties4_put_Product(This,noname,retval)	\
    ( (This)->lpVtbl -> put_Product(This,noname,retval) ) 

#define CSharpProjectProperties4_get_Copyright(This,retval)	\
    ( (This)->lpVtbl -> get_Copyright(This,retval) ) 

#define CSharpProjectProperties4_put_Copyright(This,noname,retval)	\
    ( (This)->lpVtbl -> put_Copyright(This,noname,retval) ) 

#define CSharpProjectProperties4_get_Trademark(This,retval)	\
    ( (This)->lpVtbl -> get_Trademark(This,retval) ) 

#define CSharpProjectProperties4_put_Trademark(This,noname,retval)	\
    ( (This)->lpVtbl -> put_Trademark(This,noname,retval) ) 

#define CSharpProjectProperties4_get_AssemblyType(This,retval)	\
    ( (This)->lpVtbl -> get_AssemblyType(This,retval) ) 

#define CSharpProjectProperties4_put_AssemblyType(This,noname,retval)	\
    ( (This)->lpVtbl -> put_AssemblyType(This,noname,retval) ) 

#define CSharpProjectProperties4_get_TypeComplianceDiagnostics(This,retval)	\
    ( (This)->lpVtbl -> get_TypeComplianceDiagnostics(This,retval) ) 

#define CSharpProjectProperties4_put_TypeComplianceDiagnostics(This,noname,retval)	\
    ( (This)->lpVtbl -> put_TypeComplianceDiagnostics(This,noname,retval) ) 

#define CSharpProjectProperties4_get_Win32ResourceFile(This,retval)	\
    ( (This)->lpVtbl -> get_Win32ResourceFile(This,retval) ) 

#define CSharpProjectProperties4_put_Win32ResourceFile(This,noname,retval)	\
    ( (This)->lpVtbl -> put_Win32ResourceFile(This,noname,retval) ) 

#define CSharpProjectProperties4_get_AssemblyKeyProviderName(This,retval)	\
    ( (This)->lpVtbl -> get_AssemblyKeyProviderName(This,retval) ) 

#define CSharpProjectProperties4_put_AssemblyKeyProviderName(This,noname,retval)	\
    ( (This)->lpVtbl -> put_AssemblyKeyProviderName(This,noname,retval) ) 

#define CSharpProjectProperties4_get_AssemblyOriginatorKeyFileType(This,retval)	\
    ( (This)->lpVtbl -> get_AssemblyOriginatorKeyFileType(This,retval) ) 

#define CSharpProjectProperties4_put_AssemblyOriginatorKeyFileType(This,noname,retval)	\
    ( (This)->lpVtbl -> put_AssemblyOriginatorKeyFileType(This,noname,retval) ) 

#define CSharpProjectProperties4_get_AssemblyVersion(This,retval)	\
    ( (This)->lpVtbl -> get_AssemblyVersion(This,retval) ) 

#define CSharpProjectProperties4_put_AssemblyVersion(This,noname,retval)	\
    ( (This)->lpVtbl -> put_AssemblyVersion(This,noname,retval) ) 

#define CSharpProjectProperties4_get_AssemblyFileVersion(This,retval)	\
    ( (This)->lpVtbl -> get_AssemblyFileVersion(This,retval) ) 

#define CSharpProjectProperties4_put_AssemblyFileVersion(This,noname,retval)	\
    ( (This)->lpVtbl -> put_AssemblyFileVersion(This,noname,retval) ) 

#define CSharpProjectProperties4_get_GenerateManifests(This,retval)	\
    ( (This)->lpVtbl -> get_GenerateManifests(This,retval) ) 

#define CSharpProjectProperties4_put_GenerateManifests(This,noname,retval)	\
    ( (This)->lpVtbl -> put_GenerateManifests(This,noname,retval) ) 

#define CSharpProjectProperties4_get_EnableSecurityDebugging(This,retval)	\
    ( (This)->lpVtbl -> get_EnableSecurityDebugging(This,retval) ) 

#define CSharpProjectProperties4_put_EnableSecurityDebugging(This,noname,retval)	\
    ( (This)->lpVtbl -> put_EnableSecurityDebugging(This,noname,retval) ) 

#define CSharpProjectProperties4_get_DebugSecurityZoneURL(This,retval)	\
    ( (This)->lpVtbl -> get_DebugSecurityZoneURL(This,retval) ) 

#define CSharpProjectProperties4_put_DebugSecurityZoneURL(This,noname,retval)	\
    ( (This)->lpVtbl -> put_DebugSecurityZoneURL(This,noname,retval) ) 

#define CSharpProjectProperties4_get_Publish(This,retval)	\
    ( (This)->lpVtbl -> get_Publish(This,retval) ) 

#define CSharpProjectProperties4_get_ComVisible(This,retval)	\
    ( (This)->lpVtbl -> get_ComVisible(This,retval) ) 

#define CSharpProjectProperties4_put_ComVisible(This,noname,retval)	\
    ( (This)->lpVtbl -> put_ComVisible(This,noname,retval) ) 

#define CSharpProjectProperties4_get_AssemblyGuid(This,retval)	\
    ( (This)->lpVtbl -> get_AssemblyGuid(This,retval) ) 

#define CSharpProjectProperties4_put_AssemblyGuid(This,noname,retval)	\
    ( (This)->lpVtbl -> put_AssemblyGuid(This,noname,retval) ) 

#define CSharpProjectProperties4_get_NeutralResourcesLanguage(This,retval)	\
    ( (This)->lpVtbl -> get_NeutralResourcesLanguage(This,retval) ) 

#define CSharpProjectProperties4_put_NeutralResourcesLanguage(This,noname,retval)	\
    ( (This)->lpVtbl -> put_NeutralResourcesLanguage(This,noname,retval) ) 

#define CSharpProjectProperties4_get_SignAssembly(This,retval)	\
    ( (This)->lpVtbl -> get_SignAssembly(This,retval) ) 

#define CSharpProjectProperties4_put_SignAssembly(This,noname,retval)	\
    ( (This)->lpVtbl -> put_SignAssembly(This,noname,retval) ) 

#define CSharpProjectProperties4_get_SignManifests(This,retval)	\
    ( (This)->lpVtbl -> get_SignManifests(This,retval) ) 

#define CSharpProjectProperties4_put_SignManifests(This,noname,retval)	\
    ( (This)->lpVtbl -> put_SignManifests(This,noname,retval) ) 

#define CSharpProjectProperties4_get_TargetZone(This,retval)	\
    ( (This)->lpVtbl -> get_TargetZone(This,retval) ) 

#define CSharpProjectProperties4_put_TargetZone(This,noname,retval)	\
    ( (This)->lpVtbl -> put_TargetZone(This,noname,retval) ) 

#define CSharpProjectProperties4_get_ExcludedPermissions(This,retval)	\
    ( (This)->lpVtbl -> get_ExcludedPermissions(This,retval) ) 

#define CSharpProjectProperties4_put_ExcludedPermissions(This,noname,retval)	\
    ( (This)->lpVtbl -> put_ExcludedPermissions(This,noname,retval) ) 

#define CSharpProjectProperties4_get_ManifestCertificateThumbprint(This,retval)	\
    ( (This)->lpVtbl -> get_ManifestCertificateThumbprint(This,retval) ) 

#define CSharpProjectProperties4_put_ManifestCertificateThumbprint(This,noname,retval)	\
    ( (This)->lpVtbl -> put_ManifestCertificateThumbprint(This,noname,retval) ) 

#define CSharpProjectProperties4_get_ManifestKeyFile(This,retval)	\
    ( (This)->lpVtbl -> get_ManifestKeyFile(This,retval) ) 

#define CSharpProjectProperties4_put_ManifestKeyFile(This,noname,retval)	\
    ( (This)->lpVtbl -> put_ManifestKeyFile(This,noname,retval) ) 

#define CSharpProjectProperties4_get_ManifestTimestampUrl(This,retval)	\
    ( (This)->lpVtbl -> get_ManifestTimestampUrl(This,retval) ) 

#define CSharpProjectProperties4_put_ManifestTimestampUrl(This,noname,retval)	\
    ( (This)->lpVtbl -> put_ManifestTimestampUrl(This,noname,retval) ) 


#define CSharpProjectProperties4_get_TargetFramework(This,pdwTargetFramework)	\
    ( (This)->lpVtbl -> get_TargetFramework(This,pdwTargetFramework) ) 

#define CSharpProjectProperties4_put_TargetFramework(This,dwTargetFramework)	\
    ( (This)->lpVtbl -> put_TargetFramework(This,dwTargetFramework) ) 

#define CSharpProjectProperties4_get_ApplicationManifest(This,pbstrApplicationManifest)	\
    ( (This)->lpVtbl -> get_ApplicationManifest(This,pbstrApplicationManifest) ) 

#define CSharpProjectProperties4_put_ApplicationManifest(This,ApplicationManifest)	\
    ( (This)->lpVtbl -> put_ApplicationManifest(This,ApplicationManifest) ) 

#endif /* COBJMACROS */


#endif 	/* C style interface */



/* [helpstring][id][nonbrowsable][propput] */ HRESULT STDMETHODCALLTYPE CSharpProjectProperties4_put_TargetFramework_Proxy( 
    __RPC__in CSharpProjectProperties4 * This,
    /* [in] */ DWORD dwTargetFramework);


void __RPC_STUB CSharpProjectProperties4_put_TargetFramework_Stub(
    IRpcStubBuffer *This,
    IRpcChannelBuffer *_pRpcChannelBuffer,
    PRPC_MESSAGE _pRpcMessage,
    DWORD *_pdwStubPhase);


/* [helpstring][id][propget] */ HRESULT STDMETHODCALLTYPE CSharpProjectProperties4_get_ApplicationManifest_Proxy( 
    __RPC__in CSharpProjectProperties4 * This,
    /* [retval][out] */ __RPC__deref_out_opt BSTR *pbstrApplicationManifest);


void __RPC_STUB CSharpProjectProperties4_get_ApplicationManifest_Stub(
    IRpcStubBuffer *This,
    IRpcChannelBuffer *_pRpcChannelBuffer,
    PRPC_MESSAGE _pRpcMessage,
    DWORD *_pdwStubPhase);


/* [helpstring][id][propput] */ HRESULT STDMETHODCALLTYPE CSharpProjectProperties4_put_ApplicationManifest_Proxy( 
    __RPC__in CSharpProjectProperties4 * This,
    /* [in] */ __RPC__in BSTR ApplicationManifest);


void __RPC_STUB CSharpProjectProperties4_put_ApplicationManifest_Stub(
    IRpcStubBuffer *This,
    IRpcChannelBuffer *_pRpcChannelBuffer,
    PRPC_MESSAGE _pRpcMessage,
    DWORD *_pdwStubPhase);



#endif 	/* __CSharpProjectProperties4_INTERFACE_DEFINED__ */


#ifndef __CSharpProjectConfigurationProperties4_INTERFACE_DEFINED__
#define __CSharpProjectConfigurationProperties4_INTERFACE_DEFINED__

/* interface CSharpProjectConfigurationProperties4 */
/* [object][dual][unique][helpstring][uuid] */ 


EXTERN_C const IID IID_CSharpProjectConfigurationProperties4;

#if defined(__cplusplus) && !defined(CINTERFACE)
    
    MIDL_INTERFACE("dd47c0d5-5095-4a44-ac96-105b2f194f11")
    CSharpProjectConfigurationProperties4 : public CSharpProjectConfigurationProperties3
    {
    public:
        virtual /* [helpstring][id][propget] */ HRESULT STDMETHODCALLTYPE get_CodeAnalysisIgnoreGeneratedCode( 
            /* [retval][out] */ __RPC__out VARIANT_BOOL *pbIgnoreGeneratedCode) = 0;
        
        virtual /* [helpstring][id][propput] */ HRESULT STDMETHODCALLTYPE put_CodeAnalysisIgnoreGeneratedCode( 
            /* [in] */ VARIANT_BOOL IgnoreGeneratedCode) = 0;
        
        virtual /* [helpstring][id][propget] */ HRESULT STDMETHODCALLTYPE get_CodeAnalysisOverrideRuleVisibilities( 
            /* [retval][out] */ __RPC__out VARIANT_BOOL *pbOverrideRuleVisibilities) = 0;
        
        virtual /* [helpstring][id][propput] */ HRESULT STDMETHODCALLTYPE put_CodeAnalysisOverrideRuleVisibilities( 
            /* [in] */ VARIANT_BOOL OverrideRuleVisibilities) = 0;
        
        virtual /* [helpstring][id][propget] */ HRESULT STDMETHODCALLTYPE get_CodeAnalysisDictionaries( 
            /* [retval][out] */ __RPC__deref_out_opt BSTR *pbstrDictionaries) = 0;
        
        virtual /* [helpstring][id][propput] */ HRESULT STDMETHODCALLTYPE put_CodeAnalysisDictionaries( 
            /* [in] */ __RPC__in BSTR Dictionaries) = 0;
        
        virtual /* [helpstring][id][propget] */ HRESULT STDMETHODCALLTYPE get_CodeAnalysisCulture( 
            /* [retval][out] */ __RPC__deref_out_opt BSTR *pbstrCodeAnalysisCulture) = 0;
        
        virtual /* [helpstring][id][propput] */ HRESULT STDMETHODCALLTYPE put_CodeAnalysisCulture( 
            /* [in] */ __RPC__in BSTR pbstrCodeAnalysisCulture) = 0;
        
    };
    
    
#else 	/* C style interface */

    typedef struct CSharpProjectConfigurationProperties4Vtbl
    {
        BEGIN_INTERFACE
        
        /* [id][restricted][funcdescattr] */ HRESULT ( STDMETHODCALLTYPE *QueryInterface )( 
            __RPC__in CSharpProjectConfigurationProperties4 * This,
            /* [in][idldescattr] */ __RPC__in struct GUID *riid,
            /* [out][idldescattr] */ __RPC__deref_out_opt void **ppvObj,
            /* [retval][out] */ __RPC__out void *retval);
        
        /* [id][restricted][funcdescattr] */ HRESULT ( STDMETHODCALLTYPE *AddRef )( 
            __RPC__in CSharpProjectConfigurationProperties4 * This,
            /* [retval][out] */ __RPC__out unsigned long *retval);
        
        /* [id][restricted][funcdescattr] */ HRESULT ( STDMETHODCALLTYPE *Release )( 
            __RPC__in CSharpProjectConfigurationProperties4 * This,
            /* [retval][out] */ __RPC__out unsigned long *retval);
        
        /* [id][restricted][funcdescattr] */ HRESULT ( STDMETHODCALLTYPE *GetTypeInfoCount )( 
            __RPC__in CSharpProjectConfigurationProperties4 * This,
            /* [out][idldescattr] */ __RPC__out unsigned UINT *pctinfo,
            /* [retval][out] */ __RPC__out void *retval);
        
        /* [id][restricted][funcdescattr] */ HRESULT ( STDMETHODCALLTYPE *GetTypeInfo )( 
            __RPC__in CSharpProjectConfigurationProperties4 * This,
            /* [in][idldescattr] */ unsigned UINT itinfo,
            /* [in][idldescattr] */ unsigned long lcid,
            /* [out][idldescattr] */ __RPC__deref_out_opt void **pptinfo,
            /* [retval][out] */ __RPC__out void *retval);
        
        /* [id][restricted][funcdescattr] */ HRESULT ( STDMETHODCALLTYPE *GetIDsOfNames )( 
            __RPC__in CSharpProjectConfigurationProperties4 * This,
            /* [in][idldescattr] */ __RPC__in struct GUID *riid,
            /* [in][idldescattr] */ __RPC__deref_in_opt signed char **rgszNames,
            /* [in][idldescattr] */ unsigned UINT cNames,
            /* [in][idldescattr] */ unsigned long lcid,
            /* [out][idldescattr] */ __RPC__out signed long *rgdispid,
            /* [retval][out] */ __RPC__out void *retval);
        
        /* [id][restricted][funcdescattr] */ HRESULT ( STDMETHODCALLTYPE *Invoke )( 
            __RPC__in CSharpProjectConfigurationProperties4 * This,
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
            __RPC__in CSharpProjectConfigurationProperties4 * This,
            /* [retval][out] */ __RPC__deref_out_opt BSTR *retval);
        
        /* [id][propget][funcdescattr] */ HRESULT ( STDMETHODCALLTYPE *get_DebugSymbols )( 
            __RPC__in CSharpProjectConfigurationProperties4 * This,
            /* [retval][out] */ __RPC__out BOOLEAN *retval);
        
        /* [id][propput][funcdescattr] */ HRESULT ( STDMETHODCALLTYPE *put_DebugSymbols )( 
            __RPC__in CSharpProjectConfigurationProperties4 * This,
            /* [in][idldescattr] */ BOOLEAN noname,
            /* [retval][out] */ __RPC__out void *retval);
        
        /* [id][propget][funcdescattr] */ HRESULT ( STDMETHODCALLTYPE *get_DefineDebug )( 
            __RPC__in CSharpProjectConfigurationProperties4 * This,
            /* [retval][out] */ __RPC__out BOOLEAN *retval);
        
        /* [id][propput][funcdescattr] */ HRESULT ( STDMETHODCALLTYPE *put_DefineDebug )( 
            __RPC__in CSharpProjectConfigurationProperties4 * This,
            /* [in][idldescattr] */ BOOLEAN noname,
            /* [retval][out] */ __RPC__out void *retval);
        
        /* [id][propget][funcdescattr] */ HRESULT ( STDMETHODCALLTYPE *get_DefineTrace )( 
            __RPC__in CSharpProjectConfigurationProperties4 * This,
            /* [retval][out] */ __RPC__out BOOLEAN *retval);
        
        /* [id][propput][funcdescattr] */ HRESULT ( STDMETHODCALLTYPE *put_DefineTrace )( 
            __RPC__in CSharpProjectConfigurationProperties4 * This,
            /* [in][idldescattr] */ BOOLEAN noname,
            /* [retval][out] */ __RPC__out void *retval);
        
        /* [id][propget][funcdescattr] */ HRESULT ( STDMETHODCALLTYPE *get_OutputPath )( 
            __RPC__in CSharpProjectConfigurationProperties4 * This,
            /* [retval][out] */ __RPC__deref_out_opt BSTR *retval);
        
        /* [id][propput][funcdescattr] */ HRESULT ( STDMETHODCALLTYPE *put_OutputPath )( 
            __RPC__in CSharpProjectConfigurationProperties4 * This,
            /* [in][idldescattr] */ __RPC__in BSTR noname,
            /* [retval][out] */ __RPC__out void *retval);
        
        /* [id][propget][funcdescattr] */ HRESULT ( STDMETHODCALLTYPE *get_IntermediatePath )( 
            __RPC__in CSharpProjectConfigurationProperties4 * This,
            /* [retval][out] */ __RPC__deref_out_opt BSTR *retval);
        
        /* [id][propput][hidden][funcdescattr] */ HRESULT ( STDMETHODCALLTYPE *put_IntermediatePath )( 
            __RPC__in CSharpProjectConfigurationProperties4 * This,
            /* [in][idldescattr] */ __RPC__in BSTR noname,
            /* [retval][out] */ __RPC__out void *retval);
        
        /* [id][propget][funcdescattr] */ HRESULT ( STDMETHODCALLTYPE *get_DefineConstants )( 
            __RPC__in CSharpProjectConfigurationProperties4 * This,
            /* [retval][out] */ __RPC__deref_out_opt BSTR *retval);
        
        /* [id][propput][funcdescattr] */ HRESULT ( STDMETHODCALLTYPE *put_DefineConstants )( 
            __RPC__in CSharpProjectConfigurationProperties4 * This,
            /* [in][idldescattr] */ __RPC__in BSTR noname,
            /* [retval][out] */ __RPC__out void *retval);
        
        /* [id][propget][funcdescattr] */ HRESULT ( STDMETHODCALLTYPE *get_RemoveIntegerChecks )( 
            __RPC__in CSharpProjectConfigurationProperties4 * This,
            /* [retval][out] */ __RPC__out BOOLEAN *retval);
        
        /* [id][propput][funcdescattr] */ HRESULT ( STDMETHODCALLTYPE *put_RemoveIntegerChecks )( 
            __RPC__in CSharpProjectConfigurationProperties4 * This,
            /* [in][idldescattr] */ BOOLEAN noname,
            /* [retval][out] */ __RPC__out void *retval);
        
        /* [id][propget][funcdescattr] */ HRESULT ( STDMETHODCALLTYPE *get_BaseAddress )( 
            __RPC__in CSharpProjectConfigurationProperties4 * This,
            /* [retval][out] */ __RPC__out unsigned long *retval);
        
        /* [id][propput][funcdescattr] */ HRESULT ( STDMETHODCALLTYPE *put_BaseAddress )( 
            __RPC__in CSharpProjectConfigurationProperties4 * This,
            /* [in][idldescattr] */ unsigned long noname,
            /* [retval][out] */ __RPC__out void *retval);
        
        /* [id][propget][funcdescattr] */ HRESULT ( STDMETHODCALLTYPE *get_AllowUnsafeBlocks )( 
            __RPC__in CSharpProjectConfigurationProperties4 * This,
            /* [retval][out] */ __RPC__out BOOLEAN *retval);
        
        /* [id][propput][funcdescattr] */ HRESULT ( STDMETHODCALLTYPE *put_AllowUnsafeBlocks )( 
            __RPC__in CSharpProjectConfigurationProperties4 * This,
            /* [in][idldescattr] */ BOOLEAN noname,
            /* [retval][out] */ __RPC__out void *retval);
        
        /* [id][propget][funcdescattr] */ HRESULT ( STDMETHODCALLTYPE *get_CheckForOverflowUnderflow )( 
            __RPC__in CSharpProjectConfigurationProperties4 * This,
            /* [retval][out] */ __RPC__out BOOLEAN *retval);
        
        /* [id][propput][funcdescattr] */ HRESULT ( STDMETHODCALLTYPE *put_CheckForOverflowUnderflow )( 
            __RPC__in CSharpProjectConfigurationProperties4 * This,
            /* [in][idldescattr] */ BOOLEAN noname,
            /* [retval][out] */ __RPC__out void *retval);
        
        /* [id][propget][funcdescattr] */ HRESULT ( STDMETHODCALLTYPE *get_DocumentationFile )( 
            __RPC__in CSharpProjectConfigurationProperties4 * This,
            /* [retval][out] */ __RPC__deref_out_opt BSTR *retval);
        
        /* [id][propput][funcdescattr] */ HRESULT ( STDMETHODCALLTYPE *put_DocumentationFile )( 
            __RPC__in CSharpProjectConfigurationProperties4 * This,
            /* [in][idldescattr] */ __RPC__in BSTR noname,
            /* [retval][out] */ __RPC__out void *retval);
        
        /* [id][propget][funcdescattr] */ HRESULT ( STDMETHODCALLTYPE *get_Optimize )( 
            __RPC__in CSharpProjectConfigurationProperties4 * This,
            /* [retval][out] */ __RPC__out BOOLEAN *retval);
        
        /* [id][propput][funcdescattr] */ HRESULT ( STDMETHODCALLTYPE *put_Optimize )( 
            __RPC__in CSharpProjectConfigurationProperties4 * This,
            /* [in][idldescattr] */ BOOLEAN noname,
            /* [retval][out] */ __RPC__out void *retval);
        
        /* [id][propget][funcdescattr] */ HRESULT ( STDMETHODCALLTYPE *get_IncrementalBuild )( 
            __RPC__in CSharpProjectConfigurationProperties4 * This,
            /* [retval][out] */ __RPC__out BOOLEAN *retval);
        
        /* [id][propput][funcdescattr] */ HRESULT ( STDMETHODCALLTYPE *put_IncrementalBuild )( 
            __RPC__in CSharpProjectConfigurationProperties4 * This,
            /* [in][idldescattr] */ BOOLEAN noname,
            /* [retval][out] */ __RPC__out void *retval);
        
        /* [id][propget][funcdescattr] */ HRESULT ( STDMETHODCALLTYPE *get_StartProgram )( 
            __RPC__in CSharpProjectConfigurationProperties4 * This,
            /* [retval][out] */ __RPC__deref_out_opt BSTR *retval);
        
        /* [id][propput][funcdescattr] */ HRESULT ( STDMETHODCALLTYPE *put_StartProgram )( 
            __RPC__in CSharpProjectConfigurationProperties4 * This,
            /* [in][idldescattr] */ __RPC__in BSTR noname,
            /* [retval][out] */ __RPC__out void *retval);
        
        /* [id][propget][funcdescattr] */ HRESULT ( STDMETHODCALLTYPE *get_StartWorkingDirectory )( 
            __RPC__in CSharpProjectConfigurationProperties4 * This,
            /* [retval][out] */ __RPC__deref_out_opt BSTR *retval);
        
        /* [id][propput][funcdescattr] */ HRESULT ( STDMETHODCALLTYPE *put_StartWorkingDirectory )( 
            __RPC__in CSharpProjectConfigurationProperties4 * This,
            /* [in][idldescattr] */ __RPC__in BSTR noname,
            /* [retval][out] */ __RPC__out void *retval);
        
        /* [id][propget][funcdescattr] */ HRESULT ( STDMETHODCALLTYPE *get_StartURL )( 
            __RPC__in CSharpProjectConfigurationProperties4 * This,
            /* [retval][out] */ __RPC__deref_out_opt BSTR *retval);
        
        /* [id][propput][funcdescattr] */ HRESULT ( STDMETHODCALLTYPE *put_StartURL )( 
            __RPC__in CSharpProjectConfigurationProperties4 * This,
            /* [in][idldescattr] */ __RPC__in BSTR noname,
            /* [retval][out] */ __RPC__out void *retval);
        
        /* [id][propget][funcdescattr] */ HRESULT ( STDMETHODCALLTYPE *get_StartPage )( 
            __RPC__in CSharpProjectConfigurationProperties4 * This,
            /* [retval][out] */ __RPC__deref_out_opt BSTR *retval);
        
        /* [id][propput][funcdescattr] */ HRESULT ( STDMETHODCALLTYPE *put_StartPage )( 
            __RPC__in CSharpProjectConfigurationProperties4 * This,
            /* [in][idldescattr] */ __RPC__in BSTR noname,
            /* [retval][out] */ __RPC__out void *retval);
        
        /* [id][propget][funcdescattr] */ HRESULT ( STDMETHODCALLTYPE *get_StartArguments )( 
            __RPC__in CSharpProjectConfigurationProperties4 * This,
            /* [retval][out] */ __RPC__deref_out_opt BSTR *retval);
        
        /* [id][propput][funcdescattr] */ HRESULT ( STDMETHODCALLTYPE *put_StartArguments )( 
            __RPC__in CSharpProjectConfigurationProperties4 * This,
            /* [in][idldescattr] */ __RPC__in BSTR noname,
            /* [retval][out] */ __RPC__out void *retval);
        
        /* [id][propget][funcdescattr] */ HRESULT ( STDMETHODCALLTYPE *get_StartWithIE )( 
            __RPC__in CSharpProjectConfigurationProperties4 * This,
            /* [retval][out] */ __RPC__out BOOLEAN *retval);
        
        /* [id][propput][funcdescattr] */ HRESULT ( STDMETHODCALLTYPE *put_StartWithIE )( 
            __RPC__in CSharpProjectConfigurationProperties4 * This,
            /* [in][idldescattr] */ BOOLEAN noname,
            /* [retval][out] */ __RPC__out void *retval);
        
        /* [id][propget][funcdescattr] */ HRESULT ( STDMETHODCALLTYPE *get_EnableASPDebugging )( 
            __RPC__in CSharpProjectConfigurationProperties4 * This,
            /* [retval][out] */ __RPC__out BOOLEAN *retval);
        
        /* [id][propput][funcdescattr] */ HRESULT ( STDMETHODCALLTYPE *put_EnableASPDebugging )( 
            __RPC__in CSharpProjectConfigurationProperties4 * This,
            /* [in][idldescattr] */ BOOLEAN noname,
            /* [retval][out] */ __RPC__out void *retval);
        
        /* [id][propget][funcdescattr] */ HRESULT ( STDMETHODCALLTYPE *get_EnableASPXDebugging )( 
            __RPC__in CSharpProjectConfigurationProperties4 * This,
            /* [retval][out] */ __RPC__out BOOLEAN *retval);
        
        /* [id][propput][funcdescattr] */ HRESULT ( STDMETHODCALLTYPE *put_EnableASPXDebugging )( 
            __RPC__in CSharpProjectConfigurationProperties4 * This,
            /* [in][idldescattr] */ BOOLEAN noname,
            /* [retval][out] */ __RPC__out void *retval);
        
        /* [id][propget][funcdescattr] */ HRESULT ( STDMETHODCALLTYPE *get_EnableUnmanagedDebugging )( 
            __RPC__in CSharpProjectConfigurationProperties4 * This,
            /* [retval][out] */ __RPC__out BOOLEAN *retval);
        
        /* [id][propput][funcdescattr] */ HRESULT ( STDMETHODCALLTYPE *put_EnableUnmanagedDebugging )( 
            __RPC__in CSharpProjectConfigurationProperties4 * This,
            /* [in][idldescattr] */ BOOLEAN noname,
            /* [retval][out] */ __RPC__out void *retval);
        
        /* [id][propget][funcdescattr] */ HRESULT ( STDMETHODCALLTYPE *get_StartAction )( 
            __RPC__in CSharpProjectConfigurationProperties4 * This,
            /* [retval][out] */ __RPC__out enum prjStartAction *retval);
        
        /* [id][propput][funcdescattr] */ HRESULT ( STDMETHODCALLTYPE *put_StartAction )( 
            __RPC__in CSharpProjectConfigurationProperties4 * This,
            /* [in][idldescattr] */ enum prjStartAction noname,
            /* [retval][out] */ __RPC__out void *retval);
        
        /* [id][propget][funcdescattr] */ HRESULT ( STDMETHODCALLTYPE *get_Extender )( 
            __RPC__in CSharpProjectConfigurationProperties4 * This,
            /* [in][idldescattr] */ __RPC__in BSTR ExtenderName,
            /* [retval][out] */ __RPC__deref_out_opt IDispatch **retval);
        
        /* [id][propget][funcdescattr] */ HRESULT ( STDMETHODCALLTYPE *get_ExtenderNames )( 
            __RPC__in CSharpProjectConfigurationProperties4 * This,
            /* [retval][out] */ __RPC__out VARIANT *retval);
        
        /* [id][propget][funcdescattr] */ HRESULT ( STDMETHODCALLTYPE *get_ExtenderCATID )( 
            __RPC__in CSharpProjectConfigurationProperties4 * This,
            /* [retval][out] */ __RPC__deref_out_opt BSTR *retval);
        
        /* [id][propget][funcdescattr] */ HRESULT ( STDMETHODCALLTYPE *get_WarningLevel )( 
            __RPC__in CSharpProjectConfigurationProperties4 * This,
            /* [retval][out] */ __RPC__out enum prjWarningLevel *retval);
        
        /* [id][propput][funcdescattr] */ HRESULT ( STDMETHODCALLTYPE *put_WarningLevel )( 
            __RPC__in CSharpProjectConfigurationProperties4 * This,
            /* [in][idldescattr] */ enum prjWarningLevel noname,
            /* [retval][out] */ __RPC__out void *retval);
        
        /* [id][propget][funcdescattr] */ HRESULT ( STDMETHODCALLTYPE *get_TreatWarningsAsErrors )( 
            __RPC__in CSharpProjectConfigurationProperties4 * This,
            /* [retval][out] */ __RPC__out BOOLEAN *retval);
        
        /* [id][propput][funcdescattr] */ HRESULT ( STDMETHODCALLTYPE *put_TreatWarningsAsErrors )( 
            __RPC__in CSharpProjectConfigurationProperties4 * This,
            /* [in][idldescattr] */ BOOLEAN noname,
            /* [retval][out] */ __RPC__out void *retval);
        
        /* [id][propget][funcdescattr] */ HRESULT ( STDMETHODCALLTYPE *get_EnableSQLServerDebugging )( 
            __RPC__in CSharpProjectConfigurationProperties4 * This,
            /* [retval][out] */ __RPC__out BOOLEAN *retval);
        
        /* [id][propput][funcdescattr] */ HRESULT ( STDMETHODCALLTYPE *put_EnableSQLServerDebugging )( 
            __RPC__in CSharpProjectConfigurationProperties4 * This,
            /* [in][idldescattr] */ BOOLEAN noname,
            /* [retval][out] */ __RPC__out void *retval);
        
        /* [id][propget][funcdescattr] */ HRESULT ( STDMETHODCALLTYPE *get_FileAlignment )( 
            __RPC__in CSharpProjectConfigurationProperties4 * This,
            /* [retval][out] */ __RPC__out unsigned long *retval);
        
        /* [id][propput][funcdescattr] */ HRESULT ( STDMETHODCALLTYPE *put_FileAlignment )( 
            __RPC__in CSharpProjectConfigurationProperties4 * This,
            /* [in][idldescattr] */ unsigned long noname,
            /* [retval][out] */ __RPC__out void *retval);
        
        /* [id][propget][funcdescattr] */ HRESULT ( STDMETHODCALLTYPE *get_RegisterForComInterop )( 
            __RPC__in CSharpProjectConfigurationProperties4 * This,
            /* [retval][out] */ __RPC__out BOOLEAN *retval);
        
        /* [id][propput][funcdescattr] */ HRESULT ( STDMETHODCALLTYPE *put_RegisterForComInterop )( 
            __RPC__in CSharpProjectConfigurationProperties4 * This,
            /* [in][idldescattr] */ BOOLEAN noname,
            /* [retval][out] */ __RPC__out void *retval);
        
        /* [id][propget][funcdescattr] */ HRESULT ( STDMETHODCALLTYPE *get_ConfigurationOverrideFile )( 
            __RPC__in CSharpProjectConfigurationProperties4 * This,
            /* [retval][out] */ __RPC__deref_out_opt BSTR *retval);
        
        /* [id][propput][funcdescattr] */ HRESULT ( STDMETHODCALLTYPE *put_ConfigurationOverrideFile )( 
            __RPC__in CSharpProjectConfigurationProperties4 * This,
            /* [in][idldescattr] */ __RPC__in BSTR noname,
            /* [retval][out] */ __RPC__out void *retval);
        
        /* [id][propget][funcdescattr] */ HRESULT ( STDMETHODCALLTYPE *get_RemoteDebugEnabled )( 
            __RPC__in CSharpProjectConfigurationProperties4 * This,
            /* [retval][out] */ __RPC__out BOOLEAN *retval);
        
        /* [id][propput][funcdescattr] */ HRESULT ( STDMETHODCALLTYPE *put_RemoteDebugEnabled )( 
            __RPC__in CSharpProjectConfigurationProperties4 * This,
            /* [in][idldescattr] */ BOOLEAN noname,
            /* [retval][out] */ __RPC__out void *retval);
        
        /* [id][propget][funcdescattr] */ HRESULT ( STDMETHODCALLTYPE *get_RemoteDebugMachine )( 
            __RPC__in CSharpProjectConfigurationProperties4 * This,
            /* [retval][out] */ __RPC__deref_out_opt BSTR *retval);
        
        /* [id][propput][funcdescattr] */ HRESULT ( STDMETHODCALLTYPE *put_RemoteDebugMachine )( 
            __RPC__in CSharpProjectConfigurationProperties4 * This,
            /* [in][idldescattr] */ __RPC__in BSTR noname,
            /* [retval][out] */ __RPC__out void *retval);
        
        /* [id][propget][funcdescattr] */ HRESULT ( STDMETHODCALLTYPE *get_NoWarn )( 
            __RPC__in CSharpProjectConfigurationProperties4 * This,
            /* [retval][out] */ __RPC__deref_out_opt BSTR *retval);
        
        /* [id][propput][funcdescattr] */ HRESULT ( STDMETHODCALLTYPE *put_NoWarn )( 
            __RPC__in CSharpProjectConfigurationProperties4 * This,
            /* [in][idldescattr] */ __RPC__in BSTR noname,
            /* [retval][out] */ __RPC__out void *retval);
        
        /* [id][propget][funcdescattr] */ HRESULT ( STDMETHODCALLTYPE *get_NoStdLib )( 
            __RPC__in CSharpProjectConfigurationProperties4 * This,
            /* [retval][out] */ __RPC__out BOOLEAN *retval);
        
        /* [id][propput][funcdescattr] */ HRESULT ( STDMETHODCALLTYPE *put_NoStdLib )( 
            __RPC__in CSharpProjectConfigurationProperties4 * This,
            /* [in][idldescattr] */ BOOLEAN noname,
            /* [retval][out] */ __RPC__out void *retval);
        
        /* [id][propget][funcdescattr] */ HRESULT ( STDMETHODCALLTYPE *get_DebugInfo )( 
            __RPC__in CSharpProjectConfigurationProperties4 * This,
            /* [retval][out] */ __RPC__deref_out_opt BSTR *retval);
        
        /* [id][propput][funcdescattr] */ HRESULT ( STDMETHODCALLTYPE *put_DebugInfo )( 
            __RPC__in CSharpProjectConfigurationProperties4 * This,
            /* [in][idldescattr] */ __RPC__in BSTR noname,
            /* [retval][out] */ __RPC__out void *retval);
        
        /* [id][propget][funcdescattr] */ HRESULT ( STDMETHODCALLTYPE *get_PlatformTarget )( 
            __RPC__in CSharpProjectConfigurationProperties4 * This,
            /* [retval][out] */ __RPC__deref_out_opt BSTR *retval);
        
        /* [id][propput][funcdescattr] */ HRESULT ( STDMETHODCALLTYPE *put_PlatformTarget )( 
            __RPC__in CSharpProjectConfigurationProperties4 * This,
            /* [in][idldescattr] */ __RPC__in BSTR noname,
            /* [retval][out] */ __RPC__out void *retval);
        
        /* [id][propget][funcdescattr] */ HRESULT ( STDMETHODCALLTYPE *get_TreatSpecificWarningsAsErrors )( 
            __RPC__in CSharpProjectConfigurationProperties4 * This,
            /* [retval][out] */ __RPC__deref_out_opt BSTR *retval);
        
        /* [id][propput][funcdescattr] */ HRESULT ( STDMETHODCALLTYPE *put_TreatSpecificWarningsAsErrors )( 
            __RPC__in CSharpProjectConfigurationProperties4 * This,
            /* [in][idldescattr] */ __RPC__in BSTR noname,
            /* [retval][out] */ __RPC__out void *retval);
        
        /* [id][propget][funcdescattr] */ HRESULT ( STDMETHODCALLTYPE *get_RunCodeAnalysis )( 
            __RPC__in CSharpProjectConfigurationProperties4 * This,
            /* [retval][out] */ __RPC__out BOOLEAN *retval);
        
        /* [id][propput][funcdescattr] */ HRESULT ( STDMETHODCALLTYPE *put_RunCodeAnalysis )( 
            __RPC__in CSharpProjectConfigurationProperties4 * This,
            /* [in][idldescattr] */ BOOLEAN noname,
            /* [retval][out] */ __RPC__out void *retval);
        
        /* [id][propget][funcdescattr] */ HRESULT ( STDMETHODCALLTYPE *get_CodeAnalysisLogFile )( 
            __RPC__in CSharpProjectConfigurationProperties4 * This,
            /* [retval][out] */ __RPC__deref_out_opt BSTR *retval);
        
        /* [id][propput][funcdescattr] */ HRESULT ( STDMETHODCALLTYPE *put_CodeAnalysisLogFile )( 
            __RPC__in CSharpProjectConfigurationProperties4 * This,
            /* [in][idldescattr] */ __RPC__in BSTR noname,
            /* [retval][out] */ __RPC__out void *retval);
        
        /* [id][propget][funcdescattr] */ HRESULT ( STDMETHODCALLTYPE *get_CodeAnalysisRuleAssemblies )( 
            __RPC__in CSharpProjectConfigurationProperties4 * This,
            /* [retval][out] */ __RPC__deref_out_opt BSTR *retval);
        
        /* [id][propput][funcdescattr] */ HRESULT ( STDMETHODCALLTYPE *put_CodeAnalysisRuleAssemblies )( 
            __RPC__in CSharpProjectConfigurationProperties4 * This,
            /* [in][idldescattr] */ __RPC__in BSTR noname,
            /* [retval][out] */ __RPC__out void *retval);
        
        /* [id][propget][funcdescattr] */ HRESULT ( STDMETHODCALLTYPE *get_CodeAnalysisInputAssembly )( 
            __RPC__in CSharpProjectConfigurationProperties4 * This,
            /* [retval][out] */ __RPC__deref_out_opt BSTR *retval);
        
        /* [id][propput][funcdescattr] */ HRESULT ( STDMETHODCALLTYPE *put_CodeAnalysisInputAssembly )( 
            __RPC__in CSharpProjectConfigurationProperties4 * This,
            /* [in][idldescattr] */ __RPC__in BSTR noname,
            /* [retval][out] */ __RPC__out void *retval);
        
        /* [id][propget][funcdescattr] */ HRESULT ( STDMETHODCALLTYPE *get_CodeAnalysisRules )( 
            __RPC__in CSharpProjectConfigurationProperties4 * This,
            /* [retval][out] */ __RPC__deref_out_opt BSTR *retval);
        
        /* [id][propput][funcdescattr] */ HRESULT ( STDMETHODCALLTYPE *put_CodeAnalysisRules )( 
            __RPC__in CSharpProjectConfigurationProperties4 * This,
            /* [in][idldescattr] */ __RPC__in BSTR noname,
            /* [retval][out] */ __RPC__out void *retval);
        
        /* [id][propget][funcdescattr] */ HRESULT ( STDMETHODCALLTYPE *get_CodeAnalysisSpellCheckLanguages )( 
            __RPC__in CSharpProjectConfigurationProperties4 * This,
            /* [retval][out] */ __RPC__deref_out_opt BSTR *retval);
        
        /* [id][propput][funcdescattr] */ HRESULT ( STDMETHODCALLTYPE *put_CodeAnalysisSpellCheckLanguages )( 
            __RPC__in CSharpProjectConfigurationProperties4 * This,
            /* [in][idldescattr] */ __RPC__in BSTR noname,
            /* [retval][out] */ __RPC__out void *retval);
        
        /* [id][propget][funcdescattr] */ HRESULT ( STDMETHODCALLTYPE *get_CodeAnalysisUseTypeNameInSuppression )( 
            __RPC__in CSharpProjectConfigurationProperties4 * This,
            /* [retval][out] */ __RPC__out BOOLEAN *retval);
        
        /* [id][propput][funcdescattr] */ HRESULT ( STDMETHODCALLTYPE *put_CodeAnalysisUseTypeNameInSuppression )( 
            __RPC__in CSharpProjectConfigurationProperties4 * This,
            /* [in][idldescattr] */ BOOLEAN noname,
            /* [retval][out] */ __RPC__out void *retval);
        
        /* [id][propget][funcdescattr] */ HRESULT ( STDMETHODCALLTYPE *get_CodeAnalysisModuleSuppressionsFile )( 
            __RPC__in CSharpProjectConfigurationProperties4 * This,
            /* [retval][out] */ __RPC__deref_out_opt BSTR *retval);
        
        /* [id][propput][funcdescattr] */ HRESULT ( STDMETHODCALLTYPE *put_CodeAnalysisModuleSuppressionsFile )( 
            __RPC__in CSharpProjectConfigurationProperties4 * This,
            /* [in][idldescattr] */ __RPC__in BSTR noname,
            /* [retval][out] */ __RPC__out void *retval);
        
        /* [id][propget][funcdescattr] */ HRESULT ( STDMETHODCALLTYPE *get_UseVSHostingProcess )( 
            __RPC__in CSharpProjectConfigurationProperties4 * This,
            /* [retval][out] */ __RPC__out BOOLEAN *retval);
        
        /* [id][propput][funcdescattr] */ HRESULT ( STDMETHODCALLTYPE *put_UseVSHostingProcess )( 
            __RPC__in CSharpProjectConfigurationProperties4 * This,
            /* [in][idldescattr] */ BOOLEAN noname,
            /* [retval][out] */ __RPC__out void *retval);
        
        /* [id][propget][funcdescattr] */ HRESULT ( STDMETHODCALLTYPE *get_GenerateSerializationAssemblies )( 
            __RPC__in CSharpProjectConfigurationProperties4 * This,
            /* [retval][out] */ __RPC__out enum sgenGenerationOption *retval);
        
        /* [id][propput][funcdescattr] */ HRESULT ( STDMETHODCALLTYPE *put_GenerateSerializationAssemblies )( 
            __RPC__in CSharpProjectConfigurationProperties4 * This,
            /* [in][idldescattr] */ enum sgenGenerationOption noname,
            /* [retval][out] */ __RPC__out void *retval);
        
        /* [id][propget][funcdescattr] */ HRESULT ( STDMETHODCALLTYPE *get_LanguageVersion )( 
            __RPC__in CSharpProjectConfigurationProperties4 * This,
            /* [retval][out] */ __RPC__deref_out_opt BSTR *retval);
        
        /* [id][propput][funcdescattr] */ HRESULT ( STDMETHODCALLTYPE *put_LanguageVersion )( 
            __RPC__in CSharpProjectConfigurationProperties4 * This,
            /* [in][idldescattr] */ __RPC__in BSTR noname,
            /* [retval][out] */ __RPC__out void *retval);
        
        /* [id][propget][funcdescattr] */ HRESULT ( STDMETHODCALLTYPE *get_ErrorReport )( 
            __RPC__in CSharpProjectConfigurationProperties4 * This,
            /* [retval][out] */ __RPC__deref_out_opt BSTR *retval);
        
        /* [id][propput][funcdescattr] */ HRESULT ( STDMETHODCALLTYPE *put_ErrorReport )( 
            __RPC__in CSharpProjectConfigurationProperties4 * This,
            /* [in][idldescattr] */ __RPC__in BSTR noname,
            /* [retval][out] */ __RPC__out void *retval);
        
        /* [helpstring][id][propget] */ HRESULT ( STDMETHODCALLTYPE *get_CodeAnalysisIgnoreGeneratedCode )( 
            __RPC__in CSharpProjectConfigurationProperties4 * This,
            /* [retval][out] */ __RPC__out VARIANT_BOOL *pbIgnoreGeneratedCode);
        
        /* [helpstring][id][propput] */ HRESULT ( STDMETHODCALLTYPE *put_CodeAnalysisIgnoreGeneratedCode )( 
            __RPC__in CSharpProjectConfigurationProperties4 * This,
            /* [in] */ VARIANT_BOOL IgnoreGeneratedCode);
        
        /* [helpstring][id][propget] */ HRESULT ( STDMETHODCALLTYPE *get_CodeAnalysisOverrideRuleVisibilities )( 
            __RPC__in CSharpProjectConfigurationProperties4 * This,
            /* [retval][out] */ __RPC__out VARIANT_BOOL *pbOverrideRuleVisibilities);
        
        /* [helpstring][id][propput] */ HRESULT ( STDMETHODCALLTYPE *put_CodeAnalysisOverrideRuleVisibilities )( 
            __RPC__in CSharpProjectConfigurationProperties4 * This,
            /* [in] */ VARIANT_BOOL OverrideRuleVisibilities);
        
        /* [helpstring][id][propget] */ HRESULT ( STDMETHODCALLTYPE *get_CodeAnalysisDictionaries )( 
            __RPC__in CSharpProjectConfigurationProperties4 * This,
            /* [retval][out] */ __RPC__deref_out_opt BSTR *pbstrDictionaries);
        
        /* [helpstring][id][propput] */ HRESULT ( STDMETHODCALLTYPE *put_CodeAnalysisDictionaries )( 
            __RPC__in CSharpProjectConfigurationProperties4 * This,
            /* [in] */ __RPC__in BSTR Dictionaries);
        
        /* [helpstring][id][propget] */ HRESULT ( STDMETHODCALLTYPE *get_CodeAnalysisCulture )( 
            __RPC__in CSharpProjectConfigurationProperties4 * This,
            /* [retval][out] */ __RPC__deref_out_opt BSTR *pbstrCodeAnalysisCulture);
        
        /* [helpstring][id][propput] */ HRESULT ( STDMETHODCALLTYPE *put_CodeAnalysisCulture )( 
            __RPC__in CSharpProjectConfigurationProperties4 * This,
            /* [in] */ __RPC__in BSTR pbstrCodeAnalysisCulture);
        
        END_INTERFACE
    } CSharpProjectConfigurationProperties4Vtbl;

    interface CSharpProjectConfigurationProperties4
    {
        CONST_VTBL struct CSharpProjectConfigurationProperties4Vtbl *lpVtbl;
    };

    

#ifdef COBJMACROS


#define CSharpProjectConfigurationProperties4_QueryInterface(This,riid,ppvObj,retval)	\
    ( (This)->lpVtbl -> QueryInterface(This,riid,ppvObj,retval) ) 

#define CSharpProjectConfigurationProperties4_AddRef(This,retval)	\
    ( (This)->lpVtbl -> AddRef(This,retval) ) 

#define CSharpProjectConfigurationProperties4_Release(This,retval)	\
    ( (This)->lpVtbl -> Release(This,retval) ) 

#define CSharpProjectConfigurationProperties4_GetTypeInfoCount(This,pctinfo,retval)	\
    ( (This)->lpVtbl -> GetTypeInfoCount(This,pctinfo,retval) ) 

#define CSharpProjectConfigurationProperties4_GetTypeInfo(This,itinfo,lcid,pptinfo,retval)	\
    ( (This)->lpVtbl -> GetTypeInfo(This,itinfo,lcid,pptinfo,retval) ) 

#define CSharpProjectConfigurationProperties4_GetIDsOfNames(This,riid,rgszNames,cNames,lcid,rgdispid,retval)	\
    ( (This)->lpVtbl -> GetIDsOfNames(This,riid,rgszNames,cNames,lcid,rgdispid,retval) ) 

#define CSharpProjectConfigurationProperties4_Invoke(This,dispidMember,riid,lcid,wFlags,pdispparams,pvarResult,pexcepinfo,puArgErr,retval)	\
    ( (This)->lpVtbl -> Invoke(This,dispidMember,riid,lcid,wFlags,pdispparams,pvarResult,pexcepinfo,puArgErr,retval) ) 

#define CSharpProjectConfigurationProperties4_get___id(This,retval)	\
    ( (This)->lpVtbl -> get___id(This,retval) ) 

#define CSharpProjectConfigurationProperties4_get_DebugSymbols(This,retval)	\
    ( (This)->lpVtbl -> get_DebugSymbols(This,retval) ) 

#define CSharpProjectConfigurationProperties4_put_DebugSymbols(This,noname,retval)	\
    ( (This)->lpVtbl -> put_DebugSymbols(This,noname,retval) ) 

#define CSharpProjectConfigurationProperties4_get_DefineDebug(This,retval)	\
    ( (This)->lpVtbl -> get_DefineDebug(This,retval) ) 

#define CSharpProjectConfigurationProperties4_put_DefineDebug(This,noname,retval)	\
    ( (This)->lpVtbl -> put_DefineDebug(This,noname,retval) ) 

#define CSharpProjectConfigurationProperties4_get_DefineTrace(This,retval)	\
    ( (This)->lpVtbl -> get_DefineTrace(This,retval) ) 

#define CSharpProjectConfigurationProperties4_put_DefineTrace(This,noname,retval)	\
    ( (This)->lpVtbl -> put_DefineTrace(This,noname,retval) ) 

#define CSharpProjectConfigurationProperties4_get_OutputPath(This,retval)	\
    ( (This)->lpVtbl -> get_OutputPath(This,retval) ) 

#define CSharpProjectConfigurationProperties4_put_OutputPath(This,noname,retval)	\
    ( (This)->lpVtbl -> put_OutputPath(This,noname,retval) ) 

#define CSharpProjectConfigurationProperties4_get_IntermediatePath(This,retval)	\
    ( (This)->lpVtbl -> get_IntermediatePath(This,retval) ) 

#define CSharpProjectConfigurationProperties4_put_IntermediatePath(This,noname,retval)	\
    ( (This)->lpVtbl -> put_IntermediatePath(This,noname,retval) ) 

#define CSharpProjectConfigurationProperties4_get_DefineConstants(This,retval)	\
    ( (This)->lpVtbl -> get_DefineConstants(This,retval) ) 

#define CSharpProjectConfigurationProperties4_put_DefineConstants(This,noname,retval)	\
    ( (This)->lpVtbl -> put_DefineConstants(This,noname,retval) ) 

#define CSharpProjectConfigurationProperties4_get_RemoveIntegerChecks(This,retval)	\
    ( (This)->lpVtbl -> get_RemoveIntegerChecks(This,retval) ) 

#define CSharpProjectConfigurationProperties4_put_RemoveIntegerChecks(This,noname,retval)	\
    ( (This)->lpVtbl -> put_RemoveIntegerChecks(This,noname,retval) ) 

#define CSharpProjectConfigurationProperties4_get_BaseAddress(This,retval)	\
    ( (This)->lpVtbl -> get_BaseAddress(This,retval) ) 

#define CSharpProjectConfigurationProperties4_put_BaseAddress(This,noname,retval)	\
    ( (This)->lpVtbl -> put_BaseAddress(This,noname,retval) ) 

#define CSharpProjectConfigurationProperties4_get_AllowUnsafeBlocks(This,retval)	\
    ( (This)->lpVtbl -> get_AllowUnsafeBlocks(This,retval) ) 

#define CSharpProjectConfigurationProperties4_put_AllowUnsafeBlocks(This,noname,retval)	\
    ( (This)->lpVtbl -> put_AllowUnsafeBlocks(This,noname,retval) ) 

#define CSharpProjectConfigurationProperties4_get_CheckForOverflowUnderflow(This,retval)	\
    ( (This)->lpVtbl -> get_CheckForOverflowUnderflow(This,retval) ) 

#define CSharpProjectConfigurationProperties4_put_CheckForOverflowUnderflow(This,noname,retval)	\
    ( (This)->lpVtbl -> put_CheckForOverflowUnderflow(This,noname,retval) ) 

#define CSharpProjectConfigurationProperties4_get_DocumentationFile(This,retval)	\
    ( (This)->lpVtbl -> get_DocumentationFile(This,retval) ) 

#define CSharpProjectConfigurationProperties4_put_DocumentationFile(This,noname,retval)	\
    ( (This)->lpVtbl -> put_DocumentationFile(This,noname,retval) ) 

#define CSharpProjectConfigurationProperties4_get_Optimize(This,retval)	\
    ( (This)->lpVtbl -> get_Optimize(This,retval) ) 

#define CSharpProjectConfigurationProperties4_put_Optimize(This,noname,retval)	\
    ( (This)->lpVtbl -> put_Optimize(This,noname,retval) ) 

#define CSharpProjectConfigurationProperties4_get_IncrementalBuild(This,retval)	\
    ( (This)->lpVtbl -> get_IncrementalBuild(This,retval) ) 

#define CSharpProjectConfigurationProperties4_put_IncrementalBuild(This,noname,retval)	\
    ( (This)->lpVtbl -> put_IncrementalBuild(This,noname,retval) ) 

#define CSharpProjectConfigurationProperties4_get_StartProgram(This,retval)	\
    ( (This)->lpVtbl -> get_StartProgram(This,retval) ) 

#define CSharpProjectConfigurationProperties4_put_StartProgram(This,noname,retval)	\
    ( (This)->lpVtbl -> put_StartProgram(This,noname,retval) ) 

#define CSharpProjectConfigurationProperties4_get_StartWorkingDirectory(This,retval)	\
    ( (This)->lpVtbl -> get_StartWorkingDirectory(This,retval) ) 

#define CSharpProjectConfigurationProperties4_put_StartWorkingDirectory(This,noname,retval)	\
    ( (This)->lpVtbl -> put_StartWorkingDirectory(This,noname,retval) ) 

#define CSharpProjectConfigurationProperties4_get_StartURL(This,retval)	\
    ( (This)->lpVtbl -> get_StartURL(This,retval) ) 

#define CSharpProjectConfigurationProperties4_put_StartURL(This,noname,retval)	\
    ( (This)->lpVtbl -> put_StartURL(This,noname,retval) ) 

#define CSharpProjectConfigurationProperties4_get_StartPage(This,retval)	\
    ( (This)->lpVtbl -> get_StartPage(This,retval) ) 

#define CSharpProjectConfigurationProperties4_put_StartPage(This,noname,retval)	\
    ( (This)->lpVtbl -> put_StartPage(This,noname,retval) ) 

#define CSharpProjectConfigurationProperties4_get_StartArguments(This,retval)	\
    ( (This)->lpVtbl -> get_StartArguments(This,retval) ) 

#define CSharpProjectConfigurationProperties4_put_StartArguments(This,noname,retval)	\
    ( (This)->lpVtbl -> put_StartArguments(This,noname,retval) ) 

#define CSharpProjectConfigurationProperties4_get_StartWithIE(This,retval)	\
    ( (This)->lpVtbl -> get_StartWithIE(This,retval) ) 

#define CSharpProjectConfigurationProperties4_put_StartWithIE(This,noname,retval)	\
    ( (This)->lpVtbl -> put_StartWithIE(This,noname,retval) ) 

#define CSharpProjectConfigurationProperties4_get_EnableASPDebugging(This,retval)	\
    ( (This)->lpVtbl -> get_EnableASPDebugging(This,retval) ) 

#define CSharpProjectConfigurationProperties4_put_EnableASPDebugging(This,noname,retval)	\
    ( (This)->lpVtbl -> put_EnableASPDebugging(This,noname,retval) ) 

#define CSharpProjectConfigurationProperties4_get_EnableASPXDebugging(This,retval)	\
    ( (This)->lpVtbl -> get_EnableASPXDebugging(This,retval) ) 

#define CSharpProjectConfigurationProperties4_put_EnableASPXDebugging(This,noname,retval)	\
    ( (This)->lpVtbl -> put_EnableASPXDebugging(This,noname,retval) ) 

#define CSharpProjectConfigurationProperties4_get_EnableUnmanagedDebugging(This,retval)	\
    ( (This)->lpVtbl -> get_EnableUnmanagedDebugging(This,retval) ) 

#define CSharpProjectConfigurationProperties4_put_EnableUnmanagedDebugging(This,noname,retval)	\
    ( (This)->lpVtbl -> put_EnableUnmanagedDebugging(This,noname,retval) ) 

#define CSharpProjectConfigurationProperties4_get_StartAction(This,retval)	\
    ( (This)->lpVtbl -> get_StartAction(This,retval) ) 

#define CSharpProjectConfigurationProperties4_put_StartAction(This,noname,retval)	\
    ( (This)->lpVtbl -> put_StartAction(This,noname,retval) ) 

#define CSharpProjectConfigurationProperties4_get_Extender(This,ExtenderName,retval)	\
    ( (This)->lpVtbl -> get_Extender(This,ExtenderName,retval) ) 

#define CSharpProjectConfigurationProperties4_get_ExtenderNames(This,retval)	\
    ( (This)->lpVtbl -> get_ExtenderNames(This,retval) ) 

#define CSharpProjectConfigurationProperties4_get_ExtenderCATID(This,retval)	\
    ( (This)->lpVtbl -> get_ExtenderCATID(This,retval) ) 

#define CSharpProjectConfigurationProperties4_get_WarningLevel(This,retval)	\
    ( (This)->lpVtbl -> get_WarningLevel(This,retval) ) 

#define CSharpProjectConfigurationProperties4_put_WarningLevel(This,noname,retval)	\
    ( (This)->lpVtbl -> put_WarningLevel(This,noname,retval) ) 

#define CSharpProjectConfigurationProperties4_get_TreatWarningsAsErrors(This,retval)	\
    ( (This)->lpVtbl -> get_TreatWarningsAsErrors(This,retval) ) 

#define CSharpProjectConfigurationProperties4_put_TreatWarningsAsErrors(This,noname,retval)	\
    ( (This)->lpVtbl -> put_TreatWarningsAsErrors(This,noname,retval) ) 

#define CSharpProjectConfigurationProperties4_get_EnableSQLServerDebugging(This,retval)	\
    ( (This)->lpVtbl -> get_EnableSQLServerDebugging(This,retval) ) 

#define CSharpProjectConfigurationProperties4_put_EnableSQLServerDebugging(This,noname,retval)	\
    ( (This)->lpVtbl -> put_EnableSQLServerDebugging(This,noname,retval) ) 

#define CSharpProjectConfigurationProperties4_get_FileAlignment(This,retval)	\
    ( (This)->lpVtbl -> get_FileAlignment(This,retval) ) 

#define CSharpProjectConfigurationProperties4_put_FileAlignment(This,noname,retval)	\
    ( (This)->lpVtbl -> put_FileAlignment(This,noname,retval) ) 

#define CSharpProjectConfigurationProperties4_get_RegisterForComInterop(This,retval)	\
    ( (This)->lpVtbl -> get_RegisterForComInterop(This,retval) ) 

#define CSharpProjectConfigurationProperties4_put_RegisterForComInterop(This,noname,retval)	\
    ( (This)->lpVtbl -> put_RegisterForComInterop(This,noname,retval) ) 

#define CSharpProjectConfigurationProperties4_get_ConfigurationOverrideFile(This,retval)	\
    ( (This)->lpVtbl -> get_ConfigurationOverrideFile(This,retval) ) 

#define CSharpProjectConfigurationProperties4_put_ConfigurationOverrideFile(This,noname,retval)	\
    ( (This)->lpVtbl -> put_ConfigurationOverrideFile(This,noname,retval) ) 

#define CSharpProjectConfigurationProperties4_get_RemoteDebugEnabled(This,retval)	\
    ( (This)->lpVtbl -> get_RemoteDebugEnabled(This,retval) ) 

#define CSharpProjectConfigurationProperties4_put_RemoteDebugEnabled(This,noname,retval)	\
    ( (This)->lpVtbl -> put_RemoteDebugEnabled(This,noname,retval) ) 

#define CSharpProjectConfigurationProperties4_get_RemoteDebugMachine(This,retval)	\
    ( (This)->lpVtbl -> get_RemoteDebugMachine(This,retval) ) 

#define CSharpProjectConfigurationProperties4_put_RemoteDebugMachine(This,noname,retval)	\
    ( (This)->lpVtbl -> put_RemoteDebugMachine(This,noname,retval) ) 

#define CSharpProjectConfigurationProperties4_get_NoWarn(This,retval)	\
    ( (This)->lpVtbl -> get_NoWarn(This,retval) ) 

#define CSharpProjectConfigurationProperties4_put_NoWarn(This,noname,retval)	\
    ( (This)->lpVtbl -> put_NoWarn(This,noname,retval) ) 

#define CSharpProjectConfigurationProperties4_get_NoStdLib(This,retval)	\
    ( (This)->lpVtbl -> get_NoStdLib(This,retval) ) 

#define CSharpProjectConfigurationProperties4_put_NoStdLib(This,noname,retval)	\
    ( (This)->lpVtbl -> put_NoStdLib(This,noname,retval) ) 

#define CSharpProjectConfigurationProperties4_get_DebugInfo(This,retval)	\
    ( (This)->lpVtbl -> get_DebugInfo(This,retval) ) 

#define CSharpProjectConfigurationProperties4_put_DebugInfo(This,noname,retval)	\
    ( (This)->lpVtbl -> put_DebugInfo(This,noname,retval) ) 

#define CSharpProjectConfigurationProperties4_get_PlatformTarget(This,retval)	\
    ( (This)->lpVtbl -> get_PlatformTarget(This,retval) ) 

#define CSharpProjectConfigurationProperties4_put_PlatformTarget(This,noname,retval)	\
    ( (This)->lpVtbl -> put_PlatformTarget(This,noname,retval) ) 

#define CSharpProjectConfigurationProperties4_get_TreatSpecificWarningsAsErrors(This,retval)	\
    ( (This)->lpVtbl -> get_TreatSpecificWarningsAsErrors(This,retval) ) 

#define CSharpProjectConfigurationProperties4_put_TreatSpecificWarningsAsErrors(This,noname,retval)	\
    ( (This)->lpVtbl -> put_TreatSpecificWarningsAsErrors(This,noname,retval) ) 

#define CSharpProjectConfigurationProperties4_get_RunCodeAnalysis(This,retval)	\
    ( (This)->lpVtbl -> get_RunCodeAnalysis(This,retval) ) 

#define CSharpProjectConfigurationProperties4_put_RunCodeAnalysis(This,noname,retval)	\
    ( (This)->lpVtbl -> put_RunCodeAnalysis(This,noname,retval) ) 

#define CSharpProjectConfigurationProperties4_get_CodeAnalysisLogFile(This,retval)	\
    ( (This)->lpVtbl -> get_CodeAnalysisLogFile(This,retval) ) 

#define CSharpProjectConfigurationProperties4_put_CodeAnalysisLogFile(This,noname,retval)	\
    ( (This)->lpVtbl -> put_CodeAnalysisLogFile(This,noname,retval) ) 

#define CSharpProjectConfigurationProperties4_get_CodeAnalysisRuleAssemblies(This,retval)	\
    ( (This)->lpVtbl -> get_CodeAnalysisRuleAssemblies(This,retval) ) 

#define CSharpProjectConfigurationProperties4_put_CodeAnalysisRuleAssemblies(This,noname,retval)	\
    ( (This)->lpVtbl -> put_CodeAnalysisRuleAssemblies(This,noname,retval) ) 

#define CSharpProjectConfigurationProperties4_get_CodeAnalysisInputAssembly(This,retval)	\
    ( (This)->lpVtbl -> get_CodeAnalysisInputAssembly(This,retval) ) 

#define CSharpProjectConfigurationProperties4_put_CodeAnalysisInputAssembly(This,noname,retval)	\
    ( (This)->lpVtbl -> put_CodeAnalysisInputAssembly(This,noname,retval) ) 

#define CSharpProjectConfigurationProperties4_get_CodeAnalysisRules(This,retval)	\
    ( (This)->lpVtbl -> get_CodeAnalysisRules(This,retval) ) 

#define CSharpProjectConfigurationProperties4_put_CodeAnalysisRules(This,noname,retval)	\
    ( (This)->lpVtbl -> put_CodeAnalysisRules(This,noname,retval) ) 

#define CSharpProjectConfigurationProperties4_get_CodeAnalysisSpellCheckLanguages(This,retval)	\
    ( (This)->lpVtbl -> get_CodeAnalysisSpellCheckLanguages(This,retval) ) 

#define CSharpProjectConfigurationProperties4_put_CodeAnalysisSpellCheckLanguages(This,noname,retval)	\
    ( (This)->lpVtbl -> put_CodeAnalysisSpellCheckLanguages(This,noname,retval) ) 

#define CSharpProjectConfigurationProperties4_get_CodeAnalysisUseTypeNameInSuppression(This,retval)	\
    ( (This)->lpVtbl -> get_CodeAnalysisUseTypeNameInSuppression(This,retval) ) 

#define CSharpProjectConfigurationProperties4_put_CodeAnalysisUseTypeNameInSuppression(This,noname,retval)	\
    ( (This)->lpVtbl -> put_CodeAnalysisUseTypeNameInSuppression(This,noname,retval) ) 

#define CSharpProjectConfigurationProperties4_get_CodeAnalysisModuleSuppressionsFile(This,retval)	\
    ( (This)->lpVtbl -> get_CodeAnalysisModuleSuppressionsFile(This,retval) ) 

#define CSharpProjectConfigurationProperties4_put_CodeAnalysisModuleSuppressionsFile(This,noname,retval)	\
    ( (This)->lpVtbl -> put_CodeAnalysisModuleSuppressionsFile(This,noname,retval) ) 

#define CSharpProjectConfigurationProperties4_get_UseVSHostingProcess(This,retval)	\
    ( (This)->lpVtbl -> get_UseVSHostingProcess(This,retval) ) 

#define CSharpProjectConfigurationProperties4_put_UseVSHostingProcess(This,noname,retval)	\
    ( (This)->lpVtbl -> put_UseVSHostingProcess(This,noname,retval) ) 

#define CSharpProjectConfigurationProperties4_get_GenerateSerializationAssemblies(This,retval)	\
    ( (This)->lpVtbl -> get_GenerateSerializationAssemblies(This,retval) ) 

#define CSharpProjectConfigurationProperties4_put_GenerateSerializationAssemblies(This,noname,retval)	\
    ( (This)->lpVtbl -> put_GenerateSerializationAssemblies(This,noname,retval) ) 

#define CSharpProjectConfigurationProperties4_get_LanguageVersion(This,retval)	\
    ( (This)->lpVtbl -> get_LanguageVersion(This,retval) ) 

#define CSharpProjectConfigurationProperties4_put_LanguageVersion(This,noname,retval)	\
    ( (This)->lpVtbl -> put_LanguageVersion(This,noname,retval) ) 

#define CSharpProjectConfigurationProperties4_get_ErrorReport(This,retval)	\
    ( (This)->lpVtbl -> get_ErrorReport(This,retval) ) 

#define CSharpProjectConfigurationProperties4_put_ErrorReport(This,noname,retval)	\
    ( (This)->lpVtbl -> put_ErrorReport(This,noname,retval) ) 


#define CSharpProjectConfigurationProperties4_get_CodeAnalysisIgnoreGeneratedCode(This,pbIgnoreGeneratedCode)	\
    ( (This)->lpVtbl -> get_CodeAnalysisIgnoreGeneratedCode(This,pbIgnoreGeneratedCode) ) 

#define CSharpProjectConfigurationProperties4_put_CodeAnalysisIgnoreGeneratedCode(This,IgnoreGeneratedCode)	\
    ( (This)->lpVtbl -> put_CodeAnalysisIgnoreGeneratedCode(This,IgnoreGeneratedCode) ) 

#define CSharpProjectConfigurationProperties4_get_CodeAnalysisOverrideRuleVisibilities(This,pbOverrideRuleVisibilities)	\
    ( (This)->lpVtbl -> get_CodeAnalysisOverrideRuleVisibilities(This,pbOverrideRuleVisibilities) ) 

#define CSharpProjectConfigurationProperties4_put_CodeAnalysisOverrideRuleVisibilities(This,OverrideRuleVisibilities)	\
    ( (This)->lpVtbl -> put_CodeAnalysisOverrideRuleVisibilities(This,OverrideRuleVisibilities) ) 

#define CSharpProjectConfigurationProperties4_get_CodeAnalysisDictionaries(This,pbstrDictionaries)	\
    ( (This)->lpVtbl -> get_CodeAnalysisDictionaries(This,pbstrDictionaries) ) 

#define CSharpProjectConfigurationProperties4_put_CodeAnalysisDictionaries(This,Dictionaries)	\
    ( (This)->lpVtbl -> put_CodeAnalysisDictionaries(This,Dictionaries) ) 

#define CSharpProjectConfigurationProperties4_get_CodeAnalysisCulture(This,pbstrCodeAnalysisCulture)	\
    ( (This)->lpVtbl -> get_CodeAnalysisCulture(This,pbstrCodeAnalysisCulture) ) 

#define CSharpProjectConfigurationProperties4_put_CodeAnalysisCulture(This,pbstrCodeAnalysisCulture)	\
    ( (This)->lpVtbl -> put_CodeAnalysisCulture(This,pbstrCodeAnalysisCulture) ) 

#endif /* COBJMACROS */


#endif 	/* C style interface */




#endif 	/* __CSharpProjectConfigurationProperties4_INTERFACE_DEFINED__ */


#ifndef __VBProjectConfigurationProperties4_INTERFACE_DEFINED__
#define __VBProjectConfigurationProperties4_INTERFACE_DEFINED__

/* interface VBProjectConfigurationProperties4 */
/* [object][dual][unique][helpstring][uuid] */ 


EXTERN_C const IID IID_VBProjectConfigurationProperties4;

#if defined(__cplusplus) && !defined(CINTERFACE)
    
    MIDL_INTERFACE("189a5eb4-4b8d-4083-9400-77cf9633783a")
    VBProjectConfigurationProperties4 : public ProjectConfigurationProperties3
    {
    public:
        virtual /* [helpstring][id][propget] */ HRESULT STDMETHODCALLTYPE get_CodeAnalysisIgnoreGeneratedCode( 
            /* [retval][out] */ __RPC__out VARIANT_BOOL *pbIgnoreGeneratedCode) = 0;
        
        virtual /* [helpstring][id][propput] */ HRESULT STDMETHODCALLTYPE put_CodeAnalysisIgnoreGeneratedCode( 
            /* [in] */ VARIANT_BOOL IgnoreGeneratedCode) = 0;
        
        virtual /* [helpstring][id][propget] */ HRESULT STDMETHODCALLTYPE get_CodeAnalysisOverrideRuleVisibilities( 
            /* [retval][out] */ __RPC__out VARIANT_BOOL *pbOverrideRuleVisibilities) = 0;
        
        virtual /* [helpstring][id][propput] */ HRESULT STDMETHODCALLTYPE put_CodeAnalysisOverrideRuleVisibilities( 
            /* [in] */ VARIANT_BOOL OverrideRuleVisibilities) = 0;
        
        virtual /* [helpstring][id][propget] */ HRESULT STDMETHODCALLTYPE get_CodeAnalysisDictionaries( 
            /* [retval][out] */ __RPC__deref_out_opt BSTR *pbstrDictionaries) = 0;
        
        virtual /* [helpstring][id][propput] */ HRESULT STDMETHODCALLTYPE put_CodeAnalysisDictionaries( 
            /* [in] */ __RPC__in BSTR Dictionaries) = 0;
        
        virtual /* [helpstring][id][propget] */ HRESULT STDMETHODCALLTYPE get_CodeAnalysisCulture( 
            /* [retval][out] */ __RPC__deref_out_opt BSTR *pbstrCodeAnalysisCulture) = 0;
        
        virtual /* [helpstring][id][propput] */ HRESULT STDMETHODCALLTYPE put_CodeAnalysisCulture( 
            /* [in] */ __RPC__in BSTR pbstrCodeAnalysisCulture) = 0;
        
    };
    
    
#else 	/* C style interface */

    typedef struct VBProjectConfigurationProperties4Vtbl
    {
        BEGIN_INTERFACE
        
        /* [id][restricted][funcdescattr] */ HRESULT ( STDMETHODCALLTYPE *QueryInterface )( 
            __RPC__in VBProjectConfigurationProperties4 * This,
            /* [in][idldescattr] */ __RPC__in struct GUID *riid,
            /* [out][idldescattr] */ __RPC__deref_out_opt void **ppvObj,
            /* [retval][out] */ __RPC__out void *retval);
        
        /* [id][restricted][funcdescattr] */ HRESULT ( STDMETHODCALLTYPE *AddRef )( 
            __RPC__in VBProjectConfigurationProperties4 * This,
            /* [retval][out] */ __RPC__out unsigned long *retval);
        
        /* [id][restricted][funcdescattr] */ HRESULT ( STDMETHODCALLTYPE *Release )( 
            __RPC__in VBProjectConfigurationProperties4 * This,
            /* [retval][out] */ __RPC__out unsigned long *retval);
        
        /* [id][restricted][funcdescattr] */ HRESULT ( STDMETHODCALLTYPE *GetTypeInfoCount )( 
            __RPC__in VBProjectConfigurationProperties4 * This,
            /* [out][idldescattr] */ __RPC__out unsigned UINT *pctinfo,
            /* [retval][out] */ __RPC__out void *retval);
        
        /* [id][restricted][funcdescattr] */ HRESULT ( STDMETHODCALLTYPE *GetTypeInfo )( 
            __RPC__in VBProjectConfigurationProperties4 * This,
            /* [in][idldescattr] */ unsigned UINT itinfo,
            /* [in][idldescattr] */ unsigned long lcid,
            /* [out][idldescattr] */ __RPC__deref_out_opt void **pptinfo,
            /* [retval][out] */ __RPC__out void *retval);
        
        /* [id][restricted][funcdescattr] */ HRESULT ( STDMETHODCALLTYPE *GetIDsOfNames )( 
            __RPC__in VBProjectConfigurationProperties4 * This,
            /* [in][idldescattr] */ __RPC__in struct GUID *riid,
            /* [in][idldescattr] */ __RPC__deref_in_opt signed char **rgszNames,
            /* [in][idldescattr] */ unsigned UINT cNames,
            /* [in][idldescattr] */ unsigned long lcid,
            /* [out][idldescattr] */ __RPC__out signed long *rgdispid,
            /* [retval][out] */ __RPC__out void *retval);
        
        /* [id][restricted][funcdescattr] */ HRESULT ( STDMETHODCALLTYPE *Invoke )( 
            __RPC__in VBProjectConfigurationProperties4 * This,
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
            __RPC__in VBProjectConfigurationProperties4 * This,
            /* [retval][out] */ __RPC__deref_out_opt BSTR *retval);
        
        /* [id][propget][funcdescattr] */ HRESULT ( STDMETHODCALLTYPE *get_DebugSymbols )( 
            __RPC__in VBProjectConfigurationProperties4 * This,
            /* [retval][out] */ __RPC__out BOOLEAN *retval);
        
        /* [id][propput][funcdescattr] */ HRESULT ( STDMETHODCALLTYPE *put_DebugSymbols )( 
            __RPC__in VBProjectConfigurationProperties4 * This,
            /* [in][idldescattr] */ BOOLEAN noname,
            /* [retval][out] */ __RPC__out void *retval);
        
        /* [id][propget][funcdescattr] */ HRESULT ( STDMETHODCALLTYPE *get_DefineDebug )( 
            __RPC__in VBProjectConfigurationProperties4 * This,
            /* [retval][out] */ __RPC__out BOOLEAN *retval);
        
        /* [id][propput][funcdescattr] */ HRESULT ( STDMETHODCALLTYPE *put_DefineDebug )( 
            __RPC__in VBProjectConfigurationProperties4 * This,
            /* [in][idldescattr] */ BOOLEAN noname,
            /* [retval][out] */ __RPC__out void *retval);
        
        /* [id][propget][funcdescattr] */ HRESULT ( STDMETHODCALLTYPE *get_DefineTrace )( 
            __RPC__in VBProjectConfigurationProperties4 * This,
            /* [retval][out] */ __RPC__out BOOLEAN *retval);
        
        /* [id][propput][funcdescattr] */ HRESULT ( STDMETHODCALLTYPE *put_DefineTrace )( 
            __RPC__in VBProjectConfigurationProperties4 * This,
            /* [in][idldescattr] */ BOOLEAN noname,
            /* [retval][out] */ __RPC__out void *retval);
        
        /* [id][propget][funcdescattr] */ HRESULT ( STDMETHODCALLTYPE *get_OutputPath )( 
            __RPC__in VBProjectConfigurationProperties4 * This,
            /* [retval][out] */ __RPC__deref_out_opt BSTR *retval);
        
        /* [id][propput][funcdescattr] */ HRESULT ( STDMETHODCALLTYPE *put_OutputPath )( 
            __RPC__in VBProjectConfigurationProperties4 * This,
            /* [in][idldescattr] */ __RPC__in BSTR noname,
            /* [retval][out] */ __RPC__out void *retval);
        
        /* [id][propget][funcdescattr] */ HRESULT ( STDMETHODCALLTYPE *get_IntermediatePath )( 
            __RPC__in VBProjectConfigurationProperties4 * This,
            /* [retval][out] */ __RPC__deref_out_opt BSTR *retval);
        
        /* [id][propput][hidden][funcdescattr] */ HRESULT ( STDMETHODCALLTYPE *put_IntermediatePath )( 
            __RPC__in VBProjectConfigurationProperties4 * This,
            /* [in][idldescattr] */ __RPC__in BSTR noname,
            /* [retval][out] */ __RPC__out void *retval);
        
        /* [id][propget][funcdescattr] */ HRESULT ( STDMETHODCALLTYPE *get_DefineConstants )( 
            __RPC__in VBProjectConfigurationProperties4 * This,
            /* [retval][out] */ __RPC__deref_out_opt BSTR *retval);
        
        /* [id][propput][funcdescattr] */ HRESULT ( STDMETHODCALLTYPE *put_DefineConstants )( 
            __RPC__in VBProjectConfigurationProperties4 * This,
            /* [in][idldescattr] */ __RPC__in BSTR noname,
            /* [retval][out] */ __RPC__out void *retval);
        
        /* [id][propget][funcdescattr] */ HRESULT ( STDMETHODCALLTYPE *get_RemoveIntegerChecks )( 
            __RPC__in VBProjectConfigurationProperties4 * This,
            /* [retval][out] */ __RPC__out BOOLEAN *retval);
        
        /* [id][propput][funcdescattr] */ HRESULT ( STDMETHODCALLTYPE *put_RemoveIntegerChecks )( 
            __RPC__in VBProjectConfigurationProperties4 * This,
            /* [in][idldescattr] */ BOOLEAN noname,
            /* [retval][out] */ __RPC__out void *retval);
        
        /* [id][propget][funcdescattr] */ HRESULT ( STDMETHODCALLTYPE *get_BaseAddress )( 
            __RPC__in VBProjectConfigurationProperties4 * This,
            /* [retval][out] */ __RPC__out unsigned long *retval);
        
        /* [id][propput][funcdescattr] */ HRESULT ( STDMETHODCALLTYPE *put_BaseAddress )( 
            __RPC__in VBProjectConfigurationProperties4 * This,
            /* [in][idldescattr] */ unsigned long noname,
            /* [retval][out] */ __RPC__out void *retval);
        
        /* [id][propget][funcdescattr] */ HRESULT ( STDMETHODCALLTYPE *get_AllowUnsafeBlocks )( 
            __RPC__in VBProjectConfigurationProperties4 * This,
            /* [retval][out] */ __RPC__out BOOLEAN *retval);
        
        /* [id][propput][funcdescattr] */ HRESULT ( STDMETHODCALLTYPE *put_AllowUnsafeBlocks )( 
            __RPC__in VBProjectConfigurationProperties4 * This,
            /* [in][idldescattr] */ BOOLEAN noname,
            /* [retval][out] */ __RPC__out void *retval);
        
        /* [id][propget][funcdescattr] */ HRESULT ( STDMETHODCALLTYPE *get_CheckForOverflowUnderflow )( 
            __RPC__in VBProjectConfigurationProperties4 * This,
            /* [retval][out] */ __RPC__out BOOLEAN *retval);
        
        /* [id][propput][funcdescattr] */ HRESULT ( STDMETHODCALLTYPE *put_CheckForOverflowUnderflow )( 
            __RPC__in VBProjectConfigurationProperties4 * This,
            /* [in][idldescattr] */ BOOLEAN noname,
            /* [retval][out] */ __RPC__out void *retval);
        
        /* [id][propget][funcdescattr] */ HRESULT ( STDMETHODCALLTYPE *get_DocumentationFile )( 
            __RPC__in VBProjectConfigurationProperties4 * This,
            /* [retval][out] */ __RPC__deref_out_opt BSTR *retval);
        
        /* [id][propput][funcdescattr] */ HRESULT ( STDMETHODCALLTYPE *put_DocumentationFile )( 
            __RPC__in VBProjectConfigurationProperties4 * This,
            /* [in][idldescattr] */ __RPC__in BSTR noname,
            /* [retval][out] */ __RPC__out void *retval);
        
        /* [id][propget][funcdescattr] */ HRESULT ( STDMETHODCALLTYPE *get_Optimize )( 
            __RPC__in VBProjectConfigurationProperties4 * This,
            /* [retval][out] */ __RPC__out BOOLEAN *retval);
        
        /* [id][propput][funcdescattr] */ HRESULT ( STDMETHODCALLTYPE *put_Optimize )( 
            __RPC__in VBProjectConfigurationProperties4 * This,
            /* [in][idldescattr] */ BOOLEAN noname,
            /* [retval][out] */ __RPC__out void *retval);
        
        /* [id][propget][funcdescattr] */ HRESULT ( STDMETHODCALLTYPE *get_IncrementalBuild )( 
            __RPC__in VBProjectConfigurationProperties4 * This,
            /* [retval][out] */ __RPC__out BOOLEAN *retval);
        
        /* [id][propput][funcdescattr] */ HRESULT ( STDMETHODCALLTYPE *put_IncrementalBuild )( 
            __RPC__in VBProjectConfigurationProperties4 * This,
            /* [in][idldescattr] */ BOOLEAN noname,
            /* [retval][out] */ __RPC__out void *retval);
        
        /* [id][propget][funcdescattr] */ HRESULT ( STDMETHODCALLTYPE *get_StartProgram )( 
            __RPC__in VBProjectConfigurationProperties4 * This,
            /* [retval][out] */ __RPC__deref_out_opt BSTR *retval);
        
        /* [id][propput][funcdescattr] */ HRESULT ( STDMETHODCALLTYPE *put_StartProgram )( 
            __RPC__in VBProjectConfigurationProperties4 * This,
            /* [in][idldescattr] */ __RPC__in BSTR noname,
            /* [retval][out] */ __RPC__out void *retval);
        
        /* [id][propget][funcdescattr] */ HRESULT ( STDMETHODCALLTYPE *get_StartWorkingDirectory )( 
            __RPC__in VBProjectConfigurationProperties4 * This,
            /* [retval][out] */ __RPC__deref_out_opt BSTR *retval);
        
        /* [id][propput][funcdescattr] */ HRESULT ( STDMETHODCALLTYPE *put_StartWorkingDirectory )( 
            __RPC__in VBProjectConfigurationProperties4 * This,
            /* [in][idldescattr] */ __RPC__in BSTR noname,
            /* [retval][out] */ __RPC__out void *retval);
        
        /* [id][propget][funcdescattr] */ HRESULT ( STDMETHODCALLTYPE *get_StartURL )( 
            __RPC__in VBProjectConfigurationProperties4 * This,
            /* [retval][out] */ __RPC__deref_out_opt BSTR *retval);
        
        /* [id][propput][funcdescattr] */ HRESULT ( STDMETHODCALLTYPE *put_StartURL )( 
            __RPC__in VBProjectConfigurationProperties4 * This,
            /* [in][idldescattr] */ __RPC__in BSTR noname,
            /* [retval][out] */ __RPC__out void *retval);
        
        /* [id][propget][funcdescattr] */ HRESULT ( STDMETHODCALLTYPE *get_StartPage )( 
            __RPC__in VBProjectConfigurationProperties4 * This,
            /* [retval][out] */ __RPC__deref_out_opt BSTR *retval);
        
        /* [id][propput][funcdescattr] */ HRESULT ( STDMETHODCALLTYPE *put_StartPage )( 
            __RPC__in VBProjectConfigurationProperties4 * This,
            /* [in][idldescattr] */ __RPC__in BSTR noname,
            /* [retval][out] */ __RPC__out void *retval);
        
        /* [id][propget][funcdescattr] */ HRESULT ( STDMETHODCALLTYPE *get_StartArguments )( 
            __RPC__in VBProjectConfigurationProperties4 * This,
            /* [retval][out] */ __RPC__deref_out_opt BSTR *retval);
        
        /* [id][propput][funcdescattr] */ HRESULT ( STDMETHODCALLTYPE *put_StartArguments )( 
            __RPC__in VBProjectConfigurationProperties4 * This,
            /* [in][idldescattr] */ __RPC__in BSTR noname,
            /* [retval][out] */ __RPC__out void *retval);
        
        /* [id][propget][funcdescattr] */ HRESULT ( STDMETHODCALLTYPE *get_StartWithIE )( 
            __RPC__in VBProjectConfigurationProperties4 * This,
            /* [retval][out] */ __RPC__out BOOLEAN *retval);
        
        /* [id][propput][funcdescattr] */ HRESULT ( STDMETHODCALLTYPE *put_StartWithIE )( 
            __RPC__in VBProjectConfigurationProperties4 * This,
            /* [in][idldescattr] */ BOOLEAN noname,
            /* [retval][out] */ __RPC__out void *retval);
        
        /* [id][propget][funcdescattr] */ HRESULT ( STDMETHODCALLTYPE *get_EnableASPDebugging )( 
            __RPC__in VBProjectConfigurationProperties4 * This,
            /* [retval][out] */ __RPC__out BOOLEAN *retval);
        
        /* [id][propput][funcdescattr] */ HRESULT ( STDMETHODCALLTYPE *put_EnableASPDebugging )( 
            __RPC__in VBProjectConfigurationProperties4 * This,
            /* [in][idldescattr] */ BOOLEAN noname,
            /* [retval][out] */ __RPC__out void *retval);
        
        /* [id][propget][funcdescattr] */ HRESULT ( STDMETHODCALLTYPE *get_EnableASPXDebugging )( 
            __RPC__in VBProjectConfigurationProperties4 * This,
            /* [retval][out] */ __RPC__out BOOLEAN *retval);
        
        /* [id][propput][funcdescattr] */ HRESULT ( STDMETHODCALLTYPE *put_EnableASPXDebugging )( 
            __RPC__in VBProjectConfigurationProperties4 * This,
            /* [in][idldescattr] */ BOOLEAN noname,
            /* [retval][out] */ __RPC__out void *retval);
        
        /* [id][propget][funcdescattr] */ HRESULT ( STDMETHODCALLTYPE *get_EnableUnmanagedDebugging )( 
            __RPC__in VBProjectConfigurationProperties4 * This,
            /* [retval][out] */ __RPC__out BOOLEAN *retval);
        
        /* [id][propput][funcdescattr] */ HRESULT ( STDMETHODCALLTYPE *put_EnableUnmanagedDebugging )( 
            __RPC__in VBProjectConfigurationProperties4 * This,
            /* [in][idldescattr] */ BOOLEAN noname,
            /* [retval][out] */ __RPC__out void *retval);
        
        /* [id][propget][funcdescattr] */ HRESULT ( STDMETHODCALLTYPE *get_StartAction )( 
            __RPC__in VBProjectConfigurationProperties4 * This,
            /* [retval][out] */ __RPC__out enum prjStartAction *retval);
        
        /* [id][propput][funcdescattr] */ HRESULT ( STDMETHODCALLTYPE *put_StartAction )( 
            __RPC__in VBProjectConfigurationProperties4 * This,
            /* [in][idldescattr] */ enum prjStartAction noname,
            /* [retval][out] */ __RPC__out void *retval);
        
        /* [id][propget][funcdescattr] */ HRESULT ( STDMETHODCALLTYPE *get_Extender )( 
            __RPC__in VBProjectConfigurationProperties4 * This,
            /* [in][idldescattr] */ __RPC__in BSTR ExtenderName,
            /* [retval][out] */ __RPC__deref_out_opt IDispatch **retval);
        
        /* [id][propget][funcdescattr] */ HRESULT ( STDMETHODCALLTYPE *get_ExtenderNames )( 
            __RPC__in VBProjectConfigurationProperties4 * This,
            /* [retval][out] */ __RPC__out VARIANT *retval);
        
        /* [id][propget][funcdescattr] */ HRESULT ( STDMETHODCALLTYPE *get_ExtenderCATID )( 
            __RPC__in VBProjectConfigurationProperties4 * This,
            /* [retval][out] */ __RPC__deref_out_opt BSTR *retval);
        
        /* [id][propget][funcdescattr] */ HRESULT ( STDMETHODCALLTYPE *get_WarningLevel )( 
            __RPC__in VBProjectConfigurationProperties4 * This,
            /* [retval][out] */ __RPC__out enum prjWarningLevel *retval);
        
        /* [id][propput][funcdescattr] */ HRESULT ( STDMETHODCALLTYPE *put_WarningLevel )( 
            __RPC__in VBProjectConfigurationProperties4 * This,
            /* [in][idldescattr] */ enum prjWarningLevel noname,
            /* [retval][out] */ __RPC__out void *retval);
        
        /* [id][propget][funcdescattr] */ HRESULT ( STDMETHODCALLTYPE *get_TreatWarningsAsErrors )( 
            __RPC__in VBProjectConfigurationProperties4 * This,
            /* [retval][out] */ __RPC__out BOOLEAN *retval);
        
        /* [id][propput][funcdescattr] */ HRESULT ( STDMETHODCALLTYPE *put_TreatWarningsAsErrors )( 
            __RPC__in VBProjectConfigurationProperties4 * This,
            /* [in][idldescattr] */ BOOLEAN noname,
            /* [retval][out] */ __RPC__out void *retval);
        
        /* [id][propget][funcdescattr] */ HRESULT ( STDMETHODCALLTYPE *get_EnableSQLServerDebugging )( 
            __RPC__in VBProjectConfigurationProperties4 * This,
            /* [retval][out] */ __RPC__out BOOLEAN *retval);
        
        /* [id][propput][funcdescattr] */ HRESULT ( STDMETHODCALLTYPE *put_EnableSQLServerDebugging )( 
            __RPC__in VBProjectConfigurationProperties4 * This,
            /* [in][idldescattr] */ BOOLEAN noname,
            /* [retval][out] */ __RPC__out void *retval);
        
        /* [id][propget][funcdescattr] */ HRESULT ( STDMETHODCALLTYPE *get_FileAlignment )( 
            __RPC__in VBProjectConfigurationProperties4 * This,
            /* [retval][out] */ __RPC__out unsigned long *retval);
        
        /* [id][propput][funcdescattr] */ HRESULT ( STDMETHODCALLTYPE *put_FileAlignment )( 
            __RPC__in VBProjectConfigurationProperties4 * This,
            /* [in][idldescattr] */ unsigned long noname,
            /* [retval][out] */ __RPC__out void *retval);
        
        /* [id][propget][funcdescattr] */ HRESULT ( STDMETHODCALLTYPE *get_RegisterForComInterop )( 
            __RPC__in VBProjectConfigurationProperties4 * This,
            /* [retval][out] */ __RPC__out BOOLEAN *retval);
        
        /* [id][propput][funcdescattr] */ HRESULT ( STDMETHODCALLTYPE *put_RegisterForComInterop )( 
            __RPC__in VBProjectConfigurationProperties4 * This,
            /* [in][idldescattr] */ BOOLEAN noname,
            /* [retval][out] */ __RPC__out void *retval);
        
        /* [id][propget][funcdescattr] */ HRESULT ( STDMETHODCALLTYPE *get_ConfigurationOverrideFile )( 
            __RPC__in VBProjectConfigurationProperties4 * This,
            /* [retval][out] */ __RPC__deref_out_opt BSTR *retval);
        
        /* [id][propput][funcdescattr] */ HRESULT ( STDMETHODCALLTYPE *put_ConfigurationOverrideFile )( 
            __RPC__in VBProjectConfigurationProperties4 * This,
            /* [in][idldescattr] */ __RPC__in BSTR noname,
            /* [retval][out] */ __RPC__out void *retval);
        
        /* [id][propget][funcdescattr] */ HRESULT ( STDMETHODCALLTYPE *get_RemoteDebugEnabled )( 
            __RPC__in VBProjectConfigurationProperties4 * This,
            /* [retval][out] */ __RPC__out BOOLEAN *retval);
        
        /* [id][propput][funcdescattr] */ HRESULT ( STDMETHODCALLTYPE *put_RemoteDebugEnabled )( 
            __RPC__in VBProjectConfigurationProperties4 * This,
            /* [in][idldescattr] */ BOOLEAN noname,
            /* [retval][out] */ __RPC__out void *retval);
        
        /* [id][propget][funcdescattr] */ HRESULT ( STDMETHODCALLTYPE *get_RemoteDebugMachine )( 
            __RPC__in VBProjectConfigurationProperties4 * This,
            /* [retval][out] */ __RPC__deref_out_opt BSTR *retval);
        
        /* [id][propput][funcdescattr] */ HRESULT ( STDMETHODCALLTYPE *put_RemoteDebugMachine )( 
            __RPC__in VBProjectConfigurationProperties4 * This,
            /* [in][idldescattr] */ __RPC__in BSTR noname,
            /* [retval][out] */ __RPC__out void *retval);
        
        /* [id][propget][funcdescattr] */ HRESULT ( STDMETHODCALLTYPE *get_NoWarn )( 
            __RPC__in VBProjectConfigurationProperties4 * This,
            /* [retval][out] */ __RPC__deref_out_opt BSTR *retval);
        
        /* [id][propput][funcdescattr] */ HRESULT ( STDMETHODCALLTYPE *put_NoWarn )( 
            __RPC__in VBProjectConfigurationProperties4 * This,
            /* [in][idldescattr] */ __RPC__in BSTR noname,
            /* [retval][out] */ __RPC__out void *retval);
        
        /* [id][propget][funcdescattr] */ HRESULT ( STDMETHODCALLTYPE *get_NoStdLib )( 
            __RPC__in VBProjectConfigurationProperties4 * This,
            /* [retval][out] */ __RPC__out BOOLEAN *retval);
        
        /* [id][propput][funcdescattr] */ HRESULT ( STDMETHODCALLTYPE *put_NoStdLib )( 
            __RPC__in VBProjectConfigurationProperties4 * This,
            /* [in][idldescattr] */ BOOLEAN noname,
            /* [retval][out] */ __RPC__out void *retval);
        
        /* [id][propget][funcdescattr] */ HRESULT ( STDMETHODCALLTYPE *get_DebugInfo )( 
            __RPC__in VBProjectConfigurationProperties4 * This,
            /* [retval][out] */ __RPC__deref_out_opt BSTR *retval);
        
        /* [id][propput][funcdescattr] */ HRESULT ( STDMETHODCALLTYPE *put_DebugInfo )( 
            __RPC__in VBProjectConfigurationProperties4 * This,
            /* [in][idldescattr] */ __RPC__in BSTR noname,
            /* [retval][out] */ __RPC__out void *retval);
        
        /* [id][propget][funcdescattr] */ HRESULT ( STDMETHODCALLTYPE *get_PlatformTarget )( 
            __RPC__in VBProjectConfigurationProperties4 * This,
            /* [retval][out] */ __RPC__deref_out_opt BSTR *retval);
        
        /* [id][propput][funcdescattr] */ HRESULT ( STDMETHODCALLTYPE *put_PlatformTarget )( 
            __RPC__in VBProjectConfigurationProperties4 * This,
            /* [in][idldescattr] */ __RPC__in BSTR noname,
            /* [retval][out] */ __RPC__out void *retval);
        
        /* [id][propget][funcdescattr] */ HRESULT ( STDMETHODCALLTYPE *get_TreatSpecificWarningsAsErrors )( 
            __RPC__in VBProjectConfigurationProperties4 * This,
            /* [retval][out] */ __RPC__deref_out_opt BSTR *retval);
        
        /* [id][propput][funcdescattr] */ HRESULT ( STDMETHODCALLTYPE *put_TreatSpecificWarningsAsErrors )( 
            __RPC__in VBProjectConfigurationProperties4 * This,
            /* [in][idldescattr] */ __RPC__in BSTR noname,
            /* [retval][out] */ __RPC__out void *retval);
        
        /* [id][propget][funcdescattr] */ HRESULT ( STDMETHODCALLTYPE *get_RunCodeAnalysis )( 
            __RPC__in VBProjectConfigurationProperties4 * This,
            /* [retval][out] */ __RPC__out BOOLEAN *retval);
        
        /* [id][propput][funcdescattr] */ HRESULT ( STDMETHODCALLTYPE *put_RunCodeAnalysis )( 
            __RPC__in VBProjectConfigurationProperties4 * This,
            /* [in][idldescattr] */ BOOLEAN noname,
            /* [retval][out] */ __RPC__out void *retval);
        
        /* [id][propget][funcdescattr] */ HRESULT ( STDMETHODCALLTYPE *get_CodeAnalysisLogFile )( 
            __RPC__in VBProjectConfigurationProperties4 * This,
            /* [retval][out] */ __RPC__deref_out_opt BSTR *retval);
        
        /* [id][propput][funcdescattr] */ HRESULT ( STDMETHODCALLTYPE *put_CodeAnalysisLogFile )( 
            __RPC__in VBProjectConfigurationProperties4 * This,
            /* [in][idldescattr] */ __RPC__in BSTR noname,
            /* [retval][out] */ __RPC__out void *retval);
        
        /* [id][propget][funcdescattr] */ HRESULT ( STDMETHODCALLTYPE *get_CodeAnalysisRuleAssemblies )( 
            __RPC__in VBProjectConfigurationProperties4 * This,
            /* [retval][out] */ __RPC__deref_out_opt BSTR *retval);
        
        /* [id][propput][funcdescattr] */ HRESULT ( STDMETHODCALLTYPE *put_CodeAnalysisRuleAssemblies )( 
            __RPC__in VBProjectConfigurationProperties4 * This,
            /* [in][idldescattr] */ __RPC__in BSTR noname,
            /* [retval][out] */ __RPC__out void *retval);
        
        /* [id][propget][funcdescattr] */ HRESULT ( STDMETHODCALLTYPE *get_CodeAnalysisInputAssembly )( 
            __RPC__in VBProjectConfigurationProperties4 * This,
            /* [retval][out] */ __RPC__deref_out_opt BSTR *retval);
        
        /* [id][propput][funcdescattr] */ HRESULT ( STDMETHODCALLTYPE *put_CodeAnalysisInputAssembly )( 
            __RPC__in VBProjectConfigurationProperties4 * This,
            /* [in][idldescattr] */ __RPC__in BSTR noname,
            /* [retval][out] */ __RPC__out void *retval);
        
        /* [id][propget][funcdescattr] */ HRESULT ( STDMETHODCALLTYPE *get_CodeAnalysisRules )( 
            __RPC__in VBProjectConfigurationProperties4 * This,
            /* [retval][out] */ __RPC__deref_out_opt BSTR *retval);
        
        /* [id][propput][funcdescattr] */ HRESULT ( STDMETHODCALLTYPE *put_CodeAnalysisRules )( 
            __RPC__in VBProjectConfigurationProperties4 * This,
            /* [in][idldescattr] */ __RPC__in BSTR noname,
            /* [retval][out] */ __RPC__out void *retval);
        
        /* [id][propget][funcdescattr] */ HRESULT ( STDMETHODCALLTYPE *get_CodeAnalysisSpellCheckLanguages )( 
            __RPC__in VBProjectConfigurationProperties4 * This,
            /* [retval][out] */ __RPC__deref_out_opt BSTR *retval);
        
        /* [id][propput][funcdescattr] */ HRESULT ( STDMETHODCALLTYPE *put_CodeAnalysisSpellCheckLanguages )( 
            __RPC__in VBProjectConfigurationProperties4 * This,
            /* [in][idldescattr] */ __RPC__in BSTR noname,
            /* [retval][out] */ __RPC__out void *retval);
        
        /* [id][propget][funcdescattr] */ HRESULT ( STDMETHODCALLTYPE *get_CodeAnalysisUseTypeNameInSuppression )( 
            __RPC__in VBProjectConfigurationProperties4 * This,
            /* [retval][out] */ __RPC__out BOOLEAN *retval);
        
        /* [id][propput][funcdescattr] */ HRESULT ( STDMETHODCALLTYPE *put_CodeAnalysisUseTypeNameInSuppression )( 
            __RPC__in VBProjectConfigurationProperties4 * This,
            /* [in][idldescattr] */ BOOLEAN noname,
            /* [retval][out] */ __RPC__out void *retval);
        
        /* [id][propget][funcdescattr] */ HRESULT ( STDMETHODCALLTYPE *get_CodeAnalysisModuleSuppressionsFile )( 
            __RPC__in VBProjectConfigurationProperties4 * This,
            /* [retval][out] */ __RPC__deref_out_opt BSTR *retval);
        
        /* [id][propput][funcdescattr] */ HRESULT ( STDMETHODCALLTYPE *put_CodeAnalysisModuleSuppressionsFile )( 
            __RPC__in VBProjectConfigurationProperties4 * This,
            /* [in][idldescattr] */ __RPC__in BSTR noname,
            /* [retval][out] */ __RPC__out void *retval);
        
        /* [id][propget][funcdescattr] */ HRESULT ( STDMETHODCALLTYPE *get_UseVSHostingProcess )( 
            __RPC__in VBProjectConfigurationProperties4 * This,
            /* [retval][out] */ __RPC__out BOOLEAN *retval);
        
        /* [id][propput][funcdescattr] */ HRESULT ( STDMETHODCALLTYPE *put_UseVSHostingProcess )( 
            __RPC__in VBProjectConfigurationProperties4 * This,
            /* [in][idldescattr] */ BOOLEAN noname,
            /* [retval][out] */ __RPC__out void *retval);
        
        /* [id][propget][funcdescattr] */ HRESULT ( STDMETHODCALLTYPE *get_GenerateSerializationAssemblies )( 
            __RPC__in VBProjectConfigurationProperties4 * This,
            /* [retval][out] */ __RPC__out enum sgenGenerationOption *retval);
        
        /* [id][propput][funcdescattr] */ HRESULT ( STDMETHODCALLTYPE *put_GenerateSerializationAssemblies )( 
            __RPC__in VBProjectConfigurationProperties4 * This,
            /* [in][idldescattr] */ enum sgenGenerationOption noname,
            /* [retval][out] */ __RPC__out void *retval);
        
        /* [helpstring][id][propget] */ HRESULT ( STDMETHODCALLTYPE *get_CodeAnalysisIgnoreGeneratedCode )( 
            __RPC__in VBProjectConfigurationProperties4 * This,
            /* [retval][out] */ __RPC__out VARIANT_BOOL *pbIgnoreGeneratedCode);
        
        /* [helpstring][id][propput] */ HRESULT ( STDMETHODCALLTYPE *put_CodeAnalysisIgnoreGeneratedCode )( 
            __RPC__in VBProjectConfigurationProperties4 * This,
            /* [in] */ VARIANT_BOOL IgnoreGeneratedCode);
        
        /* [helpstring][id][propget] */ HRESULT ( STDMETHODCALLTYPE *get_CodeAnalysisOverrideRuleVisibilities )( 
            __RPC__in VBProjectConfigurationProperties4 * This,
            /* [retval][out] */ __RPC__out VARIANT_BOOL *pbOverrideRuleVisibilities);
        
        /* [helpstring][id][propput] */ HRESULT ( STDMETHODCALLTYPE *put_CodeAnalysisOverrideRuleVisibilities )( 
            __RPC__in VBProjectConfigurationProperties4 * This,
            /* [in] */ VARIANT_BOOL OverrideRuleVisibilities);
        
        /* [helpstring][id][propget] */ HRESULT ( STDMETHODCALLTYPE *get_CodeAnalysisDictionaries )( 
            __RPC__in VBProjectConfigurationProperties4 * This,
            /* [retval][out] */ __RPC__deref_out_opt BSTR *pbstrDictionaries);
        
        /* [helpstring][id][propput] */ HRESULT ( STDMETHODCALLTYPE *put_CodeAnalysisDictionaries )( 
            __RPC__in VBProjectConfigurationProperties4 * This,
            /* [in] */ __RPC__in BSTR Dictionaries);
        
        /* [helpstring][id][propget] */ HRESULT ( STDMETHODCALLTYPE *get_CodeAnalysisCulture )( 
            __RPC__in VBProjectConfigurationProperties4 * This,
            /* [retval][out] */ __RPC__deref_out_opt BSTR *pbstrCodeAnalysisCulture);
        
        /* [helpstring][id][propput] */ HRESULT ( STDMETHODCALLTYPE *put_CodeAnalysisCulture )( 
            __RPC__in VBProjectConfigurationProperties4 * This,
            /* [in] */ __RPC__in BSTR pbstrCodeAnalysisCulture);
        
        END_INTERFACE
    } VBProjectConfigurationProperties4Vtbl;

    interface VBProjectConfigurationProperties4
    {
        CONST_VTBL struct VBProjectConfigurationProperties4Vtbl *lpVtbl;
    };

    

#ifdef COBJMACROS


#define VBProjectConfigurationProperties4_QueryInterface(This,riid,ppvObj,retval)	\
    ( (This)->lpVtbl -> QueryInterface(This,riid,ppvObj,retval) ) 

#define VBProjectConfigurationProperties4_AddRef(This,retval)	\
    ( (This)->lpVtbl -> AddRef(This,retval) ) 

#define VBProjectConfigurationProperties4_Release(This,retval)	\
    ( (This)->lpVtbl -> Release(This,retval) ) 

#define VBProjectConfigurationProperties4_GetTypeInfoCount(This,pctinfo,retval)	\
    ( (This)->lpVtbl -> GetTypeInfoCount(This,pctinfo,retval) ) 

#define VBProjectConfigurationProperties4_GetTypeInfo(This,itinfo,lcid,pptinfo,retval)	\
    ( (This)->lpVtbl -> GetTypeInfo(This,itinfo,lcid,pptinfo,retval) ) 

#define VBProjectConfigurationProperties4_GetIDsOfNames(This,riid,rgszNames,cNames,lcid,rgdispid,retval)	\
    ( (This)->lpVtbl -> GetIDsOfNames(This,riid,rgszNames,cNames,lcid,rgdispid,retval) ) 

#define VBProjectConfigurationProperties4_Invoke(This,dispidMember,riid,lcid,wFlags,pdispparams,pvarResult,pexcepinfo,puArgErr,retval)	\
    ( (This)->lpVtbl -> Invoke(This,dispidMember,riid,lcid,wFlags,pdispparams,pvarResult,pexcepinfo,puArgErr,retval) ) 

#define VBProjectConfigurationProperties4_get___id(This,retval)	\
    ( (This)->lpVtbl -> get___id(This,retval) ) 

#define VBProjectConfigurationProperties4_get_DebugSymbols(This,retval)	\
    ( (This)->lpVtbl -> get_DebugSymbols(This,retval) ) 

#define VBProjectConfigurationProperties4_put_DebugSymbols(This,noname,retval)	\
    ( (This)->lpVtbl -> put_DebugSymbols(This,noname,retval) ) 

#define VBProjectConfigurationProperties4_get_DefineDebug(This,retval)	\
    ( (This)->lpVtbl -> get_DefineDebug(This,retval) ) 

#define VBProjectConfigurationProperties4_put_DefineDebug(This,noname,retval)	\
    ( (This)->lpVtbl -> put_DefineDebug(This,noname,retval) ) 

#define VBProjectConfigurationProperties4_get_DefineTrace(This,retval)	\
    ( (This)->lpVtbl -> get_DefineTrace(This,retval) ) 

#define VBProjectConfigurationProperties4_put_DefineTrace(This,noname,retval)	\
    ( (This)->lpVtbl -> put_DefineTrace(This,noname,retval) ) 

#define VBProjectConfigurationProperties4_get_OutputPath(This,retval)	\
    ( (This)->lpVtbl -> get_OutputPath(This,retval) ) 

#define VBProjectConfigurationProperties4_put_OutputPath(This,noname,retval)	\
    ( (This)->lpVtbl -> put_OutputPath(This,noname,retval) ) 

#define VBProjectConfigurationProperties4_get_IntermediatePath(This,retval)	\
    ( (This)->lpVtbl -> get_IntermediatePath(This,retval) ) 

#define VBProjectConfigurationProperties4_put_IntermediatePath(This,noname,retval)	\
    ( (This)->lpVtbl -> put_IntermediatePath(This,noname,retval) ) 

#define VBProjectConfigurationProperties4_get_DefineConstants(This,retval)	\
    ( (This)->lpVtbl -> get_DefineConstants(This,retval) ) 

#define VBProjectConfigurationProperties4_put_DefineConstants(This,noname,retval)	\
    ( (This)->lpVtbl -> put_DefineConstants(This,noname,retval) ) 

#define VBProjectConfigurationProperties4_get_RemoveIntegerChecks(This,retval)	\
    ( (This)->lpVtbl -> get_RemoveIntegerChecks(This,retval) ) 

#define VBProjectConfigurationProperties4_put_RemoveIntegerChecks(This,noname,retval)	\
    ( (This)->lpVtbl -> put_RemoveIntegerChecks(This,noname,retval) ) 

#define VBProjectConfigurationProperties4_get_BaseAddress(This,retval)	\
    ( (This)->lpVtbl -> get_BaseAddress(This,retval) ) 

#define VBProjectConfigurationProperties4_put_BaseAddress(This,noname,retval)	\
    ( (This)->lpVtbl -> put_BaseAddress(This,noname,retval) ) 

#define VBProjectConfigurationProperties4_get_AllowUnsafeBlocks(This,retval)	\
    ( (This)->lpVtbl -> get_AllowUnsafeBlocks(This,retval) ) 

#define VBProjectConfigurationProperties4_put_AllowUnsafeBlocks(This,noname,retval)	\
    ( (This)->lpVtbl -> put_AllowUnsafeBlocks(This,noname,retval) ) 

#define VBProjectConfigurationProperties4_get_CheckForOverflowUnderflow(This,retval)	\
    ( (This)->lpVtbl -> get_CheckForOverflowUnderflow(This,retval) ) 

#define VBProjectConfigurationProperties4_put_CheckForOverflowUnderflow(This,noname,retval)	\
    ( (This)->lpVtbl -> put_CheckForOverflowUnderflow(This,noname,retval) ) 

#define VBProjectConfigurationProperties4_get_DocumentationFile(This,retval)	\
    ( (This)->lpVtbl -> get_DocumentationFile(This,retval) ) 

#define VBProjectConfigurationProperties4_put_DocumentationFile(This,noname,retval)	\
    ( (This)->lpVtbl -> put_DocumentationFile(This,noname,retval) ) 

#define VBProjectConfigurationProperties4_get_Optimize(This,retval)	\
    ( (This)->lpVtbl -> get_Optimize(This,retval) ) 

#define VBProjectConfigurationProperties4_put_Optimize(This,noname,retval)	\
    ( (This)->lpVtbl -> put_Optimize(This,noname,retval) ) 

#define VBProjectConfigurationProperties4_get_IncrementalBuild(This,retval)	\
    ( (This)->lpVtbl -> get_IncrementalBuild(This,retval) ) 

#define VBProjectConfigurationProperties4_put_IncrementalBuild(This,noname,retval)	\
    ( (This)->lpVtbl -> put_IncrementalBuild(This,noname,retval) ) 

#define VBProjectConfigurationProperties4_get_StartProgram(This,retval)	\
    ( (This)->lpVtbl -> get_StartProgram(This,retval) ) 

#define VBProjectConfigurationProperties4_put_StartProgram(This,noname,retval)	\
    ( (This)->lpVtbl -> put_StartProgram(This,noname,retval) ) 

#define VBProjectConfigurationProperties4_get_StartWorkingDirectory(This,retval)	\
    ( (This)->lpVtbl -> get_StartWorkingDirectory(This,retval) ) 

#define VBProjectConfigurationProperties4_put_StartWorkingDirectory(This,noname,retval)	\
    ( (This)->lpVtbl -> put_StartWorkingDirectory(This,noname,retval) ) 

#define VBProjectConfigurationProperties4_get_StartURL(This,retval)	\
    ( (This)->lpVtbl -> get_StartURL(This,retval) ) 

#define VBProjectConfigurationProperties4_put_StartURL(This,noname,retval)	\
    ( (This)->lpVtbl -> put_StartURL(This,noname,retval) ) 

#define VBProjectConfigurationProperties4_get_StartPage(This,retval)	\
    ( (This)->lpVtbl -> get_StartPage(This,retval) ) 

#define VBProjectConfigurationProperties4_put_StartPage(This,noname,retval)	\
    ( (This)->lpVtbl -> put_StartPage(This,noname,retval) ) 

#define VBProjectConfigurationProperties4_get_StartArguments(This,retval)	\
    ( (This)->lpVtbl -> get_StartArguments(This,retval) ) 

#define VBProjectConfigurationProperties4_put_StartArguments(This,noname,retval)	\
    ( (This)->lpVtbl -> put_StartArguments(This,noname,retval) ) 

#define VBProjectConfigurationProperties4_get_StartWithIE(This,retval)	\
    ( (This)->lpVtbl -> get_StartWithIE(This,retval) ) 

#define VBProjectConfigurationProperties4_put_StartWithIE(This,noname,retval)	\
    ( (This)->lpVtbl -> put_StartWithIE(This,noname,retval) ) 

#define VBProjectConfigurationProperties4_get_EnableASPDebugging(This,retval)	\
    ( (This)->lpVtbl -> get_EnableASPDebugging(This,retval) ) 

#define VBProjectConfigurationProperties4_put_EnableASPDebugging(This,noname,retval)	\
    ( (This)->lpVtbl -> put_EnableASPDebugging(This,noname,retval) ) 

#define VBProjectConfigurationProperties4_get_EnableASPXDebugging(This,retval)	\
    ( (This)->lpVtbl -> get_EnableASPXDebugging(This,retval) ) 

#define VBProjectConfigurationProperties4_put_EnableASPXDebugging(This,noname,retval)	\
    ( (This)->lpVtbl -> put_EnableASPXDebugging(This,noname,retval) ) 

#define VBProjectConfigurationProperties4_get_EnableUnmanagedDebugging(This,retval)	\
    ( (This)->lpVtbl -> get_EnableUnmanagedDebugging(This,retval) ) 

#define VBProjectConfigurationProperties4_put_EnableUnmanagedDebugging(This,noname,retval)	\
    ( (This)->lpVtbl -> put_EnableUnmanagedDebugging(This,noname,retval) ) 

#define VBProjectConfigurationProperties4_get_StartAction(This,retval)	\
    ( (This)->lpVtbl -> get_StartAction(This,retval) ) 

#define VBProjectConfigurationProperties4_put_StartAction(This,noname,retval)	\
    ( (This)->lpVtbl -> put_StartAction(This,noname,retval) ) 

#define VBProjectConfigurationProperties4_get_Extender(This,ExtenderName,retval)	\
    ( (This)->lpVtbl -> get_Extender(This,ExtenderName,retval) ) 

#define VBProjectConfigurationProperties4_get_ExtenderNames(This,retval)	\
    ( (This)->lpVtbl -> get_ExtenderNames(This,retval) ) 

#define VBProjectConfigurationProperties4_get_ExtenderCATID(This,retval)	\
    ( (This)->lpVtbl -> get_ExtenderCATID(This,retval) ) 

#define VBProjectConfigurationProperties4_get_WarningLevel(This,retval)	\
    ( (This)->lpVtbl -> get_WarningLevel(This,retval) ) 

#define VBProjectConfigurationProperties4_put_WarningLevel(This,noname,retval)	\
    ( (This)->lpVtbl -> put_WarningLevel(This,noname,retval) ) 

#define VBProjectConfigurationProperties4_get_TreatWarningsAsErrors(This,retval)	\
    ( (This)->lpVtbl -> get_TreatWarningsAsErrors(This,retval) ) 

#define VBProjectConfigurationProperties4_put_TreatWarningsAsErrors(This,noname,retval)	\
    ( (This)->lpVtbl -> put_TreatWarningsAsErrors(This,noname,retval) ) 

#define VBProjectConfigurationProperties4_get_EnableSQLServerDebugging(This,retval)	\
    ( (This)->lpVtbl -> get_EnableSQLServerDebugging(This,retval) ) 

#define VBProjectConfigurationProperties4_put_EnableSQLServerDebugging(This,noname,retval)	\
    ( (This)->lpVtbl -> put_EnableSQLServerDebugging(This,noname,retval) ) 

#define VBProjectConfigurationProperties4_get_FileAlignment(This,retval)	\
    ( (This)->lpVtbl -> get_FileAlignment(This,retval) ) 

#define VBProjectConfigurationProperties4_put_FileAlignment(This,noname,retval)	\
    ( (This)->lpVtbl -> put_FileAlignment(This,noname,retval) ) 

#define VBProjectConfigurationProperties4_get_RegisterForComInterop(This,retval)	\
    ( (This)->lpVtbl -> get_RegisterForComInterop(This,retval) ) 

#define VBProjectConfigurationProperties4_put_RegisterForComInterop(This,noname,retval)	\
    ( (This)->lpVtbl -> put_RegisterForComInterop(This,noname,retval) ) 

#define VBProjectConfigurationProperties4_get_ConfigurationOverrideFile(This,retval)	\
    ( (This)->lpVtbl -> get_ConfigurationOverrideFile(This,retval) ) 

#define VBProjectConfigurationProperties4_put_ConfigurationOverrideFile(This,noname,retval)	\
    ( (This)->lpVtbl -> put_ConfigurationOverrideFile(This,noname,retval) ) 

#define VBProjectConfigurationProperties4_get_RemoteDebugEnabled(This,retval)	\
    ( (This)->lpVtbl -> get_RemoteDebugEnabled(This,retval) ) 

#define VBProjectConfigurationProperties4_put_RemoteDebugEnabled(This,noname,retval)	\
    ( (This)->lpVtbl -> put_RemoteDebugEnabled(This,noname,retval) ) 

#define VBProjectConfigurationProperties4_get_RemoteDebugMachine(This,retval)	\
    ( (This)->lpVtbl -> get_RemoteDebugMachine(This,retval) ) 

#define VBProjectConfigurationProperties4_put_RemoteDebugMachine(This,noname,retval)	\
    ( (This)->lpVtbl -> put_RemoteDebugMachine(This,noname,retval) ) 

#define VBProjectConfigurationProperties4_get_NoWarn(This,retval)	\
    ( (This)->lpVtbl -> get_NoWarn(This,retval) ) 

#define VBProjectConfigurationProperties4_put_NoWarn(This,noname,retval)	\
    ( (This)->lpVtbl -> put_NoWarn(This,noname,retval) ) 

#define VBProjectConfigurationProperties4_get_NoStdLib(This,retval)	\
    ( (This)->lpVtbl -> get_NoStdLib(This,retval) ) 

#define VBProjectConfigurationProperties4_put_NoStdLib(This,noname,retval)	\
    ( (This)->lpVtbl -> put_NoStdLib(This,noname,retval) ) 

#define VBProjectConfigurationProperties4_get_DebugInfo(This,retval)	\
    ( (This)->lpVtbl -> get_DebugInfo(This,retval) ) 

#define VBProjectConfigurationProperties4_put_DebugInfo(This,noname,retval)	\
    ( (This)->lpVtbl -> put_DebugInfo(This,noname,retval) ) 

#define VBProjectConfigurationProperties4_get_PlatformTarget(This,retval)	\
    ( (This)->lpVtbl -> get_PlatformTarget(This,retval) ) 

#define VBProjectConfigurationProperties4_put_PlatformTarget(This,noname,retval)	\
    ( (This)->lpVtbl -> put_PlatformTarget(This,noname,retval) ) 

#define VBProjectConfigurationProperties4_get_TreatSpecificWarningsAsErrors(This,retval)	\
    ( (This)->lpVtbl -> get_TreatSpecificWarningsAsErrors(This,retval) ) 

#define VBProjectConfigurationProperties4_put_TreatSpecificWarningsAsErrors(This,noname,retval)	\
    ( (This)->lpVtbl -> put_TreatSpecificWarningsAsErrors(This,noname,retval) ) 

#define VBProjectConfigurationProperties4_get_RunCodeAnalysis(This,retval)	\
    ( (This)->lpVtbl -> get_RunCodeAnalysis(This,retval) ) 

#define VBProjectConfigurationProperties4_put_RunCodeAnalysis(This,noname,retval)	\
    ( (This)->lpVtbl -> put_RunCodeAnalysis(This,noname,retval) ) 

#define VBProjectConfigurationProperties4_get_CodeAnalysisLogFile(This,retval)	\
    ( (This)->lpVtbl -> get_CodeAnalysisLogFile(This,retval) ) 

#define VBProjectConfigurationProperties4_put_CodeAnalysisLogFile(This,noname,retval)	\
    ( (This)->lpVtbl -> put_CodeAnalysisLogFile(This,noname,retval) ) 

#define VBProjectConfigurationProperties4_get_CodeAnalysisRuleAssemblies(This,retval)	\
    ( (This)->lpVtbl -> get_CodeAnalysisRuleAssemblies(This,retval) ) 

#define VBProjectConfigurationProperties4_put_CodeAnalysisRuleAssemblies(This,noname,retval)	\
    ( (This)->lpVtbl -> put_CodeAnalysisRuleAssemblies(This,noname,retval) ) 

#define VBProjectConfigurationProperties4_get_CodeAnalysisInputAssembly(This,retval)	\
    ( (This)->lpVtbl -> get_CodeAnalysisInputAssembly(This,retval) ) 

#define VBProjectConfigurationProperties4_put_CodeAnalysisInputAssembly(This,noname,retval)	\
    ( (This)->lpVtbl -> put_CodeAnalysisInputAssembly(This,noname,retval) ) 

#define VBProjectConfigurationProperties4_get_CodeAnalysisRules(This,retval)	\
    ( (This)->lpVtbl -> get_CodeAnalysisRules(This,retval) ) 

#define VBProjectConfigurationProperties4_put_CodeAnalysisRules(This,noname,retval)	\
    ( (This)->lpVtbl -> put_CodeAnalysisRules(This,noname,retval) ) 

#define VBProjectConfigurationProperties4_get_CodeAnalysisSpellCheckLanguages(This,retval)	\
    ( (This)->lpVtbl -> get_CodeAnalysisSpellCheckLanguages(This,retval) ) 

#define VBProjectConfigurationProperties4_put_CodeAnalysisSpellCheckLanguages(This,noname,retval)	\
    ( (This)->lpVtbl -> put_CodeAnalysisSpellCheckLanguages(This,noname,retval) ) 

#define VBProjectConfigurationProperties4_get_CodeAnalysisUseTypeNameInSuppression(This,retval)	\
    ( (This)->lpVtbl -> get_CodeAnalysisUseTypeNameInSuppression(This,retval) ) 

#define VBProjectConfigurationProperties4_put_CodeAnalysisUseTypeNameInSuppression(This,noname,retval)	\
    ( (This)->lpVtbl -> put_CodeAnalysisUseTypeNameInSuppression(This,noname,retval) ) 

#define VBProjectConfigurationProperties4_get_CodeAnalysisModuleSuppressionsFile(This,retval)	\
    ( (This)->lpVtbl -> get_CodeAnalysisModuleSuppressionsFile(This,retval) ) 

#define VBProjectConfigurationProperties4_put_CodeAnalysisModuleSuppressionsFile(This,noname,retval)	\
    ( (This)->lpVtbl -> put_CodeAnalysisModuleSuppressionsFile(This,noname,retval) ) 

#define VBProjectConfigurationProperties4_get_UseVSHostingProcess(This,retval)	\
    ( (This)->lpVtbl -> get_UseVSHostingProcess(This,retval) ) 

#define VBProjectConfigurationProperties4_put_UseVSHostingProcess(This,noname,retval)	\
    ( (This)->lpVtbl -> put_UseVSHostingProcess(This,noname,retval) ) 

#define VBProjectConfigurationProperties4_get_GenerateSerializationAssemblies(This,retval)	\
    ( (This)->lpVtbl -> get_GenerateSerializationAssemblies(This,retval) ) 

#define VBProjectConfigurationProperties4_put_GenerateSerializationAssemblies(This,noname,retval)	\
    ( (This)->lpVtbl -> put_GenerateSerializationAssemblies(This,noname,retval) ) 


#define VBProjectConfigurationProperties4_get_CodeAnalysisIgnoreGeneratedCode(This,pbIgnoreGeneratedCode)	\
    ( (This)->lpVtbl -> get_CodeAnalysisIgnoreGeneratedCode(This,pbIgnoreGeneratedCode) ) 

#define VBProjectConfigurationProperties4_put_CodeAnalysisIgnoreGeneratedCode(This,IgnoreGeneratedCode)	\
    ( (This)->lpVtbl -> put_CodeAnalysisIgnoreGeneratedCode(This,IgnoreGeneratedCode) ) 

#define VBProjectConfigurationProperties4_get_CodeAnalysisOverrideRuleVisibilities(This,pbOverrideRuleVisibilities)	\
    ( (This)->lpVtbl -> get_CodeAnalysisOverrideRuleVisibilities(This,pbOverrideRuleVisibilities) ) 

#define VBProjectConfigurationProperties4_put_CodeAnalysisOverrideRuleVisibilities(This,OverrideRuleVisibilities)	\
    ( (This)->lpVtbl -> put_CodeAnalysisOverrideRuleVisibilities(This,OverrideRuleVisibilities) ) 

#define VBProjectConfigurationProperties4_get_CodeAnalysisDictionaries(This,pbstrDictionaries)	\
    ( (This)->lpVtbl -> get_CodeAnalysisDictionaries(This,pbstrDictionaries) ) 

#define VBProjectConfigurationProperties4_put_CodeAnalysisDictionaries(This,Dictionaries)	\
    ( (This)->lpVtbl -> put_CodeAnalysisDictionaries(This,Dictionaries) ) 

#define VBProjectConfigurationProperties4_get_CodeAnalysisCulture(This,pbstrCodeAnalysisCulture)	\
    ( (This)->lpVtbl -> get_CodeAnalysisCulture(This,pbstrCodeAnalysisCulture) ) 

#define VBProjectConfigurationProperties4_put_CodeAnalysisCulture(This,pbstrCodeAnalysisCulture)	\
    ( (This)->lpVtbl -> put_CodeAnalysisCulture(This,pbstrCodeAnalysisCulture) ) 

#endif /* COBJMACROS */


#endif 	/* C style interface */




#endif 	/* __VBProjectConfigurationProperties4_INTERFACE_DEFINED__ */

#endif /* __VSLangProj90_LIBRARY_DEFINED__ */

/* Additional Prototypes for ALL interfaces */

/* end of Additional Prototypes */

#ifdef __cplusplus
}
#endif

#endif


