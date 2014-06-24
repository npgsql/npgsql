#pragma once

/****************************************************************************
	MsoUser.h

	Owner: DavePa
 	Copyright (c) 1994 Microsoft Corporation

	Declarations for common functions and interfaces required for apps
	to use the Office DLL.
****************************************************************************/

#ifndef MSOUSER_H
#define MSOUSER_H

#include "msodebug.h"

#include "msoiv.h" // Instrumented Version for Office9 #defines and typedef

#ifndef MSO_NO_INTERFACES
interface IMsoControlContainer;
#endif // MSO_NO_INTERFACES

/****************************************************************************
	The ISimpleUnknown interface is a variant on IUnknown which supports
	QueryInterface but not reference counts.  All objects of this type
	are owned by their primary user and freed in an object-specific way.
	Objects are allowed to extend themselves by supporting other interfaces
	(or other versions of the primary interface), but these interfaces
	cannot be freed without the knowledge and cooperation of the object's
	owner.  Hey, it's just like a good old fashioned data structure except
	now you can extend the interfaces.
****************************************************************** DAVEPA **/

#undef  INTERFACE
#define INTERFACE  ISimpleUnknown

DECLARE_INTERFACE(ISimpleUnknown)
{
	/* ISimpleUnknown's QueryInterface has the same semantics as the one in
		IUnknown, except that QI(IUnknown) succeeds if and only if the object
		also supports any real IUnknown interfaces, QI(ISimpleUnknown) always
		succeeds, and there is no implicit AddRef when an non-IUnknown-derived
		interface is requested.  If an object supports both IUnknown-derived
		and ISimpleUnknown-derived interfaces, then it must implement a
		reference count, but all active ISimpleUnknown-derived interfaces count
		as a single reference count. */
	MSOMETHOD(QueryInterface) (THIS_ REFIID riid, void **ppvObj) PURE;
};


/****************************************************************************
	HMSOINST is an opaque reference to an Office instance record.  Each
	thread of each EXE or DLL that uses Office must call MsoFInitOffice
	to init Office and get an HMSOINST.
****************************************************************** DAVEPA **/
#ifndef HMSOINST
typedef struct MSOINST *HMSOINST;  // MSOINST is defined only within Office
#endif

#include "msotc.h"

/****************************************************************************
	The IMsoUser interface has methods for Office to call back to the
	app for general information that is common across Office features.
****************************************************************** DAVEPA **/

#undef  INTERFACE
#define INTERFACE  IMsoUser

enum {
	msofmGrowZone = 1,
};

enum {
	msocchMaxShortAppId = 15
};


/*	dlgType sent to IMsoUser::FPrepareForDialog. Modal dialogs have LSB 0.*/
#define msodlgWindowsModal			0x00000000
#define msodlgWindowsModeless		0x00000001
#define msodlgSdmModal				0x00000010
#define msodlgSdmModeless			0x00000011
#define msodlgUIModalWinModeless	0x00000101
#define msodlgUIModalSdmModeless	0x00000111
#define msodlgSdmModalNWaitAct	0x00001000


// Notification codes for FNotifyAction methods
// Names containing 'Query' indicate app's return value is sought.  Other
// values are strictly to notify app.
enum
	{
	msonaStartHelpMode = 0,			// User entered Quick tip mode (Shift-F1).  App should update any internal state
	msonaEndHelpMode,					// Quick tip was displayed.  App should restore cursor.
	msonaBeforePaletteRealize,		// Office is going to realize one or more palettes, see comment below
	msonaQueryDisablePip,			// Gives the app the chance to refuse a .PIP file, should it not want one.
	msonaQueryInsertPicture,		// Asks the app if we can insert a picture
	msonaQueryAcbAware,				// Asks the app if it's fully aware of the Active ClipBoard
	msonaBeforeInitTFCBalloons,	// tells the app that TFC balloons are about to be initialized
	msonaAfterInitTFCBalloons,		// tells the app that TFC balloon initialization is over
	msonaAddinBeforeConnect,
	msonaAddinAfterConnect,
	msonaAddinBeforeDisconnect,
	msonaAddinAfterDisconnect
	};

// Subsystem classifications for FEmNotifyAction methods
enum
	{
	msoemssToolbar = 0,			// Command bars
	msoemssAppEm,					// App Event Monitor
	msoemssTip,						// Possible future feature: tip interface
	msoemssTimer,					// Possible future feature: timer notify
	};

/* About msonaBeforePaletteRealize:

	Office will call FNotifyAction(msonaBeforePaletteRealize) to let the app
	it's going to realize a palette. The app should start palette management
	if it has delayed doing so until it absolutely needs to.
	
	The app should select and realize a palette, and from now on, should
	respond to palette messages WM_QUERYNEWPALETTE and WM_PALETTECHANGED.
*/


DECLARE_INTERFACE(IMsoUser)
{
   /* Debuging interfacing for this interface */
   MSODEBUGMETHOD

	/* Return an IDispatch object for the Application object in 'ppidisp'
		Return fSuccess. */
	MSOMETHOD_(BOOL, FGetIDispatchApp) (THIS_ IDispatch **ppidisp) PURE;

	/* Return the long representing the application, as required by the
		"Creator" method of VBA objects. */
	MSOMETHOD_(LONG, LAppCreatorCode) (THIS) PURE;		//  REVIEW:  PETERO:  Is this MAC only?

	/* If the host does not support running macros then return FALSE,
		else check the macro reference in wtzMacro, which is in a 257 char buffer,
		for validity, modify it in-place if desired, and return TRUE if valid. 
		The object trying to attach the macro, if any, is given by 'pisu'.
		The format of macro references is defined by the host, but the typical
		simple case would be the name of a VBA Sub.  The host may delay
		expensive validation checks until FRunMacro as desired. */
	MSOMETHOD_(BOOL, FCheckMacro) (THIS_ WCHAR *wtzMacro, ISimpleUnknown *pisu) PURE;

	/* Run the macro given by the reference wtz (which has been checked for
		validity by FCheckMacro).  The object to which the macro is attached, 
		if any, is given by 'pisu'.  Return TRUE if successful (FALSE if the
		host does not support running macros). */
	MSOMETHOD_(BOOL, FRunMacro) (THIS_ WCHAR *wtzMacro, ISimpleUnknown *pisu,
										 VARIANT *pvarResult, VARIANT *rgvar,
										 int cvar) PURE;

	/* When a low memory condition occurs this callback method will be invoked.  The
		Application should free up cbBytesNeeded or more if it can.  Return back the
		actual number of bytes that were freed. */
	MSOMETHOD_(int, CbFreeMem) (THIS_ int cbBytesNeeded, int msofm) PURE;

	/* Office will call this in deciding whether or not to do certain actions
		that require OLE. */
	MSOMETHOD_(BOOL, FIsOleStarted) (THIS) PURE;

	/* Office will call this in deciding whether or not to do certain actions
		that require OLE. If the Application supports delayed OLE initialization
		and OLE has not been started, try to start OLE now.  Office makes no
		guarantee that it will cache the value returned here, so this may be
		called even after OLE has been started. */
	MSOMETHOD_(BOOL, FStartOle) (THIS) PURE;
	/* If a Picture Container is being created Office will call back to the IMsoUser
		to fill the Picture Container with control(s). */
	// TODO: TCoon unsigned int should be UCBK_SDM
	MSOMETHOD_(BOOL, FFillPictureContainer) (THIS_ interface IMsoControlContainer *picc,
															unsigned int tmc, unsigned int wBtn,
															BOOL *pfStop, int *pdx, int *pdy) PURE;
	/* The app should pass thru the parameters to WinHelp or the equivalent
		on the Mac */
	MSOMETHOD_(void, CallHelp)(THIS_ HWND hwnd, WCHAR *wzHelpFile, 
			UINT uCommand, DWORD dwData) PURE;
	// WHAT IS THIS? 
	/* The init call to initialize sdm. Get called when first sdm
	   dialog needs to come up. */
	MSOMETHOD_(BOOL, FInitDialog)(THIS) PURE;

	/* AutoCorrect functions. Used to inegrate this feature with the apps
		undo functionality and extended AC functionality in Word. */
	MSOMETHOD_(void, ACRecordVars)(THIS_ DWORD dwVars) PURE;
	MSOMETHOD_(BOOL, ACFFullService)(THIS) PURE;
	MSOMETHOD_(void, ACRecordRepl)(THIS_ int, WCHAR *wzFrom, WCHAR *wzTo) PURE;
	MSOMETHOD_(void, ACAdjustAC)(THIS_ int iwz, int idiwz) PURE;

	/* Return the CLSID of the application */
	MSOMETHOD_(void, GetAppClsid) (THIS_ LPCLSID *) PURE;

	/* Before and After doing a sdm dialog, call back to the application for
		them to do their own init and cleanup.
		The dlg parameter is a bitmap flags defined here as msodlgXXXX
		*/
 	MSOMETHOD_(BOOL, FPrepareForDialog) (THIS_ void **ppvDlg, int dlgType) PURE;
 	MSOMETHOD_(void, CleanupFromDialog) (THIS_ void *pvDlg) PURE;

	// Applications must provide a short (max 15 char + '\0') string which
	// identifies the application.  This string is used as the application ID
	// with ODMA.  This string may be displayed to the user, so it should be
	// localized.  But strings should be chosen so that localized versions
	// can often use the same string.  (For example, "MS Excel" would be a
	// good string for Excel to use with most Western-language versions.)  If
	// the file format changes for a localized version (eg. for Far East or
	// bi-di versions), a different string should be used for the localized
	// versions whose file format is different.  (It is assumed that all
	// versions with the same localized string can read each other's files.)
	// The application should copy the string into the buffer provided.
	// This string cannot begin with a digit.  The application can assume
	// that wzShortAppId points to a buffer which can hold msocchMaxShortAppId
	// Unicode characters plus a terminating '\0' character.
	// If you have questions, contact erikhan.
	MSOMETHOD_(void, GetWzShortAppId) (THIS_ WCHAR *wzShortAppId) PURE;

	MSOMETHOD_(void, GetStickyDialogInfo) (THIS_ int hidDlg, POINT *ppt) PURE;
	MSOMETHOD_(void, SetPointStickyDialog) (THIS_ int hidDlg, POINT *ppt) PURE;

	/* Called before command bars start tracking, and after they stop. Note
		that this will be called even in the HMenu cases, and on the Mac.
		Also, when real command bars start tracking, you are called on
		OnComponentActivate by the Component Manager. Make sure you know which
		callback you want to use.
		This callback is used by Excel to remove/put back a keyboard change they
		have on the Mac. */
	MSOMETHOD_(void, OnToolbarTrack) (THIS_ BOOL fStart) PURE;
	
	/* Notification that the action given by 'na' occurred.
		Return TRUE if the
		notification was processed.
	*/
	MSOMETHOD_(BOOL, FNotifyAction) (THIS_ int na) PURE;

	// TODO(JBelt): this callback is obsolete
	/* Called back by the Office Darwin layer to let the app hook up its additional
		Darwin tables. The MSOTCFCF structure is explained in detail in msotc.h.
		Fill pfcf with the structure.
		You will be called on this API the very first time Office encounters a
		file id which is outside its scope. This can't happen unless you called
		MsoFGimmeFile or one of its friends with such an id in the first place.
		Non-Darwinized apps can just do nothing here. */
	MSOMETHOD_(void, HookDarwinTables) (THIS_ MSOTCFCF *pfcf) PURE;

	/*  Handle all event monitor notifications.
		There was an action of interest to the event monitor in Office, such as
		toolbar	activity.  The 'subsystem' in which the action occurs is
		indicated by emss.  na is a subsystem-specific notify action
		identifier.  A negative valued na indicates a pre-action notification
		to the event monitor.  Not all events generate a pre-action, but all do
		generate a post-action.  Arguments are packed into the structure at
		pvArgs, in a subsystem and na-specific fashion.  pvArgs is maintained
		(i.e. allocated and freed, if necessary) entirely on the Office side.
		ppvEmNotify is provided for app-side communication between pre- and
		post-action notifications.  ppvEmNotify is maintained entirely by the
		application.
		Return TRUE from FEmNotifyAction if the notification was processed.
		Currently, the return value is only relevant in the case where TRUE
		is returned from a pre-action notification, in which case no
		post-action notification is sent for that event.
	*/
	MSOMETHOD_(BOOL, FEmNotifyAction) (THIS_ int emss, int na,
										void **ppvEmNotify, void *pvArgs) PURE;

	/* Called by an office for a button customized to be hyperlink passing the
	   mode in which the App should open the hyperlink provided in pwzSource
	   Return TRUE if app opened Hyperlink for the given mode
	          FALSE if hyperlink could not be opened/app doesn't care about it */
	MSOMETHOD_(BOOL, FOpenHyperlink) (THIS_ LPCWSTR pwzSource,LPCWSTR pwzLocation,
									  DWORD grfwtbnt,int mode) PURE;
};


/****************************************************************************
	IMsoUser10 is an Office10 extension of IMsoUser interface

	Office code cannot assume that clients implement IMsoUser10 interface.

****************************************************************** IgorZ **/

#undef  INTERFACE
#define INTERFACE  IMsoUser10

DECLARE_INTERFACE(IMsoUser10)
{
	MSOMETHOD_(HRESULT, HrOnMsoFInitOffice)(
		HWND hwndMain, 
		HINSTANCE hinstClient, 
		IMsoUser *piuser, 
		const WCHAR *wzHostName,
		HMSOINST *phinst,
		BOOL *pfHandled) PURE;

	MSOMETHOD_(void, OnMsoUninitOffice)(HMSOINST hinst, BOOL *pfHandled) PURE;
};


MSOAPI_(BOOL) MsoFSetInstIMsoUser10(HMSOINST hinst, IMsoUser10 *pUser10);


// NOTE: Another copy of this definition is in msosdm.h
#ifndef PFNFFillPictureContainer
typedef BOOL (MSOSTDAPICALLTYPE *PFNFFillPictureContainer) (interface IMsoControlContainer *picc,
														unsigned int tmc, unsigned int wBtn,
														BOOL *pfStop, int *pdx, int *pdy);
#endif
#ifndef PFNFFillPictureContainerEx
typedef BOOL (MSOSTDAPICALLTYPE *PFNFFillPictureContainerEx) (interface IMsoControlContainer *picc,
														unsigned int tmc, unsigned int wBtn,
														BOOL *pfStop, int *pdx, int *pdy, UINT *pufFlags);
#endif



// What does an application do when it needs mso to call it back sometime?
// It registers a callback, of course.  We have callbacks all over the place
// and it's about time they started coming together.  Here's a mechanism
// for registering a callback in a common way.
//
// First, the callback of interest is identified by an msocb constant.  The app
// determines which callback(s) it wants to register, and calls MsoFSetCallback.
// This will return the previously registered callback for that msocb.
//
// The callback signature should be defined for each msocb.
//
// --brianhi

typedef void (MSOSTDAPICALLTYPE * PFNGENERICCALLBACK)(void);

enum
{
	// msocbAddinGetIDispatch:  The addins object uses this callback to obtain
	// an IDispatch object from a host application.

	msocbAddinGetIDispatch = 0,		// use PFNADDINGETIDISPATCH

	// Add new callback types here

	msocbCallbackCount
};

typedef IDispatch * (MSOSTDAPICALLTYPE * PFNADDINGETIDISPATCH)(const WCHAR * pwzAddinPath);


MSOAPI_(PFNGENERICCALLBACK) MsoPfnSetCallback(UINT msocb, PFNGENERICCALLBACK pfn);	// returns previous callback
MSOAPI_(PFNGENERICCALLBACK) MsoPfnGetCallback(UINT msocb);



/*****************************************************************************
	Registry structure for initing MSO for MsoFLangChanged
*****************************************************************************/
typedef struct _MSOREGLANG
{
	int msoridAppRegistryLang;	// For ORAPI apps
	WCHAR* pwzAppRegistryLang;	// If above 0, used to get registry entry (FP)
}MSOREGLANG; 

#if VSMSODEBUG

/*****************************************************************************
	Block Entry structure for Memory Checking
*****************************************************************************/
typedef struct _MSOBE
{
	void* hp;
	int bt;
	unsigned cb;
	BOOL fAllocHasSize;
	HMSOINST pinst;
}MSOBE;


/****************************************************************************
	The IMsoDebugUser interface has Debug methods for Office to call back
   to the app for debugging information that is common across Office features.
****************************************************************** JIMMUR **/

#undef  INTERFACE
#define INTERFACE  IMsoDebugUser

DECLARE_INTERFACE(IMsoDebugUser)
{
   /* Call the MsoFSaveBe API for all of the structures in this application 
		so that leak detection can be preformed.  If this function returns 
		FALSE the memory check will be aborted. The lparam parameter if the 
		same lparam value passed to the MsoFChkMem API.  This parameter should 
		in turn be passed to the MsoFSaveBe API which this method should call 
		to write out its stuctures. */
   MSOMETHOD_(BOOL, FWriteBe) (THIS_ LPARAM) PURE;

   /* This callback allows the application to abort an on going memory check.
	   If this function return TRUE the memory check will be aborted.  
		If FALSE then the memory check will continue.  The application should 
		check its message queue to determine if the memory check should 
		continue.  The lparam paramater if the same lparam value passed to the 
		MsoFChkMem API.  This allows the application to supply some context if 
		it is required. */
   MSOMETHOD_(BOOL, FCheckAbort) (THIS_ LPARAM) PURE;

   /* This callback is called when duplicate items are  found in the heap.
      This provides a way for an applications to manage its referenced counted
		items.  The prgbe parameter is a pointer to the array of MSOBE records. The
		ibe parameter is the current index into that array.  The cbe parameter
		is the count of BEs in the array.  This method should look at the MSOBE in
		question and return back the next index that should checked.  A value of
		0 for the return value will designate that an error has occured.*/
   MSOMETHOD_(int, IbeCheckItem) (THIS_ LPARAM lParam, MSOBE *prgbe, int ibe, int cbe) PURE;

	/* This call back is used to aquire the strigstring name of a Bt. This is used
		when an error occurs during a memory integrity check.  Returning FALSE means
		that there is no string.*/
	MSOMETHOD_(BOOL, FGetSzForBt) (THIS_ LPARAM lParam, MSOBE *pbe, int *pcbsz,
												char **ppszbt) PURE;

	/* This callback is used to signal to the application that an assert is
		about to come up.  szTitle is the title of the assert, and szMsg is the
		message to be displayed in the assert, pmb contains the messagebox
		flags that will be used for the assert.  Return a MessageBox return code
		(IDABORT, IDRETRY, IDIGNORE) to stop the current assert processing and
		simulate the given return behavior.  Returns 0 to proceed with default
		assert processing.  The messagebox type can be changed by modifying
		the MB at *pmb.  iaso contains the type of assert being performed */
	MSOMETHOD_(int, PreAssert) (THIS_ int iaso, char* szTitle, char* szMsg, UINT* pmb) PURE;

	/* This callback is used to signal to the application that an assert has 
		gone away.  id is the MessageBox return code for the assert.  The return
		value is used to modify the MessageBox return code behavior of the
		assert handler */
	MSOMETHOD_(int, PostAssert) (THIS_ int id) PURE;
};

MSOAPI_(BOOL) MsoFWriteHMSOINSTBe(LPARAM lParam, HMSOINST hinst);
#endif // VSMSODEBUG


/****************************************************************************
	Initialization of the Office DLL
****************************************************************************/

/* Initialize the Office DLL.  Each thread of each EXE or DLL using the
	Office DLL must call this function.  On Windows, 'hwndMain' is the hwnd of
	the app's main window, and is used to detect context switches to other 
	Office apps, and to send RPC-styles messages from one office dll to another.
	On the Mac, this used to establish window ownership (for WLM apps), and can
	be NULL for non-WLM apps.  The 'hinst' is the HINSTANCE of 
	the EXE or DLL.  The interface 'piuser' must implement the IMsoUser 
	interface for this use of Office.  wzHostName is a pointer to the short name
	of the host to be used in menu item text. It must be no longer than 32
	characters including the null terminator.
	The HMSOINST instance reference
	for this use of Office is returned in 'phinst'.  Return fSuccess. */
MSOAPI_(BOOL) MsoFInitOffice(HWND hwndMain, HINSTANCE hinstClient, 
									  IMsoUser *piuser, const WCHAR *wzHostName,
									  HMSOINST *phinst);

/* As above, but establishes a app specific registry entry to check an apps last
	UI Language.  This is compared to the current UI lang and can then correctly
	tell the app and COM addins when the lang has changed under it. */
MSOAPI_(BOOL) MsoFInitOfficeEx(HWND hwndMain, HINSTANCE hinstClient, 
									  IMsoUser *piuser, const WCHAR *wzHostName,
									  HMSOINST *phinst, MSOREGLANG* pMLRApp);

/* Uninitialize the Office DLL given the HMSOINST as returned by
	MsoFInitOffice.  The 'hinst' is no longer valid after this call. */
MSOAPI_(void) MsoUninitOffice(HMSOINST hinst);

/* This API is called by when a new thread is created which may use the
   Office memory allocation functions. */
MSOAPI_(BOOL) MsoFInitThread(HANDLE hThread);

/* This API is called by when a thread is which may use the Office memory
	allocation functions is about to be destroyed. */
MSOAPI_(void) MsoUninitThread(void);

/* These APIs are called when a thread which may use the Office memory
   allocation functions has been suspended/resumed. */
MSOAPI_(void) MsoThreadSuspended(void);
MSOAPI_(void) MsoThreadResumed(void);

/* Load and register the Office OLE Automation Type Library by searching
	for the appropriate resource or file (don't use existing registry entries).  
	Return typelib in ppitl or just register and release if ppitl is NULL.
	Return HRESULT returned	from LoadTypeLib/RegisterTypeLib. */
MSOAPI_(HRESULT) MsoHrLoadTypeLib(ITypeLib **ppitl);

/* This API is used by Office clients in their implementation of 
	IVbaProjecSite::HostCheckReference. This API returns Minor version of a typelib
	that is still 100% binary compatible with the current version of a typelib 

	return HRESULT is:
		NOERROR if rgguid was handled.
		S_FALSE	if rgguid is not handled
*/
MSOAPI_(HRESULT) MsoHrCheckMsoTypeLibReference(int fSave, REFGUID rgguid, UINT *puMajor, UINT* puMinor);

/* Register everything that Office needs in the registry for a normal user
	setup (e.g. typelib, proxy interfaces).  Return NOERROR or an HRESULT
	error code. */
MSOAPI_(HRESULT) MsoHrRegisterAll();

/* Same as MsoHrRegisterAll except takes the szPathOleAut param which specifies 
	the path name to an alternate version of oleaut32.dll to load and use. */
MSOAPIX_(HRESULT) MsoHrRegisterAllEx(char *szPathOleAut);

/* Unregister anything that is safe and easy to unregister.
	Return NOERROR or an HRESULT error code. */
MSOAPIX_(HRESULT) MsoHrUnregisterAll();

/* Reset the hwndMain of the hinst to the passed in hwndMain.  -- Word::Stevera */
MSOAPI_(BOOL) MsoFSetInstHwndMain(HMSOINST hinst, HWND hwndMain);

/*	Apps can call this when they start to shutdown.  Office can use this to
	ignore subsequent clicks on the assistant, etc. */
MSOAPI_(void) MsoStartShutdown(void);

/* Apps can call this to get an IDispatch interface to the Answer Wizard object. */
MSOAPI_(BOOL) MsoFGetIDispatchAnswerWizard(HMSOINST hinst, IDispatch **ppidisp);

#if VSMSODEBUG
	/* Add the IMsoDebugUser interface to the HMSOINST instance reference.
	   Return fSuccess. */
	MSOAPI_(BOOL) MsoFSetDebugInterface(HMSOINST hinst, IMsoDebugUser *piodu);
	MSOAPI_(BOOL) MsoFGetDebugInterface(HMSOINST hinst, IMsoDebugUser **ppiodu);

#endif

#define msopuigrfFreeCtlMem  0x00000001

/* Used to Clean up the Office dll before calling MsoUninitOffice. Only called
	in rare or catastrophic events. 

	If you are planning to add a new grf, then you should make sure that there
	is NO other way of doing what you want to do before adding the new grf. 

	If you are planning to call MsoPreUnInitOffice you should try to find a
	better way to accomplish this and just let MsoUninitOffice do its job
	like it is supposed to.

	msopuigrfFreeCtlMem - used to free the CtlMem chain since app is dying 
					in a catastrophic way. Needs to be called before they
					overflow the stack.
	
*/
MSOAPI_(void) MsoPreUnInitOffice(HMSOINST hinst, DWORD grfUninit);

/****************************************************************************
	Other APIs of global interest
****************************************************************************/

/* A generic implementation of QueryInterface for an object given by pisu
	with a single ISimpleUnknown-derived interface given by riidObj.  
	Succeeds only if riidQuery == riidObj or ISimpleUnknown.  
	Returns NOERROR and pisu in *ppvObj if success, else E_NOINTERFACE. */
MSOAPI_(HRESULT) MsoHrSimpleQueryInterface(ISimpleUnknown *pisu, 
							REFIID riidObj, REFIID riidQuery, void **ppvObj);

/* Like MsoHrSimpleQueryInterface except succeeds for either riidObj1
	or riidObj2, returning pisu in both cases and therefore useful for
	inherited interfaces. */
MSOAPI_(HRESULT) MsoHrSimpleQueryInterface2(ISimpleUnknown *pisu, 
							REFIID riidObj1, REFIID riidObj2, REFIID riidQuery, 
							void **ppvObj);

/* This message filter is called for EVERY message the host app receives.
	If the procedure processes it should return TRUE otw FALSE. */
MSOAPI_(BOOL) FHandledLimeMsg(MSG *pmsg);


/*************************************************************************
	MSOGV -- Generic Value

	Currently we have a bunch of fields in Office-defined structures
	with names like pvClient, pvDgs, etc.  These are all declared as
	void *'s, but really they're just for the user of Office to stick
	some data in an Office structure.

	The problem with using void * and calling these fields pvFoo is that
	people keep assuming that you could legitimately compare them against
	NULL and draw some conclusion (like that you didn't need to call the
	host back to free	stuff).  This tended to break hosts who were storing
	indices in these fields.

	So I invented "generic value" (great name, huh?)  Variables of this
	type are named gvFoo.  Almost by definition, there is NO gvNil.

	This type will always be unsigned and always big enough to contain
	either a uint or a pointer.  We don't promise that this stays the
	same length forever, so don't go saving them in files.
************************************************************ PeterEn ****/
typedef void *MSOGV;
#define msocbMSOGV (sizeof(MSOGV))


/*************************************************************************
	MSOCLR -- Color

	This contains "typed" colors.  The high byte is the type,
	the low three are the data.  RGB colors have a "type" of zero.
	It'd be cool you could just cast a COLORREF to an MSOCR and have it
	work (for that to work we'd have to define RGB colors by something
	other than a zero high byte)

	TODO peteren:  These used to be called MSOCR, but cr was a really bad
	hungarian choice for this, it intersects with COLORREF all over the
	place an in the hosts.  I renamed it MSOCLR.  See if we can replace
	some of the "cr" with "clr"

	TODO peteren
	TODO johnbo

	We don't really use this type everywhere we should yet.
************************************************************ PeterEn ****/
typedef ULONG MSOCLR;
#define msocbMSOCLR (sizeof(MSOCLR))
#define msoclrNil   (0xFFFFFFFF)
#define msoclrBlack (0x00000000)
#define msoclrWhite (0x00FFFFFF)
#define msoclrNinch (0x80000001)
#define MsoClrFromCr(cr) ((MSOCLR)(cr & 0x00FFFFFF))
	/* Converts a Win32 COLORREF to an MSOCLR */

/* Old names, remove these */
#define MSOCR MSOCLR
#define msocbMSOCR msocbMSOCLR
#define msocrNil   msoclrNil
#define msocrBlack msoclrBlack
#define msocrWhite msoclrWhite
#define msocrNinch msoclrNinch

/* MsoFGetColorString returns the name of a color. We'll fill out WZ
	with a string of at most cchMax character, not counting the 0 at the end.
	We return TRUE on success.  If you give us a non-NULL pcch will set *pcch
	to the number of characters in the string.
	If you have a COLORREF you can convert with MsoClrFromCr(cr). */
MSOAPI_(BOOL) MsoFGetColorString(MSOCLR clr, WCHAR *wz, int cchMax, int *pcch);

/* MsoFGetSplitMenuColorString returns a string for a split menu.

	If idsItem is not msoidsNil, we'll just insert the string for idsItem
	into the string for idsPattern and return the result in wz.
	
	If idsItem is msoidsNil, we'll try to get a string from the MSOCLR
	using MsoFGetColorString.  If that fails, we'll use
	msoidsSplitMenuCustomItem. */
MSOAPI_(BOOL) MsoFGetSplitMenuColorString(int tcidPattern, int fItem, MSOCLR clr, 
												  WCHAR *wz, int cchMax, int *pcch);


/*************************************************************************
	Stream I/O Support Functions

  	MsoFByteLoad, MsoFByteSave, MsoFWordLoad, MsoFWordSave, etc.
	The following functions are helper functions to be used when loading or
	saving toolbar data using an OLE 2 Stream.  They take care of the stream
	I/O, byte swapping for consistency between Mac and Windows, and error
	checking.  They should be used in all FLoad/FSave callback functions. 
	MsoFWtzLoad expects wtz to point at an array of 257 WCHARs.  MsoFWtzSave
	will save an empty string if wtz is passed as NULL.
	
	SetLastError:  can be set to values from IStream's Read and Write methods
************************************************************ WAHHABB ****/
MSOAPIX_(BOOL) MsoFByteLoad(LPSTREAM pistm, BYTE *pb);
MSOAPIX_(BOOL) MsoFByteSave(LPSTREAM pistm, const BYTE b);
MSOAPI_(BOOL) MsoFWordLoad(LPSTREAM pistm, WORD *pw);
MSOAPI_(BOOL) MsoFWordSave(LPSTREAM pistm, const WORD w);
MSOAPI_(BOOL) MsoFLongLoad(LPSTREAM pistm, LONG *pl);
MSOAPI_(BOOL) MsoFLongSave(LPSTREAM pistm, const LONG l);
MSOAPIX_(BOOL) MsoFWtzLoad(LPSTREAM pistm, WCHAR *wtz);
MSOAPIX_(BOOL) MsoFWtzSave(LPSTREAM pistm, const WCHAR *wtz);


/****************************************************************************
	The IMSoPref (Preferences File) Interface provides a platform independent
	way to maintain settings, using a preferences file on the Macintosh, and
	a registry subkey on Windows
************************************************************** BenW ********/

#define inifAppOnly   1	// tons of these
#define inifExcelOnly 1 // tons of these.  Old comment:  /* EXCEL.INI only */
#define inifSysOnly   2	// only one use of this: xl\dde2.c
#define inifCache     4 // tons of these, but always ORed with inifApp|ExcelOnly?

// This order is assumed in util.cpp SET::CbQueryProfileItemIndex
enum
{
	msoprfNil = 0,
	msoprfInt = 1,
	msoprfString = 2,
	msoprfBlob = 3
};

#undef  INTERFACE
#define INTERFACE  IMsoPref

DECLARE_INTERFACE(IMsoPref)
{
	//*** FDebugMessage method ***
	MSODEBUGMETHOD

	// IMsoPref methods
	MSOMETHOD_(int, LQueryProfileInt) (THIS_ const WCHAR *, const WCHAR *, int, int) PURE;
	MSOMETHOD_(int, CchQueryProfileString) (THIS_ const WCHAR *wzSection,
			const WCHAR *wzKey, const WCHAR *wzDefault, WCHAR *wzValue,
			int cchMax, int inif) PURE;
	MSOMETHOD_(int, CbQueryProfileBlob) (THIS_ const WCHAR *, const WCHAR *, BYTE *, int, BYTE *, int, int) PURE;
	MSOMETHOD_(BOOL, FWriteProfileInt) (THIS_ const WCHAR *, const WCHAR *, int, int) PURE;
	MSOMETHOD_(BOOL, FWriteProfileString) (THIS_ const WCHAR *, const WCHAR *, const WCHAR *, int) PURE;
	MSOMETHOD_(BOOL, FWriteProfileBlob)(THIS_ const WCHAR *, const WCHAR *, const BYTE *, int, int) PURE;
	MSOMETHOD_(BOOL, FDelProfileSection)(THIS_ const WCHAR *) PURE;
	MSOMETHOD_(BOOL, CbQueryProfileItemIndex)	(THIS_ const WCHAR *wzSection, int ikey, WCHAR *wzKey, int cchMaxKey, BYTE *pbValue, int cbMaxValue, int *pprf, int inif) PURE;
};

enum
{
	msoprfUser = 0x0000,	// use HKEY_CURRENT_USER
	msoprfMachine = 0x0001,	// use HKEY_LOCAL_MACHINE
	msoprfIgnoreReg = 0x8000,	// always return defaults
};

MSOAPI_(BOOL) MsoFCreateIPref(const WCHAR *wzPref, 
		const WCHAR *wzUnused, long lUnused1, long lUnused2, 
		int prf, int wUnused3, IMsoPref **ppipref);

MSOAPI_(void) MsoDestroyIPref(IMsoPref *);

MSOAPIMX_(int) MsoCchGetUsersFilesFolder(WCHAR *wzFilename);

#ifdef MAPIVIM
/*	Returns the a full pathname to the MAPIVIM DLL in wzPath.   */
MSOAPI_(int) MsoFGetMapiPath(WCHAR* wzPath, BOOL fInstall);
#endif // MAPIVIM

MSOAPIMX_(WCHAR *) MsoWzGetKey(const WCHAR *wzApp, const WCHAR *wzSection, WCHAR *wzKey);


/*-------------------------------------------------------------------------
	MsoFGetCursorLocation

	Given the name of an animated cursor, returns the file where that cursor
	is found by looking up the name in the Cursors section of the Office prefs.
	
	On Windows, we return the name of a .CUR or .ANI file.
	On the Mac, we return the name of a single file which contains all the cursors.
	NULL means to use the cursors in the Office Shared Library.
	
	For Office 97, this is NYI on the Mac

	Returns fTrue is a cursor was found, fFalse otherwise.

------------------------------------------------------------------ BENW -*/
MSOAPI_(BOOL) MsoFGetCursorLocation(WCHAR *wzCursorName, WCHAR *wzFile);

/****************************************************************************
	The IMsoSplashUser interface is implemented by a user wishing to
	display a splash screen
************************************************************** SHAMIKB *****/

#undef  INTERFACE
#define INTERFACE  IMsoSplashUser

DECLARE_INTERFACE(IMsoSplashUser)
{
	MSOMETHOD_(BOOL, FCreateBmp) (THIS_ BITMAPINFO** pbi, void** pBits) PURE;
	MSOMETHOD_(BOOL, FDestroyBmp) (THIS_ BITMAPINFO* pbi, void* pBits) PURE;
	MSOMETHOD_(void, PreBmpDisplay) (THIS_ HDC hdcScreen, HWND hwnd, BITMAPINFO* pbi, void* pBits) PURE;
	MSOMETHOD_(void, PostBmpDisplay) (THIS_ HDC hdcScreen, HWND hwnd, BITMAPINFO *pbi, void* pBits) PURE;
	MSOMETHOD_(void, ProvideTextSize) (THIS_ HDC hdcScreen, HWND hwnd, DWORD* cxSize, DWORD* cySize) PURE;
	MSOMETHOD_(void, TextOnlyDisplay) (THIS_ HDC hdcScreen, HWND hwnd) PURE;
	MSOMETHOD_(void, TextDispose) (THIS) PURE;
};

// APIs for displaying splash screen

// Note:  The fDoHydra parameter for MsoFShowStartup is used to indicate support for the ProvideTextSize
// and TextOnlyDisplay interfaces, which are used to generate a text-only splash screen under Hydra.
// Passing FALSE will force full-bitmap splash screen display even under Hydra.
MSOAPI_(BOOL) MsoFShowStartup(HWND hwndMain, BITMAPINFO* pbi, void* pBits, IMsoSplashUser *pSplshUser, BOOL fDoHydra);
//MSOAPI_(void) MsoUpdateStartup();
#define MsoUpdateStartup()
MSOAPI_(void) MsoDestroyStartup();


/****************************************************************************
	Stuff about File IO
************************************************************** PeterEn *****/

/* MSOFO = File Offset.  This is the type in which Office stores seek
	positions in files/streams.  I kinda wanted to use FP but that's already
	a floating point quantity. Note that the IStream interfaces uses
	64-bit quantities to store these; for now we're just using 32.  These
	are exactly the same thing as FCs in Word. */
typedef ULONG MSOFO;
#define msofoFirst ((MSOFO)0x00000000)
#define msofoLast  ((MSOFO)0xFFFFFFFC)
#define msofoMax   ((MSOFO)0xFFFFFFFD)
#define msofoNil   ((MSOFO)0xFFFFFFFF)

/* MSODFO = Delta File Offset.  A difference between two MSOFOs. */
typedef MSOFO MSODFO;
#define msodfoFirst ((MSODFO)0x00000000)
#define msodfoLast  ((MSODFO)0xFFFFFFFC)
#define msodfoMax   ((MSODFO)0xFFFFFFFD)
#define msodfoNil   ((MSODFO)0xFFFFFFFF)


/*-----------------------------------------------------------------------------
	MSOEAD (Mso Encrytion Algorithm Descriptor
-------------------------------------------------------------------- HAILIU -*/
#define msoeadfOffice   0x1  // office implemented RC4 encryption
#define msoeadfXor      0x2  // very weak XOR encryption
#define msoeadfCryptAPI 0x4  // CryptAPI implemented encryption (could be any algorithm)
#define msoeadfExcludeDocProps 0x8 // Don't encrypt doc properterties
#define msoeadfMask     0x7  // mask to get the encryption type

typedef struct _msoead
	{
	DWORD   dwEadf;
	DWORD   cbExtra;   // forward compatibility
	int     algid;     // encryption algo id
	int     algidHash; // hashing algo id
	UINT    cbitKey;   // encryption key length
	DWORD   dwProvType;// provider type
	LPCWSTR wzProv;    // Crypt Service Provider name
	LPBYTE  pbExtra;   // forward compatibility
	} MSOEAD;


/****************************************************************************
	Defines the IMsoCryptSession interface

	Use this interface to encrypt or decrypt data.  In the future, perhaps
	the Crypto API can be hooked up underneath.  For now, the encryption will
	be linked to office directly.
***************************************************************** MarkWal **/
#undef INTERFACE
#define INTERFACE IMsoCryptSession

DECLARE_INTERFACE(IMsoCryptSession)
{
	MSODEBUGMETHOD

	/* discard this crypt session */
	MSOMETHOD_(void, Free) (THIS) PURE;

	/* reset the encryptor to a boundary state vs. continuing current
		stream.  iBlock indicates which block boundary to reset to. */
	MSOMETHOD_(void, Reset) (THIS_ unsigned long iBlock) PURE;

	/* encrypts the buffer indicated by pv inplace.  cb indicates
		how long the data is.  Encryption can change the length of the
		data if block algorithms are allowed via cbBlock non-zero on
		the call to MsoFCreateCryptSession.  In that case, *pcbNew is set
		to the new size of the buffer.  In any other case pcbNew may be NULL. */
	MSOMETHOD_(void, Crypt) (THIS_ unsigned char *pb, int cb, int *pcbNew) PURE;
	/* decrypts the buffer indicated by pv inplace.  cb indicates
		how long the data is.  Encryption can change the length of the
		data if block algorithms are allowed via cbBlock non-zero on
		the call to MsoFCreateCryptSession.  In that case, *pcbNew is set
		to the new size of the buffer.  In any other case pcbNew may be NULL. */
	MSOMETHOD_(void, Decrypt) (THIS_ PBYTE pb, int cb, int *pcbNew) PURE;

	/* set the password to the indicated string.  Also, resets the algorithm */
	MSOMETHOD_(BOOL, FSetPass) (THIS_ const WCHAR *wtzPass) PURE;

	/* if the encryption algorithm is a block algorithm, CbBlock indicates the
		block size.  A buffer passed in to Encrypt may grow to a CbBlock
		boundary. */
	MSOMETHOD_(int, CbBlock) (THIS) PURE;

	/* make this crypt session persistent so it can be loaded by 
		MsoFLoadCryptSession, stream should be positioned correctly
		before calling FSave and it will be positioned at the next byte
		when it returns */
	MSOMETHOD_(BOOL, FSave) (THIS_ LPSTREAM pistm) PURE;

	/* make a duplicate of this crypt session */
	MSOMETHOD_(BOOL, FClone) (THIS_ interface IMsoCryptSession **ppics) PURE;

	/* return the pead */
	MSOMETHOD_(MSOEAD*, Pead) (THIS) PURE;

	/* Set the encrpytion stream which contains all the encrypted sub storage
		When passed in, the stream pointer is assumed to be at 0. Caller must
		set fCreate to TRUE if the stream passed in is a blank stream (in save
		scenario). In load scenario, fCreate should be FALSE */
	MSOMETHOD(HrSetEncryptionStream)(THIS_ LPSTREAM pistm, BOOL fCreate) PURE;
	
	/* For encryption, add the storage to the encryption stream. If fReserve is
		TRUE, the function simply reserve the name wzStg so no one else can get
		use it. pistg is ignored in this case can be NULL. If fReserve is FALSE,
		then pistg can not be NULL, this method will stream pistg and encrypted
		the result stream in the encryption stream set through
		HrSetEncryptionStream */
	MSOMETHOD(HrAddStg)(THIS_ LPCWSTR wzStg, LPSTORAGE pistg, BOOL fReserve) PURE;
	
	/* For decryption, get the storage from the encrpytion stream
		This should be called to load back the stg saved through HrAddStg */
	MSOMETHOD(HrGetStg)(THIS_ LPCWSTR wzStg, LPSTORAGE *ppistg) PURE;

	/* Same as HrAddStg but with IStream */
	MSOMETHOD(HrAddStm)(THIS_ LPCWSTR wzStm, LPSTREAM pistm, BOOL fReserve) PURE;

	/* Same as HrGetStg but with IStream */
	MSOMETHOD(HrGetStm)(THIS_ LPCWSTR wzStm, LPSTREAM *ppistm) PURE;

	/* Write the lookup table for the encrypted sub-storage/stream. No more sub
		Elements can be added once this method is called */
	MSOMETHOD(HrWriteSubTable)(THIS) PURE;

	/* Get the stream back, the app usually doesn't need to call this method.
		The only exception is XL which use to method to enable it to save to a
		stg opened with STGM_SIMPLE */
	MSOMETHOD(HrGetEncryptionStream)(THIS_ LPSTREAM *ppistm) PURE;
};


/*-----------------------------------------------------------------------------
|	MSOAPI_	MsoFEncrypt
| Determine whether the languauge is French Standard	
|	
|	
|	Arguments:
|		None
|	
|	Returns:
|			BOOL: True if Language != French (Standard); else false
|	Keywords:
|	
------------------------------------------------------------SALIMI-----------*/
MSOAPI_(BOOL) MsoFEncrypt();


/*-----------------------------------------------------------------------------
	MsoFreePead
-------------------------------------------------------------------- HAILIU -*/
MSOAPI_(VOID) MsoFreePead(MSOEAD *pead);	


/*-----------------------------------------------------------------------------
	MsoPeadClone
-------------------------------------------------------------------- HAILIU -*/
MSOAPI_(MSOEAD*) MsoPeadClone(MSOEAD *pead);


#if VSMSODEBUG
/*-----------------------------------------------------------------------------
	MsoDebugPead
-------------------------------------------------------------------- HAILIU -*/
MSOAPI_(BOOL) MsoDebugPead(MSOEAD *pead, HMSOINST hinst, UINT dm, WPARAM wparam,
	LPARAM lparam);
#endif // VSMSODEBUG

/*-----------------------------------------------------------------------------
	MsoFChooseEncryptionAllowed

	Check registry setting and see if the admin has disabled user from choosing
	encryption algorithms. 
-------------------------------------------------------------------- HAILIU -*/
MSOAPI_(BOOL) MsoFChooseEncryptionAllowed(void);


/*-----------------------------------------------------------------------------
	MsoPeadGetDefault
-------------------------------------------------------------------- HAILIU -*/
MSOAPI_(MSOEAD*) MsoPeadGetDefault(DWORD grfead);


/*-----------------------------------------------------------------------------
	MsoPeadChoose
-------------------------------------------------------------------- HAILIU -*/
MSOAPI_(MSOEAD*) MsoPeadChoose(HWND hwndParent, HMSOINST hinst,
	MSOEAD *peadDefault, DWORD grfead);

/*-----------------------------------------------------------------------------
	MsoFCreateCryptSession

	Create a new crypt session accodring to pead. The created session will
	take owner ship of the pead (even if it fails). Therefore the caller should
	not call MsoFreePead or MsoDebugPead after calling this function. This is
	done to avoid cloning pead
-------------------------------------------------------------------- HAILIU -*/
MSOAPI_(BOOL) MsoFCreateCryptSession(const WCHAR *wtzPass, MSOEAD *pead,
	interface IMsoCryptSession **ppics, int cbBlock);


/*-----------------------------------------------------------------------------
	MsoFLoadCryptSession

	Load a crypt session
-------------------------------------------------------------------- HAILIU -*/
MSOAPI_(BOOL) MsoFLoadCryptSession(const WCHAR *wtzPass, IStream *pistm,
	interface IMsoCryptSession **ppics, MSOEAD **ppead, int cbBlock);


/*-----------------------------------------------------------------------------
	MsoBstrAlgoFromPead
-------------------------------------------------------------------- HAILIU -*/
MSOAPI_(BSTR) MsoBstrAlgoFromPead(MSOEAD *pead);


/*-----------------------------------------------------------------------------
	MsoBstrProvFromPead
-------------------------------------------------------------------- HAILIU -*/
MSOAPI_(BSTR) MsoBstrProvFromPead(MSOEAD *pead);

/*-----------------------------------------------------------------------------
	MsoPeadFromWzs
-------------------------------------------------------------------- HAILIU -*/
MSOAPI_(MSOEAD *) MsoPeadFromWzs(DWORD grfead, LPCWSTR wzProv, LPCWSTR wzAlgo,
	int cbitKey, BOOL fEncryptDocProps);
	

/*-----------------------------------------------------------------------------
	MsoFRecordPead
-------------------------------------------------------------------- HAILIU -*/
MSOAPI_(BOOL) MsoFRecordPead(MSOEAD *pead, UINT appId);


/****************************************************************************
	Office ZoomRect animation code
****************************************************************************/
MSOAPI_(void) MsoZoomRect(RECT *prcFrom, RECT *prcTo, BOOL fAccelerate, HRGN hrgnClip);
MSOAPI_(void) MsoZoomRectEx(RECT *prcFrom, RECT *prcTo, BOOL fAccelerate, HRGN hrgnClip, int delay);

// Idle Initialization stuff

// Idle Init structure
typedef struct tagMSOIDLEINIT
{
	BOOL (*pfnIdleInit)(void);
} MSOIDLEINIT;

/*---------------------------------------------------------------------------
	MsoFRegisterAppIdleInitTasks

	Register the app's idle init task list with the idle init manager.
---------------------------------------------------------------- EricSchr -*/
MSOAPIX_(BOOL) MsoFRegisterAppIdleInitTasks(MSOIDLEINIT *pAppIdleInit,
	DWORD cItems);

#if VSMSODEBUG
/*	Allows testing to turn off idle initialization at any desired point. */
MSOAPIXX_(void) MsoDisableIdleInit();
/*	Simulates plenty of idle time so that all idle init tasks are executed
	- tests that they all work. */
MSOAPIXX_(void) MsoDoAllIdleInit();
#endif

// Idle Init helper macros
#define IndexFromIif(iif)   ((iif) >> 8)
#define MaskFromIif(iif) ((iif) & 0xFF)

#define MsoMarkIdleInitDone(rgIdle, iif) \
	(rgIdle[IndexFromIif(iif)] |= MaskFromIif(iif))

#define MsoFIdleInitDone(rgIdle, iif) \
	(rgIdle[IndexFromIif(iif)] & MaskFromIif(iif))


/*---------------------------------------------------------------------------
	Office Reoccuring Idle definitions
----------------------------------------------------------------- MRuhlen -*/

EXTERN_C void __stdcall DeQueueDoAddCFF(void);  // defined in dmuoldoc.cpp
MSOAPI_(void) MsoDrawingDownloadIdle(void);     // defined in drevent.cpp

typedef struct tagMSOIDLEREOCCUR
{
	void (*pfnIdleReoccur)(void);
	DWORD msec;						// Minimum app uptime before calling
} MSOIDLEREOCCUR;

BOOL MsoFKickStartIdle(void);
void MsoResetIdleTickCount(void);

EXTERN_C DWORD vgffMsoIdleReoccur;
EXTERN_C BOOL  vgfIdleReoccurShutdown;

// idle reoccur flags
// one bit for each reoccuring idle task.


#define irfIdleReoccurStop              0x80000000
#define irfDeQueueDoAddCFF              0x00000001
#define irfAlertIdsCannotSuspendAtIdle  0x00000002
#define irfDRDownloadIdle               0x00000004
#define irfMsoForgetLastGimme           0x00000008
#define irfLicenseSelect                0x00000010
#define irfLicenseAction                0x00000020
#define irfAcbRenderThumbnails          0x00000040
#define irfLicenseEnsurePID             0x00000080
#define irfLicenseValidate              0x00000100
#define irfLicenseWizard                0x00000200
#define irfLicClockChecks               0x00000400


// Helper functions to remove and add a reoccuring idle task.  Pass one of the
// flags above to these to add or remove it from the reoccuring idle list.
// DO NOT USE THESE FROM OUTSIDE MSO.DLL

__inline void RemoveReoccuringIdleIrf(unsigned irf)
{
	vgffMsoIdleReoccur &= ~irf;
}

__inline void AddReoccuringIdleIrf(unsigned irf)
{
	vgffMsoIdleReoccur |= irf;
}

// Office10.233220 -- Access needs to know if reoccuring tasks are left
MSOAPI_(BOOL) MsoFHasReoccuringIdleTasks();


/*	On the Windows side we don't call OleInitialize at boot time - only
	CoInitialize. On the Mac side this is currently not being done because
	the Running Object Table is tied in with OleInitialize - so we can't
	call RegisterActiveObject if OleInitialize is not called - may
	want to revisit this issue. */

/*	Should be called before every call that requires OleInitialize to have
	been called previously. This function calls OleInitialize if it hasn't
	already been called. */
MSOAPI_(BOOL) MsoFEnsureOleInited();
/*	If OleInitialize has been called then calls OleUninitialize */
MSOAPI_(void) MsoOleUninitialize();

// Delayed Drag Drop Registration
/*	These routines are unnecessary on the Mac since Mac OLE doesn't require OLE
    to be initialized prior to using the drag/drop routines */
/*	All calls to RegisterDragDrop should be replaced by
	MsoHrRegisterDragDrop. RegisterDragDrop requires OleInitialize so
	during boot RegisterDragDrop should not be called. This function
	adds the drop target to a queue if OleInitialize hasn't already been
	called. If it has then it just calls RegisterDragDrop. */
MSOAPI_(HRESULT) MsoHrRegisterDragDrop(HWND hwnd, IDropTarget *pDropTarget);

/*	All calls to RevokeDragDrop should be replaced by
	MsoHrRevokeDragDrop. If a delayed queue of drop targets exists
	then this checks the queue first for the target. */
MSOAPI_(HRESULT) MsoHrRevokeDragDrop(HWND hwnd);

/*	Since all drop targets previously registered at boot time are now
	stored in a queue, we need to make sure we register them sometime.
	These can become drop targets
	a. if we are initiating a drag and drop - in which case we call this
	function before calling DoDragDrop (inside MsoHrDoDragDrop).
	b. while losing activation - so we might become the drop target of
	another app. So this function is called from the WM_ACTIVATEAPP
	message handler. */
MSOAPI_(BOOL) MsoFRegisterDragDropList();

/*	This function should be called instead of DoDragDrop - it first
	registers any drop targets that may be in the lazy init queue. */
MSOAPI_(HRESULT) MsoHrDoDragDrop(IDataObject *pDataObject,
	IDropSource *pDropSource, DWORD dwOKEffect, DWORD *pdwEffect);


/*	Module names MsoLoadModule supports */
/*  IF ANY THING IS CHANGED HERE - CHANGE GLOBALS.CPP! */

enum
{
	msoimodUser,		// System User
	msoimodGdi,			// System GDI
	msoimodWinnls,		// System International utilities
	#define msoimodGetMax (msoimodWinnls+1)
	
	msoimodShell,		// System Shell
	msoimodCommctrl,	// System Common controls
	msoimodOleAuto,		// System OLE automation
	msoimodCommdlg,		// System common dialogs
	msoimodVersion,		// System version APIs
	msoimodWinmm,		// System multimedia
	msoimodMapi,		// Mail
	msoimodHlink,		// Hyperlink APIs
	msoimodUrlmon,		// Url moniker APIs
	msoimodJet,			// Jet database
	msoimodOleAcc,		// OLE Accessibility
	msoimodWinsock,		// Network Sockets
	msoimodMpr,			// Windows Network
	msoimodOdma,		// odma
	msoimodWininet,		// internet stuff
	msoimodRpcrt4,		// RPC
	msoimodDarwin,		// Darwin
	msoimodCrypt32,	// crypto dll - digital signing
	msoimodWintrust,	// wintrust.dll - digital signing
	msoimodCryptdlg,	// cryptdlg.dll - digital signing
	msoimodSigner,		// signer.dll - digital signing
	msoimodSoftpub,	// softpub.dll - digital signing
	msoimodAdvapi32,	// advapi32.dll - digital signing
	msoimodVbe,
	msoimodRichEdit,    // Richedit dll 
	msoimodMsoHev,		// Msohev.dll
	msoimodMssign32,	// mssign32.dll - digital signing
	msoimodOlepro32,    // olepro32.dll - OleCreateFontIndirect & OldCreatePictureIndirect
	msoimodCryptui,	// cryptui.dll - IE5 digital signing
	msoimodIE5Crypt32,	// IE5 crypto dll - digital signing
	msoimodWinspool,    // winspool.drv
	msoimodKernel32,    // kernel32.dll
	msoimodShlwapi,		// shlwapi.dll
	msoimodActiveds,        // activeds.dll - Active Directory dll
	msoimodWFF,         // ippwff.dll - IWebFolderForms dll
	msoimodNetapi32,        // netapi32.dll - Network API
	msoimodWtsapi32,	// WTS Api's
	msoimodMscat32,     // mscat32.dll - digital signing
	msoimodMax,
};

/* ifn enums for Modules loaded by MsoLoadModule */
/* THE ORDER MUST MATCH THAT IN GLOBALS.CPP! -- MRuhlen */

enum
{
	ifnFindTextA,
	ifnFindTextW,
	ifnReplaceTextA,
	ifnReplaceTextW,
	ifnGetFileTitleA,
	ifnGetFileTitleW,
	ifnCommDlgExtendedError,

	cfnCommdlg
};	


enum
{
	ifnGetFileVersionInfoA,
	ifnGetFileVersionInfoSizeA,
	ifnVerQueryValueA,
	ifnGetFileVersionInfoW,
	ifnGetFileVersionInfoSizeW,
	ifnVerQueryValueW,

	cfnVersion
};

enum
{
	ifnSHGetDesktopFolder,
	ifnSHGetMalloc,
	ifnSHGetPathFromIDList,
	ifnSHGetPathFromIDListW,
	ifnSHGetDataFromIDListA,
	ifnSHGetDataFromIDListW,
	ifnSHBrowseForFolderA,
	ifnSHBrowseForFolderW,
	ifnSHGetSpecialFolderLocation,
	ifnSHGetFileInfoA,
	ifnSHGetFileInfoW,
	ifnExtractIconExA,
	ifnExtractIconW,
	ifnDllGetClassObject,
	ifnDragQueryPoint,
	ifnDragQueryFileA,
	ifnDragQueryFileW,
	ifnDragFinish,
	ifnDragAcceptFiles,
	ifnExtractIconA,
	ifnShellExecuteA,
	ifnShellExecuteW,
	ifnShellExecuteExA,
	ifnShellExecuteExW,
	ifnSHAppBarMessage,
	ifnFindExecutableA,
	ifnFindExecutableW,
	ifnInvalidateDriveType,
	ifnSHGetSpecialFolderPath,
	ifnSHChangeNotify,
	ifnSHAddToRecentDocs,
	ifnSHFileOperationA,
	ifnSHFileOperationW,
	ifnExtractIconExW,
	ifnSHCoCreateInstance,
	ifnCIDLData_CreateFromIDArray,
	ifnSHCreateLinks,
	ifnShell_NotifyIconA,
	cfnShell
};

enum
{
	ifnVariantInit,
	ifnVariantClear,
	ifnVariantChangeType,
	ifnVariantChangeTypeEx,
	ifnVariantTimeToDosDateTime,
	ifnDosDateTimeToVariantTime,
	ifnSysAllocString,
	ifnSysAllocStringLen,
	ifnSysFreeString,
	ifnSysStringLen,
	ifnSafeArrayGetDim,
	ifnSafeArrayAccessData,
	ifnSafeArrayUnaccessData,
	ifnSafeArrayGetUBound,
	ifnSafeArrayGetLBound,
	ifnLoadRegTypeLib,
	ifnLoadTypeLib,
	ifnQueryPathOfRegTypeLib,
	ifnVariantCopy,
	ifnSafeArrayCreate,
	ifnSafeArrayDestroy,
	ifnSafeArrayGetElement,
	ifnRegisterTypeLib,
	ifnCreateErrorInfo,
	ifnSetErrorInfo,
	ifnGetErrorInfo,
	ifnGetActiveObject,
	ifnSysReAllocStringLen,
	ifnSysReAllocString,
	ifnSysAllocStringByteLen,
	ifnSafeArrayUnlock,
	ifnSafeArrayLock,
	ifnOleCreatePropertyFrameIndirect,
	ifnSysStringByteLen,
	ifnSafeArrayRedim,
	ifnSystemTimeToVariantTime,
	ifnSafeArrayGetElemsize,
	ifnVarBstrFromR8,
	ifnOleLoadPicture,
	ifnRevokeActiveObject,
	ifnRegisterActiveObject,
	ifnDispGetIDsOfNames,
	ifnDispInvoke,
	ifnCreateTypeLib2,
	ifnLHashValOfNameSys,
	ifnVarDateFromUdate,
	ifnVarUdateFromDate,
	ifnGetAltMonthNames,
	ifnSafeArrayPutElement,
	ifnVariantCopyInd,
	ifnSafeArrayAllocData,
	ifnSafeArrayDestroyData,
	ifnVarR4FromR8,
	ifnVarR8FromR4,
	ifnOaBuildVersion,
	ifnLoadTypeLibEx,
	ifnVariantTimeToSystemTime,
	ifnSafeArrayCopy,
	ifnUnRegisterTypeLib,
	ifnSafeArrayPtrOfIndex,
	cfnOleAuto
};

enum
{
	ifnImageList_Destroy,
	ifnImageList_Create,
	ifnImageList_Replace,
	ifnImageList_ReplaceIcon,
	ifnImageList_GetImageCount,
	ifnPropertySheetA,
	ifnPropertySheetW,
	ifnCreateToolbarEx,
	ifnImageList_SetBkColor,
	ifnImageList_GetBkColor,
	ifnImageList_Draw,
	ifnImageList_DrawEx,
	ifnImageList_GetIconSize,
	ifnImageList_SetIconSize,
	ifnImageList_AddMasked,
	ifnInitCommonControls,
	ifnInitCommonControlsEx,
	ifnImageList_LoadImageA,
	ifnImageList_LoadImageW,
	ifnImageList_Add,
	ifnImageList_AddIcon,
	ifnImageList_Merge,
	ifnImageList_GetIcon,
	ifnImageList_SetImageCount,
	ifnImageList_Read,
	ifnImageList_Write,
	ifnImageList_Copy,
	ifnImageList_SetOverlayImage,
	ifnImageList_Remove,
	ifnImageList_BeginDrag,
	ifnImageList_DragEnter,
	ifnImageList_DragLeave,
	ifnImageList_DragMove,
	ifnImageList_EndDrag,
	ifnTrackMouseEvent,
	
	cfnCommctrl
};

enum
{
	ifnHlinkCreateFromMoniker,
	ifnHlinkCreateFromString,
	ifnHlinkCreateFromData,
	ifnHlinkCreateBrowseContext,
	ifnHlinkClone,
	ifnHlinkNavigateToStringReference,
	ifnHlinkOnNavigate,
	ifnHlinkUpdateStackItem,
	ifnHlinkOnRenameDocument,
	ifnHlinkNavigate,
	ifnHlinkResolveMonikerForData,
	ifnHlinkResolveStringForData,
	ifnHlinkParseDisplayName,
	ifnHlinkQueryCreateFromData,
	ifnHlinkSetSpecialReference,
	ifnHlinkGetSpecialReference,
	ifnHlinkCreateShortcut,
	ifnHlinkResolveShortcut,
	ifnHlinkIsShortcut,
	ifnHlinkResolveShortcutToString,
	ifnHlinkCreateShortcutFromString,
	ifnHlinkGetValueFromParams,
	ifnHlinkCreateShortcutFromMoniker,
	ifnHlinkResolveShortcutToMoniker,
	ifnHlinkTranslateURL,
	ifnHlinkCreateExtensionServices,
	ifnHlinkPreprocessMoniker,
	cfnHlink
};

enum
{
	ifnCreateURLMoniker,
	ifnIsValidURL,
	ifnRegisterMediaTypeClass,
	ifnRegisterBindStatusCallback,
	ifnURLOpenBlockingStreamW,
	ifnRevokeBindStatusCallback,
#ifdef UNUSED
	ifnURLDownloadToFileW,
	ifnURLDownloadToCacheFileW,
#endif // UNUSED
	ifnCoInternetCombineUrl,
	ifnCoInternetGetSession,
	ifnFindMimeFromData,
	ifnCoInternetCreateZoneManager,
	ifnCoInternetParseUrl,
	ifnCreateAsyncBindCtxEx,
	ifnUrlMkGetSessionOption,
	ifnUrlMkSetSessionOption,
	ifnCoInternetCompareUrl,
	ifnCopyStgMedium,
	ifnReleaseBindInfo,
	ifnCoInternetQueryInfo,
	cfnUrlmon
};

enum
{
	ifnMAPIAllocateBuffer,
	ifnMAPIFreeBuffer,
	ifnHrQueryAllRows,
	ifnMAPIAllocateMore,
	ifnMAPILogon,
	ifnMAPILogoff,
	ifnMAPIAddress,
	ifnMAPIResolveName,
	ifnMAPISendMail,
	ifnMAPIInitialize,
	ifnMAPIUninitialize,
	ifnMAPILogonEx,
	ifnMAPIOpenFormMgr,
	ifnMAPIAdminProfiles,
	cfnMapi
};

enum
{
	ifnJetBeginSession,
	ifnJetCloseDatabase,
	ifnJetCloseTable,
	ifnJetEndSession,
	ifnJetGetObjectInfo,
	ifnJetInit,
	ifnJetMove,
	ifnJetOpenDatabase,
	ifnJetRetrieveColumn,
	ifnJetRetrieveProperty,
	ifnJetTerm,
	ifnJetGetTableColumnInfo,
	cfnJet
};

enum
{
	ifnLresultFromObject,
	ifnObjectFromLresult,
	ifnAccessibleObjectFromWindow,
	ifnCreateStdAccessibleObject,
	cfnOleAcc
};

enum
{
	ifnGetAddressByNameA,
	ifnGetAddressByNameW,
	ifnWSAStartup,
	ifnWSACleanup,
	cfnWinsock
};

enum
{
	ifnWNetAddConnection2W,
	ifnWNetAddConnection3W,
	ifnWNetAddConnectionW,
	ifnWNetCancelConnectionW,
	ifnWNetConnectionDialog,
	ifnWNetEnumResourceW,
	ifnWNetGetConnectionW,
	ifnWNetOpenEnumW,
	ifnWNetCloseEnum,
	ifnWNetGetLastErrorW,
	ifnWNetUseConnectionW,
	ifnWNetGetNetworkInformationW,
	ifnWNetAddConnection2A,
	ifnWNetAddConnection3A,
	ifnWNetAddConnectionA,
	ifnWNetCancelConnectionA,
	ifnWNetEnumResourceA,
	ifnWNetGetConnectionA,
	ifnWNetOpenEnumA,
	ifnWNetGetLastErrorA,
	ifnWNetUseConnectionA,
	ifnWNetGetNetworkInformationA,
	ifnWNetGetResourceInformationW,
	ifnWNetGetResourceInformationA,
	cfnMpr
};

enum
{
	ifnInternetCloseHandle,
	ifnInternetWriteFile,
	ifnInternetOpenA,
	ifnInternetOpenW,
	ifnInternetConnectA,
	ifnInternetConnectW,
	ifnFtpOpenFileA,
	ifnFtpGetFileA,
	ifnFtpSetCurrentDirectoryA,
	ifnFtpGetCurrentDirectoryA,
	ifnInternetFindNextFileA,
	ifnFtpFindFirstFileA,
	ifnInternetCombineUrlA,
	ifnGetUrlCacheConfigInfoA,
	ifnInternetCanonicalizeUrlA,
	ifnInternetCanonicalizeUrlW,
	ifnFtpRenameFileA,
	ifnFtpDeleteFileA,
	ifnFtpCreateDirectoryA,
	ifnFtpRemoveDirectoryA,
	ifnInternetCrackUrlA,
	ifnInternetCrackUrlW,
	ifnInternetGetLastResponseInfoW,
	ifnInternetReadFile,
	ifnHttpOpenRequestW,
	ifnHttpOpenRequestA,
	ifnHttpSendRequestW,
	ifnHttpSendRequestA,
	ifnHttpQueryInfoW,
	ifnHttpQueryInfoA,
	ifnInternetGetCookieW,
	ifnInternetSetOptionW,
	ifnInternetSetOptionA,
	ifnCreateUrlCacheEntryW,
	ifnCreateUrlCacheEntryA,
	ifnCommitUrlCacheEntryW,
	ifnCommitUrlCacheEntryA,
	ifnGetUrlCacheEntryInfoW,
	ifnGetUrlCacheEntryInfoA,
	ifnFindFirstUrlCacheEntryExW,
	ifnFindFirstUrlCacheEntryExA,
	ifnFindNextUrlCacheEntryExW,
	ifnFindNextUrlCacheEntryExA,
	ifnFindFirstUrlCacheEntryA,
	ifnFindNextUrlCacheEntryA,
	ifnFindFirstUrlCacheEntryW,
	ifnFindNextUrlCacheEntryW,
	ifnFindCloseUrlCache,
	ifnSetUrlCacheEntryGroupW,
	ifnSetUrlCacheEntryGroupA,
	ifnInternetQueryOptionW,
	ifnInternetQueryOptionA,
	ifnInternetOpenUrlW,
	ifnInternetOpenUrlA,
	ifnInternetGetConnectedState,
	ifnInternetAutodial,
	ifnInternetAutodialHangup,
	ifnInternetErrorDlg,
	ifnInternetGoOnlineA,
	ifnInternetGetConnectedStateExW,
	cfnWininet
};

enum
{
	ifnODMSelectDoc,
	cfnOdma
};

enum
{
	ifnPlaySoundA,
	ifnPlaySoundW,
	ifnwaveOutGetNumDevs,
	cfnWinMM
};

enum
{
	ifnNdrDllGetClassObject,
	ifnNdrDllCanUnloadNow,
	ifnNdrCStdStubBuffer_Release,
	ifnCStdStubBuffer_DebugServerRelease,
	ifnCStdStubBuffer_DebugServerQueryInterface,
	ifnCStdStubBuffer_CountRefs,
	ifnCStdStubBuffer_IsIIDSupported,
	ifnCStdStubBuffer_Invoke,
	ifnCStdStubBuffer_Disconnect,
	ifnCStdStubBuffer_Connect,
	ifnCStdStubBuffer_AddRef,
	ifnCStdStubBuffer_QueryInterface,
	ifnIUnknown_Release_Proxy,
	ifnIUnknown_AddRef_Proxy,
	ifnIUnknown_QueryInterface_Proxy,
	ifnNdrOleFree,
	ifnNdrOleAllocate,
	ifnNdrClientCall,
	cfnRpcrt4
};

enum
{
	ifnMsiGetProductCodeW,
	ifnMsiGetComponentPathW,
	ifnMsiReinstallFeatureW,
	ifnMsiReinstallProductW,
	ifnMsiQueryFeatureStateW,
	ifnMsiQueryProductStateW,
	ifnMsiUseFeatureW,
	ifnMsiGetUserInfoW,
	ifnMsiInstallMissingFileW,
	ifnMsiSetInternalUI,
	ifnMsiInstallProductW,
	ifnMsiEnumComponentQualifiersW,
	ifnMsiProvideQualifiedComponentW,
	ifnMsiVerifyPackageW,
	ifnMsiConfigureFeatureW,
	ifnMsiConfigureProductW,
	ifnMsiConfigureProductExW,
	ifnMsiProvideComponentW,
	ifnMsiInstallMissingComponentW,
	ifnMsiEnableLogW,
	ifnMsiCollectUserInfoW,
	ifnMsiGetProductInfoW,
	ifnMsiSetExternalUIW,
	ifnMsiUseFeatureExW,
	ifnMsiProvideQualifiedComponentExW,
	ifnMsiLocateComponentW,
	ifnMsiEnumComponentQualifiersA,
	ifnMsiEnumClientsW,
	ifnMsiEnumFeaturesW,
	ifnMsiGetFeatureUsageW,
	ifnMsiViewExecute,
	ifnMsiDatabaseOpenViewW,
	ifnMsiOpenDatabaseW,
	ifnMsiCloseHandle,
	ifnMsiRecordGetStringW,
	ifnMsiViewFetch,
	ifnMsiRecordIsNull,
	ifnMsiEnumComponentsW,
	cfnDarwin
};

enum
{
	ifnCertCloseStore,
	ifnCertFreeCertificateContext,
	ifnCertSaveStore,
	ifnCertAddCertificateContextToStore,
	ifnCertOpenStore,
	ifnCertFindCertificateInStore,
	ifnCertAlgIdToOID,
	ifnCryptExportPublicKeyInfo,
	ifnCertGetCertificateContextProperty,
	ifnCryptSIPAddProvider,
	ifnCryptSIPRemoveProvider,
	ifnCertNameToStrW,
	ifnCryptHashCertificate,
	ifnCertOpenSystemStoreA,
	ifnCertGetEnhancedKeyUsage,
	ifnCryptImportPublicKeyInfo,
	ifnCertAddEncodedCertificateToStore,
	ifnCertVerifyTimeValidity,
	ifnCertCompareCertificate,
	ifnCertEnumCertificatesInStore,
	ifnCertDuplicateCertificateContext,
	ifnCertDeleteCertificateFromStore,
	ifnCertGetSubjectCertificateFromStore,
	ifnCertCompareCertificateName,
	ifnCertGetIssuerCertificateFromStore,
	ifnCertVerifySubjectCertificateContext,
	ifnCryptDecodeObject,
	ifnCryptMsgGetParam,
	ifnCryptMsgClose,
	ifnCryptMsgUpdate,
	ifnCryptMsgOpenToDecode,
	ifnCryptImportPublicKeyInfoEx,
	ifnCertComparePublicKeyInfo,
	ifnCertCreateCertificateContext,
	ifnCertFindExtension,
	ifnCryptSignCertificate,
	ifnCryptEncodeObject,
	cfnCrypt32
};

enum
{
	ifnCertFreeCertificateChain,
	ifnCertGetCertificateChain,
	ifnCertVerifyCertificateChainPolicy,
	ifnCertFreeCertificateChainEngine,
	ifnCertCreateCertificateChainEngine,
	ifnCryptQueryObject,
	ifnCertGetNameStringW,
	ifnCryptFindCertificateKeyProvInfo,
	cfnIE5Crypt32
};

enum
{
	ifnWinVerifyTrust,
	ifnWintrustAddActionID,
	ifnWTHelperGetProvCertFromChain,
	ifnWTHelperGetProvSignerFromChain,
	ifnWintrustLoadFunctionPointers,
	ifnWTHelperCertIsSelfSigned,
	ifnWTHelperProvDataFromStateData,
	cfnWintrust
};

enum
{
	ifnCertSelectCertificateA,
	ifnGetFriendlyNameOfCertW,
	ifnCertViewPropertiesA,
	cfnCryptdlg
};

enum
{
	ifnSignerSign,
	ifnSignerTimeStamp,
	cfnSigner
};

enum
{
	ifnSoftpubAuthenticode,
	cfnSoftpub
};

enum
{
	ifnCryptAcquireContextA,
	ifnCryptReleaseContext,
	ifnCheckTokenMembership,
	ifnCryptAcquireContextW,
	cfnAdvapi32
};

enum
{
	ifnDllVbeGetHashOfCode,
	cfnVbe
};

enum
{
	ifnPHevCreateFileInfo,
	ifnWHevParseFile,
	ifnFHevActivateApp,
	ifnHevDestroyFileInfo,
	ifnFHevRegister,
	ifnFHevSetDefaultEditor,
	ifnWHeviAppFromProgId,
	ifnFHevAddToFileInfo,
	ifnHevFGetProgIDFromFile,
	ifnHevFGetCreatorAppIcon,
	ifnHevFGetCreatorAppName,
	ifnHevFQueryDefaultEditor,
	ifnHevFSetExtraData,
	ifnHevFCheckNonMSApp,
	cfnMsoHev
};

enum
{
	ifnSignerSignEx,
	ifnSignerFreeSignerContext,
	ifnSignerTimeStampEx,
	cfnMssign32
};

enum
{
    ifnOleCreateFontIndirect,
    ifnOleCreatePictureIndirect,
    cfnOlepro32
};

enum
{
	ifnCryptUIDlgViewCertificateA,
	ifnCryptUIDlgSelectCertificateA,
	ifnCryptUIDlgViewSignerInfoA,
	cfnCryptui
};

enum
{
	ifnGetPrinterW,
	ifnGetPrinterA,
	ifnDeviceCapabilitiesW,
	ifnDeviceCapabilitiesA,
	ifnOpenPrinterW,
	ifnOpenPrinterA,
	ifnDocumentPropertiesW,
	ifnDocumentPropertiesA,
	ifnEnumPrintersA,
	ifnEnumJobsA,
	ifnGetPrinterDriverA,
	ifnClosePrinter,
	ifnEnumPrintersW,
	ifnEnumJobsW,
	ifnGetPrinterDriverW,
	ifnAddPrinterDriverA,
	ifnAddPrinterDriverW,
	ifnGetPrinterDriverDirectoryA,
	ifnGetPrinterDriverDirectoryW,
	ifnDeletePrinter,
	ifnAddPrinterA,
	ifnAddPrinterW,
	ifnAddPrinterConnectionW,
	cfnWinspool
};

enum
{
	ifnGetLongPathNameA,
	ifnGetLongPathNameW,
	ifnProcessIdToSessionId,
	cfnKernel32
};

enum
{
	ifnPathQuoteSpacesW,
	ifnPathFindFileNameW,
	ifnPathRemoveArgsW,
	ifnPathUnquoteSpacesW,
	ifnPathRemoveFileSpecW,
	ifnPathIsURLW,
	ifnSHOpenRegStreamA,
	ifnSHOpenRegStreamW,

	cfnShlwapi
};

enum
{
	ifnADsOpenObject,

	cfnActiveds
};

enum
{
	ifnGetIWFFPtr,
	cfnWFF
};

enum
{
   ifnNetbios,
   cfnNetapi32
};

enum
{
	ifnWTSQuerySessionInformationW,
	ifnWTSFreeMemory,
	cfnWtsapi32
};

enum
{
	ifnCryptCATAdminCalcHashFromFileHandle,
	cfnMscat32
};

// we don't bother loading any functions out of these modules
#define cfnUser 0
#define cfnGdi 0
#define cfnWinnls 0
#define cfnWinmm 0
#define cfnRichEdit 0


/*	Returns the module handle of the given module imod. Loads it if it is
	not loaded already.  fForceLoad will force a LoadLibrary on the DLL
	even if it is already in memory. */
MSOAPI_(HINSTANCE) MsoLoadModule(int imod, BOOL fForceLoad);

MSOAPIX_(void) MsoFreeModule(int imod);

MSOAPI_(BOOL) MsoFModuleLoaded(int imod);

/*	Returns the proc address in the module imod of the function
	szName.  Returns NULL if the module is not found or if the entry
	point does not exist in the module. */
MSOAPI_(FARPROC) MsoGetProcAddress(int imod, const char* szName);


/*	This API should be called by the client before MsoFInitOffice to set
	our locale id so that we can load the correct international dll.
	Defaults to the user default locale if app doesn't call this API before. */
MSOAPI_(void) MsoSetLocale(LCID dwLCID);


#define msobtaNone			0
#define msobtaPreRelease	1
#define msobtaOEM			2
#define msobtaOEMCD			3
#define msobtaOEMFixed		4
#define msobtaUnlock			5

/*	Puts the Office DLL in "beta-mode".  When we're in beta mode, we do
	our beta expiration test in MsoFInitOffice. There are 2 kinds of betas:
	msobtaPreRelease:	look in the intl DLL for a hardcoded expiration date
						(Apps should make this call if they ship a beta after
						mso97.dll RTM, i.e. FE betas)
	msobtaOEM:			apps expire 90 days after first boot
	msobtaOEMCD:		same as msobtaOEM, except setup sets the date -- UNUSED FOR NOW
	msobtaOEMFixed:		same as msobtaPreRelease, except a different string
	msobtaNone:			No effect */
MSOAPIX_(void) MsoSetBetaMode(int bta);

/* Cover for standard GetTextExtentPointW that:
	1. Uses GetTextExtentPoint32W on Win32 (more accurate)
	2. Fixes Windows bug	when cch is 0.  If cch is 0 then the correct dy 
		is returned and dx will be 0.  Also, if cch is 0 then wz can be NULL. */
MSOAPI_(BOOL) MsoFGetTextExtentPointW(HDC hdc, const WCHAR *wz, int cch, LPSIZE lpSize);

/* Covers for Windows APIs that need to call the W versions if on a 
	Unicode system, else the A version. */
MSOAPI_(LRESULT) MsoDispatchMessage(const MSG *pmsg);
MSOAPI_(LRESULT) MsoSendMessage(HWND hwnd, UINT msg, WPARAM wParam, LPARAM lParam);
MSOAPI_(LONG) MsoPostMessage(HWND hwnd, UINT msg, WPARAM wParam, LPARAM lParam);
MSOAPI_(LRESULT) MsoCallWindowProc(WNDPROC pPrevWndFunc, HWND hwnd, UINT msg, 
		WPARAM wParam, LPARAM lParam);

MSOAPIX_(LONG) MsoGetWindowLong(HWND hwnd, int nIndex);
MSOAPI_(LONG) MsoSetWindowLong(HWND hwnd, int nIndex, LONG dwNewLong);

#ifdef _WIN64
MSOAPIX_(LONG_PTR) MsoGetWindowLongPtr(HWND hwnd, int nIndex);
MSOAPIX_(LONG_PTR) MsoSetWindowLongPtr(HWND hwnd, int nIndex, LONG_PTR dwNewLong);
#else
#define MsoGetWindowLongPtr(hwnd, nIndex) MsoGetWindowLong(hwnd, nIndex)
#define MsoSetWindowLongPtr(hwnd, nIndex, dwNewLong) MsoSetWindowLong(hwnd, nIndex, dwNewLong)
#endif // _WIN64

#define ETO_MSO_IME_UL_WORKAROUND 0x0800000
#define ETO_MSO_NO_GLYPH 0x1000000
#define ETO_MSO_DISPLAY_HOTKEY 0x2000000
#define ETO_MSO_NO_FONTLINK	0x20000000
#define	ETO_MSO_DONT_CALL_UCSCRIBE	0x40000000
#define ETO_MSO_FORCE_ENHMETAFILE 0x80000000
MSOAPI_(int) MsoGetWindowTextWtz(HWND hwnd, WCHAR *wtz, int cchMax);
MSOAPIX_(BOOL) MsoSetWindowTextWtz(HWND hwnd, WCHAR *wtz);
MSOAPI_(BOOL) MsoAppendMenuW(HMENU hMenu, UINT uFlags, UINT uIDNewItem,
								LPCWSTR lpNewItem);
MSOAPI_(BOOL) MsoInsertMenuW(HMENU hMenu, UINT uPosition, UINT uFlags,
								UINT uIDNewItem, LPCWSTR lpNewItem);

/*	Return the facename of the system UI (dialog) font in wtzFaceName. */
MSOAPI_(VOID) MsoGetSystemUIFont(WCHAR *wtzFaceName);

/*	Return TRUE if the user settings indicate that we should not use Tahoma
	and instead use the system UI font.  If we should use the system font 
	and plf is non-NULL and points at a font with facename currently equal 
	to "Tahoma", then overwrite it with the the appropriate system UI 
	font (e.g. "MS Sans Serif" or "MS Dialog") with the same attributes. */
MSOAPI_(BOOL) MsoFOverrideOfficeUIFont(LOGFONT *plf);

/*	Return TRUE if the user settings indicate that we should not use Tahoma
	and instead use the system UI font.  If we should use the system font 
	and the Windows dialog at hwndDlg is using the Tahoma font, then set 
	the font for all controls in the dialog to the default system dialog font.  
	Call this in WM_INITDIALOG for Windows dialogs.
		If this function creates a new font, it returns that font in *phfont.
	In this case, the caller is responsible for deleting the font when the
	dialog is closed. */
MSOAPI_(BOOL) MsoFOverrideOfficeUIWinDlgFont(HWND hwndDlg, HFONT *phfont);

/* If the LOGFONT at plf is a default system UI font then change *plf 
	to substitute the Tahoma font as appropriate for the system.
	Return TRUE if *plf was changed. */
MSOAPI_(BOOL) MsoFSubstituteTahomaLogfont(LOGFONT *plf);

/*	Some Far East languages need a minimum 9 point UI font size because 
	otherwise the glyphs are unreadable.  On other languages, we want to go 
	no lower than 8 pt (even if buggy system settings return a smaller value).
	This is controlled by a resource.  If the the font at plf describes a 
	smaller size, increase it to the minimum. */
MSOAPI_(void) MsoEnsureMinUIFontSize(LOGFONT *plf);

// Fonts supported by MsoFGetFontSettings
enum
	{
	msofntMenu,
	msofntTooltip,
	};

/* Return font and color info for the font given by 'fnt' (see msofntXXX).
	If fVertical, then the font is rotated 90 degrees if this fnt type
	supports rotation in Office.  If phfont is non-NULL, return the HFONT used 
	for this item.  This font is owned and cached by Office and should not 
	be deleted.  If phbrBk is non-NULL, return a brush used for the 
	background of this item	(owned by Office and should not be deleted).  
	If pcrText is non-NULL,	return the COLOREF used for the text color for 
	this item. Return TRUE if all requested info was returned. */
MSOAPI_(BOOL) MsoFGetFontSettings(int fnt, BOOL fVertical, HFONT *phfont, 
		HBRUSH *phbrBk, COLORREF *pcrText);

/* If the system suppports NotifyWinEvent, then call it with the given
	parameters (see \otools\inc\win\winable.h). */
MSOAPI_(void) MsoNotifyWinEvent(DWORD dwEvent, HWND hwnd, LONG idObject, LONG idChild);

/* Return FALSE iff we don't need to call MsoNotifyWinEvent.  This is only an
	optimization to avoid prep work in the caller since calling MsoNotifyWinEvent
	is always safe and fast if nobody is listening. */
MSOAPIX_(BOOL) MsoFNotifyWinEvents();

/* Return TRUE if an Accessibility screen reader is running. */
MSOAPI_(BOOL) MsoFScreenReaderPresent();

/* Call LResultFromObject in oleacc.dll to thunk an IUnknown object into
   an LRESULT to allow for cross-process access.  The wParam is the parameter
   as passed to WM_GETOBJECT. */
MSOAPI_(LRESULT) MsoLThunkIUnknown(IUnknown *punk, WPARAM wParam);

/*	Return TRUE if build version of OleAcc.Dll is greater than or equal to
	the version number passed in (in the form A.B.C.D). */
MSOAPIX_(BOOL) MsoFOleAccDllVersion(short A, short B, short C, short D);

/* Put up an alert that says that a help ghosting or shortcut could not 
	be performed because the app is in a bad state. */
MSOAPI_(void) MsoDoGhostingAlert();

/*	Constructs the name of the international dll from the locale passed in.	*/
#define MsoGetIntlName(wz) \
	MsoQfidToFilename(msoqfidMsoIntlDll, wz)

/****************************************************************************
   MsoNotifyIMEWindowChange

	This function should be called whenever the set of visible IME windows
	changes.	Usually, calling this function in response to IMN_OPENCANDIDATE
	and IMN_CHANGECANDIDATE messages is sufficient. Unfortunately, Office is
	unable to catch these messages itself because they go to the window that
	has focus, not to the top-level window of the application.
		By calling this function, the application allows Office to do useful
	things like move the assistant off of the IME windows.
****************************************************************************/
MSOAPI_(void) MsoNotifyIMEWindowChange(void);

/*-----------------------------------------------------------------------------
	MsoWzAppendVer

	Places the current build version into wz of the form (maj.min.rup)
	String is 0 terminated, returns a pointer to the null for subsequent
	append operations.  The buffer is assumed to be long enough for this
	concatination.
-------------------------------------------------------------------- JEFFJO -*/
MSOAPI_(WCHAR *) MsoWzAppendVer(WCHAR *wz);

/****************************************************************************
	MsoFInitDisableUI

	This function must be called to activate the ADMIN disabled UI. It must
	called AFTER calling MsoFCreateStdComponentManager.

	This function will parse the registry for ADMIN disabled keystrokes and
	tcid's. It will register a master component to intercept the offending 
	keystrokes and banish them to the bit bucket.
****************************************************************************/
MSOAPI_(BOOL) MsoFInitDisableUI(HMSOINST hinst);

/****************************************************************************
	MsoHtmlHelp

	This function is the replacement for WinHelp. It will display the HTMLHelp
	Appbar.
****************************************************************************/
MSOAPI_(BOOL) MsoHtmlHelp(HWND hwndMain, LPWSTR lpszHelp, UINT usCommand, DWORD dwData);

/****************************************************************************
	MsoHelpSetLeftPane

	Normally MsoHtmlHelp will cause msohelp to launch with the tabs
	extended or not depending on the current assistant state.  This function
	affects the next call to MsoHtmlHelp such that the tabs can be forced to
	be extended or not regardless of the assistant state.  
****************************************************************************/
MSOAPI_(void) MsoHelpSetLeftPane(BOOL fShow);

/*-------------------------------------------------------------------------*/

#define HELP_VBA_COMMAND   0x10000

typedef enum
{
	msoargtSwitch,
	msoargtFile,
	msoargtString,
	msoargtProfile,
	msoargtAutomation,
	msoargtRegserver,
	msoargtUnregserver,
	msoargtSwitchData,
	msoargtEmbedding,
	msoargtSafe,
	msoargtDDE,
} ARGT;

typedef struct
{
	ARGT argt;
	int ch;  // NOTE: don't change this to a CHAR since ARGC is overloaded to handle WCHAR cmdline
	union
		{
		CHAR *szData;
		int fFound;
		WCHAR *wzData;
		};
} ARGC;

MSOAPI_(int) MsoParseCommandLine(ARGC *pargc, unsigned int carg, CHAR **pszCmdLine, int fDestructive);
MSOAPI_(int) MsoParseCommandLineW(ARGC *pargc, unsigned int carg, WCHAR **pszCmdLine, int fDestructive);

/****************************************************************************
	MsoGetIntlSysSettings

	This API is important to non-US apps, namely MidEast and FarEast.  It takes
	a BOOL param, fRefresh, which will be FALSE most of the time--which is at the 
	init time.  Apps will only set it to TRUE when they think that System settings 
	have been changed.

	BIDI_TODO: It would be a good idea to remove the fRefresh param, and refresh it 
	automatically when WM_SETTINGCHANGE is sent.  Where should we trap it?
****************************************************************************/
MSOAPI_(DWORD) MsoGetIntlSysSettings(BOOL fRefresh);
MSOAPI_(BOOL) MsoFEditLangSupported(WORD lid);

#define MSOI_ARABIC_SYSTEM_INSTALLED		0x00000001	// Arabic APIs
#define MSOI_ARABIC_FONTS_INSTALLED			0x00000002	// Arabic Fonts
#define MSOI_ARABIC_KBD_INSTALLED			0x00000004	// Arabic Keyboards



//SOUTHASIA
// These uses the higher nibble of the second byte to define for SOUTHASIA.
#define MSOI_HINDI_FULLY_INSTALLED			0x00001000
#define MSOI_THAI_FULLY_INSTALLED			0x00002000
#define MSOI_VIETNAMESE_FULLY_INSTALLED		0x00004000
#define MSOI_MSO9SA_RUNNING                 0x00008000 // use to mark this MSO version as SA enabled
//SOUTHASIA

#define MSOI_ARABIC_FULLY_INSTALLED			(MSOI_ARABIC_SYSTEM_INSTALLED \
											| MSOI_ARABIC_FONTS_INSTALLED \
											| MSOI_ARABIC_KBD_INSTALLED)
											// Full Arabic system (0x00000007)

#define MSOI_HEBREW_SYSTEM_INSTALLED		0x00000010	// Hebrew APIs
#define MSOI_HEBREW_FONTS_INSTALLED			0x00000020	// Hebrew Fonts
#define MSOI_HEBREW_KBD_INSTALLED			0x00000040	// Hebrew Keyboard

#define MSOI_HEBREW_FULLY_INSTALLED			(MSOI_HEBREW_SYSTEM_INSTALLED \
											| MSOI_HEBREW_FONTS_INSTALLED \
											| MSOI_HEBREW_KBD_INSTALLED)
											// Full Hebrew system (0x00000070)

#define MSOI_NT4_RUNNING					0x00000100	// Either Windows NT 4
#define MSOI_NT5_RUNNING					0x00000200	// Or Windows NT 5
#define MSOI_WIN95_RUNNING					0x00000400	// Or Windows 95
#define MSOI_WIN98_RUNNING					0x00000800	// Or Windows 98

// Does it support Unicode?
MSOAPI_(BOOL) MsoFUnicodeCommCtrl();





/*-----------------------------------------------------------------------
 IV Declarations
 Last Modifiied July 9, 1998
-------------------------------------------------------------- t-benyu -*/

/*- MsoIVLogCheck --------------------------------------------

  An API to determine if the current build of office is
  an IV version.

  Input : none
  Return: DWORD, 1  - Yes we are IV
                 0 - Not the IV.
  Memory: no modification
-------------------------------------------------- t-benyu -*/
MSOAPI_(DWORD) MsoIVLogCheck();

/*- MsoIVLogInit ---------------------------------------------

  MSO API to register the test wizard callback function.
 
  Input : pointer to function of type PFCNIVLoggingCallback
  Return: NONE
  Memory: Modifies the 'vgpfnIVLogCallback' variable to the
          value of 'vgpfnIVnewLogCallback' of type
          PFCNIVLoggingCallback 
-------------------------------------------------- t-benyu -*/
MSOAPIX_(void) MsoIVLogInit(PFCNIVLoggingCallback);

/* extern declaration for Callback function pointer. 
  Actually declared in office.cpp*/
extern PFCNIVLoggingCallback vgpfnIVLogCallback;

/*-----------------------------------------------------------------------
 END IV Declarations
-------------------------------------------------------------- t-benyu -*/


/*-----------------------------------------------------------------------------
	MsoFNetFile

	Determines if a file name is on the network.

	NOTE:
	Local UNC names (like \\<LocalMachine>\<Share>\<File>)
	are treated as network names.
-------------------------------------------------------------------- ArthurZ -*/

MSOAPIX_(BOOL) MsoFNetFile(const WCHAR *wz);



/*-----------------------------------------------------------------------------
	MsoFNetModules

	Determines if there are any EXEs/DLLs loaded across the network.
-------------------------------------------------------------------- ArthurZ -*/
MSOAPI_(BOOL) MsoFNetModules(BOOL fDisplayMessage, BOOL fNetDocsOpen);

#define fvokNil     0
#define fvokNoIOD   1
MSOAPI_(int) MsoFVbaOK(int *pfCopyDoc, int fvok);
MSOAPI_(void) MsoRegisterVbe(void);

#endif // MSOUSER_H
