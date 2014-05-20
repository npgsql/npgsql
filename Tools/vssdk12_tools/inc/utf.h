// utf.h
//-----------------------------------------------------------------
// Microsoft Confidential
// Copyright 1998 Microsoft Corporation.  All Rights Reserved.
//
// June 1, 1998 [paulde]  Revised for UniLib, surrogates
//
// Routines are documented in more detail below the declarations in 
// the "-- DOCUMENTATION --" section.
//
//-----------------------------------------------------------------
#ifdef _MSC_VER
#pragma once
#endif

#ifndef __UTF_H__
#define __UTF_H__

// See "About the UTF-8 file signature" below for information and usage.
// EF BB BF
#define UTF8SIG     "\xEF\xBB\xBF" 
#define UTF8SIGLEN  (3)

// U8TU_* flags returned by UTF8ToUnicode[Info]
#define U8TU_NONASCII        0x00000001  // Info : found non-ASCII chars
#define U8TU_UCS4            0x00000002  // Info : found UCS-4 (4-byte) chars. The Unicode data contains surrogates.
#define U8TU_OVERLONG        0x00000004  // Info:  found UTF-8 sequence longer than required: char converted to UCH_REPLACE
#define U8TU_TRAIL_NO_COUNT  0x80000100  // Error: trail byte with 0 trail count
#define U8TU_COUNT_NO_TRAIL  0x80000200  // Error: nonzero trail count but no trail byte
#define U8TU_UCS4OUTOFRANGE  0x80000400  // Error: UCS4 char is out of range to represent in Unicode

// Test a U8TU_... value for errors
#define U8TU_IsError(dw)    (0 != ((dw) & 0x80000000))

// NOTE: UTF-8 encoders are supposed to use the shortest possible sequence to represent a character.
// Overlong sequences are either a bug in the UTF-8 encoder, or an attempt to use overlong data
// as a covert channel or to circumvent security. Overlong sequences generate UCH_REPLACE instead of 
// the character, and we flag U8TU_OVERLONG, but this is not an error.
//

// VU16_* flags returned by ValidateUTF16
#define VU16_NONASCII        0x00000001  // Info : found non-ASCII chars
#define VU16_UCS4            0x00000002  // Info : found non-BMP chars. The Unicode data contains surrogates.
#define VU16_NONCHAR         0x00000004  // Info : found noncharacters (U+xFFFE and U+xFFFF, where x is from 0 to 0x10,
                                         //        as well as the values U+FDD0..U+FDEF)
#define VU16_UNPAIRSURROGATE 0x80000100  // Error: high surrogate without a low surrogate, or vice versa

// Test a VU16_... value for errors
#define VU16_IsError(dw)    (0 != ((dw) & 0x80000000))


#define NULL_TERMINATED_MODE          (-1L)

// !!!!!!!!!!!!!!!!!!!!!!!!!! WARNING !!!!!!!!!!!!!!!!!!!!!!!!!!
//
// DO NOT pass NULL output buffers to get the required buffer size. 
//
// To get the length of output buffers, use the explicit length functions:
// UnicodeLengthOfUTF8, UTF8LengthOfUnicode.
//

// "just convert it as fast as possible"
int  WINAPI  UTF8ToUTF16         ( /* __in_xcount(cbUTF) */ PCSTR pUTF8, int cbUTF, __out_ecount(cchUTF16) PWSTR pUTF16, int cchUTF16);

// "convert it and tell me all about it"
int  WINAPI  UTF8ToUTF16Info     ( /* __in_xcount(*pcbUTF8) */ PCSTR pUTF8, int * pcbUTF8, __out_ecount(cchUTF16) PWSTR pUTF16, int cchUTF16, DWORD * pdwInfo);

// "tell me ALL about it"
// when fScanAll is FALSE, errors immediately terminate scanning the data 
int  WINAPI  GetUTF8Info         ( /* __in_xcount(*pcbUTF8) */ PCSTR pUTF8, int * pcbUTF8, DWORD * pdwInfo, BOOL fScanAll = FALSE);

// "just tell me how many wchars I'll need, as fast as possible"
int  WINAPI  UTF16LengthOfUTF8   ( /* __in_xcount(cbUTF8) */ PCSTR pUTF8, int cbUTF8);

int  WINAPI  UTF16ToUTF8         ( /* __in_xcount(*pcchUTF16) */ PCWSTR pUTF16, int * pcchUTF16, __out_ecount(cbUTF8) PSTR pUTF8, int cbUTF8);

// "tell me exactly how many chars I need to convert"
int  WINAPI  UTF8LengthOfUTF16   ( /* __in_xcount(cchUTF16) */ PCWSTR pUTF16, int cchUTF16);

// "tell me about any errors in this UTF-16 text"
DWORD WINAPI ValidateUTF16       (PCWSTR pUTF16, int cchUTF16, BOOL fScanAll = FALSE);


//================= COMPATIBILITY WRAPPERS ========================
//
// UTF-16 is the more precise term, but there is existing code that
// uses "Unicode" to refer to UTF-16.

inline int WINAPI UTF8ToUnicode( /* __in_xcount(cbUTF) */ PCSTR pUTF8, int cbUTF, __out_ecount(cchUni) PWSTR pUni, int cchUni)
{
 return UTF8ToUTF16(pUTF8, cbUTF, pUni, cchUni);
}

inline int WINAPI UTF8ToUnicodeInfo( /* __in_xcount(cbUTF) */PCSTR pUTF8, int * pcbUTF8, __out_ecount(cchUni) PWSTR pUni, int cchUni, DWORD * pdwInfo)
{
  return UTF8ToUTF16Info(pUTF8, pcbUTF8, pUni, cchUni, pdwInfo);
}

inline int WINAPI UnicodeLengthOfUTF8( /* __in_xcount(cbUTF8) */ PCSTR pUTF8, int cbUTF)
{
  return UTF16LengthOfUTF8(pUTF8, cbUTF);
}

inline int WINAPI UnicodeToUTF8(  /* __in_ecount(*pcchUni) */ PCWSTR pUni, int * pcchUni, __out_ecount(cbUTF) PSTR pUTF8, int cbUTF)
{
  return UTF16ToUTF8(pUni, pcchUni, pUTF8, cbUTF);
}

inline int WINAPI UTF8LengthOfUnicode(PCWSTR pUni, int cchUni)
{
  return UTF8LengthOfUTF16( /* __in_xcount(cchUni) */ pUni, cchUni);
}


//===================== DOCUMENTATION =============================

//-----------------------------------------------------------------
//
// int UTF8ToUTF16Info (PCSTR pUTF8, int * pcbUTF8, PWSTR pUTF16, int cchUTF16, DWORD * pdwInfo);
//
// Convert UTF8 to UTF-16
//
// pUTF8      UTF-8 data
// pcbUTF8    IN : Count of UTF-8 bytes to convert, or NULL_TERMINATED_MODE.
//            OUT: Count of UTF-8 bytes converted.
// pUTF16     Buffer for converted UTF-16 text
// cchUTF16   Size of UTF-16 buffer in WCHARs.
// pdwInfo    NULL or address of flags for errors/information.
//            See U8TU_* flags above for more info. 
//
// Return:
//   Count of 16-bit code units written, including 0 terminator 
//   if NULL_TERMINATED_MODE.
//
// The conversion always completes, even in the presence of errors. *pdwInfo contains 
// status and error information. When there are conversion errors, the Unicode buffer 
// may contain one or more of character UCH_REPLACE (0xFFFD) "REPLACEMENT CHARACTER" 
// for un-convertible data.
//

//-----------------------------------------------------------------
// int GetUTF8Info (PCSTR pUTF8, int * pcbUTF8, DWORD * pdwInfo, BOOL fScanAll = FALSE);
//
// Get size and optional information/errors for conversion of UTF-8 to UTF-16.
//
// pUTF8      UTF-8 data
// pcbUTF8    IN : Count of UTF-8 bytes to scan, or NULL_TERMINATED_MODE.
//            OUT: Count of valid UTF-8 scanned.
// pdwInfo    Information and errors in the conversion.
//            See U8TU_* flags above for more info. 
// fScanAll   TRUE : scan entire UTF-8 data
//            FALSE: stop scanning at the first error and return
// Return:
//   Count of 16-bit code units required to represent the characters scanned, 
//   including 0 terminator if NULL_TERMINATED_MODE
//

//-----------------------------------------------------------------
// DWORD ValidateUTF16 (PCWSTR pUTF16, int cchUTF16, BOOL fScanAll = FALSE)
//
// Get size and optional information/errors for conversion of UTF-8 to UTF-16.
//
// pUTF16     UTF-16 data
// cchUTF16   Count of 16-bit code units to scan, or NULL_TERMINATED_MODE.
// fScanAll   TRUE : scan entire text
//            FALSE: stop scanning at the first error and return
// Return:
//   Information and errors in the text -- See VU16_* flags above for more info. 
//

//-----------------------------------------------------------------
//
// int UTF16ToUTF8 (PCWSTR pUTF16, int * pcchUTF16, PSTR pUTF8, int cbUTF8)
//
// Convert UTF-16 to UTF-8
//
// pUTF16    Source UTF-16 data
// pcchUTF16 in:  Count of 16-bit code units to convert, or NULL_TERMINATED_MODE.
//           out: count of 16-bit code units converted.
// pUTF8     Destination buffer.
// cbUTF8    Count of bytes in pUTF8.
//
// Returns:
//    Number of bytes written, including 0 terminator if NULL_TERMINATED_MODE.
//
//

//-----------------------------------------------------------------
// int UTF8LengthOfUTF16 (PCWSTR pUTF16, int cchUTF16)
//
// Get exact number of bytes required to convert 
//

//-----------------------------------------------------------------
// About the UTF-8 file signature
// -------------------------------
// The UTF-8 file signature is the UTF-8 encoding of the Unicode byte order mark.
// the byte order mark is generally used as a signature for Unicode files.
// 
// When writing plain-text UTF-8 files, begin the file with this signature.
// 
// When reading plain-text files:
// 
//   If you see the UTF-8 signature, you can assume that the file is UTF-8.
// 
//   If there is no UTF-8 signature, you can try converting. If the conversion
//   has no errors, you can be fairly sure that the file is UTF-8 and not
//   an MBCS plain-text file. This cannot be 100% positive, however. It is 
//   possible to construct MBCS files in various codepages that also happen to 
//   be valid UTF-8.
// 
//-----------------------------------------------------------------

#endif // __UTF_H__
