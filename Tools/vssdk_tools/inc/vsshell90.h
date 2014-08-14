

/* this ALWAYS GENERATED file contains the definitions for the interfaces */


 /* File created by MIDL compiler version 7.00.0499 */
/* Compiler settings for vsshell90.idl:
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

#ifndef __vsshell90_h__
#define __vsshell90_h__

#if defined(_MSC_VER) && (_MSC_VER >= 1020)
#pragma once
#endif

/* Forward Declarations */ 

#ifndef __IVsComponentSelectorDlg3_FWD_DEFINED__
#define __IVsComponentSelectorDlg3_FWD_DEFINED__
typedef interface IVsComponentSelectorDlg3 IVsComponentSelectorDlg3;
#endif 	/* __IVsComponentSelectorDlg3_FWD_DEFINED__ */


#ifndef __IVsFindSymbol2_FWD_DEFINED__
#define __IVsFindSymbol2_FWD_DEFINED__
typedef interface IVsFindSymbol2 IVsFindSymbol2;
#endif 	/* __IVsFindSymbol2_FWD_DEFINED__ */


#ifndef __IVsFindSymbolEvents2_FWD_DEFINED__
#define __IVsFindSymbolEvents2_FWD_DEFINED__
typedef interface IVsFindSymbolEvents2 IVsFindSymbolEvents2;
#endif 	/* __IVsFindSymbolEvents2_FWD_DEFINED__ */


#ifndef __IVsWebProxy_FWD_DEFINED__
#define __IVsWebProxy_FWD_DEFINED__
typedef interface IVsWebProxy IVsWebProxy;
#endif 	/* __IVsWebProxy_FWD_DEFINED__ */


#ifndef __SVsWebProxy_FWD_DEFINED__
#define __SVsWebProxy_FWD_DEFINED__
typedef interface SVsWebProxy SVsWebProxy;
#endif 	/* __SVsWebProxy_FWD_DEFINED__ */


#ifndef __IVsToolbox4_FWD_DEFINED__
#define __IVsToolbox4_FWD_DEFINED__
typedef interface IVsToolbox4 IVsToolbox4;
#endif 	/* __IVsToolbox4_FWD_DEFINED__ */


#ifndef __IVsUserSettingsMigration_FWD_DEFINED__
#define __IVsUserSettingsMigration_FWD_DEFINED__
typedef interface IVsUserSettingsMigration IVsUserSettingsMigration;
#endif 	/* __IVsUserSettingsMigration_FWD_DEFINED__ */


#ifndef __IVsShell3_FWD_DEFINED__
#define __IVsShell3_FWD_DEFINED__
typedef interface IVsShell3 IVsShell3;
#endif 	/* __IVsShell3_FWD_DEFINED__ */


#ifndef __IVsUIShell3_FWD_DEFINED__
#define __IVsUIShell3_FWD_DEFINED__
typedef interface IVsUIShell3 IVsUIShell3;
#endif 	/* __IVsUIShell3_FWD_DEFINED__ */


#ifndef __IVsMSBuildHostObject_FWD_DEFINED__
#define __IVsMSBuildHostObject_FWD_DEFINED__
typedef interface IVsMSBuildHostObject IVsMSBuildHostObject;
#endif 	/* __IVsMSBuildHostObject_FWD_DEFINED__ */


#ifndef __IVsMSBuildTaskFileManager_FWD_DEFINED__
#define __IVsMSBuildTaskFileManager_FWD_DEFINED__
typedef interface IVsMSBuildTaskFileManager IVsMSBuildTaskFileManager;
#endif 	/* __IVsMSBuildTaskFileManager_FWD_DEFINED__ */


#ifndef __IVsUpgradeBuildPropertyStorage_FWD_DEFINED__
#define __IVsUpgradeBuildPropertyStorage_FWD_DEFINED__
typedef interface IVsUpgradeBuildPropertyStorage IVsUpgradeBuildPropertyStorage;
#endif 	/* __IVsUpgradeBuildPropertyStorage_FWD_DEFINED__ */


#ifndef __IVsProjectFlavorUpgradeViaFactory_FWD_DEFINED__
#define __IVsProjectFlavorUpgradeViaFactory_FWD_DEFINED__
typedef interface IVsProjectFlavorUpgradeViaFactory IVsProjectFlavorUpgradeViaFactory;
#endif 	/* __IVsProjectFlavorUpgradeViaFactory_FWD_DEFINED__ */


#ifndef __IVsProjectServerHost_FWD_DEFINED__
#define __IVsProjectServerHost_FWD_DEFINED__
typedef interface IVsProjectServerHost IVsProjectServerHost;
#endif 	/* __IVsProjectServerHost_FWD_DEFINED__ */


#ifndef __IVsFileUpgrade2_FWD_DEFINED__
#define __IVsFileUpgrade2_FWD_DEFINED__
typedef interface IVsFileUpgrade2 IVsFileUpgrade2;
#endif 	/* __IVsFileUpgrade2_FWD_DEFINED__ */


#ifndef __IVsSymbolicNavigationNotify_FWD_DEFINED__
#define __IVsSymbolicNavigationNotify_FWD_DEFINED__
typedef interface IVsSymbolicNavigationNotify IVsSymbolicNavigationNotify;
#endif 	/* __IVsSymbolicNavigationNotify_FWD_DEFINED__ */


#ifndef __SVsSymbolicNavigationManager_FWD_DEFINED__
#define __SVsSymbolicNavigationManager_FWD_DEFINED__
typedef interface SVsSymbolicNavigationManager SVsSymbolicNavigationManager;
#endif 	/* __SVsSymbolicNavigationManager_FWD_DEFINED__ */


#ifndef __IVsSymbolicNavigationManager_FWD_DEFINED__
#define __IVsSymbolicNavigationManager_FWD_DEFINED__
typedef interface IVsSymbolicNavigationManager IVsSymbolicNavigationManager;
#endif 	/* __IVsSymbolicNavigationManager_FWD_DEFINED__ */


#ifndef __IVsOutputWindowPane2_FWD_DEFINED__
#define __IVsOutputWindowPane2_FWD_DEFINED__
typedef interface IVsOutputWindowPane2 IVsOutputWindowPane2;
#endif 	/* __IVsOutputWindowPane2_FWD_DEFINED__ */


#ifndef __ILocalRegistry4_FWD_DEFINED__
#define __ILocalRegistry4_FWD_DEFINED__
typedef interface ILocalRegistry4 ILocalRegistry4;
#endif 	/* __ILocalRegistry4_FWD_DEFINED__ */


#ifndef __IVsEditorFactory2_FWD_DEFINED__
#define __IVsEditorFactory2_FWD_DEFINED__
typedef interface IVsEditorFactory2 IVsEditorFactory2;
#endif 	/* __IVsEditorFactory2_FWD_DEFINED__ */


#ifndef __VsSymbolicNavigationManager_FWD_DEFINED__
#define __VsSymbolicNavigationManager_FWD_DEFINED__

#ifdef __cplusplus
typedef class VsSymbolicNavigationManager VsSymbolicNavigationManager;
#else
typedef struct VsSymbolicNavigationManager VsSymbolicNavigationManager;
#endif /* __cplusplus */

#endif 	/* __VsSymbolicNavigationManager_FWD_DEFINED__ */


#ifndef __VsMSBuildTaskFileManager_FWD_DEFINED__
#define __VsMSBuildTaskFileManager_FWD_DEFINED__

#ifdef __cplusplus
typedef class VsMSBuildTaskFileManager VsMSBuildTaskFileManager;
#else
typedef struct VsMSBuildTaskFileManager VsMSBuildTaskFileManager;
#endif /* __cplusplus */

#endif 	/* __VsMSBuildTaskFileManager_FWD_DEFINED__ */


#ifndef __IOleComponent2_FWD_DEFINED__
#define __IOleComponent2_FWD_DEFINED__
typedef interface IOleComponent2 IOleComponent2;
#endif 	/* __IOleComponent2_FWD_DEFINED__ */


#ifndef __IVsPackage2_FWD_DEFINED__
#define __IVsPackage2_FWD_DEFINED__
typedef interface IVsPackage2 IVsPackage2;
#endif 	/* __IVsPackage2_FWD_DEFINED__ */


#ifndef __IVsLaunchPad3_FWD_DEFINED__
#define __IVsLaunchPad3_FWD_DEFINED__
typedef interface IVsLaunchPad3 IVsLaunchPad3;
#endif 	/* __IVsLaunchPad3_FWD_DEFINED__ */


/* header files for imported files */
#include "oaidl.h"
#include "vsshell.h"
#include "vsshell2.h"
#include "vsshell80.h"
#include "objext.h"
#include "olecm.h"

#ifdef __cplusplus
extern "C"{
#endif 


/* interface __MIDL_itf_vsshell90_0000_0000 */
/* [local] */ 

#pragma once

enum __VSPROPID3
    {	VSPROPID_IsSavingOnClose	= -8027,
	VSPROPID_FIRST3	= -8027
    } ;
typedef /* [public] */ DWORD VSPROPID3;


enum __VSFPROPID3
    {	VSFPROPID_MDIContainerID	= -5010,
	VSFPROPID_NotifyOnActivate	= -5011,
	VSFPROPID3_FIRST	= -5011
    } ;
typedef LONG VSFPROPID3;


enum __FRAMESHOW3
    {	FRAMESHOW_WinActivated	= 12
    } ;
typedef BOOL FRAMESHOW3;


enum _LIBCAT_HIERARCHYTYPE2
    {	LCHT_EXTENSIONMEMBERS	= 0x40
    } ;

enum __PSFFILEID3
    {	PSFFILEID_AppXaml	= -1008,
	PSFFILEID_FIRST3	= -1008
    } ;
typedef LONG PSFFILEID3;

typedef DWORD LIBCAT_HIERARCHYTYPE2;

#define COUNT_LIBCAT_HIERARCHYTYPE2 7
typedef /* [public] */ DWORD TARGETFRAMEWORKVERSION;

#define TARGETFRAMEWORKVERSION_UNKNOWN 0
#define TARGETFRAMEWORKVERSION_20     0x00020000
#define TARGETFRAMEWORKVERSION_30     0x00030000
#define TARGETFRAMEWORKVERSION_35     0x00030005
#define TARGETFRAMEWORKVERSION_40     0x00040000

enum WellKnownTargetFrameworkVersions
    {	TargetFrameworkVersion_Unknown	= 0,
	TargetFrameworkVersion_20	= 0x20000,
	TargetFrameworkVersion_30	= 0x30000,
	TargetFrameworkVersion_35	= 0x30005,
	TargetFrameworkVersion_40	= 0x40000
    } ;
#define VS_TBXITEMINFO_FORMAT_NAME L"VSToolBoxItemInfo"
typedef 
enum __tagGRADIENTTYPE2
    {	VSGRADIENT_FILETAB_SELECTED	= 9,
	VSGRADIENT_FILETAB_HOT	= 10
    } 	__GRADIENTTYPE2;

typedef DWORD GRADIENTTYPE2;

typedef 
enum __tagVSSYSCOLOREX2
    {	VSCOLOR_SPLASHSCREEN_BORDER	= -179,
	VSCOLOR_FILETAB_SELECTED_GRADIENTTOP	= -180,
	VSCOLOR_FILETAB_SELECTED_GRADIENTBOTTOM	= -181,
	VSCOLOR_FILETAB_HOT_GRADIENTTOP	= -182,
	VSCOLOR_FILETAB_HOT_GRADIENTBOTTOM	= -183,
	VSCOLOR_FILETAB_DOCUMENTBORDER_SHADOW	= -184,
	VSCOLOR_FILETAB_DOCUMENTBORDER_BACKGROUND	= -185,
	VSCOLOR_FILETAB_DOCUMENTBORDER_HIGHLIGHT	= -186,
	VSCOLOR_BRANDEDUI_TITLE	= -187,
	VSCOLOR_BRANDEDUI_BORDER	= -188,
	VSCOLOR_BRANDEDUI_TEXT	= -189,
	VSCOLOR_BRANDEDUI_BACKGROUND	= -190,
	VSCOLOR_BRANDEDUI_FILL	= -191,
	VSCOLOR_LASTEX2	= -191
    } 	__VSSYSCOLOREX2;


enum __VSSPROPID3
    {	VSSPROPID_CommonAppDataDir	= -9052,
	VSSPROPID_FIRST3	= -9052
    } ;
typedef LONG VSSPROPID3;


enum __VSHPROPID3
    {	VSHPROPID_TargetFrameworkVersion	= -2093,
	VSHPROPID_WebReferenceSupported	= -2094,
	VSHPROPID_ServiceReferenceSupported	= -2095,
	VSHPROPID_SupportsHierarchicalUpdate	= -2096,
	VSHPROPID_SupportsNTierDesigner	= -2097,
	VSHPROPID_SupportsLinqOverDataSet	= -2098,
	VSHPROPID_ProductBrandName	= -2099,
	VSHPROPID_RefactorExtensions	= -2100,
	VSHPROPID_IsDefaultNamespaceRefactorNotify	= -2101,
	VSHPROPID_FIRST3	= -2101
    } ;
typedef /* [public] */ DWORD VSHPROPID3;



extern RPC_IF_HANDLE __MIDL_itf_vsshell90_0000_0000_v0_0_c_ifspec;
extern RPC_IF_HANDLE __MIDL_itf_vsshell90_0000_0000_v0_0_s_ifspec;

#ifndef __IVsComponentSelectorDlg3_INTERFACE_DEFINED__
#define __IVsComponentSelectorDlg3_INTERFACE_DEFINED__

/* interface IVsComponentSelectorDlg3 */
/* [object][unique][version][uuid] */ 


EXTERN_C const IID IID_IVsComponentSelectorDlg3;

#if defined(__cplusplus) && !defined(CINTERFACE)
    
    MIDL_INTERFACE("00AE51BD-26D7-4974-801B-B0890E1753F8")
    IVsComponentSelectorDlg3 : public IUnknown
    {
    public:
        virtual HRESULT STDMETHODCALLTYPE ComponentSelectorDlg3( 
            /* [in] */ VSCOMPSELFLAGS grfFlags,
            /* [in] */ __RPC__in_opt IVsComponentUser *pUser,
            /* [in] */ __RPC__in LPCOLESTR lpszDlgTitle,
            /* [in] */ __RPC__in LPCOLESTR lpszHelpTopic,
            /* [in] */ __RPC__in REFGUID rguidShowOnlyThisTab,
            /* [in] */ __RPC__in REFGUID rguidStartOnThisTab,
            /* [in] */ __RPC__in LPCOLESTR pszMachineName,
            /* [in] */ ULONG cTabInitializers,
            /* [size_is][in] */ __RPC__in_ecount_full(cTabInitializers) VSCOMPONENTSELECTORTABINIT *prgcstiTabInitializers,
            /* [in] */ __RPC__in LPCOLESTR pszBrowseFilters,
            /* [out][in] */ __RPC__deref_inout_opt BSTR *pbstrBrowseLocation,
            /* [in] */ TARGETFRAMEWORKVERSION targetVersion) = 0;
        
        virtual HRESULT STDMETHODCALLTYPE ComponentSelectorDlg4( 
            /* [in] */ VSCOMPSELFLAGS2 grfFlags,
            /* [in] */ __RPC__in_opt IVsComponentUser *pUser,
            /* [in] */ ULONG cComponents,
            /* [size_is][in] */ __RPC__in_ecount_full(cComponents) PVSCOMPONENTSELECTORDATA rgpcsdComponents[  ],
            /* [in] */ __RPC__in LPCOLESTR lpszDlgTitle,
            /* [in] */ __RPC__in LPCOLESTR lpszHelpTopic,
            /* [out][in] */ __RPC__inout ULONG *pxDlgSize,
            /* [out][in] */ __RPC__inout ULONG *pyDlgSize,
            /* [in] */ ULONG cTabInitializers,
            /* [size_is][in] */ __RPC__in_ecount_full(cTabInitializers) VSCOMPONENTSELECTORTABINIT rgcstiTabInitializers[  ],
            /* [out][in] */ __RPC__inout GUID *pguidStartOnThisTab,
            /* [in] */ __RPC__in LPCOLESTR pszBrowseFilters,
            /* [out][in] */ __RPC__deref_inout_opt BSTR *pbstrBrowseLocation,
            /* [in] */ TARGETFRAMEWORKVERSION targetVersion) = 0;
        
    };
    
#else 	/* C style interface */

    typedef struct IVsComponentSelectorDlg3Vtbl
    {
        BEGIN_INTERFACE
        
        HRESULT ( STDMETHODCALLTYPE *QueryInterface )( 
            IVsComponentSelectorDlg3 * This,
            /* [in] */ __RPC__in REFIID riid,
            /* [iid_is][out] */ 
            __RPC__deref_out  void **ppvObject);
        
        ULONG ( STDMETHODCALLTYPE *AddRef )( 
            IVsComponentSelectorDlg3 * This);
        
        ULONG ( STDMETHODCALLTYPE *Release )( 
            IVsComponentSelectorDlg3 * This);
        
        HRESULT ( STDMETHODCALLTYPE *ComponentSelectorDlg3 )( 
            IVsComponentSelectorDlg3 * This,
            /* [in] */ VSCOMPSELFLAGS grfFlags,
            /* [in] */ __RPC__in_opt IVsComponentUser *pUser,
            /* [in] */ __RPC__in LPCOLESTR lpszDlgTitle,
            /* [in] */ __RPC__in LPCOLESTR lpszHelpTopic,
            /* [in] */ __RPC__in REFGUID rguidShowOnlyThisTab,
            /* [in] */ __RPC__in REFGUID rguidStartOnThisTab,
            /* [in] */ __RPC__in LPCOLESTR pszMachineName,
            /* [in] */ ULONG cTabInitializers,
            /* [size_is][in] */ __RPC__in_ecount_full(cTabInitializers) VSCOMPONENTSELECTORTABINIT *prgcstiTabInitializers,
            /* [in] */ __RPC__in LPCOLESTR pszBrowseFilters,
            /* [out][in] */ __RPC__deref_inout_opt BSTR *pbstrBrowseLocation,
            /* [in] */ TARGETFRAMEWORKVERSION targetVersion);
        
        HRESULT ( STDMETHODCALLTYPE *ComponentSelectorDlg4 )( 
            IVsComponentSelectorDlg3 * This,
            /* [in] */ VSCOMPSELFLAGS2 grfFlags,
            /* [in] */ __RPC__in_opt IVsComponentUser *pUser,
            /* [in] */ ULONG cComponents,
            /* [size_is][in] */ __RPC__in_ecount_full(cComponents) PVSCOMPONENTSELECTORDATA rgpcsdComponents[  ],
            /* [in] */ __RPC__in LPCOLESTR lpszDlgTitle,
            /* [in] */ __RPC__in LPCOLESTR lpszHelpTopic,
            /* [out][in] */ __RPC__inout ULONG *pxDlgSize,
            /* [out][in] */ __RPC__inout ULONG *pyDlgSize,
            /* [in] */ ULONG cTabInitializers,
            /* [size_is][in] */ __RPC__in_ecount_full(cTabInitializers) VSCOMPONENTSELECTORTABINIT rgcstiTabInitializers[  ],
            /* [out][in] */ __RPC__inout GUID *pguidStartOnThisTab,
            /* [in] */ __RPC__in LPCOLESTR pszBrowseFilters,
            /* [out][in] */ __RPC__deref_inout_opt BSTR *pbstrBrowseLocation,
            /* [in] */ TARGETFRAMEWORKVERSION targetVersion);
        
        END_INTERFACE
    } IVsComponentSelectorDlg3Vtbl;

    interface IVsComponentSelectorDlg3
    {
        CONST_VTBL struct IVsComponentSelectorDlg3Vtbl *lpVtbl;
    };

    

#ifdef COBJMACROS


#define IVsComponentSelectorDlg3_QueryInterface(This,riid,ppvObject)	\
    ( (This)->lpVtbl -> QueryInterface(This,riid,ppvObject) ) 

#define IVsComponentSelectorDlg3_AddRef(This)	\
    ( (This)->lpVtbl -> AddRef(This) ) 

#define IVsComponentSelectorDlg3_Release(This)	\
    ( (This)->lpVtbl -> Release(This) ) 


#define IVsComponentSelectorDlg3_ComponentSelectorDlg3(This,grfFlags,pUser,lpszDlgTitle,lpszHelpTopic,rguidShowOnlyThisTab,rguidStartOnThisTab,pszMachineName,cTabInitializers,prgcstiTabInitializers,pszBrowseFilters,pbstrBrowseLocation,targetVersion)	\
    ( (This)->lpVtbl -> ComponentSelectorDlg3(This,grfFlags,pUser,lpszDlgTitle,lpszHelpTopic,rguidShowOnlyThisTab,rguidStartOnThisTab,pszMachineName,cTabInitializers,prgcstiTabInitializers,pszBrowseFilters,pbstrBrowseLocation,targetVersion) ) 

#define IVsComponentSelectorDlg3_ComponentSelectorDlg4(This,grfFlags,pUser,cComponents,rgpcsdComponents,lpszDlgTitle,lpszHelpTopic,pxDlgSize,pyDlgSize,cTabInitializers,rgcstiTabInitializers,pguidStartOnThisTab,pszBrowseFilters,pbstrBrowseLocation,targetVersion)	\
    ( (This)->lpVtbl -> ComponentSelectorDlg4(This,grfFlags,pUser,cComponents,rgpcsdComponents,lpszDlgTitle,lpszHelpTopic,pxDlgSize,pyDlgSize,cTabInitializers,rgcstiTabInitializers,pguidStartOnThisTab,pszBrowseFilters,pbstrBrowseLocation,targetVersion) ) 

#endif /* COBJMACROS */


#endif 	/* C style interface */




#endif 	/* __IVsComponentSelectorDlg3_INTERFACE_DEFINED__ */


#ifndef __IVsFindSymbol2_INTERFACE_DEFINED__
#define __IVsFindSymbol2_INTERFACE_DEFINED__

/* interface IVsFindSymbol2 */
/* [object][unique][version][uuid] */ 


EXTERN_C const IID IID_IVsFindSymbol2;

#if defined(__cplusplus) && !defined(CINTERFACE)
    
    MIDL_INTERFACE("7FF85072-4667-4532-B149-63A7B205060B")
    IVsFindSymbol2 : public IUnknown
    {
    public:
        virtual HRESULT STDMETHODCALLTYPE GetUserOptions( 
            /* [out] */ __RPC__out GUID *pguidScope,
            /* [out] */ __RPC__out DWORD *pdwSubID,
            /* [out] */ __RPC__out VSOBSEARCHCRITERIA2 *pobSrch) = 0;
        
        virtual HRESULT STDMETHODCALLTYPE SetUserOptions( 
            /* [in] */ __RPC__in REFGUID guidScope,
            /* [in] */ DWORD dwSubID,
            /* [in] */ __RPC__in const VSOBSEARCHCRITERIA2 *pobSrch) = 0;
        
        virtual HRESULT STDMETHODCALLTYPE DoSearch( 
            /* [in] */ __RPC__in REFGUID guidSymbolScope,
            /* [in] */ DWORD dwSubID,
            /* [in] */ __RPC__in const VSOBSEARCHCRITERIA2 *pobSrch) = 0;
        
    };
    
#else 	/* C style interface */

    typedef struct IVsFindSymbol2Vtbl
    {
        BEGIN_INTERFACE
        
        HRESULT ( STDMETHODCALLTYPE *QueryInterface )( 
            IVsFindSymbol2 * This,
            /* [in] */ __RPC__in REFIID riid,
            /* [iid_is][out] */ 
            __RPC__deref_out  void **ppvObject);
        
        ULONG ( STDMETHODCALLTYPE *AddRef )( 
            IVsFindSymbol2 * This);
        
        ULONG ( STDMETHODCALLTYPE *Release )( 
            IVsFindSymbol2 * This);
        
        HRESULT ( STDMETHODCALLTYPE *GetUserOptions )( 
            IVsFindSymbol2 * This,
            /* [out] */ __RPC__out GUID *pguidScope,
            /* [out] */ __RPC__out DWORD *pdwSubID,
            /* [out] */ __RPC__out VSOBSEARCHCRITERIA2 *pobSrch);
        
        HRESULT ( STDMETHODCALLTYPE *SetUserOptions )( 
            IVsFindSymbol2 * This,
            /* [in] */ __RPC__in REFGUID guidScope,
            /* [in] */ DWORD dwSubID,
            /* [in] */ __RPC__in const VSOBSEARCHCRITERIA2 *pobSrch);
        
        HRESULT ( STDMETHODCALLTYPE *DoSearch )( 
            IVsFindSymbol2 * This,
            /* [in] */ __RPC__in REFGUID guidSymbolScope,
            /* [in] */ DWORD dwSubID,
            /* [in] */ __RPC__in const VSOBSEARCHCRITERIA2 *pobSrch);
        
        END_INTERFACE
    } IVsFindSymbol2Vtbl;

    interface IVsFindSymbol2
    {
        CONST_VTBL struct IVsFindSymbol2Vtbl *lpVtbl;
    };

    

#ifdef COBJMACROS


#define IVsFindSymbol2_QueryInterface(This,riid,ppvObject)	\
    ( (This)->lpVtbl -> QueryInterface(This,riid,ppvObject) ) 

#define IVsFindSymbol2_AddRef(This)	\
    ( (This)->lpVtbl -> AddRef(This) ) 

#define IVsFindSymbol2_Release(This)	\
    ( (This)->lpVtbl -> Release(This) ) 


#define IVsFindSymbol2_GetUserOptions(This,pguidScope,pdwSubID,pobSrch)	\
    ( (This)->lpVtbl -> GetUserOptions(This,pguidScope,pdwSubID,pobSrch) ) 

#define IVsFindSymbol2_SetUserOptions(This,guidScope,dwSubID,pobSrch)	\
    ( (This)->lpVtbl -> SetUserOptions(This,guidScope,dwSubID,pobSrch) ) 

#define IVsFindSymbol2_DoSearch(This,guidSymbolScope,dwSubID,pobSrch)	\
    ( (This)->lpVtbl -> DoSearch(This,guidSymbolScope,dwSubID,pobSrch) ) 

#endif /* COBJMACROS */


#endif 	/* C style interface */




#endif 	/* __IVsFindSymbol2_INTERFACE_DEFINED__ */


#ifndef __IVsFindSymbolEvents2_INTERFACE_DEFINED__
#define __IVsFindSymbolEvents2_INTERFACE_DEFINED__

/* interface IVsFindSymbolEvents2 */
/* [object][version][uuid] */ 


EXTERN_C const IID IID_IVsFindSymbolEvents2;

#if defined(__cplusplus) && !defined(CINTERFACE)
    
    MIDL_INTERFACE("18220DB2-1AEB-44ea-A924-F3571D202EF4")
    IVsFindSymbolEvents2 : public IUnknown
    {
    public:
        virtual HRESULT STDMETHODCALLTYPE OnUserOptionsChanged( 
            /* [in] */ __RPC__in REFGUID guidSymbolScope,
            /* [in] */ DWORD dwSubID,
            /* [in] */ __RPC__in const VSOBSEARCHCRITERIA2 *pobSrch) = 0;
        
    };
    
#else 	/* C style interface */

    typedef struct IVsFindSymbolEvents2Vtbl
    {
        BEGIN_INTERFACE
        
        HRESULT ( STDMETHODCALLTYPE *QueryInterface )( 
            IVsFindSymbolEvents2 * This,
            /* [in] */ __RPC__in REFIID riid,
            /* [iid_is][out] */ 
            __RPC__deref_out  void **ppvObject);
        
        ULONG ( STDMETHODCALLTYPE *AddRef )( 
            IVsFindSymbolEvents2 * This);
        
        ULONG ( STDMETHODCALLTYPE *Release )( 
            IVsFindSymbolEvents2 * This);
        
        HRESULT ( STDMETHODCALLTYPE *OnUserOptionsChanged )( 
            IVsFindSymbolEvents2 * This,
            /* [in] */ __RPC__in REFGUID guidSymbolScope,
            /* [in] */ DWORD dwSubID,
            /* [in] */ __RPC__in const VSOBSEARCHCRITERIA2 *pobSrch);
        
        END_INTERFACE
    } IVsFindSymbolEvents2Vtbl;

    interface IVsFindSymbolEvents2
    {
        CONST_VTBL struct IVsFindSymbolEvents2Vtbl *lpVtbl;
    };

    

#ifdef COBJMACROS


#define IVsFindSymbolEvents2_QueryInterface(This,riid,ppvObject)	\
    ( (This)->lpVtbl -> QueryInterface(This,riid,ppvObject) ) 

#define IVsFindSymbolEvents2_AddRef(This)	\
    ( (This)->lpVtbl -> AddRef(This) ) 

#define IVsFindSymbolEvents2_Release(This)	\
    ( (This)->lpVtbl -> Release(This) ) 


#define IVsFindSymbolEvents2_OnUserOptionsChanged(This,guidSymbolScope,dwSubID,pobSrch)	\
    ( (This)->lpVtbl -> OnUserOptionsChanged(This,guidSymbolScope,dwSubID,pobSrch) ) 

#endif /* COBJMACROS */


#endif 	/* C style interface */




#endif 	/* __IVsFindSymbolEvents2_INTERFACE_DEFINED__ */


/* interface __MIDL_itf_vsshell90_0000_0003 */
/* [local] */ 


enum _DEBUG_LAUNCH_OPERATION3
    {	DLO_LaunchBrowser	= 6
    } ;

enum __VsWebProxyState
    {	VsWebProxyState_NoCredentials	= 0,
	VsWebProxyState_DefaultCredentials	= 1,
	VsWebProxyState_CachedCredentials	= 2,
	VsWebProxyState_PromptForCredentials	= 3,
	VsWebProxyState_Abort	= 4
    } ;
typedef DWORD VsWebProxyState;



extern RPC_IF_HANDLE __MIDL_itf_vsshell90_0000_0003_v0_0_c_ifspec;
extern RPC_IF_HANDLE __MIDL_itf_vsshell90_0000_0003_v0_0_s_ifspec;

#ifndef __IVsWebProxy_INTERFACE_DEFINED__
#define __IVsWebProxy_INTERFACE_DEFINED__

/* interface IVsWebProxy */
/* [object][unique][version][uuid] */ 


EXTERN_C const IID IID_IVsWebProxy;

#if defined(__cplusplus) && !defined(CINTERFACE)
    
    MIDL_INTERFACE("62A84AEF-423D-4827-833F-7918753C0269")
    IVsWebProxy : public IUnknown
    {
    public:
        virtual HRESULT STDMETHODCALLTYPE PrepareWebProxy( 
            /* [in] */ __RPC__in BSTR strWebCallUrl,
            /* [in] */ VsWebProxyState oldProxyState,
            /* [out] */ __RPC__out VsWebProxyState *newProxyState,
            /* [in] */ BOOL fOkToPrompt) = 0;
        
    };
    
#else 	/* C style interface */

    typedef struct IVsWebProxyVtbl
    {
        BEGIN_INTERFACE
        
        HRESULT ( STDMETHODCALLTYPE *QueryInterface )( 
            IVsWebProxy * This,
            /* [in] */ __RPC__in REFIID riid,
            /* [iid_is][out] */ 
            __RPC__deref_out  void **ppvObject);
        
        ULONG ( STDMETHODCALLTYPE *AddRef )( 
            IVsWebProxy * This);
        
        ULONG ( STDMETHODCALLTYPE *Release )( 
            IVsWebProxy * This);
        
        HRESULT ( STDMETHODCALLTYPE *PrepareWebProxy )( 
            IVsWebProxy * This,
            /* [in] */ __RPC__in BSTR strWebCallUrl,
            /* [in] */ VsWebProxyState oldProxyState,
            /* [out] */ __RPC__out VsWebProxyState *newProxyState,
            /* [in] */ BOOL fOkToPrompt);
        
        END_INTERFACE
    } IVsWebProxyVtbl;

    interface IVsWebProxy
    {
        CONST_VTBL struct IVsWebProxyVtbl *lpVtbl;
    };

    

#ifdef COBJMACROS


#define IVsWebProxy_QueryInterface(This,riid,ppvObject)	\
    ( (This)->lpVtbl -> QueryInterface(This,riid,ppvObject) ) 

#define IVsWebProxy_AddRef(This)	\
    ( (This)->lpVtbl -> AddRef(This) ) 

#define IVsWebProxy_Release(This)	\
    ( (This)->lpVtbl -> Release(This) ) 


#define IVsWebProxy_PrepareWebProxy(This,strWebCallUrl,oldProxyState,newProxyState,fOkToPrompt)	\
    ( (This)->lpVtbl -> PrepareWebProxy(This,strWebCallUrl,oldProxyState,newProxyState,fOkToPrompt) ) 

#endif /* COBJMACROS */


#endif 	/* C style interface */




#endif 	/* __IVsWebProxy_INTERFACE_DEFINED__ */


#ifndef __SVsWebProxy_INTERFACE_DEFINED__
#define __SVsWebProxy_INTERFACE_DEFINED__

/* interface SVsWebProxy */
/* [object][uuid] */ 


EXTERN_C const IID IID_SVsWebProxy;

#if defined(__cplusplus) && !defined(CINTERFACE)
    
    MIDL_INTERFACE("6454F7CA-478C-4932-B007-ABB100905BD4")
    SVsWebProxy : public IUnknown
    {
    public:
    };
    
#else 	/* C style interface */

    typedef struct SVsWebProxyVtbl
    {
        BEGIN_INTERFACE
        
        HRESULT ( STDMETHODCALLTYPE *QueryInterface )( 
            SVsWebProxy * This,
            /* [in] */ __RPC__in REFIID riid,
            /* [iid_is][out] */ 
            __RPC__deref_out  void **ppvObject);
        
        ULONG ( STDMETHODCALLTYPE *AddRef )( 
            SVsWebProxy * This);
        
        ULONG ( STDMETHODCALLTYPE *Release )( 
            SVsWebProxy * This);
        
        END_INTERFACE
    } SVsWebProxyVtbl;

    interface SVsWebProxy
    {
        CONST_VTBL struct SVsWebProxyVtbl *lpVtbl;
    };

    

#ifdef COBJMACROS


#define SVsWebProxy_QueryInterface(This,riid,ppvObject)	\
    ( (This)->lpVtbl -> QueryInterface(This,riid,ppvObject) ) 

#define SVsWebProxy_AddRef(This)	\
    ( (This)->lpVtbl -> AddRef(This) ) 

#define SVsWebProxy_Release(This)	\
    ( (This)->lpVtbl -> Release(This) ) 


#endif /* COBJMACROS */


#endif 	/* C style interface */




#endif 	/* __SVsWebProxy_INTERFACE_DEFINED__ */


/* interface __MIDL_itf_vsshell90_0000_0005 */
/* [local] */ 

#define SID_SVsWebProxy IID_SVsWebProxy
#ifndef _SCC_STATUS_DEFINED
#define _SCC_STATUS_DEFINED

enum __SccStatus
    {	SCC_STATUS_INVALID	= -1L,
	SCC_STATUS_NOTCONTROLLED	= 0L,
	SCC_STATUS_CONTROLLED	= 0x1L,
	SCC_STATUS_CHECKEDOUT	= 0x2L,
	SCC_STATUS_OUTOTHER	= 0x4L,
	SCC_STATUS_OUTEXCLUSIVE	= 0x8L,
	SCC_STATUS_OUTMULTIPLE	= 0x10L,
	SCC_STATUS_OUTOFDATE	= 0x20L,
	SCC_STATUS_DELETED	= 0x40L,
	SCC_STATUS_LOCKED	= 0x80L,
	SCC_STATUS_MERGED	= 0x100L,
	SCC_STATUS_SHARED	= 0x200L,
	SCC_STATUS_PINNED	= 0x400L,
	SCC_STATUS_MODIFIED	= 0x800L,
	SCC_STATUS_OUTBYUSER	= 0x1000L,
	SCC_STATUS_NOMERGE	= 0x2000L,
	SCC_STATUS_RESERVED_1	= 0x4000L,
	SCC_STATUS_RESERVED_2	= 0x8000L,
	SCC_STATUS_RESERVED_3	= 0x10000L
    } ;
typedef DWORD SccStatus;

#endif /* _SCC_STATUS_DEFINED */


extern RPC_IF_HANDLE __MIDL_itf_vsshell90_0000_0005_v0_0_c_ifspec;
extern RPC_IF_HANDLE __MIDL_itf_vsshell90_0000_0005_v0_0_s_ifspec;

#ifndef __IVsToolbox4_INTERFACE_DEFINED__
#define __IVsToolbox4_INTERFACE_DEFINED__

/* interface IVsToolbox4 */
/* [object][unique][version][uuid] */ 


EXTERN_C const IID IID_IVsToolbox4;

#if defined(__cplusplus) && !defined(CINTERFACE)
    
    MIDL_INTERFACE("8A9899AE-041F-431b-85D0-E44F35D65CDD")
    IVsToolbox4 : public IUnknown
    {
    public:
        virtual HRESULT STDMETHODCALLTYPE ExpandTab( 
            /* [in] */ __RPC__in LPCOLESTR lpszTabID,
            /* [in] */ BOOL fExpand) = 0;
        
    };
    
#else 	/* C style interface */

    typedef struct IVsToolbox4Vtbl
    {
        BEGIN_INTERFACE
        
        HRESULT ( STDMETHODCALLTYPE *QueryInterface )( 
            IVsToolbox4 * This,
            /* [in] */ __RPC__in REFIID riid,
            /* [iid_is][out] */ 
            __RPC__deref_out  void **ppvObject);
        
        ULONG ( STDMETHODCALLTYPE *AddRef )( 
            IVsToolbox4 * This);
        
        ULONG ( STDMETHODCALLTYPE *Release )( 
            IVsToolbox4 * This);
        
        HRESULT ( STDMETHODCALLTYPE *ExpandTab )( 
            IVsToolbox4 * This,
            /* [in] */ __RPC__in LPCOLESTR lpszTabID,
            /* [in] */ BOOL fExpand);
        
        END_INTERFACE
    } IVsToolbox4Vtbl;

    interface IVsToolbox4
    {
        CONST_VTBL struct IVsToolbox4Vtbl *lpVtbl;
    };

    

#ifdef COBJMACROS


#define IVsToolbox4_QueryInterface(This,riid,ppvObject)	\
    ( (This)->lpVtbl -> QueryInterface(This,riid,ppvObject) ) 

#define IVsToolbox4_AddRef(This)	\
    ( (This)->lpVtbl -> AddRef(This) ) 

#define IVsToolbox4_Release(This)	\
    ( (This)->lpVtbl -> Release(This) ) 


#define IVsToolbox4_ExpandTab(This,lpszTabID,fExpand)	\
    ( (This)->lpVtbl -> ExpandTab(This,lpszTabID,fExpand) ) 

#endif /* COBJMACROS */


#endif 	/* C style interface */




#endif 	/* __IVsToolbox4_INTERFACE_DEFINED__ */


#ifndef __IVsUserSettingsMigration_INTERFACE_DEFINED__
#define __IVsUserSettingsMigration_INTERFACE_DEFINED__

/* interface IVsUserSettingsMigration */
/* [object][unique][version][uuid] */ 


EXTERN_C const IID IID_IVsUserSettingsMigration;

#if defined(__cplusplus) && !defined(CINTERFACE)
    
    MIDL_INTERFACE("692E1EBF-9D60-4a56-B10F-596DC48CC230")
    IVsUserSettingsMigration : public IUnknown
    {
    public:
        virtual HRESULT STDMETHODCALLTYPE MigrateSettings( 
            /* [in] */ __RPC__in_opt IVsSettingsReader *pSettingsReader,
            /* [in] */ __RPC__in_opt IVsSettingsWriter *pSettingsWriter,
            __RPC__in LPCWSTR pszGuidCategory) = 0;
        
    };
    
#else 	/* C style interface */

    typedef struct IVsUserSettingsMigrationVtbl
    {
        BEGIN_INTERFACE
        
        HRESULT ( STDMETHODCALLTYPE *QueryInterface )( 
            IVsUserSettingsMigration * This,
            /* [in] */ __RPC__in REFIID riid,
            /* [iid_is][out] */ 
            __RPC__deref_out  void **ppvObject);
        
        ULONG ( STDMETHODCALLTYPE *AddRef )( 
            IVsUserSettingsMigration * This);
        
        ULONG ( STDMETHODCALLTYPE *Release )( 
            IVsUserSettingsMigration * This);
        
        HRESULT ( STDMETHODCALLTYPE *MigrateSettings )( 
            IVsUserSettingsMigration * This,
            /* [in] */ __RPC__in_opt IVsSettingsReader *pSettingsReader,
            /* [in] */ __RPC__in_opt IVsSettingsWriter *pSettingsWriter,
            __RPC__in LPCWSTR pszGuidCategory);
        
        END_INTERFACE
    } IVsUserSettingsMigrationVtbl;

    interface IVsUserSettingsMigration
    {
        CONST_VTBL struct IVsUserSettingsMigrationVtbl *lpVtbl;
    };

    

#ifdef COBJMACROS


#define IVsUserSettingsMigration_QueryInterface(This,riid,ppvObject)	\
    ( (This)->lpVtbl -> QueryInterface(This,riid,ppvObject) ) 

#define IVsUserSettingsMigration_AddRef(This)	\
    ( (This)->lpVtbl -> AddRef(This) ) 

#define IVsUserSettingsMigration_Release(This)	\
    ( (This)->lpVtbl -> Release(This) ) 


#define IVsUserSettingsMigration_MigrateSettings(This,pSettingsReader,pSettingsWriter,pszGuidCategory)	\
    ( (This)->lpVtbl -> MigrateSettings(This,pSettingsReader,pSettingsWriter,pszGuidCategory) ) 

#endif /* COBJMACROS */


#endif 	/* C style interface */




#endif 	/* __IVsUserSettingsMigration_INTERFACE_DEFINED__ */


/* interface __MIDL_itf_vsshell90_0000_0007 */
/* [local] */ 

#define szCF_MINIMUMFRAMEWORKVERSION L"MinimumRequiredFrameworkVersion"


extern RPC_IF_HANDLE __MIDL_itf_vsshell90_0000_0007_v0_0_c_ifspec;
extern RPC_IF_HANDLE __MIDL_itf_vsshell90_0000_0007_v0_0_s_ifspec;

#ifndef __IVsShell3_INTERFACE_DEFINED__
#define __IVsShell3_INTERFACE_DEFINED__

/* interface IVsShell3 */
/* [object][unique][version][uuid] */ 


EXTERN_C const IID IID_IVsShell3;

#if defined(__cplusplus) && !defined(CINTERFACE)
    
    MIDL_INTERFACE("07bdc931-e86d-4531-a59c-20c614d6e492")
    IVsShell3 : public IUnknown
    {
    public:
        virtual HRESULT STDMETHODCALLTYPE RestartElevated( void) = 0;
        
        virtual HRESULT STDMETHODCALLTYPE IsRunningElevated( 
            /* [out] */ __RPC__out VARIANT_BOOL *pElevated) = 0;
        
    };
    
#else 	/* C style interface */

    typedef struct IVsShell3Vtbl
    {
        BEGIN_INTERFACE
        
        HRESULT ( STDMETHODCALLTYPE *QueryInterface )( 
            IVsShell3 * This,
            /* [in] */ __RPC__in REFIID riid,
            /* [iid_is][out] */ 
            __RPC__deref_out  void **ppvObject);
        
        ULONG ( STDMETHODCALLTYPE *AddRef )( 
            IVsShell3 * This);
        
        ULONG ( STDMETHODCALLTYPE *Release )( 
            IVsShell3 * This);
        
        HRESULT ( STDMETHODCALLTYPE *RestartElevated )( 
            IVsShell3 * This);
        
        HRESULT ( STDMETHODCALLTYPE *IsRunningElevated )( 
            IVsShell3 * This,
            /* [out] */ __RPC__out VARIANT_BOOL *pElevated);
        
        END_INTERFACE
    } IVsShell3Vtbl;

    interface IVsShell3
    {
        CONST_VTBL struct IVsShell3Vtbl *lpVtbl;
    };

    

#ifdef COBJMACROS


#define IVsShell3_QueryInterface(This,riid,ppvObject)	\
    ( (This)->lpVtbl -> QueryInterface(This,riid,ppvObject) ) 

#define IVsShell3_AddRef(This)	\
    ( (This)->lpVtbl -> AddRef(This) ) 

#define IVsShell3_Release(This)	\
    ( (This)->lpVtbl -> Release(This) ) 


#define IVsShell3_RestartElevated(This)	\
    ( (This)->lpVtbl -> RestartElevated(This) ) 

#define IVsShell3_IsRunningElevated(This,pElevated)	\
    ( (This)->lpVtbl -> IsRunningElevated(This,pElevated) ) 

#endif /* COBJMACROS */


#endif 	/* C style interface */




#endif 	/* __IVsShell3_INTERFACE_DEFINED__ */


#ifndef __IVsUIShell3_INTERFACE_DEFINED__
#define __IVsUIShell3_INTERFACE_DEFINED__

/* interface IVsUIShell3 */
/* [object][unique][version][uuid] */ 


EXTERN_C const IID IID_IVsUIShell3;

#if defined(__cplusplus) && !defined(CINTERFACE)
    
    MIDL_INTERFACE("1c763d26-637c-46f8-a55c-6ecc84df4e4f")
    IVsUIShell3 : public IUnknown
    {
    public:
        virtual HRESULT STDMETHODCALLTYPE ReportErrorInfo2( 
            /* [in] */ HRESULT hr,
            /* [in] */ VARIANT_BOOL Suppress) = 0;
        
        virtual HRESULT STDMETHODCALLTYPE SuppressRestart( 
            /* [in] */ VARIANT_BOOL Suppress) = 0;
        
    };
    
#else 	/* C style interface */

    typedef struct IVsUIShell3Vtbl
    {
        BEGIN_INTERFACE
        
        HRESULT ( STDMETHODCALLTYPE *QueryInterface )( 
            IVsUIShell3 * This,
            /* [in] */ __RPC__in REFIID riid,
            /* [iid_is][out] */ 
            __RPC__deref_out  void **ppvObject);
        
        ULONG ( STDMETHODCALLTYPE *AddRef )( 
            IVsUIShell3 * This);
        
        ULONG ( STDMETHODCALLTYPE *Release )( 
            IVsUIShell3 * This);
        
        HRESULT ( STDMETHODCALLTYPE *ReportErrorInfo2 )( 
            IVsUIShell3 * This,
            /* [in] */ HRESULT hr,
            /* [in] */ VARIANT_BOOL Suppress);
        
        HRESULT ( STDMETHODCALLTYPE *SuppressRestart )( 
            IVsUIShell3 * This,
            /* [in] */ VARIANT_BOOL Suppress);
        
        END_INTERFACE
    } IVsUIShell3Vtbl;

    interface IVsUIShell3
    {
        CONST_VTBL struct IVsUIShell3Vtbl *lpVtbl;
    };

    

#ifdef COBJMACROS


#define IVsUIShell3_QueryInterface(This,riid,ppvObject)	\
    ( (This)->lpVtbl -> QueryInterface(This,riid,ppvObject) ) 

#define IVsUIShell3_AddRef(This)	\
    ( (This)->lpVtbl -> AddRef(This) ) 

#define IVsUIShell3_Release(This)	\
    ( (This)->lpVtbl -> Release(This) ) 


#define IVsUIShell3_ReportErrorInfo2(This,hr,Suppress)	\
    ( (This)->lpVtbl -> ReportErrorInfo2(This,hr,Suppress) ) 

#define IVsUIShell3_SuppressRestart(This,Suppress)	\
    ( (This)->lpVtbl -> SuppressRestart(This,Suppress) ) 

#endif /* COBJMACROS */


#endif 	/* C style interface */




#endif 	/* __IVsUIShell3_INTERFACE_DEFINED__ */


/* interface __MIDL_itf_vsshell90_0000_0009 */
/* [local] */ 


enum __VSDBGLAUNCHFLAGS3
    {	DBGLAUNCH_WaitForEvent	= 0x800
    } ;
typedef DWORD VSDBGLAUNCHFLAGS3;


enum __VSDBGLAUNCHFLAGS4
    {	DBGLAUNCH_UseDefaultBrowser	= 0x1000
    } ;
typedef DWORD VSDBGLAUNCHFLAGS4;


enum __VSCREATEPROJFLAGS3
    {	CPF_SKIP_SOLUTION_ACCESS_CHECK	= 0x400
    } ;
typedef DWORD VSCREATEPROJFLAGS3;


enum __VSCREATESOLUTIONFLAGS3
    {	CSF_SKIP_SOLUTION_ACCESS_CHECK	= 0x40
    } ;
typedef /* [public] */ DWORD VSCREATESOLUTIONFLAGS3;


enum __VSSLNOPENOPTIONS3
    {	SLNOPENOPT_SkipSolutionAccessCheck	= 0x10
    } ;
typedef DWORD VSSLNOPENOPTIONS3;



extern RPC_IF_HANDLE __MIDL_itf_vsshell90_0000_0009_v0_0_c_ifspec;
extern RPC_IF_HANDLE __MIDL_itf_vsshell90_0000_0009_v0_0_s_ifspec;

#ifndef __IVsMSBuildHostObject_INTERFACE_DEFINED__
#define __IVsMSBuildHostObject_INTERFACE_DEFINED__

/* interface IVsMSBuildHostObject */
/* [object][unique][version][uuid] */ 


EXTERN_C const IID IID_IVsMSBuildHostObject;

#if defined(__cplusplus) && !defined(CINTERFACE)
    
    MIDL_INTERFACE("45F31264-BACD-45A9-AE64-036120C52582")
    IVsMSBuildHostObject : public IUnknown
    {
    public:
        virtual HRESULT STDMETHODCALLTYPE Init( 
            /* [in] */ __RPC__in_opt IVsHierarchy *pHier,
            /* [in] */ __RPC__in_opt IServiceProvider *pSP) = 0;
        
        virtual HRESULT STDMETHODCALLTYPE Close( void) = 0;
        
        virtual HRESULT STDMETHODCALLTYPE BeginBuild( void) = 0;
        
        virtual HRESULT STDMETHODCALLTYPE EndBuild( void) = 0;
        
    };
    
#else 	/* C style interface */

    typedef struct IVsMSBuildHostObjectVtbl
    {
        BEGIN_INTERFACE
        
        HRESULT ( STDMETHODCALLTYPE *QueryInterface )( 
            IVsMSBuildHostObject * This,
            /* [in] */ __RPC__in REFIID riid,
            /* [iid_is][out] */ 
            __RPC__deref_out  void **ppvObject);
        
        ULONG ( STDMETHODCALLTYPE *AddRef )( 
            IVsMSBuildHostObject * This);
        
        ULONG ( STDMETHODCALLTYPE *Release )( 
            IVsMSBuildHostObject * This);
        
        HRESULT ( STDMETHODCALLTYPE *Init )( 
            IVsMSBuildHostObject * This,
            /* [in] */ __RPC__in_opt IVsHierarchy *pHier,
            /* [in] */ __RPC__in_opt IServiceProvider *pSP);
        
        HRESULT ( STDMETHODCALLTYPE *Close )( 
            IVsMSBuildHostObject * This);
        
        HRESULT ( STDMETHODCALLTYPE *BeginBuild )( 
            IVsMSBuildHostObject * This);
        
        HRESULT ( STDMETHODCALLTYPE *EndBuild )( 
            IVsMSBuildHostObject * This);
        
        END_INTERFACE
    } IVsMSBuildHostObjectVtbl;

    interface IVsMSBuildHostObject
    {
        CONST_VTBL struct IVsMSBuildHostObjectVtbl *lpVtbl;
    };

    

#ifdef COBJMACROS


#define IVsMSBuildHostObject_QueryInterface(This,riid,ppvObject)	\
    ( (This)->lpVtbl -> QueryInterface(This,riid,ppvObject) ) 

#define IVsMSBuildHostObject_AddRef(This)	\
    ( (This)->lpVtbl -> AddRef(This) ) 

#define IVsMSBuildHostObject_Release(This)	\
    ( (This)->lpVtbl -> Release(This) ) 


#define IVsMSBuildHostObject_Init(This,pHier,pSP)	\
    ( (This)->lpVtbl -> Init(This,pHier,pSP) ) 

#define IVsMSBuildHostObject_Close(This)	\
    ( (This)->lpVtbl -> Close(This) ) 

#define IVsMSBuildHostObject_BeginBuild(This)	\
    ( (This)->lpVtbl -> BeginBuild(This) ) 

#define IVsMSBuildHostObject_EndBuild(This)	\
    ( (This)->lpVtbl -> EndBuild(This) ) 

#endif /* COBJMACROS */


#endif 	/* C style interface */




#endif 	/* __IVsMSBuildHostObject_INTERFACE_DEFINED__ */


#ifndef __IVsMSBuildTaskFileManager_INTERFACE_DEFINED__
#define __IVsMSBuildTaskFileManager_INTERFACE_DEFINED__

/* interface IVsMSBuildTaskFileManager */
/* [object][unique][version][uuid] */ 


EXTERN_C const IID IID_IVsMSBuildTaskFileManager;

#if defined(__cplusplus) && !defined(CINTERFACE)
    
    MIDL_INTERFACE("33372170-A08F-47F9-B1AE-CD9F2C3BB7C9")
    IVsMSBuildTaskFileManager : public IUnknown
    {
    public:
        virtual HRESULT STDMETHODCALLTYPE GetFileContents( 
            /* [in] */ __RPC__in LPCOLESTR wszFilename,
            /* [retval][out] */ __RPC__deref_out_opt BSTR *pbstrFileContents) = 0;
        
        virtual HRESULT STDMETHODCALLTYPE GetFileDocData( 
            /* [in] */ __RPC__in LPCOLESTR wszFilename,
            /* [retval][out] */ __RPC__deref_out_opt IUnknown **ppunkDocData) = 0;
        
        virtual HRESULT STDMETHODCALLTYPE GetFileLastChangeTime( 
            /* [in] */ __RPC__in LPCOLESTR wszFilename,
            /* [retval][out] */ __RPC__out FILETIME *pFileTime) = 0;
        
        virtual HRESULT STDMETHODCALLTYPE PutGeneratedFileContents( 
            /* [in] */ __RPC__in LPCOLESTR wszFilename,
            /* [in] */ __RPC__in LPCOLESTR wszFileContents) = 0;
        
        virtual HRESULT STDMETHODCALLTYPE IsRealBuildOperation( 
            /* [retval][out] */ __RPC__out BOOL *pfIsRealBuild) = 0;
        
        virtual HRESULT STDMETHODCALLTYPE Delete( 
            /* [in] */ __RPC__in LPCOLESTR wszFilename) = 0;
        
        virtual HRESULT STDMETHODCALLTYPE Exists( 
            /* [in] */ __RPC__in LPCOLESTR wszFilename,
            /* [in] */ BOOL fOnlyCheckOnDisk,
            /* [retval][out] */ __RPC__out BOOL *pfExists) = 0;
        
    };
    
#else 	/* C style interface */

    typedef struct IVsMSBuildTaskFileManagerVtbl
    {
        BEGIN_INTERFACE
        
        HRESULT ( STDMETHODCALLTYPE *QueryInterface )( 
            IVsMSBuildTaskFileManager * This,
            /* [in] */ __RPC__in REFIID riid,
            /* [iid_is][out] */ 
            __RPC__deref_out  void **ppvObject);
        
        ULONG ( STDMETHODCALLTYPE *AddRef )( 
            IVsMSBuildTaskFileManager * This);
        
        ULONG ( STDMETHODCALLTYPE *Release )( 
            IVsMSBuildTaskFileManager * This);
        
        HRESULT ( STDMETHODCALLTYPE *GetFileContents )( 
            IVsMSBuildTaskFileManager * This,
            /* [in] */ __RPC__in LPCOLESTR wszFilename,
            /* [retval][out] */ __RPC__deref_out_opt BSTR *pbstrFileContents);
        
        HRESULT ( STDMETHODCALLTYPE *GetFileDocData )( 
            IVsMSBuildTaskFileManager * This,
            /* [in] */ __RPC__in LPCOLESTR wszFilename,
            /* [retval][out] */ __RPC__deref_out_opt IUnknown **ppunkDocData);
        
        HRESULT ( STDMETHODCALLTYPE *GetFileLastChangeTime )( 
            IVsMSBuildTaskFileManager * This,
            /* [in] */ __RPC__in LPCOLESTR wszFilename,
            /* [retval][out] */ __RPC__out FILETIME *pFileTime);
        
        HRESULT ( STDMETHODCALLTYPE *PutGeneratedFileContents )( 
            IVsMSBuildTaskFileManager * This,
            /* [in] */ __RPC__in LPCOLESTR wszFilename,
            /* [in] */ __RPC__in LPCOLESTR wszFileContents);
        
        HRESULT ( STDMETHODCALLTYPE *IsRealBuildOperation )( 
            IVsMSBuildTaskFileManager * This,
            /* [retval][out] */ __RPC__out BOOL *pfIsRealBuild);
        
        HRESULT ( STDMETHODCALLTYPE *Delete )( 
            IVsMSBuildTaskFileManager * This,
            /* [in] */ __RPC__in LPCOLESTR wszFilename);
        
        HRESULT ( STDMETHODCALLTYPE *Exists )( 
            IVsMSBuildTaskFileManager * This,
            /* [in] */ __RPC__in LPCOLESTR wszFilename,
            /* [in] */ BOOL fOnlyCheckOnDisk,
            /* [retval][out] */ __RPC__out BOOL *pfExists);
        
        END_INTERFACE
    } IVsMSBuildTaskFileManagerVtbl;

    interface IVsMSBuildTaskFileManager
    {
        CONST_VTBL struct IVsMSBuildTaskFileManagerVtbl *lpVtbl;
    };

    

#ifdef COBJMACROS


#define IVsMSBuildTaskFileManager_QueryInterface(This,riid,ppvObject)	\
    ( (This)->lpVtbl -> QueryInterface(This,riid,ppvObject) ) 

#define IVsMSBuildTaskFileManager_AddRef(This)	\
    ( (This)->lpVtbl -> AddRef(This) ) 

#define IVsMSBuildTaskFileManager_Release(This)	\
    ( (This)->lpVtbl -> Release(This) ) 


#define IVsMSBuildTaskFileManager_GetFileContents(This,wszFilename,pbstrFileContents)	\
    ( (This)->lpVtbl -> GetFileContents(This,wszFilename,pbstrFileContents) ) 

#define IVsMSBuildTaskFileManager_GetFileDocData(This,wszFilename,ppunkDocData)	\
    ( (This)->lpVtbl -> GetFileDocData(This,wszFilename,ppunkDocData) ) 

#define IVsMSBuildTaskFileManager_GetFileLastChangeTime(This,wszFilename,pFileTime)	\
    ( (This)->lpVtbl -> GetFileLastChangeTime(This,wszFilename,pFileTime) ) 

#define IVsMSBuildTaskFileManager_PutGeneratedFileContents(This,wszFilename,wszFileContents)	\
    ( (This)->lpVtbl -> PutGeneratedFileContents(This,wszFilename,wszFileContents) ) 

#define IVsMSBuildTaskFileManager_IsRealBuildOperation(This,pfIsRealBuild)	\
    ( (This)->lpVtbl -> IsRealBuildOperation(This,pfIsRealBuild) ) 

#define IVsMSBuildTaskFileManager_Delete(This,wszFilename)	\
    ( (This)->lpVtbl -> Delete(This,wszFilename) ) 

#define IVsMSBuildTaskFileManager_Exists(This,wszFilename,fOnlyCheckOnDisk,pfExists)	\
    ( (This)->lpVtbl -> Exists(This,wszFilename,fOnlyCheckOnDisk,pfExists) ) 

#endif /* COBJMACROS */


#endif 	/* C style interface */




#endif 	/* __IVsMSBuildTaskFileManager_INTERFACE_DEFINED__ */


#ifndef __IVsUpgradeBuildPropertyStorage_INTERFACE_DEFINED__
#define __IVsUpgradeBuildPropertyStorage_INTERFACE_DEFINED__

/* interface IVsUpgradeBuildPropertyStorage */
/* [object][unique][version][uuid] */ 


EXTERN_C const IID IID_IVsUpgradeBuildPropertyStorage;

#if defined(__cplusplus) && !defined(CINTERFACE)
    
    MIDL_INTERFACE("27712a1d-abea-42e2-95a5-31d370759429")
    IVsUpgradeBuildPropertyStorage : public IUnknown
    {
    public:
        virtual HRESULT STDMETHODCALLTYPE GetPropertyValue( 
            /* [in] */ __RPC__in LPCOLESTR pszPropName,
            /* [in] */ __RPC__in LPCOLESTR pszConfigName,
            /* [in] */ PersistStorageType storage,
            /* [retval][out] */ __RPC__deref_out_opt BSTR *pbstrPropValue) = 0;
        
        virtual HRESULT STDMETHODCALLTYPE SetPropertyValue( 
            /* [in] */ __RPC__in LPCOLESTR pszPropName,
            /* [in] */ __RPC__in LPCOLESTR pszConfigName,
            /* [in] */ PersistStorageType storage,
            /* [in] */ __RPC__in LPCOLESTR pszPropValue) = 0;
        
        virtual HRESULT STDMETHODCALLTYPE RemoveProperty( 
            /* [in] */ __RPC__in LPCOLESTR pszPropName,
            /* [in] */ __RPC__in LPCOLESTR pszConfigName,
            /* [in] */ PersistStorageType storage) = 0;
        
        virtual HRESULT STDMETHODCALLTYPE AddNewImport( 
            /* [in] */ __RPC__in LPCOLESTR pszImportPath,
            /* [in] */ __RPC__in LPCOLESTR pszImportCondition) = 0;
        
        virtual HRESULT STDMETHODCALLTYPE RemoveImport( 
            /* [in] */ __RPC__in LPCOLESTR pszImportPath,
            /* [in] */ __RPC__in LPCOLESTR pszImportCondition) = 0;
        
        virtual HRESULT STDMETHODCALLTYPE GetImports( 
            /* [out] */ __RPC__deref_out_opt SAFEARRAY * *prgImportPaths,
            /* [out] */ __RPC__deref_out_opt SAFEARRAY * *prgImportConditions) = 0;
        
        virtual HRESULT STDMETHODCALLTYPE ReplaceImport( 
            /* [in] */ __RPC__in LPCOLESTR pszOldImportPath,
            /* [in] */ __RPC__in LPCOLESTR pszOldCondition,
            /* [in] */ __RPC__in LPCOLESTR pszNewImportPath,
            /* [in] */ __RPC__in LPCOLESTR pszNewCondition) = 0;
        
    };
    
#else 	/* C style interface */

    typedef struct IVsUpgradeBuildPropertyStorageVtbl
    {
        BEGIN_INTERFACE
        
        HRESULT ( STDMETHODCALLTYPE *QueryInterface )( 
            IVsUpgradeBuildPropertyStorage * This,
            /* [in] */ __RPC__in REFIID riid,
            /* [iid_is][out] */ 
            __RPC__deref_out  void **ppvObject);
        
        ULONG ( STDMETHODCALLTYPE *AddRef )( 
            IVsUpgradeBuildPropertyStorage * This);
        
        ULONG ( STDMETHODCALLTYPE *Release )( 
            IVsUpgradeBuildPropertyStorage * This);
        
        HRESULT ( STDMETHODCALLTYPE *GetPropertyValue )( 
            IVsUpgradeBuildPropertyStorage * This,
            /* [in] */ __RPC__in LPCOLESTR pszPropName,
            /* [in] */ __RPC__in LPCOLESTR pszConfigName,
            /* [in] */ PersistStorageType storage,
            /* [retval][out] */ __RPC__deref_out_opt BSTR *pbstrPropValue);
        
        HRESULT ( STDMETHODCALLTYPE *SetPropertyValue )( 
            IVsUpgradeBuildPropertyStorage * This,
            /* [in] */ __RPC__in LPCOLESTR pszPropName,
            /* [in] */ __RPC__in LPCOLESTR pszConfigName,
            /* [in] */ PersistStorageType storage,
            /* [in] */ __RPC__in LPCOLESTR pszPropValue);
        
        HRESULT ( STDMETHODCALLTYPE *RemoveProperty )( 
            IVsUpgradeBuildPropertyStorage * This,
            /* [in] */ __RPC__in LPCOLESTR pszPropName,
            /* [in] */ __RPC__in LPCOLESTR pszConfigName,
            /* [in] */ PersistStorageType storage);
        
        HRESULT ( STDMETHODCALLTYPE *AddNewImport )( 
            IVsUpgradeBuildPropertyStorage * This,
            /* [in] */ __RPC__in LPCOLESTR pszImportPath,
            /* [in] */ __RPC__in LPCOLESTR pszImportCondition);
        
        HRESULT ( STDMETHODCALLTYPE *RemoveImport )( 
            IVsUpgradeBuildPropertyStorage * This,
            /* [in] */ __RPC__in LPCOLESTR pszImportPath,
            /* [in] */ __RPC__in LPCOLESTR pszImportCondition);
        
        HRESULT ( STDMETHODCALLTYPE *GetImports )( 
            IVsUpgradeBuildPropertyStorage * This,
            /* [out] */ __RPC__deref_out_opt SAFEARRAY * *prgImportPaths,
            /* [out] */ __RPC__deref_out_opt SAFEARRAY * *prgImportConditions);
        
        HRESULT ( STDMETHODCALLTYPE *ReplaceImport )( 
            IVsUpgradeBuildPropertyStorage * This,
            /* [in] */ __RPC__in LPCOLESTR pszOldImportPath,
            /* [in] */ __RPC__in LPCOLESTR pszOldCondition,
            /* [in] */ __RPC__in LPCOLESTR pszNewImportPath,
            /* [in] */ __RPC__in LPCOLESTR pszNewCondition);
        
        END_INTERFACE
    } IVsUpgradeBuildPropertyStorageVtbl;

    interface IVsUpgradeBuildPropertyStorage
    {
        CONST_VTBL struct IVsUpgradeBuildPropertyStorageVtbl *lpVtbl;
    };

    

#ifdef COBJMACROS


#define IVsUpgradeBuildPropertyStorage_QueryInterface(This,riid,ppvObject)	\
    ( (This)->lpVtbl -> QueryInterface(This,riid,ppvObject) ) 

#define IVsUpgradeBuildPropertyStorage_AddRef(This)	\
    ( (This)->lpVtbl -> AddRef(This) ) 

#define IVsUpgradeBuildPropertyStorage_Release(This)	\
    ( (This)->lpVtbl -> Release(This) ) 


#define IVsUpgradeBuildPropertyStorage_GetPropertyValue(This,pszPropName,pszConfigName,storage,pbstrPropValue)	\
    ( (This)->lpVtbl -> GetPropertyValue(This,pszPropName,pszConfigName,storage,pbstrPropValue) ) 

#define IVsUpgradeBuildPropertyStorage_SetPropertyValue(This,pszPropName,pszConfigName,storage,pszPropValue)	\
    ( (This)->lpVtbl -> SetPropertyValue(This,pszPropName,pszConfigName,storage,pszPropValue) ) 

#define IVsUpgradeBuildPropertyStorage_RemoveProperty(This,pszPropName,pszConfigName,storage)	\
    ( (This)->lpVtbl -> RemoveProperty(This,pszPropName,pszConfigName,storage) ) 

#define IVsUpgradeBuildPropertyStorage_AddNewImport(This,pszImportPath,pszImportCondition)	\
    ( (This)->lpVtbl -> AddNewImport(This,pszImportPath,pszImportCondition) ) 

#define IVsUpgradeBuildPropertyStorage_RemoveImport(This,pszImportPath,pszImportCondition)	\
    ( (This)->lpVtbl -> RemoveImport(This,pszImportPath,pszImportCondition) ) 

#define IVsUpgradeBuildPropertyStorage_GetImports(This,prgImportPaths,prgImportConditions)	\
    ( (This)->lpVtbl -> GetImports(This,prgImportPaths,prgImportConditions) ) 

#define IVsUpgradeBuildPropertyStorage_ReplaceImport(This,pszOldImportPath,pszOldCondition,pszNewImportPath,pszNewCondition)	\
    ( (This)->lpVtbl -> ReplaceImport(This,pszOldImportPath,pszOldCondition,pszNewImportPath,pszNewCondition) ) 

#endif /* COBJMACROS */


#endif 	/* C style interface */




#endif 	/* __IVsUpgradeBuildPropertyStorage_INTERFACE_DEFINED__ */


#ifndef __IVsProjectFlavorUpgradeViaFactory_INTERFACE_DEFINED__
#define __IVsProjectFlavorUpgradeViaFactory_INTERFACE_DEFINED__

/* interface IVsProjectFlavorUpgradeViaFactory */
/* [object][unique][version][uuid] */ 


EXTERN_C const IID IID_IVsProjectFlavorUpgradeViaFactory;

#if defined(__cplusplus) && !defined(CINTERFACE)
    
    MIDL_INTERFACE("d717b926-12ee-4285-9123-522ed54c4859")
    IVsProjectFlavorUpgradeViaFactory : public IUnknown
    {
    public:
        virtual HRESULT STDMETHODCALLTYPE UpgradeProjectFlavor( 
            /* [in] */ __RPC__in LPCOLESTR pszFileName,
            /* [in] */ __RPC__in_opt IVsUpgradeBuildPropertyStorage *pUpgradeBuildPropStg,
            /* [in] */ __RPC__in LPCOLESTR pszProjFileXMLFragment,
            /* [in] */ __RPC__in LPCOLESTR pszUserFileXMLFragment,
            /* [in] */ __RPC__in_opt IVsUpgradeLogger *pLogger,
            /* [out] */ __RPC__out BOOL *pUpgradeRequired,
            /* [optional][out] */ __RPC__deref_out_opt BSTR *pbstrUpgradedProjFileXMLFragment,
            /* [optional][out] */ __RPC__deref_out_opt BSTR *pbstrUpgradedUserFileXMLFragment,
            /* [optional][out] */ __RPC__out GUID *pguidNewProjectFactory) = 0;
        
        virtual HRESULT STDMETHODCALLTYPE UpgradeProjectFlavor_CheckOnly( 
            /* [in] */ __RPC__in LPCOLESTR pszFileName,
            /* [in] */ __RPC__in_opt IVsUpgradeBuildPropertyStorage *pUpgradeBuildPropStg,
            /* [in] */ __RPC__in LPCOLESTR pszProjFileXMLFragment,
            /* [in] */ __RPC__in LPCOLESTR pszUserFileXMLFragment,
            /* [in] */ __RPC__in_opt IVsUpgradeLogger *pLogger,
            /* [out] */ __RPC__out BOOL *pUpgradeRequired,
            /* [optional][out] */ __RPC__out GUID *pguidNewProjectFactory) = 0;
        
        virtual HRESULT STDMETHODCALLTYPE OnUpgradeProjectFlavorCancelled( 
            /* [in] */ __RPC__in LPCOLESTR pszFileName) = 0;
        
    };
    
#else 	/* C style interface */

    typedef struct IVsProjectFlavorUpgradeViaFactoryVtbl
    {
        BEGIN_INTERFACE
        
        HRESULT ( STDMETHODCALLTYPE *QueryInterface )( 
            IVsProjectFlavorUpgradeViaFactory * This,
            /* [in] */ __RPC__in REFIID riid,
            /* [iid_is][out] */ 
            __RPC__deref_out  void **ppvObject);
        
        ULONG ( STDMETHODCALLTYPE *AddRef )( 
            IVsProjectFlavorUpgradeViaFactory * This);
        
        ULONG ( STDMETHODCALLTYPE *Release )( 
            IVsProjectFlavorUpgradeViaFactory * This);
        
        HRESULT ( STDMETHODCALLTYPE *UpgradeProjectFlavor )( 
            IVsProjectFlavorUpgradeViaFactory * This,
            /* [in] */ __RPC__in LPCOLESTR pszFileName,
            /* [in] */ __RPC__in_opt IVsUpgradeBuildPropertyStorage *pUpgradeBuildPropStg,
            /* [in] */ __RPC__in LPCOLESTR pszProjFileXMLFragment,
            /* [in] */ __RPC__in LPCOLESTR pszUserFileXMLFragment,
            /* [in] */ __RPC__in_opt IVsUpgradeLogger *pLogger,
            /* [out] */ __RPC__out BOOL *pUpgradeRequired,
            /* [optional][out] */ __RPC__deref_out_opt BSTR *pbstrUpgradedProjFileXMLFragment,
            /* [optional][out] */ __RPC__deref_out_opt BSTR *pbstrUpgradedUserFileXMLFragment,
            /* [optional][out] */ __RPC__out GUID *pguidNewProjectFactory);
        
        HRESULT ( STDMETHODCALLTYPE *UpgradeProjectFlavor_CheckOnly )( 
            IVsProjectFlavorUpgradeViaFactory * This,
            /* [in] */ __RPC__in LPCOLESTR pszFileName,
            /* [in] */ __RPC__in_opt IVsUpgradeBuildPropertyStorage *pUpgradeBuildPropStg,
            /* [in] */ __RPC__in LPCOLESTR pszProjFileXMLFragment,
            /* [in] */ __RPC__in LPCOLESTR pszUserFileXMLFragment,
            /* [in] */ __RPC__in_opt IVsUpgradeLogger *pLogger,
            /* [out] */ __RPC__out BOOL *pUpgradeRequired,
            /* [optional][out] */ __RPC__out GUID *pguidNewProjectFactory);
        
        HRESULT ( STDMETHODCALLTYPE *OnUpgradeProjectFlavorCancelled )( 
            IVsProjectFlavorUpgradeViaFactory * This,
            /* [in] */ __RPC__in LPCOLESTR pszFileName);
        
        END_INTERFACE
    } IVsProjectFlavorUpgradeViaFactoryVtbl;

    interface IVsProjectFlavorUpgradeViaFactory
    {
        CONST_VTBL struct IVsProjectFlavorUpgradeViaFactoryVtbl *lpVtbl;
    };

    

#ifdef COBJMACROS


#define IVsProjectFlavorUpgradeViaFactory_QueryInterface(This,riid,ppvObject)	\
    ( (This)->lpVtbl -> QueryInterface(This,riid,ppvObject) ) 

#define IVsProjectFlavorUpgradeViaFactory_AddRef(This)	\
    ( (This)->lpVtbl -> AddRef(This) ) 

#define IVsProjectFlavorUpgradeViaFactory_Release(This)	\
    ( (This)->lpVtbl -> Release(This) ) 


#define IVsProjectFlavorUpgradeViaFactory_UpgradeProjectFlavor(This,pszFileName,pUpgradeBuildPropStg,pszProjFileXMLFragment,pszUserFileXMLFragment,pLogger,pUpgradeRequired,pbstrUpgradedProjFileXMLFragment,pbstrUpgradedUserFileXMLFragment,pguidNewProjectFactory)	\
    ( (This)->lpVtbl -> UpgradeProjectFlavor(This,pszFileName,pUpgradeBuildPropStg,pszProjFileXMLFragment,pszUserFileXMLFragment,pLogger,pUpgradeRequired,pbstrUpgradedProjFileXMLFragment,pbstrUpgradedUserFileXMLFragment,pguidNewProjectFactory) ) 

#define IVsProjectFlavorUpgradeViaFactory_UpgradeProjectFlavor_CheckOnly(This,pszFileName,pUpgradeBuildPropStg,pszProjFileXMLFragment,pszUserFileXMLFragment,pLogger,pUpgradeRequired,pguidNewProjectFactory)	\
    ( (This)->lpVtbl -> UpgradeProjectFlavor_CheckOnly(This,pszFileName,pUpgradeBuildPropStg,pszProjFileXMLFragment,pszUserFileXMLFragment,pLogger,pUpgradeRequired,pguidNewProjectFactory) ) 

#define IVsProjectFlavorUpgradeViaFactory_OnUpgradeProjectFlavorCancelled(This,pszFileName)	\
    ( (This)->lpVtbl -> OnUpgradeProjectFlavorCancelled(This,pszFileName) ) 

#endif /* COBJMACROS */


#endif 	/* C style interface */




#endif 	/* __IVsProjectFlavorUpgradeViaFactory_INTERFACE_DEFINED__ */


#ifndef __IVsProjectServerHost_INTERFACE_DEFINED__
#define __IVsProjectServerHost_INTERFACE_DEFINED__

/* interface IVsProjectServerHost */
/* [object][unique][helpstring][uuid] */ 


EXTERN_C const IID IID_IVsProjectServerHost;

#if defined(__cplusplus) && !defined(CINTERFACE)
    
    MIDL_INTERFACE("C044F284-CA0B-41f9-A4C0-E2B650234F30")
    IVsProjectServerHost : public IUnknown
    {
    public:
        virtual HRESULT STDMETHODCALLTYPE StartServer( 
            /* [full][in] */ __RPC__in_opt BSTR bstrEnvironment,
            /* [retval][out] */ __RPC__out DWORD *pdwProcessId) = 0;
        
        virtual HRESULT STDMETHODCALLTYPE GetServerUrl( 
            /* [retval][out] */ __RPC__deref_out_opt BSTR *pbstrUrl) = 0;
        
        virtual HRESULT STDMETHODCALLTYPE StopServer( void) = 0;
        
        virtual HRESULT STDMETHODCALLTYPE IsServerRunning( 
            /* [retval][out] */ __RPC__out BOOL *fIsRunning) = 0;
        
    };
    
#else 	/* C style interface */

    typedef struct IVsProjectServerHostVtbl
    {
        BEGIN_INTERFACE
        
        HRESULT ( STDMETHODCALLTYPE *QueryInterface )( 
            IVsProjectServerHost * This,
            /* [in] */ __RPC__in REFIID riid,
            /* [iid_is][out] */ 
            __RPC__deref_out  void **ppvObject);
        
        ULONG ( STDMETHODCALLTYPE *AddRef )( 
            IVsProjectServerHost * This);
        
        ULONG ( STDMETHODCALLTYPE *Release )( 
            IVsProjectServerHost * This);
        
        HRESULT ( STDMETHODCALLTYPE *StartServer )( 
            IVsProjectServerHost * This,
            /* [full][in] */ __RPC__in_opt BSTR bstrEnvironment,
            /* [retval][out] */ __RPC__out DWORD *pdwProcessId);
        
        HRESULT ( STDMETHODCALLTYPE *GetServerUrl )( 
            IVsProjectServerHost * This,
            /* [retval][out] */ __RPC__deref_out_opt BSTR *pbstrUrl);
        
        HRESULT ( STDMETHODCALLTYPE *StopServer )( 
            IVsProjectServerHost * This);
        
        HRESULT ( STDMETHODCALLTYPE *IsServerRunning )( 
            IVsProjectServerHost * This,
            /* [retval][out] */ __RPC__out BOOL *fIsRunning);
        
        END_INTERFACE
    } IVsProjectServerHostVtbl;

    interface IVsProjectServerHost
    {
        CONST_VTBL struct IVsProjectServerHostVtbl *lpVtbl;
    };

    

#ifdef COBJMACROS


#define IVsProjectServerHost_QueryInterface(This,riid,ppvObject)	\
    ( (This)->lpVtbl -> QueryInterface(This,riid,ppvObject) ) 

#define IVsProjectServerHost_AddRef(This)	\
    ( (This)->lpVtbl -> AddRef(This) ) 

#define IVsProjectServerHost_Release(This)	\
    ( (This)->lpVtbl -> Release(This) ) 


#define IVsProjectServerHost_StartServer(This,bstrEnvironment,pdwProcessId)	\
    ( (This)->lpVtbl -> StartServer(This,bstrEnvironment,pdwProcessId) ) 

#define IVsProjectServerHost_GetServerUrl(This,pbstrUrl)	\
    ( (This)->lpVtbl -> GetServerUrl(This,pbstrUrl) ) 

#define IVsProjectServerHost_StopServer(This)	\
    ( (This)->lpVtbl -> StopServer(This) ) 

#define IVsProjectServerHost_IsServerRunning(This,fIsRunning)	\
    ( (This)->lpVtbl -> IsServerRunning(This,fIsRunning) ) 

#endif /* COBJMACROS */


#endif 	/* C style interface */




#endif 	/* __IVsProjectServerHost_INTERFACE_DEFINED__ */


#ifndef __IVsFileUpgrade2_INTERFACE_DEFINED__
#define __IVsFileUpgrade2_INTERFACE_DEFINED__

/* interface IVsFileUpgrade2 */
/* [object][unique][version][uuid] */ 


EXTERN_C const IID IID_IVsFileUpgrade2;

#if defined(__cplusplus) && !defined(CINTERFACE)
    
    MIDL_INTERFACE("F2394417-A219-4297-BB4E-E66864A6A7DB")
    IVsFileUpgrade2 : public IUnknown
    {
    public:
        virtual HRESULT STDMETHODCALLTYPE UpgradeFile( 
            /* [in] */ __RPC__in BSTR bstrProjectName,
            /* [in] */ __RPC__in BSTR bstrFileName,
            /* [in] */ BOOL bNoBackup,
            /* [in] */ __RPC__in_opt IVsUpgradeLogger *pLogger,
            /* [in] */ USHORT oldToolsVersionMajor,
            /* [in] */ USHORT oldToolsVersionMinor,
            /* [in] */ USHORT newToolsVersionMajor,
            /* [in] */ USHORT newToolsVersionMinor,
            /* [out] */ __RPC__out BOOL *pUpgradeRequired) = 0;
        
        virtual HRESULT STDMETHODCALLTYPE UpgradeFile_CheckOnly( 
            /* [in] */ __RPC__in BSTR bstrProjectName,
            /* [in] */ __RPC__in BSTR bstrFileName,
            /* [in] */ BOOL bNoBackup,
            /* [in] */ __RPC__in_opt IVsUpgradeLogger *pLogger,
            /* [in] */ USHORT oldToolsVersionMajor,
            /* [in] */ USHORT oldToolsVersionMinor,
            /* [in] */ USHORT newToolsVersionMajor,
            /* [in] */ USHORT newToolsVersionMinor,
            /* [out] */ __RPC__out BOOL *pUpgradeRequired) = 0;
        
    };
    
#else 	/* C style interface */

    typedef struct IVsFileUpgrade2Vtbl
    {
        BEGIN_INTERFACE
        
        HRESULT ( STDMETHODCALLTYPE *QueryInterface )( 
            IVsFileUpgrade2 * This,
            /* [in] */ __RPC__in REFIID riid,
            /* [iid_is][out] */ 
            __RPC__deref_out  void **ppvObject);
        
        ULONG ( STDMETHODCALLTYPE *AddRef )( 
            IVsFileUpgrade2 * This);
        
        ULONG ( STDMETHODCALLTYPE *Release )( 
            IVsFileUpgrade2 * This);
        
        HRESULT ( STDMETHODCALLTYPE *UpgradeFile )( 
            IVsFileUpgrade2 * This,
            /* [in] */ __RPC__in BSTR bstrProjectName,
            /* [in] */ __RPC__in BSTR bstrFileName,
            /* [in] */ BOOL bNoBackup,
            /* [in] */ __RPC__in_opt IVsUpgradeLogger *pLogger,
            /* [in] */ USHORT oldToolsVersionMajor,
            /* [in] */ USHORT oldToolsVersionMinor,
            /* [in] */ USHORT newToolsVersionMajor,
            /* [in] */ USHORT newToolsVersionMinor,
            /* [out] */ __RPC__out BOOL *pUpgradeRequired);
        
        HRESULT ( STDMETHODCALLTYPE *UpgradeFile_CheckOnly )( 
            IVsFileUpgrade2 * This,
            /* [in] */ __RPC__in BSTR bstrProjectName,
            /* [in] */ __RPC__in BSTR bstrFileName,
            /* [in] */ BOOL bNoBackup,
            /* [in] */ __RPC__in_opt IVsUpgradeLogger *pLogger,
            /* [in] */ USHORT oldToolsVersionMajor,
            /* [in] */ USHORT oldToolsVersionMinor,
            /* [in] */ USHORT newToolsVersionMajor,
            /* [in] */ USHORT newToolsVersionMinor,
            /* [out] */ __RPC__out BOOL *pUpgradeRequired);
        
        END_INTERFACE
    } IVsFileUpgrade2Vtbl;

    interface IVsFileUpgrade2
    {
        CONST_VTBL struct IVsFileUpgrade2Vtbl *lpVtbl;
    };

    

#ifdef COBJMACROS


#define IVsFileUpgrade2_QueryInterface(This,riid,ppvObject)	\
    ( (This)->lpVtbl -> QueryInterface(This,riid,ppvObject) ) 

#define IVsFileUpgrade2_AddRef(This)	\
    ( (This)->lpVtbl -> AddRef(This) ) 

#define IVsFileUpgrade2_Release(This)	\
    ( (This)->lpVtbl -> Release(This) ) 


#define IVsFileUpgrade2_UpgradeFile(This,bstrProjectName,bstrFileName,bNoBackup,pLogger,oldToolsVersionMajor,oldToolsVersionMinor,newToolsVersionMajor,newToolsVersionMinor,pUpgradeRequired)	\
    ( (This)->lpVtbl -> UpgradeFile(This,bstrProjectName,bstrFileName,bNoBackup,pLogger,oldToolsVersionMajor,oldToolsVersionMinor,newToolsVersionMajor,newToolsVersionMinor,pUpgradeRequired) ) 

#define IVsFileUpgrade2_UpgradeFile_CheckOnly(This,bstrProjectName,bstrFileName,bNoBackup,pLogger,oldToolsVersionMajor,oldToolsVersionMinor,newToolsVersionMajor,newToolsVersionMinor,pUpgradeRequired)	\
    ( (This)->lpVtbl -> UpgradeFile_CheckOnly(This,bstrProjectName,bstrFileName,bNoBackup,pLogger,oldToolsVersionMajor,oldToolsVersionMinor,newToolsVersionMajor,newToolsVersionMinor,pUpgradeRequired) ) 

#endif /* COBJMACROS */


#endif 	/* C style interface */




#endif 	/* __IVsFileUpgrade2_INTERFACE_DEFINED__ */


#ifndef __IVsSymbolicNavigationNotify_INTERFACE_DEFINED__
#define __IVsSymbolicNavigationNotify_INTERFACE_DEFINED__

/* interface IVsSymbolicNavigationNotify */
/* [object][unique][version][uuid] */ 


EXTERN_C const IID IID_IVsSymbolicNavigationNotify;

#if defined(__cplusplus) && !defined(CINTERFACE)
    
    MIDL_INTERFACE("4DE38661-BB8F-4b9b-8D2F-425949341BBE")
    IVsSymbolicNavigationNotify : public IUnknown
    {
    public:
        virtual HRESULT STDMETHODCALLTYPE OnBeforeNavigateToSymbol( 
            /* [in] */ __RPC__in_opt IVsHierarchy *pHierCodeFile,
            /* [in] */ VSITEMID itemidCodeFile,
            /* [in] */ __RPC__in LPCOLESTR pszRQName,
            /* [retval][out] */ __RPC__out BOOL *pfNavigationHandled) = 0;
        
        virtual HRESULT STDMETHODCALLTYPE QueryNavigateToSymbol( 
            /* [in] */ __RPC__in_opt IVsHierarchy *pHierCodeFile,
            /* [in] */ VSITEMID itemidCodeFile,
            /* [in] */ __RPC__in LPCOLESTR pszRQName,
            /* [out] */ __RPC__deref_out_opt IVsHierarchy **ppHierToNavigate,
            /* [out] */ __RPC__out VSITEMID *pitemidToNavigate,
            /* [out] */ __RPC__out TextSpan *pSpanToNavigate,
            /* [retval][out] */ __RPC__out BOOL *pfWouldNavigate) = 0;
        
    };
    
#else 	/* C style interface */

    typedef struct IVsSymbolicNavigationNotifyVtbl
    {
        BEGIN_INTERFACE
        
        HRESULT ( STDMETHODCALLTYPE *QueryInterface )( 
            IVsSymbolicNavigationNotify * This,
            /* [in] */ __RPC__in REFIID riid,
            /* [iid_is][out] */ 
            __RPC__deref_out  void **ppvObject);
        
        ULONG ( STDMETHODCALLTYPE *AddRef )( 
            IVsSymbolicNavigationNotify * This);
        
        ULONG ( STDMETHODCALLTYPE *Release )( 
            IVsSymbolicNavigationNotify * This);
        
        HRESULT ( STDMETHODCALLTYPE *OnBeforeNavigateToSymbol )( 
            IVsSymbolicNavigationNotify * This,
            /* [in] */ __RPC__in_opt IVsHierarchy *pHierCodeFile,
            /* [in] */ VSITEMID itemidCodeFile,
            /* [in] */ __RPC__in LPCOLESTR pszRQName,
            /* [retval][out] */ __RPC__out BOOL *pfNavigationHandled);
        
        HRESULT ( STDMETHODCALLTYPE *QueryNavigateToSymbol )( 
            IVsSymbolicNavigationNotify * This,
            /* [in] */ __RPC__in_opt IVsHierarchy *pHierCodeFile,
            /* [in] */ VSITEMID itemidCodeFile,
            /* [in] */ __RPC__in LPCOLESTR pszRQName,
            /* [out] */ __RPC__deref_out_opt IVsHierarchy **ppHierToNavigate,
            /* [out] */ __RPC__out VSITEMID *pitemidToNavigate,
            /* [out] */ __RPC__out TextSpan *pSpanToNavigate,
            /* [retval][out] */ __RPC__out BOOL *pfWouldNavigate);
        
        END_INTERFACE
    } IVsSymbolicNavigationNotifyVtbl;

    interface IVsSymbolicNavigationNotify
    {
        CONST_VTBL struct IVsSymbolicNavigationNotifyVtbl *lpVtbl;
    };

    

#ifdef COBJMACROS


#define IVsSymbolicNavigationNotify_QueryInterface(This,riid,ppvObject)	\
    ( (This)->lpVtbl -> QueryInterface(This,riid,ppvObject) ) 

#define IVsSymbolicNavigationNotify_AddRef(This)	\
    ( (This)->lpVtbl -> AddRef(This) ) 

#define IVsSymbolicNavigationNotify_Release(This)	\
    ( (This)->lpVtbl -> Release(This) ) 


#define IVsSymbolicNavigationNotify_OnBeforeNavigateToSymbol(This,pHierCodeFile,itemidCodeFile,pszRQName,pfNavigationHandled)	\
    ( (This)->lpVtbl -> OnBeforeNavigateToSymbol(This,pHierCodeFile,itemidCodeFile,pszRQName,pfNavigationHandled) ) 

#define IVsSymbolicNavigationNotify_QueryNavigateToSymbol(This,pHierCodeFile,itemidCodeFile,pszRQName,ppHierToNavigate,pitemidToNavigate,pSpanToNavigate,pfWouldNavigate)	\
    ( (This)->lpVtbl -> QueryNavigateToSymbol(This,pHierCodeFile,itemidCodeFile,pszRQName,ppHierToNavigate,pitemidToNavigate,pSpanToNavigate,pfWouldNavigate) ) 

#endif /* COBJMACROS */


#endif 	/* C style interface */




#endif 	/* __IVsSymbolicNavigationNotify_INTERFACE_DEFINED__ */


#ifndef __SVsSymbolicNavigationManager_INTERFACE_DEFINED__
#define __SVsSymbolicNavigationManager_INTERFACE_DEFINED__

/* interface SVsSymbolicNavigationManager */
/* [object][uuid] */ 


EXTERN_C const IID IID_SVsSymbolicNavigationManager;

#if defined(__cplusplus) && !defined(CINTERFACE)
    
    MIDL_INTERFACE("C67A5F0C-31C0-4316-9EF9-B451B30C829E")
    SVsSymbolicNavigationManager : public IUnknown
    {
    public:
    };
    
#else 	/* C style interface */

    typedef struct SVsSymbolicNavigationManagerVtbl
    {
        BEGIN_INTERFACE
        
        HRESULT ( STDMETHODCALLTYPE *QueryInterface )( 
            SVsSymbolicNavigationManager * This,
            /* [in] */ __RPC__in REFIID riid,
            /* [iid_is][out] */ 
            __RPC__deref_out  void **ppvObject);
        
        ULONG ( STDMETHODCALLTYPE *AddRef )( 
            SVsSymbolicNavigationManager * This);
        
        ULONG ( STDMETHODCALLTYPE *Release )( 
            SVsSymbolicNavigationManager * This);
        
        END_INTERFACE
    } SVsSymbolicNavigationManagerVtbl;

    interface SVsSymbolicNavigationManager
    {
        CONST_VTBL struct SVsSymbolicNavigationManagerVtbl *lpVtbl;
    };

    

#ifdef COBJMACROS


#define SVsSymbolicNavigationManager_QueryInterface(This,riid,ppvObject)	\
    ( (This)->lpVtbl -> QueryInterface(This,riid,ppvObject) ) 

#define SVsSymbolicNavigationManager_AddRef(This)	\
    ( (This)->lpVtbl -> AddRef(This) ) 

#define SVsSymbolicNavigationManager_Release(This)	\
    ( (This)->lpVtbl -> Release(This) ) 


#endif /* COBJMACROS */


#endif 	/* C style interface */




#endif 	/* __SVsSymbolicNavigationManager_INTERFACE_DEFINED__ */


/* interface __MIDL_itf_vsshell90_0000_0017 */
/* [local] */ 

#define SID_SVsSymbolicNavigationManager IID_SVsSymbolicNavigationManager


extern RPC_IF_HANDLE __MIDL_itf_vsshell90_0000_0017_v0_0_c_ifspec;
extern RPC_IF_HANDLE __MIDL_itf_vsshell90_0000_0017_v0_0_s_ifspec;

#ifndef __IVsSymbolicNavigationManager_INTERFACE_DEFINED__
#define __IVsSymbolicNavigationManager_INTERFACE_DEFINED__

/* interface IVsSymbolicNavigationManager */
/* [object][uuid] */ 


EXTERN_C const IID IID_IVsSymbolicNavigationManager;

#if defined(__cplusplus) && !defined(CINTERFACE)
    
    MIDL_INTERFACE("C4D4F197-941E-43b1-9D42-BE527F9D5D00")
    IVsSymbolicNavigationManager : public IUnknown
    {
    public:
        virtual HRESULT STDMETHODCALLTYPE RegisterSymbolicNavigationNotify( 
            /* [in] */ __RPC__in_opt IVsSymbolicNavigationNotify *pNotify,
            /* [out] */ __RPC__out VSCOOKIE *pdwCookie) = 0;
        
        virtual HRESULT STDMETHODCALLTYPE UnregisterSymbolicNavigationNotify( 
            /* [in] */ VSCOOKIE dwCookie) = 0;
        
        virtual HRESULT STDMETHODCALLTYPE OnBeforeNavigateToSymbol( 
            /* [in] */ __RPC__in_opt IVsHierarchy *pHierCodeFile,
            /* [in] */ VSITEMID itemidCodeFile,
            /* [in] */ __RPC__in LPCOLESTR pszRQName,
            /* [retval][out] */ __RPC__out BOOL *pfNavigationHandled) = 0;
        
        virtual HRESULT STDMETHODCALLTYPE QueryNavigateToSymbol( 
            /* [in] */ __RPC__in_opt IVsHierarchy *pHierCodeFile,
            /* [in] */ VSITEMID itemidCodeFile,
            /* [in] */ __RPC__in LPCOLESTR pszRQName,
            /* [out] */ __RPC__deref_out_opt IVsHierarchy **ppHierToNavigate,
            /* [out] */ __RPC__out VSITEMID *pitemidToNavigate,
            /* [out] */ __RPC__out TextSpan *pSpanToNavigate,
            /* [retval][out] */ __RPC__out BOOL *pfWouldNavigate) = 0;
        
    };
    
#else 	/* C style interface */

    typedef struct IVsSymbolicNavigationManagerVtbl
    {
        BEGIN_INTERFACE
        
        HRESULT ( STDMETHODCALLTYPE *QueryInterface )( 
            IVsSymbolicNavigationManager * This,
            /* [in] */ __RPC__in REFIID riid,
            /* [iid_is][out] */ 
            __RPC__deref_out  void **ppvObject);
        
        ULONG ( STDMETHODCALLTYPE *AddRef )( 
            IVsSymbolicNavigationManager * This);
        
        ULONG ( STDMETHODCALLTYPE *Release )( 
            IVsSymbolicNavigationManager * This);
        
        HRESULT ( STDMETHODCALLTYPE *RegisterSymbolicNavigationNotify )( 
            IVsSymbolicNavigationManager * This,
            /* [in] */ __RPC__in_opt IVsSymbolicNavigationNotify *pNotify,
            /* [out] */ __RPC__out VSCOOKIE *pdwCookie);
        
        HRESULT ( STDMETHODCALLTYPE *UnregisterSymbolicNavigationNotify )( 
            IVsSymbolicNavigationManager * This,
            /* [in] */ VSCOOKIE dwCookie);
        
        HRESULT ( STDMETHODCALLTYPE *OnBeforeNavigateToSymbol )( 
            IVsSymbolicNavigationManager * This,
            /* [in] */ __RPC__in_opt IVsHierarchy *pHierCodeFile,
            /* [in] */ VSITEMID itemidCodeFile,
            /* [in] */ __RPC__in LPCOLESTR pszRQName,
            /* [retval][out] */ __RPC__out BOOL *pfNavigationHandled);
        
        HRESULT ( STDMETHODCALLTYPE *QueryNavigateToSymbol )( 
            IVsSymbolicNavigationManager * This,
            /* [in] */ __RPC__in_opt IVsHierarchy *pHierCodeFile,
            /* [in] */ VSITEMID itemidCodeFile,
            /* [in] */ __RPC__in LPCOLESTR pszRQName,
            /* [out] */ __RPC__deref_out_opt IVsHierarchy **ppHierToNavigate,
            /* [out] */ __RPC__out VSITEMID *pitemidToNavigate,
            /* [out] */ __RPC__out TextSpan *pSpanToNavigate,
            /* [retval][out] */ __RPC__out BOOL *pfWouldNavigate);
        
        END_INTERFACE
    } IVsSymbolicNavigationManagerVtbl;

    interface IVsSymbolicNavigationManager
    {
        CONST_VTBL struct IVsSymbolicNavigationManagerVtbl *lpVtbl;
    };

    

#ifdef COBJMACROS


#define IVsSymbolicNavigationManager_QueryInterface(This,riid,ppvObject)	\
    ( (This)->lpVtbl -> QueryInterface(This,riid,ppvObject) ) 

#define IVsSymbolicNavigationManager_AddRef(This)	\
    ( (This)->lpVtbl -> AddRef(This) ) 

#define IVsSymbolicNavigationManager_Release(This)	\
    ( (This)->lpVtbl -> Release(This) ) 


#define IVsSymbolicNavigationManager_RegisterSymbolicNavigationNotify(This,pNotify,pdwCookie)	\
    ( (This)->lpVtbl -> RegisterSymbolicNavigationNotify(This,pNotify,pdwCookie) ) 

#define IVsSymbolicNavigationManager_UnregisterSymbolicNavigationNotify(This,dwCookie)	\
    ( (This)->lpVtbl -> UnregisterSymbolicNavigationNotify(This,dwCookie) ) 

#define IVsSymbolicNavigationManager_OnBeforeNavigateToSymbol(This,pHierCodeFile,itemidCodeFile,pszRQName,pfNavigationHandled)	\
    ( (This)->lpVtbl -> OnBeforeNavigateToSymbol(This,pHierCodeFile,itemidCodeFile,pszRQName,pfNavigationHandled) ) 

#define IVsSymbolicNavigationManager_QueryNavigateToSymbol(This,pHierCodeFile,itemidCodeFile,pszRQName,ppHierToNavigate,pitemidToNavigate,pSpanToNavigate,pfWouldNavigate)	\
    ( (This)->lpVtbl -> QueryNavigateToSymbol(This,pHierCodeFile,itemidCodeFile,pszRQName,ppHierToNavigate,pitemidToNavigate,pSpanToNavigate,pfWouldNavigate) ) 

#endif /* COBJMACROS */


#endif 	/* C style interface */




#endif 	/* __IVsSymbolicNavigationManager_INTERFACE_DEFINED__ */


#ifndef __IVsOutputWindowPane2_INTERFACE_DEFINED__
#define __IVsOutputWindowPane2_INTERFACE_DEFINED__

/* interface IVsOutputWindowPane2 */
/* [object][unique][version][uuid] */ 


EXTERN_C const IID IID_IVsOutputWindowPane2;

#if defined(__cplusplus) && !defined(CINTERFACE)
    
    MIDL_INTERFACE("4A65481B-49CF-4fcb-A891-32AE435EC941")
    IVsOutputWindowPane2 : public IUnknown
    {
    public:
        virtual HRESULT STDMETHODCALLTYPE OutputTaskItemStringEx2( 
            /* [in] */ __RPC__in LPCOLESTR pszOutputString,
            /* [in] */ VSTASKPRIORITY nPriority,
            /* [in] */ VSTASKCATEGORY nCategory,
            /* [in] */ __RPC__in LPCOLESTR pszSubcategory,
            /* [in] */ VSTASKBITMAP nBitmap,
            /* [in] */ __RPC__in LPCOLESTR pszFilename,
            /* [in] */ ULONG nLineNum,
            /* [in] */ ULONG nColumn,
            /* [in] */ __RPC__in LPCOLESTR pszProjectUniqueName,
            /* [in] */ __RPC__in LPCOLESTR pszTaskItemText,
            /* [in] */ __RPC__in LPCOLESTR pszLookupKwd) = 0;
        
    };
    
#else 	/* C style interface */

    typedef struct IVsOutputWindowPane2Vtbl
    {
        BEGIN_INTERFACE
        
        HRESULT ( STDMETHODCALLTYPE *QueryInterface )( 
            IVsOutputWindowPane2 * This,
            /* [in] */ __RPC__in REFIID riid,
            /* [iid_is][out] */ 
            __RPC__deref_out  void **ppvObject);
        
        ULONG ( STDMETHODCALLTYPE *AddRef )( 
            IVsOutputWindowPane2 * This);
        
        ULONG ( STDMETHODCALLTYPE *Release )( 
            IVsOutputWindowPane2 * This);
        
        HRESULT ( STDMETHODCALLTYPE *OutputTaskItemStringEx2 )( 
            IVsOutputWindowPane2 * This,
            /* [in] */ __RPC__in LPCOLESTR pszOutputString,
            /* [in] */ VSTASKPRIORITY nPriority,
            /* [in] */ VSTASKCATEGORY nCategory,
            /* [in] */ __RPC__in LPCOLESTR pszSubcategory,
            /* [in] */ VSTASKBITMAP nBitmap,
            /* [in] */ __RPC__in LPCOLESTR pszFilename,
            /* [in] */ ULONG nLineNum,
            /* [in] */ ULONG nColumn,
            /* [in] */ __RPC__in LPCOLESTR pszProjectUniqueName,
            /* [in] */ __RPC__in LPCOLESTR pszTaskItemText,
            /* [in] */ __RPC__in LPCOLESTR pszLookupKwd);
        
        END_INTERFACE
    } IVsOutputWindowPane2Vtbl;

    interface IVsOutputWindowPane2
    {
        CONST_VTBL struct IVsOutputWindowPane2Vtbl *lpVtbl;
    };

    

#ifdef COBJMACROS


#define IVsOutputWindowPane2_QueryInterface(This,riid,ppvObject)	\
    ( (This)->lpVtbl -> QueryInterface(This,riid,ppvObject) ) 

#define IVsOutputWindowPane2_AddRef(This)	\
    ( (This)->lpVtbl -> AddRef(This) ) 

#define IVsOutputWindowPane2_Release(This)	\
    ( (This)->lpVtbl -> Release(This) ) 


#define IVsOutputWindowPane2_OutputTaskItemStringEx2(This,pszOutputString,nPriority,nCategory,pszSubcategory,nBitmap,pszFilename,nLineNum,nColumn,pszProjectUniqueName,pszTaskItemText,pszLookupKwd)	\
    ( (This)->lpVtbl -> OutputTaskItemStringEx2(This,pszOutputString,nPriority,nCategory,pszSubcategory,nBitmap,pszFilename,nLineNum,nColumn,pszProjectUniqueName,pszTaskItemText,pszLookupKwd) ) 

#endif /* COBJMACROS */


#endif 	/* C style interface */




#endif 	/* __IVsOutputWindowPane2_INTERFACE_DEFINED__ */


/* interface __MIDL_itf_vsshell90_0000_0019 */
/* [local] */ 


enum __VsLocalRegistryType
    {	RegType_UserSettings	= 1,
	RegType_Configuration	= 2,
	RegType_PrivateConfig	= 3,
	RegType_SessionSettings	= 4,
	RegType_NewUserSettings	= 5
    } ;
typedef DWORD VSLOCALREGISTRYTYPE;


enum __VsLocalRegistryRootHandle
    {	RegHandle_Invalid	= 0,
	RegHandle_CurrentUser	= 0x80000001,
	RegHandle_LocalMachine	= 0x80000002
    } ;
typedef DWORD VSLOCALREGISTRYROOTHANDLE;

#define VSLOCALREGISTRYROOTHANDLE_TO_HKEY(h) (( HKEY ) (ULONG_PTR)((LONG)(h)) )


extern RPC_IF_HANDLE __MIDL_itf_vsshell90_0000_0019_v0_0_c_ifspec;
extern RPC_IF_HANDLE __MIDL_itf_vsshell90_0000_0019_v0_0_s_ifspec;

#ifndef __ILocalRegistry4_INTERFACE_DEFINED__
#define __ILocalRegistry4_INTERFACE_DEFINED__

/* interface ILocalRegistry4 */
/* [object][unique][version][uuid] */ 


EXTERN_C const IID IID_ILocalRegistry4;

#if defined(__cplusplus) && !defined(CINTERFACE)
    
    MIDL_INTERFACE("5C45B909-E820-4acc-B894-0A013C6DA212")
    ILocalRegistry4 : public IUnknown
    {
    public:
        virtual /* [local] */ HRESULT STDMETHODCALLTYPE RegisterClassObject( 
            /* [in] */ REFCLSID rclsid,
            /* [out] */ DWORD *pdwCookie) = 0;
        
        virtual /* [local] */ HRESULT STDMETHODCALLTYPE RevokeClassObject( 
            DWORD dwCookie) = 0;
        
        virtual /* [local] */ HRESULT STDMETHODCALLTYPE RegisterInterface( 
            /* [in] */ REFIID riid) = 0;
        
        virtual HRESULT STDMETHODCALLTYPE GetLocalRegistryRootEx( 
            /* [in] */ VSLOCALREGISTRYTYPE dwRegType,
            /* [out] */ __RPC__out VSLOCALREGISTRYROOTHANDLE *pdwRegRootHandle,
            /* [out] */ __RPC__deref_out_opt BSTR *pbstrRoot) = 0;
        
    };
    
#else 	/* C style interface */

    typedef struct ILocalRegistry4Vtbl
    {
        BEGIN_INTERFACE
        
        HRESULT ( STDMETHODCALLTYPE *QueryInterface )( 
            ILocalRegistry4 * This,
            /* [in] */ __RPC__in REFIID riid,
            /* [iid_is][out] */ 
            __RPC__deref_out  void **ppvObject);
        
        ULONG ( STDMETHODCALLTYPE *AddRef )( 
            ILocalRegistry4 * This);
        
        ULONG ( STDMETHODCALLTYPE *Release )( 
            ILocalRegistry4 * This);
        
        /* [local] */ HRESULT ( STDMETHODCALLTYPE *RegisterClassObject )( 
            ILocalRegistry4 * This,
            /* [in] */ REFCLSID rclsid,
            /* [out] */ DWORD *pdwCookie);
        
        /* [local] */ HRESULT ( STDMETHODCALLTYPE *RevokeClassObject )( 
            ILocalRegistry4 * This,
            DWORD dwCookie);
        
        /* [local] */ HRESULT ( STDMETHODCALLTYPE *RegisterInterface )( 
            ILocalRegistry4 * This,
            /* [in] */ REFIID riid);
        
        HRESULT ( STDMETHODCALLTYPE *GetLocalRegistryRootEx )( 
            ILocalRegistry4 * This,
            /* [in] */ VSLOCALREGISTRYTYPE dwRegType,
            /* [out] */ __RPC__out VSLOCALREGISTRYROOTHANDLE *pdwRegRootHandle,
            /* [out] */ __RPC__deref_out_opt BSTR *pbstrRoot);
        
        END_INTERFACE
    } ILocalRegistry4Vtbl;

    interface ILocalRegistry4
    {
        CONST_VTBL struct ILocalRegistry4Vtbl *lpVtbl;
    };

    

#ifdef COBJMACROS


#define ILocalRegistry4_QueryInterface(This,riid,ppvObject)	\
    ( (This)->lpVtbl -> QueryInterface(This,riid,ppvObject) ) 

#define ILocalRegistry4_AddRef(This)	\
    ( (This)->lpVtbl -> AddRef(This) ) 

#define ILocalRegistry4_Release(This)	\
    ( (This)->lpVtbl -> Release(This) ) 


#define ILocalRegistry4_RegisterClassObject(This,rclsid,pdwCookie)	\
    ( (This)->lpVtbl -> RegisterClassObject(This,rclsid,pdwCookie) ) 

#define ILocalRegistry4_RevokeClassObject(This,dwCookie)	\
    ( (This)->lpVtbl -> RevokeClassObject(This,dwCookie) ) 

#define ILocalRegistry4_RegisterInterface(This,riid)	\
    ( (This)->lpVtbl -> RegisterInterface(This,riid) ) 

#define ILocalRegistry4_GetLocalRegistryRootEx(This,dwRegType,pdwRegRootHandle,pbstrRoot)	\
    ( (This)->lpVtbl -> GetLocalRegistryRootEx(This,dwRegType,pdwRegRootHandle,pbstrRoot) ) 

#endif /* COBJMACROS */


#endif 	/* C style interface */




#endif 	/* __ILocalRegistry4_INTERFACE_DEFINED__ */


#ifndef __IVsEditorFactory2_INTERFACE_DEFINED__
#define __IVsEditorFactory2_INTERFACE_DEFINED__

/* interface IVsEditorFactory2 */
/* [object][unique][version][uuid] */ 


EXTERN_C const IID IID_IVsEditorFactory2;

#if defined(__cplusplus) && !defined(CINTERFACE)
    
    MIDL_INTERFACE("8abd2347-3360-46ae-96fa-3b70a44d3f73")
    IVsEditorFactory2 : public IUnknown
    {
    public:
        virtual HRESULT STDMETHODCALLTYPE RetargetCodeOrDesignerToOpen( 
            /* [in] */ __RPC__in LPCOLESTR pszMkDocumentSource,
            /* [in] */ __RPC__in REFGUID rguidLogicalViewSource,
            /* [in] */ __RPC__in_opt IVsHierarchy *pvHier,
            /* [in] */ VSITEMID itemidSource,
            /* [out] */ __RPC__out VSITEMID *pitemidTarget,
            /* [out] */ __RPC__out VSSPECIFICEDITORFLAGS *pgrfEditorFlags,
            /* [out] */ __RPC__out GUID *pguidEditorTypeTarget,
            /* [out] */ __RPC__deref_out_opt BSTR *pbstrPhysicalViewTarget,
            /* [out] */ __RPC__out GUID *pguidLogicalViewTarget) = 0;
        
    };
    
#else 	/* C style interface */

    typedef struct IVsEditorFactory2Vtbl
    {
        BEGIN_INTERFACE
        
        HRESULT ( STDMETHODCALLTYPE *QueryInterface )( 
            IVsEditorFactory2 * This,
            /* [in] */ __RPC__in REFIID riid,
            /* [iid_is][out] */ 
            __RPC__deref_out  void **ppvObject);
        
        ULONG ( STDMETHODCALLTYPE *AddRef )( 
            IVsEditorFactory2 * This);
        
        ULONG ( STDMETHODCALLTYPE *Release )( 
            IVsEditorFactory2 * This);
        
        HRESULT ( STDMETHODCALLTYPE *RetargetCodeOrDesignerToOpen )( 
            IVsEditorFactory2 * This,
            /* [in] */ __RPC__in LPCOLESTR pszMkDocumentSource,
            /* [in] */ __RPC__in REFGUID rguidLogicalViewSource,
            /* [in] */ __RPC__in_opt IVsHierarchy *pvHier,
            /* [in] */ VSITEMID itemidSource,
            /* [out] */ __RPC__out VSITEMID *pitemidTarget,
            /* [out] */ __RPC__out VSSPECIFICEDITORFLAGS *pgrfEditorFlags,
            /* [out] */ __RPC__out GUID *pguidEditorTypeTarget,
            /* [out] */ __RPC__deref_out_opt BSTR *pbstrPhysicalViewTarget,
            /* [out] */ __RPC__out GUID *pguidLogicalViewTarget);
        
        END_INTERFACE
    } IVsEditorFactory2Vtbl;

    interface IVsEditorFactory2
    {
        CONST_VTBL struct IVsEditorFactory2Vtbl *lpVtbl;
    };

    

#ifdef COBJMACROS


#define IVsEditorFactory2_QueryInterface(This,riid,ppvObject)	\
    ( (This)->lpVtbl -> QueryInterface(This,riid,ppvObject) ) 

#define IVsEditorFactory2_AddRef(This)	\
    ( (This)->lpVtbl -> AddRef(This) ) 

#define IVsEditorFactory2_Release(This)	\
    ( (This)->lpVtbl -> Release(This) ) 


#define IVsEditorFactory2_RetargetCodeOrDesignerToOpen(This,pszMkDocumentSource,rguidLogicalViewSource,pvHier,itemidSource,pitemidTarget,pgrfEditorFlags,pguidEditorTypeTarget,pbstrPhysicalViewTarget,pguidLogicalViewTarget)	\
    ( (This)->lpVtbl -> RetargetCodeOrDesignerToOpen(This,pszMkDocumentSource,rguidLogicalViewSource,pvHier,itemidSource,pitemidTarget,pgrfEditorFlags,pguidEditorTypeTarget,pbstrPhysicalViewTarget,pguidLogicalViewTarget) ) 

#endif /* COBJMACROS */


#endif 	/* C style interface */




#endif 	/* __IVsEditorFactory2_INTERFACE_DEFINED__ */



#ifndef __VSShell90Internal_LIBRARY_DEFINED__
#define __VSShell90Internal_LIBRARY_DEFINED__

/* library VSShell90Internal */
/* [version][uuid] */ 


EXTERN_C const IID LIBID_VSShell90Internal;

EXTERN_C const CLSID CLSID_VsSymbolicNavigationManager;

#ifdef __cplusplus

class DECLSPEC_UUID("3FD6EE6A-1DF2-4305-9946-2146D0E16930")
VsSymbolicNavigationManager;
#endif

EXTERN_C const CLSID CLSID_VsMSBuildTaskFileManager;

#ifdef __cplusplus

class DECLSPEC_UUID("E2905C7C-4435-4212-9148-BE9614BD377B")
VsMSBuildTaskFileManager;
#endif
#endif /* __VSShell90Internal_LIBRARY_DEFINED__ */

/* interface __MIDL_itf_vsshell90_0000_0021 */
/* [local] */ 


enum _OLELOOP2
    {	oleloopModelessFormWithHandles	= 0x14,
	oleloopModelessFormWithHandlesNoIdle	= 0x15,
	oleloopModalFormWithHandles	= 0x16,
	oleloopModalFormWithHandlesNoIdle	= 0x17
    } ;


extern RPC_IF_HANDLE __MIDL_itf_vsshell90_0000_0021_v0_0_c_ifspec;
extern RPC_IF_HANDLE __MIDL_itf_vsshell90_0000_0021_v0_0_s_ifspec;

#ifndef __IOleComponent2_INTERFACE_DEFINED__
#define __IOleComponent2_INTERFACE_DEFINED__

/* interface IOleComponent2 */
/* [object][local][unique][version][uuid] */ 


EXTERN_C const IID IID_IOleComponent2;

#if defined(__cplusplus) && !defined(CINTERFACE)
    
    MIDL_INTERFACE("ED0751FC-D772-4d1d-88FC-0C1AA275391B")
    IOleComponent2 : public IOleComponent
    {
    public:
        virtual HRESULT STDMETHODCALLTYPE GetWaitHandlesAndTimeout( 
            /* [out] */ HANDLE aHandles[ 64 ],
            /* [out] */ UINT *pnHandles,
            /* [out] */ DWORD *pdwTimeout,
            /* [in] */ void *pvLoopData) = 0;
        
        virtual HRESULT STDMETHODCALLTYPE OnHandleSignaled( 
            /* [in] */ UINT nHandle,
            /* [in] */ void *pvLoopData,
            /* [out] */ VARIANT_BOOL *pfContinue) = 0;
        
        virtual HRESULT STDMETHODCALLTYPE OnTimeout( 
            /* [in] */ void *pvLoopData,
            /* [out] */ VARIANT_BOOL *pfContinue) = 0;
        
    };
    
#else 	/* C style interface */

    typedef struct IOleComponent2Vtbl
    {
        BEGIN_INTERFACE
        
        HRESULT ( STDMETHODCALLTYPE *QueryInterface )( 
            IOleComponent2 * This,
            /* [in] */ REFIID riid,
            /* [iid_is][out] */ 
            __RPC__deref_out  void **ppvObject);
        
        ULONG ( STDMETHODCALLTYPE *AddRef )( 
            IOleComponent2 * This);
        
        ULONG ( STDMETHODCALLTYPE *Release )( 
            IOleComponent2 * This);
        
        BOOL ( STDMETHODCALLTYPE *FReserved1 )( 
            IOleComponent2 * This,
            /* [in] */ DWORD dwReserved,
            /* [in] */ UINT message,
            /* [in] */ WPARAM wParam,
            /* [in] */ LPARAM lParam);
        
        BOOL ( STDMETHODCALLTYPE *FPreTranslateMessage )( 
            IOleComponent2 * This,
            /* [out][in] */ MSG *pMsg);
        
        void ( STDMETHODCALLTYPE *OnEnterState )( 
            IOleComponent2 * This,
            /* [in] */ OLECSTATE uStateID,
            /* [in] */ BOOL fEnter);
        
        void ( STDMETHODCALLTYPE *OnAppActivate )( 
            IOleComponent2 * This,
            /* [in] */ BOOL fActive,
            /* [in] */ DWORD dwOtherThreadID);
        
        void ( STDMETHODCALLTYPE *OnLoseActivation )( 
            IOleComponent2 * This);
        
        void ( STDMETHODCALLTYPE *OnActivationChange )( 
            IOleComponent2 * This,
            /* [in] */ IOleComponent *pic,
            /* [in] */ BOOL fSameComponent,
            /* [in] */ const OLECRINFO *pcrinfo,
            /* [in] */ BOOL fHostIsActivating,
            /* [in] */ const OLECHOSTINFO *pchostinfo,
            /* [in] */ DWORD dwReserved);
        
        BOOL ( STDMETHODCALLTYPE *FDoIdle )( 
            IOleComponent2 * This,
            /* [in] */ OLEIDLEF grfidlef);
        
        BOOL ( STDMETHODCALLTYPE *FContinueMessageLoop )( 
            IOleComponent2 * This,
            /* [in] */ OLELOOP uReason,
            /* [in] */ void *pvLoopData,
            /* [in] */ MSG *pMsgPeeked);
        
        BOOL ( STDMETHODCALLTYPE *FQueryTerminate )( 
            IOleComponent2 * This,
            /* [in] */ BOOL fPromptUser);
        
        void ( STDMETHODCALLTYPE *Terminate )( 
            IOleComponent2 * This);
        
        HWND ( STDMETHODCALLTYPE *HwndGetWindow )( 
            IOleComponent2 * This,
            /* [in] */ OLECWINDOW dwWhich,
            /* [in] */ DWORD dwReserved);
        
        HRESULT ( STDMETHODCALLTYPE *GetWaitHandlesAndTimeout )( 
            IOleComponent2 * This,
            /* [out] */ HANDLE aHandles[ 64 ],
            /* [out] */ UINT *pnHandles,
            /* [out] */ DWORD *pdwTimeout,
            /* [in] */ void *pvLoopData);
        
        HRESULT ( STDMETHODCALLTYPE *OnHandleSignaled )( 
            IOleComponent2 * This,
            /* [in] */ UINT nHandle,
            /* [in] */ void *pvLoopData,
            /* [out] */ VARIANT_BOOL *pfContinue);
        
        HRESULT ( STDMETHODCALLTYPE *OnTimeout )( 
            IOleComponent2 * This,
            /* [in] */ void *pvLoopData,
            /* [out] */ VARIANT_BOOL *pfContinue);
        
        END_INTERFACE
    } IOleComponent2Vtbl;

    interface IOleComponent2
    {
        CONST_VTBL struct IOleComponent2Vtbl *lpVtbl;
    };

    

#ifdef COBJMACROS


#define IOleComponent2_QueryInterface(This,riid,ppvObject)	\
    ( (This)->lpVtbl -> QueryInterface(This,riid,ppvObject) ) 

#define IOleComponent2_AddRef(This)	\
    ( (This)->lpVtbl -> AddRef(This) ) 

#define IOleComponent2_Release(This)	\
    ( (This)->lpVtbl -> Release(This) ) 


#define IOleComponent2_FReserved1(This,dwReserved,message,wParam,lParam)	\
    ( (This)->lpVtbl -> FReserved1(This,dwReserved,message,wParam,lParam) ) 

#define IOleComponent2_FPreTranslateMessage(This,pMsg)	\
    ( (This)->lpVtbl -> FPreTranslateMessage(This,pMsg) ) 

#define IOleComponent2_OnEnterState(This,uStateID,fEnter)	\
    ( (This)->lpVtbl -> OnEnterState(This,uStateID,fEnter) ) 

#define IOleComponent2_OnAppActivate(This,fActive,dwOtherThreadID)	\
    ( (This)->lpVtbl -> OnAppActivate(This,fActive,dwOtherThreadID) ) 

#define IOleComponent2_OnLoseActivation(This)	\
    ( (This)->lpVtbl -> OnLoseActivation(This) ) 

#define IOleComponent2_OnActivationChange(This,pic,fSameComponent,pcrinfo,fHostIsActivating,pchostinfo,dwReserved)	\
    ( (This)->lpVtbl -> OnActivationChange(This,pic,fSameComponent,pcrinfo,fHostIsActivating,pchostinfo,dwReserved) ) 

#define IOleComponent2_FDoIdle(This,grfidlef)	\
    ( (This)->lpVtbl -> FDoIdle(This,grfidlef) ) 

#define IOleComponent2_FContinueMessageLoop(This,uReason,pvLoopData,pMsgPeeked)	\
    ( (This)->lpVtbl -> FContinueMessageLoop(This,uReason,pvLoopData,pMsgPeeked) ) 

#define IOleComponent2_FQueryTerminate(This,fPromptUser)	\
    ( (This)->lpVtbl -> FQueryTerminate(This,fPromptUser) ) 

#define IOleComponent2_Terminate(This)	\
    ( (This)->lpVtbl -> Terminate(This) ) 

#define IOleComponent2_HwndGetWindow(This,dwWhich,dwReserved)	\
    ( (This)->lpVtbl -> HwndGetWindow(This,dwWhich,dwReserved) ) 


#define IOleComponent2_GetWaitHandlesAndTimeout(This,aHandles,pnHandles,pdwTimeout,pvLoopData)	\
    ( (This)->lpVtbl -> GetWaitHandlesAndTimeout(This,aHandles,pnHandles,pdwTimeout,pvLoopData) ) 

#define IOleComponent2_OnHandleSignaled(This,nHandle,pvLoopData,pfContinue)	\
    ( (This)->lpVtbl -> OnHandleSignaled(This,nHandle,pvLoopData,pfContinue) ) 

#define IOleComponent2_OnTimeout(This,pvLoopData,pfContinue)	\
    ( (This)->lpVtbl -> OnTimeout(This,pvLoopData,pfContinue) ) 

#endif /* COBJMACROS */


#endif 	/* C style interface */




#endif 	/* __IOleComponent2_INTERFACE_DEFINED__ */


#ifndef __IVsPackage2_INTERFACE_DEFINED__
#define __IVsPackage2_INTERFACE_DEFINED__

/* interface IVsPackage2 */
/* [object][unique][version][uuid] */ 


EXTERN_C const IID IID_IVsPackage2;

#if defined(__cplusplus) && !defined(CINTERFACE)
    
    MIDL_INTERFACE("0130701B-B0BE-474b-B4B6-35BABB2008F1")
    IVsPackage2 : public IUnknown
    {
    public:
        virtual HRESULT STDMETHODCALLTYPE get_CanClose( 
            /* [retval][out] */ __RPC__out VARIANT_BOOL *pfCanClose) = 0;
        
    };
    
#else 	/* C style interface */

    typedef struct IVsPackage2Vtbl
    {
        BEGIN_INTERFACE
        
        HRESULT ( STDMETHODCALLTYPE *QueryInterface )( 
            IVsPackage2 * This,
            /* [in] */ __RPC__in REFIID riid,
            /* [iid_is][out] */ 
            __RPC__deref_out  void **ppvObject);
        
        ULONG ( STDMETHODCALLTYPE *AddRef )( 
            IVsPackage2 * This);
        
        ULONG ( STDMETHODCALLTYPE *Release )( 
            IVsPackage2 * This);
        
        HRESULT ( STDMETHODCALLTYPE *get_CanClose )( 
            IVsPackage2 * This,
            /* [retval][out] */ __RPC__out VARIANT_BOOL *pfCanClose);
        
        END_INTERFACE
    } IVsPackage2Vtbl;

    interface IVsPackage2
    {
        CONST_VTBL struct IVsPackage2Vtbl *lpVtbl;
    };

    

#ifdef COBJMACROS


#define IVsPackage2_QueryInterface(This,riid,ppvObject)	\
    ( (This)->lpVtbl -> QueryInterface(This,riid,ppvObject) ) 

#define IVsPackage2_AddRef(This)	\
    ( (This)->lpVtbl -> AddRef(This) ) 

#define IVsPackage2_Release(This)	\
    ( (This)->lpVtbl -> Release(This) ) 


#define IVsPackage2_get_CanClose(This,pfCanClose)	\
    ( (This)->lpVtbl -> get_CanClose(This,pfCanClose) ) 

#endif /* COBJMACROS */


#endif 	/* C style interface */




#endif 	/* __IVsPackage2_INTERFACE_DEFINED__ */


#ifndef __IVsLaunchPad3_INTERFACE_DEFINED__
#define __IVsLaunchPad3_INTERFACE_DEFINED__

/* interface IVsLaunchPad3 */
/* [object][unique][version][uuid] */ 


EXTERN_C const IID IID_IVsLaunchPad3;

#if defined(__cplusplus) && !defined(CINTERFACE)
    
    MIDL_INTERFACE("96A8C871-BD60-47cd-BCBA-D4455806C54F")
    IVsLaunchPad3 : public IUnknown
    {
    public:
        virtual HRESULT STDMETHODCALLTYPE ExecCommandWithElevationIfRequired( 
            /* [in] */ __RPC__in LPCOLESTR pszApplicationName,
            /* [in] */ __RPC__in LPCOLESTR pszCommandLine,
            /* [in] */ __RPC__in LPCOLESTR pszWorkingDir) = 0;
        
    };
    
#else 	/* C style interface */

    typedef struct IVsLaunchPad3Vtbl
    {
        BEGIN_INTERFACE
        
        HRESULT ( STDMETHODCALLTYPE *QueryInterface )( 
            IVsLaunchPad3 * This,
            /* [in] */ __RPC__in REFIID riid,
            /* [iid_is][out] */ 
            __RPC__deref_out  void **ppvObject);
        
        ULONG ( STDMETHODCALLTYPE *AddRef )( 
            IVsLaunchPad3 * This);
        
        ULONG ( STDMETHODCALLTYPE *Release )( 
            IVsLaunchPad3 * This);
        
        HRESULT ( STDMETHODCALLTYPE *ExecCommandWithElevationIfRequired )( 
            IVsLaunchPad3 * This,
            /* [in] */ __RPC__in LPCOLESTR pszApplicationName,
            /* [in] */ __RPC__in LPCOLESTR pszCommandLine,
            /* [in] */ __RPC__in LPCOLESTR pszWorkingDir);
        
        END_INTERFACE
    } IVsLaunchPad3Vtbl;

    interface IVsLaunchPad3
    {
        CONST_VTBL struct IVsLaunchPad3Vtbl *lpVtbl;
    };

    

#ifdef COBJMACROS


#define IVsLaunchPad3_QueryInterface(This,riid,ppvObject)	\
    ( (This)->lpVtbl -> QueryInterface(This,riid,ppvObject) ) 

#define IVsLaunchPad3_AddRef(This)	\
    ( (This)->lpVtbl -> AddRef(This) ) 

#define IVsLaunchPad3_Release(This)	\
    ( (This)->lpVtbl -> Release(This) ) 


#define IVsLaunchPad3_ExecCommandWithElevationIfRequired(This,pszApplicationName,pszCommandLine,pszWorkingDir)	\
    ( (This)->lpVtbl -> ExecCommandWithElevationIfRequired(This,pszApplicationName,pszCommandLine,pszWorkingDir) ) 

#endif /* COBJMACROS */


#endif 	/* C style interface */




#endif 	/* __IVsLaunchPad3_INTERFACE_DEFINED__ */


/* Additional Prototypes for ALL interfaces */

unsigned long             __RPC_USER  BSTR_UserSize(     unsigned long *, unsigned long            , BSTR * ); 
unsigned char * __RPC_USER  BSTR_UserMarshal(  unsigned long *, unsigned char *, BSTR * ); 
unsigned char * __RPC_USER  BSTR_UserUnmarshal(unsigned long *, unsigned char *, BSTR * ); 
void                      __RPC_USER  BSTR_UserFree(     unsigned long *, BSTR * ); 

unsigned long             __RPC_USER  LPSAFEARRAY_UserSize(     unsigned long *, unsigned long            , LPSAFEARRAY * ); 
unsigned char * __RPC_USER  LPSAFEARRAY_UserMarshal(  unsigned long *, unsigned char *, LPSAFEARRAY * ); 
unsigned char * __RPC_USER  LPSAFEARRAY_UserUnmarshal(unsigned long *, unsigned char *, LPSAFEARRAY * ); 
void                      __RPC_USER  LPSAFEARRAY_UserFree(     unsigned long *, LPSAFEARRAY * ); 

unsigned long             __RPC_USER  VARIANT_UserSize(     unsigned long *, unsigned long            , VARIANT * ); 
unsigned char * __RPC_USER  VARIANT_UserMarshal(  unsigned long *, unsigned char *, VARIANT * ); 
unsigned char * __RPC_USER  VARIANT_UserUnmarshal(unsigned long *, unsigned char *, VARIANT * ); 
void                      __RPC_USER  VARIANT_UserFree(     unsigned long *, VARIANT * ); 

/* end of Additional Prototypes */

#ifdef __cplusplus
}
#endif

#endif


