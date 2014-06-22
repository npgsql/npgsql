/****************************************************************************
	MsoGUIDs.h

	Owner: DavePa
	Copyright (c) 1994 Microsoft Corporation

	This file defines (if INIT_MSO_GUIDS is defined) or declares all the
	OLE GUIDs exported by Office.

	Office Development has reserved GUIDs in the range:
		000Cxxxx-0000-0000-C000-000000000046

****************************************************************************/

#ifndef MSOGUIDS_H
#define MSOGUIDS_H

/* If INIT_MSO_GUIDS is defined then we're going to define all the GUIDS,
	otherwise we'll just declare them.  Office GUIDS are specified by
	a category 0x00-0xFF and an index 0x00-0xFF. */
#undef DEFINE_MSO_GUID
#undef DEFINE_MSO95_GUID
#ifdef INIT_MSO_GUIDS
	#define DEFINE_MSO_GUID(name, bCategory, bIndex) \
		EXTERN_C const GUID name = {0x000C0000 | MAKEWORD(bIndex, bCategory), \
													  0, 0, {0xC0,0,0,0,0,0,0,0x46}}

	// This is added for Office 95 backwards compatibility
	#define DEFINE_MSO95_GUID(name, bCategory, bIndex) \
		EXTERN_C const GUID name = {0x2DF80000 | MAKEWORD(bIndex, bCategory), \
													  0x5BFA, 0x101B, {0xBD,0xE5,0x00,0xAA,0x00,0x44,0xDE,0x52}}
#else
	#define DEFINE_MSO_GUID(name, bCategory, bIndex) \
		EXTERN_C const GUID name
	#define DEFINE_MSO95_GUID(name, bCategory, bIndex) \
		EXTERN_C const GUID name
#endif


/****************************************************************************
	Office GUIDS are specified by a category 0x00-0xFF and an index 0x00-0xFF.
****************************************************************************/

// Category 00 is reserved for LIME.EXE

// Category 01: Toolbar Interfaces and other GUIDS
DEFINE_MSO_GUID(IID_IMsoToolbarSet,             0x01, 0x00);
DEFINE_MSO_GUID(IID_IMsoToolbarSetUser,         0x01, 0x01);
DEFINE_MSO_GUID(IID_IMsoControlContainer,       0x01, 0x02);
DEFINE_MSO_GUID(IID_IMsoControlContainerUser,   0x01, 0x03);
DEFINE_MSO_GUID(IID_IMsoToolbar,                0x01, 0x04);
DEFINE_MSO_GUID(IID_IMsoToolbarUser,            0x01, 0x05);
DEFINE_MSO_GUID(IID_IMsoControl,                0x01, 0x06);
DEFINE_MSO_GUID(IID_IMsoControlSite,            0x01, 0x07);
DEFINE_MSO_GUID(IID_IMsoGenericContainer,       0x01, 0x08);
DEFINE_MSO_GUID(IID_IMsoControlUser,            0x01, 0x09);
DEFINE_MSO_GUID(IID_IMsoButtonUser,             0x01, 0x0A);
DEFINE_MSO_GUID(IID_IMsoContainerControlUser,   0x01, 0x0B);
DEFINE_MSO_GUID(IID_IMsoDropdownUser,           0x01, 0x0C);
DEFINE_MSO_GUID(IID_IMsoOwnerDropdownUser,      0x01, 0x0D);
DEFINE_MSO_GUID(IID_IMsoWellUser,               0x01, 0x0E);
DEFINE_MSO_GUID(IID_IMsoMenuUser,               0x01, 0x0F);
DEFINE_MSO_GUID(IID_IMsoExpandingGridUser,      0x01, 0x10);
DEFINE_MSO_GUID(IID_IMsoLabelUser,              0x01, 0x11);
DEFINE_MSO_GUID(IID_IMsoGenericDropdownUser,    0x01, 0x12);
DEFINE_MSO_GUID(IID_IMsoSwatchUser,             0x01, 0x13);
DEFINE_MSO_GUID(IID_IMsoTbFrame,                0x01, 0x14);
DEFINE_MSO_GUID(IID_IMsoOCXDropUser,            0x01, 0x15);
DEFINE_MSO_GUID(IID_IMsoGaugeUser,              0x01, 0x16);
DEFINE_MSO_GUID(IID_IMsoSpecialUser,			0x01, 0x17);
DEFINE_MSO_GUID(CATID_IMsoTbFrame,				0x01, 0x18);
DEFINE_MSO_GUID(IID_IMsoTbServer,				0x01, 0x19);
DEFINE_MSO_GUID(IID_IMsoTbInProc,				0x01, 0x1A);
DEFINE_MSO_GUID(IID_IMsoWysiwygDropdownUser,    0x01, 0x1B);
DEFINE_MSO_GUID(IID_IMsoPaneUser,               0x01, 0x1C);
#ifdef OWSME
DEFINE_MSO_GUID(IID_IMsoMultiEditUser,          0x01, 0x1D);
#endif
DEFINE_MSO_GUID(IID_IMsoFilterKeyEvents,        0x01, 0x1E);
DEFINE_MSO_GUID(IID_IMsoExtButtonUser,          0x01, 0x1F);
DEFINE_MSO_GUID(IID_IMsoLabelExUser,            0x01, 0x20);
DEFINE_MSO_GUID(IID_IMsoSpinnerUser,            0x01, 0x21);
DEFINE_MSO_GUID(IID_IMsoSpeechRecoLabelUser,    0x01, 0x22);
DEFINE_MSO_GUID(IID_IMsoSpeechReco,             0x01, 0x23);
DEFINE_MSO_GUID(IID_IMsoSpeechRecoUser,         0x01, 0x24);
DEFINE_MSO_GUID(IID_IMsoAutoCompleteDropdownUser,	0x01, 0x25);

// The class factory for tbframe etc. recycles the IID_IMsoTbFrame
// for its CLSID.
#define CLSID_Mso_From97 IID_IMsoTbFrame

// The class factory for mailsite etc. recycles the IID_IMsoMailSite2
// for its CLSID.
#define CLSID_Mso_From2000 IID_IMsoMailSite

// Category 02: Drawing Interfaces
DEFINE_MSO_GUID(IID_IMsoShape,                  0x02, 0x00);
DEFINE_MSO_GUID(IID_IMsoShapeProp,              0x02, 0x01);
DEFINE_MSO_GUID(IID_IMsoDrawing,                0x02, 0x02);
DEFINE_MSO_GUID(IID_IMsoDrawingSite,            0x02, 0x03);
DEFINE_MSO_GUID(IID_IMsoDisplayManager,         0x02, 0x04);
DEFINE_MSO_GUID(IID_IMsoDisplayManagerSite,     0x02, 0x05);
DEFINE_MSO_GUID(IID_IMsoDisplayElementSet,      0x02, 0x06);
DEFINE_MSO_GUID(IID_IMsoDrawingView,            0x02, 0x07);
DEFINE_MSO_GUID(IID_IMsoDrawingViewSite,        0x02, 0x08);
DEFINE_MSO_GUID(IID_IMsoArray,                  0x02, 0x09);
DEFINE_MSO_GUID(IID_IMsoDrawingSelection,       0x02, 0x0A);
DEFINE_MSO_GUID(IID_IMsoDrawingViewGroup,       0x02, 0x0B);
DEFINE_MSO_GUID(IID_IMsoDrawingGroup,           0x02, 0x0C);
DEFINE_MSO_GUID(IID_IMsoDrag,                   0x02, 0x0D);
DEFINE_MSO_GUID(IID_IMsoDragSite,               0x02, 0x0E);
DEFINE_MSO_GUID(CLSID_PrimitiveShape,           0x02, 0x0F);
DEFINE_MSO_GUID(CLSID_CompoundShape,            0x02, 0x10);
DEFINE_MSO_GUID(IID_IMsoTextboxSite,            0x02, 0x11);
DEFINE_MSO_GUID(IID_IMsoTextbox,                0x02, 0x12);
DEFINE_MSO_GUID(IID_IMsoDrawingUserInterface,   0x02, 0x13);
DEFINE_MSO_GUID(IID_IMsoDrawingUserInterfaceSite,0x02, 0x14);
DEFINE_MSO_GUID(IID_IMsoBlip,                   0x02, 0x15);
DEFINE_MSO_GUID(IID_IMsoDrawingGroupSite,       0x02, 0x16);
DEFINE_MSO_GUID(IID_IMsoRule,							0x02, 0x17);
DEFINE_MSO_GUID(IID_IMsoDrawingSelectionSite,   0x02, 0x18);
DEFINE_MSO_GUID(IID_IMsoDrawingXMLImport,       0x02, 0x19);
DEFINE_MSO_GUID(IID_IMsoDrawingXMLImportSite,   0x02, 0x1A);
DEFINE_MSO_GUID(IID_IMsoDrawingHTMLExport,      0x02, 0x1B);
DEFINE_MSO_GUID(IID_IMsoDrawingHTMLExportSite,  0x02, 0x1C);
DEFINE_MSO_GUID(IID_IMsoBitmapCreator,          0x02, 0x1D);
DEFINE_MSO_GUID(IID_IMsoGraphicsXMLImport,      0x02, 0x1E);
DEFINE_MSO_GUID(IID_IMsoGraphicsXMLImportSite,  0x02, 0x1F);
DEFINE_MSO_GUID(IID_IMsoColorspace,             0x02, 0x20);
DEFINE_MSO_GUID(IID_IMsoQueryLocation,          0x02, 0x21);
DEFINE_MSO_GUID(IID_IMsoQueryNotifySink,        0x02, 0x22);
DEFINE_MSO_GUID(IID_IMsoEnumQUERYELEMENT, 		0x02, 0x23);
DEFINE_MSO_GUID(IID_IMsoDrawingDiagram,			0x02, 0x24);
DEFINE_MSO_GUID(IID_IMsoDrawingDiagramShape,		0x02, 0x25);
DEFINE_MSO_GUID(IID_IMsoDrawingLayoutManager,	0x02, 0x26);
DEFINE_MSO_GUID(IID_IMsoDrawingLayoutObj,		   0x02, 0x27);
DEFINE_MSO_GUID(IID_IMsoDrawingLayoutShape,		0x02, 0x28);
DEFINE_MSO_GUID(IID_IMsoDrawingLayoutConnector, 0x02, 0x29);
DEFINE_MSO_GUID(IID_IMsoDrawingLayout,          0x02, 0x2A);
DEFINE_MSO_GUID(IID_IMsoDrawingLineRout,        0x02, 0x2B);
DEFINE_MSO_GUID(IID_IMsoDrawingLayoutPoints,    0x02, 0x2C);
DEFINE_MSO_GUID(IID_IMsoDrawingLayoutObjs,      0x02, 0x2D);
DEFINE_MSO_GUID(IID_IMsoCMS,                    0x02, 0x2E);
DEFINE_MSO_GUID(IID_IMsoBlipState,              0x02, 0x2F);
DEFINE_MSO_GUID(IID_IMsoAGIFAnimState,          0x02, 0x30);

// Category 03: OLE Automation GUIDs (see mso96.odl)
// Note: Free values between non-contiguous items are free for the taking
DEFINE_MSO_GUID(UIID__IMsoDispObj,              0x03, 0x00);
DEFINE_MSO_GUID(UIID__IMsoOleAccDispObj,        0x03, 0x01);
DEFINE_MSO_GUID(IID_IMsoDispCommandBars,        0x03, 0x02);
DEFINE_MSO_GUID(IID__IMsoAccessible,            0x03, 0x03);
DEFINE_MSO_GUID(IID_IMsoDispCommandBar,         0x03, 0x04);
DEFINE_MSO_GUID(IID_IMsoDispCommandBarControls, 0x03, 0x06);
DEFINE_MSO_GUID(IID_IMsoDispCommandBarControl,  0x03, 0x08);
DEFINE_MSO_GUID(IID_IMsoDispCommandBarPopup,    0x03, 0x0A);
DEFINE_MSO_GUID(IID_IMsoDispCommandBarComboBox, 0x03, 0x0C);
DEFINE_MSO_GUID(IID_IMsoDispCommandBarActiveX,  0x03, 0x0D);
DEFINE_MSO_GUID(IID_IMsoDispCommandBarButton,   0x03, 0x0E);
#ifdef UNDO_WEBPANE_CUT
DEFINE_MSO_GUID(IID_IMsoDispCommandBarWebPane,  0x03, 0x0F);
#endif
DEFINE_MSO_GUID(IID_IMsoDispAdjustments,        0x03, 0x10);
DEFINE_MSO_GUID(IID_IMsoDispCallOut,            0x03, 0x11);
DEFINE_MSO_GUID(IID_IMsoDispColor,              0x03, 0x12);
DEFINE_MSO_GUID(IID_IMsoDispConnectorFormat,    0x03, 0x13);
DEFINE_MSO_GUID(IID_IMsoDispFillFormat,         0x03, 0x14);
DEFINE_MSO_GUID(IID_IMsoDispFreeformBuilder,    0x03, 0x15);
DEFINE_MSO_GUID(IID_IMsoDispGroupShapes,        0x03, 0x16);
DEFINE_MSO_GUID(IID_IMsoDispLineFormat,         0x03, 0x17);
DEFINE_MSO_GUID(IID_IMsoDispNode,               0x03, 0x18);
DEFINE_MSO_GUID(IID_IMsoDispNodes,              0x03, 0x19);
DEFINE_MSO_GUID(IID_IMsoDispPictureFormat,      0x03, 0x1A);
DEFINE_MSO_GUID(IID_IMsoDispShadowFormat,       0x03, 0x1B);
DEFINE_MSO_GUID(IID_IMsoDispShape,              0x03, 0x1C);
DEFINE_MSO_GUID(IID_IMsoDispShapeRange,         0x03, 0x1D);
DEFINE_MSO_GUID(IID_IMsoDispShapes,             0x03, 0x1E);
DEFINE_MSO_GUID(IID_IMsoDispTextEffectFormat,   0x03, 0x1F);
DEFINE_MSO_GUID(IID_IMsoDispTextFrame,          0x03, 0x20);
DEFINE_MSO_GUID(IID_IMsoDispThreeDFormat,       0x03, 0x21);
DEFINE_MSO_GUID(IID_IMsoDispAssistant,		   	0x03, 0x22);
DEFINE_MSO_GUID(IID_IMsoDispAssistBalloon,		0x03, 0x24);
DEFINE_MSO_GUID(IID_IMsoDispAssistCheckboxes,	0x03, 0x26);
DEFINE_MSO_GUID(IID_IMsoDispAssistCheckbox,		0x03, 0x28);
DEFINE_MSO_GUID(IID_IMsoDispAssistEditboxes, 	0x03, 0x2A);
DEFINE_MSO_GUID(IID_IMsoDispAssistEditbox,		0x03, 0x2C);
DEFINE_MSO_GUID(IID_IMsoDispAssistLabels, 		0x03, 0x2E);
DEFINE_MSO_GUID(IID_IMsoDispAssistLabel,			0x03, 0x30);
DEFINE_MSO_GUID(IID_FoundFiles,						0x03, 0x31);
DEFINE_MSO_GUID(IID_FileSearch,						0x03, 0x32);
DEFINE_MSO_GUID(IID_PropertyTest,					0x03, 0x33);
DEFINE_MSO_GUID(IID_PropertyTests,					0x03, 0x34);
DEFINE_MSO_GUID(IID_FileSearches,					0x03, 0x35);
DEFINE_MSO_GUID(IID_FFResults,						0x03, 0x36);
DEFINE_MSO_GUID(IID_IFind,								0x03, 0x37);
DEFINE_MSO_GUID(IID_IFoundFiles,						0x03, 0x38);
DEFINE_MSO_GUID(IID_IMsoDispAddInsX,				0x03, 0x39);
DEFINE_MSO_GUID(IID_IMsoDispAddInX, 				0x03, 0x3A);
DEFINE_MSO_GUID(IID_IMsoDispScripts, 				0x03, 0x40);
DEFINE_MSO_GUID(IID_IMsoDispScript, 				0x03, 0x41);
DEFINE_MSO_GUID(DIID_IMsoDispCommandBarButtonEvents,	0x03, 0x51);
DEFINE_MSO_GUID(DIID_IMsoDispCommandBarsEvents,	0x03, 0x52);
DEFINE_MSO_GUID(IID_IMsoLanguageSettings,			0x03, 0x53);
DEFINE_MSO_GUID(DIID_IMsoDispCommandBarComboBoxEvents,	0x03, 0x54);
DEFINE_MSO_GUID(IID_IMsoCAGClient,					0x03, 0x55);
DEFINE_MSO_GUID(IID_IMsoHTMLProject,				0x03, 0x56);
DEFINE_MSO_GUID(IID_IMsoHTMLProjItems,				0x03, 0x57);
DEFINE_MSO_GUID(IID_IMsoHTMLProjItem,				0x03, 0x58);
DEFINE_MSO_GUID(IID_IMsoDispCagNotifySink,			0x03, 0x59);
DEFINE_MSO_GUID(IID_IMsoDebugOptions,				0x03, 0x5A);
DEFINE_MSO_GUID(IID_IMsoDispAnswerWizard,			0x03, 0x60);
DEFINE_MSO_GUID(IID_IMsoDispAnswerWizardFiles,		0x03, 0x61);
DEFINE_MSO_GUID(IID_IMsoFileDialog,					0x03, 0x62);
DEFINE_MSO_GUID(IID_IMsoFileDialogSelectedItems,	0x03, 0x63);
DEFINE_MSO_GUID(IID_IMsoFileDialogFilter,			0x03, 0x64);
DEFINE_MSO_GUID(IID_IMsoFileDialogFilters,			0x03, 0x65);
DEFINE_MSO_GUID(IID_ISearchScopes,					0x03, 0x66);
DEFINE_MSO_GUID(IID_ISearchScope,					0x03, 0x67);
DEFINE_MSO_GUID(IID_IScopeFolder,					0x03, 0x68);
DEFINE_MSO_GUID(IID_IScopeFolders,					0x03, 0x69);
DEFINE_MSO_GUID(IID_ISearchFolders,					0x03, 0x6A);
DEFINE_MSO_GUID(IID_ISearchPaths,					0x03, 0x6B);
DEFINE_MSO_GUID(IID_IFileTypes,						0x03, 0x6C);
DEFINE_MSO_GUID(IID_IMsoDispDiagram,			   0x03, 0x6D);
DEFINE_MSO_GUID(IID_IMsoDispDiagramNodes,		   0x03, 0x6E);
DEFINE_MSO_GUID(IID_IMsoDispDiagramNodeChildren,0x03, 0x6F);
DEFINE_MSO_GUID(IID_IMsoDispDiagramNode,			0x03, 0x70);
DEFINE_MSO_GUID(IID_IMsoDispCanvasShapes,       0x03, 0x71);

// Category 04 is reserved for the Document Management group
DEFINE_MSO_GUID(IID_IMsoFindFile,					0x04, 0x00);
DEFINE_MSO_GUID(IID_IMsoSearch,						0x04, 0x01);
DEFINE_MSO_GUID(IID_IMsoFoundFileList,				0x04, 0x02);
DEFINE_MSO_GUID(IID_IMsoSelectedFileList,			0x04, 0x03);
DEFINE_MSO_GUID(IID_IMsoFoundFile,					0x04, 0x04);
DEFINE_MSO_GUID(IID_IMsoDMControl,					0x04, 0x05);
DEFINE_MSO_GUID(IID_IMsoControlList,				0x04, 0x06);
DEFINE_MSO_GUID(IID_IMsoCommandList,				0x04, 0x07);
DEFINE_MSO_GUID(IID_IMsoFileTypeList,				0x04, 0x08);
DEFINE_MSO_GUID(IID_IMsoCodePageList,				0x04, 0x09);
DEFINE_MSO_GUID(IID_IMsoAppPreview,					0x04, 0x0A);
DEFINE_MSO_GUID(IID_IMsoOLDocument,					0x04, 0x0B);
DEFINE_MSO_GUID(IID_IMsoOLDocument2,				0x04, 0x0C);
DEFINE_MSO_GUID(IID_IMsoGSV,							0x04, 0x0D);
DEFINE_MSO_GUID(IID_IMsoDMIBindStatusCallback,	0x04, 0x0E);
DEFINE_MSO_GUID(IID_IMsoRedirectedMoniker,		0x04, 0x0F);
DEFINE_MSO_GUID(IID_IMsoDispSignatureSet,           0x04, 0x10);
DEFINE_MSO_GUID(IID_IMsoDispSignature,              0x04, 0x11);
DEFINE_MSO_GUID(IID_IMsoAsyncStream,				0x04, 0x12);

// Category 05: Misc. general-purpose GUIDs
DEFINE_MSO_GUID(IID_ISimpleUnknown,             0x05, 0x00);
DEFINE_MSO_GUID(IID_IMsoAuf,             			0x05, 0x01);
DEFINE_MSO_GUID(IID_IBogus,             			0x05, 0x02);
//DEFINE_MSO_GUID(IID_IMsoOfficeAsst,				0x05, 0x03);  // Office 97 only
DEFINE_MSO_GUID(IID_IMsoMacDragWindow,				0x05, 0x04);
DEFINE_MSO_GUID(IID_IMsoMacDrag,						0x05, 0x05);
DEFINE_MSO_GUID(IID_IUnknownInProc,					0x05, 0x06);
DEFINE_MSO_GUID(IID_IMsoShMemory,					0x05, 0x07);
DEFINE_MSO_GUID(IID_IAMNotifySink,					0x05, 0x08);
DEFINE_MSO_GUID(IID_IMsoSoundPlayer,				0x05, 0x09);
DEFINE_MSO_GUID(IID_IMsoOwsEventsExtension,		0x05, 0x0A);
DEFINE_MSO_GUID(IID_IMsoDebugUser,					0x05, 0x0B);
DEFINE_MSO_GUID(IID_IMsoOwsExtension,				0x05, 0x0C);
DEFINE_MSO_GUID(IID_IMsoOfficeAsst2,				0x05, 0x0D);
DEFINE_MSO_GUID(IID_IMsoString,                 0x05, 0x0E);
DEFINE_MSO_GUID(IID_IMsoOnObjManager,               0x05, 0x0F);
DEFINE_MSO_GUID(IID_IMsoOnObjManagerUser,           0x05, 0x10);
DEFINE_MSO_GUID(IID_IMsoOnObjControl,               0x05, 0x11);
DEFINE_MSO_GUID(IID_IMsoOnObjControlUser,           0x05, 0x12);
DEFINE_MSO_GUID(IID_IWDWizard,                  0x05, 0x13);
DEFINE_MSO_GUID(IID_IMsoMallocEx,                   0x05, 0x14);

// Category 06: Component Integration GUIDs
DEFINE_MSO_GUID(IID_IMsoComponent,              0x06, 0x00);
DEFINE_MSO_GUID(IID_IMsoComponentManager,       0x06, 0x01);
DEFINE_MSO_GUID(IID_IMsoStdComponentMgr,        0x06, 0x02);
DEFINE_MSO_GUID(IID_IMsoComponentHost,          0x06, 0x03);
DEFINE_MSO_GUID(IID_IMsoInPlaceComponent,       0x06, 0x04);
DEFINE_MSO_GUID(IID_IMsoInPlaceComponentSite,   0x06, 0x05);
DEFINE_MSO_GUID(IID_IMsoComponentUIManager,     0x06, 0x06);
DEFINE_MSO_GUID(IID_IMsoSimpleRecorder,         0x06, 0x07);
DEFINE_MSO_GUID(SID_SMsoComponentUIManager,     0x06, 0x08);
DEFINE_MSO_GUID(SID_SMsoSimpleRecorder,         0x06, 0x09);
DEFINE_MSO_GUID(IID_IMsoInPlaceComponentUIManager, 0x06, 0x0A);
DEFINE_MSO_GUID(SID_SMsoComponentManager,       0x06, 0x0B);
DEFINE_MSO_GUID(IID_IMsoHlinkPrxy,              0x06, 0x0C);

// Category 07: Web Toolbar GUIDs
DEFINE_MSO_GUID(IID_IMsoWebToolbarHelper,       0x07, 0x00);
DEFINE_MSO_GUID(IID_IMsoWebToolbarHelperUser,   0x07, 0x01);
DEFINE_MSO_GUID(IID_IMsoAuthorHlinkDlg,         0x07, 0x02);

// Category 08: Office 9 Base/Infrastructure (JimW)

// Category 09: Office 9 Web Client (WillK/CathySax/KBrown)
DEFINE_MSO_GUID(IID_IMsoHTMLImport,             0x09, 0x00);
DEFINE_MSO_GUID(IID_IMsoHTMLImportUser,         0x09, 0x01);
DEFINE_MSO_GUID(IID_IMsoHTMLExport,             0x09, 0x02);
DEFINE_MSO_GUID(IID_IMsoHTMLExportUser,         0x09, 0x03);

DEFINE_MSO_GUID(IID_IMsoHTMLPropertyBag,        0x09, 0x04);

DEFINE_MSO_GUID(IID_IMsoThemeList,              0x09, 0x05);
DEFINE_MSO_GUID(IID_IMsoTheme,                  0x09, 0x06);
DEFINE_MSO_GUID(IID_IMsoWebThemeDlg,            0x09, 0x07);

DEFINE_MSO_GUID(IID_IMsoHTMLExportSet,          0x09, 0x08);
DEFINE_MSO_GUID(IID_IMsoHTMLExportSetUser,      0x09, 0x09);

DEFINE_MSO_GUID(IID_IMsoCSSImportUser,          0x09, 0x0A);
DEFINE_MSO_GUID(IID_IMsoCSSImportAdvancedUser,  0x09, 0x0B);

DEFINE_MSO_GUID(IID_IMsoWebOptionsDlg,			0x09, 0x0C);
DEFINE_MSO_GUID(IID_IMsoHTMLOcxHelper,          0x09, 0x0D);
DEFINE_MSO_GUID(IID_IMsoThemePage,              0x09, 0x0E);

DEFINE_MSO_GUID(IID_IMsoHTMLFileNameTable,      0x09, 0x0F);

DEFINE_MSO_GUID(IID_IMsoSaveAsWebPageDlg,       0x09, 0x10);
DEFINE_MSO_GUID(IID_IMsoLocationMru,            0x09, 0x11);
DEFINE_MSO_GUID(IID_IMsoThemeFontMap,           0x09, 0x12);

DEFINE_MSO_GUID(IID_IWebPageFont,               0x09, 0x13);
DEFINE_MSO_GUID(IID_IWebPageFonts,              0x09, 0x14);

DEFINE_MSO_GUID(IID_IMsoUrl,                    0x09, 0x15);
DEFINE_MSO_GUID(IID_IMsoHyperlink,              0x09, 0x16);

DEFINE_MSO_GUID(IID_IMsoSearchManager,              0x09, 0x17);
DEFINE_MSO_GUID(IID_IMsoSearchExecutor,             0x09, 0x18);
DEFINE_MSO_GUID(IID_IMsoSearchScope,                0x09, 0x19);
DEFINE_MSO_GUID(IID_IMsoSearchScope2,               0x09, 0x1A);
DEFINE_MSO_GUID(IID_IMsoSearchScopeEx,              0x09, 0x1B);
DEFINE_MSO_GUID(IID_IMsoSearchHandler,              0x09, 0x1C);
DEFINE_MSO_GUID(IID_IMsoSearchDefinition,           0x09, 0x1D);
DEFINE_MSO_GUID(IID_IMsoSearchDefinition2,          0x09, 0x1E);
DEFINE_MSO_GUID(IID_IMsoSearchDefinitionEx,         0x09, 0x1F);
DEFINE_MSO_GUID(IID_IMsoSearchClause,               0x09, 0x20);
DEFINE_MSO_GUID(IID_IMsoSearchFileType,             0x09, 0x21);
DEFINE_MSO_GUID(IID_IMsoSearchColumn,               0x09, 0x22);
DEFINE_MSO_GUID(IID_IMsoSearchLocation,             0x09, 0x23);
DEFINE_MSO_GUID(IID_IMsoSearchResult,               0x09, 0x24);
DEFINE_MSO_GUID(IID_IMsoSearchAction,               0x09, 0x25);
DEFINE_MSO_GUID(IID_IMsoSearchActionEx,             0x09, 0x26);
DEFINE_MSO_GUID(IID_IMsoSearchClient,               0x09, 0x27);
DEFINE_MSO_GUID(IID_IMsoSearchClientEx,             0x09, 0x28);
DEFINE_MSO_GUID(IID_IMsoSearchDebug,                0x09, 0x29);
DEFINE_MSO_GUID(IID_IMsoSearchAbsoluteLocation,     0x09, 0x2A);
DEFINE_MSO_GUID(IID_IEnumMsoSearchScope,            0x09, 0x2B);
DEFINE_MSO_GUID(IID_IEnumMsoSearchScopeEx,          0x09, 0x2C);
DEFINE_MSO_GUID(IID_IEnumMsoSearchClause,           0x09, 0x2D);
DEFINE_MSO_GUID(IID_IEnumMsoSearchFileType,         0x09, 0x2E);
DEFINE_MSO_GUID(IID_IEnumMsoSearchColumn,           0x09, 0x2F);
DEFINE_MSO_GUID(IID_IEnumMsoSearchLocation,         0x09, 0x30);
DEFINE_MSO_GUID(IID_IEnumMsoSearchResult,           0x09, 0x31);
DEFINE_MSO_GUID(IID_IEnumMsoSearchAction,           0x09, 0x32);
DEFINE_MSO_GUID(IID_IEnumMsoSearchActionEx,         0x09, 0x33);
DEFINE_MSO_GUID(IID_IEnumMsoSearchAbsoluteLocation, 0x09, 0x34);
DEFINE_MSO_GUID(IID_IMsoFileNewProp,                0x09, 0x35);
DEFINE_MSO_GUID(IID_StartWorking,                   0x09, 0x36);
DEFINE_MSO_GUID(IID_IMsoPropertyBag2,               0x09, 0x37);
DEFINE_MSO_GUID(IID_IMsoSearchClient2,              0x09, 0x38);
DEFINE_MSO_GUID(IID_IMsoSearchFileType2,            0x09, 0x39);

// Category 0A is reserved for Project (MMaruch)
// Category 0B is reserved for Project (MMaruch)
// Category 0C is reserved for Project (MMaruch)

// Category 10: Darwin Installer Components (JohnDelo)

// Category 11: Darwin Tools, Tests, Authoring (JohnDelo)

// Category 12: Office Scripting Package GUIDs	(IgorZ)

// These interfaces were used for Office2000 MSE
DEFINE_MSO_GUID(CLSID_MsoVsPackage9,			0x12, 0x00);
DEFINE_MSO_GUID(CLSID_MsoVsProject9,			0x12, 0x01);
DEFINE_MSO_GUID(IID_IMsoVsPackage9,				0x12, 0x02);	// DANGER: This definition should match msoitf.idl and selfreg.h
DEFINE_MSO_GUID(IID_IMsoVsProject9,				0x12, 0x03);	// DANGER: This definition should match msoitf.idl and selfreg.h
DEFINE_MSO_GUID(IID_IMsoVsProjectSite9,			0x12, 0x04);	// DANGER: This definition should match msoitf.idl and selfreg.h
DEFINE_MSO_GUID(CLSID_MsoVseProxy9,				0x12, 0x05);
DEFINE_MSO_GUID(IID_IMsoVsProjectProxy9,		0x12, 0x06);
//DEFINE_MSO_GUID(IID_IHtmMsoHtmlPersist,		0x12, 0x07);
																// This interface is used to communicate between htmed.dll and msovse.dll
																// and does not require marshalling. It is defined here to make sure
																// that we do not reuse its GUID.
DEFINE_MSO_GUID(IID_IMsoVsSetForegroundWindow9,	0x12, 0x08);
DEFINE_MSO_GUID(IID_IMsoVsProjectProxySite9,	0x12, 0x09);

// Office10 MSE interfaces
DEFINE_MSO_GUID(IID_IMsoVsPackage10,			0x12, 0x12);	// DANGER: This definition should match msoitf.idl
DEFINE_MSO_GUID(IID_IMsoVsProject10,			0x12, 0x13);	// DANGER: This definition should match msoitf.idl
DEFINE_MSO_GUID(IID_IMsoVsProjectSite10,		0x12, 0x14);	// DANGER: This definition should match msoitf.idl
DEFINE_MSO_GUID(CLSID_MsoVseProxy10,			0x12, 0x15);
DEFINE_MSO_GUID(IID_IMsoVsProjectProxy10,		0x12, 0x16);
DEFINE_MSO_GUID(CLSID_MsoVsMenuPackage10,		0x12, 0x17);
DEFINE_MSO_GUID(IID_IMsoVsSetForegroundWindow10,0x12, 0x18);
DEFINE_MSO_GUID(IID_IMsoVsProjectProxySite10,	0x12, 0x19);
DEFINE_MSO_GUID(CLSID_MsoVsPackageClassObject10,0x12, 0x1A);

// current MSE interfaces defines

// note that these CLSIDs do not rev. since this registration goes
// under AppId reg. root and therefore does not have to change.
#define CLSID_MsoVsPackage				CLSID_MsoVsPackage9
#define CLSID_MsoVsProject				CLSID_MsoVsProject9

#define IID_IMsoVsPackage				IID_IMsoVsPackage10
#define IID_IMsoVsProject				IID_IMsoVsProject10
#define IID_IMsoVsProjectSite			IID_IMsoVsProjectSite10
#define CLSID_MsoVseProxy				CLSID_MsoVseProxy10
#define IID_IMsoVsProjectProxy			IID_IMsoVsProjectProxy10
#define CLSID_MsoVsMenuPackage			CLSID_MsoVsMenuPackage10
#define IID_IMsoVsSetForegroundWindow	IID_IMsoVsSetForegroundWindow10
#define IID_IMsoVsProjectProxySite		IID_IMsoVsProjectProxySite10
#define CLSID_MsoVsPackageClassObject	CLSID_MsoVsPackageClassObject10


DEFINE_MSO_GUID(UICONTEXT_MSOVS_SYNCH,			0x12, 0x50);
DEFINE_MSO_GUID(guidMsoVsCmds,					0x12, 0x51);

// Category 13: TCO Team (MikeKell)
DEFINE_MSO_GUID(CLSID_MsoSoftDistExt,			0x13, 0x00);
DEFINE_MSO_GUID(IID_IMsoLicense,			0x13, 0x01);

// Category 14: is reserved for Office Designer (IgorZ, KKahl)
// DONT ADD ANYTHING HERE UNLESS YOU KNOW WHAT YOU'RE DOING
DEFINE_MSO_GUID(IID_IODTransactionHelper, 0x14, 0x84);
DEFINE_MSO_GUID(IID_IODInterfaceReference, 0x14, 0x86);

// Category 15: Office Data Source Object (ODSO) 
DEFINE_MSO_GUID(IID_IMsoOdso, 0x15, 0x00);
DEFINE_MSO_GUID(IID_IMsoOdsoColumn, 0x15, 0x01);
DEFINE_MSO_GUID(IID_IMsoOdsoPersist, 0x15, 0x02);
DEFINE_MSO_GUID(IID_IMsoOdsoRowset, 0x15, 0x03);
DEFINE_MSO_GUID(IID_IMsoOdsoConvert, 0x15, 0x04);
DEFINE_MSO_GUID(IID_IMsoMmMigrate, 0x15, 0x05);
DEFINE_MSO_GUID(IID_IMsoOdsoClientSite, 0x15, 0x06);
DEFINE_MSO_GUID(IID_IMsoOdsoClientSiteMigrate, 0x15, 0x07);
DEFINE_MSO_GUID(IID_IMsoMailMerge, 0x15, 0x08);
DEFINE_MSO_GUID(IID_IMsoOdsoConnInfo, 0x15, 0x09);
DEFINE_MSO_GUID(IID_IMsoMailMergeInsertField, 0x15, 0x0A);
DEFINE_MSO_GUID(IID_IMsoMailMergeShowRecord, 0x15, 0x0B);
DEFINE_MSO_GUID(IID_IMsoMailMergeClientSite, 0x15, 0x0C);
DEFINE_MSO_GUID(IID_IMsoMailMergeWordClientSite, 0x15, 0x0D);
//  -- What about 0x0E - 0x0F?
DEFINE_MSO_GUID(IID_IMsoOdsoFileOpen, 0x15, 0x10);

// ODSO automation 
DEFINE_MSO_GUID(IID_IMsoDispOdso, 0x15, 0x30);
DEFINE_MSO_GUID(IID_IMsoDispOdsoColumn, 0x15, 0x31);
DEFINE_MSO_GUID(IID_IMsoDispOdsoColumns, 0x15, 0x32);
DEFINE_MSO_GUID(IID_IMsoDispOdsoFilter, 0x15, 0x33);
DEFINE_MSO_GUID(IID_IMsoDispOdsoFilters, 0x15, 0x34);

// Category 16: Insert Web Page Component (VinnyRom)
DEFINE_MSO_GUID(IID_IMsoWPC, 0x16, 0x00);
DEFINE_MSO_GUID(IID_IMsoWPCGroup, 0x16, 0x01);
DEFINE_MSO_GUID(IID_IMsoWPCUser, 0x16, 0x02);
DEFINE_MSO_GUID(IID_IMsoWPCGroupUser, 0x16, 0x03);
DEFINE_MSO_GUID(IID_IMsoWPCDialogUser, 0x16, 0x04);
DEFINE_MSO_GUID(IID_IMsoNavBar, 0x16, 0x05);
DEFINE_MSO_GUID(IID_IMsoNavBars, 0x16, 0x06);
DEFINE_MSO_GUID(IID_IMsoListView, 0x16, 0x07);
DEFINE_MSO_GUID(IID_IMsoListViews, 0x16, 0x08);

// Category A0: Office Activation (animation) (ElaineLa)
DEFINE_MSO_GUID(IID_IMsoTimeNode, 					0xA0, 0x00);
DEFINE_MSO_GUID(IID_IMsoTimeStructure, 			    0xA0, 0x01);
DEFINE_MSO_GUID(IID_IMsoTimeStructureListener,	    0xA0, 0x02);
DEFINE_MSO_GUID(IID_IMsoTimePropertyListener, 	    0xA0, 0x03);
DEFINE_MSO_GUID(IID_IMsoTimeCondition, 			    0xA0, 0x04);
DEFINE_MSO_GUID(IID_IMsoTimeBehavior, 			    0xA0, 0x05);
DEFINE_MSO_GUID(IID_IMsoTimeVisualElement, 		    0xA0, 0x06);
DEFINE_MSO_GUID(IID_IMsoTimeHostView, 		        0xA0, 0x07);
DEFINE_MSO_GUID(IID_IMsoTimeSelectionListener,      0xA0, 0x08);
DEFINE_MSO_GUID(IID_IMsoTimeHostDocument, 		    0xA0, 0x09);
DEFINE_MSO_GUID(IID_IMsoTimeList,           		0xA0, 0x10);
DEFINE_MSO_GUID(IID_IMsoTimeStringList,       		0xA0, 0x11);
DEFINE_MSO_GUID(IID_IMsoTimePropertyList,     		0xA0, 0x14);
DEFINE_MSO_GUID(IID_IMsoTimePropertyFixedList, 	    0xA0, 0x15);
DEFINE_MSO_GUID(IID_IMsoTimeAnimationValueList,  	0xA0, 0x16);
DEFINE_MSO_GUID(IID_IMsoTimeSequenceNode,    		0xA0, 0x18);
DEFINE_MSO_GUID(IID_IMsoTimeNodeGroup,    		0xA0, 0x19);

DEFINE_MSO_GUID(IID_IMsoTimeAnimateBehavior, 				0xA0, 0x20);
DEFINE_MSO_GUID(IID_IMsoTimeAnimateMotionBehavior, 	    	0xA0, 0x21);
DEFINE_MSO_GUID(IID_IMsoTimeAnimateEffectBehavior, 		    0xA0, 0x22);
DEFINE_MSO_GUID(IID_IMsoTimeAnimateColorBehavior,	 		0xA0, 0x23);
DEFINE_MSO_GUID(IID_IMsoTimeAnimateRotationBehavior, 		0xA0, 0x24);
DEFINE_MSO_GUID(IID_IMsoTimeAnimateScaleBehavior, 			0xA0, 0x25);
DEFINE_MSO_GUID(IID_IMsoTimeSetBehavior, 					0xA0, 0x26);
DEFINE_MSO_GUID(IID_IMsoTimeMediaElement, 			        0xA0, 0x27);
DEFINE_MSO_GUID(IID_IMsoTimeIterateData,                    0xA0, 0x28);
DEFINE_MSO_GUID(IID_IMsoTimePPTBehavior,                    0xA0, 0x29);
DEFINE_MSO_GUID(IID_IMsoTimeCommandBehavior,                0xA0, 0x2a);

// Category D0 is used by the Doc Management group.
DEFINE_MSO95_GUID(LIBID_Office, 						0xD0, 0x4C);
DEFINE_MSO95_GUID(IID_DocumentProperty,			0xD0, 0x4E);
DEFINE_MSO95_GUID(IID_DocumentProperties, 		0xD0, 0x4D);
DEFINE_MSO95_GUID(CLSID_Office, 						0xD0, 0x54);

// Category D1 is used by Web Components
DEFINE_MSO_GUID(IID_WebComponent,                  0xD1, 0x00);
DEFINE_MSO_GUID(IID_WebComponentWindowExternal,    0xD1, 0x01);
DEFINE_MSO_GUID(IID_WebComponentFormat,            0xD1, 0x02);      

// Category FE is reserved for DAO
// Category FF is reserved for DAO


// WordMail stuff is in mso9.dll now that docobj.dll is dead.
EXTERN_C const GUID IID_IMsoMailSite
#ifdef INIT_MSO_GUIDS
= {0xb722bccd,0x4e68,0x101b,{0xa2,0xbc,0x00,0xaa,0x00,0x40,0x47,0x70}}
#endif
;
EXTERN_C const GUID IID_IMsoMailSite2
#ifdef INIT_MSO_GUIDS
= {0x00067063,0x0000,0x0000,{0xC0,0x00,0x00,0x00,0x00,0x00,0x00,0x46}}
#endif
;
EXTERN_C const GUID IID_IMsoMailEditor
#ifdef INIT_MSO_GUIDS
= {0xb722bcce,0x4e68,0x101b,{0xa2,0xbc,0x00,0xaa,0x00,0x40,0x47,0x70}}
#endif
;
EXTERN_C const GUID IID_IMsoMailEditor2
#ifdef INIT_MSO_GUIDS
= {0x0006726F,0x0000,0x0000,{0xC0,0x00,0x00,0x00,0x00,0x00,0x00,0x46}}
#endif
;

// MsoEnvelope stuff in mso9.dll
EXTERN_C const GUID IID_IMsoEnvelopeVB
#ifdef INIT_MSO_GUIDS
= {0x000672AC,0x0000,0x0000,{0xC0,0x00,0x00,0x00,0x00,0x00,0x00,0x46}}
#endif
;
EXTERN_C const GUID IID_IMsoEnvelopeVBEvents
#ifdef INIT_MSO_GUIDS
= {0x000672AD,0x0000,0x0000,{0xC0,0x00,0x00,0x00,0x00,0x00,0x00,0x46}}
#endif
;

// Some Binder-related stuff.
EXTERN_C const GUID IID_IMsoHdrFtrProvider
#ifdef INIT_MSO_GUIDS
= {0xB722BCD2,0x4E68,0x101B,{0xA2,0xBC,0x00,0xAA,0x00,0x40,0x47,0x70}}
#endif
;
EXTERN_C const GUID IID_IMsoHdrFtrClient
#ifdef INIT_MSO_GUIDS
= {0xB722BCD3,0x4E68,0x101B,{0xA2,0xBC,0x00,0xAA,0x00,0x40,0x47,0x70}}
#endif
;
EXTERN_C const GUID IID_IMsoInplacePrintPreviewCallback
#ifdef INIT_MSO_GUIDS
= {0xB722BCD5,0x4E68,0x101B,{0xA2,0xBC,0x00,0xAA,0x00,0x40,0x47,0x70}}
#endif
;
EXTERN_C const GUID IID_IMsoInplacePrintPreview
#ifdef INIT_MSO_GUIDS
= {0xB722BCD4,0x4E68,0x101B,{0xA2,0xBC,0x00,0xAA,0x00,0x40,0x47,0x70}}
#endif
;
EXTERN_C const GUID IID_IMsoFormSite
#ifdef INIT_MSO_GUIDS
= {0xB722BCCC,0x4E68,0x101B,{0xA2,0xBC,0x00,0xAA,0x00,0x40,0x47,0x70}}
#endif
;

// Category D0 is used by the Doc Management group.
EXTERN_C const GUID IID_IWizardClient
#ifdef INIT_MSO_GUIDS
= {0xBDEADF03,0xC265,0x11d0,{0xbc, 0xed, 0x0, 0xa0, 0xc9, 0xa, 0xb5, 0x0f}}
#endif
;

// Used by Twain and WIA Acquisition code
EXTERN_C const GUID IID_IMsoAcquisitionMgr
#ifdef INIT_MSO_GUIDS
= {0xf6df8bfb,0x833d,0x4848,{0x8a,0x10,0x75,0xaa,0x32,0xa1,0x89,0x82}}
#endif
;

EXTERN_C const GUID IID_IMsoAcquisitionTarget
#ifdef INIT_MSO_GUIDS
= {0xf6df8bfa,0x833d,0x4848,{0x8a,0x10,0x75,0xaa,0x32,0xa1,0x89,0x82}}
#endif
;

EXTERN_C const GUID IID_IBln
#ifdef INIT_MSO_GUIDS
= {0xE0B1C713,0xE991,0x11D0,{0x9E,0xA7,0x00,0xC0,0x4F,0xD7,0x08,0x1F}};
#endif
;

EXTERN_C const GUID IID_IRedirectedMoniker
#ifdef INIT_MSO_GUIDS
= { 0xBDEBBF02, 0xC265, 0x11d0, {0xBC,0xED,0x00,0xA0,0xC9,0x0A,0xB5,0x0F}}
#endif
;

#endif	// MSOGUIDS_H
