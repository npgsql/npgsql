//***************************************************************************
// scc.h
//
// This module contains the prototypes and definitions for Microsoft's
// Source Code Control API.  This file and the SCC API are covered by
// VSIP and VSIP Extras license agreement and are not to be redistributed
// without permission by Microsoft.
//
// Terms used in this module:
//  IDE = Visual Studio Integrated Development Environment
//  dir = directory on local/client machine
//
// This module has been updated to include the following:
//	May '96:	
//	 * The SCC version number has been bumped to 1.1.
//	 * 16-bit is no longer supported.
//	 * A new capability bit, SCC_CAP_REENTRANT, has been added.  A provider
//		which returns this bit can handle multiple SCC context values (eg:
//		multiple simultaneous open projects), and reentrant calls for those
//		contexts (ie: thread safe).
//   * A new option, SCC_OPT_SCCCHECKOUTONLY has been added. This is used
//      to disallow scc operations from outside of integration.  Integration
//      hosts like MS Access use this to prevent files from being checked out
//      without also being imported into scc.
//  March '98:
//   * The SetOption SCC_OPT_SHARESUBPROJ was added to allow destination dir 
//      for share. Setting this option changes the semantics of SccAddFromScc 
//      to accept the target as input.
//  May '02:
//   * The SCC version number has been bumped to 1.2.
//   * The following new capability bits have been added:
//      SCC_CAP_CREATESUBPROJECT,
//      SCC_CAP_GETPARENTPROJECT,
//      SCC_CAP_BATCH,
//      SCC_CAP_DIRECTORYSTATUS,
//      SCC_CAP_DIRECTORYDIFF,
//      SCC_CAP_MULTICHECKOUT,
//      SCC_CAP_SCCFILE
//   * The following new functions corresponding to the new capability bits
//     have been added:
//      SccCreateSubProject
//      SccGetParentProjectPath
//      SccBeginBatch
//      SccEndBatch
//      SccDirQueryInfo
//      SccDirDiff
//      SccIsMultiCheckoutEnabled
//      SccWillCreateSccFile
//   * The new enum SccDirStatus has been added for use with SccDirQueryInfo
//  April '03
//   * The SCC version number has been bumped to 1.3
//      SccGetExtendedCapabilities was added for extra capability bits
//      SccEnumChangedFiles was added for retrieving the list of files who 
//              have newer versions in the database than on local disk.
//      SccQueryChanges was added to discover namespace changes (renames/additions/deletions)( 
//      SccPopulateDirList was added for discovering folders in the store
//      SccAddFilesFromScc
//		SccBackgroundGet
//      SccGetUserOption
//    * File types and encodings have been superseded to support Unicode
//    * All API functions were annotated with SAL macros.
//      See specstrings.h from Windows SDK for more information about the macros. 
//
// Copyright (c) Microsoft Corporation, All rights reserved.
//***************************************************************************
#ifndef _SCC_DEFS
#define _SCC_DEFS

#include <stdlib.h>


/****************************************************************************
    Make it easy to export functions
****************************************************************************/
#if !defined( EXTFUN )
#if defined( WIN32 )
#define EXTFUN __declspec(dllexport)
#else
#define EXTFUN __export
#endif
#endif

#if defined( __cplusplus )
#define SCCEXTERNC extern "C"
#else
#define SCCEXTERNC
#endif



/****************************************************************************
    Linkage for external functions will be C naming mode.
****************************************************************************/
#if defined( __cplusplus )
extern "C" {
#endif



/****************************************************************************
    Take care of type information based on platform and settings.
****************************************************************************/
#include <winnls.h>

#if defined( WIN16 )
typedef BOOL far * LPBOOL;
#endif

/****************************************************************************
    Version Flag.  Major is HIWORD, Minor is LOWORD
****************************************************************************/
#define SCC_MAJOR_VER_VAL   1
#define SCC_MINOR_VER_VAL   3
#define SCC_VER_NUM         MAKELONG(SCC_MINOR_VER_VAL, SCC_MAJOR_VER_VAL)
#define SCC_GET_MAJOR_VER(ver)  HIWORD(ver)
#define SCC_GET_MINOR_VER(ver)  LOWORD(ver)



/****************************************************************************
    Following strings are the keys for accessing the registry to find
    the SCC provider.
****************************************************************************/
#if !defined( _SCC_REG_KEYS )
#define _SCC_REG_KEYS
#define STR_SCC_PROVIDER_REG_LOCATION   "Software\\SourceCodeControlProvider"
#define STR_PROVIDERREGKEY              "ProviderRegKey"
#define STR_SCCPROVIDERPATH             "SCCServerPath"
#define STR_SCCPROVIDERNAME             "SCCServerName"
#define STR_SCC_INI_SECTION             "Source Code Control"
#define STR_SCC_INI_KEY                 "SourceCodeControlProvider"
#define SCC_PROJECTNAME_KEY             "SCC_Project_Name"
#define SCC_PROJECTAUX_KEY              "SCC_Aux_Path"
#define SCC_STATUS_FILE                 "MSSCCPRJ.SCC"
#define SCC_KEY                         "SCC"
#define SCC_FILE_SIGNATURE              "This is a source code control file"
#define SCC_NSE                         "Namespace Extension"
#define SCC_NSE_DisableOpenSCC          "DisableOpenFromSourceControl"
#define STR_SCCHELPCOLLECTION			"HelpCollection"
#define STR_UI_LANGUAGE					"UILanguage"
#define STR_SRCSAFE_ROOT_KEY			"Software\\Microsoft\\SourceSafe"

#endif /* _SCC_REG_KEYS */



/****************************************************************************
    String lengths (each length does *not* include the terminating '\0')
****************************************************************************/
#define SCC_NAME_LEN            31      // lpSccName, SCCInitialize
#define SCC_AUXLABEL_LEN        31      // lpAuxPathLabel, SCCInitialize
#define SCC_USER_LEN            31      // lpUser, SCCOpenProject
#define SCC_PRJPATH_LEN         300     // lpAuxProjPath, SCCGetProjPath

/****************************************************************************
    String sizes (which are lengths + 1 for the terminating '\0')
****************************************************************************/
#define SCC_NAME_SIZE           (SCC_NAME_LEN + 1)
#define SCC_AUXLABEL_SIZE       (SCC_AUXLABEL_LEN + 1)
#define SCC_USER_SIZE           (SCC_USER_LEN + 1)
#define SCC_PRJPATH_SIZE        (SCC_PRJPATH_LEN + 1)

/****************************************************************************
    These are the error codes that may be returned from a function.
    All errors are < 0, warnings are > 0, and success is 0.  Use the
    macros provided for quick checking.  
****************************************************************************/
typedef long SCCRTN;
typedef SCCRTN FAR * LPSCCRTN;

#define IS_SCC_ERROR(rtn) (((rtn) < 0) ? TRUE : FALSE)
#define IS_SCC_SUCCESS(rtn) (((rtn) == SCC_OK) ? TRUE : FALSE)
#define IS_SCC_WARNING(rtn) (((rtn) > 0) ? TRUE : FALSE)


#define SCC_I_SHARESUBPROJOK				7
#define SCC_I_FILEDIFFERS						6
#define SCC_I_RELOADFILE						5
#define SCC_I_FILENOTAFFECTED                   4
#define SCC_I_PROJECTCREATED                    3
#define SCC_I_OPERATIONCANCELED                 2
#define SCC_I_ADV_SUPPORT                       1

#define SCC_OK                                  0

#define SCC_E_INITIALIZEFAILED                  -1
#define SCC_E_UNKNOWNPROJECT                    -2
#define SCC_E_COULDNOTCREATEPROJECT             -3
#define SCC_E_NOTCHECKEDOUT                     -4
#define SCC_E_ALREADYCHECKEDOUT                 -5
#define SCC_E_FILEISLOCKED                      -6
#define SCC_E_FILEOUTEXCLUSIVE                  -7
#define SCC_E_ACCESSFAILURE                     -8
#define SCC_E_CHECKINCONFLICT                   -9
#define SCC_E_FILEALREADYEXISTS                 -10
#define SCC_E_FILENOTCONTROLLED                 -11
#define SCC_E_FILEISCHECKEDOUT                  -12
#define SCC_E_NOSPECIFIEDVERSION                -13
#define SCC_E_OPNOTSUPPORTED                    -14
#define SCC_E_NONSPECIFICERROR                  -15
#define SCC_E_OPNOTPERFORMED                    -16
#define SCC_E_TYPENOTSUPPORTED                  -17
#define SCC_E_VERIFYMERGE                       -18
#define SCC_E_FIXMERGE                          -19
#define SCC_E_SHELLFAILURE                      -20
#define SCC_E_INVALIDUSER                       -21
#define SCC_E_PROJECTALREADYOPEN                -22
#define SCC_E_PROJSYNTAXERR                     -23
#define SCC_E_INVALIDFILEPATH                   -24
#define SCC_E_PROJNOTOPEN                       -25
#define SCC_E_NOTAUTHORIZED                     -26
#define SCC_E_FILESYNTAXERR                     -27
#define SCC_E_FILENOTEXIST                      -28
#define SCC_E_CONNECTIONFAILURE                 -29
#define SCC_E_UNKNOWNERROR                      -30
#define SCC_E_BACKGROUNDGETINPROGRESS           -31



#ifndef _SCC_STATUS_DEFINED
#define _SCC_STATUS_DEFINED
/****************************************************************************
    The SCC_STATUS_xxx macros define the state of a file
****************************************************************************/
enum  SccStatus 
{
    SCC_STATUS_INVALID          = -1L,			// Status could not be obtained, don't rely on it
    SCC_STATUS_NOTCONTROLLED    = 0x00000000L,	// File is not under source control
    SCC_STATUS_CONTROLLED       = 0x00000001L,	// File is under source code control
    SCC_STATUS_CHECKEDOUT       = 0x00000002L,	// Checked out to current user at local path
    SCC_STATUS_OUTOTHER         = 0x00000004L,	// File is checked out to another user
    SCC_STATUS_OUTEXCLUSIVE     = 0x00000008L,	// File is exclusively check out
    SCC_STATUS_OUTMULTIPLE      = 0x00000010L,	// File is checked out to multiple people
    SCC_STATUS_OUTOFDATE        = 0x00000020L,	// The file is not the most recent
    SCC_STATUS_DELETED          = 0x00000040L,	// File has been deleted from the project
    SCC_STATUS_LOCKED           = 0x00000080L,	// No more versions allowed
    SCC_STATUS_MERGED           = 0x00000100L,	// File has been merged but not yet fixed/verified
    SCC_STATUS_SHARED           = 0x00000200L,	// File is shared between projects
    SCC_STATUS_PINNED           = 0x00000400L,	// File is shared to an explicit version
    SCC_STATUS_MODIFIED         = 0x00000800L,	// File has been modified/broken/violated
    SCC_STATUS_OUTBYUSER        = 0x00001000L,	// File is checked out by current user someplace
    SCC_STATUS_NOMERGE          = 0x00002000L,	// File is never mergeable and need not be saved before a GET
    SCC_STATUS_RESERVED_1       = 0x00004000L,	// Status bit reserved for internal use
    SCC_STATUS_RESERVED_2       = 0x00008000L,	// Status bit reserved for internal use
    SCC_STATUS_RESERVED_3       = 0x00010000L 	// Status bit reserved for internal use
};
#endif /* _SCC_STATUS_DEFINED */



#ifndef _SCC_DIRSTATUS_DEFINED
#define _SCC_DIRSTATUS_DEFINED
/****************************************************************************
    The SCC_DIRSTATUS_xxx macros define the state of a directory
****************************************************************************/
enum  SccDirStatus 
{
    SCC_DIRSTATUS_INVALID       = -1L,		// Status could not be obtained, don't rely on it
    SCC_DIRSTATUS_NOTCONTROLLED = 0x0000L,	// Directory is not under source control
											//   i.e. there is no project corresponding to the directory
    SCC_DIRSTATUS_CONTROLLED    = 0x0001L,	// Directory is under source code control
											//   i.e. there exists a project corresponding to the directory
    SCC_DIRSTATUS_EMPTYPROJ     = 0x0002L	// Project corresponding to directory is empty
};
#endif /* _SCC_DIRSTATUS_DEFINED */



/****************************************************************************
    SccOpenProject flags
****************************************************************************/
#define SCC_OP_CREATEIFNEW      0x00000001L
#define SCC_OP_SILENTOPEN       0x00000002L


/****************************************************************************
    Keep checked out
****************************************************************************/
#define SCC_KEEP_CHECKEDOUT     0x1000


/****************************************************************************
    Add flags
****************************************************************************/
#define SCC_ADD_STORELATEST     0x04	// Store only the latest version of the file(s).
#define SCC_FILETYPE_AUTO       0x00	// Auto-detect type of the file(s).

// The following flags are mutually exculsive.
#define SCC_FILETYPE_TEXT       0x01	// Obsolete. Use SCC_FILETYPE_TEXT_ANSI instead.
#define SCC_FILETYPE_BINARY     0x02	// Treat the file(s) as binary.
#define SCC_FILETYPE_TEXT_ANSI  0x08	// Treat the file(s) as ANSI.
#define SCC_FILETYPE_UTF8       0x10	// Treat the file(s) as Unicode in UTF8 format.
#define SCC_FILETYPE_UTF16LE    0x20	// Treat the file(s) as Unicode in UTF16 Little Endian format.
#define SCC_FILETYPE_UTF16BE    0x40	// Treat the file(s) as Unicode in UTF16 Big Endian format.


/****************************************************************************
    Diff flags.  The SCC_DIFF_QD_xxx flags are mutually exclusive.  If any
	one of the three are specified, then no visual feed back is to be given.
	If one is specified but not supported, then the next best one is chosen.
****************************************************************************/
#define SCC_DIFF_IGNORECASE     0x0002
#define SCC_DIFF_IGNORESPACE    0x0004
#define SCC_DIFF_QD_CONTENTS	0x0010
#define SCC_DIFF_QD_CHECKSUM	0x0020
#define SCC_DIFF_QD_TIME		0x0040
#define SCC_DIFF_QUICK_DIFF		0x0070		/* Any QD means no display     */


/****************************************************************************
    Get flags
****************************************************************************/
#define SCC_GET_ALL             0x00000001L
#define SCC_GET_RECURSIVE       0x00000002L
#define SCC_GET_OVERWRITE       0x00000004L

/****************************************************************************
    PopulateList flags
****************************************************************************/
#define SCC_PL_DIR				0x00000001L	/* whether input items are directory names */

/****************************************************************************
    Checkout flags
****************************************************************************/
#define SCC_CHECKOUT_LOCALVER   0x00000002L

/****************************************************************************
    Options for SccRemoveDir
****************************************************************************/
#define SCC_RD_DEFAULT			0x0000		/* remove the folder and files within */
#define SCC_RD_EMPTYTREE		0x0001		/* remove the folder if the hierarchy contains only empty folders */

/****************************************************************************
    Options for SccGetCommandOptions and SccPopulateList
****************************************************************************/
typedef LPVOID LPCMDOPTS;
#ifndef SCCCOMMAND_DEFINED
#define SCCCOMMAND_DEFINED
enum  SCCCOMMAND 
{
	SCC_COMMAND_GET,
	SCC_COMMAND_CHECKOUT,
	SCC_COMMAND_CHECKIN,
	SCC_COMMAND_UNCHECKOUT,
  	SCC_COMMAND_ADD,
	SCC_COMMAND_REMOVE,
	SCC_COMMAND_DIFF,
	SCC_COMMAND_HISTORY,
	SCC_COMMAND_RENAME,
	SCC_COMMAND_PROPERTIES,
	SCC_COMMAND_OPTIONS
};
#endif /* SCCCOMMAND_DEFINED */

typedef BOOL (__cdecl *POPLISTFUNC)  (_Inout_opt_ LPVOID pvCallerData, _In_ BOOL bAddKeep, _In_ LONG nStatus, _In_z_ LPCSTR lpFile);
typedef BOOL (__cdecl *POPLISTFUNCA) (_Inout_opt_ LPVOID pvCallerData, _In_ BOOL bAddKeep, _In_ LONG nStatus, _In_z_ LPCSTR lpFile);
typedef BOOL (__cdecl *POPLISTFUNCW) (_Inout_opt_ LPVOID pvCallerData, _In_ BOOL bAddKeep, _In_ LONG nStatus, _In_z_ LPCWSTR lpFile);
#ifdef UNICODE
typedef POPLISTFUNCW POPLISTFUNCT;
#else
typedef POPLISTFUNCA POPLISTFUNCT;
#endif

/****************************************************************************
    Options and callback function declarations for SccPopulateDirList
****************************************************************************/
#define SCC_PDL_ONELEVEL		0x0000
#define SCC_PDL_RECURSIVE		0x0001
#define SCC_PDL_INCLUDEFILES	0x0002

typedef BOOL (__cdecl *POPDIRLISTFUNC) (_Inout_opt_ LPVOID pvCallerData, _In_ BOOL bFolder,  _In_z_ LPCSTR lpDirectoryOrFileName);
typedef BOOL (__cdecl *POPDIRLISTFUNCA)(_Inout_opt_ LPVOID pvCallerData, _In_ BOOL bFolder, _In_z_ LPCSTR lpDirectoryOrFileName);
typedef BOOL (__cdecl *POPDIRLISTFUNCW)(_Inout_opt_ LPVOID pvCallerData, _In_ BOOL bFolder, _In_z_ LPCWSTR lpDirectoryOrFileName);

/****************************************************************************
    Options and callback function declarations for SccQueryChanges
****************************************************************************/

/* Structure used by the SccQueryChanges callback */
struct QUERYCHANGESDATA_A
{
       DWORD  dwSize;
       LPCSTR lpFileName;
       DWORD  dwChangeType;
       LPCSTR lpLatestName;
};

typedef struct QUERYCHANGESDATA_A QUERYCHANGESDATA;

struct QUERYCHANGESDATA_W
{
       DWORD   dwSize;
       LPCWSTR lpFileName;
       DWORD   dwChangeType;
       LPCWSTR lpLatestName;
};

/* Values for dwChangeType 

Note: When SCC_CHANGE_RENAMED_TO is returned, the lpLatestName should contain the new file name in local-path terms.
If the new filename cannot be relativized to the current connection opened, return SCC_CHANGE_DATABASE_DELETED instead.
*/
#define SCC_CHANGE_UNKNOWN			0 /* cannot tell what has changed */
#define SCC_CHANGE_UNCHANGED		1 /* no namespace change for this file */
#define SCC_CHANGE_DIFFERENT		2 /* file with different identity exists in the database */
#define SCC_CHANGE_NONEXISTENT		3 /* file does not exist neither in the database nor locally */
#define SCC_CHANGE_DATABASE_DELETED	4 /* file deleted in the database */
#define SCC_CHANGE_LOCAL_DELETED	5 /* file deleted locally but still exists in the database; if this cannot be determined, you can return SCC_CHANGE_DATABASE_ADDED instead */
#define SCC_CHANGE_DATABASE_ADDED	6 /* file added in the database, does not exist locally */
#define SCC_CHANGE_LOCAL_ADDED		7 /* file does not exist in database, and is a new local file */
#define SCC_CHANGE_RENAMED_TO		8 /* file renamed or moved in the database into lpLatestName */
#define SCC_CHANGE_RENAMED_FROM		9 /* file renamed or moved in the database from lpLatestName; if this is too expensive to track, you can return other flag, like SCC_CHANGE_DATABASE_ADDED */

typedef BOOL (__cdecl *QUERYCHANGESFUNC) (_Inout_opt_ LPVOID pvCallerData, _In_ QUERYCHANGESDATA * pChangesData);
typedef BOOL (__cdecl *QUERYCHANGESFUNCA)(_Inout_opt_ LPVOID pvCallerData, _In_ QUERYCHANGESDATA_A * pChangesData);
typedef BOOL (__cdecl *QUERYCHANGESFUNCW)(_Inout_opt_ LPVOID pvCallerData, _In_ QUERYCHANGESDATA_W * pChangesData);

/****************************************************************************
    The SCC_CAP_xxx flags are used to determine what capabilites a provider
    has.
****************************************************************************/
#define SCC_CAP_REMOVE            0x00000001L   // Supports the SCC_Remove command
#define SCC_CAP_RENAME            0x00000002L   // Supports the SCC_Rename command
#define SCC_CAP_DIFF              0x00000004L   // Supports the SCC_Diff command
#define SCC_CAP_HISTORY           0x00000008L   // Supports the SCC_History command
#define SCC_CAP_PROPERTIES        0x00000010L   // Supports the SCC_Properties command
#define SCC_CAP_RUNSCC            0x00000020L   // Supports the SCC_RunScc command
#define SCC_CAP_GETCOMMANDOPTIONS 0x00000040L   // Supports the SCC_GetCommandOptions command
#define SCC_CAP_QUERYINFO         0x00000080L   // Supports the SCC_QueryInfo command
#define SCC_CAP_GETEVENTS         0x00000100L   // Supports the SCC_GetEvents command
#define SCC_CAP_GETPROJPATH       0x00000200L   // Supports the SCC_GetProjPath command
#define SCC_CAP_ADDFROMSCC        0x00000400L   // Supports the SCC_AddFromScc command
#define SCC_CAP_COMMENTCHECKOUT   0x00000800L   // Supports a comment on Checkout
#define SCC_CAP_COMMENTCHECKIN    0x00001000L   // Supports a comment on Checkin
#define SCC_CAP_COMMENTADD        0x00002000L   // Supports a comment on Add
#define SCC_CAP_COMMENTREMOVE     0x00004000L   // Supports a comment on Remove
#define SCC_CAP_TEXTOUT           0x00008000L   // Writes text to an IDE-provided output function
#define SCC_CAP_CREATESUBPROJECT  0x00010000L   // Supports the SccCreateSubProject command
#define SCC_CAP_GETPARENTPROJECT  0x00020000L   // Supports the SccGetParentProjectPath command
#define SCC_CAP_BATCH             0x00040000L   // Supports the SccBeginBatch and SccEndBatch commands
#define SCC_CAP_DIRECTORYSTATUS   0x00080000L   // Supports the querying of directory status
#define SCC_CAP_DIRECTORYDIFF     0x00100000L   // Supports differencing on directories
#define SCC_CAP_ADD_STORELATEST   0x00200000L   // Supports storing files without deltas
#define SCC_CAP_HISTORY_MULTFILE  0x00400000L   // Multiple file history is supported
#define SCC_CAP_IGNORECASE        0x00800000L   // Supports case insensitive file comparison
#define SCC_CAP_IGNORESPACE       0x01000000L   // Supports file comparison that ignores white space
#define SCC_CAP_POPULATELIST      0x02000000L   // Supports finding extra files
#define SCC_CAP_COMMENTPROJECT    0x04000000L   // Supports comments on create project
#define SCC_CAP_MULTICHECKOUT     0x08000000L   // Supports multiple checkouts on a file
												//   (subject to administrator override)
#define SCC_CAP_DIFFALWAYS        0x10000000L   // Supports diff in all states if under control
#define SCC_CAP_GET_NOUI          0x20000000L	// Provider doesn't support a UI for SccGet,
												//   but IDE may still call SccGet function.
#define SCC_CAP_REENTRANT		  0x40000000L	// Provider is reentrant and thread safe.
#define SCC_CAP_SCCFILE           0x80000000L   // Supports the MSSCCPRJ.SCC file
												//   (subject to user/administrator override)

/****************************************************************************
    The SCC_EXCAP_xxx values are used to determine what extra capabilites 
	a provider has.
****************************************************************************/
#define SCC_EXCAP_CHECKOUT_LOCALVER		1L   // Supports the Checkout local version
#define SCC_EXCAP_BACKGROUND_GET		2L   // Supports the SccBackgroundGet operation
#define SCC_EXCAP_ENUM_CHANGED_FILES	3L   // Supports the SccEnumChangedFiles operation
#define SCC_EXCAP_POPULATELIST_DIR		4L   // Supports finding extra directories
#define SCC_EXCAP_QUERYCHANGES			5L   // Supports enumerating file changes
#define SCC_EXCAP_ADD_FILES_FROM_SCC	6L   // Supports the SccAddFilesFromSCC operation
#define SCC_EXCAP_GET_USER_OPTIONS		7L   // Supports the SccGetUserOption function
#define SCC_EXCAP_THREADSAFE_QUERY_INFO	8L   // Supports calling SccQueryInfo on multiple threads
#define SCC_EXCAP_REMOVE_DIR			9L   // Supports the SccRemoveDir function
#define SCC_EXCAP_DELETE_CHECKEDOUT    10L   // Can delete checked out files
#define SCC_EXCAP_RENAME_CHECKEDOUT	   11L   // Can rename checked out files
// Note: other capabilities may be added to this list in future releases
// Providers should play safe and return NotSupported from a call to GetSccExtendedCapabilities
// for all capabilities that are not supported or are not recognized by the provider.

// Data structures for background processing messages
typedef struct 
{
	DWORD dwBackgroundOperationID;	// ID of the background operation
} SccMsgDataIsCancelled;

typedef struct 
{
	DWORD dwBackgroundOperationID;	// ID of the background operation
	PCSTR szFile;					// File path
} SccMsgDataOnBeforeGetFile;

typedef struct 
{
	DWORD dwBackgroundOperationID;	// ID of the background operation
	PCSTR szFile;					// File path
	SCCRTN sResult;					// Result of retrieving of the file
} SccMsgDataOnAfterGetFile;

typedef struct 
{
	DWORD dwBackgroundOperationID;	// ID of the background operation
	PCSTR szMessage;		// The message text
	BOOL bIsError;			// TRUE for an error message; FALSE for a warning or for an informational message
} SccMsgDataOnMessage;

/****************************************************************************
	The following flags are used for the print call-back that the IDE
	provides on SccInitialize.  
	
	If the IDE supports cancel, it may get one of the Cancel messages.
	In this case, the provider will inform the IDE to show the Cancel
	button with SCC_MSG_STARTCANCEL.  After this, any set of normal
	messages may be sent.  If any of these return SCC_MSG_RTN_CANCEL,
	then the provider will quit the operation and return.  The Provider
	will also poll periodically with SCC_MSG_DOCANCEL to see if the
	user has canceled the operation.  When all operations are done, or
	the user has canceled, SCC_MSG_STOPCANCEL will be sent through.

	The SCC_MSG_INFO, WARNING, and ERROR types are used for messages that
	get displayed in the scrolling list of messages.  SCC_MSG_STATUS is
	a special type that indicates that the text should show up in a 
	status bar or temporary display area.  This message type should not
	remain permanently in the list.

	All background operation has an ID associated with it. The first argument
	of all background processing messages must be casted to 
	the correspondent structure which contains the ID.

****************************************************************************/
enum
{
	// Return codes
	SCC_MSG_RTN_CANCEL=-1,				// Returned from call-back to indicate cancel
	SCC_MSG_RTN_OK=0,					// Returned from call-back to continue
	// Message types
	SCC_MSG_INFO=1,						// Message is informational
	SCC_MSG_WARNING,					// Message is a warning
	SCC_MSG_ERROR,						// Message is an error
	SCC_MSG_STATUS,						// Message is meant for status bar
	// IDE supports Cancel operation
	SCC_MSG_DOCANCEL,					// No text, IDE returns 0 or SCC_MSG_RTN_CANCEL
	SCC_MSG_STARTCANCEL,				// Start a cancel loop
	SCC_MSG_STOPCANCEL,					// Stop the cancel loop

	// Background processing operations
	SCC_MSG_BACKGROUND_IS_CANCELLED,		// IDE returns 0 if the operation is not cancelled or SCC_MSG_RTN_CANCEL if it is.
											// Cast first argument to SccMsgDataIsCancelled structure pointer. 
	SCC_MSG_BACKGROUND_ON_BEFORE_GET_FILE,	// Called before file file is retrived. Cast first argument to SccMsgDataOnBeforeGetFile structure pointer. 
	SCC_MSG_BACKGROUND_ON_AFTER_GET_FILE,	// Called after file was processed. Cast first argument to SccMsgDataOnAfterGetFile structure pointer.
	SCC_MSG_BACKGROUND_ON_MESSAGE			// Called to provide information about background processing. Cast first argument to SccMsgDataOnMessage structure pointer.
};

#define SCC_FIRST_BACKGROUND_MSG SCC_MSG_BACKGROUND_IS_CANCELLED
#define SCC_LAST_BACKGROUND_MSG SCC_MSG_BACKGROUND_ON_MESSAGE

#ifndef _LPTEXTOUTPROC_DEFINED
#define _LPTEXTOUTPROC_DEFINED
typedef long (__cdecl *LPTEXTOUTPROC) (_In_opt_z_ LPCSTR, _In_ DWORD);
#endif /* _LPTEXTOUTPROC_DEFINED */


/****************************************************************************
    nOption values for SccSetOption.
****************************************************************************/
#define SCC_OPT_EVENTQUEUE      0x00000001L     // Set status of the event queue
#define SCC_OPT_USERDATA        0x00000002L     // Specify user data for 
                                                // SCC_OPT_NAMECHANGEPFN
#define SCC_OPT_HASCANCELMODE	0x00000003L     // The IDE can handle Cancel 
                                                // of long running operations
#define SCC_OPT_NAMECHANGEPFN   0x00000004L     // Set a callback for name changes
#define SCC_OPT_SCCCHECKOUTONLY 0x00000005L     // Disable SS explorer checkout, 
                                                // and don't set working dir
#define SCC_OPT_SHARESUBPROJ    0x00000006L     // if this is turned on, allow
                                                // AddFromScc to specify a working
                                                // dir, try to share into the assoc
                                                // project if direct descendant.
/* SCC_OPT_EVENTQUEUE values */
#define SCC_OPT_EQ_DISABLE      0x00L           // Suspend event queue activity
#define SCC_OPT_EQ_ENABLE       0x01L           // Enable event queue logging

/* SCC_OPT_NAMECHANGEPFN callback typedef */
typedef void (__cdecl *OPTNAMECHANGEPFN)(_Inout_opt_ LONG pvCallerData, 
                    _In_z_ LPCSTR pszOldName, _In_z_ LPCSTR pszNewName);

/****************************************************************************
	Values for SCC_OPT_HASCANCELMODE.  By default, it is assumed that the IDE
	will not allow for canceling a long running operation.  The provider must
	handle this on their own in this case.  If the IDE, however, sets this
	option to SCC_OPT_HCM_YES, it means that it will handle canceling the
	operation.  In this case, use the SCC_MSG_xxx flags with the output
	call-back to tell the IDE what messages to display while the operation
	is running.
****************************************************************************/
#define SCC_OPT_HCM_NO			0L				// (Default) Has no cancel mode,
												//	Provider must supply if desired
#define SCC_OPT_HCM_YES			1L				// IDE handles cancel

/****************************************************************************
	Values for SCC_OPT_SCCCHECKOUTONLY.  By default, it is assumed that 
	the user may use the gui to get and checkout files from this project,
	and that a working dir should be set,  If this option is explicitly turned on,
	then no working dir is set for the project, and the files may only be gotten
	or checked in or out from scc integration, never from the gui.
****************************************************************************/
#define SCC_OPT_SCO_NO			0L				// (Default) OK to checkout from GUI
												//	Working dir is set.
#define SCC_OPT_SCO_YES			1L				// no GUI checkout, no working dir

/****************************************************************************
    nOption values for SccGetUserOption (not binary flags)
****************************************************************************/
#define SCC_USEROPT_CHECKOUT_LOCALVER   1L     // Whether the user wants to checkout local version of files

/****************************************************************************
	Values for SCC_USEROPT_CHECKOUT_LOCALVER.  
****************************************************************************/
#define SCC_USEROPT_COLV_NO  0L					// Checkout local version is supported and the user 
												// prefers to checkout the tip version of files
#define SCC_USEROPT_COLV_YES 1L					// Checkout local version is supported and the user wants 
												// to checkout local version of files, if that can be detected
#define SCC_USEROPT_COLV_DISABLED 2L			// Checkout local version is not supported. Might have been disabled
												// by administrator.

/****************************************************************************
    Following are the ASCII definitions of the functions.
****************************************************************************/



/*******************************************************************************
	Returns a 4 byte version of the provider.  This can be used to check for 
	SCC spec conformance.
*******************************************************************************/
SCCEXTERNC LONG EXTFUN __cdecl SccGetVersion(void);

/*******************************************************************************
	Call this function once per instance of a provider.
*******************************************************************************/
SCCEXTERNC SCCRTN EXTFUN __cdecl SccInitialize(
                        _Deref_out_ LPVOID * ppContext, 
                        _In_ HWND hWnd, 
                        _In_z_ LPCSTR lpCallerName,
                        _Out_z_cap_(SCC_NAME_SIZE) LPSTR lpSccName, 
                        _Out_ LPLONG lpSccCaps, 
                        _Out_z_cap_(SCC_AUXLABEL_SIZE) LPSTR lpAuxPathLabel, 
                        _Out_ LPLONG pnCheckoutCommentLen, 
                        _Out_ LPLONG pnCommentLen
                        );

/*******************************************************************************
	Call this function once for every instance of a provider, when it is going
	away.  You must call SccInitialize before calling this function, and should
	not call it with any open projects.
*******************************************************************************/
SCCEXTERNC SCCRTN EXTFUN __cdecl SccUninitialize(
                        _Inout_ LPVOID pContext
                        );

/*******************************************************************************
	Opens a project.  This function should never be called with an already open
	project on pContext.  The lpUser, lpProjName, and lpAuxProjPath values
	may be modified by the provider.
*******************************************************************************/
SCCEXTERNC SCCRTN EXTFUN __cdecl SccOpenProject(
                        _Inout_ LPVOID pContext,
                        _In_ HWND hWnd, 
                        _Inout_z_ _Inout_cap_(SCC_USER_SIZE) LPSTR lpUser,
                        _Inout_z_ _Inout_cap_(SCC_PRJPATH_SIZE) LPSTR lpProjName,
                        _In_z_ LPCSTR lpLocalProjPath,
                        _Inout_z_ _Inout_cap_(SCC_PRJPATH_SIZE) LPSTR lpAuxProjPath,
                        _In_opt_z_ LPCSTR lpComment,
                        _In_opt_ LPTEXTOUTPROC lpTextOutProc,
                        _In_ LONG dwFlags
                        );

/*******************************************************************************
	Called to close a project opened by SccOpenProject.
*******************************************************************************/
SCCEXTERNC SCCRTN EXTFUN __cdecl SccCloseProject(
                        _Inout_ LPVOID pContext
                        );

/*******************************************************************************
	Prompts the user for provider project information.  This may include the
	path to a certain project.  The caller must be prepared to accept changes
	to lpUser, lpProjName, lpLocalPath, and lpAuxProjPath.  lpProjName and
	lpAuxProjPath are then used in a call to SccOpenProject.  They should not
	be modified by the caller upon return.  The caller should avoid displaying
	these two parameters upon return, as the provider might use a formatted
	string that is not ready for view.
*******************************************************************************/
SCCEXTERNC SCCRTN EXTFUN __cdecl SccGetProjPath(
                        _Inout_ LPVOID pContext, 
                        _In_ HWND hWnd, 
                        _Inout_z_ _Inout_cap_(SCC_USER_SIZE) LPSTR lpUser,
                        _Inout_z_ _Inout_cap_(SCC_PRJPATH_SIZE) LPSTR lpProjName, 
                        _Inout_z_ _Inout_cap_(_MAX_PATH) LPSTR lpLocalPath,
                        _Inout_z_ _Inout_cap_(SCC_PRJPATH_SIZE) LPSTR lpAuxProjPath,
                        _In_ BOOL bAllowChangePath,
                        _Inout_ LPBOOL pbNew
                        );

/*******************************************************************************
	Retrieves a read only copy of a set of files.  The array is a set of files
	on the local disk.  The paths must be fully qualified.
*******************************************************************************/
SCCEXTERNC SCCRTN EXTFUN __cdecl SccGet(
                        _Inout_ LPVOID pContext, 
                        _In_ HWND hWnd, 
                        _In_ LONG nFiles, 
                        _In_count_(nFiles) LPCSTR* lpFileNames, 
                        _In_ LONG dwFlags,
                        _In_opt_ LPCMDOPTS pvOptions
                        );

/*******************************************************************************
	Initiates UI-less background retrieval of a set of files.  
	The array is a set of files on the local disk.  The paths must be fully qualified.
	The function must be thread-safe. It is always called on the thread different
	from the one that loaded the provider.
*******************************************************************************/
SCCEXTERNC SCCRTN EXTFUN __cdecl SccBackgroundGet(
                        _Inout_ LPVOID pContext, 
                        _In_ LONG nFiles, 
                        _In_count_(nFiles) LPCSTR* lpFileNames, 
                        _In_ LONG dwFlags,
                        _In_ LONG dwBackgroundOperationID
                        );

/*******************************************************************************
	Checks out the array of files.  The array is a set of fully qualified local
	path names.
*******************************************************************************/
SCCEXTERNC SCCRTN EXTFUN __cdecl SccCheckout(
                        _Inout_ LPVOID pContext, 
                        _In_ HWND hWnd, 
                        _In_ LONG nFiles, 
                        _In_count_(nFiles) LPCSTR* lpFileNames, 
                        _In_opt_z_ LPCSTR lpComment, 
                        _In_ LONG dwFlags,
                        _In_opt_ LPCMDOPTS pvOptions
                        );

/*******************************************************************************
	Undo a checkout of an array of files.  The array is a set of fully qualified
	local path names.
*******************************************************************************/
SCCEXTERNC SCCRTN EXTFUN __cdecl SccUncheckout(
                        _Inout_ LPVOID pContext, 
                        _In_ HWND hWnd, 
                        _In_ LONG nFiles, 
                        _In_count_(nFiles) LPCSTR* lpFileNames, 
                        _In_ LONG dwFlags,
                        _In_opt_ LPCMDOPTS pvOptions
                        );

/*******************************************************************************
	Make the modifications the user has made to an array of files permanent. The
	file names must be fully qualified local paths.
*******************************************************************************/
SCCEXTERNC SCCRTN EXTFUN __cdecl SccCheckin(
                        _Inout_ LPVOID pContext, 
                        _In_ HWND hWnd, 
                        _In_ LONG nFiles, 
                        _In_count_(nFiles) LPCSTR* lpFileNames, 
                        _In_opt_z_ LPCSTR lpComment, 
                        _In_ LONG dwFlags,
                        _In_opt_ LPCMDOPTS pvOptions
                        );

/*******************************************************************************
	Add an array of fully qualified files to the source control system.  The 
	array of flags describe the type of file.  See the SCC_FILETYPE_xxxx flags.
*******************************************************************************/
SCCEXTERNC SCCRTN EXTFUN __cdecl SccAdd(
                        _Inout_ LPVOID pContext, 
                        _In_ HWND hWnd, 
                        _In_ LONG nFiles, 
                        _In_count_(nFiles) LPCSTR* lpFileNames, 
                        _In_opt_z_ LPCSTR lpComment, 
                        _In_count_(nFiles) LONG * pdwFlags,
                        _In_opt_ LPCMDOPTS pvOptions
                        );

/*******************************************************************************
	Removes the array of fully qualified files from the source control system.
	The files are not removed from the user's disk, unless advanced options
	are set by the user.  Advaned options are defined by the provider.
*******************************************************************************/
SCCEXTERNC SCCRTN EXTFUN __cdecl SccRemove(
                        _Inout_ LPVOID pContext, 
                        _In_ HWND hWnd, 
                        _In_ LONG nFiles, 
                        _In_count_(nFiles) LPCSTR* lpFileNames,
                        _In_opt_z_ LPCSTR lpComment,
                        _In_ LONG dwFlags,
                        _In_opt_ LPCMDOPTS pvOptions
                        );

/*******************************************************************************
	Enables us to avoid prompting for user input more than once during a
	"batched" operation. SccBeginBatch and SccEndBatch are used as a pair
	to indicate the beginning and end of an operation. They cannot be nested.
	SccBeginBatch sets flag indicating that a batch operation is in progress.
*******************************************************************************/
SCCEXTERNC SCCRTN EXTFUN __cdecl SccBeginBatch(void);

/*******************************************************************************
	Enables us to avoid prompting for user input more than once during a
	"batched" operation. SccBeginBatch and SccEndBatch are used as a pair
	to indicate the beginning and end of an operation. They cannot be nested.
	SccEndBatch clears the batch operation in progress flag.
*******************************************************************************/
SCCEXTERNC SCCRTN EXTFUN __cdecl SccEndBatch(void);

/*******************************************************************************
	Renames the given file to a new name in the source control system.  The
	provider should not attempt to access the file on disk.  It is the
	caller's responsibility to rename the file on disk.
*******************************************************************************/
SCCEXTERNC SCCRTN EXTFUN __cdecl SccRename(
                        _Inout_ LPVOID pContext, 
                        _In_ HWND hWnd, 
                        _In_z_ LPCSTR lpFileName,
                        _In_z_ LPCSTR lpNewName
                        );

/*******************************************************************************
	Show the differences between the local users fully qualified file and the
	version under source control.
*******************************************************************************/
SCCEXTERNC SCCRTN EXTFUN __cdecl SccDiff(
                        _Inout_ LPVOID pContext, 
                        _In_ HWND hWnd, 
                        _In_z_ LPCSTR lpFileName, 
                        _In_ LONG dwFlags,
                        _In_opt_ LPCMDOPTS pvOptions
                        );

/*******************************************************************************
	Show the differences between the local user's fully qualified directory and
	the project under source control.
*******************************************************************************/
SCCEXTERNC SCCRTN EXTFUN __cdecl SccDirDiff(
                        _Inout_ LPVOID pContext, 
                        _In_ HWND hWnd, 
                        _In_z_ LPCSTR lpDirName, 
                        _In_ LONG dwFlags,
                        _In_opt_ LPCMDOPTS pvOptions
                        );

/*******************************************************************************
	Show the history for an array of fully qualified local file names.  The
	provider may not always support an array of files, in which case only the
	first files history will be shown.
*******************************************************************************/
SCCEXTERNC SCCRTN EXTFUN __cdecl SccHistory(
                        _Inout_ LPVOID pContext, 
                        _In_ HWND hWnd, 
                        _In_ LONG nFiles, 
                        _In_count_(nFiles) LPCSTR* lpFileNames, 
                        _In_ LONG dwFlags,
                        _In_opt_ LPCMDOPTS pvOptions
                        );

/*******************************************************************************
	Show the properties of a fully qualified file.  The properties are defined
	by the provider and may be different for each one.
*******************************************************************************/
SCCEXTERNC SCCRTN EXTFUN __cdecl SccProperties(
                        _Inout_ LPVOID pContext, 
                        _In_ HWND hWnd, 
                        _In_z_ LPCSTR lpFileName
                        );

/*******************************************************************************
	Examine a list of fully qualified files for their current status.  The
	return array will be a bitmask of SCC_STATUS_xxxx bits.  A provider may
	not support all of the bit types.  For example, SCC_STATUS_OUTOFDATE may
	be expensive for some provider to provide.  In this case the bit is simply
	not set.
*******************************************************************************/
SCCEXTERNC SCCRTN EXTFUN __cdecl SccQueryInfo(
                        _Inout_ LPVOID pContext, 
                        _In_ LONG nFiles, 
                        _In_count_(nFiles) LPCSTR* lpFileNames, 
                        _Out_cap_(nFiles) LPLONG lpStatus
                        );

/*******************************************************************************
	Examine a list of fully qualified dirs for their current status.  The
	return array will be a bitmask of SCC_DIRSTATUS_xxxx bits.  A provider may
	not support all of the bit types.  For example, SCC_DIRSTATUS_EMPTYPROJ may
	be expensive for some provider to provide.  In this case the bit is simply
	not set.
*******************************************************************************/
SCCEXTERNC SCCRTN EXTFUN __cdecl SccDirQueryInfo(
                        _Inout_ LPVOID pContext, 
                        _In_ LONG nDirs, 
                        _In_count_(nDirs) LPCSTR* lpDirNames, 
                        _Out_cap_(nDirs) LPLONG lpStatus
                        );

/*******************************************************************************
	Like SccQueryInfo, this function will examine the list of files for their
	current status.  In addition, it will use the pfnPopulate function to 
	notify the caller when a file does not match the critera for the nCommand.
	For example, if the command is SCC_COMMAND_CHECKIN, and a file in the list
	is not checked out, then the callback is used to tell the caller this.  
	Finally, the provider may find other files that could be part of the command
	and add them.  This allows a VB user to check out a .bmp file that is used
	by their VB project, but does not appear in the VB makefile.
*******************************************************************************/
SCCEXTERNC SCCRTN EXTFUN __cdecl SccPopulateList(
                        _Inout_ LPVOID pContext, 
                        _In_ enum SCCCOMMAND nCommand, 
                        _In_ LONG nFiles, 
                        _In_count_(nFiles) LPCSTR* lpFileNames, 
                        _In_ POPLISTFUNC pfnPopulate, 
                        _Inout_opt_ LPVOID pvCallerData,
						_Out_cap_(nFiles) LPLONG lpStatus, 
						_In_ LONG dwFlags
                        );

/*******************************************************************************
	Like SccPopulateList, this function will examine the list of directories and 
	will use the pfnPopulate function to notify the caller when a source control 
	project does not match the client's local directories.
	This function can be used to enumerate the subprojects of one project, 
	or to enumerate local folders that don't have matching projects, etc.
*******************************************************************************/
SCCEXTERNC SCCRTN EXTFUN __cdecl SccPopulateDirList(
                        _Inout_ LPVOID pContext, 
                        _In_ LONG nDirs, 
                        _In_count_(nDirs) LPCSTR* lpDirPaths, 
                        _In_ POPDIRLISTFUNC pfnPopulate, 
                        _Inout_opt_ LPVOID pvCallerData,
						_In_ LONG fOptions 
                        );

/*******************************************************************************
	Like SccQueryInfo, this function will examine the list of files for their
	current status and history.  
	For each file the provider will call back the IDE giving more infomration 
	about the file (e.g. file was deleted, or file was renamed to XXXX, or
	another file with the same name was already added, etc)
*******************************************************************************/
SCCEXTERNC SCCRTN EXTFUN __cdecl SccQueryChanges(
						_Inout_ LPVOID pContext, 
						_In_ LONG nFiles, 
						_In_count_(nFiles) LPCSTR *lpFileNames, 
						_In_ QUERYCHANGESFUNC pfnCallback,
						_Inout_opt_ LPVOID pvCallerData
						);

/*******************************************************************************
	SccGetEvents runs in the background checking the status of files that the
	caller has asked about (via SccQueryInfo).  When the status changes, it 
	builds a list of those changes that the caller may exhaust on idle.  This
	function must take virtually no time to run, or the performance of the 
	caller will start to degrade.  For this reason, some providers may choose
	not to implement this function.
*******************************************************************************/
SCCEXTERNC SCCRTN EXTFUN __cdecl SccGetEvents(
                        _Inout_ LPVOID pContext, 
                        _Out_z_cap_(MAX_PATH + 1) LPSTR lpFileName,
                        _Out_ LPLONG lpStatus,
                        _Out_ LPLONG pnEventsRemaining
                        );

/*******************************************************************************
	This function allows a user to access the full range of features of the
	source control system.  This might involve launching the native front end
	to the product.  Optionally, a list of files are given for the call.  This
	allows the provider to immediately select or subset their list.  If the
	provider does not support this feature, it simply ignores the values.
*******************************************************************************/
SCCEXTERNC SCCRTN EXTFUN __cdecl SccRunScc(
                        _Inout_ LPVOID pContext, 
                        _In_ HWND hWnd, 
                        _In_ LONG nFiles, 
                        _In_count_(nFiles) LPCSTR* lpFileNames
                        );

/*******************************************************************************
	This function will prompt the user for advaned options for the given
	command.  Call it once with ppvOptions==NULL to see if the provider
	actually supports the feature.  Call it again when the user wants to see
	the advaned options (usually implemented as an Advaned button on a dialog).
	If a valid *ppvOptions is returned from the second call, then this value
	becomes the pvOptions value for the SccGet, SccCheckout, SccCheckin, etc...
	functions.
*******************************************************************************/
SCCEXTERNC SCCRTN EXTFUN __cdecl SccGetCommandOptions(
                        _Inout_ LPVOID pContext, 
                        _In_ HWND hWnd, 
                        _In_ enum SCCCOMMAND nCommand,
                        _Deref_opt_out_opt_ LPCMDOPTS * ppvOptions
                        );

/*******************************************************************************
	SccGetUserOption is a generic function used to retrieve a variety of 
	user-specific options. Each option starts with SCC_USEROPT_xxx and has its own 
	defined set of values.
*******************************************************************************/
SCCEXTERNC SCCRTN EXTFUN __cdecl SccGetUserOption(
                        _Inout_ LPVOID pContext, 
                        _In_ LONG nOption,
                        _Out_ LPLONG lpVal 
						);

/*******************************************************************************
	This function allows the user to browse for files that are already in the
	source control system and then make those files part of the current project.
	This is handy, for example, to get a common header file into the current
	project without having to copy the file.  The return array of files
	(lplpFileNames) contains the list of files that the user wants added to
	the current makefile/project.
*******************************************************************************/
SCCEXTERNC SCCRTN EXTFUN __cdecl SccAddFromScc(
                        _Inout_ LPVOID pContext, 
                        _In_ HWND hWnd, 
                        _Out_ LPLONG pnFiles,
                        _Out_cap_(*pnFiles) LPCSTR** lplpFileNames
                        );

/*******************************************************************************
	SccSetOption is a generic function used to set a wide variety of options.
	Each option starts with SCC_OPT_xxx and has its own defined set of values.
*******************************************************************************/
SCCEXTERNC SCCRTN EXTFUN __cdecl SccSetOption(
                        _Inout_ LPVOID pContext,
                        _In_ LONG nOption,
                        _In_ LONG dwVal
                        );

/*******************************************************************************
	Creates a subproject with the given name under the existing parent project. 
	If a subproject with the name already exists the function can change 
	the default name in order to create a unique one, for example by adding 
	"_<number>" to it. The caller must be prepared to accept changes
	to lpUser, lpSubProjPath, and lpAuxProjPath. lpSubProjPath and
	lpAuxProjPath are then used in a call to SccOpenProject.  They should not
	be modified by the caller upon return.  The caller should avoid displaying
	these two parameters upon return, as the provider might use a formatted
	string that is not ready for view.
*******************************************************************************/
SCCEXTERNC SCCRTN EXTFUN __cdecl SccCreateSubProject(
                        _Inout_ LPVOID pContext, 
                        _In_ HWND hWnd, 
                        _Inout_z_ _Inout_cap_(SCC_USER_SIZE) LPSTR lpUser,
                        _In_z_ _In_count_(SCC_PRJPATH_SIZE) LPCSTR lpParentProjPath, 
                        _In_z_ _In_count_(SCC_PRJPATH_SIZE) LPCSTR lpSubProjName,
                        _Inout_z_ _Inout_cap_(SCC_PRJPATH_SIZE) LPSTR lpAuxProjPath,
                        _Out_z_cap_(SCC_PRJPATH_SIZE) LPSTR lpSubProjPath
                        );

/*******************************************************************************
	Returns parent path of the given project (the project must exist). 
	For the store root folder, the function should return the given path 
	(i.e. the same root folder path). The caller must be prepared to accept changes 
	to lpUser, lpParentProjPath, and lpAuxProjPath. lpParentProjPath and lpAuxProjPath 
	are then used in a call to SccOpenProject.  They should not be modified 
	by the caller upon return. The caller should avoid displaying these two parameters 
	upon return, as the provider might use a formatted string that is not ready for view.
*******************************************************************************/
SCCEXTERNC SCCRTN EXTFUN __cdecl SccGetParentProjectPath(
                        _Inout_ LPVOID pContext, 
                        _In_ HWND hWnd, 
                        _Inout_z_ _Inout_cap_(SCC_USER_SIZE) LPSTR lpUser,
                        _In_z_ _In_count_(SCC_PRJPATH_SIZE) LPCSTR lpProjPath, 
                        _Inout_z_ _Inout_cap_(SCC_PRJPATH_SIZE) LPSTR lpAuxProjPath,
                        _Out_z_cap_(SCC_PRJPATH_SIZE) LPSTR lpParentProjPath
                        );

/*******************************************************************************
	Checks if multiple checkouts on a file are allowed.
*******************************************************************************/
SCCEXTERNC SCCRTN EXTFUN __cdecl SccIsMultiCheckoutEnabled(
                        _Inout_ LPVOID pContext, 
			            _Out_ LPBOOL pbMultiCheckout
                        );

/*******************************************************************************
	Checks if the provider will create MSSCCPRJ.SCC files in the same
	directories as the given files if the given files are placed under source
	control. The file paths must be fully qualified.
*******************************************************************************/
SCCEXTERNC SCCRTN EXTFUN __cdecl SccWillCreateSccFile(
                        _Inout_ LPVOID pContext, 
                        _In_ LONG nFiles,
                        _In_count_(nFiles) LPCSTR* lpFileNames,
						_Out_cap_(nFiles) LPBOOL pbSccFiles
                        );

/*******************************************************************************
	Returns extra capabilities for the provider.
*******************************************************************************/
SCCEXTERNC SCCRTN EXTFUN __cdecl SccGetExtendedCapabilities(
                        _Inout_ LPVOID pContext, 
                        _In_ LONG   lSccExCap,
                        _Out_ LPBOOL pbSupported
			);

/*******************************************************************************
	Returns the list of files in the database different than local files.
*******************************************************************************/
SCCEXTERNC SCCRTN EXTFUN __cdecl SccEnumChangedFiles(
			_Inout_ LPVOID pContext,									  
			_In_ HWND hWnd, 
			_In_ LONG cFiles, 
			_In_count_(cFiles) LPCSTR* lpFileNames, 
			_Out_cap_(cFiles) LONG* plIsFileDifferent
			);

/*******************************************************************************
	Add the list of source controlled files to the currently open project.
*******************************************************************************/
SCCEXTERNC SCCRTN EXTFUN __cdecl SccAddFilesFromSCC(
			_Inout_ LPVOID pContext,									  
			_In_ HWND hWnd, 
			_Inout_z_ _Inout_cap_(SCC_USER_SIZE) LPSTR lpUser,
			_Inout_z_ _Inout_cap_(SCC_PRJPATH_SIZE) LPSTR lpAuxProjPath,
			_In_ LONG cFiles, 
			_In_count_(cFiles) LPCSTR* lpFilePaths,
			_In_z_ LPCSTR lpDestination,
			_In_opt_z_ LPCSTR lpComment,
			_Out_cap_(cFiles) LPBOOL pbResults	 
			);

/*******************************************************************************
	Removes the array of fully qualified files from the source control system.
	The files are not removed from the user's disk, unless advanced options
	are set by the user.  Advaned options are defined by the provider.
*******************************************************************************/
SCCEXTERNC SCCRTN EXTFUN __cdecl SccRemoveDir(
                        _Inout_ LPVOID pContext, 
                        _In_ HWND hWnd, 
                        _In_ LONG nDirs, 
                        _In_count_(nDirs) LPCSTR* lpDirNames, 
                        _In_opt_z_ LPCSTR lpComment,
                        _In_ LONG dwFlags,
                        _In_opt_ LPCMDOPTS pvOptions
                        );

#if defined( __cplusplus )
}
#endif





#endif // _SCC_DEFS

//******* EOF ********
