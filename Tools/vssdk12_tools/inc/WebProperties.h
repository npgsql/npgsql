

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


#ifndef __webproperties_h__
#define __webproperties_h__

#if defined(_MSC_VER) && (_MSC_VER >= 1020)
#pragma once
#endif

/* Forward Declarations */ 

#ifndef __WebSiteProperties_FWD_DEFINED__
#define __WebSiteProperties_FWD_DEFINED__
typedef interface WebSiteProperties WebSiteProperties;

#endif 	/* __WebSiteProperties_FWD_DEFINED__ */


#ifndef __WebSiteProperties2_FWD_DEFINED__
#define __WebSiteProperties2_FWD_DEFINED__
typedef interface WebSiteProperties2 WebSiteProperties2;

#endif 	/* __WebSiteProperties2_FWD_DEFINED__ */


#ifndef __WebFileProperties_FWD_DEFINED__
#define __WebFileProperties_FWD_DEFINED__
typedef interface WebFileProperties WebFileProperties;

#endif 	/* __WebFileProperties_FWD_DEFINED__ */


#ifndef __WebFolderProperties_FWD_DEFINED__
#define __WebFolderProperties_FWD_DEFINED__
typedef interface WebFolderProperties WebFolderProperties;

#endif 	/* __WebFolderProperties_FWD_DEFINED__ */


#ifndef __WebService_FWD_DEFINED__
#define __WebService_FWD_DEFINED__
typedef interface WebService WebService;

#endif 	/* __WebService_FWD_DEFINED__ */


#ifndef __WebServices_FWD_DEFINED__
#define __WebServices_FWD_DEFINED__
typedef interface WebServices WebServices;

#endif 	/* __WebServices_FWD_DEFINED__ */


#ifndef __WebReference_FWD_DEFINED__
#define __WebReference_FWD_DEFINED__
typedef interface WebReference WebReference;

#endif 	/* __WebReference_FWD_DEFINED__ */


#ifndef __WebReferences_FWD_DEFINED__
#define __WebReferences_FWD_DEFINED__
typedef interface WebReferences WebReferences;

#endif 	/* __WebReferences_FWD_DEFINED__ */


#ifndef ___WebReferencesEvents_FWD_DEFINED__
#define ___WebReferencesEvents_FWD_DEFINED__
typedef interface _WebReferencesEvents _WebReferencesEvents;

#endif 	/* ___WebReferencesEvents_FWD_DEFINED__ */


#ifndef ___dispWebReferencesEvents_FWD_DEFINED__
#define ___dispWebReferencesEvents_FWD_DEFINED__
typedef interface _dispWebReferencesEvents _dispWebReferencesEvents;

#endif 	/* ___dispWebReferencesEvents_FWD_DEFINED__ */


#ifndef __WebReferencesEvents_FWD_DEFINED__
#define __WebReferencesEvents_FWD_DEFINED__

#ifdef __cplusplus
typedef class WebReferencesEvents WebReferencesEvents;
#else
typedef struct WebReferencesEvents WebReferencesEvents;
#endif /* __cplusplus */

#endif 	/* __WebReferencesEvents_FWD_DEFINED__ */


#ifndef __AssemblyReference_FWD_DEFINED__
#define __AssemblyReference_FWD_DEFINED__
typedef interface AssemblyReference AssemblyReference;

#endif 	/* __AssemblyReference_FWD_DEFINED__ */


#ifndef __AssemblyReferences_FWD_DEFINED__
#define __AssemblyReferences_FWD_DEFINED__
typedef interface AssemblyReferences AssemblyReferences;

#endif 	/* __AssemblyReferences_FWD_DEFINED__ */


#ifndef ___AssemblyReferencesEvents_FWD_DEFINED__
#define ___AssemblyReferencesEvents_FWD_DEFINED__
typedef interface _AssemblyReferencesEvents _AssemblyReferencesEvents;

#endif 	/* ___AssemblyReferencesEvents_FWD_DEFINED__ */


#ifndef ___dispAssemblyReferencesEvents_FWD_DEFINED__
#define ___dispAssemblyReferencesEvents_FWD_DEFINED__
typedef interface _dispAssemblyReferencesEvents _dispAssemblyReferencesEvents;

#endif 	/* ___dispAssemblyReferencesEvents_FWD_DEFINED__ */


#ifndef __AssemblyReferencesEvents_FWD_DEFINED__
#define __AssemblyReferencesEvents_FWD_DEFINED__

#ifdef __cplusplus
typedef class AssemblyReferencesEvents AssemblyReferencesEvents;
#else
typedef struct AssemblyReferencesEvents AssemblyReferencesEvents;
#endif /* __cplusplus */

#endif 	/* __AssemblyReferencesEvents_FWD_DEFINED__ */


#ifndef ___WebServicesEvents_FWD_DEFINED__
#define ___WebServicesEvents_FWD_DEFINED__
typedef interface _WebServicesEvents _WebServicesEvents;

#endif 	/* ___WebServicesEvents_FWD_DEFINED__ */


#ifndef ___dispWebServicesEvents_FWD_DEFINED__
#define ___dispWebServicesEvents_FWD_DEFINED__
typedef interface _dispWebServicesEvents _dispWebServicesEvents;

#endif 	/* ___dispWebServicesEvents_FWD_DEFINED__ */


#ifndef __WebServicesEvents_FWD_DEFINED__
#define __WebServicesEvents_FWD_DEFINED__

#ifdef __cplusplus
typedef class WebServicesEvents WebServicesEvents;
#else
typedef struct WebServicesEvents WebServicesEvents;
#endif /* __cplusplus */

#endif 	/* __WebServicesEvents_FWD_DEFINED__ */


#ifndef ___WebSiteMiscEvents_FWD_DEFINED__
#define ___WebSiteMiscEvents_FWD_DEFINED__
typedef interface _WebSiteMiscEvents _WebSiteMiscEvents;

#endif 	/* ___WebSiteMiscEvents_FWD_DEFINED__ */


#ifndef ___dispWebSiteMiscEvents_FWD_DEFINED__
#define ___dispWebSiteMiscEvents_FWD_DEFINED__
typedef interface _dispWebSiteMiscEvents _dispWebSiteMiscEvents;

#endif 	/* ___dispWebSiteMiscEvents_FWD_DEFINED__ */


#ifndef __WebSiteMiscEvents_FWD_DEFINED__
#define __WebSiteMiscEvents_FWD_DEFINED__

#ifdef __cplusplus
typedef class WebSiteMiscEvents WebSiteMiscEvents;
#else
typedef struct WebSiteMiscEvents WebSiteMiscEvents;
#endif /* __cplusplus */

#endif 	/* __WebSiteMiscEvents_FWD_DEFINED__ */


#ifndef __VSWebSiteEvents_FWD_DEFINED__
#define __VSWebSiteEvents_FWD_DEFINED__
typedef interface VSWebSiteEvents VSWebSiteEvents;

#endif 	/* __VSWebSiteEvents_FWD_DEFINED__ */


#ifndef __CodeFolder_FWD_DEFINED__
#define __CodeFolder_FWD_DEFINED__
typedef interface CodeFolder CodeFolder;

#endif 	/* __CodeFolder_FWD_DEFINED__ */


#ifndef __CodeFolders_FWD_DEFINED__
#define __CodeFolders_FWD_DEFINED__
typedef interface CodeFolders CodeFolders;

#endif 	/* __CodeFolders_FWD_DEFINED__ */


#ifndef __VSWebSite_FWD_DEFINED__
#define __VSWebSite_FWD_DEFINED__
typedef interface VSWebSite VSWebSite;

#endif 	/* __VSWebSite_FWD_DEFINED__ */


#ifndef __RelatedFiles_FWD_DEFINED__
#define __RelatedFiles_FWD_DEFINED__
typedef interface RelatedFiles RelatedFiles;

#endif 	/* __RelatedFiles_FWD_DEFINED__ */


#ifndef __VSWebProjectItem_FWD_DEFINED__
#define __VSWebProjectItem_FWD_DEFINED__
typedef interface VSWebProjectItem VSWebProjectItem;

#endif 	/* __VSWebProjectItem_FWD_DEFINED__ */


#ifndef __VSWebPackage_FWD_DEFINED__
#define __VSWebPackage_FWD_DEFINED__
typedef interface VSWebPackage VSWebPackage;

#endif 	/* __VSWebPackage_FWD_DEFINED__ */


/* header files for imported files */
#include "oaidl.h"

#ifdef __cplusplus
extern "C"{
#endif 


/* interface __MIDL_itf_WebProperties_0000_0000 */
/* [local] */ 

#include "dte.h"
#ifdef FORCE_EXPLICIT_DTE_NAMESPACE
#define DTE VxDTE::DTE
#define Project VxDTE::Project
#define ProjectItem VxDTE::ProjectItem
#define FileCodeModel VxDTE::FileCodeModel
#define CodeModel VxDTE::CodeModel
#endif
#define VSWEBSITE_VER_MAJ    8
#define VSWEBSITE_VER_MIN    0

enum __MIDL___MIDL_itf_WebProperties_0000_0000_0001
    {
        WEBSITEPROPID__First	= 1000,
        WEBSITEPROPID_OpenedURL	= ( WEBSITEPROPID__First + 1 ) ,
        WEBSITEPROPID_FullPath	= ( WEBSITEPROPID_OpenedURL + 1 ) ,
        WEBSITEPROPID_WebSiteType	= ( WEBSITEPROPID_FullPath + 1 ) ,
        WEBSITEPROPID_CurrentWebsiteLanguage	= ( WEBSITEPROPID_WebSiteType + 1 ) ,
        WEBSITEPROPID_BrowseURL	= ( WEBSITEPROPID_CurrentWebsiteLanguage + 1 ) ,
        WEBSITEPROPID_EnableVsWebServer	= ( WEBSITEPROPID_BrowseURL + 1 ) ,
        WEBSITEPROPID_EnableVsWebServerDynamicPort	= ( WEBSITEPROPID_EnableVsWebServer + 1 ) ,
        WEBSITEPROPID_VsWebServerPort	= ( WEBSITEPROPID_EnableVsWebServerDynamicPort + 1 ) ,
        WEBSITEPROPID_StartArguments	= ( WEBSITEPROPID_VsWebServerPort + 1 ) ,
        WEBSITEPROPID_StartAction	= ( WEBSITEPROPID_StartArguments + 1 ) ,
        WEBSITEPROPID_StartProgram	= ( WEBSITEPROPID_StartAction + 1 ) ,
        WEBSITEPROPID_StartWorkingDirectory	= ( WEBSITEPROPID_StartProgram + 1 ) ,
        WEBSITEPROPID_StartURL	= ( WEBSITEPROPID_StartWorkingDirectory + 1 ) ,
        WEBSITEPROPID_StartPage	= ( WEBSITEPROPID_StartURL + 1 ) ,
        WEBSITEPROPID_EnableASPXDebugging	= ( WEBSITEPROPID_StartPage + 1 ) ,
        WEBSITEPROPID_EnableUnmanagedDebugging	= ( WEBSITEPROPID_EnableASPXDebugging + 1 ) ,
        WEBSITEPROPID_EnableSQLServerDebugging	= ( WEBSITEPROPID_EnableUnmanagedDebugging + 1 ) ,
        WEBSITEPROPID_EnableNTLMAuthentication	= ( WEBSITEPROPID_EnableSQLServerDebugging + 1 ) ,
        WEBSITEPROPID_Extender	= ( WEBSITEPROPID_EnableNTLMAuthentication + 1 ) ,
        WEBSITEPROPID_ExtenderNames	= ( WEBSITEPROPID_Extender + 1 ) ,
        WEBSITEPROPID_ExtenderCATID	= ( WEBSITEPROPID_ExtenderNames + 1 ) ,
        WEBSITEPROPID_EnableFxCop	= ( WEBSITEPROPID_ExtenderCATID + 1 ) ,
        WEBSITEPROPID_FxCopRuleAssemblies	= ( WEBSITEPROPID_EnableFxCop + 1 ) ,
        WEBSITEPROPID_FxCopRules	= ( WEBSITEPROPID_FxCopRuleAssemblies + 1 ) ,
        WEBSITEPROPID_ProjectDirty	= ( WEBSITEPROPID_FxCopRules + 1 ) ,
        WEBSITEPROPID_VsWebServerVPath	= ( WEBSITEPROPID_ProjectDirty + 1 ) 
    } ;

enum __MIDL___MIDL_itf_WebProperties_0000_0000_0002
    {
        WEBFILEPROPID_FileName	= DISPID_VALUE,
        WEBFILEPROPID_FullPath	= ( WEBFILEPROPID_FileName + 1 ) ,
        WEBFILEPROPID_Extension	= ( WEBFILEPROPID_FullPath + 1 ) ,
        WEBFILEPROPID_URL	= ( WEBFILEPROPID_Extension + 1 ) ,
        WEBFILEPROPID_RelativeURL	= ( WEBFILEPROPID_URL + 1 ) ,
        WEBFILEPROPID_Extender	= ( WEBFILEPROPID_RelativeURL + 1 ) ,
        WEBFILEPROPID_ExtenderNames	= ( WEBFILEPROPID_Extender + 1 ) ,
        WEBFILEPROPID_ExtenderCATID	= ( WEBFILEPROPID_ExtenderNames + 1 ) ,
        WEBFILEPROPID_AutoRefreshPath	= ( WEBFILEPROPID_ExtenderCATID + 1 ) 
    } ;

enum __MIDL___MIDL_itf_WebProperties_0000_0000_0003
    {
        WEBFOLDERPROPID_FileName	= DISPID_VALUE,
        WEBFOLDERPROPID_FullPath	= ( WEBFOLDERPROPID_FileName + 1 ) ,
        WEBFOLDERPROPID_URL	= ( WEBFOLDERPROPID_FullPath + 1 ) ,
        WEBFOLDERPROPID_FolderType	= ( WEBFOLDERPROPID_URL + 1 ) ,
        WEBFOLDERPROPID_RelativeURL	= ( WEBFOLDERPROPID_FolderType + 1 ) ,
        WEBFOLDERPROPID_CodeLanguage	= ( WEBFOLDERPROPID_RelativeURL + 1 ) ,
        WEBFOLDERPROPID_MergeInProgress	= ( WEBFOLDERPROPID_CodeLanguage + 1 ) ,
        WEBFOLDERPROPID_Extender	= ( WEBFOLDERPROPID_MergeInProgress + 1 ) ,
        WEBFOLDERPROPID_ExtenderNames	= ( WEBFOLDERPROPID_Extender + 1 ) ,
        WEBFOLDERPROPID_ExtenderCATID	= ( WEBFOLDERPROPID_ExtenderNames + 1 ) 
    } ;

enum __MIDL___MIDL_itf_WebProperties_0000_0000_0004
    {
        DISPID_WebService_URL	= DISPID_VALUE,
        DISPID_WebService_DTE	= ( DISPID_WebService_URL + 1 ) ,
        DISPID_WebService_ContainingProject	= ( DISPID_WebService_DTE + 1 ) ,
        DISPID_WebService_AppRelativeURL	= ( DISPID_WebService_ContainingProject + 1 ) ,
        DISPID_WebService_ClassName	= ( DISPID_WebService_AppRelativeURL + 1 ) ,
        DISPID_WebService_ProjectItem	= ( DISPID_WebService_ClassName + 1 ) ,
        DISPID_WebService_FileCodeModel	= ( DISPID_WebService_ProjectItem + 1 ) ,
        DISPID_WebService_ClassFileItem	= ( DISPID_WebService_FileCodeModel + 1 ) 
    } ;

enum __MIDL___MIDL_itf_WebProperties_0000_0000_0005
    {
        DISPID_WebServices_Item	= DISPID_VALUE,
        DISPID_WebServices_DTE	= ( DISPID_WebServices_Item + 1 ) ,
        DISPID_WebServices_ContainingProject	= ( DISPID_WebServices_DTE + 1 ) ,
        DISPID_WebServices_Count	= ( DISPID_WebServices_ContainingProject + 1 ) 
    } ;

enum __MIDL___MIDL_itf_WebProperties_0000_0000_0006
    {
        DISPID_WebReference_DynamicUrl	= DISPID_VALUE,
        DISPID_WebReference_DTE	= ( DISPID_WebReference_DynamicUrl + 1 ) ,
        DISPID_WebReference_ContainingProject	= ( DISPID_WebReference_DTE + 1 ) ,
        DISPID_WebReference_ProjectItem	= ( DISPID_WebReference_ContainingProject + 1 ) ,
        DISPID_WebReference_Remove	= ( DISPID_WebReference_ProjectItem + 1 ) ,
        DISPID_WebReference_DynamicPropName	= ( DISPID_WebReference_Remove + 1 ) ,
        DISPID_WebReference_Namespace	= ( DISPID_WebReference_DynamicPropName + 1 ) ,
        DISPID_WebReference_ServiceName	= ( DISPID_WebReference_Namespace + 1 ) ,
        DISPID_WebReference_ServiceLocationUrl	= ( DISPID_WebReference_ServiceName + 1 ) ,
        DISPID_WebReference_Update	= ( DISPID_WebReference_ServiceLocationUrl + 1 ) ,
        DISPID_WebReference_FileCodeModel	= ( DISPID_WebReference_Update + 1 ) ,
        DISPID_WebReference_ServiceDefinitionUrl	= ( DISPID_WebReference_FileCodeModel + 1 ) ,
        DISPID_WebReference_Discomap	= ( DISPID_WebReference_ServiceDefinitionUrl + 1 ) ,
        DISPID_WebReference_WsdlAppRelativeUrl	= ( DISPID_WebReference_Discomap + 1 ) 
    } ;

enum __MIDL___MIDL_itf_WebProperties_0000_0000_0007
    {
        DISPID_WebReferences_Item	= DISPID_VALUE,
        DISPID_WebReferences_DTE	= ( DISPID_WebReferences_Item + 1 ) ,
        DISPID_WebReferences_ContainingProject	= ( DISPID_WebReferences_DTE + 1 ) ,
        DISPID_WebReferences_Count	= ( DISPID_WebReferences_ContainingProject + 1 ) ,
        DISPID_WebReferences_Add	= ( DISPID_WebReferences_Count + 1 ) ,
        DISPID_WebReferences_Update	= ( DISPID_WebReferences_Add + 1 ) 
    } ;

enum __MIDL___MIDL_itf_WebProperties_0000_0000_0008
    {
        DISPID_AssemblyReference_DTE	= DISPID_VALUE,
        DISPID_AssemblyReference_ContainingProject	= ( DISPID_AssemblyReference_DTE + 1 ) ,
        DISPID_AssemblyReference_Name	= ( DISPID_AssemblyReference_ContainingProject + 1 ) ,
        DISPID_AssemblyReference_Remove	= ( DISPID_AssemblyReference_Name + 1 ) ,
        DISPID_AssemblyReference_ReferenceKind	= ( DISPID_AssemblyReference_Remove + 1 ) ,
        DISPID_AssemblyReference_FullPath	= ( DISPID_AssemblyReference_ReferenceKind + 1 ) ,
        DISPID_AssemblyReference_StrongName	= ( DISPID_AssemblyReference_FullPath + 1 ) ,
        DISPID_AssemblyReference_ReferencedProject	= ( DISPID_AssemblyReference_StrongName + 1 ) 
    } ;

enum __MIDL___MIDL_itf_WebProperties_0000_0000_0009
    {
        DISPID_AssemblyReferences_DTE	= DISPID_VALUE,
        DISPID_AssemblyReferences_ContainingProject	= ( DISPID_AssemblyReferences_DTE + 1 ) ,
        DISPID_AssemblyReferences_Count	= ( DISPID_AssemblyReferences_ContainingProject + 1 ) ,
        DISPID_AssemblyReferences_Item	= ( DISPID_AssemblyReferences_Count + 1 ) ,
        DISPID_AssemblyReferences_AddFromGac	= ( DISPID_AssemblyReferences_Item + 1 ) ,
        DISPID_AssemblyReferences_AddFromFile	= ( DISPID_AssemblyReferences_AddFromGac + 1 ) ,
        DISPID_AssemblyReferences_AddFromProject	= ( DISPID_AssemblyReferences_AddFromFile + 1 ) 
    } ;

enum __MIDL___MIDL_itf_WebProperties_0000_0000_0010
    {
        DISPID_WebSite_URL	= DISPID_VALUE,
        DISPID_WebSite_DTE	= ( DISPID_WebSite_URL + 1 ) ,
        DISPID_WebSite_Project	= ( DISPID_WebSite_DTE + 1 ) ,
        DISPID_WebSite_VSWebSiteEvents	= ( DISPID_WebSite_Project + 1 ) ,
        DISPID_WebSite_TemplatePath	= ( DISPID_WebSite_VSWebSiteEvents + 1 ) ,
        DISPID_WebSite_UserTemplatePath	= ( DISPID_WebSite_TemplatePath + 1 ) ,
        DISPID_WebSite_Refresh	= ( DISPID_WebSite_UserTemplatePath + 1 ) ,
        DISPID_WebSite_GetUniqueFilename	= ( DISPID_WebSite_Refresh + 1 ) ,
        DISPID_WebSite_WebReferences	= ( DISPID_WebSite_GetUniqueFilename + 1 ) ,
        DISPID_WebSite_WebServices	= ( DISPID_WebSite_WebReferences + 1 ) ,
        DISPID_WebSite_References	= ( DISPID_WebSite_WebServices + 1 ) ,
        DISPID_WebSite_CodeFolders	= ( DISPID_WebSite_References + 1 ) ,
        DISPID_WebSite_EnsureServerRunning	= ( DISPID_WebSite_CodeFolders + 1 ) ,
        DISPID_WebSite_AddFromTemplate	= ( DISPID_WebSite_EnsureServerRunning + 1 ) ,
        DISPID_WebSite_PreCompileWeb	= ( DISPID_WebSite_AddFromTemplate + 1 ) ,
        DISPID_WebSite_WaitUntilReady	= ( DISPID_WebSite_PreCompileWeb + 1 ) 
    } ;

enum __MIDL___MIDL_itf_WebProperties_0000_0000_0011
    {
        DISPID_WebProjectItem_DTE	= 1,
        DISPID_WebProjectItem_ProjectItem	= ( DISPID_WebProjectItem_DTE + 1 ) ,
        DISPID_WebProjectItem_ContainingProject	= ( DISPID_WebProjectItem_ProjectItem + 1 ) ,
        DISPID_WebProjectItem_UpdateLocalCopy	= ( DISPID_WebProjectItem_ContainingProject + 1 ) ,
        DISPID_WebProjectItem_UpdateRemoteCopy	= ( DISPID_WebProjectItem_UpdateLocalCopy + 1 ) ,
        DISPID_WebProjectItem_WaitUntilReady	= ( DISPID_WebProjectItem_UpdateRemoteCopy + 1 ) ,
        DISPID_WebProjectItem_Load	= ( DISPID_WebProjectItem_WaitUntilReady + 1 ) ,
        DISPID_WebProjectItem_Unload	= ( DISPID_WebProjectItem_Load + 1 ) ,
        DISPID_RelatedFiles	= ( DISPID_WebProjectItem_Unload + 1 ) 
    } ;

enum __MIDL___MIDL_itf_WebProperties_0000_0000_0012
    {
        DISPID_RelatedFiles_Count	= DISPID_VALUE,
        DISPID_RelatedFiles_Item	= ( DISPID_RelatedFiles_Count + 1 ) 
    } ;

enum __MIDL___MIDL_itf_WebProperties_0000_0000_0013
    {
        DISPID_CodeFolders_DTE	= 1,
        DISPID_CodeFolders_ContainingProject	= ( DISPID_CodeFolders_DTE + 1 ) ,
        DISPID_CodeFolders_Count	= ( DISPID_CodeFolders_ContainingProject + 1 ) ,
        DISPID_CodeFolders_Item	= ( DISPID_CodeFolders_Count + 1 ) ,
        DISPID_CodeFolders_Add	= ( DISPID_CodeFolders_Item + 1 ) 
    } ;

enum __MIDL___MIDL_itf_WebProperties_0000_0000_0014
    {
        DISPID_CodeFolder_DTE	= 1,
        DISPID_CodeFolder_ProjectItem	= ( DISPID_CodeFolder_DTE + 1 ) ,
        DISPID_CodeFolder_ContainingProject	= ( DISPID_CodeFolder_ProjectItem + 1 ) ,
        DISPID_CodeFolder_Language	= ( DISPID_CodeFolder_ContainingProject + 1 ) ,
        DISPID_CodeFolder_FolderPath	= ( DISPID_CodeFolder_Language + 1 ) ,
        DISPID_CodeFolder_CodeModel	= ( DISPID_CodeFolder_FolderPath + 1 ) ,
        DISPID_CodeFolder_Remove	= ( DISPID_CodeFolder_CodeModel + 1 ) 
    } ;

enum __MIDL___MIDL_itf_WebProperties_0000_0000_0015
    {
        DISPID_WebPackage_DTE	= 1,
        DISPID_WebPackage_OpenWebSite	= ( DISPID_WebPackage_DTE + 1 ) 
    } ;


extern RPC_IF_HANDLE __MIDL_itf_WebProperties_0000_0000_v0_0_c_ifspec;
extern RPC_IF_HANDLE __MIDL_itf_WebProperties_0000_0000_v0_0_s_ifspec;


#ifndef __VSWebSite_LIBRARY_DEFINED__
#define __VSWebSite_LIBRARY_DEFINED__

/* library VSWebSite */
/* [version][helpstring][uuid] */ 

// CATID's for automation extension of extensibility objects 
DEFINE_GUID(CATID_ExtPrj,            0x40c5fc1d, 0x46ab, 0x4d0b, 0xb3, 0x40, 0xd0, 0xa5, 0x7b, 0xd2, 0xb8, 0xd2);
DEFINE_GUID(CATID_ExtPrjItem,        0xd1836bac, 0x7a8d, 0x4f7c, 0xaa, 0xce, 0xf2, 0xd2, 0x17, 0x35, 0x56, 0x4e);
// CATID's for automation extension of browse objects
DEFINE_GUID(CATID_WebSiteProps, 0xeef81a81, 0xd390, 0x4725, 0xb1, 0x6d, 0xe1, 0x3, 0xe0, 0xf9, 0x67, 0xb4);
DEFINE_GUID(CATID_WebFileProps, 0xe231573c, 0xc018, 0x4768, 0xa3, 0x83, 0x18, 0xb7, 0x3f, 0x26, 0x7e, 0x71);
DEFINE_GUID(CATID_WebFolderProps, 0xa7f7e5e, 0x5d30, 0x41dd, 0x90, 0x65, 0x1a, 0x29, 0x4d, 0x73, 0x6f, 0x44);
extern const __declspec(selectany) GUID UICONTEXT_WebPackage = {0x39c9c826, 0x8ef8, 0x4079, { 0x8c, 0x95, 0x42, 0x8f, 0x5b, 0x1c, 0x32, 0x3f}};
// Enum values of project properties
typedef /* [uuid] */  DECLSPEC_UUID("557C19E5-8624-47fe-8080-C37F77AC4A47") 
enum webStartAction
    {
        webStartActionCurrentPage	= 0,
        webStartActionSpecificPage	= ( webStartActionCurrentPage + 1 ) ,
        webStartActionProgram	= ( webStartActionSpecificPage + 1 ) ,
        webStartActionURL	= ( webStartActionProgram + 1 ) ,
        webStartActionNoStartPage	= ( webStartActionURL + 1 ) 
    } 	webStartAction;

#define webStartActionMin  webStartActionCurrentPage
#define webStartActionMax  webStartActionNoStartPage
typedef /* [uuid] */  DECLSPEC_UUID("787E1222-5DEB-461d-8234-8E9ABE7F4515") 
enum webType
    {
        webTypeFileSystem	= 0,
        webTypeLocalhost	= ( webTypeFileSystem + 1 ) ,
        webTypeFTP	= ( webTypeLocalhost + 1 ) ,
        webTypeFrontPage	= ( webTypeFTP + 1 ) 
    } 	webType;

#define webTypeMin webTypeFileSystem
#define webTypeMax webTypeFrontPage
typedef /* [uuid] */  DECLSPEC_UUID("623F3A65-DF2F-448d-9AB6-C61AC1F3D088") 
enum webFolderType
    {
        webFolderTypeNormal	= 0,
        webFolderTypeBin	= ( webFolderTypeNormal + 1 ) ,
        webFolderTypeCode	= ( webFolderTypeBin + 1 ) ,
        webFolderTypeThemes	= ( webFolderTypeCode + 1 ) ,
        webFolderTypeGlobalResources	= ( webFolderTypeThemes + 1 ) ,
        webFolderTypeLocalResources	= ( webFolderTypeGlobalResources + 1 ) ,
        webFolderTypeData	= ( webFolderTypeLocalResources + 1 ) ,
        webFolderTypeBrowsers	= ( webFolderTypeData + 1 ) ,
        webFolderTypeWebReferences	= ( webFolderTypeBrowsers + 1 ) 
    } 	webFolderType;

#define webFolderTypeMin webFolderTypeNormal
#define webFolderTypeMax webFolderTypeWebReferences
typedef /* [uuid] */  DECLSPEC_UUID("0F2BB482-5EDE-45a8-979D-13700E2B2520") 
enum AssemblyReferenceType
    {
        AssemblyReferenceBin	= 0,
        AssemblyReferenceConfig	= ( AssemblyReferenceBin + 1 ) ,
        AssemblyReferenceClientProject	= ( AssemblyReferenceConfig + 1 ) 
    } 	AssemblyReferenceType;

#define AssemblyReferenceTypeMin AssemblyReferenceBin
#define AssemblyReferenceTypeMax AssemblyReferenceClientProject
#define SID_SVSWebProjectItem IID_ProjectItem
typedef /* [uuid] */  DECLSPEC_UUID("63183147-9B1D-45a2-BA93-405EF1A5A8D4") 
enum OpenWebsiteOptions
    {
        OpenWebsiteOption_None	= 0,
        OpenWebsiteOption_UsePassiveFTP	= 1,
        OpenWebsiteOption_UseFrontpageForLocalhost	= 2
    } 	OpenWebsiteOptions;


EXTERN_C const IID LIBID_VSWebSite;


#ifndef __PrjKind_MODULE_DEFINED__
#define __PrjKind_MODULE_DEFINED__


/* module PrjKind */
/* [uuid] */ 

/* [helpstring] */ const LPWSTR prjKindVenusProject	=	L"{E24C65DC-7377-472b-9ABA-BC803B73C61A}";

#endif /* __PrjKind_MODULE_DEFINED__ */


#ifndef __PrjCATID_MODULE_DEFINED__
#define __PrjCATID_MODULE_DEFINED__


/* module PrjCATID */
/* [uuid] */ 

/* [helpstring] */ const LPWSTR prjCATIDWebProject	=	L"{40C5FC1D-46AB-4d0b-B340-D0A57BD2B8D2}";

/* [helpstring] */ const LPWSTR prjCATIDWebProjectItem	=	L"{D1836BAC-7A8D-4f7c-AACE-F2D21735564E}";

#endif /* __PrjCATID_MODULE_DEFINED__ */


#ifndef __PrjBrowseObjectCATID_MODULE_DEFINED__
#define __PrjBrowseObjectCATID_MODULE_DEFINED__


/* module PrjBrowseObjectCATID */
/* [uuid] */ 

/* [helpstring] */ const LPWSTR prjCATIDWebSiteProjectBrowseObject	=	L"{EEF81A81-D390-4725-B16D-E103E0F967B4}";

/* [helpstring] */ const LPWSTR prjCATIDWebFileBrowseObject	=	L"{E231573C-C018-4768-A383-18B73F267E71}";

/* [helpstring] */ const LPWSTR prjCATIDWebFolderBrowseObject	=	L"{0A7F7E5E-5D30-41DD-9065-1A294D736F44}";

#endif /* __PrjBrowseObjectCATID_MODULE_DEFINED__ */

#ifndef __WebSiteProperties_INTERFACE_DEFINED__
#define __WebSiteProperties_INTERFACE_DEFINED__

/* interface WebSiteProperties */
/* [object][dual][unique][helpstring][uuid] */ 


EXTERN_C const IID IID_WebSiteProperties;

#if defined(__cplusplus) && !defined(CINTERFACE)
    
    MIDL_INTERFACE("477BFD8A-5FD5-434f-981B-2FD3C145B473")
    WebSiteProperties : public IDispatch
    {
    public:
        virtual /* [hidden][helpstring][id][propget] */ HRESULT STDMETHODCALLTYPE get___id( 
            /* [retval][out] */ __RPC__deref_out_opt BSTR *pbstrName) = 0;
        
        virtual /* [helpstring][id][propget] */ HRESULT STDMETHODCALLTYPE get_OpenedURL( 
            /* [retval][out] */ __RPC__deref_out_opt BSTR *pbstrURL) = 0;
        
        virtual /* [helpstring][id][propget] */ HRESULT STDMETHODCALLTYPE get_FullPath( 
            /* [retval][out] */ __RPC__deref_out_opt BSTR *pbstrFullPath) = 0;
        
        virtual /* [helpstring][id][propget] */ HRESULT STDMETHODCALLTYPE get_BrowseURL( 
            /* [retval][out] */ __RPC__deref_out_opt BSTR *pbstrBrowseUrl) = 0;
        
        virtual /* [helpstring][id][propput] */ HRESULT STDMETHODCALLTYPE put_BrowseURL( 
            /* [in] */ __RPC__in BSTR bstrBrowseURL) = 0;
        
        virtual /* [helpstring][id][propget] */ HRESULT STDMETHODCALLTYPE get_EnableVsWebServer( 
            /* [retval][out] */ __RPC__out VARIANT_BOOL *pbEnableVsWebServer) = 0;
        
        virtual /* [helpstring][id][propput] */ HRESULT STDMETHODCALLTYPE put_EnableVsWebServer( 
            /* [in] */ VARIANT_BOOL bEnableVsWebServer) = 0;
        
        virtual /* [helpstring][id][propget] */ HRESULT STDMETHODCALLTYPE get_WebSiteType( 
            /* [retval][out] */ __RPC__out webType *pWebType) = 0;
        
        virtual /* [helpstring][id][propget] */ HRESULT STDMETHODCALLTYPE get_CurrentWebsiteLanguage( 
            /* [retval][out] */ __RPC__deref_out_opt BSTR *pbstrLanguage) = 0;
        
        virtual /* [helpstring][id][propput] */ HRESULT STDMETHODCALLTYPE put_CurrentWebsiteLanguage( 
            /* [in] */ __RPC__in BSTR pbstrLanguage) = 0;
        
        virtual /* [helpstring][id][propget] */ HRESULT STDMETHODCALLTYPE get_StartProgram( 
            /* [retval][out] */ __RPC__deref_out_opt BSTR *pbstrStartProgram) = 0;
        
        virtual /* [helpstring][id][propput] */ HRESULT STDMETHODCALLTYPE put_StartProgram( 
            /* [in] */ __RPC__in BSTR bstrStartProgram) = 0;
        
        virtual /* [helpstring][id][propget] */ HRESULT STDMETHODCALLTYPE get_StartWorkingDirectory( 
            /* [retval][out] */ __RPC__deref_out_opt BSTR *pbstrStartWorkingDirectory) = 0;
        
        virtual /* [helpstring][id][propput] */ HRESULT STDMETHODCALLTYPE put_StartWorkingDirectory( 
            /* [in] */ __RPC__in BSTR bstrStartWorkingDirectory) = 0;
        
        virtual /* [helpstring][id][propget] */ HRESULT STDMETHODCALLTYPE get_StartURL( 
            /* [retval][out] */ __RPC__deref_out_opt BSTR *pbstrStartURL) = 0;
        
        virtual /* [helpstring][id][propput] */ HRESULT STDMETHODCALLTYPE put_StartURL( 
            /* [in] */ __RPC__in BSTR bstrStartURL) = 0;
        
        virtual /* [helpstring][id][propget] */ HRESULT STDMETHODCALLTYPE get_StartPage( 
            /* [retval][out] */ __RPC__deref_out_opt BSTR *pbstrStartPage) = 0;
        
        virtual /* [helpstring][id][propput] */ HRESULT STDMETHODCALLTYPE put_StartPage( 
            /* [in] */ __RPC__in BSTR bstrStartPage) = 0;
        
        virtual /* [helpstring][id][propget] */ HRESULT STDMETHODCALLTYPE get_StartArguments( 
            /* [retval][out] */ __RPC__deref_out_opt BSTR *pbstrStartArguments) = 0;
        
        virtual /* [helpstring][id][propput] */ HRESULT STDMETHODCALLTYPE put_StartArguments( 
            /* [in] */ __RPC__in BSTR bstrStartArguments) = 0;
        
        virtual /* [helpstring][id][propget] */ HRESULT STDMETHODCALLTYPE get_EnableASPXDebugging( 
            /* [retval][out] */ __RPC__out VARIANT_BOOL *pbEnableASPXDebugging) = 0;
        
        virtual /* [helpstring][id][propput] */ HRESULT STDMETHODCALLTYPE put_EnableASPXDebugging( 
            /* [in] */ VARIANT_BOOL bEnableASPXDebugging) = 0;
        
        virtual /* [helpstring][id][propget] */ HRESULT STDMETHODCALLTYPE get_EnableUnmanagedDebugging( 
            /* [retval][out] */ __RPC__out VARIANT_BOOL *pbEnableUnmanagedDebugging) = 0;
        
        virtual /* [helpstring][id][propput] */ HRESULT STDMETHODCALLTYPE put_EnableUnmanagedDebugging( 
            /* [in] */ VARIANT_BOOL bEnableUnmanagedDebugging) = 0;
        
        virtual /* [helpstring][id][propget] */ HRESULT STDMETHODCALLTYPE get_StartAction( 
            /* [retval][out] */ __RPC__out webStartAction *pdebugStartMode) = 0;
        
        virtual /* [helpstring][id][propput] */ HRESULT STDMETHODCALLTYPE put_StartAction( 
            /* [in] */ webStartAction debugStartMode) = 0;
        
        virtual /* [helpstring][id][propget] */ HRESULT STDMETHODCALLTYPE get_EnableSQLServerDebugging( 
            /* [retval][out] */ __RPC__out VARIANT_BOOL *pbEnableSQLServerDebugging) = 0;
        
        virtual /* [helpstring][id][propput] */ HRESULT STDMETHODCALLTYPE put_EnableSQLServerDebugging( 
            /* [in] */ VARIANT_BOOL bEnableSQLServerDebugging) = 0;
        
        virtual /* [hidden][helpstring][nonbrowsable][id][propget] */ HRESULT STDMETHODCALLTYPE get_Extender( 
            /* [in] */ __RPC__in BSTR ExtenderName,
            /* [retval][out] */ __RPC__deref_out_opt IDispatch **Extender) = 0;
        
        virtual /* [hidden][helpstring][nonbrowsable][id][propget] */ HRESULT STDMETHODCALLTYPE get_ExtenderNames( 
            /* [retval][out] */ __RPC__out VARIANT *ExtenderNames) = 0;
        
        virtual /* [hidden][helpstring][nonbrowsable][id][propget] */ HRESULT STDMETHODCALLTYPE get_ExtenderCATID( 
            /* [retval][out] */ __RPC__deref_out_opt BSTR *pRetval) = 0;
        
        virtual /* [helpstring][id][propget] */ HRESULT STDMETHODCALLTYPE get_EnableFxCop( 
            /* [retval][out] */ __RPC__out VARIANT_BOOL *pbEnableFxCop) = 0;
        
        virtual /* [helpstring][id][propput] */ HRESULT STDMETHODCALLTYPE put_EnableFxCop( 
            /* [in] */ VARIANT_BOOL bEnableFxCop) = 0;
        
        virtual /* [helpstring][id][propget] */ HRESULT STDMETHODCALLTYPE get_FxCopRuleAssemblies( 
            /* [retval][out] */ __RPC__deref_out_opt BSTR *pbstrFxCopRuleAssemblies) = 0;
        
        virtual /* [helpstring][id][propput] */ HRESULT STDMETHODCALLTYPE put_FxCopRuleAssemblies( 
            /* [in] */ __RPC__in BSTR bstrFxCopRuleAssemblies) = 0;
        
        virtual /* [helpstring][id][propget] */ HRESULT STDMETHODCALLTYPE get_FxCopRules( 
            /* [retval][out] */ __RPC__deref_out_opt BSTR *pbstrFxCopRules) = 0;
        
        virtual /* [helpstring][id][propput] */ HRESULT STDMETHODCALLTYPE put_FxCopRules( 
            /* [in] */ __RPC__in BSTR bstrFxCopRules) = 0;
        
        virtual /* [helpstring][id][propget] */ HRESULT STDMETHODCALLTYPE get_EnableVsWebServerDynamicPort( 
            /* [retval][out] */ __RPC__out VARIANT_BOOL *pbEnableDynamicPort) = 0;
        
        virtual /* [helpstring][id][propput] */ HRESULT STDMETHODCALLTYPE put_EnableVsWebServerDynamicPort( 
            /* [in] */ VARIANT_BOOL bEnableDynamicPort) = 0;
        
        virtual /* [helpstring][id][propget] */ HRESULT STDMETHODCALLTYPE get_VsWebServerPort( 
            /* [retval][out] */ __RPC__out USHORT *pusPort) = 0;
        
        virtual /* [helpstring][id][propput] */ HRESULT STDMETHODCALLTYPE put_VsWebServerPort( 
            /* [in] */ USHORT usPort) = 0;
        
        virtual /* [helpstring][id][propget] */ HRESULT STDMETHODCALLTYPE get_EnableNTLMAuthentication( 
            /* [retval][out] */ __RPC__out VARIANT_BOOL *pbEnableNTLMAuthentication) = 0;
        
        virtual /* [helpstring][id][propput] */ HRESULT STDMETHODCALLTYPE put_EnableNTLMAuthentication( 
            /* [in] */ VARIANT_BOOL bEnableNTLMAuthentication) = 0;
        
        virtual /* [helpstring][id][propget] */ HRESULT STDMETHODCALLTYPE get_ProjectDirty( 
            /* [retval][out] */ __RPC__out VARIANT_BOOL *pbIsDirty) = 0;
        
    };
    
    
#else 	/* C style interface */

    typedef struct WebSitePropertiesVtbl
    {
        BEGIN_INTERFACE
        
        HRESULT ( STDMETHODCALLTYPE *QueryInterface )( 
            __RPC__in WebSiteProperties * This,
            /* [in] */ __RPC__in REFIID riid,
            /* [annotation][iid_is][out] */ 
            _COM_Outptr_  void **ppvObject);
        
        ULONG ( STDMETHODCALLTYPE *AddRef )( 
            __RPC__in WebSiteProperties * This);
        
        ULONG ( STDMETHODCALLTYPE *Release )( 
            __RPC__in WebSiteProperties * This);
        
        HRESULT ( STDMETHODCALLTYPE *GetTypeInfoCount )( 
            __RPC__in WebSiteProperties * This,
            /* [out] */ __RPC__out UINT *pctinfo);
        
        HRESULT ( STDMETHODCALLTYPE *GetTypeInfo )( 
            __RPC__in WebSiteProperties * This,
            /* [in] */ UINT iTInfo,
            /* [in] */ LCID lcid,
            /* [out] */ __RPC__deref_out_opt ITypeInfo **ppTInfo);
        
        HRESULT ( STDMETHODCALLTYPE *GetIDsOfNames )( 
            __RPC__in WebSiteProperties * This,
            /* [in] */ __RPC__in REFIID riid,
            /* [size_is][in] */ __RPC__in_ecount_full(cNames) LPOLESTR *rgszNames,
            /* [range][in] */ __RPC__in_range(0,16384) UINT cNames,
            /* [in] */ LCID lcid,
            /* [size_is][out] */ __RPC__out_ecount_full(cNames) DISPID *rgDispId);
        
        /* [local] */ HRESULT ( STDMETHODCALLTYPE *Invoke )( 
            WebSiteProperties * This,
            /* [annotation][in] */ 
            _In_  DISPID dispIdMember,
            /* [annotation][in] */ 
            _In_  REFIID riid,
            /* [annotation][in] */ 
            _In_  LCID lcid,
            /* [annotation][in] */ 
            _In_  WORD wFlags,
            /* [annotation][out][in] */ 
            _In_  DISPPARAMS *pDispParams,
            /* [annotation][out] */ 
            _Out_opt_  VARIANT *pVarResult,
            /* [annotation][out] */ 
            _Out_opt_  EXCEPINFO *pExcepInfo,
            /* [annotation][out] */ 
            _Out_opt_  UINT *puArgErr);
        
        /* [hidden][helpstring][id][propget] */ HRESULT ( STDMETHODCALLTYPE *get___id )( 
            __RPC__in WebSiteProperties * This,
            /* [retval][out] */ __RPC__deref_out_opt BSTR *pbstrName);
        
        /* [helpstring][id][propget] */ HRESULT ( STDMETHODCALLTYPE *get_OpenedURL )( 
            __RPC__in WebSiteProperties * This,
            /* [retval][out] */ __RPC__deref_out_opt BSTR *pbstrURL);
        
        /* [helpstring][id][propget] */ HRESULT ( STDMETHODCALLTYPE *get_FullPath )( 
            __RPC__in WebSiteProperties * This,
            /* [retval][out] */ __RPC__deref_out_opt BSTR *pbstrFullPath);
        
        /* [helpstring][id][propget] */ HRESULT ( STDMETHODCALLTYPE *get_BrowseURL )( 
            __RPC__in WebSiteProperties * This,
            /* [retval][out] */ __RPC__deref_out_opt BSTR *pbstrBrowseUrl);
        
        /* [helpstring][id][propput] */ HRESULT ( STDMETHODCALLTYPE *put_BrowseURL )( 
            __RPC__in WebSiteProperties * This,
            /* [in] */ __RPC__in BSTR bstrBrowseURL);
        
        /* [helpstring][id][propget] */ HRESULT ( STDMETHODCALLTYPE *get_EnableVsWebServer )( 
            __RPC__in WebSiteProperties * This,
            /* [retval][out] */ __RPC__out VARIANT_BOOL *pbEnableVsWebServer);
        
        /* [helpstring][id][propput] */ HRESULT ( STDMETHODCALLTYPE *put_EnableVsWebServer )( 
            __RPC__in WebSiteProperties * This,
            /* [in] */ VARIANT_BOOL bEnableVsWebServer);
        
        /* [helpstring][id][propget] */ HRESULT ( STDMETHODCALLTYPE *get_WebSiteType )( 
            __RPC__in WebSiteProperties * This,
            /* [retval][out] */ __RPC__out webType *pWebType);
        
        /* [helpstring][id][propget] */ HRESULT ( STDMETHODCALLTYPE *get_CurrentWebsiteLanguage )( 
            __RPC__in WebSiteProperties * This,
            /* [retval][out] */ __RPC__deref_out_opt BSTR *pbstrLanguage);
        
        /* [helpstring][id][propput] */ HRESULT ( STDMETHODCALLTYPE *put_CurrentWebsiteLanguage )( 
            __RPC__in WebSiteProperties * This,
            /* [in] */ __RPC__in BSTR pbstrLanguage);
        
        /* [helpstring][id][propget] */ HRESULT ( STDMETHODCALLTYPE *get_StartProgram )( 
            __RPC__in WebSiteProperties * This,
            /* [retval][out] */ __RPC__deref_out_opt BSTR *pbstrStartProgram);
        
        /* [helpstring][id][propput] */ HRESULT ( STDMETHODCALLTYPE *put_StartProgram )( 
            __RPC__in WebSiteProperties * This,
            /* [in] */ __RPC__in BSTR bstrStartProgram);
        
        /* [helpstring][id][propget] */ HRESULT ( STDMETHODCALLTYPE *get_StartWorkingDirectory )( 
            __RPC__in WebSiteProperties * This,
            /* [retval][out] */ __RPC__deref_out_opt BSTR *pbstrStartWorkingDirectory);
        
        /* [helpstring][id][propput] */ HRESULT ( STDMETHODCALLTYPE *put_StartWorkingDirectory )( 
            __RPC__in WebSiteProperties * This,
            /* [in] */ __RPC__in BSTR bstrStartWorkingDirectory);
        
        /* [helpstring][id][propget] */ HRESULT ( STDMETHODCALLTYPE *get_StartURL )( 
            __RPC__in WebSiteProperties * This,
            /* [retval][out] */ __RPC__deref_out_opt BSTR *pbstrStartURL);
        
        /* [helpstring][id][propput] */ HRESULT ( STDMETHODCALLTYPE *put_StartURL )( 
            __RPC__in WebSiteProperties * This,
            /* [in] */ __RPC__in BSTR bstrStartURL);
        
        /* [helpstring][id][propget] */ HRESULT ( STDMETHODCALLTYPE *get_StartPage )( 
            __RPC__in WebSiteProperties * This,
            /* [retval][out] */ __RPC__deref_out_opt BSTR *pbstrStartPage);
        
        /* [helpstring][id][propput] */ HRESULT ( STDMETHODCALLTYPE *put_StartPage )( 
            __RPC__in WebSiteProperties * This,
            /* [in] */ __RPC__in BSTR bstrStartPage);
        
        /* [helpstring][id][propget] */ HRESULT ( STDMETHODCALLTYPE *get_StartArguments )( 
            __RPC__in WebSiteProperties * This,
            /* [retval][out] */ __RPC__deref_out_opt BSTR *pbstrStartArguments);
        
        /* [helpstring][id][propput] */ HRESULT ( STDMETHODCALLTYPE *put_StartArguments )( 
            __RPC__in WebSiteProperties * This,
            /* [in] */ __RPC__in BSTR bstrStartArguments);
        
        /* [helpstring][id][propget] */ HRESULT ( STDMETHODCALLTYPE *get_EnableASPXDebugging )( 
            __RPC__in WebSiteProperties * This,
            /* [retval][out] */ __RPC__out VARIANT_BOOL *pbEnableASPXDebugging);
        
        /* [helpstring][id][propput] */ HRESULT ( STDMETHODCALLTYPE *put_EnableASPXDebugging )( 
            __RPC__in WebSiteProperties * This,
            /* [in] */ VARIANT_BOOL bEnableASPXDebugging);
        
        /* [helpstring][id][propget] */ HRESULT ( STDMETHODCALLTYPE *get_EnableUnmanagedDebugging )( 
            __RPC__in WebSiteProperties * This,
            /* [retval][out] */ __RPC__out VARIANT_BOOL *pbEnableUnmanagedDebugging);
        
        /* [helpstring][id][propput] */ HRESULT ( STDMETHODCALLTYPE *put_EnableUnmanagedDebugging )( 
            __RPC__in WebSiteProperties * This,
            /* [in] */ VARIANT_BOOL bEnableUnmanagedDebugging);
        
        /* [helpstring][id][propget] */ HRESULT ( STDMETHODCALLTYPE *get_StartAction )( 
            __RPC__in WebSiteProperties * This,
            /* [retval][out] */ __RPC__out webStartAction *pdebugStartMode);
        
        /* [helpstring][id][propput] */ HRESULT ( STDMETHODCALLTYPE *put_StartAction )( 
            __RPC__in WebSiteProperties * This,
            /* [in] */ webStartAction debugStartMode);
        
        /* [helpstring][id][propget] */ HRESULT ( STDMETHODCALLTYPE *get_EnableSQLServerDebugging )( 
            __RPC__in WebSiteProperties * This,
            /* [retval][out] */ __RPC__out VARIANT_BOOL *pbEnableSQLServerDebugging);
        
        /* [helpstring][id][propput] */ HRESULT ( STDMETHODCALLTYPE *put_EnableSQLServerDebugging )( 
            __RPC__in WebSiteProperties * This,
            /* [in] */ VARIANT_BOOL bEnableSQLServerDebugging);
        
        /* [hidden][helpstring][nonbrowsable][id][propget] */ HRESULT ( STDMETHODCALLTYPE *get_Extender )( 
            __RPC__in WebSiteProperties * This,
            /* [in] */ __RPC__in BSTR ExtenderName,
            /* [retval][out] */ __RPC__deref_out_opt IDispatch **Extender);
        
        /* [hidden][helpstring][nonbrowsable][id][propget] */ HRESULT ( STDMETHODCALLTYPE *get_ExtenderNames )( 
            __RPC__in WebSiteProperties * This,
            /* [retval][out] */ __RPC__out VARIANT *ExtenderNames);
        
        /* [hidden][helpstring][nonbrowsable][id][propget] */ HRESULT ( STDMETHODCALLTYPE *get_ExtenderCATID )( 
            __RPC__in WebSiteProperties * This,
            /* [retval][out] */ __RPC__deref_out_opt BSTR *pRetval);
        
        /* [helpstring][id][propget] */ HRESULT ( STDMETHODCALLTYPE *get_EnableFxCop )( 
            __RPC__in WebSiteProperties * This,
            /* [retval][out] */ __RPC__out VARIANT_BOOL *pbEnableFxCop);
        
        /* [helpstring][id][propput] */ HRESULT ( STDMETHODCALLTYPE *put_EnableFxCop )( 
            __RPC__in WebSiteProperties * This,
            /* [in] */ VARIANT_BOOL bEnableFxCop);
        
        /* [helpstring][id][propget] */ HRESULT ( STDMETHODCALLTYPE *get_FxCopRuleAssemblies )( 
            __RPC__in WebSiteProperties * This,
            /* [retval][out] */ __RPC__deref_out_opt BSTR *pbstrFxCopRuleAssemblies);
        
        /* [helpstring][id][propput] */ HRESULT ( STDMETHODCALLTYPE *put_FxCopRuleAssemblies )( 
            __RPC__in WebSiteProperties * This,
            /* [in] */ __RPC__in BSTR bstrFxCopRuleAssemblies);
        
        /* [helpstring][id][propget] */ HRESULT ( STDMETHODCALLTYPE *get_FxCopRules )( 
            __RPC__in WebSiteProperties * This,
            /* [retval][out] */ __RPC__deref_out_opt BSTR *pbstrFxCopRules);
        
        /* [helpstring][id][propput] */ HRESULT ( STDMETHODCALLTYPE *put_FxCopRules )( 
            __RPC__in WebSiteProperties * This,
            /* [in] */ __RPC__in BSTR bstrFxCopRules);
        
        /* [helpstring][id][propget] */ HRESULT ( STDMETHODCALLTYPE *get_EnableVsWebServerDynamicPort )( 
            __RPC__in WebSiteProperties * This,
            /* [retval][out] */ __RPC__out VARIANT_BOOL *pbEnableDynamicPort);
        
        /* [helpstring][id][propput] */ HRESULT ( STDMETHODCALLTYPE *put_EnableVsWebServerDynamicPort )( 
            __RPC__in WebSiteProperties * This,
            /* [in] */ VARIANT_BOOL bEnableDynamicPort);
        
        /* [helpstring][id][propget] */ HRESULT ( STDMETHODCALLTYPE *get_VsWebServerPort )( 
            __RPC__in WebSiteProperties * This,
            /* [retval][out] */ __RPC__out USHORT *pusPort);
        
        /* [helpstring][id][propput] */ HRESULT ( STDMETHODCALLTYPE *put_VsWebServerPort )( 
            __RPC__in WebSiteProperties * This,
            /* [in] */ USHORT usPort);
        
        /* [helpstring][id][propget] */ HRESULT ( STDMETHODCALLTYPE *get_EnableNTLMAuthentication )( 
            __RPC__in WebSiteProperties * This,
            /* [retval][out] */ __RPC__out VARIANT_BOOL *pbEnableNTLMAuthentication);
        
        /* [helpstring][id][propput] */ HRESULT ( STDMETHODCALLTYPE *put_EnableNTLMAuthentication )( 
            __RPC__in WebSiteProperties * This,
            /* [in] */ VARIANT_BOOL bEnableNTLMAuthentication);
        
        /* [helpstring][id][propget] */ HRESULT ( STDMETHODCALLTYPE *get_ProjectDirty )( 
            __RPC__in WebSiteProperties * This,
            /* [retval][out] */ __RPC__out VARIANT_BOOL *pbIsDirty);
        
        END_INTERFACE
    } WebSitePropertiesVtbl;

    interface WebSiteProperties
    {
        CONST_VTBL struct WebSitePropertiesVtbl *lpVtbl;
    };

    

#ifdef COBJMACROS


#define WebSiteProperties_QueryInterface(This,riid,ppvObject)	\
    ( (This)->lpVtbl -> QueryInterface(This,riid,ppvObject) ) 

#define WebSiteProperties_AddRef(This)	\
    ( (This)->lpVtbl -> AddRef(This) ) 

#define WebSiteProperties_Release(This)	\
    ( (This)->lpVtbl -> Release(This) ) 


#define WebSiteProperties_GetTypeInfoCount(This,pctinfo)	\
    ( (This)->lpVtbl -> GetTypeInfoCount(This,pctinfo) ) 

#define WebSiteProperties_GetTypeInfo(This,iTInfo,lcid,ppTInfo)	\
    ( (This)->lpVtbl -> GetTypeInfo(This,iTInfo,lcid,ppTInfo) ) 

#define WebSiteProperties_GetIDsOfNames(This,riid,rgszNames,cNames,lcid,rgDispId)	\
    ( (This)->lpVtbl -> GetIDsOfNames(This,riid,rgszNames,cNames,lcid,rgDispId) ) 

#define WebSiteProperties_Invoke(This,dispIdMember,riid,lcid,wFlags,pDispParams,pVarResult,pExcepInfo,puArgErr)	\
    ( (This)->lpVtbl -> Invoke(This,dispIdMember,riid,lcid,wFlags,pDispParams,pVarResult,pExcepInfo,puArgErr) ) 


#define WebSiteProperties_get___id(This,pbstrName)	\
    ( (This)->lpVtbl -> get___id(This,pbstrName) ) 

#define WebSiteProperties_get_OpenedURL(This,pbstrURL)	\
    ( (This)->lpVtbl -> get_OpenedURL(This,pbstrURL) ) 

#define WebSiteProperties_get_FullPath(This,pbstrFullPath)	\
    ( (This)->lpVtbl -> get_FullPath(This,pbstrFullPath) ) 

#define WebSiteProperties_get_BrowseURL(This,pbstrBrowseUrl)	\
    ( (This)->lpVtbl -> get_BrowseURL(This,pbstrBrowseUrl) ) 

#define WebSiteProperties_put_BrowseURL(This,bstrBrowseURL)	\
    ( (This)->lpVtbl -> put_BrowseURL(This,bstrBrowseURL) ) 

#define WebSiteProperties_get_EnableVsWebServer(This,pbEnableVsWebServer)	\
    ( (This)->lpVtbl -> get_EnableVsWebServer(This,pbEnableVsWebServer) ) 

#define WebSiteProperties_put_EnableVsWebServer(This,bEnableVsWebServer)	\
    ( (This)->lpVtbl -> put_EnableVsWebServer(This,bEnableVsWebServer) ) 

#define WebSiteProperties_get_WebSiteType(This,pWebType)	\
    ( (This)->lpVtbl -> get_WebSiteType(This,pWebType) ) 

#define WebSiteProperties_get_CurrentWebsiteLanguage(This,pbstrLanguage)	\
    ( (This)->lpVtbl -> get_CurrentWebsiteLanguage(This,pbstrLanguage) ) 

#define WebSiteProperties_put_CurrentWebsiteLanguage(This,pbstrLanguage)	\
    ( (This)->lpVtbl -> put_CurrentWebsiteLanguage(This,pbstrLanguage) ) 

#define WebSiteProperties_get_StartProgram(This,pbstrStartProgram)	\
    ( (This)->lpVtbl -> get_StartProgram(This,pbstrStartProgram) ) 

#define WebSiteProperties_put_StartProgram(This,bstrStartProgram)	\
    ( (This)->lpVtbl -> put_StartProgram(This,bstrStartProgram) ) 

#define WebSiteProperties_get_StartWorkingDirectory(This,pbstrStartWorkingDirectory)	\
    ( (This)->lpVtbl -> get_StartWorkingDirectory(This,pbstrStartWorkingDirectory) ) 

#define WebSiteProperties_put_StartWorkingDirectory(This,bstrStartWorkingDirectory)	\
    ( (This)->lpVtbl -> put_StartWorkingDirectory(This,bstrStartWorkingDirectory) ) 

#define WebSiteProperties_get_StartURL(This,pbstrStartURL)	\
    ( (This)->lpVtbl -> get_StartURL(This,pbstrStartURL) ) 

#define WebSiteProperties_put_StartURL(This,bstrStartURL)	\
    ( (This)->lpVtbl -> put_StartURL(This,bstrStartURL) ) 

#define WebSiteProperties_get_StartPage(This,pbstrStartPage)	\
    ( (This)->lpVtbl -> get_StartPage(This,pbstrStartPage) ) 

#define WebSiteProperties_put_StartPage(This,bstrStartPage)	\
    ( (This)->lpVtbl -> put_StartPage(This,bstrStartPage) ) 

#define WebSiteProperties_get_StartArguments(This,pbstrStartArguments)	\
    ( (This)->lpVtbl -> get_StartArguments(This,pbstrStartArguments) ) 

#define WebSiteProperties_put_StartArguments(This,bstrStartArguments)	\
    ( (This)->lpVtbl -> put_StartArguments(This,bstrStartArguments) ) 

#define WebSiteProperties_get_EnableASPXDebugging(This,pbEnableASPXDebugging)	\
    ( (This)->lpVtbl -> get_EnableASPXDebugging(This,pbEnableASPXDebugging) ) 

#define WebSiteProperties_put_EnableASPXDebugging(This,bEnableASPXDebugging)	\
    ( (This)->lpVtbl -> put_EnableASPXDebugging(This,bEnableASPXDebugging) ) 

#define WebSiteProperties_get_EnableUnmanagedDebugging(This,pbEnableUnmanagedDebugging)	\
    ( (This)->lpVtbl -> get_EnableUnmanagedDebugging(This,pbEnableUnmanagedDebugging) ) 

#define WebSiteProperties_put_EnableUnmanagedDebugging(This,bEnableUnmanagedDebugging)	\
    ( (This)->lpVtbl -> put_EnableUnmanagedDebugging(This,bEnableUnmanagedDebugging) ) 

#define WebSiteProperties_get_StartAction(This,pdebugStartMode)	\
    ( (This)->lpVtbl -> get_StartAction(This,pdebugStartMode) ) 

#define WebSiteProperties_put_StartAction(This,debugStartMode)	\
    ( (This)->lpVtbl -> put_StartAction(This,debugStartMode) ) 

#define WebSiteProperties_get_EnableSQLServerDebugging(This,pbEnableSQLServerDebugging)	\
    ( (This)->lpVtbl -> get_EnableSQLServerDebugging(This,pbEnableSQLServerDebugging) ) 

#define WebSiteProperties_put_EnableSQLServerDebugging(This,bEnableSQLServerDebugging)	\
    ( (This)->lpVtbl -> put_EnableSQLServerDebugging(This,bEnableSQLServerDebugging) ) 

#define WebSiteProperties_get_Extender(This,ExtenderName,Extender)	\
    ( (This)->lpVtbl -> get_Extender(This,ExtenderName,Extender) ) 

#define WebSiteProperties_get_ExtenderNames(This,ExtenderNames)	\
    ( (This)->lpVtbl -> get_ExtenderNames(This,ExtenderNames) ) 

#define WebSiteProperties_get_ExtenderCATID(This,pRetval)	\
    ( (This)->lpVtbl -> get_ExtenderCATID(This,pRetval) ) 

#define WebSiteProperties_get_EnableFxCop(This,pbEnableFxCop)	\
    ( (This)->lpVtbl -> get_EnableFxCop(This,pbEnableFxCop) ) 

#define WebSiteProperties_put_EnableFxCop(This,bEnableFxCop)	\
    ( (This)->lpVtbl -> put_EnableFxCop(This,bEnableFxCop) ) 

#define WebSiteProperties_get_FxCopRuleAssemblies(This,pbstrFxCopRuleAssemblies)	\
    ( (This)->lpVtbl -> get_FxCopRuleAssemblies(This,pbstrFxCopRuleAssemblies) ) 

#define WebSiteProperties_put_FxCopRuleAssemblies(This,bstrFxCopRuleAssemblies)	\
    ( (This)->lpVtbl -> put_FxCopRuleAssemblies(This,bstrFxCopRuleAssemblies) ) 

#define WebSiteProperties_get_FxCopRules(This,pbstrFxCopRules)	\
    ( (This)->lpVtbl -> get_FxCopRules(This,pbstrFxCopRules) ) 

#define WebSiteProperties_put_FxCopRules(This,bstrFxCopRules)	\
    ( (This)->lpVtbl -> put_FxCopRules(This,bstrFxCopRules) ) 

#define WebSiteProperties_get_EnableVsWebServerDynamicPort(This,pbEnableDynamicPort)	\
    ( (This)->lpVtbl -> get_EnableVsWebServerDynamicPort(This,pbEnableDynamicPort) ) 

#define WebSiteProperties_put_EnableVsWebServerDynamicPort(This,bEnableDynamicPort)	\
    ( (This)->lpVtbl -> put_EnableVsWebServerDynamicPort(This,bEnableDynamicPort) ) 

#define WebSiteProperties_get_VsWebServerPort(This,pusPort)	\
    ( (This)->lpVtbl -> get_VsWebServerPort(This,pusPort) ) 

#define WebSiteProperties_put_VsWebServerPort(This,usPort)	\
    ( (This)->lpVtbl -> put_VsWebServerPort(This,usPort) ) 

#define WebSiteProperties_get_EnableNTLMAuthentication(This,pbEnableNTLMAuthentication)	\
    ( (This)->lpVtbl -> get_EnableNTLMAuthentication(This,pbEnableNTLMAuthentication) ) 

#define WebSiteProperties_put_EnableNTLMAuthentication(This,bEnableNTLMAuthentication)	\
    ( (This)->lpVtbl -> put_EnableNTLMAuthentication(This,bEnableNTLMAuthentication) ) 

#define WebSiteProperties_get_ProjectDirty(This,pbIsDirty)	\
    ( (This)->lpVtbl -> get_ProjectDirty(This,pbIsDirty) ) 

#endif /* COBJMACROS */


#endif 	/* C style interface */




#endif 	/* __WebSiteProperties_INTERFACE_DEFINED__ */


#ifndef __WebSiteProperties2_INTERFACE_DEFINED__
#define __WebSiteProperties2_INTERFACE_DEFINED__

/* interface WebSiteProperties2 */
/* [object][dual][unique][helpstring][uuid] */ 


EXTERN_C const IID IID_WebSiteProperties2;

#if defined(__cplusplus) && !defined(CINTERFACE)
    
    MIDL_INTERFACE("0F4EDFB6-C797-42fb-A7E0-0C93D6F4FB6B")
    WebSiteProperties2 : public WebSiteProperties
    {
    public:
        virtual /* [helpstring][id][propget] */ HRESULT STDMETHODCALLTYPE get_VsWebServerVPath( 
            /* [retval][out] */ __RPC__deref_out_opt BSTR *pbstrVPath) = 0;
        
        virtual /* [helpstring][id][propput] */ HRESULT STDMETHODCALLTYPE put_VsWebServerVPath( 
            /* [in] */ __RPC__in BSTR bstrVPath) = 0;
        
    };
    
    
#else 	/* C style interface */

    typedef struct WebSiteProperties2Vtbl
    {
        BEGIN_INTERFACE
        
        HRESULT ( STDMETHODCALLTYPE *QueryInterface )( 
            __RPC__in WebSiteProperties2 * This,
            /* [in] */ __RPC__in REFIID riid,
            /* [annotation][iid_is][out] */ 
            _COM_Outptr_  void **ppvObject);
        
        ULONG ( STDMETHODCALLTYPE *AddRef )( 
            __RPC__in WebSiteProperties2 * This);
        
        ULONG ( STDMETHODCALLTYPE *Release )( 
            __RPC__in WebSiteProperties2 * This);
        
        HRESULT ( STDMETHODCALLTYPE *GetTypeInfoCount )( 
            __RPC__in WebSiteProperties2 * This,
            /* [out] */ __RPC__out UINT *pctinfo);
        
        HRESULT ( STDMETHODCALLTYPE *GetTypeInfo )( 
            __RPC__in WebSiteProperties2 * This,
            /* [in] */ UINT iTInfo,
            /* [in] */ LCID lcid,
            /* [out] */ __RPC__deref_out_opt ITypeInfo **ppTInfo);
        
        HRESULT ( STDMETHODCALLTYPE *GetIDsOfNames )( 
            __RPC__in WebSiteProperties2 * This,
            /* [in] */ __RPC__in REFIID riid,
            /* [size_is][in] */ __RPC__in_ecount_full(cNames) LPOLESTR *rgszNames,
            /* [range][in] */ __RPC__in_range(0,16384) UINT cNames,
            /* [in] */ LCID lcid,
            /* [size_is][out] */ __RPC__out_ecount_full(cNames) DISPID *rgDispId);
        
        /* [local] */ HRESULT ( STDMETHODCALLTYPE *Invoke )( 
            WebSiteProperties2 * This,
            /* [annotation][in] */ 
            _In_  DISPID dispIdMember,
            /* [annotation][in] */ 
            _In_  REFIID riid,
            /* [annotation][in] */ 
            _In_  LCID lcid,
            /* [annotation][in] */ 
            _In_  WORD wFlags,
            /* [annotation][out][in] */ 
            _In_  DISPPARAMS *pDispParams,
            /* [annotation][out] */ 
            _Out_opt_  VARIANT *pVarResult,
            /* [annotation][out] */ 
            _Out_opt_  EXCEPINFO *pExcepInfo,
            /* [annotation][out] */ 
            _Out_opt_  UINT *puArgErr);
        
        /* [hidden][helpstring][id][propget] */ HRESULT ( STDMETHODCALLTYPE *get___id )( 
            __RPC__in WebSiteProperties2 * This,
            /* [retval][out] */ __RPC__deref_out_opt BSTR *pbstrName);
        
        /* [helpstring][id][propget] */ HRESULT ( STDMETHODCALLTYPE *get_OpenedURL )( 
            __RPC__in WebSiteProperties2 * This,
            /* [retval][out] */ __RPC__deref_out_opt BSTR *pbstrURL);
        
        /* [helpstring][id][propget] */ HRESULT ( STDMETHODCALLTYPE *get_FullPath )( 
            __RPC__in WebSiteProperties2 * This,
            /* [retval][out] */ __RPC__deref_out_opt BSTR *pbstrFullPath);
        
        /* [helpstring][id][propget] */ HRESULT ( STDMETHODCALLTYPE *get_BrowseURL )( 
            __RPC__in WebSiteProperties2 * This,
            /* [retval][out] */ __RPC__deref_out_opt BSTR *pbstrBrowseUrl);
        
        /* [helpstring][id][propput] */ HRESULT ( STDMETHODCALLTYPE *put_BrowseURL )( 
            __RPC__in WebSiteProperties2 * This,
            /* [in] */ __RPC__in BSTR bstrBrowseURL);
        
        /* [helpstring][id][propget] */ HRESULT ( STDMETHODCALLTYPE *get_EnableVsWebServer )( 
            __RPC__in WebSiteProperties2 * This,
            /* [retval][out] */ __RPC__out VARIANT_BOOL *pbEnableVsWebServer);
        
        /* [helpstring][id][propput] */ HRESULT ( STDMETHODCALLTYPE *put_EnableVsWebServer )( 
            __RPC__in WebSiteProperties2 * This,
            /* [in] */ VARIANT_BOOL bEnableVsWebServer);
        
        /* [helpstring][id][propget] */ HRESULT ( STDMETHODCALLTYPE *get_WebSiteType )( 
            __RPC__in WebSiteProperties2 * This,
            /* [retval][out] */ __RPC__out webType *pWebType);
        
        /* [helpstring][id][propget] */ HRESULT ( STDMETHODCALLTYPE *get_CurrentWebsiteLanguage )( 
            __RPC__in WebSiteProperties2 * This,
            /* [retval][out] */ __RPC__deref_out_opt BSTR *pbstrLanguage);
        
        /* [helpstring][id][propput] */ HRESULT ( STDMETHODCALLTYPE *put_CurrentWebsiteLanguage )( 
            __RPC__in WebSiteProperties2 * This,
            /* [in] */ __RPC__in BSTR pbstrLanguage);
        
        /* [helpstring][id][propget] */ HRESULT ( STDMETHODCALLTYPE *get_StartProgram )( 
            __RPC__in WebSiteProperties2 * This,
            /* [retval][out] */ __RPC__deref_out_opt BSTR *pbstrStartProgram);
        
        /* [helpstring][id][propput] */ HRESULT ( STDMETHODCALLTYPE *put_StartProgram )( 
            __RPC__in WebSiteProperties2 * This,
            /* [in] */ __RPC__in BSTR bstrStartProgram);
        
        /* [helpstring][id][propget] */ HRESULT ( STDMETHODCALLTYPE *get_StartWorkingDirectory )( 
            __RPC__in WebSiteProperties2 * This,
            /* [retval][out] */ __RPC__deref_out_opt BSTR *pbstrStartWorkingDirectory);
        
        /* [helpstring][id][propput] */ HRESULT ( STDMETHODCALLTYPE *put_StartWorkingDirectory )( 
            __RPC__in WebSiteProperties2 * This,
            /* [in] */ __RPC__in BSTR bstrStartWorkingDirectory);
        
        /* [helpstring][id][propget] */ HRESULT ( STDMETHODCALLTYPE *get_StartURL )( 
            __RPC__in WebSiteProperties2 * This,
            /* [retval][out] */ __RPC__deref_out_opt BSTR *pbstrStartURL);
        
        /* [helpstring][id][propput] */ HRESULT ( STDMETHODCALLTYPE *put_StartURL )( 
            __RPC__in WebSiteProperties2 * This,
            /* [in] */ __RPC__in BSTR bstrStartURL);
        
        /* [helpstring][id][propget] */ HRESULT ( STDMETHODCALLTYPE *get_StartPage )( 
            __RPC__in WebSiteProperties2 * This,
            /* [retval][out] */ __RPC__deref_out_opt BSTR *pbstrStartPage);
        
        /* [helpstring][id][propput] */ HRESULT ( STDMETHODCALLTYPE *put_StartPage )( 
            __RPC__in WebSiteProperties2 * This,
            /* [in] */ __RPC__in BSTR bstrStartPage);
        
        /* [helpstring][id][propget] */ HRESULT ( STDMETHODCALLTYPE *get_StartArguments )( 
            __RPC__in WebSiteProperties2 * This,
            /* [retval][out] */ __RPC__deref_out_opt BSTR *pbstrStartArguments);
        
        /* [helpstring][id][propput] */ HRESULT ( STDMETHODCALLTYPE *put_StartArguments )( 
            __RPC__in WebSiteProperties2 * This,
            /* [in] */ __RPC__in BSTR bstrStartArguments);
        
        /* [helpstring][id][propget] */ HRESULT ( STDMETHODCALLTYPE *get_EnableASPXDebugging )( 
            __RPC__in WebSiteProperties2 * This,
            /* [retval][out] */ __RPC__out VARIANT_BOOL *pbEnableASPXDebugging);
        
        /* [helpstring][id][propput] */ HRESULT ( STDMETHODCALLTYPE *put_EnableASPXDebugging )( 
            __RPC__in WebSiteProperties2 * This,
            /* [in] */ VARIANT_BOOL bEnableASPXDebugging);
        
        /* [helpstring][id][propget] */ HRESULT ( STDMETHODCALLTYPE *get_EnableUnmanagedDebugging )( 
            __RPC__in WebSiteProperties2 * This,
            /* [retval][out] */ __RPC__out VARIANT_BOOL *pbEnableUnmanagedDebugging);
        
        /* [helpstring][id][propput] */ HRESULT ( STDMETHODCALLTYPE *put_EnableUnmanagedDebugging )( 
            __RPC__in WebSiteProperties2 * This,
            /* [in] */ VARIANT_BOOL bEnableUnmanagedDebugging);
        
        /* [helpstring][id][propget] */ HRESULT ( STDMETHODCALLTYPE *get_StartAction )( 
            __RPC__in WebSiteProperties2 * This,
            /* [retval][out] */ __RPC__out webStartAction *pdebugStartMode);
        
        /* [helpstring][id][propput] */ HRESULT ( STDMETHODCALLTYPE *put_StartAction )( 
            __RPC__in WebSiteProperties2 * This,
            /* [in] */ webStartAction debugStartMode);
        
        /* [helpstring][id][propget] */ HRESULT ( STDMETHODCALLTYPE *get_EnableSQLServerDebugging )( 
            __RPC__in WebSiteProperties2 * This,
            /* [retval][out] */ __RPC__out VARIANT_BOOL *pbEnableSQLServerDebugging);
        
        /* [helpstring][id][propput] */ HRESULT ( STDMETHODCALLTYPE *put_EnableSQLServerDebugging )( 
            __RPC__in WebSiteProperties2 * This,
            /* [in] */ VARIANT_BOOL bEnableSQLServerDebugging);
        
        /* [hidden][helpstring][nonbrowsable][id][propget] */ HRESULT ( STDMETHODCALLTYPE *get_Extender )( 
            __RPC__in WebSiteProperties2 * This,
            /* [in] */ __RPC__in BSTR ExtenderName,
            /* [retval][out] */ __RPC__deref_out_opt IDispatch **Extender);
        
        /* [hidden][helpstring][nonbrowsable][id][propget] */ HRESULT ( STDMETHODCALLTYPE *get_ExtenderNames )( 
            __RPC__in WebSiteProperties2 * This,
            /* [retval][out] */ __RPC__out VARIANT *ExtenderNames);
        
        /* [hidden][helpstring][nonbrowsable][id][propget] */ HRESULT ( STDMETHODCALLTYPE *get_ExtenderCATID )( 
            __RPC__in WebSiteProperties2 * This,
            /* [retval][out] */ __RPC__deref_out_opt BSTR *pRetval);
        
        /* [helpstring][id][propget] */ HRESULT ( STDMETHODCALLTYPE *get_EnableFxCop )( 
            __RPC__in WebSiteProperties2 * This,
            /* [retval][out] */ __RPC__out VARIANT_BOOL *pbEnableFxCop);
        
        /* [helpstring][id][propput] */ HRESULT ( STDMETHODCALLTYPE *put_EnableFxCop )( 
            __RPC__in WebSiteProperties2 * This,
            /* [in] */ VARIANT_BOOL bEnableFxCop);
        
        /* [helpstring][id][propget] */ HRESULT ( STDMETHODCALLTYPE *get_FxCopRuleAssemblies )( 
            __RPC__in WebSiteProperties2 * This,
            /* [retval][out] */ __RPC__deref_out_opt BSTR *pbstrFxCopRuleAssemblies);
        
        /* [helpstring][id][propput] */ HRESULT ( STDMETHODCALLTYPE *put_FxCopRuleAssemblies )( 
            __RPC__in WebSiteProperties2 * This,
            /* [in] */ __RPC__in BSTR bstrFxCopRuleAssemblies);
        
        /* [helpstring][id][propget] */ HRESULT ( STDMETHODCALLTYPE *get_FxCopRules )( 
            __RPC__in WebSiteProperties2 * This,
            /* [retval][out] */ __RPC__deref_out_opt BSTR *pbstrFxCopRules);
        
        /* [helpstring][id][propput] */ HRESULT ( STDMETHODCALLTYPE *put_FxCopRules )( 
            __RPC__in WebSiteProperties2 * This,
            /* [in] */ __RPC__in BSTR bstrFxCopRules);
        
        /* [helpstring][id][propget] */ HRESULT ( STDMETHODCALLTYPE *get_EnableVsWebServerDynamicPort )( 
            __RPC__in WebSiteProperties2 * This,
            /* [retval][out] */ __RPC__out VARIANT_BOOL *pbEnableDynamicPort);
        
        /* [helpstring][id][propput] */ HRESULT ( STDMETHODCALLTYPE *put_EnableVsWebServerDynamicPort )( 
            __RPC__in WebSiteProperties2 * This,
            /* [in] */ VARIANT_BOOL bEnableDynamicPort);
        
        /* [helpstring][id][propget] */ HRESULT ( STDMETHODCALLTYPE *get_VsWebServerPort )( 
            __RPC__in WebSiteProperties2 * This,
            /* [retval][out] */ __RPC__out USHORT *pusPort);
        
        /* [helpstring][id][propput] */ HRESULT ( STDMETHODCALLTYPE *put_VsWebServerPort )( 
            __RPC__in WebSiteProperties2 * This,
            /* [in] */ USHORT usPort);
        
        /* [helpstring][id][propget] */ HRESULT ( STDMETHODCALLTYPE *get_EnableNTLMAuthentication )( 
            __RPC__in WebSiteProperties2 * This,
            /* [retval][out] */ __RPC__out VARIANT_BOOL *pbEnableNTLMAuthentication);
        
        /* [helpstring][id][propput] */ HRESULT ( STDMETHODCALLTYPE *put_EnableNTLMAuthentication )( 
            __RPC__in WebSiteProperties2 * This,
            /* [in] */ VARIANT_BOOL bEnableNTLMAuthentication);
        
        /* [helpstring][id][propget] */ HRESULT ( STDMETHODCALLTYPE *get_ProjectDirty )( 
            __RPC__in WebSiteProperties2 * This,
            /* [retval][out] */ __RPC__out VARIANT_BOOL *pbIsDirty);
        
        /* [helpstring][id][propget] */ HRESULT ( STDMETHODCALLTYPE *get_VsWebServerVPath )( 
            __RPC__in WebSiteProperties2 * This,
            /* [retval][out] */ __RPC__deref_out_opt BSTR *pbstrVPath);
        
        /* [helpstring][id][propput] */ HRESULT ( STDMETHODCALLTYPE *put_VsWebServerVPath )( 
            __RPC__in WebSiteProperties2 * This,
            /* [in] */ __RPC__in BSTR bstrVPath);
        
        END_INTERFACE
    } WebSiteProperties2Vtbl;

    interface WebSiteProperties2
    {
        CONST_VTBL struct WebSiteProperties2Vtbl *lpVtbl;
    };

    

#ifdef COBJMACROS


#define WebSiteProperties2_QueryInterface(This,riid,ppvObject)	\
    ( (This)->lpVtbl -> QueryInterface(This,riid,ppvObject) ) 

#define WebSiteProperties2_AddRef(This)	\
    ( (This)->lpVtbl -> AddRef(This) ) 

#define WebSiteProperties2_Release(This)	\
    ( (This)->lpVtbl -> Release(This) ) 


#define WebSiteProperties2_GetTypeInfoCount(This,pctinfo)	\
    ( (This)->lpVtbl -> GetTypeInfoCount(This,pctinfo) ) 

#define WebSiteProperties2_GetTypeInfo(This,iTInfo,lcid,ppTInfo)	\
    ( (This)->lpVtbl -> GetTypeInfo(This,iTInfo,lcid,ppTInfo) ) 

#define WebSiteProperties2_GetIDsOfNames(This,riid,rgszNames,cNames,lcid,rgDispId)	\
    ( (This)->lpVtbl -> GetIDsOfNames(This,riid,rgszNames,cNames,lcid,rgDispId) ) 

#define WebSiteProperties2_Invoke(This,dispIdMember,riid,lcid,wFlags,pDispParams,pVarResult,pExcepInfo,puArgErr)	\
    ( (This)->lpVtbl -> Invoke(This,dispIdMember,riid,lcid,wFlags,pDispParams,pVarResult,pExcepInfo,puArgErr) ) 


#define WebSiteProperties2_get___id(This,pbstrName)	\
    ( (This)->lpVtbl -> get___id(This,pbstrName) ) 

#define WebSiteProperties2_get_OpenedURL(This,pbstrURL)	\
    ( (This)->lpVtbl -> get_OpenedURL(This,pbstrURL) ) 

#define WebSiteProperties2_get_FullPath(This,pbstrFullPath)	\
    ( (This)->lpVtbl -> get_FullPath(This,pbstrFullPath) ) 

#define WebSiteProperties2_get_BrowseURL(This,pbstrBrowseUrl)	\
    ( (This)->lpVtbl -> get_BrowseURL(This,pbstrBrowseUrl) ) 

#define WebSiteProperties2_put_BrowseURL(This,bstrBrowseURL)	\
    ( (This)->lpVtbl -> put_BrowseURL(This,bstrBrowseURL) ) 

#define WebSiteProperties2_get_EnableVsWebServer(This,pbEnableVsWebServer)	\
    ( (This)->lpVtbl -> get_EnableVsWebServer(This,pbEnableVsWebServer) ) 

#define WebSiteProperties2_put_EnableVsWebServer(This,bEnableVsWebServer)	\
    ( (This)->lpVtbl -> put_EnableVsWebServer(This,bEnableVsWebServer) ) 

#define WebSiteProperties2_get_WebSiteType(This,pWebType)	\
    ( (This)->lpVtbl -> get_WebSiteType(This,pWebType) ) 

#define WebSiteProperties2_get_CurrentWebsiteLanguage(This,pbstrLanguage)	\
    ( (This)->lpVtbl -> get_CurrentWebsiteLanguage(This,pbstrLanguage) ) 

#define WebSiteProperties2_put_CurrentWebsiteLanguage(This,pbstrLanguage)	\
    ( (This)->lpVtbl -> put_CurrentWebsiteLanguage(This,pbstrLanguage) ) 

#define WebSiteProperties2_get_StartProgram(This,pbstrStartProgram)	\
    ( (This)->lpVtbl -> get_StartProgram(This,pbstrStartProgram) ) 

#define WebSiteProperties2_put_StartProgram(This,bstrStartProgram)	\
    ( (This)->lpVtbl -> put_StartProgram(This,bstrStartProgram) ) 

#define WebSiteProperties2_get_StartWorkingDirectory(This,pbstrStartWorkingDirectory)	\
    ( (This)->lpVtbl -> get_StartWorkingDirectory(This,pbstrStartWorkingDirectory) ) 

#define WebSiteProperties2_put_StartWorkingDirectory(This,bstrStartWorkingDirectory)	\
    ( (This)->lpVtbl -> put_StartWorkingDirectory(This,bstrStartWorkingDirectory) ) 

#define WebSiteProperties2_get_StartURL(This,pbstrStartURL)	\
    ( (This)->lpVtbl -> get_StartURL(This,pbstrStartURL) ) 

#define WebSiteProperties2_put_StartURL(This,bstrStartURL)	\
    ( (This)->lpVtbl -> put_StartURL(This,bstrStartURL) ) 

#define WebSiteProperties2_get_StartPage(This,pbstrStartPage)	\
    ( (This)->lpVtbl -> get_StartPage(This,pbstrStartPage) ) 

#define WebSiteProperties2_put_StartPage(This,bstrStartPage)	\
    ( (This)->lpVtbl -> put_StartPage(This,bstrStartPage) ) 

#define WebSiteProperties2_get_StartArguments(This,pbstrStartArguments)	\
    ( (This)->lpVtbl -> get_StartArguments(This,pbstrStartArguments) ) 

#define WebSiteProperties2_put_StartArguments(This,bstrStartArguments)	\
    ( (This)->lpVtbl -> put_StartArguments(This,bstrStartArguments) ) 

#define WebSiteProperties2_get_EnableASPXDebugging(This,pbEnableASPXDebugging)	\
    ( (This)->lpVtbl -> get_EnableASPXDebugging(This,pbEnableASPXDebugging) ) 

#define WebSiteProperties2_put_EnableASPXDebugging(This,bEnableASPXDebugging)	\
    ( (This)->lpVtbl -> put_EnableASPXDebugging(This,bEnableASPXDebugging) ) 

#define WebSiteProperties2_get_EnableUnmanagedDebugging(This,pbEnableUnmanagedDebugging)	\
    ( (This)->lpVtbl -> get_EnableUnmanagedDebugging(This,pbEnableUnmanagedDebugging) ) 

#define WebSiteProperties2_put_EnableUnmanagedDebugging(This,bEnableUnmanagedDebugging)	\
    ( (This)->lpVtbl -> put_EnableUnmanagedDebugging(This,bEnableUnmanagedDebugging) ) 

#define WebSiteProperties2_get_StartAction(This,pdebugStartMode)	\
    ( (This)->lpVtbl -> get_StartAction(This,pdebugStartMode) ) 

#define WebSiteProperties2_put_StartAction(This,debugStartMode)	\
    ( (This)->lpVtbl -> put_StartAction(This,debugStartMode) ) 

#define WebSiteProperties2_get_EnableSQLServerDebugging(This,pbEnableSQLServerDebugging)	\
    ( (This)->lpVtbl -> get_EnableSQLServerDebugging(This,pbEnableSQLServerDebugging) ) 

#define WebSiteProperties2_put_EnableSQLServerDebugging(This,bEnableSQLServerDebugging)	\
    ( (This)->lpVtbl -> put_EnableSQLServerDebugging(This,bEnableSQLServerDebugging) ) 

#define WebSiteProperties2_get_Extender(This,ExtenderName,Extender)	\
    ( (This)->lpVtbl -> get_Extender(This,ExtenderName,Extender) ) 

#define WebSiteProperties2_get_ExtenderNames(This,ExtenderNames)	\
    ( (This)->lpVtbl -> get_ExtenderNames(This,ExtenderNames) ) 

#define WebSiteProperties2_get_ExtenderCATID(This,pRetval)	\
    ( (This)->lpVtbl -> get_ExtenderCATID(This,pRetval) ) 

#define WebSiteProperties2_get_EnableFxCop(This,pbEnableFxCop)	\
    ( (This)->lpVtbl -> get_EnableFxCop(This,pbEnableFxCop) ) 

#define WebSiteProperties2_put_EnableFxCop(This,bEnableFxCop)	\
    ( (This)->lpVtbl -> put_EnableFxCop(This,bEnableFxCop) ) 

#define WebSiteProperties2_get_FxCopRuleAssemblies(This,pbstrFxCopRuleAssemblies)	\
    ( (This)->lpVtbl -> get_FxCopRuleAssemblies(This,pbstrFxCopRuleAssemblies) ) 

#define WebSiteProperties2_put_FxCopRuleAssemblies(This,bstrFxCopRuleAssemblies)	\
    ( (This)->lpVtbl -> put_FxCopRuleAssemblies(This,bstrFxCopRuleAssemblies) ) 

#define WebSiteProperties2_get_FxCopRules(This,pbstrFxCopRules)	\
    ( (This)->lpVtbl -> get_FxCopRules(This,pbstrFxCopRules) ) 

#define WebSiteProperties2_put_FxCopRules(This,bstrFxCopRules)	\
    ( (This)->lpVtbl -> put_FxCopRules(This,bstrFxCopRules) ) 

#define WebSiteProperties2_get_EnableVsWebServerDynamicPort(This,pbEnableDynamicPort)	\
    ( (This)->lpVtbl -> get_EnableVsWebServerDynamicPort(This,pbEnableDynamicPort) ) 

#define WebSiteProperties2_put_EnableVsWebServerDynamicPort(This,bEnableDynamicPort)	\
    ( (This)->lpVtbl -> put_EnableVsWebServerDynamicPort(This,bEnableDynamicPort) ) 

#define WebSiteProperties2_get_VsWebServerPort(This,pusPort)	\
    ( (This)->lpVtbl -> get_VsWebServerPort(This,pusPort) ) 

#define WebSiteProperties2_put_VsWebServerPort(This,usPort)	\
    ( (This)->lpVtbl -> put_VsWebServerPort(This,usPort) ) 

#define WebSiteProperties2_get_EnableNTLMAuthentication(This,pbEnableNTLMAuthentication)	\
    ( (This)->lpVtbl -> get_EnableNTLMAuthentication(This,pbEnableNTLMAuthentication) ) 

#define WebSiteProperties2_put_EnableNTLMAuthentication(This,bEnableNTLMAuthentication)	\
    ( (This)->lpVtbl -> put_EnableNTLMAuthentication(This,bEnableNTLMAuthentication) ) 

#define WebSiteProperties2_get_ProjectDirty(This,pbIsDirty)	\
    ( (This)->lpVtbl -> get_ProjectDirty(This,pbIsDirty) ) 


#define WebSiteProperties2_get_VsWebServerVPath(This,pbstrVPath)	\
    ( (This)->lpVtbl -> get_VsWebServerVPath(This,pbstrVPath) ) 

#define WebSiteProperties2_put_VsWebServerVPath(This,bstrVPath)	\
    ( (This)->lpVtbl -> put_VsWebServerVPath(This,bstrVPath) ) 

#endif /* COBJMACROS */


#endif 	/* C style interface */




#endif 	/* __WebSiteProperties2_INTERFACE_DEFINED__ */


#ifndef __WebFileProperties_INTERFACE_DEFINED__
#define __WebFileProperties_INTERFACE_DEFINED__

/* interface WebFileProperties */
/* [local][hidden][helpstring][dual][uuid][object] */ 


EXTERN_C const IID IID_WebFileProperties;

#if defined(__cplusplus) && !defined(CINTERFACE)
    
    MIDL_INTERFACE("51B867E4-CD7E-4ff6-93AA-CA151E36351F")
    WebFileProperties : public IDispatch
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
        
        virtual /* [helpstring][id][propget] */ HRESULT STDMETHODCALLTYPE get_FullPath( 
            /* [retval][out] */ BSTR *pbstrFullPath) = 0;
        
        virtual /* [helpstring][id][propget] */ HRESULT STDMETHODCALLTYPE get_URL( 
            /* [retval][out] */ BSTR *pbstrURL) = 0;
        
        virtual /* [hidden][helpstring][nonbrowsable][propget][id] */ HRESULT STDMETHODCALLTYPE get_Extender( 
            /* [in] */ BSTR ExtenderName,
            /* [retval][out] */ IDispatch **Extender) = 0;
        
        virtual /* [hidden][helpstring][nonbrowsable][propget][id] */ HRESULT STDMETHODCALLTYPE get_ExtenderNames( 
            /* [retval][out] */ VARIANT *ExtenderNames) = 0;
        
        virtual /* [hidden][helpstring][nonbrowsable][propget][id] */ HRESULT STDMETHODCALLTYPE get_ExtenderCATID( 
            /* [retval][out] */ BSTR *pRetval) = 0;
        
        virtual /* [helpstring][id][propget] */ HRESULT STDMETHODCALLTYPE get_RelativeURL( 
            /* [retval][out] */ BSTR *pbstrRelativeURL) = 0;
        
        virtual /* [helpstring][id][propget] */ HRESULT STDMETHODCALLTYPE get_AutoRefreshPath( 
            /* [retval][out] */ BSTR *pbstrAutoRefreshPath) = 0;
        
        virtual /* [helpstring][id][propput] */ HRESULT STDMETHODCALLTYPE put_AutoRefreshPath( 
            /* [in] */ BSTR bstrAutoRefreshPath) = 0;
        
    };
    
    
#else 	/* C style interface */

    typedef struct WebFilePropertiesVtbl
    {
        BEGIN_INTERFACE
        
        HRESULT ( STDMETHODCALLTYPE *QueryInterface )( 
            WebFileProperties * This,
            /* [in] */ REFIID riid,
            /* [annotation][iid_is][out] */ 
            _COM_Outptr_  void **ppvObject);
        
        ULONG ( STDMETHODCALLTYPE *AddRef )( 
            WebFileProperties * This);
        
        ULONG ( STDMETHODCALLTYPE *Release )( 
            WebFileProperties * This);
        
        HRESULT ( STDMETHODCALLTYPE *GetTypeInfoCount )( 
            WebFileProperties * This,
            /* [out] */ UINT *pctinfo);
        
        HRESULT ( STDMETHODCALLTYPE *GetTypeInfo )( 
            WebFileProperties * This,
            /* [in] */ UINT iTInfo,
            /* [in] */ LCID lcid,
            /* [out] */ ITypeInfo **ppTInfo);
        
        HRESULT ( STDMETHODCALLTYPE *GetIDsOfNames )( 
            WebFileProperties * This,
            /* [in] */ REFIID riid,
            /* [size_is][in] */ LPOLESTR *rgszNames,
            /* [range][in] */ UINT cNames,
            /* [in] */ LCID lcid,
            /* [size_is][out] */ DISPID *rgDispId);
        
        /* [local] */ HRESULT ( STDMETHODCALLTYPE *Invoke )( 
            WebFileProperties * This,
            /* [annotation][in] */ 
            _In_  DISPID dispIdMember,
            /* [annotation][in] */ 
            _In_  REFIID riid,
            /* [annotation][in] */ 
            _In_  LCID lcid,
            /* [annotation][in] */ 
            _In_  WORD wFlags,
            /* [annotation][out][in] */ 
            _In_  DISPPARAMS *pDispParams,
            /* [annotation][out] */ 
            _Out_opt_  VARIANT *pVarResult,
            /* [annotation][out] */ 
            _Out_opt_  EXCEPINFO *pExcepInfo,
            /* [annotation][out] */ 
            _Out_opt_  UINT *puArgErr);
        
        /* [hidden][helpstring][id][propget] */ HRESULT ( STDMETHODCALLTYPE *get___id )( 
            WebFileProperties * This,
            /* [retval][out] */ BSTR *pbstrName);
        
        /* [helpstring][id][propget] */ HRESULT ( STDMETHODCALLTYPE *get_FileName )( 
            WebFileProperties * This,
            /* [retval][out] */ BSTR *pbstrName);
        
        /* [helpstring][id][propput] */ HRESULT ( STDMETHODCALLTYPE *put_FileName )( 
            WebFileProperties * This,
            /* [in] */ BSTR bstrName);
        
        /* [helpstring][id][propget] */ HRESULT ( STDMETHODCALLTYPE *get_Extension )( 
            WebFileProperties * This,
            /* [retval][out] */ BSTR *pbstrExtension);
        
        /* [helpstring][id][propget] */ HRESULT ( STDMETHODCALLTYPE *get_FullPath )( 
            WebFileProperties * This,
            /* [retval][out] */ BSTR *pbstrFullPath);
        
        /* [helpstring][id][propget] */ HRESULT ( STDMETHODCALLTYPE *get_URL )( 
            WebFileProperties * This,
            /* [retval][out] */ BSTR *pbstrURL);
        
        /* [hidden][helpstring][nonbrowsable][propget][id] */ HRESULT ( STDMETHODCALLTYPE *get_Extender )( 
            WebFileProperties * This,
            /* [in] */ BSTR ExtenderName,
            /* [retval][out] */ IDispatch **Extender);
        
        /* [hidden][helpstring][nonbrowsable][propget][id] */ HRESULT ( STDMETHODCALLTYPE *get_ExtenderNames )( 
            WebFileProperties * This,
            /* [retval][out] */ VARIANT *ExtenderNames);
        
        /* [hidden][helpstring][nonbrowsable][propget][id] */ HRESULT ( STDMETHODCALLTYPE *get_ExtenderCATID )( 
            WebFileProperties * This,
            /* [retval][out] */ BSTR *pRetval);
        
        /* [helpstring][id][propget] */ HRESULT ( STDMETHODCALLTYPE *get_RelativeURL )( 
            WebFileProperties * This,
            /* [retval][out] */ BSTR *pbstrRelativeURL);
        
        /* [helpstring][id][propget] */ HRESULT ( STDMETHODCALLTYPE *get_AutoRefreshPath )( 
            WebFileProperties * This,
            /* [retval][out] */ BSTR *pbstrAutoRefreshPath);
        
        /* [helpstring][id][propput] */ HRESULT ( STDMETHODCALLTYPE *put_AutoRefreshPath )( 
            WebFileProperties * This,
            /* [in] */ BSTR bstrAutoRefreshPath);
        
        END_INTERFACE
    } WebFilePropertiesVtbl;

    interface WebFileProperties
    {
        CONST_VTBL struct WebFilePropertiesVtbl *lpVtbl;
    };

    

#ifdef COBJMACROS


#define WebFileProperties_QueryInterface(This,riid,ppvObject)	\
    ( (This)->lpVtbl -> QueryInterface(This,riid,ppvObject) ) 

#define WebFileProperties_AddRef(This)	\
    ( (This)->lpVtbl -> AddRef(This) ) 

#define WebFileProperties_Release(This)	\
    ( (This)->lpVtbl -> Release(This) ) 


#define WebFileProperties_GetTypeInfoCount(This,pctinfo)	\
    ( (This)->lpVtbl -> GetTypeInfoCount(This,pctinfo) ) 

#define WebFileProperties_GetTypeInfo(This,iTInfo,lcid,ppTInfo)	\
    ( (This)->lpVtbl -> GetTypeInfo(This,iTInfo,lcid,ppTInfo) ) 

#define WebFileProperties_GetIDsOfNames(This,riid,rgszNames,cNames,lcid,rgDispId)	\
    ( (This)->lpVtbl -> GetIDsOfNames(This,riid,rgszNames,cNames,lcid,rgDispId) ) 

#define WebFileProperties_Invoke(This,dispIdMember,riid,lcid,wFlags,pDispParams,pVarResult,pExcepInfo,puArgErr)	\
    ( (This)->lpVtbl -> Invoke(This,dispIdMember,riid,lcid,wFlags,pDispParams,pVarResult,pExcepInfo,puArgErr) ) 


#define WebFileProperties_get___id(This,pbstrName)	\
    ( (This)->lpVtbl -> get___id(This,pbstrName) ) 

#define WebFileProperties_get_FileName(This,pbstrName)	\
    ( (This)->lpVtbl -> get_FileName(This,pbstrName) ) 

#define WebFileProperties_put_FileName(This,bstrName)	\
    ( (This)->lpVtbl -> put_FileName(This,bstrName) ) 

#define WebFileProperties_get_Extension(This,pbstrExtension)	\
    ( (This)->lpVtbl -> get_Extension(This,pbstrExtension) ) 

#define WebFileProperties_get_FullPath(This,pbstrFullPath)	\
    ( (This)->lpVtbl -> get_FullPath(This,pbstrFullPath) ) 

#define WebFileProperties_get_URL(This,pbstrURL)	\
    ( (This)->lpVtbl -> get_URL(This,pbstrURL) ) 

#define WebFileProperties_get_Extender(This,ExtenderName,Extender)	\
    ( (This)->lpVtbl -> get_Extender(This,ExtenderName,Extender) ) 

#define WebFileProperties_get_ExtenderNames(This,ExtenderNames)	\
    ( (This)->lpVtbl -> get_ExtenderNames(This,ExtenderNames) ) 

#define WebFileProperties_get_ExtenderCATID(This,pRetval)	\
    ( (This)->lpVtbl -> get_ExtenderCATID(This,pRetval) ) 

#define WebFileProperties_get_RelativeURL(This,pbstrRelativeURL)	\
    ( (This)->lpVtbl -> get_RelativeURL(This,pbstrRelativeURL) ) 

#define WebFileProperties_get_AutoRefreshPath(This,pbstrAutoRefreshPath)	\
    ( (This)->lpVtbl -> get_AutoRefreshPath(This,pbstrAutoRefreshPath) ) 

#define WebFileProperties_put_AutoRefreshPath(This,bstrAutoRefreshPath)	\
    ( (This)->lpVtbl -> put_AutoRefreshPath(This,bstrAutoRefreshPath) ) 

#endif /* COBJMACROS */


#endif 	/* C style interface */




#endif 	/* __WebFileProperties_INTERFACE_DEFINED__ */


#ifndef __WebFolderProperties_INTERFACE_DEFINED__
#define __WebFolderProperties_INTERFACE_DEFINED__

/* interface WebFolderProperties */
/* [local][hidden][unique][helpstring][dual][uuid][object] */ 


EXTERN_C const IID IID_WebFolderProperties;

#if defined(__cplusplus) && !defined(CINTERFACE)
    
    MIDL_INTERFACE("422AC997-E209-4faa-8DC6-9D2E5EFA4216")
    WebFolderProperties : public IDispatch
    {
    public:
        virtual /* [hidden][helpstring][id][propget] */ HRESULT STDMETHODCALLTYPE get___id( 
            /* [retval][out] */ BSTR *pbstrName) = 0;
        
        virtual /* [helpstring][id][propget] */ HRESULT STDMETHODCALLTYPE get_FileName( 
            /* [retval][out] */ BSTR *pbstrName) = 0;
        
        virtual /* [helpstring][id][propput] */ HRESULT STDMETHODCALLTYPE put_FileName( 
            /* [in] */ BSTR bstrName) = 0;
        
        virtual /* [helpstring][id][propget] */ HRESULT STDMETHODCALLTYPE get_FullPath( 
            /* [retval][out] */ BSTR *pbstrFullPath) = 0;
        
        virtual /* [helpstring][id][propget] */ HRESULT STDMETHODCALLTYPE get_URL( 
            /* [retval][out] */ BSTR *pbstrURL) = 0;
        
        virtual /* [helpstring][id][propget] */ HRESULT STDMETHODCALLTYPE get_FolderType( 
            /* [retval][out] */ webFolderType *pFolderType) = 0;
        
        virtual /* [helpstring][id][propget] */ HRESULT STDMETHODCALLTYPE get_RelativeURL( 
            /* [retval][out] */ BSTR *pbstrRelativeURL) = 0;
        
        virtual /* [helpstring][id][propget] */ HRESULT STDMETHODCALLTYPE get_CodeLanguage( 
            /* [retval][out] */ BSTR *pbstrCodeLanguage) = 0;
        
        virtual /* [helpstring][id][propget] */ HRESULT STDMETHODCALLTYPE get_MergeInProgress( 
            /* [retval][out] */ VARIANT_BOOL *pvbBOOL) = 0;
        
        virtual /* [hidden][helpstring][nonbrowsable][id][propget] */ HRESULT STDMETHODCALLTYPE get_Extender( 
            /* [in] */ BSTR ExtenderName,
            /* [retval][out] */ IDispatch **Extender) = 0;
        
        virtual /* [hidden][helpstring][nonbrowsable][id][propget] */ HRESULT STDMETHODCALLTYPE get_ExtenderNames( 
            /* [retval][out] */ VARIANT *ExtenderNames) = 0;
        
        virtual /* [hidden][helpstring][nonbrowsable][id][propget] */ HRESULT STDMETHODCALLTYPE get_ExtenderCATID( 
            /* [retval][out] */ BSTR *pRetval) = 0;
        
    };
    
    
#else 	/* C style interface */

    typedef struct WebFolderPropertiesVtbl
    {
        BEGIN_INTERFACE
        
        HRESULT ( STDMETHODCALLTYPE *QueryInterface )( 
            WebFolderProperties * This,
            /* [in] */ REFIID riid,
            /* [annotation][iid_is][out] */ 
            _COM_Outptr_  void **ppvObject);
        
        ULONG ( STDMETHODCALLTYPE *AddRef )( 
            WebFolderProperties * This);
        
        ULONG ( STDMETHODCALLTYPE *Release )( 
            WebFolderProperties * This);
        
        HRESULT ( STDMETHODCALLTYPE *GetTypeInfoCount )( 
            WebFolderProperties * This,
            /* [out] */ UINT *pctinfo);
        
        HRESULT ( STDMETHODCALLTYPE *GetTypeInfo )( 
            WebFolderProperties * This,
            /* [in] */ UINT iTInfo,
            /* [in] */ LCID lcid,
            /* [out] */ ITypeInfo **ppTInfo);
        
        HRESULT ( STDMETHODCALLTYPE *GetIDsOfNames )( 
            WebFolderProperties * This,
            /* [in] */ REFIID riid,
            /* [size_is][in] */ LPOLESTR *rgszNames,
            /* [range][in] */ UINT cNames,
            /* [in] */ LCID lcid,
            /* [size_is][out] */ DISPID *rgDispId);
        
        /* [local] */ HRESULT ( STDMETHODCALLTYPE *Invoke )( 
            WebFolderProperties * This,
            /* [annotation][in] */ 
            _In_  DISPID dispIdMember,
            /* [annotation][in] */ 
            _In_  REFIID riid,
            /* [annotation][in] */ 
            _In_  LCID lcid,
            /* [annotation][in] */ 
            _In_  WORD wFlags,
            /* [annotation][out][in] */ 
            _In_  DISPPARAMS *pDispParams,
            /* [annotation][out] */ 
            _Out_opt_  VARIANT *pVarResult,
            /* [annotation][out] */ 
            _Out_opt_  EXCEPINFO *pExcepInfo,
            /* [annotation][out] */ 
            _Out_opt_  UINT *puArgErr);
        
        /* [hidden][helpstring][id][propget] */ HRESULT ( STDMETHODCALLTYPE *get___id )( 
            WebFolderProperties * This,
            /* [retval][out] */ BSTR *pbstrName);
        
        /* [helpstring][id][propget] */ HRESULT ( STDMETHODCALLTYPE *get_FileName )( 
            WebFolderProperties * This,
            /* [retval][out] */ BSTR *pbstrName);
        
        /* [helpstring][id][propput] */ HRESULT ( STDMETHODCALLTYPE *put_FileName )( 
            WebFolderProperties * This,
            /* [in] */ BSTR bstrName);
        
        /* [helpstring][id][propget] */ HRESULT ( STDMETHODCALLTYPE *get_FullPath )( 
            WebFolderProperties * This,
            /* [retval][out] */ BSTR *pbstrFullPath);
        
        /* [helpstring][id][propget] */ HRESULT ( STDMETHODCALLTYPE *get_URL )( 
            WebFolderProperties * This,
            /* [retval][out] */ BSTR *pbstrURL);
        
        /* [helpstring][id][propget] */ HRESULT ( STDMETHODCALLTYPE *get_FolderType )( 
            WebFolderProperties * This,
            /* [retval][out] */ webFolderType *pFolderType);
        
        /* [helpstring][id][propget] */ HRESULT ( STDMETHODCALLTYPE *get_RelativeURL )( 
            WebFolderProperties * This,
            /* [retval][out] */ BSTR *pbstrRelativeURL);
        
        /* [helpstring][id][propget] */ HRESULT ( STDMETHODCALLTYPE *get_CodeLanguage )( 
            WebFolderProperties * This,
            /* [retval][out] */ BSTR *pbstrCodeLanguage);
        
        /* [helpstring][id][propget] */ HRESULT ( STDMETHODCALLTYPE *get_MergeInProgress )( 
            WebFolderProperties * This,
            /* [retval][out] */ VARIANT_BOOL *pvbBOOL);
        
        /* [hidden][helpstring][nonbrowsable][id][propget] */ HRESULT ( STDMETHODCALLTYPE *get_Extender )( 
            WebFolderProperties * This,
            /* [in] */ BSTR ExtenderName,
            /* [retval][out] */ IDispatch **Extender);
        
        /* [hidden][helpstring][nonbrowsable][id][propget] */ HRESULT ( STDMETHODCALLTYPE *get_ExtenderNames )( 
            WebFolderProperties * This,
            /* [retval][out] */ VARIANT *ExtenderNames);
        
        /* [hidden][helpstring][nonbrowsable][id][propget] */ HRESULT ( STDMETHODCALLTYPE *get_ExtenderCATID )( 
            WebFolderProperties * This,
            /* [retval][out] */ BSTR *pRetval);
        
        END_INTERFACE
    } WebFolderPropertiesVtbl;

    interface WebFolderProperties
    {
        CONST_VTBL struct WebFolderPropertiesVtbl *lpVtbl;
    };

    

#ifdef COBJMACROS


#define WebFolderProperties_QueryInterface(This,riid,ppvObject)	\
    ( (This)->lpVtbl -> QueryInterface(This,riid,ppvObject) ) 

#define WebFolderProperties_AddRef(This)	\
    ( (This)->lpVtbl -> AddRef(This) ) 

#define WebFolderProperties_Release(This)	\
    ( (This)->lpVtbl -> Release(This) ) 


#define WebFolderProperties_GetTypeInfoCount(This,pctinfo)	\
    ( (This)->lpVtbl -> GetTypeInfoCount(This,pctinfo) ) 

#define WebFolderProperties_GetTypeInfo(This,iTInfo,lcid,ppTInfo)	\
    ( (This)->lpVtbl -> GetTypeInfo(This,iTInfo,lcid,ppTInfo) ) 

#define WebFolderProperties_GetIDsOfNames(This,riid,rgszNames,cNames,lcid,rgDispId)	\
    ( (This)->lpVtbl -> GetIDsOfNames(This,riid,rgszNames,cNames,lcid,rgDispId) ) 

#define WebFolderProperties_Invoke(This,dispIdMember,riid,lcid,wFlags,pDispParams,pVarResult,pExcepInfo,puArgErr)	\
    ( (This)->lpVtbl -> Invoke(This,dispIdMember,riid,lcid,wFlags,pDispParams,pVarResult,pExcepInfo,puArgErr) ) 


#define WebFolderProperties_get___id(This,pbstrName)	\
    ( (This)->lpVtbl -> get___id(This,pbstrName) ) 

#define WebFolderProperties_get_FileName(This,pbstrName)	\
    ( (This)->lpVtbl -> get_FileName(This,pbstrName) ) 

#define WebFolderProperties_put_FileName(This,bstrName)	\
    ( (This)->lpVtbl -> put_FileName(This,bstrName) ) 

#define WebFolderProperties_get_FullPath(This,pbstrFullPath)	\
    ( (This)->lpVtbl -> get_FullPath(This,pbstrFullPath) ) 

#define WebFolderProperties_get_URL(This,pbstrURL)	\
    ( (This)->lpVtbl -> get_URL(This,pbstrURL) ) 

#define WebFolderProperties_get_FolderType(This,pFolderType)	\
    ( (This)->lpVtbl -> get_FolderType(This,pFolderType) ) 

#define WebFolderProperties_get_RelativeURL(This,pbstrRelativeURL)	\
    ( (This)->lpVtbl -> get_RelativeURL(This,pbstrRelativeURL) ) 

#define WebFolderProperties_get_CodeLanguage(This,pbstrCodeLanguage)	\
    ( (This)->lpVtbl -> get_CodeLanguage(This,pbstrCodeLanguage) ) 

#define WebFolderProperties_get_MergeInProgress(This,pvbBOOL)	\
    ( (This)->lpVtbl -> get_MergeInProgress(This,pvbBOOL) ) 

#define WebFolderProperties_get_Extender(This,ExtenderName,Extender)	\
    ( (This)->lpVtbl -> get_Extender(This,ExtenderName,Extender) ) 

#define WebFolderProperties_get_ExtenderNames(This,ExtenderNames)	\
    ( (This)->lpVtbl -> get_ExtenderNames(This,ExtenderNames) ) 

#define WebFolderProperties_get_ExtenderCATID(This,pRetval)	\
    ( (This)->lpVtbl -> get_ExtenderCATID(This,pRetval) ) 

#endif /* COBJMACROS */


#endif 	/* C style interface */




#endif 	/* __WebFolderProperties_INTERFACE_DEFINED__ */


#ifndef __WebService_INTERFACE_DEFINED__
#define __WebService_INTERFACE_DEFINED__

/* interface WebService */
/* [uuid][object][oleautomation][dual] */ 


EXTERN_C const IID IID_WebService;

#if defined(__cplusplus) && !defined(CINTERFACE)
    
    MIDL_INTERFACE("298682BA-70DD-40bf-95E9-4DD4293BA56D")
    WebService : public IDispatch
    {
    public:
        virtual /* [helpstring][propget][id] */ HRESULT STDMETHODCALLTYPE get_DTE( 
            /* [retval][out] */ __RPC__deref_out_opt /* external definition not present */ DTE **ppDTE) = 0;
        
        virtual /* [helpstring][propget][id] */ HRESULT STDMETHODCALLTYPE get_ContainingProject( 
            /* [retval][out] */ __RPC__deref_out_opt /* external definition not present */ Project **ppProject) = 0;
        
        virtual /* [helpstring][propget][id] */ HRESULT STDMETHODCALLTYPE get_URL( 
            /* [retval][out] */ __RPC__deref_out_opt BSTR *bstrUrl) = 0;
        
        virtual /* [helpstring][propget][id] */ HRESULT STDMETHODCALLTYPE get_ClassName( 
            /* [retval][out] */ __RPC__deref_out_opt BSTR *bstrClassName) = 0;
        
        virtual /* [helpstring][propget][id] */ HRESULT STDMETHODCALLTYPE get_AppRelativeUrl( 
            /* [retval][out] */ __RPC__deref_out_opt BSTR *bstrAppUrl) = 0;
        
        virtual /* [helpstring][propget][id] */ HRESULT STDMETHODCALLTYPE get_FileCodeModel( 
            /* [retval][out] */ __RPC__deref_out_opt /* external definition not present */ FileCodeModel **ppFileCodeModel) = 0;
        
        virtual /* [helpstring][propget][id] */ HRESULT STDMETHODCALLTYPE get_ProjectItem( 
            /* [retval][out] */ __RPC__deref_out_opt /* external definition not present */ ProjectItem **ppProjectItem) = 0;
        
        virtual /* [helpstring][propget][id] */ HRESULT STDMETHODCALLTYPE get_ClassFileItem( 
            /* [retval][out] */ __RPC__deref_out_opt /* external definition not present */ ProjectItem **ppClassFileItem) = 0;
        
    };
    
    
#else 	/* C style interface */

    typedef struct WebServiceVtbl
    {
        BEGIN_INTERFACE
        
        HRESULT ( STDMETHODCALLTYPE *QueryInterface )( 
            __RPC__in WebService * This,
            /* [in] */ __RPC__in REFIID riid,
            /* [annotation][iid_is][out] */ 
            _COM_Outptr_  void **ppvObject);
        
        ULONG ( STDMETHODCALLTYPE *AddRef )( 
            __RPC__in WebService * This);
        
        ULONG ( STDMETHODCALLTYPE *Release )( 
            __RPC__in WebService * This);
        
        HRESULT ( STDMETHODCALLTYPE *GetTypeInfoCount )( 
            __RPC__in WebService * This,
            /* [out] */ __RPC__out UINT *pctinfo);
        
        HRESULT ( STDMETHODCALLTYPE *GetTypeInfo )( 
            __RPC__in WebService * This,
            /* [in] */ UINT iTInfo,
            /* [in] */ LCID lcid,
            /* [out] */ __RPC__deref_out_opt ITypeInfo **ppTInfo);
        
        HRESULT ( STDMETHODCALLTYPE *GetIDsOfNames )( 
            __RPC__in WebService * This,
            /* [in] */ __RPC__in REFIID riid,
            /* [size_is][in] */ __RPC__in_ecount_full(cNames) LPOLESTR *rgszNames,
            /* [range][in] */ __RPC__in_range(0,16384) UINT cNames,
            /* [in] */ LCID lcid,
            /* [size_is][out] */ __RPC__out_ecount_full(cNames) DISPID *rgDispId);
        
        /* [local] */ HRESULT ( STDMETHODCALLTYPE *Invoke )( 
            WebService * This,
            /* [annotation][in] */ 
            _In_  DISPID dispIdMember,
            /* [annotation][in] */ 
            _In_  REFIID riid,
            /* [annotation][in] */ 
            _In_  LCID lcid,
            /* [annotation][in] */ 
            _In_  WORD wFlags,
            /* [annotation][out][in] */ 
            _In_  DISPPARAMS *pDispParams,
            /* [annotation][out] */ 
            _Out_opt_  VARIANT *pVarResult,
            /* [annotation][out] */ 
            _Out_opt_  EXCEPINFO *pExcepInfo,
            /* [annotation][out] */ 
            _Out_opt_  UINT *puArgErr);
        
        /* [helpstring][propget][id] */ HRESULT ( STDMETHODCALLTYPE *get_DTE )( 
            __RPC__in WebService * This,
            /* [retval][out] */ __RPC__deref_out_opt /* external definition not present */ DTE **ppDTE);
        
        /* [helpstring][propget][id] */ HRESULT ( STDMETHODCALLTYPE *get_ContainingProject )( 
            __RPC__in WebService * This,
            /* [retval][out] */ __RPC__deref_out_opt /* external definition not present */ Project **ppProject);
        
        /* [helpstring][propget][id] */ HRESULT ( STDMETHODCALLTYPE *get_URL )( 
            __RPC__in WebService * This,
            /* [retval][out] */ __RPC__deref_out_opt BSTR *bstrUrl);
        
        /* [helpstring][propget][id] */ HRESULT ( STDMETHODCALLTYPE *get_ClassName )( 
            __RPC__in WebService * This,
            /* [retval][out] */ __RPC__deref_out_opt BSTR *bstrClassName);
        
        /* [helpstring][propget][id] */ HRESULT ( STDMETHODCALLTYPE *get_AppRelativeUrl )( 
            __RPC__in WebService * This,
            /* [retval][out] */ __RPC__deref_out_opt BSTR *bstrAppUrl);
        
        /* [helpstring][propget][id] */ HRESULT ( STDMETHODCALLTYPE *get_FileCodeModel )( 
            __RPC__in WebService * This,
            /* [retval][out] */ __RPC__deref_out_opt /* external definition not present */ FileCodeModel **ppFileCodeModel);
        
        /* [helpstring][propget][id] */ HRESULT ( STDMETHODCALLTYPE *get_ProjectItem )( 
            __RPC__in WebService * This,
            /* [retval][out] */ __RPC__deref_out_opt /* external definition not present */ ProjectItem **ppProjectItem);
        
        /* [helpstring][propget][id] */ HRESULT ( STDMETHODCALLTYPE *get_ClassFileItem )( 
            __RPC__in WebService * This,
            /* [retval][out] */ __RPC__deref_out_opt /* external definition not present */ ProjectItem **ppClassFileItem);
        
        END_INTERFACE
    } WebServiceVtbl;

    interface WebService
    {
        CONST_VTBL struct WebServiceVtbl *lpVtbl;
    };

    

#ifdef COBJMACROS


#define WebService_QueryInterface(This,riid,ppvObject)	\
    ( (This)->lpVtbl -> QueryInterface(This,riid,ppvObject) ) 

#define WebService_AddRef(This)	\
    ( (This)->lpVtbl -> AddRef(This) ) 

#define WebService_Release(This)	\
    ( (This)->lpVtbl -> Release(This) ) 


#define WebService_GetTypeInfoCount(This,pctinfo)	\
    ( (This)->lpVtbl -> GetTypeInfoCount(This,pctinfo) ) 

#define WebService_GetTypeInfo(This,iTInfo,lcid,ppTInfo)	\
    ( (This)->lpVtbl -> GetTypeInfo(This,iTInfo,lcid,ppTInfo) ) 

#define WebService_GetIDsOfNames(This,riid,rgszNames,cNames,lcid,rgDispId)	\
    ( (This)->lpVtbl -> GetIDsOfNames(This,riid,rgszNames,cNames,lcid,rgDispId) ) 

#define WebService_Invoke(This,dispIdMember,riid,lcid,wFlags,pDispParams,pVarResult,pExcepInfo,puArgErr)	\
    ( (This)->lpVtbl -> Invoke(This,dispIdMember,riid,lcid,wFlags,pDispParams,pVarResult,pExcepInfo,puArgErr) ) 


#define WebService_get_DTE(This,ppDTE)	\
    ( (This)->lpVtbl -> get_DTE(This,ppDTE) ) 

#define WebService_get_ContainingProject(This,ppProject)	\
    ( (This)->lpVtbl -> get_ContainingProject(This,ppProject) ) 

#define WebService_get_URL(This,bstrUrl)	\
    ( (This)->lpVtbl -> get_URL(This,bstrUrl) ) 

#define WebService_get_ClassName(This,bstrClassName)	\
    ( (This)->lpVtbl -> get_ClassName(This,bstrClassName) ) 

#define WebService_get_AppRelativeUrl(This,bstrAppUrl)	\
    ( (This)->lpVtbl -> get_AppRelativeUrl(This,bstrAppUrl) ) 

#define WebService_get_FileCodeModel(This,ppFileCodeModel)	\
    ( (This)->lpVtbl -> get_FileCodeModel(This,ppFileCodeModel) ) 

#define WebService_get_ProjectItem(This,ppProjectItem)	\
    ( (This)->lpVtbl -> get_ProjectItem(This,ppProjectItem) ) 

#define WebService_get_ClassFileItem(This,ppClassFileItem)	\
    ( (This)->lpVtbl -> get_ClassFileItem(This,ppClassFileItem) ) 

#endif /* COBJMACROS */


#endif 	/* C style interface */




#endif 	/* __WebService_INTERFACE_DEFINED__ */


#ifndef __WebServices_INTERFACE_DEFINED__
#define __WebServices_INTERFACE_DEFINED__

/* interface WebServices */
/* [uuid][object][oleautomation][dual] */ 


EXTERN_C const IID IID_WebServices;

#if defined(__cplusplus) && !defined(CINTERFACE)
    
    MIDL_INTERFACE("7BAEA84D-83B3-449d-B029-D225BD95820E")
    WebServices : public IDispatch
    {
    public:
        virtual /* [helpstring][propget][id] */ HRESULT STDMETHODCALLTYPE get_DTE( 
            /* [retval][out] */ __RPC__deref_out_opt /* external definition not present */ DTE **ppDTE) = 0;
        
        virtual /* [helpstring][propget][id] */ HRESULT STDMETHODCALLTYPE get_ContainingProject( 
            /* [retval][out] */ __RPC__deref_out_opt /* external definition not present */ Project **ppProject) = 0;
        
        virtual /* [helpstring][propget][id] */ HRESULT STDMETHODCALLTYPE get_Count( 
            /* [retval][out] */ __RPC__out long *plCount) = 0;
        
        virtual /* [helpstring][id] */ HRESULT STDMETHODCALLTYPE Item( 
            /* [in] */ VARIANT index,
            /* [retval][out] */ __RPC__deref_out_opt WebService **ppWebServices) = 0;
        
        virtual /* [id][restricted] */ HRESULT STDMETHODCALLTYPE _NewEnum( 
            /* [retval][out] */ __RPC__deref_out_opt IUnknown **ppiuReturn) = 0;
        
    };
    
    
#else 	/* C style interface */

    typedef struct WebServicesVtbl
    {
        BEGIN_INTERFACE
        
        HRESULT ( STDMETHODCALLTYPE *QueryInterface )( 
            __RPC__in WebServices * This,
            /* [in] */ __RPC__in REFIID riid,
            /* [annotation][iid_is][out] */ 
            _COM_Outptr_  void **ppvObject);
        
        ULONG ( STDMETHODCALLTYPE *AddRef )( 
            __RPC__in WebServices * This);
        
        ULONG ( STDMETHODCALLTYPE *Release )( 
            __RPC__in WebServices * This);
        
        HRESULT ( STDMETHODCALLTYPE *GetTypeInfoCount )( 
            __RPC__in WebServices * This,
            /* [out] */ __RPC__out UINT *pctinfo);
        
        HRESULT ( STDMETHODCALLTYPE *GetTypeInfo )( 
            __RPC__in WebServices * This,
            /* [in] */ UINT iTInfo,
            /* [in] */ LCID lcid,
            /* [out] */ __RPC__deref_out_opt ITypeInfo **ppTInfo);
        
        HRESULT ( STDMETHODCALLTYPE *GetIDsOfNames )( 
            __RPC__in WebServices * This,
            /* [in] */ __RPC__in REFIID riid,
            /* [size_is][in] */ __RPC__in_ecount_full(cNames) LPOLESTR *rgszNames,
            /* [range][in] */ __RPC__in_range(0,16384) UINT cNames,
            /* [in] */ LCID lcid,
            /* [size_is][out] */ __RPC__out_ecount_full(cNames) DISPID *rgDispId);
        
        /* [local] */ HRESULT ( STDMETHODCALLTYPE *Invoke )( 
            WebServices * This,
            /* [annotation][in] */ 
            _In_  DISPID dispIdMember,
            /* [annotation][in] */ 
            _In_  REFIID riid,
            /* [annotation][in] */ 
            _In_  LCID lcid,
            /* [annotation][in] */ 
            _In_  WORD wFlags,
            /* [annotation][out][in] */ 
            _In_  DISPPARAMS *pDispParams,
            /* [annotation][out] */ 
            _Out_opt_  VARIANT *pVarResult,
            /* [annotation][out] */ 
            _Out_opt_  EXCEPINFO *pExcepInfo,
            /* [annotation][out] */ 
            _Out_opt_  UINT *puArgErr);
        
        /* [helpstring][propget][id] */ HRESULT ( STDMETHODCALLTYPE *get_DTE )( 
            __RPC__in WebServices * This,
            /* [retval][out] */ __RPC__deref_out_opt /* external definition not present */ DTE **ppDTE);
        
        /* [helpstring][propget][id] */ HRESULT ( STDMETHODCALLTYPE *get_ContainingProject )( 
            __RPC__in WebServices * This,
            /* [retval][out] */ __RPC__deref_out_opt /* external definition not present */ Project **ppProject);
        
        /* [helpstring][propget][id] */ HRESULT ( STDMETHODCALLTYPE *get_Count )( 
            __RPC__in WebServices * This,
            /* [retval][out] */ __RPC__out long *plCount);
        
        /* [helpstring][id] */ HRESULT ( STDMETHODCALLTYPE *Item )( 
            __RPC__in WebServices * This,
            /* [in] */ VARIANT index,
            /* [retval][out] */ __RPC__deref_out_opt WebService **ppWebServices);
        
        /* [id][restricted] */ HRESULT ( STDMETHODCALLTYPE *_NewEnum )( 
            __RPC__in WebServices * This,
            /* [retval][out] */ __RPC__deref_out_opt IUnknown **ppiuReturn);
        
        END_INTERFACE
    } WebServicesVtbl;

    interface WebServices
    {
        CONST_VTBL struct WebServicesVtbl *lpVtbl;
    };

    

#ifdef COBJMACROS


#define WebServices_QueryInterface(This,riid,ppvObject)	\
    ( (This)->lpVtbl -> QueryInterface(This,riid,ppvObject) ) 

#define WebServices_AddRef(This)	\
    ( (This)->lpVtbl -> AddRef(This) ) 

#define WebServices_Release(This)	\
    ( (This)->lpVtbl -> Release(This) ) 


#define WebServices_GetTypeInfoCount(This,pctinfo)	\
    ( (This)->lpVtbl -> GetTypeInfoCount(This,pctinfo) ) 

#define WebServices_GetTypeInfo(This,iTInfo,lcid,ppTInfo)	\
    ( (This)->lpVtbl -> GetTypeInfo(This,iTInfo,lcid,ppTInfo) ) 

#define WebServices_GetIDsOfNames(This,riid,rgszNames,cNames,lcid,rgDispId)	\
    ( (This)->lpVtbl -> GetIDsOfNames(This,riid,rgszNames,cNames,lcid,rgDispId) ) 

#define WebServices_Invoke(This,dispIdMember,riid,lcid,wFlags,pDispParams,pVarResult,pExcepInfo,puArgErr)	\
    ( (This)->lpVtbl -> Invoke(This,dispIdMember,riid,lcid,wFlags,pDispParams,pVarResult,pExcepInfo,puArgErr) ) 


#define WebServices_get_DTE(This,ppDTE)	\
    ( (This)->lpVtbl -> get_DTE(This,ppDTE) ) 

#define WebServices_get_ContainingProject(This,ppProject)	\
    ( (This)->lpVtbl -> get_ContainingProject(This,ppProject) ) 

#define WebServices_get_Count(This,plCount)	\
    ( (This)->lpVtbl -> get_Count(This,plCount) ) 

#define WebServices_Item(This,index,ppWebServices)	\
    ( (This)->lpVtbl -> Item(This,index,ppWebServices) ) 

#define WebServices__NewEnum(This,ppiuReturn)	\
    ( (This)->lpVtbl -> _NewEnum(This,ppiuReturn) ) 

#endif /* COBJMACROS */


#endif 	/* C style interface */




#endif 	/* __WebServices_INTERFACE_DEFINED__ */


#ifndef __WebReference_INTERFACE_DEFINED__
#define __WebReference_INTERFACE_DEFINED__

/* interface WebReference */
/* [uuid][object][oleautomation][dual] */ 


EXTERN_C const IID IID_WebReference;

#if defined(__cplusplus) && !defined(CINTERFACE)
    
    MIDL_INTERFACE("DCB348C9-62ED-4332-B90F-077AAC7BAB14")
    WebReference : public IDispatch
    {
    public:
        virtual /* [helpstring][propget][id] */ HRESULT STDMETHODCALLTYPE get_DTE( 
            /* [retval][out] */ __RPC__deref_out_opt /* external definition not present */ DTE **ppDTE) = 0;
        
        virtual /* [helpstring][propget][id] */ HRESULT STDMETHODCALLTYPE get_ContainingProject( 
            /* [retval][out] */ __RPC__deref_out_opt /* external definition not present */ Project **ppProject) = 0;
        
        virtual /* [helpstring][id] */ HRESULT STDMETHODCALLTYPE Remove( void) = 0;
        
        virtual /* [helpstring][propget][id] */ HRESULT STDMETHODCALLTYPE get_DynamicUrl( 
            /* [retval][out] */ __RPC__deref_out_opt BSTR *pbstrUrl) = 0;
        
        virtual /* [helpstring][propput][id] */ HRESULT STDMETHODCALLTYPE put_DynamicUrl( 
            /* [in] */ __RPC__in BSTR bstrUrl) = 0;
        
        virtual /* [helpstring][propget][id] */ HRESULT STDMETHODCALLTYPE get_DynamicPropName( 
            /* [retval][out] */ __RPC__deref_out_opt BSTR *pbstrDynamicPropNameUrl) = 0;
        
        virtual /* [helpstring][propget][id] */ HRESULT STDMETHODCALLTYPE get_Namespace( 
            /* [retval][out] */ __RPC__deref_out_opt BSTR *pbstrRefreshUrl) = 0;
        
        virtual /* [helpstring][propget][id] */ HRESULT STDMETHODCALLTYPE get_ServiceName( 
            /* [retval][out] */ __RPC__deref_out_opt BSTR *pbstrRefreshUrl) = 0;
        
        virtual /* [helpstring][propget][id] */ HRESULT STDMETHODCALLTYPE get_ServiceLocationUrl( 
            /* [retval][out] */ __RPC__deref_out_opt BSTR *pbstrUrl) = 0;
        
        virtual /* [helpstring][id] */ HRESULT STDMETHODCALLTYPE Update( void) = 0;
        
        virtual /* [helpstring][propget][id] */ HRESULT STDMETHODCALLTYPE get_FileCodeModel( 
            /* [retval][out] */ __RPC__deref_out_opt /* external definition not present */ FileCodeModel **ppFileCodeModel) = 0;
        
        virtual /* [helpstring][propget][id] */ HRESULT STDMETHODCALLTYPE get_ProjectItem( 
            /* [retval][out] */ __RPC__deref_out_opt /* external definition not present */ ProjectItem **ppProjectItem) = 0;
        
        virtual /* [helpstring][propget][id] */ HRESULT STDMETHODCALLTYPE get_ServiceDefinitionUrl( 
            /* [retval][out] */ __RPC__deref_out_opt BSTR *pbstrUrl) = 0;
        
        virtual /* [helpstring][propget][id] */ HRESULT STDMETHODCALLTYPE get_Discomap( 
            /* [retval][out] */ __RPC__deref_out_opt /* external definition not present */ ProjectItem **ppProjectItem) = 0;
        
        virtual /* [helpstring][propget][id] */ HRESULT STDMETHODCALLTYPE get_WsdlAppRelativeUrl( 
            /* [retval][out] */ __RPC__deref_out_opt BSTR *pbstrWsdlAppRelativeUrl) = 0;
        
    };
    
    
#else 	/* C style interface */

    typedef struct WebReferenceVtbl
    {
        BEGIN_INTERFACE
        
        HRESULT ( STDMETHODCALLTYPE *QueryInterface )( 
            __RPC__in WebReference * This,
            /* [in] */ __RPC__in REFIID riid,
            /* [annotation][iid_is][out] */ 
            _COM_Outptr_  void **ppvObject);
        
        ULONG ( STDMETHODCALLTYPE *AddRef )( 
            __RPC__in WebReference * This);
        
        ULONG ( STDMETHODCALLTYPE *Release )( 
            __RPC__in WebReference * This);
        
        HRESULT ( STDMETHODCALLTYPE *GetTypeInfoCount )( 
            __RPC__in WebReference * This,
            /* [out] */ __RPC__out UINT *pctinfo);
        
        HRESULT ( STDMETHODCALLTYPE *GetTypeInfo )( 
            __RPC__in WebReference * This,
            /* [in] */ UINT iTInfo,
            /* [in] */ LCID lcid,
            /* [out] */ __RPC__deref_out_opt ITypeInfo **ppTInfo);
        
        HRESULT ( STDMETHODCALLTYPE *GetIDsOfNames )( 
            __RPC__in WebReference * This,
            /* [in] */ __RPC__in REFIID riid,
            /* [size_is][in] */ __RPC__in_ecount_full(cNames) LPOLESTR *rgszNames,
            /* [range][in] */ __RPC__in_range(0,16384) UINT cNames,
            /* [in] */ LCID lcid,
            /* [size_is][out] */ __RPC__out_ecount_full(cNames) DISPID *rgDispId);
        
        /* [local] */ HRESULT ( STDMETHODCALLTYPE *Invoke )( 
            WebReference * This,
            /* [annotation][in] */ 
            _In_  DISPID dispIdMember,
            /* [annotation][in] */ 
            _In_  REFIID riid,
            /* [annotation][in] */ 
            _In_  LCID lcid,
            /* [annotation][in] */ 
            _In_  WORD wFlags,
            /* [annotation][out][in] */ 
            _In_  DISPPARAMS *pDispParams,
            /* [annotation][out] */ 
            _Out_opt_  VARIANT *pVarResult,
            /* [annotation][out] */ 
            _Out_opt_  EXCEPINFO *pExcepInfo,
            /* [annotation][out] */ 
            _Out_opt_  UINT *puArgErr);
        
        /* [helpstring][propget][id] */ HRESULT ( STDMETHODCALLTYPE *get_DTE )( 
            __RPC__in WebReference * This,
            /* [retval][out] */ __RPC__deref_out_opt /* external definition not present */ DTE **ppDTE);
        
        /* [helpstring][propget][id] */ HRESULT ( STDMETHODCALLTYPE *get_ContainingProject )( 
            __RPC__in WebReference * This,
            /* [retval][out] */ __RPC__deref_out_opt /* external definition not present */ Project **ppProject);
        
        /* [helpstring][id] */ HRESULT ( STDMETHODCALLTYPE *Remove )( 
            __RPC__in WebReference * This);
        
        /* [helpstring][propget][id] */ HRESULT ( STDMETHODCALLTYPE *get_DynamicUrl )( 
            __RPC__in WebReference * This,
            /* [retval][out] */ __RPC__deref_out_opt BSTR *pbstrUrl);
        
        /* [helpstring][propput][id] */ HRESULT ( STDMETHODCALLTYPE *put_DynamicUrl )( 
            __RPC__in WebReference * This,
            /* [in] */ __RPC__in BSTR bstrUrl);
        
        /* [helpstring][propget][id] */ HRESULT ( STDMETHODCALLTYPE *get_DynamicPropName )( 
            __RPC__in WebReference * This,
            /* [retval][out] */ __RPC__deref_out_opt BSTR *pbstrDynamicPropNameUrl);
        
        /* [helpstring][propget][id] */ HRESULT ( STDMETHODCALLTYPE *get_Namespace )( 
            __RPC__in WebReference * This,
            /* [retval][out] */ __RPC__deref_out_opt BSTR *pbstrRefreshUrl);
        
        /* [helpstring][propget][id] */ HRESULT ( STDMETHODCALLTYPE *get_ServiceName )( 
            __RPC__in WebReference * This,
            /* [retval][out] */ __RPC__deref_out_opt BSTR *pbstrRefreshUrl);
        
        /* [helpstring][propget][id] */ HRESULT ( STDMETHODCALLTYPE *get_ServiceLocationUrl )( 
            __RPC__in WebReference * This,
            /* [retval][out] */ __RPC__deref_out_opt BSTR *pbstrUrl);
        
        /* [helpstring][id] */ HRESULT ( STDMETHODCALLTYPE *Update )( 
            __RPC__in WebReference * This);
        
        /* [helpstring][propget][id] */ HRESULT ( STDMETHODCALLTYPE *get_FileCodeModel )( 
            __RPC__in WebReference * This,
            /* [retval][out] */ __RPC__deref_out_opt /* external definition not present */ FileCodeModel **ppFileCodeModel);
        
        /* [helpstring][propget][id] */ HRESULT ( STDMETHODCALLTYPE *get_ProjectItem )( 
            __RPC__in WebReference * This,
            /* [retval][out] */ __RPC__deref_out_opt /* external definition not present */ ProjectItem **ppProjectItem);
        
        /* [helpstring][propget][id] */ HRESULT ( STDMETHODCALLTYPE *get_ServiceDefinitionUrl )( 
            __RPC__in WebReference * This,
            /* [retval][out] */ __RPC__deref_out_opt BSTR *pbstrUrl);
        
        /* [helpstring][propget][id] */ HRESULT ( STDMETHODCALLTYPE *get_Discomap )( 
            __RPC__in WebReference * This,
            /* [retval][out] */ __RPC__deref_out_opt /* external definition not present */ ProjectItem **ppProjectItem);
        
        /* [helpstring][propget][id] */ HRESULT ( STDMETHODCALLTYPE *get_WsdlAppRelativeUrl )( 
            __RPC__in WebReference * This,
            /* [retval][out] */ __RPC__deref_out_opt BSTR *pbstrWsdlAppRelativeUrl);
        
        END_INTERFACE
    } WebReferenceVtbl;

    interface WebReference
    {
        CONST_VTBL struct WebReferenceVtbl *lpVtbl;
    };

    

#ifdef COBJMACROS


#define WebReference_QueryInterface(This,riid,ppvObject)	\
    ( (This)->lpVtbl -> QueryInterface(This,riid,ppvObject) ) 

#define WebReference_AddRef(This)	\
    ( (This)->lpVtbl -> AddRef(This) ) 

#define WebReference_Release(This)	\
    ( (This)->lpVtbl -> Release(This) ) 


#define WebReference_GetTypeInfoCount(This,pctinfo)	\
    ( (This)->lpVtbl -> GetTypeInfoCount(This,pctinfo) ) 

#define WebReference_GetTypeInfo(This,iTInfo,lcid,ppTInfo)	\
    ( (This)->lpVtbl -> GetTypeInfo(This,iTInfo,lcid,ppTInfo) ) 

#define WebReference_GetIDsOfNames(This,riid,rgszNames,cNames,lcid,rgDispId)	\
    ( (This)->lpVtbl -> GetIDsOfNames(This,riid,rgszNames,cNames,lcid,rgDispId) ) 

#define WebReference_Invoke(This,dispIdMember,riid,lcid,wFlags,pDispParams,pVarResult,pExcepInfo,puArgErr)	\
    ( (This)->lpVtbl -> Invoke(This,dispIdMember,riid,lcid,wFlags,pDispParams,pVarResult,pExcepInfo,puArgErr) ) 


#define WebReference_get_DTE(This,ppDTE)	\
    ( (This)->lpVtbl -> get_DTE(This,ppDTE) ) 

#define WebReference_get_ContainingProject(This,ppProject)	\
    ( (This)->lpVtbl -> get_ContainingProject(This,ppProject) ) 

#define WebReference_Remove(This)	\
    ( (This)->lpVtbl -> Remove(This) ) 

#define WebReference_get_DynamicUrl(This,pbstrUrl)	\
    ( (This)->lpVtbl -> get_DynamicUrl(This,pbstrUrl) ) 

#define WebReference_put_DynamicUrl(This,bstrUrl)	\
    ( (This)->lpVtbl -> put_DynamicUrl(This,bstrUrl) ) 

#define WebReference_get_DynamicPropName(This,pbstrDynamicPropNameUrl)	\
    ( (This)->lpVtbl -> get_DynamicPropName(This,pbstrDynamicPropNameUrl) ) 

#define WebReference_get_Namespace(This,pbstrRefreshUrl)	\
    ( (This)->lpVtbl -> get_Namespace(This,pbstrRefreshUrl) ) 

#define WebReference_get_ServiceName(This,pbstrRefreshUrl)	\
    ( (This)->lpVtbl -> get_ServiceName(This,pbstrRefreshUrl) ) 

#define WebReference_get_ServiceLocationUrl(This,pbstrUrl)	\
    ( (This)->lpVtbl -> get_ServiceLocationUrl(This,pbstrUrl) ) 

#define WebReference_Update(This)	\
    ( (This)->lpVtbl -> Update(This) ) 

#define WebReference_get_FileCodeModel(This,ppFileCodeModel)	\
    ( (This)->lpVtbl -> get_FileCodeModel(This,ppFileCodeModel) ) 

#define WebReference_get_ProjectItem(This,ppProjectItem)	\
    ( (This)->lpVtbl -> get_ProjectItem(This,ppProjectItem) ) 

#define WebReference_get_ServiceDefinitionUrl(This,pbstrUrl)	\
    ( (This)->lpVtbl -> get_ServiceDefinitionUrl(This,pbstrUrl) ) 

#define WebReference_get_Discomap(This,ppProjectItem)	\
    ( (This)->lpVtbl -> get_Discomap(This,ppProjectItem) ) 

#define WebReference_get_WsdlAppRelativeUrl(This,pbstrWsdlAppRelativeUrl)	\
    ( (This)->lpVtbl -> get_WsdlAppRelativeUrl(This,pbstrWsdlAppRelativeUrl) ) 

#endif /* COBJMACROS */


#endif 	/* C style interface */




#endif 	/* __WebReference_INTERFACE_DEFINED__ */


#ifndef __WebReferences_INTERFACE_DEFINED__
#define __WebReferences_INTERFACE_DEFINED__

/* interface WebReferences */
/* [uuid][object][oleautomation][dual] */ 


EXTERN_C const IID IID_WebReferences;

#if defined(__cplusplus) && !defined(CINTERFACE)
    
    MIDL_INTERFACE("C9E6C08E-1FBB-4f2c-9D26-A206BAC90004")
    WebReferences : public IDispatch
    {
    public:
        virtual /* [helpstring][propget][id] */ HRESULT STDMETHODCALLTYPE get_DTE( 
            /* [retval][out] */ __RPC__deref_out_opt /* external definition not present */ DTE **ppDTE) = 0;
        
        virtual /* [helpstring][propget][id] */ HRESULT STDMETHODCALLTYPE get_ContainingProject( 
            /* [retval][out] */ __RPC__deref_out_opt /* external definition not present */ Project **ppProject) = 0;
        
        virtual /* [helpstring][propget][id] */ HRESULT STDMETHODCALLTYPE get_Count( 
            /* [retval][out] */ __RPC__out long *plCount) = 0;
        
        virtual /* [helpstring][id] */ HRESULT STDMETHODCALLTYPE Item( 
            /* [in] */ VARIANT index,
            /* [retval][out] */ __RPC__deref_out_opt WebReference **ppProjectReference) = 0;
        
        virtual /* [id][restricted] */ HRESULT STDMETHODCALLTYPE _NewEnum( 
            /* [retval][out] */ __RPC__deref_out_opt IUnknown **ppiuReturn) = 0;
        
        virtual /* [helpstring][id] */ HRESULT STDMETHODCALLTYPE Add( 
            /* [in] */ __RPC__in BSTR bstrUrl,
            /* [in] */ __RPC__in BSTR bstrNamespace) = 0;
        
        virtual /* [helpstring][id] */ HRESULT STDMETHODCALLTYPE Update( void) = 0;
        
    };
    
    
#else 	/* C style interface */

    typedef struct WebReferencesVtbl
    {
        BEGIN_INTERFACE
        
        HRESULT ( STDMETHODCALLTYPE *QueryInterface )( 
            __RPC__in WebReferences * This,
            /* [in] */ __RPC__in REFIID riid,
            /* [annotation][iid_is][out] */ 
            _COM_Outptr_  void **ppvObject);
        
        ULONG ( STDMETHODCALLTYPE *AddRef )( 
            __RPC__in WebReferences * This);
        
        ULONG ( STDMETHODCALLTYPE *Release )( 
            __RPC__in WebReferences * This);
        
        HRESULT ( STDMETHODCALLTYPE *GetTypeInfoCount )( 
            __RPC__in WebReferences * This,
            /* [out] */ __RPC__out UINT *pctinfo);
        
        HRESULT ( STDMETHODCALLTYPE *GetTypeInfo )( 
            __RPC__in WebReferences * This,
            /* [in] */ UINT iTInfo,
            /* [in] */ LCID lcid,
            /* [out] */ __RPC__deref_out_opt ITypeInfo **ppTInfo);
        
        HRESULT ( STDMETHODCALLTYPE *GetIDsOfNames )( 
            __RPC__in WebReferences * This,
            /* [in] */ __RPC__in REFIID riid,
            /* [size_is][in] */ __RPC__in_ecount_full(cNames) LPOLESTR *rgszNames,
            /* [range][in] */ __RPC__in_range(0,16384) UINT cNames,
            /* [in] */ LCID lcid,
            /* [size_is][out] */ __RPC__out_ecount_full(cNames) DISPID *rgDispId);
        
        /* [local] */ HRESULT ( STDMETHODCALLTYPE *Invoke )( 
            WebReferences * This,
            /* [annotation][in] */ 
            _In_  DISPID dispIdMember,
            /* [annotation][in] */ 
            _In_  REFIID riid,
            /* [annotation][in] */ 
            _In_  LCID lcid,
            /* [annotation][in] */ 
            _In_  WORD wFlags,
            /* [annotation][out][in] */ 
            _In_  DISPPARAMS *pDispParams,
            /* [annotation][out] */ 
            _Out_opt_  VARIANT *pVarResult,
            /* [annotation][out] */ 
            _Out_opt_  EXCEPINFO *pExcepInfo,
            /* [annotation][out] */ 
            _Out_opt_  UINT *puArgErr);
        
        /* [helpstring][propget][id] */ HRESULT ( STDMETHODCALLTYPE *get_DTE )( 
            __RPC__in WebReferences * This,
            /* [retval][out] */ __RPC__deref_out_opt /* external definition not present */ DTE **ppDTE);
        
        /* [helpstring][propget][id] */ HRESULT ( STDMETHODCALLTYPE *get_ContainingProject )( 
            __RPC__in WebReferences * This,
            /* [retval][out] */ __RPC__deref_out_opt /* external definition not present */ Project **ppProject);
        
        /* [helpstring][propget][id] */ HRESULT ( STDMETHODCALLTYPE *get_Count )( 
            __RPC__in WebReferences * This,
            /* [retval][out] */ __RPC__out long *plCount);
        
        /* [helpstring][id] */ HRESULT ( STDMETHODCALLTYPE *Item )( 
            __RPC__in WebReferences * This,
            /* [in] */ VARIANT index,
            /* [retval][out] */ __RPC__deref_out_opt WebReference **ppProjectReference);
        
        /* [id][restricted] */ HRESULT ( STDMETHODCALLTYPE *_NewEnum )( 
            __RPC__in WebReferences * This,
            /* [retval][out] */ __RPC__deref_out_opt IUnknown **ppiuReturn);
        
        /* [helpstring][id] */ HRESULT ( STDMETHODCALLTYPE *Add )( 
            __RPC__in WebReferences * This,
            /* [in] */ __RPC__in BSTR bstrUrl,
            /* [in] */ __RPC__in BSTR bstrNamespace);
        
        /* [helpstring][id] */ HRESULT ( STDMETHODCALLTYPE *Update )( 
            __RPC__in WebReferences * This);
        
        END_INTERFACE
    } WebReferencesVtbl;

    interface WebReferences
    {
        CONST_VTBL struct WebReferencesVtbl *lpVtbl;
    };

    

#ifdef COBJMACROS


#define WebReferences_QueryInterface(This,riid,ppvObject)	\
    ( (This)->lpVtbl -> QueryInterface(This,riid,ppvObject) ) 

#define WebReferences_AddRef(This)	\
    ( (This)->lpVtbl -> AddRef(This) ) 

#define WebReferences_Release(This)	\
    ( (This)->lpVtbl -> Release(This) ) 


#define WebReferences_GetTypeInfoCount(This,pctinfo)	\
    ( (This)->lpVtbl -> GetTypeInfoCount(This,pctinfo) ) 

#define WebReferences_GetTypeInfo(This,iTInfo,lcid,ppTInfo)	\
    ( (This)->lpVtbl -> GetTypeInfo(This,iTInfo,lcid,ppTInfo) ) 

#define WebReferences_GetIDsOfNames(This,riid,rgszNames,cNames,lcid,rgDispId)	\
    ( (This)->lpVtbl -> GetIDsOfNames(This,riid,rgszNames,cNames,lcid,rgDispId) ) 

#define WebReferences_Invoke(This,dispIdMember,riid,lcid,wFlags,pDispParams,pVarResult,pExcepInfo,puArgErr)	\
    ( (This)->lpVtbl -> Invoke(This,dispIdMember,riid,lcid,wFlags,pDispParams,pVarResult,pExcepInfo,puArgErr) ) 


#define WebReferences_get_DTE(This,ppDTE)	\
    ( (This)->lpVtbl -> get_DTE(This,ppDTE) ) 

#define WebReferences_get_ContainingProject(This,ppProject)	\
    ( (This)->lpVtbl -> get_ContainingProject(This,ppProject) ) 

#define WebReferences_get_Count(This,plCount)	\
    ( (This)->lpVtbl -> get_Count(This,plCount) ) 

#define WebReferences_Item(This,index,ppProjectReference)	\
    ( (This)->lpVtbl -> Item(This,index,ppProjectReference) ) 

#define WebReferences__NewEnum(This,ppiuReturn)	\
    ( (This)->lpVtbl -> _NewEnum(This,ppiuReturn) ) 

#define WebReferences_Add(This,bstrUrl,bstrNamespace)	\
    ( (This)->lpVtbl -> Add(This,bstrUrl,bstrNamespace) ) 

#define WebReferences_Update(This)	\
    ( (This)->lpVtbl -> Update(This) ) 

#endif /* COBJMACROS */


#endif 	/* C style interface */




#endif 	/* __WebReferences_INTERFACE_DEFINED__ */


#ifndef ___WebReferencesEvents_INTERFACE_DEFINED__
#define ___WebReferencesEvents_INTERFACE_DEFINED__

/* interface _WebReferencesEvents */
/* [object][dual][uuid] */ 


EXTERN_C const IID IID__WebReferencesEvents;

#if defined(__cplusplus) && !defined(CINTERFACE)
    
    MIDL_INTERFACE("63FE9F71-D793-435e-9F80-FD4082CA1444")
    _WebReferencesEvents : public IDispatch
    {
    public:
    };
    
    
#else 	/* C style interface */

    typedef struct _WebReferencesEventsVtbl
    {
        BEGIN_INTERFACE
        
        HRESULT ( STDMETHODCALLTYPE *QueryInterface )( 
            __RPC__in _WebReferencesEvents * This,
            /* [in] */ __RPC__in REFIID riid,
            /* [annotation][iid_is][out] */ 
            _COM_Outptr_  void **ppvObject);
        
        ULONG ( STDMETHODCALLTYPE *AddRef )( 
            __RPC__in _WebReferencesEvents * This);
        
        ULONG ( STDMETHODCALLTYPE *Release )( 
            __RPC__in _WebReferencesEvents * This);
        
        HRESULT ( STDMETHODCALLTYPE *GetTypeInfoCount )( 
            __RPC__in _WebReferencesEvents * This,
            /* [out] */ __RPC__out UINT *pctinfo);
        
        HRESULT ( STDMETHODCALLTYPE *GetTypeInfo )( 
            __RPC__in _WebReferencesEvents * This,
            /* [in] */ UINT iTInfo,
            /* [in] */ LCID lcid,
            /* [out] */ __RPC__deref_out_opt ITypeInfo **ppTInfo);
        
        HRESULT ( STDMETHODCALLTYPE *GetIDsOfNames )( 
            __RPC__in _WebReferencesEvents * This,
            /* [in] */ __RPC__in REFIID riid,
            /* [size_is][in] */ __RPC__in_ecount_full(cNames) LPOLESTR *rgszNames,
            /* [range][in] */ __RPC__in_range(0,16384) UINT cNames,
            /* [in] */ LCID lcid,
            /* [size_is][out] */ __RPC__out_ecount_full(cNames) DISPID *rgDispId);
        
        /* [local] */ HRESULT ( STDMETHODCALLTYPE *Invoke )( 
            _WebReferencesEvents * This,
            /* [annotation][in] */ 
            _In_  DISPID dispIdMember,
            /* [annotation][in] */ 
            _In_  REFIID riid,
            /* [annotation][in] */ 
            _In_  LCID lcid,
            /* [annotation][in] */ 
            _In_  WORD wFlags,
            /* [annotation][out][in] */ 
            _In_  DISPPARAMS *pDispParams,
            /* [annotation][out] */ 
            _Out_opt_  VARIANT *pVarResult,
            /* [annotation][out] */ 
            _Out_opt_  EXCEPINFO *pExcepInfo,
            /* [annotation][out] */ 
            _Out_opt_  UINT *puArgErr);
        
        END_INTERFACE
    } _WebReferencesEventsVtbl;

    interface _WebReferencesEvents
    {
        CONST_VTBL struct _WebReferencesEventsVtbl *lpVtbl;
    };

    

#ifdef COBJMACROS


#define _WebReferencesEvents_QueryInterface(This,riid,ppvObject)	\
    ( (This)->lpVtbl -> QueryInterface(This,riid,ppvObject) ) 

#define _WebReferencesEvents_AddRef(This)	\
    ( (This)->lpVtbl -> AddRef(This) ) 

#define _WebReferencesEvents_Release(This)	\
    ( (This)->lpVtbl -> Release(This) ) 


#define _WebReferencesEvents_GetTypeInfoCount(This,pctinfo)	\
    ( (This)->lpVtbl -> GetTypeInfoCount(This,pctinfo) ) 

#define _WebReferencesEvents_GetTypeInfo(This,iTInfo,lcid,ppTInfo)	\
    ( (This)->lpVtbl -> GetTypeInfo(This,iTInfo,lcid,ppTInfo) ) 

#define _WebReferencesEvents_GetIDsOfNames(This,riid,rgszNames,cNames,lcid,rgDispId)	\
    ( (This)->lpVtbl -> GetIDsOfNames(This,riid,rgszNames,cNames,lcid,rgDispId) ) 

#define _WebReferencesEvents_Invoke(This,dispIdMember,riid,lcid,wFlags,pDispParams,pVarResult,pExcepInfo,puArgErr)	\
    ( (This)->lpVtbl -> Invoke(This,dispIdMember,riid,lcid,wFlags,pDispParams,pVarResult,pExcepInfo,puArgErr) ) 


#endif /* COBJMACROS */


#endif 	/* C style interface */




#endif 	/* ___WebReferencesEvents_INTERFACE_DEFINED__ */


#ifndef ___dispWebReferencesEvents_DISPINTERFACE_DEFINED__
#define ___dispWebReferencesEvents_DISPINTERFACE_DEFINED__

/* dispinterface _dispWebReferencesEvents */
/* [uuid] */ 


EXTERN_C const IID DIID__dispWebReferencesEvents;

#if defined(__cplusplus) && !defined(CINTERFACE)

    MIDL_INTERFACE("11C784F8-E5A0-4c19-831A-2C2D00C6D897")
    _dispWebReferencesEvents : public IDispatch
    {
    };
    
#else 	/* C style interface */

    typedef struct _dispWebReferencesEventsVtbl
    {
        BEGIN_INTERFACE
        
        HRESULT ( STDMETHODCALLTYPE *QueryInterface )( 
            __RPC__in _dispWebReferencesEvents * This,
            /* [in] */ __RPC__in REFIID riid,
            /* [annotation][iid_is][out] */ 
            _COM_Outptr_  void **ppvObject);
        
        ULONG ( STDMETHODCALLTYPE *AddRef )( 
            __RPC__in _dispWebReferencesEvents * This);
        
        ULONG ( STDMETHODCALLTYPE *Release )( 
            __RPC__in _dispWebReferencesEvents * This);
        
        HRESULT ( STDMETHODCALLTYPE *GetTypeInfoCount )( 
            __RPC__in _dispWebReferencesEvents * This,
            /* [out] */ __RPC__out UINT *pctinfo);
        
        HRESULT ( STDMETHODCALLTYPE *GetTypeInfo )( 
            __RPC__in _dispWebReferencesEvents * This,
            /* [in] */ UINT iTInfo,
            /* [in] */ LCID lcid,
            /* [out] */ __RPC__deref_out_opt ITypeInfo **ppTInfo);
        
        HRESULT ( STDMETHODCALLTYPE *GetIDsOfNames )( 
            __RPC__in _dispWebReferencesEvents * This,
            /* [in] */ __RPC__in REFIID riid,
            /* [size_is][in] */ __RPC__in_ecount_full(cNames) LPOLESTR *rgszNames,
            /* [range][in] */ __RPC__in_range(0,16384) UINT cNames,
            /* [in] */ LCID lcid,
            /* [size_is][out] */ __RPC__out_ecount_full(cNames) DISPID *rgDispId);
        
        /* [local] */ HRESULT ( STDMETHODCALLTYPE *Invoke )( 
            _dispWebReferencesEvents * This,
            /* [annotation][in] */ 
            _In_  DISPID dispIdMember,
            /* [annotation][in] */ 
            _In_  REFIID riid,
            /* [annotation][in] */ 
            _In_  LCID lcid,
            /* [annotation][in] */ 
            _In_  WORD wFlags,
            /* [annotation][out][in] */ 
            _In_  DISPPARAMS *pDispParams,
            /* [annotation][out] */ 
            _Out_opt_  VARIANT *pVarResult,
            /* [annotation][out] */ 
            _Out_opt_  EXCEPINFO *pExcepInfo,
            /* [annotation][out] */ 
            _Out_opt_  UINT *puArgErr);
        
        END_INTERFACE
    } _dispWebReferencesEventsVtbl;

    interface _dispWebReferencesEvents
    {
        CONST_VTBL struct _dispWebReferencesEventsVtbl *lpVtbl;
    };

    

#ifdef COBJMACROS


#define _dispWebReferencesEvents_QueryInterface(This,riid,ppvObject)	\
    ( (This)->lpVtbl -> QueryInterface(This,riid,ppvObject) ) 

#define _dispWebReferencesEvents_AddRef(This)	\
    ( (This)->lpVtbl -> AddRef(This) ) 

#define _dispWebReferencesEvents_Release(This)	\
    ( (This)->lpVtbl -> Release(This) ) 


#define _dispWebReferencesEvents_GetTypeInfoCount(This,pctinfo)	\
    ( (This)->lpVtbl -> GetTypeInfoCount(This,pctinfo) ) 

#define _dispWebReferencesEvents_GetTypeInfo(This,iTInfo,lcid,ppTInfo)	\
    ( (This)->lpVtbl -> GetTypeInfo(This,iTInfo,lcid,ppTInfo) ) 

#define _dispWebReferencesEvents_GetIDsOfNames(This,riid,rgszNames,cNames,lcid,rgDispId)	\
    ( (This)->lpVtbl -> GetIDsOfNames(This,riid,rgszNames,cNames,lcid,rgDispId) ) 

#define _dispWebReferencesEvents_Invoke(This,dispIdMember,riid,lcid,wFlags,pDispParams,pVarResult,pExcepInfo,puArgErr)	\
    ( (This)->lpVtbl -> Invoke(This,dispIdMember,riid,lcid,wFlags,pDispParams,pVarResult,pExcepInfo,puArgErr) ) 

#endif /* COBJMACROS */


#endif 	/* C style interface */


#endif 	/* ___dispWebReferencesEvents_DISPINTERFACE_DEFINED__ */


EXTERN_C const CLSID CLSID_WebReferencesEvents;

#ifdef __cplusplus

class DECLSPEC_UUID("83121F90-51F7-452a-AF12-1EFB69B735D3")
WebReferencesEvents;
#endif

#ifndef __AssemblyReference_INTERFACE_DEFINED__
#define __AssemblyReference_INTERFACE_DEFINED__

/* interface AssemblyReference */
/* [uuid][object][oleautomation][dual] */ 


EXTERN_C const IID IID_AssemblyReference;

#if defined(__cplusplus) && !defined(CINTERFACE)
    
    MIDL_INTERFACE("229F3491-6E60-4e50-90E5-7DB14B0DC004")
    AssemblyReference : public IDispatch
    {
    public:
        virtual /* [helpstring][propget][id] */ HRESULT STDMETHODCALLTYPE get_DTE( 
            /* [retval][out] */ __RPC__deref_out_opt /* external definition not present */ DTE **ppDTE) = 0;
        
        virtual /* [helpstring][propget][id] */ HRESULT STDMETHODCALLTYPE get_ContainingProject( 
            /* [retval][out] */ __RPC__deref_out_opt /* external definition not present */ Project **ppProject) = 0;
        
        virtual /* [helpstring][propget][id] */ HRESULT STDMETHODCALLTYPE get_Name( 
            /* [retval][out] */ __RPC__deref_out_opt BSTR *pbstrName) = 0;
        
        virtual /* [helpstring][id] */ HRESULT STDMETHODCALLTYPE Remove( void) = 0;
        
        virtual /* [helpstring][propget][id] */ HRESULT STDMETHODCALLTYPE get_ReferenceKind( 
            /* [retval][out] */ __RPC__out AssemblyReferenceType *pAssemblyReferenceType) = 0;
        
        virtual /* [helpstring][propget][id] */ HRESULT STDMETHODCALLTYPE get_FullPath( 
            /* [retval][out] */ __RPC__deref_out_opt BSTR *pbstrFullPath) = 0;
        
        virtual /* [helpstring][propget][id] */ HRESULT STDMETHODCALLTYPE get_StrongName( 
            /* [retval][out] */ __RPC__deref_out_opt BSTR *pbstrStrongName) = 0;
        
        virtual /* [helpstring][propget][id] */ HRESULT STDMETHODCALLTYPE get_ReferencedProject( 
            /* [retval][out] */ __RPC__deref_out_opt /* external definition not present */ Project **ppProject) = 0;
        
    };
    
    
#else 	/* C style interface */

    typedef struct AssemblyReferenceVtbl
    {
        BEGIN_INTERFACE
        
        HRESULT ( STDMETHODCALLTYPE *QueryInterface )( 
            __RPC__in AssemblyReference * This,
            /* [in] */ __RPC__in REFIID riid,
            /* [annotation][iid_is][out] */ 
            _COM_Outptr_  void **ppvObject);
        
        ULONG ( STDMETHODCALLTYPE *AddRef )( 
            __RPC__in AssemblyReference * This);
        
        ULONG ( STDMETHODCALLTYPE *Release )( 
            __RPC__in AssemblyReference * This);
        
        HRESULT ( STDMETHODCALLTYPE *GetTypeInfoCount )( 
            __RPC__in AssemblyReference * This,
            /* [out] */ __RPC__out UINT *pctinfo);
        
        HRESULT ( STDMETHODCALLTYPE *GetTypeInfo )( 
            __RPC__in AssemblyReference * This,
            /* [in] */ UINT iTInfo,
            /* [in] */ LCID lcid,
            /* [out] */ __RPC__deref_out_opt ITypeInfo **ppTInfo);
        
        HRESULT ( STDMETHODCALLTYPE *GetIDsOfNames )( 
            __RPC__in AssemblyReference * This,
            /* [in] */ __RPC__in REFIID riid,
            /* [size_is][in] */ __RPC__in_ecount_full(cNames) LPOLESTR *rgszNames,
            /* [range][in] */ __RPC__in_range(0,16384) UINT cNames,
            /* [in] */ LCID lcid,
            /* [size_is][out] */ __RPC__out_ecount_full(cNames) DISPID *rgDispId);
        
        /* [local] */ HRESULT ( STDMETHODCALLTYPE *Invoke )( 
            AssemblyReference * This,
            /* [annotation][in] */ 
            _In_  DISPID dispIdMember,
            /* [annotation][in] */ 
            _In_  REFIID riid,
            /* [annotation][in] */ 
            _In_  LCID lcid,
            /* [annotation][in] */ 
            _In_  WORD wFlags,
            /* [annotation][out][in] */ 
            _In_  DISPPARAMS *pDispParams,
            /* [annotation][out] */ 
            _Out_opt_  VARIANT *pVarResult,
            /* [annotation][out] */ 
            _Out_opt_  EXCEPINFO *pExcepInfo,
            /* [annotation][out] */ 
            _Out_opt_  UINT *puArgErr);
        
        /* [helpstring][propget][id] */ HRESULT ( STDMETHODCALLTYPE *get_DTE )( 
            __RPC__in AssemblyReference * This,
            /* [retval][out] */ __RPC__deref_out_opt /* external definition not present */ DTE **ppDTE);
        
        /* [helpstring][propget][id] */ HRESULT ( STDMETHODCALLTYPE *get_ContainingProject )( 
            __RPC__in AssemblyReference * This,
            /* [retval][out] */ __RPC__deref_out_opt /* external definition not present */ Project **ppProject);
        
        /* [helpstring][propget][id] */ HRESULT ( STDMETHODCALLTYPE *get_Name )( 
            __RPC__in AssemblyReference * This,
            /* [retval][out] */ __RPC__deref_out_opt BSTR *pbstrName);
        
        /* [helpstring][id] */ HRESULT ( STDMETHODCALLTYPE *Remove )( 
            __RPC__in AssemblyReference * This);
        
        /* [helpstring][propget][id] */ HRESULT ( STDMETHODCALLTYPE *get_ReferenceKind )( 
            __RPC__in AssemblyReference * This,
            /* [retval][out] */ __RPC__out AssemblyReferenceType *pAssemblyReferenceType);
        
        /* [helpstring][propget][id] */ HRESULT ( STDMETHODCALLTYPE *get_FullPath )( 
            __RPC__in AssemblyReference * This,
            /* [retval][out] */ __RPC__deref_out_opt BSTR *pbstrFullPath);
        
        /* [helpstring][propget][id] */ HRESULT ( STDMETHODCALLTYPE *get_StrongName )( 
            __RPC__in AssemblyReference * This,
            /* [retval][out] */ __RPC__deref_out_opt BSTR *pbstrStrongName);
        
        /* [helpstring][propget][id] */ HRESULT ( STDMETHODCALLTYPE *get_ReferencedProject )( 
            __RPC__in AssemblyReference * This,
            /* [retval][out] */ __RPC__deref_out_opt /* external definition not present */ Project **ppProject);
        
        END_INTERFACE
    } AssemblyReferenceVtbl;

    interface AssemblyReference
    {
        CONST_VTBL struct AssemblyReferenceVtbl *lpVtbl;
    };

    

#ifdef COBJMACROS


#define AssemblyReference_QueryInterface(This,riid,ppvObject)	\
    ( (This)->lpVtbl -> QueryInterface(This,riid,ppvObject) ) 

#define AssemblyReference_AddRef(This)	\
    ( (This)->lpVtbl -> AddRef(This) ) 

#define AssemblyReference_Release(This)	\
    ( (This)->lpVtbl -> Release(This) ) 


#define AssemblyReference_GetTypeInfoCount(This,pctinfo)	\
    ( (This)->lpVtbl -> GetTypeInfoCount(This,pctinfo) ) 

#define AssemblyReference_GetTypeInfo(This,iTInfo,lcid,ppTInfo)	\
    ( (This)->lpVtbl -> GetTypeInfo(This,iTInfo,lcid,ppTInfo) ) 

#define AssemblyReference_GetIDsOfNames(This,riid,rgszNames,cNames,lcid,rgDispId)	\
    ( (This)->lpVtbl -> GetIDsOfNames(This,riid,rgszNames,cNames,lcid,rgDispId) ) 

#define AssemblyReference_Invoke(This,dispIdMember,riid,lcid,wFlags,pDispParams,pVarResult,pExcepInfo,puArgErr)	\
    ( (This)->lpVtbl -> Invoke(This,dispIdMember,riid,lcid,wFlags,pDispParams,pVarResult,pExcepInfo,puArgErr) ) 


#define AssemblyReference_get_DTE(This,ppDTE)	\
    ( (This)->lpVtbl -> get_DTE(This,ppDTE) ) 

#define AssemblyReference_get_ContainingProject(This,ppProject)	\
    ( (This)->lpVtbl -> get_ContainingProject(This,ppProject) ) 

#define AssemblyReference_get_Name(This,pbstrName)	\
    ( (This)->lpVtbl -> get_Name(This,pbstrName) ) 

#define AssemblyReference_Remove(This)	\
    ( (This)->lpVtbl -> Remove(This) ) 

#define AssemblyReference_get_ReferenceKind(This,pAssemblyReferenceType)	\
    ( (This)->lpVtbl -> get_ReferenceKind(This,pAssemblyReferenceType) ) 

#define AssemblyReference_get_FullPath(This,pbstrFullPath)	\
    ( (This)->lpVtbl -> get_FullPath(This,pbstrFullPath) ) 

#define AssemblyReference_get_StrongName(This,pbstrStrongName)	\
    ( (This)->lpVtbl -> get_StrongName(This,pbstrStrongName) ) 

#define AssemblyReference_get_ReferencedProject(This,ppProject)	\
    ( (This)->lpVtbl -> get_ReferencedProject(This,ppProject) ) 

#endif /* COBJMACROS */


#endif 	/* C style interface */




#endif 	/* __AssemblyReference_INTERFACE_DEFINED__ */


#ifndef __AssemblyReferences_INTERFACE_DEFINED__
#define __AssemblyReferences_INTERFACE_DEFINED__

/* interface AssemblyReferences */
/* [uuid][object][oleautomation][dual] */ 


EXTERN_C const IID IID_AssemblyReferences;

#if defined(__cplusplus) && !defined(CINTERFACE)
    
    MIDL_INTERFACE("2C264A1A-DBFB-43fe-9434-997B5BE0FCCC")
    AssemblyReferences : public IDispatch
    {
    public:
        virtual /* [helpstring][propget][id] */ HRESULT STDMETHODCALLTYPE get_DTE( 
            /* [retval][out] */ __RPC__deref_out_opt /* external definition not present */ DTE **ppDTE) = 0;
        
        virtual /* [helpstring][propget][id] */ HRESULT STDMETHODCALLTYPE get_ContainingProject( 
            /* [retval][out] */ __RPC__deref_out_opt /* external definition not present */ Project **ppProject) = 0;
        
        virtual /* [helpstring][propget][id] */ HRESULT STDMETHODCALLTYPE get_Count( 
            /* [retval][out] */ __RPC__out long *plCount) = 0;
        
        virtual /* [helpstring][id] */ HRESULT STDMETHODCALLTYPE Item( 
            /* [in] */ VARIANT index,
            /* [retval][out] */ __RPC__deref_out_opt AssemblyReference **ppProjectReference) = 0;
        
        virtual /* [id][restricted] */ HRESULT STDMETHODCALLTYPE _NewEnum( 
            /* [retval][out] */ __RPC__deref_out_opt IUnknown **ppiuReturn) = 0;
        
        virtual /* [helpstring][id] */ HRESULT STDMETHODCALLTYPE AddFromGAC( 
            /* [in] */ __RPC__in BSTR bstrAssemblyName,
            /* [retval][out] */ __RPC__deref_out_opt AssemblyReference **ppAssemblyReference) = 0;
        
        virtual /* [helpstring][id] */ HRESULT STDMETHODCALLTYPE AddFromFile( 
            /* [in] */ __RPC__in BSTR bstrPath,
            /* [retval][out] */ __RPC__deref_out_opt AssemblyReference **ppAssemblyReference) = 0;
        
        virtual /* [helpstring][id] */ HRESULT STDMETHODCALLTYPE AddFromProject( 
            /* [in] */ __RPC__in /* external definition not present */ Project *pProj) = 0;
        
    };
    
    
#else 	/* C style interface */

    typedef struct AssemblyReferencesVtbl
    {
        BEGIN_INTERFACE
        
        HRESULT ( STDMETHODCALLTYPE *QueryInterface )( 
            __RPC__in AssemblyReferences * This,
            /* [in] */ __RPC__in REFIID riid,
            /* [annotation][iid_is][out] */ 
            _COM_Outptr_  void **ppvObject);
        
        ULONG ( STDMETHODCALLTYPE *AddRef )( 
            __RPC__in AssemblyReferences * This);
        
        ULONG ( STDMETHODCALLTYPE *Release )( 
            __RPC__in AssemblyReferences * This);
        
        HRESULT ( STDMETHODCALLTYPE *GetTypeInfoCount )( 
            __RPC__in AssemblyReferences * This,
            /* [out] */ __RPC__out UINT *pctinfo);
        
        HRESULT ( STDMETHODCALLTYPE *GetTypeInfo )( 
            __RPC__in AssemblyReferences * This,
            /* [in] */ UINT iTInfo,
            /* [in] */ LCID lcid,
            /* [out] */ __RPC__deref_out_opt ITypeInfo **ppTInfo);
        
        HRESULT ( STDMETHODCALLTYPE *GetIDsOfNames )( 
            __RPC__in AssemblyReferences * This,
            /* [in] */ __RPC__in REFIID riid,
            /* [size_is][in] */ __RPC__in_ecount_full(cNames) LPOLESTR *rgszNames,
            /* [range][in] */ __RPC__in_range(0,16384) UINT cNames,
            /* [in] */ LCID lcid,
            /* [size_is][out] */ __RPC__out_ecount_full(cNames) DISPID *rgDispId);
        
        /* [local] */ HRESULT ( STDMETHODCALLTYPE *Invoke )( 
            AssemblyReferences * This,
            /* [annotation][in] */ 
            _In_  DISPID dispIdMember,
            /* [annotation][in] */ 
            _In_  REFIID riid,
            /* [annotation][in] */ 
            _In_  LCID lcid,
            /* [annotation][in] */ 
            _In_  WORD wFlags,
            /* [annotation][out][in] */ 
            _In_  DISPPARAMS *pDispParams,
            /* [annotation][out] */ 
            _Out_opt_  VARIANT *pVarResult,
            /* [annotation][out] */ 
            _Out_opt_  EXCEPINFO *pExcepInfo,
            /* [annotation][out] */ 
            _Out_opt_  UINT *puArgErr);
        
        /* [helpstring][propget][id] */ HRESULT ( STDMETHODCALLTYPE *get_DTE )( 
            __RPC__in AssemblyReferences * This,
            /* [retval][out] */ __RPC__deref_out_opt /* external definition not present */ DTE **ppDTE);
        
        /* [helpstring][propget][id] */ HRESULT ( STDMETHODCALLTYPE *get_ContainingProject )( 
            __RPC__in AssemblyReferences * This,
            /* [retval][out] */ __RPC__deref_out_opt /* external definition not present */ Project **ppProject);
        
        /* [helpstring][propget][id] */ HRESULT ( STDMETHODCALLTYPE *get_Count )( 
            __RPC__in AssemblyReferences * This,
            /* [retval][out] */ __RPC__out long *plCount);
        
        /* [helpstring][id] */ HRESULT ( STDMETHODCALLTYPE *Item )( 
            __RPC__in AssemblyReferences * This,
            /* [in] */ VARIANT index,
            /* [retval][out] */ __RPC__deref_out_opt AssemblyReference **ppProjectReference);
        
        /* [id][restricted] */ HRESULT ( STDMETHODCALLTYPE *_NewEnum )( 
            __RPC__in AssemblyReferences * This,
            /* [retval][out] */ __RPC__deref_out_opt IUnknown **ppiuReturn);
        
        /* [helpstring][id] */ HRESULT ( STDMETHODCALLTYPE *AddFromGAC )( 
            __RPC__in AssemblyReferences * This,
            /* [in] */ __RPC__in BSTR bstrAssemblyName,
            /* [retval][out] */ __RPC__deref_out_opt AssemblyReference **ppAssemblyReference);
        
        /* [helpstring][id] */ HRESULT ( STDMETHODCALLTYPE *AddFromFile )( 
            __RPC__in AssemblyReferences * This,
            /* [in] */ __RPC__in BSTR bstrPath,
            /* [retval][out] */ __RPC__deref_out_opt AssemblyReference **ppAssemblyReference);
        
        /* [helpstring][id] */ HRESULT ( STDMETHODCALLTYPE *AddFromProject )( 
            __RPC__in AssemblyReferences * This,
            /* [in] */ __RPC__in /* external definition not present */ Project *pProj);
        
        END_INTERFACE
    } AssemblyReferencesVtbl;

    interface AssemblyReferences
    {
        CONST_VTBL struct AssemblyReferencesVtbl *lpVtbl;
    };

    

#ifdef COBJMACROS


#define AssemblyReferences_QueryInterface(This,riid,ppvObject)	\
    ( (This)->lpVtbl -> QueryInterface(This,riid,ppvObject) ) 

#define AssemblyReferences_AddRef(This)	\
    ( (This)->lpVtbl -> AddRef(This) ) 

#define AssemblyReferences_Release(This)	\
    ( (This)->lpVtbl -> Release(This) ) 


#define AssemblyReferences_GetTypeInfoCount(This,pctinfo)	\
    ( (This)->lpVtbl -> GetTypeInfoCount(This,pctinfo) ) 

#define AssemblyReferences_GetTypeInfo(This,iTInfo,lcid,ppTInfo)	\
    ( (This)->lpVtbl -> GetTypeInfo(This,iTInfo,lcid,ppTInfo) ) 

#define AssemblyReferences_GetIDsOfNames(This,riid,rgszNames,cNames,lcid,rgDispId)	\
    ( (This)->lpVtbl -> GetIDsOfNames(This,riid,rgszNames,cNames,lcid,rgDispId) ) 

#define AssemblyReferences_Invoke(This,dispIdMember,riid,lcid,wFlags,pDispParams,pVarResult,pExcepInfo,puArgErr)	\
    ( (This)->lpVtbl -> Invoke(This,dispIdMember,riid,lcid,wFlags,pDispParams,pVarResult,pExcepInfo,puArgErr) ) 


#define AssemblyReferences_get_DTE(This,ppDTE)	\
    ( (This)->lpVtbl -> get_DTE(This,ppDTE) ) 

#define AssemblyReferences_get_ContainingProject(This,ppProject)	\
    ( (This)->lpVtbl -> get_ContainingProject(This,ppProject) ) 

#define AssemblyReferences_get_Count(This,plCount)	\
    ( (This)->lpVtbl -> get_Count(This,plCount) ) 

#define AssemblyReferences_Item(This,index,ppProjectReference)	\
    ( (This)->lpVtbl -> Item(This,index,ppProjectReference) ) 

#define AssemblyReferences__NewEnum(This,ppiuReturn)	\
    ( (This)->lpVtbl -> _NewEnum(This,ppiuReturn) ) 

#define AssemblyReferences_AddFromGAC(This,bstrAssemblyName,ppAssemblyReference)	\
    ( (This)->lpVtbl -> AddFromGAC(This,bstrAssemblyName,ppAssemblyReference) ) 

#define AssemblyReferences_AddFromFile(This,bstrPath,ppAssemblyReference)	\
    ( (This)->lpVtbl -> AddFromFile(This,bstrPath,ppAssemblyReference) ) 

#define AssemblyReferences_AddFromProject(This,pProj)	\
    ( (This)->lpVtbl -> AddFromProject(This,pProj) ) 

#endif /* COBJMACROS */


#endif 	/* C style interface */




#endif 	/* __AssemblyReferences_INTERFACE_DEFINED__ */


#ifndef ___AssemblyReferencesEvents_INTERFACE_DEFINED__
#define ___AssemblyReferencesEvents_INTERFACE_DEFINED__

/* interface _AssemblyReferencesEvents */
/* [object][dual][uuid] */ 


EXTERN_C const IID IID__AssemblyReferencesEvents;

#if defined(__cplusplus) && !defined(CINTERFACE)
    
    MIDL_INTERFACE("E5E56972-0CFE-49b7-9EFB-923613FDA978")
    _AssemblyReferencesEvents : public IDispatch
    {
    public:
    };
    
    
#else 	/* C style interface */

    typedef struct _AssemblyReferencesEventsVtbl
    {
        BEGIN_INTERFACE
        
        HRESULT ( STDMETHODCALLTYPE *QueryInterface )( 
            __RPC__in _AssemblyReferencesEvents * This,
            /* [in] */ __RPC__in REFIID riid,
            /* [annotation][iid_is][out] */ 
            _COM_Outptr_  void **ppvObject);
        
        ULONG ( STDMETHODCALLTYPE *AddRef )( 
            __RPC__in _AssemblyReferencesEvents * This);
        
        ULONG ( STDMETHODCALLTYPE *Release )( 
            __RPC__in _AssemblyReferencesEvents * This);
        
        HRESULT ( STDMETHODCALLTYPE *GetTypeInfoCount )( 
            __RPC__in _AssemblyReferencesEvents * This,
            /* [out] */ __RPC__out UINT *pctinfo);
        
        HRESULT ( STDMETHODCALLTYPE *GetTypeInfo )( 
            __RPC__in _AssemblyReferencesEvents * This,
            /* [in] */ UINT iTInfo,
            /* [in] */ LCID lcid,
            /* [out] */ __RPC__deref_out_opt ITypeInfo **ppTInfo);
        
        HRESULT ( STDMETHODCALLTYPE *GetIDsOfNames )( 
            __RPC__in _AssemblyReferencesEvents * This,
            /* [in] */ __RPC__in REFIID riid,
            /* [size_is][in] */ __RPC__in_ecount_full(cNames) LPOLESTR *rgszNames,
            /* [range][in] */ __RPC__in_range(0,16384) UINT cNames,
            /* [in] */ LCID lcid,
            /* [size_is][out] */ __RPC__out_ecount_full(cNames) DISPID *rgDispId);
        
        /* [local] */ HRESULT ( STDMETHODCALLTYPE *Invoke )( 
            _AssemblyReferencesEvents * This,
            /* [annotation][in] */ 
            _In_  DISPID dispIdMember,
            /* [annotation][in] */ 
            _In_  REFIID riid,
            /* [annotation][in] */ 
            _In_  LCID lcid,
            /* [annotation][in] */ 
            _In_  WORD wFlags,
            /* [annotation][out][in] */ 
            _In_  DISPPARAMS *pDispParams,
            /* [annotation][out] */ 
            _Out_opt_  VARIANT *pVarResult,
            /* [annotation][out] */ 
            _Out_opt_  EXCEPINFO *pExcepInfo,
            /* [annotation][out] */ 
            _Out_opt_  UINT *puArgErr);
        
        END_INTERFACE
    } _AssemblyReferencesEventsVtbl;

    interface _AssemblyReferencesEvents
    {
        CONST_VTBL struct _AssemblyReferencesEventsVtbl *lpVtbl;
    };

    

#ifdef COBJMACROS


#define _AssemblyReferencesEvents_QueryInterface(This,riid,ppvObject)	\
    ( (This)->lpVtbl -> QueryInterface(This,riid,ppvObject) ) 

#define _AssemblyReferencesEvents_AddRef(This)	\
    ( (This)->lpVtbl -> AddRef(This) ) 

#define _AssemblyReferencesEvents_Release(This)	\
    ( (This)->lpVtbl -> Release(This) ) 


#define _AssemblyReferencesEvents_GetTypeInfoCount(This,pctinfo)	\
    ( (This)->lpVtbl -> GetTypeInfoCount(This,pctinfo) ) 

#define _AssemblyReferencesEvents_GetTypeInfo(This,iTInfo,lcid,ppTInfo)	\
    ( (This)->lpVtbl -> GetTypeInfo(This,iTInfo,lcid,ppTInfo) ) 

#define _AssemblyReferencesEvents_GetIDsOfNames(This,riid,rgszNames,cNames,lcid,rgDispId)	\
    ( (This)->lpVtbl -> GetIDsOfNames(This,riid,rgszNames,cNames,lcid,rgDispId) ) 

#define _AssemblyReferencesEvents_Invoke(This,dispIdMember,riid,lcid,wFlags,pDispParams,pVarResult,pExcepInfo,puArgErr)	\
    ( (This)->lpVtbl -> Invoke(This,dispIdMember,riid,lcid,wFlags,pDispParams,pVarResult,pExcepInfo,puArgErr) ) 


#endif /* COBJMACROS */


#endif 	/* C style interface */




#endif 	/* ___AssemblyReferencesEvents_INTERFACE_DEFINED__ */


#ifndef ___dispAssemblyReferencesEvents_DISPINTERFACE_DEFINED__
#define ___dispAssemblyReferencesEvents_DISPINTERFACE_DEFINED__

/* dispinterface _dispAssemblyReferencesEvents */
/* [uuid] */ 


EXTERN_C const IID DIID__dispAssemblyReferencesEvents;

#if defined(__cplusplus) && !defined(CINTERFACE)

    MIDL_INTERFACE("A1B04977-56DD-40d1-89E3-65E1902A9103")
    _dispAssemblyReferencesEvents : public IDispatch
    {
    };
    
#else 	/* C style interface */

    typedef struct _dispAssemblyReferencesEventsVtbl
    {
        BEGIN_INTERFACE
        
        HRESULT ( STDMETHODCALLTYPE *QueryInterface )( 
            __RPC__in _dispAssemblyReferencesEvents * This,
            /* [in] */ __RPC__in REFIID riid,
            /* [annotation][iid_is][out] */ 
            _COM_Outptr_  void **ppvObject);
        
        ULONG ( STDMETHODCALLTYPE *AddRef )( 
            __RPC__in _dispAssemblyReferencesEvents * This);
        
        ULONG ( STDMETHODCALLTYPE *Release )( 
            __RPC__in _dispAssemblyReferencesEvents * This);
        
        HRESULT ( STDMETHODCALLTYPE *GetTypeInfoCount )( 
            __RPC__in _dispAssemblyReferencesEvents * This,
            /* [out] */ __RPC__out UINT *pctinfo);
        
        HRESULT ( STDMETHODCALLTYPE *GetTypeInfo )( 
            __RPC__in _dispAssemblyReferencesEvents * This,
            /* [in] */ UINT iTInfo,
            /* [in] */ LCID lcid,
            /* [out] */ __RPC__deref_out_opt ITypeInfo **ppTInfo);
        
        HRESULT ( STDMETHODCALLTYPE *GetIDsOfNames )( 
            __RPC__in _dispAssemblyReferencesEvents * This,
            /* [in] */ __RPC__in REFIID riid,
            /* [size_is][in] */ __RPC__in_ecount_full(cNames) LPOLESTR *rgszNames,
            /* [range][in] */ __RPC__in_range(0,16384) UINT cNames,
            /* [in] */ LCID lcid,
            /* [size_is][out] */ __RPC__out_ecount_full(cNames) DISPID *rgDispId);
        
        /* [local] */ HRESULT ( STDMETHODCALLTYPE *Invoke )( 
            _dispAssemblyReferencesEvents * This,
            /* [annotation][in] */ 
            _In_  DISPID dispIdMember,
            /* [annotation][in] */ 
            _In_  REFIID riid,
            /* [annotation][in] */ 
            _In_  LCID lcid,
            /* [annotation][in] */ 
            _In_  WORD wFlags,
            /* [annotation][out][in] */ 
            _In_  DISPPARAMS *pDispParams,
            /* [annotation][out] */ 
            _Out_opt_  VARIANT *pVarResult,
            /* [annotation][out] */ 
            _Out_opt_  EXCEPINFO *pExcepInfo,
            /* [annotation][out] */ 
            _Out_opt_  UINT *puArgErr);
        
        END_INTERFACE
    } _dispAssemblyReferencesEventsVtbl;

    interface _dispAssemblyReferencesEvents
    {
        CONST_VTBL struct _dispAssemblyReferencesEventsVtbl *lpVtbl;
    };

    

#ifdef COBJMACROS


#define _dispAssemblyReferencesEvents_QueryInterface(This,riid,ppvObject)	\
    ( (This)->lpVtbl -> QueryInterface(This,riid,ppvObject) ) 

#define _dispAssemblyReferencesEvents_AddRef(This)	\
    ( (This)->lpVtbl -> AddRef(This) ) 

#define _dispAssemblyReferencesEvents_Release(This)	\
    ( (This)->lpVtbl -> Release(This) ) 


#define _dispAssemblyReferencesEvents_GetTypeInfoCount(This,pctinfo)	\
    ( (This)->lpVtbl -> GetTypeInfoCount(This,pctinfo) ) 

#define _dispAssemblyReferencesEvents_GetTypeInfo(This,iTInfo,lcid,ppTInfo)	\
    ( (This)->lpVtbl -> GetTypeInfo(This,iTInfo,lcid,ppTInfo) ) 

#define _dispAssemblyReferencesEvents_GetIDsOfNames(This,riid,rgszNames,cNames,lcid,rgDispId)	\
    ( (This)->lpVtbl -> GetIDsOfNames(This,riid,rgszNames,cNames,lcid,rgDispId) ) 

#define _dispAssemblyReferencesEvents_Invoke(This,dispIdMember,riid,lcid,wFlags,pDispParams,pVarResult,pExcepInfo,puArgErr)	\
    ( (This)->lpVtbl -> Invoke(This,dispIdMember,riid,lcid,wFlags,pDispParams,pVarResult,pExcepInfo,puArgErr) ) 

#endif /* COBJMACROS */


#endif 	/* C style interface */


#endif 	/* ___dispAssemblyReferencesEvents_DISPINTERFACE_DEFINED__ */


EXTERN_C const CLSID CLSID_AssemblyReferencesEvents;

#ifdef __cplusplus

class DECLSPEC_UUID("F11526E7-4102-4070-9B60-BD4F5CD3006B")
AssemblyReferencesEvents;
#endif

#ifndef ___WebServicesEvents_INTERFACE_DEFINED__
#define ___WebServicesEvents_INTERFACE_DEFINED__

/* interface _WebServicesEvents */
/* [object][dual][uuid] */ 


EXTERN_C const IID IID__WebServicesEvents;

#if defined(__cplusplus) && !defined(CINTERFACE)
    
    MIDL_INTERFACE("E7E27BE0-FF6A-4d94-9B1B-D67F01D1E0FE")
    _WebServicesEvents : public IDispatch
    {
    public:
    };
    
    
#else 	/* C style interface */

    typedef struct _WebServicesEventsVtbl
    {
        BEGIN_INTERFACE
        
        HRESULT ( STDMETHODCALLTYPE *QueryInterface )( 
            __RPC__in _WebServicesEvents * This,
            /* [in] */ __RPC__in REFIID riid,
            /* [annotation][iid_is][out] */ 
            _COM_Outptr_  void **ppvObject);
        
        ULONG ( STDMETHODCALLTYPE *AddRef )( 
            __RPC__in _WebServicesEvents * This);
        
        ULONG ( STDMETHODCALLTYPE *Release )( 
            __RPC__in _WebServicesEvents * This);
        
        HRESULT ( STDMETHODCALLTYPE *GetTypeInfoCount )( 
            __RPC__in _WebServicesEvents * This,
            /* [out] */ __RPC__out UINT *pctinfo);
        
        HRESULT ( STDMETHODCALLTYPE *GetTypeInfo )( 
            __RPC__in _WebServicesEvents * This,
            /* [in] */ UINT iTInfo,
            /* [in] */ LCID lcid,
            /* [out] */ __RPC__deref_out_opt ITypeInfo **ppTInfo);
        
        HRESULT ( STDMETHODCALLTYPE *GetIDsOfNames )( 
            __RPC__in _WebServicesEvents * This,
            /* [in] */ __RPC__in REFIID riid,
            /* [size_is][in] */ __RPC__in_ecount_full(cNames) LPOLESTR *rgszNames,
            /* [range][in] */ __RPC__in_range(0,16384) UINT cNames,
            /* [in] */ LCID lcid,
            /* [size_is][out] */ __RPC__out_ecount_full(cNames) DISPID *rgDispId);
        
        /* [local] */ HRESULT ( STDMETHODCALLTYPE *Invoke )( 
            _WebServicesEvents * This,
            /* [annotation][in] */ 
            _In_  DISPID dispIdMember,
            /* [annotation][in] */ 
            _In_  REFIID riid,
            /* [annotation][in] */ 
            _In_  LCID lcid,
            /* [annotation][in] */ 
            _In_  WORD wFlags,
            /* [annotation][out][in] */ 
            _In_  DISPPARAMS *pDispParams,
            /* [annotation][out] */ 
            _Out_opt_  VARIANT *pVarResult,
            /* [annotation][out] */ 
            _Out_opt_  EXCEPINFO *pExcepInfo,
            /* [annotation][out] */ 
            _Out_opt_  UINT *puArgErr);
        
        END_INTERFACE
    } _WebServicesEventsVtbl;

    interface _WebServicesEvents
    {
        CONST_VTBL struct _WebServicesEventsVtbl *lpVtbl;
    };

    

#ifdef COBJMACROS


#define _WebServicesEvents_QueryInterface(This,riid,ppvObject)	\
    ( (This)->lpVtbl -> QueryInterface(This,riid,ppvObject) ) 

#define _WebServicesEvents_AddRef(This)	\
    ( (This)->lpVtbl -> AddRef(This) ) 

#define _WebServicesEvents_Release(This)	\
    ( (This)->lpVtbl -> Release(This) ) 


#define _WebServicesEvents_GetTypeInfoCount(This,pctinfo)	\
    ( (This)->lpVtbl -> GetTypeInfoCount(This,pctinfo) ) 

#define _WebServicesEvents_GetTypeInfo(This,iTInfo,lcid,ppTInfo)	\
    ( (This)->lpVtbl -> GetTypeInfo(This,iTInfo,lcid,ppTInfo) ) 

#define _WebServicesEvents_GetIDsOfNames(This,riid,rgszNames,cNames,lcid,rgDispId)	\
    ( (This)->lpVtbl -> GetIDsOfNames(This,riid,rgszNames,cNames,lcid,rgDispId) ) 

#define _WebServicesEvents_Invoke(This,dispIdMember,riid,lcid,wFlags,pDispParams,pVarResult,pExcepInfo,puArgErr)	\
    ( (This)->lpVtbl -> Invoke(This,dispIdMember,riid,lcid,wFlags,pDispParams,pVarResult,pExcepInfo,puArgErr) ) 


#endif /* COBJMACROS */


#endif 	/* C style interface */




#endif 	/* ___WebServicesEvents_INTERFACE_DEFINED__ */


#ifndef ___dispWebServicesEvents_DISPINTERFACE_DEFINED__
#define ___dispWebServicesEvents_DISPINTERFACE_DEFINED__

/* dispinterface _dispWebServicesEvents */
/* [uuid] */ 


EXTERN_C const IID DIID__dispWebServicesEvents;

#if defined(__cplusplus) && !defined(CINTERFACE)

    MIDL_INTERFACE("B3AF808B-B55A-4dfe-A9FC-C3B4F4B37903")
    _dispWebServicesEvents : public IDispatch
    {
    };
    
#else 	/* C style interface */

    typedef struct _dispWebServicesEventsVtbl
    {
        BEGIN_INTERFACE
        
        HRESULT ( STDMETHODCALLTYPE *QueryInterface )( 
            __RPC__in _dispWebServicesEvents * This,
            /* [in] */ __RPC__in REFIID riid,
            /* [annotation][iid_is][out] */ 
            _COM_Outptr_  void **ppvObject);
        
        ULONG ( STDMETHODCALLTYPE *AddRef )( 
            __RPC__in _dispWebServicesEvents * This);
        
        ULONG ( STDMETHODCALLTYPE *Release )( 
            __RPC__in _dispWebServicesEvents * This);
        
        HRESULT ( STDMETHODCALLTYPE *GetTypeInfoCount )( 
            __RPC__in _dispWebServicesEvents * This,
            /* [out] */ __RPC__out UINT *pctinfo);
        
        HRESULT ( STDMETHODCALLTYPE *GetTypeInfo )( 
            __RPC__in _dispWebServicesEvents * This,
            /* [in] */ UINT iTInfo,
            /* [in] */ LCID lcid,
            /* [out] */ __RPC__deref_out_opt ITypeInfo **ppTInfo);
        
        HRESULT ( STDMETHODCALLTYPE *GetIDsOfNames )( 
            __RPC__in _dispWebServicesEvents * This,
            /* [in] */ __RPC__in REFIID riid,
            /* [size_is][in] */ __RPC__in_ecount_full(cNames) LPOLESTR *rgszNames,
            /* [range][in] */ __RPC__in_range(0,16384) UINT cNames,
            /* [in] */ LCID lcid,
            /* [size_is][out] */ __RPC__out_ecount_full(cNames) DISPID *rgDispId);
        
        /* [local] */ HRESULT ( STDMETHODCALLTYPE *Invoke )( 
            _dispWebServicesEvents * This,
            /* [annotation][in] */ 
            _In_  DISPID dispIdMember,
            /* [annotation][in] */ 
            _In_  REFIID riid,
            /* [annotation][in] */ 
            _In_  LCID lcid,
            /* [annotation][in] */ 
            _In_  WORD wFlags,
            /* [annotation][out][in] */ 
            _In_  DISPPARAMS *pDispParams,
            /* [annotation][out] */ 
            _Out_opt_  VARIANT *pVarResult,
            /* [annotation][out] */ 
            _Out_opt_  EXCEPINFO *pExcepInfo,
            /* [annotation][out] */ 
            _Out_opt_  UINT *puArgErr);
        
        END_INTERFACE
    } _dispWebServicesEventsVtbl;

    interface _dispWebServicesEvents
    {
        CONST_VTBL struct _dispWebServicesEventsVtbl *lpVtbl;
    };

    

#ifdef COBJMACROS


#define _dispWebServicesEvents_QueryInterface(This,riid,ppvObject)	\
    ( (This)->lpVtbl -> QueryInterface(This,riid,ppvObject) ) 

#define _dispWebServicesEvents_AddRef(This)	\
    ( (This)->lpVtbl -> AddRef(This) ) 

#define _dispWebServicesEvents_Release(This)	\
    ( (This)->lpVtbl -> Release(This) ) 


#define _dispWebServicesEvents_GetTypeInfoCount(This,pctinfo)	\
    ( (This)->lpVtbl -> GetTypeInfoCount(This,pctinfo) ) 

#define _dispWebServicesEvents_GetTypeInfo(This,iTInfo,lcid,ppTInfo)	\
    ( (This)->lpVtbl -> GetTypeInfo(This,iTInfo,lcid,ppTInfo) ) 

#define _dispWebServicesEvents_GetIDsOfNames(This,riid,rgszNames,cNames,lcid,rgDispId)	\
    ( (This)->lpVtbl -> GetIDsOfNames(This,riid,rgszNames,cNames,lcid,rgDispId) ) 

#define _dispWebServicesEvents_Invoke(This,dispIdMember,riid,lcid,wFlags,pDispParams,pVarResult,pExcepInfo,puArgErr)	\
    ( (This)->lpVtbl -> Invoke(This,dispIdMember,riid,lcid,wFlags,pDispParams,pVarResult,pExcepInfo,puArgErr) ) 

#endif /* COBJMACROS */


#endif 	/* C style interface */


#endif 	/* ___dispWebServicesEvents_DISPINTERFACE_DEFINED__ */


EXTERN_C const CLSID CLSID_WebServicesEvents;

#ifdef __cplusplus

class DECLSPEC_UUID("F4DD7750-F662-4430-AB7C-74F9E8EA93BF")
WebServicesEvents;
#endif

#ifndef ___WebSiteMiscEvents_INTERFACE_DEFINED__
#define ___WebSiteMiscEvents_INTERFACE_DEFINED__

/* interface _WebSiteMiscEvents */
/* [object][dual][uuid] */ 


EXTERN_C const IID IID__WebSiteMiscEvents;

#if defined(__cplusplus) && !defined(CINTERFACE)
    
    MIDL_INTERFACE("9D9D4002-3258-4ebb-BF58-AF62C3890990")
    _WebSiteMiscEvents : public IDispatch
    {
    public:
    };
    
    
#else 	/* C style interface */

    typedef struct _WebSiteMiscEventsVtbl
    {
        BEGIN_INTERFACE
        
        HRESULT ( STDMETHODCALLTYPE *QueryInterface )( 
            __RPC__in _WebSiteMiscEvents * This,
            /* [in] */ __RPC__in REFIID riid,
            /* [annotation][iid_is][out] */ 
            _COM_Outptr_  void **ppvObject);
        
        ULONG ( STDMETHODCALLTYPE *AddRef )( 
            __RPC__in _WebSiteMiscEvents * This);
        
        ULONG ( STDMETHODCALLTYPE *Release )( 
            __RPC__in _WebSiteMiscEvents * This);
        
        HRESULT ( STDMETHODCALLTYPE *GetTypeInfoCount )( 
            __RPC__in _WebSiteMiscEvents * This,
            /* [out] */ __RPC__out UINT *pctinfo);
        
        HRESULT ( STDMETHODCALLTYPE *GetTypeInfo )( 
            __RPC__in _WebSiteMiscEvents * This,
            /* [in] */ UINT iTInfo,
            /* [in] */ LCID lcid,
            /* [out] */ __RPC__deref_out_opt ITypeInfo **ppTInfo);
        
        HRESULT ( STDMETHODCALLTYPE *GetIDsOfNames )( 
            __RPC__in _WebSiteMiscEvents * This,
            /* [in] */ __RPC__in REFIID riid,
            /* [size_is][in] */ __RPC__in_ecount_full(cNames) LPOLESTR *rgszNames,
            /* [range][in] */ __RPC__in_range(0,16384) UINT cNames,
            /* [in] */ LCID lcid,
            /* [size_is][out] */ __RPC__out_ecount_full(cNames) DISPID *rgDispId);
        
        /* [local] */ HRESULT ( STDMETHODCALLTYPE *Invoke )( 
            _WebSiteMiscEvents * This,
            /* [annotation][in] */ 
            _In_  DISPID dispIdMember,
            /* [annotation][in] */ 
            _In_  REFIID riid,
            /* [annotation][in] */ 
            _In_  LCID lcid,
            /* [annotation][in] */ 
            _In_  WORD wFlags,
            /* [annotation][out][in] */ 
            _In_  DISPPARAMS *pDispParams,
            /* [annotation][out] */ 
            _Out_opt_  VARIANT *pVarResult,
            /* [annotation][out] */ 
            _Out_opt_  EXCEPINFO *pExcepInfo,
            /* [annotation][out] */ 
            _Out_opt_  UINT *puArgErr);
        
        END_INTERFACE
    } _WebSiteMiscEventsVtbl;

    interface _WebSiteMiscEvents
    {
        CONST_VTBL struct _WebSiteMiscEventsVtbl *lpVtbl;
    };

    

#ifdef COBJMACROS


#define _WebSiteMiscEvents_QueryInterface(This,riid,ppvObject)	\
    ( (This)->lpVtbl -> QueryInterface(This,riid,ppvObject) ) 

#define _WebSiteMiscEvents_AddRef(This)	\
    ( (This)->lpVtbl -> AddRef(This) ) 

#define _WebSiteMiscEvents_Release(This)	\
    ( (This)->lpVtbl -> Release(This) ) 


#define _WebSiteMiscEvents_GetTypeInfoCount(This,pctinfo)	\
    ( (This)->lpVtbl -> GetTypeInfoCount(This,pctinfo) ) 

#define _WebSiteMiscEvents_GetTypeInfo(This,iTInfo,lcid,ppTInfo)	\
    ( (This)->lpVtbl -> GetTypeInfo(This,iTInfo,lcid,ppTInfo) ) 

#define _WebSiteMiscEvents_GetIDsOfNames(This,riid,rgszNames,cNames,lcid,rgDispId)	\
    ( (This)->lpVtbl -> GetIDsOfNames(This,riid,rgszNames,cNames,lcid,rgDispId) ) 

#define _WebSiteMiscEvents_Invoke(This,dispIdMember,riid,lcid,wFlags,pDispParams,pVarResult,pExcepInfo,puArgErr)	\
    ( (This)->lpVtbl -> Invoke(This,dispIdMember,riid,lcid,wFlags,pDispParams,pVarResult,pExcepInfo,puArgErr) ) 


#endif /* COBJMACROS */


#endif 	/* C style interface */




#endif 	/* ___WebSiteMiscEvents_INTERFACE_DEFINED__ */


#ifndef ___dispWebSiteMiscEvents_DISPINTERFACE_DEFINED__
#define ___dispWebSiteMiscEvents_DISPINTERFACE_DEFINED__

/* dispinterface _dispWebSiteMiscEvents */
/* [uuid] */ 


EXTERN_C const IID DIID__dispWebSiteMiscEvents;

#if defined(__cplusplus) && !defined(CINTERFACE)

    MIDL_INTERFACE("B4C62092-F8DB-4cca-9B73-5C7C56DF72AA")
    _dispWebSiteMiscEvents : public IDispatch
    {
    };
    
#else 	/* C style interface */

    typedef struct _dispWebSiteMiscEventsVtbl
    {
        BEGIN_INTERFACE
        
        HRESULT ( STDMETHODCALLTYPE *QueryInterface )( 
            __RPC__in _dispWebSiteMiscEvents * This,
            /* [in] */ __RPC__in REFIID riid,
            /* [annotation][iid_is][out] */ 
            _COM_Outptr_  void **ppvObject);
        
        ULONG ( STDMETHODCALLTYPE *AddRef )( 
            __RPC__in _dispWebSiteMiscEvents * This);
        
        ULONG ( STDMETHODCALLTYPE *Release )( 
            __RPC__in _dispWebSiteMiscEvents * This);
        
        HRESULT ( STDMETHODCALLTYPE *GetTypeInfoCount )( 
            __RPC__in _dispWebSiteMiscEvents * This,
            /* [out] */ __RPC__out UINT *pctinfo);
        
        HRESULT ( STDMETHODCALLTYPE *GetTypeInfo )( 
            __RPC__in _dispWebSiteMiscEvents * This,
            /* [in] */ UINT iTInfo,
            /* [in] */ LCID lcid,
            /* [out] */ __RPC__deref_out_opt ITypeInfo **ppTInfo);
        
        HRESULT ( STDMETHODCALLTYPE *GetIDsOfNames )( 
            __RPC__in _dispWebSiteMiscEvents * This,
            /* [in] */ __RPC__in REFIID riid,
            /* [size_is][in] */ __RPC__in_ecount_full(cNames) LPOLESTR *rgszNames,
            /* [range][in] */ __RPC__in_range(0,16384) UINT cNames,
            /* [in] */ LCID lcid,
            /* [size_is][out] */ __RPC__out_ecount_full(cNames) DISPID *rgDispId);
        
        /* [local] */ HRESULT ( STDMETHODCALLTYPE *Invoke )( 
            _dispWebSiteMiscEvents * This,
            /* [annotation][in] */ 
            _In_  DISPID dispIdMember,
            /* [annotation][in] */ 
            _In_  REFIID riid,
            /* [annotation][in] */ 
            _In_  LCID lcid,
            /* [annotation][in] */ 
            _In_  WORD wFlags,
            /* [annotation][out][in] */ 
            _In_  DISPPARAMS *pDispParams,
            /* [annotation][out] */ 
            _Out_opt_  VARIANT *pVarResult,
            /* [annotation][out] */ 
            _Out_opt_  EXCEPINFO *pExcepInfo,
            /* [annotation][out] */ 
            _Out_opt_  UINT *puArgErr);
        
        END_INTERFACE
    } _dispWebSiteMiscEventsVtbl;

    interface _dispWebSiteMiscEvents
    {
        CONST_VTBL struct _dispWebSiteMiscEventsVtbl *lpVtbl;
    };

    

#ifdef COBJMACROS


#define _dispWebSiteMiscEvents_QueryInterface(This,riid,ppvObject)	\
    ( (This)->lpVtbl -> QueryInterface(This,riid,ppvObject) ) 

#define _dispWebSiteMiscEvents_AddRef(This)	\
    ( (This)->lpVtbl -> AddRef(This) ) 

#define _dispWebSiteMiscEvents_Release(This)	\
    ( (This)->lpVtbl -> Release(This) ) 


#define _dispWebSiteMiscEvents_GetTypeInfoCount(This,pctinfo)	\
    ( (This)->lpVtbl -> GetTypeInfoCount(This,pctinfo) ) 

#define _dispWebSiteMiscEvents_GetTypeInfo(This,iTInfo,lcid,ppTInfo)	\
    ( (This)->lpVtbl -> GetTypeInfo(This,iTInfo,lcid,ppTInfo) ) 

#define _dispWebSiteMiscEvents_GetIDsOfNames(This,riid,rgszNames,cNames,lcid,rgDispId)	\
    ( (This)->lpVtbl -> GetIDsOfNames(This,riid,rgszNames,cNames,lcid,rgDispId) ) 

#define _dispWebSiteMiscEvents_Invoke(This,dispIdMember,riid,lcid,wFlags,pDispParams,pVarResult,pExcepInfo,puArgErr)	\
    ( (This)->lpVtbl -> Invoke(This,dispIdMember,riid,lcid,wFlags,pDispParams,pVarResult,pExcepInfo,puArgErr) ) 

#endif /* COBJMACROS */


#endif 	/* C style interface */


#endif 	/* ___dispWebSiteMiscEvents_DISPINTERFACE_DEFINED__ */


EXTERN_C const CLSID CLSID_WebSiteMiscEvents;

#ifdef __cplusplus

class DECLSPEC_UUID("BC6984AB-D661-4b5e-A0CB-6DFD5FE2DDF4")
WebSiteMiscEvents;
#endif

#ifndef __VSWebSiteEvents_INTERFACE_DEFINED__
#define __VSWebSiteEvents_INTERFACE_DEFINED__

/* interface VSWebSiteEvents */
/* [uuid][object][oleautomation][dual] */ 


EXTERN_C const IID IID_VSWebSiteEvents;

#if defined(__cplusplus) && !defined(CINTERFACE)
    
    MIDL_INTERFACE("9F1B1C2C-FA11-44ab-A7DA-926FF1927C70")
    VSWebSiteEvents : public IDispatch
    {
    public:
        virtual /* [helpstring][propget][id] */ HRESULT STDMETHODCALLTYPE get_WebReferencesEvents( 
            /* [retval][out] */ __RPC__deref_out_opt WebReferencesEvents	**ppWebRefsEvents) = 0;
        
        virtual /* [helpstring][propget][id] */ HRESULT STDMETHODCALLTYPE get_WebServicesEvents( 
            /* [retval][out] */ __RPC__deref_out_opt WebServicesEvents	**ppWebServiceEvents) = 0;
        
        virtual /* [helpstring][propget][id] */ HRESULT STDMETHODCALLTYPE get_AssemblyReferencesEvents( 
            /* [retval][out] */ __RPC__deref_out_opt AssemblyReferencesEvents	**ppWebSiteRefsEvents) = 0;
        
        virtual /* [helpstring][propget][id] */ HRESULT STDMETHODCALLTYPE get_WebSiteMiscEvents( 
            /* [retval][out] */ __RPC__deref_out_opt WebSiteMiscEvents	**ppWebSiteMiscEvents) = 0;
        
    };
    
    
#else 	/* C style interface */

    typedef struct VSWebSiteEventsVtbl
    {
        BEGIN_INTERFACE
        
        HRESULT ( STDMETHODCALLTYPE *QueryInterface )( 
            __RPC__in VSWebSiteEvents * This,
            /* [in] */ __RPC__in REFIID riid,
            /* [annotation][iid_is][out] */ 
            _COM_Outptr_  void **ppvObject);
        
        ULONG ( STDMETHODCALLTYPE *AddRef )( 
            __RPC__in VSWebSiteEvents * This);
        
        ULONG ( STDMETHODCALLTYPE *Release )( 
            __RPC__in VSWebSiteEvents * This);
        
        HRESULT ( STDMETHODCALLTYPE *GetTypeInfoCount )( 
            __RPC__in VSWebSiteEvents * This,
            /* [out] */ __RPC__out UINT *pctinfo);
        
        HRESULT ( STDMETHODCALLTYPE *GetTypeInfo )( 
            __RPC__in VSWebSiteEvents * This,
            /* [in] */ UINT iTInfo,
            /* [in] */ LCID lcid,
            /* [out] */ __RPC__deref_out_opt ITypeInfo **ppTInfo);
        
        HRESULT ( STDMETHODCALLTYPE *GetIDsOfNames )( 
            __RPC__in VSWebSiteEvents * This,
            /* [in] */ __RPC__in REFIID riid,
            /* [size_is][in] */ __RPC__in_ecount_full(cNames) LPOLESTR *rgszNames,
            /* [range][in] */ __RPC__in_range(0,16384) UINT cNames,
            /* [in] */ LCID lcid,
            /* [size_is][out] */ __RPC__out_ecount_full(cNames) DISPID *rgDispId);
        
        /* [local] */ HRESULT ( STDMETHODCALLTYPE *Invoke )( 
            VSWebSiteEvents * This,
            /* [annotation][in] */ 
            _In_  DISPID dispIdMember,
            /* [annotation][in] */ 
            _In_  REFIID riid,
            /* [annotation][in] */ 
            _In_  LCID lcid,
            /* [annotation][in] */ 
            _In_  WORD wFlags,
            /* [annotation][out][in] */ 
            _In_  DISPPARAMS *pDispParams,
            /* [annotation][out] */ 
            _Out_opt_  VARIANT *pVarResult,
            /* [annotation][out] */ 
            _Out_opt_  EXCEPINFO *pExcepInfo,
            /* [annotation][out] */ 
            _Out_opt_  UINT *puArgErr);
        
        /* [helpstring][propget][id] */ HRESULT ( STDMETHODCALLTYPE *get_WebReferencesEvents )( 
            __RPC__in VSWebSiteEvents * This,
            /* [retval][out] */ __RPC__deref_out_opt WebReferencesEvents	**ppWebRefsEvents);
        
        /* [helpstring][propget][id] */ HRESULT ( STDMETHODCALLTYPE *get_WebServicesEvents )( 
            __RPC__in VSWebSiteEvents * This,
            /* [retval][out] */ __RPC__deref_out_opt WebServicesEvents	**ppWebServiceEvents);
        
        /* [helpstring][propget][id] */ HRESULT ( STDMETHODCALLTYPE *get_AssemblyReferencesEvents )( 
            __RPC__in VSWebSiteEvents * This,
            /* [retval][out] */ __RPC__deref_out_opt AssemblyReferencesEvents	**ppWebSiteRefsEvents);
        
        /* [helpstring][propget][id] */ HRESULT ( STDMETHODCALLTYPE *get_WebSiteMiscEvents )( 
            __RPC__in VSWebSiteEvents * This,
            /* [retval][out] */ __RPC__deref_out_opt WebSiteMiscEvents	**ppWebSiteMiscEvents);
        
        END_INTERFACE
    } VSWebSiteEventsVtbl;

    interface VSWebSiteEvents
    {
        CONST_VTBL struct VSWebSiteEventsVtbl *lpVtbl;
    };

    

#ifdef COBJMACROS


#define VSWebSiteEvents_QueryInterface(This,riid,ppvObject)	\
    ( (This)->lpVtbl -> QueryInterface(This,riid,ppvObject) ) 

#define VSWebSiteEvents_AddRef(This)	\
    ( (This)->lpVtbl -> AddRef(This) ) 

#define VSWebSiteEvents_Release(This)	\
    ( (This)->lpVtbl -> Release(This) ) 


#define VSWebSiteEvents_GetTypeInfoCount(This,pctinfo)	\
    ( (This)->lpVtbl -> GetTypeInfoCount(This,pctinfo) ) 

#define VSWebSiteEvents_GetTypeInfo(This,iTInfo,lcid,ppTInfo)	\
    ( (This)->lpVtbl -> GetTypeInfo(This,iTInfo,lcid,ppTInfo) ) 

#define VSWebSiteEvents_GetIDsOfNames(This,riid,rgszNames,cNames,lcid,rgDispId)	\
    ( (This)->lpVtbl -> GetIDsOfNames(This,riid,rgszNames,cNames,lcid,rgDispId) ) 

#define VSWebSiteEvents_Invoke(This,dispIdMember,riid,lcid,wFlags,pDispParams,pVarResult,pExcepInfo,puArgErr)	\
    ( (This)->lpVtbl -> Invoke(This,dispIdMember,riid,lcid,wFlags,pDispParams,pVarResult,pExcepInfo,puArgErr) ) 


#define VSWebSiteEvents_get_WebReferencesEvents(This,ppWebRefsEvents)	\
    ( (This)->lpVtbl -> get_WebReferencesEvents(This,ppWebRefsEvents) ) 

#define VSWebSiteEvents_get_WebServicesEvents(This,ppWebServiceEvents)	\
    ( (This)->lpVtbl -> get_WebServicesEvents(This,ppWebServiceEvents) ) 

#define VSWebSiteEvents_get_AssemblyReferencesEvents(This,ppWebSiteRefsEvents)	\
    ( (This)->lpVtbl -> get_AssemblyReferencesEvents(This,ppWebSiteRefsEvents) ) 

#define VSWebSiteEvents_get_WebSiteMiscEvents(This,ppWebSiteMiscEvents)	\
    ( (This)->lpVtbl -> get_WebSiteMiscEvents(This,ppWebSiteMiscEvents) ) 

#endif /* COBJMACROS */


#endif 	/* C style interface */




#endif 	/* __VSWebSiteEvents_INTERFACE_DEFINED__ */


#ifndef __CodeFolder_INTERFACE_DEFINED__
#define __CodeFolder_INTERFACE_DEFINED__

/* interface CodeFolder */
/* [uuid][object][oleautomation][dual] */ 


EXTERN_C const IID IID_CodeFolder;

#if defined(__cplusplus) && !defined(CINTERFACE)
    
    MIDL_INTERFACE("58B5E6E3-C2D3-4d56-97EB-26DDA09323F7")
    CodeFolder : public IDispatch
    {
    public:
        virtual /* [helpstring][propget][id] */ HRESULT STDMETHODCALLTYPE get_DTE( 
            /* [retval][out] */ __RPC__deref_out_opt /* external definition not present */ DTE **ppDTE) = 0;
        
        virtual /* [helpstring][propget][id] */ HRESULT STDMETHODCALLTYPE get_ProjectItem( 
            /* [retval][out] */ __RPC__deref_out_opt /* external definition not present */ ProjectItem **ppProjectItem) = 0;
        
        virtual /* [helpstring][propget][id] */ HRESULT STDMETHODCALLTYPE get_ContainingProject( 
            /* [retval][out] */ __RPC__deref_out_opt /* external definition not present */ Project **ppProject) = 0;
        
        virtual /* [helpstring][propget][id] */ HRESULT STDMETHODCALLTYPE get_Language( 
            /* [retval][out] */ __RPC__deref_out_opt BSTR *pbstrLanguage) = 0;
        
        virtual /* [helpstring][propget][id] */ HRESULT STDMETHODCALLTYPE get_FolderPath( 
            /* [retval][out] */ __RPC__deref_out_opt BSTR *pbstrPath) = 0;
        
        virtual /* [helpstring][propget][id] */ HRESULT STDMETHODCALLTYPE get_CodeModel( 
            /* [retval][out] */ __RPC__deref_out_opt /* external definition not present */ CodeModel **ppCodeModel) = 0;
        
        virtual /* [helpstring][id] */ HRESULT STDMETHODCALLTYPE Remove( void) = 0;
        
    };
    
    
#else 	/* C style interface */

    typedef struct CodeFolderVtbl
    {
        BEGIN_INTERFACE
        
        HRESULT ( STDMETHODCALLTYPE *QueryInterface )( 
            __RPC__in CodeFolder * This,
            /* [in] */ __RPC__in REFIID riid,
            /* [annotation][iid_is][out] */ 
            _COM_Outptr_  void **ppvObject);
        
        ULONG ( STDMETHODCALLTYPE *AddRef )( 
            __RPC__in CodeFolder * This);
        
        ULONG ( STDMETHODCALLTYPE *Release )( 
            __RPC__in CodeFolder * This);
        
        HRESULT ( STDMETHODCALLTYPE *GetTypeInfoCount )( 
            __RPC__in CodeFolder * This,
            /* [out] */ __RPC__out UINT *pctinfo);
        
        HRESULT ( STDMETHODCALLTYPE *GetTypeInfo )( 
            __RPC__in CodeFolder * This,
            /* [in] */ UINT iTInfo,
            /* [in] */ LCID lcid,
            /* [out] */ __RPC__deref_out_opt ITypeInfo **ppTInfo);
        
        HRESULT ( STDMETHODCALLTYPE *GetIDsOfNames )( 
            __RPC__in CodeFolder * This,
            /* [in] */ __RPC__in REFIID riid,
            /* [size_is][in] */ __RPC__in_ecount_full(cNames) LPOLESTR *rgszNames,
            /* [range][in] */ __RPC__in_range(0,16384) UINT cNames,
            /* [in] */ LCID lcid,
            /* [size_is][out] */ __RPC__out_ecount_full(cNames) DISPID *rgDispId);
        
        /* [local] */ HRESULT ( STDMETHODCALLTYPE *Invoke )( 
            CodeFolder * This,
            /* [annotation][in] */ 
            _In_  DISPID dispIdMember,
            /* [annotation][in] */ 
            _In_  REFIID riid,
            /* [annotation][in] */ 
            _In_  LCID lcid,
            /* [annotation][in] */ 
            _In_  WORD wFlags,
            /* [annotation][out][in] */ 
            _In_  DISPPARAMS *pDispParams,
            /* [annotation][out] */ 
            _Out_opt_  VARIANT *pVarResult,
            /* [annotation][out] */ 
            _Out_opt_  EXCEPINFO *pExcepInfo,
            /* [annotation][out] */ 
            _Out_opt_  UINT *puArgErr);
        
        /* [helpstring][propget][id] */ HRESULT ( STDMETHODCALLTYPE *get_DTE )( 
            __RPC__in CodeFolder * This,
            /* [retval][out] */ __RPC__deref_out_opt /* external definition not present */ DTE **ppDTE);
        
        /* [helpstring][propget][id] */ HRESULT ( STDMETHODCALLTYPE *get_ProjectItem )( 
            __RPC__in CodeFolder * This,
            /* [retval][out] */ __RPC__deref_out_opt /* external definition not present */ ProjectItem **ppProjectItem);
        
        /* [helpstring][propget][id] */ HRESULT ( STDMETHODCALLTYPE *get_ContainingProject )( 
            __RPC__in CodeFolder * This,
            /* [retval][out] */ __RPC__deref_out_opt /* external definition not present */ Project **ppProject);
        
        /* [helpstring][propget][id] */ HRESULT ( STDMETHODCALLTYPE *get_Language )( 
            __RPC__in CodeFolder * This,
            /* [retval][out] */ __RPC__deref_out_opt BSTR *pbstrLanguage);
        
        /* [helpstring][propget][id] */ HRESULT ( STDMETHODCALLTYPE *get_FolderPath )( 
            __RPC__in CodeFolder * This,
            /* [retval][out] */ __RPC__deref_out_opt BSTR *pbstrPath);
        
        /* [helpstring][propget][id] */ HRESULT ( STDMETHODCALLTYPE *get_CodeModel )( 
            __RPC__in CodeFolder * This,
            /* [retval][out] */ __RPC__deref_out_opt /* external definition not present */ CodeModel **ppCodeModel);
        
        /* [helpstring][id] */ HRESULT ( STDMETHODCALLTYPE *Remove )( 
            __RPC__in CodeFolder * This);
        
        END_INTERFACE
    } CodeFolderVtbl;

    interface CodeFolder
    {
        CONST_VTBL struct CodeFolderVtbl *lpVtbl;
    };

    

#ifdef COBJMACROS


#define CodeFolder_QueryInterface(This,riid,ppvObject)	\
    ( (This)->lpVtbl -> QueryInterface(This,riid,ppvObject) ) 

#define CodeFolder_AddRef(This)	\
    ( (This)->lpVtbl -> AddRef(This) ) 

#define CodeFolder_Release(This)	\
    ( (This)->lpVtbl -> Release(This) ) 


#define CodeFolder_GetTypeInfoCount(This,pctinfo)	\
    ( (This)->lpVtbl -> GetTypeInfoCount(This,pctinfo) ) 

#define CodeFolder_GetTypeInfo(This,iTInfo,lcid,ppTInfo)	\
    ( (This)->lpVtbl -> GetTypeInfo(This,iTInfo,lcid,ppTInfo) ) 

#define CodeFolder_GetIDsOfNames(This,riid,rgszNames,cNames,lcid,rgDispId)	\
    ( (This)->lpVtbl -> GetIDsOfNames(This,riid,rgszNames,cNames,lcid,rgDispId) ) 

#define CodeFolder_Invoke(This,dispIdMember,riid,lcid,wFlags,pDispParams,pVarResult,pExcepInfo,puArgErr)	\
    ( (This)->lpVtbl -> Invoke(This,dispIdMember,riid,lcid,wFlags,pDispParams,pVarResult,pExcepInfo,puArgErr) ) 


#define CodeFolder_get_DTE(This,ppDTE)	\
    ( (This)->lpVtbl -> get_DTE(This,ppDTE) ) 

#define CodeFolder_get_ProjectItem(This,ppProjectItem)	\
    ( (This)->lpVtbl -> get_ProjectItem(This,ppProjectItem) ) 

#define CodeFolder_get_ContainingProject(This,ppProject)	\
    ( (This)->lpVtbl -> get_ContainingProject(This,ppProject) ) 

#define CodeFolder_get_Language(This,pbstrLanguage)	\
    ( (This)->lpVtbl -> get_Language(This,pbstrLanguage) ) 

#define CodeFolder_get_FolderPath(This,pbstrPath)	\
    ( (This)->lpVtbl -> get_FolderPath(This,pbstrPath) ) 

#define CodeFolder_get_CodeModel(This,ppCodeModel)	\
    ( (This)->lpVtbl -> get_CodeModel(This,ppCodeModel) ) 

#define CodeFolder_Remove(This)	\
    ( (This)->lpVtbl -> Remove(This) ) 

#endif /* COBJMACROS */


#endif 	/* C style interface */




#endif 	/* __CodeFolder_INTERFACE_DEFINED__ */


#ifndef __CodeFolders_INTERFACE_DEFINED__
#define __CodeFolders_INTERFACE_DEFINED__

/* interface CodeFolders */
/* [uuid][object][oleautomation][dual] */ 


EXTERN_C const IID IID_CodeFolders;

#if defined(__cplusplus) && !defined(CINTERFACE)
    
    MIDL_INTERFACE("FDCAD245-E468-48f3-B603-2641E020D6D0")
    CodeFolders : public IDispatch
    {
    public:
        virtual /* [helpstring][propget][id] */ HRESULT STDMETHODCALLTYPE get_DTE( 
            /* [retval][out] */ __RPC__deref_out_opt /* external definition not present */ DTE **ppDTE) = 0;
        
        virtual /* [helpstring][propget][id] */ HRESULT STDMETHODCALLTYPE get_ContainingProject( 
            /* [retval][out] */ __RPC__deref_out_opt /* external definition not present */ Project **ppProject) = 0;
        
        virtual /* [helpstring][propget][id] */ HRESULT STDMETHODCALLTYPE get_Count( 
            /* [retval][out] */ __RPC__out long *plCount) = 0;
        
        virtual /* [helpstring][id] */ HRESULT STDMETHODCALLTYPE Item( 
            /* [in] */ VARIANT index,
            /* [retval][out] */ __RPC__deref_out_opt CodeFolder **ppCodeFolder) = 0;
        
        virtual /* [id][restricted] */ HRESULT STDMETHODCALLTYPE _NewEnum( 
            /* [retval][out] */ __RPC__deref_out_opt IUnknown **ppiuReturn) = 0;
        
        virtual /* [helpstring][id] */ HRESULT STDMETHODCALLTYPE Add( 
            /* [in] */ __RPC__in BSTR bstrPath,
            /* [retval][out] */ __RPC__deref_out_opt /* external definition not present */ ProjectItem **ppProjectItem) = 0;
        
    };
    
    
#else 	/* C style interface */

    typedef struct CodeFoldersVtbl
    {
        BEGIN_INTERFACE
        
        HRESULT ( STDMETHODCALLTYPE *QueryInterface )( 
            __RPC__in CodeFolders * This,
            /* [in] */ __RPC__in REFIID riid,
            /* [annotation][iid_is][out] */ 
            _COM_Outptr_  void **ppvObject);
        
        ULONG ( STDMETHODCALLTYPE *AddRef )( 
            __RPC__in CodeFolders * This);
        
        ULONG ( STDMETHODCALLTYPE *Release )( 
            __RPC__in CodeFolders * This);
        
        HRESULT ( STDMETHODCALLTYPE *GetTypeInfoCount )( 
            __RPC__in CodeFolders * This,
            /* [out] */ __RPC__out UINT *pctinfo);
        
        HRESULT ( STDMETHODCALLTYPE *GetTypeInfo )( 
            __RPC__in CodeFolders * This,
            /* [in] */ UINT iTInfo,
            /* [in] */ LCID lcid,
            /* [out] */ __RPC__deref_out_opt ITypeInfo **ppTInfo);
        
        HRESULT ( STDMETHODCALLTYPE *GetIDsOfNames )( 
            __RPC__in CodeFolders * This,
            /* [in] */ __RPC__in REFIID riid,
            /* [size_is][in] */ __RPC__in_ecount_full(cNames) LPOLESTR *rgszNames,
            /* [range][in] */ __RPC__in_range(0,16384) UINT cNames,
            /* [in] */ LCID lcid,
            /* [size_is][out] */ __RPC__out_ecount_full(cNames) DISPID *rgDispId);
        
        /* [local] */ HRESULT ( STDMETHODCALLTYPE *Invoke )( 
            CodeFolders * This,
            /* [annotation][in] */ 
            _In_  DISPID dispIdMember,
            /* [annotation][in] */ 
            _In_  REFIID riid,
            /* [annotation][in] */ 
            _In_  LCID lcid,
            /* [annotation][in] */ 
            _In_  WORD wFlags,
            /* [annotation][out][in] */ 
            _In_  DISPPARAMS *pDispParams,
            /* [annotation][out] */ 
            _Out_opt_  VARIANT *pVarResult,
            /* [annotation][out] */ 
            _Out_opt_  EXCEPINFO *pExcepInfo,
            /* [annotation][out] */ 
            _Out_opt_  UINT *puArgErr);
        
        /* [helpstring][propget][id] */ HRESULT ( STDMETHODCALLTYPE *get_DTE )( 
            __RPC__in CodeFolders * This,
            /* [retval][out] */ __RPC__deref_out_opt /* external definition not present */ DTE **ppDTE);
        
        /* [helpstring][propget][id] */ HRESULT ( STDMETHODCALLTYPE *get_ContainingProject )( 
            __RPC__in CodeFolders * This,
            /* [retval][out] */ __RPC__deref_out_opt /* external definition not present */ Project **ppProject);
        
        /* [helpstring][propget][id] */ HRESULT ( STDMETHODCALLTYPE *get_Count )( 
            __RPC__in CodeFolders * This,
            /* [retval][out] */ __RPC__out long *plCount);
        
        /* [helpstring][id] */ HRESULT ( STDMETHODCALLTYPE *Item )( 
            __RPC__in CodeFolders * This,
            /* [in] */ VARIANT index,
            /* [retval][out] */ __RPC__deref_out_opt CodeFolder **ppCodeFolder);
        
        /* [id][restricted] */ HRESULT ( STDMETHODCALLTYPE *_NewEnum )( 
            __RPC__in CodeFolders * This,
            /* [retval][out] */ __RPC__deref_out_opt IUnknown **ppiuReturn);
        
        /* [helpstring][id] */ HRESULT ( STDMETHODCALLTYPE *Add )( 
            __RPC__in CodeFolders * This,
            /* [in] */ __RPC__in BSTR bstrPath,
            /* [retval][out] */ __RPC__deref_out_opt /* external definition not present */ ProjectItem **ppProjectItem);
        
        END_INTERFACE
    } CodeFoldersVtbl;

    interface CodeFolders
    {
        CONST_VTBL struct CodeFoldersVtbl *lpVtbl;
    };

    

#ifdef COBJMACROS


#define CodeFolders_QueryInterface(This,riid,ppvObject)	\
    ( (This)->lpVtbl -> QueryInterface(This,riid,ppvObject) ) 

#define CodeFolders_AddRef(This)	\
    ( (This)->lpVtbl -> AddRef(This) ) 

#define CodeFolders_Release(This)	\
    ( (This)->lpVtbl -> Release(This) ) 


#define CodeFolders_GetTypeInfoCount(This,pctinfo)	\
    ( (This)->lpVtbl -> GetTypeInfoCount(This,pctinfo) ) 

#define CodeFolders_GetTypeInfo(This,iTInfo,lcid,ppTInfo)	\
    ( (This)->lpVtbl -> GetTypeInfo(This,iTInfo,lcid,ppTInfo) ) 

#define CodeFolders_GetIDsOfNames(This,riid,rgszNames,cNames,lcid,rgDispId)	\
    ( (This)->lpVtbl -> GetIDsOfNames(This,riid,rgszNames,cNames,lcid,rgDispId) ) 

#define CodeFolders_Invoke(This,dispIdMember,riid,lcid,wFlags,pDispParams,pVarResult,pExcepInfo,puArgErr)	\
    ( (This)->lpVtbl -> Invoke(This,dispIdMember,riid,lcid,wFlags,pDispParams,pVarResult,pExcepInfo,puArgErr) ) 


#define CodeFolders_get_DTE(This,ppDTE)	\
    ( (This)->lpVtbl -> get_DTE(This,ppDTE) ) 

#define CodeFolders_get_ContainingProject(This,ppProject)	\
    ( (This)->lpVtbl -> get_ContainingProject(This,ppProject) ) 

#define CodeFolders_get_Count(This,plCount)	\
    ( (This)->lpVtbl -> get_Count(This,plCount) ) 

#define CodeFolders_Item(This,index,ppCodeFolder)	\
    ( (This)->lpVtbl -> Item(This,index,ppCodeFolder) ) 

#define CodeFolders__NewEnum(This,ppiuReturn)	\
    ( (This)->lpVtbl -> _NewEnum(This,ppiuReturn) ) 

#define CodeFolders_Add(This,bstrPath,ppProjectItem)	\
    ( (This)->lpVtbl -> Add(This,bstrPath,ppProjectItem) ) 

#endif /* COBJMACROS */


#endif 	/* C style interface */




#endif 	/* __CodeFolders_INTERFACE_DEFINED__ */


#ifndef __VSWebSite_INTERFACE_DEFINED__
#define __VSWebSite_INTERFACE_DEFINED__

/* interface VSWebSite */
/* [uuid][object][oleautomation][dual] */ 


EXTERN_C const IID IID_VSWebSite;

#if defined(__cplusplus) && !defined(CINTERFACE)
    
    MIDL_INTERFACE("70758CA4-91F8-46ad-80A7-73BC21BAE68B")
    VSWebSite : public IDispatch
    {
    public:
        virtual /* [helpstring][propget][id] */ HRESULT STDMETHODCALLTYPE get_DTE( 
            /* [retval][out] */ __RPC__deref_out_opt /* external definition not present */ DTE **ppDTE) = 0;
        
        virtual /* [helpstring][propget][id] */ HRESULT STDMETHODCALLTYPE get_Project( 
            /* [retval][out] */ __RPC__deref_out_opt /* external definition not present */ Project **ppProject) = 0;
        
        virtual /* [helpstring][propget][id] */ HRESULT STDMETHODCALLTYPE get_VsWebSiteEvents( 
            /* [retval][out] */ __RPC__deref_out_opt VSWebSiteEvents **ppProject) = 0;
        
        virtual /* [helpstring][propget][id] */ HRESULT STDMETHODCALLTYPE get_URL( 
            /* [retval][out] */ __RPC__deref_out_opt BSTR *pbstrUrl) = 0;
        
        virtual /* [helpstring][propget][id] */ HRESULT STDMETHODCALLTYPE get_TemplatePath( 
            /* [retval][out] */ __RPC__deref_out_opt BSTR *pbstrTemplatePath) = 0;
        
        virtual /* [helpstring][propget][id] */ HRESULT STDMETHODCALLTYPE get_UserTemplatePath( 
            /* [retval][out] */ __RPC__deref_out_opt BSTR *pbstrUserTemplatePath) = 0;
        
        virtual /* [helpstring][id] */ HRESULT STDMETHODCALLTYPE Refresh( void) = 0;
        
        virtual /* [helpstring][id] */ HRESULT STDMETHODCALLTYPE GetUniqueFilename( 
            /* [in] */ __RPC__in BSTR bstrFolder,
            /* [in] */ __RPC__in BSTR bstrRoot,
            /* [in] */ __RPC__in BSTR bstrDesiredExt,
            /* [retval][out] */ __RPC__deref_out_opt BSTR *pbstrFileName) = 0;
        
        virtual /* [helpstring][propget][id] */ HRESULT STDMETHODCALLTYPE get_WebReferences( 
            /* [retval][out] */ __RPC__deref_out_opt WebReferences **ppWebReferences) = 0;
        
        virtual /* [helpstring][propget][id] */ HRESULT STDMETHODCALLTYPE get_WebServices( 
            /* [retval][out] */ __RPC__deref_out_opt WebServices **ppWebServices) = 0;
        
        virtual /* [helpstring][propget][id] */ HRESULT STDMETHODCALLTYPE get_References( 
            /* [retval][out] */ __RPC__deref_out_opt AssemblyReferences **ppReferences) = 0;
        
        virtual /* [helpstring][propget][id] */ HRESULT STDMETHODCALLTYPE get_CodeFolders( 
            /* [retval][out] */ __RPC__deref_out_opt CodeFolders **ppCodeFolders) = 0;
        
        virtual /* [helpstring][id] */ HRESULT STDMETHODCALLTYPE EnsureServerRunning( 
            /* [retval][out] */ __RPC__deref_out_opt BSTR *pbstrUrl) = 0;
        
        virtual /* [helpstring][id] */ HRESULT STDMETHODCALLTYPE AddFromTemplate( 
            /* [in] */ __RPC__in BSTR bstrRelFolderUrl,
            /* [in] */ __RPC__in BSTR bstrWizardName,
            /* [in] */ __RPC__in BSTR bstrLanguage,
            /* [in] */ __RPC__in BSTR bstrItemName,
            /* [in] */ VARIANT_BOOL bUseCodeSeparation,
            /* [in] */ __RPC__in BSTR bstrMasterPage,
            /* [in] */ __RPC__in BSTR bstrDocType,
            /* [retval][out] */ __RPC__deref_out_opt /* external definition not present */ ProjectItem **ppNewProjectItem) = 0;
        
        virtual /* [helpstring][id] */ HRESULT STDMETHODCALLTYPE PreCompileWeb( 
            /* [in] */ __RPC__in BSTR bstrCompilePath,
            /* [in] */ VARIANT_BOOL bUpdateable,
            /* [retval][out] */ __RPC__out VARIANT_BOOL *pSuccess) = 0;
        
        virtual /* [helpstring][id] */ HRESULT STDMETHODCALLTYPE WaitUntilReady( void) = 0;
        
    };
    
    
#else 	/* C style interface */

    typedef struct VSWebSiteVtbl
    {
        BEGIN_INTERFACE
        
        HRESULT ( STDMETHODCALLTYPE *QueryInterface )( 
            __RPC__in VSWebSite * This,
            /* [in] */ __RPC__in REFIID riid,
            /* [annotation][iid_is][out] */ 
            _COM_Outptr_  void **ppvObject);
        
        ULONG ( STDMETHODCALLTYPE *AddRef )( 
            __RPC__in VSWebSite * This);
        
        ULONG ( STDMETHODCALLTYPE *Release )( 
            __RPC__in VSWebSite * This);
        
        HRESULT ( STDMETHODCALLTYPE *GetTypeInfoCount )( 
            __RPC__in VSWebSite * This,
            /* [out] */ __RPC__out UINT *pctinfo);
        
        HRESULT ( STDMETHODCALLTYPE *GetTypeInfo )( 
            __RPC__in VSWebSite * This,
            /* [in] */ UINT iTInfo,
            /* [in] */ LCID lcid,
            /* [out] */ __RPC__deref_out_opt ITypeInfo **ppTInfo);
        
        HRESULT ( STDMETHODCALLTYPE *GetIDsOfNames )( 
            __RPC__in VSWebSite * This,
            /* [in] */ __RPC__in REFIID riid,
            /* [size_is][in] */ __RPC__in_ecount_full(cNames) LPOLESTR *rgszNames,
            /* [range][in] */ __RPC__in_range(0,16384) UINT cNames,
            /* [in] */ LCID lcid,
            /* [size_is][out] */ __RPC__out_ecount_full(cNames) DISPID *rgDispId);
        
        /* [local] */ HRESULT ( STDMETHODCALLTYPE *Invoke )( 
            VSWebSite * This,
            /* [annotation][in] */ 
            _In_  DISPID dispIdMember,
            /* [annotation][in] */ 
            _In_  REFIID riid,
            /* [annotation][in] */ 
            _In_  LCID lcid,
            /* [annotation][in] */ 
            _In_  WORD wFlags,
            /* [annotation][out][in] */ 
            _In_  DISPPARAMS *pDispParams,
            /* [annotation][out] */ 
            _Out_opt_  VARIANT *pVarResult,
            /* [annotation][out] */ 
            _Out_opt_  EXCEPINFO *pExcepInfo,
            /* [annotation][out] */ 
            _Out_opt_  UINT *puArgErr);
        
        /* [helpstring][propget][id] */ HRESULT ( STDMETHODCALLTYPE *get_DTE )( 
            __RPC__in VSWebSite * This,
            /* [retval][out] */ __RPC__deref_out_opt /* external definition not present */ DTE **ppDTE);
        
        /* [helpstring][propget][id] */ HRESULT ( STDMETHODCALLTYPE *get_Project )( 
            __RPC__in VSWebSite * This,
            /* [retval][out] */ __RPC__deref_out_opt /* external definition not present */ Project **ppProject);
        
        /* [helpstring][propget][id] */ HRESULT ( STDMETHODCALLTYPE *get_VsWebSiteEvents )( 
            __RPC__in VSWebSite * This,
            /* [retval][out] */ __RPC__deref_out_opt VSWebSiteEvents **ppProject);
        
        /* [helpstring][propget][id] */ HRESULT ( STDMETHODCALLTYPE *get_URL )( 
            __RPC__in VSWebSite * This,
            /* [retval][out] */ __RPC__deref_out_opt BSTR *pbstrUrl);
        
        /* [helpstring][propget][id] */ HRESULT ( STDMETHODCALLTYPE *get_TemplatePath )( 
            __RPC__in VSWebSite * This,
            /* [retval][out] */ __RPC__deref_out_opt BSTR *pbstrTemplatePath);
        
        /* [helpstring][propget][id] */ HRESULT ( STDMETHODCALLTYPE *get_UserTemplatePath )( 
            __RPC__in VSWebSite * This,
            /* [retval][out] */ __RPC__deref_out_opt BSTR *pbstrUserTemplatePath);
        
        /* [helpstring][id] */ HRESULT ( STDMETHODCALLTYPE *Refresh )( 
            __RPC__in VSWebSite * This);
        
        /* [helpstring][id] */ HRESULT ( STDMETHODCALLTYPE *GetUniqueFilename )( 
            __RPC__in VSWebSite * This,
            /* [in] */ __RPC__in BSTR bstrFolder,
            /* [in] */ __RPC__in BSTR bstrRoot,
            /* [in] */ __RPC__in BSTR bstrDesiredExt,
            /* [retval][out] */ __RPC__deref_out_opt BSTR *pbstrFileName);
        
        /* [helpstring][propget][id] */ HRESULT ( STDMETHODCALLTYPE *get_WebReferences )( 
            __RPC__in VSWebSite * This,
            /* [retval][out] */ __RPC__deref_out_opt WebReferences **ppWebReferences);
        
        /* [helpstring][propget][id] */ HRESULT ( STDMETHODCALLTYPE *get_WebServices )( 
            __RPC__in VSWebSite * This,
            /* [retval][out] */ __RPC__deref_out_opt WebServices **ppWebServices);
        
        /* [helpstring][propget][id] */ HRESULT ( STDMETHODCALLTYPE *get_References )( 
            __RPC__in VSWebSite * This,
            /* [retval][out] */ __RPC__deref_out_opt AssemblyReferences **ppReferences);
        
        /* [helpstring][propget][id] */ HRESULT ( STDMETHODCALLTYPE *get_CodeFolders )( 
            __RPC__in VSWebSite * This,
            /* [retval][out] */ __RPC__deref_out_opt CodeFolders **ppCodeFolders);
        
        /* [helpstring][id] */ HRESULT ( STDMETHODCALLTYPE *EnsureServerRunning )( 
            __RPC__in VSWebSite * This,
            /* [retval][out] */ __RPC__deref_out_opt BSTR *pbstrUrl);
        
        /* [helpstring][id] */ HRESULT ( STDMETHODCALLTYPE *AddFromTemplate )( 
            __RPC__in VSWebSite * This,
            /* [in] */ __RPC__in BSTR bstrRelFolderUrl,
            /* [in] */ __RPC__in BSTR bstrWizardName,
            /* [in] */ __RPC__in BSTR bstrLanguage,
            /* [in] */ __RPC__in BSTR bstrItemName,
            /* [in] */ VARIANT_BOOL bUseCodeSeparation,
            /* [in] */ __RPC__in BSTR bstrMasterPage,
            /* [in] */ __RPC__in BSTR bstrDocType,
            /* [retval][out] */ __RPC__deref_out_opt /* external definition not present */ ProjectItem **ppNewProjectItem);
        
        /* [helpstring][id] */ HRESULT ( STDMETHODCALLTYPE *PreCompileWeb )( 
            __RPC__in VSWebSite * This,
            /* [in] */ __RPC__in BSTR bstrCompilePath,
            /* [in] */ VARIANT_BOOL bUpdateable,
            /* [retval][out] */ __RPC__out VARIANT_BOOL *pSuccess);
        
        /* [helpstring][id] */ HRESULT ( STDMETHODCALLTYPE *WaitUntilReady )( 
            __RPC__in VSWebSite * This);
        
        END_INTERFACE
    } VSWebSiteVtbl;

    interface VSWebSite
    {
        CONST_VTBL struct VSWebSiteVtbl *lpVtbl;
    };

    

#ifdef COBJMACROS


#define VSWebSite_QueryInterface(This,riid,ppvObject)	\
    ( (This)->lpVtbl -> QueryInterface(This,riid,ppvObject) ) 

#define VSWebSite_AddRef(This)	\
    ( (This)->lpVtbl -> AddRef(This) ) 

#define VSWebSite_Release(This)	\
    ( (This)->lpVtbl -> Release(This) ) 


#define VSWebSite_GetTypeInfoCount(This,pctinfo)	\
    ( (This)->lpVtbl -> GetTypeInfoCount(This,pctinfo) ) 

#define VSWebSite_GetTypeInfo(This,iTInfo,lcid,ppTInfo)	\
    ( (This)->lpVtbl -> GetTypeInfo(This,iTInfo,lcid,ppTInfo) ) 

#define VSWebSite_GetIDsOfNames(This,riid,rgszNames,cNames,lcid,rgDispId)	\
    ( (This)->lpVtbl -> GetIDsOfNames(This,riid,rgszNames,cNames,lcid,rgDispId) ) 

#define VSWebSite_Invoke(This,dispIdMember,riid,lcid,wFlags,pDispParams,pVarResult,pExcepInfo,puArgErr)	\
    ( (This)->lpVtbl -> Invoke(This,dispIdMember,riid,lcid,wFlags,pDispParams,pVarResult,pExcepInfo,puArgErr) ) 


#define VSWebSite_get_DTE(This,ppDTE)	\
    ( (This)->lpVtbl -> get_DTE(This,ppDTE) ) 

#define VSWebSite_get_Project(This,ppProject)	\
    ( (This)->lpVtbl -> get_Project(This,ppProject) ) 

#define VSWebSite_get_VsWebSiteEvents(This,ppProject)	\
    ( (This)->lpVtbl -> get_VsWebSiteEvents(This,ppProject) ) 

#define VSWebSite_get_URL(This,pbstrUrl)	\
    ( (This)->lpVtbl -> get_URL(This,pbstrUrl) ) 

#define VSWebSite_get_TemplatePath(This,pbstrTemplatePath)	\
    ( (This)->lpVtbl -> get_TemplatePath(This,pbstrTemplatePath) ) 

#define VSWebSite_get_UserTemplatePath(This,pbstrUserTemplatePath)	\
    ( (This)->lpVtbl -> get_UserTemplatePath(This,pbstrUserTemplatePath) ) 

#define VSWebSite_Refresh(This)	\
    ( (This)->lpVtbl -> Refresh(This) ) 

#define VSWebSite_GetUniqueFilename(This,bstrFolder,bstrRoot,bstrDesiredExt,pbstrFileName)	\
    ( (This)->lpVtbl -> GetUniqueFilename(This,bstrFolder,bstrRoot,bstrDesiredExt,pbstrFileName) ) 

#define VSWebSite_get_WebReferences(This,ppWebReferences)	\
    ( (This)->lpVtbl -> get_WebReferences(This,ppWebReferences) ) 

#define VSWebSite_get_WebServices(This,ppWebServices)	\
    ( (This)->lpVtbl -> get_WebServices(This,ppWebServices) ) 

#define VSWebSite_get_References(This,ppReferences)	\
    ( (This)->lpVtbl -> get_References(This,ppReferences) ) 

#define VSWebSite_get_CodeFolders(This,ppCodeFolders)	\
    ( (This)->lpVtbl -> get_CodeFolders(This,ppCodeFolders) ) 

#define VSWebSite_EnsureServerRunning(This,pbstrUrl)	\
    ( (This)->lpVtbl -> EnsureServerRunning(This,pbstrUrl) ) 

#define VSWebSite_AddFromTemplate(This,bstrRelFolderUrl,bstrWizardName,bstrLanguage,bstrItemName,bUseCodeSeparation,bstrMasterPage,bstrDocType,ppNewProjectItem)	\
    ( (This)->lpVtbl -> AddFromTemplate(This,bstrRelFolderUrl,bstrWizardName,bstrLanguage,bstrItemName,bUseCodeSeparation,bstrMasterPage,bstrDocType,ppNewProjectItem) ) 

#define VSWebSite_PreCompileWeb(This,bstrCompilePath,bUpdateable,pSuccess)	\
    ( (This)->lpVtbl -> PreCompileWeb(This,bstrCompilePath,bUpdateable,pSuccess) ) 

#define VSWebSite_WaitUntilReady(This)	\
    ( (This)->lpVtbl -> WaitUntilReady(This) ) 

#endif /* COBJMACROS */


#endif 	/* C style interface */




#endif 	/* __VSWebSite_INTERFACE_DEFINED__ */


#ifndef __RelatedFiles_INTERFACE_DEFINED__
#define __RelatedFiles_INTERFACE_DEFINED__

/* interface RelatedFiles */
/* [helpstring][oleautomation][dual][uuid][object] */ 


EXTERN_C const IID IID_RelatedFiles;

#if defined(__cplusplus) && !defined(CINTERFACE)
    
    MIDL_INTERFACE("7C55A332-A2EC-4387-A242-CD1B9A29F1D2")
    RelatedFiles : public IDispatch
    {
    public:
        virtual /* [helpstring][id][propget] */ HRESULT STDMETHODCALLTYPE get_Count( 
            /* [retval][out] */ __RPC__out long *pdwCount) = 0;
        
        virtual /* [helpstring][id] */ HRESULT STDMETHODCALLTYPE Item( 
            /* [in] */ long index,
            /* [retval][out] */ __RPC__deref_out_opt /* external definition not present */ ProjectItem **ppProjectItem) = 0;
        
        virtual /* [id][restricted] */ HRESULT STDMETHODCALLTYPE _NewEnum( 
            /* [retval][out] */ __RPC__deref_out_opt IUnknown **ppiuReturn) = 0;
        
    };
    
    
#else 	/* C style interface */

    typedef struct RelatedFilesVtbl
    {
        BEGIN_INTERFACE
        
        HRESULT ( STDMETHODCALLTYPE *QueryInterface )( 
            __RPC__in RelatedFiles * This,
            /* [in] */ __RPC__in REFIID riid,
            /* [annotation][iid_is][out] */ 
            _COM_Outptr_  void **ppvObject);
        
        ULONG ( STDMETHODCALLTYPE *AddRef )( 
            __RPC__in RelatedFiles * This);
        
        ULONG ( STDMETHODCALLTYPE *Release )( 
            __RPC__in RelatedFiles * This);
        
        HRESULT ( STDMETHODCALLTYPE *GetTypeInfoCount )( 
            __RPC__in RelatedFiles * This,
            /* [out] */ __RPC__out UINT *pctinfo);
        
        HRESULT ( STDMETHODCALLTYPE *GetTypeInfo )( 
            __RPC__in RelatedFiles * This,
            /* [in] */ UINT iTInfo,
            /* [in] */ LCID lcid,
            /* [out] */ __RPC__deref_out_opt ITypeInfo **ppTInfo);
        
        HRESULT ( STDMETHODCALLTYPE *GetIDsOfNames )( 
            __RPC__in RelatedFiles * This,
            /* [in] */ __RPC__in REFIID riid,
            /* [size_is][in] */ __RPC__in_ecount_full(cNames) LPOLESTR *rgszNames,
            /* [range][in] */ __RPC__in_range(0,16384) UINT cNames,
            /* [in] */ LCID lcid,
            /* [size_is][out] */ __RPC__out_ecount_full(cNames) DISPID *rgDispId);
        
        /* [local] */ HRESULT ( STDMETHODCALLTYPE *Invoke )( 
            RelatedFiles * This,
            /* [annotation][in] */ 
            _In_  DISPID dispIdMember,
            /* [annotation][in] */ 
            _In_  REFIID riid,
            /* [annotation][in] */ 
            _In_  LCID lcid,
            /* [annotation][in] */ 
            _In_  WORD wFlags,
            /* [annotation][out][in] */ 
            _In_  DISPPARAMS *pDispParams,
            /* [annotation][out] */ 
            _Out_opt_  VARIANT *pVarResult,
            /* [annotation][out] */ 
            _Out_opt_  EXCEPINFO *pExcepInfo,
            /* [annotation][out] */ 
            _Out_opt_  UINT *puArgErr);
        
        /* [helpstring][id][propget] */ HRESULT ( STDMETHODCALLTYPE *get_Count )( 
            __RPC__in RelatedFiles * This,
            /* [retval][out] */ __RPC__out long *pdwCount);
        
        /* [helpstring][id] */ HRESULT ( STDMETHODCALLTYPE *Item )( 
            __RPC__in RelatedFiles * This,
            /* [in] */ long index,
            /* [retval][out] */ __RPC__deref_out_opt /* external definition not present */ ProjectItem **ppProjectItem);
        
        /* [id][restricted] */ HRESULT ( STDMETHODCALLTYPE *_NewEnum )( 
            __RPC__in RelatedFiles * This,
            /* [retval][out] */ __RPC__deref_out_opt IUnknown **ppiuReturn);
        
        END_INTERFACE
    } RelatedFilesVtbl;

    interface RelatedFiles
    {
        CONST_VTBL struct RelatedFilesVtbl *lpVtbl;
    };

    

#ifdef COBJMACROS


#define RelatedFiles_QueryInterface(This,riid,ppvObject)	\
    ( (This)->lpVtbl -> QueryInterface(This,riid,ppvObject) ) 

#define RelatedFiles_AddRef(This)	\
    ( (This)->lpVtbl -> AddRef(This) ) 

#define RelatedFiles_Release(This)	\
    ( (This)->lpVtbl -> Release(This) ) 


#define RelatedFiles_GetTypeInfoCount(This,pctinfo)	\
    ( (This)->lpVtbl -> GetTypeInfoCount(This,pctinfo) ) 

#define RelatedFiles_GetTypeInfo(This,iTInfo,lcid,ppTInfo)	\
    ( (This)->lpVtbl -> GetTypeInfo(This,iTInfo,lcid,ppTInfo) ) 

#define RelatedFiles_GetIDsOfNames(This,riid,rgszNames,cNames,lcid,rgDispId)	\
    ( (This)->lpVtbl -> GetIDsOfNames(This,riid,rgszNames,cNames,lcid,rgDispId) ) 

#define RelatedFiles_Invoke(This,dispIdMember,riid,lcid,wFlags,pDispParams,pVarResult,pExcepInfo,puArgErr)	\
    ( (This)->lpVtbl -> Invoke(This,dispIdMember,riid,lcid,wFlags,pDispParams,pVarResult,pExcepInfo,puArgErr) ) 


#define RelatedFiles_get_Count(This,pdwCount)	\
    ( (This)->lpVtbl -> get_Count(This,pdwCount) ) 

#define RelatedFiles_Item(This,index,ppProjectItem)	\
    ( (This)->lpVtbl -> Item(This,index,ppProjectItem) ) 

#define RelatedFiles__NewEnum(This,ppiuReturn)	\
    ( (This)->lpVtbl -> _NewEnum(This,ppiuReturn) ) 

#endif /* COBJMACROS */


#endif 	/* C style interface */




#endif 	/* __RelatedFiles_INTERFACE_DEFINED__ */


#ifndef __VSWebProjectItem_INTERFACE_DEFINED__
#define __VSWebProjectItem_INTERFACE_DEFINED__

/* interface VSWebProjectItem */
/* [uuid][object][oleautomation][dual] */ 


EXTERN_C const IID IID_VSWebProjectItem;

#if defined(__cplusplus) && !defined(CINTERFACE)
    
    MIDL_INTERFACE("C77EB2D2-F01E-4c87-8C91-262213C40B31")
    VSWebProjectItem : public IDispatch
    {
    public:
        virtual /* [helpstring][propget][id] */ HRESULT STDMETHODCALLTYPE get_DTE( 
            /* [retval][out] */ __RPC__deref_out_opt /* external definition not present */ DTE **ppDTE) = 0;
        
        virtual /* [helpstring][propget][id] */ HRESULT STDMETHODCALLTYPE get_ProjectItem( 
            /* [retval][out] */ __RPC__deref_out_opt /* external definition not present */ ProjectItem **ppProjectItem) = 0;
        
        virtual /* [helpstring][propget][id] */ HRESULT STDMETHODCALLTYPE get_ContainingProject( 
            /* [retval][out] */ __RPC__deref_out_opt /* external definition not present */ Project **ppProject) = 0;
        
        virtual /* [helpstring][id] */ HRESULT STDMETHODCALLTYPE UpdateLocalCopy( 
            /* [retval][out] */ __RPC__deref_out_opt BSTR *pbstrPath) = 0;
        
        virtual /* [helpstring][id] */ HRESULT STDMETHODCALLTYPE UpdateRemoteCopy( void) = 0;
        
        virtual /* [helpstring][id] */ HRESULT STDMETHODCALLTYPE WaitUntilReady( void) = 0;
        
        virtual /* [helpstring][id] */ HRESULT STDMETHODCALLTYPE Load( void) = 0;
        
        virtual /* [helpstring][id] */ HRESULT STDMETHODCALLTYPE Unload( void) = 0;
        
        virtual /* [helpstring][id][propget] */ HRESULT STDMETHODCALLTYPE get_RelatedFiles( 
            /* [retval][out] */ __RPC__deref_out_opt RelatedFiles **ppRelatedFiles) = 0;
        
    };
    
    
#else 	/* C style interface */

    typedef struct VSWebProjectItemVtbl
    {
        BEGIN_INTERFACE
        
        HRESULT ( STDMETHODCALLTYPE *QueryInterface )( 
            __RPC__in VSWebProjectItem * This,
            /* [in] */ __RPC__in REFIID riid,
            /* [annotation][iid_is][out] */ 
            _COM_Outptr_  void **ppvObject);
        
        ULONG ( STDMETHODCALLTYPE *AddRef )( 
            __RPC__in VSWebProjectItem * This);
        
        ULONG ( STDMETHODCALLTYPE *Release )( 
            __RPC__in VSWebProjectItem * This);
        
        HRESULT ( STDMETHODCALLTYPE *GetTypeInfoCount )( 
            __RPC__in VSWebProjectItem * This,
            /* [out] */ __RPC__out UINT *pctinfo);
        
        HRESULT ( STDMETHODCALLTYPE *GetTypeInfo )( 
            __RPC__in VSWebProjectItem * This,
            /* [in] */ UINT iTInfo,
            /* [in] */ LCID lcid,
            /* [out] */ __RPC__deref_out_opt ITypeInfo **ppTInfo);
        
        HRESULT ( STDMETHODCALLTYPE *GetIDsOfNames )( 
            __RPC__in VSWebProjectItem * This,
            /* [in] */ __RPC__in REFIID riid,
            /* [size_is][in] */ __RPC__in_ecount_full(cNames) LPOLESTR *rgszNames,
            /* [range][in] */ __RPC__in_range(0,16384) UINT cNames,
            /* [in] */ LCID lcid,
            /* [size_is][out] */ __RPC__out_ecount_full(cNames) DISPID *rgDispId);
        
        /* [local] */ HRESULT ( STDMETHODCALLTYPE *Invoke )( 
            VSWebProjectItem * This,
            /* [annotation][in] */ 
            _In_  DISPID dispIdMember,
            /* [annotation][in] */ 
            _In_  REFIID riid,
            /* [annotation][in] */ 
            _In_  LCID lcid,
            /* [annotation][in] */ 
            _In_  WORD wFlags,
            /* [annotation][out][in] */ 
            _In_  DISPPARAMS *pDispParams,
            /* [annotation][out] */ 
            _Out_opt_  VARIANT *pVarResult,
            /* [annotation][out] */ 
            _Out_opt_  EXCEPINFO *pExcepInfo,
            /* [annotation][out] */ 
            _Out_opt_  UINT *puArgErr);
        
        /* [helpstring][propget][id] */ HRESULT ( STDMETHODCALLTYPE *get_DTE )( 
            __RPC__in VSWebProjectItem * This,
            /* [retval][out] */ __RPC__deref_out_opt /* external definition not present */ DTE **ppDTE);
        
        /* [helpstring][propget][id] */ HRESULT ( STDMETHODCALLTYPE *get_ProjectItem )( 
            __RPC__in VSWebProjectItem * This,
            /* [retval][out] */ __RPC__deref_out_opt /* external definition not present */ ProjectItem **ppProjectItem);
        
        /* [helpstring][propget][id] */ HRESULT ( STDMETHODCALLTYPE *get_ContainingProject )( 
            __RPC__in VSWebProjectItem * This,
            /* [retval][out] */ __RPC__deref_out_opt /* external definition not present */ Project **ppProject);
        
        /* [helpstring][id] */ HRESULT ( STDMETHODCALLTYPE *UpdateLocalCopy )( 
            __RPC__in VSWebProjectItem * This,
            /* [retval][out] */ __RPC__deref_out_opt BSTR *pbstrPath);
        
        /* [helpstring][id] */ HRESULT ( STDMETHODCALLTYPE *UpdateRemoteCopy )( 
            __RPC__in VSWebProjectItem * This);
        
        /* [helpstring][id] */ HRESULT ( STDMETHODCALLTYPE *WaitUntilReady )( 
            __RPC__in VSWebProjectItem * This);
        
        /* [helpstring][id] */ HRESULT ( STDMETHODCALLTYPE *Load )( 
            __RPC__in VSWebProjectItem * This);
        
        /* [helpstring][id] */ HRESULT ( STDMETHODCALLTYPE *Unload )( 
            __RPC__in VSWebProjectItem * This);
        
        /* [helpstring][id][propget] */ HRESULT ( STDMETHODCALLTYPE *get_RelatedFiles )( 
            __RPC__in VSWebProjectItem * This,
            /* [retval][out] */ __RPC__deref_out_opt RelatedFiles **ppRelatedFiles);
        
        END_INTERFACE
    } VSWebProjectItemVtbl;

    interface VSWebProjectItem
    {
        CONST_VTBL struct VSWebProjectItemVtbl *lpVtbl;
    };

    

#ifdef COBJMACROS


#define VSWebProjectItem_QueryInterface(This,riid,ppvObject)	\
    ( (This)->lpVtbl -> QueryInterface(This,riid,ppvObject) ) 

#define VSWebProjectItem_AddRef(This)	\
    ( (This)->lpVtbl -> AddRef(This) ) 

#define VSWebProjectItem_Release(This)	\
    ( (This)->lpVtbl -> Release(This) ) 


#define VSWebProjectItem_GetTypeInfoCount(This,pctinfo)	\
    ( (This)->lpVtbl -> GetTypeInfoCount(This,pctinfo) ) 

#define VSWebProjectItem_GetTypeInfo(This,iTInfo,lcid,ppTInfo)	\
    ( (This)->lpVtbl -> GetTypeInfo(This,iTInfo,lcid,ppTInfo) ) 

#define VSWebProjectItem_GetIDsOfNames(This,riid,rgszNames,cNames,lcid,rgDispId)	\
    ( (This)->lpVtbl -> GetIDsOfNames(This,riid,rgszNames,cNames,lcid,rgDispId) ) 

#define VSWebProjectItem_Invoke(This,dispIdMember,riid,lcid,wFlags,pDispParams,pVarResult,pExcepInfo,puArgErr)	\
    ( (This)->lpVtbl -> Invoke(This,dispIdMember,riid,lcid,wFlags,pDispParams,pVarResult,pExcepInfo,puArgErr) ) 


#define VSWebProjectItem_get_DTE(This,ppDTE)	\
    ( (This)->lpVtbl -> get_DTE(This,ppDTE) ) 

#define VSWebProjectItem_get_ProjectItem(This,ppProjectItem)	\
    ( (This)->lpVtbl -> get_ProjectItem(This,ppProjectItem) ) 

#define VSWebProjectItem_get_ContainingProject(This,ppProject)	\
    ( (This)->lpVtbl -> get_ContainingProject(This,ppProject) ) 

#define VSWebProjectItem_UpdateLocalCopy(This,pbstrPath)	\
    ( (This)->lpVtbl -> UpdateLocalCopy(This,pbstrPath) ) 

#define VSWebProjectItem_UpdateRemoteCopy(This)	\
    ( (This)->lpVtbl -> UpdateRemoteCopy(This) ) 

#define VSWebProjectItem_WaitUntilReady(This)	\
    ( (This)->lpVtbl -> WaitUntilReady(This) ) 

#define VSWebProjectItem_Load(This)	\
    ( (This)->lpVtbl -> Load(This) ) 

#define VSWebProjectItem_Unload(This)	\
    ( (This)->lpVtbl -> Unload(This) ) 

#define VSWebProjectItem_get_RelatedFiles(This,ppRelatedFiles)	\
    ( (This)->lpVtbl -> get_RelatedFiles(This,ppRelatedFiles) ) 

#endif /* COBJMACROS */


#endif 	/* C style interface */




#endif 	/* __VSWebProjectItem_INTERFACE_DEFINED__ */


#ifndef __VSWebPackage_INTERFACE_DEFINED__
#define __VSWebPackage_INTERFACE_DEFINED__

/* interface VSWebPackage */
/* [uuid][object][oleautomation][dual] */ 


EXTERN_C const IID IID_VSWebPackage;

#if defined(__cplusplus) && !defined(CINTERFACE)
    
    MIDL_INTERFACE("443E6A57-0669-42a7-9B84-51EBF6719D78")
    VSWebPackage : public IDispatch
    {
    public:
        virtual /* [helpstring][propget][id] */ HRESULT STDMETHODCALLTYPE get_DTE( 
            /* [retval][out] */ __RPC__deref_out_opt /* external definition not present */ DTE **ppDTE) = 0;
        
        virtual /* [helpstring][id] */ HRESULT STDMETHODCALLTYPE OpenWebSite( 
            /* [in] */ __RPC__in BSTR bstrUrl,
            /* [in] */ OpenWebsiteOptions options,
            /* [in] */ VARIANT_BOOL bAddToSolution,
            /* [retval][out] */ __RPC__deref_out_opt /* external definition not present */ Project **ppProject) = 0;
        
    };
    
    
#else 	/* C style interface */

    typedef struct VSWebPackageVtbl
    {
        BEGIN_INTERFACE
        
        HRESULT ( STDMETHODCALLTYPE *QueryInterface )( 
            __RPC__in VSWebPackage * This,
            /* [in] */ __RPC__in REFIID riid,
            /* [annotation][iid_is][out] */ 
            _COM_Outptr_  void **ppvObject);
        
        ULONG ( STDMETHODCALLTYPE *AddRef )( 
            __RPC__in VSWebPackage * This);
        
        ULONG ( STDMETHODCALLTYPE *Release )( 
            __RPC__in VSWebPackage * This);
        
        HRESULT ( STDMETHODCALLTYPE *GetTypeInfoCount )( 
            __RPC__in VSWebPackage * This,
            /* [out] */ __RPC__out UINT *pctinfo);
        
        HRESULT ( STDMETHODCALLTYPE *GetTypeInfo )( 
            __RPC__in VSWebPackage * This,
            /* [in] */ UINT iTInfo,
            /* [in] */ LCID lcid,
            /* [out] */ __RPC__deref_out_opt ITypeInfo **ppTInfo);
        
        HRESULT ( STDMETHODCALLTYPE *GetIDsOfNames )( 
            __RPC__in VSWebPackage * This,
            /* [in] */ __RPC__in REFIID riid,
            /* [size_is][in] */ __RPC__in_ecount_full(cNames) LPOLESTR *rgszNames,
            /* [range][in] */ __RPC__in_range(0,16384) UINT cNames,
            /* [in] */ LCID lcid,
            /* [size_is][out] */ __RPC__out_ecount_full(cNames) DISPID *rgDispId);
        
        /* [local] */ HRESULT ( STDMETHODCALLTYPE *Invoke )( 
            VSWebPackage * This,
            /* [annotation][in] */ 
            _In_  DISPID dispIdMember,
            /* [annotation][in] */ 
            _In_  REFIID riid,
            /* [annotation][in] */ 
            _In_  LCID lcid,
            /* [annotation][in] */ 
            _In_  WORD wFlags,
            /* [annotation][out][in] */ 
            _In_  DISPPARAMS *pDispParams,
            /* [annotation][out] */ 
            _Out_opt_  VARIANT *pVarResult,
            /* [annotation][out] */ 
            _Out_opt_  EXCEPINFO *pExcepInfo,
            /* [annotation][out] */ 
            _Out_opt_  UINT *puArgErr);
        
        /* [helpstring][propget][id] */ HRESULT ( STDMETHODCALLTYPE *get_DTE )( 
            __RPC__in VSWebPackage * This,
            /* [retval][out] */ __RPC__deref_out_opt /* external definition not present */ DTE **ppDTE);
        
        /* [helpstring][id] */ HRESULT ( STDMETHODCALLTYPE *OpenWebSite )( 
            __RPC__in VSWebPackage * This,
            /* [in] */ __RPC__in BSTR bstrUrl,
            /* [in] */ OpenWebsiteOptions options,
            /* [in] */ VARIANT_BOOL bAddToSolution,
            /* [retval][out] */ __RPC__deref_out_opt /* external definition not present */ Project **ppProject);
        
        END_INTERFACE
    } VSWebPackageVtbl;

    interface VSWebPackage
    {
        CONST_VTBL struct VSWebPackageVtbl *lpVtbl;
    };

    

#ifdef COBJMACROS


#define VSWebPackage_QueryInterface(This,riid,ppvObject)	\
    ( (This)->lpVtbl -> QueryInterface(This,riid,ppvObject) ) 

#define VSWebPackage_AddRef(This)	\
    ( (This)->lpVtbl -> AddRef(This) ) 

#define VSWebPackage_Release(This)	\
    ( (This)->lpVtbl -> Release(This) ) 


#define VSWebPackage_GetTypeInfoCount(This,pctinfo)	\
    ( (This)->lpVtbl -> GetTypeInfoCount(This,pctinfo) ) 

#define VSWebPackage_GetTypeInfo(This,iTInfo,lcid,ppTInfo)	\
    ( (This)->lpVtbl -> GetTypeInfo(This,iTInfo,lcid,ppTInfo) ) 

#define VSWebPackage_GetIDsOfNames(This,riid,rgszNames,cNames,lcid,rgDispId)	\
    ( (This)->lpVtbl -> GetIDsOfNames(This,riid,rgszNames,cNames,lcid,rgDispId) ) 

#define VSWebPackage_Invoke(This,dispIdMember,riid,lcid,wFlags,pDispParams,pVarResult,pExcepInfo,puArgErr)	\
    ( (This)->lpVtbl -> Invoke(This,dispIdMember,riid,lcid,wFlags,pDispParams,pVarResult,pExcepInfo,puArgErr) ) 


#define VSWebPackage_get_DTE(This,ppDTE)	\
    ( (This)->lpVtbl -> get_DTE(This,ppDTE) ) 

#define VSWebPackage_OpenWebSite(This,bstrUrl,options,bAddToSolution,ppProject)	\
    ( (This)->lpVtbl -> OpenWebSite(This,bstrUrl,options,bAddToSolution,ppProject) ) 

#endif /* COBJMACROS */


#endif 	/* C style interface */




#endif 	/* __VSWebPackage_INTERFACE_DEFINED__ */

#endif /* __VSWebSite_LIBRARY_DEFINED__ */

/* interface __MIDL_itf_WebProperties_0000_0001 */
/* [local] */ 

#ifdef FORCE_EXPLICIT_DTE_NAMESPACE
#undef DTE
#undef Project
#undef ProjectItem
#undef FileCodeModel
#undef CodeModel
#endif


extern RPC_IF_HANDLE __MIDL_itf_WebProperties_0000_0001_v0_0_c_ifspec;
extern RPC_IF_HANDLE __MIDL_itf_WebProperties_0000_0001_v0_0_s_ifspec;

/* Additional Prototypes for ALL interfaces */

/* end of Additional Prototypes */

#ifdef __cplusplus
}
#endif

#endif


