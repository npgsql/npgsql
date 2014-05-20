#pragma once

/*------------------------------------------------------------------------*
 * msoswct.h (previously known as sdmtowct.h): SDM PUBLIC include file    *
 *   containing the interface to be used for communication from outside   *
 *   applications with SDM dialogs.                                       *
 *                                                                        *
 * Please do not modify or check in this file without contacting NicoleP. *
 *------------------------------------------------------------------------*/


#ifndef SDM_WCT_DEFINED
#define SDM_WCT_DEFINED

//------------------------------------------------------------------------
// WCT/SDM Values - Definitions and descriptions
//------------------------------------------------------------------------
#define wVerAnsi			2		// Ansi strings
#define wVerUnicode			3		// Unicode strings
#define wVerWord			wVerAnsi	//historical -- this should be removed at some point
#define wVerAnsiExt			4		// Ansi with extened functionality. (off10 and beyond)
#define wVerUnicodeExt		5	// Unicode with extended functionality. (off10 and beyond)

#define FIsWVerAnsi(wVer) (((WORD)(wVer) == wVerAnsi) || ((WORD)(wVer) == wVerAnsiExt))
#define FIsWVerUnicode(wVer) (((WORD)(wVer) == wVerUnicode) || ((WORD)(wVer) == wVerUnicodeExt))
#define FIsWVerExt(wVer) (((WORD)(wVer) == wVerAnsiExt) || ((WORD)(wVer) == wVerUnicodeExt))
#define FIsWVerValid(wVer) (((WORD)(wVer) == wVerAnsi) || ((WORD)(wVer) == wVerAnsiExt) || ((WORD)(wVer) == wVerUnicode) || ((WORD)(wVer) == wVerUnicodeExt))

// A pointer to an array of WCTL structures is passed as the lParam
//  in a WM_GETCONTROLS message
//
typedef struct _wctl
	{
	WORD wtp;				// Item type
	WORD wId;				// Unique identifier within this dialog (TMC)
	WORD wState;			// Current value if fHasState
	WORD cchText;			// Size of text value, if fHasText
	WORD cchTitle;			// Size of title, if fHasTitle
	RECT rect;				// Rectangle in dialog window
	LONG fHasState:1;		// Can this type of item have a numeric state?
	LONG fHasText:1;		// Can this type of item have a text value?
	LONG fHasTitle:1;		// Does the item have a title?
	LONG fEnabled:1;		// Is the item currently enabled?
	LONG fVisible:1;		// Is the item visible?
	LONG fCombo:1;			// Is the item a combo edit or listbox?
	LONG fSpin:1;			// Is the item a spin edit?
	LONG fOwnerDraw:1;	// Is the item owner-draw (or extended listbox)?
	LONG fCanFocus:1;		// Can the item receive focus?
	LONG fHasFocus:1;		// Does the item have focus?
	LONG fList:1;			// Supports wtxi.wIndex, WM_GETLISTCOUNT
	LONG fPageTabs:1;	  	// Is the item a page tab list?
	LONG fSelected:1;	  	// Is the item selected (for general pictures)?
	LONG fIsSecret:1;		// is a secret (passworded) edit control.
	LONG lReserved:18;	// A bunch o' bits
	WORD wParam1;			// for tmtStaticText, tmtFormattedText
	union
		{
		struct
			{
			WORD wParam2;	// as above
			WORD wParam3;	// yet another spare value for drawing routines
			};
		LONG lParam;		// long version of spare value
		HWND hwnd;			// hwnd of general picture control, if any
		};
	} WCTL, *PWCTL, FAR *LPWCTL;

/* Possible values for wctl.wtp */
#define wtpMin				1
#define wtpStaticText		1
#define wtpPushButton		2
#define wtpCheckBox			3
#define wtpRadioButton		4
#define wtpGroupBox			5
#define wtpEdit				6
#define wtpFormattedText	7
#define wtpListBox			8
#define wtpDropList			9
#define wtpBitmap			10
#define wtpGeneralPicture	11
#define wtpScroll			12
#define wtpHyperlink		13
#define wtpMax				14

// A pointer to a WTXI structure is passed as the lParam of
//  a WM_GETCTLTEXT or WM_GETCTLTITLE message.
// DONT change this structure EVER.
typedef struct _wtxi		// WinWord text info
	{
	LPWSTR	lpszBuffer;		// Buffer to receive string
	WORD	cch;			// Size of buffer to receive string, in chars
	WORD	wId;			// Item identifier (TMC) (as in wctl.wId)
	RECT	rect;			// Used for WM_GETCTLTITLE and general picture lists
	WORD	wIndex;		// Used for WM_GETCTLTEXT on ListBoxes or if WCTL.fList
	} WTXI, *PWTXI, FAR *LPWTXI;

// Don't change this structure without checking with PaulCole (RCA: sjade, peterth)
// Because of external dependencies entries may only be added to the end of 
// the extended structure and may only be used intereally if the 
// wVerId version flag is Extended.
typedef struct _wtxi_extended
	{
	// Everything in the WTXI struct... don't ever alter this
	LPWSTR	lpszBuffer;		// Buffer to receive string
	WORD	cch;			// Size of buffer to receive string, in chars
	WORD	wId;			// Item identifier (TMC) (as in wctl.wId)
	RECT	rect;			// Used for WM_GETCTLTITLE and general picture lists
	WORD	wIndex;		// Used for WM_GETCTLTEXT on ListBoxes or if WCTL.fList

	// OK you can add things below here, but when accessing them you must
	// ensure that the ver ID is one of the extended versions.
	union
		{
		LONG lFlags;

		struct
			{		
			LONG fIsSecret:1;	// Use only with a password edit field.
			LONG unused:31;
			};
		};		
	} WTXI_EXTENDED, *PWTXI_EXTENDED, FAR *LPWTXI_EXTENDED;



//------------------------------------------------------------------------
// WCT/SDM MESSAGES - Definitions and descriptions
//------------------------------------------------------------------------

#define WM_GETCOUNT		0x7FFE
	// Returns the number of bytes needed to store control info.
	//	wParam	- the version id
	//		Must be wVerAnsi[Ext] or wVerUnicode[Ext]
	//	lParam	- Unused
	//		Must be 0

#define WM_GETCONTROLMSAA 0x7FF3
#define WM_GETCONTROLSSHAREDMEM 0x7FF6
#define WM_GETCONTROLS	0x7FF7
	// Retrieves control information for the dialog.
	//	wParam	- the version id
	//		Must be wVerAnsi[Ext] or wVerUnicode[Ext]
	//	lParam	- LPWCTL
	//		Must be at least the size returned by WM_GETCOUNT
	// Return value is the number of WCTL structures filled.

#define WM_GETCTLTEXT	0x7FFD
	// Retrieves the text value for the specified control
	//	wParam	- the version id
	//		Must be wVerAnsi[Ext] or wVerUnicode[Ext]
	//	lParam	- LPWTXI
	//		(*lParam)->wId is the wctl.wId retrieved by WM_GETCONTROLS.
	//		For a listbox (wtpListBox or wtpDropList) (*lParam)->wIndex
	//		  must be the index of the listbox entry to be retrieved.

#define WM_GETCTLTITLE	0x7FFC
	// Retrieves the title of the specified control
	//	wParam	- the version id
	//		Must be wVerAnsi[Ext] or wVerUnicode[Ext]
	//	lParam	- LPWTXI
	//		(*lParam)->wId is the wctl.wId retrieved by WM_GETCONTROLS

#define WM_GETCTLFOCUS	0x7FFB
	// Returns the wId (TMC) (as in wctl.wId) of the control with focus.
	//	wParam	- the version id
	//		Must be wVerAnsi[Ext] or wVerUnicode[Ext]
	//	lParam	- Unused
	//		Must be 0

#define WM_SETCTLFOCUS	0x7FFA
	// Sets focus to the specified control
	//	wParam	- the version id
	//		Must be wVerAnsi[Ext] or wVerUnicode[Ext]
	//	lParam	- a wId value as retrieved by WM_GETCONTROLS

#define WM_GETLISTCOUNT 0x7FF9
	// Returns the number of entries in a listbox
	// wParam	- the version id
	//		Must be wVerAnsi[Ext] or wVerUnicode[Ext]
	//	lParam	- a wId value as retrieved by WM_GETCONTROLS
	//		Must be a listbox (wtpListBox or wtpDropList)

#define WM_GETHELPID	0x7FF8
	// Returns the dialog's Help ID
	// wParam	- the version id
	//		Must be wVerAnsi[Ext] or wVerUnicode[Ext]
	//	lParam	- Unused
	//		Must be 0

#define WM_GETCONTROLSMOUSEDRV 0x7FFF
	// special "light" version of WM_GETCONTROLS used by the mouse 9.01 driver

#define WM_GETCONTROLSMSAA 0x7FF4
	// version of WM_GETCONTROLS used by MSAA 

#define WM_GETDROPDOWNID 0x7FF5
	//Returns the item identifier (TMC) of the control that currently owns the dropdown list window
	//This message should be sent directly to the dropdown list window.
	// wParam	- the version id
	//		Must be wVerAnsi[Ext] or wVerUnicode[Ext]
	//	lParam	- Unused
	//		Must be 0

//------------------------------------------------------------------------
// WIN32 Memory Access
//------------------------------------------------------------------------

#define SZWCTNAME "SDMWCT"
#define hFileNoneWct -1

//------------------------------------------------------------------------
// WCT/SDM error return values.
//------------------------------------------------------------------------

#define	errWctOOM			0xfffc	// Out o' memory!
#define	errNoCurrentDlg	0xfffd	// Attempt to get info for non-existent dialog
#define errCountCtls	0xfffe	// Invalid buffer size
#define	errInvalidVerId	0xffff	// Invalid Version of the data structure
#define errNotSDM		0x0000	// WinPRocs return 0 for unknown msgs by default

#define	uNoValue		0x000e	// Value of control where control has no numeric value

#endif //SDM_WCT_DEFINED

