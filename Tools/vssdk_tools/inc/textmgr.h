

/* this ALWAYS GENERATED file contains the definitions for the interfaces */


 /* File created by MIDL compiler version 7.00.0499 */
/* Compiler settings for textmgr.idl:
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

#ifndef __textmgr_h__
#define __textmgr_h__

#if defined(_MSC_VER) && (_MSC_VER >= 1020)
#pragma once
#endif

/* Forward Declarations */ 

#ifndef __IVsCompoundAction_FWD_DEFINED__
#define __IVsCompoundAction_FWD_DEFINED__
typedef interface IVsCompoundAction IVsCompoundAction;
#endif 	/* __IVsCompoundAction_FWD_DEFINED__ */


#ifndef __IVsCompoundViewChange_FWD_DEFINED__
#define __IVsCompoundViewChange_FWD_DEFINED__
typedef interface IVsCompoundViewChange IVsCompoundViewChange;
#endif 	/* __IVsCompoundViewChange_FWD_DEFINED__ */


#ifndef __IVsCompoundActionEvents_FWD_DEFINED__
#define __IVsCompoundActionEvents_FWD_DEFINED__
typedef interface IVsCompoundActionEvents IVsCompoundActionEvents;
#endif 	/* __IVsCompoundActionEvents_FWD_DEFINED__ */


#ifndef __IVsAction_FWD_DEFINED__
#define __IVsAction_FWD_DEFINED__
typedef interface IVsAction IVsAction;
#endif 	/* __IVsAction_FWD_DEFINED__ */


#ifndef __IVsTextEditorPropertyCategoryContainer_FWD_DEFINED__
#define __IVsTextEditorPropertyCategoryContainer_FWD_DEFINED__
typedef interface IVsTextEditorPropertyCategoryContainer IVsTextEditorPropertyCategoryContainer;
#endif 	/* __IVsTextEditorPropertyCategoryContainer_FWD_DEFINED__ */


#ifndef __IVsTextEditorPropertyContainer_FWD_DEFINED__
#define __IVsTextEditorPropertyContainer_FWD_DEFINED__
typedef interface IVsTextEditorPropertyContainer IVsTextEditorPropertyContainer;
#endif 	/* __IVsTextEditorPropertyContainer_FWD_DEFINED__ */


#ifndef __IVsCompletionSet_FWD_DEFINED__
#define __IVsCompletionSet_FWD_DEFINED__
typedef interface IVsCompletionSet IVsCompletionSet;
#endif 	/* __IVsCompletionSet_FWD_DEFINED__ */


#ifndef __IVsTipWindow_FWD_DEFINED__
#define __IVsTipWindow_FWD_DEFINED__
typedef interface IVsTipWindow IVsTipWindow;
#endif 	/* __IVsTipWindow_FWD_DEFINED__ */


#ifndef __IVsMethodTipWindow_FWD_DEFINED__
#define __IVsMethodTipWindow_FWD_DEFINED__
typedef interface IVsMethodTipWindow IVsMethodTipWindow;
#endif 	/* __IVsMethodTipWindow_FWD_DEFINED__ */


#ifndef __IVsTextTipWindow_FWD_DEFINED__
#define __IVsTextTipWindow_FWD_DEFINED__
typedef interface IVsTextTipWindow IVsTextTipWindow;
#endif 	/* __IVsTextTipWindow_FWD_DEFINED__ */


#ifndef __IVsMethodData_FWD_DEFINED__
#define __IVsMethodData_FWD_DEFINED__
typedef interface IVsMethodData IVsMethodData;
#endif 	/* __IVsMethodData_FWD_DEFINED__ */


#ifndef __IVsTextTipData_FWD_DEFINED__
#define __IVsTextTipData_FWD_DEFINED__
typedef interface IVsTextTipData IVsTextTipData;
#endif 	/* __IVsTextTipData_FWD_DEFINED__ */


#ifndef __IVsTextView_FWD_DEFINED__
#define __IVsTextView_FWD_DEFINED__
typedef interface IVsTextView IVsTextView;
#endif 	/* __IVsTextView_FWD_DEFINED__ */


#ifndef __IVsThreadSafeTextView_FWD_DEFINED__
#define __IVsThreadSafeTextView_FWD_DEFINED__
typedef interface IVsThreadSafeTextView IVsThreadSafeTextView;
#endif 	/* __IVsThreadSafeTextView_FWD_DEFINED__ */


#ifndef __IVsLayeredTextView_FWD_DEFINED__
#define __IVsLayeredTextView_FWD_DEFINED__
typedef interface IVsLayeredTextView IVsLayeredTextView;
#endif 	/* __IVsLayeredTextView_FWD_DEFINED__ */


#ifndef __IVsTextViewFilter_FWD_DEFINED__
#define __IVsTextViewFilter_FWD_DEFINED__
typedef interface IVsTextViewFilter IVsTextViewFilter;
#endif 	/* __IVsTextViewFilter_FWD_DEFINED__ */


#ifndef __IVsLanguageContextProvider_FWD_DEFINED__
#define __IVsLanguageContextProvider_FWD_DEFINED__
typedef interface IVsLanguageContextProvider IVsLanguageContextProvider;
#endif 	/* __IVsLanguageContextProvider_FWD_DEFINED__ */


#ifndef __IVsTextMarkerContextProvider_FWD_DEFINED__
#define __IVsTextMarkerContextProvider_FWD_DEFINED__
typedef interface IVsTextMarkerContextProvider IVsTextMarkerContextProvider;
#endif 	/* __IVsTextMarkerContextProvider_FWD_DEFINED__ */


#ifndef __IVsViewRangeClient_FWD_DEFINED__
#define __IVsViewRangeClient_FWD_DEFINED__
typedef interface IVsViewRangeClient IVsViewRangeClient;
#endif 	/* __IVsViewRangeClient_FWD_DEFINED__ */


#ifndef __IVsTextViewEvents_FWD_DEFINED__
#define __IVsTextViewEvents_FWD_DEFINED__
typedef interface IVsTextViewEvents IVsTextViewEvents;
#endif 	/* __IVsTextViewEvents_FWD_DEFINED__ */


#ifndef __IVsTextManager_FWD_DEFINED__
#define __IVsTextManager_FWD_DEFINED__
typedef interface IVsTextManager IVsTextManager;
#endif 	/* __IVsTextManager_FWD_DEFINED__ */


#ifndef __IVsShortcutManager_FWD_DEFINED__
#define __IVsShortcutManager_FWD_DEFINED__
typedef interface IVsShortcutManager IVsShortcutManager;
#endif 	/* __IVsShortcutManager_FWD_DEFINED__ */


#ifndef __IVsEnumTextBuffers_FWD_DEFINED__
#define __IVsEnumTextBuffers_FWD_DEFINED__
typedef interface IVsEnumTextBuffers IVsEnumTextBuffers;
#endif 	/* __IVsEnumTextBuffers_FWD_DEFINED__ */


#ifndef __IVsTextManagerEvents_FWD_DEFINED__
#define __IVsTextManagerEvents_FWD_DEFINED__
typedef interface IVsTextManagerEvents IVsTextManagerEvents;
#endif 	/* __IVsTextManagerEvents_FWD_DEFINED__ */


#ifndef __IVsTextSelectionAction_FWD_DEFINED__
#define __IVsTextSelectionAction_FWD_DEFINED__
typedef interface IVsTextSelectionAction IVsTextSelectionAction;
#endif 	/* __IVsTextSelectionAction_FWD_DEFINED__ */


#ifndef __IVsEnumTextViews_FWD_DEFINED__
#define __IVsEnumTextViews_FWD_DEFINED__
typedef interface IVsEnumTextViews IVsEnumTextViews;
#endif 	/* __IVsEnumTextViews_FWD_DEFINED__ */


#ifndef __IVsEnumIndependentViews_FWD_DEFINED__
#define __IVsEnumIndependentViews_FWD_DEFINED__
typedef interface IVsEnumIndependentViews IVsEnumIndependentViews;
#endif 	/* __IVsEnumIndependentViews_FWD_DEFINED__ */


#ifndef __IVsColorableItem_FWD_DEFINED__
#define __IVsColorableItem_FWD_DEFINED__
typedef interface IVsColorableItem IVsColorableItem;
#endif 	/* __IVsColorableItem_FWD_DEFINED__ */


#ifndef __IVsMergeableUIItem_FWD_DEFINED__
#define __IVsMergeableUIItem_FWD_DEFINED__
typedef interface IVsMergeableUIItem IVsMergeableUIItem;
#endif 	/* __IVsMergeableUIItem_FWD_DEFINED__ */


#ifndef __IVsProvideColorableItems_FWD_DEFINED__
#define __IVsProvideColorableItems_FWD_DEFINED__
typedef interface IVsProvideColorableItems IVsProvideColorableItems;
#endif 	/* __IVsProvideColorableItems_FWD_DEFINED__ */


#ifndef __IVsColorizer_FWD_DEFINED__
#define __IVsColorizer_FWD_DEFINED__
typedef interface IVsColorizer IVsColorizer;
#endif 	/* __IVsColorizer_FWD_DEFINED__ */


#ifndef __IVsEnumGUID_FWD_DEFINED__
#define __IVsEnumGUID_FWD_DEFINED__
typedef interface IVsEnumGUID IVsEnumGUID;
#endif 	/* __IVsEnumGUID_FWD_DEFINED__ */


#ifndef __IVsLanguageInfo_FWD_DEFINED__
#define __IVsLanguageInfo_FWD_DEFINED__
typedef interface IVsLanguageInfo IVsLanguageInfo;
#endif 	/* __IVsLanguageInfo_FWD_DEFINED__ */


#ifndef __IVsLanguageBlock_FWD_DEFINED__
#define __IVsLanguageBlock_FWD_DEFINED__
typedef interface IVsLanguageBlock IVsLanguageBlock;
#endif 	/* __IVsLanguageBlock_FWD_DEFINED__ */


#ifndef __IVsEnumBSTR_FWD_DEFINED__
#define __IVsEnumBSTR_FWD_DEFINED__
typedef interface IVsEnumBSTR IVsEnumBSTR;
#endif 	/* __IVsEnumBSTR_FWD_DEFINED__ */


#ifndef __IVsDebugName_FWD_DEFINED__
#define __IVsDebugName_FWD_DEFINED__
typedef interface IVsDebugName IVsDebugName;
#endif 	/* __IVsDebugName_FWD_DEFINED__ */


#ifndef __IVsEnumDebugName_FWD_DEFINED__
#define __IVsEnumDebugName_FWD_DEFINED__
typedef interface IVsEnumDebugName IVsEnumDebugName;
#endif 	/* __IVsEnumDebugName_FWD_DEFINED__ */


#ifndef __IVsLanguageDebugInfo_FWD_DEFINED__
#define __IVsLanguageDebugInfo_FWD_DEFINED__
typedef interface IVsLanguageDebugInfo IVsLanguageDebugInfo;
#endif 	/* __IVsLanguageDebugInfo_FWD_DEFINED__ */


#ifndef __IVsTextBufferTempInit_FWD_DEFINED__
#define __IVsTextBufferTempInit_FWD_DEFINED__
typedef interface IVsTextBufferTempInit IVsTextBufferTempInit;
#endif 	/* __IVsTextBufferTempInit_FWD_DEFINED__ */


#ifndef __IVsTextBuffer_FWD_DEFINED__
#define __IVsTextBuffer_FWD_DEFINED__
typedef interface IVsTextBuffer IVsTextBuffer;
#endif 	/* __IVsTextBuffer_FWD_DEFINED__ */


#ifndef __IVsTextLines_FWD_DEFINED__
#define __IVsTextLines_FWD_DEFINED__
typedef interface IVsTextLines IVsTextLines;
#endif 	/* __IVsTextLines_FWD_DEFINED__ */


#ifndef __IVsTextColorState_FWD_DEFINED__
#define __IVsTextColorState_FWD_DEFINED__
typedef interface IVsTextColorState IVsTextColorState;
#endif 	/* __IVsTextColorState_FWD_DEFINED__ */


#ifndef __IVsTextLinesEvents_FWD_DEFINED__
#define __IVsTextLinesEvents_FWD_DEFINED__
typedef interface IVsTextLinesEvents IVsTextLinesEvents;
#endif 	/* __IVsTextLinesEvents_FWD_DEFINED__ */


#ifndef __IVsTextStream_FWD_DEFINED__
#define __IVsTextStream_FWD_DEFINED__
typedef interface IVsTextStream IVsTextStream;
#endif 	/* __IVsTextStream_FWD_DEFINED__ */


#ifndef __IVsEnumStreamMarkers_FWD_DEFINED__
#define __IVsEnumStreamMarkers_FWD_DEFINED__
typedef interface IVsEnumStreamMarkers IVsEnumStreamMarkers;
#endif 	/* __IVsEnumStreamMarkers_FWD_DEFINED__ */


#ifndef __IVsEnumLineMarkers_FWD_DEFINED__
#define __IVsEnumLineMarkers_FWD_DEFINED__
typedef interface IVsEnumLineMarkers IVsEnumLineMarkers;
#endif 	/* __IVsEnumLineMarkers_FWD_DEFINED__ */


#ifndef __IVsEnumLayerMarkers_FWD_DEFINED__
#define __IVsEnumLayerMarkers_FWD_DEFINED__
typedef interface IVsEnumLayerMarkers IVsEnumLayerMarkers;
#endif 	/* __IVsEnumLayerMarkers_FWD_DEFINED__ */


#ifndef __IVsTextMarker_FWD_DEFINED__
#define __IVsTextMarker_FWD_DEFINED__
typedef interface IVsTextMarker IVsTextMarker;
#endif 	/* __IVsTextMarker_FWD_DEFINED__ */


#ifndef __IVsTextStreamMarker_FWD_DEFINED__
#define __IVsTextStreamMarker_FWD_DEFINED__
typedef interface IVsTextStreamMarker IVsTextStreamMarker;
#endif 	/* __IVsTextStreamMarker_FWD_DEFINED__ */


#ifndef __IVsTextLineMarker_FWD_DEFINED__
#define __IVsTextLineMarker_FWD_DEFINED__
typedef interface IVsTextLineMarker IVsTextLineMarker;
#endif 	/* __IVsTextLineMarker_FWD_DEFINED__ */


#ifndef __IVsTextLayerMarker_FWD_DEFINED__
#define __IVsTextLayerMarker_FWD_DEFINED__
typedef interface IVsTextLayerMarker IVsTextLayerMarker;
#endif 	/* __IVsTextLayerMarker_FWD_DEFINED__ */


#ifndef __IVsTextMarkerType_FWD_DEFINED__
#define __IVsTextMarkerType_FWD_DEFINED__
typedef interface IVsTextMarkerType IVsTextMarkerType;
#endif 	/* __IVsTextMarkerType_FWD_DEFINED__ */


#ifndef __IVsTextMarkerColorSet_FWD_DEFINED__
#define __IVsTextMarkerColorSet_FWD_DEFINED__
typedef interface IVsTextMarkerColorSet IVsTextMarkerColorSet;
#endif 	/* __IVsTextMarkerColorSet_FWD_DEFINED__ */


#ifndef __IVsPackageDefinedTextMarkerType_FWD_DEFINED__
#define __IVsPackageDefinedTextMarkerType_FWD_DEFINED__
typedef interface IVsPackageDefinedTextMarkerType IVsPackageDefinedTextMarkerType;
#endif 	/* __IVsPackageDefinedTextMarkerType_FWD_DEFINED__ */


#ifndef __IVsTextMarkerTypeProvider_FWD_DEFINED__
#define __IVsTextMarkerTypeProvider_FWD_DEFINED__
typedef interface IVsTextMarkerTypeProvider IVsTextMarkerTypeProvider;
#endif 	/* __IVsTextMarkerTypeProvider_FWD_DEFINED__ */


#ifndef __IVsTextMarkerClient_FWD_DEFINED__
#define __IVsTextMarkerClient_FWD_DEFINED__
typedef interface IVsTextMarkerClient IVsTextMarkerClient;
#endif 	/* __IVsTextMarkerClient_FWD_DEFINED__ */


#ifndef __IVsMouseCursorProvider_FWD_DEFINED__
#define __IVsMouseCursorProvider_FWD_DEFINED__
typedef interface IVsMouseCursorProvider IVsMouseCursorProvider;
#endif 	/* __IVsMouseCursorProvider_FWD_DEFINED__ */


#ifndef __IVsTextMarkerGlyphDropHandler_FWD_DEFINED__
#define __IVsTextMarkerGlyphDropHandler_FWD_DEFINED__
typedef interface IVsTextMarkerGlyphDropHandler IVsTextMarkerGlyphDropHandler;
#endif 	/* __IVsTextMarkerGlyphDropHandler_FWD_DEFINED__ */


#ifndef __IVsTextTrackingPoint_FWD_DEFINED__
#define __IVsTextTrackingPoint_FWD_DEFINED__
typedef interface IVsTextTrackingPoint IVsTextTrackingPoint;
#endif 	/* __IVsTextTrackingPoint_FWD_DEFINED__ */


#ifndef __IVsFullTextScanner_FWD_DEFINED__
#define __IVsFullTextScanner_FWD_DEFINED__
typedef interface IVsFullTextScanner IVsFullTextScanner;
#endif 	/* __IVsFullTextScanner_FWD_DEFINED__ */


#ifndef __IVsTextScanner_FWD_DEFINED__
#define __IVsTextScanner_FWD_DEFINED__
typedef interface IVsTextScanner IVsTextScanner;
#endif 	/* __IVsTextScanner_FWD_DEFINED__ */


#ifndef __IVsTextFind_FWD_DEFINED__
#define __IVsTextFind_FWD_DEFINED__
typedef interface IVsTextFind IVsTextFind;
#endif 	/* __IVsTextFind_FWD_DEFINED__ */


#ifndef __IVsTextBufferEvents_FWD_DEFINED__
#define __IVsTextBufferEvents_FWD_DEFINED__
typedef interface IVsTextBufferEvents IVsTextBufferEvents;
#endif 	/* __IVsTextBufferEvents_FWD_DEFINED__ */


#ifndef __IVsTextBufferDataEvents_FWD_DEFINED__
#define __IVsTextBufferDataEvents_FWD_DEFINED__
typedef interface IVsTextBufferDataEvents IVsTextBufferDataEvents;
#endif 	/* __IVsTextBufferDataEvents_FWD_DEFINED__ */


#ifndef __IVsTextStreamEvents_FWD_DEFINED__
#define __IVsTextStreamEvents_FWD_DEFINED__
typedef interface IVsTextStreamEvents IVsTextStreamEvents;
#endif 	/* __IVsTextStreamEvents_FWD_DEFINED__ */


#ifndef __IVsSplitPane_FWD_DEFINED__
#define __IVsSplitPane_FWD_DEFINED__
typedef interface IVsSplitPane IVsSplitPane;
#endif 	/* __IVsSplitPane_FWD_DEFINED__ */


#ifndef __IVsSplitter_FWD_DEFINED__
#define __IVsSplitter_FWD_DEFINED__
typedef interface IVsSplitter IVsSplitter;
#endif 	/* __IVsSplitter_FWD_DEFINED__ */


#ifndef __IVsSplitRoot_FWD_DEFINED__
#define __IVsSplitRoot_FWD_DEFINED__
typedef interface IVsSplitRoot IVsSplitRoot;
#endif 	/* __IVsSplitRoot_FWD_DEFINED__ */


#ifndef __IVsCodeWindow_FWD_DEFINED__
#define __IVsCodeWindow_FWD_DEFINED__
typedef interface IVsCodeWindow IVsCodeWindow;
#endif 	/* __IVsCodeWindow_FWD_DEFINED__ */


#ifndef __IVsCodeWindowManager_FWD_DEFINED__
#define __IVsCodeWindowManager_FWD_DEFINED__
typedef interface IVsCodeWindowManager IVsCodeWindowManager;
#endif 	/* __IVsCodeWindowManager_FWD_DEFINED__ */


#ifndef __IVsDropdownBarClient_FWD_DEFINED__
#define __IVsDropdownBarClient_FWD_DEFINED__
typedef interface IVsDropdownBarClient IVsDropdownBarClient;
#endif 	/* __IVsDropdownBarClient_FWD_DEFINED__ */


#ifndef __IVsDropdownBar_FWD_DEFINED__
#define __IVsDropdownBar_FWD_DEFINED__
typedef interface IVsDropdownBar IVsDropdownBar;
#endif 	/* __IVsDropdownBar_FWD_DEFINED__ */


#ifndef __IVsDropdownBarManager_FWD_DEFINED__
#define __IVsDropdownBarManager_FWD_DEFINED__
typedef interface IVsDropdownBarManager IVsDropdownBarManager;
#endif 	/* __IVsDropdownBarManager_FWD_DEFINED__ */


#ifndef __IVsButtonBarClient_FWD_DEFINED__
#define __IVsButtonBarClient_FWD_DEFINED__
typedef interface IVsButtonBarClient IVsButtonBarClient;
#endif 	/* __IVsButtonBarClient_FWD_DEFINED__ */


#ifndef __IVsButtonBar_FWD_DEFINED__
#define __IVsButtonBar_FWD_DEFINED__
typedef interface IVsButtonBar IVsButtonBar;
#endif 	/* __IVsButtonBar_FWD_DEFINED__ */


#ifndef __IVsButtonBarManager_FWD_DEFINED__
#define __IVsButtonBarManager_FWD_DEFINED__
typedef interface IVsButtonBarManager IVsButtonBarManager;
#endif 	/* __IVsButtonBarManager_FWD_DEFINED__ */


#ifndef __IVsDefaultButtonBarImages_FWD_DEFINED__
#define __IVsDefaultButtonBarImages_FWD_DEFINED__
typedef interface IVsDefaultButtonBarImages IVsDefaultButtonBarImages;
#endif 	/* __IVsDefaultButtonBarImages_FWD_DEFINED__ */


#ifndef __IVsTextMacroHelper_FWD_DEFINED__
#define __IVsTextMacroHelper_FWD_DEFINED__
typedef interface IVsTextMacroHelper IVsTextMacroHelper;
#endif 	/* __IVsTextMacroHelper_FWD_DEFINED__ */


#ifndef __IVsUserData_FWD_DEFINED__
#define __IVsUserData_FWD_DEFINED__
typedef interface IVsUserData IVsUserData;
#endif 	/* __IVsUserData_FWD_DEFINED__ */


#ifndef __IVsUserDataEvents_FWD_DEFINED__
#define __IVsUserDataEvents_FWD_DEFINED__
typedef interface IVsUserDataEvents IVsUserDataEvents;
#endif 	/* __IVsUserDataEvents_FWD_DEFINED__ */


#ifndef __IVsUndoUnit_FWD_DEFINED__
#define __IVsUndoUnit_FWD_DEFINED__
typedef interface IVsUndoUnit IVsUndoUnit;
#endif 	/* __IVsUndoUnit_FWD_DEFINED__ */


#ifndef __IVsUndoTrackingEvents_FWD_DEFINED__
#define __IVsUndoTrackingEvents_FWD_DEFINED__
typedef interface IVsUndoTrackingEvents IVsUndoTrackingEvents;
#endif 	/* __IVsUndoTrackingEvents_FWD_DEFINED__ */


#ifndef __IVsChangeTrackingUndoManager_FWD_DEFINED__
#define __IVsChangeTrackingUndoManager_FWD_DEFINED__
typedef interface IVsChangeTrackingUndoManager IVsChangeTrackingUndoManager;
#endif 	/* __IVsChangeTrackingUndoManager_FWD_DEFINED__ */


#ifndef __IVsLinkedUndoClient_FWD_DEFINED__
#define __IVsLinkedUndoClient_FWD_DEFINED__
typedef interface IVsLinkedUndoClient IVsLinkedUndoClient;
#endif 	/* __IVsLinkedUndoClient_FWD_DEFINED__ */


#ifndef __IVsLinkCapableUndoManager_FWD_DEFINED__
#define __IVsLinkCapableUndoManager_FWD_DEFINED__
typedef interface IVsLinkCapableUndoManager IVsLinkCapableUndoManager;
#endif 	/* __IVsLinkCapableUndoManager_FWD_DEFINED__ */


#ifndef __IVsLifetimeControlledObject_FWD_DEFINED__
#define __IVsLifetimeControlledObject_FWD_DEFINED__
typedef interface IVsLifetimeControlledObject IVsLifetimeControlledObject;
#endif 	/* __IVsLifetimeControlledObject_FWD_DEFINED__ */


#ifndef __IVsLinkedUndoTransactionManager_FWD_DEFINED__
#define __IVsLinkedUndoTransactionManager_FWD_DEFINED__
typedef interface IVsLinkedUndoTransactionManager IVsLinkedUndoTransactionManager;
#endif 	/* __IVsLinkedUndoTransactionManager_FWD_DEFINED__ */


#ifndef __IVsChangeClusterEvents_FWD_DEFINED__
#define __IVsChangeClusterEvents_FWD_DEFINED__
typedef interface IVsChangeClusterEvents IVsChangeClusterEvents;
#endif 	/* __IVsChangeClusterEvents_FWD_DEFINED__ */


#ifndef __VsLocalUserData_FWD_DEFINED__
#define __VsLocalUserData_FWD_DEFINED__

#ifdef __cplusplus
typedef class VsLocalUserData VsLocalUserData;
#else
typedef struct VsLocalUserData VsLocalUserData;
#endif /* __cplusplus */

#endif 	/* __VsLocalUserData_FWD_DEFINED__ */


#ifndef __VsUserData_FWD_DEFINED__
#define __VsUserData_FWD_DEFINED__

#ifdef __cplusplus
typedef class VsUserData VsUserData;
#else
typedef struct VsUserData VsUserData;
#endif /* __cplusplus */

#endif 	/* __VsUserData_FWD_DEFINED__ */


#ifndef __VsCodeWindow_FWD_DEFINED__
#define __VsCodeWindow_FWD_DEFINED__

#ifdef __cplusplus
typedef class VsCodeWindow VsCodeWindow;
#else
typedef struct VsCodeWindow VsCodeWindow;
#endif /* __cplusplus */

#endif 	/* __VsCodeWindow_FWD_DEFINED__ */


#ifndef __VsSplitRoot_FWD_DEFINED__
#define __VsSplitRoot_FWD_DEFINED__

#ifdef __cplusplus
typedef class VsSplitRoot VsSplitRoot;
#else
typedef struct VsSplitRoot VsSplitRoot;
#endif /* __cplusplus */

#endif 	/* __VsSplitRoot_FWD_DEFINED__ */


#ifndef __VsDropdownBar_FWD_DEFINED__
#define __VsDropdownBar_FWD_DEFINED__

#ifdef __cplusplus
typedef class VsDropdownBar VsDropdownBar;
#else
typedef struct VsDropdownBar VsDropdownBar;
#endif /* __cplusplus */

#endif 	/* __VsDropdownBar_FWD_DEFINED__ */


#ifndef __VsTextBuffer_FWD_DEFINED__
#define __VsTextBuffer_FWD_DEFINED__

#ifdef __cplusplus
typedef class VsTextBuffer VsTextBuffer;
#else
typedef struct VsTextBuffer VsTextBuffer;
#endif /* __cplusplus */

#endif 	/* __VsTextBuffer_FWD_DEFINED__ */


#ifndef __VsTextLineStorage_FWD_DEFINED__
#define __VsTextLineStorage_FWD_DEFINED__

#ifdef __cplusplus
typedef class VsTextLineStorage VsTextLineStorage;
#else
typedef struct VsTextLineStorage VsTextLineStorage;
#endif /* __cplusplus */

#endif 	/* __VsTextLineStorage_FWD_DEFINED__ */


#ifndef __VsTextManager_FWD_DEFINED__
#define __VsTextManager_FWD_DEFINED__

#ifdef __cplusplus
typedef class VsTextManager VsTextManager;
#else
typedef struct VsTextManager VsTextManager;
#endif /* __cplusplus */

#endif 	/* __VsTextManager_FWD_DEFINED__ */


#ifndef __VsTextView_FWD_DEFINED__
#define __VsTextView_FWD_DEFINED__

#ifdef __cplusplus
typedef class VsTextView VsTextView;
#else
typedef struct VsTextView VsTextView;
#endif /* __cplusplus */

#endif 	/* __VsTextView_FWD_DEFINED__ */


#ifndef __VsTextPackage_FWD_DEFINED__
#define __VsTextPackage_FWD_DEFINED__

#ifdef __cplusplus
typedef class VsTextPackage VsTextPackage;
#else
typedef struct VsTextPackage VsTextPackage;
#endif /* __cplusplus */

#endif 	/* __VsTextPackage_FWD_DEFINED__ */


#ifndef __VsUndoPackage_FWD_DEFINED__
#define __VsUndoPackage_FWD_DEFINED__

#ifdef __cplusplus
typedef class VsUndoPackage VsUndoPackage;
#else
typedef struct VsUndoPackage VsUndoPackage;
#endif /* __cplusplus */

#endif 	/* __VsUndoPackage_FWD_DEFINED__ */


#ifndef __VsUndoUnit_FWD_DEFINED__
#define __VsUndoUnit_FWD_DEFINED__

#ifdef __cplusplus
typedef class VsUndoUnit VsUndoUnit;
#else
typedef struct VsUndoUnit VsUndoUnit;
#endif /* __cplusplus */

#endif 	/* __VsUndoUnit_FWD_DEFINED__ */


#ifndef __VsMethodTipWindow_FWD_DEFINED__
#define __VsMethodTipWindow_FWD_DEFINED__

#ifdef __cplusplus
typedef class VsMethodTipWindow VsMethodTipWindow;
#else
typedef struct VsMethodTipWindow VsMethodTipWindow;
#endif /* __cplusplus */

#endif 	/* __VsMethodTipWindow_FWD_DEFINED__ */


#ifndef __VsTextTipWindow_FWD_DEFINED__
#define __VsTextTipWindow_FWD_DEFINED__

#ifdef __cplusplus
typedef class VsTextTipWindow VsTextTipWindow;
#else
typedef struct VsTextTipWindow VsTextTipWindow;
#endif /* __cplusplus */

#endif 	/* __VsTextTipWindow_FWD_DEFINED__ */


#ifndef __VsHiddenTextLayer_FWD_DEFINED__
#define __VsHiddenTextLayer_FWD_DEFINED__

#ifdef __cplusplus
typedef class VsHiddenTextLayer VsHiddenTextLayer;
#else
typedef struct VsHiddenTextLayer VsHiddenTextLayer;
#endif /* __cplusplus */

#endif 	/* __VsHiddenTextLayer_FWD_DEFINED__ */


#ifndef __VsDefaultButtonBarImages_FWD_DEFINED__
#define __VsDefaultButtonBarImages_FWD_DEFINED__

#ifdef __cplusplus
typedef class VsDefaultButtonBarImages VsDefaultButtonBarImages;
#else
typedef struct VsDefaultButtonBarImages VsDefaultButtonBarImages;
#endif /* __cplusplus */

#endif 	/* __VsDefaultButtonBarImages_FWD_DEFINED__ */


#ifndef __VsSyntheticTextLayer_FWD_DEFINED__
#define __VsSyntheticTextLayer_FWD_DEFINED__

#ifdef __cplusplus
typedef class VsSyntheticTextLayer VsSyntheticTextLayer;
#else
typedef struct VsSyntheticTextLayer VsSyntheticTextLayer;
#endif /* __cplusplus */

#endif 	/* __VsSyntheticTextLayer_FWD_DEFINED__ */


#ifndef __VsDiffLayer_FWD_DEFINED__
#define __VsDiffLayer_FWD_DEFINED__

#ifdef __cplusplus
typedef class VsDiffLayer VsDiffLayer;
#else
typedef struct VsDiffLayer VsDiffLayer;
#endif /* __cplusplus */

#endif 	/* __VsDiffLayer_FWD_DEFINED__ */


#ifndef __IVsDynamicTabProvider_FWD_DEFINED__
#define __IVsDynamicTabProvider_FWD_DEFINED__
typedef interface IVsDynamicTabProvider IVsDynamicTabProvider;
#endif 	/* __IVsDynamicTabProvider_FWD_DEFINED__ */


#ifndef __IVsLastChangeTimeProvider_FWD_DEFINED__
#define __IVsLastChangeTimeProvider_FWD_DEFINED__
typedef interface IVsLastChangeTimeProvider IVsLastChangeTimeProvider;
#endif 	/* __IVsLastChangeTimeProvider_FWD_DEFINED__ */


#ifndef __IVsEnumTextSpans_FWD_DEFINED__
#define __IVsEnumTextSpans_FWD_DEFINED__
typedef interface IVsEnumTextSpans IVsEnumTextSpans;
#endif 	/* __IVsEnumTextSpans_FWD_DEFINED__ */


#ifndef __IVsTextLayer_FWD_DEFINED__
#define __IVsTextLayer_FWD_DEFINED__
typedef interface IVsTextLayer IVsTextLayer;
#endif 	/* __IVsTextLayer_FWD_DEFINED__ */


#ifndef __IVsSyntheticRegion_FWD_DEFINED__
#define __IVsSyntheticRegion_FWD_DEFINED__
typedef interface IVsSyntheticRegion IVsSyntheticRegion;
#endif 	/* __IVsSyntheticRegion_FWD_DEFINED__ */


#ifndef __IVsEnumSyntheticRegions_FWD_DEFINED__
#define __IVsEnumSyntheticRegions_FWD_DEFINED__
typedef interface IVsEnumSyntheticRegions IVsEnumSyntheticRegions;
#endif 	/* __IVsEnumSyntheticRegions_FWD_DEFINED__ */


#ifndef __IVsSyntheticTextClient_FWD_DEFINED__
#define __IVsSyntheticTextClient_FWD_DEFINED__
typedef interface IVsSyntheticTextClient IVsSyntheticTextClient;
#endif 	/* __IVsSyntheticTextClient_FWD_DEFINED__ */


#ifndef __IVsSyntheticTextSession_FWD_DEFINED__
#define __IVsSyntheticTextSession_FWD_DEFINED__
typedef interface IVsSyntheticTextSession IVsSyntheticTextSession;
#endif 	/* __IVsSyntheticTextSession_FWD_DEFINED__ */


#ifndef __IVsSyntheticTextManager_FWD_DEFINED__
#define __IVsSyntheticTextManager_FWD_DEFINED__
typedef interface IVsSyntheticTextManager IVsSyntheticTextManager;
#endif 	/* __IVsSyntheticTextManager_FWD_DEFINED__ */


#ifndef __IVsTextHidingLayerModule_FWD_DEFINED__
#define __IVsTextHidingLayerModule_FWD_DEFINED__
typedef interface IVsTextHidingLayerModule IVsTextHidingLayerModule;
#endif 	/* __IVsTextHidingLayerModule_FWD_DEFINED__ */


#ifndef __IVsHiddenRegion_FWD_DEFINED__
#define __IVsHiddenRegion_FWD_DEFINED__
typedef interface IVsHiddenRegion IVsHiddenRegion;
#endif 	/* __IVsHiddenRegion_FWD_DEFINED__ */


#ifndef __IVsEnumHiddenRegions_FWD_DEFINED__
#define __IVsEnumHiddenRegions_FWD_DEFINED__
typedef interface IVsEnumHiddenRegions IVsEnumHiddenRegions;
#endif 	/* __IVsEnumHiddenRegions_FWD_DEFINED__ */


#ifndef __IVsHiddenTextClient_FWD_DEFINED__
#define __IVsHiddenTextClient_FWD_DEFINED__
typedef interface IVsHiddenTextClient IVsHiddenTextClient;
#endif 	/* __IVsHiddenTextClient_FWD_DEFINED__ */


#ifndef __IVsHiddenTextManager_FWD_DEFINED__
#define __IVsHiddenTextManager_FWD_DEFINED__
typedef interface IVsHiddenTextManager IVsHiddenTextManager;
#endif 	/* __IVsHiddenTextManager_FWD_DEFINED__ */


#ifndef __IVsHiddenTextSession_FWD_DEFINED__
#define __IVsHiddenTextSession_FWD_DEFINED__
typedef interface IVsHiddenTextSession IVsHiddenTextSession;
#endif 	/* __IVsHiddenTextSession_FWD_DEFINED__ */


#ifndef __IVsOutliningSession_FWD_DEFINED__
#define __IVsOutliningSession_FWD_DEFINED__
typedef interface IVsOutliningSession IVsOutliningSession;
#endif 	/* __IVsOutliningSession_FWD_DEFINED__ */


#ifndef __IVsOutliningCapableLanguage_FWD_DEFINED__
#define __IVsOutliningCapableLanguage_FWD_DEFINED__
typedef interface IVsOutliningCapableLanguage IVsOutliningCapableLanguage;
#endif 	/* __IVsOutliningCapableLanguage_FWD_DEFINED__ */


#ifndef __IVsTextLayerEvents_FWD_DEFINED__
#define __IVsTextLayerEvents_FWD_DEFINED__
typedef interface IVsTextLayerEvents IVsTextLayerEvents;
#endif 	/* __IVsTextLayerEvents_FWD_DEFINED__ */


#ifndef __IVsTextStorage_FWD_DEFINED__
#define __IVsTextStorage_FWD_DEFINED__
typedef interface IVsTextStorage IVsTextStorage;
#endif 	/* __IVsTextStorage_FWD_DEFINED__ */


#ifndef __IVsPersistentTextImage_FWD_DEFINED__
#define __IVsPersistentTextImage_FWD_DEFINED__
typedef interface IVsPersistentTextImage IVsPersistentTextImage;
#endif 	/* __IVsPersistentTextImage_FWD_DEFINED__ */


#ifndef __IVsTextStorageColorState_FWD_DEFINED__
#define __IVsTextStorageColorState_FWD_DEFINED__
typedef interface IVsTextStorageColorState IVsTextStorageColorState;
#endif 	/* __IVsTextStorageColorState_FWD_DEFINED__ */


#ifndef __IVsFinalTextChangeCommitEvents_FWD_DEFINED__
#define __IVsFinalTextChangeCommitEvents_FWD_DEFINED__
typedef interface IVsFinalTextChangeCommitEvents IVsFinalTextChangeCommitEvents;
#endif 	/* __IVsFinalTextChangeCommitEvents_FWD_DEFINED__ */


#ifndef __IVsPreliminaryTextChangeCommitEvents_FWD_DEFINED__
#define __IVsPreliminaryTextChangeCommitEvents_FWD_DEFINED__
typedef interface IVsPreliminaryTextChangeCommitEvents IVsPreliminaryTextChangeCommitEvents;
#endif 	/* __IVsPreliminaryTextChangeCommitEvents_FWD_DEFINED__ */


#ifndef __IVsUndoRedoClusterWithCommitEvents_FWD_DEFINED__
#define __IVsUndoRedoClusterWithCommitEvents_FWD_DEFINED__
typedef interface IVsUndoRedoClusterWithCommitEvents IVsUndoRedoClusterWithCommitEvents;
#endif 	/* __IVsUndoRedoClusterWithCommitEvents_FWD_DEFINED__ */


#ifndef __IVsTextImageEvents_FWD_DEFINED__
#define __IVsTextImageEvents_FWD_DEFINED__
typedef interface IVsTextImageEvents IVsTextImageEvents;
#endif 	/* __IVsTextImageEvents_FWD_DEFINED__ */


#ifndef __IVsCommitGestureSink_FWD_DEFINED__
#define __IVsCommitGestureSink_FWD_DEFINED__
typedef interface IVsCommitGestureSink IVsCommitGestureSink;
#endif 	/* __IVsCommitGestureSink_FWD_DEFINED__ */


#ifndef __IVsTextImage_FWD_DEFINED__
#define __IVsTextImage_FWD_DEFINED__
typedef interface IVsTextImage IVsTextImage;
#endif 	/* __IVsTextImage_FWD_DEFINED__ */


#ifndef __IVsEditorGoBackLocations_FWD_DEFINED__
#define __IVsEditorGoBackLocations_FWD_DEFINED__
typedef interface IVsEditorGoBackLocations IVsEditorGoBackLocations;
#endif 	/* __IVsEditorGoBackLocations_FWD_DEFINED__ */


#ifndef __IVsTextReplaceEvents_FWD_DEFINED__
#define __IVsTextReplaceEvents_FWD_DEFINED__
typedef interface IVsTextReplaceEvents IVsTextReplaceEvents;
#endif 	/* __IVsTextReplaceEvents_FWD_DEFINED__ */


#ifndef __IVsFormatFilterProvider_FWD_DEFINED__
#define __IVsFormatFilterProvider_FWD_DEFINED__
typedef interface IVsFormatFilterProvider IVsFormatFilterProvider;
#endif 	/* __IVsFormatFilterProvider_FWD_DEFINED__ */


#ifndef __IVsNavigableLocationResolver_FWD_DEFINED__
#define __IVsNavigableLocationResolver_FWD_DEFINED__
typedef interface IVsNavigableLocationResolver IVsNavigableLocationResolver;
#endif 	/* __IVsNavigableLocationResolver_FWD_DEFINED__ */


#ifndef __IVsLanguageTextOps_FWD_DEFINED__
#define __IVsLanguageTextOps_FWD_DEFINED__
typedef interface IVsLanguageTextOps IVsLanguageTextOps;
#endif 	/* __IVsLanguageTextOps_FWD_DEFINED__ */


#ifndef __IVsLanguageClipboardOps_FWD_DEFINED__
#define __IVsLanguageClipboardOps_FWD_DEFINED__
typedef interface IVsLanguageClipboardOps IVsLanguageClipboardOps;
#endif 	/* __IVsLanguageClipboardOps_FWD_DEFINED__ */


#ifndef __IVsQueryLineChangeCommit_FWD_DEFINED__
#define __IVsQueryLineChangeCommit_FWD_DEFINED__
typedef interface IVsQueryLineChangeCommit IVsQueryLineChangeCommit;
#endif 	/* __IVsQueryLineChangeCommit_FWD_DEFINED__ */


#ifndef __IVsCodeWindowEvents_FWD_DEFINED__
#define __IVsCodeWindowEvents_FWD_DEFINED__
typedef interface IVsCodeWindowEvents IVsCodeWindowEvents;
#endif 	/* __IVsCodeWindowEvents_FWD_DEFINED__ */


#ifndef __IVsFileBackup_FWD_DEFINED__
#define __IVsFileBackup_FWD_DEFINED__
typedef interface IVsFileBackup IVsFileBackup;
#endif 	/* __IVsFileBackup_FWD_DEFINED__ */


#ifndef __IVsLineAttributes_FWD_DEFINED__
#define __IVsLineAttributes_FWD_DEFINED__
typedef interface IVsLineAttributes IVsLineAttributes;
#endif 	/* __IVsLineAttributes_FWD_DEFINED__ */


/* header files for imported files */
#include "docobj.h"
#include "context.h"

#ifdef __cplusplus
extern "C"{
#endif 


/* interface __MIDL_itf_textmgr_0000_0000 */
/* [local] */ 

#ifndef TEXTSPAN_DEFINED
#define TEXTSPAN_DEFINED
typedef long ViewCol;

typedef long CharIndex;

typedef __int64 TEXTADDR;

struct _TextAddress
    {
    CharIndex index;
    long line;
    } ;
typedef struct _TextAddress TextAddress;

struct _TextSpan
    {
    CharIndex iStartIndex;
    long iStartLine;
    CharIndex iEndIndex;
    long iEndLine;
    } ;
typedef struct _TextSpan TextSpan;

#endif // TEXTSPAN_DEFINED
#ifndef __INDENTSTYLE__
#define __INDENTSTYLE__
typedef /* [uuid] */  DECLSPEC_UUID("BCCEBE05-D29C-11D2-AABD-00C04F688DDE") 
enum _vsIndentStyle
    {	vsIndentStyleNone	= 0,
	vsIndentStyleDefault	= ( vsIndentStyleNone + 1 ) ,
	vsIndentStyleSmart	= ( vsIndentStyleDefault + 1 ) 
    } 	vsIndentStyle;

#endif // __INDENTSTYLE__
#pragma once









































































extern RPC_IF_HANDLE __MIDL_itf_textmgr_0000_0000_v0_0_c_ifspec;
extern RPC_IF_HANDLE __MIDL_itf_textmgr_0000_0000_v0_0_s_ifspec;


#ifndef __TextManagerInternal_LIBRARY_DEFINED__
#define __TextManagerInternal_LIBRARY_DEFINED__

/* library TextManagerInternal */
/* [version][uuid] */ 


enum TextBufferErrors
    {	BUFFER_E_READONLY	= ( ( 0x80000000 | ( 4 << 16 )  )  | 0x1001 ) ,
	BUFFER_E_READONLY_REGION	= ( ( 0x80000000 | ( 4 << 16 )  )  | 0x1002 ) ,
	BUFFER_E_TIMEOUT	= ( ( 0x80000000 | ( 4 << 16 )  )  | 0x1003 ) ,
	BUFFER_E_LOCKED	= ( ( 0x80000000 | ( 4 << 16 )  )  | 0x1004 ) ,
	BUFFER_E_SCC_READONLY	= ( ( 0x80000000 | ( 4 << 16 )  )  | 0x1005 ) ,
	BUFFER_E_DEST_TOO_SMALL	= ( ( 0x80000000 | ( 4 << 16 )  )  | 0x1006 ) ,
	MARKER_E_INVALIDATED	= ( ( 0x80000000 | ( 4 << 16 )  )  | 0x1007 ) ,
	MARKER_E_BUFFERDESTROYED	= ( ( 0x80000000 | ( 4 << 16 )  )  | 0x1008 ) 
    } ;

enum TextViewErrors
    {	VIEW_E_NO_BOOKMARKS	= ( ( 0x80000000 | ( 4 << 16 )  )  | 0x2001 ) ,
	VIEW_E_NO_DEBUGGER	= ( ( 0x80000000 | ( 4 << 16 )  )  | 0x2002 ) ,
	VIEW_E_USERREADONLY	= ( ( 0x80000000 | ( 4 << 16 )  )  | 0x2004 ) ,
	VIEW_E_LOCATION_HIDDEN	= ( ( 0x80000000 | ( 4 << 16 )  )  | 0x2005 ) 
    } ;

enum LinkedUndoErrors
    {	LINKEDUNDO_E_INTERVENING_ACTION	= ( ( 0x80000000 | ( 4 << 16 )  )  | 0x3001 ) 
    } ;

enum UndoErrors
    {	UNDO_E_CLIENTABORT	= ( ( 0x80000000 | ( 4 << 16 )  )  | 0x4001 ) 
    } ;

enum TipSuccesses
    {	TIP_S_ONLYIFNOMARKER	= ( ( 4 << 16 )  | 0x4001 ) 
    } ;
#include <stdidcmd.h>
#include <vsshlids.h>

enum __MIDL___MIDL_itf_textmgr_0001_0026_0001
    {	MAX_FILE_TYPE	= 24
    } ;

enum __MIDL___MIDL_itf_textmgr_0001_0026_0002
    {	MAX_EDITOR_TAB_SIZE	= 60
    } ;
typedef struct _LANGPREFERENCES
    {
    CHAR szFileType[ 24 ];
    unsigned int fShowCompletion;
    unsigned int fShowSmartIndent;
    unsigned int fHideAdvancedAutoListMembers;
    unsigned int uTabSize;
    unsigned int uIndentSize;
    unsigned int fInsertTabs;
    vsIndentStyle IndentStyle;
    unsigned int fAutoListMembers;
    unsigned int fAutoListParams;
    unsigned int fVirtualSpace;
    unsigned int fWordWrap;
    unsigned int fTwoWayTreeview;
    unsigned int fHotURLs;
    unsigned int fDropdownBar;
    unsigned int fLineNumbers;
    GUID guidLang;
    } 	LANGPREFERENCES;

DEFINE_GUID(DEFAULT_FILE_TYPE, 0x8239bec4, 0xee87, 0x11d0, 0x8c, 0x98, 0x0, 0xc0, 0x4f, 0xc2, 0xab, 0x22);
DEFINE_GUID(ALL_FILE_TYPES, 0x2149a66e, 0xf261, 0x11d0, 0x8c, 0x9c, 0x0, 0xc0, 0x4f, 0xc2, 0xab, 0x22);
typedef struct _VIEWPREFERENCES
    {
    unsigned int fVisibleWhitespace;
    unsigned int fSelectionMargin;
    unsigned int fAutoDelimiterHighlight;
    unsigned int fGoToAnchorAfterEscape;
    unsigned int fDragDropEditing;
    unsigned int fUndoCaretMovements;
    unsigned int fOvertype;
    unsigned int fDragDropMove;
    unsigned int fWidgetMargin;
    } 	VIEWPREFERENCES;

typedef struct _FRAMEPREFERENCES
    {
    unsigned int fHorzScrollbar;
    unsigned int fVertScrollbar;
    } 	FRAMEPREFERENCES;

typedef struct _FONTCOLORPREFERENCES
    {
    const GUID *pguidColorService;
    IUnknown *pColorTable;
    HFONT hRegularViewFont;
    HFONT hBoldViewFont;
    const GUID *pguidFontCategory;
    const GUID *pguidColorCategory;
    } 	FONTCOLORPREFERENCES;

typedef 
enum __VSEDITPROPID
    {	VSEDITPROPID_LAST	= -0x1,
	VSEDITPROPID_ViewComposite_AllCodeWindowDefaults	= -0x10001,
	VSEDITPROPID_ViewGeneral_FontCategory	= -0x12005,
	VSEDITPROPID_ViewGeneral_ColorCategory	= -0x12006,
	VSEDITPROPID_ViewGlobalOpt_VisibleWhitespace	= -0x20001,
	VSEDITPROPID_ViewGlobalOpt_Overtype	= -0x20002,
	VSEDITPROPID_ViewGlobalOpt_SelectionMargin	= -0x20003,
	VSEDITPROPID_ViewGlobalOpt_AutoScrollCaretOnTextEntry	= -0x20010,
	VSEDITPROPID_ViewLangOpt_WordWrap	= -0x30001,
	VSEDITPROPID_ViewLangOpt_VirtualSpace	= -0x30006,
	VSEDITPROPID_ViewLangOpt_RawTextDisplay	= -0x30009,
	VSEDITPROPID_FrameOpt_SuppressResizeWidget	= -0x50003,
	VSEDITPROPID_ProgLang_DebuggerLanguages	= -0x60003,
	VSEDITPROPID_ProgLang_SuppressMultipleCodeWindows	= -0x60006,
	VSEDITPROPID_ProgLang_SupportCF_HTML	= -0x60008,
	VSEDITPROPID_FIRST	= -0x7fffffff
    } 	VSEDITPROPID;

DEFINE_GUID(GUID_EditPropCategory_View_MasterSettings, 0xd1756e7c, 0xb7fd, 0x49a8, 0xb4, 0x8e, 0x87, 0xb1, 0x4a, 0x55, 0x65, 0x5a);
DEFINE_GUID(GUID_EditPropCategory_TextMgr_Global, 0x6bfb60a2, 0x48d8, 0x424e, 0x81, 0xa2, 0x4, 0xa, 0xca, 0xb, 0x1f, 0x68);
typedef 
enum _TextSelMode
    {	SM_STREAM	= 0,
	SM_BOX	= ( SM_STREAM + 1 ) 
    } 	TextSelMode;

typedef 
enum _TextViewInitFlags
    {	VIF_DEFAULT	= 0,
	VIF_HSCROLL	= 0x100000,
	VIF_VSCROLL	= 0x200000,
	VIF_UPDATE_STATUS_BAR	= 0x400000,
	VIF_SET_WIDGET_MARGIN	= 0x1,
	VIF_SET_SELECTION_MARGIN	= 0x2,
	VIF_SET_VIRTUAL_SPACE	= 0x4,
	VIF_SET_INDENT_MODE	= 0x8,
	VIF_SET_STREAM_SEL_MODE	= 0x10,
	VIF_SET_VISIBLE_WHITESPACE	= 0x20,
	VIF_SET_OVERTYPE	= 0x40,
	VIF_SET_DRAGDROPMOVE	= 0x80,
	VIF_SET_HOTURLS	= 0x100
    } 	TextViewInitFlags;

typedef struct _INITVIEW
    {
    unsigned int fVirtualSpace;
    unsigned int fStreamSelMode;
    unsigned int fOvertype;
    unsigned int fVisibleWhitespace;
    unsigned int fWidgetMargin;
    unsigned int fSelectionMargin;
    unsigned int fDragDropMove;
    unsigned int fHotURLs;
    vsIndentStyle IndentStyle;
    } 	INITVIEW;

typedef 
enum _CompletionStatusFlags
    {	UCS_NAMESCHANGED	= 0x1,
	UCS_EXTENTCHANGED	= 0x2,
	UCS_FLAGSCHANGED	= 0x4,
	UCS_COMPLETEWORD	= 0x8,
	CSF_CUSTOMMATCHING	= 0x4,
	CSF_CUSTOMCOMMIT	= 0x8,
	CSF_HAVEDESCRIPTIONS	= 0x10,
	CSF_INITIALEXTENTKNOWN	= 0x20,
	CSF_NOCASEMATCHING	= 0x40,
	CSF_NOCASEUNIQUENESS	= 0x80,
	GBM_SELECT	= 0x1,
	GBM_UNIQUE	= 0x2
    } 	UpdateCompletionFlags;

typedef 
enum _TipWindowFlags
    {	UTW_CONTEXTCHANGED	= 0x1,
	UTW_CONTENTCHANGED	= 0x2,
	UTW_DISMISS	= 0x4
    } 	TipWindowFlags;

typedef 
enum _TipPosPreference
    {	TPP_ABOVE	= 0,
	TPP_BELOW	= ( TPP_ABOVE + 1 ) ,
	TPP_LEFT	= ( TPP_BELOW + 1 ) ,
	TPP_RIGHT	= ( TPP_LEFT + 1 ) ,
	TPP_DOCKED	= ( TPP_RIGHT + 1 ) 
    } 	TipPosPreference;

typedef struct _tipsizedata
    {
    SIZE size;
    TipPosPreference dwPosition;
    } 	TIPSIZEDATA;

typedef 
enum _MethodTextType
    {	MTT_TYPEPREFIX	= 0,
	MTT_TYPEPOSTFIX	= ( MTT_TYPEPREFIX + 1 ) ,
	MTT_NAME	= ( MTT_TYPEPOSTFIX + 1 ) ,
	MTT_DESCRIPTION	= ( MTT_NAME + 1 ) ,
	MTT_OPENBRACKET	= ( MTT_DESCRIPTION + 1 ) ,
	MTT_CLOSEBRACKET	= ( MTT_OPENBRACKET + 1 ) ,
	MTT_DELIMITER	= ( MTT_CLOSEBRACKET + 1 ) 
    } 	MethodTextType;

typedef 
enum _ParameterTextType
    {	PTT_DECLARATION	= 0,
	PTT_NAME	= ( PTT_DECLARATION + 1 ) ,
	PTT_DESCRIPTION	= ( PTT_NAME + 1 ) 
    } 	ParameterTextType;

typedef 
enum _GetSelectedAtomFlags
    {	gsaDefault	= 0
    } 	GetSelectedAtomFlags;

typedef 
enum _SELECTIONSTATEFLAGS
    {	ssfDefault	= 0,
	ssfColumn	= 1
    } 	SELECTIONSTATEFLAGS;

typedef struct _SELECTIONSTATE
    {
    ViewCol iAnchorCol;
    long iAnchorLine;
    ViewCol iActiveCol;
    long iActiveLine;
    DWORD dwFlags;
    DWORD dwReserved;
    } 	SELECTIONSTATE;

typedef 
enum _RelativeSelectionStateFlags
    {	RSS_DEFAULT	= 0,
	RSS_TOP_LAYER	= 0,
	RSS_BUFFER	= 0x1,
	RSS_RELATIVE_LAYER	= 0x2,
	RSS_TRACKSEL	= 0x10,
	RSS_SET_WITHOUT_SCROLLING	= 0x1000,
	RSS_SET_TO_NEAREST_LOCAL	= 0x2000,
	rssDefault	= RSS_DEFAULT,
	rssTopLayer	= RSS_TOP_LAYER,
	rssBuffer	= RSS_BUFFER,
	rssRelativeLayer	= RSS_RELATIVE_LAYER,
	rssTrackSel	= RSS_TRACKSEL
    } 	RelativeSelectionStateFlags;

typedef 
enum _wordextflags
    {	WORDEXT_MOVETYPE_MASK	= 0x3,
	WORDEXT_CURRENT	= 0,
	WORDEXT_PREVIOUS	= 0x1,
	WORDEXT_NEXT	= 0x2,
	WORDEXT_NEAREST	= 0x3,
	WORDEXT_FINDWORD	= 0,
	WORDEXT_FINDTOKEN	= 0x4,
	WORDEXT_FINDEXPRESSION	= 0x8
    } 	WORDEXTFLAGS;

typedef 
enum _LanguageContextHint
    {	LCH_DEFAULT	= 0,
	LCH_F1_HELP	= 0x1
    } 	LanguageContextHint;

typedef 
enum _TextViewAction
    {	TVA_SETCARETPOS	= 0,
	TVA_CENTERLINES	= ( TVA_SETCARETPOS + 1 ) 
    } 	TextViewAction;

typedef 
enum _COLORINDEX
    {	CI_USERTEXT_FG	= 0,
	CI_USERTEXT_BK	= ( CI_USERTEXT_FG + 1 ) ,
	CI_FIRSTFIXEDCOLOR	= ( CI_USERTEXT_BK + 1 ) ,
	CI_BLACK	= CI_FIRSTFIXEDCOLOR,
	CI_WHITE	= ( CI_BLACK + 1 ) ,
	CI_MAROON	= ( CI_WHITE + 1 ) ,
	CI_DARKGREEN	= ( CI_MAROON + 1 ) ,
	CI_BROWN	= ( CI_DARKGREEN + 1 ) ,
	CI_DARKBLUE	= ( CI_BROWN + 1 ) ,
	CI_PURPLE	= ( CI_DARKBLUE + 1 ) ,
	CI_AQUAMARINE	= ( CI_PURPLE + 1 ) ,
	CI_LIGHTGRAY	= ( CI_AQUAMARINE + 1 ) ,
	CI_DARKGRAY	= ( CI_LIGHTGRAY + 1 ) ,
	CI_RED	= ( CI_DARKGRAY + 1 ) ,
	CI_GREEN	= ( CI_RED + 1 ) ,
	CI_YELLOW	= ( CI_GREEN + 1 ) ,
	CI_BLUE	= ( CI_YELLOW + 1 ) ,
	CI_MAGENTA	= ( CI_BLUE + 1 ) ,
	CI_CYAN	= ( CI_MAGENTA + 1 ) ,
	CI_LASTFIXEDCOLOR	= CI_CYAN,
	CI_SYSSEL_FG	= ( CI_LASTFIXEDCOLOR + 1 ) ,
	CI_SYSSEL_BK	= ( CI_SYSSEL_FG + 1 ) ,
	CI_SYSINACTSEL_FG	= ( CI_SYSSEL_BK + 1 ) ,
	CI_SYSINACTSEL_BK	= ( CI_SYSINACTSEL_FG + 1 ) ,
	CI_SYSWIDGETMGN_BK	= ( CI_SYSINACTSEL_BK + 1 ) ,
	CI_SYSPLAINTEXT_FG	= ( CI_SYSWIDGETMGN_BK + 1 ) ,
	CI_SYSPLAINTEXT_BK	= ( CI_SYSPLAINTEXT_FG + 1 ) ,
	CI_PALETTESIZE	= ( CI_SYSPLAINTEXT_BK + 1 ) ,
	CI_FORBIDCUSTOMIZATION	= ( CI_PALETTESIZE + 1 ) ,
	CI_SYSTEXT_FG	= CI_USERTEXT_FG,
	CI_SYSTEXT_BK	= CI_USERTEXT_BK
    } 	COLORINDEX;

typedef 
enum _LINESTYLE
    {	LI_NONE	= 0,
	LI_SOLID	= 1,
	LI_SQUIGGLY	= 2,
	LI_HATCH	= 3,
	LI_DOTTED	= 4
    } 	LINESTYLE;

typedef 
enum _GLYPHDRAWFLAGS
    {	GDF_DEFAULT	= 0,
	GDF_MULTILINE	= 0x1,
	GDF_TOPEDGE	= 0x2,
	GDF_BOTTOMEDGE	= 0x4
    } 	GLYPHDRAWFLAGS;

typedef 
enum _MARKERVISUAL
    {	MV_GLYPH	= 0x1,
	MV_COLOR_ALWAYS	= 0x2,
	MV_COLOR_LINE_IF_NO_MARGIN	= 0x4,
	MV_LINE	= 0x8,
	MV_TIP_FOR_BODY	= 0x10,
	MV_TIP_FOR_GLYPH	= 0x20,
	MV_SEL_MARGIN_GLYPH	= 0x40,
	MV_FORCE_INVISIBLE	= 0x80,
	MV_BORDER	= 0x100,
	MV_GLYPH_HOVER_CURSOR	= 0x200,
	MV_DRAGGABLE_GLYPH	= 0x400,
	MV_MULTILINE_GLYPH	= 0x800,
	MV_CONTEXT_CONTRIBUTION_FOR_BODY	= 0x1000,
	MV_COLOR_SPAN_IF_ZERO_LENGTH	= 0x2000
    } 	MARKERVISUAL;

typedef 
enum _FONTFLAGS
    {	FF_DEFAULT	= 0,
	FF_BOLD	= 0x1,
	FF_STRIKETHROUGH	= 0x2
    } 	FONTFLAGS;

typedef 
enum _markerbehaviorflags
    {	MB_DEFAULT	= 0,
	MB_LINESPAN	= 0x1,
	MB_LEFTEDGE_LEFTTRACK	= 0x2,
	MB_RIGHTEDGE_RIGHTTRACK	= 0x4,
	MB_MULTILINESPAN	= 0x8,
	MB_TRACK_EDIT_ON_RELOAD	= 0x100
    } 	MARKERBEHAVIORFLAGS;

typedef 
enum _DEFAULTITEMS
    {	COLITEM_TEXT	= 0,
	COLITEM_KEYWORD	= ( COLITEM_TEXT + 1 ) ,
	COLITEM_COMMENT	= ( COLITEM_KEYWORD + 1 ) ,
	COLITEM_IDENTIFIER	= ( COLITEM_COMMENT + 1 ) ,
	COLITEM_STRING	= ( COLITEM_IDENTIFIER + 1 ) ,
	COLITEM_NUMBER	= ( COLITEM_STRING + 1 ) ,
	DEFAULT_ITEM_COUNT	= ( COLITEM_NUMBER + 1 ) 
    } 	DEFAULTITEMS;

typedef 
enum _COLORIZER_ATTRIBUTE
    {	COLOR_INDEX_MASK	= 0xff,
	COLOR_MARKER_MASK	= 0x3f00,
	LINE_MARKER_MASK	= 0xfc000,
	PRIVATE_CLIENT_MASK	= 0xf00000,
	HUMAN_TEXT_ATTR	= 0x1000000,
	SEPARATOR_AFTER_ATTR	= 0x2000000,
	PRIVATE_EDITOR_MASK	= 0xfc000000
    } 	COLORIZER_ATTRIBUTE;

extern const __declspec(selectany) GUID GUID_VsLang_Support_CF_HTML = { 0x27e97702, 0x589e, 0x11d2, { 0x82, 0x33, 0x0, 0x80, 0xc7, 0x47, 0xd9, 0xa0 } };
typedef 
enum _resolvenameflags
    {	RNF_BREAKPOINT	= 0x1
    } 	RESOLVENAMEFLAGS;

typedef 
enum _markertype
    {	MARKER_INVISIBLE	= 0,
	MARKER_READONLY	= ( MARKER_INVISIBLE + 1 ) ,
	MARKER_SHORTCUT	= ( MARKER_READONLY + 1 ) ,
	MARKER_BOOKMARK	= ( MARKER_SHORTCUT + 1 ) ,
	MARKER_COMPILE_ERROR	= ( MARKER_BOOKMARK + 1 ) ,
	MARKER_CODESENSE_ERROR	= ( MARKER_COMPILE_ERROR + 1 ) ,
	MARKER_OTHER_ERROR	= ( MARKER_CODESENSE_ERROR + 1 ) ,
	MARKER_REGION_COLLAPSED	= ( MARKER_OTHER_ERROR + 1 ) ,
	MARKER_REGION_EXPANDED	= ( MARKER_REGION_COLLAPSED + 1 ) ,
	MARKER_LIST_LOCATION	= ( MARKER_REGION_EXPANDED + 1 ) ,
	DEF_MARKER_COUNT	= ( MARKER_LIST_LOCATION + 1 ) ,
	MARKER_IDENTERROR	= MARKER_COMPILE_ERROR,
	MARKER_SYNTAXERROR	= MARKER_COMPILE_ERROR
    } 	MARKERTYPE;

typedef 
enum _enummarkerflags
    {	EM_DEFAULT	= 0,
	EM_ENTIREBUFFER	= 0x1,
	EM_ALLTYPES	= 0x2,
	EM_SORTDESCENDING	= 0x4,
	EM_SORTPRIORITY	= 0x8,
	EM_GLYPHINSPAN	= 0x10,
	EM_INCLUDEINVISIBLE	= 0x20,
	EM_CONTAINED	= 0x40
    } 	ENUMMARKERFLAGS;

typedef 
enum _findmarkerflags
    {	FM_FORWARD	= 0,
	FM_BACKWARD	= 0x1
    } 	FINDMARKERFLAGS;

typedef 
enum _bufferstateflags
    {	BSF_USER_READONLY	= 0x1,
	BSF_FILESYS_READONLY	= 0x2,
	BSF_MODIFIED	= 0x4,
	BSF_SUPPRESS_UI	= 0x8,
	BSF_RESERVED	= ~(( ( BSF_USER_READONLY | BSF_FILESYS_READONLY )  | BSF_MODIFIED ) )
    } 	BUFFERSTATEFLAGS;

typedef 
enum _BufferLockFlags
    {	BLF_READ	= 0,
	BLF_READ_AND_WRITE	= 0x1
    } 	BufferLockFlags;

typedef 
enum _textbufferinitflags
    {	BIF_DEFAULT	= 0,
	BIF_CUSTOMSTORAGE	= 0x1,
	BIF_INITTEXT	= 0x2
    } 	TextBufferInitFlags;

typedef /* [public][public] */ struct __MIDL___MIDL_itf_textmgr_0001_0060_0001
    {
    DWORD dwFlags;
    IUnknown *pStorage;
    const WCHAR *pszText;
    long iLength;
    } 	BUFFERINITEX;

typedef 
enum _CHARFMT
    {	CHARFMT_UNI	= 0,
	CHARFMT_UNI_SWAP	= ( CHARFMT_UNI + 1 ) ,
	CHARFMT_MBCS	= ( CHARFMT_UNI_SWAP + 1 ) ,
	CHARFMT_UTF8	= ( CHARFMT_MBCS + 1 ) 
    } 	CHARFMT;

typedef 
enum _EOLTYPE
    {	eolCRLF	= 0,
	eolCR	= ( eolCRLF + 1 ) ,
	eolLF	= ( eolCR + 1 ) ,
	eolUNI_LINESEP	= ( eolLF + 1 ) ,
	eolUNI_PARASEP	= ( eolUNI_LINESEP + 1 ) ,
	eolEOF	= ( eolUNI_PARASEP + 1 ) ,
	eolNONE	= ( eolEOF + 1 ) ,
	MAX_EOLTYPES	= ( eolNONE + 1 ) 
    } 	EOLTYPE;

typedef 
enum _LINEDATAEXFLAGS
    {	ldfDefault	= 0,
	ldfMarkers	= 0x1,
	ldfNoReadLock	= 0x2
    } 	LINEDATAEXFLAGS;

typedef struct _linedata
    {
    long iLength;
    const WCHAR *pszText;
    const ULONG *pAttributes;
    EOLTYPE iEolType;
    BOOL fMarkers;
    } 	LINEDATA;

typedef struct _tag_AtomicText AtomicText;

struct _tag_AtomicText
    {
    AtomicText *pNext;
    CharIndex iIndex;
    const WCHAR *pszAtomicText;
    IUnknown *punkAtom;
    } ;
typedef struct _linedataex
    {
    long iLength;
    const WCHAR *pszText;
    EOLTYPE iEolType;
    const ULONG *pAttributes;
    USHORT dwFlags;
    USHORT dwReserved;
    AtomicText *pAtomicTextChain;
    } 	LINEDATAEX;

typedef 
enum _paintingrestrictions
    {	PR_NONE	= 0,
	PR_NO_MARGIN	= ( PR_NONE + 1 ) 
    } 	PAINTINGRESTRICTIONS;

typedef struct _tag_MARKERDATA MARKERDATA;

struct _tag_MARKERDATA
    {
    long iTopLine;
    long iBottomLine;
    long iCount;
    IVsTextLayerMarker **rgpMarker;
    BOOL *rgfLineMarked;
    IVsTextLayer *pLayer;
    MARKERDATA *pNext;
    } ;
typedef 
enum _GLDE_FLAGS
    {	gldeDefault	= 0,
	gldeSubset	= 0x1,
	gldeColorizerState	= 0x2,
	gldeSyntaxAttr	= 0x4,
	gldeMarkerAttr	= 0x8,
	gldeFullAttr	= ( gldeSyntaxAttr | gldeMarkerAttr ) ,
	gldeNoReadLock	= 0x10,
	gldeNoWidgetMargin	= 0x20
    } 	GLDE_FLAGS;

typedef 
enum _REPLACE_TEXT_FLAGS
    {	rtfDefault	= 0,
	rtfKeepMarkers	= 0x1,
	rtfDoNotNormalizeNewlines	= 0x2,
	rtfNormalizeTabsAndSpaces	= 0x4,
	rtfSyntacticReformat	= 0x8,
	rtfClientSuppressFormatting	= 0x10,
	rtfInterimText	= 0x20
    } 	REPLACE_TEXT_FLAGS;

typedef struct _TextLineChange
    {
    CharIndex iStartIndex;
    long iStartLine;
    CharIndex iOldEndIndex;
    long iOldEndLine;
    CharIndex iNewEndIndex;
    long iNewEndLine;
    } 	TextLineChange;

typedef 
enum _MarkerCommandValues
    {	mcvFirstContextMenuCommand	= 0,
	mcvLastContextMenuCommand	= 0x9,
	mcvGlyphSingleClickCommand	= 0x101,
	mcvBodyDoubleClickCommand	= 0x102,
	mcvGlyphDoubleClickCommand	= 0x103
    } 	MarkerCommandValues;

typedef 
enum _CUSTOM_DROP_HANDLER_FLAGS
    {	CDH_DEFAULT	= 0,
	CDH_OVER_SELF	= 1,
	CDH_INVALID_DEST	= 2
    } 	CUSTOM_DROP_HANDLER_FLAGS;

typedef 
enum _TextFindMode
    {	TFIND_BACKWARDS	= 1,
	TFIND_MATCH_CASE	= 2,
	TFIND_WHOLE_WORD	= 4,
	TFIND_REG_EXPR	= 8,
	TFIND_WRAP_SEARCH	= 16,
	TFIND_IN_SELECTION	= 32,
	TFIND_NEW_SEARCH	= 64
    } 	TextFindMode;

typedef 
enum _splitnotifycode
    {	SNC_BEGINSIZE	= 0,
	SNC_ENDSIZE	= ( SNC_BEGINSIZE + 1 ) ,
	SNC_ANCESTORDETACHED	= ( SNC_ENDSIZE + 1 ) ,
	SNC_ANCESTORATTACHED	= ( SNC_ANCESTORDETACHED + 1 ) ,
	SNC_LBUTTONDBLCLK	= ( SNC_ANCESTORATTACHED + 1 ) 
    } 	SPLITNOTIFYCODE;

typedef 
enum _splitflags
    {	SF_HORIZONTAL	= 0,
	SF_VERTICAL	= 0x1,
	SF_MAKESECONDARY	= 0,
	SF_MAKEPRIMARY	= 0x2,
	SF_ADJUSTABLE	= 0,
	SF_FIXED	= 0x4
    } 	SPLITFLAGS;

typedef 
enum _paneflags
    {	PF_SUNKEN	= 0,
	PF_RAISED	= 0x1,
	PF_ETCHED	= 0x2,
	PF_BUMP	= 0x3,
	PF_SHALLOW	= 0x4,
	PF_BORDERMASK	= 0x7,
	PF_BORDER	= 0x8,
	PF_ELASTICX	= 0x10,
	PF_ELASTICY	= 0x20,
	PF_NOFOCUS	= 0x40
    } 	PANEFLAGS;

typedef 
enum _paneevents
    {	PE_GOTFOCUS	= 1,
	PE_CLOSED	= ( PE_GOTFOCUS + 1 ) ,
	PE_FLAGSCHANGED	= ( PE_CLOSED + 1 ) ,
	PE_DETACHED	= ( PE_FLAGSCHANGED + 1 ) ,
	PE_FOCUS_TO_EDIT_PANE	= ( PE_DETACHED + 1 ) 
    } 	PANEEVENTS;

typedef 
enum _panetype
    {	GP_FOCUS	= 1,
	GP_NEXT	= ( GP_FOCUS + 1 ) ,
	GP_PREV	= ( GP_NEXT + 1 ) 
    } 	PANETYPE;

typedef 
enum _READONLYSTATUS
    {	ROSTATUS_NotReadOnly	= 0,
	ROSTATUS_ReadOnly	= 1,
	ROSTATUS_Unknown	= -1
    } 	READONLYSTATUS;


enum DROPDOWNFONTATTR
    {	FONTATTR_PLAIN	= 0,
	FONTATTR_BOLD	= 1,
	FONTATTR_ITALIC	= 2,
	FONTATTR_UNDERLINE	= 4,
	FONTATTR_GRAY	= 8
    } ;

enum DROPDOWNENTRYTYPE
    {	ENTRY_TEXT	= 1,
	ENTRY_ATTR	= 2,
	ENTRY_IMAGE	= 4
    } ;
typedef /* [public][public] */ 
enum __MIDL___MIDL_itf_textmgr_0001_0099_0001
    {	MOVESELECTION_REL_CHARACTER	= 1,
	MOVESELECTION_REL_WORD	= ( MOVESELECTION_REL_CHARACTER + 1 ) ,
	MOVESELECTION_REL_LINE	= ( MOVESELECTION_REL_WORD + 1 ) ,
	MOVESELECTION_REL_PAGE	= ( MOVESELECTION_REL_LINE + 1 ) 
    } 	MOVESELECTION_REL_TYPE;

typedef /* [public][public] */ 
enum __MIDL___MIDL_itf_textmgr_0001_0099_0002
    {	MOVESELECTION_ABS_FIRSTCOLUMN	= 1,
	MOVESELECTION_ABS_FIRSTTEXTCOLUMN	= ( MOVESELECTION_ABS_FIRSTCOLUMN + 1 ) ,
	MOVESELECTION_ABS_LASTCOLUMN	= ( MOVESELECTION_ABS_FIRSTTEXTCOLUMN + 1 ) ,
	MOVESELECTION_ABS_FIRSTLINE	= ( MOVESELECTION_ABS_LASTCOLUMN + 1 ) ,
	MOVESELECTION_ABS_LASTLINE	= ( MOVESELECTION_ABS_FIRSTLINE + 1 ) 
    } 	MOVESELECTION_ABS_TYPE;

typedef /* [public][public] */ 
enum __MIDL___MIDL_itf_textmgr_0001_0099_0003
    {	CASESELECTION_UPPER	= 1,
	CASESELECTION_LOWER	= ( CASESELECTION_UPPER + 1 ) ,
	CASESELECTION_TITLE	= ( CASESELECTION_LOWER + 1 ) 
    } 	CASESELECTION_TYPE;

typedef 
enum _LinkedTransactionFlags
    {	mdtDefault	= 0,
	mdtStrict	= 0x1
    } 	LinkedTransactionFlags;

typedef 
enum _ChangeClusterFlags
    {	CCE_BEFORE_OPEN	= 0x1,
	CCE_BEFORE_CLOSE	= 0x2,
	CCE_AFTER_CLOSE	= 0x4,
	CCE_UNDO	= 0x8,
	CCE_REDO	= 0x10,
	CCE_FLUSH	= 0x20,
	CCE_ABORTED	= 0x100
    } 	ChangeClusterFlags;

#define SID_SVsTextManager CLSID_VsTextManager
#define SID_SVsLanguageFilter IID_IVsTextViewFilter
#define SID_SVsCodeWindowManager IID_IVsCodeWindowManager
#define SID_SVsCodeWindow IID_IVsCodeWindow
#define SID_SVsLinkedUndoTransactionManager IID_IVsLinkedUndoTransactionManager
#define GUID_VsBufferMoniker IID_IVsUserData
extern const __declspec(selectany) GUID GUID_VsBufferIsDiskFile = { 0xd9126592, 0x1473, 0x11d3, { 0xbe, 0xc6, 0x0, 0x80, 0xc7, 0x47, 0xd9, 0xa0 } };
extern const __declspec(selectany) GUID GUID_VsBufferEncoding = {0x212729ac,0xd6bb,0x11d0,{0xae,0x75,0x0,0xc0,0x4f,0xb6,0x80,0x6}};
extern const __declspec(selectany) GUID GUID_VsBufferEncodingVSTFF = { 0x16417f39, 0xa6b7, 0x4c90, { 0x89, 0xfa, 0x77, 0xd, 0x2c, 0x60, 0x44, 0xb }};
extern const __declspec(selectany) GUID GUID_VsBufferEncodingPromptOnLoad = { 0x99ec03f0, 0xc843, 0x4c09, { 0xbe, 0x74, 0xcd, 0xca, 0x51, 0x58, 0xd3, 0x6c }};
extern const __declspec(selectany) GUID GUID_VsBufferDetectCharSet = {0x36358d1f,0xbf7e,0x11d1,{0xb0,0x3a,0x0,0xc0,0x4f,0xb6,0x80,0x6}};
extern const __declspec(selectany) GUID GUID_VsBufferDetectLangSID = { 0x17f375ac, 0xc814, 0x11d1, { 0x88, 0xad, 0x0, 0x0, 0xf8, 0x75, 0x79, 0xd2 } };
extern const __declspec(selectany) GUID GUID_PropertyBrowserSID = { 0xce6ddbba, 0x8d13, 0x11d1, { 0x88, 0x89, 0x0, 0x0, 0xf8, 0x75, 0x79, 0xd2 }};
extern const __declspec(selectany) GUID GUID_UserReadOnlyErrorString = { 0xa3bcfe56, 0xcf1b, 0x11d1, { 0x88, 0xb1, 0x0, 0x0, 0xf8, 0x75, 0x79, 0xd2 }};
extern const __declspec(selectany) GUID GUID_BufferStorage = { 0xd97f167a, 0x638e, 0x11d2, { 0x88, 0xf6, 0x0, 0x0, 0xf8, 0x75, 0x79, 0xd2 } };
extern const __declspec(selectany) GUID CLSID_TextEditorFactory = { 0x8B382828, 0x6202, 0x11d1, { 0x88, 0x70, 0x00, 0x00, 0xF8, 0x75, 0x79, 0xD2 }};
extern const __declspec(selectany) GUID CMDUIGUID_TextEditor = { 0x8B382828, 0x6202, 0x11d1, { 0x88, 0x70, 0x00, 0x00, 0xF8, 0x75, 0x79, 0xD2 }};
struct TabStop
    {
    long *iCol;
    ULONG cEl;
    UINT iRepeatWidth;
    DWORD dwFlags;
    } ;

enum TabStopFlags
    {	USE_DEFAULT_REPEAT	= 0x1
    } ;
typedef 
enum _MapLocalSpanFlags
    {	MLS_VISIBLE_ONLY	= 0,
	MLS_INCLUDE_USER_EXPANDABLE	= 0x1,
	MLS_NEVER_COMBINE_SPANS	= 0x2,
	mlsVisibleOnly	= MLS_VISIBLE_ONLY,
	mlsIncludeUserExpandable	= MLS_INCLUDE_USER_EXPANDABLE,
	mlsNeverCombineSpans	= MLS_NEVER_COMBINE_SPANS
    } 	MapLocalSpanFlags;

typedef 
enum _DeeperLayerLineIndexToLocalFlags
    {	DLI_DEFAULT	= 0,
	DLI_CLOSEST	= 0x1,
	DLI_RIGHTMOST	= 0x2,
	DLI_AVOID_OUTER_EDGE	= 0x4,
	dliDefault	= DLI_DEFAULT,
	dliClosest	= DLI_CLOSEST,
	dliRightmost	= DLI_RIGHTMOST
    } 	DeeperLayerLineIndexToLocalFlags;

typedef 
enum _tag_LAYER_MODULE_CAPABILITIES
    {	LMC_TEXTFIND	= 0x1,
	LMC_TEXTPRINT	= 0x2,
	LMC_TEXTORIGINATE	= 0x4
    } 	LAYER_MODULE_CAPABILITIES;

typedef struct _NewSyntheticRegion
    {
    long iMarkerType;
    DWORD dwBehavior;
    LPCWSTR pszText;
    long iAnchorLine;
    CharIndex iAnchorIndex;
    DWORD dwClient;
    long iSerialValue;
    } 	NewSyntheticRegion;

typedef 
enum _syntheticregionevent
    {	sreRegionReloaded	= 0
    } 	SYNTHETIC_REGION_EVENT;

typedef 
enum _tag_FIND_SYNTHETIC_REGION_FLAGS
    {	FSR_ALL_REGIONS	= 0,
	FSR_BY_CLIENT_DATA	= 0x1,
	FSR_WITHIN_SPAN	= 0x4,
	FSR_EXACT_SPAN	= 0x8
    } 	FIND_SYNTHETIC_REGION_FLAGS;

typedef 
enum _tag_CHANGE_SYNTHETIC_REGION_FLAGS
    {	CSR_DEFAULT	= 0,
	CSR_NONUNDOABLE	= 0x1
    } 	CHANGE_SYNTHETIC_REGION_FLAGS;

typedef 
enum _hidden_region_state
    {	hrsDefault	= 0,
	hrsExpanded	= 0x1
    } 	HIDDEN_REGION_STATE;

typedef 
enum _hidden_region_type
    {	hrtConcealed	= 0,
	hrtCollapsible	= 0x1
    } 	HIDDEN_REGION_TYPE;

typedef 
enum _hidden_region_behavior
    {	hrbClientControlled	= 0,
	hrbEditorControlled	= 0x1
    } 	HIDDEN_REGION_BEHAVIOR;

typedef 
enum _tag_CHANGE_HIDDEN_REGION_FLAGS
    {	chrDefault	= 0,
	chrNonUndoable	= 0x1
    } 	CHANGE_HIDDEN_REGION_FLAGS;

typedef struct _tag_NewHiddenRegion
    {
    long iType;
    DWORD dwBehavior;
    DWORD dwState;
    TextSpan tsHiddenText;
    LPCWSTR pszBanner;
    DWORD_PTR dwClient;
    } 	NewHiddenRegion;

typedef 
enum _hiddenregionevent
    {	hreRegionDeleted	= 0,
	hreRegionResurrected	= ( hreRegionDeleted + 1 ) ,
	hreRegionReloaded	= ( hreRegionResurrected + 1 ) ,
	hreBeforeRegionExpanded	= ( hreRegionReloaded + 1 ) ,
	hreAfterRegionExpanded	= ( hreBeforeRegionExpanded + 1 ) ,
	hreBeforeRegionCollapsed	= ( hreAfterRegionExpanded + 1 ) ,
	hreAfterRegionCollapsed	= ( hreBeforeRegionCollapsed + 1 ) 
    } 	HIDDEN_REGION_EVENT;

typedef 
enum _tag_FIND_HIDDEN_REGION_FLAGS
    {	FHR_ALL_REGIONS	= 0,
	FHR_BY_CLIENT_DATA	= 0x1,
	FHR_BY_TYPE	= 0x2,
	FHR_INTERSECTS_SPAN	= 0x4,
	FHR_EXACT_SPAN	= 0x8,
	FHR_EDITOR_CONTROLLED_ONLY	= 0x10,
	FHR_CLIENT_CONTROLLED_ONLY	= 0x20,
	FHR_ENTIRELY_WITHIN_SPAN	= 0x40,
	FHR_INNERMOST	= 0x80,
	FHR_VISIBLE_ONLY	= 0x100,
	FHR_PREFER_START_IN_SPAN	= 0x200,
	FHR_MUST_START_IN_SPAN	= 0x400,
	fhrAllRegions	= FHR_ALL_REGIONS,
	fhrByClientData	= FHR_BY_CLIENT_DATA,
	fhrByType	= FHR_BY_TYPE,
	fhrWithinSpan	= FHR_INTERSECTS_SPAN,
	fhrExactSpan	= FHR_EXACT_SPAN,
	fhrEditorControlledOnly	= FHR_EDITOR_CONTROLLED_ONLY,
	fhrClientControlledOnly	= FHR_CLIENT_CONTROLLED_ONLY
    } 	FIND_HIDDEN_REGION_FLAGS;

typedef struct _tag_NewOutlineRegion
    {
    DWORD dwState;
    TextSpan tsHiddenText;
    } 	NewOutlineRegion;

typedef 
enum _tag_ADD_OUTLINE_REGION_FLAGS
    {	AOR_DEFAULT	= 0,
	AOR_PRESERVE_EXISTING	= 0x1,
	AOR_NON_UNDOABLE	= 0x100
    } 	ADD_OUTLINE_REGION_FLAGS;

typedef 
enum _tag_DATA_OBJECT_RENDER_HINT
    {	DORH_DEFAULT	= 0,
	DORH_MOVE_IN_SAME_DOC	= 0x1,
	DORH_DROP	= 0x2,
	DORH_CLIPBOARD	= 0x4,
	DORH_TOOLBOX	= 0x8,
	DORH_PREVIEW	= 0x10
    } 	DATA_OBJECT_RENDER_HINT;

typedef struct tag_ChangeInput
    {
    TextSpan m_DelSpan;
    long m_iOldLen;
    long m_iNewLen;
    const WCHAR *m_pszNewText;
    DWORD m_dwFlags;
    } 	ChangeInput;

typedef 
enum _StorageLoadFlags
    {	slfDefault	= 0,
	slfReload	= 0x1
    } 	StorageLoadFlags;

typedef 
enum _ChangeCommitGestureFlags
    {	CCG_SAVE	= 0x1,
	CCG_MULTILINE_CHANGE	= 0x2,
	CCG_CARET_ON_NEW_BUFFER_LINE	= 0x10,
	CCG_MASS_REPLACE	= 0x8,
	CCG_ENTER_COMMAND	= 0x10,
	CCG_FIND_STARTING	= 0x20,
	CCG_LOST_FOCUS	= 0x40,
	CCG_DESTRUCTIVEACTION	= 0x80,
	CCG_PROGRAMMATIC_CHANGE	= 0x1000
    } 	ChangeCommitGestureFlags;


EXTERN_C const IID LIBID_TextManagerInternal;

#ifndef __IVsCompoundAction_INTERFACE_DEFINED__
#define __IVsCompoundAction_INTERFACE_DEFINED__

/* interface IVsCompoundAction */
/* [object][uuid] */ 


EXTERN_C const IID IID_IVsCompoundAction;

#if defined(__cplusplus) && !defined(CINTERFACE)
    
    MIDL_INTERFACE("B414D071-87BA-411A-9780-33FC7D87D882")
    IVsCompoundAction : public IUnknown
    {
    public:
        virtual HRESULT STDMETHODCALLTYPE OpenCompoundAction( 
            /* [in] */ __RPC__in const WCHAR *pszDescription) = 0;
        
        virtual HRESULT STDMETHODCALLTYPE AbortCompoundAction( void) = 0;
        
        virtual HRESULT STDMETHODCALLTYPE CloseCompoundAction( void) = 0;
        
        virtual HRESULT STDMETHODCALLTYPE FlushEditActions( void) = 0;
        
    };
    
#else 	/* C style interface */

    typedef struct IVsCompoundActionVtbl
    {
        BEGIN_INTERFACE
        
        HRESULT ( STDMETHODCALLTYPE *QueryInterface )( 
            IVsCompoundAction * This,
            /* [in] */ __RPC__in REFIID riid,
            /* [iid_is][out] */ 
            __RPC__deref_out  void **ppvObject);
        
        ULONG ( STDMETHODCALLTYPE *AddRef )( 
            IVsCompoundAction * This);
        
        ULONG ( STDMETHODCALLTYPE *Release )( 
            IVsCompoundAction * This);
        
        HRESULT ( STDMETHODCALLTYPE *OpenCompoundAction )( 
            IVsCompoundAction * This,
            /* [in] */ __RPC__in const WCHAR *pszDescription);
        
        HRESULT ( STDMETHODCALLTYPE *AbortCompoundAction )( 
            IVsCompoundAction * This);
        
        HRESULT ( STDMETHODCALLTYPE *CloseCompoundAction )( 
            IVsCompoundAction * This);
        
        HRESULT ( STDMETHODCALLTYPE *FlushEditActions )( 
            IVsCompoundAction * This);
        
        END_INTERFACE
    } IVsCompoundActionVtbl;

    interface IVsCompoundAction
    {
        CONST_VTBL struct IVsCompoundActionVtbl *lpVtbl;
    };

    

#ifdef COBJMACROS


#define IVsCompoundAction_QueryInterface(This,riid,ppvObject)	\
    ( (This)->lpVtbl -> QueryInterface(This,riid,ppvObject) ) 

#define IVsCompoundAction_AddRef(This)	\
    ( (This)->lpVtbl -> AddRef(This) ) 

#define IVsCompoundAction_Release(This)	\
    ( (This)->lpVtbl -> Release(This) ) 


#define IVsCompoundAction_OpenCompoundAction(This,pszDescription)	\
    ( (This)->lpVtbl -> OpenCompoundAction(This,pszDescription) ) 

#define IVsCompoundAction_AbortCompoundAction(This)	\
    ( (This)->lpVtbl -> AbortCompoundAction(This) ) 

#define IVsCompoundAction_CloseCompoundAction(This)	\
    ( (This)->lpVtbl -> CloseCompoundAction(This) ) 

#define IVsCompoundAction_FlushEditActions(This)	\
    ( (This)->lpVtbl -> FlushEditActions(This) ) 

#endif /* COBJMACROS */


#endif 	/* C style interface */




#endif 	/* __IVsCompoundAction_INTERFACE_DEFINED__ */


#ifndef __IVsCompoundViewChange_INTERFACE_DEFINED__
#define __IVsCompoundViewChange_INTERFACE_DEFINED__

/* interface IVsCompoundViewChange */
/* [object][uuid] */ 


EXTERN_C const IID IID_IVsCompoundViewChange;

#if defined(__cplusplus) && !defined(CINTERFACE)
    
    MIDL_INTERFACE("B5C14FF1-8431-458c-BEBC-34C6925D1976")
    IVsCompoundViewChange : public IUnknown
    {
    public:
        virtual HRESULT STDMETHODCALLTYPE OpenCompoundViewChange( void) = 0;
        
        virtual HRESULT STDMETHODCALLTYPE CloseCompoundViewChange( void) = 0;
        
    };
    
#else 	/* C style interface */

    typedef struct IVsCompoundViewChangeVtbl
    {
        BEGIN_INTERFACE
        
        HRESULT ( STDMETHODCALLTYPE *QueryInterface )( 
            IVsCompoundViewChange * This,
            /* [in] */ __RPC__in REFIID riid,
            /* [iid_is][out] */ 
            __RPC__deref_out  void **ppvObject);
        
        ULONG ( STDMETHODCALLTYPE *AddRef )( 
            IVsCompoundViewChange * This);
        
        ULONG ( STDMETHODCALLTYPE *Release )( 
            IVsCompoundViewChange * This);
        
        HRESULT ( STDMETHODCALLTYPE *OpenCompoundViewChange )( 
            IVsCompoundViewChange * This);
        
        HRESULT ( STDMETHODCALLTYPE *CloseCompoundViewChange )( 
            IVsCompoundViewChange * This);
        
        END_INTERFACE
    } IVsCompoundViewChangeVtbl;

    interface IVsCompoundViewChange
    {
        CONST_VTBL struct IVsCompoundViewChangeVtbl *lpVtbl;
    };

    

#ifdef COBJMACROS


#define IVsCompoundViewChange_QueryInterface(This,riid,ppvObject)	\
    ( (This)->lpVtbl -> QueryInterface(This,riid,ppvObject) ) 

#define IVsCompoundViewChange_AddRef(This)	\
    ( (This)->lpVtbl -> AddRef(This) ) 

#define IVsCompoundViewChange_Release(This)	\
    ( (This)->lpVtbl -> Release(This) ) 


#define IVsCompoundViewChange_OpenCompoundViewChange(This)	\
    ( (This)->lpVtbl -> OpenCompoundViewChange(This) ) 

#define IVsCompoundViewChange_CloseCompoundViewChange(This)	\
    ( (This)->lpVtbl -> CloseCompoundViewChange(This) ) 

#endif /* COBJMACROS */


#endif 	/* C style interface */




#endif 	/* __IVsCompoundViewChange_INTERFACE_DEFINED__ */


#ifndef __IVsCompoundActionEvents_INTERFACE_DEFINED__
#define __IVsCompoundActionEvents_INTERFACE_DEFINED__

/* interface IVsCompoundActionEvents */
/* [object][uuid] */ 


EXTERN_C const IID IID_IVsCompoundActionEvents;

#if defined(__cplusplus) && !defined(CINTERFACE)
    
    MIDL_INTERFACE("29B88B8E-BC39-45AE-ACAB-0C670E49BCC2")
    IVsCompoundActionEvents : public IUnknown
    {
    public:
        virtual void STDMETHODCALLTYPE OnBeforeAction( void) = 0;
        
        virtual void STDMETHODCALLTYPE OnAbortAction( void) = 0;
        
    };
    
#else 	/* C style interface */

    typedef struct IVsCompoundActionEventsVtbl
    {
        BEGIN_INTERFACE
        
        HRESULT ( STDMETHODCALLTYPE *QueryInterface )( 
            IVsCompoundActionEvents * This,
            /* [in] */ __RPC__in REFIID riid,
            /* [iid_is][out] */ 
            __RPC__deref_out  void **ppvObject);
        
        ULONG ( STDMETHODCALLTYPE *AddRef )( 
            IVsCompoundActionEvents * This);
        
        ULONG ( STDMETHODCALLTYPE *Release )( 
            IVsCompoundActionEvents * This);
        
        void ( STDMETHODCALLTYPE *OnBeforeAction )( 
            IVsCompoundActionEvents * This);
        
        void ( STDMETHODCALLTYPE *OnAbortAction )( 
            IVsCompoundActionEvents * This);
        
        END_INTERFACE
    } IVsCompoundActionEventsVtbl;

    interface IVsCompoundActionEvents
    {
        CONST_VTBL struct IVsCompoundActionEventsVtbl *lpVtbl;
    };

    

#ifdef COBJMACROS


#define IVsCompoundActionEvents_QueryInterface(This,riid,ppvObject)	\
    ( (This)->lpVtbl -> QueryInterface(This,riid,ppvObject) ) 

#define IVsCompoundActionEvents_AddRef(This)	\
    ( (This)->lpVtbl -> AddRef(This) ) 

#define IVsCompoundActionEvents_Release(This)	\
    ( (This)->lpVtbl -> Release(This) ) 


#define IVsCompoundActionEvents_OnBeforeAction(This)	\
    ( (This)->lpVtbl -> OnBeforeAction(This) ) 

#define IVsCompoundActionEvents_OnAbortAction(This)	\
    ( (This)->lpVtbl -> OnAbortAction(This) ) 

#endif /* COBJMACROS */


#endif 	/* C style interface */




#endif 	/* __IVsCompoundActionEvents_INTERFACE_DEFINED__ */


#ifndef __IVsAction_INTERFACE_DEFINED__
#define __IVsAction_INTERFACE_DEFINED__

/* interface IVsAction */
/* [unique][helpstring][uuid][object] */ 


EXTERN_C const IID IID_IVsAction;

#if defined(__cplusplus) && !defined(CINTERFACE)
    
    MIDL_INTERFACE("15D2C32A-0770-4872-BB5F-EC21B0FA2436")
    IVsAction : public IUnknown
    {
    public:
        virtual HRESULT STDMETHODCALLTYPE CanMerge( 
            /* [in] */ __RPC__in_opt IVsAction *pAction) = 0;
        
        virtual HRESULT STDMETHODCALLTYPE Merge( 
            /* [in] */ __RPC__in_opt IVsAction *pAction) = 0;
        
    };
    
#else 	/* C style interface */

    typedef struct IVsActionVtbl
    {
        BEGIN_INTERFACE
        
        HRESULT ( STDMETHODCALLTYPE *QueryInterface )( 
            IVsAction * This,
            /* [in] */ __RPC__in REFIID riid,
            /* [iid_is][out] */ 
            __RPC__deref_out  void **ppvObject);
        
        ULONG ( STDMETHODCALLTYPE *AddRef )( 
            IVsAction * This);
        
        ULONG ( STDMETHODCALLTYPE *Release )( 
            IVsAction * This);
        
        HRESULT ( STDMETHODCALLTYPE *CanMerge )( 
            IVsAction * This,
            /* [in] */ __RPC__in_opt IVsAction *pAction);
        
        HRESULT ( STDMETHODCALLTYPE *Merge )( 
            IVsAction * This,
            /* [in] */ __RPC__in_opt IVsAction *pAction);
        
        END_INTERFACE
    } IVsActionVtbl;

    interface IVsAction
    {
        CONST_VTBL struct IVsActionVtbl *lpVtbl;
    };

    

#ifdef COBJMACROS


#define IVsAction_QueryInterface(This,riid,ppvObject)	\
    ( (This)->lpVtbl -> QueryInterface(This,riid,ppvObject) ) 

#define IVsAction_AddRef(This)	\
    ( (This)->lpVtbl -> AddRef(This) ) 

#define IVsAction_Release(This)	\
    ( (This)->lpVtbl -> Release(This) ) 


#define IVsAction_CanMerge(This,pAction)	\
    ( (This)->lpVtbl -> CanMerge(This,pAction) ) 

#define IVsAction_Merge(This,pAction)	\
    ( (This)->lpVtbl -> Merge(This,pAction) ) 

#endif /* COBJMACROS */


#endif 	/* C style interface */




#endif 	/* __IVsAction_INTERFACE_DEFINED__ */


#ifndef __IVsTextEditorPropertyCategoryContainer_INTERFACE_DEFINED__
#define __IVsTextEditorPropertyCategoryContainer_INTERFACE_DEFINED__

/* interface IVsTextEditorPropertyCategoryContainer */
/* [object][uuid] */ 


EXTERN_C const IID IID_IVsTextEditorPropertyCategoryContainer;

#if defined(__cplusplus) && !defined(CINTERFACE)
    
    MIDL_INTERFACE("A6078038-D578-4F32-B9B2-C83E1726454D")
    IVsTextEditorPropertyCategoryContainer : public IUnknown
    {
    public:
        virtual HRESULT STDMETHODCALLTYPE GetPropertyCategory( 
            /* [in] */ __RPC__in REFGUID rguidCategory,
            /* [out] */ __RPC__deref_out_opt IVsTextEditorPropertyContainer **ppProp) = 0;
        
    };
    
#else 	/* C style interface */

    typedef struct IVsTextEditorPropertyCategoryContainerVtbl
    {
        BEGIN_INTERFACE
        
        HRESULT ( STDMETHODCALLTYPE *QueryInterface )( 
            IVsTextEditorPropertyCategoryContainer * This,
            /* [in] */ __RPC__in REFIID riid,
            /* [iid_is][out] */ 
            __RPC__deref_out  void **ppvObject);
        
        ULONG ( STDMETHODCALLTYPE *AddRef )( 
            IVsTextEditorPropertyCategoryContainer * This);
        
        ULONG ( STDMETHODCALLTYPE *Release )( 
            IVsTextEditorPropertyCategoryContainer * This);
        
        HRESULT ( STDMETHODCALLTYPE *GetPropertyCategory )( 
            IVsTextEditorPropertyCategoryContainer * This,
            /* [in] */ __RPC__in REFGUID rguidCategory,
            /* [out] */ __RPC__deref_out_opt IVsTextEditorPropertyContainer **ppProp);
        
        END_INTERFACE
    } IVsTextEditorPropertyCategoryContainerVtbl;

    interface IVsTextEditorPropertyCategoryContainer
    {
        CONST_VTBL struct IVsTextEditorPropertyCategoryContainerVtbl *lpVtbl;
    };

    

#ifdef COBJMACROS


#define IVsTextEditorPropertyCategoryContainer_QueryInterface(This,riid,ppvObject)	\
    ( (This)->lpVtbl -> QueryInterface(This,riid,ppvObject) ) 

#define IVsTextEditorPropertyCategoryContainer_AddRef(This)	\
    ( (This)->lpVtbl -> AddRef(This) ) 

#define IVsTextEditorPropertyCategoryContainer_Release(This)	\
    ( (This)->lpVtbl -> Release(This) ) 


#define IVsTextEditorPropertyCategoryContainer_GetPropertyCategory(This,rguidCategory,ppProp)	\
    ( (This)->lpVtbl -> GetPropertyCategory(This,rguidCategory,ppProp) ) 

#endif /* COBJMACROS */


#endif 	/* C style interface */




#endif 	/* __IVsTextEditorPropertyCategoryContainer_INTERFACE_DEFINED__ */


#ifndef __IVsTextEditorPropertyContainer_INTERFACE_DEFINED__
#define __IVsTextEditorPropertyContainer_INTERFACE_DEFINED__

/* interface IVsTextEditorPropertyContainer */
/* [object][uuid] */ 


EXTERN_C const IID IID_IVsTextEditorPropertyContainer;

#if defined(__cplusplus) && !defined(CINTERFACE)
    
    MIDL_INTERFACE("7669CC95-4640-461C-8158-16F65265D77C")
    IVsTextEditorPropertyContainer : public IUnknown
    {
    public:
        virtual HRESULT STDMETHODCALLTYPE GetProperty( 
            /* [in] */ VSEDITPROPID idProp,
            /* [out] */ __RPC__out VARIANT *pvar) = 0;
        
        virtual HRESULT STDMETHODCALLTYPE SetProperty( 
            /* [in] */ VSEDITPROPID idProp,
            /* [in] */ VARIANT var) = 0;
        
        virtual HRESULT STDMETHODCALLTYPE RemoveProperty( 
            /* [in] */ VSEDITPROPID idProp) = 0;
        
    };
    
#else 	/* C style interface */

    typedef struct IVsTextEditorPropertyContainerVtbl
    {
        BEGIN_INTERFACE
        
        HRESULT ( STDMETHODCALLTYPE *QueryInterface )( 
            IVsTextEditorPropertyContainer * This,
            /* [in] */ __RPC__in REFIID riid,
            /* [iid_is][out] */ 
            __RPC__deref_out  void **ppvObject);
        
        ULONG ( STDMETHODCALLTYPE *AddRef )( 
            IVsTextEditorPropertyContainer * This);
        
        ULONG ( STDMETHODCALLTYPE *Release )( 
            IVsTextEditorPropertyContainer * This);
        
        HRESULT ( STDMETHODCALLTYPE *GetProperty )( 
            IVsTextEditorPropertyContainer * This,
            /* [in] */ VSEDITPROPID idProp,
            /* [out] */ __RPC__out VARIANT *pvar);
        
        HRESULT ( STDMETHODCALLTYPE *SetProperty )( 
            IVsTextEditorPropertyContainer * This,
            /* [in] */ VSEDITPROPID idProp,
            /* [in] */ VARIANT var);
        
        HRESULT ( STDMETHODCALLTYPE *RemoveProperty )( 
            IVsTextEditorPropertyContainer * This,
            /* [in] */ VSEDITPROPID idProp);
        
        END_INTERFACE
    } IVsTextEditorPropertyContainerVtbl;

    interface IVsTextEditorPropertyContainer
    {
        CONST_VTBL struct IVsTextEditorPropertyContainerVtbl *lpVtbl;
    };

    

#ifdef COBJMACROS


#define IVsTextEditorPropertyContainer_QueryInterface(This,riid,ppvObject)	\
    ( (This)->lpVtbl -> QueryInterface(This,riid,ppvObject) ) 

#define IVsTextEditorPropertyContainer_AddRef(This)	\
    ( (This)->lpVtbl -> AddRef(This) ) 

#define IVsTextEditorPropertyContainer_Release(This)	\
    ( (This)->lpVtbl -> Release(This) ) 


#define IVsTextEditorPropertyContainer_GetProperty(This,idProp,pvar)	\
    ( (This)->lpVtbl -> GetProperty(This,idProp,pvar) ) 

#define IVsTextEditorPropertyContainer_SetProperty(This,idProp,var)	\
    ( (This)->lpVtbl -> SetProperty(This,idProp,var) ) 

#define IVsTextEditorPropertyContainer_RemoveProperty(This,idProp)	\
    ( (This)->lpVtbl -> RemoveProperty(This,idProp) ) 

#endif /* COBJMACROS */


#endif 	/* C style interface */




#endif 	/* __IVsTextEditorPropertyContainer_INTERFACE_DEFINED__ */


#ifndef __IVsCompletionSet_INTERFACE_DEFINED__
#define __IVsCompletionSet_INTERFACE_DEFINED__

/* interface IVsCompletionSet */
/* [object][uuid] */ 


EXTERN_C const IID IID_IVsCompletionSet;

#if defined(__cplusplus) && !defined(CINTERFACE)
    
    MIDL_INTERFACE("0EF79249-B0BF-4CD0-A966-C4713546C3A5")
    IVsCompletionSet : public IUnknown
    {
    public:
        virtual DWORD STDMETHODCALLTYPE GetFlags( void) = 0;
        
        virtual long STDMETHODCALLTYPE GetCount( void) = 0;
        
        virtual HRESULT STDMETHODCALLTYPE GetDisplayText( 
            /* [in] */ long iIndex,
            /* [out] */ __RPC__deref_out_opt const WCHAR **ppszText,
            /* [optional][out] */ __RPC__out long *piGlyph) = 0;
        
        virtual HRESULT STDMETHODCALLTYPE GetImageList( 
            /* [out] */ __RPC__deref_out_opt HANDLE *phImages) = 0;
        
        virtual HRESULT STDMETHODCALLTYPE GetDescriptionText( 
            /* [in] */ long iIndex,
            /* [out] */ __RPC__deref_out_opt BSTR *pbstrDescription) = 0;
        
        virtual HRESULT STDMETHODCALLTYPE GetInitialExtent( 
            /* [out] */ __RPC__out long *piLine,
            /* [out] */ __RPC__out long *piStartCol,
            /* [out] */ __RPC__out long *piEndCol) = 0;
        
        virtual HRESULT STDMETHODCALLTYPE GetBestMatch( 
            /* [in] */ __RPC__in const WCHAR *pszSoFar,
            /* [in] */ long iLength,
            /* [out] */ __RPC__out long *piIndex,
            /* [out] */ __RPC__out DWORD *pdwFlags) = 0;
        
        virtual HRESULT STDMETHODCALLTYPE OnCommit( 
            /* [in] */ __RPC__in const WCHAR *pszSoFar,
            /* [in] */ long iIndex,
            /* [in] */ BOOL fSelected,
            /* [in] */ WCHAR cCommit,
            /* [out] */ __RPC__deref_out_opt BSTR *pbstrCompleteWord) = 0;
        
        virtual void STDMETHODCALLTYPE Dismiss( void) = 0;
        
    };
    
#else 	/* C style interface */

    typedef struct IVsCompletionSetVtbl
    {
        BEGIN_INTERFACE
        
        HRESULT ( STDMETHODCALLTYPE *QueryInterface )( 
            IVsCompletionSet * This,
            /* [in] */ __RPC__in REFIID riid,
            /* [iid_is][out] */ 
            __RPC__deref_out  void **ppvObject);
        
        ULONG ( STDMETHODCALLTYPE *AddRef )( 
            IVsCompletionSet * This);
        
        ULONG ( STDMETHODCALLTYPE *Release )( 
            IVsCompletionSet * This);
        
        DWORD ( STDMETHODCALLTYPE *GetFlags )( 
            IVsCompletionSet * This);
        
        long ( STDMETHODCALLTYPE *GetCount )( 
            IVsCompletionSet * This);
        
        HRESULT ( STDMETHODCALLTYPE *GetDisplayText )( 
            IVsCompletionSet * This,
            /* [in] */ long iIndex,
            /* [out] */ __RPC__deref_out_opt const WCHAR **ppszText,
            /* [optional][out] */ __RPC__out long *piGlyph);
        
        HRESULT ( STDMETHODCALLTYPE *GetImageList )( 
            IVsCompletionSet * This,
            /* [out] */ __RPC__deref_out_opt HANDLE *phImages);
        
        HRESULT ( STDMETHODCALLTYPE *GetDescriptionText )( 
            IVsCompletionSet * This,
            /* [in] */ long iIndex,
            /* [out] */ __RPC__deref_out_opt BSTR *pbstrDescription);
        
        HRESULT ( STDMETHODCALLTYPE *GetInitialExtent )( 
            IVsCompletionSet * This,
            /* [out] */ __RPC__out long *piLine,
            /* [out] */ __RPC__out long *piStartCol,
            /* [out] */ __RPC__out long *piEndCol);
        
        HRESULT ( STDMETHODCALLTYPE *GetBestMatch )( 
            IVsCompletionSet * This,
            /* [in] */ __RPC__in const WCHAR *pszSoFar,
            /* [in] */ long iLength,
            /* [out] */ __RPC__out long *piIndex,
            /* [out] */ __RPC__out DWORD *pdwFlags);
        
        HRESULT ( STDMETHODCALLTYPE *OnCommit )( 
            IVsCompletionSet * This,
            /* [in] */ __RPC__in const WCHAR *pszSoFar,
            /* [in] */ long iIndex,
            /* [in] */ BOOL fSelected,
            /* [in] */ WCHAR cCommit,
            /* [out] */ __RPC__deref_out_opt BSTR *pbstrCompleteWord);
        
        void ( STDMETHODCALLTYPE *Dismiss )( 
            IVsCompletionSet * This);
        
        END_INTERFACE
    } IVsCompletionSetVtbl;

    interface IVsCompletionSet
    {
        CONST_VTBL struct IVsCompletionSetVtbl *lpVtbl;
    };

    

#ifdef COBJMACROS


#define IVsCompletionSet_QueryInterface(This,riid,ppvObject)	\
    ( (This)->lpVtbl -> QueryInterface(This,riid,ppvObject) ) 

#define IVsCompletionSet_AddRef(This)	\
    ( (This)->lpVtbl -> AddRef(This) ) 

#define IVsCompletionSet_Release(This)	\
    ( (This)->lpVtbl -> Release(This) ) 


#define IVsCompletionSet_GetFlags(This)	\
    ( (This)->lpVtbl -> GetFlags(This) ) 

#define IVsCompletionSet_GetCount(This)	\
    ( (This)->lpVtbl -> GetCount(This) ) 

#define IVsCompletionSet_GetDisplayText(This,iIndex,ppszText,piGlyph)	\
    ( (This)->lpVtbl -> GetDisplayText(This,iIndex,ppszText,piGlyph) ) 

#define IVsCompletionSet_GetImageList(This,phImages)	\
    ( (This)->lpVtbl -> GetImageList(This,phImages) ) 

#define IVsCompletionSet_GetDescriptionText(This,iIndex,pbstrDescription)	\
    ( (This)->lpVtbl -> GetDescriptionText(This,iIndex,pbstrDescription) ) 

#define IVsCompletionSet_GetInitialExtent(This,piLine,piStartCol,piEndCol)	\
    ( (This)->lpVtbl -> GetInitialExtent(This,piLine,piStartCol,piEndCol) ) 

#define IVsCompletionSet_GetBestMatch(This,pszSoFar,iLength,piIndex,pdwFlags)	\
    ( (This)->lpVtbl -> GetBestMatch(This,pszSoFar,iLength,piIndex,pdwFlags) ) 

#define IVsCompletionSet_OnCommit(This,pszSoFar,iIndex,fSelected,cCommit,pbstrCompleteWord)	\
    ( (This)->lpVtbl -> OnCommit(This,pszSoFar,iIndex,fSelected,cCommit,pbstrCompleteWord) ) 

#define IVsCompletionSet_Dismiss(This)	\
    ( (This)->lpVtbl -> Dismiss(This) ) 

#endif /* COBJMACROS */


#endif 	/* C style interface */




#endif 	/* __IVsCompletionSet_INTERFACE_DEFINED__ */


#ifndef __IVsTipWindow_INTERFACE_DEFINED__
#define __IVsTipWindow_INTERFACE_DEFINED__

/* interface IVsTipWindow */
/* [object][uuid] */ 


EXTERN_C const IID IID_IVsTipWindow;

#if defined(__cplusplus) && !defined(CINTERFACE)
    
    MIDL_INTERFACE("64C7FFC4-B9EE-4599-B130-FF9D890ECFD4")
    IVsTipWindow : public IUnknown
    {
    public:
        virtual HRESULT STDMETHODCALLTYPE GetContextStream( 
            /* [out] */ __RPC__out long *piPos,
            /* [out] */ __RPC__out long *piLength) = 0;
        
        virtual HRESULT STDMETHODCALLTYPE GetSizePreferences( 
            /* [out] */ __RPC__out const RECT *prcCtxBounds,
            /* [out] */ __RPC__out TIPSIZEDATA *pSizeData) = 0;
        
        virtual HRESULT STDMETHODCALLTYPE Paint( 
            /* [in] */ __RPC__in HDC hdc,
            /* [in] */ __RPC__in const RECT *prc) = 0;
        
        virtual void STDMETHODCALLTYPE Dismiss( void) = 0;
        
        virtual LRESULT STDMETHODCALLTYPE WndProc( 
            /* [in] */ __RPC__in HWND hwnd,
            /* [in] */ UINT iMsg,
            /* [in] */ WPARAM wParam,
            /* [in] */ LPARAM lParam) = 0;
        
    };
    
#else 	/* C style interface */

    typedef struct IVsTipWindowVtbl
    {
        BEGIN_INTERFACE
        
        HRESULT ( STDMETHODCALLTYPE *QueryInterface )( 
            IVsTipWindow * This,
            /* [in] */ __RPC__in REFIID riid,
            /* [iid_is][out] */ 
            __RPC__deref_out  void **ppvObject);
        
        ULONG ( STDMETHODCALLTYPE *AddRef )( 
            IVsTipWindow * This);
        
        ULONG ( STDMETHODCALLTYPE *Release )( 
            IVsTipWindow * This);
        
        HRESULT ( STDMETHODCALLTYPE *GetContextStream )( 
            IVsTipWindow * This,
            /* [out] */ __RPC__out long *piPos,
            /* [out] */ __RPC__out long *piLength);
        
        HRESULT ( STDMETHODCALLTYPE *GetSizePreferences )( 
            IVsTipWindow * This,
            /* [out] */ __RPC__out const RECT *prcCtxBounds,
            /* [out] */ __RPC__out TIPSIZEDATA *pSizeData);
        
        HRESULT ( STDMETHODCALLTYPE *Paint )( 
            IVsTipWindow * This,
            /* [in] */ __RPC__in HDC hdc,
            /* [in] */ __RPC__in const RECT *prc);
        
        void ( STDMETHODCALLTYPE *Dismiss )( 
            IVsTipWindow * This);
        
        LRESULT ( STDMETHODCALLTYPE *WndProc )( 
            IVsTipWindow * This,
            /* [in] */ __RPC__in HWND hwnd,
            /* [in] */ UINT iMsg,
            /* [in] */ WPARAM wParam,
            /* [in] */ LPARAM lParam);
        
        END_INTERFACE
    } IVsTipWindowVtbl;

    interface IVsTipWindow
    {
        CONST_VTBL struct IVsTipWindowVtbl *lpVtbl;
    };

    

#ifdef COBJMACROS


#define IVsTipWindow_QueryInterface(This,riid,ppvObject)	\
    ( (This)->lpVtbl -> QueryInterface(This,riid,ppvObject) ) 

#define IVsTipWindow_AddRef(This)	\
    ( (This)->lpVtbl -> AddRef(This) ) 

#define IVsTipWindow_Release(This)	\
    ( (This)->lpVtbl -> Release(This) ) 


#define IVsTipWindow_GetContextStream(This,piPos,piLength)	\
    ( (This)->lpVtbl -> GetContextStream(This,piPos,piLength) ) 

#define IVsTipWindow_GetSizePreferences(This,prcCtxBounds,pSizeData)	\
    ( (This)->lpVtbl -> GetSizePreferences(This,prcCtxBounds,pSizeData) ) 

#define IVsTipWindow_Paint(This,hdc,prc)	\
    ( (This)->lpVtbl -> Paint(This,hdc,prc) ) 

#define IVsTipWindow_Dismiss(This)	\
    ( (This)->lpVtbl -> Dismiss(This) ) 

#define IVsTipWindow_WndProc(This,hwnd,iMsg,wParam,lParam)	\
    ( (This)->lpVtbl -> WndProc(This,hwnd,iMsg,wParam,lParam) ) 

#endif /* COBJMACROS */


#endif 	/* C style interface */




#endif 	/* __IVsTipWindow_INTERFACE_DEFINED__ */


#ifndef __IVsMethodTipWindow_INTERFACE_DEFINED__
#define __IVsMethodTipWindow_INTERFACE_DEFINED__

/* interface IVsMethodTipWindow */
/* [object][uuid] */ 


EXTERN_C const IID IID_IVsMethodTipWindow;

#if defined(__cplusplus) && !defined(CINTERFACE)
    
    MIDL_INTERFACE("794767F6-538A-4CD3-B29E-6C76C79A7C9C")
    IVsMethodTipWindow : public IVsTipWindow
    {
    public:
        virtual HRESULT STDMETHODCALLTYPE SetMethodData( 
            /* [in] */ __RPC__in_opt IVsMethodData *pMethodData) = 0;
        
    };
    
#else 	/* C style interface */

    typedef struct IVsMethodTipWindowVtbl
    {
        BEGIN_INTERFACE
        
        HRESULT ( STDMETHODCALLTYPE *QueryInterface )( 
            IVsMethodTipWindow * This,
            /* [in] */ __RPC__in REFIID riid,
            /* [iid_is][out] */ 
            __RPC__deref_out  void **ppvObject);
        
        ULONG ( STDMETHODCALLTYPE *AddRef )( 
            IVsMethodTipWindow * This);
        
        ULONG ( STDMETHODCALLTYPE *Release )( 
            IVsMethodTipWindow * This);
        
        HRESULT ( STDMETHODCALLTYPE *GetContextStream )( 
            IVsMethodTipWindow * This,
            /* [out] */ __RPC__out long *piPos,
            /* [out] */ __RPC__out long *piLength);
        
        HRESULT ( STDMETHODCALLTYPE *GetSizePreferences )( 
            IVsMethodTipWindow * This,
            /* [out] */ __RPC__out const RECT *prcCtxBounds,
            /* [out] */ __RPC__out TIPSIZEDATA *pSizeData);
        
        HRESULT ( STDMETHODCALLTYPE *Paint )( 
            IVsMethodTipWindow * This,
            /* [in] */ __RPC__in HDC hdc,
            /* [in] */ __RPC__in const RECT *prc);
        
        void ( STDMETHODCALLTYPE *Dismiss )( 
            IVsMethodTipWindow * This);
        
        LRESULT ( STDMETHODCALLTYPE *WndProc )( 
            IVsMethodTipWindow * This,
            /* [in] */ __RPC__in HWND hwnd,
            /* [in] */ UINT iMsg,
            /* [in] */ WPARAM wParam,
            /* [in] */ LPARAM lParam);
        
        HRESULT ( STDMETHODCALLTYPE *SetMethodData )( 
            IVsMethodTipWindow * This,
            /* [in] */ __RPC__in_opt IVsMethodData *pMethodData);
        
        END_INTERFACE
    } IVsMethodTipWindowVtbl;

    interface IVsMethodTipWindow
    {
        CONST_VTBL struct IVsMethodTipWindowVtbl *lpVtbl;
    };

    

#ifdef COBJMACROS


#define IVsMethodTipWindow_QueryInterface(This,riid,ppvObject)	\
    ( (This)->lpVtbl -> QueryInterface(This,riid,ppvObject) ) 

#define IVsMethodTipWindow_AddRef(This)	\
    ( (This)->lpVtbl -> AddRef(This) ) 

#define IVsMethodTipWindow_Release(This)	\
    ( (This)->lpVtbl -> Release(This) ) 


#define IVsMethodTipWindow_GetContextStream(This,piPos,piLength)	\
    ( (This)->lpVtbl -> GetContextStream(This,piPos,piLength) ) 

#define IVsMethodTipWindow_GetSizePreferences(This,prcCtxBounds,pSizeData)	\
    ( (This)->lpVtbl -> GetSizePreferences(This,prcCtxBounds,pSizeData) ) 

#define IVsMethodTipWindow_Paint(This,hdc,prc)	\
    ( (This)->lpVtbl -> Paint(This,hdc,prc) ) 

#define IVsMethodTipWindow_Dismiss(This)	\
    ( (This)->lpVtbl -> Dismiss(This) ) 

#define IVsMethodTipWindow_WndProc(This,hwnd,iMsg,wParam,lParam)	\
    ( (This)->lpVtbl -> WndProc(This,hwnd,iMsg,wParam,lParam) ) 


#define IVsMethodTipWindow_SetMethodData(This,pMethodData)	\
    ( (This)->lpVtbl -> SetMethodData(This,pMethodData) ) 

#endif /* COBJMACROS */


#endif 	/* C style interface */




#endif 	/* __IVsMethodTipWindow_INTERFACE_DEFINED__ */


#ifndef __IVsTextTipWindow_INTERFACE_DEFINED__
#define __IVsTextTipWindow_INTERFACE_DEFINED__

/* interface IVsTextTipWindow */
/* [object][uuid] */ 


EXTERN_C const IID IID_IVsTextTipWindow;

#if defined(__cplusplus) && !defined(CINTERFACE)
    
    MIDL_INTERFACE("A6ACCB2F-1E0D-458B-A878-D40EB89C1DF4")
    IVsTextTipWindow : public IVsTipWindow
    {
    public:
        virtual HRESULT STDMETHODCALLTYPE SetTextTipData( 
            /* [in] */ __RPC__in_opt IVsTextTipData *pMethodData) = 0;
        
    };
    
#else 	/* C style interface */

    typedef struct IVsTextTipWindowVtbl
    {
        BEGIN_INTERFACE
        
        HRESULT ( STDMETHODCALLTYPE *QueryInterface )( 
            IVsTextTipWindow * This,
            /* [in] */ __RPC__in REFIID riid,
            /* [iid_is][out] */ 
            __RPC__deref_out  void **ppvObject);
        
        ULONG ( STDMETHODCALLTYPE *AddRef )( 
            IVsTextTipWindow * This);
        
        ULONG ( STDMETHODCALLTYPE *Release )( 
            IVsTextTipWindow * This);
        
        HRESULT ( STDMETHODCALLTYPE *GetContextStream )( 
            IVsTextTipWindow * This,
            /* [out] */ __RPC__out long *piPos,
            /* [out] */ __RPC__out long *piLength);
        
        HRESULT ( STDMETHODCALLTYPE *GetSizePreferences )( 
            IVsTextTipWindow * This,
            /* [out] */ __RPC__out const RECT *prcCtxBounds,
            /* [out] */ __RPC__out TIPSIZEDATA *pSizeData);
        
        HRESULT ( STDMETHODCALLTYPE *Paint )( 
            IVsTextTipWindow * This,
            /* [in] */ __RPC__in HDC hdc,
            /* [in] */ __RPC__in const RECT *prc);
        
        void ( STDMETHODCALLTYPE *Dismiss )( 
            IVsTextTipWindow * This);
        
        LRESULT ( STDMETHODCALLTYPE *WndProc )( 
            IVsTextTipWindow * This,
            /* [in] */ __RPC__in HWND hwnd,
            /* [in] */ UINT iMsg,
            /* [in] */ WPARAM wParam,
            /* [in] */ LPARAM lParam);
        
        HRESULT ( STDMETHODCALLTYPE *SetTextTipData )( 
            IVsTextTipWindow * This,
            /* [in] */ __RPC__in_opt IVsTextTipData *pMethodData);
        
        END_INTERFACE
    } IVsTextTipWindowVtbl;

    interface IVsTextTipWindow
    {
        CONST_VTBL struct IVsTextTipWindowVtbl *lpVtbl;
    };

    

#ifdef COBJMACROS


#define IVsTextTipWindow_QueryInterface(This,riid,ppvObject)	\
    ( (This)->lpVtbl -> QueryInterface(This,riid,ppvObject) ) 

#define IVsTextTipWindow_AddRef(This)	\
    ( (This)->lpVtbl -> AddRef(This) ) 

#define IVsTextTipWindow_Release(This)	\
    ( (This)->lpVtbl -> Release(This) ) 


#define IVsTextTipWindow_GetContextStream(This,piPos,piLength)	\
    ( (This)->lpVtbl -> GetContextStream(This,piPos,piLength) ) 

#define IVsTextTipWindow_GetSizePreferences(This,prcCtxBounds,pSizeData)	\
    ( (This)->lpVtbl -> GetSizePreferences(This,prcCtxBounds,pSizeData) ) 

#define IVsTextTipWindow_Paint(This,hdc,prc)	\
    ( (This)->lpVtbl -> Paint(This,hdc,prc) ) 

#define IVsTextTipWindow_Dismiss(This)	\
    ( (This)->lpVtbl -> Dismiss(This) ) 

#define IVsTextTipWindow_WndProc(This,hwnd,iMsg,wParam,lParam)	\
    ( (This)->lpVtbl -> WndProc(This,hwnd,iMsg,wParam,lParam) ) 


#define IVsTextTipWindow_SetTextTipData(This,pMethodData)	\
    ( (This)->lpVtbl -> SetTextTipData(This,pMethodData) ) 

#endif /* COBJMACROS */


#endif 	/* C style interface */




#endif 	/* __IVsTextTipWindow_INTERFACE_DEFINED__ */


#ifndef __IVsMethodData_INTERFACE_DEFINED__
#define __IVsMethodData_INTERFACE_DEFINED__

/* interface IVsMethodData */
/* [object][uuid] */ 


EXTERN_C const IID IID_IVsMethodData;

#if defined(__cplusplus) && !defined(CINTERFACE)
    
    MIDL_INTERFACE("69D2811C-682C-4290-B92E-5DFB823DF24C")
    IVsMethodData : public IUnknown
    {
    public:
        virtual long STDMETHODCALLTYPE GetOverloadCount( void) = 0;
        
        virtual const WCHAR *STDMETHODCALLTYPE GetMethodText( 
            /* [in] */ long iMethod,
            /* [in] */ MethodTextType type) = 0;
        
        virtual long STDMETHODCALLTYPE GetParameterCount( 
            /* [in] */ long iMethod) = 0;
        
        virtual const WCHAR *STDMETHODCALLTYPE GetParameterText( 
            /* [in] */ long iMethod,
            /* [in] */ long iParm,
            /* [in] */ ParameterTextType type) = 0;
        
        virtual long STDMETHODCALLTYPE GetCurrentParameter( 
            /* [in] */ long iMethod) = 0;
        
        virtual HRESULT STDMETHODCALLTYPE GetContextStream( 
            /* [out] */ __RPC__out long *piPos,
            /* [out] */ __RPC__out long *piLength) = 0;
        
        virtual void STDMETHODCALLTYPE OnDismiss( void) = 0;
        
        virtual long STDMETHODCALLTYPE GetCurMethod( void) = 0;
        
        virtual void STDMETHODCALLTYPE UpdateView( void) = 0;
        
        virtual long STDMETHODCALLTYPE NextMethod( void) = 0;
        
        virtual long STDMETHODCALLTYPE PrevMethod( void) = 0;
        
    };
    
#else 	/* C style interface */

    typedef struct IVsMethodDataVtbl
    {
        BEGIN_INTERFACE
        
        HRESULT ( STDMETHODCALLTYPE *QueryInterface )( 
            IVsMethodData * This,
            /* [in] */ __RPC__in REFIID riid,
            /* [iid_is][out] */ 
            __RPC__deref_out  void **ppvObject);
        
        ULONG ( STDMETHODCALLTYPE *AddRef )( 
            IVsMethodData * This);
        
        ULONG ( STDMETHODCALLTYPE *Release )( 
            IVsMethodData * This);
        
        long ( STDMETHODCALLTYPE *GetOverloadCount )( 
            IVsMethodData * This);
        
        const WCHAR *( STDMETHODCALLTYPE *GetMethodText )( 
            IVsMethodData * This,
            /* [in] */ long iMethod,
            /* [in] */ MethodTextType type);
        
        long ( STDMETHODCALLTYPE *GetParameterCount )( 
            IVsMethodData * This,
            /* [in] */ long iMethod);
        
        const WCHAR *( STDMETHODCALLTYPE *GetParameterText )( 
            IVsMethodData * This,
            /* [in] */ long iMethod,
            /* [in] */ long iParm,
            /* [in] */ ParameterTextType type);
        
        long ( STDMETHODCALLTYPE *GetCurrentParameter )( 
            IVsMethodData * This,
            /* [in] */ long iMethod);
        
        HRESULT ( STDMETHODCALLTYPE *GetContextStream )( 
            IVsMethodData * This,
            /* [out] */ __RPC__out long *piPos,
            /* [out] */ __RPC__out long *piLength);
        
        void ( STDMETHODCALLTYPE *OnDismiss )( 
            IVsMethodData * This);
        
        long ( STDMETHODCALLTYPE *GetCurMethod )( 
            IVsMethodData * This);
        
        void ( STDMETHODCALLTYPE *UpdateView )( 
            IVsMethodData * This);
        
        long ( STDMETHODCALLTYPE *NextMethod )( 
            IVsMethodData * This);
        
        long ( STDMETHODCALLTYPE *PrevMethod )( 
            IVsMethodData * This);
        
        END_INTERFACE
    } IVsMethodDataVtbl;

    interface IVsMethodData
    {
        CONST_VTBL struct IVsMethodDataVtbl *lpVtbl;
    };

    

#ifdef COBJMACROS


#define IVsMethodData_QueryInterface(This,riid,ppvObject)	\
    ( (This)->lpVtbl -> QueryInterface(This,riid,ppvObject) ) 

#define IVsMethodData_AddRef(This)	\
    ( (This)->lpVtbl -> AddRef(This) ) 

#define IVsMethodData_Release(This)	\
    ( (This)->lpVtbl -> Release(This) ) 


#define IVsMethodData_GetOverloadCount(This)	\
    ( (This)->lpVtbl -> GetOverloadCount(This) ) 

#define IVsMethodData_GetMethodText(This,iMethod,type)	\
    ( (This)->lpVtbl -> GetMethodText(This,iMethod,type) ) 

#define IVsMethodData_GetParameterCount(This,iMethod)	\
    ( (This)->lpVtbl -> GetParameterCount(This,iMethod) ) 

#define IVsMethodData_GetParameterText(This,iMethod,iParm,type)	\
    ( (This)->lpVtbl -> GetParameterText(This,iMethod,iParm,type) ) 

#define IVsMethodData_GetCurrentParameter(This,iMethod)	\
    ( (This)->lpVtbl -> GetCurrentParameter(This,iMethod) ) 

#define IVsMethodData_GetContextStream(This,piPos,piLength)	\
    ( (This)->lpVtbl -> GetContextStream(This,piPos,piLength) ) 

#define IVsMethodData_OnDismiss(This)	\
    ( (This)->lpVtbl -> OnDismiss(This) ) 

#define IVsMethodData_GetCurMethod(This)	\
    ( (This)->lpVtbl -> GetCurMethod(This) ) 

#define IVsMethodData_UpdateView(This)	\
    ( (This)->lpVtbl -> UpdateView(This) ) 

#define IVsMethodData_NextMethod(This)	\
    ( (This)->lpVtbl -> NextMethod(This) ) 

#define IVsMethodData_PrevMethod(This)	\
    ( (This)->lpVtbl -> PrevMethod(This) ) 

#endif /* COBJMACROS */


#endif 	/* C style interface */




#endif 	/* __IVsMethodData_INTERFACE_DEFINED__ */


#ifndef __IVsTextTipData_INTERFACE_DEFINED__
#define __IVsTextTipData_INTERFACE_DEFINED__

/* interface IVsTextTipData */
/* [object][uuid] */ 


EXTERN_C const IID IID_IVsTextTipData;

#if defined(__cplusplus) && !defined(CINTERFACE)
    
    MIDL_INTERFACE("90609097-17C8-40E8-8128-67194C7384B1")
    IVsTextTipData : public IUnknown
    {
    public:
        virtual HRESULT STDMETHODCALLTYPE GetTipText( 
            /* [custom][out] */ __RPC__deref_out_opt BSTR *pbstrText,
            /* [out] */ __RPC__out BOOL *pfGetFontInfo) = 0;
        
        virtual HRESULT STDMETHODCALLTYPE GetTipFontInfo( 
            /* [in] */ long cChars,
            /* [size_is][out] */ __RPC__out_ecount_full(cChars) ULONG *pdwFontAttr) = 0;
        
        virtual HRESULT STDMETHODCALLTYPE GetContextStream( 
            /* [out] */ __RPC__out long *piPos,
            /* [out] */ __RPC__out long *piLength) = 0;
        
        virtual void STDMETHODCALLTYPE OnDismiss( void) = 0;
        
        virtual void STDMETHODCALLTYPE UpdateView( void) = 0;
        
    };
    
#else 	/* C style interface */

    typedef struct IVsTextTipDataVtbl
    {
        BEGIN_INTERFACE
        
        HRESULT ( STDMETHODCALLTYPE *QueryInterface )( 
            IVsTextTipData * This,
            /* [in] */ __RPC__in REFIID riid,
            /* [iid_is][out] */ 
            __RPC__deref_out  void **ppvObject);
        
        ULONG ( STDMETHODCALLTYPE *AddRef )( 
            IVsTextTipData * This);
        
        ULONG ( STDMETHODCALLTYPE *Release )( 
            IVsTextTipData * This);
        
        HRESULT ( STDMETHODCALLTYPE *GetTipText )( 
            IVsTextTipData * This,
            /* [custom][out] */ __RPC__deref_out_opt BSTR *pbstrText,
            /* [out] */ __RPC__out BOOL *pfGetFontInfo);
        
        HRESULT ( STDMETHODCALLTYPE *GetTipFontInfo )( 
            IVsTextTipData * This,
            /* [in] */ long cChars,
            /* [size_is][out] */ __RPC__out_ecount_full(cChars) ULONG *pdwFontAttr);
        
        HRESULT ( STDMETHODCALLTYPE *GetContextStream )( 
            IVsTextTipData * This,
            /* [out] */ __RPC__out long *piPos,
            /* [out] */ __RPC__out long *piLength);
        
        void ( STDMETHODCALLTYPE *OnDismiss )( 
            IVsTextTipData * This);
        
        void ( STDMETHODCALLTYPE *UpdateView )( 
            IVsTextTipData * This);
        
        END_INTERFACE
    } IVsTextTipDataVtbl;

    interface IVsTextTipData
    {
        CONST_VTBL struct IVsTextTipDataVtbl *lpVtbl;
    };

    

#ifdef COBJMACROS


#define IVsTextTipData_QueryInterface(This,riid,ppvObject)	\
    ( (This)->lpVtbl -> QueryInterface(This,riid,ppvObject) ) 

#define IVsTextTipData_AddRef(This)	\
    ( (This)->lpVtbl -> AddRef(This) ) 

#define IVsTextTipData_Release(This)	\
    ( (This)->lpVtbl -> Release(This) ) 


#define IVsTextTipData_GetTipText(This,pbstrText,pfGetFontInfo)	\
    ( (This)->lpVtbl -> GetTipText(This,pbstrText,pfGetFontInfo) ) 

#define IVsTextTipData_GetTipFontInfo(This,cChars,pdwFontAttr)	\
    ( (This)->lpVtbl -> GetTipFontInfo(This,cChars,pdwFontAttr) ) 

#define IVsTextTipData_GetContextStream(This,piPos,piLength)	\
    ( (This)->lpVtbl -> GetContextStream(This,piPos,piLength) ) 

#define IVsTextTipData_OnDismiss(This)	\
    ( (This)->lpVtbl -> OnDismiss(This) ) 

#define IVsTextTipData_UpdateView(This)	\
    ( (This)->lpVtbl -> UpdateView(This) ) 

#endif /* COBJMACROS */


#endif 	/* C style interface */




#endif 	/* __IVsTextTipData_INTERFACE_DEFINED__ */


#ifndef __IVsTextView_INTERFACE_DEFINED__
#define __IVsTextView_INTERFACE_DEFINED__

/* interface IVsTextView */
/* [object][uuid] */ 


EXTERN_C const IID IID_IVsTextView;

#if defined(__cplusplus) && !defined(CINTERFACE)
    
    MIDL_INTERFACE("BB23A14B-7C61-469A-9890-A95648CED5E6")
    IVsTextView : public IUnknown
    {
    public:
        virtual HRESULT STDMETHODCALLTYPE Initialize( 
            /* [in] */ __RPC__in_opt IVsTextLines *pBuffer,
            /* [in] */ __RPC__in HWND hwndParent,
            /* [in] */ DWORD InitFlags,
            /* [in] */ __RPC__in const INITVIEW *pInitView) = 0;
        
        virtual HRESULT STDMETHODCALLTYPE CloseView( void) = 0;
        
        virtual HRESULT STDMETHODCALLTYPE GetCaretPos( 
            /* [out] */ __RPC__out long *piLine,
            /* [out] */ __RPC__out ViewCol *piColumn) = 0;
        
        virtual HRESULT STDMETHODCALLTYPE SetCaretPos( 
            /* [in] */ long iLine,
            /* [in] */ ViewCol iColumn) = 0;
        
        virtual HRESULT STDMETHODCALLTYPE GetSelectionSpan( 
            /* [out] */ __RPC__out TextSpan *pSpan) = 0;
        
        virtual HRESULT STDMETHODCALLTYPE GetSelection( 
            /* [out] */ __RPC__out long *piAnchorLine,
            /* [out] */ __RPC__out ViewCol *piAnchorCol,
            /* [out] */ __RPC__out long *piEndLine,
            /* [out] */ __RPC__out ViewCol *piEndCol) = 0;
        
        virtual HRESULT STDMETHODCALLTYPE SetSelection( 
            /* [in] */ long iAnchorLine,
            /* [in] */ ViewCol iAnchorCol,
            /* [in] */ long iEndLine,
            /* [in] */ ViewCol iEndCol) = 0;
        
        virtual TextSelMode STDMETHODCALLTYPE GetSelectionMode( void) = 0;
        
        virtual HRESULT STDMETHODCALLTYPE SetSelectionMode( 
            /* [in] */ TextSelMode iSelMode) = 0;
        
        virtual HRESULT STDMETHODCALLTYPE ClearSelection( 
            /* [in] */ BOOL fMoveToAnchor) = 0;
        
        virtual HRESULT STDMETHODCALLTYPE CenterLines( 
            /* [in] */ long iTopLine,
            /* [in] */ long iCount) = 0;
        
        virtual HRESULT STDMETHODCALLTYPE GetSelectedText( 
            /* [out] */ __RPC__deref_out_opt BSTR *pbstrText) = 0;
        
        virtual HRESULT STDMETHODCALLTYPE GetSelectionDataObject( 
            /* [out] */ __RPC__deref_out_opt IDataObject **ppIDataObject) = 0;
        
        virtual HRESULT STDMETHODCALLTYPE GetTextStream( 
            /* [in] */ long iTopLine,
            /* [in] */ ViewCol iTopCol,
            /* [in] */ long iBottomLine,
            /* [in] */ ViewCol iBottomCol,
            /* [out] */ __RPC__deref_out_opt BSTR *pbstrText) = 0;
        
        virtual HRESULT STDMETHODCALLTYPE GetBuffer( 
            /* [out] */ __RPC__deref_out_opt IVsTextLines **ppBuffer) = 0;
        
        virtual HRESULT STDMETHODCALLTYPE SetBuffer( 
            /* [in] */ __RPC__in_opt IVsTextLines *pBuffer) = 0;
        
        virtual HWND STDMETHODCALLTYPE GetWindowHandle( void) = 0;
        
        virtual HRESULT STDMETHODCALLTYPE GetScrollInfo( 
            /* [in] */ long iBar,
            /* [out] */ __RPC__out long *piMinUnit,
            /* [out] */ __RPC__out long *piMaxUnit,
            /* [out] */ __RPC__out long *piVisibleUnits,
            /* [out] */ __RPC__out long *piFirstVisibleUnit) = 0;
        
        virtual HRESULT STDMETHODCALLTYPE SetScrollPosition( 
            /* [in] */ long iBar,
            /* [in] */ long iFirstVisibleUnit) = 0;
        
        virtual HRESULT STDMETHODCALLTYPE AddCommandFilter( 
            /* [in] */ __RPC__in_opt IOleCommandTarget *pNewCmdTarg,
            /* [out] */ __RPC__deref_out_opt IOleCommandTarget **ppNextCmdTarg) = 0;
        
        virtual HRESULT STDMETHODCALLTYPE RemoveCommandFilter( 
            /* [in] */ __RPC__in_opt IOleCommandTarget *pCmdTarg) = 0;
        
        virtual HRESULT STDMETHODCALLTYPE UpdateCompletionStatus( 
            /* [in] */ __RPC__in_opt IVsCompletionSet *pCompSet,
            /* [in] */ DWORD dwFlags) = 0;
        
        virtual HRESULT STDMETHODCALLTYPE UpdateTipWindow( 
            /* [in] */ __RPC__in_opt IVsTipWindow *pTipWindow,
            /* [in] */ DWORD dwFlags) = 0;
        
        virtual HRESULT STDMETHODCALLTYPE GetWordExtent( 
            /* [in] */ long iLine,
            /* [in] */ CharIndex iCol,
            /* [in] */ DWORD dwFlags,
            /* [out] */ __RPC__out TextSpan *pSpan) = 0;
        
        virtual HRESULT STDMETHODCALLTYPE RestrictViewRange( 
            /* [in] */ long iMinLine,
            /* [in] */ long iMaxLine,
            /* [in] */ __RPC__in_opt IVsViewRangeClient *pClient) = 0;
        
        virtual HRESULT STDMETHODCALLTYPE ReplaceTextOnLine( 
            /* [in] */ long iLine,
            /* [in] */ CharIndex iStartCol,
            /* [in] */ long iCharsToReplace,
            /* [in] */ __RPC__in const WCHAR *pszNewText,
            /* [in] */ long iNewLen) = 0;
        
        virtual HRESULT STDMETHODCALLTYPE GetLineAndColumn( 
            /* [in] */ long iPos,
            /* [out] */ __RPC__out long *piLine,
            /* [out] */ __RPC__out CharIndex *piIndex) = 0;
        
        virtual HRESULT STDMETHODCALLTYPE GetNearestPosition( 
            /* [in] */ long iLine,
            /* [in] */ ViewCol iCol,
            /* [out] */ __RPC__out long *piPos,
            /* [out] */ __RPC__out long *piVirtualSpaces) = 0;
        
        virtual HRESULT STDMETHODCALLTYPE UpdateViewFrameCaption( void) = 0;
        
        virtual HRESULT STDMETHODCALLTYPE CenterColumns( 
            /* [in] */ long iLine,
            /* [in] */ ViewCol iLeftCol,
            /* [in] */ long iColCount) = 0;
        
        virtual HRESULT STDMETHODCALLTYPE EnsureSpanVisible( 
            /* [in] */ TextSpan span) = 0;
        
        virtual HRESULT STDMETHODCALLTYPE PositionCaretForEditing( 
            /* [in] */ long iLine,
            /* [in] */ long cIndentLevels) = 0;
        
        virtual HRESULT STDMETHODCALLTYPE GetPointOfLineColumn( 
            /* [in] */ long iLine,
            /* [in] */ ViewCol iCol,
            /* [retval][out] */ __RPC__out POINT *ppt) = 0;
        
        virtual HRESULT STDMETHODCALLTYPE GetLineHeight( 
            /* [retval][out] */ __RPC__out long *piLineHeight) = 0;
        
        virtual HRESULT STDMETHODCALLTYPE HighlightMatchingBrace( 
            /* [in] */ DWORD dwFlags,
            /* [in] */ ULONG cSpans,
            /* [size_is][in] */ __RPC__in_ecount_full(cSpans) TextSpan *rgBaseSpans) = 0;
        
        virtual HRESULT STDMETHODCALLTYPE SendExplicitFocus( void) = 0;
        
        virtual HRESULT STDMETHODCALLTYPE SetTopLine( 
            /* [in] */ long iBaseLine) = 0;
        
    };
    
#else 	/* C style interface */

    typedef struct IVsTextViewVtbl
    {
        BEGIN_INTERFACE
        
        HRESULT ( STDMETHODCALLTYPE *QueryInterface )( 
            IVsTextView * This,
            /* [in] */ __RPC__in REFIID riid,
            /* [iid_is][out] */ 
            __RPC__deref_out  void **ppvObject);
        
        ULONG ( STDMETHODCALLTYPE *AddRef )( 
            IVsTextView * This);
        
        ULONG ( STDMETHODCALLTYPE *Release )( 
            IVsTextView * This);
        
        HRESULT ( STDMETHODCALLTYPE *Initialize )( 
            IVsTextView * This,
            /* [in] */ __RPC__in_opt IVsTextLines *pBuffer,
            /* [in] */ __RPC__in HWND hwndParent,
            /* [in] */ DWORD InitFlags,
            /* [in] */ __RPC__in const INITVIEW *pInitView);
        
        HRESULT ( STDMETHODCALLTYPE *CloseView )( 
            IVsTextView * This);
        
        HRESULT ( STDMETHODCALLTYPE *GetCaretPos )( 
            IVsTextView * This,
            /* [out] */ __RPC__out long *piLine,
            /* [out] */ __RPC__out ViewCol *piColumn);
        
        HRESULT ( STDMETHODCALLTYPE *SetCaretPos )( 
            IVsTextView * This,
            /* [in] */ long iLine,
            /* [in] */ ViewCol iColumn);
        
        HRESULT ( STDMETHODCALLTYPE *GetSelectionSpan )( 
            IVsTextView * This,
            /* [out] */ __RPC__out TextSpan *pSpan);
        
        HRESULT ( STDMETHODCALLTYPE *GetSelection )( 
            IVsTextView * This,
            /* [out] */ __RPC__out long *piAnchorLine,
            /* [out] */ __RPC__out ViewCol *piAnchorCol,
            /* [out] */ __RPC__out long *piEndLine,
            /* [out] */ __RPC__out ViewCol *piEndCol);
        
        HRESULT ( STDMETHODCALLTYPE *SetSelection )( 
            IVsTextView * This,
            /* [in] */ long iAnchorLine,
            /* [in] */ ViewCol iAnchorCol,
            /* [in] */ long iEndLine,
            /* [in] */ ViewCol iEndCol);
        
        TextSelMode ( STDMETHODCALLTYPE *GetSelectionMode )( 
            IVsTextView * This);
        
        HRESULT ( STDMETHODCALLTYPE *SetSelectionMode )( 
            IVsTextView * This,
            /* [in] */ TextSelMode iSelMode);
        
        HRESULT ( STDMETHODCALLTYPE *ClearSelection )( 
            IVsTextView * This,
            /* [in] */ BOOL fMoveToAnchor);
        
        HRESULT ( STDMETHODCALLTYPE *CenterLines )( 
            IVsTextView * This,
            /* [in] */ long iTopLine,
            /* [in] */ long iCount);
        
        HRESULT ( STDMETHODCALLTYPE *GetSelectedText )( 
            IVsTextView * This,
            /* [out] */ __RPC__deref_out_opt BSTR *pbstrText);
        
        HRESULT ( STDMETHODCALLTYPE *GetSelectionDataObject )( 
            IVsTextView * This,
            /* [out] */ __RPC__deref_out_opt IDataObject **ppIDataObject);
        
        HRESULT ( STDMETHODCALLTYPE *GetTextStream )( 
            IVsTextView * This,
            /* [in] */ long iTopLine,
            /* [in] */ ViewCol iTopCol,
            /* [in] */ long iBottomLine,
            /* [in] */ ViewCol iBottomCol,
            /* [out] */ __RPC__deref_out_opt BSTR *pbstrText);
        
        HRESULT ( STDMETHODCALLTYPE *GetBuffer )( 
            IVsTextView * This,
            /* [out] */ __RPC__deref_out_opt IVsTextLines **ppBuffer);
        
        HRESULT ( STDMETHODCALLTYPE *SetBuffer )( 
            IVsTextView * This,
            /* [in] */ __RPC__in_opt IVsTextLines *pBuffer);
        
        HWND ( STDMETHODCALLTYPE *GetWindowHandle )( 
            IVsTextView * This);
        
        HRESULT ( STDMETHODCALLTYPE *GetScrollInfo )( 
            IVsTextView * This,
            /* [in] */ long iBar,
            /* [out] */ __RPC__out long *piMinUnit,
            /* [out] */ __RPC__out long *piMaxUnit,
            /* [out] */ __RPC__out long *piVisibleUnits,
            /* [out] */ __RPC__out long *piFirstVisibleUnit);
        
        HRESULT ( STDMETHODCALLTYPE *SetScrollPosition )( 
            IVsTextView * This,
            /* [in] */ long iBar,
            /* [in] */ long iFirstVisibleUnit);
        
        HRESULT ( STDMETHODCALLTYPE *AddCommandFilter )( 
            IVsTextView * This,
            /* [in] */ __RPC__in_opt IOleCommandTarget *pNewCmdTarg,
            /* [out] */ __RPC__deref_out_opt IOleCommandTarget **ppNextCmdTarg);
        
        HRESULT ( STDMETHODCALLTYPE *RemoveCommandFilter )( 
            IVsTextView * This,
            /* [in] */ __RPC__in_opt IOleCommandTarget *pCmdTarg);
        
        HRESULT ( STDMETHODCALLTYPE *UpdateCompletionStatus )( 
            IVsTextView * This,
            /* [in] */ __RPC__in_opt IVsCompletionSet *pCompSet,
            /* [in] */ DWORD dwFlags);
        
        HRESULT ( STDMETHODCALLTYPE *UpdateTipWindow )( 
            IVsTextView * This,
            /* [in] */ __RPC__in_opt IVsTipWindow *pTipWindow,
            /* [in] */ DWORD dwFlags);
        
        HRESULT ( STDMETHODCALLTYPE *GetWordExtent )( 
            IVsTextView * This,
            /* [in] */ long iLine,
            /* [in] */ CharIndex iCol,
            /* [in] */ DWORD dwFlags,
            /* [out] */ __RPC__out TextSpan *pSpan);
        
        HRESULT ( STDMETHODCALLTYPE *RestrictViewRange )( 
            IVsTextView * This,
            /* [in] */ long iMinLine,
            /* [in] */ long iMaxLine,
            /* [in] */ __RPC__in_opt IVsViewRangeClient *pClient);
        
        HRESULT ( STDMETHODCALLTYPE *ReplaceTextOnLine )( 
            IVsTextView * This,
            /* [in] */ long iLine,
            /* [in] */ CharIndex iStartCol,
            /* [in] */ long iCharsToReplace,
            /* [in] */ __RPC__in const WCHAR *pszNewText,
            /* [in] */ long iNewLen);
        
        HRESULT ( STDMETHODCALLTYPE *GetLineAndColumn )( 
            IVsTextView * This,
            /* [in] */ long iPos,
            /* [out] */ __RPC__out long *piLine,
            /* [out] */ __RPC__out CharIndex *piIndex);
        
        HRESULT ( STDMETHODCALLTYPE *GetNearestPosition )( 
            IVsTextView * This,
            /* [in] */ long iLine,
            /* [in] */ ViewCol iCol,
            /* [out] */ __RPC__out long *piPos,
            /* [out] */ __RPC__out long *piVirtualSpaces);
        
        HRESULT ( STDMETHODCALLTYPE *UpdateViewFrameCaption )( 
            IVsTextView * This);
        
        HRESULT ( STDMETHODCALLTYPE *CenterColumns )( 
            IVsTextView * This,
            /* [in] */ long iLine,
            /* [in] */ ViewCol iLeftCol,
            /* [in] */ long iColCount);
        
        HRESULT ( STDMETHODCALLTYPE *EnsureSpanVisible )( 
            IVsTextView * This,
            /* [in] */ TextSpan span);
        
        HRESULT ( STDMETHODCALLTYPE *PositionCaretForEditing )( 
            IVsTextView * This,
            /* [in] */ long iLine,
            /* [in] */ long cIndentLevels);
        
        HRESULT ( STDMETHODCALLTYPE *GetPointOfLineColumn )( 
            IVsTextView * This,
            /* [in] */ long iLine,
            /* [in] */ ViewCol iCol,
            /* [retval][out] */ __RPC__out POINT *ppt);
        
        HRESULT ( STDMETHODCALLTYPE *GetLineHeight )( 
            IVsTextView * This,
            /* [retval][out] */ __RPC__out long *piLineHeight);
        
        HRESULT ( STDMETHODCALLTYPE *HighlightMatchingBrace )( 
            IVsTextView * This,
            /* [in] */ DWORD dwFlags,
            /* [in] */ ULONG cSpans,
            /* [size_is][in] */ __RPC__in_ecount_full(cSpans) TextSpan *rgBaseSpans);
        
        HRESULT ( STDMETHODCALLTYPE *SendExplicitFocus )( 
            IVsTextView * This);
        
        HRESULT ( STDMETHODCALLTYPE *SetTopLine )( 
            IVsTextView * This,
            /* [in] */ long iBaseLine);
        
        END_INTERFACE
    } IVsTextViewVtbl;

    interface IVsTextView
    {
        CONST_VTBL struct IVsTextViewVtbl *lpVtbl;
    };

    

#ifdef COBJMACROS


#define IVsTextView_QueryInterface(This,riid,ppvObject)	\
    ( (This)->lpVtbl -> QueryInterface(This,riid,ppvObject) ) 

#define IVsTextView_AddRef(This)	\
    ( (This)->lpVtbl -> AddRef(This) ) 

#define IVsTextView_Release(This)	\
    ( (This)->lpVtbl -> Release(This) ) 


#define IVsTextView_Initialize(This,pBuffer,hwndParent,InitFlags,pInitView)	\
    ( (This)->lpVtbl -> Initialize(This,pBuffer,hwndParent,InitFlags,pInitView) ) 

#define IVsTextView_CloseView(This)	\
    ( (This)->lpVtbl -> CloseView(This) ) 

#define IVsTextView_GetCaretPos(This,piLine,piColumn)	\
    ( (This)->lpVtbl -> GetCaretPos(This,piLine,piColumn) ) 

#define IVsTextView_SetCaretPos(This,iLine,iColumn)	\
    ( (This)->lpVtbl -> SetCaretPos(This,iLine,iColumn) ) 

#define IVsTextView_GetSelectionSpan(This,pSpan)	\
    ( (This)->lpVtbl -> GetSelectionSpan(This,pSpan) ) 

#define IVsTextView_GetSelection(This,piAnchorLine,piAnchorCol,piEndLine,piEndCol)	\
    ( (This)->lpVtbl -> GetSelection(This,piAnchorLine,piAnchorCol,piEndLine,piEndCol) ) 

#define IVsTextView_SetSelection(This,iAnchorLine,iAnchorCol,iEndLine,iEndCol)	\
    ( (This)->lpVtbl -> SetSelection(This,iAnchorLine,iAnchorCol,iEndLine,iEndCol) ) 

#define IVsTextView_GetSelectionMode(This)	\
    ( (This)->lpVtbl -> GetSelectionMode(This) ) 

#define IVsTextView_SetSelectionMode(This,iSelMode)	\
    ( (This)->lpVtbl -> SetSelectionMode(This,iSelMode) ) 

#define IVsTextView_ClearSelection(This,fMoveToAnchor)	\
    ( (This)->lpVtbl -> ClearSelection(This,fMoveToAnchor) ) 

#define IVsTextView_CenterLines(This,iTopLine,iCount)	\
    ( (This)->lpVtbl -> CenterLines(This,iTopLine,iCount) ) 

#define IVsTextView_GetSelectedText(This,pbstrText)	\
    ( (This)->lpVtbl -> GetSelectedText(This,pbstrText) ) 

#define IVsTextView_GetSelectionDataObject(This,ppIDataObject)	\
    ( (This)->lpVtbl -> GetSelectionDataObject(This,ppIDataObject) ) 

#define IVsTextView_GetTextStream(This,iTopLine,iTopCol,iBottomLine,iBottomCol,pbstrText)	\
    ( (This)->lpVtbl -> GetTextStream(This,iTopLine,iTopCol,iBottomLine,iBottomCol,pbstrText) ) 

#define IVsTextView_GetBuffer(This,ppBuffer)	\
    ( (This)->lpVtbl -> GetBuffer(This,ppBuffer) ) 

#define IVsTextView_SetBuffer(This,pBuffer)	\
    ( (This)->lpVtbl -> SetBuffer(This,pBuffer) ) 

#define IVsTextView_GetWindowHandle(This)	\
    ( (This)->lpVtbl -> GetWindowHandle(This) ) 

#define IVsTextView_GetScrollInfo(This,iBar,piMinUnit,piMaxUnit,piVisibleUnits,piFirstVisibleUnit)	\
    ( (This)->lpVtbl -> GetScrollInfo(This,iBar,piMinUnit,piMaxUnit,piVisibleUnits,piFirstVisibleUnit) ) 

#define IVsTextView_SetScrollPosition(This,iBar,iFirstVisibleUnit)	\
    ( (This)->lpVtbl -> SetScrollPosition(This,iBar,iFirstVisibleUnit) ) 

#define IVsTextView_AddCommandFilter(This,pNewCmdTarg,ppNextCmdTarg)	\
    ( (This)->lpVtbl -> AddCommandFilter(This,pNewCmdTarg,ppNextCmdTarg) ) 

#define IVsTextView_RemoveCommandFilter(This,pCmdTarg)	\
    ( (This)->lpVtbl -> RemoveCommandFilter(This,pCmdTarg) ) 

#define IVsTextView_UpdateCompletionStatus(This,pCompSet,dwFlags)	\
    ( (This)->lpVtbl -> UpdateCompletionStatus(This,pCompSet,dwFlags) ) 

#define IVsTextView_UpdateTipWindow(This,pTipWindow,dwFlags)	\
    ( (This)->lpVtbl -> UpdateTipWindow(This,pTipWindow,dwFlags) ) 

#define IVsTextView_GetWordExtent(This,iLine,iCol,dwFlags,pSpan)	\
    ( (This)->lpVtbl -> GetWordExtent(This,iLine,iCol,dwFlags,pSpan) ) 

#define IVsTextView_RestrictViewRange(This,iMinLine,iMaxLine,pClient)	\
    ( (This)->lpVtbl -> RestrictViewRange(This,iMinLine,iMaxLine,pClient) ) 

#define IVsTextView_ReplaceTextOnLine(This,iLine,iStartCol,iCharsToReplace,pszNewText,iNewLen)	\
    ( (This)->lpVtbl -> ReplaceTextOnLine(This,iLine,iStartCol,iCharsToReplace,pszNewText,iNewLen) ) 

#define IVsTextView_GetLineAndColumn(This,iPos,piLine,piIndex)	\
    ( (This)->lpVtbl -> GetLineAndColumn(This,iPos,piLine,piIndex) ) 

#define IVsTextView_GetNearestPosition(This,iLine,iCol,piPos,piVirtualSpaces)	\
    ( (This)->lpVtbl -> GetNearestPosition(This,iLine,iCol,piPos,piVirtualSpaces) ) 

#define IVsTextView_UpdateViewFrameCaption(This)	\
    ( (This)->lpVtbl -> UpdateViewFrameCaption(This) ) 

#define IVsTextView_CenterColumns(This,iLine,iLeftCol,iColCount)	\
    ( (This)->lpVtbl -> CenterColumns(This,iLine,iLeftCol,iColCount) ) 

#define IVsTextView_EnsureSpanVisible(This,span)	\
    ( (This)->lpVtbl -> EnsureSpanVisible(This,span) ) 

#define IVsTextView_PositionCaretForEditing(This,iLine,cIndentLevels)	\
    ( (This)->lpVtbl -> PositionCaretForEditing(This,iLine,cIndentLevels) ) 

#define IVsTextView_GetPointOfLineColumn(This,iLine,iCol,ppt)	\
    ( (This)->lpVtbl -> GetPointOfLineColumn(This,iLine,iCol,ppt) ) 

#define IVsTextView_GetLineHeight(This,piLineHeight)	\
    ( (This)->lpVtbl -> GetLineHeight(This,piLineHeight) ) 

#define IVsTextView_HighlightMatchingBrace(This,dwFlags,cSpans,rgBaseSpans)	\
    ( (This)->lpVtbl -> HighlightMatchingBrace(This,dwFlags,cSpans,rgBaseSpans) ) 

#define IVsTextView_SendExplicitFocus(This)	\
    ( (This)->lpVtbl -> SendExplicitFocus(This) ) 

#define IVsTextView_SetTopLine(This,iBaseLine)	\
    ( (This)->lpVtbl -> SetTopLine(This,iBaseLine) ) 

#endif /* COBJMACROS */


#endif 	/* C style interface */




#endif 	/* __IVsTextView_INTERFACE_DEFINED__ */


#ifndef __IVsThreadSafeTextView_INTERFACE_DEFINED__
#define __IVsThreadSafeTextView_INTERFACE_DEFINED__

/* interface IVsThreadSafeTextView */
/* [object][uuid] */ 


EXTERN_C const IID IID_IVsThreadSafeTextView;

#if defined(__cplusplus) && !defined(CINTERFACE)
    
    MIDL_INTERFACE("FEC3BF2A-7833-4777-AE74-5FFF86D6794E")
    IVsThreadSafeTextView : public IUnknown
    {
    public:
        virtual HRESULT STDMETHODCALLTYPE ThreadSafeSetCaretPos( 
            /* [in] */ long iLine,
            /* [in] */ ViewCol iColumn) = 0;
        
    };
    
#else 	/* C style interface */

    typedef struct IVsThreadSafeTextViewVtbl
    {
        BEGIN_INTERFACE
        
        HRESULT ( STDMETHODCALLTYPE *QueryInterface )( 
            IVsThreadSafeTextView * This,
            /* [in] */ __RPC__in REFIID riid,
            /* [iid_is][out] */ 
            __RPC__deref_out  void **ppvObject);
        
        ULONG ( STDMETHODCALLTYPE *AddRef )( 
            IVsThreadSafeTextView * This);
        
        ULONG ( STDMETHODCALLTYPE *Release )( 
            IVsThreadSafeTextView * This);
        
        HRESULT ( STDMETHODCALLTYPE *ThreadSafeSetCaretPos )( 
            IVsThreadSafeTextView * This,
            /* [in] */ long iLine,
            /* [in] */ ViewCol iColumn);
        
        END_INTERFACE
    } IVsThreadSafeTextViewVtbl;

    interface IVsThreadSafeTextView
    {
        CONST_VTBL struct IVsThreadSafeTextViewVtbl *lpVtbl;
    };

    

#ifdef COBJMACROS


#define IVsThreadSafeTextView_QueryInterface(This,riid,ppvObject)	\
    ( (This)->lpVtbl -> QueryInterface(This,riid,ppvObject) ) 

#define IVsThreadSafeTextView_AddRef(This)	\
    ( (This)->lpVtbl -> AddRef(This) ) 

#define IVsThreadSafeTextView_Release(This)	\
    ( (This)->lpVtbl -> Release(This) ) 


#define IVsThreadSafeTextView_ThreadSafeSetCaretPos(This,iLine,iColumn)	\
    ( (This)->lpVtbl -> ThreadSafeSetCaretPos(This,iLine,iColumn) ) 

#endif /* COBJMACROS */


#endif 	/* C style interface */




#endif 	/* __IVsThreadSafeTextView_INTERFACE_DEFINED__ */


#ifndef __IVsLayeredTextView_INTERFACE_DEFINED__
#define __IVsLayeredTextView_INTERFACE_DEFINED__

/* interface IVsLayeredTextView */
/* [object][uuid] */ 


EXTERN_C const IID IID_IVsLayeredTextView;

#if defined(__cplusplus) && !defined(CINTERFACE)
    
    MIDL_INTERFACE("60BD661B-133C-4B5C-A335-4B174C5A352F")
    IVsLayeredTextView : public IUnknown
    {
    public:
        virtual HRESULT STDMETHODCALLTYPE GetSelectedAtom( 
            /* [in] */ DWORD dwFlags,
            /* [out] */ __RPC__deref_out_opt IUnknown **ppunkAtom) = 0;
        
        virtual HRESULT STDMETHODCALLTYPE GetRelativeSelectionState( 
            /* [in] */ DWORD dwFlags,
            /* [in] */ __RPC__in_opt IVsTextLayer *pReferenceLayer,
            /* [out] */ __RPC__out SELECTIONSTATE *pSelState) = 0;
        
        virtual HRESULT STDMETHODCALLTYPE SetRelativeSelectionState( 
            /* [in] */ DWORD dwFlags,
            /* [in] */ __RPC__in_opt IVsTextLayer *pReferenceLayer,
            /* [in] */ __RPC__in SELECTIONSTATE *pSelState) = 0;
        
        virtual HRESULT STDMETHODCALLTYPE GetTopmostLayer( 
            /* [out] */ __RPC__deref_out_opt IVsTextLayer **ppLayer) = 0;
        
    };
    
#else 	/* C style interface */

    typedef struct IVsLayeredTextViewVtbl
    {
        BEGIN_INTERFACE
        
        HRESULT ( STDMETHODCALLTYPE *QueryInterface )( 
            IVsLayeredTextView * This,
            /* [in] */ __RPC__in REFIID riid,
            /* [iid_is][out] */ 
            __RPC__deref_out  void **ppvObject);
        
        ULONG ( STDMETHODCALLTYPE *AddRef )( 
            IVsLayeredTextView * This);
        
        ULONG ( STDMETHODCALLTYPE *Release )( 
            IVsLayeredTextView * This);
        
        HRESULT ( STDMETHODCALLTYPE *GetSelectedAtom )( 
            IVsLayeredTextView * This,
            /* [in] */ DWORD dwFlags,
            /* [out] */ __RPC__deref_out_opt IUnknown **ppunkAtom);
        
        HRESULT ( STDMETHODCALLTYPE *GetRelativeSelectionState )( 
            IVsLayeredTextView * This,
            /* [in] */ DWORD dwFlags,
            /* [in] */ __RPC__in_opt IVsTextLayer *pReferenceLayer,
            /* [out] */ __RPC__out SELECTIONSTATE *pSelState);
        
        HRESULT ( STDMETHODCALLTYPE *SetRelativeSelectionState )( 
            IVsLayeredTextView * This,
            /* [in] */ DWORD dwFlags,
            /* [in] */ __RPC__in_opt IVsTextLayer *pReferenceLayer,
            /* [in] */ __RPC__in SELECTIONSTATE *pSelState);
        
        HRESULT ( STDMETHODCALLTYPE *GetTopmostLayer )( 
            IVsLayeredTextView * This,
            /* [out] */ __RPC__deref_out_opt IVsTextLayer **ppLayer);
        
        END_INTERFACE
    } IVsLayeredTextViewVtbl;

    interface IVsLayeredTextView
    {
        CONST_VTBL struct IVsLayeredTextViewVtbl *lpVtbl;
    };

    

#ifdef COBJMACROS


#define IVsLayeredTextView_QueryInterface(This,riid,ppvObject)	\
    ( (This)->lpVtbl -> QueryInterface(This,riid,ppvObject) ) 

#define IVsLayeredTextView_AddRef(This)	\
    ( (This)->lpVtbl -> AddRef(This) ) 

#define IVsLayeredTextView_Release(This)	\
    ( (This)->lpVtbl -> Release(This) ) 


#define IVsLayeredTextView_GetSelectedAtom(This,dwFlags,ppunkAtom)	\
    ( (This)->lpVtbl -> GetSelectedAtom(This,dwFlags,ppunkAtom) ) 

#define IVsLayeredTextView_GetRelativeSelectionState(This,dwFlags,pReferenceLayer,pSelState)	\
    ( (This)->lpVtbl -> GetRelativeSelectionState(This,dwFlags,pReferenceLayer,pSelState) ) 

#define IVsLayeredTextView_SetRelativeSelectionState(This,dwFlags,pReferenceLayer,pSelState)	\
    ( (This)->lpVtbl -> SetRelativeSelectionState(This,dwFlags,pReferenceLayer,pSelState) ) 

#define IVsLayeredTextView_GetTopmostLayer(This,ppLayer)	\
    ( (This)->lpVtbl -> GetTopmostLayer(This,ppLayer) ) 

#endif /* COBJMACROS */


#endif 	/* C style interface */




#endif 	/* __IVsLayeredTextView_INTERFACE_DEFINED__ */


#ifndef __IVsTextViewFilter_INTERFACE_DEFINED__
#define __IVsTextViewFilter_INTERFACE_DEFINED__

/* interface IVsTextViewFilter */
/* [object][uuid] */ 


EXTERN_C const IID IID_IVsTextViewFilter;

#if defined(__cplusplus) && !defined(CINTERFACE)
    
    MIDL_INTERFACE("6B6F0B32-B88B-40F8-A8FE-97438C5BDBEF")
    IVsTextViewFilter : public IUnknown
    {
    public:
        virtual HRESULT STDMETHODCALLTYPE GetWordExtent( 
            /* [in] */ long iLine,
            /* [in] */ CharIndex iIndex,
            /* [in] */ DWORD dwFlags,
            /* [out] */ __RPC__out TextSpan *pSpan) = 0;
        
        virtual HRESULT STDMETHODCALLTYPE GetDataTipText( 
            /* [out][in] */ __RPC__inout TextSpan *pSpan,
            /* [out] */ __RPC__deref_out_opt BSTR *pbstrText) = 0;
        
        virtual HRESULT STDMETHODCALLTYPE GetPairExtents( 
            /* [in] */ long iLine,
            /* [in] */ CharIndex iIndex,
            /* [out] */ __RPC__out TextSpan *pSpan) = 0;
        
    };
    
#else 	/* C style interface */

    typedef struct IVsTextViewFilterVtbl
    {
        BEGIN_INTERFACE
        
        HRESULT ( STDMETHODCALLTYPE *QueryInterface )( 
            IVsTextViewFilter * This,
            /* [in] */ __RPC__in REFIID riid,
            /* [iid_is][out] */ 
            __RPC__deref_out  void **ppvObject);
        
        ULONG ( STDMETHODCALLTYPE *AddRef )( 
            IVsTextViewFilter * This);
        
        ULONG ( STDMETHODCALLTYPE *Release )( 
            IVsTextViewFilter * This);
        
        HRESULT ( STDMETHODCALLTYPE *GetWordExtent )( 
            IVsTextViewFilter * This,
            /* [in] */ long iLine,
            /* [in] */ CharIndex iIndex,
            /* [in] */ DWORD dwFlags,
            /* [out] */ __RPC__out TextSpan *pSpan);
        
        HRESULT ( STDMETHODCALLTYPE *GetDataTipText )( 
            IVsTextViewFilter * This,
            /* [out][in] */ __RPC__inout TextSpan *pSpan,
            /* [out] */ __RPC__deref_out_opt BSTR *pbstrText);
        
        HRESULT ( STDMETHODCALLTYPE *GetPairExtents )( 
            IVsTextViewFilter * This,
            /* [in] */ long iLine,
            /* [in] */ CharIndex iIndex,
            /* [out] */ __RPC__out TextSpan *pSpan);
        
        END_INTERFACE
    } IVsTextViewFilterVtbl;

    interface IVsTextViewFilter
    {
        CONST_VTBL struct IVsTextViewFilterVtbl *lpVtbl;
    };

    

#ifdef COBJMACROS


#define IVsTextViewFilter_QueryInterface(This,riid,ppvObject)	\
    ( (This)->lpVtbl -> QueryInterface(This,riid,ppvObject) ) 

#define IVsTextViewFilter_AddRef(This)	\
    ( (This)->lpVtbl -> AddRef(This) ) 

#define IVsTextViewFilter_Release(This)	\
    ( (This)->lpVtbl -> Release(This) ) 


#define IVsTextViewFilter_GetWordExtent(This,iLine,iIndex,dwFlags,pSpan)	\
    ( (This)->lpVtbl -> GetWordExtent(This,iLine,iIndex,dwFlags,pSpan) ) 

#define IVsTextViewFilter_GetDataTipText(This,pSpan,pbstrText)	\
    ( (This)->lpVtbl -> GetDataTipText(This,pSpan,pbstrText) ) 

#define IVsTextViewFilter_GetPairExtents(This,iLine,iIndex,pSpan)	\
    ( (This)->lpVtbl -> GetPairExtents(This,iLine,iIndex,pSpan) ) 

#endif /* COBJMACROS */


#endif 	/* C style interface */




#endif 	/* __IVsTextViewFilter_INTERFACE_DEFINED__ */


#ifndef __IVsLanguageContextProvider_INTERFACE_DEFINED__
#define __IVsLanguageContextProvider_INTERFACE_DEFINED__

/* interface IVsLanguageContextProvider */
/* [object][uuid] */ 


EXTERN_C const IID IID_IVsLanguageContextProvider;

#if defined(__cplusplus) && !defined(CINTERFACE)
    
    MIDL_INTERFACE("19404D57-F8E4-42F4-9255-B8F889B0C50C")
    IVsLanguageContextProvider : public IUnknown
    {
    public:
        virtual HRESULT STDMETHODCALLTYPE UpdateLanguageContext( 
            DWORD dwHint,
            __RPC__in_opt IVsTextLines *pBuffer,
            __RPC__in TextSpan *ptsSelection,
            __RPC__in_opt IVsUserContext *pUC) = 0;
        
    };
    
#else 	/* C style interface */

    typedef struct IVsLanguageContextProviderVtbl
    {
        BEGIN_INTERFACE
        
        HRESULT ( STDMETHODCALLTYPE *QueryInterface )( 
            IVsLanguageContextProvider * This,
            /* [in] */ __RPC__in REFIID riid,
            /* [iid_is][out] */ 
            __RPC__deref_out  void **ppvObject);
        
        ULONG ( STDMETHODCALLTYPE *AddRef )( 
            IVsLanguageContextProvider * This);
        
        ULONG ( STDMETHODCALLTYPE *Release )( 
            IVsLanguageContextProvider * This);
        
        HRESULT ( STDMETHODCALLTYPE *UpdateLanguageContext )( 
            IVsLanguageContextProvider * This,
            DWORD dwHint,
            __RPC__in_opt IVsTextLines *pBuffer,
            __RPC__in TextSpan *ptsSelection,
            __RPC__in_opt IVsUserContext *pUC);
        
        END_INTERFACE
    } IVsLanguageContextProviderVtbl;

    interface IVsLanguageContextProvider
    {
        CONST_VTBL struct IVsLanguageContextProviderVtbl *lpVtbl;
    };

    

#ifdef COBJMACROS


#define IVsLanguageContextProvider_QueryInterface(This,riid,ppvObject)	\
    ( (This)->lpVtbl -> QueryInterface(This,riid,ppvObject) ) 

#define IVsLanguageContextProvider_AddRef(This)	\
    ( (This)->lpVtbl -> AddRef(This) ) 

#define IVsLanguageContextProvider_Release(This)	\
    ( (This)->lpVtbl -> Release(This) ) 


#define IVsLanguageContextProvider_UpdateLanguageContext(This,dwHint,pBuffer,ptsSelection,pUC)	\
    ( (This)->lpVtbl -> UpdateLanguageContext(This,dwHint,pBuffer,ptsSelection,pUC) ) 

#endif /* COBJMACROS */


#endif 	/* C style interface */




#endif 	/* __IVsLanguageContextProvider_INTERFACE_DEFINED__ */


#ifndef __IVsTextMarkerContextProvider_INTERFACE_DEFINED__
#define __IVsTextMarkerContextProvider_INTERFACE_DEFINED__

/* interface IVsTextMarkerContextProvider */
/* [object][uuid] */ 


EXTERN_C const IID IID_IVsTextMarkerContextProvider;

#if defined(__cplusplus) && !defined(CINTERFACE)
    
    MIDL_INTERFACE("F1671B6B-1090-4DFF-BBFD-133EB428BB6C")
    IVsTextMarkerContextProvider : public IUnknown
    {
    public:
        virtual HRESULT STDMETHODCALLTYPE UpdateContextForMarker( 
            DWORD dwReserved,
            __RPC__in_opt IVsUserContext *pUC) = 0;
        
        virtual HRESULT STDMETHODCALLTYPE RemoveLastContext( 
            DWORD dwReserved,
            __RPC__in_opt IVsUserContext *pUC) = 0;
        
    };
    
#else 	/* C style interface */

    typedef struct IVsTextMarkerContextProviderVtbl
    {
        BEGIN_INTERFACE
        
        HRESULT ( STDMETHODCALLTYPE *QueryInterface )( 
            IVsTextMarkerContextProvider * This,
            /* [in] */ __RPC__in REFIID riid,
            /* [iid_is][out] */ 
            __RPC__deref_out  void **ppvObject);
        
        ULONG ( STDMETHODCALLTYPE *AddRef )( 
            IVsTextMarkerContextProvider * This);
        
        ULONG ( STDMETHODCALLTYPE *Release )( 
            IVsTextMarkerContextProvider * This);
        
        HRESULT ( STDMETHODCALLTYPE *UpdateContextForMarker )( 
            IVsTextMarkerContextProvider * This,
            DWORD dwReserved,
            __RPC__in_opt IVsUserContext *pUC);
        
        HRESULT ( STDMETHODCALLTYPE *RemoveLastContext )( 
            IVsTextMarkerContextProvider * This,
            DWORD dwReserved,
            __RPC__in_opt IVsUserContext *pUC);
        
        END_INTERFACE
    } IVsTextMarkerContextProviderVtbl;

    interface IVsTextMarkerContextProvider
    {
        CONST_VTBL struct IVsTextMarkerContextProviderVtbl *lpVtbl;
    };

    

#ifdef COBJMACROS


#define IVsTextMarkerContextProvider_QueryInterface(This,riid,ppvObject)	\
    ( (This)->lpVtbl -> QueryInterface(This,riid,ppvObject) ) 

#define IVsTextMarkerContextProvider_AddRef(This)	\
    ( (This)->lpVtbl -> AddRef(This) ) 

#define IVsTextMarkerContextProvider_Release(This)	\
    ( (This)->lpVtbl -> Release(This) ) 


#define IVsTextMarkerContextProvider_UpdateContextForMarker(This,dwReserved,pUC)	\
    ( (This)->lpVtbl -> UpdateContextForMarker(This,dwReserved,pUC) ) 

#define IVsTextMarkerContextProvider_RemoveLastContext(This,dwReserved,pUC)	\
    ( (This)->lpVtbl -> RemoveLastContext(This,dwReserved,pUC) ) 

#endif /* COBJMACROS */


#endif 	/* C style interface */




#endif 	/* __IVsTextMarkerContextProvider_INTERFACE_DEFINED__ */


#ifndef __IVsViewRangeClient_INTERFACE_DEFINED__
#define __IVsViewRangeClient_INTERFACE_DEFINED__

/* interface IVsViewRangeClient */
/* [object][uuid] */ 


EXTERN_C const IID IID_IVsViewRangeClient;

#if defined(__cplusplus) && !defined(CINTERFACE)
    
    MIDL_INTERFACE("30491A5B-A47E-4C9C-8204-18586648A277")
    IVsViewRangeClient : public IUnknown
    {
    public:
        virtual HRESULT STDMETHODCALLTYPE AdjustViewRange( 
            /* [in] */ __RPC__in_opt IVsTextView *pView,
            /* [in] */ TextViewAction action,
            /* [in] */ long iLine,
            /* [in] */ long iCount) = 0;
        
    };
    
#else 	/* C style interface */

    typedef struct IVsViewRangeClientVtbl
    {
        BEGIN_INTERFACE
        
        HRESULT ( STDMETHODCALLTYPE *QueryInterface )( 
            IVsViewRangeClient * This,
            /* [in] */ __RPC__in REFIID riid,
            /* [iid_is][out] */ 
            __RPC__deref_out  void **ppvObject);
        
        ULONG ( STDMETHODCALLTYPE *AddRef )( 
            IVsViewRangeClient * This);
        
        ULONG ( STDMETHODCALLTYPE *Release )( 
            IVsViewRangeClient * This);
        
        HRESULT ( STDMETHODCALLTYPE *AdjustViewRange )( 
            IVsViewRangeClient * This,
            /* [in] */ __RPC__in_opt IVsTextView *pView,
            /* [in] */ TextViewAction action,
            /* [in] */ long iLine,
            /* [in] */ long iCount);
        
        END_INTERFACE
    } IVsViewRangeClientVtbl;

    interface IVsViewRangeClient
    {
        CONST_VTBL struct IVsViewRangeClientVtbl *lpVtbl;
    };

    

#ifdef COBJMACROS


#define IVsViewRangeClient_QueryInterface(This,riid,ppvObject)	\
    ( (This)->lpVtbl -> QueryInterface(This,riid,ppvObject) ) 

#define IVsViewRangeClient_AddRef(This)	\
    ( (This)->lpVtbl -> AddRef(This) ) 

#define IVsViewRangeClient_Release(This)	\
    ( (This)->lpVtbl -> Release(This) ) 


#define IVsViewRangeClient_AdjustViewRange(This,pView,action,iLine,iCount)	\
    ( (This)->lpVtbl -> AdjustViewRange(This,pView,action,iLine,iCount) ) 

#endif /* COBJMACROS */


#endif 	/* C style interface */




#endif 	/* __IVsViewRangeClient_INTERFACE_DEFINED__ */


#ifndef __IVsTextViewEvents_INTERFACE_DEFINED__
#define __IVsTextViewEvents_INTERFACE_DEFINED__

/* interface IVsTextViewEvents */
/* [object][uuid] */ 


EXTERN_C const IID IID_IVsTextViewEvents;

#if defined(__cplusplus) && !defined(CINTERFACE)
    
    MIDL_INTERFACE("E1965DA9-E791-49E2-9F9D-ED766D885967")
    IVsTextViewEvents : public IUnknown
    {
    public:
        virtual void STDMETHODCALLTYPE OnSetFocus( 
            /* [in] */ __RPC__in_opt IVsTextView *pView) = 0;
        
        virtual void STDMETHODCALLTYPE OnKillFocus( 
            /* [in] */ __RPC__in_opt IVsTextView *pView) = 0;
        
        virtual void STDMETHODCALLTYPE OnSetBuffer( 
            /* [in] */ __RPC__in_opt IVsTextView *pView,
            /* [in] */ __RPC__in_opt IVsTextLines *pBuffer) = 0;
        
        virtual void STDMETHODCALLTYPE OnChangeScrollInfo( 
            /* [in] */ __RPC__in_opt IVsTextView *pView,
            /* [in] */ long iBar,
            /* [in] */ long iMinUnit,
            /* [in] */ long iMaxUnits,
            /* [in] */ long iVisibleUnits,
            /* [in] */ long iFirstVisibleUnit) = 0;
        
        virtual void STDMETHODCALLTYPE OnChangeCaretLine( 
            /* [in] */ __RPC__in_opt IVsTextView *pView,
            /* [in] */ long iNewLine,
            /* [in] */ long iOldLine) = 0;
        
    };
    
#else 	/* C style interface */

    typedef struct IVsTextViewEventsVtbl
    {
        BEGIN_INTERFACE
        
        HRESULT ( STDMETHODCALLTYPE *QueryInterface )( 
            IVsTextViewEvents * This,
            /* [in] */ __RPC__in REFIID riid,
            /* [iid_is][out] */ 
            __RPC__deref_out  void **ppvObject);
        
        ULONG ( STDMETHODCALLTYPE *AddRef )( 
            IVsTextViewEvents * This);
        
        ULONG ( STDMETHODCALLTYPE *Release )( 
            IVsTextViewEvents * This);
        
        void ( STDMETHODCALLTYPE *OnSetFocus )( 
            IVsTextViewEvents * This,
            /* [in] */ __RPC__in_opt IVsTextView *pView);
        
        void ( STDMETHODCALLTYPE *OnKillFocus )( 
            IVsTextViewEvents * This,
            /* [in] */ __RPC__in_opt IVsTextView *pView);
        
        void ( STDMETHODCALLTYPE *OnSetBuffer )( 
            IVsTextViewEvents * This,
            /* [in] */ __RPC__in_opt IVsTextView *pView,
            /* [in] */ __RPC__in_opt IVsTextLines *pBuffer);
        
        void ( STDMETHODCALLTYPE *OnChangeScrollInfo )( 
            IVsTextViewEvents * This,
            /* [in] */ __RPC__in_opt IVsTextView *pView,
            /* [in] */ long iBar,
            /* [in] */ long iMinUnit,
            /* [in] */ long iMaxUnits,
            /* [in] */ long iVisibleUnits,
            /* [in] */ long iFirstVisibleUnit);
        
        void ( STDMETHODCALLTYPE *OnChangeCaretLine )( 
            IVsTextViewEvents * This,
            /* [in] */ __RPC__in_opt IVsTextView *pView,
            /* [in] */ long iNewLine,
            /* [in] */ long iOldLine);
        
        END_INTERFACE
    } IVsTextViewEventsVtbl;

    interface IVsTextViewEvents
    {
        CONST_VTBL struct IVsTextViewEventsVtbl *lpVtbl;
    };

    

#ifdef COBJMACROS


#define IVsTextViewEvents_QueryInterface(This,riid,ppvObject)	\
    ( (This)->lpVtbl -> QueryInterface(This,riid,ppvObject) ) 

#define IVsTextViewEvents_AddRef(This)	\
    ( (This)->lpVtbl -> AddRef(This) ) 

#define IVsTextViewEvents_Release(This)	\
    ( (This)->lpVtbl -> Release(This) ) 


#define IVsTextViewEvents_OnSetFocus(This,pView)	\
    ( (This)->lpVtbl -> OnSetFocus(This,pView) ) 

#define IVsTextViewEvents_OnKillFocus(This,pView)	\
    ( (This)->lpVtbl -> OnKillFocus(This,pView) ) 

#define IVsTextViewEvents_OnSetBuffer(This,pView,pBuffer)	\
    ( (This)->lpVtbl -> OnSetBuffer(This,pView,pBuffer) ) 

#define IVsTextViewEvents_OnChangeScrollInfo(This,pView,iBar,iMinUnit,iMaxUnits,iVisibleUnits,iFirstVisibleUnit)	\
    ( (This)->lpVtbl -> OnChangeScrollInfo(This,pView,iBar,iMinUnit,iMaxUnits,iVisibleUnits,iFirstVisibleUnit) ) 

#define IVsTextViewEvents_OnChangeCaretLine(This,pView,iNewLine,iOldLine)	\
    ( (This)->lpVtbl -> OnChangeCaretLine(This,pView,iNewLine,iOldLine) ) 

#endif /* COBJMACROS */


#endif 	/* C style interface */




#endif 	/* __IVsTextViewEvents_INTERFACE_DEFINED__ */


#ifndef __IVsTextManager_INTERFACE_DEFINED__
#define __IVsTextManager_INTERFACE_DEFINED__

/* interface IVsTextManager */
/* [object][uuid] */ 


EXTERN_C const IID IID_IVsTextManager;

#if defined(__cplusplus) && !defined(CINTERFACE)
    
    MIDL_INTERFACE("909F83E3-B3FC-4BBF-8820-64378744B39B")
    IVsTextManager : public IUnknown
    {
    public:
        virtual HRESULT STDMETHODCALLTYPE RegisterView( 
            /* [in] */ __RPC__in_opt IVsTextView *pView,
            /* [in] */ __RPC__in_opt IVsTextBuffer *pBuffer) = 0;
        
        virtual HRESULT STDMETHODCALLTYPE UnregisterView( 
            /* [in] */ __RPC__in_opt IVsTextView *pView) = 0;
        
        virtual HRESULT STDMETHODCALLTYPE EnumViews( 
            /* [in] */ __RPC__in_opt IVsTextBuffer *pBuffer,
            /* [out] */ __RPC__deref_out_opt IVsEnumTextViews **ppEnum) = 0;
        
        virtual HRESULT STDMETHODCALLTYPE CreateSelectionAction( 
            /* [in] */ __RPC__in_opt IVsTextBuffer *pBuffer,
            /* [out] */ __RPC__deref_out_opt IVsTextSelectionAction **ppAction) = 0;
        
        virtual HRESULT STDMETHODCALLTYPE MapFilenameToLanguageSID( 
            /* [in] */ __RPC__in const WCHAR *pszFileName,
            /* [out] */ __RPC__out GUID *pguidLangSID) = 0;
        
        virtual HRESULT STDMETHODCALLTYPE GetRegisteredMarkerTypeID( 
            /* [in] */ __RPC__in const GUID *pguidMarker,
            /* [out] */ __RPC__out long *piMarkerTypeID) = 0;
        
        virtual HRESULT STDMETHODCALLTYPE GetMarkerTypeInterface( 
            /* [in] */ long iMarkerTypeID,
            /* [out] */ __RPC__deref_out_opt IVsTextMarkerType **ppMarkerType) = 0;
        
        virtual HRESULT STDMETHODCALLTYPE GetMarkerTypeCount( 
            /* [out] */ __RPC__out long *piMarkerTypeCount) = 0;
        
        virtual HRESULT STDMETHODCALLTYPE GetActiveView( 
            /* [in] */ BOOL fMustHaveFocus,
            /* [in] */ __RPC__in_opt IVsTextBuffer *pBuffer,
            /* [out] */ __RPC__deref_out_opt IVsTextView **ppView) = 0;
        
        virtual HRESULT STDMETHODCALLTYPE GetUserPreferences( 
            /* [out] */ __RPC__out VIEWPREFERENCES *pViewPrefs,
            /* [out] */ __RPC__out FRAMEPREFERENCES *pFramePrefs,
            /* [out][in] */ __RPC__inout LANGPREFERENCES *pLangPrefs,
            /* [out][in] */ __RPC__inout FONTCOLORPREFERENCES *pColorPrefs) = 0;
        
        virtual HRESULT STDMETHODCALLTYPE SetUserPreferences( 
            /* [in] */ __RPC__in const VIEWPREFERENCES *pViewPrefs,
            /* [in] */ __RPC__in const FRAMEPREFERENCES *pFramePrefs,
            /* [in] */ __RPC__in const LANGPREFERENCES *pLangPrefs,
            /* [in] */ __RPC__in const FONTCOLORPREFERENCES *pColorPrefs) = 0;
        
        virtual HRESULT STDMETHODCALLTYPE SetFileChangeAdvise( 
            /* [in] */ __RPC__in const WCHAR *pszFileName,
            /* [in] */ BOOL fStart) = 0;
        
        virtual HRESULT STDMETHODCALLTYPE SuspendFileChangeAdvise( 
            /* [in] */ __RPC__in const WCHAR *pszFileName,
            /* [in] */ BOOL fSuspend) = 0;
        
        virtual HRESULT STDMETHODCALLTYPE NavigateToPosition( 
            /* [in] */ __RPC__in_opt IVsTextBuffer *pBuffer,
            /* [in] */ __RPC__in REFGUID guidDocViewType,
            /* [in] */ long iPos,
            /* [in] */ long iLen) = 0;
        
        virtual HRESULT STDMETHODCALLTYPE NavigateToLineAndColumn( 
            /* [in] */ __RPC__in_opt IVsTextBuffer *pBuffer,
            /* [in] */ __RPC__in REFGUID guidDocViewType,
            /* [in] */ long iStartRow,
            /* [in] */ CharIndex iStartIndex,
            /* [in] */ long iEndRow,
            /* [in] */ CharIndex iEndIndex) = 0;
        
        virtual HRESULT STDMETHODCALLTYPE GetBufferSccStatus( 
            /* [in] */ __RPC__in_opt IVsUserData *pBufData,
            /* [out] */ __RPC__out BOOL *pbNonEditable) = 0;
        
        virtual HRESULT STDMETHODCALLTYPE RegisterBuffer( 
            /* [in] */ __RPC__in_opt IVsTextBuffer *pBuffer) = 0;
        
        virtual HRESULT STDMETHODCALLTYPE UnregisterBuffer( 
            /* [in] */ __RPC__in_opt IVsTextBuffer *pBuffer) = 0;
        
        virtual HRESULT STDMETHODCALLTYPE EnumBuffers( 
            /* [out] */ __RPC__deref_out_opt IVsEnumTextBuffers **ppEnum) = 0;
        
        virtual HRESULT STDMETHODCALLTYPE GetPerLanguagePreferences( 
            /* [out] */ __RPC__out LANGPREFERENCES *pLangPrefs) = 0;
        
        virtual HRESULT STDMETHODCALLTYPE SetPerLanguagePreferences( 
            /* [in] */ __RPC__in const LANGPREFERENCES *pLangPrefs) = 0;
        
        virtual HRESULT STDMETHODCALLTYPE AttemptToCheckOutBufferFromScc( 
            /* [in] */ __RPC__in_opt IVsUserData *pBufData,
            /* [out] */ __RPC__out BOOL *pfCheckoutSucceeded) = 0;
        
        virtual HRESULT STDMETHODCALLTYPE GetShortcutManager( 
            /* [out] */ __RPC__deref_out_opt IVsShortcutManager **ppShortcutMgr) = 0;
        
        virtual HRESULT STDMETHODCALLTYPE RegisterIndependentView( 
            /* [in] */ __RPC__in_opt IUnknown *pUnk,
            /* [in] */ __RPC__in_opt IVsTextBuffer *pBuffer) = 0;
        
        virtual HRESULT STDMETHODCALLTYPE UnregisterIndependentView( 
            /* [in] */ __RPC__in_opt IUnknown *pUnk,
            /* [in] */ __RPC__in_opt IVsTextBuffer *pBuffer) = 0;
        
        virtual HRESULT STDMETHODCALLTYPE IgnoreNextFileChange( 
            /* [in] */ __RPC__in_opt IVsTextBuffer *pBuffer) = 0;
        
        virtual HRESULT STDMETHODCALLTYPE AdjustFileChangeIgnoreCount( 
            /* [in] */ __RPC__in_opt IVsTextBuffer *pBuffer,
            /* [in] */ BOOL fIgnore) = 0;
        
        virtual HRESULT STDMETHODCALLTYPE GetBufferSccStatus2( 
            /* [in] */ __RPC__in const WCHAR *pszFileName,
            /* [out] */ __RPC__out BOOL *pbNonEditable,
            /* [out] */ __RPC__out int *piStatusFlags) = 0;
        
        virtual HRESULT STDMETHODCALLTYPE AttemptToCheckOutBufferFromScc2( 
            /* [in] */ __RPC__in const WCHAR *pszFileName,
            /* [out] */ __RPC__out BOOL *pfCheckoutSucceeded,
            /* [out] */ __RPC__out int *piStatusFlags) = 0;
        
        virtual HRESULT STDMETHODCALLTYPE EnumLanguageServices( 
            /* [out] */ __RPC__deref_out_opt IVsEnumGUID **ppEnum) = 0;
        
        virtual HRESULT STDMETHODCALLTYPE EnumIndependentViews( 
            /* [in] */ __RPC__in_opt IVsTextBuffer *pBuffer,
            /* [out] */ __RPC__deref_out_opt IVsEnumIndependentViews **ppEnum) = 0;
        
    };
    
#else 	/* C style interface */

    typedef struct IVsTextManagerVtbl
    {
        BEGIN_INTERFACE
        
        HRESULT ( STDMETHODCALLTYPE *QueryInterface )( 
            IVsTextManager * This,
            /* [in] */ __RPC__in REFIID riid,
            /* [iid_is][out] */ 
            __RPC__deref_out  void **ppvObject);
        
        ULONG ( STDMETHODCALLTYPE *AddRef )( 
            IVsTextManager * This);
        
        ULONG ( STDMETHODCALLTYPE *Release )( 
            IVsTextManager * This);
        
        HRESULT ( STDMETHODCALLTYPE *RegisterView )( 
            IVsTextManager * This,
            /* [in] */ __RPC__in_opt IVsTextView *pView,
            /* [in] */ __RPC__in_opt IVsTextBuffer *pBuffer);
        
        HRESULT ( STDMETHODCALLTYPE *UnregisterView )( 
            IVsTextManager * This,
            /* [in] */ __RPC__in_opt IVsTextView *pView);
        
        HRESULT ( STDMETHODCALLTYPE *EnumViews )( 
            IVsTextManager * This,
            /* [in] */ __RPC__in_opt IVsTextBuffer *pBuffer,
            /* [out] */ __RPC__deref_out_opt IVsEnumTextViews **ppEnum);
        
        HRESULT ( STDMETHODCALLTYPE *CreateSelectionAction )( 
            IVsTextManager * This,
            /* [in] */ __RPC__in_opt IVsTextBuffer *pBuffer,
            /* [out] */ __RPC__deref_out_opt IVsTextSelectionAction **ppAction);
        
        HRESULT ( STDMETHODCALLTYPE *MapFilenameToLanguageSID )( 
            IVsTextManager * This,
            /* [in] */ __RPC__in const WCHAR *pszFileName,
            /* [out] */ __RPC__out GUID *pguidLangSID);
        
        HRESULT ( STDMETHODCALLTYPE *GetRegisteredMarkerTypeID )( 
            IVsTextManager * This,
            /* [in] */ __RPC__in const GUID *pguidMarker,
            /* [out] */ __RPC__out long *piMarkerTypeID);
        
        HRESULT ( STDMETHODCALLTYPE *GetMarkerTypeInterface )( 
            IVsTextManager * This,
            /* [in] */ long iMarkerTypeID,
            /* [out] */ __RPC__deref_out_opt IVsTextMarkerType **ppMarkerType);
        
        HRESULT ( STDMETHODCALLTYPE *GetMarkerTypeCount )( 
            IVsTextManager * This,
            /* [out] */ __RPC__out long *piMarkerTypeCount);
        
        HRESULT ( STDMETHODCALLTYPE *GetActiveView )( 
            IVsTextManager * This,
            /* [in] */ BOOL fMustHaveFocus,
            /* [in] */ __RPC__in_opt IVsTextBuffer *pBuffer,
            /* [out] */ __RPC__deref_out_opt IVsTextView **ppView);
        
        HRESULT ( STDMETHODCALLTYPE *GetUserPreferences )( 
            IVsTextManager * This,
            /* [out] */ __RPC__out VIEWPREFERENCES *pViewPrefs,
            /* [out] */ __RPC__out FRAMEPREFERENCES *pFramePrefs,
            /* [out][in] */ __RPC__inout LANGPREFERENCES *pLangPrefs,
            /* [out][in] */ __RPC__inout FONTCOLORPREFERENCES *pColorPrefs);
        
        HRESULT ( STDMETHODCALLTYPE *SetUserPreferences )( 
            IVsTextManager * This,
            /* [in] */ __RPC__in const VIEWPREFERENCES *pViewPrefs,
            /* [in] */ __RPC__in const FRAMEPREFERENCES *pFramePrefs,
            /* [in] */ __RPC__in const LANGPREFERENCES *pLangPrefs,
            /* [in] */ __RPC__in const FONTCOLORPREFERENCES *pColorPrefs);
        
        HRESULT ( STDMETHODCALLTYPE *SetFileChangeAdvise )( 
            IVsTextManager * This,
            /* [in] */ __RPC__in const WCHAR *pszFileName,
            /* [in] */ BOOL fStart);
        
        HRESULT ( STDMETHODCALLTYPE *SuspendFileChangeAdvise )( 
            IVsTextManager * This,
            /* [in] */ __RPC__in const WCHAR *pszFileName,
            /* [in] */ BOOL fSuspend);
        
        HRESULT ( STDMETHODCALLTYPE *NavigateToPosition )( 
            IVsTextManager * This,
            /* [in] */ __RPC__in_opt IVsTextBuffer *pBuffer,
            /* [in] */ __RPC__in REFGUID guidDocViewType,
            /* [in] */ long iPos,
            /* [in] */ long iLen);
        
        HRESULT ( STDMETHODCALLTYPE *NavigateToLineAndColumn )( 
            IVsTextManager * This,
            /* [in] */ __RPC__in_opt IVsTextBuffer *pBuffer,
            /* [in] */ __RPC__in REFGUID guidDocViewType,
            /* [in] */ long iStartRow,
            /* [in] */ CharIndex iStartIndex,
            /* [in] */ long iEndRow,
            /* [in] */ CharIndex iEndIndex);
        
        HRESULT ( STDMETHODCALLTYPE *GetBufferSccStatus )( 
            IVsTextManager * This,
            /* [in] */ __RPC__in_opt IVsUserData *pBufData,
            /* [out] */ __RPC__out BOOL *pbNonEditable);
        
        HRESULT ( STDMETHODCALLTYPE *RegisterBuffer )( 
            IVsTextManager * This,
            /* [in] */ __RPC__in_opt IVsTextBuffer *pBuffer);
        
        HRESULT ( STDMETHODCALLTYPE *UnregisterBuffer )( 
            IVsTextManager * This,
            /* [in] */ __RPC__in_opt IVsTextBuffer *pBuffer);
        
        HRESULT ( STDMETHODCALLTYPE *EnumBuffers )( 
            IVsTextManager * This,
            /* [out] */ __RPC__deref_out_opt IVsEnumTextBuffers **ppEnum);
        
        HRESULT ( STDMETHODCALLTYPE *GetPerLanguagePreferences )( 
            IVsTextManager * This,
            /* [out] */ __RPC__out LANGPREFERENCES *pLangPrefs);
        
        HRESULT ( STDMETHODCALLTYPE *SetPerLanguagePreferences )( 
            IVsTextManager * This,
            /* [in] */ __RPC__in const LANGPREFERENCES *pLangPrefs);
        
        HRESULT ( STDMETHODCALLTYPE *AttemptToCheckOutBufferFromScc )( 
            IVsTextManager * This,
            /* [in] */ __RPC__in_opt IVsUserData *pBufData,
            /* [out] */ __RPC__out BOOL *pfCheckoutSucceeded);
        
        HRESULT ( STDMETHODCALLTYPE *GetShortcutManager )( 
            IVsTextManager * This,
            /* [out] */ __RPC__deref_out_opt IVsShortcutManager **ppShortcutMgr);
        
        HRESULT ( STDMETHODCALLTYPE *RegisterIndependentView )( 
            IVsTextManager * This,
            /* [in] */ __RPC__in_opt IUnknown *pUnk,
            /* [in] */ __RPC__in_opt IVsTextBuffer *pBuffer);
        
        HRESULT ( STDMETHODCALLTYPE *UnregisterIndependentView )( 
            IVsTextManager * This,
            /* [in] */ __RPC__in_opt IUnknown *pUnk,
            /* [in] */ __RPC__in_opt IVsTextBuffer *pBuffer);
        
        HRESULT ( STDMETHODCALLTYPE *IgnoreNextFileChange )( 
            IVsTextManager * This,
            /* [in] */ __RPC__in_opt IVsTextBuffer *pBuffer);
        
        HRESULT ( STDMETHODCALLTYPE *AdjustFileChangeIgnoreCount )( 
            IVsTextManager * This,
            /* [in] */ __RPC__in_opt IVsTextBuffer *pBuffer,
            /* [in] */ BOOL fIgnore);
        
        HRESULT ( STDMETHODCALLTYPE *GetBufferSccStatus2 )( 
            IVsTextManager * This,
            /* [in] */ __RPC__in const WCHAR *pszFileName,
            /* [out] */ __RPC__out BOOL *pbNonEditable,
            /* [out] */ __RPC__out int *piStatusFlags);
        
        HRESULT ( STDMETHODCALLTYPE *AttemptToCheckOutBufferFromScc2 )( 
            IVsTextManager * This,
            /* [in] */ __RPC__in const WCHAR *pszFileName,
            /* [out] */ __RPC__out BOOL *pfCheckoutSucceeded,
            /* [out] */ __RPC__out int *piStatusFlags);
        
        HRESULT ( STDMETHODCALLTYPE *EnumLanguageServices )( 
            IVsTextManager * This,
            /* [out] */ __RPC__deref_out_opt IVsEnumGUID **ppEnum);
        
        HRESULT ( STDMETHODCALLTYPE *EnumIndependentViews )( 
            IVsTextManager * This,
            /* [in] */ __RPC__in_opt IVsTextBuffer *pBuffer,
            /* [out] */ __RPC__deref_out_opt IVsEnumIndependentViews **ppEnum);
        
        END_INTERFACE
    } IVsTextManagerVtbl;

    interface IVsTextManager
    {
        CONST_VTBL struct IVsTextManagerVtbl *lpVtbl;
    };

    

#ifdef COBJMACROS


#define IVsTextManager_QueryInterface(This,riid,ppvObject)	\
    ( (This)->lpVtbl -> QueryInterface(This,riid,ppvObject) ) 

#define IVsTextManager_AddRef(This)	\
    ( (This)->lpVtbl -> AddRef(This) ) 

#define IVsTextManager_Release(This)	\
    ( (This)->lpVtbl -> Release(This) ) 


#define IVsTextManager_RegisterView(This,pView,pBuffer)	\
    ( (This)->lpVtbl -> RegisterView(This,pView,pBuffer) ) 

#define IVsTextManager_UnregisterView(This,pView)	\
    ( (This)->lpVtbl -> UnregisterView(This,pView) ) 

#define IVsTextManager_EnumViews(This,pBuffer,ppEnum)	\
    ( (This)->lpVtbl -> EnumViews(This,pBuffer,ppEnum) ) 

#define IVsTextManager_CreateSelectionAction(This,pBuffer,ppAction)	\
    ( (This)->lpVtbl -> CreateSelectionAction(This,pBuffer,ppAction) ) 

#define IVsTextManager_MapFilenameToLanguageSID(This,pszFileName,pguidLangSID)	\
    ( (This)->lpVtbl -> MapFilenameToLanguageSID(This,pszFileName,pguidLangSID) ) 

#define IVsTextManager_GetRegisteredMarkerTypeID(This,pguidMarker,piMarkerTypeID)	\
    ( (This)->lpVtbl -> GetRegisteredMarkerTypeID(This,pguidMarker,piMarkerTypeID) ) 

#define IVsTextManager_GetMarkerTypeInterface(This,iMarkerTypeID,ppMarkerType)	\
    ( (This)->lpVtbl -> GetMarkerTypeInterface(This,iMarkerTypeID,ppMarkerType) ) 

#define IVsTextManager_GetMarkerTypeCount(This,piMarkerTypeCount)	\
    ( (This)->lpVtbl -> GetMarkerTypeCount(This,piMarkerTypeCount) ) 

#define IVsTextManager_GetActiveView(This,fMustHaveFocus,pBuffer,ppView)	\
    ( (This)->lpVtbl -> GetActiveView(This,fMustHaveFocus,pBuffer,ppView) ) 

#define IVsTextManager_GetUserPreferences(This,pViewPrefs,pFramePrefs,pLangPrefs,pColorPrefs)	\
    ( (This)->lpVtbl -> GetUserPreferences(This,pViewPrefs,pFramePrefs,pLangPrefs,pColorPrefs) ) 

#define IVsTextManager_SetUserPreferences(This,pViewPrefs,pFramePrefs,pLangPrefs,pColorPrefs)	\
    ( (This)->lpVtbl -> SetUserPreferences(This,pViewPrefs,pFramePrefs,pLangPrefs,pColorPrefs) ) 

#define IVsTextManager_SetFileChangeAdvise(This,pszFileName,fStart)	\
    ( (This)->lpVtbl -> SetFileChangeAdvise(This,pszFileName,fStart) ) 

#define IVsTextManager_SuspendFileChangeAdvise(This,pszFileName,fSuspend)	\
    ( (This)->lpVtbl -> SuspendFileChangeAdvise(This,pszFileName,fSuspend) ) 

#define IVsTextManager_NavigateToPosition(This,pBuffer,guidDocViewType,iPos,iLen)	\
    ( (This)->lpVtbl -> NavigateToPosition(This,pBuffer,guidDocViewType,iPos,iLen) ) 

#define IVsTextManager_NavigateToLineAndColumn(This,pBuffer,guidDocViewType,iStartRow,iStartIndex,iEndRow,iEndIndex)	\
    ( (This)->lpVtbl -> NavigateToLineAndColumn(This,pBuffer,guidDocViewType,iStartRow,iStartIndex,iEndRow,iEndIndex) ) 

#define IVsTextManager_GetBufferSccStatus(This,pBufData,pbNonEditable)	\
    ( (This)->lpVtbl -> GetBufferSccStatus(This,pBufData,pbNonEditable) ) 

#define IVsTextManager_RegisterBuffer(This,pBuffer)	\
    ( (This)->lpVtbl -> RegisterBuffer(This,pBuffer) ) 

#define IVsTextManager_UnregisterBuffer(This,pBuffer)	\
    ( (This)->lpVtbl -> UnregisterBuffer(This,pBuffer) ) 

#define IVsTextManager_EnumBuffers(This,ppEnum)	\
    ( (This)->lpVtbl -> EnumBuffers(This,ppEnum) ) 

#define IVsTextManager_GetPerLanguagePreferences(This,pLangPrefs)	\
    ( (This)->lpVtbl -> GetPerLanguagePreferences(This,pLangPrefs) ) 

#define IVsTextManager_SetPerLanguagePreferences(This,pLangPrefs)	\
    ( (This)->lpVtbl -> SetPerLanguagePreferences(This,pLangPrefs) ) 

#define IVsTextManager_AttemptToCheckOutBufferFromScc(This,pBufData,pfCheckoutSucceeded)	\
    ( (This)->lpVtbl -> AttemptToCheckOutBufferFromScc(This,pBufData,pfCheckoutSucceeded) ) 

#define IVsTextManager_GetShortcutManager(This,ppShortcutMgr)	\
    ( (This)->lpVtbl -> GetShortcutManager(This,ppShortcutMgr) ) 

#define IVsTextManager_RegisterIndependentView(This,pUnk,pBuffer)	\
    ( (This)->lpVtbl -> RegisterIndependentView(This,pUnk,pBuffer) ) 

#define IVsTextManager_UnregisterIndependentView(This,pUnk,pBuffer)	\
    ( (This)->lpVtbl -> UnregisterIndependentView(This,pUnk,pBuffer) ) 

#define IVsTextManager_IgnoreNextFileChange(This,pBuffer)	\
    ( (This)->lpVtbl -> IgnoreNextFileChange(This,pBuffer) ) 

#define IVsTextManager_AdjustFileChangeIgnoreCount(This,pBuffer,fIgnore)	\
    ( (This)->lpVtbl -> AdjustFileChangeIgnoreCount(This,pBuffer,fIgnore) ) 

#define IVsTextManager_GetBufferSccStatus2(This,pszFileName,pbNonEditable,piStatusFlags)	\
    ( (This)->lpVtbl -> GetBufferSccStatus2(This,pszFileName,pbNonEditable,piStatusFlags) ) 

#define IVsTextManager_AttemptToCheckOutBufferFromScc2(This,pszFileName,pfCheckoutSucceeded,piStatusFlags)	\
    ( (This)->lpVtbl -> AttemptToCheckOutBufferFromScc2(This,pszFileName,pfCheckoutSucceeded,piStatusFlags) ) 

#define IVsTextManager_EnumLanguageServices(This,ppEnum)	\
    ( (This)->lpVtbl -> EnumLanguageServices(This,ppEnum) ) 

#define IVsTextManager_EnumIndependentViews(This,pBuffer,ppEnum)	\
    ( (This)->lpVtbl -> EnumIndependentViews(This,pBuffer,ppEnum) ) 

#endif /* COBJMACROS */


#endif 	/* C style interface */




#endif 	/* __IVsTextManager_INTERFACE_DEFINED__ */


#ifndef __IVsShortcutManager_INTERFACE_DEFINED__
#define __IVsShortcutManager_INTERFACE_DEFINED__

/* interface IVsShortcutManager */
/* [object][uuid] */ 


EXTERN_C const IID IID_IVsShortcutManager;

#if defined(__cplusplus) && !defined(CINTERFACE)
    
    MIDL_INTERFACE("78201465-554E-4A7B-998F-443156FD71D3")
    IVsShortcutManager : public IUnknown
    {
    public:
        virtual HRESULT STDMETHODCALLTYPE CreateItem( 
            /* [in] */ long iShortcutLine,
            /* [in] */ __RPC__in_opt IVsTextLines *pBuffer,
            /* [in] */ __RPC__in LPCOLESTR pszBufMoniker) = 0;
        
        virtual HRESULT STDMETHODCALLTYPE RemoveItem( 
            /* [in] */ __RPC__in_opt IVsTextLineMarker *pMarker) = 0;
        
        virtual HRESULT STDMETHODCALLTYPE WriteUserOptions( 
            /* [in] */ __RPC__in_opt IStream *pOptionsStream) = 0;
        
        virtual HRESULT STDMETHODCALLTYPE ReadUserOptions( 
            /* [in] */ __RPC__in_opt IStream *pOptionsStream) = 0;
        
    };
    
#else 	/* C style interface */

    typedef struct IVsShortcutManagerVtbl
    {
        BEGIN_INTERFACE
        
        HRESULT ( STDMETHODCALLTYPE *QueryInterface )( 
            IVsShortcutManager * This,
            /* [in] */ __RPC__in REFIID riid,
            /* [iid_is][out] */ 
            __RPC__deref_out  void **ppvObject);
        
        ULONG ( STDMETHODCALLTYPE *AddRef )( 
            IVsShortcutManager * This);
        
        ULONG ( STDMETHODCALLTYPE *Release )( 
            IVsShortcutManager * This);
        
        HRESULT ( STDMETHODCALLTYPE *CreateItem )( 
            IVsShortcutManager * This,
            /* [in] */ long iShortcutLine,
            /* [in] */ __RPC__in_opt IVsTextLines *pBuffer,
            /* [in] */ __RPC__in LPCOLESTR pszBufMoniker);
        
        HRESULT ( STDMETHODCALLTYPE *RemoveItem )( 
            IVsShortcutManager * This,
            /* [in] */ __RPC__in_opt IVsTextLineMarker *pMarker);
        
        HRESULT ( STDMETHODCALLTYPE *WriteUserOptions )( 
            IVsShortcutManager * This,
            /* [in] */ __RPC__in_opt IStream *pOptionsStream);
        
        HRESULT ( STDMETHODCALLTYPE *ReadUserOptions )( 
            IVsShortcutManager * This,
            /* [in] */ __RPC__in_opt IStream *pOptionsStream);
        
        END_INTERFACE
    } IVsShortcutManagerVtbl;

    interface IVsShortcutManager
    {
        CONST_VTBL struct IVsShortcutManagerVtbl *lpVtbl;
    };

    

#ifdef COBJMACROS


#define IVsShortcutManager_QueryInterface(This,riid,ppvObject)	\
    ( (This)->lpVtbl -> QueryInterface(This,riid,ppvObject) ) 

#define IVsShortcutManager_AddRef(This)	\
    ( (This)->lpVtbl -> AddRef(This) ) 

#define IVsShortcutManager_Release(This)	\
    ( (This)->lpVtbl -> Release(This) ) 


#define IVsShortcutManager_CreateItem(This,iShortcutLine,pBuffer,pszBufMoniker)	\
    ( (This)->lpVtbl -> CreateItem(This,iShortcutLine,pBuffer,pszBufMoniker) ) 

#define IVsShortcutManager_RemoveItem(This,pMarker)	\
    ( (This)->lpVtbl -> RemoveItem(This,pMarker) ) 

#define IVsShortcutManager_WriteUserOptions(This,pOptionsStream)	\
    ( (This)->lpVtbl -> WriteUserOptions(This,pOptionsStream) ) 

#define IVsShortcutManager_ReadUserOptions(This,pOptionsStream)	\
    ( (This)->lpVtbl -> ReadUserOptions(This,pOptionsStream) ) 

#endif /* COBJMACROS */


#endif 	/* C style interface */




#endif 	/* __IVsShortcutManager_INTERFACE_DEFINED__ */


#ifndef __IVsEnumTextBuffers_INTERFACE_DEFINED__
#define __IVsEnumTextBuffers_INTERFACE_DEFINED__

/* interface IVsEnumTextBuffers */
/* [object][uuid] */ 


EXTERN_C const IID IID_IVsEnumTextBuffers;

#if defined(__cplusplus) && !defined(CINTERFACE)
    
    MIDL_INTERFACE("74BBEE88-AB6E-4276-9BD2-27F566B8BBAF")
    IVsEnumTextBuffers : public IUnknown
    {
    public:
        virtual HRESULT STDMETHODCALLTYPE Next( 
            ULONG celt,
            /* [length_is][size_is][out] */ __RPC__out_ecount_part(celt, *pceltFetched) IVsTextBuffer **rgelt,
            __RPC__in ULONG *pceltFetched) = 0;
        
        virtual HRESULT STDMETHODCALLTYPE Skip( 
            ULONG celt) = 0;
        
        virtual HRESULT STDMETHODCALLTYPE Reset( void) = 0;
        
        virtual HRESULT STDMETHODCALLTYPE Clone( 
            __RPC__deref_in_opt IVsEnumTextBuffers **ppenum) = 0;
        
    };
    
#else 	/* C style interface */

    typedef struct IVsEnumTextBuffersVtbl
    {
        BEGIN_INTERFACE
        
        HRESULT ( STDMETHODCALLTYPE *QueryInterface )( 
            IVsEnumTextBuffers * This,
            /* [in] */ __RPC__in REFIID riid,
            /* [iid_is][out] */ 
            __RPC__deref_out  void **ppvObject);
        
        ULONG ( STDMETHODCALLTYPE *AddRef )( 
            IVsEnumTextBuffers * This);
        
        ULONG ( STDMETHODCALLTYPE *Release )( 
            IVsEnumTextBuffers * This);
        
        HRESULT ( STDMETHODCALLTYPE *Next )( 
            IVsEnumTextBuffers * This,
            ULONG celt,
            /* [length_is][size_is][out] */ __RPC__out_ecount_part(celt, *pceltFetched) IVsTextBuffer **rgelt,
            __RPC__in ULONG *pceltFetched);
        
        HRESULT ( STDMETHODCALLTYPE *Skip )( 
            IVsEnumTextBuffers * This,
            ULONG celt);
        
        HRESULT ( STDMETHODCALLTYPE *Reset )( 
            IVsEnumTextBuffers * This);
        
        HRESULT ( STDMETHODCALLTYPE *Clone )( 
            IVsEnumTextBuffers * This,
            __RPC__deref_in_opt IVsEnumTextBuffers **ppenum);
        
        END_INTERFACE
    } IVsEnumTextBuffersVtbl;

    interface IVsEnumTextBuffers
    {
        CONST_VTBL struct IVsEnumTextBuffersVtbl *lpVtbl;
    };

    

#ifdef COBJMACROS


#define IVsEnumTextBuffers_QueryInterface(This,riid,ppvObject)	\
    ( (This)->lpVtbl -> QueryInterface(This,riid,ppvObject) ) 

#define IVsEnumTextBuffers_AddRef(This)	\
    ( (This)->lpVtbl -> AddRef(This) ) 

#define IVsEnumTextBuffers_Release(This)	\
    ( (This)->lpVtbl -> Release(This) ) 


#define IVsEnumTextBuffers_Next(This,celt,rgelt,pceltFetched)	\
    ( (This)->lpVtbl -> Next(This,celt,rgelt,pceltFetched) ) 

#define IVsEnumTextBuffers_Skip(This,celt)	\
    ( (This)->lpVtbl -> Skip(This,celt) ) 

#define IVsEnumTextBuffers_Reset(This)	\
    ( (This)->lpVtbl -> Reset(This) ) 

#define IVsEnumTextBuffers_Clone(This,ppenum)	\
    ( (This)->lpVtbl -> Clone(This,ppenum) ) 

#endif /* COBJMACROS */


#endif 	/* C style interface */




#endif 	/* __IVsEnumTextBuffers_INTERFACE_DEFINED__ */


#ifndef __IVsTextManagerEvents_INTERFACE_DEFINED__
#define __IVsTextManagerEvents_INTERFACE_DEFINED__

/* interface IVsTextManagerEvents */
/* [object][uuid] */ 


EXTERN_C const IID IID_IVsTextManagerEvents;

#if defined(__cplusplus) && !defined(CINTERFACE)
    
    MIDL_INTERFACE("B3175059-C6AA-4519-86C3-1FE594D26C35")
    IVsTextManagerEvents : public IUnknown
    {
    public:
        virtual void STDMETHODCALLTYPE OnRegisterMarkerType( 
            /* [in] */ long iMarkerType) = 0;
        
        virtual void STDMETHODCALLTYPE OnRegisterView( 
            /* [in] */ __RPC__in_opt IVsTextView *pView) = 0;
        
        virtual void STDMETHODCALLTYPE OnUnregisterView( 
            /* [in] */ __RPC__in_opt IVsTextView *pView) = 0;
        
        virtual void STDMETHODCALLTYPE OnUserPreferencesChanged( 
            /* [in] */ __RPC__in const VIEWPREFERENCES *pViewPrefs,
            /* [in] */ __RPC__in const FRAMEPREFERENCES *pFramePrefs,
            /* [in] */ __RPC__in const LANGPREFERENCES *pLangPrefs,
            /* [in] */ __RPC__in const FONTCOLORPREFERENCES *pColorPrefs) = 0;
        
    };
    
#else 	/* C style interface */

    typedef struct IVsTextManagerEventsVtbl
    {
        BEGIN_INTERFACE
        
        HRESULT ( STDMETHODCALLTYPE *QueryInterface )( 
            IVsTextManagerEvents * This,
            /* [in] */ __RPC__in REFIID riid,
            /* [iid_is][out] */ 
            __RPC__deref_out  void **ppvObject);
        
        ULONG ( STDMETHODCALLTYPE *AddRef )( 
            IVsTextManagerEvents * This);
        
        ULONG ( STDMETHODCALLTYPE *Release )( 
            IVsTextManagerEvents * This);
        
        void ( STDMETHODCALLTYPE *OnRegisterMarkerType )( 
            IVsTextManagerEvents * This,
            /* [in] */ long iMarkerType);
        
        void ( STDMETHODCALLTYPE *OnRegisterView )( 
            IVsTextManagerEvents * This,
            /* [in] */ __RPC__in_opt IVsTextView *pView);
        
        void ( STDMETHODCALLTYPE *OnUnregisterView )( 
            IVsTextManagerEvents * This,
            /* [in] */ __RPC__in_opt IVsTextView *pView);
        
        void ( STDMETHODCALLTYPE *OnUserPreferencesChanged )( 
            IVsTextManagerEvents * This,
            /* [in] */ __RPC__in const VIEWPREFERENCES *pViewPrefs,
            /* [in] */ __RPC__in const FRAMEPREFERENCES *pFramePrefs,
            /* [in] */ __RPC__in const LANGPREFERENCES *pLangPrefs,
            /* [in] */ __RPC__in const FONTCOLORPREFERENCES *pColorPrefs);
        
        END_INTERFACE
    } IVsTextManagerEventsVtbl;

    interface IVsTextManagerEvents
    {
        CONST_VTBL struct IVsTextManagerEventsVtbl *lpVtbl;
    };

    

#ifdef COBJMACROS


#define IVsTextManagerEvents_QueryInterface(This,riid,ppvObject)	\
    ( (This)->lpVtbl -> QueryInterface(This,riid,ppvObject) ) 

#define IVsTextManagerEvents_AddRef(This)	\
    ( (This)->lpVtbl -> AddRef(This) ) 

#define IVsTextManagerEvents_Release(This)	\
    ( (This)->lpVtbl -> Release(This) ) 


#define IVsTextManagerEvents_OnRegisterMarkerType(This,iMarkerType)	\
    ( (This)->lpVtbl -> OnRegisterMarkerType(This,iMarkerType) ) 

#define IVsTextManagerEvents_OnRegisterView(This,pView)	\
    ( (This)->lpVtbl -> OnRegisterView(This,pView) ) 

#define IVsTextManagerEvents_OnUnregisterView(This,pView)	\
    ( (This)->lpVtbl -> OnUnregisterView(This,pView) ) 

#define IVsTextManagerEvents_OnUserPreferencesChanged(This,pViewPrefs,pFramePrefs,pLangPrefs,pColorPrefs)	\
    ( (This)->lpVtbl -> OnUserPreferencesChanged(This,pViewPrefs,pFramePrefs,pLangPrefs,pColorPrefs) ) 

#endif /* COBJMACROS */


#endif 	/* C style interface */




#endif 	/* __IVsTextManagerEvents_INTERFACE_DEFINED__ */


#ifndef __IVsTextSelectionAction_INTERFACE_DEFINED__
#define __IVsTextSelectionAction_INTERFACE_DEFINED__

/* interface IVsTextSelectionAction */
/* [object][uuid] */ 


EXTERN_C const IID IID_IVsTextSelectionAction;

#if defined(__cplusplus) && !defined(CINTERFACE)
    
    MIDL_INTERFACE("CB49CF53-CCE9-4493-AD7A-3595F4C1CAF3")
    IVsTextSelectionAction : public IUnknown
    {
    public:
        virtual HRESULT STDMETHODCALLTYPE GetOrigin( 
            /* [out] */ __RPC__out SELECTIONSTATE *pSelState) = 0;
        
        virtual HRESULT STDMETHODCALLTYPE GetDestination( 
            /* [out] */ __RPC__out SELECTIONSTATE *pSelState) = 0;
        
        virtual HRESULT STDMETHODCALLTYPE SetOrigin( 
            /* [in] */ __RPC__in SELECTIONSTATE *pSelState) = 0;
        
        virtual HRESULT STDMETHODCALLTYPE SetDestination( 
            /* [in] */ __RPC__in SELECTIONSTATE *pSelState) = 0;
        
    };
    
#else 	/* C style interface */

    typedef struct IVsTextSelectionActionVtbl
    {
        BEGIN_INTERFACE
        
        HRESULT ( STDMETHODCALLTYPE *QueryInterface )( 
            IVsTextSelectionAction * This,
            /* [in] */ __RPC__in REFIID riid,
            /* [iid_is][out] */ 
            __RPC__deref_out  void **ppvObject);
        
        ULONG ( STDMETHODCALLTYPE *AddRef )( 
            IVsTextSelectionAction * This);
        
        ULONG ( STDMETHODCALLTYPE *Release )( 
            IVsTextSelectionAction * This);
        
        HRESULT ( STDMETHODCALLTYPE *GetOrigin )( 
            IVsTextSelectionAction * This,
            /* [out] */ __RPC__out SELECTIONSTATE *pSelState);
        
        HRESULT ( STDMETHODCALLTYPE *GetDestination )( 
            IVsTextSelectionAction * This,
            /* [out] */ __RPC__out SELECTIONSTATE *pSelState);
        
        HRESULT ( STDMETHODCALLTYPE *SetOrigin )( 
            IVsTextSelectionAction * This,
            /* [in] */ __RPC__in SELECTIONSTATE *pSelState);
        
        HRESULT ( STDMETHODCALLTYPE *SetDestination )( 
            IVsTextSelectionAction * This,
            /* [in] */ __RPC__in SELECTIONSTATE *pSelState);
        
        END_INTERFACE
    } IVsTextSelectionActionVtbl;

    interface IVsTextSelectionAction
    {
        CONST_VTBL struct IVsTextSelectionActionVtbl *lpVtbl;
    };

    

#ifdef COBJMACROS


#define IVsTextSelectionAction_QueryInterface(This,riid,ppvObject)	\
    ( (This)->lpVtbl -> QueryInterface(This,riid,ppvObject) ) 

#define IVsTextSelectionAction_AddRef(This)	\
    ( (This)->lpVtbl -> AddRef(This) ) 

#define IVsTextSelectionAction_Release(This)	\
    ( (This)->lpVtbl -> Release(This) ) 


#define IVsTextSelectionAction_GetOrigin(This,pSelState)	\
    ( (This)->lpVtbl -> GetOrigin(This,pSelState) ) 

#define IVsTextSelectionAction_GetDestination(This,pSelState)	\
    ( (This)->lpVtbl -> GetDestination(This,pSelState) ) 

#define IVsTextSelectionAction_SetOrigin(This,pSelState)	\
    ( (This)->lpVtbl -> SetOrigin(This,pSelState) ) 

#define IVsTextSelectionAction_SetDestination(This,pSelState)	\
    ( (This)->lpVtbl -> SetDestination(This,pSelState) ) 

#endif /* COBJMACROS */


#endif 	/* C style interface */




#endif 	/* __IVsTextSelectionAction_INTERFACE_DEFINED__ */


#ifndef __IVsEnumTextViews_INTERFACE_DEFINED__
#define __IVsEnumTextViews_INTERFACE_DEFINED__

/* interface IVsEnumTextViews */
/* [object][uuid] */ 


EXTERN_C const IID IID_IVsEnumTextViews;

#if defined(__cplusplus) && !defined(CINTERFACE)
    
    MIDL_INTERFACE("DB685578-E6D4-4D5E-B714-FA344A509ABF")
    IVsEnumTextViews : public IUnknown
    {
    public:
        virtual HRESULT STDMETHODCALLTYPE Next( 
            ULONG celt,
            /* [length_is][size_is][out] */ __RPC__out_ecount_part(celt, *pceltFetched) IVsTextView **rgelt,
            __RPC__in ULONG *pceltFetched) = 0;
        
        virtual HRESULT STDMETHODCALLTYPE Reset( void) = 0;
        
        virtual HRESULT STDMETHODCALLTYPE GetCount( 
            __RPC__in ULONG *pCount) = 0;
        
    };
    
#else 	/* C style interface */

    typedef struct IVsEnumTextViewsVtbl
    {
        BEGIN_INTERFACE
        
        HRESULT ( STDMETHODCALLTYPE *QueryInterface )( 
            IVsEnumTextViews * This,
            /* [in] */ __RPC__in REFIID riid,
            /* [iid_is][out] */ 
            __RPC__deref_out  void **ppvObject);
        
        ULONG ( STDMETHODCALLTYPE *AddRef )( 
            IVsEnumTextViews * This);
        
        ULONG ( STDMETHODCALLTYPE *Release )( 
            IVsEnumTextViews * This);
        
        HRESULT ( STDMETHODCALLTYPE *Next )( 
            IVsEnumTextViews * This,
            ULONG celt,
            /* [length_is][size_is][out] */ __RPC__out_ecount_part(celt, *pceltFetched) IVsTextView **rgelt,
            __RPC__in ULONG *pceltFetched);
        
        HRESULT ( STDMETHODCALLTYPE *Reset )( 
            IVsEnumTextViews * This);
        
        HRESULT ( STDMETHODCALLTYPE *GetCount )( 
            IVsEnumTextViews * This,
            __RPC__in ULONG *pCount);
        
        END_INTERFACE
    } IVsEnumTextViewsVtbl;

    interface IVsEnumTextViews
    {
        CONST_VTBL struct IVsEnumTextViewsVtbl *lpVtbl;
    };

    

#ifdef COBJMACROS


#define IVsEnumTextViews_QueryInterface(This,riid,ppvObject)	\
    ( (This)->lpVtbl -> QueryInterface(This,riid,ppvObject) ) 

#define IVsEnumTextViews_AddRef(This)	\
    ( (This)->lpVtbl -> AddRef(This) ) 

#define IVsEnumTextViews_Release(This)	\
    ( (This)->lpVtbl -> Release(This) ) 


#define IVsEnumTextViews_Next(This,celt,rgelt,pceltFetched)	\
    ( (This)->lpVtbl -> Next(This,celt,rgelt,pceltFetched) ) 

#define IVsEnumTextViews_Reset(This)	\
    ( (This)->lpVtbl -> Reset(This) ) 

#define IVsEnumTextViews_GetCount(This,pCount)	\
    ( (This)->lpVtbl -> GetCount(This,pCount) ) 

#endif /* COBJMACROS */


#endif 	/* C style interface */




#endif 	/* __IVsEnumTextViews_INTERFACE_DEFINED__ */


#ifndef __IVsEnumIndependentViews_INTERFACE_DEFINED__
#define __IVsEnumIndependentViews_INTERFACE_DEFINED__

/* interface IVsEnumIndependentViews */
/* [object][uuid] */ 


EXTERN_C const IID IID_IVsEnumIndependentViews;

#if defined(__cplusplus) && !defined(CINTERFACE)
    
    MIDL_INTERFACE("B9040F95-DADB-443a-A3C3-1C442F1BDA72")
    IVsEnumIndependentViews : public IUnknown
    {
    public:
        virtual HRESULT STDMETHODCALLTYPE Next( 
            ULONG celt,
            /* [length_is][size_is][out] */ __RPC__out_ecount_part(celt, *pceltFetched) IUnknown **rgelt,
            /* [out] */ __RPC__out ULONG *pceltFetched) = 0;
        
        virtual HRESULT STDMETHODCALLTYPE Reset( void) = 0;
        
        virtual HRESULT STDMETHODCALLTYPE GetCount( 
            __RPC__in ULONG *pCount) = 0;
        
    };
    
#else 	/* C style interface */

    typedef struct IVsEnumIndependentViewsVtbl
    {
        BEGIN_INTERFACE
        
        HRESULT ( STDMETHODCALLTYPE *QueryInterface )( 
            IVsEnumIndependentViews * This,
            /* [in] */ __RPC__in REFIID riid,
            /* [iid_is][out] */ 
            __RPC__deref_out  void **ppvObject);
        
        ULONG ( STDMETHODCALLTYPE *AddRef )( 
            IVsEnumIndependentViews * This);
        
        ULONG ( STDMETHODCALLTYPE *Release )( 
            IVsEnumIndependentViews * This);
        
        HRESULT ( STDMETHODCALLTYPE *Next )( 
            IVsEnumIndependentViews * This,
            ULONG celt,
            /* [length_is][size_is][out] */ __RPC__out_ecount_part(celt, *pceltFetched) IUnknown **rgelt,
            /* [out] */ __RPC__out ULONG *pceltFetched);
        
        HRESULT ( STDMETHODCALLTYPE *Reset )( 
            IVsEnumIndependentViews * This);
        
        HRESULT ( STDMETHODCALLTYPE *GetCount )( 
            IVsEnumIndependentViews * This,
            __RPC__in ULONG *pCount);
        
        END_INTERFACE
    } IVsEnumIndependentViewsVtbl;

    interface IVsEnumIndependentViews
    {
        CONST_VTBL struct IVsEnumIndependentViewsVtbl *lpVtbl;
    };

    

#ifdef COBJMACROS


#define IVsEnumIndependentViews_QueryInterface(This,riid,ppvObject)	\
    ( (This)->lpVtbl -> QueryInterface(This,riid,ppvObject) ) 

#define IVsEnumIndependentViews_AddRef(This)	\
    ( (This)->lpVtbl -> AddRef(This) ) 

#define IVsEnumIndependentViews_Release(This)	\
    ( (This)->lpVtbl -> Release(This) ) 


#define IVsEnumIndependentViews_Next(This,celt,rgelt,pceltFetched)	\
    ( (This)->lpVtbl -> Next(This,celt,rgelt,pceltFetched) ) 

#define IVsEnumIndependentViews_Reset(This)	\
    ( (This)->lpVtbl -> Reset(This) ) 

#define IVsEnumIndependentViews_GetCount(This,pCount)	\
    ( (This)->lpVtbl -> GetCount(This,pCount) ) 

#endif /* COBJMACROS */


#endif 	/* C style interface */




#endif 	/* __IVsEnumIndependentViews_INTERFACE_DEFINED__ */


#ifndef __IVsColorableItem_INTERFACE_DEFINED__
#define __IVsColorableItem_INTERFACE_DEFINED__

/* interface IVsColorableItem */
/* [object][uuid] */ 


EXTERN_C const IID IID_IVsColorableItem;

#if defined(__cplusplus) && !defined(CINTERFACE)
    
    MIDL_INTERFACE("56694A40-78D0-45DD-AB15-681BC207579C")
    IVsColorableItem : public IUnknown
    {
    public:
        virtual HRESULT STDMETHODCALLTYPE GetDefaultColors( 
            /* [out] */ __RPC__out COLORINDEX *piForeground,
            /* [out] */ __RPC__out COLORINDEX *piBackground) = 0;
        
        virtual HRESULT STDMETHODCALLTYPE GetDefaultFontFlags( 
            /* [out] */ __RPC__out DWORD *pdwFontFlags) = 0;
        
        virtual HRESULT STDMETHODCALLTYPE GetDisplayName( 
            /* [out] */ __RPC__deref_out_opt BSTR *pbstrName) = 0;
        
    };
    
#else 	/* C style interface */

    typedef struct IVsColorableItemVtbl
    {
        BEGIN_INTERFACE
        
        HRESULT ( STDMETHODCALLTYPE *QueryInterface )( 
            IVsColorableItem * This,
            /* [in] */ __RPC__in REFIID riid,
            /* [iid_is][out] */ 
            __RPC__deref_out  void **ppvObject);
        
        ULONG ( STDMETHODCALLTYPE *AddRef )( 
            IVsColorableItem * This);
        
        ULONG ( STDMETHODCALLTYPE *Release )( 
            IVsColorableItem * This);
        
        HRESULT ( STDMETHODCALLTYPE *GetDefaultColors )( 
            IVsColorableItem * This,
            /* [out] */ __RPC__out COLORINDEX *piForeground,
            /* [out] */ __RPC__out COLORINDEX *piBackground);
        
        HRESULT ( STDMETHODCALLTYPE *GetDefaultFontFlags )( 
            IVsColorableItem * This,
            /* [out] */ __RPC__out DWORD *pdwFontFlags);
        
        HRESULT ( STDMETHODCALLTYPE *GetDisplayName )( 
            IVsColorableItem * This,
            /* [out] */ __RPC__deref_out_opt BSTR *pbstrName);
        
        END_INTERFACE
    } IVsColorableItemVtbl;

    interface IVsColorableItem
    {
        CONST_VTBL struct IVsColorableItemVtbl *lpVtbl;
    };

    

#ifdef COBJMACROS


#define IVsColorableItem_QueryInterface(This,riid,ppvObject)	\
    ( (This)->lpVtbl -> QueryInterface(This,riid,ppvObject) ) 

#define IVsColorableItem_AddRef(This)	\
    ( (This)->lpVtbl -> AddRef(This) ) 

#define IVsColorableItem_Release(This)	\
    ( (This)->lpVtbl -> Release(This) ) 


#define IVsColorableItem_GetDefaultColors(This,piForeground,piBackground)	\
    ( (This)->lpVtbl -> GetDefaultColors(This,piForeground,piBackground) ) 

#define IVsColorableItem_GetDefaultFontFlags(This,pdwFontFlags)	\
    ( (This)->lpVtbl -> GetDefaultFontFlags(This,pdwFontFlags) ) 

#define IVsColorableItem_GetDisplayName(This,pbstrName)	\
    ( (This)->lpVtbl -> GetDisplayName(This,pbstrName) ) 

#endif /* COBJMACROS */


#endif 	/* C style interface */




#endif 	/* __IVsColorableItem_INTERFACE_DEFINED__ */


#ifndef __IVsMergeableUIItem_INTERFACE_DEFINED__
#define __IVsMergeableUIItem_INTERFACE_DEFINED__

/* interface IVsMergeableUIItem */
/* [object][uuid] */ 


EXTERN_C const IID IID_IVsMergeableUIItem;

#if defined(__cplusplus) && !defined(CINTERFACE)
    
    MIDL_INTERFACE("0AE3E015-2900-4DDF-87AF-CC522AE02284")
    IVsMergeableUIItem : public IUnknown
    {
    public:
        virtual HRESULT STDMETHODCALLTYPE GetCanonicalName( 
            /* [out] */ __RPC__deref_out_opt BSTR *pbstrNonLocalizeName) = 0;
        
        virtual HRESULT STDMETHODCALLTYPE GetDisplayName( 
            /* [out] */ __RPC__deref_out_opt BSTR *pbstrDisplayName) = 0;
        
        virtual HRESULT STDMETHODCALLTYPE GetMergingPriority( 
            /* [out] */ __RPC__out long *piMergingPriority) = 0;
        
        virtual HRESULT STDMETHODCALLTYPE GetDescription( 
            /* [out] */ __RPC__deref_out_opt BSTR *pbstrDesc) = 0;
        
    };
    
#else 	/* C style interface */

    typedef struct IVsMergeableUIItemVtbl
    {
        BEGIN_INTERFACE
        
        HRESULT ( STDMETHODCALLTYPE *QueryInterface )( 
            IVsMergeableUIItem * This,
            /* [in] */ __RPC__in REFIID riid,
            /* [iid_is][out] */ 
            __RPC__deref_out  void **ppvObject);
        
        ULONG ( STDMETHODCALLTYPE *AddRef )( 
            IVsMergeableUIItem * This);
        
        ULONG ( STDMETHODCALLTYPE *Release )( 
            IVsMergeableUIItem * This);
        
        HRESULT ( STDMETHODCALLTYPE *GetCanonicalName )( 
            IVsMergeableUIItem * This,
            /* [out] */ __RPC__deref_out_opt BSTR *pbstrNonLocalizeName);
        
        HRESULT ( STDMETHODCALLTYPE *GetDisplayName )( 
            IVsMergeableUIItem * This,
            /* [out] */ __RPC__deref_out_opt BSTR *pbstrDisplayName);
        
        HRESULT ( STDMETHODCALLTYPE *GetMergingPriority )( 
            IVsMergeableUIItem * This,
            /* [out] */ __RPC__out long *piMergingPriority);
        
        HRESULT ( STDMETHODCALLTYPE *GetDescription )( 
            IVsMergeableUIItem * This,
            /* [out] */ __RPC__deref_out_opt BSTR *pbstrDesc);
        
        END_INTERFACE
    } IVsMergeableUIItemVtbl;

    interface IVsMergeableUIItem
    {
        CONST_VTBL struct IVsMergeableUIItemVtbl *lpVtbl;
    };

    

#ifdef COBJMACROS


#define IVsMergeableUIItem_QueryInterface(This,riid,ppvObject)	\
    ( (This)->lpVtbl -> QueryInterface(This,riid,ppvObject) ) 

#define IVsMergeableUIItem_AddRef(This)	\
    ( (This)->lpVtbl -> AddRef(This) ) 

#define IVsMergeableUIItem_Release(This)	\
    ( (This)->lpVtbl -> Release(This) ) 


#define IVsMergeableUIItem_GetCanonicalName(This,pbstrNonLocalizeName)	\
    ( (This)->lpVtbl -> GetCanonicalName(This,pbstrNonLocalizeName) ) 

#define IVsMergeableUIItem_GetDisplayName(This,pbstrDisplayName)	\
    ( (This)->lpVtbl -> GetDisplayName(This,pbstrDisplayName) ) 

#define IVsMergeableUIItem_GetMergingPriority(This,piMergingPriority)	\
    ( (This)->lpVtbl -> GetMergingPriority(This,piMergingPriority) ) 

#define IVsMergeableUIItem_GetDescription(This,pbstrDesc)	\
    ( (This)->lpVtbl -> GetDescription(This,pbstrDesc) ) 

#endif /* COBJMACROS */


#endif 	/* C style interface */




#endif 	/* __IVsMergeableUIItem_INTERFACE_DEFINED__ */


#ifndef __IVsProvideColorableItems_INTERFACE_DEFINED__
#define __IVsProvideColorableItems_INTERFACE_DEFINED__

/* interface IVsProvideColorableItems */
/* [object][uuid] */ 


EXTERN_C const IID IID_IVsProvideColorableItems;

#if defined(__cplusplus) && !defined(CINTERFACE)
    
    MIDL_INTERFACE("100B9A33-905C-4312-B2A2-452189F19AB9")
    IVsProvideColorableItems : public IUnknown
    {
    public:
        virtual HRESULT STDMETHODCALLTYPE GetItemCount( 
            /* [out] */ __RPC__out int *piCount) = 0;
        
        virtual HRESULT STDMETHODCALLTYPE GetColorableItem( 
            /* [in] */ int iIndex,
            /* [out] */ __RPC__deref_out_opt IVsColorableItem **ppItem) = 0;
        
    };
    
#else 	/* C style interface */

    typedef struct IVsProvideColorableItemsVtbl
    {
        BEGIN_INTERFACE
        
        HRESULT ( STDMETHODCALLTYPE *QueryInterface )( 
            IVsProvideColorableItems * This,
            /* [in] */ __RPC__in REFIID riid,
            /* [iid_is][out] */ 
            __RPC__deref_out  void **ppvObject);
        
        ULONG ( STDMETHODCALLTYPE *AddRef )( 
            IVsProvideColorableItems * This);
        
        ULONG ( STDMETHODCALLTYPE *Release )( 
            IVsProvideColorableItems * This);
        
        HRESULT ( STDMETHODCALLTYPE *GetItemCount )( 
            IVsProvideColorableItems * This,
            /* [out] */ __RPC__out int *piCount);
        
        HRESULT ( STDMETHODCALLTYPE *GetColorableItem )( 
            IVsProvideColorableItems * This,
            /* [in] */ int iIndex,
            /* [out] */ __RPC__deref_out_opt IVsColorableItem **ppItem);
        
        END_INTERFACE
    } IVsProvideColorableItemsVtbl;

    interface IVsProvideColorableItems
    {
        CONST_VTBL struct IVsProvideColorableItemsVtbl *lpVtbl;
    };

    

#ifdef COBJMACROS


#define IVsProvideColorableItems_QueryInterface(This,riid,ppvObject)	\
    ( (This)->lpVtbl -> QueryInterface(This,riid,ppvObject) ) 

#define IVsProvideColorableItems_AddRef(This)	\
    ( (This)->lpVtbl -> AddRef(This) ) 

#define IVsProvideColorableItems_Release(This)	\
    ( (This)->lpVtbl -> Release(This) ) 


#define IVsProvideColorableItems_GetItemCount(This,piCount)	\
    ( (This)->lpVtbl -> GetItemCount(This,piCount) ) 

#define IVsProvideColorableItems_GetColorableItem(This,iIndex,ppItem)	\
    ( (This)->lpVtbl -> GetColorableItem(This,iIndex,ppItem) ) 

#endif /* COBJMACROS */


#endif 	/* C style interface */




#endif 	/* __IVsProvideColorableItems_INTERFACE_DEFINED__ */


#ifndef __IVsColorizer_INTERFACE_DEFINED__
#define __IVsColorizer_INTERFACE_DEFINED__

/* interface IVsColorizer */
/* [object][uuid] */ 


EXTERN_C const IID IID_IVsColorizer;

#if defined(__cplusplus) && !defined(CINTERFACE)
    
    MIDL_INTERFACE("6698EF11-FF17-441C-8C5D-BC24A339B37D")
    IVsColorizer : public IUnknown
    {
    public:
        virtual HRESULT STDMETHODCALLTYPE GetStateMaintenanceFlag( 
            /* [out] */ __RPC__out BOOL *pfFlag) = 0;
        
        virtual HRESULT STDMETHODCALLTYPE GetStartState( 
            /* [out] */ __RPC__out long *piStartState) = 0;
        
        virtual long STDMETHODCALLTYPE ColorizeLine( 
            /* [in] */ long iLine,
            /* [in] */ long iLength,
            /* [in] */ __RPC__in const WCHAR *pszText,
            /* [in] */ long iState,
            /* [out] */ __RPC__out ULONG *pAttributes) = 0;
        
        virtual long STDMETHODCALLTYPE GetStateAtEndOfLine( 
            /* [in] */ long iLine,
            /* [in] */ long iLength,
            /* [in] */ __RPC__in const WCHAR *pText,
            /* [in] */ long iState) = 0;
        
        virtual void STDMETHODCALLTYPE CloseColorizer( void) = 0;
        
    };
    
#else 	/* C style interface */

    typedef struct IVsColorizerVtbl
    {
        BEGIN_INTERFACE
        
        HRESULT ( STDMETHODCALLTYPE *QueryInterface )( 
            IVsColorizer * This,
            /* [in] */ __RPC__in REFIID riid,
            /* [iid_is][out] */ 
            __RPC__deref_out  void **ppvObject);
        
        ULONG ( STDMETHODCALLTYPE *AddRef )( 
            IVsColorizer * This);
        
        ULONG ( STDMETHODCALLTYPE *Release )( 
            IVsColorizer * This);
        
        HRESULT ( STDMETHODCALLTYPE *GetStateMaintenanceFlag )( 
            IVsColorizer * This,
            /* [out] */ __RPC__out BOOL *pfFlag);
        
        HRESULT ( STDMETHODCALLTYPE *GetStartState )( 
            IVsColorizer * This,
            /* [out] */ __RPC__out long *piStartState);
        
        long ( STDMETHODCALLTYPE *ColorizeLine )( 
            IVsColorizer * This,
            /* [in] */ long iLine,
            /* [in] */ long iLength,
            /* [in] */ __RPC__in const WCHAR *pszText,
            /* [in] */ long iState,
            /* [out] */ __RPC__out ULONG *pAttributes);
        
        long ( STDMETHODCALLTYPE *GetStateAtEndOfLine )( 
            IVsColorizer * This,
            /* [in] */ long iLine,
            /* [in] */ long iLength,
            /* [in] */ __RPC__in const WCHAR *pText,
            /* [in] */ long iState);
        
        void ( STDMETHODCALLTYPE *CloseColorizer )( 
            IVsColorizer * This);
        
        END_INTERFACE
    } IVsColorizerVtbl;

    interface IVsColorizer
    {
        CONST_VTBL struct IVsColorizerVtbl *lpVtbl;
    };

    

#ifdef COBJMACROS


#define IVsColorizer_QueryInterface(This,riid,ppvObject)	\
    ( (This)->lpVtbl -> QueryInterface(This,riid,ppvObject) ) 

#define IVsColorizer_AddRef(This)	\
    ( (This)->lpVtbl -> AddRef(This) ) 

#define IVsColorizer_Release(This)	\
    ( (This)->lpVtbl -> Release(This) ) 


#define IVsColorizer_GetStateMaintenanceFlag(This,pfFlag)	\
    ( (This)->lpVtbl -> GetStateMaintenanceFlag(This,pfFlag) ) 

#define IVsColorizer_GetStartState(This,piStartState)	\
    ( (This)->lpVtbl -> GetStartState(This,piStartState) ) 

#define IVsColorizer_ColorizeLine(This,iLine,iLength,pszText,iState,pAttributes)	\
    ( (This)->lpVtbl -> ColorizeLine(This,iLine,iLength,pszText,iState,pAttributes) ) 

#define IVsColorizer_GetStateAtEndOfLine(This,iLine,iLength,pText,iState)	\
    ( (This)->lpVtbl -> GetStateAtEndOfLine(This,iLine,iLength,pText,iState) ) 

#define IVsColorizer_CloseColorizer(This)	\
    ( (This)->lpVtbl -> CloseColorizer(This) ) 

#endif /* COBJMACROS */


#endif 	/* C style interface */




#endif 	/* __IVsColorizer_INTERFACE_DEFINED__ */


#ifndef __IVsEnumGUID_INTERFACE_DEFINED__
#define __IVsEnumGUID_INTERFACE_DEFINED__

/* interface IVsEnumGUID */
/* [object][uuid] */ 


EXTERN_C const IID IID_IVsEnumGUID;

#if defined(__cplusplus) && !defined(CINTERFACE)
    
    MIDL_INTERFACE("C7A919CC-79C5-43D7-9B1E-B22E459F6557")
    IVsEnumGUID : public IUnknown
    {
    public:
        virtual HRESULT STDMETHODCALLTYPE Next( 
            /* [in] */ ULONG celt,
            /* [length_is][size_is][out] */ __RPC__out_ecount_part(celt, *pceltFetched) GUID *rgelt,
            /* [out] */ __RPC__out ULONG *pceltFetched) = 0;
        
        virtual HRESULT STDMETHODCALLTYPE Skip( 
            /* [in] */ ULONG celt) = 0;
        
        virtual HRESULT STDMETHODCALLTYPE Reset( void) = 0;
        
        virtual HRESULT STDMETHODCALLTYPE Clone( 
            /* [out] */ __RPC__deref_out_opt IVsEnumGUID **ppenum) = 0;
        
        virtual HRESULT STDMETHODCALLTYPE GetCount( 
            /* [out] */ __RPC__out ULONG *pceltCount) = 0;
        
    };
    
#else 	/* C style interface */

    typedef struct IVsEnumGUIDVtbl
    {
        BEGIN_INTERFACE
        
        HRESULT ( STDMETHODCALLTYPE *QueryInterface )( 
            IVsEnumGUID * This,
            /* [in] */ __RPC__in REFIID riid,
            /* [iid_is][out] */ 
            __RPC__deref_out  void **ppvObject);
        
        ULONG ( STDMETHODCALLTYPE *AddRef )( 
            IVsEnumGUID * This);
        
        ULONG ( STDMETHODCALLTYPE *Release )( 
            IVsEnumGUID * This);
        
        HRESULT ( STDMETHODCALLTYPE *Next )( 
            IVsEnumGUID * This,
            /* [in] */ ULONG celt,
            /* [length_is][size_is][out] */ __RPC__out_ecount_part(celt, *pceltFetched) GUID *rgelt,
            /* [out] */ __RPC__out ULONG *pceltFetched);
        
        HRESULT ( STDMETHODCALLTYPE *Skip )( 
            IVsEnumGUID * This,
            /* [in] */ ULONG celt);
        
        HRESULT ( STDMETHODCALLTYPE *Reset )( 
            IVsEnumGUID * This);
        
        HRESULT ( STDMETHODCALLTYPE *Clone )( 
            IVsEnumGUID * This,
            /* [out] */ __RPC__deref_out_opt IVsEnumGUID **ppenum);
        
        HRESULT ( STDMETHODCALLTYPE *GetCount )( 
            IVsEnumGUID * This,
            /* [out] */ __RPC__out ULONG *pceltCount);
        
        END_INTERFACE
    } IVsEnumGUIDVtbl;

    interface IVsEnumGUID
    {
        CONST_VTBL struct IVsEnumGUIDVtbl *lpVtbl;
    };

    

#ifdef COBJMACROS


#define IVsEnumGUID_QueryInterface(This,riid,ppvObject)	\
    ( (This)->lpVtbl -> QueryInterface(This,riid,ppvObject) ) 

#define IVsEnumGUID_AddRef(This)	\
    ( (This)->lpVtbl -> AddRef(This) ) 

#define IVsEnumGUID_Release(This)	\
    ( (This)->lpVtbl -> Release(This) ) 


#define IVsEnumGUID_Next(This,celt,rgelt,pceltFetched)	\
    ( (This)->lpVtbl -> Next(This,celt,rgelt,pceltFetched) ) 

#define IVsEnumGUID_Skip(This,celt)	\
    ( (This)->lpVtbl -> Skip(This,celt) ) 

#define IVsEnumGUID_Reset(This)	\
    ( (This)->lpVtbl -> Reset(This) ) 

#define IVsEnumGUID_Clone(This,ppenum)	\
    ( (This)->lpVtbl -> Clone(This,ppenum) ) 

#define IVsEnumGUID_GetCount(This,pceltCount)	\
    ( (This)->lpVtbl -> GetCount(This,pceltCount) ) 

#endif /* COBJMACROS */


#endif 	/* C style interface */




#endif 	/* __IVsEnumGUID_INTERFACE_DEFINED__ */


#ifndef __IVsLanguageInfo_INTERFACE_DEFINED__
#define __IVsLanguageInfo_INTERFACE_DEFINED__

/* interface IVsLanguageInfo */
/* [object][uuid] */ 


EXTERN_C const IID IID_IVsLanguageInfo;

#if defined(__cplusplus) && !defined(CINTERFACE)
    
    MIDL_INTERFACE("11DDB920-52C7-4237-8610-9FE8BB11656D")
    IVsLanguageInfo : public IUnknown
    {
    public:
        virtual HRESULT STDMETHODCALLTYPE GetLanguageName( 
            /* [out] */ __RPC__deref_out_opt BSTR *bstrName) = 0;
        
        virtual HRESULT STDMETHODCALLTYPE GetFileExtensions( 
            /* [out] */ __RPC__deref_out_opt BSTR *pbstrExtensions) = 0;
        
        virtual HRESULT STDMETHODCALLTYPE GetColorizer( 
            /* [in] */ __RPC__in_opt IVsTextLines *pBuffer,
            /* [out] */ __RPC__deref_out_opt IVsColorizer **ppColorizer) = 0;
        
        virtual HRESULT STDMETHODCALLTYPE GetCodeWindowManager( 
            /* [in] */ __RPC__in_opt IVsCodeWindow *pCodeWin,
            /* [out] */ __RPC__deref_out_opt IVsCodeWindowManager **ppCodeWinMgr) = 0;
        
    };
    
#else 	/* C style interface */

    typedef struct IVsLanguageInfoVtbl
    {
        BEGIN_INTERFACE
        
        HRESULT ( STDMETHODCALLTYPE *QueryInterface )( 
            IVsLanguageInfo * This,
            /* [in] */ __RPC__in REFIID riid,
            /* [iid_is][out] */ 
            __RPC__deref_out  void **ppvObject);
        
        ULONG ( STDMETHODCALLTYPE *AddRef )( 
            IVsLanguageInfo * This);
        
        ULONG ( STDMETHODCALLTYPE *Release )( 
            IVsLanguageInfo * This);
        
        HRESULT ( STDMETHODCALLTYPE *GetLanguageName )( 
            IVsLanguageInfo * This,
            /* [out] */ __RPC__deref_out_opt BSTR *bstrName);
        
        HRESULT ( STDMETHODCALLTYPE *GetFileExtensions )( 
            IVsLanguageInfo * This,
            /* [out] */ __RPC__deref_out_opt BSTR *pbstrExtensions);
        
        HRESULT ( STDMETHODCALLTYPE *GetColorizer )( 
            IVsLanguageInfo * This,
            /* [in] */ __RPC__in_opt IVsTextLines *pBuffer,
            /* [out] */ __RPC__deref_out_opt IVsColorizer **ppColorizer);
        
        HRESULT ( STDMETHODCALLTYPE *GetCodeWindowManager )( 
            IVsLanguageInfo * This,
            /* [in] */ __RPC__in_opt IVsCodeWindow *pCodeWin,
            /* [out] */ __RPC__deref_out_opt IVsCodeWindowManager **ppCodeWinMgr);
        
        END_INTERFACE
    } IVsLanguageInfoVtbl;

    interface IVsLanguageInfo
    {
        CONST_VTBL struct IVsLanguageInfoVtbl *lpVtbl;
    };

    

#ifdef COBJMACROS


#define IVsLanguageInfo_QueryInterface(This,riid,ppvObject)	\
    ( (This)->lpVtbl -> QueryInterface(This,riid,ppvObject) ) 

#define IVsLanguageInfo_AddRef(This)	\
    ( (This)->lpVtbl -> AddRef(This) ) 

#define IVsLanguageInfo_Release(This)	\
    ( (This)->lpVtbl -> Release(This) ) 


#define IVsLanguageInfo_GetLanguageName(This,bstrName)	\
    ( (This)->lpVtbl -> GetLanguageName(This,bstrName) ) 

#define IVsLanguageInfo_GetFileExtensions(This,pbstrExtensions)	\
    ( (This)->lpVtbl -> GetFileExtensions(This,pbstrExtensions) ) 

#define IVsLanguageInfo_GetColorizer(This,pBuffer,ppColorizer)	\
    ( (This)->lpVtbl -> GetColorizer(This,pBuffer,ppColorizer) ) 

#define IVsLanguageInfo_GetCodeWindowManager(This,pCodeWin,ppCodeWinMgr)	\
    ( (This)->lpVtbl -> GetCodeWindowManager(This,pCodeWin,ppCodeWinMgr) ) 

#endif /* COBJMACROS */


#endif 	/* C style interface */




#endif 	/* __IVsLanguageInfo_INTERFACE_DEFINED__ */


#ifndef __IVsLanguageBlock_INTERFACE_DEFINED__
#define __IVsLanguageBlock_INTERFACE_DEFINED__

/* interface IVsLanguageBlock */
/* [object][uuid] */ 


EXTERN_C const IID IID_IVsLanguageBlock;

#if defined(__cplusplus) && !defined(CINTERFACE)
    
    MIDL_INTERFACE("BA303DF5-6283-4961-A4CE-76D4F0662D11")
    IVsLanguageBlock : public IUnknown
    {
    public:
        virtual HRESULT STDMETHODCALLTYPE GetCurrentBlock( 
            /* [in] */ __RPC__in_opt IVsTextLines *pTextLines,
            /* [in] */ LONG iCurrentLine,
            /* [in] */ LONG iCurrentChar,
            /* [out] */ __RPC__out TextSpan *ptsBlockSpan,
            /* [out] */ __RPC__deref_out_opt BSTR *pbstrDescription,
            /* [retval][out] */ __RPC__out BOOL *pfBlockAvailable) = 0;
        
    };
    
#else 	/* C style interface */

    typedef struct IVsLanguageBlockVtbl
    {
        BEGIN_INTERFACE
        
        HRESULT ( STDMETHODCALLTYPE *QueryInterface )( 
            IVsLanguageBlock * This,
            /* [in] */ __RPC__in REFIID riid,
            /* [iid_is][out] */ 
            __RPC__deref_out  void **ppvObject);
        
        ULONG ( STDMETHODCALLTYPE *AddRef )( 
            IVsLanguageBlock * This);
        
        ULONG ( STDMETHODCALLTYPE *Release )( 
            IVsLanguageBlock * This);
        
        HRESULT ( STDMETHODCALLTYPE *GetCurrentBlock )( 
            IVsLanguageBlock * This,
            /* [in] */ __RPC__in_opt IVsTextLines *pTextLines,
            /* [in] */ LONG iCurrentLine,
            /* [in] */ LONG iCurrentChar,
            /* [out] */ __RPC__out TextSpan *ptsBlockSpan,
            /* [out] */ __RPC__deref_out_opt BSTR *pbstrDescription,
            /* [retval][out] */ __RPC__out BOOL *pfBlockAvailable);
        
        END_INTERFACE
    } IVsLanguageBlockVtbl;

    interface IVsLanguageBlock
    {
        CONST_VTBL struct IVsLanguageBlockVtbl *lpVtbl;
    };

    

#ifdef COBJMACROS


#define IVsLanguageBlock_QueryInterface(This,riid,ppvObject)	\
    ( (This)->lpVtbl -> QueryInterface(This,riid,ppvObject) ) 

#define IVsLanguageBlock_AddRef(This)	\
    ( (This)->lpVtbl -> AddRef(This) ) 

#define IVsLanguageBlock_Release(This)	\
    ( (This)->lpVtbl -> Release(This) ) 


#define IVsLanguageBlock_GetCurrentBlock(This,pTextLines,iCurrentLine,iCurrentChar,ptsBlockSpan,pbstrDescription,pfBlockAvailable)	\
    ( (This)->lpVtbl -> GetCurrentBlock(This,pTextLines,iCurrentLine,iCurrentChar,ptsBlockSpan,pbstrDescription,pfBlockAvailable) ) 

#endif /* COBJMACROS */


#endif 	/* C style interface */




#endif 	/* __IVsLanguageBlock_INTERFACE_DEFINED__ */


#ifndef __IVsEnumBSTR_INTERFACE_DEFINED__
#define __IVsEnumBSTR_INTERFACE_DEFINED__

/* interface IVsEnumBSTR */
/* [object][uuid] */ 


EXTERN_C const IID IID_IVsEnumBSTR;

#if defined(__cplusplus) && !defined(CINTERFACE)
    
    MIDL_INTERFACE("A0A9C2B0-95A3-4819-ADA3-5BD9756BBD36")
    IVsEnumBSTR : public IUnknown
    {
    public:
        virtual HRESULT STDMETHODCALLTYPE Next( 
            /* [in] */ ULONG celt,
            /* [length_is][size_is][out] */ __RPC__out_ecount_part(celt, *pceltFetched) BSTR *rgelt,
            /* [out] */ __RPC__out ULONG *pceltFetched) = 0;
        
        virtual HRESULT STDMETHODCALLTYPE Skip( 
            /* [in] */ ULONG celt) = 0;
        
        virtual HRESULT STDMETHODCALLTYPE Reset( void) = 0;
        
        virtual HRESULT STDMETHODCALLTYPE Clone( 
            /* [out] */ __RPC__deref_out_opt IVsEnumBSTR **ppenum) = 0;
        
        virtual HRESULT STDMETHODCALLTYPE GetCount( 
            /* [out] */ __RPC__out ULONG *pceltCount) = 0;
        
    };
    
#else 	/* C style interface */

    typedef struct IVsEnumBSTRVtbl
    {
        BEGIN_INTERFACE
        
        HRESULT ( STDMETHODCALLTYPE *QueryInterface )( 
            IVsEnumBSTR * This,
            /* [in] */ __RPC__in REFIID riid,
            /* [iid_is][out] */ 
            __RPC__deref_out  void **ppvObject);
        
        ULONG ( STDMETHODCALLTYPE *AddRef )( 
            IVsEnumBSTR * This);
        
        ULONG ( STDMETHODCALLTYPE *Release )( 
            IVsEnumBSTR * This);
        
        HRESULT ( STDMETHODCALLTYPE *Next )( 
            IVsEnumBSTR * This,
            /* [in] */ ULONG celt,
            /* [length_is][size_is][out] */ __RPC__out_ecount_part(celt, *pceltFetched) BSTR *rgelt,
            /* [out] */ __RPC__out ULONG *pceltFetched);
        
        HRESULT ( STDMETHODCALLTYPE *Skip )( 
            IVsEnumBSTR * This,
            /* [in] */ ULONG celt);
        
        HRESULT ( STDMETHODCALLTYPE *Reset )( 
            IVsEnumBSTR * This);
        
        HRESULT ( STDMETHODCALLTYPE *Clone )( 
            IVsEnumBSTR * This,
            /* [out] */ __RPC__deref_out_opt IVsEnumBSTR **ppenum);
        
        HRESULT ( STDMETHODCALLTYPE *GetCount )( 
            IVsEnumBSTR * This,
            /* [out] */ __RPC__out ULONG *pceltCount);
        
        END_INTERFACE
    } IVsEnumBSTRVtbl;

    interface IVsEnumBSTR
    {
        CONST_VTBL struct IVsEnumBSTRVtbl *lpVtbl;
    };

    

#ifdef COBJMACROS


#define IVsEnumBSTR_QueryInterface(This,riid,ppvObject)	\
    ( (This)->lpVtbl -> QueryInterface(This,riid,ppvObject) ) 

#define IVsEnumBSTR_AddRef(This)	\
    ( (This)->lpVtbl -> AddRef(This) ) 

#define IVsEnumBSTR_Release(This)	\
    ( (This)->lpVtbl -> Release(This) ) 


#define IVsEnumBSTR_Next(This,celt,rgelt,pceltFetched)	\
    ( (This)->lpVtbl -> Next(This,celt,rgelt,pceltFetched) ) 

#define IVsEnumBSTR_Skip(This,celt)	\
    ( (This)->lpVtbl -> Skip(This,celt) ) 

#define IVsEnumBSTR_Reset(This)	\
    ( (This)->lpVtbl -> Reset(This) ) 

#define IVsEnumBSTR_Clone(This,ppenum)	\
    ( (This)->lpVtbl -> Clone(This,ppenum) ) 

#define IVsEnumBSTR_GetCount(This,pceltCount)	\
    ( (This)->lpVtbl -> GetCount(This,pceltCount) ) 

#endif /* COBJMACROS */


#endif 	/* C style interface */




#endif 	/* __IVsEnumBSTR_INTERFACE_DEFINED__ */


#ifndef __IVsDebugName_INTERFACE_DEFINED__
#define __IVsDebugName_INTERFACE_DEFINED__

/* interface IVsDebugName */
/* [object][uuid] */ 


EXTERN_C const IID IID_IVsDebugName;

#if defined(__cplusplus) && !defined(CINTERFACE)
    
    MIDL_INTERFACE("0BCE4F72-5889-4733-8605-413C28F5899F")
    IVsDebugName : public IUnknown
    {
    public:
        virtual HRESULT STDMETHODCALLTYPE GetName( 
            /* [out] */ __RPC__deref_out_opt BSTR *pbstrName) = 0;
        
        virtual HRESULT STDMETHODCALLTYPE GetLocation( 
            /* [out] */ __RPC__deref_out_opt BSTR *pbstrMkDoc,
            /* [out] */ __RPC__out TextSpan *pspanLocation) = 0;
        
    };
    
#else 	/* C style interface */

    typedef struct IVsDebugNameVtbl
    {
        BEGIN_INTERFACE
        
        HRESULT ( STDMETHODCALLTYPE *QueryInterface )( 
            IVsDebugName * This,
            /* [in] */ __RPC__in REFIID riid,
            /* [iid_is][out] */ 
            __RPC__deref_out  void **ppvObject);
        
        ULONG ( STDMETHODCALLTYPE *AddRef )( 
            IVsDebugName * This);
        
        ULONG ( STDMETHODCALLTYPE *Release )( 
            IVsDebugName * This);
        
        HRESULT ( STDMETHODCALLTYPE *GetName )( 
            IVsDebugName * This,
            /* [out] */ __RPC__deref_out_opt BSTR *pbstrName);
        
        HRESULT ( STDMETHODCALLTYPE *GetLocation )( 
            IVsDebugName * This,
            /* [out] */ __RPC__deref_out_opt BSTR *pbstrMkDoc,
            /* [out] */ __RPC__out TextSpan *pspanLocation);
        
        END_INTERFACE
    } IVsDebugNameVtbl;

    interface IVsDebugName
    {
        CONST_VTBL struct IVsDebugNameVtbl *lpVtbl;
    };

    

#ifdef COBJMACROS


#define IVsDebugName_QueryInterface(This,riid,ppvObject)	\
    ( (This)->lpVtbl -> QueryInterface(This,riid,ppvObject) ) 

#define IVsDebugName_AddRef(This)	\
    ( (This)->lpVtbl -> AddRef(This) ) 

#define IVsDebugName_Release(This)	\
    ( (This)->lpVtbl -> Release(This) ) 


#define IVsDebugName_GetName(This,pbstrName)	\
    ( (This)->lpVtbl -> GetName(This,pbstrName) ) 

#define IVsDebugName_GetLocation(This,pbstrMkDoc,pspanLocation)	\
    ( (This)->lpVtbl -> GetLocation(This,pbstrMkDoc,pspanLocation) ) 

#endif /* COBJMACROS */


#endif 	/* C style interface */




#endif 	/* __IVsDebugName_INTERFACE_DEFINED__ */


#ifndef __IVsEnumDebugName_INTERFACE_DEFINED__
#define __IVsEnumDebugName_INTERFACE_DEFINED__

/* interface IVsEnumDebugName */
/* [object][uuid] */ 


EXTERN_C const IID IID_IVsEnumDebugName;

#if defined(__cplusplus) && !defined(CINTERFACE)
    
    MIDL_INTERFACE("9AD7EC03-4157-45B4-A999-403D6DB94578")
    IVsEnumDebugName : public IUnknown
    {
    public:
        virtual HRESULT STDMETHODCALLTYPE Next( 
            /* [in] */ ULONG celt,
            /* [length_is][size_is][out] */ __RPC__out_ecount_part(celt, *pceltFetched) IVsDebugName **rgelt,
            /* [out] */ __RPC__out ULONG *pceltFetched) = 0;
        
        virtual HRESULT STDMETHODCALLTYPE Skip( 
            /* [in] */ ULONG celt) = 0;
        
        virtual HRESULT STDMETHODCALLTYPE Reset( void) = 0;
        
        virtual HRESULT STDMETHODCALLTYPE Clone( 
            /* [out] */ __RPC__deref_out_opt IVsEnumDebugName **ppenum) = 0;
        
        virtual HRESULT STDMETHODCALLTYPE GetCount( 
            /* [out] */ __RPC__out ULONG *pceltCount) = 0;
        
    };
    
#else 	/* C style interface */

    typedef struct IVsEnumDebugNameVtbl
    {
        BEGIN_INTERFACE
        
        HRESULT ( STDMETHODCALLTYPE *QueryInterface )( 
            IVsEnumDebugName * This,
            /* [in] */ __RPC__in REFIID riid,
            /* [iid_is][out] */ 
            __RPC__deref_out  void **ppvObject);
        
        ULONG ( STDMETHODCALLTYPE *AddRef )( 
            IVsEnumDebugName * This);
        
        ULONG ( STDMETHODCALLTYPE *Release )( 
            IVsEnumDebugName * This);
        
        HRESULT ( STDMETHODCALLTYPE *Next )( 
            IVsEnumDebugName * This,
            /* [in] */ ULONG celt,
            /* [length_is][size_is][out] */ __RPC__out_ecount_part(celt, *pceltFetched) IVsDebugName **rgelt,
            /* [out] */ __RPC__out ULONG *pceltFetched);
        
        HRESULT ( STDMETHODCALLTYPE *Skip )( 
            IVsEnumDebugName * This,
            /* [in] */ ULONG celt);
        
        HRESULT ( STDMETHODCALLTYPE *Reset )( 
            IVsEnumDebugName * This);
        
        HRESULT ( STDMETHODCALLTYPE *Clone )( 
            IVsEnumDebugName * This,
            /* [out] */ __RPC__deref_out_opt IVsEnumDebugName **ppenum);
        
        HRESULT ( STDMETHODCALLTYPE *GetCount )( 
            IVsEnumDebugName * This,
            /* [out] */ __RPC__out ULONG *pceltCount);
        
        END_INTERFACE
    } IVsEnumDebugNameVtbl;

    interface IVsEnumDebugName
    {
        CONST_VTBL struct IVsEnumDebugNameVtbl *lpVtbl;
    };

    

#ifdef COBJMACROS


#define IVsEnumDebugName_QueryInterface(This,riid,ppvObject)	\
    ( (This)->lpVtbl -> QueryInterface(This,riid,ppvObject) ) 

#define IVsEnumDebugName_AddRef(This)	\
    ( (This)->lpVtbl -> AddRef(This) ) 

#define IVsEnumDebugName_Release(This)	\
    ( (This)->lpVtbl -> Release(This) ) 


#define IVsEnumDebugName_Next(This,celt,rgelt,pceltFetched)	\
    ( (This)->lpVtbl -> Next(This,celt,rgelt,pceltFetched) ) 

#define IVsEnumDebugName_Skip(This,celt)	\
    ( (This)->lpVtbl -> Skip(This,celt) ) 

#define IVsEnumDebugName_Reset(This)	\
    ( (This)->lpVtbl -> Reset(This) ) 

#define IVsEnumDebugName_Clone(This,ppenum)	\
    ( (This)->lpVtbl -> Clone(This,ppenum) ) 

#define IVsEnumDebugName_GetCount(This,pceltCount)	\
    ( (This)->lpVtbl -> GetCount(This,pceltCount) ) 

#endif /* COBJMACROS */


#endif 	/* C style interface */




#endif 	/* __IVsEnumDebugName_INTERFACE_DEFINED__ */


#ifndef __IVsLanguageDebugInfo_INTERFACE_DEFINED__
#define __IVsLanguageDebugInfo_INTERFACE_DEFINED__

/* interface IVsLanguageDebugInfo */
/* [object][uuid] */ 


EXTERN_C const IID IID_IVsLanguageDebugInfo;

#if defined(__cplusplus) && !defined(CINTERFACE)
    
    MIDL_INTERFACE("F30A6A07-5340-4C0E-B312-5772558B0E63")
    IVsLanguageDebugInfo : public IUnknown
    {
    public:
        virtual HRESULT STDMETHODCALLTYPE GetProximityExpressions( 
            /* [in] */ __RPC__in_opt IVsTextBuffer *pBuffer,
            /* [in] */ long iLine,
            /* [in] */ long iCol,
            /* [in] */ long cLines,
            /* [out] */ __RPC__deref_out_opt IVsEnumBSTR **ppEnum) = 0;
        
        virtual HRESULT STDMETHODCALLTYPE ValidateBreakpointLocation( 
            /* [in] */ __RPC__in_opt IVsTextBuffer *pBuffer,
            /* [in] */ long iLine,
            /* [in] */ long iCol,
            /* [out] */ __RPC__out TextSpan *pCodeSpan) = 0;
        
        virtual HRESULT STDMETHODCALLTYPE GetNameOfLocation( 
            /* [in] */ __RPC__in_opt IVsTextBuffer *pBuffer,
            /* [in] */ long iLine,
            /* [in] */ long iCol,
            /* [out] */ __RPC__deref_out_opt BSTR *pbstrName,
            /* [out] */ __RPC__out long *piLineOffset) = 0;
        
        virtual HRESULT STDMETHODCALLTYPE GetLocationOfName( 
            /* [in] */ __RPC__in LPCOLESTR pszName,
            /* [out] */ __RPC__deref_out_opt BSTR *pbstrMkDoc,
            /* [out] */ __RPC__out TextSpan *pspanLocation) = 0;
        
        virtual HRESULT STDMETHODCALLTYPE ResolveName( 
            /* [in] */ __RPC__in LPCOLESTR pszName,
            /* [in] */ DWORD dwFlags,
            /* [out] */ __RPC__deref_out_opt IVsEnumDebugName **ppNames) = 0;
        
        virtual HRESULT STDMETHODCALLTYPE GetLanguageID( 
            /* [in] */ __RPC__in_opt IVsTextBuffer *pBuffer,
            /* [in] */ long iLine,
            /* [in] */ long iCol,
            /* [out] */ __RPC__out GUID *pguidLanguageID) = 0;
        
        virtual HRESULT STDMETHODCALLTYPE IsMappedLocation( 
            /* [in] */ __RPC__in_opt IVsTextBuffer *pBuffer,
            /* [in] */ long iLine,
            /* [in] */ long iCol) = 0;
        
    };
    
#else 	/* C style interface */

    typedef struct IVsLanguageDebugInfoVtbl
    {
        BEGIN_INTERFACE
        
        HRESULT ( STDMETHODCALLTYPE *QueryInterface )( 
            IVsLanguageDebugInfo * This,
            /* [in] */ __RPC__in REFIID riid,
            /* [iid_is][out] */ 
            __RPC__deref_out  void **ppvObject);
        
        ULONG ( STDMETHODCALLTYPE *AddRef )( 
            IVsLanguageDebugInfo * This);
        
        ULONG ( STDMETHODCALLTYPE *Release )( 
            IVsLanguageDebugInfo * This);
        
        HRESULT ( STDMETHODCALLTYPE *GetProximityExpressions )( 
            IVsLanguageDebugInfo * This,
            /* [in] */ __RPC__in_opt IVsTextBuffer *pBuffer,
            /* [in] */ long iLine,
            /* [in] */ long iCol,
            /* [in] */ long cLines,
            /* [out] */ __RPC__deref_out_opt IVsEnumBSTR **ppEnum);
        
        HRESULT ( STDMETHODCALLTYPE *ValidateBreakpointLocation )( 
            IVsLanguageDebugInfo * This,
            /* [in] */ __RPC__in_opt IVsTextBuffer *pBuffer,
            /* [in] */ long iLine,
            /* [in] */ long iCol,
            /* [out] */ __RPC__out TextSpan *pCodeSpan);
        
        HRESULT ( STDMETHODCALLTYPE *GetNameOfLocation )( 
            IVsLanguageDebugInfo * This,
            /* [in] */ __RPC__in_opt IVsTextBuffer *pBuffer,
            /* [in] */ long iLine,
            /* [in] */ long iCol,
            /* [out] */ __RPC__deref_out_opt BSTR *pbstrName,
            /* [out] */ __RPC__out long *piLineOffset);
        
        HRESULT ( STDMETHODCALLTYPE *GetLocationOfName )( 
            IVsLanguageDebugInfo * This,
            /* [in] */ __RPC__in LPCOLESTR pszName,
            /* [out] */ __RPC__deref_out_opt BSTR *pbstrMkDoc,
            /* [out] */ __RPC__out TextSpan *pspanLocation);
        
        HRESULT ( STDMETHODCALLTYPE *ResolveName )( 
            IVsLanguageDebugInfo * This,
            /* [in] */ __RPC__in LPCOLESTR pszName,
            /* [in] */ DWORD dwFlags,
            /* [out] */ __RPC__deref_out_opt IVsEnumDebugName **ppNames);
        
        HRESULT ( STDMETHODCALLTYPE *GetLanguageID )( 
            IVsLanguageDebugInfo * This,
            /* [in] */ __RPC__in_opt IVsTextBuffer *pBuffer,
            /* [in] */ long iLine,
            /* [in] */ long iCol,
            /* [out] */ __RPC__out GUID *pguidLanguageID);
        
        HRESULT ( STDMETHODCALLTYPE *IsMappedLocation )( 
            IVsLanguageDebugInfo * This,
            /* [in] */ __RPC__in_opt IVsTextBuffer *pBuffer,
            /* [in] */ long iLine,
            /* [in] */ long iCol);
        
        END_INTERFACE
    } IVsLanguageDebugInfoVtbl;

    interface IVsLanguageDebugInfo
    {
        CONST_VTBL struct IVsLanguageDebugInfoVtbl *lpVtbl;
    };

    

#ifdef COBJMACROS


#define IVsLanguageDebugInfo_QueryInterface(This,riid,ppvObject)	\
    ( (This)->lpVtbl -> QueryInterface(This,riid,ppvObject) ) 

#define IVsLanguageDebugInfo_AddRef(This)	\
    ( (This)->lpVtbl -> AddRef(This) ) 

#define IVsLanguageDebugInfo_Release(This)	\
    ( (This)->lpVtbl -> Release(This) ) 


#define IVsLanguageDebugInfo_GetProximityExpressions(This,pBuffer,iLine,iCol,cLines,ppEnum)	\
    ( (This)->lpVtbl -> GetProximityExpressions(This,pBuffer,iLine,iCol,cLines,ppEnum) ) 

#define IVsLanguageDebugInfo_ValidateBreakpointLocation(This,pBuffer,iLine,iCol,pCodeSpan)	\
    ( (This)->lpVtbl -> ValidateBreakpointLocation(This,pBuffer,iLine,iCol,pCodeSpan) ) 

#define IVsLanguageDebugInfo_GetNameOfLocation(This,pBuffer,iLine,iCol,pbstrName,piLineOffset)	\
    ( (This)->lpVtbl -> GetNameOfLocation(This,pBuffer,iLine,iCol,pbstrName,piLineOffset) ) 

#define IVsLanguageDebugInfo_GetLocationOfName(This,pszName,pbstrMkDoc,pspanLocation)	\
    ( (This)->lpVtbl -> GetLocationOfName(This,pszName,pbstrMkDoc,pspanLocation) ) 

#define IVsLanguageDebugInfo_ResolveName(This,pszName,dwFlags,ppNames)	\
    ( (This)->lpVtbl -> ResolveName(This,pszName,dwFlags,ppNames) ) 

#define IVsLanguageDebugInfo_GetLanguageID(This,pBuffer,iLine,iCol,pguidLanguageID)	\
    ( (This)->lpVtbl -> GetLanguageID(This,pBuffer,iLine,iCol,pguidLanguageID) ) 

#define IVsLanguageDebugInfo_IsMappedLocation(This,pBuffer,iLine,iCol)	\
    ( (This)->lpVtbl -> IsMappedLocation(This,pBuffer,iLine,iCol) ) 

#endif /* COBJMACROS */


#endif 	/* C style interface */




#endif 	/* __IVsLanguageDebugInfo_INTERFACE_DEFINED__ */


#ifndef __IVsTextBufferTempInit_INTERFACE_DEFINED__
#define __IVsTextBufferTempInit_INTERFACE_DEFINED__

/* interface IVsTextBufferTempInit */
/* [object][uuid] */ 


EXTERN_C const IID IID_IVsTextBufferTempInit;

#if defined(__cplusplus) && !defined(CINTERFACE)
    
    MIDL_INTERFACE("F229A34E-5880-4C20-94C3-717CF4BE1FDA")
    IVsTextBufferTempInit : public IUnknown
    {
    public:
        virtual HRESULT STDMETHODCALLTYPE InitializeContentEx( 
            /* [in] */ __RPC__in BUFFERINITEX *pInit) = 0;
        
    };
    
#else 	/* C style interface */

    typedef struct IVsTextBufferTempInitVtbl
    {
        BEGIN_INTERFACE
        
        HRESULT ( STDMETHODCALLTYPE *QueryInterface )( 
            IVsTextBufferTempInit * This,
            /* [in] */ __RPC__in REFIID riid,
            /* [iid_is][out] */ 
            __RPC__deref_out  void **ppvObject);
        
        ULONG ( STDMETHODCALLTYPE *AddRef )( 
            IVsTextBufferTempInit * This);
        
        ULONG ( STDMETHODCALLTYPE *Release )( 
            IVsTextBufferTempInit * This);
        
        HRESULT ( STDMETHODCALLTYPE *InitializeContentEx )( 
            IVsTextBufferTempInit * This,
            /* [in] */ __RPC__in BUFFERINITEX *pInit);
        
        END_INTERFACE
    } IVsTextBufferTempInitVtbl;

    interface IVsTextBufferTempInit
    {
        CONST_VTBL struct IVsTextBufferTempInitVtbl *lpVtbl;
    };

    

#ifdef COBJMACROS


#define IVsTextBufferTempInit_QueryInterface(This,riid,ppvObject)	\
    ( (This)->lpVtbl -> QueryInterface(This,riid,ppvObject) ) 

#define IVsTextBufferTempInit_AddRef(This)	\
    ( (This)->lpVtbl -> AddRef(This) ) 

#define IVsTextBufferTempInit_Release(This)	\
    ( (This)->lpVtbl -> Release(This) ) 


#define IVsTextBufferTempInit_InitializeContentEx(This,pInit)	\
    ( (This)->lpVtbl -> InitializeContentEx(This,pInit) ) 

#endif /* COBJMACROS */


#endif 	/* C style interface */




#endif 	/* __IVsTextBufferTempInit_INTERFACE_DEFINED__ */


#ifndef __IVsTextBuffer_INTERFACE_DEFINED__
#define __IVsTextBuffer_INTERFACE_DEFINED__

/* interface IVsTextBuffer */
/* [object][uuid] */ 


EXTERN_C const IID IID_IVsTextBuffer;

#if defined(__cplusplus) && !defined(CINTERFACE)
    
    MIDL_INTERFACE("C08E5275-0D26-4DE9-8892-994024C23750")
    IVsTextBuffer : public IUnknown
    {
    public:
        virtual HRESULT STDMETHODCALLTYPE LockBuffer( void) = 0;
        
        virtual HRESULT STDMETHODCALLTYPE UnlockBuffer( void) = 0;
        
        virtual HRESULT STDMETHODCALLTYPE InitializeContent( 
            /* [in] */ __RPC__in const WCHAR *pszText,
            /* [in] */ long iLength) = 0;
        
        virtual HRESULT STDMETHODCALLTYPE GetStateFlags( 
            /* [out] */ __RPC__out DWORD *pdwReadOnlyFlags) = 0;
        
        virtual HRESULT STDMETHODCALLTYPE SetStateFlags( 
            /* [in] */ DWORD dwReadOnlyFlags) = 0;
        
        virtual HRESULT STDMETHODCALLTYPE GetPositionOfLine( 
            /* [in] */ long iLine,
            /* [out] */ __RPC__out long *piPosition) = 0;
        
        virtual HRESULT STDMETHODCALLTYPE GetPositionOfLineIndex( 
            /* [in] */ long iLine,
            /* [in] */ CharIndex iIndex,
            /* [out] */ __RPC__out long *piPosition) = 0;
        
        virtual HRESULT STDMETHODCALLTYPE GetLineIndexOfPosition( 
            /* [in] */ long iPosition,
            /* [out] */ __RPC__out long *piLine,
            /* [out] */ __RPC__out CharIndex *piColumn) = 0;
        
        virtual HRESULT STDMETHODCALLTYPE GetLengthOfLine( 
            /* [in] */ long iLine,
            /* [out] */ __RPC__out long *piLength) = 0;
        
        virtual HRESULT STDMETHODCALLTYPE GetLineCount( 
            /* [out] */ __RPC__out long *piLineCount) = 0;
        
        virtual HRESULT STDMETHODCALLTYPE GetSize( 
            /* [out] */ __RPC__out long *piLength) = 0;
        
        virtual HRESULT STDMETHODCALLTYPE GetLanguageServiceID( 
            /* [out] */ __RPC__out GUID *pguidLangService) = 0;
        
        virtual HRESULT STDMETHODCALLTYPE SetLanguageServiceID( 
            /* [in] */ __RPC__in REFGUID guidLangService) = 0;
        
        virtual HRESULT STDMETHODCALLTYPE GetUndoManager( 
            /* [out] */ __RPC__deref_out_opt IOleUndoManager **ppUndoManager) = 0;
        
        virtual HRESULT STDMETHODCALLTYPE Reserved1( void) = 0;
        
        virtual HRESULT STDMETHODCALLTYPE Reserved2( void) = 0;
        
        virtual HRESULT STDMETHODCALLTYPE Reserved3( void) = 0;
        
        virtual HRESULT STDMETHODCALLTYPE Reserved4( void) = 0;
        
        virtual HRESULT STDMETHODCALLTYPE Reload( 
            /* [in] */ BOOL fUndoable) = 0;
        
        virtual HRESULT STDMETHODCALLTYPE LockBufferEx( 
            /* [in] */ DWORD dwFlags) = 0;
        
        virtual HRESULT STDMETHODCALLTYPE UnlockBufferEx( 
            /* [in] */ DWORD dwFlags) = 0;
        
        virtual HRESULT STDMETHODCALLTYPE GetLastLineIndex( 
            /* [out] */ __RPC__out long *piLine,
            /* [out] */ __RPC__out long *piIndex) = 0;
        
        virtual HRESULT STDMETHODCALLTYPE Reserved5( void) = 0;
        
        virtual HRESULT STDMETHODCALLTYPE Reserved6( void) = 0;
        
        virtual HRESULT STDMETHODCALLTYPE Reserved7( void) = 0;
        
        virtual HRESULT STDMETHODCALLTYPE Reserved8( void) = 0;
        
        virtual HRESULT STDMETHODCALLTYPE Reserved9( void) = 0;
        
        virtual HRESULT STDMETHODCALLTYPE Reserved10( void) = 0;
        
    };
    
#else 	/* C style interface */

    typedef struct IVsTextBufferVtbl
    {
        BEGIN_INTERFACE
        
        HRESULT ( STDMETHODCALLTYPE *QueryInterface )( 
            IVsTextBuffer * This,
            /* [in] */ __RPC__in REFIID riid,
            /* [iid_is][out] */ 
            __RPC__deref_out  void **ppvObject);
        
        ULONG ( STDMETHODCALLTYPE *AddRef )( 
            IVsTextBuffer * This);
        
        ULONG ( STDMETHODCALLTYPE *Release )( 
            IVsTextBuffer * This);
        
        HRESULT ( STDMETHODCALLTYPE *LockBuffer )( 
            IVsTextBuffer * This);
        
        HRESULT ( STDMETHODCALLTYPE *UnlockBuffer )( 
            IVsTextBuffer * This);
        
        HRESULT ( STDMETHODCALLTYPE *InitializeContent )( 
            IVsTextBuffer * This,
            /* [in] */ __RPC__in const WCHAR *pszText,
            /* [in] */ long iLength);
        
        HRESULT ( STDMETHODCALLTYPE *GetStateFlags )( 
            IVsTextBuffer * This,
            /* [out] */ __RPC__out DWORD *pdwReadOnlyFlags);
        
        HRESULT ( STDMETHODCALLTYPE *SetStateFlags )( 
            IVsTextBuffer * This,
            /* [in] */ DWORD dwReadOnlyFlags);
        
        HRESULT ( STDMETHODCALLTYPE *GetPositionOfLine )( 
            IVsTextBuffer * This,
            /* [in] */ long iLine,
            /* [out] */ __RPC__out long *piPosition);
        
        HRESULT ( STDMETHODCALLTYPE *GetPositionOfLineIndex )( 
            IVsTextBuffer * This,
            /* [in] */ long iLine,
            /* [in] */ CharIndex iIndex,
            /* [out] */ __RPC__out long *piPosition);
        
        HRESULT ( STDMETHODCALLTYPE *GetLineIndexOfPosition )( 
            IVsTextBuffer * This,
            /* [in] */ long iPosition,
            /* [out] */ __RPC__out long *piLine,
            /* [out] */ __RPC__out CharIndex *piColumn);
        
        HRESULT ( STDMETHODCALLTYPE *GetLengthOfLine )( 
            IVsTextBuffer * This,
            /* [in] */ long iLine,
            /* [out] */ __RPC__out long *piLength);
        
        HRESULT ( STDMETHODCALLTYPE *GetLineCount )( 
            IVsTextBuffer * This,
            /* [out] */ __RPC__out long *piLineCount);
        
        HRESULT ( STDMETHODCALLTYPE *GetSize )( 
            IVsTextBuffer * This,
            /* [out] */ __RPC__out long *piLength);
        
        HRESULT ( STDMETHODCALLTYPE *GetLanguageServiceID )( 
            IVsTextBuffer * This,
            /* [out] */ __RPC__out GUID *pguidLangService);
        
        HRESULT ( STDMETHODCALLTYPE *SetLanguageServiceID )( 
            IVsTextBuffer * This,
            /* [in] */ __RPC__in REFGUID guidLangService);
        
        HRESULT ( STDMETHODCALLTYPE *GetUndoManager )( 
            IVsTextBuffer * This,
            /* [out] */ __RPC__deref_out_opt IOleUndoManager **ppUndoManager);
        
        HRESULT ( STDMETHODCALLTYPE *Reserved1 )( 
            IVsTextBuffer * This);
        
        HRESULT ( STDMETHODCALLTYPE *Reserved2 )( 
            IVsTextBuffer * This);
        
        HRESULT ( STDMETHODCALLTYPE *Reserved3 )( 
            IVsTextBuffer * This);
        
        HRESULT ( STDMETHODCALLTYPE *Reserved4 )( 
            IVsTextBuffer * This);
        
        HRESULT ( STDMETHODCALLTYPE *Reload )( 
            IVsTextBuffer * This,
            /* [in] */ BOOL fUndoable);
        
        HRESULT ( STDMETHODCALLTYPE *LockBufferEx )( 
            IVsTextBuffer * This,
            /* [in] */ DWORD dwFlags);
        
        HRESULT ( STDMETHODCALLTYPE *UnlockBufferEx )( 
            IVsTextBuffer * This,
            /* [in] */ DWORD dwFlags);
        
        HRESULT ( STDMETHODCALLTYPE *GetLastLineIndex )( 
            IVsTextBuffer * This,
            /* [out] */ __RPC__out long *piLine,
            /* [out] */ __RPC__out long *piIndex);
        
        HRESULT ( STDMETHODCALLTYPE *Reserved5 )( 
            IVsTextBuffer * This);
        
        HRESULT ( STDMETHODCALLTYPE *Reserved6 )( 
            IVsTextBuffer * This);
        
        HRESULT ( STDMETHODCALLTYPE *Reserved7 )( 
            IVsTextBuffer * This);
        
        HRESULT ( STDMETHODCALLTYPE *Reserved8 )( 
            IVsTextBuffer * This);
        
        HRESULT ( STDMETHODCALLTYPE *Reserved9 )( 
            IVsTextBuffer * This);
        
        HRESULT ( STDMETHODCALLTYPE *Reserved10 )( 
            IVsTextBuffer * This);
        
        END_INTERFACE
    } IVsTextBufferVtbl;

    interface IVsTextBuffer
    {
        CONST_VTBL struct IVsTextBufferVtbl *lpVtbl;
    };

    

#ifdef COBJMACROS


#define IVsTextBuffer_QueryInterface(This,riid,ppvObject)	\
    ( (This)->lpVtbl -> QueryInterface(This,riid,ppvObject) ) 

#define IVsTextBuffer_AddRef(This)	\
    ( (This)->lpVtbl -> AddRef(This) ) 

#define IVsTextBuffer_Release(This)	\
    ( (This)->lpVtbl -> Release(This) ) 


#define IVsTextBuffer_LockBuffer(This)	\
    ( (This)->lpVtbl -> LockBuffer(This) ) 

#define IVsTextBuffer_UnlockBuffer(This)	\
    ( (This)->lpVtbl -> UnlockBuffer(This) ) 

#define IVsTextBuffer_InitializeContent(This,pszText,iLength)	\
    ( (This)->lpVtbl -> InitializeContent(This,pszText,iLength) ) 

#define IVsTextBuffer_GetStateFlags(This,pdwReadOnlyFlags)	\
    ( (This)->lpVtbl -> GetStateFlags(This,pdwReadOnlyFlags) ) 

#define IVsTextBuffer_SetStateFlags(This,dwReadOnlyFlags)	\
    ( (This)->lpVtbl -> SetStateFlags(This,dwReadOnlyFlags) ) 

#define IVsTextBuffer_GetPositionOfLine(This,iLine,piPosition)	\
    ( (This)->lpVtbl -> GetPositionOfLine(This,iLine,piPosition) ) 

#define IVsTextBuffer_GetPositionOfLineIndex(This,iLine,iIndex,piPosition)	\
    ( (This)->lpVtbl -> GetPositionOfLineIndex(This,iLine,iIndex,piPosition) ) 

#define IVsTextBuffer_GetLineIndexOfPosition(This,iPosition,piLine,piColumn)	\
    ( (This)->lpVtbl -> GetLineIndexOfPosition(This,iPosition,piLine,piColumn) ) 

#define IVsTextBuffer_GetLengthOfLine(This,iLine,piLength)	\
    ( (This)->lpVtbl -> GetLengthOfLine(This,iLine,piLength) ) 

#define IVsTextBuffer_GetLineCount(This,piLineCount)	\
    ( (This)->lpVtbl -> GetLineCount(This,piLineCount) ) 

#define IVsTextBuffer_GetSize(This,piLength)	\
    ( (This)->lpVtbl -> GetSize(This,piLength) ) 

#define IVsTextBuffer_GetLanguageServiceID(This,pguidLangService)	\
    ( (This)->lpVtbl -> GetLanguageServiceID(This,pguidLangService) ) 

#define IVsTextBuffer_SetLanguageServiceID(This,guidLangService)	\
    ( (This)->lpVtbl -> SetLanguageServiceID(This,guidLangService) ) 

#define IVsTextBuffer_GetUndoManager(This,ppUndoManager)	\
    ( (This)->lpVtbl -> GetUndoManager(This,ppUndoManager) ) 

#define IVsTextBuffer_Reserved1(This)	\
    ( (This)->lpVtbl -> Reserved1(This) ) 

#define IVsTextBuffer_Reserved2(This)	\
    ( (This)->lpVtbl -> Reserved2(This) ) 

#define IVsTextBuffer_Reserved3(This)	\
    ( (This)->lpVtbl -> Reserved3(This) ) 

#define IVsTextBuffer_Reserved4(This)	\
    ( (This)->lpVtbl -> Reserved4(This) ) 

#define IVsTextBuffer_Reload(This,fUndoable)	\
    ( (This)->lpVtbl -> Reload(This,fUndoable) ) 

#define IVsTextBuffer_LockBufferEx(This,dwFlags)	\
    ( (This)->lpVtbl -> LockBufferEx(This,dwFlags) ) 

#define IVsTextBuffer_UnlockBufferEx(This,dwFlags)	\
    ( (This)->lpVtbl -> UnlockBufferEx(This,dwFlags) ) 

#define IVsTextBuffer_GetLastLineIndex(This,piLine,piIndex)	\
    ( (This)->lpVtbl -> GetLastLineIndex(This,piLine,piIndex) ) 

#define IVsTextBuffer_Reserved5(This)	\
    ( (This)->lpVtbl -> Reserved5(This) ) 

#define IVsTextBuffer_Reserved6(This)	\
    ( (This)->lpVtbl -> Reserved6(This) ) 

#define IVsTextBuffer_Reserved7(This)	\
    ( (This)->lpVtbl -> Reserved7(This) ) 

#define IVsTextBuffer_Reserved8(This)	\
    ( (This)->lpVtbl -> Reserved8(This) ) 

#define IVsTextBuffer_Reserved9(This)	\
    ( (This)->lpVtbl -> Reserved9(This) ) 

#define IVsTextBuffer_Reserved10(This)	\
    ( (This)->lpVtbl -> Reserved10(This) ) 

#endif /* COBJMACROS */


#endif 	/* C style interface */




#endif 	/* __IVsTextBuffer_INTERFACE_DEFINED__ */


#ifndef __IVsTextLines_INTERFACE_DEFINED__
#define __IVsTextLines_INTERFACE_DEFINED__

/* interface IVsTextLines */
/* [object][uuid] */ 


EXTERN_C const IID IID_IVsTextLines;

#if defined(__cplusplus) && !defined(CINTERFACE)
    
    MIDL_INTERFACE("ECF3E19D-149C-43AA-80C2-D0A46946DAA3")
    IVsTextLines : public IVsTextBuffer
    {
    public:
        virtual HRESULT STDMETHODCALLTYPE GetMarkerData( 
            /* [in] */ long iTopLine,
            /* [in] */ long iBottomLine,
            /* [out] */ __RPC__out MARKERDATA *pMarkerData) = 0;
        
        virtual HRESULT STDMETHODCALLTYPE ReleaseMarkerData( 
            /* [in] */ __RPC__in MARKERDATA *pMarkerData) = 0;
        
        virtual HRESULT STDMETHODCALLTYPE GetLineData( 
            /* [in] */ long iLine,
            /* [out] */ __RPC__out LINEDATA *pLineData,
            /* [in] */ __RPC__in MARKERDATA *pMarkerData) = 0;
        
        virtual HRESULT STDMETHODCALLTYPE ReleaseLineData( 
            /* [in] */ __RPC__in LINEDATA *pLineData) = 0;
        
        virtual HRESULT STDMETHODCALLTYPE GetLineText( 
            /* [in] */ long iStartLine,
            /* [in] */ CharIndex iStartIndex,
            /* [in] */ long iEndLine,
            /* [in] */ CharIndex iEndIndex,
            /* [out] */ __RPC__deref_out_opt BSTR *pbstrBuf) = 0;
        
        virtual HRESULT STDMETHODCALLTYPE CopyLineText( 
            /* [in] */ long iStartLine,
            /* [in] */ CharIndex iStartIndex,
            /* [in] */ long iEndLine,
            /* [in] */ CharIndex iEndIndex,
            /* [in] */ __RPC__in LPWSTR pszBuf,
            /* [out][in] */ __RPC__inout long *pcchBuf) = 0;
        
        virtual HRESULT STDMETHODCALLTYPE ReplaceLines( 
            /* [in] */ long iStartLine,
            /* [in] */ CharIndex iStartIndex,
            /* [in] */ long iEndLine,
            /* [in] */ CharIndex iEndIndex,
            /* [in] */ __RPC__in LPCWSTR pszText,
            /* [in] */ long iNewLen,
            /* [out] */ __RPC__out TextSpan *pChangedSpan) = 0;
        
        virtual HRESULT STDMETHODCALLTYPE CanReplaceLines( 
            /* [in] */ long iStartLine,
            /* [in] */ CharIndex iStartIndex,
            /* [in] */ long iEndLine,
            /* [in] */ CharIndex iEndIndex,
            /* [in] */ long iNewLen) = 0;
        
        virtual HRESULT STDMETHODCALLTYPE CreateLineMarker( 
            /* [in] */ long iMarkerType,
            /* [in] */ long iStartLine,
            /* [in] */ CharIndex iStartIndex,
            /* [in] */ long iEndLine,
            /* [in] */ CharIndex iEndIndex,
            /* [in] */ __RPC__in_opt IVsTextMarkerClient *pClient,
            /* [out] */ __RPC__deref_out_opt IVsTextLineMarker **ppMarker) = 0;
        
        virtual HRESULT STDMETHODCALLTYPE EnumMarkers( 
            /* [in] */ long iStartLine,
            /* [in] */ CharIndex iStartIndex,
            /* [in] */ long iEndLine,
            /* [in] */ CharIndex iEndIndex,
            /* [in] */ long iMarkerType,
            /* [in] */ DWORD dwFlags,
            /* [out] */ __RPC__deref_out_opt IVsEnumLineMarkers **ppEnum) = 0;
        
        virtual HRESULT STDMETHODCALLTYPE FindMarkerByLineIndex( 
            /* [in] */ long iMarkerType,
            /* [in] */ long iStartingLine,
            /* [in] */ CharIndex iStartingIndex,
            /* [in] */ DWORD dwFlags,
            /* [out] */ __RPC__deref_out_opt IVsTextLineMarker **ppMarker) = 0;
        
        virtual HRESULT STDMETHODCALLTYPE AdviseTextLinesEvents( 
            /* [in] */ __RPC__in_opt IVsTextLinesEvents *pSink,
            /* [out] */ __RPC__out DWORD *pdwCookie) = 0;
        
        virtual HRESULT STDMETHODCALLTYPE UnadviseTextLinesEvents( 
            /* [in] */ DWORD dwCookie) = 0;
        
        virtual HRESULT STDMETHODCALLTYPE GetPairExtents( 
            /* [in] */ __RPC__in const TextSpan *pSpanIn,
            /* [out] */ __RPC__out TextSpan *pSpanOut) = 0;
        
        virtual HRESULT STDMETHODCALLTYPE ReloadLines( 
            /* [in] */ long iStartLine,
            /* [in] */ CharIndex iStartIndex,
            /* [in] */ long iEndLine,
            /* [in] */ CharIndex iEndIndex,
            /* [in] */ __RPC__in LPCWSTR pszText,
            /* [in] */ long iNewLen,
            /* [out] */ __RPC__out TextSpan *pChangedSpan) = 0;
        
        virtual HRESULT STDMETHODCALLTYPE IVsTextLinesReserved1( 
            /* [in] */ long iLine,
            /* [out] */ __RPC__out LINEDATA *pLineData,
            /* [in] */ BOOL fAttributes) = 0;
        
        virtual HRESULT STDMETHODCALLTYPE GetLineDataEx( 
            /* [in] */ DWORD dwFlags,
            /* [in] */ long iLine,
            /* [in] */ long iStartIndex,
            /* [in] */ long iEndIndex,
            /* [out] */ __RPC__out LINEDATAEX *pLineData,
            /* [in] */ __RPC__in MARKERDATA *pMarkerData) = 0;
        
        virtual HRESULT STDMETHODCALLTYPE ReleaseLineDataEx( 
            /* [in] */ __RPC__in LINEDATAEX *pLineData) = 0;
        
        virtual HRESULT STDMETHODCALLTYPE CreateEditPoint( 
            /* [in] */ long iLine,
            /* [in] */ CharIndex iIndex,
            /* [out] */ __RPC__deref_out_opt IDispatch **ppEditPoint) = 0;
        
        virtual HRESULT STDMETHODCALLTYPE ReplaceLinesEx( 
            /* [in] */ DWORD dwFlags,
            /* [in] */ long iStartLine,
            /* [in] */ CharIndex iStartIndex,
            /* [in] */ long iEndLine,
            /* [in] */ CharIndex iEndIndex,
            /* [in] */ __RPC__in LPCWSTR pszText,
            /* [in] */ long iNewLen,
            /* [out] */ __RPC__out TextSpan *pChangedSpan) = 0;
        
        virtual HRESULT STDMETHODCALLTYPE CreateTextPoint( 
            /* [in] */ long iLine,
            /* [in] */ CharIndex iIndex,
            /* [out] */ __RPC__deref_out_opt IDispatch **ppTextPoint) = 0;
        
    };
    
#else 	/* C style interface */

    typedef struct IVsTextLinesVtbl
    {
        BEGIN_INTERFACE
        
        HRESULT ( STDMETHODCALLTYPE *QueryInterface )( 
            IVsTextLines * This,
            /* [in] */ __RPC__in REFIID riid,
            /* [iid_is][out] */ 
            __RPC__deref_out  void **ppvObject);
        
        ULONG ( STDMETHODCALLTYPE *AddRef )( 
            IVsTextLines * This);
        
        ULONG ( STDMETHODCALLTYPE *Release )( 
            IVsTextLines * This);
        
        HRESULT ( STDMETHODCALLTYPE *LockBuffer )( 
            IVsTextLines * This);
        
        HRESULT ( STDMETHODCALLTYPE *UnlockBuffer )( 
            IVsTextLines * This);
        
        HRESULT ( STDMETHODCALLTYPE *InitializeContent )( 
            IVsTextLines * This,
            /* [in] */ __RPC__in const WCHAR *pszText,
            /* [in] */ long iLength);
        
        HRESULT ( STDMETHODCALLTYPE *GetStateFlags )( 
            IVsTextLines * This,
            /* [out] */ __RPC__out DWORD *pdwReadOnlyFlags);
        
        HRESULT ( STDMETHODCALLTYPE *SetStateFlags )( 
            IVsTextLines * This,
            /* [in] */ DWORD dwReadOnlyFlags);
        
        HRESULT ( STDMETHODCALLTYPE *GetPositionOfLine )( 
            IVsTextLines * This,
            /* [in] */ long iLine,
            /* [out] */ __RPC__out long *piPosition);
        
        HRESULT ( STDMETHODCALLTYPE *GetPositionOfLineIndex )( 
            IVsTextLines * This,
            /* [in] */ long iLine,
            /* [in] */ CharIndex iIndex,
            /* [out] */ __RPC__out long *piPosition);
        
        HRESULT ( STDMETHODCALLTYPE *GetLineIndexOfPosition )( 
            IVsTextLines * This,
            /* [in] */ long iPosition,
            /* [out] */ __RPC__out long *piLine,
            /* [out] */ __RPC__out CharIndex *piColumn);
        
        HRESULT ( STDMETHODCALLTYPE *GetLengthOfLine )( 
            IVsTextLines * This,
            /* [in] */ long iLine,
            /* [out] */ __RPC__out long *piLength);
        
        HRESULT ( STDMETHODCALLTYPE *GetLineCount )( 
            IVsTextLines * This,
            /* [out] */ __RPC__out long *piLineCount);
        
        HRESULT ( STDMETHODCALLTYPE *GetSize )( 
            IVsTextLines * This,
            /* [out] */ __RPC__out long *piLength);
        
        HRESULT ( STDMETHODCALLTYPE *GetLanguageServiceID )( 
            IVsTextLines * This,
            /* [out] */ __RPC__out GUID *pguidLangService);
        
        HRESULT ( STDMETHODCALLTYPE *SetLanguageServiceID )( 
            IVsTextLines * This,
            /* [in] */ __RPC__in REFGUID guidLangService);
        
        HRESULT ( STDMETHODCALLTYPE *GetUndoManager )( 
            IVsTextLines * This,
            /* [out] */ __RPC__deref_out_opt IOleUndoManager **ppUndoManager);
        
        HRESULT ( STDMETHODCALLTYPE *Reserved1 )( 
            IVsTextLines * This);
        
        HRESULT ( STDMETHODCALLTYPE *Reserved2 )( 
            IVsTextLines * This);
        
        HRESULT ( STDMETHODCALLTYPE *Reserved3 )( 
            IVsTextLines * This);
        
        HRESULT ( STDMETHODCALLTYPE *Reserved4 )( 
            IVsTextLines * This);
        
        HRESULT ( STDMETHODCALLTYPE *Reload )( 
            IVsTextLines * This,
            /* [in] */ BOOL fUndoable);
        
        HRESULT ( STDMETHODCALLTYPE *LockBufferEx )( 
            IVsTextLines * This,
            /* [in] */ DWORD dwFlags);
        
        HRESULT ( STDMETHODCALLTYPE *UnlockBufferEx )( 
            IVsTextLines * This,
            /* [in] */ DWORD dwFlags);
        
        HRESULT ( STDMETHODCALLTYPE *GetLastLineIndex )( 
            IVsTextLines * This,
            /* [out] */ __RPC__out long *piLine,
            /* [out] */ __RPC__out long *piIndex);
        
        HRESULT ( STDMETHODCALLTYPE *Reserved5 )( 
            IVsTextLines * This);
        
        HRESULT ( STDMETHODCALLTYPE *Reserved6 )( 
            IVsTextLines * This);
        
        HRESULT ( STDMETHODCALLTYPE *Reserved7 )( 
            IVsTextLines * This);
        
        HRESULT ( STDMETHODCALLTYPE *Reserved8 )( 
            IVsTextLines * This);
        
        HRESULT ( STDMETHODCALLTYPE *Reserved9 )( 
            IVsTextLines * This);
        
        HRESULT ( STDMETHODCALLTYPE *Reserved10 )( 
            IVsTextLines * This);
        
        HRESULT ( STDMETHODCALLTYPE *GetMarkerData )( 
            IVsTextLines * This,
            /* [in] */ long iTopLine,
            /* [in] */ long iBottomLine,
            /* [out] */ __RPC__out MARKERDATA *pMarkerData);
        
        HRESULT ( STDMETHODCALLTYPE *ReleaseMarkerData )( 
            IVsTextLines * This,
            /* [in] */ __RPC__in MARKERDATA *pMarkerData);
        
        HRESULT ( STDMETHODCALLTYPE *GetLineData )( 
            IVsTextLines * This,
            /* [in] */ long iLine,
            /* [out] */ __RPC__out LINEDATA *pLineData,
            /* [in] */ __RPC__in MARKERDATA *pMarkerData);
        
        HRESULT ( STDMETHODCALLTYPE *ReleaseLineData )( 
            IVsTextLines * This,
            /* [in] */ __RPC__in LINEDATA *pLineData);
        
        HRESULT ( STDMETHODCALLTYPE *GetLineText )( 
            IVsTextLines * This,
            /* [in] */ long iStartLine,
            /* [in] */ CharIndex iStartIndex,
            /* [in] */ long iEndLine,
            /* [in] */ CharIndex iEndIndex,
            /* [out] */ __RPC__deref_out_opt BSTR *pbstrBuf);
        
        HRESULT ( STDMETHODCALLTYPE *CopyLineText )( 
            IVsTextLines * This,
            /* [in] */ long iStartLine,
            /* [in] */ CharIndex iStartIndex,
            /* [in] */ long iEndLine,
            /* [in] */ CharIndex iEndIndex,
            /* [in] */ __RPC__in LPWSTR pszBuf,
            /* [out][in] */ __RPC__inout long *pcchBuf);
        
        HRESULT ( STDMETHODCALLTYPE *ReplaceLines )( 
            IVsTextLines * This,
            /* [in] */ long iStartLine,
            /* [in] */ CharIndex iStartIndex,
            /* [in] */ long iEndLine,
            /* [in] */ CharIndex iEndIndex,
            /* [in] */ __RPC__in LPCWSTR pszText,
            /* [in] */ long iNewLen,
            /* [out] */ __RPC__out TextSpan *pChangedSpan);
        
        HRESULT ( STDMETHODCALLTYPE *CanReplaceLines )( 
            IVsTextLines * This,
            /* [in] */ long iStartLine,
            /* [in] */ CharIndex iStartIndex,
            /* [in] */ long iEndLine,
            /* [in] */ CharIndex iEndIndex,
            /* [in] */ long iNewLen);
        
        HRESULT ( STDMETHODCALLTYPE *CreateLineMarker )( 
            IVsTextLines * This,
            /* [in] */ long iMarkerType,
            /* [in] */ long iStartLine,
            /* [in] */ CharIndex iStartIndex,
            /* [in] */ long iEndLine,
            /* [in] */ CharIndex iEndIndex,
            /* [in] */ __RPC__in_opt IVsTextMarkerClient *pClient,
            /* [out] */ __RPC__deref_out_opt IVsTextLineMarker **ppMarker);
        
        HRESULT ( STDMETHODCALLTYPE *EnumMarkers )( 
            IVsTextLines * This,
            /* [in] */ long iStartLine,
            /* [in] */ CharIndex iStartIndex,
            /* [in] */ long iEndLine,
            /* [in] */ CharIndex iEndIndex,
            /* [in] */ long iMarkerType,
            /* [in] */ DWORD dwFlags,
            /* [out] */ __RPC__deref_out_opt IVsEnumLineMarkers **ppEnum);
        
        HRESULT ( STDMETHODCALLTYPE *FindMarkerByLineIndex )( 
            IVsTextLines * This,
            /* [in] */ long iMarkerType,
            /* [in] */ long iStartingLine,
            /* [in] */ CharIndex iStartingIndex,
            /* [in] */ DWORD dwFlags,
            /* [out] */ __RPC__deref_out_opt IVsTextLineMarker **ppMarker);
        
        HRESULT ( STDMETHODCALLTYPE *AdviseTextLinesEvents )( 
            IVsTextLines * This,
            /* [in] */ __RPC__in_opt IVsTextLinesEvents *pSink,
            /* [out] */ __RPC__out DWORD *pdwCookie);
        
        HRESULT ( STDMETHODCALLTYPE *UnadviseTextLinesEvents )( 
            IVsTextLines * This,
            /* [in] */ DWORD dwCookie);
        
        HRESULT ( STDMETHODCALLTYPE *GetPairExtents )( 
            IVsTextLines * This,
            /* [in] */ __RPC__in const TextSpan *pSpanIn,
            /* [out] */ __RPC__out TextSpan *pSpanOut);
        
        HRESULT ( STDMETHODCALLTYPE *ReloadLines )( 
            IVsTextLines * This,
            /* [in] */ long iStartLine,
            /* [in] */ CharIndex iStartIndex,
            /* [in] */ long iEndLine,
            /* [in] */ CharIndex iEndIndex,
            /* [in] */ __RPC__in LPCWSTR pszText,
            /* [in] */ long iNewLen,
            /* [out] */ __RPC__out TextSpan *pChangedSpan);
        
        HRESULT ( STDMETHODCALLTYPE *IVsTextLinesReserved1 )( 
            IVsTextLines * This,
            /* [in] */ long iLine,
            /* [out] */ __RPC__out LINEDATA *pLineData,
            /* [in] */ BOOL fAttributes);
        
        HRESULT ( STDMETHODCALLTYPE *GetLineDataEx )( 
            IVsTextLines * This,
            /* [in] */ DWORD dwFlags,
            /* [in] */ long iLine,
            /* [in] */ long iStartIndex,
            /* [in] */ long iEndIndex,
            /* [out] */ __RPC__out LINEDATAEX *pLineData,
            /* [in] */ __RPC__in MARKERDATA *pMarkerData);
        
        HRESULT ( STDMETHODCALLTYPE *ReleaseLineDataEx )( 
            IVsTextLines * This,
            /* [in] */ __RPC__in LINEDATAEX *pLineData);
        
        HRESULT ( STDMETHODCALLTYPE *CreateEditPoint )( 
            IVsTextLines * This,
            /* [in] */ long iLine,
            /* [in] */ CharIndex iIndex,
            /* [out] */ __RPC__deref_out_opt IDispatch **ppEditPoint);
        
        HRESULT ( STDMETHODCALLTYPE *ReplaceLinesEx )( 
            IVsTextLines * This,
            /* [in] */ DWORD dwFlags,
            /* [in] */ long iStartLine,
            /* [in] */ CharIndex iStartIndex,
            /* [in] */ long iEndLine,
            /* [in] */ CharIndex iEndIndex,
            /* [in] */ __RPC__in LPCWSTR pszText,
            /* [in] */ long iNewLen,
            /* [out] */ __RPC__out TextSpan *pChangedSpan);
        
        HRESULT ( STDMETHODCALLTYPE *CreateTextPoint )( 
            IVsTextLines * This,
            /* [in] */ long iLine,
            /* [in] */ CharIndex iIndex,
            /* [out] */ __RPC__deref_out_opt IDispatch **ppTextPoint);
        
        END_INTERFACE
    } IVsTextLinesVtbl;

    interface IVsTextLines
    {
        CONST_VTBL struct IVsTextLinesVtbl *lpVtbl;
    };

    

#ifdef COBJMACROS


#define IVsTextLines_QueryInterface(This,riid,ppvObject)	\
    ( (This)->lpVtbl -> QueryInterface(This,riid,ppvObject) ) 

#define IVsTextLines_AddRef(This)	\
    ( (This)->lpVtbl -> AddRef(This) ) 

#define IVsTextLines_Release(This)	\
    ( (This)->lpVtbl -> Release(This) ) 


#define IVsTextLines_LockBuffer(This)	\
    ( (This)->lpVtbl -> LockBuffer(This) ) 

#define IVsTextLines_UnlockBuffer(This)	\
    ( (This)->lpVtbl -> UnlockBuffer(This) ) 

#define IVsTextLines_InitializeContent(This,pszText,iLength)	\
    ( (This)->lpVtbl -> InitializeContent(This,pszText,iLength) ) 

#define IVsTextLines_GetStateFlags(This,pdwReadOnlyFlags)	\
    ( (This)->lpVtbl -> GetStateFlags(This,pdwReadOnlyFlags) ) 

#define IVsTextLines_SetStateFlags(This,dwReadOnlyFlags)	\
    ( (This)->lpVtbl -> SetStateFlags(This,dwReadOnlyFlags) ) 

#define IVsTextLines_GetPositionOfLine(This,iLine,piPosition)	\
    ( (This)->lpVtbl -> GetPositionOfLine(This,iLine,piPosition) ) 

#define IVsTextLines_GetPositionOfLineIndex(This,iLine,iIndex,piPosition)	\
    ( (This)->lpVtbl -> GetPositionOfLineIndex(This,iLine,iIndex,piPosition) ) 

#define IVsTextLines_GetLineIndexOfPosition(This,iPosition,piLine,piColumn)	\
    ( (This)->lpVtbl -> GetLineIndexOfPosition(This,iPosition,piLine,piColumn) ) 

#define IVsTextLines_GetLengthOfLine(This,iLine,piLength)	\
    ( (This)->lpVtbl -> GetLengthOfLine(This,iLine,piLength) ) 

#define IVsTextLines_GetLineCount(This,piLineCount)	\
    ( (This)->lpVtbl -> GetLineCount(This,piLineCount) ) 

#define IVsTextLines_GetSize(This,piLength)	\
    ( (This)->lpVtbl -> GetSize(This,piLength) ) 

#define IVsTextLines_GetLanguageServiceID(This,pguidLangService)	\
    ( (This)->lpVtbl -> GetLanguageServiceID(This,pguidLangService) ) 

#define IVsTextLines_SetLanguageServiceID(This,guidLangService)	\
    ( (This)->lpVtbl -> SetLanguageServiceID(This,guidLangService) ) 

#define IVsTextLines_GetUndoManager(This,ppUndoManager)	\
    ( (This)->lpVtbl -> GetUndoManager(This,ppUndoManager) ) 

#define IVsTextLines_Reserved1(This)	\
    ( (This)->lpVtbl -> Reserved1(This) ) 

#define IVsTextLines_Reserved2(This)	\
    ( (This)->lpVtbl -> Reserved2(This) ) 

#define IVsTextLines_Reserved3(This)	\
    ( (This)->lpVtbl -> Reserved3(This) ) 

#define IVsTextLines_Reserved4(This)	\
    ( (This)->lpVtbl -> Reserved4(This) ) 

#define IVsTextLines_Reload(This,fUndoable)	\
    ( (This)->lpVtbl -> Reload(This,fUndoable) ) 

#define IVsTextLines_LockBufferEx(This,dwFlags)	\
    ( (This)->lpVtbl -> LockBufferEx(This,dwFlags) ) 

#define IVsTextLines_UnlockBufferEx(This,dwFlags)	\
    ( (This)->lpVtbl -> UnlockBufferEx(This,dwFlags) ) 

#define IVsTextLines_GetLastLineIndex(This,piLine,piIndex)	\
    ( (This)->lpVtbl -> GetLastLineIndex(This,piLine,piIndex) ) 

#define IVsTextLines_Reserved5(This)	\
    ( (This)->lpVtbl -> Reserved5(This) ) 

#define IVsTextLines_Reserved6(This)	\
    ( (This)->lpVtbl -> Reserved6(This) ) 

#define IVsTextLines_Reserved7(This)	\
    ( (This)->lpVtbl -> Reserved7(This) ) 

#define IVsTextLines_Reserved8(This)	\
    ( (This)->lpVtbl -> Reserved8(This) ) 

#define IVsTextLines_Reserved9(This)	\
    ( (This)->lpVtbl -> Reserved9(This) ) 

#define IVsTextLines_Reserved10(This)	\
    ( (This)->lpVtbl -> Reserved10(This) ) 


#define IVsTextLines_GetMarkerData(This,iTopLine,iBottomLine,pMarkerData)	\
    ( (This)->lpVtbl -> GetMarkerData(This,iTopLine,iBottomLine,pMarkerData) ) 

#define IVsTextLines_ReleaseMarkerData(This,pMarkerData)	\
    ( (This)->lpVtbl -> ReleaseMarkerData(This,pMarkerData) ) 

#define IVsTextLines_GetLineData(This,iLine,pLineData,pMarkerData)	\
    ( (This)->lpVtbl -> GetLineData(This,iLine,pLineData,pMarkerData) ) 

#define IVsTextLines_ReleaseLineData(This,pLineData)	\
    ( (This)->lpVtbl -> ReleaseLineData(This,pLineData) ) 

#define IVsTextLines_GetLineText(This,iStartLine,iStartIndex,iEndLine,iEndIndex,pbstrBuf)	\
    ( (This)->lpVtbl -> GetLineText(This,iStartLine,iStartIndex,iEndLine,iEndIndex,pbstrBuf) ) 

#define IVsTextLines_CopyLineText(This,iStartLine,iStartIndex,iEndLine,iEndIndex,pszBuf,pcchBuf)	\
    ( (This)->lpVtbl -> CopyLineText(This,iStartLine,iStartIndex,iEndLine,iEndIndex,pszBuf,pcchBuf) ) 

#define IVsTextLines_ReplaceLines(This,iStartLine,iStartIndex,iEndLine,iEndIndex,pszText,iNewLen,pChangedSpan)	\
    ( (This)->lpVtbl -> ReplaceLines(This,iStartLine,iStartIndex,iEndLine,iEndIndex,pszText,iNewLen,pChangedSpan) ) 

#define IVsTextLines_CanReplaceLines(This,iStartLine,iStartIndex,iEndLine,iEndIndex,iNewLen)	\
    ( (This)->lpVtbl -> CanReplaceLines(This,iStartLine,iStartIndex,iEndLine,iEndIndex,iNewLen) ) 

#define IVsTextLines_CreateLineMarker(This,iMarkerType,iStartLine,iStartIndex,iEndLine,iEndIndex,pClient,ppMarker)	\
    ( (This)->lpVtbl -> CreateLineMarker(This,iMarkerType,iStartLine,iStartIndex,iEndLine,iEndIndex,pClient,ppMarker) ) 

#define IVsTextLines_EnumMarkers(This,iStartLine,iStartIndex,iEndLine,iEndIndex,iMarkerType,dwFlags,ppEnum)	\
    ( (This)->lpVtbl -> EnumMarkers(This,iStartLine,iStartIndex,iEndLine,iEndIndex,iMarkerType,dwFlags,ppEnum) ) 

#define IVsTextLines_FindMarkerByLineIndex(This,iMarkerType,iStartingLine,iStartingIndex,dwFlags,ppMarker)	\
    ( (This)->lpVtbl -> FindMarkerByLineIndex(This,iMarkerType,iStartingLine,iStartingIndex,dwFlags,ppMarker) ) 

#define IVsTextLines_AdviseTextLinesEvents(This,pSink,pdwCookie)	\
    ( (This)->lpVtbl -> AdviseTextLinesEvents(This,pSink,pdwCookie) ) 

#define IVsTextLines_UnadviseTextLinesEvents(This,dwCookie)	\
    ( (This)->lpVtbl -> UnadviseTextLinesEvents(This,dwCookie) ) 

#define IVsTextLines_GetPairExtents(This,pSpanIn,pSpanOut)	\
    ( (This)->lpVtbl -> GetPairExtents(This,pSpanIn,pSpanOut) ) 

#define IVsTextLines_ReloadLines(This,iStartLine,iStartIndex,iEndLine,iEndIndex,pszText,iNewLen,pChangedSpan)	\
    ( (This)->lpVtbl -> ReloadLines(This,iStartLine,iStartIndex,iEndLine,iEndIndex,pszText,iNewLen,pChangedSpan) ) 

#define IVsTextLines_IVsTextLinesReserved1(This,iLine,pLineData,fAttributes)	\
    ( (This)->lpVtbl -> IVsTextLinesReserved1(This,iLine,pLineData,fAttributes) ) 

#define IVsTextLines_GetLineDataEx(This,dwFlags,iLine,iStartIndex,iEndIndex,pLineData,pMarkerData)	\
    ( (This)->lpVtbl -> GetLineDataEx(This,dwFlags,iLine,iStartIndex,iEndIndex,pLineData,pMarkerData) ) 

#define IVsTextLines_ReleaseLineDataEx(This,pLineData)	\
    ( (This)->lpVtbl -> ReleaseLineDataEx(This,pLineData) ) 

#define IVsTextLines_CreateEditPoint(This,iLine,iIndex,ppEditPoint)	\
    ( (This)->lpVtbl -> CreateEditPoint(This,iLine,iIndex,ppEditPoint) ) 

#define IVsTextLines_ReplaceLinesEx(This,dwFlags,iStartLine,iStartIndex,iEndLine,iEndIndex,pszText,iNewLen,pChangedSpan)	\
    ( (This)->lpVtbl -> ReplaceLinesEx(This,dwFlags,iStartLine,iStartIndex,iEndLine,iEndIndex,pszText,iNewLen,pChangedSpan) ) 

#define IVsTextLines_CreateTextPoint(This,iLine,iIndex,ppTextPoint)	\
    ( (This)->lpVtbl -> CreateTextPoint(This,iLine,iIndex,ppTextPoint) ) 

#endif /* COBJMACROS */


#endif 	/* C style interface */




#endif 	/* __IVsTextLines_INTERFACE_DEFINED__ */


#ifndef __IVsTextColorState_INTERFACE_DEFINED__
#define __IVsTextColorState_INTERFACE_DEFINED__

/* interface IVsTextColorState */
/* [object][uuid] */ 


EXTERN_C const IID IID_IVsTextColorState;

#if defined(__cplusplus) && !defined(CINTERFACE)
    
    MIDL_INTERFACE("A7A64DE3-9B5C-47F0-ABAD-0AEDBD6CC057")
    IVsTextColorState : public IUnknown
    {
    public:
        virtual HRESULT STDMETHODCALLTYPE ReColorizeLines( 
            /* [in] */ long iTopLine,
            /* [in] */ long iBottomLine) = 0;
        
        virtual HRESULT STDMETHODCALLTYPE GetColorStateAtStartOfLine( 
            /* [in] */ long iLine,
            /* [out] */ __RPC__out long *piState) = 0;
        
    };
    
#else 	/* C style interface */

    typedef struct IVsTextColorStateVtbl
    {
        BEGIN_INTERFACE
        
        HRESULT ( STDMETHODCALLTYPE *QueryInterface )( 
            IVsTextColorState * This,
            /* [in] */ __RPC__in REFIID riid,
            /* [iid_is][out] */ 
            __RPC__deref_out  void **ppvObject);
        
        ULONG ( STDMETHODCALLTYPE *AddRef )( 
            IVsTextColorState * This);
        
        ULONG ( STDMETHODCALLTYPE *Release )( 
            IVsTextColorState * This);
        
        HRESULT ( STDMETHODCALLTYPE *ReColorizeLines )( 
            IVsTextColorState * This,
            /* [in] */ long iTopLine,
            /* [in] */ long iBottomLine);
        
        HRESULT ( STDMETHODCALLTYPE *GetColorStateAtStartOfLine )( 
            IVsTextColorState * This,
            /* [in] */ long iLine,
            /* [out] */ __RPC__out long *piState);
        
        END_INTERFACE
    } IVsTextColorStateVtbl;

    interface IVsTextColorState
    {
        CONST_VTBL struct IVsTextColorStateVtbl *lpVtbl;
    };

    

#ifdef COBJMACROS


#define IVsTextColorState_QueryInterface(This,riid,ppvObject)	\
    ( (This)->lpVtbl -> QueryInterface(This,riid,ppvObject) ) 

#define IVsTextColorState_AddRef(This)	\
    ( (This)->lpVtbl -> AddRef(This) ) 

#define IVsTextColorState_Release(This)	\
    ( (This)->lpVtbl -> Release(This) ) 


#define IVsTextColorState_ReColorizeLines(This,iTopLine,iBottomLine)	\
    ( (This)->lpVtbl -> ReColorizeLines(This,iTopLine,iBottomLine) ) 

#define IVsTextColorState_GetColorStateAtStartOfLine(This,iLine,piState)	\
    ( (This)->lpVtbl -> GetColorStateAtStartOfLine(This,iLine,piState) ) 

#endif /* COBJMACROS */


#endif 	/* C style interface */




#endif 	/* __IVsTextColorState_INTERFACE_DEFINED__ */


#ifndef __IVsTextLinesEvents_INTERFACE_DEFINED__
#define __IVsTextLinesEvents_INTERFACE_DEFINED__

/* interface IVsTextLinesEvents */
/* [object][uuid] */ 


EXTERN_C const IID IID_IVsTextLinesEvents;

#if defined(__cplusplus) && !defined(CINTERFACE)
    
    MIDL_INTERFACE("598D7074-DC17-4162-9A2F-97DD4540C2DD")
    IVsTextLinesEvents : public IUnknown
    {
    public:
        virtual void STDMETHODCALLTYPE OnChangeLineText( 
            /* [in] */ __RPC__in const TextLineChange *pTextLineChange,
            /* [in] */ BOOL fLast) = 0;
        
        virtual void STDMETHODCALLTYPE OnChangeLineAttributes( 
            /* [in] */ long iFirstLine,
            /* [in] */ long iLastLine) = 0;
        
    };
    
#else 	/* C style interface */

    typedef struct IVsTextLinesEventsVtbl
    {
        BEGIN_INTERFACE
        
        HRESULT ( STDMETHODCALLTYPE *QueryInterface )( 
            IVsTextLinesEvents * This,
            /* [in] */ __RPC__in REFIID riid,
            /* [iid_is][out] */ 
            __RPC__deref_out  void **ppvObject);
        
        ULONG ( STDMETHODCALLTYPE *AddRef )( 
            IVsTextLinesEvents * This);
        
        ULONG ( STDMETHODCALLTYPE *Release )( 
            IVsTextLinesEvents * This);
        
        void ( STDMETHODCALLTYPE *OnChangeLineText )( 
            IVsTextLinesEvents * This,
            /* [in] */ __RPC__in const TextLineChange *pTextLineChange,
            /* [in] */ BOOL fLast);
        
        void ( STDMETHODCALLTYPE *OnChangeLineAttributes )( 
            IVsTextLinesEvents * This,
            /* [in] */ long iFirstLine,
            /* [in] */ long iLastLine);
        
        END_INTERFACE
    } IVsTextLinesEventsVtbl;

    interface IVsTextLinesEvents
    {
        CONST_VTBL struct IVsTextLinesEventsVtbl *lpVtbl;
    };

    

#ifdef COBJMACROS


#define IVsTextLinesEvents_QueryInterface(This,riid,ppvObject)	\
    ( (This)->lpVtbl -> QueryInterface(This,riid,ppvObject) ) 

#define IVsTextLinesEvents_AddRef(This)	\
    ( (This)->lpVtbl -> AddRef(This) ) 

#define IVsTextLinesEvents_Release(This)	\
    ( (This)->lpVtbl -> Release(This) ) 


#define IVsTextLinesEvents_OnChangeLineText(This,pTextLineChange,fLast)	\
    ( (This)->lpVtbl -> OnChangeLineText(This,pTextLineChange,fLast) ) 

#define IVsTextLinesEvents_OnChangeLineAttributes(This,iFirstLine,iLastLine)	\
    ( (This)->lpVtbl -> OnChangeLineAttributes(This,iFirstLine,iLastLine) ) 

#endif /* COBJMACROS */


#endif 	/* C style interface */




#endif 	/* __IVsTextLinesEvents_INTERFACE_DEFINED__ */


#ifndef __IVsTextStream_INTERFACE_DEFINED__
#define __IVsTextStream_INTERFACE_DEFINED__

/* interface IVsTextStream */
/* [object][uuid] */ 


EXTERN_C const IID IID_IVsTextStream;

#if defined(__cplusplus) && !defined(CINTERFACE)
    
    MIDL_INTERFACE("D3AF020B-A862-4D22-B373-20F1B30B7D9E")
    IVsTextStream : public IVsTextBuffer
    {
    public:
        virtual HRESULT STDMETHODCALLTYPE GetStream( 
            /* [in] */ long iPos,
            /* [in] */ long iLength,
            /* [out] */ __RPC__out WCHAR *pszDest) = 0;
        
        virtual HRESULT STDMETHODCALLTYPE ReplaceStream( 
            /* [in] */ long iPos,
            /* [in] */ long iOldLen,
            /* [in] */ __RPC__in LPCOLESTR pszText,
            /* [in] */ long iNewLen) = 0;
        
        virtual HRESULT STDMETHODCALLTYPE CanReplaceStream( 
            /* [in] */ long iPos,
            /* [in] */ long iOldLen,
            /* [in] */ long iNewLen) = 0;
        
        virtual HRESULT STDMETHODCALLTYPE CreateStreamMarker( 
            /* [in] */ long iMarkerType,
            /* [in] */ long iPos,
            /* [in] */ long iLength,
            /* [in] */ __RPC__in_opt IVsTextMarkerClient *pClient,
            /* [out] */ __RPC__deref_out_opt IVsTextStreamMarker **ppMarker) = 0;
        
        virtual HRESULT STDMETHODCALLTYPE EnumMarkers( 
            /* [in] */ long iPos,
            /* [in] */ long iLen,
            /* [in] */ long iMarkerType,
            /* [in] */ DWORD dwFlags,
            /* [out] */ __RPC__deref_out_opt IVsEnumStreamMarkers **ppEnum) = 0;
        
        virtual HRESULT STDMETHODCALLTYPE FindMarkerByPosition( 
            /* [in] */ long iMarkerType,
            /* [in] */ long iStartingPos,
            /* [in] */ DWORD dwFlags,
            /* [out] */ __RPC__deref_out_opt IVsTextStreamMarker **ppMarker) = 0;
        
        virtual HRESULT STDMETHODCALLTYPE AdviseTextStreamEvents( 
            /* [in] */ __RPC__in_opt IVsTextStreamEvents *pSink,
            /* [out] */ __RPC__out DWORD *pdwCookie) = 0;
        
        virtual HRESULT STDMETHODCALLTYPE UnadviseTextStreamEvents( 
            /* [in] */ DWORD dwCookie) = 0;
        
        virtual HRESULT STDMETHODCALLTYPE ReloadStream( 
            /* [in] */ long iPos,
            /* [in] */ long iOldLen,
            /* [in] */ __RPC__in const WCHAR *pszText,
            /* [in] */ long iNewLen) = 0;
        
        virtual HRESULT STDMETHODCALLTYPE CreateEditPoint( 
            /* [in] */ long iPosition,
            /* [out] */ __RPC__deref_out_opt IDispatch **ppEditPoint) = 0;
        
        virtual HRESULT STDMETHODCALLTYPE ReplaceStreamEx( 
            /* [in] */ DWORD dwFlags,
            /* [in] */ long iPos,
            /* [in] */ long iOldLen,
            /* [in] */ __RPC__in const WCHAR *pszText,
            /* [in] */ long iNewLen,
            /* [out] */ __RPC__out long *piActualLen) = 0;
        
        virtual HRESULT STDMETHODCALLTYPE CreateTextPoint( 
            /* [in] */ long iPosition,
            /* [out] */ __RPC__deref_out_opt IDispatch **ppTextPoint) = 0;
        
    };
    
#else 	/* C style interface */

    typedef struct IVsTextStreamVtbl
    {
        BEGIN_INTERFACE
        
        HRESULT ( STDMETHODCALLTYPE *QueryInterface )( 
            IVsTextStream * This,
            /* [in] */ __RPC__in REFIID riid,
            /* [iid_is][out] */ 
            __RPC__deref_out  void **ppvObject);
        
        ULONG ( STDMETHODCALLTYPE *AddRef )( 
            IVsTextStream * This);
        
        ULONG ( STDMETHODCALLTYPE *Release )( 
            IVsTextStream * This);
        
        HRESULT ( STDMETHODCALLTYPE *LockBuffer )( 
            IVsTextStream * This);
        
        HRESULT ( STDMETHODCALLTYPE *UnlockBuffer )( 
            IVsTextStream * This);
        
        HRESULT ( STDMETHODCALLTYPE *InitializeContent )( 
            IVsTextStream * This,
            /* [in] */ __RPC__in const WCHAR *pszText,
            /* [in] */ long iLength);
        
        HRESULT ( STDMETHODCALLTYPE *GetStateFlags )( 
            IVsTextStream * This,
            /* [out] */ __RPC__out DWORD *pdwReadOnlyFlags);
        
        HRESULT ( STDMETHODCALLTYPE *SetStateFlags )( 
            IVsTextStream * This,
            /* [in] */ DWORD dwReadOnlyFlags);
        
        HRESULT ( STDMETHODCALLTYPE *GetPositionOfLine )( 
            IVsTextStream * This,
            /* [in] */ long iLine,
            /* [out] */ __RPC__out long *piPosition);
        
        HRESULT ( STDMETHODCALLTYPE *GetPositionOfLineIndex )( 
            IVsTextStream * This,
            /* [in] */ long iLine,
            /* [in] */ CharIndex iIndex,
            /* [out] */ __RPC__out long *piPosition);
        
        HRESULT ( STDMETHODCALLTYPE *GetLineIndexOfPosition )( 
            IVsTextStream * This,
            /* [in] */ long iPosition,
            /* [out] */ __RPC__out long *piLine,
            /* [out] */ __RPC__out CharIndex *piColumn);
        
        HRESULT ( STDMETHODCALLTYPE *GetLengthOfLine )( 
            IVsTextStream * This,
            /* [in] */ long iLine,
            /* [out] */ __RPC__out long *piLength);
        
        HRESULT ( STDMETHODCALLTYPE *GetLineCount )( 
            IVsTextStream * This,
            /* [out] */ __RPC__out long *piLineCount);
        
        HRESULT ( STDMETHODCALLTYPE *GetSize )( 
            IVsTextStream * This,
            /* [out] */ __RPC__out long *piLength);
        
        HRESULT ( STDMETHODCALLTYPE *GetLanguageServiceID )( 
            IVsTextStream * This,
            /* [out] */ __RPC__out GUID *pguidLangService);
        
        HRESULT ( STDMETHODCALLTYPE *SetLanguageServiceID )( 
            IVsTextStream * This,
            /* [in] */ __RPC__in REFGUID guidLangService);
        
        HRESULT ( STDMETHODCALLTYPE *GetUndoManager )( 
            IVsTextStream * This,
            /* [out] */ __RPC__deref_out_opt IOleUndoManager **ppUndoManager);
        
        HRESULT ( STDMETHODCALLTYPE *Reserved1 )( 
            IVsTextStream * This);
        
        HRESULT ( STDMETHODCALLTYPE *Reserved2 )( 
            IVsTextStream * This);
        
        HRESULT ( STDMETHODCALLTYPE *Reserved3 )( 
            IVsTextStream * This);
        
        HRESULT ( STDMETHODCALLTYPE *Reserved4 )( 
            IVsTextStream * This);
        
        HRESULT ( STDMETHODCALLTYPE *Reload )( 
            IVsTextStream * This,
            /* [in] */ BOOL fUndoable);
        
        HRESULT ( STDMETHODCALLTYPE *LockBufferEx )( 
            IVsTextStream * This,
            /* [in] */ DWORD dwFlags);
        
        HRESULT ( STDMETHODCALLTYPE *UnlockBufferEx )( 
            IVsTextStream * This,
            /* [in] */ DWORD dwFlags);
        
        HRESULT ( STDMETHODCALLTYPE *GetLastLineIndex )( 
            IVsTextStream * This,
            /* [out] */ __RPC__out long *piLine,
            /* [out] */ __RPC__out long *piIndex);
        
        HRESULT ( STDMETHODCALLTYPE *Reserved5 )( 
            IVsTextStream * This);
        
        HRESULT ( STDMETHODCALLTYPE *Reserved6 )( 
            IVsTextStream * This);
        
        HRESULT ( STDMETHODCALLTYPE *Reserved7 )( 
            IVsTextStream * This);
        
        HRESULT ( STDMETHODCALLTYPE *Reserved8 )( 
            IVsTextStream * This);
        
        HRESULT ( STDMETHODCALLTYPE *Reserved9 )( 
            IVsTextStream * This);
        
        HRESULT ( STDMETHODCALLTYPE *Reserved10 )( 
            IVsTextStream * This);
        
        HRESULT ( STDMETHODCALLTYPE *GetStream )( 
            IVsTextStream * This,
            /* [in] */ long iPos,
            /* [in] */ long iLength,
            /* [out] */ __RPC__out WCHAR *pszDest);
        
        HRESULT ( STDMETHODCALLTYPE *ReplaceStream )( 
            IVsTextStream * This,
            /* [in] */ long iPos,
            /* [in] */ long iOldLen,
            /* [in] */ __RPC__in LPCOLESTR pszText,
            /* [in] */ long iNewLen);
        
        HRESULT ( STDMETHODCALLTYPE *CanReplaceStream )( 
            IVsTextStream * This,
            /* [in] */ long iPos,
            /* [in] */ long iOldLen,
            /* [in] */ long iNewLen);
        
        HRESULT ( STDMETHODCALLTYPE *CreateStreamMarker )( 
            IVsTextStream * This,
            /* [in] */ long iMarkerType,
            /* [in] */ long iPos,
            /* [in] */ long iLength,
            /* [in] */ __RPC__in_opt IVsTextMarkerClient *pClient,
            /* [out] */ __RPC__deref_out_opt IVsTextStreamMarker **ppMarker);
        
        HRESULT ( STDMETHODCALLTYPE *EnumMarkers )( 
            IVsTextStream * This,
            /* [in] */ long iPos,
            /* [in] */ long iLen,
            /* [in] */ long iMarkerType,
            /* [in] */ DWORD dwFlags,
            /* [out] */ __RPC__deref_out_opt IVsEnumStreamMarkers **ppEnum);
        
        HRESULT ( STDMETHODCALLTYPE *FindMarkerByPosition )( 
            IVsTextStream * This,
            /* [in] */ long iMarkerType,
            /* [in] */ long iStartingPos,
            /* [in] */ DWORD dwFlags,
            /* [out] */ __RPC__deref_out_opt IVsTextStreamMarker **ppMarker);
        
        HRESULT ( STDMETHODCALLTYPE *AdviseTextStreamEvents )( 
            IVsTextStream * This,
            /* [in] */ __RPC__in_opt IVsTextStreamEvents *pSink,
            /* [out] */ __RPC__out DWORD *pdwCookie);
        
        HRESULT ( STDMETHODCALLTYPE *UnadviseTextStreamEvents )( 
            IVsTextStream * This,
            /* [in] */ DWORD dwCookie);
        
        HRESULT ( STDMETHODCALLTYPE *ReloadStream )( 
            IVsTextStream * This,
            /* [in] */ long iPos,
            /* [in] */ long iOldLen,
            /* [in] */ __RPC__in const WCHAR *pszText,
            /* [in] */ long iNewLen);
        
        HRESULT ( STDMETHODCALLTYPE *CreateEditPoint )( 
            IVsTextStream * This,
            /* [in] */ long iPosition,
            /* [out] */ __RPC__deref_out_opt IDispatch **ppEditPoint);
        
        HRESULT ( STDMETHODCALLTYPE *ReplaceStreamEx )( 
            IVsTextStream * This,
            /* [in] */ DWORD dwFlags,
            /* [in] */ long iPos,
            /* [in] */ long iOldLen,
            /* [in] */ __RPC__in const WCHAR *pszText,
            /* [in] */ long iNewLen,
            /* [out] */ __RPC__out long *piActualLen);
        
        HRESULT ( STDMETHODCALLTYPE *CreateTextPoint )( 
            IVsTextStream * This,
            /* [in] */ long iPosition,
            /* [out] */ __RPC__deref_out_opt IDispatch **ppTextPoint);
        
        END_INTERFACE
    } IVsTextStreamVtbl;

    interface IVsTextStream
    {
        CONST_VTBL struct IVsTextStreamVtbl *lpVtbl;
    };

    

#ifdef COBJMACROS


#define IVsTextStream_QueryInterface(This,riid,ppvObject)	\
    ( (This)->lpVtbl -> QueryInterface(This,riid,ppvObject) ) 

#define IVsTextStream_AddRef(This)	\
    ( (This)->lpVtbl -> AddRef(This) ) 

#define IVsTextStream_Release(This)	\
    ( (This)->lpVtbl -> Release(This) ) 


#define IVsTextStream_LockBuffer(This)	\
    ( (This)->lpVtbl -> LockBuffer(This) ) 

#define IVsTextStream_UnlockBuffer(This)	\
    ( (This)->lpVtbl -> UnlockBuffer(This) ) 

#define IVsTextStream_InitializeContent(This,pszText,iLength)	\
    ( (This)->lpVtbl -> InitializeContent(This,pszText,iLength) ) 

#define IVsTextStream_GetStateFlags(This,pdwReadOnlyFlags)	\
    ( (This)->lpVtbl -> GetStateFlags(This,pdwReadOnlyFlags) ) 

#define IVsTextStream_SetStateFlags(This,dwReadOnlyFlags)	\
    ( (This)->lpVtbl -> SetStateFlags(This,dwReadOnlyFlags) ) 

#define IVsTextStream_GetPositionOfLine(This,iLine,piPosition)	\
    ( (This)->lpVtbl -> GetPositionOfLine(This,iLine,piPosition) ) 

#define IVsTextStream_GetPositionOfLineIndex(This,iLine,iIndex,piPosition)	\
    ( (This)->lpVtbl -> GetPositionOfLineIndex(This,iLine,iIndex,piPosition) ) 

#define IVsTextStream_GetLineIndexOfPosition(This,iPosition,piLine,piColumn)	\
    ( (This)->lpVtbl -> GetLineIndexOfPosition(This,iPosition,piLine,piColumn) ) 

#define IVsTextStream_GetLengthOfLine(This,iLine,piLength)	\
    ( (This)->lpVtbl -> GetLengthOfLine(This,iLine,piLength) ) 

#define IVsTextStream_GetLineCount(This,piLineCount)	\
    ( (This)->lpVtbl -> GetLineCount(This,piLineCount) ) 

#define IVsTextStream_GetSize(This,piLength)	\
    ( (This)->lpVtbl -> GetSize(This,piLength) ) 

#define IVsTextStream_GetLanguageServiceID(This,pguidLangService)	\
    ( (This)->lpVtbl -> GetLanguageServiceID(This,pguidLangService) ) 

#define IVsTextStream_SetLanguageServiceID(This,guidLangService)	\
    ( (This)->lpVtbl -> SetLanguageServiceID(This,guidLangService) ) 

#define IVsTextStream_GetUndoManager(This,ppUndoManager)	\
    ( (This)->lpVtbl -> GetUndoManager(This,ppUndoManager) ) 

#define IVsTextStream_Reserved1(This)	\
    ( (This)->lpVtbl -> Reserved1(This) ) 

#define IVsTextStream_Reserved2(This)	\
    ( (This)->lpVtbl -> Reserved2(This) ) 

#define IVsTextStream_Reserved3(This)	\
    ( (This)->lpVtbl -> Reserved3(This) ) 

#define IVsTextStream_Reserved4(This)	\
    ( (This)->lpVtbl -> Reserved4(This) ) 

#define IVsTextStream_Reload(This,fUndoable)	\
    ( (This)->lpVtbl -> Reload(This,fUndoable) ) 

#define IVsTextStream_LockBufferEx(This,dwFlags)	\
    ( (This)->lpVtbl -> LockBufferEx(This,dwFlags) ) 

#define IVsTextStream_UnlockBufferEx(This,dwFlags)	\
    ( (This)->lpVtbl -> UnlockBufferEx(This,dwFlags) ) 

#define IVsTextStream_GetLastLineIndex(This,piLine,piIndex)	\
    ( (This)->lpVtbl -> GetLastLineIndex(This,piLine,piIndex) ) 

#define IVsTextStream_Reserved5(This)	\
    ( (This)->lpVtbl -> Reserved5(This) ) 

#define IVsTextStream_Reserved6(This)	\
    ( (This)->lpVtbl -> Reserved6(This) ) 

#define IVsTextStream_Reserved7(This)	\
    ( (This)->lpVtbl -> Reserved7(This) ) 

#define IVsTextStream_Reserved8(This)	\
    ( (This)->lpVtbl -> Reserved8(This) ) 

#define IVsTextStream_Reserved9(This)	\
    ( (This)->lpVtbl -> Reserved9(This) ) 

#define IVsTextStream_Reserved10(This)	\
    ( (This)->lpVtbl -> Reserved10(This) ) 


#define IVsTextStream_GetStream(This,iPos,iLength,pszDest)	\
    ( (This)->lpVtbl -> GetStream(This,iPos,iLength,pszDest) ) 

#define IVsTextStream_ReplaceStream(This,iPos,iOldLen,pszText,iNewLen)	\
    ( (This)->lpVtbl -> ReplaceStream(This,iPos,iOldLen,pszText,iNewLen) ) 

#define IVsTextStream_CanReplaceStream(This,iPos,iOldLen,iNewLen)	\
    ( (This)->lpVtbl -> CanReplaceStream(This,iPos,iOldLen,iNewLen) ) 

#define IVsTextStream_CreateStreamMarker(This,iMarkerType,iPos,iLength,pClient,ppMarker)	\
    ( (This)->lpVtbl -> CreateStreamMarker(This,iMarkerType,iPos,iLength,pClient,ppMarker) ) 

#define IVsTextStream_EnumMarkers(This,iPos,iLen,iMarkerType,dwFlags,ppEnum)	\
    ( (This)->lpVtbl -> EnumMarkers(This,iPos,iLen,iMarkerType,dwFlags,ppEnum) ) 

#define IVsTextStream_FindMarkerByPosition(This,iMarkerType,iStartingPos,dwFlags,ppMarker)	\
    ( (This)->lpVtbl -> FindMarkerByPosition(This,iMarkerType,iStartingPos,dwFlags,ppMarker) ) 

#define IVsTextStream_AdviseTextStreamEvents(This,pSink,pdwCookie)	\
    ( (This)->lpVtbl -> AdviseTextStreamEvents(This,pSink,pdwCookie) ) 

#define IVsTextStream_UnadviseTextStreamEvents(This,dwCookie)	\
    ( (This)->lpVtbl -> UnadviseTextStreamEvents(This,dwCookie) ) 

#define IVsTextStream_ReloadStream(This,iPos,iOldLen,pszText,iNewLen)	\
    ( (This)->lpVtbl -> ReloadStream(This,iPos,iOldLen,pszText,iNewLen) ) 

#define IVsTextStream_CreateEditPoint(This,iPosition,ppEditPoint)	\
    ( (This)->lpVtbl -> CreateEditPoint(This,iPosition,ppEditPoint) ) 

#define IVsTextStream_ReplaceStreamEx(This,dwFlags,iPos,iOldLen,pszText,iNewLen,piActualLen)	\
    ( (This)->lpVtbl -> ReplaceStreamEx(This,dwFlags,iPos,iOldLen,pszText,iNewLen,piActualLen) ) 

#define IVsTextStream_CreateTextPoint(This,iPosition,ppTextPoint)	\
    ( (This)->lpVtbl -> CreateTextPoint(This,iPosition,ppTextPoint) ) 

#endif /* COBJMACROS */


#endif 	/* C style interface */




#endif 	/* __IVsTextStream_INTERFACE_DEFINED__ */


#ifndef __IVsEnumStreamMarkers_INTERFACE_DEFINED__
#define __IVsEnumStreamMarkers_INTERFACE_DEFINED__

/* interface IVsEnumStreamMarkers */
/* [object][uuid] */ 


EXTERN_C const IID IID_IVsEnumStreamMarkers;

#if defined(__cplusplus) && !defined(CINTERFACE)
    
    MIDL_INTERFACE("06E27359-6FEE-4CCC-9E8B-AFC20C0F662A")
    IVsEnumStreamMarkers : public IUnknown
    {
    public:
        virtual HRESULT STDMETHODCALLTYPE Reset( void) = 0;
        
        virtual HRESULT STDMETHODCALLTYPE Next( 
            /* [retval][out] */ __RPC__deref_out_opt IVsTextStreamMarker **ppRetval) = 0;
        
        virtual HRESULT STDMETHODCALLTYPE GetCount( 
            /* [out] */ __RPC__out long *pcMarkers) = 0;
        
    };
    
#else 	/* C style interface */

    typedef struct IVsEnumStreamMarkersVtbl
    {
        BEGIN_INTERFACE
        
        HRESULT ( STDMETHODCALLTYPE *QueryInterface )( 
            IVsEnumStreamMarkers * This,
            /* [in] */ __RPC__in REFIID riid,
            /* [iid_is][out] */ 
            __RPC__deref_out  void **ppvObject);
        
        ULONG ( STDMETHODCALLTYPE *AddRef )( 
            IVsEnumStreamMarkers * This);
        
        ULONG ( STDMETHODCALLTYPE *Release )( 
            IVsEnumStreamMarkers * This);
        
        HRESULT ( STDMETHODCALLTYPE *Reset )( 
            IVsEnumStreamMarkers * This);
        
        HRESULT ( STDMETHODCALLTYPE *Next )( 
            IVsEnumStreamMarkers * This,
            /* [retval][out] */ __RPC__deref_out_opt IVsTextStreamMarker **ppRetval);
        
        HRESULT ( STDMETHODCALLTYPE *GetCount )( 
            IVsEnumStreamMarkers * This,
            /* [out] */ __RPC__out long *pcMarkers);
        
        END_INTERFACE
    } IVsEnumStreamMarkersVtbl;

    interface IVsEnumStreamMarkers
    {
        CONST_VTBL struct IVsEnumStreamMarkersVtbl *lpVtbl;
    };

    

#ifdef COBJMACROS


#define IVsEnumStreamMarkers_QueryInterface(This,riid,ppvObject)	\
    ( (This)->lpVtbl -> QueryInterface(This,riid,ppvObject) ) 

#define IVsEnumStreamMarkers_AddRef(This)	\
    ( (This)->lpVtbl -> AddRef(This) ) 

#define IVsEnumStreamMarkers_Release(This)	\
    ( (This)->lpVtbl -> Release(This) ) 


#define IVsEnumStreamMarkers_Reset(This)	\
    ( (This)->lpVtbl -> Reset(This) ) 

#define IVsEnumStreamMarkers_Next(This,ppRetval)	\
    ( (This)->lpVtbl -> Next(This,ppRetval) ) 

#define IVsEnumStreamMarkers_GetCount(This,pcMarkers)	\
    ( (This)->lpVtbl -> GetCount(This,pcMarkers) ) 

#endif /* COBJMACROS */


#endif 	/* C style interface */




#endif 	/* __IVsEnumStreamMarkers_INTERFACE_DEFINED__ */


#ifndef __IVsEnumLineMarkers_INTERFACE_DEFINED__
#define __IVsEnumLineMarkers_INTERFACE_DEFINED__

/* interface IVsEnumLineMarkers */
/* [object][uuid] */ 


EXTERN_C const IID IID_IVsEnumLineMarkers;

#if defined(__cplusplus) && !defined(CINTERFACE)
    
    MIDL_INTERFACE("35E981F1-77EF-4343-AAA1-8741F38627A2")
    IVsEnumLineMarkers : public IUnknown
    {
    public:
        virtual HRESULT STDMETHODCALLTYPE Reset( void) = 0;
        
        virtual HRESULT STDMETHODCALLTYPE Next( 
            /* [retval][out] */ __RPC__deref_out_opt IVsTextLineMarker **ppRetval) = 0;
        
        virtual HRESULT STDMETHODCALLTYPE GetCount( 
            /* [out] */ __RPC__out long *pcMarkers) = 0;
        
    };
    
#else 	/* C style interface */

    typedef struct IVsEnumLineMarkersVtbl
    {
        BEGIN_INTERFACE
        
        HRESULT ( STDMETHODCALLTYPE *QueryInterface )( 
            IVsEnumLineMarkers * This,
            /* [in] */ __RPC__in REFIID riid,
            /* [iid_is][out] */ 
            __RPC__deref_out  void **ppvObject);
        
        ULONG ( STDMETHODCALLTYPE *AddRef )( 
            IVsEnumLineMarkers * This);
        
        ULONG ( STDMETHODCALLTYPE *Release )( 
            IVsEnumLineMarkers * This);
        
        HRESULT ( STDMETHODCALLTYPE *Reset )( 
            IVsEnumLineMarkers * This);
        
        HRESULT ( STDMETHODCALLTYPE *Next )( 
            IVsEnumLineMarkers * This,
            /* [retval][out] */ __RPC__deref_out_opt IVsTextLineMarker **ppRetval);
        
        HRESULT ( STDMETHODCALLTYPE *GetCount )( 
            IVsEnumLineMarkers * This,
            /* [out] */ __RPC__out long *pcMarkers);
        
        END_INTERFACE
    } IVsEnumLineMarkersVtbl;

    interface IVsEnumLineMarkers
    {
        CONST_VTBL struct IVsEnumLineMarkersVtbl *lpVtbl;
    };

    

#ifdef COBJMACROS


#define IVsEnumLineMarkers_QueryInterface(This,riid,ppvObject)	\
    ( (This)->lpVtbl -> QueryInterface(This,riid,ppvObject) ) 

#define IVsEnumLineMarkers_AddRef(This)	\
    ( (This)->lpVtbl -> AddRef(This) ) 

#define IVsEnumLineMarkers_Release(This)	\
    ( (This)->lpVtbl -> Release(This) ) 


#define IVsEnumLineMarkers_Reset(This)	\
    ( (This)->lpVtbl -> Reset(This) ) 

#define IVsEnumLineMarkers_Next(This,ppRetval)	\
    ( (This)->lpVtbl -> Next(This,ppRetval) ) 

#define IVsEnumLineMarkers_GetCount(This,pcMarkers)	\
    ( (This)->lpVtbl -> GetCount(This,pcMarkers) ) 

#endif /* COBJMACROS */


#endif 	/* C style interface */




#endif 	/* __IVsEnumLineMarkers_INTERFACE_DEFINED__ */


#ifndef __IVsEnumLayerMarkers_INTERFACE_DEFINED__
#define __IVsEnumLayerMarkers_INTERFACE_DEFINED__

/* interface IVsEnumLayerMarkers */
/* [object][uuid] */ 


EXTERN_C const IID IID_IVsEnumLayerMarkers;

#if defined(__cplusplus) && !defined(CINTERFACE)
    
    MIDL_INTERFACE("8F591607-2A26-4A9D-A6C5-147C2E51E895")
    IVsEnumLayerMarkers : public IUnknown
    {
    public:
        virtual HRESULT STDMETHODCALLTYPE Reset( void) = 0;
        
        virtual HRESULT STDMETHODCALLTYPE Next( 
            /* [out] */ __RPC__deref_out_opt IVsTextLayerMarker **ppRetval) = 0;
        
        virtual HRESULT STDMETHODCALLTYPE GetCount( 
            /* [out] */ __RPC__out long *pcMarkers) = 0;
        
    };
    
#else 	/* C style interface */

    typedef struct IVsEnumLayerMarkersVtbl
    {
        BEGIN_INTERFACE
        
        HRESULT ( STDMETHODCALLTYPE *QueryInterface )( 
            IVsEnumLayerMarkers * This,
            /* [in] */ __RPC__in REFIID riid,
            /* [iid_is][out] */ 
            __RPC__deref_out  void **ppvObject);
        
        ULONG ( STDMETHODCALLTYPE *AddRef )( 
            IVsEnumLayerMarkers * This);
        
        ULONG ( STDMETHODCALLTYPE *Release )( 
            IVsEnumLayerMarkers * This);
        
        HRESULT ( STDMETHODCALLTYPE *Reset )( 
            IVsEnumLayerMarkers * This);
        
        HRESULT ( STDMETHODCALLTYPE *Next )( 
            IVsEnumLayerMarkers * This,
            /* [out] */ __RPC__deref_out_opt IVsTextLayerMarker **ppRetval);
        
        HRESULT ( STDMETHODCALLTYPE *GetCount )( 
            IVsEnumLayerMarkers * This,
            /* [out] */ __RPC__out long *pcMarkers);
        
        END_INTERFACE
    } IVsEnumLayerMarkersVtbl;

    interface IVsEnumLayerMarkers
    {
        CONST_VTBL struct IVsEnumLayerMarkersVtbl *lpVtbl;
    };

    

#ifdef COBJMACROS


#define IVsEnumLayerMarkers_QueryInterface(This,riid,ppvObject)	\
    ( (This)->lpVtbl -> QueryInterface(This,riid,ppvObject) ) 

#define IVsEnumLayerMarkers_AddRef(This)	\
    ( (This)->lpVtbl -> AddRef(This) ) 

#define IVsEnumLayerMarkers_Release(This)	\
    ( (This)->lpVtbl -> Release(This) ) 


#define IVsEnumLayerMarkers_Reset(This)	\
    ( (This)->lpVtbl -> Reset(This) ) 

#define IVsEnumLayerMarkers_Next(This,ppRetval)	\
    ( (This)->lpVtbl -> Next(This,ppRetval) ) 

#define IVsEnumLayerMarkers_GetCount(This,pcMarkers)	\
    ( (This)->lpVtbl -> GetCount(This,pcMarkers) ) 

#endif /* COBJMACROS */


#endif 	/* C style interface */




#endif 	/* __IVsEnumLayerMarkers_INTERFACE_DEFINED__ */


#ifndef __IVsTextMarker_INTERFACE_DEFINED__
#define __IVsTextMarker_INTERFACE_DEFINED__

/* interface IVsTextMarker */
/* [object][uuid] */ 


EXTERN_C const IID IID_IVsTextMarker;

#if defined(__cplusplus) && !defined(CINTERFACE)
    
    MIDL_INTERFACE("950122D9-1A51-43CA-8CED-B5D9E42DE1B5")
    IVsTextMarker : public IUnknown
    {
    public:
        virtual HRESULT STDMETHODCALLTYPE GetType( 
            /* [out] */ __RPC__out long *piMarkerType) = 0;
        
        virtual HRESULT STDMETHODCALLTYPE SetType( 
            /* [in] */ long iMarkerType) = 0;
        
        virtual HRESULT STDMETHODCALLTYPE GetVisualStyle( 
            /* [out] */ __RPC__out DWORD *pdwFlags) = 0;
        
        virtual HRESULT STDMETHODCALLTYPE SetVisualStyle( 
            /* [in] */ DWORD dwFlags) = 0;
        
        virtual HRESULT STDMETHODCALLTYPE Invalidate( void) = 0;
        
        virtual HRESULT STDMETHODCALLTYPE DrawGlyph( 
            /* [in] */ __RPC__in HDC hdc,
            /* [in] */ __RPC__in RECT *pRect) = 0;
        
        virtual HRESULT STDMETHODCALLTYPE GetTipText( 
            /* [optional][out] */ __RPC__deref_out_opt BSTR *pbstrText) = 0;
        
        virtual HRESULT STDMETHODCALLTYPE UnadviseClient( void) = 0;
        
        virtual HRESULT STDMETHODCALLTYPE GetMarkerCommandInfo( 
            /* [in] */ long iItem,
            /* [custom][out] */ __RPC__deref_out_opt BSTR *pbstrText,
            /* [out] */ __RPC__out DWORD *pcmdf) = 0;
        
        virtual HRESULT STDMETHODCALLTYPE ExecMarkerCommand( 
            /* [in] */ long iItem) = 0;
        
        virtual HRESULT STDMETHODCALLTYPE GetBehavior( 
            /* [out] */ __RPC__out DWORD *pdwBehavior) = 0;
        
        virtual HRESULT STDMETHODCALLTYPE SetBehavior( 
            /* [in] */ DWORD dwBehavior) = 0;
        
        virtual HRESULT STDMETHODCALLTYPE GetPriorityIndex( 
            /* [out] */ __RPC__out long *piPriorityIndex) = 0;
        
    };
    
#else 	/* C style interface */

    typedef struct IVsTextMarkerVtbl
    {
        BEGIN_INTERFACE
        
        HRESULT ( STDMETHODCALLTYPE *QueryInterface )( 
            IVsTextMarker * This,
            /* [in] */ __RPC__in REFIID riid,
            /* [iid_is][out] */ 
            __RPC__deref_out  void **ppvObject);
        
        ULONG ( STDMETHODCALLTYPE *AddRef )( 
            IVsTextMarker * This);
        
        ULONG ( STDMETHODCALLTYPE *Release )( 
            IVsTextMarker * This);
        
        HRESULT ( STDMETHODCALLTYPE *GetType )( 
            IVsTextMarker * This,
            /* [out] */ __RPC__out long *piMarkerType);
        
        HRESULT ( STDMETHODCALLTYPE *SetType )( 
            IVsTextMarker * This,
            /* [in] */ long iMarkerType);
        
        HRESULT ( STDMETHODCALLTYPE *GetVisualStyle )( 
            IVsTextMarker * This,
            /* [out] */ __RPC__out DWORD *pdwFlags);
        
        HRESULT ( STDMETHODCALLTYPE *SetVisualStyle )( 
            IVsTextMarker * This,
            /* [in] */ DWORD dwFlags);
        
        HRESULT ( STDMETHODCALLTYPE *Invalidate )( 
            IVsTextMarker * This);
        
        HRESULT ( STDMETHODCALLTYPE *DrawGlyph )( 
            IVsTextMarker * This,
            /* [in] */ __RPC__in HDC hdc,
            /* [in] */ __RPC__in RECT *pRect);
        
        HRESULT ( STDMETHODCALLTYPE *GetTipText )( 
            IVsTextMarker * This,
            /* [optional][out] */ __RPC__deref_out_opt BSTR *pbstrText);
        
        HRESULT ( STDMETHODCALLTYPE *UnadviseClient )( 
            IVsTextMarker * This);
        
        HRESULT ( STDMETHODCALLTYPE *GetMarkerCommandInfo )( 
            IVsTextMarker * This,
            /* [in] */ long iItem,
            /* [custom][out] */ __RPC__deref_out_opt BSTR *pbstrText,
            /* [out] */ __RPC__out DWORD *pcmdf);
        
        HRESULT ( STDMETHODCALLTYPE *ExecMarkerCommand )( 
            IVsTextMarker * This,
            /* [in] */ long iItem);
        
        HRESULT ( STDMETHODCALLTYPE *GetBehavior )( 
            IVsTextMarker * This,
            /* [out] */ __RPC__out DWORD *pdwBehavior);
        
        HRESULT ( STDMETHODCALLTYPE *SetBehavior )( 
            IVsTextMarker * This,
            /* [in] */ DWORD dwBehavior);
        
        HRESULT ( STDMETHODCALLTYPE *GetPriorityIndex )( 
            IVsTextMarker * This,
            /* [out] */ __RPC__out long *piPriorityIndex);
        
        END_INTERFACE
    } IVsTextMarkerVtbl;

    interface IVsTextMarker
    {
        CONST_VTBL struct IVsTextMarkerVtbl *lpVtbl;
    };

    

#ifdef COBJMACROS


#define IVsTextMarker_QueryInterface(This,riid,ppvObject)	\
    ( (This)->lpVtbl -> QueryInterface(This,riid,ppvObject) ) 

#define IVsTextMarker_AddRef(This)	\
    ( (This)->lpVtbl -> AddRef(This) ) 

#define IVsTextMarker_Release(This)	\
    ( (This)->lpVtbl -> Release(This) ) 


#define IVsTextMarker_GetType(This,piMarkerType)	\
    ( (This)->lpVtbl -> GetType(This,piMarkerType) ) 

#define IVsTextMarker_SetType(This,iMarkerType)	\
    ( (This)->lpVtbl -> SetType(This,iMarkerType) ) 

#define IVsTextMarker_GetVisualStyle(This,pdwFlags)	\
    ( (This)->lpVtbl -> GetVisualStyle(This,pdwFlags) ) 

#define IVsTextMarker_SetVisualStyle(This,dwFlags)	\
    ( (This)->lpVtbl -> SetVisualStyle(This,dwFlags) ) 

#define IVsTextMarker_Invalidate(This)	\
    ( (This)->lpVtbl -> Invalidate(This) ) 

#define IVsTextMarker_DrawGlyph(This,hdc,pRect)	\
    ( (This)->lpVtbl -> DrawGlyph(This,hdc,pRect) ) 

#define IVsTextMarker_GetTipText(This,pbstrText)	\
    ( (This)->lpVtbl -> GetTipText(This,pbstrText) ) 

#define IVsTextMarker_UnadviseClient(This)	\
    ( (This)->lpVtbl -> UnadviseClient(This) ) 

#define IVsTextMarker_GetMarkerCommandInfo(This,iItem,pbstrText,pcmdf)	\
    ( (This)->lpVtbl -> GetMarkerCommandInfo(This,iItem,pbstrText,pcmdf) ) 

#define IVsTextMarker_ExecMarkerCommand(This,iItem)	\
    ( (This)->lpVtbl -> ExecMarkerCommand(This,iItem) ) 

#define IVsTextMarker_GetBehavior(This,pdwBehavior)	\
    ( (This)->lpVtbl -> GetBehavior(This,pdwBehavior) ) 

#define IVsTextMarker_SetBehavior(This,dwBehavior)	\
    ( (This)->lpVtbl -> SetBehavior(This,dwBehavior) ) 

#define IVsTextMarker_GetPriorityIndex(This,piPriorityIndex)	\
    ( (This)->lpVtbl -> GetPriorityIndex(This,piPriorityIndex) ) 

#endif /* COBJMACROS */


#endif 	/* C style interface */




#endif 	/* __IVsTextMarker_INTERFACE_DEFINED__ */


#ifndef __IVsTextStreamMarker_INTERFACE_DEFINED__
#define __IVsTextStreamMarker_INTERFACE_DEFINED__

/* interface IVsTextStreamMarker */
/* [object][uuid] */ 


EXTERN_C const IID IID_IVsTextStreamMarker;

#if defined(__cplusplus) && !defined(CINTERFACE)
    
    MIDL_INTERFACE("10D17872-91BA-4D65-80CC-E5FC3222AC82")
    IVsTextStreamMarker : public IVsTextMarker
    {
    public:
        virtual HRESULT STDMETHODCALLTYPE GetStreamBuffer( 
            /* [out] */ __RPC__deref_out_opt IVsTextStream **ppBuffer) = 0;
        
        virtual HRESULT STDMETHODCALLTYPE ResetSpan( 
            /* [in] */ long iNewPos,
            /* [in] */ long iNewLen) = 0;
        
        virtual HRESULT STDMETHODCALLTYPE GetCurrentSpan( 
            /* [out] */ __RPC__out long *piPos,
            /* [out] */ __RPC__out long *piLen) = 0;
        
    };
    
#else 	/* C style interface */

    typedef struct IVsTextStreamMarkerVtbl
    {
        BEGIN_INTERFACE
        
        HRESULT ( STDMETHODCALLTYPE *QueryInterface )( 
            IVsTextStreamMarker * This,
            /* [in] */ __RPC__in REFIID riid,
            /* [iid_is][out] */ 
            __RPC__deref_out  void **ppvObject);
        
        ULONG ( STDMETHODCALLTYPE *AddRef )( 
            IVsTextStreamMarker * This);
        
        ULONG ( STDMETHODCALLTYPE *Release )( 
            IVsTextStreamMarker * This);
        
        HRESULT ( STDMETHODCALLTYPE *GetType )( 
            IVsTextStreamMarker * This,
            /* [out] */ __RPC__out long *piMarkerType);
        
        HRESULT ( STDMETHODCALLTYPE *SetType )( 
            IVsTextStreamMarker * This,
            /* [in] */ long iMarkerType);
        
        HRESULT ( STDMETHODCALLTYPE *GetVisualStyle )( 
            IVsTextStreamMarker * This,
            /* [out] */ __RPC__out DWORD *pdwFlags);
        
        HRESULT ( STDMETHODCALLTYPE *SetVisualStyle )( 
            IVsTextStreamMarker * This,
            /* [in] */ DWORD dwFlags);
        
        HRESULT ( STDMETHODCALLTYPE *Invalidate )( 
            IVsTextStreamMarker * This);
        
        HRESULT ( STDMETHODCALLTYPE *DrawGlyph )( 
            IVsTextStreamMarker * This,
            /* [in] */ __RPC__in HDC hdc,
            /* [in] */ __RPC__in RECT *pRect);
        
        HRESULT ( STDMETHODCALLTYPE *GetTipText )( 
            IVsTextStreamMarker * This,
            /* [optional][out] */ __RPC__deref_out_opt BSTR *pbstrText);
        
        HRESULT ( STDMETHODCALLTYPE *UnadviseClient )( 
            IVsTextStreamMarker * This);
        
        HRESULT ( STDMETHODCALLTYPE *GetMarkerCommandInfo )( 
            IVsTextStreamMarker * This,
            /* [in] */ long iItem,
            /* [custom][out] */ __RPC__deref_out_opt BSTR *pbstrText,
            /* [out] */ __RPC__out DWORD *pcmdf);
        
        HRESULT ( STDMETHODCALLTYPE *ExecMarkerCommand )( 
            IVsTextStreamMarker * This,
            /* [in] */ long iItem);
        
        HRESULT ( STDMETHODCALLTYPE *GetBehavior )( 
            IVsTextStreamMarker * This,
            /* [out] */ __RPC__out DWORD *pdwBehavior);
        
        HRESULT ( STDMETHODCALLTYPE *SetBehavior )( 
            IVsTextStreamMarker * This,
            /* [in] */ DWORD dwBehavior);
        
        HRESULT ( STDMETHODCALLTYPE *GetPriorityIndex )( 
            IVsTextStreamMarker * This,
            /* [out] */ __RPC__out long *piPriorityIndex);
        
        HRESULT ( STDMETHODCALLTYPE *GetStreamBuffer )( 
            IVsTextStreamMarker * This,
            /* [out] */ __RPC__deref_out_opt IVsTextStream **ppBuffer);
        
        HRESULT ( STDMETHODCALLTYPE *ResetSpan )( 
            IVsTextStreamMarker * This,
            /* [in] */ long iNewPos,
            /* [in] */ long iNewLen);
        
        HRESULT ( STDMETHODCALLTYPE *GetCurrentSpan )( 
            IVsTextStreamMarker * This,
            /* [out] */ __RPC__out long *piPos,
            /* [out] */ __RPC__out long *piLen);
        
        END_INTERFACE
    } IVsTextStreamMarkerVtbl;

    interface IVsTextStreamMarker
    {
        CONST_VTBL struct IVsTextStreamMarkerVtbl *lpVtbl;
    };

    

#ifdef COBJMACROS


#define IVsTextStreamMarker_QueryInterface(This,riid,ppvObject)	\
    ( (This)->lpVtbl -> QueryInterface(This,riid,ppvObject) ) 

#define IVsTextStreamMarker_AddRef(This)	\
    ( (This)->lpVtbl -> AddRef(This) ) 

#define IVsTextStreamMarker_Release(This)	\
    ( (This)->lpVtbl -> Release(This) ) 


#define IVsTextStreamMarker_GetType(This,piMarkerType)	\
    ( (This)->lpVtbl -> GetType(This,piMarkerType) ) 

#define IVsTextStreamMarker_SetType(This,iMarkerType)	\
    ( (This)->lpVtbl -> SetType(This,iMarkerType) ) 

#define IVsTextStreamMarker_GetVisualStyle(This,pdwFlags)	\
    ( (This)->lpVtbl -> GetVisualStyle(This,pdwFlags) ) 

#define IVsTextStreamMarker_SetVisualStyle(This,dwFlags)	\
    ( (This)->lpVtbl -> SetVisualStyle(This,dwFlags) ) 

#define IVsTextStreamMarker_Invalidate(This)	\
    ( (This)->lpVtbl -> Invalidate(This) ) 

#define IVsTextStreamMarker_DrawGlyph(This,hdc,pRect)	\
    ( (This)->lpVtbl -> DrawGlyph(This,hdc,pRect) ) 

#define IVsTextStreamMarker_GetTipText(This,pbstrText)	\
    ( (This)->lpVtbl -> GetTipText(This,pbstrText) ) 

#define IVsTextStreamMarker_UnadviseClient(This)	\
    ( (This)->lpVtbl -> UnadviseClient(This) ) 

#define IVsTextStreamMarker_GetMarkerCommandInfo(This,iItem,pbstrText,pcmdf)	\
    ( (This)->lpVtbl -> GetMarkerCommandInfo(This,iItem,pbstrText,pcmdf) ) 

#define IVsTextStreamMarker_ExecMarkerCommand(This,iItem)	\
    ( (This)->lpVtbl -> ExecMarkerCommand(This,iItem) ) 

#define IVsTextStreamMarker_GetBehavior(This,pdwBehavior)	\
    ( (This)->lpVtbl -> GetBehavior(This,pdwBehavior) ) 

#define IVsTextStreamMarker_SetBehavior(This,dwBehavior)	\
    ( (This)->lpVtbl -> SetBehavior(This,dwBehavior) ) 

#define IVsTextStreamMarker_GetPriorityIndex(This,piPriorityIndex)	\
    ( (This)->lpVtbl -> GetPriorityIndex(This,piPriorityIndex) ) 


#define IVsTextStreamMarker_GetStreamBuffer(This,ppBuffer)	\
    ( (This)->lpVtbl -> GetStreamBuffer(This,ppBuffer) ) 

#define IVsTextStreamMarker_ResetSpan(This,iNewPos,iNewLen)	\
    ( (This)->lpVtbl -> ResetSpan(This,iNewPos,iNewLen) ) 

#define IVsTextStreamMarker_GetCurrentSpan(This,piPos,piLen)	\
    ( (This)->lpVtbl -> GetCurrentSpan(This,piPos,piLen) ) 

#endif /* COBJMACROS */


#endif 	/* C style interface */




#endif 	/* __IVsTextStreamMarker_INTERFACE_DEFINED__ */


#ifndef __IVsTextLineMarker_INTERFACE_DEFINED__
#define __IVsTextLineMarker_INTERFACE_DEFINED__

/* interface IVsTextLineMarker */
/* [object][uuid] */ 


EXTERN_C const IID IID_IVsTextLineMarker;

#if defined(__cplusplus) && !defined(CINTERFACE)
    
    MIDL_INTERFACE("31E2DCA7-CCFF-4E09-B433-17C739CF69AD")
    IVsTextLineMarker : public IVsTextMarker
    {
    public:
        virtual HRESULT STDMETHODCALLTYPE GetLineBuffer( 
            /* [out] */ __RPC__deref_out_opt IVsTextLines **ppBuffer) = 0;
        
        virtual HRESULT STDMETHODCALLTYPE ResetSpan( 
            /* [in] */ long iStartLine,
            /* [in] */ CharIndex iStartIndex,
            /* [in] */ long iEndLine,
            /* [in] */ CharIndex iEndIndex) = 0;
        
        virtual HRESULT STDMETHODCALLTYPE GetCurrentSpan( 
            /* [out] */ __RPC__out TextSpan *pSpan) = 0;
        
    };
    
#else 	/* C style interface */

    typedef struct IVsTextLineMarkerVtbl
    {
        BEGIN_INTERFACE
        
        HRESULT ( STDMETHODCALLTYPE *QueryInterface )( 
            IVsTextLineMarker * This,
            /* [in] */ __RPC__in REFIID riid,
            /* [iid_is][out] */ 
            __RPC__deref_out  void **ppvObject);
        
        ULONG ( STDMETHODCALLTYPE *AddRef )( 
            IVsTextLineMarker * This);
        
        ULONG ( STDMETHODCALLTYPE *Release )( 
            IVsTextLineMarker * This);
        
        HRESULT ( STDMETHODCALLTYPE *GetType )( 
            IVsTextLineMarker * This,
            /* [out] */ __RPC__out long *piMarkerType);
        
        HRESULT ( STDMETHODCALLTYPE *SetType )( 
            IVsTextLineMarker * This,
            /* [in] */ long iMarkerType);
        
        HRESULT ( STDMETHODCALLTYPE *GetVisualStyle )( 
            IVsTextLineMarker * This,
            /* [out] */ __RPC__out DWORD *pdwFlags);
        
        HRESULT ( STDMETHODCALLTYPE *SetVisualStyle )( 
            IVsTextLineMarker * This,
            /* [in] */ DWORD dwFlags);
        
        HRESULT ( STDMETHODCALLTYPE *Invalidate )( 
            IVsTextLineMarker * This);
        
        HRESULT ( STDMETHODCALLTYPE *DrawGlyph )( 
            IVsTextLineMarker * This,
            /* [in] */ __RPC__in HDC hdc,
            /* [in] */ __RPC__in RECT *pRect);
        
        HRESULT ( STDMETHODCALLTYPE *GetTipText )( 
            IVsTextLineMarker * This,
            /* [optional][out] */ __RPC__deref_out_opt BSTR *pbstrText);
        
        HRESULT ( STDMETHODCALLTYPE *UnadviseClient )( 
            IVsTextLineMarker * This);
        
        HRESULT ( STDMETHODCALLTYPE *GetMarkerCommandInfo )( 
            IVsTextLineMarker * This,
            /* [in] */ long iItem,
            /* [custom][out] */ __RPC__deref_out_opt BSTR *pbstrText,
            /* [out] */ __RPC__out DWORD *pcmdf);
        
        HRESULT ( STDMETHODCALLTYPE *ExecMarkerCommand )( 
            IVsTextLineMarker * This,
            /* [in] */ long iItem);
        
        HRESULT ( STDMETHODCALLTYPE *GetBehavior )( 
            IVsTextLineMarker * This,
            /* [out] */ __RPC__out DWORD *pdwBehavior);
        
        HRESULT ( STDMETHODCALLTYPE *SetBehavior )( 
            IVsTextLineMarker * This,
            /* [in] */ DWORD dwBehavior);
        
        HRESULT ( STDMETHODCALLTYPE *GetPriorityIndex )( 
            IVsTextLineMarker * This,
            /* [out] */ __RPC__out long *piPriorityIndex);
        
        HRESULT ( STDMETHODCALLTYPE *GetLineBuffer )( 
            IVsTextLineMarker * This,
            /* [out] */ __RPC__deref_out_opt IVsTextLines **ppBuffer);
        
        HRESULT ( STDMETHODCALLTYPE *ResetSpan )( 
            IVsTextLineMarker * This,
            /* [in] */ long iStartLine,
            /* [in] */ CharIndex iStartIndex,
            /* [in] */ long iEndLine,
            /* [in] */ CharIndex iEndIndex);
        
        HRESULT ( STDMETHODCALLTYPE *GetCurrentSpan )( 
            IVsTextLineMarker * This,
            /* [out] */ __RPC__out TextSpan *pSpan);
        
        END_INTERFACE
    } IVsTextLineMarkerVtbl;

    interface IVsTextLineMarker
    {
        CONST_VTBL struct IVsTextLineMarkerVtbl *lpVtbl;
    };

    

#ifdef COBJMACROS


#define IVsTextLineMarker_QueryInterface(This,riid,ppvObject)	\
    ( (This)->lpVtbl -> QueryInterface(This,riid,ppvObject) ) 

#define IVsTextLineMarker_AddRef(This)	\
    ( (This)->lpVtbl -> AddRef(This) ) 

#define IVsTextLineMarker_Release(This)	\
    ( (This)->lpVtbl -> Release(This) ) 


#define IVsTextLineMarker_GetType(This,piMarkerType)	\
    ( (This)->lpVtbl -> GetType(This,piMarkerType) ) 

#define IVsTextLineMarker_SetType(This,iMarkerType)	\
    ( (This)->lpVtbl -> SetType(This,iMarkerType) ) 

#define IVsTextLineMarker_GetVisualStyle(This,pdwFlags)	\
    ( (This)->lpVtbl -> GetVisualStyle(This,pdwFlags) ) 

#define IVsTextLineMarker_SetVisualStyle(This,dwFlags)	\
    ( (This)->lpVtbl -> SetVisualStyle(This,dwFlags) ) 

#define IVsTextLineMarker_Invalidate(This)	\
    ( (This)->lpVtbl -> Invalidate(This) ) 

#define IVsTextLineMarker_DrawGlyph(This,hdc,pRect)	\
    ( (This)->lpVtbl -> DrawGlyph(This,hdc,pRect) ) 

#define IVsTextLineMarker_GetTipText(This,pbstrText)	\
    ( (This)->lpVtbl -> GetTipText(This,pbstrText) ) 

#define IVsTextLineMarker_UnadviseClient(This)	\
    ( (This)->lpVtbl -> UnadviseClient(This) ) 

#define IVsTextLineMarker_GetMarkerCommandInfo(This,iItem,pbstrText,pcmdf)	\
    ( (This)->lpVtbl -> GetMarkerCommandInfo(This,iItem,pbstrText,pcmdf) ) 

#define IVsTextLineMarker_ExecMarkerCommand(This,iItem)	\
    ( (This)->lpVtbl -> ExecMarkerCommand(This,iItem) ) 

#define IVsTextLineMarker_GetBehavior(This,pdwBehavior)	\
    ( (This)->lpVtbl -> GetBehavior(This,pdwBehavior) ) 

#define IVsTextLineMarker_SetBehavior(This,dwBehavior)	\
    ( (This)->lpVtbl -> SetBehavior(This,dwBehavior) ) 

#define IVsTextLineMarker_GetPriorityIndex(This,piPriorityIndex)	\
    ( (This)->lpVtbl -> GetPriorityIndex(This,piPriorityIndex) ) 


#define IVsTextLineMarker_GetLineBuffer(This,ppBuffer)	\
    ( (This)->lpVtbl -> GetLineBuffer(This,ppBuffer) ) 

#define IVsTextLineMarker_ResetSpan(This,iStartLine,iStartIndex,iEndLine,iEndIndex)	\
    ( (This)->lpVtbl -> ResetSpan(This,iStartLine,iStartIndex,iEndLine,iEndIndex) ) 

#define IVsTextLineMarker_GetCurrentSpan(This,pSpan)	\
    ( (This)->lpVtbl -> GetCurrentSpan(This,pSpan) ) 

#endif /* COBJMACROS */


#endif 	/* C style interface */




#endif 	/* __IVsTextLineMarker_INTERFACE_DEFINED__ */


#ifndef __IVsTextLayerMarker_INTERFACE_DEFINED__
#define __IVsTextLayerMarker_INTERFACE_DEFINED__

/* interface IVsTextLayerMarker */
/* [object][uuid] */ 


EXTERN_C const IID IID_IVsTextLayerMarker;

#if defined(__cplusplus) && !defined(CINTERFACE)
    
    MIDL_INTERFACE("28C149D2-8FCB-4AB3-8584-9A2747F3F8FC")
    IVsTextLayerMarker : public IVsTextMarker
    {
    public:
        virtual HRESULT STDMETHODCALLTYPE GetTextLayer( 
            /* [out] */ __RPC__deref_out_opt IVsTextLayer **ppLayer) = 0;
        
        virtual HRESULT STDMETHODCALLTYPE ResetSpan( 
            /* [in] */ long iStartLine,
            /* [in] */ CharIndex iStartIndex,
            /* [in] */ long iEndLine,
            /* [in] */ CharIndex iEndIndex) = 0;
        
        virtual HRESULT STDMETHODCALLTYPE GetCurrentSpan( 
            /* [out] */ __RPC__out TextSpan *pSpan) = 0;
        
        virtual HRESULT STDMETHODCALLTYPE IsInvalidated( void) = 0;
        
        virtual HRESULT STDMETHODCALLTYPE QueryClientInterface( 
            /* [in] */ __RPC__in REFIID iid,
            /* [out] */ __RPC__deref_out_opt void **ppClient) = 0;
        
        virtual HRESULT STDMETHODCALLTYPE DrawGlyphEx( 
            /* [in] */ DWORD dwFlags,
            /* [in] */ __RPC__in HDC hdc,
            /* [in] */ __RPC__in RECT *pRect,
            /* [in] */ long iLineHeight) = 0;
        
    };
    
#else 	/* C style interface */

    typedef struct IVsTextLayerMarkerVtbl
    {
        BEGIN_INTERFACE
        
        HRESULT ( STDMETHODCALLTYPE *QueryInterface )( 
            IVsTextLayerMarker * This,
            /* [in] */ __RPC__in REFIID riid,
            /* [iid_is][out] */ 
            __RPC__deref_out  void **ppvObject);
        
        ULONG ( STDMETHODCALLTYPE *AddRef )( 
            IVsTextLayerMarker * This);
        
        ULONG ( STDMETHODCALLTYPE *Release )( 
            IVsTextLayerMarker * This);
        
        HRESULT ( STDMETHODCALLTYPE *GetType )( 
            IVsTextLayerMarker * This,
            /* [out] */ __RPC__out long *piMarkerType);
        
        HRESULT ( STDMETHODCALLTYPE *SetType )( 
            IVsTextLayerMarker * This,
            /* [in] */ long iMarkerType);
        
        HRESULT ( STDMETHODCALLTYPE *GetVisualStyle )( 
            IVsTextLayerMarker * This,
            /* [out] */ __RPC__out DWORD *pdwFlags);
        
        HRESULT ( STDMETHODCALLTYPE *SetVisualStyle )( 
            IVsTextLayerMarker * This,
            /* [in] */ DWORD dwFlags);
        
        HRESULT ( STDMETHODCALLTYPE *Invalidate )( 
            IVsTextLayerMarker * This);
        
        HRESULT ( STDMETHODCALLTYPE *DrawGlyph )( 
            IVsTextLayerMarker * This,
            /* [in] */ __RPC__in HDC hdc,
            /* [in] */ __RPC__in RECT *pRect);
        
        HRESULT ( STDMETHODCALLTYPE *GetTipText )( 
            IVsTextLayerMarker * This,
            /* [optional][out] */ __RPC__deref_out_opt BSTR *pbstrText);
        
        HRESULT ( STDMETHODCALLTYPE *UnadviseClient )( 
            IVsTextLayerMarker * This);
        
        HRESULT ( STDMETHODCALLTYPE *GetMarkerCommandInfo )( 
            IVsTextLayerMarker * This,
            /* [in] */ long iItem,
            /* [custom][out] */ __RPC__deref_out_opt BSTR *pbstrText,
            /* [out] */ __RPC__out DWORD *pcmdf);
        
        HRESULT ( STDMETHODCALLTYPE *ExecMarkerCommand )( 
            IVsTextLayerMarker * This,
            /* [in] */ long iItem);
        
        HRESULT ( STDMETHODCALLTYPE *GetBehavior )( 
            IVsTextLayerMarker * This,
            /* [out] */ __RPC__out DWORD *pdwBehavior);
        
        HRESULT ( STDMETHODCALLTYPE *SetBehavior )( 
            IVsTextLayerMarker * This,
            /* [in] */ DWORD dwBehavior);
        
        HRESULT ( STDMETHODCALLTYPE *GetPriorityIndex )( 
            IVsTextLayerMarker * This,
            /* [out] */ __RPC__out long *piPriorityIndex);
        
        HRESULT ( STDMETHODCALLTYPE *GetTextLayer )( 
            IVsTextLayerMarker * This,
            /* [out] */ __RPC__deref_out_opt IVsTextLayer **ppLayer);
        
        HRESULT ( STDMETHODCALLTYPE *ResetSpan )( 
            IVsTextLayerMarker * This,
            /* [in] */ long iStartLine,
            /* [in] */ CharIndex iStartIndex,
            /* [in] */ long iEndLine,
            /* [in] */ CharIndex iEndIndex);
        
        HRESULT ( STDMETHODCALLTYPE *GetCurrentSpan )( 
            IVsTextLayerMarker * This,
            /* [out] */ __RPC__out TextSpan *pSpan);
        
        HRESULT ( STDMETHODCALLTYPE *IsInvalidated )( 
            IVsTextLayerMarker * This);
        
        HRESULT ( STDMETHODCALLTYPE *QueryClientInterface )( 
            IVsTextLayerMarker * This,
            /* [in] */ __RPC__in REFIID iid,
            /* [out] */ __RPC__deref_out_opt void **ppClient);
        
        HRESULT ( STDMETHODCALLTYPE *DrawGlyphEx )( 
            IVsTextLayerMarker * This,
            /* [in] */ DWORD dwFlags,
            /* [in] */ __RPC__in HDC hdc,
            /* [in] */ __RPC__in RECT *pRect,
            /* [in] */ long iLineHeight);
        
        END_INTERFACE
    } IVsTextLayerMarkerVtbl;

    interface IVsTextLayerMarker
    {
        CONST_VTBL struct IVsTextLayerMarkerVtbl *lpVtbl;
    };

    

#ifdef COBJMACROS


#define IVsTextLayerMarker_QueryInterface(This,riid,ppvObject)	\
    ( (This)->lpVtbl -> QueryInterface(This,riid,ppvObject) ) 

#define IVsTextLayerMarker_AddRef(This)	\
    ( (This)->lpVtbl -> AddRef(This) ) 

#define IVsTextLayerMarker_Release(This)	\
    ( (This)->lpVtbl -> Release(This) ) 


#define IVsTextLayerMarker_GetType(This,piMarkerType)	\
    ( (This)->lpVtbl -> GetType(This,piMarkerType) ) 

#define IVsTextLayerMarker_SetType(This,iMarkerType)	\
    ( (This)->lpVtbl -> SetType(This,iMarkerType) ) 

#define IVsTextLayerMarker_GetVisualStyle(This,pdwFlags)	\
    ( (This)->lpVtbl -> GetVisualStyle(This,pdwFlags) ) 

#define IVsTextLayerMarker_SetVisualStyle(This,dwFlags)	\
    ( (This)->lpVtbl -> SetVisualStyle(This,dwFlags) ) 

#define IVsTextLayerMarker_Invalidate(This)	\
    ( (This)->lpVtbl -> Invalidate(This) ) 

#define IVsTextLayerMarker_DrawGlyph(This,hdc,pRect)	\
    ( (This)->lpVtbl -> DrawGlyph(This,hdc,pRect) ) 

#define IVsTextLayerMarker_GetTipText(This,pbstrText)	\
    ( (This)->lpVtbl -> GetTipText(This,pbstrText) ) 

#define IVsTextLayerMarker_UnadviseClient(This)	\
    ( (This)->lpVtbl -> UnadviseClient(This) ) 

#define IVsTextLayerMarker_GetMarkerCommandInfo(This,iItem,pbstrText,pcmdf)	\
    ( (This)->lpVtbl -> GetMarkerCommandInfo(This,iItem,pbstrText,pcmdf) ) 

#define IVsTextLayerMarker_ExecMarkerCommand(This,iItem)	\
    ( (This)->lpVtbl -> ExecMarkerCommand(This,iItem) ) 

#define IVsTextLayerMarker_GetBehavior(This,pdwBehavior)	\
    ( (This)->lpVtbl -> GetBehavior(This,pdwBehavior) ) 

#define IVsTextLayerMarker_SetBehavior(This,dwBehavior)	\
    ( (This)->lpVtbl -> SetBehavior(This,dwBehavior) ) 

#define IVsTextLayerMarker_GetPriorityIndex(This,piPriorityIndex)	\
    ( (This)->lpVtbl -> GetPriorityIndex(This,piPriorityIndex) ) 


#define IVsTextLayerMarker_GetTextLayer(This,ppLayer)	\
    ( (This)->lpVtbl -> GetTextLayer(This,ppLayer) ) 

#define IVsTextLayerMarker_ResetSpan(This,iStartLine,iStartIndex,iEndLine,iEndIndex)	\
    ( (This)->lpVtbl -> ResetSpan(This,iStartLine,iStartIndex,iEndLine,iEndIndex) ) 

#define IVsTextLayerMarker_GetCurrentSpan(This,pSpan)	\
    ( (This)->lpVtbl -> GetCurrentSpan(This,pSpan) ) 

#define IVsTextLayerMarker_IsInvalidated(This)	\
    ( (This)->lpVtbl -> IsInvalidated(This) ) 

#define IVsTextLayerMarker_QueryClientInterface(This,iid,ppClient)	\
    ( (This)->lpVtbl -> QueryClientInterface(This,iid,ppClient) ) 

#define IVsTextLayerMarker_DrawGlyphEx(This,dwFlags,hdc,pRect,iLineHeight)	\
    ( (This)->lpVtbl -> DrawGlyphEx(This,dwFlags,hdc,pRect,iLineHeight) ) 

#endif /* COBJMACROS */


#endif 	/* C style interface */




#endif 	/* __IVsTextLayerMarker_INTERFACE_DEFINED__ */


#ifndef __IVsTextMarkerType_INTERFACE_DEFINED__
#define __IVsTextMarkerType_INTERFACE_DEFINED__

/* interface IVsTextMarkerType */
/* [object][uuid] */ 


EXTERN_C const IID IID_IVsTextMarkerType;

#if defined(__cplusplus) && !defined(CINTERFACE)
    
    MIDL_INTERFACE("8C4A2B6E-6377-4C71-9940-FBA1BE6EDD6F")
    IVsTextMarkerType : public IUnknown
    {
    public:
        virtual HRESULT STDMETHODCALLTYPE GetVisualStyle( 
            /* [out] */ __RPC__out DWORD *pdwVisualFlags) = 0;
        
        virtual HRESULT STDMETHODCALLTYPE GetDefaultColors( 
            /* [out] */ __RPC__out COLORINDEX *piForeground,
            /* [out] */ __RPC__out COLORINDEX *piBackground) = 0;
        
        virtual HRESULT STDMETHODCALLTYPE GetDefaultLineStyle( 
            /* [out] */ __RPC__out COLORINDEX *piLineColor,
            /* [out] */ __RPC__out LINESTYLE *piLineIndex) = 0;
        
        virtual HRESULT STDMETHODCALLTYPE DrawGlyph( 
            /* [in] */ __RPC__in HDC hdc,
            /* [in] */ __RPC__in RECT *pRect) = 0;
        
        virtual HRESULT STDMETHODCALLTYPE GetBehaviorFlags( 
            /* [out] */ __RPC__out DWORD *pdwFlags) = 0;
        
        virtual HRESULT STDMETHODCALLTYPE GetPriorityIndex( 
            /* [out] */ __RPC__out long *piPriorityIndex) = 0;
        
        virtual HRESULT STDMETHODCALLTYPE GetDisplayName( 
            /* [out] */ __RPC__deref_out_opt BSTR *pbstrName) = 0;
        
        virtual HRESULT STDMETHODCALLTYPE DrawGlyphEx( 
            /* [in] */ DWORD dwFlags,
            /* [in] */ __RPC__in HDC hdc,
            /* [in] */ __RPC__in RECT *pRect,
            /* [in] */ long iLineHeight) = 0;
        
    };
    
#else 	/* C style interface */

    typedef struct IVsTextMarkerTypeVtbl
    {
        BEGIN_INTERFACE
        
        HRESULT ( STDMETHODCALLTYPE *QueryInterface )( 
            IVsTextMarkerType * This,
            /* [in] */ __RPC__in REFIID riid,
            /* [iid_is][out] */ 
            __RPC__deref_out  void **ppvObject);
        
        ULONG ( STDMETHODCALLTYPE *AddRef )( 
            IVsTextMarkerType * This);
        
        ULONG ( STDMETHODCALLTYPE *Release )( 
            IVsTextMarkerType * This);
        
        HRESULT ( STDMETHODCALLTYPE *GetVisualStyle )( 
            IVsTextMarkerType * This,
            /* [out] */ __RPC__out DWORD *pdwVisualFlags);
        
        HRESULT ( STDMETHODCALLTYPE *GetDefaultColors )( 
            IVsTextMarkerType * This,
            /* [out] */ __RPC__out COLORINDEX *piForeground,
            /* [out] */ __RPC__out COLORINDEX *piBackground);
        
        HRESULT ( STDMETHODCALLTYPE *GetDefaultLineStyle )( 
            IVsTextMarkerType * This,
            /* [out] */ __RPC__out COLORINDEX *piLineColor,
            /* [out] */ __RPC__out LINESTYLE *piLineIndex);
        
        HRESULT ( STDMETHODCALLTYPE *DrawGlyph )( 
            IVsTextMarkerType * This,
            /* [in] */ __RPC__in HDC hdc,
            /* [in] */ __RPC__in RECT *pRect);
        
        HRESULT ( STDMETHODCALLTYPE *GetBehaviorFlags )( 
            IVsTextMarkerType * This,
            /* [out] */ __RPC__out DWORD *pdwFlags);
        
        HRESULT ( STDMETHODCALLTYPE *GetPriorityIndex )( 
            IVsTextMarkerType * This,
            /* [out] */ __RPC__out long *piPriorityIndex);
        
        HRESULT ( STDMETHODCALLTYPE *GetDisplayName )( 
            IVsTextMarkerType * This,
            /* [out] */ __RPC__deref_out_opt BSTR *pbstrName);
        
        HRESULT ( STDMETHODCALLTYPE *DrawGlyphEx )( 
            IVsTextMarkerType * This,
            /* [in] */ DWORD dwFlags,
            /* [in] */ __RPC__in HDC hdc,
            /* [in] */ __RPC__in RECT *pRect,
            /* [in] */ long iLineHeight);
        
        END_INTERFACE
    } IVsTextMarkerTypeVtbl;

    interface IVsTextMarkerType
    {
        CONST_VTBL struct IVsTextMarkerTypeVtbl *lpVtbl;
    };

    

#ifdef COBJMACROS


#define IVsTextMarkerType_QueryInterface(This,riid,ppvObject)	\
    ( (This)->lpVtbl -> QueryInterface(This,riid,ppvObject) ) 

#define IVsTextMarkerType_AddRef(This)	\
    ( (This)->lpVtbl -> AddRef(This) ) 

#define IVsTextMarkerType_Release(This)	\
    ( (This)->lpVtbl -> Release(This) ) 


#define IVsTextMarkerType_GetVisualStyle(This,pdwVisualFlags)	\
    ( (This)->lpVtbl -> GetVisualStyle(This,pdwVisualFlags) ) 

#define IVsTextMarkerType_GetDefaultColors(This,piForeground,piBackground)	\
    ( (This)->lpVtbl -> GetDefaultColors(This,piForeground,piBackground) ) 

#define IVsTextMarkerType_GetDefaultLineStyle(This,piLineColor,piLineIndex)	\
    ( (This)->lpVtbl -> GetDefaultLineStyle(This,piLineColor,piLineIndex) ) 

#define IVsTextMarkerType_DrawGlyph(This,hdc,pRect)	\
    ( (This)->lpVtbl -> DrawGlyph(This,hdc,pRect) ) 

#define IVsTextMarkerType_GetBehaviorFlags(This,pdwFlags)	\
    ( (This)->lpVtbl -> GetBehaviorFlags(This,pdwFlags) ) 

#define IVsTextMarkerType_GetPriorityIndex(This,piPriorityIndex)	\
    ( (This)->lpVtbl -> GetPriorityIndex(This,piPriorityIndex) ) 

#define IVsTextMarkerType_GetDisplayName(This,pbstrName)	\
    ( (This)->lpVtbl -> GetDisplayName(This,pbstrName) ) 

#define IVsTextMarkerType_DrawGlyphEx(This,dwFlags,hdc,pRect,iLineHeight)	\
    ( (This)->lpVtbl -> DrawGlyphEx(This,dwFlags,hdc,pRect,iLineHeight) ) 

#endif /* COBJMACROS */


#endif 	/* C style interface */




#endif 	/* __IVsTextMarkerType_INTERFACE_DEFINED__ */


#ifndef __IVsTextMarkerColorSet_INTERFACE_DEFINED__
#define __IVsTextMarkerColorSet_INTERFACE_DEFINED__

/* interface IVsTextMarkerColorSet */
/* [object][uuid] */ 


EXTERN_C const IID IID_IVsTextMarkerColorSet;

#if defined(__cplusplus) && !defined(CINTERFACE)
    
    MIDL_INTERFACE("93373F76-DF07-46DB-9415-1772069F60A4")
    IVsTextMarkerColorSet : public IUnknown
    {
    public:
        virtual HRESULT STDMETHODCALLTYPE GetMarkerColors( 
            /* [in] */ long iMarkerType,
            /* [out] */ __RPC__out COLORREF *pclrFore,
            /* [out] */ __RPC__out COLORREF *pclrBack) = 0;
        
    };
    
#else 	/* C style interface */

    typedef struct IVsTextMarkerColorSetVtbl
    {
        BEGIN_INTERFACE
        
        HRESULT ( STDMETHODCALLTYPE *QueryInterface )( 
            IVsTextMarkerColorSet * This,
            /* [in] */ __RPC__in REFIID riid,
            /* [iid_is][out] */ 
            __RPC__deref_out  void **ppvObject);
        
        ULONG ( STDMETHODCALLTYPE *AddRef )( 
            IVsTextMarkerColorSet * This);
        
        ULONG ( STDMETHODCALLTYPE *Release )( 
            IVsTextMarkerColorSet * This);
        
        HRESULT ( STDMETHODCALLTYPE *GetMarkerColors )( 
            IVsTextMarkerColorSet * This,
            /* [in] */ long iMarkerType,
            /* [out] */ __RPC__out COLORREF *pclrFore,
            /* [out] */ __RPC__out COLORREF *pclrBack);
        
        END_INTERFACE
    } IVsTextMarkerColorSetVtbl;

    interface IVsTextMarkerColorSet
    {
        CONST_VTBL struct IVsTextMarkerColorSetVtbl *lpVtbl;
    };

    

#ifdef COBJMACROS


#define IVsTextMarkerColorSet_QueryInterface(This,riid,ppvObject)	\
    ( (This)->lpVtbl -> QueryInterface(This,riid,ppvObject) ) 

#define IVsTextMarkerColorSet_AddRef(This)	\
    ( (This)->lpVtbl -> AddRef(This) ) 

#define IVsTextMarkerColorSet_Release(This)	\
    ( (This)->lpVtbl -> Release(This) ) 


#define IVsTextMarkerColorSet_GetMarkerColors(This,iMarkerType,pclrFore,pclrBack)	\
    ( (This)->lpVtbl -> GetMarkerColors(This,iMarkerType,pclrFore,pclrBack) ) 

#endif /* COBJMACROS */


#endif 	/* C style interface */




#endif 	/* __IVsTextMarkerColorSet_INTERFACE_DEFINED__ */


#ifndef __IVsPackageDefinedTextMarkerType_INTERFACE_DEFINED__
#define __IVsPackageDefinedTextMarkerType_INTERFACE_DEFINED__

/* interface IVsPackageDefinedTextMarkerType */
/* [object][uuid] */ 


EXTERN_C const IID IID_IVsPackageDefinedTextMarkerType;

#if defined(__cplusplus) && !defined(CINTERFACE)
    
    MIDL_INTERFACE("C70F1528-1A66-403F-AD42-F60F9D413E05")
    IVsPackageDefinedTextMarkerType : public IUnknown
    {
    public:
        virtual HRESULT STDMETHODCALLTYPE GetVisualStyle( 
            /* [out] */ __RPC__out DWORD *pdwVisualFlags) = 0;
        
        virtual HRESULT STDMETHODCALLTYPE GetDefaultColors( 
            /* [out] */ __RPC__out COLORINDEX *piForeground,
            /* [out] */ __RPC__out COLORINDEX *piBackground) = 0;
        
        virtual HRESULT STDMETHODCALLTYPE GetDefaultLineStyle( 
            /* [out] */ __RPC__out COLORINDEX *piLineColor,
            /* [out] */ __RPC__out LINESTYLE *piLineIndex) = 0;
        
        virtual HRESULT STDMETHODCALLTYPE GetDefaultFontFlags( 
            /* [out] */ __RPC__out DWORD *pdwFontFlags) = 0;
        
        virtual HRESULT STDMETHODCALLTYPE DrawGlyphWithColors( 
            /* [in] */ __RPC__in HDC hdc,
            /* [in] */ __RPC__in RECT *pRect,
            /* [in] */ long iMarkerType,
            /* [in] */ __RPC__in_opt IVsTextMarkerColorSet *pMarkerColors,
            /* [in] */ DWORD dwGlyphDrawFlags,
            /* [in] */ long iLineHeight) = 0;
        
        virtual HRESULT STDMETHODCALLTYPE GetBehaviorFlags( 
            /* [out] */ __RPC__out DWORD *pdwFlags) = 0;
        
        virtual HRESULT STDMETHODCALLTYPE GetPriorityIndex( 
            /* [out] */ __RPC__out long *piPriorityIndex) = 0;
        
    };
    
#else 	/* C style interface */

    typedef struct IVsPackageDefinedTextMarkerTypeVtbl
    {
        BEGIN_INTERFACE
        
        HRESULT ( STDMETHODCALLTYPE *QueryInterface )( 
            IVsPackageDefinedTextMarkerType * This,
            /* [in] */ __RPC__in REFIID riid,
            /* [iid_is][out] */ 
            __RPC__deref_out  void **ppvObject);
        
        ULONG ( STDMETHODCALLTYPE *AddRef )( 
            IVsPackageDefinedTextMarkerType * This);
        
        ULONG ( STDMETHODCALLTYPE *Release )( 
            IVsPackageDefinedTextMarkerType * This);
        
        HRESULT ( STDMETHODCALLTYPE *GetVisualStyle )( 
            IVsPackageDefinedTextMarkerType * This,
            /* [out] */ __RPC__out DWORD *pdwVisualFlags);
        
        HRESULT ( STDMETHODCALLTYPE *GetDefaultColors )( 
            IVsPackageDefinedTextMarkerType * This,
            /* [out] */ __RPC__out COLORINDEX *piForeground,
            /* [out] */ __RPC__out COLORINDEX *piBackground);
        
        HRESULT ( STDMETHODCALLTYPE *GetDefaultLineStyle )( 
            IVsPackageDefinedTextMarkerType * This,
            /* [out] */ __RPC__out COLORINDEX *piLineColor,
            /* [out] */ __RPC__out LINESTYLE *piLineIndex);
        
        HRESULT ( STDMETHODCALLTYPE *GetDefaultFontFlags )( 
            IVsPackageDefinedTextMarkerType * This,
            /* [out] */ __RPC__out DWORD *pdwFontFlags);
        
        HRESULT ( STDMETHODCALLTYPE *DrawGlyphWithColors )( 
            IVsPackageDefinedTextMarkerType * This,
            /* [in] */ __RPC__in HDC hdc,
            /* [in] */ __RPC__in RECT *pRect,
            /* [in] */ long iMarkerType,
            /* [in] */ __RPC__in_opt IVsTextMarkerColorSet *pMarkerColors,
            /* [in] */ DWORD dwGlyphDrawFlags,
            /* [in] */ long iLineHeight);
        
        HRESULT ( STDMETHODCALLTYPE *GetBehaviorFlags )( 
            IVsPackageDefinedTextMarkerType * This,
            /* [out] */ __RPC__out DWORD *pdwFlags);
        
        HRESULT ( STDMETHODCALLTYPE *GetPriorityIndex )( 
            IVsPackageDefinedTextMarkerType * This,
            /* [out] */ __RPC__out long *piPriorityIndex);
        
        END_INTERFACE
    } IVsPackageDefinedTextMarkerTypeVtbl;

    interface IVsPackageDefinedTextMarkerType
    {
        CONST_VTBL struct IVsPackageDefinedTextMarkerTypeVtbl *lpVtbl;
    };

    

#ifdef COBJMACROS


#define IVsPackageDefinedTextMarkerType_QueryInterface(This,riid,ppvObject)	\
    ( (This)->lpVtbl -> QueryInterface(This,riid,ppvObject) ) 

#define IVsPackageDefinedTextMarkerType_AddRef(This)	\
    ( (This)->lpVtbl -> AddRef(This) ) 

#define IVsPackageDefinedTextMarkerType_Release(This)	\
    ( (This)->lpVtbl -> Release(This) ) 


#define IVsPackageDefinedTextMarkerType_GetVisualStyle(This,pdwVisualFlags)	\
    ( (This)->lpVtbl -> GetVisualStyle(This,pdwVisualFlags) ) 

#define IVsPackageDefinedTextMarkerType_GetDefaultColors(This,piForeground,piBackground)	\
    ( (This)->lpVtbl -> GetDefaultColors(This,piForeground,piBackground) ) 

#define IVsPackageDefinedTextMarkerType_GetDefaultLineStyle(This,piLineColor,piLineIndex)	\
    ( (This)->lpVtbl -> GetDefaultLineStyle(This,piLineColor,piLineIndex) ) 

#define IVsPackageDefinedTextMarkerType_GetDefaultFontFlags(This,pdwFontFlags)	\
    ( (This)->lpVtbl -> GetDefaultFontFlags(This,pdwFontFlags) ) 

#define IVsPackageDefinedTextMarkerType_DrawGlyphWithColors(This,hdc,pRect,iMarkerType,pMarkerColors,dwGlyphDrawFlags,iLineHeight)	\
    ( (This)->lpVtbl -> DrawGlyphWithColors(This,hdc,pRect,iMarkerType,pMarkerColors,dwGlyphDrawFlags,iLineHeight) ) 

#define IVsPackageDefinedTextMarkerType_GetBehaviorFlags(This,pdwFlags)	\
    ( (This)->lpVtbl -> GetBehaviorFlags(This,pdwFlags) ) 

#define IVsPackageDefinedTextMarkerType_GetPriorityIndex(This,piPriorityIndex)	\
    ( (This)->lpVtbl -> GetPriorityIndex(This,piPriorityIndex) ) 

#endif /* COBJMACROS */


#endif 	/* C style interface */




#endif 	/* __IVsPackageDefinedTextMarkerType_INTERFACE_DEFINED__ */


#ifndef __IVsTextMarkerTypeProvider_INTERFACE_DEFINED__
#define __IVsTextMarkerTypeProvider_INTERFACE_DEFINED__

/* interface IVsTextMarkerTypeProvider */
/* [object][uuid] */ 


EXTERN_C const IID IID_IVsTextMarkerTypeProvider;

#if defined(__cplusplus) && !defined(CINTERFACE)
    
    MIDL_INTERFACE("7EF86CFD-D561-4BA5-9F38-C1BFF588E3AC")
    IVsTextMarkerTypeProvider : public IUnknown
    {
    public:
        virtual HRESULT STDMETHODCALLTYPE GetTextMarkerType( 
            /* [in] */ __RPC__in GUID *pguidMarker,
            /* [out] */ __RPC__deref_out_opt IVsPackageDefinedTextMarkerType **ppMarkerType) = 0;
        
    };
    
#else 	/* C style interface */

    typedef struct IVsTextMarkerTypeProviderVtbl
    {
        BEGIN_INTERFACE
        
        HRESULT ( STDMETHODCALLTYPE *QueryInterface )( 
            IVsTextMarkerTypeProvider * This,
            /* [in] */ __RPC__in REFIID riid,
            /* [iid_is][out] */ 
            __RPC__deref_out  void **ppvObject);
        
        ULONG ( STDMETHODCALLTYPE *AddRef )( 
            IVsTextMarkerTypeProvider * This);
        
        ULONG ( STDMETHODCALLTYPE *Release )( 
            IVsTextMarkerTypeProvider * This);
        
        HRESULT ( STDMETHODCALLTYPE *GetTextMarkerType )( 
            IVsTextMarkerTypeProvider * This,
            /* [in] */ __RPC__in GUID *pguidMarker,
            /* [out] */ __RPC__deref_out_opt IVsPackageDefinedTextMarkerType **ppMarkerType);
        
        END_INTERFACE
    } IVsTextMarkerTypeProviderVtbl;

    interface IVsTextMarkerTypeProvider
    {
        CONST_VTBL struct IVsTextMarkerTypeProviderVtbl *lpVtbl;
    };

    

#ifdef COBJMACROS


#define IVsTextMarkerTypeProvider_QueryInterface(This,riid,ppvObject)	\
    ( (This)->lpVtbl -> QueryInterface(This,riid,ppvObject) ) 

#define IVsTextMarkerTypeProvider_AddRef(This)	\
    ( (This)->lpVtbl -> AddRef(This) ) 

#define IVsTextMarkerTypeProvider_Release(This)	\
    ( (This)->lpVtbl -> Release(This) ) 


#define IVsTextMarkerTypeProvider_GetTextMarkerType(This,pguidMarker,ppMarkerType)	\
    ( (This)->lpVtbl -> GetTextMarkerType(This,pguidMarker,ppMarkerType) ) 

#endif /* COBJMACROS */


#endif 	/* C style interface */




#endif 	/* __IVsTextMarkerTypeProvider_INTERFACE_DEFINED__ */


#ifndef __IVsTextMarkerClient_INTERFACE_DEFINED__
#define __IVsTextMarkerClient_INTERFACE_DEFINED__

/* interface IVsTextMarkerClient */
/* [object][uuid] */ 


EXTERN_C const IID IID_IVsTextMarkerClient;

#if defined(__cplusplus) && !defined(CINTERFACE)
    
    MIDL_INTERFACE("B1938F1B-D7A9-42F8-9960-D009027B3D2E")
    IVsTextMarkerClient : public IUnknown
    {
    public:
        virtual void STDMETHODCALLTYPE MarkerInvalidated( void) = 0;
        
        virtual HRESULT STDMETHODCALLTYPE GetTipText( 
            /* [in] */ __RPC__in_opt IVsTextMarker *pMarker,
            /* [optional][out] */ __RPC__deref_out_opt BSTR *pbstrText) = 0;
        
        virtual void STDMETHODCALLTYPE OnBufferSave( 
            __RPC__in LPCOLESTR pszFileName) = 0;
        
        virtual void STDMETHODCALLTYPE OnBeforeBufferClose( void) = 0;
        
        virtual HRESULT STDMETHODCALLTYPE GetMarkerCommandInfo( 
            /* [in] */ __RPC__in_opt IVsTextMarker *pMarker,
            /* [in] */ long iItem,
            /* [custom][out] */ __RPC__deref_out_opt BSTR *pbstrText,
            /* [out] */ __RPC__out DWORD *pcmdf) = 0;
        
        virtual HRESULT STDMETHODCALLTYPE ExecMarkerCommand( 
            /* [in] */ __RPC__in_opt IVsTextMarker *pMarker,
            /* [in] */ long iItem) = 0;
        
        virtual void STDMETHODCALLTYPE OnAfterSpanReload( void) = 0;
        
        virtual HRESULT STDMETHODCALLTYPE OnAfterMarkerChange( 
            /* [in] */ __RPC__in_opt IVsTextMarker *pMarker) = 0;
        
    };
    
#else 	/* C style interface */

    typedef struct IVsTextMarkerClientVtbl
    {
        BEGIN_INTERFACE
        
        HRESULT ( STDMETHODCALLTYPE *QueryInterface )( 
            IVsTextMarkerClient * This,
            /* [in] */ __RPC__in REFIID riid,
            /* [iid_is][out] */ 
            __RPC__deref_out  void **ppvObject);
        
        ULONG ( STDMETHODCALLTYPE *AddRef )( 
            IVsTextMarkerClient * This);
        
        ULONG ( STDMETHODCALLTYPE *Release )( 
            IVsTextMarkerClient * This);
        
        void ( STDMETHODCALLTYPE *MarkerInvalidated )( 
            IVsTextMarkerClient * This);
        
        HRESULT ( STDMETHODCALLTYPE *GetTipText )( 
            IVsTextMarkerClient * This,
            /* [in] */ __RPC__in_opt IVsTextMarker *pMarker,
            /* [optional][out] */ __RPC__deref_out_opt BSTR *pbstrText);
        
        void ( STDMETHODCALLTYPE *OnBufferSave )( 
            IVsTextMarkerClient * This,
            __RPC__in LPCOLESTR pszFileName);
        
        void ( STDMETHODCALLTYPE *OnBeforeBufferClose )( 
            IVsTextMarkerClient * This);
        
        HRESULT ( STDMETHODCALLTYPE *GetMarkerCommandInfo )( 
            IVsTextMarkerClient * This,
            /* [in] */ __RPC__in_opt IVsTextMarker *pMarker,
            /* [in] */ long iItem,
            /* [custom][out] */ __RPC__deref_out_opt BSTR *pbstrText,
            /* [out] */ __RPC__out DWORD *pcmdf);
        
        HRESULT ( STDMETHODCALLTYPE *ExecMarkerCommand )( 
            IVsTextMarkerClient * This,
            /* [in] */ __RPC__in_opt IVsTextMarker *pMarker,
            /* [in] */ long iItem);
        
        void ( STDMETHODCALLTYPE *OnAfterSpanReload )( 
            IVsTextMarkerClient * This);
        
        HRESULT ( STDMETHODCALLTYPE *OnAfterMarkerChange )( 
            IVsTextMarkerClient * This,
            /* [in] */ __RPC__in_opt IVsTextMarker *pMarker);
        
        END_INTERFACE
    } IVsTextMarkerClientVtbl;

    interface IVsTextMarkerClient
    {
        CONST_VTBL struct IVsTextMarkerClientVtbl *lpVtbl;
    };

    

#ifdef COBJMACROS


#define IVsTextMarkerClient_QueryInterface(This,riid,ppvObject)	\
    ( (This)->lpVtbl -> QueryInterface(This,riid,ppvObject) ) 

#define IVsTextMarkerClient_AddRef(This)	\
    ( (This)->lpVtbl -> AddRef(This) ) 

#define IVsTextMarkerClient_Release(This)	\
    ( (This)->lpVtbl -> Release(This) ) 


#define IVsTextMarkerClient_MarkerInvalidated(This)	\
    ( (This)->lpVtbl -> MarkerInvalidated(This) ) 

#define IVsTextMarkerClient_GetTipText(This,pMarker,pbstrText)	\
    ( (This)->lpVtbl -> GetTipText(This,pMarker,pbstrText) ) 

#define IVsTextMarkerClient_OnBufferSave(This,pszFileName)	\
    ( (This)->lpVtbl -> OnBufferSave(This,pszFileName) ) 

#define IVsTextMarkerClient_OnBeforeBufferClose(This)	\
    ( (This)->lpVtbl -> OnBeforeBufferClose(This) ) 

#define IVsTextMarkerClient_GetMarkerCommandInfo(This,pMarker,iItem,pbstrText,pcmdf)	\
    ( (This)->lpVtbl -> GetMarkerCommandInfo(This,pMarker,iItem,pbstrText,pcmdf) ) 

#define IVsTextMarkerClient_ExecMarkerCommand(This,pMarker,iItem)	\
    ( (This)->lpVtbl -> ExecMarkerCommand(This,pMarker,iItem) ) 

#define IVsTextMarkerClient_OnAfterSpanReload(This)	\
    ( (This)->lpVtbl -> OnAfterSpanReload(This) ) 

#define IVsTextMarkerClient_OnAfterMarkerChange(This,pMarker)	\
    ( (This)->lpVtbl -> OnAfterMarkerChange(This,pMarker) ) 

#endif /* COBJMACROS */


#endif 	/* C style interface */




#endif 	/* __IVsTextMarkerClient_INTERFACE_DEFINED__ */


#ifndef __IVsMouseCursorProvider_INTERFACE_DEFINED__
#define __IVsMouseCursorProvider_INTERFACE_DEFINED__

/* interface IVsMouseCursorProvider */
/* [object][uuid] */ 


EXTERN_C const IID IID_IVsMouseCursorProvider;

#if defined(__cplusplus) && !defined(CINTERFACE)
    
    MIDL_INTERFACE("E32F08DB-6364-4276-8EA8-BAA6752F7807")
    IVsMouseCursorProvider : public IUnknown
    {
    public:
        virtual HRESULT STDMETHODCALLTYPE SetMouseCursor( 
            /* [in] */ DWORD dwReserved) = 0;
        
    };
    
#else 	/* C style interface */

    typedef struct IVsMouseCursorProviderVtbl
    {
        BEGIN_INTERFACE
        
        HRESULT ( STDMETHODCALLTYPE *QueryInterface )( 
            IVsMouseCursorProvider * This,
            /* [in] */ __RPC__in REFIID riid,
            /* [iid_is][out] */ 
            __RPC__deref_out  void **ppvObject);
        
        ULONG ( STDMETHODCALLTYPE *AddRef )( 
            IVsMouseCursorProvider * This);
        
        ULONG ( STDMETHODCALLTYPE *Release )( 
            IVsMouseCursorProvider * This);
        
        HRESULT ( STDMETHODCALLTYPE *SetMouseCursor )( 
            IVsMouseCursorProvider * This,
            /* [in] */ DWORD dwReserved);
        
        END_INTERFACE
    } IVsMouseCursorProviderVtbl;

    interface IVsMouseCursorProvider
    {
        CONST_VTBL struct IVsMouseCursorProviderVtbl *lpVtbl;
    };

    

#ifdef COBJMACROS


#define IVsMouseCursorProvider_QueryInterface(This,riid,ppvObject)	\
    ( (This)->lpVtbl -> QueryInterface(This,riid,ppvObject) ) 

#define IVsMouseCursorProvider_AddRef(This)	\
    ( (This)->lpVtbl -> AddRef(This) ) 

#define IVsMouseCursorProvider_Release(This)	\
    ( (This)->lpVtbl -> Release(This) ) 


#define IVsMouseCursorProvider_SetMouseCursor(This,dwReserved)	\
    ( (This)->lpVtbl -> SetMouseCursor(This,dwReserved) ) 

#endif /* COBJMACROS */


#endif 	/* C style interface */




#endif 	/* __IVsMouseCursorProvider_INTERFACE_DEFINED__ */


#ifndef __IVsTextMarkerGlyphDropHandler_INTERFACE_DEFINED__
#define __IVsTextMarkerGlyphDropHandler_INTERFACE_DEFINED__

/* interface IVsTextMarkerGlyphDropHandler */
/* [object][uuid] */ 


EXTERN_C const IID IID_IVsTextMarkerGlyphDropHandler;

#if defined(__cplusplus) && !defined(CINTERFACE)
    
    MIDL_INTERFACE("1E220776-3C20-46E9-A691-DE549C295E26")
    IVsTextMarkerGlyphDropHandler : public IUnknown
    {
    public:
        virtual HRESULT STDMETHODCALLTYPE DrawCandidateOutlineGlyph( 
            /* [in] */ DWORD dwReserved,
            /* [in] */ __RPC__in HDC hdc,
            /* [in] */ __RPC__in RECT *pRect,
            /* [in] */ COLORREF clrref) = 0;
        
        virtual HRESULT STDMETHODCALLTYPE QueryDropLocation( 
            /* [in] */ DWORD dwReserved,
            /* [in] */ __RPC__in_opt IVsTextView *pDestView,
            /* [in] */ __RPC__in_opt IVsTextLines *pDestBuffer,
            /* [in] */ long iLine,
            /* [in] */ CharIndex iIndex,
            /* [out] */ __RPC__out DWORD *pdwDropResult) = 0;
        
        virtual HRESULT STDMETHODCALLTYPE DropAtLocation( 
            /* [in] */ DWORD dwReserved,
            /* [in] */ __RPC__in_opt IVsTextView *pDestView,
            /* [in] */ __RPC__in_opt IVsTextLines *pDestBuffer,
            /* [in] */ long iLine,
            /* [in] */ CharIndex iIndex,
            /* [out] */ __RPC__out DWORD *pdwDropResult) = 0;
        
    };
    
#else 	/* C style interface */

    typedef struct IVsTextMarkerGlyphDropHandlerVtbl
    {
        BEGIN_INTERFACE
        
        HRESULT ( STDMETHODCALLTYPE *QueryInterface )( 
            IVsTextMarkerGlyphDropHandler * This,
            /* [in] */ __RPC__in REFIID riid,
            /* [iid_is][out] */ 
            __RPC__deref_out  void **ppvObject);
        
        ULONG ( STDMETHODCALLTYPE *AddRef )( 
            IVsTextMarkerGlyphDropHandler * This);
        
        ULONG ( STDMETHODCALLTYPE *Release )( 
            IVsTextMarkerGlyphDropHandler * This);
        
        HRESULT ( STDMETHODCALLTYPE *DrawCandidateOutlineGlyph )( 
            IVsTextMarkerGlyphDropHandler * This,
            /* [in] */ DWORD dwReserved,
            /* [in] */ __RPC__in HDC hdc,
            /* [in] */ __RPC__in RECT *pRect,
            /* [in] */ COLORREF clrref);
        
        HRESULT ( STDMETHODCALLTYPE *QueryDropLocation )( 
            IVsTextMarkerGlyphDropHandler * This,
            /* [in] */ DWORD dwReserved,
            /* [in] */ __RPC__in_opt IVsTextView *pDestView,
            /* [in] */ __RPC__in_opt IVsTextLines *pDestBuffer,
            /* [in] */ long iLine,
            /* [in] */ CharIndex iIndex,
            /* [out] */ __RPC__out DWORD *pdwDropResult);
        
        HRESULT ( STDMETHODCALLTYPE *DropAtLocation )( 
            IVsTextMarkerGlyphDropHandler * This,
            /* [in] */ DWORD dwReserved,
            /* [in] */ __RPC__in_opt IVsTextView *pDestView,
            /* [in] */ __RPC__in_opt IVsTextLines *pDestBuffer,
            /* [in] */ long iLine,
            /* [in] */ CharIndex iIndex,
            /* [out] */ __RPC__out DWORD *pdwDropResult);
        
        END_INTERFACE
    } IVsTextMarkerGlyphDropHandlerVtbl;

    interface IVsTextMarkerGlyphDropHandler
    {
        CONST_VTBL struct IVsTextMarkerGlyphDropHandlerVtbl *lpVtbl;
    };

    

#ifdef COBJMACROS


#define IVsTextMarkerGlyphDropHandler_QueryInterface(This,riid,ppvObject)	\
    ( (This)->lpVtbl -> QueryInterface(This,riid,ppvObject) ) 

#define IVsTextMarkerGlyphDropHandler_AddRef(This)	\
    ( (This)->lpVtbl -> AddRef(This) ) 

#define IVsTextMarkerGlyphDropHandler_Release(This)	\
    ( (This)->lpVtbl -> Release(This) ) 


#define IVsTextMarkerGlyphDropHandler_DrawCandidateOutlineGlyph(This,dwReserved,hdc,pRect,clrref)	\
    ( (This)->lpVtbl -> DrawCandidateOutlineGlyph(This,dwReserved,hdc,pRect,clrref) ) 

#define IVsTextMarkerGlyphDropHandler_QueryDropLocation(This,dwReserved,pDestView,pDestBuffer,iLine,iIndex,pdwDropResult)	\
    ( (This)->lpVtbl -> QueryDropLocation(This,dwReserved,pDestView,pDestBuffer,iLine,iIndex,pdwDropResult) ) 

#define IVsTextMarkerGlyphDropHandler_DropAtLocation(This,dwReserved,pDestView,pDestBuffer,iLine,iIndex,pdwDropResult)	\
    ( (This)->lpVtbl -> DropAtLocation(This,dwReserved,pDestView,pDestBuffer,iLine,iIndex,pdwDropResult) ) 

#endif /* COBJMACROS */


#endif 	/* C style interface */




#endif 	/* __IVsTextMarkerGlyphDropHandler_INTERFACE_DEFINED__ */


#ifndef __IVsTextTrackingPoint_INTERFACE_DEFINED__
#define __IVsTextTrackingPoint_INTERFACE_DEFINED__

/* interface IVsTextTrackingPoint */
/* [object][uuid] */ 


EXTERN_C const IID IID_IVsTextTrackingPoint;

#if defined(__cplusplus) && !defined(CINTERFACE)
    
    MIDL_INTERFACE("D6BF0A8A-3798-49C5-8806-648A635EACC8")
    IVsTextTrackingPoint : public IUnknown
    {
    public:
        virtual HRESULT STDMETHODCALLTYPE GetTextLayer( 
            /* [out] */ __RPC__deref_out_opt IVsTextLayer **ppLayer) = 0;
        
        virtual HRESULT STDMETHODCALLTYPE GetCurrentLineIndex( 
            /* [out] */ __RPC__out long *piLine,
            /* [out] */ __RPC__out long *piIndex) = 0;
        
        virtual HRESULT STDMETHODCALLTYPE GetBehavior( 
            /* [out] */ __RPC__out DWORD *pdwBehavior) = 0;
        
        virtual HRESULT STDMETHODCALLTYPE SetBehavior( 
            /* [in] */ DWORD dwBehavior) = 0;
        
    };
    
#else 	/* C style interface */

    typedef struct IVsTextTrackingPointVtbl
    {
        BEGIN_INTERFACE
        
        HRESULT ( STDMETHODCALLTYPE *QueryInterface )( 
            IVsTextTrackingPoint * This,
            /* [in] */ __RPC__in REFIID riid,
            /* [iid_is][out] */ 
            __RPC__deref_out  void **ppvObject);
        
        ULONG ( STDMETHODCALLTYPE *AddRef )( 
            IVsTextTrackingPoint * This);
        
        ULONG ( STDMETHODCALLTYPE *Release )( 
            IVsTextTrackingPoint * This);
        
        HRESULT ( STDMETHODCALLTYPE *GetTextLayer )( 
            IVsTextTrackingPoint * This,
            /* [out] */ __RPC__deref_out_opt IVsTextLayer **ppLayer);
        
        HRESULT ( STDMETHODCALLTYPE *GetCurrentLineIndex )( 
            IVsTextTrackingPoint * This,
            /* [out] */ __RPC__out long *piLine,
            /* [out] */ __RPC__out long *piIndex);
        
        HRESULT ( STDMETHODCALLTYPE *GetBehavior )( 
            IVsTextTrackingPoint * This,
            /* [out] */ __RPC__out DWORD *pdwBehavior);
        
        HRESULT ( STDMETHODCALLTYPE *SetBehavior )( 
            IVsTextTrackingPoint * This,
            /* [in] */ DWORD dwBehavior);
        
        END_INTERFACE
    } IVsTextTrackingPointVtbl;

    interface IVsTextTrackingPoint
    {
        CONST_VTBL struct IVsTextTrackingPointVtbl *lpVtbl;
    };

    

#ifdef COBJMACROS


#define IVsTextTrackingPoint_QueryInterface(This,riid,ppvObject)	\
    ( (This)->lpVtbl -> QueryInterface(This,riid,ppvObject) ) 

#define IVsTextTrackingPoint_AddRef(This)	\
    ( (This)->lpVtbl -> AddRef(This) ) 

#define IVsTextTrackingPoint_Release(This)	\
    ( (This)->lpVtbl -> Release(This) ) 


#define IVsTextTrackingPoint_GetTextLayer(This,ppLayer)	\
    ( (This)->lpVtbl -> GetTextLayer(This,ppLayer) ) 

#define IVsTextTrackingPoint_GetCurrentLineIndex(This,piLine,piIndex)	\
    ( (This)->lpVtbl -> GetCurrentLineIndex(This,piLine,piIndex) ) 

#define IVsTextTrackingPoint_GetBehavior(This,pdwBehavior)	\
    ( (This)->lpVtbl -> GetBehavior(This,pdwBehavior) ) 

#define IVsTextTrackingPoint_SetBehavior(This,dwBehavior)	\
    ( (This)->lpVtbl -> SetBehavior(This,dwBehavior) ) 

#endif /* COBJMACROS */


#endif 	/* C style interface */




#endif 	/* __IVsTextTrackingPoint_INTERFACE_DEFINED__ */


#ifndef __IVsFullTextScanner_INTERFACE_DEFINED__
#define __IVsFullTextScanner_INTERFACE_DEFINED__

/* interface IVsFullTextScanner */
/* [object][uuid] */ 


EXTERN_C const IID IID_IVsFullTextScanner;

#if defined(__cplusplus) && !defined(CINTERFACE)
    
    MIDL_INTERFACE("BE93F7B1-F2FD-45B4-938D-1C66441CD421")
    IVsFullTextScanner : public IUnknown
    {
    public:
        virtual HRESULT STDMETHODCALLTYPE OpenFullTextScan( void) = 0;
        
        virtual HRESULT STDMETHODCALLTYPE FullTextRead( 
            /* [out] */ __RPC__deref_out_opt const WCHAR **ppszText,
            /* [out] */ __RPC__out long *piLength) = 0;
        
        virtual HRESULT STDMETHODCALLTYPE CloseFullTextScan( void) = 0;
        
    };
    
#else 	/* C style interface */

    typedef struct IVsFullTextScannerVtbl
    {
        BEGIN_INTERFACE
        
        HRESULT ( STDMETHODCALLTYPE *QueryInterface )( 
            IVsFullTextScanner * This,
            /* [in] */ __RPC__in REFIID riid,
            /* [iid_is][out] */ 
            __RPC__deref_out  void **ppvObject);
        
        ULONG ( STDMETHODCALLTYPE *AddRef )( 
            IVsFullTextScanner * This);
        
        ULONG ( STDMETHODCALLTYPE *Release )( 
            IVsFullTextScanner * This);
        
        HRESULT ( STDMETHODCALLTYPE *OpenFullTextScan )( 
            IVsFullTextScanner * This);
        
        HRESULT ( STDMETHODCALLTYPE *FullTextRead )( 
            IVsFullTextScanner * This,
            /* [out] */ __RPC__deref_out_opt const WCHAR **ppszText,
            /* [out] */ __RPC__out long *piLength);
        
        HRESULT ( STDMETHODCALLTYPE *CloseFullTextScan )( 
            IVsFullTextScanner * This);
        
        END_INTERFACE
    } IVsFullTextScannerVtbl;

    interface IVsFullTextScanner
    {
        CONST_VTBL struct IVsFullTextScannerVtbl *lpVtbl;
    };

    

#ifdef COBJMACROS


#define IVsFullTextScanner_QueryInterface(This,riid,ppvObject)	\
    ( (This)->lpVtbl -> QueryInterface(This,riid,ppvObject) ) 

#define IVsFullTextScanner_AddRef(This)	\
    ( (This)->lpVtbl -> AddRef(This) ) 

#define IVsFullTextScanner_Release(This)	\
    ( (This)->lpVtbl -> Release(This) ) 


#define IVsFullTextScanner_OpenFullTextScan(This)	\
    ( (This)->lpVtbl -> OpenFullTextScan(This) ) 

#define IVsFullTextScanner_FullTextRead(This,ppszText,piLength)	\
    ( (This)->lpVtbl -> FullTextRead(This,ppszText,piLength) ) 

#define IVsFullTextScanner_CloseFullTextScan(This)	\
    ( (This)->lpVtbl -> CloseFullTextScan(This) ) 

#endif /* COBJMACROS */


#endif 	/* C style interface */




#endif 	/* __IVsFullTextScanner_INTERFACE_DEFINED__ */


#ifndef __IVsTextScanner_INTERFACE_DEFINED__
#define __IVsTextScanner_INTERFACE_DEFINED__

/* interface IVsTextScanner */
/* [object][uuid] */ 


EXTERN_C const IID IID_IVsTextScanner;

#if defined(__cplusplus) && !defined(CINTERFACE)
    
    MIDL_INTERFACE("EBD73923-C190-45C1-91C5-0BC61A8AAB14")
    IVsTextScanner : public IUnknown
    {
    public:
        virtual HRESULT STDMETHODCALLTYPE OpenScan( void) = 0;
        
        virtual HRESULT STDMETHODCALLTYPE Read( 
            /* [out] */ __RPC__deref_out_opt const WCHAR **ppszText,
            /* [out] */ __RPC__out long *piLength) = 0;
        
        virtual HRESULT STDMETHODCALLTYPE CloseScan( void) = 0;
        
    };
    
#else 	/* C style interface */

    typedef struct IVsTextScannerVtbl
    {
        BEGIN_INTERFACE
        
        HRESULT ( STDMETHODCALLTYPE *QueryInterface )( 
            IVsTextScanner * This,
            /* [in] */ __RPC__in REFIID riid,
            /* [iid_is][out] */ 
            __RPC__deref_out  void **ppvObject);
        
        ULONG ( STDMETHODCALLTYPE *AddRef )( 
            IVsTextScanner * This);
        
        ULONG ( STDMETHODCALLTYPE *Release )( 
            IVsTextScanner * This);
        
        HRESULT ( STDMETHODCALLTYPE *OpenScan )( 
            IVsTextScanner * This);
        
        HRESULT ( STDMETHODCALLTYPE *Read )( 
            IVsTextScanner * This,
            /* [out] */ __RPC__deref_out_opt const WCHAR **ppszText,
            /* [out] */ __RPC__out long *piLength);
        
        HRESULT ( STDMETHODCALLTYPE *CloseScan )( 
            IVsTextScanner * This);
        
        END_INTERFACE
    } IVsTextScannerVtbl;

    interface IVsTextScanner
    {
        CONST_VTBL struct IVsTextScannerVtbl *lpVtbl;
    };

    

#ifdef COBJMACROS


#define IVsTextScanner_QueryInterface(This,riid,ppvObject)	\
    ( (This)->lpVtbl -> QueryInterface(This,riid,ppvObject) ) 

#define IVsTextScanner_AddRef(This)	\
    ( (This)->lpVtbl -> AddRef(This) ) 

#define IVsTextScanner_Release(This)	\
    ( (This)->lpVtbl -> Release(This) ) 


#define IVsTextScanner_OpenScan(This)	\
    ( (This)->lpVtbl -> OpenScan(This) ) 

#define IVsTextScanner_Read(This,ppszText,piLength)	\
    ( (This)->lpVtbl -> Read(This,ppszText,piLength) ) 

#define IVsTextScanner_CloseScan(This)	\
    ( (This)->lpVtbl -> CloseScan(This) ) 

#endif /* COBJMACROS */


#endif 	/* C style interface */




#endif 	/* __IVsTextScanner_INTERFACE_DEFINED__ */


#ifndef __IVsTextFind_INTERFACE_DEFINED__
#define __IVsTextFind_INTERFACE_DEFINED__

/* interface IVsTextFind */
/* [object][uuid] */ 


EXTERN_C const IID IID_IVsTextFind;

#if defined(__cplusplus) && !defined(CINTERFACE)
    
    MIDL_INTERFACE("0BF16544-9CEC-40E1-80B1-E467E21B97FE")
    IVsTextFind : public IUnknown
    {
    public:
        virtual HRESULT STDMETHODCALLTYPE Find( 
            /* [in] */ __RPC__in const WCHAR *pszText,
            /* [in] */ long iStartLine,
            /* [in] */ CharIndex iStartIndex,
            /* [in] */ long iEndLine,
            /* [in] */ CharIndex iEndIndex,
            /* [in] */ long iFlags,
            /* [out] */ __RPC__out long *piLine,
            /* [out] */ __RPC__out long *piCol) = 0;
        
    };
    
#else 	/* C style interface */

    typedef struct IVsTextFindVtbl
    {
        BEGIN_INTERFACE
        
        HRESULT ( STDMETHODCALLTYPE *QueryInterface )( 
            IVsTextFind * This,
            /* [in] */ __RPC__in REFIID riid,
            /* [iid_is][out] */ 
            __RPC__deref_out  void **ppvObject);
        
        ULONG ( STDMETHODCALLTYPE *AddRef )( 
            IVsTextFind * This);
        
        ULONG ( STDMETHODCALLTYPE *Release )( 
            IVsTextFind * This);
        
        HRESULT ( STDMETHODCALLTYPE *Find )( 
            IVsTextFind * This,
            /* [in] */ __RPC__in const WCHAR *pszText,
            /* [in] */ long iStartLine,
            /* [in] */ CharIndex iStartIndex,
            /* [in] */ long iEndLine,
            /* [in] */ CharIndex iEndIndex,
            /* [in] */ long iFlags,
            /* [out] */ __RPC__out long *piLine,
            /* [out] */ __RPC__out long *piCol);
        
        END_INTERFACE
    } IVsTextFindVtbl;

    interface IVsTextFind
    {
        CONST_VTBL struct IVsTextFindVtbl *lpVtbl;
    };

    

#ifdef COBJMACROS


#define IVsTextFind_QueryInterface(This,riid,ppvObject)	\
    ( (This)->lpVtbl -> QueryInterface(This,riid,ppvObject) ) 

#define IVsTextFind_AddRef(This)	\
    ( (This)->lpVtbl -> AddRef(This) ) 

#define IVsTextFind_Release(This)	\
    ( (This)->lpVtbl -> Release(This) ) 


#define IVsTextFind_Find(This,pszText,iStartLine,iStartIndex,iEndLine,iEndIndex,iFlags,piLine,piCol)	\
    ( (This)->lpVtbl -> Find(This,pszText,iStartLine,iStartIndex,iEndLine,iEndIndex,iFlags,piLine,piCol) ) 

#endif /* COBJMACROS */


#endif 	/* C style interface */




#endif 	/* __IVsTextFind_INTERFACE_DEFINED__ */


#ifndef __IVsTextBufferEvents_INTERFACE_DEFINED__
#define __IVsTextBufferEvents_INTERFACE_DEFINED__

/* interface IVsTextBufferEvents */
/* [object][uuid] */ 


EXTERN_C const IID IID_IVsTextBufferEvents;

#if defined(__cplusplus) && !defined(CINTERFACE)
    
    MIDL_INTERFACE("44B0CAB3-6DF8-4931-9064-8FD18AFBC88E")
    IVsTextBufferEvents : public IUnknown
    {
    public:
        virtual void STDMETHODCALLTYPE OnNewLanguageService( 
            /* [in] */ __RPC__in REFGUID sidLangServiceID) = 0;
        
    };
    
#else 	/* C style interface */

    typedef struct IVsTextBufferEventsVtbl
    {
        BEGIN_INTERFACE
        
        HRESULT ( STDMETHODCALLTYPE *QueryInterface )( 
            IVsTextBufferEvents * This,
            /* [in] */ __RPC__in REFIID riid,
            /* [iid_is][out] */ 
            __RPC__deref_out  void **ppvObject);
        
        ULONG ( STDMETHODCALLTYPE *AddRef )( 
            IVsTextBufferEvents * This);
        
        ULONG ( STDMETHODCALLTYPE *Release )( 
            IVsTextBufferEvents * This);
        
        void ( STDMETHODCALLTYPE *OnNewLanguageService )( 
            IVsTextBufferEvents * This,
            /* [in] */ __RPC__in REFGUID sidLangServiceID);
        
        END_INTERFACE
    } IVsTextBufferEventsVtbl;

    interface IVsTextBufferEvents
    {
        CONST_VTBL struct IVsTextBufferEventsVtbl *lpVtbl;
    };

    

#ifdef COBJMACROS


#define IVsTextBufferEvents_QueryInterface(This,riid,ppvObject)	\
    ( (This)->lpVtbl -> QueryInterface(This,riid,ppvObject) ) 

#define IVsTextBufferEvents_AddRef(This)	\
    ( (This)->lpVtbl -> AddRef(This) ) 

#define IVsTextBufferEvents_Release(This)	\
    ( (This)->lpVtbl -> Release(This) ) 


#define IVsTextBufferEvents_OnNewLanguageService(This,sidLangServiceID)	\
    ( (This)->lpVtbl -> OnNewLanguageService(This,sidLangServiceID) ) 

#endif /* COBJMACROS */


#endif 	/* C style interface */




#endif 	/* __IVsTextBufferEvents_INTERFACE_DEFINED__ */


#ifndef __IVsTextBufferDataEvents_INTERFACE_DEFINED__
#define __IVsTextBufferDataEvents_INTERFACE_DEFINED__

/* interface IVsTextBufferDataEvents */
/* [object][uuid] */ 


EXTERN_C const IID IID_IVsTextBufferDataEvents;

#if defined(__cplusplus) && !defined(CINTERFACE)
    
    MIDL_INTERFACE("B7515E7A-70F0-44ED-96B7-FB7EB6450C10")
    IVsTextBufferDataEvents : public IUnknown
    {
    public:
        virtual void STDMETHODCALLTYPE OnFileChanged( 
            /* [in] */ DWORD grfChange,
            /* [in] */ DWORD dwFileAttrs) = 0;
        
        virtual HRESULT STDMETHODCALLTYPE OnLoadCompleted( 
            /* [in] */ BOOL fReload) = 0;
        
    };
    
#else 	/* C style interface */

    typedef struct IVsTextBufferDataEventsVtbl
    {
        BEGIN_INTERFACE
        
        HRESULT ( STDMETHODCALLTYPE *QueryInterface )( 
            IVsTextBufferDataEvents * This,
            /* [in] */ __RPC__in REFIID riid,
            /* [iid_is][out] */ 
            __RPC__deref_out  void **ppvObject);
        
        ULONG ( STDMETHODCALLTYPE *AddRef )( 
            IVsTextBufferDataEvents * This);
        
        ULONG ( STDMETHODCALLTYPE *Release )( 
            IVsTextBufferDataEvents * This);
        
        void ( STDMETHODCALLTYPE *OnFileChanged )( 
            IVsTextBufferDataEvents * This,
            /* [in] */ DWORD grfChange,
            /* [in] */ DWORD dwFileAttrs);
        
        HRESULT ( STDMETHODCALLTYPE *OnLoadCompleted )( 
            IVsTextBufferDataEvents * This,
            /* [in] */ BOOL fReload);
        
        END_INTERFACE
    } IVsTextBufferDataEventsVtbl;

    interface IVsTextBufferDataEvents
    {
        CONST_VTBL struct IVsTextBufferDataEventsVtbl *lpVtbl;
    };

    

#ifdef COBJMACROS


#define IVsTextBufferDataEvents_QueryInterface(This,riid,ppvObject)	\
    ( (This)->lpVtbl -> QueryInterface(This,riid,ppvObject) ) 

#define IVsTextBufferDataEvents_AddRef(This)	\
    ( (This)->lpVtbl -> AddRef(This) ) 

#define IVsTextBufferDataEvents_Release(This)	\
    ( (This)->lpVtbl -> Release(This) ) 


#define IVsTextBufferDataEvents_OnFileChanged(This,grfChange,dwFileAttrs)	\
    ( (This)->lpVtbl -> OnFileChanged(This,grfChange,dwFileAttrs) ) 

#define IVsTextBufferDataEvents_OnLoadCompleted(This,fReload)	\
    ( (This)->lpVtbl -> OnLoadCompleted(This,fReload) ) 

#endif /* COBJMACROS */


#endif 	/* C style interface */




#endif 	/* __IVsTextBufferDataEvents_INTERFACE_DEFINED__ */


#ifndef __IVsTextStreamEvents_INTERFACE_DEFINED__
#define __IVsTextStreamEvents_INTERFACE_DEFINED__

/* interface IVsTextStreamEvents */
/* [object][uuid] */ 


EXTERN_C const IID IID_IVsTextStreamEvents;

#if defined(__cplusplus) && !defined(CINTERFACE)
    
    MIDL_INTERFACE("96FC7D44-BCDD-4F00-AE4D-07E26B2C0E52")
    IVsTextStreamEvents : public IUnknown
    {
    public:
        virtual void STDMETHODCALLTYPE OnChangeStreamText( 
            /* [in] */ long iPos,
            /* [in] */ long iOldLen,
            /* [in] */ long iNewLen,
            /* [in] */ BOOL fLast) = 0;
        
        virtual void STDMETHODCALLTYPE OnChangeStreamAttributes( 
            /* [in] */ long iPos,
            /* [in] */ long iLength) = 0;
        
    };
    
#else 	/* C style interface */

    typedef struct IVsTextStreamEventsVtbl
    {
        BEGIN_INTERFACE
        
        HRESULT ( STDMETHODCALLTYPE *QueryInterface )( 
            IVsTextStreamEvents * This,
            /* [in] */ __RPC__in REFIID riid,
            /* [iid_is][out] */ 
            __RPC__deref_out  void **ppvObject);
        
        ULONG ( STDMETHODCALLTYPE *AddRef )( 
            IVsTextStreamEvents * This);
        
        ULONG ( STDMETHODCALLTYPE *Release )( 
            IVsTextStreamEvents * This);
        
        void ( STDMETHODCALLTYPE *OnChangeStreamText )( 
            IVsTextStreamEvents * This,
            /* [in] */ long iPos,
            /* [in] */ long iOldLen,
            /* [in] */ long iNewLen,
            /* [in] */ BOOL fLast);
        
        void ( STDMETHODCALLTYPE *OnChangeStreamAttributes )( 
            IVsTextStreamEvents * This,
            /* [in] */ long iPos,
            /* [in] */ long iLength);
        
        END_INTERFACE
    } IVsTextStreamEventsVtbl;

    interface IVsTextStreamEvents
    {
        CONST_VTBL struct IVsTextStreamEventsVtbl *lpVtbl;
    };

    

#ifdef COBJMACROS


#define IVsTextStreamEvents_QueryInterface(This,riid,ppvObject)	\
    ( (This)->lpVtbl -> QueryInterface(This,riid,ppvObject) ) 

#define IVsTextStreamEvents_AddRef(This)	\
    ( (This)->lpVtbl -> AddRef(This) ) 

#define IVsTextStreamEvents_Release(This)	\
    ( (This)->lpVtbl -> Release(This) ) 


#define IVsTextStreamEvents_OnChangeStreamText(This,iPos,iOldLen,iNewLen,fLast)	\
    ( (This)->lpVtbl -> OnChangeStreamText(This,iPos,iOldLen,iNewLen,fLast) ) 

#define IVsTextStreamEvents_OnChangeStreamAttributes(This,iPos,iLength)	\
    ( (This)->lpVtbl -> OnChangeStreamAttributes(This,iPos,iLength) ) 

#endif /* COBJMACROS */


#endif 	/* C style interface */




#endif 	/* __IVsTextStreamEvents_INTERFACE_DEFINED__ */


#ifndef __IVsSplitPane_INTERFACE_DEFINED__
#define __IVsSplitPane_INTERFACE_DEFINED__

/* interface IVsSplitPane */
/* [object][uuid] */ 


EXTERN_C const IID IID_IVsSplitPane;

#if defined(__cplusplus) && !defined(CINTERFACE)
    
    MIDL_INTERFACE("721D4D8E-E987-4712-B878-88A0E7DBA117")
    IVsSplitPane : public IUnknown
    {
    public:
        virtual HRESULT STDMETHODCALLTYPE SetParentSplitter( 
            /* [in] */ __RPC__in_opt IVsSplitter *pSplitter) = 0;
        
        virtual HRESULT STDMETHODCALLTYPE GetParentSplitter( 
            /* [out] */ __RPC__deref_out_opt IVsSplitter **ppSplitter) = 0;
        
        virtual HRESULT STDMETHODCALLTYPE SetFocus( void) = 0;
        
        virtual HRESULT STDMETHODCALLTYPE SetPosition( 
            /* [in] */ long iLeft,
            /* [in] */ long iTop,
            /* [in] */ long iRight,
            /* [in] */ long iBottom) = 0;
        
        virtual HRESULT STDMETHODCALLTYPE GetPosition( 
            /* [out] */ __RPC__out long *piLeft,
            /* [out] */ __RPC__out long *piTop,
            /* [out] */ __RPC__out long *piRight,
            /* [out] */ __RPC__out long *piBottom) = 0;
        
        virtual HRESULT STDMETHODCALLTYPE GetSizeExtents( 
            /* [out] */ __RPC__out long *piMinHorz,
            /* [out] */ __RPC__out long *piMaxHorz,
            /* [out] */ __RPC__out long *piMinVert,
            /* [out] */ __RPC__out long *piMaxVert) = 0;
        
        virtual HRESULT STDMETHODCALLTYPE GetParentWindow( 
            /* [out] */ __RPC__deref_out_opt HWND *phwndParent) = 0;
        
        virtual HRESULT STDMETHODCALLTYPE GetPaneFlags( 
            /* [out] */ __RPC__out DWORD *pdwPaneFlags) = 0;
        
        virtual HRESULT STDMETHODCALLTYPE ClosePane( void) = 0;
        
        virtual HRESULT STDMETHODCALLTYPE NotifyPane( 
            /* [in] */ SPLITNOTIFYCODE iCode) = 0;
        
        virtual HRESULT STDMETHODCALLTYPE DetachPane( void) = 0;
        
    };
    
#else 	/* C style interface */

    typedef struct IVsSplitPaneVtbl
    {
        BEGIN_INTERFACE
        
        HRESULT ( STDMETHODCALLTYPE *QueryInterface )( 
            IVsSplitPane * This,
            /* [in] */ __RPC__in REFIID riid,
            /* [iid_is][out] */ 
            __RPC__deref_out  void **ppvObject);
        
        ULONG ( STDMETHODCALLTYPE *AddRef )( 
            IVsSplitPane * This);
        
        ULONG ( STDMETHODCALLTYPE *Release )( 
            IVsSplitPane * This);
        
        HRESULT ( STDMETHODCALLTYPE *SetParentSplitter )( 
            IVsSplitPane * This,
            /* [in] */ __RPC__in_opt IVsSplitter *pSplitter);
        
        HRESULT ( STDMETHODCALLTYPE *GetParentSplitter )( 
            IVsSplitPane * This,
            /* [out] */ __RPC__deref_out_opt IVsSplitter **ppSplitter);
        
        HRESULT ( STDMETHODCALLTYPE *SetFocus )( 
            IVsSplitPane * This);
        
        HRESULT ( STDMETHODCALLTYPE *SetPosition )( 
            IVsSplitPane * This,
            /* [in] */ long iLeft,
            /* [in] */ long iTop,
            /* [in] */ long iRight,
            /* [in] */ long iBottom);
        
        HRESULT ( STDMETHODCALLTYPE *GetPosition )( 
            IVsSplitPane * This,
            /* [out] */ __RPC__out long *piLeft,
            /* [out] */ __RPC__out long *piTop,
            /* [out] */ __RPC__out long *piRight,
            /* [out] */ __RPC__out long *piBottom);
        
        HRESULT ( STDMETHODCALLTYPE *GetSizeExtents )( 
            IVsSplitPane * This,
            /* [out] */ __RPC__out long *piMinHorz,
            /* [out] */ __RPC__out long *piMaxHorz,
            /* [out] */ __RPC__out long *piMinVert,
            /* [out] */ __RPC__out long *piMaxVert);
        
        HRESULT ( STDMETHODCALLTYPE *GetParentWindow )( 
            IVsSplitPane * This,
            /* [out] */ __RPC__deref_out_opt HWND *phwndParent);
        
        HRESULT ( STDMETHODCALLTYPE *GetPaneFlags )( 
            IVsSplitPane * This,
            /* [out] */ __RPC__out DWORD *pdwPaneFlags);
        
        HRESULT ( STDMETHODCALLTYPE *ClosePane )( 
            IVsSplitPane * This);
        
        HRESULT ( STDMETHODCALLTYPE *NotifyPane )( 
            IVsSplitPane * This,
            /* [in] */ SPLITNOTIFYCODE iCode);
        
        HRESULT ( STDMETHODCALLTYPE *DetachPane )( 
            IVsSplitPane * This);
        
        END_INTERFACE
    } IVsSplitPaneVtbl;

    interface IVsSplitPane
    {
        CONST_VTBL struct IVsSplitPaneVtbl *lpVtbl;
    };

    

#ifdef COBJMACROS


#define IVsSplitPane_QueryInterface(This,riid,ppvObject)	\
    ( (This)->lpVtbl -> QueryInterface(This,riid,ppvObject) ) 

#define IVsSplitPane_AddRef(This)	\
    ( (This)->lpVtbl -> AddRef(This) ) 

#define IVsSplitPane_Release(This)	\
    ( (This)->lpVtbl -> Release(This) ) 


#define IVsSplitPane_SetParentSplitter(This,pSplitter)	\
    ( (This)->lpVtbl -> SetParentSplitter(This,pSplitter) ) 

#define IVsSplitPane_GetParentSplitter(This,ppSplitter)	\
    ( (This)->lpVtbl -> GetParentSplitter(This,ppSplitter) ) 

#define IVsSplitPane_SetFocus(This)	\
    ( (This)->lpVtbl -> SetFocus(This) ) 

#define IVsSplitPane_SetPosition(This,iLeft,iTop,iRight,iBottom)	\
    ( (This)->lpVtbl -> SetPosition(This,iLeft,iTop,iRight,iBottom) ) 

#define IVsSplitPane_GetPosition(This,piLeft,piTop,piRight,piBottom)	\
    ( (This)->lpVtbl -> GetPosition(This,piLeft,piTop,piRight,piBottom) ) 

#define IVsSplitPane_GetSizeExtents(This,piMinHorz,piMaxHorz,piMinVert,piMaxVert)	\
    ( (This)->lpVtbl -> GetSizeExtents(This,piMinHorz,piMaxHorz,piMinVert,piMaxVert) ) 

#define IVsSplitPane_GetParentWindow(This,phwndParent)	\
    ( (This)->lpVtbl -> GetParentWindow(This,phwndParent) ) 

#define IVsSplitPane_GetPaneFlags(This,pdwPaneFlags)	\
    ( (This)->lpVtbl -> GetPaneFlags(This,pdwPaneFlags) ) 

#define IVsSplitPane_ClosePane(This)	\
    ( (This)->lpVtbl -> ClosePane(This) ) 

#define IVsSplitPane_NotifyPane(This,iCode)	\
    ( (This)->lpVtbl -> NotifyPane(This,iCode) ) 

#define IVsSplitPane_DetachPane(This)	\
    ( (This)->lpVtbl -> DetachPane(This) ) 

#endif /* COBJMACROS */


#endif 	/* C style interface */




#endif 	/* __IVsSplitPane_INTERFACE_DEFINED__ */


#ifndef __IVsSplitter_INTERFACE_DEFINED__
#define __IVsSplitter_INTERFACE_DEFINED__

/* interface IVsSplitter */
/* [object][uuid] */ 


EXTERN_C const IID IID_IVsSplitter;

#if defined(__cplusplus) && !defined(CINTERFACE)
    
    MIDL_INTERFACE("66280096-A495-48E9-8AF4-E758FDE1FCDC")
    IVsSplitter : public IVsSplitPane
    {
    public:
        virtual HRESULT STDMETHODCALLTYPE GetPane( 
            /* [in] */ long iIndex,
            /* [out] */ __RPC__deref_out_opt IVsSplitPane **ppPane) = 0;
        
        virtual HRESULT STDMETHODCALLTYPE NotifySplitter( 
            /* [in] */ __RPC__in_opt IVsSplitPane *pPane,
            /* [in] */ PANEEVENTS iEvent) = 0;
        
        virtual HRESULT STDMETHODCALLTYPE IsPanePrimary( 
            /* [in] */ __RPC__in_opt IVsSplitPane *pPane) = 0;
        
        virtual HRESULT STDMETHODCALLTYPE HandleMessage( 
            /* [in] */ UINT iMsg,
            /* [in] */ WPARAM wParam,
            /* [in] */ LPARAM lParam) = 0;
        
        virtual HRESULT STDMETHODCALLTYPE SplitPane( 
            /* [in] */ __RPC__in_opt IVsSplitPane *pNewPane,
            /* [in] */ long iIndex,
            /* [in] */ long iSize,
            /* [in] */ DWORD dwFlags,
            /* [out] */ __RPC__deref_out_opt IVsSplitter **ppSplitter) = 0;
        
    };
    
#else 	/* C style interface */

    typedef struct IVsSplitterVtbl
    {
        BEGIN_INTERFACE
        
        HRESULT ( STDMETHODCALLTYPE *QueryInterface )( 
            IVsSplitter * This,
            /* [in] */ __RPC__in REFIID riid,
            /* [iid_is][out] */ 
            __RPC__deref_out  void **ppvObject);
        
        ULONG ( STDMETHODCALLTYPE *AddRef )( 
            IVsSplitter * This);
        
        ULONG ( STDMETHODCALLTYPE *Release )( 
            IVsSplitter * This);
        
        HRESULT ( STDMETHODCALLTYPE *SetParentSplitter )( 
            IVsSplitter * This,
            /* [in] */ __RPC__in_opt IVsSplitter *pSplitter);
        
        HRESULT ( STDMETHODCALLTYPE *GetParentSplitter )( 
            IVsSplitter * This,
            /* [out] */ __RPC__deref_out_opt IVsSplitter **ppSplitter);
        
        HRESULT ( STDMETHODCALLTYPE *SetFocus )( 
            IVsSplitter * This);
        
        HRESULT ( STDMETHODCALLTYPE *SetPosition )( 
            IVsSplitter * This,
            /* [in] */ long iLeft,
            /* [in] */ long iTop,
            /* [in] */ long iRight,
            /* [in] */ long iBottom);
        
        HRESULT ( STDMETHODCALLTYPE *GetPosition )( 
            IVsSplitter * This,
            /* [out] */ __RPC__out long *piLeft,
            /* [out] */ __RPC__out long *piTop,
            /* [out] */ __RPC__out long *piRight,
            /* [out] */ __RPC__out long *piBottom);
        
        HRESULT ( STDMETHODCALLTYPE *GetSizeExtents )( 
            IVsSplitter * This,
            /* [out] */ __RPC__out long *piMinHorz,
            /* [out] */ __RPC__out long *piMaxHorz,
            /* [out] */ __RPC__out long *piMinVert,
            /* [out] */ __RPC__out long *piMaxVert);
        
        HRESULT ( STDMETHODCALLTYPE *GetParentWindow )( 
            IVsSplitter * This,
            /* [out] */ __RPC__deref_out_opt HWND *phwndParent);
        
        HRESULT ( STDMETHODCALLTYPE *GetPaneFlags )( 
            IVsSplitter * This,
            /* [out] */ __RPC__out DWORD *pdwPaneFlags);
        
        HRESULT ( STDMETHODCALLTYPE *ClosePane )( 
            IVsSplitter * This);
        
        HRESULT ( STDMETHODCALLTYPE *NotifyPane )( 
            IVsSplitter * This,
            /* [in] */ SPLITNOTIFYCODE iCode);
        
        HRESULT ( STDMETHODCALLTYPE *DetachPane )( 
            IVsSplitter * This);
        
        HRESULT ( STDMETHODCALLTYPE *GetPane )( 
            IVsSplitter * This,
            /* [in] */ long iIndex,
            /* [out] */ __RPC__deref_out_opt IVsSplitPane **ppPane);
        
        HRESULT ( STDMETHODCALLTYPE *NotifySplitter )( 
            IVsSplitter * This,
            /* [in] */ __RPC__in_opt IVsSplitPane *pPane,
            /* [in] */ PANEEVENTS iEvent);
        
        HRESULT ( STDMETHODCALLTYPE *IsPanePrimary )( 
            IVsSplitter * This,
            /* [in] */ __RPC__in_opt IVsSplitPane *pPane);
        
        HRESULT ( STDMETHODCALLTYPE *HandleMessage )( 
            IVsSplitter * This,
            /* [in] */ UINT iMsg,
            /* [in] */ WPARAM wParam,
            /* [in] */ LPARAM lParam);
        
        HRESULT ( STDMETHODCALLTYPE *SplitPane )( 
            IVsSplitter * This,
            /* [in] */ __RPC__in_opt IVsSplitPane *pNewPane,
            /* [in] */ long iIndex,
            /* [in] */ long iSize,
            /* [in] */ DWORD dwFlags,
            /* [out] */ __RPC__deref_out_opt IVsSplitter **ppSplitter);
        
        END_INTERFACE
    } IVsSplitterVtbl;

    interface IVsSplitter
    {
        CONST_VTBL struct IVsSplitterVtbl *lpVtbl;
    };

    

#ifdef COBJMACROS


#define IVsSplitter_QueryInterface(This,riid,ppvObject)	\
    ( (This)->lpVtbl -> QueryInterface(This,riid,ppvObject) ) 

#define IVsSplitter_AddRef(This)	\
    ( (This)->lpVtbl -> AddRef(This) ) 

#define IVsSplitter_Release(This)	\
    ( (This)->lpVtbl -> Release(This) ) 


#define IVsSplitter_SetParentSplitter(This,pSplitter)	\
    ( (This)->lpVtbl -> SetParentSplitter(This,pSplitter) ) 

#define IVsSplitter_GetParentSplitter(This,ppSplitter)	\
    ( (This)->lpVtbl -> GetParentSplitter(This,ppSplitter) ) 

#define IVsSplitter_SetFocus(This)	\
    ( (This)->lpVtbl -> SetFocus(This) ) 

#define IVsSplitter_SetPosition(This,iLeft,iTop,iRight,iBottom)	\
    ( (This)->lpVtbl -> SetPosition(This,iLeft,iTop,iRight,iBottom) ) 

#define IVsSplitter_GetPosition(This,piLeft,piTop,piRight,piBottom)	\
    ( (This)->lpVtbl -> GetPosition(This,piLeft,piTop,piRight,piBottom) ) 

#define IVsSplitter_GetSizeExtents(This,piMinHorz,piMaxHorz,piMinVert,piMaxVert)	\
    ( (This)->lpVtbl -> GetSizeExtents(This,piMinHorz,piMaxHorz,piMinVert,piMaxVert) ) 

#define IVsSplitter_GetParentWindow(This,phwndParent)	\
    ( (This)->lpVtbl -> GetParentWindow(This,phwndParent) ) 

#define IVsSplitter_GetPaneFlags(This,pdwPaneFlags)	\
    ( (This)->lpVtbl -> GetPaneFlags(This,pdwPaneFlags) ) 

#define IVsSplitter_ClosePane(This)	\
    ( (This)->lpVtbl -> ClosePane(This) ) 

#define IVsSplitter_NotifyPane(This,iCode)	\
    ( (This)->lpVtbl -> NotifyPane(This,iCode) ) 

#define IVsSplitter_DetachPane(This)	\
    ( (This)->lpVtbl -> DetachPane(This) ) 


#define IVsSplitter_GetPane(This,iIndex,ppPane)	\
    ( (This)->lpVtbl -> GetPane(This,iIndex,ppPane) ) 

#define IVsSplitter_NotifySplitter(This,pPane,iEvent)	\
    ( (This)->lpVtbl -> NotifySplitter(This,pPane,iEvent) ) 

#define IVsSplitter_IsPanePrimary(This,pPane)	\
    ( (This)->lpVtbl -> IsPanePrimary(This,pPane) ) 

#define IVsSplitter_HandleMessage(This,iMsg,wParam,lParam)	\
    ( (This)->lpVtbl -> HandleMessage(This,iMsg,wParam,lParam) ) 

#define IVsSplitter_SplitPane(This,pNewPane,iIndex,iSize,dwFlags,ppSplitter)	\
    ( (This)->lpVtbl -> SplitPane(This,pNewPane,iIndex,iSize,dwFlags,ppSplitter) ) 

#endif /* COBJMACROS */


#endif 	/* C style interface */




#endif 	/* __IVsSplitter_INTERFACE_DEFINED__ */


#ifndef __IVsSplitRoot_INTERFACE_DEFINED__
#define __IVsSplitRoot_INTERFACE_DEFINED__

/* interface IVsSplitRoot */
/* [object][uuid] */ 


EXTERN_C const IID IID_IVsSplitRoot;

#if defined(__cplusplus) && !defined(CINTERFACE)
    
    MIDL_INTERFACE("5486C948-AF82-4DA5-A968-97F3ADA66670")
    IVsSplitRoot : public IUnknown
    {
    public:
        virtual HRESULT STDMETHODCALLTYPE GetRootSplitter( 
            /* [out] */ __RPC__deref_out_opt IVsSplitter **ppPane) = 0;
        
        virtual HRESULT STDMETHODCALLTYPE GetWindowHandle( 
            /* [out] */ __RPC__deref_out_opt HWND *phwnd) = 0;
        
        virtual HRESULT STDMETHODCALLTYPE GetPane( 
            /* [in] */ PANETYPE paneType,
            /* [out] */ __RPC__deref_out_opt IVsSplitPane **ppPane) = 0;
        
    };
    
#else 	/* C style interface */

    typedef struct IVsSplitRootVtbl
    {
        BEGIN_INTERFACE
        
        HRESULT ( STDMETHODCALLTYPE *QueryInterface )( 
            IVsSplitRoot * This,
            /* [in] */ __RPC__in REFIID riid,
            /* [iid_is][out] */ 
            __RPC__deref_out  void **ppvObject);
        
        ULONG ( STDMETHODCALLTYPE *AddRef )( 
            IVsSplitRoot * This);
        
        ULONG ( STDMETHODCALLTYPE *Release )( 
            IVsSplitRoot * This);
        
        HRESULT ( STDMETHODCALLTYPE *GetRootSplitter )( 
            IVsSplitRoot * This,
            /* [out] */ __RPC__deref_out_opt IVsSplitter **ppPane);
        
        HRESULT ( STDMETHODCALLTYPE *GetWindowHandle )( 
            IVsSplitRoot * This,
            /* [out] */ __RPC__deref_out_opt HWND *phwnd);
        
        HRESULT ( STDMETHODCALLTYPE *GetPane )( 
            IVsSplitRoot * This,
            /* [in] */ PANETYPE paneType,
            /* [out] */ __RPC__deref_out_opt IVsSplitPane **ppPane);
        
        END_INTERFACE
    } IVsSplitRootVtbl;

    interface IVsSplitRoot
    {
        CONST_VTBL struct IVsSplitRootVtbl *lpVtbl;
    };

    

#ifdef COBJMACROS


#define IVsSplitRoot_QueryInterface(This,riid,ppvObject)	\
    ( (This)->lpVtbl -> QueryInterface(This,riid,ppvObject) ) 

#define IVsSplitRoot_AddRef(This)	\
    ( (This)->lpVtbl -> AddRef(This) ) 

#define IVsSplitRoot_Release(This)	\
    ( (This)->lpVtbl -> Release(This) ) 


#define IVsSplitRoot_GetRootSplitter(This,ppPane)	\
    ( (This)->lpVtbl -> GetRootSplitter(This,ppPane) ) 

#define IVsSplitRoot_GetWindowHandle(This,phwnd)	\
    ( (This)->lpVtbl -> GetWindowHandle(This,phwnd) ) 

#define IVsSplitRoot_GetPane(This,paneType,ppPane)	\
    ( (This)->lpVtbl -> GetPane(This,paneType,ppPane) ) 

#endif /* COBJMACROS */


#endif 	/* C style interface */




#endif 	/* __IVsSplitRoot_INTERFACE_DEFINED__ */


#ifndef __IVsCodeWindow_INTERFACE_DEFINED__
#define __IVsCodeWindow_INTERFACE_DEFINED__

/* interface IVsCodeWindow */
/* [object][uuid] */ 


EXTERN_C const IID IID_IVsCodeWindow;

#if defined(__cplusplus) && !defined(CINTERFACE)
    
    MIDL_INTERFACE("8560CECD-DFAC-4F7B-9D2A-E6D9810F3443")
    IVsCodeWindow : public IUnknown
    {
    public:
        virtual HRESULT STDMETHODCALLTYPE SetBuffer( 
            /* [in] */ __RPC__in_opt IVsTextLines *pBuffer) = 0;
        
        virtual HRESULT STDMETHODCALLTYPE GetBuffer( 
            /* [out] */ __RPC__deref_out_opt IVsTextLines **ppBuffer) = 0;
        
        virtual HRESULT STDMETHODCALLTYPE GetPrimaryView( 
            /* [out] */ __RPC__deref_out_opt IVsTextView **ppView) = 0;
        
        virtual HRESULT STDMETHODCALLTYPE GetSecondaryView( 
            /* [out] */ __RPC__deref_out_opt IVsTextView **ppView) = 0;
        
        virtual HRESULT STDMETHODCALLTYPE SetViewClassID( 
            /* [in] */ __RPC__in REFCLSID clsidView) = 0;
        
        virtual HRESULT STDMETHODCALLTYPE GetViewClassID( 
            /* [out] */ __RPC__out CLSID *pclsidView) = 0;
        
        virtual HRESULT STDMETHODCALLTYPE SetBaseEditorCaption( 
            /* [in] */ __RPC__deref_in_opt LPCOLESTR *pszBaseEditorCaption) = 0;
        
        virtual HRESULT STDMETHODCALLTYPE GetEditorCaption( 
            /* [in] */ READONLYSTATUS dwReadOnly,
            /* [out] */ __RPC__deref_out_opt BSTR *pbstrEditorCaption) = 0;
        
        virtual HRESULT STDMETHODCALLTYPE Close( void) = 0;
        
        virtual HRESULT STDMETHODCALLTYPE GetLastActiveView( 
            /* [out] */ __RPC__deref_out_opt IVsTextView **ppView) = 0;
        
    };
    
#else 	/* C style interface */

    typedef struct IVsCodeWindowVtbl
    {
        BEGIN_INTERFACE
        
        HRESULT ( STDMETHODCALLTYPE *QueryInterface )( 
            IVsCodeWindow * This,
            /* [in] */ __RPC__in REFIID riid,
            /* [iid_is][out] */ 
            __RPC__deref_out  void **ppvObject);
        
        ULONG ( STDMETHODCALLTYPE *AddRef )( 
            IVsCodeWindow * This);
        
        ULONG ( STDMETHODCALLTYPE *Release )( 
            IVsCodeWindow * This);
        
        HRESULT ( STDMETHODCALLTYPE *SetBuffer )( 
            IVsCodeWindow * This,
            /* [in] */ __RPC__in_opt IVsTextLines *pBuffer);
        
        HRESULT ( STDMETHODCALLTYPE *GetBuffer )( 
            IVsCodeWindow * This,
            /* [out] */ __RPC__deref_out_opt IVsTextLines **ppBuffer);
        
        HRESULT ( STDMETHODCALLTYPE *GetPrimaryView )( 
            IVsCodeWindow * This,
            /* [out] */ __RPC__deref_out_opt IVsTextView **ppView);
        
        HRESULT ( STDMETHODCALLTYPE *GetSecondaryView )( 
            IVsCodeWindow * This,
            /* [out] */ __RPC__deref_out_opt IVsTextView **ppView);
        
        HRESULT ( STDMETHODCALLTYPE *SetViewClassID )( 
            IVsCodeWindow * This,
            /* [in] */ __RPC__in REFCLSID clsidView);
        
        HRESULT ( STDMETHODCALLTYPE *GetViewClassID )( 
            IVsCodeWindow * This,
            /* [out] */ __RPC__out CLSID *pclsidView);
        
        HRESULT ( STDMETHODCALLTYPE *SetBaseEditorCaption )( 
            IVsCodeWindow * This,
            /* [in] */ __RPC__deref_in_opt LPCOLESTR *pszBaseEditorCaption);
        
        HRESULT ( STDMETHODCALLTYPE *GetEditorCaption )( 
            IVsCodeWindow * This,
            /* [in] */ READONLYSTATUS dwReadOnly,
            /* [out] */ __RPC__deref_out_opt BSTR *pbstrEditorCaption);
        
        HRESULT ( STDMETHODCALLTYPE *Close )( 
            IVsCodeWindow * This);
        
        HRESULT ( STDMETHODCALLTYPE *GetLastActiveView )( 
            IVsCodeWindow * This,
            /* [out] */ __RPC__deref_out_opt IVsTextView **ppView);
        
        END_INTERFACE
    } IVsCodeWindowVtbl;

    interface IVsCodeWindow
    {
        CONST_VTBL struct IVsCodeWindowVtbl *lpVtbl;
    };

    

#ifdef COBJMACROS


#define IVsCodeWindow_QueryInterface(This,riid,ppvObject)	\
    ( (This)->lpVtbl -> QueryInterface(This,riid,ppvObject) ) 

#define IVsCodeWindow_AddRef(This)	\
    ( (This)->lpVtbl -> AddRef(This) ) 

#define IVsCodeWindow_Release(This)	\
    ( (This)->lpVtbl -> Release(This) ) 


#define IVsCodeWindow_SetBuffer(This,pBuffer)	\
    ( (This)->lpVtbl -> SetBuffer(This,pBuffer) ) 

#define IVsCodeWindow_GetBuffer(This,ppBuffer)	\
    ( (This)->lpVtbl -> GetBuffer(This,ppBuffer) ) 

#define IVsCodeWindow_GetPrimaryView(This,ppView)	\
    ( (This)->lpVtbl -> GetPrimaryView(This,ppView) ) 

#define IVsCodeWindow_GetSecondaryView(This,ppView)	\
    ( (This)->lpVtbl -> GetSecondaryView(This,ppView) ) 

#define IVsCodeWindow_SetViewClassID(This,clsidView)	\
    ( (This)->lpVtbl -> SetViewClassID(This,clsidView) ) 

#define IVsCodeWindow_GetViewClassID(This,pclsidView)	\
    ( (This)->lpVtbl -> GetViewClassID(This,pclsidView) ) 

#define IVsCodeWindow_SetBaseEditorCaption(This,pszBaseEditorCaption)	\
    ( (This)->lpVtbl -> SetBaseEditorCaption(This,pszBaseEditorCaption) ) 

#define IVsCodeWindow_GetEditorCaption(This,dwReadOnly,pbstrEditorCaption)	\
    ( (This)->lpVtbl -> GetEditorCaption(This,dwReadOnly,pbstrEditorCaption) ) 

#define IVsCodeWindow_Close(This)	\
    ( (This)->lpVtbl -> Close(This) ) 

#define IVsCodeWindow_GetLastActiveView(This,ppView)	\
    ( (This)->lpVtbl -> GetLastActiveView(This,ppView) ) 

#endif /* COBJMACROS */


#endif 	/* C style interface */




#endif 	/* __IVsCodeWindow_INTERFACE_DEFINED__ */


#ifndef __IVsCodeWindowManager_INTERFACE_DEFINED__
#define __IVsCodeWindowManager_INTERFACE_DEFINED__

/* interface IVsCodeWindowManager */
/* [object][uuid] */ 


EXTERN_C const IID IID_IVsCodeWindowManager;

#if defined(__cplusplus) && !defined(CINTERFACE)
    
    MIDL_INTERFACE("4758CB90-6110-4440-B577-2E3FC91E4B84")
    IVsCodeWindowManager : public IUnknown
    {
    public:
        virtual HRESULT STDMETHODCALLTYPE AddAdornments( void) = 0;
        
        virtual HRESULT STDMETHODCALLTYPE RemoveAdornments( void) = 0;
        
        virtual HRESULT STDMETHODCALLTYPE OnNewView( 
            /* [in] */ __RPC__in_opt IVsTextView *pView) = 0;
        
    };
    
#else 	/* C style interface */

    typedef struct IVsCodeWindowManagerVtbl
    {
        BEGIN_INTERFACE
        
        HRESULT ( STDMETHODCALLTYPE *QueryInterface )( 
            IVsCodeWindowManager * This,
            /* [in] */ __RPC__in REFIID riid,
            /* [iid_is][out] */ 
            __RPC__deref_out  void **ppvObject);
        
        ULONG ( STDMETHODCALLTYPE *AddRef )( 
            IVsCodeWindowManager * This);
        
        ULONG ( STDMETHODCALLTYPE *Release )( 
            IVsCodeWindowManager * This);
        
        HRESULT ( STDMETHODCALLTYPE *AddAdornments )( 
            IVsCodeWindowManager * This);
        
        HRESULT ( STDMETHODCALLTYPE *RemoveAdornments )( 
            IVsCodeWindowManager * This);
        
        HRESULT ( STDMETHODCALLTYPE *OnNewView )( 
            IVsCodeWindowManager * This,
            /* [in] */ __RPC__in_opt IVsTextView *pView);
        
        END_INTERFACE
    } IVsCodeWindowManagerVtbl;

    interface IVsCodeWindowManager
    {
        CONST_VTBL struct IVsCodeWindowManagerVtbl *lpVtbl;
    };

    

#ifdef COBJMACROS


#define IVsCodeWindowManager_QueryInterface(This,riid,ppvObject)	\
    ( (This)->lpVtbl -> QueryInterface(This,riid,ppvObject) ) 

#define IVsCodeWindowManager_AddRef(This)	\
    ( (This)->lpVtbl -> AddRef(This) ) 

#define IVsCodeWindowManager_Release(This)	\
    ( (This)->lpVtbl -> Release(This) ) 


#define IVsCodeWindowManager_AddAdornments(This)	\
    ( (This)->lpVtbl -> AddAdornments(This) ) 

#define IVsCodeWindowManager_RemoveAdornments(This)	\
    ( (This)->lpVtbl -> RemoveAdornments(This) ) 

#define IVsCodeWindowManager_OnNewView(This,pView)	\
    ( (This)->lpVtbl -> OnNewView(This,pView) ) 

#endif /* COBJMACROS */


#endif 	/* C style interface */




#endif 	/* __IVsCodeWindowManager_INTERFACE_DEFINED__ */


#ifndef __IVsDropdownBarClient_INTERFACE_DEFINED__
#define __IVsDropdownBarClient_INTERFACE_DEFINED__

/* interface IVsDropdownBarClient */
/* [object][uuid] */ 


EXTERN_C const IID IID_IVsDropdownBarClient;

#if defined(__cplusplus) && !defined(CINTERFACE)
    
    MIDL_INTERFACE("AEE77BDD-05C8-4DE1-B48E-1B003F49ACB7")
    IVsDropdownBarClient : public IUnknown
    {
    public:
        virtual HRESULT STDMETHODCALLTYPE SetDropdownBar( 
            /* [in] */ __RPC__in_opt IVsDropdownBar *pDropdownBar) = 0;
        
        virtual HRESULT STDMETHODCALLTYPE GetComboAttributes( 
            /* [in] */ long iCombo,
            /* [out] */ __RPC__out ULONG *pcEntries,
            /* [out] */ __RPC__out ULONG *puEntryType,
            /* [out] */ __RPC__deref_out_opt HANDLE *phImageList) = 0;
        
        virtual HRESULT STDMETHODCALLTYPE GetEntryText( 
            /* [in] */ long iCombo,
            /* [in] */ long iIndex,
            /* [out] */ __RPC__deref_out_opt WCHAR **ppszText) = 0;
        
        virtual HRESULT STDMETHODCALLTYPE GetEntryAttributes( 
            /* [in] */ long iCombo,
            /* [in] */ long iIndex,
            /* [out] */ __RPC__out ULONG *pAttr) = 0;
        
        virtual HRESULT STDMETHODCALLTYPE GetEntryImage( 
            /* [in] */ long iCombo,
            /* [in] */ long iIndex,
            /* [out] */ __RPC__out long *piImageIndex) = 0;
        
        virtual HRESULT STDMETHODCALLTYPE OnItemSelected( 
            /* [in] */ long iCombo,
            /* [in] */ long iIndex) = 0;
        
        virtual HRESULT STDMETHODCALLTYPE OnItemChosen( 
            /* [in] */ long iCombo,
            /* [in] */ long iIndex) = 0;
        
        virtual HRESULT STDMETHODCALLTYPE OnComboGetFocus( 
            /* [in] */ long iCombo) = 0;
        
        virtual HRESULT STDMETHODCALLTYPE GetComboTipText( 
            /* [in] */ long iCombo,
            /* [out] */ __RPC__deref_out_opt BSTR *pbstrText) = 0;
        
    };
    
#else 	/* C style interface */

    typedef struct IVsDropdownBarClientVtbl
    {
        BEGIN_INTERFACE
        
        HRESULT ( STDMETHODCALLTYPE *QueryInterface )( 
            IVsDropdownBarClient * This,
            /* [in] */ __RPC__in REFIID riid,
            /* [iid_is][out] */ 
            __RPC__deref_out  void **ppvObject);
        
        ULONG ( STDMETHODCALLTYPE *AddRef )( 
            IVsDropdownBarClient * This);
        
        ULONG ( STDMETHODCALLTYPE *Release )( 
            IVsDropdownBarClient * This);
        
        HRESULT ( STDMETHODCALLTYPE *SetDropdownBar )( 
            IVsDropdownBarClient * This,
            /* [in] */ __RPC__in_opt IVsDropdownBar *pDropdownBar);
        
        HRESULT ( STDMETHODCALLTYPE *GetComboAttributes )( 
            IVsDropdownBarClient * This,
            /* [in] */ long iCombo,
            /* [out] */ __RPC__out ULONG *pcEntries,
            /* [out] */ __RPC__out ULONG *puEntryType,
            /* [out] */ __RPC__deref_out_opt HANDLE *phImageList);
        
        HRESULT ( STDMETHODCALLTYPE *GetEntryText )( 
            IVsDropdownBarClient * This,
            /* [in] */ long iCombo,
            /* [in] */ long iIndex,
            /* [out] */ __RPC__deref_out_opt WCHAR **ppszText);
        
        HRESULT ( STDMETHODCALLTYPE *GetEntryAttributes )( 
            IVsDropdownBarClient * This,
            /* [in] */ long iCombo,
            /* [in] */ long iIndex,
            /* [out] */ __RPC__out ULONG *pAttr);
        
        HRESULT ( STDMETHODCALLTYPE *GetEntryImage )( 
            IVsDropdownBarClient * This,
            /* [in] */ long iCombo,
            /* [in] */ long iIndex,
            /* [out] */ __RPC__out long *piImageIndex);
        
        HRESULT ( STDMETHODCALLTYPE *OnItemSelected )( 
            IVsDropdownBarClient * This,
            /* [in] */ long iCombo,
            /* [in] */ long iIndex);
        
        HRESULT ( STDMETHODCALLTYPE *OnItemChosen )( 
            IVsDropdownBarClient * This,
            /* [in] */ long iCombo,
            /* [in] */ long iIndex);
        
        HRESULT ( STDMETHODCALLTYPE *OnComboGetFocus )( 
            IVsDropdownBarClient * This,
            /* [in] */ long iCombo);
        
        HRESULT ( STDMETHODCALLTYPE *GetComboTipText )( 
            IVsDropdownBarClient * This,
            /* [in] */ long iCombo,
            /* [out] */ __RPC__deref_out_opt BSTR *pbstrText);
        
        END_INTERFACE
    } IVsDropdownBarClientVtbl;

    interface IVsDropdownBarClient
    {
        CONST_VTBL struct IVsDropdownBarClientVtbl *lpVtbl;
    };

    

#ifdef COBJMACROS


#define IVsDropdownBarClient_QueryInterface(This,riid,ppvObject)	\
    ( (This)->lpVtbl -> QueryInterface(This,riid,ppvObject) ) 

#define IVsDropdownBarClient_AddRef(This)	\
    ( (This)->lpVtbl -> AddRef(This) ) 

#define IVsDropdownBarClient_Release(This)	\
    ( (This)->lpVtbl -> Release(This) ) 


#define IVsDropdownBarClient_SetDropdownBar(This,pDropdownBar)	\
    ( (This)->lpVtbl -> SetDropdownBar(This,pDropdownBar) ) 

#define IVsDropdownBarClient_GetComboAttributes(This,iCombo,pcEntries,puEntryType,phImageList)	\
    ( (This)->lpVtbl -> GetComboAttributes(This,iCombo,pcEntries,puEntryType,phImageList) ) 

#define IVsDropdownBarClient_GetEntryText(This,iCombo,iIndex,ppszText)	\
    ( (This)->lpVtbl -> GetEntryText(This,iCombo,iIndex,ppszText) ) 

#define IVsDropdownBarClient_GetEntryAttributes(This,iCombo,iIndex,pAttr)	\
    ( (This)->lpVtbl -> GetEntryAttributes(This,iCombo,iIndex,pAttr) ) 

#define IVsDropdownBarClient_GetEntryImage(This,iCombo,iIndex,piImageIndex)	\
    ( (This)->lpVtbl -> GetEntryImage(This,iCombo,iIndex,piImageIndex) ) 

#define IVsDropdownBarClient_OnItemSelected(This,iCombo,iIndex)	\
    ( (This)->lpVtbl -> OnItemSelected(This,iCombo,iIndex) ) 

#define IVsDropdownBarClient_OnItemChosen(This,iCombo,iIndex)	\
    ( (This)->lpVtbl -> OnItemChosen(This,iCombo,iIndex) ) 

#define IVsDropdownBarClient_OnComboGetFocus(This,iCombo)	\
    ( (This)->lpVtbl -> OnComboGetFocus(This,iCombo) ) 

#define IVsDropdownBarClient_GetComboTipText(This,iCombo,pbstrText)	\
    ( (This)->lpVtbl -> GetComboTipText(This,iCombo,pbstrText) ) 

#endif /* COBJMACROS */


#endif 	/* C style interface */




#endif 	/* __IVsDropdownBarClient_INTERFACE_DEFINED__ */


#ifndef __IVsDropdownBar_INTERFACE_DEFINED__
#define __IVsDropdownBar_INTERFACE_DEFINED__

/* interface IVsDropdownBar */
/* [object][uuid] */ 


EXTERN_C const IID IID_IVsDropdownBar;

#if defined(__cplusplus) && !defined(CINTERFACE)
    
    MIDL_INTERFACE("D7820968-4CF8-4760-B633-570282E5F66D")
    IVsDropdownBar : public IUnknown
    {
    public:
        virtual HRESULT STDMETHODCALLTYPE Attach( 
            /* [in] */ __RPC__in HWND hwndParent,
            /* [in] */ long cCombos,
            /* [in] */ __RPC__in_opt IVsDropdownBarClient *pClient) = 0;
        
        virtual void STDMETHODCALLTYPE Detach( void) = 0;
        
        virtual HRESULT STDMETHODCALLTYPE GetCurrentSelection( 
            /* [in] */ long iCombo,
            /* [out] */ __RPC__out long *piCurSel) = 0;
        
        virtual HRESULT STDMETHODCALLTYPE RefreshCombo( 
            /* [in] */ long iCombo,
            /* [in] */ long iNewSelection) = 0;
        
        virtual HRESULT STDMETHODCALLTYPE GetClient( 
            /* [out] */ __RPC__deref_out_opt IVsDropdownBarClient **ppClient) = 0;
        
        virtual HRESULT STDMETHODCALLTYPE SetCurrentSelection( 
            /* [in] */ long iCombo,
            /* [in] */ long iNewSelection) = 0;
        
    };
    
#else 	/* C style interface */

    typedef struct IVsDropdownBarVtbl
    {
        BEGIN_INTERFACE
        
        HRESULT ( STDMETHODCALLTYPE *QueryInterface )( 
            IVsDropdownBar * This,
            /* [in] */ __RPC__in REFIID riid,
            /* [iid_is][out] */ 
            __RPC__deref_out  void **ppvObject);
        
        ULONG ( STDMETHODCALLTYPE *AddRef )( 
            IVsDropdownBar * This);
        
        ULONG ( STDMETHODCALLTYPE *Release )( 
            IVsDropdownBar * This);
        
        HRESULT ( STDMETHODCALLTYPE *Attach )( 
            IVsDropdownBar * This,
            /* [in] */ __RPC__in HWND hwndParent,
            /* [in] */ long cCombos,
            /* [in] */ __RPC__in_opt IVsDropdownBarClient *pClient);
        
        void ( STDMETHODCALLTYPE *Detach )( 
            IVsDropdownBar * This);
        
        HRESULT ( STDMETHODCALLTYPE *GetCurrentSelection )( 
            IVsDropdownBar * This,
            /* [in] */ long iCombo,
            /* [out] */ __RPC__out long *piCurSel);
        
        HRESULT ( STDMETHODCALLTYPE *RefreshCombo )( 
            IVsDropdownBar * This,
            /* [in] */ long iCombo,
            /* [in] */ long iNewSelection);
        
        HRESULT ( STDMETHODCALLTYPE *GetClient )( 
            IVsDropdownBar * This,
            /* [out] */ __RPC__deref_out_opt IVsDropdownBarClient **ppClient);
        
        HRESULT ( STDMETHODCALLTYPE *SetCurrentSelection )( 
            IVsDropdownBar * This,
            /* [in] */ long iCombo,
            /* [in] */ long iNewSelection);
        
        END_INTERFACE
    } IVsDropdownBarVtbl;

    interface IVsDropdownBar
    {
        CONST_VTBL struct IVsDropdownBarVtbl *lpVtbl;
    };

    

#ifdef COBJMACROS


#define IVsDropdownBar_QueryInterface(This,riid,ppvObject)	\
    ( (This)->lpVtbl -> QueryInterface(This,riid,ppvObject) ) 

#define IVsDropdownBar_AddRef(This)	\
    ( (This)->lpVtbl -> AddRef(This) ) 

#define IVsDropdownBar_Release(This)	\
    ( (This)->lpVtbl -> Release(This) ) 


#define IVsDropdownBar_Attach(This,hwndParent,cCombos,pClient)	\
    ( (This)->lpVtbl -> Attach(This,hwndParent,cCombos,pClient) ) 

#define IVsDropdownBar_Detach(This)	\
    ( (This)->lpVtbl -> Detach(This) ) 

#define IVsDropdownBar_GetCurrentSelection(This,iCombo,piCurSel)	\
    ( (This)->lpVtbl -> GetCurrentSelection(This,iCombo,piCurSel) ) 

#define IVsDropdownBar_RefreshCombo(This,iCombo,iNewSelection)	\
    ( (This)->lpVtbl -> RefreshCombo(This,iCombo,iNewSelection) ) 

#define IVsDropdownBar_GetClient(This,ppClient)	\
    ( (This)->lpVtbl -> GetClient(This,ppClient) ) 

#define IVsDropdownBar_SetCurrentSelection(This,iCombo,iNewSelection)	\
    ( (This)->lpVtbl -> SetCurrentSelection(This,iCombo,iNewSelection) ) 

#endif /* COBJMACROS */


#endif 	/* C style interface */




#endif 	/* __IVsDropdownBar_INTERFACE_DEFINED__ */


#ifndef __IVsDropdownBarManager_INTERFACE_DEFINED__
#define __IVsDropdownBarManager_INTERFACE_DEFINED__

/* interface IVsDropdownBarManager */
/* [object][uuid] */ 


EXTERN_C const IID IID_IVsDropdownBarManager;

#if defined(__cplusplus) && !defined(CINTERFACE)
    
    MIDL_INTERFACE("5CCB1170-6527-4D88-BE3E-198F0EF5538D")
    IVsDropdownBarManager : public IUnknown
    {
    public:
        virtual HRESULT STDMETHODCALLTYPE AddDropdownBar( 
            /* [in] */ long cCombos,
            /* [in] */ __RPC__in_opt IVsDropdownBarClient *pClient) = 0;
        
        virtual HRESULT STDMETHODCALLTYPE GetDropdownBar( 
            /* [out] */ __RPC__deref_out_opt IVsDropdownBar **ppDropdownBar) = 0;
        
        virtual HRESULT STDMETHODCALLTYPE RemoveDropdownBar( void) = 0;
        
    };
    
#else 	/* C style interface */

    typedef struct IVsDropdownBarManagerVtbl
    {
        BEGIN_INTERFACE
        
        HRESULT ( STDMETHODCALLTYPE *QueryInterface )( 
            IVsDropdownBarManager * This,
            /* [in] */ __RPC__in REFIID riid,
            /* [iid_is][out] */ 
            __RPC__deref_out  void **ppvObject);
        
        ULONG ( STDMETHODCALLTYPE *AddRef )( 
            IVsDropdownBarManager * This);
        
        ULONG ( STDMETHODCALLTYPE *Release )( 
            IVsDropdownBarManager * This);
        
        HRESULT ( STDMETHODCALLTYPE *AddDropdownBar )( 
            IVsDropdownBarManager * This,
            /* [in] */ long cCombos,
            /* [in] */ __RPC__in_opt IVsDropdownBarClient *pClient);
        
        HRESULT ( STDMETHODCALLTYPE *GetDropdownBar )( 
            IVsDropdownBarManager * This,
            /* [out] */ __RPC__deref_out_opt IVsDropdownBar **ppDropdownBar);
        
        HRESULT ( STDMETHODCALLTYPE *RemoveDropdownBar )( 
            IVsDropdownBarManager * This);
        
        END_INTERFACE
    } IVsDropdownBarManagerVtbl;

    interface IVsDropdownBarManager
    {
        CONST_VTBL struct IVsDropdownBarManagerVtbl *lpVtbl;
    };

    

#ifdef COBJMACROS


#define IVsDropdownBarManager_QueryInterface(This,riid,ppvObject)	\
    ( (This)->lpVtbl -> QueryInterface(This,riid,ppvObject) ) 

#define IVsDropdownBarManager_AddRef(This)	\
    ( (This)->lpVtbl -> AddRef(This) ) 

#define IVsDropdownBarManager_Release(This)	\
    ( (This)->lpVtbl -> Release(This) ) 


#define IVsDropdownBarManager_AddDropdownBar(This,cCombos,pClient)	\
    ( (This)->lpVtbl -> AddDropdownBar(This,cCombos,pClient) ) 

#define IVsDropdownBarManager_GetDropdownBar(This,ppDropdownBar)	\
    ( (This)->lpVtbl -> GetDropdownBar(This,ppDropdownBar) ) 

#define IVsDropdownBarManager_RemoveDropdownBar(This)	\
    ( (This)->lpVtbl -> RemoveDropdownBar(This) ) 

#endif /* COBJMACROS */


#endif 	/* C style interface */




#endif 	/* __IVsDropdownBarManager_INTERFACE_DEFINED__ */


#ifndef __IVsButtonBarClient_INTERFACE_DEFINED__
#define __IVsButtonBarClient_INTERFACE_DEFINED__

/* interface IVsButtonBarClient */
/* [object][uuid] */ 


EXTERN_C const IID IID_IVsButtonBarClient;

#if defined(__cplusplus) && !defined(CINTERFACE)
    
    MIDL_INTERFACE("B59AF843-E075-496E-A00D-E8EEB3C748EB")
    IVsButtonBarClient : public IUnknown
    {
    public:
        virtual HRESULT STDMETHODCALLTYPE SetButtonBar( 
            /* [in] */ __RPC__in_opt IVsButtonBar *pButtonBar) = 0;
        
        virtual HRESULT STDMETHODCALLTYPE GetButtonTipText( 
            /* [in] */ long iButton,
            /* [out] */ __RPC__deref_out_opt BSTR *pbstrTip) = 0;
        
        virtual HRESULT STDMETHODCALLTYPE OnButtonPressed( 
            /* [in] */ long iButton) = 0;
        
    };
    
#else 	/* C style interface */

    typedef struct IVsButtonBarClientVtbl
    {
        BEGIN_INTERFACE
        
        HRESULT ( STDMETHODCALLTYPE *QueryInterface )( 
            IVsButtonBarClient * This,
            /* [in] */ __RPC__in REFIID riid,
            /* [iid_is][out] */ 
            __RPC__deref_out  void **ppvObject);
        
        ULONG ( STDMETHODCALLTYPE *AddRef )( 
            IVsButtonBarClient * This);
        
        ULONG ( STDMETHODCALLTYPE *Release )( 
            IVsButtonBarClient * This);
        
        HRESULT ( STDMETHODCALLTYPE *SetButtonBar )( 
            IVsButtonBarClient * This,
            /* [in] */ __RPC__in_opt IVsButtonBar *pButtonBar);
        
        HRESULT ( STDMETHODCALLTYPE *GetButtonTipText )( 
            IVsButtonBarClient * This,
            /* [in] */ long iButton,
            /* [out] */ __RPC__deref_out_opt BSTR *pbstrTip);
        
        HRESULT ( STDMETHODCALLTYPE *OnButtonPressed )( 
            IVsButtonBarClient * This,
            /* [in] */ long iButton);
        
        END_INTERFACE
    } IVsButtonBarClientVtbl;

    interface IVsButtonBarClient
    {
        CONST_VTBL struct IVsButtonBarClientVtbl *lpVtbl;
    };

    

#ifdef COBJMACROS


#define IVsButtonBarClient_QueryInterface(This,riid,ppvObject)	\
    ( (This)->lpVtbl -> QueryInterface(This,riid,ppvObject) ) 

#define IVsButtonBarClient_AddRef(This)	\
    ( (This)->lpVtbl -> AddRef(This) ) 

#define IVsButtonBarClient_Release(This)	\
    ( (This)->lpVtbl -> Release(This) ) 


#define IVsButtonBarClient_SetButtonBar(This,pButtonBar)	\
    ( (This)->lpVtbl -> SetButtonBar(This,pButtonBar) ) 

#define IVsButtonBarClient_GetButtonTipText(This,iButton,pbstrTip)	\
    ( (This)->lpVtbl -> GetButtonTipText(This,iButton,pbstrTip) ) 

#define IVsButtonBarClient_OnButtonPressed(This,iButton)	\
    ( (This)->lpVtbl -> OnButtonPressed(This,iButton) ) 

#endif /* COBJMACROS */


#endif 	/* C style interface */




#endif 	/* __IVsButtonBarClient_INTERFACE_DEFINED__ */


#ifndef __IVsButtonBar_INTERFACE_DEFINED__
#define __IVsButtonBar_INTERFACE_DEFINED__

/* interface IVsButtonBar */
/* [object][uuid] */ 


EXTERN_C const IID IID_IVsButtonBar;

#if defined(__cplusplus) && !defined(CINTERFACE)
    
    MIDL_INTERFACE("4C4095EC-0AF1-48B2-A900-DB496765EE9E")
    IVsButtonBar : public IUnknown
    {
    public:
        virtual HRESULT STDMETHODCALLTYPE Attach( 
            /* [in] */ __RPC__in HWND hwndParent,
            /* [in] */ long cButtons,
            /* [in] */ __RPC__in HANDLE hImageList,
            /* [in] */ __RPC__in_opt IVsButtonBarClient *pClient) = 0;
        
        virtual void STDMETHODCALLTYPE Detach( void) = 0;
        
        virtual HRESULT STDMETHODCALLTYPE SetHeight( 
            /* [in] */ long iHeight) = 0;
        
        virtual HRESULT STDMETHODCALLTYPE GetSize( 
            /* [out] */ __RPC__out long *piWidth,
            __RPC__in long *piHeight) = 0;
        
        virtual HRESULT STDMETHODCALLTYPE SetCurrentSelection( 
            /* [in] */ long iCurButton) = 0;
        
        virtual HRESULT STDMETHODCALLTYPE GetCurrentSelection( 
            /* [out] */ __RPC__out long *piCurButton) = 0;
        
        virtual HRESULT STDMETHODCALLTYPE GetClient( 
            /* [out] */ __RPC__deref_out_opt IVsButtonBarClient **ppClient) = 0;
        
    };
    
#else 	/* C style interface */

    typedef struct IVsButtonBarVtbl
    {
        BEGIN_INTERFACE
        
        HRESULT ( STDMETHODCALLTYPE *QueryInterface )( 
            IVsButtonBar * This,
            /* [in] */ __RPC__in REFIID riid,
            /* [iid_is][out] */ 
            __RPC__deref_out  void **ppvObject);
        
        ULONG ( STDMETHODCALLTYPE *AddRef )( 
            IVsButtonBar * This);
        
        ULONG ( STDMETHODCALLTYPE *Release )( 
            IVsButtonBar * This);
        
        HRESULT ( STDMETHODCALLTYPE *Attach )( 
            IVsButtonBar * This,
            /* [in] */ __RPC__in HWND hwndParent,
            /* [in] */ long cButtons,
            /* [in] */ __RPC__in HANDLE hImageList,
            /* [in] */ __RPC__in_opt IVsButtonBarClient *pClient);
        
        void ( STDMETHODCALLTYPE *Detach )( 
            IVsButtonBar * This);
        
        HRESULT ( STDMETHODCALLTYPE *SetHeight )( 
            IVsButtonBar * This,
            /* [in] */ long iHeight);
        
        HRESULT ( STDMETHODCALLTYPE *GetSize )( 
            IVsButtonBar * This,
            /* [out] */ __RPC__out long *piWidth,
            __RPC__in long *piHeight);
        
        HRESULT ( STDMETHODCALLTYPE *SetCurrentSelection )( 
            IVsButtonBar * This,
            /* [in] */ long iCurButton);
        
        HRESULT ( STDMETHODCALLTYPE *GetCurrentSelection )( 
            IVsButtonBar * This,
            /* [out] */ __RPC__out long *piCurButton);
        
        HRESULT ( STDMETHODCALLTYPE *GetClient )( 
            IVsButtonBar * This,
            /* [out] */ __RPC__deref_out_opt IVsButtonBarClient **ppClient);
        
        END_INTERFACE
    } IVsButtonBarVtbl;

    interface IVsButtonBar
    {
        CONST_VTBL struct IVsButtonBarVtbl *lpVtbl;
    };

    

#ifdef COBJMACROS


#define IVsButtonBar_QueryInterface(This,riid,ppvObject)	\
    ( (This)->lpVtbl -> QueryInterface(This,riid,ppvObject) ) 

#define IVsButtonBar_AddRef(This)	\
    ( (This)->lpVtbl -> AddRef(This) ) 

#define IVsButtonBar_Release(This)	\
    ( (This)->lpVtbl -> Release(This) ) 


#define IVsButtonBar_Attach(This,hwndParent,cButtons,hImageList,pClient)	\
    ( (This)->lpVtbl -> Attach(This,hwndParent,cButtons,hImageList,pClient) ) 

#define IVsButtonBar_Detach(This)	\
    ( (This)->lpVtbl -> Detach(This) ) 

#define IVsButtonBar_SetHeight(This,iHeight)	\
    ( (This)->lpVtbl -> SetHeight(This,iHeight) ) 

#define IVsButtonBar_GetSize(This,piWidth,piHeight)	\
    ( (This)->lpVtbl -> GetSize(This,piWidth,piHeight) ) 

#define IVsButtonBar_SetCurrentSelection(This,iCurButton)	\
    ( (This)->lpVtbl -> SetCurrentSelection(This,iCurButton) ) 

#define IVsButtonBar_GetCurrentSelection(This,piCurButton)	\
    ( (This)->lpVtbl -> GetCurrentSelection(This,piCurButton) ) 

#define IVsButtonBar_GetClient(This,ppClient)	\
    ( (This)->lpVtbl -> GetClient(This,ppClient) ) 

#endif /* COBJMACROS */


#endif 	/* C style interface */




#endif 	/* __IVsButtonBar_INTERFACE_DEFINED__ */


#ifndef __IVsButtonBarManager_INTERFACE_DEFINED__
#define __IVsButtonBarManager_INTERFACE_DEFINED__

/* interface IVsButtonBarManager */
/* [object][uuid] */ 


EXTERN_C const IID IID_IVsButtonBarManager;

#if defined(__cplusplus) && !defined(CINTERFACE)
    
    MIDL_INTERFACE("B4D5B0A3-1945-4F5F-B5E1-4ADD352E78C1")
    IVsButtonBarManager : public IUnknown
    {
    public:
        virtual HRESULT STDMETHODCALLTYPE AddButtonBar( 
            /* [in] */ long cButtons,
            /* [in] */ __RPC__in HANDLE hImageList,
            /* [in] */ __RPC__in_opt IVsButtonBarClient *pClient) = 0;
        
        virtual HRESULT STDMETHODCALLTYPE GetButtonBar( 
            /* [out] */ __RPC__deref_out_opt IVsButtonBar **ppButtonBar) = 0;
        
        virtual HRESULT STDMETHODCALLTYPE RemoveButtonBar( void) = 0;
        
    };
    
#else 	/* C style interface */

    typedef struct IVsButtonBarManagerVtbl
    {
        BEGIN_INTERFACE
        
        HRESULT ( STDMETHODCALLTYPE *QueryInterface )( 
            IVsButtonBarManager * This,
            /* [in] */ __RPC__in REFIID riid,
            /* [iid_is][out] */ 
            __RPC__deref_out  void **ppvObject);
        
        ULONG ( STDMETHODCALLTYPE *AddRef )( 
            IVsButtonBarManager * This);
        
        ULONG ( STDMETHODCALLTYPE *Release )( 
            IVsButtonBarManager * This);
        
        HRESULT ( STDMETHODCALLTYPE *AddButtonBar )( 
            IVsButtonBarManager * This,
            /* [in] */ long cButtons,
            /* [in] */ __RPC__in HANDLE hImageList,
            /* [in] */ __RPC__in_opt IVsButtonBarClient *pClient);
        
        HRESULT ( STDMETHODCALLTYPE *GetButtonBar )( 
            IVsButtonBarManager * This,
            /* [out] */ __RPC__deref_out_opt IVsButtonBar **ppButtonBar);
        
        HRESULT ( STDMETHODCALLTYPE *RemoveButtonBar )( 
            IVsButtonBarManager * This);
        
        END_INTERFACE
    } IVsButtonBarManagerVtbl;

    interface IVsButtonBarManager
    {
        CONST_VTBL struct IVsButtonBarManagerVtbl *lpVtbl;
    };

    

#ifdef COBJMACROS


#define IVsButtonBarManager_QueryInterface(This,riid,ppvObject)	\
    ( (This)->lpVtbl -> QueryInterface(This,riid,ppvObject) ) 

#define IVsButtonBarManager_AddRef(This)	\
    ( (This)->lpVtbl -> AddRef(This) ) 

#define IVsButtonBarManager_Release(This)	\
    ( (This)->lpVtbl -> Release(This) ) 


#define IVsButtonBarManager_AddButtonBar(This,cButtons,hImageList,pClient)	\
    ( (This)->lpVtbl -> AddButtonBar(This,cButtons,hImageList,pClient) ) 

#define IVsButtonBarManager_GetButtonBar(This,ppButtonBar)	\
    ( (This)->lpVtbl -> GetButtonBar(This,ppButtonBar) ) 

#define IVsButtonBarManager_RemoveButtonBar(This)	\
    ( (This)->lpVtbl -> RemoveButtonBar(This) ) 

#endif /* COBJMACROS */


#endif 	/* C style interface */




#endif 	/* __IVsButtonBarManager_INTERFACE_DEFINED__ */


#ifndef __IVsDefaultButtonBarImages_INTERFACE_DEFINED__
#define __IVsDefaultButtonBarImages_INTERFACE_DEFINED__

/* interface IVsDefaultButtonBarImages */
/* [object][uuid] */ 


EXTERN_C const IID IID_IVsDefaultButtonBarImages;

#if defined(__cplusplus) && !defined(CINTERFACE)
    
    MIDL_INTERFACE("73B7DC00-F498-4ABD-AB79-D07AFD52F395")
    IVsDefaultButtonBarImages : public IUnknown
    {
    public:
        virtual HRESULT STDMETHODCALLTYPE GetButtonCount( 
            /* [out] */ __RPC__out long *pcButtons) = 0;
        
        virtual HRESULT STDMETHODCALLTYPE GetImageList( 
            /* [out] */ __RPC__deref_out_opt HANDLE *phImageList) = 0;
        
    };
    
#else 	/* C style interface */

    typedef struct IVsDefaultButtonBarImagesVtbl
    {
        BEGIN_INTERFACE
        
        HRESULT ( STDMETHODCALLTYPE *QueryInterface )( 
            IVsDefaultButtonBarImages * This,
            /* [in] */ __RPC__in REFIID riid,
            /* [iid_is][out] */ 
            __RPC__deref_out  void **ppvObject);
        
        ULONG ( STDMETHODCALLTYPE *AddRef )( 
            IVsDefaultButtonBarImages * This);
        
        ULONG ( STDMETHODCALLTYPE *Release )( 
            IVsDefaultButtonBarImages * This);
        
        HRESULT ( STDMETHODCALLTYPE *GetButtonCount )( 
            IVsDefaultButtonBarImages * This,
            /* [out] */ __RPC__out long *pcButtons);
        
        HRESULT ( STDMETHODCALLTYPE *GetImageList )( 
            IVsDefaultButtonBarImages * This,
            /* [out] */ __RPC__deref_out_opt HANDLE *phImageList);
        
        END_INTERFACE
    } IVsDefaultButtonBarImagesVtbl;

    interface IVsDefaultButtonBarImages
    {
        CONST_VTBL struct IVsDefaultButtonBarImagesVtbl *lpVtbl;
    };

    

#ifdef COBJMACROS


#define IVsDefaultButtonBarImages_QueryInterface(This,riid,ppvObject)	\
    ( (This)->lpVtbl -> QueryInterface(This,riid,ppvObject) ) 

#define IVsDefaultButtonBarImages_AddRef(This)	\
    ( (This)->lpVtbl -> AddRef(This) ) 

#define IVsDefaultButtonBarImages_Release(This)	\
    ( (This)->lpVtbl -> Release(This) ) 


#define IVsDefaultButtonBarImages_GetButtonCount(This,pcButtons)	\
    ( (This)->lpVtbl -> GetButtonCount(This,pcButtons) ) 

#define IVsDefaultButtonBarImages_GetImageList(This,phImageList)	\
    ( (This)->lpVtbl -> GetImageList(This,phImageList) ) 

#endif /* COBJMACROS */


#endif 	/* C style interface */




#endif 	/* __IVsDefaultButtonBarImages_INTERFACE_DEFINED__ */


#ifndef __IVsTextMacroHelper_INTERFACE_DEFINED__
#define __IVsTextMacroHelper_INTERFACE_DEFINED__

/* interface IVsTextMacroHelper */
/* [object][uuid] */ 


EXTERN_C const IID IID_IVsTextMacroHelper;

#if defined(__cplusplus) && !defined(CINTERFACE)
    
    MIDL_INTERFACE("CAF8109F-B74B-429F-A11A-4944AADE9AB2")
    IVsTextMacroHelper : public IUnknown
    {
    public:
        virtual HRESULT STDMETHODCALLTYPE RecordNewLine( void) = 0;
        
        virtual HRESULT STDMETHODCALLTYPE RecordTypeChar( 
            OLECHAR wchChar,
            BOOL fIsOvertype) = 0;
        
        virtual HRESULT STDMETHODCALLTYPE RecordTypeChars( 
            __RPC__in LPCOLESTR pwszChars,
            BOOL fIsOvertype) = 0;
        
        virtual HRESULT STDMETHODCALLTYPE RecordRemovePreviousTyping( 
            __RPC__in LPCOLESTR pwszPrevChars) = 0;
        
        virtual HRESULT STDMETHODCALLTYPE RecordDelete( 
            BOOL fLeft,
            UINT uiReps) = 0;
        
        virtual HRESULT STDMETHODCALLTYPE RecordDeleteSpace( 
            BOOL fVertical) = 0;
        
        virtual HRESULT STDMETHODCALLTYPE RecordMoveSelectionRel( 
            MOVESELECTION_REL_TYPE mst,
            BOOL fBackwards,
            BOOL fExtend) = 0;
        
        virtual HRESULT STDMETHODCALLTYPE RecordMoveSelectionAbs( 
            MOVESELECTION_ABS_TYPE mst,
            BOOL fExtend) = 0;
        
        virtual HRESULT STDMETHODCALLTYPE RecordCollapseSelection( void) = 0;
        
        virtual HRESULT STDMETHODCALLTYPE RecordSelectAll( void) = 0;
        
        virtual HRESULT STDMETHODCALLTYPE RecordSwapAnchor( void) = 0;
        
        virtual HRESULT STDMETHODCALLTYPE RecordEnterBoxMode( void) = 0;
        
        virtual HRESULT STDMETHODCALLTYPE RecordActivateDocument( void) = 0;
        
        virtual HRESULT STDMETHODCALLTYPE RecordGotoLine( 
            long iLine,
            BOOL fExtend) = 0;
        
        virtual HRESULT STDMETHODCALLTYPE RecordCut( void) = 0;
        
        virtual HRESULT STDMETHODCALLTYPE RecordCopy( void) = 0;
        
        virtual HRESULT STDMETHODCALLTYPE RecordPaste( void) = 0;
        
        virtual HRESULT STDMETHODCALLTYPE RecordBookmarkClearAll( void) = 0;
        
        virtual HRESULT STDMETHODCALLTYPE RecordBookmarkSetClear( 
            BOOL fSet) = 0;
        
        virtual HRESULT STDMETHODCALLTYPE RecordBookmarkNextPrev( 
            BOOL fNext) = 0;
        
        virtual HRESULT STDMETHODCALLTYPE RecordChangeCase( 
            CASESELECTION_TYPE cst) = 0;
        
        virtual HRESULT STDMETHODCALLTYPE RecordIndentUnindent( 
            BOOL fIndent) = 0;
        
        virtual HRESULT STDMETHODCALLTYPE RecordFormatSelection( void) = 0;
        
        virtual HRESULT STDMETHODCALLTYPE RecordTabifyUntabify( 
            BOOL fTabify) = 0;
        
        virtual HRESULT STDMETHODCALLTYPE RecordInsertFile( 
            __RPC__in LPCOLESTR pwszName) = 0;
        
    };
    
#else 	/* C style interface */

    typedef struct IVsTextMacroHelperVtbl
    {
        BEGIN_INTERFACE
        
        HRESULT ( STDMETHODCALLTYPE *QueryInterface )( 
            IVsTextMacroHelper * This,
            /* [in] */ __RPC__in REFIID riid,
            /* [iid_is][out] */ 
            __RPC__deref_out  void **ppvObject);
        
        ULONG ( STDMETHODCALLTYPE *AddRef )( 
            IVsTextMacroHelper * This);
        
        ULONG ( STDMETHODCALLTYPE *Release )( 
            IVsTextMacroHelper * This);
        
        HRESULT ( STDMETHODCALLTYPE *RecordNewLine )( 
            IVsTextMacroHelper * This);
        
        HRESULT ( STDMETHODCALLTYPE *RecordTypeChar )( 
            IVsTextMacroHelper * This,
            OLECHAR wchChar,
            BOOL fIsOvertype);
        
        HRESULT ( STDMETHODCALLTYPE *RecordTypeChars )( 
            IVsTextMacroHelper * This,
            __RPC__in LPCOLESTR pwszChars,
            BOOL fIsOvertype);
        
        HRESULT ( STDMETHODCALLTYPE *RecordRemovePreviousTyping )( 
            IVsTextMacroHelper * This,
            __RPC__in LPCOLESTR pwszPrevChars);
        
        HRESULT ( STDMETHODCALLTYPE *RecordDelete )( 
            IVsTextMacroHelper * This,
            BOOL fLeft,
            UINT uiReps);
        
        HRESULT ( STDMETHODCALLTYPE *RecordDeleteSpace )( 
            IVsTextMacroHelper * This,
            BOOL fVertical);
        
        HRESULT ( STDMETHODCALLTYPE *RecordMoveSelectionRel )( 
            IVsTextMacroHelper * This,
            MOVESELECTION_REL_TYPE mst,
            BOOL fBackwards,
            BOOL fExtend);
        
        HRESULT ( STDMETHODCALLTYPE *RecordMoveSelectionAbs )( 
            IVsTextMacroHelper * This,
            MOVESELECTION_ABS_TYPE mst,
            BOOL fExtend);
        
        HRESULT ( STDMETHODCALLTYPE *RecordCollapseSelection )( 
            IVsTextMacroHelper * This);
        
        HRESULT ( STDMETHODCALLTYPE *RecordSelectAll )( 
            IVsTextMacroHelper * This);
        
        HRESULT ( STDMETHODCALLTYPE *RecordSwapAnchor )( 
            IVsTextMacroHelper * This);
        
        HRESULT ( STDMETHODCALLTYPE *RecordEnterBoxMode )( 
            IVsTextMacroHelper * This);
        
        HRESULT ( STDMETHODCALLTYPE *RecordActivateDocument )( 
            IVsTextMacroHelper * This);
        
        HRESULT ( STDMETHODCALLTYPE *RecordGotoLine )( 
            IVsTextMacroHelper * This,
            long iLine,
            BOOL fExtend);
        
        HRESULT ( STDMETHODCALLTYPE *RecordCut )( 
            IVsTextMacroHelper * This);
        
        HRESULT ( STDMETHODCALLTYPE *RecordCopy )( 
            IVsTextMacroHelper * This);
        
        HRESULT ( STDMETHODCALLTYPE *RecordPaste )( 
            IVsTextMacroHelper * This);
        
        HRESULT ( STDMETHODCALLTYPE *RecordBookmarkClearAll )( 
            IVsTextMacroHelper * This);
        
        HRESULT ( STDMETHODCALLTYPE *RecordBookmarkSetClear )( 
            IVsTextMacroHelper * This,
            BOOL fSet);
        
        HRESULT ( STDMETHODCALLTYPE *RecordBookmarkNextPrev )( 
            IVsTextMacroHelper * This,
            BOOL fNext);
        
        HRESULT ( STDMETHODCALLTYPE *RecordChangeCase )( 
            IVsTextMacroHelper * This,
            CASESELECTION_TYPE cst);
        
        HRESULT ( STDMETHODCALLTYPE *RecordIndentUnindent )( 
            IVsTextMacroHelper * This,
            BOOL fIndent);
        
        HRESULT ( STDMETHODCALLTYPE *RecordFormatSelection )( 
            IVsTextMacroHelper * This);
        
        HRESULT ( STDMETHODCALLTYPE *RecordTabifyUntabify )( 
            IVsTextMacroHelper * This,
            BOOL fTabify);
        
        HRESULT ( STDMETHODCALLTYPE *RecordInsertFile )( 
            IVsTextMacroHelper * This,
            __RPC__in LPCOLESTR pwszName);
        
        END_INTERFACE
    } IVsTextMacroHelperVtbl;

    interface IVsTextMacroHelper
    {
        CONST_VTBL struct IVsTextMacroHelperVtbl *lpVtbl;
    };

    

#ifdef COBJMACROS


#define IVsTextMacroHelper_QueryInterface(This,riid,ppvObject)	\
    ( (This)->lpVtbl -> QueryInterface(This,riid,ppvObject) ) 

#define IVsTextMacroHelper_AddRef(This)	\
    ( (This)->lpVtbl -> AddRef(This) ) 

#define IVsTextMacroHelper_Release(This)	\
    ( (This)->lpVtbl -> Release(This) ) 


#define IVsTextMacroHelper_RecordNewLine(This)	\
    ( (This)->lpVtbl -> RecordNewLine(This) ) 

#define IVsTextMacroHelper_RecordTypeChar(This,wchChar,fIsOvertype)	\
    ( (This)->lpVtbl -> RecordTypeChar(This,wchChar,fIsOvertype) ) 

#define IVsTextMacroHelper_RecordTypeChars(This,pwszChars,fIsOvertype)	\
    ( (This)->lpVtbl -> RecordTypeChars(This,pwszChars,fIsOvertype) ) 

#define IVsTextMacroHelper_RecordRemovePreviousTyping(This,pwszPrevChars)	\
    ( (This)->lpVtbl -> RecordRemovePreviousTyping(This,pwszPrevChars) ) 

#define IVsTextMacroHelper_RecordDelete(This,fLeft,uiReps)	\
    ( (This)->lpVtbl -> RecordDelete(This,fLeft,uiReps) ) 

#define IVsTextMacroHelper_RecordDeleteSpace(This,fVertical)	\
    ( (This)->lpVtbl -> RecordDeleteSpace(This,fVertical) ) 

#define IVsTextMacroHelper_RecordMoveSelectionRel(This,mst,fBackwards,fExtend)	\
    ( (This)->lpVtbl -> RecordMoveSelectionRel(This,mst,fBackwards,fExtend) ) 

#define IVsTextMacroHelper_RecordMoveSelectionAbs(This,mst,fExtend)	\
    ( (This)->lpVtbl -> RecordMoveSelectionAbs(This,mst,fExtend) ) 

#define IVsTextMacroHelper_RecordCollapseSelection(This)	\
    ( (This)->lpVtbl -> RecordCollapseSelection(This) ) 

#define IVsTextMacroHelper_RecordSelectAll(This)	\
    ( (This)->lpVtbl -> RecordSelectAll(This) ) 

#define IVsTextMacroHelper_RecordSwapAnchor(This)	\
    ( (This)->lpVtbl -> RecordSwapAnchor(This) ) 

#define IVsTextMacroHelper_RecordEnterBoxMode(This)	\
    ( (This)->lpVtbl -> RecordEnterBoxMode(This) ) 

#define IVsTextMacroHelper_RecordActivateDocument(This)	\
    ( (This)->lpVtbl -> RecordActivateDocument(This) ) 

#define IVsTextMacroHelper_RecordGotoLine(This,iLine,fExtend)	\
    ( (This)->lpVtbl -> RecordGotoLine(This,iLine,fExtend) ) 

#define IVsTextMacroHelper_RecordCut(This)	\
    ( (This)->lpVtbl -> RecordCut(This) ) 

#define IVsTextMacroHelper_RecordCopy(This)	\
    ( (This)->lpVtbl -> RecordCopy(This) ) 

#define IVsTextMacroHelper_RecordPaste(This)	\
    ( (This)->lpVtbl -> RecordPaste(This) ) 

#define IVsTextMacroHelper_RecordBookmarkClearAll(This)	\
    ( (This)->lpVtbl -> RecordBookmarkClearAll(This) ) 

#define IVsTextMacroHelper_RecordBookmarkSetClear(This,fSet)	\
    ( (This)->lpVtbl -> RecordBookmarkSetClear(This,fSet) ) 

#define IVsTextMacroHelper_RecordBookmarkNextPrev(This,fNext)	\
    ( (This)->lpVtbl -> RecordBookmarkNextPrev(This,fNext) ) 

#define IVsTextMacroHelper_RecordChangeCase(This,cst)	\
    ( (This)->lpVtbl -> RecordChangeCase(This,cst) ) 

#define IVsTextMacroHelper_RecordIndentUnindent(This,fIndent)	\
    ( (This)->lpVtbl -> RecordIndentUnindent(This,fIndent) ) 

#define IVsTextMacroHelper_RecordFormatSelection(This)	\
    ( (This)->lpVtbl -> RecordFormatSelection(This) ) 

#define IVsTextMacroHelper_RecordTabifyUntabify(This,fTabify)	\
    ( (This)->lpVtbl -> RecordTabifyUntabify(This,fTabify) ) 

#define IVsTextMacroHelper_RecordInsertFile(This,pwszName)	\
    ( (This)->lpVtbl -> RecordInsertFile(This,pwszName) ) 

#endif /* COBJMACROS */


#endif 	/* C style interface */




#endif 	/* __IVsTextMacroHelper_INTERFACE_DEFINED__ */


#ifndef __IVsUserData_INTERFACE_DEFINED__
#define __IVsUserData_INTERFACE_DEFINED__

/* interface IVsUserData */
/* [object][uuid] */ 


EXTERN_C const IID IID_IVsUserData;

#if defined(__cplusplus) && !defined(CINTERFACE)
    
    MIDL_INTERFACE("978A8E17-4DF8-432A-9623-D530A26452BC")
    IVsUserData : public IUnknown
    {
    public:
        virtual HRESULT STDMETHODCALLTYPE GetData( 
            /* [in] */ __RPC__in REFGUID riidKey,
            /* [out] */ __RPC__out VARIANT *pvtData) = 0;
        
        virtual HRESULT STDMETHODCALLTYPE SetData( 
            /* [in] */ __RPC__in REFGUID riidKey,
            /* [in] */ VARIANT vtData) = 0;
        
    };
    
#else 	/* C style interface */

    typedef struct IVsUserDataVtbl
    {
        BEGIN_INTERFACE
        
        HRESULT ( STDMETHODCALLTYPE *QueryInterface )( 
            IVsUserData * This,
            /* [in] */ __RPC__in REFIID riid,
            /* [iid_is][out] */ 
            __RPC__deref_out  void **ppvObject);
        
        ULONG ( STDMETHODCALLTYPE *AddRef )( 
            IVsUserData * This);
        
        ULONG ( STDMETHODCALLTYPE *Release )( 
            IVsUserData * This);
        
        HRESULT ( STDMETHODCALLTYPE *GetData )( 
            IVsUserData * This,
            /* [in] */ __RPC__in REFGUID riidKey,
            /* [out] */ __RPC__out VARIANT *pvtData);
        
        HRESULT ( STDMETHODCALLTYPE *SetData )( 
            IVsUserData * This,
            /* [in] */ __RPC__in REFGUID riidKey,
            /* [in] */ VARIANT vtData);
        
        END_INTERFACE
    } IVsUserDataVtbl;

    interface IVsUserData
    {
        CONST_VTBL struct IVsUserDataVtbl *lpVtbl;
    };

    

#ifdef COBJMACROS


#define IVsUserData_QueryInterface(This,riid,ppvObject)	\
    ( (This)->lpVtbl -> QueryInterface(This,riid,ppvObject) ) 

#define IVsUserData_AddRef(This)	\
    ( (This)->lpVtbl -> AddRef(This) ) 

#define IVsUserData_Release(This)	\
    ( (This)->lpVtbl -> Release(This) ) 


#define IVsUserData_GetData(This,riidKey,pvtData)	\
    ( (This)->lpVtbl -> GetData(This,riidKey,pvtData) ) 

#define IVsUserData_SetData(This,riidKey,vtData)	\
    ( (This)->lpVtbl -> SetData(This,riidKey,vtData) ) 

#endif /* COBJMACROS */


#endif 	/* C style interface */




#endif 	/* __IVsUserData_INTERFACE_DEFINED__ */


#ifndef __IVsUserDataEvents_INTERFACE_DEFINED__
#define __IVsUserDataEvents_INTERFACE_DEFINED__

/* interface IVsUserDataEvents */
/* [object][uuid] */ 


EXTERN_C const IID IID_IVsUserDataEvents;

#if defined(__cplusplus) && !defined(CINTERFACE)
    
    MIDL_INTERFACE("5E120D5E-9637-47D9-861C-92FA03683C6C")
    IVsUserDataEvents : public IUnknown
    {
    public:
        virtual void STDMETHODCALLTYPE OnUserDataChange( 
            /* [in] */ __RPC__in REFGUID riidKey,
            /* [in] */ VARIANT vtNewValue) = 0;
        
    };
    
#else 	/* C style interface */

    typedef struct IVsUserDataEventsVtbl
    {
        BEGIN_INTERFACE
        
        HRESULT ( STDMETHODCALLTYPE *QueryInterface )( 
            IVsUserDataEvents * This,
            /* [in] */ __RPC__in REFIID riid,
            /* [iid_is][out] */ 
            __RPC__deref_out  void **ppvObject);
        
        ULONG ( STDMETHODCALLTYPE *AddRef )( 
            IVsUserDataEvents * This);
        
        ULONG ( STDMETHODCALLTYPE *Release )( 
            IVsUserDataEvents * This);
        
        void ( STDMETHODCALLTYPE *OnUserDataChange )( 
            IVsUserDataEvents * This,
            /* [in] */ __RPC__in REFGUID riidKey,
            /* [in] */ VARIANT vtNewValue);
        
        END_INTERFACE
    } IVsUserDataEventsVtbl;

    interface IVsUserDataEvents
    {
        CONST_VTBL struct IVsUserDataEventsVtbl *lpVtbl;
    };

    

#ifdef COBJMACROS


#define IVsUserDataEvents_QueryInterface(This,riid,ppvObject)	\
    ( (This)->lpVtbl -> QueryInterface(This,riid,ppvObject) ) 

#define IVsUserDataEvents_AddRef(This)	\
    ( (This)->lpVtbl -> AddRef(This) ) 

#define IVsUserDataEvents_Release(This)	\
    ( (This)->lpVtbl -> Release(This) ) 


#define IVsUserDataEvents_OnUserDataChange(This,riidKey,vtNewValue)	\
    ( (This)->lpVtbl -> OnUserDataChange(This,riidKey,vtNewValue) ) 

#endif /* COBJMACROS */


#endif 	/* C style interface */




#endif 	/* __IVsUserDataEvents_INTERFACE_DEFINED__ */


#ifndef __IVsUndoUnit_INTERFACE_DEFINED__
#define __IVsUndoUnit_INTERFACE_DEFINED__

/* interface IVsUndoUnit */
/* [object][uuid] */ 


EXTERN_C const IID IID_IVsUndoUnit;

#if defined(__cplusplus) && !defined(CINTERFACE)
    
    MIDL_INTERFACE("A8C3D107-7EDF-4B1B-8A81-080AC91590E9")
    IVsUndoUnit : public IUnknown
    {
    public:
        virtual BOOL STDMETHODCALLTYPE fChangeData( void) = 0;
        
    };
    
#else 	/* C style interface */

    typedef struct IVsUndoUnitVtbl
    {
        BEGIN_INTERFACE
        
        HRESULT ( STDMETHODCALLTYPE *QueryInterface )( 
            IVsUndoUnit * This,
            /* [in] */ __RPC__in REFIID riid,
            /* [iid_is][out] */ 
            __RPC__deref_out  void **ppvObject);
        
        ULONG ( STDMETHODCALLTYPE *AddRef )( 
            IVsUndoUnit * This);
        
        ULONG ( STDMETHODCALLTYPE *Release )( 
            IVsUndoUnit * This);
        
        BOOL ( STDMETHODCALLTYPE *fChangeData )( 
            IVsUndoUnit * This);
        
        END_INTERFACE
    } IVsUndoUnitVtbl;

    interface IVsUndoUnit
    {
        CONST_VTBL struct IVsUndoUnitVtbl *lpVtbl;
    };

    

#ifdef COBJMACROS


#define IVsUndoUnit_QueryInterface(This,riid,ppvObject)	\
    ( (This)->lpVtbl -> QueryInterface(This,riid,ppvObject) ) 

#define IVsUndoUnit_AddRef(This)	\
    ( (This)->lpVtbl -> AddRef(This) ) 

#define IVsUndoUnit_Release(This)	\
    ( (This)->lpVtbl -> Release(This) ) 


#define IVsUndoUnit_fChangeData(This)	\
    ( (This)->lpVtbl -> fChangeData(This) ) 

#endif /* COBJMACROS */


#endif 	/* C style interface */




#endif 	/* __IVsUndoUnit_INTERFACE_DEFINED__ */


#ifndef __IVsUndoTrackingEvents_INTERFACE_DEFINED__
#define __IVsUndoTrackingEvents_INTERFACE_DEFINED__

/* interface IVsUndoTrackingEvents */
/* [object][uuid] */ 


EXTERN_C const IID IID_IVsUndoTrackingEvents;

#if defined(__cplusplus) && !defined(CINTERFACE)
    
    MIDL_INTERFACE("31024DD7-30E5-4C91-821D-218BDEADA18B")
    IVsUndoTrackingEvents : public IUnknown
    {
    public:
        virtual void STDMETHODCALLTYPE OnReturnToCleanState( void) = 0;
        
    };
    
#else 	/* C style interface */

    typedef struct IVsUndoTrackingEventsVtbl
    {
        BEGIN_INTERFACE
        
        HRESULT ( STDMETHODCALLTYPE *QueryInterface )( 
            IVsUndoTrackingEvents * This,
            /* [in] */ __RPC__in REFIID riid,
            /* [iid_is][out] */ 
            __RPC__deref_out  void **ppvObject);
        
        ULONG ( STDMETHODCALLTYPE *AddRef )( 
            IVsUndoTrackingEvents * This);
        
        ULONG ( STDMETHODCALLTYPE *Release )( 
            IVsUndoTrackingEvents * This);
        
        void ( STDMETHODCALLTYPE *OnReturnToCleanState )( 
            IVsUndoTrackingEvents * This);
        
        END_INTERFACE
    } IVsUndoTrackingEventsVtbl;

    interface IVsUndoTrackingEvents
    {
        CONST_VTBL struct IVsUndoTrackingEventsVtbl *lpVtbl;
    };

    

#ifdef COBJMACROS


#define IVsUndoTrackingEvents_QueryInterface(This,riid,ppvObject)	\
    ( (This)->lpVtbl -> QueryInterface(This,riid,ppvObject) ) 

#define IVsUndoTrackingEvents_AddRef(This)	\
    ( (This)->lpVtbl -> AddRef(This) ) 

#define IVsUndoTrackingEvents_Release(This)	\
    ( (This)->lpVtbl -> Release(This) ) 


#define IVsUndoTrackingEvents_OnReturnToCleanState(This)	\
    ( (This)->lpVtbl -> OnReturnToCleanState(This) ) 

#endif /* COBJMACROS */


#endif 	/* C style interface */




#endif 	/* __IVsUndoTrackingEvents_INTERFACE_DEFINED__ */


#ifndef __IVsChangeTrackingUndoManager_INTERFACE_DEFINED__
#define __IVsChangeTrackingUndoManager_INTERFACE_DEFINED__

/* interface IVsChangeTrackingUndoManager */
/* [object][uuid] */ 


EXTERN_C const IID IID_IVsChangeTrackingUndoManager;

#if defined(__cplusplus) && !defined(CINTERFACE)
    
    MIDL_INTERFACE("E33C4DCF-F50C-4356-8CD8-87A7F213337B")
    IVsChangeTrackingUndoManager : public IUnknown
    {
    public:
        virtual HRESULT STDMETHODCALLTYPE MarkCleanState( void) = 0;
        
        virtual HRESULT STDMETHODCALLTYPE QueryCleanState( 
            /* [out] */ __RPC__out BOOL *pfClean) = 0;
        
        virtual HRESULT STDMETHODCALLTYPE AdviseTrackingClient( 
            /* [in] */ __RPC__in_opt IVsUndoTrackingEvents *pUndoTrackingEvents) = 0;
        
        virtual HRESULT STDMETHODCALLTYPE UnadviseTrackingClient( void) = 0;
        
    };
    
#else 	/* C style interface */

    typedef struct IVsChangeTrackingUndoManagerVtbl
    {
        BEGIN_INTERFACE
        
        HRESULT ( STDMETHODCALLTYPE *QueryInterface )( 
            IVsChangeTrackingUndoManager * This,
            /* [in] */ __RPC__in REFIID riid,
            /* [iid_is][out] */ 
            __RPC__deref_out  void **ppvObject);
        
        ULONG ( STDMETHODCALLTYPE *AddRef )( 
            IVsChangeTrackingUndoManager * This);
        
        ULONG ( STDMETHODCALLTYPE *Release )( 
            IVsChangeTrackingUndoManager * This);
        
        HRESULT ( STDMETHODCALLTYPE *MarkCleanState )( 
            IVsChangeTrackingUndoManager * This);
        
        HRESULT ( STDMETHODCALLTYPE *QueryCleanState )( 
            IVsChangeTrackingUndoManager * This,
            /* [out] */ __RPC__out BOOL *pfClean);
        
        HRESULT ( STDMETHODCALLTYPE *AdviseTrackingClient )( 
            IVsChangeTrackingUndoManager * This,
            /* [in] */ __RPC__in_opt IVsUndoTrackingEvents *pUndoTrackingEvents);
        
        HRESULT ( STDMETHODCALLTYPE *UnadviseTrackingClient )( 
            IVsChangeTrackingUndoManager * This);
        
        END_INTERFACE
    } IVsChangeTrackingUndoManagerVtbl;

    interface IVsChangeTrackingUndoManager
    {
        CONST_VTBL struct IVsChangeTrackingUndoManagerVtbl *lpVtbl;
    };

    

#ifdef COBJMACROS


#define IVsChangeTrackingUndoManager_QueryInterface(This,riid,ppvObject)	\
    ( (This)->lpVtbl -> QueryInterface(This,riid,ppvObject) ) 

#define IVsChangeTrackingUndoManager_AddRef(This)	\
    ( (This)->lpVtbl -> AddRef(This) ) 

#define IVsChangeTrackingUndoManager_Release(This)	\
    ( (This)->lpVtbl -> Release(This) ) 


#define IVsChangeTrackingUndoManager_MarkCleanState(This)	\
    ( (This)->lpVtbl -> MarkCleanState(This) ) 

#define IVsChangeTrackingUndoManager_QueryCleanState(This,pfClean)	\
    ( (This)->lpVtbl -> QueryCleanState(This,pfClean) ) 

#define IVsChangeTrackingUndoManager_AdviseTrackingClient(This,pUndoTrackingEvents)	\
    ( (This)->lpVtbl -> AdviseTrackingClient(This,pUndoTrackingEvents) ) 

#define IVsChangeTrackingUndoManager_UnadviseTrackingClient(This)	\
    ( (This)->lpVtbl -> UnadviseTrackingClient(This) ) 

#endif /* COBJMACROS */


#endif 	/* C style interface */




#endif 	/* __IVsChangeTrackingUndoManager_INTERFACE_DEFINED__ */


#ifndef __IVsLinkedUndoClient_INTERFACE_DEFINED__
#define __IVsLinkedUndoClient_INTERFACE_DEFINED__

/* interface IVsLinkedUndoClient */
/* [object][uuid] */ 


EXTERN_C const IID IID_IVsLinkedUndoClient;

#if defined(__cplusplus) && !defined(CINTERFACE)
    
    MIDL_INTERFACE("33452684-FAB0-4F76-B4E9-19EE0B96B4AD")
    IVsLinkedUndoClient : public IUnknown
    {
    public:
        virtual HRESULT STDMETHODCALLTYPE OnInterveningUnitBlockingLinkedUndo( void) = 0;
        
    };
    
#else 	/* C style interface */

    typedef struct IVsLinkedUndoClientVtbl
    {
        BEGIN_INTERFACE
        
        HRESULT ( STDMETHODCALLTYPE *QueryInterface )( 
            IVsLinkedUndoClient * This,
            /* [in] */ __RPC__in REFIID riid,
            /* [iid_is][out] */ 
            __RPC__deref_out  void **ppvObject);
        
        ULONG ( STDMETHODCALLTYPE *AddRef )( 
            IVsLinkedUndoClient * This);
        
        ULONG ( STDMETHODCALLTYPE *Release )( 
            IVsLinkedUndoClient * This);
        
        HRESULT ( STDMETHODCALLTYPE *OnInterveningUnitBlockingLinkedUndo )( 
            IVsLinkedUndoClient * This);
        
        END_INTERFACE
    } IVsLinkedUndoClientVtbl;

    interface IVsLinkedUndoClient
    {
        CONST_VTBL struct IVsLinkedUndoClientVtbl *lpVtbl;
    };

    

#ifdef COBJMACROS


#define IVsLinkedUndoClient_QueryInterface(This,riid,ppvObject)	\
    ( (This)->lpVtbl -> QueryInterface(This,riid,ppvObject) ) 

#define IVsLinkedUndoClient_AddRef(This)	\
    ( (This)->lpVtbl -> AddRef(This) ) 

#define IVsLinkedUndoClient_Release(This)	\
    ( (This)->lpVtbl -> Release(This) ) 


#define IVsLinkedUndoClient_OnInterveningUnitBlockingLinkedUndo(This)	\
    ( (This)->lpVtbl -> OnInterveningUnitBlockingLinkedUndo(This) ) 

#endif /* COBJMACROS */


#endif 	/* C style interface */




#endif 	/* __IVsLinkedUndoClient_INTERFACE_DEFINED__ */


#ifndef __IVsLinkCapableUndoManager_INTERFACE_DEFINED__
#define __IVsLinkCapableUndoManager_INTERFACE_DEFINED__

/* interface IVsLinkCapableUndoManager */
/* [object][uuid] */ 


EXTERN_C const IID IID_IVsLinkCapableUndoManager;

#if defined(__cplusplus) && !defined(CINTERFACE)
    
    MIDL_INTERFACE("F747D466-83CA-41D6-A0E8-3F78283D01E7")
    IVsLinkCapableUndoManager : public IUnknown
    {
    public:
        virtual HRESULT STDMETHODCALLTYPE AdviseLinkedUndoClient( 
            /* [in] */ __RPC__in_opt IVsLinkedUndoClient *pClient) = 0;
        
        virtual HRESULT STDMETHODCALLTYPE UnadviseLinkedUndoClient( void) = 0;
        
    };
    
#else 	/* C style interface */

    typedef struct IVsLinkCapableUndoManagerVtbl
    {
        BEGIN_INTERFACE
        
        HRESULT ( STDMETHODCALLTYPE *QueryInterface )( 
            IVsLinkCapableUndoManager * This,
            /* [in] */ __RPC__in REFIID riid,
            /* [iid_is][out] */ 
            __RPC__deref_out  void **ppvObject);
        
        ULONG ( STDMETHODCALLTYPE *AddRef )( 
            IVsLinkCapableUndoManager * This);
        
        ULONG ( STDMETHODCALLTYPE *Release )( 
            IVsLinkCapableUndoManager * This);
        
        HRESULT ( STDMETHODCALLTYPE *AdviseLinkedUndoClient )( 
            IVsLinkCapableUndoManager * This,
            /* [in] */ __RPC__in_opt IVsLinkedUndoClient *pClient);
        
        HRESULT ( STDMETHODCALLTYPE *UnadviseLinkedUndoClient )( 
            IVsLinkCapableUndoManager * This);
        
        END_INTERFACE
    } IVsLinkCapableUndoManagerVtbl;

    interface IVsLinkCapableUndoManager
    {
        CONST_VTBL struct IVsLinkCapableUndoManagerVtbl *lpVtbl;
    };

    

#ifdef COBJMACROS


#define IVsLinkCapableUndoManager_QueryInterface(This,riid,ppvObject)	\
    ( (This)->lpVtbl -> QueryInterface(This,riid,ppvObject) ) 

#define IVsLinkCapableUndoManager_AddRef(This)	\
    ( (This)->lpVtbl -> AddRef(This) ) 

#define IVsLinkCapableUndoManager_Release(This)	\
    ( (This)->lpVtbl -> Release(This) ) 


#define IVsLinkCapableUndoManager_AdviseLinkedUndoClient(This,pClient)	\
    ( (This)->lpVtbl -> AdviseLinkedUndoClient(This,pClient) ) 

#define IVsLinkCapableUndoManager_UnadviseLinkedUndoClient(This)	\
    ( (This)->lpVtbl -> UnadviseLinkedUndoClient(This) ) 

#endif /* COBJMACROS */


#endif 	/* C style interface */




#endif 	/* __IVsLinkCapableUndoManager_INTERFACE_DEFINED__ */


#ifndef __IVsLifetimeControlledObject_INTERFACE_DEFINED__
#define __IVsLifetimeControlledObject_INTERFACE_DEFINED__

/* interface IVsLifetimeControlledObject */
/* [object][uuid] */ 


EXTERN_C const IID IID_IVsLifetimeControlledObject;

#if defined(__cplusplus) && !defined(CINTERFACE)
    
    MIDL_INTERFACE("B3B2E5D4-F03E-4BB9-85F9-61B5F45AC498")
    IVsLifetimeControlledObject : public IUnknown
    {
    public:
        virtual HRESULT STDMETHODCALLTYPE SeverReferencesToOwner( void) = 0;
        
    };
    
#else 	/* C style interface */

    typedef struct IVsLifetimeControlledObjectVtbl
    {
        BEGIN_INTERFACE
        
        HRESULT ( STDMETHODCALLTYPE *QueryInterface )( 
            IVsLifetimeControlledObject * This,
            /* [in] */ __RPC__in REFIID riid,
            /* [iid_is][out] */ 
            __RPC__deref_out  void **ppvObject);
        
        ULONG ( STDMETHODCALLTYPE *AddRef )( 
            IVsLifetimeControlledObject * This);
        
        ULONG ( STDMETHODCALLTYPE *Release )( 
            IVsLifetimeControlledObject * This);
        
        HRESULT ( STDMETHODCALLTYPE *SeverReferencesToOwner )( 
            IVsLifetimeControlledObject * This);
        
        END_INTERFACE
    } IVsLifetimeControlledObjectVtbl;

    interface IVsLifetimeControlledObject
    {
        CONST_VTBL struct IVsLifetimeControlledObjectVtbl *lpVtbl;
    };

    

#ifdef COBJMACROS


#define IVsLifetimeControlledObject_QueryInterface(This,riid,ppvObject)	\
    ( (This)->lpVtbl -> QueryInterface(This,riid,ppvObject) ) 

#define IVsLifetimeControlledObject_AddRef(This)	\
    ( (This)->lpVtbl -> AddRef(This) ) 

#define IVsLifetimeControlledObject_Release(This)	\
    ( (This)->lpVtbl -> Release(This) ) 


#define IVsLifetimeControlledObject_SeverReferencesToOwner(This)	\
    ( (This)->lpVtbl -> SeverReferencesToOwner(This) ) 

#endif /* COBJMACROS */


#endif 	/* C style interface */




#endif 	/* __IVsLifetimeControlledObject_INTERFACE_DEFINED__ */


#ifndef __IVsLinkedUndoTransactionManager_INTERFACE_DEFINED__
#define __IVsLinkedUndoTransactionManager_INTERFACE_DEFINED__

/* interface IVsLinkedUndoTransactionManager */
/* [object][uuid] */ 


EXTERN_C const IID IID_IVsLinkedUndoTransactionManager;

#if defined(__cplusplus) && !defined(CINTERFACE)
    
    MIDL_INTERFACE("F65478CC-96F1-4BA9-9EF9-A575ACB96031")
    IVsLinkedUndoTransactionManager : public IUnknown
    {
    public:
        virtual HRESULT STDMETHODCALLTYPE OpenLinkedUndo( 
            /* [in] */ DWORD dwFlags,
            /* [in] */ __RPC__in const WCHAR *pszDescription) = 0;
        
        virtual HRESULT STDMETHODCALLTYPE CloseLinkedUndo( void) = 0;
        
        virtual HRESULT STDMETHODCALLTYPE AbortLinkedUndo( void) = 0;
        
        virtual HRESULT STDMETHODCALLTYPE IsAborted( 
            __RPC__in BOOL *pfAborted) = 0;
        
        virtual HRESULT STDMETHODCALLTYPE IsStrict( 
            __RPC__in BOOL *pfStrict) = 0;
        
        virtual HRESULT STDMETHODCALLTYPE CountOpenTransactions( 
            __RPC__in long *pCount) = 0;
        
    };
    
#else 	/* C style interface */

    typedef struct IVsLinkedUndoTransactionManagerVtbl
    {
        BEGIN_INTERFACE
        
        HRESULT ( STDMETHODCALLTYPE *QueryInterface )( 
            IVsLinkedUndoTransactionManager * This,
            /* [in] */ __RPC__in REFIID riid,
            /* [iid_is][out] */ 
            __RPC__deref_out  void **ppvObject);
        
        ULONG ( STDMETHODCALLTYPE *AddRef )( 
            IVsLinkedUndoTransactionManager * This);
        
        ULONG ( STDMETHODCALLTYPE *Release )( 
            IVsLinkedUndoTransactionManager * This);
        
        HRESULT ( STDMETHODCALLTYPE *OpenLinkedUndo )( 
            IVsLinkedUndoTransactionManager * This,
            /* [in] */ DWORD dwFlags,
            /* [in] */ __RPC__in const WCHAR *pszDescription);
        
        HRESULT ( STDMETHODCALLTYPE *CloseLinkedUndo )( 
            IVsLinkedUndoTransactionManager * This);
        
        HRESULT ( STDMETHODCALLTYPE *AbortLinkedUndo )( 
            IVsLinkedUndoTransactionManager * This);
        
        HRESULT ( STDMETHODCALLTYPE *IsAborted )( 
            IVsLinkedUndoTransactionManager * This,
            __RPC__in BOOL *pfAborted);
        
        HRESULT ( STDMETHODCALLTYPE *IsStrict )( 
            IVsLinkedUndoTransactionManager * This,
            __RPC__in BOOL *pfStrict);
        
        HRESULT ( STDMETHODCALLTYPE *CountOpenTransactions )( 
            IVsLinkedUndoTransactionManager * This,
            __RPC__in long *pCount);
        
        END_INTERFACE
    } IVsLinkedUndoTransactionManagerVtbl;

    interface IVsLinkedUndoTransactionManager
    {
        CONST_VTBL struct IVsLinkedUndoTransactionManagerVtbl *lpVtbl;
    };

    

#ifdef COBJMACROS


#define IVsLinkedUndoTransactionManager_QueryInterface(This,riid,ppvObject)	\
    ( (This)->lpVtbl -> QueryInterface(This,riid,ppvObject) ) 

#define IVsLinkedUndoTransactionManager_AddRef(This)	\
    ( (This)->lpVtbl -> AddRef(This) ) 

#define IVsLinkedUndoTransactionManager_Release(This)	\
    ( (This)->lpVtbl -> Release(This) ) 


#define IVsLinkedUndoTransactionManager_OpenLinkedUndo(This,dwFlags,pszDescription)	\
    ( (This)->lpVtbl -> OpenLinkedUndo(This,dwFlags,pszDescription) ) 

#define IVsLinkedUndoTransactionManager_CloseLinkedUndo(This)	\
    ( (This)->lpVtbl -> CloseLinkedUndo(This) ) 

#define IVsLinkedUndoTransactionManager_AbortLinkedUndo(This)	\
    ( (This)->lpVtbl -> AbortLinkedUndo(This) ) 

#define IVsLinkedUndoTransactionManager_IsAborted(This,pfAborted)	\
    ( (This)->lpVtbl -> IsAborted(This,pfAborted) ) 

#define IVsLinkedUndoTransactionManager_IsStrict(This,pfStrict)	\
    ( (This)->lpVtbl -> IsStrict(This,pfStrict) ) 

#define IVsLinkedUndoTransactionManager_CountOpenTransactions(This,pCount)	\
    ( (This)->lpVtbl -> CountOpenTransactions(This,pCount) ) 

#endif /* COBJMACROS */


#endif 	/* C style interface */




#endif 	/* __IVsLinkedUndoTransactionManager_INTERFACE_DEFINED__ */


#ifndef __IVsChangeClusterEvents_INTERFACE_DEFINED__
#define __IVsChangeClusterEvents_INTERFACE_DEFINED__

/* interface IVsChangeClusterEvents */
/* [object][uuid] */ 


EXTERN_C const IID IID_IVsChangeClusterEvents;

#if defined(__cplusplus) && !defined(CINTERFACE)
    
    MIDL_INTERFACE("E55C4E80-A01C-47E8-9E94-D664B94DF6CF")
    IVsChangeClusterEvents : public IUnknown
    {
    public:
        virtual void STDMETHODCALLTYPE OnChangeClusterOpening( 
            /* [in] */ DWORD dwFlags) = 0;
        
        virtual void STDMETHODCALLTYPE OnChangeClusterClosing( 
            /* [in] */ DWORD dwFlags) = 0;
        
    };
    
#else 	/* C style interface */

    typedef struct IVsChangeClusterEventsVtbl
    {
        BEGIN_INTERFACE
        
        HRESULT ( STDMETHODCALLTYPE *QueryInterface )( 
            IVsChangeClusterEvents * This,
            /* [in] */ __RPC__in REFIID riid,
            /* [iid_is][out] */ 
            __RPC__deref_out  void **ppvObject);
        
        ULONG ( STDMETHODCALLTYPE *AddRef )( 
            IVsChangeClusterEvents * This);
        
        ULONG ( STDMETHODCALLTYPE *Release )( 
            IVsChangeClusterEvents * This);
        
        void ( STDMETHODCALLTYPE *OnChangeClusterOpening )( 
            IVsChangeClusterEvents * This,
            /* [in] */ DWORD dwFlags);
        
        void ( STDMETHODCALLTYPE *OnChangeClusterClosing )( 
            IVsChangeClusterEvents * This,
            /* [in] */ DWORD dwFlags);
        
        END_INTERFACE
    } IVsChangeClusterEventsVtbl;

    interface IVsChangeClusterEvents
    {
        CONST_VTBL struct IVsChangeClusterEventsVtbl *lpVtbl;
    };

    

#ifdef COBJMACROS


#define IVsChangeClusterEvents_QueryInterface(This,riid,ppvObject)	\
    ( (This)->lpVtbl -> QueryInterface(This,riid,ppvObject) ) 

#define IVsChangeClusterEvents_AddRef(This)	\
    ( (This)->lpVtbl -> AddRef(This) ) 

#define IVsChangeClusterEvents_Release(This)	\
    ( (This)->lpVtbl -> Release(This) ) 


#define IVsChangeClusterEvents_OnChangeClusterOpening(This,dwFlags)	\
    ( (This)->lpVtbl -> OnChangeClusterOpening(This,dwFlags) ) 

#define IVsChangeClusterEvents_OnChangeClusterClosing(This,dwFlags)	\
    ( (This)->lpVtbl -> OnChangeClusterClosing(This,dwFlags) ) 

#endif /* COBJMACROS */


#endif 	/* C style interface */




#endif 	/* __IVsChangeClusterEvents_INTERFACE_DEFINED__ */


EXTERN_C const CLSID CLSID_VsLocalUserData;

#ifdef __cplusplus

class DECLSPEC_UUID("5130C7AB-85BB-11d3-B363-00C04F611E41")
VsLocalUserData;
#endif

EXTERN_C const CLSID CLSID_VsUserData;

#ifdef __cplusplus

class DECLSPEC_UUID("F5E7E718-1401-11d1-883B-0000F87579D2")
VsUserData;
#endif

EXTERN_C const CLSID CLSID_VsCodeWindow;

#ifdef __cplusplus

class DECLSPEC_UUID("F5E7E719-1401-11d1-883B-0000F87579D2")
VsCodeWindow;
#endif

EXTERN_C const CLSID CLSID_VsSplitRoot;

#ifdef __cplusplus

class DECLSPEC_UUID("F5E7E71A-1401-11d1-883B-0000F87579D2")
VsSplitRoot;
#endif

EXTERN_C const CLSID CLSID_VsDropdownBar;

#ifdef __cplusplus

class DECLSPEC_UUID("452807CC-9BFA-11d0-8C3F-00C04FC2AAE2")
VsDropdownBar;
#endif

EXTERN_C const CLSID CLSID_VsTextBuffer;

#ifdef __cplusplus

class DECLSPEC_UUID("8E7B96A8-E33D-11d0-A6D5-00C04FB67F6A")
VsTextBuffer;
#endif

EXTERN_C const CLSID CLSID_VsTextLineStorage;

#ifdef __cplusplus

class DECLSPEC_UUID("D6513A91-ACC8-11d2-8919-0000F87579D2")
VsTextLineStorage;
#endif

EXTERN_C const CLSID CLSID_VsTextManager;

#ifdef __cplusplus

class DECLSPEC_UUID("F5E7E71D-1401-11d1-883B-0000F87579D2")
VsTextManager;
#endif

EXTERN_C const CLSID CLSID_VsTextView;

#ifdef __cplusplus

class DECLSPEC_UUID("F5E7E71E-1401-11d1-883B-0000F87579D2")
VsTextView;
#endif

EXTERN_C const CLSID CLSID_VsTextPackage;

#ifdef __cplusplus

class DECLSPEC_UUID("F5E7E720-1401-11d1-883B-0000F87579D2")
VsTextPackage;
#endif

EXTERN_C const CLSID CLSID_VsUndoPackage;

#ifdef __cplusplus

class DECLSPEC_UUID("1D76B2E0-F11B-11d2-AFC3-00105A9991EF")
VsUndoPackage;
#endif

EXTERN_C const CLSID CLSID_VsUndoUnit;

#ifdef __cplusplus

class DECLSPEC_UUID("9ADF33D9-8AAD-11d0-B606-00A0C922E851")
VsUndoUnit;
#endif

EXTERN_C const CLSID CLSID_VsMethodTipWindow;

#ifdef __cplusplus

class DECLSPEC_UUID("261A5572-C649-11d0-A8DF-00A0C921A4D2")
VsMethodTipWindow;
#endif

EXTERN_C const CLSID CLSID_VsTextTipWindow;

#ifdef __cplusplus

class DECLSPEC_UUID("05DD7650-130A-11d3-AFCB-00105A9991EF")
VsTextTipWindow;
#endif

EXTERN_C const CLSID CLSID_VsHiddenTextLayer;

#ifdef __cplusplus

class DECLSPEC_UUID("9B6AB22E-916A-11d2-8914-0000F87579D2")
VsHiddenTextLayer;
#endif

EXTERN_C const CLSID CLSID_VsDefaultButtonBarImages;

#ifdef __cplusplus

class DECLSPEC_UUID("B2715971-D02C-11d2-AFB4-00105A9991EF")
VsDefaultButtonBarImages;
#endif

EXTERN_C const CLSID CLSID_VsSyntheticTextLayer;

#ifdef __cplusplus

class DECLSPEC_UUID("42469677-FCFF-11d2-AFC4-00105A9991EF")
VsSyntheticTextLayer;
#endif

EXTERN_C const CLSID CLSID_VsDiffLayer;

#ifdef __cplusplus

class DECLSPEC_UUID("42469676-FCFF-11d2-AFC4-00105A9991EF")
VsDiffLayer;
#endif

#ifndef __IVsDynamicTabProvider_INTERFACE_DEFINED__
#define __IVsDynamicTabProvider_INTERFACE_DEFINED__

/* interface IVsDynamicTabProvider */
/* [object][uuid] */ 


EXTERN_C const IID IID_IVsDynamicTabProvider;

#if defined(__cplusplus) && !defined(CINTERFACE)
    
    MIDL_INTERFACE("E82466CA-CF0E-4653-9E76-4D4C6C548DA2")
    IVsDynamicTabProvider : public IUnknown
    {
    public:
        virtual HRESULT STDMETHODCALLTYPE GetTabStopElements( 
            /* [in] */ __RPC__in_opt IVsTextLines *pBuffer,
            /* [in] */ long iLine,
            /* [out] */ __RPC__out UINT *cEl) = 0;
        
        virtual HRESULT STDMETHODCALLTYPE GetTabStop( 
            /* [in] */ __RPC__in_opt IVsTextLines *pBuffer,
            /* [in] */ long iLine,
            /* [out] */ __RPC__out struct TabStop *sTabStop) = 0;
        
    };
    
#else 	/* C style interface */

    typedef struct IVsDynamicTabProviderVtbl
    {
        BEGIN_INTERFACE
        
        HRESULT ( STDMETHODCALLTYPE *QueryInterface )( 
            IVsDynamicTabProvider * This,
            /* [in] */ __RPC__in REFIID riid,
            /* [iid_is][out] */ 
            __RPC__deref_out  void **ppvObject);
        
        ULONG ( STDMETHODCALLTYPE *AddRef )( 
            IVsDynamicTabProvider * This);
        
        ULONG ( STDMETHODCALLTYPE *Release )( 
            IVsDynamicTabProvider * This);
        
        HRESULT ( STDMETHODCALLTYPE *GetTabStopElements )( 
            IVsDynamicTabProvider * This,
            /* [in] */ __RPC__in_opt IVsTextLines *pBuffer,
            /* [in] */ long iLine,
            /* [out] */ __RPC__out UINT *cEl);
        
        HRESULT ( STDMETHODCALLTYPE *GetTabStop )( 
            IVsDynamicTabProvider * This,
            /* [in] */ __RPC__in_opt IVsTextLines *pBuffer,
            /* [in] */ long iLine,
            /* [out] */ __RPC__out struct TabStop *sTabStop);
        
        END_INTERFACE
    } IVsDynamicTabProviderVtbl;

    interface IVsDynamicTabProvider
    {
        CONST_VTBL struct IVsDynamicTabProviderVtbl *lpVtbl;
    };

    

#ifdef COBJMACROS


#define IVsDynamicTabProvider_QueryInterface(This,riid,ppvObject)	\
    ( (This)->lpVtbl -> QueryInterface(This,riid,ppvObject) ) 

#define IVsDynamicTabProvider_AddRef(This)	\
    ( (This)->lpVtbl -> AddRef(This) ) 

#define IVsDynamicTabProvider_Release(This)	\
    ( (This)->lpVtbl -> Release(This) ) 


#define IVsDynamicTabProvider_GetTabStopElements(This,pBuffer,iLine,cEl)	\
    ( (This)->lpVtbl -> GetTabStopElements(This,pBuffer,iLine,cEl) ) 

#define IVsDynamicTabProvider_GetTabStop(This,pBuffer,iLine,sTabStop)	\
    ( (This)->lpVtbl -> GetTabStop(This,pBuffer,iLine,sTabStop) ) 

#endif /* COBJMACROS */


#endif 	/* C style interface */




#endif 	/* __IVsDynamicTabProvider_INTERFACE_DEFINED__ */


#ifndef __IVsLastChangeTimeProvider_INTERFACE_DEFINED__
#define __IVsLastChangeTimeProvider_INTERFACE_DEFINED__

/* interface IVsLastChangeTimeProvider */
/* [object][uuid] */ 


EXTERN_C const IID IID_IVsLastChangeTimeProvider;

#if defined(__cplusplus) && !defined(CINTERFACE)
    
    MIDL_INTERFACE("35E88F96-047C-443C-A336-91BB8E2B40EC")
    IVsLastChangeTimeProvider : public IUnknown
    {
    public:
        virtual HRESULT STDMETHODCALLTYPE GetLastChangeTime( 
            /* [out] */ __RPC__out FILETIME *pFileTime) = 0;
        
    };
    
#else 	/* C style interface */

    typedef struct IVsLastChangeTimeProviderVtbl
    {
        BEGIN_INTERFACE
        
        HRESULT ( STDMETHODCALLTYPE *QueryInterface )( 
            IVsLastChangeTimeProvider * This,
            /* [in] */ __RPC__in REFIID riid,
            /* [iid_is][out] */ 
            __RPC__deref_out  void **ppvObject);
        
        ULONG ( STDMETHODCALLTYPE *AddRef )( 
            IVsLastChangeTimeProvider * This);
        
        ULONG ( STDMETHODCALLTYPE *Release )( 
            IVsLastChangeTimeProvider * This);
        
        HRESULT ( STDMETHODCALLTYPE *GetLastChangeTime )( 
            IVsLastChangeTimeProvider * This,
            /* [out] */ __RPC__out FILETIME *pFileTime);
        
        END_INTERFACE
    } IVsLastChangeTimeProviderVtbl;

    interface IVsLastChangeTimeProvider
    {
        CONST_VTBL struct IVsLastChangeTimeProviderVtbl *lpVtbl;
    };

    

#ifdef COBJMACROS


#define IVsLastChangeTimeProvider_QueryInterface(This,riid,ppvObject)	\
    ( (This)->lpVtbl -> QueryInterface(This,riid,ppvObject) ) 

#define IVsLastChangeTimeProvider_AddRef(This)	\
    ( (This)->lpVtbl -> AddRef(This) ) 

#define IVsLastChangeTimeProvider_Release(This)	\
    ( (This)->lpVtbl -> Release(This) ) 


#define IVsLastChangeTimeProvider_GetLastChangeTime(This,pFileTime)	\
    ( (This)->lpVtbl -> GetLastChangeTime(This,pFileTime) ) 

#endif /* COBJMACROS */


#endif 	/* C style interface */




#endif 	/* __IVsLastChangeTimeProvider_INTERFACE_DEFINED__ */


#ifndef __IVsEnumTextSpans_INTERFACE_DEFINED__
#define __IVsEnumTextSpans_INTERFACE_DEFINED__

/* interface IVsEnumTextSpans */
/* [object][uuid] */ 


EXTERN_C const IID IID_IVsEnumTextSpans;

#if defined(__cplusplus) && !defined(CINTERFACE)
    
    MIDL_INTERFACE("0F343A75-968B-439E-81D6-0D066E53D28D")
    IVsEnumTextSpans : public IUnknown
    {
    public:
        virtual HRESULT STDMETHODCALLTYPE Reset( void) = 0;
        
        virtual HRESULT STDMETHODCALLTYPE Next( 
            /* [in] */ ULONG cEl,
            /* [size_is][out] */ __RPC__out_ecount_full(cEl) TextSpan *ppOut,
            /* [out] */ __RPC__out ULONG *pcElFetched) = 0;
        
        virtual HRESULT STDMETHODCALLTYPE GetCount( 
            /* [out] */ __RPC__out ULONG *pcSpans) = 0;
        
    };
    
#else 	/* C style interface */

    typedef struct IVsEnumTextSpansVtbl
    {
        BEGIN_INTERFACE
        
        HRESULT ( STDMETHODCALLTYPE *QueryInterface )( 
            IVsEnumTextSpans * This,
            /* [in] */ __RPC__in REFIID riid,
            /* [iid_is][out] */ 
            __RPC__deref_out  void **ppvObject);
        
        ULONG ( STDMETHODCALLTYPE *AddRef )( 
            IVsEnumTextSpans * This);
        
        ULONG ( STDMETHODCALLTYPE *Release )( 
            IVsEnumTextSpans * This);
        
        HRESULT ( STDMETHODCALLTYPE *Reset )( 
            IVsEnumTextSpans * This);
        
        HRESULT ( STDMETHODCALLTYPE *Next )( 
            IVsEnumTextSpans * This,
            /* [in] */ ULONG cEl,
            /* [size_is][out] */ __RPC__out_ecount_full(cEl) TextSpan *ppOut,
            /* [out] */ __RPC__out ULONG *pcElFetched);
        
        HRESULT ( STDMETHODCALLTYPE *GetCount )( 
            IVsEnumTextSpans * This,
            /* [out] */ __RPC__out ULONG *pcSpans);
        
        END_INTERFACE
    } IVsEnumTextSpansVtbl;

    interface IVsEnumTextSpans
    {
        CONST_VTBL struct IVsEnumTextSpansVtbl *lpVtbl;
    };

    

#ifdef COBJMACROS


#define IVsEnumTextSpans_QueryInterface(This,riid,ppvObject)	\
    ( (This)->lpVtbl -> QueryInterface(This,riid,ppvObject) ) 

#define IVsEnumTextSpans_AddRef(This)	\
    ( (This)->lpVtbl -> AddRef(This) ) 

#define IVsEnumTextSpans_Release(This)	\
    ( (This)->lpVtbl -> Release(This) ) 


#define IVsEnumTextSpans_Reset(This)	\
    ( (This)->lpVtbl -> Reset(This) ) 

#define IVsEnumTextSpans_Next(This,cEl,ppOut,pcElFetched)	\
    ( (This)->lpVtbl -> Next(This,cEl,ppOut,pcElFetched) ) 

#define IVsEnumTextSpans_GetCount(This,pcSpans)	\
    ( (This)->lpVtbl -> GetCount(This,pcSpans) ) 

#endif /* COBJMACROS */


#endif 	/* C style interface */




#endif 	/* __IVsEnumTextSpans_INTERFACE_DEFINED__ */


#ifndef __IVsTextLayer_INTERFACE_DEFINED__
#define __IVsTextLayer_INTERFACE_DEFINED__

/* interface IVsTextLayer */
/* [object][uuid] */ 


EXTERN_C const IID IID_IVsTextLayer;

#if defined(__cplusplus) && !defined(CINTERFACE)
    
    MIDL_INTERFACE("0E145D3F-BEFC-4FD9-8714-B01AE89F4396")
    IVsTextLayer : public IUnknown
    {
    public:
        virtual HRESULT STDMETHODCALLTYPE LocalLineIndexToBase( 
            /* [in] */ long iLocalLine,
            /* [in] */ CharIndex iLocalIndex,
            /* [out] */ __RPC__out long *piBaseLine,
            /* [out] */ __RPC__out CharIndex *piBaseIndex) = 0;
        
        virtual HRESULT STDMETHODCALLTYPE BaseLineIndexToLocal( 
            /* [in] */ long iBaseLine,
            /* [in] */ CharIndex iBaseIndex,
            /* [out] */ __RPC__out long *piLocalLine,
            /* [out] */ __RPC__out CharIndex *piLocalIndex) = 0;
        
        virtual HRESULT STDMETHODCALLTYPE LocalLineIndexToDeeperLayer( 
            /* [in] */ __RPC__in_opt IVsTextLayer *pTargetLayer,
            /* [in] */ long iLocalLine,
            /* [in] */ CharIndex iLocalIndex,
            /* [out] */ __RPC__out long *piTargetLine,
            /* [out] */ __RPC__out CharIndex *piTargetIndex) = 0;
        
        virtual HRESULT STDMETHODCALLTYPE DeeperLayerLineIndexToLocal( 
            /* [in] */ DWORD dwFlags,
            /* [in] */ __RPC__in_opt IVsTextLayer *pTargetLayer,
            /* [in] */ long iLayerLine,
            /* [in] */ CharIndex iLayerIndex,
            /* [out] */ __RPC__out long *piLocalLine,
            /* [out] */ __RPC__out CharIndex *piLocalIndex) = 0;
        
        virtual HRESULT STDMETHODCALLTYPE GetBaseBuffer( 
            /* [out] */ __RPC__deref_out_opt IVsTextLines **ppiBuf) = 0;
        
        virtual HRESULT STDMETHODCALLTYPE LockBufferEx( 
            /* [in] */ DWORD dwFlags) = 0;
        
        virtual HRESULT STDMETHODCALLTYPE UnlockBufferEx( 
            /* [in] */ DWORD dwFlags) = 0;
        
        virtual HRESULT STDMETHODCALLTYPE GetLengthOfLine( 
            /* [in] */ long iLine,
            /* [out] */ __RPC__out long *piLength) = 0;
        
        virtual HRESULT STDMETHODCALLTYPE GetLineCount( 
            /* [out] */ __RPC__out long *piLineCount) = 0;
        
        virtual HRESULT STDMETHODCALLTYPE GetLastLineIndex( 
            /* [out] */ __RPC__out long *piLine,
            /* [out] */ __RPC__out long *piIndex) = 0;
        
        virtual HRESULT STDMETHODCALLTYPE GetMarkerData( 
            /* [in] */ long iTopLine,
            /* [in] */ long iBottomLine,
            /* [out] */ __RPC__out MARKERDATA *pMarkerData) = 0;
        
        virtual HRESULT STDMETHODCALLTYPE ReleaseMarkerData( 
            /* [in] */ __RPC__in MARKERDATA *pMarkerData) = 0;
        
        virtual HRESULT STDMETHODCALLTYPE GetLineDataEx( 
            /* [in] */ DWORD dwFlags,
            /* [in] */ long iLine,
            /* [in] */ long iStartIndex,
            /* [in] */ long iEndIndex,
            /* [out] */ __RPC__out LINEDATAEX *pLineData,
            /* [in] */ __RPC__in MARKERDATA *pMarkerData) = 0;
        
        virtual HRESULT STDMETHODCALLTYPE ReleaseLineDataEx( 
            /* [in] */ __RPC__in LINEDATAEX *pLineData) = 0;
        
        virtual HRESULT STDMETHODCALLTYPE GetLineText( 
            /* [in] */ long iStartLine,
            /* [in] */ CharIndex iStartIndex,
            /* [in] */ long iEndLine,
            /* [in] */ CharIndex iEndIndex,
            /* [out] */ __RPC__deref_out_opt BSTR *pbstrBuf) = 0;
        
        virtual HRESULT STDMETHODCALLTYPE CopyLineText( 
            /* [in] */ long iStartLine,
            /* [in] */ CharIndex iStartIndex,
            /* [in] */ long iEndLine,
            /* [in] */ CharIndex iEndIndex,
            /* [in] */ __RPC__in LPWSTR pszBuf,
            /* [out][in] */ __RPC__inout long *pcchBuf) = 0;
        
        virtual HRESULT STDMETHODCALLTYPE ReplaceLines( 
            /* [in] */ long iStartLine,
            /* [in] */ CharIndex iStartIndex,
            /* [in] */ long iEndLine,
            /* [in] */ CharIndex iEndIndex,
            /* [in] */ __RPC__in LPCWSTR pszText,
            /* [in] */ long iNewLen,
            /* [out] */ __RPC__out TextSpan *pChangedSpan) = 0;
        
        virtual HRESULT STDMETHODCALLTYPE CanReplaceLines( 
            /* [in] */ long iStartLine,
            /* [in] */ CharIndex iStartIndex,
            /* [in] */ long iEndLine,
            /* [in] */ CharIndex iEndIndex,
            /* [in] */ long iNewLen) = 0;
        
        virtual HRESULT STDMETHODCALLTYPE CreateTrackingPoint( 
            /* [in] */ long iLine,
            /* [in] */ CharIndex iIndex,
            /* [out] */ __RPC__deref_out_opt IVsTextTrackingPoint **ppMarker) = 0;
        
        virtual HRESULT STDMETHODCALLTYPE EnumLayerMarkers( 
            /* [in] */ long iStartLine,
            /* [in] */ CharIndex iStartIndex,
            /* [in] */ long iEndLine,
            /* [in] */ CharIndex iEndIndex,
            /* [in] */ long iMarkerType,
            /* [in] */ DWORD dwFlags,
            /* [out] */ __RPC__deref_out_opt IVsEnumLayerMarkers **ppEnum) = 0;
        
        virtual HRESULT STDMETHODCALLTYPE ReplaceLinesEx( 
            /* [in] */ DWORD dwFlags,
            /* [in] */ long iStartLine,
            /* [in] */ CharIndex iStartIndex,
            /* [in] */ long iEndLine,
            /* [in] */ CharIndex iEndIndex,
            /* [in] */ __RPC__in LPCWSTR pszText,
            /* [in] */ long iNewLen,
            /* [out] */ __RPC__out TextSpan *pChangedSpan) = 0;
        
        virtual HRESULT STDMETHODCALLTYPE MapLocalSpansToTextOriginatingLayer( 
            /* [in] */ DWORD dwFlags,
            /* [in] */ __RPC__in_opt IVsEnumTextSpans *pLocalSpanEnum,
            /* [out] */ __RPC__deref_out_opt IVsTextLayer **ppTargetLayer,
            /* [out] */ __RPC__deref_out_opt IVsEnumTextSpans **ppTargetSpanEnum) = 0;
        
    };
    
#else 	/* C style interface */

    typedef struct IVsTextLayerVtbl
    {
        BEGIN_INTERFACE
        
        HRESULT ( STDMETHODCALLTYPE *QueryInterface )( 
            IVsTextLayer * This,
            /* [in] */ __RPC__in REFIID riid,
            /* [iid_is][out] */ 
            __RPC__deref_out  void **ppvObject);
        
        ULONG ( STDMETHODCALLTYPE *AddRef )( 
            IVsTextLayer * This);
        
        ULONG ( STDMETHODCALLTYPE *Release )( 
            IVsTextLayer * This);
        
        HRESULT ( STDMETHODCALLTYPE *LocalLineIndexToBase )( 
            IVsTextLayer * This,
            /* [in] */ long iLocalLine,
            /* [in] */ CharIndex iLocalIndex,
            /* [out] */ __RPC__out long *piBaseLine,
            /* [out] */ __RPC__out CharIndex *piBaseIndex);
        
        HRESULT ( STDMETHODCALLTYPE *BaseLineIndexToLocal )( 
            IVsTextLayer * This,
            /* [in] */ long iBaseLine,
            /* [in] */ CharIndex iBaseIndex,
            /* [out] */ __RPC__out long *piLocalLine,
            /* [out] */ __RPC__out CharIndex *piLocalIndex);
        
        HRESULT ( STDMETHODCALLTYPE *LocalLineIndexToDeeperLayer )( 
            IVsTextLayer * This,
            /* [in] */ __RPC__in_opt IVsTextLayer *pTargetLayer,
            /* [in] */ long iLocalLine,
            /* [in] */ CharIndex iLocalIndex,
            /* [out] */ __RPC__out long *piTargetLine,
            /* [out] */ __RPC__out CharIndex *piTargetIndex);
        
        HRESULT ( STDMETHODCALLTYPE *DeeperLayerLineIndexToLocal )( 
            IVsTextLayer * This,
            /* [in] */ DWORD dwFlags,
            /* [in] */ __RPC__in_opt IVsTextLayer *pTargetLayer,
            /* [in] */ long iLayerLine,
            /* [in] */ CharIndex iLayerIndex,
            /* [out] */ __RPC__out long *piLocalLine,
            /* [out] */ __RPC__out CharIndex *piLocalIndex);
        
        HRESULT ( STDMETHODCALLTYPE *GetBaseBuffer )( 
            IVsTextLayer * This,
            /* [out] */ __RPC__deref_out_opt IVsTextLines **ppiBuf);
        
        HRESULT ( STDMETHODCALLTYPE *LockBufferEx )( 
            IVsTextLayer * This,
            /* [in] */ DWORD dwFlags);
        
        HRESULT ( STDMETHODCALLTYPE *UnlockBufferEx )( 
            IVsTextLayer * This,
            /* [in] */ DWORD dwFlags);
        
        HRESULT ( STDMETHODCALLTYPE *GetLengthOfLine )( 
            IVsTextLayer * This,
            /* [in] */ long iLine,
            /* [out] */ __RPC__out long *piLength);
        
        HRESULT ( STDMETHODCALLTYPE *GetLineCount )( 
            IVsTextLayer * This,
            /* [out] */ __RPC__out long *piLineCount);
        
        HRESULT ( STDMETHODCALLTYPE *GetLastLineIndex )( 
            IVsTextLayer * This,
            /* [out] */ __RPC__out long *piLine,
            /* [out] */ __RPC__out long *piIndex);
        
        HRESULT ( STDMETHODCALLTYPE *GetMarkerData )( 
            IVsTextLayer * This,
            /* [in] */ long iTopLine,
            /* [in] */ long iBottomLine,
            /* [out] */ __RPC__out MARKERDATA *pMarkerData);
        
        HRESULT ( STDMETHODCALLTYPE *ReleaseMarkerData )( 
            IVsTextLayer * This,
            /* [in] */ __RPC__in MARKERDATA *pMarkerData);
        
        HRESULT ( STDMETHODCALLTYPE *GetLineDataEx )( 
            IVsTextLayer * This,
            /* [in] */ DWORD dwFlags,
            /* [in] */ long iLine,
            /* [in] */ long iStartIndex,
            /* [in] */ long iEndIndex,
            /* [out] */ __RPC__out LINEDATAEX *pLineData,
            /* [in] */ __RPC__in MARKERDATA *pMarkerData);
        
        HRESULT ( STDMETHODCALLTYPE *ReleaseLineDataEx )( 
            IVsTextLayer * This,
            /* [in] */ __RPC__in LINEDATAEX *pLineData);
        
        HRESULT ( STDMETHODCALLTYPE *GetLineText )( 
            IVsTextLayer * This,
            /* [in] */ long iStartLine,
            /* [in] */ CharIndex iStartIndex,
            /* [in] */ long iEndLine,
            /* [in] */ CharIndex iEndIndex,
            /* [out] */ __RPC__deref_out_opt BSTR *pbstrBuf);
        
        HRESULT ( STDMETHODCALLTYPE *CopyLineText )( 
            IVsTextLayer * This,
            /* [in] */ long iStartLine,
            /* [in] */ CharIndex iStartIndex,
            /* [in] */ long iEndLine,
            /* [in] */ CharIndex iEndIndex,
            /* [in] */ __RPC__in LPWSTR pszBuf,
            /* [out][in] */ __RPC__inout long *pcchBuf);
        
        HRESULT ( STDMETHODCALLTYPE *ReplaceLines )( 
            IVsTextLayer * This,
            /* [in] */ long iStartLine,
            /* [in] */ CharIndex iStartIndex,
            /* [in] */ long iEndLine,
            /* [in] */ CharIndex iEndIndex,
            /* [in] */ __RPC__in LPCWSTR pszText,
            /* [in] */ long iNewLen,
            /* [out] */ __RPC__out TextSpan *pChangedSpan);
        
        HRESULT ( STDMETHODCALLTYPE *CanReplaceLines )( 
            IVsTextLayer * This,
            /* [in] */ long iStartLine,
            /* [in] */ CharIndex iStartIndex,
            /* [in] */ long iEndLine,
            /* [in] */ CharIndex iEndIndex,
            /* [in] */ long iNewLen);
        
        HRESULT ( STDMETHODCALLTYPE *CreateTrackingPoint )( 
            IVsTextLayer * This,
            /* [in] */ long iLine,
            /* [in] */ CharIndex iIndex,
            /* [out] */ __RPC__deref_out_opt IVsTextTrackingPoint **ppMarker);
        
        HRESULT ( STDMETHODCALLTYPE *EnumLayerMarkers )( 
            IVsTextLayer * This,
            /* [in] */ long iStartLine,
            /* [in] */ CharIndex iStartIndex,
            /* [in] */ long iEndLine,
            /* [in] */ CharIndex iEndIndex,
            /* [in] */ long iMarkerType,
            /* [in] */ DWORD dwFlags,
            /* [out] */ __RPC__deref_out_opt IVsEnumLayerMarkers **ppEnum);
        
        HRESULT ( STDMETHODCALLTYPE *ReplaceLinesEx )( 
            IVsTextLayer * This,
            /* [in] */ DWORD dwFlags,
            /* [in] */ long iStartLine,
            /* [in] */ CharIndex iStartIndex,
            /* [in] */ long iEndLine,
            /* [in] */ CharIndex iEndIndex,
            /* [in] */ __RPC__in LPCWSTR pszText,
            /* [in] */ long iNewLen,
            /* [out] */ __RPC__out TextSpan *pChangedSpan);
        
        HRESULT ( STDMETHODCALLTYPE *MapLocalSpansToTextOriginatingLayer )( 
            IVsTextLayer * This,
            /* [in] */ DWORD dwFlags,
            /* [in] */ __RPC__in_opt IVsEnumTextSpans *pLocalSpanEnum,
            /* [out] */ __RPC__deref_out_opt IVsTextLayer **ppTargetLayer,
            /* [out] */ __RPC__deref_out_opt IVsEnumTextSpans **ppTargetSpanEnum);
        
        END_INTERFACE
    } IVsTextLayerVtbl;

    interface IVsTextLayer
    {
        CONST_VTBL struct IVsTextLayerVtbl *lpVtbl;
    };

    

#ifdef COBJMACROS


#define IVsTextLayer_QueryInterface(This,riid,ppvObject)	\
    ( (This)->lpVtbl -> QueryInterface(This,riid,ppvObject) ) 

#define IVsTextLayer_AddRef(This)	\
    ( (This)->lpVtbl -> AddRef(This) ) 

#define IVsTextLayer_Release(This)	\
    ( (This)->lpVtbl -> Release(This) ) 


#define IVsTextLayer_LocalLineIndexToBase(This,iLocalLine,iLocalIndex,piBaseLine,piBaseIndex)	\
    ( (This)->lpVtbl -> LocalLineIndexToBase(This,iLocalLine,iLocalIndex,piBaseLine,piBaseIndex) ) 

#define IVsTextLayer_BaseLineIndexToLocal(This,iBaseLine,iBaseIndex,piLocalLine,piLocalIndex)	\
    ( (This)->lpVtbl -> BaseLineIndexToLocal(This,iBaseLine,iBaseIndex,piLocalLine,piLocalIndex) ) 

#define IVsTextLayer_LocalLineIndexToDeeperLayer(This,pTargetLayer,iLocalLine,iLocalIndex,piTargetLine,piTargetIndex)	\
    ( (This)->lpVtbl -> LocalLineIndexToDeeperLayer(This,pTargetLayer,iLocalLine,iLocalIndex,piTargetLine,piTargetIndex) ) 

#define IVsTextLayer_DeeperLayerLineIndexToLocal(This,dwFlags,pTargetLayer,iLayerLine,iLayerIndex,piLocalLine,piLocalIndex)	\
    ( (This)->lpVtbl -> DeeperLayerLineIndexToLocal(This,dwFlags,pTargetLayer,iLayerLine,iLayerIndex,piLocalLine,piLocalIndex) ) 

#define IVsTextLayer_GetBaseBuffer(This,ppiBuf)	\
    ( (This)->lpVtbl -> GetBaseBuffer(This,ppiBuf) ) 

#define IVsTextLayer_LockBufferEx(This,dwFlags)	\
    ( (This)->lpVtbl -> LockBufferEx(This,dwFlags) ) 

#define IVsTextLayer_UnlockBufferEx(This,dwFlags)	\
    ( (This)->lpVtbl -> UnlockBufferEx(This,dwFlags) ) 

#define IVsTextLayer_GetLengthOfLine(This,iLine,piLength)	\
    ( (This)->lpVtbl -> GetLengthOfLine(This,iLine,piLength) ) 

#define IVsTextLayer_GetLineCount(This,piLineCount)	\
    ( (This)->lpVtbl -> GetLineCount(This,piLineCount) ) 

#define IVsTextLayer_GetLastLineIndex(This,piLine,piIndex)	\
    ( (This)->lpVtbl -> GetLastLineIndex(This,piLine,piIndex) ) 

#define IVsTextLayer_GetMarkerData(This,iTopLine,iBottomLine,pMarkerData)	\
    ( (This)->lpVtbl -> GetMarkerData(This,iTopLine,iBottomLine,pMarkerData) ) 

#define IVsTextLayer_ReleaseMarkerData(This,pMarkerData)	\
    ( (This)->lpVtbl -> ReleaseMarkerData(This,pMarkerData) ) 

#define IVsTextLayer_GetLineDataEx(This,dwFlags,iLine,iStartIndex,iEndIndex,pLineData,pMarkerData)	\
    ( (This)->lpVtbl -> GetLineDataEx(This,dwFlags,iLine,iStartIndex,iEndIndex,pLineData,pMarkerData) ) 

#define IVsTextLayer_ReleaseLineDataEx(This,pLineData)	\
    ( (This)->lpVtbl -> ReleaseLineDataEx(This,pLineData) ) 

#define IVsTextLayer_GetLineText(This,iStartLine,iStartIndex,iEndLine,iEndIndex,pbstrBuf)	\
    ( (This)->lpVtbl -> GetLineText(This,iStartLine,iStartIndex,iEndLine,iEndIndex,pbstrBuf) ) 

#define IVsTextLayer_CopyLineText(This,iStartLine,iStartIndex,iEndLine,iEndIndex,pszBuf,pcchBuf)	\
    ( (This)->lpVtbl -> CopyLineText(This,iStartLine,iStartIndex,iEndLine,iEndIndex,pszBuf,pcchBuf) ) 

#define IVsTextLayer_ReplaceLines(This,iStartLine,iStartIndex,iEndLine,iEndIndex,pszText,iNewLen,pChangedSpan)	\
    ( (This)->lpVtbl -> ReplaceLines(This,iStartLine,iStartIndex,iEndLine,iEndIndex,pszText,iNewLen,pChangedSpan) ) 

#define IVsTextLayer_CanReplaceLines(This,iStartLine,iStartIndex,iEndLine,iEndIndex,iNewLen)	\
    ( (This)->lpVtbl -> CanReplaceLines(This,iStartLine,iStartIndex,iEndLine,iEndIndex,iNewLen) ) 

#define IVsTextLayer_CreateTrackingPoint(This,iLine,iIndex,ppMarker)	\
    ( (This)->lpVtbl -> CreateTrackingPoint(This,iLine,iIndex,ppMarker) ) 

#define IVsTextLayer_EnumLayerMarkers(This,iStartLine,iStartIndex,iEndLine,iEndIndex,iMarkerType,dwFlags,ppEnum)	\
    ( (This)->lpVtbl -> EnumLayerMarkers(This,iStartLine,iStartIndex,iEndLine,iEndIndex,iMarkerType,dwFlags,ppEnum) ) 

#define IVsTextLayer_ReplaceLinesEx(This,dwFlags,iStartLine,iStartIndex,iEndLine,iEndIndex,pszText,iNewLen,pChangedSpan)	\
    ( (This)->lpVtbl -> ReplaceLinesEx(This,dwFlags,iStartLine,iStartIndex,iEndLine,iEndIndex,pszText,iNewLen,pChangedSpan) ) 

#define IVsTextLayer_MapLocalSpansToTextOriginatingLayer(This,dwFlags,pLocalSpanEnum,ppTargetLayer,ppTargetSpanEnum)	\
    ( (This)->lpVtbl -> MapLocalSpansToTextOriginatingLayer(This,dwFlags,pLocalSpanEnum,ppTargetLayer,ppTargetSpanEnum) ) 

#endif /* COBJMACROS */


#endif 	/* C style interface */




#endif 	/* __IVsTextLayer_INTERFACE_DEFINED__ */


#ifndef __IVsSyntheticRegion_INTERFACE_DEFINED__
#define __IVsSyntheticRegion_INTERFACE_DEFINED__

/* interface IVsSyntheticRegion */
/* [object][uuid] */ 


EXTERN_C const IID IID_IVsSyntheticRegion;

#if defined(__cplusplus) && !defined(CINTERFACE)
    
    MIDL_INTERFACE("B9C97AB2-2A37-4134-93E9-C3014F364AA8")
    IVsSyntheticRegion : public IUnknown
    {
    public:
        virtual HRESULT STDMETHODCALLTYPE GetMarkerType( 
            /* [out] */ __RPC__out long *piMarkerType) = 0;
        
        virtual HRESULT STDMETHODCALLTYPE GetBehavior( 
            /* [out] */ __RPC__out DWORD *pdwBehavior) = 0;
        
        virtual HRESULT STDMETHODCALLTYPE GetText( 
            /* [out] */ __RPC__deref_out_opt BSTR *pbstrText) = 0;
        
        virtual HRESULT STDMETHODCALLTYPE SetText( 
            /* [in] */ __RPC__in LPCWSTR pszText) = 0;
        
        virtual HRESULT STDMETHODCALLTYPE GetBaseBufferAnchor( 
            /* [out] */ __RPC__out long *piAnchorLine,
            /* [out] */ __RPC__out long *piAnchorIndex) = 0;
        
        virtual HRESULT STDMETHODCALLTYPE SetBaseBufferAnchor( 
            /* [in] */ long iAnchorLine,
            /* [in] */ long iAnchorIndex) = 0;
        
        virtual HRESULT STDMETHODCALLTYPE GetClientData( 
            /* [out] */ __RPC__out DWORD_PTR *pdwData) = 0;
        
        virtual HRESULT STDMETHODCALLTYPE SetClientData( 
            /* [in] */ DWORD_PTR dwData) = 0;
        
        virtual HRESULT STDMETHODCALLTYPE Invalidate( 
            /* [in] */ DWORD dwUpdate) = 0;
        
        virtual HRESULT STDMETHODCALLTYPE IsValid( 
            /* [out] */ __RPC__out BOOL *pfValid) = 0;
        
        virtual HRESULT STDMETHODCALLTYPE GetTextLayer( 
            /* [out] */ __RPC__deref_out_opt IVsTextLayer **ppLayer) = 0;
        
        virtual HRESULT STDMETHODCALLTYPE GetSyntheticLayerSpan( 
            /* [out] */ __RPC__out TextSpan *pSpan) = 0;
        
        virtual HRESULT STDMETHODCALLTYPE GetTextImage( 
            /* [out] */ __RPC__deref_out_opt IVsTextImage **ppImage) = 0;
        
    };
    
#else 	/* C style interface */

    typedef struct IVsSyntheticRegionVtbl
    {
        BEGIN_INTERFACE
        
        HRESULT ( STDMETHODCALLTYPE *QueryInterface )( 
            IVsSyntheticRegion * This,
            /* [in] */ __RPC__in REFIID riid,
            /* [iid_is][out] */ 
            __RPC__deref_out  void **ppvObject);
        
        ULONG ( STDMETHODCALLTYPE *AddRef )( 
            IVsSyntheticRegion * This);
        
        ULONG ( STDMETHODCALLTYPE *Release )( 
            IVsSyntheticRegion * This);
        
        HRESULT ( STDMETHODCALLTYPE *GetMarkerType )( 
            IVsSyntheticRegion * This,
            /* [out] */ __RPC__out long *piMarkerType);
        
        HRESULT ( STDMETHODCALLTYPE *GetBehavior )( 
            IVsSyntheticRegion * This,
            /* [out] */ __RPC__out DWORD *pdwBehavior);
        
        HRESULT ( STDMETHODCALLTYPE *GetText )( 
            IVsSyntheticRegion * This,
            /* [out] */ __RPC__deref_out_opt BSTR *pbstrText);
        
        HRESULT ( STDMETHODCALLTYPE *SetText )( 
            IVsSyntheticRegion * This,
            /* [in] */ __RPC__in LPCWSTR pszText);
        
        HRESULT ( STDMETHODCALLTYPE *GetBaseBufferAnchor )( 
            IVsSyntheticRegion * This,
            /* [out] */ __RPC__out long *piAnchorLine,
            /* [out] */ __RPC__out long *piAnchorIndex);
        
        HRESULT ( STDMETHODCALLTYPE *SetBaseBufferAnchor )( 
            IVsSyntheticRegion * This,
            /* [in] */ long iAnchorLine,
            /* [in] */ long iAnchorIndex);
        
        HRESULT ( STDMETHODCALLTYPE *GetClientData )( 
            IVsSyntheticRegion * This,
            /* [out] */ __RPC__out DWORD_PTR *pdwData);
        
        HRESULT ( STDMETHODCALLTYPE *SetClientData )( 
            IVsSyntheticRegion * This,
            /* [in] */ DWORD_PTR dwData);
        
        HRESULT ( STDMETHODCALLTYPE *Invalidate )( 
            IVsSyntheticRegion * This,
            /* [in] */ DWORD dwUpdate);
        
        HRESULT ( STDMETHODCALLTYPE *IsValid )( 
            IVsSyntheticRegion * This,
            /* [out] */ __RPC__out BOOL *pfValid);
        
        HRESULT ( STDMETHODCALLTYPE *GetTextLayer )( 
            IVsSyntheticRegion * This,
            /* [out] */ __RPC__deref_out_opt IVsTextLayer **ppLayer);
        
        HRESULT ( STDMETHODCALLTYPE *GetSyntheticLayerSpan )( 
            IVsSyntheticRegion * This,
            /* [out] */ __RPC__out TextSpan *pSpan);
        
        HRESULT ( STDMETHODCALLTYPE *GetTextImage )( 
            IVsSyntheticRegion * This,
            /* [out] */ __RPC__deref_out_opt IVsTextImage **ppImage);
        
        END_INTERFACE
    } IVsSyntheticRegionVtbl;

    interface IVsSyntheticRegion
    {
        CONST_VTBL struct IVsSyntheticRegionVtbl *lpVtbl;
    };

    

#ifdef COBJMACROS


#define IVsSyntheticRegion_QueryInterface(This,riid,ppvObject)	\
    ( (This)->lpVtbl -> QueryInterface(This,riid,ppvObject) ) 

#define IVsSyntheticRegion_AddRef(This)	\
    ( (This)->lpVtbl -> AddRef(This) ) 

#define IVsSyntheticRegion_Release(This)	\
    ( (This)->lpVtbl -> Release(This) ) 


#define IVsSyntheticRegion_GetMarkerType(This,piMarkerType)	\
    ( (This)->lpVtbl -> GetMarkerType(This,piMarkerType) ) 

#define IVsSyntheticRegion_GetBehavior(This,pdwBehavior)	\
    ( (This)->lpVtbl -> GetBehavior(This,pdwBehavior) ) 

#define IVsSyntheticRegion_GetText(This,pbstrText)	\
    ( (This)->lpVtbl -> GetText(This,pbstrText) ) 

#define IVsSyntheticRegion_SetText(This,pszText)	\
    ( (This)->lpVtbl -> SetText(This,pszText) ) 

#define IVsSyntheticRegion_GetBaseBufferAnchor(This,piAnchorLine,piAnchorIndex)	\
    ( (This)->lpVtbl -> GetBaseBufferAnchor(This,piAnchorLine,piAnchorIndex) ) 

#define IVsSyntheticRegion_SetBaseBufferAnchor(This,iAnchorLine,iAnchorIndex)	\
    ( (This)->lpVtbl -> SetBaseBufferAnchor(This,iAnchorLine,iAnchorIndex) ) 

#define IVsSyntheticRegion_GetClientData(This,pdwData)	\
    ( (This)->lpVtbl -> GetClientData(This,pdwData) ) 

#define IVsSyntheticRegion_SetClientData(This,dwData)	\
    ( (This)->lpVtbl -> SetClientData(This,dwData) ) 

#define IVsSyntheticRegion_Invalidate(This,dwUpdate)	\
    ( (This)->lpVtbl -> Invalidate(This,dwUpdate) ) 

#define IVsSyntheticRegion_IsValid(This,pfValid)	\
    ( (This)->lpVtbl -> IsValid(This,pfValid) ) 

#define IVsSyntheticRegion_GetTextLayer(This,ppLayer)	\
    ( (This)->lpVtbl -> GetTextLayer(This,ppLayer) ) 

#define IVsSyntheticRegion_GetSyntheticLayerSpan(This,pSpan)	\
    ( (This)->lpVtbl -> GetSyntheticLayerSpan(This,pSpan) ) 

#define IVsSyntheticRegion_GetTextImage(This,ppImage)	\
    ( (This)->lpVtbl -> GetTextImage(This,ppImage) ) 

#endif /* COBJMACROS */


#endif 	/* C style interface */




#endif 	/* __IVsSyntheticRegion_INTERFACE_DEFINED__ */


#ifndef __IVsEnumSyntheticRegions_INTERFACE_DEFINED__
#define __IVsEnumSyntheticRegions_INTERFACE_DEFINED__

/* interface IVsEnumSyntheticRegions */
/* [object][uuid] */ 


EXTERN_C const IID IID_IVsEnumSyntheticRegions;

#if defined(__cplusplus) && !defined(CINTERFACE)
    
    MIDL_INTERFACE("67D9795E-CEA4-45FF-A3DA-055CF78F139A")
    IVsEnumSyntheticRegions : public IUnknown
    {
    public:
        virtual HRESULT STDMETHODCALLTYPE Reset( void) = 0;
        
        virtual HRESULT STDMETHODCALLTYPE Next( 
            /* [in] */ ULONG cEl,
            /* [size_is][out] */ __RPC__out_ecount_full(cEl) IVsSyntheticRegion **ppOut,
            /* [out] */ __RPC__out ULONG *pcElFetched) = 0;
        
        virtual HRESULT STDMETHODCALLTYPE GetCount( 
            /* [out] */ __RPC__out ULONG *pcRegions) = 0;
        
    };
    
#else 	/* C style interface */

    typedef struct IVsEnumSyntheticRegionsVtbl
    {
        BEGIN_INTERFACE
        
        HRESULT ( STDMETHODCALLTYPE *QueryInterface )( 
            IVsEnumSyntheticRegions * This,
            /* [in] */ __RPC__in REFIID riid,
            /* [iid_is][out] */ 
            __RPC__deref_out  void **ppvObject);
        
        ULONG ( STDMETHODCALLTYPE *AddRef )( 
            IVsEnumSyntheticRegions * This);
        
        ULONG ( STDMETHODCALLTYPE *Release )( 
            IVsEnumSyntheticRegions * This);
        
        HRESULT ( STDMETHODCALLTYPE *Reset )( 
            IVsEnumSyntheticRegions * This);
        
        HRESULT ( STDMETHODCALLTYPE *Next )( 
            IVsEnumSyntheticRegions * This,
            /* [in] */ ULONG cEl,
            /* [size_is][out] */ __RPC__out_ecount_full(cEl) IVsSyntheticRegion **ppOut,
            /* [out] */ __RPC__out ULONG *pcElFetched);
        
        HRESULT ( STDMETHODCALLTYPE *GetCount )( 
            IVsEnumSyntheticRegions * This,
            /* [out] */ __RPC__out ULONG *pcRegions);
        
        END_INTERFACE
    } IVsEnumSyntheticRegionsVtbl;

    interface IVsEnumSyntheticRegions
    {
        CONST_VTBL struct IVsEnumSyntheticRegionsVtbl *lpVtbl;
    };

    

#ifdef COBJMACROS


#define IVsEnumSyntheticRegions_QueryInterface(This,riid,ppvObject)	\
    ( (This)->lpVtbl -> QueryInterface(This,riid,ppvObject) ) 

#define IVsEnumSyntheticRegions_AddRef(This)	\
    ( (This)->lpVtbl -> AddRef(This) ) 

#define IVsEnumSyntheticRegions_Release(This)	\
    ( (This)->lpVtbl -> Release(This) ) 


#define IVsEnumSyntheticRegions_Reset(This)	\
    ( (This)->lpVtbl -> Reset(This) ) 

#define IVsEnumSyntheticRegions_Next(This,cEl,ppOut,pcElFetched)	\
    ( (This)->lpVtbl -> Next(This,cEl,ppOut,pcElFetched) ) 

#define IVsEnumSyntheticRegions_GetCount(This,pcRegions)	\
    ( (This)->lpVtbl -> GetCount(This,pcRegions) ) 

#endif /* COBJMACROS */


#endif 	/* C style interface */




#endif 	/* __IVsEnumSyntheticRegions_INTERFACE_DEFINED__ */


#ifndef __IVsSyntheticTextClient_INTERFACE_DEFINED__
#define __IVsSyntheticTextClient_INTERFACE_DEFINED__

/* interface IVsSyntheticTextClient */
/* [object][uuid] */ 


EXTERN_C const IID IID_IVsSyntheticTextClient;

#if defined(__cplusplus) && !defined(CINTERFACE)
    
    MIDL_INTERFACE("1A8FBF86-5949-4E08-B9B6-F528AFB2DF9C")
    IVsSyntheticTextClient : public IUnknown
    {
    public:
        virtual void STDMETHODCALLTYPE OnSyntheticRegionChange( 
            /* [in] */ __RPC__in_opt IVsSyntheticRegion *pSynthReg,
            /* [in] */ SYNTHETIC_REGION_EVENT EventCode) = 0;
        
        virtual HRESULT STDMETHODCALLTYPE GetTipText( 
            /* [in] */ __RPC__in_opt IVsSyntheticRegion *pSynthReg,
            /* [optional][out] */ __RPC__deref_out_opt BSTR *pbstrText) = 0;
        
        virtual HRESULT STDMETHODCALLTYPE GetErrorText( 
            /* [in] */ DWORD dwReason,
            /* [in] */ __RPC__in_opt IVsSyntheticRegion *pSynthReg,
            /* [out] */ __RPC__deref_out_opt BSTR *pbstrText) = 0;
        
        virtual HRESULT STDMETHODCALLTYPE GetMarkerCommandInfo( 
            /* [in] */ __RPC__in_opt IVsSyntheticRegion *pSynthReg,
            /* [in] */ long iItem,
            /* [custom][out] */ __RPC__deref_out_opt BSTR *pbstrText,
            /* [out] */ __RPC__out DWORD *pcmdf) = 0;
        
        virtual HRESULT STDMETHODCALLTYPE ExecMarkerCommand( 
            /* [in] */ __RPC__in_opt IVsSyntheticRegion *pSynthReg,
            /* [in] */ long iItem) = 0;
        
        virtual void STDMETHODCALLTYPE OnBeforeSessionEnd( void) = 0;
        
    };
    
#else 	/* C style interface */

    typedef struct IVsSyntheticTextClientVtbl
    {
        BEGIN_INTERFACE
        
        HRESULT ( STDMETHODCALLTYPE *QueryInterface )( 
            IVsSyntheticTextClient * This,
            /* [in] */ __RPC__in REFIID riid,
            /* [iid_is][out] */ 
            __RPC__deref_out  void **ppvObject);
        
        ULONG ( STDMETHODCALLTYPE *AddRef )( 
            IVsSyntheticTextClient * This);
        
        ULONG ( STDMETHODCALLTYPE *Release )( 
            IVsSyntheticTextClient * This);
        
        void ( STDMETHODCALLTYPE *OnSyntheticRegionChange )( 
            IVsSyntheticTextClient * This,
            /* [in] */ __RPC__in_opt IVsSyntheticRegion *pSynthReg,
            /* [in] */ SYNTHETIC_REGION_EVENT EventCode);
        
        HRESULT ( STDMETHODCALLTYPE *GetTipText )( 
            IVsSyntheticTextClient * This,
            /* [in] */ __RPC__in_opt IVsSyntheticRegion *pSynthReg,
            /* [optional][out] */ __RPC__deref_out_opt BSTR *pbstrText);
        
        HRESULT ( STDMETHODCALLTYPE *GetErrorText )( 
            IVsSyntheticTextClient * This,
            /* [in] */ DWORD dwReason,
            /* [in] */ __RPC__in_opt IVsSyntheticRegion *pSynthReg,
            /* [out] */ __RPC__deref_out_opt BSTR *pbstrText);
        
        HRESULT ( STDMETHODCALLTYPE *GetMarkerCommandInfo )( 
            IVsSyntheticTextClient * This,
            /* [in] */ __RPC__in_opt IVsSyntheticRegion *pSynthReg,
            /* [in] */ long iItem,
            /* [custom][out] */ __RPC__deref_out_opt BSTR *pbstrText,
            /* [out] */ __RPC__out DWORD *pcmdf);
        
        HRESULT ( STDMETHODCALLTYPE *ExecMarkerCommand )( 
            IVsSyntheticTextClient * This,
            /* [in] */ __RPC__in_opt IVsSyntheticRegion *pSynthReg,
            /* [in] */ long iItem);
        
        void ( STDMETHODCALLTYPE *OnBeforeSessionEnd )( 
            IVsSyntheticTextClient * This);
        
        END_INTERFACE
    } IVsSyntheticTextClientVtbl;

    interface IVsSyntheticTextClient
    {
        CONST_VTBL struct IVsSyntheticTextClientVtbl *lpVtbl;
    };

    

#ifdef COBJMACROS


#define IVsSyntheticTextClient_QueryInterface(This,riid,ppvObject)	\
    ( (This)->lpVtbl -> QueryInterface(This,riid,ppvObject) ) 

#define IVsSyntheticTextClient_AddRef(This)	\
    ( (This)->lpVtbl -> AddRef(This) ) 

#define IVsSyntheticTextClient_Release(This)	\
    ( (This)->lpVtbl -> Release(This) ) 


#define IVsSyntheticTextClient_OnSyntheticRegionChange(This,pSynthReg,EventCode)	\
    ( (This)->lpVtbl -> OnSyntheticRegionChange(This,pSynthReg,EventCode) ) 

#define IVsSyntheticTextClient_GetTipText(This,pSynthReg,pbstrText)	\
    ( (This)->lpVtbl -> GetTipText(This,pSynthReg,pbstrText) ) 

#define IVsSyntheticTextClient_GetErrorText(This,dwReason,pSynthReg,pbstrText)	\
    ( (This)->lpVtbl -> GetErrorText(This,dwReason,pSynthReg,pbstrText) ) 

#define IVsSyntheticTextClient_GetMarkerCommandInfo(This,pSynthReg,iItem,pbstrText,pcmdf)	\
    ( (This)->lpVtbl -> GetMarkerCommandInfo(This,pSynthReg,iItem,pbstrText,pcmdf) ) 

#define IVsSyntheticTextClient_ExecMarkerCommand(This,pSynthReg,iItem)	\
    ( (This)->lpVtbl -> ExecMarkerCommand(This,pSynthReg,iItem) ) 

#define IVsSyntheticTextClient_OnBeforeSessionEnd(This)	\
    ( (This)->lpVtbl -> OnBeforeSessionEnd(This) ) 

#endif /* COBJMACROS */


#endif 	/* C style interface */




#endif 	/* __IVsSyntheticTextClient_INTERFACE_DEFINED__ */


#ifndef __IVsSyntheticTextSession_INTERFACE_DEFINED__
#define __IVsSyntheticTextSession_INTERFACE_DEFINED__

/* interface IVsSyntheticTextSession */
/* [object][uuid] */ 


EXTERN_C const IID IID_IVsSyntheticTextSession;

#if defined(__cplusplus) && !defined(CINTERFACE)
    
    MIDL_INTERFACE("0C71E351-5141-4B6F-B838-E212F0764875")
    IVsSyntheticTextSession : public IUnknown
    {
    public:
        virtual HRESULT STDMETHODCALLTYPE AddSyntheticRegions( 
            /* [in] */ DWORD dwUpdateFlags,
            /* [in] */ long cRegions,
            /* [size_is][in] */ __RPC__in_ecount_full(cRegions) NewSyntheticRegion *rgSynthReg,
            /* [out] */ __RPC__deref_out_opt IVsEnumSyntheticRegions **ppEnum) = 0;
        
        virtual HRESULT STDMETHODCALLTYPE EnumSyntheticRegions( 
            /* [in] */ DWORD dwFindFlags,
            /* [in] */ DWORD_PTR dwCookie,
            /* [in] */ __RPC__in TextSpan *ptsRange,
            /* [out] */ __RPC__deref_out_opt IVsEnumSyntheticRegions **ppEnum) = 0;
        
        virtual HRESULT STDMETHODCALLTYPE UnadviseClient( void) = 0;
        
        virtual HRESULT STDMETHODCALLTYPE Terminate( void) = 0;
        
    };
    
#else 	/* C style interface */

    typedef struct IVsSyntheticTextSessionVtbl
    {
        BEGIN_INTERFACE
        
        HRESULT ( STDMETHODCALLTYPE *QueryInterface )( 
            IVsSyntheticTextSession * This,
            /* [in] */ __RPC__in REFIID riid,
            /* [iid_is][out] */ 
            __RPC__deref_out  void **ppvObject);
        
        ULONG ( STDMETHODCALLTYPE *AddRef )( 
            IVsSyntheticTextSession * This);
        
        ULONG ( STDMETHODCALLTYPE *Release )( 
            IVsSyntheticTextSession * This);
        
        HRESULT ( STDMETHODCALLTYPE *AddSyntheticRegions )( 
            IVsSyntheticTextSession * This,
            /* [in] */ DWORD dwUpdateFlags,
            /* [in] */ long cRegions,
            /* [size_is][in] */ __RPC__in_ecount_full(cRegions) NewSyntheticRegion *rgSynthReg,
            /* [out] */ __RPC__deref_out_opt IVsEnumSyntheticRegions **ppEnum);
        
        HRESULT ( STDMETHODCALLTYPE *EnumSyntheticRegions )( 
            IVsSyntheticTextSession * This,
            /* [in] */ DWORD dwFindFlags,
            /* [in] */ DWORD_PTR dwCookie,
            /* [in] */ __RPC__in TextSpan *ptsRange,
            /* [out] */ __RPC__deref_out_opt IVsEnumSyntheticRegions **ppEnum);
        
        HRESULT ( STDMETHODCALLTYPE *UnadviseClient )( 
            IVsSyntheticTextSession * This);
        
        HRESULT ( STDMETHODCALLTYPE *Terminate )( 
            IVsSyntheticTextSession * This);
        
        END_INTERFACE
    } IVsSyntheticTextSessionVtbl;

    interface IVsSyntheticTextSession
    {
        CONST_VTBL struct IVsSyntheticTextSessionVtbl *lpVtbl;
    };

    

#ifdef COBJMACROS


#define IVsSyntheticTextSession_QueryInterface(This,riid,ppvObject)	\
    ( (This)->lpVtbl -> QueryInterface(This,riid,ppvObject) ) 

#define IVsSyntheticTextSession_AddRef(This)	\
    ( (This)->lpVtbl -> AddRef(This) ) 

#define IVsSyntheticTextSession_Release(This)	\
    ( (This)->lpVtbl -> Release(This) ) 


#define IVsSyntheticTextSession_AddSyntheticRegions(This,dwUpdateFlags,cRegions,rgSynthReg,ppEnum)	\
    ( (This)->lpVtbl -> AddSyntheticRegions(This,dwUpdateFlags,cRegions,rgSynthReg,ppEnum) ) 

#define IVsSyntheticTextSession_EnumSyntheticRegions(This,dwFindFlags,dwCookie,ptsRange,ppEnum)	\
    ( (This)->lpVtbl -> EnumSyntheticRegions(This,dwFindFlags,dwCookie,ptsRange,ppEnum) ) 

#define IVsSyntheticTextSession_UnadviseClient(This)	\
    ( (This)->lpVtbl -> UnadviseClient(This) ) 

#define IVsSyntheticTextSession_Terminate(This)	\
    ( (This)->lpVtbl -> Terminate(This) ) 

#endif /* COBJMACROS */


#endif 	/* C style interface */




#endif 	/* __IVsSyntheticTextSession_INTERFACE_DEFINED__ */


#ifndef __IVsSyntheticTextManager_INTERFACE_DEFINED__
#define __IVsSyntheticTextManager_INTERFACE_DEFINED__

/* interface IVsSyntheticTextManager */
/* [object][uuid] */ 


EXTERN_C const IID IID_IVsSyntheticTextManager;

#if defined(__cplusplus) && !defined(CINTERFACE)
    
    MIDL_INTERFACE("2AB20832-0942-42F9-974F-4E65B85DABF3")
    IVsSyntheticTextManager : public IUnknown
    {
    public:
        virtual HRESULT STDMETHODCALLTYPE GetSyntheticTextSession( 
            /* [in] */ __RPC__in_opt IUnknown *pOwningObject,
            /* [out] */ __RPC__deref_out_opt IVsSyntheticTextSession **ppSession) = 0;
        
        virtual HRESULT STDMETHODCALLTYPE CreateSyntheticTextSession( 
            /* [in] */ DWORD dwFlags,
            /* [in] */ __RPC__in_opt IUnknown *pOwningObject,
            /* [in] */ __RPC__in_opt IVsSyntheticTextClient *pClient,
            /* [out] */ __RPC__deref_out_opt IVsSyntheticTextSession **ppState) = 0;
        
    };
    
#else 	/* C style interface */

    typedef struct IVsSyntheticTextManagerVtbl
    {
        BEGIN_INTERFACE
        
        HRESULT ( STDMETHODCALLTYPE *QueryInterface )( 
            IVsSyntheticTextManager * This,
            /* [in] */ __RPC__in REFIID riid,
            /* [iid_is][out] */ 
            __RPC__deref_out  void **ppvObject);
        
        ULONG ( STDMETHODCALLTYPE *AddRef )( 
            IVsSyntheticTextManager * This);
        
        ULONG ( STDMETHODCALLTYPE *Release )( 
            IVsSyntheticTextManager * This);
        
        HRESULT ( STDMETHODCALLTYPE *GetSyntheticTextSession )( 
            IVsSyntheticTextManager * This,
            /* [in] */ __RPC__in_opt IUnknown *pOwningObject,
            /* [out] */ __RPC__deref_out_opt IVsSyntheticTextSession **ppSession);
        
        HRESULT ( STDMETHODCALLTYPE *CreateSyntheticTextSession )( 
            IVsSyntheticTextManager * This,
            /* [in] */ DWORD dwFlags,
            /* [in] */ __RPC__in_opt IUnknown *pOwningObject,
            /* [in] */ __RPC__in_opt IVsSyntheticTextClient *pClient,
            /* [out] */ __RPC__deref_out_opt IVsSyntheticTextSession **ppState);
        
        END_INTERFACE
    } IVsSyntheticTextManagerVtbl;

    interface IVsSyntheticTextManager
    {
        CONST_VTBL struct IVsSyntheticTextManagerVtbl *lpVtbl;
    };

    

#ifdef COBJMACROS


#define IVsSyntheticTextManager_QueryInterface(This,riid,ppvObject)	\
    ( (This)->lpVtbl -> QueryInterface(This,riid,ppvObject) ) 

#define IVsSyntheticTextManager_AddRef(This)	\
    ( (This)->lpVtbl -> AddRef(This) ) 

#define IVsSyntheticTextManager_Release(This)	\
    ( (This)->lpVtbl -> Release(This) ) 


#define IVsSyntheticTextManager_GetSyntheticTextSession(This,pOwningObject,ppSession)	\
    ( (This)->lpVtbl -> GetSyntheticTextSession(This,pOwningObject,ppSession) ) 

#define IVsSyntheticTextManager_CreateSyntheticTextSession(This,dwFlags,pOwningObject,pClient,ppState)	\
    ( (This)->lpVtbl -> CreateSyntheticTextSession(This,dwFlags,pOwningObject,pClient,ppState) ) 

#endif /* COBJMACROS */


#endif 	/* C style interface */




#endif 	/* __IVsSyntheticTextManager_INTERFACE_DEFINED__ */


#ifndef __IVsTextHidingLayerModule_INTERFACE_DEFINED__
#define __IVsTextHidingLayerModule_INTERFACE_DEFINED__

/* interface IVsTextHidingLayerModule */
/* [object][uuid] */ 


EXTERN_C const IID IID_IVsTextHidingLayerModule;

#if defined(__cplusplus) && !defined(CINTERFACE)
    
    MIDL_INTERFACE("1A705427-815C-4DB6-9B5C-6D45CDDDD0C4")
    IVsTextHidingLayerModule : public IUnknown
    {
    public:
        virtual HRESULT STDMETHODCALLTYPE MakeBaseSpanVisible( 
            __RPC__in TextSpan *pBaseSpan) = 0;
        
    };
    
#else 	/* C style interface */

    typedef struct IVsTextHidingLayerModuleVtbl
    {
        BEGIN_INTERFACE
        
        HRESULT ( STDMETHODCALLTYPE *QueryInterface )( 
            IVsTextHidingLayerModule * This,
            /* [in] */ __RPC__in REFIID riid,
            /* [iid_is][out] */ 
            __RPC__deref_out  void **ppvObject);
        
        ULONG ( STDMETHODCALLTYPE *AddRef )( 
            IVsTextHidingLayerModule * This);
        
        ULONG ( STDMETHODCALLTYPE *Release )( 
            IVsTextHidingLayerModule * This);
        
        HRESULT ( STDMETHODCALLTYPE *MakeBaseSpanVisible )( 
            IVsTextHidingLayerModule * This,
            __RPC__in TextSpan *pBaseSpan);
        
        END_INTERFACE
    } IVsTextHidingLayerModuleVtbl;

    interface IVsTextHidingLayerModule
    {
        CONST_VTBL struct IVsTextHidingLayerModuleVtbl *lpVtbl;
    };

    

#ifdef COBJMACROS


#define IVsTextHidingLayerModule_QueryInterface(This,riid,ppvObject)	\
    ( (This)->lpVtbl -> QueryInterface(This,riid,ppvObject) ) 

#define IVsTextHidingLayerModule_AddRef(This)	\
    ( (This)->lpVtbl -> AddRef(This) ) 

#define IVsTextHidingLayerModule_Release(This)	\
    ( (This)->lpVtbl -> Release(This) ) 


#define IVsTextHidingLayerModule_MakeBaseSpanVisible(This,pBaseSpan)	\
    ( (This)->lpVtbl -> MakeBaseSpanVisible(This,pBaseSpan) ) 

#endif /* COBJMACROS */


#endif 	/* C style interface */




#endif 	/* __IVsTextHidingLayerModule_INTERFACE_DEFINED__ */


#ifndef __IVsHiddenRegion_INTERFACE_DEFINED__
#define __IVsHiddenRegion_INTERFACE_DEFINED__

/* interface IVsHiddenRegion */
/* [object][uuid] */ 


EXTERN_C const IID IID_IVsHiddenRegion;

#if defined(__cplusplus) && !defined(CINTERFACE)
    
    MIDL_INTERFACE("41E57382-63F9-48E6-9DBD-163D2B14D41B")
    IVsHiddenRegion : public IUnknown
    {
    public:
        virtual HRESULT STDMETHODCALLTYPE GetType( 
            /* [out] */ __RPC__out long *piHiddenRegionType) = 0;
        
        virtual HRESULT STDMETHODCALLTYPE GetBehavior( 
            /* [out] */ __RPC__out DWORD *pdwBehavior) = 0;
        
        virtual HRESULT STDMETHODCALLTYPE GetState( 
            /* [out] */ __RPC__out DWORD *dwState) = 0;
        
        virtual HRESULT STDMETHODCALLTYPE SetState( 
            /* [in] */ DWORD dwState,
            /* [in] */ DWORD dwUpdate) = 0;
        
        virtual HRESULT STDMETHODCALLTYPE GetBanner( 
            /* [out] */ __RPC__deref_out_opt BSTR *pbstrBanner) = 0;
        
        virtual HRESULT STDMETHODCALLTYPE SetBanner( 
            /* [in] */ __RPC__in LPCWSTR pszBanner) = 0;
        
        virtual HRESULT STDMETHODCALLTYPE GetSpan( 
            /* [out] */ __RPC__out TextSpan *pSpan) = 0;
        
        virtual HRESULT STDMETHODCALLTYPE SetSpan( 
            /* [in] */ __RPC__in TextSpan *pSpan) = 0;
        
        virtual HRESULT STDMETHODCALLTYPE GetClientData( 
            /* [out] */ __RPC__out DWORD_PTR *pdwData) = 0;
        
        virtual HRESULT STDMETHODCALLTYPE SetClientData( 
            /* [in] */ DWORD_PTR dwData) = 0;
        
        virtual HRESULT STDMETHODCALLTYPE Invalidate( 
            /* [in] */ DWORD dwUpdate) = 0;
        
        virtual HRESULT STDMETHODCALLTYPE IsValid( void) = 0;
        
        virtual HRESULT STDMETHODCALLTYPE GetBaseBuffer( 
            /* [out] */ __RPC__deref_out_opt IVsTextLines **ppBuffer) = 0;
        
    };
    
#else 	/* C style interface */

    typedef struct IVsHiddenRegionVtbl
    {
        BEGIN_INTERFACE
        
        HRESULT ( STDMETHODCALLTYPE *QueryInterface )( 
            IVsHiddenRegion * This,
            /* [in] */ __RPC__in REFIID riid,
            /* [iid_is][out] */ 
            __RPC__deref_out  void **ppvObject);
        
        ULONG ( STDMETHODCALLTYPE *AddRef )( 
            IVsHiddenRegion * This);
        
        ULONG ( STDMETHODCALLTYPE *Release )( 
            IVsHiddenRegion * This);
        
        HRESULT ( STDMETHODCALLTYPE *GetType )( 
            IVsHiddenRegion * This,
            /* [out] */ __RPC__out long *piHiddenRegionType);
        
        HRESULT ( STDMETHODCALLTYPE *GetBehavior )( 
            IVsHiddenRegion * This,
            /* [out] */ __RPC__out DWORD *pdwBehavior);
        
        HRESULT ( STDMETHODCALLTYPE *GetState )( 
            IVsHiddenRegion * This,
            /* [out] */ __RPC__out DWORD *dwState);
        
        HRESULT ( STDMETHODCALLTYPE *SetState )( 
            IVsHiddenRegion * This,
            /* [in] */ DWORD dwState,
            /* [in] */ DWORD dwUpdate);
        
        HRESULT ( STDMETHODCALLTYPE *GetBanner )( 
            IVsHiddenRegion * This,
            /* [out] */ __RPC__deref_out_opt BSTR *pbstrBanner);
        
        HRESULT ( STDMETHODCALLTYPE *SetBanner )( 
            IVsHiddenRegion * This,
            /* [in] */ __RPC__in LPCWSTR pszBanner);
        
        HRESULT ( STDMETHODCALLTYPE *GetSpan )( 
            IVsHiddenRegion * This,
            /* [out] */ __RPC__out TextSpan *pSpan);
        
        HRESULT ( STDMETHODCALLTYPE *SetSpan )( 
            IVsHiddenRegion * This,
            /* [in] */ __RPC__in TextSpan *pSpan);
        
        HRESULT ( STDMETHODCALLTYPE *GetClientData )( 
            IVsHiddenRegion * This,
            /* [out] */ __RPC__out DWORD_PTR *pdwData);
        
        HRESULT ( STDMETHODCALLTYPE *SetClientData )( 
            IVsHiddenRegion * This,
            /* [in] */ DWORD_PTR dwData);
        
        HRESULT ( STDMETHODCALLTYPE *Invalidate )( 
            IVsHiddenRegion * This,
            /* [in] */ DWORD dwUpdate);
        
        HRESULT ( STDMETHODCALLTYPE *IsValid )( 
            IVsHiddenRegion * This);
        
        HRESULT ( STDMETHODCALLTYPE *GetBaseBuffer )( 
            IVsHiddenRegion * This,
            /* [out] */ __RPC__deref_out_opt IVsTextLines **ppBuffer);
        
        END_INTERFACE
    } IVsHiddenRegionVtbl;

    interface IVsHiddenRegion
    {
        CONST_VTBL struct IVsHiddenRegionVtbl *lpVtbl;
    };

    

#ifdef COBJMACROS


#define IVsHiddenRegion_QueryInterface(This,riid,ppvObject)	\
    ( (This)->lpVtbl -> QueryInterface(This,riid,ppvObject) ) 

#define IVsHiddenRegion_AddRef(This)	\
    ( (This)->lpVtbl -> AddRef(This) ) 

#define IVsHiddenRegion_Release(This)	\
    ( (This)->lpVtbl -> Release(This) ) 


#define IVsHiddenRegion_GetType(This,piHiddenRegionType)	\
    ( (This)->lpVtbl -> GetType(This,piHiddenRegionType) ) 

#define IVsHiddenRegion_GetBehavior(This,pdwBehavior)	\
    ( (This)->lpVtbl -> GetBehavior(This,pdwBehavior) ) 

#define IVsHiddenRegion_GetState(This,dwState)	\
    ( (This)->lpVtbl -> GetState(This,dwState) ) 

#define IVsHiddenRegion_SetState(This,dwState,dwUpdate)	\
    ( (This)->lpVtbl -> SetState(This,dwState,dwUpdate) ) 

#define IVsHiddenRegion_GetBanner(This,pbstrBanner)	\
    ( (This)->lpVtbl -> GetBanner(This,pbstrBanner) ) 

#define IVsHiddenRegion_SetBanner(This,pszBanner)	\
    ( (This)->lpVtbl -> SetBanner(This,pszBanner) ) 

#define IVsHiddenRegion_GetSpan(This,pSpan)	\
    ( (This)->lpVtbl -> GetSpan(This,pSpan) ) 

#define IVsHiddenRegion_SetSpan(This,pSpan)	\
    ( (This)->lpVtbl -> SetSpan(This,pSpan) ) 

#define IVsHiddenRegion_GetClientData(This,pdwData)	\
    ( (This)->lpVtbl -> GetClientData(This,pdwData) ) 

#define IVsHiddenRegion_SetClientData(This,dwData)	\
    ( (This)->lpVtbl -> SetClientData(This,dwData) ) 

#define IVsHiddenRegion_Invalidate(This,dwUpdate)	\
    ( (This)->lpVtbl -> Invalidate(This,dwUpdate) ) 

#define IVsHiddenRegion_IsValid(This)	\
    ( (This)->lpVtbl -> IsValid(This) ) 

#define IVsHiddenRegion_GetBaseBuffer(This,ppBuffer)	\
    ( (This)->lpVtbl -> GetBaseBuffer(This,ppBuffer) ) 

#endif /* COBJMACROS */


#endif 	/* C style interface */




#endif 	/* __IVsHiddenRegion_INTERFACE_DEFINED__ */


#ifndef __IVsEnumHiddenRegions_INTERFACE_DEFINED__
#define __IVsEnumHiddenRegions_INTERFACE_DEFINED__

/* interface IVsEnumHiddenRegions */
/* [object][uuid] */ 


EXTERN_C const IID IID_IVsEnumHiddenRegions;

#if defined(__cplusplus) && !defined(CINTERFACE)
    
    MIDL_INTERFACE("681B0FFF-30B4-4578-A911-B67FB049DAFA")
    IVsEnumHiddenRegions : public IUnknown
    {
    public:
        virtual HRESULT STDMETHODCALLTYPE Reset( void) = 0;
        
        virtual HRESULT STDMETHODCALLTYPE Next( 
            /* [in] */ ULONG cEl,
            /* [size_is][out] */ __RPC__out_ecount_full(cEl) IVsHiddenRegion **ppOut,
            /* [out] */ __RPC__out ULONG *pcElFetched) = 0;
        
        virtual HRESULT STDMETHODCALLTYPE GetCount( 
            /* [out] */ __RPC__out ULONG *pcRegions) = 0;
        
    };
    
#else 	/* C style interface */

    typedef struct IVsEnumHiddenRegionsVtbl
    {
        BEGIN_INTERFACE
        
        HRESULT ( STDMETHODCALLTYPE *QueryInterface )( 
            IVsEnumHiddenRegions * This,
            /* [in] */ __RPC__in REFIID riid,
            /* [iid_is][out] */ 
            __RPC__deref_out  void **ppvObject);
        
        ULONG ( STDMETHODCALLTYPE *AddRef )( 
            IVsEnumHiddenRegions * This);
        
        ULONG ( STDMETHODCALLTYPE *Release )( 
            IVsEnumHiddenRegions * This);
        
        HRESULT ( STDMETHODCALLTYPE *Reset )( 
            IVsEnumHiddenRegions * This);
        
        HRESULT ( STDMETHODCALLTYPE *Next )( 
            IVsEnumHiddenRegions * This,
            /* [in] */ ULONG cEl,
            /* [size_is][out] */ __RPC__out_ecount_full(cEl) IVsHiddenRegion **ppOut,
            /* [out] */ __RPC__out ULONG *pcElFetched);
        
        HRESULT ( STDMETHODCALLTYPE *GetCount )( 
            IVsEnumHiddenRegions * This,
            /* [out] */ __RPC__out ULONG *pcRegions);
        
        END_INTERFACE
    } IVsEnumHiddenRegionsVtbl;

    interface IVsEnumHiddenRegions
    {
        CONST_VTBL struct IVsEnumHiddenRegionsVtbl *lpVtbl;
    };

    

#ifdef COBJMACROS


#define IVsEnumHiddenRegions_QueryInterface(This,riid,ppvObject)	\
    ( (This)->lpVtbl -> QueryInterface(This,riid,ppvObject) ) 

#define IVsEnumHiddenRegions_AddRef(This)	\
    ( (This)->lpVtbl -> AddRef(This) ) 

#define IVsEnumHiddenRegions_Release(This)	\
    ( (This)->lpVtbl -> Release(This) ) 


#define IVsEnumHiddenRegions_Reset(This)	\
    ( (This)->lpVtbl -> Reset(This) ) 

#define IVsEnumHiddenRegions_Next(This,cEl,ppOut,pcElFetched)	\
    ( (This)->lpVtbl -> Next(This,cEl,ppOut,pcElFetched) ) 

#define IVsEnumHiddenRegions_GetCount(This,pcRegions)	\
    ( (This)->lpVtbl -> GetCount(This,pcRegions) ) 

#endif /* COBJMACROS */


#endif 	/* C style interface */




#endif 	/* __IVsEnumHiddenRegions_INTERFACE_DEFINED__ */


#ifndef __IVsHiddenTextClient_INTERFACE_DEFINED__
#define __IVsHiddenTextClient_INTERFACE_DEFINED__

/* interface IVsHiddenTextClient */
/* [object][uuid] */ 


EXTERN_C const IID IID_IVsHiddenTextClient;

#if defined(__cplusplus) && !defined(CINTERFACE)
    
    MIDL_INTERFACE("CA3F0E7F-CFA6-444E-A3FE-7B3DFAD6E5E4")
    IVsHiddenTextClient : public IUnknown
    {
    public:
        virtual void STDMETHODCALLTYPE OnHiddenRegionChange( 
            /* [in] */ __RPC__in_opt IVsHiddenRegion *pHidReg,
            /* [in] */ HIDDEN_REGION_EVENT EventCode,
            /* [in] */ BOOL fBufferModifiable) = 0;
        
        virtual HRESULT STDMETHODCALLTYPE GetTipText( 
            /* [in] */ __RPC__in_opt IVsHiddenRegion *pHidReg,
            /* [optional][out] */ __RPC__deref_out_opt BSTR *pbstrText) = 0;
        
        virtual HRESULT STDMETHODCALLTYPE GetMarkerCommandInfo( 
            /* [in] */ __RPC__in_opt IVsHiddenRegion *pHidReg,
            /* [in] */ long iItem,
            /* [custom][out] */ __RPC__deref_out_opt BSTR *pbstrText,
            /* [out] */ __RPC__out DWORD *pcmdf) = 0;
        
        virtual HRESULT STDMETHODCALLTYPE ExecMarkerCommand( 
            /* [in] */ __RPC__in_opt IVsHiddenRegion *pHidReg,
            /* [in] */ long iItem) = 0;
        
        virtual HRESULT STDMETHODCALLTYPE MakeBaseSpanVisible( 
            /* [in] */ __RPC__in_opt IVsHiddenRegion *pHidReg,
            /* [in] */ __RPC__in TextSpan *pBaseSpan) = 0;
        
        virtual void STDMETHODCALLTYPE OnBeforeSessionEnd( void) = 0;
        
    };
    
#else 	/* C style interface */

    typedef struct IVsHiddenTextClientVtbl
    {
        BEGIN_INTERFACE
        
        HRESULT ( STDMETHODCALLTYPE *QueryInterface )( 
            IVsHiddenTextClient * This,
            /* [in] */ __RPC__in REFIID riid,
            /* [iid_is][out] */ 
            __RPC__deref_out  void **ppvObject);
        
        ULONG ( STDMETHODCALLTYPE *AddRef )( 
            IVsHiddenTextClient * This);
        
        ULONG ( STDMETHODCALLTYPE *Release )( 
            IVsHiddenTextClient * This);
        
        void ( STDMETHODCALLTYPE *OnHiddenRegionChange )( 
            IVsHiddenTextClient * This,
            /* [in] */ __RPC__in_opt IVsHiddenRegion *pHidReg,
            /* [in] */ HIDDEN_REGION_EVENT EventCode,
            /* [in] */ BOOL fBufferModifiable);
        
        HRESULT ( STDMETHODCALLTYPE *GetTipText )( 
            IVsHiddenTextClient * This,
            /* [in] */ __RPC__in_opt IVsHiddenRegion *pHidReg,
            /* [optional][out] */ __RPC__deref_out_opt BSTR *pbstrText);
        
        HRESULT ( STDMETHODCALLTYPE *GetMarkerCommandInfo )( 
            IVsHiddenTextClient * This,
            /* [in] */ __RPC__in_opt IVsHiddenRegion *pHidReg,
            /* [in] */ long iItem,
            /* [custom][out] */ __RPC__deref_out_opt BSTR *pbstrText,
            /* [out] */ __RPC__out DWORD *pcmdf);
        
        HRESULT ( STDMETHODCALLTYPE *ExecMarkerCommand )( 
            IVsHiddenTextClient * This,
            /* [in] */ __RPC__in_opt IVsHiddenRegion *pHidReg,
            /* [in] */ long iItem);
        
        HRESULT ( STDMETHODCALLTYPE *MakeBaseSpanVisible )( 
            IVsHiddenTextClient * This,
            /* [in] */ __RPC__in_opt IVsHiddenRegion *pHidReg,
            /* [in] */ __RPC__in TextSpan *pBaseSpan);
        
        void ( STDMETHODCALLTYPE *OnBeforeSessionEnd )( 
            IVsHiddenTextClient * This);
        
        END_INTERFACE
    } IVsHiddenTextClientVtbl;

    interface IVsHiddenTextClient
    {
        CONST_VTBL struct IVsHiddenTextClientVtbl *lpVtbl;
    };

    

#ifdef COBJMACROS


#define IVsHiddenTextClient_QueryInterface(This,riid,ppvObject)	\
    ( (This)->lpVtbl -> QueryInterface(This,riid,ppvObject) ) 

#define IVsHiddenTextClient_AddRef(This)	\
    ( (This)->lpVtbl -> AddRef(This) ) 

#define IVsHiddenTextClient_Release(This)	\
    ( (This)->lpVtbl -> Release(This) ) 


#define IVsHiddenTextClient_OnHiddenRegionChange(This,pHidReg,EventCode,fBufferModifiable)	\
    ( (This)->lpVtbl -> OnHiddenRegionChange(This,pHidReg,EventCode,fBufferModifiable) ) 

#define IVsHiddenTextClient_GetTipText(This,pHidReg,pbstrText)	\
    ( (This)->lpVtbl -> GetTipText(This,pHidReg,pbstrText) ) 

#define IVsHiddenTextClient_GetMarkerCommandInfo(This,pHidReg,iItem,pbstrText,pcmdf)	\
    ( (This)->lpVtbl -> GetMarkerCommandInfo(This,pHidReg,iItem,pbstrText,pcmdf) ) 

#define IVsHiddenTextClient_ExecMarkerCommand(This,pHidReg,iItem)	\
    ( (This)->lpVtbl -> ExecMarkerCommand(This,pHidReg,iItem) ) 

#define IVsHiddenTextClient_MakeBaseSpanVisible(This,pHidReg,pBaseSpan)	\
    ( (This)->lpVtbl -> MakeBaseSpanVisible(This,pHidReg,pBaseSpan) ) 

#define IVsHiddenTextClient_OnBeforeSessionEnd(This)	\
    ( (This)->lpVtbl -> OnBeforeSessionEnd(This) ) 

#endif /* COBJMACROS */


#endif 	/* C style interface */




#endif 	/* __IVsHiddenTextClient_INTERFACE_DEFINED__ */


#ifndef __IVsHiddenTextManager_INTERFACE_DEFINED__
#define __IVsHiddenTextManager_INTERFACE_DEFINED__

/* interface IVsHiddenTextManager */
/* [object][uuid] */ 


EXTERN_C const IID IID_IVsHiddenTextManager;

#if defined(__cplusplus) && !defined(CINTERFACE)
    
    MIDL_INTERFACE("0E5B1E59-53A7-4636-ADBB-0B23C561E293")
    IVsHiddenTextManager : public IUnknown
    {
    public:
        virtual HRESULT STDMETHODCALLTYPE GetHiddenTextSession( 
            /* [in] */ __RPC__in_opt IUnknown *pOwningObject,
            /* [out] */ __RPC__deref_out_opt IVsHiddenTextSession **ppSession) = 0;
        
        virtual HRESULT STDMETHODCALLTYPE CreateHiddenTextSession( 
            /* [in] */ DWORD dwFlags,
            /* [in] */ __RPC__in_opt IUnknown *pOwningObject,
            /* [in] */ __RPC__in_opt IVsHiddenTextClient *pClient,
            /* [out] */ __RPC__deref_out_opt IVsHiddenTextSession **ppState) = 0;
        
    };
    
#else 	/* C style interface */

    typedef struct IVsHiddenTextManagerVtbl
    {
        BEGIN_INTERFACE
        
        HRESULT ( STDMETHODCALLTYPE *QueryInterface )( 
            IVsHiddenTextManager * This,
            /* [in] */ __RPC__in REFIID riid,
            /* [iid_is][out] */ 
            __RPC__deref_out  void **ppvObject);
        
        ULONG ( STDMETHODCALLTYPE *AddRef )( 
            IVsHiddenTextManager * This);
        
        ULONG ( STDMETHODCALLTYPE *Release )( 
            IVsHiddenTextManager * This);
        
        HRESULT ( STDMETHODCALLTYPE *GetHiddenTextSession )( 
            IVsHiddenTextManager * This,
            /* [in] */ __RPC__in_opt IUnknown *pOwningObject,
            /* [out] */ __RPC__deref_out_opt IVsHiddenTextSession **ppSession);
        
        HRESULT ( STDMETHODCALLTYPE *CreateHiddenTextSession )( 
            IVsHiddenTextManager * This,
            /* [in] */ DWORD dwFlags,
            /* [in] */ __RPC__in_opt IUnknown *pOwningObject,
            /* [in] */ __RPC__in_opt IVsHiddenTextClient *pClient,
            /* [out] */ __RPC__deref_out_opt IVsHiddenTextSession **ppState);
        
        END_INTERFACE
    } IVsHiddenTextManagerVtbl;

    interface IVsHiddenTextManager
    {
        CONST_VTBL struct IVsHiddenTextManagerVtbl *lpVtbl;
    };

    

#ifdef COBJMACROS


#define IVsHiddenTextManager_QueryInterface(This,riid,ppvObject)	\
    ( (This)->lpVtbl -> QueryInterface(This,riid,ppvObject) ) 

#define IVsHiddenTextManager_AddRef(This)	\
    ( (This)->lpVtbl -> AddRef(This) ) 

#define IVsHiddenTextManager_Release(This)	\
    ( (This)->lpVtbl -> Release(This) ) 


#define IVsHiddenTextManager_GetHiddenTextSession(This,pOwningObject,ppSession)	\
    ( (This)->lpVtbl -> GetHiddenTextSession(This,pOwningObject,ppSession) ) 

#define IVsHiddenTextManager_CreateHiddenTextSession(This,dwFlags,pOwningObject,pClient,ppState)	\
    ( (This)->lpVtbl -> CreateHiddenTextSession(This,dwFlags,pOwningObject,pClient,ppState) ) 

#endif /* COBJMACROS */


#endif 	/* C style interface */




#endif 	/* __IVsHiddenTextManager_INTERFACE_DEFINED__ */


#ifndef __IVsHiddenTextSession_INTERFACE_DEFINED__
#define __IVsHiddenTextSession_INTERFACE_DEFINED__

/* interface IVsHiddenTextSession */
/* [object][uuid] */ 


EXTERN_C const IID IID_IVsHiddenTextSession;

#if defined(__cplusplus) && !defined(CINTERFACE)
    
    MIDL_INTERFACE("82B21BAE-664B-43E1-95B1-0302805FF7D3")
    IVsHiddenTextSession : public IUnknown
    {
    public:
        virtual HRESULT STDMETHODCALLTYPE AddHiddenRegions( 
            /* [in] */ DWORD dwUpdateFlags,
            /* [in] */ long cRegions,
            /* [size_is][in] */ __RPC__in_ecount_full(cRegions) NewHiddenRegion *rgHidReg,
            /* [out] */ __RPC__deref_out_opt IVsEnumHiddenRegions **ppEnum) = 0;
        
        virtual HRESULT STDMETHODCALLTYPE EnumHiddenRegions( 
            /* [in] */ DWORD dwFindFlags,
            /* [in] */ DWORD_PTR dwCookie,
            /* [in] */ __RPC__in TextSpan *ptsRange,
            /* [out] */ __RPC__deref_out_opt IVsEnumHiddenRegions **ppEnum) = 0;
        
        virtual HRESULT STDMETHODCALLTYPE UnadviseClient( void) = 0;
        
        virtual HRESULT STDMETHODCALLTYPE Terminate( void) = 0;
        
    };
    
#else 	/* C style interface */

    typedef struct IVsHiddenTextSessionVtbl
    {
        BEGIN_INTERFACE
        
        HRESULT ( STDMETHODCALLTYPE *QueryInterface )( 
            IVsHiddenTextSession * This,
            /* [in] */ __RPC__in REFIID riid,
            /* [iid_is][out] */ 
            __RPC__deref_out  void **ppvObject);
        
        ULONG ( STDMETHODCALLTYPE *AddRef )( 
            IVsHiddenTextSession * This);
        
        ULONG ( STDMETHODCALLTYPE *Release )( 
            IVsHiddenTextSession * This);
        
        HRESULT ( STDMETHODCALLTYPE *AddHiddenRegions )( 
            IVsHiddenTextSession * This,
            /* [in] */ DWORD dwUpdateFlags,
            /* [in] */ long cRegions,
            /* [size_is][in] */ __RPC__in_ecount_full(cRegions) NewHiddenRegion *rgHidReg,
            /* [out] */ __RPC__deref_out_opt IVsEnumHiddenRegions **ppEnum);
        
        HRESULT ( STDMETHODCALLTYPE *EnumHiddenRegions )( 
            IVsHiddenTextSession * This,
            /* [in] */ DWORD dwFindFlags,
            /* [in] */ DWORD_PTR dwCookie,
            /* [in] */ __RPC__in TextSpan *ptsRange,
            /* [out] */ __RPC__deref_out_opt IVsEnumHiddenRegions **ppEnum);
        
        HRESULT ( STDMETHODCALLTYPE *UnadviseClient )( 
            IVsHiddenTextSession * This);
        
        HRESULT ( STDMETHODCALLTYPE *Terminate )( 
            IVsHiddenTextSession * This);
        
        END_INTERFACE
    } IVsHiddenTextSessionVtbl;

    interface IVsHiddenTextSession
    {
        CONST_VTBL struct IVsHiddenTextSessionVtbl *lpVtbl;
    };

    

#ifdef COBJMACROS


#define IVsHiddenTextSession_QueryInterface(This,riid,ppvObject)	\
    ( (This)->lpVtbl -> QueryInterface(This,riid,ppvObject) ) 

#define IVsHiddenTextSession_AddRef(This)	\
    ( (This)->lpVtbl -> AddRef(This) ) 

#define IVsHiddenTextSession_Release(This)	\
    ( (This)->lpVtbl -> Release(This) ) 


#define IVsHiddenTextSession_AddHiddenRegions(This,dwUpdateFlags,cRegions,rgHidReg,ppEnum)	\
    ( (This)->lpVtbl -> AddHiddenRegions(This,dwUpdateFlags,cRegions,rgHidReg,ppEnum) ) 

#define IVsHiddenTextSession_EnumHiddenRegions(This,dwFindFlags,dwCookie,ptsRange,ppEnum)	\
    ( (This)->lpVtbl -> EnumHiddenRegions(This,dwFindFlags,dwCookie,ptsRange,ppEnum) ) 

#define IVsHiddenTextSession_UnadviseClient(This)	\
    ( (This)->lpVtbl -> UnadviseClient(This) ) 

#define IVsHiddenTextSession_Terminate(This)	\
    ( (This)->lpVtbl -> Terminate(This) ) 

#endif /* COBJMACROS */


#endif 	/* C style interface */




#endif 	/* __IVsHiddenTextSession_INTERFACE_DEFINED__ */


#ifndef __IVsOutliningSession_INTERFACE_DEFINED__
#define __IVsOutliningSession_INTERFACE_DEFINED__

/* interface IVsOutliningSession */
/* [object][uuid] */ 


EXTERN_C const IID IID_IVsOutliningSession;

#if defined(__cplusplus) && !defined(CINTERFACE)
    
    MIDL_INTERFACE("9A436975-A86A-4459-A350-B05604B53309")
    IVsOutliningSession : public IUnknown
    {
    public:
        virtual HRESULT STDMETHODCALLTYPE AddOutlineRegions( 
            /* [in] */ DWORD dwOutliningFlags,
            /* [in] */ long cRegions,
            /* [size_is][in] */ __RPC__in_ecount_full(cRegions) NewOutlineRegion *rgOutlnReg) = 0;
        
    };
    
#else 	/* C style interface */

    typedef struct IVsOutliningSessionVtbl
    {
        BEGIN_INTERFACE
        
        HRESULT ( STDMETHODCALLTYPE *QueryInterface )( 
            IVsOutliningSession * This,
            /* [in] */ __RPC__in REFIID riid,
            /* [iid_is][out] */ 
            __RPC__deref_out  void **ppvObject);
        
        ULONG ( STDMETHODCALLTYPE *AddRef )( 
            IVsOutliningSession * This);
        
        ULONG ( STDMETHODCALLTYPE *Release )( 
            IVsOutliningSession * This);
        
        HRESULT ( STDMETHODCALLTYPE *AddOutlineRegions )( 
            IVsOutliningSession * This,
            /* [in] */ DWORD dwOutliningFlags,
            /* [in] */ long cRegions,
            /* [size_is][in] */ __RPC__in_ecount_full(cRegions) NewOutlineRegion *rgOutlnReg);
        
        END_INTERFACE
    } IVsOutliningSessionVtbl;

    interface IVsOutliningSession
    {
        CONST_VTBL struct IVsOutliningSessionVtbl *lpVtbl;
    };

    

#ifdef COBJMACROS


#define IVsOutliningSession_QueryInterface(This,riid,ppvObject)	\
    ( (This)->lpVtbl -> QueryInterface(This,riid,ppvObject) ) 

#define IVsOutliningSession_AddRef(This)	\
    ( (This)->lpVtbl -> AddRef(This) ) 

#define IVsOutliningSession_Release(This)	\
    ( (This)->lpVtbl -> Release(This) ) 


#define IVsOutliningSession_AddOutlineRegions(This,dwOutliningFlags,cRegions,rgOutlnReg)	\
    ( (This)->lpVtbl -> AddOutlineRegions(This,dwOutliningFlags,cRegions,rgOutlnReg) ) 

#endif /* COBJMACROS */


#endif 	/* C style interface */




#endif 	/* __IVsOutliningSession_INTERFACE_DEFINED__ */


#ifndef __IVsOutliningCapableLanguage_INTERFACE_DEFINED__
#define __IVsOutliningCapableLanguage_INTERFACE_DEFINED__

/* interface IVsOutliningCapableLanguage */
/* [object][uuid] */ 


EXTERN_C const IID IID_IVsOutliningCapableLanguage;

#if defined(__cplusplus) && !defined(CINTERFACE)
    
    MIDL_INTERFACE("B97D5C98-0507-40FA-BF89-CE4F73EBA651")
    IVsOutliningCapableLanguage : public IUnknown
    {
    public:
        virtual HRESULT STDMETHODCALLTYPE CollapseToDefinitions( 
            /* [in] */ __RPC__in_opt IVsTextLines *pTextLines,
            /* [in] */ __RPC__in_opt IVsOutliningSession *pSession) = 0;
        
    };
    
#else 	/* C style interface */

    typedef struct IVsOutliningCapableLanguageVtbl
    {
        BEGIN_INTERFACE
        
        HRESULT ( STDMETHODCALLTYPE *QueryInterface )( 
            IVsOutliningCapableLanguage * This,
            /* [in] */ __RPC__in REFIID riid,
            /* [iid_is][out] */ 
            __RPC__deref_out  void **ppvObject);
        
        ULONG ( STDMETHODCALLTYPE *AddRef )( 
            IVsOutliningCapableLanguage * This);
        
        ULONG ( STDMETHODCALLTYPE *Release )( 
            IVsOutliningCapableLanguage * This);
        
        HRESULT ( STDMETHODCALLTYPE *CollapseToDefinitions )( 
            IVsOutliningCapableLanguage * This,
            /* [in] */ __RPC__in_opt IVsTextLines *pTextLines,
            /* [in] */ __RPC__in_opt IVsOutliningSession *pSession);
        
        END_INTERFACE
    } IVsOutliningCapableLanguageVtbl;

    interface IVsOutliningCapableLanguage
    {
        CONST_VTBL struct IVsOutliningCapableLanguageVtbl *lpVtbl;
    };

    

#ifdef COBJMACROS


#define IVsOutliningCapableLanguage_QueryInterface(This,riid,ppvObject)	\
    ( (This)->lpVtbl -> QueryInterface(This,riid,ppvObject) ) 

#define IVsOutliningCapableLanguage_AddRef(This)	\
    ( (This)->lpVtbl -> AddRef(This) ) 

#define IVsOutliningCapableLanguage_Release(This)	\
    ( (This)->lpVtbl -> Release(This) ) 


#define IVsOutliningCapableLanguage_CollapseToDefinitions(This,pTextLines,pSession)	\
    ( (This)->lpVtbl -> CollapseToDefinitions(This,pTextLines,pSession) ) 

#endif /* COBJMACROS */


#endif 	/* C style interface */




#endif 	/* __IVsOutliningCapableLanguage_INTERFACE_DEFINED__ */


#ifndef __IVsTextLayerEvents_INTERFACE_DEFINED__
#define __IVsTextLayerEvents_INTERFACE_DEFINED__

/* interface IVsTextLayerEvents */
/* [object][uuid] */ 


EXTERN_C const IID IID_IVsTextLayerEvents;

#if defined(__cplusplus) && !defined(CINTERFACE)
    
    MIDL_INTERFACE("CDE3413A-AD8E-495F-A74F-760547294A6F")
    IVsTextLayerEvents : public IUnknown
    {
    public:
        virtual void STDMETHODCALLTYPE OnMappingChange( void) = 0;
        
        virtual void STDMETHODCALLTYPE OnMappingChangeComplete( void) = 0;
        
    };
    
#else 	/* C style interface */

    typedef struct IVsTextLayerEventsVtbl
    {
        BEGIN_INTERFACE
        
        HRESULT ( STDMETHODCALLTYPE *QueryInterface )( 
            IVsTextLayerEvents * This,
            /* [in] */ __RPC__in REFIID riid,
            /* [iid_is][out] */ 
            __RPC__deref_out  void **ppvObject);
        
        ULONG ( STDMETHODCALLTYPE *AddRef )( 
            IVsTextLayerEvents * This);
        
        ULONG ( STDMETHODCALLTYPE *Release )( 
            IVsTextLayerEvents * This);
        
        void ( STDMETHODCALLTYPE *OnMappingChange )( 
            IVsTextLayerEvents * This);
        
        void ( STDMETHODCALLTYPE *OnMappingChangeComplete )( 
            IVsTextLayerEvents * This);
        
        END_INTERFACE
    } IVsTextLayerEventsVtbl;

    interface IVsTextLayerEvents
    {
        CONST_VTBL struct IVsTextLayerEventsVtbl *lpVtbl;
    };

    

#ifdef COBJMACROS


#define IVsTextLayerEvents_QueryInterface(This,riid,ppvObject)	\
    ( (This)->lpVtbl -> QueryInterface(This,riid,ppvObject) ) 

#define IVsTextLayerEvents_AddRef(This)	\
    ( (This)->lpVtbl -> AddRef(This) ) 

#define IVsTextLayerEvents_Release(This)	\
    ( (This)->lpVtbl -> Release(This) ) 


#define IVsTextLayerEvents_OnMappingChange(This)	\
    ( (This)->lpVtbl -> OnMappingChange(This) ) 

#define IVsTextLayerEvents_OnMappingChangeComplete(This)	\
    ( (This)->lpVtbl -> OnMappingChangeComplete(This) ) 

#endif /* COBJMACROS */


#endif 	/* C style interface */




#endif 	/* __IVsTextLayerEvents_INTERFACE_DEFINED__ */


#ifndef __IVsTextStorage_INTERFACE_DEFINED__
#define __IVsTextStorage_INTERFACE_DEFINED__

/* interface IVsTextStorage */
/* [object][uuid] */ 


EXTERN_C const IID IID_IVsTextStorage;

#if defined(__cplusplus) && !defined(CINTERFACE)
    
    MIDL_INTERFACE("1ABAC269-8FB0-438E-8217-230AC6EB5766")
    IVsTextStorage : public IUnknown
    {
    public:
        virtual HRESULT STDMETHODCALLTYPE Storage_Attach( 
            /* [in] */ __RPC__in_opt IUnknown *pBuffer) = 0;
        
        virtual void STDMETHODCALLTYPE Storage_Detach( void) = 0;
        
        virtual long STDMETHODCALLTYPE Storage_GetLength( void) = 0;
        
        virtual long STDMETHODCALLTYPE Storage_GetLineCount( void) = 0;
        
        virtual HRESULT STDMETHODCALLTYPE Storage_GetLengthOfLine( 
            /* [in] */ long iLine,
            /* [out] */ __RPC__out long *piLength) = 0;
        
        virtual HRESULT STDMETHODCALLTYPE Storage_GetPositionOfLineIndex( 
            /* [in] */ long iLine,
            /* [in] */ CharIndex iIndex,
            /* [out] */ __RPC__out long *piPosition) = 0;
        
        virtual HRESULT STDMETHODCALLTYPE Storage_GetLineIndexOfPosition( 
            /* [in] */ long iPosition,
            /* [out] */ __RPC__out long *piLine,
            /* [out] */ __RPC__out CharIndex *piIndex) = 0;
        
        virtual HRESULT STDMETHODCALLTYPE Storage_Replace( 
            /* [in] */ __RPC__in ChangeInput *pCI,
            /* [out] */ __RPC__out TextSpan *ptsOut) = 0;
        
        virtual HRESULT STDMETHODCALLTYPE Storage_CopyText( 
            /* [in] */ long iStartLine,
            /* [in] */ CharIndex iStartIndex,
            /* [in] */ long iBufMax,
            /* [out] */ __RPC__out WCHAR *pszDest,
            /* [out] */ __RPC__out long *pcCharsCopied) = 0;
        
        virtual HRESULT STDMETHODCALLTYPE Storage_GetCharsInSpan( 
            /* [in] */ __RPC__in TextSpan *pSpanToMeasure,
            /* [out] */ __RPC__out long *piChars) = 0;
        
        virtual HRESULT STDMETHODCALLTYPE Storage_GetLineDataEx( 
            /* [in] */ DWORD dwFlags,
            /* [in] */ long iLine,
            /* [in] */ long iStartIndex,
            /* [in] */ long iEndIndex,
            /* [out] */ __RPC__out LINEDATAEX *pLineData) = 0;
        
        virtual HRESULT STDMETHODCALLTYPE Storage_ReleaseLineDataEx( 
            /* [in] */ __RPC__in LINEDATAEX *pLineData) = 0;
        
        virtual HRESULT STDMETHODCALLTYPE Storage_GetLanguageServiceID( 
            /* [out] */ __RPC__out GUID *pguidLangService) = 0;
        
        virtual HRESULT STDMETHODCALLTYPE Storage_SetLanguageServiceID( 
            /* [in] */ __RPC__in REFGUID guidLangService) = 0;
        
        virtual HRESULT STDMETHODCALLTYPE Storage_RefreshColorization( 
            /* [in] */ long iTopLine,
            /* [in] */ long iBotLine,
            /* [out] */ __RPC__out long *piLastAffectedLine) = 0;
        
        virtual HRESULT STDMETHODCALLTYPE Storage_Find( 
            /* [in] */ __RPC__in const WCHAR *pszText,
            /* [in] */ long iStartLine,
            /* [in] */ CharIndex iStartIndex,
            /* [in] */ long iEndLine,
            /* [in] */ CharIndex iEndIndex,
            /* [in] */ long iFlags,
            /* [out] */ __RPC__out long *piLine,
            /* [out] */ __RPC__out CharIndex *piCol) = 0;
        
    };
    
#else 	/* C style interface */

    typedef struct IVsTextStorageVtbl
    {
        BEGIN_INTERFACE
        
        HRESULT ( STDMETHODCALLTYPE *QueryInterface )( 
            IVsTextStorage * This,
            /* [in] */ __RPC__in REFIID riid,
            /* [iid_is][out] */ 
            __RPC__deref_out  void **ppvObject);
        
        ULONG ( STDMETHODCALLTYPE *AddRef )( 
            IVsTextStorage * This);
        
        ULONG ( STDMETHODCALLTYPE *Release )( 
            IVsTextStorage * This);
        
        HRESULT ( STDMETHODCALLTYPE *Storage_Attach )( 
            IVsTextStorage * This,
            /* [in] */ __RPC__in_opt IUnknown *pBuffer);
        
        void ( STDMETHODCALLTYPE *Storage_Detach )( 
            IVsTextStorage * This);
        
        long ( STDMETHODCALLTYPE *Storage_GetLength )( 
            IVsTextStorage * This);
        
        long ( STDMETHODCALLTYPE *Storage_GetLineCount )( 
            IVsTextStorage * This);
        
        HRESULT ( STDMETHODCALLTYPE *Storage_GetLengthOfLine )( 
            IVsTextStorage * This,
            /* [in] */ long iLine,
            /* [out] */ __RPC__out long *piLength);
        
        HRESULT ( STDMETHODCALLTYPE *Storage_GetPositionOfLineIndex )( 
            IVsTextStorage * This,
            /* [in] */ long iLine,
            /* [in] */ CharIndex iIndex,
            /* [out] */ __RPC__out long *piPosition);
        
        HRESULT ( STDMETHODCALLTYPE *Storage_GetLineIndexOfPosition )( 
            IVsTextStorage * This,
            /* [in] */ long iPosition,
            /* [out] */ __RPC__out long *piLine,
            /* [out] */ __RPC__out CharIndex *piIndex);
        
        HRESULT ( STDMETHODCALLTYPE *Storage_Replace )( 
            IVsTextStorage * This,
            /* [in] */ __RPC__in ChangeInput *pCI,
            /* [out] */ __RPC__out TextSpan *ptsOut);
        
        HRESULT ( STDMETHODCALLTYPE *Storage_CopyText )( 
            IVsTextStorage * This,
            /* [in] */ long iStartLine,
            /* [in] */ CharIndex iStartIndex,
            /* [in] */ long iBufMax,
            /* [out] */ __RPC__out WCHAR *pszDest,
            /* [out] */ __RPC__out long *pcCharsCopied);
        
        HRESULT ( STDMETHODCALLTYPE *Storage_GetCharsInSpan )( 
            IVsTextStorage * This,
            /* [in] */ __RPC__in TextSpan *pSpanToMeasure,
            /* [out] */ __RPC__out long *piChars);
        
        HRESULT ( STDMETHODCALLTYPE *Storage_GetLineDataEx )( 
            IVsTextStorage * This,
            /* [in] */ DWORD dwFlags,
            /* [in] */ long iLine,
            /* [in] */ long iStartIndex,
            /* [in] */ long iEndIndex,
            /* [out] */ __RPC__out LINEDATAEX *pLineData);
        
        HRESULT ( STDMETHODCALLTYPE *Storage_ReleaseLineDataEx )( 
            IVsTextStorage * This,
            /* [in] */ __RPC__in LINEDATAEX *pLineData);
        
        HRESULT ( STDMETHODCALLTYPE *Storage_GetLanguageServiceID )( 
            IVsTextStorage * This,
            /* [out] */ __RPC__out GUID *pguidLangService);
        
        HRESULT ( STDMETHODCALLTYPE *Storage_SetLanguageServiceID )( 
            IVsTextStorage * This,
            /* [in] */ __RPC__in REFGUID guidLangService);
        
        HRESULT ( STDMETHODCALLTYPE *Storage_RefreshColorization )( 
            IVsTextStorage * This,
            /* [in] */ long iTopLine,
            /* [in] */ long iBotLine,
            /* [out] */ __RPC__out long *piLastAffectedLine);
        
        HRESULT ( STDMETHODCALLTYPE *Storage_Find )( 
            IVsTextStorage * This,
            /* [in] */ __RPC__in const WCHAR *pszText,
            /* [in] */ long iStartLine,
            /* [in] */ CharIndex iStartIndex,
            /* [in] */ long iEndLine,
            /* [in] */ CharIndex iEndIndex,
            /* [in] */ long iFlags,
            /* [out] */ __RPC__out long *piLine,
            /* [out] */ __RPC__out CharIndex *piCol);
        
        END_INTERFACE
    } IVsTextStorageVtbl;

    interface IVsTextStorage
    {
        CONST_VTBL struct IVsTextStorageVtbl *lpVtbl;
    };

    

#ifdef COBJMACROS


#define IVsTextStorage_QueryInterface(This,riid,ppvObject)	\
    ( (This)->lpVtbl -> QueryInterface(This,riid,ppvObject) ) 

#define IVsTextStorage_AddRef(This)	\
    ( (This)->lpVtbl -> AddRef(This) ) 

#define IVsTextStorage_Release(This)	\
    ( (This)->lpVtbl -> Release(This) ) 


#define IVsTextStorage_Storage_Attach(This,pBuffer)	\
    ( (This)->lpVtbl -> Storage_Attach(This,pBuffer) ) 

#define IVsTextStorage_Storage_Detach(This)	\
    ( (This)->lpVtbl -> Storage_Detach(This) ) 

#define IVsTextStorage_Storage_GetLength(This)	\
    ( (This)->lpVtbl -> Storage_GetLength(This) ) 

#define IVsTextStorage_Storage_GetLineCount(This)	\
    ( (This)->lpVtbl -> Storage_GetLineCount(This) ) 

#define IVsTextStorage_Storage_GetLengthOfLine(This,iLine,piLength)	\
    ( (This)->lpVtbl -> Storage_GetLengthOfLine(This,iLine,piLength) ) 

#define IVsTextStorage_Storage_GetPositionOfLineIndex(This,iLine,iIndex,piPosition)	\
    ( (This)->lpVtbl -> Storage_GetPositionOfLineIndex(This,iLine,iIndex,piPosition) ) 

#define IVsTextStorage_Storage_GetLineIndexOfPosition(This,iPosition,piLine,piIndex)	\
    ( (This)->lpVtbl -> Storage_GetLineIndexOfPosition(This,iPosition,piLine,piIndex) ) 

#define IVsTextStorage_Storage_Replace(This,pCI,ptsOut)	\
    ( (This)->lpVtbl -> Storage_Replace(This,pCI,ptsOut) ) 

#define IVsTextStorage_Storage_CopyText(This,iStartLine,iStartIndex,iBufMax,pszDest,pcCharsCopied)	\
    ( (This)->lpVtbl -> Storage_CopyText(This,iStartLine,iStartIndex,iBufMax,pszDest,pcCharsCopied) ) 

#define IVsTextStorage_Storage_GetCharsInSpan(This,pSpanToMeasure,piChars)	\
    ( (This)->lpVtbl -> Storage_GetCharsInSpan(This,pSpanToMeasure,piChars) ) 

#define IVsTextStorage_Storage_GetLineDataEx(This,dwFlags,iLine,iStartIndex,iEndIndex,pLineData)	\
    ( (This)->lpVtbl -> Storage_GetLineDataEx(This,dwFlags,iLine,iStartIndex,iEndIndex,pLineData) ) 

#define IVsTextStorage_Storage_ReleaseLineDataEx(This,pLineData)	\
    ( (This)->lpVtbl -> Storage_ReleaseLineDataEx(This,pLineData) ) 

#define IVsTextStorage_Storage_GetLanguageServiceID(This,pguidLangService)	\
    ( (This)->lpVtbl -> Storage_GetLanguageServiceID(This,pguidLangService) ) 

#define IVsTextStorage_Storage_SetLanguageServiceID(This,guidLangService)	\
    ( (This)->lpVtbl -> Storage_SetLanguageServiceID(This,guidLangService) ) 

#define IVsTextStorage_Storage_RefreshColorization(This,iTopLine,iBotLine,piLastAffectedLine)	\
    ( (This)->lpVtbl -> Storage_RefreshColorization(This,iTopLine,iBotLine,piLastAffectedLine) ) 

#define IVsTextStorage_Storage_Find(This,pszText,iStartLine,iStartIndex,iEndLine,iEndIndex,iFlags,piLine,piCol)	\
    ( (This)->lpVtbl -> Storage_Find(This,pszText,iStartLine,iStartIndex,iEndLine,iEndIndex,iFlags,piLine,piCol) ) 

#endif /* COBJMACROS */


#endif 	/* C style interface */




#endif 	/* __IVsTextStorage_INTERFACE_DEFINED__ */


#ifndef __IVsPersistentTextImage_INTERFACE_DEFINED__
#define __IVsPersistentTextImage_INTERFACE_DEFINED__

/* interface IVsPersistentTextImage */
/* [object][uuid] */ 


EXTERN_C const IID IID_IVsPersistentTextImage;

#if defined(__cplusplus) && !defined(CINTERFACE)
    
    MIDL_INTERFACE("C47FFD1E-D84E-43A5-8CBE-E4AD06BE795B")
    IVsPersistentTextImage : public IUnknown
    {
    public:
        virtual void *STDMETHODCALLTYPE Image_Alloc( 
            /* [in] */ int iSize) = 0;
        
        virtual void *STDMETHODCALLTYPE Image_Realloc( 
            /* [out][in] */ __RPC__inout void *p,
            /* [in] */ int iSize) = 0;
        
        virtual void STDMETHODCALLTYPE Image_Free( 
            /* [in] */ __RPC__in void *p) = 0;
        
        virtual long STDMETHODCALLTYPE Image_GetLength( void) = 0;
        
        virtual HRESULT STDMETHODCALLTYPE Image_LoadText( 
            /* [in] */ __RPC__in const WCHAR *pszText,
            /* [in] */ INT iLength,
            /* [in] */ DWORD dwFlags) = 0;
        
        virtual HRESULT STDMETHODCALLTYPE Image_OpenFullTextScan( void) = 0;
        
        virtual HRESULT STDMETHODCALLTYPE Image_FullTextRead( 
            /* [out] */ __RPC__deref_out_opt const WCHAR **ppszText,
            /* [out] */ __RPC__out long *piLength) = 0;
        
        virtual HRESULT STDMETHODCALLTYPE Image_CloseFullTextScan( void) = 0;
        
    };
    
#else 	/* C style interface */

    typedef struct IVsPersistentTextImageVtbl
    {
        BEGIN_INTERFACE
        
        HRESULT ( STDMETHODCALLTYPE *QueryInterface )( 
            IVsPersistentTextImage * This,
            /* [in] */ __RPC__in REFIID riid,
            /* [iid_is][out] */ 
            __RPC__deref_out  void **ppvObject);
        
        ULONG ( STDMETHODCALLTYPE *AddRef )( 
            IVsPersistentTextImage * This);
        
        ULONG ( STDMETHODCALLTYPE *Release )( 
            IVsPersistentTextImage * This);
        
        void *( STDMETHODCALLTYPE *Image_Alloc )( 
            IVsPersistentTextImage * This,
            /* [in] */ int iSize);
        
        void *( STDMETHODCALLTYPE *Image_Realloc )( 
            IVsPersistentTextImage * This,
            /* [out][in] */ __RPC__inout void *p,
            /* [in] */ int iSize);
        
        void ( STDMETHODCALLTYPE *Image_Free )( 
            IVsPersistentTextImage * This,
            /* [in] */ __RPC__in void *p);
        
        long ( STDMETHODCALLTYPE *Image_GetLength )( 
            IVsPersistentTextImage * This);
        
        HRESULT ( STDMETHODCALLTYPE *Image_LoadText )( 
            IVsPersistentTextImage * This,
            /* [in] */ __RPC__in const WCHAR *pszText,
            /* [in] */ INT iLength,
            /* [in] */ DWORD dwFlags);
        
        HRESULT ( STDMETHODCALLTYPE *Image_OpenFullTextScan )( 
            IVsPersistentTextImage * This);
        
        HRESULT ( STDMETHODCALLTYPE *Image_FullTextRead )( 
            IVsPersistentTextImage * This,
            /* [out] */ __RPC__deref_out_opt const WCHAR **ppszText,
            /* [out] */ __RPC__out long *piLength);
        
        HRESULT ( STDMETHODCALLTYPE *Image_CloseFullTextScan )( 
            IVsPersistentTextImage * This);
        
        END_INTERFACE
    } IVsPersistentTextImageVtbl;

    interface IVsPersistentTextImage
    {
        CONST_VTBL struct IVsPersistentTextImageVtbl *lpVtbl;
    };

    

#ifdef COBJMACROS


#define IVsPersistentTextImage_QueryInterface(This,riid,ppvObject)	\
    ( (This)->lpVtbl -> QueryInterface(This,riid,ppvObject) ) 

#define IVsPersistentTextImage_AddRef(This)	\
    ( (This)->lpVtbl -> AddRef(This) ) 

#define IVsPersistentTextImage_Release(This)	\
    ( (This)->lpVtbl -> Release(This) ) 


#define IVsPersistentTextImage_Image_Alloc(This,iSize)	\
    ( (This)->lpVtbl -> Image_Alloc(This,iSize) ) 

#define IVsPersistentTextImage_Image_Realloc(This,p,iSize)	\
    ( (This)->lpVtbl -> Image_Realloc(This,p,iSize) ) 

#define IVsPersistentTextImage_Image_Free(This,p)	\
    ( (This)->lpVtbl -> Image_Free(This,p) ) 

#define IVsPersistentTextImage_Image_GetLength(This)	\
    ( (This)->lpVtbl -> Image_GetLength(This) ) 

#define IVsPersistentTextImage_Image_LoadText(This,pszText,iLength,dwFlags)	\
    ( (This)->lpVtbl -> Image_LoadText(This,pszText,iLength,dwFlags) ) 

#define IVsPersistentTextImage_Image_OpenFullTextScan(This)	\
    ( (This)->lpVtbl -> Image_OpenFullTextScan(This) ) 

#define IVsPersistentTextImage_Image_FullTextRead(This,ppszText,piLength)	\
    ( (This)->lpVtbl -> Image_FullTextRead(This,ppszText,piLength) ) 

#define IVsPersistentTextImage_Image_CloseFullTextScan(This)	\
    ( (This)->lpVtbl -> Image_CloseFullTextScan(This) ) 

#endif /* COBJMACROS */


#endif 	/* C style interface */




#endif 	/* __IVsPersistentTextImage_INTERFACE_DEFINED__ */


#ifndef __IVsTextStorageColorState_INTERFACE_DEFINED__
#define __IVsTextStorageColorState_INTERFACE_DEFINED__

/* interface IVsTextStorageColorState */
/* [object][uuid] */ 


EXTERN_C const IID IID_IVsTextStorageColorState;

#if defined(__cplusplus) && !defined(CINTERFACE)
    
    MIDL_INTERFACE("1F027F1F-9696-4816-B345-3CD7D3122A48")
    IVsTextStorageColorState : public IUnknown
    {
    public:
        virtual HRESULT STDMETHODCALLTYPE GetColorStateAtStartOfLine( 
            /* [in] */ long iLine,
            /* [out] */ __RPC__out long *piState) = 0;
        
    };
    
#else 	/* C style interface */

    typedef struct IVsTextStorageColorStateVtbl
    {
        BEGIN_INTERFACE
        
        HRESULT ( STDMETHODCALLTYPE *QueryInterface )( 
            IVsTextStorageColorState * This,
            /* [in] */ __RPC__in REFIID riid,
            /* [iid_is][out] */ 
            __RPC__deref_out  void **ppvObject);
        
        ULONG ( STDMETHODCALLTYPE *AddRef )( 
            IVsTextStorageColorState * This);
        
        ULONG ( STDMETHODCALLTYPE *Release )( 
            IVsTextStorageColorState * This);
        
        HRESULT ( STDMETHODCALLTYPE *GetColorStateAtStartOfLine )( 
            IVsTextStorageColorState * This,
            /* [in] */ long iLine,
            /* [out] */ __RPC__out long *piState);
        
        END_INTERFACE
    } IVsTextStorageColorStateVtbl;

    interface IVsTextStorageColorState
    {
        CONST_VTBL struct IVsTextStorageColorStateVtbl *lpVtbl;
    };

    

#ifdef COBJMACROS


#define IVsTextStorageColorState_QueryInterface(This,riid,ppvObject)	\
    ( (This)->lpVtbl -> QueryInterface(This,riid,ppvObject) ) 

#define IVsTextStorageColorState_AddRef(This)	\
    ( (This)->lpVtbl -> AddRef(This) ) 

#define IVsTextStorageColorState_Release(This)	\
    ( (This)->lpVtbl -> Release(This) ) 


#define IVsTextStorageColorState_GetColorStateAtStartOfLine(This,iLine,piState)	\
    ( (This)->lpVtbl -> GetColorStateAtStartOfLine(This,iLine,piState) ) 

#endif /* COBJMACROS */


#endif 	/* C style interface */




#endif 	/* __IVsTextStorageColorState_INTERFACE_DEFINED__ */


#ifndef __IVsFinalTextChangeCommitEvents_INTERFACE_DEFINED__
#define __IVsFinalTextChangeCommitEvents_INTERFACE_DEFINED__

/* interface IVsFinalTextChangeCommitEvents */
/* [object][uuid] */ 


EXTERN_C const IID IID_IVsFinalTextChangeCommitEvents;

#if defined(__cplusplus) && !defined(CINTERFACE)
    
    MIDL_INTERFACE("CE62717C-797D-445F-BC89-1D02C54CCF96")
    IVsFinalTextChangeCommitEvents : public IUnknown
    {
    public:
        virtual void STDMETHODCALLTYPE OnChangesCommitted( 
            /* [in] */ DWORD dwGestureFlags,
            /* [in] */ __RPC__in TextSpan *ptsChanged) = 0;
        
    };
    
#else 	/* C style interface */

    typedef struct IVsFinalTextChangeCommitEventsVtbl
    {
        BEGIN_INTERFACE
        
        HRESULT ( STDMETHODCALLTYPE *QueryInterface )( 
            IVsFinalTextChangeCommitEvents * This,
            /* [in] */ __RPC__in REFIID riid,
            /* [iid_is][out] */ 
            __RPC__deref_out  void **ppvObject);
        
        ULONG ( STDMETHODCALLTYPE *AddRef )( 
            IVsFinalTextChangeCommitEvents * This);
        
        ULONG ( STDMETHODCALLTYPE *Release )( 
            IVsFinalTextChangeCommitEvents * This);
        
        void ( STDMETHODCALLTYPE *OnChangesCommitted )( 
            IVsFinalTextChangeCommitEvents * This,
            /* [in] */ DWORD dwGestureFlags,
            /* [in] */ __RPC__in TextSpan *ptsChanged);
        
        END_INTERFACE
    } IVsFinalTextChangeCommitEventsVtbl;

    interface IVsFinalTextChangeCommitEvents
    {
        CONST_VTBL struct IVsFinalTextChangeCommitEventsVtbl *lpVtbl;
    };

    

#ifdef COBJMACROS


#define IVsFinalTextChangeCommitEvents_QueryInterface(This,riid,ppvObject)	\
    ( (This)->lpVtbl -> QueryInterface(This,riid,ppvObject) ) 

#define IVsFinalTextChangeCommitEvents_AddRef(This)	\
    ( (This)->lpVtbl -> AddRef(This) ) 

#define IVsFinalTextChangeCommitEvents_Release(This)	\
    ( (This)->lpVtbl -> Release(This) ) 


#define IVsFinalTextChangeCommitEvents_OnChangesCommitted(This,dwGestureFlags,ptsChanged)	\
    ( (This)->lpVtbl -> OnChangesCommitted(This,dwGestureFlags,ptsChanged) ) 

#endif /* COBJMACROS */


#endif 	/* C style interface */




#endif 	/* __IVsFinalTextChangeCommitEvents_INTERFACE_DEFINED__ */


#ifndef __IVsPreliminaryTextChangeCommitEvents_INTERFACE_DEFINED__
#define __IVsPreliminaryTextChangeCommitEvents_INTERFACE_DEFINED__

/* interface IVsPreliminaryTextChangeCommitEvents */
/* [object][uuid] */ 


EXTERN_C const IID IID_IVsPreliminaryTextChangeCommitEvents;

#if defined(__cplusplus) && !defined(CINTERFACE)
    
    MIDL_INTERFACE("7F57B996-148B-46DE-A261-B1A02E6D9DBA")
    IVsPreliminaryTextChangeCommitEvents : public IVsFinalTextChangeCommitEvents
    {
    public:
    };
    
#else 	/* C style interface */

    typedef struct IVsPreliminaryTextChangeCommitEventsVtbl
    {
        BEGIN_INTERFACE
        
        HRESULT ( STDMETHODCALLTYPE *QueryInterface )( 
            IVsPreliminaryTextChangeCommitEvents * This,
            /* [in] */ __RPC__in REFIID riid,
            /* [iid_is][out] */ 
            __RPC__deref_out  void **ppvObject);
        
        ULONG ( STDMETHODCALLTYPE *AddRef )( 
            IVsPreliminaryTextChangeCommitEvents * This);
        
        ULONG ( STDMETHODCALLTYPE *Release )( 
            IVsPreliminaryTextChangeCommitEvents * This);
        
        void ( STDMETHODCALLTYPE *OnChangesCommitted )( 
            IVsPreliminaryTextChangeCommitEvents * This,
            /* [in] */ DWORD dwGestureFlags,
            /* [in] */ __RPC__in TextSpan *ptsChanged);
        
        END_INTERFACE
    } IVsPreliminaryTextChangeCommitEventsVtbl;

    interface IVsPreliminaryTextChangeCommitEvents
    {
        CONST_VTBL struct IVsPreliminaryTextChangeCommitEventsVtbl *lpVtbl;
    };

    

#ifdef COBJMACROS


#define IVsPreliminaryTextChangeCommitEvents_QueryInterface(This,riid,ppvObject)	\
    ( (This)->lpVtbl -> QueryInterface(This,riid,ppvObject) ) 

#define IVsPreliminaryTextChangeCommitEvents_AddRef(This)	\
    ( (This)->lpVtbl -> AddRef(This) ) 

#define IVsPreliminaryTextChangeCommitEvents_Release(This)	\
    ( (This)->lpVtbl -> Release(This) ) 


#define IVsPreliminaryTextChangeCommitEvents_OnChangesCommitted(This,dwGestureFlags,ptsChanged)	\
    ( (This)->lpVtbl -> OnChangesCommitted(This,dwGestureFlags,ptsChanged) ) 


#endif /* COBJMACROS */


#endif 	/* C style interface */




#endif 	/* __IVsPreliminaryTextChangeCommitEvents_INTERFACE_DEFINED__ */


#ifndef __IVsUndoRedoClusterWithCommitEvents_INTERFACE_DEFINED__
#define __IVsUndoRedoClusterWithCommitEvents_INTERFACE_DEFINED__

/* interface IVsUndoRedoClusterWithCommitEvents */
/* [object][uuid] */ 


EXTERN_C const IID IID_IVsUndoRedoClusterWithCommitEvents;

#if defined(__cplusplus) && !defined(CINTERFACE)
    
    MIDL_INTERFACE("F98298D9-53C4-43DB-90D6-D1A3CA4B1849")
    IVsUndoRedoClusterWithCommitEvents : public IUnknown
    {
    public:
        virtual void STDMETHODCALLTYPE OnUndoRedoClusterClosingWithCommitGesture( 
            /* [in] */ DWORD dwGestureFlags,
            /* [in] */ __RPC__in TextSpan *ptsChanged) = 0;
        
    };
    
#else 	/* C style interface */

    typedef struct IVsUndoRedoClusterWithCommitEventsVtbl
    {
        BEGIN_INTERFACE
        
        HRESULT ( STDMETHODCALLTYPE *QueryInterface )( 
            IVsUndoRedoClusterWithCommitEvents * This,
            /* [in] */ __RPC__in REFIID riid,
            /* [iid_is][out] */ 
            __RPC__deref_out  void **ppvObject);
        
        ULONG ( STDMETHODCALLTYPE *AddRef )( 
            IVsUndoRedoClusterWithCommitEvents * This);
        
        ULONG ( STDMETHODCALLTYPE *Release )( 
            IVsUndoRedoClusterWithCommitEvents * This);
        
        void ( STDMETHODCALLTYPE *OnUndoRedoClusterClosingWithCommitGesture )( 
            IVsUndoRedoClusterWithCommitEvents * This,
            /* [in] */ DWORD dwGestureFlags,
            /* [in] */ __RPC__in TextSpan *ptsChanged);
        
        END_INTERFACE
    } IVsUndoRedoClusterWithCommitEventsVtbl;

    interface IVsUndoRedoClusterWithCommitEvents
    {
        CONST_VTBL struct IVsUndoRedoClusterWithCommitEventsVtbl *lpVtbl;
    };

    

#ifdef COBJMACROS


#define IVsUndoRedoClusterWithCommitEvents_QueryInterface(This,riid,ppvObject)	\
    ( (This)->lpVtbl -> QueryInterface(This,riid,ppvObject) ) 

#define IVsUndoRedoClusterWithCommitEvents_AddRef(This)	\
    ( (This)->lpVtbl -> AddRef(This) ) 

#define IVsUndoRedoClusterWithCommitEvents_Release(This)	\
    ( (This)->lpVtbl -> Release(This) ) 


#define IVsUndoRedoClusterWithCommitEvents_OnUndoRedoClusterClosingWithCommitGesture(This,dwGestureFlags,ptsChanged)	\
    ( (This)->lpVtbl -> OnUndoRedoClusterClosingWithCommitGesture(This,dwGestureFlags,ptsChanged) ) 

#endif /* COBJMACROS */


#endif 	/* C style interface */




#endif 	/* __IVsUndoRedoClusterWithCommitEvents_INTERFACE_DEFINED__ */


#ifndef __IVsTextImageEvents_INTERFACE_DEFINED__
#define __IVsTextImageEvents_INTERFACE_DEFINED__

/* interface IVsTextImageEvents */
/* [object][version][uuid] */ 


EXTERN_C const IID IID_IVsTextImageEvents;

#if defined(__cplusplus) && !defined(CINTERFACE)
    
    MIDL_INTERFACE("B1ADB4FC-562F-43E6-8A91-CD2CFFA2B377")
    IVsTextImageEvents : public IUnknown
    {
    public:
        virtual void STDMETHODCALLTYPE OnTextChange( 
            /* [in] */ TextAddress taStart,
            /* [in] */ TextAddress taEnd,
            /* [in] */ TextAddress taNewEnd) = 0;
        
    };
    
#else 	/* C style interface */

    typedef struct IVsTextImageEventsVtbl
    {
        BEGIN_INTERFACE
        
        HRESULT ( STDMETHODCALLTYPE *QueryInterface )( 
            IVsTextImageEvents * This,
            /* [in] */ __RPC__in REFIID riid,
            /* [iid_is][out] */ 
            __RPC__deref_out  void **ppvObject);
        
        ULONG ( STDMETHODCALLTYPE *AddRef )( 
            IVsTextImageEvents * This);
        
        ULONG ( STDMETHODCALLTYPE *Release )( 
            IVsTextImageEvents * This);
        
        void ( STDMETHODCALLTYPE *OnTextChange )( 
            IVsTextImageEvents * This,
            /* [in] */ TextAddress taStart,
            /* [in] */ TextAddress taEnd,
            /* [in] */ TextAddress taNewEnd);
        
        END_INTERFACE
    } IVsTextImageEventsVtbl;

    interface IVsTextImageEvents
    {
        CONST_VTBL struct IVsTextImageEventsVtbl *lpVtbl;
    };

    

#ifdef COBJMACROS


#define IVsTextImageEvents_QueryInterface(This,riid,ppvObject)	\
    ( (This)->lpVtbl -> QueryInterface(This,riid,ppvObject) ) 

#define IVsTextImageEvents_AddRef(This)	\
    ( (This)->lpVtbl -> AddRef(This) ) 

#define IVsTextImageEvents_Release(This)	\
    ( (This)->lpVtbl -> Release(This) ) 


#define IVsTextImageEvents_OnTextChange(This,taStart,taEnd,taNewEnd)	\
    ( (This)->lpVtbl -> OnTextChange(This,taStart,taEnd,taNewEnd) ) 

#endif /* COBJMACROS */


#endif 	/* C style interface */




#endif 	/* __IVsTextImageEvents_INTERFACE_DEFINED__ */


#ifndef __IVsCommitGestureSink_INTERFACE_DEFINED__
#define __IVsCommitGestureSink_INTERFACE_DEFINED__

/* interface IVsCommitGestureSink */
/* [object][uuid] */ 


EXTERN_C const IID IID_IVsCommitGestureSink;

#if defined(__cplusplus) && !defined(CINTERFACE)
    
    MIDL_INTERFACE("957970B0-71E1-11d3-AFF1-00105A9991EF")
    IVsCommitGestureSink : public IUnknown
    {
    public:
        virtual void STDMETHODCALLTYPE OnCommitGesture( 
            /* [in] */ DWORD dwGestureFlags) = 0;
        
        virtual void STDMETHODCALLTYPE FlushCommitEventIfCurrentClusterIsOutermost( void) = 0;
        
    };
    
#else 	/* C style interface */

    typedef struct IVsCommitGestureSinkVtbl
    {
        BEGIN_INTERFACE
        
        HRESULT ( STDMETHODCALLTYPE *QueryInterface )( 
            IVsCommitGestureSink * This,
            /* [in] */ __RPC__in REFIID riid,
            /* [iid_is][out] */ 
            __RPC__deref_out  void **ppvObject);
        
        ULONG ( STDMETHODCALLTYPE *AddRef )( 
            IVsCommitGestureSink * This);
        
        ULONG ( STDMETHODCALLTYPE *Release )( 
            IVsCommitGestureSink * This);
        
        void ( STDMETHODCALLTYPE *OnCommitGesture )( 
            IVsCommitGestureSink * This,
            /* [in] */ DWORD dwGestureFlags);
        
        void ( STDMETHODCALLTYPE *FlushCommitEventIfCurrentClusterIsOutermost )( 
            IVsCommitGestureSink * This);
        
        END_INTERFACE
    } IVsCommitGestureSinkVtbl;

    interface IVsCommitGestureSink
    {
        CONST_VTBL struct IVsCommitGestureSinkVtbl *lpVtbl;
    };

    

#ifdef COBJMACROS


#define IVsCommitGestureSink_QueryInterface(This,riid,ppvObject)	\
    ( (This)->lpVtbl -> QueryInterface(This,riid,ppvObject) ) 

#define IVsCommitGestureSink_AddRef(This)	\
    ( (This)->lpVtbl -> AddRef(This) ) 

#define IVsCommitGestureSink_Release(This)	\
    ( (This)->lpVtbl -> Release(This) ) 


#define IVsCommitGestureSink_OnCommitGesture(This,dwGestureFlags)	\
    ( (This)->lpVtbl -> OnCommitGesture(This,dwGestureFlags) ) 

#define IVsCommitGestureSink_FlushCommitEventIfCurrentClusterIsOutermost(This)	\
    ( (This)->lpVtbl -> FlushCommitEventIfCurrentClusterIsOutermost(This) ) 

#endif /* COBJMACROS */


#endif 	/* C style interface */




#endif 	/* __IVsCommitGestureSink_INTERFACE_DEFINED__ */


#ifndef __IVsTextImage_INTERFACE_DEFINED__
#define __IVsTextImage_INTERFACE_DEFINED__

/* interface IVsTextImage */
/* [object][unique][version][uuid] */ 


EXTERN_C const IID IID_IVsTextImage;

#if defined(__cplusplus) && !defined(CINTERFACE)
    
    MIDL_INTERFACE("67068DC0-C069-11d2-936C-D714766E8B50")
    IVsTextImage : public IUnknown
    {
    public:
        virtual HRESULT STDMETHODCALLTYPE GetCharSize( 
            /* [retval][out] */ __RPC__out LONG *pcch) = 0;
        
        virtual HRESULT STDMETHODCALLTYPE GetLineSize( 
            /* [retval][out] */ __RPC__out LONG *pcLines) = 0;
        
        virtual HRESULT STDMETHODCALLTYPE GetOffsetOfTextAddress( 
            /* [in] */ TextAddress ta,
            /* [retval][out] */ __RPC__out LONG *piOffset) = 0;
        
        virtual HRESULT STDMETHODCALLTYPE GetTextAddressOfOffset( 
            /* [in] */ LONG iOffset,
            /* [retval][out] */ __RPC__out TextAddress *pta) = 0;
        
        virtual HRESULT STDMETHODCALLTYPE Replace( 
            /* [in] */ DWORD dwFlags,
            /* [in] */ __RPC__in const TextSpan *pts,
            /* [in] */ LONG cch,
            /* [size_is][in] */ __RPC__in_ecount_full(cch) LPCOLESTR pchText,
            /* [retval][out] */ __RPC__out TextSpan *ptsChanged) = 0;
        
        virtual HRESULT STDMETHODCALLTYPE GetSpanLength( 
            /* [in] */ __RPC__in const TextSpan *pts,
            /* [retval][out] */ __RPC__out LONG *pcch) = 0;
        
        virtual HRESULT STDMETHODCALLTYPE GetTextBSTR( 
            /* [in] */ __RPC__in const TextSpan *pts,
            /* [retval][out] */ __RPC__deref_out_opt BSTR *pbstrText) = 0;
        
        virtual HRESULT STDMETHODCALLTYPE GetText( 
            /* [in] */ __RPC__in const TextSpan *pts,
            /* [in] */ LONG cch,
            /* [size_is][out] */ __RPC__out_ecount_full(cch) LPOLESTR psz) = 0;
        
        virtual HRESULT STDMETHODCALLTYPE GetLineLength( 
            /* [in] */ LONG iLine,
            /* [retval][out] */ __RPC__out LONG *piLength) = 0;
        
        virtual HRESULT STDMETHODCALLTYPE GetLine( 
            /* [in] */ DWORD grfGet,
            /* [in] */ LONG iLine,
            /* [in] */ LONG iStartIndex,
            /* [in] */ LONG iEndIndex,
            /* [retval][out] */ __RPC__out LINEDATAEX *pLineData) = 0;
        
        virtual HRESULT STDMETHODCALLTYPE ReleaseLine( 
            /* [in] */ __RPC__in LINEDATAEX *pLineData) = 0;
        
        virtual HRESULT STDMETHODCALLTYPE AdviseTextImageEvents( 
            /* [in] */ __RPC__in_opt IVsTextImageEvents *pSink,
            /* [retval][out] */ __RPC__out DWORD *pCookie) = 0;
        
        virtual HRESULT STDMETHODCALLTYPE UnadviseTextImageEvents( 
            /* [in] */ DWORD Cookie) = 0;
        
        virtual HRESULT STDMETHODCALLTYPE LockImage( 
            /* [in] */ DWORD grfLock) = 0;
        
        virtual HRESULT STDMETHODCALLTYPE UnlockImage( 
            /* [in] */ DWORD grfLock) = 0;
        
    };
    
#else 	/* C style interface */

    typedef struct IVsTextImageVtbl
    {
        BEGIN_INTERFACE
        
        HRESULT ( STDMETHODCALLTYPE *QueryInterface )( 
            IVsTextImage * This,
            /* [in] */ __RPC__in REFIID riid,
            /* [iid_is][out] */ 
            __RPC__deref_out  void **ppvObject);
        
        ULONG ( STDMETHODCALLTYPE *AddRef )( 
            IVsTextImage * This);
        
        ULONG ( STDMETHODCALLTYPE *Release )( 
            IVsTextImage * This);
        
        HRESULT ( STDMETHODCALLTYPE *GetCharSize )( 
            IVsTextImage * This,
            /* [retval][out] */ __RPC__out LONG *pcch);
        
        HRESULT ( STDMETHODCALLTYPE *GetLineSize )( 
            IVsTextImage * This,
            /* [retval][out] */ __RPC__out LONG *pcLines);
        
        HRESULT ( STDMETHODCALLTYPE *GetOffsetOfTextAddress )( 
            IVsTextImage * This,
            /* [in] */ TextAddress ta,
            /* [retval][out] */ __RPC__out LONG *piOffset);
        
        HRESULT ( STDMETHODCALLTYPE *GetTextAddressOfOffset )( 
            IVsTextImage * This,
            /* [in] */ LONG iOffset,
            /* [retval][out] */ __RPC__out TextAddress *pta);
        
        HRESULT ( STDMETHODCALLTYPE *Replace )( 
            IVsTextImage * This,
            /* [in] */ DWORD dwFlags,
            /* [in] */ __RPC__in const TextSpan *pts,
            /* [in] */ LONG cch,
            /* [size_is][in] */ __RPC__in_ecount_full(cch) LPCOLESTR pchText,
            /* [retval][out] */ __RPC__out TextSpan *ptsChanged);
        
        HRESULT ( STDMETHODCALLTYPE *GetSpanLength )( 
            IVsTextImage * This,
            /* [in] */ __RPC__in const TextSpan *pts,
            /* [retval][out] */ __RPC__out LONG *pcch);
        
        HRESULT ( STDMETHODCALLTYPE *GetTextBSTR )( 
            IVsTextImage * This,
            /* [in] */ __RPC__in const TextSpan *pts,
            /* [retval][out] */ __RPC__deref_out_opt BSTR *pbstrText);
        
        HRESULT ( STDMETHODCALLTYPE *GetText )( 
            IVsTextImage * This,
            /* [in] */ __RPC__in const TextSpan *pts,
            /* [in] */ LONG cch,
            /* [size_is][out] */ __RPC__out_ecount_full(cch) LPOLESTR psz);
        
        HRESULT ( STDMETHODCALLTYPE *GetLineLength )( 
            IVsTextImage * This,
            /* [in] */ LONG iLine,
            /* [retval][out] */ __RPC__out LONG *piLength);
        
        HRESULT ( STDMETHODCALLTYPE *GetLine )( 
            IVsTextImage * This,
            /* [in] */ DWORD grfGet,
            /* [in] */ LONG iLine,
            /* [in] */ LONG iStartIndex,
            /* [in] */ LONG iEndIndex,
            /* [retval][out] */ __RPC__out LINEDATAEX *pLineData);
        
        HRESULT ( STDMETHODCALLTYPE *ReleaseLine )( 
            IVsTextImage * This,
            /* [in] */ __RPC__in LINEDATAEX *pLineData);
        
        HRESULT ( STDMETHODCALLTYPE *AdviseTextImageEvents )( 
            IVsTextImage * This,
            /* [in] */ __RPC__in_opt IVsTextImageEvents *pSink,
            /* [retval][out] */ __RPC__out DWORD *pCookie);
        
        HRESULT ( STDMETHODCALLTYPE *UnadviseTextImageEvents )( 
            IVsTextImage * This,
            /* [in] */ DWORD Cookie);
        
        HRESULT ( STDMETHODCALLTYPE *LockImage )( 
            IVsTextImage * This,
            /* [in] */ DWORD grfLock);
        
        HRESULT ( STDMETHODCALLTYPE *UnlockImage )( 
            IVsTextImage * This,
            /* [in] */ DWORD grfLock);
        
        END_INTERFACE
    } IVsTextImageVtbl;

    interface IVsTextImage
    {
        CONST_VTBL struct IVsTextImageVtbl *lpVtbl;
    };

    

#ifdef COBJMACROS


#define IVsTextImage_QueryInterface(This,riid,ppvObject)	\
    ( (This)->lpVtbl -> QueryInterface(This,riid,ppvObject) ) 

#define IVsTextImage_AddRef(This)	\
    ( (This)->lpVtbl -> AddRef(This) ) 

#define IVsTextImage_Release(This)	\
    ( (This)->lpVtbl -> Release(This) ) 


#define IVsTextImage_GetCharSize(This,pcch)	\
    ( (This)->lpVtbl -> GetCharSize(This,pcch) ) 

#define IVsTextImage_GetLineSize(This,pcLines)	\
    ( (This)->lpVtbl -> GetLineSize(This,pcLines) ) 

#define IVsTextImage_GetOffsetOfTextAddress(This,ta,piOffset)	\
    ( (This)->lpVtbl -> GetOffsetOfTextAddress(This,ta,piOffset) ) 

#define IVsTextImage_GetTextAddressOfOffset(This,iOffset,pta)	\
    ( (This)->lpVtbl -> GetTextAddressOfOffset(This,iOffset,pta) ) 

#define IVsTextImage_Replace(This,dwFlags,pts,cch,pchText,ptsChanged)	\
    ( (This)->lpVtbl -> Replace(This,dwFlags,pts,cch,pchText,ptsChanged) ) 

#define IVsTextImage_GetSpanLength(This,pts,pcch)	\
    ( (This)->lpVtbl -> GetSpanLength(This,pts,pcch) ) 

#define IVsTextImage_GetTextBSTR(This,pts,pbstrText)	\
    ( (This)->lpVtbl -> GetTextBSTR(This,pts,pbstrText) ) 

#define IVsTextImage_GetText(This,pts,cch,psz)	\
    ( (This)->lpVtbl -> GetText(This,pts,cch,psz) ) 

#define IVsTextImage_GetLineLength(This,iLine,piLength)	\
    ( (This)->lpVtbl -> GetLineLength(This,iLine,piLength) ) 

#define IVsTextImage_GetLine(This,grfGet,iLine,iStartIndex,iEndIndex,pLineData)	\
    ( (This)->lpVtbl -> GetLine(This,grfGet,iLine,iStartIndex,iEndIndex,pLineData) ) 

#define IVsTextImage_ReleaseLine(This,pLineData)	\
    ( (This)->lpVtbl -> ReleaseLine(This,pLineData) ) 

#define IVsTextImage_AdviseTextImageEvents(This,pSink,pCookie)	\
    ( (This)->lpVtbl -> AdviseTextImageEvents(This,pSink,pCookie) ) 

#define IVsTextImage_UnadviseTextImageEvents(This,Cookie)	\
    ( (This)->lpVtbl -> UnadviseTextImageEvents(This,Cookie) ) 

#define IVsTextImage_LockImage(This,grfLock)	\
    ( (This)->lpVtbl -> LockImage(This,grfLock) ) 

#define IVsTextImage_UnlockImage(This,grfLock)	\
    ( (This)->lpVtbl -> UnlockImage(This,grfLock) ) 

#endif /* COBJMACROS */


#endif 	/* C style interface */




#endif 	/* __IVsTextImage_INTERFACE_DEFINED__ */


#ifndef __IVsEditorGoBackLocations_INTERFACE_DEFINED__
#define __IVsEditorGoBackLocations_INTERFACE_DEFINED__

/* interface IVsEditorGoBackLocations */
/* [object][uuid] */ 


EXTERN_C const IID IID_IVsEditorGoBackLocations;

#if defined(__cplusplus) && !defined(CINTERFACE)
    
    MIDL_INTERFACE("714FFF37-5CDC-424a-BCB4-CA50800524F1")
    IVsEditorGoBackLocations : public IUnknown
    {
    public:
        virtual HRESULT STDMETHODCALLTYPE SetNonMergeableGoBackLocation( 
            /* [in] */ BOOL fCurrentCaretPos,
            /* [in] */ long iBaseLine,
            /* [in] */ long iBaseCol) = 0;
        
        virtual HRESULT STDMETHODCALLTYPE SetMergeableGoBackLocation( 
            /* [in] */ BOOL fCurrentCaretPos,
            /* [in] */ long iBaseLine,
            /* [in] */ long iBaseCol) = 0;
        
    };
    
#else 	/* C style interface */

    typedef struct IVsEditorGoBackLocationsVtbl
    {
        BEGIN_INTERFACE
        
        HRESULT ( STDMETHODCALLTYPE *QueryInterface )( 
            IVsEditorGoBackLocations * This,
            /* [in] */ __RPC__in REFIID riid,
            /* [iid_is][out] */ 
            __RPC__deref_out  void **ppvObject);
        
        ULONG ( STDMETHODCALLTYPE *AddRef )( 
            IVsEditorGoBackLocations * This);
        
        ULONG ( STDMETHODCALLTYPE *Release )( 
            IVsEditorGoBackLocations * This);
        
        HRESULT ( STDMETHODCALLTYPE *SetNonMergeableGoBackLocation )( 
            IVsEditorGoBackLocations * This,
            /* [in] */ BOOL fCurrentCaretPos,
            /* [in] */ long iBaseLine,
            /* [in] */ long iBaseCol);
        
        HRESULT ( STDMETHODCALLTYPE *SetMergeableGoBackLocation )( 
            IVsEditorGoBackLocations * This,
            /* [in] */ BOOL fCurrentCaretPos,
            /* [in] */ long iBaseLine,
            /* [in] */ long iBaseCol);
        
        END_INTERFACE
    } IVsEditorGoBackLocationsVtbl;

    interface IVsEditorGoBackLocations
    {
        CONST_VTBL struct IVsEditorGoBackLocationsVtbl *lpVtbl;
    };

    

#ifdef COBJMACROS


#define IVsEditorGoBackLocations_QueryInterface(This,riid,ppvObject)	\
    ( (This)->lpVtbl -> QueryInterface(This,riid,ppvObject) ) 

#define IVsEditorGoBackLocations_AddRef(This)	\
    ( (This)->lpVtbl -> AddRef(This) ) 

#define IVsEditorGoBackLocations_Release(This)	\
    ( (This)->lpVtbl -> Release(This) ) 


#define IVsEditorGoBackLocations_SetNonMergeableGoBackLocation(This,fCurrentCaretPos,iBaseLine,iBaseCol)	\
    ( (This)->lpVtbl -> SetNonMergeableGoBackLocation(This,fCurrentCaretPos,iBaseLine,iBaseCol) ) 

#define IVsEditorGoBackLocations_SetMergeableGoBackLocation(This,fCurrentCaretPos,iBaseLine,iBaseCol)	\
    ( (This)->lpVtbl -> SetMergeableGoBackLocation(This,fCurrentCaretPos,iBaseLine,iBaseCol) ) 

#endif /* COBJMACROS */


#endif 	/* C style interface */




#endif 	/* __IVsEditorGoBackLocations_INTERFACE_DEFINED__ */

#endif /* __TextManagerInternal_LIBRARY_DEFINED__ */

#ifndef __IVsTextReplaceEvents_INTERFACE_DEFINED__
#define __IVsTextReplaceEvents_INTERFACE_DEFINED__

/* interface IVsTextReplaceEvents */
/* [object][uuid] */ 


EXTERN_C const IID IID_IVsTextReplaceEvents;

#if defined(__cplusplus) && !defined(CINTERFACE)
    
    MIDL_INTERFACE("CF9928D9-65AE-4319-A446-94ED5C45ECDE")
    IVsTextReplaceEvents : public IUnknown
    {
    public:
        virtual HRESULT STDMETHODCALLTYPE OnReplace( 
            /* [in] */ __RPC__in const ChangeInput *pCI) = 0;
        
    };
    
#else 	/* C style interface */

    typedef struct IVsTextReplaceEventsVtbl
    {
        BEGIN_INTERFACE
        
        HRESULT ( STDMETHODCALLTYPE *QueryInterface )( 
            IVsTextReplaceEvents * This,
            /* [in] */ __RPC__in REFIID riid,
            /* [iid_is][out] */ 
            __RPC__deref_out  void **ppvObject);
        
        ULONG ( STDMETHODCALLTYPE *AddRef )( 
            IVsTextReplaceEvents * This);
        
        ULONG ( STDMETHODCALLTYPE *Release )( 
            IVsTextReplaceEvents * This);
        
        HRESULT ( STDMETHODCALLTYPE *OnReplace )( 
            IVsTextReplaceEvents * This,
            /* [in] */ __RPC__in const ChangeInput *pCI);
        
        END_INTERFACE
    } IVsTextReplaceEventsVtbl;

    interface IVsTextReplaceEvents
    {
        CONST_VTBL struct IVsTextReplaceEventsVtbl *lpVtbl;
    };

    

#ifdef COBJMACROS


#define IVsTextReplaceEvents_QueryInterface(This,riid,ppvObject)	\
    ( (This)->lpVtbl -> QueryInterface(This,riid,ppvObject) ) 

#define IVsTextReplaceEvents_AddRef(This)	\
    ( (This)->lpVtbl -> AddRef(This) ) 

#define IVsTextReplaceEvents_Release(This)	\
    ( (This)->lpVtbl -> Release(This) ) 


#define IVsTextReplaceEvents_OnReplace(This,pCI)	\
    ( (This)->lpVtbl -> OnReplace(This,pCI) ) 

#endif /* COBJMACROS */


#endif 	/* C style interface */




#endif 	/* __IVsTextReplaceEvents_INTERFACE_DEFINED__ */


/* interface __MIDL_itf_textmgr_0000_0001 */
/* [local] */ 


enum __PROMPTONLOADFLAGS
    {	codepageNoPrompt	= 0,
	codepagePrompt	= 0x1
    } ;
typedef DWORD PROMPTONLOADFLAGS;


enum __VSTFF
    {	VSTFF_SIGNATURE	= 0x10000,
	VSTFF_DETECT	= 0x1000000,
	VSTFF_HTML	= 0x2000000,
	VSTFF_XML	= 0x4000000,
	VSTFF_BINARY	= 0x8000000,
	VSTFF_CPMASK	= 0xffff,
	VSTFF_FLAGSMASK	= 0xffff0000
    } ;
typedef DWORD VSTFF;



extern RPC_IF_HANDLE __MIDL_itf_textmgr_0000_0001_v0_0_c_ifspec;
extern RPC_IF_HANDLE __MIDL_itf_textmgr_0000_0001_v0_0_s_ifspec;

#ifndef __IVsFormatFilterProvider_INTERFACE_DEFINED__
#define __IVsFormatFilterProvider_INTERFACE_DEFINED__

/* interface IVsFormatFilterProvider */
/* [object][uuid] */ 


EXTERN_C const IID IID_IVsFormatFilterProvider;

#if defined(__cplusplus) && !defined(CINTERFACE)
    
    MIDL_INTERFACE("181237A7-4861-4D6C-8CA1-8A004BEA2E8E")
    IVsFormatFilterProvider : public IUnknown
    {
    public:
        virtual HRESULT STDMETHODCALLTYPE GetFormatFilterList( 
            /* [out] */ __RPC__deref_out_opt BSTR *pbstrFilterList) = 0;
        
        virtual HRESULT STDMETHODCALLTYPE CurFileExtensionFormat( 
            /* [in] */ __RPC__in BSTR bstrFileName,
            /* [out] */ __RPC__out DWORD *pdwExtnIndex) = 0;
        
        virtual HRESULT STDMETHODCALLTYPE QueryInvalidEncoding( 
            /* [in] */ VSTFF format,
            /* [out] */ __RPC__deref_out_opt BSTR *pbstrMessage) = 0;
        
    };
    
#else 	/* C style interface */

    typedef struct IVsFormatFilterProviderVtbl
    {
        BEGIN_INTERFACE
        
        HRESULT ( STDMETHODCALLTYPE *QueryInterface )( 
            IVsFormatFilterProvider * This,
            /* [in] */ __RPC__in REFIID riid,
            /* [iid_is][out] */ 
            __RPC__deref_out  void **ppvObject);
        
        ULONG ( STDMETHODCALLTYPE *AddRef )( 
            IVsFormatFilterProvider * This);
        
        ULONG ( STDMETHODCALLTYPE *Release )( 
            IVsFormatFilterProvider * This);
        
        HRESULT ( STDMETHODCALLTYPE *GetFormatFilterList )( 
            IVsFormatFilterProvider * This,
            /* [out] */ __RPC__deref_out_opt BSTR *pbstrFilterList);
        
        HRESULT ( STDMETHODCALLTYPE *CurFileExtensionFormat )( 
            IVsFormatFilterProvider * This,
            /* [in] */ __RPC__in BSTR bstrFileName,
            /* [out] */ __RPC__out DWORD *pdwExtnIndex);
        
        HRESULT ( STDMETHODCALLTYPE *QueryInvalidEncoding )( 
            IVsFormatFilterProvider * This,
            /* [in] */ VSTFF format,
            /* [out] */ __RPC__deref_out_opt BSTR *pbstrMessage);
        
        END_INTERFACE
    } IVsFormatFilterProviderVtbl;

    interface IVsFormatFilterProvider
    {
        CONST_VTBL struct IVsFormatFilterProviderVtbl *lpVtbl;
    };

    

#ifdef COBJMACROS


#define IVsFormatFilterProvider_QueryInterface(This,riid,ppvObject)	\
    ( (This)->lpVtbl -> QueryInterface(This,riid,ppvObject) ) 

#define IVsFormatFilterProvider_AddRef(This)	\
    ( (This)->lpVtbl -> AddRef(This) ) 

#define IVsFormatFilterProvider_Release(This)	\
    ( (This)->lpVtbl -> Release(This) ) 


#define IVsFormatFilterProvider_GetFormatFilterList(This,pbstrFilterList)	\
    ( (This)->lpVtbl -> GetFormatFilterList(This,pbstrFilterList) ) 

#define IVsFormatFilterProvider_CurFileExtensionFormat(This,bstrFileName,pdwExtnIndex)	\
    ( (This)->lpVtbl -> CurFileExtensionFormat(This,bstrFileName,pdwExtnIndex) ) 

#define IVsFormatFilterProvider_QueryInvalidEncoding(This,format,pbstrMessage)	\
    ( (This)->lpVtbl -> QueryInvalidEncoding(This,format,pbstrMessage) ) 

#endif /* COBJMACROS */


#endif 	/* C style interface */




#endif 	/* __IVsFormatFilterProvider_INTERFACE_DEFINED__ */


/* interface __MIDL_itf_textmgr_0000_0002 */
/* [local] */ 


enum _NavigableLocationResolverFlags
    {	NLR_ABSOLUTEPATH	= 0x1,
	NLR_PARTIALPATH	= 0x2,
	NLR_URL	= 0x10
    } ;
typedef DWORD NavigableLocationResolverFlags;



extern RPC_IF_HANDLE __MIDL_itf_textmgr_0000_0002_v0_0_c_ifspec;
extern RPC_IF_HANDLE __MIDL_itf_textmgr_0000_0002_v0_0_s_ifspec;

#ifndef __IVsNavigableLocationResolver_INTERFACE_DEFINED__
#define __IVsNavigableLocationResolver_INTERFACE_DEFINED__

/* interface IVsNavigableLocationResolver */
/* [object][uuid] */ 


EXTERN_C const IID IID_IVsNavigableLocationResolver;

#if defined(__cplusplus) && !defined(CINTERFACE)
    
    MIDL_INTERFACE("82D04B51-1626-43AA-B57F-9B23D4280333")
    IVsNavigableLocationResolver : public IUnknown
    {
    public:
        virtual HRESULT STDMETHODCALLTYPE GetDisplayText( 
            /* [in] */ DWORD dwReserved,
            /* [in] */ __RPC__in TextSpan *ptsBase,
            /* [in] */ __RPC__in_opt IVsTextLines *pBuffer,
            /* [out] */ __RPC__out NavigableLocationResolverFlags *dwOutFlags,
            /* [retval][out] */ __RPC__deref_out_opt BSTR *pbstrDisplayText) = 0;
        
        virtual HRESULT STDMETHODCALLTYPE GetPathFromLocation( 
            /* [in] */ DWORD dwReserved,
            /* [in] */ __RPC__in TextSpan *ptsBase,
            /* [in] */ __RPC__in_opt IVsTextLines *pBuffer,
            /* [in] */ __RPC__in const WCHAR *pszDisplayText,
            /* [out] */ __RPC__out NavigableLocationResolverFlags *dwOutFlags,
            /* [retval][out] */ __RPC__deref_out_opt BSTR *pbstrPath) = 0;
        
    };
    
#else 	/* C style interface */

    typedef struct IVsNavigableLocationResolverVtbl
    {
        BEGIN_INTERFACE
        
        HRESULT ( STDMETHODCALLTYPE *QueryInterface )( 
            IVsNavigableLocationResolver * This,
            /* [in] */ __RPC__in REFIID riid,
            /* [iid_is][out] */ 
            __RPC__deref_out  void **ppvObject);
        
        ULONG ( STDMETHODCALLTYPE *AddRef )( 
            IVsNavigableLocationResolver * This);
        
        ULONG ( STDMETHODCALLTYPE *Release )( 
            IVsNavigableLocationResolver * This);
        
        HRESULT ( STDMETHODCALLTYPE *GetDisplayText )( 
            IVsNavigableLocationResolver * This,
            /* [in] */ DWORD dwReserved,
            /* [in] */ __RPC__in TextSpan *ptsBase,
            /* [in] */ __RPC__in_opt IVsTextLines *pBuffer,
            /* [out] */ __RPC__out NavigableLocationResolverFlags *dwOutFlags,
            /* [retval][out] */ __RPC__deref_out_opt BSTR *pbstrDisplayText);
        
        HRESULT ( STDMETHODCALLTYPE *GetPathFromLocation )( 
            IVsNavigableLocationResolver * This,
            /* [in] */ DWORD dwReserved,
            /* [in] */ __RPC__in TextSpan *ptsBase,
            /* [in] */ __RPC__in_opt IVsTextLines *pBuffer,
            /* [in] */ __RPC__in const WCHAR *pszDisplayText,
            /* [out] */ __RPC__out NavigableLocationResolverFlags *dwOutFlags,
            /* [retval][out] */ __RPC__deref_out_opt BSTR *pbstrPath);
        
        END_INTERFACE
    } IVsNavigableLocationResolverVtbl;

    interface IVsNavigableLocationResolver
    {
        CONST_VTBL struct IVsNavigableLocationResolverVtbl *lpVtbl;
    };

    

#ifdef COBJMACROS


#define IVsNavigableLocationResolver_QueryInterface(This,riid,ppvObject)	\
    ( (This)->lpVtbl -> QueryInterface(This,riid,ppvObject) ) 

#define IVsNavigableLocationResolver_AddRef(This)	\
    ( (This)->lpVtbl -> AddRef(This) ) 

#define IVsNavigableLocationResolver_Release(This)	\
    ( (This)->lpVtbl -> Release(This) ) 


#define IVsNavigableLocationResolver_GetDisplayText(This,dwReserved,ptsBase,pBuffer,dwOutFlags,pbstrDisplayText)	\
    ( (This)->lpVtbl -> GetDisplayText(This,dwReserved,ptsBase,pBuffer,dwOutFlags,pbstrDisplayText) ) 

#define IVsNavigableLocationResolver_GetPathFromLocation(This,dwReserved,ptsBase,pBuffer,pszDisplayText,dwOutFlags,pbstrPath)	\
    ( (This)->lpVtbl -> GetPathFromLocation(This,dwReserved,ptsBase,pBuffer,pszDisplayText,dwOutFlags,pbstrPath) ) 

#endif /* COBJMACROS */


#endif 	/* C style interface */




#endif 	/* __IVsNavigableLocationResolver_INTERFACE_DEFINED__ */


#ifndef __IVsLanguageTextOps_INTERFACE_DEFINED__
#define __IVsLanguageTextOps_INTERFACE_DEFINED__

/* interface IVsLanguageTextOps */
/* [object][uuid] */ 


EXTERN_C const IID IID_IVsLanguageTextOps;

#if defined(__cplusplus) && !defined(CINTERFACE)
    
    MIDL_INTERFACE("518020A6-8D59-4C80-9AAE-015AB9DCC3DE")
    IVsLanguageTextOps : public IUnknown
    {
    public:
        virtual HRESULT STDMETHODCALLTYPE GetDataTip( 
            /* [in] */ __RPC__in_opt IVsTextLayer *pTextLayer,
            /* [in] */ __RPC__in const TextSpan *ptsSel,
            /* [out] */ __RPC__out TextSpan *ptsTip,
            /* [retval][out] */ __RPC__deref_out_opt BSTR *pbstrText) = 0;
        
        virtual HRESULT STDMETHODCALLTYPE GetPairExtent( 
            /* [in] */ __RPC__in_opt IVsTextLayer *pTextLayer,
            /* [in] */ TextAddress ta,
            /* [retval][out] */ __RPC__out TextSpan *pts) = 0;
        
        virtual HRESULT STDMETHODCALLTYPE GetWordExtent( 
            /* [in] */ __RPC__in_opt IVsTextLayer *pTextLayer,
            /* [in] */ TextAddress ta,
            /* [in] */ WORDEXTFLAGS flags,
            /* [retval][out] */ __RPC__out TextSpan *pts) = 0;
        
        virtual HRESULT STDMETHODCALLTYPE Format( 
            /* [in] */ __RPC__in_opt IVsTextLayer *pTextLayer,
            /* [in] */ __RPC__in const TextSpan *ptsSel) = 0;
        
    };
    
#else 	/* C style interface */

    typedef struct IVsLanguageTextOpsVtbl
    {
        BEGIN_INTERFACE
        
        HRESULT ( STDMETHODCALLTYPE *QueryInterface )( 
            IVsLanguageTextOps * This,
            /* [in] */ __RPC__in REFIID riid,
            /* [iid_is][out] */ 
            __RPC__deref_out  void **ppvObject);
        
        ULONG ( STDMETHODCALLTYPE *AddRef )( 
            IVsLanguageTextOps * This);
        
        ULONG ( STDMETHODCALLTYPE *Release )( 
            IVsLanguageTextOps * This);
        
        HRESULT ( STDMETHODCALLTYPE *GetDataTip )( 
            IVsLanguageTextOps * This,
            /* [in] */ __RPC__in_opt IVsTextLayer *pTextLayer,
            /* [in] */ __RPC__in const TextSpan *ptsSel,
            /* [out] */ __RPC__out TextSpan *ptsTip,
            /* [retval][out] */ __RPC__deref_out_opt BSTR *pbstrText);
        
        HRESULT ( STDMETHODCALLTYPE *GetPairExtent )( 
            IVsLanguageTextOps * This,
            /* [in] */ __RPC__in_opt IVsTextLayer *pTextLayer,
            /* [in] */ TextAddress ta,
            /* [retval][out] */ __RPC__out TextSpan *pts);
        
        HRESULT ( STDMETHODCALLTYPE *GetWordExtent )( 
            IVsLanguageTextOps * This,
            /* [in] */ __RPC__in_opt IVsTextLayer *pTextLayer,
            /* [in] */ TextAddress ta,
            /* [in] */ WORDEXTFLAGS flags,
            /* [retval][out] */ __RPC__out TextSpan *pts);
        
        HRESULT ( STDMETHODCALLTYPE *Format )( 
            IVsLanguageTextOps * This,
            /* [in] */ __RPC__in_opt IVsTextLayer *pTextLayer,
            /* [in] */ __RPC__in const TextSpan *ptsSel);
        
        END_INTERFACE
    } IVsLanguageTextOpsVtbl;

    interface IVsLanguageTextOps
    {
        CONST_VTBL struct IVsLanguageTextOpsVtbl *lpVtbl;
    };

    

#ifdef COBJMACROS


#define IVsLanguageTextOps_QueryInterface(This,riid,ppvObject)	\
    ( (This)->lpVtbl -> QueryInterface(This,riid,ppvObject) ) 

#define IVsLanguageTextOps_AddRef(This)	\
    ( (This)->lpVtbl -> AddRef(This) ) 

#define IVsLanguageTextOps_Release(This)	\
    ( (This)->lpVtbl -> Release(This) ) 


#define IVsLanguageTextOps_GetDataTip(This,pTextLayer,ptsSel,ptsTip,pbstrText)	\
    ( (This)->lpVtbl -> GetDataTip(This,pTextLayer,ptsSel,ptsTip,pbstrText) ) 

#define IVsLanguageTextOps_GetPairExtent(This,pTextLayer,ta,pts)	\
    ( (This)->lpVtbl -> GetPairExtent(This,pTextLayer,ta,pts) ) 

#define IVsLanguageTextOps_GetWordExtent(This,pTextLayer,ta,flags,pts)	\
    ( (This)->lpVtbl -> GetWordExtent(This,pTextLayer,ta,flags,pts) ) 

#define IVsLanguageTextOps_Format(This,pTextLayer,ptsSel)	\
    ( (This)->lpVtbl -> Format(This,pTextLayer,ptsSel) ) 

#endif /* COBJMACROS */


#endif 	/* C style interface */




#endif 	/* __IVsLanguageTextOps_INTERFACE_DEFINED__ */


/* interface __MIDL_itf_textmgr_0000_0004 */
/* [local] */ 

typedef 
enum _LTE_TEXTDATAFLAGS
    {	LTE_NORMAL	= 0,
	LTE_BOX	= 0x1,
	LTE_LINE	= 0x2
    } 	LTE_TEXTDATAFLAGS;



extern RPC_IF_HANDLE __MIDL_itf_textmgr_0000_0004_v0_0_c_ifspec;
extern RPC_IF_HANDLE __MIDL_itf_textmgr_0000_0004_v0_0_s_ifspec;

#ifndef __IVsLanguageClipboardOps_INTERFACE_DEFINED__
#define __IVsLanguageClipboardOps_INTERFACE_DEFINED__

/* interface IVsLanguageClipboardOps */
/* [object][uuid] */ 


EXTERN_C const IID IID_IVsLanguageClipboardOps;

#if defined(__cplusplus) && !defined(CINTERFACE)
    
    MIDL_INTERFACE("2ED7B280-B372-415E-862D-D7D9AEAFCD10")
    IVsLanguageClipboardOps : public IUnknown
    {
    public:
        virtual HRESULT STDMETHODCALLTYPE GetDataObject( 
            /* [in] */ __RPC__in_opt IVsTextView *pView,
            /* [in] */ __RPC__in_opt IVsTextLayer *pTextLayer,
            /* [retval][out] */ __RPC__deref_out_opt IDataObject **ppDO) = 0;
        
        virtual HRESULT STDMETHODCALLTYPE IsTextData( 
            /* [in] */ __RPC__in_opt IDataObject *pDO) = 0;
        
        virtual HRESULT STDMETHODCALLTYPE TextFromData( 
            /* [in] */ __RPC__in_opt IVsTextLayer *pTextLayer,
            /* [in] */ __RPC__in_opt IDataObject *pDO,
            /* [out] */ __RPC__out LTE_TEXTDATAFLAGS *ptdfFlags,
            /* [retval][out] */ __RPC__deref_out_opt BSTR *pbstrText) = 0;
        
        virtual HRESULT STDMETHODCALLTYPE DataObjectRendered( 
            /* [in] */ __RPC__in_opt IVsTextLines *pTextLines,
            /* [in] */ DWORD dwHint,
            /* [in] */ __RPC__in TextSpan *ptsInsertedText) = 0;
        
    };
    
#else 	/* C style interface */

    typedef struct IVsLanguageClipboardOpsVtbl
    {
        BEGIN_INTERFACE
        
        HRESULT ( STDMETHODCALLTYPE *QueryInterface )( 
            IVsLanguageClipboardOps * This,
            /* [in] */ __RPC__in REFIID riid,
            /* [iid_is][out] */ 
            __RPC__deref_out  void **ppvObject);
        
        ULONG ( STDMETHODCALLTYPE *AddRef )( 
            IVsLanguageClipboardOps * This);
        
        ULONG ( STDMETHODCALLTYPE *Release )( 
            IVsLanguageClipboardOps * This);
        
        HRESULT ( STDMETHODCALLTYPE *GetDataObject )( 
            IVsLanguageClipboardOps * This,
            /* [in] */ __RPC__in_opt IVsTextView *pView,
            /* [in] */ __RPC__in_opt IVsTextLayer *pTextLayer,
            /* [retval][out] */ __RPC__deref_out_opt IDataObject **ppDO);
        
        HRESULT ( STDMETHODCALLTYPE *IsTextData )( 
            IVsLanguageClipboardOps * This,
            /* [in] */ __RPC__in_opt IDataObject *pDO);
        
        HRESULT ( STDMETHODCALLTYPE *TextFromData )( 
            IVsLanguageClipboardOps * This,
            /* [in] */ __RPC__in_opt IVsTextLayer *pTextLayer,
            /* [in] */ __RPC__in_opt IDataObject *pDO,
            /* [out] */ __RPC__out LTE_TEXTDATAFLAGS *ptdfFlags,
            /* [retval][out] */ __RPC__deref_out_opt BSTR *pbstrText);
        
        HRESULT ( STDMETHODCALLTYPE *DataObjectRendered )( 
            IVsLanguageClipboardOps * This,
            /* [in] */ __RPC__in_opt IVsTextLines *pTextLines,
            /* [in] */ DWORD dwHint,
            /* [in] */ __RPC__in TextSpan *ptsInsertedText);
        
        END_INTERFACE
    } IVsLanguageClipboardOpsVtbl;

    interface IVsLanguageClipboardOps
    {
        CONST_VTBL struct IVsLanguageClipboardOpsVtbl *lpVtbl;
    };

    

#ifdef COBJMACROS


#define IVsLanguageClipboardOps_QueryInterface(This,riid,ppvObject)	\
    ( (This)->lpVtbl -> QueryInterface(This,riid,ppvObject) ) 

#define IVsLanguageClipboardOps_AddRef(This)	\
    ( (This)->lpVtbl -> AddRef(This) ) 

#define IVsLanguageClipboardOps_Release(This)	\
    ( (This)->lpVtbl -> Release(This) ) 


#define IVsLanguageClipboardOps_GetDataObject(This,pView,pTextLayer,ppDO)	\
    ( (This)->lpVtbl -> GetDataObject(This,pView,pTextLayer,ppDO) ) 

#define IVsLanguageClipboardOps_IsTextData(This,pDO)	\
    ( (This)->lpVtbl -> IsTextData(This,pDO) ) 

#define IVsLanguageClipboardOps_TextFromData(This,pTextLayer,pDO,ptdfFlags,pbstrText)	\
    ( (This)->lpVtbl -> TextFromData(This,pTextLayer,pDO,ptdfFlags,pbstrText) ) 

#define IVsLanguageClipboardOps_DataObjectRendered(This,pTextLines,dwHint,ptsInsertedText)	\
    ( (This)->lpVtbl -> DataObjectRendered(This,pTextLines,dwHint,ptsInsertedText) ) 

#endif /* COBJMACROS */


#endif 	/* C style interface */




#endif 	/* __IVsLanguageClipboardOps_INTERFACE_DEFINED__ */


/* interface __MIDL_itf_textmgr_0000_0005 */
/* [local] */ 


enum _QuerySendCommitFlags
    {	QSCF_DO_SEND_ENTER_COMMIT	= 0,
	QSCF_DO_NOT_SEND_ENTER_COMMIT	= 0x1
    } ;
typedef DWORD QuerySendCommitFlags;



extern RPC_IF_HANDLE __MIDL_itf_textmgr_0000_0005_v0_0_c_ifspec;
extern RPC_IF_HANDLE __MIDL_itf_textmgr_0000_0005_v0_0_s_ifspec;

#ifndef __IVsQueryLineChangeCommit_INTERFACE_DEFINED__
#define __IVsQueryLineChangeCommit_INTERFACE_DEFINED__

/* interface IVsQueryLineChangeCommit */
/* [object][uuid] */ 


EXTERN_C const IID IID_IVsQueryLineChangeCommit;

#if defined(__cplusplus) && !defined(CINTERFACE)
    
    MIDL_INTERFACE("72E67329-8765-4586-8B15-0B445E6F3B57")
    IVsQueryLineChangeCommit : public IUnknown
    {
    public:
        virtual HRESULT STDMETHODCALLTYPE QuerySendEnterLineCommit( 
            /* [in] */ DWORD dwReserved,
            /* [in] */ long iLineStart,
            /* [in] */ long iLineEnd,
            /* [retval][out] */ __RPC__out QuerySendCommitFlags *dwQuerySendCommitFlags) = 0;
        
    };
    
#else 	/* C style interface */

    typedef struct IVsQueryLineChangeCommitVtbl
    {
        BEGIN_INTERFACE
        
        HRESULT ( STDMETHODCALLTYPE *QueryInterface )( 
            IVsQueryLineChangeCommit * This,
            /* [in] */ __RPC__in REFIID riid,
            /* [iid_is][out] */ 
            __RPC__deref_out  void **ppvObject);
        
        ULONG ( STDMETHODCALLTYPE *AddRef )( 
            IVsQueryLineChangeCommit * This);
        
        ULONG ( STDMETHODCALLTYPE *Release )( 
            IVsQueryLineChangeCommit * This);
        
        HRESULT ( STDMETHODCALLTYPE *QuerySendEnterLineCommit )( 
            IVsQueryLineChangeCommit * This,
            /* [in] */ DWORD dwReserved,
            /* [in] */ long iLineStart,
            /* [in] */ long iLineEnd,
            /* [retval][out] */ __RPC__out QuerySendCommitFlags *dwQuerySendCommitFlags);
        
        END_INTERFACE
    } IVsQueryLineChangeCommitVtbl;

    interface IVsQueryLineChangeCommit
    {
        CONST_VTBL struct IVsQueryLineChangeCommitVtbl *lpVtbl;
    };

    

#ifdef COBJMACROS


#define IVsQueryLineChangeCommit_QueryInterface(This,riid,ppvObject)	\
    ( (This)->lpVtbl -> QueryInterface(This,riid,ppvObject) ) 

#define IVsQueryLineChangeCommit_AddRef(This)	\
    ( (This)->lpVtbl -> AddRef(This) ) 

#define IVsQueryLineChangeCommit_Release(This)	\
    ( (This)->lpVtbl -> Release(This) ) 


#define IVsQueryLineChangeCommit_QuerySendEnterLineCommit(This,dwReserved,iLineStart,iLineEnd,dwQuerySendCommitFlags)	\
    ( (This)->lpVtbl -> QuerySendEnterLineCommit(This,dwReserved,iLineStart,iLineEnd,dwQuerySendCommitFlags) ) 

#endif /* COBJMACROS */


#endif 	/* C style interface */




#endif 	/* __IVsQueryLineChangeCommit_INTERFACE_DEFINED__ */


#ifndef __IVsCodeWindowEvents_INTERFACE_DEFINED__
#define __IVsCodeWindowEvents_INTERFACE_DEFINED__

/* interface IVsCodeWindowEvents */
/* [object][uuid] */ 


EXTERN_C const IID IID_IVsCodeWindowEvents;

#if defined(__cplusplus) && !defined(CINTERFACE)
    
    MIDL_INTERFACE("26ACC7FD-C665-426B-B120-E96762C5DD1A")
    IVsCodeWindowEvents : public IUnknown
    {
    public:
        virtual HRESULT STDMETHODCALLTYPE OnNewView( 
            /* [in] */ __RPC__in_opt IVsTextView *pView) = 0;
        
        virtual HRESULT STDMETHODCALLTYPE OnCloseView( 
            /* [in] */ __RPC__in_opt IVsTextView *pView) = 0;
        
    };
    
#else 	/* C style interface */

    typedef struct IVsCodeWindowEventsVtbl
    {
        BEGIN_INTERFACE
        
        HRESULT ( STDMETHODCALLTYPE *QueryInterface )( 
            IVsCodeWindowEvents * This,
            /* [in] */ __RPC__in REFIID riid,
            /* [iid_is][out] */ 
            __RPC__deref_out  void **ppvObject);
        
        ULONG ( STDMETHODCALLTYPE *AddRef )( 
            IVsCodeWindowEvents * This);
        
        ULONG ( STDMETHODCALLTYPE *Release )( 
            IVsCodeWindowEvents * This);
        
        HRESULT ( STDMETHODCALLTYPE *OnNewView )( 
            IVsCodeWindowEvents * This,
            /* [in] */ __RPC__in_opt IVsTextView *pView);
        
        HRESULT ( STDMETHODCALLTYPE *OnCloseView )( 
            IVsCodeWindowEvents * This,
            /* [in] */ __RPC__in_opt IVsTextView *pView);
        
        END_INTERFACE
    } IVsCodeWindowEventsVtbl;

    interface IVsCodeWindowEvents
    {
        CONST_VTBL struct IVsCodeWindowEventsVtbl *lpVtbl;
    };

    

#ifdef COBJMACROS


#define IVsCodeWindowEvents_QueryInterface(This,riid,ppvObject)	\
    ( (This)->lpVtbl -> QueryInterface(This,riid,ppvObject) ) 

#define IVsCodeWindowEvents_AddRef(This)	\
    ( (This)->lpVtbl -> AddRef(This) ) 

#define IVsCodeWindowEvents_Release(This)	\
    ( (This)->lpVtbl -> Release(This) ) 


#define IVsCodeWindowEvents_OnNewView(This,pView)	\
    ( (This)->lpVtbl -> OnNewView(This,pView) ) 

#define IVsCodeWindowEvents_OnCloseView(This,pView)	\
    ( (This)->lpVtbl -> OnCloseView(This,pView) ) 

#endif /* COBJMACROS */


#endif 	/* C style interface */




#endif 	/* __IVsCodeWindowEvents_INTERFACE_DEFINED__ */


#ifndef __IVsFileBackup_INTERFACE_DEFINED__
#define __IVsFileBackup_INTERFACE_DEFINED__

/* interface IVsFileBackup */
/* [object][unique][version][uuid] */ 


EXTERN_C const IID IID_IVsFileBackup;

#if defined(__cplusplus) && !defined(CINTERFACE)
    
    MIDL_INTERFACE("7B32BDF9-89D3-4574-B6AC-F2EEE9DEAB18")
    IVsFileBackup : public IUnknown
    {
    public:
        virtual HRESULT STDMETHODCALLTYPE BackupFile( 
            /* [in] */ __RPC__in LPCOLESTR pszBackupFileName) = 0;
        
        virtual HRESULT STDMETHODCALLTYPE IsBackupFileObsolete( 
            /* [out] */ __RPC__out BOOL *pbObsolete) = 0;
        
    };
    
#else 	/* C style interface */

    typedef struct IVsFileBackupVtbl
    {
        BEGIN_INTERFACE
        
        HRESULT ( STDMETHODCALLTYPE *QueryInterface )( 
            IVsFileBackup * This,
            /* [in] */ __RPC__in REFIID riid,
            /* [iid_is][out] */ 
            __RPC__deref_out  void **ppvObject);
        
        ULONG ( STDMETHODCALLTYPE *AddRef )( 
            IVsFileBackup * This);
        
        ULONG ( STDMETHODCALLTYPE *Release )( 
            IVsFileBackup * This);
        
        HRESULT ( STDMETHODCALLTYPE *BackupFile )( 
            IVsFileBackup * This,
            /* [in] */ __RPC__in LPCOLESTR pszBackupFileName);
        
        HRESULT ( STDMETHODCALLTYPE *IsBackupFileObsolete )( 
            IVsFileBackup * This,
            /* [out] */ __RPC__out BOOL *pbObsolete);
        
        END_INTERFACE
    } IVsFileBackupVtbl;

    interface IVsFileBackup
    {
        CONST_VTBL struct IVsFileBackupVtbl *lpVtbl;
    };

    

#ifdef COBJMACROS


#define IVsFileBackup_QueryInterface(This,riid,ppvObject)	\
    ( (This)->lpVtbl -> QueryInterface(This,riid,ppvObject) ) 

#define IVsFileBackup_AddRef(This)	\
    ( (This)->lpVtbl -> AddRef(This) ) 

#define IVsFileBackup_Release(This)	\
    ( (This)->lpVtbl -> Release(This) ) 


#define IVsFileBackup_BackupFile(This,pszBackupFileName)	\
    ( (This)->lpVtbl -> BackupFile(This,pszBackupFileName) ) 

#define IVsFileBackup_IsBackupFileObsolete(This,pbObsolete)	\
    ( (This)->lpVtbl -> IsBackupFileObsolete(This,pbObsolete) ) 

#endif /* COBJMACROS */


#endif 	/* C style interface */




#endif 	/* __IVsFileBackup_INTERFACE_DEFINED__ */


#ifndef __IVsLineAttributes_INTERFACE_DEFINED__
#define __IVsLineAttributes_INTERFACE_DEFINED__

/* interface IVsLineAttributes */
/* [object][unique][version][uuid] */ 


EXTERN_C const IID IID_IVsLineAttributes;

#if defined(__cplusplus) && !defined(CINTERFACE)
    
    MIDL_INTERFACE("BBB6EAEA-4835-4263-BEDB-2705607803CA")
    IVsLineAttributes : public IUnknown
    {
    public:
        virtual HRESULT STDMETHODCALLTYPE SetLineSeparator( 
            /* [in] */ __RPC__in long *rgLines,
            /* [in] */ long cLines,
            /* [in] */ BOOL fRemoveAll) = 0;
        
    };
    
#else 	/* C style interface */

    typedef struct IVsLineAttributesVtbl
    {
        BEGIN_INTERFACE
        
        HRESULT ( STDMETHODCALLTYPE *QueryInterface )( 
            IVsLineAttributes * This,
            /* [in] */ __RPC__in REFIID riid,
            /* [iid_is][out] */ 
            __RPC__deref_out  void **ppvObject);
        
        ULONG ( STDMETHODCALLTYPE *AddRef )( 
            IVsLineAttributes * This);
        
        ULONG ( STDMETHODCALLTYPE *Release )( 
            IVsLineAttributes * This);
        
        HRESULT ( STDMETHODCALLTYPE *SetLineSeparator )( 
            IVsLineAttributes * This,
            /* [in] */ __RPC__in long *rgLines,
            /* [in] */ long cLines,
            /* [in] */ BOOL fRemoveAll);
        
        END_INTERFACE
    } IVsLineAttributesVtbl;

    interface IVsLineAttributes
    {
        CONST_VTBL struct IVsLineAttributesVtbl *lpVtbl;
    };

    

#ifdef COBJMACROS


#define IVsLineAttributes_QueryInterface(This,riid,ppvObject)	\
    ( (This)->lpVtbl -> QueryInterface(This,riid,ppvObject) ) 

#define IVsLineAttributes_AddRef(This)	\
    ( (This)->lpVtbl -> AddRef(This) ) 

#define IVsLineAttributes_Release(This)	\
    ( (This)->lpVtbl -> Release(This) ) 


#define IVsLineAttributes_SetLineSeparator(This,rgLines,cLines,fRemoveAll)	\
    ( (This)->lpVtbl -> SetLineSeparator(This,rgLines,cLines,fRemoveAll) ) 

#endif /* COBJMACROS */


#endif 	/* C style interface */




#endif 	/* __IVsLineAttributes_INTERFACE_DEFINED__ */


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


