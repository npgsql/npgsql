#pragma once

/*************************************************************************
 	msodebug.h

 	Owner: rickp
 	Copyright (c) 1994 Microsoft Corporation

	Standard debugging definitions for the shared Office libraries.
	Includes asserts, tracing, and other cool stuff like that.
*************************************************************************/

#if !defined(MSODEBUG_H)
#define MSODEBUG_H

#if !defined(MSOSTD_H)
#include <msostd.h>
#endif

#if defined(__cplusplus)
extern "C" {
#endif

/*************************************************************************
	Random useful macros
*************************************************************************/

/* Exception to VSMSODEBUG */
#if DEBUG
	#define Debug(e) e
	#define DebugOnly(e) e
	#define DebugElse(s, t)	s
#else
	#define Debug(e)
	#define DebugOnly(e)
	#define DebugElse(s, t) t
#endif


//  REVIEW:  PETERO:  This should be DEBUG only
/****************************************************************************
   This enum contains the Debug "Messages" that are sent to the FDebugMessage
   Method
 ****************************************************************** JIMMUR **/
enum
{
	msodmWriteBe = 1, /* write out the BE record for this object */

	/* Here begin drawing only debug messages */
	msodmDgvRcvOfHsp = 2001,
		/* Ask a DGV for the bounding rectangle (if any) of an HSP.
			Assumes lParam is really a pointer to an MSODGDB, looks at the
			hsp field thereof and fills out the rcv field. */
	msodmDgsWriteBePvAnchor,
		/* Write out the BE record for a host allocated pvAnchor. */
	msodmDgsWriteBePvClient,
		/* Write out the BE record for any host allocated client data. */
	msodmDgvsAfterMouseInsert,
		/* Passed to IMsoDrawingViewSite after a shape is interactively
			inserted with the mouse. lParam is really the inserted HSP. */
	msodmDgvsAfterMarquee,
		/* Passed to IMsoDrawingViewSite after one drags out a rectangle
			with the pointer tool selecting zero or more shapes. */
	msodmIsNotMso96,
		/* Returns FALSE if the specified object is implemented by MSO96.DLL.
			Allows sleazy up-casts, for example, from IMsoDrawingView *
			to DGV *. */
	msodmGetHdesShape,
		/* Ask a DGVs for its m_hdesShape (in *(MSOHDES *)lParam).  Returns
			FALSE if it filled out an HDES. */
	msodmGetHdesSelection,
		/* Ask a DGVs for its m_hdesSelection (in *(MSOHDES *)lParam).
			Returns FALSE if it filled out an HDES. */
	msodmDguiWriteBeForDgc,
		/* Ask a DGUI to write BEs for a DGC it allocated. */
	msodmDgsWriteBeTxid,
		/* Write out the BE record for the attached text of a shape. */
	msodmDgsWriteBePvAnchorUndo,
		/* Write out the BE record for a host anchor in the undo stack. */
	msodmDgvsDragDrop,
		/* Let the host know that I just did a drag-drop from this window. */
};

enum
{
   msodmbtDoNotWriteObj = 0,    // Do Not write out the object 
   msodmbtWriteObj,             // Do write out the object and
                                    // embedded pointers
};


enum
{
	msocchBt = 20,						// Maximum size of a Bt description String
};

/* Some debug messages need more arguments than fit through
	the arguments to FDebugMethod.  For these there are various
	MSODMBfoo structs, usually defined near the objects they're passed
	to. */


/****************************************************************************
    Interface debug routine
 ****************************************************************** JIMMUR **/
#if VSMSODEBUG
   #define MSODEBUGMETHOD  MSOMETHOD_(BOOL, FDebugMessage) (THIS_ HMSOINST hinst, \
         UINT message, WPARAM wParam, LPARAM lParam) PURE;

   #define MSODEBUGMETHODIMP MSOMETHODIMP_(BOOL) FDebugMessage (HMSOINST hinst, \
         UINT message, WPARAM wParam, LPARAM lParam); \
         static BOOL FCheckObject(LPVOID pv, int cb);
			
	#define MSOMACDEBUGMETHODIMP MSOMACPUB	 MSOMETHODIMP_(BOOL) FDebugMessage (HMSOINST hinst, \
         UINT message, WPARAM wParam, LPARAM lParam); \
         static BOOL FCheckObject(LPVOID pv, int cb);

   #define DEBUGMETHOD(cn,bt) STDMETHODIMP_(BOOL) cn::FDebugMessage \
         (HMSOINST hinst, UINT message, WPARAM wParam, LPARAM lParam) \
         { \
            if (msodmWriteBE == message) \
               {  \
                  return MsoFSaveBe(hinst,lParam,(void*)this,sizeof(cn),bt); \
               } \
            return FALSE; \
         }
 #else
   #define MSODEBUGMETHOD  MSOMETHOD_(BOOL, FDebugMessage) (THIS_ HMSOINST hinst, \
         UINT message, WPARAM wParam, LPARAM lParam) PURE;

   #define MSODEBUGMETHODIMP MSOMETHODIMP_(BOOL) FDebugMessage (HMSOINST hinst, \
         UINT message, WPARAM wParam, LPARAM lParam);
			
   #define MSOMACDEBUGMETHODIMP MSOMACPUB MSOMETHODIMP_(BOOL) FDebugMessage (HMSOINST hinst, \
         UINT message, WPARAM wParam, LPARAM lParam);
	
   #define DEBUGMETHOD(cn,bt)  STDMETHODIMP_(BOOL) cn::FDebugMessage (HMSOINST, \
         UINT, WPARAM, LPARAM) { return TRUE; }
#endif



/*************************************************************************
	Enabling/disabling debug options
*************************************************************************/

enum
{
	msodcAsserts = 0,	/* asserts enabled */
	msodcPushAsserts = 1, /* push asserts enabled */
	msodcMemoryFill = 2,	/* memory fills enabled */
	msodcMemoryFillCheck = 3,	/* check memory fills */
	msodcTrace = 4,	/* trace output */
	msodcHeap = 5,	/* heap checking */
	msodcMemLeakCheck = 6,
	msodcMemTrace = 7,	/* memory allocation trace */
	msodcGdiNoBatch = 8,	/* don't batch GDI calls */
	msodcShakeMem = 9,	/* shake memory on allocations */
	msodcReports = 10,	/* report output enabled */
	msodcMsgTrace = 11,	/* WLM message trace - MAC only */
	msodcWlmValidate = 12,	/* WLM parameter validation - MAC only */
	msodcGdiNoExcep = 13,  /* Don't call GetObjectType for debug */
	msodcDisplaySlowTests = 14, /* Do slow (O(n^2) and worse) Drawing debug checks */
	msodcDisplayAbortOften = 15, /* Check for aborting redraw really often. */
	msodcDisplayAbortNever = 16, /* Don't abort redraw */
	msodcPurgedMaxSmall = 17,
	msodcMarkMemLeakCheck = 18,
	msodcSpare19 = 19, /* USE ME */
	msodcSpare20 = 20, /* USE ME */
	msodcSpare21 = 21, /* USE ME */
	msodcSpare22 = 22, /* USE ME */
	msodcMax = 23,
};


/* Enables/disables various office debug checks. dc is the check to 
	change, fEnabled is TRUE if the check should be enabled, FALSE
	if disabled */
#if VSMSODEBUG
	MSOAPI_(BOOL) MsoEnableDebugCheck(int dc, BOOL fEnabled);
	#if !OFFICE_BUILD
		MSOAPI_(BOOL) MsoFGetDebugCheck(int dc);
	#else
		extern BYTE msovmpdcfDisabled[msodcMax];
		#define MsoFGetDebugCheck(dc) (!msovmpdcfDisabled[(dc)])
	#endif
#else
	#define MsoEnableDebugCheck(dc, fEnabled) (FALSE)
	#define MsoFGetDebugCheck(dc) (FALSE)
#endif


/* Enables/disables various office feature reports. bitfFeatureFilter
	is the feature reports to change, fEnable should be TRUE if the
	feature reports should be enabled, FALSE if disabled. */
#if VSMSODEBUG
	MSOAPI_(UINT) MsoEnableFeatureReports(UINT bitfFeatureFilter, BOOL fEnable);
	MSOAPI_(UINT) MsoSetFeatureReports(UINT grfFeatureFilter);
	#if !OFFICE_BUILD
		MSOAPI_(BOOL) MsoFFeatureReportsEnabled(UINT bitfFeatureFilter);
		MSOAPI_(UINT) MsoGetFeatureReports();
	#else
		extern UINT vuFeatureReportsFilter;
		#define MsoFFeatureReportsEnabled(bitfFeatureFilter) \
					(vuFeatureReportsFilter & bitfFeatureFilter)
		#define MsoGetFeatureReports() (vuFeatureReportsFilter)
	#endif
#else
	#define MsoEnableFeatureReports(bitfFeatureFilter, fEnable) (0)
	#define MsoFFeatureReportsEnabled(bitfFeatureFilter) (FALSE)
	#define MsoSetFeatureReports(grfFeatureFilter) (0)
	#define MsoGetFeatureReports() (0)
#endif


/*	Assert Output types */
enum
{
	msoiasoAssert,
	msoiasoTrace,
	msoiasoReport,
	msoiasoMax
};


/*	Returns the current debug output settings.  Note that these are
	macros referencing a DLL global variable. */
//#define MsoFAssertsEnabled() (MsoFGetDebugCheck(msodcAsserts))
MSOAPI_(BOOL) MsoFAssertsEnabled();
#define MsoFTraceEnabled() (MsoFGetDebugCheck(msodcTrace))
#define MsoFReportsEnabled() (MsoFGetDebugCheck(msodcReports))

enum
{
	msoaoDebugger = 0x01,	/* output to debugger */
	msoaoFile = 0x02,	/* output goes to file */
	msoaoMsgBox = 0x04,	/* output displayed in message box (no Traces) */
	msoaoPort = 0x08,	/* output sent to serial port */
	msoaoMappedFile = 0x10,	/* output recorded in memory mapped file */
	msoaoDebugBreak = 0x20,	/* msoaoDebugger breaks into the debugger */

	msoaoAppend = 0x8000,	/* output appended to existing file */
};

/*	Sets the destination of assert output */
#if VSMSODEBUG
	MSOAPI_(int) MsoSetAssertOutput(int iaso, int ao);
#else
	#define MsoSetAssertOutput(iaso, ao) (0)
#endif

/*	Returns the current assert output destination. */
#if VSMSODEBUG
	MSOAPI_(int) MsoGetAssertOutput(int iaso);
#else
	#define MsoGetAssertOutput(iaso) (0)
#endif

/* Sets the name of the file that the assert information gets written
	to, if file output is enabled using msoaoFile */
#if VSMSODEBUG
	MSOAPI_(void) MsoSetAssertOutputFile(int iaso, const CHAR* szFile);
#else
	#define MsoSetAssertOutputFile(iaso, szFile) (0)
#endif

/*	Returns the current name of the file that we're writing assert
	output to.   The name is saved in the buffer szFile, which must be
	cchMax characters long.  Returns the actual  length of the string
	returned. */
#if VSMSODEBUG
	MSOAPIXX_(int) MsoGetAssertOutputFile(int iaso, CHAR* szFile, int cchMax);
#else
	#define MsoGetAssertOutputFile(iaso, szFile, cchMax) (0)
#endif


/*************************************************************************
	Debugger breaks
*************************************************************************/

/* REVIEW KirkG: Should these be defined in Ship versions? */

/* REVIEW KirkG: Why do we need both Inline and non-Inline? */

/* Breaks into the debugger.  Works (more or less) on all supported
 	systems. */
#if X86
	#define MsoDebugBreakInline() {__asm int 3}
#else
	#define MsoDebugBreakInline() { MsoDebugBreak(); }
#endif

/*	A version of debug break that you can actually call, instead of the
	above inline weirdness we use in most cases.  Can therefore be used in
	expressions. Returns 0 */
#if VSMSODEBUG
	MSOAPI_(int) MsoDebugBreak(void);
#else
	#define MsoDebugBreak() (0)
#endif


/*************************************************************************
	Assertion failures
*************************************************************************/

#if !defined(MSO_NO_ASSERTS)

/*	Displays the assert message, including flushing any assert stack.
	szFile and li are the filename and line number of the failure,
	and szMsg is an optional message to display with the assert.
	Returns FALSE if the caller should break into the debugger. */
#if VSMSODEBUG
	MSOAPI_(BOOL) MsoFAssert(const CHAR* szFile, int li, const CHAR* szMsg);
#ifndef STATIC_LIB_DEF
	MSOAPI_(BOOL) MsoFAssertTag(DWORD dwTag, const CHAR* szFile, int li, 
	                            const CHAR* szMsg);
#endif // !STATIC_LIB_DEF
#else
	#define MsoFAssert(szFile, li, szMsg) (TRUE)
	#define MsoFAssertTag(dwTag, szFile, li, szMsg) (TRUE)
#endif

/*	Same as MsoFAssert above, except an optional title string can be
	displayed. */
#if VSMSODEBUG
	MSOAPI_(BOOL) MsoFAssertTitle(const CHAR* szTitle,
			const CHAR* szFile, int li, const CHAR* szMsg);
	MSOAPI_(BOOL) MsoFAssertTagTitle(DWORD dwAssertTag, const CHAR* szTitle,
			const CHAR* szFile, int li, const CHAR* szMsg);
#else
	#define MsoFAssertTitle(szTitle, szFile, li, szMsg) (TRUE)
	#define MsoFAssertTagTitle(dwAssertTag, szTitle, szFile, li, szMsg) (TRUE)
#endif

/*	Same as MsoFAssertTitle above, except you can pass in your own 
	MessageBox flags */
#if VSMSODEBUG
	MSOAPIXX_(BOOL) MsoFAssertTitleMb(const CHAR* szTitle,
			const CHAR* szFile, int li, const CHAR* szMsg, UINT mb);
#else
	#define MsoFAssertTitleMb(szTitle, szFile, li, szMsg, mb) (TRUE)
#endif

/*	Set a global for what type of NoTag file we should write. */
#if VSMSODEBUG
	MSOAPI_(VOID) MsoAssertNoTag(int iLog);
#else
	#define MsoAssertNoTag(iLog)
#endif

// This enum has to match the array vrgNoTag in dbassert.cpp.
enum
	{
	iNoTagDefault = 0,
	iNoTagAssert = 0,
	iNoTagHTML = 1,
	iNoTagGEL = 2,
	iNoTagMem = 3
	};
	

/*	Same as MsoFAssertTitleMb above, except you can pass in your own 
	assert output type */
#if VSMSODEBUG
	MSOAPI_(BOOL) MsoFAssertTitleAsoMb(int iaso, const CHAR* szTitle,
			const CHAR* szFile, int li, const CHAR* szMsg, UINT mb);
#else
	#define MsoFAssertTitleAsoMb(iaso, szTitle, szFile, li, szMsg, mb) (TRUE)
#endif

/*	Similiar to MsoFAssertTitle above but is a report instead, also you can
	pass in the number of function frames to go back to disable the report */
#if VSMSODEBUG
	MSOAPI_(BOOL) MsoFReportTitleLevel(const CHAR* szTitle, int wLevel,
			const CHAR* szFile, int li, const CHAR* szMsg);
#else
	#define MsoFReportTitleLevel(szTitle, wLevel, szFile, li, szMsg) (TRUE)
#endif

// REVIEW KirkG: Move this gunk to msoassert.h

/*	The actual guts of the assert.  if the flag f is FALSE, then we kick
	of the assertion failure, displaying the optional message szMsg along
	with the filename and line number of the failure */
#if !VSMSODEBUG
	#define AssertMsgInline(f, szMsg)
	#define AssertMsgTemplate(f, szMsg)
#else
	#define AssertMsgInline(f, szMsg) \
		do { \
		if (!(f) && MsoFAssertsEnabled() && \
				!MsoFAssert(__FILE__, __LINE__, (const CHAR*)(szMsg))) \
			MsoDebugBreakInline(); \
		} while (0)
	// Template inlines don't like the inline __asm for some reason
	#define AssertMsgTemplate(f, szMsg) \
		do { \
		if (!(f) && MsoFAssertsEnabled() && \
				!MsoFAssert(__FILE__, __LINE__, (const CHAR*)(szMsg))) \
			MsoDebugBreak(); \
		} while (0)
#endif


/*	Tells if the Office is currently displaying an alert message box */
#if !VSMSODEBUG
	#define MsoFInAssert() (FALSE)
#else
	MSOAPI_(BOOL) MsoFInAssert(void);
#endif

/*	Random compatability versions of the assert macros */

#if MSO_ASSERT_EXP
	#define AssertInline(f) AssertMsgInline((f), #f)
	#define AssertTemplate(f) AssertMsgTemplate((f), #f)
#else
	#define AssertInline(f) AssertMsgInline((f), NULL)
	#define AssertTemplate(f) AssertMsgTemplate((f), NULL)
#endif


/*************************************************************************
	Untested notifications
*************************************************************************/

#if !VSMSODEBUG
	#define UntestedMsg(szMsg)
#else
	#define UntestedMsg(szMsg) \
		do { \
		if (MsoFAssertsEnabled() && \
				!MsoFAssertTitle("Untested", vszAssertFile, __LINE__, (const CHAR*)(szMsg))) \
			MsoDebugBreakInline(); \
		} while (0)
#endif

#define Untested() UntestedMsg(NULL)


/*************************************************************************
	Unreached notifications
*************************************************************************/

#if !VSMSODEBUG
	#define UnreachedMsg(szMsg)
#else
	#define UnreachedMsg(szMsg) \
		do { \
		if (MsoFAssertsEnabled() && \
				!MsoFAssertTitle("Unreached", vszAssertFile, __LINE__, (const CHAR*)(szMsg))) \
			MsoDebugBreakInline(); \
		} while (0)
#endif

#define Unreached() UnreachedMsg(NULL)


#if !VSMSODEBUG
	#define UnreachedMsgTag(szMsg,dwAssertTag)
#else
	#define UnreachedMsgTag(szMsg,dwAssertTag) \
		do { \
		if (MsoFAssertsEnabled() && \
				!MsoFAssertTagTitle(dwAssertTag, "Unreached", vszAssertFile, __LINE__, (const CHAR*)(szMsg))) \
			MsoDebugBreakInline(); \
		} while (0)
#endif

#define UnreachedTag(dwAssertTag) UnreachedMsgTag(NULL, dwAssertTag)

/*************************************************************************
	PushAsserts
*************************************************************************/

/*	Like Assert, except the message is not immediately displayed.
	Instead, the message is saved on a LIFO stack, which is dumped
	to the screen when an Assert eventually occurs.  This can be
	useful for displaying additional information about the type of
	failure inside a nested validation routine.

 	Yeah, I know, this isn't the best idea, but I had the code, so I
	might as well use it. */

#if !VSMSODEBUG
	#define MsoFPushAssert(szFile, li, szMsg) (0)
	#define PushAssertMsg(f, szMsg) (1)
#else
	MSOAPIXX_(BOOL) MsoFPushAssert(const CHAR* szFile, int li, const CHAR* szMsg);
	#define PushAssertMsg(f, szMsg) \
		((f) || (!MsoFPushAssert(vszAssertFile, __LINE__, szMsg) && MsoDebugBreak()))
#endif

#if MSO_ASSERT_EXP
	#define PushAssert(f) PushAssertMsg((f), #f)
#else
	#define PushAssert(f) PushAssertMsg((f), NULL)
#endif	
#define PushAssertExp(f) PushAssertMsg((f), #f)


/*************************************************************************
	Scratch GDI Objects
*************************************************************************/

/*	Routines to ensure only single access to global scratch GDI objects */

#if !VSMSODEBUG

	#define MsoUseScratchObj(hobj, szObjName)
	#define MsoReleaseScratchObj(hobj, szObjName)
	#define UseScratchDC(hdc)
	#define ReleaseScratchDC(hdc)
	#define UseScratchRgn(hrgn)
	#define ReleaseScratchRgn(hrgn)

#else

	/* mask that contains unused bits in the handle */
	#define msohInUse (0xffffffff)
	// REVIEW: any handle bits we can rely on to make this test more correct?
	#define MsoFObjInUse(hobj) (GetObjectType(hobj) != 0)

	#define MsoUseScratchObj(hobj, szObjName) \
			do { \
			if (MsoFObjInUse(hobj) && MsoFAssertsEnabled() && \
					!MsoFAssert(vszAssertFile, __LINE__, "Scratch " szObjName " " #hobj " already in use")) \
				MsoDebugBreakInline(); \
			*(int*)&(hobj) ^= msohInUse; \
			} while (0)

	#define MsoReleaseScratchObj(hobj, szObjName) \
			do { \
			if (!MsoFObjInUse(hobj) && MsoFAssertsEnabled() && \
					!MsoFAssert(vszAssertFile, __LINE__, "Scratch " szObjName " " #hobj " not in use")) \
				MsoDebugBreakInline(); \
			*(int*)&(hobj) ^= msohInUse; \
			} while (0)

	#define UseScratchDC(hdc) MsoUseScratchObj(hdc, "DC")
	#define ReleaseScratchDC(hdc) MsoReleaseScratchObj(hdc, "DC")
	#define UseScratchRgn(hrgn) MsoUseScratchObj(hrgn, "region")
	#define ReleaseScratchRgn(hrgn) MsoReleaseScratchObj(hrgn, "region")

#endif


/*************************************************************************
	Reports
*************************************************************************/

// REVIEW KirkG: Get rid of these, and use msodbglg.h's MsoReportSz

#if VSMSODEBUG
	MSOAPI_(BOOL) MsoFReport(const CHAR* szFile, int li, const CHAR* szMsg);
	#define ReportMsg(f, szMsg) \
		do { \
		if (MsoFReportsEnabled() && !(f) && \
				!MsoFReport(vszAssertFile, __LINE__, (const CHAR*)(szMsg))) \
			MsoDebugBreakInline(); \
		} while (0)
#else
	#define MsoFReport(szFile, li, szMsg) (TRUE)
	#define ReportMsg(f, szMsg)
#endif


#endif // MSO_NO_ASSERTS

/*************************************************************************
	Inline Assert stubs - these should only happen for people who
	define MSO_NO_ASSERTS and don't define the asserts we need.  If
	it happens -- too bad.  They don't get these kinds of asserts.
*************************************************************************/

#ifndef AssertMsgInline
	#define AssertMsgInline(f, szMsg)
#endif
#ifndef AssertMsgTemplate
	#define AssertMsgTemplate(f, szMsg)
#endif
#ifndef AssertInline
	#define AssertInline(f)
#endif
#ifndef AssertTemplate
	#define AssertTemplate(f)
#endif


/*************************************************************************
	Tracing
*************************************************************************/

/*	Displays the string sz in the debug output location */
#if VSMSODEBUG
	MSOAPI_(void) MsoTraceSz(const CHAR* szMsg, ...);
	MSOAPI_(void) MsoTraceVa(const CHAR* szMsg, va_list va);
#elif __cplusplus
	__inline void __cdecl MsoTraceSz(const CHAR*,...) {}
	#define MsoTraceVa(szMsg, va)
#else
	__inline void __cdecl MsoTraceSz(const CHAR* szMsg,...) {}
	#define MsoTraceVa(szMsg, va)
#endif



/*************************************************************************
	Debug fills
*************************************************************************/

//  REVIEW:  PETERO:  Should be able to #ifdef VSMSODEBUG this enum and still build successfully.
enum
{
	msomfSentinel,	// sentinel fill value - one before and one after memory allocations to test for over/under write
	msomfFree,		// free memory fill value
	msomfNew,       // new memory fill value
	msomfBuffer,    // fill value used to test buffers passed into APIs against expected size
	msomfStack,     // fill value used in chkstk
	msomfClass,		// fill value used to mark classes to test for uninit members
	msomfMax
};

/*	Fills the memory pointed to by pv with the fill value lFill.  The
	area is assumed to be cb bytes in length.  Does nothing in the
	non-debug build */
#if VSMSODEBUG
	MSOAPI_(void) MsoDebugFillValue(void* pv, INT_PTR cb, DWORD lFill);
#else
	#define MsoDebugFillValue(pv, cb, lFill)
#endif

/*	In the debug version, used to fill the area of memory pointed to by
	pv with a the standard fill value specified by mf.  The memory is 
	assumed to be cb bytes long. */
#if VSMSODEBUG
	MSOAPI_(void) MsoDebugFill(void* pv, INT_PTR cb, int mf);
#else
	#define MsoDebugFill(pv, cb, mf)
#endif

/*	Checks the area given by pv and cb are filled with the debug fill
	value lFill. */
#if VSMSODEBUG
	MSOAPI_(BOOL) MsoFCheckDebugFillValue(const void* pv, INT_PTR cb, DWORD lFill);
#else
	#define MsoFCheckDebugFillValue(pv, cb, lFill) (TRUE)
#endif

/*	Checks the area given by pv and cb are filled with the debug fill
	of type mf. */
#if VSMSODEBUG
	MSOAPI_(BOOL) MsoFCheckDebugFill(const void* pv, INT_PTR cb, int mf);
#else
	#define MsoFCheckDebugFill(pv, cb, mf) (TRUE)
#endif

/* Returns the fill value corresponding to the given fill value type mf. */
#if VSMSODEBUG
	MSOAPI_(DWORD) MsoLGetDebugFillValue(int mf);
#else
	#define MsoLGetDebugFillValue(mf) ((DWORD)0)
#endif

/*	Sets the fill value to lFill for the given memory fill type mf.
	Returns the previous fill value. */
#if VSMSODEBUG
	MSOAPI_(DWORD) MsoSetDebugFillValue(int mf, DWORD lFill);
#else
	#define MsoSetDebugFillValue(mf, lFill) ((DWORD)0)
#endif

#define MsoDebugFillLocal(l) MsoDebugFill(&(l), sizeof(l), msomfFree)

/*************************************************************************
	Debug APIs
*************************************************************************/

enum
{
	msodbSaveBe = 1,
	msodbValidate = 2,
};


/*************************************************************************
	Standard debugging UI for controlling Office debug options from
	within the app.
*************************************************************************/

/*	Debug options for the Debug Options dialog box */
typedef struct MSODBO
{
	int aoEnabled;	// assert outputs
	char szOut[128];	// assert output file (for msoaoFile)
	DWORD mpmflFill[msomfMax];	// memory fill values
	UINT uFeatureReportsFilter;	// feature reports enabled
	BOOL mpdcfEnabled[msodcMax];	// debug checks enabled
		/* TODO rickp(peteren): I moved mpdcfEnabled to the end
			so's you could avoid full builds after adding new options.
			That OK? */
	BOOL fShowShapePropertyDlg;
} MSODBO;

enum
{
	msodboGetDefaults = 1,	/* return default debug options */
	msodboShowDialog = 2,	/* show default debug options dialog */
	msodboSetOptions = 3	/* set debug options */
};

/*	Displays the Office standard debug dialog box with owner hwndParent;
	for msdboGetDefaults, returns the current debug settings in pdbo; for
	msdboShowDialog, displays the dialog box using the settings passed
	in pdbo, and returns the new values (if the user hits OK); for 
	msdboSetOptions, sets the current debug settings to values in pdbo.
	Returns FALSE if the user canceled the dialog. */
#if VSMSODEBUG
	MSOAPI_(BOOL) MsoFDebugOptionsDlg(HWND hwndParent, MSODBO* pdbo, int dbo);
#else
	#define MsoFDebugOptionsDlg(hwndParent, pdbo, dbo) (0)
#endif

#if VSMSODEBUG
	MSOAPI_(BOOL) MsoFDebugOptionsDlgEx(HWND hwndParent, 
		interface IMsoDrawingUserInterface* pidgui, MSODBO* pdbo, int dbo);
#else
	#define MsoFDebugOptionsDlgEx(hwndParent, pidgui, pdbo, dbo) (0)
#endif


/*	Puts up the debug dialog box that displays all the cool and
	interesting version information for all the modules linked into
	the running application.  The owning window is hwndParent, and
	additional DLL instances can be displayed by passing an array
	of instance handles in rghinst, with size chinst. */
#if VSMSODEBUG
	MSOAPI_(void) MsoModuleVersionDlg(HWND hwndParent, const HINSTANCE* rghinst,
			int chinst);
#else
	#define MsoModuleVersionDlg(hwndParent, rghinst, chinst)
#endif


/*************************************************************************
	Debug Monitors
*************************************************************************/

/*	Monitor notifcations */

enum
{
	msonmAlloc=0x1000,	// memory allocation
	msonmFree,	// memory freed
	msonmRealloc,	// memory reallocation
	msonmStartTrace,	// start trace
	msonmEndTrace,	// end trace
	msonmSbMic,
	msonmSbMac,
	msonmSbCbFree,
	msonmSbCbTotal,
};

#if VSMSODEBUG

	MSOAPIXX_(LRESULT) MsoNotifyMonitor(int nm, ...);
	MSOAPIXX_(HWND) MsoGetMonitor(void);
	MSOAPIXX_(BOOL) MsoFAddMonitor(HWND hwnd);
	MSOAPIXX_(BOOL) MsoFRemoveMonitor(HWND hwnd);
	MSOAPIXX_(BOOL) MsoFMonitorProcess(HWND hwnd);
	MSOAPIXX_(HWND) MsoGetMonitoredProcess(void);
	MSOAPIXX_(LRESULT) MsoAskMonitoredProcess(int nm, LPARAM lParam);

#elif __cplusplus
	__inline void __cdecl MsoNotifyMonitor(int,...) {}
#else
	__inline void __cdecl MsoNotifyMonitor(int nm,...) {}
#endif

/*************************************************************************
	Debug Menu Preference List
*************************************************************************/
#if VSMSODEBUG
	// To add a boolean to the office debug menu, add an identifier to
	// this enumeration.  Please use the msodbgf<team initials><...>
	// format for future extensibility
	enum {
		msodbgfTCOTrace,
		msodbgfTCODisableResiliency,
		msodbgfTCODisableInstalls,
		msodbgfTCOAssertResiliency,
		msodbgfTCOAssertInstalls,
		msodbgfTCOAssertDarwinErrors,
		msodbgfTCOAssertAuthErrors,
		msodbgfTCOEnablePolicy,
		msodbgfTCOFakeQualifiedComponents, 
		msodbgfTCOFakeDemandInstall, 
		msodbgfTCOOrapiTraces,
		msodbgfTCOOrapiTraceFunctions,
		msodbgfTCOOrapiAlwaysWriteValues,
		msodbgfTCOIEVersion,
		msodbgfUIAppendMSAADumps,
		msodbgfUIInfoViaHelp,
		msodbgfWCTestNestedTags,
		msodbgfPRGAlwaysShowProgress,
 		msodbgfOATransferGif,
 		msodbgfOATransferJfif,
 		msodbgfOATransferPng,
		msodbgfUISOWWizTest,
		msodbgfUISOWWizLIS,
		msodbgfKJReallocNew,
		msodbgfFUNLogFile,
		msodbgfKJIdleLog,
		// sentinel, place boolean declarations above this line
		msodbgfMax,
	};

	MSOAPI_(BOOL) MsoFDebugPref( unsigned int idbgf );
	MSOAPI_(BOOL) MsoFSetDebugPref( unsigned int idgbf, BOOL fActive );
#endif

#if defined(__cplusplus)
}
#endif

/******************************************************************************
	OLE related debug utlities
******************************************************************************/	
#if VSMSODEBUG
/*-----------------------------------------------------------------------------
	MsoDebugPeekStg	

	One level enumeration of any IStorage object
-------------------------------------------------------------------- HAILIU -*/
MSOAPI_(VOID) MsoDebugPeekStg(IStorage *pistg);

/*-----------------------------------------------------------------------------
	MsoDebugDumpStg	

	Dump any IStorage to c:\$stgdump.stg
-------------------------------------------------------------------- HAILIU -*/
MSOAPI_(VOID) MsoDebugDumpStg(IStorage *pistg);

/*-----------------------------------------------------------------------------
	MsoDebugPeekDataObject

	enumerate the supported formatetc of the IDataObject object
-------------------------------------------------------------------- HAILIU -*/
MSOAPI_(VOID) MsoDebugPeekDataObject(IDataObject *pidobj);

/*-----------------------------------------------------------------------------
	MsoFDebugUseViewObject

	return TURE if the object should use IViewObject interface for rendering
	if the object support a IDataObject which in turn support at least
	CF_METAFILEPICT, you should not be using IViewObject
-------------------------------------------------------------------- HAILIU -*/
MSOAPI_(BOOL) MsoFDebugUseViewObject(LPUNKNOWN punk);

/*-----------------------------------------------------------------------------
	MsoDebugPeekStm
-------------------------------------------------------------------- HAILIU -*/
MSOAPI_(VOID) MsoDebugPeekStm(IStream *pistm);

/*-----------------------------------------------------------------------------
	MsoDebugPeekDC

	Give you a chance to look at the various aspect the hdc
-------------------------------------------------------------------- HAILIU -*/
MSOAPI_(VOID) MsoDebugPeekDC(HDC hdc);

/*-----------------------------------------------------------------------------
	MsoDebugPeekViewObjectEmf
-------------------------------------------------------------------- HAILIU -*/
MSOAPI_(VOID) MsoDebugPeekViewObjectEmf(IViewObject *pivobj);

#else
#define MsoDebugPeekStg(pistg)
#define MsoDebugDumpStg(pistg)
#define MsoDebugPeekDataObject(pidobj)
#define MsoFDebugUseViewObject(punk)  (TRUE)
#define MsoDebugPeekStm(pistm)
#define MsoDebugPeekDC(hdc)
#define MsoDebugPeekViewObjectEmf(pivobj)
#endif //VSMSODEBUG


/****************************************************************************
	MsoDebugOptions VBA object related routine.
**************************************************************** SureshT ***/

/*-----------------------------------------------------------------------------
	MsoHrGetMsoDebugOptions

	Mso API to get the MsoDebugOptions VBA object.

------------------------------------------------------------------- SureshT -*/
MSOAPI_(HRESULT) MsoHrGetMsoDebugOptions(struct MSOINST *hmsoinst, IDispatch **ppidisp);


/*****************************************************************************
	Return address structure
*****************************************************************************/
typedef struct _MSORADDR
{
	void* pfnCaller;
//  REVIEW:  PETERO:  Consider removing PPCMAC
#if PPCMAC
	void* pfnJumpEntry;
#endif
} MSORADDR;

/*-----------------------------------------------------------------------------
	MsoHrGetMsoDebugOptions

	Skips cSkip levels then stores up to maxraddr return 
	addresses from the call stack.
	pcraddr is set to the  number of valid elements in rgraddr
------------------------------------------------------------------- fviton -*/
MSOAPI_(void) MsoDbgGetCallStack(MSORADDR* rgraddr, int cSkip, int maxraddr, int* pcraddr);


/*---------------------------------------------------------------------------
	MsoFCopyToClipboardSz

	Copies sz to the clipboard.  hwnd is the owner window for the string and
	may be NULL.

	Returns true if the copy was successful.
---------------------------------------------------------------- PaulCole -*/
#if VSMSODEBUG
MSOAPI_(BOOL) MsoFCopyToClipboardSz(char * sz, HWND hwnd);
#endif



#endif // MSODEBUG_H
