#pragma once

/*------------------------------------------------------------------------*
 * msosdm.h (previously known as sdm.h): SDM Main PUBLIC include file.    *
 *                                                                        *
 * Please do not modify or check in this file without contacting MsoSdmQs.*
 *------------------------------------------------------------------------*/


//----------------------------------------------------------------------//
// #IFDEF       Use when...
// ------------ ----------------------------------------
// WIN32        Always true	for WIN
// DEBUG
// TSMTE        FE version
//
//----------------------------------------------------------------------//

#ifndef	SDM_INCLUDED	// Entire file.
#define	SDM_INCLUDED

#include <msostd.h>
#include <msouser.h>
#include <msoswct.h>
#define SDMPUBLIC MSOAPICALLTYPE

//============================
// who's not defined?
#ifdef SDM_TYPES_DEFINED
#error
#endif
#ifdef DBCS
// REVIEW KirkG: DBCS is not defined for Mso.  It is defined for Word
//  (in word.h), but I don't think anything in Word's build uses it.
// What does this mean for its use in the TMW struct below?  Does the 
//   client indeed allocate those things?
//#error
#endif
// who's already defined?
#ifndef SDM_WCT_DEFINED
#error
#endif
//============================

///////////////////////////////////////////////////////////////////////////////
// Standard types and values.						     

#ifndef CSTD_H
typedef	char * LSZ;
typedef char * SZ;
#endif
typedef WCHAR * LWZ;
typedef WCHAR *	WZ;

typedef const char * CONST_SZ;
typedef const WCHAR * CONST_WZ;

#ifndef	SDM_TYPES_DEFINED
#define	SDM_TYPES_DEFINED
typedef	unsigned char	U8_SDM;
typedef	char		S8_SDM;
typedef	unsigned short	U16_SDM;
typedef	short		S16_SDM;
typedef	unsigned short	BIT_SDM;	// short ==> better packing in Win32


// AKadatch: all these types hold pointers
// typedef	long		S32_SDM;
// typedef	unsigned long	U32_SDM;
// typedef int INT_SDM;
// typedef unsigned int UINT_SDM;
// typedef	unsigned long	BARG_SDM;


// AKadatch: some apps use obsolete and/or modified WinNT.h
// *_PTR types should be declared manually
#include <basetsd.h>

typedef	LONG_PTR	S32_SDM;
typedef	ULONG_PTR	U32_SDM;
typedef INT_PTR		INT_SDM;
typedef UINT_PTR	UINT_SDM;
typedef	ULONG_PTR	BARG_SDM;

//FEH: typedef short INT_SDM;
//FEH: typedef unsigned short UINT_SDM;
typedef UINT_SDM UCAB_SDM;	//cab arg
typedef UINT_SDM UCBK_SDM;	//callback parameter/return

// FUTURE Should we change Win to also define ILBE_SDM as INT_SDM?
//
// The Mac needsILBE_SDM to signed, since ilbeFirst can be < 0 in
// droplists, since there can be blank list items above the first
// real list item.  However changing ILBE_SDM to INT_SDM on the
// Windows side caused problems in WinWord16, since ilbeNil was
// being sign-extended in longs in some structures.  I tried to 
// change ilbeNil and uNinchList to 0x7fff but ran into many places 
// which hard-coded returning -1 instead of ilbeNil or that counted 
// on ilbeNil+1==0
typedef UINT_SDM ILBE_SDM;  // first entry in listbox

typedef int BOOL_SDM;	//BOOL

typedef int XY_SDM;		//screen coordinate
#endif	//SDM_TYPES_DEFINED
///////////////////////////////////////////////////////////////////////////////
//	Function storage class.

#define SDM_MAC_EXPORT

typedef union _pnt
	{
	struct
		{
		XY_SDM	x;
		XY_SDM	y;
		};
	XY_SDM	rgxy[2];
	} PNT;

typedef struct _rec
	{
	XY_SDM	x;
	XY_SDM	y;
	XY_SDM	dx;
	XY_SDM	dy;
	} REC;				// Rectangle. 

typedef POINTS	PT_SDM;


///////////////////////////////////////////////////////////////////////////////
// Convert RECT's to REC's, and vice-versa.

// Environment independant conversion.
#define	RectOfRec(P1, P2) \
	{ \
	(P1).left = (P2).x; \
	(P1).top = (P2).y; \
	(P1).right = (P2).x + (P2).dx; \
	(P1).bottom = (P2).y + (P2).dy; \
	}

// Environment dependant conversion.
#define RecOfRect(P1, P2) \
	{ \
	(P1).x = ((P2).left); \
	(P1).dx = ((P2).right - (P2).left); \
	(P1).dy = ((P2).bottom - (P2).top); \
	(P1).y = ((P2).top); \
	}

#define FPtInRec(P1, P2) \
	( \
	(P1).x >= (P2).x && \
	(P1).x < (P2).x + (P2).dx && \
	(P1).y >= (P2).y && \
	(P1).y < (P2).y + (P2).dy \
	)


///////////////////////////////////////////////////////////////////////////////
// Text Selection.							     
typedef U32_SDM	TXS;

#define	ichLimLast	0x7fff		// Go to end of edit item. 
#define TxsOfFirstLim(f, l)	((TXS)MAKELONG((f), (l)))
#define TxsAll()		TxsOfFirstLim(0, ichLimLast)
#define IchFirstOfTxs(txs)	LOWORD(txs)
#define IchLimOfTxs(txs)	HIWORD(txs)



///////////////////////////////////////////////////////////////////////////////
// Base dialog types.							     

typedef VOID **	HDLG;		// A near handle. 

#define	HdlgOfWNewWOld(wNew, wOld)	((HDLG)(wOld))
#define	HdlgOfWNewWOldFar(wNew, wOld)	((HDLG)MAKELONG((wNew), (wOld)))

typedef char * STR;            // String (SZ or STZ).
#define strNull		(char *)NULL	// (Use "char *" instead of SZ since 
					// not all apps. use cstd.h.)

typedef WCHAR * WSTR;
#define wstrNull (WCHAR *)NULL

#define hdlgNull	((HDLG) NULL)
#define hdlgError	((HDLG) -1)
#define hdlgCabError	((HDLG) -1)
#define hdltNull	((struct _dlt * *) NULL)



///////////////////////////////////////////////////////////////////////////////
// TMC.									     

typedef UINT_SDM	TMC;			// Item codes. 

// Standard item codes (tmc). 
#define	tmcNull		((TMC)0)
#define tmcError	((TMC)-1)
#define	tmcOK		((TMC)1)
#define	tmcOk		tmcOK
#define	tmcCancel	((TMC)2)
#define	tmcSysMin	((TMC)0x10)
#define	tmcSysMax	((TMC)0x400)
#define	ftmcGrouped	0x8000		// OR'd to specify whole group as 
					// opposed to first item. 
#define	tmcUserMin	tmcSysMax
#define	tmcUserMax	((TMC)ftmcGrouped)



///////////////////////////////////////////////////////////////////////////////
// Tmc macros. 

#define	TmcValue(tmc)		((tmc) & ~ftmcGrouped)
#define FIsGroupTmc(tmc)	((tmc & ftmcGrouped) != fFalse)


///////////////////////////////////////////////////////////////////////////////
// TMT
typedef	UINT_SDM	TMT;		/* TM types */


///////////////////////////////////////////////////////////////////////////////
// DLM.									     

typedef UINT_SDM	DLM;			// Dialog proc / item proc messages. 

// Global Messages. 
#define	dlmInit 	((DLM)0x0001)	// Do custom initialization. 
#define	dlmPlayBackInit ((DLM)0x0002)	// Do custom noninteractive init. 
#define	dlmTerm 	((DLM)0x0003)	// Termination for one of many 
					// reasons. 
#define	dlmExit		((DLM)0x0004)	// Dialog is about to be blown away. 

// Item messages. 
#define	dlmChange	((DLM)0x0005)	// Edit control may have changed. 
#define	dlmClick	((DLM)0x0006)	// Item was clicked. 
#define	dlmDblClk	((DLM)0x0007)	// Double click in listbox/radio. 
#define dlmClickDisabled ((DLM)0x0008)	// Noninteractive Dialog-sessions
					// only, the button passed to
					// FSetNoninteractive() has been 
					// disabled by a call to 
					// EnableNoninteractiveTmc() at dlmInit
					// time. 

// Rare Item Messages. 
#define dlmTab		((DLM)0x0009)	// Tab key intercept
#define	dlmKey		((DLM)0x000a)	// Any untrapped key. 
#define	dlmSetItmFocus	((DLM)0x000b)	// Item gets focus. 
#define	dlmKillItmFocus	((DLM)0x000c)	// Item loses focus. 

// Rare Dialog Proc Messages. 
#define dlmSetDlgFocus         ((DLM)0x000d)  // Dialog gets focus. 
#define dlmKillDlgFocus        ((DLM)0x000e)  // Dialog loses focus. 
#define dlmAdjustPos           ((DLM)0x000f)  // Adjust item's rec?
#define dlmTabOut              ((DLM)0x0010)  // Tab out of dialog? 
#define dlmIdle                ((DLM)0x0011)  // Idle for modal dialogs. 
#define dlmDlgClick            ((DLM)0x0012)  // Click in dialog's window. 
#define dlmDlgDblClick         ((DLM)0x0013)  // Double click in dialog's window. 
#define dlmShowCaret           ((DLM)0x0014)  // Show a caret.
#define dlmHideCaret           ((DLM)0x0015)  // Hide a caret.
#define dlmButtonDown          ((DLM)0x0016)  // Sent at mousedown on a button
#define dlmCreate              ((DLM)0x0017)  // Items are about to be created
#define dlmDlgMove             ((DLM)0x0018)  // Dialog window was just moved by user (JOHNTE)
#define dlmQueryNewPalette     ((DLM)0x0019)  // WM_QUERYNEWPALETTE (Win only)
#define dlmPaletteChanged      ((DLM)0x001a)  // WM_PALETTECHANGED (Win only)
#define dlmSubDialog           ((DLM)0x001b)  // New subdialog is about to appear
#define dlmFilterKey           ((DLM)0x001c)  // Dialog key message detected
#define dlmContextHelp         ((DLM)0x001d)  // Context Help (QuickTip)
#define dlmCtrlTab             ((DLM)0x001e)  // Ctrl+Tab pressed
#define dlmUpdateDefault       ((DLM)0x001f)  // give the app a chance
#define dlmDrag                ((DLM)0x0020)  // for Owner Drag listboxes
#define dlmRefEditShrink       ((DLM)0x0021)  //for notifying app of a dialog shrinking for ref edits
#define dlmChangeCheckBox      ((DLM)0x0022)  // for querying App if the Check box should be changed
#define dlmGetRefEditBmp       ((DLM)0x0023)
#define dlmGetRefEditBmpShrunk ((DLM)0x0024)
#define dlmSemiSubDialog       ((DLM)0x0025)  // half way through a sub dialog
#define dlmActivateApp         ((DLM)0x0026)  // Recieved WM_ACTIVATEAPP TRUE. 
#define dlmAskEnable           ((DLM)0x0027)  // Give a app to disallow enabling of TMC
#define dlmClickNotify         ((DLM)0x0028)  // User click on a disabled item
#define dlmRedisplayLbx        ((DLM)0x0029)  // Redisplay of list requested
#define dlmContextHelpOverride ((DLM)0x0030)  // Redisplay of list requested
#define dlmAutoLayout          ((DLM)0x0031)  // Called to invoke AutoLayout code
#define dlmComboBoxDrop            ((DLM)0x0032)  // Called when lbox is dropped down
#define dlmUserMin             ((DLM)0x0040)  // For App use. 


///////////////////////////////////////////////////////////////////////////////
// TMM Messages.							     

typedef UINT_SDM	TMM;			// Control proc messages.

// ListboxProc messages. 
#define	tmmCount	((TMM)0x0002)	// Return # of items. 
#define	tmmText 	((TMM)0x0003)	// Return text of n'th item. (always sent sequentially). 
#define	tmmEditText	((TMM)0x0004)	// Like tmmText but send randomly. 
#define tmmTooltipText ((TMM)0x0005) // Return tooltip for nth item (sent randomly)
// DON'T USE 0X0009.  (See tmm's for both general pictures and listboxes below.)
#define	cszUnknown	((UINT_SDM) -1)	// Return to tmmCount if unknown. 

// Gallery control context menu messages (a Gallery is a type of Listbox)
#define	tmmCountContext	((TMM)0x0006)	// Context menu entries this ilbe.
#define	tmmContextSTCR		((TMM)0x0007)	// The n'th Simple Toolbar Control Record.
#define	tmmContextClicked	((TMM)0x0008)	// Which STCR was clicked. 

// ParseProc messages. 
#define	tmmFormat	((TMM)0x0001)	// Format data. 
#define	tmmParse	((TMM)0x0002)	// Parse data. 

// Render Procs 
#define	tmmRender	((TMM)0x0001)	// Repaint entire item. 
#define tmmNewState	((TMM)0x0002)	// State has changed. 
#define tmmRepaint	((TMM)0x0003)	// Repaint everything but text.
#define tmmNewText	((TMM)0x0004)	// Repaint text alone

// GeneralPictures and ListboxProc 
#define	tmmCreate        ((TMM)0x0001)	// Create windows and such. 
#define	tmmAboutToResize ((TMM)0x0009)	// Listbox or general picture is about to resize.

// General Pictures 
// don't use the value 2 since tmmNewState is also used
// by general pictures
#define tmmPaint         ((TMM)0x0003)   // Paint yourself. 
#define tmmFocus         ((TMM)0x0004)   // Show yourself with/without focus.
#define tmmInput         ((TMM)0x0005)   // User input received.
#define tmmMeasure       ((TMM)0x0006)   // Provide optimal default control size based on content (DAL)
#define tmmResize        ((TMM)0x0007)   // Alert the control that its size has changed.
#define tmmMetricsChange ((TMM)0x0008)   // Alert the control that system metrics
                                         // have changed (as SDM is notified through
                                         // a WM_WININICHANGE message)
// DON'T USE 0X0009.  (See tmm's for both general pictures and listboxes above.)

#define tmmTabStop		((TMM)0x000a)	// Query if item is tabstop 
#define tmmDestroy		((TMM)0x000b)	// Inform item is being destroyed.
#define tmmMatchAccel	((TMM)0x000c)	// Attempt to match accelerator key
#define tmmWctControl	((TMM)0x000d)	// Complete a WPCTL structure (WM_GETCONTROLS)
#define tmmWctText		((TMM)0x000e)	// Fill a WTXI structure (WM_GETTEXT)
#define tmmWctListCount	((TMM)0x000f)	// Get list count (WM_GETLISTCOUNT)
#define tmmCtrlTab		((TMM)0x0010)	// Ctrl+Tab pressed
#define tmmAccelerate	((TMM)0x0011)	// picture was just sent focus by an accelerator key
#define tmmFilterKey	((TMM)0x0012)	// allows pictures to filter their keystrokes before SDM mucks with them.
#define tmmGccKeydown	((TMM)0x0013)	// keydowns for gcc color dropdowns
#define tmmGccDefaultChanged	((TMM)0x0014)	//default tmc changed, so gcc doesn't need to reset it
#define tmmGccClick     ((TMM)0x0015)   // a buttondown occured while a picture has the focus
#define tmmSysInput     ((TMM)0x0016)   // User input (syskeydown) received

#if VSMSODEBUG
#define tmmSaveBe	((TMM)0x003f)	// debug memory accounting
#endif

#define	tmmUserMin	((TMM)0x0040)	// For user extensions. 

#define wFilterEaten	((UINT_SDM)0xFFED)	// return value for tmmFilterKey when
											// the event was handled by the picture proc

typedef	struct _drm_sdm
	{
	WCHAR *	wzTmpl;
	WCHAR *	wzName;
	WCHAR *	wzType;
	} DRM_SDM;



///////////////////////////////////////////////////////////////////////////////
// FTMS - Item States.							     

typedef UINT_SDM	FTMS;

#define ftmsNull	((FTMS)0x0000)	// None of the following. 

// The folowing four alignment bits are specific to static text
// Note: the contiguity and ordering of the following 3 is assumed. 

#define ftmsLeft		((FTMS)0x0000)	// left aligned	(default)
#define ftmsCenter		((FTMS)0x0001)	// centered
#define ftmsRight		((FTMS)0x0002)	// right aligned
#define	ftmsAlt			((FTMS)(ftmsLeft|ftmsCenter|ftmsRight))

#define ftmsDefault		((FTMS)0x0002)	// Item is default pushbutton. 
#define ftmsPushed		((FTMS)0x0004)	// Button is "depressed".
#define ftmsMouseDown   ftmsPushed	// newer name for ftmsPushed
#define ftmsInvert		ftmsPushed	// Old name for ftmsPushed.
#define ftmsOn			((FTMS)0x0008)	// Item is "on". 
#define ftmsNinch		((FTMS)0x0010)	// Tri-state only - third state. 

// Note: the contiguity and ordering of the following 4 is assumed. 
#define ftmsEnable		((FTMS)0x0020)	// Item is enabled. 
#define ftmsVisible		((FTMS)0x0040)	// Item is invisible. 
#define ftmsMember		((FTMS)0x0080)	// Member of current subdialog. 

#define ftmsNewText		((FTMS)0x0100)	// Text has changed

//The following ftms are for drawing static text

#define ftmsMultiline	((FTMS)0x0200)	// multiline static text
#define ftmsBorder		((FTMS)0x0400)	// has a border
#define ftmsNoPrefix	((FTMS)0x0800)	// don't treat chAccel as a prefix

#define ftmsNoRedraw	((FTMS)0x1000)	// don't redraw until clear

#define ftmsLight		((FTMS)0x2000)	// use light font (sdi.hfontLight)

#define ftmsFocus		((FTMS)0x4000)	// Item has the focus.

#define ftmsTempHidden	((FTMS)0x8000)	// Item is temporarily hidden 
/* FMidEast */
#define ftmsHasReo		ftmsMultiline	// make edit item respect ftmsRTLReo
#define ftmsRTLReo		((FTMS)0x10000)	// draw static strings with RTL reading 
										// order. edit text requires ftmsHasReo
/* FMidEast End */

#define ftmsHyperlink   ((FTMS)0x20000) // Draw this control as a hyperlink
#define ftmsMouseOver	((FTMS)0x80000) // Draws control in the mouse over state

///////////////////////////////////////////////////////////////////////////////
// TMV - Item Values.							     

typedef UINT_SDM	TMV;

#define	tmvNoType	((TMV)0)
#define	tmvWord		((TMV)1)
#define	tmvFixed	((TMV)2)
#define	tmvString	((TMV)3)
#define tmvRgw		((TMV)4)


///////////////////////////////////////////////////////////////////////////////
// dlmChange notifications

typedef	UINT_SDM	FTMN;

#define	ftmnNull		((FTMN)0x0000)
#define	ftmnCombo		((FTMN)0x0001)
#define	ftmnCharValidated	((FTMN)0x0002)
#define	ftmnKillFocus		((FTMN)0x0004)
#define	ftmnCabVal		((FTMN)0x0008)


///////////////////////////////////////////////////////////////////////////////
// CAB info.								     

typedef void * * * PCABH;	// Pointer to a handle value inside of a CAB.
typedef void * * HCAB;	// CAB is an abstract data type. 
typedef void * PCAB;	// Pointer to arbitrary CAB.
typedef UINT_SDM			CABI;	// CAB initializer.
typedef UINT_SDM			IAG;  // Iag 
#define	Cabi(cwTotal, cHandle)		((cwTotal) + (cHandle << 8))
#define hcabNull	((HCAB)NULL)
#define hcabNonNull	((HCAB)1)
#define hcabNotFilled	((HCAB)-1)	// Could be returned by HcabFromDlg().

typedef unsigned SAB_SDM;

// Command argument block header. 
typedef struct _cabh
	{
	UINT_SDM	cwSimple;		// Total size of CAB less CABH. 
	UINT_SDM	cHandle;		// # of handles. 
	} CABH;

// Minimum CAB size : header + sab. 
#define	cwCabMin	((sizeof(CABH) + sizeof(SAB_SDM)) / sizeof(UCAB_SDM))
#define	cbCabMin	(cwCabMin * sizeof(UCAB_SDM))
#define	cbCabOverhead	(cwCabMin * sizeof(UCAB_SDM))

// Iag macro - returns iag corresponding to field fld in application structure
#define iagNil		((UINT_SDM)0x00ff)

// AKadatch: use FIELD_OFFSET
// #define Iag(sz, fld)	\
// 	((BARG_SDM)((BARG_SDM)&(((sz *)0)->fld) / sizeof(UCAB_SDM) - cwCabMin))
#define Iag(sz, fld)	\
	((BARG_SDM)((BARG_SDM)FIELD_OFFSET(sz,fld) / sizeof(UCAB_SDM) - cwCabMin))

// Macro to get void pointer to general CAB arg given offset.
#define	PvParseArg(hv, bArg)					\
	(((hv) == (VOID **)NULL) ? (VOID *)(bArg) :	\
		(VOID *)(*(char * *)(hv) + (bArg)))

//----------originally in sdmparse.h----------
#define SetPpvBToW(ppv, bArg, w) *((UCAB_SDM *) PvParseArg(ppv, bArg)) = w
#define WFromPpvB(ppv, bArg) *((UCAB_SDM *) PvParseArg(ppv, bArg))

/* Dialog Parse Values */

// Publisher has a conflict with the typedef of DPV, so in some cases, they
// need to be able to remove it by #defining REMOVE_TYPEDEF_DPV
#ifndef REMOVE_TYPEDEF_DPV
typedef int DPV;
#endif // REMOVE_TYPEDEF_DPV

#define dpvError	0x00
#define dpvNormal	0x01
#define dpvBlank	0x02
#define dpvAuto		0x04
#define dpvDouble	0x08
#define dpvSpaces	0x10 
//--------------------------------------------

// Macro to get void pointer to general CAB arg given iag.
#define PvFromCabIag(hcab, iag)					\
	((VOID *)(*((UCAB_SDM * *)(hcab)) + cwCabMin + (iag)))
		
// Cab string/data pointers. 
typedef	WCHAR *	WTZ_CAB;
typedef const WCHAR * CONST_WTZ_CAB;

typedef WCHAR * WZ_CAB;
typedef const WCHAR * CONST_WZ_CAB;

typedef char * SZ_CAB;
typedef const char * CONST_SZ_CAB;

typedef WCHAR * RGB_CAB;
typedef const WCHAR * CONST_RGB_CAB;

typedef WCHAR * WT_CAB;
typedef WCHAR * LWTZ_CAB;
typedef WCHAR * LRGB_CAB;


///////////////////////////////////////////////////////////////////////////////
// structure passed with dlmInit message

typedef struct _dmi_sdm
	{
	HDC	hpdc;
	HCAB	hcab;
	} DMI_SDM;



///////////////////////////////////////////////////////////////////////////////
// Other special values.

// Special ninch (No Input, No CHange) value. 
#define	wNinch		(-32767)	// Ints. 
#define	uNinch		((UINT_SDM)-1)	// Unsigned. 
#define	uNinchRadio	uNinch		// RadioGroups. 
#define	uNinchCheck	uNinch		// CheckBoxes 
#define	wNinchCheck	uNinchCheck	// Old name. 
#define	uNinchList 	((ILBE_SDM)-1) // Listboxes. 

#define	iszNinchList uNinchList		// Other name. 

// Special parse error values. 
#define	wError		(-32766)	// Ints. 
//original source: #define	uError		(0xfffe)	// Unsigneds. 
//current source:  #define	uError		((short)0xfffe)	// Unsigneds. 
#define	uError		((UINT_SDM)-2)	// Unsigneds. 

// Default no help. 
#define	hidDlgNull	0		// For no help.


// For Memory allocation callbacks
typedef U32_SDM 	SB_SDM;


// Scroll Bar notification messages (wNew in dlmClick)
typedef unsigned SBN_SDM;
#define sbnLineUp			SB_LINEUP
#define sbnLineDown			SB_LINEDOWN
#define sbnPageUp			SB_PAGEUP
#define sbnPageDown			SB_PAGEDOWN
#define sbnThumbPosition	SB_THUMBPOSITION
#define sbnThumbTrack		SB_THUMBTRACK
#define sbnTop				SB_TOP
#define sbnBottom			SB_BOTTOM
#define sbnEndScroll		SB_ENDSCROLL


//
// Max size of SDM strings, and the buffer size w/ zero term.
//
#define cchSDMWzMax    MSO_MAX_PATH
#define cchSDMWzBufMax (MSO_MAX_PATH + 1)
//
// This is our old string size, so that we can keep things the same for
// legacy dialogs, and so that we can use short strings if we'll never
// need anything larger than 255, like on tab controls.
//
#define cchSDMWzMaxShort 255
#define cchSDMWzBufMaxShort 256



///////////////////////////////////////////////////////////////////////////////
// Dialog Initialization.						     

typedef	U32_SDM	FDLG;

#define fdlgNull				((FDLG)0x00000000)

#define fdlgModal				((FDLG)0x00000001)	// Create Modal.
#define fdlgInvisible			((FDLG)0x00000002)	// Start invisible.
#define	fdlgEnableTabOut		((FDLG)0x00000004)	// Send dlmTabOut.

#define fdlgPopup				((FDLG)0x00000008)	// Popup dialog - Win only
#define fdlgScreenCoords		fdlgPopup			// Mac: same as above.

#define fdlgClipChildren		((FDLG)0x00000020)	// Clip controls - Win only
#define fdlgFedt				((FDLG)0x00000040)	// EditItem is FEDT - Win only
#define fdlgAdjustPos			((FDLG)0x00000080)	// Adjust item rec's.
#define fdlgOwnDC				((FDLG)0x00000100)	// Dialog owns a DC.
#define fdlgNoHelpIcon		   	((FDLG)0x00000200)	// No Win95 Help Icon - Win only
#define fdlgNoMvDefBut			((FDLG)0x00000400)	// Don't move defaultness

#define fdlgRepainting			((FDLG)0x00000800)	// currently processing WM_PAINT - Win only

#define fdlgEditConvert			((FDLG)0x00001000)	// ANSI -=> OEM -=> ANSI :-P  - !MAC only

#define fdlgSysModal			((FDLG)0x00002000)	// Sys modal dialog

#define fdlgEditMenu			((FDLG)0x00004000)	// TEs respond to edit menu - MAC only
#ifndef fdlgNoSabResize
#define fdlgNoSabResize			fdlgEditMenu		// let app resize on sab switch - !MAC only
#endif

#define fdlgDelayListbox		((FDLG)0x00008000)	// Delay listbox fill
#define fdlgHideAccel			((FDLG)0x00010000)	// No accel underlines
#define fdlgOnDemandSubdlg		((FDLG)0x00020000)	// Init only member controls
#define fdlgNoUpdate			((FDLG)0x00040000)	// Don't UpdateWindow
#define fdlgShrunk				((FDLG)0x00080000)	// RefEdits
#define fdlgRefEditEnabled	fdlgShrunk		// RefEdits - initialization, dialog is refedit enabled

#define fdlgUseTrueTypeFonts	((FDLG)0x00100000)	// MAC only, use truetype fonts for drawing
#define fdlgImeOff				fdlgUseTrueTypeFonts // FE - WinOnly, turn on DES IME control.

// This was added to help with SW Pane's perf wrt hidden controls. Since 
// SDM can't dynamically create controls, the maximum was allocated in 
// the resource file, but we don't need to show all of them most of the time.
// Note: same as the old fdlgMacDisabled, which is not referenced by MSO at all.
// (Raid O10 223434)
// Note: This flag is mutually exclusive of fdlgOnDemandSubdlg
// Note; O10 257833 Using this to lazy create tmw's can cause FEnumTmw to return fFalse for
// some functions. Check enormous for details.
#define fdlgCreateOnDemand      ((FDLG)0x00200000)	// Create and init only controls that are needed

// WARNING:  The following bits are private to SDM -- do not
// set these bits in dli.fdlg!  Additionally, support for these
// bits are not guaranteed.
// REVIEW fdlgSendUpdDflt isn't private.  We are out of bits, and need more!
#define fdlgDisabled		((FDLG)0x80000000)
#define fdlgHasButtons		((FDLG)0x40000000)	// Has a pushbutton.
#define fdlgInitializing	((FDLG)0x20000000)	// Is initializing.
#define fdlgAbort			((FDLG)0x10000000)	// Being destroyed.
#define fdlgListOOM			((FDLG)0x08000000)	// sevList hit.
#define fdlgCreating		((FDLG)0x04000000)	// Creating dialog.
#define fdlgNoninteractive	((FDLG)0x02000000)	// Non-Interactive?
#define fdlgDestroyed		((FDLG)0x01000000)	// Non-Interactive?
#define fdlgNoPaint			((FDLG)0x00800000)	// Don't paint dialog
#define fdlgChildDlg		((FDLG)0x00400000)	// MAC only, Dialog is item of parent
#define fdlgSendUpdDflt		fdlgChildDlg		// !MAC only, notify to update default ring
#define fdlgImeOn			fdlgMacDisabled		// FE - Turn On IME when a dialog gets focus.
#define fdlgPrivateMask		((FDLG)0xff800000)	// All private bits.

// REVIEW old FDLGS included so that we link properly
// with older applications

// all dialogs do this now
#define fdlgNoPopupRetDismiss	fdlgNull	// return in dropped does not dismiss popups
#define fdlgNoKey				fdlgNull	// keyboard interface for controls

#define FDlgCurNoKey() (fFalse)


#define FTestFdlg(fdlg, fdlgTest)	(((fdlg) & (fdlgTest)) != fdlgNull)
#define ClearFdlg(fdlg, fdlgClear)	((fdlg) &= ~(fdlgClear))
#define SetFdlg(fdlg, fdlgSet)		((fdlg) |= (fdlgSet))
#define	FlipFdlg(fdlg, fdlgFlip)	((fdlg) ^= (fdlgFlip))

typedef struct _dli			// DiaLog Initializer. 
	{
	HWND		hwnd;
	XY_SDM		dx, dy;
	FDLG		fdlg;
	UINT_SDM	wRef;
	BYTE *	rgb;			// App-supplied rgtmw (in sbDlg).
	DWORD	clrWindow;	// not used yet on the Mac
	SB_SDM		sb; // majic number to pass back to mem alloc callbacks
	}  DLI;
#define pdliNull	((DLI *)0)



///////////////////////////////////////////////////////////////////////////////
// Misc Functions .

extern UINT_SDM wRefDlgCur;		// Cached value. 
extern HCAB	hcabDlgCur;		// Cached value.

#define HcabDlgCur()	HcabQueryCur()
#define WRefDlgCur()	WRefQueryCur()

///////////////////////////////////////////////////////////////////////////////
// RenderProc environment-specific Draw Structure.			     

typedef struct _rds
	{
	HDC	hpdc;
	HWND		hwnd;
	RECT	rect;

	WCHAR * *	pwz;
	BOOL_SDM	fNoKey;	//no keybd interface
	} RDS;

#define SM_SETSECRET		0x800a	// Tell (any) FEDT text is "secret". 
#define SM_GETSECRET		0x800b  // Is the FEDT secret?

// Private message sent from fedt to dialog window.
#define SM_USER			0x8005


///////////////////////////////////////////////////////////////////////////////
// PictureProc message-specific parameter.				     

typedef struct _sdmp
	{
	HWND hwnd;
	REC *	prec;
	FTMS	ftms;
	union
		{
		LPMSG	lpmsg;                 // tmmInput
		unsigned chAccel;              // tmmMatchAccel
		BOOL_SDM fBackTab;             // tmmCtrlTab
		struct
			{
			BOOL_SDM fAction;          // tmmCreate
			BOOL_SDM fHelp;            // tmmCreate
			};
#ifdef SDM_WCT_DEFINED	//msoswct.h included
		LPWCTL lpwctl;                 // tmmWctControl
		struct
			{
			LPWTXI lpwtxi;             // tmmWctText
			int cch;                   // tmmWctText output
			};
		int cwz;                       // tmmWctListCount output
#endif //SDM_WCT_DEFINED
		struct                         
			{
			HDC   hpdc;                // tmmPaint, tmmMeasure
			POINT ptDesiredDimensions; // tmmMeasure, tmmAboutToResize
			};
		};
	} SDMP;

#define	psdmpNull	((SDMP *)NULL)

///////////////////////////////////////////////////////////////////////////////
// Procedure templates (for callbacks).					     

#define	SDM_CALLBACK	PASCAL	// Far callback.

typedef BOOL_SDM (SDM_CALLBACK * PFN_DIALOG)(DLM, TMC, UCBK_SDM, UCBK_SDM, UCBK_SDM);
typedef PFN_DIALOG	PFN_ITEM;

// EB/EL Cab Save CallBack. 

typedef VOID (SDM_CALLBACK * PFN_SAVECAB)(HCAB, UINT_SDM, TMC, BOOL_SDM);
#define pfnSaveCabNull	((PFN_SAVECAB)0)

// Top level Modal Message Filter. 
typedef BOOL_SDM (SDM_CALLBACK * PFN_FILTERMSG)(LPMSG);

#define pfnFilterMsgNull	((PFN_FILTERMSG)0)

typedef int (WINAPI * PFN_ALERT)(HWND, LPCWSTR, LPCWSTR, UINT);
#define pfnAlertNull		((PFN_ALERT)NULL)

// General control proc template. 
typedef UCBK_SDM (SDM_CALLBACK * PFN_CTRL)(TMM, VOID *, UCBK_SDM, UCBK_SDM, TMC, UCBK_SDM);
#define pfnCtrlNull	((PFN_CTRL)0)

typedef UCBK_SDM (SDM_CALLBACK * PFN_PIC)(TMM, SDMP *, UCBK_SDM, UCBK_SDM, TMC, UCBK_SDM);
typedef UCBK_SDM (SDM_CALLBACK * PFN_PARSE)(TMM, WCHAR *, VOID * *, BARG_SDM, TMC, UCBK_SDM);
typedef UCBK_SDM (SDM_CALLBACK * PFN_LISTBOX)(TMM, WCHAR *, ILBE_SDM, UCBK_SDM, TMC, UCBK_SDM);
typedef UCBK_SDM (SDM_CALLBACK * PFN_RENDER)(TMM, RDS *, FTMS, FTMS, TMC, UCBK_SDM);
typedef PFN_PARSE	PFN_FORMAT;

// Autocomplete call back
// Second Argument(WCHAR *) is the string to be autocompleted
// Third Argument(WCHAR *) is the return string by the function
// Return TRUE if autocompleting should occur
// Return FALSE if no changes(i.e. autocompletion) should occur
typedef BOOL_SDM (SDM_CALLBACK * PFN_AUTOCOMPLETE)(void *, TMC, WCHAR *, WCHAR *);

// Resize callback
// first arg = tmc of control being moved
// second arg = REC pointing at move location
typedef void (SDM_CALLBACK * PFN_RESIZE)(TMC, REC *);


// Dialog Creation callback
// Argument is the new dialog hwnd which might be useful for subclassing.
typedef void (SDM_CALLBACK * PFN_DLGCREATED)(HWND);

// Peek/GetMessage callback
// 
typedef BOOL_SDM (SDM_CALLBACK *SDM_PFNPEEKMESSAGE)(MSG *, HWND, UINT, UINT, UINT);
typedef BOOL_SDM (SDM_CALLBACK *SDM_PFNGETMESSAGE)(MSG *, HWND, UINT, UINT);
///////////////////////////////////////////////////////////////////////////////
// Procedure Templates.							     


MSOAPI_(HDLG) HdlgGetCur( void );
MSOAPI_(HDLG) HdlgGetFocus( void );
MSOAPI_(BOOL) MsoFDlgIsWorkpane(HDLG hdlg);
MSOAPI_(void) MsoProtectSDMInWorkpane(void);
MSOAPIX_(void) MsoUnprotectSDMInWorkpane(void);
MSOAPI_(void) MsoNotifySdmOfFocus(TMC tmc, BOOL fFocus);
MSOAPI_(FARPROC) LpfnMsoSetSdmMessageWrap(FARPROC lpfn, int fUnicode, int fPeek);


///////////////////////////////////////////////////////////////////////////////
// FtmeIsSdmMessage() return values.					     

// Need special return values for functions that normally return fTrue/fFalse.
typedef UINT_SDM		FTME;
#define ftmeNull	((FTME)0)
#define ftmeTrue	((FTME)1)
#define ftmeError	((FTME)2)
#define ftmeDone	((FTME)4)
#define ftmeEaten	((FTME)8)	// modal dialog is front window,
					// and received click on other 
					// window.  App should beep.b




///////////////////////////////////////////////////////////////////////////////
// Hard-coded callbacks.						     

// Out Of Memory Support. 
typedef	UINT_SDM		SEV;
#define	sevMinor	1		// Minor (painting) error.
					// don't cast, since used in MASM
#define sevMajor	((SEV)2)	// Major error.
#define sevLmem		((SEV)3)	// Out of LMEM memory.
#define sevHcabFromDlg	((SEV)4)	// HcabFromDlg() failure.
#define sevList		((SEV)6)	// ListBox fill failure.

/* Current SDM mem functions. */
#ifndef PpvSdmAllocCb
MSOMACAPI_(VOID**) 	SDM_CALLBACK PpvSdmAllocCb(SB_SDM, UINT_SDM);
MSOMACAPI_(BOOL_SDM) SDM_CALLBACK FSdmReallocPpv(SB_SDM, VOID **, UINT_SDM);
MSOMACAPI_(VOID)		SDM_CALLBACK FreeSdmPpv(SB_SDM, VOID **);
#if VSMSODEBUG
MSOMACAPI_(BOOL_SDM) SDM_CALLBACK FSaveSdmBe(HMSOINST hinst, LPARAM lparam, VOID** ppv, int bt);
#endif
MSOMACAPI_(UINT_SDM)	SDM_CALLBACK CbSdmSizePpv(SB_SDM, VOID **);
MSOMACAPI_(BOOL_SDM)	SDM_CALLBACK FSdmDoIdle(BOOL_SDM);
MSOMACAPI_(BOOL_SDM)	SDM_CALLBACK FRetrySdmError(UINT_SDM, HDLG, SEV);
#endif

/* Default SDM mem functions. */
MSOMACAPIX_(VOID**) 	SDM_CALLBACK MsoPpvSdmAllocCb(SB_SDM, UINT_SDM);
BOOL_SDM SDM_CALLBACK MsoFSdmReallocPpv(SB_SDM, VOID **, UINT_SDM);
VOID		SDM_CALLBACK MsoFreeSdmPpv(SB_SDM, VOID **);
UINT_SDM	SDM_CALLBACK MsoCbSdmSizePpv(SB_SDM, VOID **);
#if VSMSODEBUG
BOOL_SDM SDM_CALLBACK MsoFSaveSdmBe(HMSOINST hinst, LPARAM lparam, VOID** ppv, int bt);
#endif
BOOL_SDM	SDM_CALLBACK MsoFSdmDoIdle(BOOL_SDM);
BOOL_SDM	SDM_CALLBACK MsoFRetrySdmError(UINT_SDM, HDLG, SEV);

// Bitmap support - handle from id. 

typedef	HBITMAP	HBITMAP_SDM;

HBITMAP_SDM	SDM_CALLBACK	MsoHbmpFromIBmp(UINT_SDM);
#ifndef HbmpFromIBmp
MSOMACAPI_(HBITMAP_SDM)	SDM_CALLBACK HbmpFromIBmp(UINT_SDM);
#endif
#define	hbmpNull	((HBITMAP_SDM)NULL)


///////////////////////////////////////////////////////////////////////////////
// Misc types.								     

#define hNull		NULL                    	// Generic null handle.
#define	ppvNull		NULL                    	// Null lmem handle.

#define hfontNull	((HFONT)NULL)



///////////////////////////////////////////////////////////////////////////////
// SDM Initialization structure.					     

typedef FARPROC	SDM_FARPROC;

#ifndef	lpfncompNull
#define	lpfncompNull	((SDM_FARPROC)NULL)
#endif	//!lpfncompNull


typedef struct _sdi
	{
	char *	szApp;			// Unique application name. 
	HANDLE	hinstCur;		// Current application instance. 
	HANDLE	hinstPrev;		// Previous application instance. 
	HCURSOR	hcursorArrow;		// For dialog class
	HDC	hdcMem;			// Memory DC.
	XY_SDM	dyLeading;		// TextMetrics tmExternalLeading.

	HWND	hwndApp;		// Application's main window.
	XY_SDM	dxSysFontChar;		// Width of system font (average).
	XY_SDM	dySysFontChar;		// Height of system font (maximum).
	XY_SDM	dySysFontAscent;	// Height of system font ascenders.

	PFN_FILTERMSG	pfnFilterMsg;	// Message filter callback.
	PFN_ALERT		pfnAlert;	// client supplied replacement for MessageBox

	SB_SDM	sbEL;	// majic number

	char *	szFedtClass;		// Class name for FEDT. 

	SDM_FARPROC	lpfncomp;
	char *	szScrollClass;		// class for listbox scrollbars

	HFONT	hfont;			// Font for dialogs; use system font if NULL
	HFONT	hfontLight;		// Font for "light" text items; use hfont if NULL
	HFONT	hfontBold;		// Bold font; is hfont for FE

	BOOL_SDM (SDM_CALLBACK *pfnDlmDlgFilter) (DLM, TMC, UCBK_SDM, UCBK_SDM, UCBK_SDM);
	BOOL_SDM (SDM_CALLBACK *pfnDlmItemFilter) (TMM, TMC, UCBK_SDM, UCBK_SDM, UCBK_SDM);
	BOOL_SDM (SDM_CALLBACK *pfnDlmEmDlgFilter) (TMM, TMC, UCBK_SDM, UCBK_SDM, UCBK_SDM);

	HBITMAP_SDM (SDM_CALLBACK *pfnHbmpFromIBmp) (UINT_SDM);
	BOOL_SDM	(SDM_CALLBACK *pfnFSdmDoIdle)(BOOL_SDM);
	BOOL_SDM	(SDM_CALLBACK *pfnFRetrySdmError)(UINT_SDM, HDLG, SEV);
	UINT_SDM	cbWndExtraClient;
	BOOL_SDM	fChisled;	//that gratuitous "chisled" look
	BOOL_SDM	fPixelScale;	// no scaling (JOHNTE)
	BOOL_SDM	fExtendedTmm;	// extended listboxes get tmmCount?
	int		iDBCSCtry;

	WNDPROC pfnLSdmWP; // class window proc
	UINT_SDM	lid;				// Language for dialogs
	UINT_SDM	uCodePage;			// Code page for dialogs

	SDM_PFNPEEKMESSAGE pfnPeekMessageW;
	SDM_PFNPEEKMESSAGE pfnPeekMessageA;
	SDM_PFNGETMESSAGE  pfnGetMessageW;
	SDM_PFNGETMESSAGE  pfnGetMessageA;
	} SDI;

#define psdiNull	((SDI *)0)



///////////////////////////////////////////////////////////////////////////////
// DLG access. defined here for the following macros that are defined        

typedef	struct _dlh
	{
	struct _dlt * * hdlt; // Dialog Template. 
	HCAB	hcab;			// Initial CAB for Dialog.
	HWND	hwndDlg;		// Dialog's (frame) window. 

#define	hwndDlgClient	hwndDlg		// I know its gross, but it saves a lot
					// of mess in the sdm .c files. 

	UINT_SDM wRef;		// User supplied Dialog word. 
	FDLG	fdlg;			// Dialog flags. 
	UINT_SDM	hid;			// Help ID.
	UINT_SDM	hidBase;	  	// Base Help ID.
	TMC tmcContext;	// tmc that was clicked in QuickTip mode
	BOOL_SDM fTmcContextEnabled;	// was that tmc enabled or not?
	PT_SDM ptClick;     // Mouse position of click for QuickTip
	} DLH;				// Public part of DLG structure.



///////////////////////////////////////////////////////////////////////////////
// SDS access. (Only defined here for the following macros that are defined  
// by the SDM project.  This struct is NOT for public use!)                  


typedef struct _sds_sdm			// State of SDM. 
	{
	SB_SDM	sbDlgCur;		// SbDds if not SDM_MULTI_SB. 
	SB_SDM	sbDlgFocus;
	void * * ppdlgCur;	// Current Dialog.
	void * * ppdlgFocus; // Dialog with input focus.
	} SDS_SDM;
#define SbsdmDlgCur()	sds.sbDlgCur

///////////////////////////////////////////////////////////////////////////////
// Other common functions 

// Return the value TMC (first TMC in group usually) or do nothing if not
//	grouped.
#define	TmcValue(tmcG)	((tmcG) & ~ftmcGrouped)
#define FIsGroupTmc(tmc)	((tmc & ftmcGrouped) != fFalse)



///////////////////////////////////////////////////////////////////////////////
// General objects support.

typedef VOID **	HOBJ_SDM;			// A near handle.

#define hobjNull	((HOBJ_SDM) NULL)

typedef struct _gobj
	{
	HOBJ_SDM	hobjBase;	// Handle to base object.
	BARG_SDM	bArg;		// Offset from that object.
	} GOBJ;			// Reference to any object.

#define	GobjToGobj(g1, g2)	(g2 = g1)

#define FIsNullGobj(g)		((g).hobjBase == hobjNull && (g).bArg == 0)
#define	FIsNullHobjbArg(h, b)	((h) == hobjNull && (b) == 0)
#define PvFromGobj(h, b)	\
	((h) == hobjNull ? (VOID *)(b) : *(VOID * *)(h) + (b))

#define	FIsEmptyGobj(g)		FIsEmptyHobjBArg((g).hobjBase, (g).bArg)

#define FIsEmptyHobjBArg(h, b)	((h) == hobjNull && (b) == 0)

///////////////////////////////////////////////////////////////////////////////
// Miscellaneous LBOX stuff (for apps that don't include msolbox.h but use the  //
// toolbox.								     //
///////////////////////////////////////////////////////////////////////////////

typedef	struct	_LBX * *	HLBX;

typedef	RECT	LBR;

typedef	UINT_SDM	LBC;

#define	LBOX_CALLBACK		SDM_CALLBACK
#define	LBOX_CALLBACK_NAT	SDM_CALLBACK


typedef struct _lbm
	{
	HLBX hlbx;
	ILBE_SDM cEntryVisible;	// Number of entries visible in list
	XY_SDM dySmall;			// Height of small list
	} LBM;

#define	cEntryVariable	((ILBE_SDM)0x8000)

#include "msosproc.h"


///////////////////////////////////////////////////////////////////////////////
// rgtmw allocataion stuff
///////////////////////////////////////////////////////////////////////////////

// DOUBLE SUPER WARNING - this is a private SDM structure, here only
// because the client has to allocate these things. Use these fields,
// and it's guaranteed you will get broken.
//
// Sizeof runtime space.
// WARNING WARNING WARNING WARNING WARNING WARNING WARNING WARNING
// This structure is also defined in sdmnat.asm.  If you change it here,
// then change it there!!
// WARNING WARNING WARNING WARNING WARNING WARNING WARNING WARNING
typedef U32_SDM	FTMW_SDM;

typedef struct _tmw
	{
	//WARNING: for 32 bit alignment, the items in this structure must
	// be ordered with all 32-bit data before any 16-bit data
	// (so the hokey CbRuntimeCtm will work out right)

	union
		{
		HWND		     hwnd;	// Window handle for edit.

		HLBX	hlbx;	//list
		WCHAR * * pwzTitle;	// Control text.
		UCAB_SDM ibmp;		//non-CAB bitmap id
		} ctl;

	union
		{
		GOBJ	gobjTitle;		// Item title.
		};
	GOBJ	gobjData;		// CAB (or other) data.

	REC	rec;			// Scaled rectangle.
#ifdef DBCS
	BYTE bCharSet;
#endif
	DWORD himcOld;		//Original himc assosiated to edit contorol.

	U32_SDM	lUser;			// User dword

	FTMS	ftms;			// DialogItem state - see list.
	FTMS	ftmsOld;		// Previous state (for buttons).
	FTMW_SDM	ftmw;			// DialogItem flags.

	TMC	tmc;			// iTeM Code.
	TMT	tmt;			// Item type.
	UINT_SDM	cwVal;			// Data size (-1=> var length).
	TMC	tmcGroup;		// tmcNull or TMC of group (for Combo/
					// ListBox/RadioButton).
	int	tcidIcon;	// tcid of icon used inside a button
	int iWidth;		// width of the galley control.

	WCHAR *wtzTooltip; // wtz of the tooltip for this item or NULL
	int    fmtTip;     // format of the tip

	int cdxText;	// Used for caching text dimensions for DAL.
	int cdyText;	// these are only valid if != TEXT_MEASURE_NOT_CACHED
	DWORD grfSDMTextMeasure;

// SDM_PRIVATE

///////////////////////////////////////////////////////////////////////////////
// -- Private part of TMW_SDM, only used by sdmtmw

	// The following 4/5 fields are indices back into the template.
	UINT_SDM itmBase;		// Index of base tm for item.
/* FMidEast */
	U16_SDM itmExt2;		// Extension #2 or itmNil.
	U16_SDM itmExt3;		// Extension #3 or itmNil.
/* FMidEast End */
	U16_SDM itmExt4;
	UINT_SDM itmTmxdFirst;		// First dummy item or itmNil.
	UINT_SDM itmTmxiFirst;		// First item proc or itmNil or itmUnknown.

	WCHAR	ch;			// Accelerator.
	WCHAR	ch2;			// Kana Accelerator.
	BYTE	iagData;		// CAB index of value.
	BYTE	fNonseqTmc:1;		// => Special or Imported TMC.
	BYTE	fHandleData:1;		// If iagData != iagNil, => if data is
					// handle.
	BYTE	fAction:1;		// => call dialog proc.
	BYTE	fHelp:1;		// => wants context help
	BYTE	fRichEdit :1; //Indicates the genralpicture is for Word's RTE.
	BYTE	fSpecialTab :1;		// => wants the special autocomplete tab behaviour
	BYTE	fUnused :2;
//SDM_PRIVATE
	} TMW_SDM_PRIVATE;

#define CbRuntimeCtm(ctm) ((ctm) * sizeof(TMW_SDM_PRIVATE))

#define iFENone		0
#define iFEJapan	81
#define iFEKorea	82
#define iFETaiwan	886
#define iFEPRChina	86

// Flags that can be passed in as a grf to MsoUPicContainerEx
#define msoupicExtended         0x00000001
#define msoupicNoResize         0x00000002

MSOAPI_(UCBK_SDM SDM_CALLBACK) MsoWPicContainer(HMSOINST pinst,
												void **hwndDlg,
												TMM tmm, SDMP *psdmp,
												UCBK_SDM rsvd2, UCBK_SDM rsvd,
												TMC tmc, UCBK_SDM wBtn,
												/* PFNFFillPictureContainer */ void *pfnFPC);

MSOAPI_(UCBK_SDM SDM_CALLBACK) MsoUPicContainerEx(HMSOINST pinst,
												void **hwndDlg,
												TMM tmm, SDMP *psdmp,
												UCBK_SDM rsvd2, UCBK_SDM rsvd,
												TMC tmc, UCBK_SDM wBtn,
												void *pfnFPC, UINT grf);

// Call to set the contexthelp override
MSOAPIX_(void) MsoSetContextOverride(BOOL fContextHelpOverride);

// Call to set the autocomplete function for a dialog.
MSOAPI_(void) MsoSetPfnAutoComplete(/*PFN_AUTOCOMPLETE*/ void *pfnAutoComplete, void *pvData);

// Return the WhatsThis status
MSOAPIX_(int) MsoSDMWhatsThis();

//Following is the private message sent by sdm to edit filter 
//it sends unicode string
#define msoWmSdmPrivSetTextMsg	(WM_USER+289)
#define msoWmSdmPrivGetTextMsg	(WM_USER+290)

#endif	//!SDM_INCLUDED		Entire file.
