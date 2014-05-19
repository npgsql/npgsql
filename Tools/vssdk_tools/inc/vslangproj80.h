

/* this ALWAYS GENERATED file contains the definitions for the interfaces */


 /* File created by MIDL compiler version 7.00.0499 */
/* Compiler settings for vslangproj80.idl:
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


#ifndef __vslangproj80_h__
#define __vslangproj80_h__

#if defined(_MSC_VER) && (_MSC_VER >= 1020)
#pragma once
#endif

/* Forward Declarations */ 

#ifndef __ProjectConfigurationProperties3_FWD_DEFINED__
#define __ProjectConfigurationProperties3_FWD_DEFINED__
typedef interface ProjectConfigurationProperties3 ProjectConfigurationProperties3;
#endif 	/* __ProjectConfigurationProperties3_FWD_DEFINED__ */


#ifndef ___VSLangProjWebReferencesEvents_FWD_DEFINED__
#define ___VSLangProjWebReferencesEvents_FWD_DEFINED__
typedef interface _VSLangProjWebReferencesEvents _VSLangProjWebReferencesEvents;
#endif 	/* ___VSLangProjWebReferencesEvents_FWD_DEFINED__ */


#ifndef ___dispVSLangProjWebReferencesEvents_FWD_DEFINED__
#define ___dispVSLangProjWebReferencesEvents_FWD_DEFINED__
typedef interface _dispVSLangProjWebReferencesEvents _dispVSLangProjWebReferencesEvents;
#endif 	/* ___dispVSLangProjWebReferencesEvents_FWD_DEFINED__ */


#ifndef __VSLangProjWebReferencesEvents_FWD_DEFINED__
#define __VSLangProjWebReferencesEvents_FWD_DEFINED__

#ifdef __cplusplus
typedef class VSLangProjWebReferencesEvents VSLangProjWebReferencesEvents;
#else
typedef struct VSLangProjWebReferencesEvents VSLangProjWebReferencesEvents;
#endif /* __cplusplus */

#endif 	/* __VSLangProjWebReferencesEvents_FWD_DEFINED__ */


#ifndef __VSProjectEvents2_FWD_DEFINED__
#define __VSProjectEvents2_FWD_DEFINED__
typedef interface VSProjectEvents2 VSProjectEvents2;
#endif 	/* __VSProjectEvents2_FWD_DEFINED__ */


#ifndef __VSProject2_FWD_DEFINED__
#define __VSProject2_FWD_DEFINED__
typedef interface VSProject2 VSProject2;
#endif 	/* __VSProject2_FWD_DEFINED__ */


#ifndef __ProjectProperties3_FWD_DEFINED__
#define __ProjectProperties3_FWD_DEFINED__
typedef interface ProjectProperties3 ProjectProperties3;
#endif 	/* __ProjectProperties3_FWD_DEFINED__ */


#ifndef __VBProjectProperties3_FWD_DEFINED__
#define __VBProjectProperties3_FWD_DEFINED__
typedef interface VBProjectProperties3 VBProjectProperties3;
#endif 	/* __VBProjectProperties3_FWD_DEFINED__ */


#ifndef __CSharpProjectConfigurationProperties3_FWD_DEFINED__
#define __CSharpProjectConfigurationProperties3_FWD_DEFINED__
typedef interface CSharpProjectConfigurationProperties3 CSharpProjectConfigurationProperties3;
#endif 	/* __CSharpProjectConfigurationProperties3_FWD_DEFINED__ */


#ifndef __Reference3_FWD_DEFINED__
#define __Reference3_FWD_DEFINED__
typedef interface Reference3 Reference3;
#endif 	/* __Reference3_FWD_DEFINED__ */


#ifndef __JSharpProjectConfigurationProperties3_FWD_DEFINED__
#define __JSharpProjectConfigurationProperties3_FWD_DEFINED__
typedef interface JSharpProjectConfigurationProperties3 JSharpProjectConfigurationProperties3;
#endif 	/* __JSharpProjectConfigurationProperties3_FWD_DEFINED__ */


#ifndef __FolderProperties2_FWD_DEFINED__
#define __FolderProperties2_FWD_DEFINED__
typedef interface FolderProperties2 FolderProperties2;
#endif 	/* __FolderProperties2_FWD_DEFINED__ */


#ifndef __FileProperties2_FWD_DEFINED__
#define __FileProperties2_FWD_DEFINED__
typedef interface FileProperties2 FileProperties2;
#endif 	/* __FileProperties2_FWD_DEFINED__ */


#ifndef __IVsApplicationSettings_FWD_DEFINED__
#define __IVsApplicationSettings_FWD_DEFINED__
typedef interface IVsApplicationSettings IVsApplicationSettings;
#endif 	/* __IVsApplicationSettings_FWD_DEFINED__ */


#ifndef __SVSWebReferenceDynamicProperties_FWD_DEFINED__
#define __SVSWebReferenceDynamicProperties_FWD_DEFINED__
typedef interface SVSWebReferenceDynamicProperties SVSWebReferenceDynamicProperties;
#endif 	/* __SVSWebReferenceDynamicProperties_FWD_DEFINED__ */


#ifndef __IVSWebReferenceDynamicProperties2_FWD_DEFINED__
#define __IVSWebReferenceDynamicProperties2_FWD_DEFINED__
typedef interface IVSWebReferenceDynamicProperties2 IVSWebReferenceDynamicProperties2;
#endif 	/* __IVSWebReferenceDynamicProperties2_FWD_DEFINED__ */


/* header files for imported files */
#include "oaidl.h"

#ifdef __cplusplus
extern "C"{
#endif 


/* interface __MIDL_itf_vslangproj80_0000_0000 */
/* [local] */ 

#define CSharpProjectProperties3 ProjectProperties3
#define IID_CSharpProjectProperties3 IID_ProjectProperties3
#define VJSharpProjectProperties3 ProjectProperties3
#define IID_VJSharpProjectProperties3 IID_ProjectProperties3
#define VSLANGPROJ80_VER_MAJ    8
#define VSLANGPROJ80_VER_MIN    0


extern RPC_IF_HANDLE __MIDL_itf_vslangproj80_0000_0000_v0_0_c_ifspec;
extern RPC_IF_HANDLE __MIDL_itf_vslangproj80_0000_0000_v0_0_s_ifspec;


#ifndef __VSLangProj80_LIBRARY_DEFINED__
#define __VSLangProj80_LIBRARY_DEFINED__

/* library VSLangProj80 */
/* [version][helpstring][uuid] */ 


enum VsProjPropId80
    {	VBPROJPROPID_DebugInfo	= 10106,
	VBPROJPROPID_PlatformTarget	= 10109,
	VBPROJPROPID_UseVSHostingProcess	= 10110,
	VBPROJPROPID_GenerateSerializationAssemblies	= 10111,
	VBPROJPROPID_RunCodeAnalysis	= 12200,
	VBPROJPROPID_CodeAnalysisLogFile	= 12201,
	VBPROJPROPID_CodeAnalysisRuleAssemblies	= 12202,
	VBPROJPROPID_CodeAnalysisRules	= 12203,
	VBPROJPROPID_CodeAnalysisInputAssembly	= 12204,
	VBPROJPROPID_CodeAnalysisSpellCheckLanguages	= 12205,
	VBPROJPROPID_CodeAnalysisUseTypeNameInSuppression	= 12206,
	VBPROJPROPID_CodeAnalysisModuleSuppressionsFile	= 12207,
	VBPROJPROPID_InstanceType	= 12100,
	VBPROJPROPID_ShutdownMode	= 12101,
	VBPROJPROPID_AssemblyTitle	= 12102,
	VBPROJPROPID_AssemblyDescription	= 12103,
	VBPROJPROPID_AssemblyCompany	= 12104,
	VBPROJPROPID_AssemblyProduct	= 12105,
	VBPROJPROPID_AssemblyCopyright	= 12106,
	VBPROJPROPID_AssemblyTrademark	= 12107,
	VBPROJPROPID_AssemblyType	= 12108,
	VBPROJPROPID_TypeComplianceDiagnostics	= 12109,
	VBPROJPROPID_CompatibilityChecks	= 12110,
	VBPROJPROPID_CompatibleAssembly	= 12111,
	VBPROJPROPID_Win32ResourceFile	= 12112,
	VBPROJPROPID_AssemblyOriginatorKeyFileType	= 12114,
	VBPROJPROPID_AssemblyKeyProviderName	= 12115,
	VBPROJPROPID_AssemblyVersion	= 12116,
	VBPROJPROPID_AssemblyFileVersion	= 12117,
	VBPROJPROPID_SecureDebugURL	= 12118,
	VBPROJPROPID_GenerateManifests	= 12119,
	VBPROJPROPID_EnableSecurityDebugging	= 12120,
	VBPROJPROPID_TreatSpecificWarningsAsErrors	= 12121,
	VBPROJPROPID_Publish	= 12122,
	VBPROJPROPID_ComVisible	= 12123,
	VBPROJPROPID_AssemblyGuid	= 12124,
	VBPROJPROPID_NeutralResourcesLanguage	= 12125,
	VBPROJPROPID_SignAssembly	= 12126,
	VBPROJPROPID_TargetZone	= 12127,
	VBPROJPROPID_ExcludedPermissions	= 12128,
	VBPROJPROPID_ManifestCertificateThumbprint	= 12129,
	VBPROJPROPID_ManifestKeyFile	= 12130,
	VBPROJPROPID_ManifestTimestampUrl	= 12131,
	VBPROJPROPID_SignManifests	= 12132
    } ;

enum VsProjFolderPropId80
    {	DISPID_VBFolder_WebReferenceInterface	= 11
    } ;

enum VsProjReferencePropId80
    {	DISPID_Reference_SpecificVersion	= 120,
	DISPID_Reference_SubType	= 121,
	DISPID_Reference_Isolated	= 122,
	DISPID_Reference_Aliases	= 123,
	DISPID_Reference_RefType	= 124,
	DISPID_Reference_AutoReferenced	= 125,
	DISPID_Reference_Resolved	= 126
    } ;

enum VsProjFilePropId80
    {	DISPID_VBFile_CopyToOutputDirectory	= ( DISPID_VALUE + 100 ) ,
	DISPID_VBFile_ItemType	= ( DISPID_VALUE + 101 ) ,
	DISPID_VBFile_IsSharedDesignTimeBuildInput	= ( DISPID_VALUE + 102 ) 
    } ;

enum CSharpProjPropId
    {	CSPROJPROPID_TreatSpecificWarningsAsErrors	= 13102,
	CSPROJPROPID_LanguageVersion	= 13104,
	CSPROJPROPID_ErrorReport	= 13105
    } ;

enum VBProjPropId
    {	VBPROJPROPID_MyApplication	= 14101,
	VBPROJPROPID_MyType	= 14102
    } ;

enum JSharpProjPropId
    {	JSPROJPROPID_CodePage	= 15101,
	JSPROJPROPID_JCPA	= 15102,
	JSPROJPROPID_DisableLangXtns	= 15103,
	JSPROJPROPID_SecureScoping	= 15104
    } ;

enum __PROJECTREFERENCETYPE
    {	PROJREFTYPE_ASSEMBLY	= 1,
	PROJREFTYPE_ACTIVEX	= 2,
	PROJREFTYPE_NATIVE	= 3
    } ;
typedef DWORD PROJECTREFERENCETYPE;


enum _prjOriginatorKeyFileType
    {	prjOriginatorKeyFileTypeSNK	= 1,
	prjOriginatorKeyFileTypePFX	= 2
    } ;
typedef DWORD prjOriginatorKeyFileType;


enum __COPYTOOUTPUTSTATE
    {	COPYTOOUTPUTSTATE_Never	= 0,
	COPYTOOUTPUTSTATE_Always	= 1,
	COPYTOOUTPUTSTATE_PreserveNewestFile	= 2
    } ;
typedef DWORD COPYTOOUTPUTSTATE;

#define SID_SVsApplicationSettings IID_IVsApplicationSettings

EXTERN_C const IID LIBID_VSLangProj80;

#ifndef __ProjectConfigurationProperties3_INTERFACE_DEFINED__
#define __ProjectConfigurationProperties3_INTERFACE_DEFINED__

/* interface ProjectConfigurationProperties3 */
/* [object][dual][unique][helpstring][uuid] */ 

typedef /* [uuid] */  DECLSPEC_UUID("DEBB3D21-A024-410e-AA3B-1ABC31625EB7") 
enum sgenGenerationOption
    {	sgenGenerationOption_Off	= 0,
	sgenGenerationOption_On	= 1,
	sgenGenerationOption_Auto	= 2
    } 	sgenGenerationOption;

#define sgenGenerationOptionMin  sgenGenerationOption_Off
#define sgenGenerationOptionMax  sgenGenerationOption_Auto

EXTERN_C const IID IID_ProjectConfigurationProperties3;

#if defined(__cplusplus) && !defined(CINTERFACE)
    
    MIDL_INTERFACE("9918DAB5-FC67-4cd4-B352-2F3FA1E2BD08")
    ProjectConfigurationProperties3 : public ProjectConfigurationProperties2
    {
    public:
        virtual /* [helpstring][id][propget] */ HRESULT STDMETHODCALLTYPE get_DebugInfo( 
            /* [retval][out] */ __RPC__deref_out_opt BSTR *pbstrDebugInfo) = 0;
        
        virtual /* [helpstring][id][propput] */ HRESULT STDMETHODCALLTYPE put_DebugInfo( 
            /* [in] */ __RPC__in BSTR DebugInfo) = 0;
        
        virtual /* [helpstring][id][propget] */ HRESULT STDMETHODCALLTYPE get_PlatformTarget( 
            /* [retval][out] */ __RPC__deref_out_opt BSTR *pbstrPlatformTarget) = 0;
        
        virtual /* [helpstring][id][propput] */ HRESULT STDMETHODCALLTYPE put_PlatformTarget( 
            /* [in] */ __RPC__in BSTR PlatformTarget) = 0;
        
        virtual /* [helpstring][id][propget] */ HRESULT STDMETHODCALLTYPE get_TreatSpecificWarningsAsErrors( 
            /* [retval][out] */ __RPC__deref_out_opt BSTR *pbstrWarningsAsErrors) = 0;
        
        virtual /* [helpstring][id][propput] */ HRESULT STDMETHODCALLTYPE put_TreatSpecificWarningsAsErrors( 
            /* [in] */ __RPC__in BSTR WarningsAsErrors) = 0;
        
        virtual /* [helpstring][id][propget] */ HRESULT STDMETHODCALLTYPE get_RunCodeAnalysis( 
            /* [retval][out] */ __RPC__out VARIANT_BOOL *pbRunCodeAnalysis) = 0;
        
        virtual /* [helpstring][id][propput] */ HRESULT STDMETHODCALLTYPE put_RunCodeAnalysis( 
            /* [in] */ VARIANT_BOOL RunCodeAnalysis) = 0;
        
        virtual /* [helpstring][id][propget] */ HRESULT STDMETHODCALLTYPE get_CodeAnalysisLogFile( 
            /* [retval][out] */ __RPC__deref_out_opt BSTR *pbstrCodeAnalysisLogFile) = 0;
        
        virtual /* [helpstring][id][propput] */ HRESULT STDMETHODCALLTYPE put_CodeAnalysisLogFile( 
            /* [in] */ __RPC__in BSTR CodeAnalysisLogFile) = 0;
        
        virtual /* [helpstring][id][propget] */ HRESULT STDMETHODCALLTYPE get_CodeAnalysisRuleAssemblies( 
            /* [retval][out] */ __RPC__deref_out_opt BSTR *pbstrCodeAnalysisRuleAssemblies) = 0;
        
        virtual /* [helpstring][id][propput] */ HRESULT STDMETHODCALLTYPE put_CodeAnalysisRuleAssemblies( 
            /* [in] */ __RPC__in BSTR CodeAnalysisRuleAssemblies) = 0;
        
        virtual /* [helpstring][id][propget] */ HRESULT STDMETHODCALLTYPE get_CodeAnalysisInputAssembly( 
            /* [retval][out] */ __RPC__deref_out_opt BSTR *pbstrCodeAnalysisInputAssembly) = 0;
        
        virtual /* [helpstring][id][propput] */ HRESULT STDMETHODCALLTYPE put_CodeAnalysisInputAssembly( 
            /* [in] */ __RPC__in BSTR CodeAnalysisInputAssembly) = 0;
        
        virtual /* [helpstring][id][propget] */ HRESULT STDMETHODCALLTYPE get_CodeAnalysisRules( 
            /* [retval][out] */ __RPC__deref_out_opt BSTR *pbstrCodeAnalysisRules) = 0;
        
        virtual /* [helpstring][id][propput] */ HRESULT STDMETHODCALLTYPE put_CodeAnalysisRules( 
            /* [in] */ __RPC__in BSTR CodeAnalysisRules) = 0;
        
        virtual /* [helpstring][id][propget] */ HRESULT STDMETHODCALLTYPE get_CodeAnalysisSpellCheckLanguages( 
            /* [retval][out] */ __RPC__deref_out_opt BSTR *pbstrCodeAnalysisSpellCheckLanguages) = 0;
        
        virtual /* [helpstring][id][propput] */ HRESULT STDMETHODCALLTYPE put_CodeAnalysisSpellCheckLanguages( 
            /* [in] */ __RPC__in BSTR CodeAnalysisSpellCheckLanguages) = 0;
        
        virtual /* [helpstring][id][propget] */ HRESULT STDMETHODCALLTYPE get_CodeAnalysisUseTypeNameInSuppression( 
            /* [retval][out] */ __RPC__out VARIANT_BOOL *bUseTypeNameInSuppression) = 0;
        
        virtual /* [helpstring][id][propput] */ HRESULT STDMETHODCALLTYPE put_CodeAnalysisUseTypeNameInSuppression( 
            /* [in] */ VARIANT_BOOL UseTypeNameInSuppression) = 0;
        
        virtual /* [helpstring][id][propget] */ HRESULT STDMETHODCALLTYPE get_CodeAnalysisModuleSuppressionsFile( 
            /* [retval][out] */ __RPC__deref_out_opt BSTR *pbstrCodeAnalysisModuleSuppressionsFile) = 0;
        
        virtual /* [helpstring][id][propput] */ HRESULT STDMETHODCALLTYPE put_CodeAnalysisModuleSuppressionsFile( 
            /* [in] */ __RPC__in BSTR CodeAnalysisModuleSuppressionsFile) = 0;
        
        virtual /* [helpstring][id][propget] */ HRESULT STDMETHODCALLTYPE get_UseVSHostingProcess( 
            /* [retval][out] */ __RPC__out VARIANT_BOOL *pbUseVSHostingProcess) = 0;
        
        virtual /* [helpstring][id][propput] */ HRESULT STDMETHODCALLTYPE put_UseVSHostingProcess( 
            /* [in] */ VARIANT_BOOL UseVSHostingProcess) = 0;
        
        virtual /* [helpstring][id][propget] */ HRESULT STDMETHODCALLTYPE get_GenerateSerializationAssemblies( 
            /* [retval][out] */ __RPC__out sgenGenerationOption *pSgenGenerationOption) = 0;
        
        virtual /* [helpstring][id][propput] */ HRESULT STDMETHODCALLTYPE put_GenerateSerializationAssemblies( 
            /* [in] */ sgenGenerationOption SgenGenerationOption) = 0;
        
    };
    
#else 	/* C style interface */

    typedef struct ProjectConfigurationProperties3Vtbl
    {
        BEGIN_INTERFACE
        
        /* [id][restricted][funcdescattr] */ HRESULT ( STDMETHODCALLTYPE *QueryInterface )( 
            ProjectConfigurationProperties3 * This,
            /* [in][idldescattr] */ __RPC__in struct GUID *riid,
            /* [out][idldescattr] */ __RPC__deref_out_opt void **ppvObj,
            /* [retval][out] */ __RPC__out void *retval);
        
        /* [id][restricted][funcdescattr] */ HRESULT ( STDMETHODCALLTYPE *AddRef )( 
            ProjectConfigurationProperties3 * This,
            /* [retval][out] */ __RPC__out unsigned long *retval);
        
        /* [id][restricted][funcdescattr] */ HRESULT ( STDMETHODCALLTYPE *Release )( 
            ProjectConfigurationProperties3 * This,
            /* [retval][out] */ __RPC__out unsigned long *retval);
        
        /* [id][restricted][funcdescattr] */ HRESULT ( STDMETHODCALLTYPE *GetTypeInfoCount )( 
            ProjectConfigurationProperties3 * This,
            /* [out][idldescattr] */ __RPC__out unsigned UINT *pctinfo,
            /* [retval][out] */ __RPC__out void *retval);
        
        /* [id][restricted][funcdescattr] */ HRESULT ( STDMETHODCALLTYPE *GetTypeInfo )( 
            ProjectConfigurationProperties3 * This,
            /* [in][idldescattr] */ unsigned UINT itinfo,
            /* [in][idldescattr] */ unsigned long lcid,
            /* [out][idldescattr] */ __RPC__deref_out_opt void **pptinfo,
            /* [retval][out] */ __RPC__out void *retval);
        
        /* [id][restricted][funcdescattr] */ HRESULT ( STDMETHODCALLTYPE *GetIDsOfNames )( 
            ProjectConfigurationProperties3 * This,
            /* [in][idldescattr] */ __RPC__in struct GUID *riid,
            /* [in][idldescattr] */ __RPC__deref_in_opt signed char **rgszNames,
            /* [in][idldescattr] */ unsigned UINT cNames,
            /* [in][idldescattr] */ unsigned long lcid,
            /* [out][idldescattr] */ __RPC__out signed long *rgdispid,
            /* [retval][out] */ __RPC__out void *retval);
        
        /* [id][restricted][funcdescattr] */ HRESULT ( STDMETHODCALLTYPE *Invoke )( 
            ProjectConfigurationProperties3 * This,
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
            ProjectConfigurationProperties3 * This,
            /* [retval][out] */ __RPC__deref_out_opt BSTR *retval);
        
        /* [id][propget][funcdescattr] */ HRESULT ( STDMETHODCALLTYPE *get_DebugSymbols )( 
            ProjectConfigurationProperties3 * This,
            /* [retval][out] */ __RPC__out BOOLEAN *retval);
        
        /* [id][propput][funcdescattr] */ HRESULT ( STDMETHODCALLTYPE *put_DebugSymbols )( 
            ProjectConfigurationProperties3 * This,
            /* [in][idldescattr] */ BOOLEAN noname,
            /* [retval][out] */ __RPC__out void *retval);
        
        /* [id][propget][funcdescattr] */ HRESULT ( STDMETHODCALLTYPE *get_DefineDebug )( 
            ProjectConfigurationProperties3 * This,
            /* [retval][out] */ __RPC__out BOOLEAN *retval);
        
        /* [id][propput][funcdescattr] */ HRESULT ( STDMETHODCALLTYPE *put_DefineDebug )( 
            ProjectConfigurationProperties3 * This,
            /* [in][idldescattr] */ BOOLEAN noname,
            /* [retval][out] */ __RPC__out void *retval);
        
        /* [id][propget][funcdescattr] */ HRESULT ( STDMETHODCALLTYPE *get_DefineTrace )( 
            ProjectConfigurationProperties3 * This,
            /* [retval][out] */ __RPC__out BOOLEAN *retval);
        
        /* [id][propput][funcdescattr] */ HRESULT ( STDMETHODCALLTYPE *put_DefineTrace )( 
            ProjectConfigurationProperties3 * This,
            /* [in][idldescattr] */ BOOLEAN noname,
            /* [retval][out] */ __RPC__out void *retval);
        
        /* [id][propget][funcdescattr] */ HRESULT ( STDMETHODCALLTYPE *get_OutputPath )( 
            ProjectConfigurationProperties3 * This,
            /* [retval][out] */ __RPC__deref_out_opt BSTR *retval);
        
        /* [id][propput][funcdescattr] */ HRESULT ( STDMETHODCALLTYPE *put_OutputPath )( 
            ProjectConfigurationProperties3 * This,
            /* [in][idldescattr] */ __RPC__in BSTR noname,
            /* [retval][out] */ __RPC__out void *retval);
        
        /* [id][propget][funcdescattr] */ HRESULT ( STDMETHODCALLTYPE *get_IntermediatePath )( 
            ProjectConfigurationProperties3 * This,
            /* [retval][out] */ __RPC__deref_out_opt BSTR *retval);
        
        /* [id][propput][hidden][funcdescattr] */ HRESULT ( STDMETHODCALLTYPE *put_IntermediatePath )( 
            ProjectConfigurationProperties3 * This,
            /* [in][idldescattr] */ __RPC__in BSTR noname,
            /* [retval][out] */ __RPC__out void *retval);
        
        /* [id][propget][funcdescattr] */ HRESULT ( STDMETHODCALLTYPE *get_DefineConstants )( 
            ProjectConfigurationProperties3 * This,
            /* [retval][out] */ __RPC__deref_out_opt BSTR *retval);
        
        /* [id][propput][funcdescattr] */ HRESULT ( STDMETHODCALLTYPE *put_DefineConstants )( 
            ProjectConfigurationProperties3 * This,
            /* [in][idldescattr] */ __RPC__in BSTR noname,
            /* [retval][out] */ __RPC__out void *retval);
        
        /* [id][propget][funcdescattr] */ HRESULT ( STDMETHODCALLTYPE *get_RemoveIntegerChecks )( 
            ProjectConfigurationProperties3 * This,
            /* [retval][out] */ __RPC__out BOOLEAN *retval);
        
        /* [id][propput][funcdescattr] */ HRESULT ( STDMETHODCALLTYPE *put_RemoveIntegerChecks )( 
            ProjectConfigurationProperties3 * This,
            /* [in][idldescattr] */ BOOLEAN noname,
            /* [retval][out] */ __RPC__out void *retval);
        
        /* [id][propget][funcdescattr] */ HRESULT ( STDMETHODCALLTYPE *get_BaseAddress )( 
            ProjectConfigurationProperties3 * This,
            /* [retval][out] */ __RPC__out unsigned long *retval);
        
        /* [id][propput][funcdescattr] */ HRESULT ( STDMETHODCALLTYPE *put_BaseAddress )( 
            ProjectConfigurationProperties3 * This,
            /* [in][idldescattr] */ unsigned long noname,
            /* [retval][out] */ __RPC__out void *retval);
        
        /* [id][propget][funcdescattr] */ HRESULT ( STDMETHODCALLTYPE *get_AllowUnsafeBlocks )( 
            ProjectConfigurationProperties3 * This,
            /* [retval][out] */ __RPC__out BOOLEAN *retval);
        
        /* [id][propput][funcdescattr] */ HRESULT ( STDMETHODCALLTYPE *put_AllowUnsafeBlocks )( 
            ProjectConfigurationProperties3 * This,
            /* [in][idldescattr] */ BOOLEAN noname,
            /* [retval][out] */ __RPC__out void *retval);
        
        /* [id][propget][funcdescattr] */ HRESULT ( STDMETHODCALLTYPE *get_CheckForOverflowUnderflow )( 
            ProjectConfigurationProperties3 * This,
            /* [retval][out] */ __RPC__out BOOLEAN *retval);
        
        /* [id][propput][funcdescattr] */ HRESULT ( STDMETHODCALLTYPE *put_CheckForOverflowUnderflow )( 
            ProjectConfigurationProperties3 * This,
            /* [in][idldescattr] */ BOOLEAN noname,
            /* [retval][out] */ __RPC__out void *retval);
        
        /* [id][propget][funcdescattr] */ HRESULT ( STDMETHODCALLTYPE *get_DocumentationFile )( 
            ProjectConfigurationProperties3 * This,
            /* [retval][out] */ __RPC__deref_out_opt BSTR *retval);
        
        /* [id][propput][funcdescattr] */ HRESULT ( STDMETHODCALLTYPE *put_DocumentationFile )( 
            ProjectConfigurationProperties3 * This,
            /* [in][idldescattr] */ __RPC__in BSTR noname,
            /* [retval][out] */ __RPC__out void *retval);
        
        /* [id][propget][funcdescattr] */ HRESULT ( STDMETHODCALLTYPE *get_Optimize )( 
            ProjectConfigurationProperties3 * This,
            /* [retval][out] */ __RPC__out BOOLEAN *retval);
        
        /* [id][propput][funcdescattr] */ HRESULT ( STDMETHODCALLTYPE *put_Optimize )( 
            ProjectConfigurationProperties3 * This,
            /* [in][idldescattr] */ BOOLEAN noname,
            /* [retval][out] */ __RPC__out void *retval);
        
        /* [id][propget][funcdescattr] */ HRESULT ( STDMETHODCALLTYPE *get_IncrementalBuild )( 
            ProjectConfigurationProperties3 * This,
            /* [retval][out] */ __RPC__out BOOLEAN *retval);
        
        /* [id][propput][funcdescattr] */ HRESULT ( STDMETHODCALLTYPE *put_IncrementalBuild )( 
            ProjectConfigurationProperties3 * This,
            /* [in][idldescattr] */ BOOLEAN noname,
            /* [retval][out] */ __RPC__out void *retval);
        
        /* [id][propget][funcdescattr] */ HRESULT ( STDMETHODCALLTYPE *get_StartProgram )( 
            ProjectConfigurationProperties3 * This,
            /* [retval][out] */ __RPC__deref_out_opt BSTR *retval);
        
        /* [id][propput][funcdescattr] */ HRESULT ( STDMETHODCALLTYPE *put_StartProgram )( 
            ProjectConfigurationProperties3 * This,
            /* [in][idldescattr] */ __RPC__in BSTR noname,
            /* [retval][out] */ __RPC__out void *retval);
        
        /* [id][propget][funcdescattr] */ HRESULT ( STDMETHODCALLTYPE *get_StartWorkingDirectory )( 
            ProjectConfigurationProperties3 * This,
            /* [retval][out] */ __RPC__deref_out_opt BSTR *retval);
        
        /* [id][propput][funcdescattr] */ HRESULT ( STDMETHODCALLTYPE *put_StartWorkingDirectory )( 
            ProjectConfigurationProperties3 * This,
            /* [in][idldescattr] */ __RPC__in BSTR noname,
            /* [retval][out] */ __RPC__out void *retval);
        
        /* [id][propget][funcdescattr] */ HRESULT ( STDMETHODCALLTYPE *get_StartURL )( 
            ProjectConfigurationProperties3 * This,
            /* [retval][out] */ __RPC__deref_out_opt BSTR *retval);
        
        /* [id][propput][funcdescattr] */ HRESULT ( STDMETHODCALLTYPE *put_StartURL )( 
            ProjectConfigurationProperties3 * This,
            /* [in][idldescattr] */ __RPC__in BSTR noname,
            /* [retval][out] */ __RPC__out void *retval);
        
        /* [id][propget][funcdescattr] */ HRESULT ( STDMETHODCALLTYPE *get_StartPage )( 
            ProjectConfigurationProperties3 * This,
            /* [retval][out] */ __RPC__deref_out_opt BSTR *retval);
        
        /* [id][propput][funcdescattr] */ HRESULT ( STDMETHODCALLTYPE *put_StartPage )( 
            ProjectConfigurationProperties3 * This,
            /* [in][idldescattr] */ __RPC__in BSTR noname,
            /* [retval][out] */ __RPC__out void *retval);
        
        /* [id][propget][funcdescattr] */ HRESULT ( STDMETHODCALLTYPE *get_StartArguments )( 
            ProjectConfigurationProperties3 * This,
            /* [retval][out] */ __RPC__deref_out_opt BSTR *retval);
        
        /* [id][propput][funcdescattr] */ HRESULT ( STDMETHODCALLTYPE *put_StartArguments )( 
            ProjectConfigurationProperties3 * This,
            /* [in][idldescattr] */ __RPC__in BSTR noname,
            /* [retval][out] */ __RPC__out void *retval);
        
        /* [id][propget][funcdescattr] */ HRESULT ( STDMETHODCALLTYPE *get_StartWithIE )( 
            ProjectConfigurationProperties3 * This,
            /* [retval][out] */ __RPC__out BOOLEAN *retval);
        
        /* [id][propput][funcdescattr] */ HRESULT ( STDMETHODCALLTYPE *put_StartWithIE )( 
            ProjectConfigurationProperties3 * This,
            /* [in][idldescattr] */ BOOLEAN noname,
            /* [retval][out] */ __RPC__out void *retval);
        
        /* [id][propget][funcdescattr] */ HRESULT ( STDMETHODCALLTYPE *get_EnableASPDebugging )( 
            ProjectConfigurationProperties3 * This,
            /* [retval][out] */ __RPC__out BOOLEAN *retval);
        
        /* [id][propput][funcdescattr] */ HRESULT ( STDMETHODCALLTYPE *put_EnableASPDebugging )( 
            ProjectConfigurationProperties3 * This,
            /* [in][idldescattr] */ BOOLEAN noname,
            /* [retval][out] */ __RPC__out void *retval);
        
        /* [id][propget][funcdescattr] */ HRESULT ( STDMETHODCALLTYPE *get_EnableASPXDebugging )( 
            ProjectConfigurationProperties3 * This,
            /* [retval][out] */ __RPC__out BOOLEAN *retval);
        
        /* [id][propput][funcdescattr] */ HRESULT ( STDMETHODCALLTYPE *put_EnableASPXDebugging )( 
            ProjectConfigurationProperties3 * This,
            /* [in][idldescattr] */ BOOLEAN noname,
            /* [retval][out] */ __RPC__out void *retval);
        
        /* [id][propget][funcdescattr] */ HRESULT ( STDMETHODCALLTYPE *get_EnableUnmanagedDebugging )( 
            ProjectConfigurationProperties3 * This,
            /* [retval][out] */ __RPC__out BOOLEAN *retval);
        
        /* [id][propput][funcdescattr] */ HRESULT ( STDMETHODCALLTYPE *put_EnableUnmanagedDebugging )( 
            ProjectConfigurationProperties3 * This,
            /* [in][idldescattr] */ BOOLEAN noname,
            /* [retval][out] */ __RPC__out void *retval);
        
        /* [id][propget][funcdescattr] */ HRESULT ( STDMETHODCALLTYPE *get_StartAction )( 
            ProjectConfigurationProperties3 * This,
            /* [retval][out] */ __RPC__out enum prjStartAction *retval);
        
        /* [id][propput][funcdescattr] */ HRESULT ( STDMETHODCALLTYPE *put_StartAction )( 
            ProjectConfigurationProperties3 * This,
            /* [in][idldescattr] */ enum prjStartAction noname,
            /* [retval][out] */ __RPC__out void *retval);
        
        /* [id][propget][funcdescattr] */ HRESULT ( STDMETHODCALLTYPE *get_Extender )( 
            ProjectConfigurationProperties3 * This,
            /* [in][idldescattr] */ __RPC__in BSTR ExtenderName,
            /* [retval][out] */ __RPC__deref_out_opt IDispatch **retval);
        
        /* [id][propget][funcdescattr] */ HRESULT ( STDMETHODCALLTYPE *get_ExtenderNames )( 
            ProjectConfigurationProperties3 * This,
            /* [retval][out] */ __RPC__out VARIANT *retval);
        
        /* [id][propget][funcdescattr] */ HRESULT ( STDMETHODCALLTYPE *get_ExtenderCATID )( 
            ProjectConfigurationProperties3 * This,
            /* [retval][out] */ __RPC__deref_out_opt BSTR *retval);
        
        /* [id][propget][funcdescattr] */ HRESULT ( STDMETHODCALLTYPE *get_WarningLevel )( 
            ProjectConfigurationProperties3 * This,
            /* [retval][out] */ __RPC__out enum prjWarningLevel *retval);
        
        /* [id][propput][funcdescattr] */ HRESULT ( STDMETHODCALLTYPE *put_WarningLevel )( 
            ProjectConfigurationProperties3 * This,
            /* [in][idldescattr] */ enum prjWarningLevel noname,
            /* [retval][out] */ __RPC__out void *retval);
        
        /* [id][propget][funcdescattr] */ HRESULT ( STDMETHODCALLTYPE *get_TreatWarningsAsErrors )( 
            ProjectConfigurationProperties3 * This,
            /* [retval][out] */ __RPC__out BOOLEAN *retval);
        
        /* [id][propput][funcdescattr] */ HRESULT ( STDMETHODCALLTYPE *put_TreatWarningsAsErrors )( 
            ProjectConfigurationProperties3 * This,
            /* [in][idldescattr] */ BOOLEAN noname,
            /* [retval][out] */ __RPC__out void *retval);
        
        /* [id][propget][funcdescattr] */ HRESULT ( STDMETHODCALLTYPE *get_EnableSQLServerDebugging )( 
            ProjectConfigurationProperties3 * This,
            /* [retval][out] */ __RPC__out BOOLEAN *retval);
        
        /* [id][propput][funcdescattr] */ HRESULT ( STDMETHODCALLTYPE *put_EnableSQLServerDebugging )( 
            ProjectConfigurationProperties3 * This,
            /* [in][idldescattr] */ BOOLEAN noname,
            /* [retval][out] */ __RPC__out void *retval);
        
        /* [id][propget][funcdescattr] */ HRESULT ( STDMETHODCALLTYPE *get_FileAlignment )( 
            ProjectConfigurationProperties3 * This,
            /* [retval][out] */ __RPC__out unsigned long *retval);
        
        /* [id][propput][funcdescattr] */ HRESULT ( STDMETHODCALLTYPE *put_FileAlignment )( 
            ProjectConfigurationProperties3 * This,
            /* [in][idldescattr] */ unsigned long noname,
            /* [retval][out] */ __RPC__out void *retval);
        
        /* [id][propget][funcdescattr] */ HRESULT ( STDMETHODCALLTYPE *get_RegisterForComInterop )( 
            ProjectConfigurationProperties3 * This,
            /* [retval][out] */ __RPC__out BOOLEAN *retval);
        
        /* [id][propput][funcdescattr] */ HRESULT ( STDMETHODCALLTYPE *put_RegisterForComInterop )( 
            ProjectConfigurationProperties3 * This,
            /* [in][idldescattr] */ BOOLEAN noname,
            /* [retval][out] */ __RPC__out void *retval);
        
        /* [id][propget][funcdescattr] */ HRESULT ( STDMETHODCALLTYPE *get_ConfigurationOverrideFile )( 
            ProjectConfigurationProperties3 * This,
            /* [retval][out] */ __RPC__deref_out_opt BSTR *retval);
        
        /* [id][propput][funcdescattr] */ HRESULT ( STDMETHODCALLTYPE *put_ConfigurationOverrideFile )( 
            ProjectConfigurationProperties3 * This,
            /* [in][idldescattr] */ __RPC__in BSTR noname,
            /* [retval][out] */ __RPC__out void *retval);
        
        /* [id][propget][funcdescattr] */ HRESULT ( STDMETHODCALLTYPE *get_RemoteDebugEnabled )( 
            ProjectConfigurationProperties3 * This,
            /* [retval][out] */ __RPC__out BOOLEAN *retval);
        
        /* [id][propput][funcdescattr] */ HRESULT ( STDMETHODCALLTYPE *put_RemoteDebugEnabled )( 
            ProjectConfigurationProperties3 * This,
            /* [in][idldescattr] */ BOOLEAN noname,
            /* [retval][out] */ __RPC__out void *retval);
        
        /* [id][propget][funcdescattr] */ HRESULT ( STDMETHODCALLTYPE *get_RemoteDebugMachine )( 
            ProjectConfigurationProperties3 * This,
            /* [retval][out] */ __RPC__deref_out_opt BSTR *retval);
        
        /* [id][propput][funcdescattr] */ HRESULT ( STDMETHODCALLTYPE *put_RemoteDebugMachine )( 
            ProjectConfigurationProperties3 * This,
            /* [in][idldescattr] */ __RPC__in BSTR noname,
            /* [retval][out] */ __RPC__out void *retval);
        
        /* [id][propget][funcdescattr] */ HRESULT ( STDMETHODCALLTYPE *get_NoWarn )( 
            ProjectConfigurationProperties3 * This,
            /* [retval][out] */ __RPC__deref_out_opt BSTR *retval);
        
        /* [id][propput][funcdescattr] */ HRESULT ( STDMETHODCALLTYPE *put_NoWarn )( 
            ProjectConfigurationProperties3 * This,
            /* [in][idldescattr] */ __RPC__in BSTR noname,
            /* [retval][out] */ __RPC__out void *retval);
        
        /* [id][propget][funcdescattr] */ HRESULT ( STDMETHODCALLTYPE *get_NoStdLib )( 
            ProjectConfigurationProperties3 * This,
            /* [retval][out] */ __RPC__out BOOLEAN *retval);
        
        /* [id][propput][funcdescattr] */ HRESULT ( STDMETHODCALLTYPE *put_NoStdLib )( 
            ProjectConfigurationProperties3 * This,
            /* [in][idldescattr] */ BOOLEAN noname,
            /* [retval][out] */ __RPC__out void *retval);
        
        /* [helpstring][id][propget] */ HRESULT ( STDMETHODCALLTYPE *get_DebugInfo )( 
            ProjectConfigurationProperties3 * This,
            /* [retval][out] */ __RPC__deref_out_opt BSTR *pbstrDebugInfo);
        
        /* [helpstring][id][propput] */ HRESULT ( STDMETHODCALLTYPE *put_DebugInfo )( 
            ProjectConfigurationProperties3 * This,
            /* [in] */ __RPC__in BSTR DebugInfo);
        
        /* [helpstring][id][propget] */ HRESULT ( STDMETHODCALLTYPE *get_PlatformTarget )( 
            ProjectConfigurationProperties3 * This,
            /* [retval][out] */ __RPC__deref_out_opt BSTR *pbstrPlatformTarget);
        
        /* [helpstring][id][propput] */ HRESULT ( STDMETHODCALLTYPE *put_PlatformTarget )( 
            ProjectConfigurationProperties3 * This,
            /* [in] */ __RPC__in BSTR PlatformTarget);
        
        /* [helpstring][id][propget] */ HRESULT ( STDMETHODCALLTYPE *get_TreatSpecificWarningsAsErrors )( 
            ProjectConfigurationProperties3 * This,
            /* [retval][out] */ __RPC__deref_out_opt BSTR *pbstrWarningsAsErrors);
        
        /* [helpstring][id][propput] */ HRESULT ( STDMETHODCALLTYPE *put_TreatSpecificWarningsAsErrors )( 
            ProjectConfigurationProperties3 * This,
            /* [in] */ __RPC__in BSTR WarningsAsErrors);
        
        /* [helpstring][id][propget] */ HRESULT ( STDMETHODCALLTYPE *get_RunCodeAnalysis )( 
            ProjectConfigurationProperties3 * This,
            /* [retval][out] */ __RPC__out VARIANT_BOOL *pbRunCodeAnalysis);
        
        /* [helpstring][id][propput] */ HRESULT ( STDMETHODCALLTYPE *put_RunCodeAnalysis )( 
            ProjectConfigurationProperties3 * This,
            /* [in] */ VARIANT_BOOL RunCodeAnalysis);
        
        /* [helpstring][id][propget] */ HRESULT ( STDMETHODCALLTYPE *get_CodeAnalysisLogFile )( 
            ProjectConfigurationProperties3 * This,
            /* [retval][out] */ __RPC__deref_out_opt BSTR *pbstrCodeAnalysisLogFile);
        
        /* [helpstring][id][propput] */ HRESULT ( STDMETHODCALLTYPE *put_CodeAnalysisLogFile )( 
            ProjectConfigurationProperties3 * This,
            /* [in] */ __RPC__in BSTR CodeAnalysisLogFile);
        
        /* [helpstring][id][propget] */ HRESULT ( STDMETHODCALLTYPE *get_CodeAnalysisRuleAssemblies )( 
            ProjectConfigurationProperties3 * This,
            /* [retval][out] */ __RPC__deref_out_opt BSTR *pbstrCodeAnalysisRuleAssemblies);
        
        /* [helpstring][id][propput] */ HRESULT ( STDMETHODCALLTYPE *put_CodeAnalysisRuleAssemblies )( 
            ProjectConfigurationProperties3 * This,
            /* [in] */ __RPC__in BSTR CodeAnalysisRuleAssemblies);
        
        /* [helpstring][id][propget] */ HRESULT ( STDMETHODCALLTYPE *get_CodeAnalysisInputAssembly )( 
            ProjectConfigurationProperties3 * This,
            /* [retval][out] */ __RPC__deref_out_opt BSTR *pbstrCodeAnalysisInputAssembly);
        
        /* [helpstring][id][propput] */ HRESULT ( STDMETHODCALLTYPE *put_CodeAnalysisInputAssembly )( 
            ProjectConfigurationProperties3 * This,
            /* [in] */ __RPC__in BSTR CodeAnalysisInputAssembly);
        
        /* [helpstring][id][propget] */ HRESULT ( STDMETHODCALLTYPE *get_CodeAnalysisRules )( 
            ProjectConfigurationProperties3 * This,
            /* [retval][out] */ __RPC__deref_out_opt BSTR *pbstrCodeAnalysisRules);
        
        /* [helpstring][id][propput] */ HRESULT ( STDMETHODCALLTYPE *put_CodeAnalysisRules )( 
            ProjectConfigurationProperties3 * This,
            /* [in] */ __RPC__in BSTR CodeAnalysisRules);
        
        /* [helpstring][id][propget] */ HRESULT ( STDMETHODCALLTYPE *get_CodeAnalysisSpellCheckLanguages )( 
            ProjectConfigurationProperties3 * This,
            /* [retval][out] */ __RPC__deref_out_opt BSTR *pbstrCodeAnalysisSpellCheckLanguages);
        
        /* [helpstring][id][propput] */ HRESULT ( STDMETHODCALLTYPE *put_CodeAnalysisSpellCheckLanguages )( 
            ProjectConfigurationProperties3 * This,
            /* [in] */ __RPC__in BSTR CodeAnalysisSpellCheckLanguages);
        
        /* [helpstring][id][propget] */ HRESULT ( STDMETHODCALLTYPE *get_CodeAnalysisUseTypeNameInSuppression )( 
            ProjectConfigurationProperties3 * This,
            /* [retval][out] */ __RPC__out VARIANT_BOOL *bUseTypeNameInSuppression);
        
        /* [helpstring][id][propput] */ HRESULT ( STDMETHODCALLTYPE *put_CodeAnalysisUseTypeNameInSuppression )( 
            ProjectConfigurationProperties3 * This,
            /* [in] */ VARIANT_BOOL UseTypeNameInSuppression);
        
        /* [helpstring][id][propget] */ HRESULT ( STDMETHODCALLTYPE *get_CodeAnalysisModuleSuppressionsFile )( 
            ProjectConfigurationProperties3 * This,
            /* [retval][out] */ __RPC__deref_out_opt BSTR *pbstrCodeAnalysisModuleSuppressionsFile);
        
        /* [helpstring][id][propput] */ HRESULT ( STDMETHODCALLTYPE *put_CodeAnalysisModuleSuppressionsFile )( 
            ProjectConfigurationProperties3 * This,
            /* [in] */ __RPC__in BSTR CodeAnalysisModuleSuppressionsFile);
        
        /* [helpstring][id][propget] */ HRESULT ( STDMETHODCALLTYPE *get_UseVSHostingProcess )( 
            ProjectConfigurationProperties3 * This,
            /* [retval][out] */ __RPC__out VARIANT_BOOL *pbUseVSHostingProcess);
        
        /* [helpstring][id][propput] */ HRESULT ( STDMETHODCALLTYPE *put_UseVSHostingProcess )( 
            ProjectConfigurationProperties3 * This,
            /* [in] */ VARIANT_BOOL UseVSHostingProcess);
        
        /* [helpstring][id][propget] */ HRESULT ( STDMETHODCALLTYPE *get_GenerateSerializationAssemblies )( 
            ProjectConfigurationProperties3 * This,
            /* [retval][out] */ __RPC__out sgenGenerationOption *pSgenGenerationOption);
        
        /* [helpstring][id][propput] */ HRESULT ( STDMETHODCALLTYPE *put_GenerateSerializationAssemblies )( 
            ProjectConfigurationProperties3 * This,
            /* [in] */ sgenGenerationOption SgenGenerationOption);
        
        END_INTERFACE
    } ProjectConfigurationProperties3Vtbl;

    interface ProjectConfigurationProperties3
    {
        CONST_VTBL struct ProjectConfigurationProperties3Vtbl *lpVtbl;
    };

    

#ifdef COBJMACROS


#define ProjectConfigurationProperties3_QueryInterface(This,riid,ppvObj,retval)	\
    ( (This)->lpVtbl -> QueryInterface(This,riid,ppvObj,retval) ) 

#define ProjectConfigurationProperties3_AddRef(This,retval)	\
    ( (This)->lpVtbl -> AddRef(This,retval) ) 

#define ProjectConfigurationProperties3_Release(This,retval)	\
    ( (This)->lpVtbl -> Release(This,retval) ) 

#define ProjectConfigurationProperties3_GetTypeInfoCount(This,pctinfo,retval)	\
    ( (This)->lpVtbl -> GetTypeInfoCount(This,pctinfo,retval) ) 

#define ProjectConfigurationProperties3_GetTypeInfo(This,itinfo,lcid,pptinfo,retval)	\
    ( (This)->lpVtbl -> GetTypeInfo(This,itinfo,lcid,pptinfo,retval) ) 

#define ProjectConfigurationProperties3_GetIDsOfNames(This,riid,rgszNames,cNames,lcid,rgdispid,retval)	\
    ( (This)->lpVtbl -> GetIDsOfNames(This,riid,rgszNames,cNames,lcid,rgdispid,retval) ) 

#define ProjectConfigurationProperties3_Invoke(This,dispidMember,riid,lcid,wFlags,pdispparams,pvarResult,pexcepinfo,puArgErr,retval)	\
    ( (This)->lpVtbl -> Invoke(This,dispidMember,riid,lcid,wFlags,pdispparams,pvarResult,pexcepinfo,puArgErr,retval) ) 

#define ProjectConfigurationProperties3_get___id(This,retval)	\
    ( (This)->lpVtbl -> get___id(This,retval) ) 

#define ProjectConfigurationProperties3_get_DebugSymbols(This,retval)	\
    ( (This)->lpVtbl -> get_DebugSymbols(This,retval) ) 

#define ProjectConfigurationProperties3_put_DebugSymbols(This,noname,retval)	\
    ( (This)->lpVtbl -> put_DebugSymbols(This,noname,retval) ) 

#define ProjectConfigurationProperties3_get_DefineDebug(This,retval)	\
    ( (This)->lpVtbl -> get_DefineDebug(This,retval) ) 

#define ProjectConfigurationProperties3_put_DefineDebug(This,noname,retval)	\
    ( (This)->lpVtbl -> put_DefineDebug(This,noname,retval) ) 

#define ProjectConfigurationProperties3_get_DefineTrace(This,retval)	\
    ( (This)->lpVtbl -> get_DefineTrace(This,retval) ) 

#define ProjectConfigurationProperties3_put_DefineTrace(This,noname,retval)	\
    ( (This)->lpVtbl -> put_DefineTrace(This,noname,retval) ) 

#define ProjectConfigurationProperties3_get_OutputPath(This,retval)	\
    ( (This)->lpVtbl -> get_OutputPath(This,retval) ) 

#define ProjectConfigurationProperties3_put_OutputPath(This,noname,retval)	\
    ( (This)->lpVtbl -> put_OutputPath(This,noname,retval) ) 

#define ProjectConfigurationProperties3_get_IntermediatePath(This,retval)	\
    ( (This)->lpVtbl -> get_IntermediatePath(This,retval) ) 

#define ProjectConfigurationProperties3_put_IntermediatePath(This,noname,retval)	\
    ( (This)->lpVtbl -> put_IntermediatePath(This,noname,retval) ) 

#define ProjectConfigurationProperties3_get_DefineConstants(This,retval)	\
    ( (This)->lpVtbl -> get_DefineConstants(This,retval) ) 

#define ProjectConfigurationProperties3_put_DefineConstants(This,noname,retval)	\
    ( (This)->lpVtbl -> put_DefineConstants(This,noname,retval) ) 

#define ProjectConfigurationProperties3_get_RemoveIntegerChecks(This,retval)	\
    ( (This)->lpVtbl -> get_RemoveIntegerChecks(This,retval) ) 

#define ProjectConfigurationProperties3_put_RemoveIntegerChecks(This,noname,retval)	\
    ( (This)->lpVtbl -> put_RemoveIntegerChecks(This,noname,retval) ) 

#define ProjectConfigurationProperties3_get_BaseAddress(This,retval)	\
    ( (This)->lpVtbl -> get_BaseAddress(This,retval) ) 

#define ProjectConfigurationProperties3_put_BaseAddress(This,noname,retval)	\
    ( (This)->lpVtbl -> put_BaseAddress(This,noname,retval) ) 

#define ProjectConfigurationProperties3_get_AllowUnsafeBlocks(This,retval)	\
    ( (This)->lpVtbl -> get_AllowUnsafeBlocks(This,retval) ) 

#define ProjectConfigurationProperties3_put_AllowUnsafeBlocks(This,noname,retval)	\
    ( (This)->lpVtbl -> put_AllowUnsafeBlocks(This,noname,retval) ) 

#define ProjectConfigurationProperties3_get_CheckForOverflowUnderflow(This,retval)	\
    ( (This)->lpVtbl -> get_CheckForOverflowUnderflow(This,retval) ) 

#define ProjectConfigurationProperties3_put_CheckForOverflowUnderflow(This,noname,retval)	\
    ( (This)->lpVtbl -> put_CheckForOverflowUnderflow(This,noname,retval) ) 

#define ProjectConfigurationProperties3_get_DocumentationFile(This,retval)	\
    ( (This)->lpVtbl -> get_DocumentationFile(This,retval) ) 

#define ProjectConfigurationProperties3_put_DocumentationFile(This,noname,retval)	\
    ( (This)->lpVtbl -> put_DocumentationFile(This,noname,retval) ) 

#define ProjectConfigurationProperties3_get_Optimize(This,retval)	\
    ( (This)->lpVtbl -> get_Optimize(This,retval) ) 

#define ProjectConfigurationProperties3_put_Optimize(This,noname,retval)	\
    ( (This)->lpVtbl -> put_Optimize(This,noname,retval) ) 

#define ProjectConfigurationProperties3_get_IncrementalBuild(This,retval)	\
    ( (This)->lpVtbl -> get_IncrementalBuild(This,retval) ) 

#define ProjectConfigurationProperties3_put_IncrementalBuild(This,noname,retval)	\
    ( (This)->lpVtbl -> put_IncrementalBuild(This,noname,retval) ) 

#define ProjectConfigurationProperties3_get_StartProgram(This,retval)	\
    ( (This)->lpVtbl -> get_StartProgram(This,retval) ) 

#define ProjectConfigurationProperties3_put_StartProgram(This,noname,retval)	\
    ( (This)->lpVtbl -> put_StartProgram(This,noname,retval) ) 

#define ProjectConfigurationProperties3_get_StartWorkingDirectory(This,retval)	\
    ( (This)->lpVtbl -> get_StartWorkingDirectory(This,retval) ) 

#define ProjectConfigurationProperties3_put_StartWorkingDirectory(This,noname,retval)	\
    ( (This)->lpVtbl -> put_StartWorkingDirectory(This,noname,retval) ) 

#define ProjectConfigurationProperties3_get_StartURL(This,retval)	\
    ( (This)->lpVtbl -> get_StartURL(This,retval) ) 

#define ProjectConfigurationProperties3_put_StartURL(This,noname,retval)	\
    ( (This)->lpVtbl -> put_StartURL(This,noname,retval) ) 

#define ProjectConfigurationProperties3_get_StartPage(This,retval)	\
    ( (This)->lpVtbl -> get_StartPage(This,retval) ) 

#define ProjectConfigurationProperties3_put_StartPage(This,noname,retval)	\
    ( (This)->lpVtbl -> put_StartPage(This,noname,retval) ) 

#define ProjectConfigurationProperties3_get_StartArguments(This,retval)	\
    ( (This)->lpVtbl -> get_StartArguments(This,retval) ) 

#define ProjectConfigurationProperties3_put_StartArguments(This,noname,retval)	\
    ( (This)->lpVtbl -> put_StartArguments(This,noname,retval) ) 

#define ProjectConfigurationProperties3_get_StartWithIE(This,retval)	\
    ( (This)->lpVtbl -> get_StartWithIE(This,retval) ) 

#define ProjectConfigurationProperties3_put_StartWithIE(This,noname,retval)	\
    ( (This)->lpVtbl -> put_StartWithIE(This,noname,retval) ) 

#define ProjectConfigurationProperties3_get_EnableASPDebugging(This,retval)	\
    ( (This)->lpVtbl -> get_EnableASPDebugging(This,retval) ) 

#define ProjectConfigurationProperties3_put_EnableASPDebugging(This,noname,retval)	\
    ( (This)->lpVtbl -> put_EnableASPDebugging(This,noname,retval) ) 

#define ProjectConfigurationProperties3_get_EnableASPXDebugging(This,retval)	\
    ( (This)->lpVtbl -> get_EnableASPXDebugging(This,retval) ) 

#define ProjectConfigurationProperties3_put_EnableASPXDebugging(This,noname,retval)	\
    ( (This)->lpVtbl -> put_EnableASPXDebugging(This,noname,retval) ) 

#define ProjectConfigurationProperties3_get_EnableUnmanagedDebugging(This,retval)	\
    ( (This)->lpVtbl -> get_EnableUnmanagedDebugging(This,retval) ) 

#define ProjectConfigurationProperties3_put_EnableUnmanagedDebugging(This,noname,retval)	\
    ( (This)->lpVtbl -> put_EnableUnmanagedDebugging(This,noname,retval) ) 

#define ProjectConfigurationProperties3_get_StartAction(This,retval)	\
    ( (This)->lpVtbl -> get_StartAction(This,retval) ) 

#define ProjectConfigurationProperties3_put_StartAction(This,noname,retval)	\
    ( (This)->lpVtbl -> put_StartAction(This,noname,retval) ) 

#define ProjectConfigurationProperties3_get_Extender(This,ExtenderName,retval)	\
    ( (This)->lpVtbl -> get_Extender(This,ExtenderName,retval) ) 

#define ProjectConfigurationProperties3_get_ExtenderNames(This,retval)	\
    ( (This)->lpVtbl -> get_ExtenderNames(This,retval) ) 

#define ProjectConfigurationProperties3_get_ExtenderCATID(This,retval)	\
    ( (This)->lpVtbl -> get_ExtenderCATID(This,retval) ) 

#define ProjectConfigurationProperties3_get_WarningLevel(This,retval)	\
    ( (This)->lpVtbl -> get_WarningLevel(This,retval) ) 

#define ProjectConfigurationProperties3_put_WarningLevel(This,noname,retval)	\
    ( (This)->lpVtbl -> put_WarningLevel(This,noname,retval) ) 

#define ProjectConfigurationProperties3_get_TreatWarningsAsErrors(This,retval)	\
    ( (This)->lpVtbl -> get_TreatWarningsAsErrors(This,retval) ) 

#define ProjectConfigurationProperties3_put_TreatWarningsAsErrors(This,noname,retval)	\
    ( (This)->lpVtbl -> put_TreatWarningsAsErrors(This,noname,retval) ) 

#define ProjectConfigurationProperties3_get_EnableSQLServerDebugging(This,retval)	\
    ( (This)->lpVtbl -> get_EnableSQLServerDebugging(This,retval) ) 

#define ProjectConfigurationProperties3_put_EnableSQLServerDebugging(This,noname,retval)	\
    ( (This)->lpVtbl -> put_EnableSQLServerDebugging(This,noname,retval) ) 

#define ProjectConfigurationProperties3_get_FileAlignment(This,retval)	\
    ( (This)->lpVtbl -> get_FileAlignment(This,retval) ) 

#define ProjectConfigurationProperties3_put_FileAlignment(This,noname,retval)	\
    ( (This)->lpVtbl -> put_FileAlignment(This,noname,retval) ) 

#define ProjectConfigurationProperties3_get_RegisterForComInterop(This,retval)	\
    ( (This)->lpVtbl -> get_RegisterForComInterop(This,retval) ) 

#define ProjectConfigurationProperties3_put_RegisterForComInterop(This,noname,retval)	\
    ( (This)->lpVtbl -> put_RegisterForComInterop(This,noname,retval) ) 

#define ProjectConfigurationProperties3_get_ConfigurationOverrideFile(This,retval)	\
    ( (This)->lpVtbl -> get_ConfigurationOverrideFile(This,retval) ) 

#define ProjectConfigurationProperties3_put_ConfigurationOverrideFile(This,noname,retval)	\
    ( (This)->lpVtbl -> put_ConfigurationOverrideFile(This,noname,retval) ) 

#define ProjectConfigurationProperties3_get_RemoteDebugEnabled(This,retval)	\
    ( (This)->lpVtbl -> get_RemoteDebugEnabled(This,retval) ) 

#define ProjectConfigurationProperties3_put_RemoteDebugEnabled(This,noname,retval)	\
    ( (This)->lpVtbl -> put_RemoteDebugEnabled(This,noname,retval) ) 

#define ProjectConfigurationProperties3_get_RemoteDebugMachine(This,retval)	\
    ( (This)->lpVtbl -> get_RemoteDebugMachine(This,retval) ) 

#define ProjectConfigurationProperties3_put_RemoteDebugMachine(This,noname,retval)	\
    ( (This)->lpVtbl -> put_RemoteDebugMachine(This,noname,retval) ) 

#define ProjectConfigurationProperties3_get_NoWarn(This,retval)	\
    ( (This)->lpVtbl -> get_NoWarn(This,retval) ) 

#define ProjectConfigurationProperties3_put_NoWarn(This,noname,retval)	\
    ( (This)->lpVtbl -> put_NoWarn(This,noname,retval) ) 

#define ProjectConfigurationProperties3_get_NoStdLib(This,retval)	\
    ( (This)->lpVtbl -> get_NoStdLib(This,retval) ) 

#define ProjectConfigurationProperties3_put_NoStdLib(This,noname,retval)	\
    ( (This)->lpVtbl -> put_NoStdLib(This,noname,retval) ) 


#define ProjectConfigurationProperties3_get_DebugInfo(This,pbstrDebugInfo)	\
    ( (This)->lpVtbl -> get_DebugInfo(This,pbstrDebugInfo) ) 

#define ProjectConfigurationProperties3_put_DebugInfo(This,DebugInfo)	\
    ( (This)->lpVtbl -> put_DebugInfo(This,DebugInfo) ) 

#define ProjectConfigurationProperties3_get_PlatformTarget(This,pbstrPlatformTarget)	\
    ( (This)->lpVtbl -> get_PlatformTarget(This,pbstrPlatformTarget) ) 

#define ProjectConfigurationProperties3_put_PlatformTarget(This,PlatformTarget)	\
    ( (This)->lpVtbl -> put_PlatformTarget(This,PlatformTarget) ) 

#define ProjectConfigurationProperties3_get_TreatSpecificWarningsAsErrors(This,pbstrWarningsAsErrors)	\
    ( (This)->lpVtbl -> get_TreatSpecificWarningsAsErrors(This,pbstrWarningsAsErrors) ) 

#define ProjectConfigurationProperties3_put_TreatSpecificWarningsAsErrors(This,WarningsAsErrors)	\
    ( (This)->lpVtbl -> put_TreatSpecificWarningsAsErrors(This,WarningsAsErrors) ) 

#define ProjectConfigurationProperties3_get_RunCodeAnalysis(This,pbRunCodeAnalysis)	\
    ( (This)->lpVtbl -> get_RunCodeAnalysis(This,pbRunCodeAnalysis) ) 

#define ProjectConfigurationProperties3_put_RunCodeAnalysis(This,RunCodeAnalysis)	\
    ( (This)->lpVtbl -> put_RunCodeAnalysis(This,RunCodeAnalysis) ) 

#define ProjectConfigurationProperties3_get_CodeAnalysisLogFile(This,pbstrCodeAnalysisLogFile)	\
    ( (This)->lpVtbl -> get_CodeAnalysisLogFile(This,pbstrCodeAnalysisLogFile) ) 

#define ProjectConfigurationProperties3_put_CodeAnalysisLogFile(This,CodeAnalysisLogFile)	\
    ( (This)->lpVtbl -> put_CodeAnalysisLogFile(This,CodeAnalysisLogFile) ) 

#define ProjectConfigurationProperties3_get_CodeAnalysisRuleAssemblies(This,pbstrCodeAnalysisRuleAssemblies)	\
    ( (This)->lpVtbl -> get_CodeAnalysisRuleAssemblies(This,pbstrCodeAnalysisRuleAssemblies) ) 

#define ProjectConfigurationProperties3_put_CodeAnalysisRuleAssemblies(This,CodeAnalysisRuleAssemblies)	\
    ( (This)->lpVtbl -> put_CodeAnalysisRuleAssemblies(This,CodeAnalysisRuleAssemblies) ) 

#define ProjectConfigurationProperties3_get_CodeAnalysisInputAssembly(This,pbstrCodeAnalysisInputAssembly)	\
    ( (This)->lpVtbl -> get_CodeAnalysisInputAssembly(This,pbstrCodeAnalysisInputAssembly) ) 

#define ProjectConfigurationProperties3_put_CodeAnalysisInputAssembly(This,CodeAnalysisInputAssembly)	\
    ( (This)->lpVtbl -> put_CodeAnalysisInputAssembly(This,CodeAnalysisInputAssembly) ) 

#define ProjectConfigurationProperties3_get_CodeAnalysisRules(This,pbstrCodeAnalysisRules)	\
    ( (This)->lpVtbl -> get_CodeAnalysisRules(This,pbstrCodeAnalysisRules) ) 

#define ProjectConfigurationProperties3_put_CodeAnalysisRules(This,CodeAnalysisRules)	\
    ( (This)->lpVtbl -> put_CodeAnalysisRules(This,CodeAnalysisRules) ) 

#define ProjectConfigurationProperties3_get_CodeAnalysisSpellCheckLanguages(This,pbstrCodeAnalysisSpellCheckLanguages)	\
    ( (This)->lpVtbl -> get_CodeAnalysisSpellCheckLanguages(This,pbstrCodeAnalysisSpellCheckLanguages) ) 

#define ProjectConfigurationProperties3_put_CodeAnalysisSpellCheckLanguages(This,CodeAnalysisSpellCheckLanguages)	\
    ( (This)->lpVtbl -> put_CodeAnalysisSpellCheckLanguages(This,CodeAnalysisSpellCheckLanguages) ) 

#define ProjectConfigurationProperties3_get_CodeAnalysisUseTypeNameInSuppression(This,bUseTypeNameInSuppression)	\
    ( (This)->lpVtbl -> get_CodeAnalysisUseTypeNameInSuppression(This,bUseTypeNameInSuppression) ) 

#define ProjectConfigurationProperties3_put_CodeAnalysisUseTypeNameInSuppression(This,UseTypeNameInSuppression)	\
    ( (This)->lpVtbl -> put_CodeAnalysisUseTypeNameInSuppression(This,UseTypeNameInSuppression) ) 

#define ProjectConfigurationProperties3_get_CodeAnalysisModuleSuppressionsFile(This,pbstrCodeAnalysisModuleSuppressionsFile)	\
    ( (This)->lpVtbl -> get_CodeAnalysisModuleSuppressionsFile(This,pbstrCodeAnalysisModuleSuppressionsFile) ) 

#define ProjectConfigurationProperties3_put_CodeAnalysisModuleSuppressionsFile(This,CodeAnalysisModuleSuppressionsFile)	\
    ( (This)->lpVtbl -> put_CodeAnalysisModuleSuppressionsFile(This,CodeAnalysisModuleSuppressionsFile) ) 

#define ProjectConfigurationProperties3_get_UseVSHostingProcess(This,pbUseVSHostingProcess)	\
    ( (This)->lpVtbl -> get_UseVSHostingProcess(This,pbUseVSHostingProcess) ) 

#define ProjectConfigurationProperties3_put_UseVSHostingProcess(This,UseVSHostingProcess)	\
    ( (This)->lpVtbl -> put_UseVSHostingProcess(This,UseVSHostingProcess) ) 

#define ProjectConfigurationProperties3_get_GenerateSerializationAssemblies(This,pSgenGenerationOption)	\
    ( (This)->lpVtbl -> get_GenerateSerializationAssemblies(This,pSgenGenerationOption) ) 

#define ProjectConfigurationProperties3_put_GenerateSerializationAssemblies(This,SgenGenerationOption)	\
    ( (This)->lpVtbl -> put_GenerateSerializationAssemblies(This,SgenGenerationOption) ) 

#endif /* COBJMACROS */


#endif 	/* C style interface */




#endif 	/* __ProjectConfigurationProperties3_INTERFACE_DEFINED__ */


#ifndef ___VSLangProjWebReferencesEvents_INTERFACE_DEFINED__
#define ___VSLangProjWebReferencesEvents_INTERFACE_DEFINED__

/* interface _VSLangProjWebReferencesEvents */
/* [object][dual][uuid] */ 


EXTERN_C const IID IID__VSLangProjWebReferencesEvents;

#if defined(__cplusplus) && !defined(CINTERFACE)
    
    MIDL_INTERFACE("33BD7FEF-EEB4-412a-A4C1-9FBFF6F57067")
    _VSLangProjWebReferencesEvents : public IDispatch
    {
    public:
    };
    
#else 	/* C style interface */

    typedef struct _VSLangProjWebReferencesEventsVtbl
    {
        BEGIN_INTERFACE
        
        HRESULT ( STDMETHODCALLTYPE *QueryInterface )( 
            _VSLangProjWebReferencesEvents * This,
            /* [in] */ __RPC__in REFIID riid,
            /* [iid_is][out] */ 
            __RPC__deref_out  void **ppvObject);
        
        ULONG ( STDMETHODCALLTYPE *AddRef )( 
            _VSLangProjWebReferencesEvents * This);
        
        ULONG ( STDMETHODCALLTYPE *Release )( 
            _VSLangProjWebReferencesEvents * This);
        
        HRESULT ( STDMETHODCALLTYPE *GetTypeInfoCount )( 
            _VSLangProjWebReferencesEvents * This,
            /* [out] */ __RPC__out UINT *pctinfo);
        
        HRESULT ( STDMETHODCALLTYPE *GetTypeInfo )( 
            _VSLangProjWebReferencesEvents * This,
            /* [in] */ UINT iTInfo,
            /* [in] */ LCID lcid,
            /* [out] */ __RPC__deref_out_opt ITypeInfo **ppTInfo);
        
        HRESULT ( STDMETHODCALLTYPE *GetIDsOfNames )( 
            _VSLangProjWebReferencesEvents * This,
            /* [in] */ __RPC__in REFIID riid,
            /* [size_is][in] */ __RPC__in_ecount_full(cNames) LPOLESTR *rgszNames,
            /* [range][in] */ UINT cNames,
            /* [in] */ LCID lcid,
            /* [size_is][out] */ __RPC__out_ecount_full(cNames) DISPID *rgDispId);
        
        /* [local] */ HRESULT ( STDMETHODCALLTYPE *Invoke )( 
            _VSLangProjWebReferencesEvents * This,
            /* [in] */ DISPID dispIdMember,
            /* [in] */ REFIID riid,
            /* [in] */ LCID lcid,
            /* [in] */ WORD wFlags,
            /* [out][in] */ DISPPARAMS *pDispParams,
            /* [out] */ VARIANT *pVarResult,
            /* [out] */ EXCEPINFO *pExcepInfo,
            /* [out] */ UINT *puArgErr);
        
        END_INTERFACE
    } _VSLangProjWebReferencesEventsVtbl;

    interface _VSLangProjWebReferencesEvents
    {
        CONST_VTBL struct _VSLangProjWebReferencesEventsVtbl *lpVtbl;
    };

    

#ifdef COBJMACROS


#define _VSLangProjWebReferencesEvents_QueryInterface(This,riid,ppvObject)	\
    ( (This)->lpVtbl -> QueryInterface(This,riid,ppvObject) ) 

#define _VSLangProjWebReferencesEvents_AddRef(This)	\
    ( (This)->lpVtbl -> AddRef(This) ) 

#define _VSLangProjWebReferencesEvents_Release(This)	\
    ( (This)->lpVtbl -> Release(This) ) 


#define _VSLangProjWebReferencesEvents_GetTypeInfoCount(This,pctinfo)	\
    ( (This)->lpVtbl -> GetTypeInfoCount(This,pctinfo) ) 

#define _VSLangProjWebReferencesEvents_GetTypeInfo(This,iTInfo,lcid,ppTInfo)	\
    ( (This)->lpVtbl -> GetTypeInfo(This,iTInfo,lcid,ppTInfo) ) 

#define _VSLangProjWebReferencesEvents_GetIDsOfNames(This,riid,rgszNames,cNames,lcid,rgDispId)	\
    ( (This)->lpVtbl -> GetIDsOfNames(This,riid,rgszNames,cNames,lcid,rgDispId) ) 

#define _VSLangProjWebReferencesEvents_Invoke(This,dispIdMember,riid,lcid,wFlags,pDispParams,pVarResult,pExcepInfo,puArgErr)	\
    ( (This)->lpVtbl -> Invoke(This,dispIdMember,riid,lcid,wFlags,pDispParams,pVarResult,pExcepInfo,puArgErr) ) 


#endif /* COBJMACROS */


#endif 	/* C style interface */




#endif 	/* ___VSLangProjWebReferencesEvents_INTERFACE_DEFINED__ */


#ifndef ___dispVSLangProjWebReferencesEvents_DISPINTERFACE_DEFINED__
#define ___dispVSLangProjWebReferencesEvents_DISPINTERFACE_DEFINED__

/* dispinterface _dispVSLangProjWebReferencesEvents */
/* [uuid] */ 


EXTERN_C const IID DIID__dispVSLangProjWebReferencesEvents;

#if defined(__cplusplus) && !defined(CINTERFACE)

    MIDL_INTERFACE("9AAD53A9-32CB-4d84-97A7-B4AFA94F5878")
    _dispVSLangProjWebReferencesEvents : public IDispatch
    {
    };
    
#else 	/* C style interface */

    typedef struct _dispVSLangProjWebReferencesEventsVtbl
    {
        BEGIN_INTERFACE
        
        HRESULT ( STDMETHODCALLTYPE *QueryInterface )( 
            _dispVSLangProjWebReferencesEvents * This,
            /* [in] */ __RPC__in REFIID riid,
            /* [iid_is][out] */ 
            __RPC__deref_out  void **ppvObject);
        
        ULONG ( STDMETHODCALLTYPE *AddRef )( 
            _dispVSLangProjWebReferencesEvents * This);
        
        ULONG ( STDMETHODCALLTYPE *Release )( 
            _dispVSLangProjWebReferencesEvents * This);
        
        HRESULT ( STDMETHODCALLTYPE *GetTypeInfoCount )( 
            _dispVSLangProjWebReferencesEvents * This,
            /* [out] */ __RPC__out UINT *pctinfo);
        
        HRESULT ( STDMETHODCALLTYPE *GetTypeInfo )( 
            _dispVSLangProjWebReferencesEvents * This,
            /* [in] */ UINT iTInfo,
            /* [in] */ LCID lcid,
            /* [out] */ __RPC__deref_out_opt ITypeInfo **ppTInfo);
        
        HRESULT ( STDMETHODCALLTYPE *GetIDsOfNames )( 
            _dispVSLangProjWebReferencesEvents * This,
            /* [in] */ __RPC__in REFIID riid,
            /* [size_is][in] */ __RPC__in_ecount_full(cNames) LPOLESTR *rgszNames,
            /* [range][in] */ UINT cNames,
            /* [in] */ LCID lcid,
            /* [size_is][out] */ __RPC__out_ecount_full(cNames) DISPID *rgDispId);
        
        /* [local] */ HRESULT ( STDMETHODCALLTYPE *Invoke )( 
            _dispVSLangProjWebReferencesEvents * This,
            /* [in] */ DISPID dispIdMember,
            /* [in] */ REFIID riid,
            /* [in] */ LCID lcid,
            /* [in] */ WORD wFlags,
            /* [out][in] */ DISPPARAMS *pDispParams,
            /* [out] */ VARIANT *pVarResult,
            /* [out] */ EXCEPINFO *pExcepInfo,
            /* [out] */ UINT *puArgErr);
        
        END_INTERFACE
    } _dispVSLangProjWebReferencesEventsVtbl;

    interface _dispVSLangProjWebReferencesEvents
    {
        CONST_VTBL struct _dispVSLangProjWebReferencesEventsVtbl *lpVtbl;
    };

    

#ifdef COBJMACROS


#define _dispVSLangProjWebReferencesEvents_QueryInterface(This,riid,ppvObject)	\
    ( (This)->lpVtbl -> QueryInterface(This,riid,ppvObject) ) 

#define _dispVSLangProjWebReferencesEvents_AddRef(This)	\
    ( (This)->lpVtbl -> AddRef(This) ) 

#define _dispVSLangProjWebReferencesEvents_Release(This)	\
    ( (This)->lpVtbl -> Release(This) ) 


#define _dispVSLangProjWebReferencesEvents_GetTypeInfoCount(This,pctinfo)	\
    ( (This)->lpVtbl -> GetTypeInfoCount(This,pctinfo) ) 

#define _dispVSLangProjWebReferencesEvents_GetTypeInfo(This,iTInfo,lcid,ppTInfo)	\
    ( (This)->lpVtbl -> GetTypeInfo(This,iTInfo,lcid,ppTInfo) ) 

#define _dispVSLangProjWebReferencesEvents_GetIDsOfNames(This,riid,rgszNames,cNames,lcid,rgDispId)	\
    ( (This)->lpVtbl -> GetIDsOfNames(This,riid,rgszNames,cNames,lcid,rgDispId) ) 

#define _dispVSLangProjWebReferencesEvents_Invoke(This,dispIdMember,riid,lcid,wFlags,pDispParams,pVarResult,pExcepInfo,puArgErr)	\
    ( (This)->lpVtbl -> Invoke(This,dispIdMember,riid,lcid,wFlags,pDispParams,pVarResult,pExcepInfo,puArgErr) ) 

#endif /* COBJMACROS */


#endif 	/* C style interface */


#endif 	/* ___dispVSLangProjWebReferencesEvents_DISPINTERFACE_DEFINED__ */


EXTERN_C const CLSID CLSID_VSLangProjWebReferencesEvents;

#ifdef __cplusplus

class DECLSPEC_UUID("2DF4B0DE-579D-46b2-9D6E-88AF01E08FB0")
VSLangProjWebReferencesEvents;
#endif

#ifndef __VSProjectEvents2_INTERFACE_DEFINED__
#define __VSProjectEvents2_INTERFACE_DEFINED__

/* interface VSProjectEvents2 */
/* [uuid][object][oleautomation][dual] */ 


EXTERN_C const IID IID_VSProjectEvents2;

#if defined(__cplusplus) && !defined(CINTERFACE)
    
    MIDL_INTERFACE("6DCBC5A7-37BF-461c-958F-A81DA10D242E")
    VSProjectEvents2 : public VSProjectEvents
    {
    public:
        virtual /* [helpstring][propget][id] */ HRESULT STDMETHODCALLTYPE get_VSLangProjWebReferencesEvents( 
            /* [retval][out] */ __RPC__deref_out_opt VSLangProjWebReferencesEvents	**ppVSLangProjWebReferencesEvents) = 0;
        
    };
    
#else 	/* C style interface */

    typedef struct VSProjectEvents2Vtbl
    {
        BEGIN_INTERFACE
        
        /* [id][restricted][funcdescattr] */ HRESULT ( STDMETHODCALLTYPE *QueryInterface )( 
            VSProjectEvents2 * This,
            /* [in][idldescattr] */ __RPC__in struct GUID *riid,
            /* [out][idldescattr] */ __RPC__deref_out_opt void **ppvObj,
            /* [retval][out] */ __RPC__out void *retval);
        
        /* [id][restricted][funcdescattr] */ HRESULT ( STDMETHODCALLTYPE *AddRef )( 
            VSProjectEvents2 * This,
            /* [retval][out] */ __RPC__out unsigned long *retval);
        
        /* [id][restricted][funcdescattr] */ HRESULT ( STDMETHODCALLTYPE *Release )( 
            VSProjectEvents2 * This,
            /* [retval][out] */ __RPC__out unsigned long *retval);
        
        /* [id][restricted][funcdescattr] */ HRESULT ( STDMETHODCALLTYPE *GetTypeInfoCount )( 
            VSProjectEvents2 * This,
            /* [out][idldescattr] */ __RPC__out unsigned UINT *pctinfo,
            /* [retval][out] */ __RPC__out void *retval);
        
        /* [id][restricted][funcdescattr] */ HRESULT ( STDMETHODCALLTYPE *GetTypeInfo )( 
            VSProjectEvents2 * This,
            /* [in][idldescattr] */ unsigned UINT itinfo,
            /* [in][idldescattr] */ unsigned long lcid,
            /* [out][idldescattr] */ __RPC__deref_out_opt void **pptinfo,
            /* [retval][out] */ __RPC__out void *retval);
        
        /* [id][restricted][funcdescattr] */ HRESULT ( STDMETHODCALLTYPE *GetIDsOfNames )( 
            VSProjectEvents2 * This,
            /* [in][idldescattr] */ __RPC__in struct GUID *riid,
            /* [in][idldescattr] */ __RPC__deref_in_opt signed char **rgszNames,
            /* [in][idldescattr] */ unsigned UINT cNames,
            /* [in][idldescattr] */ unsigned long lcid,
            /* [out][idldescattr] */ __RPC__out signed long *rgdispid,
            /* [retval][out] */ __RPC__out void *retval);
        
        /* [id][restricted][funcdescattr] */ HRESULT ( STDMETHODCALLTYPE *Invoke )( 
            VSProjectEvents2 * This,
            /* [in][idldescattr] */ signed long dispidMember,
            /* [in][idldescattr] */ __RPC__in struct GUID *riid,
            /* [in][idldescattr] */ unsigned long lcid,
            /* [in][idldescattr] */ unsigned short wFlags,
            /* [in][idldescattr] */ __RPC__in struct DISPPARAMS *pdispparams,
            /* [out][idldescattr] */ __RPC__out VARIANT *pvarResult,
            /* [out][idldescattr] */ __RPC__out struct EXCEPINFO *pexcepinfo,
            /* [out][idldescattr] */ __RPC__out unsigned UINT *puArgErr,
            /* [retval][out] */ __RPC__out void *retval);
        
        /* [id][propget][funcdescattr] */ HRESULT ( STDMETHODCALLTYPE *get_ReferencesEvents )( 
            VSProjectEvents2 * This,
            /* [retval][out] */ __RPC__deref_out_opt **retval);
        
        /* [id][propget][funcdescattr] */ HRESULT ( STDMETHODCALLTYPE *get_BuildManagerEvents )( 
            VSProjectEvents2 * This,
            /* [retval][out] */ __RPC__deref_out_opt **retval);
        
        /* [id][propget][funcdescattr] */ HRESULT ( STDMETHODCALLTYPE *get_ImportsEvents )( 
            VSProjectEvents2 * This,
            /* [retval][out] */ __RPC__deref_out_opt **retval);
        
        /* [helpstring][propget][id] */ HRESULT ( STDMETHODCALLTYPE *get_VSLangProjWebReferencesEvents )( 
            VSProjectEvents2 * This,
            /* [retval][out] */ __RPC__deref_out_opt VSLangProjWebReferencesEvents	**ppVSLangProjWebReferencesEvents);
        
        END_INTERFACE
    } VSProjectEvents2Vtbl;

    interface VSProjectEvents2
    {
        CONST_VTBL struct VSProjectEvents2Vtbl *lpVtbl;
    };

    

#ifdef COBJMACROS


#define VSProjectEvents2_QueryInterface(This,riid,ppvObj,retval)	\
    ( (This)->lpVtbl -> QueryInterface(This,riid,ppvObj,retval) ) 

#define VSProjectEvents2_AddRef(This,retval)	\
    ( (This)->lpVtbl -> AddRef(This,retval) ) 

#define VSProjectEvents2_Release(This,retval)	\
    ( (This)->lpVtbl -> Release(This,retval) ) 

#define VSProjectEvents2_GetTypeInfoCount(This,pctinfo,retval)	\
    ( (This)->lpVtbl -> GetTypeInfoCount(This,pctinfo,retval) ) 

#define VSProjectEvents2_GetTypeInfo(This,itinfo,lcid,pptinfo,retval)	\
    ( (This)->lpVtbl -> GetTypeInfo(This,itinfo,lcid,pptinfo,retval) ) 

#define VSProjectEvents2_GetIDsOfNames(This,riid,rgszNames,cNames,lcid,rgdispid,retval)	\
    ( (This)->lpVtbl -> GetIDsOfNames(This,riid,rgszNames,cNames,lcid,rgdispid,retval) ) 

#define VSProjectEvents2_Invoke(This,dispidMember,riid,lcid,wFlags,pdispparams,pvarResult,pexcepinfo,puArgErr,retval)	\
    ( (This)->lpVtbl -> Invoke(This,dispidMember,riid,lcid,wFlags,pdispparams,pvarResult,pexcepinfo,puArgErr,retval) ) 

#define VSProjectEvents2_get_ReferencesEvents(This,retval)	\
    ( (This)->lpVtbl -> get_ReferencesEvents(This,retval) ) 

#define VSProjectEvents2_get_BuildManagerEvents(This,retval)	\
    ( (This)->lpVtbl -> get_BuildManagerEvents(This,retval) ) 

#define VSProjectEvents2_get_ImportsEvents(This,retval)	\
    ( (This)->lpVtbl -> get_ImportsEvents(This,retval) ) 


#define VSProjectEvents2_get_VSLangProjWebReferencesEvents(This,ppVSLangProjWebReferencesEvents)	\
    ( (This)->lpVtbl -> get_VSLangProjWebReferencesEvents(This,ppVSLangProjWebReferencesEvents) ) 

#endif /* COBJMACROS */


#endif 	/* C style interface */




#endif 	/* __VSProjectEvents2_INTERFACE_DEFINED__ */


#ifndef __VSProject2_INTERFACE_DEFINED__
#define __VSProject2_INTERFACE_DEFINED__

/* interface VSProject2 */
/* [uuid][object][oleautomation][dual] */ 


EXTERN_C const IID IID_VSProject2;

#if defined(__cplusplus) && !defined(CINTERFACE)
    
    MIDL_INTERFACE("B1042570-25C6-424a-B58B-56FA83AA828A")
    VSProject2 : public VSProject
    {
    public:
        virtual /* [helpstring][propget][id] */ HRESULT STDMETHODCALLTYPE get_PublishManager( 
            /* [retval][out] */ __RPC__deref_out_opt IDispatch **ppPublishManager) = 0;
        
        virtual /* [helpstring][propget][id] */ HRESULT STDMETHODCALLTYPE get_Events2( 
            /* [retval][out] */ __RPC__deref_out_opt VSProjectEvents2 **ppEvents) = 0;
        
    };
    
#else 	/* C style interface */

    typedef struct VSProject2Vtbl
    {
        BEGIN_INTERFACE
        
        /* [id][restricted][funcdescattr] */ HRESULT ( STDMETHODCALLTYPE *QueryInterface )( 
            VSProject2 * This,
            /* [in][idldescattr] */ __RPC__in struct GUID *riid,
            /* [out][idldescattr] */ __RPC__deref_out_opt void **ppvObj,
            /* [retval][out] */ __RPC__out void *retval);
        
        /* [id][restricted][funcdescattr] */ HRESULT ( STDMETHODCALLTYPE *AddRef )( 
            VSProject2 * This,
            /* [retval][out] */ __RPC__out unsigned long *retval);
        
        /* [id][restricted][funcdescattr] */ HRESULT ( STDMETHODCALLTYPE *Release )( 
            VSProject2 * This,
            /* [retval][out] */ __RPC__out unsigned long *retval);
        
        /* [id][restricted][funcdescattr] */ HRESULT ( STDMETHODCALLTYPE *GetTypeInfoCount )( 
            VSProject2 * This,
            /* [out][idldescattr] */ __RPC__out unsigned UINT *pctinfo,
            /* [retval][out] */ __RPC__out void *retval);
        
        /* [id][restricted][funcdescattr] */ HRESULT ( STDMETHODCALLTYPE *GetTypeInfo )( 
            VSProject2 * This,
            /* [in][idldescattr] */ unsigned UINT itinfo,
            /* [in][idldescattr] */ unsigned long lcid,
            /* [out][idldescattr] */ __RPC__deref_out_opt void **pptinfo,
            /* [retval][out] */ __RPC__out void *retval);
        
        /* [id][restricted][funcdescattr] */ HRESULT ( STDMETHODCALLTYPE *GetIDsOfNames )( 
            VSProject2 * This,
            /* [in][idldescattr] */ __RPC__in struct GUID *riid,
            /* [in][idldescattr] */ __RPC__deref_in_opt signed char **rgszNames,
            /* [in][idldescattr] */ unsigned UINT cNames,
            /* [in][idldescattr] */ unsigned long lcid,
            /* [out][idldescattr] */ __RPC__out signed long *rgdispid,
            /* [retval][out] */ __RPC__out void *retval);
        
        /* [id][restricted][funcdescattr] */ HRESULT ( STDMETHODCALLTYPE *Invoke )( 
            VSProject2 * This,
            /* [in][idldescattr] */ signed long dispidMember,
            /* [in][idldescattr] */ __RPC__in struct GUID *riid,
            /* [in][idldescattr] */ unsigned long lcid,
            /* [in][idldescattr] */ unsigned short wFlags,
            /* [in][idldescattr] */ __RPC__in struct DISPPARAMS *pdispparams,
            /* [out][idldescattr] */ __RPC__out VARIANT *pvarResult,
            /* [out][idldescattr] */ __RPC__out struct EXCEPINFO *pexcepinfo,
            /* [out][idldescattr] */ __RPC__out unsigned UINT *puArgErr,
            /* [retval][out] */ __RPC__out void *retval);
        
        /* [id][propget][funcdescattr] */ HRESULT ( STDMETHODCALLTYPE *get_References )( 
            VSProject2 * This,
            /* [retval][out] */ __RPC__deref_out_opt References **retval);
        
        /* [id][propget][funcdescattr] */ HRESULT ( STDMETHODCALLTYPE *get_BuildManager )( 
            VSProject2 * This,
            /* [retval][out] */ __RPC__deref_out_opt BuildManager **retval);
        
        /* [id][propget][funcdescattr] */ HRESULT ( STDMETHODCALLTYPE *get_DTE )( 
            VSProject2 * This,
            /* [retval][out] */ __RPC__deref_out_opt **retval);
        
        /* [id][propget][funcdescattr] */ HRESULT ( STDMETHODCALLTYPE *get_Project )( 
            VSProject2 * This,
            /* [retval][out] */ __RPC__deref_out_opt Project **retval);
        
        /* [id][funcdescattr] */ HRESULT ( STDMETHODCALLTYPE *CreateWebReferencesFolder )( 
            VSProject2 * This,
            /* [retval][out] */ __RPC__deref_out_opt ProjectItem **retval);
        
        /* [id][propget][funcdescattr] */ HRESULT ( STDMETHODCALLTYPE *get_WebReferencesFolder )( 
            VSProject2 * This,
            /* [retval][out] */ __RPC__deref_out_opt ProjectItem **retval);
        
        /* [id][funcdescattr] */ HRESULT ( STDMETHODCALLTYPE *AddWebReference )( 
            VSProject2 * This,
            /* [in][idldescattr] */ __RPC__in BSTR bstrUrl,
            /* [retval][out] */ __RPC__deref_out_opt ProjectItem **retval);
        
        /* [id][propget][funcdescattr] */ HRESULT ( STDMETHODCALLTYPE *get_TemplatePath )( 
            VSProject2 * This,
            /* [retval][out] */ __RPC__deref_out_opt BSTR *retval);
        
        /* [id][funcdescattr] */ HRESULT ( STDMETHODCALLTYPE *Refresh )( 
            VSProject2 * This,
            /* [retval][out] */ __RPC__out void *retval);
        
        /* [id][propget][funcdescattr] */ HRESULT ( STDMETHODCALLTYPE *get_WorkOffline )( 
            VSProject2 * This,
            /* [retval][out] */ __RPC__out BOOLEAN *retval);
        
        /* [id][propput][funcdescattr] */ HRESULT ( STDMETHODCALLTYPE *put_WorkOffline )( 
            VSProject2 * This,
            /* [in][idldescattr] */ BOOLEAN noname,
            /* [retval][out] */ __RPC__out void *retval);
        
        /* [id][propget][funcdescattr] */ HRESULT ( STDMETHODCALLTYPE *get_Imports )( 
            VSProject2 * This,
            /* [retval][out] */ __RPC__deref_out_opt Imports **retval);
        
        /* [id][propget][funcdescattr] */ HRESULT ( STDMETHODCALLTYPE *get_Events )( 
            VSProject2 * This,
            /* [retval][out] */ __RPC__deref_out_opt VSProjectEvents **retval);
        
        /* [id][funcdescattr] */ HRESULT ( STDMETHODCALLTYPE *CopyProject )( 
            VSProject2 * This,
            /* [in][idldescattr] */ __RPC__in BSTR bstrDestFolder,
            /* [in][idldescattr] */ __RPC__in BSTR bstrDestUNCPath,
            /* [in][idldescattr] */ enum prjCopyProjectOption copyProjectOption,
            /* [in][idldescattr] */ __RPC__in BSTR bstrUsername,
            /* [in][idldescattr] */ __RPC__in BSTR bstrPassword,
            /* [retval][out] */ __RPC__out void *retval);
        
        /* [id][hidden][funcdescattr] */ HRESULT ( STDMETHODCALLTYPE *Exec )( 
            VSProject2 * This,
            /* [in][idldescattr] */ enum prjExecCommand command,
            /* [idldescattr] */ signed long bSuppressUI,
            /* [in][idldescattr] */ VARIANT varIn,
            /* [out][idldescattr] */ __RPC__out VARIANT *pVarOut,
            /* [retval][out] */ __RPC__out void *retval);
        
        /* [id][funcdescattr] */ HRESULT ( STDMETHODCALLTYPE *GenerateKeyPairFiles )( 
            VSProject2 * This,
            /* [in][idldescattr] */ __RPC__in BSTR strPublicPrivateFile,
            /* [in][idldescattr] */ __RPC__in BSTR strPublicOnlyFile,
            /* [retval][out] */ __RPC__out void *retval);
        
        /* [id][funcdescattr] */ HRESULT ( STDMETHODCALLTYPE *GetUniqueFilename )( 
            VSProject2 * This,
            /* [in][idldescattr] */ __RPC__in_opt IDispatch *pDispatch,
            /* [in][idldescattr] */ __RPC__in BSTR bstrRoot,
            /* [in][idldescattr] */ __RPC__in BSTR bstrDesiredExt,
            /* [retval][out] */ __RPC__deref_out_opt BSTR *retval);
        
        /* [helpstring][propget][id] */ HRESULT ( STDMETHODCALLTYPE *get_PublishManager )( 
            VSProject2 * This,
            /* [retval][out] */ __RPC__deref_out_opt IDispatch **ppPublishManager);
        
        /* [helpstring][propget][id] */ HRESULT ( STDMETHODCALLTYPE *get_Events2 )( 
            VSProject2 * This,
            /* [retval][out] */ __RPC__deref_out_opt VSProjectEvents2 **ppEvents);
        
        END_INTERFACE
    } VSProject2Vtbl;

    interface VSProject2
    {
        CONST_VTBL struct VSProject2Vtbl *lpVtbl;
    };

    

#ifdef COBJMACROS


#define VSProject2_QueryInterface(This,riid,ppvObj,retval)	\
    ( (This)->lpVtbl -> QueryInterface(This,riid,ppvObj,retval) ) 

#define VSProject2_AddRef(This,retval)	\
    ( (This)->lpVtbl -> AddRef(This,retval) ) 

#define VSProject2_Release(This,retval)	\
    ( (This)->lpVtbl -> Release(This,retval) ) 

#define VSProject2_GetTypeInfoCount(This,pctinfo,retval)	\
    ( (This)->lpVtbl -> GetTypeInfoCount(This,pctinfo,retval) ) 

#define VSProject2_GetTypeInfo(This,itinfo,lcid,pptinfo,retval)	\
    ( (This)->lpVtbl -> GetTypeInfo(This,itinfo,lcid,pptinfo,retval) ) 

#define VSProject2_GetIDsOfNames(This,riid,rgszNames,cNames,lcid,rgdispid,retval)	\
    ( (This)->lpVtbl -> GetIDsOfNames(This,riid,rgszNames,cNames,lcid,rgdispid,retval) ) 

#define VSProject2_Invoke(This,dispidMember,riid,lcid,wFlags,pdispparams,pvarResult,pexcepinfo,puArgErr,retval)	\
    ( (This)->lpVtbl -> Invoke(This,dispidMember,riid,lcid,wFlags,pdispparams,pvarResult,pexcepinfo,puArgErr,retval) ) 

#define VSProject2_get_References(This,retval)	\
    ( (This)->lpVtbl -> get_References(This,retval) ) 

#define VSProject2_get_BuildManager(This,retval)	\
    ( (This)->lpVtbl -> get_BuildManager(This,retval) ) 

#define VSProject2_get_DTE(This,retval)	\
    ( (This)->lpVtbl -> get_DTE(This,retval) ) 

#define VSProject2_get_Project(This,retval)	\
    ( (This)->lpVtbl -> get_Project(This,retval) ) 

#define VSProject2_CreateWebReferencesFolder(This,retval)	\
    ( (This)->lpVtbl -> CreateWebReferencesFolder(This,retval) ) 

#define VSProject2_get_WebReferencesFolder(This,retval)	\
    ( (This)->lpVtbl -> get_WebReferencesFolder(This,retval) ) 

#define VSProject2_AddWebReference(This,bstrUrl,retval)	\
    ( (This)->lpVtbl -> AddWebReference(This,bstrUrl,retval) ) 

#define VSProject2_get_TemplatePath(This,retval)	\
    ( (This)->lpVtbl -> get_TemplatePath(This,retval) ) 

#define VSProject2_Refresh(This,retval)	\
    ( (This)->lpVtbl -> Refresh(This,retval) ) 

#define VSProject2_get_WorkOffline(This,retval)	\
    ( (This)->lpVtbl -> get_WorkOffline(This,retval) ) 

#define VSProject2_put_WorkOffline(This,noname,retval)	\
    ( (This)->lpVtbl -> put_WorkOffline(This,noname,retval) ) 

#define VSProject2_get_Imports(This,retval)	\
    ( (This)->lpVtbl -> get_Imports(This,retval) ) 

#define VSProject2_get_Events(This,retval)	\
    ( (This)->lpVtbl -> get_Events(This,retval) ) 

#define VSProject2_CopyProject(This,bstrDestFolder,bstrDestUNCPath,copyProjectOption,bstrUsername,bstrPassword,retval)	\
    ( (This)->lpVtbl -> CopyProject(This,bstrDestFolder,bstrDestUNCPath,copyProjectOption,bstrUsername,bstrPassword,retval) ) 

#define VSProject2_Exec(This,command,bSuppressUI,varIn,pVarOut,retval)	\
    ( (This)->lpVtbl -> Exec(This,command,bSuppressUI,varIn,pVarOut,retval) ) 

#define VSProject2_GenerateKeyPairFiles(This,strPublicPrivateFile,strPublicOnlyFile,retval)	\
    ( (This)->lpVtbl -> GenerateKeyPairFiles(This,strPublicPrivateFile,strPublicOnlyFile,retval) ) 

#define VSProject2_GetUniqueFilename(This,pDispatch,bstrRoot,bstrDesiredExt,retval)	\
    ( (This)->lpVtbl -> GetUniqueFilename(This,pDispatch,bstrRoot,bstrDesiredExt,retval) ) 


#define VSProject2_get_PublishManager(This,ppPublishManager)	\
    ( (This)->lpVtbl -> get_PublishManager(This,ppPublishManager) ) 

#define VSProject2_get_Events2(This,ppEvents)	\
    ( (This)->lpVtbl -> get_Events2(This,ppEvents) ) 

#endif /* COBJMACROS */


#endif 	/* C style interface */




#endif 	/* __VSProject2_INTERFACE_DEFINED__ */


#ifndef __ProjectProperties3_INTERFACE_DEFINED__
#define __ProjectProperties3_INTERFACE_DEFINED__

/* interface ProjectProperties3 */
/* [object][dual][unique][helpstring][uuid] */ 

typedef /* [uuid] */  DECLSPEC_UUID("355354DE-E2F0-45d5-9632-4823655C9C95") 
enum prjAssemblyType
    {	prjAssemblyType_Library	= 0,
	prjAssemblyType_Platform	= 1
    } 	prjAssemblyType;

#define prjAssemblyTypeMin  prjAssemblyType_Library
#define prjAssemblyTypeMax  prjAssemblyType_Platform

EXTERN_C const IID IID_ProjectProperties3;

#if defined(__cplusplus) && !defined(CINTERFACE)
    
    MIDL_INTERFACE("7C9D1773-F1F3-447c-AF1A-218E5E2C2F7F")
    ProjectProperties3 : public ProjectProperties2
    {
    public:
        virtual /* [helpstring][id][nonbrowsable][propget] */ HRESULT STDMETHODCALLTYPE get_Title( 
            /* [retval][out] */ __RPC__deref_out_opt BSTR *pbstrTitle) = 0;
        
        virtual /* [helpstring][id][nonbrowsable][propput] */ HRESULT STDMETHODCALLTYPE put_Title( 
            /* [in] */ __RPC__in BSTR Title) = 0;
        
        virtual /* [helpstring][id][nonbrowsable][propget] */ HRESULT STDMETHODCALLTYPE get_Description( 
            /* [retval][out] */ __RPC__deref_out_opt BSTR *pbstrDescription) = 0;
        
        virtual /* [helpstring][id][nonbrowsable][propput] */ HRESULT STDMETHODCALLTYPE put_Description( 
            /* [in] */ __RPC__in BSTR Description) = 0;
        
        virtual /* [helpstring][id][nonbrowsable][propget] */ HRESULT STDMETHODCALLTYPE get_Company( 
            /* [retval][out] */ __RPC__deref_out_opt BSTR *pbstrCompany) = 0;
        
        virtual /* [helpstring][id][nonbrowsable][propput] */ HRESULT STDMETHODCALLTYPE put_Company( 
            /* [in] */ __RPC__in BSTR Company) = 0;
        
        virtual /* [helpstring][id][nonbrowsable][propget] */ HRESULT STDMETHODCALLTYPE get_Product( 
            /* [retval][out] */ __RPC__deref_out_opt BSTR *pbstrProduct) = 0;
        
        virtual /* [helpstring][id][nonbrowsable][propput] */ HRESULT STDMETHODCALLTYPE put_Product( 
            /* [in] */ __RPC__in BSTR Product) = 0;
        
        virtual /* [helpstring][id][nonbrowsable][propget] */ HRESULT STDMETHODCALLTYPE get_Copyright( 
            /* [retval][out] */ __RPC__deref_out_opt BSTR *pbstrCopyright) = 0;
        
        virtual /* [helpstring][id][nonbrowsable][propput] */ HRESULT STDMETHODCALLTYPE put_Copyright( 
            /* [in] */ __RPC__in BSTR Copyright) = 0;
        
        virtual /* [helpstring][id][nonbrowsable][propget] */ HRESULT STDMETHODCALLTYPE get_Trademark( 
            /* [retval][out] */ __RPC__deref_out_opt BSTR *pbstrTrademark) = 0;
        
        virtual /* [helpstring][id][nonbrowsable][propput] */ HRESULT STDMETHODCALLTYPE put_Trademark( 
            /* [in] */ __RPC__in BSTR Trademark) = 0;
        
        virtual /* [helpstring][id][propget] */ HRESULT STDMETHODCALLTYPE get_AssemblyType( 
            /* [retval][out] */ __RPC__out prjAssemblyType *pAssemblyType) = 0;
        
        virtual /* [helpstring][id][propput] */ HRESULT STDMETHODCALLTYPE put_AssemblyType( 
            /* [in] */ prjAssemblyType AssemblyType) = 0;
        
        virtual /* [helpstring][id][propget] */ HRESULT STDMETHODCALLTYPE get_TypeComplianceDiagnostics( 
            /* [retval][out] */ __RPC__out VARIANT_BOOL *pbTypeComplianceDiagnostics) = 0;
        
        virtual /* [helpstring][id][propput] */ HRESULT STDMETHODCALLTYPE put_TypeComplianceDiagnostics( 
            /* [in] */ VARIANT_BOOL TypeComplianceDiagnostics) = 0;
        
        virtual /* [helpstring][id][propget] */ HRESULT STDMETHODCALLTYPE get_Win32ResourceFile( 
            /* [retval][out] */ __RPC__deref_out_opt BSTR *pbstrW32ResourceFile) = 0;
        
        virtual /* [helpstring][id][propput] */ HRESULT STDMETHODCALLTYPE put_Win32ResourceFile( 
            /* [in] */ __RPC__in BSTR Win32ResourceFile) = 0;
        
        virtual /* [helpstring][id][propget] */ HRESULT STDMETHODCALLTYPE get_AssemblyKeyProviderName( 
            /* [retval][out] */ __RPC__deref_out_opt BSTR *pbstrKeyProviderName) = 0;
        
        virtual /* [helpstring][id][propput] */ HRESULT STDMETHODCALLTYPE put_AssemblyKeyProviderName( 
            /* [in] */ __RPC__in BSTR KeyProviderName) = 0;
        
        virtual /* [helpstring][id][propget] */ HRESULT STDMETHODCALLTYPE get_AssemblyOriginatorKeyFileType( 
            /* [retval][out] */ __RPC__out DWORD *pdwOriginatorKeyFileType) = 0;
        
        virtual /* [helpstring][id][propput] */ HRESULT STDMETHODCALLTYPE put_AssemblyOriginatorKeyFileType( 
            /* [in] */ DWORD OriginatorKeyFileType) = 0;
        
        virtual /* [helpstring][id][nonbrowsable][propget] */ HRESULT STDMETHODCALLTYPE get_AssemblyVersion( 
            /* [retval][out] */ __RPC__deref_out_opt BSTR *pbstrAssemblyVersion) = 0;
        
        virtual /* [helpstring][id][nonbrowsable][propput] */ HRESULT STDMETHODCALLTYPE put_AssemblyVersion( 
            /* [in] */ __RPC__in BSTR AssemblyVersion) = 0;
        
        virtual /* [helpstring][id][nonbrowsable][propget] */ HRESULT STDMETHODCALLTYPE get_AssemblyFileVersion( 
            /* [retval][out] */ __RPC__deref_out_opt BSTR *pbstrAssemblyFileVersion) = 0;
        
        virtual /* [helpstring][id][nonbrowsable][propput] */ HRESULT STDMETHODCALLTYPE put_AssemblyFileVersion( 
            /* [in] */ __RPC__in BSTR AssemblyFileVersion) = 0;
        
        virtual /* [helpstring][id][propget] */ HRESULT STDMETHODCALLTYPE get_GenerateManifests( 
            /* [retval][out] */ __RPC__out VARIANT_BOOL *pbGenerateManifests) = 0;
        
        virtual /* [helpstring][id][propput] */ HRESULT STDMETHODCALLTYPE put_GenerateManifests( 
            /* [in] */ VARIANT_BOOL GenerateManifests) = 0;
        
        virtual /* [helpstring][id][propget] */ HRESULT STDMETHODCALLTYPE get_EnableSecurityDebugging( 
            /* [retval][out] */ __RPC__out VARIANT_BOOL *pbEnableSecurityDebugging) = 0;
        
        virtual /* [helpstring][id][propput] */ HRESULT STDMETHODCALLTYPE put_EnableSecurityDebugging( 
            /* [in] */ VARIANT_BOOL EnableSecurityDebugging) = 0;
        
        virtual /* [helpstring][id][propget] */ HRESULT STDMETHODCALLTYPE get_DebugSecurityZoneURL( 
            /* [retval][out] */ __RPC__deref_out_opt BSTR *pbstrSecurityURL) = 0;
        
        virtual /* [helpstring][id][propput] */ HRESULT STDMETHODCALLTYPE put_DebugSecurityZoneURL( 
            /* [in] */ __RPC__in BSTR SecurityURL) = 0;
        
        virtual /* [helpstring][id][nonbrowsable][propget] */ HRESULT STDMETHODCALLTYPE get_Publish( 
            /* [retval][out] */ __RPC__deref_out_opt IDispatch **ppdispPublish) = 0;
        
        virtual /* [helpstring][id][nonbrowsable][propget] */ HRESULT STDMETHODCALLTYPE get_ComVisible( 
            /* [retval][out] */ __RPC__out VARIANT_BOOL *pbComVisible) = 0;
        
        virtual /* [helpstring][id][nonbrowsable][propput] */ HRESULT STDMETHODCALLTYPE put_ComVisible( 
            /* [in] */ VARIANT_BOOL ComVisible) = 0;
        
        virtual /* [helpstring][id][nonbrowsable][propget] */ HRESULT STDMETHODCALLTYPE get_AssemblyGuid( 
            /* [retval][out] */ __RPC__deref_out_opt BSTR *pbstrAssemblyGuid) = 0;
        
        virtual /* [helpstring][id][nonbrowsable][propput] */ HRESULT STDMETHODCALLTYPE put_AssemblyGuid( 
            /* [in] */ __RPC__in BSTR AssemblyGuid) = 0;
        
        virtual /* [helpstring][id][nonbrowsable][propget] */ HRESULT STDMETHODCALLTYPE get_NeutralResourcesLanguage( 
            /* [retval][out] */ __RPC__deref_out_opt BSTR *pbstrNeutralResourcesLanguage) = 0;
        
        virtual /* [helpstring][id][nonbrowsable][propput] */ HRESULT STDMETHODCALLTYPE put_NeutralResourcesLanguage( 
            /* [in] */ __RPC__in BSTR NeutralResourcesLanguage) = 0;
        
        virtual /* [helpstring][id][propget] */ HRESULT STDMETHODCALLTYPE get_SignAssembly( 
            /* [retval][out] */ __RPC__out VARIANT_BOOL *pbSignAssembly) = 0;
        
        virtual /* [helpstring][id][propput] */ HRESULT STDMETHODCALLTYPE put_SignAssembly( 
            /* [in] */ VARIANT_BOOL SignAssembly) = 0;
        
        virtual /* [helpstring][id][propget] */ HRESULT STDMETHODCALLTYPE get_SignManifests( 
            /* [retval][out] */ __RPC__out VARIANT_BOOL *pbSignManifests) = 0;
        
        virtual /* [helpstring][id][propput] */ HRESULT STDMETHODCALLTYPE put_SignManifests( 
            /* [in] */ VARIANT_BOOL bSignManifests) = 0;
        
        virtual /* [helpstring][id][propget] */ HRESULT STDMETHODCALLTYPE get_TargetZone( 
            /* [retval][out] */ __RPC__deref_out_opt BSTR *pbstrTargetZone) = 0;
        
        virtual /* [helpstring][id][propput] */ HRESULT STDMETHODCALLTYPE put_TargetZone( 
            /* [in] */ __RPC__in BSTR TargetZone) = 0;
        
        virtual /* [helpstring][id][propget] */ HRESULT STDMETHODCALLTYPE get_ExcludedPermissions( 
            /* [retval][out] */ __RPC__deref_out_opt BSTR *pbstrExcludedPermissions) = 0;
        
        virtual /* [helpstring][id][propput] */ HRESULT STDMETHODCALLTYPE put_ExcludedPermissions( 
            /* [in] */ __RPC__in BSTR ExcludedPermissions) = 0;
        
        virtual /* [helpstring][id][propget] */ HRESULT STDMETHODCALLTYPE get_ManifestCertificateThumbprint( 
            /* [retval][out] */ __RPC__deref_out_opt BSTR *pbstrManifestCertificateThumbprint) = 0;
        
        virtual /* [helpstring][id][propput] */ HRESULT STDMETHODCALLTYPE put_ManifestCertificateThumbprint( 
            /* [in] */ __RPC__in BSTR ManifestCertificateThumbprint) = 0;
        
        virtual /* [helpstring][id][propget] */ HRESULT STDMETHODCALLTYPE get_ManifestKeyFile( 
            /* [retval][out] */ __RPC__deref_out_opt BSTR *pbstrManifestKeyFile) = 0;
        
        virtual /* [helpstring][id][propput] */ HRESULT STDMETHODCALLTYPE put_ManifestKeyFile( 
            /* [in] */ __RPC__in BSTR ManifestKeyFile) = 0;
        
        virtual /* [helpstring][id][propget] */ HRESULT STDMETHODCALLTYPE get_ManifestTimestampUrl( 
            /* [retval][out] */ __RPC__deref_out_opt BSTR *pbstrManifestTimestampUrl) = 0;
        
        virtual /* [helpstring][id][propput] */ HRESULT STDMETHODCALLTYPE put_ManifestTimestampUrl( 
            /* [in] */ __RPC__in BSTR ManifestTimestampUrl) = 0;
        
    };
    
#else 	/* C style interface */

    typedef struct ProjectProperties3Vtbl
    {
        BEGIN_INTERFACE
        
        /* [id][restricted][funcdescattr] */ HRESULT ( STDMETHODCALLTYPE *QueryInterface )( 
            ProjectProperties3 * This,
            /* [in][idldescattr] */ __RPC__in struct GUID *riid,
            /* [out][idldescattr] */ __RPC__deref_out_opt void **ppvObj,
            /* [retval][out] */ __RPC__out void *retval);
        
        /* [id][restricted][funcdescattr] */ HRESULT ( STDMETHODCALLTYPE *AddRef )( 
            ProjectProperties3 * This,
            /* [retval][out] */ __RPC__out unsigned long *retval);
        
        /* [id][restricted][funcdescattr] */ HRESULT ( STDMETHODCALLTYPE *Release )( 
            ProjectProperties3 * This,
            /* [retval][out] */ __RPC__out unsigned long *retval);
        
        /* [id][restricted][funcdescattr] */ HRESULT ( STDMETHODCALLTYPE *GetTypeInfoCount )( 
            ProjectProperties3 * This,
            /* [out][idldescattr] */ __RPC__out unsigned UINT *pctinfo,
            /* [retval][out] */ __RPC__out void *retval);
        
        /* [id][restricted][funcdescattr] */ HRESULT ( STDMETHODCALLTYPE *GetTypeInfo )( 
            ProjectProperties3 * This,
            /* [in][idldescattr] */ unsigned UINT itinfo,
            /* [in][idldescattr] */ unsigned long lcid,
            /* [out][idldescattr] */ __RPC__deref_out_opt void **pptinfo,
            /* [retval][out] */ __RPC__out void *retval);
        
        /* [id][restricted][funcdescattr] */ HRESULT ( STDMETHODCALLTYPE *GetIDsOfNames )( 
            ProjectProperties3 * This,
            /* [in][idldescattr] */ __RPC__in struct GUID *riid,
            /* [in][idldescattr] */ __RPC__deref_in_opt signed char **rgszNames,
            /* [in][idldescattr] */ unsigned UINT cNames,
            /* [in][idldescattr] */ unsigned long lcid,
            /* [out][idldescattr] */ __RPC__out signed long *rgdispid,
            /* [retval][out] */ __RPC__out void *retval);
        
        /* [id][restricted][funcdescattr] */ HRESULT ( STDMETHODCALLTYPE *Invoke )( 
            ProjectProperties3 * This,
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
            ProjectProperties3 * This,
            /* [retval][out] */ __RPC__deref_out_opt BSTR *retval);
        
        /* [id][propget][hidden][funcdescattr] */ HRESULT ( STDMETHODCALLTYPE *get___project )( 
            ProjectProperties3 * This,
            /* [retval][out] */ __RPC__deref_out_opt IUnknown **retval);
        
        /* [id][propget][funcdescattr] */ HRESULT ( STDMETHODCALLTYPE *get_StartupObject )( 
            ProjectProperties3 * This,
            /* [retval][out] */ __RPC__deref_out_opt BSTR *retval);
        
        /* [id][propput][funcdescattr] */ HRESULT ( STDMETHODCALLTYPE *put_StartupObject )( 
            ProjectProperties3 * This,
            /* [in][idldescattr] */ __RPC__in BSTR noname,
            /* [retval][out] */ __RPC__out void *retval);
        
        /* [id][propget][funcdescattr] */ HRESULT ( STDMETHODCALLTYPE *get_OutputType )( 
            ProjectProperties3 * This,
            /* [retval][out] */ __RPC__out enum prjOutputType *retval);
        
        /* [id][propput][funcdescattr] */ HRESULT ( STDMETHODCALLTYPE *put_OutputType )( 
            ProjectProperties3 * This,
            /* [in][idldescattr] */ enum prjOutputType noname,
            /* [retval][out] */ __RPC__out void *retval);
        
        /* [id][propget][funcdescattr] */ HRESULT ( STDMETHODCALLTYPE *get_RootNamespace )( 
            ProjectProperties3 * This,
            /* [retval][out] */ __RPC__deref_out_opt BSTR *retval);
        
        /* [id][propput][funcdescattr] */ HRESULT ( STDMETHODCALLTYPE *put_RootNamespace )( 
            ProjectProperties3 * This,
            /* [in][idldescattr] */ __RPC__in BSTR noname,
            /* [retval][out] */ __RPC__out void *retval);
        
        /* [id][propget][funcdescattr] */ HRESULT ( STDMETHODCALLTYPE *get_AssemblyName )( 
            ProjectProperties3 * This,
            /* [retval][out] */ __RPC__deref_out_opt BSTR *retval);
        
        /* [id][propput][funcdescattr] */ HRESULT ( STDMETHODCALLTYPE *put_AssemblyName )( 
            ProjectProperties3 * This,
            /* [in][idldescattr] */ __RPC__in BSTR noname,
            /* [retval][out] */ __RPC__out void *retval);
        
        /* [id][propget][funcdescattr] */ HRESULT ( STDMETHODCALLTYPE *get_AssemblyOriginatorKeyFile )( 
            ProjectProperties3 * This,
            /* [retval][out] */ __RPC__deref_out_opt BSTR *retval);
        
        /* [id][propput][funcdescattr] */ HRESULT ( STDMETHODCALLTYPE *put_AssemblyOriginatorKeyFile )( 
            ProjectProperties3 * This,
            /* [in][idldescattr] */ __RPC__in BSTR noname,
            /* [retval][out] */ __RPC__out void *retval);
        
        /* [id][propget][funcdescattr] */ HRESULT ( STDMETHODCALLTYPE *get_AssemblyKeyContainerName )( 
            ProjectProperties3 * This,
            /* [retval][out] */ __RPC__deref_out_opt BSTR *retval);
        
        /* [id][propput][funcdescattr] */ HRESULT ( STDMETHODCALLTYPE *put_AssemblyKeyContainerName )( 
            ProjectProperties3 * This,
            /* [in][idldescattr] */ __RPC__in BSTR noname,
            /* [retval][out] */ __RPC__out void *retval);
        
        /* [id][propget][funcdescattr] */ HRESULT ( STDMETHODCALLTYPE *get_AssemblyOriginatorKeyMode )( 
            ProjectProperties3 * This,
            /* [retval][out] */ __RPC__out enum prjOriginatorKeyMode *retval);
        
        /* [id][propput][funcdescattr] */ HRESULT ( STDMETHODCALLTYPE *put_AssemblyOriginatorKeyMode )( 
            ProjectProperties3 * This,
            /* [in][idldescattr] */ enum prjOriginatorKeyMode noname,
            /* [retval][out] */ __RPC__out void *retval);
        
        /* [id][propget][funcdescattr] */ HRESULT ( STDMETHODCALLTYPE *get_DelaySign )( 
            ProjectProperties3 * This,
            /* [retval][out] */ __RPC__out BOOLEAN *retval);
        
        /* [id][propput][funcdescattr] */ HRESULT ( STDMETHODCALLTYPE *put_DelaySign )( 
            ProjectProperties3 * This,
            /* [in][idldescattr] */ BOOLEAN noname,
            /* [retval][out] */ __RPC__out void *retval);
        
        /* [id][propget][funcdescattr] */ HRESULT ( STDMETHODCALLTYPE *get_WebServer )( 
            ProjectProperties3 * This,
            /* [retval][out] */ __RPC__deref_out_opt BSTR *retval);
        
        /* [id][propget][funcdescattr] */ HRESULT ( STDMETHODCALLTYPE *get_WebServerVersion )( 
            ProjectProperties3 * This,
            /* [retval][out] */ __RPC__deref_out_opt BSTR *retval);
        
        /* [id][propget][funcdescattr] */ HRESULT ( STDMETHODCALLTYPE *get_ServerExtensionsVersion )( 
            ProjectProperties3 * This,
            /* [retval][out] */ __RPC__deref_out_opt BSTR *retval);
        
        /* [id][propget][funcdescattr] */ HRESULT ( STDMETHODCALLTYPE *get_LinkRepair )( 
            ProjectProperties3 * This,
            /* [retval][out] */ __RPC__out BOOLEAN *retval);
        
        /* [id][propput][funcdescattr] */ HRESULT ( STDMETHODCALLTYPE *put_LinkRepair )( 
            ProjectProperties3 * This,
            /* [in][idldescattr] */ BOOLEAN noname,
            /* [retval][out] */ __RPC__out void *retval);
        
        /* [id][propget][funcdescattr] */ HRESULT ( STDMETHODCALLTYPE *get_OfflineURL )( 
            ProjectProperties3 * This,
            /* [retval][out] */ __RPC__deref_out_opt BSTR *retval);
        
        /* [id][propget][funcdescattr] */ HRESULT ( STDMETHODCALLTYPE *get_FileSharePath )( 
            ProjectProperties3 * This,
            /* [retval][out] */ __RPC__deref_out_opt BSTR *retval);
        
        /* [id][propput][funcdescattr] */ HRESULT ( STDMETHODCALLTYPE *put_FileSharePath )( 
            ProjectProperties3 * This,
            /* [in][idldescattr] */ __RPC__in BSTR noname,
            /* [retval][out] */ __RPC__out void *retval);
        
        /* [id][propget][funcdescattr] */ HRESULT ( STDMETHODCALLTYPE *get_ActiveFileSharePath )( 
            ProjectProperties3 * This,
            /* [retval][out] */ __RPC__deref_out_opt BSTR *retval);
        
        /* [id][propget][funcdescattr] */ HRESULT ( STDMETHODCALLTYPE *get_WebAccessMethod )( 
            ProjectProperties3 * This,
            /* [retval][out] */ __RPC__out enum prjWebAccessMethod *retval);
        
        /* [id][propput][funcdescattr] */ HRESULT ( STDMETHODCALLTYPE *put_WebAccessMethod )( 
            ProjectProperties3 * This,
            /* [in][idldescattr] */ enum prjWebAccessMethod noname,
            /* [retval][out] */ __RPC__out void *retval);
        
        /* [id][propget][funcdescattr] */ HRESULT ( STDMETHODCALLTYPE *get_ActiveWebAccessMethod )( 
            ProjectProperties3 * This,
            /* [retval][out] */ __RPC__out enum prjWebAccessMethod *retval);
        
        /* [id][propget][funcdescattr] */ HRESULT ( STDMETHODCALLTYPE *get_DefaultClientScript )( 
            ProjectProperties3 * This,
            /* [retval][out] */ __RPC__out enum prjScriptLanguage *retval);
        
        /* [id][propput][funcdescattr] */ HRESULT ( STDMETHODCALLTYPE *put_DefaultClientScript )( 
            ProjectProperties3 * This,
            /* [in][idldescattr] */ enum prjScriptLanguage noname,
            /* [retval][out] */ __RPC__out void *retval);
        
        /* [id][propget][funcdescattr] */ HRESULT ( STDMETHODCALLTYPE *get_DefaultTargetSchema )( 
            ProjectProperties3 * This,
            /* [retval][out] */ __RPC__out enum prjTargetSchema *retval);
        
        /* [id][propput][funcdescattr] */ HRESULT ( STDMETHODCALLTYPE *put_DefaultTargetSchema )( 
            ProjectProperties3 * This,
            /* [in][idldescattr] */ enum prjTargetSchema noname,
            /* [retval][out] */ __RPC__out void *retval);
        
        /* [id][propget][funcdescattr] */ HRESULT ( STDMETHODCALLTYPE *get_DefaultHTMLPageLayout )( 
            ProjectProperties3 * This,
            /* [retval][out] */ __RPC__out enum prjHTMLPageLayout *retval);
        
        /* [id][propput][funcdescattr] */ HRESULT ( STDMETHODCALLTYPE *put_DefaultHTMLPageLayout )( 
            ProjectProperties3 * This,
            /* [in][idldescattr] */ enum prjHTMLPageLayout noname,
            /* [retval][out] */ __RPC__out void *retval);
        
        /* [id][propget][funcdescattr] */ HRESULT ( STDMETHODCALLTYPE *get_FileName )( 
            ProjectProperties3 * This,
            /* [retval][out] */ __RPC__deref_out_opt BSTR *retval);
        
        /* [id][propput][funcdescattr] */ HRESULT ( STDMETHODCALLTYPE *put_FileName )( 
            ProjectProperties3 * This,
            /* [in][idldescattr] */ __RPC__in BSTR noname,
            /* [retval][out] */ __RPC__out void *retval);
        
        /* [id][propget][funcdescattr] */ HRESULT ( STDMETHODCALLTYPE *get_FullPath )( 
            ProjectProperties3 * This,
            /* [retval][out] */ __RPC__deref_out_opt BSTR *retval);
        
        /* [id][propget][funcdescattr] */ HRESULT ( STDMETHODCALLTYPE *get_LocalPath )( 
            ProjectProperties3 * This,
            /* [retval][out] */ __RPC__deref_out_opt BSTR *retval);
        
        /* [id][propget][funcdescattr] */ HRESULT ( STDMETHODCALLTYPE *get_URL )( 
            ProjectProperties3 * This,
            /* [retval][out] */ __RPC__deref_out_opt BSTR *retval);
        
        /* [id][propget][funcdescattr] */ HRESULT ( STDMETHODCALLTYPE *get_ActiveConfigurationSettings )( 
            ProjectProperties3 * This,
            /* [retval][out] */ __RPC__deref_out_opt ProjectConfigurationProperties **retval);
        
        /* [id][propget][funcdescattr] */ HRESULT ( STDMETHODCALLTYPE *get_Extender )( 
            ProjectProperties3 * This,
            /* [in][idldescattr] */ __RPC__in BSTR ExtenderName,
            /* [retval][out] */ __RPC__deref_out_opt IDispatch **retval);
        
        /* [id][propget][funcdescattr] */ HRESULT ( STDMETHODCALLTYPE *get_ExtenderNames )( 
            ProjectProperties3 * This,
            /* [retval][out] */ __RPC__out VARIANT *retval);
        
        /* [id][propget][funcdescattr] */ HRESULT ( STDMETHODCALLTYPE *get_ExtenderCATID )( 
            ProjectProperties3 * This,
            /* [retval][out] */ __RPC__deref_out_opt BSTR *retval);
        
        /* [id][propget][funcdescattr] */ HRESULT ( STDMETHODCALLTYPE *get_ApplicationIcon )( 
            ProjectProperties3 * This,
            /* [retval][out] */ __RPC__deref_out_opt BSTR *retval);
        
        /* [id][propput][funcdescattr] */ HRESULT ( STDMETHODCALLTYPE *put_ApplicationIcon )( 
            ProjectProperties3 * This,
            /* [in][idldescattr] */ __RPC__in BSTR noname,
            /* [retval][out] */ __RPC__out void *retval);
        
        /* [id][propget][funcdescattr] */ HRESULT ( STDMETHODCALLTYPE *get_OptionStrict )( 
            ProjectProperties3 * This,
            /* [retval][out] */ __RPC__out enum prjOptionStrict *retval);
        
        /* [id][propput][funcdescattr] */ HRESULT ( STDMETHODCALLTYPE *put_OptionStrict )( 
            ProjectProperties3 * This,
            /* [in][idldescattr] */ enum prjOptionStrict noname,
            /* [retval][out] */ __RPC__out void *retval);
        
        /* [id][propget][funcdescattr] */ HRESULT ( STDMETHODCALLTYPE *get_ReferencePath )( 
            ProjectProperties3 * This,
            /* [retval][out] */ __RPC__deref_out_opt BSTR *retval);
        
        /* [id][propput][funcdescattr] */ HRESULT ( STDMETHODCALLTYPE *put_ReferencePath )( 
            ProjectProperties3 * This,
            /* [in][idldescattr] */ __RPC__in BSTR noname,
            /* [retval][out] */ __RPC__out void *retval);
        
        /* [id][propget][funcdescattr] */ HRESULT ( STDMETHODCALLTYPE *get_OutputFileName )( 
            ProjectProperties3 * This,
            /* [retval][out] */ __RPC__deref_out_opt BSTR *retval);
        
        /* [id][propget][hidden][funcdescattr] */ HRESULT ( STDMETHODCALLTYPE *get_AbsoluteProjectDirectory )( 
            ProjectProperties3 * This,
            /* [retval][out] */ __RPC__deref_out_opt BSTR *retval);
        
        /* [id][propget][funcdescattr] */ HRESULT ( STDMETHODCALLTYPE *get_OptionExplicit )( 
            ProjectProperties3 * This,
            /* [retval][out] */ __RPC__out enum prjOptionExplicit *retval);
        
        /* [id][propput][funcdescattr] */ HRESULT ( STDMETHODCALLTYPE *put_OptionExplicit )( 
            ProjectProperties3 * This,
            /* [in][idldescattr] */ enum prjOptionExplicit noname,
            /* [retval][out] */ __RPC__out void *retval);
        
        /* [id][propget][funcdescattr] */ HRESULT ( STDMETHODCALLTYPE *get_OptionCompare )( 
            ProjectProperties3 * This,
            /* [retval][out] */ __RPC__out enum prjCompare *retval);
        
        /* [id][propput][funcdescattr] */ HRESULT ( STDMETHODCALLTYPE *put_OptionCompare )( 
            ProjectProperties3 * This,
            /* [in][idldescattr] */ enum prjCompare noname,
            /* [retval][out] */ __RPC__out void *retval);
        
        /* [id][propget][funcdescattr] */ HRESULT ( STDMETHODCALLTYPE *get_ProjectType )( 
            ProjectProperties3 * This,
            /* [retval][out] */ __RPC__out enum prjProjectType *retval);
        
        /* [id][propget][funcdescattr] */ HRESULT ( STDMETHODCALLTYPE *get_DefaultNamespace )( 
            ProjectProperties3 * This,
            /* [retval][out] */ __RPC__deref_out_opt BSTR *retval);
        
        /* [id][propput][funcdescattr] */ HRESULT ( STDMETHODCALLTYPE *put_DefaultNamespace )( 
            ProjectProperties3 * This,
            /* [in][idldescattr] */ __RPC__in BSTR noname,
            /* [retval][out] */ __RPC__out void *retval);
        
        /* [id][propget][funcdescattr] */ HRESULT ( STDMETHODCALLTYPE *get_PreBuildEvent )( 
            ProjectProperties3 * This,
            /* [retval][out] */ __RPC__deref_out_opt BSTR *retval);
        
        /* [id][propput][funcdescattr] */ HRESULT ( STDMETHODCALLTYPE *put_PreBuildEvent )( 
            ProjectProperties3 * This,
            /* [in][idldescattr] */ __RPC__in BSTR noname,
            /* [retval][out] */ __RPC__out void *retval);
        
        /* [id][propget][funcdescattr] */ HRESULT ( STDMETHODCALLTYPE *get_PostBuildEvent )( 
            ProjectProperties3 * This,
            /* [retval][out] */ __RPC__deref_out_opt BSTR *retval);
        
        /* [id][propput][funcdescattr] */ HRESULT ( STDMETHODCALLTYPE *put_PostBuildEvent )( 
            ProjectProperties3 * This,
            /* [in][idldescattr] */ __RPC__in BSTR noname,
            /* [retval][out] */ __RPC__out void *retval);
        
        /* [id][propget][funcdescattr] */ HRESULT ( STDMETHODCALLTYPE *get_RunPostBuildEvent )( 
            ProjectProperties3 * This,
            /* [retval][out] */ __RPC__out enum prjRunPostBuildEvent *retval);
        
        /* [id][propput][funcdescattr] */ HRESULT ( STDMETHODCALLTYPE *put_RunPostBuildEvent )( 
            ProjectProperties3 * This,
            /* [in][idldescattr] */ enum prjRunPostBuildEvent noname,
            /* [retval][out] */ __RPC__out void *retval);
        
        /* [id][propget][funcdescattr] */ HRESULT ( STDMETHODCALLTYPE *get_AspnetVersion )( 
            ProjectProperties3 * This,
            /* [retval][out] */ __RPC__deref_out_opt BSTR *retval);
        
        /* [helpstring][id][nonbrowsable][propget] */ HRESULT ( STDMETHODCALLTYPE *get_Title )( 
            ProjectProperties3 * This,
            /* [retval][out] */ __RPC__deref_out_opt BSTR *pbstrTitle);
        
        /* [helpstring][id][nonbrowsable][propput] */ HRESULT ( STDMETHODCALLTYPE *put_Title )( 
            ProjectProperties3 * This,
            /* [in] */ __RPC__in BSTR Title);
        
        /* [helpstring][id][nonbrowsable][propget] */ HRESULT ( STDMETHODCALLTYPE *get_Description )( 
            ProjectProperties3 * This,
            /* [retval][out] */ __RPC__deref_out_opt BSTR *pbstrDescription);
        
        /* [helpstring][id][nonbrowsable][propput] */ HRESULT ( STDMETHODCALLTYPE *put_Description )( 
            ProjectProperties3 * This,
            /* [in] */ __RPC__in BSTR Description);
        
        /* [helpstring][id][nonbrowsable][propget] */ HRESULT ( STDMETHODCALLTYPE *get_Company )( 
            ProjectProperties3 * This,
            /* [retval][out] */ __RPC__deref_out_opt BSTR *pbstrCompany);
        
        /* [helpstring][id][nonbrowsable][propput] */ HRESULT ( STDMETHODCALLTYPE *put_Company )( 
            ProjectProperties3 * This,
            /* [in] */ __RPC__in BSTR Company);
        
        /* [helpstring][id][nonbrowsable][propget] */ HRESULT ( STDMETHODCALLTYPE *get_Product )( 
            ProjectProperties3 * This,
            /* [retval][out] */ __RPC__deref_out_opt BSTR *pbstrProduct);
        
        /* [helpstring][id][nonbrowsable][propput] */ HRESULT ( STDMETHODCALLTYPE *put_Product )( 
            ProjectProperties3 * This,
            /* [in] */ __RPC__in BSTR Product);
        
        /* [helpstring][id][nonbrowsable][propget] */ HRESULT ( STDMETHODCALLTYPE *get_Copyright )( 
            ProjectProperties3 * This,
            /* [retval][out] */ __RPC__deref_out_opt BSTR *pbstrCopyright);
        
        /* [helpstring][id][nonbrowsable][propput] */ HRESULT ( STDMETHODCALLTYPE *put_Copyright )( 
            ProjectProperties3 * This,
            /* [in] */ __RPC__in BSTR Copyright);
        
        /* [helpstring][id][nonbrowsable][propget] */ HRESULT ( STDMETHODCALLTYPE *get_Trademark )( 
            ProjectProperties3 * This,
            /* [retval][out] */ __RPC__deref_out_opt BSTR *pbstrTrademark);
        
        /* [helpstring][id][nonbrowsable][propput] */ HRESULT ( STDMETHODCALLTYPE *put_Trademark )( 
            ProjectProperties3 * This,
            /* [in] */ __RPC__in BSTR Trademark);
        
        /* [helpstring][id][propget] */ HRESULT ( STDMETHODCALLTYPE *get_AssemblyType )( 
            ProjectProperties3 * This,
            /* [retval][out] */ __RPC__out prjAssemblyType *pAssemblyType);
        
        /* [helpstring][id][propput] */ HRESULT ( STDMETHODCALLTYPE *put_AssemblyType )( 
            ProjectProperties3 * This,
            /* [in] */ prjAssemblyType AssemblyType);
        
        /* [helpstring][id][propget] */ HRESULT ( STDMETHODCALLTYPE *get_TypeComplianceDiagnostics )( 
            ProjectProperties3 * This,
            /* [retval][out] */ __RPC__out VARIANT_BOOL *pbTypeComplianceDiagnostics);
        
        /* [helpstring][id][propput] */ HRESULT ( STDMETHODCALLTYPE *put_TypeComplianceDiagnostics )( 
            ProjectProperties3 * This,
            /* [in] */ VARIANT_BOOL TypeComplianceDiagnostics);
        
        /* [helpstring][id][propget] */ HRESULT ( STDMETHODCALLTYPE *get_Win32ResourceFile )( 
            ProjectProperties3 * This,
            /* [retval][out] */ __RPC__deref_out_opt BSTR *pbstrW32ResourceFile);
        
        /* [helpstring][id][propput] */ HRESULT ( STDMETHODCALLTYPE *put_Win32ResourceFile )( 
            ProjectProperties3 * This,
            /* [in] */ __RPC__in BSTR Win32ResourceFile);
        
        /* [helpstring][id][propget] */ HRESULT ( STDMETHODCALLTYPE *get_AssemblyKeyProviderName )( 
            ProjectProperties3 * This,
            /* [retval][out] */ __RPC__deref_out_opt BSTR *pbstrKeyProviderName);
        
        /* [helpstring][id][propput] */ HRESULT ( STDMETHODCALLTYPE *put_AssemblyKeyProviderName )( 
            ProjectProperties3 * This,
            /* [in] */ __RPC__in BSTR KeyProviderName);
        
        /* [helpstring][id][propget] */ HRESULT ( STDMETHODCALLTYPE *get_AssemblyOriginatorKeyFileType )( 
            ProjectProperties3 * This,
            /* [retval][out] */ __RPC__out DWORD *pdwOriginatorKeyFileType);
        
        /* [helpstring][id][propput] */ HRESULT ( STDMETHODCALLTYPE *put_AssemblyOriginatorKeyFileType )( 
            ProjectProperties3 * This,
            /* [in] */ DWORD OriginatorKeyFileType);
        
        /* [helpstring][id][nonbrowsable][propget] */ HRESULT ( STDMETHODCALLTYPE *get_AssemblyVersion )( 
            ProjectProperties3 * This,
            /* [retval][out] */ __RPC__deref_out_opt BSTR *pbstrAssemblyVersion);
        
        /* [helpstring][id][nonbrowsable][propput] */ HRESULT ( STDMETHODCALLTYPE *put_AssemblyVersion )( 
            ProjectProperties3 * This,
            /* [in] */ __RPC__in BSTR AssemblyVersion);
        
        /* [helpstring][id][nonbrowsable][propget] */ HRESULT ( STDMETHODCALLTYPE *get_AssemblyFileVersion )( 
            ProjectProperties3 * This,
            /* [retval][out] */ __RPC__deref_out_opt BSTR *pbstrAssemblyFileVersion);
        
        /* [helpstring][id][nonbrowsable][propput] */ HRESULT ( STDMETHODCALLTYPE *put_AssemblyFileVersion )( 
            ProjectProperties3 * This,
            /* [in] */ __RPC__in BSTR AssemblyFileVersion);
        
        /* [helpstring][id][propget] */ HRESULT ( STDMETHODCALLTYPE *get_GenerateManifests )( 
            ProjectProperties3 * This,
            /* [retval][out] */ __RPC__out VARIANT_BOOL *pbGenerateManifests);
        
        /* [helpstring][id][propput] */ HRESULT ( STDMETHODCALLTYPE *put_GenerateManifests )( 
            ProjectProperties3 * This,
            /* [in] */ VARIANT_BOOL GenerateManifests);
        
        /* [helpstring][id][propget] */ HRESULT ( STDMETHODCALLTYPE *get_EnableSecurityDebugging )( 
            ProjectProperties3 * This,
            /* [retval][out] */ __RPC__out VARIANT_BOOL *pbEnableSecurityDebugging);
        
        /* [helpstring][id][propput] */ HRESULT ( STDMETHODCALLTYPE *put_EnableSecurityDebugging )( 
            ProjectProperties3 * This,
            /* [in] */ VARIANT_BOOL EnableSecurityDebugging);
        
        /* [helpstring][id][propget] */ HRESULT ( STDMETHODCALLTYPE *get_DebugSecurityZoneURL )( 
            ProjectProperties3 * This,
            /* [retval][out] */ __RPC__deref_out_opt BSTR *pbstrSecurityURL);
        
        /* [helpstring][id][propput] */ HRESULT ( STDMETHODCALLTYPE *put_DebugSecurityZoneURL )( 
            ProjectProperties3 * This,
            /* [in] */ __RPC__in BSTR SecurityURL);
        
        /* [helpstring][id][nonbrowsable][propget] */ HRESULT ( STDMETHODCALLTYPE *get_Publish )( 
            ProjectProperties3 * This,
            /* [retval][out] */ __RPC__deref_out_opt IDispatch **ppdispPublish);
        
        /* [helpstring][id][nonbrowsable][propget] */ HRESULT ( STDMETHODCALLTYPE *get_ComVisible )( 
            ProjectProperties3 * This,
            /* [retval][out] */ __RPC__out VARIANT_BOOL *pbComVisible);
        
        /* [helpstring][id][nonbrowsable][propput] */ HRESULT ( STDMETHODCALLTYPE *put_ComVisible )( 
            ProjectProperties3 * This,
            /* [in] */ VARIANT_BOOL ComVisible);
        
        /* [helpstring][id][nonbrowsable][propget] */ HRESULT ( STDMETHODCALLTYPE *get_AssemblyGuid )( 
            ProjectProperties3 * This,
            /* [retval][out] */ __RPC__deref_out_opt BSTR *pbstrAssemblyGuid);
        
        /* [helpstring][id][nonbrowsable][propput] */ HRESULT ( STDMETHODCALLTYPE *put_AssemblyGuid )( 
            ProjectProperties3 * This,
            /* [in] */ __RPC__in BSTR AssemblyGuid);
        
        /* [helpstring][id][nonbrowsable][propget] */ HRESULT ( STDMETHODCALLTYPE *get_NeutralResourcesLanguage )( 
            ProjectProperties3 * This,
            /* [retval][out] */ __RPC__deref_out_opt BSTR *pbstrNeutralResourcesLanguage);
        
        /* [helpstring][id][nonbrowsable][propput] */ HRESULT ( STDMETHODCALLTYPE *put_NeutralResourcesLanguage )( 
            ProjectProperties3 * This,
            /* [in] */ __RPC__in BSTR NeutralResourcesLanguage);
        
        /* [helpstring][id][propget] */ HRESULT ( STDMETHODCALLTYPE *get_SignAssembly )( 
            ProjectProperties3 * This,
            /* [retval][out] */ __RPC__out VARIANT_BOOL *pbSignAssembly);
        
        /* [helpstring][id][propput] */ HRESULT ( STDMETHODCALLTYPE *put_SignAssembly )( 
            ProjectProperties3 * This,
            /* [in] */ VARIANT_BOOL SignAssembly);
        
        /* [helpstring][id][propget] */ HRESULT ( STDMETHODCALLTYPE *get_SignManifests )( 
            ProjectProperties3 * This,
            /* [retval][out] */ __RPC__out VARIANT_BOOL *pbSignManifests);
        
        /* [helpstring][id][propput] */ HRESULT ( STDMETHODCALLTYPE *put_SignManifests )( 
            ProjectProperties3 * This,
            /* [in] */ VARIANT_BOOL bSignManifests);
        
        /* [helpstring][id][propget] */ HRESULT ( STDMETHODCALLTYPE *get_TargetZone )( 
            ProjectProperties3 * This,
            /* [retval][out] */ __RPC__deref_out_opt BSTR *pbstrTargetZone);
        
        /* [helpstring][id][propput] */ HRESULT ( STDMETHODCALLTYPE *put_TargetZone )( 
            ProjectProperties3 * This,
            /* [in] */ __RPC__in BSTR TargetZone);
        
        /* [helpstring][id][propget] */ HRESULT ( STDMETHODCALLTYPE *get_ExcludedPermissions )( 
            ProjectProperties3 * This,
            /* [retval][out] */ __RPC__deref_out_opt BSTR *pbstrExcludedPermissions);
        
        /* [helpstring][id][propput] */ HRESULT ( STDMETHODCALLTYPE *put_ExcludedPermissions )( 
            ProjectProperties3 * This,
            /* [in] */ __RPC__in BSTR ExcludedPermissions);
        
        /* [helpstring][id][propget] */ HRESULT ( STDMETHODCALLTYPE *get_ManifestCertificateThumbprint )( 
            ProjectProperties3 * This,
            /* [retval][out] */ __RPC__deref_out_opt BSTR *pbstrManifestCertificateThumbprint);
        
        /* [helpstring][id][propput] */ HRESULT ( STDMETHODCALLTYPE *put_ManifestCertificateThumbprint )( 
            ProjectProperties3 * This,
            /* [in] */ __RPC__in BSTR ManifestCertificateThumbprint);
        
        /* [helpstring][id][propget] */ HRESULT ( STDMETHODCALLTYPE *get_ManifestKeyFile )( 
            ProjectProperties3 * This,
            /* [retval][out] */ __RPC__deref_out_opt BSTR *pbstrManifestKeyFile);
        
        /* [helpstring][id][propput] */ HRESULT ( STDMETHODCALLTYPE *put_ManifestKeyFile )( 
            ProjectProperties3 * This,
            /* [in] */ __RPC__in BSTR ManifestKeyFile);
        
        /* [helpstring][id][propget] */ HRESULT ( STDMETHODCALLTYPE *get_ManifestTimestampUrl )( 
            ProjectProperties3 * This,
            /* [retval][out] */ __RPC__deref_out_opt BSTR *pbstrManifestTimestampUrl);
        
        /* [helpstring][id][propput] */ HRESULT ( STDMETHODCALLTYPE *put_ManifestTimestampUrl )( 
            ProjectProperties3 * This,
            /* [in] */ __RPC__in BSTR ManifestTimestampUrl);
        
        END_INTERFACE
    } ProjectProperties3Vtbl;

    interface ProjectProperties3
    {
        CONST_VTBL struct ProjectProperties3Vtbl *lpVtbl;
    };

    

#ifdef COBJMACROS


#define ProjectProperties3_QueryInterface(This,riid,ppvObj,retval)	\
    ( (This)->lpVtbl -> QueryInterface(This,riid,ppvObj,retval) ) 

#define ProjectProperties3_AddRef(This,retval)	\
    ( (This)->lpVtbl -> AddRef(This,retval) ) 

#define ProjectProperties3_Release(This,retval)	\
    ( (This)->lpVtbl -> Release(This,retval) ) 

#define ProjectProperties3_GetTypeInfoCount(This,pctinfo,retval)	\
    ( (This)->lpVtbl -> GetTypeInfoCount(This,pctinfo,retval) ) 

#define ProjectProperties3_GetTypeInfo(This,itinfo,lcid,pptinfo,retval)	\
    ( (This)->lpVtbl -> GetTypeInfo(This,itinfo,lcid,pptinfo,retval) ) 

#define ProjectProperties3_GetIDsOfNames(This,riid,rgszNames,cNames,lcid,rgdispid,retval)	\
    ( (This)->lpVtbl -> GetIDsOfNames(This,riid,rgszNames,cNames,lcid,rgdispid,retval) ) 

#define ProjectProperties3_Invoke(This,dispidMember,riid,lcid,wFlags,pdispparams,pvarResult,pexcepinfo,puArgErr,retval)	\
    ( (This)->lpVtbl -> Invoke(This,dispidMember,riid,lcid,wFlags,pdispparams,pvarResult,pexcepinfo,puArgErr,retval) ) 

#define ProjectProperties3_get___id(This,retval)	\
    ( (This)->lpVtbl -> get___id(This,retval) ) 

#define ProjectProperties3_get___project(This,retval)	\
    ( (This)->lpVtbl -> get___project(This,retval) ) 

#define ProjectProperties3_get_StartupObject(This,retval)	\
    ( (This)->lpVtbl -> get_StartupObject(This,retval) ) 

#define ProjectProperties3_put_StartupObject(This,noname,retval)	\
    ( (This)->lpVtbl -> put_StartupObject(This,noname,retval) ) 

#define ProjectProperties3_get_OutputType(This,retval)	\
    ( (This)->lpVtbl -> get_OutputType(This,retval) ) 

#define ProjectProperties3_put_OutputType(This,noname,retval)	\
    ( (This)->lpVtbl -> put_OutputType(This,noname,retval) ) 

#define ProjectProperties3_get_RootNamespace(This,retval)	\
    ( (This)->lpVtbl -> get_RootNamespace(This,retval) ) 

#define ProjectProperties3_put_RootNamespace(This,noname,retval)	\
    ( (This)->lpVtbl -> put_RootNamespace(This,noname,retval) ) 

#define ProjectProperties3_get_AssemblyName(This,retval)	\
    ( (This)->lpVtbl -> get_AssemblyName(This,retval) ) 

#define ProjectProperties3_put_AssemblyName(This,noname,retval)	\
    ( (This)->lpVtbl -> put_AssemblyName(This,noname,retval) ) 

#define ProjectProperties3_get_AssemblyOriginatorKeyFile(This,retval)	\
    ( (This)->lpVtbl -> get_AssemblyOriginatorKeyFile(This,retval) ) 

#define ProjectProperties3_put_AssemblyOriginatorKeyFile(This,noname,retval)	\
    ( (This)->lpVtbl -> put_AssemblyOriginatorKeyFile(This,noname,retval) ) 

#define ProjectProperties3_get_AssemblyKeyContainerName(This,retval)	\
    ( (This)->lpVtbl -> get_AssemblyKeyContainerName(This,retval) ) 

#define ProjectProperties3_put_AssemblyKeyContainerName(This,noname,retval)	\
    ( (This)->lpVtbl -> put_AssemblyKeyContainerName(This,noname,retval) ) 

#define ProjectProperties3_get_AssemblyOriginatorKeyMode(This,retval)	\
    ( (This)->lpVtbl -> get_AssemblyOriginatorKeyMode(This,retval) ) 

#define ProjectProperties3_put_AssemblyOriginatorKeyMode(This,noname,retval)	\
    ( (This)->lpVtbl -> put_AssemblyOriginatorKeyMode(This,noname,retval) ) 

#define ProjectProperties3_get_DelaySign(This,retval)	\
    ( (This)->lpVtbl -> get_DelaySign(This,retval) ) 

#define ProjectProperties3_put_DelaySign(This,noname,retval)	\
    ( (This)->lpVtbl -> put_DelaySign(This,noname,retval) ) 

#define ProjectProperties3_get_WebServer(This,retval)	\
    ( (This)->lpVtbl -> get_WebServer(This,retval) ) 

#define ProjectProperties3_get_WebServerVersion(This,retval)	\
    ( (This)->lpVtbl -> get_WebServerVersion(This,retval) ) 

#define ProjectProperties3_get_ServerExtensionsVersion(This,retval)	\
    ( (This)->lpVtbl -> get_ServerExtensionsVersion(This,retval) ) 

#define ProjectProperties3_get_LinkRepair(This,retval)	\
    ( (This)->lpVtbl -> get_LinkRepair(This,retval) ) 

#define ProjectProperties3_put_LinkRepair(This,noname,retval)	\
    ( (This)->lpVtbl -> put_LinkRepair(This,noname,retval) ) 

#define ProjectProperties3_get_OfflineURL(This,retval)	\
    ( (This)->lpVtbl -> get_OfflineURL(This,retval) ) 

#define ProjectProperties3_get_FileSharePath(This,retval)	\
    ( (This)->lpVtbl -> get_FileSharePath(This,retval) ) 

#define ProjectProperties3_put_FileSharePath(This,noname,retval)	\
    ( (This)->lpVtbl -> put_FileSharePath(This,noname,retval) ) 

#define ProjectProperties3_get_ActiveFileSharePath(This,retval)	\
    ( (This)->lpVtbl -> get_ActiveFileSharePath(This,retval) ) 

#define ProjectProperties3_get_WebAccessMethod(This,retval)	\
    ( (This)->lpVtbl -> get_WebAccessMethod(This,retval) ) 

#define ProjectProperties3_put_WebAccessMethod(This,noname,retval)	\
    ( (This)->lpVtbl -> put_WebAccessMethod(This,noname,retval) ) 

#define ProjectProperties3_get_ActiveWebAccessMethod(This,retval)	\
    ( (This)->lpVtbl -> get_ActiveWebAccessMethod(This,retval) ) 

#define ProjectProperties3_get_DefaultClientScript(This,retval)	\
    ( (This)->lpVtbl -> get_DefaultClientScript(This,retval) ) 

#define ProjectProperties3_put_DefaultClientScript(This,noname,retval)	\
    ( (This)->lpVtbl -> put_DefaultClientScript(This,noname,retval) ) 

#define ProjectProperties3_get_DefaultTargetSchema(This,retval)	\
    ( (This)->lpVtbl -> get_DefaultTargetSchema(This,retval) ) 

#define ProjectProperties3_put_DefaultTargetSchema(This,noname,retval)	\
    ( (This)->lpVtbl -> put_DefaultTargetSchema(This,noname,retval) ) 

#define ProjectProperties3_get_DefaultHTMLPageLayout(This,retval)	\
    ( (This)->lpVtbl -> get_DefaultHTMLPageLayout(This,retval) ) 

#define ProjectProperties3_put_DefaultHTMLPageLayout(This,noname,retval)	\
    ( (This)->lpVtbl -> put_DefaultHTMLPageLayout(This,noname,retval) ) 

#define ProjectProperties3_get_FileName(This,retval)	\
    ( (This)->lpVtbl -> get_FileName(This,retval) ) 

#define ProjectProperties3_put_FileName(This,noname,retval)	\
    ( (This)->lpVtbl -> put_FileName(This,noname,retval) ) 

#define ProjectProperties3_get_FullPath(This,retval)	\
    ( (This)->lpVtbl -> get_FullPath(This,retval) ) 

#define ProjectProperties3_get_LocalPath(This,retval)	\
    ( (This)->lpVtbl -> get_LocalPath(This,retval) ) 

#define ProjectProperties3_get_URL(This,retval)	\
    ( (This)->lpVtbl -> get_URL(This,retval) ) 

#define ProjectProperties3_get_ActiveConfigurationSettings(This,retval)	\
    ( (This)->lpVtbl -> get_ActiveConfigurationSettings(This,retval) ) 

#define ProjectProperties3_get_Extender(This,ExtenderName,retval)	\
    ( (This)->lpVtbl -> get_Extender(This,ExtenderName,retval) ) 

#define ProjectProperties3_get_ExtenderNames(This,retval)	\
    ( (This)->lpVtbl -> get_ExtenderNames(This,retval) ) 

#define ProjectProperties3_get_ExtenderCATID(This,retval)	\
    ( (This)->lpVtbl -> get_ExtenderCATID(This,retval) ) 

#define ProjectProperties3_get_ApplicationIcon(This,retval)	\
    ( (This)->lpVtbl -> get_ApplicationIcon(This,retval) ) 

#define ProjectProperties3_put_ApplicationIcon(This,noname,retval)	\
    ( (This)->lpVtbl -> put_ApplicationIcon(This,noname,retval) ) 

#define ProjectProperties3_get_OptionStrict(This,retval)	\
    ( (This)->lpVtbl -> get_OptionStrict(This,retval) ) 

#define ProjectProperties3_put_OptionStrict(This,noname,retval)	\
    ( (This)->lpVtbl -> put_OptionStrict(This,noname,retval) ) 

#define ProjectProperties3_get_ReferencePath(This,retval)	\
    ( (This)->lpVtbl -> get_ReferencePath(This,retval) ) 

#define ProjectProperties3_put_ReferencePath(This,noname,retval)	\
    ( (This)->lpVtbl -> put_ReferencePath(This,noname,retval) ) 

#define ProjectProperties3_get_OutputFileName(This,retval)	\
    ( (This)->lpVtbl -> get_OutputFileName(This,retval) ) 

#define ProjectProperties3_get_AbsoluteProjectDirectory(This,retval)	\
    ( (This)->lpVtbl -> get_AbsoluteProjectDirectory(This,retval) ) 

#define ProjectProperties3_get_OptionExplicit(This,retval)	\
    ( (This)->lpVtbl -> get_OptionExplicit(This,retval) ) 

#define ProjectProperties3_put_OptionExplicit(This,noname,retval)	\
    ( (This)->lpVtbl -> put_OptionExplicit(This,noname,retval) ) 

#define ProjectProperties3_get_OptionCompare(This,retval)	\
    ( (This)->lpVtbl -> get_OptionCompare(This,retval) ) 

#define ProjectProperties3_put_OptionCompare(This,noname,retval)	\
    ( (This)->lpVtbl -> put_OptionCompare(This,noname,retval) ) 

#define ProjectProperties3_get_ProjectType(This,retval)	\
    ( (This)->lpVtbl -> get_ProjectType(This,retval) ) 

#define ProjectProperties3_get_DefaultNamespace(This,retval)	\
    ( (This)->lpVtbl -> get_DefaultNamespace(This,retval) ) 

#define ProjectProperties3_put_DefaultNamespace(This,noname,retval)	\
    ( (This)->lpVtbl -> put_DefaultNamespace(This,noname,retval) ) 

#define ProjectProperties3_get_PreBuildEvent(This,retval)	\
    ( (This)->lpVtbl -> get_PreBuildEvent(This,retval) ) 

#define ProjectProperties3_put_PreBuildEvent(This,noname,retval)	\
    ( (This)->lpVtbl -> put_PreBuildEvent(This,noname,retval) ) 

#define ProjectProperties3_get_PostBuildEvent(This,retval)	\
    ( (This)->lpVtbl -> get_PostBuildEvent(This,retval) ) 

#define ProjectProperties3_put_PostBuildEvent(This,noname,retval)	\
    ( (This)->lpVtbl -> put_PostBuildEvent(This,noname,retval) ) 

#define ProjectProperties3_get_RunPostBuildEvent(This,retval)	\
    ( (This)->lpVtbl -> get_RunPostBuildEvent(This,retval) ) 

#define ProjectProperties3_put_RunPostBuildEvent(This,noname,retval)	\
    ( (This)->lpVtbl -> put_RunPostBuildEvent(This,noname,retval) ) 

#define ProjectProperties3_get_AspnetVersion(This,retval)	\
    ( (This)->lpVtbl -> get_AspnetVersion(This,retval) ) 


#define ProjectProperties3_get_Title(This,pbstrTitle)	\
    ( (This)->lpVtbl -> get_Title(This,pbstrTitle) ) 

#define ProjectProperties3_put_Title(This,Title)	\
    ( (This)->lpVtbl -> put_Title(This,Title) ) 

#define ProjectProperties3_get_Description(This,pbstrDescription)	\
    ( (This)->lpVtbl -> get_Description(This,pbstrDescription) ) 

#define ProjectProperties3_put_Description(This,Description)	\
    ( (This)->lpVtbl -> put_Description(This,Description) ) 

#define ProjectProperties3_get_Company(This,pbstrCompany)	\
    ( (This)->lpVtbl -> get_Company(This,pbstrCompany) ) 

#define ProjectProperties3_put_Company(This,Company)	\
    ( (This)->lpVtbl -> put_Company(This,Company) ) 

#define ProjectProperties3_get_Product(This,pbstrProduct)	\
    ( (This)->lpVtbl -> get_Product(This,pbstrProduct) ) 

#define ProjectProperties3_put_Product(This,Product)	\
    ( (This)->lpVtbl -> put_Product(This,Product) ) 

#define ProjectProperties3_get_Copyright(This,pbstrCopyright)	\
    ( (This)->lpVtbl -> get_Copyright(This,pbstrCopyright) ) 

#define ProjectProperties3_put_Copyright(This,Copyright)	\
    ( (This)->lpVtbl -> put_Copyright(This,Copyright) ) 

#define ProjectProperties3_get_Trademark(This,pbstrTrademark)	\
    ( (This)->lpVtbl -> get_Trademark(This,pbstrTrademark) ) 

#define ProjectProperties3_put_Trademark(This,Trademark)	\
    ( (This)->lpVtbl -> put_Trademark(This,Trademark) ) 

#define ProjectProperties3_get_AssemblyType(This,pAssemblyType)	\
    ( (This)->lpVtbl -> get_AssemblyType(This,pAssemblyType) ) 

#define ProjectProperties3_put_AssemblyType(This,AssemblyType)	\
    ( (This)->lpVtbl -> put_AssemblyType(This,AssemblyType) ) 

#define ProjectProperties3_get_TypeComplianceDiagnostics(This,pbTypeComplianceDiagnostics)	\
    ( (This)->lpVtbl -> get_TypeComplianceDiagnostics(This,pbTypeComplianceDiagnostics) ) 

#define ProjectProperties3_put_TypeComplianceDiagnostics(This,TypeComplianceDiagnostics)	\
    ( (This)->lpVtbl -> put_TypeComplianceDiagnostics(This,TypeComplianceDiagnostics) ) 

#define ProjectProperties3_get_Win32ResourceFile(This,pbstrW32ResourceFile)	\
    ( (This)->lpVtbl -> get_Win32ResourceFile(This,pbstrW32ResourceFile) ) 

#define ProjectProperties3_put_Win32ResourceFile(This,Win32ResourceFile)	\
    ( (This)->lpVtbl -> put_Win32ResourceFile(This,Win32ResourceFile) ) 

#define ProjectProperties3_get_AssemblyKeyProviderName(This,pbstrKeyProviderName)	\
    ( (This)->lpVtbl -> get_AssemblyKeyProviderName(This,pbstrKeyProviderName) ) 

#define ProjectProperties3_put_AssemblyKeyProviderName(This,KeyProviderName)	\
    ( (This)->lpVtbl -> put_AssemblyKeyProviderName(This,KeyProviderName) ) 

#define ProjectProperties3_get_AssemblyOriginatorKeyFileType(This,pdwOriginatorKeyFileType)	\
    ( (This)->lpVtbl -> get_AssemblyOriginatorKeyFileType(This,pdwOriginatorKeyFileType) ) 

#define ProjectProperties3_put_AssemblyOriginatorKeyFileType(This,OriginatorKeyFileType)	\
    ( (This)->lpVtbl -> put_AssemblyOriginatorKeyFileType(This,OriginatorKeyFileType) ) 

#define ProjectProperties3_get_AssemblyVersion(This,pbstrAssemblyVersion)	\
    ( (This)->lpVtbl -> get_AssemblyVersion(This,pbstrAssemblyVersion) ) 

#define ProjectProperties3_put_AssemblyVersion(This,AssemblyVersion)	\
    ( (This)->lpVtbl -> put_AssemblyVersion(This,AssemblyVersion) ) 

#define ProjectProperties3_get_AssemblyFileVersion(This,pbstrAssemblyFileVersion)	\
    ( (This)->lpVtbl -> get_AssemblyFileVersion(This,pbstrAssemblyFileVersion) ) 

#define ProjectProperties3_put_AssemblyFileVersion(This,AssemblyFileVersion)	\
    ( (This)->lpVtbl -> put_AssemblyFileVersion(This,AssemblyFileVersion) ) 

#define ProjectProperties3_get_GenerateManifests(This,pbGenerateManifests)	\
    ( (This)->lpVtbl -> get_GenerateManifests(This,pbGenerateManifests) ) 

#define ProjectProperties3_put_GenerateManifests(This,GenerateManifests)	\
    ( (This)->lpVtbl -> put_GenerateManifests(This,GenerateManifests) ) 

#define ProjectProperties3_get_EnableSecurityDebugging(This,pbEnableSecurityDebugging)	\
    ( (This)->lpVtbl -> get_EnableSecurityDebugging(This,pbEnableSecurityDebugging) ) 

#define ProjectProperties3_put_EnableSecurityDebugging(This,EnableSecurityDebugging)	\
    ( (This)->lpVtbl -> put_EnableSecurityDebugging(This,EnableSecurityDebugging) ) 

#define ProjectProperties3_get_DebugSecurityZoneURL(This,pbstrSecurityURL)	\
    ( (This)->lpVtbl -> get_DebugSecurityZoneURL(This,pbstrSecurityURL) ) 

#define ProjectProperties3_put_DebugSecurityZoneURL(This,SecurityURL)	\
    ( (This)->lpVtbl -> put_DebugSecurityZoneURL(This,SecurityURL) ) 

#define ProjectProperties3_get_Publish(This,ppdispPublish)	\
    ( (This)->lpVtbl -> get_Publish(This,ppdispPublish) ) 

#define ProjectProperties3_get_ComVisible(This,pbComVisible)	\
    ( (This)->lpVtbl -> get_ComVisible(This,pbComVisible) ) 

#define ProjectProperties3_put_ComVisible(This,ComVisible)	\
    ( (This)->lpVtbl -> put_ComVisible(This,ComVisible) ) 

#define ProjectProperties3_get_AssemblyGuid(This,pbstrAssemblyGuid)	\
    ( (This)->lpVtbl -> get_AssemblyGuid(This,pbstrAssemblyGuid) ) 

#define ProjectProperties3_put_AssemblyGuid(This,AssemblyGuid)	\
    ( (This)->lpVtbl -> put_AssemblyGuid(This,AssemblyGuid) ) 

#define ProjectProperties3_get_NeutralResourcesLanguage(This,pbstrNeutralResourcesLanguage)	\
    ( (This)->lpVtbl -> get_NeutralResourcesLanguage(This,pbstrNeutralResourcesLanguage) ) 

#define ProjectProperties3_put_NeutralResourcesLanguage(This,NeutralResourcesLanguage)	\
    ( (This)->lpVtbl -> put_NeutralResourcesLanguage(This,NeutralResourcesLanguage) ) 

#define ProjectProperties3_get_SignAssembly(This,pbSignAssembly)	\
    ( (This)->lpVtbl -> get_SignAssembly(This,pbSignAssembly) ) 

#define ProjectProperties3_put_SignAssembly(This,SignAssembly)	\
    ( (This)->lpVtbl -> put_SignAssembly(This,SignAssembly) ) 

#define ProjectProperties3_get_SignManifests(This,pbSignManifests)	\
    ( (This)->lpVtbl -> get_SignManifests(This,pbSignManifests) ) 

#define ProjectProperties3_put_SignManifests(This,bSignManifests)	\
    ( (This)->lpVtbl -> put_SignManifests(This,bSignManifests) ) 

#define ProjectProperties3_get_TargetZone(This,pbstrTargetZone)	\
    ( (This)->lpVtbl -> get_TargetZone(This,pbstrTargetZone) ) 

#define ProjectProperties3_put_TargetZone(This,TargetZone)	\
    ( (This)->lpVtbl -> put_TargetZone(This,TargetZone) ) 

#define ProjectProperties3_get_ExcludedPermissions(This,pbstrExcludedPermissions)	\
    ( (This)->lpVtbl -> get_ExcludedPermissions(This,pbstrExcludedPermissions) ) 

#define ProjectProperties3_put_ExcludedPermissions(This,ExcludedPermissions)	\
    ( (This)->lpVtbl -> put_ExcludedPermissions(This,ExcludedPermissions) ) 

#define ProjectProperties3_get_ManifestCertificateThumbprint(This,pbstrManifestCertificateThumbprint)	\
    ( (This)->lpVtbl -> get_ManifestCertificateThumbprint(This,pbstrManifestCertificateThumbprint) ) 

#define ProjectProperties3_put_ManifestCertificateThumbprint(This,ManifestCertificateThumbprint)	\
    ( (This)->lpVtbl -> put_ManifestCertificateThumbprint(This,ManifestCertificateThumbprint) ) 

#define ProjectProperties3_get_ManifestKeyFile(This,pbstrManifestKeyFile)	\
    ( (This)->lpVtbl -> get_ManifestKeyFile(This,pbstrManifestKeyFile) ) 

#define ProjectProperties3_put_ManifestKeyFile(This,ManifestKeyFile)	\
    ( (This)->lpVtbl -> put_ManifestKeyFile(This,ManifestKeyFile) ) 

#define ProjectProperties3_get_ManifestTimestampUrl(This,pbstrManifestTimestampUrl)	\
    ( (This)->lpVtbl -> get_ManifestTimestampUrl(This,pbstrManifestTimestampUrl) ) 

#define ProjectProperties3_put_ManifestTimestampUrl(This,ManifestTimestampUrl)	\
    ( (This)->lpVtbl -> put_ManifestTimestampUrl(This,ManifestTimestampUrl) ) 

#endif /* COBJMACROS */


#endif 	/* C style interface */




#endif 	/* __ProjectProperties3_INTERFACE_DEFINED__ */


#ifndef __VBProjectProperties3_INTERFACE_DEFINED__
#define __VBProjectProperties3_INTERFACE_DEFINED__

/* interface VBProjectProperties3 */
/* [object][dual][unique][helpstring][uuid] */ 


EXTERN_C const IID IID_VBProjectProperties3;

#if defined(__cplusplus) && !defined(CINTERFACE)
    
    MIDL_INTERFACE("A93BFE7B-CFDA-471D-93C6-1747D0B06E8E")
    VBProjectProperties3 : public ProjectProperties3
    {
    public:
        virtual /* [helpstring][id][nonbrowsable][propget] */ HRESULT STDMETHODCALLTYPE get_MyApplication( 
            /* [retval][out] */ __RPC__deref_out_opt IDispatch **ppdispMyApplication) = 0;
        
        virtual /* [helpstring][id][propget] */ HRESULT STDMETHODCALLTYPE get_MyType( 
            /* [retval][out] */ __RPC__deref_out_opt BSTR *pbstrMyType) = 0;
        
        virtual /* [helpstring][id][propput] */ HRESULT STDMETHODCALLTYPE put_MyType( 
            /* [in] */ __RPC__in BSTR MyType) = 0;
        
    };
    
#else 	/* C style interface */

    typedef struct VBProjectProperties3Vtbl
    {
        BEGIN_INTERFACE
        
        /* [id][restricted][funcdescattr] */ HRESULT ( STDMETHODCALLTYPE *QueryInterface )( 
            VBProjectProperties3 * This,
            /* [in][idldescattr] */ __RPC__in struct GUID *riid,
            /* [out][idldescattr] */ __RPC__deref_out_opt void **ppvObj,
            /* [retval][out] */ __RPC__out void *retval);
        
        /* [id][restricted][funcdescattr] */ HRESULT ( STDMETHODCALLTYPE *AddRef )( 
            VBProjectProperties3 * This,
            /* [retval][out] */ __RPC__out unsigned long *retval);
        
        /* [id][restricted][funcdescattr] */ HRESULT ( STDMETHODCALLTYPE *Release )( 
            VBProjectProperties3 * This,
            /* [retval][out] */ __RPC__out unsigned long *retval);
        
        /* [id][restricted][funcdescattr] */ HRESULT ( STDMETHODCALLTYPE *GetTypeInfoCount )( 
            VBProjectProperties3 * This,
            /* [out][idldescattr] */ __RPC__out unsigned UINT *pctinfo,
            /* [retval][out] */ __RPC__out void *retval);
        
        /* [id][restricted][funcdescattr] */ HRESULT ( STDMETHODCALLTYPE *GetTypeInfo )( 
            VBProjectProperties3 * This,
            /* [in][idldescattr] */ unsigned UINT itinfo,
            /* [in][idldescattr] */ unsigned long lcid,
            /* [out][idldescattr] */ __RPC__deref_out_opt void **pptinfo,
            /* [retval][out] */ __RPC__out void *retval);
        
        /* [id][restricted][funcdescattr] */ HRESULT ( STDMETHODCALLTYPE *GetIDsOfNames )( 
            VBProjectProperties3 * This,
            /* [in][idldescattr] */ __RPC__in struct GUID *riid,
            /* [in][idldescattr] */ __RPC__deref_in_opt signed char **rgszNames,
            /* [in][idldescattr] */ unsigned UINT cNames,
            /* [in][idldescattr] */ unsigned long lcid,
            /* [out][idldescattr] */ __RPC__out signed long *rgdispid,
            /* [retval][out] */ __RPC__out void *retval);
        
        /* [id][restricted][funcdescattr] */ HRESULT ( STDMETHODCALLTYPE *Invoke )( 
            VBProjectProperties3 * This,
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
            VBProjectProperties3 * This,
            /* [retval][out] */ __RPC__deref_out_opt BSTR *retval);
        
        /* [id][propget][hidden][funcdescattr] */ HRESULT ( STDMETHODCALLTYPE *get___project )( 
            VBProjectProperties3 * This,
            /* [retval][out] */ __RPC__deref_out_opt IUnknown **retval);
        
        /* [id][propget][funcdescattr] */ HRESULT ( STDMETHODCALLTYPE *get_StartupObject )( 
            VBProjectProperties3 * This,
            /* [retval][out] */ __RPC__deref_out_opt BSTR *retval);
        
        /* [id][propput][funcdescattr] */ HRESULT ( STDMETHODCALLTYPE *put_StartupObject )( 
            VBProjectProperties3 * This,
            /* [in][idldescattr] */ __RPC__in BSTR noname,
            /* [retval][out] */ __RPC__out void *retval);
        
        /* [id][propget][funcdescattr] */ HRESULT ( STDMETHODCALLTYPE *get_OutputType )( 
            VBProjectProperties3 * This,
            /* [retval][out] */ __RPC__out enum prjOutputType *retval);
        
        /* [id][propput][funcdescattr] */ HRESULT ( STDMETHODCALLTYPE *put_OutputType )( 
            VBProjectProperties3 * This,
            /* [in][idldescattr] */ enum prjOutputType noname,
            /* [retval][out] */ __RPC__out void *retval);
        
        /* [id][propget][funcdescattr] */ HRESULT ( STDMETHODCALLTYPE *get_RootNamespace )( 
            VBProjectProperties3 * This,
            /* [retval][out] */ __RPC__deref_out_opt BSTR *retval);
        
        /* [id][propput][funcdescattr] */ HRESULT ( STDMETHODCALLTYPE *put_RootNamespace )( 
            VBProjectProperties3 * This,
            /* [in][idldescattr] */ __RPC__in BSTR noname,
            /* [retval][out] */ __RPC__out void *retval);
        
        /* [id][propget][funcdescattr] */ HRESULT ( STDMETHODCALLTYPE *get_AssemblyName )( 
            VBProjectProperties3 * This,
            /* [retval][out] */ __RPC__deref_out_opt BSTR *retval);
        
        /* [id][propput][funcdescattr] */ HRESULT ( STDMETHODCALLTYPE *put_AssemblyName )( 
            VBProjectProperties3 * This,
            /* [in][idldescattr] */ __RPC__in BSTR noname,
            /* [retval][out] */ __RPC__out void *retval);
        
        /* [id][propget][funcdescattr] */ HRESULT ( STDMETHODCALLTYPE *get_AssemblyOriginatorKeyFile )( 
            VBProjectProperties3 * This,
            /* [retval][out] */ __RPC__deref_out_opt BSTR *retval);
        
        /* [id][propput][funcdescattr] */ HRESULT ( STDMETHODCALLTYPE *put_AssemblyOriginatorKeyFile )( 
            VBProjectProperties3 * This,
            /* [in][idldescattr] */ __RPC__in BSTR noname,
            /* [retval][out] */ __RPC__out void *retval);
        
        /* [id][propget][funcdescattr] */ HRESULT ( STDMETHODCALLTYPE *get_AssemblyKeyContainerName )( 
            VBProjectProperties3 * This,
            /* [retval][out] */ __RPC__deref_out_opt BSTR *retval);
        
        /* [id][propput][funcdescattr] */ HRESULT ( STDMETHODCALLTYPE *put_AssemblyKeyContainerName )( 
            VBProjectProperties3 * This,
            /* [in][idldescattr] */ __RPC__in BSTR noname,
            /* [retval][out] */ __RPC__out void *retval);
        
        /* [id][propget][funcdescattr] */ HRESULT ( STDMETHODCALLTYPE *get_AssemblyOriginatorKeyMode )( 
            VBProjectProperties3 * This,
            /* [retval][out] */ __RPC__out enum prjOriginatorKeyMode *retval);
        
        /* [id][propput][funcdescattr] */ HRESULT ( STDMETHODCALLTYPE *put_AssemblyOriginatorKeyMode )( 
            VBProjectProperties3 * This,
            /* [in][idldescattr] */ enum prjOriginatorKeyMode noname,
            /* [retval][out] */ __RPC__out void *retval);
        
        /* [id][propget][funcdescattr] */ HRESULT ( STDMETHODCALLTYPE *get_DelaySign )( 
            VBProjectProperties3 * This,
            /* [retval][out] */ __RPC__out BOOLEAN *retval);
        
        /* [id][propput][funcdescattr] */ HRESULT ( STDMETHODCALLTYPE *put_DelaySign )( 
            VBProjectProperties3 * This,
            /* [in][idldescattr] */ BOOLEAN noname,
            /* [retval][out] */ __RPC__out void *retval);
        
        /* [id][propget][funcdescattr] */ HRESULT ( STDMETHODCALLTYPE *get_WebServer )( 
            VBProjectProperties3 * This,
            /* [retval][out] */ __RPC__deref_out_opt BSTR *retval);
        
        /* [id][propget][funcdescattr] */ HRESULT ( STDMETHODCALLTYPE *get_WebServerVersion )( 
            VBProjectProperties3 * This,
            /* [retval][out] */ __RPC__deref_out_opt BSTR *retval);
        
        /* [id][propget][funcdescattr] */ HRESULT ( STDMETHODCALLTYPE *get_ServerExtensionsVersion )( 
            VBProjectProperties3 * This,
            /* [retval][out] */ __RPC__deref_out_opt BSTR *retval);
        
        /* [id][propget][funcdescattr] */ HRESULT ( STDMETHODCALLTYPE *get_LinkRepair )( 
            VBProjectProperties3 * This,
            /* [retval][out] */ __RPC__out BOOLEAN *retval);
        
        /* [id][propput][funcdescattr] */ HRESULT ( STDMETHODCALLTYPE *put_LinkRepair )( 
            VBProjectProperties3 * This,
            /* [in][idldescattr] */ BOOLEAN noname,
            /* [retval][out] */ __RPC__out void *retval);
        
        /* [id][propget][funcdescattr] */ HRESULT ( STDMETHODCALLTYPE *get_OfflineURL )( 
            VBProjectProperties3 * This,
            /* [retval][out] */ __RPC__deref_out_opt BSTR *retval);
        
        /* [id][propget][funcdescattr] */ HRESULT ( STDMETHODCALLTYPE *get_FileSharePath )( 
            VBProjectProperties3 * This,
            /* [retval][out] */ __RPC__deref_out_opt BSTR *retval);
        
        /* [id][propput][funcdescattr] */ HRESULT ( STDMETHODCALLTYPE *put_FileSharePath )( 
            VBProjectProperties3 * This,
            /* [in][idldescattr] */ __RPC__in BSTR noname,
            /* [retval][out] */ __RPC__out void *retval);
        
        /* [id][propget][funcdescattr] */ HRESULT ( STDMETHODCALLTYPE *get_ActiveFileSharePath )( 
            VBProjectProperties3 * This,
            /* [retval][out] */ __RPC__deref_out_opt BSTR *retval);
        
        /* [id][propget][funcdescattr] */ HRESULT ( STDMETHODCALLTYPE *get_WebAccessMethod )( 
            VBProjectProperties3 * This,
            /* [retval][out] */ __RPC__out enum prjWebAccessMethod *retval);
        
        /* [id][propput][funcdescattr] */ HRESULT ( STDMETHODCALLTYPE *put_WebAccessMethod )( 
            VBProjectProperties3 * This,
            /* [in][idldescattr] */ enum prjWebAccessMethod noname,
            /* [retval][out] */ __RPC__out void *retval);
        
        /* [id][propget][funcdescattr] */ HRESULT ( STDMETHODCALLTYPE *get_ActiveWebAccessMethod )( 
            VBProjectProperties3 * This,
            /* [retval][out] */ __RPC__out enum prjWebAccessMethod *retval);
        
        /* [id][propget][funcdescattr] */ HRESULT ( STDMETHODCALLTYPE *get_DefaultClientScript )( 
            VBProjectProperties3 * This,
            /* [retval][out] */ __RPC__out enum prjScriptLanguage *retval);
        
        /* [id][propput][funcdescattr] */ HRESULT ( STDMETHODCALLTYPE *put_DefaultClientScript )( 
            VBProjectProperties3 * This,
            /* [in][idldescattr] */ enum prjScriptLanguage noname,
            /* [retval][out] */ __RPC__out void *retval);
        
        /* [id][propget][funcdescattr] */ HRESULT ( STDMETHODCALLTYPE *get_DefaultTargetSchema )( 
            VBProjectProperties3 * This,
            /* [retval][out] */ __RPC__out enum prjTargetSchema *retval);
        
        /* [id][propput][funcdescattr] */ HRESULT ( STDMETHODCALLTYPE *put_DefaultTargetSchema )( 
            VBProjectProperties3 * This,
            /* [in][idldescattr] */ enum prjTargetSchema noname,
            /* [retval][out] */ __RPC__out void *retval);
        
        /* [id][propget][funcdescattr] */ HRESULT ( STDMETHODCALLTYPE *get_DefaultHTMLPageLayout )( 
            VBProjectProperties3 * This,
            /* [retval][out] */ __RPC__out enum prjHTMLPageLayout *retval);
        
        /* [id][propput][funcdescattr] */ HRESULT ( STDMETHODCALLTYPE *put_DefaultHTMLPageLayout )( 
            VBProjectProperties3 * This,
            /* [in][idldescattr] */ enum prjHTMLPageLayout noname,
            /* [retval][out] */ __RPC__out void *retval);
        
        /* [id][propget][funcdescattr] */ HRESULT ( STDMETHODCALLTYPE *get_FileName )( 
            VBProjectProperties3 * This,
            /* [retval][out] */ __RPC__deref_out_opt BSTR *retval);
        
        /* [id][propput][funcdescattr] */ HRESULT ( STDMETHODCALLTYPE *put_FileName )( 
            VBProjectProperties3 * This,
            /* [in][idldescattr] */ __RPC__in BSTR noname,
            /* [retval][out] */ __RPC__out void *retval);
        
        /* [id][propget][funcdescattr] */ HRESULT ( STDMETHODCALLTYPE *get_FullPath )( 
            VBProjectProperties3 * This,
            /* [retval][out] */ __RPC__deref_out_opt BSTR *retval);
        
        /* [id][propget][funcdescattr] */ HRESULT ( STDMETHODCALLTYPE *get_LocalPath )( 
            VBProjectProperties3 * This,
            /* [retval][out] */ __RPC__deref_out_opt BSTR *retval);
        
        /* [id][propget][funcdescattr] */ HRESULT ( STDMETHODCALLTYPE *get_URL )( 
            VBProjectProperties3 * This,
            /* [retval][out] */ __RPC__deref_out_opt BSTR *retval);
        
        /* [id][propget][funcdescattr] */ HRESULT ( STDMETHODCALLTYPE *get_ActiveConfigurationSettings )( 
            VBProjectProperties3 * This,
            /* [retval][out] */ __RPC__deref_out_opt ProjectConfigurationProperties **retval);
        
        /* [id][propget][funcdescattr] */ HRESULT ( STDMETHODCALLTYPE *get_Extender )( 
            VBProjectProperties3 * This,
            /* [in][idldescattr] */ __RPC__in BSTR ExtenderName,
            /* [retval][out] */ __RPC__deref_out_opt IDispatch **retval);
        
        /* [id][propget][funcdescattr] */ HRESULT ( STDMETHODCALLTYPE *get_ExtenderNames )( 
            VBProjectProperties3 * This,
            /* [retval][out] */ __RPC__out VARIANT *retval);
        
        /* [id][propget][funcdescattr] */ HRESULT ( STDMETHODCALLTYPE *get_ExtenderCATID )( 
            VBProjectProperties3 * This,
            /* [retval][out] */ __RPC__deref_out_opt BSTR *retval);
        
        /* [id][propget][funcdescattr] */ HRESULT ( STDMETHODCALLTYPE *get_ApplicationIcon )( 
            VBProjectProperties3 * This,
            /* [retval][out] */ __RPC__deref_out_opt BSTR *retval);
        
        /* [id][propput][funcdescattr] */ HRESULT ( STDMETHODCALLTYPE *put_ApplicationIcon )( 
            VBProjectProperties3 * This,
            /* [in][idldescattr] */ __RPC__in BSTR noname,
            /* [retval][out] */ __RPC__out void *retval);
        
        /* [id][propget][funcdescattr] */ HRESULT ( STDMETHODCALLTYPE *get_OptionStrict )( 
            VBProjectProperties3 * This,
            /* [retval][out] */ __RPC__out enum prjOptionStrict *retval);
        
        /* [id][propput][funcdescattr] */ HRESULT ( STDMETHODCALLTYPE *put_OptionStrict )( 
            VBProjectProperties3 * This,
            /* [in][idldescattr] */ enum prjOptionStrict noname,
            /* [retval][out] */ __RPC__out void *retval);
        
        /* [id][propget][funcdescattr] */ HRESULT ( STDMETHODCALLTYPE *get_ReferencePath )( 
            VBProjectProperties3 * This,
            /* [retval][out] */ __RPC__deref_out_opt BSTR *retval);
        
        /* [id][propput][funcdescattr] */ HRESULT ( STDMETHODCALLTYPE *put_ReferencePath )( 
            VBProjectProperties3 * This,
            /* [in][idldescattr] */ __RPC__in BSTR noname,
            /* [retval][out] */ __RPC__out void *retval);
        
        /* [id][propget][funcdescattr] */ HRESULT ( STDMETHODCALLTYPE *get_OutputFileName )( 
            VBProjectProperties3 * This,
            /* [retval][out] */ __RPC__deref_out_opt BSTR *retval);
        
        /* [id][propget][hidden][funcdescattr] */ HRESULT ( STDMETHODCALLTYPE *get_AbsoluteProjectDirectory )( 
            VBProjectProperties3 * This,
            /* [retval][out] */ __RPC__deref_out_opt BSTR *retval);
        
        /* [id][propget][funcdescattr] */ HRESULT ( STDMETHODCALLTYPE *get_OptionExplicit )( 
            VBProjectProperties3 * This,
            /* [retval][out] */ __RPC__out enum prjOptionExplicit *retval);
        
        /* [id][propput][funcdescattr] */ HRESULT ( STDMETHODCALLTYPE *put_OptionExplicit )( 
            VBProjectProperties3 * This,
            /* [in][idldescattr] */ enum prjOptionExplicit noname,
            /* [retval][out] */ __RPC__out void *retval);
        
        /* [id][propget][funcdescattr] */ HRESULT ( STDMETHODCALLTYPE *get_OptionCompare )( 
            VBProjectProperties3 * This,
            /* [retval][out] */ __RPC__out enum prjCompare *retval);
        
        /* [id][propput][funcdescattr] */ HRESULT ( STDMETHODCALLTYPE *put_OptionCompare )( 
            VBProjectProperties3 * This,
            /* [in][idldescattr] */ enum prjCompare noname,
            /* [retval][out] */ __RPC__out void *retval);
        
        /* [id][propget][funcdescattr] */ HRESULT ( STDMETHODCALLTYPE *get_ProjectType )( 
            VBProjectProperties3 * This,
            /* [retval][out] */ __RPC__out enum prjProjectType *retval);
        
        /* [id][propget][funcdescattr] */ HRESULT ( STDMETHODCALLTYPE *get_DefaultNamespace )( 
            VBProjectProperties3 * This,
            /* [retval][out] */ __RPC__deref_out_opt BSTR *retval);
        
        /* [id][propput][funcdescattr] */ HRESULT ( STDMETHODCALLTYPE *put_DefaultNamespace )( 
            VBProjectProperties3 * This,
            /* [in][idldescattr] */ __RPC__in BSTR noname,
            /* [retval][out] */ __RPC__out void *retval);
        
        /* [id][propget][funcdescattr] */ HRESULT ( STDMETHODCALLTYPE *get_PreBuildEvent )( 
            VBProjectProperties3 * This,
            /* [retval][out] */ __RPC__deref_out_opt BSTR *retval);
        
        /* [id][propput][funcdescattr] */ HRESULT ( STDMETHODCALLTYPE *put_PreBuildEvent )( 
            VBProjectProperties3 * This,
            /* [in][idldescattr] */ __RPC__in BSTR noname,
            /* [retval][out] */ __RPC__out void *retval);
        
        /* [id][propget][funcdescattr] */ HRESULT ( STDMETHODCALLTYPE *get_PostBuildEvent )( 
            VBProjectProperties3 * This,
            /* [retval][out] */ __RPC__deref_out_opt BSTR *retval);
        
        /* [id][propput][funcdescattr] */ HRESULT ( STDMETHODCALLTYPE *put_PostBuildEvent )( 
            VBProjectProperties3 * This,
            /* [in][idldescattr] */ __RPC__in BSTR noname,
            /* [retval][out] */ __RPC__out void *retval);
        
        /* [id][propget][funcdescattr] */ HRESULT ( STDMETHODCALLTYPE *get_RunPostBuildEvent )( 
            VBProjectProperties3 * This,
            /* [retval][out] */ __RPC__out enum prjRunPostBuildEvent *retval);
        
        /* [id][propput][funcdescattr] */ HRESULT ( STDMETHODCALLTYPE *put_RunPostBuildEvent )( 
            VBProjectProperties3 * This,
            /* [in][idldescattr] */ enum prjRunPostBuildEvent noname,
            /* [retval][out] */ __RPC__out void *retval);
        
        /* [id][propget][funcdescattr] */ HRESULT ( STDMETHODCALLTYPE *get_AspnetVersion )( 
            VBProjectProperties3 * This,
            /* [retval][out] */ __RPC__deref_out_opt BSTR *retval);
        
        /* [helpstring][id][nonbrowsable][propget] */ HRESULT ( STDMETHODCALLTYPE *get_Title )( 
            VBProjectProperties3 * This,
            /* [retval][out] */ __RPC__deref_out_opt BSTR *pbstrTitle);
        
        /* [helpstring][id][nonbrowsable][propput] */ HRESULT ( STDMETHODCALLTYPE *put_Title )( 
            VBProjectProperties3 * This,
            /* [in] */ __RPC__in BSTR Title);
        
        /* [helpstring][id][nonbrowsable][propget] */ HRESULT ( STDMETHODCALLTYPE *get_Description )( 
            VBProjectProperties3 * This,
            /* [retval][out] */ __RPC__deref_out_opt BSTR *pbstrDescription);
        
        /* [helpstring][id][nonbrowsable][propput] */ HRESULT ( STDMETHODCALLTYPE *put_Description )( 
            VBProjectProperties3 * This,
            /* [in] */ __RPC__in BSTR Description);
        
        /* [helpstring][id][nonbrowsable][propget] */ HRESULT ( STDMETHODCALLTYPE *get_Company )( 
            VBProjectProperties3 * This,
            /* [retval][out] */ __RPC__deref_out_opt BSTR *pbstrCompany);
        
        /* [helpstring][id][nonbrowsable][propput] */ HRESULT ( STDMETHODCALLTYPE *put_Company )( 
            VBProjectProperties3 * This,
            /* [in] */ __RPC__in BSTR Company);
        
        /* [helpstring][id][nonbrowsable][propget] */ HRESULT ( STDMETHODCALLTYPE *get_Product )( 
            VBProjectProperties3 * This,
            /* [retval][out] */ __RPC__deref_out_opt BSTR *pbstrProduct);
        
        /* [helpstring][id][nonbrowsable][propput] */ HRESULT ( STDMETHODCALLTYPE *put_Product )( 
            VBProjectProperties3 * This,
            /* [in] */ __RPC__in BSTR Product);
        
        /* [helpstring][id][nonbrowsable][propget] */ HRESULT ( STDMETHODCALLTYPE *get_Copyright )( 
            VBProjectProperties3 * This,
            /* [retval][out] */ __RPC__deref_out_opt BSTR *pbstrCopyright);
        
        /* [helpstring][id][nonbrowsable][propput] */ HRESULT ( STDMETHODCALLTYPE *put_Copyright )( 
            VBProjectProperties3 * This,
            /* [in] */ __RPC__in BSTR Copyright);
        
        /* [helpstring][id][nonbrowsable][propget] */ HRESULT ( STDMETHODCALLTYPE *get_Trademark )( 
            VBProjectProperties3 * This,
            /* [retval][out] */ __RPC__deref_out_opt BSTR *pbstrTrademark);
        
        /* [helpstring][id][nonbrowsable][propput] */ HRESULT ( STDMETHODCALLTYPE *put_Trademark )( 
            VBProjectProperties3 * This,
            /* [in] */ __RPC__in BSTR Trademark);
        
        /* [helpstring][id][propget] */ HRESULT ( STDMETHODCALLTYPE *get_AssemblyType )( 
            VBProjectProperties3 * This,
            /* [retval][out] */ __RPC__out prjAssemblyType *pAssemblyType);
        
        /* [helpstring][id][propput] */ HRESULT ( STDMETHODCALLTYPE *put_AssemblyType )( 
            VBProjectProperties3 * This,
            /* [in] */ prjAssemblyType AssemblyType);
        
        /* [helpstring][id][propget] */ HRESULT ( STDMETHODCALLTYPE *get_TypeComplianceDiagnostics )( 
            VBProjectProperties3 * This,
            /* [retval][out] */ __RPC__out VARIANT_BOOL *pbTypeComplianceDiagnostics);
        
        /* [helpstring][id][propput] */ HRESULT ( STDMETHODCALLTYPE *put_TypeComplianceDiagnostics )( 
            VBProjectProperties3 * This,
            /* [in] */ VARIANT_BOOL TypeComplianceDiagnostics);
        
        /* [helpstring][id][propget] */ HRESULT ( STDMETHODCALLTYPE *get_Win32ResourceFile )( 
            VBProjectProperties3 * This,
            /* [retval][out] */ __RPC__deref_out_opt BSTR *pbstrW32ResourceFile);
        
        /* [helpstring][id][propput] */ HRESULT ( STDMETHODCALLTYPE *put_Win32ResourceFile )( 
            VBProjectProperties3 * This,
            /* [in] */ __RPC__in BSTR Win32ResourceFile);
        
        /* [helpstring][id][propget] */ HRESULT ( STDMETHODCALLTYPE *get_AssemblyKeyProviderName )( 
            VBProjectProperties3 * This,
            /* [retval][out] */ __RPC__deref_out_opt BSTR *pbstrKeyProviderName);
        
        /* [helpstring][id][propput] */ HRESULT ( STDMETHODCALLTYPE *put_AssemblyKeyProviderName )( 
            VBProjectProperties3 * This,
            /* [in] */ __RPC__in BSTR KeyProviderName);
        
        /* [helpstring][id][propget] */ HRESULT ( STDMETHODCALLTYPE *get_AssemblyOriginatorKeyFileType )( 
            VBProjectProperties3 * This,
            /* [retval][out] */ __RPC__out DWORD *pdwOriginatorKeyFileType);
        
        /* [helpstring][id][propput] */ HRESULT ( STDMETHODCALLTYPE *put_AssemblyOriginatorKeyFileType )( 
            VBProjectProperties3 * This,
            /* [in] */ DWORD OriginatorKeyFileType);
        
        /* [helpstring][id][nonbrowsable][propget] */ HRESULT ( STDMETHODCALLTYPE *get_AssemblyVersion )( 
            VBProjectProperties3 * This,
            /* [retval][out] */ __RPC__deref_out_opt BSTR *pbstrAssemblyVersion);
        
        /* [helpstring][id][nonbrowsable][propput] */ HRESULT ( STDMETHODCALLTYPE *put_AssemblyVersion )( 
            VBProjectProperties3 * This,
            /* [in] */ __RPC__in BSTR AssemblyVersion);
        
        /* [helpstring][id][nonbrowsable][propget] */ HRESULT ( STDMETHODCALLTYPE *get_AssemblyFileVersion )( 
            VBProjectProperties3 * This,
            /* [retval][out] */ __RPC__deref_out_opt BSTR *pbstrAssemblyFileVersion);
        
        /* [helpstring][id][nonbrowsable][propput] */ HRESULT ( STDMETHODCALLTYPE *put_AssemblyFileVersion )( 
            VBProjectProperties3 * This,
            /* [in] */ __RPC__in BSTR AssemblyFileVersion);
        
        /* [helpstring][id][propget] */ HRESULT ( STDMETHODCALLTYPE *get_GenerateManifests )( 
            VBProjectProperties3 * This,
            /* [retval][out] */ __RPC__out VARIANT_BOOL *pbGenerateManifests);
        
        /* [helpstring][id][propput] */ HRESULT ( STDMETHODCALLTYPE *put_GenerateManifests )( 
            VBProjectProperties3 * This,
            /* [in] */ VARIANT_BOOL GenerateManifests);
        
        /* [helpstring][id][propget] */ HRESULT ( STDMETHODCALLTYPE *get_EnableSecurityDebugging )( 
            VBProjectProperties3 * This,
            /* [retval][out] */ __RPC__out VARIANT_BOOL *pbEnableSecurityDebugging);
        
        /* [helpstring][id][propput] */ HRESULT ( STDMETHODCALLTYPE *put_EnableSecurityDebugging )( 
            VBProjectProperties3 * This,
            /* [in] */ VARIANT_BOOL EnableSecurityDebugging);
        
        /* [helpstring][id][propget] */ HRESULT ( STDMETHODCALLTYPE *get_DebugSecurityZoneURL )( 
            VBProjectProperties3 * This,
            /* [retval][out] */ __RPC__deref_out_opt BSTR *pbstrSecurityURL);
        
        /* [helpstring][id][propput] */ HRESULT ( STDMETHODCALLTYPE *put_DebugSecurityZoneURL )( 
            VBProjectProperties3 * This,
            /* [in] */ __RPC__in BSTR SecurityURL);
        
        /* [helpstring][id][nonbrowsable][propget] */ HRESULT ( STDMETHODCALLTYPE *get_Publish )( 
            VBProjectProperties3 * This,
            /* [retval][out] */ __RPC__deref_out_opt IDispatch **ppdispPublish);
        
        /* [helpstring][id][nonbrowsable][propget] */ HRESULT ( STDMETHODCALLTYPE *get_ComVisible )( 
            VBProjectProperties3 * This,
            /* [retval][out] */ __RPC__out VARIANT_BOOL *pbComVisible);
        
        /* [helpstring][id][nonbrowsable][propput] */ HRESULT ( STDMETHODCALLTYPE *put_ComVisible )( 
            VBProjectProperties3 * This,
            /* [in] */ VARIANT_BOOL ComVisible);
        
        /* [helpstring][id][nonbrowsable][propget] */ HRESULT ( STDMETHODCALLTYPE *get_AssemblyGuid )( 
            VBProjectProperties3 * This,
            /* [retval][out] */ __RPC__deref_out_opt BSTR *pbstrAssemblyGuid);
        
        /* [helpstring][id][nonbrowsable][propput] */ HRESULT ( STDMETHODCALLTYPE *put_AssemblyGuid )( 
            VBProjectProperties3 * This,
            /* [in] */ __RPC__in BSTR AssemblyGuid);
        
        /* [helpstring][id][nonbrowsable][propget] */ HRESULT ( STDMETHODCALLTYPE *get_NeutralResourcesLanguage )( 
            VBProjectProperties3 * This,
            /* [retval][out] */ __RPC__deref_out_opt BSTR *pbstrNeutralResourcesLanguage);
        
        /* [helpstring][id][nonbrowsable][propput] */ HRESULT ( STDMETHODCALLTYPE *put_NeutralResourcesLanguage )( 
            VBProjectProperties3 * This,
            /* [in] */ __RPC__in BSTR NeutralResourcesLanguage);
        
        /* [helpstring][id][propget] */ HRESULT ( STDMETHODCALLTYPE *get_SignAssembly )( 
            VBProjectProperties3 * This,
            /* [retval][out] */ __RPC__out VARIANT_BOOL *pbSignAssembly);
        
        /* [helpstring][id][propput] */ HRESULT ( STDMETHODCALLTYPE *put_SignAssembly )( 
            VBProjectProperties3 * This,
            /* [in] */ VARIANT_BOOL SignAssembly);
        
        /* [helpstring][id][propget] */ HRESULT ( STDMETHODCALLTYPE *get_SignManifests )( 
            VBProjectProperties3 * This,
            /* [retval][out] */ __RPC__out VARIANT_BOOL *pbSignManifests);
        
        /* [helpstring][id][propput] */ HRESULT ( STDMETHODCALLTYPE *put_SignManifests )( 
            VBProjectProperties3 * This,
            /* [in] */ VARIANT_BOOL bSignManifests);
        
        /* [helpstring][id][propget] */ HRESULT ( STDMETHODCALLTYPE *get_TargetZone )( 
            VBProjectProperties3 * This,
            /* [retval][out] */ __RPC__deref_out_opt BSTR *pbstrTargetZone);
        
        /* [helpstring][id][propput] */ HRESULT ( STDMETHODCALLTYPE *put_TargetZone )( 
            VBProjectProperties3 * This,
            /* [in] */ __RPC__in BSTR TargetZone);
        
        /* [helpstring][id][propget] */ HRESULT ( STDMETHODCALLTYPE *get_ExcludedPermissions )( 
            VBProjectProperties3 * This,
            /* [retval][out] */ __RPC__deref_out_opt BSTR *pbstrExcludedPermissions);
        
        /* [helpstring][id][propput] */ HRESULT ( STDMETHODCALLTYPE *put_ExcludedPermissions )( 
            VBProjectProperties3 * This,
            /* [in] */ __RPC__in BSTR ExcludedPermissions);
        
        /* [helpstring][id][propget] */ HRESULT ( STDMETHODCALLTYPE *get_ManifestCertificateThumbprint )( 
            VBProjectProperties3 * This,
            /* [retval][out] */ __RPC__deref_out_opt BSTR *pbstrManifestCertificateThumbprint);
        
        /* [helpstring][id][propput] */ HRESULT ( STDMETHODCALLTYPE *put_ManifestCertificateThumbprint )( 
            VBProjectProperties3 * This,
            /* [in] */ __RPC__in BSTR ManifestCertificateThumbprint);
        
        /* [helpstring][id][propget] */ HRESULT ( STDMETHODCALLTYPE *get_ManifestKeyFile )( 
            VBProjectProperties3 * This,
            /* [retval][out] */ __RPC__deref_out_opt BSTR *pbstrManifestKeyFile);
        
        /* [helpstring][id][propput] */ HRESULT ( STDMETHODCALLTYPE *put_ManifestKeyFile )( 
            VBProjectProperties3 * This,
            /* [in] */ __RPC__in BSTR ManifestKeyFile);
        
        /* [helpstring][id][propget] */ HRESULT ( STDMETHODCALLTYPE *get_ManifestTimestampUrl )( 
            VBProjectProperties3 * This,
            /* [retval][out] */ __RPC__deref_out_opt BSTR *pbstrManifestTimestampUrl);
        
        /* [helpstring][id][propput] */ HRESULT ( STDMETHODCALLTYPE *put_ManifestTimestampUrl )( 
            VBProjectProperties3 * This,
            /* [in] */ __RPC__in BSTR ManifestTimestampUrl);
        
        /* [helpstring][id][nonbrowsable][propget] */ HRESULT ( STDMETHODCALLTYPE *get_MyApplication )( 
            VBProjectProperties3 * This,
            /* [retval][out] */ __RPC__deref_out_opt IDispatch **ppdispMyApplication);
        
        /* [helpstring][id][propget] */ HRESULT ( STDMETHODCALLTYPE *get_MyType )( 
            VBProjectProperties3 * This,
            /* [retval][out] */ __RPC__deref_out_opt BSTR *pbstrMyType);
        
        /* [helpstring][id][propput] */ HRESULT ( STDMETHODCALLTYPE *put_MyType )( 
            VBProjectProperties3 * This,
            /* [in] */ __RPC__in BSTR MyType);
        
        END_INTERFACE
    } VBProjectProperties3Vtbl;

    interface VBProjectProperties3
    {
        CONST_VTBL struct VBProjectProperties3Vtbl *lpVtbl;
    };

    

#ifdef COBJMACROS


#define VBProjectProperties3_QueryInterface(This,riid,ppvObj,retval)	\
    ( (This)->lpVtbl -> QueryInterface(This,riid,ppvObj,retval) ) 

#define VBProjectProperties3_AddRef(This,retval)	\
    ( (This)->lpVtbl -> AddRef(This,retval) ) 

#define VBProjectProperties3_Release(This,retval)	\
    ( (This)->lpVtbl -> Release(This,retval) ) 

#define VBProjectProperties3_GetTypeInfoCount(This,pctinfo,retval)	\
    ( (This)->lpVtbl -> GetTypeInfoCount(This,pctinfo,retval) ) 

#define VBProjectProperties3_GetTypeInfo(This,itinfo,lcid,pptinfo,retval)	\
    ( (This)->lpVtbl -> GetTypeInfo(This,itinfo,lcid,pptinfo,retval) ) 

#define VBProjectProperties3_GetIDsOfNames(This,riid,rgszNames,cNames,lcid,rgdispid,retval)	\
    ( (This)->lpVtbl -> GetIDsOfNames(This,riid,rgszNames,cNames,lcid,rgdispid,retval) ) 

#define VBProjectProperties3_Invoke(This,dispidMember,riid,lcid,wFlags,pdispparams,pvarResult,pexcepinfo,puArgErr,retval)	\
    ( (This)->lpVtbl -> Invoke(This,dispidMember,riid,lcid,wFlags,pdispparams,pvarResult,pexcepinfo,puArgErr,retval) ) 

#define VBProjectProperties3_get___id(This,retval)	\
    ( (This)->lpVtbl -> get___id(This,retval) ) 

#define VBProjectProperties3_get___project(This,retval)	\
    ( (This)->lpVtbl -> get___project(This,retval) ) 

#define VBProjectProperties3_get_StartupObject(This,retval)	\
    ( (This)->lpVtbl -> get_StartupObject(This,retval) ) 

#define VBProjectProperties3_put_StartupObject(This,noname,retval)	\
    ( (This)->lpVtbl -> put_StartupObject(This,noname,retval) ) 

#define VBProjectProperties3_get_OutputType(This,retval)	\
    ( (This)->lpVtbl -> get_OutputType(This,retval) ) 

#define VBProjectProperties3_put_OutputType(This,noname,retval)	\
    ( (This)->lpVtbl -> put_OutputType(This,noname,retval) ) 

#define VBProjectProperties3_get_RootNamespace(This,retval)	\
    ( (This)->lpVtbl -> get_RootNamespace(This,retval) ) 

#define VBProjectProperties3_put_RootNamespace(This,noname,retval)	\
    ( (This)->lpVtbl -> put_RootNamespace(This,noname,retval) ) 

#define VBProjectProperties3_get_AssemblyName(This,retval)	\
    ( (This)->lpVtbl -> get_AssemblyName(This,retval) ) 

#define VBProjectProperties3_put_AssemblyName(This,noname,retval)	\
    ( (This)->lpVtbl -> put_AssemblyName(This,noname,retval) ) 

#define VBProjectProperties3_get_AssemblyOriginatorKeyFile(This,retval)	\
    ( (This)->lpVtbl -> get_AssemblyOriginatorKeyFile(This,retval) ) 

#define VBProjectProperties3_put_AssemblyOriginatorKeyFile(This,noname,retval)	\
    ( (This)->lpVtbl -> put_AssemblyOriginatorKeyFile(This,noname,retval) ) 

#define VBProjectProperties3_get_AssemblyKeyContainerName(This,retval)	\
    ( (This)->lpVtbl -> get_AssemblyKeyContainerName(This,retval) ) 

#define VBProjectProperties3_put_AssemblyKeyContainerName(This,noname,retval)	\
    ( (This)->lpVtbl -> put_AssemblyKeyContainerName(This,noname,retval) ) 

#define VBProjectProperties3_get_AssemblyOriginatorKeyMode(This,retval)	\
    ( (This)->lpVtbl -> get_AssemblyOriginatorKeyMode(This,retval) ) 

#define VBProjectProperties3_put_AssemblyOriginatorKeyMode(This,noname,retval)	\
    ( (This)->lpVtbl -> put_AssemblyOriginatorKeyMode(This,noname,retval) ) 

#define VBProjectProperties3_get_DelaySign(This,retval)	\
    ( (This)->lpVtbl -> get_DelaySign(This,retval) ) 

#define VBProjectProperties3_put_DelaySign(This,noname,retval)	\
    ( (This)->lpVtbl -> put_DelaySign(This,noname,retval) ) 

#define VBProjectProperties3_get_WebServer(This,retval)	\
    ( (This)->lpVtbl -> get_WebServer(This,retval) ) 

#define VBProjectProperties3_get_WebServerVersion(This,retval)	\
    ( (This)->lpVtbl -> get_WebServerVersion(This,retval) ) 

#define VBProjectProperties3_get_ServerExtensionsVersion(This,retval)	\
    ( (This)->lpVtbl -> get_ServerExtensionsVersion(This,retval) ) 

#define VBProjectProperties3_get_LinkRepair(This,retval)	\
    ( (This)->lpVtbl -> get_LinkRepair(This,retval) ) 

#define VBProjectProperties3_put_LinkRepair(This,noname,retval)	\
    ( (This)->lpVtbl -> put_LinkRepair(This,noname,retval) ) 

#define VBProjectProperties3_get_OfflineURL(This,retval)	\
    ( (This)->lpVtbl -> get_OfflineURL(This,retval) ) 

#define VBProjectProperties3_get_FileSharePath(This,retval)	\
    ( (This)->lpVtbl -> get_FileSharePath(This,retval) ) 

#define VBProjectProperties3_put_FileSharePath(This,noname,retval)	\
    ( (This)->lpVtbl -> put_FileSharePath(This,noname,retval) ) 

#define VBProjectProperties3_get_ActiveFileSharePath(This,retval)	\
    ( (This)->lpVtbl -> get_ActiveFileSharePath(This,retval) ) 

#define VBProjectProperties3_get_WebAccessMethod(This,retval)	\
    ( (This)->lpVtbl -> get_WebAccessMethod(This,retval) ) 

#define VBProjectProperties3_put_WebAccessMethod(This,noname,retval)	\
    ( (This)->lpVtbl -> put_WebAccessMethod(This,noname,retval) ) 

#define VBProjectProperties3_get_ActiveWebAccessMethod(This,retval)	\
    ( (This)->lpVtbl -> get_ActiveWebAccessMethod(This,retval) ) 

#define VBProjectProperties3_get_DefaultClientScript(This,retval)	\
    ( (This)->lpVtbl -> get_DefaultClientScript(This,retval) ) 

#define VBProjectProperties3_put_DefaultClientScript(This,noname,retval)	\
    ( (This)->lpVtbl -> put_DefaultClientScript(This,noname,retval) ) 

#define VBProjectProperties3_get_DefaultTargetSchema(This,retval)	\
    ( (This)->lpVtbl -> get_DefaultTargetSchema(This,retval) ) 

#define VBProjectProperties3_put_DefaultTargetSchema(This,noname,retval)	\
    ( (This)->lpVtbl -> put_DefaultTargetSchema(This,noname,retval) ) 

#define VBProjectProperties3_get_DefaultHTMLPageLayout(This,retval)	\
    ( (This)->lpVtbl -> get_DefaultHTMLPageLayout(This,retval) ) 

#define VBProjectProperties3_put_DefaultHTMLPageLayout(This,noname,retval)	\
    ( (This)->lpVtbl -> put_DefaultHTMLPageLayout(This,noname,retval) ) 

#define VBProjectProperties3_get_FileName(This,retval)	\
    ( (This)->lpVtbl -> get_FileName(This,retval) ) 

#define VBProjectProperties3_put_FileName(This,noname,retval)	\
    ( (This)->lpVtbl -> put_FileName(This,noname,retval) ) 

#define VBProjectProperties3_get_FullPath(This,retval)	\
    ( (This)->lpVtbl -> get_FullPath(This,retval) ) 

#define VBProjectProperties3_get_LocalPath(This,retval)	\
    ( (This)->lpVtbl -> get_LocalPath(This,retval) ) 

#define VBProjectProperties3_get_URL(This,retval)	\
    ( (This)->lpVtbl -> get_URL(This,retval) ) 

#define VBProjectProperties3_get_ActiveConfigurationSettings(This,retval)	\
    ( (This)->lpVtbl -> get_ActiveConfigurationSettings(This,retval) ) 

#define VBProjectProperties3_get_Extender(This,ExtenderName,retval)	\
    ( (This)->lpVtbl -> get_Extender(This,ExtenderName,retval) ) 

#define VBProjectProperties3_get_ExtenderNames(This,retval)	\
    ( (This)->lpVtbl -> get_ExtenderNames(This,retval) ) 

#define VBProjectProperties3_get_ExtenderCATID(This,retval)	\
    ( (This)->lpVtbl -> get_ExtenderCATID(This,retval) ) 

#define VBProjectProperties3_get_ApplicationIcon(This,retval)	\
    ( (This)->lpVtbl -> get_ApplicationIcon(This,retval) ) 

#define VBProjectProperties3_put_ApplicationIcon(This,noname,retval)	\
    ( (This)->lpVtbl -> put_ApplicationIcon(This,noname,retval) ) 

#define VBProjectProperties3_get_OptionStrict(This,retval)	\
    ( (This)->lpVtbl -> get_OptionStrict(This,retval) ) 

#define VBProjectProperties3_put_OptionStrict(This,noname,retval)	\
    ( (This)->lpVtbl -> put_OptionStrict(This,noname,retval) ) 

#define VBProjectProperties3_get_ReferencePath(This,retval)	\
    ( (This)->lpVtbl -> get_ReferencePath(This,retval) ) 

#define VBProjectProperties3_put_ReferencePath(This,noname,retval)	\
    ( (This)->lpVtbl -> put_ReferencePath(This,noname,retval) ) 

#define VBProjectProperties3_get_OutputFileName(This,retval)	\
    ( (This)->lpVtbl -> get_OutputFileName(This,retval) ) 

#define VBProjectProperties3_get_AbsoluteProjectDirectory(This,retval)	\
    ( (This)->lpVtbl -> get_AbsoluteProjectDirectory(This,retval) ) 

#define VBProjectProperties3_get_OptionExplicit(This,retval)	\
    ( (This)->lpVtbl -> get_OptionExplicit(This,retval) ) 

#define VBProjectProperties3_put_OptionExplicit(This,noname,retval)	\
    ( (This)->lpVtbl -> put_OptionExplicit(This,noname,retval) ) 

#define VBProjectProperties3_get_OptionCompare(This,retval)	\
    ( (This)->lpVtbl -> get_OptionCompare(This,retval) ) 

#define VBProjectProperties3_put_OptionCompare(This,noname,retval)	\
    ( (This)->lpVtbl -> put_OptionCompare(This,noname,retval) ) 

#define VBProjectProperties3_get_ProjectType(This,retval)	\
    ( (This)->lpVtbl -> get_ProjectType(This,retval) ) 

#define VBProjectProperties3_get_DefaultNamespace(This,retval)	\
    ( (This)->lpVtbl -> get_DefaultNamespace(This,retval) ) 

#define VBProjectProperties3_put_DefaultNamespace(This,noname,retval)	\
    ( (This)->lpVtbl -> put_DefaultNamespace(This,noname,retval) ) 

#define VBProjectProperties3_get_PreBuildEvent(This,retval)	\
    ( (This)->lpVtbl -> get_PreBuildEvent(This,retval) ) 

#define VBProjectProperties3_put_PreBuildEvent(This,noname,retval)	\
    ( (This)->lpVtbl -> put_PreBuildEvent(This,noname,retval) ) 

#define VBProjectProperties3_get_PostBuildEvent(This,retval)	\
    ( (This)->lpVtbl -> get_PostBuildEvent(This,retval) ) 

#define VBProjectProperties3_put_PostBuildEvent(This,noname,retval)	\
    ( (This)->lpVtbl -> put_PostBuildEvent(This,noname,retval) ) 

#define VBProjectProperties3_get_RunPostBuildEvent(This,retval)	\
    ( (This)->lpVtbl -> get_RunPostBuildEvent(This,retval) ) 

#define VBProjectProperties3_put_RunPostBuildEvent(This,noname,retval)	\
    ( (This)->lpVtbl -> put_RunPostBuildEvent(This,noname,retval) ) 

#define VBProjectProperties3_get_AspnetVersion(This,retval)	\
    ( (This)->lpVtbl -> get_AspnetVersion(This,retval) ) 


#define VBProjectProperties3_get_Title(This,pbstrTitle)	\
    ( (This)->lpVtbl -> get_Title(This,pbstrTitle) ) 

#define VBProjectProperties3_put_Title(This,Title)	\
    ( (This)->lpVtbl -> put_Title(This,Title) ) 

#define VBProjectProperties3_get_Description(This,pbstrDescription)	\
    ( (This)->lpVtbl -> get_Description(This,pbstrDescription) ) 

#define VBProjectProperties3_put_Description(This,Description)	\
    ( (This)->lpVtbl -> put_Description(This,Description) ) 

#define VBProjectProperties3_get_Company(This,pbstrCompany)	\
    ( (This)->lpVtbl -> get_Company(This,pbstrCompany) ) 

#define VBProjectProperties3_put_Company(This,Company)	\
    ( (This)->lpVtbl -> put_Company(This,Company) ) 

#define VBProjectProperties3_get_Product(This,pbstrProduct)	\
    ( (This)->lpVtbl -> get_Product(This,pbstrProduct) ) 

#define VBProjectProperties3_put_Product(This,Product)	\
    ( (This)->lpVtbl -> put_Product(This,Product) ) 

#define VBProjectProperties3_get_Copyright(This,pbstrCopyright)	\
    ( (This)->lpVtbl -> get_Copyright(This,pbstrCopyright) ) 

#define VBProjectProperties3_put_Copyright(This,Copyright)	\
    ( (This)->lpVtbl -> put_Copyright(This,Copyright) ) 

#define VBProjectProperties3_get_Trademark(This,pbstrTrademark)	\
    ( (This)->lpVtbl -> get_Trademark(This,pbstrTrademark) ) 

#define VBProjectProperties3_put_Trademark(This,Trademark)	\
    ( (This)->lpVtbl -> put_Trademark(This,Trademark) ) 

#define VBProjectProperties3_get_AssemblyType(This,pAssemblyType)	\
    ( (This)->lpVtbl -> get_AssemblyType(This,pAssemblyType) ) 

#define VBProjectProperties3_put_AssemblyType(This,AssemblyType)	\
    ( (This)->lpVtbl -> put_AssemblyType(This,AssemblyType) ) 

#define VBProjectProperties3_get_TypeComplianceDiagnostics(This,pbTypeComplianceDiagnostics)	\
    ( (This)->lpVtbl -> get_TypeComplianceDiagnostics(This,pbTypeComplianceDiagnostics) ) 

#define VBProjectProperties3_put_TypeComplianceDiagnostics(This,TypeComplianceDiagnostics)	\
    ( (This)->lpVtbl -> put_TypeComplianceDiagnostics(This,TypeComplianceDiagnostics) ) 

#define VBProjectProperties3_get_Win32ResourceFile(This,pbstrW32ResourceFile)	\
    ( (This)->lpVtbl -> get_Win32ResourceFile(This,pbstrW32ResourceFile) ) 

#define VBProjectProperties3_put_Win32ResourceFile(This,Win32ResourceFile)	\
    ( (This)->lpVtbl -> put_Win32ResourceFile(This,Win32ResourceFile) ) 

#define VBProjectProperties3_get_AssemblyKeyProviderName(This,pbstrKeyProviderName)	\
    ( (This)->lpVtbl -> get_AssemblyKeyProviderName(This,pbstrKeyProviderName) ) 

#define VBProjectProperties3_put_AssemblyKeyProviderName(This,KeyProviderName)	\
    ( (This)->lpVtbl -> put_AssemblyKeyProviderName(This,KeyProviderName) ) 

#define VBProjectProperties3_get_AssemblyOriginatorKeyFileType(This,pdwOriginatorKeyFileType)	\
    ( (This)->lpVtbl -> get_AssemblyOriginatorKeyFileType(This,pdwOriginatorKeyFileType) ) 

#define VBProjectProperties3_put_AssemblyOriginatorKeyFileType(This,OriginatorKeyFileType)	\
    ( (This)->lpVtbl -> put_AssemblyOriginatorKeyFileType(This,OriginatorKeyFileType) ) 

#define VBProjectProperties3_get_AssemblyVersion(This,pbstrAssemblyVersion)	\
    ( (This)->lpVtbl -> get_AssemblyVersion(This,pbstrAssemblyVersion) ) 

#define VBProjectProperties3_put_AssemblyVersion(This,AssemblyVersion)	\
    ( (This)->lpVtbl -> put_AssemblyVersion(This,AssemblyVersion) ) 

#define VBProjectProperties3_get_AssemblyFileVersion(This,pbstrAssemblyFileVersion)	\
    ( (This)->lpVtbl -> get_AssemblyFileVersion(This,pbstrAssemblyFileVersion) ) 

#define VBProjectProperties3_put_AssemblyFileVersion(This,AssemblyFileVersion)	\
    ( (This)->lpVtbl -> put_AssemblyFileVersion(This,AssemblyFileVersion) ) 

#define VBProjectProperties3_get_GenerateManifests(This,pbGenerateManifests)	\
    ( (This)->lpVtbl -> get_GenerateManifests(This,pbGenerateManifests) ) 

#define VBProjectProperties3_put_GenerateManifests(This,GenerateManifests)	\
    ( (This)->lpVtbl -> put_GenerateManifests(This,GenerateManifests) ) 

#define VBProjectProperties3_get_EnableSecurityDebugging(This,pbEnableSecurityDebugging)	\
    ( (This)->lpVtbl -> get_EnableSecurityDebugging(This,pbEnableSecurityDebugging) ) 

#define VBProjectProperties3_put_EnableSecurityDebugging(This,EnableSecurityDebugging)	\
    ( (This)->lpVtbl -> put_EnableSecurityDebugging(This,EnableSecurityDebugging) ) 

#define VBProjectProperties3_get_DebugSecurityZoneURL(This,pbstrSecurityURL)	\
    ( (This)->lpVtbl -> get_DebugSecurityZoneURL(This,pbstrSecurityURL) ) 

#define VBProjectProperties3_put_DebugSecurityZoneURL(This,SecurityURL)	\
    ( (This)->lpVtbl -> put_DebugSecurityZoneURL(This,SecurityURL) ) 

#define VBProjectProperties3_get_Publish(This,ppdispPublish)	\
    ( (This)->lpVtbl -> get_Publish(This,ppdispPublish) ) 

#define VBProjectProperties3_get_ComVisible(This,pbComVisible)	\
    ( (This)->lpVtbl -> get_ComVisible(This,pbComVisible) ) 

#define VBProjectProperties3_put_ComVisible(This,ComVisible)	\
    ( (This)->lpVtbl -> put_ComVisible(This,ComVisible) ) 

#define VBProjectProperties3_get_AssemblyGuid(This,pbstrAssemblyGuid)	\
    ( (This)->lpVtbl -> get_AssemblyGuid(This,pbstrAssemblyGuid) ) 

#define VBProjectProperties3_put_AssemblyGuid(This,AssemblyGuid)	\
    ( (This)->lpVtbl -> put_AssemblyGuid(This,AssemblyGuid) ) 

#define VBProjectProperties3_get_NeutralResourcesLanguage(This,pbstrNeutralResourcesLanguage)	\
    ( (This)->lpVtbl -> get_NeutralResourcesLanguage(This,pbstrNeutralResourcesLanguage) ) 

#define VBProjectProperties3_put_NeutralResourcesLanguage(This,NeutralResourcesLanguage)	\
    ( (This)->lpVtbl -> put_NeutralResourcesLanguage(This,NeutralResourcesLanguage) ) 

#define VBProjectProperties3_get_SignAssembly(This,pbSignAssembly)	\
    ( (This)->lpVtbl -> get_SignAssembly(This,pbSignAssembly) ) 

#define VBProjectProperties3_put_SignAssembly(This,SignAssembly)	\
    ( (This)->lpVtbl -> put_SignAssembly(This,SignAssembly) ) 

#define VBProjectProperties3_get_SignManifests(This,pbSignManifests)	\
    ( (This)->lpVtbl -> get_SignManifests(This,pbSignManifests) ) 

#define VBProjectProperties3_put_SignManifests(This,bSignManifests)	\
    ( (This)->lpVtbl -> put_SignManifests(This,bSignManifests) ) 

#define VBProjectProperties3_get_TargetZone(This,pbstrTargetZone)	\
    ( (This)->lpVtbl -> get_TargetZone(This,pbstrTargetZone) ) 

#define VBProjectProperties3_put_TargetZone(This,TargetZone)	\
    ( (This)->lpVtbl -> put_TargetZone(This,TargetZone) ) 

#define VBProjectProperties3_get_ExcludedPermissions(This,pbstrExcludedPermissions)	\
    ( (This)->lpVtbl -> get_ExcludedPermissions(This,pbstrExcludedPermissions) ) 

#define VBProjectProperties3_put_ExcludedPermissions(This,ExcludedPermissions)	\
    ( (This)->lpVtbl -> put_ExcludedPermissions(This,ExcludedPermissions) ) 

#define VBProjectProperties3_get_ManifestCertificateThumbprint(This,pbstrManifestCertificateThumbprint)	\
    ( (This)->lpVtbl -> get_ManifestCertificateThumbprint(This,pbstrManifestCertificateThumbprint) ) 

#define VBProjectProperties3_put_ManifestCertificateThumbprint(This,ManifestCertificateThumbprint)	\
    ( (This)->lpVtbl -> put_ManifestCertificateThumbprint(This,ManifestCertificateThumbprint) ) 

#define VBProjectProperties3_get_ManifestKeyFile(This,pbstrManifestKeyFile)	\
    ( (This)->lpVtbl -> get_ManifestKeyFile(This,pbstrManifestKeyFile) ) 

#define VBProjectProperties3_put_ManifestKeyFile(This,ManifestKeyFile)	\
    ( (This)->lpVtbl -> put_ManifestKeyFile(This,ManifestKeyFile) ) 

#define VBProjectProperties3_get_ManifestTimestampUrl(This,pbstrManifestTimestampUrl)	\
    ( (This)->lpVtbl -> get_ManifestTimestampUrl(This,pbstrManifestTimestampUrl) ) 

#define VBProjectProperties3_put_ManifestTimestampUrl(This,ManifestTimestampUrl)	\
    ( (This)->lpVtbl -> put_ManifestTimestampUrl(This,ManifestTimestampUrl) ) 


#define VBProjectProperties3_get_MyApplication(This,ppdispMyApplication)	\
    ( (This)->lpVtbl -> get_MyApplication(This,ppdispMyApplication) ) 

#define VBProjectProperties3_get_MyType(This,pbstrMyType)	\
    ( (This)->lpVtbl -> get_MyType(This,pbstrMyType) ) 

#define VBProjectProperties3_put_MyType(This,MyType)	\
    ( (This)->lpVtbl -> put_MyType(This,MyType) ) 

#endif /* COBJMACROS */


#endif 	/* C style interface */



/* [helpstring][id][propget] */ HRESULT STDMETHODCALLTYPE VBProjectProperties3_get_MyType_Proxy( 
    VBProjectProperties3 * This,
    /* [retval][out] */ __RPC__deref_out_opt BSTR *pbstrMyType);


void __RPC_STUB VBProjectProperties3_get_MyType_Stub(
    IRpcStubBuffer *This,
    IRpcChannelBuffer *_pRpcChannelBuffer,
    PRPC_MESSAGE _pRpcMessage,
    DWORD *_pdwStubPhase);


/* [helpstring][id][propput] */ HRESULT STDMETHODCALLTYPE VBProjectProperties3_put_MyType_Proxy( 
    VBProjectProperties3 * This,
    /* [in] */ __RPC__in BSTR MyType);


void __RPC_STUB VBProjectProperties3_put_MyType_Stub(
    IRpcStubBuffer *This,
    IRpcChannelBuffer *_pRpcChannelBuffer,
    PRPC_MESSAGE _pRpcMessage,
    DWORD *_pdwStubPhase);



#endif 	/* __VBProjectProperties3_INTERFACE_DEFINED__ */


#ifndef __CSharpProjectConfigurationProperties3_INTERFACE_DEFINED__
#define __CSharpProjectConfigurationProperties3_INTERFACE_DEFINED__

/* interface CSharpProjectConfigurationProperties3 */
/* [object][dual][unique][helpstring][uuid] */ 


EXTERN_C const IID IID_CSharpProjectConfigurationProperties3;

#if defined(__cplusplus) && !defined(CINTERFACE)
    
    MIDL_INTERFACE("F25C9AD7-E371-414d-82A0-24E8BBC25C99")
    CSharpProjectConfigurationProperties3 : public ProjectConfigurationProperties3
    {
    public:
        virtual /* [helpstring][id][propget] */ HRESULT STDMETHODCALLTYPE get_LanguageVersion( 
            /* [retval][out] */ __RPC__deref_out_opt BSTR *pbstrLanguageVersion) = 0;
        
        virtual /* [helpstring][id][propput] */ HRESULT STDMETHODCALLTYPE put_LanguageVersion( 
            /* [in] */ __RPC__in BSTR LanguageVersion) = 0;
        
        virtual /* [helpstring][id][propget] */ HRESULT STDMETHODCALLTYPE get_ErrorReport( 
            /* [retval][out] */ __RPC__deref_out_opt BSTR *pbstrErrorReport) = 0;
        
        virtual /* [helpstring][id][propput] */ HRESULT STDMETHODCALLTYPE put_ErrorReport( 
            /* [in] */ __RPC__in BSTR ErrorReport) = 0;
        
    };
    
#else 	/* C style interface */

    typedef struct CSharpProjectConfigurationProperties3Vtbl
    {
        BEGIN_INTERFACE
        
        /* [id][restricted][funcdescattr] */ HRESULT ( STDMETHODCALLTYPE *QueryInterface )( 
            CSharpProjectConfigurationProperties3 * This,
            /* [in][idldescattr] */ __RPC__in struct GUID *riid,
            /* [out][idldescattr] */ __RPC__deref_out_opt void **ppvObj,
            /* [retval][out] */ __RPC__out void *retval);
        
        /* [id][restricted][funcdescattr] */ HRESULT ( STDMETHODCALLTYPE *AddRef )( 
            CSharpProjectConfigurationProperties3 * This,
            /* [retval][out] */ __RPC__out unsigned long *retval);
        
        /* [id][restricted][funcdescattr] */ HRESULT ( STDMETHODCALLTYPE *Release )( 
            CSharpProjectConfigurationProperties3 * This,
            /* [retval][out] */ __RPC__out unsigned long *retval);
        
        /* [id][restricted][funcdescattr] */ HRESULT ( STDMETHODCALLTYPE *GetTypeInfoCount )( 
            CSharpProjectConfigurationProperties3 * This,
            /* [out][idldescattr] */ __RPC__out unsigned UINT *pctinfo,
            /* [retval][out] */ __RPC__out void *retval);
        
        /* [id][restricted][funcdescattr] */ HRESULT ( STDMETHODCALLTYPE *GetTypeInfo )( 
            CSharpProjectConfigurationProperties3 * This,
            /* [in][idldescattr] */ unsigned UINT itinfo,
            /* [in][idldescattr] */ unsigned long lcid,
            /* [out][idldescattr] */ __RPC__deref_out_opt void **pptinfo,
            /* [retval][out] */ __RPC__out void *retval);
        
        /* [id][restricted][funcdescattr] */ HRESULT ( STDMETHODCALLTYPE *GetIDsOfNames )( 
            CSharpProjectConfigurationProperties3 * This,
            /* [in][idldescattr] */ __RPC__in struct GUID *riid,
            /* [in][idldescattr] */ __RPC__deref_in_opt signed char **rgszNames,
            /* [in][idldescattr] */ unsigned UINT cNames,
            /* [in][idldescattr] */ unsigned long lcid,
            /* [out][idldescattr] */ __RPC__out signed long *rgdispid,
            /* [retval][out] */ __RPC__out void *retval);
        
        /* [id][restricted][funcdescattr] */ HRESULT ( STDMETHODCALLTYPE *Invoke )( 
            CSharpProjectConfigurationProperties3 * This,
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
            CSharpProjectConfigurationProperties3 * This,
            /* [retval][out] */ __RPC__deref_out_opt BSTR *retval);
        
        /* [id][propget][funcdescattr] */ HRESULT ( STDMETHODCALLTYPE *get_DebugSymbols )( 
            CSharpProjectConfigurationProperties3 * This,
            /* [retval][out] */ __RPC__out BOOLEAN *retval);
        
        /* [id][propput][funcdescattr] */ HRESULT ( STDMETHODCALLTYPE *put_DebugSymbols )( 
            CSharpProjectConfigurationProperties3 * This,
            /* [in][idldescattr] */ BOOLEAN noname,
            /* [retval][out] */ __RPC__out void *retval);
        
        /* [id][propget][funcdescattr] */ HRESULT ( STDMETHODCALLTYPE *get_DefineDebug )( 
            CSharpProjectConfigurationProperties3 * This,
            /* [retval][out] */ __RPC__out BOOLEAN *retval);
        
        /* [id][propput][funcdescattr] */ HRESULT ( STDMETHODCALLTYPE *put_DefineDebug )( 
            CSharpProjectConfigurationProperties3 * This,
            /* [in][idldescattr] */ BOOLEAN noname,
            /* [retval][out] */ __RPC__out void *retval);
        
        /* [id][propget][funcdescattr] */ HRESULT ( STDMETHODCALLTYPE *get_DefineTrace )( 
            CSharpProjectConfigurationProperties3 * This,
            /* [retval][out] */ __RPC__out BOOLEAN *retval);
        
        /* [id][propput][funcdescattr] */ HRESULT ( STDMETHODCALLTYPE *put_DefineTrace )( 
            CSharpProjectConfigurationProperties3 * This,
            /* [in][idldescattr] */ BOOLEAN noname,
            /* [retval][out] */ __RPC__out void *retval);
        
        /* [id][propget][funcdescattr] */ HRESULT ( STDMETHODCALLTYPE *get_OutputPath )( 
            CSharpProjectConfigurationProperties3 * This,
            /* [retval][out] */ __RPC__deref_out_opt BSTR *retval);
        
        /* [id][propput][funcdescattr] */ HRESULT ( STDMETHODCALLTYPE *put_OutputPath )( 
            CSharpProjectConfigurationProperties3 * This,
            /* [in][idldescattr] */ __RPC__in BSTR noname,
            /* [retval][out] */ __RPC__out void *retval);
        
        /* [id][propget][funcdescattr] */ HRESULT ( STDMETHODCALLTYPE *get_IntermediatePath )( 
            CSharpProjectConfigurationProperties3 * This,
            /* [retval][out] */ __RPC__deref_out_opt BSTR *retval);
        
        /* [id][propput][hidden][funcdescattr] */ HRESULT ( STDMETHODCALLTYPE *put_IntermediatePath )( 
            CSharpProjectConfigurationProperties3 * This,
            /* [in][idldescattr] */ __RPC__in BSTR noname,
            /* [retval][out] */ __RPC__out void *retval);
        
        /* [id][propget][funcdescattr] */ HRESULT ( STDMETHODCALLTYPE *get_DefineConstants )( 
            CSharpProjectConfigurationProperties3 * This,
            /* [retval][out] */ __RPC__deref_out_opt BSTR *retval);
        
        /* [id][propput][funcdescattr] */ HRESULT ( STDMETHODCALLTYPE *put_DefineConstants )( 
            CSharpProjectConfigurationProperties3 * This,
            /* [in][idldescattr] */ __RPC__in BSTR noname,
            /* [retval][out] */ __RPC__out void *retval);
        
        /* [id][propget][funcdescattr] */ HRESULT ( STDMETHODCALLTYPE *get_RemoveIntegerChecks )( 
            CSharpProjectConfigurationProperties3 * This,
            /* [retval][out] */ __RPC__out BOOLEAN *retval);
        
        /* [id][propput][funcdescattr] */ HRESULT ( STDMETHODCALLTYPE *put_RemoveIntegerChecks )( 
            CSharpProjectConfigurationProperties3 * This,
            /* [in][idldescattr] */ BOOLEAN noname,
            /* [retval][out] */ __RPC__out void *retval);
        
        /* [id][propget][funcdescattr] */ HRESULT ( STDMETHODCALLTYPE *get_BaseAddress )( 
            CSharpProjectConfigurationProperties3 * This,
            /* [retval][out] */ __RPC__out unsigned long *retval);
        
        /* [id][propput][funcdescattr] */ HRESULT ( STDMETHODCALLTYPE *put_BaseAddress )( 
            CSharpProjectConfigurationProperties3 * This,
            /* [in][idldescattr] */ unsigned long noname,
            /* [retval][out] */ __RPC__out void *retval);
        
        /* [id][propget][funcdescattr] */ HRESULT ( STDMETHODCALLTYPE *get_AllowUnsafeBlocks )( 
            CSharpProjectConfigurationProperties3 * This,
            /* [retval][out] */ __RPC__out BOOLEAN *retval);
        
        /* [id][propput][funcdescattr] */ HRESULT ( STDMETHODCALLTYPE *put_AllowUnsafeBlocks )( 
            CSharpProjectConfigurationProperties3 * This,
            /* [in][idldescattr] */ BOOLEAN noname,
            /* [retval][out] */ __RPC__out void *retval);
        
        /* [id][propget][funcdescattr] */ HRESULT ( STDMETHODCALLTYPE *get_CheckForOverflowUnderflow )( 
            CSharpProjectConfigurationProperties3 * This,
            /* [retval][out] */ __RPC__out BOOLEAN *retval);
        
        /* [id][propput][funcdescattr] */ HRESULT ( STDMETHODCALLTYPE *put_CheckForOverflowUnderflow )( 
            CSharpProjectConfigurationProperties3 * This,
            /* [in][idldescattr] */ BOOLEAN noname,
            /* [retval][out] */ __RPC__out void *retval);
        
        /* [id][propget][funcdescattr] */ HRESULT ( STDMETHODCALLTYPE *get_DocumentationFile )( 
            CSharpProjectConfigurationProperties3 * This,
            /* [retval][out] */ __RPC__deref_out_opt BSTR *retval);
        
        /* [id][propput][funcdescattr] */ HRESULT ( STDMETHODCALLTYPE *put_DocumentationFile )( 
            CSharpProjectConfigurationProperties3 * This,
            /* [in][idldescattr] */ __RPC__in BSTR noname,
            /* [retval][out] */ __RPC__out void *retval);
        
        /* [id][propget][funcdescattr] */ HRESULT ( STDMETHODCALLTYPE *get_Optimize )( 
            CSharpProjectConfigurationProperties3 * This,
            /* [retval][out] */ __RPC__out BOOLEAN *retval);
        
        /* [id][propput][funcdescattr] */ HRESULT ( STDMETHODCALLTYPE *put_Optimize )( 
            CSharpProjectConfigurationProperties3 * This,
            /* [in][idldescattr] */ BOOLEAN noname,
            /* [retval][out] */ __RPC__out void *retval);
        
        /* [id][propget][funcdescattr] */ HRESULT ( STDMETHODCALLTYPE *get_IncrementalBuild )( 
            CSharpProjectConfigurationProperties3 * This,
            /* [retval][out] */ __RPC__out BOOLEAN *retval);
        
        /* [id][propput][funcdescattr] */ HRESULT ( STDMETHODCALLTYPE *put_IncrementalBuild )( 
            CSharpProjectConfigurationProperties3 * This,
            /* [in][idldescattr] */ BOOLEAN noname,
            /* [retval][out] */ __RPC__out void *retval);
        
        /* [id][propget][funcdescattr] */ HRESULT ( STDMETHODCALLTYPE *get_StartProgram )( 
            CSharpProjectConfigurationProperties3 * This,
            /* [retval][out] */ __RPC__deref_out_opt BSTR *retval);
        
        /* [id][propput][funcdescattr] */ HRESULT ( STDMETHODCALLTYPE *put_StartProgram )( 
            CSharpProjectConfigurationProperties3 * This,
            /* [in][idldescattr] */ __RPC__in BSTR noname,
            /* [retval][out] */ __RPC__out void *retval);
        
        /* [id][propget][funcdescattr] */ HRESULT ( STDMETHODCALLTYPE *get_StartWorkingDirectory )( 
            CSharpProjectConfigurationProperties3 * This,
            /* [retval][out] */ __RPC__deref_out_opt BSTR *retval);
        
        /* [id][propput][funcdescattr] */ HRESULT ( STDMETHODCALLTYPE *put_StartWorkingDirectory )( 
            CSharpProjectConfigurationProperties3 * This,
            /* [in][idldescattr] */ __RPC__in BSTR noname,
            /* [retval][out] */ __RPC__out void *retval);
        
        /* [id][propget][funcdescattr] */ HRESULT ( STDMETHODCALLTYPE *get_StartURL )( 
            CSharpProjectConfigurationProperties3 * This,
            /* [retval][out] */ __RPC__deref_out_opt BSTR *retval);
        
        /* [id][propput][funcdescattr] */ HRESULT ( STDMETHODCALLTYPE *put_StartURL )( 
            CSharpProjectConfigurationProperties3 * This,
            /* [in][idldescattr] */ __RPC__in BSTR noname,
            /* [retval][out] */ __RPC__out void *retval);
        
        /* [id][propget][funcdescattr] */ HRESULT ( STDMETHODCALLTYPE *get_StartPage )( 
            CSharpProjectConfigurationProperties3 * This,
            /* [retval][out] */ __RPC__deref_out_opt BSTR *retval);
        
        /* [id][propput][funcdescattr] */ HRESULT ( STDMETHODCALLTYPE *put_StartPage )( 
            CSharpProjectConfigurationProperties3 * This,
            /* [in][idldescattr] */ __RPC__in BSTR noname,
            /* [retval][out] */ __RPC__out void *retval);
        
        /* [id][propget][funcdescattr] */ HRESULT ( STDMETHODCALLTYPE *get_StartArguments )( 
            CSharpProjectConfigurationProperties3 * This,
            /* [retval][out] */ __RPC__deref_out_opt BSTR *retval);
        
        /* [id][propput][funcdescattr] */ HRESULT ( STDMETHODCALLTYPE *put_StartArguments )( 
            CSharpProjectConfigurationProperties3 * This,
            /* [in][idldescattr] */ __RPC__in BSTR noname,
            /* [retval][out] */ __RPC__out void *retval);
        
        /* [id][propget][funcdescattr] */ HRESULT ( STDMETHODCALLTYPE *get_StartWithIE )( 
            CSharpProjectConfigurationProperties3 * This,
            /* [retval][out] */ __RPC__out BOOLEAN *retval);
        
        /* [id][propput][funcdescattr] */ HRESULT ( STDMETHODCALLTYPE *put_StartWithIE )( 
            CSharpProjectConfigurationProperties3 * This,
            /* [in][idldescattr] */ BOOLEAN noname,
            /* [retval][out] */ __RPC__out void *retval);
        
        /* [id][propget][funcdescattr] */ HRESULT ( STDMETHODCALLTYPE *get_EnableASPDebugging )( 
            CSharpProjectConfigurationProperties3 * This,
            /* [retval][out] */ __RPC__out BOOLEAN *retval);
        
        /* [id][propput][funcdescattr] */ HRESULT ( STDMETHODCALLTYPE *put_EnableASPDebugging )( 
            CSharpProjectConfigurationProperties3 * This,
            /* [in][idldescattr] */ BOOLEAN noname,
            /* [retval][out] */ __RPC__out void *retval);
        
        /* [id][propget][funcdescattr] */ HRESULT ( STDMETHODCALLTYPE *get_EnableASPXDebugging )( 
            CSharpProjectConfigurationProperties3 * This,
            /* [retval][out] */ __RPC__out BOOLEAN *retval);
        
        /* [id][propput][funcdescattr] */ HRESULT ( STDMETHODCALLTYPE *put_EnableASPXDebugging )( 
            CSharpProjectConfigurationProperties3 * This,
            /* [in][idldescattr] */ BOOLEAN noname,
            /* [retval][out] */ __RPC__out void *retval);
        
        /* [id][propget][funcdescattr] */ HRESULT ( STDMETHODCALLTYPE *get_EnableUnmanagedDebugging )( 
            CSharpProjectConfigurationProperties3 * This,
            /* [retval][out] */ __RPC__out BOOLEAN *retval);
        
        /* [id][propput][funcdescattr] */ HRESULT ( STDMETHODCALLTYPE *put_EnableUnmanagedDebugging )( 
            CSharpProjectConfigurationProperties3 * This,
            /* [in][idldescattr] */ BOOLEAN noname,
            /* [retval][out] */ __RPC__out void *retval);
        
        /* [id][propget][funcdescattr] */ HRESULT ( STDMETHODCALLTYPE *get_StartAction )( 
            CSharpProjectConfigurationProperties3 * This,
            /* [retval][out] */ __RPC__out enum prjStartAction *retval);
        
        /* [id][propput][funcdescattr] */ HRESULT ( STDMETHODCALLTYPE *put_StartAction )( 
            CSharpProjectConfigurationProperties3 * This,
            /* [in][idldescattr] */ enum prjStartAction noname,
            /* [retval][out] */ __RPC__out void *retval);
        
        /* [id][propget][funcdescattr] */ HRESULT ( STDMETHODCALLTYPE *get_Extender )( 
            CSharpProjectConfigurationProperties3 * This,
            /* [in][idldescattr] */ __RPC__in BSTR ExtenderName,
            /* [retval][out] */ __RPC__deref_out_opt IDispatch **retval);
        
        /* [id][propget][funcdescattr] */ HRESULT ( STDMETHODCALLTYPE *get_ExtenderNames )( 
            CSharpProjectConfigurationProperties3 * This,
            /* [retval][out] */ __RPC__out VARIANT *retval);
        
        /* [id][propget][funcdescattr] */ HRESULT ( STDMETHODCALLTYPE *get_ExtenderCATID )( 
            CSharpProjectConfigurationProperties3 * This,
            /* [retval][out] */ __RPC__deref_out_opt BSTR *retval);
        
        /* [id][propget][funcdescattr] */ HRESULT ( STDMETHODCALLTYPE *get_WarningLevel )( 
            CSharpProjectConfigurationProperties3 * This,
            /* [retval][out] */ __RPC__out enum prjWarningLevel *retval);
        
        /* [id][propput][funcdescattr] */ HRESULT ( STDMETHODCALLTYPE *put_WarningLevel )( 
            CSharpProjectConfigurationProperties3 * This,
            /* [in][idldescattr] */ enum prjWarningLevel noname,
            /* [retval][out] */ __RPC__out void *retval);
        
        /* [id][propget][funcdescattr] */ HRESULT ( STDMETHODCALLTYPE *get_TreatWarningsAsErrors )( 
            CSharpProjectConfigurationProperties3 * This,
            /* [retval][out] */ __RPC__out BOOLEAN *retval);
        
        /* [id][propput][funcdescattr] */ HRESULT ( STDMETHODCALLTYPE *put_TreatWarningsAsErrors )( 
            CSharpProjectConfigurationProperties3 * This,
            /* [in][idldescattr] */ BOOLEAN noname,
            /* [retval][out] */ __RPC__out void *retval);
        
        /* [id][propget][funcdescattr] */ HRESULT ( STDMETHODCALLTYPE *get_EnableSQLServerDebugging )( 
            CSharpProjectConfigurationProperties3 * This,
            /* [retval][out] */ __RPC__out BOOLEAN *retval);
        
        /* [id][propput][funcdescattr] */ HRESULT ( STDMETHODCALLTYPE *put_EnableSQLServerDebugging )( 
            CSharpProjectConfigurationProperties3 * This,
            /* [in][idldescattr] */ BOOLEAN noname,
            /* [retval][out] */ __RPC__out void *retval);
        
        /* [id][propget][funcdescattr] */ HRESULT ( STDMETHODCALLTYPE *get_FileAlignment )( 
            CSharpProjectConfigurationProperties3 * This,
            /* [retval][out] */ __RPC__out unsigned long *retval);
        
        /* [id][propput][funcdescattr] */ HRESULT ( STDMETHODCALLTYPE *put_FileAlignment )( 
            CSharpProjectConfigurationProperties3 * This,
            /* [in][idldescattr] */ unsigned long noname,
            /* [retval][out] */ __RPC__out void *retval);
        
        /* [id][propget][funcdescattr] */ HRESULT ( STDMETHODCALLTYPE *get_RegisterForComInterop )( 
            CSharpProjectConfigurationProperties3 * This,
            /* [retval][out] */ __RPC__out BOOLEAN *retval);
        
        /* [id][propput][funcdescattr] */ HRESULT ( STDMETHODCALLTYPE *put_RegisterForComInterop )( 
            CSharpProjectConfigurationProperties3 * This,
            /* [in][idldescattr] */ BOOLEAN noname,
            /* [retval][out] */ __RPC__out void *retval);
        
        /* [id][propget][funcdescattr] */ HRESULT ( STDMETHODCALLTYPE *get_ConfigurationOverrideFile )( 
            CSharpProjectConfigurationProperties3 * This,
            /* [retval][out] */ __RPC__deref_out_opt BSTR *retval);
        
        /* [id][propput][funcdescattr] */ HRESULT ( STDMETHODCALLTYPE *put_ConfigurationOverrideFile )( 
            CSharpProjectConfigurationProperties3 * This,
            /* [in][idldescattr] */ __RPC__in BSTR noname,
            /* [retval][out] */ __RPC__out void *retval);
        
        /* [id][propget][funcdescattr] */ HRESULT ( STDMETHODCALLTYPE *get_RemoteDebugEnabled )( 
            CSharpProjectConfigurationProperties3 * This,
            /* [retval][out] */ __RPC__out BOOLEAN *retval);
        
        /* [id][propput][funcdescattr] */ HRESULT ( STDMETHODCALLTYPE *put_RemoteDebugEnabled )( 
            CSharpProjectConfigurationProperties3 * This,
            /* [in][idldescattr] */ BOOLEAN noname,
            /* [retval][out] */ __RPC__out void *retval);
        
        /* [id][propget][funcdescattr] */ HRESULT ( STDMETHODCALLTYPE *get_RemoteDebugMachine )( 
            CSharpProjectConfigurationProperties3 * This,
            /* [retval][out] */ __RPC__deref_out_opt BSTR *retval);
        
        /* [id][propput][funcdescattr] */ HRESULT ( STDMETHODCALLTYPE *put_RemoteDebugMachine )( 
            CSharpProjectConfigurationProperties3 * This,
            /* [in][idldescattr] */ __RPC__in BSTR noname,
            /* [retval][out] */ __RPC__out void *retval);
        
        /* [id][propget][funcdescattr] */ HRESULT ( STDMETHODCALLTYPE *get_NoWarn )( 
            CSharpProjectConfigurationProperties3 * This,
            /* [retval][out] */ __RPC__deref_out_opt BSTR *retval);
        
        /* [id][propput][funcdescattr] */ HRESULT ( STDMETHODCALLTYPE *put_NoWarn )( 
            CSharpProjectConfigurationProperties3 * This,
            /* [in][idldescattr] */ __RPC__in BSTR noname,
            /* [retval][out] */ __RPC__out void *retval);
        
        /* [id][propget][funcdescattr] */ HRESULT ( STDMETHODCALLTYPE *get_NoStdLib )( 
            CSharpProjectConfigurationProperties3 * This,
            /* [retval][out] */ __RPC__out BOOLEAN *retval);
        
        /* [id][propput][funcdescattr] */ HRESULT ( STDMETHODCALLTYPE *put_NoStdLib )( 
            CSharpProjectConfigurationProperties3 * This,
            /* [in][idldescattr] */ BOOLEAN noname,
            /* [retval][out] */ __RPC__out void *retval);
        
        /* [helpstring][id][propget] */ HRESULT ( STDMETHODCALLTYPE *get_DebugInfo )( 
            CSharpProjectConfigurationProperties3 * This,
            /* [retval][out] */ __RPC__deref_out_opt BSTR *pbstrDebugInfo);
        
        /* [helpstring][id][propput] */ HRESULT ( STDMETHODCALLTYPE *put_DebugInfo )( 
            CSharpProjectConfigurationProperties3 * This,
            /* [in] */ __RPC__in BSTR DebugInfo);
        
        /* [helpstring][id][propget] */ HRESULT ( STDMETHODCALLTYPE *get_PlatformTarget )( 
            CSharpProjectConfigurationProperties3 * This,
            /* [retval][out] */ __RPC__deref_out_opt BSTR *pbstrPlatformTarget);
        
        /* [helpstring][id][propput] */ HRESULT ( STDMETHODCALLTYPE *put_PlatformTarget )( 
            CSharpProjectConfigurationProperties3 * This,
            /* [in] */ __RPC__in BSTR PlatformTarget);
        
        /* [helpstring][id][propget] */ HRESULT ( STDMETHODCALLTYPE *get_TreatSpecificWarningsAsErrors )( 
            CSharpProjectConfigurationProperties3 * This,
            /* [retval][out] */ __RPC__deref_out_opt BSTR *pbstrWarningsAsErrors);
        
        /* [helpstring][id][propput] */ HRESULT ( STDMETHODCALLTYPE *put_TreatSpecificWarningsAsErrors )( 
            CSharpProjectConfigurationProperties3 * This,
            /* [in] */ __RPC__in BSTR WarningsAsErrors);
        
        /* [helpstring][id][propget] */ HRESULT ( STDMETHODCALLTYPE *get_RunCodeAnalysis )( 
            CSharpProjectConfigurationProperties3 * This,
            /* [retval][out] */ __RPC__out VARIANT_BOOL *pbRunCodeAnalysis);
        
        /* [helpstring][id][propput] */ HRESULT ( STDMETHODCALLTYPE *put_RunCodeAnalysis )( 
            CSharpProjectConfigurationProperties3 * This,
            /* [in] */ VARIANT_BOOL RunCodeAnalysis);
        
        /* [helpstring][id][propget] */ HRESULT ( STDMETHODCALLTYPE *get_CodeAnalysisLogFile )( 
            CSharpProjectConfigurationProperties3 * This,
            /* [retval][out] */ __RPC__deref_out_opt BSTR *pbstrCodeAnalysisLogFile);
        
        /* [helpstring][id][propput] */ HRESULT ( STDMETHODCALLTYPE *put_CodeAnalysisLogFile )( 
            CSharpProjectConfigurationProperties3 * This,
            /* [in] */ __RPC__in BSTR CodeAnalysisLogFile);
        
        /* [helpstring][id][propget] */ HRESULT ( STDMETHODCALLTYPE *get_CodeAnalysisRuleAssemblies )( 
            CSharpProjectConfigurationProperties3 * This,
            /* [retval][out] */ __RPC__deref_out_opt BSTR *pbstrCodeAnalysisRuleAssemblies);
        
        /* [helpstring][id][propput] */ HRESULT ( STDMETHODCALLTYPE *put_CodeAnalysisRuleAssemblies )( 
            CSharpProjectConfigurationProperties3 * This,
            /* [in] */ __RPC__in BSTR CodeAnalysisRuleAssemblies);
        
        /* [helpstring][id][propget] */ HRESULT ( STDMETHODCALLTYPE *get_CodeAnalysisInputAssembly )( 
            CSharpProjectConfigurationProperties3 * This,
            /* [retval][out] */ __RPC__deref_out_opt BSTR *pbstrCodeAnalysisInputAssembly);
        
        /* [helpstring][id][propput] */ HRESULT ( STDMETHODCALLTYPE *put_CodeAnalysisInputAssembly )( 
            CSharpProjectConfigurationProperties3 * This,
            /* [in] */ __RPC__in BSTR CodeAnalysisInputAssembly);
        
        /* [helpstring][id][propget] */ HRESULT ( STDMETHODCALLTYPE *get_CodeAnalysisRules )( 
            CSharpProjectConfigurationProperties3 * This,
            /* [retval][out] */ __RPC__deref_out_opt BSTR *pbstrCodeAnalysisRules);
        
        /* [helpstring][id][propput] */ HRESULT ( STDMETHODCALLTYPE *put_CodeAnalysisRules )( 
            CSharpProjectConfigurationProperties3 * This,
            /* [in] */ __RPC__in BSTR CodeAnalysisRules);
        
        /* [helpstring][id][propget] */ HRESULT ( STDMETHODCALLTYPE *get_CodeAnalysisSpellCheckLanguages )( 
            CSharpProjectConfigurationProperties3 * This,
            /* [retval][out] */ __RPC__deref_out_opt BSTR *pbstrCodeAnalysisSpellCheckLanguages);
        
        /* [helpstring][id][propput] */ HRESULT ( STDMETHODCALLTYPE *put_CodeAnalysisSpellCheckLanguages )( 
            CSharpProjectConfigurationProperties3 * This,
            /* [in] */ __RPC__in BSTR CodeAnalysisSpellCheckLanguages);
        
        /* [helpstring][id][propget] */ HRESULT ( STDMETHODCALLTYPE *get_CodeAnalysisUseTypeNameInSuppression )( 
            CSharpProjectConfigurationProperties3 * This,
            /* [retval][out] */ __RPC__out VARIANT_BOOL *bUseTypeNameInSuppression);
        
        /* [helpstring][id][propput] */ HRESULT ( STDMETHODCALLTYPE *put_CodeAnalysisUseTypeNameInSuppression )( 
            CSharpProjectConfigurationProperties3 * This,
            /* [in] */ VARIANT_BOOL UseTypeNameInSuppression);
        
        /* [helpstring][id][propget] */ HRESULT ( STDMETHODCALLTYPE *get_CodeAnalysisModuleSuppressionsFile )( 
            CSharpProjectConfigurationProperties3 * This,
            /* [retval][out] */ __RPC__deref_out_opt BSTR *pbstrCodeAnalysisModuleSuppressionsFile);
        
        /* [helpstring][id][propput] */ HRESULT ( STDMETHODCALLTYPE *put_CodeAnalysisModuleSuppressionsFile )( 
            CSharpProjectConfigurationProperties3 * This,
            /* [in] */ __RPC__in BSTR CodeAnalysisModuleSuppressionsFile);
        
        /* [helpstring][id][propget] */ HRESULT ( STDMETHODCALLTYPE *get_UseVSHostingProcess )( 
            CSharpProjectConfigurationProperties3 * This,
            /* [retval][out] */ __RPC__out VARIANT_BOOL *pbUseVSHostingProcess);
        
        /* [helpstring][id][propput] */ HRESULT ( STDMETHODCALLTYPE *put_UseVSHostingProcess )( 
            CSharpProjectConfigurationProperties3 * This,
            /* [in] */ VARIANT_BOOL UseVSHostingProcess);
        
        /* [helpstring][id][propget] */ HRESULT ( STDMETHODCALLTYPE *get_GenerateSerializationAssemblies )( 
            CSharpProjectConfigurationProperties3 * This,
            /* [retval][out] */ __RPC__out sgenGenerationOption *pSgenGenerationOption);
        
        /* [helpstring][id][propput] */ HRESULT ( STDMETHODCALLTYPE *put_GenerateSerializationAssemblies )( 
            CSharpProjectConfigurationProperties3 * This,
            /* [in] */ sgenGenerationOption SgenGenerationOption);
        
        /* [helpstring][id][propget] */ HRESULT ( STDMETHODCALLTYPE *get_LanguageVersion )( 
            CSharpProjectConfigurationProperties3 * This,
            /* [retval][out] */ __RPC__deref_out_opt BSTR *pbstrLanguageVersion);
        
        /* [helpstring][id][propput] */ HRESULT ( STDMETHODCALLTYPE *put_LanguageVersion )( 
            CSharpProjectConfigurationProperties3 * This,
            /* [in] */ __RPC__in BSTR LanguageVersion);
        
        /* [helpstring][id][propget] */ HRESULT ( STDMETHODCALLTYPE *get_ErrorReport )( 
            CSharpProjectConfigurationProperties3 * This,
            /* [retval][out] */ __RPC__deref_out_opt BSTR *pbstrErrorReport);
        
        /* [helpstring][id][propput] */ HRESULT ( STDMETHODCALLTYPE *put_ErrorReport )( 
            CSharpProjectConfigurationProperties3 * This,
            /* [in] */ __RPC__in BSTR ErrorReport);
        
        END_INTERFACE
    } CSharpProjectConfigurationProperties3Vtbl;

    interface CSharpProjectConfigurationProperties3
    {
        CONST_VTBL struct CSharpProjectConfigurationProperties3Vtbl *lpVtbl;
    };

    

#ifdef COBJMACROS


#define CSharpProjectConfigurationProperties3_QueryInterface(This,riid,ppvObj,retval)	\
    ( (This)->lpVtbl -> QueryInterface(This,riid,ppvObj,retval) ) 

#define CSharpProjectConfigurationProperties3_AddRef(This,retval)	\
    ( (This)->lpVtbl -> AddRef(This,retval) ) 

#define CSharpProjectConfigurationProperties3_Release(This,retval)	\
    ( (This)->lpVtbl -> Release(This,retval) ) 

#define CSharpProjectConfigurationProperties3_GetTypeInfoCount(This,pctinfo,retval)	\
    ( (This)->lpVtbl -> GetTypeInfoCount(This,pctinfo,retval) ) 

#define CSharpProjectConfigurationProperties3_GetTypeInfo(This,itinfo,lcid,pptinfo,retval)	\
    ( (This)->lpVtbl -> GetTypeInfo(This,itinfo,lcid,pptinfo,retval) ) 

#define CSharpProjectConfigurationProperties3_GetIDsOfNames(This,riid,rgszNames,cNames,lcid,rgdispid,retval)	\
    ( (This)->lpVtbl -> GetIDsOfNames(This,riid,rgszNames,cNames,lcid,rgdispid,retval) ) 

#define CSharpProjectConfigurationProperties3_Invoke(This,dispidMember,riid,lcid,wFlags,pdispparams,pvarResult,pexcepinfo,puArgErr,retval)	\
    ( (This)->lpVtbl -> Invoke(This,dispidMember,riid,lcid,wFlags,pdispparams,pvarResult,pexcepinfo,puArgErr,retval) ) 

#define CSharpProjectConfigurationProperties3_get___id(This,retval)	\
    ( (This)->lpVtbl -> get___id(This,retval) ) 

#define CSharpProjectConfigurationProperties3_get_DebugSymbols(This,retval)	\
    ( (This)->lpVtbl -> get_DebugSymbols(This,retval) ) 

#define CSharpProjectConfigurationProperties3_put_DebugSymbols(This,noname,retval)	\
    ( (This)->lpVtbl -> put_DebugSymbols(This,noname,retval) ) 

#define CSharpProjectConfigurationProperties3_get_DefineDebug(This,retval)	\
    ( (This)->lpVtbl -> get_DefineDebug(This,retval) ) 

#define CSharpProjectConfigurationProperties3_put_DefineDebug(This,noname,retval)	\
    ( (This)->lpVtbl -> put_DefineDebug(This,noname,retval) ) 

#define CSharpProjectConfigurationProperties3_get_DefineTrace(This,retval)	\
    ( (This)->lpVtbl -> get_DefineTrace(This,retval) ) 

#define CSharpProjectConfigurationProperties3_put_DefineTrace(This,noname,retval)	\
    ( (This)->lpVtbl -> put_DefineTrace(This,noname,retval) ) 

#define CSharpProjectConfigurationProperties3_get_OutputPath(This,retval)	\
    ( (This)->lpVtbl -> get_OutputPath(This,retval) ) 

#define CSharpProjectConfigurationProperties3_put_OutputPath(This,noname,retval)	\
    ( (This)->lpVtbl -> put_OutputPath(This,noname,retval) ) 

#define CSharpProjectConfigurationProperties3_get_IntermediatePath(This,retval)	\
    ( (This)->lpVtbl -> get_IntermediatePath(This,retval) ) 

#define CSharpProjectConfigurationProperties3_put_IntermediatePath(This,noname,retval)	\
    ( (This)->lpVtbl -> put_IntermediatePath(This,noname,retval) ) 

#define CSharpProjectConfigurationProperties3_get_DefineConstants(This,retval)	\
    ( (This)->lpVtbl -> get_DefineConstants(This,retval) ) 

#define CSharpProjectConfigurationProperties3_put_DefineConstants(This,noname,retval)	\
    ( (This)->lpVtbl -> put_DefineConstants(This,noname,retval) ) 

#define CSharpProjectConfigurationProperties3_get_RemoveIntegerChecks(This,retval)	\
    ( (This)->lpVtbl -> get_RemoveIntegerChecks(This,retval) ) 

#define CSharpProjectConfigurationProperties3_put_RemoveIntegerChecks(This,noname,retval)	\
    ( (This)->lpVtbl -> put_RemoveIntegerChecks(This,noname,retval) ) 

#define CSharpProjectConfigurationProperties3_get_BaseAddress(This,retval)	\
    ( (This)->lpVtbl -> get_BaseAddress(This,retval) ) 

#define CSharpProjectConfigurationProperties3_put_BaseAddress(This,noname,retval)	\
    ( (This)->lpVtbl -> put_BaseAddress(This,noname,retval) ) 

#define CSharpProjectConfigurationProperties3_get_AllowUnsafeBlocks(This,retval)	\
    ( (This)->lpVtbl -> get_AllowUnsafeBlocks(This,retval) ) 

#define CSharpProjectConfigurationProperties3_put_AllowUnsafeBlocks(This,noname,retval)	\
    ( (This)->lpVtbl -> put_AllowUnsafeBlocks(This,noname,retval) ) 

#define CSharpProjectConfigurationProperties3_get_CheckForOverflowUnderflow(This,retval)	\
    ( (This)->lpVtbl -> get_CheckForOverflowUnderflow(This,retval) ) 

#define CSharpProjectConfigurationProperties3_put_CheckForOverflowUnderflow(This,noname,retval)	\
    ( (This)->lpVtbl -> put_CheckForOverflowUnderflow(This,noname,retval) ) 

#define CSharpProjectConfigurationProperties3_get_DocumentationFile(This,retval)	\
    ( (This)->lpVtbl -> get_DocumentationFile(This,retval) ) 

#define CSharpProjectConfigurationProperties3_put_DocumentationFile(This,noname,retval)	\
    ( (This)->lpVtbl -> put_DocumentationFile(This,noname,retval) ) 

#define CSharpProjectConfigurationProperties3_get_Optimize(This,retval)	\
    ( (This)->lpVtbl -> get_Optimize(This,retval) ) 

#define CSharpProjectConfigurationProperties3_put_Optimize(This,noname,retval)	\
    ( (This)->lpVtbl -> put_Optimize(This,noname,retval) ) 

#define CSharpProjectConfigurationProperties3_get_IncrementalBuild(This,retval)	\
    ( (This)->lpVtbl -> get_IncrementalBuild(This,retval) ) 

#define CSharpProjectConfigurationProperties3_put_IncrementalBuild(This,noname,retval)	\
    ( (This)->lpVtbl -> put_IncrementalBuild(This,noname,retval) ) 

#define CSharpProjectConfigurationProperties3_get_StartProgram(This,retval)	\
    ( (This)->lpVtbl -> get_StartProgram(This,retval) ) 

#define CSharpProjectConfigurationProperties3_put_StartProgram(This,noname,retval)	\
    ( (This)->lpVtbl -> put_StartProgram(This,noname,retval) ) 

#define CSharpProjectConfigurationProperties3_get_StartWorkingDirectory(This,retval)	\
    ( (This)->lpVtbl -> get_StartWorkingDirectory(This,retval) ) 

#define CSharpProjectConfigurationProperties3_put_StartWorkingDirectory(This,noname,retval)	\
    ( (This)->lpVtbl -> put_StartWorkingDirectory(This,noname,retval) ) 

#define CSharpProjectConfigurationProperties3_get_StartURL(This,retval)	\
    ( (This)->lpVtbl -> get_StartURL(This,retval) ) 

#define CSharpProjectConfigurationProperties3_put_StartURL(This,noname,retval)	\
    ( (This)->lpVtbl -> put_StartURL(This,noname,retval) ) 

#define CSharpProjectConfigurationProperties3_get_StartPage(This,retval)	\
    ( (This)->lpVtbl -> get_StartPage(This,retval) ) 

#define CSharpProjectConfigurationProperties3_put_StartPage(This,noname,retval)	\
    ( (This)->lpVtbl -> put_StartPage(This,noname,retval) ) 

#define CSharpProjectConfigurationProperties3_get_StartArguments(This,retval)	\
    ( (This)->lpVtbl -> get_StartArguments(This,retval) ) 

#define CSharpProjectConfigurationProperties3_put_StartArguments(This,noname,retval)	\
    ( (This)->lpVtbl -> put_StartArguments(This,noname,retval) ) 

#define CSharpProjectConfigurationProperties3_get_StartWithIE(This,retval)	\
    ( (This)->lpVtbl -> get_StartWithIE(This,retval) ) 

#define CSharpProjectConfigurationProperties3_put_StartWithIE(This,noname,retval)	\
    ( (This)->lpVtbl -> put_StartWithIE(This,noname,retval) ) 

#define CSharpProjectConfigurationProperties3_get_EnableASPDebugging(This,retval)	\
    ( (This)->lpVtbl -> get_EnableASPDebugging(This,retval) ) 

#define CSharpProjectConfigurationProperties3_put_EnableASPDebugging(This,noname,retval)	\
    ( (This)->lpVtbl -> put_EnableASPDebugging(This,noname,retval) ) 

#define CSharpProjectConfigurationProperties3_get_EnableASPXDebugging(This,retval)	\
    ( (This)->lpVtbl -> get_EnableASPXDebugging(This,retval) ) 

#define CSharpProjectConfigurationProperties3_put_EnableASPXDebugging(This,noname,retval)	\
    ( (This)->lpVtbl -> put_EnableASPXDebugging(This,noname,retval) ) 

#define CSharpProjectConfigurationProperties3_get_EnableUnmanagedDebugging(This,retval)	\
    ( (This)->lpVtbl -> get_EnableUnmanagedDebugging(This,retval) ) 

#define CSharpProjectConfigurationProperties3_put_EnableUnmanagedDebugging(This,noname,retval)	\
    ( (This)->lpVtbl -> put_EnableUnmanagedDebugging(This,noname,retval) ) 

#define CSharpProjectConfigurationProperties3_get_StartAction(This,retval)	\
    ( (This)->lpVtbl -> get_StartAction(This,retval) ) 

#define CSharpProjectConfigurationProperties3_put_StartAction(This,noname,retval)	\
    ( (This)->lpVtbl -> put_StartAction(This,noname,retval) ) 

#define CSharpProjectConfigurationProperties3_get_Extender(This,ExtenderName,retval)	\
    ( (This)->lpVtbl -> get_Extender(This,ExtenderName,retval) ) 

#define CSharpProjectConfigurationProperties3_get_ExtenderNames(This,retval)	\
    ( (This)->lpVtbl -> get_ExtenderNames(This,retval) ) 

#define CSharpProjectConfigurationProperties3_get_ExtenderCATID(This,retval)	\
    ( (This)->lpVtbl -> get_ExtenderCATID(This,retval) ) 

#define CSharpProjectConfigurationProperties3_get_WarningLevel(This,retval)	\
    ( (This)->lpVtbl -> get_WarningLevel(This,retval) ) 

#define CSharpProjectConfigurationProperties3_put_WarningLevel(This,noname,retval)	\
    ( (This)->lpVtbl -> put_WarningLevel(This,noname,retval) ) 

#define CSharpProjectConfigurationProperties3_get_TreatWarningsAsErrors(This,retval)	\
    ( (This)->lpVtbl -> get_TreatWarningsAsErrors(This,retval) ) 

#define CSharpProjectConfigurationProperties3_put_TreatWarningsAsErrors(This,noname,retval)	\
    ( (This)->lpVtbl -> put_TreatWarningsAsErrors(This,noname,retval) ) 

#define CSharpProjectConfigurationProperties3_get_EnableSQLServerDebugging(This,retval)	\
    ( (This)->lpVtbl -> get_EnableSQLServerDebugging(This,retval) ) 

#define CSharpProjectConfigurationProperties3_put_EnableSQLServerDebugging(This,noname,retval)	\
    ( (This)->lpVtbl -> put_EnableSQLServerDebugging(This,noname,retval) ) 

#define CSharpProjectConfigurationProperties3_get_FileAlignment(This,retval)	\
    ( (This)->lpVtbl -> get_FileAlignment(This,retval) ) 

#define CSharpProjectConfigurationProperties3_put_FileAlignment(This,noname,retval)	\
    ( (This)->lpVtbl -> put_FileAlignment(This,noname,retval) ) 

#define CSharpProjectConfigurationProperties3_get_RegisterForComInterop(This,retval)	\
    ( (This)->lpVtbl -> get_RegisterForComInterop(This,retval) ) 

#define CSharpProjectConfigurationProperties3_put_RegisterForComInterop(This,noname,retval)	\
    ( (This)->lpVtbl -> put_RegisterForComInterop(This,noname,retval) ) 

#define CSharpProjectConfigurationProperties3_get_ConfigurationOverrideFile(This,retval)	\
    ( (This)->lpVtbl -> get_ConfigurationOverrideFile(This,retval) ) 

#define CSharpProjectConfigurationProperties3_put_ConfigurationOverrideFile(This,noname,retval)	\
    ( (This)->lpVtbl -> put_ConfigurationOverrideFile(This,noname,retval) ) 

#define CSharpProjectConfigurationProperties3_get_RemoteDebugEnabled(This,retval)	\
    ( (This)->lpVtbl -> get_RemoteDebugEnabled(This,retval) ) 

#define CSharpProjectConfigurationProperties3_put_RemoteDebugEnabled(This,noname,retval)	\
    ( (This)->lpVtbl -> put_RemoteDebugEnabled(This,noname,retval) ) 

#define CSharpProjectConfigurationProperties3_get_RemoteDebugMachine(This,retval)	\
    ( (This)->lpVtbl -> get_RemoteDebugMachine(This,retval) ) 

#define CSharpProjectConfigurationProperties3_put_RemoteDebugMachine(This,noname,retval)	\
    ( (This)->lpVtbl -> put_RemoteDebugMachine(This,noname,retval) ) 

#define CSharpProjectConfigurationProperties3_get_NoWarn(This,retval)	\
    ( (This)->lpVtbl -> get_NoWarn(This,retval) ) 

#define CSharpProjectConfigurationProperties3_put_NoWarn(This,noname,retval)	\
    ( (This)->lpVtbl -> put_NoWarn(This,noname,retval) ) 

#define CSharpProjectConfigurationProperties3_get_NoStdLib(This,retval)	\
    ( (This)->lpVtbl -> get_NoStdLib(This,retval) ) 

#define CSharpProjectConfigurationProperties3_put_NoStdLib(This,noname,retval)	\
    ( (This)->lpVtbl -> put_NoStdLib(This,noname,retval) ) 


#define CSharpProjectConfigurationProperties3_get_DebugInfo(This,pbstrDebugInfo)	\
    ( (This)->lpVtbl -> get_DebugInfo(This,pbstrDebugInfo) ) 

#define CSharpProjectConfigurationProperties3_put_DebugInfo(This,DebugInfo)	\
    ( (This)->lpVtbl -> put_DebugInfo(This,DebugInfo) ) 

#define CSharpProjectConfigurationProperties3_get_PlatformTarget(This,pbstrPlatformTarget)	\
    ( (This)->lpVtbl -> get_PlatformTarget(This,pbstrPlatformTarget) ) 

#define CSharpProjectConfigurationProperties3_put_PlatformTarget(This,PlatformTarget)	\
    ( (This)->lpVtbl -> put_PlatformTarget(This,PlatformTarget) ) 

#define CSharpProjectConfigurationProperties3_get_TreatSpecificWarningsAsErrors(This,pbstrWarningsAsErrors)	\
    ( (This)->lpVtbl -> get_TreatSpecificWarningsAsErrors(This,pbstrWarningsAsErrors) ) 

#define CSharpProjectConfigurationProperties3_put_TreatSpecificWarningsAsErrors(This,WarningsAsErrors)	\
    ( (This)->lpVtbl -> put_TreatSpecificWarningsAsErrors(This,WarningsAsErrors) ) 

#define CSharpProjectConfigurationProperties3_get_RunCodeAnalysis(This,pbRunCodeAnalysis)	\
    ( (This)->lpVtbl -> get_RunCodeAnalysis(This,pbRunCodeAnalysis) ) 

#define CSharpProjectConfigurationProperties3_put_RunCodeAnalysis(This,RunCodeAnalysis)	\
    ( (This)->lpVtbl -> put_RunCodeAnalysis(This,RunCodeAnalysis) ) 

#define CSharpProjectConfigurationProperties3_get_CodeAnalysisLogFile(This,pbstrCodeAnalysisLogFile)	\
    ( (This)->lpVtbl -> get_CodeAnalysisLogFile(This,pbstrCodeAnalysisLogFile) ) 

#define CSharpProjectConfigurationProperties3_put_CodeAnalysisLogFile(This,CodeAnalysisLogFile)	\
    ( (This)->lpVtbl -> put_CodeAnalysisLogFile(This,CodeAnalysisLogFile) ) 

#define CSharpProjectConfigurationProperties3_get_CodeAnalysisRuleAssemblies(This,pbstrCodeAnalysisRuleAssemblies)	\
    ( (This)->lpVtbl -> get_CodeAnalysisRuleAssemblies(This,pbstrCodeAnalysisRuleAssemblies) ) 

#define CSharpProjectConfigurationProperties3_put_CodeAnalysisRuleAssemblies(This,CodeAnalysisRuleAssemblies)	\
    ( (This)->lpVtbl -> put_CodeAnalysisRuleAssemblies(This,CodeAnalysisRuleAssemblies) ) 

#define CSharpProjectConfigurationProperties3_get_CodeAnalysisInputAssembly(This,pbstrCodeAnalysisInputAssembly)	\
    ( (This)->lpVtbl -> get_CodeAnalysisInputAssembly(This,pbstrCodeAnalysisInputAssembly) ) 

#define CSharpProjectConfigurationProperties3_put_CodeAnalysisInputAssembly(This,CodeAnalysisInputAssembly)	\
    ( (This)->lpVtbl -> put_CodeAnalysisInputAssembly(This,CodeAnalysisInputAssembly) ) 

#define CSharpProjectConfigurationProperties3_get_CodeAnalysisRules(This,pbstrCodeAnalysisRules)	\
    ( (This)->lpVtbl -> get_CodeAnalysisRules(This,pbstrCodeAnalysisRules) ) 

#define CSharpProjectConfigurationProperties3_put_CodeAnalysisRules(This,CodeAnalysisRules)	\
    ( (This)->lpVtbl -> put_CodeAnalysisRules(This,CodeAnalysisRules) ) 

#define CSharpProjectConfigurationProperties3_get_CodeAnalysisSpellCheckLanguages(This,pbstrCodeAnalysisSpellCheckLanguages)	\
    ( (This)->lpVtbl -> get_CodeAnalysisSpellCheckLanguages(This,pbstrCodeAnalysisSpellCheckLanguages) ) 

#define CSharpProjectConfigurationProperties3_put_CodeAnalysisSpellCheckLanguages(This,CodeAnalysisSpellCheckLanguages)	\
    ( (This)->lpVtbl -> put_CodeAnalysisSpellCheckLanguages(This,CodeAnalysisSpellCheckLanguages) ) 

#define CSharpProjectConfigurationProperties3_get_CodeAnalysisUseTypeNameInSuppression(This,bUseTypeNameInSuppression)	\
    ( (This)->lpVtbl -> get_CodeAnalysisUseTypeNameInSuppression(This,bUseTypeNameInSuppression) ) 

#define CSharpProjectConfigurationProperties3_put_CodeAnalysisUseTypeNameInSuppression(This,UseTypeNameInSuppression)	\
    ( (This)->lpVtbl -> put_CodeAnalysisUseTypeNameInSuppression(This,UseTypeNameInSuppression) ) 

#define CSharpProjectConfigurationProperties3_get_CodeAnalysisModuleSuppressionsFile(This,pbstrCodeAnalysisModuleSuppressionsFile)	\
    ( (This)->lpVtbl -> get_CodeAnalysisModuleSuppressionsFile(This,pbstrCodeAnalysisModuleSuppressionsFile) ) 

#define CSharpProjectConfigurationProperties3_put_CodeAnalysisModuleSuppressionsFile(This,CodeAnalysisModuleSuppressionsFile)	\
    ( (This)->lpVtbl -> put_CodeAnalysisModuleSuppressionsFile(This,CodeAnalysisModuleSuppressionsFile) ) 

#define CSharpProjectConfigurationProperties3_get_UseVSHostingProcess(This,pbUseVSHostingProcess)	\
    ( (This)->lpVtbl -> get_UseVSHostingProcess(This,pbUseVSHostingProcess) ) 

#define CSharpProjectConfigurationProperties3_put_UseVSHostingProcess(This,UseVSHostingProcess)	\
    ( (This)->lpVtbl -> put_UseVSHostingProcess(This,UseVSHostingProcess) ) 

#define CSharpProjectConfigurationProperties3_get_GenerateSerializationAssemblies(This,pSgenGenerationOption)	\
    ( (This)->lpVtbl -> get_GenerateSerializationAssemblies(This,pSgenGenerationOption) ) 

#define CSharpProjectConfigurationProperties3_put_GenerateSerializationAssemblies(This,SgenGenerationOption)	\
    ( (This)->lpVtbl -> put_GenerateSerializationAssemblies(This,SgenGenerationOption) ) 


#define CSharpProjectConfigurationProperties3_get_LanguageVersion(This,pbstrLanguageVersion)	\
    ( (This)->lpVtbl -> get_LanguageVersion(This,pbstrLanguageVersion) ) 

#define CSharpProjectConfigurationProperties3_put_LanguageVersion(This,LanguageVersion)	\
    ( (This)->lpVtbl -> put_LanguageVersion(This,LanguageVersion) ) 

#define CSharpProjectConfigurationProperties3_get_ErrorReport(This,pbstrErrorReport)	\
    ( (This)->lpVtbl -> get_ErrorReport(This,pbstrErrorReport) ) 

#define CSharpProjectConfigurationProperties3_put_ErrorReport(This,ErrorReport)	\
    ( (This)->lpVtbl -> put_ErrorReport(This,ErrorReport) ) 

#endif /* COBJMACROS */


#endif 	/* C style interface */




#endif 	/* __CSharpProjectConfigurationProperties3_INTERFACE_DEFINED__ */


#ifndef __Reference3_INTERFACE_DEFINED__
#define __Reference3_INTERFACE_DEFINED__

/* interface Reference3 */
/* [uuid][object][oleautomation][dual] */ 


EXTERN_C const IID IID_Reference3;

#if defined(__cplusplus) && !defined(CINTERFACE)
    
    MIDL_INTERFACE("5021602E-2025-4299-88D2-0A92E8B41ADF")
    Reference3 : public Reference2
    {
    public:
        virtual /* [helpstring][propget][id] */ HRESULT STDMETHODCALLTYPE get_SpecificVersion( 
            /* [retval][out] */ __RPC__out VARIANT_BOOL *pbSpecificVersion) = 0;
        
        virtual /* [helpstring][propput][id] */ HRESULT STDMETHODCALLTYPE put_SpecificVersion( 
            /* [in] */ VARIANT_BOOL SpecificVersion) = 0;
        
        virtual /* [helpstring][propget][id] */ HRESULT STDMETHODCALLTYPE get_SubType( 
            /* [retval][out] */ __RPC__deref_out_opt BSTR *pbstrSubType) = 0;
        
        virtual /* [helpstring][propput][id] */ HRESULT STDMETHODCALLTYPE put_SubType( 
            /* [in] */ __RPC__in BSTR SubType) = 0;
        
        virtual /* [helpstring][propget][id] */ HRESULT STDMETHODCALLTYPE get_Isolated( 
            /* [retval][out] */ __RPC__out VARIANT_BOOL *pbIsolated) = 0;
        
        virtual /* [helpstring][propput][id] */ HRESULT STDMETHODCALLTYPE put_Isolated( 
            /* [in] */ VARIANT_BOOL Isolated) = 0;
        
        virtual /* [helpstring][propget][id] */ HRESULT STDMETHODCALLTYPE get_Aliases( 
            /* [retval][out] */ __RPC__deref_out_opt BSTR *pbstrAliases) = 0;
        
        virtual /* [helpstring][propput][id] */ HRESULT STDMETHODCALLTYPE put_Aliases( 
            /* [in] */ __RPC__in BSTR Aliases) = 0;
        
        virtual /* [helpstring][propget][id] */ HRESULT STDMETHODCALLTYPE get_RefType( 
            /* [retval][out] */ __RPC__out PROJECTREFERENCETYPE *pProjRefType) = 0;
        
        virtual /* [helpstring][propget][id] */ HRESULT STDMETHODCALLTYPE get_AutoReferenced( 
            /* [retval][out] */ __RPC__out VARIANT_BOOL *pbAutoReferenced) = 0;
        
        virtual /* [helpstring][propget][id] */ HRESULT STDMETHODCALLTYPE get_Resolved( 
            /* [retval][out] */ __RPC__out VARIANT_BOOL *pbResolved) = 0;
        
    };
    
#else 	/* C style interface */

    typedef struct Reference3Vtbl
    {
        BEGIN_INTERFACE
        
        /* [id][restricted][funcdescattr] */ HRESULT ( STDMETHODCALLTYPE *QueryInterface )( 
            Reference3 * This,
            /* [in][idldescattr] */ __RPC__in struct GUID *riid,
            /* [out][idldescattr] */ __RPC__deref_out_opt void **ppvObj,
            /* [retval][out] */ __RPC__out void *retval);
        
        /* [id][restricted][funcdescattr] */ HRESULT ( STDMETHODCALLTYPE *AddRef )( 
            Reference3 * This,
            /* [retval][out] */ __RPC__out unsigned long *retval);
        
        /* [id][restricted][funcdescattr] */ HRESULT ( STDMETHODCALLTYPE *Release )( 
            Reference3 * This,
            /* [retval][out] */ __RPC__out unsigned long *retval);
        
        /* [id][restricted][funcdescattr] */ HRESULT ( STDMETHODCALLTYPE *GetTypeInfoCount )( 
            Reference3 * This,
            /* [out][idldescattr] */ __RPC__out unsigned UINT *pctinfo,
            /* [retval][out] */ __RPC__out void *retval);
        
        /* [id][restricted][funcdescattr] */ HRESULT ( STDMETHODCALLTYPE *GetTypeInfo )( 
            Reference3 * This,
            /* [in][idldescattr] */ unsigned UINT itinfo,
            /* [in][idldescattr] */ unsigned long lcid,
            /* [out][idldescattr] */ __RPC__deref_out_opt void **pptinfo,
            /* [retval][out] */ __RPC__out void *retval);
        
        /* [id][restricted][funcdescattr] */ HRESULT ( STDMETHODCALLTYPE *GetIDsOfNames )( 
            Reference3 * This,
            /* [in][idldescattr] */ __RPC__in struct GUID *riid,
            /* [in][idldescattr] */ __RPC__deref_in_opt signed char **rgszNames,
            /* [in][idldescattr] */ unsigned UINT cNames,
            /* [in][idldescattr] */ unsigned long lcid,
            /* [out][idldescattr] */ __RPC__out signed long *rgdispid,
            /* [retval][out] */ __RPC__out void *retval);
        
        /* [id][restricted][funcdescattr] */ HRESULT ( STDMETHODCALLTYPE *Invoke )( 
            Reference3 * This,
            /* [in][idldescattr] */ signed long dispidMember,
            /* [in][idldescattr] */ __RPC__in struct GUID *riid,
            /* [in][idldescattr] */ unsigned long lcid,
            /* [in][idldescattr] */ unsigned short wFlags,
            /* [in][idldescattr] */ __RPC__in struct DISPPARAMS *pdispparams,
            /* [out][idldescattr] */ __RPC__out VARIANT *pvarResult,
            /* [out][idldescattr] */ __RPC__out struct EXCEPINFO *pexcepinfo,
            /* [out][idldescattr] */ __RPC__out unsigned UINT *puArgErr,
            /* [retval][out] */ __RPC__out void *retval);
        
        /* [id][propget][funcdescattr] */ HRESULT ( STDMETHODCALLTYPE *get_DTE )( 
            Reference3 * This,
            /* [retval][out] */ __RPC__deref_out_opt **retval);
        
        /* [id][propget][funcdescattr] */ HRESULT ( STDMETHODCALLTYPE *get_Collection )( 
            Reference3 * This,
            /* [retval][out] */ __RPC__deref_out_opt References **retval);
        
        /* [id][propget][funcdescattr] */ HRESULT ( STDMETHODCALLTYPE *get_ContainingProject )( 
            Reference3 * This,
            /* [retval][out] */ __RPC__deref_out_opt Project **retval);
        
        /* [id][funcdescattr] */ HRESULT ( STDMETHODCALLTYPE *Remove )( 
            Reference3 * This,
            /* [retval][out] */ __RPC__out void *retval);
        
        /* [id][propget][funcdescattr] */ HRESULT ( STDMETHODCALLTYPE *get_Name )( 
            Reference3 * This,
            /* [retval][out] */ __RPC__deref_out_opt BSTR *retval);
        
        /* [id][propget][funcdescattr] */ HRESULT ( STDMETHODCALLTYPE *get_Type )( 
            Reference3 * This,
            /* [retval][out] */ __RPC__out enum prjReferenceType *retval);
        
        /* [id][propget][funcdescattr] */ HRESULT ( STDMETHODCALLTYPE *get_Identity )( 
            Reference3 * This,
            /* [retval][out] */ __RPC__deref_out_opt BSTR *retval);
        
        /* [id][propget][funcdescattr] */ HRESULT ( STDMETHODCALLTYPE *get_Path )( 
            Reference3 * This,
            /* [retval][out] */ __RPC__deref_out_opt BSTR *retval);
        
        /* [id][propget][funcdescattr] */ HRESULT ( STDMETHODCALLTYPE *get_Description )( 
            Reference3 * This,
            /* [retval][out] */ __RPC__deref_out_opt BSTR *retval);
        
        /* [id][propget][funcdescattr] */ HRESULT ( STDMETHODCALLTYPE *get_Culture )( 
            Reference3 * This,
            /* [retval][out] */ __RPC__deref_out_opt BSTR *retval);
        
        /* [id][propget][funcdescattr] */ HRESULT ( STDMETHODCALLTYPE *get_MajorVersion )( 
            Reference3 * This,
            /* [retval][out] */ __RPC__out signed long *retval);
        
        /* [id][propget][funcdescattr] */ HRESULT ( STDMETHODCALLTYPE *get_MinorVersion )( 
            Reference3 * This,
            /* [retval][out] */ __RPC__out signed long *retval);
        
        /* [id][propget][funcdescattr] */ HRESULT ( STDMETHODCALLTYPE *get_RevisionNumber )( 
            Reference3 * This,
            /* [retval][out] */ __RPC__out signed long *retval);
        
        /* [id][propget][funcdescattr] */ HRESULT ( STDMETHODCALLTYPE *get_BuildNumber )( 
            Reference3 * This,
            /* [retval][out] */ __RPC__out signed long *retval);
        
        /* [id][propget][funcdescattr] */ HRESULT ( STDMETHODCALLTYPE *get_StrongName )( 
            Reference3 * This,
            /* [retval][out] */ __RPC__out BOOLEAN *retval);
        
        /* [id][propget][funcdescattr] */ HRESULT ( STDMETHODCALLTYPE *get_SourceProject )( 
            Reference3 * This,
            /* [retval][out] */ __RPC__deref_out_opt Project **retval);
        
        /* [id][propget][funcdescattr] */ HRESULT ( STDMETHODCALLTYPE *get_CopyLocal )( 
            Reference3 * This,
            /* [retval][out] */ __RPC__out BOOLEAN *retval);
        
        /* [id][propput][funcdescattr] */ HRESULT ( STDMETHODCALLTYPE *put_CopyLocal )( 
            Reference3 * This,
            /* [in][idldescattr] */ BOOLEAN noname,
            /* [retval][out] */ __RPC__out void *retval);
        
        /* [id][propget][funcdescattr] */ HRESULT ( STDMETHODCALLTYPE *get_Extender )( 
            Reference3 * This,
            /* [in][idldescattr] */ __RPC__in BSTR ExtenderName,
            /* [retval][out] */ __RPC__deref_out_opt IDispatch **retval);
        
        /* [id][propget][funcdescattr] */ HRESULT ( STDMETHODCALLTYPE *get_ExtenderNames )( 
            Reference3 * This,
            /* [retval][out] */ __RPC__out VARIANT *retval);
        
        /* [id][propget][funcdescattr] */ HRESULT ( STDMETHODCALLTYPE *get_ExtenderCATID )( 
            Reference3 * This,
            /* [retval][out] */ __RPC__deref_out_opt BSTR *retval);
        
        /* [id][propget][funcdescattr] */ HRESULT ( STDMETHODCALLTYPE *get_PublicKeyToken )( 
            Reference3 * This,
            /* [retval][out] */ __RPC__deref_out_opt BSTR *retval);
        
        /* [id][propget][funcdescattr] */ HRESULT ( STDMETHODCALLTYPE *get_Version )( 
            Reference3 * This,
            /* [retval][out] */ __RPC__deref_out_opt BSTR *retval);
        
        /* [id][propget][funcdescattr] */ HRESULT ( STDMETHODCALLTYPE *get_RuntimeVersion )( 
            Reference3 * This,
            /* [retval][out] */ __RPC__deref_out_opt BSTR *retval);
        
        /* [helpstring][propget][id] */ HRESULT ( STDMETHODCALLTYPE *get_SpecificVersion )( 
            Reference3 * This,
            /* [retval][out] */ __RPC__out VARIANT_BOOL *pbSpecificVersion);
        
        /* [helpstring][propput][id] */ HRESULT ( STDMETHODCALLTYPE *put_SpecificVersion )( 
            Reference3 * This,
            /* [in] */ VARIANT_BOOL SpecificVersion);
        
        /* [helpstring][propget][id] */ HRESULT ( STDMETHODCALLTYPE *get_SubType )( 
            Reference3 * This,
            /* [retval][out] */ __RPC__deref_out_opt BSTR *pbstrSubType);
        
        /* [helpstring][propput][id] */ HRESULT ( STDMETHODCALLTYPE *put_SubType )( 
            Reference3 * This,
            /* [in] */ __RPC__in BSTR SubType);
        
        /* [helpstring][propget][id] */ HRESULT ( STDMETHODCALLTYPE *get_Isolated )( 
            Reference3 * This,
            /* [retval][out] */ __RPC__out VARIANT_BOOL *pbIsolated);
        
        /* [helpstring][propput][id] */ HRESULT ( STDMETHODCALLTYPE *put_Isolated )( 
            Reference3 * This,
            /* [in] */ VARIANT_BOOL Isolated);
        
        /* [helpstring][propget][id] */ HRESULT ( STDMETHODCALLTYPE *get_Aliases )( 
            Reference3 * This,
            /* [retval][out] */ __RPC__deref_out_opt BSTR *pbstrAliases);
        
        /* [helpstring][propput][id] */ HRESULT ( STDMETHODCALLTYPE *put_Aliases )( 
            Reference3 * This,
            /* [in] */ __RPC__in BSTR Aliases);
        
        /* [helpstring][propget][id] */ HRESULT ( STDMETHODCALLTYPE *get_RefType )( 
            Reference3 * This,
            /* [retval][out] */ __RPC__out PROJECTREFERENCETYPE *pProjRefType);
        
        /* [helpstring][propget][id] */ HRESULT ( STDMETHODCALLTYPE *get_AutoReferenced )( 
            Reference3 * This,
            /* [retval][out] */ __RPC__out VARIANT_BOOL *pbAutoReferenced);
        
        /* [helpstring][propget][id] */ HRESULT ( STDMETHODCALLTYPE *get_Resolved )( 
            Reference3 * This,
            /* [retval][out] */ __RPC__out VARIANT_BOOL *pbResolved);
        
        END_INTERFACE
    } Reference3Vtbl;

    interface Reference3
    {
        CONST_VTBL struct Reference3Vtbl *lpVtbl;
    };

    

#ifdef COBJMACROS


#define Reference3_QueryInterface(This,riid,ppvObj,retval)	\
    ( (This)->lpVtbl -> QueryInterface(This,riid,ppvObj,retval) ) 

#define Reference3_AddRef(This,retval)	\
    ( (This)->lpVtbl -> AddRef(This,retval) ) 

#define Reference3_Release(This,retval)	\
    ( (This)->lpVtbl -> Release(This,retval) ) 

#define Reference3_GetTypeInfoCount(This,pctinfo,retval)	\
    ( (This)->lpVtbl -> GetTypeInfoCount(This,pctinfo,retval) ) 

#define Reference3_GetTypeInfo(This,itinfo,lcid,pptinfo,retval)	\
    ( (This)->lpVtbl -> GetTypeInfo(This,itinfo,lcid,pptinfo,retval) ) 

#define Reference3_GetIDsOfNames(This,riid,rgszNames,cNames,lcid,rgdispid,retval)	\
    ( (This)->lpVtbl -> GetIDsOfNames(This,riid,rgszNames,cNames,lcid,rgdispid,retval) ) 

#define Reference3_Invoke(This,dispidMember,riid,lcid,wFlags,pdispparams,pvarResult,pexcepinfo,puArgErr,retval)	\
    ( (This)->lpVtbl -> Invoke(This,dispidMember,riid,lcid,wFlags,pdispparams,pvarResult,pexcepinfo,puArgErr,retval) ) 

#define Reference3_get_DTE(This,retval)	\
    ( (This)->lpVtbl -> get_DTE(This,retval) ) 

#define Reference3_get_Collection(This,retval)	\
    ( (This)->lpVtbl -> get_Collection(This,retval) ) 

#define Reference3_get_ContainingProject(This,retval)	\
    ( (This)->lpVtbl -> get_ContainingProject(This,retval) ) 

#define Reference3_Remove(This,retval)	\
    ( (This)->lpVtbl -> Remove(This,retval) ) 

#define Reference3_get_Name(This,retval)	\
    ( (This)->lpVtbl -> get_Name(This,retval) ) 

#define Reference3_get_Type(This,retval)	\
    ( (This)->lpVtbl -> get_Type(This,retval) ) 

#define Reference3_get_Identity(This,retval)	\
    ( (This)->lpVtbl -> get_Identity(This,retval) ) 

#define Reference3_get_Path(This,retval)	\
    ( (This)->lpVtbl -> get_Path(This,retval) ) 

#define Reference3_get_Description(This,retval)	\
    ( (This)->lpVtbl -> get_Description(This,retval) ) 

#define Reference3_get_Culture(This,retval)	\
    ( (This)->lpVtbl -> get_Culture(This,retval) ) 

#define Reference3_get_MajorVersion(This,retval)	\
    ( (This)->lpVtbl -> get_MajorVersion(This,retval) ) 

#define Reference3_get_MinorVersion(This,retval)	\
    ( (This)->lpVtbl -> get_MinorVersion(This,retval) ) 

#define Reference3_get_RevisionNumber(This,retval)	\
    ( (This)->lpVtbl -> get_RevisionNumber(This,retval) ) 

#define Reference3_get_BuildNumber(This,retval)	\
    ( (This)->lpVtbl -> get_BuildNumber(This,retval) ) 

#define Reference3_get_StrongName(This,retval)	\
    ( (This)->lpVtbl -> get_StrongName(This,retval) ) 

#define Reference3_get_SourceProject(This,retval)	\
    ( (This)->lpVtbl -> get_SourceProject(This,retval) ) 

#define Reference3_get_CopyLocal(This,retval)	\
    ( (This)->lpVtbl -> get_CopyLocal(This,retval) ) 

#define Reference3_put_CopyLocal(This,noname,retval)	\
    ( (This)->lpVtbl -> put_CopyLocal(This,noname,retval) ) 

#define Reference3_get_Extender(This,ExtenderName,retval)	\
    ( (This)->lpVtbl -> get_Extender(This,ExtenderName,retval) ) 

#define Reference3_get_ExtenderNames(This,retval)	\
    ( (This)->lpVtbl -> get_ExtenderNames(This,retval) ) 

#define Reference3_get_ExtenderCATID(This,retval)	\
    ( (This)->lpVtbl -> get_ExtenderCATID(This,retval) ) 

#define Reference3_get_PublicKeyToken(This,retval)	\
    ( (This)->lpVtbl -> get_PublicKeyToken(This,retval) ) 

#define Reference3_get_Version(This,retval)	\
    ( (This)->lpVtbl -> get_Version(This,retval) ) 

#define Reference3_get_RuntimeVersion(This,retval)	\
    ( (This)->lpVtbl -> get_RuntimeVersion(This,retval) ) 


#define Reference3_get_SpecificVersion(This,pbSpecificVersion)	\
    ( (This)->lpVtbl -> get_SpecificVersion(This,pbSpecificVersion) ) 

#define Reference3_put_SpecificVersion(This,SpecificVersion)	\
    ( (This)->lpVtbl -> put_SpecificVersion(This,SpecificVersion) ) 

#define Reference3_get_SubType(This,pbstrSubType)	\
    ( (This)->lpVtbl -> get_SubType(This,pbstrSubType) ) 

#define Reference3_put_SubType(This,SubType)	\
    ( (This)->lpVtbl -> put_SubType(This,SubType) ) 

#define Reference3_get_Isolated(This,pbIsolated)	\
    ( (This)->lpVtbl -> get_Isolated(This,pbIsolated) ) 

#define Reference3_put_Isolated(This,Isolated)	\
    ( (This)->lpVtbl -> put_Isolated(This,Isolated) ) 

#define Reference3_get_Aliases(This,pbstrAliases)	\
    ( (This)->lpVtbl -> get_Aliases(This,pbstrAliases) ) 

#define Reference3_put_Aliases(This,Aliases)	\
    ( (This)->lpVtbl -> put_Aliases(This,Aliases) ) 

#define Reference3_get_RefType(This,pProjRefType)	\
    ( (This)->lpVtbl -> get_RefType(This,pProjRefType) ) 

#define Reference3_get_AutoReferenced(This,pbAutoReferenced)	\
    ( (This)->lpVtbl -> get_AutoReferenced(This,pbAutoReferenced) ) 

#define Reference3_get_Resolved(This,pbResolved)	\
    ( (This)->lpVtbl -> get_Resolved(This,pbResolved) ) 

#endif /* COBJMACROS */


#endif 	/* C style interface */




#endif 	/* __Reference3_INTERFACE_DEFINED__ */


#ifndef __JSharpProjectConfigurationProperties3_INTERFACE_DEFINED__
#define __JSharpProjectConfigurationProperties3_INTERFACE_DEFINED__

/* interface JSharpProjectConfigurationProperties3 */
/* [object][dual][unique][helpstring][uuid] */ 

typedef /* [uuid] */  DECLSPEC_UUID("3600DDB6-34C2-4CED-9337-5B266CC0A169") 
enum prjDisableLangXtns
    {	prjDisableLangXtnsNone	= 0,
	prjDisableLangXtnsNET	= ( prjDisableLangXtnsNone + 1 ) ,
	prjDisableLangXtnsAll	= ( prjDisableLangXtnsNET + 1 ) 
    } 	prjDisableLangXtns;

#define prjDisableLangXtnsMin        prjDisableLangXtnsNone
#define prjDisableLangXtnsMax        prjDisableLangXtnsAll

EXTERN_C const IID IID_JSharpProjectConfigurationProperties3;

#if defined(__cplusplus) && !defined(CINTERFACE)
    
    MIDL_INTERFACE("3600DDB7-34C2-4CED-9337-5B266CC0A169")
    JSharpProjectConfigurationProperties3 : public ProjectConfigurationProperties3
    {
    public:
        virtual /* [helpstring][id][propget] */ HRESULT STDMETHODCALLTYPE get_CodePage( 
            /* [retval][out] */ __RPC__deref_out_opt BSTR *pbstrCodePage) = 0;
        
        virtual /* [helpstring][id][propput] */ HRESULT STDMETHODCALLTYPE put_CodePage( 
            /* [in] */ __RPC__in BSTR CodePage) = 0;
        
        virtual /* [helpstring][id][propget] */ HRESULT STDMETHODCALLTYPE get_JCPA( 
            /* [retval][out] */ __RPC__deref_out_opt BSTR *pbstrJCPA) = 0;
        
        virtual /* [helpstring][id][propput] */ HRESULT STDMETHODCALLTYPE put_JCPA( 
            /* [in] */ __RPC__in BSTR JCPA) = 0;
        
        virtual /* [helpstring][id][propget] */ HRESULT STDMETHODCALLTYPE get_DisableLangXtns( 
            /* [retval][out] */ __RPC__out prjDisableLangXtns *pDisableLangXtns) = 0;
        
        virtual /* [helpstring][id][propput] */ HRESULT STDMETHODCALLTYPE put_DisableLangXtns( 
            /* [in] */ prjDisableLangXtns DisableLangXtns) = 0;
        
        virtual /* [helpstring][id][propget] */ HRESULT STDMETHODCALLTYPE get_SecureScoping( 
            /* [retval][out] */ __RPC__out VARIANT_BOOL *pbSecureScoping) = 0;
        
        virtual /* [helpstring][id][propput] */ HRESULT STDMETHODCALLTYPE put_SecureScoping( 
            /* [in] */ VARIANT_BOOL bSecureScoping) = 0;
        
    };
    
#else 	/* C style interface */

    typedef struct JSharpProjectConfigurationProperties3Vtbl
    {
        BEGIN_INTERFACE
        
        /* [id][restricted][funcdescattr] */ HRESULT ( STDMETHODCALLTYPE *QueryInterface )( 
            JSharpProjectConfigurationProperties3 * This,
            /* [in][idldescattr] */ __RPC__in struct GUID *riid,
            /* [out][idldescattr] */ __RPC__deref_out_opt void **ppvObj,
            /* [retval][out] */ __RPC__out void *retval);
        
        /* [id][restricted][funcdescattr] */ HRESULT ( STDMETHODCALLTYPE *AddRef )( 
            JSharpProjectConfigurationProperties3 * This,
            /* [retval][out] */ __RPC__out unsigned long *retval);
        
        /* [id][restricted][funcdescattr] */ HRESULT ( STDMETHODCALLTYPE *Release )( 
            JSharpProjectConfigurationProperties3 * This,
            /* [retval][out] */ __RPC__out unsigned long *retval);
        
        /* [id][restricted][funcdescattr] */ HRESULT ( STDMETHODCALLTYPE *GetTypeInfoCount )( 
            JSharpProjectConfigurationProperties3 * This,
            /* [out][idldescattr] */ __RPC__out unsigned UINT *pctinfo,
            /* [retval][out] */ __RPC__out void *retval);
        
        /* [id][restricted][funcdescattr] */ HRESULT ( STDMETHODCALLTYPE *GetTypeInfo )( 
            JSharpProjectConfigurationProperties3 * This,
            /* [in][idldescattr] */ unsigned UINT itinfo,
            /* [in][idldescattr] */ unsigned long lcid,
            /* [out][idldescattr] */ __RPC__deref_out_opt void **pptinfo,
            /* [retval][out] */ __RPC__out void *retval);
        
        /* [id][restricted][funcdescattr] */ HRESULT ( STDMETHODCALLTYPE *GetIDsOfNames )( 
            JSharpProjectConfigurationProperties3 * This,
            /* [in][idldescattr] */ __RPC__in struct GUID *riid,
            /* [in][idldescattr] */ __RPC__deref_in_opt signed char **rgszNames,
            /* [in][idldescattr] */ unsigned UINT cNames,
            /* [in][idldescattr] */ unsigned long lcid,
            /* [out][idldescattr] */ __RPC__out signed long *rgdispid,
            /* [retval][out] */ __RPC__out void *retval);
        
        /* [id][restricted][funcdescattr] */ HRESULT ( STDMETHODCALLTYPE *Invoke )( 
            JSharpProjectConfigurationProperties3 * This,
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
            JSharpProjectConfigurationProperties3 * This,
            /* [retval][out] */ __RPC__deref_out_opt BSTR *retval);
        
        /* [id][propget][funcdescattr] */ HRESULT ( STDMETHODCALLTYPE *get_DebugSymbols )( 
            JSharpProjectConfigurationProperties3 * This,
            /* [retval][out] */ __RPC__out BOOLEAN *retval);
        
        /* [id][propput][funcdescattr] */ HRESULT ( STDMETHODCALLTYPE *put_DebugSymbols )( 
            JSharpProjectConfigurationProperties3 * This,
            /* [in][idldescattr] */ BOOLEAN noname,
            /* [retval][out] */ __RPC__out void *retval);
        
        /* [id][propget][funcdescattr] */ HRESULT ( STDMETHODCALLTYPE *get_DefineDebug )( 
            JSharpProjectConfigurationProperties3 * This,
            /* [retval][out] */ __RPC__out BOOLEAN *retval);
        
        /* [id][propput][funcdescattr] */ HRESULT ( STDMETHODCALLTYPE *put_DefineDebug )( 
            JSharpProjectConfigurationProperties3 * This,
            /* [in][idldescattr] */ BOOLEAN noname,
            /* [retval][out] */ __RPC__out void *retval);
        
        /* [id][propget][funcdescattr] */ HRESULT ( STDMETHODCALLTYPE *get_DefineTrace )( 
            JSharpProjectConfigurationProperties3 * This,
            /* [retval][out] */ __RPC__out BOOLEAN *retval);
        
        /* [id][propput][funcdescattr] */ HRESULT ( STDMETHODCALLTYPE *put_DefineTrace )( 
            JSharpProjectConfigurationProperties3 * This,
            /* [in][idldescattr] */ BOOLEAN noname,
            /* [retval][out] */ __RPC__out void *retval);
        
        /* [id][propget][funcdescattr] */ HRESULT ( STDMETHODCALLTYPE *get_OutputPath )( 
            JSharpProjectConfigurationProperties3 * This,
            /* [retval][out] */ __RPC__deref_out_opt BSTR *retval);
        
        /* [id][propput][funcdescattr] */ HRESULT ( STDMETHODCALLTYPE *put_OutputPath )( 
            JSharpProjectConfigurationProperties3 * This,
            /* [in][idldescattr] */ __RPC__in BSTR noname,
            /* [retval][out] */ __RPC__out void *retval);
        
        /* [id][propget][funcdescattr] */ HRESULT ( STDMETHODCALLTYPE *get_IntermediatePath )( 
            JSharpProjectConfigurationProperties3 * This,
            /* [retval][out] */ __RPC__deref_out_opt BSTR *retval);
        
        /* [id][propput][hidden][funcdescattr] */ HRESULT ( STDMETHODCALLTYPE *put_IntermediatePath )( 
            JSharpProjectConfigurationProperties3 * This,
            /* [in][idldescattr] */ __RPC__in BSTR noname,
            /* [retval][out] */ __RPC__out void *retval);
        
        /* [id][propget][funcdescattr] */ HRESULT ( STDMETHODCALLTYPE *get_DefineConstants )( 
            JSharpProjectConfigurationProperties3 * This,
            /* [retval][out] */ __RPC__deref_out_opt BSTR *retval);
        
        /* [id][propput][funcdescattr] */ HRESULT ( STDMETHODCALLTYPE *put_DefineConstants )( 
            JSharpProjectConfigurationProperties3 * This,
            /* [in][idldescattr] */ __RPC__in BSTR noname,
            /* [retval][out] */ __RPC__out void *retval);
        
        /* [id][propget][funcdescattr] */ HRESULT ( STDMETHODCALLTYPE *get_RemoveIntegerChecks )( 
            JSharpProjectConfigurationProperties3 * This,
            /* [retval][out] */ __RPC__out BOOLEAN *retval);
        
        /* [id][propput][funcdescattr] */ HRESULT ( STDMETHODCALLTYPE *put_RemoveIntegerChecks )( 
            JSharpProjectConfigurationProperties3 * This,
            /* [in][idldescattr] */ BOOLEAN noname,
            /* [retval][out] */ __RPC__out void *retval);
        
        /* [id][propget][funcdescattr] */ HRESULT ( STDMETHODCALLTYPE *get_BaseAddress )( 
            JSharpProjectConfigurationProperties3 * This,
            /* [retval][out] */ __RPC__out unsigned long *retval);
        
        /* [id][propput][funcdescattr] */ HRESULT ( STDMETHODCALLTYPE *put_BaseAddress )( 
            JSharpProjectConfigurationProperties3 * This,
            /* [in][idldescattr] */ unsigned long noname,
            /* [retval][out] */ __RPC__out void *retval);
        
        /* [id][propget][funcdescattr] */ HRESULT ( STDMETHODCALLTYPE *get_AllowUnsafeBlocks )( 
            JSharpProjectConfigurationProperties3 * This,
            /* [retval][out] */ __RPC__out BOOLEAN *retval);
        
        /* [id][propput][funcdescattr] */ HRESULT ( STDMETHODCALLTYPE *put_AllowUnsafeBlocks )( 
            JSharpProjectConfigurationProperties3 * This,
            /* [in][idldescattr] */ BOOLEAN noname,
            /* [retval][out] */ __RPC__out void *retval);
        
        /* [id][propget][funcdescattr] */ HRESULT ( STDMETHODCALLTYPE *get_CheckForOverflowUnderflow )( 
            JSharpProjectConfigurationProperties3 * This,
            /* [retval][out] */ __RPC__out BOOLEAN *retval);
        
        /* [id][propput][funcdescattr] */ HRESULT ( STDMETHODCALLTYPE *put_CheckForOverflowUnderflow )( 
            JSharpProjectConfigurationProperties3 * This,
            /* [in][idldescattr] */ BOOLEAN noname,
            /* [retval][out] */ __RPC__out void *retval);
        
        /* [id][propget][funcdescattr] */ HRESULT ( STDMETHODCALLTYPE *get_DocumentationFile )( 
            JSharpProjectConfigurationProperties3 * This,
            /* [retval][out] */ __RPC__deref_out_opt BSTR *retval);
        
        /* [id][propput][funcdescattr] */ HRESULT ( STDMETHODCALLTYPE *put_DocumentationFile )( 
            JSharpProjectConfigurationProperties3 * This,
            /* [in][idldescattr] */ __RPC__in BSTR noname,
            /* [retval][out] */ __RPC__out void *retval);
        
        /* [id][propget][funcdescattr] */ HRESULT ( STDMETHODCALLTYPE *get_Optimize )( 
            JSharpProjectConfigurationProperties3 * This,
            /* [retval][out] */ __RPC__out BOOLEAN *retval);
        
        /* [id][propput][funcdescattr] */ HRESULT ( STDMETHODCALLTYPE *put_Optimize )( 
            JSharpProjectConfigurationProperties3 * This,
            /* [in][idldescattr] */ BOOLEAN noname,
            /* [retval][out] */ __RPC__out void *retval);
        
        /* [id][propget][funcdescattr] */ HRESULT ( STDMETHODCALLTYPE *get_IncrementalBuild )( 
            JSharpProjectConfigurationProperties3 * This,
            /* [retval][out] */ __RPC__out BOOLEAN *retval);
        
        /* [id][propput][funcdescattr] */ HRESULT ( STDMETHODCALLTYPE *put_IncrementalBuild )( 
            JSharpProjectConfigurationProperties3 * This,
            /* [in][idldescattr] */ BOOLEAN noname,
            /* [retval][out] */ __RPC__out void *retval);
        
        /* [id][propget][funcdescattr] */ HRESULT ( STDMETHODCALLTYPE *get_StartProgram )( 
            JSharpProjectConfigurationProperties3 * This,
            /* [retval][out] */ __RPC__deref_out_opt BSTR *retval);
        
        /* [id][propput][funcdescattr] */ HRESULT ( STDMETHODCALLTYPE *put_StartProgram )( 
            JSharpProjectConfigurationProperties3 * This,
            /* [in][idldescattr] */ __RPC__in BSTR noname,
            /* [retval][out] */ __RPC__out void *retval);
        
        /* [id][propget][funcdescattr] */ HRESULT ( STDMETHODCALLTYPE *get_StartWorkingDirectory )( 
            JSharpProjectConfigurationProperties3 * This,
            /* [retval][out] */ __RPC__deref_out_opt BSTR *retval);
        
        /* [id][propput][funcdescattr] */ HRESULT ( STDMETHODCALLTYPE *put_StartWorkingDirectory )( 
            JSharpProjectConfigurationProperties3 * This,
            /* [in][idldescattr] */ __RPC__in BSTR noname,
            /* [retval][out] */ __RPC__out void *retval);
        
        /* [id][propget][funcdescattr] */ HRESULT ( STDMETHODCALLTYPE *get_StartURL )( 
            JSharpProjectConfigurationProperties3 * This,
            /* [retval][out] */ __RPC__deref_out_opt BSTR *retval);
        
        /* [id][propput][funcdescattr] */ HRESULT ( STDMETHODCALLTYPE *put_StartURL )( 
            JSharpProjectConfigurationProperties3 * This,
            /* [in][idldescattr] */ __RPC__in BSTR noname,
            /* [retval][out] */ __RPC__out void *retval);
        
        /* [id][propget][funcdescattr] */ HRESULT ( STDMETHODCALLTYPE *get_StartPage )( 
            JSharpProjectConfigurationProperties3 * This,
            /* [retval][out] */ __RPC__deref_out_opt BSTR *retval);
        
        /* [id][propput][funcdescattr] */ HRESULT ( STDMETHODCALLTYPE *put_StartPage )( 
            JSharpProjectConfigurationProperties3 * This,
            /* [in][idldescattr] */ __RPC__in BSTR noname,
            /* [retval][out] */ __RPC__out void *retval);
        
        /* [id][propget][funcdescattr] */ HRESULT ( STDMETHODCALLTYPE *get_StartArguments )( 
            JSharpProjectConfigurationProperties3 * This,
            /* [retval][out] */ __RPC__deref_out_opt BSTR *retval);
        
        /* [id][propput][funcdescattr] */ HRESULT ( STDMETHODCALLTYPE *put_StartArguments )( 
            JSharpProjectConfigurationProperties3 * This,
            /* [in][idldescattr] */ __RPC__in BSTR noname,
            /* [retval][out] */ __RPC__out void *retval);
        
        /* [id][propget][funcdescattr] */ HRESULT ( STDMETHODCALLTYPE *get_StartWithIE )( 
            JSharpProjectConfigurationProperties3 * This,
            /* [retval][out] */ __RPC__out BOOLEAN *retval);
        
        /* [id][propput][funcdescattr] */ HRESULT ( STDMETHODCALLTYPE *put_StartWithIE )( 
            JSharpProjectConfigurationProperties3 * This,
            /* [in][idldescattr] */ BOOLEAN noname,
            /* [retval][out] */ __RPC__out void *retval);
        
        /* [id][propget][funcdescattr] */ HRESULT ( STDMETHODCALLTYPE *get_EnableASPDebugging )( 
            JSharpProjectConfigurationProperties3 * This,
            /* [retval][out] */ __RPC__out BOOLEAN *retval);
        
        /* [id][propput][funcdescattr] */ HRESULT ( STDMETHODCALLTYPE *put_EnableASPDebugging )( 
            JSharpProjectConfigurationProperties3 * This,
            /* [in][idldescattr] */ BOOLEAN noname,
            /* [retval][out] */ __RPC__out void *retval);
        
        /* [id][propget][funcdescattr] */ HRESULT ( STDMETHODCALLTYPE *get_EnableASPXDebugging )( 
            JSharpProjectConfigurationProperties3 * This,
            /* [retval][out] */ __RPC__out BOOLEAN *retval);
        
        /* [id][propput][funcdescattr] */ HRESULT ( STDMETHODCALLTYPE *put_EnableASPXDebugging )( 
            JSharpProjectConfigurationProperties3 * This,
            /* [in][idldescattr] */ BOOLEAN noname,
            /* [retval][out] */ __RPC__out void *retval);
        
        /* [id][propget][funcdescattr] */ HRESULT ( STDMETHODCALLTYPE *get_EnableUnmanagedDebugging )( 
            JSharpProjectConfigurationProperties3 * This,
            /* [retval][out] */ __RPC__out BOOLEAN *retval);
        
        /* [id][propput][funcdescattr] */ HRESULT ( STDMETHODCALLTYPE *put_EnableUnmanagedDebugging )( 
            JSharpProjectConfigurationProperties3 * This,
            /* [in][idldescattr] */ BOOLEAN noname,
            /* [retval][out] */ __RPC__out void *retval);
        
        /* [id][propget][funcdescattr] */ HRESULT ( STDMETHODCALLTYPE *get_StartAction )( 
            JSharpProjectConfigurationProperties3 * This,
            /* [retval][out] */ __RPC__out enum prjStartAction *retval);
        
        /* [id][propput][funcdescattr] */ HRESULT ( STDMETHODCALLTYPE *put_StartAction )( 
            JSharpProjectConfigurationProperties3 * This,
            /* [in][idldescattr] */ enum prjStartAction noname,
            /* [retval][out] */ __RPC__out void *retval);
        
        /* [id][propget][funcdescattr] */ HRESULT ( STDMETHODCALLTYPE *get_Extender )( 
            JSharpProjectConfigurationProperties3 * This,
            /* [in][idldescattr] */ __RPC__in BSTR ExtenderName,
            /* [retval][out] */ __RPC__deref_out_opt IDispatch **retval);
        
        /* [id][propget][funcdescattr] */ HRESULT ( STDMETHODCALLTYPE *get_ExtenderNames )( 
            JSharpProjectConfigurationProperties3 * This,
            /* [retval][out] */ __RPC__out VARIANT *retval);
        
        /* [id][propget][funcdescattr] */ HRESULT ( STDMETHODCALLTYPE *get_ExtenderCATID )( 
            JSharpProjectConfigurationProperties3 * This,
            /* [retval][out] */ __RPC__deref_out_opt BSTR *retval);
        
        /* [id][propget][funcdescattr] */ HRESULT ( STDMETHODCALLTYPE *get_WarningLevel )( 
            JSharpProjectConfigurationProperties3 * This,
            /* [retval][out] */ __RPC__out enum prjWarningLevel *retval);
        
        /* [id][propput][funcdescattr] */ HRESULT ( STDMETHODCALLTYPE *put_WarningLevel )( 
            JSharpProjectConfigurationProperties3 * This,
            /* [in][idldescattr] */ enum prjWarningLevel noname,
            /* [retval][out] */ __RPC__out void *retval);
        
        /* [id][propget][funcdescattr] */ HRESULT ( STDMETHODCALLTYPE *get_TreatWarningsAsErrors )( 
            JSharpProjectConfigurationProperties3 * This,
            /* [retval][out] */ __RPC__out BOOLEAN *retval);
        
        /* [id][propput][funcdescattr] */ HRESULT ( STDMETHODCALLTYPE *put_TreatWarningsAsErrors )( 
            JSharpProjectConfigurationProperties3 * This,
            /* [in][idldescattr] */ BOOLEAN noname,
            /* [retval][out] */ __RPC__out void *retval);
        
        /* [id][propget][funcdescattr] */ HRESULT ( STDMETHODCALLTYPE *get_EnableSQLServerDebugging )( 
            JSharpProjectConfigurationProperties3 * This,
            /* [retval][out] */ __RPC__out BOOLEAN *retval);
        
        /* [id][propput][funcdescattr] */ HRESULT ( STDMETHODCALLTYPE *put_EnableSQLServerDebugging )( 
            JSharpProjectConfigurationProperties3 * This,
            /* [in][idldescattr] */ BOOLEAN noname,
            /* [retval][out] */ __RPC__out void *retval);
        
        /* [id][propget][funcdescattr] */ HRESULT ( STDMETHODCALLTYPE *get_FileAlignment )( 
            JSharpProjectConfigurationProperties3 * This,
            /* [retval][out] */ __RPC__out unsigned long *retval);
        
        /* [id][propput][funcdescattr] */ HRESULT ( STDMETHODCALLTYPE *put_FileAlignment )( 
            JSharpProjectConfigurationProperties3 * This,
            /* [in][idldescattr] */ unsigned long noname,
            /* [retval][out] */ __RPC__out void *retval);
        
        /* [id][propget][funcdescattr] */ HRESULT ( STDMETHODCALLTYPE *get_RegisterForComInterop )( 
            JSharpProjectConfigurationProperties3 * This,
            /* [retval][out] */ __RPC__out BOOLEAN *retval);
        
        /* [id][propput][funcdescattr] */ HRESULT ( STDMETHODCALLTYPE *put_RegisterForComInterop )( 
            JSharpProjectConfigurationProperties3 * This,
            /* [in][idldescattr] */ BOOLEAN noname,
            /* [retval][out] */ __RPC__out void *retval);
        
        /* [id][propget][funcdescattr] */ HRESULT ( STDMETHODCALLTYPE *get_ConfigurationOverrideFile )( 
            JSharpProjectConfigurationProperties3 * This,
            /* [retval][out] */ __RPC__deref_out_opt BSTR *retval);
        
        /* [id][propput][funcdescattr] */ HRESULT ( STDMETHODCALLTYPE *put_ConfigurationOverrideFile )( 
            JSharpProjectConfigurationProperties3 * This,
            /* [in][idldescattr] */ __RPC__in BSTR noname,
            /* [retval][out] */ __RPC__out void *retval);
        
        /* [id][propget][funcdescattr] */ HRESULT ( STDMETHODCALLTYPE *get_RemoteDebugEnabled )( 
            JSharpProjectConfigurationProperties3 * This,
            /* [retval][out] */ __RPC__out BOOLEAN *retval);
        
        /* [id][propput][funcdescattr] */ HRESULT ( STDMETHODCALLTYPE *put_RemoteDebugEnabled )( 
            JSharpProjectConfigurationProperties3 * This,
            /* [in][idldescattr] */ BOOLEAN noname,
            /* [retval][out] */ __RPC__out void *retval);
        
        /* [id][propget][funcdescattr] */ HRESULT ( STDMETHODCALLTYPE *get_RemoteDebugMachine )( 
            JSharpProjectConfigurationProperties3 * This,
            /* [retval][out] */ __RPC__deref_out_opt BSTR *retval);
        
        /* [id][propput][funcdescattr] */ HRESULT ( STDMETHODCALLTYPE *put_RemoteDebugMachine )( 
            JSharpProjectConfigurationProperties3 * This,
            /* [in][idldescattr] */ __RPC__in BSTR noname,
            /* [retval][out] */ __RPC__out void *retval);
        
        /* [id][propget][funcdescattr] */ HRESULT ( STDMETHODCALLTYPE *get_NoWarn )( 
            JSharpProjectConfigurationProperties3 * This,
            /* [retval][out] */ __RPC__deref_out_opt BSTR *retval);
        
        /* [id][propput][funcdescattr] */ HRESULT ( STDMETHODCALLTYPE *put_NoWarn )( 
            JSharpProjectConfigurationProperties3 * This,
            /* [in][idldescattr] */ __RPC__in BSTR noname,
            /* [retval][out] */ __RPC__out void *retval);
        
        /* [id][propget][funcdescattr] */ HRESULT ( STDMETHODCALLTYPE *get_NoStdLib )( 
            JSharpProjectConfigurationProperties3 * This,
            /* [retval][out] */ __RPC__out BOOLEAN *retval);
        
        /* [id][propput][funcdescattr] */ HRESULT ( STDMETHODCALLTYPE *put_NoStdLib )( 
            JSharpProjectConfigurationProperties3 * This,
            /* [in][idldescattr] */ BOOLEAN noname,
            /* [retval][out] */ __RPC__out void *retval);
        
        /* [helpstring][id][propget] */ HRESULT ( STDMETHODCALLTYPE *get_DebugInfo )( 
            JSharpProjectConfigurationProperties3 * This,
            /* [retval][out] */ __RPC__deref_out_opt BSTR *pbstrDebugInfo);
        
        /* [helpstring][id][propput] */ HRESULT ( STDMETHODCALLTYPE *put_DebugInfo )( 
            JSharpProjectConfigurationProperties3 * This,
            /* [in] */ __RPC__in BSTR DebugInfo);
        
        /* [helpstring][id][propget] */ HRESULT ( STDMETHODCALLTYPE *get_PlatformTarget )( 
            JSharpProjectConfigurationProperties3 * This,
            /* [retval][out] */ __RPC__deref_out_opt BSTR *pbstrPlatformTarget);
        
        /* [helpstring][id][propput] */ HRESULT ( STDMETHODCALLTYPE *put_PlatformTarget )( 
            JSharpProjectConfigurationProperties3 * This,
            /* [in] */ __RPC__in BSTR PlatformTarget);
        
        /* [helpstring][id][propget] */ HRESULT ( STDMETHODCALLTYPE *get_TreatSpecificWarningsAsErrors )( 
            JSharpProjectConfigurationProperties3 * This,
            /* [retval][out] */ __RPC__deref_out_opt BSTR *pbstrWarningsAsErrors);
        
        /* [helpstring][id][propput] */ HRESULT ( STDMETHODCALLTYPE *put_TreatSpecificWarningsAsErrors )( 
            JSharpProjectConfigurationProperties3 * This,
            /* [in] */ __RPC__in BSTR WarningsAsErrors);
        
        /* [helpstring][id][propget] */ HRESULT ( STDMETHODCALLTYPE *get_RunCodeAnalysis )( 
            JSharpProjectConfigurationProperties3 * This,
            /* [retval][out] */ __RPC__out VARIANT_BOOL *pbRunCodeAnalysis);
        
        /* [helpstring][id][propput] */ HRESULT ( STDMETHODCALLTYPE *put_RunCodeAnalysis )( 
            JSharpProjectConfigurationProperties3 * This,
            /* [in] */ VARIANT_BOOL RunCodeAnalysis);
        
        /* [helpstring][id][propget] */ HRESULT ( STDMETHODCALLTYPE *get_CodeAnalysisLogFile )( 
            JSharpProjectConfigurationProperties3 * This,
            /* [retval][out] */ __RPC__deref_out_opt BSTR *pbstrCodeAnalysisLogFile);
        
        /* [helpstring][id][propput] */ HRESULT ( STDMETHODCALLTYPE *put_CodeAnalysisLogFile )( 
            JSharpProjectConfigurationProperties3 * This,
            /* [in] */ __RPC__in BSTR CodeAnalysisLogFile);
        
        /* [helpstring][id][propget] */ HRESULT ( STDMETHODCALLTYPE *get_CodeAnalysisRuleAssemblies )( 
            JSharpProjectConfigurationProperties3 * This,
            /* [retval][out] */ __RPC__deref_out_opt BSTR *pbstrCodeAnalysisRuleAssemblies);
        
        /* [helpstring][id][propput] */ HRESULT ( STDMETHODCALLTYPE *put_CodeAnalysisRuleAssemblies )( 
            JSharpProjectConfigurationProperties3 * This,
            /* [in] */ __RPC__in BSTR CodeAnalysisRuleAssemblies);
        
        /* [helpstring][id][propget] */ HRESULT ( STDMETHODCALLTYPE *get_CodeAnalysisInputAssembly )( 
            JSharpProjectConfigurationProperties3 * This,
            /* [retval][out] */ __RPC__deref_out_opt BSTR *pbstrCodeAnalysisInputAssembly);
        
        /* [helpstring][id][propput] */ HRESULT ( STDMETHODCALLTYPE *put_CodeAnalysisInputAssembly )( 
            JSharpProjectConfigurationProperties3 * This,
            /* [in] */ __RPC__in BSTR CodeAnalysisInputAssembly);
        
        /* [helpstring][id][propget] */ HRESULT ( STDMETHODCALLTYPE *get_CodeAnalysisRules )( 
            JSharpProjectConfigurationProperties3 * This,
            /* [retval][out] */ __RPC__deref_out_opt BSTR *pbstrCodeAnalysisRules);
        
        /* [helpstring][id][propput] */ HRESULT ( STDMETHODCALLTYPE *put_CodeAnalysisRules )( 
            JSharpProjectConfigurationProperties3 * This,
            /* [in] */ __RPC__in BSTR CodeAnalysisRules);
        
        /* [helpstring][id][propget] */ HRESULT ( STDMETHODCALLTYPE *get_CodeAnalysisSpellCheckLanguages )( 
            JSharpProjectConfigurationProperties3 * This,
            /* [retval][out] */ __RPC__deref_out_opt BSTR *pbstrCodeAnalysisSpellCheckLanguages);
        
        /* [helpstring][id][propput] */ HRESULT ( STDMETHODCALLTYPE *put_CodeAnalysisSpellCheckLanguages )( 
            JSharpProjectConfigurationProperties3 * This,
            /* [in] */ __RPC__in BSTR CodeAnalysisSpellCheckLanguages);
        
        /* [helpstring][id][propget] */ HRESULT ( STDMETHODCALLTYPE *get_CodeAnalysisUseTypeNameInSuppression )( 
            JSharpProjectConfigurationProperties3 * This,
            /* [retval][out] */ __RPC__out VARIANT_BOOL *bUseTypeNameInSuppression);
        
        /* [helpstring][id][propput] */ HRESULT ( STDMETHODCALLTYPE *put_CodeAnalysisUseTypeNameInSuppression )( 
            JSharpProjectConfigurationProperties3 * This,
            /* [in] */ VARIANT_BOOL UseTypeNameInSuppression);
        
        /* [helpstring][id][propget] */ HRESULT ( STDMETHODCALLTYPE *get_CodeAnalysisModuleSuppressionsFile )( 
            JSharpProjectConfigurationProperties3 * This,
            /* [retval][out] */ __RPC__deref_out_opt BSTR *pbstrCodeAnalysisModuleSuppressionsFile);
        
        /* [helpstring][id][propput] */ HRESULT ( STDMETHODCALLTYPE *put_CodeAnalysisModuleSuppressionsFile )( 
            JSharpProjectConfigurationProperties3 * This,
            /* [in] */ __RPC__in BSTR CodeAnalysisModuleSuppressionsFile);
        
        /* [helpstring][id][propget] */ HRESULT ( STDMETHODCALLTYPE *get_UseVSHostingProcess )( 
            JSharpProjectConfigurationProperties3 * This,
            /* [retval][out] */ __RPC__out VARIANT_BOOL *pbUseVSHostingProcess);
        
        /* [helpstring][id][propput] */ HRESULT ( STDMETHODCALLTYPE *put_UseVSHostingProcess )( 
            JSharpProjectConfigurationProperties3 * This,
            /* [in] */ VARIANT_BOOL UseVSHostingProcess);
        
        /* [helpstring][id][propget] */ HRESULT ( STDMETHODCALLTYPE *get_GenerateSerializationAssemblies )( 
            JSharpProjectConfigurationProperties3 * This,
            /* [retval][out] */ __RPC__out sgenGenerationOption *pSgenGenerationOption);
        
        /* [helpstring][id][propput] */ HRESULT ( STDMETHODCALLTYPE *put_GenerateSerializationAssemblies )( 
            JSharpProjectConfigurationProperties3 * This,
            /* [in] */ sgenGenerationOption SgenGenerationOption);
        
        /* [helpstring][id][propget] */ HRESULT ( STDMETHODCALLTYPE *get_CodePage )( 
            JSharpProjectConfigurationProperties3 * This,
            /* [retval][out] */ __RPC__deref_out_opt BSTR *pbstrCodePage);
        
        /* [helpstring][id][propput] */ HRESULT ( STDMETHODCALLTYPE *put_CodePage )( 
            JSharpProjectConfigurationProperties3 * This,
            /* [in] */ __RPC__in BSTR CodePage);
        
        /* [helpstring][id][propget] */ HRESULT ( STDMETHODCALLTYPE *get_JCPA )( 
            JSharpProjectConfigurationProperties3 * This,
            /* [retval][out] */ __RPC__deref_out_opt BSTR *pbstrJCPA);
        
        /* [helpstring][id][propput] */ HRESULT ( STDMETHODCALLTYPE *put_JCPA )( 
            JSharpProjectConfigurationProperties3 * This,
            /* [in] */ __RPC__in BSTR JCPA);
        
        /* [helpstring][id][propget] */ HRESULT ( STDMETHODCALLTYPE *get_DisableLangXtns )( 
            JSharpProjectConfigurationProperties3 * This,
            /* [retval][out] */ __RPC__out prjDisableLangXtns *pDisableLangXtns);
        
        /* [helpstring][id][propput] */ HRESULT ( STDMETHODCALLTYPE *put_DisableLangXtns )( 
            JSharpProjectConfigurationProperties3 * This,
            /* [in] */ prjDisableLangXtns DisableLangXtns);
        
        /* [helpstring][id][propget] */ HRESULT ( STDMETHODCALLTYPE *get_SecureScoping )( 
            JSharpProjectConfigurationProperties3 * This,
            /* [retval][out] */ __RPC__out VARIANT_BOOL *pbSecureScoping);
        
        /* [helpstring][id][propput] */ HRESULT ( STDMETHODCALLTYPE *put_SecureScoping )( 
            JSharpProjectConfigurationProperties3 * This,
            /* [in] */ VARIANT_BOOL bSecureScoping);
        
        END_INTERFACE
    } JSharpProjectConfigurationProperties3Vtbl;

    interface JSharpProjectConfigurationProperties3
    {
        CONST_VTBL struct JSharpProjectConfigurationProperties3Vtbl *lpVtbl;
    };

    

#ifdef COBJMACROS


#define JSharpProjectConfigurationProperties3_QueryInterface(This,riid,ppvObj,retval)	\
    ( (This)->lpVtbl -> QueryInterface(This,riid,ppvObj,retval) ) 

#define JSharpProjectConfigurationProperties3_AddRef(This,retval)	\
    ( (This)->lpVtbl -> AddRef(This,retval) ) 

#define JSharpProjectConfigurationProperties3_Release(This,retval)	\
    ( (This)->lpVtbl -> Release(This,retval) ) 

#define JSharpProjectConfigurationProperties3_GetTypeInfoCount(This,pctinfo,retval)	\
    ( (This)->lpVtbl -> GetTypeInfoCount(This,pctinfo,retval) ) 

#define JSharpProjectConfigurationProperties3_GetTypeInfo(This,itinfo,lcid,pptinfo,retval)	\
    ( (This)->lpVtbl -> GetTypeInfo(This,itinfo,lcid,pptinfo,retval) ) 

#define JSharpProjectConfigurationProperties3_GetIDsOfNames(This,riid,rgszNames,cNames,lcid,rgdispid,retval)	\
    ( (This)->lpVtbl -> GetIDsOfNames(This,riid,rgszNames,cNames,lcid,rgdispid,retval) ) 

#define JSharpProjectConfigurationProperties3_Invoke(This,dispidMember,riid,lcid,wFlags,pdispparams,pvarResult,pexcepinfo,puArgErr,retval)	\
    ( (This)->lpVtbl -> Invoke(This,dispidMember,riid,lcid,wFlags,pdispparams,pvarResult,pexcepinfo,puArgErr,retval) ) 

#define JSharpProjectConfigurationProperties3_get___id(This,retval)	\
    ( (This)->lpVtbl -> get___id(This,retval) ) 

#define JSharpProjectConfigurationProperties3_get_DebugSymbols(This,retval)	\
    ( (This)->lpVtbl -> get_DebugSymbols(This,retval) ) 

#define JSharpProjectConfigurationProperties3_put_DebugSymbols(This,noname,retval)	\
    ( (This)->lpVtbl -> put_DebugSymbols(This,noname,retval) ) 

#define JSharpProjectConfigurationProperties3_get_DefineDebug(This,retval)	\
    ( (This)->lpVtbl -> get_DefineDebug(This,retval) ) 

#define JSharpProjectConfigurationProperties3_put_DefineDebug(This,noname,retval)	\
    ( (This)->lpVtbl -> put_DefineDebug(This,noname,retval) ) 

#define JSharpProjectConfigurationProperties3_get_DefineTrace(This,retval)	\
    ( (This)->lpVtbl -> get_DefineTrace(This,retval) ) 

#define JSharpProjectConfigurationProperties3_put_DefineTrace(This,noname,retval)	\
    ( (This)->lpVtbl -> put_DefineTrace(This,noname,retval) ) 

#define JSharpProjectConfigurationProperties3_get_OutputPath(This,retval)	\
    ( (This)->lpVtbl -> get_OutputPath(This,retval) ) 

#define JSharpProjectConfigurationProperties3_put_OutputPath(This,noname,retval)	\
    ( (This)->lpVtbl -> put_OutputPath(This,noname,retval) ) 

#define JSharpProjectConfigurationProperties3_get_IntermediatePath(This,retval)	\
    ( (This)->lpVtbl -> get_IntermediatePath(This,retval) ) 

#define JSharpProjectConfigurationProperties3_put_IntermediatePath(This,noname,retval)	\
    ( (This)->lpVtbl -> put_IntermediatePath(This,noname,retval) ) 

#define JSharpProjectConfigurationProperties3_get_DefineConstants(This,retval)	\
    ( (This)->lpVtbl -> get_DefineConstants(This,retval) ) 

#define JSharpProjectConfigurationProperties3_put_DefineConstants(This,noname,retval)	\
    ( (This)->lpVtbl -> put_DefineConstants(This,noname,retval) ) 

#define JSharpProjectConfigurationProperties3_get_RemoveIntegerChecks(This,retval)	\
    ( (This)->lpVtbl -> get_RemoveIntegerChecks(This,retval) ) 

#define JSharpProjectConfigurationProperties3_put_RemoveIntegerChecks(This,noname,retval)	\
    ( (This)->lpVtbl -> put_RemoveIntegerChecks(This,noname,retval) ) 

#define JSharpProjectConfigurationProperties3_get_BaseAddress(This,retval)	\
    ( (This)->lpVtbl -> get_BaseAddress(This,retval) ) 

#define JSharpProjectConfigurationProperties3_put_BaseAddress(This,noname,retval)	\
    ( (This)->lpVtbl -> put_BaseAddress(This,noname,retval) ) 

#define JSharpProjectConfigurationProperties3_get_AllowUnsafeBlocks(This,retval)	\
    ( (This)->lpVtbl -> get_AllowUnsafeBlocks(This,retval) ) 

#define JSharpProjectConfigurationProperties3_put_AllowUnsafeBlocks(This,noname,retval)	\
    ( (This)->lpVtbl -> put_AllowUnsafeBlocks(This,noname,retval) ) 

#define JSharpProjectConfigurationProperties3_get_CheckForOverflowUnderflow(This,retval)	\
    ( (This)->lpVtbl -> get_CheckForOverflowUnderflow(This,retval) ) 

#define JSharpProjectConfigurationProperties3_put_CheckForOverflowUnderflow(This,noname,retval)	\
    ( (This)->lpVtbl -> put_CheckForOverflowUnderflow(This,noname,retval) ) 

#define JSharpProjectConfigurationProperties3_get_DocumentationFile(This,retval)	\
    ( (This)->lpVtbl -> get_DocumentationFile(This,retval) ) 

#define JSharpProjectConfigurationProperties3_put_DocumentationFile(This,noname,retval)	\
    ( (This)->lpVtbl -> put_DocumentationFile(This,noname,retval) ) 

#define JSharpProjectConfigurationProperties3_get_Optimize(This,retval)	\
    ( (This)->lpVtbl -> get_Optimize(This,retval) ) 

#define JSharpProjectConfigurationProperties3_put_Optimize(This,noname,retval)	\
    ( (This)->lpVtbl -> put_Optimize(This,noname,retval) ) 

#define JSharpProjectConfigurationProperties3_get_IncrementalBuild(This,retval)	\
    ( (This)->lpVtbl -> get_IncrementalBuild(This,retval) ) 

#define JSharpProjectConfigurationProperties3_put_IncrementalBuild(This,noname,retval)	\
    ( (This)->lpVtbl -> put_IncrementalBuild(This,noname,retval) ) 

#define JSharpProjectConfigurationProperties3_get_StartProgram(This,retval)	\
    ( (This)->lpVtbl -> get_StartProgram(This,retval) ) 

#define JSharpProjectConfigurationProperties3_put_StartProgram(This,noname,retval)	\
    ( (This)->lpVtbl -> put_StartProgram(This,noname,retval) ) 

#define JSharpProjectConfigurationProperties3_get_StartWorkingDirectory(This,retval)	\
    ( (This)->lpVtbl -> get_StartWorkingDirectory(This,retval) ) 

#define JSharpProjectConfigurationProperties3_put_StartWorkingDirectory(This,noname,retval)	\
    ( (This)->lpVtbl -> put_StartWorkingDirectory(This,noname,retval) ) 

#define JSharpProjectConfigurationProperties3_get_StartURL(This,retval)	\
    ( (This)->lpVtbl -> get_StartURL(This,retval) ) 

#define JSharpProjectConfigurationProperties3_put_StartURL(This,noname,retval)	\
    ( (This)->lpVtbl -> put_StartURL(This,noname,retval) ) 

#define JSharpProjectConfigurationProperties3_get_StartPage(This,retval)	\
    ( (This)->lpVtbl -> get_StartPage(This,retval) ) 

#define JSharpProjectConfigurationProperties3_put_StartPage(This,noname,retval)	\
    ( (This)->lpVtbl -> put_StartPage(This,noname,retval) ) 

#define JSharpProjectConfigurationProperties3_get_StartArguments(This,retval)	\
    ( (This)->lpVtbl -> get_StartArguments(This,retval) ) 

#define JSharpProjectConfigurationProperties3_put_StartArguments(This,noname,retval)	\
    ( (This)->lpVtbl -> put_StartArguments(This,noname,retval) ) 

#define JSharpProjectConfigurationProperties3_get_StartWithIE(This,retval)	\
    ( (This)->lpVtbl -> get_StartWithIE(This,retval) ) 

#define JSharpProjectConfigurationProperties3_put_StartWithIE(This,noname,retval)	\
    ( (This)->lpVtbl -> put_StartWithIE(This,noname,retval) ) 

#define JSharpProjectConfigurationProperties3_get_EnableASPDebugging(This,retval)	\
    ( (This)->lpVtbl -> get_EnableASPDebugging(This,retval) ) 

#define JSharpProjectConfigurationProperties3_put_EnableASPDebugging(This,noname,retval)	\
    ( (This)->lpVtbl -> put_EnableASPDebugging(This,noname,retval) ) 

#define JSharpProjectConfigurationProperties3_get_EnableASPXDebugging(This,retval)	\
    ( (This)->lpVtbl -> get_EnableASPXDebugging(This,retval) ) 

#define JSharpProjectConfigurationProperties3_put_EnableASPXDebugging(This,noname,retval)	\
    ( (This)->lpVtbl -> put_EnableASPXDebugging(This,noname,retval) ) 

#define JSharpProjectConfigurationProperties3_get_EnableUnmanagedDebugging(This,retval)	\
    ( (This)->lpVtbl -> get_EnableUnmanagedDebugging(This,retval) ) 

#define JSharpProjectConfigurationProperties3_put_EnableUnmanagedDebugging(This,noname,retval)	\
    ( (This)->lpVtbl -> put_EnableUnmanagedDebugging(This,noname,retval) ) 

#define JSharpProjectConfigurationProperties3_get_StartAction(This,retval)	\
    ( (This)->lpVtbl -> get_StartAction(This,retval) ) 

#define JSharpProjectConfigurationProperties3_put_StartAction(This,noname,retval)	\
    ( (This)->lpVtbl -> put_StartAction(This,noname,retval) ) 

#define JSharpProjectConfigurationProperties3_get_Extender(This,ExtenderName,retval)	\
    ( (This)->lpVtbl -> get_Extender(This,ExtenderName,retval) ) 

#define JSharpProjectConfigurationProperties3_get_ExtenderNames(This,retval)	\
    ( (This)->lpVtbl -> get_ExtenderNames(This,retval) ) 

#define JSharpProjectConfigurationProperties3_get_ExtenderCATID(This,retval)	\
    ( (This)->lpVtbl -> get_ExtenderCATID(This,retval) ) 

#define JSharpProjectConfigurationProperties3_get_WarningLevel(This,retval)	\
    ( (This)->lpVtbl -> get_WarningLevel(This,retval) ) 

#define JSharpProjectConfigurationProperties3_put_WarningLevel(This,noname,retval)	\
    ( (This)->lpVtbl -> put_WarningLevel(This,noname,retval) ) 

#define JSharpProjectConfigurationProperties3_get_TreatWarningsAsErrors(This,retval)	\
    ( (This)->lpVtbl -> get_TreatWarningsAsErrors(This,retval) ) 

#define JSharpProjectConfigurationProperties3_put_TreatWarningsAsErrors(This,noname,retval)	\
    ( (This)->lpVtbl -> put_TreatWarningsAsErrors(This,noname,retval) ) 

#define JSharpProjectConfigurationProperties3_get_EnableSQLServerDebugging(This,retval)	\
    ( (This)->lpVtbl -> get_EnableSQLServerDebugging(This,retval) ) 

#define JSharpProjectConfigurationProperties3_put_EnableSQLServerDebugging(This,noname,retval)	\
    ( (This)->lpVtbl -> put_EnableSQLServerDebugging(This,noname,retval) ) 

#define JSharpProjectConfigurationProperties3_get_FileAlignment(This,retval)	\
    ( (This)->lpVtbl -> get_FileAlignment(This,retval) ) 

#define JSharpProjectConfigurationProperties3_put_FileAlignment(This,noname,retval)	\
    ( (This)->lpVtbl -> put_FileAlignment(This,noname,retval) ) 

#define JSharpProjectConfigurationProperties3_get_RegisterForComInterop(This,retval)	\
    ( (This)->lpVtbl -> get_RegisterForComInterop(This,retval) ) 

#define JSharpProjectConfigurationProperties3_put_RegisterForComInterop(This,noname,retval)	\
    ( (This)->lpVtbl -> put_RegisterForComInterop(This,noname,retval) ) 

#define JSharpProjectConfigurationProperties3_get_ConfigurationOverrideFile(This,retval)	\
    ( (This)->lpVtbl -> get_ConfigurationOverrideFile(This,retval) ) 

#define JSharpProjectConfigurationProperties3_put_ConfigurationOverrideFile(This,noname,retval)	\
    ( (This)->lpVtbl -> put_ConfigurationOverrideFile(This,noname,retval) ) 

#define JSharpProjectConfigurationProperties3_get_RemoteDebugEnabled(This,retval)	\
    ( (This)->lpVtbl -> get_RemoteDebugEnabled(This,retval) ) 

#define JSharpProjectConfigurationProperties3_put_RemoteDebugEnabled(This,noname,retval)	\
    ( (This)->lpVtbl -> put_RemoteDebugEnabled(This,noname,retval) ) 

#define JSharpProjectConfigurationProperties3_get_RemoteDebugMachine(This,retval)	\
    ( (This)->lpVtbl -> get_RemoteDebugMachine(This,retval) ) 

#define JSharpProjectConfigurationProperties3_put_RemoteDebugMachine(This,noname,retval)	\
    ( (This)->lpVtbl -> put_RemoteDebugMachine(This,noname,retval) ) 

#define JSharpProjectConfigurationProperties3_get_NoWarn(This,retval)	\
    ( (This)->lpVtbl -> get_NoWarn(This,retval) ) 

#define JSharpProjectConfigurationProperties3_put_NoWarn(This,noname,retval)	\
    ( (This)->lpVtbl -> put_NoWarn(This,noname,retval) ) 

#define JSharpProjectConfigurationProperties3_get_NoStdLib(This,retval)	\
    ( (This)->lpVtbl -> get_NoStdLib(This,retval) ) 

#define JSharpProjectConfigurationProperties3_put_NoStdLib(This,noname,retval)	\
    ( (This)->lpVtbl -> put_NoStdLib(This,noname,retval) ) 


#define JSharpProjectConfigurationProperties3_get_DebugInfo(This,pbstrDebugInfo)	\
    ( (This)->lpVtbl -> get_DebugInfo(This,pbstrDebugInfo) ) 

#define JSharpProjectConfigurationProperties3_put_DebugInfo(This,DebugInfo)	\
    ( (This)->lpVtbl -> put_DebugInfo(This,DebugInfo) ) 

#define JSharpProjectConfigurationProperties3_get_PlatformTarget(This,pbstrPlatformTarget)	\
    ( (This)->lpVtbl -> get_PlatformTarget(This,pbstrPlatformTarget) ) 

#define JSharpProjectConfigurationProperties3_put_PlatformTarget(This,PlatformTarget)	\
    ( (This)->lpVtbl -> put_PlatformTarget(This,PlatformTarget) ) 

#define JSharpProjectConfigurationProperties3_get_TreatSpecificWarningsAsErrors(This,pbstrWarningsAsErrors)	\
    ( (This)->lpVtbl -> get_TreatSpecificWarningsAsErrors(This,pbstrWarningsAsErrors) ) 

#define JSharpProjectConfigurationProperties3_put_TreatSpecificWarningsAsErrors(This,WarningsAsErrors)	\
    ( (This)->lpVtbl -> put_TreatSpecificWarningsAsErrors(This,WarningsAsErrors) ) 

#define JSharpProjectConfigurationProperties3_get_RunCodeAnalysis(This,pbRunCodeAnalysis)	\
    ( (This)->lpVtbl -> get_RunCodeAnalysis(This,pbRunCodeAnalysis) ) 

#define JSharpProjectConfigurationProperties3_put_RunCodeAnalysis(This,RunCodeAnalysis)	\
    ( (This)->lpVtbl -> put_RunCodeAnalysis(This,RunCodeAnalysis) ) 

#define JSharpProjectConfigurationProperties3_get_CodeAnalysisLogFile(This,pbstrCodeAnalysisLogFile)	\
    ( (This)->lpVtbl -> get_CodeAnalysisLogFile(This,pbstrCodeAnalysisLogFile) ) 

#define JSharpProjectConfigurationProperties3_put_CodeAnalysisLogFile(This,CodeAnalysisLogFile)	\
    ( (This)->lpVtbl -> put_CodeAnalysisLogFile(This,CodeAnalysisLogFile) ) 

#define JSharpProjectConfigurationProperties3_get_CodeAnalysisRuleAssemblies(This,pbstrCodeAnalysisRuleAssemblies)	\
    ( (This)->lpVtbl -> get_CodeAnalysisRuleAssemblies(This,pbstrCodeAnalysisRuleAssemblies) ) 

#define JSharpProjectConfigurationProperties3_put_CodeAnalysisRuleAssemblies(This,CodeAnalysisRuleAssemblies)	\
    ( (This)->lpVtbl -> put_CodeAnalysisRuleAssemblies(This,CodeAnalysisRuleAssemblies) ) 

#define JSharpProjectConfigurationProperties3_get_CodeAnalysisInputAssembly(This,pbstrCodeAnalysisInputAssembly)	\
    ( (This)->lpVtbl -> get_CodeAnalysisInputAssembly(This,pbstrCodeAnalysisInputAssembly) ) 

#define JSharpProjectConfigurationProperties3_put_CodeAnalysisInputAssembly(This,CodeAnalysisInputAssembly)	\
    ( (This)->lpVtbl -> put_CodeAnalysisInputAssembly(This,CodeAnalysisInputAssembly) ) 

#define JSharpProjectConfigurationProperties3_get_CodeAnalysisRules(This,pbstrCodeAnalysisRules)	\
    ( (This)->lpVtbl -> get_CodeAnalysisRules(This,pbstrCodeAnalysisRules) ) 

#define JSharpProjectConfigurationProperties3_put_CodeAnalysisRules(This,CodeAnalysisRules)	\
    ( (This)->lpVtbl -> put_CodeAnalysisRules(This,CodeAnalysisRules) ) 

#define JSharpProjectConfigurationProperties3_get_CodeAnalysisSpellCheckLanguages(This,pbstrCodeAnalysisSpellCheckLanguages)	\
    ( (This)->lpVtbl -> get_CodeAnalysisSpellCheckLanguages(This,pbstrCodeAnalysisSpellCheckLanguages) ) 

#define JSharpProjectConfigurationProperties3_put_CodeAnalysisSpellCheckLanguages(This,CodeAnalysisSpellCheckLanguages)	\
    ( (This)->lpVtbl -> put_CodeAnalysisSpellCheckLanguages(This,CodeAnalysisSpellCheckLanguages) ) 

#define JSharpProjectConfigurationProperties3_get_CodeAnalysisUseTypeNameInSuppression(This,bUseTypeNameInSuppression)	\
    ( (This)->lpVtbl -> get_CodeAnalysisUseTypeNameInSuppression(This,bUseTypeNameInSuppression) ) 

#define JSharpProjectConfigurationProperties3_put_CodeAnalysisUseTypeNameInSuppression(This,UseTypeNameInSuppression)	\
    ( (This)->lpVtbl -> put_CodeAnalysisUseTypeNameInSuppression(This,UseTypeNameInSuppression) ) 

#define JSharpProjectConfigurationProperties3_get_CodeAnalysisModuleSuppressionsFile(This,pbstrCodeAnalysisModuleSuppressionsFile)	\
    ( (This)->lpVtbl -> get_CodeAnalysisModuleSuppressionsFile(This,pbstrCodeAnalysisModuleSuppressionsFile) ) 

#define JSharpProjectConfigurationProperties3_put_CodeAnalysisModuleSuppressionsFile(This,CodeAnalysisModuleSuppressionsFile)	\
    ( (This)->lpVtbl -> put_CodeAnalysisModuleSuppressionsFile(This,CodeAnalysisModuleSuppressionsFile) ) 

#define JSharpProjectConfigurationProperties3_get_UseVSHostingProcess(This,pbUseVSHostingProcess)	\
    ( (This)->lpVtbl -> get_UseVSHostingProcess(This,pbUseVSHostingProcess) ) 

#define JSharpProjectConfigurationProperties3_put_UseVSHostingProcess(This,UseVSHostingProcess)	\
    ( (This)->lpVtbl -> put_UseVSHostingProcess(This,UseVSHostingProcess) ) 

#define JSharpProjectConfigurationProperties3_get_GenerateSerializationAssemblies(This,pSgenGenerationOption)	\
    ( (This)->lpVtbl -> get_GenerateSerializationAssemblies(This,pSgenGenerationOption) ) 

#define JSharpProjectConfigurationProperties3_put_GenerateSerializationAssemblies(This,SgenGenerationOption)	\
    ( (This)->lpVtbl -> put_GenerateSerializationAssemblies(This,SgenGenerationOption) ) 


#define JSharpProjectConfigurationProperties3_get_CodePage(This,pbstrCodePage)	\
    ( (This)->lpVtbl -> get_CodePage(This,pbstrCodePage) ) 

#define JSharpProjectConfigurationProperties3_put_CodePage(This,CodePage)	\
    ( (This)->lpVtbl -> put_CodePage(This,CodePage) ) 

#define JSharpProjectConfigurationProperties3_get_JCPA(This,pbstrJCPA)	\
    ( (This)->lpVtbl -> get_JCPA(This,pbstrJCPA) ) 

#define JSharpProjectConfigurationProperties3_put_JCPA(This,JCPA)	\
    ( (This)->lpVtbl -> put_JCPA(This,JCPA) ) 

#define JSharpProjectConfigurationProperties3_get_DisableLangXtns(This,pDisableLangXtns)	\
    ( (This)->lpVtbl -> get_DisableLangXtns(This,pDisableLangXtns) ) 

#define JSharpProjectConfigurationProperties3_put_DisableLangXtns(This,DisableLangXtns)	\
    ( (This)->lpVtbl -> put_DisableLangXtns(This,DisableLangXtns) ) 

#define JSharpProjectConfigurationProperties3_get_SecureScoping(This,pbSecureScoping)	\
    ( (This)->lpVtbl -> get_SecureScoping(This,pbSecureScoping) ) 

#define JSharpProjectConfigurationProperties3_put_SecureScoping(This,bSecureScoping)	\
    ( (This)->lpVtbl -> put_SecureScoping(This,bSecureScoping) ) 

#endif /* COBJMACROS */


#endif 	/* C style interface */




#endif 	/* __JSharpProjectConfigurationProperties3_INTERFACE_DEFINED__ */


#ifndef __FolderProperties2_INTERFACE_DEFINED__
#define __FolderProperties2_INTERFACE_DEFINED__

/* interface FolderProperties2 */
/* [local][unique][helpstring][dual][uuid][object] */ 


EXTERN_C const IID IID_FolderProperties2;

#if defined(__cplusplus) && !defined(CINTERFACE)
    
    MIDL_INTERFACE("2ACA2576-0738-466f-845F-16062ED8D1BC")
    FolderProperties2 : public FolderProperties
    {
    public:
        virtual /* [helpstring][id][propget] */ HRESULT STDMETHODCALLTYPE get_WebReferenceInterface( 
            /* [retval][out] */ IUnknown **ppWebReference) = 0;
        
    };
    
#else 	/* C style interface */

    typedef struct FolderProperties2Vtbl
    {
        BEGIN_INTERFACE
        
        /* [id][restricted][funcdescattr] */ HRESULT ( STDMETHODCALLTYPE *QueryInterface )( 
            FolderProperties2 * This,
            /* [in][idldescattr] */ struct GUID *riid,
            /* [out][idldescattr] */ void **ppvObj,
            /* [retval][out] */ void *retval);
        
        /* [id][restricted][funcdescattr] */ HRESULT ( STDMETHODCALLTYPE *AddRef )( 
            FolderProperties2 * This,
            /* [retval][out] */ unsigned long *retval);
        
        /* [id][restricted][funcdescattr] */ HRESULT ( STDMETHODCALLTYPE *Release )( 
            FolderProperties2 * This,
            /* [retval][out] */ unsigned long *retval);
        
        /* [id][restricted][funcdescattr] */ HRESULT ( STDMETHODCALLTYPE *GetTypeInfoCount )( 
            FolderProperties2 * This,
            /* [out][idldescattr] */ unsigned UINT *pctinfo,
            /* [retval][out] */ void *retval);
        
        /* [id][restricted][funcdescattr] */ HRESULT ( STDMETHODCALLTYPE *GetTypeInfo )( 
            FolderProperties2 * This,
            /* [in][idldescattr] */ unsigned UINT itinfo,
            /* [in][idldescattr] */ unsigned long lcid,
            /* [out][idldescattr] */ void **pptinfo,
            /* [retval][out] */ void *retval);
        
        /* [id][restricted][funcdescattr] */ HRESULT ( STDMETHODCALLTYPE *GetIDsOfNames )( 
            FolderProperties2 * This,
            /* [in][idldescattr] */ struct GUID *riid,
            /* [in][idldescattr] */ signed char **rgszNames,
            /* [in][idldescattr] */ unsigned UINT cNames,
            /* [in][idldescattr] */ unsigned long lcid,
            /* [out][idldescattr] */ signed long *rgdispid,
            /* [retval][out] */ void *retval);
        
        /* [id][restricted][funcdescattr] */ HRESULT ( STDMETHODCALLTYPE *Invoke )( 
            FolderProperties2 * This,
            /* [in][idldescattr] */ signed long dispidMember,
            /* [in][idldescattr] */ struct GUID *riid,
            /* [in][idldescattr] */ unsigned long lcid,
            /* [in][idldescattr] */ unsigned short wFlags,
            /* [in][idldescattr] */ struct DISPPARAMS *pdispparams,
            /* [out][idldescattr] */ VARIANT *pvarResult,
            /* [out][idldescattr] */ struct EXCEPINFO *pexcepinfo,
            /* [out][idldescattr] */ unsigned UINT *puArgErr,
            /* [retval][out] */ void *retval);
        
        /* [id][propget][hidden][funcdescattr] */ HRESULT ( STDMETHODCALLTYPE *get___id )( 
            FolderProperties2 * This,
            /* [retval][out] */ BSTR *retval);
        
        /* [id][propget][funcdescattr] */ HRESULT ( STDMETHODCALLTYPE *get_FileName )( 
            FolderProperties2 * This,
            /* [retval][out] */ BSTR *retval);
        
        /* [id][propput][funcdescattr] */ HRESULT ( STDMETHODCALLTYPE *put_FileName )( 
            FolderProperties2 * This,
            /* [in][idldescattr] */ BSTR noname,
            /* [retval][out] */ void *retval);
        
        /* [id][propget][funcdescattr] */ HRESULT ( STDMETHODCALLTYPE *get_LocalPath )( 
            FolderProperties2 * This,
            /* [retval][out] */ BSTR *retval);
        
        /* [id][propget][funcdescattr] */ HRESULT ( STDMETHODCALLTYPE *get_FullPath )( 
            FolderProperties2 * This,
            /* [retval][out] */ BSTR *retval);
        
        /* [id][propget][funcdescattr] */ HRESULT ( STDMETHODCALLTYPE *get_URL )( 
            FolderProperties2 * This,
            /* [retval][out] */ BSTR *retval);
        
        /* [id][propget][funcdescattr] */ HRESULT ( STDMETHODCALLTYPE *get_Extender )( 
            FolderProperties2 * This,
            /* [in][idldescattr] */ BSTR ExtenderName,
            /* [retval][out] */ IDispatch **retval);
        
        /* [id][propget][funcdescattr] */ HRESULT ( STDMETHODCALLTYPE *get_ExtenderNames )( 
            FolderProperties2 * This,
            /* [retval][out] */ VARIANT *retval);
        
        /* [id][propget][funcdescattr] */ HRESULT ( STDMETHODCALLTYPE *get_ExtenderCATID )( 
            FolderProperties2 * This,
            /* [retval][out] */ BSTR *retval);
        
        /* [id][propget][funcdescattr] */ HRESULT ( STDMETHODCALLTYPE *get_WebReference )( 
            FolderProperties2 * This,
            /* [retval][out] */ BSTR *retval);
        
        /* [id][propput][funcdescattr] */ HRESULT ( STDMETHODCALLTYPE *put_WebReference )( 
            FolderProperties2 * This,
            /* [in][idldescattr] */ BSTR noname,
            /* [retval][out] */ void *retval);
        
        /* [id][propget][funcdescattr] */ HRESULT ( STDMETHODCALLTYPE *get_DefaultNamespace )( 
            FolderProperties2 * This,
            /* [retval][out] */ BSTR *retval);
        
        /* [id][propget][funcdescattr] */ HRESULT ( STDMETHODCALLTYPE *get_UrlBehavior )( 
            FolderProperties2 * This,
            /* [retval][out] */ enum webrefUrlBehavior *retval);
        
        /* [id][propput][funcdescattr] */ HRESULT ( STDMETHODCALLTYPE *put_UrlBehavior )( 
            FolderProperties2 * This,
            /* [in][idldescattr] */ enum webrefUrlBehavior noname,
            /* [retval][out] */ void *retval);
        
        /* [helpstring][id][propget] */ HRESULT ( STDMETHODCALLTYPE *get_WebReferenceInterface )( 
            FolderProperties2 * This,
            /* [retval][out] */ IUnknown **ppWebReference);
        
        END_INTERFACE
    } FolderProperties2Vtbl;

    interface FolderProperties2
    {
        CONST_VTBL struct FolderProperties2Vtbl *lpVtbl;
    };

    

#ifdef COBJMACROS


#define FolderProperties2_QueryInterface(This,riid,ppvObj,retval)	\
    ( (This)->lpVtbl -> QueryInterface(This,riid,ppvObj,retval) ) 

#define FolderProperties2_AddRef(This,retval)	\
    ( (This)->lpVtbl -> AddRef(This,retval) ) 

#define FolderProperties2_Release(This,retval)	\
    ( (This)->lpVtbl -> Release(This,retval) ) 

#define FolderProperties2_GetTypeInfoCount(This,pctinfo,retval)	\
    ( (This)->lpVtbl -> GetTypeInfoCount(This,pctinfo,retval) ) 

#define FolderProperties2_GetTypeInfo(This,itinfo,lcid,pptinfo,retval)	\
    ( (This)->lpVtbl -> GetTypeInfo(This,itinfo,lcid,pptinfo,retval) ) 

#define FolderProperties2_GetIDsOfNames(This,riid,rgszNames,cNames,lcid,rgdispid,retval)	\
    ( (This)->lpVtbl -> GetIDsOfNames(This,riid,rgszNames,cNames,lcid,rgdispid,retval) ) 

#define FolderProperties2_Invoke(This,dispidMember,riid,lcid,wFlags,pdispparams,pvarResult,pexcepinfo,puArgErr,retval)	\
    ( (This)->lpVtbl -> Invoke(This,dispidMember,riid,lcid,wFlags,pdispparams,pvarResult,pexcepinfo,puArgErr,retval) ) 

#define FolderProperties2_get___id(This,retval)	\
    ( (This)->lpVtbl -> get___id(This,retval) ) 

#define FolderProperties2_get_FileName(This,retval)	\
    ( (This)->lpVtbl -> get_FileName(This,retval) ) 

#define FolderProperties2_put_FileName(This,noname,retval)	\
    ( (This)->lpVtbl -> put_FileName(This,noname,retval) ) 

#define FolderProperties2_get_LocalPath(This,retval)	\
    ( (This)->lpVtbl -> get_LocalPath(This,retval) ) 

#define FolderProperties2_get_FullPath(This,retval)	\
    ( (This)->lpVtbl -> get_FullPath(This,retval) ) 

#define FolderProperties2_get_URL(This,retval)	\
    ( (This)->lpVtbl -> get_URL(This,retval) ) 

#define FolderProperties2_get_Extender(This,ExtenderName,retval)	\
    ( (This)->lpVtbl -> get_Extender(This,ExtenderName,retval) ) 

#define FolderProperties2_get_ExtenderNames(This,retval)	\
    ( (This)->lpVtbl -> get_ExtenderNames(This,retval) ) 

#define FolderProperties2_get_ExtenderCATID(This,retval)	\
    ( (This)->lpVtbl -> get_ExtenderCATID(This,retval) ) 

#define FolderProperties2_get_WebReference(This,retval)	\
    ( (This)->lpVtbl -> get_WebReference(This,retval) ) 

#define FolderProperties2_put_WebReference(This,noname,retval)	\
    ( (This)->lpVtbl -> put_WebReference(This,noname,retval) ) 

#define FolderProperties2_get_DefaultNamespace(This,retval)	\
    ( (This)->lpVtbl -> get_DefaultNamespace(This,retval) ) 

#define FolderProperties2_get_UrlBehavior(This,retval)	\
    ( (This)->lpVtbl -> get_UrlBehavior(This,retval) ) 

#define FolderProperties2_put_UrlBehavior(This,noname,retval)	\
    ( (This)->lpVtbl -> put_UrlBehavior(This,noname,retval) ) 


#define FolderProperties2_get_WebReferenceInterface(This,ppWebReference)	\
    ( (This)->lpVtbl -> get_WebReferenceInterface(This,ppWebReference) ) 

#endif /* COBJMACROS */


#endif 	/* C style interface */




#endif 	/* __FolderProperties2_INTERFACE_DEFINED__ */


#ifndef __FileProperties2_INTERFACE_DEFINED__
#define __FileProperties2_INTERFACE_DEFINED__

/* interface FileProperties2 */
/* [object][dual][unique][helpstring][uuid] */ 


EXTERN_C const IID IID_FileProperties2;

#if defined(__cplusplus) && !defined(CINTERFACE)
    
    MIDL_INTERFACE("41BE8D4D-F235-46d4-B9F8-C6D6459D503C")
    FileProperties2 : public FileProperties
    {
    public:
        virtual /* [helpstring][id][propget] */ HRESULT STDMETHODCALLTYPE get_CopyToOutputDirectory( 
            /* [retval][out] */ __RPC__out COPYTOOUTPUTSTATE *pCopy) = 0;
        
        virtual /* [helpstring][id][propput] */ HRESULT STDMETHODCALLTYPE put_CopyToOutputDirectory( 
            /* [in] */ COPYTOOUTPUTSTATE Copy) = 0;
        
        virtual /* [helpstring][id][propget] */ HRESULT STDMETHODCALLTYPE get_ItemType( 
            /* [retval][out] */ __RPC__deref_out_opt BSTR *pbstrItemType) = 0;
        
        virtual /* [helpstring][id][propput] */ HRESULT STDMETHODCALLTYPE put_ItemType( 
            /* [in] */ __RPC__in BSTR ItemType) = 0;
        
        virtual /* [helpstring][id][propget] */ HRESULT STDMETHODCALLTYPE get_IsSharedDesignTimeBuildInput( 
            /* [retval][out] */ __RPC__out VARIANT_BOOL *pbIsSharedDesignTimeBuildInput) = 0;
        
    };
    
#else 	/* C style interface */

    typedef struct FileProperties2Vtbl
    {
        BEGIN_INTERFACE
        
        /* [id][restricted][funcdescattr] */ HRESULT ( STDMETHODCALLTYPE *QueryInterface )( 
            FileProperties2 * This,
            /* [in][idldescattr] */ __RPC__in struct GUID *riid,
            /* [out][idldescattr] */ __RPC__deref_out_opt void **ppvObj,
            /* [retval][out] */ __RPC__out void *retval);
        
        /* [id][restricted][funcdescattr] */ HRESULT ( STDMETHODCALLTYPE *AddRef )( 
            FileProperties2 * This,
            /* [retval][out] */ __RPC__out unsigned long *retval);
        
        /* [id][restricted][funcdescattr] */ HRESULT ( STDMETHODCALLTYPE *Release )( 
            FileProperties2 * This,
            /* [retval][out] */ __RPC__out unsigned long *retval);
        
        /* [id][restricted][funcdescattr] */ HRESULT ( STDMETHODCALLTYPE *GetTypeInfoCount )( 
            FileProperties2 * This,
            /* [out][idldescattr] */ __RPC__out unsigned UINT *pctinfo,
            /* [retval][out] */ __RPC__out void *retval);
        
        /* [id][restricted][funcdescattr] */ HRESULT ( STDMETHODCALLTYPE *GetTypeInfo )( 
            FileProperties2 * This,
            /* [in][idldescattr] */ unsigned UINT itinfo,
            /* [in][idldescattr] */ unsigned long lcid,
            /* [out][idldescattr] */ __RPC__deref_out_opt void **pptinfo,
            /* [retval][out] */ __RPC__out void *retval);
        
        /* [id][restricted][funcdescattr] */ HRESULT ( STDMETHODCALLTYPE *GetIDsOfNames )( 
            FileProperties2 * This,
            /* [in][idldescattr] */ __RPC__in struct GUID *riid,
            /* [in][idldescattr] */ __RPC__deref_in_opt signed char **rgszNames,
            /* [in][idldescattr] */ unsigned UINT cNames,
            /* [in][idldescattr] */ unsigned long lcid,
            /* [out][idldescattr] */ __RPC__out signed long *rgdispid,
            /* [retval][out] */ __RPC__out void *retval);
        
        /* [id][restricted][funcdescattr] */ HRESULT ( STDMETHODCALLTYPE *Invoke )( 
            FileProperties2 * This,
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
            FileProperties2 * This,
            /* [retval][out] */ __RPC__deref_out_opt BSTR *retval);
        
        /* [id][propget][funcdescattr] */ HRESULT ( STDMETHODCALLTYPE *get_FileName )( 
            FileProperties2 * This,
            /* [retval][out] */ __RPC__deref_out_opt BSTR *retval);
        
        /* [id][propput][funcdescattr] */ HRESULT ( STDMETHODCALLTYPE *put_FileName )( 
            FileProperties2 * This,
            /* [in][idldescattr] */ __RPC__in BSTR noname,
            /* [retval][out] */ __RPC__out void *retval);
        
        /* [id][propget][funcdescattr] */ HRESULT ( STDMETHODCALLTYPE *get_Extension )( 
            FileProperties2 * This,
            /* [retval][out] */ __RPC__deref_out_opt BSTR *retval);
        
        /* [id][propget][funcdescattr] */ HRESULT ( STDMETHODCALLTYPE *get_Filesize )( 
            FileProperties2 * This,
            /* [retval][out] */ __RPC__out unsigned long *retval);
        
        /* [id][propget][funcdescattr] */ HRESULT ( STDMETHODCALLTYPE *get_LocalPath )( 
            FileProperties2 * This,
            /* [retval][out] */ __RPC__deref_out_opt BSTR *retval);
        
        /* [id][propget][funcdescattr] */ HRESULT ( STDMETHODCALLTYPE *get_FullPath )( 
            FileProperties2 * This,
            /* [retval][out] */ __RPC__deref_out_opt BSTR *retval);
        
        /* [id][propget][funcdescattr] */ HRESULT ( STDMETHODCALLTYPE *get_URL )( 
            FileProperties2 * This,
            /* [retval][out] */ __RPC__deref_out_opt BSTR *retval);
        
        /* [id][propget][funcdescattr] */ HRESULT ( STDMETHODCALLTYPE *get_HTMLTitle )( 
            FileProperties2 * This,
            /* [retval][out] */ __RPC__deref_out_opt BSTR *retval);
        
        /* [id][propget][funcdescattr] */ HRESULT ( STDMETHODCALLTYPE *get_Author )( 
            FileProperties2 * This,
            /* [retval][out] */ __RPC__deref_out_opt BSTR *retval);
        
        /* [id][propget][funcdescattr] */ HRESULT ( STDMETHODCALLTYPE *get_DateCreated )( 
            FileProperties2 * This,
            /* [retval][out] */ __RPC__deref_out_opt BSTR *retval);
        
        /* [id][propget][funcdescattr] */ HRESULT ( STDMETHODCALLTYPE *get_DateModified )( 
            FileProperties2 * This,
            /* [retval][out] */ __RPC__deref_out_opt BSTR *retval);
        
        /* [id][propget][funcdescattr] */ HRESULT ( STDMETHODCALLTYPE *get_ModifiedBy )( 
            FileProperties2 * This,
            /* [retval][out] */ __RPC__deref_out_opt BSTR *retval);
        
        /* [id][propget][funcdescattr] */ HRESULT ( STDMETHODCALLTYPE *get_SubType )( 
            FileProperties2 * This,
            /* [retval][out] */ __RPC__deref_out_opt BSTR *retval);
        
        /* [id][propput][funcdescattr] */ HRESULT ( STDMETHODCALLTYPE *put_SubType )( 
            FileProperties2 * This,
            /* [in][idldescattr] */ __RPC__in BSTR noname,
            /* [retval][out] */ __RPC__out void *retval);
        
        /* [id][propget][funcdescattr] */ HRESULT ( STDMETHODCALLTYPE *get_Extender )( 
            FileProperties2 * This,
            /* [in][idldescattr] */ __RPC__in BSTR ExtenderName,
            /* [retval][out] */ __RPC__deref_out_opt IDispatch **retval);
        
        /* [id][propget][funcdescattr] */ HRESULT ( STDMETHODCALLTYPE *get_ExtenderNames )( 
            FileProperties2 * This,
            /* [retval][out] */ __RPC__out VARIANT *retval);
        
        /* [id][propget][funcdescattr] */ HRESULT ( STDMETHODCALLTYPE *get_ExtenderCATID )( 
            FileProperties2 * This,
            /* [retval][out] */ __RPC__deref_out_opt BSTR *retval);
        
        /* [id][propget][funcdescattr] */ HRESULT ( STDMETHODCALLTYPE *get_BuildAction )( 
            FileProperties2 * This,
            /* [retval][out] */ __RPC__out enum prjBuildAction *retval);
        
        /* [id][propput][funcdescattr] */ HRESULT ( STDMETHODCALLTYPE *put_BuildAction )( 
            FileProperties2 * This,
            /* [in][idldescattr] */ enum prjBuildAction noname,
            /* [retval][out] */ __RPC__out void *retval);
        
        /* [id][propget][funcdescattr] */ HRESULT ( STDMETHODCALLTYPE *get_CustomTool )( 
            FileProperties2 * This,
            /* [retval][out] */ __RPC__deref_out_opt BSTR *retval);
        
        /* [id][propput][funcdescattr] */ HRESULT ( STDMETHODCALLTYPE *put_CustomTool )( 
            FileProperties2 * This,
            /* [in][idldescattr] */ __RPC__in BSTR noname,
            /* [retval][out] */ __RPC__out void *retval);
        
        /* [id][propget][funcdescattr] */ HRESULT ( STDMETHODCALLTYPE *get_CustomToolNamespace )( 
            FileProperties2 * This,
            /* [retval][out] */ __RPC__deref_out_opt BSTR *retval);
        
        /* [id][propput][funcdescattr] */ HRESULT ( STDMETHODCALLTYPE *put_CustomToolNamespace )( 
            FileProperties2 * This,
            /* [in][idldescattr] */ __RPC__in BSTR noname,
            /* [retval][out] */ __RPC__out void *retval);
        
        /* [id][propget][funcdescattr] */ HRESULT ( STDMETHODCALLTYPE *get_CustomToolOutput )( 
            FileProperties2 * This,
            /* [retval][out] */ __RPC__deref_out_opt BSTR *retval);
        
        /* [id][propget][funcdescattr] */ HRESULT ( STDMETHODCALLTYPE *get_IsCustomToolOutput )( 
            FileProperties2 * This,
            /* [retval][out] */ __RPC__out BOOLEAN *retval);
        
        /* [id][propget][funcdescattr] */ HRESULT ( STDMETHODCALLTYPE *get_IsDependentFile )( 
            FileProperties2 * This,
            /* [retval][out] */ __RPC__out BOOLEAN *retval);
        
        /* [id][propget][funcdescattr] */ HRESULT ( STDMETHODCALLTYPE *get_IsLink )( 
            FileProperties2 * This,
            /* [retval][out] */ __RPC__out BOOLEAN *retval);
        
        /* [id][propget][funcdescattr] */ HRESULT ( STDMETHODCALLTYPE *get_IsDesignTimeBuildInput )( 
            FileProperties2 * This,
            /* [retval][out] */ __RPC__out BOOLEAN *retval);
        
        /* [helpstring][id][propget] */ HRESULT ( STDMETHODCALLTYPE *get_CopyToOutputDirectory )( 
            FileProperties2 * This,
            /* [retval][out] */ __RPC__out COPYTOOUTPUTSTATE *pCopy);
        
        /* [helpstring][id][propput] */ HRESULT ( STDMETHODCALLTYPE *put_CopyToOutputDirectory )( 
            FileProperties2 * This,
            /* [in] */ COPYTOOUTPUTSTATE Copy);
        
        /* [helpstring][id][propget] */ HRESULT ( STDMETHODCALLTYPE *get_ItemType )( 
            FileProperties2 * This,
            /* [retval][out] */ __RPC__deref_out_opt BSTR *pbstrItemType);
        
        /* [helpstring][id][propput] */ HRESULT ( STDMETHODCALLTYPE *put_ItemType )( 
            FileProperties2 * This,
            /* [in] */ __RPC__in BSTR ItemType);
        
        /* [helpstring][id][propget] */ HRESULT ( STDMETHODCALLTYPE *get_IsSharedDesignTimeBuildInput )( 
            FileProperties2 * This,
            /* [retval][out] */ __RPC__out VARIANT_BOOL *pbIsSharedDesignTimeBuildInput);
        
        END_INTERFACE
    } FileProperties2Vtbl;

    interface FileProperties2
    {
        CONST_VTBL struct FileProperties2Vtbl *lpVtbl;
    };

    

#ifdef COBJMACROS


#define FileProperties2_QueryInterface(This,riid,ppvObj,retval)	\
    ( (This)->lpVtbl -> QueryInterface(This,riid,ppvObj,retval) ) 

#define FileProperties2_AddRef(This,retval)	\
    ( (This)->lpVtbl -> AddRef(This,retval) ) 

#define FileProperties2_Release(This,retval)	\
    ( (This)->lpVtbl -> Release(This,retval) ) 

#define FileProperties2_GetTypeInfoCount(This,pctinfo,retval)	\
    ( (This)->lpVtbl -> GetTypeInfoCount(This,pctinfo,retval) ) 

#define FileProperties2_GetTypeInfo(This,itinfo,lcid,pptinfo,retval)	\
    ( (This)->lpVtbl -> GetTypeInfo(This,itinfo,lcid,pptinfo,retval) ) 

#define FileProperties2_GetIDsOfNames(This,riid,rgszNames,cNames,lcid,rgdispid,retval)	\
    ( (This)->lpVtbl -> GetIDsOfNames(This,riid,rgszNames,cNames,lcid,rgdispid,retval) ) 

#define FileProperties2_Invoke(This,dispidMember,riid,lcid,wFlags,pdispparams,pvarResult,pexcepinfo,puArgErr,retval)	\
    ( (This)->lpVtbl -> Invoke(This,dispidMember,riid,lcid,wFlags,pdispparams,pvarResult,pexcepinfo,puArgErr,retval) ) 

#define FileProperties2_get___id(This,retval)	\
    ( (This)->lpVtbl -> get___id(This,retval) ) 

#define FileProperties2_get_FileName(This,retval)	\
    ( (This)->lpVtbl -> get_FileName(This,retval) ) 

#define FileProperties2_put_FileName(This,noname,retval)	\
    ( (This)->lpVtbl -> put_FileName(This,noname,retval) ) 

#define FileProperties2_get_Extension(This,retval)	\
    ( (This)->lpVtbl -> get_Extension(This,retval) ) 

#define FileProperties2_get_Filesize(This,retval)	\
    ( (This)->lpVtbl -> get_Filesize(This,retval) ) 

#define FileProperties2_get_LocalPath(This,retval)	\
    ( (This)->lpVtbl -> get_LocalPath(This,retval) ) 

#define FileProperties2_get_FullPath(This,retval)	\
    ( (This)->lpVtbl -> get_FullPath(This,retval) ) 

#define FileProperties2_get_URL(This,retval)	\
    ( (This)->lpVtbl -> get_URL(This,retval) ) 

#define FileProperties2_get_HTMLTitle(This,retval)	\
    ( (This)->lpVtbl -> get_HTMLTitle(This,retval) ) 

#define FileProperties2_get_Author(This,retval)	\
    ( (This)->lpVtbl -> get_Author(This,retval) ) 

#define FileProperties2_get_DateCreated(This,retval)	\
    ( (This)->lpVtbl -> get_DateCreated(This,retval) ) 

#define FileProperties2_get_DateModified(This,retval)	\
    ( (This)->lpVtbl -> get_DateModified(This,retval) ) 

#define FileProperties2_get_ModifiedBy(This,retval)	\
    ( (This)->lpVtbl -> get_ModifiedBy(This,retval) ) 

#define FileProperties2_get_SubType(This,retval)	\
    ( (This)->lpVtbl -> get_SubType(This,retval) ) 

#define FileProperties2_put_SubType(This,noname,retval)	\
    ( (This)->lpVtbl -> put_SubType(This,noname,retval) ) 

#define FileProperties2_get_Extender(This,ExtenderName,retval)	\
    ( (This)->lpVtbl -> get_Extender(This,ExtenderName,retval) ) 

#define FileProperties2_get_ExtenderNames(This,retval)	\
    ( (This)->lpVtbl -> get_ExtenderNames(This,retval) ) 

#define FileProperties2_get_ExtenderCATID(This,retval)	\
    ( (This)->lpVtbl -> get_ExtenderCATID(This,retval) ) 

#define FileProperties2_get_BuildAction(This,retval)	\
    ( (This)->lpVtbl -> get_BuildAction(This,retval) ) 

#define FileProperties2_put_BuildAction(This,noname,retval)	\
    ( (This)->lpVtbl -> put_BuildAction(This,noname,retval) ) 

#define FileProperties2_get_CustomTool(This,retval)	\
    ( (This)->lpVtbl -> get_CustomTool(This,retval) ) 

#define FileProperties2_put_CustomTool(This,noname,retval)	\
    ( (This)->lpVtbl -> put_CustomTool(This,noname,retval) ) 

#define FileProperties2_get_CustomToolNamespace(This,retval)	\
    ( (This)->lpVtbl -> get_CustomToolNamespace(This,retval) ) 

#define FileProperties2_put_CustomToolNamespace(This,noname,retval)	\
    ( (This)->lpVtbl -> put_CustomToolNamespace(This,noname,retval) ) 

#define FileProperties2_get_CustomToolOutput(This,retval)	\
    ( (This)->lpVtbl -> get_CustomToolOutput(This,retval) ) 

#define FileProperties2_get_IsCustomToolOutput(This,retval)	\
    ( (This)->lpVtbl -> get_IsCustomToolOutput(This,retval) ) 

#define FileProperties2_get_IsDependentFile(This,retval)	\
    ( (This)->lpVtbl -> get_IsDependentFile(This,retval) ) 

#define FileProperties2_get_IsLink(This,retval)	\
    ( (This)->lpVtbl -> get_IsLink(This,retval) ) 

#define FileProperties2_get_IsDesignTimeBuildInput(This,retval)	\
    ( (This)->lpVtbl -> get_IsDesignTimeBuildInput(This,retval) ) 


#define FileProperties2_get_CopyToOutputDirectory(This,pCopy)	\
    ( (This)->lpVtbl -> get_CopyToOutputDirectory(This,pCopy) ) 

#define FileProperties2_put_CopyToOutputDirectory(This,Copy)	\
    ( (This)->lpVtbl -> put_CopyToOutputDirectory(This,Copy) ) 

#define FileProperties2_get_ItemType(This,pbstrItemType)	\
    ( (This)->lpVtbl -> get_ItemType(This,pbstrItemType) ) 

#define FileProperties2_put_ItemType(This,ItemType)	\
    ( (This)->lpVtbl -> put_ItemType(This,ItemType) ) 

#define FileProperties2_get_IsSharedDesignTimeBuildInput(This,pbIsSharedDesignTimeBuildInput)	\
    ( (This)->lpVtbl -> get_IsSharedDesignTimeBuildInput(This,pbIsSharedDesignTimeBuildInput) ) 

#endif /* COBJMACROS */


#endif 	/* C style interface */




#endif 	/* __FileProperties2_INTERFACE_DEFINED__ */


#ifndef __IVsApplicationSettings_INTERFACE_DEFINED__
#define __IVsApplicationSettings_INTERFACE_DEFINED__

/* interface IVsApplicationSettings */
/* [uuid][object] */ 


EXTERN_C const IID IID_IVsApplicationSettings;

#if defined(__cplusplus) && !defined(CINTERFACE)
    
    MIDL_INTERFACE("CC5B9866-BD93-4aff-8D61-C73ED3BB77D0")
    IVsApplicationSettings : public IUnknown
    {
    public:
        virtual /* [helpstring][id] */ HRESULT STDMETHODCALLTYPE GetPropertyInfo( 
            /* [in] */ __RPC__in LPCWSTR pszWebServiceName,
            /* [out] */ __RPC__deref_out_opt BSTR *pbstrAppSettingsObjectName,
            /* [out] */ __RPC__deref_out_opt BSTR *pbstrPropertyName) = 0;
        
        virtual /* [helpstring][id] */ HRESULT STDMETHODCALLTYPE GetAppSettingsPropertyExpression( 
            /* [in] */ __RPC__in LPCWSTR pszAppSettingsObjectName,
            /* [in] */ __RPC__in LPCWSTR pszPropertyName,
            /* [retval][out] */ __RPC__deref_out_opt IUnknown **ppUnkExpression) = 0;
        
        virtual /* [helpstring][id] */ HRESULT STDMETHODCALLTYPE EnsureWebServiceUrlPropertyExpression( 
            /* [in] */ __RPC__in LPCWSTR pszAppSettingsObjectName,
            /* [in] */ __RPC__in LPCWSTR pszPropertyName,
            /* [in] */ VARIANT varPropertyValue,
            /* [retval][out] */ __RPC__deref_out_opt IUnknown **ppUnkExpression) = 0;
        
        virtual /* [helpstring][id] */ HRESULT STDMETHODCALLTYPE SetPropertyInfo( 
            /* [in] */ __RPC__in LPCWSTR pszAppSettingsObjectName,
            /* [in] */ __RPC__in LPCWSTR pszPropertyName) = 0;
        
    };
    
#else 	/* C style interface */

    typedef struct IVsApplicationSettingsVtbl
    {
        BEGIN_INTERFACE
        
        HRESULT ( STDMETHODCALLTYPE *QueryInterface )( 
            IVsApplicationSettings * This,
            /* [in] */ __RPC__in REFIID riid,
            /* [iid_is][out] */ 
            __RPC__deref_out  void **ppvObject);
        
        ULONG ( STDMETHODCALLTYPE *AddRef )( 
            IVsApplicationSettings * This);
        
        ULONG ( STDMETHODCALLTYPE *Release )( 
            IVsApplicationSettings * This);
        
        /* [helpstring][id] */ HRESULT ( STDMETHODCALLTYPE *GetPropertyInfo )( 
            IVsApplicationSettings * This,
            /* [in] */ __RPC__in LPCWSTR pszWebServiceName,
            /* [out] */ __RPC__deref_out_opt BSTR *pbstrAppSettingsObjectName,
            /* [out] */ __RPC__deref_out_opt BSTR *pbstrPropertyName);
        
        /* [helpstring][id] */ HRESULT ( STDMETHODCALLTYPE *GetAppSettingsPropertyExpression )( 
            IVsApplicationSettings * This,
            /* [in] */ __RPC__in LPCWSTR pszAppSettingsObjectName,
            /* [in] */ __RPC__in LPCWSTR pszPropertyName,
            /* [retval][out] */ __RPC__deref_out_opt IUnknown **ppUnkExpression);
        
        /* [helpstring][id] */ HRESULT ( STDMETHODCALLTYPE *EnsureWebServiceUrlPropertyExpression )( 
            IVsApplicationSettings * This,
            /* [in] */ __RPC__in LPCWSTR pszAppSettingsObjectName,
            /* [in] */ __RPC__in LPCWSTR pszPropertyName,
            /* [in] */ VARIANT varPropertyValue,
            /* [retval][out] */ __RPC__deref_out_opt IUnknown **ppUnkExpression);
        
        /* [helpstring][id] */ HRESULT ( STDMETHODCALLTYPE *SetPropertyInfo )( 
            IVsApplicationSettings * This,
            /* [in] */ __RPC__in LPCWSTR pszAppSettingsObjectName,
            /* [in] */ __RPC__in LPCWSTR pszPropertyName);
        
        END_INTERFACE
    } IVsApplicationSettingsVtbl;

    interface IVsApplicationSettings
    {
        CONST_VTBL struct IVsApplicationSettingsVtbl *lpVtbl;
    };

    

#ifdef COBJMACROS


#define IVsApplicationSettings_QueryInterface(This,riid,ppvObject)	\
    ( (This)->lpVtbl -> QueryInterface(This,riid,ppvObject) ) 

#define IVsApplicationSettings_AddRef(This)	\
    ( (This)->lpVtbl -> AddRef(This) ) 

#define IVsApplicationSettings_Release(This)	\
    ( (This)->lpVtbl -> Release(This) ) 


#define IVsApplicationSettings_GetPropertyInfo(This,pszWebServiceName,pbstrAppSettingsObjectName,pbstrPropertyName)	\
    ( (This)->lpVtbl -> GetPropertyInfo(This,pszWebServiceName,pbstrAppSettingsObjectName,pbstrPropertyName) ) 

#define IVsApplicationSettings_GetAppSettingsPropertyExpression(This,pszAppSettingsObjectName,pszPropertyName,ppUnkExpression)	\
    ( (This)->lpVtbl -> GetAppSettingsPropertyExpression(This,pszAppSettingsObjectName,pszPropertyName,ppUnkExpression) ) 

#define IVsApplicationSettings_EnsureWebServiceUrlPropertyExpression(This,pszAppSettingsObjectName,pszPropertyName,varPropertyValue,ppUnkExpression)	\
    ( (This)->lpVtbl -> EnsureWebServiceUrlPropertyExpression(This,pszAppSettingsObjectName,pszPropertyName,varPropertyValue,ppUnkExpression) ) 

#define IVsApplicationSettings_SetPropertyInfo(This,pszAppSettingsObjectName,pszPropertyName)	\
    ( (This)->lpVtbl -> SetPropertyInfo(This,pszAppSettingsObjectName,pszPropertyName) ) 

#endif /* COBJMACROS */


#endif 	/* C style interface */




#endif 	/* __IVsApplicationSettings_INTERFACE_DEFINED__ */


#ifndef __SVSWebReferenceDynamicProperties_INTERFACE_DEFINED__
#define __SVSWebReferenceDynamicProperties_INTERFACE_DEFINED__

/* interface SVSWebReferenceDynamicProperties */
/* [uuid][object] */ 


EXTERN_C const IID IID_SVSWebReferenceDynamicProperties;

#if defined(__cplusplus) && !defined(CINTERFACE)
    
    MIDL_INTERFACE("C65A2F92-B350-4b0f-9BC6-B00E24BC8B9D")
    SVSWebReferenceDynamicProperties : public IUnknown
    {
    public:
    };
    
#else 	/* C style interface */

    typedef struct SVSWebReferenceDynamicPropertiesVtbl
    {
        BEGIN_INTERFACE
        
        HRESULT ( STDMETHODCALLTYPE *QueryInterface )( 
            SVSWebReferenceDynamicProperties * This,
            /* [in] */ __RPC__in REFIID riid,
            /* [iid_is][out] */ 
            __RPC__deref_out  void **ppvObject);
        
        ULONG ( STDMETHODCALLTYPE *AddRef )( 
            SVSWebReferenceDynamicProperties * This);
        
        ULONG ( STDMETHODCALLTYPE *Release )( 
            SVSWebReferenceDynamicProperties * This);
        
        END_INTERFACE
    } SVSWebReferenceDynamicPropertiesVtbl;

    interface SVSWebReferenceDynamicProperties
    {
        CONST_VTBL struct SVSWebReferenceDynamicPropertiesVtbl *lpVtbl;
    };

    

#ifdef COBJMACROS


#define SVSWebReferenceDynamicProperties_QueryInterface(This,riid,ppvObject)	\
    ( (This)->lpVtbl -> QueryInterface(This,riid,ppvObject) ) 

#define SVSWebReferenceDynamicProperties_AddRef(This)	\
    ( (This)->lpVtbl -> AddRef(This) ) 

#define SVSWebReferenceDynamicProperties_Release(This)	\
    ( (This)->lpVtbl -> Release(This) ) 


#endif /* COBJMACROS */


#endif 	/* C style interface */




#endif 	/* __SVSWebReferenceDynamicProperties_INTERFACE_DEFINED__ */


#ifndef __IVSWebReferenceDynamicProperties2_INTERFACE_DEFINED__
#define __IVSWebReferenceDynamicProperties2_INTERFACE_DEFINED__

/* interface IVSWebReferenceDynamicProperties2 */
/* [dual][unique][helpstring][uuid][object] */ 


EXTERN_C const IID IID_IVSWebReferenceDynamicProperties2;

#if defined(__cplusplus) && !defined(CINTERFACE)
    
    MIDL_INTERFACE("E4311E4C-0819-404b-B0D9-F5F44716ECEC")
    IVSWebReferenceDynamicProperties2 : public IDispatch
    {
    public:
        virtual /* [helpstring][id] */ HRESULT STDMETHODCALLTYPE GetDynamicPropertyName( 
            /* [in] */ __RPC__in LPCWSTR pszWebServiceName,
            /* [retval][out] */ __RPC__deref_out_opt BSTR *pbstrPropertyName) = 0;
        
        virtual /* [helpstring][id] */ HRESULT STDMETHODCALLTYPE SetDynamicProperty( 
            /* [in] */ __RPC__in LPCWSTR pszUrl,
            /* [in] */ __RPC__in LPCWSTR pszPropertyName) = 0;
        
        virtual /* [helpstring][id] */ HRESULT STDMETHODCALLTYPE SupportsDynamicProperties( 
            /* [retval][out] */ __RPC__out VARIANT_BOOL *pbSupportsDynamicProperties) = 0;
        
    };
    
#else 	/* C style interface */

    typedef struct IVSWebReferenceDynamicProperties2Vtbl
    {
        BEGIN_INTERFACE
        
        HRESULT ( STDMETHODCALLTYPE *QueryInterface )( 
            IVSWebReferenceDynamicProperties2 * This,
            /* [in] */ __RPC__in REFIID riid,
            /* [iid_is][out] */ 
            __RPC__deref_out  void **ppvObject);
        
        ULONG ( STDMETHODCALLTYPE *AddRef )( 
            IVSWebReferenceDynamicProperties2 * This);
        
        ULONG ( STDMETHODCALLTYPE *Release )( 
            IVSWebReferenceDynamicProperties2 * This);
        
        HRESULT ( STDMETHODCALLTYPE *GetTypeInfoCount )( 
            IVSWebReferenceDynamicProperties2 * This,
            /* [out] */ __RPC__out UINT *pctinfo);
        
        HRESULT ( STDMETHODCALLTYPE *GetTypeInfo )( 
            IVSWebReferenceDynamicProperties2 * This,
            /* [in] */ UINT iTInfo,
            /* [in] */ LCID lcid,
            /* [out] */ __RPC__deref_out_opt ITypeInfo **ppTInfo);
        
        HRESULT ( STDMETHODCALLTYPE *GetIDsOfNames )( 
            IVSWebReferenceDynamicProperties2 * This,
            /* [in] */ __RPC__in REFIID riid,
            /* [size_is][in] */ __RPC__in_ecount_full(cNames) LPOLESTR *rgszNames,
            /* [range][in] */ UINT cNames,
            /* [in] */ LCID lcid,
            /* [size_is][out] */ __RPC__out_ecount_full(cNames) DISPID *rgDispId);
        
        /* [local] */ HRESULT ( STDMETHODCALLTYPE *Invoke )( 
            IVSWebReferenceDynamicProperties2 * This,
            /* [in] */ DISPID dispIdMember,
            /* [in] */ REFIID riid,
            /* [in] */ LCID lcid,
            /* [in] */ WORD wFlags,
            /* [out][in] */ DISPPARAMS *pDispParams,
            /* [out] */ VARIANT *pVarResult,
            /* [out] */ EXCEPINFO *pExcepInfo,
            /* [out] */ UINT *puArgErr);
        
        /* [helpstring][id] */ HRESULT ( STDMETHODCALLTYPE *GetDynamicPropertyName )( 
            IVSWebReferenceDynamicProperties2 * This,
            /* [in] */ __RPC__in LPCWSTR pszWebServiceName,
            /* [retval][out] */ __RPC__deref_out_opt BSTR *pbstrPropertyName);
        
        /* [helpstring][id] */ HRESULT ( STDMETHODCALLTYPE *SetDynamicProperty )( 
            IVSWebReferenceDynamicProperties2 * This,
            /* [in] */ __RPC__in LPCWSTR pszUrl,
            /* [in] */ __RPC__in LPCWSTR pszPropertyName);
        
        /* [helpstring][id] */ HRESULT ( STDMETHODCALLTYPE *SupportsDynamicProperties )( 
            IVSWebReferenceDynamicProperties2 * This,
            /* [retval][out] */ __RPC__out VARIANT_BOOL *pbSupportsDynamicProperties);
        
        END_INTERFACE
    } IVSWebReferenceDynamicProperties2Vtbl;

    interface IVSWebReferenceDynamicProperties2
    {
        CONST_VTBL struct IVSWebReferenceDynamicProperties2Vtbl *lpVtbl;
    };

    

#ifdef COBJMACROS


#define IVSWebReferenceDynamicProperties2_QueryInterface(This,riid,ppvObject)	\
    ( (This)->lpVtbl -> QueryInterface(This,riid,ppvObject) ) 

#define IVSWebReferenceDynamicProperties2_AddRef(This)	\
    ( (This)->lpVtbl -> AddRef(This) ) 

#define IVSWebReferenceDynamicProperties2_Release(This)	\
    ( (This)->lpVtbl -> Release(This) ) 


#define IVSWebReferenceDynamicProperties2_GetTypeInfoCount(This,pctinfo)	\
    ( (This)->lpVtbl -> GetTypeInfoCount(This,pctinfo) ) 

#define IVSWebReferenceDynamicProperties2_GetTypeInfo(This,iTInfo,lcid,ppTInfo)	\
    ( (This)->lpVtbl -> GetTypeInfo(This,iTInfo,lcid,ppTInfo) ) 

#define IVSWebReferenceDynamicProperties2_GetIDsOfNames(This,riid,rgszNames,cNames,lcid,rgDispId)	\
    ( (This)->lpVtbl -> GetIDsOfNames(This,riid,rgszNames,cNames,lcid,rgDispId) ) 

#define IVSWebReferenceDynamicProperties2_Invoke(This,dispIdMember,riid,lcid,wFlags,pDispParams,pVarResult,pExcepInfo,puArgErr)	\
    ( (This)->lpVtbl -> Invoke(This,dispIdMember,riid,lcid,wFlags,pDispParams,pVarResult,pExcepInfo,puArgErr) ) 


#define IVSWebReferenceDynamicProperties2_GetDynamicPropertyName(This,pszWebServiceName,pbstrPropertyName)	\
    ( (This)->lpVtbl -> GetDynamicPropertyName(This,pszWebServiceName,pbstrPropertyName) ) 

#define IVSWebReferenceDynamicProperties2_SetDynamicProperty(This,pszUrl,pszPropertyName)	\
    ( (This)->lpVtbl -> SetDynamicProperty(This,pszUrl,pszPropertyName) ) 

#define IVSWebReferenceDynamicProperties2_SupportsDynamicProperties(This,pbSupportsDynamicProperties)	\
    ( (This)->lpVtbl -> SupportsDynamicProperties(This,pbSupportsDynamicProperties) ) 

#endif /* COBJMACROS */


#endif 	/* C style interface */




#endif 	/* __IVSWebReferenceDynamicProperties2_INTERFACE_DEFINED__ */



#ifndef __vsContextGuids_MODULE_DEFINED__
#define __vsContextGuids_MODULE_DEFINED__


/* module vsContextGuids */
/* [helpstring][dllname][uuid] */ 

const LPSTR vsContextGuidVCSProject	=	"{FAE04EC1-301F-11D3-BF4B-00C04F79EFBC}";

const LPSTR vsContextGuidVCSEditor	=	"{694DD9B6-B865-4C5B-AD85-86356E9C88DC}";

const LPSTR vsContextGuidVBProject	=	"{164B10B9-B200-11D0-8C61-00A0C91E29D5}";

const LPSTR vsContextGuidVBEditor	=	"{E34ACDC0-BAAE-11D0-88BF-00A0C9110049}";

const LPSTR vsContextGuidVJSProject	=	"{E6FDF8B0-F3D1-11D4-8576-0002A516ECE8}";

const LPSTR vsContextGuidVJSEditor	=	"{E6FDF88A-F3D1-11D4-8576-0002A516ECE8}";

#endif /* __vsContextGuids_MODULE_DEFINED__ */
#endif /* __VSLangProj80_LIBRARY_DEFINED__ */

/* Additional Prototypes for ALL interfaces */

/* end of Additional Prototypes */

#ifdef __cplusplus
}
#endif

#endif


