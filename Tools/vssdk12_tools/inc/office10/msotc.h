/****************************************************************************
	Msotc.h

	Owner: EricSchr
 	Copyright (c) 1997 Microsoft Corporation

	Declarations for functions related to TCO
****************************************************************************/

#pragma once

#ifndef MSOTC_H
#define MSOTC_H 1




/*------------------------------------------------------------------------
	MsoGetPathPreference

	0 - Default behavior (whatever was given)
	1 - Prefer Drive Letter
	2 - Prefer UNC name

---------------------------------------------------------------- AndrewH -*/
MSOAPI_(BYTE) MsoGetPathPreference();

enum
{
	msoNoPathPreference	= 0,
	msoPathPreferLetter	= 1,
	msoPathPreferUNC	= 2
};


#ifdef OFFICE10
// Stuff from tcpush.cpp cut from office9:

/*------------------------------------------------------------------------
	MsoFCheckForUpdate

	Check the registry for the product of szPID for a software update
	if returns true lpwzAbstract and lpwzInfoUrl must be deallocated unless
	MsoFAlertForUpdate is called (in which case they are deallocated there)
---------------------------------------------------------------- t-andreh -*/
MSOAPI_(BOOL) MsoFCheckForUpdate(char* szPID, LPWSTR *lpwzAbstract, LPWSTR *lpwzInfoUrl);

/*------------------------------------------------------------------------
	MsoFFAlertForUpdate

	Do an alert box with the agent prompting the user about a software
	update, using the given strings for the abstract and url to a web
	page with information.  
	The strings are deallocated by this function.
---------------------------------------------------------------- t-andreh -*/
MSOAPI_(BOOL) MsoFAlertForUpdate(char* szPID, LPWSTR *lpwzAbstract, LPWSTR *lpwzInfoUrl);

#endif // OFFICE10

/*---------------------------------------------------------------------------
	MsoAppendToPath

	Append a string, ensuring that there's the proper slash in between.
------------------------------------------------------------------ JJames -*/
MSOAPI_(void) MsoAppendToPath(const WCHAR *wzSub, WCHAR *wzPath);

/*-----------------------------------------------------------------------------
	MsoWzAfterPath

	Return a pointer after the last backslash, or the start if there is none.
------------------------------------------------------------------ JJames ---*/
MSOAPI_(WCHAR *) MsoWzAfterPath(const WCHAR *wzPathName);

/*-----------------------------------------------------------------------------
	MsoWzBeforeExt

	Return a pointer just before extension.
	Return NULL if there is no extension.
------------------------------------------------------------------ IgorZ ---*/
MSOAPI_(WCHAR *) MsoWzBeforeExt(const WCHAR *wzPathName);

/*-----------------------------------------------------------------------------
	MsoFFileExist

	Returns fTrue iff the file exists and is not a directory.
------------------------------------------------------------------ JJames ---*/
MSOAPI_(BOOL) MsoFFileExist(const WCHAR *wzFile);


/*---------------------------------------------------------------------------
	MsoFDirExist

	Return fTrue if and only if wzDir exists and is a directory.
---------------------------------------------------------------- EricSchr -*/
MSOAPI_(BOOL) MsoFDirExist(const WCHAR *wzDir);


/*---------------------------------------------------------------------------
	MsoFCreateFullDirectory

	Create directory wzDir, creating subdirectories as necessary.
	Return fTrue if successful.
---------------------------------------------------------------- EricSchr -*/
MSOAPI_(BOOL) MsoFCreateFullDirectory(const WCHAR *wzDir);


// MSOADF (MSO Application Data Folder)
// The subfolder under the App Data folder
typedef enum
	{
	msoadfFirst, msoadfMin = msoadfFirst, msoadfMinLessOne = msoadfMin - 1,
	
	msoadfMicrosoft,	// Application Data\Microsoft
	msoadfWord,			// msoadfMicrosoft\Word
	msoadfExcel,		// msoadfMicrosoft\Excel
	msoadfGraph,		// msoadfMicrosoft\Graph
	msoadfAccess,		// msoadfMicrosoft\Access
	msoadfOutlook,		// msoadfMicrosoft\Outlook
	msoadfPowerPoint,	// msoadfMicrosoft\PowerPoint
	msoadfOffice,		// msoadfMicrosoft\Office
	msoadfStartup,		// msoadfWord\Startup
	msoadfXlstart,		// msoadfExcel\Xlstart
	msoadfXlURL,		// msoadfExcel\URL
	msoadfAddins,		// msoadfMicrosoft\Addins
	msoadfQueries,		// msoadfMicrosoft\Queries
	msoadfProof,		// msoadfMicrosoft\Proof
	msoadfTemplates,	// msoadfMicrosoft\Templates
	msoadfRecentFiles,	// msoadfOffice\Recent Files
	msoadfActors,		// msoadfOffice\Actors
	msoadfThemes,		// msoadfMicrosoft\Themes
	msoadfOSB,			// msoadfOffice\OSB
	msoadfStationery,	// msoadfMicrosoft\Stationery
	msoadfSignatures,	// msoadfMicrosoft\Signatures
	msoadfPublisher,	// msoadfMicrosoft\Publisher
	msoadfDesigner,		// msoadfMicrosoft\Designer
	msoadfDesignerServers, // msoadfDesigner\My Servers
	msoadfDesignerPersonalFolders, // msoadfDesigner\Personal Folders
	msoadfMse, 			// msoadfMicrosoft\Mse

	msoadfMax, msoadfLast = msoadfMax - 1
	} MSOADF;


/*---------------------------------------------------------------------------
	MsoHrGetAppDataFolder

	Return the location of the user's Application Data folder in his/her
	profile.  It appends (and creates) the subdirectory specified by adf.
	'wz' is the buffer where the result will go.  'wz' should be at least
	MAX_PATH in size.  Use 'fCreate' to specify if the folder should be
	created if it doesn't exist already.  If fCreate is FALSE and the
	directory doesn't exist, return E_FAIL, otherwise return S_OK.
---------------------------------------------------------------- EricSchr -*/
MSOAPI_(HRESULT) MsoHrGetAppDataFolder(WCHAR *wz, MSOADF adf, BOOL fCreate);


/*---------------------------------------------------------------------------
	MsoHrGetCommonAppDataFolder

	Same as MsoHrGetAppDataFolder, but takes the All Users\Application Data
	directory as opposed to the <username>\Application Data directory.
------------------------------------------------------------------ DVierz -*/
MSOAPI_(HRESULT) MsoHrGetCommonAppDataFolder(WCHAR *wz, MSOADF adf, BOOL fCreate);

/*---------------------------------------------------------------------------
	MsoHrGetLocalAppDataFolder

	Same as MsoHrGetAppDataFolder, but returns the local (NON-roaming)
    [...\<username>\Local Settings\Application Data] directory as 
    opposed to the [...\<username>\Application Data] directory.
---------------------------------------------------------------- camerost -*/
MSOAPI_(HRESULT) MsoHrGetLocalAppDataFolder(WCHAR *wz, MSOADF adf, BOOL fCreate);

// behavioral flags
#define fadfCheckExist 0x01
#define fadfCreate     0x02
#define grfadfDefault  (fadfCheckExist | fadfCreate)

// specific folder type flags
#define fadftRoamingAppData  0
#define fadftCommonAppData   1
#define fadftLocalAppData    2

/*---------------------------------------------------------------------------
	MsoHrGetAppDataFolderEx
------------------------------------------------------- HAILIU / camerost -*/
MSOAPI_(HRESULT) MsoHrGetAppDataFolderEx(WCHAR *wz, MSOADF adf, DWORD grf, int adfType);

/*---------------------------------------------------------------------------
	MsoHrGetMyDocumentsFolder

	Return the location of the user's My Documents folder in their profile.
	'wz' is the buffer where the result will go.  It must be at least
	MAX_PATH in size.  Always return S_OK.
---------------------------------------------------------------- EricSchr -*/
MSOAPI_(HRESULT) MsoHrGetMyDocumentsFolder(WCHAR *wz);


/*---------------------------------------------------------------------------
	MsoHrGetFavoritesFolder

	Return the location of the user's Favorites folder in their profile.
	'wz' is the buffer where the result will go.  It must be at least
	MAX_PATH in size.  Always return S_OK.
---------------------------------------------------------------- EricSchr -*/
MSOAPI_(HRESULT) MsoHrGetFavoritesFolder(WCHAR *wz);


/*---------------------------------------------------------------------------
	MsoHrGetDesktopFolder

	Return the location of the user's Desktop folder in their profile.
	'wa' is the place where the result will go.  It must be at least
	MAX_PATH in size.  Always return S_OK.
---------------------------------------------------------------- EricSchr -*/
MSOAPI_(HRESULT) MsoHrGetDesktopFolder(WCHAR *wz);


/*---------------------------------------------------------------------------
	MsoFIsAppDataFolder

	Determine if 'wz' is the application data folder specified by 'msoadf'.
---------------------------------------------------------------- EricSchr -*/
MSOAPI_(BOOL) MsoFIsAppDataFolder(const WCHAR *wz, MSOADF adf);


/*---------------------------------------------------------------------------
	MsoFIsMyDocumentsFolder

	Determine if 'wz' is the My Documents folder.
---------------------------------------------------------------- EricSchr -*/
MSOAPI_(BOOL) MsoFIsMyDocumentsFolder(const WCHAR *wz);


/*---------------------------------------------------------------------------
	MsoFFileInAppDataFolder

	Determine if the non-leaf part of 'wz' is the app data folder specified
	by 'adf'.
---------------------------------------------------------------- EricSchr -*/
MSOAPIX_(BOOL) MsoFFileInAppDataFolder(const WCHAR *wz, MSOADF adf);
	

/*---------------------------------------------------------------------------
	MsoFStripAppDataFolder

	If the non-leaf path is the app data folder specified by 'adf',
	remove it.  Return fTrue if the removal occurs, fFalse otherwise.
---------------------------------------------------------------- EricSchr -*/
MSOAPI_(BOOL) MsoFStripAppDataFolder(WCHAR *wz, MSOADF adf);


/*---------------------------------------------------------------------------
	MsoFAddAppDataFolder

	If 'wz' is just a filename (determined by searching for a backslash),
	then prepend the app data folder specified by 'msoadf'.  Return fTrue if
	the folder is added, fFalse otherwise.  Do nothing and return fFalse
	if 'wz' is empty.
---------------------------------------------------------------- EricSchr -*/
MSOAPI_(BOOL) MsoFAddAppDataFolder(WCHAR *wz, MSOADF adf);


/*---------------------------------------------------------------------------
	MsoHrGetUserQueriesFolder

	Look in the registry for the Queries folder (can be set only by
	policy).  If not there, return the Queries folder from
	Application Data.  Return S_OK.
---------------------------------------------------------------- EricSchr -*/
MSOAPI_(HRESULT) MsoHrGetUserQueriesFolder(WCHAR *wz);


#ifdef LVP
// HwndForLVType is a callback function type to be implemented by the
// application.  If it is set, LV will call it to get an hwnd to parent
// off of instead of trying to figure it out.  If the function returns
// NULL, the LV code will not bring up the dialog but instead make
// the callback again later.
typedef HWND (CALLBACK *PfnHwndForLV)(void);
#endif	// LVP

// MSOLVP (MSO License Verification Property)
// Used with MsoSetLVProperty.
typedef enum
	{
	msolvpFirst, msolvpMin = msolvpFirst, msolvpMinLessOne = msolvpMin - 1,

	msolvpNoLV,		// Turns off License Verification, pv should be NULL.
	msolvpNoModal,	// Tells LV that can't go modal yet and when it can,
					// pv should be a BOOL.  Its primary purpose is to
					// prevent a modal dialog during OLE interactions.
					// Call with TRUE to disable any modal dialog, FALSE
					// to turn back on.  It is also used by the Gimme
					// layer (including Darwin dialogs).
	msolvpPfnHwnd,	// Passes to LV a callback function which it can
					// use to ask the app for a window to parent off
					// of. 'pv' should be a function of type PfnHwndForLV.

	msolvpMax, msolvpLast = msolvpMax - 1
	} MSOLVP;

/*---------------------------------------------------------------------------
	MsoSetLVProperty

	Use this function to set specific properties for License Verification to
	use.  See the MSOLVP declaration to determine what 'pv' should be.
---------------------------------------------------------------- EricSchr -*/
MSOAPI_(void) MsoSetLVProperty(MSOLVP lvp, void *pv);


#ifdef LVP
/*---------------------------------------------------------------------------
	MsoFDoLV

	For applications that don't participate in the component manager, they
	need a way to fire off the License Verification dialog.  This is a single
	API that they can call.  Note that a TRUE return value means that the
	application can run, while a FALSE return value means that the
	application needs to call again at a later time.  If the verification
	fails, there is no return value because the code will automatically
	exit.
---------------------------------------------------------------- EricSchr -*/
MSOAPIX_(BOOL) MsoFDoLV(void);
#endif	// LVP



/*---------------------------------------------------------------------------
	 MsoFixMeDlg
	
	 Bring up the Office FixMe dialog.
---------------------------------------------------------------- NancyDo -*/
MSOAPI_(HRESULT) MsoFixMeDlg(HMSOINST hinst, HWND hwndOwner);


/*---------------------------------------------------------------------------
	 MsoHrFixMe
	
	 Skip the Office FixMe dialog and just do the fixing, using the two
	 settings as indicated.
---------------------------------------------------------------- BrianHi -*/
MSOAPI_(HRESULT) MsoHrFixMe(BOOL fRestoreShortcuts, BOOL fResetSettings);



/****************************************************************************
	Office layer over Darwin (GimmeFile and friends).

	* DO NOT EVER CALL DARWIN DIRECTLY. ALWAYS GO THROUGH OFFICE. *
	All MsoGimme* APIs are provided for overall performance, extra resiliency,
	and for your convenience. Gimme(TM) is a trademark of KirkG.

	The structures below add up to a MSOTCFCF structure which you have
	to give to IMsoUser::FHookDarwinTables.

	This structure holds tables which describe to MsoFGimmeFile and
	associated APIs the structure of files, components, and features, as
	authored in Setup. From the app's point of view, instead of accessing
	files by name, you access them by fid (file id). Some files are
	known by Office and the id is built in (see msotcdar.h). Others used
	only by your app need to be hooked up to Office so that the Office
	code can apply the same resiliency rules as with its own files.

	The tcmsi.exe tool (in otools) takes as input files which describe the
	Setup database layout, and output two files:
	- a header, which lists fid's local to your app (and cid's for
		components, and ftid's for features).
	- a C file, which contains the MSOTCFCF structure for your app.

	Note that Office itself uses this tool to generate msotcdar.h (included
	below) and tcdar.inc (included in the bowels of Office code).

	Expect the internals of the MSOTCFCF structure to move around a lot
	as we figure out boot, string loading, etc.
****************************************************************************/


// this flag controls whether we build a single table with enums (1)
// or externs to individual structs per row (0)
#define MSOGIMME_INDEXIDS 1

typedef int msofidT;
typedef int msocidT;
typedef int msoftidT;

// must be in ssync with otcdarmake's output
#define msoidstcoGeneric 0

typedef enum
	{
	msotcidmin = 0,
	msofid = 0,
	msocid,
	msoftid,
	msoqcid,
	msoqfid,
	msotcidmax = msoqfid
	} MSOTCID;

typedef	enum { // default language
	msolangNone, 
	msolangInstall, 
	msolangUI, 
	msolangHelp,
	msolangInstallFlavor,
	msolangPreviousUI,
	msolangPreviousInstallFlavor
	} msolangT;

typedef struct _msotcfileinfo {
	CHAR *szFilename;		// filename
	msocidT cid;			// component ID
} MSOTCFILEINFO;

typedef struct _msotccomponentinfo {
	GUID msoguid;
	CHAR *szKeyFile;		// name of keyfile or static qualifier
	msoftidT ftid;			// feature ID, -1 if belongs to multiple features
	int idsInstall;			// string id, msoidstcoGeneric if none
	int idsRepair;			// TODO(JBelt): delete (otcdarmake, ssync with VB)
	unsigned langDefault : 2;
	unsigned fLcidQualified : 1;
	unsigned fFilenameQualified : 1;
	unsigned fOtherQualified : 1;
	unsigned fStaticQualified : 1;
} MSOTCCOMPONENTINFO;

typedef struct _msotcfeatureinfo {
	CHAR *szFtid;			// GUID
	msocidT qcid;			// publish component for cross-product features, -1 if none
	int idsInstall;			// string id, msoidstcoGeneric if none
	int idsRepair;			// TODO(JBelt): delete (otcdarmake, ssync with VB)
} MSOTCFEATUREINFO;

typedef struct _msotcclassinfo {
	CLSID clsid;	// GUID constant 
	DWORD dwClsCtx;	// Class Context
	msocidT cid;	// Feature ID;
} MSOTCCLASSINFO;

typedef struct _msotcbackdoorinfo {
	msocidT cid;
	void *pReserved;
	CHAR *szRelativePath;
	CHAR *szQualifier;
	CHAR *szAppData;
} MSOTCBACKDOORINFO;

#define MSOTCFILEINFO_NIL { "invalidFid", msocidNil }
#define MSOTCCOMPONENTINFO_NIL { {0,0,0,0}, "invalidCid", msoftidNil, -1 }
#define MSOTCFEATUREINFO_NIL { "invalidFtid", msoftidNil }
#define MSOTCCLASSINFO_NIL { {0,0,0,0}, 0, msocidNil }
#define MSOTCBACKDOORINFO_NIL { msocidNil, 0, "", "", "" }

typedef struct _msotcfcf {
	DWORD dwVersion;				// version
	int iTableIndex;
	const MSOTCFILEINFO *rgfi;		// file table
	const MSOTCCOMPONENTINFO *rgci;	// component table
	const MSOTCFEATUREINFO *rgfti;	// feature table
	const MSOTCBACKDOORINFO *rgbdi;	// backdoor data
	int cfi, cci, cfti, cbdi;		// counts
} MSOTCFCF;

/* Edit language info */
typedef struct _msoeli
	{
	UCHAR	fExplicit;
	LCID	lcid;
	}MSOELI;
	
// include file, component, and feature id's from Darwin
#include "msotcdar.h"

// Maximum length of a GUID string in the standard format,
// "{000Cxxxx-0000-0000-C000-000000000046}"
#define MAX_GUID 39


/*---------------------------------------------------------------------------
	dwGimmeFlags

	These flags specify options for the MsoFGimme*Ex functions below.
	Each functions always checks the install state of the object.
------------------------------------------------------------------ JJames -*/
#define msotcogfDemandInstall			0x0001	// install if not already
#define msotcogfSearchForFile			0x0002	// check file system if darwin fails
#define msotcogfTryOtherLanguages		0x0004	// try backup lcid's if requested one fails
#define msotcogfVerifyFileExists		0x0008	// verify that the requested file exists
#define msotcogfFixIfNecessary			0x0010	// call darwin fix functions if necessary
#define msotcogfForceFix				0x0020	// call darwin fix functions
#define msotcogfTrueIfAdvertised		0x0040	// return TRUE if the object is advertised
#define msotcogfForceFixMachineRegistry    0x0080	// force repair of user registry data
#define msotcogfNoInstallUI				0x0100	// no install confirmation UI (assume Yes)
#define msotcogfNoRepairUI				0x0200	// no repair confirmation UI (assume Yes)
#define msotcogfNoRetryUI				0x0400	// no retry on busy UI (assume Cancel)
#define msotcogfNoDisabledUI			0x0800	// no disabled feature UI (assume Ok)
#define msotcogfValidate				0x1000  // call MsiUseFeature instead of QueryFeature
#define msotcogfUninstall				0x2000  // change feature to advertised
#define msotcogfForceFixUserRegistry	0x4000  // force repair of machine registry data
#define msotcogfSearchFirst             0x8000  // check file system before querying Darwin (boot perf.)
#define msotcogfNoSourceDialog		   	0x10000  // suppress Darwin's source dialog
#define msotcogfNoCustomUI			   	0x20000  // Disable Office demandinstallUI dialog
#define msotcogfNoAutoApprove		   	0x40000  // Don't say "Yes" automatically
#define msotcogfNoAutoReject		   	0x80000  // Don't say "No" automatically
#define msotcogfForceUI				   0x100000  // Ignore the app callback
// reserved for Gimme internal     		0xF0000000

// preserve ForceFixRegistry option
#define msotcogfForceFixRegistry (msotcogfForceFixUserRegistry | msotcogfForceFixMachineRegistry)

#define msotcogfResiliency (msotcogfSearchForFile | msotcogfTryOtherLanguages)
	
// test whether the associated feature is enabled for install
#define msotcogfEnabled (msotcogfTrueIfAdvertised | msotcogfResiliency)

// test whether the file or component is already installed on the machine
#define msotcogfInstalled (msotcogfResiliency)
#define msotcogfInstalledNoResiliency (0)

// request the file or component, installing if necessary
#define msotcogfProvide (msotcogfDemandInstall | msotcogfResiliency)

// do whatever it takes to get the file or component
#define msotcogfRequired (msotcogfDemandInstall | msotcogfResiliency | msotcogfVerifyFileExists | msotcogfFixIfNecessary)
#define msotcogfRequiredNoResiliency (msotcogfDemandInstall | msotcogfVerifyFileExists | msotcogfFixIfNecessary)

// don't display any UI
// TODO(JBelt): merge all UI flags into one?
#define msotcogfQuiet (msotcogfNoInstallUI | msotcogfNoRepairUI | msotcogfNoRetryUI | msotcogfNoDisabledUI)

// don't automatically approve or deny an install or repair
#define msotcogfNoAutoReponse (msotcogfNoAutoApprove | msotcogfNoAutoReject)

/*---------------------------------------------------------------------------
	MsoFGimmeFeatureEx

	Perform an operation on a Darwin feature, as specified in
	%otools%\inc\misc\tcinuse.txt.  Will demand install, search, fix, check 
	advertisement, etc. according to options in dwGimmeFlags.

	Returns TRUE on success, which usually means the feature is installed and 
	enabled.  However, some flags affect this return.

	Use the Wz version only when absolutely necessary.
------------------------------------------------------------------ JJames -*/
MSOAPI_(BOOL) MsoFGimmeFeatureEx(msoftidT ftid, DWORD dwGimmeFlags);
MSOAPI_(BOOL) MsoFGimmeFeatureExWz(const WCHAR *wzFeature, DWORD dwGimmeFlags);

#define MsoFGimmeFeature(ftid) MsoFGimmeFeatureEx(ftid, msotcogfProvide)
#define _MsoFGimmeFeature(wzFeature) MsoFGimmeFeatureExWz(wzFeature, msotcogfProvide)
#define MsoFEnabledFeature(ftid) MsoFGimmeFeatureEx(ftid, msotcogfEnabled)
#define _MsoFEnabledFeature(wzFeature) MsoFGimmeFeatureExWz(wzFeature, msotcogfEnabled)
#define MsoFInstalledFeature(ftid) MsoFGimmeFeatureEx(ftid, msotcogfInstalled)
#define _MsoFInstalledFeature(wzFeature) MsoFGimmeFeatureExWz(wzFeature, msotcogfInstalled)
#define MsoFFixFeature(ftid) MsoFGimmeFeatureEx(ftid, msotcogfForceFix)

/*---------------------------------------------------------------------------
	MsoFGimmeComponentEx

	Returns a full pathname to the component keyfile specified by the component
	id with possible language and string qualification according to the cid 
	specification found in %otools%\inc\misc\tcinuse.txt.  Will demand install,
	search, fix, check advertisement, etc. according to options in dwGimmeFlags.

	Since components don't always have keyfiles (or they can be misauthored),
	you should generally use MsoFGimmeFileEx if you are relying on the pathname return.
	
	wzPath must be NULL or MAX_PATH characters in size.
	Returns TRUE on success, which usually means the component is installed and 
	enabled.  However, some flags affect this return.
------------------------------------------------------------------ JJames -*/
MSOAPI_(BOOL) MsoFGimmeComponentEx(msocidT cid, LCID lcid, const WCHAR *wzQualifier, WCHAR *wzPath, DWORD dwGimmeFlags);

#define MsoFGimmeComponent(cid, wzPath) MsoFGimmeComponentEx(cid, 0, NULL, wzPath, msotcogfProvide)
#define MsoFGimmeComponentQualified(cid, wzQualifier, wzPath) MsoFGimmeComponentEx(cid, 0, wzQualifier, wzPath, msotcogfProvide)
#define MsoFGimmeLocalizedComponent(cid, lcid, wzPath) MsoFGimmeComponentEx(cid, lcid, NULL, wzPath, msotcogfProvide)
#define MsoFFixLocalizedComponent(cid, lcid, wzPath) MsoFGimmeComponentEx(cid, lcid, NULL, wzPath, msotcogfForceFix)

/*---------------------------------------------------------------------------
	MsoFGimmeFileEx
	
	Returns a full pathname to the file specified by the file id with possible
	language and string qualification according to the fid specification found
	in %otools%\inc\misc\tcinuse.txt.  Will demand install,	search, fix, check 
	advertisement, etc. according to options in dwGimmeFlags.
	
	wzPath must be NULL or MAX_PATH characters in size.
	Returns TRUE on success, which usually means the file is installed and 
	enabled.  However, some flags affect this return.
------------------------------------------------------------------ JJames -*/
MSOAPI_(BOOL) MsoFGimmeFileEx(msofidT fid, LCID lcid, const WCHAR *wzQualifier, WCHAR *wzPath, DWORD dwGimmeFlags);

#define MsoFGimmeFile(fid, wzPath) MsoFGimmeFileEx(fid, 0, NULL, wzPath, msotcogfProvide)
#define MsoFGimmeFileQualified(fid, wzQualifier, wzPath) MsoFGimmeFileEx(fid, 0, wzQualifier, wzPath, msotcogfProvide)
#define MsoFGimmeLocalizedFile(fid, lcid, wzPath) MsoFGimmeFileEx(fid, lcid, NULL, wzPath, msotcogfProvide)
#define MsoFGimmeAdvertisedFile(qcid, wzFilename, wzPath, fDemandInstall) MsoFGimmeComponentEx(qcid, 0, wzFilename, wzPath, \
	(fDemandInstall) ? msotcogfProvide : msotcogfInstalled)
#define MsoFFixFile(fid, wzPath) MsoFGimmeFileEx(fid, 0, NULL, wzPath, msotcogfForceFix)
#define MsoFEnabledFile(fid) MsoFGimmeFileEx(fid, 0, NULL, NULL, msotcogfEnabled)
#define MsoFInstalledFile(fid) MsoFGimmeFileEx(fid, 0, NULL, NULL, msotcogfInstalled)

/*-----------------------------------------------------------------------------
	MsoFGimmeFileVersion

	In addition to grabbing file (modeled after MsoFGimmeFileFull), this checks 
	version of the file that we get and calls a more agressive repair if the
	version is less that the one expected.
  
	*** Inherited from MsoFGimmeFileFull ***
	Returns a full pathname to the file specified by the file id with possible
	language and string qualification according to the fid specification found
	in %otools%\inc\misc\tcinuse.txt.  Will demand install, search, fix, check 
	advertisement, etc. according to options in dwGimmeFlags.

	wzPath must be NULL or MAX_PATH characters in size.
	Returns the language used in *plcid.
	*** Inherited from MsoFGimmeFileFull ***

	Returns TRUE on success, which usually means the file is installed, enabled,
	and at least the version requested.  However, some flags affect this return.
  
---------------------------------------------------------------- RFlaming ---*/
MSOAPI_(BOOL) MsoFGimmeFileVersion(msofidT fid, LCID lcid, const WCHAR *wzQualifier,
	WCHAR *wzPath, DWORD dwGimmeFlags, DWORD dwTargetVersionMS, DWORD dwTargetVersionLS);


/*----------------------------------------------------------------------------
	MsoFGimmeComponentPathEx

	Thin wrapper around MsoFGimmeComponent, which strips out the keyfile if
	there is one.
-------------------------------------------------------------------- JBelt --*/
MSOAPI_(BOOL) MsoFGimmeComponentPathEx(msocidT cid, LCID lcid, const WCHAR *wzQualifier, WCHAR *wzPath, DWORD dwGimmeFlags);
#define MsoFGimmeComponentPath(cid, wzPath) MsoFGimmeComponentPathEx(cid, 0, NULL, wzPath, msotcogfProvide)


/*---------------------------------------------------------------------------
	MsoFEnumComponentQualifiers

	Enumerate the qualifiers advertised under this qcid.
	Begin with iIndex = 0 and increase until the function returns FALSE.
	String args except wzQualifier can be NULL.
	Non-NULL string args must be at least MAX_PATH characters long.
	Can pass qcid and wzQualifier to MsoFGimmeComponentQualified.
	Returns FALSE when there are no more to be had.
---------------------------------------------------------------- JJames -*/
MSOAPI_(BOOL) MsoFEnumComponentQualifiers(msocidT qcid, DWORD iIndex, WCHAR *wzQualifier, 
	WCHAR *wzAppData);


/*---------------------------------------------------------------------------
	MsoFEnumGraphicFilters

	Specialized graphic filter enumeration routine.

	Input:
	- qcid: qualified component to enumerate
	- piIndex: fill with 0 prior to initial call

	Output (all strings must be 256 chars, including terminator)
	- piIndex: incremented internally, do not modify
	- wzClass: class name
	- wzName: friendly display name
	- wzDarwinPath: Gimme token representing the path. Give this path to
		MsoFGimmeComponentQcidQualifierEx to get the real path, and possibly
		install on demand. This string is guaranteed to start with '{'.
	- wzExtensions: extensions, separated by spaces. No lowercase / uppercase
		assumptions can be made.
	- pgfo: bit field representing graphic filter options. Use msogfoxxx flags
		below. May be NULL.
-------------------------------------------------------------------- JBelt --*/
#define msogfoShowOptionsDialog  0x0001
#define msogfoShowProgressDialog 0x0002
MSOAPI_(BOOL) MsoFEnumGraphicFilters(msocidT qcid, int *piIndex,
	WCHAR *wzClass, WCHAR *wzName, WCHAR *wzDarwinPath, WCHAR *wzExtensions,
	DWORD *pgfo);


/*---------------------------------------------------------------------------
	MsoEnumComponentQualifiersEx

	Enumerate the qualifiers advertised under this qcid.
	lcid identifies a language for doubly qualified components and should
	be the same throughout a sequence of calls.	wzAppData can be null. 
	Begin with *pdwIterator = 0, the function will increment on success, 
	possibly by more than +1.

	Pass the lcid and wzQualifier to MsoFGimmeComponentQualifiedEx to
	retrieve the component.
---------------------------------------------------------------- JJames -*/
MSOAPI_(UINT) MsoEnumComponentQualifiersEx(
	msocidT qcid,         // gimme id
	LCID lcid,            // language id for double-qualified components, 0 if not
	WCHAR *wzQualifier,   // buffer for to receive qualifier
	DWORD *pcchQualifier, // pointer to size of buffer, receives resulting size
	WCHAR *wzAppData,     // buffer to receive application data (can be NULL)
	DWORD *pcchAppData,   // pointer to size of buffer, receives resulting size
	DWORD *pdwIterator);  // internally incremented iterator 

#if 0
MSOAPI_(UINT) MsoEnumComponentQualifiersExEx(
	msocidT qcid,         // gimme id
	LCID lcid,            // language id for double-qualified components, 0 if not
	WCHAR *wzQualifier,   // buffer for to receive qualifier
	DWORD *pcchQualifier, // pointer to size of buffer, receives resulting size
	WCHAR *wzAppData,     // buffer to receive application data (can be NULL)
	DWORD *pcchAppData,   // pointer to size of buffer, receives resulting size
	DWORD *pdwIterator,   // internally incremented iterator 
	DWORD fAnsiCPConversion);
#endif

/*---------------------------------------------------------------------------
	MsoFGimmeComponentQualifiedData

	Get the wzAppData field for this qualified component.
	Returns TRUE and sets wzAppData if the the qualifier was found.
------------------------------------------------------------------ JJames -*/
MSOAPI_(BOOL) MsoFGimmeComponentQualifiedData(msocidT qcid, const WCHAR *wzQualifier, WCHAR *wzAppData);

/*---------------------------------------------------------------------------
	MsoFGimmeAdvertisedName

	Returns the filename qualifier for a given pathname, verifying that the
	darwin entry for that qualifier is installed at that path.
	Reverse of MsoFGimmeAdvertisedFile.
	Returns TRUE if the path proved to be a darwin aware file.
------------------------------------------------------------------ JJames -*/
MSOAPIX_(BOOL) MsoFGimmeAdvertisedName(msocidT qcid, const WCHAR *wzPath, WCHAR *wzQualifier);

/*---------------------------------------------------------------------------
	MsoFGimmeProductCode

	Copies the 39 character product code into wzPath if true is returned
------------------------------------------------------------------ AndrewH -*/
MSOAPI_(BOOL) MsoFGimmeProductCode(WCHAR *wzPath);


/*-----------------------------------------------------------------------------
	MsoFGimmeOleServer

	Demand load a server based on an OLE object.
------------------------------------------------------------------ JJames ---*/
MSOAPI_(BOOL) MsoFGimmeOleServer(IOleObject *pOleObject, DWORD dwGimmeFlags);


/*----------------------------------------------------------------------------
	MsoFidToFilename

	Returns the filename corresponding to qcid. The filename can be NULL.
------------------------------------------------------------------ JJames --*/
MSOAPI_(VOID) MsoFidToFilename(msofidT fid, WCHAR *wzFilename);
MSOAPI_(VOID) MsoCidToFilename(msocidT cid, WCHAR *wzFilename);

#define MsoQfidToFilename MsoFidToFilename
#define MsoQcidToFilename MsoCidToFilename


/*----------------------------------------------------------------------------
	MsoFidToGuid

	Returns the GUID corresponding to the component the file belongs to.
	wzGuid must should be at least MAX_GUID characters long (39)
-------------------------------------------------------------------- JBelt --*/
MSOAPIX_(void) MsoFidToGuid(msofidT fid, WCHAR *wzGuid);

/*----------------------------------------------------------------------------
	MsoFindFid

	Returns the fid corresponding to a known filename.
	Returns msofidNil if not found.
	These should be used ONLY when filenames are given from outside sources.
------------------------------------------------------------------ JJames --*/
MSOAPIX_(msofidT) MsoFindFid(const WCHAR *wzFile);

#define MsoFindQfid MsoFindFid

/*----------------------------------------------------------------------------
	MsoFindFidInList

	Returns one of the fid's in the msofidNil terminated list according
	to equivalent filenames.  Returns msofidNil if not found.
	This should be used ONLY when filenames are given from outside sources.
------------------------------------------------------------------ JJames --*/
MSOAPI_(msofidT) MsoFindFidInList(const WCHAR *wzFile, msofidT *pfid);


/*----------------------------------------------------------------------------
	MsoFFirstRun

	Performs Office first run if necessary. Call this only if your app has
	already detected that *it* needs to do a first run. This saves one boot
	registry lookup. If this returns FALSE, you must refuse to boot.
------------------------------------------------------------------- JBelt --*/
MSOAPI_(BOOL) MsoFFirstRun(HMSOINST hinst);


/*----------------------------------------------------------------------------
	MsoFReinstallProduct

	Reinstall the product.
-------------------------------------------------------------------- KirkG --*/
MSOAPIX_(BOOL) MsoFReinstallProduct(void);

/*---------------------------------------------------------------------------
	MsoFGetUserInfo

	Returns name, company, and serial number (CD key). Each string must be at
	as long msocch[Username|UserInitials|Company|Serial]Max. Pass in NULL if
	not interested in a particular string.
------------------------------------------------------------------- JBelt -*/
MSOAPI_(BOOL) MsoFGetUserInfo(WCHAR *wzName, WCHAR *wzInitials,
	WCHAR *wzCompany, WCHAR *wzSerial);


// string size limits, not including null terminator
#define msocchUsernameMax		52
#define msocchUserInitialsMax	9
#define msocchCompanyMax		52
#define msocchSerialMax			23	// RPCNO-LOC-SERIALX-SEQNC

// for compatibility
#define cbCDUserNameMax 		msocchUsernameMax
#define cbCDOrgNameMax  		msocchCompanyMax
#define cbFormattedPID  		msocchSerialMax


/*---------------------------------------------------------------------------
	MsoLGetProductInfo

	An Office wrapper around MsiGetProductInfoW().
---------------------------------------------------------------- EricSchr -*/
MSOAPIX_(LONG) MsoLGetProductInfo(const WCHAR *wzProperty,
	WCHAR *wzValueBuf, DWORD *pcchValueBuf);


/*-----------------------------------------------------------------------------
	MsoLoadLocalizedLibraryFull

	LoadLibraryEx's the file fid and language plcid (if non NULL).

	If dwFlags is zero, does the equivalent of a simple LoadLibrary.

	If wzFullPath is non NULL, returns the path of the module loaded (max
	length MAX_PATH + null char).
------------------------------------------------------------------- JBelt ---*/
MSOAPI_(HMODULE) MsoLoadLocalizedLibraryFull(msofidT fid, LCID *plcid,
	const DWORD dwFlags, WCHAR *wzFullPath);


/*-----------------------------------------------------------------------------
	MsoLoadLocalizedLibraryEx

	Thin wrapper around MsoLoadLocalizedLibraryFull.
------------------------------------------------------------------- JBelt ---*/
MSOAPI_(HMODULE) MsoLoadLocalizedLibraryEx(msofidT fid, LCID lcid, DWORD dwFlags);

#define MsoLoadLibrary(fid) MsoLoadLocalizedLibraryEx(fid, 0, 0)
#define MsoLoadLibraryEx(fid, dwFlags) MsoLoadLocalizedLibraryEx(fid, 0, dwFlags)
#define MsoLoadLocalizedLibrary(fid, lcid) MsoLoadLocalizedLibraryEx(fid, lcid, 0)


/*----------------------------------------------------------------------------
	MsoWzEncodeQcidQualifier

	From a qcid and a qualifier, fills wzQC with {Qcid\Qualifier. This lets
	you store both inside a single string. wzQC must be at least 256
	characters long. Returns a pointer to the end of the string.
-------------------------------------------------------------------- JBelt --*/
MSOAPI_(WCHAR *) MsoWzEncodeQcidQualifier(msocidT qcid, WCHAR *wzQualifier, WCHAR *wzQC);


/*----------------------------------------------------------------------------
	MsoFGimmeComponentQcidQualifierEx

	Decodes the qcid and qualifier encoded in wzQC, and calls
	MsoFGimmeComponentQualifier on the results. wzPath must be at least
	MAX_PATH+1 characters long.
-------------------------------------------------------------------- JBelt --*/
MSOAPI_(BOOL) MsoFGimmeComponentQcidQualifierEx(WCHAR *wzQC, WCHAR *wzPath,
	DWORD dwGimmeFlags);

#define MsoFGimmeComponentQcidQualifier(wzQC, wzPath) MsoFGimmeComponentQcidQualifierEx(wzQC, wzPath, msotcogfProvide)


/*----------------------------------------------------------------------------
	MsoFGimmeComponentQcidQualifierDp

	Wrapper around MsoFGimmeComponentQcidQualifierEx, which also takes into
	account the dp variable passed in (see msotcodpxxx below). Used in loops,
	to carry the user choice to the first Gimme prompt through all subsequent
	prompts.
-------------------------------------------------------------------- JBelt --*/
MSOAPI_(BOOL) MsoFGimmeComponentQcidQualifierDp(WCHAR *wzQC, WCHAR *wzPath,
	int *pdp);

#define msotcodpNotAsked    0
#define msotcodpInstall     1
#define msotcodpDontInstall 2


/*-----------------------------------------------------------------------------
	MsoForgetLastGimme

	The Gimme API automatically approves or rejects subsequent calls
	within 10 seconds and before the event monitor picks up a user action.
	Successful installs turn on automatic approval, failures and user-cancels
	turn on automatic rejection.  This function resets that memory and should
	be used in cases where events don't get to the monitor.
------------------------------------------------------------------ JJames ---*/
MSOAPI_(void) MsoForgetLastGimme(void);


/*----------------------------------------------------------------------------
	MsoFFormatMessage

	Formats an appropriate error message for the last Gimme error.
	Returns FALSE if unknown.
	NOT REALLY USED.
------------------------------------------------------------------ JJames --*/
MSOAPI_(BOOL) MsoFFormatMessage(DWORD dwError, WCHAR *wzMessage);


/*------------------------------------------------------------------------
	MsoLaunchFid

	Given a FID for a file that represents a setup-installed EXE,
	ShellExec it and return the hinstance.  Pases arguments specified
	in character string, if any (may be NULL)
---------------------------------------------------------------- MikeKell -*/
MSOAPI_(HINSTANCE) MsoLaunchFid(msofidT fid, const WCHAR *wzArguments, int sw);


/*------------------------------------------------------------------------
	MsoFLaunchMsInfo

	Launches MSInfo.  Does some extra checking like bringing it forward
	if if is already there.  wzArguments should contain the name of the
	application invoking MSInfo.
---------------------------------------------------------------- MikeKell -*/
MSOAPI_(BOOL) MsoFLaunchMsInfo(const WCHAR *wzArguments);


/*----------------------------------------------------------------------------
	MsoFEnsureUserData

	If the registry referenced by rid than 1, ensures the user data in ftid
	is on the machine, then writes 1 in the registry. Call this after initing
	ORAPI, but before reading anything, to make sure Setup-time user data has
	been written for this particular user.
-------------------------------------------------------------------- JBelt --*/
MSOAPI_(BOOL) MsoFEnsureUserData(int rid, msoftidT ftid);


/*----------------------------------------------------------------------------
	MsoFEnsureTypelib

	1) Detect Mso Typelib key existence: 
	[HKEY_CLASSES_ROOT\TypeLib\{2DF8D04C-5BFA-101B-BDE5-00AA0044DE52}\2.2\0\win32]
    2) call Darwin to repair the feature that contains the typelib (ProductFiles)
	Note: Call this after initing ORAPI and Gimme. 
---------------------------------------------------------------- (EricLam) --*/
MSOAPI_(BOOL) MsoFEnsureMsoTypelib();


/*----------------------------------------------------------------------------
	MsoFInGimme

	Returns TRUE if we're currently stuck in a potentially long Darwin call.
	Be patient if you get called in a message filter and this returns TRUE.
	Fix Office 9 24103.
-------------------------------------------------------------------- JBelt --*/
MSOAPI_(BOOL) MsoFInGimme();


/*----------------------------------------------------------------------------
	MsoDwGetGimmeTableVersion

	Returns the version number of the passed-in Gimme table.
-------------------------------------------------------------------- JBelt --*/
MSOAPIX_(DWORD) MsoDwGetGimmeTableVersion(MSOTCFCF *pfcf);


/*------------------------------------------------------------------------
	MsoFIsOLEAwareDarwin

	Returns TRUE if OS recognizes Darwin descriptors in the OLE registry.
---------------------------------------------------------------- WesYang -*/
MSOAPIX_(BOOL) MsoFIsOLEAwareDarwin ();


/*------------------------------------------------------------------------
	Obsolete Gimme wrapper to the standard OLE calls
---------------------------------------------------------------- JJames -*/

#define MsoFGimmeCoCreateInstance(rclsid,pUnkOuter,dwClsContext,riid,ppv) \
		         CoCreateInstance(rclsid,pUnkOuter,dwClsContext,riid,ppv)
#define MsoFGimmeCoGetClassObject(rclsid,dwClsContext,pServerInfo,riid,ppv) \
		         CoGetClassObject(rclsid,dwClsContext,pServerInfo,riid,ppv)
#define MsoHrCoCreateInstance(rclsid,pUnkOuter,dwClsContext,riid,ppv) \
		     CoCreateInstance(rclsid,pUnkOuter,dwClsContext,riid,ppv)
#define MsoHrCoGetClassObject(rclsid,dwClsContext,pServerInfo,riid,ppv) \
		     CoGetClassObject(rclsid,dwClsContext,pServerInfo,riid,ppv)
#define MsoHrOleCreate(rclsid,rrid,renderopt,pFormatEtc,pClientSite,pStg,ppvObject) \
		     OleCreate(rclsid,rrid,renderopt,pFormatEtc,pClientSite,pStg,ppvObject)
#define MsoHrOleCreateLink(pmkLinkSrc,rrid,renderopt,pFormatEtc,pClientSite,pStg,ppvObj) \
		     OleCreateLink(pmkLinkSrc,rrid,renderopt,pFormatEtc,pClientSite,pStg,ppvObj)
#define MsoHrOleCreateFromFile(rclsid,lpszFileName,rrid,renderopt,pFormatEtc,pClientSite,pStg,ppvObject) \
		     OleCreateFromFile(rclsid,lpszFileName,rrid,renderopt,pFormatEtc,pClientSite,pStg,ppvObject)
#define MsoHrOleCreateLinkToFile(lpszFileName,rrid,renderopt,pFormatEtc,pClientSite,pStg,ppvObject) \
		     OleCreateLinkToFile(lpszFileName,rrid,renderopt,pFormatEtc,pClientSite,pStg,ppvObject)
#define MsoHrOleCreateFromData(pSrcDataObj,rrid,renderopt,pFormatEtc,pClientSite,pStg,ppvObject) \
		     OleCreateFromData(pSrcDataObj,rrid,renderopt,pFormatEtc,pClientSite,pStg,ppvObject)
#define MsoHrOleCreateStaticFromData(pSrcDataObj,rrid,renderopt,pFormatEtc,pClientSite,pStg,ppvObject) \
		     OleCreateStaticFromData(pSrcDataObj,rrid,renderopt,pFormatEtc,pClientSite,pStg,ppvObject)
#define MsoHrOleCreateLinkFromData(pSrcDataObj,rrid,renderopt,pFormatEtc,pClientSite,pStg,ppvObject) \
		     OleCreateLinkFromData(pSrcDataObj,rrid,renderopt,pFormatEtc,pClientSite,pStg,ppvObject)
#define MsoHrOleLoad(pStg,riid,pClientSite,ppvObj) \
		     OleLoad(pStg,riid,pClientSite,ppvObj)
#define MsoHrOleLoadFromStream(pStm,iidInterface,ppvObj) \
		     OleLoadFromStream(pStm,iidInterface,ppvObj)

                            
/*---------------------------------------------------------------------------
	IMsoGimmeUser callback for Gimme clients.
------------------------------------------------------------------- JBelt -*/

#undef  INTERFACE
#define INTERFACE  IMsoGimmeUser

#define	cchMaxMsoGimmeUserFGetString	300	// maximum length of returned string
											// from IMsoGimmeUser::FGetString
#define GIMMEUSER_INSTALL	0x01	// ok to install
#define GIMMEUSER_GIMMEUI	0x02	// ok to display Gimme UI (install prompt, etc)
#define GIMMEUSER_DARWINUI	0x04	// ok to display Darwin UI
#define GIMMEUSER_DEFAULT   (GIMMEUSER_INSTALL | GIMMEUSER_GIMMEUI | GIMMEUSER_DARWINUI)

DECLARE_INTERFACE(IMsoGimmeUser)
{

	/* Debugging interface for this interface */
	MSODEBUGMETHOD

	/* If Darwin is on the machine, but your app doesn't have a product code,
		Office will look for a MSI to install on the fly, and call you.
		If you return FALSE, Office immediately gives up looking for a MSI.
		If you return TRUE, Office will look for the MSI.
		- If you set *pfPattern to TRUE, wtzPattern is assumed to be a file
			pattern (with wildcards). If you set *pfPattern to FALSE, wtzPattern
			is assumed to be a fully qualified path to the file, and is
			used as is.
		- Office prefills wtzPattern with a suggested search pattern, and
			pfPattern with TRUE. */
	MSOMETHOD_(BOOL, FSearchMSI) (THIS_ WCHAR *wtzPattern, BOOL *pfPattern) PURE;

	/* Gimme is ready to demand install something. Return a combination of
		GIMMEUSER_xxx flags declared above, or GIMMEUSER_DEFAULT. */
	MSOMETHOD_(DWORD, DwInstallBehavior) (THIS) PURE;

	/* The Gimme layer needs the string corresponding to the string identifier
		corresponding to an id you have specified in your tcinuse.txt. Fill
		wtz with the string and return TRUE; return FALSE if unable to fill
		the string, and Gimme will default to a generic string.  Note the
		buffer is a wtz, i.e. the first entry is the length of the string,
		not including the terminating null.  The size of the buffer is
		cchMaxMsoGimmeUserFGetString.  */
	MSOMETHOD_(BOOL, FGetString) (THIS_ int ids, WCHAR *wtz) PURE;

	/* Return the directory in which most of your files live (your main, or bin,
		directory), and TRUE. The Gimme layer uses this directory for search for
		files for dev override (oprep machines), and resiliency (if Darwin is
		dead, there's a MSI mismatch, etc). Office prefills wtzDir with the
		directory where the EXE which launched the process lives. wtzDir is
		MAX_PATH+1 long. Returning FALSE turns off override / resiliency.
		Hint: use MsoGetModuleFilenameW to get your main DLL / EXE path. */
	MSOMETHOD_(BOOL, FGetRootDirectory) (THIS_ WCHAR *wtzDir) PURE;
};

/*----------------------------------------------------------------------------
	MsoDwGimmeUserInstallBehavior

	This code generates the behavior flags needed for DarwinOK
------------------------------------------------------------------- ARSHADA --*/
MSOAPI_(DWORD) MsoDwGimmeUserInstallBehavior(WORD FeatureInstall, BOOL fDisplayAlerts);

// OBSOLETE, DO NOT CALL. Use MsoDwGimmeUserInstallBehavior instead
// TODO(JBelt): delete, ssync with VB
MSOAPIX_(BOOL) MsoFGimmeUserInstallBehavior(WORD FeatureInstall, BOOL fDisplayAlerts, WORD *pwFlag);


/*---------------------------------------------------------------------------
	MsoInitGimme

	Hook up your own Gimme tables, if you have any. If you don't, you don't
	need to call this API.
	
	cidCore is a Gimme component ID to your core component.  It must be given.
	pGimmeTables is build by otcdarmake.exe as "vfcf" and can be NULL.
	If your cid is not in the Office tables, then you must build your own
	using otcdarmake.exe from a version of %otools%\inc\misc\tcinuse.txt.
	See http://officedev/tco/gimmehelp.htm for details.

	See IMsoGimmeUser above for description of pigu. You must implement this
	interface, or some Darwin features will be disabled.
------------------------------------------------------------------- JBelt -*/
MSOAPI_(void) MsoInitGimmeEx(msocidT cidCore, MSOTCFCF *pGimmeTables,
		IMsoGimmeUser *pigu, msocidT cidFull, DWORD dwUnused);
MSOAPI_(void) MsoInitGimme(msocidT cidCore, MSOTCFCF *pGimmeTables,
	IMsoGimmeUser *pigu);


// 
// This enum list which is used for the MsoGimmePublishComponentString() API
// below is to be mapped with the %OTOOLS%\inc\misc\msistr.pp list of
// string identifiers.  This list is on a "fill in as you go" basis if 
// other clients wish to access certain strings of the MSI string table.
enum {
	msiIndexDesignTemplates = 4, // msiidsDesignTemplates
};

MSOAPI_(void) MsoGimmePublishComponentString(DWORD dwIndex, LPWSTR pwzBuffer, DWORD *pcchBuffer);

/****************************************************************************
 World-Wide Exe
****************************************************************************/

/*----------------------------------------------------------------------------
	MsoInitPluggableUI

	Init and cache the language settings used by pluggable UI
------------------------------------------------------------------- ZIYIW --*/
MSOAPI_(BOOL) MsoInitPluggableUI(void);

/*-----------------------------------------------------------------------------
	MsoSetupFontLink

	Setup the global Fontlink switch : vfDoFontLink
--------------------------------------------------------------------- ZIYIW -*/
MSOAPIX_(void) MsoSetupFontLink(LCID lcidUINew);

#ifdef FUTURE
/*-----------------------------------------------------------------------------
	MsoMarkFontForInstall

	Marks specified font for demand installation upon next boot. 
------------------------------------------------------------------- NobuyaH -*/
MSOAPIX_(VOID) MsoMarkFontForInstall(WCHAR *wzFont);

/*-----------------------------------------------------------------------------
	MsoCommitFontForInstall

	Commits marked fonts for demand installation into registry so they will be
	demand installed upon next boot.
------------------------------------------------------------------- NobuyaH -*/
MSOAPIX_(VOID) MsoCommitFontForInstall();
#endif // FUTURE

/*----------------------------------------------------------------------------
	MsoAnsiCodePageLimited

	Declare that your application is code page limited.
	Must be called before MsoInitPluggableUI.
------------------------------------------------------------------- ZIYIW --*/
MSOAPI_(VOID) MsoAnsiCodePageLimited(BOOL fLimited);

/*----------------------------------------------------------------------------
	MsoFAnsiCodePageSupportsLCID

	Test whether the code page supports the lcid for an ANSI application.
	Typically, cp = GetACP().
------------------------------------------------------------------- ZIYIW --*/
MSOAPI_(BOOL) MsoFAnsiCodePageSupportsLCID(UINT cp, LCID lcid);


/*----------------------------------------------------------------------------
	MsoFValidLocale

	Test whether this lcid is valid on this machine.
------------------------------------------------------------------- JJames --*/
MSOAPI_(BOOL) MsoFValidLocale(LCID lcid);

#define ILS_NOTCHANGED				0
#define ILS_CHANGED_NOT_PROCESSED	1
#define ILS_CHANGED_PROCESSED		2
#define ILS_CHANGED_PROCESSING		3

#define APPID_WORD					0
#define APPID_XL					1
#define APPID_PPT					2
#define APPID_ACCESS				3
#define APPID_OUTLOOK				4
#define APPID_FRONTPAGE				5
#define APPID_PUBLISHER				6
#define APPID_PROJECT				7
#define APPID_DESIGNER				8
/*----------------------------------------------------------------------------
	MsoAppSetChangeInstallLanguageState
	MsoAppGetChangeInstallLanguageState

	Get/Set Application based install language change state.
------------------------------------------------------------------- ZIYIW --*/
MSOAPI_(int) MsoAppSetChangeInstallLanguageState(int idApp, int ils);
MSOAPI_(int) MsoAppGetChangeInstallLanguageState(int idApp);

/*----------------------------------------------------------------------------
	MsoGetInstallLcid

	return the cached office install lcid
------------------------------------------------------------------- ZIYIW --*/
MSOAPI_(LCID) MsoGetInstallLcid(void);

/*----------------------------------------------------------------------------
	MsoGetInstallLcid

	return the cached office install lcid
------------------------------------------------------------------- ZIYIW --*/
MSOAPI_(LCID) MsoGetInstallLcid2000Compatible(void);

/*-----------------------------------------------------------------------------
	MsoGetInstallFlavor

	return the cached office install flavor lcid
--------------------------------------------------------------------- ZIYIW -*/
MSOAPI_(LCID) MsoGetInstallFlavor(void);

/*-----------------------------------------------------------------------------
	MsoFLangChanged

	return a flag whether or not the UI lang has been changed since last time
--------------------------------------------------------------------- ZIYIW -*/
MSOAPIX_(int) MsoFLangChanged(LCID *plcid);

/*-----------------------------------------------------------------------------
	MsoGetPreviousUILcid 
	
	return the cached previous ui lcid
--------------------------------------------------------------------- ZIYIW -*/
MSOAPIX_(LCID) MsoGetPreviousUILcid(void);

/*-----------------------------------------------------------------------------
	MsoGetPreviousInstallFlavor 
	
	return the cached previous install flavor
--------------------------------------------------------------------- ZIYIW -*/
MSOAPI_(LCID) MsoGetPreviousInstallFlavor(void);

/*----------------------------------------------------------------------------
	MsoGetUILcid

	return the cached office UI lcid
------------------------------------------------------------------- ZIYIW --*/
MSOAPI_(LCID) MsoGetUILcid(void);

/*----------------------------------------------------------------------------
	MsoGetHelpLcid

	return the cached office Help lcid
------------------------------------------------------------------- ZIYIW --*/
MSOAPI_(LCID) MsoGetHelpLcid(void);

/*-----------------------------------------------------------------------------
	MsoGetExeModeLcid
	
	return the cached ExeMode lcid
-------------------------------------------------------------------- IrfanGo -*/
MSOAPI_(LCID) MsoGetExeModeLcid(void);

/*----------------------------------------------------------------------------
	MsoGetSKULcid

	return the cached office installed SKU lcid
------------------------------------------------------------------- ZIYIW --*/
MSOAPI_(LCID) MsoGetSKULcid(void);

/*-----------------------------------------------------------------------------
	MsoGetWebLocale -  return the cached weblocale lcid
-------------------------------------------------------------------- ZIYIW -*/
MSOAPI_(LCID) MsoGetWebLocale(void);

/*----------------------------------------------------------------------------
	MsoEnumEditLcid

	enumerate throught the cached office edit lcids
------------------------------------------------------------------- ZIYIW --*/
MSOAPI_(BOOL) MsoEnumEditLcid(LCID*, int);

/*----------------------------------------------------------------------------
	MsoFAddRemoveEditLcidFromReg

	Turn On/Off a edit langauge in reg
	You generally want to avoid the Ex version
------------------------------------------------------------------- ZIYIW --*/
MSOAPI_(int) MsoFAddRemoveEditLcidFromReg(LCID lcid, int fAdd, int fExplicit);
MSOAPI_(int) MsoFAddRemoveEditLcidFromRegEx(LCID lcid, int fAdd, int fExplicit, int fOverride);

// Return a best-guess LCID for the current keyboard and the optional character
MSOAPI_(LCID) MsoLcidKeyboard(
	HMSOINST			hinst,
	LCID				lcidFEDefault,
	WCHAR               *pwch,                           // Optional, or NULL
	int                  cch
	);

/*----------------------------------------------------------------------------
	MsoLcidGetLanguages

	read various language settings from the registry throught ORAPI
----------------------------------------------------------- ZIYIW/irfango --*/
BOOL MsoLcidGetLanguages(LCID*, LCID*, LCID*, MSOELI*, int*, MSOELI*, int*, LCID*);

/*-----------------------------------------------------------------------------
	MsoPropagateInstallFlavor

	Setup InstallFlavor reg setting for ENG/FRN/GER SKU
--------------------------------------------------------------------- ZIYIW -*/
void MsoPropagateInstallFlavor(LCID lcidInstall);

/*-----------------------------------------------------------------------------
	MsoHrCreateLanguageSettingsObject

	creates a new instance of the LanguageSettings OLE object
--------------------------------------------------------------------- ZIYIW -*/
MSOAPI_(int) MsoHrCreateLanguageSettingsObject(HMSOINST hmsoinst, void **pplss);

/*-----------------------------------------------------------------------------
	MsoDialogFontNameLid

	get the Localized/EUC dialog font name based on UI lid passed in. 
--------------------------------------------------------------------- ZIYIW -*/
MSOAPI_(void) MsoDialogFontNameLid(WCHAR *wzName, LCID lid);

/*-----------------------------------------------------------------------------
	MsoDialogFontJpnAlt

	get the Localied/EUC alternate dialog font name for JPN.
------------------------------------------------------------------- NobuyaH -*/
MSOAPI_(void) MsoDialogFontJpnAlt(WCHAR *wzName);


/*-----------------------------------------------------------------------------
	MsoSetPureReg/MsoGetPureReg

	Operations on the Pure language resource registry
--------------------------------------------------------------------- ZIYIW -*/
#define REG_PURE_UNKNOWN		0
#define REG_PURE_OFF			1
#define REG_PURE_COMPLETED		2
#define REG_PURE_PROHIBITED		3
#define REG_PURE_ON             REG_PURE_PROHIBITED

MSOAPIX_(int) MsoSetPureReg(int iState);
MSOAPI_(int) MsoGetPureReg(void);

/****************************************************************************
 Migration
****************************************************************************/

enum
{
	msoFirstApp  = 0,
	msoWord      = 0,
	msoExcel     = 1,
	msoAccess    = 2,
	msoPPT       = 3,
	msoOffice    = 4,
	msoGraph     = 5,
	msoOutlook   = 6,
	msoFrontPage = 7,
	msoPublisher = 8,
	msoProject   = 9,
	msoVisio     = 10,
	msoDesigner  = 11,
	msoOSA		 = 12,
	msoLastApp   = msoOSA
};

#define msoNoCmwPopulate 0x80000000

enum
{
	msoOfficeCurrentVersion = 0,
	msoOffice10Version      = 0,
	msoOffice9Version       = 1,
	msoOffice97Version      = 2
};

enum
{
	msoNoMigration		= 0,
	msoOfficeVersion6	= 6,
	msoOfficeVersion7	= 7,
	msoOfficeVersion8	= 8,
	msoOfficeVersion9	= 9
};

MSOAPI_(BOOL) MsoMigrate(int iApp, DWORD *pdwMigrationVersion);


/* - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - -
	The New New New  O R A P I   A P I s
- - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - */

#include "msoreg.h"


/*-----------------------------------------------------------------------------
	ORAPI Cache data
--------------------------------------------------------------------dgray----*/
typedef struct KEYNODE_S {
	HKEY	hKey;                   // handle to the key
	int		keyID;                  // enum ID number for this key
	union
		{
		int		Options;            // ORAPICacheOptionFlags
		struct
			{
			BOOL fPersist  : 1;
			BOOL fRWAccess : 1;
			BOOL fIsPolicy : 1;
			BOOL fIsApp    : 1;
			BOOL fIsValid  : 1;
			// if the ref count is changed, make sure 
			// ORAPI_MAX_REF_COUNT in tcorapi.cpp matches
			int nRefCount  : 3;
			// TODO DGray : Pad this to 32 bits
			// int pad        : 24;
			};
		};
#ifdef VSMSODEBUG
	CHAR	szKeyName[MAX_PATH];  // Name of the key
	int		nTimesUsed;           // Number of times this key has been hit
#endif // VSMSODEBUG
	struct KEYNODE_S* pNext;      // The next keyID in the cache
	struct KEYNODE_S* pPrev;      // The next keyID in the cache
} KEYNODE;


/*

 PERSIST    - When set, Do not remove this key from the cache
 RW_ACCESS  - When set, key opened with READ/WRITE access, otherwise just read
 IS_POLICY  - Set, this key exists in the policy tree, otherwise user tree
 IS_APP_KEY - Set, this key is from the APP, otherwise from MSO
 KEY_VALID  - The hkey attached to this node is valid

*/
enum ORAPICacheOptionFlags
{
	PERSIST			= 0x01,
	RW_ACCESS		= 0x02,
	IS_POLICY		= 0x04,
	IS_APP_KEY		= 0x08,
	KEY_VALID		= 0x10,

	MASK_PERSIST	= 0xFFFFFFFE,
	MASK_RW			= 0xFFFFFFFD,
	MASK_POLICY		= 0xFFFFFFFB,
	MASK_APP		= 0xFFFFFFF7,
};



/*-----------------------------------------------------------------------------
	MsoFOrapiPrimeKeyCache

	Prime a root of the registry tree in the cache
	Used only in the init
--------------------------------------------------------------------dgray----*/
MSOAPI_(BOOL) MsoFOrapiPrimeKeyCache(int keyID, int Options, HKEY hKey,
                                     BOOL fHoldRef, KEYNODE** ppkn, PCSTR sz);
//#ifdef VSMSODEBUG
//MSOAPI_(BOOL) MsoFOrapiPrimeKeyCache(int keyID, int Options, HKEY hKey,
//                                     BOOL fHoldRef, KEYNODE** ppkn,
//                                     PCSTR sz);
//#else  // ! DEBUG
//#define MsoFOrapiPrimeKeyCache(keyID,Options,hKey,fHoldRef,ppkn,sz) \
//        MsoFOrapiPrimeKeyCache(keyID,Options,hKey,fHoldRef,ppkn)
//MSOAPI_(BOOL) MsoFOrapiPrimeKeyCache(int keyID, int Options, HKEY hKey,
//                                     BOOL fHoldRef, KEYNODE** ppkn);
//#endif // ! VSMSODEBUG


/*-----------------------------------------------------------------------------
	ORAPI App init routines
--------------------------------------------------------------------dgray----*/

/*-----------------------------------------------------------------------------
   MsoFRegHookAppTables

   Hooks the application data tables into ORAPI.  Needs to be called before 
   any ORAPI calls are made by an application.
   Returns TRUE if policy is in effect, FALSE if no policy.
--------------------------------------------------------------------dgray----*/
MSOAPI_(BOOL) MsoFRegHookAppTables(const void* pAppReg, const void* pAppOrkey,
                                   int cNumRegs, int cNumKeys);



/*-----------------------------------------------------------------------------
	These ORAPI functions are used to get a handle to a HKEY.  This should 
	only be used when you are using orapi to open the key, and then you are
	using the HKEY to do something ORAPI doesn't handle, such as enumeration.

	ALWAYS close the key through ORAPI with MsoRegCloseKeyHkey().

--------------------------------------------------------------------dgray----*/
MSOAPI_(LONG) MsoRegOpenKey(int msorid, PHKEY phkResult);
MSOAPI_(LONG) MsoRegOpenKeyEx (int msorid, REGSAM samDesired, PHKEY phkResult);
MSOAPI_(LONG) MsoRegCreateKeyEx(int msorid, PHKEY phkResult,
                                LPDWORD lpdwDisposition);
MSOAPI_(LONG) MsoRegCreateKey(int msorid, PHKEY phkResult);



/*-----------------------------------------------------------------------------
   MsoRegDeleteValue

   This function deletes a value from the registry.
--------------------------------------------------------------------dgray----*/
MSOAPI_(LONG) MsoRegDeleteValue(int msorid);



/*-----------------------------------------------------------------------------
   MsoRegDeleteKey

   This function deletes a registry key from the users registry tree.
   It also clears the value in the cache if there is one.

--------------------------------------------------------------------dgray----*/
MSOAPI_(LONG) MsoRegDeleteKey(int msorid);



/*-----------------------------------------------------------------------------
	MsoCbRegGetBufferSize*

	Use these functions to mimic the win32 RegQueryValueEx() call to get
	buffer size.  RegQueryValueEx(phkey, pValueName, NULL, NULL, NULL, &Size);
	Use the function suited to the type accessed.
	
  MsoCbRegGetBufferSizeCore
  MsoCbRegGetBufferSizeDefaultCore
    Don't use these.  Other funcions wrap or #define to them.

  MsoCbRegGetBufferSizeSz
    Use this function to query for the size you need to allocate in order
    to query for a REG_SZ in ansi space.  Returns the size in bytes.

  MsoCbRegGetBufferSizeDefaultSz
    Use this function to query for the size you need to allocate in order
    to query for the default value of a REG_SZ in ansi space.  
    Returns the size in bytes.

  MsoCbRegGetBufferSizeWz
    Use this function to query for the size you need to allocate in order
    to query for a REG_SZ in unicode space.  Returns the size in bytes.

  MsoCbRegGetBufferSizeDefaultWz
    Use this function to query for the size you need to allocate in order
    to query for the default value of a REG_SZ in unicode space.  
    Returns the size in bytes.

  MsoCbRegGetBufferSizeBinary
    Use this function to query for the size you need to allocate in order
    to query for a REG_BINARY value data.  Returns size in bytes.
--------------------------------------------------------------------dgray----*/
MSOAPI_(DWORD) MsoCbRegGetBufferSizeCore(int msorid);
MSOAPIX_(DWORD) MsoCbRegGetBufferSizeDefaultCore(int msorid);
MSOAPI_(DWORD) MsoCbRegGetBufferSizeSz(int msorid);
MSOAPIX_(DWORD) MsoCbRegGetBufferSizeDefaultSz(int msorid);
MSOAPI_(DWORD) MsoCbRegGetBufferSizeWz(int msorid);
MSOAPIX_(DWORD) MsoCbRegGetBufferSizeDefaultWz(int msorid);
#ifdef VSMSODEBUG
MSOAPI_(DWORD) MsoCbRegGetBufferSizeBinary(int msorid);
#else  // ! DEBUG
#define MsoCbRegGetBufferSizeBinary(msorid) MsoCbRegGetBufferSizeCore(msorid)
#endif // ! VSMSODEBUG



/*-----------------------------------------------------------------------------
   MsoRegForceWriteDefaultValue

   DO NOT USE THIS FUNCTION!  It is not safe for general consumption.  This
   should only be used by JJames.  See the comments in tcorapi.cpp
--------------------------------------------------------------------dgray----*/
MSOAPIX_(LONG) MsoRegForceWriteDefaultValue(int msorid);



/*-----------------------------------------------------------------------------
	MsoFRegGetDw

	Gets the REG_DWORD value for this msorid and puts it in *pdwData.
	MsoFRegGetDw
		Returns TRUE if succeeded, FALSE if failed.  Assert if default value
		is not NO_DEFAULT
--------------------------------------------------------------------dgray----*/
MSOAPI_(BOOL)  MsoFRegGetDwCore(int msorid, DWORD* pdwData);
#ifdef VSMSODEBUG
MSOAPI_(BOOL)  MsoFRegGetDw(int msorid, DWORD* pdwData);
#else  // ! DEBUG
#define MsoFRegGetDw(msorid, pdwData) MsoFRegGetDwCore(msorid, pdwData)
#endif // ! VSMSODEBUG



/*-----------------------------------------------------------------------------
	MsoDwRegGetDw

	Returns the REG_DWORD value for this msorid.
	Does not check for failure, which is fine if default values are defined.
--------------------------------------------------------------------dgray----*/
MSOAPI_(DWORD) MsoDwRegGetDw(int msorid);



/*-----------------------------------------------------------------------------
	These should be used in only select cases!  These functions should be
	used only in cases where we know that a DWORD may be written out as a
	REG_BINARY type.  In general, we will accept a REG_BINARY if it is the 
	correct size, but we will assert if the type does not match.  These
	functions turn that assert off and back on.  Use like so:
		MsoRegDwTypeMatchAssertOff();
		dw = MsoDwRegGetDw(msoridFoo);
		MsoRegDwTypeMatchAssertOn();

	ALWAYS CALL THIS IN PAIRS SO THE ASSERT GETS RE-ENABLED!
--------------------------------------------------------------------dgray----*/
#ifdef VSMSODEBUG
MSOAPI_(void) MsoRegDwTypeMatchAssertOff();
MSOAPI_(void) MsoRegDwTypeMatchAssertOn();
#else  // ! DEBUG
#define MsoRegDwTypeMatchAssertOff()
#define MsoRegDwTypeMatchAssertOn()
#endif // ! VSMSODEBUG



/*-----------------------------------------------------------------------------
	These should be used in only select cases!  These functions should be
	used when we're calling MsoFRegGetDw, but the rid may have Orapi
	default-value-data.  In general, MsoFRegGetDw will assert if the rid has 
	def-value-data, since the code could call MsoDwRegGetDw instead (guaranteed 
	not to fail). 
	These functions turn that assert off and back on.  Use like so:
		MsoRegDefValAssertOff();
		if (MsoFDwRegGetDw(msoridFoo)) blah;
		MsoRegDefValAssertOn();

	ALWAYS CALL THIS IN PAIRS SO THE ASSERT GETS RE-ENABLED!
--------------------------------------------------------------------dgray----*/
#ifdef VSMSODEBUG
MSOAPI_(void) MsoRegDefValAssertOff();
MSOAPI_(void) MsoRegDefValAssertOn();
#else  // ! DEBUG
#define MsoRegDefValAssertOff()
#define MsoRegDefValAssertOn()
#endif // ! VSMSODEBUG



/*-----------------------------------------------------------------------------
	MsoFRegSetDw

	Sets the REG_DWORD value for this msorid.
	Returns TRUE if succeeded, FALSE if failed.
--------------------------------------------------------------------dgray----*/
MSOAPI_(BOOL)  MsoFRegSetDw(int msorid, DWORD dwData);



/*-----------------------------------------------------------------------------
	MsoFRegGetBinary

	Gets the REG_BINARY value for this msorid. pCb should be set to the size
	of the buffer passed in.  (pCb needed can be queried with GetBufferSize
	functions)
	
	Returns:	TRUE if succeeded, FALSE if failed.
	Sides:		*pbData is filled with the binary data retrieved.
				*pCb is filled with the size of the returned binary.
--------------------------------------------------------------------dgray----*/
MSOAPI_(BOOL)  MsoFRegGetBinary(int msorid, LPBYTE pbData, DWORD* pCb);



/*-----------------------------------------------------------------------------
	MsoFRegSetBinary

	Sets the REG_BINARY value for this msorid. Cb should be set to the size
	of the buffer passed in to be written.
	
	Returns:	TRUE if succeeded, FALSE if failed.
--------------------------------------------------------------------dgray----*/
MSOAPI_(BOOL)  MsoFRegSetBinary(int msorid, const BYTE *pbData, DWORD Cb);



/*-----------------------------------------------------------------------------
	Mso*RegGetSz

	Gets the REG_SZ value for this msorid. 

	Input Parameters:
						Cb should be set to the size of the buffer passed
							in in bytes
						sz should be the character buffer to be filled.
	
	Sides:		*sz is filled with the ansi string data retrieved.

	MsoFRegGetSz
		Returns TRUE if success, FALSE if failed.  Asserts is default value
		is not NO_DEFAULT
	MsoRegGetSz
		No return value.  Used if there is a default value in the database.
--------------------------------------------------------------------dgray----*/
MSOAPI_(BOOL)  MsoFRegGetSzCore(int msorid, PSTR sz, DWORD Cb);
#ifdef VSMSODEBUG
MSOAPI_(VOID)  MsoRegGetSz (int msorid, PSTR sz, DWORD Cb);
MSOAPI_(BOOL)  MsoFRegGetSz(int msorid, PSTR sz, DWORD Cb);
#else  // ! DEBUG
#define MsoFRegGetSz(msorid, sz, Cb) MsoFRegGetSzCore(msorid, sz, Cb)
#define MsoRegGetSz(msorid, sz, Cb)  MsoFRegGetSzCore(msorid, sz, Cb)
#endif // ! VSMSODEBUG



/*-----------------------------------------------------------------------------
	Mso*RegGetWz

	Gets the REG_SZ value for this msorid.

	Input Parameters:
						Cb should be set to the size of the buffer passed
							in in bytes
						wz should be the character buffer to be filled.
	
	Sides:		*wz is filled with the wide string data retrieved.

	MsoFRegGetWz
		Returns TRUE if success, FALSE if failed.  Asserts is default value
		is not NO_DEFAULT
	MsoRegGetWz
		No return value.  Used if there is a default value in the database.
--------------------------------------------------------------------dgray----*/
MSOAPI_(BOOL)  MsoFRegGetWzCore(int msorid, PWSTR wz, DWORD Cb);
#ifdef VSMSODEBUG
MSOAPI_(VOID)  MsoRegGetWz (int msorid, PWSTR wz, DWORD Cb);
MSOAPI_(BOOL)  MsoFRegGetWz(int msorid, PWSTR wz, DWORD Cb);
#else  // ! DEBUG
#define MsoFRegGetWz(msorid, wz, Cb) MsoFRegGetWzCore(msorid, wz, Cb)
#define MsoRegGetWz(msorid, wz, Cb)  MsoFRegGetWzCore(msorid, wz, Cb)
#endif // ! VSMSODEBUG
MSOAPI_(DWORD) MsoCchRegGetWz(int msorid, PWSTR wz, DWORD Cb);



/*-----------------------------------------------------------------------------
	MsoFRegSetSz

	Sets the REG_SZ registry value for this msorid using sz as the input.

	Input Parameters:
						*sz is the buffer containing the ansi string to write.

	Returns:	TRUE if succeeded, FALSE if failed.	
--------------------------------------------------------------------dgray----*/
MSOAPI_(BOOL)  MsoFRegSetSz(int msorid, PCSTR sz);



/*-----------------------------------------------------------------------------
	MsoFRegSetWz

	Sets the REG_SZ registry value for this msorid using a wz as the input.

	Input Parameters:
						*wz is the buffer containing the wide string to write.

	Returns:	TRUE if succeeded, FALSE if failed.	
--------------------------------------------------------------------dgray----*/
MSOAPI_(BOOL)  MsoFRegSetWz(int msorid, PCWSTR wz);



/*-----------------------------------------------------------------------------
	MsoFRegKeyExists

	Returns true if the key for this msorid exists in the registry.
	This could be in either the policy tree or the user reg tree.
	Returns false if there is no value in the registry.
--------------------------------------------------------------------dgray----*/
MSOAPI_(BOOL) MsoFRegKeyExists(int msorid);



/*-----------------------------------------------------------------------------
	Mso*RegGetDefault*

	Retrieves the DEFAULT value for a particular rid, in the same method as the 
	other retrieval functions above.

	False if failed (no default value)
--------------------------------------------------------------------dgray----*/
MSOAPIX_(BOOL) MsoFRegGetDefaultSz(int msorid, PSTR sz, DWORD Cb);
MSOAPI_(BOOL) MsoFRegGetDefaultWz(int msorid, PWSTR wz, DWORD Cb);
MSOAPI_(DWORD) MsoDwRegGetDefaultDw(int msorid);



/*-----------------------------------------------------------------------------
	MsoFRegValueExists

	Returns true if value data for this msorid exists in the registry.
	This could be in either the policy tree or the user reg tree.
	Returns false if there is no value in the registry.

--------------------------------------------------------------------dgray----*/
MSOAPI_(BOOL) MsoFRegValueExists(int msorid);



/*-----------------------------------------------------------------------------
	MsoFRegDefaultValueExists

	Returns > 0 if default value exists, returns 0 if no default exists.
--------------------------------------------------------------------dgray----*/
MSOAPIDBG_(BOOL) MsoFRegValueExistsDefault(int msorid);



/*-----------------------------------------------------------------------------
	MsoFRegPolicyValueExists

	Returns TRUE if there is a value in the Policies tree to return. 
	FALSE otherwise.
--------------------------------------------------------------------dgray----*/
MSOAPI_(BOOL) MsoFRegPolicyValueExists(int msorid);



/*-----------------------------------------------------------------------------
	OrapiSetVal

	Sets a Generic ORAPI value

	Don't use this function if you just check for ERROR_SUCCESS.  It is
	wrapped for each type.

	Returns:	Win32 error code
	Sides:		None.
--------------------------------------------------------------------dgray----*/
MSOAPIX_(LONG) OrapiSetVal(int msorid, const BYTE* pbData, DWORD Cb);



/*-----------------------------------------------------------------------------
	OrapiQueryVal

	Queries for a Generic ORAPI value, not a string
	Order of how it gets the value
			1) Query Software/Policy Tree
			2) Query Software Tree
			3) Use Default Value
			4) Fill with empty value (0; 0x00, 0x00;)

	Returns:	Win32 error code (ERROR_SUCCESS except in case 4, but never
					use to check for ERROR_MORE_DATA, or 
					ERROR_INSUFFICIENT_BUFFER.
					(Use the GetBufferSize functions)
	Sides:		Fills wzData with a valid wz string
				Fills *pCb with the size in bytes returned.

REVIEW DGray: 
		Is there a way I can make this function even more compact by
		using a pointer to a function to make the query calls, 
		and dynamically setting it to either the W or A version, based 
		on vfUnicodeAPI?  The only thing holding my up is the type checking for 
		the wzValueName or rgMsoReg[msorid].szValue
--------------------------------------------------------------------dgray----*/
MSOAPIX_(LONG) OrapiQueryVal(int msorid, LPBYTE pbData, LPDWORD pCb);



/*-----------------------------------------------------------------------------
	OrapiQuerySzVal

	Queries for an ORAPI string value, to be returned as an sz
	Order of how it gets the string:
			1) Query Software/Policy Tree
			2) Query Software Tree
			3) Use Default Value
			4) Fill with empty string ("\0")


	Returns:	Win32 error code (ERROR_SUCCESS except in case 4, but never
					use to check for ERROR_MORE_DATA, or 
					ERROR_INSUFFICIENT_BUFFER.
					(Use the GetBufferSize functions)
	Sides:		Fills szData with a valid sz string
--------------------------------------------------------------------dgray----*/
MSOAPI_(LONG) OrapiQuerySzVal(int msorid, PSTR szData, DWORD Cb);



/*-----------------------------------------------------------------------------
	OrapiQueryWzVal

	Queries for an ORAPI string value, to be returned as a wz
	Order of how it gets the string:
			1) Query Software/Policy Tree
			2) Query Software Tree
			3) Use Default Value
			4) Fill with empty string (L"\0")


	Returns:	Win32 error code (ERROR_SUCCESS except in case 4, but never
					use to check for ERROR_MORE_DATA, or 
					ERROR_INSUFFICIENT_BUFFER.  
					(Use the GetBufferSize functions)

	Sides:		Fills wzData with a valid wz string
--------------------------------------------------------------------dgray----*/
MSOAPI_(LONG) OrapiQueryWzVal(int msorid, PWSTR wzData, DWORD Cb);



/*-----------------------------------------------------------------------------
	OrapiGetRid

	Given a value name, what is the rid associated with it.
--------------------------------------------------------------------dgray----*/
MSOAPIX_(BOOL) OrapiGetRidForValueEx(PSTR pszValueName, PSTR pszKeyName,
                                    BOOL fUseApp, DWORD *pdwMsoRid,
                                    DWORD *pdwRegType);
MSOAPI_(BOOL) OrapiGetRidForValueExW(PWSTR pwzValueName, PWSTR pwzKeyName, BOOL fUseApp, DWORD *pdwMsoRid, DWORD *pdwRegType);
#define OrapiGetRidForValue(pszValueName, pszKeyName, fUseApp, pdwMsoRid) \
	OrapiGetRidForValueEx(pszValueName, pszKeyName, fUseApp, pdwMsoRid, NULL)


/*-----------------------------------------------------------------------------
	FOfficePolicyKeyExists

	Determines if registry policy is being applied by reading the hkey.  Also
	inserts the key into the cache.
--------------------------------------------------------------------dgray----*/
BOOL FOfficePolicyKeyExists(HKEY hHive, int keyID);


/*-----------------------------------------------------------------------------
	When dealing with shared Excel and Graph code, use this to distinguish 
	between places where a single reg call is used to access graph in some
	cases and excel in others.  Tag on a rid that is msorid*XL to see an
	example.
--------------------------------------------------------------------dgray----*/
#ifdef EXCEL_BUILD
#ifdef GRAF
#define GetRid(x) x##GR
#else
#define GetRid(x) x##XL
#endif
#endif



/*-----------------------------------------------------------------------------
	MsoFRegCheckKeyPath

	Fills a buffer with the Ansi key path to a rid.  Used to make sure
	certain paths don't get accidentally changed.  Note: This path does NOT
	include the root, that is, HKEY_CURRENT_USER, HKEY_LOCAL_MACHINE, etc.
	So "Software\\Policies" would match "HKEY_CURRENT_USER\\Software\\Policies"
	and also "HKEY_LOCAL_MACHINE\\Software\\Policies", but that is not where 
	these key mishaps that we are trying to catch actually happen, so this is
	fine.  If msoridRoot is specified, we build the string up only until that
	key.
	
	Returns:
		TRUE  if the rid's key matches the expected string
		FALSE if the rid's key does not match the expected string.
--------------------------------------------------------------------dgray----*/
#ifdef VSMSODEBUG
MSOAPI_(LONG) MsoFRegCheckKeyPath(int msorid, int msoridRoot, const CHAR* sz);
#endif // VSMSODEBUG



/*----------------------------------------------------------------------------
	Experimental code to detect unused rids.
-------------------------------------------------------------------- KirKG--*/
// REVIEW: KirkG (DGray)  This is gone now, so we should rip it out, yes?
#ifdef ORAPI_RIDCHECK
#define MsoRegFGetSz(rid,b,c,d)	(ORAPI_##rid(), MsoFRegGetSzCore(rid,b,c,d)) 
MSOAPI_(LONG) MsoFRegGetSzCore(int msorid, LPDWORD lpType, LPBYTE lpData, LPDWORD lpcbData);
#endif


/****************************************************************************
	Binary Policy Routines (for Word and Excel)
****************************************************************************/

/*---------------------------------------------------------------------------
	MsoPolicyApplyBinary

	Applies policy settings to bitmapped members of a structure.  Stores
	a history list of changes so that policy changes are rolled back and
	don't infect the app's user preference settings in the registry.
-------------------------------------------------------------------JoelDow-*/	
MSOAPI_(void) MsoPolicyApplyBinary(void* pvStruct, int msoridKey, 
	WORD wcbStruct, HANDLE* phRestore);


/*---------------------------------------------------------------------------
	MsoApplyAppBinarySettings

	Similar in concept to MsoPolicyApplyBinary, this API will apply, on a ONE
	time basis, bits to certain app structures. After it finishes, it deletes
	the key containing the bits. ( msoridKey ) Used for CMW settings.
-------------------------------------------------------------------MattP-*/	
MSOAPI_(void) MsoApplyAppBinarySettings(void* pvStruct, int msoridKey, 
	WORD wcbStruct);

/*---------------------------------------------------------------------------
	MsoPolicyRestoreBinary
	
	"Undoes" previous changes to bitmapped members of a structure to 
	prevent policy settings from infecting a user's configuration
	options in the registry.
-------------------------------------------------------------------JoelDow-*/	
MSOAPI_(void) MsoPolicyRestoreBinary(void* pvStruct, WORD wcbStruct, HANDLE hRestore);

#ifdef VSMSODEBUG
/*---------------------------------------------------------------------------
	MsoPolicyDumpBinary
	
	Outputs a list of active policy overrides for the context provided
	in hRestore.  Intended for use in a debug-only status dump.
-------------------------------------------------------------------JoelDow-*/	

MSOAPI_(void) MsoPolicyDumpBinary(HANDLE fhOut, HANDLE hRestore);
#endif


/****************************************************************************
	Terminal Server (Hydra) Support/Detection
****************************************************************************/

/*------------------------------------------------------------

	MsoFIsTerminalServer

	Old API to detect Hydra.  I'm keeping this function here
	to maintain binary compatibility with all MSO clients.

	IT IS OBSOLETE.  PLEASE DO NOT USE THIS FUNCTION.

	This function will assert and ask everyone to use the 
	new behavior modification API's. However, to maintain 
	some semblance of backward compatibility, we will return 
	assume that what the caller wants to know is whether or
	not this code is running on an AppServer, since that's
	really the only interesting way WTS 4 was used and WTS 5
	hadn't shipped when Office 9 did.

	It's just an incomplete answer, since now there are more
	"flavors" of Terminal Server, and we may want to modify
	our behavior on the Console vs. when we're running with 
	graphics over the wire, etc.

----------------------------------------------- (FrankRon) -*/
MSOAPI_(BOOL) MsoFIsTerminalServer(void);



/*------------------------------------------------------------

	MsoFTSAppServer

	Use this routine to fork behavior based on whether or not
	we're running on a regular TS App Server machine (console
	or not).  TS-Lite/Remote-Admin and regular non-TS work-
	stations return FALSE here.
----------------------------------------------- (FrankRon) -*/
MSOAPI_(BOOL) MsoFTSAppServer(void);

/*------------------------------------------------------------

	MsoFEnableComplexGraphics

	Use this routine to fork behavior on animation/sound-
	intensive features (e.g., splashes) to minimize unnecessary 
	graphics "candy."  Basically this is important for Hydra
	systems were we're transmitting lots of graphics bits
	over the wire, but it's also useful if the Shell ever
	implements a "Simplify Graphics" key in their Display
	Properties that may actually be exposed on all systems
----------------------------------------------- (FrankRon) -*/
MSOAPI_(BOOL) MsoFEnableComplexGraphics(void);

/*------------------------------------------------------------

	MsoFAllowIOD

	Use this routine to determine if Darwin is going to let
	us do an Install-On-Demand. Especially useful for TS.

	Basically, we allow IOD for non-TS systems, TS-Lite/RA,
	or when the user is an admin and the Policy to allow
	remote installs on TS is set.

	Note that usually Darwin will allow an admin to do an 
	install at the console even if the policy is not set,
	but we don't allow this.  Dynamic installation of Office
	is generally not a good idea, and we will respect admin
	policy, but we aren't going to allow admin console install
	without the policy setting.
----------------------------------------------- (FrankRon) -*/
MSOAPI_(BOOL) MsoFAllowIOD(void);

/*------------------------------------------------------------

	MsoFRemoteUI

	Use this routine if a feature needs to know if it's UI
	is running remotely (as is the case on TS non-Console
	sessions).
----------------------------------------------- (FrankRon) -*/
MSOAPI_(BOOL) MsoFRemoteUI(void);

/*---------------------------------------------------------------------------
	MsoFIEPolicyAndVersion

	Put the current version of IE in 'plMajor' and 'plMinor, with a zero
	meaning that IE is not currently installed.  Return TRUE is policy is set
	to disable just-in-time installation of IE, FALSE otherwise.
---------------------------------------------------------------- EricSchr -*/
MSOAPI_(BOOL) MsoFIEPolicyAndVersion(long *plMajor, long *plMinor);


/*-----------------------------------------------------------------------------
	MsoFCheckIEVersion

	Return TRUE if we have an IE of at least the version passed in.
	Don't worry about policy or prompting in case it isn't.  MsoFIEPolicyAndVersion()
	and MsoFUseIEFeature() are for that.

-------------------------------------------------------------------- KBrown -*/
MSOAPI_(BOOL) MsoFCheckIEVersion(long lMajor, long lMinor);

/*---------------------------------------------------------------------------
	MsoFUseIEFeature

	Return TRUE if the version of IE installed on the machine is high enough
	to use a feature requiring version 'lMajor' and 'lMinor'.  Return
	FALSE otherwise.  Note that 'lMinor' could be a two digit number, so
	that version 4.1 is actually lMajor == 4 and lMinor == 10, since
	lMinor == 1 implies 4.01.  The function will not do a demand
	installation, but it does warn about not having a new enough version
	and about admin disabling.
---------------------------------------------------------------- EricSchr -*/
MSOAPI_(BOOL) MsoFUseIEFeature(long lMajor, long lMinor);


/*---------------------------------------------------------------------------
	MsoCheckHEVReg

	Calls the Self-registration routine for the HEV project.
---------------------------------------------------------------- A-GordRo -*/
MSOAPI_(void) MsoCheckHEVReg(void);

/*-----------------------------------------------------------------------------
	MsoFSupportThisEditLID
	
	return whether the editing of specified lang is supported
-------------------------------------------------------------------- IrfanGo -*/
MSOAPI_(BOOL) MsoFSupportThisEditLID(UINT lid);

/*-----------------------------------------------------------------------------
	MsoFSupportThisEditBaseLID
	
	return whether the editing of specified lang is supported
-------------------------------------------------------------------- katsun -*/
MSOAPIX_(BOOL) MsoFSupportThisEditBaseLID(UINT lid);

/*----------------------------------------------------------------------------
	MsoSetInstallingState

	Tell Office that app is starting/finsihing installing components.
-------------------------------------------------------------------- MattP --*/
MSOAPIX_(BOOL) MsoSetInstallingState(BOOL fStartInstall);

/*----------------------------------------------------------------------------
	MsoFGetInstallingState

	Returns what the current global install state is.
-------------------------------------------------------------------- MattP --*/
MSOAPIX_(BOOL) MsoFGetInstallingState(void);

/*-----------------------------------------------------------------------------
	MsoFSupportFEEditLID - 

	return fTrue if one of the FE lids are among the editing languages 
	specified by the langtool
------------------------------------------------------------------ JeffreyK -*/
MSOAPI_(BOOL) MsoFSupportFEEditLID(void);

/*-----------------------------------------------------------------------------
	MsoFSupportFEEditBaseLID - 

	return fTrue if one of the FE lids are among the editing languages 
	specified by the langtool
------------------------------------------------------------------ katsun -*/
MSOAPIX_(BOOL) MsoFSupportFEEditBaseLID(void);

/*-----------------------------------------------------------------------------
	MsoGimmeLocalizedLibrary
	
	Extended entry-point for MsoLoadLocalizedLibraryFull -- allows user to
	pass flags requesting check of file system before calling Darwin.  Intended
	for use in boot optimization.

	Supports msotcgfProvide (same behavior as MsoLoadLocalizedLibraryFull,
	and msotcogfProvide | msotcogfSearchFirst (check file system).
----------------------------------------------------------------- JoelDow ---*/
MSOAPI_(HMODULE) MsoGimmeLocalizedLibrary(msofidT fid, LCID *plcid,
	const DWORD dwFlags, WCHAR *wzFullPath, DWORD dwGimmeFlags);

/*----------------------------------------------------------------------------
	Find out version of the CLSID's OLE server and append it to the given string
	in the form of "#version=X.X.X.X"
---------------------------------------------------------------- vadimc ----*/
MSOAPI_(BOOL) MsoFAppendCodebaseVer(const CLSID* pclsid, WCHAR* wzBuff, int cchMax);

/*---------------------------------------------------------------------------------------------------
	MsoFOsChange() is designed to let the apps tell minor differences in the OS since they last booted, i.e. 
	a new service pack or minor version.  Note that these structures will be identical the very first time 
	this API is called by anyapp after installation.
	   hmsoinst may be NULL. If so, wzHostName must be non-NULL and must give the short name
	of the application, i.e. the name passed to MsoFInitOffice.  If hmsoinst is non-NULL,
	wzHostName may be NULL.  This scheme is designed to allow apps to call this function
	before calling MsoFInitOffice if necessary and in that case an HMSOINST will not be
	available.
	   If poviOld is non-NULL, it will be filled with the OSVERSIONINFOA structure cached for this application
	the last time this API was called.  If this is the first time the API was called, it will be filled with
	current OSVERSIONINFOA information.
	   If poviNew is non-NULL, it will be filled with the current return from GetVersionExA, i.e. the current 
	OSVERSIONINFOA information.  
	Either or both of poviOld and poviNew may be NULL if that information is not needed.

	Returns TRUE if anything has changed in the OSVERSIONINFOA structure between the 
	cached copy	from the last time this API was called by this appplication and this call.
-------------------------------------------------------------------------- VadimC ------------------*/
MSOAPIX_(BOOL) MsoFOsChange(const HMSOINST hmsoinst, const WCHAR* wzHostName, 
							OSVERSIONINFOA* poviOld, OSVERSIONINFOA* poviNew);

/*---------------------------------------------------------------------------------------------------
	Returns	TRUE if the platform changed from VER_PLATFORM_WIN32_WINDOWS to VER_PLATFORM_WIN32_NT 
	and the major version of NT is greater than or equal to 5, i.e. we upgraded from Win9X to NT 5.0.
-------------------------------------------------------------------------- VadimC ------------------*/
MSOAPIX_(BOOL) MsoFOsPlatformChanged(const HMSOINST hmsoinst, const WCHAR* wzHostName);


/*----------------------------------------------------------------------------
 MSOAPI_(BOOL) MsoFSystemPolicyEnabled(int msorid) 
 Returns TRUE if Windows2000 policy corresponding to the msorid is enabled, 
 FALSE if disabled or not configured. 
 
 The following msorids are currently passed to this macro:
  	msoridNoDrives
  	msoridRestrictRun
  	msoridNoRecentDocsHistory
  	msoridNoPlacesBar 
  	msoridNoBackButton
  	msoridNoFileMru
  	msoridNoNetConnectDisconnect  

 All these msorids have an Orapi DEFAULT-VALUE-DATA of 0 (false).

------------------------------------------------------------------ AnzhelN  */
#define MsoFSystemPolicyEnabled(msorid)	MsoDwRegGetDw(msorid)


/*----------------------------------------------------------------------------
 MSOAPI_(BOOL) MsoFCanBrowse(void)

 Returns TRUE if  default browser is allowed to run by 
 "Run only allowed applications" Windows2000 policy.

 ----------------------------------------------------------------- AnzhelN  */
 MSOAPI_(BOOL) MsoFCanBrowse(void);  

/*----------------------------------------------------------------------------
MSOAPI_(BOOL) MsoFCanLaunch(WCHAR * wzAppName)

This API checks the list of allowed apps for wzAppName. Returns TRUE if it's on 
the list, FALSE otherwize
------------------------------------------------------------------ AnzhelN -*/
MSOAPI_(BOOL) MsoFCanLaunch(WCHAR * wzAppName);

//AnzhelN: ERROR_RESTRICTED_APP is an internally defined system error which is 
//returned by GetLastError() in case if ShellExecute or ShellExecuteEx failed 
//for the reason of RestrictRun system policy
#define ERROR_RESTRICTED_APP ((UINT)-1)

/*----------------------------------------------------------------------------
MSOAPI_(void) MsoSystemPolicyAlert(void);

Displays an Alert message analogious to the system Restrictions alert for
System Policy restricted applications
------------------------------------------------------------------ AnzhelN -*/
MSOAPI_(void) MsoSystemPolicyAlert(void);

/*----------------------------------------------------------------------------
MSOAPI_(WCHAR*) MsoWzGetAppNameFromPath(WCHAR * wzAppPath)

Given application path wzAppPath as stored in the registry 
returns its name without a path
------------------------------------------------------------------ AnzhelN -*/
MSOAPI_(WCHAR*) MsoWzGetAppNameFromPath(WCHAR * wzAppPath);

/*----------------------------------------------------------------------------
MSOAPI_(BOOL) MsoFHistoryPolicyEnabled(void);

Checks if NoRecentDocsHistory is enabled. Called in FrontPage.
------------------------------------------------------------------ AnzhelN -*/
MSOAPI_(BOOL) MsoFHistoryPolicyEnabled(void);

/*---------------------------------------------------------------------------
MSOAPI_(VOID) MsoSetPolicyTooltip(unsigned int tmc)

The tooltip text is from 
HKEY_CURRENT_USER\Software\Policies\Microsoft\Office\10.0\Common\
Toolbars\AttemptDisabledActionMessage
which is set by Admin through the policy editor
The tooltip is set only if EnableWPFeatures regkey is 1. 
For SDM dialogs only.
------------------------------------------------------------------ AnzhelN -*/
MSOAPI_(VOID) MsoSetPolicyTooltip(unsigned int tmc);

/*-----------------------------------------------------------------------------
	MsoEnsureValidDocObj

	make sure docobj.dll (for ie4) or actxprxy.dll (for ie5) are correctly
	registered
-------------------------------------------------------------------- HAILIU -*/
MSOAPI_(VOID) MsoEnsureValidDocObj(void);

/*-----------------------------------------------------------------------------
MSOAPI_(BOOL) MsoFQueryPhotoDraw(UINT32 grfqphd)

Asks various interesting things about the installed PhotoDraw and image
editing.
* msofqphdMayEditPictures asks whether or not PhotoDraw should edit raster
  images from Office Drawing.  It's just a thin wrapper on the Gimme API
  to verify PhotoDraw is available (at least advertised.)
------------------------------------------------------------------- JustinV -*/
#define msofqphdMayEditPictures (1<<0)
MSOAPI_(BOOL) MsoFQueryPhotoDraw(UINT32 grfqphd);

#endif // MSOTC_H
