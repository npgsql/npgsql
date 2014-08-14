

/* this ALWAYS GENERATED file contains the definitions for the interfaces */


 /* File created by MIDL compiler version 6.00.0361 */
/* Compiler settings for vslangproj.idl:
    Oicf, W0, Zp8, env=Win32 (32b run)
    protocol : dce , ms_ext, c_ext
    error checks: allocation ref bounds_check enum stub_data 
    VC __declspec() decoration level: 
         __declspec(uuid()), __declspec(selectany), __declspec(novtable)
         DECLSPEC_UUID(), MIDL_INTERFACE()
*/
//@@MIDL_FILE_HEADING(  )

#pragma warning( disable: 4049 )  /* more than 64k source lines */


/* verify that the <rpcndr.h> version is high enough to compile this file*/
#ifndef __REQUIRED_RPCNDR_H_VERSION__
#define __REQUIRED_RPCNDR_H_VERSION__ 440
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

#ifndef __vslangproj_h__
#define __vslangproj_h__

#if defined(_MSC_VER) && (_MSC_VER >= 1020)
#pragma once
#endif

/* Forward Declarations */ 

#ifndef __ProjectConfigurationProperties_FWD_DEFINED__
#define __ProjectConfigurationProperties_FWD_DEFINED__
typedef interface ProjectConfigurationProperties ProjectConfigurationProperties;
#endif 	/* __ProjectConfigurationProperties_FWD_DEFINED__ */


#ifndef __ProjectProperties_FWD_DEFINED__
#define __ProjectProperties_FWD_DEFINED__
typedef interface ProjectProperties ProjectProperties;
#endif 	/* __ProjectProperties_FWD_DEFINED__ */


#ifndef __FileProperties_FWD_DEFINED__
#define __FileProperties_FWD_DEFINED__
typedef interface FileProperties FileProperties;
#endif 	/* __FileProperties_FWD_DEFINED__ */


#ifndef __FolderProperties_FWD_DEFINED__
#define __FolderProperties_FWD_DEFINED__
typedef interface FolderProperties FolderProperties;
#endif 	/* __FolderProperties_FWD_DEFINED__ */


#ifndef __Reference_FWD_DEFINED__
#define __Reference_FWD_DEFINED__
typedef interface Reference Reference;
#endif 	/* __Reference_FWD_DEFINED__ */


#ifndef __References_FWD_DEFINED__
#define __References_FWD_DEFINED__
typedef interface References References;
#endif 	/* __References_FWD_DEFINED__ */


#ifndef ___ReferencesEvents_FWD_DEFINED__
#define ___ReferencesEvents_FWD_DEFINED__
typedef interface _ReferencesEvents _ReferencesEvents;
#endif 	/* ___ReferencesEvents_FWD_DEFINED__ */


#ifndef ___dispReferencesEvents_FWD_DEFINED__
#define ___dispReferencesEvents_FWD_DEFINED__
typedef interface _dispReferencesEvents _dispReferencesEvents;
#endif 	/* ___dispReferencesEvents_FWD_DEFINED__ */


#ifndef __ReferencesEvents_FWD_DEFINED__
#define __ReferencesEvents_FWD_DEFINED__

#ifdef __cplusplus
typedef class ReferencesEvents ReferencesEvents;
#else
typedef struct ReferencesEvents ReferencesEvents;
#endif /* __cplusplus */

#endif 	/* __ReferencesEvents_FWD_DEFINED__ */


#ifndef __BuildManager_FWD_DEFINED__
#define __BuildManager_FWD_DEFINED__
typedef interface BuildManager BuildManager;
#endif 	/* __BuildManager_FWD_DEFINED__ */


#ifndef ___BuildManagerEvents_FWD_DEFINED__
#define ___BuildManagerEvents_FWD_DEFINED__
typedef interface _BuildManagerEvents _BuildManagerEvents;
#endif 	/* ___BuildManagerEvents_FWD_DEFINED__ */


#ifndef ___dispBuildManagerEvents_FWD_DEFINED__
#define ___dispBuildManagerEvents_FWD_DEFINED__
typedef interface _dispBuildManagerEvents _dispBuildManagerEvents;
#endif 	/* ___dispBuildManagerEvents_FWD_DEFINED__ */


#ifndef __BuildManagerEvents_FWD_DEFINED__
#define __BuildManagerEvents_FWD_DEFINED__

#ifdef __cplusplus
typedef class BuildManagerEvents BuildManagerEvents;
#else
typedef struct BuildManagerEvents BuildManagerEvents;
#endif /* __cplusplus */

#endif 	/* __BuildManagerEvents_FWD_DEFINED__ */


#ifndef __Imports_FWD_DEFINED__
#define __Imports_FWD_DEFINED__
typedef interface Imports Imports;
#endif 	/* __Imports_FWD_DEFINED__ */


#ifndef ___ImportsEvents_FWD_DEFINED__
#define ___ImportsEvents_FWD_DEFINED__
typedef interface _ImportsEvents _ImportsEvents;
#endif 	/* ___ImportsEvents_FWD_DEFINED__ */


#ifndef ___dispImportsEvents_FWD_DEFINED__
#define ___dispImportsEvents_FWD_DEFINED__
typedef interface _dispImportsEvents _dispImportsEvents;
#endif 	/* ___dispImportsEvents_FWD_DEFINED__ */


#ifndef __ImportsEvents_FWD_DEFINED__
#define __ImportsEvents_FWD_DEFINED__

#ifdef __cplusplus
typedef class ImportsEvents ImportsEvents;
#else
typedef struct ImportsEvents ImportsEvents;
#endif /* __cplusplus */

#endif 	/* __ImportsEvents_FWD_DEFINED__ */


#ifndef __VSProjectEvents_FWD_DEFINED__
#define __VSProjectEvents_FWD_DEFINED__
typedef interface VSProjectEvents VSProjectEvents;
#endif 	/* __VSProjectEvents_FWD_DEFINED__ */


#ifndef __VSProject_FWD_DEFINED__
#define __VSProject_FWD_DEFINED__
typedef interface VSProject VSProject;
#endif 	/* __VSProject_FWD_DEFINED__ */


#ifndef __VSProjectItem_FWD_DEFINED__
#define __VSProjectItem_FWD_DEFINED__
typedef interface VSProjectItem VSProjectItem;
#endif 	/* __VSProjectItem_FWD_DEFINED__ */


#ifndef __WebSettings_FWD_DEFINED__
#define __WebSettings_FWD_DEFINED__
typedef interface WebSettings WebSettings;
#endif 	/* __WebSettings_FWD_DEFINED__ */


#ifndef __IVSWebReferenceDynamicProperties_FWD_DEFINED__
#define __IVSWebReferenceDynamicProperties_FWD_DEFINED__
typedef interface IVSWebReferenceDynamicProperties IVSWebReferenceDynamicProperties;
#endif 	/* __IVSWebReferenceDynamicProperties_FWD_DEFINED__ */


/* header files for imported files */
#include "oaidl.h"

#ifdef __cplusplus
extern "C"{
#endif 

void * __RPC_USER MIDL_user_allocate(size_t);
void __RPC_USER MIDL_user_free( void * ); 

/* interface __MIDL_itf_vslangproj_0000 */
/* [local] */ 

#include "dte.h"
#ifdef FORCE_EXPLICIT_DTE_NAMESPACE
#define DTE VxDTE::DTE
#define Project VxDTE::Project
#define ProjectItem VxDTE::ProjectItem
#endif

#define VBFileProperties FileProperties
#define VBFolderProperties FolderProperties
#define VBProjectProperties ProjectProperties
#define VBProjectConfigProperties ProjectConfigurationProperties
#define IID_VBFileProperties IID_FileProperties
#define IID_VBFolderProperties IID_FolderProperties
#define IID_VBProjectProperties IID_ProjectProperties
#define IID_VBProjectConfigProperties IID_ProjectConfigurationProperties
extern const __declspec(selectany) GUID CATID_VBCodeFunction = {
    0xc28e28ca,
    0xe6dc,
    0x446f,
    {0xbe, 0x1a, 0xd4, 0x96, 0xbe, 0xf8, 0x34, 0x0}
  };
extern const __declspec(selectany) GUID CATID_VBCodeClass = {
    0xc28e28ca,
    0xe6dc,
    0x446f,
    {0xbe, 0x1a, 0xd4, 0x96, 0xbe, 0xf8, 0x34, 0x1}
  };
extern const __declspec(selectany) GUID CATID_VBCodeDelegate = {
    0xc28e28ca,
    0xe6dc,
    0x446f,
    {0xbe, 0x1a, 0xd4, 0x96, 0xbe, 0xf8, 0x34, 0x2}
  };
extern const __declspec(selectany) GUID CATID_VBCodeVariable = {
    0xc28e28ca,
    0xe6dc,
    0x446f,
    {0xbe, 0x1a, 0xd4, 0x96, 0xbe, 0xf8, 0x34, 0x3}
  };
extern const __declspec(selectany) GUID CATID_VBCodeProperty = {
    0xc28e28ca,
    0xe6dc,
    0x446f,
    {0xbe, 0x1a, 0xd4, 0x96, 0xbe, 0xf8, 0x34, 0x4}
  };
extern const __declspec(selectany) GUID CATID_VBCodeParameter = {
    0xc28e28ca,
    0xe6dc,
    0x446f,
    {0xbe, 0x1a, 0xd4, 0x96, 0xbe, 0xf8, 0x34, 0x5}
  };
extern const __declspec(selectany) GUID CATID_VBCodeInterface = {
    0xc28e28ca,
    0xe6dc,
    0x446f,
    {0xbe, 0x1a, 0xd4, 0x96, 0xbe, 0xf8, 0x34, 0x6}
  };
extern const __declspec(selectany) GUID CATID_VBCodeStruct = {
    0xc28e28ca,
    0xe6dc,
    0x446f,
    {0xbe, 0x1a, 0xd4, 0x96, 0xbe, 0xf8, 0x34, 0x7}
  };
extern const __declspec(selectany) GUID CATID_VBCodeEnum = {
    0xc28e28ca,
    0xe6dc,
    0x446f,
    {0xbe, 0x1a, 0xd4, 0x96, 0xbe, 0xf8, 0x34, 0x8}
  };
extern const __declspec(selectany) GUID CATID_VBCodeNamespace = {
    0xc28e28ca,
    0xe6dc,
    0x446f,
    {0xbe, 0x1a, 0xd4, 0x96, 0xbe, 0xf8, 0x34, 0x9}
  };
extern const __declspec(selectany) GUID CATID_VBCodeAttribute = {
    0xc28e28ca,
    0xe6dc,
    0x446f,
    {0xbe, 0x1a, 0xd4, 0x96, 0xbe, 0xf8, 0x34, 0xa}
  };

enum __MIDL___MIDL_itf_vslangproj_0000_0001
    {	VBPROJPROPID__First	= 10000,
	VBPROJPROPID_DebugSymbols	= VBPROJPROPID__First + 1,
	VBPROJPROPID_StartArguments	= VBPROJPROPID_DebugSymbols + 1,
	VBPROJPROPID_StartAction	= VBPROJPROPID_StartArguments + 1,
	VBPROJPROPID_OutputPath	= VBPROJPROPID_StartAction + 1,
	VBPROJPROPID_DefineConstants	= VBPROJPROPID_OutputPath + 1,
	VBPROJPROPID_StartProgram	= VBPROJPROPID_DefineConstants + 1,
	VBPROJPROPID_StartWorkingDirectory	= VBPROJPROPID_StartProgram + 1,
	VBPROJPROPID_StartURL	= VBPROJPROPID_StartWorkingDirectory + 1,
	VBPROJPROPID_OutputFileName	= VBPROJPROPID_StartURL + 1,
	VBPROJPROPID_IntermediatePath	= VBPROJPROPID_OutputFileName + 1,
	VBPROJPROPID_ApplicationIcon	= VBPROJPROPID_IntermediatePath + 1,
	VBPROJPROPID_WebServer	= VBPROJPROPID_ApplicationIcon + 1,
	VBPROJPROPID_AssemblyName	= VBPROJPROPID_WebServer + 1,
	VBPROJPROPID_Unused1	= VBPROJPROPID_AssemblyName + 1,
	VBPROJPROPID_StartupObject	= VBPROJPROPID_Unused1 + 1,
	VBPROJPROPID_OutputType	= VBPROJPROPID_StartupObject + 1,
	VBPROJPROPID_WebServerVersion	= VBPROJPROPID_OutputType + 1,
	VBPROJPROPID_ServerExtensionsVersion	= VBPROJPROPID_WebServerVersion + 1,
	VBPROJPROPID_LinkRepair	= VBPROJPROPID_ServerExtensionsVersion + 1,
	VBPROJPROPID_OfflineURL	= VBPROJPROPID_LinkRepair + 1,
	VBPROJPROPID_DefaultClientScript	= VBPROJPROPID_OfflineURL + 1,
	VBPROJPROPID_DefaultTargetSchema	= VBPROJPROPID_DefaultClientScript + 1,
	VBPROJPROPID_DefaultHTMLPageLayout	= VBPROJPROPID_DefaultTargetSchema + 1,
	VBPROJPROPID_ProjectFolder	= VBPROJPROPID_DefaultHTMLPageLayout + 1,
	VBPROJPROPID_ProjectURL	= VBPROJPROPID_ProjectFolder + 1,
	VBPROJPROPID_FileName	= VBPROJPROPID_ProjectURL + 1,
	VBPROJPROPID_FullPath	= VBPROJPROPID_FileName + 1,
	VBPROJPROPID_LocalPath	= VBPROJPROPID_FullPath + 1,
	VBPROJPROPID_URL	= VBPROJPROPID_LocalPath + 1,
	VBPROJPROPID_Extender	= VBPROJPROPID_URL + 1,
	VBPROJPROPID_ExtenderNames	= VBPROJPROPID_Extender + 1,
	VBPROJPROPID_ExtenderCATID	= VBPROJPROPID_ExtenderNames + 1,
	VBPROJPROPID_ActiveConfigurationSettings	= VBPROJPROPID_ExtenderCATID + 1,
	VBPROJPROPID_AbsoluteProjectDirectory	= VBPROJPROPID_ActiveConfigurationSettings + 1,
	VBPROJPROPID__Project	= VBPROJPROPID_AbsoluteProjectDirectory + 1,
	VBPROJPROPID_DefineDebug	= VBPROJPROPID__Project + 1,
	VBPROJPROPID_DefineTrace	= VBPROJPROPID_DefineDebug + 1,
	VBPROJPROPID_StartPage	= VBPROJPROPID_DefineTrace + 1,
	VBPROJPROPID_StartWithIE	= VBPROJPROPID_StartPage + 1,
	VBPROJPROPID_EnableASPDebugging	= VBPROJPROPID_StartWithIE + 1,
	VBPROJPROPID_EnableASPXDebugging	= VBPROJPROPID_EnableASPDebugging + 1,
	VBPROJPROPID_RootNamespace	= VBPROJPROPID_EnableASPXDebugging + 1,
	VBPROJPROPID_AssemblyOriginatorKeyMode	= VBPROJPROPID_RootNamespace + 1,
	VBPROJPROPID_AssemblyOriginatorKeyFile	= VBPROJPROPID_AssemblyOriginatorKeyMode + 1,
	VBPROJPROPID_AssemblyKeyContainerName	= VBPROJPROPID_AssemblyOriginatorKeyFile + 1,
	VBPROJPROPID_DelaySign	= VBPROJPROPID_AssemblyKeyContainerName + 1,
	VBPROJPROPID_FileSharePath	= VBPROJPROPID_DelaySign + 1,
	VBPROJPROPID_ActiveFileSharePath	= VBPROJPROPID_FileSharePath + 1,
	VBPROJPROPID_WebAccessMethod	= VBPROJPROPID_ActiveFileSharePath + 1,
	VBPROJPROPID_ActiveWebAccessMethod	= VBPROJPROPID_WebAccessMethod + 1,
	VBPROJPROPID_OptionStrict	= VBPROJPROPID_ActiveWebAccessMethod + 1,
	VBPROJPROPID_WarningLevel	= VBPROJPROPID_OptionStrict + 1,
	VBPROJPROPID_TreatWarningsAsErrors	= VBPROJPROPID_WarningLevel + 1,
	VBPROJPROPID_RemoveIntegerChecks	= VBPROJPROPID_TreatWarningsAsErrors + 1,
	VBPROJPROPID_BaseAddress	= VBPROJPROPID_RemoveIntegerChecks + 1,
	VBPROJPROPID_ReferencePath	= VBPROJPROPID_BaseAddress + 1,
	VBPROJPROPID_EnableUnmanagedDebugging	= VBPROJPROPID_ReferencePath + 1,
	VBPROJPROPID_EnableSQLServerDebugging	= VBPROJPROPID_EnableUnmanagedDebugging + 1,
	VBPROJPROPID_OptionExplicit	= VBPROJPROPID_EnableSQLServerDebugging + 1,
	VBPROJPROPID_OptionCompare	= VBPROJPROPID_OptionExplicit + 1,
	VBPROJPROPID_AllowUnsafeBlocks	= VBPROJPROPID_OptionCompare + 1,
	VBPROJPROPID_CheckForOverflowUnderflow	= VBPROJPROPID_AllowUnsafeBlocks + 1,
	VBPROJPROPID_DocumentationFile	= VBPROJPROPID_CheckForOverflowUnderflow + 1,
	VBPROJPROPID_Optimize	= VBPROJPROPID_DocumentationFile + 1,
	VBPROJPROPID_IncrementalBuild	= VBPROJPROPID_Optimize + 1,
	VBPROJPROPID_NoStandardLibraries	= VBPROJPROPID_IncrementalBuild + 1,
	VBPROJPROPID_ProjectType	= VBPROJPROPID_NoStandardLibraries + 1,
	VBPROJPROPID_DefaultNamespace	= VBPROJPROPID_ProjectType + 1,
	VBPROJPROPID_FileAlignment	= VBPROJPROPID_DefaultNamespace + 1,
	VBPROJPROPID_DisableWarnings	= VBPROJPROPID_FileAlignment + 1,
	VBAPROJPROPID_ProjectName	= VBPROJPROPID_DisableWarnings + 1,
	VBPROJPROPID_RegisterForComInterop	= VBAPROJPROPID_ProjectName + 1,
	VBPROJPROPID_ConfigurationOverrideFile	= VBPROJPROPID_RegisterForComInterop + 1,
	VBPROJPROPID_RemoteDebugEnabled	= VBPROJPROPID_ConfigurationOverrideFile + 1,
	VBPROJPROPID_RemoteDebugMachine	= VBPROJPROPID_RemoteDebugEnabled + 1
    } ;

enum __MIDL___MIDL_itf_vslangproj_0000_0002
    {	DISPID_VBFile_FileName	= DISPID_VALUE,
	DISPID_VBFile_Extension	= DISPID_VBFile_FileName + 1,
	DISPID_VBFile_Filesize	= DISPID_VBFile_Extension + 1,
	DISPID_VBFile_LocalPath	= DISPID_VBFile_Filesize + 1,
	DISPID_VBFile_FullPath	= DISPID_VBFile_LocalPath + 1,
	DISPID_VBFile_URL	= DISPID_VBFile_FullPath + 1,
	DISPID_VBFile_HTMLTitle	= DISPID_VBFile_URL + 1,
	DISPID_VBFile_Author	= DISPID_VBFile_HTMLTitle + 1,
	DISPID_VBFile_DateCreated	= DISPID_VBFile_Author + 1,
	DISPID_VBFile_DateModified	= DISPID_VBFile_DateCreated + 1,
	DISPID_VBFile_ModifiedBy	= DISPID_VBFile_DateModified + 1,
	DISPID_VBFile_SubType	= DISPID_VBFile_ModifiedBy + 1,
	DISPID_VBFile_Extender	= DISPID_VBFile_SubType + 1,
	DISPID_VBFile_ExtenderNames	= DISPID_VBFile_Extender + 1,
	DISPID_VBFile_ExtenderCATID	= DISPID_VBFile_ExtenderNames + 1,
	DISPID_VBFile_BuildAction	= DISPID_VBFile_ExtenderCATID + 1,
	DISPID_VBFile_CustomTool	= DISPID_VBFile_BuildAction + 1,
	DISPID_VBFile_CustomToolNamespace	= DISPID_VBFile_CustomTool + 1,
	DISPID_VBFile_CustomToolOutput	= DISPID_VBFile_CustomToolNamespace + 1,
	DISPID_VBFile_IsCustomToolOutput	= DISPID_VBFile_CustomToolOutput + 1,
	DISPID_VBFile_IsDependentFile	= DISPID_VBFile_IsCustomToolOutput + 1,
	DISPID_VBFile_IsLink	= DISPID_VBFile_IsDependentFile + 1,
	DISPID_VBFile_IsDesignTimeBuildInput	= DISPID_VBFile_IsLink + 1
    } ;

enum __MIDL___MIDL_itf_vslangproj_0000_0003
    {	DISPID_VBFolder_FileName	= 1,
	DISPID_VBFolder_LocalPath	= DISPID_VBFolder_FileName + 1,
	DISPID_VBFolder_FullPath	= DISPID_VBFolder_LocalPath + 1,
	DISPID_VBFolder_URL	= DISPID_VBFolder_FullPath + 1,
	DISPID_VBFolder_Extender	= DISPID_VBFolder_URL + 1,
	DISPID_VBFolder_ExtenderNames	= DISPID_VBFolder_Extender + 1,
	DISPID_VBFolder_ExtenderCATID	= DISPID_VBFolder_ExtenderNames + 1,
	DISPID_VBFolder_WebReference	= DISPID_VBFolder_ExtenderCATID + 1,
	DISPID_VBFolder_DefaultNamespace	= DISPID_VBFolder_WebReference + 1,
	DISPID_VBFolder_UrlBehavior	= DISPID_VBFolder_DefaultNamespace + 1
    } ;

enum __MIDL___MIDL_itf_vslangproj_0000_0004
    {	DISPID_Reference_DTE	= 1,
	DISPID_Reference_Collection	= DISPID_Reference_DTE + 1,
	DISPID_Reference_ContainingProject	= DISPID_Reference_Collection + 1,
	DISPID_Reference_Remove	= DISPID_Reference_ContainingProject + 1,
	DISPID_Reference_Name	= DISPID_Reference_Remove + 1,
	DISPID_Reference_Type	= DISPID_Reference_Name + 1,
	DISPID_Reference_Identity	= DISPID_Reference_Type + 1,
	DISPID_Reference_Path	= DISPID_Reference_Identity + 1,
	DISPID_Reference_Description	= DISPID_Reference_Path + 1,
	DISPID_Reference_Culture	= DISPID_Reference_Description + 1,
	DISPID_Reference_MajorVersion	= DISPID_Reference_Culture + 1,
	DISPID_Reference_MinorVersion	= DISPID_Reference_MajorVersion + 1,
	DISPID_Reference_RevisionNumber	= DISPID_Reference_MinorVersion + 1,
	DISPID_Reference_BuildNumber	= DISPID_Reference_RevisionNumber + 1,
	DISPID_Reference_StrongName	= DISPID_Reference_BuildNumber + 1,
	DISPID_Reference_SourceProject	= DISPID_Reference_StrongName + 1,
	DISPID_Reference_CopyLocal	= DISPID_Reference_SourceProject + 1,
	DISPID_Reference_Extender	= DISPID_Reference_CopyLocal + 1,
	DISPID_Reference_ExtenderNames	= DISPID_Reference_Extender + 1,
	DISPID_Reference_ExtenderCATID	= DISPID_Reference_ExtenderNames + 1,
	DISPID_Reference_PublicKeyToken	= DISPID_Reference_ExtenderCATID + 1,
	DISPID_Reference_Version	= DISPID_Reference_PublicKeyToken + 1
    } ;
#define prjOutputTypeUnknown  ((prjOutputType)-1) 
typedef 
enum tagProjectReferencesEvent
    {	RefsEvt_RefAdded	= 1,
	RefsEvt_RefRemoved	= RefsEvt_RefAdded + 1,
	RefsEvt_RefChanged	= RefsEvt_RefRemoved + 1
    } 	ProjectReferencesEvent;

DEFINE_GUID(CLSID_CVsExtProjectReferences,        0x89c537a6, 0xad15, 0x4a0f, 0xaa, 0x56, 0xe8, 0x17, 0x54, 0xd3, 0xf3, 0xa4);
#define VSLANGPROJ_VER_MAJ    7
#define VSLANGPROJ_VER_MIN    0


extern RPC_IF_HANDLE __MIDL_itf_vslangproj_0000_v0_0_c_ifspec;
extern RPC_IF_HANDLE __MIDL_itf_vslangproj_0000_v0_0_s_ifspec;


#ifndef __VSLangProj_LIBRARY_DEFINED__
#define __VSLangProj_LIBRARY_DEFINED__

/* library VSLangProj */
/* [version][helpstring][uuid] */ 

// CATID's for automation extension of extensibility objects 
// These are not diff for diff packages since the user can determine 
// the pkg from Project.Kind.
DEFINE_GUID(CATID_ExtPrj,            0x610d4614, 0xd0d5, 0x11d2, 0x85, 0x99, 0x00, 0x60, 0x97, 0xc6, 0x8e, 0x81);
DEFINE_GUID(CATID_ExtPrjItem,        0x610d4615, 0xd0d5, 0x11d2, 0x85, 0x99, 0x00, 0x60, 0x97, 0xc6, 0x8e, 0x81);
// CATID's for automation extension of browse objects
DEFINE_GUID(CATID_VBPrjProps,        0xe0fdc879, 0xc32a, 0x4751, 0xa3, 0xd3, 0xb, 0x38, 0x24, 0xbd, 0x57, 0x5f);
DEFINE_GUID(CATID_VBFileProps,        0xea5bd05d, 0x3c72, 0x40a5, 0x95, 0xa0, 0x28, 0xa2, 0x77, 0x33, 0x11, 0xca);
DEFINE_GUID(CATID_VBFolderProps,        0x932dc619, 0x2eaa, 0x4192, 0xb7, 0xe6, 0x3d, 0x15, 0xad, 0x31, 0xdf, 0x49);
DEFINE_GUID(CATID_VBRefProps,        0x2289b812, 0x8191, 0x4e81, 0xb7, 0xb3, 0x17, 0x40, 0x45, 0xab, 0xc, 0xb5);
DEFINE_GUID(CATID_CSharpFileProps,    0x8d58e6af, 0xed4e, 0x48b0, 0x8c, 0x7b, 0xc7, 0x4e, 0xf0, 0x73, 0x54, 0x51);
DEFINE_GUID(CATID_CSharpFolderProps,    0x914fe278, 0x54a, 0x45db, 0xbf, 0x9e, 0x5f, 0x22, 0x48, 0x4c, 0xc8, 0x4c);
DEFINE_GUID(CATID_VBAFileProps,        0xac2912b2, 0x50ed, 0x4e62, 0x8d, 0xff, 0x42, 0x9b, 0x4b, 0x88, 0xfc, 0x9e);
DEFINE_GUID(CATID_VBAFolderProps,    0x79231b36, 0x6213, 0x481d, 0xaa, 0x7d, 0xf, 0x93, 0x1e, 0x8f, 0x2c, 0xf9);
// Enum values of project properties
typedef /* [uuid] */  DECLSPEC_UUID("504876A3-4B7D-4932-B1D7-E91129D4AEBF") 
enum prjStartAction
    {	prjStartActionProject	= 0,
	prjStartActionProgram	= prjStartActionProject + 1,
	prjStartActionURL	= prjStartActionProgram + 1,
	prjStartActionNone	= prjStartActionURL + 1
    } 	prjStartAction;

#define prjStartActionMin  prjStartActionProject
#define prjStartActionMax  prjStartActionNone
typedef /* [uuid] */  DECLSPEC_UUID("FB309311-8F09-41e7-8347-68F5A079592D") 
enum prjOutputType
    {	prjOutputTypeWinExe	= 0,
	prjOutputTypeExe	= prjOutputTypeWinExe + 1,
	prjOutputTypeLibrary	= prjOutputTypeExe + 1
    } 	prjOutputType;

#define prjOutputTypeMin prjOutputTypeWinExe
#define prjOutputTypeMax prjOutputTypeLibrary
typedef /* [uuid] */  DECLSPEC_UUID("D760C0E8-311E-45eb-B06C-033F8CC1E5EC") 
enum prjScriptLanguage
    {	prjScriptLanguageJScript	= 0,
	prjScriptLanguageVBScript	= 1
    } 	prjScriptLanguage;

#define prjScriptLanguageMin prjScriptLanguageJScript
#define prjScriptLanguageMax prjScriptLanguageVBScript
typedef /* [uuid] */  DECLSPEC_UUID("B743460B-B3A1-40dc-9A0C-19ECCB63650E") 
enum prjTargetSchema
    {	prjTargetSchemaIE32Nav30	= 0,
	prjTargetSchemaIE50	= prjTargetSchemaIE32Nav30 + 1,
	prjTargetSchemaNav40	= prjTargetSchemaIE50 + 1
    } 	prjTargetSchema;

#define prjTargetSchemaHTML32 prjTargetSchemaIE32Nav30
#define prjTargetSchemaHTML40 prjTargetSchemaIE50
#define prjTargetSchemaNetscape40 prjTargetSchemaNav40
#define prjTargetSchemaMin  prjTargetSchemaIE32Nav30
#define prjTargetSchemaMax  prjTargetSchemaNav40
typedef /* [uuid] */  DECLSPEC_UUID("8821C6DB-40B9-4584-B3F1-28336B36A23D") 
enum prjHTMLPageLayout
    {	prjHTMLPageLayoutFlow	= 0,
	prjHTMLPageLayoutGrid	= prjHTMLPageLayoutFlow + 1
    } 	prjHTMLPageLayout;

#define prjHTMLPageLayoutLinear prjHTMLPageLayoutFlow
#define prjHTMLPageLayoutMin  prjHTMLPageLayoutFlow
#define prjHTMLPageLayoutMax  prjHTMLPageLayoutGrid
typedef /* [uuid] */  DECLSPEC_UUID("02720598-3E01-4721-ADAF-E2937BD6C645") 
enum prjOriginatorKeyMode
    {	prjOriginatorKeyModeNone	= 0,
	prjOriginatorKeyModeFile	= prjOriginatorKeyModeNone + 1,
	prjOriginatorKeyModeContainer	= prjOriginatorKeyModeFile + 1
    } 	prjOriginatorKeyMode;

#define prjOriginatorKeyModeMin  prjOriginatorKeyModeNone
#define prjOriginatorKeyModeMax  prjOriginatorKeyModeContainer
typedef /* [uuid] */  DECLSPEC_UUID("88A4C7D7-2587-4cc3-ADAC-993896B5D094") 
enum prjWebAccessMethod
    {	prjWebAccessMethodFileShare	= 0,
	prjWebAccessMethodFrontPage	= prjWebAccessMethodFileShare + 1
    } 	prjWebAccessMethod;

#define prjWebAccessMethodMin  prjWebAccessMethodFileShare
#define prjWebAccessMethodMax  prjWebAccessMethodFrontPage
typedef /* [uuid] */  DECLSPEC_UUID("31DDDF62-1891-4870-8DF5-00D9028826DF") 
enum prjWarningLevel
    {	prjWarningLevel0	= 0,
	prjWarningLevel1	= prjWarningLevel0 + 1,
	prjWarningLevel2	= prjWarningLevel1 + 1,
	prjWarningLevel3	= prjWarningLevel2 + 1,
	prjWarningLevel4	= prjWarningLevel3 + 1
    } 	prjWarningLevel;

#define prjWarningLevelMin  prjWarningLevel0
#define prjWarningLevelMax  prjWarningLevel4
typedef /* [uuid] */  DECLSPEC_UUID("5B50016F-F7CC-4687-A1DA-3F234F7620EE") 
enum prjProjectType
    {	prjProjectTypeLocal	= 0,
	prjProjectTypeWeb	= prjProjectTypeLocal + 1
    } 	prjProjectType;

#define prjProjectTypeMin  prjProjectTypeLocal
#define prjProjectTypeMax  prjProjectTypeWeb
typedef /* [uuid] */  DECLSPEC_UUID("D399129C-23CC-4301-81B0-5A60DC52E67D") 
enum prjBuildAction
    {	prjBuildActionNone	= 0,
	prjBuildActionCompile	= 1,
	prjBuildActionContent	= 2,
	prjBuildActionEmbeddedResource	= 3
    } 	prjBuildAction;

#define prjBuildActionMin  prjBuildActionNone
#define prjBuildActionMax  prjBuildActionCustom
typedef /* [uuid] */  DECLSPEC_UUID("EBDA8DD4-E450-452a-9FF5-7970904DEA7F") 
enum prjCompare
    {	prjCompareBinary	= 0,
	prjCompareText	= prjCompareBinary + 1
    } 	prjCompare;

#define prjCompareMin  prjCompareBinary
#define prjCompareMax  prjCompareText
typedef /* [uuid] */  DECLSPEC_UUID("95DCFABC-145D-498d-A454-47F33D47139C") 
enum prjOptionExplicit
    {	prjOptionExplicitOff	= 0,
	prjOptionExplicitOn	= prjOptionExplicitOff + 1
    } 	prjOptionExplicit;

#define prjOptionExplicitMin  prjOptionExplicitOff
#define prjOptionExplicitMax  prjOptionExplicitOn
typedef /* [uuid] */  DECLSPEC_UUID("CE6AA0FD-6CCD-4601-A730-FA75219862C3") 
enum prjOptionStrict
    {	prjOptionStrictOff	= 0,
	prjOptionStrictOn	= prjOptionStrictOff + 1
    } 	prjOptionStrict;

#define prjOptionStrictMin  prjOptionStrictOff
#define prjOptionStrictMax  prjOptionStrictOn
typedef /* [uuid] */  DECLSPEC_UUID("1FE01DDF-C760-4307-8A40-0D023AEAFF07") 
enum webrefUrlBehavior
    {	webrefUrlBehaviorStatic	= 0,
	webrefUrlBehaviorDynamic	= webrefUrlBehaviorStatic + 1
    } 	webrefUrlBehavior;

typedef /* [uuid] */  DECLSPEC_UUID("B21668EF-29F1-425d-85D7-118CB838C362") 
enum prjReferenceType
    {	prjReferenceTypeAssembly	= 0,
	prjReferenceTypeActiveX	= prjReferenceTypeAssembly + 1
    } 	prjReferenceType;

typedef /* [uuid] */  DECLSPEC_UUID("5DE9F309-E701-44d1-8068-1860738C0084") 
enum prjCopyProjectOption
    {	prjRunFiles	= 0,
	prjProjectFiles	= prjRunFiles + 1,
	prjAllFiles	= prjProjectFiles + 1
    } 	prjCopyProjectOption;

typedef /* [hidden][uuid] */  DECLSPEC_UUID("F111445E-A9ED-4d12-9A0E-738F5F995F10") 
enum prjExecCommand
    {	prjExecCommandHandleInvalidStartupObject	= 0
    } 	prjExecCommand;

#define SID_SVSProjectItem IID_ProjectItem
typedef /* [uuid] */  DECLSPEC_UUID("316A5305-224A-4580-91EA-5C62AFEC07FF") 
enum tagWebPrjAuthoringAccess
    {	webPrjAuthoringAccess_FileShare	= 1,
	webPrjAuthoringAccess_FrontPage	= 2
    } 	webPrjAuthoringAccess;


EXTERN_C const IID LIBID_VSLangProj;


#ifndef __PrjKind_MODULE_DEFINED__
#define __PrjKind_MODULE_DEFINED__


/* module PrjKind */
/* [uuid] */ 

/* [helpstring] */ const LPSTR prjKindVBProject	=	"{F184B08F-C81C-45F6-A57F-5ABD9991F28F}";

/* [helpstring] */ const LPSTR prjKindCSharpProject	=	"{FAE04EC0-301F-11D3-BF4B-00C04F79EFBC}";

/* [helpstring] */ const LPSTR prjKindVSAProject	=	"{13B7A3EE-4614-11D3-9BC7-00C04F79DE25}";

/* [helpstring] */ const LPSTR prjKindVenusProject	=	"{E24C65DC-7377-472b-9ABA-BC803B73C61A}";

#endif /* __PrjKind_MODULE_DEFINED__ */


#ifndef __PrjCATID_MODULE_DEFINED__
#define __PrjCATID_MODULE_DEFINED__


/* module PrjCATID */
/* [uuid] */ 

/* [helpstring] */ const LPSTR prjCATIDProject	=	"{610D4614-D0D5-11D2-8599-006097C68E81}";

/* [helpstring] */ const LPSTR prjCATIDProjectItem	=	"{610D4615-D0D5-11D2-8599-006097C68E81}";

#endif /* __PrjCATID_MODULE_DEFINED__ */


#ifndef __PrjBrowseObjectCATID_MODULE_DEFINED__
#define __PrjBrowseObjectCATID_MODULE_DEFINED__


/* module PrjBrowseObjectCATID */
/* [uuid] */ 

/* [helpstring] */ const LPSTR prjCATIDVBProjectBrowseObject	=	"{E0FDC879-C32A-4751-A3D3-0B3824BD575F}";

/* [helpstring] */ const LPSTR prjCATIDCSharpProjectBrowseObject	=	"{4EF9F003-DE95-4d60-96B0-212979F2A857}";

/* [helpstring] */ const LPSTR prjCATIDVBProjectConfigBrowseObject	=	"{67F8DD11-14EB-489b-87F0-F01C52AF3870}";

/* [helpstring] */ const LPSTR prjCATIDCSharpProjectConfigBrowseObject	=	"{A12CE10A-227F-4963-ADB6-3A43388513CA}";

/* [helpstring] */ const LPSTR prjCATIDVBFileBrowseObject	=	"{EA5BD05D-3C72-40A5-95A0-28A2773311CA}";

/* [helpstring] */ const LPSTR prjCATIDCSharpFileBrowseObject	=	"{8D58E6AF-ED4E-48B0-8C7B-C74EF0735451}";

/* [helpstring] */ const LPSTR prjCATIDVSAFileBrowseObject	=	"{AC2912B2-50ED-4E62-8DFF-429B4B88FC9E}";

/* [helpstring] */ const LPSTR prjCATIDVBFolderBrowseObject	=	"{932DC619-2EAA-4192-B7E6-3D15AD31DF49}";

/* [helpstring] */ const LPSTR prjCATIDCSharpFolderBrowseObject	=	"{914FE278-054A-45DB-BF9E-5F22484CC84C}";

/* [helpstring] */ const LPSTR prjCATIDVSAFolderBrowseObject	=	"{79231B36-6213-481D-AA7D-0F931E8F2CF9}";

/* [helpstring] */ const LPSTR prjCATIDVBReferenceBrowseObject	=	"{2289B812-8191-4e81-B7B3-174045AB0CB5}";

/* [helpstring] */ const LPSTR prjCATIDCSharpReferenceBrowseObject	=	"{2F0FA3B8-C855-4a4e-95A5-CB45C67D6C27}";

/* [helpstring] */ const LPSTR prjCATIDVSAReferenceBrowseObject	=	"{4E018D0E-1143-47d6-A139-68D01E39BF5F}";

/* [helpstring] */ const LPSTR prjCATIDVBConfig	=	"{5A30A635-0BA6-468f-A1C6-952DA61DB00B}";

/* [helpstring] */ const LPSTR prjCATIDCSharpConfig	=	"{89FB23F7-E591-4a2f-8E0F-64C0522FCF77}";

/* [helpstring] */ const LPSTR prjCATIDVSAConfig	=	"{1AA19227-163B-42fd-87CC-F5E78DABF52B}";

#endif /* __PrjBrowseObjectCATID_MODULE_DEFINED__ */

#ifndef __ProjectConfigurationProperties_INTERFACE_DEFINED__
#define __ProjectConfigurationProperties_INTERFACE_DEFINED__

/* interface ProjectConfigurationProperties */
/* [object][dual][unique][helpstring][uuid] */ 


EXTERN_C const IID IID_ProjectConfigurationProperties;

#if defined(__cplusplus) && !defined(CINTERFACE)
    
    MIDL_INTERFACE("3CDAA65D-1E9D-11d3-B202-00C04F79CACB")
    ProjectConfigurationProperties : public IDispatch
    {
    public:
        virtual /* [hidden][helpstring][id][propget] */ HRESULT STDMETHODCALLTYPE get___id( 
            /* [retval][out] */ BSTR *pbstrName) = 0;
        
        virtual /* [helpstring][id][propget] */ HRESULT STDMETHODCALLTYPE get_DebugSymbols( 
            /* [retval][out] */ VARIANT_BOOL *pbGenerate) = 0;
        
        virtual /* [helpstring][id][propput] */ HRESULT STDMETHODCALLTYPE put_DebugSymbols( 
            /* [in] */ VARIANT_BOOL bGenerate) = 0;
        
        virtual /* [helpstring][id][propget] */ HRESULT STDMETHODCALLTYPE get_DefineDebug( 
            /* [retval][out] */ VARIANT_BOOL *pbDefineDebug) = 0;
        
        virtual /* [helpstring][id][propput] */ HRESULT STDMETHODCALLTYPE put_DefineDebug( 
            /* [in] */ VARIANT_BOOL bDefineDebug) = 0;
        
        virtual /* [helpstring][id][propget] */ HRESULT STDMETHODCALLTYPE get_DefineTrace( 
            /* [retval][out] */ VARIANT_BOOL *pbDefineTrace) = 0;
        
        virtual /* [helpstring][id][propput] */ HRESULT STDMETHODCALLTYPE put_DefineTrace( 
            /* [in] */ VARIANT_BOOL bDefineTrace) = 0;
        
        virtual /* [helpstring][id][propget] */ HRESULT STDMETHODCALLTYPE get_OutputPath( 
            /* [retval][out] */ BSTR *pbstrOutputPath) = 0;
        
        virtual /* [helpstring][id][propput] */ HRESULT STDMETHODCALLTYPE put_OutputPath( 
            /* [in] */ BSTR bstrOutputPath) = 0;
        
        virtual /* [helpstring][id][propget] */ HRESULT STDMETHODCALLTYPE get_IntermediatePath( 
            /* [retval][out] */ BSTR *pbstrIntermediatePath) = 0;
        
        virtual /* [hidden][helpstring][id][propput] */ HRESULT STDMETHODCALLTYPE put_IntermediatePath( 
            /* [in] */ BSTR bstrIntermediatePath) = 0;
        
        virtual /* [helpstring][id][propget] */ HRESULT STDMETHODCALLTYPE get_DefineConstants( 
            /* [retval][out] */ BSTR *pbstrDefineConstants) = 0;
        
        virtual /* [helpstring][id][propput] */ HRESULT STDMETHODCALLTYPE put_DefineConstants( 
            /* [in] */ BSTR bstrDefineConstants) = 0;
        
        virtual /* [helpstring][id][propget] */ HRESULT STDMETHODCALLTYPE get_RemoveIntegerChecks( 
            /* [retval][out] */ VARIANT_BOOL *pbRemoveIntegerChecks) = 0;
        
        virtual /* [helpstring][id][propput] */ HRESULT STDMETHODCALLTYPE put_RemoveIntegerChecks( 
            /* [in] */ VARIANT_BOOL bRemoveIntegerChecks) = 0;
        
        virtual /* [helpstring][id][propget] */ HRESULT STDMETHODCALLTYPE get_BaseAddress( 
            /* [retval][out] */ DWORD *pdwBaseAddress) = 0;
        
        virtual /* [helpstring][id][propput] */ HRESULT STDMETHODCALLTYPE put_BaseAddress( 
            /* [in] */ DWORD dwBaseAddress) = 0;
        
        virtual /* [helpstring][id][propget] */ HRESULT STDMETHODCALLTYPE get_AllowUnsafeBlocks( 
            /* [retval][out] */ VARIANT_BOOL *pbUnsafe) = 0;
        
        virtual /* [helpstring][id][propput] */ HRESULT STDMETHODCALLTYPE put_AllowUnsafeBlocks( 
            /* [in] */ VARIANT_BOOL bUnsafe) = 0;
        
        virtual /* [helpstring][id][propget] */ HRESULT STDMETHODCALLTYPE get_CheckForOverflowUnderflow( 
            /* [retval][out] */ VARIANT_BOOL *pbCheckForOverflowUnderflow) = 0;
        
        virtual /* [helpstring][id][propput] */ HRESULT STDMETHODCALLTYPE put_CheckForOverflowUnderflow( 
            /* [in] */ VARIANT_BOOL bCheckForOverflowUnderflow) = 0;
        
        virtual /* [helpstring][id][propget] */ HRESULT STDMETHODCALLTYPE get_DocumentationFile( 
            /* [retval][out] */ BSTR *pbstrDocumentationFile) = 0;
        
        virtual /* [helpstring][id][propput] */ HRESULT STDMETHODCALLTYPE put_DocumentationFile( 
            /* [in] */ BSTR bstrDocumentationFile) = 0;
        
        virtual /* [helpstring][id][propget] */ HRESULT STDMETHODCALLTYPE get_Optimize( 
            /* [retval][out] */ VARIANT_BOOL *pbOptimize) = 0;
        
        virtual /* [helpstring][id][propput] */ HRESULT STDMETHODCALLTYPE put_Optimize( 
            /* [in] */ VARIANT_BOOL bCheckForOverflowUnderflow) = 0;
        
        virtual /* [helpstring][id][propget] */ HRESULT STDMETHODCALLTYPE get_IncrementalBuild( 
            /* [retval][out] */ VARIANT_BOOL *pbIncrementalBuild) = 0;
        
        virtual /* [helpstring][id][propput] */ HRESULT STDMETHODCALLTYPE put_IncrementalBuild( 
            /* [in] */ VARIANT_BOOL bIncrementalBuild) = 0;
        
        virtual /* [helpstring][id][propget] */ HRESULT STDMETHODCALLTYPE get_StartProgram( 
            /* [retval][out] */ BSTR *pbstrStartProgram) = 0;
        
        virtual /* [helpstring][id][propput] */ HRESULT STDMETHODCALLTYPE put_StartProgram( 
            /* [in] */ BSTR bstrStartProgram) = 0;
        
        virtual /* [helpstring][id][propget] */ HRESULT STDMETHODCALLTYPE get_StartWorkingDirectory( 
            /* [retval][out] */ BSTR *pbstrStartWorkingDirectory) = 0;
        
        virtual /* [helpstring][id][propput] */ HRESULT STDMETHODCALLTYPE put_StartWorkingDirectory( 
            /* [in] */ BSTR bstrStartWorkingDirectory) = 0;
        
        virtual /* [helpstring][id][propget] */ HRESULT STDMETHODCALLTYPE get_StartURL( 
            /* [retval][out] */ BSTR *pbstrStartURL) = 0;
        
        virtual /* [helpstring][id][propput] */ HRESULT STDMETHODCALLTYPE put_StartURL( 
            /* [in] */ BSTR bstrStartURL) = 0;
        
        virtual /* [helpstring][id][propget] */ HRESULT STDMETHODCALLTYPE get_StartPage( 
            /* [retval][out] */ BSTR *pbstrStartPage) = 0;
        
        virtual /* [helpstring][id][propput] */ HRESULT STDMETHODCALLTYPE put_StartPage( 
            /* [in] */ BSTR bstrStartPage) = 0;
        
        virtual /* [helpstring][id][propget] */ HRESULT STDMETHODCALLTYPE get_StartArguments( 
            /* [retval][out] */ BSTR *pbstrStartArguments) = 0;
        
        virtual /* [helpstring][id][propput] */ HRESULT STDMETHODCALLTYPE put_StartArguments( 
            /* [in] */ BSTR bstrStartArguments) = 0;
        
        virtual /* [helpstring][id][propget] */ HRESULT STDMETHODCALLTYPE get_StartWithIE( 
            /* [retval][out] */ VARIANT_BOOL *pbStartWithIE) = 0;
        
        virtual /* [helpstring][id][propput] */ HRESULT STDMETHODCALLTYPE put_StartWithIE( 
            /* [in] */ VARIANT_BOOL bStartWithIE) = 0;
        
        virtual /* [helpstring][id][propget] */ HRESULT STDMETHODCALLTYPE get_EnableASPDebugging( 
            /* [retval][out] */ VARIANT_BOOL *pbEnableASPDebugging) = 0;
        
        virtual /* [helpstring][id][propput] */ HRESULT STDMETHODCALLTYPE put_EnableASPDebugging( 
            /* [in] */ VARIANT_BOOL bEnableASPDebugging) = 0;
        
        virtual /* [helpstring][id][propget] */ HRESULT STDMETHODCALLTYPE get_EnableASPXDebugging( 
            /* [retval][out] */ VARIANT_BOOL *pbEnableASPXDebugging) = 0;
        
        virtual /* [helpstring][id][propput] */ HRESULT STDMETHODCALLTYPE put_EnableASPXDebugging( 
            /* [in] */ VARIANT_BOOL bEnableASPXDebugging) = 0;
        
        virtual /* [helpstring][id][propget] */ HRESULT STDMETHODCALLTYPE get_EnableUnmanagedDebugging( 
            /* [retval][out] */ VARIANT_BOOL *pbEnableUnmanagedDebugging) = 0;
        
        virtual /* [helpstring][id][propput] */ HRESULT STDMETHODCALLTYPE put_EnableUnmanagedDebugging( 
            /* [in] */ VARIANT_BOOL bEnableUnmanagedDebugging) = 0;
        
        virtual /* [helpstring][id][propget] */ HRESULT STDMETHODCALLTYPE get_StartAction( 
            /* [retval][out] */ prjStartAction *pdebugStartMode) = 0;
        
        virtual /* [helpstring][id][propput] */ HRESULT STDMETHODCALLTYPE put_StartAction( 
            /* [in] */ prjStartAction debugStartMode) = 0;
        
        virtual /* [hidden][helpstring][nonbrowsable][id][propget] */ HRESULT STDMETHODCALLTYPE get_Extender( 
            /* [in] */ BSTR ExtenderName,
            /* [retval][out] */ IDispatch **Extender) = 0;
        
        virtual /* [hidden][helpstring][nonbrowsable][id][propget] */ HRESULT STDMETHODCALLTYPE get_ExtenderNames( 
            /* [retval][out] */ VARIANT *ExtenderNames) = 0;
        
        virtual /* [hidden][helpstring][nonbrowsable][id][propget] */ HRESULT STDMETHODCALLTYPE get_ExtenderCATID( 
            /* [retval][out] */ BSTR *pRetval) = 0;
        
        virtual /* [helpstring][id][propget] */ HRESULT STDMETHODCALLTYPE get_WarningLevel( 
            /* [retval][out] */ prjWarningLevel *pWarningLeve) = 0;
        
        virtual /* [helpstring][id][propput] */ HRESULT STDMETHODCALLTYPE put_WarningLevel( 
            /* [in] */ prjWarningLevel warningLevel) = 0;
        
        virtual /* [helpstring][id][propget] */ HRESULT STDMETHODCALLTYPE get_TreatWarningsAsErrors( 
            /* [retval][out] */ VARIANT_BOOL *pWarningAsError) = 0;
        
        virtual /* [helpstring][id][propput] */ HRESULT STDMETHODCALLTYPE put_TreatWarningsAsErrors( 
            /* [in] */ VARIANT_BOOL warningAsError) = 0;
        
        virtual /* [helpstring][id][propget] */ HRESULT STDMETHODCALLTYPE get_EnableSQLServerDebugging( 
            /* [retval][out] */ VARIANT_BOOL *pbEnableSQLServerDebugging) = 0;
        
        virtual /* [helpstring][id][propput] */ HRESULT STDMETHODCALLTYPE put_EnableSQLServerDebugging( 
            /* [in] */ VARIANT_BOOL bEnableSQLServerDebugging) = 0;
        
        virtual /* [helpstring][id][propget] */ HRESULT STDMETHODCALLTYPE get_FileAlignment( 
            /* [retval][out] */ DWORD *pdwFileAlignment) = 0;
        
        virtual /* [helpstring][id][propput] */ HRESULT STDMETHODCALLTYPE put_FileAlignment( 
            /* [in] */ DWORD dwFileAlignment) = 0;
        
        virtual /* [helpstring][id][propget] */ HRESULT STDMETHODCALLTYPE get_RegisterForComInterop( 
            /* [retval][out] */ VARIANT_BOOL *pVal) = 0;
        
        virtual /* [helpstring][id][propput] */ HRESULT STDMETHODCALLTYPE put_RegisterForComInterop( 
            /* [in] */ VARIANT_BOOL val) = 0;
        
        virtual /* [helpstring][id][propget] */ HRESULT STDMETHODCALLTYPE get_ConfigurationOverrideFile( 
            /* [retval][out] */ BSTR *pbstrConfigFile) = 0;
        
        virtual /* [helpstring][id][propput] */ HRESULT STDMETHODCALLTYPE put_ConfigurationOverrideFile( 
            /* [in] */ BSTR bstrConfigFile) = 0;
        
        virtual /* [helpstring][id][propget] */ HRESULT STDMETHODCALLTYPE get_RemoteDebugEnabled( 
            /* [retval][out] */ VARIANT_BOOL *pbEnableRemoteLaunch) = 0;
        
        virtual /* [helpstring][id][propput] */ HRESULT STDMETHODCALLTYPE put_RemoteDebugEnabled( 
            /* [in] */ VARIANT_BOOL bEnableRemoteLaunch) = 0;
        
        virtual /* [helpstring][id][propget] */ HRESULT STDMETHODCALLTYPE get_RemoteDebugMachine( 
            /* [retval][out] */ BSTR *pbstrRemoteLaunchMach) = 0;
        
        virtual /* [helpstring][id][propput] */ HRESULT STDMETHODCALLTYPE put_RemoteDebugMachine( 
            /* [in] */ BSTR bstrRemoteLaunchMach) = 0;
        
    };
    
#else 	/* C style interface */

    typedef struct ProjectConfigurationPropertiesVtbl
    {
        BEGIN_INTERFACE
        
        HRESULT ( STDMETHODCALLTYPE *QueryInterface )( 
            ProjectConfigurationProperties * This,
            /* [in] */ REFIID riid,
            /* [iid_is][out] */ void **ppvObject);
        
        ULONG ( STDMETHODCALLTYPE *AddRef )( 
            ProjectConfigurationProperties * This);
        
        ULONG ( STDMETHODCALLTYPE *Release )( 
            ProjectConfigurationProperties * This);
        
        HRESULT ( STDMETHODCALLTYPE *GetTypeInfoCount )( 
            ProjectConfigurationProperties * This,
            /* [out] */ UINT *pctinfo);
        
        HRESULT ( STDMETHODCALLTYPE *GetTypeInfo )( 
            ProjectConfigurationProperties * This,
            /* [in] */ UINT iTInfo,
            /* [in] */ LCID lcid,
            /* [out] */ ITypeInfo **ppTInfo);
        
        HRESULT ( STDMETHODCALLTYPE *GetIDsOfNames )( 
            ProjectConfigurationProperties * This,
            /* [in] */ REFIID riid,
            /* [size_is][in] */ LPOLESTR *rgszNames,
            /* [in] */ UINT cNames,
            /* [in] */ LCID lcid,
            /* [size_is][out] */ DISPID *rgDispId);
        
        /* [local] */ HRESULT ( STDMETHODCALLTYPE *Invoke )( 
            ProjectConfigurationProperties * This,
            /* [in] */ DISPID dispIdMember,
            /* [in] */ REFIID riid,
            /* [in] */ LCID lcid,
            /* [in] */ WORD wFlags,
            /* [out][in] */ DISPPARAMS *pDispParams,
            /* [out] */ VARIANT *pVarResult,
            /* [out] */ EXCEPINFO *pExcepInfo,
            /* [out] */ UINT *puArgErr);
        
        /* [hidden][helpstring][id][propget] */ HRESULT ( STDMETHODCALLTYPE *get___id )( 
            ProjectConfigurationProperties * This,
            /* [retval][out] */ BSTR *pbstrName);
        
        /* [helpstring][id][propget] */ HRESULT ( STDMETHODCALLTYPE *get_DebugSymbols )( 
            ProjectConfigurationProperties * This,
            /* [retval][out] */ VARIANT_BOOL *pbGenerate);
        
        /* [helpstring][id][propput] */ HRESULT ( STDMETHODCALLTYPE *put_DebugSymbols )( 
            ProjectConfigurationProperties * This,
            /* [in] */ VARIANT_BOOL bGenerate);
        
        /* [helpstring][id][propget] */ HRESULT ( STDMETHODCALLTYPE *get_DefineDebug )( 
            ProjectConfigurationProperties * This,
            /* [retval][out] */ VARIANT_BOOL *pbDefineDebug);
        
        /* [helpstring][id][propput] */ HRESULT ( STDMETHODCALLTYPE *put_DefineDebug )( 
            ProjectConfigurationProperties * This,
            /* [in] */ VARIANT_BOOL bDefineDebug);
        
        /* [helpstring][id][propget] */ HRESULT ( STDMETHODCALLTYPE *get_DefineTrace )( 
            ProjectConfigurationProperties * This,
            /* [retval][out] */ VARIANT_BOOL *pbDefineTrace);
        
        /* [helpstring][id][propput] */ HRESULT ( STDMETHODCALLTYPE *put_DefineTrace )( 
            ProjectConfigurationProperties * This,
            /* [in] */ VARIANT_BOOL bDefineTrace);
        
        /* [helpstring][id][propget] */ HRESULT ( STDMETHODCALLTYPE *get_OutputPath )( 
            ProjectConfigurationProperties * This,
            /* [retval][out] */ BSTR *pbstrOutputPath);
        
        /* [helpstring][id][propput] */ HRESULT ( STDMETHODCALLTYPE *put_OutputPath )( 
            ProjectConfigurationProperties * This,
            /* [in] */ BSTR bstrOutputPath);
        
        /* [helpstring][id][propget] */ HRESULT ( STDMETHODCALLTYPE *get_IntermediatePath )( 
            ProjectConfigurationProperties * This,
            /* [retval][out] */ BSTR *pbstrIntermediatePath);
        
        /* [hidden][helpstring][id][propput] */ HRESULT ( STDMETHODCALLTYPE *put_IntermediatePath )( 
            ProjectConfigurationProperties * This,
            /* [in] */ BSTR bstrIntermediatePath);
        
        /* [helpstring][id][propget] */ HRESULT ( STDMETHODCALLTYPE *get_DefineConstants )( 
            ProjectConfigurationProperties * This,
            /* [retval][out] */ BSTR *pbstrDefineConstants);
        
        /* [helpstring][id][propput] */ HRESULT ( STDMETHODCALLTYPE *put_DefineConstants )( 
            ProjectConfigurationProperties * This,
            /* [in] */ BSTR bstrDefineConstants);
        
        /* [helpstring][id][propget] */ HRESULT ( STDMETHODCALLTYPE *get_RemoveIntegerChecks )( 
            ProjectConfigurationProperties * This,
            /* [retval][out] */ VARIANT_BOOL *pbRemoveIntegerChecks);
        
        /* [helpstring][id][propput] */ HRESULT ( STDMETHODCALLTYPE *put_RemoveIntegerChecks )( 
            ProjectConfigurationProperties * This,
            /* [in] */ VARIANT_BOOL bRemoveIntegerChecks);
        
        /* [helpstring][id][propget] */ HRESULT ( STDMETHODCALLTYPE *get_BaseAddress )( 
            ProjectConfigurationProperties * This,
            /* [retval][out] */ DWORD *pdwBaseAddress);
        
        /* [helpstring][id][propput] */ HRESULT ( STDMETHODCALLTYPE *put_BaseAddress )( 
            ProjectConfigurationProperties * This,
            /* [in] */ DWORD dwBaseAddress);
        
        /* [helpstring][id][propget] */ HRESULT ( STDMETHODCALLTYPE *get_AllowUnsafeBlocks )( 
            ProjectConfigurationProperties * This,
            /* [retval][out] */ VARIANT_BOOL *pbUnsafe);
        
        /* [helpstring][id][propput] */ HRESULT ( STDMETHODCALLTYPE *put_AllowUnsafeBlocks )( 
            ProjectConfigurationProperties * This,
            /* [in] */ VARIANT_BOOL bUnsafe);
        
        /* [helpstring][id][propget] */ HRESULT ( STDMETHODCALLTYPE *get_CheckForOverflowUnderflow )( 
            ProjectConfigurationProperties * This,
            /* [retval][out] */ VARIANT_BOOL *pbCheckForOverflowUnderflow);
        
        /* [helpstring][id][propput] */ HRESULT ( STDMETHODCALLTYPE *put_CheckForOverflowUnderflow )( 
            ProjectConfigurationProperties * This,
            /* [in] */ VARIANT_BOOL bCheckForOverflowUnderflow);
        
        /* [helpstring][id][propget] */ HRESULT ( STDMETHODCALLTYPE *get_DocumentationFile )( 
            ProjectConfigurationProperties * This,
            /* [retval][out] */ BSTR *pbstrDocumentationFile);
        
        /* [helpstring][id][propput] */ HRESULT ( STDMETHODCALLTYPE *put_DocumentationFile )( 
            ProjectConfigurationProperties * This,
            /* [in] */ BSTR bstrDocumentationFile);
        
        /* [helpstring][id][propget] */ HRESULT ( STDMETHODCALLTYPE *get_Optimize )( 
            ProjectConfigurationProperties * This,
            /* [retval][out] */ VARIANT_BOOL *pbOptimize);
        
        /* [helpstring][id][propput] */ HRESULT ( STDMETHODCALLTYPE *put_Optimize )( 
            ProjectConfigurationProperties * This,
            /* [in] */ VARIANT_BOOL bCheckForOverflowUnderflow);
        
        /* [helpstring][id][propget] */ HRESULT ( STDMETHODCALLTYPE *get_IncrementalBuild )( 
            ProjectConfigurationProperties * This,
            /* [retval][out] */ VARIANT_BOOL *pbIncrementalBuild);
        
        /* [helpstring][id][propput] */ HRESULT ( STDMETHODCALLTYPE *put_IncrementalBuild )( 
            ProjectConfigurationProperties * This,
            /* [in] */ VARIANT_BOOL bIncrementalBuild);
        
        /* [helpstring][id][propget] */ HRESULT ( STDMETHODCALLTYPE *get_StartProgram )( 
            ProjectConfigurationProperties * This,
            /* [retval][out] */ BSTR *pbstrStartProgram);
        
        /* [helpstring][id][propput] */ HRESULT ( STDMETHODCALLTYPE *put_StartProgram )( 
            ProjectConfigurationProperties * This,
            /* [in] */ BSTR bstrStartProgram);
        
        /* [helpstring][id][propget] */ HRESULT ( STDMETHODCALLTYPE *get_StartWorkingDirectory )( 
            ProjectConfigurationProperties * This,
            /* [retval][out] */ BSTR *pbstrStartWorkingDirectory);
        
        /* [helpstring][id][propput] */ HRESULT ( STDMETHODCALLTYPE *put_StartWorkingDirectory )( 
            ProjectConfigurationProperties * This,
            /* [in] */ BSTR bstrStartWorkingDirectory);
        
        /* [helpstring][id][propget] */ HRESULT ( STDMETHODCALLTYPE *get_StartURL )( 
            ProjectConfigurationProperties * This,
            /* [retval][out] */ BSTR *pbstrStartURL);
        
        /* [helpstring][id][propput] */ HRESULT ( STDMETHODCALLTYPE *put_StartURL )( 
            ProjectConfigurationProperties * This,
            /* [in] */ BSTR bstrStartURL);
        
        /* [helpstring][id][propget] */ HRESULT ( STDMETHODCALLTYPE *get_StartPage )( 
            ProjectConfigurationProperties * This,
            /* [retval][out] */ BSTR *pbstrStartPage);
        
        /* [helpstring][id][propput] */ HRESULT ( STDMETHODCALLTYPE *put_StartPage )( 
            ProjectConfigurationProperties * This,
            /* [in] */ BSTR bstrStartPage);
        
        /* [helpstring][id][propget] */ HRESULT ( STDMETHODCALLTYPE *get_StartArguments )( 
            ProjectConfigurationProperties * This,
            /* [retval][out] */ BSTR *pbstrStartArguments);
        
        /* [helpstring][id][propput] */ HRESULT ( STDMETHODCALLTYPE *put_StartArguments )( 
            ProjectConfigurationProperties * This,
            /* [in] */ BSTR bstrStartArguments);
        
        /* [helpstring][id][propget] */ HRESULT ( STDMETHODCALLTYPE *get_StartWithIE )( 
            ProjectConfigurationProperties * This,
            /* [retval][out] */ VARIANT_BOOL *pbStartWithIE);
        
        /* [helpstring][id][propput] */ HRESULT ( STDMETHODCALLTYPE *put_StartWithIE )( 
            ProjectConfigurationProperties * This,
            /* [in] */ VARIANT_BOOL bStartWithIE);
        
        /* [helpstring][id][propget] */ HRESULT ( STDMETHODCALLTYPE *get_EnableASPDebugging )( 
            ProjectConfigurationProperties * This,
            /* [retval][out] */ VARIANT_BOOL *pbEnableASPDebugging);
        
        /* [helpstring][id][propput] */ HRESULT ( STDMETHODCALLTYPE *put_EnableASPDebugging )( 
            ProjectConfigurationProperties * This,
            /* [in] */ VARIANT_BOOL bEnableASPDebugging);
        
        /* [helpstring][id][propget] */ HRESULT ( STDMETHODCALLTYPE *get_EnableASPXDebugging )( 
            ProjectConfigurationProperties * This,
            /* [retval][out] */ VARIANT_BOOL *pbEnableASPXDebugging);
        
        /* [helpstring][id][propput] */ HRESULT ( STDMETHODCALLTYPE *put_EnableASPXDebugging )( 
            ProjectConfigurationProperties * This,
            /* [in] */ VARIANT_BOOL bEnableASPXDebugging);
        
        /* [helpstring][id][propget] */ HRESULT ( STDMETHODCALLTYPE *get_EnableUnmanagedDebugging )( 
            ProjectConfigurationProperties * This,
            /* [retval][out] */ VARIANT_BOOL *pbEnableUnmanagedDebugging);
        
        /* [helpstring][id][propput] */ HRESULT ( STDMETHODCALLTYPE *put_EnableUnmanagedDebugging )( 
            ProjectConfigurationProperties * This,
            /* [in] */ VARIANT_BOOL bEnableUnmanagedDebugging);
        
        /* [helpstring][id][propget] */ HRESULT ( STDMETHODCALLTYPE *get_StartAction )( 
            ProjectConfigurationProperties * This,
            /* [retval][out] */ prjStartAction *pdebugStartMode);
        
        /* [helpstring][id][propput] */ HRESULT ( STDMETHODCALLTYPE *put_StartAction )( 
            ProjectConfigurationProperties * This,
            /* [in] */ prjStartAction debugStartMode);
        
        /* [hidden][helpstring][nonbrowsable][id][propget] */ HRESULT ( STDMETHODCALLTYPE *get_Extender )( 
            ProjectConfigurationProperties * This,
            /* [in] */ BSTR ExtenderName,
            /* [retval][out] */ IDispatch **Extender);
        
        /* [hidden][helpstring][nonbrowsable][id][propget] */ HRESULT ( STDMETHODCALLTYPE *get_ExtenderNames )( 
            ProjectConfigurationProperties * This,
            /* [retval][out] */ VARIANT *ExtenderNames);
        
        /* [hidden][helpstring][nonbrowsable][id][propget] */ HRESULT ( STDMETHODCALLTYPE *get_ExtenderCATID )( 
            ProjectConfigurationProperties * This,
            /* [retval][out] */ BSTR *pRetval);
        
        /* [helpstring][id][propget] */ HRESULT ( STDMETHODCALLTYPE *get_WarningLevel )( 
            ProjectConfigurationProperties * This,
            /* [retval][out] */ prjWarningLevel *pWarningLeve);
        
        /* [helpstring][id][propput] */ HRESULT ( STDMETHODCALLTYPE *put_WarningLevel )( 
            ProjectConfigurationProperties * This,
            /* [in] */ prjWarningLevel warningLevel);
        
        /* [helpstring][id][propget] */ HRESULT ( STDMETHODCALLTYPE *get_TreatWarningsAsErrors )( 
            ProjectConfigurationProperties * This,
            /* [retval][out] */ VARIANT_BOOL *pWarningAsError);
        
        /* [helpstring][id][propput] */ HRESULT ( STDMETHODCALLTYPE *put_TreatWarningsAsErrors )( 
            ProjectConfigurationProperties * This,
            /* [in] */ VARIANT_BOOL warningAsError);
        
        /* [helpstring][id][propget] */ HRESULT ( STDMETHODCALLTYPE *get_EnableSQLServerDebugging )( 
            ProjectConfigurationProperties * This,
            /* [retval][out] */ VARIANT_BOOL *pbEnableSQLServerDebugging);
        
        /* [helpstring][id][propput] */ HRESULT ( STDMETHODCALLTYPE *put_EnableSQLServerDebugging )( 
            ProjectConfigurationProperties * This,
            /* [in] */ VARIANT_BOOL bEnableSQLServerDebugging);
        
        /* [helpstring][id][propget] */ HRESULT ( STDMETHODCALLTYPE *get_FileAlignment )( 
            ProjectConfigurationProperties * This,
            /* [retval][out] */ DWORD *pdwFileAlignment);
        
        /* [helpstring][id][propput] */ HRESULT ( STDMETHODCALLTYPE *put_FileAlignment )( 
            ProjectConfigurationProperties * This,
            /* [in] */ DWORD dwFileAlignment);
        
        /* [helpstring][id][propget] */ HRESULT ( STDMETHODCALLTYPE *get_RegisterForComInterop )( 
            ProjectConfigurationProperties * This,
            /* [retval][out] */ VARIANT_BOOL *pVal);
        
        /* [helpstring][id][propput] */ HRESULT ( STDMETHODCALLTYPE *put_RegisterForComInterop )( 
            ProjectConfigurationProperties * This,
            /* [in] */ VARIANT_BOOL val);
        
        /* [helpstring][id][propget] */ HRESULT ( STDMETHODCALLTYPE *get_ConfigurationOverrideFile )( 
            ProjectConfigurationProperties * This,
            /* [retval][out] */ BSTR *pbstrConfigFile);
        
        /* [helpstring][id][propput] */ HRESULT ( STDMETHODCALLTYPE *put_ConfigurationOverrideFile )( 
            ProjectConfigurationProperties * This,
            /* [in] */ BSTR bstrConfigFile);
        
        /* [helpstring][id][propget] */ HRESULT ( STDMETHODCALLTYPE *get_RemoteDebugEnabled )( 
            ProjectConfigurationProperties * This,
            /* [retval][out] */ VARIANT_BOOL *pbEnableRemoteLaunch);
        
        /* [helpstring][id][propput] */ HRESULT ( STDMETHODCALLTYPE *put_RemoteDebugEnabled )( 
            ProjectConfigurationProperties * This,
            /* [in] */ VARIANT_BOOL bEnableRemoteLaunch);
        
        /* [helpstring][id][propget] */ HRESULT ( STDMETHODCALLTYPE *get_RemoteDebugMachine )( 
            ProjectConfigurationProperties * This,
            /* [retval][out] */ BSTR *pbstrRemoteLaunchMach);
        
        /* [helpstring][id][propput] */ HRESULT ( STDMETHODCALLTYPE *put_RemoteDebugMachine )( 
            ProjectConfigurationProperties * This,
            /* [in] */ BSTR bstrRemoteLaunchMach);
        
        END_INTERFACE
    } ProjectConfigurationPropertiesVtbl;

    interface ProjectConfigurationProperties
    {
        CONST_VTBL struct ProjectConfigurationPropertiesVtbl *lpVtbl;
    };

    

#ifdef COBJMACROS


#define ProjectConfigurationProperties_QueryInterface(This,riid,ppvObject)	\
    (This)->lpVtbl -> QueryInterface(This,riid,ppvObject)

#define ProjectConfigurationProperties_AddRef(This)	\
    (This)->lpVtbl -> AddRef(This)

#define ProjectConfigurationProperties_Release(This)	\
    (This)->lpVtbl -> Release(This)


#define ProjectConfigurationProperties_GetTypeInfoCount(This,pctinfo)	\
    (This)->lpVtbl -> GetTypeInfoCount(This,pctinfo)

#define ProjectConfigurationProperties_GetTypeInfo(This,iTInfo,lcid,ppTInfo)	\
    (This)->lpVtbl -> GetTypeInfo(This,iTInfo,lcid,ppTInfo)

#define ProjectConfigurationProperties_GetIDsOfNames(This,riid,rgszNames,cNames,lcid,rgDispId)	\
    (This)->lpVtbl -> GetIDsOfNames(This,riid,rgszNames,cNames,lcid,rgDispId)

#define ProjectConfigurationProperties_Invoke(This,dispIdMember,riid,lcid,wFlags,pDispParams,pVarResult,pExcepInfo,puArgErr)	\
    (This)->lpVtbl -> Invoke(This,dispIdMember,riid,lcid,wFlags,pDispParams,pVarResult,pExcepInfo,puArgErr)


#define ProjectConfigurationProperties_get___id(This,pbstrName)	\
    (This)->lpVtbl -> get___id(This,pbstrName)

#define ProjectConfigurationProperties_get_DebugSymbols(This,pbGenerate)	\
    (This)->lpVtbl -> get_DebugSymbols(This,pbGenerate)

#define ProjectConfigurationProperties_put_DebugSymbols(This,bGenerate)	\
    (This)->lpVtbl -> put_DebugSymbols(This,bGenerate)

#define ProjectConfigurationProperties_get_DefineDebug(This,pbDefineDebug)	\
    (This)->lpVtbl -> get_DefineDebug(This,pbDefineDebug)

#define ProjectConfigurationProperties_put_DefineDebug(This,bDefineDebug)	\
    (This)->lpVtbl -> put_DefineDebug(This,bDefineDebug)

#define ProjectConfigurationProperties_get_DefineTrace(This,pbDefineTrace)	\
    (This)->lpVtbl -> get_DefineTrace(This,pbDefineTrace)

#define ProjectConfigurationProperties_put_DefineTrace(This,bDefineTrace)	\
    (This)->lpVtbl -> put_DefineTrace(This,bDefineTrace)

#define ProjectConfigurationProperties_get_OutputPath(This,pbstrOutputPath)	\
    (This)->lpVtbl -> get_OutputPath(This,pbstrOutputPath)

#define ProjectConfigurationProperties_put_OutputPath(This,bstrOutputPath)	\
    (This)->lpVtbl -> put_OutputPath(This,bstrOutputPath)

#define ProjectConfigurationProperties_get_IntermediatePath(This,pbstrIntermediatePath)	\
    (This)->lpVtbl -> get_IntermediatePath(This,pbstrIntermediatePath)

#define ProjectConfigurationProperties_put_IntermediatePath(This,bstrIntermediatePath)	\
    (This)->lpVtbl -> put_IntermediatePath(This,bstrIntermediatePath)

#define ProjectConfigurationProperties_get_DefineConstants(This,pbstrDefineConstants)	\
    (This)->lpVtbl -> get_DefineConstants(This,pbstrDefineConstants)

#define ProjectConfigurationProperties_put_DefineConstants(This,bstrDefineConstants)	\
    (This)->lpVtbl -> put_DefineConstants(This,bstrDefineConstants)

#define ProjectConfigurationProperties_get_RemoveIntegerChecks(This,pbRemoveIntegerChecks)	\
    (This)->lpVtbl -> get_RemoveIntegerChecks(This,pbRemoveIntegerChecks)

#define ProjectConfigurationProperties_put_RemoveIntegerChecks(This,bRemoveIntegerChecks)	\
    (This)->lpVtbl -> put_RemoveIntegerChecks(This,bRemoveIntegerChecks)

#define ProjectConfigurationProperties_get_BaseAddress(This,pdwBaseAddress)	\
    (This)->lpVtbl -> get_BaseAddress(This,pdwBaseAddress)

#define ProjectConfigurationProperties_put_BaseAddress(This,dwBaseAddress)	\
    (This)->lpVtbl -> put_BaseAddress(This,dwBaseAddress)

#define ProjectConfigurationProperties_get_AllowUnsafeBlocks(This,pbUnsafe)	\
    (This)->lpVtbl -> get_AllowUnsafeBlocks(This,pbUnsafe)

#define ProjectConfigurationProperties_put_AllowUnsafeBlocks(This,bUnsafe)	\
    (This)->lpVtbl -> put_AllowUnsafeBlocks(This,bUnsafe)

#define ProjectConfigurationProperties_get_CheckForOverflowUnderflow(This,pbCheckForOverflowUnderflow)	\
    (This)->lpVtbl -> get_CheckForOverflowUnderflow(This,pbCheckForOverflowUnderflow)

#define ProjectConfigurationProperties_put_CheckForOverflowUnderflow(This,bCheckForOverflowUnderflow)	\
    (This)->lpVtbl -> put_CheckForOverflowUnderflow(This,bCheckForOverflowUnderflow)

#define ProjectConfigurationProperties_get_DocumentationFile(This,pbstrDocumentationFile)	\
    (This)->lpVtbl -> get_DocumentationFile(This,pbstrDocumentationFile)

#define ProjectConfigurationProperties_put_DocumentationFile(This,bstrDocumentationFile)	\
    (This)->lpVtbl -> put_DocumentationFile(This,bstrDocumentationFile)

#define ProjectConfigurationProperties_get_Optimize(This,pbOptimize)	\
    (This)->lpVtbl -> get_Optimize(This,pbOptimize)

#define ProjectConfigurationProperties_put_Optimize(This,bCheckForOverflowUnderflow)	\
    (This)->lpVtbl -> put_Optimize(This,bCheckForOverflowUnderflow)

#define ProjectConfigurationProperties_get_IncrementalBuild(This,pbIncrementalBuild)	\
    (This)->lpVtbl -> get_IncrementalBuild(This,pbIncrementalBuild)

#define ProjectConfigurationProperties_put_IncrementalBuild(This,bIncrementalBuild)	\
    (This)->lpVtbl -> put_IncrementalBuild(This,bIncrementalBuild)

#define ProjectConfigurationProperties_get_StartProgram(This,pbstrStartProgram)	\
    (This)->lpVtbl -> get_StartProgram(This,pbstrStartProgram)

#define ProjectConfigurationProperties_put_StartProgram(This,bstrStartProgram)	\
    (This)->lpVtbl -> put_StartProgram(This,bstrStartProgram)

#define ProjectConfigurationProperties_get_StartWorkingDirectory(This,pbstrStartWorkingDirectory)	\
    (This)->lpVtbl -> get_StartWorkingDirectory(This,pbstrStartWorkingDirectory)

#define ProjectConfigurationProperties_put_StartWorkingDirectory(This,bstrStartWorkingDirectory)	\
    (This)->lpVtbl -> put_StartWorkingDirectory(This,bstrStartWorkingDirectory)

#define ProjectConfigurationProperties_get_StartURL(This,pbstrStartURL)	\
    (This)->lpVtbl -> get_StartURL(This,pbstrStartURL)

#define ProjectConfigurationProperties_put_StartURL(This,bstrStartURL)	\
    (This)->lpVtbl -> put_StartURL(This,bstrStartURL)

#define ProjectConfigurationProperties_get_StartPage(This,pbstrStartPage)	\
    (This)->lpVtbl -> get_StartPage(This,pbstrStartPage)

#define ProjectConfigurationProperties_put_StartPage(This,bstrStartPage)	\
    (This)->lpVtbl -> put_StartPage(This,bstrStartPage)

#define ProjectConfigurationProperties_get_StartArguments(This,pbstrStartArguments)	\
    (This)->lpVtbl -> get_StartArguments(This,pbstrStartArguments)

#define ProjectConfigurationProperties_put_StartArguments(This,bstrStartArguments)	\
    (This)->lpVtbl -> put_StartArguments(This,bstrStartArguments)

#define ProjectConfigurationProperties_get_StartWithIE(This,pbStartWithIE)	\
    (This)->lpVtbl -> get_StartWithIE(This,pbStartWithIE)

#define ProjectConfigurationProperties_put_StartWithIE(This,bStartWithIE)	\
    (This)->lpVtbl -> put_StartWithIE(This,bStartWithIE)

#define ProjectConfigurationProperties_get_EnableASPDebugging(This,pbEnableASPDebugging)	\
    (This)->lpVtbl -> get_EnableASPDebugging(This,pbEnableASPDebugging)

#define ProjectConfigurationProperties_put_EnableASPDebugging(This,bEnableASPDebugging)	\
    (This)->lpVtbl -> put_EnableASPDebugging(This,bEnableASPDebugging)

#define ProjectConfigurationProperties_get_EnableASPXDebugging(This,pbEnableASPXDebugging)	\
    (This)->lpVtbl -> get_EnableASPXDebugging(This,pbEnableASPXDebugging)

#define ProjectConfigurationProperties_put_EnableASPXDebugging(This,bEnableASPXDebugging)	\
    (This)->lpVtbl -> put_EnableASPXDebugging(This,bEnableASPXDebugging)

#define ProjectConfigurationProperties_get_EnableUnmanagedDebugging(This,pbEnableUnmanagedDebugging)	\
    (This)->lpVtbl -> get_EnableUnmanagedDebugging(This,pbEnableUnmanagedDebugging)

#define ProjectConfigurationProperties_put_EnableUnmanagedDebugging(This,bEnableUnmanagedDebugging)	\
    (This)->lpVtbl -> put_EnableUnmanagedDebugging(This,bEnableUnmanagedDebugging)

#define ProjectConfigurationProperties_get_StartAction(This,pdebugStartMode)	\
    (This)->lpVtbl -> get_StartAction(This,pdebugStartMode)

#define ProjectConfigurationProperties_put_StartAction(This,debugStartMode)	\
    (This)->lpVtbl -> put_StartAction(This,debugStartMode)

#define ProjectConfigurationProperties_get_Extender(This,ExtenderName,Extender)	\
    (This)->lpVtbl -> get_Extender(This,ExtenderName,Extender)

#define ProjectConfigurationProperties_get_ExtenderNames(This,ExtenderNames)	\
    (This)->lpVtbl -> get_ExtenderNames(This,ExtenderNames)

#define ProjectConfigurationProperties_get_ExtenderCATID(This,pRetval)	\
    (This)->lpVtbl -> get_ExtenderCATID(This,pRetval)

#define ProjectConfigurationProperties_get_WarningLevel(This,pWarningLeve)	\
    (This)->lpVtbl -> get_WarningLevel(This,pWarningLeve)

#define ProjectConfigurationProperties_put_WarningLevel(This,warningLevel)	\
    (This)->lpVtbl -> put_WarningLevel(This,warningLevel)

#define ProjectConfigurationProperties_get_TreatWarningsAsErrors(This,pWarningAsError)	\
    (This)->lpVtbl -> get_TreatWarningsAsErrors(This,pWarningAsError)

#define ProjectConfigurationProperties_put_TreatWarningsAsErrors(This,warningAsError)	\
    (This)->lpVtbl -> put_TreatWarningsAsErrors(This,warningAsError)

#define ProjectConfigurationProperties_get_EnableSQLServerDebugging(This,pbEnableSQLServerDebugging)	\
    (This)->lpVtbl -> get_EnableSQLServerDebugging(This,pbEnableSQLServerDebugging)

#define ProjectConfigurationProperties_put_EnableSQLServerDebugging(This,bEnableSQLServerDebugging)	\
    (This)->lpVtbl -> put_EnableSQLServerDebugging(This,bEnableSQLServerDebugging)

#define ProjectConfigurationProperties_get_FileAlignment(This,pdwFileAlignment)	\
    (This)->lpVtbl -> get_FileAlignment(This,pdwFileAlignment)

#define ProjectConfigurationProperties_put_FileAlignment(This,dwFileAlignment)	\
    (This)->lpVtbl -> put_FileAlignment(This,dwFileAlignment)

#define ProjectConfigurationProperties_get_RegisterForComInterop(This,pVal)	\
    (This)->lpVtbl -> get_RegisterForComInterop(This,pVal)

#define ProjectConfigurationProperties_put_RegisterForComInterop(This,val)	\
    (This)->lpVtbl -> put_RegisterForComInterop(This,val)

#define ProjectConfigurationProperties_get_ConfigurationOverrideFile(This,pbstrConfigFile)	\
    (This)->lpVtbl -> get_ConfigurationOverrideFile(This,pbstrConfigFile)

#define ProjectConfigurationProperties_put_ConfigurationOverrideFile(This,bstrConfigFile)	\
    (This)->lpVtbl -> put_ConfigurationOverrideFile(This,bstrConfigFile)

#define ProjectConfigurationProperties_get_RemoteDebugEnabled(This,pbEnableRemoteLaunch)	\
    (This)->lpVtbl -> get_RemoteDebugEnabled(This,pbEnableRemoteLaunch)

#define ProjectConfigurationProperties_put_RemoteDebugEnabled(This,bEnableRemoteLaunch)	\
    (This)->lpVtbl -> put_RemoteDebugEnabled(This,bEnableRemoteLaunch)

#define ProjectConfigurationProperties_get_RemoteDebugMachine(This,pbstrRemoteLaunchMach)	\
    (This)->lpVtbl -> get_RemoteDebugMachine(This,pbstrRemoteLaunchMach)

#define ProjectConfigurationProperties_put_RemoteDebugMachine(This,bstrRemoteLaunchMach)	\
    (This)->lpVtbl -> put_RemoteDebugMachine(This,bstrRemoteLaunchMach)

#endif /* COBJMACROS */


#endif 	/* C style interface */



/* [hidden][helpstring][id][propget] */ HRESULT STDMETHODCALLTYPE ProjectConfigurationProperties_get___id_Proxy( 
    ProjectConfigurationProperties * This,
    /* [retval][out] */ BSTR *pbstrName);


void __RPC_STUB ProjectConfigurationProperties_get___id_Stub(
    IRpcStubBuffer *This,
    IRpcChannelBuffer *_pRpcChannelBuffer,
    PRPC_MESSAGE _pRpcMessage,
    DWORD *_pdwStubPhase);


/* [helpstring][id][propget] */ HRESULT STDMETHODCALLTYPE ProjectConfigurationProperties_get_DebugSymbols_Proxy( 
    ProjectConfigurationProperties * This,
    /* [retval][out] */ VARIANT_BOOL *pbGenerate);


void __RPC_STUB ProjectConfigurationProperties_get_DebugSymbols_Stub(
    IRpcStubBuffer *This,
    IRpcChannelBuffer *_pRpcChannelBuffer,
    PRPC_MESSAGE _pRpcMessage,
    DWORD *_pdwStubPhase);


/* [helpstring][id][propput] */ HRESULT STDMETHODCALLTYPE ProjectConfigurationProperties_put_DebugSymbols_Proxy( 
    ProjectConfigurationProperties * This,
    /* [in] */ VARIANT_BOOL bGenerate);


void __RPC_STUB ProjectConfigurationProperties_put_DebugSymbols_Stub(
    IRpcStubBuffer *This,
    IRpcChannelBuffer *_pRpcChannelBuffer,
    PRPC_MESSAGE _pRpcMessage,
    DWORD *_pdwStubPhase);


/* [helpstring][id][propget] */ HRESULT STDMETHODCALLTYPE ProjectConfigurationProperties_get_DefineDebug_Proxy( 
    ProjectConfigurationProperties * This,
    /* [retval][out] */ VARIANT_BOOL *pbDefineDebug);


void __RPC_STUB ProjectConfigurationProperties_get_DefineDebug_Stub(
    IRpcStubBuffer *This,
    IRpcChannelBuffer *_pRpcChannelBuffer,
    PRPC_MESSAGE _pRpcMessage,
    DWORD *_pdwStubPhase);


/* [helpstring][id][propput] */ HRESULT STDMETHODCALLTYPE ProjectConfigurationProperties_put_DefineDebug_Proxy( 
    ProjectConfigurationProperties * This,
    /* [in] */ VARIANT_BOOL bDefineDebug);


void __RPC_STUB ProjectConfigurationProperties_put_DefineDebug_Stub(
    IRpcStubBuffer *This,
    IRpcChannelBuffer *_pRpcChannelBuffer,
    PRPC_MESSAGE _pRpcMessage,
    DWORD *_pdwStubPhase);


/* [helpstring][id][propget] */ HRESULT STDMETHODCALLTYPE ProjectConfigurationProperties_get_DefineTrace_Proxy( 
    ProjectConfigurationProperties * This,
    /* [retval][out] */ VARIANT_BOOL *pbDefineTrace);


void __RPC_STUB ProjectConfigurationProperties_get_DefineTrace_Stub(
    IRpcStubBuffer *This,
    IRpcChannelBuffer *_pRpcChannelBuffer,
    PRPC_MESSAGE _pRpcMessage,
    DWORD *_pdwStubPhase);


/* [helpstring][id][propput] */ HRESULT STDMETHODCALLTYPE ProjectConfigurationProperties_put_DefineTrace_Proxy( 
    ProjectConfigurationProperties * This,
    /* [in] */ VARIANT_BOOL bDefineTrace);


void __RPC_STUB ProjectConfigurationProperties_put_DefineTrace_Stub(
    IRpcStubBuffer *This,
    IRpcChannelBuffer *_pRpcChannelBuffer,
    PRPC_MESSAGE _pRpcMessage,
    DWORD *_pdwStubPhase);


/* [helpstring][id][propget] */ HRESULT STDMETHODCALLTYPE ProjectConfigurationProperties_get_OutputPath_Proxy( 
    ProjectConfigurationProperties * This,
    /* [retval][out] */ BSTR *pbstrOutputPath);


void __RPC_STUB ProjectConfigurationProperties_get_OutputPath_Stub(
    IRpcStubBuffer *This,
    IRpcChannelBuffer *_pRpcChannelBuffer,
    PRPC_MESSAGE _pRpcMessage,
    DWORD *_pdwStubPhase);


/* [helpstring][id][propput] */ HRESULT STDMETHODCALLTYPE ProjectConfigurationProperties_put_OutputPath_Proxy( 
    ProjectConfigurationProperties * This,
    /* [in] */ BSTR bstrOutputPath);


void __RPC_STUB ProjectConfigurationProperties_put_OutputPath_Stub(
    IRpcStubBuffer *This,
    IRpcChannelBuffer *_pRpcChannelBuffer,
    PRPC_MESSAGE _pRpcMessage,
    DWORD *_pdwStubPhase);


/* [helpstring][id][propget] */ HRESULT STDMETHODCALLTYPE ProjectConfigurationProperties_get_IntermediatePath_Proxy( 
    ProjectConfigurationProperties * This,
    /* [retval][out] */ BSTR *pbstrIntermediatePath);


void __RPC_STUB ProjectConfigurationProperties_get_IntermediatePath_Stub(
    IRpcStubBuffer *This,
    IRpcChannelBuffer *_pRpcChannelBuffer,
    PRPC_MESSAGE _pRpcMessage,
    DWORD *_pdwStubPhase);


/* [hidden][helpstring][id][propput] */ HRESULT STDMETHODCALLTYPE ProjectConfigurationProperties_put_IntermediatePath_Proxy( 
    ProjectConfigurationProperties * This,
    /* [in] */ BSTR bstrIntermediatePath);


void __RPC_STUB ProjectConfigurationProperties_put_IntermediatePath_Stub(
    IRpcStubBuffer *This,
    IRpcChannelBuffer *_pRpcChannelBuffer,
    PRPC_MESSAGE _pRpcMessage,
    DWORD *_pdwStubPhase);


/* [helpstring][id][propget] */ HRESULT STDMETHODCALLTYPE ProjectConfigurationProperties_get_DefineConstants_Proxy( 
    ProjectConfigurationProperties * This,
    /* [retval][out] */ BSTR *pbstrDefineConstants);


void __RPC_STUB ProjectConfigurationProperties_get_DefineConstants_Stub(
    IRpcStubBuffer *This,
    IRpcChannelBuffer *_pRpcChannelBuffer,
    PRPC_MESSAGE _pRpcMessage,
    DWORD *_pdwStubPhase);


/* [helpstring][id][propput] */ HRESULT STDMETHODCALLTYPE ProjectConfigurationProperties_put_DefineConstants_Proxy( 
    ProjectConfigurationProperties * This,
    /* [in] */ BSTR bstrDefineConstants);


void __RPC_STUB ProjectConfigurationProperties_put_DefineConstants_Stub(
    IRpcStubBuffer *This,
    IRpcChannelBuffer *_pRpcChannelBuffer,
    PRPC_MESSAGE _pRpcMessage,
    DWORD *_pdwStubPhase);


/* [helpstring][id][propget] */ HRESULT STDMETHODCALLTYPE ProjectConfigurationProperties_get_RemoveIntegerChecks_Proxy( 
    ProjectConfigurationProperties * This,
    /* [retval][out] */ VARIANT_BOOL *pbRemoveIntegerChecks);


void __RPC_STUB ProjectConfigurationProperties_get_RemoveIntegerChecks_Stub(
    IRpcStubBuffer *This,
    IRpcChannelBuffer *_pRpcChannelBuffer,
    PRPC_MESSAGE _pRpcMessage,
    DWORD *_pdwStubPhase);


/* [helpstring][id][propput] */ HRESULT STDMETHODCALLTYPE ProjectConfigurationProperties_put_RemoveIntegerChecks_Proxy( 
    ProjectConfigurationProperties * This,
    /* [in] */ VARIANT_BOOL bRemoveIntegerChecks);


void __RPC_STUB ProjectConfigurationProperties_put_RemoveIntegerChecks_Stub(
    IRpcStubBuffer *This,
    IRpcChannelBuffer *_pRpcChannelBuffer,
    PRPC_MESSAGE _pRpcMessage,
    DWORD *_pdwStubPhase);


/* [helpstring][id][propget] */ HRESULT STDMETHODCALLTYPE ProjectConfigurationProperties_get_BaseAddress_Proxy( 
    ProjectConfigurationProperties * This,
    /* [retval][out] */ DWORD *pdwBaseAddress);


void __RPC_STUB ProjectConfigurationProperties_get_BaseAddress_Stub(
    IRpcStubBuffer *This,
    IRpcChannelBuffer *_pRpcChannelBuffer,
    PRPC_MESSAGE _pRpcMessage,
    DWORD *_pdwStubPhase);


/* [helpstring][id][propput] */ HRESULT STDMETHODCALLTYPE ProjectConfigurationProperties_put_BaseAddress_Proxy( 
    ProjectConfigurationProperties * This,
    /* [in] */ DWORD dwBaseAddress);


void __RPC_STUB ProjectConfigurationProperties_put_BaseAddress_Stub(
    IRpcStubBuffer *This,
    IRpcChannelBuffer *_pRpcChannelBuffer,
    PRPC_MESSAGE _pRpcMessage,
    DWORD *_pdwStubPhase);


/* [helpstring][id][propget] */ HRESULT STDMETHODCALLTYPE ProjectConfigurationProperties_get_AllowUnsafeBlocks_Proxy( 
    ProjectConfigurationProperties * This,
    /* [retval][out] */ VARIANT_BOOL *pbUnsafe);


void __RPC_STUB ProjectConfigurationProperties_get_AllowUnsafeBlocks_Stub(
    IRpcStubBuffer *This,
    IRpcChannelBuffer *_pRpcChannelBuffer,
    PRPC_MESSAGE _pRpcMessage,
    DWORD *_pdwStubPhase);


/* [helpstring][id][propput] */ HRESULT STDMETHODCALLTYPE ProjectConfigurationProperties_put_AllowUnsafeBlocks_Proxy( 
    ProjectConfigurationProperties * This,
    /* [in] */ VARIANT_BOOL bUnsafe);


void __RPC_STUB ProjectConfigurationProperties_put_AllowUnsafeBlocks_Stub(
    IRpcStubBuffer *This,
    IRpcChannelBuffer *_pRpcChannelBuffer,
    PRPC_MESSAGE _pRpcMessage,
    DWORD *_pdwStubPhase);


/* [helpstring][id][propget] */ HRESULT STDMETHODCALLTYPE ProjectConfigurationProperties_get_CheckForOverflowUnderflow_Proxy( 
    ProjectConfigurationProperties * This,
    /* [retval][out] */ VARIANT_BOOL *pbCheckForOverflowUnderflow);


void __RPC_STUB ProjectConfigurationProperties_get_CheckForOverflowUnderflow_Stub(
    IRpcStubBuffer *This,
    IRpcChannelBuffer *_pRpcChannelBuffer,
    PRPC_MESSAGE _pRpcMessage,
    DWORD *_pdwStubPhase);


/* [helpstring][id][propput] */ HRESULT STDMETHODCALLTYPE ProjectConfigurationProperties_put_CheckForOverflowUnderflow_Proxy( 
    ProjectConfigurationProperties * This,
    /* [in] */ VARIANT_BOOL bCheckForOverflowUnderflow);


void __RPC_STUB ProjectConfigurationProperties_put_CheckForOverflowUnderflow_Stub(
    IRpcStubBuffer *This,
    IRpcChannelBuffer *_pRpcChannelBuffer,
    PRPC_MESSAGE _pRpcMessage,
    DWORD *_pdwStubPhase);


/* [helpstring][id][propget] */ HRESULT STDMETHODCALLTYPE ProjectConfigurationProperties_get_DocumentationFile_Proxy( 
    ProjectConfigurationProperties * This,
    /* [retval][out] */ BSTR *pbstrDocumentationFile);


void __RPC_STUB ProjectConfigurationProperties_get_DocumentationFile_Stub(
    IRpcStubBuffer *This,
    IRpcChannelBuffer *_pRpcChannelBuffer,
    PRPC_MESSAGE _pRpcMessage,
    DWORD *_pdwStubPhase);


/* [helpstring][id][propput] */ HRESULT STDMETHODCALLTYPE ProjectConfigurationProperties_put_DocumentationFile_Proxy( 
    ProjectConfigurationProperties * This,
    /* [in] */ BSTR bstrDocumentationFile);


void __RPC_STUB ProjectConfigurationProperties_put_DocumentationFile_Stub(
    IRpcStubBuffer *This,
    IRpcChannelBuffer *_pRpcChannelBuffer,
    PRPC_MESSAGE _pRpcMessage,
    DWORD *_pdwStubPhase);


/* [helpstring][id][propget] */ HRESULT STDMETHODCALLTYPE ProjectConfigurationProperties_get_Optimize_Proxy( 
    ProjectConfigurationProperties * This,
    /* [retval][out] */ VARIANT_BOOL *pbOptimize);


void __RPC_STUB ProjectConfigurationProperties_get_Optimize_Stub(
    IRpcStubBuffer *This,
    IRpcChannelBuffer *_pRpcChannelBuffer,
    PRPC_MESSAGE _pRpcMessage,
    DWORD *_pdwStubPhase);


/* [helpstring][id][propput] */ HRESULT STDMETHODCALLTYPE ProjectConfigurationProperties_put_Optimize_Proxy( 
    ProjectConfigurationProperties * This,
    /* [in] */ VARIANT_BOOL bCheckForOverflowUnderflow);


void __RPC_STUB ProjectConfigurationProperties_put_Optimize_Stub(
    IRpcStubBuffer *This,
    IRpcChannelBuffer *_pRpcChannelBuffer,
    PRPC_MESSAGE _pRpcMessage,
    DWORD *_pdwStubPhase);


/* [helpstring][id][propget] */ HRESULT STDMETHODCALLTYPE ProjectConfigurationProperties_get_IncrementalBuild_Proxy( 
    ProjectConfigurationProperties * This,
    /* [retval][out] */ VARIANT_BOOL *pbIncrementalBuild);


void __RPC_STUB ProjectConfigurationProperties_get_IncrementalBuild_Stub(
    IRpcStubBuffer *This,
    IRpcChannelBuffer *_pRpcChannelBuffer,
    PRPC_MESSAGE _pRpcMessage,
    DWORD *_pdwStubPhase);


/* [helpstring][id][propput] */ HRESULT STDMETHODCALLTYPE ProjectConfigurationProperties_put_IncrementalBuild_Proxy( 
    ProjectConfigurationProperties * This,
    /* [in] */ VARIANT_BOOL bIncrementalBuild);


void __RPC_STUB ProjectConfigurationProperties_put_IncrementalBuild_Stub(
    IRpcStubBuffer *This,
    IRpcChannelBuffer *_pRpcChannelBuffer,
    PRPC_MESSAGE _pRpcMessage,
    DWORD *_pdwStubPhase);


/* [helpstring][id][propget] */ HRESULT STDMETHODCALLTYPE ProjectConfigurationProperties_get_StartProgram_Proxy( 
    ProjectConfigurationProperties * This,
    /* [retval][out] */ BSTR *pbstrStartProgram);


void __RPC_STUB ProjectConfigurationProperties_get_StartProgram_Stub(
    IRpcStubBuffer *This,
    IRpcChannelBuffer *_pRpcChannelBuffer,
    PRPC_MESSAGE _pRpcMessage,
    DWORD *_pdwStubPhase);


/* [helpstring][id][propput] */ HRESULT STDMETHODCALLTYPE ProjectConfigurationProperties_put_StartProgram_Proxy( 
    ProjectConfigurationProperties * This,
    /* [in] */ BSTR bstrStartProgram);


void __RPC_STUB ProjectConfigurationProperties_put_StartProgram_Stub(
    IRpcStubBuffer *This,
    IRpcChannelBuffer *_pRpcChannelBuffer,
    PRPC_MESSAGE _pRpcMessage,
    DWORD *_pdwStubPhase);


/* [helpstring][id][propget] */ HRESULT STDMETHODCALLTYPE ProjectConfigurationProperties_get_StartWorkingDirectory_Proxy( 
    ProjectConfigurationProperties * This,
    /* [retval][out] */ BSTR *pbstrStartWorkingDirectory);


void __RPC_STUB ProjectConfigurationProperties_get_StartWorkingDirectory_Stub(
    IRpcStubBuffer *This,
    IRpcChannelBuffer *_pRpcChannelBuffer,
    PRPC_MESSAGE _pRpcMessage,
    DWORD *_pdwStubPhase);


/* [helpstring][id][propput] */ HRESULT STDMETHODCALLTYPE ProjectConfigurationProperties_put_StartWorkingDirectory_Proxy( 
    ProjectConfigurationProperties * This,
    /* [in] */ BSTR bstrStartWorkingDirectory);


void __RPC_STUB ProjectConfigurationProperties_put_StartWorkingDirectory_Stub(
    IRpcStubBuffer *This,
    IRpcChannelBuffer *_pRpcChannelBuffer,
    PRPC_MESSAGE _pRpcMessage,
    DWORD *_pdwStubPhase);


/* [helpstring][id][propget] */ HRESULT STDMETHODCALLTYPE ProjectConfigurationProperties_get_StartURL_Proxy( 
    ProjectConfigurationProperties * This,
    /* [retval][out] */ BSTR *pbstrStartURL);


void __RPC_STUB ProjectConfigurationProperties_get_StartURL_Stub(
    IRpcStubBuffer *This,
    IRpcChannelBuffer *_pRpcChannelBuffer,
    PRPC_MESSAGE _pRpcMessage,
    DWORD *_pdwStubPhase);


/* [helpstring][id][propput] */ HRESULT STDMETHODCALLTYPE ProjectConfigurationProperties_put_StartURL_Proxy( 
    ProjectConfigurationProperties * This,
    /* [in] */ BSTR bstrStartURL);


void __RPC_STUB ProjectConfigurationProperties_put_StartURL_Stub(
    IRpcStubBuffer *This,
    IRpcChannelBuffer *_pRpcChannelBuffer,
    PRPC_MESSAGE _pRpcMessage,
    DWORD *_pdwStubPhase);


/* [helpstring][id][propget] */ HRESULT STDMETHODCALLTYPE ProjectConfigurationProperties_get_StartPage_Proxy( 
    ProjectConfigurationProperties * This,
    /* [retval][out] */ BSTR *pbstrStartPage);


void __RPC_STUB ProjectConfigurationProperties_get_StartPage_Stub(
    IRpcStubBuffer *This,
    IRpcChannelBuffer *_pRpcChannelBuffer,
    PRPC_MESSAGE _pRpcMessage,
    DWORD *_pdwStubPhase);


/* [helpstring][id][propput] */ HRESULT STDMETHODCALLTYPE ProjectConfigurationProperties_put_StartPage_Proxy( 
    ProjectConfigurationProperties * This,
    /* [in] */ BSTR bstrStartPage);


void __RPC_STUB ProjectConfigurationProperties_put_StartPage_Stub(
    IRpcStubBuffer *This,
    IRpcChannelBuffer *_pRpcChannelBuffer,
    PRPC_MESSAGE _pRpcMessage,
    DWORD *_pdwStubPhase);


/* [helpstring][id][propget] */ HRESULT STDMETHODCALLTYPE ProjectConfigurationProperties_get_StartArguments_Proxy( 
    ProjectConfigurationProperties * This,
    /* [retval][out] */ BSTR *pbstrStartArguments);


void __RPC_STUB ProjectConfigurationProperties_get_StartArguments_Stub(
    IRpcStubBuffer *This,
    IRpcChannelBuffer *_pRpcChannelBuffer,
    PRPC_MESSAGE _pRpcMessage,
    DWORD *_pdwStubPhase);


/* [helpstring][id][propput] */ HRESULT STDMETHODCALLTYPE ProjectConfigurationProperties_put_StartArguments_Proxy( 
    ProjectConfigurationProperties * This,
    /* [in] */ BSTR bstrStartArguments);


void __RPC_STUB ProjectConfigurationProperties_put_StartArguments_Stub(
    IRpcStubBuffer *This,
    IRpcChannelBuffer *_pRpcChannelBuffer,
    PRPC_MESSAGE _pRpcMessage,
    DWORD *_pdwStubPhase);


/* [helpstring][id][propget] */ HRESULT STDMETHODCALLTYPE ProjectConfigurationProperties_get_StartWithIE_Proxy( 
    ProjectConfigurationProperties * This,
    /* [retval][out] */ VARIANT_BOOL *pbStartWithIE);


void __RPC_STUB ProjectConfigurationProperties_get_StartWithIE_Stub(
    IRpcStubBuffer *This,
    IRpcChannelBuffer *_pRpcChannelBuffer,
    PRPC_MESSAGE _pRpcMessage,
    DWORD *_pdwStubPhase);


/* [helpstring][id][propput] */ HRESULT STDMETHODCALLTYPE ProjectConfigurationProperties_put_StartWithIE_Proxy( 
    ProjectConfigurationProperties * This,
    /* [in] */ VARIANT_BOOL bStartWithIE);


void __RPC_STUB ProjectConfigurationProperties_put_StartWithIE_Stub(
    IRpcStubBuffer *This,
    IRpcChannelBuffer *_pRpcChannelBuffer,
    PRPC_MESSAGE _pRpcMessage,
    DWORD *_pdwStubPhase);


/* [helpstring][id][propget] */ HRESULT STDMETHODCALLTYPE ProjectConfigurationProperties_get_EnableASPDebugging_Proxy( 
    ProjectConfigurationProperties * This,
    /* [retval][out] */ VARIANT_BOOL *pbEnableASPDebugging);


void __RPC_STUB ProjectConfigurationProperties_get_EnableASPDebugging_Stub(
    IRpcStubBuffer *This,
    IRpcChannelBuffer *_pRpcChannelBuffer,
    PRPC_MESSAGE _pRpcMessage,
    DWORD *_pdwStubPhase);


/* [helpstring][id][propput] */ HRESULT STDMETHODCALLTYPE ProjectConfigurationProperties_put_EnableASPDebugging_Proxy( 
    ProjectConfigurationProperties * This,
    /* [in] */ VARIANT_BOOL bEnableASPDebugging);


void __RPC_STUB ProjectConfigurationProperties_put_EnableASPDebugging_Stub(
    IRpcStubBuffer *This,
    IRpcChannelBuffer *_pRpcChannelBuffer,
    PRPC_MESSAGE _pRpcMessage,
    DWORD *_pdwStubPhase);


/* [helpstring][id][propget] */ HRESULT STDMETHODCALLTYPE ProjectConfigurationProperties_get_EnableASPXDebugging_Proxy( 
    ProjectConfigurationProperties * This,
    /* [retval][out] */ VARIANT_BOOL *pbEnableASPXDebugging);


void __RPC_STUB ProjectConfigurationProperties_get_EnableASPXDebugging_Stub(
    IRpcStubBuffer *This,
    IRpcChannelBuffer *_pRpcChannelBuffer,
    PRPC_MESSAGE _pRpcMessage,
    DWORD *_pdwStubPhase);


/* [helpstring][id][propput] */ HRESULT STDMETHODCALLTYPE ProjectConfigurationProperties_put_EnableASPXDebugging_Proxy( 
    ProjectConfigurationProperties * This,
    /* [in] */ VARIANT_BOOL bEnableASPXDebugging);


void __RPC_STUB ProjectConfigurationProperties_put_EnableASPXDebugging_Stub(
    IRpcStubBuffer *This,
    IRpcChannelBuffer *_pRpcChannelBuffer,
    PRPC_MESSAGE _pRpcMessage,
    DWORD *_pdwStubPhase);


/* [helpstring][id][propget] */ HRESULT STDMETHODCALLTYPE ProjectConfigurationProperties_get_EnableUnmanagedDebugging_Proxy( 
    ProjectConfigurationProperties * This,
    /* [retval][out] */ VARIANT_BOOL *pbEnableUnmanagedDebugging);


void __RPC_STUB ProjectConfigurationProperties_get_EnableUnmanagedDebugging_Stub(
    IRpcStubBuffer *This,
    IRpcChannelBuffer *_pRpcChannelBuffer,
    PRPC_MESSAGE _pRpcMessage,
    DWORD *_pdwStubPhase);


/* [helpstring][id][propput] */ HRESULT STDMETHODCALLTYPE ProjectConfigurationProperties_put_EnableUnmanagedDebugging_Proxy( 
    ProjectConfigurationProperties * This,
    /* [in] */ VARIANT_BOOL bEnableUnmanagedDebugging);


void __RPC_STUB ProjectConfigurationProperties_put_EnableUnmanagedDebugging_Stub(
    IRpcStubBuffer *This,
    IRpcChannelBuffer *_pRpcChannelBuffer,
    PRPC_MESSAGE _pRpcMessage,
    DWORD *_pdwStubPhase);


/* [helpstring][id][propget] */ HRESULT STDMETHODCALLTYPE ProjectConfigurationProperties_get_StartAction_Proxy( 
    ProjectConfigurationProperties * This,
    /* [retval][out] */ prjStartAction *pdebugStartMode);


void __RPC_STUB ProjectConfigurationProperties_get_StartAction_Stub(
    IRpcStubBuffer *This,
    IRpcChannelBuffer *_pRpcChannelBuffer,
    PRPC_MESSAGE _pRpcMessage,
    DWORD *_pdwStubPhase);


/* [helpstring][id][propput] */ HRESULT STDMETHODCALLTYPE ProjectConfigurationProperties_put_StartAction_Proxy( 
    ProjectConfigurationProperties * This,
    /* [in] */ prjStartAction debugStartMode);


void __RPC_STUB ProjectConfigurationProperties_put_StartAction_Stub(
    IRpcStubBuffer *This,
    IRpcChannelBuffer *_pRpcChannelBuffer,
    PRPC_MESSAGE _pRpcMessage,
    DWORD *_pdwStubPhase);


/* [hidden][helpstring][nonbrowsable][id][propget] */ HRESULT STDMETHODCALLTYPE ProjectConfigurationProperties_get_Extender_Proxy( 
    ProjectConfigurationProperties * This,
    /* [in] */ BSTR ExtenderName,
    /* [retval][out] */ IDispatch **Extender);


void __RPC_STUB ProjectConfigurationProperties_get_Extender_Stub(
    IRpcStubBuffer *This,
    IRpcChannelBuffer *_pRpcChannelBuffer,
    PRPC_MESSAGE _pRpcMessage,
    DWORD *_pdwStubPhase);


/* [hidden][helpstring][nonbrowsable][id][propget] */ HRESULT STDMETHODCALLTYPE ProjectConfigurationProperties_get_ExtenderNames_Proxy( 
    ProjectConfigurationProperties * This,
    /* [retval][out] */ VARIANT *ExtenderNames);


void __RPC_STUB ProjectConfigurationProperties_get_ExtenderNames_Stub(
    IRpcStubBuffer *This,
    IRpcChannelBuffer *_pRpcChannelBuffer,
    PRPC_MESSAGE _pRpcMessage,
    DWORD *_pdwStubPhase);


/* [hidden][helpstring][nonbrowsable][id][propget] */ HRESULT STDMETHODCALLTYPE ProjectConfigurationProperties_get_ExtenderCATID_Proxy( 
    ProjectConfigurationProperties * This,
    /* [retval][out] */ BSTR *pRetval);


void __RPC_STUB ProjectConfigurationProperties_get_ExtenderCATID_Stub(
    IRpcStubBuffer *This,
    IRpcChannelBuffer *_pRpcChannelBuffer,
    PRPC_MESSAGE _pRpcMessage,
    DWORD *_pdwStubPhase);


/* [helpstring][id][propget] */ HRESULT STDMETHODCALLTYPE ProjectConfigurationProperties_get_WarningLevel_Proxy( 
    ProjectConfigurationProperties * This,
    /* [retval][out] */ prjWarningLevel *pWarningLeve);


void __RPC_STUB ProjectConfigurationProperties_get_WarningLevel_Stub(
    IRpcStubBuffer *This,
    IRpcChannelBuffer *_pRpcChannelBuffer,
    PRPC_MESSAGE _pRpcMessage,
    DWORD *_pdwStubPhase);


/* [helpstring][id][propput] */ HRESULT STDMETHODCALLTYPE ProjectConfigurationProperties_put_WarningLevel_Proxy( 
    ProjectConfigurationProperties * This,
    /* [in] */ prjWarningLevel warningLevel);


void __RPC_STUB ProjectConfigurationProperties_put_WarningLevel_Stub(
    IRpcStubBuffer *This,
    IRpcChannelBuffer *_pRpcChannelBuffer,
    PRPC_MESSAGE _pRpcMessage,
    DWORD *_pdwStubPhase);


/* [helpstring][id][propget] */ HRESULT STDMETHODCALLTYPE ProjectConfigurationProperties_get_TreatWarningsAsErrors_Proxy( 
    ProjectConfigurationProperties * This,
    /* [retval][out] */ VARIANT_BOOL *pWarningAsError);


void __RPC_STUB ProjectConfigurationProperties_get_TreatWarningsAsErrors_Stub(
    IRpcStubBuffer *This,
    IRpcChannelBuffer *_pRpcChannelBuffer,
    PRPC_MESSAGE _pRpcMessage,
    DWORD *_pdwStubPhase);


/* [helpstring][id][propput] */ HRESULT STDMETHODCALLTYPE ProjectConfigurationProperties_put_TreatWarningsAsErrors_Proxy( 
    ProjectConfigurationProperties * This,
    /* [in] */ VARIANT_BOOL warningAsError);


void __RPC_STUB ProjectConfigurationProperties_put_TreatWarningsAsErrors_Stub(
    IRpcStubBuffer *This,
    IRpcChannelBuffer *_pRpcChannelBuffer,
    PRPC_MESSAGE _pRpcMessage,
    DWORD *_pdwStubPhase);


/* [helpstring][id][propget] */ HRESULT STDMETHODCALLTYPE ProjectConfigurationProperties_get_EnableSQLServerDebugging_Proxy( 
    ProjectConfigurationProperties * This,
    /* [retval][out] */ VARIANT_BOOL *pbEnableSQLServerDebugging);


void __RPC_STUB ProjectConfigurationProperties_get_EnableSQLServerDebugging_Stub(
    IRpcStubBuffer *This,
    IRpcChannelBuffer *_pRpcChannelBuffer,
    PRPC_MESSAGE _pRpcMessage,
    DWORD *_pdwStubPhase);


/* [helpstring][id][propput] */ HRESULT STDMETHODCALLTYPE ProjectConfigurationProperties_put_EnableSQLServerDebugging_Proxy( 
    ProjectConfigurationProperties * This,
    /* [in] */ VARIANT_BOOL bEnableSQLServerDebugging);


void __RPC_STUB ProjectConfigurationProperties_put_EnableSQLServerDebugging_Stub(
    IRpcStubBuffer *This,
    IRpcChannelBuffer *_pRpcChannelBuffer,
    PRPC_MESSAGE _pRpcMessage,
    DWORD *_pdwStubPhase);


/* [helpstring][id][propget] */ HRESULT STDMETHODCALLTYPE ProjectConfigurationProperties_get_FileAlignment_Proxy( 
    ProjectConfigurationProperties * This,
    /* [retval][out] */ DWORD *pdwFileAlignment);


void __RPC_STUB ProjectConfigurationProperties_get_FileAlignment_Stub(
    IRpcStubBuffer *This,
    IRpcChannelBuffer *_pRpcChannelBuffer,
    PRPC_MESSAGE _pRpcMessage,
    DWORD *_pdwStubPhase);


/* [helpstring][id][propput] */ HRESULT STDMETHODCALLTYPE ProjectConfigurationProperties_put_FileAlignment_Proxy( 
    ProjectConfigurationProperties * This,
    /* [in] */ DWORD dwFileAlignment);


void __RPC_STUB ProjectConfigurationProperties_put_FileAlignment_Stub(
    IRpcStubBuffer *This,
    IRpcChannelBuffer *_pRpcChannelBuffer,
    PRPC_MESSAGE _pRpcMessage,
    DWORD *_pdwStubPhase);


/* [helpstring][id][propget] */ HRESULT STDMETHODCALLTYPE ProjectConfigurationProperties_get_RegisterForComInterop_Proxy( 
    ProjectConfigurationProperties * This,
    /* [retval][out] */ VARIANT_BOOL *pVal);


void __RPC_STUB ProjectConfigurationProperties_get_RegisterForComInterop_Stub(
    IRpcStubBuffer *This,
    IRpcChannelBuffer *_pRpcChannelBuffer,
    PRPC_MESSAGE _pRpcMessage,
    DWORD *_pdwStubPhase);


/* [helpstring][id][propput] */ HRESULT STDMETHODCALLTYPE ProjectConfigurationProperties_put_RegisterForComInterop_Proxy( 
    ProjectConfigurationProperties * This,
    /* [in] */ VARIANT_BOOL val);


void __RPC_STUB ProjectConfigurationProperties_put_RegisterForComInterop_Stub(
    IRpcStubBuffer *This,
    IRpcChannelBuffer *_pRpcChannelBuffer,
    PRPC_MESSAGE _pRpcMessage,
    DWORD *_pdwStubPhase);


/* [helpstring][id][propget] */ HRESULT STDMETHODCALLTYPE ProjectConfigurationProperties_get_ConfigurationOverrideFile_Proxy( 
    ProjectConfigurationProperties * This,
    /* [retval][out] */ BSTR *pbstrConfigFile);


void __RPC_STUB ProjectConfigurationProperties_get_ConfigurationOverrideFile_Stub(
    IRpcStubBuffer *This,
    IRpcChannelBuffer *_pRpcChannelBuffer,
    PRPC_MESSAGE _pRpcMessage,
    DWORD *_pdwStubPhase);


/* [helpstring][id][propput] */ HRESULT STDMETHODCALLTYPE ProjectConfigurationProperties_put_ConfigurationOverrideFile_Proxy( 
    ProjectConfigurationProperties * This,
    /* [in] */ BSTR bstrConfigFile);


void __RPC_STUB ProjectConfigurationProperties_put_ConfigurationOverrideFile_Stub(
    IRpcStubBuffer *This,
    IRpcChannelBuffer *_pRpcChannelBuffer,
    PRPC_MESSAGE _pRpcMessage,
    DWORD *_pdwStubPhase);


/* [helpstring][id][propget] */ HRESULT STDMETHODCALLTYPE ProjectConfigurationProperties_get_RemoteDebugEnabled_Proxy( 
    ProjectConfigurationProperties * This,
    /* [retval][out] */ VARIANT_BOOL *pbEnableRemoteLaunch);


void __RPC_STUB ProjectConfigurationProperties_get_RemoteDebugEnabled_Stub(
    IRpcStubBuffer *This,
    IRpcChannelBuffer *_pRpcChannelBuffer,
    PRPC_MESSAGE _pRpcMessage,
    DWORD *_pdwStubPhase);


/* [helpstring][id][propput] */ HRESULT STDMETHODCALLTYPE ProjectConfigurationProperties_put_RemoteDebugEnabled_Proxy( 
    ProjectConfigurationProperties * This,
    /* [in] */ VARIANT_BOOL bEnableRemoteLaunch);


void __RPC_STUB ProjectConfigurationProperties_put_RemoteDebugEnabled_Stub(
    IRpcStubBuffer *This,
    IRpcChannelBuffer *_pRpcChannelBuffer,
    PRPC_MESSAGE _pRpcMessage,
    DWORD *_pdwStubPhase);


/* [helpstring][id][propget] */ HRESULT STDMETHODCALLTYPE ProjectConfigurationProperties_get_RemoteDebugMachine_Proxy( 
    ProjectConfigurationProperties * This,
    /* [retval][out] */ BSTR *pbstrRemoteLaunchMach);


void __RPC_STUB ProjectConfigurationProperties_get_RemoteDebugMachine_Stub(
    IRpcStubBuffer *This,
    IRpcChannelBuffer *_pRpcChannelBuffer,
    PRPC_MESSAGE _pRpcMessage,
    DWORD *_pdwStubPhase);


/* [helpstring][id][propput] */ HRESULT STDMETHODCALLTYPE ProjectConfigurationProperties_put_RemoteDebugMachine_Proxy( 
    ProjectConfigurationProperties * This,
    /* [in] */ BSTR bstrRemoteLaunchMach);


void __RPC_STUB ProjectConfigurationProperties_put_RemoteDebugMachine_Stub(
    IRpcStubBuffer *This,
    IRpcChannelBuffer *_pRpcChannelBuffer,
    PRPC_MESSAGE _pRpcMessage,
    DWORD *_pdwStubPhase);



#endif 	/* __ProjectConfigurationProperties_INTERFACE_DEFINED__ */


#ifndef __ProjectProperties_INTERFACE_DEFINED__
#define __ProjectProperties_INTERFACE_DEFINED__

/* interface ProjectProperties */
/* [object][dual][unique][helpstring][uuid] */ 


EXTERN_C const IID IID_ProjectProperties;

#if defined(__cplusplus) && !defined(CINTERFACE)
    
    MIDL_INTERFACE("3CDAA65E-1E9D-11d3-B202-00C04F79CACB")
    ProjectProperties : public IDispatch
    {
    public:
        virtual /* [hidden][helpstring][id][propget] */ HRESULT STDMETHODCALLTYPE get___id( 
            /* [retval][out] */ BSTR *pbstrName) = 0;
        
        virtual /* [hidden][id][propget] */ HRESULT STDMETHODCALLTYPE get___project( 
            /* [retval][out] */ IUnknown **ppUnk) = 0;
        
        virtual /* [helpstring][id][propget] */ HRESULT STDMETHODCALLTYPE get_StartupObject( 
            /* [retval][out] */ BSTR *pbstrStartupObject) = 0;
        
        virtual /* [helpstring][id][propput] */ HRESULT STDMETHODCALLTYPE put_StartupObject( 
            /* [in] */ BSTR bstrStartupObject) = 0;
        
        virtual /* [helpstring][id][propget] */ HRESULT STDMETHODCALLTYPE get_OutputType( 
            /* [retval][out] */ prjOutputType *pOutputType) = 0;
        
        virtual /* [helpstring][id][propput] */ HRESULT STDMETHODCALLTYPE put_OutputType( 
            /* [in] */ prjOutputType outputType) = 0;
        
        virtual /* [helpstring][id][propget] */ HRESULT STDMETHODCALLTYPE get_RootNamespace( 
            /* [retval][out] */ BSTR *pbstrRootNamespace) = 0;
        
        virtual /* [helpstring][id][propput] */ HRESULT STDMETHODCALLTYPE put_RootNamespace( 
            /* [in] */ BSTR bstrRootNamespace) = 0;
        
        virtual /* [helpstring][id][propget] */ HRESULT STDMETHODCALLTYPE get_AssemblyName( 
            /* [retval][out] */ BSTR *pbstrAssemblyName) = 0;
        
        virtual /* [helpstring][id][propput] */ HRESULT STDMETHODCALLTYPE put_AssemblyName( 
            /* [in] */ BSTR bstrAssemblyName) = 0;
        
        virtual /* [helpstring][id][propget] */ HRESULT STDMETHODCALLTYPE get_AssemblyOriginatorKeyFile( 
            /* [retval][out] */ BSTR *pbstrOriginatorKeyFile) = 0;
        
        virtual /* [helpstring][id][propput] */ HRESULT STDMETHODCALLTYPE put_AssemblyOriginatorKeyFile( 
            /* [in] */ BSTR bstrOriginatorKeyFile) = 0;
        
        virtual /* [helpstring][id][propget] */ HRESULT STDMETHODCALLTYPE get_AssemblyKeyContainerName( 
            /* [retval][out] */ BSTR *pbstrKeyContainerName) = 0;
        
        virtual /* [helpstring][id][propput] */ HRESULT STDMETHODCALLTYPE put_AssemblyKeyContainerName( 
            /* [in] */ BSTR bstrKeyContainerName) = 0;
        
        virtual /* [helpstring][id][propget] */ HRESULT STDMETHODCALLTYPE get_AssemblyOriginatorKeyMode( 
            /* [retval][out] */ prjOriginatorKeyMode *pOriginatorKeyMode) = 0;
        
        virtual /* [helpstring][id][propput] */ HRESULT STDMETHODCALLTYPE put_AssemblyOriginatorKeyMode( 
            /* [in] */ prjOriginatorKeyMode originatorKeyMode) = 0;
        
        virtual /* [helpstring][id][propget] */ HRESULT STDMETHODCALLTYPE get_DelaySign( 
            /* [retval][out] */ VARIANT_BOOL *pbDelaySign) = 0;
        
        virtual /* [helpstring][id][propput] */ HRESULT STDMETHODCALLTYPE put_DelaySign( 
            /* [in] */ VARIANT_BOOL bDelaySign) = 0;
        
        virtual /* [helpstring][id][propget] */ HRESULT STDMETHODCALLTYPE get_WebServer( 
            /* [retval][out] */ BSTR *pbstrWebServer) = 0;
        
        virtual /* [helpstring][id][propget] */ HRESULT STDMETHODCALLTYPE get_WebServerVersion( 
            /* [retval][out] */ BSTR *pbstrWebServerVersion) = 0;
        
        virtual /* [helpstring][id][propget] */ HRESULT STDMETHODCALLTYPE get_ServerExtensionsVersion( 
            /* [retval][out] */ BSTR *pbstrServerExtensionsVersion) = 0;
        
        virtual /* [helpstring][id][propget] */ HRESULT STDMETHODCALLTYPE get_LinkRepair( 
            /* [retval][out] */ VARIANT_BOOL *pLinkRepair) = 0;
        
        virtual /* [helpstring][id][propput] */ HRESULT STDMETHODCALLTYPE put_LinkRepair( 
            /* [in] */ VARIANT_BOOL linkRepair) = 0;
        
        virtual /* [helpstring][id][propget] */ HRESULT STDMETHODCALLTYPE get_OfflineURL( 
            /* [retval][out] */ BSTR *pbstrOfflineURL) = 0;
        
        virtual /* [helpstring][id][propget] */ HRESULT STDMETHODCALLTYPE get_FileSharePath( 
            /* [retval][out] */ BSTR *pbstrFileSharePath) = 0;
        
        virtual /* [helpstring][id][propput] */ HRESULT STDMETHODCALLTYPE put_FileSharePath( 
            /* [in] */ BSTR bstrFileSharePath) = 0;
        
        virtual /* [helpstring][id][propget] */ HRESULT STDMETHODCALLTYPE get_ActiveFileSharePath( 
            /* [retval][out] */ BSTR *pbstrFileSharePath) = 0;
        
        virtual /* [helpstring][id][propget] */ HRESULT STDMETHODCALLTYPE get_WebAccessMethod( 
            /* [retval][out] */ prjWebAccessMethod *pWebAccessMethod) = 0;
        
        virtual /* [helpstring][id][propput] */ HRESULT STDMETHODCALLTYPE put_WebAccessMethod( 
            /* [in] */ prjWebAccessMethod authoringAccessMethod) = 0;
        
        virtual /* [hidden][helpstring][nonbrowsable][id][propget] */ HRESULT STDMETHODCALLTYPE get_ActiveWebAccessMethod( 
            /* [retval][out] */ prjWebAccessMethod *pActiveWebAccessMethod) = 0;
        
        virtual /* [helpstring][id][propget] */ HRESULT STDMETHODCALLTYPE get_DefaultClientScript( 
            /* [retval][out] */ prjScriptLanguage *pScriptLanguage) = 0;
        
        virtual /* [helpstring][id][propput] */ HRESULT STDMETHODCALLTYPE put_DefaultClientScript( 
            /* [in] */ prjScriptLanguage scriptLanguage) = 0;
        
        virtual /* [helpstring][id][propget] */ HRESULT STDMETHODCALLTYPE get_DefaultTargetSchema( 
            /* [retval][out] */ prjTargetSchema *pTargetSchema) = 0;
        
        virtual /* [helpstring][id][propput] */ HRESULT STDMETHODCALLTYPE put_DefaultTargetSchema( 
            /* [in] */ prjTargetSchema htmlPlatform) = 0;
        
        virtual /* [helpstring][id][propget] */ HRESULT STDMETHODCALLTYPE get_DefaultHTMLPageLayout( 
            /* [retval][out] */ prjHTMLPageLayout *pHTMLPageLayout) = 0;
        
        virtual /* [helpstring][id][propput] */ HRESULT STDMETHODCALLTYPE put_DefaultHTMLPageLayout( 
            /* [in] */ prjHTMLPageLayout htmlPageLayout) = 0;
        
        virtual /* [helpstring][id][propget] */ HRESULT STDMETHODCALLTYPE get_FileName( 
            /* [retval][out] */ BSTR *pbstrFileName) = 0;
        
        virtual /* [helpstring][id][propput] */ HRESULT STDMETHODCALLTYPE put_FileName( 
            /* [in] */ BSTR bstrFileName) = 0;
        
        virtual /* [helpstring][id][propget] */ HRESULT STDMETHODCALLTYPE get_FullPath( 
            /* [retval][out] */ BSTR *pbstrFullPath) = 0;
        
        virtual /* [helpstring][id][propget] */ HRESULT STDMETHODCALLTYPE get_LocalPath( 
            /* [retval][out] */ BSTR *pbstrLocalPath) = 0;
        
        virtual /* [helpstring][id][propget] */ HRESULT STDMETHODCALLTYPE get_URL( 
            /* [retval][out] */ BSTR *pbstrURL) = 0;
        
        virtual /* [hidden][helpstring][nonbrowsable][id][propget] */ HRESULT STDMETHODCALLTYPE get_ActiveConfigurationSettings( 
            /* [retval][out] */ ProjectConfigurationProperties **ppVBProjConfigProps) = 0;
        
        virtual /* [hidden][helpstring][nonbrowsable][id][propget] */ HRESULT STDMETHODCALLTYPE get_Extender( 
            /* [in] */ BSTR ExtenderName,
            /* [retval][out] */ IDispatch **Extender) = 0;
        
        virtual /* [hidden][helpstring][nonbrowsable][id][propget] */ HRESULT STDMETHODCALLTYPE get_ExtenderNames( 
            /* [retval][out] */ VARIANT *ExtenderNames) = 0;
        
        virtual /* [hidden][helpstring][nonbrowsable][id][propget] */ HRESULT STDMETHODCALLTYPE get_ExtenderCATID( 
            /* [retval][out] */ BSTR *pRetval) = 0;
        
        virtual /* [helpstring][id][propget] */ HRESULT STDMETHODCALLTYPE get_ApplicationIcon( 
            /* [retval][out] */ BSTR *pbstrApplicationIcon) = 0;
        
        virtual /* [helpstring][id][propput] */ HRESULT STDMETHODCALLTYPE put_ApplicationIcon( 
            /* [in] */ BSTR bstrApplicationIcon) = 0;
        
        virtual /* [helpstring][id][propget] */ HRESULT STDMETHODCALLTYPE get_OptionStrict( 
            /* [retval][out] */ prjOptionStrict *pOptionStrict) = 0;
        
        virtual /* [helpstring][id][propput] */ HRESULT STDMETHODCALLTYPE put_OptionStrict( 
            /* [in] */ prjOptionStrict optionStrict) = 0;
        
        virtual /* [helpstring][id][propget] */ HRESULT STDMETHODCALLTYPE get_ReferencePath( 
            /* [retval][out] */ BSTR *pbstrReferencePath) = 0;
        
        virtual /* [helpstring][id][propput] */ HRESULT STDMETHODCALLTYPE put_ReferencePath( 
            /* [in] */ BSTR bstrReferencePath) = 0;
        
        virtual /* [helpstring][id][propget] */ HRESULT STDMETHODCALLTYPE get_OutputFileName( 
            /* [retval][out] */ BSTR *pbstrOutputFileName) = 0;
        
        virtual /* [hidden][helpstring][id][propget] */ HRESULT STDMETHODCALLTYPE get_AbsoluteProjectDirectory( 
            /* [retval][out] */ BSTR *pbstrDir) = 0;
        
        virtual /* [helpstring][id][propget] */ HRESULT STDMETHODCALLTYPE get_OptionExplicit( 
            /* [retval][out] */ prjOptionExplicit *pOptionExplicit) = 0;
        
        virtual /* [helpstring][id][propput] */ HRESULT STDMETHODCALLTYPE put_OptionExplicit( 
            /* [in] */ prjOptionExplicit optionExplicit) = 0;
        
        virtual /* [helpstring][id][propget] */ HRESULT STDMETHODCALLTYPE get_OptionCompare( 
            /* [retval][out] */ prjCompare *pOptionCompare) = 0;
        
        virtual /* [helpstring][id][propput] */ HRESULT STDMETHODCALLTYPE put_OptionCompare( 
            /* [in] */ prjCompare optionCompare) = 0;
        
        virtual /* [helpstring][id][propget] */ HRESULT STDMETHODCALLTYPE get_ProjectType( 
            /* [retval][out] */ prjProjectType *pProjectType) = 0;
        
        virtual /* [helpstring][id][propget] */ HRESULT STDMETHODCALLTYPE get_DefaultNamespace( 
            /* [retval][out] */ BSTR *pbstrRootNamespace) = 0;
        
        virtual /* [helpstring][id][propput] */ HRESULT STDMETHODCALLTYPE put_DefaultNamespace( 
            /* [in] */ BSTR bstrRootNamespace) = 0;
        
    };
    
#else 	/* C style interface */

    typedef struct ProjectPropertiesVtbl
    {
        BEGIN_INTERFACE
        
        HRESULT ( STDMETHODCALLTYPE *QueryInterface )( 
            ProjectProperties * This,
            /* [in] */ REFIID riid,
            /* [iid_is][out] */ void **ppvObject);
        
        ULONG ( STDMETHODCALLTYPE *AddRef )( 
            ProjectProperties * This);
        
        ULONG ( STDMETHODCALLTYPE *Release )( 
            ProjectProperties * This);
        
        HRESULT ( STDMETHODCALLTYPE *GetTypeInfoCount )( 
            ProjectProperties * This,
            /* [out] */ UINT *pctinfo);
        
        HRESULT ( STDMETHODCALLTYPE *GetTypeInfo )( 
            ProjectProperties * This,
            /* [in] */ UINT iTInfo,
            /* [in] */ LCID lcid,
            /* [out] */ ITypeInfo **ppTInfo);
        
        HRESULT ( STDMETHODCALLTYPE *GetIDsOfNames )( 
            ProjectProperties * This,
            /* [in] */ REFIID riid,
            /* [size_is][in] */ LPOLESTR *rgszNames,
            /* [in] */ UINT cNames,
            /* [in] */ LCID lcid,
            /* [size_is][out] */ DISPID *rgDispId);
        
        /* [local] */ HRESULT ( STDMETHODCALLTYPE *Invoke )( 
            ProjectProperties * This,
            /* [in] */ DISPID dispIdMember,
            /* [in] */ REFIID riid,
            /* [in] */ LCID lcid,
            /* [in] */ WORD wFlags,
            /* [out][in] */ DISPPARAMS *pDispParams,
            /* [out] */ VARIANT *pVarResult,
            /* [out] */ EXCEPINFO *pExcepInfo,
            /* [out] */ UINT *puArgErr);
        
        /* [hidden][helpstring][id][propget] */ HRESULT ( STDMETHODCALLTYPE *get___id )( 
            ProjectProperties * This,
            /* [retval][out] */ BSTR *pbstrName);
        
        /* [hidden][id][propget] */ HRESULT ( STDMETHODCALLTYPE *get___project )( 
            ProjectProperties * This,
            /* [retval][out] */ IUnknown **ppUnk);
        
        /* [helpstring][id][propget] */ HRESULT ( STDMETHODCALLTYPE *get_StartupObject )( 
            ProjectProperties * This,
            /* [retval][out] */ BSTR *pbstrStartupObject);
        
        /* [helpstring][id][propput] */ HRESULT ( STDMETHODCALLTYPE *put_StartupObject )( 
            ProjectProperties * This,
            /* [in] */ BSTR bstrStartupObject);
        
        /* [helpstring][id][propget] */ HRESULT ( STDMETHODCALLTYPE *get_OutputType )( 
            ProjectProperties * This,
            /* [retval][out] */ prjOutputType *pOutputType);
        
        /* [helpstring][id][propput] */ HRESULT ( STDMETHODCALLTYPE *put_OutputType )( 
            ProjectProperties * This,
            /* [in] */ prjOutputType outputType);
        
        /* [helpstring][id][propget] */ HRESULT ( STDMETHODCALLTYPE *get_RootNamespace )( 
            ProjectProperties * This,
            /* [retval][out] */ BSTR *pbstrRootNamespace);
        
        /* [helpstring][id][propput] */ HRESULT ( STDMETHODCALLTYPE *put_RootNamespace )( 
            ProjectProperties * This,
            /* [in] */ BSTR bstrRootNamespace);
        
        /* [helpstring][id][propget] */ HRESULT ( STDMETHODCALLTYPE *get_AssemblyName )( 
            ProjectProperties * This,
            /* [retval][out] */ BSTR *pbstrAssemblyName);
        
        /* [helpstring][id][propput] */ HRESULT ( STDMETHODCALLTYPE *put_AssemblyName )( 
            ProjectProperties * This,
            /* [in] */ BSTR bstrAssemblyName);
        
        /* [helpstring][id][propget] */ HRESULT ( STDMETHODCALLTYPE *get_AssemblyOriginatorKeyFile )( 
            ProjectProperties * This,
            /* [retval][out] */ BSTR *pbstrOriginatorKeyFile);
        
        /* [helpstring][id][propput] */ HRESULT ( STDMETHODCALLTYPE *put_AssemblyOriginatorKeyFile )( 
            ProjectProperties * This,
            /* [in] */ BSTR bstrOriginatorKeyFile);
        
        /* [helpstring][id][propget] */ HRESULT ( STDMETHODCALLTYPE *get_AssemblyKeyContainerName )( 
            ProjectProperties * This,
            /* [retval][out] */ BSTR *pbstrKeyContainerName);
        
        /* [helpstring][id][propput] */ HRESULT ( STDMETHODCALLTYPE *put_AssemblyKeyContainerName )( 
            ProjectProperties * This,
            /* [in] */ BSTR bstrKeyContainerName);
        
        /* [helpstring][id][propget] */ HRESULT ( STDMETHODCALLTYPE *get_AssemblyOriginatorKeyMode )( 
            ProjectProperties * This,
            /* [retval][out] */ prjOriginatorKeyMode *pOriginatorKeyMode);
        
        /* [helpstring][id][propput] */ HRESULT ( STDMETHODCALLTYPE *put_AssemblyOriginatorKeyMode )( 
            ProjectProperties * This,
            /* [in] */ prjOriginatorKeyMode originatorKeyMode);
        
        /* [helpstring][id][propget] */ HRESULT ( STDMETHODCALLTYPE *get_DelaySign )( 
            ProjectProperties * This,
            /* [retval][out] */ VARIANT_BOOL *pbDelaySign);
        
        /* [helpstring][id][propput] */ HRESULT ( STDMETHODCALLTYPE *put_DelaySign )( 
            ProjectProperties * This,
            /* [in] */ VARIANT_BOOL bDelaySign);
        
        /* [helpstring][id][propget] */ HRESULT ( STDMETHODCALLTYPE *get_WebServer )( 
            ProjectProperties * This,
            /* [retval][out] */ BSTR *pbstrWebServer);
        
        /* [helpstring][id][propget] */ HRESULT ( STDMETHODCALLTYPE *get_WebServerVersion )( 
            ProjectProperties * This,
            /* [retval][out] */ BSTR *pbstrWebServerVersion);
        
        /* [helpstring][id][propget] */ HRESULT ( STDMETHODCALLTYPE *get_ServerExtensionsVersion )( 
            ProjectProperties * This,
            /* [retval][out] */ BSTR *pbstrServerExtensionsVersion);
        
        /* [helpstring][id][propget] */ HRESULT ( STDMETHODCALLTYPE *get_LinkRepair )( 
            ProjectProperties * This,
            /* [retval][out] */ VARIANT_BOOL *pLinkRepair);
        
        /* [helpstring][id][propput] */ HRESULT ( STDMETHODCALLTYPE *put_LinkRepair )( 
            ProjectProperties * This,
            /* [in] */ VARIANT_BOOL linkRepair);
        
        /* [helpstring][id][propget] */ HRESULT ( STDMETHODCALLTYPE *get_OfflineURL )( 
            ProjectProperties * This,
            /* [retval][out] */ BSTR *pbstrOfflineURL);
        
        /* [helpstring][id][propget] */ HRESULT ( STDMETHODCALLTYPE *get_FileSharePath )( 
            ProjectProperties * This,
            /* [retval][out] */ BSTR *pbstrFileSharePath);
        
        /* [helpstring][id][propput] */ HRESULT ( STDMETHODCALLTYPE *put_FileSharePath )( 
            ProjectProperties * This,
            /* [in] */ BSTR bstrFileSharePath);
        
        /* [helpstring][id][propget] */ HRESULT ( STDMETHODCALLTYPE *get_ActiveFileSharePath )( 
            ProjectProperties * This,
            /* [retval][out] */ BSTR *pbstrFileSharePath);
        
        /* [helpstring][id][propget] */ HRESULT ( STDMETHODCALLTYPE *get_WebAccessMethod )( 
            ProjectProperties * This,
            /* [retval][out] */ prjWebAccessMethod *pWebAccessMethod);
        
        /* [helpstring][id][propput] */ HRESULT ( STDMETHODCALLTYPE *put_WebAccessMethod )( 
            ProjectProperties * This,
            /* [in] */ prjWebAccessMethod authoringAccessMethod);
        
        /* [hidden][helpstring][nonbrowsable][id][propget] */ HRESULT ( STDMETHODCALLTYPE *get_ActiveWebAccessMethod )( 
            ProjectProperties * This,
            /* [retval][out] */ prjWebAccessMethod *pActiveWebAccessMethod);
        
        /* [helpstring][id][propget] */ HRESULT ( STDMETHODCALLTYPE *get_DefaultClientScript )( 
            ProjectProperties * This,
            /* [retval][out] */ prjScriptLanguage *pScriptLanguage);
        
        /* [helpstring][id][propput] */ HRESULT ( STDMETHODCALLTYPE *put_DefaultClientScript )( 
            ProjectProperties * This,
            /* [in] */ prjScriptLanguage scriptLanguage);
        
        /* [helpstring][id][propget] */ HRESULT ( STDMETHODCALLTYPE *get_DefaultTargetSchema )( 
            ProjectProperties * This,
            /* [retval][out] */ prjTargetSchema *pTargetSchema);
        
        /* [helpstring][id][propput] */ HRESULT ( STDMETHODCALLTYPE *put_DefaultTargetSchema )( 
            ProjectProperties * This,
            /* [in] */ prjTargetSchema htmlPlatform);
        
        /* [helpstring][id][propget] */ HRESULT ( STDMETHODCALLTYPE *get_DefaultHTMLPageLayout )( 
            ProjectProperties * This,
            /* [retval][out] */ prjHTMLPageLayout *pHTMLPageLayout);
        
        /* [helpstring][id][propput] */ HRESULT ( STDMETHODCALLTYPE *put_DefaultHTMLPageLayout )( 
            ProjectProperties * This,
            /* [in] */ prjHTMLPageLayout htmlPageLayout);
        
        /* [helpstring][id][propget] */ HRESULT ( STDMETHODCALLTYPE *get_FileName )( 
            ProjectProperties * This,
            /* [retval][out] */ BSTR *pbstrFileName);
        
        /* [helpstring][id][propput] */ HRESULT ( STDMETHODCALLTYPE *put_FileName )( 
            ProjectProperties * This,
            /* [in] */ BSTR bstrFileName);
        
        /* [helpstring][id][propget] */ HRESULT ( STDMETHODCALLTYPE *get_FullPath )( 
            ProjectProperties * This,
            /* [retval][out] */ BSTR *pbstrFullPath);
        
        /* [helpstring][id][propget] */ HRESULT ( STDMETHODCALLTYPE *get_LocalPath )( 
            ProjectProperties * This,
            /* [retval][out] */ BSTR *pbstrLocalPath);
        
        /* [helpstring][id][propget] */ HRESULT ( STDMETHODCALLTYPE *get_URL )( 
            ProjectProperties * This,
            /* [retval][out] */ BSTR *pbstrURL);
        
        /* [hidden][helpstring][nonbrowsable][id][propget] */ HRESULT ( STDMETHODCALLTYPE *get_ActiveConfigurationSettings )( 
            ProjectProperties * This,
            /* [retval][out] */ ProjectConfigurationProperties **ppVBProjConfigProps);
        
        /* [hidden][helpstring][nonbrowsable][id][propget] */ HRESULT ( STDMETHODCALLTYPE *get_Extender )( 
            ProjectProperties * This,
            /* [in] */ BSTR ExtenderName,
            /* [retval][out] */ IDispatch **Extender);
        
        /* [hidden][helpstring][nonbrowsable][id][propget] */ HRESULT ( STDMETHODCALLTYPE *get_ExtenderNames )( 
            ProjectProperties * This,
            /* [retval][out] */ VARIANT *ExtenderNames);
        
        /* [hidden][helpstring][nonbrowsable][id][propget] */ HRESULT ( STDMETHODCALLTYPE *get_ExtenderCATID )( 
            ProjectProperties * This,
            /* [retval][out] */ BSTR *pRetval);
        
        /* [helpstring][id][propget] */ HRESULT ( STDMETHODCALLTYPE *get_ApplicationIcon )( 
            ProjectProperties * This,
            /* [retval][out] */ BSTR *pbstrApplicationIcon);
        
        /* [helpstring][id][propput] */ HRESULT ( STDMETHODCALLTYPE *put_ApplicationIcon )( 
            ProjectProperties * This,
            /* [in] */ BSTR bstrApplicationIcon);
        
        /* [helpstring][id][propget] */ HRESULT ( STDMETHODCALLTYPE *get_OptionStrict )( 
            ProjectProperties * This,
            /* [retval][out] */ prjOptionStrict *pOptionStrict);
        
        /* [helpstring][id][propput] */ HRESULT ( STDMETHODCALLTYPE *put_OptionStrict )( 
            ProjectProperties * This,
            /* [in] */ prjOptionStrict optionStrict);
        
        /* [helpstring][id][propget] */ HRESULT ( STDMETHODCALLTYPE *get_ReferencePath )( 
            ProjectProperties * This,
            /* [retval][out] */ BSTR *pbstrReferencePath);
        
        /* [helpstring][id][propput] */ HRESULT ( STDMETHODCALLTYPE *put_ReferencePath )( 
            ProjectProperties * This,
            /* [in] */ BSTR bstrReferencePath);
        
        /* [helpstring][id][propget] */ HRESULT ( STDMETHODCALLTYPE *get_OutputFileName )( 
            ProjectProperties * This,
            /* [retval][out] */ BSTR *pbstrOutputFileName);
        
        /* [hidden][helpstring][id][propget] */ HRESULT ( STDMETHODCALLTYPE *get_AbsoluteProjectDirectory )( 
            ProjectProperties * This,
            /* [retval][out] */ BSTR *pbstrDir);
        
        /* [helpstring][id][propget] */ HRESULT ( STDMETHODCALLTYPE *get_OptionExplicit )( 
            ProjectProperties * This,
            /* [retval][out] */ prjOptionExplicit *pOptionExplicit);
        
        /* [helpstring][id][propput] */ HRESULT ( STDMETHODCALLTYPE *put_OptionExplicit )( 
            ProjectProperties * This,
            /* [in] */ prjOptionExplicit optionExplicit);
        
        /* [helpstring][id][propget] */ HRESULT ( STDMETHODCALLTYPE *get_OptionCompare )( 
            ProjectProperties * This,
            /* [retval][out] */ prjCompare *pOptionCompare);
        
        /* [helpstring][id][propput] */ HRESULT ( STDMETHODCALLTYPE *put_OptionCompare )( 
            ProjectProperties * This,
            /* [in] */ prjCompare optionCompare);
        
        /* [helpstring][id][propget] */ HRESULT ( STDMETHODCALLTYPE *get_ProjectType )( 
            ProjectProperties * This,
            /* [retval][out] */ prjProjectType *pProjectType);
        
        /* [helpstring][id][propget] */ HRESULT ( STDMETHODCALLTYPE *get_DefaultNamespace )( 
            ProjectProperties * This,
            /* [retval][out] */ BSTR *pbstrRootNamespace);
        
        /* [helpstring][id][propput] */ HRESULT ( STDMETHODCALLTYPE *put_DefaultNamespace )( 
            ProjectProperties * This,
            /* [in] */ BSTR bstrRootNamespace);
        
        END_INTERFACE
    } ProjectPropertiesVtbl;

    interface ProjectProperties
    {
        CONST_VTBL struct ProjectPropertiesVtbl *lpVtbl;
    };

    

#ifdef COBJMACROS


#define ProjectProperties_QueryInterface(This,riid,ppvObject)	\
    (This)->lpVtbl -> QueryInterface(This,riid,ppvObject)

#define ProjectProperties_AddRef(This)	\
    (This)->lpVtbl -> AddRef(This)

#define ProjectProperties_Release(This)	\
    (This)->lpVtbl -> Release(This)


#define ProjectProperties_GetTypeInfoCount(This,pctinfo)	\
    (This)->lpVtbl -> GetTypeInfoCount(This,pctinfo)

#define ProjectProperties_GetTypeInfo(This,iTInfo,lcid,ppTInfo)	\
    (This)->lpVtbl -> GetTypeInfo(This,iTInfo,lcid,ppTInfo)

#define ProjectProperties_GetIDsOfNames(This,riid,rgszNames,cNames,lcid,rgDispId)	\
    (This)->lpVtbl -> GetIDsOfNames(This,riid,rgszNames,cNames,lcid,rgDispId)

#define ProjectProperties_Invoke(This,dispIdMember,riid,lcid,wFlags,pDispParams,pVarResult,pExcepInfo,puArgErr)	\
    (This)->lpVtbl -> Invoke(This,dispIdMember,riid,lcid,wFlags,pDispParams,pVarResult,pExcepInfo,puArgErr)


#define ProjectProperties_get___id(This,pbstrName)	\
    (This)->lpVtbl -> get___id(This,pbstrName)

#define ProjectProperties_get___project(This,ppUnk)	\
    (This)->lpVtbl -> get___project(This,ppUnk)

#define ProjectProperties_get_StartupObject(This,pbstrStartupObject)	\
    (This)->lpVtbl -> get_StartupObject(This,pbstrStartupObject)

#define ProjectProperties_put_StartupObject(This,bstrStartupObject)	\
    (This)->lpVtbl -> put_StartupObject(This,bstrStartupObject)

#define ProjectProperties_get_OutputType(This,pOutputType)	\
    (This)->lpVtbl -> get_OutputType(This,pOutputType)

#define ProjectProperties_put_OutputType(This,outputType)	\
    (This)->lpVtbl -> put_OutputType(This,outputType)

#define ProjectProperties_get_RootNamespace(This,pbstrRootNamespace)	\
    (This)->lpVtbl -> get_RootNamespace(This,pbstrRootNamespace)

#define ProjectProperties_put_RootNamespace(This,bstrRootNamespace)	\
    (This)->lpVtbl -> put_RootNamespace(This,bstrRootNamespace)

#define ProjectProperties_get_AssemblyName(This,pbstrAssemblyName)	\
    (This)->lpVtbl -> get_AssemblyName(This,pbstrAssemblyName)

#define ProjectProperties_put_AssemblyName(This,bstrAssemblyName)	\
    (This)->lpVtbl -> put_AssemblyName(This,bstrAssemblyName)

#define ProjectProperties_get_AssemblyOriginatorKeyFile(This,pbstrOriginatorKeyFile)	\
    (This)->lpVtbl -> get_AssemblyOriginatorKeyFile(This,pbstrOriginatorKeyFile)

#define ProjectProperties_put_AssemblyOriginatorKeyFile(This,bstrOriginatorKeyFile)	\
    (This)->lpVtbl -> put_AssemblyOriginatorKeyFile(This,bstrOriginatorKeyFile)

#define ProjectProperties_get_AssemblyKeyContainerName(This,pbstrKeyContainerName)	\
    (This)->lpVtbl -> get_AssemblyKeyContainerName(This,pbstrKeyContainerName)

#define ProjectProperties_put_AssemblyKeyContainerName(This,bstrKeyContainerName)	\
    (This)->lpVtbl -> put_AssemblyKeyContainerName(This,bstrKeyContainerName)

#define ProjectProperties_get_AssemblyOriginatorKeyMode(This,pOriginatorKeyMode)	\
    (This)->lpVtbl -> get_AssemblyOriginatorKeyMode(This,pOriginatorKeyMode)

#define ProjectProperties_put_AssemblyOriginatorKeyMode(This,originatorKeyMode)	\
    (This)->lpVtbl -> put_AssemblyOriginatorKeyMode(This,originatorKeyMode)

#define ProjectProperties_get_DelaySign(This,pbDelaySign)	\
    (This)->lpVtbl -> get_DelaySign(This,pbDelaySign)

#define ProjectProperties_put_DelaySign(This,bDelaySign)	\
    (This)->lpVtbl -> put_DelaySign(This,bDelaySign)

#define ProjectProperties_get_WebServer(This,pbstrWebServer)	\
    (This)->lpVtbl -> get_WebServer(This,pbstrWebServer)

#define ProjectProperties_get_WebServerVersion(This,pbstrWebServerVersion)	\
    (This)->lpVtbl -> get_WebServerVersion(This,pbstrWebServerVersion)

#define ProjectProperties_get_ServerExtensionsVersion(This,pbstrServerExtensionsVersion)	\
    (This)->lpVtbl -> get_ServerExtensionsVersion(This,pbstrServerExtensionsVersion)

#define ProjectProperties_get_LinkRepair(This,pLinkRepair)	\
    (This)->lpVtbl -> get_LinkRepair(This,pLinkRepair)

#define ProjectProperties_put_LinkRepair(This,linkRepair)	\
    (This)->lpVtbl -> put_LinkRepair(This,linkRepair)

#define ProjectProperties_get_OfflineURL(This,pbstrOfflineURL)	\
    (This)->lpVtbl -> get_OfflineURL(This,pbstrOfflineURL)

#define ProjectProperties_get_FileSharePath(This,pbstrFileSharePath)	\
    (This)->lpVtbl -> get_FileSharePath(This,pbstrFileSharePath)

#define ProjectProperties_put_FileSharePath(This,bstrFileSharePath)	\
    (This)->lpVtbl -> put_FileSharePath(This,bstrFileSharePath)

#define ProjectProperties_get_ActiveFileSharePath(This,pbstrFileSharePath)	\
    (This)->lpVtbl -> get_ActiveFileSharePath(This,pbstrFileSharePath)

#define ProjectProperties_get_WebAccessMethod(This,pWebAccessMethod)	\
    (This)->lpVtbl -> get_WebAccessMethod(This,pWebAccessMethod)

#define ProjectProperties_put_WebAccessMethod(This,authoringAccessMethod)	\
    (This)->lpVtbl -> put_WebAccessMethod(This,authoringAccessMethod)

#define ProjectProperties_get_ActiveWebAccessMethod(This,pActiveWebAccessMethod)	\
    (This)->lpVtbl -> get_ActiveWebAccessMethod(This,pActiveWebAccessMethod)

#define ProjectProperties_get_DefaultClientScript(This,pScriptLanguage)	\
    (This)->lpVtbl -> get_DefaultClientScript(This,pScriptLanguage)

#define ProjectProperties_put_DefaultClientScript(This,scriptLanguage)	\
    (This)->lpVtbl -> put_DefaultClientScript(This,scriptLanguage)

#define ProjectProperties_get_DefaultTargetSchema(This,pTargetSchema)	\
    (This)->lpVtbl -> get_DefaultTargetSchema(This,pTargetSchema)

#define ProjectProperties_put_DefaultTargetSchema(This,htmlPlatform)	\
    (This)->lpVtbl -> put_DefaultTargetSchema(This,htmlPlatform)

#define ProjectProperties_get_DefaultHTMLPageLayout(This,pHTMLPageLayout)	\
    (This)->lpVtbl -> get_DefaultHTMLPageLayout(This,pHTMLPageLayout)

#define ProjectProperties_put_DefaultHTMLPageLayout(This,htmlPageLayout)	\
    (This)->lpVtbl -> put_DefaultHTMLPageLayout(This,htmlPageLayout)

#define ProjectProperties_get_FileName(This,pbstrFileName)	\
    (This)->lpVtbl -> get_FileName(This,pbstrFileName)

#define ProjectProperties_put_FileName(This,bstrFileName)	\
    (This)->lpVtbl -> put_FileName(This,bstrFileName)

#define ProjectProperties_get_FullPath(This,pbstrFullPath)	\
    (This)->lpVtbl -> get_FullPath(This,pbstrFullPath)

#define ProjectProperties_get_LocalPath(This,pbstrLocalPath)	\
    (This)->lpVtbl -> get_LocalPath(This,pbstrLocalPath)

#define ProjectProperties_get_URL(This,pbstrURL)	\
    (This)->lpVtbl -> get_URL(This,pbstrURL)

#define ProjectProperties_get_ActiveConfigurationSettings(This,ppVBProjConfigProps)	\
    (This)->lpVtbl -> get_ActiveConfigurationSettings(This,ppVBProjConfigProps)

#define ProjectProperties_get_Extender(This,ExtenderName,Extender)	\
    (This)->lpVtbl -> get_Extender(This,ExtenderName,Extender)

#define ProjectProperties_get_ExtenderNames(This,ExtenderNames)	\
    (This)->lpVtbl -> get_ExtenderNames(This,ExtenderNames)

#define ProjectProperties_get_ExtenderCATID(This,pRetval)	\
    (This)->lpVtbl -> get_ExtenderCATID(This,pRetval)

#define ProjectProperties_get_ApplicationIcon(This,pbstrApplicationIcon)	\
    (This)->lpVtbl -> get_ApplicationIcon(This,pbstrApplicationIcon)

#define ProjectProperties_put_ApplicationIcon(This,bstrApplicationIcon)	\
    (This)->lpVtbl -> put_ApplicationIcon(This,bstrApplicationIcon)

#define ProjectProperties_get_OptionStrict(This,pOptionStrict)	\
    (This)->lpVtbl -> get_OptionStrict(This,pOptionStrict)

#define ProjectProperties_put_OptionStrict(This,optionStrict)	\
    (This)->lpVtbl -> put_OptionStrict(This,optionStrict)

#define ProjectProperties_get_ReferencePath(This,pbstrReferencePath)	\
    (This)->lpVtbl -> get_ReferencePath(This,pbstrReferencePath)

#define ProjectProperties_put_ReferencePath(This,bstrReferencePath)	\
    (This)->lpVtbl -> put_ReferencePath(This,bstrReferencePath)

#define ProjectProperties_get_OutputFileName(This,pbstrOutputFileName)	\
    (This)->lpVtbl -> get_OutputFileName(This,pbstrOutputFileName)

#define ProjectProperties_get_AbsoluteProjectDirectory(This,pbstrDir)	\
    (This)->lpVtbl -> get_AbsoluteProjectDirectory(This,pbstrDir)

#define ProjectProperties_get_OptionExplicit(This,pOptionExplicit)	\
    (This)->lpVtbl -> get_OptionExplicit(This,pOptionExplicit)

#define ProjectProperties_put_OptionExplicit(This,optionExplicit)	\
    (This)->lpVtbl -> put_OptionExplicit(This,optionExplicit)

#define ProjectProperties_get_OptionCompare(This,pOptionCompare)	\
    (This)->lpVtbl -> get_OptionCompare(This,pOptionCompare)

#define ProjectProperties_put_OptionCompare(This,optionCompare)	\
    (This)->lpVtbl -> put_OptionCompare(This,optionCompare)

#define ProjectProperties_get_ProjectType(This,pProjectType)	\
    (This)->lpVtbl -> get_ProjectType(This,pProjectType)

#define ProjectProperties_get_DefaultNamespace(This,pbstrRootNamespace)	\
    (This)->lpVtbl -> get_DefaultNamespace(This,pbstrRootNamespace)

#define ProjectProperties_put_DefaultNamespace(This,bstrRootNamespace)	\
    (This)->lpVtbl -> put_DefaultNamespace(This,bstrRootNamespace)

#endif /* COBJMACROS */


#endif 	/* C style interface */



/* [hidden][helpstring][id][propget] */ HRESULT STDMETHODCALLTYPE ProjectProperties_get___id_Proxy( 
    ProjectProperties * This,
    /* [retval][out] */ BSTR *pbstrName);


void __RPC_STUB ProjectProperties_get___id_Stub(
    IRpcStubBuffer *This,
    IRpcChannelBuffer *_pRpcChannelBuffer,
    PRPC_MESSAGE _pRpcMessage,
    DWORD *_pdwStubPhase);


/* [hidden][id][propget] */ HRESULT STDMETHODCALLTYPE ProjectProperties_get___project_Proxy( 
    ProjectProperties * This,
    /* [retval][out] */ IUnknown **ppUnk);


void __RPC_STUB ProjectProperties_get___project_Stub(
    IRpcStubBuffer *This,
    IRpcChannelBuffer *_pRpcChannelBuffer,
    PRPC_MESSAGE _pRpcMessage,
    DWORD *_pdwStubPhase);


/* [helpstring][id][propget] */ HRESULT STDMETHODCALLTYPE ProjectProperties_get_StartupObject_Proxy( 
    ProjectProperties * This,
    /* [retval][out] */ BSTR *pbstrStartupObject);


void __RPC_STUB ProjectProperties_get_StartupObject_Stub(
    IRpcStubBuffer *This,
    IRpcChannelBuffer *_pRpcChannelBuffer,
    PRPC_MESSAGE _pRpcMessage,
    DWORD *_pdwStubPhase);


/* [helpstring][id][propput] */ HRESULT STDMETHODCALLTYPE ProjectProperties_put_StartupObject_Proxy( 
    ProjectProperties * This,
    /* [in] */ BSTR bstrStartupObject);


void __RPC_STUB ProjectProperties_put_StartupObject_Stub(
    IRpcStubBuffer *This,
    IRpcChannelBuffer *_pRpcChannelBuffer,
    PRPC_MESSAGE _pRpcMessage,
    DWORD *_pdwStubPhase);


/* [helpstring][id][propget] */ HRESULT STDMETHODCALLTYPE ProjectProperties_get_OutputType_Proxy( 
    ProjectProperties * This,
    /* [retval][out] */ prjOutputType *pOutputType);


void __RPC_STUB ProjectProperties_get_OutputType_Stub(
    IRpcStubBuffer *This,
    IRpcChannelBuffer *_pRpcChannelBuffer,
    PRPC_MESSAGE _pRpcMessage,
    DWORD *_pdwStubPhase);


/* [helpstring][id][propput] */ HRESULT STDMETHODCALLTYPE ProjectProperties_put_OutputType_Proxy( 
    ProjectProperties * This,
    /* [in] */ prjOutputType outputType);


void __RPC_STUB ProjectProperties_put_OutputType_Stub(
    IRpcStubBuffer *This,
    IRpcChannelBuffer *_pRpcChannelBuffer,
    PRPC_MESSAGE _pRpcMessage,
    DWORD *_pdwStubPhase);


/* [helpstring][id][propget] */ HRESULT STDMETHODCALLTYPE ProjectProperties_get_RootNamespace_Proxy( 
    ProjectProperties * This,
    /* [retval][out] */ BSTR *pbstrRootNamespace);


void __RPC_STUB ProjectProperties_get_RootNamespace_Stub(
    IRpcStubBuffer *This,
    IRpcChannelBuffer *_pRpcChannelBuffer,
    PRPC_MESSAGE _pRpcMessage,
    DWORD *_pdwStubPhase);


/* [helpstring][id][propput] */ HRESULT STDMETHODCALLTYPE ProjectProperties_put_RootNamespace_Proxy( 
    ProjectProperties * This,
    /* [in] */ BSTR bstrRootNamespace);


void __RPC_STUB ProjectProperties_put_RootNamespace_Stub(
    IRpcStubBuffer *This,
    IRpcChannelBuffer *_pRpcChannelBuffer,
    PRPC_MESSAGE _pRpcMessage,
    DWORD *_pdwStubPhase);


/* [helpstring][id][propget] */ HRESULT STDMETHODCALLTYPE ProjectProperties_get_AssemblyName_Proxy( 
    ProjectProperties * This,
    /* [retval][out] */ BSTR *pbstrAssemblyName);


void __RPC_STUB ProjectProperties_get_AssemblyName_Stub(
    IRpcStubBuffer *This,
    IRpcChannelBuffer *_pRpcChannelBuffer,
    PRPC_MESSAGE _pRpcMessage,
    DWORD *_pdwStubPhase);


/* [helpstring][id][propput] */ HRESULT STDMETHODCALLTYPE ProjectProperties_put_AssemblyName_Proxy( 
    ProjectProperties * This,
    /* [in] */ BSTR bstrAssemblyName);


void __RPC_STUB ProjectProperties_put_AssemblyName_Stub(
    IRpcStubBuffer *This,
    IRpcChannelBuffer *_pRpcChannelBuffer,
    PRPC_MESSAGE _pRpcMessage,
    DWORD *_pdwStubPhase);


/* [helpstring][id][propget] */ HRESULT STDMETHODCALLTYPE ProjectProperties_get_AssemblyOriginatorKeyFile_Proxy( 
    ProjectProperties * This,
    /* [retval][out] */ BSTR *pbstrOriginatorKeyFile);


void __RPC_STUB ProjectProperties_get_AssemblyOriginatorKeyFile_Stub(
    IRpcStubBuffer *This,
    IRpcChannelBuffer *_pRpcChannelBuffer,
    PRPC_MESSAGE _pRpcMessage,
    DWORD *_pdwStubPhase);


/* [helpstring][id][propput] */ HRESULT STDMETHODCALLTYPE ProjectProperties_put_AssemblyOriginatorKeyFile_Proxy( 
    ProjectProperties * This,
    /* [in] */ BSTR bstrOriginatorKeyFile);


void __RPC_STUB ProjectProperties_put_AssemblyOriginatorKeyFile_Stub(
    IRpcStubBuffer *This,
    IRpcChannelBuffer *_pRpcChannelBuffer,
    PRPC_MESSAGE _pRpcMessage,
    DWORD *_pdwStubPhase);


/* [helpstring][id][propget] */ HRESULT STDMETHODCALLTYPE ProjectProperties_get_AssemblyKeyContainerName_Proxy( 
    ProjectProperties * This,
    /* [retval][out] */ BSTR *pbstrKeyContainerName);


void __RPC_STUB ProjectProperties_get_AssemblyKeyContainerName_Stub(
    IRpcStubBuffer *This,
    IRpcChannelBuffer *_pRpcChannelBuffer,
    PRPC_MESSAGE _pRpcMessage,
    DWORD *_pdwStubPhase);


/* [helpstring][id][propput] */ HRESULT STDMETHODCALLTYPE ProjectProperties_put_AssemblyKeyContainerName_Proxy( 
    ProjectProperties * This,
    /* [in] */ BSTR bstrKeyContainerName);


void __RPC_STUB ProjectProperties_put_AssemblyKeyContainerName_Stub(
    IRpcStubBuffer *This,
    IRpcChannelBuffer *_pRpcChannelBuffer,
    PRPC_MESSAGE _pRpcMessage,
    DWORD *_pdwStubPhase);


/* [helpstring][id][propget] */ HRESULT STDMETHODCALLTYPE ProjectProperties_get_AssemblyOriginatorKeyMode_Proxy( 
    ProjectProperties * This,
    /* [retval][out] */ prjOriginatorKeyMode *pOriginatorKeyMode);


void __RPC_STUB ProjectProperties_get_AssemblyOriginatorKeyMode_Stub(
    IRpcStubBuffer *This,
    IRpcChannelBuffer *_pRpcChannelBuffer,
    PRPC_MESSAGE _pRpcMessage,
    DWORD *_pdwStubPhase);


/* [helpstring][id][propput] */ HRESULT STDMETHODCALLTYPE ProjectProperties_put_AssemblyOriginatorKeyMode_Proxy( 
    ProjectProperties * This,
    /* [in] */ prjOriginatorKeyMode originatorKeyMode);


void __RPC_STUB ProjectProperties_put_AssemblyOriginatorKeyMode_Stub(
    IRpcStubBuffer *This,
    IRpcChannelBuffer *_pRpcChannelBuffer,
    PRPC_MESSAGE _pRpcMessage,
    DWORD *_pdwStubPhase);


/* [helpstring][id][propget] */ HRESULT STDMETHODCALLTYPE ProjectProperties_get_DelaySign_Proxy( 
    ProjectProperties * This,
    /* [retval][out] */ VARIANT_BOOL *pbDelaySign);


void __RPC_STUB ProjectProperties_get_DelaySign_Stub(
    IRpcStubBuffer *This,
    IRpcChannelBuffer *_pRpcChannelBuffer,
    PRPC_MESSAGE _pRpcMessage,
    DWORD *_pdwStubPhase);


/* [helpstring][id][propput] */ HRESULT STDMETHODCALLTYPE ProjectProperties_put_DelaySign_Proxy( 
    ProjectProperties * This,
    /* [in] */ VARIANT_BOOL bDelaySign);


void __RPC_STUB ProjectProperties_put_DelaySign_Stub(
    IRpcStubBuffer *This,
    IRpcChannelBuffer *_pRpcChannelBuffer,
    PRPC_MESSAGE _pRpcMessage,
    DWORD *_pdwStubPhase);


/* [helpstring][id][propget] */ HRESULT STDMETHODCALLTYPE ProjectProperties_get_WebServer_Proxy( 
    ProjectProperties * This,
    /* [retval][out] */ BSTR *pbstrWebServer);


void __RPC_STUB ProjectProperties_get_WebServer_Stub(
    IRpcStubBuffer *This,
    IRpcChannelBuffer *_pRpcChannelBuffer,
    PRPC_MESSAGE _pRpcMessage,
    DWORD *_pdwStubPhase);


/* [helpstring][id][propget] */ HRESULT STDMETHODCALLTYPE ProjectProperties_get_WebServerVersion_Proxy( 
    ProjectProperties * This,
    /* [retval][out] */ BSTR *pbstrWebServerVersion);


void __RPC_STUB ProjectProperties_get_WebServerVersion_Stub(
    IRpcStubBuffer *This,
    IRpcChannelBuffer *_pRpcChannelBuffer,
    PRPC_MESSAGE _pRpcMessage,
    DWORD *_pdwStubPhase);


/* [helpstring][id][propget] */ HRESULT STDMETHODCALLTYPE ProjectProperties_get_ServerExtensionsVersion_Proxy( 
    ProjectProperties * This,
    /* [retval][out] */ BSTR *pbstrServerExtensionsVersion);


void __RPC_STUB ProjectProperties_get_ServerExtensionsVersion_Stub(
    IRpcStubBuffer *This,
    IRpcChannelBuffer *_pRpcChannelBuffer,
    PRPC_MESSAGE _pRpcMessage,
    DWORD *_pdwStubPhase);


/* [helpstring][id][propget] */ HRESULT STDMETHODCALLTYPE ProjectProperties_get_LinkRepair_Proxy( 
    ProjectProperties * This,
    /* [retval][out] */ VARIANT_BOOL *pLinkRepair);


void __RPC_STUB ProjectProperties_get_LinkRepair_Stub(
    IRpcStubBuffer *This,
    IRpcChannelBuffer *_pRpcChannelBuffer,
    PRPC_MESSAGE _pRpcMessage,
    DWORD *_pdwStubPhase);


/* [helpstring][id][propput] */ HRESULT STDMETHODCALLTYPE ProjectProperties_put_LinkRepair_Proxy( 
    ProjectProperties * This,
    /* [in] */ VARIANT_BOOL linkRepair);


void __RPC_STUB ProjectProperties_put_LinkRepair_Stub(
    IRpcStubBuffer *This,
    IRpcChannelBuffer *_pRpcChannelBuffer,
    PRPC_MESSAGE _pRpcMessage,
    DWORD *_pdwStubPhase);


/* [helpstring][id][propget] */ HRESULT STDMETHODCALLTYPE ProjectProperties_get_OfflineURL_Proxy( 
    ProjectProperties * This,
    /* [retval][out] */ BSTR *pbstrOfflineURL);


void __RPC_STUB ProjectProperties_get_OfflineURL_Stub(
    IRpcStubBuffer *This,
    IRpcChannelBuffer *_pRpcChannelBuffer,
    PRPC_MESSAGE _pRpcMessage,
    DWORD *_pdwStubPhase);


/* [helpstring][id][propget] */ HRESULT STDMETHODCALLTYPE ProjectProperties_get_FileSharePath_Proxy( 
    ProjectProperties * This,
    /* [retval][out] */ BSTR *pbstrFileSharePath);


void __RPC_STUB ProjectProperties_get_FileSharePath_Stub(
    IRpcStubBuffer *This,
    IRpcChannelBuffer *_pRpcChannelBuffer,
    PRPC_MESSAGE _pRpcMessage,
    DWORD *_pdwStubPhase);


/* [helpstring][id][propput] */ HRESULT STDMETHODCALLTYPE ProjectProperties_put_FileSharePath_Proxy( 
    ProjectProperties * This,
    /* [in] */ BSTR bstrFileSharePath);


void __RPC_STUB ProjectProperties_put_FileSharePath_Stub(
    IRpcStubBuffer *This,
    IRpcChannelBuffer *_pRpcChannelBuffer,
    PRPC_MESSAGE _pRpcMessage,
    DWORD *_pdwStubPhase);


/* [helpstring][id][propget] */ HRESULT STDMETHODCALLTYPE ProjectProperties_get_ActiveFileSharePath_Proxy( 
    ProjectProperties * This,
    /* [retval][out] */ BSTR *pbstrFileSharePath);


void __RPC_STUB ProjectProperties_get_ActiveFileSharePath_Stub(
    IRpcStubBuffer *This,
    IRpcChannelBuffer *_pRpcChannelBuffer,
    PRPC_MESSAGE _pRpcMessage,
    DWORD *_pdwStubPhase);


/* [helpstring][id][propget] */ HRESULT STDMETHODCALLTYPE ProjectProperties_get_WebAccessMethod_Proxy( 
    ProjectProperties * This,
    /* [retval][out] */ prjWebAccessMethod *pWebAccessMethod);


void __RPC_STUB ProjectProperties_get_WebAccessMethod_Stub(
    IRpcStubBuffer *This,
    IRpcChannelBuffer *_pRpcChannelBuffer,
    PRPC_MESSAGE _pRpcMessage,
    DWORD *_pdwStubPhase);


/* [helpstring][id][propput] */ HRESULT STDMETHODCALLTYPE ProjectProperties_put_WebAccessMethod_Proxy( 
    ProjectProperties * This,
    /* [in] */ prjWebAccessMethod authoringAccessMethod);


void __RPC_STUB ProjectProperties_put_WebAccessMethod_Stub(
    IRpcStubBuffer *This,
    IRpcChannelBuffer *_pRpcChannelBuffer,
    PRPC_MESSAGE _pRpcMessage,
    DWORD *_pdwStubPhase);


/* [hidden][helpstring][nonbrowsable][id][propget] */ HRESULT STDMETHODCALLTYPE ProjectProperties_get_ActiveWebAccessMethod_Proxy( 
    ProjectProperties * This,
    /* [retval][out] */ prjWebAccessMethod *pActiveWebAccessMethod);


void __RPC_STUB ProjectProperties_get_ActiveWebAccessMethod_Stub(
    IRpcStubBuffer *This,
    IRpcChannelBuffer *_pRpcChannelBuffer,
    PRPC_MESSAGE _pRpcMessage,
    DWORD *_pdwStubPhase);


/* [helpstring][id][propget] */ HRESULT STDMETHODCALLTYPE ProjectProperties_get_DefaultClientScript_Proxy( 
    ProjectProperties * This,
    /* [retval][out] */ prjScriptLanguage *pScriptLanguage);


void __RPC_STUB ProjectProperties_get_DefaultClientScript_Stub(
    IRpcStubBuffer *This,
    IRpcChannelBuffer *_pRpcChannelBuffer,
    PRPC_MESSAGE _pRpcMessage,
    DWORD *_pdwStubPhase);


/* [helpstring][id][propput] */ HRESULT STDMETHODCALLTYPE ProjectProperties_put_DefaultClientScript_Proxy( 
    ProjectProperties * This,
    /* [in] */ prjScriptLanguage scriptLanguage);


void __RPC_STUB ProjectProperties_put_DefaultClientScript_Stub(
    IRpcStubBuffer *This,
    IRpcChannelBuffer *_pRpcChannelBuffer,
    PRPC_MESSAGE _pRpcMessage,
    DWORD *_pdwStubPhase);


/* [helpstring][id][propget] */ HRESULT STDMETHODCALLTYPE ProjectProperties_get_DefaultTargetSchema_Proxy( 
    ProjectProperties * This,
    /* [retval][out] */ prjTargetSchema *pTargetSchema);


void __RPC_STUB ProjectProperties_get_DefaultTargetSchema_Stub(
    IRpcStubBuffer *This,
    IRpcChannelBuffer *_pRpcChannelBuffer,
    PRPC_MESSAGE _pRpcMessage,
    DWORD *_pdwStubPhase);


/* [helpstring][id][propput] */ HRESULT STDMETHODCALLTYPE ProjectProperties_put_DefaultTargetSchema_Proxy( 
    ProjectProperties * This,
    /* [in] */ prjTargetSchema htmlPlatform);


void __RPC_STUB ProjectProperties_put_DefaultTargetSchema_Stub(
    IRpcStubBuffer *This,
    IRpcChannelBuffer *_pRpcChannelBuffer,
    PRPC_MESSAGE _pRpcMessage,
    DWORD *_pdwStubPhase);


/* [helpstring][id][propget] */ HRESULT STDMETHODCALLTYPE ProjectProperties_get_DefaultHTMLPageLayout_Proxy( 
    ProjectProperties * This,
    /* [retval][out] */ prjHTMLPageLayout *pHTMLPageLayout);


void __RPC_STUB ProjectProperties_get_DefaultHTMLPageLayout_Stub(
    IRpcStubBuffer *This,
    IRpcChannelBuffer *_pRpcChannelBuffer,
    PRPC_MESSAGE _pRpcMessage,
    DWORD *_pdwStubPhase);


/* [helpstring][id][propput] */ HRESULT STDMETHODCALLTYPE ProjectProperties_put_DefaultHTMLPageLayout_Proxy( 
    ProjectProperties * This,
    /* [in] */ prjHTMLPageLayout htmlPageLayout);


void __RPC_STUB ProjectProperties_put_DefaultHTMLPageLayout_Stub(
    IRpcStubBuffer *This,
    IRpcChannelBuffer *_pRpcChannelBuffer,
    PRPC_MESSAGE _pRpcMessage,
    DWORD *_pdwStubPhase);


/* [helpstring][id][propget] */ HRESULT STDMETHODCALLTYPE ProjectProperties_get_FileName_Proxy( 
    ProjectProperties * This,
    /* [retval][out] */ BSTR *pbstrFileName);


void __RPC_STUB ProjectProperties_get_FileName_Stub(
    IRpcStubBuffer *This,
    IRpcChannelBuffer *_pRpcChannelBuffer,
    PRPC_MESSAGE _pRpcMessage,
    DWORD *_pdwStubPhase);


/* [helpstring][id][propput] */ HRESULT STDMETHODCALLTYPE ProjectProperties_put_FileName_Proxy( 
    ProjectProperties * This,
    /* [in] */ BSTR bstrFileName);


void __RPC_STUB ProjectProperties_put_FileName_Stub(
    IRpcStubBuffer *This,
    IRpcChannelBuffer *_pRpcChannelBuffer,
    PRPC_MESSAGE _pRpcMessage,
    DWORD *_pdwStubPhase);


/* [helpstring][id][propget] */ HRESULT STDMETHODCALLTYPE ProjectProperties_get_FullPath_Proxy( 
    ProjectProperties * This,
    /* [retval][out] */ BSTR *pbstrFullPath);


void __RPC_STUB ProjectProperties_get_FullPath_Stub(
    IRpcStubBuffer *This,
    IRpcChannelBuffer *_pRpcChannelBuffer,
    PRPC_MESSAGE _pRpcMessage,
    DWORD *_pdwStubPhase);


/* [helpstring][id][propget] */ HRESULT STDMETHODCALLTYPE ProjectProperties_get_LocalPath_Proxy( 
    ProjectProperties * This,
    /* [retval][out] */ BSTR *pbstrLocalPath);


void __RPC_STUB ProjectProperties_get_LocalPath_Stub(
    IRpcStubBuffer *This,
    IRpcChannelBuffer *_pRpcChannelBuffer,
    PRPC_MESSAGE _pRpcMessage,
    DWORD *_pdwStubPhase);


/* [helpstring][id][propget] */ HRESULT STDMETHODCALLTYPE ProjectProperties_get_URL_Proxy( 
    ProjectProperties * This,
    /* [retval][out] */ BSTR *pbstrURL);


void __RPC_STUB ProjectProperties_get_URL_Stub(
    IRpcStubBuffer *This,
    IRpcChannelBuffer *_pRpcChannelBuffer,
    PRPC_MESSAGE _pRpcMessage,
    DWORD *_pdwStubPhase);


/* [hidden][helpstring][nonbrowsable][id][propget] */ HRESULT STDMETHODCALLTYPE ProjectProperties_get_ActiveConfigurationSettings_Proxy( 
    ProjectProperties * This,
    /* [retval][out] */ ProjectConfigurationProperties **ppVBProjConfigProps);


void __RPC_STUB ProjectProperties_get_ActiveConfigurationSettings_Stub(
    IRpcStubBuffer *This,
    IRpcChannelBuffer *_pRpcChannelBuffer,
    PRPC_MESSAGE _pRpcMessage,
    DWORD *_pdwStubPhase);


/* [hidden][helpstring][nonbrowsable][id][propget] */ HRESULT STDMETHODCALLTYPE ProjectProperties_get_Extender_Proxy( 
    ProjectProperties * This,
    /* [in] */ BSTR ExtenderName,
    /* [retval][out] */ IDispatch **Extender);


void __RPC_STUB ProjectProperties_get_Extender_Stub(
    IRpcStubBuffer *This,
    IRpcChannelBuffer *_pRpcChannelBuffer,
    PRPC_MESSAGE _pRpcMessage,
    DWORD *_pdwStubPhase);


/* [hidden][helpstring][nonbrowsable][id][propget] */ HRESULT STDMETHODCALLTYPE ProjectProperties_get_ExtenderNames_Proxy( 
    ProjectProperties * This,
    /* [retval][out] */ VARIANT *ExtenderNames);


void __RPC_STUB ProjectProperties_get_ExtenderNames_Stub(
    IRpcStubBuffer *This,
    IRpcChannelBuffer *_pRpcChannelBuffer,
    PRPC_MESSAGE _pRpcMessage,
    DWORD *_pdwStubPhase);


/* [hidden][helpstring][nonbrowsable][id][propget] */ HRESULT STDMETHODCALLTYPE ProjectProperties_get_ExtenderCATID_Proxy( 
    ProjectProperties * This,
    /* [retval][out] */ BSTR *pRetval);


void __RPC_STUB ProjectProperties_get_ExtenderCATID_Stub(
    IRpcStubBuffer *This,
    IRpcChannelBuffer *_pRpcChannelBuffer,
    PRPC_MESSAGE _pRpcMessage,
    DWORD *_pdwStubPhase);


/* [helpstring][id][propget] */ HRESULT STDMETHODCALLTYPE ProjectProperties_get_ApplicationIcon_Proxy( 
    ProjectProperties * This,
    /* [retval][out] */ BSTR *pbstrApplicationIcon);


void __RPC_STUB ProjectProperties_get_ApplicationIcon_Stub(
    IRpcStubBuffer *This,
    IRpcChannelBuffer *_pRpcChannelBuffer,
    PRPC_MESSAGE _pRpcMessage,
    DWORD *_pdwStubPhase);


/* [helpstring][id][propput] */ HRESULT STDMETHODCALLTYPE ProjectProperties_put_ApplicationIcon_Proxy( 
    ProjectProperties * This,
    /* [in] */ BSTR bstrApplicationIcon);


void __RPC_STUB ProjectProperties_put_ApplicationIcon_Stub(
    IRpcStubBuffer *This,
    IRpcChannelBuffer *_pRpcChannelBuffer,
    PRPC_MESSAGE _pRpcMessage,
    DWORD *_pdwStubPhase);


/* [helpstring][id][propget] */ HRESULT STDMETHODCALLTYPE ProjectProperties_get_OptionStrict_Proxy( 
    ProjectProperties * This,
    /* [retval][out] */ prjOptionStrict *pOptionStrict);


void __RPC_STUB ProjectProperties_get_OptionStrict_Stub(
    IRpcStubBuffer *This,
    IRpcChannelBuffer *_pRpcChannelBuffer,
    PRPC_MESSAGE _pRpcMessage,
    DWORD *_pdwStubPhase);


/* [helpstring][id][propput] */ HRESULT STDMETHODCALLTYPE ProjectProperties_put_OptionStrict_Proxy( 
    ProjectProperties * This,
    /* [in] */ prjOptionStrict optionStrict);


void __RPC_STUB ProjectProperties_put_OptionStrict_Stub(
    IRpcStubBuffer *This,
    IRpcChannelBuffer *_pRpcChannelBuffer,
    PRPC_MESSAGE _pRpcMessage,
    DWORD *_pdwStubPhase);


/* [helpstring][id][propget] */ HRESULT STDMETHODCALLTYPE ProjectProperties_get_ReferencePath_Proxy( 
    ProjectProperties * This,
    /* [retval][out] */ BSTR *pbstrReferencePath);


void __RPC_STUB ProjectProperties_get_ReferencePath_Stub(
    IRpcStubBuffer *This,
    IRpcChannelBuffer *_pRpcChannelBuffer,
    PRPC_MESSAGE _pRpcMessage,
    DWORD *_pdwStubPhase);


/* [helpstring][id][propput] */ HRESULT STDMETHODCALLTYPE ProjectProperties_put_ReferencePath_Proxy( 
    ProjectProperties * This,
    /* [in] */ BSTR bstrReferencePath);


void __RPC_STUB ProjectProperties_put_ReferencePath_Stub(
    IRpcStubBuffer *This,
    IRpcChannelBuffer *_pRpcChannelBuffer,
    PRPC_MESSAGE _pRpcMessage,
    DWORD *_pdwStubPhase);


/* [helpstring][id][propget] */ HRESULT STDMETHODCALLTYPE ProjectProperties_get_OutputFileName_Proxy( 
    ProjectProperties * This,
    /* [retval][out] */ BSTR *pbstrOutputFileName);


void __RPC_STUB ProjectProperties_get_OutputFileName_Stub(
    IRpcStubBuffer *This,
    IRpcChannelBuffer *_pRpcChannelBuffer,
    PRPC_MESSAGE _pRpcMessage,
    DWORD *_pdwStubPhase);


/* [hidden][helpstring][id][propget] */ HRESULT STDMETHODCALLTYPE ProjectProperties_get_AbsoluteProjectDirectory_Proxy( 
    ProjectProperties * This,
    /* [retval][out] */ BSTR *pbstrDir);


void __RPC_STUB ProjectProperties_get_AbsoluteProjectDirectory_Stub(
    IRpcStubBuffer *This,
    IRpcChannelBuffer *_pRpcChannelBuffer,
    PRPC_MESSAGE _pRpcMessage,
    DWORD *_pdwStubPhase);


/* [helpstring][id][propget] */ HRESULT STDMETHODCALLTYPE ProjectProperties_get_OptionExplicit_Proxy( 
    ProjectProperties * This,
    /* [retval][out] */ prjOptionExplicit *pOptionExplicit);


void __RPC_STUB ProjectProperties_get_OptionExplicit_Stub(
    IRpcStubBuffer *This,
    IRpcChannelBuffer *_pRpcChannelBuffer,
    PRPC_MESSAGE _pRpcMessage,
    DWORD *_pdwStubPhase);


/* [helpstring][id][propput] */ HRESULT STDMETHODCALLTYPE ProjectProperties_put_OptionExplicit_Proxy( 
    ProjectProperties * This,
    /* [in] */ prjOptionExplicit optionExplicit);


void __RPC_STUB ProjectProperties_put_OptionExplicit_Stub(
    IRpcStubBuffer *This,
    IRpcChannelBuffer *_pRpcChannelBuffer,
    PRPC_MESSAGE _pRpcMessage,
    DWORD *_pdwStubPhase);


/* [helpstring][id][propget] */ HRESULT STDMETHODCALLTYPE ProjectProperties_get_OptionCompare_Proxy( 
    ProjectProperties * This,
    /* [retval][out] */ prjCompare *pOptionCompare);


void __RPC_STUB ProjectProperties_get_OptionCompare_Stub(
    IRpcStubBuffer *This,
    IRpcChannelBuffer *_pRpcChannelBuffer,
    PRPC_MESSAGE _pRpcMessage,
    DWORD *_pdwStubPhase);


/* [helpstring][id][propput] */ HRESULT STDMETHODCALLTYPE ProjectProperties_put_OptionCompare_Proxy( 
    ProjectProperties * This,
    /* [in] */ prjCompare optionCompare);


void __RPC_STUB ProjectProperties_put_OptionCompare_Stub(
    IRpcStubBuffer *This,
    IRpcChannelBuffer *_pRpcChannelBuffer,
    PRPC_MESSAGE _pRpcMessage,
    DWORD *_pdwStubPhase);


/* [helpstring][id][propget] */ HRESULT STDMETHODCALLTYPE ProjectProperties_get_ProjectType_Proxy( 
    ProjectProperties * This,
    /* [retval][out] */ prjProjectType *pProjectType);


void __RPC_STUB ProjectProperties_get_ProjectType_Stub(
    IRpcStubBuffer *This,
    IRpcChannelBuffer *_pRpcChannelBuffer,
    PRPC_MESSAGE _pRpcMessage,
    DWORD *_pdwStubPhase);


/* [helpstring][id][propget] */ HRESULT STDMETHODCALLTYPE ProjectProperties_get_DefaultNamespace_Proxy( 
    ProjectProperties * This,
    /* [retval][out] */ BSTR *pbstrRootNamespace);


void __RPC_STUB ProjectProperties_get_DefaultNamespace_Stub(
    IRpcStubBuffer *This,
    IRpcChannelBuffer *_pRpcChannelBuffer,
    PRPC_MESSAGE _pRpcMessage,
    DWORD *_pdwStubPhase);


/* [helpstring][id][propput] */ HRESULT STDMETHODCALLTYPE ProjectProperties_put_DefaultNamespace_Proxy( 
    ProjectProperties * This,
    /* [in] */ BSTR bstrRootNamespace);


void __RPC_STUB ProjectProperties_put_DefaultNamespace_Stub(
    IRpcStubBuffer *This,
    IRpcChannelBuffer *_pRpcChannelBuffer,
    PRPC_MESSAGE _pRpcMessage,
    DWORD *_pdwStubPhase);



#endif 	/* __ProjectProperties_INTERFACE_DEFINED__ */


#ifndef __FileProperties_INTERFACE_DEFINED__
#define __FileProperties_INTERFACE_DEFINED__

/* interface FileProperties */
/* [local][hidden][helpstring][dual][uuid][object] */ 


EXTERN_C const IID IID_FileProperties;

#if defined(__cplusplus) && !defined(CINTERFACE)
    
    MIDL_INTERFACE("516BD64E-51C0-11D3-85CF-00C04F6123B3")
    FileProperties : public IDispatch
    {
    public:
        virtual /* [hidden][helpstring][id][propget] */ HRESULT STDMETHODCALLTYPE get___id( 
            /* [retval][out] */ BSTR *pbstrName) = 0;
        
        virtual /* [helpstring][id][propget] */ HRESULT STDMETHODCALLTYPE get_FileName( 
            /* [retval][out] */ BSTR *pbstrName) = 0;
        
        virtual /* [helpstring][id][propput] */ HRESULT STDMETHODCALLTYPE put_FileName( 
            /* [in] */ BSTR bstrName) = 0;
        
        virtual /* [helpstring][id][propget] */ HRESULT STDMETHODCALLTYPE get_Extension( 
            /* [retval][out] */ BSTR *pbstrExtension) = 0;
        
        virtual /* [helpstring][id][propget] */ HRESULT STDMETHODCALLTYPE get_Filesize( 
            /* [retval][out] */ unsigned long *pdwSize) = 0;
        
        virtual /* [helpstring][id][propget] */ HRESULT STDMETHODCALLTYPE get_LocalPath( 
            /* [retval][out] */ BSTR *pbstrLocalPath) = 0;
        
        virtual /* [helpstring][id][propget] */ HRESULT STDMETHODCALLTYPE get_FullPath( 
            /* [retval][out] */ BSTR *pbstrFullPath) = 0;
        
        virtual /* [helpstring][id][propget] */ HRESULT STDMETHODCALLTYPE get_URL( 
            /* [retval][out] */ BSTR *pbstrURL) = 0;
        
        virtual /* [helpstring][id][propget] */ HRESULT STDMETHODCALLTYPE get_HTMLTitle( 
            /* [retval][out] */ BSTR *pbstrTitle) = 0;
        
        virtual /* [helpstring][id][propget] */ HRESULT STDMETHODCALLTYPE get_Author( 
            /* [retval][out] */ BSTR *pbstrTitle) = 0;
        
        virtual /* [helpstring][id][propget] */ HRESULT STDMETHODCALLTYPE get_DateCreated( 
            /* [retval][out] */ BSTR *pbstrDateCreated) = 0;
        
        virtual /* [helpstring][id][propget] */ HRESULT STDMETHODCALLTYPE get_DateModified( 
            /* [retval][out] */ BSTR *pbstrDateCreated) = 0;
        
        virtual /* [helpstring][id][propget] */ HRESULT STDMETHODCALLTYPE get_ModifiedBy( 
            /* [retval][out] */ BSTR *pbstrDateCreated) = 0;
        
        virtual /* [nonbrowsable][id][propget] */ HRESULT STDMETHODCALLTYPE get_SubType( 
            /* [retval][out] */ BSTR *pbstrSubType) = 0;
        
        virtual /* [nonbrowsable][id][propput] */ HRESULT STDMETHODCALLTYPE put_SubType( 
            /* [in] */ BSTR bstrSubType) = 0;
        
        virtual /* [hidden][helpstring][nonbrowsable][propget][id] */ HRESULT STDMETHODCALLTYPE get_Extender( 
            /* [in] */ BSTR ExtenderName,
            /* [retval][out] */ IDispatch **Extender) = 0;
        
        virtual /* [hidden][helpstring][nonbrowsable][propget][id] */ HRESULT STDMETHODCALLTYPE get_ExtenderNames( 
            /* [retval][out] */ VARIANT *ExtenderNames) = 0;
        
        virtual /* [hidden][helpstring][nonbrowsable][propget][id] */ HRESULT STDMETHODCALLTYPE get_ExtenderCATID( 
            /* [retval][out] */ BSTR *pRetval) = 0;
        
        virtual /* [helpstring][id][propget] */ HRESULT STDMETHODCALLTYPE get_BuildAction( 
            /* [retval][out] */ prjBuildAction *pbuildAction) = 0;
        
        virtual /* [helpstring][id][propput] */ HRESULT STDMETHODCALLTYPE put_BuildAction( 
            /* [in] */ prjBuildAction buildAction) = 0;
        
        virtual /* [helpstring][id][propget] */ HRESULT STDMETHODCALLTYPE get_CustomTool( 
            /* [retval][out] */ BSTR *pbstrCustomTool) = 0;
        
        virtual /* [helpstring][id][propput] */ HRESULT STDMETHODCALLTYPE put_CustomTool( 
            /* [in] */ BSTR bstrCustomTool) = 0;
        
        virtual /* [helpstring][id][propget] */ HRESULT STDMETHODCALLTYPE get_CustomToolNamespace( 
            /* [retval][out] */ BSTR *pbstrCustomToolNamespace) = 0;
        
        virtual /* [helpstring][id][propput] */ HRESULT STDMETHODCALLTYPE put_CustomToolNamespace( 
            /* [in] */ BSTR bstrCustomToolNamespace) = 0;
        
        virtual /* [helpstring][nonbrowsable][id][propget] */ HRESULT STDMETHODCALLTYPE get_CustomToolOutput( 
            /* [retval][out] */ BSTR *pbstrCustomToolOutput) = 0;
        
        virtual /* [helpstring][nonbrowsable][id][propget] */ HRESULT STDMETHODCALLTYPE get_IsCustomToolOutput( 
            /* [retval][out] */ VARIANT_BOOL *pbIsCustomToolOutput) = 0;
        
        virtual /* [helpstring][nonbrowsable][id][propget] */ HRESULT STDMETHODCALLTYPE get_IsDependentFile( 
            /* [retval][out] */ VARIANT_BOOL *pbIsDepedentFile) = 0;
        
        virtual /* [helpstring][nonbrowsable][id][propget] */ HRESULT STDMETHODCALLTYPE get_IsLink( 
            /* [retval][out] */ VARIANT_BOOL *pbIsLinkFile) = 0;
        
        virtual /* [helpstring][nonbrowsable][id][propget] */ HRESULT STDMETHODCALLTYPE get_IsDesignTimeBuildInput( 
            /* [retval][out] */ VARIANT_BOOL *pbIsDesignTimeBuildInput) = 0;
        
    };
    
#else 	/* C style interface */

    typedef struct FilePropertiesVtbl
    {
        BEGIN_INTERFACE
        
        HRESULT ( STDMETHODCALLTYPE *QueryInterface )( 
            FileProperties * This,
            /* [in] */ REFIID riid,
            /* [iid_is][out] */ void **ppvObject);
        
        ULONG ( STDMETHODCALLTYPE *AddRef )( 
            FileProperties * This);
        
        ULONG ( STDMETHODCALLTYPE *Release )( 
            FileProperties * This);
        
        HRESULT ( STDMETHODCALLTYPE *GetTypeInfoCount )( 
            FileProperties * This,
            /* [out] */ UINT *pctinfo);
        
        HRESULT ( STDMETHODCALLTYPE *GetTypeInfo )( 
            FileProperties * This,
            /* [in] */ UINT iTInfo,
            /* [in] */ LCID lcid,
            /* [out] */ ITypeInfo **ppTInfo);
        
        HRESULT ( STDMETHODCALLTYPE *GetIDsOfNames )( 
            FileProperties * This,
            /* [in] */ REFIID riid,
            /* [size_is][in] */ LPOLESTR *rgszNames,
            /* [in] */ UINT cNames,
            /* [in] */ LCID lcid,
            /* [size_is][out] */ DISPID *rgDispId);
        
        /* [local] */ HRESULT ( STDMETHODCALLTYPE *Invoke )( 
            FileProperties * This,
            /* [in] */ DISPID dispIdMember,
            /* [in] */ REFIID riid,
            /* [in] */ LCID lcid,
            /* [in] */ WORD wFlags,
            /* [out][in] */ DISPPARAMS *pDispParams,
            /* [out] */ VARIANT *pVarResult,
            /* [out] */ EXCEPINFO *pExcepInfo,
            /* [out] */ UINT *puArgErr);
        
        /* [hidden][helpstring][id][propget] */ HRESULT ( STDMETHODCALLTYPE *get___id )( 
            FileProperties * This,
            /* [retval][out] */ BSTR *pbstrName);
        
        /* [helpstring][id][propget] */ HRESULT ( STDMETHODCALLTYPE *get_FileName )( 
            FileProperties * This,
            /* [retval][out] */ BSTR *pbstrName);
        
        /* [helpstring][id][propput] */ HRESULT ( STDMETHODCALLTYPE *put_FileName )( 
            FileProperties * This,
            /* [in] */ BSTR bstrName);
        
        /* [helpstring][id][propget] */ HRESULT ( STDMETHODCALLTYPE *get_Extension )( 
            FileProperties * This,
            /* [retval][out] */ BSTR *pbstrExtension);
        
        /* [helpstring][id][propget] */ HRESULT ( STDMETHODCALLTYPE *get_Filesize )( 
            FileProperties * This,
            /* [retval][out] */ unsigned long *pdwSize);
        
        /* [helpstring][id][propget] */ HRESULT ( STDMETHODCALLTYPE *get_LocalPath )( 
            FileProperties * This,
            /* [retval][out] */ BSTR *pbstrLocalPath);
        
        /* [helpstring][id][propget] */ HRESULT ( STDMETHODCALLTYPE *get_FullPath )( 
            FileProperties * This,
            /* [retval][out] */ BSTR *pbstrFullPath);
        
        /* [helpstring][id][propget] */ HRESULT ( STDMETHODCALLTYPE *get_URL )( 
            FileProperties * This,
            /* [retval][out] */ BSTR *pbstrURL);
        
        /* [helpstring][id][propget] */ HRESULT ( STDMETHODCALLTYPE *get_HTMLTitle )( 
            FileProperties * This,
            /* [retval][out] */ BSTR *pbstrTitle);
        
        /* [helpstring][id][propget] */ HRESULT ( STDMETHODCALLTYPE *get_Author )( 
            FileProperties * This,
            /* [retval][out] */ BSTR *pbstrTitle);
        
        /* [helpstring][id][propget] */ HRESULT ( STDMETHODCALLTYPE *get_DateCreated )( 
            FileProperties * This,
            /* [retval][out] */ BSTR *pbstrDateCreated);
        
        /* [helpstring][id][propget] */ HRESULT ( STDMETHODCALLTYPE *get_DateModified )( 
            FileProperties * This,
            /* [retval][out] */ BSTR *pbstrDateCreated);
        
        /* [helpstring][id][propget] */ HRESULT ( STDMETHODCALLTYPE *get_ModifiedBy )( 
            FileProperties * This,
            /* [retval][out] */ BSTR *pbstrDateCreated);
        
        /* [nonbrowsable][id][propget] */ HRESULT ( STDMETHODCALLTYPE *get_SubType )( 
            FileProperties * This,
            /* [retval][out] */ BSTR *pbstrSubType);
        
        /* [nonbrowsable][id][propput] */ HRESULT ( STDMETHODCALLTYPE *put_SubType )( 
            FileProperties * This,
            /* [in] */ BSTR bstrSubType);
        
        /* [hidden][helpstring][nonbrowsable][propget][id] */ HRESULT ( STDMETHODCALLTYPE *get_Extender )( 
            FileProperties * This,
            /* [in] */ BSTR ExtenderName,
            /* [retval][out] */ IDispatch **Extender);
        
        /* [hidden][helpstring][nonbrowsable][propget][id] */ HRESULT ( STDMETHODCALLTYPE *get_ExtenderNames )( 
            FileProperties * This,
            /* [retval][out] */ VARIANT *ExtenderNames);
        
        /* [hidden][helpstring][nonbrowsable][propget][id] */ HRESULT ( STDMETHODCALLTYPE *get_ExtenderCATID )( 
            FileProperties * This,
            /* [retval][out] */ BSTR *pRetval);
        
        /* [helpstring][id][propget] */ HRESULT ( STDMETHODCALLTYPE *get_BuildAction )( 
            FileProperties * This,
            /* [retval][out] */ prjBuildAction *pbuildAction);
        
        /* [helpstring][id][propput] */ HRESULT ( STDMETHODCALLTYPE *put_BuildAction )( 
            FileProperties * This,
            /* [in] */ prjBuildAction buildAction);
        
        /* [helpstring][id][propget] */ HRESULT ( STDMETHODCALLTYPE *get_CustomTool )( 
            FileProperties * This,
            /* [retval][out] */ BSTR *pbstrCustomTool);
        
        /* [helpstring][id][propput] */ HRESULT ( STDMETHODCALLTYPE *put_CustomTool )( 
            FileProperties * This,
            /* [in] */ BSTR bstrCustomTool);
        
        /* [helpstring][id][propget] */ HRESULT ( STDMETHODCALLTYPE *get_CustomToolNamespace )( 
            FileProperties * This,
            /* [retval][out] */ BSTR *pbstrCustomToolNamespace);
        
        /* [helpstring][id][propput] */ HRESULT ( STDMETHODCALLTYPE *put_CustomToolNamespace )( 
            FileProperties * This,
            /* [in] */ BSTR bstrCustomToolNamespace);
        
        /* [helpstring][nonbrowsable][id][propget] */ HRESULT ( STDMETHODCALLTYPE *get_CustomToolOutput )( 
            FileProperties * This,
            /* [retval][out] */ BSTR *pbstrCustomToolOutput);
        
        /* [helpstring][nonbrowsable][id][propget] */ HRESULT ( STDMETHODCALLTYPE *get_IsCustomToolOutput )( 
            FileProperties * This,
            /* [retval][out] */ VARIANT_BOOL *pbIsCustomToolOutput);
        
        /* [helpstring][nonbrowsable][id][propget] */ HRESULT ( STDMETHODCALLTYPE *get_IsDependentFile )( 
            FileProperties * This,
            /* [retval][out] */ VARIANT_BOOL *pbIsDepedentFile);
        
        /* [helpstring][nonbrowsable][id][propget] */ HRESULT ( STDMETHODCALLTYPE *get_IsLink )( 
            FileProperties * This,
            /* [retval][out] */ VARIANT_BOOL *pbIsLinkFile);
        
        /* [helpstring][nonbrowsable][id][propget] */ HRESULT ( STDMETHODCALLTYPE *get_IsDesignTimeBuildInput )( 
            FileProperties * This,
            /* [retval][out] */ VARIANT_BOOL *pbIsDesignTimeBuildInput);
        
        END_INTERFACE
    } FilePropertiesVtbl;

    interface FileProperties
    {
        CONST_VTBL struct FilePropertiesVtbl *lpVtbl;
    };

    

#ifdef COBJMACROS


#define FileProperties_QueryInterface(This,riid,ppvObject)	\
    (This)->lpVtbl -> QueryInterface(This,riid,ppvObject)

#define FileProperties_AddRef(This)	\
    (This)->lpVtbl -> AddRef(This)

#define FileProperties_Release(This)	\
    (This)->lpVtbl -> Release(This)


#define FileProperties_GetTypeInfoCount(This,pctinfo)	\
    (This)->lpVtbl -> GetTypeInfoCount(This,pctinfo)

#define FileProperties_GetTypeInfo(This,iTInfo,lcid,ppTInfo)	\
    (This)->lpVtbl -> GetTypeInfo(This,iTInfo,lcid,ppTInfo)

#define FileProperties_GetIDsOfNames(This,riid,rgszNames,cNames,lcid,rgDispId)	\
    (This)->lpVtbl -> GetIDsOfNames(This,riid,rgszNames,cNames,lcid,rgDispId)

#define FileProperties_Invoke(This,dispIdMember,riid,lcid,wFlags,pDispParams,pVarResult,pExcepInfo,puArgErr)	\
    (This)->lpVtbl -> Invoke(This,dispIdMember,riid,lcid,wFlags,pDispParams,pVarResult,pExcepInfo,puArgErr)


#define FileProperties_get___id(This,pbstrName)	\
    (This)->lpVtbl -> get___id(This,pbstrName)

#define FileProperties_get_FileName(This,pbstrName)	\
    (This)->lpVtbl -> get_FileName(This,pbstrName)

#define FileProperties_put_FileName(This,bstrName)	\
    (This)->lpVtbl -> put_FileName(This,bstrName)

#define FileProperties_get_Extension(This,pbstrExtension)	\
    (This)->lpVtbl -> get_Extension(This,pbstrExtension)

#define FileProperties_get_Filesize(This,pdwSize)	\
    (This)->lpVtbl -> get_Filesize(This,pdwSize)

#define FileProperties_get_LocalPath(This,pbstrLocalPath)	\
    (This)->lpVtbl -> get_LocalPath(This,pbstrLocalPath)

#define FileProperties_get_FullPath(This,pbstrFullPath)	\
    (This)->lpVtbl -> get_FullPath(This,pbstrFullPath)

#define FileProperties_get_URL(This,pbstrURL)	\
    (This)->lpVtbl -> get_URL(This,pbstrURL)

#define FileProperties_get_HTMLTitle(This,pbstrTitle)	\
    (This)->lpVtbl -> get_HTMLTitle(This,pbstrTitle)

#define FileProperties_get_Author(This,pbstrTitle)	\
    (This)->lpVtbl -> get_Author(This,pbstrTitle)

#define FileProperties_get_DateCreated(This,pbstrDateCreated)	\
    (This)->lpVtbl -> get_DateCreated(This,pbstrDateCreated)

#define FileProperties_get_DateModified(This,pbstrDateCreated)	\
    (This)->lpVtbl -> get_DateModified(This,pbstrDateCreated)

#define FileProperties_get_ModifiedBy(This,pbstrDateCreated)	\
    (This)->lpVtbl -> get_ModifiedBy(This,pbstrDateCreated)

#define FileProperties_get_SubType(This,pbstrSubType)	\
    (This)->lpVtbl -> get_SubType(This,pbstrSubType)

#define FileProperties_put_SubType(This,bstrSubType)	\
    (This)->lpVtbl -> put_SubType(This,bstrSubType)

#define FileProperties_get_Extender(This,ExtenderName,Extender)	\
    (This)->lpVtbl -> get_Extender(This,ExtenderName,Extender)

#define FileProperties_get_ExtenderNames(This,ExtenderNames)	\
    (This)->lpVtbl -> get_ExtenderNames(This,ExtenderNames)

#define FileProperties_get_ExtenderCATID(This,pRetval)	\
    (This)->lpVtbl -> get_ExtenderCATID(This,pRetval)

#define FileProperties_get_BuildAction(This,pbuildAction)	\
    (This)->lpVtbl -> get_BuildAction(This,pbuildAction)

#define FileProperties_put_BuildAction(This,buildAction)	\
    (This)->lpVtbl -> put_BuildAction(This,buildAction)

#define FileProperties_get_CustomTool(This,pbstrCustomTool)	\
    (This)->lpVtbl -> get_CustomTool(This,pbstrCustomTool)

#define FileProperties_put_CustomTool(This,bstrCustomTool)	\
    (This)->lpVtbl -> put_CustomTool(This,bstrCustomTool)

#define FileProperties_get_CustomToolNamespace(This,pbstrCustomToolNamespace)	\
    (This)->lpVtbl -> get_CustomToolNamespace(This,pbstrCustomToolNamespace)

#define FileProperties_put_CustomToolNamespace(This,bstrCustomToolNamespace)	\
    (This)->lpVtbl -> put_CustomToolNamespace(This,bstrCustomToolNamespace)

#define FileProperties_get_CustomToolOutput(This,pbstrCustomToolOutput)	\
    (This)->lpVtbl -> get_CustomToolOutput(This,pbstrCustomToolOutput)

#define FileProperties_get_IsCustomToolOutput(This,pbIsCustomToolOutput)	\
    (This)->lpVtbl -> get_IsCustomToolOutput(This,pbIsCustomToolOutput)

#define FileProperties_get_IsDependentFile(This,pbIsDepedentFile)	\
    (This)->lpVtbl -> get_IsDependentFile(This,pbIsDepedentFile)

#define FileProperties_get_IsLink(This,pbIsLinkFile)	\
    (This)->lpVtbl -> get_IsLink(This,pbIsLinkFile)

#define FileProperties_get_IsDesignTimeBuildInput(This,pbIsDesignTimeBuildInput)	\
    (This)->lpVtbl -> get_IsDesignTimeBuildInput(This,pbIsDesignTimeBuildInput)

#endif /* COBJMACROS */


#endif 	/* C style interface */



/* [hidden][helpstring][id][propget] */ HRESULT STDMETHODCALLTYPE FileProperties_get___id_Proxy( 
    FileProperties * This,
    /* [retval][out] */ BSTR *pbstrName);


void __RPC_STUB FileProperties_get___id_Stub(
    IRpcStubBuffer *This,
    IRpcChannelBuffer *_pRpcChannelBuffer,
    PRPC_MESSAGE _pRpcMessage,
    DWORD *_pdwStubPhase);


/* [helpstring][id][propget] */ HRESULT STDMETHODCALLTYPE FileProperties_get_FileName_Proxy( 
    FileProperties * This,
    /* [retval][out] */ BSTR *pbstrName);


void __RPC_STUB FileProperties_get_FileName_Stub(
    IRpcStubBuffer *This,
    IRpcChannelBuffer *_pRpcChannelBuffer,
    PRPC_MESSAGE _pRpcMessage,
    DWORD *_pdwStubPhase);


/* [helpstring][id][propput] */ HRESULT STDMETHODCALLTYPE FileProperties_put_FileName_Proxy( 
    FileProperties * This,
    /* [in] */ BSTR bstrName);


void __RPC_STUB FileProperties_put_FileName_Stub(
    IRpcStubBuffer *This,
    IRpcChannelBuffer *_pRpcChannelBuffer,
    PRPC_MESSAGE _pRpcMessage,
    DWORD *_pdwStubPhase);


/* [helpstring][id][propget] */ HRESULT STDMETHODCALLTYPE FileProperties_get_Extension_Proxy( 
    FileProperties * This,
    /* [retval][out] */ BSTR *pbstrExtension);


void __RPC_STUB FileProperties_get_Extension_Stub(
    IRpcStubBuffer *This,
    IRpcChannelBuffer *_pRpcChannelBuffer,
    PRPC_MESSAGE _pRpcMessage,
    DWORD *_pdwStubPhase);


/* [helpstring][id][propget] */ HRESULT STDMETHODCALLTYPE FileProperties_get_Filesize_Proxy( 
    FileProperties * This,
    /* [retval][out] */ unsigned long *pdwSize);


void __RPC_STUB FileProperties_get_Filesize_Stub(
    IRpcStubBuffer *This,
    IRpcChannelBuffer *_pRpcChannelBuffer,
    PRPC_MESSAGE _pRpcMessage,
    DWORD *_pdwStubPhase);


/* [helpstring][id][propget] */ HRESULT STDMETHODCALLTYPE FileProperties_get_LocalPath_Proxy( 
    FileProperties * This,
    /* [retval][out] */ BSTR *pbstrLocalPath);


void __RPC_STUB FileProperties_get_LocalPath_Stub(
    IRpcStubBuffer *This,
    IRpcChannelBuffer *_pRpcChannelBuffer,
    PRPC_MESSAGE _pRpcMessage,
    DWORD *_pdwStubPhase);


/* [helpstring][id][propget] */ HRESULT STDMETHODCALLTYPE FileProperties_get_FullPath_Proxy( 
    FileProperties * This,
    /* [retval][out] */ BSTR *pbstrFullPath);


void __RPC_STUB FileProperties_get_FullPath_Stub(
    IRpcStubBuffer *This,
    IRpcChannelBuffer *_pRpcChannelBuffer,
    PRPC_MESSAGE _pRpcMessage,
    DWORD *_pdwStubPhase);


/* [helpstring][id][propget] */ HRESULT STDMETHODCALLTYPE FileProperties_get_URL_Proxy( 
    FileProperties * This,
    /* [retval][out] */ BSTR *pbstrURL);


void __RPC_STUB FileProperties_get_URL_Stub(
    IRpcStubBuffer *This,
    IRpcChannelBuffer *_pRpcChannelBuffer,
    PRPC_MESSAGE _pRpcMessage,
    DWORD *_pdwStubPhase);


/* [helpstring][id][propget] */ HRESULT STDMETHODCALLTYPE FileProperties_get_HTMLTitle_Proxy( 
    FileProperties * This,
    /* [retval][out] */ BSTR *pbstrTitle);


void __RPC_STUB FileProperties_get_HTMLTitle_Stub(
    IRpcStubBuffer *This,
    IRpcChannelBuffer *_pRpcChannelBuffer,
    PRPC_MESSAGE _pRpcMessage,
    DWORD *_pdwStubPhase);


/* [helpstring][id][propget] */ HRESULT STDMETHODCALLTYPE FileProperties_get_Author_Proxy( 
    FileProperties * This,
    /* [retval][out] */ BSTR *pbstrTitle);


void __RPC_STUB FileProperties_get_Author_Stub(
    IRpcStubBuffer *This,
    IRpcChannelBuffer *_pRpcChannelBuffer,
    PRPC_MESSAGE _pRpcMessage,
    DWORD *_pdwStubPhase);


/* [helpstring][id][propget] */ HRESULT STDMETHODCALLTYPE FileProperties_get_DateCreated_Proxy( 
    FileProperties * This,
    /* [retval][out] */ BSTR *pbstrDateCreated);


void __RPC_STUB FileProperties_get_DateCreated_Stub(
    IRpcStubBuffer *This,
    IRpcChannelBuffer *_pRpcChannelBuffer,
    PRPC_MESSAGE _pRpcMessage,
    DWORD *_pdwStubPhase);


/* [helpstring][id][propget] */ HRESULT STDMETHODCALLTYPE FileProperties_get_DateModified_Proxy( 
    FileProperties * This,
    /* [retval][out] */ BSTR *pbstrDateCreated);


void __RPC_STUB FileProperties_get_DateModified_Stub(
    IRpcStubBuffer *This,
    IRpcChannelBuffer *_pRpcChannelBuffer,
    PRPC_MESSAGE _pRpcMessage,
    DWORD *_pdwStubPhase);


/* [helpstring][id][propget] */ HRESULT STDMETHODCALLTYPE FileProperties_get_ModifiedBy_Proxy( 
    FileProperties * This,
    /* [retval][out] */ BSTR *pbstrDateCreated);


void __RPC_STUB FileProperties_get_ModifiedBy_Stub(
    IRpcStubBuffer *This,
    IRpcChannelBuffer *_pRpcChannelBuffer,
    PRPC_MESSAGE _pRpcMessage,
    DWORD *_pdwStubPhase);


/* [nonbrowsable][id][propget] */ HRESULT STDMETHODCALLTYPE FileProperties_get_SubType_Proxy( 
    FileProperties * This,
    /* [retval][out] */ BSTR *pbstrSubType);


void __RPC_STUB FileProperties_get_SubType_Stub(
    IRpcStubBuffer *This,
    IRpcChannelBuffer *_pRpcChannelBuffer,
    PRPC_MESSAGE _pRpcMessage,
    DWORD *_pdwStubPhase);


/* [nonbrowsable][id][propput] */ HRESULT STDMETHODCALLTYPE FileProperties_put_SubType_Proxy( 
    FileProperties * This,
    /* [in] */ BSTR bstrSubType);


void __RPC_STUB FileProperties_put_SubType_Stub(
    IRpcStubBuffer *This,
    IRpcChannelBuffer *_pRpcChannelBuffer,
    PRPC_MESSAGE _pRpcMessage,
    DWORD *_pdwStubPhase);


/* [hidden][helpstring][nonbrowsable][propget][id] */ HRESULT STDMETHODCALLTYPE FileProperties_get_Extender_Proxy( 
    FileProperties * This,
    /* [in] */ BSTR ExtenderName,
    /* [retval][out] */ IDispatch **Extender);


void __RPC_STUB FileProperties_get_Extender_Stub(
    IRpcStubBuffer *This,
    IRpcChannelBuffer *_pRpcChannelBuffer,
    PRPC_MESSAGE _pRpcMessage,
    DWORD *_pdwStubPhase);


/* [hidden][helpstring][nonbrowsable][propget][id] */ HRESULT STDMETHODCALLTYPE FileProperties_get_ExtenderNames_Proxy( 
    FileProperties * This,
    /* [retval][out] */ VARIANT *ExtenderNames);


void __RPC_STUB FileProperties_get_ExtenderNames_Stub(
    IRpcStubBuffer *This,
    IRpcChannelBuffer *_pRpcChannelBuffer,
    PRPC_MESSAGE _pRpcMessage,
    DWORD *_pdwStubPhase);


/* [hidden][helpstring][nonbrowsable][propget][id] */ HRESULT STDMETHODCALLTYPE FileProperties_get_ExtenderCATID_Proxy( 
    FileProperties * This,
    /* [retval][out] */ BSTR *pRetval);


void __RPC_STUB FileProperties_get_ExtenderCATID_Stub(
    IRpcStubBuffer *This,
    IRpcChannelBuffer *_pRpcChannelBuffer,
    PRPC_MESSAGE _pRpcMessage,
    DWORD *_pdwStubPhase);


/* [helpstring][id][propget] */ HRESULT STDMETHODCALLTYPE FileProperties_get_BuildAction_Proxy( 
    FileProperties * This,
    /* [retval][out] */ prjBuildAction *pbuildAction);


void __RPC_STUB FileProperties_get_BuildAction_Stub(
    IRpcStubBuffer *This,
    IRpcChannelBuffer *_pRpcChannelBuffer,
    PRPC_MESSAGE _pRpcMessage,
    DWORD *_pdwStubPhase);


/* [helpstring][id][propput] */ HRESULT STDMETHODCALLTYPE FileProperties_put_BuildAction_Proxy( 
    FileProperties * This,
    /* [in] */ prjBuildAction buildAction);


void __RPC_STUB FileProperties_put_BuildAction_Stub(
    IRpcStubBuffer *This,
    IRpcChannelBuffer *_pRpcChannelBuffer,
    PRPC_MESSAGE _pRpcMessage,
    DWORD *_pdwStubPhase);


/* [helpstring][id][propget] */ HRESULT STDMETHODCALLTYPE FileProperties_get_CustomTool_Proxy( 
    FileProperties * This,
    /* [retval][out] */ BSTR *pbstrCustomTool);


void __RPC_STUB FileProperties_get_CustomTool_Stub(
    IRpcStubBuffer *This,
    IRpcChannelBuffer *_pRpcChannelBuffer,
    PRPC_MESSAGE _pRpcMessage,
    DWORD *_pdwStubPhase);


/* [helpstring][id][propput] */ HRESULT STDMETHODCALLTYPE FileProperties_put_CustomTool_Proxy( 
    FileProperties * This,
    /* [in] */ BSTR bstrCustomTool);


void __RPC_STUB FileProperties_put_CustomTool_Stub(
    IRpcStubBuffer *This,
    IRpcChannelBuffer *_pRpcChannelBuffer,
    PRPC_MESSAGE _pRpcMessage,
    DWORD *_pdwStubPhase);


/* [helpstring][id][propget] */ HRESULT STDMETHODCALLTYPE FileProperties_get_CustomToolNamespace_Proxy( 
    FileProperties * This,
    /* [retval][out] */ BSTR *pbstrCustomToolNamespace);


void __RPC_STUB FileProperties_get_CustomToolNamespace_Stub(
    IRpcStubBuffer *This,
    IRpcChannelBuffer *_pRpcChannelBuffer,
    PRPC_MESSAGE _pRpcMessage,
    DWORD *_pdwStubPhase);


/* [helpstring][id][propput] */ HRESULT STDMETHODCALLTYPE FileProperties_put_CustomToolNamespace_Proxy( 
    FileProperties * This,
    /* [in] */ BSTR bstrCustomToolNamespace);


void __RPC_STUB FileProperties_put_CustomToolNamespace_Stub(
    IRpcStubBuffer *This,
    IRpcChannelBuffer *_pRpcChannelBuffer,
    PRPC_MESSAGE _pRpcMessage,
    DWORD *_pdwStubPhase);


/* [helpstring][nonbrowsable][id][propget] */ HRESULT STDMETHODCALLTYPE FileProperties_get_CustomToolOutput_Proxy( 
    FileProperties * This,
    /* [retval][out] */ BSTR *pbstrCustomToolOutput);


void __RPC_STUB FileProperties_get_CustomToolOutput_Stub(
    IRpcStubBuffer *This,
    IRpcChannelBuffer *_pRpcChannelBuffer,
    PRPC_MESSAGE _pRpcMessage,
    DWORD *_pdwStubPhase);


/* [helpstring][nonbrowsable][id][propget] */ HRESULT STDMETHODCALLTYPE FileProperties_get_IsCustomToolOutput_Proxy( 
    FileProperties * This,
    /* [retval][out] */ VARIANT_BOOL *pbIsCustomToolOutput);


void __RPC_STUB FileProperties_get_IsCustomToolOutput_Stub(
    IRpcStubBuffer *This,
    IRpcChannelBuffer *_pRpcChannelBuffer,
    PRPC_MESSAGE _pRpcMessage,
    DWORD *_pdwStubPhase);


/* [helpstring][nonbrowsable][id][propget] */ HRESULT STDMETHODCALLTYPE FileProperties_get_IsDependentFile_Proxy( 
    FileProperties * This,
    /* [retval][out] */ VARIANT_BOOL *pbIsDepedentFile);


void __RPC_STUB FileProperties_get_IsDependentFile_Stub(
    IRpcStubBuffer *This,
    IRpcChannelBuffer *_pRpcChannelBuffer,
    PRPC_MESSAGE _pRpcMessage,
    DWORD *_pdwStubPhase);


/* [helpstring][nonbrowsable][id][propget] */ HRESULT STDMETHODCALLTYPE FileProperties_get_IsLink_Proxy( 
    FileProperties * This,
    /* [retval][out] */ VARIANT_BOOL *pbIsLinkFile);


void __RPC_STUB FileProperties_get_IsLink_Stub(
    IRpcStubBuffer *This,
    IRpcChannelBuffer *_pRpcChannelBuffer,
    PRPC_MESSAGE _pRpcMessage,
    DWORD *_pdwStubPhase);


/* [helpstring][nonbrowsable][id][propget] */ HRESULT STDMETHODCALLTYPE FileProperties_get_IsDesignTimeBuildInput_Proxy( 
    FileProperties * This,
    /* [retval][out] */ VARIANT_BOOL *pbIsDesignTimeBuildInput);


void __RPC_STUB FileProperties_get_IsDesignTimeBuildInput_Stub(
    IRpcStubBuffer *This,
    IRpcChannelBuffer *_pRpcChannelBuffer,
    PRPC_MESSAGE _pRpcMessage,
    DWORD *_pdwStubPhase);



#endif 	/* __FileProperties_INTERFACE_DEFINED__ */


#ifndef __FolderProperties_INTERFACE_DEFINED__
#define __FolderProperties_INTERFACE_DEFINED__

/* interface FolderProperties */
/* [local][hidden][unique][helpstring][dual][uuid][object] */ 


EXTERN_C const IID IID_FolderProperties;

#if defined(__cplusplus) && !defined(CINTERFACE)
    
    MIDL_INTERFACE("8E4AA768-51E1-11D3-85CF-00C04F6123B3")
    FolderProperties : public IDispatch
    {
    public:
        virtual /* [hidden][helpstring][id][propget] */ HRESULT STDMETHODCALLTYPE get___id( 
            /* [retval][out] */ BSTR *pbstrName) = 0;
        
        virtual /* [helpstring][id][propget] */ HRESULT STDMETHODCALLTYPE get_FileName( 
            /* [retval][out] */ BSTR *pbstrName) = 0;
        
        virtual /* [helpstring][id][propput] */ HRESULT STDMETHODCALLTYPE put_FileName( 
            /* [in] */ BSTR bstrName) = 0;
        
        virtual /* [helpstring][id][propget] */ HRESULT STDMETHODCALLTYPE get_LocalPath( 
            /* [retval][out] */ BSTR *pbstrLocalPath) = 0;
        
        virtual /* [helpstring][id][propget] */ HRESULT STDMETHODCALLTYPE get_FullPath( 
            /* [retval][out] */ BSTR *pbstrFullPath) = 0;
        
        virtual /* [helpstring][id][propget] */ HRESULT STDMETHODCALLTYPE get_URL( 
            /* [retval][out] */ BSTR *pbstrURL) = 0;
        
        virtual /* [hidden][helpstring][nonbrowsable][id][propget] */ HRESULT STDMETHODCALLTYPE get_Extender( 
            /* [in] */ BSTR ExtenderName,
            /* [retval][out] */ IDispatch **Extender) = 0;
        
        virtual /* [hidden][helpstring][nonbrowsable][id][propget] */ HRESULT STDMETHODCALLTYPE get_ExtenderNames( 
            /* [retval][out] */ VARIANT *ExtenderNames) = 0;
        
        virtual /* [hidden][helpstring][nonbrowsable][id][propget] */ HRESULT STDMETHODCALLTYPE get_ExtenderCATID( 
            /* [retval][out] */ BSTR *pRetval) = 0;
        
        virtual /* [helpstring][id][propget] */ HRESULT STDMETHODCALLTYPE get_WebReference( 
            /* [retval][out] */ BSTR *pbstrWebReferenceUrl) = 0;
        
        virtual /* [helpstring][id][propput] */ HRESULT STDMETHODCALLTYPE put_WebReference( 
            /* [in] */ BSTR bstrWebReferenceUrl) = 0;
        
        virtual /* [helpstring][nonbrowsable][id][propget] */ HRESULT STDMETHODCALLTYPE get_DefaultNamespace( 
            /* [retval][out] */ BSTR *pbstrNamespace) = 0;
        
        virtual /* [helpstring][id][propget] */ HRESULT STDMETHODCALLTYPE get_UrlBehavior( 
            /* [retval][out] */ webrefUrlBehavior *pUrlBehavior) = 0;
        
        virtual /* [helpstring][id][propput] */ HRESULT STDMETHODCALLTYPE put_UrlBehavior( 
            /* [in] */ webrefUrlBehavior urlBehavior) = 0;
        
    };
    
#else 	/* C style interface */

    typedef struct FolderPropertiesVtbl
    {
        BEGIN_INTERFACE
        
        HRESULT ( STDMETHODCALLTYPE *QueryInterface )( 
            FolderProperties * This,
            /* [in] */ REFIID riid,
            /* [iid_is][out] */ void **ppvObject);
        
        ULONG ( STDMETHODCALLTYPE *AddRef )( 
            FolderProperties * This);
        
        ULONG ( STDMETHODCALLTYPE *Release )( 
            FolderProperties * This);
        
        HRESULT ( STDMETHODCALLTYPE *GetTypeInfoCount )( 
            FolderProperties * This,
            /* [out] */ UINT *pctinfo);
        
        HRESULT ( STDMETHODCALLTYPE *GetTypeInfo )( 
            FolderProperties * This,
            /* [in] */ UINT iTInfo,
            /* [in] */ LCID lcid,
            /* [out] */ ITypeInfo **ppTInfo);
        
        HRESULT ( STDMETHODCALLTYPE *GetIDsOfNames )( 
            FolderProperties * This,
            /* [in] */ REFIID riid,
            /* [size_is][in] */ LPOLESTR *rgszNames,
            /* [in] */ UINT cNames,
            /* [in] */ LCID lcid,
            /* [size_is][out] */ DISPID *rgDispId);
        
        /* [local] */ HRESULT ( STDMETHODCALLTYPE *Invoke )( 
            FolderProperties * This,
            /* [in] */ DISPID dispIdMember,
            /* [in] */ REFIID riid,
            /* [in] */ LCID lcid,
            /* [in] */ WORD wFlags,
            /* [out][in] */ DISPPARAMS *pDispParams,
            /* [out] */ VARIANT *pVarResult,
            /* [out] */ EXCEPINFO *pExcepInfo,
            /* [out] */ UINT *puArgErr);
        
        /* [hidden][helpstring][id][propget] */ HRESULT ( STDMETHODCALLTYPE *get___id )( 
            FolderProperties * This,
            /* [retval][out] */ BSTR *pbstrName);
        
        /* [helpstring][id][propget] */ HRESULT ( STDMETHODCALLTYPE *get_FileName )( 
            FolderProperties * This,
            /* [retval][out] */ BSTR *pbstrName);
        
        /* [helpstring][id][propput] */ HRESULT ( STDMETHODCALLTYPE *put_FileName )( 
            FolderProperties * This,
            /* [in] */ BSTR bstrName);
        
        /* [helpstring][id][propget] */ HRESULT ( STDMETHODCALLTYPE *get_LocalPath )( 
            FolderProperties * This,
            /* [retval][out] */ BSTR *pbstrLocalPath);
        
        /* [helpstring][id][propget] */ HRESULT ( STDMETHODCALLTYPE *get_FullPath )( 
            FolderProperties * This,
            /* [retval][out] */ BSTR *pbstrFullPath);
        
        /* [helpstring][id][propget] */ HRESULT ( STDMETHODCALLTYPE *get_URL )( 
            FolderProperties * This,
            /* [retval][out] */ BSTR *pbstrURL);
        
        /* [hidden][helpstring][nonbrowsable][id][propget] */ HRESULT ( STDMETHODCALLTYPE *get_Extender )( 
            FolderProperties * This,
            /* [in] */ BSTR ExtenderName,
            /* [retval][out] */ IDispatch **Extender);
        
        /* [hidden][helpstring][nonbrowsable][id][propget] */ HRESULT ( STDMETHODCALLTYPE *get_ExtenderNames )( 
            FolderProperties * This,
            /* [retval][out] */ VARIANT *ExtenderNames);
        
        /* [hidden][helpstring][nonbrowsable][id][propget] */ HRESULT ( STDMETHODCALLTYPE *get_ExtenderCATID )( 
            FolderProperties * This,
            /* [retval][out] */ BSTR *pRetval);
        
        /* [helpstring][id][propget] */ HRESULT ( STDMETHODCALLTYPE *get_WebReference )( 
            FolderProperties * This,
            /* [retval][out] */ BSTR *pbstrWebReferenceUrl);
        
        /* [helpstring][id][propput] */ HRESULT ( STDMETHODCALLTYPE *put_WebReference )( 
            FolderProperties * This,
            /* [in] */ BSTR bstrWebReferenceUrl);
        
        /* [helpstring][nonbrowsable][id][propget] */ HRESULT ( STDMETHODCALLTYPE *get_DefaultNamespace )( 
            FolderProperties * This,
            /* [retval][out] */ BSTR *pbstrNamespace);
        
        /* [helpstring][id][propget] */ HRESULT ( STDMETHODCALLTYPE *get_UrlBehavior )( 
            FolderProperties * This,
            /* [retval][out] */ webrefUrlBehavior *pUrlBehavior);
        
        /* [helpstring][id][propput] */ HRESULT ( STDMETHODCALLTYPE *put_UrlBehavior )( 
            FolderProperties * This,
            /* [in] */ webrefUrlBehavior urlBehavior);
        
        END_INTERFACE
    } FolderPropertiesVtbl;

    interface FolderProperties
    {
        CONST_VTBL struct FolderPropertiesVtbl *lpVtbl;
    };

    

#ifdef COBJMACROS


#define FolderProperties_QueryInterface(This,riid,ppvObject)	\
    (This)->lpVtbl -> QueryInterface(This,riid,ppvObject)

#define FolderProperties_AddRef(This)	\
    (This)->lpVtbl -> AddRef(This)

#define FolderProperties_Release(This)	\
    (This)->lpVtbl -> Release(This)


#define FolderProperties_GetTypeInfoCount(This,pctinfo)	\
    (This)->lpVtbl -> GetTypeInfoCount(This,pctinfo)

#define FolderProperties_GetTypeInfo(This,iTInfo,lcid,ppTInfo)	\
    (This)->lpVtbl -> GetTypeInfo(This,iTInfo,lcid,ppTInfo)

#define FolderProperties_GetIDsOfNames(This,riid,rgszNames,cNames,lcid,rgDispId)	\
    (This)->lpVtbl -> GetIDsOfNames(This,riid,rgszNames,cNames,lcid,rgDispId)

#define FolderProperties_Invoke(This,dispIdMember,riid,lcid,wFlags,pDispParams,pVarResult,pExcepInfo,puArgErr)	\
    (This)->lpVtbl -> Invoke(This,dispIdMember,riid,lcid,wFlags,pDispParams,pVarResult,pExcepInfo,puArgErr)


#define FolderProperties_get___id(This,pbstrName)	\
    (This)->lpVtbl -> get___id(This,pbstrName)

#define FolderProperties_get_FileName(This,pbstrName)	\
    (This)->lpVtbl -> get_FileName(This,pbstrName)

#define FolderProperties_put_FileName(This,bstrName)	\
    (This)->lpVtbl -> put_FileName(This,bstrName)

#define FolderProperties_get_LocalPath(This,pbstrLocalPath)	\
    (This)->lpVtbl -> get_LocalPath(This,pbstrLocalPath)

#define FolderProperties_get_FullPath(This,pbstrFullPath)	\
    (This)->lpVtbl -> get_FullPath(This,pbstrFullPath)

#define FolderProperties_get_URL(This,pbstrURL)	\
    (This)->lpVtbl -> get_URL(This,pbstrURL)

#define FolderProperties_get_Extender(This,ExtenderName,Extender)	\
    (This)->lpVtbl -> get_Extender(This,ExtenderName,Extender)

#define FolderProperties_get_ExtenderNames(This,ExtenderNames)	\
    (This)->lpVtbl -> get_ExtenderNames(This,ExtenderNames)

#define FolderProperties_get_ExtenderCATID(This,pRetval)	\
    (This)->lpVtbl -> get_ExtenderCATID(This,pRetval)

#define FolderProperties_get_WebReference(This,pbstrWebReferenceUrl)	\
    (This)->lpVtbl -> get_WebReference(This,pbstrWebReferenceUrl)

#define FolderProperties_put_WebReference(This,bstrWebReferenceUrl)	\
    (This)->lpVtbl -> put_WebReference(This,bstrWebReferenceUrl)

#define FolderProperties_get_DefaultNamespace(This,pbstrNamespace)	\
    (This)->lpVtbl -> get_DefaultNamespace(This,pbstrNamespace)

#define FolderProperties_get_UrlBehavior(This,pUrlBehavior)	\
    (This)->lpVtbl -> get_UrlBehavior(This,pUrlBehavior)

#define FolderProperties_put_UrlBehavior(This,urlBehavior)	\
    (This)->lpVtbl -> put_UrlBehavior(This,urlBehavior)

#endif /* COBJMACROS */


#endif 	/* C style interface */



/* [hidden][helpstring][id][propget] */ HRESULT STDMETHODCALLTYPE FolderProperties_get___id_Proxy( 
    FolderProperties * This,
    /* [retval][out] */ BSTR *pbstrName);


void __RPC_STUB FolderProperties_get___id_Stub(
    IRpcStubBuffer *This,
    IRpcChannelBuffer *_pRpcChannelBuffer,
    PRPC_MESSAGE _pRpcMessage,
    DWORD *_pdwStubPhase);


/* [helpstring][id][propget] */ HRESULT STDMETHODCALLTYPE FolderProperties_get_FileName_Proxy( 
    FolderProperties * This,
    /* [retval][out] */ BSTR *pbstrName);


void __RPC_STUB FolderProperties_get_FileName_Stub(
    IRpcStubBuffer *This,
    IRpcChannelBuffer *_pRpcChannelBuffer,
    PRPC_MESSAGE _pRpcMessage,
    DWORD *_pdwStubPhase);


/* [helpstring][id][propput] */ HRESULT STDMETHODCALLTYPE FolderProperties_put_FileName_Proxy( 
    FolderProperties * This,
    /* [in] */ BSTR bstrName);


void __RPC_STUB FolderProperties_put_FileName_Stub(
    IRpcStubBuffer *This,
    IRpcChannelBuffer *_pRpcChannelBuffer,
    PRPC_MESSAGE _pRpcMessage,
    DWORD *_pdwStubPhase);


/* [helpstring][id][propget] */ HRESULT STDMETHODCALLTYPE FolderProperties_get_LocalPath_Proxy( 
    FolderProperties * This,
    /* [retval][out] */ BSTR *pbstrLocalPath);


void __RPC_STUB FolderProperties_get_LocalPath_Stub(
    IRpcStubBuffer *This,
    IRpcChannelBuffer *_pRpcChannelBuffer,
    PRPC_MESSAGE _pRpcMessage,
    DWORD *_pdwStubPhase);


/* [helpstring][id][propget] */ HRESULT STDMETHODCALLTYPE FolderProperties_get_FullPath_Proxy( 
    FolderProperties * This,
    /* [retval][out] */ BSTR *pbstrFullPath);


void __RPC_STUB FolderProperties_get_FullPath_Stub(
    IRpcStubBuffer *This,
    IRpcChannelBuffer *_pRpcChannelBuffer,
    PRPC_MESSAGE _pRpcMessage,
    DWORD *_pdwStubPhase);


/* [helpstring][id][propget] */ HRESULT STDMETHODCALLTYPE FolderProperties_get_URL_Proxy( 
    FolderProperties * This,
    /* [retval][out] */ BSTR *pbstrURL);


void __RPC_STUB FolderProperties_get_URL_Stub(
    IRpcStubBuffer *This,
    IRpcChannelBuffer *_pRpcChannelBuffer,
    PRPC_MESSAGE _pRpcMessage,
    DWORD *_pdwStubPhase);


/* [hidden][helpstring][nonbrowsable][id][propget] */ HRESULT STDMETHODCALLTYPE FolderProperties_get_Extender_Proxy( 
    FolderProperties * This,
    /* [in] */ BSTR ExtenderName,
    /* [retval][out] */ IDispatch **Extender);


void __RPC_STUB FolderProperties_get_Extender_Stub(
    IRpcStubBuffer *This,
    IRpcChannelBuffer *_pRpcChannelBuffer,
    PRPC_MESSAGE _pRpcMessage,
    DWORD *_pdwStubPhase);


/* [hidden][helpstring][nonbrowsable][id][propget] */ HRESULT STDMETHODCALLTYPE FolderProperties_get_ExtenderNames_Proxy( 
    FolderProperties * This,
    /* [retval][out] */ VARIANT *ExtenderNames);


void __RPC_STUB FolderProperties_get_ExtenderNames_Stub(
    IRpcStubBuffer *This,
    IRpcChannelBuffer *_pRpcChannelBuffer,
    PRPC_MESSAGE _pRpcMessage,
    DWORD *_pdwStubPhase);


/* [hidden][helpstring][nonbrowsable][id][propget] */ HRESULT STDMETHODCALLTYPE FolderProperties_get_ExtenderCATID_Proxy( 
    FolderProperties * This,
    /* [retval][out] */ BSTR *pRetval);


void __RPC_STUB FolderProperties_get_ExtenderCATID_Stub(
    IRpcStubBuffer *This,
    IRpcChannelBuffer *_pRpcChannelBuffer,
    PRPC_MESSAGE _pRpcMessage,
    DWORD *_pdwStubPhase);


/* [helpstring][id][propget] */ HRESULT STDMETHODCALLTYPE FolderProperties_get_WebReference_Proxy( 
    FolderProperties * This,
    /* [retval][out] */ BSTR *pbstrWebReferenceUrl);


void __RPC_STUB FolderProperties_get_WebReference_Stub(
    IRpcStubBuffer *This,
    IRpcChannelBuffer *_pRpcChannelBuffer,
    PRPC_MESSAGE _pRpcMessage,
    DWORD *_pdwStubPhase);


/* [helpstring][id][propput] */ HRESULT STDMETHODCALLTYPE FolderProperties_put_WebReference_Proxy( 
    FolderProperties * This,
    /* [in] */ BSTR bstrWebReferenceUrl);


void __RPC_STUB FolderProperties_put_WebReference_Stub(
    IRpcStubBuffer *This,
    IRpcChannelBuffer *_pRpcChannelBuffer,
    PRPC_MESSAGE _pRpcMessage,
    DWORD *_pdwStubPhase);


/* [helpstring][nonbrowsable][id][propget] */ HRESULT STDMETHODCALLTYPE FolderProperties_get_DefaultNamespace_Proxy( 
    FolderProperties * This,
    /* [retval][out] */ BSTR *pbstrNamespace);


void __RPC_STUB FolderProperties_get_DefaultNamespace_Stub(
    IRpcStubBuffer *This,
    IRpcChannelBuffer *_pRpcChannelBuffer,
    PRPC_MESSAGE _pRpcMessage,
    DWORD *_pdwStubPhase);


/* [helpstring][id][propget] */ HRESULT STDMETHODCALLTYPE FolderProperties_get_UrlBehavior_Proxy( 
    FolderProperties * This,
    /* [retval][out] */ webrefUrlBehavior *pUrlBehavior);


void __RPC_STUB FolderProperties_get_UrlBehavior_Stub(
    IRpcStubBuffer *This,
    IRpcChannelBuffer *_pRpcChannelBuffer,
    PRPC_MESSAGE _pRpcMessage,
    DWORD *_pdwStubPhase);


/* [helpstring][id][propput] */ HRESULT STDMETHODCALLTYPE FolderProperties_put_UrlBehavior_Proxy( 
    FolderProperties * This,
    /* [in] */ webrefUrlBehavior urlBehavior);


void __RPC_STUB FolderProperties_put_UrlBehavior_Stub(
    IRpcStubBuffer *This,
    IRpcChannelBuffer *_pRpcChannelBuffer,
    PRPC_MESSAGE _pRpcMessage,
    DWORD *_pdwStubPhase);



#endif 	/* __FolderProperties_INTERFACE_DEFINED__ */


#ifndef __Reference_INTERFACE_DEFINED__
#define __Reference_INTERFACE_DEFINED__

/* interface Reference */
/* [uuid][object][oleautomation][dual] */ 


EXTERN_C const IID IID_Reference;

#if defined(__cplusplus) && !defined(CINTERFACE)
    
    MIDL_INTERFACE("35D6FB50-35B6-4c81-B91C-3930B0D95386")
    Reference : public IDispatch
    {
    public:
        virtual /* [helpstring][propget][id] */ HRESULT STDMETHODCALLTYPE get_DTE( 
            /* [retval][out] */ /* external definition not present */ DTE **ppDTE) = 0;
        
        virtual /* [helpstring][propget][id] */ HRESULT STDMETHODCALLTYPE get_Collection( 
            /* [retval][out] */ References **ppProjectReferences) = 0;
        
        virtual /* [helpstring][propget][id] */ HRESULT STDMETHODCALLTYPE get_ContainingProject( 
            /* [retval][out] */ /* external definition not present */ Project **ppProject) = 0;
        
        virtual /* [helpstring][id] */ HRESULT STDMETHODCALLTYPE Remove( void) = 0;
        
        virtual /* [helpstring][propget][id] */ HRESULT STDMETHODCALLTYPE get_Name( 
            /* [retval][out] */ BSTR *pbstrName) = 0;
        
        virtual /* [helpstring][propget][id] */ HRESULT STDMETHODCALLTYPE get_Type( 
            /* [retval][out] */ prjReferenceType *pType) = 0;
        
        virtual /* [helpstring][propget][id] */ HRESULT STDMETHODCALLTYPE get_Identity( 
            /* [retval][out] */ BSTR *pbstrIdentity) = 0;
        
        virtual /* [helpstring][propget][id] */ HRESULT STDMETHODCALLTYPE get_Path( 
            /* [retval][out] */ BSTR *pbstrPath) = 0;
        
        virtual /* [helpstring][propget][id] */ HRESULT STDMETHODCALLTYPE get_Description( 
            /* [retval][out] */ BSTR *pbstrDesc) = 0;
        
        virtual /* [helpstring][propget][id] */ HRESULT STDMETHODCALLTYPE get_Culture( 
            /* [retval][out] */ BSTR *pbstrCulture) = 0;
        
        virtual /* [helpstring][propget][id] */ HRESULT STDMETHODCALLTYPE get_MajorVersion( 
            /* [retval][out] */ long *plMajorVer) = 0;
        
        virtual /* [helpstring][propget][id] */ HRESULT STDMETHODCALLTYPE get_MinorVersion( 
            /* [retval][out] */ long *plMinorVer) = 0;
        
        virtual /* [helpstring][propget][id] */ HRESULT STDMETHODCALLTYPE get_RevisionNumber( 
            /* [retval][out] */ long *plRevNo) = 0;
        
        virtual /* [helpstring][propget][id] */ HRESULT STDMETHODCALLTYPE get_BuildNumber( 
            /* [retval][out] */ long *plBuildNo) = 0;
        
        virtual /* [helpstring][propget][id] */ HRESULT STDMETHODCALLTYPE get_StrongName( 
            /* [retval][out] */ VARIANT_BOOL *pfStrongName) = 0;
        
        virtual /* [helpstring][propget][id] */ HRESULT STDMETHODCALLTYPE get_SourceProject( 
            /* [retval][out] */ /* external definition not present */ Project **ppProject) = 0;
        
        virtual /* [helpstring][propget][id] */ HRESULT STDMETHODCALLTYPE get_CopyLocal( 
            /* [retval][out] */ VARIANT_BOOL *pbCopyLocal) = 0;
        
        virtual /* [helpstring][propput][id] */ HRESULT STDMETHODCALLTYPE put_CopyLocal( 
            /* [in] */ VARIANT_BOOL bCopyLocal) = 0;
        
        virtual /* [helpstring][hidden][nonbrowsable][propget][id] */ HRESULT STDMETHODCALLTYPE get_Extender( 
            /* [in] */ BSTR ExtenderName,
            /* [retval][out] */ IDispatch **Extender) = 0;
        
        virtual /* [helpstring][hidden][nonbrowsable][propget][id] */ HRESULT STDMETHODCALLTYPE get_ExtenderNames( 
            /* [retval][out] */ VARIANT *ExtenderNames) = 0;
        
        virtual /* [helpstring][hidden][nonbrowsable][propget][id] */ HRESULT STDMETHODCALLTYPE get_ExtenderCATID( 
            /* [retval][out] */ BSTR *pRetval) = 0;
        
        virtual /* [helpstring][hidden][nonbrowsable][propget][id] */ HRESULT STDMETHODCALLTYPE get_PublicKeyToken( 
            /* [retval][out] */ BSTR *pbstrPublicKeyToken) = 0;
        
        virtual /* [helpstring][propget][id] */ HRESULT STDMETHODCALLTYPE get_Version( 
            /* [retval][out] */ BSTR *pbstrVersion) = 0;
        
    };
    
#else 	/* C style interface */

    typedef struct ReferenceVtbl
    {
        BEGIN_INTERFACE
        
        HRESULT ( STDMETHODCALLTYPE *QueryInterface )( 
            Reference * This,
            /* [in] */ REFIID riid,
            /* [iid_is][out] */ void **ppvObject);
        
        ULONG ( STDMETHODCALLTYPE *AddRef )( 
            Reference * This);
        
        ULONG ( STDMETHODCALLTYPE *Release )( 
            Reference * This);
        
        HRESULT ( STDMETHODCALLTYPE *GetTypeInfoCount )( 
            Reference * This,
            /* [out] */ UINT *pctinfo);
        
        HRESULT ( STDMETHODCALLTYPE *GetTypeInfo )( 
            Reference * This,
            /* [in] */ UINT iTInfo,
            /* [in] */ LCID lcid,
            /* [out] */ ITypeInfo **ppTInfo);
        
        HRESULT ( STDMETHODCALLTYPE *GetIDsOfNames )( 
            Reference * This,
            /* [in] */ REFIID riid,
            /* [size_is][in] */ LPOLESTR *rgszNames,
            /* [in] */ UINT cNames,
            /* [in] */ LCID lcid,
            /* [size_is][out] */ DISPID *rgDispId);
        
        /* [local] */ HRESULT ( STDMETHODCALLTYPE *Invoke )( 
            Reference * This,
            /* [in] */ DISPID dispIdMember,
            /* [in] */ REFIID riid,
            /* [in] */ LCID lcid,
            /* [in] */ WORD wFlags,
            /* [out][in] */ DISPPARAMS *pDispParams,
            /* [out] */ VARIANT *pVarResult,
            /* [out] */ EXCEPINFO *pExcepInfo,
            /* [out] */ UINT *puArgErr);
        
        /* [helpstring][propget][id] */ HRESULT ( STDMETHODCALLTYPE *get_DTE )( 
            Reference * This,
            /* [retval][out] */ /* external definition not present */ DTE **ppDTE);
        
        /* [helpstring][propget][id] */ HRESULT ( STDMETHODCALLTYPE *get_Collection )( 
            Reference * This,
            /* [retval][out] */ References **ppProjectReferences);
        
        /* [helpstring][propget][id] */ HRESULT ( STDMETHODCALLTYPE *get_ContainingProject )( 
            Reference * This,
            /* [retval][out] */ /* external definition not present */ Project **ppProject);
        
        /* [helpstring][id] */ HRESULT ( STDMETHODCALLTYPE *Remove )( 
            Reference * This);
        
        /* [helpstring][propget][id] */ HRESULT ( STDMETHODCALLTYPE *get_Name )( 
            Reference * This,
            /* [retval][out] */ BSTR *pbstrName);
        
        /* [helpstring][propget][id] */ HRESULT ( STDMETHODCALLTYPE *get_Type )( 
            Reference * This,
            /* [retval][out] */ prjReferenceType *pType);
        
        /* [helpstring][propget][id] */ HRESULT ( STDMETHODCALLTYPE *get_Identity )( 
            Reference * This,
            /* [retval][out] */ BSTR *pbstrIdentity);
        
        /* [helpstring][propget][id] */ HRESULT ( STDMETHODCALLTYPE *get_Path )( 
            Reference * This,
            /* [retval][out] */ BSTR *pbstrPath);
        
        /* [helpstring][propget][id] */ HRESULT ( STDMETHODCALLTYPE *get_Description )( 
            Reference * This,
            /* [retval][out] */ BSTR *pbstrDesc);
        
        /* [helpstring][propget][id] */ HRESULT ( STDMETHODCALLTYPE *get_Culture )( 
            Reference * This,
            /* [retval][out] */ BSTR *pbstrCulture);
        
        /* [helpstring][propget][id] */ HRESULT ( STDMETHODCALLTYPE *get_MajorVersion )( 
            Reference * This,
            /* [retval][out] */ long *plMajorVer);
        
        /* [helpstring][propget][id] */ HRESULT ( STDMETHODCALLTYPE *get_MinorVersion )( 
            Reference * This,
            /* [retval][out] */ long *plMinorVer);
        
        /* [helpstring][propget][id] */ HRESULT ( STDMETHODCALLTYPE *get_RevisionNumber )( 
            Reference * This,
            /* [retval][out] */ long *plRevNo);
        
        /* [helpstring][propget][id] */ HRESULT ( STDMETHODCALLTYPE *get_BuildNumber )( 
            Reference * This,
            /* [retval][out] */ long *plBuildNo);
        
        /* [helpstring][propget][id] */ HRESULT ( STDMETHODCALLTYPE *get_StrongName )( 
            Reference * This,
            /* [retval][out] */ VARIANT_BOOL *pfStrongName);
        
        /* [helpstring][propget][id] */ HRESULT ( STDMETHODCALLTYPE *get_SourceProject )( 
            Reference * This,
            /* [retval][out] */ /* external definition not present */ Project **ppProject);
        
        /* [helpstring][propget][id] */ HRESULT ( STDMETHODCALLTYPE *get_CopyLocal )( 
            Reference * This,
            /* [retval][out] */ VARIANT_BOOL *pbCopyLocal);
        
        /* [helpstring][propput][id] */ HRESULT ( STDMETHODCALLTYPE *put_CopyLocal )( 
            Reference * This,
            /* [in] */ VARIANT_BOOL bCopyLocal);
        
        /* [helpstring][hidden][nonbrowsable][propget][id] */ HRESULT ( STDMETHODCALLTYPE *get_Extender )( 
            Reference * This,
            /* [in] */ BSTR ExtenderName,
            /* [retval][out] */ IDispatch **Extender);
        
        /* [helpstring][hidden][nonbrowsable][propget][id] */ HRESULT ( STDMETHODCALLTYPE *get_ExtenderNames )( 
            Reference * This,
            /* [retval][out] */ VARIANT *ExtenderNames);
        
        /* [helpstring][hidden][nonbrowsable][propget][id] */ HRESULT ( STDMETHODCALLTYPE *get_ExtenderCATID )( 
            Reference * This,
            /* [retval][out] */ BSTR *pRetval);
        
        /* [helpstring][hidden][nonbrowsable][propget][id] */ HRESULT ( STDMETHODCALLTYPE *get_PublicKeyToken )( 
            Reference * This,
            /* [retval][out] */ BSTR *pbstrPublicKeyToken);
        
        /* [helpstring][propget][id] */ HRESULT ( STDMETHODCALLTYPE *get_Version )( 
            Reference * This,
            /* [retval][out] */ BSTR *pbstrVersion);
        
        END_INTERFACE
    } ReferenceVtbl;

    interface Reference
    {
        CONST_VTBL struct ReferenceVtbl *lpVtbl;
    };

    

#ifdef COBJMACROS


#define Reference_QueryInterface(This,riid,ppvObject)	\
    (This)->lpVtbl -> QueryInterface(This,riid,ppvObject)

#define Reference_AddRef(This)	\
    (This)->lpVtbl -> AddRef(This)

#define Reference_Release(This)	\
    (This)->lpVtbl -> Release(This)


#define Reference_GetTypeInfoCount(This,pctinfo)	\
    (This)->lpVtbl -> GetTypeInfoCount(This,pctinfo)

#define Reference_GetTypeInfo(This,iTInfo,lcid,ppTInfo)	\
    (This)->lpVtbl -> GetTypeInfo(This,iTInfo,lcid,ppTInfo)

#define Reference_GetIDsOfNames(This,riid,rgszNames,cNames,lcid,rgDispId)	\
    (This)->lpVtbl -> GetIDsOfNames(This,riid,rgszNames,cNames,lcid,rgDispId)

#define Reference_Invoke(This,dispIdMember,riid,lcid,wFlags,pDispParams,pVarResult,pExcepInfo,puArgErr)	\
    (This)->lpVtbl -> Invoke(This,dispIdMember,riid,lcid,wFlags,pDispParams,pVarResult,pExcepInfo,puArgErr)


#define Reference_get_DTE(This,ppDTE)	\
    (This)->lpVtbl -> get_DTE(This,ppDTE)

#define Reference_get_Collection(This,ppProjectReferences)	\
    (This)->lpVtbl -> get_Collection(This,ppProjectReferences)

#define Reference_get_ContainingProject(This,ppProject)	\
    (This)->lpVtbl -> get_ContainingProject(This,ppProject)

#define Reference_Remove(This)	\
    (This)->lpVtbl -> Remove(This)

#define Reference_get_Name(This,pbstrName)	\
    (This)->lpVtbl -> get_Name(This,pbstrName)

#define Reference_get_Type(This,pType)	\
    (This)->lpVtbl -> get_Type(This,pType)

#define Reference_get_Identity(This,pbstrIdentity)	\
    (This)->lpVtbl -> get_Identity(This,pbstrIdentity)

#define Reference_get_Path(This,pbstrPath)	\
    (This)->lpVtbl -> get_Path(This,pbstrPath)

#define Reference_get_Description(This,pbstrDesc)	\
    (This)->lpVtbl -> get_Description(This,pbstrDesc)

#define Reference_get_Culture(This,pbstrCulture)	\
    (This)->lpVtbl -> get_Culture(This,pbstrCulture)

#define Reference_get_MajorVersion(This,plMajorVer)	\
    (This)->lpVtbl -> get_MajorVersion(This,plMajorVer)

#define Reference_get_MinorVersion(This,plMinorVer)	\
    (This)->lpVtbl -> get_MinorVersion(This,plMinorVer)

#define Reference_get_RevisionNumber(This,plRevNo)	\
    (This)->lpVtbl -> get_RevisionNumber(This,plRevNo)

#define Reference_get_BuildNumber(This,plBuildNo)	\
    (This)->lpVtbl -> get_BuildNumber(This,plBuildNo)

#define Reference_get_StrongName(This,pfStrongName)	\
    (This)->lpVtbl -> get_StrongName(This,pfStrongName)

#define Reference_get_SourceProject(This,ppProject)	\
    (This)->lpVtbl -> get_SourceProject(This,ppProject)

#define Reference_get_CopyLocal(This,pbCopyLocal)	\
    (This)->lpVtbl -> get_CopyLocal(This,pbCopyLocal)

#define Reference_put_CopyLocal(This,bCopyLocal)	\
    (This)->lpVtbl -> put_CopyLocal(This,bCopyLocal)

#define Reference_get_Extender(This,ExtenderName,Extender)	\
    (This)->lpVtbl -> get_Extender(This,ExtenderName,Extender)

#define Reference_get_ExtenderNames(This,ExtenderNames)	\
    (This)->lpVtbl -> get_ExtenderNames(This,ExtenderNames)

#define Reference_get_ExtenderCATID(This,pRetval)	\
    (This)->lpVtbl -> get_ExtenderCATID(This,pRetval)

#define Reference_get_PublicKeyToken(This,pbstrPublicKeyToken)	\
    (This)->lpVtbl -> get_PublicKeyToken(This,pbstrPublicKeyToken)

#define Reference_get_Version(This,pbstrVersion)	\
    (This)->lpVtbl -> get_Version(This,pbstrVersion)

#endif /* COBJMACROS */


#endif 	/* C style interface */



/* [helpstring][propget][id] */ HRESULT STDMETHODCALLTYPE Reference_get_DTE_Proxy( 
    Reference * This,
    /* [retval][out] */ /* external definition not present */ DTE **ppDTE);


void __RPC_STUB Reference_get_DTE_Stub(
    IRpcStubBuffer *This,
    IRpcChannelBuffer *_pRpcChannelBuffer,
    PRPC_MESSAGE _pRpcMessage,
    DWORD *_pdwStubPhase);


/* [helpstring][propget][id] */ HRESULT STDMETHODCALLTYPE Reference_get_Collection_Proxy( 
    Reference * This,
    /* [retval][out] */ References **ppProjectReferences);


void __RPC_STUB Reference_get_Collection_Stub(
    IRpcStubBuffer *This,
    IRpcChannelBuffer *_pRpcChannelBuffer,
    PRPC_MESSAGE _pRpcMessage,
    DWORD *_pdwStubPhase);


/* [helpstring][propget][id] */ HRESULT STDMETHODCALLTYPE Reference_get_ContainingProject_Proxy( 
    Reference * This,
    /* [retval][out] */ /* external definition not present */ Project **ppProject);


void __RPC_STUB Reference_get_ContainingProject_Stub(
    IRpcStubBuffer *This,
    IRpcChannelBuffer *_pRpcChannelBuffer,
    PRPC_MESSAGE _pRpcMessage,
    DWORD *_pdwStubPhase);


/* [helpstring][id] */ HRESULT STDMETHODCALLTYPE Reference_Remove_Proxy( 
    Reference * This);


void __RPC_STUB Reference_Remove_Stub(
    IRpcStubBuffer *This,
    IRpcChannelBuffer *_pRpcChannelBuffer,
    PRPC_MESSAGE _pRpcMessage,
    DWORD *_pdwStubPhase);


/* [helpstring][propget][id] */ HRESULT STDMETHODCALLTYPE Reference_get_Name_Proxy( 
    Reference * This,
    /* [retval][out] */ BSTR *pbstrName);


void __RPC_STUB Reference_get_Name_Stub(
    IRpcStubBuffer *This,
    IRpcChannelBuffer *_pRpcChannelBuffer,
    PRPC_MESSAGE _pRpcMessage,
    DWORD *_pdwStubPhase);


/* [helpstring][propget][id] */ HRESULT STDMETHODCALLTYPE Reference_get_Type_Proxy( 
    Reference * This,
    /* [retval][out] */ prjReferenceType *pType);


void __RPC_STUB Reference_get_Type_Stub(
    IRpcStubBuffer *This,
    IRpcChannelBuffer *_pRpcChannelBuffer,
    PRPC_MESSAGE _pRpcMessage,
    DWORD *_pdwStubPhase);


/* [helpstring][propget][id] */ HRESULT STDMETHODCALLTYPE Reference_get_Identity_Proxy( 
    Reference * This,
    /* [retval][out] */ BSTR *pbstrIdentity);


void __RPC_STUB Reference_get_Identity_Stub(
    IRpcStubBuffer *This,
    IRpcChannelBuffer *_pRpcChannelBuffer,
    PRPC_MESSAGE _pRpcMessage,
    DWORD *_pdwStubPhase);


/* [helpstring][propget][id] */ HRESULT STDMETHODCALLTYPE Reference_get_Path_Proxy( 
    Reference * This,
    /* [retval][out] */ BSTR *pbstrPath);


void __RPC_STUB Reference_get_Path_Stub(
    IRpcStubBuffer *This,
    IRpcChannelBuffer *_pRpcChannelBuffer,
    PRPC_MESSAGE _pRpcMessage,
    DWORD *_pdwStubPhase);


/* [helpstring][propget][id] */ HRESULT STDMETHODCALLTYPE Reference_get_Description_Proxy( 
    Reference * This,
    /* [retval][out] */ BSTR *pbstrDesc);


void __RPC_STUB Reference_get_Description_Stub(
    IRpcStubBuffer *This,
    IRpcChannelBuffer *_pRpcChannelBuffer,
    PRPC_MESSAGE _pRpcMessage,
    DWORD *_pdwStubPhase);


/* [helpstring][propget][id] */ HRESULT STDMETHODCALLTYPE Reference_get_Culture_Proxy( 
    Reference * This,
    /* [retval][out] */ BSTR *pbstrCulture);


void __RPC_STUB Reference_get_Culture_Stub(
    IRpcStubBuffer *This,
    IRpcChannelBuffer *_pRpcChannelBuffer,
    PRPC_MESSAGE _pRpcMessage,
    DWORD *_pdwStubPhase);


/* [helpstring][propget][id] */ HRESULT STDMETHODCALLTYPE Reference_get_MajorVersion_Proxy( 
    Reference * This,
    /* [retval][out] */ long *plMajorVer);


void __RPC_STUB Reference_get_MajorVersion_Stub(
    IRpcStubBuffer *This,
    IRpcChannelBuffer *_pRpcChannelBuffer,
    PRPC_MESSAGE _pRpcMessage,
    DWORD *_pdwStubPhase);


/* [helpstring][propget][id] */ HRESULT STDMETHODCALLTYPE Reference_get_MinorVersion_Proxy( 
    Reference * This,
    /* [retval][out] */ long *plMinorVer);


void __RPC_STUB Reference_get_MinorVersion_Stub(
    IRpcStubBuffer *This,
    IRpcChannelBuffer *_pRpcChannelBuffer,
    PRPC_MESSAGE _pRpcMessage,
    DWORD *_pdwStubPhase);


/* [helpstring][propget][id] */ HRESULT STDMETHODCALLTYPE Reference_get_RevisionNumber_Proxy( 
    Reference * This,
    /* [retval][out] */ long *plRevNo);


void __RPC_STUB Reference_get_RevisionNumber_Stub(
    IRpcStubBuffer *This,
    IRpcChannelBuffer *_pRpcChannelBuffer,
    PRPC_MESSAGE _pRpcMessage,
    DWORD *_pdwStubPhase);


/* [helpstring][propget][id] */ HRESULT STDMETHODCALLTYPE Reference_get_BuildNumber_Proxy( 
    Reference * This,
    /* [retval][out] */ long *plBuildNo);


void __RPC_STUB Reference_get_BuildNumber_Stub(
    IRpcStubBuffer *This,
    IRpcChannelBuffer *_pRpcChannelBuffer,
    PRPC_MESSAGE _pRpcMessage,
    DWORD *_pdwStubPhase);


/* [helpstring][propget][id] */ HRESULT STDMETHODCALLTYPE Reference_get_StrongName_Proxy( 
    Reference * This,
    /* [retval][out] */ VARIANT_BOOL *pfStrongName);


void __RPC_STUB Reference_get_StrongName_Stub(
    IRpcStubBuffer *This,
    IRpcChannelBuffer *_pRpcChannelBuffer,
    PRPC_MESSAGE _pRpcMessage,
    DWORD *_pdwStubPhase);


/* [helpstring][propget][id] */ HRESULT STDMETHODCALLTYPE Reference_get_SourceProject_Proxy( 
    Reference * This,
    /* [retval][out] */ /* external definition not present */ Project **ppProject);


void __RPC_STUB Reference_get_SourceProject_Stub(
    IRpcStubBuffer *This,
    IRpcChannelBuffer *_pRpcChannelBuffer,
    PRPC_MESSAGE _pRpcMessage,
    DWORD *_pdwStubPhase);


/* [helpstring][propget][id] */ HRESULT STDMETHODCALLTYPE Reference_get_CopyLocal_Proxy( 
    Reference * This,
    /* [retval][out] */ VARIANT_BOOL *pbCopyLocal);


void __RPC_STUB Reference_get_CopyLocal_Stub(
    IRpcStubBuffer *This,
    IRpcChannelBuffer *_pRpcChannelBuffer,
    PRPC_MESSAGE _pRpcMessage,
    DWORD *_pdwStubPhase);


/* [helpstring][propput][id] */ HRESULT STDMETHODCALLTYPE Reference_put_CopyLocal_Proxy( 
    Reference * This,
    /* [in] */ VARIANT_BOOL bCopyLocal);


void __RPC_STUB Reference_put_CopyLocal_Stub(
    IRpcStubBuffer *This,
    IRpcChannelBuffer *_pRpcChannelBuffer,
    PRPC_MESSAGE _pRpcMessage,
    DWORD *_pdwStubPhase);


/* [helpstring][hidden][nonbrowsable][propget][id] */ HRESULT STDMETHODCALLTYPE Reference_get_Extender_Proxy( 
    Reference * This,
    /* [in] */ BSTR ExtenderName,
    /* [retval][out] */ IDispatch **Extender);


void __RPC_STUB Reference_get_Extender_Stub(
    IRpcStubBuffer *This,
    IRpcChannelBuffer *_pRpcChannelBuffer,
    PRPC_MESSAGE _pRpcMessage,
    DWORD *_pdwStubPhase);


/* [helpstring][hidden][nonbrowsable][propget][id] */ HRESULT STDMETHODCALLTYPE Reference_get_ExtenderNames_Proxy( 
    Reference * This,
    /* [retval][out] */ VARIANT *ExtenderNames);


void __RPC_STUB Reference_get_ExtenderNames_Stub(
    IRpcStubBuffer *This,
    IRpcChannelBuffer *_pRpcChannelBuffer,
    PRPC_MESSAGE _pRpcMessage,
    DWORD *_pdwStubPhase);


/* [helpstring][hidden][nonbrowsable][propget][id] */ HRESULT STDMETHODCALLTYPE Reference_get_ExtenderCATID_Proxy( 
    Reference * This,
    /* [retval][out] */ BSTR *pRetval);


void __RPC_STUB Reference_get_ExtenderCATID_Stub(
    IRpcStubBuffer *This,
    IRpcChannelBuffer *_pRpcChannelBuffer,
    PRPC_MESSAGE _pRpcMessage,
    DWORD *_pdwStubPhase);


/* [helpstring][hidden][nonbrowsable][propget][id] */ HRESULT STDMETHODCALLTYPE Reference_get_PublicKeyToken_Proxy( 
    Reference * This,
    /* [retval][out] */ BSTR *pbstrPublicKeyToken);


void __RPC_STUB Reference_get_PublicKeyToken_Stub(
    IRpcStubBuffer *This,
    IRpcChannelBuffer *_pRpcChannelBuffer,
    PRPC_MESSAGE _pRpcMessage,
    DWORD *_pdwStubPhase);


/* [helpstring][propget][id] */ HRESULT STDMETHODCALLTYPE Reference_get_Version_Proxy( 
    Reference * This,
    /* [retval][out] */ BSTR *pbstrVersion);


void __RPC_STUB Reference_get_Version_Stub(
    IRpcStubBuffer *This,
    IRpcChannelBuffer *_pRpcChannelBuffer,
    PRPC_MESSAGE _pRpcMessage,
    DWORD *_pdwStubPhase);



#endif 	/* __Reference_INTERFACE_DEFINED__ */


#ifndef __References_INTERFACE_DEFINED__
#define __References_INTERFACE_DEFINED__

/* interface References */
/* [uuid][object][oleautomation][dual] */ 


EXTERN_C const IID IID_References;

#if defined(__cplusplus) && !defined(CINTERFACE)
    
    MIDL_INTERFACE("B8758EE4-0553-4bc9-8432-440449D35C14")
    References : public IDispatch
    {
    public:
        virtual /* [helpstring][propget][id] */ HRESULT STDMETHODCALLTYPE get_DTE( 
            /* [retval][out] */ /* external definition not present */ DTE **ppDTE) = 0;
        
        virtual /* [helpstring][propget][id] */ HRESULT STDMETHODCALLTYPE get_Parent( 
            /* [retval][out] */ IDispatch **ppdispParent) = 0;
        
        virtual /* [helpstring][propget][id] */ HRESULT STDMETHODCALLTYPE get_ContainingProject( 
            /* [retval][out] */ /* external definition not present */ Project **ppProject) = 0;
        
        virtual /* [helpstring][propget][id] */ HRESULT STDMETHODCALLTYPE get_Count( 
            /* [retval][out] */ long *plCount) = 0;
        
        virtual /* [helpstring][id] */ HRESULT STDMETHODCALLTYPE Item( 
            /* [in] */ VARIANT index,
            /* [retval][out] */ Reference **ppProjectReference) = 0;
        
        virtual /* [id][restricted] */ HRESULT STDMETHODCALLTYPE _NewEnum( 
            /* [retval][out] */ IUnknown **ppiuReturn) = 0;
        
        virtual /* [helpstring][id] */ HRESULT STDMETHODCALLTYPE Find( 
            /* [in] */ BSTR bstrIdentity,
            /* [retval][out] */ Reference **ppProjectReference) = 0;
        
        virtual /* [helpstring][id] */ HRESULT STDMETHODCALLTYPE Add( 
            /* [in] */ BSTR bstrPath,
            /* [retval][out] */ Reference **ppProjectReference) = 0;
        
        virtual /* [helpstring][id] */ HRESULT STDMETHODCALLTYPE AddActiveX( 
            /* [in] */ BSTR bstrTypeLibGuid,
            /* [defaultvalue][in] */ long lMajorVer,
            /* [defaultvalue][in] */ long lMinorVer,
            /* [defaultvalue][in] */ long lLocaleId,
            /* [defaultvalue][in] */ BSTR bstrWrapperTool,
            /* [retval][out] */ Reference **ppProjectReference) = 0;
        
        virtual /* [helpstring][id] */ HRESULT STDMETHODCALLTYPE AddProject( 
            /* [in] */ /* external definition not present */ Project *pProject,
            /* [retval][out] */ Reference **ppProjectReference) = 0;
        
    };
    
#else 	/* C style interface */

    typedef struct ReferencesVtbl
    {
        BEGIN_INTERFACE
        
        HRESULT ( STDMETHODCALLTYPE *QueryInterface )( 
            References * This,
            /* [in] */ REFIID riid,
            /* [iid_is][out] */ void **ppvObject);
        
        ULONG ( STDMETHODCALLTYPE *AddRef )( 
            References * This);
        
        ULONG ( STDMETHODCALLTYPE *Release )( 
            References * This);
        
        HRESULT ( STDMETHODCALLTYPE *GetTypeInfoCount )( 
            References * This,
            /* [out] */ UINT *pctinfo);
        
        HRESULT ( STDMETHODCALLTYPE *GetTypeInfo )( 
            References * This,
            /* [in] */ UINT iTInfo,
            /* [in] */ LCID lcid,
            /* [out] */ ITypeInfo **ppTInfo);
        
        HRESULT ( STDMETHODCALLTYPE *GetIDsOfNames )( 
            References * This,
            /* [in] */ REFIID riid,
            /* [size_is][in] */ LPOLESTR *rgszNames,
            /* [in] */ UINT cNames,
            /* [in] */ LCID lcid,
            /* [size_is][out] */ DISPID *rgDispId);
        
        /* [local] */ HRESULT ( STDMETHODCALLTYPE *Invoke )( 
            References * This,
            /* [in] */ DISPID dispIdMember,
            /* [in] */ REFIID riid,
            /* [in] */ LCID lcid,
            /* [in] */ WORD wFlags,
            /* [out][in] */ DISPPARAMS *pDispParams,
            /* [out] */ VARIANT *pVarResult,
            /* [out] */ EXCEPINFO *pExcepInfo,
            /* [out] */ UINT *puArgErr);
        
        /* [helpstring][propget][id] */ HRESULT ( STDMETHODCALLTYPE *get_DTE )( 
            References * This,
            /* [retval][out] */ /* external definition not present */ DTE **ppDTE);
        
        /* [helpstring][propget][id] */ HRESULT ( STDMETHODCALLTYPE *get_Parent )( 
            References * This,
            /* [retval][out] */ IDispatch **ppdispParent);
        
        /* [helpstring][propget][id] */ HRESULT ( STDMETHODCALLTYPE *get_ContainingProject )( 
            References * This,
            /* [retval][out] */ /* external definition not present */ Project **ppProject);
        
        /* [helpstring][propget][id] */ HRESULT ( STDMETHODCALLTYPE *get_Count )( 
            References * This,
            /* [retval][out] */ long *plCount);
        
        /* [helpstring][id] */ HRESULT ( STDMETHODCALLTYPE *Item )( 
            References * This,
            /* [in] */ VARIANT index,
            /* [retval][out] */ Reference **ppProjectReference);
        
        /* [id][restricted] */ HRESULT ( STDMETHODCALLTYPE *_NewEnum )( 
            References * This,
            /* [retval][out] */ IUnknown **ppiuReturn);
        
        /* [helpstring][id] */ HRESULT ( STDMETHODCALLTYPE *Find )( 
            References * This,
            /* [in] */ BSTR bstrIdentity,
            /* [retval][out] */ Reference **ppProjectReference);
        
        /* [helpstring][id] */ HRESULT ( STDMETHODCALLTYPE *Add )( 
            References * This,
            /* [in] */ BSTR bstrPath,
            /* [retval][out] */ Reference **ppProjectReference);
        
        /* [helpstring][id] */ HRESULT ( STDMETHODCALLTYPE *AddActiveX )( 
            References * This,
            /* [in] */ BSTR bstrTypeLibGuid,
            /* [defaultvalue][in] */ long lMajorVer,
            /* [defaultvalue][in] */ long lMinorVer,
            /* [defaultvalue][in] */ long lLocaleId,
            /* [defaultvalue][in] */ BSTR bstrWrapperTool,
            /* [retval][out] */ Reference **ppProjectReference);
        
        /* [helpstring][id] */ HRESULT ( STDMETHODCALLTYPE *AddProject )( 
            References * This,
            /* [in] */ /* external definition not present */ Project *pProject,
            /* [retval][out] */ Reference **ppProjectReference);
        
        END_INTERFACE
    } ReferencesVtbl;

    interface References
    {
        CONST_VTBL struct ReferencesVtbl *lpVtbl;
    };

    

#ifdef COBJMACROS


#define References_QueryInterface(This,riid,ppvObject)	\
    (This)->lpVtbl -> QueryInterface(This,riid,ppvObject)

#define References_AddRef(This)	\
    (This)->lpVtbl -> AddRef(This)

#define References_Release(This)	\
    (This)->lpVtbl -> Release(This)


#define References_GetTypeInfoCount(This,pctinfo)	\
    (This)->lpVtbl -> GetTypeInfoCount(This,pctinfo)

#define References_GetTypeInfo(This,iTInfo,lcid,ppTInfo)	\
    (This)->lpVtbl -> GetTypeInfo(This,iTInfo,lcid,ppTInfo)

#define References_GetIDsOfNames(This,riid,rgszNames,cNames,lcid,rgDispId)	\
    (This)->lpVtbl -> GetIDsOfNames(This,riid,rgszNames,cNames,lcid,rgDispId)

#define References_Invoke(This,dispIdMember,riid,lcid,wFlags,pDispParams,pVarResult,pExcepInfo,puArgErr)	\
    (This)->lpVtbl -> Invoke(This,dispIdMember,riid,lcid,wFlags,pDispParams,pVarResult,pExcepInfo,puArgErr)


#define References_get_DTE(This,ppDTE)	\
    (This)->lpVtbl -> get_DTE(This,ppDTE)

#define References_get_Parent(This,ppdispParent)	\
    (This)->lpVtbl -> get_Parent(This,ppdispParent)

#define References_get_ContainingProject(This,ppProject)	\
    (This)->lpVtbl -> get_ContainingProject(This,ppProject)

#define References_get_Count(This,plCount)	\
    (This)->lpVtbl -> get_Count(This,plCount)

#define References_Item(This,index,ppProjectReference)	\
    (This)->lpVtbl -> Item(This,index,ppProjectReference)

#define References__NewEnum(This,ppiuReturn)	\
    (This)->lpVtbl -> _NewEnum(This,ppiuReturn)

#define References_Find(This,bstrIdentity,ppProjectReference)	\
    (This)->lpVtbl -> Find(This,bstrIdentity,ppProjectReference)

#define References_Add(This,bstrPath,ppProjectReference)	\
    (This)->lpVtbl -> Add(This,bstrPath,ppProjectReference)

#define References_AddActiveX(This,bstrTypeLibGuid,lMajorVer,lMinorVer,lLocaleId,bstrWrapperTool,ppProjectReference)	\
    (This)->lpVtbl -> AddActiveX(This,bstrTypeLibGuid,lMajorVer,lMinorVer,lLocaleId,bstrWrapperTool,ppProjectReference)

#define References_AddProject(This,pProject,ppProjectReference)	\
    (This)->lpVtbl -> AddProject(This,pProject,ppProjectReference)

#endif /* COBJMACROS */


#endif 	/* C style interface */



/* [helpstring][propget][id] */ HRESULT STDMETHODCALLTYPE References_get_DTE_Proxy( 
    References * This,
    /* [retval][out] */ /* external definition not present */ DTE **ppDTE);


void __RPC_STUB References_get_DTE_Stub(
    IRpcStubBuffer *This,
    IRpcChannelBuffer *_pRpcChannelBuffer,
    PRPC_MESSAGE _pRpcMessage,
    DWORD *_pdwStubPhase);


/* [helpstring][propget][id] */ HRESULT STDMETHODCALLTYPE References_get_Parent_Proxy( 
    References * This,
    /* [retval][out] */ IDispatch **ppdispParent);


void __RPC_STUB References_get_Parent_Stub(
    IRpcStubBuffer *This,
    IRpcChannelBuffer *_pRpcChannelBuffer,
    PRPC_MESSAGE _pRpcMessage,
    DWORD *_pdwStubPhase);


/* [helpstring][propget][id] */ HRESULT STDMETHODCALLTYPE References_get_ContainingProject_Proxy( 
    References * This,
    /* [retval][out] */ /* external definition not present */ Project **ppProject);


void __RPC_STUB References_get_ContainingProject_Stub(
    IRpcStubBuffer *This,
    IRpcChannelBuffer *_pRpcChannelBuffer,
    PRPC_MESSAGE _pRpcMessage,
    DWORD *_pdwStubPhase);


/* [helpstring][propget][id] */ HRESULT STDMETHODCALLTYPE References_get_Count_Proxy( 
    References * This,
    /* [retval][out] */ long *plCount);


void __RPC_STUB References_get_Count_Stub(
    IRpcStubBuffer *This,
    IRpcChannelBuffer *_pRpcChannelBuffer,
    PRPC_MESSAGE _pRpcMessage,
    DWORD *_pdwStubPhase);


/* [helpstring][id] */ HRESULT STDMETHODCALLTYPE References_Item_Proxy( 
    References * This,
    /* [in] */ VARIANT index,
    /* [retval][out] */ Reference **ppProjectReference);


void __RPC_STUB References_Item_Stub(
    IRpcStubBuffer *This,
    IRpcChannelBuffer *_pRpcChannelBuffer,
    PRPC_MESSAGE _pRpcMessage,
    DWORD *_pdwStubPhase);


/* [id][restricted] */ HRESULT STDMETHODCALLTYPE References__NewEnum_Proxy( 
    References * This,
    /* [retval][out] */ IUnknown **ppiuReturn);


void __RPC_STUB References__NewEnum_Stub(
    IRpcStubBuffer *This,
    IRpcChannelBuffer *_pRpcChannelBuffer,
    PRPC_MESSAGE _pRpcMessage,
    DWORD *_pdwStubPhase);


/* [helpstring][id] */ HRESULT STDMETHODCALLTYPE References_Find_Proxy( 
    References * This,
    /* [in] */ BSTR bstrIdentity,
    /* [retval][out] */ Reference **ppProjectReference);


void __RPC_STUB References_Find_Stub(
    IRpcStubBuffer *This,
    IRpcChannelBuffer *_pRpcChannelBuffer,
    PRPC_MESSAGE _pRpcMessage,
    DWORD *_pdwStubPhase);


/* [helpstring][id] */ HRESULT STDMETHODCALLTYPE References_Add_Proxy( 
    References * This,
    /* [in] */ BSTR bstrPath,
    /* [retval][out] */ Reference **ppProjectReference);


void __RPC_STUB References_Add_Stub(
    IRpcStubBuffer *This,
    IRpcChannelBuffer *_pRpcChannelBuffer,
    PRPC_MESSAGE _pRpcMessage,
    DWORD *_pdwStubPhase);


/* [helpstring][id] */ HRESULT STDMETHODCALLTYPE References_AddActiveX_Proxy( 
    References * This,
    /* [in] */ BSTR bstrTypeLibGuid,
    /* [defaultvalue][in] */ long lMajorVer,
    /* [defaultvalue][in] */ long lMinorVer,
    /* [defaultvalue][in] */ long lLocaleId,
    /* [defaultvalue][in] */ BSTR bstrWrapperTool,
    /* [retval][out] */ Reference **ppProjectReference);


void __RPC_STUB References_AddActiveX_Stub(
    IRpcStubBuffer *This,
    IRpcChannelBuffer *_pRpcChannelBuffer,
    PRPC_MESSAGE _pRpcMessage,
    DWORD *_pdwStubPhase);


/* [helpstring][id] */ HRESULT STDMETHODCALLTYPE References_AddProject_Proxy( 
    References * This,
    /* [in] */ /* external definition not present */ Project *pProject,
    /* [retval][out] */ Reference **ppProjectReference);


void __RPC_STUB References_AddProject_Stub(
    IRpcStubBuffer *This,
    IRpcChannelBuffer *_pRpcChannelBuffer,
    PRPC_MESSAGE _pRpcMessage,
    DWORD *_pdwStubPhase);



#endif 	/* __References_INTERFACE_DEFINED__ */


#ifndef ___ReferencesEvents_INTERFACE_DEFINED__
#define ___ReferencesEvents_INTERFACE_DEFINED__

/* interface _ReferencesEvents */
/* [object][dual][uuid] */ 


EXTERN_C const IID IID__ReferencesEvents;

#if defined(__cplusplus) && !defined(CINTERFACE)
    
    MIDL_INTERFACE("1CF40C9E-D548-4b45-AD0F-3D7843F62BBB")
    _ReferencesEvents : public IDispatch
    {
    public:
    };
    
#else 	/* C style interface */

    typedef struct _ReferencesEventsVtbl
    {
        BEGIN_INTERFACE
        
        HRESULT ( STDMETHODCALLTYPE *QueryInterface )( 
            _ReferencesEvents * This,
            /* [in] */ REFIID riid,
            /* [iid_is][out] */ void **ppvObject);
        
        ULONG ( STDMETHODCALLTYPE *AddRef )( 
            _ReferencesEvents * This);
        
        ULONG ( STDMETHODCALLTYPE *Release )( 
            _ReferencesEvents * This);
        
        HRESULT ( STDMETHODCALLTYPE *GetTypeInfoCount )( 
            _ReferencesEvents * This,
            /* [out] */ UINT *pctinfo);
        
        HRESULT ( STDMETHODCALLTYPE *GetTypeInfo )( 
            _ReferencesEvents * This,
            /* [in] */ UINT iTInfo,
            /* [in] */ LCID lcid,
            /* [out] */ ITypeInfo **ppTInfo);
        
        HRESULT ( STDMETHODCALLTYPE *GetIDsOfNames )( 
            _ReferencesEvents * This,
            /* [in] */ REFIID riid,
            /* [size_is][in] */ LPOLESTR *rgszNames,
            /* [in] */ UINT cNames,
            /* [in] */ LCID lcid,
            /* [size_is][out] */ DISPID *rgDispId);
        
        /* [local] */ HRESULT ( STDMETHODCALLTYPE *Invoke )( 
            _ReferencesEvents * This,
            /* [in] */ DISPID dispIdMember,
            /* [in] */ REFIID riid,
            /* [in] */ LCID lcid,
            /* [in] */ WORD wFlags,
            /* [out][in] */ DISPPARAMS *pDispParams,
            /* [out] */ VARIANT *pVarResult,
            /* [out] */ EXCEPINFO *pExcepInfo,
            /* [out] */ UINT *puArgErr);
        
        END_INTERFACE
    } _ReferencesEventsVtbl;

    interface _ReferencesEvents
    {
        CONST_VTBL struct _ReferencesEventsVtbl *lpVtbl;
    };

    

#ifdef COBJMACROS


#define _ReferencesEvents_QueryInterface(This,riid,ppvObject)	\
    (This)->lpVtbl -> QueryInterface(This,riid,ppvObject)

#define _ReferencesEvents_AddRef(This)	\
    (This)->lpVtbl -> AddRef(This)

#define _ReferencesEvents_Release(This)	\
    (This)->lpVtbl -> Release(This)


#define _ReferencesEvents_GetTypeInfoCount(This,pctinfo)	\
    (This)->lpVtbl -> GetTypeInfoCount(This,pctinfo)

#define _ReferencesEvents_GetTypeInfo(This,iTInfo,lcid,ppTInfo)	\
    (This)->lpVtbl -> GetTypeInfo(This,iTInfo,lcid,ppTInfo)

#define _ReferencesEvents_GetIDsOfNames(This,riid,rgszNames,cNames,lcid,rgDispId)	\
    (This)->lpVtbl -> GetIDsOfNames(This,riid,rgszNames,cNames,lcid,rgDispId)

#define _ReferencesEvents_Invoke(This,dispIdMember,riid,lcid,wFlags,pDispParams,pVarResult,pExcepInfo,puArgErr)	\
    (This)->lpVtbl -> Invoke(This,dispIdMember,riid,lcid,wFlags,pDispParams,pVarResult,pExcepInfo,puArgErr)


#endif /* COBJMACROS */


#endif 	/* C style interface */




#endif 	/* ___ReferencesEvents_INTERFACE_DEFINED__ */


#ifndef ___dispReferencesEvents_DISPINTERFACE_DEFINED__
#define ___dispReferencesEvents_DISPINTERFACE_DEFINED__

/* dispinterface _dispReferencesEvents */
/* [uuid] */ 


EXTERN_C const IID DIID__dispReferencesEvents;

#if defined(__cplusplus) && !defined(CINTERFACE)

    MIDL_INTERFACE("287EB27C-0F8B-4d2d-8E82-A9CA50B6766E")
    _dispReferencesEvents : public IDispatch
    {
    };
    
#else 	/* C style interface */

    typedef struct _dispReferencesEventsVtbl
    {
        BEGIN_INTERFACE
        
        HRESULT ( STDMETHODCALLTYPE *QueryInterface )( 
            _dispReferencesEvents * This,
            /* [in] */ REFIID riid,
            /* [iid_is][out] */ void **ppvObject);
        
        ULONG ( STDMETHODCALLTYPE *AddRef )( 
            _dispReferencesEvents * This);
        
        ULONG ( STDMETHODCALLTYPE *Release )( 
            _dispReferencesEvents * This);
        
        HRESULT ( STDMETHODCALLTYPE *GetTypeInfoCount )( 
            _dispReferencesEvents * This,
            /* [out] */ UINT *pctinfo);
        
        HRESULT ( STDMETHODCALLTYPE *GetTypeInfo )( 
            _dispReferencesEvents * This,
            /* [in] */ UINT iTInfo,
            /* [in] */ LCID lcid,
            /* [out] */ ITypeInfo **ppTInfo);
        
        HRESULT ( STDMETHODCALLTYPE *GetIDsOfNames )( 
            _dispReferencesEvents * This,
            /* [in] */ REFIID riid,
            /* [size_is][in] */ LPOLESTR *rgszNames,
            /* [in] */ UINT cNames,
            /* [in] */ LCID lcid,
            /* [size_is][out] */ DISPID *rgDispId);
        
        /* [local] */ HRESULT ( STDMETHODCALLTYPE *Invoke )( 
            _dispReferencesEvents * This,
            /* [in] */ DISPID dispIdMember,
            /* [in] */ REFIID riid,
            /* [in] */ LCID lcid,
            /* [in] */ WORD wFlags,
            /* [out][in] */ DISPPARAMS *pDispParams,
            /* [out] */ VARIANT *pVarResult,
            /* [out] */ EXCEPINFO *pExcepInfo,
            /* [out] */ UINT *puArgErr);
        
        END_INTERFACE
    } _dispReferencesEventsVtbl;

    interface _dispReferencesEvents
    {
        CONST_VTBL struct _dispReferencesEventsVtbl *lpVtbl;
    };

    

#ifdef COBJMACROS


#define _dispReferencesEvents_QueryInterface(This,riid,ppvObject)	\
    (This)->lpVtbl -> QueryInterface(This,riid,ppvObject)

#define _dispReferencesEvents_AddRef(This)	\
    (This)->lpVtbl -> AddRef(This)

#define _dispReferencesEvents_Release(This)	\
    (This)->lpVtbl -> Release(This)


#define _dispReferencesEvents_GetTypeInfoCount(This,pctinfo)	\
    (This)->lpVtbl -> GetTypeInfoCount(This,pctinfo)

#define _dispReferencesEvents_GetTypeInfo(This,iTInfo,lcid,ppTInfo)	\
    (This)->lpVtbl -> GetTypeInfo(This,iTInfo,lcid,ppTInfo)

#define _dispReferencesEvents_GetIDsOfNames(This,riid,rgszNames,cNames,lcid,rgDispId)	\
    (This)->lpVtbl -> GetIDsOfNames(This,riid,rgszNames,cNames,lcid,rgDispId)

#define _dispReferencesEvents_Invoke(This,dispIdMember,riid,lcid,wFlags,pDispParams,pVarResult,pExcepInfo,puArgErr)	\
    (This)->lpVtbl -> Invoke(This,dispIdMember,riid,lcid,wFlags,pDispParams,pVarResult,pExcepInfo,puArgErr)

#endif /* COBJMACROS */


#endif 	/* C style interface */


#endif 	/* ___dispReferencesEvents_DISPINTERFACE_DEFINED__ */


EXTERN_C const CLSID CLSID_ReferencesEvents;

#ifdef __cplusplus

class DECLSPEC_UUID("1CDB29FE-33B7-4392-9742-D9415D3408FE")
ReferencesEvents;
#endif

#ifndef __BuildManager_INTERFACE_DEFINED__
#define __BuildManager_INTERFACE_DEFINED__

/* interface BuildManager */
/* [uuid][object][oleautomation][dual] */ 


EXTERN_C const IID IID_BuildManager;

#if defined(__cplusplus) && !defined(CINTERFACE)
    
    MIDL_INTERFACE("C711E2B7-3C58-4C37-9359-705208A890AE")
    BuildManager : public IDispatch
    {
    public:
        virtual /* [helpstring][propget][id] */ HRESULT STDMETHODCALLTYPE get_DTE( 
            /* [retval][out] */ /* external definition not present */ DTE **ppDTE) = 0;
        
        virtual /* [helpstring][propget][id] */ HRESULT STDMETHODCALLTYPE get_Parent( 
            /* [retval][out] */ IDispatch **ppdispParent) = 0;
        
        virtual /* [helpstring][propget][id] */ HRESULT STDMETHODCALLTYPE get_ContainingProject( 
            /* [retval][out] */ /* external definition not present */ Project **ppProject) = 0;
        
        virtual /* [helpstring][propget][id] */ HRESULT STDMETHODCALLTYPE get_DesignTimeOutputMonikers( 
            /* [retval][out] */ VARIANT *pvarMonikers) = 0;
        
        virtual /* [helpstring][id] */ HRESULT STDMETHODCALLTYPE BuildDesignTimeOutput( 
            /* [in] */ BSTR bstrOutputMoniker,
            /* [retval][out] */ BSTR *pbstrXMLFormat) = 0;
        
    };
    
#else 	/* C style interface */

    typedef struct BuildManagerVtbl
    {
        BEGIN_INTERFACE
        
        HRESULT ( STDMETHODCALLTYPE *QueryInterface )( 
            BuildManager * This,
            /* [in] */ REFIID riid,
            /* [iid_is][out] */ void **ppvObject);
        
        ULONG ( STDMETHODCALLTYPE *AddRef )( 
            BuildManager * This);
        
        ULONG ( STDMETHODCALLTYPE *Release )( 
            BuildManager * This);
        
        HRESULT ( STDMETHODCALLTYPE *GetTypeInfoCount )( 
            BuildManager * This,
            /* [out] */ UINT *pctinfo);
        
        HRESULT ( STDMETHODCALLTYPE *GetTypeInfo )( 
            BuildManager * This,
            /* [in] */ UINT iTInfo,
            /* [in] */ LCID lcid,
            /* [out] */ ITypeInfo **ppTInfo);
        
        HRESULT ( STDMETHODCALLTYPE *GetIDsOfNames )( 
            BuildManager * This,
            /* [in] */ REFIID riid,
            /* [size_is][in] */ LPOLESTR *rgszNames,
            /* [in] */ UINT cNames,
            /* [in] */ LCID lcid,
            /* [size_is][out] */ DISPID *rgDispId);
        
        /* [local] */ HRESULT ( STDMETHODCALLTYPE *Invoke )( 
            BuildManager * This,
            /* [in] */ DISPID dispIdMember,
            /* [in] */ REFIID riid,
            /* [in] */ LCID lcid,
            /* [in] */ WORD wFlags,
            /* [out][in] */ DISPPARAMS *pDispParams,
            /* [out] */ VARIANT *pVarResult,
            /* [out] */ EXCEPINFO *pExcepInfo,
            /* [out] */ UINT *puArgErr);
        
        /* [helpstring][propget][id] */ HRESULT ( STDMETHODCALLTYPE *get_DTE )( 
            BuildManager * This,
            /* [retval][out] */ /* external definition not present */ DTE **ppDTE);
        
        /* [helpstring][propget][id] */ HRESULT ( STDMETHODCALLTYPE *get_Parent )( 
            BuildManager * This,
            /* [retval][out] */ IDispatch **ppdispParent);
        
        /* [helpstring][propget][id] */ HRESULT ( STDMETHODCALLTYPE *get_ContainingProject )( 
            BuildManager * This,
            /* [retval][out] */ /* external definition not present */ Project **ppProject);
        
        /* [helpstring][propget][id] */ HRESULT ( STDMETHODCALLTYPE *get_DesignTimeOutputMonikers )( 
            BuildManager * This,
            /* [retval][out] */ VARIANT *pvarMonikers);
        
        /* [helpstring][id] */ HRESULT ( STDMETHODCALLTYPE *BuildDesignTimeOutput )( 
            BuildManager * This,
            /* [in] */ BSTR bstrOutputMoniker,
            /* [retval][out] */ BSTR *pbstrXMLFormat);
        
        END_INTERFACE
    } BuildManagerVtbl;

    interface BuildManager
    {
        CONST_VTBL struct BuildManagerVtbl *lpVtbl;
    };

    

#ifdef COBJMACROS


#define BuildManager_QueryInterface(This,riid,ppvObject)	\
    (This)->lpVtbl -> QueryInterface(This,riid,ppvObject)

#define BuildManager_AddRef(This)	\
    (This)->lpVtbl -> AddRef(This)

#define BuildManager_Release(This)	\
    (This)->lpVtbl -> Release(This)


#define BuildManager_GetTypeInfoCount(This,pctinfo)	\
    (This)->lpVtbl -> GetTypeInfoCount(This,pctinfo)

#define BuildManager_GetTypeInfo(This,iTInfo,lcid,ppTInfo)	\
    (This)->lpVtbl -> GetTypeInfo(This,iTInfo,lcid,ppTInfo)

#define BuildManager_GetIDsOfNames(This,riid,rgszNames,cNames,lcid,rgDispId)	\
    (This)->lpVtbl -> GetIDsOfNames(This,riid,rgszNames,cNames,lcid,rgDispId)

#define BuildManager_Invoke(This,dispIdMember,riid,lcid,wFlags,pDispParams,pVarResult,pExcepInfo,puArgErr)	\
    (This)->lpVtbl -> Invoke(This,dispIdMember,riid,lcid,wFlags,pDispParams,pVarResult,pExcepInfo,puArgErr)


#define BuildManager_get_DTE(This,ppDTE)	\
    (This)->lpVtbl -> get_DTE(This,ppDTE)

#define BuildManager_get_Parent(This,ppdispParent)	\
    (This)->lpVtbl -> get_Parent(This,ppdispParent)

#define BuildManager_get_ContainingProject(This,ppProject)	\
    (This)->lpVtbl -> get_ContainingProject(This,ppProject)

#define BuildManager_get_DesignTimeOutputMonikers(This,pvarMonikers)	\
    (This)->lpVtbl -> get_DesignTimeOutputMonikers(This,pvarMonikers)

#define BuildManager_BuildDesignTimeOutput(This,bstrOutputMoniker,pbstrXMLFormat)	\
    (This)->lpVtbl -> BuildDesignTimeOutput(This,bstrOutputMoniker,pbstrXMLFormat)

#endif /* COBJMACROS */


#endif 	/* C style interface */



/* [helpstring][propget][id] */ HRESULT STDMETHODCALLTYPE BuildManager_get_DTE_Proxy( 
    BuildManager * This,
    /* [retval][out] */ /* external definition not present */ DTE **ppDTE);


void __RPC_STUB BuildManager_get_DTE_Stub(
    IRpcStubBuffer *This,
    IRpcChannelBuffer *_pRpcChannelBuffer,
    PRPC_MESSAGE _pRpcMessage,
    DWORD *_pdwStubPhase);


/* [helpstring][propget][id] */ HRESULT STDMETHODCALLTYPE BuildManager_get_Parent_Proxy( 
    BuildManager * This,
    /* [retval][out] */ IDispatch **ppdispParent);


void __RPC_STUB BuildManager_get_Parent_Stub(
    IRpcStubBuffer *This,
    IRpcChannelBuffer *_pRpcChannelBuffer,
    PRPC_MESSAGE _pRpcMessage,
    DWORD *_pdwStubPhase);


/* [helpstring][propget][id] */ HRESULT STDMETHODCALLTYPE BuildManager_get_ContainingProject_Proxy( 
    BuildManager * This,
    /* [retval][out] */ /* external definition not present */ Project **ppProject);


void __RPC_STUB BuildManager_get_ContainingProject_Stub(
    IRpcStubBuffer *This,
    IRpcChannelBuffer *_pRpcChannelBuffer,
    PRPC_MESSAGE _pRpcMessage,
    DWORD *_pdwStubPhase);


/* [helpstring][propget][id] */ HRESULT STDMETHODCALLTYPE BuildManager_get_DesignTimeOutputMonikers_Proxy( 
    BuildManager * This,
    /* [retval][out] */ VARIANT *pvarMonikers);


void __RPC_STUB BuildManager_get_DesignTimeOutputMonikers_Stub(
    IRpcStubBuffer *This,
    IRpcChannelBuffer *_pRpcChannelBuffer,
    PRPC_MESSAGE _pRpcMessage,
    DWORD *_pdwStubPhase);


/* [helpstring][id] */ HRESULT STDMETHODCALLTYPE BuildManager_BuildDesignTimeOutput_Proxy( 
    BuildManager * This,
    /* [in] */ BSTR bstrOutputMoniker,
    /* [retval][out] */ BSTR *pbstrXMLFormat);


void __RPC_STUB BuildManager_BuildDesignTimeOutput_Stub(
    IRpcStubBuffer *This,
    IRpcChannelBuffer *_pRpcChannelBuffer,
    PRPC_MESSAGE _pRpcMessage,
    DWORD *_pdwStubPhase);



#endif 	/* __BuildManager_INTERFACE_DEFINED__ */


#ifndef ___BuildManagerEvents_INTERFACE_DEFINED__
#define ___BuildManagerEvents_INTERFACE_DEFINED__

/* interface _BuildManagerEvents */
/* [object][dual][uuid] */ 


EXTERN_C const IID IID__BuildManagerEvents;

#if defined(__cplusplus) && !defined(CINTERFACE)
    
    MIDL_INTERFACE("5F4AAE42-BC94-401d-9213-B8A8B9E553DE")
    _BuildManagerEvents : public IDispatch
    {
    public:
    };
    
#else 	/* C style interface */

    typedef struct _BuildManagerEventsVtbl
    {
        BEGIN_INTERFACE
        
        HRESULT ( STDMETHODCALLTYPE *QueryInterface )( 
            _BuildManagerEvents * This,
            /* [in] */ REFIID riid,
            /* [iid_is][out] */ void **ppvObject);
        
        ULONG ( STDMETHODCALLTYPE *AddRef )( 
            _BuildManagerEvents * This);
        
        ULONG ( STDMETHODCALLTYPE *Release )( 
            _BuildManagerEvents * This);
        
        HRESULT ( STDMETHODCALLTYPE *GetTypeInfoCount )( 
            _BuildManagerEvents * This,
            /* [out] */ UINT *pctinfo);
        
        HRESULT ( STDMETHODCALLTYPE *GetTypeInfo )( 
            _BuildManagerEvents * This,
            /* [in] */ UINT iTInfo,
            /* [in] */ LCID lcid,
            /* [out] */ ITypeInfo **ppTInfo);
        
        HRESULT ( STDMETHODCALLTYPE *GetIDsOfNames )( 
            _BuildManagerEvents * This,
            /* [in] */ REFIID riid,
            /* [size_is][in] */ LPOLESTR *rgszNames,
            /* [in] */ UINT cNames,
            /* [in] */ LCID lcid,
            /* [size_is][out] */ DISPID *rgDispId);
        
        /* [local] */ HRESULT ( STDMETHODCALLTYPE *Invoke )( 
            _BuildManagerEvents * This,
            /* [in] */ DISPID dispIdMember,
            /* [in] */ REFIID riid,
            /* [in] */ LCID lcid,
            /* [in] */ WORD wFlags,
            /* [out][in] */ DISPPARAMS *pDispParams,
            /* [out] */ VARIANT *pVarResult,
            /* [out] */ EXCEPINFO *pExcepInfo,
            /* [out] */ UINT *puArgErr);
        
        END_INTERFACE
    } _BuildManagerEventsVtbl;

    interface _BuildManagerEvents
    {
        CONST_VTBL struct _BuildManagerEventsVtbl *lpVtbl;
    };

    

#ifdef COBJMACROS


#define _BuildManagerEvents_QueryInterface(This,riid,ppvObject)	\
    (This)->lpVtbl -> QueryInterface(This,riid,ppvObject)

#define _BuildManagerEvents_AddRef(This)	\
    (This)->lpVtbl -> AddRef(This)

#define _BuildManagerEvents_Release(This)	\
    (This)->lpVtbl -> Release(This)


#define _BuildManagerEvents_GetTypeInfoCount(This,pctinfo)	\
    (This)->lpVtbl -> GetTypeInfoCount(This,pctinfo)

#define _BuildManagerEvents_GetTypeInfo(This,iTInfo,lcid,ppTInfo)	\
    (This)->lpVtbl -> GetTypeInfo(This,iTInfo,lcid,ppTInfo)

#define _BuildManagerEvents_GetIDsOfNames(This,riid,rgszNames,cNames,lcid,rgDispId)	\
    (This)->lpVtbl -> GetIDsOfNames(This,riid,rgszNames,cNames,lcid,rgDispId)

#define _BuildManagerEvents_Invoke(This,dispIdMember,riid,lcid,wFlags,pDispParams,pVarResult,pExcepInfo,puArgErr)	\
    (This)->lpVtbl -> Invoke(This,dispIdMember,riid,lcid,wFlags,pDispParams,pVarResult,pExcepInfo,puArgErr)


#endif /* COBJMACROS */


#endif 	/* C style interface */




#endif 	/* ___BuildManagerEvents_INTERFACE_DEFINED__ */


#ifndef ___dispBuildManagerEvents_DISPINTERFACE_DEFINED__
#define ___dispBuildManagerEvents_DISPINTERFACE_DEFINED__

/* dispinterface _dispBuildManagerEvents */
/* [uuid] */ 


EXTERN_C const IID DIID__dispBuildManagerEvents;

#if defined(__cplusplus) && !defined(CINTERFACE)

    MIDL_INTERFACE("828914F7-1D81-4f5c-83CE-37819D7EE759")
    _dispBuildManagerEvents : public IDispatch
    {
    };
    
#else 	/* C style interface */

    typedef struct _dispBuildManagerEventsVtbl
    {
        BEGIN_INTERFACE
        
        HRESULT ( STDMETHODCALLTYPE *QueryInterface )( 
            _dispBuildManagerEvents * This,
            /* [in] */ REFIID riid,
            /* [iid_is][out] */ void **ppvObject);
        
        ULONG ( STDMETHODCALLTYPE *AddRef )( 
            _dispBuildManagerEvents * This);
        
        ULONG ( STDMETHODCALLTYPE *Release )( 
            _dispBuildManagerEvents * This);
        
        HRESULT ( STDMETHODCALLTYPE *GetTypeInfoCount )( 
            _dispBuildManagerEvents * This,
            /* [out] */ UINT *pctinfo);
        
        HRESULT ( STDMETHODCALLTYPE *GetTypeInfo )( 
            _dispBuildManagerEvents * This,
            /* [in] */ UINT iTInfo,
            /* [in] */ LCID lcid,
            /* [out] */ ITypeInfo **ppTInfo);
        
        HRESULT ( STDMETHODCALLTYPE *GetIDsOfNames )( 
            _dispBuildManagerEvents * This,
            /* [in] */ REFIID riid,
            /* [size_is][in] */ LPOLESTR *rgszNames,
            /* [in] */ UINT cNames,
            /* [in] */ LCID lcid,
            /* [size_is][out] */ DISPID *rgDispId);
        
        /* [local] */ HRESULT ( STDMETHODCALLTYPE *Invoke )( 
            _dispBuildManagerEvents * This,
            /* [in] */ DISPID dispIdMember,
            /* [in] */ REFIID riid,
            /* [in] */ LCID lcid,
            /* [in] */ WORD wFlags,
            /* [out][in] */ DISPPARAMS *pDispParams,
            /* [out] */ VARIANT *pVarResult,
            /* [out] */ EXCEPINFO *pExcepInfo,
            /* [out] */ UINT *puArgErr);
        
        END_INTERFACE
    } _dispBuildManagerEventsVtbl;

    interface _dispBuildManagerEvents
    {
        CONST_VTBL struct _dispBuildManagerEventsVtbl *lpVtbl;
    };

    

#ifdef COBJMACROS


#define _dispBuildManagerEvents_QueryInterface(This,riid,ppvObject)	\
    (This)->lpVtbl -> QueryInterface(This,riid,ppvObject)

#define _dispBuildManagerEvents_AddRef(This)	\
    (This)->lpVtbl -> AddRef(This)

#define _dispBuildManagerEvents_Release(This)	\
    (This)->lpVtbl -> Release(This)


#define _dispBuildManagerEvents_GetTypeInfoCount(This,pctinfo)	\
    (This)->lpVtbl -> GetTypeInfoCount(This,pctinfo)

#define _dispBuildManagerEvents_GetTypeInfo(This,iTInfo,lcid,ppTInfo)	\
    (This)->lpVtbl -> GetTypeInfo(This,iTInfo,lcid,ppTInfo)

#define _dispBuildManagerEvents_GetIDsOfNames(This,riid,rgszNames,cNames,lcid,rgDispId)	\
    (This)->lpVtbl -> GetIDsOfNames(This,riid,rgszNames,cNames,lcid,rgDispId)

#define _dispBuildManagerEvents_Invoke(This,dispIdMember,riid,lcid,wFlags,pDispParams,pVarResult,pExcepInfo,puArgErr)	\
    (This)->lpVtbl -> Invoke(This,dispIdMember,riid,lcid,wFlags,pDispParams,pVarResult,pExcepInfo,puArgErr)

#endif /* COBJMACROS */


#endif 	/* C style interface */


#endif 	/* ___dispBuildManagerEvents_DISPINTERFACE_DEFINED__ */


EXTERN_C const CLSID CLSID_BuildManagerEvents;

#ifdef __cplusplus

class DECLSPEC_UUID("66923B02-677B-4920-A319-F8925A0BA8A8")
BuildManagerEvents;
#endif

#ifndef __Imports_INTERFACE_DEFINED__
#define __Imports_INTERFACE_DEFINED__

/* interface Imports */
/* [uuid][object][oleautomation][dual] */ 


EXTERN_C const IID IID_Imports;

#if defined(__cplusplus) && !defined(CINTERFACE)
    
    MIDL_INTERFACE("642789F9-210D-4574-96FD-5A653451E216")
    Imports : public IDispatch
    {
    public:
        virtual /* [helpstring][propget][id] */ HRESULT STDMETHODCALLTYPE get_DTE( 
            /* [retval][out] */ /* external definition not present */ DTE **ppDTE) = 0;
        
        virtual /* [helpstring][propget][id] */ HRESULT STDMETHODCALLTYPE get_Parent( 
            /* [retval][out] */ IDispatch **ppdispParent) = 0;
        
        virtual /* [helpstring][propget][id] */ HRESULT STDMETHODCALLTYPE get_ContainingProject( 
            /* [retval][out] */ /* external definition not present */ Project **ppProject) = 0;
        
        virtual /* [helpstring][propget][id] */ HRESULT STDMETHODCALLTYPE get_Count( 
            /* [retval][out] */ long *plCount) = 0;
        
        virtual /* [helpstring][id] */ HRESULT STDMETHODCALLTYPE Item( 
            /* [in] */ long lIndex,
            /* [retval][out] */ BSTR *pbstrImport) = 0;
        
        virtual /* [helpstring][id] */ HRESULT STDMETHODCALLTYPE Add( 
            /* [in] */ BSTR bstrImport) = 0;
        
        virtual /* [helpstring][id] */ HRESULT STDMETHODCALLTYPE Remove( 
            /* [in] */ VARIANT index) = 0;
        
        virtual /* [id][restricted] */ HRESULT STDMETHODCALLTYPE _NewEnum( 
            /* [retval][out] */ IUnknown **ppiuReturn) = 0;
        
    };
    
#else 	/* C style interface */

    typedef struct ImportsVtbl
    {
        BEGIN_INTERFACE
        
        HRESULT ( STDMETHODCALLTYPE *QueryInterface )( 
            Imports * This,
            /* [in] */ REFIID riid,
            /* [iid_is][out] */ void **ppvObject);
        
        ULONG ( STDMETHODCALLTYPE *AddRef )( 
            Imports * This);
        
        ULONG ( STDMETHODCALLTYPE *Release )( 
            Imports * This);
        
        HRESULT ( STDMETHODCALLTYPE *GetTypeInfoCount )( 
            Imports * This,
            /* [out] */ UINT *pctinfo);
        
        HRESULT ( STDMETHODCALLTYPE *GetTypeInfo )( 
            Imports * This,
            /* [in] */ UINT iTInfo,
            /* [in] */ LCID lcid,
            /* [out] */ ITypeInfo **ppTInfo);
        
        HRESULT ( STDMETHODCALLTYPE *GetIDsOfNames )( 
            Imports * This,
            /* [in] */ REFIID riid,
            /* [size_is][in] */ LPOLESTR *rgszNames,
            /* [in] */ UINT cNames,
            /* [in] */ LCID lcid,
            /* [size_is][out] */ DISPID *rgDispId);
        
        /* [local] */ HRESULT ( STDMETHODCALLTYPE *Invoke )( 
            Imports * This,
            /* [in] */ DISPID dispIdMember,
            /* [in] */ REFIID riid,
            /* [in] */ LCID lcid,
            /* [in] */ WORD wFlags,
            /* [out][in] */ DISPPARAMS *pDispParams,
            /* [out] */ VARIANT *pVarResult,
            /* [out] */ EXCEPINFO *pExcepInfo,
            /* [out] */ UINT *puArgErr);
        
        /* [helpstring][propget][id] */ HRESULT ( STDMETHODCALLTYPE *get_DTE )( 
            Imports * This,
            /* [retval][out] */ /* external definition not present */ DTE **ppDTE);
        
        /* [helpstring][propget][id] */ HRESULT ( STDMETHODCALLTYPE *get_Parent )( 
            Imports * This,
            /* [retval][out] */ IDispatch **ppdispParent);
        
        /* [helpstring][propget][id] */ HRESULT ( STDMETHODCALLTYPE *get_ContainingProject )( 
            Imports * This,
            /* [retval][out] */ /* external definition not present */ Project **ppProject);
        
        /* [helpstring][propget][id] */ HRESULT ( STDMETHODCALLTYPE *get_Count )( 
            Imports * This,
            /* [retval][out] */ long *plCount);
        
        /* [helpstring][id] */ HRESULT ( STDMETHODCALLTYPE *Item )( 
            Imports * This,
            /* [in] */ long lIndex,
            /* [retval][out] */ BSTR *pbstrImport);
        
        /* [helpstring][id] */ HRESULT ( STDMETHODCALLTYPE *Add )( 
            Imports * This,
            /* [in] */ BSTR bstrImport);
        
        /* [helpstring][id] */ HRESULT ( STDMETHODCALLTYPE *Remove )( 
            Imports * This,
            /* [in] */ VARIANT index);
        
        /* [id][restricted] */ HRESULT ( STDMETHODCALLTYPE *_NewEnum )( 
            Imports * This,
            /* [retval][out] */ IUnknown **ppiuReturn);
        
        END_INTERFACE
    } ImportsVtbl;

    interface Imports
    {
        CONST_VTBL struct ImportsVtbl *lpVtbl;
    };

    

#ifdef COBJMACROS


#define Imports_QueryInterface(This,riid,ppvObject)	\
    (This)->lpVtbl -> QueryInterface(This,riid,ppvObject)

#define Imports_AddRef(This)	\
    (This)->lpVtbl -> AddRef(This)

#define Imports_Release(This)	\
    (This)->lpVtbl -> Release(This)


#define Imports_GetTypeInfoCount(This,pctinfo)	\
    (This)->lpVtbl -> GetTypeInfoCount(This,pctinfo)

#define Imports_GetTypeInfo(This,iTInfo,lcid,ppTInfo)	\
    (This)->lpVtbl -> GetTypeInfo(This,iTInfo,lcid,ppTInfo)

#define Imports_GetIDsOfNames(This,riid,rgszNames,cNames,lcid,rgDispId)	\
    (This)->lpVtbl -> GetIDsOfNames(This,riid,rgszNames,cNames,lcid,rgDispId)

#define Imports_Invoke(This,dispIdMember,riid,lcid,wFlags,pDispParams,pVarResult,pExcepInfo,puArgErr)	\
    (This)->lpVtbl -> Invoke(This,dispIdMember,riid,lcid,wFlags,pDispParams,pVarResult,pExcepInfo,puArgErr)


#define Imports_get_DTE(This,ppDTE)	\
    (This)->lpVtbl -> get_DTE(This,ppDTE)

#define Imports_get_Parent(This,ppdispParent)	\
    (This)->lpVtbl -> get_Parent(This,ppdispParent)

#define Imports_get_ContainingProject(This,ppProject)	\
    (This)->lpVtbl -> get_ContainingProject(This,ppProject)

#define Imports_get_Count(This,plCount)	\
    (This)->lpVtbl -> get_Count(This,plCount)

#define Imports_Item(This,lIndex,pbstrImport)	\
    (This)->lpVtbl -> Item(This,lIndex,pbstrImport)

#define Imports_Add(This,bstrImport)	\
    (This)->lpVtbl -> Add(This,bstrImport)

#define Imports_Remove(This,index)	\
    (This)->lpVtbl -> Remove(This,index)

#define Imports__NewEnum(This,ppiuReturn)	\
    (This)->lpVtbl -> _NewEnum(This,ppiuReturn)

#endif /* COBJMACROS */


#endif 	/* C style interface */



/* [helpstring][propget][id] */ HRESULT STDMETHODCALLTYPE Imports_get_DTE_Proxy( 
    Imports * This,
    /* [retval][out] */ /* external definition not present */ DTE **ppDTE);


void __RPC_STUB Imports_get_DTE_Stub(
    IRpcStubBuffer *This,
    IRpcChannelBuffer *_pRpcChannelBuffer,
    PRPC_MESSAGE _pRpcMessage,
    DWORD *_pdwStubPhase);


/* [helpstring][propget][id] */ HRESULT STDMETHODCALLTYPE Imports_get_Parent_Proxy( 
    Imports * This,
    /* [retval][out] */ IDispatch **ppdispParent);


void __RPC_STUB Imports_get_Parent_Stub(
    IRpcStubBuffer *This,
    IRpcChannelBuffer *_pRpcChannelBuffer,
    PRPC_MESSAGE _pRpcMessage,
    DWORD *_pdwStubPhase);


/* [helpstring][propget][id] */ HRESULT STDMETHODCALLTYPE Imports_get_ContainingProject_Proxy( 
    Imports * This,
    /* [retval][out] */ /* external definition not present */ Project **ppProject);


void __RPC_STUB Imports_get_ContainingProject_Stub(
    IRpcStubBuffer *This,
    IRpcChannelBuffer *_pRpcChannelBuffer,
    PRPC_MESSAGE _pRpcMessage,
    DWORD *_pdwStubPhase);


/* [helpstring][propget][id] */ HRESULT STDMETHODCALLTYPE Imports_get_Count_Proxy( 
    Imports * This,
    /* [retval][out] */ long *plCount);


void __RPC_STUB Imports_get_Count_Stub(
    IRpcStubBuffer *This,
    IRpcChannelBuffer *_pRpcChannelBuffer,
    PRPC_MESSAGE _pRpcMessage,
    DWORD *_pdwStubPhase);


/* [helpstring][id] */ HRESULT STDMETHODCALLTYPE Imports_Item_Proxy( 
    Imports * This,
    /* [in] */ long lIndex,
    /* [retval][out] */ BSTR *pbstrImport);


void __RPC_STUB Imports_Item_Stub(
    IRpcStubBuffer *This,
    IRpcChannelBuffer *_pRpcChannelBuffer,
    PRPC_MESSAGE _pRpcMessage,
    DWORD *_pdwStubPhase);


/* [helpstring][id] */ HRESULT STDMETHODCALLTYPE Imports_Add_Proxy( 
    Imports * This,
    /* [in] */ BSTR bstrImport);


void __RPC_STUB Imports_Add_Stub(
    IRpcStubBuffer *This,
    IRpcChannelBuffer *_pRpcChannelBuffer,
    PRPC_MESSAGE _pRpcMessage,
    DWORD *_pdwStubPhase);


/* [helpstring][id] */ HRESULT STDMETHODCALLTYPE Imports_Remove_Proxy( 
    Imports * This,
    /* [in] */ VARIANT index);


void __RPC_STUB Imports_Remove_Stub(
    IRpcStubBuffer *This,
    IRpcChannelBuffer *_pRpcChannelBuffer,
    PRPC_MESSAGE _pRpcMessage,
    DWORD *_pdwStubPhase);


/* [id][restricted] */ HRESULT STDMETHODCALLTYPE Imports__NewEnum_Proxy( 
    Imports * This,
    /* [retval][out] */ IUnknown **ppiuReturn);


void __RPC_STUB Imports__NewEnum_Stub(
    IRpcStubBuffer *This,
    IRpcChannelBuffer *_pRpcChannelBuffer,
    PRPC_MESSAGE _pRpcMessage,
    DWORD *_pdwStubPhase);



#endif 	/* __Imports_INTERFACE_DEFINED__ */


#ifndef ___ImportsEvents_INTERFACE_DEFINED__
#define ___ImportsEvents_INTERFACE_DEFINED__

/* interface _ImportsEvents */
/* [object][dual][uuid] */ 


EXTERN_C const IID IID__ImportsEvents;

#if defined(__cplusplus) && !defined(CINTERFACE)
    
    MIDL_INTERFACE("037AD859-7A75-4cf3-8A38-83D6E045FEE3")
    _ImportsEvents : public IDispatch
    {
    public:
    };
    
#else 	/* C style interface */

    typedef struct _ImportsEventsVtbl
    {
        BEGIN_INTERFACE
        
        HRESULT ( STDMETHODCALLTYPE *QueryInterface )( 
            _ImportsEvents * This,
            /* [in] */ REFIID riid,
            /* [iid_is][out] */ void **ppvObject);
        
        ULONG ( STDMETHODCALLTYPE *AddRef )( 
            _ImportsEvents * This);
        
        ULONG ( STDMETHODCALLTYPE *Release )( 
            _ImportsEvents * This);
        
        HRESULT ( STDMETHODCALLTYPE *GetTypeInfoCount )( 
            _ImportsEvents * This,
            /* [out] */ UINT *pctinfo);
        
        HRESULT ( STDMETHODCALLTYPE *GetTypeInfo )( 
            _ImportsEvents * This,
            /* [in] */ UINT iTInfo,
            /* [in] */ LCID lcid,
            /* [out] */ ITypeInfo **ppTInfo);
        
        HRESULT ( STDMETHODCALLTYPE *GetIDsOfNames )( 
            _ImportsEvents * This,
            /* [in] */ REFIID riid,
            /* [size_is][in] */ LPOLESTR *rgszNames,
            /* [in] */ UINT cNames,
            /* [in] */ LCID lcid,
            /* [size_is][out] */ DISPID *rgDispId);
        
        /* [local] */ HRESULT ( STDMETHODCALLTYPE *Invoke )( 
            _ImportsEvents * This,
            /* [in] */ DISPID dispIdMember,
            /* [in] */ REFIID riid,
            /* [in] */ LCID lcid,
            /* [in] */ WORD wFlags,
            /* [out][in] */ DISPPARAMS *pDispParams,
            /* [out] */ VARIANT *pVarResult,
            /* [out] */ EXCEPINFO *pExcepInfo,
            /* [out] */ UINT *puArgErr);
        
        END_INTERFACE
    } _ImportsEventsVtbl;

    interface _ImportsEvents
    {
        CONST_VTBL struct _ImportsEventsVtbl *lpVtbl;
    };

    

#ifdef COBJMACROS


#define _ImportsEvents_QueryInterface(This,riid,ppvObject)	\
    (This)->lpVtbl -> QueryInterface(This,riid,ppvObject)

#define _ImportsEvents_AddRef(This)	\
    (This)->lpVtbl -> AddRef(This)

#define _ImportsEvents_Release(This)	\
    (This)->lpVtbl -> Release(This)


#define _ImportsEvents_GetTypeInfoCount(This,pctinfo)	\
    (This)->lpVtbl -> GetTypeInfoCount(This,pctinfo)

#define _ImportsEvents_GetTypeInfo(This,iTInfo,lcid,ppTInfo)	\
    (This)->lpVtbl -> GetTypeInfo(This,iTInfo,lcid,ppTInfo)

#define _ImportsEvents_GetIDsOfNames(This,riid,rgszNames,cNames,lcid,rgDispId)	\
    (This)->lpVtbl -> GetIDsOfNames(This,riid,rgszNames,cNames,lcid,rgDispId)

#define _ImportsEvents_Invoke(This,dispIdMember,riid,lcid,wFlags,pDispParams,pVarResult,pExcepInfo,puArgErr)	\
    (This)->lpVtbl -> Invoke(This,dispIdMember,riid,lcid,wFlags,pDispParams,pVarResult,pExcepInfo,puArgErr)


#endif /* COBJMACROS */


#endif 	/* C style interface */




#endif 	/* ___ImportsEvents_INTERFACE_DEFINED__ */


#ifndef ___dispImportsEvents_DISPINTERFACE_DEFINED__
#define ___dispImportsEvents_DISPINTERFACE_DEFINED__

/* dispinterface _dispImportsEvents */
/* [uuid] */ 


EXTERN_C const IID DIID__dispImportsEvents;

#if defined(__cplusplus) && !defined(CINTERFACE)

    MIDL_INTERFACE("40806CEA-ABAB-4887-A356-D8869C28A6E6")
    _dispImportsEvents : public IDispatch
    {
    };
    
#else 	/* C style interface */

    typedef struct _dispImportsEventsVtbl
    {
        BEGIN_INTERFACE
        
        HRESULT ( STDMETHODCALLTYPE *QueryInterface )( 
            _dispImportsEvents * This,
            /* [in] */ REFIID riid,
            /* [iid_is][out] */ void **ppvObject);
        
        ULONG ( STDMETHODCALLTYPE *AddRef )( 
            _dispImportsEvents * This);
        
        ULONG ( STDMETHODCALLTYPE *Release )( 
            _dispImportsEvents * This);
        
        HRESULT ( STDMETHODCALLTYPE *GetTypeInfoCount )( 
            _dispImportsEvents * This,
            /* [out] */ UINT *pctinfo);
        
        HRESULT ( STDMETHODCALLTYPE *GetTypeInfo )( 
            _dispImportsEvents * This,
            /* [in] */ UINT iTInfo,
            /* [in] */ LCID lcid,
            /* [out] */ ITypeInfo **ppTInfo);
        
        HRESULT ( STDMETHODCALLTYPE *GetIDsOfNames )( 
            _dispImportsEvents * This,
            /* [in] */ REFIID riid,
            /* [size_is][in] */ LPOLESTR *rgszNames,
            /* [in] */ UINT cNames,
            /* [in] */ LCID lcid,
            /* [size_is][out] */ DISPID *rgDispId);
        
        /* [local] */ HRESULT ( STDMETHODCALLTYPE *Invoke )( 
            _dispImportsEvents * This,
            /* [in] */ DISPID dispIdMember,
            /* [in] */ REFIID riid,
            /* [in] */ LCID lcid,
            /* [in] */ WORD wFlags,
            /* [out][in] */ DISPPARAMS *pDispParams,
            /* [out] */ VARIANT *pVarResult,
            /* [out] */ EXCEPINFO *pExcepInfo,
            /* [out] */ UINT *puArgErr);
        
        END_INTERFACE
    } _dispImportsEventsVtbl;

    interface _dispImportsEvents
    {
        CONST_VTBL struct _dispImportsEventsVtbl *lpVtbl;
    };

    

#ifdef COBJMACROS


#define _dispImportsEvents_QueryInterface(This,riid,ppvObject)	\
    (This)->lpVtbl -> QueryInterface(This,riid,ppvObject)

#define _dispImportsEvents_AddRef(This)	\
    (This)->lpVtbl -> AddRef(This)

#define _dispImportsEvents_Release(This)	\
    (This)->lpVtbl -> Release(This)


#define _dispImportsEvents_GetTypeInfoCount(This,pctinfo)	\
    (This)->lpVtbl -> GetTypeInfoCount(This,pctinfo)

#define _dispImportsEvents_GetTypeInfo(This,iTInfo,lcid,ppTInfo)	\
    (This)->lpVtbl -> GetTypeInfo(This,iTInfo,lcid,ppTInfo)

#define _dispImportsEvents_GetIDsOfNames(This,riid,rgszNames,cNames,lcid,rgDispId)	\
    (This)->lpVtbl -> GetIDsOfNames(This,riid,rgszNames,cNames,lcid,rgDispId)

#define _dispImportsEvents_Invoke(This,dispIdMember,riid,lcid,wFlags,pDispParams,pVarResult,pExcepInfo,puArgErr)	\
    (This)->lpVtbl -> Invoke(This,dispIdMember,riid,lcid,wFlags,pDispParams,pVarResult,pExcepInfo,puArgErr)

#endif /* COBJMACROS */


#endif 	/* C style interface */


#endif 	/* ___dispImportsEvents_DISPINTERFACE_DEFINED__ */


EXTERN_C const CLSID CLSID_ImportsEvents;

#ifdef __cplusplus

class DECLSPEC_UUID("AC779606-837C-444f-B8FA-A69805B59976")
ImportsEvents;
#endif

#ifndef __VSProjectEvents_INTERFACE_DEFINED__
#define __VSProjectEvents_INTERFACE_DEFINED__

/* interface VSProjectEvents */
/* [uuid][object][oleautomation][dual] */ 


EXTERN_C const IID IID_VSProjectEvents;

#if defined(__cplusplus) && !defined(CINTERFACE)
    
    MIDL_INTERFACE("F8B92546-F1A2-4066-92F6-FDF2E691A50C")
    VSProjectEvents : public IDispatch
    {
    public:
        virtual /* [helpstring][propget][id] */ HRESULT STDMETHODCALLTYPE get_ReferencesEvents( 
            /* [retval][out] */ ReferencesEvents	**ppRefsEvents) = 0;
        
        virtual /* [helpstring][propget][id] */ HRESULT STDMETHODCALLTYPE get_BuildManagerEvents( 
            /* [retval][out] */ BuildManagerEvents	**ppBuildMgrEvents) = 0;
        
        virtual /* [helpstring][propget][id] */ HRESULT STDMETHODCALLTYPE get_ImportsEvents( 
            /* [retval][out] */ ImportsEvents	**ppImportsEvents) = 0;
        
    };
    
#else 	/* C style interface */

    typedef struct VSProjectEventsVtbl
    {
        BEGIN_INTERFACE
        
        HRESULT ( STDMETHODCALLTYPE *QueryInterface )( 
            VSProjectEvents * This,
            /* [in] */ REFIID riid,
            /* [iid_is][out] */ void **ppvObject);
        
        ULONG ( STDMETHODCALLTYPE *AddRef )( 
            VSProjectEvents * This);
        
        ULONG ( STDMETHODCALLTYPE *Release )( 
            VSProjectEvents * This);
        
        HRESULT ( STDMETHODCALLTYPE *GetTypeInfoCount )( 
            VSProjectEvents * This,
            /* [out] */ UINT *pctinfo);
        
        HRESULT ( STDMETHODCALLTYPE *GetTypeInfo )( 
            VSProjectEvents * This,
            /* [in] */ UINT iTInfo,
            /* [in] */ LCID lcid,
            /* [out] */ ITypeInfo **ppTInfo);
        
        HRESULT ( STDMETHODCALLTYPE *GetIDsOfNames )( 
            VSProjectEvents * This,
            /* [in] */ REFIID riid,
            /* [size_is][in] */ LPOLESTR *rgszNames,
            /* [in] */ UINT cNames,
            /* [in] */ LCID lcid,
            /* [size_is][out] */ DISPID *rgDispId);
        
        /* [local] */ HRESULT ( STDMETHODCALLTYPE *Invoke )( 
            VSProjectEvents * This,
            /* [in] */ DISPID dispIdMember,
            /* [in] */ REFIID riid,
            /* [in] */ LCID lcid,
            /* [in] */ WORD wFlags,
            /* [out][in] */ DISPPARAMS *pDispParams,
            /* [out] */ VARIANT *pVarResult,
            /* [out] */ EXCEPINFO *pExcepInfo,
            /* [out] */ UINT *puArgErr);
        
        /* [helpstring][propget][id] */ HRESULT ( STDMETHODCALLTYPE *get_ReferencesEvents )( 
            VSProjectEvents * This,
            /* [retval][out] */ ReferencesEvents	**ppRefsEvents);
        
        /* [helpstring][propget][id] */ HRESULT ( STDMETHODCALLTYPE *get_BuildManagerEvents )( 
            VSProjectEvents * This,
            /* [retval][out] */ BuildManagerEvents	**ppBuildMgrEvents);
        
        /* [helpstring][propget][id] */ HRESULT ( STDMETHODCALLTYPE *get_ImportsEvents )( 
            VSProjectEvents * This,
            /* [retval][out] */ ImportsEvents	**ppImportsEvents);
        
        END_INTERFACE
    } VSProjectEventsVtbl;

    interface VSProjectEvents
    {
        CONST_VTBL struct VSProjectEventsVtbl *lpVtbl;
    };

    

#ifdef COBJMACROS


#define VSProjectEvents_QueryInterface(This,riid,ppvObject)	\
    (This)->lpVtbl -> QueryInterface(This,riid,ppvObject)

#define VSProjectEvents_AddRef(This)	\
    (This)->lpVtbl -> AddRef(This)

#define VSProjectEvents_Release(This)	\
    (This)->lpVtbl -> Release(This)


#define VSProjectEvents_GetTypeInfoCount(This,pctinfo)	\
    (This)->lpVtbl -> GetTypeInfoCount(This,pctinfo)

#define VSProjectEvents_GetTypeInfo(This,iTInfo,lcid,ppTInfo)	\
    (This)->lpVtbl -> GetTypeInfo(This,iTInfo,lcid,ppTInfo)

#define VSProjectEvents_GetIDsOfNames(This,riid,rgszNames,cNames,lcid,rgDispId)	\
    (This)->lpVtbl -> GetIDsOfNames(This,riid,rgszNames,cNames,lcid,rgDispId)

#define VSProjectEvents_Invoke(This,dispIdMember,riid,lcid,wFlags,pDispParams,pVarResult,pExcepInfo,puArgErr)	\
    (This)->lpVtbl -> Invoke(This,dispIdMember,riid,lcid,wFlags,pDispParams,pVarResult,pExcepInfo,puArgErr)


#define VSProjectEvents_get_ReferencesEvents(This,ppRefsEvents)	\
    (This)->lpVtbl -> get_ReferencesEvents(This,ppRefsEvents)

#define VSProjectEvents_get_BuildManagerEvents(This,ppBuildMgrEvents)	\
    (This)->lpVtbl -> get_BuildManagerEvents(This,ppBuildMgrEvents)

#define VSProjectEvents_get_ImportsEvents(This,ppImportsEvents)	\
    (This)->lpVtbl -> get_ImportsEvents(This,ppImportsEvents)

#endif /* COBJMACROS */


#endif 	/* C style interface */



/* [helpstring][propget][id] */ HRESULT STDMETHODCALLTYPE VSProjectEvents_get_ReferencesEvents_Proxy( 
    VSProjectEvents * This,
    /* [retval][out] */ ReferencesEvents	**ppRefsEvents);


void __RPC_STUB VSProjectEvents_get_ReferencesEvents_Stub(
    IRpcStubBuffer *This,
    IRpcChannelBuffer *_pRpcChannelBuffer,
    PRPC_MESSAGE _pRpcMessage,
    DWORD *_pdwStubPhase);


/* [helpstring][propget][id] */ HRESULT STDMETHODCALLTYPE VSProjectEvents_get_BuildManagerEvents_Proxy( 
    VSProjectEvents * This,
    /* [retval][out] */ BuildManagerEvents	**ppBuildMgrEvents);


void __RPC_STUB VSProjectEvents_get_BuildManagerEvents_Stub(
    IRpcStubBuffer *This,
    IRpcChannelBuffer *_pRpcChannelBuffer,
    PRPC_MESSAGE _pRpcMessage,
    DWORD *_pdwStubPhase);


/* [helpstring][propget][id] */ HRESULT STDMETHODCALLTYPE VSProjectEvents_get_ImportsEvents_Proxy( 
    VSProjectEvents * This,
    /* [retval][out] */ ImportsEvents	**ppImportsEvents);


void __RPC_STUB VSProjectEvents_get_ImportsEvents_Stub(
    IRpcStubBuffer *This,
    IRpcChannelBuffer *_pRpcChannelBuffer,
    PRPC_MESSAGE _pRpcMessage,
    DWORD *_pdwStubPhase);



#endif 	/* __VSProjectEvents_INTERFACE_DEFINED__ */


#ifndef __VSProject_INTERFACE_DEFINED__
#define __VSProject_INTERFACE_DEFINED__

/* interface VSProject */
/* [uuid][object][oleautomation][dual] */ 


EXTERN_C const IID IID_VSProject;

#if defined(__cplusplus) && !defined(CINTERFACE)
    
    MIDL_INTERFACE("2CFB826F-F6BF-480d-A546-95A0381CC411")
    VSProject : public IDispatch
    {
    public:
        virtual /* [helpstring][propget][id] */ HRESULT STDMETHODCALLTYPE get_References( 
            /* [retval][out] */ References **ppRefs) = 0;
        
        virtual /* [helpstring][propget][id] */ HRESULT STDMETHODCALLTYPE get_BuildManager( 
            /* [retval][out] */ BuildManager **ppBuildMgr) = 0;
        
        virtual /* [helpstring][propget][id] */ HRESULT STDMETHODCALLTYPE get_DTE( 
            /* [retval][out] */ /* external definition not present */ DTE **ppDTE) = 0;
        
        virtual /* [helpstring][propget][id] */ HRESULT STDMETHODCALLTYPE get_Project( 
            /* [retval][out] */ /* external definition not present */ Project **ppProject) = 0;
        
        virtual /* [helpstring][id] */ HRESULT STDMETHODCALLTYPE CreateWebReferencesFolder( 
            /* [retval][out] */ /* external definition not present */ ProjectItem **ppProjectItem) = 0;
        
        virtual /* [helpstring][propget][id] */ HRESULT STDMETHODCALLTYPE get_WebReferencesFolder( 
            /* [retval][out] */ /* external definition not present */ ProjectItem **ppProjectItem) = 0;
        
        virtual /* [helpstring][id] */ HRESULT STDMETHODCALLTYPE AddWebReference( 
            /* [in] */ BSTR bstrUrl,
            /* [retval][out] */ /* external definition not present */ ProjectItem **ppProjectItem) = 0;
        
        virtual /* [helpstring][propget][id] */ HRESULT STDMETHODCALLTYPE get_TemplatePath( 
            /* [retval][out] */ BSTR *pbstrTemplatePath) = 0;
        
        virtual /* [helpstring][id] */ HRESULT STDMETHODCALLTYPE Refresh( void) = 0;
        
        virtual /* [helpstring][propget][id] */ HRESULT STDMETHODCALLTYPE get_WorkOffline( 
            /* [retval][out] */ VARIANT_BOOL *pbWorkOffline) = 0;
        
        virtual /* [helpstring][propput][id] */ HRESULT STDMETHODCALLTYPE put_WorkOffline( 
            /* [in] */ VARIANT_BOOL bWorkOffline) = 0;
        
        virtual /* [helpstring][propget][id] */ HRESULT STDMETHODCALLTYPE get_Imports( 
            /* [retval][out] */ Imports **ppImports) = 0;
        
        virtual /* [helpstring][propget][id] */ HRESULT STDMETHODCALLTYPE get_Events( 
            /* [retval][out] */ VSProjectEvents **ppEvents) = 0;
        
        virtual /* [helpstring][id] */ HRESULT STDMETHODCALLTYPE CopyProject( 
            /* [in] */ BSTR bstrDestFolder,
            /* [in] */ BSTR bstrDestUNCPath,
            /* [in] */ prjCopyProjectOption copyProjectOption,
            /* [in] */ BSTR bstrUsername,
            /* [in] */ BSTR bstrPassword) = 0;
        
        virtual /* [hidden][helpstring][id] */ HRESULT STDMETHODCALLTYPE Exec( 
            /* [in] */ prjExecCommand command,
            BOOL bSuppressUI,
            /* [in] */ VARIANT varIn,
            /* [out] */ VARIANT *pVarOut) = 0;
        
        virtual /* [helpstring][id] */ HRESULT STDMETHODCALLTYPE GenerateKeyPairFiles( 
            /* [in] */ BSTR strPublicPrivateFile,
            /* [defaultvalue][in] */ BSTR strPublicOnlyFile = 0) = 0;
        
        virtual /* [helpstring][id] */ HRESULT STDMETHODCALLTYPE GetUniqueFilename( 
            /* [in] */ IDispatch *pDispatch,
            /* [in] */ BSTR bstrRoot,
            /* [in] */ BSTR bstrDesiredExt,
            /* [retval][out] */ BSTR *pbstrFileName) = 0;
        
    };
    
#else 	/* C style interface */

    typedef struct VSProjectVtbl
    {
        BEGIN_INTERFACE
        
        HRESULT ( STDMETHODCALLTYPE *QueryInterface )( 
            VSProject * This,
            /* [in] */ REFIID riid,
            /* [iid_is][out] */ void **ppvObject);
        
        ULONG ( STDMETHODCALLTYPE *AddRef )( 
            VSProject * This);
        
        ULONG ( STDMETHODCALLTYPE *Release )( 
            VSProject * This);
        
        HRESULT ( STDMETHODCALLTYPE *GetTypeInfoCount )( 
            VSProject * This,
            /* [out] */ UINT *pctinfo);
        
        HRESULT ( STDMETHODCALLTYPE *GetTypeInfo )( 
            VSProject * This,
            /* [in] */ UINT iTInfo,
            /* [in] */ LCID lcid,
            /* [out] */ ITypeInfo **ppTInfo);
        
        HRESULT ( STDMETHODCALLTYPE *GetIDsOfNames )( 
            VSProject * This,
            /* [in] */ REFIID riid,
            /* [size_is][in] */ LPOLESTR *rgszNames,
            /* [in] */ UINT cNames,
            /* [in] */ LCID lcid,
            /* [size_is][out] */ DISPID *rgDispId);
        
        /* [local] */ HRESULT ( STDMETHODCALLTYPE *Invoke )( 
            VSProject * This,
            /* [in] */ DISPID dispIdMember,
            /* [in] */ REFIID riid,
            /* [in] */ LCID lcid,
            /* [in] */ WORD wFlags,
            /* [out][in] */ DISPPARAMS *pDispParams,
            /* [out] */ VARIANT *pVarResult,
            /* [out] */ EXCEPINFO *pExcepInfo,
            /* [out] */ UINT *puArgErr);
        
        /* [helpstring][propget][id] */ HRESULT ( STDMETHODCALLTYPE *get_References )( 
            VSProject * This,
            /* [retval][out] */ References **ppRefs);
        
        /* [helpstring][propget][id] */ HRESULT ( STDMETHODCALLTYPE *get_BuildManager )( 
            VSProject * This,
            /* [retval][out] */ BuildManager **ppBuildMgr);
        
        /* [helpstring][propget][id] */ HRESULT ( STDMETHODCALLTYPE *get_DTE )( 
            VSProject * This,
            /* [retval][out] */ /* external definition not present */ DTE **ppDTE);
        
        /* [helpstring][propget][id] */ HRESULT ( STDMETHODCALLTYPE *get_Project )( 
            VSProject * This,
            /* [retval][out] */ /* external definition not present */ Project **ppProject);
        
        /* [helpstring][id] */ HRESULT ( STDMETHODCALLTYPE *CreateWebReferencesFolder )( 
            VSProject * This,
            /* [retval][out] */ /* external definition not present */ ProjectItem **ppProjectItem);
        
        /* [helpstring][propget][id] */ HRESULT ( STDMETHODCALLTYPE *get_WebReferencesFolder )( 
            VSProject * This,
            /* [retval][out] */ /* external definition not present */ ProjectItem **ppProjectItem);
        
        /* [helpstring][id] */ HRESULT ( STDMETHODCALLTYPE *AddWebReference )( 
            VSProject * This,
            /* [in] */ BSTR bstrUrl,
            /* [retval][out] */ /* external definition not present */ ProjectItem **ppProjectItem);
        
        /* [helpstring][propget][id] */ HRESULT ( STDMETHODCALLTYPE *get_TemplatePath )( 
            VSProject * This,
            /* [retval][out] */ BSTR *pbstrTemplatePath);
        
        /* [helpstring][id] */ HRESULT ( STDMETHODCALLTYPE *Refresh )( 
            VSProject * This);
        
        /* [helpstring][propget][id] */ HRESULT ( STDMETHODCALLTYPE *get_WorkOffline )( 
            VSProject * This,
            /* [retval][out] */ VARIANT_BOOL *pbWorkOffline);
        
        /* [helpstring][propput][id] */ HRESULT ( STDMETHODCALLTYPE *put_WorkOffline )( 
            VSProject * This,
            /* [in] */ VARIANT_BOOL bWorkOffline);
        
        /* [helpstring][propget][id] */ HRESULT ( STDMETHODCALLTYPE *get_Imports )( 
            VSProject * This,
            /* [retval][out] */ Imports **ppImports);
        
        /* [helpstring][propget][id] */ HRESULT ( STDMETHODCALLTYPE *get_Events )( 
            VSProject * This,
            /* [retval][out] */ VSProjectEvents **ppEvents);
        
        /* [helpstring][id] */ HRESULT ( STDMETHODCALLTYPE *CopyProject )( 
            VSProject * This,
            /* [in] */ BSTR bstrDestFolder,
            /* [in] */ BSTR bstrDestUNCPath,
            /* [in] */ prjCopyProjectOption copyProjectOption,
            /* [in] */ BSTR bstrUsername,
            /* [in] */ BSTR bstrPassword);
        
        /* [hidden][helpstring][id] */ HRESULT ( STDMETHODCALLTYPE *Exec )( 
            VSProject * This,
            /* [in] */ prjExecCommand command,
            BOOL bSuppressUI,
            /* [in] */ VARIANT varIn,
            /* [out] */ VARIANT *pVarOut);
        
        /* [helpstring][id] */ HRESULT ( STDMETHODCALLTYPE *GenerateKeyPairFiles )( 
            VSProject * This,
            /* [in] */ BSTR strPublicPrivateFile,
            /* [defaultvalue][in] */ BSTR strPublicOnlyFile);
        
        /* [helpstring][id] */ HRESULT ( STDMETHODCALLTYPE *GetUniqueFilename )( 
            VSProject * This,
            /* [in] */ IDispatch *pDispatch,
            /* [in] */ BSTR bstrRoot,
            /* [in] */ BSTR bstrDesiredExt,
            /* [retval][out] */ BSTR *pbstrFileName);
        
        END_INTERFACE
    } VSProjectVtbl;

    interface VSProject
    {
        CONST_VTBL struct VSProjectVtbl *lpVtbl;
    };

    

#ifdef COBJMACROS


#define VSProject_QueryInterface(This,riid,ppvObject)	\
    (This)->lpVtbl -> QueryInterface(This,riid,ppvObject)

#define VSProject_AddRef(This)	\
    (This)->lpVtbl -> AddRef(This)

#define VSProject_Release(This)	\
    (This)->lpVtbl -> Release(This)


#define VSProject_GetTypeInfoCount(This,pctinfo)	\
    (This)->lpVtbl -> GetTypeInfoCount(This,pctinfo)

#define VSProject_GetTypeInfo(This,iTInfo,lcid,ppTInfo)	\
    (This)->lpVtbl -> GetTypeInfo(This,iTInfo,lcid,ppTInfo)

#define VSProject_GetIDsOfNames(This,riid,rgszNames,cNames,lcid,rgDispId)	\
    (This)->lpVtbl -> GetIDsOfNames(This,riid,rgszNames,cNames,lcid,rgDispId)

#define VSProject_Invoke(This,dispIdMember,riid,lcid,wFlags,pDispParams,pVarResult,pExcepInfo,puArgErr)	\
    (This)->lpVtbl -> Invoke(This,dispIdMember,riid,lcid,wFlags,pDispParams,pVarResult,pExcepInfo,puArgErr)


#define VSProject_get_References(This,ppRefs)	\
    (This)->lpVtbl -> get_References(This,ppRefs)

#define VSProject_get_BuildManager(This,ppBuildMgr)	\
    (This)->lpVtbl -> get_BuildManager(This,ppBuildMgr)

#define VSProject_get_DTE(This,ppDTE)	\
    (This)->lpVtbl -> get_DTE(This,ppDTE)

#define VSProject_get_Project(This,ppProject)	\
    (This)->lpVtbl -> get_Project(This,ppProject)

#define VSProject_CreateWebReferencesFolder(This,ppProjectItem)	\
    (This)->lpVtbl -> CreateWebReferencesFolder(This,ppProjectItem)

#define VSProject_get_WebReferencesFolder(This,ppProjectItem)	\
    (This)->lpVtbl -> get_WebReferencesFolder(This,ppProjectItem)

#define VSProject_AddWebReference(This,bstrUrl,ppProjectItem)	\
    (This)->lpVtbl -> AddWebReference(This,bstrUrl,ppProjectItem)

#define VSProject_get_TemplatePath(This,pbstrTemplatePath)	\
    (This)->lpVtbl -> get_TemplatePath(This,pbstrTemplatePath)

#define VSProject_Refresh(This)	\
    (This)->lpVtbl -> Refresh(This)

#define VSProject_get_WorkOffline(This,pbWorkOffline)	\
    (This)->lpVtbl -> get_WorkOffline(This,pbWorkOffline)

#define VSProject_put_WorkOffline(This,bWorkOffline)	\
    (This)->lpVtbl -> put_WorkOffline(This,bWorkOffline)

#define VSProject_get_Imports(This,ppImports)	\
    (This)->lpVtbl -> get_Imports(This,ppImports)

#define VSProject_get_Events(This,ppEvents)	\
    (This)->lpVtbl -> get_Events(This,ppEvents)

#define VSProject_CopyProject(This,bstrDestFolder,bstrDestUNCPath,copyProjectOption,bstrUsername,bstrPassword)	\
    (This)->lpVtbl -> CopyProject(This,bstrDestFolder,bstrDestUNCPath,copyProjectOption,bstrUsername,bstrPassword)

#define VSProject_Exec(This,command,bSuppressUI,varIn,pVarOut)	\
    (This)->lpVtbl -> Exec(This,command,bSuppressUI,varIn,pVarOut)

#define VSProject_GenerateKeyPairFiles(This,strPublicPrivateFile,strPublicOnlyFile)	\
    (This)->lpVtbl -> GenerateKeyPairFiles(This,strPublicPrivateFile,strPublicOnlyFile)

#define VSProject_GetUniqueFilename(This,pDispatch,bstrRoot,bstrDesiredExt,pbstrFileName)	\
    (This)->lpVtbl -> GetUniqueFilename(This,pDispatch,bstrRoot,bstrDesiredExt,pbstrFileName)

#endif /* COBJMACROS */


#endif 	/* C style interface */



/* [helpstring][propget][id] */ HRESULT STDMETHODCALLTYPE VSProject_get_References_Proxy( 
    VSProject * This,
    /* [retval][out] */ References **ppRefs);


void __RPC_STUB VSProject_get_References_Stub(
    IRpcStubBuffer *This,
    IRpcChannelBuffer *_pRpcChannelBuffer,
    PRPC_MESSAGE _pRpcMessage,
    DWORD *_pdwStubPhase);


/* [helpstring][propget][id] */ HRESULT STDMETHODCALLTYPE VSProject_get_BuildManager_Proxy( 
    VSProject * This,
    /* [retval][out] */ BuildManager **ppBuildMgr);


void __RPC_STUB VSProject_get_BuildManager_Stub(
    IRpcStubBuffer *This,
    IRpcChannelBuffer *_pRpcChannelBuffer,
    PRPC_MESSAGE _pRpcMessage,
    DWORD *_pdwStubPhase);


/* [helpstring][propget][id] */ HRESULT STDMETHODCALLTYPE VSProject_get_DTE_Proxy( 
    VSProject * This,
    /* [retval][out] */ /* external definition not present */ DTE **ppDTE);


void __RPC_STUB VSProject_get_DTE_Stub(
    IRpcStubBuffer *This,
    IRpcChannelBuffer *_pRpcChannelBuffer,
    PRPC_MESSAGE _pRpcMessage,
    DWORD *_pdwStubPhase);


/* [helpstring][propget][id] */ HRESULT STDMETHODCALLTYPE VSProject_get_Project_Proxy( 
    VSProject * This,
    /* [retval][out] */ /* external definition not present */ Project **ppProject);


void __RPC_STUB VSProject_get_Project_Stub(
    IRpcStubBuffer *This,
    IRpcChannelBuffer *_pRpcChannelBuffer,
    PRPC_MESSAGE _pRpcMessage,
    DWORD *_pdwStubPhase);


/* [helpstring][id] */ HRESULT STDMETHODCALLTYPE VSProject_CreateWebReferencesFolder_Proxy( 
    VSProject * This,
    /* [retval][out] */ /* external definition not present */ ProjectItem **ppProjectItem);


void __RPC_STUB VSProject_CreateWebReferencesFolder_Stub(
    IRpcStubBuffer *This,
    IRpcChannelBuffer *_pRpcChannelBuffer,
    PRPC_MESSAGE _pRpcMessage,
    DWORD *_pdwStubPhase);


/* [helpstring][propget][id] */ HRESULT STDMETHODCALLTYPE VSProject_get_WebReferencesFolder_Proxy( 
    VSProject * This,
    /* [retval][out] */ /* external definition not present */ ProjectItem **ppProjectItem);


void __RPC_STUB VSProject_get_WebReferencesFolder_Stub(
    IRpcStubBuffer *This,
    IRpcChannelBuffer *_pRpcChannelBuffer,
    PRPC_MESSAGE _pRpcMessage,
    DWORD *_pdwStubPhase);


/* [helpstring][id] */ HRESULT STDMETHODCALLTYPE VSProject_AddWebReference_Proxy( 
    VSProject * This,
    /* [in] */ BSTR bstrUrl,
    /* [retval][out] */ /* external definition not present */ ProjectItem **ppProjectItem);


void __RPC_STUB VSProject_AddWebReference_Stub(
    IRpcStubBuffer *This,
    IRpcChannelBuffer *_pRpcChannelBuffer,
    PRPC_MESSAGE _pRpcMessage,
    DWORD *_pdwStubPhase);


/* [helpstring][propget][id] */ HRESULT STDMETHODCALLTYPE VSProject_get_TemplatePath_Proxy( 
    VSProject * This,
    /* [retval][out] */ BSTR *pbstrTemplatePath);


void __RPC_STUB VSProject_get_TemplatePath_Stub(
    IRpcStubBuffer *This,
    IRpcChannelBuffer *_pRpcChannelBuffer,
    PRPC_MESSAGE _pRpcMessage,
    DWORD *_pdwStubPhase);


/* [helpstring][id] */ HRESULT STDMETHODCALLTYPE VSProject_Refresh_Proxy( 
    VSProject * This);


void __RPC_STUB VSProject_Refresh_Stub(
    IRpcStubBuffer *This,
    IRpcChannelBuffer *_pRpcChannelBuffer,
    PRPC_MESSAGE _pRpcMessage,
    DWORD *_pdwStubPhase);


/* [helpstring][propget][id] */ HRESULT STDMETHODCALLTYPE VSProject_get_WorkOffline_Proxy( 
    VSProject * This,
    /* [retval][out] */ VARIANT_BOOL *pbWorkOffline);


void __RPC_STUB VSProject_get_WorkOffline_Stub(
    IRpcStubBuffer *This,
    IRpcChannelBuffer *_pRpcChannelBuffer,
    PRPC_MESSAGE _pRpcMessage,
    DWORD *_pdwStubPhase);


/* [helpstring][propput][id] */ HRESULT STDMETHODCALLTYPE VSProject_put_WorkOffline_Proxy( 
    VSProject * This,
    /* [in] */ VARIANT_BOOL bWorkOffline);


void __RPC_STUB VSProject_put_WorkOffline_Stub(
    IRpcStubBuffer *This,
    IRpcChannelBuffer *_pRpcChannelBuffer,
    PRPC_MESSAGE _pRpcMessage,
    DWORD *_pdwStubPhase);


/* [helpstring][propget][id] */ HRESULT STDMETHODCALLTYPE VSProject_get_Imports_Proxy( 
    VSProject * This,
    /* [retval][out] */ Imports **ppImports);


void __RPC_STUB VSProject_get_Imports_Stub(
    IRpcStubBuffer *This,
    IRpcChannelBuffer *_pRpcChannelBuffer,
    PRPC_MESSAGE _pRpcMessage,
    DWORD *_pdwStubPhase);


/* [helpstring][propget][id] */ HRESULT STDMETHODCALLTYPE VSProject_get_Events_Proxy( 
    VSProject * This,
    /* [retval][out] */ VSProjectEvents **ppEvents);


void __RPC_STUB VSProject_get_Events_Stub(
    IRpcStubBuffer *This,
    IRpcChannelBuffer *_pRpcChannelBuffer,
    PRPC_MESSAGE _pRpcMessage,
    DWORD *_pdwStubPhase);


/* [helpstring][id] */ HRESULT STDMETHODCALLTYPE VSProject_CopyProject_Proxy( 
    VSProject * This,
    /* [in] */ BSTR bstrDestFolder,
    /* [in] */ BSTR bstrDestUNCPath,
    /* [in] */ prjCopyProjectOption copyProjectOption,
    /* [in] */ BSTR bstrUsername,
    /* [in] */ BSTR bstrPassword);


void __RPC_STUB VSProject_CopyProject_Stub(
    IRpcStubBuffer *This,
    IRpcChannelBuffer *_pRpcChannelBuffer,
    PRPC_MESSAGE _pRpcMessage,
    DWORD *_pdwStubPhase);


/* [hidden][helpstring][id] */ HRESULT STDMETHODCALLTYPE VSProject_Exec_Proxy( 
    VSProject * This,
    /* [in] */ prjExecCommand command,
    BOOL bSuppressUI,
    /* [in] */ VARIANT varIn,
    /* [out] */ VARIANT *pVarOut);


void __RPC_STUB VSProject_Exec_Stub(
    IRpcStubBuffer *This,
    IRpcChannelBuffer *_pRpcChannelBuffer,
    PRPC_MESSAGE _pRpcMessage,
    DWORD *_pdwStubPhase);


/* [helpstring][id] */ HRESULT STDMETHODCALLTYPE VSProject_GenerateKeyPairFiles_Proxy( 
    VSProject * This,
    /* [in] */ BSTR strPublicPrivateFile,
    /* [defaultvalue][in] */ BSTR strPublicOnlyFile);


void __RPC_STUB VSProject_GenerateKeyPairFiles_Stub(
    IRpcStubBuffer *This,
    IRpcChannelBuffer *_pRpcChannelBuffer,
    PRPC_MESSAGE _pRpcMessage,
    DWORD *_pdwStubPhase);


/* [helpstring][id] */ HRESULT STDMETHODCALLTYPE VSProject_GetUniqueFilename_Proxy( 
    VSProject * This,
    /* [in] */ IDispatch *pDispatch,
    /* [in] */ BSTR bstrRoot,
    /* [in] */ BSTR bstrDesiredExt,
    /* [retval][out] */ BSTR *pbstrFileName);


void __RPC_STUB VSProject_GetUniqueFilename_Stub(
    IRpcStubBuffer *This,
    IRpcChannelBuffer *_pRpcChannelBuffer,
    PRPC_MESSAGE _pRpcMessage,
    DWORD *_pdwStubPhase);



#endif 	/* __VSProject_INTERFACE_DEFINED__ */


#ifndef __VSProjectItem_INTERFACE_DEFINED__
#define __VSProjectItem_INTERFACE_DEFINED__

/* interface VSProjectItem */
/* [uuid][object][oleautomation][dual] */ 


EXTERN_C const IID IID_VSProjectItem;

#if defined(__cplusplus) && !defined(CINTERFACE)
    
    MIDL_INTERFACE("89FF44C6-979D-49b6-AF56-EC9509001DE4")
    VSProjectItem : public IDispatch
    {
    public:
        virtual /* [helpstring][propget][id] */ HRESULT STDMETHODCALLTYPE get_DTE( 
            /* [retval][out] */ /* external definition not present */ DTE **ppDTE) = 0;
        
        virtual /* [helpstring][propget][id] */ HRESULT STDMETHODCALLTYPE get_ProjectItem( 
            /* [retval][out] */ /* external definition not present */ ProjectItem **ppProjectItem) = 0;
        
        virtual /* [helpstring][propget][id] */ HRESULT STDMETHODCALLTYPE get_ContainingProject( 
            /* [retval][out] */ /* external definition not present */ Project **ppProject) = 0;
        
        virtual /* [helpstring][id] */ HRESULT STDMETHODCALLTYPE RunCustomTool( void) = 0;
        
    };
    
#else 	/* C style interface */

    typedef struct VSProjectItemVtbl
    {
        BEGIN_INTERFACE
        
        HRESULT ( STDMETHODCALLTYPE *QueryInterface )( 
            VSProjectItem * This,
            /* [in] */ REFIID riid,
            /* [iid_is][out] */ void **ppvObject);
        
        ULONG ( STDMETHODCALLTYPE *AddRef )( 
            VSProjectItem * This);
        
        ULONG ( STDMETHODCALLTYPE *Release )( 
            VSProjectItem * This);
        
        HRESULT ( STDMETHODCALLTYPE *GetTypeInfoCount )( 
            VSProjectItem * This,
            /* [out] */ UINT *pctinfo);
        
        HRESULT ( STDMETHODCALLTYPE *GetTypeInfo )( 
            VSProjectItem * This,
            /* [in] */ UINT iTInfo,
            /* [in] */ LCID lcid,
            /* [out] */ ITypeInfo **ppTInfo);
        
        HRESULT ( STDMETHODCALLTYPE *GetIDsOfNames )( 
            VSProjectItem * This,
            /* [in] */ REFIID riid,
            /* [size_is][in] */ LPOLESTR *rgszNames,
            /* [in] */ UINT cNames,
            /* [in] */ LCID lcid,
            /* [size_is][out] */ DISPID *rgDispId);
        
        /* [local] */ HRESULT ( STDMETHODCALLTYPE *Invoke )( 
            VSProjectItem * This,
            /* [in] */ DISPID dispIdMember,
            /* [in] */ REFIID riid,
            /* [in] */ LCID lcid,
            /* [in] */ WORD wFlags,
            /* [out][in] */ DISPPARAMS *pDispParams,
            /* [out] */ VARIANT *pVarResult,
            /* [out] */ EXCEPINFO *pExcepInfo,
            /* [out] */ UINT *puArgErr);
        
        /* [helpstring][propget][id] */ HRESULT ( STDMETHODCALLTYPE *get_DTE )( 
            VSProjectItem * This,
            /* [retval][out] */ /* external definition not present */ DTE **ppDTE);
        
        /* [helpstring][propget][id] */ HRESULT ( STDMETHODCALLTYPE *get_ProjectItem )( 
            VSProjectItem * This,
            /* [retval][out] */ /* external definition not present */ ProjectItem **ppProjectItem);
        
        /* [helpstring][propget][id] */ HRESULT ( STDMETHODCALLTYPE *get_ContainingProject )( 
            VSProjectItem * This,
            /* [retval][out] */ /* external definition not present */ Project **ppProject);
        
        /* [helpstring][id] */ HRESULT ( STDMETHODCALLTYPE *RunCustomTool )( 
            VSProjectItem * This);
        
        END_INTERFACE
    } VSProjectItemVtbl;

    interface VSProjectItem
    {
        CONST_VTBL struct VSProjectItemVtbl *lpVtbl;
    };

    

#ifdef COBJMACROS


#define VSProjectItem_QueryInterface(This,riid,ppvObject)	\
    (This)->lpVtbl -> QueryInterface(This,riid,ppvObject)

#define VSProjectItem_AddRef(This)	\
    (This)->lpVtbl -> AddRef(This)

#define VSProjectItem_Release(This)	\
    (This)->lpVtbl -> Release(This)


#define VSProjectItem_GetTypeInfoCount(This,pctinfo)	\
    (This)->lpVtbl -> GetTypeInfoCount(This,pctinfo)

#define VSProjectItem_GetTypeInfo(This,iTInfo,lcid,ppTInfo)	\
    (This)->lpVtbl -> GetTypeInfo(This,iTInfo,lcid,ppTInfo)

#define VSProjectItem_GetIDsOfNames(This,riid,rgszNames,cNames,lcid,rgDispId)	\
    (This)->lpVtbl -> GetIDsOfNames(This,riid,rgszNames,cNames,lcid,rgDispId)

#define VSProjectItem_Invoke(This,dispIdMember,riid,lcid,wFlags,pDispParams,pVarResult,pExcepInfo,puArgErr)	\
    (This)->lpVtbl -> Invoke(This,dispIdMember,riid,lcid,wFlags,pDispParams,pVarResult,pExcepInfo,puArgErr)


#define VSProjectItem_get_DTE(This,ppDTE)	\
    (This)->lpVtbl -> get_DTE(This,ppDTE)

#define VSProjectItem_get_ProjectItem(This,ppProjectItem)	\
    (This)->lpVtbl -> get_ProjectItem(This,ppProjectItem)

#define VSProjectItem_get_ContainingProject(This,ppProject)	\
    (This)->lpVtbl -> get_ContainingProject(This,ppProject)

#define VSProjectItem_RunCustomTool(This)	\
    (This)->lpVtbl -> RunCustomTool(This)

#endif /* COBJMACROS */


#endif 	/* C style interface */



/* [helpstring][propget][id] */ HRESULT STDMETHODCALLTYPE VSProjectItem_get_DTE_Proxy( 
    VSProjectItem * This,
    /* [retval][out] */ /* external definition not present */ DTE **ppDTE);


void __RPC_STUB VSProjectItem_get_DTE_Stub(
    IRpcStubBuffer *This,
    IRpcChannelBuffer *_pRpcChannelBuffer,
    PRPC_MESSAGE _pRpcMessage,
    DWORD *_pdwStubPhase);


/* [helpstring][propget][id] */ HRESULT STDMETHODCALLTYPE VSProjectItem_get_ProjectItem_Proxy( 
    VSProjectItem * This,
    /* [retval][out] */ /* external definition not present */ ProjectItem **ppProjectItem);


void __RPC_STUB VSProjectItem_get_ProjectItem_Stub(
    IRpcStubBuffer *This,
    IRpcChannelBuffer *_pRpcChannelBuffer,
    PRPC_MESSAGE _pRpcMessage,
    DWORD *_pdwStubPhase);


/* [helpstring][propget][id] */ HRESULT STDMETHODCALLTYPE VSProjectItem_get_ContainingProject_Proxy( 
    VSProjectItem * This,
    /* [retval][out] */ /* external definition not present */ Project **ppProject);


void __RPC_STUB VSProjectItem_get_ContainingProject_Stub(
    IRpcStubBuffer *This,
    IRpcChannelBuffer *_pRpcChannelBuffer,
    PRPC_MESSAGE _pRpcMessage,
    DWORD *_pdwStubPhase);


/* [helpstring][id] */ HRESULT STDMETHODCALLTYPE VSProjectItem_RunCustomTool_Proxy( 
    VSProjectItem * This);


void __RPC_STUB VSProjectItem_RunCustomTool_Stub(
    IRpcStubBuffer *This,
    IRpcChannelBuffer *_pRpcChannelBuffer,
    PRPC_MESSAGE _pRpcMessage,
    DWORD *_pdwStubPhase);



#endif 	/* __VSProjectItem_INTERFACE_DEFINED__ */


#ifndef __WebSettings_INTERFACE_DEFINED__
#define __WebSettings_INTERFACE_DEFINED__

/* interface WebSettings */
/* [uuid][object][oleautomation][dual] */ 


EXTERN_C const IID IID_WebSettings;

#if defined(__cplusplus) && !defined(CINTERFACE)
    
    MIDL_INTERFACE("8FA8D496-CAF1-40b3-BC58-5FC877FEFEA7")
    WebSettings : public IDispatch
    {
    public:
        virtual /* [helpstring][propget][id] */ HRESULT STDMETHODCALLTYPE get_AuthoringAccess( 
            /* [retval][out] */ webPrjAuthoringAccess *pAccessMethod) = 0;
        
        virtual /* [helpstring][propput][id] */ HRESULT STDMETHODCALLTYPE put_AuthoringAccess( 
            /* [in] */ webPrjAuthoringAccess AccessMethod) = 0;
        
        virtual /* [helpstring][propget][id] */ HRESULT STDMETHODCALLTYPE get_RepairLinks( 
            /* [retval][out] */ VARIANT_BOOL *pbRepairLinks) = 0;
        
        virtual /* [helpstring][propput][id] */ HRESULT STDMETHODCALLTYPE put_RepairLinks( 
            /* [in] */ VARIANT_BOOL bRepairLinks) = 0;
        
        virtual /* [helpstring][propget][id] */ HRESULT STDMETHODCALLTYPE get_WebProjectCacheDirectory( 
            /* [retval][out] */ BSTR *pbstrDirectory) = 0;
        
        virtual /* [helpstring][propput][id] */ HRESULT STDMETHODCALLTYPE put_WebProjectCacheDirectory( 
            /* [in] */ BSTR bstrDirectory) = 0;
        
        virtual /* [helpstring][id] */ HRESULT STDMETHODCALLTYPE SetDefaultWebProjectCacheDirectory( void) = 0;
        
    };
    
#else 	/* C style interface */

    typedef struct WebSettingsVtbl
    {
        BEGIN_INTERFACE
        
        HRESULT ( STDMETHODCALLTYPE *QueryInterface )( 
            WebSettings * This,
            /* [in] */ REFIID riid,
            /* [iid_is][out] */ void **ppvObject);
        
        ULONG ( STDMETHODCALLTYPE *AddRef )( 
            WebSettings * This);
        
        ULONG ( STDMETHODCALLTYPE *Release )( 
            WebSettings * This);
        
        HRESULT ( STDMETHODCALLTYPE *GetTypeInfoCount )( 
            WebSettings * This,
            /* [out] */ UINT *pctinfo);
        
        HRESULT ( STDMETHODCALLTYPE *GetTypeInfo )( 
            WebSettings * This,
            /* [in] */ UINT iTInfo,
            /* [in] */ LCID lcid,
            /* [out] */ ITypeInfo **ppTInfo);
        
        HRESULT ( STDMETHODCALLTYPE *GetIDsOfNames )( 
            WebSettings * This,
            /* [in] */ REFIID riid,
            /* [size_is][in] */ LPOLESTR *rgszNames,
            /* [in] */ UINT cNames,
            /* [in] */ LCID lcid,
            /* [size_is][out] */ DISPID *rgDispId);
        
        /* [local] */ HRESULT ( STDMETHODCALLTYPE *Invoke )( 
            WebSettings * This,
            /* [in] */ DISPID dispIdMember,
            /* [in] */ REFIID riid,
            /* [in] */ LCID lcid,
            /* [in] */ WORD wFlags,
            /* [out][in] */ DISPPARAMS *pDispParams,
            /* [out] */ VARIANT *pVarResult,
            /* [out] */ EXCEPINFO *pExcepInfo,
            /* [out] */ UINT *puArgErr);
        
        /* [helpstring][propget][id] */ HRESULT ( STDMETHODCALLTYPE *get_AuthoringAccess )( 
            WebSettings * This,
            /* [retval][out] */ webPrjAuthoringAccess *pAccessMethod);
        
        /* [helpstring][propput][id] */ HRESULT ( STDMETHODCALLTYPE *put_AuthoringAccess )( 
            WebSettings * This,
            /* [in] */ webPrjAuthoringAccess AccessMethod);
        
        /* [helpstring][propget][id] */ HRESULT ( STDMETHODCALLTYPE *get_RepairLinks )( 
            WebSettings * This,
            /* [retval][out] */ VARIANT_BOOL *pbRepairLinks);
        
        /* [helpstring][propput][id] */ HRESULT ( STDMETHODCALLTYPE *put_RepairLinks )( 
            WebSettings * This,
            /* [in] */ VARIANT_BOOL bRepairLinks);
        
        /* [helpstring][propget][id] */ HRESULT ( STDMETHODCALLTYPE *get_WebProjectCacheDirectory )( 
            WebSettings * This,
            /* [retval][out] */ BSTR *pbstrDirectory);
        
        /* [helpstring][propput][id] */ HRESULT ( STDMETHODCALLTYPE *put_WebProjectCacheDirectory )( 
            WebSettings * This,
            /* [in] */ BSTR bstrDirectory);
        
        /* [helpstring][id] */ HRESULT ( STDMETHODCALLTYPE *SetDefaultWebProjectCacheDirectory )( 
            WebSettings * This);
        
        END_INTERFACE
    } WebSettingsVtbl;

    interface WebSettings
    {
        CONST_VTBL struct WebSettingsVtbl *lpVtbl;
    };

    

#ifdef COBJMACROS


#define WebSettings_QueryInterface(This,riid,ppvObject)	\
    (This)->lpVtbl -> QueryInterface(This,riid,ppvObject)

#define WebSettings_AddRef(This)	\
    (This)->lpVtbl -> AddRef(This)

#define WebSettings_Release(This)	\
    (This)->lpVtbl -> Release(This)


#define WebSettings_GetTypeInfoCount(This,pctinfo)	\
    (This)->lpVtbl -> GetTypeInfoCount(This,pctinfo)

#define WebSettings_GetTypeInfo(This,iTInfo,lcid,ppTInfo)	\
    (This)->lpVtbl -> GetTypeInfo(This,iTInfo,lcid,ppTInfo)

#define WebSettings_GetIDsOfNames(This,riid,rgszNames,cNames,lcid,rgDispId)	\
    (This)->lpVtbl -> GetIDsOfNames(This,riid,rgszNames,cNames,lcid,rgDispId)

#define WebSettings_Invoke(This,dispIdMember,riid,lcid,wFlags,pDispParams,pVarResult,pExcepInfo,puArgErr)	\
    (This)->lpVtbl -> Invoke(This,dispIdMember,riid,lcid,wFlags,pDispParams,pVarResult,pExcepInfo,puArgErr)


#define WebSettings_get_AuthoringAccess(This,pAccessMethod)	\
    (This)->lpVtbl -> get_AuthoringAccess(This,pAccessMethod)

#define WebSettings_put_AuthoringAccess(This,AccessMethod)	\
    (This)->lpVtbl -> put_AuthoringAccess(This,AccessMethod)

#define WebSettings_get_RepairLinks(This,pbRepairLinks)	\
    (This)->lpVtbl -> get_RepairLinks(This,pbRepairLinks)

#define WebSettings_put_RepairLinks(This,bRepairLinks)	\
    (This)->lpVtbl -> put_RepairLinks(This,bRepairLinks)

#define WebSettings_get_WebProjectCacheDirectory(This,pbstrDirectory)	\
    (This)->lpVtbl -> get_WebProjectCacheDirectory(This,pbstrDirectory)

#define WebSettings_put_WebProjectCacheDirectory(This,bstrDirectory)	\
    (This)->lpVtbl -> put_WebProjectCacheDirectory(This,bstrDirectory)

#define WebSettings_SetDefaultWebProjectCacheDirectory(This)	\
    (This)->lpVtbl -> SetDefaultWebProjectCacheDirectory(This)

#endif /* COBJMACROS */


#endif 	/* C style interface */



/* [helpstring][propget][id] */ HRESULT STDMETHODCALLTYPE WebSettings_get_AuthoringAccess_Proxy( 
    WebSettings * This,
    /* [retval][out] */ webPrjAuthoringAccess *pAccessMethod);


void __RPC_STUB WebSettings_get_AuthoringAccess_Stub(
    IRpcStubBuffer *This,
    IRpcChannelBuffer *_pRpcChannelBuffer,
    PRPC_MESSAGE _pRpcMessage,
    DWORD *_pdwStubPhase);


/* [helpstring][propput][id] */ HRESULT STDMETHODCALLTYPE WebSettings_put_AuthoringAccess_Proxy( 
    WebSettings * This,
    /* [in] */ webPrjAuthoringAccess AccessMethod);


void __RPC_STUB WebSettings_put_AuthoringAccess_Stub(
    IRpcStubBuffer *This,
    IRpcChannelBuffer *_pRpcChannelBuffer,
    PRPC_MESSAGE _pRpcMessage,
    DWORD *_pdwStubPhase);


/* [helpstring][propget][id] */ HRESULT STDMETHODCALLTYPE WebSettings_get_RepairLinks_Proxy( 
    WebSettings * This,
    /* [retval][out] */ VARIANT_BOOL *pbRepairLinks);


void __RPC_STUB WebSettings_get_RepairLinks_Stub(
    IRpcStubBuffer *This,
    IRpcChannelBuffer *_pRpcChannelBuffer,
    PRPC_MESSAGE _pRpcMessage,
    DWORD *_pdwStubPhase);


/* [helpstring][propput][id] */ HRESULT STDMETHODCALLTYPE WebSettings_put_RepairLinks_Proxy( 
    WebSettings * This,
    /* [in] */ VARIANT_BOOL bRepairLinks);


void __RPC_STUB WebSettings_put_RepairLinks_Stub(
    IRpcStubBuffer *This,
    IRpcChannelBuffer *_pRpcChannelBuffer,
    PRPC_MESSAGE _pRpcMessage,
    DWORD *_pdwStubPhase);


/* [helpstring][propget][id] */ HRESULT STDMETHODCALLTYPE WebSettings_get_WebProjectCacheDirectory_Proxy( 
    WebSettings * This,
    /* [retval][out] */ BSTR *pbstrDirectory);


void __RPC_STUB WebSettings_get_WebProjectCacheDirectory_Stub(
    IRpcStubBuffer *This,
    IRpcChannelBuffer *_pRpcChannelBuffer,
    PRPC_MESSAGE _pRpcMessage,
    DWORD *_pdwStubPhase);


/* [helpstring][propput][id] */ HRESULT STDMETHODCALLTYPE WebSettings_put_WebProjectCacheDirectory_Proxy( 
    WebSettings * This,
    /* [in] */ BSTR bstrDirectory);


void __RPC_STUB WebSettings_put_WebProjectCacheDirectory_Stub(
    IRpcStubBuffer *This,
    IRpcChannelBuffer *_pRpcChannelBuffer,
    PRPC_MESSAGE _pRpcMessage,
    DWORD *_pdwStubPhase);


/* [helpstring][id] */ HRESULT STDMETHODCALLTYPE WebSettings_SetDefaultWebProjectCacheDirectory_Proxy( 
    WebSettings * This);


void __RPC_STUB WebSettings_SetDefaultWebProjectCacheDirectory_Stub(
    IRpcStubBuffer *This,
    IRpcChannelBuffer *_pRpcChannelBuffer,
    PRPC_MESSAGE _pRpcMessage,
    DWORD *_pdwStubPhase);



#endif 	/* __WebSettings_INTERFACE_DEFINED__ */

#endif /* __VSLangProj_LIBRARY_DEFINED__ */

#ifndef __IVSWebReferenceDynamicProperties_INTERFACE_DEFINED__
#define __IVSWebReferenceDynamicProperties_INTERFACE_DEFINED__

/* interface IVSWebReferenceDynamicProperties */
/* [uuid][object] */ 


EXTERN_C const IID IID_IVSWebReferenceDynamicProperties;

#if defined(__cplusplus) && !defined(CINTERFACE)
    
    MIDL_INTERFACE("C65A2F92-B350-4b0f-9BC6-B00E24BC8B9D")
    IVSWebReferenceDynamicProperties : public IUnknown
    {
    public:
        virtual /* [helpstring][id] */ HRESULT STDMETHODCALLTYPE GetDynamicPropertyName( 
            /* [in] */ LPCWSTR pszWebServiceName,
            /* [retval][out] */ BSTR *pbstrPropertyName) = 0;
        
        virtual /* [helpstring][id] */ HRESULT STDMETHODCALLTYPE SetDynamicProperty( 
            /* [in] */ LPCWSTR pszUrl,
            /* [in] */ LPCWSTR pszPropertyName) = 0;
        
        virtual /* [helpstring][id] */ HRESULT STDMETHODCALLTYPE SupportsDynamicProperties( 
            /* [retval][out] */ VARIANT_BOOL *pbSupportsDynamicProperties) = 0;
        
    };
    
#else 	/* C style interface */

    typedef struct IVSWebReferenceDynamicPropertiesVtbl
    {
        BEGIN_INTERFACE
        
        HRESULT ( STDMETHODCALLTYPE *QueryInterface )( 
            IVSWebReferenceDynamicProperties * This,
            /* [in] */ REFIID riid,
            /* [iid_is][out] */ void **ppvObject);
        
        ULONG ( STDMETHODCALLTYPE *AddRef )( 
            IVSWebReferenceDynamicProperties * This);
        
        ULONG ( STDMETHODCALLTYPE *Release )( 
            IVSWebReferenceDynamicProperties * This);
        
        /* [helpstring][id] */ HRESULT ( STDMETHODCALLTYPE *GetDynamicPropertyName )( 
            IVSWebReferenceDynamicProperties * This,
            /* [in] */ LPCWSTR pszWebServiceName,
            /* [retval][out] */ BSTR *pbstrPropertyName);
        
        /* [helpstring][id] */ HRESULT ( STDMETHODCALLTYPE *SetDynamicProperty )( 
            IVSWebReferenceDynamicProperties * This,
            /* [in] */ LPCWSTR pszUrl,
            /* [in] */ LPCWSTR pszPropertyName);
        
        /* [helpstring][id] */ HRESULT ( STDMETHODCALLTYPE *SupportsDynamicProperties )( 
            IVSWebReferenceDynamicProperties * This,
            /* [retval][out] */ VARIANT_BOOL *pbSupportsDynamicProperties);
        
        END_INTERFACE
    } IVSWebReferenceDynamicPropertiesVtbl;

    interface IVSWebReferenceDynamicProperties
    {
        CONST_VTBL struct IVSWebReferenceDynamicPropertiesVtbl *lpVtbl;
    };

    

#ifdef COBJMACROS


#define IVSWebReferenceDynamicProperties_QueryInterface(This,riid,ppvObject)	\
    (This)->lpVtbl -> QueryInterface(This,riid,ppvObject)

#define IVSWebReferenceDynamicProperties_AddRef(This)	\
    (This)->lpVtbl -> AddRef(This)

#define IVSWebReferenceDynamicProperties_Release(This)	\
    (This)->lpVtbl -> Release(This)


#define IVSWebReferenceDynamicProperties_GetDynamicPropertyName(This,pszWebServiceName,pbstrPropertyName)	\
    (This)->lpVtbl -> GetDynamicPropertyName(This,pszWebServiceName,pbstrPropertyName)

#define IVSWebReferenceDynamicProperties_SetDynamicProperty(This,pszUrl,pszPropertyName)	\
    (This)->lpVtbl -> SetDynamicProperty(This,pszUrl,pszPropertyName)

#define IVSWebReferenceDynamicProperties_SupportsDynamicProperties(This,pbSupportsDynamicProperties)	\
    (This)->lpVtbl -> SupportsDynamicProperties(This,pbSupportsDynamicProperties)

#endif /* COBJMACROS */


#endif 	/* C style interface */



/* [helpstring][id] */ HRESULT STDMETHODCALLTYPE IVSWebReferenceDynamicProperties_GetDynamicPropertyName_Proxy( 
    IVSWebReferenceDynamicProperties * This,
    /* [in] */ LPCWSTR pszWebServiceName,
    /* [retval][out] */ BSTR *pbstrPropertyName);


void __RPC_STUB IVSWebReferenceDynamicProperties_GetDynamicPropertyName_Stub(
    IRpcStubBuffer *This,
    IRpcChannelBuffer *_pRpcChannelBuffer,
    PRPC_MESSAGE _pRpcMessage,
    DWORD *_pdwStubPhase);


/* [helpstring][id] */ HRESULT STDMETHODCALLTYPE IVSWebReferenceDynamicProperties_SetDynamicProperty_Proxy( 
    IVSWebReferenceDynamicProperties * This,
    /* [in] */ LPCWSTR pszUrl,
    /* [in] */ LPCWSTR pszPropertyName);


void __RPC_STUB IVSWebReferenceDynamicProperties_SetDynamicProperty_Stub(
    IRpcStubBuffer *This,
    IRpcChannelBuffer *_pRpcChannelBuffer,
    PRPC_MESSAGE _pRpcMessage,
    DWORD *_pdwStubPhase);


/* [helpstring][id] */ HRESULT STDMETHODCALLTYPE IVSWebReferenceDynamicProperties_SupportsDynamicProperties_Proxy( 
    IVSWebReferenceDynamicProperties * This,
    /* [retval][out] */ VARIANT_BOOL *pbSupportsDynamicProperties);


void __RPC_STUB IVSWebReferenceDynamicProperties_SupportsDynamicProperties_Stub(
    IRpcStubBuffer *This,
    IRpcChannelBuffer *_pRpcChannelBuffer,
    PRPC_MESSAGE _pRpcMessage,
    DWORD *_pdwStubPhase);



#endif 	/* __IVSWebReferenceDynamicProperties_INTERFACE_DEFINED__ */


/* interface __MIDL_itf_vslangproj_0137 */
/* [local] */ 

#define SID_SVSWebReferenceDynamicProperties IID_IVSWebReferenceDynamicProperties
#ifdef FORCE_EXPLICIT_DTE_NAMESPACE
#undef DTE
#undef Project
#undef ProjectItem
#endif


extern RPC_IF_HANDLE __MIDL_itf_vslangproj_0137_v0_0_c_ifspec;
extern RPC_IF_HANDLE __MIDL_itf_vslangproj_0137_v0_0_s_ifspec;

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


