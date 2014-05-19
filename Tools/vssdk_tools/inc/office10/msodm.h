#pragma once

 // msodm.h - Public header for Office Document Management
//
// This header contains the definitions for the Application API to Office
// File Open and the Office Librarian.
//
// The functions and classes defined here are explained in
// "Find File 2.0 API" and "IMsoOLDocument".  See these documents for details.

#ifndef __MSODM_H__
#define __MSODM_H__

#include <msostd.h>
#include <commctrl.h>

#if defined(RSH) // for Word -- they define RSH for instrumented version.
#define INSTRUMENTED
#endif // defined(RSH)

#include "msosdm.h"

// Include debug header if needed
#ifdef VSMSODEBUG
#include "msodmd.h"
#endif // VSMSODEBUG

#include "msoprops.h"   // For SetRenSummInfo
#include "shlobj.h"     // For MsoHrHttpUrlToPsfPidl
#include "msourl.h"
// Added for msoFileDialogType
#ifdef __cplusplus
#include "msotl.h"
#else  // __cplusplus
#include "msotlc.h"
#endif  // __cplusplus


// Add the DM_DLLFUNC specifier to all functions that we
// export.
#ifdef OFFICE_BUILD
#define DM_DLLFUNC __declspec(dllexport)
#else // OFFICE_BUILD
#define DM_DLLFUNC __declspec(dllimport)
#endif // OFFICE_BUILD
#define DM_DLLFUNCX


#ifdef __cplusplus
#define CONST_METHOD_FF const
#else // __cplusplus
#define CONST_METHOD_FF
#endif // __cplusplus

// Standard type of callbacks used by Office Open/Find.  Note that your
// functions must match this calltype.
#ifndef	OFFOPEN_CALLBACK
#define	OFFOPEN_CALLBACK	__cdecl
#endif // OFFOPEN_CALLBACK

// REVIEW brianwen: Do we really need our own bool, or can we just use office's?
typedef int MSOBOOL;
typedef ULONGLONG MSODMCOOKIE;

EXTERN_C const IID IID_IBindStatusCallback;

// ----------------------------------------------------------------------------
// Types and constants.


// Special HRESULT value used by IMsoSearch::GetSzFileName.

#define MSO_S_TRUNCATED				0x00350001

// Special HRESULT values used by IFindFile::GetHwnd.

#define MSO_E_DIALOG_VISIBLE			0x80350002
#define MSO_E_DIALOG_NOT_VISIBLE		0x80350003

// Error codes used with WebFind.  These will come back from
// ExecuteQuery
#define MSO_E_NO_FOLDER					0x80350004		// only 1 folder
#define MSO_E_TOO_MANY_FOLDERS		0x80350005		// only 1 folder
#define MSO_E_NO_INDEX					0x80350006		// no index in folder
#define MSO_E_NO_SEARCH					0x80350007		// no text/property search specified
#define MSO_E_SYNTAX_ERROR				0x80350008
#define	MSO_E_ERROR_DURING_SEARCH		0x80350010		// error while executing search.

// Special HRESULT value used by IMsoOLDocument::GetMoniker.

#define MSO_E_IOL_NO_ODMA_MONIKER	0x80350009
#define MSO_E_IOL_ODMA_NOT_AVAIL		0x8035000a

// Special HRESULT value used by IDMBindStatusCallback.

#define MSO_S_BSC_BINARY				0x00350002

// Special HRESULT values used by IMsoOLDocument::BeginCmd

#define MSO_S_IOLCMD_NONE				0x00350000				// Not used by apps
#define MSO_S_IOLCMD_FIRST				0x00350010
#define MSO_S_IOLCMD_OPEN				MSO_S_IOLCMD_FIRST
#define MSO_S_IOLCMD_OPENRO			(MSO_S_IOLCMD_FIRST + 0x01)
#define MSO_S_IOLCMD_NEW				(MSO_S_IOLCMD_FIRST + 0x02)
#define MSO_S_IOLCMD_CHECK_IN			(MSO_S_IOLCMD_FIRST + 0x03)
#define MSO_S_IOLCMD_CHECK_OUT		(MSO_S_IOLCMD_FIRST + 0x04)
#define MSO_S_IOLCMD_SAVE				(MSO_S_IOLCMD_FIRST + 0x05)
#define MSO_S_IOLCMD_SAVE_AS_DLG		(MSO_S_IOLCMD_FIRST + 0x06)
#define MSO_S_IOLCMD_SAVE_VERSION	(MSO_S_IOLCMD_FIRST + 0x07)
#define MSO_S_IOLCMD_CLOSE				(MSO_S_IOLCMD_FIRST + 0x08)
#define MSO_S_IOLCMD_APP_MUST_MERGE	(MSO_S_IOLCMD_FIRST + 0x09)
#define MSO_S_IOLCMD_ROLLBACK			(MSO_S_IOLCMD_FIRST + 0x0a)
#define MSO_S_IOLCMD_CANCEL			(MSO_S_IOLCMD_FIRST + 0x0b)
#define MSO_S_IOLCMD_REFRESH			(MSO_S_IOLCMD_FIRST + 0x0c)
#define MSO_S_IOLCMD_SAVE_COPY		(MSO_S_IOLCMD_FIRST + 0x0d)
#define MSO_S_IOLCMD_LIM				(MSO_S_IOLCMD_FIRST + 0x0e) // Must be last

// Error HRESULT values used by IMsoOLDocument::BeginCmd
#define MSO_E_IOLCMD_ALREADY_OPENRO		0x8035000b

// Standard dialog items.

typedef int MSOIOTM;

enum
	{
	msodmiotmMin = -13L,
	msodmiotmAnyTextMRU,	// Any Text edit MRU.
	msodmiotmFileNameMRU,	// File name edit control MRU
	msodmiotmLocation,		// Location drop list.
	msodmiotmFileList,		// File display listbox.
	msodmiotmFileName,		// File name edit control.
	msodmiotmFileType,	 	// File type drop list.
	msodmiotmLastModified,	// Last Modified drop down.
	msodmiotmAnyText,		// Any Text edit.
	msodmiotmFindNow,		// Find now button.
	msodmiotmClear,			// Clear button.
	msodmiotmOpenButton,	// Open button.
	msodmiotmCancelButton,	// Cancel button.
	msodmiotmMax			// MUST BE LAST, should be zero if FF_iotmMin right.
	};

// Buffer lengths.

enum
	{
	msodmcchMaxDlgTitle	= 128,	// Dialog title (in title bar).
	msodmcchMaxItemTitle	= 52,	// Item title, e.g. "Open" for the open button.
	msodmcchMaxSearchName= 64,	// Saved search name.
	msodmcchMaxType		= 512,	// File type.  Includes description and space
								// for types (eg. *.doc).
	msodmcchMaxEdit		= 256,	// Maximum length of an edit control value.
	msodmcchMaxCommand	= 52, 	// Maximum length of a command.
	msodmcchMaxCodePage	= 64		// Maximum length of a code page.
	};

// Custom item types.

typedef int MSOOTP;

enum
	{
	msodmotpNil = 0,
	msodmotpEdit,					// Edit control.
	msodmotpCommand,				// Command (push) button.
	msodmotpDescrText,				// Descriptive text.
	msodmotpCheckbox,				// Check box.
	msodmotpRadioButton,			// Radio button in single radio group.
	msodmotpGroupBox,				// Group box.
	msodmotpToolsMenuItem,			// Tools menu item
	msodmotpToolsMenuDismissItem,	// Tools menu item that dismisses the dialog.
	msodmotpToolsMenuCheckbox, 		// Tools menu radio button
	msodmotpOpenBtnDropDnItem, 		// Open button dropdown Item:
	msodmotpOpenBtnDropDnCheckbox, 	// Open button dropdown radio button
	msodmotpStdButton,				// Standard push button.
	msodmotpPushButton,				// Normal push button.
	msodmotpListBox,				// List box.
	msodmotpDropList,				// Drop down.
	msodmotpComboBox,				// Combo box.
	msodmotpConvertButton,			// Convert button.
	msodmotpMax
	};


// Event operation codes.

typedef int MSOIOOP;

enum
	{
	msodmioopChange = 0,		// A standard or custom control was modified.
	msodmioopCommand,			// An application defined command was selected.
	msodmioopDismiss,			// The dialog is about to be dismissed.
	msodmioopQueryFinished,		// A query has just finished executing.
	msodmioopDefSaveHelp,		// Help requested from default save alert.
	msodmioopDefSavePrompt,		// We are prompting user to save in admin format.
	msodmioopMax				// MUST BE LAST
	};

// File system change event operation codes.
//
// For each system change event (Rename, Delete, or MakeDir), a "Pre" event
// (eg. msodmfnPreRename) is sent.  If the app returns MSODMFSCHANGE_YOU_DO_IT,
// then office does the action (eg. Rename), and sends a Failed or Succeeded event
// (eg. msodmfnRenameFailed or msodmfnRenameSucceeded).  This gives the
// app the opportunity to completely override the rename, delete, and makedir
// verbs, or to allow Office to do the rename, delete, and MakeDir, and have
// the app notified of the success or failure.

typedef int MSODMFSCHANGE;

enum
	{
	msodmfnPreRename = 0,		// A file needs to be renamed from wzOld to wzNew
	msodmfnRenameFailed,		// Office tried to rename from wzOld to wzNew and failed
	msodmfnRenameSucceeded,		// Office renamed from wzOld to wzNew successfully
	msodmfnPreDelete,				// A file in wzOld needs to be deleted
	msodmfnDeleteFailed,		// Office tried to delete wzOld but failed
	msodmfnDeleteSucceeded,		// Office deleted wzOld
	msodmfnPreMakeDir,				// A subdirectory named wzNew needs to be created
	msodmfnMakeDirFailed,		// Office tried to create subdirectory wzNew and failed
	msodmfnMakeDirSucceeded,	// Office created subdirectory wzNew
	msodmfnMax					// MUST BE LAST
	};

//	MSODMFSCHANGE_YOU_DO_IT		Office should handle it.
//	MSODMFSCHANGE_I_DID_IT		App handled it.
#define	MSODMFSCHANGE_YOU_DO_IT			FALSE
#define	MSODMFSCHANGE_I_DID_IT			TRUE
// Confirm replacement dialog options for save dialog


// Web-document Server Info codes
// 
// The structures below were added to tell us what type of server 
// the URL in the OLdoc points to, and in what protocol the server
// is speaking (dav or wec).  These structures are used with
// IMsoOLDoc::GetServerInfo.

typedef int MSODMSERVERTYPE;

enum
	{
	msodmsvrtypeUnknown = 0,  // The specific servertype is unknown, however we may still know dav/wec
	msodmsvrtypeOWS,				// OWS
	msodmsvrtypePlatinum,	// Exchange Platinum
	msodmsvrtypeTahoe,	// Tahoe
	msodmsvrtypeTahoeEnhancedFolder, // Tahoe Enhanced Folder
	msodmsvrtypeMax  // MUST BE LAST
	};

typedef int MSODMSERVERPROTOCOL;

enum
	{
	msodmsvrprotUnknown = 0, // Server protocol is unknown
	msodmsvrprotWEC,	// Server protocol is WEC
	msodmsvrprotDAV,  // Server protocl is DAV
	msodmsvrprotMax  // MUST BE LAST
	};

typedef struct tagMSODMGSI
{
	MSODMSERVERTYPE type;
	MSODMSERVERPROTOCOL protocol;
} MSODMGSI;


// GetServerInfo flags

typedef UINT MSOGSIOPT;

enum
	{
	msodmgsiFailIfNotCached = 1,  // Ensures we will not roundtrip to server, instead we will return a fail code
	msodmgsiMax = 2 // MUST BE LAST
	};


typedef int MSODMCR;

enum
	{
	msodmcrAlways = 0,			// Always display confirm replace dialog on save
								// equivalent of SetFConfirmReplace(TRUE)
	msodmcrNever,				// Never display confirm replace dialog.
								// equivalent of SetFConfirmReplace(FALSE)
	msodmcrOnlyForURL,			// Confirm replacement only of URLs
	msodmcrMax					// MUST BE LAST
	};

// Miscellaneous constants

enum { msodmccstitmMax = 10 };	// Maximum number of custom items allowed.

enum { msodmiszNil = -1 };		// No selected file type.

// Command values returned to the application.
enum
	{
	msodmicmdNone	   = 0,
	msodmicmdCancel    = 1,           // Cancel or close pressed (also esc. key)
	msodmicmdOpen,					// Normal open
	msodmicmdOpenInNativeApp,		// Open drop down - "open in current app"
	msodmicmdShiftOpen,				// Open with the shift key pressed
	msodmicmdOpenRO,					// Open read only from the command button
	msodmicmdOpenAsCopy,					// Open As Copy from the command button
	msodmicmdPrint,					// Print from the command button
	msodmicmdMax,
	msodmicmdCmdMask 	 = 0xff,	// Masks actual command bits.
	// The following value may be ORed with other msodmicmd values returned to
	// the application.
	msodmicmdExecQuery = 0x100,		// Indicates that query should be executed.
	msodmicmdEndDlgOk = 0x200,
	msodmicmdEndDlgCancel = 0x400,

	// The application may also use the 13-16 least significant bits to
	// store application defined information.
	msodmicmdUserMin   = 0x1000,		// User bits.
	msodmicmdUserMask  = 0x7000
	};

// Indices of entries in the commands dropdown in the Open dialog.  These
// indices are used to enable or disable commands.
typedef int MSODMICLE;

enum
	{
	msodmicleOpenDefault = 0,	// Perform default button action (Open/Save)
	msodmicleOpenRO,				// Open read only from the command button
	msodmicleOpenAsCopy,			// Open As Copy from the command button
	msodmicleSorting,				// Sorting command list entry
	msodmicleIncludeSubfolders,		// Include Subfolders command list entry
	msodmicleShowGroups,			// Show Groups command list entry
	msodmicleFtpSites,			// Open the FTP sites control dlg
	msodmicleToggleFuzzy,		// Toggle fuzzy find on/off (FOR DBCS)
	msodmicleOpenInBrowser,		// Launch in default browser
	msodmicleOpenInNativeApp,	// Open the file in native app of dialog
	msodmicleFind,				// Bring up the Find dialog
	msodmicleDelete,			// Delete file
	msodmicleRename,				// Rename file in place
	msodmiclePrint,					// Print from the command button
	msodmicleAddToFavs,				// Add to Favorites
	msodmicleAddToPlaces,			// Add to My Places
	msodmicleMapNetworkDrive,		// Map Network Drive command list entry
	msodmicleProperties,			// Properties command list entry

	// App specified items for the Tools menu
	msodmicleAppToolsFirst,
	msodmicleAppTools1 = msodmicleAppToolsFirst,
	msodmicleAppTools2,
	msodmicleAppTools3,
	msodmicleAppTools4,
	msodmicleAppTools5,
	msodmicleAppTools6,
	msodmicleAppTools7,
	msodmicleAppTools8,
	msodmicleAppTools9,
	msodmicleAppToolsLast = msodmicleAppTools9,

	// App specified items for the Open Button Dropdown menu
	msodmicleAppOpenDropdnFirst,
	msodmicleAppOpenDropdn1 = msodmicleAppOpenDropdnFirst,
	msodmicleAppOpenDropdn2,
	msodmicleAppOpenDropdn3,
	msodmicleAppOpenDropdn4,
	msodmicleAppOpenDropdn5,
	msodmicleAppOpenDropdn6,
	msodmicleAppOpenDropdn7,
	msodmicleAppOpenDropdn8,
	msodmicleAppOpenDropdn9,
	msodmicleAppOpenDropdnLast = msodmicleAppOpenDropdn9,

	msodmicleClearHistory,			// Clear the shortcuts in History Folder
	msodmicleManage,				// Manage webdrives

	msodmicleMax
	};


typedef enum
	{
	msodmiAppNil=-1,
	msodmiAppWord,
	msodmiAppExcel,
	msodmiAppPowerPoint,
	msodmiAppAccess,
	msodmiAppFrontPage,
	msodmiAppPublisher,
	msodmiAppWordPad,
	msodmiAppExplorer,
	msodmiAppNonMS,
	msodmiAppNonMSBrowser,
	msodmcAPPS
	} MSODMAPPID;

// Standard properties for calls which get and set the criteria for these
// properties.  For Word macro language and WebFind only.

typedef int MSOSTDPROP;

enum
	{
	msodmstdpropTitle,			// Title property.
	msodmstdpropAuthor,			// Author property.
	msodmstdpropKeywords,		// Keywords property
	msodmstdpropSubject,		// Subject property
	msodmstdpropText,			// Text property
	msodmstdpropDateSavedFrom,	// Starting saved date
	msodmstdpropDateSavedTo,	// Ending saved date
	msodmstdpropSavedBy,		// Saved by
	msodmstdpropDateCreatedFrom,// Starting created date
	msodmstdpropDateCreatedTo,	// Ending created date
	msodmstdpropCreatedBy,		// Created by
	msodmstdpropSubDir,			// Include subdirectories
	msodmstdpropMatchCase,		// Match Case in text searches
	msodmstdpropPatternMatch,	// Treat text as a pattern
	msodmstdpropOmitAlwaysAccurate,	// skip always accurate part of search
#ifdef SHILSHOLE_ENABLED
	msodmstdpropRelevance,		// Relevance property
	msodmstdpropWebSearch,		// Searching web, don't apply directory screen.
	msodmstdpropHttpURL,		// want http: URLs returned (otherwise get file)
								// only used with Web Search.
#endif // SHILSHOLE_ENABLED
	msodmstdpropComments,		// Comments
	msodmstdpropCategory,		// Category of document

#ifdef SHILSHOLE_ENABLED
	// Following values are for WebFind retrieval only.  DON'T use with
	// WordBasic.
	msodmstdpropSize,
	msodmstdpropContents,
	msodmstdpropAppName,

	msodmstdpropExtendedProperties,		// Return extended property list.
									// For Task Center.
	msodmstdpropDumpDocList,		// For Yahoo categorization, return entire
									// doclist.
	msodmstdpropStartingHit,		// Start returning hits at this item (default 0).
	msodmstdpropNumberHits,			// Limit return to n hits (default all hits).
#endif // SHILSHOLE_ENABLED

	msodmstdpropMax				// MUST BE LAST
 	};


// Sort fields  For Word macro language only.  Note order
// is set to match WordBasic macro values.

typedef int MSOSORT;

enum
	{
	msodmsortOther=-1,			// Sorting by some non-standard property.
	msodmsortAuthor,			// Author property.
	msodmsortCreationDate,		// Creation Date property.
	msodmsortSavedBy,			// Last Modifier property.
	msodmsortSavedDate,			// Last Modified property.
	msodmsortFilename,			// Filename property.
	msodmsortSize,				// Size property.
#ifdef SHILSHOLE_ENABLED
	msodmsortRelevance,			// Relevance property.
#endif // SHILSHOLE_ENABLED
	msodmsortFileType,			// File Type property
	msodmsortMax				// MUST BE LAST
	};

// Views available in the dialog.  This is used to set which type
// of view is visible.  Note order is set to match WordBasic values.

typedef int MSOVIEW;

enum
	{
	msodmviewDetails,		// Details view.
	msodmviewPreview,		// Preview view.
	msodmviewProperties,	// Properties view.
	msodmviewList,			// List view.
	msodmviewThumbnail,		// Thumbnail view.
	msodmviewLargeIcons,	// Large Icons view.
	msodmviewSmallIcons,	// Small Icons view.
	msodmviewWebView,
	msodmviewMax			// MUST BE LAST
	};


// The different fonts you can set in the dialog.

typedef int MSOFONT;

enum
	{
	msodmfontNormal,		// Normal font.
	msodmfontLight,			// Light dialog font.
	msodmfontBold,			// Bold dialog font.
	msodmfontItalic,		// Italics.
	msodmfontMax			// Leave as last.
	};


// Search Connector options

typedef int MSODMCONN;

enum
	{
	msodmconnOr,
	msodmconnAnd,
	msodmconnMax			// Leave as last.
	};


// Three layout styles to enable extra sections for SaveAsWebPage.

typedef int MSODMLAYOUT;

enum {
	msodmlayoutNormal,
	msodmlayoutOneExtraGroup,
	msodmlayoutTwoExtraGroups
	};


// File Type flags (for describing file types appended to
// the file type list using IMsoFileType::AppendEx

typedef ULONG MSOFTF;


#define	msoftfNone					0x0
#define msoftfVersionMask			0x000000FF		// file type version bits
#define	msoftfNotAppDocType			0x00000000		// Not a native app type
#define	msoftfVersion1Doc			0x00000001		// From app v1
#define	msoftfVersion2Doc			0x00000002		// From app v2
#define	msoftfVersion3Doc			0x00000003		// From app v3
#define	msoftfVersion4Doc			0x00000004		// From app v4
#define	msoftfVersion5Doc			0x00000005		// From app v5
#define	msoftfVersion6Doc			0x00000006		// From app v6
#define	msoftfVersion7Doc			0x00000007		// From app v7
#define	msoftfVersion8Doc			0x00000008		// From app v8
#define	msoftfVersion9Doc			0x00000009		// From app v9
#define	msoftfVersion10Doc			0x0000000A		// From app v10
#define	msoftfVersion11Doc			0x0000000B		// From app v11
#define	msoftfFreezeDir				0x00000100		// fFreezeDir
#define	msoftfAddIn					0x00000200		// Add in type
#define msoftfHtml					0x00000400		// HTML type
#define msoftfSimulatesFreezeDir	0x00000800		// Tries to simulate freeze dir
													// by changing directory on type
													// change event

#define msoftfCurrentVersionDoc		msoftfVersion9Doc	// change for 10!!


// Shell Settings flags (for querying shell settings using MsoDwGetShellSetting

typedef ULONG MSOSSF;

#define	msosffNone					0x0
#define msosffShowExtensions		0x00000001		// show file extensions
#define	msosffShowHidden			0x00000002		// show hidden files
#define	msosffShowSystem			0x00000004		// show protected system files

// IMsoOLDocument string options for GetWzPcchGdn and SetWzGdn.

typedef int MSOIOLGDN;

enum
	{
	msoiolgdnTitle,							// Only used in window titles
	msoiolgdnNameOnly,						// Used in messages and menus
	msoiolgdnMRUName,							// Used in messages and menus
	msoiolgdnPersistent,						// Identifies a document
	msoiolgdnPersistDir,						// Dir for FAT and URLs.  (N/A w/ ODMA)
	msoiolgdnPersistLeafWithExt,			// May be used internally
	msoiolgdnPersistLeafWoExt,				// May be used internally
	msoiolgdnPersistExt,						// File extension.  Used internally

	msoiolgdnTempFilePath,					// Where the document is stored
	msoiolgdnTempFileDir,					// Where temp doc is stored
	msoiolgdnTempFileLeafWithExt,			// May be used internally
	msoiolgdnTempFileLeafWoExt,			// May be used internally
	msoiolgdnTempFileExt,					// File extension.  Used internally

	msoiolgdnAuthor,							// Author of this document
	msoiolgdnManager,							// Manager progerty for this document
	msoiolgdnCompany,							// Company progerty for this document
	msoiolgdnKeywords,						// Keywords progerty for this document
	msoiolgdnTempFileToCopy,				// Used with Refresh of Web documents
	msoiolgdnVBAFullName,					// Used with VBA
	msoiolgdnRedirectedURL,					// Set in download if URL is redirected

	msoiolgdnPersistVolume,
	msoiolgdnTempFileVolume,

	msoiolgdnCurrentUser,					// User currently editing this document
	msoiolgdnURLBookmark,
	msoiolgdnFolderSuffix,
	msoiolgdnTempFolderPath,    			// Temp folder used for web-case. Internal only.

	msoiolgdnPersistLeafWithExtNoDecode,
	msoiolgdnPersistLeafWoExtNoDecode,
	msoiolgdnPersistExtNoDecode,

	msoiolgdnODMADocId,						// Supported ODMA attributes:-

	msoiolgdnMax
	};

// IMsoOLDocument BeginCmd command specifiers.

typedef ULONG MSOIOLCMD;

#define msoiolcmdNone				0
#define msoiolcmdNew				0x00000001
#define msoiolcmdOpen				0x00000002
#define msoiolcmdOpenRO				0x00000004
#define msoiolcmdClose				0x00000008
#define msoiolcmdSaveAsDlg			0x00000010
#define msoiolcmdRefresh			0x00000020
#define msoiolcmdSave				0x00000040
#define msoiolcmdSaveCopy			0x00000080
#define msoiolcmdSaveVersion		0x00001000	// Not used in MSO97
#define msoiolcmdCheckIn			0x00002000	// Not used in MSO97
#define msoiolcmdCheckOut			0x00004000	// Not used in MSO97

#define msoiolcmdOpenMask			(msoiolcmdOpen | msoiolcmdOpenRO)
#define msoiolcmdMask				0x0000ffff	// All MSOIOLCMD values must be
															// in this range.

// IMsoOLDocument events.

typedef ULONG MSOIOLEVT;

#define msoiolevtNew					msoiolcmdNew
#define msoiolevtOpen				msoiolcmdOpen
#define msoiolevtOpenRO				msoiolcmdOpenRO
#define msoiolevtOpenShared		(msoiolcmdOpen | msoiolcmdOpenRO)
#define msoiolevtClose				msoiolcmdClose
#define msoiolevtSaveAsDlg			msoiolcmdSaveAsDlg
#define msoiolevtRefresh			msoiolcmdRefresh
#define msoiolevtSave				msoiolcmdSave
#define msoiolevtSaveCopy			msoiolcmdSaveCopy
#define msoiolevtSaveVersion		msoiolcmdSaveVersion
#define msoiolevtCheckIn			msoiolcmdCheckIn
#define msoiolevtCheckOut			msoiolcmdCheckOut

#define msoiolevtCopy				0x00010000	// Not sure if this is needed.
#define msoiolevtMove				0x00020000
#define msoiolevtRename				0x00040000
#define msoiolevtPrint				0x00080000
#define msoiolevtRoute				0x00100000
#define msoiolevtSend				0x00200000
#define msoiolevtPost				0x00400000

#define msoiolevtCmdCompleted		0x40000000	// Cmd from perv BeginCmd call

#define msoiolevtOpenMask			\
					(msoiolevtOpen | msoiolevtOpenRO | msoiolevtOpenShared)


// IMsoOLDocument event logging state.

#define MSO_S_IOLELS_NO_LOGGING	0x00
#define MSO_S_IOLELS_REN_LOGGING	0x01

typedef int MSOIOLELS;

#define msoiolelsNoLogging			MSO_S_IOLELS_NO_LOGGING
#define msoiolelsRenLogging		MSO_S_IOLELS_REN_LOGGING



// IMsoOLDocument type specifying integers which give information about a
// document.

typedef ULONG MSOIOLTDI;

enum
	{
	msoioltdiFirst					=	1,
	msoioltdiAttr					=	1,		// Document attributes
	msoioltdiCPagesPrinted,					// Count of pages printed during the
													// current editing session.  (This may
													// be negative if the current count of
													// pages printed is not known.)
	msoioltdiDcPagesPrinted,				// Change in the count of pages printed
													// during the current editing session.
													// (This may be negative.)
	msoioltdiModeIStorage,					// Open mode used with an IStorage
													// interface.
	msoioltdiOWSStgError,					// The document opened for RW on OWS gave
													// this storage error
	msoioltdiReDownload,						// Set this to force OLDoc to redownload the web document
													// By default this is set to 0;
													// setting this to 1 enables the redownload on the next call
													// to GetWzGdn(msoiolgdnTempFilePath).
													// Note: the redownload will only happen on the first call to
													// GetWzGdn.
													// Resetting this to 0 is a success condition that the caller
													// accepts the redownloaded file and office should discard the
													// old interface and file handle.
													// setting this to -1 indicates error and office should reinstate
													// the original state of OLDoc.
	msoioltdiRedirected,						// If this is set (usually after a call to GetFilePath (for urls)
													// or GetDisplayName (for InFileSys files)) then the persistent
													// name for this file has changed and needs to be updated in the
													// apps data structure by calling GetDisplayName(persist)
													// or something again.
	msoioltdiEnableRedirect,				// This flag determines whether or not redirect is allowed.
	msoioltdiSniffed,							// Set if we sniffed the file type.
	msoioltdiHtml,								// Set if redirect detects as HTML
	msoioltdiMHtml,							// Set if redirect detects as MHTML
	msoioltdiXml,						// Set if redirect detects as XML
	msoioltdiUseDefaultFolderSuffix,    // Set if app overrides existing folder
	msoioltdiCodepage,						// The codepage of the document
	msoioltdiOWSLowDateTime,				// dwLowDateTime for OWS Urls
	msoioltdiOWSHighDateTime,				// dwHighDateTime for OWS Urls
	msoioltdiURLSyntax,					// The URL syntax of the Persistent Name
	msoioltdiEnableCheckout,				// Enable checkout on open
	msoioltdiDelayedCancel,					// Set by OLDoc to abort current operation in a command which cannot
													// return MSO_S_IOLCMD_CANCEL result back to the app (due to functions
													// further up the stack dropping the result), the app should check for
													// this flag when it can safely abort the (open/close) operation.
	msoioltdiNoASPRedirection,
	msoioltdiLim
	};


// IMsoOLDocument type for adding additional file and server operations.
typedef ULONG MSOIOLFOPTYPE;
enum
	{
	msoiolfopFirst					= 1,
	msoiolfopRename				= 1,		// Rename the file - talk to brianwen if you really need this.
	msoiolfopDelete,							// Delete the file
	msoiolfopSave,								// Save the file
	msoiolfopCreateDir,						// Create a directory on the server if needed
	msoiolfopClearFopList,					// Empty the fop list for the document.
	msoiolfopLim
	};

typedef struct tagMSOIOLFOP
{
	MSOIOLFOPTYPE 		fop;					// What you want to do
	WCHAR *				pwzFile;				// The filename to act on
													//		For fopSave, this is the full local pathname or relative
													//			to the current directory.
													//		For fopDelete & fopRename, relative to the
													//			doc location.
	WCHAR *				pwzNewFile;			// The new file name
													//		For fopRename, this is the new file name, relative to the
													//			doc location.
													//		For fopSave, this is the name of the file on the server
													//			relative to the doc location.  If pwzFile is relative
													//			then you may set pwzNewFile = pwzFile.
} MSOIOLFOP, *LPMSOIOLFOP;


// IMsoOLDocument attributes.

typedef ULONG MSOIOLATTR;

#define msoiolattrRenLogging				0x00000001
#define msoiolattrNewDocument				0x00000002
#define msoiolattrInFileSys					0x00000004
#define msoiolattrWebDocument				0x00000008
#define msoiolattrODMADocument				0x00000010
#define msoiolattrOpen						0x00000020
#define msoiolattrReadOnly					0x00000040
#define msoiolattrShared					0x00000080
#define msoiolattrNewlyCreated				0x00000100
#define msoiolattrCanChange					0x00000200
#define msoiolattrOldVersion				0x00000400
#define msoiolattrSaveVersion				0x00000800
#define msoiolattrCheckedOut				0x00001000
#define msoiolattrCanCheckOut				0x00002000
#define msoiolattrCanToggleRO				0x00004000
#define msoiolattrUseCritSection			0x00008000
#define msoiolattrDeleteTemp				0x00010000
#define msoiolattrShowUI					0x00020000
#define msoiolattrCmdInProgress				0x00040000
#define msoiolattrReDownload				0x00080000
#define msoiolattrAddToRecent				0x00100000
#define msoiolattrNotPublishable			0x00200000
#define msoiolattrAsyncStream				0x00400000
#define msoiolattrDoNotUploadPrimaryFile  	0x00800000
#define msoiolattrSamePersTempName			0x01000000
#define msoiolattrGroupPrimaryFile          0x02000000
#define msoiolattrOLEEmbedded				0x04000000
#define msoiolattrForcePubMon				0x08000000
// hotfix 641/Office9 254817, bypass rosebud in the save as operations... 
#define msoiolattrBypassRosebud				0x10000000
#define msoiolattrNoRosebudPrompt			0x20000000

#define msoiolattrDocStoreMask	(msoiolattrNewDocument \
												| msoiolattrInFileSys \
												| msoiolattrWebDocument \
												| msoiolattrODMADocument)

#define msoiolattrOpenModeMask	(msoiolattrOpen \
												| msoiolattrReadOnly \
												| msoiolattrShared)


// IMsoOLDocument (globals) options.

typedef ULONG MSOIOLOPT;

#define msoioloptRenLogging		0x00000001
#define msoioloptOnlyInFileSys	0x00000002
#define msoioloptAllowURLs			0x00000004
#define msoioloptAllowODMA			0x00000008
#define msoioloptShortTempPaths	0x00000010
#define msoioloptANSITempPaths	0x00000020
#define msoioloptShortDispNames	0x00000040
#define msoioloptANSIDispNames	0x00000080
#define msoioloptANSISaveChars	0x00000100
#define msoioloptNewIsInFileSys	0x00000200

#define msoioloptDefault			( msoioloptRenLogging \
												| msoioloptAllowURLs \
												| msoioloptAllowODMA \
												| msoioloptNewIsInFileSys )


// IMsoOLDocument GUIDs.

typedef ULONG MSOIOLTOG;

#define msoioltogRollbackLimit	0x00000001


// IMsoOLDocument Office Librarian Operations.

typedef ULONG MSOIOLOP;

#define msoiolopVersionHist		0x00000001	// The Version History dialog.


// Maximum number of displayable findfile dialogs.

enum { msodmfindfileMax = 10 };

// Event handlers from the apps.

interface IMsoFindFile;			// Forward declare.

interface IMsoOLDocument;			// Forward declare.

interface IMsoDocumentNotifyList;		// Forward declare

interface IMsoPKMClient;	// Forward declare.

typedef int MSOINOTIFYVAL;

typedef HRESULT (OFFOPEN_CALLBACK *PFNDOCNOTIFYCALLBACK) (const WCHAR *);

typedef HRESULT (OFFOPEN_CALLBACK *PFNDODOCRONOTIFYDLG) (const WCHAR *, const WCHAR *, MSOINOTIFYVAL *);

typedef HRESULT (OFFOPEN_CALLBACK *PFNDODOCRWNOTIFYDLG) (const WCHAR *, const WCHAR *);

typedef HRESULT (OFFOPEN_CALLBACK *PFNOPENEVENT) (MSOIOOP,
		interface IMsoFindFile *, int, int, int, int *);

typedef BOOL (OFFOPEN_CALLBACK *PFNDMFSCHANGE) (MSODMFSCHANGE, WCHAR *, WCHAR *);

typedef BOOL (OFFOPEN_CALLBACK *PFNDMFLDRICONHDLR) (WCHAR *, BOOL, HIMAGELIST, int *);

typedef HRESULT (OFFOPEN_CALLBACK *PFNCHECKDOCAVAILABLERW) (const WCHAR *);

typedef HRESULT (OFFOPEN_CALLBACK *PFNCOMPAREDOCTIMESTAMP) (const WCHAR *, FILETIME, MSOINOTIFYVAL *);

typedef BOOL (OFFOPEN_CALLBACK *PFNTRANSLATECODEPAGE) (LPSTR, DWORD, DWORD, BOOL);

typedef BOOL (OFFOPEN_CALLBACK *PFNAPPREADWRITEPROP) (WCHAR *, WCHAR *, void *, BOOL, BOOL);

typedef BOOL (OFFOPEN_CALLBACK *PFNFDAACTIONFUNCTION) (const WCHAR *, int);

// IMsoSearch SetSearchPathEx flags.

typedef DWORD MSOSPF;

#define msospfNone					0
#define msospfDontClearFirst		0x00000001		// Don't clear the query first

// instrumented version only
// This is an api we export to client apps so they can give us their
// instrumentation proc

// type for the instrumentation proc
typedef BOOL_SDM (CALLBACK *PFNINSTRUMENTED)(DLM dlm, TMC tmc, UCBK_SDM wNew,
		UCBK_SDM wOld, UCBK_SDM wParam);

#ifdef INSTRUMENTED
MSOAPI_(void) RegisterPfnInstrumented(PFNINSTRUMENTED pfnInstrumented);
#endif


// defines for false tmc's used by instrumented version
// in order to treat certain types of information as
// data from a control
#ifdef INSTRUMENTED
#define tmcInstrumentedFakeStart (TMC)(0x300)
#define tmcFakeToolbarStart (TMC)(tmcInstrumentedFakeStart + 0x10)
#define tmcFakeQueryInfStart (TMC)(tmcInstrumentedFakeStart + 0x80)

// fake tmc's for the toolbar buttons
#define tmcFakeTBtnFileInfo (TMC)(tmcFakeToolbarStart + 2)
#define tmcFakeTBtnSummary (TMC)(tmcFakeToolbarStart + 3)
#define tmcFakeTBtnPreview (TMC)(tmcFakeToolbarStart + 4)
#define tmcFakeTBtnCommands (TMC)(tmcFakeToolbarStart + 5)
#define tmcFakeTBtnFind (TMC)(tmcFakeToolbarStart + 6)
#define tmcFakeTBtnParentFolder (TMC)(tmcFakeToolbarStart + 7)
#define tmcFakeTBtnNewFolder (TMC)(tmcFakeToolbarStart + 8)
#define tmcFakeTBtnQueryMenu (TMC)(tmcFakeToolbarStart + 10)
#define tmcFakeTBtnCmdMenu (TMC)(tmcFakeToolbarStart + 11)
#define tmcFakeTBtnGotoFav (TMC)(tmcFakeToolbarStart + 12)
#define tmcFakeTBtnFavMenu (TMC)(tmcFakeToolbarStart + 13)

// fake tmc's for various bits of query information
#define tmcFakeQueryMultiDirs (TMC)(tmcFakeQueryInfStart+0)
#define tmcFakeQueryTextOrPropUsed (TMC)(tmcFakeQueryInfStart + 1)
#define tmcFakeQueryMultiWords (TMC)(tmcFakeQueryInfStart+2)
#define tmcFakeQueryLastModUsed (TMC)(tmcFakeQueryInfStart +3)
#define tmcFakeQueryFilenameUsed (TMC)(tmcFakeQueryInfStart + 4)
#define tmcFakeQueryAdvancedUsed (TMC)(tmcFakeQueryInfStart + 5)
#define tmcFakeQueryIndexUsed (TMC)(tmcFakeQueryInfStart + 6)
#define tmcFakeQueryMatchAllWords (TMC)(tmcFakeQueryInfStart+7)
#define tmcFakeQueryMatchCase (TMC)(tmcFakeQueryInfStart+8)
#define tmcFakeQuerySearchSub (TMC)(tmcFakeQueryInfStart+9)
#define tmcFakeQueryGroupFolder (TMC)(tmcFakeQueryInfStart+10)
#define tmcFakeQuerySortSet (TMC)(tmcFakeQueryInfStart + 11)
#define tmcFakeQueryOnlyFoldChanged (TMC)(tmcFakeQueryInfStart + 12)
#define tmcFakeQueryActiveView0 (TMC)(tmcFakeQueryInfStart + 13)
#define tmcFakeQueryIndexerActive (TMC)(tmcFakeQueryInfStart + 14)
#define tmcFakeQueryDismissAction (TMC)(tmcFakeQueryInfStart + 15)
#define tmcFakeQueryInaccurateResults (TMC)(tmcFakeQueryInfStart + 16)
#define tmcFakeQueryClickOpen (TMC)(tmcFakeQueryInfStart + 17)
#define tmcFakeQueryNonClickOpen (TMC)(tmcFakeQueryInfStart + 18)
#define tmcFakeQueryActiveView1 (TMC)(tmcFakeQueryInfStart + 23)

// miscellaneous fake tmc's
#define tmcFakeFolderDblClick (TMC)(tmcFakeQueryInfStart + 19)
#define tmcFakeDialogDismiss (TMC)(tmcFakeQueryInfStart + 20)
#define tmcFakeEnteredDirName (TMC)(tmcFakeQueryInfStart + 21)
#define tmcFakeStopButton (TMC)(tmcFakeQueryInfStart + 22)

// fake fci's (dialog identifiers) for instrumentation
#define fciFakeDlgFind (0xC032)
#define fciFakeDlgOpen (0xC038)

// fake dialog message to tell word that our query is finished
// and that word should log the query flags
#define dlmLogQueryInfo (dlmUserMin + 2)

#endif //defined(INSTRUMENTED)


///////////////////////////////////////////////////////////////////////////////
// Places constants
// 
#define MSODMFFPLACES_HISTORY		0x00000001
#define MSODMFFPLACES_MYDOCUMENTS   0x00000002
#define MSODMFFPLACES_DESKTOP       0x00000004
#define MSODMFFPLACES_FAVORITES		0x00000008
#define MSODMFFPLACES_WEBFOLDERS	0x00000010
#define MSODMFFPLACES_ALL			(MSODMFFPLACES_HISTORY | MSODMFFPLACES_MYDOCUMENTS | MSODMFFPLACES_DESKTOP | MSODMFFPLACES_FAVORITES | MSODMFFPLACES_WEBFOLDERS)


/////////////////////////////////////////////////////////////////////////////
// Places Icons
//
typedef int MSODMFFPI;

enum
	{
	msodmffpiFirst, msodmffpiMin = msodmffpiFirst, msodmffpiMinLessOne = msodmffpiMin - 1,

	msodmffpiActiveDesktop,
	msodmffpiUser,
	msodmffpiFavorites,
	msodmffpiDesktop,
	msodmffpiPublishing,
	msodmffpiAdmin,
	msodmffpiRecent,

	msodmffpiMax, msodmffpiLast = msodmffpiMax - 1
	};


/////////////////////////////////////////////////////////////////////////////
// Dynamic Places Structure
//
typedef struct tagMSODMFFDP
{
	MSODMFFPI pi;                 // Which icon to show
	const WCHAR *wzDisplayName;   // The display name of the place
	const WCHAR *wzPath;          // The physical location the place points to
} MSODMFFDP, *PMSODMFFDP;


///////////////////////////////////////////////////////////////////////////////
// IMsoFindFile
//
// IFindFile represents an instance of Find File.

interface IMsoSearch;				// Forward declare
interface IMsoFileTypeList;		// Forward declare
interface IMsoControlList;			// Forward declare
interface IMsoCommandList;			// Forward declare
interface IMsoAppPreview;			// Forward declare
interface IMsoCodePageList;        // Forward declare

#undef INTERFACE
#define INTERFACE IMsoFindFile

DECLARE_INTERFACE_(IMsoFindFile, IUnknown)
{
	// ------------------------------------------------------------------------
	// Begin Interface
	//
	// This is important on the Mac.
	BEGIN_MSOINTERFACE

	// ------------------------------------------------------------------------
	// IUnknown Methods

	MSOMETHOD(QueryInterface) (THIS_ REFIID refiid, void * * ppvObject) PURE;

	MSOMETHOD_(ULONG, AddRef) (THIS) PURE;

	MSOMETHOD_(ULONG, Release) (THIS) PURE;

	// ------------------------------------------------------------------------
	// Standard Office Debug method
	MSODEBUGMETHOD

	// ------------------------------------------------------------------------
	// Base dialog configuration methods.

	// FSaveAs() - Returns fTrue if this is a Save As dialog.

	MSOMETHOD_(MSOBOOL, FSaveAs) (THIS) CONST_METHOD_FF PURE;

	// SetFSaveAs() - Changes the dialog from an Open to a Save As dialog
	// or back.  Can be set only when the dialog is not visible.

	MSOMETHOD(SetFSaveAs) (THIS_ MSOBOOL fSaveAs) PURE;

	// FSearchEnabled() - Returns fTrue if searching is enabled in the dialog.

	MSOMETHOD_(MSOBOOL, FSearchEnabled) (THIS) CONST_METHOD_FF PURE;

	// EnableSearch() - Enables or disables the search.  Can be set only when
	//					the dialog is not visible.

	MSOMETHOD(EnableSearch) (THIS_ MSOBOOL fSearchEnabled) PURE;

	// FPreviewEnabled() - Returns fTrue if previews are enabled in the
	//					   dialog.

	MSOMETHOD_(MSOBOOL, FPreviewEnabled) (THIS) CONST_METHOD_FF PURE;

	// EnablePreview() - Enables preview in the dialog.  Can be set only when
	//					 the dialog is not visible.

	MSOMETHOD(EnablePreview) (THIS_ MSOBOOL fEnablePreview,
							  interface IMsoAppPreview *pvInterface) PURE;

	// FDirPicker() - Returns fTrue if the dialog is a directory picker dialog,
	//				  fFalse otherwise, default is fFalse.

	MSOMETHOD_(MSOBOOL, FDirPicker) (THIS) CONST_METHOD_FF PURE;

	// SetFDirPicker() - Sets the dialog to be a directory picker dialog.
	//					 Can only be set when the dialog is not visible.

	MSOMETHOD(SetFDirPicker) (THIS_ MSOBOOL fDirPicker) PURE;

	// FMultiSelect() - Returns fTrue if the dialog is a multi-select dialog.

	MSOMETHOD_(MSOBOOL, FMultiSelect) (THIS) CONST_METHOD_FF PURE;

	// SetFMultiSelect() - Makes the dialog multi-select or single-select.
	//					   Can be set only when the dialog is not visible.

	MSOMETHOD(SetFMultiSelect) (THIS_ MSOBOOL fMultiSelect) PURE;

	// FChangeDir() - Specifies whether find file will change the current
	// directory to the directory set by the user.

	MSOMETHOD_(MSOBOOL, FChangeDir) (THIS) CONST_METHOD_FF PURE;

	// SetFChangeDir() - If set to fTrue, Find File will change the current
	// directory to the search directory, if there is only one search
	// directory.  Otherwise, Find File will not change the directory at
	// all.

	MSOMETHOD(SetFChangeDir) (THIS_ MSOBOOL fChangeDir) PURE;

	// GetFreezeDir() - Copies the freeze directory into the buffer argument.
	// The directory name will be truncated and null terminated if it cannot
	// fit inside the buffer as is.

	MSOMETHOD(GetFreezeDir) (THIS_ WCHAR * szDir, int cb) CONST_METHOD_FF PURE;

	// SetFreezeDir() - Sets the freeze directory to the contents of the
	// buffer.  The freeze directory can only be set when the dialog is
	// not shown and if at least one of the types in the dialog's list of
	// types has the "frozen directory" flag set.

	MSOMETHOD(SetFreezeDir) (THIS_ const WCHAR * szDir) PURE;

	// FConfirmReplace() - Specifies whether Find File (Save As dialog only)
	// will put up an alert when an existing filename is specified in the file
	// name control.

	MSOMETHOD_(MSOBOOL, FConfirmReplace) (THIS) CONST_METHOD_FF PURE;

	// SetFConfirmReplace() - If set to fTrue, Find File will alert the user
	// when an existing file name is specified in the Save As dialog.
	// Deprecated API - see SetConfirmReplaceEx, which provides more
	// options.

	MSOMETHOD(SetFConfirmReplace) (THIS_ MSOBOOL fConfirmReplace) PURE;

	// GetDefaultFileName() - Copies the default file name into the buffe
	// argument.  The name will be truncated and null terminated if it cannot
	// fit inside the buffer as is.

	MSOMETHOD(GetDefaultFileName) (THIS_ WCHAR * szFile, int cb) CONST_METHOD_FF PURE;

	// SetDefaultFileName() - Sets the default file name.  This can only be
	// set if the dialog is not yet visible.

	MSOMETHOD(SetDefaultFileName) (THIS_ const WCHAR * szFile) PURE;

	// FKeepSelectedType() - Returns the flag indicating whether or not the
	// type list blanks on an unrecognized type.

	MSOMETHOD_(MSOBOOL, FKeepSelectedType) (THIS) CONST_METHOD_FF PURE;

	// SetFKeepSelectedType() - Sets the flag indicating whether or not the
	// type list blanks on an unrecognized type.  This flag defaults to FALSE.

	MSOMETHOD(SetFKeepSelectedType) (THIS_ MSOBOOL fKeepSelectedType) PURE;

	// FSysDirPicker() - Returns the flag indicating whether or not the
	// system directory browser should be used instead of our own SDM dir picker.

	MSOMETHOD_(MSOBOOL, FSysDirPicker) (THIS) CONST_METHOD_FF PURE;

	// SetFSysDirPicker() - Sets the flag indicating whether or not the
	// system directory browser should be used instead of our own SDM dir picker.

	MSOMETHOD(SetFSysDirPicker) (THIS_ MSOBOOL fSysDirPicker) PURE;

	// -----------------------------------------------------------------------
	// Cosmetic dialog customization.

	// GetAppName() - Returns the current name for the application.  This
	//				  name is used in message box titles.  The given buffer
	//				  must be at least of length cbMaxDlgTitle.  The name is
	//				  zero-terminated.

	MSOMETHOD_(void, GetAppName) (THIS_ WCHAR * szAppName) CONST_METHOD_FF PURE;

	// SetAppName() - Sets the current name for the application.  This name is
	//				  used in message box titles.  The name must be zero-
	//				  terminated in a buffer no larger than cbMaxDlgTitle.
	//				  The title can be set only when the dialog is not visible.

	MSOMETHOD(SetAppName) (THIS_ const WCHAR * szAppName) PURE;

	// GetDlgTitle() - Returns the dialog title in the given buffer, which
	//				   must be at least of length cbMaxDlgTitle.  The title
	//				   is null-terminated.

	MSOMETHOD_(void, GetDlgTitle) (THIS_ WCHAR * szTitle) CONST_METHOD_FF PURE;

	// SetDlgTitle() - Sets the dialog title with the given title.  The title
	// must be zero-terminated in a buffer no larger than cbMaxDlgTitle.  The
	// title can be set only when the dialog is not visible.

	MSOMETHOD(SetDlgTitle) (THIS_ const WCHAR *szTitle) PURE;

//	MSOMETHOD(SetDlgTitle) (THIS_ const char *szTitle) PURE;

	// GetXY() - Returns the the current position of the dialog (whether it's
	// 			 visible or not).  In pixels.

	MSOMETHOD_(void, GetXY) (THIS_ int *px, int *py) CONST_METHOD_FF PURE;

	// SetXY() - Sets the position of the dialog when it is displayed.  Can
	//			 be set only when the dialog is not visible.  In pixels.

	MSOMETHOD(SetXY) (THIS_ int x, int y) PURE;

	// GetCenterXY() - Returns the the current position of the dialog's center
	//				   (whether it's visible or not).  In pixels.

	MSOMETHOD_(void, GetCenterXY) (THIS_ int *px, int *py) CONST_METHOD_FF PURE;

	// SetCenterXY() - Sets the center position of the dialog when it is
	//			 	   displayed.  Can be set only when the dialog is not
	// 				   visible.  In pixels.

	MSOMETHOD(SetCenterXY) (THIS_ int x, int y) PURE;


	// ------------------------------------------------------------------------
	// Dialog display methods.

	// ShowDlg() - Shows the dialog.  Only one dialog can be shown per
	// process calling the .dll.

	MSOMETHOD(ShowDlg) (THIS_ int * picmd, HWND hwnd) PURE;

	// GetHwnd() - Returns the hwnd of the dialog client.  Only works if the
	// dialog is up.

	MSOMETHOD(GetHwnd) (THIS_ HWND * hwnd) CONST_METHOD_FF PURE;

	// ------------------------------------------------------------------------
	// Events.

	// RegisterPfnEvent() - Register an event handler pointed to by pfnEvent.
	// The event handler must be of the form

	// HRESULT OpenEventHandler(IOOP, IFindFile *, int, int, int, int *)

	MSOMETHOD(RegisterPfnEvent) (THIS_ PFNOPENEVENT pfnEvent, int wApp) PURE;


	// ------------------------------------------------------------------------
	// Short-cut methods.

	// GetOpenTitle() - Gets the current title for the "Open" command button.
	//				    The given buffer must be at least cbMaxItemTitle in
	//					length.  The title will be zero-terminated.

	MSOMETHOD_(void, GetOpenTitle) (THIS_ WCHAR * szTitle) CONST_METHOD_FF PURE;

	// SetOpenTitle() - Sets the current title for the "Open" command button.
	//					The title must be zero-terminated in a buffer no
	//					larger than cbMaxItemTitle.  The title can only be
	//					set when the dialog is not visible.

	MSOMETHOD(SetOpenTitle) (THIS_ const WCHAR * szTitle) PURE;

	// GetIcntrlValue() - Shortcuts to return the values of controls.
	//					   Return E_INVALIDARG if the wrong flavor was used.
	//					   Use "W" for checkboxes, radiogroups, and list boxes.
	//					   Returns S_TRUNCATED if the return string was
	//					   truncated, and S_OK otherwise.

	MSOMETHOD(GetIcntrlValueW) (THIS_ int icntrl, int * pval) CONST_METHOD_FF PURE;

	MSOMETHOD(GetIcntrlValueSz) (THIS_ int icntrl, WCHAR * szVal, int cbVal)
			CONST_METHOD_FF PURE;

	// SetIcntrlValue() - Shortcuts to set the value of controls.  Return
	//					   E_INVALIDARG if the wrong flavor was used.  Returns
	//					   Resource allocation failures, and S_OK otherwise.

	MSOMETHOD(SetIcntrlValueW) (THIS_ int icntrl, int val) PURE;

	MSOMETHOD(SetIcntrlValueSz) (THIS_ int icntrl, const WCHAR * szVal) PURE;

	// FEnabledIcntrl() - Returns fTrue if the given control is enabled, fFalse
	//					  otherwise.  The default is fTrue.

	MSOMETHOD_(MSOBOOL, FEnabledIcntrl) (THIS_ int icntrl) CONST_METHOD_FF PURE;

	// EnableIcntrl() - Enables/disables the given control.

	MSOMETHOD(EnableIcntrl) (THIS_ int icntrl, MSOBOOL fEnable) PURE;

	// SetFocusIcntrl() - Sets the focus to be on the given icntrl.

	MSOMETHOD(SetFocusIcntrl) (THIS_ int icntrl) PURE;

	// ------------------------------------------------------------------------
	// Query Definition Short-cut methods

	// SetSearchPath() - Sets the query for this find file to search for the
	// default file types (as set using IFileTypeList::SetIszDefault()), with
	// no other selection criteria.  If NULL, the current directory is used.
	// This method short-cuts using the IMsoSearch object for this find file
	// object.

	MSOMETHOD(SetSearchPath) (THIS_ const WCHAR * szPath) PURE;

	// ------------------------------------------------------------------------
	// Search for extension.

	// IszFileTypeGet() - Returns the currently selected file type.  Returns
	// iszNil if nothing is selected.

	MSOMETHOD_(int, IszFileTypeGet) (THIS) CONST_METHOD_FF PURE;

	// SetIszFileType() - Used to set the current file type.  Default upon
	// dialog startup is set in the file type list itself.

	MSOMETHOD(SetIszFileType) (THIS_ int iszFileType) PURE;

	// ------------------------------------------------------------------------
	// Query Definition and Search Results

	MSOMETHOD_(void, GetActiveIMsoSearch) (THIS_ interface IMsoSearch ** ppsrch)
			CONST_METHOD_FF PURE;


	// ------------------------------------------------------------------------
	// Collection methods

	// GetIMsoControlList() - Returns a pointer to the custom item list.
	// This is a pointer to the list in the object referenced by IMsoFindFile,
	// not a copy.  Should be released when through.

	MSOMETHOD_(void, GetIMsoFileTypeList) (THIS_
			interface IMsoFileTypeList ** ppfiltyplist) CONST_METHOD_FF PURE;

	MSOMETHOD_(void, GetIMsoControlList) (THIS_
			interface IMsoControlList ** ppcstitmlist) CONST_METHOD_FF PURE;

	MSOMETHOD_(void, GetIMsoCommandList) (THIS_
			interface IMsoCommandList ** ppcmdlist) CONST_METHOD_FF PURE;

	// UNDONE:  Move these two APIs to where they belong.  /Steve

	// FAdvSearchEnabled() - Returns fTrue if the advanced search dialog is
	// 						 enabled.  This returns the setting of this flag,
	//						 and the flag could be fTrue even though advanced
	//						 search is disabled if it is disabled by another
	//						 dialog setting (such as the fSearchEnabled flag).

	MSOMETHOD_(MSOBOOL, FAdvSearchEnabled) (THIS) CONST_METHOD_FF PURE;

	// EnableAdvSearch() - Enables or disables the advance search dialog.  Can
	//					   only be set when the dialog is not visible.

	MSOMETHOD(EnableAdvSearch) (THIS_ MSOBOOL fAdvSearchEnabled) PURE;

	// FFileDirPicker() - Returns fTrue if the dialog is a file/dir picker
	//					  dialog, fFalse otherwise.  fFalse is default.

	MSOMETHOD_(MSOBOOL, FFileDirPicker) (THIS) CONST_METHOD_FF PURE;

	// SetFFileDirPicker() - Sets the dialog to be a file/dir picker dialog.
	//						 Can only be set when the dialog is not visible.

	MSOMETHOD(SetFFileDirPicker) (THIS_ MSOBOOL fFileDirPicker) PURE;

	// SetView() - Sets the "view" shown in the dialog.  Not valid
	// to do this while the dialog is visible.

	MSOMETHOD(SetView) (THIS_ MSOVIEW ff) PURE;

	// GetView() - Gets the "view" shown in the dialog.

	MSOMETHOD(GetView) (THIS_ MSOVIEW * pvff) CONST_METHOD_FF PURE;

	// SetFShowGroups() - Sets the dialog groups hits by directory
	// or not.  Not valid to do this when the dialog is visible.

	MSOMETHOD(SetFShowGroups) (THIS_ MSOBOOL fShowGroups) PURE;

	// GetView() - Gets whether or not the dialog groups hits by
	// directory.

	MSOMETHOD_(MSOBOOL, FShowGroups) (THIS) CONST_METHOD_FF PURE;

	// SetSort() - Sets the field used to sort hits shown in the dialog.
	// Not valid to do this while the dialog is visible.

	MSOMETHOD(SetSort) (THIS_ MSOSORT sff) PURE;

	// GetView() - Gets the field the dialog hits are sorted on.

	MSOMETHOD(GetSort) (THIS_ MSOSORT * psff) CONST_METHOD_FF PURE;

	// ExecuteQuery() - Executes the active search without showing the
	// dialog.  This updates the results list.  It is not valid to do this
	// when a dialog is visible.

	MSOMETHOD(ExecuteQuery) (THIS) PURE;

/* FMidEast */

/* FMidEast End */

	// SetPfnTranslateCodepage() - Sets the pointer to a fucntion used to
	// translate code pages
	MSOMETHOD_(void, SetPfnTranslateCodepage) (THIS_
					PFNTRANSLATECODEPAGE pfnXlateCodepg) PURE;

	// PfnTranslateCodepageGet() - Gets the pointer to a fucntion used to
	// translate code pages.  Returns NULL if SetPfnTranslateCodepage has
	// not been called.
	MSOMETHOD_(PFNTRANSLATECODEPAGE, PfnTranslateCodepageGet) (THIS) PURE;

	// SetFPreserveQuotes() - Sets whether double quotes in a file name are
	// preserved in the name returned to the app.
	//
	MSOMETHOD(SetFPreserveQuotes) (THIS_ MSOBOOL fPreserveQuotes) PURE;

	// FPreserveQuotes() - Returns fTrue if set to preserve double quotes
	// around a non-standard file name.
	MSOMETHOD_(MSOBOOL, FPreserveQuotes) (THIS) CONST_METHOD_FF PURE;

	// DontUseODMAInNextDlg() - Set an internal flag which will be used to
	// make sure that the next call to ShowDlg uses our standard dialogs,
	// not the ODMA dialogs (which might normally be used if the user has
	// ODMA installed).
	MSOMETHOD_(void, DontUseODMAInNextDlg) (THIS) PURE;

	// FOnlyFileSys() - Returns fTrue if the dialog only allows items in the
	// file system to be selected (as opposed to eg. URLs).

	MSOMETHOD_(MSOBOOL, FOnlyFileSys) (THIS) CONST_METHOD_FF PURE;

	// SetFOnlyFileSys() - Sets whether the dialog allows only items in the
	// file system to be selected (as opposed to e.g. URLs).   The default
	// is that items may be selected from any supported store.

	MSOMETHOD(SetFOnlyFileSys) (THIS_ MSOBOOL fOnlyFileSys) PURE;

#if	VSMSODEBUG
	// SetFNoLoadIcon - For Powerpoint, to not do LoadIcon on NT which
	// causes an Access Violation and makes debugging powerpoint difficult.
	// Office '96 bug 40614

	MSOMETHOD(SetFNoLoadIcon) (THIS_ MSOBOOL fNoLoadIcon) PURE;
#endif // VSMSODEBUG

	// SetFResolveLinks() - Sets whether links should be resolved or not.
	// The default is to resolve them.

	MSOMETHOD(SetFResolveLinks) (THIS_ MSOBOOL fResolveLinks) PURE;

	// SetHmsoinst() - Sets the HMSOINST for this IMsoFindFile
	// instance.  If not called, the last HMSOINST that called
 	// FInitOffice prior to MakeFindFile being called for this
	// IMsoFindFile instance is used.

	MSOMETHOD(SetHmsoinst) (THIS_ HMSOINST hmsoinst) PURE;

	// GetQueryErrorMessage() - If ExecuteQuery returns an error,
	// this routine may be used to get a more detailed error message.
 	// Useful primarily for automation and Web Find.  The buffer pointed
 	// by wzMsg should be at least 255 characters long.  If no error
 	// occurred, or there is no more detailed error message available,
 	// the buffer will be empty on return.

	MSOMETHOD(GetQueryErrorMessage) (THIS_ WCHAR *wzMsg) CONST_METHOD_FF PURE;

	// EnbalePreviewButton - Sets a flag to enable/disable the preview
	// button on the toolbar.
	MSOMETHOD_(void, EnablePreviewButton) (THIS_ MSOBOOL f) PURE;

#ifdef SHILSHOLE_ENABLED
	// GetTotalHitsAvailable() - For Web Search, we provide a mechanism
	// to limit the total hits returned for performance reasons.  To
	// allow displaying the number of hits available, the ISAPI module
	// can call this routine.  Note that this will work only with Web
	// Find.  It does not return the number of items on the Found File
	// list, but the actual count of hits retrieved by the retrieve
	// enumerator.

	MSOMETHOD(GetTotalHitsAvailable) (THIS_ unsigned *pchitTotal) CONST_METHOD_FF PURE;
#endif // SHILSHOLE_ENABLED

	// SetConfirmReplaceEx() - A more functional version of SetFConfirmReplace.
	// Allows app to take responsibility only for overwrite checking in
	// file system (added mainly for word).

	MSOMETHOD(SetConfirmReplaceEx) (THIS_ MSODMCR msocr) PURE;

/* FMidEast */

/* FMidEast End */

	// SetIszFileTypePolicyDefault() - For "Default Save" feature, set
	// the index in the file type list of the type recommended by
	// administrative policy.  If not set, the default save feature is
	// not enabled.

	MSOMETHOD(SetIszFileTypePolicyDefault) (THIS_ int iszFileType) PURE;

	// GetIszFileTypePolicyDefault() - For "Default Save" feature, get
	// the index in the file type list of the type recommended by
	// administrative policy.  If not set, the default save feature is
	// not enabled.

	MSOMETHOD_(int, GetIszFileTypePolicyDefault) (THIS) CONST_METHOD_FF PURE;

	// SetSearchPathPidl() - Sets the query for this find file to search for the
	// default file types (as set using IFileTypeList::SetIszDefault()), with
	// no other selection criteria.  If NULL, the current directory is used.
	// This method short-cuts using the IMsoSearch object for this find file
	// object. The ppidl is an array of pidl (so multpile pidl folders can be
	// passed). Each pidl passed should be relative to desktop folder (or in other
	// words, absolute pidl). cpidl is the number of pidls passed.
	// NOTE: ppidl is taken as a void* parameter. However, it should be LPITEMIDLIST*
	// The reason for it being void here is that LPITEMIDLIST's definition is in
	// shlobj.h and excel fails to compile if you include it in this file.

	MSOMETHOD(SetSearchPathPidl) (THIS_ const void *ppidl, int cpidl) PURE;

	// EnableOpenInNativeApp - enables opening of the selected files in their respective
	// apps. This method is valid only for Open dialogs. The flag will be neglected for
	// other variations of the dialog.
	// Default: Flag is false.
	MSOMETHOD(EnableOpenInNativeApp) (THIS_ MSOBOOL fEnable, MSOBOOL fOpensUnknown,
									  MSODMAPPID appidCurrentApp) PURE;

	// EnableSaveAsWebOptions - enables switching layout for SaveAsWebPage
	MSOMETHOD(EnableSaveAsWebOptions) (THIS_ int iHtmlTypeFamily,
									  MSODMLAYOUT layoutStyle) PURE;

	// DisableSaveAsWebOptions - disables switching layout for SaveAsWebPage
	MSOMETHOD(DisableSaveAsWebOptions) (THIS) PURE;

	// Returns index into type list of the html type set by EnableSaveAsWebOptions
	// !!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!
	// This method is obsolete and is kept here to maintain backward compatibility with mso9
	MSOMETHOD(GetIHtmlTypeFamily) (THIS_ int *iHtmlTypeFamily) PURE;

	// Returns layout style set by EnableSaveAsWebOptions
	// !!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!
	// This method is obsolete and is kept here to maintain backward compatibility with mso9
	MSOMETHOD(GetLayoutStyle) (THIS_ MSODMLAYOUT  *layoutStyle) PURE;

	// SetShortFileName() - Specifies whether Find File (Save As dialog only)
	// will force 8.3 file names.

	MSOMETHOD_(void, SetShortFileName) (THIS_ MSOBOOL fSet) PURE;

	// Sets the tcid for the two group picture
	MSOMETHOD(TcidTwoGroupPictSet) (THIS_ int tcid) PURE;

	// Gets the tcid for the two group picture
	MSOMETHOD(TcidTwoGroupPictGet) (THIS_ int *ptcid) PURE;

	// Sets the tcid for the open button picture
	MSOMETHOD(SetOpenBtnTcid) (THIS_ int tcid) PURE;

	// Gets the tcid for the open button picture
	MSOMETHOD(GetOpenBtnTcid) (THIS_ int *ptcid) PURE;

	// ------------------------------------------------------------------------
	// File system change events that can be handled by the app

	// RegisterPfnFSChangeHandler() - Register a change handler pointed to by pfnFSChange.
	// The handler must be of the form

	// BOOL FFileSystemChangeHandler(MSODMFSCHANGE fsChangeType, WCHAR * wzOld, WCHAR *wzNew);

	MSOMETHOD(RegisterPfnFSChangeHandler) (THIS_ PFNDMFSCHANGE pfnFSChange) PURE;

	// ------------------------------------------------------------------------
	// RegisterPfnFolderIconHandler() - Register a callback that lets the app
	// override the icons for folders.
	// The handler must be of the form

	// BOOL FFolderIconHandler(WCHAR *wzPath, BOOL fOpenFolder, HIMAGELIST himl, int *piiml);

	MSOMETHOD(RegisterPfnFolderIconHandler) (THIS_ PFNDMFLDRICONHDLR pfnFolderIconHandler) PURE;

	// FIgnoreVtiDirs()

	MSOMETHOD_(MSOBOOL, FIgnoreVtiDirs) (THIS) CONST_METHOD_FF PURE;

	// SetFIgnoreVtiDirs() - Makes the dialog not display _VTI_* dirs
	//					   Can be set only when the dialog is not visible.

	MSOMETHOD(SetFIgnoreVtiDirs) (THIS_ MSOBOOL fIgnoreVtiDirs) PURE;

	// SetGrfPlaces() - Sets which standard places will appear. By default, all standard places will be displayed. Calling this
	// will override that behavior.
	//(MSODMFFPLACES_HISTORY | MSODMFFPLACES_MYDOCUMENTS | MSODMFFPLACES_DESKTOP | MSODMFFPLACES_FAVORITES | MSODMFFPLACES_WEBFOLDERS)
	MSOMETHOD(SetGrfPlaces)(THIS_ ULONG grfPlaces) PURE;

	// GrfPlaces() - Returns which standard places will appear. By default, will be MSODMFFPLACES_ALL.
	//(MSODMFFPLACES_HISTORY | MSODMFFPLACES_MYDOCUMENTS | MSODMFFPLACES_DESKTOP | MSODMFFPLACES_FAVORITES | MSODMFFPLACES_WEBFOLDERS)
	MSOMETHOD_(DWORD, GrfPlaces)(THIS) CONST_METHOD_FF PURE;
	
	// SetFShowPlacesBar - Sets whether the Places Bar will appear or not. By default, set to TRUE.
	MSOMETHOD(SetFShowPlacesBar)(THIS_ MSOBOOL fShow) PURE;

	// FShowPlacesBar - Returns whether the Places Bar will appear or not. By default, returns TRUE.
	MSOMETHOD_(MSOBOOL,FShowPlacesBar)(THIS) CONST_METHOD_FF PURE;

	// ------------------------------------------------------------------------
	// Dialog display methods.

	// GetHwndParent() - Returns the hwnd of the dialog client's parent.
	MSOMETHOD(GetHwndParent)(THIS_ HWND * hwnd) CONST_METHOD_FF PURE;

	// SetHwndParent() - Sets the dialog client's parent.
	MSOMETHOD(SetHwndParent)(THIS_ HWND hwnd) PURE;
	
	// GetWzFilenameTitle() - Sets the "File Name:" static text dialog field.
	MSOMETHOD_(void, GetWzFilenameTitle) (THIS_ WCHAR * wzFilenameTitle) CONST_METHOD_FF PURE;

	// SetWzFilenameTitle() - Sets the "File Name:" static text dialog field.
	MSOMETHOD(SetWzFilenameTitle)(THIS_ WCHAR *wzFolderTitle) PURE;

	// HrSetDynamicPlaces - Set the array of dynamic places.  The array
	//                      should be terminated with a NULL pointer.  Can
	//                      only be set when the dialog is not visible.
	MSOMETHOD(HrSetDynamicPlaces) (THIS_ MSODMFFDP **rgpdp) PURE;

	// DisableFileTypeDropdown - disable the dropdown control for the file type list
	MSOMETHOD_(void, DisableFileTypeDropdown) (THIS) PURE;
	// SetOpenAsNew - open the selected file into its native app but as a new doc
	MSOMETHOD_(void, SetOpenAsNew) (THIS) PURE;
	// SetFileNewWebTemplates - when one is in the Templates for my projects dialog
	MSOMETHOD_(void, SetFileNewWebTemplates) (THIS) PURE;

	//SetFFilenameFiltering - Don't use this. This is for Word.
	MSOMETHOD_(void, SetFFilenameFiltering)(THIS_ MSOBOOL fFiltering) PURE;

	//SetFInitialFiltering - Don't use this. This is for Word.
	MSOMETHOD_(MSOBOOL, FFilenameFiltering)(THIS) PURE;

	// ActivateDefaultSaveURLForTemplates - use Web Forms to get the default save
	//                                      for the URL but only when the file type 
	//                                      selected is the same as 'iwz'
	MSOMETHOD_(void, ActivateDefaultSaveURLForTemplates) (THIS_ int iwz) PURE;

	// SetLimitScopeInWebFolders - only show Web Folders, plus only http folders
	// that are non-WebDrives and enumerable
	MSOMETHOD_(void, SetLimitScopeInWebFolders) (THIS) PURE;

	MSOMETHOD_(void, SetPfnAppReadWriteProp) (THIS_
				PFNAPPREADWRITEPROP pfnAppReadWriteProp, DWORD dwPrvData) PURE;

	// PfnAppReadWriteProp() - Gets the pointer to a function used to
	// read\write app doc property.  Returns NULL if SetPfnAppReadWriteProp has
	// not been called.
	MSOMETHOD_(PFNAPPREADWRITEPROP, PfnAppReadWriteProp) (THIS) PURE;

	MSOMETHOD_(DWORD, GetMetaDataPrvData) (THIS) PURE;

	// FEnableThumbnails() - Returns fTrue if thumbnail support is enabled.
	//					  fFalse is default.
	MSOMETHOD_(MSOBOOL, FEnableThumbnails) (THIS) PURE;

	// SetFEnableThumbnails() - Enables or disables thumbnail support.
	//						 Can only be set when the dialog is not visible.
	MSOMETHOD_(void, SetFEnableThumbnails) (THIS_ MSOBOOL fEnable) PURE;

	// SetFormsURLForFileNew() - passes in the retrieved FormsURL of the
	// default path so File Open will show it in Web View properly
	MSOMETHOD_(void, SetFormsURLForFileNew)(THIS_ LPWSTR pwzURL) PURE;

	// FNoFormDialog() - Specifies whether Find File (Save As dialog only)
	// will try to bring up the form dialog
	MSOMETHOD_(MSOBOOL, FNoFormDialog) (THIS) CONST_METHOD_FF PURE;

	// SetFNoFormDialog() - If set to fTrue, Find File (Save As) will NOT
	// try to bring up the form dialog
	MSOMETHOD(SetFNoFormDialog) (THIS_ MSOBOOL fNoFormDialog) PURE;

   // SetFNoWow() - If set to fTrue, the Add Web Folder wizard item
   // will not be displayed.
	MSOMETHOD(SetFNoWOW) (THIS_ MSOBOOL fNoWOW) PURE;

   // FNoWow() - Specifies whether the Add Web Folder wizard item
   // should be displayed.
   MSOMETHOD_(MSOBOOL, FNoWOW) (THIS) CONST_METHOD_FF PURE;

	// SetHaveFileNewDefaultPath - default path is given for File New dialog
	MSOMETHOD_(void, SetHaveFileNewDefaultPath) (THIS) PURE;

	//O10 139844 - SetFEnableWebView - enable/disable webview in File Dialogs for 
	//OWS10 Office.Net sites and document libraries.
	MSOMETHOD(SetFEnableWebView) (THIS_ MSOBOOL fWebView) PURE;

	//FEnableWebView - is webview in File Dialogs for 
	//OWS10 Office.Net sites and document libraries enabled?
	MSOMETHOD_(MSOBOOL, FEnableWebView) (THIS) CONST_METHOD_FF PURE;

	//O10 221942 - SetFDefaultWebView - We default to webview in 
	//File Dialogs for OWS10 Office.Net sites and document libraries 
	//We won't default if fDefault is FALSE. 
	MSOMETHOD(SetFDefaultWebView) (THIS_ MSOBOOL fDefault) PURE;

	//FDefaultWebView - we don't default to webview for 
	//OWS10 Office.Net sites and document libraries if this returns FALSE
	MSOMETHOD_(MSOBOOL, FDefaultWebView) (THIS) CONST_METHOD_FF PURE;

	// FOtherAppFiles - file(s) launched into other app(s)
	MSOMETHOD_(MSOBOOL, FOtherAppFiles) (THIS_ interface IMsoString *pstr) PURE;

	//SetFWordMail
	MSOMETHOD(SetFWordMail) (THIS_ MSOBOOL fWordMail) PURE;

	//FWordMail
	MSOMETHOD_(MSOBOOL, FWordMail) (THIS) CONST_METHOD_FF PURE;

	//SetFEnableShellBrowser
	MSOMETHOD(SetFEnableShellBrowser) (THIS_ MSOBOOL fEnable) PURE;

	//FEnableShellBrowser
	MSOMETHOD_(MSOBOOL, FEnableShellBrowser) (THIS) CONST_METHOD_FF PURE;

	//SetFEnableSearchTheWeb
	MSOMETHOD(SetFEnableSearchTheWeb) (THIS_ MSOBOOL fEnable) PURE;

	//FEnableSearchTheWeb
	MSOMETHOD_(MSOBOOL, FEnableSearchTheWeb) (THIS) CONST_METHOD_FF PURE;

	//SetWVDefaultSaveLocationWz
	MSOMETHOD_(void, SetWVDefaultSaveLocationWz) (THIS_ LPCWSTR pwz) PURE;

	//WzGetWVDefaultSaveLocation
	MSOMETHOD_(LPCWSTR, WzGetWVDefaultSaveLocation) (THIS) PURE;

};


///////////////////////////////////////////////////////////////////////////////
// MsoMakeFindFile
//
// Returns a new IMsoFindFile instance.  Will return CO_E_DLLNOTFOUND if a
// required DLL is missing.

MSOAPI_(HRESULT) MsoMakeFindFile(IMsoFindFile ** ppfndfile);


///////////////////////////////////////////////////////////////////////////////
// MsoHrDoSimpleEditDlg
//

MSOAPI_(HRESULT) MsoHrDoSimpleEditDlg(HMSOINST pinst,
									  const WCHAR *wzDlgTitle,
									  const WCHAR *wzEditTitle,
									  WCHAR *wzEdit,
									  const WCHAR *wzDescrText);

///////////////////////////////////////////////////////////////////////////////
// MsoFRosebudMonikerProvider
//

MSOAPIX_(BOOL) MsoFRosebudMonikerProvider(const WCHAR *wzUrl, DWORD cp);

///////////////////////////////////////////////////////////////////////////////
// MsoDontMakeFindFile
//
// This is called by Ren only, just before FLoadFileOpen (or any future apps
// that won't need our dialog).  Don't create a default CFindFile object in
// FLoadFileOpen.
//
MSOAPI_(HRESULT) MsoDontMakeFindFile ();

#ifdef SHILSHOLE_ENABLED
///////////////////////////////////////////////////////////////////////////////
// MsoGetIndexInformation
//
// Return information about an index in the directory wzDir.  Any of the
// other parameters may be NULL if you don't care about that particular piece
// of information.  Returns S_OK if OK, E_INVALID if the directory is invalid
// or does not contain an index.  The buffers passed for dates should be at
// least 255 characters long to accomdate all localized forms of dates.
//
MSOAPI_(HRESULT) MsoGetIndexInformation(
	const WCHAR		*wzDir,
	WCHAR			*wzCreatedDate,			// formatted date strings
	WCHAR			*wzModifiedDate,
	WCHAR			*wzDocTypes,			// e.g. "Microsoft Office Documents"
	DWORD			*pdwNumDocs,			// Number of documents in the index
	BOOL			*pfRelevance,			// TRUE if index supports relevance ranking
	BOOL			*pfPropCache,			// TRUE if the index includes a property cache
	BOOL			*pfPhraseSearch,		// TRUE if the index supports phrase searches
	BOOL			*pfHttpMappings,		// TRUE if the index supports http returns.
	DWORD			*pdwMaxReturnedDocs);	// Maximum number of documents that will be
											// returned from a retrieval request
											// (0xFFFFFFFF -> no limit)
#endif // SHILSHOLE_ENABLED

///////////////////////////////////////////////////////////////////////////////
// MsoDontInitializeVisibles
//
// This is called by FindFast NT Service only, just before FLoadFileOpen
// It prevents FLoadFileOpen from initializing the message window, etc.
//
MSOAPI_(HRESULT) MsoDontInitializeVisibles();


///////////////////////////////////////////////////////////////////////////////
// MsoFLoadFileOpen
//
// Load the language-specific DLL for File Open and initialize the structures
// used by File Open.  This can be called at idle time, after Office is
// initialized.  MakeFindFile will call FLoadFileOpen if FLoadFileOpen is
// not called before MakeFindFile is called.
// The NoUI version of MsoFLoadFileOpen does non-ui initialization only.  If
// it is called, then MsoFLoadFileOpen with UI must also be called afterward.

MSOAPI_(MSOBOOL) MsoFLoadFileOpen(void);
MSOAPIX_(MSOBOOL) MsoFLoadFileOpenNoUI(void);


///////////////////////////////////////////////////////////////////////////////
// MsoSetOlDocLogHandle()
//
// This must be called before any IOLDocument events can be logged to Ren.
// The hOutlook handle should be obtained from MsoFInitLogging and released
// using MsoFUninitLogging.
MSOAPI_(void) MsoSetOlDocLogHandle(HANDLE hOutlook);

///////////////////////////////////////////////////////////////////////////////
// MsoSetElsGlobalLoggingState()
//
// This can be called to globally enable/disable Ren logging for IOLDocuments.
// This is set to msoiolelsRenLogging by default.
MSOAPI_(void) MsoSetElsGlobalLoggingState(MSOIOLELS elsLoggingState);

///////////////////////////////////////////////////////////////////////////////
// MsoGetElsGlobalLoggingState()
//
// This can be called to retrieve the global Ren logging state for IOLDocuments.
// This is set to msoiolelsRenLogging by default.
MSOAPIX_(MSOIOLELS) MsoGetElsGlobalLoggingState();


///////////////////////////////////////////////////////////////////////////////
// MsoHcursorFindFile
//
// Gets the current mouse cursor.  This should be used by client apps.
// when responding to WM_SETCURSOR messages while an Office dialog is up.

MSOAPI_(HCURSOR) MsoHcursorFindFile();


///////////////////////////////////////////////////////////////////////////////
// MsoSetFindFileFont
//
// Set the fonts for the current find file object.  This should be called
// prior to calling MakeFindFile.  At that point, FindFile will create its own
// fonts if the fonts are NULL.  (It will also create whatever fonts are
// missing.
//
MSOAPIX_(HRESULT) MsoSetFindFileFont(HFONT hfont, MSOFONT ifont);


///////////////////////////////////////////////////////////////////////////////
// MsoCreateMonitorWindow
//
// Apps which call FLoadFileOpen in a thread (Word) must first call this
// routine from the main thread (where they will call ShowDlg from) in order
// to create a little monitor window we use for Plug and Play messages.  This
// must be created BEFORE FLoadFileOpen is called.
//
MSOAPI_(HRESULT) MsoCreateMonitorWindow(void);


///////////////////////////////////////////////////////////////////////////////
// MsoDwGetShellSetting
//
// Get a shell setting.
//
MSOAPI_(DWORD) MsoDwGetShellSetting(MSOSSF msossf);


///////////////////////////////////////////////////////////////////////////////
// IMsoSearch
//
// Represents a search and the search results.

interface IMsoFoundFileList;		// Forward declare
interface IMsoSelectedFileList;	// Forward declare

#undef INTERFACE
#define INTERFACE IMsoSearch

DECLARE_INTERFACE_(IMsoSearch, IUnknown)
{
	// ------------------------------------------------------------------------
	// Begin Interface
	//
	// This is important on the Mac.
	BEGIN_MSOINTERFACE

	// ------------------------------------------------------------------------
	// IUnknown Methods

	MSOMETHOD(QueryInterface) (THIS_ REFIID refiid, void **ppvObject) PURE;

	MSOMETHOD_(ULONG, AddRef) (THIS) PURE;

	MSOMETHOD_(ULONG, Release) (THIS) PURE;

	// ------------------------------------------------------------------------
	// Standard Office Debug method
	MSODEBUGMETHOD

	// ------------------------------------------------------------------------
	// Query Modification

	// GetSzFileName() - Retrieves the file name for this search.  A blank
	// file name implies that all files will match (subject to other
	// criteria).  FF_S_TRUNCATED is returned if the file name was truncated.

	MSOMETHOD(GetSzFileName) (THIS_ WCHAR *szFileName, int cb)
			CONST_METHOD_FF PURE;

	// SetSzFileName() - Sets the file name for the search.  A zero-length
	// or NULL file name are both treated as matching all files.

	MSOMETHOD(SetSzFileName) (THIS_ const WCHAR *szFileName) PURE;

	// ------------------------------------------------------------------------
	// Search Results

	// GetIMsoFoundFileList() - Returns the object representing the list of
	// found files.

	MSOMETHOD(GetIMsoFoundFileList) (THIS_
			interface IMsoFoundFileList ** ppffilelist)
			CONST_METHOD_FF PURE;

	// GetISelectedFileList() - Returns the object representing the list of
	// selected files.

	MSOMETHOD(GetIMsoSelectedFileList) (THIS_
			interface IMsoSelectedFileList ** ppselfilelist) CONST_METHOD_FF PURE;

	// UNDONE:  Move these APIs to where they belong. /Steve

	// FGetPropSzValMacro() - Gets the value of the contraint on the property
	// specified by the enumerated type.  FFalse is returned if the contraint
	// on this property is too complex, i.e. that there is more than one
	// criterion on the given property.  Otherwise, fTrue is returned, even
	// if there is no criterion on this property, in which case the returned
	// string is zero-length.

	MSOMETHOD_(BOOL, FGetPropSzValMacro) (THIS_ MSOSTDPROP stdprop,
			WCHAR *szPropVal, int cb)
			CONST_METHOD_FF PURE;

	// SetPropSzValMacro() - Sets the value for the criterion of the given
	// property, removing all other criteria for the property, even if the
	// pre-existing criteria were complex.  No criteria is set for the property
	// if the given string is zero-length.  If wzErrMsg is non-NULL, it should
	// point to a buffer that will contain an error message if this routine
	// returns MSO_E_SYNTAX_ERROR.  In this case, cchErrMsg is the size of
	// the buffer passed.  Make this buffer at least 300 characters to insure
	// that you can hold all error messages.  If an error is returned and
	// pichFirst and pichLim are non-NULL, the integers they point to will be
	// set to the character positions within the input string (szPropVal)
	// that delimit the error.

	MSOMETHOD(SetPropSzValMacro) (THIS_ MSOSTDPROP stdprop,
			const WCHAR *szPropVal,
			WCHAR *wzErrMsg,
			unsigned cchErrMsg,
			int	*pichFirst,
			int *pichLim) PURE;

	// Clear() - Clears the search.

	MSOMETHOD(Clear) (THIS) PURE;

	// SetSearchPath() - Sets the directory(s) to search.
	// If NULL, the current directory is used.  The directories
	// should be a list separated with the list separator.

	MSOMETHOD(SetSearchPath) (THIS_ const WCHAR * szPath) PURE;

	// GetSearchPath() - Gets the directory(s) to search.
	// The list of directories is returned as a list
	// separated with the list separator.  The buffer pointed
	// to by szPath is cbMax bytes long.

	MSOMETHOD(GetSearchPath) (THIS_ WCHAR * szPath, int cbMax) CONST_METHOD_FF PURE;

	// GetName() - Gets the name of the search.
	// The buffer pointed to by szName is cbMax bytes long.
	// Note that all searches have generated names.

	MSOMETHOD(GetName) (THIS_ WCHAR * szName, int cbMax) CONST_METHOD_FF  PURE;

	// SaveAs() - Saves the search under the name given.

	MSOMETHOD(SaveAs) (THIS_ const WCHAR * szName) PURE;

	// Load() - Loads the named search given.

	MSOMETHOD(Load) (THIS_ const WCHAR * szName) PURE;

	// Delete() - Deletes the search.

	MSOMETHOD(Delete) (THIS) PURE;

	// SetSearchConnector() - Sets whether to connect criteria specified
	// with SetPropSzValMacro should be connected with AND or OR.  Default
	// is AND.  The same connector should be used for all the things set
	// through SetPropSzValMacro.

	MSOMETHOD(SetSearchConnector) (THIS_ MSODMCONN msodmconn) PURE;

	// SetSearchPathEx() - Sets the directory(s) to search.
	// If NULL, the current directory is used.  The directories
	// should be a list separated with the list separator.  The msospf
	// may be used to pass additional flags (see above).

	MSOMETHOD(SetSearchPathEx) (THIS_ const WCHAR * szPath, MSOSPF msospf) PURE;

	// SetSearchPathPidl() - Sets the directory(s) to search.
	// The ppidl is an array of pidl (so multpile pidl folders can be
	// passed). Each pidl passed should be relative to desktop folder (or in other
	// words, absolute pidl). cpidl is the number of pidls passed.
	// NOTE: ppidl is taken as a void* parameter. However, it should be LPITEMIDLIST*
	// The reason for it being void here is that LPITEMIDLIST's definition is in
	// shlobj.h and excel fails to compile if you include it in this file.

	MSOMETHOD(SetSearchPathPidl) (THIS_ const void *ppidl, int cpidl, MSOSPF msospf) PURE;
};


/* Get the magic pidl for Publishing Places
   NOTE: ppidl is taken as a void* parameter. However, it should be LPITEMIDLIST*
   The reason for it being void here is that LPITEMIDLIST's definition is in
   shlobj.h and excel fails to compile if you include it in this file.
   This function should always be called in pairs. First time with fAlloc == fTrue. The
   pidl will be allocated in this case. Second time with fAlloc == fFalse. The pidl
   will be freed and no longer usable. */
MSOAPI_(HRESULT) MsoGetPublishingPlacesPidl(void *ppidl, BOOL fAlloc);
MSOAPI_(HRESULT) MsoHrCreatePublishingPlace(HWND hwndParent, LPCWSTR wzUrl, LPCWSTR wzFriendlyName);
///////////////////////////////////////////////////////////////////////////////
// IMsoFoundFileList
//
// Represents the list of found files.

interface IMsoFoundFile;			// Forward declare

#undef INTERFACE
#define INTERFACE IMsoFoundFileList

DECLARE_INTERFACE_(IMsoFoundFileList, IUnknown)
{
	// ------------------------------------------------------------------------
	// Begin Interface
	//
	// This is important on the Mac.
	BEGIN_MSOINTERFACE

	// ------------------------------------------------------------------------
	// IUnknown Methods

	MSOMETHOD(QueryInterface) (THIS_ REFIID refiid, void **ppvObject) PURE;

	MSOMETHOD_(ULONG, AddRef) (THIS) PURE;

	MSOMETHOD_(ULONG, Release) (THIS) PURE;

	// ------------------------------------------------------------------------
	// Standard Office Debug method
	MSODEBUGMETHOD

	// ------------------------------------------------------------------------
	// Methods

	// CffGet() - Returns the count of found files in the collection.

	MSOMETHOD_(int, CffGet) (THIS) CONST_METHOD_FF PURE;

	// GetIMsoFoundFile()

	MSOMETHOD(GetIMsoFoundFile) (THIS_ int iff, interface IMsoFoundFile ** ppffile)
			PURE;

	// GetIMsoOLDocumentI() - Returns an IMsoOLDocument interface for the found file.

	MSOMETHOD(GetIMsoOLDocumentI) (THIS_ interface IMsoOLDocument **ppIOLDocument,
			int iFile) PURE;

	// GetPropI() - Returns a standard property for the iffth found file.  This
	// returns the raw data in the VARIANT passed in.  This function calls
	// VariantInit, not VariantClear, on pvarg, so the caller is responsible for
	// freeing any memory before calling this function as well as, of course,
	// afterwards.

	MSOMETHOD(GetPropI) (THIS_ int iff, MSOSTDPROP prop, VARIANTARG* pvarg) PURE;

	// GetPropISz() - Returns a standard property for the iffth found file as
	// a string, properly formatted.  Note, for unicode the cb parameter is
	// actually cch (in the confusing manner of all windows APIs).

	MSOMETHOD(GetPropISz) (THIS_ int iff, MSOSTDPROP prop, WCHAR *psz, int cb) PURE;
};



///////////////////////////////////////////////////////////////////////////////
// IMsoSelectedFileList
//
// Represents the list of selected files.

interface IMsoFoundFile;			// Forward declare

#undef INTERFACE
#define INTERFACE IMsoSelectedFileList

DECLARE_INTERFACE_(IMsoSelectedFileList, IUnknown)
{
	// ------------------------------------------------------------------------
	// Begin Interface
	//
	// This is important on the Mac.
	BEGIN_MSOINTERFACE

	// ------------------------------------------------------------------------
	// IUnknown Methods

	MSOMETHOD(QueryInterface) (THIS_ REFIID refiid, void **ppvObject) PURE;

	MSOMETHOD_(ULONG, AddRef) (THIS) PURE;

	MSOMETHOD_(ULONG, Release) (THIS) PURE;

	// ------------------------------------------------------------------------
	// Standard Office Debug method
	MSODEBUGMETHOD

	// ------------------------------------------------------------------------
	// Methods

	// CffGet() - Returns the count of found files in the collection.

	MSOMETHOD_(int, CffGet) (THIS) CONST_METHOD_FF PURE;

	// GetIMsoFoundFile()

	MSOMETHOD(GetIMsoFoundFile) (THIS_ int iff, interface IMsoFoundFile ** ppffile)
			CONST_METHOD_FF PURE;

	// GetIMsoOLDocumentI() - Returns an IMsoOLDocument interface for the selected
	//					   file.

	MSOMETHOD(GetIMsoOLDocumentI) (THIS_ interface IMsoOLDocument **ppIOLDocument,
			int iFile) CONST_METHOD_FF PURE;
};



///////////////////////////////////////////////////////////////////////////////
// IMsoFoundFile
//
// Represents a single found file.

#undef INTERFACE
#define INTERFACE IMsoFoundFile

DECLARE_INTERFACE_(IMsoFoundFile, IUnknown)
{
	// ------------------------------------------------------------------------
	// Begin Interface
	//
	// This is important on the Mac.
	BEGIN_MSOINTERFACE

	// ------------------------------------------------------------------------
	// IUnknown Methods

	MSOMETHOD(QueryInterface) (THIS_ REFIID refiid, void **ppvObject) PURE;

	MSOMETHOD_(ULONG, AddRef) (THIS) PURE;

	MSOMETHOD_(ULONG, Release) (THIS) PURE;

	// ------------------------------------------------------------------------
	// Standard Office Debug method
	MSODEBUGMETHOD

	// ------------------------------------------------------------------------
	// Methods

	// GetPath() - Returns the path of the found file.

	MSOMETHOD(GetPath) (THIS_ WCHAR * szPath, int cbPath) CONST_METHOD_FF PURE;

	// GetIMsofile() - Returns the index of the found file.

	MSOMETHOD(GetIfile) (THIS_ int *pifile) CONST_METHOD_FF PURE;

	// GetIMsoOLDocument() - Returns an IOLDocument interface for the found file.

	MSOMETHOD(GetIMsoOLDocument) (THIS_ interface IMsoOLDocument **ppIOLDocument)
			CONST_METHOD_FF PURE;
};



///////////////////////////////////////////////////////////////////////////////
// IMsoDMControl
//
// Represents a custom item.  Used to create it in a Find File dialog, and
// also to get and set it's value.

#undef INTERFACE
#define INTERFACE IMsoDMControl

DECLARE_INTERFACE_(IMsoDMControl, IUnknown)
{
	// ------------------------------------------------------------------------
	// Begin Interface
	//
	// This is important on the Mac.
	BEGIN_MSOINTERFACE

	// ------------------------------------------------------------------------
	// IUnknown Methods

	MSOMETHOD(QueryInterface) (THIS_ REFIID refiid, void **ppvObject) PURE;

	MSOMETHOD_(ULONG, AddRef) (THIS) PURE;

	MSOMETHOD_(ULONG, Release) (THIS) PURE;

	// ------------------------------------------------------------------------
	// Standard Office Debug method
	MSODEBUGMETHOD

	// ------------------------------------------------------------------------
	// Methods to get and set the properties of a custom item.  Unless
	// otherwise specified, the methods that set properties can only be
	// called when the dialog is visible.

	// Otp() - Returns the item type of the custom item.

	MSOMETHOD_(MSOOTP, Otp) (THIS) CONST_METHOD_FF PURE;

	// SetOtp() - Sets the item type of the custom item.

	MSOMETHOD(SetOtp) (THIS_ MSOOTP otp) PURE;

	// GetSzTitle() - Gets the title of the item and places it in the given
	// buffer, null-terminated.  The buffer must be at least
	// IFindFile::cbMaxItemTitle in length.

	MSOMETHOD_(void, GetSzTitle) (THIS_ WCHAR * szTitle) CONST_METHOD_FF PURE;

	// SetSzTitle() - Sets the title of the item.  The given title must be
	// null-terminated, and no longer than IFindFile::cbMaxItemTitle with
	// the terminator.

	MSOMETHOD(SetSzTitle) (THIS_ const WCHAR * szTitle) PURE;

	// FEnabled() - Returns fTrue if the item is enabled.

	MSOMETHOD_(MSOBOOL, FEnabled) (THIS) PURE;

	// Enable() - Enables or disables the item.  Can be set while the dialog
	// is visible.

	MSOMETHOD_(void, Enable) (THIS_ MSOBOOL fEnable) PURE;

	// GetValue() - Gets the value for the item.  If it is a checkbox or
	// radio group, then the one returning an int should be used.  Otherwise,
	// the one returning a text string should be used.

	MSOMETHOD(GetValueW) (THIS_ int * pval) PURE;

	MSOMETHOD(GetValueSz) (THIS_ WCHAR * szVal, int cbVal) PURE;

	// SetValue() - Sets the value for the item.  If a checkbox or radio
	// group, use the int version.  Otherwise, use the text string version.

	MSOMETHOD(SetValueW) (THIS_ int val) PURE;

	MSOMETHOD(SetValueSz) (THIS_ const WCHAR * wzVal) PURE;

	MSOMETHOD(SetValueSzEx) (THIS_ const WCHAR * wzVal,
								   const WCHAR * wzBtnText,
								   const WCHAR * wzDlgTitle,
								   const WCHAR * wzDescrText) PURE;

	// SetFEnableWithOK ties enabling of a custom control to enabling of OK button
	MSOMETHOD_(void, SetFEnableWithOK) (THIS_ MSOBOOL fEnableWithOKButton) PURE;

	// Returns whether control's enabledment is tied to that of OK button
	MSOMETHOD_(MSOBOOL, FEnabledWithOK) (THIS) PURE;

	// Sets the index into the command list for a control.  This is called
	// at dialog init time by the Open code, and the apps should not call this.
	// Once this is set (automatically at dialog init time), the app can call
	// the associated Get method (ICleGet()).
	MSOMETHOD_(void, SetICle) (THIS_ MSODMICLE icle) PURE;

	// Returns the command list index for this control, which is automatically
	// set at dialog init time.  The app never actually needs to call this
	// since we guarantee that the order of the custom commands matches the
	// order of the custom controls.  So the first custom toolbar control added
	// is assigned msodmicleAppToolsFirst, the second is assigned
	// msodmicleAppToolsFirst+1 and so on.  Similarly, the first Open Button
	// Dropdown custom item is msodmicleAppOpenDropdnFirst, the second is
	// msodmicleAppOpenDropdnFirst+1, and so on.
	MSOMETHOD_(MSODMICLE, ICleGet) (THIS) PURE;

	// SetFDisablesOKDropDown ties disabling of the OK button drop down to the custom control
	MSOMETHOD_(void, SetFDisablesOKDropDown) (THIS_ MSOBOOL fDisablesOKDropDown) PURE;

	// Returns whether control disables the OK button drop down
	MSOMETHOD_(MSOBOOL, FDisablesOKDropDown) (THIS) PURE;

	// Sets alternate OK button title tied to the custom control
	MSOMETHOD(SetAltOpenTitle) (THIS_ const WCHAR * wzTitle) PURE;

	// Gets alternate OK button title tied to the custom control
	MSOMETHOD_(void, GetAltOpenTitle) (THIS_ WCHAR * wzTitle) PURE;
};



///////////////////////////////////////////////////////////////////////////////
// MsoMakeCustomItem
//
// Returns a new IMsoDMControl instance.

MSOAPI_(HRESULT) MsoMakeCustomItem(IMsoDMControl ** ppcstitm);



///////////////////////////////////////////////////////////////////////////////
// IMsoControlList
//
// The collection of custom items.

#undef INTERFACE
#define INTERFACE IMsoControlList
DECLARE_INTERFACE_(IMsoControlList, IUnknown)
{
	// ------------------------------------------------------------------------
	// Begin Interface
	//
	// This is important on the Mac.
	BEGIN_MSOINTERFACE

	// ------------------------------------------------------------------------
	// IUnknown Methods

	MSOMETHOD(QueryInterface) (THIS_ REFIID refiid, void **ppvObject) PURE;

	MSOMETHOD_(ULONG, AddRef) (THIS) PURE;

	MSOMETHOD_(ULONG, Release) (THIS) PURE;

	// ------------------------------------------------------------------------
	// Standard Office Debug method
	MSODEBUGMETHOD

	// ------------------------------------------------------------------------
	// Methods

	// CcntrlGet() - Returns the number of custom controls in the collection.

	MSOMETHOD_(int, CcntrlGet) (THIS) CONST_METHOD_FF PURE;

	// Get() - Gets a custom item from the collection.

	MSOMETHOD(Get) (THIS_ int icstitm, interface IMsoDMControl ** ppcstitm)
			CONST_METHOD_FF PURE;

	// Append() - Appends a custom item to the collection.  Can use either a
	// fully-formed IControl, or give the parameters individually.
	// (The caller passes the IMsoDMControl to the IMsoControlList by calling
	// this method.  If this method succeeds, the IMsoControlList will release
	// the IMsoDMControl when it is done using the IMsoDMControl.  If the
	// caller wishes to continue using the IMsoDMControl after calling this
	// method, the caller should call AddRef on the IMsoDMControl before
	// calling this method.)

	MSOMETHOD(Append) (THIS_ interface IMsoDMControl * pcstitm) PURE;

	// Delete() - Deletes the specified custom item from the collection.

	MSOMETHOD(Delete) (THIS_ int icstitm) PURE;

	// IcntrlOfHid() - Given a HID (Help ID) passed to the help
	// callback, return the Icntrl to which it refers.  Returns FF_iotmMin
	// if this help id refers to a control not publicly
	// exposed (i.e. not in FF_IOTM).
	MSOMETHOD_(int, IcntrlOfHid) (THIS_ unsigned long hid) CONST_METHOD_FF PURE;
};



///////////////////////////////////////////////////////////////////////////////
// IMsoCommandList
//
// Collection of commands available from the toolbar, toolbar drop downs.

#undef INTERFACE
#define INTERFACE IMsoCommandList
DECLARE_INTERFACE_(IMsoCommandList, IUnknown)
{
	// ------------------------------------------------------------------------
	// Begin Interface
	//
	// This is important on the Mac.
	BEGIN_MSOINTERFACE

	// ------------------------------------------------------------------------
	// IUnknown Methods

	MSOMETHOD(QueryInterface) (THIS_ REFIID refiid, void **ppvObject) PURE;

	MSOMETHOD_(ULONG, AddRef) (THIS) PURE;

	MSOMETHOD_(ULONG, Release) (THIS) PURE;

	// ------------------------------------------------------------------------
	// Standard Office Debug method
	MSODEBUGMETHOD

	// ------------------------------------------------------------------------
	// Methods

	// CszGet() - Returns the number of types in the collection.

	MSOMETHOD_(int, CcmdGet) (THIS) CONST_METHOD_FF PURE;

	// Get() - Returns the requested command.  The given buffer must be of
	// length IFindFile::cbMaxCommand.

	MSOMETHOD(Get) (THIS_ MSODMICLE icle, WCHAR * szCle) CONST_METHOD_FF PURE;

	// GetFEnabled() - Returns fTrue if the given command is enabled, fFalse
	// otherwise.

	MSOMETHOD(GetFEnabled) (THIS_ MSODMICLE icle, MSOBOOL * pfEnabled) CONST_METHOD_FF PURE;

	// SetFEnabled() - Enables/disables commands.

	MSOMETHOD(SetFEnabled) (THIS_ MSODMICLE icle, MSOBOOL fEnabled) PURE;

	// GetFVisible() - Returns fTrue if the given command is visible, fFalse
	// otherwise.

	MSOMETHOD(GetFVisible) (THIS_ MSODMICLE icle, MSOBOOL * pfVisible) CONST_METHOD_FF PURE;

	// SetFVisible() - Makes commands visible or hides them.

	MSOMETHOD(SetFVisible) (THIS_ MSODMICLE icle, MSOBOOL fVisible) PURE;
};


///////////////////////////////////////////////////////////////////////////////
// IMsoFileTypeList
//
// Collection of file types to display in the file types dropdown.  Each type
// should be of the format "Description (.tp1, .tp2, ...)"

#undef INTERFACE
#define INTERFACE IMsoFileTypeList
DECLARE_INTERFACE_(IMsoFileTypeList, IUnknown)
{
	// ------------------------------------------------------------------------
	// Begin Interface
	//
	// This is important on the Mac.
	BEGIN_MSOINTERFACE

	// ------------------------------------------------------------------------
	// IUnknown Methods

	MSOMETHOD(QueryInterface) (THIS_ REFIID refiid, void **ppvObject) PURE;

	MSOMETHOD_(ULONG, AddRef) (THIS) PURE;

	MSOMETHOD_(ULONG, Release) (THIS) PURE;

	// ------------------------------------------------------------------------
	// Standard Office Debug method
	MSODEBUGMETHOD

	// ------------------------------------------------------------------------
	// Methods

	// CszGet() - Returns the number of types in the collection.

	MSOMETHOD_(int, CszGet) (THIS) CONST_METHOD_FF PURE;

	// Get() - Returns the requested type.  The given buffer must be of
	// length IFindFile::cbMaxType.

	MSOMETHOD(Get) (THIS_ int iszType, WCHAR * szType, MSOBOOL * pfFreezeDir)
			CONST_METHOD_FF PURE;

	// Append() - Appends the given type to the list.  Buffer should be no
	// longer than IFindFile::cbMaxType in length.

	MSOMETHOD(Append) (THIS_ const WCHAR * szType, MSOBOOL fFreezeDir) PURE;

	// Delete() - Deletes the given type from the collection.

	MSOMETHOD(Delete) (THIS_ int iszType) PURE;

	// IszDefaultGet() - Returns the index of the default type.  Usually this
	// is zero.

	MSOMETHOD_(int, IszDefaultGet) (THIS) CONST_METHOD_FF PURE;

	// SetIszDefault() - Sets the index of the default type.

	MSOMETHOD(SetIszDefault) (THIS_ int iszDefault) PURE;

	// AppendEx() - Appends the given type to the list.  Buffer should be no
	// longer than IFindFile::cbMaxType in length.
	// Allows specifying extra flags.

	MSOMETHOD(AppendEx) (THIS_ const WCHAR * szType, MSOFTF msoftf) PURE;

};

///////////////////////////////////////////////////////////////////////////////
// IMsoCodePageList
//
// Collection of code pages

#undef INTERFACE
#define INTERFACE IMsoCodePageList
DECLARE_INTERFACE_(IMsoCodePageList, IUnknown)
{
	// ------------------------------------------------------------------------
	// Begin Interface
	//
	// This is important on the Mac.
	BEGIN_MSOINTERFACE

	// ------------------------------------------------------------------------
	// IUnknown Methods

	MSOMETHOD(QueryInterface) (THIS_ REFIID refiid, void **ppvObject) PURE;

	MSOMETHOD_(ULONG, AddRef) (THIS) PURE;

	MSOMETHOD_(ULONG, Release) (THIS) PURE;

	// ------------------------------------------------------------------------
	// Standard Office Debug method
	MSODEBUGMETHOD

	// ------------------------------------------------------------------------
	// Methods

	// CszGet() - Returns the number of types in the collection.

	MSOMETHOD_(int, CszGet) (THIS) CONST_METHOD_FF PURE;

	// Get() - Returns the requested type.  The given buffer must be of
	// length IFindFile::cbMaxCodePage.

	MSOMETHOD(Get) (THIS_ int iszCodePage, WCHAR * szCodePage)
			CONST_METHOD_FF PURE;

	// Append() - Appends the given type to the list.  Buffer should be no
	// longer than IFindFile::cbMaxCodePage in length.

	MSOMETHOD(Append) (THIS_ const WCHAR * szCodePage) PURE;

	// Delete() - Deletes the given type from the collection.

	MSOMETHOD(Delete) (THIS_ int iszCodePage) PURE;

	// IszDefaultGet() - Returns the index of the default type.  Usually this
	// is zero.

	MSOMETHOD_(int, IszDefaultGet) (THIS) CONST_METHOD_FF PURE;

	// SetIszDefault() - Sets the index of the default type.

	MSOMETHOD(SetIszDefault) (THIS_ int iszDefault) PURE;
};


///////////////////////////////////////////////////////////////////////////////
// IMsoOLDocument
//
// IMsoOLDocument represents a version of a document stored in some storage,
// but it may not be stored in a file in the filesystem.  (For example, the
// document could be a storage in a MAPI store.)  IMsoOLDocument is also the
// interface that allows the applications to communicate with the Office
// Librarian.

#undef INTERFACE
#define INTERFACE IMsoOLDocument

DECLARE_INTERFACE_(IMsoOLDocument, IUnknown)
{
	// ------------------------------------------------------------------------
	// Begin Interface
	//
	// This is important on the Mac.
	BEGIN_MSOINTERFACE

	// ------------------------------------------------------------------------
	// IUnknown Methods

	MSOMETHOD(QueryInterface) (THIS_ REFIID refiid, void * * ppvObject) PURE;

	MSOMETHOD_(ULONG, AddRef) (THIS) PURE;

	MSOMETHOD_(ULONG, Release) (THIS) PURE;


	// ------------------------------------------------------------------------
	// Standard Office Debug method
	MSODEBUGMETHOD


	// ------------------------------------------------------------------------
	// Platform-independent methods.


	// GetInterface() - Get an AddRefed pointer to the specified interface
	//                  for this document.  Unlike QueryInterface, the
	//                  interface returned by this method may not be used
	//                  to get back to this IMsoOLDocument.  (Calling
	//                  QueryInterface for IID_IMsoOLDocument on the
	//                  returned interface may fail.)

	MSOMETHOD(GetInterface) (THIS_ IUnknown * * ppIUnknown, REFIID refiid)
			PURE;

	// SetInterface() - Give an interface to this IMsoOLDocument.  This is used
	//                  to give interfaces like IStorage, IBindStatusCallback,
	//                  and IBindStatusCallback to this object.  If pIUnknown
	//                  is NULL, this IMsoOLDocument will release and interface
	//                  of the specified type given to it via a previous call
	//                  to SetInterface.

	MSOMETHOD(SetInterface) (THIS_ IUnknown * pIUnknown, REFIID refiid) PURE;

	// GetPwTdi() - Get an integer value giving the specified type of
	//              information about this document.

	MSOMETHOD(GetPwTdi) (THIS_ int *pw, MSOIOLTDI tdi) PURE;

	// SetWTdi() - Set an integer value giving the specified type of
	//             information about this document.

	MSOMETHOD(SetWTdi) (THIS_ int w, MSOIOLTDI tdi) PURE;

	// GetWzPcchGdn() - Get the specified string for this document.  This
	//                  method is used to get display names and file paths.
	//                  If the caller wants to find the number of characters
	//                  (including the terminating '\0' character) in a
	//                  string, the caller can set wz to NULL and the count
	//                  will be returned in *pcch.  If wz is not NULL, the
	//                  caller should pass the buffer size (including the
	//                  terminating '\0' character) in *pcch, and the number
	//                  of characters copied to wz (including the terminating
	//                  '\0' character) will be returned in *pcch.

	MSOMETHOD(GetWzPcchGdn) (THIS_ WCHAR * wz, ULONG * pcch, MSOIOLGDN gdn)
			PURE;

	// SetWzGdn() - Set the specified string for this document.  This method
	//              is used to set the Author, Manager, Company, and Keywords
	//              strings used with Ren.  This method may fail if the
	//              application is not allowed to set the specified string.
	//              (For example, this will fail if the application attempts
	//              to change the persistent name.)

	MSOMETHOD(SetWzGdn) (THIS_ const WCHAR * wz, MSOIOLGDN gdn) PURE;

	// AttrGet() - Get the atrributes of this document.
	//
	MSOMETHOD_(MSOIOLATTR, AttrGet) (THIS) CONST_METHOD_FF PURE;

	// SetAttrInAttrMask() - Set the attributes selected in attrMask to the
	//                       values given in attr.

	MSOMETHOD(SetAttrInAttrMask) (THIS_ MSOIOLATTR attr, MSOIOLATTR attrMask)
			PURE;

	// BeginCmd() - Find the operation which the application must perform to
	//				complete the specified command.

	MSOMETHOD(BeginCmd) (THIS_ MSOIOLCMD cmd,
			IMsoOLDocument * * ppIOLDocOther) PURE;

	// RecordEvent() - Indicate that a command has been performed, or that
	//                 the user has performed some action on this document.
	//                 The hr parameter indicates whether the command or
	//                 action succeeded or failed.

	// UNDONE: MSOMETHOD(RecordEvent) (THIS_ MSOIOLEVT evt, HRESULT hr,
	MSOMETHOD(RecordEvent) (THIS_ MSOIOLEVT evt, const void *pvMisc,
			GUID * pguid) PURE;

	// ------------------------------------------------------------------------
	// Future Office Librarian methods.  (Not used in MSO97.)


	// GetPguidTogI() - Get the specified GUID.  Depending on tog, an index
	//                  may or may not be needed.

	MSOMETHOD(GetPguidTogI) (THIS_ GUID * pguid, MSOIOLTOG tog, int i)
			PURE;

	// SetPguidTogI() - Set the specified GUID.  Depending on tog, an index
	//                  may or may not be needed.

	MSOMETHOD(SetPguidTogI) (THIS_ const GUID * pguid, MSOIOLTOG tog,
			int i) PURE;

	// DoIOLDocPguidOp() - Do the specified Office Librarian operation.  The
	//                     operation may use or return a pointer to another
	//                     IOLDoc and a GUID.  The ulReserved parameter should
	//                     be set to 0.

	MSOMETHOD(DoIOLDocPguidOp) (THIS_ IMsoOLDocument * * ppIOLDocOther,
			GUID * pguid, MSOIOLOP op, ULONG ulReserved) PURE;

	// SetRenSummInfo() - Set the summary information strings used by REN
	//		when a document is closed, using given Office Summary Info objects.
	//		This function extracts the appropriate strings from the summary
	//      info objects and stores them internally (ie. the given objects
	//      are not saved).
	//
	//		Note: If Office Summary Info objects are not available, use the
	//      SetWzGdn API for each string as an alternative.
	//
	MSOMETHOD(SetRenSummInfo) (THIS_
		const LPSIOBJ lpsiobj,
		const LPDSIOBJ lpdsiobj,
		const LPUDOBJ lpudobj) PURE;

	// SetRelatedFile
	//		Adds the given file & operation data to the list of related
	//		files for the main document.
	MSOMETHOD(SetRelatedFile) (THIS_ const LPMSOIOLFOP pfop) PURE;

	// ExecuteRelatedFileOps
	//		Perform the related file operations for the ioldoc.  This currently does not support the
	//		save file operation, use RecordEvent to get that.
	// 	This also clears the list of related operations.
	MSOMETHOD(ExecuteRelatedFileOps) (THIS) PURE;

	// SetDocumentCookie
	// Associates a generic "cookie" with this document. The app can store instance-specific
	// data (ie. a pointer to a data structure, etc). Its use is defined by the caller:
	// The IMsoIOLDocument implementation has no knowledge of what this cookie means.
	MSOMETHOD(SetDocumentCookie)(THIS_ MSODMCOOKIE msoCookie) PURE;

	// GetDocumentCookie
	// Retrieves the generic "cookie" associated with this document. Its use is defined by the caller:
	// The IMsoIOLDocument implementation has no knowledge of what this cookie means.
	MSOMETHOD_(MSODMCOOKIE, GetDocumentCookie)(THIS) PURE;

	// SetPKMClient
	// If our document lies on a Tahoe server and versioning is enabled, we may bind to
	// documents with strange naming schemes, but which actually are checked out versions
	// of our requested URL doc.  The PKMClient will make this transparent in office.
	MSOMETHOD(SetPKMClient)(THIS_ interface IMsoPKMClient *pipkmclient) PURE;

	// GetPKMClient
	// Retrieves the PKMClient.. description is above in SetPKMClient property.
	MSOMETHOD(GetPKMClient)(THIS_ interface IMsoPKMClient **ppipkmclient) PURE;

	// GetServerInfo
	// Retrieves info about the web server that we are communicating with.
	MSOMETHOD(GetServerInfo)(THIS_ MSODMGSI *psvrinfo, MSOGSIOPT optMask) PURE;

	// GetCOpen
	// Retrieves the open count
	MSOMETHOD(GetCOpen)(THIS_ int *pcOpen) PURE;

#if !defined(OFFICE_BUILD) && defined(__cplusplus)
	// ------------------------------------------------------------------------
	// Inline methods (replaced by the methods above).

	// GetFilePath() - Find the location of this document in the filesystem.
	// pibc and pibsc may be NULL in which case defaults will be used if a
	// BindToStorage is required to get this path (e.g. the document is
	// referenced via a URL).

	HRESULT GetFilePath (THIS_
			interface IBindCtx *pibc, interface IBindStatusCallback *pibsc,
			WCHAR *wzPath,
			ULONG *pcbPath, BOOL fEnableRedirect)
		{
		HRESULT hr = (pcbPath == 0) ? E_POINTER : S_OK;

		if (wzPath != 0)
			*wzPath = 0;

		if ((SUCCEEDED(hr)) && (pibc != 0))
			hr = SetInterface((IUnknown *)pibc, IID_IBindCtx);
		if ((SUCCEEDED(hr)) && (pibsc != 0))
			hr = SetInterface((IUnknown *)pibsc, IID_IBindStatusCallback);

		if (SUCCEEDED(hr))
			{
			SetWTdi(fEnableRedirect, msoioltdiEnableRedirect);
			ULONG cch = *pcbPath / sizeof(WCHAR);
			hr = GetWzPcchGdn(wzPath, &cch, msoiolgdnTempFilePath);
			*pcbPath = cch * sizeof(WCHAR);
			}

		return hr;
		}

	// GetDisplayName() - Find a friendly or persistent name for this document.

	HRESULT GetDisplayName (THIS_ WCHAR *wzDisplayName,
			ULONG *pcbDisplayName, MSOIOLGDN gdn)
		{
		HRESULT hr;

		if (pcbDisplayName == 0)
			{
			if (wzDisplayName != 0)
				*wzDisplayName = 0;
			hr = E_POINTER;
			}
		else
			{
			ULONG cch = *pcbDisplayName / sizeof(WCHAR);
			hr = GetWzPcchGdn(wzDisplayName, &cch, gdn);
			*pcbDisplayName = cch * sizeof(WCHAR);
			}

		return hr;
		}

	// SetIStorage() - Give an IStorage to the IMsoOLDocument, so that the
	//				   IMsoOLDocument methods don't need to call StgOpenStorage.

	HRESULT SetIStorage (THIS_ IStorage *pIStorage, DWORD grfMode)
		{
		HRESULT hr = SetInterface(pIStorage, IID_IStorage);
		if (SUCCEEDED(hr))
			hr = SetWTdi(grfMode, msoioltdiModeIStorage);
		return hr;
		}

	// IsInFileSys() - Is this document stored in the file system?

	HRESULT IsInFileSys (THIS)
		{
		if ((AttrGet() & msoiolattrInFileSys) != 0)
			return S_OK;
		else
			return S_FALSE;
		}

	// IsANewDocument() - Is this document a new document, which does not have
	//                    a file associated with it?

	HRESULT IsANewDocument (THIS)
		{
		if ((AttrGet() & msoiolattrNewDocument) != 0)
			return S_OK;
		else
			return S_FALSE;
		}

	// GetLoggingState() - Returns an IOL_S_ELS_XXX value indicating the
	//                     current event logging state.

	HRESULT GetLoggingState (THIS) CONST_METHOD_FF
		{
		return (AttrGet() & msoiolattrRenLogging) != 0
				? MSO_S_IOLELS_REN_LOGGING : MSO_S_IOLELS_NO_LOGGING;
		}

	// SetLoggingState() - Indicate how events passed to RecordEvent will
	//                     be logged.

	HRESULT SetElsLoggingState (THIS_ MSOIOLELS elsLoggingState)
		{
		if (elsLoggingState == msoiolelsRenLogging)
			return SetAttrInAttrMask(msoiolattrRenLogging, msoiolattrRenLogging);
		else
			return SetAttrInAttrMask(0, msoiolattrRenLogging);
		}

	// GetMoniker() - Get an IMoniker interface.

	HRESULT GetMoniker (THIS_ IMoniker **ppIMoniker)
		{ return GetInterface((IUnknown **) ppIMoniker, IID_IMoniker); }

#endif // !defined(OFFICE_BUILD) && defined(__cplusplus)
};

///////////////////////////////////////////////////////////////////////////////
// MsoPIOLDocOtherGet
// Returns a Non-AddRef'd pointer to the other document associated with the
// pioldoc passed in. Usually, only interesting during a saveas.
// Keep in mind that this pointer is NOT AddRef'd and you can only use it
// within local scope
//
MSOAPI_(IMsoOLDocument *) MsoPIOLDocOtherGet(IMsoOLDocument* pioldoc);


///////////////////////////////////////////////////////////////////////////////
// MsoioloptGet
//
// Get flags describing global IMsoOLDocument option settings.
//
MSOAPIX_(MSOIOLOPT) MsoioloptGet(void);


///////////////////////////////////////////////////////////////////////////////
// MsoSetioloptInIoloptMask
//
// Change the specified global IMsoOLDocument option settings.
//
MSOAPI_(HRESULT) MsoSetioloptInIoloptMask(MSOIOLOPT opt, MSOIOLOPT optMask);


///////////////////////////////////////////////////////////////////////////////
// MsoCreateIOLDocFromMoniker
//
// Given a pointer to an IMoniker interface which represents an IMsoOLDocument,
// get an IMsoOLDocument.
//
MSOAPI_(HRESULT) MsoCreateIOLDocFromMoniker(IMsoOLDocument * * ppIOLDoc,
		IMoniker * pIMoniker, IBindCtx * pIBindCtx);


///////////////////////////////////////////////////////////////////////////////
// MsoCreateIOLDocFromWzPersistentName
//
// Given the persistent name of a document, return an IMsoOLDocument interface
// for the document.  szBaseName may be passed as NULL; if not, it will be
// used to resolve relative names in the peristent name.
//
MSOAPI_(HRESULT) MsoCreateIOLDocFromWzPersistentName(
		IMsoOLDocument * * ppIOLDoc, const WCHAR * wzPersistentName,
		const WCHAR * wzBaseName);


///////////////////////////////////////////////////////////////////////////////
// MsoCreateIOLDocTempFromWzDisplayName
//
// Given the persistent name of a document, return an IMsoOLDocument
// interface for the document.
//
MSOAPI_(HRESULT) MsoCreateIOLDocTempFromWzDisplayName(
		IMsoOLDocument * * ppIOLDoc, const WCHAR * wzDisplayName);


///////////////////////////////////////////////////////////////////////////////
// MsoPIOLDocFindWithWzPersistentName
//
// Helper function which checks all IMsoOLDocument objects currently in use
// and returns a pointer to the IMsoOLDocument which has the specified
// persistent name or 0 if no IMsoOLDocument has the specified
// persistent name.  If a pointer to an IMsoOLDocument object is returned,
// the caller must call IMsoOLDOcument::Release when finished with the
// IMsoOLDocument object.
//
MSOAPI_(IMsoOLDocument *) MsoPIOLDocFindWithWzPersistentName(
		const WCHAR * wzPersistentName, const WCHAR * wzBaseName);


MSOAPI_(int) MsoIchInFileSysWzPersistentName(const WCHAR *wzPersistentName);

///////////////////////////////////////////////////////////////////////////////

MSOAPI_(void) MsoSetSaveAsWoc(IMsoOLDocument* pioldoc);

///////////////////////////////////////////////////////////////////////////////
// MsoFIsInFileSysWzPersistentName
//
// Helper function which determines whether a given persistent name is the
// persistent name of a document in the file system.  (Actually, this
// function determines whether the form of the given persistent name is
// recognized as the form of the name of a document which is not in the
// file system.  If so, fFalse is returned, otherwise fTrue is returned.)
//
MSOAPI_(BOOL) MsoFIsInFileSysWzPersistentName(
		const WCHAR * wzPersistentName, const WCHAR * wzBaseName);

MSOAPI_(BOOL) MsoFIsUrlWzPersistentName(
		const WCHAR * wzPersistentName, const WCHAR * wzBaseName);

MSOAPI_(BOOL) MsoFIsODMAWzPersistentName(const WCHAR *wzPersistentName);

///////////////////////////////////////////////////////////////////////////////
// MsoFAddFileToRecent
//
// Add a shortcut to the given file or directory name in the recent files place
// and garbage collect expired shortcuts where appropriate.
// if fParent = True, create link to its parent
MSOAPI_(BOOL) MsoFAddToRecentFolder(WCHAR *wzFile, BOOL fParent);


///////////////////////////////////////////////////////////////////////////////
// MsoSetFDmNoModalComponents
//
// Set the SDM dialog state to have no modal components for any
// subsequent dialogs brought up by dm.  Should
// really only be needed by apps which have no need for a component
// manager, such as FindFast and Shortcut Bar, since they cannot be inplace
// in another app.  The default state of this flag is FALSE, indicating
// that there are modal components.
MSOAPI_(void) MsoSetFDmNoModalComponents (MSOBOOL f);


///////////////////////////////////////////////////////////////////////////////
// MsoWzGetTempFilename
//
//	Generates a unique temp file name that doesn't already exist and returns it.
//	The file is NOT created.  The extension of the original file is preserved.
//
//	The file name is copied into the buffer, and a pointer to it is returned.  The
//	buffer should be MAX_PATH long.
MSOAPI_(WCHAR*) MsoWzGetTempFilename (WCHAR *wz, const WCHAR *wzOrig);


///////////////////////////////////////////////////////////////////////////////
// MsoIsWzPathAnAnsiPath
//
// Helper function which determines whether a given path can be represented
// in ANSI characters.  Returns S_OK if the given path can be represented
// in ANSI characters.
//
MSOAPIX_(HRESULT) MsoIsWzPathAnAnsiPath(const WCHAR *wzPath);


///////////////////////////////////////////////////////////////////////////////
// MsoDwUrlAttributes
//
// Returns the win32 attributes of the given url.
//
// It is expected that the url has already been encoded.
//
// Param cp is the codepage context for the given url (if in doubt use CP_ACP).
//
MSOAPI_(DWORD) MsoDwUrlAttributes (const WCHAR *pwzUrl, DWORD cp, BOOL fShowUI);


///////////////////////////////////////////////////////////////////////////////
// MsoFValidSaveLocation
//
// Validates a given file location for save.
//
MSOAPI_(BOOL) MsoFValidSaveLocation(const WCHAR *wzFile, HWND hwnd);


// converts a drive mapped path to full UNC form
MSOAPIX_(void) MsoConvertDriveMappedUNCs(WCHAR *pwzPath);

// ... and vice-versa
MSOAPIX_(void) MsoConvertUNCsDriveMapped(WCHAR *pwzPath);


// the functions listed below will pass in the MAINOTIFYVAL in the Code of a successful HRESULT
#define MsoINotifyValFromHr(hr) (MSOINOTIFYVAL)(hr)
enum
	{
	msoinotifyvalCancel = 0,		//ShowDocRONotifyDlg, SaveOrThrowChangesDlg
	msoinotifyvalReadOnly,			//ShowDocRONotifyDlg, CheckDocAvailableRW,
	msoinotifyvalNotify,				//ShowDocRONotifyDlg
	msoinotifyvalShareViolation,  //CheckDocAvailableRW,
	msoinotifyvalReadWrite,			//CheckDocAvailableRW,
   msoinotifyvalNoMemory,			//CheckDocAvailableRW,
	msoinotifyvalNoServerChanges,	//SaveOrThrowChangesDlg
	msoinotifyvalDiscardChanges,  //SaveOrThrowChangesDlg
	msoinotifyvalSaveAsChanges,   //SaveOrThrowChangesDlg
	msoinotifyvalUnknown,			//All 3
	msoinotifyvalSameTime,			//CompareDocTimestamp
	msoinotifyvalDifferentTime,	//CompareDocTimestamp
	msoinotifyvalMerge,				//ShowDocRONotifyDlg
	msoinotifyvalNoFile,		// CheckDocAvailableRW
	};

///////////////////////////////////////////////////////////////////////////////
// IMsoDocumentNotifyList
//
// IMsoDocumentNotifyList is an interface that maintains a list of documents opened
// by the applications in read-only mode (becuase the document was in use and
// locked by another application somewhere) and have requested to be notified
// once the document is available for editing.

#undef INTERFACE
#define INTERFACE IMsoDocumentNotifyList

DECLARE_INTERFACE_(IMsoDocumentNotifyList, IUnknown)
{
	// ------------------------------------------------------------------------
	// Begin Interface
	//
	// This is important on the Mac.
	BEGIN_MSOINTERFACE

	// ------------------------------------------------------------------------
	// IUnknown Methods

	MSOMETHOD(QueryInterface) (THIS_ REFIID refiid, void * * ppvObject) PURE;

	MSOMETHOD_(ULONG, AddRef) (THIS) PURE;

	MSOMETHOD_(ULONG, Release) (THIS) PURE;


	// ------------------------------------------------------------------------
	// Standard Office Debug method
	MSODEBUGMETHOD


	// ------------------------------------------------------------------------
	// Platform-independent methods.

	MSOMETHOD(SetDocNotifyCallback) (THIS_ PFNDOCNOTIFYCALLBACK pfnDocNotifyCallBack) PURE;

	MSOMETHOD(CheckDocsAndNotify) (THIS) PURE;

	MSOMETHOD(AddDocToNotifyList) (THIS_ const WCHAR * wzDocPersistName) PURE;

	MSOMETHOD(AddDocToNotifyListEx) (THIS_ const WCHAR * wzDocPersistName, PFNCHECKDOCAVAILABLERW pfnCheckDocAvailableRW) PURE;

	MSOMETHOD(RemoveDocFromNotifyList) (THIS_ const WCHAR * wzDocPersistName) PURE;

	MSOMETHOD(RemoveDocFromNotifyListEx) (THIS_ const WCHAR * wzDocPersistName) PURE;

	MSOMETHOD(CheckDocAvailableRW) (THIS_ const WCHAR * wzDocPersistName) PURE;

	MSOMETHOD(ShowDocRONotifyDlg) (THIS_ const WCHAR * wzDocPersistName, const WCHAR * wzUserName, BOOL fForceNotify, BOOL fHideRO) PURE;

	MSOMETHOD(ShowDocRONotifyDlgEx) (THIS_ const WCHAR * wzDocPersistName, const WCHAR * wzUserName, PFNCHECKDOCAVAILABLERW pfnCheckDocAvailableRW, BOOL fForceNotify, BOOL fHideRO) PURE;

	MSOMETHOD(ShowDocRWNotifyDlg) (THIS_ const WCHAR * wzDocPersistName) PURE;

	MSOMETHOD(ShowDocSaveThrowChgDlg) (THIS_ const WCHAR * wzDocPersistName, MSOINOTIFYVAL * pRetVal) PURE;

	MSOMETHOD(AddOLDocToNotifyList) (THIS_ IMsoOLDocument * pIOLDoc) PURE;

	MSOMETHOD(RemoveOLDocFromNotifyList) (THIS_ IMsoOLDocument * pIOLDoc) PURE;

	MSOMETHOD(CheckOLDocAvailableRW) (THIS_ IMsoOLDocument * pIOLDoc) PURE;

	MSOMETHOD(ShowOLDocRONotifyDlg) (THIS_ IMsoOLDocument * pIOLDoc, const WCHAR * wzUserName, BOOL fForceNotify, BOOL fHideRO) PURE;

	MSOMETHOD(ShowOLDocRWNotifyDlg) (THIS_ IMsoOLDocument * pIOLDoc) PURE;

	MSOMETHOD(ShowOLDocSaveThrowChgDlg) (THIS_ IMsoOLDocument * pIOLDoc, MSOINOTIFYVAL * pRetVal) PURE;

	MSOMETHOD(CompareOLDocTimestamp) (THIS_ IMsoOLDocument * pIOLDoc, FILETIME ftCompare, MSOINOTIFYVAL * pValRet) PURE;

	MSOMETHOD(CompareDocTimestamp) (THIS_ const WCHAR * wzPersistName, FILETIME ftCompare, MSOINOTIFYVAL * pValRet) PURE;

	MSOMETHOD(SetDocNotifyDlgCallbacks) (THIS_ PFNDODOCRONOTIFYDLG pfnDoDocRONotifyDlg, PFNDODOCRWNOTIFYDLG pfnDoDocRWNotifyDlg) PURE;

	MSOMETHOD(SetDocCopyNameInNotifyList) (THIS_ const WCHAR * wzPersistName, const WCHAR * wzCopyName);

};



///////////////////////////////////////////////////////////////////////////////
// IGSV
//
// A useful interface for getting / setting values which an object uses.

#undef INTERFACE
#define INTERFACE IMsoGSV

DECLARE_INTERFACE_(IMsoGSV, IUnknown)
{
	// ------------------------------------------------------------------------
	// Begin Interface
	//
	// This is important on the Mac.
	BEGIN_MSOINTERFACE

	// ------------------------------------------------------------------------
	// IUnknown Methods

	MSOMETHOD(QueryInterface) (THIS_ REFIID refiid, void * * ppvObject) PURE;

	MSOMETHOD_(ULONG, AddRef) (THIS) PURE;

	MSOMETHOD_(ULONG, Release) (THIS) PURE;

	// ------------------------------------------------------------------------
	// Standard Office Debug method
	MSODEBUGMETHOD

	// ------------------------------------------------------------------------
	// IMsoGSV methods.


	// GetGsv() - Find the specified value.

	MSOMETHOD(GetGsv) (THIS_ ULONG gsv, void *pv) PURE;

	// SetGsv() - Set the specified value.

	MSOMETHOD(SetGsv) (THIS_ ULONG gsv, void *pv) PURE;
};



#ifndef __cplusplus

///////////////////////////////////////////////////////////////////////////////
// IMsoFindFile

// ----------------------------------------------------------------------------
// IUnknown methods.
#define IMsoFindFile_QueryInterface(This, refiid, ppvObject) \
		(This)->lpVtbl->QueryInterface(This, refiid, ppvObject)

#define IMsoFindFile_AddRef(This) \
		(This)->lpVtbl->AddRef(This)

#define IMsoFindFile_Release(This) \
		(This)->lpVtbl->Release(This)

// ----------------------------------------------------------------------------
// Dialog behavior flags and settings.

#define IMsoFindFile_FSaveAs(This) \
		(This)->lpVtbl->FSaveAs(This)

#define IMsoFindFile_SetFSaveAs(This, fSaveAs) \
		(This)->lpVtbl->SetFSaveAs(This, fSaveAs)

#define IMsoFindFile_FSearchEnabled(This) \
		(This)->lpVtbl->FSearchEnabled(This)

#define IMsoFindFile_EnableSearch(This, fSearchEnabled) \
		(This)->lpVtbl->EnableSearch(This, fSearchEnabled)

#define IMsoFindFile_FAdvSearchEnabled(This) \
		(This)->lpVtbl->FAdvSearchEnabled(This)

#define IMsoFindFile_EnableAdvSearch(This, fAdvSearchEnabled) \
		(This)->lpVtbll->EnableAdvSearch(This, fAdvSearchEnabled)

#define IMsoFindFile_FPreviewEnabled(This) \
		(This)->lpVtbl->FPreviewEnabled(This)

#define IMsoFindFile_EnablePreview(This, fEnablePreview, pvInterface) \
		(This)->lpVtbl->EnablePreview(This, fEnablePreview,  pvInterface)

#define IMsoFindFile_FDirPicker(This) \
		(This)->lpVtbl->FDirPicker(This)

#define IMsoFindFile_SetFDirPicker(This, fDirPicker) \
		(This)->lpVtbl->SetFDirPicker(This, fDirPicker)

#define IMsoFindFile_FFileDirPicker(This) \
		(This)->lpVtbl->FFileDirPicker(This)

#define IMsoFindFile_SetFFileDirPicker(This, fFileDirPicker) \
		(This)->lpVtbl->SetFFileDirPicker(This, fFileDirPicker)

#define IMsoFindFile_FMultiSelect(This) \
		(This)->lpVtbl->FMultiSelect(This)

#define IMsoFindFile_SetFMultiSelect(This, fMultiSelect) \
		(This)->lpVtbl->SetFMultiSelect(This, fMultiSelect)

#define IMsoFindFile_FChangeDir(This) \
		(This)->lpVtbl->FChangeDir(This)

#define IMsoFindFile_SetFChangeDir(This, fChangeDir) \
		(This)->lpVtbl->SetFChangeDir(This, fChangeDir)

#define IMsoFindFile_GetFreezeDir(This, szDir, cb) \
		(This)->lpVtbl->GetFreezeDir(This, szDir, cb)

#define IMsoFindFile_SetFreezeDir(This, szDir) \
		(This)->lpVtbl->SetFreezeDir(This, szDir)

#define IMsoFindFile_FConfirmReplace(This)	\
		(This)->lpVtbl->FConfirmReplace(This)

#define IMsoFindFile_SetShortFileName(This, fSet)	\
		(This)->lpVtbl->SetShortFileName(This, fSet)

#define IMsoFindFile_SetFConfirmReplace(This, fConfirm) \
		(This)->lpVtbl->SetFConfirmReplace(This, fConfirm)

#define IMsoFindFile_GetDefaultFileName(This, szFile, cb) \
		(This)->lpVtbl->GetDefaultFileName(This, szFile, cb)

#define IMsoFindFile_SetDefaultFileName(This, szFile) \
		(This)->lpVtbl->SetDefaultFileName(This, szFile)

#define IMsoFindFile_FKeepSelectedType(This) \
		(This)->lpVtbl->FKeepSelectedType(This)

#define IMsoFindFile_SetFKeepSelectedType(This, fKeep) \
		(This)->lpVtbl->SetFKeepSelectedType(This, fKeep)

#define IMsoFindFile_FSysDirPicker(This) \
		(This)->lpVtbl->FSysDirPicker(This)

#define IMsoFindFile_SetFSysDirPicker(This, fSysDirPicker) \
		(This)->lpVtbl->SetFSysDirPicker(This, fSysDirPicker)

#define IMsoFindFile_EnableOpenInNativeApp(This, fEnable, fOpensUnknown, appidCurrentApp) \
		(This)->lpVtbl->EnableOpenInNativeApp(This, fEnable, fOpensUnknown, appidCurrentApp)

// ----------------------------------------------------------------------------
// Cosmetic dialog customization.

#define IMsoFindFile_GetAppName(This, szAppName) \
		(This)->lpVtbl->GetAppName(This, szAppName)

#define IMsoFindFile_SetAppName(This, szAppName) \
		(This)->lpVtbl->SetAppName(This, szAppName)

#define IMsoFindFile_GetDlgTitle(This, szTitle) \
		(This)->lpVtbl->GetDlgTitle(This, szTitle)

#define IMsoFindFile_SetDlgTitle(This, szTitle) \
		(This)->lpVtbl->SetDlgTitle(This, szTitle)

#define IMsoFindFile_GetXY(This, px, py) \
		(This)->lpVtbl->GetXY(This, px, py)

#define IMsoFindFile_SetXY(This, x, y) \
		(This)->lpVtbl->SetXY(This, x, y)

#define IMsoFindFile_GetCenterXY(This, x, y) \
		(This)->lpVtbl->GetCenterXY(This, x, y)

#define IMsoFindFile_SetCenterXY(This, x, y) \
		(This)->lpVtbl->SetCenterXY(This, x, y)

#define IMsoFindFile_SetView(This, v) \
		(This)->lpVtbl->SetView(This, v)

#define IMsoFindFile_GetView(This, pv) \
		(This)->lpVtbl->GetView(This, pv)

#define IMsoFindFile_SetFShowGroups(This, f) \
		(This)->lpVtbl->SetFShowGroups(This, f)

#define IMsoFindFile_FShowGroups(This) \
		(This)->lpVtbl->FShowGroups(This)

#define IMsoFindFile_SetSort(This, sf) \
		(This)->lpVtbl->SetSort(This, sf)

#define IMsoFindFile_GetSort(This, psf) \
		(This)->lpVtbl->GetSort(This, psf)

#define IMsoFindFile_ExecuteQuery(This) \
		(This)->lpVtbl->ExecuteQuery(This)

/* FMidEast */

/* FMidEast End */
#define IMsoFindFile_SetFPreserveQuotes(This, fPreserve) \
		(This)->lpVtbl->SetFPreserveQuotes(This, fPreserve)

#define IMsoFindFile_FPreserveQuotes(This) \
		(This)->lpVtbl->FPreserveQuotes(This)

#define IMsoFindFile_SetFOnlyFileSys(This, fOnlyFileSys) \
		(This)->lpVtbl->SetFOnlyFileSys(This, fOnlyFileSys)

#define IMsoFindFile_FOnlyFileSys(This) \
		(This)->lpVtbl->FOnlyFileSys(This)

#define IMsoFindFile_SetFResolveLinks(This, fResolveLinks) \
		(This)->lpVtbl->SetFResolveLinks(This, fResolveLinks)

#ifdef	VSMSODEBUG
#define IMsoFindFile_SetFNoLoadIcon(This, fNoLoadIcon) \
		(This)->lpVtbl->SetFNoLoadIcon(This, fNoLoadIcon)
#endif // VSMSODEBUG

#define IMsoFindFile_SetHmsoinst(This, hmsoinst) \
		(This)->lpVtbl->SetHmsoinst(This, hmsoinst)

#define IMsoFindFile_GetQueryErrorMessage(This, wzMsg) \
		(This)->lpVtbl->GetQueryErrorMessage(This, wzMsg)

#define IMsoFindFile_GetTotalHitsAvailable(This, pchitTotal) \
		(This)->lpVtbl->GetTotalHitsAvailable(This, pchitTotal)

#define IMsoFindFile_SetConfirmReplaceEx(This, msocr) \
		(This)->lpVtbl->SetConfirmReplaceEx(This, msocr)


// ----------------------------------------------------------------------------
// Save As Web Page control.

#define IMsoFindFile_EnableSaveAsWebOptions(This, iHtmlTypeFamily, layoutStyle) \
		(This)->lpVtbl->EnableSaveAsWebOptions(This, iHtmlTypeFamily, layoutStyle)

#define IMsoFindFile_DisableSaveAsWebOptions(This) \
		(This)->lpVtbl->DisableSaveAsWebOptions(This)

//#define IMsoFindFile_GetIHtmlTypeFamily(This, piHtmlTypeFamily) \
//		(This)->lpVtbl->GetIHtmlTypeFamily(This, piHtmlTypeFamily)

//#define IMsoFindFile_GetLayoutStyle(This, playoutStyle) \
//		(This)->lpVtbl->GetLayoutStyle(This, playoutStyle)

// ----------------------------------------------------------------------------
// Dialog behavior control.

#define IMsoFindFile_ShowDlg(This, picmd, hwnd) \
		(This)->lpVtbl->ShowDlg(This, picmd, hwnd)

#define IMsoFindFile_GetHwnd(This, hwnd) \
		(This)->lpVtbl->GetHwnd(This, hwnd)

// ----------------------------------------------------------------------------
// Events.

#define IMsoFindFile_RegisterPfnEvent(This, pfnEvent, wApp) \
		(This)->lpVtbl->RegisterPfnEvent(This, pfnEvent, wApp)

// ----------------------------------------------------------------------------
// Short-cut methods.

#define IMsoFindFile_GetOpenTitle(This, szTitle) \
		(This)->lpVtbl->GetOpenTitle(This, szTitle)

#define IMsoFindFile_SetOpenTitle(This, szTitle) \
		(This)->lpVtbl->SetOpenTitle(This, szTitle)

#define IMsoFindFile_GetIcntrlValueW(This, icntrl, pval) \
		(This)->lpVtbl->GetIcntrlValueW(This, icntrl, pval)

#define IMsoFindFile_GetIcntrlValueSz(This, icntrl, szVal, cbVal) \
		(This)->lpVtbl->GetIcntrlValueSz(This, icntrl, szVal, cbVal)

#define IMsoFindFile_SetIcntrlValueW(This, icntrl, val) \
		(This)->lpVtbl->SetIcntrlValueW(This, icntrl, val)

#define IMsoFindFile_SetIcntrlValueSz(This, icntrl, szVal) \
		(This)->lpVtbl->SetIcntrlValueSz(This, icntrl, szVal)

#define IMsoFindFile_FEnabledIcntrl(This, icntrl) \
		(This)->lpVtbl->FEnabledIcntrl(This, icntrl)

#define IMsoFindFile_EnableIcntrl(This, icntrl, fEnable) \
		(This)->lpVtbl->EnableIcntrl(This, icntrl, fEnable)

#define IMsoFindFile_SetFocusIcntrl(This, icntrl) \
		(This)->lpVtbl->SetFocusIcntrl(This, icntrl)

// ----------------------------------------------------------------------------
// Query specification short-cut methods.

#define IMsoFindFile_IszFileTypeGet(This) \
		(This)->lpVtbl->IszFileTypeGet(This)

#define IMsoFindFile_SetIszFileType(This, iszFileType) \
		(This)->lpVtbl->SetIszFileType(This, iszFileType)

#define IMsoFindFile_GetActiveIMsoSearch(This, ppsrch) \
		(This)->lpVtbl->GetActiveIMsoSearch(This, ppsrch)

#define IMsoFindFile_SetSearchPath(This, szPath) \
		(This)->lpVtbl->SetSearchPath(This, szPath)

#define IMsoFindFile_SetSearchPathPidl(This, ppidl, cpidl) \
		(This)->lpVtbl->SetSearchPathPidl(This, ppidl, cpidl)

// ----------------------------------------------------------------------------
// Collection methods.
#define IMsoFindFile_GetIMsoFileTypeList(This, ppfiltyplist) \
		(This)->lpVtbl->GetIMsoFileTypeList(This, ppfiltyplist)

#define IMsoFindFile_GetIMsoControlList(This, ppcstitmlist) \
		(This)->lpVtbl->GetIMsoControlList(This, ppcstitmlist)

#define IMsoFindFile_GetIMsoCommandList(This, ppcmdlist) \
		(This)->lpVtbl->GetIMsoCommandList(This, ppcmdlist)

// ----------------------------------------------------------------------------
// Memory check debug methods
#define IMsoFindFile_RegisterPfnMemCheck(This, pfnMemCheck) \
		(This)->lpVtbl->RegisterPfnMemCheck(This, pfnMemCheck)


#define IMsoFindFile_GetICodePageList(This, ppcodepagelist) \
		(This)->lpVtbl->GetICodePageList(This, ppcodepagelist)

/* FMidEast */

/* FMidEast End */

#define IMsoFindFile_SetIszFileTypePolicyDefault(This, iszFileType) \
		(This)->lpVtbl->SetIszFileTypePolicyDefault(This, iszFileType)

#define IMsoFindFile_GetIszFileTypePolicyDefault(This) \
		(This)->lpVtbl->GetIszFileTypePolicyDefault(This)

#define IMsoFindFile_ActivateDefaultSaveURLForTemplates(This, iwz) \
		(This)->lpVtbl->ActivateDefaultSaveURLForTemplates(This, iwz)

/* MetaData */
#define IMsoFindFile_SetPfnAppReadWriteProp(This, pfn, dwPrvData) \
		(This)->lpVtbl->SetPfnAppReadWriteProp(This, pfn, dwPrvData)

#define IMsoFindFile_FNoFormDialog(This)	\
		(This)->lpVtbl->FNoFormDialog(This)

#define IMsoFindFile_SetFNoFormDialog(This, fNoForm) \
		(This)->lpVtbl->SetFNoFormDialog(This, fNoForm)

#define IMsoFindFile_SetFNoWOW(This, fNoWOW) \
      (This)->lpVtbl->SetFNoWOW(This, fNoWOW)

#define IMsoFindFile_FNoWow(This) \
      (This)->lpVtbl->FNoWow(This)

#define IMsoFindFile_SetHaveFileNewDefaultPath(This) \
		(This)->lpVtbl->SetHaveFileNewDefaultPath(This)

#define IMsoFindFile_FOtherAppFiles(This, pstr) \
		(This)->lpVtbl->FOtherAppFiles(This, pstr)

#define IMsoFindFile_SetFWordMail(This, fWordMail) \
		(This)->lpVtbl->SetFWordMail(This, fWordMail)

///////////////////////////////////////////////////////////////////////////////
// IMsoSearch

// ----------------------------------------------------------------------------
// IMsoUnknown methods.

#define IMsoSearch_QueryInterface(This, refiid, ppvObject) \
		(This)->lpVtbl->QueryInterface(This, refiid, ppvObject)

#define IMsoSearch_AddRef(This) \
		(This)->lpVtbl->AddRef(This)

#define IMsoSearch_Release(This) \
		(This)->lpVtbl->Release(This)

// ----------------------------------------------------------------------------
// IMsoSearch methods.

#define IMsoSearch_GetSzFileName(This, szFileName, cbFileName) \
		(This)->lpVtbl->GetSzFileName(This, szFileName, cbFileName)

#define IMsoSearch_SetSzFileName(This, szFileName) \
		(This)->lpVtbl->SetSzFileName(This, szFileName)

#define IMsoSearch_Clear(This)	\
		(This)->lpVtbl->Clear(This)

#define IMsoSearch_FGetPropSzValMacro(This, stdprop, szPropVal, cb) \
		(This)->lpVtbl->FGetPropSzValMacro(This, stdprop, szPropVal, cb)

#define IMsoSearch_SetPropSzValMacro(This, stdprop, szPropVal, wzErrMsg, cchErrMsg, pichFirst, pichLim) \
		(This)->lpVtbl->SetPropSzValMacro(This, stdprop, szPropVal, wzErrMsg, cchErrMsg, pichFirst, pichLim)

#define IMsoSearch_GetIMsoFoundFileList(This, ppffilelist) \
		(This)->lpVtbl->GetIMsoFoundFileList(This, ppffilelist)

#define IMsoSearch_GetIMsoSelectedFileList(This, ppselfilelist) \
		(This)->lpVtbl->GetIMsoSelectedFileList(This, ppselfilelist)

#define IMsoSearch_SetSearchPath(This, szPath) \
		(This)->lpVtbl->SetSearchPath(This, szPath)

#define IMsoSearch_GetSearchPath(This, szPath, cbMax) \
		(This)->lpVtbl->GetSearchPath(This, szPath, cbMax)

#define IMsoSearch_GetName(This, szPath, cbMax) \
		(This)->lpVtbl->GetName(This, szPath, cbMax)

#define IMsoSearch_SaveAs(This, szPath) \
		(This)->lpVtbl->SaveAs(This, szPath)

#define IMsoSearch_Load(This, szPath) \
		(This)->lpVtbl->Load(This, szPath)

#define IMsoSearch_Delete(This) \
		(This)->lpVtbl->Delete(This)

#define IMsoSearch_SetSearchConnector(This, msodmconn) \
		(This)->lpVtbl->SetSearchConnector(This, msodmconn)

#define IMsoSearch_SetSearchPathEx(This, szPath, msospf) \
		(This)->lpVtbl->SetSearchPathEx(This, szPath, msospf)

#define IMsoSearch_SetSearchPathPidl(This, ppidl, cpidl, msospf) \
		(This)->lpVtbl->SetSearchPathPidl(This, ppidl, cpidl, msospf)


///////////////////////////////////////////////////////////////////////////////
// IMsoFoundFileList

// ----------------------------------------------------------------------------
// IMsoUnknown methods.

#define IMsoFoundFileList_QueryInterface(This, refiid, ppvObject) \
		(This)->lpVtbl->QueryInterface(This, refiid, ppvObject)

#define IMsoFoundFileList_AddRef(This) \
		(This)->lpVtbl->AddRef(This)

#define IMsoFoundFileList_Release(This) \
		(This)->lpVtbl->Release(This)

// ----------------------------------------------------------------------------
// IMsoFoundFileList methods.

#define IMsoFoundFileList_CffGet(This) \
		(This)->lpVtbl->CffGet(This)

#define IMsoFoundFileList_GetIMsoFoundFile(This, iff, ppffile) \
		(This)->lpVtbl->GetIMsoFoundFile(This, iff, ppffile)

#define IMsoFoundFileList_GetIMsoOLDocumentI(This, ppIOLDocument, iFile) \
		(This)->lpVtbl->GetIMsoOLDocumentI(This, ppIOLDocument, iFile)

#define IMsoFoundFileList_GetPropI(This, iff, prop, var) \
		(This)->lpVtbl->GetPropI(This, iff, prop, var)

#define IMsoFoundFileList_GetPropISz(This, iff, prop, psz, cb) \
		(This)->lpVtbl->GetPropI(This, iff, prop, psz, cb)


///////////////////////////////////////////////////////////////////////////////
// IMsoSelectedFileList

// ----------------------------------------------------------------------------
// IMsoUnknown methods.

#define IMsoSelectedFileList_QueryInterface(This, refiid, ppvObject) \
		(This)->lpVtbl->QueryInterface(This, refiid, ppvObject)

#define IMsoSelectedFileList_AddRef(This) \
		(This)->lpVtbl->AddRef(This)

#define IMsoSelectedFileList_Release(This) \
		(This)->lpVtbl->Release(This)

// ----------------------------------------------------------------------------
// IMsoSelectedFileList methods.

#define IMsoSelectedFileList_CffGet(This) \
		(This)->lpVtbl->CffGet(This)

#define IMsoSelectedFileList_GetIMsoFoundFile(This, iff, ppffile) \
		(This)->lpVtbl->GetIMsoFoundFile(This, iff, ppffile)

#define IMsoSelectedFileList_GetIMsoOLDocumentI(This, ppIOLDocument, iFile) \
		(This)->lpVtbl->GetIMsoOLDocumentI(This, ppIOLDocument, iFile)


///////////////////////////////////////////////////////////////////////////////
// IMsoFoundFile

// ----------------------------------------------------------------------------
// IMsoUnknown methods.

#define IMsoFoundFile_QueryInterface(This, refiid, ppvObject) \
		(This)->lpVtbl->QueryInterface(This, refiid, ppvObject)

#define IMsoFoundFile_AddRef(This) \
		(This)->lpVtbl->AddRef(This)

#define IMsoFoundFile_Release(This) \
		(This)->lpVtbl->Release(This)

// ----------------------------------------------------------------------------
// IMsoFoundFile methods.

#define IMsoFoundFile_GetPath(This, szPath, cbPath) \
		(This)->lpVtbl->GetPath(This, szPath, cbPath)

#define IMsoFoundFile_GetIfile(This, pifile) \
		(This)->lpVtbl->GetIfile(This, pifile)

#define IMsoFoundFile_GetIMsoOLDocument(This, ppIOLDocument) \
		(This)->lpVtbl->GetIMsoOLDocument(This, ppIOLDocument)


///////////////////////////////////////////////////////////////////////////////
// IMsoDMControl

// ----------------------------------------------------------------------------
// IMsoUnknown methods.

#define IMsoDMControl_QueryInterface(This, refiid, ppvObject) \
		(This)->lpVtbl->QueryInterface(This, refiid, ppvObject)

#define IMsoDMControl_AddRef(This) \
		(This)->lpVtbl->AddRef(This)

#define IMsoDMControl_Release(This) \
		(This)->lpVtbl->Release(This)

// ----------------------------------------------------------------------------
// IMsoDMControl methods.

#define IMsoDMControl_Otp(This) \
		(This)->lpVtbl->Otp(This)

#define IMsoDMControl_SetOtp(This, otp) \
		(This)->lpVtbl->SetOtp(This, otp)

#define IMsoDMControl_GetSzTitle(This, szTitle) \
		(This)->lpVtbl->GetSzTitle(This, szTitle)

#define IMsoDMControl_SetSzTitle(This, szTitle) \
		(This)->lpVtbl->SetSzTitle(This, szTitle)

#define IMsoDMControl_FEnabled(This) \
		(This)->lpVtbl->FEnabled(This)

#define IMsoDMControl_Enable(This, fEnable) \
		(This)->lpVtbl->Enable(This, fEnable)

#define IMsoDMControl_GetValueW(This, pval) \
		(This)->lpVtbl->GetValueW(This, pval)

#define IMsoDMControl_GetValueSz(This, szVal, cbVal) \
		(This)->lpVtbl->GetValueSz(This, szVal, cbVal)

#define IMsoDMControl_SetValueW(This, val) \
		(This)->lpVtbl->SetValueW(This, val)

#define IMsoDMControl_SetValueSz(This, szVal) \
		(This)->lpVtbl->SetValueSz(This, szVal)

#define IMsoDMControl_SetValueSzEx(This, wzVal, wzBtnText, wzDlgTitle, wzDescrText) \
		(This)->lpVtbl->SetValueSzEx(This, wzVal, wzBtnText, wzDlgTitle, wzDescrText)

#define IMsoDMControl_SetFEnableWithOK(This, fEnableWithOKButton) \
		(This)->lpVtbl->SetFEnableWithOK(This, fEnableWithOKButton)

#define IMsoDMControl_FEnabledWithOK(This) \
		(This)->lpVtbl->FEnabledWithOK(This)

#define IMsoDMControl_SetFDisablesOKDropDown(This, fDisablesOKDropDown) \
		(This)->lpVtbl->SetFDisablesOKDropDown(This, fDisablesOKDropDown)

#define IMsoDMControl_FDisablesOKDropDown(This) \
		(This)->lpVtbl->FDisablesOKDropDown(This)

#define IMsoDMControl_SetAltOpenTitle(This, wzTitle) \
		(This)->lpVtbl->SetAltOpenTitle(This, wzTitle)

#define IMsoDMControl_GetAltOpenTitle(This, wzTitle) \
		(This)->lpVtbl->GetAltOpenTitle(This, wzTitle)

///////////////////////////////////////////////////////////////////////////////
// IMsoControlList

// ----------------------------------------------------------------------------
// IMsoUnknown methods.

#define IMsoControlList_QueryInterface(This, refiid, ppvObject) \
		(This)->lpVtbl->QueryInterface(This, refiid, ppvObject)

#define IMsoControlList_AddRef(This) \
		(This)->lpVtbl->AddRef(This)

#define IMsoControlList_Release(This) \
		(This)->lpVtbl->Release(This)

// ----------------------------------------------------------------------------
// IMsoControlList methods.

#define IMsoControlList_CcntrlGet(This) \
		(This)->lpVtbl->CcntrlGet(This)

#define IMsoControlList_Get(This, icstitm, ppcstitm) \
		(This)->lpVtbl->Get(This, icstitm, ppcstitm)

#define IMsoControlList_Append(This, pcstitm) \
		(This)->lpVtbl->Append(This, pcstitm)

#define IMsoControlList_Delete(This, icstitm) \
		(This)->lpVtbl->Delete(This, icstitm)

#define IMsoControlList_IcntrlOfHid(This, hid) \
		(This)->lpVtbl->IcntrlOfHid(This, hid)


///////////////////////////////////////////////////////////////////////////////
// IMsoFileTypeList

// ----------------------------------------------------------------------------
// IMsoUnknown methods.

#define IMsoFileTypeList_QueryInterface(This, refiid, ppvObject) \
		(This)->lpVtbl->QueryInterface(This, refiid, ppvObject)

#define IMsoFileTypeList_AddRef(This) \
		(This)->lpVtbl->AddRef(This)

#define IMsoFileTypeList_Release(This) \
		(This)->lpVtbl->Release(This)

// ----------------------------------------------------------------------------
// IMsoFileTypeList methods.

#define IMsoFileTypeList_CszGet(This) \
		(This)->lpVtbl->CszGet(This)

#define IMsoFileTypeList_Get(This, iszType, szType, pfFreezeDir) \
		(This)->lpVtbl->Get(This, iszType, szType, pfFreezeDir)

#define IMsoFileTypeList_Append(This, szType, fFreezeDir) \
		(This)->lpVtbl->Append(This, szType, fFreezeDir)

#define IMsoFileTypeList_Delete(This, iszType) \
		(This)->lpVtbl->Delete(This, iszType)

#define IMsoFileTypeList_IszDefaultGet(This) \
		(This)->lpVtbl->IszDefaultGet(This)

#define IMsoFileTypeList_SetIszDefault(This, iszDefault) \
		(This)->lpVtbl->SetIszDefault(This, iszDefault)

#define IMsoFileTypeList_AppendEx(This, szType, msoftf) \
		(This)->lpVtbl->AppendEx(This, szType, msoftf)


///////////////////////////////////////////////////////////////////////////////
// IMsoCodePageList

// ----------------------------------------------------------------------------
// IMsoUnknown methods.

#define IMsoCodePageList_QueryInterface(This, refiid, ppvObject) \
		(This)->lpVtbl->QueryInterface(This, refiid, ppvObject)

#define IMsoCodePageList_AddRef(This) \
		(This)->lpVtbl->AddRef(This)

#define IMsoCodePageList_Release(This) \
		(This)->lpVtbl->Release(This)

// ----------------------------------------------------------------------------
// IMsoCodePageList methods.

#define IMsoCodePageList_CszGet(This) \
		(This)->lpVtbl->CszGet(This)

#define IMsoCodePageList_Get(This, iszCodePage, szCodePage) \
		(This)->lpVtbl->Get(This, iszCodePage, szCodePage)

#define IMsoCodePageList_Append(This, szCodePage) \
		(This)->lpVtbl->Append(This, szCodePage)

#define IMsoCodePageList_Delete(This, iszCodePage) \
		(This)->lpVtbl->Delete(This, iszCodePage)

#define IMsoCodePageList_IszDefaultGet(This) \
		(This)->lpVtbl->IszDefaultGet(This)

#define IMsoCodePageList_SetIszDefault(This, iszDefault) \
		(This)->lpVtbl->SetIszDefault(This, iszDefault)

///////////////////////////////////////////////////////////////////////////////
// IMsoCommandList

// ----------------------------------------------------------------------------
// IMsoUnknown methods.

#define IMsoCommandList_QueryInterface(This, refiid, ppvObject)	\
		(This)->lpVtbl->QueryInterface(This, refiid, ppvObject)

#define IMsoCommandList_AddRef(This) \
		(This)->lpVtbl->AddRef(This)

#define IMsoCommandList_Release(This) \
		(This)->lpVtbl->Release(This)


// ----------------------------------------------------------------------------
// IMsoCommandList methods.
#define IMsoCommandList_CcmdGet(This)	\
		(This)->lpVtbl->CcmdGet(This)

#define IMsoCommandList_Get(This, icmd, szCmd) \
		(This)->lpVtbl->Get(This, icmd, szCmd)

#define IMsoCommandList_GetFEnabled(This, icmd, pfEnabled) \
		(This)->lpVtbl->GetFEnabled(This, icmd, pfEnabled)

#define IMsoCommandList_SetFEnabled(This, icmd, fEnabled) \
		(This)->lpVtbl->SetFEnabled(This, icmd, fEnabled)

#define IMsoCommandList_GetFVisible(This, icmd, pfVisible) \
		(This)->lpVtbl->GetFVisible(This, icmd, pfVisible)

#define IMsoCommandList_SetFVisible(This, icmd, fVisible) \
		(This)->lpVtbl->SetFVisible(This, icmd, fVisible)


///////////////////////////////////////////////////////////////////////////////
// IMsoOLDocument

#define IMsoOLDocument_QueryInterface(This, refiid, ppvObject) \
		(This)->lpVtbl->QueryInterface(This, refiid, ppvObject)

#define IMsoOLDocument_AddRef(This) \
		(This)->lpVtbl->AddRef(This)

#define IMsoOLDocument_Release(This) \
		(This)->lpVtbl->Release(This)

#define IMsoOLDocument_GetInterface(This, ppIUnknown, refiid) \
		(This)->lpVtbl->GetInterface(This, ppIUnknown, refiid)

#define IMsoOLDocument_SetInterface(This, pIUnknown, refiid) \
		(This)->lpVtbl->SetInterface(This, pIUnknown, refiid)

#define IMsoOLDocument_GetPwTdi(This, pw, tdi) \
		(This)->lpVtbl->GetPwTdi(This, pw, tdi)

#define IMsoOLDocument_SetWTdi(This, w, tdi) \
		(This)->lpVtbl->SetWTdi(This, w, tdi)

#define IMsoOLDocument_GetWzPcchGdn(This, wz, pcch, gdn) \
		(This)->lpVtbl->GetWzPcchGdn(This, wz, pcch, gdn)

#define IMsoOLDocument_SetWzGdn(This, wz, gdn) \
		(This)->lpVtbl->SetWzGdn(This, wz, gdn)

#define IMsoOLDocument_SetAttrInAttrMask(This, attr, attrMask) \
		(This)->lpVtbl->SetAttrInAttrMask(This, attr, attrMask)

#define IMsoOLDocument_AttrGet(This) \
		(This)->lpVtbl->AttrGet(This)

#define IMsoOLDocument_BeginCmd(This, cmd, ppIOLDocumentOther) \
		(This)->lpVtbl->BeginCmd(This, cmd, ppIOLDocumentOther)

#define IMsoOLDocument_RecordEvent(This, evt, pvMisc, refId) \
		(This)->lpVtbl->RecordEvent(This, evt, pvMisc, refId)

#define IMsoOLDocument_GetFSSpec(This, pFSSpec) \
		(This)->lpVtbl->GetFSSpec(This, pFSSpec)

#define IMsoOLDocument_GetPguidTogI(This, pguid, tog, i) \
		(This)->lpVtbl->GetPguidTogI(This, pguid, tog, i)

#define IMsoOLDocument_SetPguidTogI(This, pguid, tog, i) \
		(This)->lpVtbl->SetPguidTogI(This, pguid, tog, i)

#define IMsoOLDocument_DoIOLDocPguidOp(This, ppIOLDocOther, pguid, op, ul) \
		(This)->lpVtbl->DoIOLDocPguidOp(This, ppIOLDocOther, pguid, op, ul)

#define IMsoOLDocument_SetRenSummInfo(This, lpsiobj, lpdsiobj, lpudobj) \
		(This)->lpVtbl->SetRenSummInfo(This, lpsiobj, lpdsiobj, lpudobj)

#define IMsoOLDocument_SetDocumentCookie(This, cookie) \
		(This)->lpVtbl->SetDocumentCookie(This, cookie)

#define IMsoOLDocument_GetDocumentCookie(This) \
		(This)->lpVtbl->GetDocumentCookie(This)

#define IMsoOLDocument_GetPKMClient(This, pkmclient) \
		(This)->lpVtbl->GetPKMClient(This, pkmclient)

#define IMsoOLDocument_SetPKMClient(This, pkmclient) \
		(This)->lpVtbl->SetPKMClient(This, pkmclient)

#define IMsoOLDocument_GetCOpen(This, pcOpen) \
		(This)->lpVtbl->GetCOpen(This, pcOpen)

#if !defined(OFFICE_BUILD) && !defined(__cplusplus)

__inline IMsoOLDocument_GetFilePath (
		 IMsoOLDocument *piolDoc,
		 interface IBindCtx *pibc, interface IBindStatusCallback *pibsc,
		 WCHAR *wzPath, ULONG *pcbPath)
	{
	HRESULT hr = (pcbPath == 0) ? E_POINTER : S_OK;

	if (wzPath != 0)
		*wzPath = 0;

	if ((SUCCEEDED(hr)) && (pibc != 0))
		hr = piolDoc->lpVtbl->SetInterface(piolDoc, (IUnknown *) pibc, &IID_IBindCtx);
	if ((SUCCEEDED(hr)) && (pibsc != 0))
		hr = piolDoc->lpVtbl->SetInterface(piolDoc, (IUnknown *) pibsc, &IID_IBindStatusCallback);

	if (SUCCEEDED(hr))
		{
		ULONG cch = *pcbPath / sizeof(WCHAR);
		hr = piolDoc->lpVtbl->GetWzPcchGdn(piolDoc, wzPath, &cch, msoiolgdnTempFilePath);
		*pcbPath = cch * sizeof(WCHAR);
		}

	return hr;
	}

__inline IMsoOLDocument_GetDisplayName (IMsoOLDocument *piolDoc,
		WCHAR *wzDisplayName, ULONG *pcbDisplayName, MSOIOLGDN gdn)
	{
	HRESULT hr;

	if (pcbDisplayName == 0)
		{
		if (wzDisplayName != 0)
			*wzDisplayName = 0;
		hr = E_POINTER;
		}
	else
		{
		ULONG cch = *pcbDisplayName / sizeof(WCHAR);
		hr = piolDoc->lpVtbl->GetWzPcchGdn(piolDoc, wzDisplayName, &cch, gdn);
		*pcbDisplayName = cch * sizeof(WCHAR);
		}

	return hr;
	}

__inline IMsoOLDocument_GetOWSStgError (IMsoOLDocument *piolDoc)
	{
	HRESULT hr;
	piolDoc->lpVtbl->GetPwTdi(piolDoc, (int *)&hr, msoioltdiOWSStgError);
	return hr;
	}

__inline IMsoOLDocument_GetOWSLowDateTime (IMsoOLDocument *piolDoc)
	{
	DWORD dw;
	piolDoc->lpVtbl->GetPwTdi(piolDoc, (int *)&dw, msoioltdiOWSLowDateTime);
	return dw;
	}

__inline IMsoOLDocument_GetOWSHighDateTime (IMsoOLDocument *piolDoc)
	{
	DWORD dw;
	piolDoc->lpVtbl->GetPwTdi(piolDoc, (int *)&dw, msoioltdiOWSHighDateTime);
	return dw;
	}

__inline IMsoOLDocument_GetIReDownload (IMsoOLDocument *piolDoc)
	{
	int i;
	piolDoc->lpVtbl->GetPwTdi(piolDoc, (int *)&i, msoioltdiReDownload);
	return i;
	}

__inline IMsoOLDocument_SetIReDownload (IMsoOLDocument *piolDoc, int i)
	{
	return piolDoc->lpVtbl->SetWTdi(piolDoc, (DWORD)i, msoioltdiReDownload);
	}

__inline IMsoOLDocument_GetIRedirected (IMsoOLDocument *piolDoc)
	{
	int i;
	piolDoc->lpVtbl->GetPwTdi(piolDoc, &i, msoioltdiRedirected);
	return i;
	}

__inline IMsoOLDocument_SetIEnableRedirect(IMsoOLDocument *piolDoc, int i)
	{
	return piolDoc->lpVtbl->SetWTdi(piolDoc, i, msoioltdiEnableRedirect);
	}

__inline IMsoOLDocument_GetIEnableRedirect(IMsoOLDocument *piolDoc)
	{
	int i;
	piolDoc->lpVtbl->GetPwTdi(piolDoc, &i, msoioltdiEnableRedirect);
	return i;
	}

__inline IMsoOLDocument_IsInFileSys (IMsoOLDocument *piolDoc)
	{
	if ((piolDoc->lpVtbl->AttrGet(piolDoc) & msoiolattrInFileSys) != 0)
		return S_OK;
	else
		return S_FALSE;
	}

__inline IMsoOLDocument_IsANewDocument (IMsoOLDocument *piolDoc)
	{
	if ((piolDoc->lpVtbl->AttrGet(piolDoc) & msoiolattrNewDocument) != 0)
		return S_OK;
	else
		return S_FALSE;
	}

__inline IMsoOLDocument_GetLoggingState (IMsoOLDocument *piolDoc) CONST_METHOD_FF
	{
	if ((piolDoc->lpVtbl->AttrGet(piolDoc) & msoiolattrRenLogging) != 0)
		return MSO_S_IOLELS_REN_LOGGING;
	else
		return MSO_S_IOLELS_NO_LOGGING;
	}

__inline IMsoOLDocument_SetElsLoggingState (IMsoOLDocument *piolDoc,
		MSOIOLELS elsLoggingState)
	{
	if (elsLoggingState == msoiolelsRenLogging)
		return piolDoc->lpVtbl->SetAttrInAttrMask(piolDoc, msoiolattrRenLogging, msoiolattrRenLogging);
	else
		return piolDoc->lpVtbl->SetAttrInAttrMask(piolDoc, 0, msoiolattrRenLogging);
	}

__inline IMsoOLDocument_GetMoniker (IMsoOLDocument *piolDoc,
		IMoniker **ppIMoniker)
	{ return piolDoc->lpVtbl->GetInterface(piolDoc, (IUnknown **) ppIMoniker, &IID_IMoniker); }

__inline IMsoOLDocument_GetIHtml (IMsoOLDocument *piolDoc)
	{
	int i;
	piolDoc->lpVtbl->GetPwTdi(piolDoc, &i, msoioltdiHtml);
	return i;
	}

__inline IMsoOLDocument_GetIMHtml (IMsoOLDocument *piolDoc)
	{
	int i;
	piolDoc->lpVtbl->GetPwTdi(piolDoc, &i, msoioltdiMHtml);
	return i;
	}

__inline IMsoOLDocument_GetIXml (IMsoOLDocument *piolDoc)
	{
	int i;
	piolDoc->lpVtbl->GetPwTdi(piolDoc, &i, msoioltdiXml);
	return i;
	}

__inline IMsoOLDocument_GetISniffed (IMsoOLDocument *piolDoc)
	{
	int i;
	piolDoc->lpVtbl->GetPwTdi(piolDoc, &i, msoioltdiSniffed);
	return i;
	}

__inline IMsoOLDocument_GetIUseDefaultFolderSuffix(IMsoOLDocument *piolDoc)
	{
	int i;
	piolDoc->lpVtbl->GetPwTdi(piolDoc, &i, msoioltdiUseDefaultFolderSuffix);
	return i;
	}

__inline IMsoOLDocument_SetIUseDefaultFolderSuffix(IMsoOLDocument *piolDoc, int i)
	{
	return piolDoc->lpVtbl->SetWTdi(piolDoc, i, msoioltdiUseDefaultFolderSuffix);
	}
#endif // !defined(OFFICE_BUILD) && !defined(__cplusplus)

MSOAPI_(HRESULT) MsoHrPubmonParseDisplayName(
	LPBC pibc,
	LPCWSTR pwzDisplayName,
	IMoniker ** ppimk);



///////////////////////////////////////////////////////////////////////////////
// IMsoGSV

// ----------------------------------------------------------------------------
// IMsoUnknown methods.
#define IMsoGSV_QueryInterface(This, refiid, ppvObject) \
		(This)->lpVtbl->QueryInterface(This, refiid, ppvObject)

#define IMsoGSV_AddRef(This) \
		(This)->lpVtbl->AddRef(This)

#define IMsoGSV_Release(This) \
		(This)->lpVtbl->Release(This)

// ----------------------------------------------------------------------------
// IMsoGSV methods.

#define IMsoGSV_GetGsv(This, gsv, pv) \
		(This)->lpVtbl->GetGsv(This, gsv, pv)

#define IMsoGSV_SetGsv(This, gsv, pv) \
		(This)->lpVtbl->SetGsv(This, gsv, pv)


#endif // !__cplusplus

//BUG:CHINTANM this is ifndef cplusplus only need it for everyone. email TUNAE when ready
MSOAPI_(HRESULT) MsoGetIMsoDocumentNotifyListI(IMsoDocumentNotifyList * * ppIMsoDocumentNotifyList, PFNDOCNOTIFYCALLBACK pfnDocNotifyCallback);


// Tri-state for desire/ability for app to do preview
typedef enum _MSODesirePreview
	{
	msodmyesPreview,  // App desires to do the preview
	msodmnoPreview,   // App doesn't want to do the preview
	msodmcanPreview,  // App can do the preview if DM cannot
	msodmnowPreview,  // App wants to do the preview immediately (do not wait
							// for idle time)
	} MSODesirePreview;

///////////////////////////////////////////////////////////////////////////////
//
// Class used by applications to define the interface for app driven preview
//
#undef INTERFACE
#define INTERFACE IMsoAppPreview
DECLARE_INTERFACE(IMsoAppPreview)
	{
	// ------------------------------------------------------------------------
	// Begin Interface
	//
	// This is important on the Mac.
	BEGIN_MSOINTERFACE

#ifdef NOTUSED
	MSOMETHOD(QueryInterface) (THIS_ REFIID refiid, void * * ppvObject) PURE;
	MSOMETHOD_(ULONG, AddRef) (THIS) PURE;
	MSOMETHOD_(ULONG, Release) (THIS) PURE;
#endif // NOTUSED
	MSOMETHOD_(MSOBOOL, FEnableZoom) (THIS_ const WCHAR * szFile) PURE;
	MSOMETHOD_(MSODesirePreview, DesirePreview) (THIS_ const WCHAR * szFile) PURE;
	MSOMETHOD_(MSOBOOL, FOpenFile) (THIS_ const WCHAR *szFile) PURE;
	MSOMETHOD_(void, CloseFile) (THIS) PURE;
	MSOMETHOD_(unsigned int, WPictureProc)(THIS_
				UINT_PTR	tmm,
				void *			psdmp,
				UINT_PTR	filler1,
				UINT_PTR	filler2,
				UINT_PTR	tmc,
				UINT_PTR	wParam) PURE;
	MSOMETHOD_(MSOBOOL, FEnableStream) (THIS) PURE; //Return TRUE if you want FOpenStream instead of FOpenFile().
	                                                //Currently only used by themes.
	MSOMETHOD_(MSOBOOL, FOpenStream) (THIS_ const IStream *istm) PURE;
	};

// Debug only API for setting debug options.
MSOAPI_(HRESULT) MsoMemoryDebugDlg();

// Debug only API for seeing alerts.
MSOAPI_(HRESULT) MsoAlertDebugDlg();

#ifdef VSMSODEBUG

// Debug only API for dumping indexes.
// sIndexDump points the custom index dump specification. If it is NULL,
// entire index is dumped out by default
MSOAPI_(BOOL) MsoDumpIndex(WCHAR * wszIndex, MsoIndexDump *psIndexDump);
#else
#endif // VSMSODEBUG

// Help API
// UNDONE: this should be moved to offcapi.h since it is Office wide
#ifdef __cplusplus
extern "C" {
#endif // __cplusplus

// Type of function that is invoked as a help filter.  This function
// is called from Office File dialogs when help is requested.  hid has
// the context ID.  The app returns fTrue if it wants to handle this hid,
// fFalse if it wants office to handle it; alternatively, you can use the
// following defines:
//
//	MSOOFFHELP_YOU_DO_IT		Office should handle it.
//	MSOOFFHELP_I_DID_IT		I handled it.
//
// Prior to putting up the dialog (or at any time when it is up), call
// OfficeSetHelpFilter with a pointer to the function you want to filter
// help requests.
//

#define	MSOOFFHELP_YOU_DO_IT			0
#define	MSOOFFHELP_I_DID_IT			1

typedef int (OFFOPEN_CALLBACK *PFNFOFFHELPFILT) (void *pvHelp,
		unsigned long hid);
void OFFOPEN_CALLBACK OfficeSetHelpFilter(PFNFOFFHELPFILT pfnHelp,
		void *pvHelp);

//	The following section defines symbols pertaining to the word morphology
//	databased used by the Word group.

#define	prtNS			0	// These are parts of speech as returned in the
#define	prtJJ			1	//  "pdwPart" parameter passed to some of the
#define	prtRB			2	//  following calls.
#define	prtNP			3
#define	prtINF			4
#define	prtED_EN		5
#define	prtING			6
#define	prt3PS			7
#define	prtJJER			8
#define	prtJJEST		9
#define	prtNS_NP		10
#define	prtRBER			11
#define	prtRBEST		12
#define	prtEN			13
#define	prtED			14
#define	prtINF_ED		15
#define	prtINF_EN		16
#define	prtINF_ED_EN	17
#define	prtNOM			18
#define	prtOBJ			19
#define	prtPOSS1		20
#define	prtPOSS2		21
#define	prtREFL			22
#define	prtPOSS1_POSS2	23
#define	prtNOM_OBJ		24
#define	prt3PS_ED		25
#define	prtINF_3PS		26
#define	prtEMPTY		27
#define	prtBED_BEN		28
#define	prtBING			29
#define	prtN3PS			30
#define	prtNINF			31
#define	prtNED			32
#define	prtOBJ_POSS1	33
#define	prtPOSS2_POSS1	34
#define	prtBNP			35
#define	prtING_ED_EN	36
#define	prtN3PS_NED		37

typedef VOID * PMORPH;

MSOAPI_(PMORPH) PmorphOpenMorph(
	const WCHAR *pwzFilename, LCID lcid);

MSOAPI_(BOOL) FGetFirstRingEntry(
	PMORPH pmorph,
	BYTE * szWord,
	DWORD * pdwPart);

MSOAPI_(BOOL) FGetLemma(
	PMORPH pmorph,
	BYTE * szWord,
	DWORD * pdwPart);

MSOAPIXX_(BOOL) FGetNextRingElement(
	PMORPH pmorph,
	BYTE * szWord,
	DWORD * pdwPart);

MSOAPI_(BOOL) FGetNextRingEntry(
	PMORPH pmorph,
	DWORD * pdwPart);

MSOAPI_(void) CloseMorph(
	PMORPH pmorph);

// Retrieves a search object for VBE.
MSOAPI_(BOOL) MsoFGetFFSearch(
	HMSOINST hinst,
	VARIANT *pvarg);

MSOAPI_(BOOL) MsoFGetOAFileDialog(
	HMSOINST pinst,
	MsoFileDialogType type,
	HWND hwnd,
	WCHAR **rgwzFilters,
	int cFilters,
	IMsoFileTypeList **ppfiletypelist,
	PFNFDAACTIONFUNCTION pfnExecute,
	IDispatch** ppiDispatch);

// This API allows Test Wizard to get the Open Dialog Look In dropdown strings.
// Contact: James Rodrigues
MSOAPIX_(void) TW_ListNameFOR_TESTING_ONLY(
	WCHAR *wzBuf,
	WCHAR *pDisplayName);

MSOAPIX_(HRESULT) MsoHrHttpUrlToPsfPidl(
	 const WCHAR *pwzUrl,
	 LPSHELLFOLDER *ppsf,
	 LPITEMIDLIST *ppidl,
	 LPITEMIDLIST *ppidlParent,
	 ULONG *pcchEaten,
	 BOOL fAllowNoExist,
	 BOOL fRelativePath);

// Computes a 32-bit CRC for the given byte sequence...
MSOAPI_(ULONG) MsoCrc32Compute(ULONG ulCrc, const BYTE *pbData, int cbData);

// Returns the 32-bit CRC for the given collab path...
MSOAPI_(ULONG) MsoCrc32ComputeForCollab(const WCHAR *wzPath);

int MsoNetHoodResolveLink(const WCHAR *wzLink, WCHAR *wzTarget);
int MsoGetNetHoodShellFolderPath(WCHAR *wzPath, LPCITEMIDLIST pidl);
void MsoGetFTPLocationsString(WCHAR *wzFTPLocation);
int MsoGetShellFolderPath(WCHAR *wzPath, LPCITEMIDLIST pidl);

MSOAPI_(HRESULT) MsoHrPSFCreateIOLDocFromPersistent(LPWSTR lpwzPersistent, IMsoOLDocument **ppioldoc);
MSOAPI_(void) MsoGetFilenameCurFindFileDlg(WCHAR *xsz);

MSOAPI_(BOOL) MsoFHidePathsInAlerts(void);
MSOAPI_(BOOL) MsoFSanitizePath(const WCHAR *wzPath, WCHAR *wzOut, int cchWzOut);
MSOAPI_(BOOL) MsoFFileDialogUp(void);

///////////////////////////////////////////////////////////////////////////////
// IMsoAsyncStreeam
//

#undef INTERFACE
#define INTERFACE IMsoAsyncStream

DECLARE_INTERFACE_(IMsoAsyncStream, IStream)
{
	// ------------------------------------------------------------------------
	// Begin Interface
	//
	// This is important on the Mac.
	BEGIN_MSOINTERFACE

	// ------------------------------------------------------------------------
	// IUnknown Methods

	MSOMETHOD(QueryInterface) (THIS_ REFIID refiid, void * * ppvObject) PURE;
	MSOMETHOD_(ULONG, AddRef) (THIS) PURE;
	MSOMETHOD_(ULONG, Release) (THIS) PURE;

	// ------------------------------------------------------------------------
	// IStream methods.
	MSOMETHOD(Read) (THIS_ void *pv, ULONG cb, ULONG *pcbRead) PURE;
	MSOMETHOD(Write) (THIS_ const void *pv, ULONG cb, ULONG *pcbWritten) PURE;
	MSOMETHOD(Seek) (THIS_ LARGE_INTEGER dlibMove, DWORD dwOrigin,
			ULARGE_INTEGER *plibNewPosition) PURE;
	MSOMETHOD(SetSize) (THIS_ ULARGE_INTEGER libNewSize) PURE;
	MSOMETHOD(CopyTo) (THIS_ IStream *pstm, ULARGE_INTEGER cb,
			ULARGE_INTEGER *pcbRead, ULARGE_INTEGER *pcbWritten) PURE;
	MSOMETHOD(Commit) (THIS_ DWORD grfCommitFlags) PURE;
	MSOMETHOD(Revert) (THIS) PURE;
	MSOMETHOD(LockRegion) (THIS_ ULARGE_INTEGER libOffset, ULARGE_INTEGER cb,
			DWORD dwLockType) PURE;
	MSOMETHOD(UnlockRegion) (THIS_ ULARGE_INTEGER libOffset, ULARGE_INTEGER cb,
			DWORD dwLockType) PURE;
	MSOMETHOD(Stat) (THIS_ STATSTG *pstatstg, DWORD grfStatFlag) PURE;
	MSOMETHOD(Clone) (THIS_ IStream **ppstm) PURE;

	// ------------------------------------------------------------------------
	// IMsoAsyncStream methods.
	MSOMETHOD(NewDataStg) (THIS_ STGMEDIUM *pstgmed, BOOL fLast) PURE;
	MSOMETHOD_(const WCHAR *, WzFilenameGet)(THIS) PURE;
	MSOMETHOD(SetStatus)(THIS_ HRESULT hrStatus) PURE;
	MSOMETHOD(GetStatus)(THIS) PURE;
	MSOMETHOD(SetStreamSize)(THIS_ DWORD dwSize) PURE;
	MSOMETHOD(GetAvailableSize)(THIS) PURE;
	MSOMETHOD(Reset)(THIS);
	MSOMETHOD(Wait)(THIS);
	MSOMETHOD(CloseStream)(THIS);
};


#ifdef __cplusplus
};	// extern "C"
#endif // __cplusplus

#endif // !__MSODM_H__



// ----------------------------------------------------------------------------
// Change Log
//
// Date		Version		Description
// -----	-------		-------------------------------------------------------
// 09/08/94 1.1		    Added GetSzFileName and SetSzFileName to ISearch.
//						Added IszGetFileType and SetIszFileType to IFindFile.
//
// 09/14	  			Added FDirPicker and SetFDirPicker to IFindFile
//						Added FSearchEnabled and EnabledSearch to IFindFile
//						Added FPreviewEnabled and EnablePreview to IFindFile
//						(#ifdef LATER)
//						Added RegisterPfnEvent to IFindFile
//						Added GetControlValueW, GetControlValueSz,
//						SetControlValueW, SetControlValueSz to IFindFile
//						(#ifdef LATER)
//						IOOP_FF type defined as int, and enum constants
//						defined.  -amirr
//
// 09/15				Added IControl, IControlList interfaces to replace
//						ICustomItem, ICustomItemList interfaces.  #define'd
//						ICustomItem, ICustomItemList, CCustomItem,
//						CCustomItemList for backwards compatibility.
//						Changed GetICustomItemList on IFindFile to
//						GetIControlList #define'd for backwards compatibility.
//						Changed CcstitmGet to CcntrlGet on IControlList.
//						#define'd for backwards compatibility.
//						IOTM_FF enum constants redefined.
//						-amirr
//
// 09/16				Added standard control values to otp list.
//						Added SetFocusIcntrl to IFindFile.
//						-amirr
//
// 09/21				Reordered declarations to match up with API document
//						for easier updating and maintainability.
//						Added GetFreezeDir and SetFreezeDir to IFindFile
//						Added short-cut methods GetIcntrlValueW,
//						GetIcntrlValueSz, SetIcntrlValueW, SetIcntrlValueSz to
//						IFindFile.
//						Added ICommandList interface and functions AddRef,
//						Release, QueryInterface, Get, GetFEnabled, SetFEnabled,
//
// 9/29					Added IAppPreview.
//						Added FFDesirePreview
//						Modified EnablePreview to have an (IAppPreview *) parm.
//						-stevepol
// 9/30					Added help stuff - mikekell
//
// 10/13				Added GetHwnd, GetCenterXY, SetCenterXY to IFindFile.
//						FF_E_DIALOG_NOT_VISIBLE returned when trying to do a
//						GetHwnd on the dialog when it is not displayed.
// 						Multiple dialog instances now working.  FF_findfileMax
//						denotes the largest number of find file dialogs
//						displayable at one time.  -amirr
//
// 10/21				Added FConfirmReplace, SetFConfirmReplace,
//						GetDefaultFileName, SetDefaultFileName to IFindFile.
//						-amirr
//
// 10/28				Added FKeepSelectedType, SetFKeepSelectedType to
//						IFindFile.  Added FF_icmd enum values,
//						FF_icmdShiftOpen, FF_icmdOpenRO, and FF_icmdPrint.
//						FF_cbMaxType extended to 120 from 60.
//						-amirr.
//
// 12/14				Changed char * paramaters to const char * parameters
//						in SetXXXXX methods.  - erikhan
//
// 01/03/95				Added FF_icleXXX constants used to enable or disable
//						commands in the Open dialog's commands dropdown.
//
// 01/03				Added FFileDirPicker and SetFFileDirPicker to
//						IFindFile.  Added Clear to ISearch. UNDONE:  Reorganize
//						the functions when the opportunity presents itself.
//						- amirr.
//
// 01/06				Added GetView and SetView.  - mikekell
//
//						Added FInitFileOpen and CleanupFileOpen.  - erikhan
//
// 02/21				Added FLoadFileOpen prototype.  - erikhan
//
// 04/06				Added FF_ioopQueryFinished and FGetSzMissingDll.
//						- erikhan
//
// 08/04				Name changed to "dm96.h" for the `96 project.
//						- erikhan
//
// 08/10				Added IOLDocument.  -- erikhan
//
// 08/14				Added GUIDs for IIDs.  - erikhan
//
// 09/26				Changed interfaces, constants and types to Office
// 						naming conventions      - brianwen
//
// 12/02				Added IBindCtx and IBindStatusCallback to GetFilePath
//						and GetFSSpec for IMsoOLDocument per SriniK.  Also
//						added new method SetFOnlyFileSys (and related query method)
//						to require that only items physically present in the file
//						system be returned (no URLs).  This is useful for e.g.
//						the indexer and Word's Tools/Options/File Locations browser.
//						Carried in dm96.h changes.
//
// 12/17/95				Added Mac FindFile code from Paradigm.
//
//
// 1/3/96				Added ffopOpenAsCopy to FFOP enumeration
//						Added msodmicmdOpenAsCopy and msodmicleOpenAsCopy
//						Added fGreyOpenAsCopyCmd to GLUEPREFS  -- kander
// 3/4/96				Added params to IMsoSearch_SetPropSzValMacro to
//						return parse error info for Shilshole.
//						Added new API, MsoGetIndexInformation.
//						Added new API, IMsoSearch_SetSearchConnector, which
//						allows WebFind to connect search criteria with OR instead
//						of AND.
//						Added a new API to allow Ren to get back unresolved links.
//
// 1/8/97				FrankRon added API's to allow the enabling of custom
//						controls on the Open Dialogs to be linked to the enabling
//						of the OK button
//
// 1/16/97				FrankRon added API's to allow use of the System Dir
//						Picker (SHBrowseForFolder) instead of our own SDM DirPicker
//
// 8/13/97				IlyaV renamed msodm.h//
// 9/23/97				stevepol GetFVisible, SetFVisible, TW_ListNameFOR_TESTING_ONLY
// 12/24/97				IlyaV commented out Shilshole-related stuff.
// 2/3/98				IlyaV general cleanup.
//
// 3/23/98			ThomasOl added fEnableRedirect parm to IMsoOLDocument_GetFilePath
//
// 8/17/99				ABeeman added MsoHrHttpUrlToPsfPidl
//
// 3/1/00				Shanem added MsoHrPSFCreateIOLDocFromPersistent
