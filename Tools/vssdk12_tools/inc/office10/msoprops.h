#pragma once

/****************************************************************************
	msoprops.h

	Owner: MartinTh
 	Copyright (c) 1994-95 Microsoft Corporation

	This file contains the exported interfaces and declarations for
	the Extended OLE Properties.
****************************************************************************/
#ifndef MSOPROPS_H
#define MSOPROPS_H
#include <msodig.h>
#include <msobp.h>
#pragma pack( push, msoprops, 4 )

//	Call back functions implemented by client apps (those will be passed to mso96(95).dll
//	as function pointers) must explicitly declare its calling convention using the
//	OFC_CALLBACK macro
//	TODO: 	Will investigate if we define OFC_CALLBACK as  MSOSTDAPICALLTYPE, they are
//			defined as the same except in Mac builds, (of course Mso95 doesn't have a Mac
//			build, yet)
//	PRIORITY:6
//	DIFFICULTY:1

#define OFC_CALLBACK __stdcall

////////////////////////////////////////////////////////////////////////////////
// EXTENDED OLE DOC PROPERTIES APIs
// Overview:
//              To use extended ole properties do the following
//              1.Open your file
//              2.Call FOfficeCreateAndInitObjects: This will create 3 objects which are
//                      siobj (sum info obj
//                      dsiobj (doc sum info obj)
//                      udobj (user defined data or custom obj)
//               and provides a pointer to each of these.
//               To make any subsequent calls, you will have to provide the pointer to the
//               appropriate object.
//              3.Before you close a file call FOfficeDestroyObjects.
////////////////////////////////////////////////////////////////////////////////
//
// Summary Information interface API.
//
// Notes:
//  - define OLE_PROPS to build OLE 2 interface objects too.
//
// The actual data is stored in SUMINFO.  The layout of the first
// 3 entries must not be changed, since it will be overlayed with
// other structures.  All property exchange data structures have
// this format.
//
// The first parameter of all functions must be LPSIOBJ in order for these
// functions to work as OLE objects.
//
// All functions defined here have "SumInfo" in them.
//
// Several macros are used to hide the stuff that changes in this
// file when it is used to support OLE 2 objects.
// They are:
//   SIVTBLSTRUCT - For OLE, expands to the pointer to the interface Vtbl
//              - Otherwise, expands to dummy struct same size as Vtbl
//   LPSIOBJ    - For OLE, expands to a pointer to the interface which is
//                just the lpVtbl portion of the data, to be overlayed later.
//              - Otherwise, expands to a pointer to the whole data
//
////////////////////////////////////////////////////////////////////////////////

  // Apps should use these for "Create" calls to fill out rglpfn
#define ifnCPConvert    0               // Index of Code Page Converter
#define ifnFWzToNum     1               // Index of Sz To Num routine
#define ifnFNumToWz     2               // Index of Num To Sz routine
#define ifnFUpdateStats 3               // Index of routine to update statistics
#define ifnMax          4               // Max index

  // Predefined Security level values for Property Sets in the standard
#define SECURITY_NONE                   0x0     /* No security */
#define SECURITY_PASSWORD               0x1     /* Password-protected */
#define SECURITY_READONLYRECOMMEND      0x2     /* Read-only access recommened */
#define SECURITY_READONLYENFORCED       0x4     /* Read-only access enforced */
#define SECURITY_LOCKED                 0x8     /* Locked for annotations */

  // The types supported by the User-Defined properties
typedef enum _UDTYPES
{
  wUDlpsz    = VT_LPSTR,  // In Office 97 we are actually unicode...
  wUDdate    = VT_FILETIME,
  wUDdw      = VT_I4,
  wUDfloat   = VT_R8,
  wUDbool    = VT_BOOL,
  wUDinvalid = VT_VARIANT        // VT_VARIANT is invalid because it
				 // must always be combined with VT_VECTOR
} UDTYPES;

  // Create a placeholder Vtbl for non-OLE objects.
#define SIVTBLSTRUCT struct _SIVTBLSTRUCT { void FAR *lpVtbl; } SIVTBLSTRUCT

  // For non-OLE objects, first param is pointer to real data.
#define LPSIOBJ LPOFFICESUMINFO

// For more information on the thumbnail look in OLE 2 Programmer's Reference, Volume 1, pp 874-875.

typedef struct tagSINAIL
{
   DWORD cbData;     // size of *pdata
   DWORD cftag;      // either 0,-1,-2,-3, or positive. This decides the size of pFMTID.
   BYTE *pbFMTID;    // bytes representing the FMTID
   BYTE *pbData;     // bytes representing the data
} SINAIL;

typedef SINAIL FAR * LPSINAIL;

// Note about tagSINAIL:
//
// if cftag is
//             0 - pFMTID is NULL i.e. no format name
//            -1 - Windows built-in Clipboard format. pFMTID points to a DWORD (e.g. CF_DIB)
//            -2 - Macintosh Format Value.            pFMTID points to a DWORD
//            -3 - FMTID.                             pFMTID points to 16 bytes
//            >0 - Length of string.                  pFMTID points to cftag bytes
//

  // Summary info data.  Callers should *never* access this data directly,
  // always use the supplied API's.
typedef struct _OFFICESUMINFO {

  SIVTBLSTRUCT;                             // Vtbl goes here for OLE objs,
					    // Must be here for overlays to work!
  BOOL                m_fObjChanged;        // Indicates the object has changed
  LPVOID              m_lpData;             // Pointer to the real data

} OFFICESUMINFO, FAR * LPOFFICESUMINFO;

#ifdef __cplusplus
extern "C" {
#endif // __cplusplus

//
// Indices to pass to API routines to get the specifc data.
//
  // Strings
#define SI_TITLE        0
#define SI_SUBJECT      1
#define SI_AUTHOR       2
#define SI_KEYWORDS     3
#define SI_COMMENTS     4
#define SI_TEMPLATE     5
#define SI_LASTAUTH     6
#define SI_REVISION     7
#define SI_APPNAME      8
#define SI_STRINGLAST   8

  // Times
#define SI_TOTALEDIT    0
#define SI_LASTPRINT    1
#define SI_CREATION     2
#define SI_LASTSAVE     3
#define SI_TIMELAST     3

  // Integer stats
#define SI_PAGES        0
#define SI_WORDS        1
#define SI_CHARS        2
#define SI_SECURITY     3
#define SI_INTLAST      3

//
// Standard I/O routines
//
    // Indicates if the summary info data has changed.
    //
    // Parameters:
    //
    //   lpSIObj - pointer to Summary Info object
    //
    // Return value:
    //
    //   TRUE -- the data has changed, and should be saved.
    //   FALSE -- the data has not changed.
    //

MSOAPI_(BOOL ) MsoFSumInfoShouldSave (LPSIOBJ lpSIObj);

//
// Data manipulation
//
    // Get the size of a given string (UNICODE) property.
    //
    // Parameters:
    //
    //   lpSIObj - pointer to Summary Info object.
    //   iw - specifies which string to get the size of and should be
    //        one of the following values:
    //      SI_TITLE
    //      SI_SUBJECT
    //      SI_AUTHOR
    //      SI_KEYWORDS
    //      SI_COMMENTS
    //      SI_TEMPLATE
    //      SI_LASTAUTH
    //      SI_REVISION
    //      SI_APPNAME
    //
    //   pdw - pointer to a dword, will contain cb on return
    //
    // Return value:
    //
    //   The function returns TRUE on success, FALSE on error.

MSOAPI_(BOOL) MsoFCbSumInfoString
  (LPSIOBJ lpSIObj,                     // Pointer to object
   WORD iw,                             // Index of string to get size of
   DWORD *pdw);                         // Pointer to dword

    // Get a given string (UNICODE) property.
    //
    // Parameters:
    //
    //   lpSIObj - pointer to Summary Info object
    //   wz - buffer to hold string (allocated by caller)
    //   iw - specifies which string to get and should be
    //        one of the following values:
    //      SI_TITLE
    //      SI_SUBJECT
    //      SI_AUTHOR
    //      SI_KEYWORDS
    //      SI_COMMENTS
    //      SI_TEMPLATE
    //      SI_LASTAUTH
    //      SI_REVISION
    //      SI_APPNAME
    //
    //   cbMax - size of buffer
    //
    // Return value:
    //
    //   The function returns TRUE on success, FALSE on error.
    //

MSOAPI_(BOOL) MsoFSumInfoGetString
  (LPSIOBJ lpSIObj,                     // Pointer to object
   WORD iw,                             // Index of string to get
   DWORD cbMax,                         // Size of lpsz
   WCHAR *wz);                        // Pointer to buffer

    // Set a string (UNICODE) property to a given value
    //
    // Parameters:
    //
    //   lpSIObj - pointer to Summary Info object
    //   iw - specifies which string to set and should be
    //        one of the following values:
    //      SI_TITLE
    //      SI_SUBJECT
    //      SI_AUTHOR
    //      SI_KEYWORDS
    //      SI_COMMENTS
    //      SI_TEMPLATE
    //      SI_LASTAUTH
    //      SI_REVISION
    //      SI_APPNAME
    //
    //   wz - buffer containing string value
    //
    // Return value:
    //
    //   The function returns TRUE on success.
    //   The function returns FALSE on error.
    //
    //   If SI_REVISION is passed, the string must point to a whole number.
    //   If not, the function will return FALSE.
    //
    // Note: The function will dirty the object on success.
    //
MSOAPI_(BOOL) MsoFSumInfoSetString
  (LPSIOBJ lpSIObj,                     // Pointer to object
   WORD iw,                             // Index of string to set
   const WCHAR *wz);                          // Pointer to new title


    // Get a given time property.
    //
    // Parameters:
    //
    //   lpSIObj - pointer to a Summary Info object
    //   iw - specifies which time to get and should be
    //        one of the following values:
    //      SI_TOTALEDIT
    //      SI_LASTPRINT
    //      SI_CREATION
    //      SI_LASTSAVE
    //
    //   lpTime - buffer to hold filetime
    //
    // Return value:
    //
    //   The function returns TRUE on succes.
    //   The function returns FALSE on error (bogus argument, or the time
    //   requested doesn't exist - i.e. has not been set, or loaded).
    //
    //  NOTE:    The filetime will be based Coordinated Universal Time (UTC).
    //           This ensures that the time is displayed correctly all over the
    //           world.
    //
    // NOTE: FOR SI_TOTALEDIT lpTime WILL ACTUALLY BE THE TIME
    //       THE FILE HAS BEEN EDITED, NOT A DATE.  THE TIME
    //       WILL BE EXPRESSED IN UNITS OF 100ns.  I KNOW THIS IS
    //       A WEIRD UNIT TO USE, BUT WE HAVE TO DO THAT FOR BACK-
    //       WARDS COMPATABILITY REASONS WITH 16-BIT WORD 6.
    //
    //       OFFICE provides a utility routine to convert a number of
    //       units of 100ns into minutes. Call Convert100nsToMin.
    //
MSOAPI_(BOOL ) MsoFSumInfoGetTime (LPSIOBJ lpSIObj,
					   WORD iw,
					   LPFILETIME lpTime);

    // Set the time property to a given value
    //
    // Parameters:
    //
    //   lpSIObj - pointer to a Summary Info object
    //   iw - specifies which time to set and should be
    //        one of the following values:
    //      SI_TOTALEDIT
    //      SI_LASTPRINT
    //      SI_CREATION
    //      SI_LASTSAVE
    //
    //   lpTime - buffer containing new filetime
    //
    //   NOTE:    The filetime should be based Coordinated Universal Time (UTC).
    //            This ensures that the time is displayed correctly all over the
    //            world.
    //
    // Return value:
    //
    //   The function returns TRUE on succes.
    //   The function returns FALSE on error.
    //
    // Note: The function will dirty the object on success.
    //
    // NOTE: FOR SI_TOTALEDIT lpTime WILL BE INTERPRETED AS THE TIME
    //       THE FILE HAS BEEN EDITED, NOT A DATE.  THE TIME SHOULD
    //       BE EXPRESSED IN UNITS OF 100ns.  I KNOW THIS IS
    //       A WEIRD UNIT TO USE, BUT WE HAVE TO DO THAT FOR BACK-
    //       WARDS COMPATABILITY REASONS WITH 16-BIT WORD 6.
    //
    //       ALSO NOTE THAT THE TIME WILL BE SHOW IN MINUTES IN THE
    //       PROPERTIES DIALOG.
    //
    //       OFFICE provides a utility routine to convert a number of
    //       minutes into units of 100ns. Call ConvertMinTo100ns
    //
MSOAPI_(BOOL ) MsoFSumInfoSetTime (LPSIOBJ lpSIObj, WORD iw, LPFILETIME lpTime);

  // Convert a number of minutes into units of 100ns.
  //
  // Parameters:
  //
  //     lpTime - on intput: contains the number of minutes.
  //     lptime - on output: contains the equivalent number expressed in 100ns.
  //
  // Return value:
  //
  //     None.
  //
MSOAPI_(VOID ) MsoConvertMinTo100ns(LPFILETIME lpTime);

  // Convert a number in units of 100ns into number of minutes.
  //
  // Parameters:
  //
  //     lptime - on input: contains a number expressed in 100ns.
  //              on output: contains the equivalent number of minutes.
  //
  // Return value:
  //
  //     None.
  //
MSOAPI_(VOID ) MsoConvert100nsToMin(LPFILETIME lpTime);

    // Get an integer property
    //
    // Parameters:
    //
    //   lpSIObj - pointer to Summary Info object
    //   iw - specifies which integer to get and should be
    //        one of the following values:
    //      SI_PAGES
    //      SI_WORDS
    //      SI_CHARS
    //      SI_SECURITY
    //
    //   pdw - pointer to a dword, will contain the int on return
    // Return value:
    //
    //   The function returns TRUE on succes, FALSE on error.
MSOAPI_(BOOL ) MsoFDwSumInfoGetInt (LPSIOBJ lpSIObj, WORD iw, DWORD *pdw);

    // Set an integer property to a given value
    //
    // Parameters:
    //
    //   lpSIObj - pointer to Summary Info object
    //   iw - specifies which integer to set and should be
    //        one of the following values:
    //      SI_PAGES
    //      SI_WORDS
    //      SI_CHARS
    //      SI_SECURITY
    //
    //   dw - the value 
    //
    // Return value:
    //
    //   The function returns TRUE on success.
    //   The function returns FALSE on error.
    //
    // Note: The function will dirty the object on success.
    //
MSOAPI_(BOOL ) MsoFSumInfoSetInt (LPSIOBJ lpSIObj, WORD iw, DWORD dw);


    // Get the thumbnail property.
    //
    // Parameters:
    //   lpSIObj - pointer to Summary Info object
    //   lpSINail - will hold the SINAIL information
    //
    // Return value:
    //
    //   The function returns TRUE on success.
    //   The function returns FALSE on error.  The caller should ignore values
    //   set in the SINAIL struct.
    //
    // Note1: The function will allocate memory to hold the data.
    // Note2: lpSINail->cbData can be 0, in which case lpSINail->pData is NULL.
    //        This is legal.
    // Note3: lpSINail->cftag can be 0, in which case lpSINail->pFMTID is NULL.
    //        This is legal.
    //
    // Note4: You must call FreeThumbnailData before freeing the lpSINail you
    //        passed to this function.  You must do this since Office will
    //        allocate the pointers in the structure, so Office must also free
    //        them to avoid memory leaks.
    //
MSOAPI_(BOOL ) MsoFSumInfoGetThumbnail (LPSIOBJ lpSIObj, LPSINAIL lpSINail);

    // Free the data hanging of the SINail struct.
    //
    // Parameters:
    //   lpSINail - pointer to a SINAIL structure.
    //
    // Return Value:
    //   None.
    //
    // Note: This should only be called for Thumbnails obtained through
    //       FSumInfoGetThumbnail.
    //
MSOAPI_(VOID ) MsoFreeThumbnailData (LPSINAIL lpSINail);

    // Set the thumbnail property
    //
    // Parameters:
    //
    //   lpSIObj - pointer to Summary Info object
    //   lpSINail - holds the SINAIL information
    //
    // Return value:
    //
    //   The function returns TRUE on success.
    //   The function returns FALSE on error.
    //
    // Note: The function wil dirty the object on success.
    //
MSOAPI_(BOOL ) MsoFSumInfoSetThumbnail (LPSIOBJ lpSIObj, LPSINAIL lpSINail);

    // Should the thumbnail property be saved
    //
    // Parameters:
    //
    //   lpSIObj - pointer to Summary Info object
    //
    // Return value:
    //
    //   The function returns TRUE on success.
    //   The function returns FALSE is there is no thumbnail.
    //   The function returns FALSE on error.
    //
MSOAPI_(BOOL ) MsoFSumInfoShouldSaveThumbnail (LPSIOBJ lpSIObj);

    // Set the flag deciding whether the thumbnail property should be saved
    //
    // Parameters:
    //
    //   lpSIObj - pointer to Summary Info object
	//	 fSave   - TRUE (should save), FALSE (don't save)
    //
    // Return value:
    //
	//	 None.
    //
MSOAPI_(VOID) MsoSumInfoSetSaveThumbnail (LPSIOBJ lpSIObj, BOOL fSave);

MSOAPI_(BPSC) MsoBpscBulletProofSinfo(MSOBPCB *pmsobpcb, LPSIOBJ *ppsiobj);

#ifdef __cplusplus
}; // extern "C"
#endif // __cplusplus


////////////////////////////////////////////////////////////////////////////////
//
// MS Office Document Summary Information
//
// The Document Summary Information follows the serialized format for
// property sets defined in Appendix B ("OLE Property Sets") of
// "OLE 2 Programmer's Reference, Volume 1"
//
// Notes:
//  - define OLE_PROPS to build OLE 2 interface objects too.
//
// The actual data is stored in DOCSUMINFO.  The layout of the first
// 3 entries must not be changed, since it will be overlayed with
// other structures.  All property exchange data structures have
// this format.
//
// The first parameter of all functions must be LPDSIOBJ in order for these
// functions to work as OLE objects.
//
// All functions defined here have "DocSum" in them.
//
// Several macros are used to hide the stuff that changes in this
// file when it is used to support OLE 2 objects.
// They are:
//   DSIVTBLSTRUCT - For OLE, expands to the pointer to the interface Vtbl
//              - Otherwise, expands to dummy struct same size as Vtbl
//   LPDSIOBJ   - For OLE, expands to a pointer to the interface which is
//                just the lpVtbl portion of the data, to be overlayed later.
//              - Otherwise, expands to a pointer to the whole data
//
////////////////////////////////////////////////////////////////////////////////

  // Create a placeholder Vtbl for non-OLE objects.
#define DSIVTBLSTRUCT struct _DSIVTBLSTRUCT { void FAR *lpVtbl; } DSIVTBLSTRUCT

  // For non-OLE objects, first param is pointer to real data.
#define LPDSIOBJ LPDOCSUMINFO

  // Our object
typedef struct _DOCSUMINFO {

  DSIVTBLSTRUCT;                            // Vtbl goes here for OLE objs,
					    // Must be here for overlays to work!
  BOOL                m_fObjChanged;        // Indicates the object has changed
  LPVOID              m_lpData;             // Pointer to the real data

} DOCSUMINFO, FAR * LPDOCSUMINFO;


#ifdef __cplusplus
extern "C" {
#endif

//
// Indices to pass to API routines to get the specifc data.
//

  // Strings
#define DSI_CATEGORY    0
#define DSI_FORMAT      1
#define DSI_MANAGER     2
#define DSI_COMPANY     3
#define DSI_GUID		4
#define DSI_LINKBASE    5
#define DSI_STRINGLAST  5

  // Integer statistics
#define DSI_BYTES       0
#define DSI_LINES       1
#define DSI_PARAS       2
#define DSI_SLIDES      3
#define DSI_NOTES       4
#define DSI_HIDDENSLIDES 5
#define DSI_MMCLIPS     6
#define DSI_CCHWSPACES	7	// count of characters including spaces
#define DSI_VERSION		8	// Stream format version
#define DSI_INTLAST     8

  // Booleans
#define DSI_SHAREDDOC   	  0
#define DSI_HYPERLINKSCHANGED 1
#define DSI_BOOLLAST    	  1

  // Arrays
#define DSI_HLINKS	0		// Array of hyperlinks
#define DSI_RGLAST  0

  // HLINKS array element
typedef struct _hlinkprop
{
	DWORD dwHash;
	DWORD dwApp;
	DWORD dwEscher;
	DWORD dwInfo;
	WCHAR *wzHlink1;
	WCHAR *wzHlink2;
} HLINKPROP;

/* LiNK Kind -- stored in LOWORD(dwInfo) */
#define lnkkBackground		0	// graphic shown as background of doc
#define lnkkPicture			1	// graphic shown in doc
#define lnkkFill			2	// graphic used to fill a shape
#define lnkkLine			3	// graphic used for shape outline
#define lnkkHlinkShape		4	// hyperlink attached to a shape
#define lnkkHlinkField		5	// hyperlink attached to a (Word) field
#define lnkkHlinkRange		6	// hyperlink attached to an (Excel) range

/* LiNK Action -- stored in HIWORD(dwInfo) */
#define lnkaNil				0	// nothing to do
#define lnkaChange			1	// change link to new wzHlink value
#define lnkaRemove			2	// remove link from object

// VBA Project digital signature info structure
typedef struct _DIGSIGBLOB {
	DWORD	cbData;
	BYTE	*pbData;
} DIGSIGBLOB, *PDIGSIGBLOB;

//
// Given a null terminated string, computes a hash value.
//
// Parameters:
//
//		wz  - pointer to null terminated string
//		pdw - pointer to an integer
//
// Returns:
//
//		The hash value in pdw.
//
// Notes:
//
//	1. The function uses a lower-case version of the string.
//  2. Only the first 255 characters of the string are used.
//
MSOAPI_(VOID) HashWzToInt(const WCHAR *wz, DWORD *pdw);

//
// Standard I/O routines
//

    // Indicates if the Document Summary Infodata has changed.
    //
    // Parameters:
    //
    //   lpDSIObj - pointer to Document Summary Info object
    //
    // Return value:
    //
    //   TRUE -- the data has changed, and should be saved.
    //   FALSE -- the data has not changed.
    //
MSOAPI_(BOOL ) MsoFDocSumShouldSave (LPDSIOBJ lpDSIObj);

//
// Data manipulation routines
//

    // Get the size of a given string (UNICODE) property.
    //
    // Parameters:
    //
    //   lpDSIObj - pointer to Document Summary Info object.
    //   iw - specifies which string to get the size of and should be
    //        one of the following values:
    //      DSI_CATEGORY
    //      DSI_FORMAT
    //      DSI_MANAGER
    //      DSI_COMPANY
    //
    //   pdw - pointer to a dword, will contain the cb on return
    // Return value:
    //
	// 		True on success, false otherwise
	//
MSOAPI_(BOOL)  MsoFCbDocSumString(LPDSIOBJ lpDSIObj, WORD iw, DWORD *pdw);

    // Get a given string (UNICODE) property.
    //
    // Parameters:
    //
    //   lpDSIObj - pointer to Document Summary Info object
    //   iw - specifies which string to set and should be
    //        one of the following values:
    //      DSI_CATEGORY
    //      DSI_FORMAT
    //      DSI_MANAGER
    //      DSI_COMPANY
    //
    //   wz - buffer to hold string (allocated by caller)
    //   cbMax - size of buffer
    //
    // Return value:
    //
	//	True on success, False otherwise
    //
MSOAPI_(BOOL) MsoFDocSumGetString (LPDSIOBJ lpDSIObj,
				       WORD iw,
				       DWORD cbMax,
				       WCHAR *wz);

    // Set a string (UNICODE) property to a given value
    //
    // Parameters:
    //
    //   lpDSIObj - pointer to Document Summary Info object
    //   iw - specifies which string to set and should be
    //        one of the following values:
    //      DSI_CATEGORY
    //      DSI_FORMAT
    //      DSI_MANAGER
    //      DSI_COMPANY
    //
    //   wz - buffer containing string value
    //
    // Return value:
    //
    //   The function returns TRUE on success.
    //   The function returns FALSE on error.
    //
    // Note: The function will dirty the object on success.
    //
MSOAPI_(BOOL ) MsoFDocSumSetString(LPDSIOBJ lpDSIObj, WORD iw, const WCHAR *wz);

  //
  // How Heading and Document parts work:
  //
  // Heading:
  // --------
  // Heading is a list of non-indented headings that will be
  // displayed in the "Contents" ply.
  //
  // Associated with each Heading is the number of document parts
  // that goes with the particular heading -- this is the concept of a
  // Heading Pair.
  //
  // Document Parts:
  // ---------------
  // Document Parts is a list of parts associated with a heading.
  //
  // Example (as it could be implemented in Microsoft Excel):
  // ----------------------------------------------
  // Worksheets
  //     Sheet1
  //     Sheet2
  // Modules
  //     Module1                             Figure 1
  // Charts
  //     Chart1
  //     Chart2
  //     Chart3
  //
  // Thus the Heading Pairs would be:
  //
  // Heading Pair
  //    string                           count
  //------------------------------------
  // Worksheets            2
  // Modules               1                 Figure 2
  // Charts                3
  //
  //
  // And the Document Parts would be:
  //
  // Document Parts
  //--------------------------
  // Sheet1
  // Sheet2
  // Module1
  // Chart1                                  Figure 3
  // Chart2
  // Chart3
  //
  //
  // Note: Headings and Document Parts are not restricted to be parts of
  //       a document, but can be whatever the client wants.  Car models,
  //       car makes, customers, etc...
  //
  //       The above is just an example.
  //

    // Determine how many Document Parts there are total.
    //
    // Parameters:
    //
    //   lpDSIObj - pointer to Document Summary Info object
    //   pdw      - pointer to dword, will contain the count on return
    //
    // Return value:
    //
    //   The function returns TRUE on success, FALSE on error.
    //
MSOAPIX_(BOOL ) MsoFCDocSumDocParts (LPDSIOBJ lpDSIObj, DWORD *pdw);

    // Determine how many Document Parts there are for a given heading.
    //
    // Parameters:
    //
    //   lpDSIObj    - pointer to Document Summary Info object
    //   idwHeading  - 1-based index of Heading
    //   wzHeading   - name of Heading (UNICODE)
    //   pdw         - pointer to dword, will contain the count on return
    //
    //   If wzHeading is non-null, this value will be used to look up
    //   the heading. Otherwise idwHeading will be used.
    //
    // Return value:
    //
    //   The function returns TRUE on success, FALSE on error.
    //
MSOAPI_(BOOL ) MsoFCDocSumDocPartsByHeading(LPDSIOBJ lpDSIObj,
						       DWORD idwHeading,
						       const WCHAR *wzHeading,
						       DWORD *pdw);

    // Determine the size of a specific (one) Document Part
    // for a given heading.
    //
    // Parameters:
    //
    //   lpDSIObj    - pointer to Document Summary Info object.
    //   idwPart     - 1-based index of Document part.
    //   idwHeading  - 1-based index of Heading
    //   wzHeading   - name of Heading (UNICODE)
    //   pdw         - pointer to dword, will contain cb
    //
    //   If wzHeading is non-null, this value will be used to look up
    //   the heading. Otherwise idwHeading will be used.
    //
    // Return value:
    //
    //   The function returns TRUE on success, FALSE on error
    //   (including non-existing Heading).
    //
MSOAPIX_(BOOL ) MsoFCbDocSumDocPart(LPDSIOBJ lpDSIObj,
					      DWORD idwPart,
					      DWORD idwHeading,
					      const WCHAR *wzHeading,
					      DWORD *pdw);

    // Get one of the Document Parts for a given Heading.
    //
    // Parameters:
    //
    //   lpDSIObj    - pointer to Document Summary Info object
    //   idwPart     - 1-based index of Document part
    //   idwHeading  - 1-based index of Heading
    //   wzHeading   - name of Heading (UNICODE)
    //   cbMax       -  number of bytes in wz
    //   wz          -  buffer to hold Document part (allocated by caller)
    //
    //   If wzHeading is non-null, this value will be used to look up
    //   the heading. Otherwise idwHeading will be used.
    //
    // Return value:
    //
    //   The function returns lpsz on success.
    //   The function returns NULL on errors.
    //
MSOAPI_(BOOL) MsoFDocSumGetDocPart(LPDSIOBJ lpDSIObj,
						   DWORD idwPart,
						   DWORD idwHeading,
						   const WCHAR *wzHeading,
						   DWORD cbMax,
						   WCHAR *wz);

    // Set one (existing) Document Part by heading
    //
    // Parameters:
    //
    //   lpDSIObj    - pointer to Document Summary Info object
    //   idwPart     - 1-based index of Document part
    //   idwHeading  - 1-based index of Heading
    //   wzHeading   - name of Heading
    //   wz          - buffer containing new Document Part
    //
    //   If wzHeading is non-null, this value will be used to look up
    //   the heading. Otherwise idwHeading will be used.
    //
    // Return value:
    //
    //   The function returns TRUE on success.
    //   The function returns FALSE on error.
    //
MSOAPI_(BOOL ) MsoFDocSumSetDocPart(LPDSIOBJ lpDSIObj,
					       DWORD idwPart,
					       DWORD idwHeading,
					       const WCHAR *wzHeading,
					       const WCHAR *wz);

    // Remove one (existing) Document Part by heading.
    //
    // Parameters:
    //
    //   lpDSIObj    - pointer to Document Summary Info object
    //   idwPart     - 1-based index of Document part
    //   idwHeading  - 1-based index of Heading
    //   wzHeading   - name of Heading
    //
    //   If wzHeading is non-null, this value will be used to look up
    //   the heading. Otherwise idwHeading will be used.
    //
    // Return value:
    //
    //   The function returns TRUE on success.
    //   The function returns FALSE on error.
    //
    // Note: The count for the Heading will be adjusted on success.
    //
MSOAPIX_(BOOL ) MsoFDocSumDeleteDocPart(LPDSIOBJ lpDSIObj,
						  DWORD idwPart,
						  DWORD idwHeading,
						  const WCHAR *wzHeading);

    // Insert a Document Part at the given location for a given Heading.
    //
    // Parameters:
    //
    //   lpDSIObj    - pointer to Document Summary Info object
    //   idwPart     - 1-based index of Document part to insert at
    //                   1 <= idwPart <= FCDocSumDocPartsByHeading(...)+1
    //                   idwPart = FCDocSumDocPartsByHeading(...)+1 will append a Document Part
    //   idwHeading  - 1-based index of Heading
    //   wzHeading   - name of Heading
    //   wz          - buffer containing new Document Part
    //
    //   If wzHeading is non-null, this value will be used to look up
    //   the heading. Otherwise idwHeading will be used.
    //
    // Note: If the Heading doesn't exist, the heading will be created and inserted
    //       at idwHeaing.
    //       1 <= idwHeading <= FCDocSumHeadingPairs(..)+1
    //       idwHeading = FCDocSumHeadingPairs(...)+1 will append a Heading Pair
    //
    //       In this case wzHeading should contain the heading name.
    //       idwPart will be ignored, and the docpart will be added as the first docpart
    //       for the heading.
    //
    // Return value:
    //
    //   The function returns TRUE on success.
    //   The function returns FALSE on error.
    //
    // Note: The count for the Heading will be adjusted on success.
    //
MSOAPI_(BOOL ) MsoFDocSumInsertDocPart(LPDSIOBJ lpDSIObj,
						  DWORD idwPart,
						  DWORD idwHeading,
						  const WCHAR *wzHeading,
						  const WCHAR *wz);

    // Determine how many Heading Pairs there are.
    //
    // Parameters:
    //
    //   lpDSIObj - pointer to Document Summary Info object
    //   pdw      - pointer to dword, will contain count
    //
    // Return value:
    //
    //   The function returns TRUE on success, FALSE on error.
    //
MSOAPI_(BOOL ) MsoFCDocSumHeadingPairs (LPDSIOBJ lpDSIObj, DWORD *pdw);

    // Get the size of one heading string
    //
    // Parameters:
    //
    //   lpDSIObj    - pointer to Document Summary Info object.
    //   idwHeading  - 1-based index of heading
    //   pdw         - pointer to dword, will contain cb
    //
    // Return value:
    //
    //   The function returns TRUE on success, FALSE on error.
    //
MSOAPIX_(BOOL ) MsoFCbDocSumHeadingPair (LPDSIOBJ lpDSIObj,
						  DWORD idwHeading,
						  DWORD *pdw);

    // Get one heading pair.
    //
    // Parameters:
    //
    //   lpDSIObj    - pointer to Document Summary Info object
    //   idwheading  - 1-based index of heading pair
    //   wzHeading   - name of Heading
    //   cbMax       - number of bytes in lpsz
    //   wz          - buffer to hold heading string (allocated by user)
    //   pdwcParts   - will be set to number of document parts for the heading
    //
    //   If wzHeading is non-null, this value will be used to look up
    //   the heading (could be only dwcParts is wanted).
    //   Otherwise idwHeading will be used.
    //
    // Return value:
    //
    //   The function will return TRUE on success.
    //   The function will return FALSE on error.
    //
MSOAPIX_(BOOL) MsoFDocSumGetHeadingPair(LPDSIOBJ lpDSIObj,
					    DWORD idwHeading,
					    const WCHAR *wzHeading,
					    DWORD cbMax,
					    WCHAR *wz,
					    DWORD *pdwcParts);

// #ifdef UNUSED
// Used in Binder
    // Set one heading pair
    //
    // Parameters:
    //
    //   lpDSIObj    - pointer to Document Summary Info object
    //   idwheading  - 1-based index of heading pair
    //   wzHeading   - name of Heading
    //   wz          - buffer containing heading string
    //
    //   If wzHeading is non-null, this value will be used to look up
    //   the heading (could be only dwcParts should be set).
    //   Otherwise idwHeading will be used.
    //
    // Return value:
    //
    //   The function will return TRUE on success.
    //   The function will return FALSE on error.
    //
MSOAPIX_(BOOL ) MsoFDocSumSetHeadingPair(LPDSIOBJ lpDSIObj,
						   DWORD idwHeading,
						   const WCHAR *wzHeading,
						   const WCHAR *wz);
//#endif // UNUSED

    // Delete a heading pair
    //
    // Note:  This will also delete ALL document parts associated
    //        with the heading.
    //
    // Parameters:
    //
    //   lpDSIObj    - pointer to Document Summary Info object
    //   idwheading  - 1-based index of heading pair
    //   wzHeading - name of Heading
    //
    //   If wzHeading is non-null, this value will be used to look up
    //   the heading.  Otherwise idwHeading will be used.
    //
    // Return value:
    //
    //   The function will return TRUE on success.
    //   The function will return FALSE on error.
    //
MSOAPI_(BOOL ) MsoFDocSumDeleteHeadingPair(LPDSIOBJ lpDSIObj,
						      DWORD idwHeading,
						      const WCHAR *wzHeading);

    // Delete all heading pair and all their document parts. I.e.
    // clear the contents data.
    //
    // Parameters:
    //
    //   lpDSIObj    - pointer to Document Summary Info object
    //
    // Return value:
    //
    //   The function will return TRUE on success.
    //   The function will return FALSE on error.
    //
MSOAPI_(BOOL ) MsoFDocSumDeleteAllHeadingPair (LPDSIOBJ lpDSIObj);

    // Insert a heading pair at the given location
    //
    // Parameters:
    //
    //   lpDSIObj          - pointer to Document Summary Info object
    //   idwHeading        - 1-based index of Heading pair to insert at
    //                        1 <= idwHeading <= FCDocSumHeadingPairs(..)+1
    //                        idwHeading = FCDocSumHeadingPairs(...)+1 will append a Heading Pair
    //   wzHeadingBefore   - name of a Heading
    //   wzNewHeading      - buffer containing new Heading string
    //
    //   If wzHeadingBefore is non-null, the new Heading will be inserted right before
    //   wzHeadingBefore.  To insert at the end of the list pass NULL for this parameter,
    //   and set idwHeading = cDocSumHeadingPairs+1.
    //
    // Return value:
    //
    //   The function returns TRUE on success.
    //   The function returns FALSE on error.
    //
MSOAPI_(BOOL ) MsoFDocSumInsertHeadingPair(LPDSIOBJ lpDSIObj,
					   DWORD idwHeading,
					   const WCHAR *wzHeadingBefore,
					   const WCHAR *wzNewHeading);

    // Get an integer property
    //
    // Parameters:
    //
    //   lpDSIObj - pointer to Document Summary Info object
    //   iw - specifies which integer to get and should be
    //        one of the following values:
    //      DSI_BYTES
    //      DSI_LINES
    //      DSI_PARAS
    //      DSI_SLIDES
    //      DSI_NOTES
    //      DSI_HIDDENSLIDES
    //      DSI_MMCLIPS
	//		DSI_CCHWSPACES
	//		DSI_VERSION
    //
    //   pdw - pointer to dword, will contain integer
    //
    // Return value:
    //
    //   The function returns TRUE on success, FALSE on error
	//
	//	Note: If you ask for DSI_VERSION and the function returns false
	//		  that means (assuming all parameters are valid) that the property
	//		  wasn't in the stream, and as such the property data was originally 
	//		  in the Office 95 property format.
	//		
MSOAPI_(BOOL ) MsoFDwDocSumGetInt (LPDSIOBJ lpDSIObj, WORD iw, DWORD *pdw);

    // Set an integer property to a given value
    //
    // Parameters:
    //
    //   lpDSIObj - pointer to Document Summary Info object
    //   iw - specifies which integer to set and should be
    //        one of the following values:
    //      DSI_BYTES
    //      DSI_LINES
    //      DSI_PARAS
    //      DSI_SLIDES
    //      DSI_NOTES
    //      DSI_HIDDENSLIDES
    //      DSI_MMCLIPS
	//		DSI_CCHWSPACES
    //
    //   dw - the value 
    //
    // Return value:
    //
    //   The function returns TRUE on success.
    //   The function returns FALSE on error.
    //
    // Note: The function will dirty the object on success.
    //
	// Note: You cannot set DSI_VERSION
	//	
MSOAPI_(BOOL ) MsoFDocSumSetInt (LPDSIOBJ lpDSIObj, WORD iw, DWORD dw);


// #ifdef UNUSED
// Used in Binder
    // Get the Scalability property
    //
    // Parameters:
    //
    //   lpDSIObj - pointer to Document Summary Info object
    //
    // Return value:
    //
    //   The function returns TRUE when scaling, FALSE when cropping.
    //
    //   The function will also return FALSE on error, i.e. if lpDSIObj is null
    //   or there is no data in the object.
    //
MSOAPIX_(BOOL ) MsoFDocSumGetScalability (LPDSIOBJ lpDSIObj);
//#endif // UNUSED

    // Determine if the object has the Scalable property
    //
    // Parameters:
    //
    //   lpDSIObj - pointer to Document Summary Info object
    //
    // Return value:
    //
    //   The function returns TRUE if scaling, FALSE otherwise.
    //
    //   The function will also return FALSE on error, i.e. if lpDSIObj is null
    //   or there is no data in the object.
    //
MSOAPIX_(BOOL ) MsoFDocSumIsScalable (LPDSIOBJ lpDSIObj);

    // Determine if the object has the Croppable property
    //
    // Parameters:
    //
    //   lpDSIObj - pointer to Document Summary Info object
    //
    // Return value:
    //
    //   The function returns TRUE when cropping, FALSE otherwise.
    //
    //   The function will also return FALSE on error, i.e. if lpDSIObj is null
    //   or there is no data in the object.
    //
MSOAPIX_(BOOL ) MsoFDocSumIsCroppable (LPDSIOBJ lpDSIObj);

    // Set the Scalability property
    //
    // Parameters:
    //
    //   lpDSIObj - pointer to Document Summary Info object
    //   fScalable - should be set to TRUE if setting to scalable,
    //               should be set to FALSE if setting to cropping
    // Return value:
    //
    //   The function returns TRUE on success.
    //   The function returns FALSE on error.
    //
    // Note: The function dirties the object on success.
    //
MSOAPIX_(BOOL ) MsoFDocSumSetScalability (LPDSIOBJ lpDSIObj, BOOL fScalable);

    // Determine if the actual values of the LINKED user defined properties has changed
	 // This function should only be called right after loading the properties to
	 // see if the caller should update the link values.
	 //
	 // NOTE: The function works by checking the value of the PID_LINKSDIRTY property.
	 //       When this function is called the property will be set to FALSE, so that
	 //       flag is cleared next time the properties are saved.
	 //
	 // NOTE: Only the app that created the file that are being loaded should call this
	 //       function.  I.e. Excel calls this for .xls files, noone else does, etc...
     //
     // Parameters:
     //
     //     lpDSIObj - pointer to Document Summary Info object
     //
     // Return value:
     //
     //     The function returns TRUE if the link values have changed.
     //     The function returns FALSE if the link value have not
     //     changed, or on error.
     //
MSOAPI_(BOOL ) MsoFLinkValsChanged(LPDSIOBJ lpDSIObj);

    // Set the DSI_GUID string (UNICODE) property to the value of a newly 
    // created GUID.  Applications should call this function only when saving
    // a document for the first time (such as during File-SaveAs).  
    // This function allocates a new GUID via CoCreateGuid, converts the GUID
    // to a string, and sets the DSI_GUID property to this string. 
    //
    // Parameters:
    //
    //   lpDSIObj - pointer to Document Summary Info object
    //
    // Return value:
    //
    //   The function returns TRUE on success.
    //   The function returns FALSE on error.
    //
    // Note: The function dirties the object on success.
    //
MSOAPI_(BOOL ) MsoFDocSumSetGUID(LPDSIOBJ lpDSIObj);

    // Get a boolean property
    //
    // Parameters:
    //
    //   lpDSIObj - pointer to Document Summary Info object
    //   iw - specifies which boolean to get and should be
    //        one of the following values:
    //      DSI_SHAREDDOC - is the document shared or not
    //		DSI_HYPERLINKSCHANGED - did the hyperlinks change
	//
    //   pf - pointer to BOOL, will contain boolean
    //
    // Return value:
    //
    //   The function returns TRUE on success, FALSE on error
    //
MSOAPI_(BOOL ) MsoFDocSumGetBool (LPDSIOBJ lpDSIObj, WORD iw, BOOL *pf);

    // Set a boolean property 
    //
    // Parameters:
    //
    //   lpDSIObj - pointer to Document Summary Info object
    //   iw - specifies which boolean to set and should be
    //        one of the following values:
    //      DSI_SHAREDDOC - is the document shared or not
    //		DSI_HYPERLINKSCHANGED - did the hyperlinks change
    //
    //   f - the value 
    //
    // Return value:
    //
    //   The function returns TRUE on success.
    //   The function returns FALSE on error.
    //
    // Note: The function will dirty the object on success.
    //
MSOAPI_(BOOL ) MsoFDocSumSetBool (LPDSIOBJ lpDSIObj, WORD iw, BOOL f);


	//
	// Get the count of elements in an array
	//
	// Parameters:
	//
	//		lpDSIObj   - pointer to a Document Summary Info object
	//		iArray     - specifies the array
	//		pcElements - will hold the count of elements, 0 if none
	//
	// Return value:
	//
	//		The function returns TRUE on success (even if count is 0)
	//		The function returns FALSE on error.
	//
	// Notes:
	//	
	// 1. Legal values for iArray are:
	//
	//		DSI_HLINKS
	//
MSOAPI_(BOOL) MsoFDocSumGetArrayElementCount(LPDSIOBJ lpDSIObj, DWORD iArray, DWORD *pcElements);

	//
	// Set the ith element in an array.
	//
	// Parameters:
	//
	//		lpDSIObj   - pointer to a Document Summary Info object
	//		iArray     - specifies the array
	//		iElement   - specifies the element
	//		pvData	   - points to the data (array element)
	//
	// Return value:
	//
	//		The function returns TRUE on success
	//		The function returns FALSE on error.
	//
	// Notes:
	//
	// 1. Legal values for iArray are:
	//
	//		DSI_HLINKS
	//
	// 2. If iElement == -1, the element will be added to the end of the array.
	// 3. iElement is 0-based.
	// 4. You can always set an element at position 0 or -1.  This is equivalent
	//    to creating the array if it doesn't exist.
	// 5. If iElement already exists in the array, the data will be replaced.
	// 6. pvData will be typecast to the correct type of array element.
	//
MSOAPI_(BOOL) MsoFDocSumSetArrayElement(LPDSIOBJ lpDSIObj, DWORD iArray, DWORD iElement, PVOID pvData);

	//
	// Get the ith element in an array.
	//
	// Parameters:
	//
	//		lpDSIObj   - pointer to a Document Summary Info object
	//		iArray     - specifies the array
	//		iElement   - specifies the element
	//		pvData	   - will point to the data (array element)
	//
	// Return value:
	//
	//		The function returns TRUE on success
	//		The function returns FALSE on error.
	//
	// Notes:
	//
	// 1. Legal values for iArray are:
	//
	//		DSI_HLINKS
	//
	// 2. iElement is 0-based.
	// 3. pvData can be typecast to the appropriate type of array element.
	//    The caller should not free any memory hanging off of pvData.
	// 4. It's the caller's resposibility to make sure that pvData points
	//    to the correct data structure.
	//
MSOAPI_(BOOL) MsoFDocSumGetArrayElement(LPDSIOBJ lpDSIObj, DWORD iArray, DWORD iElement, PVOID pvData);

	//
	// Delete the ith element from an array
	//
	// Parameters:
	//
	//		lpDSIObj   - pointer to a Document Summary Info object
	//		iArray     - specifies the array
	//		iElement   - specifies the element
	//
	// Return value:
	//
	//		The function returns TRUE on success
	//		The function returns FALSE on error.
	//
	// Notes:
	//
	// 1. Legal values for iArray are:
	//
	//		DSI_HLINKS
	//
	// 2. iElement is 0-based.
	//
MSOAPI_(BOOL) MsoFDocSumDeleteArrayElement(LPDSIOBJ lpDSIObj, DWORD iArray, DWORD iElement);

	// 
	// Delete the entire array
	//
	// Parameters:
	//
	//		lpDSIObj   - pointer to a Document Summary Info object
	//		iArray     - specifies the array
	//
	// Return value:
	//
	//		The function returns TRUE on success
	//		The function returns FALSE on error.
	//
	// Notes:

	// 1. Legal values for iArray are:
	//
	//		DSI_HLINKS
	//
	// 2. Deleting the last element from the array also deletes the array.
	//
MSOAPI_(BOOL) MsoFDocSumDeleteArray(LPDSIOBJ lpDSIObj, DWORD iArray);

    // Set the digital signature property to a given value
    //
    // Parameters:
    //
    //   lpDSIObj - pointer to Document Summary Info object
    //
    //   pDigSig - the value 
    //
    // Return value:
    //
    //   The function returns TRUE on success.
    //   The function returns FALSE on error.
    //
    // Note: The function will dirty the object on success.
    //
MSOAPI_(BOOL) MsoFDocSumSetDigSig (LPDSIOBJ lpDSIObj, PDIGSIGBLOB pDigSig);

    // Get the digital signature property
    //
    // Parameters:
    //
    //   lpDSIObj - pointer to Document Summary Info object
    //
    //   pDigSig - the value 
    //
    // Return value:
    //
    //   The function returns TRUE on success.
    //   The function returns FALSE on error.
    //
MSOAPI_(BOOL) MsoFDocSumGetDigSig (LPDSIOBJ lpDSIObj, PDIGSIGBLOB *ppDigSig);

MSOAPI_(BPSC) MsoBpscBulletProofDsinfo(MSOBPCB *pmsobpcb, LPDSIOBJ *ppdsiobj);

#ifdef __cplusplus
}; // extern "C"
#endif


////////////////////////////////////////////////////////////////////////////////
//
// MS Office User Defined Property Information
//
// The User Defined Property Information follows the serialized format for
// property sets defined in Appendix B ("OLE Property Sets") of
// "OLE 2 Programmer's Reference, Volume 1"
//
// Notes:
//  - define OLE_PROPS to build OLE 2 interface objects too.
//
// The actual data is stored in USERPROP.  The layout of the first
// 3 entries must not be changed, since it will be overlayed with
// other structures.  All property exchange data structures have
// this format.
//
// The first parameter of all functions must be LPUDOBJ in order for these
// functions to work as OLE objects.
//
// All functions defined here have "UserDef" in them.
//
// Several macros are used to hide the stuff that changes in this
// file when it is used to support OLE 2 objects.
// They are:
//   UDPVTBLSTRUCT - For OLE, expands to the pointer to the interface Vtbl
//              - Otherwise, expands to dummy struct same size as Vtbl
//   LPUDOBJ    - For OLE, expands to a pointer to the interface which is
//                just the lpVtbl portion of the data, to be overlayed later.
//              - Otherwise, expands to a pointer to the whole data
//
////////////////////////////////////////////////////////////////////////////////

  // Create a placeholder Vtbl for non-OLE objects.
#define UDPVTBLSTRUCT struct _UDPVTBLSTRUCT { void FAR *lpVtbl; } UDPVTBLSTRUCT

  // For non-OLE objects, first param is pointer to real data.
#define LPUDOBJ LPUSERPROP

  // User-defined property data.  Callers should *never* access this
  // data directly, always use the supplied API's.

typedef struct _USERPROP {

  UDPVTBLSTRUCT;                            // Vtbl goes here for OLE objs,
					    // Must be here for overlays to work!
  BOOL                m_fObjChanged;        // Indicates the object has changed
  LPVOID              m_lpData;             // Pointer to the real data

} USERPROP, FAR * LPUSERPROP;


//
// Interface API's for User Property Information.
//
#ifdef __cplusplus
extern "C" {
#endif

//
// Standard I/O routines
//
    // Indicates if the data has changed, meaning a write is needed.
MSOAPI_(BOOL ) MsoFUserDefShouldSave (LPUDOBJ lpUDObj);

//
// Routines to query and modify data.
//
  //
  // How User-defined properties work:
  //
  // See the OLE Property Exchange spec for full details.
  //
  // Each User-defined type has a string "Name" and integer Property Id
  // value associated with it.  The Property Id's are sequential, but
  // are only good for the current object in memory (i.e. you can't count
  // on the Property Id value remaining the same between loads of the
  // data.  The string will remain the same, if it has not been changed
  // or deleted.)
  // Currently, the User-defined types can have 5 types for the value:
  // String, Date, Integer, float and boolean.  When setting and getting the values, you
  // must make sure that the type stored matches what you expect to
  // retreive.  For Int's, the LPVOID should be the int itself, not
  // a pointer.  In all other cases, the LPVOID should point to a buffer
  // of appropriate size for the type.
  //

  // Masks used for querying property data.  Note that these are
  // mutually exclusive.
#define UD_STATIC       0x00
#define UD_LINK         0x01
#define UD_IMONIKER     0x10

    // Determine the number of user-defined properties for the object.
    // Returns -1 on error
MSOAPI_(BOOL ) MsoFCUserDefNumProps (LPUDOBJ lpUDObj, DWORD *pdw);

    // Determine the size of the Property Value for the given Property string
    // Note that for types other that wUDlpsz, this will return the size
    // of the structure that holds the data.
    // dwMask is used to specify whether the cb is for the static value
    //   or for the link or IMoniker name.  For Links & IMonikers,
    //   the type is wUDlpsz.
    // pcb - will hold the cb
    // Returns FALSE on error, TRUE on success
	//
	// Note that all strings are UNICODE
	//
MSOAPI_(BOOL) MsoFCbUserDefPropVal
  (LPUDOBJ lpUDObj,             // Pointer to object
   const WCHAR *wz,                   // Pointer to string
   DWORD dwMask,                // Mask telling what value to get cb for
   DWORD *pdw);                 // Pointer to dword

    // Returns the type of the given Property Value from the string
    // Returns wUDInvalid on error
	// 
MSOAPI_(UDTYPES) MsoUdtypesUserDefType(LPUDOBJ lpUDObj, const WCHAR *wz);

    // This will return the Property Value for the given Property string.
    // lpszProp is the property string
    // lpv is a buffer to hold the value, of size cbMax.
    // pfLink tells if the value is a link,
    // pfIMoniker tells if the value is a moniker.
    // pfLinkInvalid tells if the link is invalid
    // dwMask is used to specify whether the value returned is the
    //  static value, link name or IMoniker name.
	//
    // Function returns NULL on error.
	//
    // WARNING! Be very careful calling this.  Be sure that the
    // buffer and return value match the type for the Property Value!
	//

MSOAPI_(LPVOID) MsoLpvoidUserDefGetPropVal
  (LPUDOBJ lpUDObj,             // Pointer to object
   const WCHAR *wzProp,               // Property string
   DWORD cbMax,                 // Size of lpv
   LPVOID lpv,                  // Buffer for prop val
   DWORD dwMask,                // Mask for what value is needed
   BOOL *pfLink,                // Indicates a link
   BOOL *pfIMoniker,            // Indicates an IMoniker
   BOOL *pfLinkInvalid);        // Is the link invalid

    // Set the value of a given property to a new value.
    // Be careful when setting properties that are linked - be sure
    // that the type the iterator is set to matches what the link is to.
    // If udtype == wUDinvalid, the type of the iterator will not change,
    // the value will be assumed to be the current type.
    //
	// fLinkInvalid : If the link is no longer valid, set this flag to true.
 	//                A special icon will displayed in the listview and the last
	//                known value and type will be used.  Thus the values passed
	//                to this function will be ignored in this case.
	//
	//                If fLinkInvalid is true, but the iterator is not a link,
	//                the function will return FALSE
    //
    //                If fLinkInvalid is true the value will _not_ be changed.
	//
	// NOTE: If udtype == wUDDate you can set the value to 0 (not NULL)
	//       This will be interpreted as an invalid date and the date will
	//              be displayed as the empty string in the list box.
MSOAPI_( BOOL ) MsoFUserDefChangeVal
  (LPUDOBJ lpUDObj,                     // Pointer to object
   const WCHAR *wzProp,                       // Property string
   UDTYPES udtype,                      // Type of new value
   LPVOID lpv,                          // New value.
   BOOL fLinkInvalid);				    // Is the link still valid?

    // Set the string (i.e. the name) for the given Property String (lpszOld) 
    // to the new string (lpszNew).
MSOAPIX_(BOOL) MsoFUserDefSetPropString
  (LPUDOBJ lpUDObj,             // Pointer to object
   const WCHAR *wzOld,                // Old prop string
   const WCHAR *wzNew);               // New prop string

//
// Routines to create and remove data from the Property Set.
//

    // This will add a new Property to the set, with the given
    // Property string.  This function can also be used to modify
    // an existing property.
    //
    // lpUDObj      - pointer to the UD properties
    // wzPropName   - name of property to be added/modified
    // lpvVal       - value of the property
    // udtype       - value type
    // wzLinkMonik  - name of the link/moniker
    // fLink        - true if the property is a link
    // fIMoniker    - true if the property is an imoniker
    // fHidden      - true if the property is hidden
    //
    // NOTE: fLink and fIMoniker cannot be true at the same time.  If
    //       so, the property will not be added and the function will
    //       return FALSE.
    //
    //
    // NOTE: If udtype == wUDbool, lpv must point to a DWORD, but the
    //       HIWORD must be 0.
    //
    // WARNING: Be sure that the type matches what the lpv really is!
    //
    // The caller is responsible for freeing any memory
    // associated with a property value after it is added to the
    // User-defined Property object.
    //
	// NOTE: If udtype == wUDDate you can set the value to 0 (not NULL)
	//       This will be interpreted as an invalid date and the date will
	//              be displayed as the empty string in the list box.
    //
    // The function returns TRUE if the property was succesfully added,
    // FALSE otherwise.
    //
MSOAPI_(BOOL) MsoFUserDefAddProp
  (LPUDOBJ lpUDObj,             // Pointer to object
   const WCHAR *wzPropName,           // Property string
   LPVOID lpv,                  // Property value
   UDTYPES udtype,              // Property type
   const WCHAR *wzLinkMonik,          // The link/imoniker name
   BOOL fLink,                  // Indicates the property is a link
   BOOL fHidden,                // Indicates the property is hidden
   BOOL fIMoniker);             // Indicates the property is a moniker.

    // This will delete a Property from the set given a Property string.
MSOAPI_(BOOL ) MsoFUserDefDeleteProp (LPUDOBJ lpUDObj, const WCHAR *wz);

//
// Routines to iterate through the User-defined properties
//
// Notes: Adding and deleting elements invalidates the iterator.
//
    // An iterator for User-defined Properties
  typedef struct _UDITER FAR * LPUDITER;

    // Create a User-defined Properties iterator
MSOAPI_(LPUDITER ) MsoLpudiUserDefCreateIterator (LPUDOBJ lpUDObj);

    // Destroy a User-defined Properties iterator
MSOAPI_(BOOL ) MsoFUserDefDestroyIterator (LPUDITER *lplpUDIter);

    // Determine if an iterator is still valid
MSOAPI_(BOOL ) MsoFUserDefIteratorValid (LPUDITER lpUDIter);

    // Iterate to the next element
	 // Returns TRUE if we could get to the next element, FALSE otherwise.
MSOAPI_(BOOL ) MsoFUserDefIteratorNext (LPUDITER lpUDIter);

    // Returns true if the iterator is a link, false otherwise
MSOAPI_(BOOL) MsoFUserDefIteratorIsLink (LPUDITER lpUDIter);

    // Returns true if the iterator is an invalid link, returns false if the
    // iterator is not a link or if the iterator is a valid link
MSOAPI_(BOOL) MsoFUserDefIteratorIsLinkInvalid (LPUDITER lpUDIter);

    // Determine the size of the Property Value for the given iterator
    // Note that for types other that UDlpsz, this will return the size
    // of the structure that holds the data.
    // dwMask is used to specify whether the cb is for the static value
    //   or for the link or IMoniker name.  For Links & IMonikers,
    //   the type is wUDlpsz.
    // Returns 0 on error
MSOAPI_(BOOL ) MsoFCbUserDefIteratorVal (LPUDITER lpUDIter, DWORD dwMask, DWORD *pcb);

    // Returns the type of the given Property Value from the iterator
    // Returns wUDInvalid on error
MSOAPI_(UDTYPES ) MsoUdtypesUserDefIteratorType (LPUDITER lpUDIter);

    // This will return the Property Value for the given iterator
    // lpv is a buffer to hold the value, of size cbMax.
    // dwMask is used to specify whether the value returned is the
    //  static value, link name or IMoniker name.
    // pfLink tells if the value is a link,
    // pfIMoniker tells if the value is a moniker.
	 // pfLinkInvalid tells if the link is invalid.
    // Function returns NULL on error.
    // WARNING! Be very careful calling this.  Be sure that the
    // buffer and return value match the type for the Property Value!
	// 
	// Note that strings are UNICODE
	//
MSOAPI_(LPVOID ) MsoLpvoidUserDefGetIteratorVal (LPUDITER lpUDIter,
						DWORD cbMax,
						LPVOID lpv,
						DWORD dwMask,
						BOOL *pfLink,
						BOOL *pfIMoniker,
						BOOL *pfLinkInvalid);

    // Set the value of the iterator item to a new value.
    // Be careful when setting properties that are linked - be sure
    // that the type the iterator is set to matches what the link is to.
    // If udtype == wUDinvalid, the type of the iterator will not change,
    // the value will be assumed to be the current type.
    //
	// fLinkInvalid : If the link is no longer valid, set this flag to true.
	//                A special icon will displayed in the listview and the last
	//                known value and type will be used.  Thus the values passed
	//                to this function will be ignored in this case.
	//
	//                If fLinkInvalid is true, but the iterator is not a link,
	//                the function will return FALSE
    //
    //                If fLinkInvalid is true the value will _not_ be changed.
    //
    //                If fLinkInvalid is false, the value _will_ be changed.
	//
	// NOTE: If udtype == wUDDate you can set the value to 0 (not NULL)
	//       This will be interpreted as an invalid date and the date will
	//              be displayed as the empty string in the list box.

MSOAPI_(BOOL ) MsoFUserDefIteratorChangeVal (LPUDOBJ lpUDObj,
						   LPUDITER lpUDIter,
						   UDTYPES udtype,
						   LPVOID lpv,
						   BOOL fLinkInvalid);

    // This will return the size of the Property string for the property
MSOAPI_(BOOL ) MsoFCbUserDefIteratorName (LPUDITER lpUDIter, DWORD *pcb);

    // This will return the Property String (name) for the property
MSOAPI_(BOOL) MsoFUserDefIteratorName
  (LPUDITER lpUDIter,                   // Pointer to iterator
   DWORD cbMax,                         // Max size of lpsz
   WCHAR *wz);                          // Buffer to copy into

    // Set the string (i.e. property name) for the given Property String (lpszOld) 
    // to the new string (lpszNew).
MSOAPIX_( BOOL ) MsoFUserDefIteratorSetPropString
  (LPUDOBJ lpUDObj,                     // Pointer to object
   LPUDITER lpUDIter,                   // Pointer to iterator
   const WCHAR *wzNew);                  // Pointer to new name

//
// Misc. utility routines
//

  // Routines dealing with hidden Properties.

    // Determine if a Property string is hidden.
MSOAPI_(BOOL) MsoFUserDefIsHidden
  (LPUDOBJ lpUDObj,             // Pointer to object
   const WCHAR *wz);                  // Property string

    // Make a property visible based on the Property string
MSOAPIX_(BOOL) MsoFUserDefMakeVisible
  (LPUDOBJ lpUDObj,             // Pointer to object
   const WCHAR *wz);                  // String to show.

    // Hide a Property based on the Property string.
MSOAPIX_(BOOL) MsoFUserDefMakeHidden
  (LPUDOBJ lpUDObj,             // Pointer to object
   const WCHAR *wz);                  // String to hide

MSOAPI_(BPSC) MsoBpscBulletProofUdinfo
  (MSOBPCB *pmsobpcb,
  LPUDOBJ *ppudobj);

#ifdef __cplusplus
}; // extern "C"
#endif

#ifdef __cplusplus
extern "C" {
#endif

  // Commands for DWQUERYLD
#define QLD_CLINKS      1  /* Return the number of links */
#define QLD_LINKNAME    2  /* Return a pointer to the string for index */
#define QLD_LINKTYPE    3  /* Returns the type of the value of the index */
#define QLD_LINKVAL     4  /* Return value for the index, use same
						      rules as for LPVOIDs in UserDef functions */

  // This functions should respond to the above commands by returning the
  // appropriate value.  For commands that require an index, the
  // wzName parameter will be the Name of the link item previously
  // retrieved from the index, if it is not NULL.
  // lplpvBuf is the buffer supplied by "us" (the dll) to copy the
  // value to.  Use the function LpvOfficeCopyValToBuffer() to 
  // copy the data.  This parameter will be NULL for QLD_CLINKS and
  // QLD_VALTYPE
typedef DWORD_PTR (OFC_CALLBACK *DWQUERYLD)(DWORD dwCommand, DWORD_PTR dwi, LPVOID *lplpvBuf, WCHAR *wzName);


    // Copies the given data to the given buffer.  Pointer to the
    // buffer is returned.
    // lpvVal - Value to copy into buffer
    // udtype - Type for the value
    // lplpvBuf - Buffer to copy into
	//
MSOAPI_(LPVOID ) MsoLpvOfficeCopyValToBuffer (LPVOID lpvVal,
				 							  UDTYPES udtype,
						    				  LPVOID *lplpvBuf);

  // Masks for different options
#define OSPD_ALLOWLINKS         0x01   // The Custom dialog will allow fields to be linked if this is set.
#define OSPD_NOSAVEPREVIEW      0x02   // Don't show the Save Preview Picture checkbox
#define OSPD_SAVEPREVIEW_ON     0x04   // Save Preview Picture should be on by default
#define OSPD_CAPTIONFORFILENAME 0x08   // Display the caption as the filename too.
#define OSPD_NOLASTACCESSED     0x10   // Don't display the Last Accessed field on the Statistics tab.
#define OSPD_NOLASTPRINT        0x20   // Don't display the Last Print field on the Statistics tab.
#define OSPD_READONLY           0x40   // Display the dialog in read-only mode

    // LPUDObj is a pointer to a pointer to a user-defined property object.
    // If *lplpUDObj == NULL, an object will be created by the dialog as needed.
    // Note that the object will use the same malloc & free routines as
    // the lpSIObj uses.
    //
    // wzFileName is the fully qualified name of the storage as it appears
    // in the filesystem.  This can be NULL if no file exists.
    //
    // dwMask contains either 0 or a set of valid flags for various options.
    //
    // LPFN_DWQLD is a callback, that when given a dwCommand of 0
    // returns the number of links, and for any other number 0 < NumLinks,
    // places the link data & static value in the lpld buffer and returns non-0
    // if the function succeeded.
    //
    // The storage for the buffer is to be allocated by the app, and a pointer
    // to that storage passed back.
    //
    // pptCtr - POINT struct filled with the coordinates of the center 
    //          of the dialog.  Used to make sure we are using sticky
    //          dialog coordinates.  If pPoint->x == -1, we ignore and use
    //          the default position for the dialog.
    //
    //          pptCtr will be filled with the coordinates of the new position
    //          of the dialog on returning.
    //
    //          The coordinates should be in client area coordinates, i.e. in
    //          hWndParent coordinates.
    //
    // wzCaption - caption for the dialog.  This should be the filename as it is
    //             displayed in the apps document title bar.
    //             The properties dialog caption will be as follows:
    //
    //               <foo> Properties
    //
    //               where foo is the string pointed to by wzCaption.
    //
	// wzFileType - Override for the file type field on the General tab.
	//              If NULL is passed in, the system will determine the file type.
	//
    // The function returns TRUE on success, FALSE on error or if the user hit Cancel.
    //
    // Note: It's the caller's resposibility to invalidate any links (if appropriate)
    //       before calling this function.
    //
    // Note: If lpfnDwQueryLinkData is NULL, the caller must invalidate any linked properties.
    //
MSOAPI_(BOOL ) MsoFOfficeShowPropDlg (HWND hWndParent,
				     				  const WCHAR *wzFileName,
				     				  LPSIOBJ lpSIObj,
				     				  LPDSIOBJ lpDSIObj,
				     				  LPUDOBJ FAR *lplpUDObj,
					      			  DWORD dwMask,
				     				  DWQUERYLD lpfnDwQueryLinkData,
				     				  LPPOINT pptCtr,
				     				  const WCHAR *wzCaption,
				     				  const WCHAR *wzFileType);


/*-----------------------------------------------------------------------------
	MsoFOfficeShowPropDlgEx

	Same as MsoFOfficeShowPropDlg with addition of the "signatures" tab if
	pissc is not NULL

	new params:
	[in] pissc --> signature set client implemented by client app
	[in] pvSignatureClient --> client specific data
	[in] fSigOnly --> TRUE if only want to show the signatrue tab, set
	                  when calling this API through sigature toolbar button.
	                  reuse the code whenever we can (not elegant design, but
	                  cheap in terms of dev time)
-------------------------------------------------------------------- HAILIU -*/
MSOAPI_(BOOL) MsoFOfficeShowPropDlgEx(HWND hWndParent,
				     				  const WCHAR *wzFileName,
				     				  LPSIOBJ lpSIObj,
				     				  LPDSIOBJ lpDSIObj,
				     				  LPUDOBJ FAR *lplpUDObj,
					      			  DWORD dwMask,
				     				  DWQUERYLD lpfnDwQueryLinkData,
				     				  LPPOINT pptCtr,
				     				  const WCHAR *wzCaption,
				     				  const WCHAR *wzFileType,
									  IMsoSignatureSetClient *pissc,
									  LPVOID pvSignatureClient,
									  BOOL fSigOnly
				     				  );
				     				  
    // Creates and initializes all non-NULL objects.
    // Create the object and return it.  Caller responsible for destruction.
    //
    // rglpfn is an array, with the following callbacks supplied by the user:
    //
    //  Code Page Conversion
    //
    //  rglpfn[ifnCPConvert] = (BOOL) (OFC_CALLBACK *lpfnFCPConvert) (LPSTR lpsz,
    //                                                  DWORD dwFrom,
    //                                                  DWORD dwTo,
    //                                                  BOOL fMacintosh)
    //    lpsz is a 0 terminated C string, dwFrom is the code page
    //    lpsz is currently stored as, dwTo is the code page it should
    //    be converted to, fMacintosh indicates whether dwFrom is a Mac
    //    or Windows code page identifier.
	//
	//	  For the MAC, the following code page indentifiers are used for dwTo.
	//	  (Mac code pages (10000+script ids))
	//
	//	  To determine if dwTo is Mac or Win, use MsoFMacCp defined below.
	//
#define msocpidMac		(10000)			/* Mac, smRoman */
#define msocpidMacSJIS ((10000+1))		/* Mac, smJapanese */
#define msocpidMacBIG5 ((10000+2))		/* Mac, smTradChinese */
#define msocpidMacKSC  ((10000+3))		/* Mac, smKorean */
#define msocpidMArab	((10000+4))		/* Mac, smArabic */
#define msocpidMHebr	((10000+5))		/* Mac, smHebrew */
#define msocpidMGreek	((10000+6))		/* Mac, smGreek */
#define msocpidMCyril	((10000+7))		/* Mac, smCyrillic */
#define msocpidMacPRC  ((10000+25))		/* Mac, smSimpChinese */
#define msocpidMSlavic	((10000+29))		/* Mac, smEastEurRoman */
#define msocpidMIce    ((10000+64+15))	/* Mac, smRoman,langIcelandic */
#define msocpidMTurk   ((10000+64+17))	/* Mac, smRoman,langTurkish */
#define msocpidMacGB2312	((10000+31))		/* Mac, smChinese */

#define msocpidMacLast	((10000+64+256))	/* highest Mac msocpid (just a guess) */

#define MsoFMacCp(cp) ((cp) >= msocpidMac && (cp) <= msocpidMacLast)

    //
    //  Convert an sz to a double
    //
    //  rglpfn[ifnFWzToNum] = (BOOL) (OFC_CALLBACK *lpfnFWzToNum)(
    //                                   double *lpdbl,
    //                                   LPSTR lpszNum)
    //
    //   lpdbl - pointer to a double, this is set by the app
    //   lpszNum - zero-terminated string representing the number
    //
    //  Convert a double to an sz
    //
    //  rglpfn[ifnFNumToWz] = (BOOL) (OFC_CALLBACK *lpfnFNumToWz)(
    //                                   double *lpdbl,
    //                                   LPSTR lpszNum,
    //                                   DWORD cbMax)
    //   lpdbl   - pointer to a double
    //   lpszNum - on return a zero-terminated string representing the number
    //   cbMax   - Max number of bytes in lpszNum
    //
    //   Update the statistics on the Statistics tab
    //
    //   rglpfn[ifnFUpdateStats] = (BOOL) (OFC_CALLBACK *lpfnFUpdateStats)(
    //                                       HWND hwndParent,
    //                                       LPSIOBJ lpSIObj,
    //                                       LPDSIOBJ lpDSIObj)
    //
    //      hwndParent - window of the properties dialog, so that the app
    //                   can put up an alert, letting the user know the the
    //                   data is being updated.
    //
    //      lpSIObj, lpDSIObj - objects to update
    //
    //   Note:  If the app does not want to set the statistics before bringing up
    //          the dialog, they can provide this callback function.  If the
    //          function pointer is not NULL, the function will be called the first
    //          time the user clicks on the Statistics tab.  The app should then update
    //          all appropriate statistics for the tab and return TRUE on success, FALSE
    //          on failure.  If the function pointer is NULL, the existing data will be
    //          used.
    //
    //  Note:
    //         Only rglpfn[ifnCPConvert] must be non-NULL.  If it is NULL, the
    //         function will return FALSE, and the objects will not be created.
    //
    //         rglpfn[ifnFWzToNum] and rglpfn[ifnFNumToWz] must either both be
    //         non-NULL, or NULL.  Otherwise, the function will return FALSE, and
    //         the objects will not be created.  If both functions are NULL, there
    //         will be no floating point support in OLE Extended Properties (i.e. on
    //         the Custom tab), but integers will be supported.
	//
	//  Note:  hinst must be non-NULL.
    //
MSOAPI_(BOOL ) MsoFOfficeCreateAndInitObjects (LPSIOBJ *lplpSIObj,
										     LPDSIOBJ *lplpDSIObj,
										     LPUDOBJ *lplpUDObj,
										     void *prglpfn[],
										     HMSOINST hinst);

    // Clear any non-null objects
MSOAPI_(BOOL ) MsoFOfficeClearObjects (LPSIOBJ lpSIObj,
								     LPDSIOBJ lpDSIObj,
								     LPUDOBJ lpUDObj);

    // Destroy any non-null objects
MSOAPI_(BOOL ) MsoFOfficeDestroyObjects (LPSIOBJ *lplpSIObj,
								       LPDSIOBJ *lplpDSIObj,
					    			   LPUDOBJ *lplpUDObj);

  // Use these functions to set the dirty flag of the given object.
  // Note: It's the caller's responsibility to make sure that the
  //       object is non-NULL
MSOAPI_(VOID ) MsoOfficeDirtySIObj(LPSIOBJ lpSIObj, BOOL fDirty);

MSOAPI_(VOID ) MsoOfficeDirtyDSIObj(LPDSIOBJ lpDSIObj, BOOL fDirty);

MSOAPI_(VOID ) MsoOfficeDirtyUDObj(LPUDOBJ lpUDObj, BOOL fDirty);


// Flags for Load & Save
#define OIO_ANSI                0x0001 // The storage is an ANSI storage (UNICODE is the default)
#define OIO_SAVEIFCHANGEONLY    0x0002 // Only streams that are dirty should be saved.
#define OIO_SAVESIMPLEDOCFILE   0x0004 // The storage is a simple DOC file.
#define OIO_SAVE_AS_95          0x0008 // Save the properties in the 95 format
#define OIO_SAVE_AS_UNICODE     0x0010 // Save the properties in UNICODE format. 
                                       // Not supported in 97!!!
#define OIO_LINKSCHANGED        0x0020 // Added for Outlook.  Force the links changed flag to be TRUE
#define OIO_SAVE_AS_HTML        0x0040 // Save the properties in HTML format
#define OIO_SAVE_ONLY_VBASIG    0x0080 // When the doc is encrypted, only save VBA signature unencrypted
                                       // this flag used internally, client app should not use it.
#define OIO_SAVE_TITLE_XML      0x0100 // Save the title prop in the xml collection (if OIO_SAVE_AS_HTML set)
#define OIO_SKIP_VBASIG         0x0200 // Skip the SAVE_ONLY_VBASIG logic, client will do a separate step
#define OIO_SAVE_LINKBASE_XML   0x0400 // Save the hyperlink base prop in the xml collection (if OIO_SAVE_AS_HTML set)

    // Populate the objects with data.  lpStg is the root stream.
    // Returns the number of streams loaded.
    // dwFlags: OIO_ANSI specifies that lpStg is an ANSI storage (UNICODE is the default)
    //
    // The function returns the following:
    //
#define MSO_IO_ERROR   0     // The stream(s) were found, but the load failed
#define MSO_IO_NOSTM   1     // The stream(s) were not found
#define MSO_IO_SUCCESS 2     // The stream(s) were found, and the load succeeded
    //
    // NOTE: The caller can load either the summary info stream (lpSIObj != NULL), or
    //       the Document Summary Info stream (lpDSIObj != NULL && lpUDObj != NULL) or
    //       both.
    //
    // NOTE: If the caller asks to load both streams, MSO_IO_NOSTM will not be returned, as
    //       long as one of the streams exists.

MSOAPI_(DWORD ) MsoDwOfficeLoadProperties (LPSTORAGE lpStg,
						 LPSIOBJ lpSIObj,
						 LPDSIOBJ lpDSIObj,
						 LPUDOBJ lpUDObj,
						 DWORD dwFlags);


/*-----------------------------------------------------------------------------
	MsoDwOfficeLoadPropertiesEx

	same as MsoDwOfficeLoadProperties, but with the option for encryption
	MsoDwOfficeLoadProperties calls MsoDwOfficeLoadPropertiesEx
-------------------------------------------------------------------- HAILIU -*/
MSOAPI_(DWORD ) MsoDwOfficeLoadPropertiesEx (LPSTORAGE lpStg,
						 LPSIOBJ lpSIObj,
						 LPDSIOBJ lpDSIObj,
						 LPUDOBJ lpUDObj,
						 IMsoCryptSession *pics,
						 DWORD dwFlags);


MSOAPIX_(DWORD ) MsoDwOfficeLoadIntProperties (LPSTORAGE lpStg,
						 LPSIOBJ lpSIObj,
						 LPDSIOBJ lpDSIObj,
						 LPUDOBJ lpUDObj,

						 DWORD dwFlags);
//
// Do a normal load (like calling MsoDwOfficeLoadProperties),
// but also return the codepage.
//
// Used by the Office Compatible dudes...
//
MSOAPIMX_(DWORD)  MsoDwLoadPropertiesCodePage(LPSTORAGE lpStg,
						 LPSIOBJ lpSIObj,
						 LPDSIOBJ lpDSIObj,
						 LPUDOBJ lpUDObj,
						 DWORD dwFlags,
						 ULONG *pdwCodePage);

    // Write the data in the given objects.
    // pvContent is either the root stream or IMsoHtmlExport.
    // Returns the number of streams saved.
    // dwFlags: OIO_ANSI specifies that root stream is an ANSI storage (UNICODE is the default)
    //
    //          OIO_SAVEIFCHANGEONLY specificies that only streams that are
    //           "dirty" will be saved.  Do NOT specify this if you are 
    //           saving to a tmp file.  Also do not attempt to "outsmart"
    //           the save by passing NULL objects, use this flag instead.
    //
    //          OIO_SAVESIMPLEDOCFILE specifies that the storage is a simple DOC file.
	//
	//			OIO_SAVE_AS_HTML specifies that pvContent is an IMsoHtmlExport and
	//			 we will be exporting as html.  If not set, then pvContent is an
	//			 LPSTORAGE and we will be saving as binary.
    //
MSOAPI_(DWORD ) MsoDwOfficeSaveProperties (LPVOID pvContent,
						 LPSIOBJ lpSIObj,
						 LPDSIOBJ lpDSIObj,
						 LPUDOBJ lpUDObj,
						 DWORD dwFlags);


/*-----------------------------------------------------------------------------
	MsoDwOfficeSavePropertiesEx

	Same as MsoDwOfficeSaveProperties but with option for encryption
	MsoDwOfficeSaveProperties now calls MsoDwOfficeSavePropertiesEx with NULL for pics
-------------------------------------------------------------------- HAILIU -*/
MSOAPI_( DWORD ) MsoDwOfficeSavePropertiesEx
  (LPVOID pvContent,							 // Pointer to root storage or pihe
   LPSIOBJ lpSIObj,                     // Pointer to Summary Obj
   LPDSIOBJ lpDSIObj,                   // Pointer to Document Summary obj
   LPUDOBJ lpUDObj,                     // Pointer to User-defined Obj
   IMsoCryptSession *pics,              // non=NULL, if the property should be encrypted
   DWORD dwFlags);                      // Flags
   
 
/*-----------------------------------------------------------------------------
	MsoDwOfficeSaveVBASig
 
 	A lot like MsoDwOfficeSaveProperties, but only saves the VBASig property
 	in the stream.  Gives the client a chance to close down the flattening stream
 	during encryption before creating the property stream.
------------------------------------------------------------------- MarkWal -*/
MSOAPI_(DWORD) MsoDwOfficeSaveVBASig
	(LPVOID pvContent,
	 LPDSIOBJ lpdsiobj,
	 DWORD dwFlags);

#ifdef VSMSODEBUG

/*-----------------------------------------------------------------------------
	MsoFWriteOlePropBe

	Account for memory allocated for the OLE Properties objects.

	Will only check non-NULL paramters (lpSIObj, lpDSIObj, lpUDObj)

------------------------------------------------------------------ MARTINTH -*/
MSOAPI_(BOOL) MsoFWriteOlePropBe(LPSIOBJ lpSIObj, LPDSIOBJ lpDSIObj, LPUDOBJ lpUDObj);

#endif // VSMSODEBUG

////////////////////////////////////////////////////
// VB support routines - see spec for details.
////////////////////////////////////////////////////

//
//  Used by Office Compatible.  Don't unexport.
// 

// Converts a FileTime to a VariantDate
//
MSOAPIX_(BOOL) FFtToVariantDate(LPFILETIME lpft, LPVARIANTARG lpvarg);
MSOAPIX_(BOOL) FVariantDateToSt(LPVARIANTARG lpvarg, LPSYSTEMTIME lpst);

// Converts a VariantDate to a FileTime.
//
MSOAPIX_(BOOL) FVariantDateToFt(LPVARIANTARG lpvarg, LPFILETIME lpft);

    // Creates a Builtin property collection and returns it.
    // pParent is the parent IDispatch object.
    // The new IDispatch object is returned via pvarg.
MSOAPI_(BOOL ) MsoFGetBuiltinPropCollection (LCID lcid,
						   LPSIOBJ lpSIObj,
						   LPDSIOBJ lpDSIObj,
						   IDispatch *pParent,
						   VARIANT *pvarg,
						   HMSOINST hinst);

    // Creates a Custom property collection and returns it.
    // pParent is the parent IDispatch object.
    // The new IDispatch object is returned via pvarg.
MSOAPI_(BOOL ) MsoFGetCustomPropCollection (LCID lcid,
						  LPUDOBJ lpUDObj,
						  IDispatch *pParent,
						  VARIANT *pvarg,
						  HMSOINST hinst);

/*-----------------------------------------------------------------------------
	MsoFRemovePrivateProperties

	Clears properties with private information
-------------------------------------------------------------------- JORGEF -*/
MSOAPI_(BOOL) MsoFRemovePrivateProperties(LPSIOBJ lpSIObj, LPDSIOBJ lpDSIObj, LPUDOBJ lpUDObj);

#ifdef __cplusplus
}; // extern "C"
#endif

#pragma pack( pop, msoprops )
#endif
