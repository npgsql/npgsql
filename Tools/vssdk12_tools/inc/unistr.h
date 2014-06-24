// unistr.h
//-----------------------------------------------------------------
// Microsoft Confidential
// Copyright 1998 Microsoft Corporation.  All Rights Reserved.
//
// June 1, 1998 [paulde]
//
//-----------------------------------------------------------------
#ifdef _MSC_VER
#pragma once
#endif

#ifndef __UNISTR_H__
#define __UNISTR_H__

#ifdef _MSC_VER
#if !defined(_M_CEE)
#pragma intrinsic(strlen)
#endif
#endif

// Replacement for _snwprintf, etc., that performs correct zero termination
int __cdecl BufPrint(_Out_cap_(cchBuf) LPWSTR pwszBuf, size_t cchBuf, _In_z_ LPCWSTR pwszFormat, ...);
int __cdecl BufPrintArgs(DWORD dwFlags, _Out_z_cap_(cchBuf) LPWSTR pwszBuf, size_t cchBuf, _In_z_ LPCWSTR pwszFormat, va_list argList);

// Replacement for _snwprintf, etc., that performs correct zero termination
int __cdecl BufPrintA( _Out_cap_(cchBuf) LPSTR pszBuf, size_t cchBuf, _In_z_ LPCSTR pszFormat, ...);
int __cdecl BufPrintArgsA(DWORD dwFlags, _Out_cap_(cchBuf) LPSTR pszBuf, size_t cchBuf, _In_z_ LPCSTR pszFormat, va_list argList);



int     WINAPI   StrLen          ( _In_opt_z_ PCWSTR psz);
int     WINAPI   StrLenA         ( _In_opt_z_ PCSTR  psz);


//Note that all of the functions below are somewhat unsafe

// Copy strings for catenation.  Returns the next position for appending.

//These methods are all unsafe and should be deprecated.
PWSTR   WINAPI   CopyCat         ( _Pre_notnull_ _Post_z_ PWSTR dst, _In_z_ PCWSTR src);
PSTR    WINAPI   CopyCatA        ( _Pre_notnull_ _Post_z_ PSTR  dst, _In_z_ PCSTR  src);

// The 'N' versions guarantee zero termination, but can write one more than cchz.
PWSTR   WINAPI   CopyNCat        ( _Out_z_cap_(cchz+1) PWSTR dst, _In_z_ PCWSTR src, int cchz);  
PSTR    WINAPI   CopyNCatA       ( _Out_z_cap_(cchz+1) PSTR  dst, _In_z_ PCSTR  src, int cchz);

PWSTR   WINAPI   CopyCatInt      ( _Pre_notnull_ _Post_z_ PWSTR dst, int n, int radix);
PSTR    WINAPI   CopyCatIntA     ( _Pre_notnull_ _Post_z_ PSTR  dst, int n, int radix);

//Instead of using the functions above, you should use the various equivalent methods from strsafe.h
//So, instead of:
//      WCHAR*  p = sz;
//
//      p = CopyCat   (p, szSrc1);
//      p = CopyCat   (p, szSrc2);
//      p = CopyNCat  (p, szSrc3, iLenSrc3);
//      p = CopyCatInt(p, n, base);
//      CopyCat(p, szSrc4);
//
//You should do the following:
//      WCHAR*  p = sz;
//      size_t  cchP = _countof(sz);
//      HRESULT hr;
//
//      hr = StringCchCopyExW (p, cchP, szSrc1, &p, &cchP, 0);
//      hr = StringCchCopyExW (p, cchP, szSrc2, &p, &cchP, 0);
//      hr = StringCchCopyNExW(p, cchP, szSrc3, iLenSrc3, &p, &cchP, 0);
//      hr = StringCchIntExW  (p, cchP, n, base, &p, &cchP, 0);
//      hr = StringCchCopyW   (p, cchP, szSrc4);

//As above, but copy the integer represented in the given base rather than the source string
//Note that these functions -- though they follow the StringCchCopyEx semantics -- are implemented in unistr.cpp
HRESULT StringCchIntExW  (_Out_cap_(cchDst) wchar_t*  dst, size_t cchDst,
                          int n, unsigned int radix,
                          _Out_opt_ _Deref_post_count_(*pcchDstEnd) wchar_t** ppDstEnd, _Out_opt_cap_(1) size_t* pcchDstEnd, unsigned long dwFlags);
HRESULT StringCchIntExA  (_Out_cap_(cchDst) char*    dst, size_t cchDst,
                          int n, unsigned int radix,
                          _Out_opt_ _Deref_post_count_(*pcchDstEnd) char**    ppDstEnd, _Out_opt_cap_(1) size_t* pcchDstEnd, unsigned long dwFlags);

//----------------------------------------------------------------
// NextChar, PrevChar - Walk text by logical characters
// These routines honor 'grapheme' boundaries. They are aware of things like 
// combining characters (e.g. diacritics), surrogate pairs, Hangul syllables 
// formed by Hangul combining Jamo.
//
// They return NULL after reaching the bound of the string or buffer.
//
PCWSTR  WINAPI   NextChar ( _In_opt_z_ PCWSTR pchPoint);                // NUL terminated
PCWSTR  WINAPI   NextChar ( _In_opt_z_ PCWSTR pchPoint, _In_opt_z_ PCWSTR pchEnd); // length-bounded
PCWSTR  WINAPI   PrevChar ( _In_ PCWSTR pchStart, _In_ PCWSTR pchPoint);

// VS7:32377 non-const variants, like CRT
inline PWSTR WINAPI NextChar ( _In_opt_z_ PWSTR pchPoint                ) {return const_cast<PWSTR>(NextChar (const_cast<PCWSTR>(pchPoint)));}
inline PWSTR WINAPI NextChar ( _In_opt_z_ PWSTR pchPoint, _In_opt_z_ PWSTR pchEnd  ) {return const_cast<PWSTR>(NextChar (const_cast<PCWSTR>(pchPoint), const_cast<PCWSTR>(pchEnd)));}
inline PWSTR WINAPI PrevChar ( _In_ PWSTR pchStart, _In_ PWSTR pchPoint) {return const_cast<PWSTR>(PrevChar (const_cast<PCWSTR>(pchStart), const_cast<PCWSTR>(pchPoint)));}

// Returns true if pchPoint is at a grapheme boundary
bool    WINAPI   IsGraphemeBreak ( _In_z_ PCWSTR pchStart, _In_count_(1) PCWSTR pchPoint);

//----------------------------------------------------------------
// See docs below
//#define wcsnpbrk FindCharInSet
PCWSTR  WINAPI   FindCharInSet         (_In_opt_count_(cchBuffer) PCWSTR pchBuffer, int cchBuffer, _In_opt_ PCWSTR set);
PCWSTR  WINAPI   FindCharInOrderedSet  (_In_count_(cchBuffer) PCWSTR pchBuffer, int cchBuffer, _In_z_ PCWSTR set);
PCWSTR  WINAPI   FindChar              (_In_opt_z_ PCWSTR psz, WCHAR ch);
PCWSTR  WINAPI   FindCharN             (_In_opt_count_(cch) PCWSTR pchBuffer, int cch, WCHAR ch);
PCWSTR  WINAPI   FindLastChar          (_In_z_ PCWSTR psz, WCHAR ch);
PCSTR   WINAPI   FindLastCharA         (_In_z_ PCSTR  psz, CHAR  ch);
PCWSTR  WINAPI   CharInOrderedSet      (WCHAR ch, _In_opt_z_ PCWSTR set);

// VS7:32377  non-const variants, like CRT
inline PWSTR WINAPI FindCharInSet         (_In_opt_ PWSTR pchBuffer, int cchBuffer, PCWSTR set) {return const_cast<PWSTR>(FindCharInSet (const_cast<PCWSTR>(pchBuffer), cchBuffer, set));}
inline PWSTR WINAPI FindCharInOrderedSet  (_In_opt_ PWSTR pchBuffer, int cchBuffer, PCWSTR set) {return const_cast<PWSTR>(FindCharInOrderedSet (const_cast<PCWSTR>(pchBuffer), cchBuffer, set));}
inline PWSTR WINAPI FindChar              (_In_opt_ PWSTR psz, WCHAR ch) {return const_cast<PWSTR>(FindChar (const_cast<PCWSTR>(psz), ch));}
inline PWSTR WINAPI FindCharN             (_In_opt_ PWSTR pchBuffer, int cch, WCHAR ch) {return const_cast<PWSTR>(FindCharN (const_cast<PCWSTR>(pchBuffer), cch, ch));}
inline PWSTR WINAPI FindLastChar          (_In_opt_ PWSTR psz, WCHAR ch) {return const_cast<PWSTR>(FindLastChar (const_cast<PCWSTR>(psz), ch));}
inline PSTR  WINAPI FindLastCharA         (_In_opt_ PSTR  psz, CHAR  ch) {return const_cast<PSTR>(FindLastCharA (const_cast<PCSTR>(psz), ch));}

//----------------------------------------------------------------
// PathSplit  - Split a unc/path/filename into it's elements
//
// Forward/back slashes are normalized before splitting.
//
// Argument  Description    Required Size   Comments
// --------  ------------   -------------   -----------------------------------------
// pszFN     Source path    n/a
// pszV      Drive or UNC   MAX_PATH        e.g. "C:" or "\\server\share"
// pszD      Directory      MAX_PATH        Everything between Drive|UNC and name[.ext]
// pszN      Name           MAX_PATH
// pszE      .Ext           MAX_PATH
//
// Note: a path like "d:\some\silly\thing" returns "d:", "\some\silly\", "thing", ""
//
void WINAPI PathSplit ( _In_z_ PCWSTR pszFN, _Out_opt_cap_(MAX_PATH) PWSTR pszV, _Out_opt_cap_(MAX_PATH) PWSTR pszD, _Out_opt_cap_(MAX_PATH) PWSTR pszN, _Out_opt_cap_(MAX_PATH) PWSTR pszE);

void WINAPI PathSplitInPlace
(
	_In_z_ PCWSTR pszPath,
	int iPathLength,
	int *piDrive,
	int *piDriveLength,
	int *piDirectory,
	int *piDirectoryLength,
	int *piFilename,
	int *piFilenameLength,
	int *piExtension,
	int *piExtensionLength
);

//----------------------------------------------------------------
// Calculate the line and character index of an offset into a text buffer
BOOL    WINAPI   LineAndCharIndexOfPos (_In_count_(cch) PCWSTR pchText, int cch, int cchPos, int * piLine, int * piIndex);

//----------------------------------------------------------------
// StrList* operate on empty string-terminated lists of NUL-terminated strings (e.g. filter strings).
//
int     WINAPI   StrListSize   ( _In_z_ PCWSTR psz); // count of chars to hold the list (includes terminator)
int     WINAPI   StrListSizeA  ( _In_z_ PCSTR  psz); // count of bytes to hold the list (includes terminator)
int     WINAPI   StrListCount  ( _In_z_ PCWSTR psz); // count of strings in the list (not including terminator)
int     WINAPI   StrListCountA ( _In_z_ PCSTR  psz);
int     WINAPI   StrListCounts ( _In_z_ PCWSTR psz, int * pcStr = NULL); // return count of chars to hold list, *pcStr=count of strings

PCWSTR * WINAPI  StrListCreateArray ( _In_z_ PCWSTR pList, int * pcel);
PCWSTR  WINAPI   StrListNext        ( _In_z_ PCWSTR pList);
PCWSTR  WINAPI   StrListFind        ( _In_z_ PCWSTR pList, _In_z_ PCWSTR pPattern, bool fCase = true);
PCWSTR  WINAPI   StrListFindSorted  ( _In_z_ PCWSTR pList, _In_z_ PCWSTR pPattern, bool fCase = true);

// VS7:32377  non-const variants
inline PWSTR * WINAPI StrListCreateArray ( _In_z_ PWSTR pList, int * pcel) {return const_cast<PWSTR*>(StrListCreateArray (const_cast<PCWSTR>(pList), pcel));}
inline PWSTR   WINAPI StrListNext        ( _In_z_ PWSTR pList) {return const_cast<PWSTR>(StrListNext(const_cast<PCWSTR>(pList)));}
inline PWSTR   WINAPI StrListFind        ( _In_z_ PWSTR pList, _In_z_ PCWSTR pPattern, bool fCase = true){return const_cast<PWSTR>(StrListFind        (const_cast<PCWSTR>(pList), pPattern, fCase));}
inline PWSTR   WINAPI StrListFindSorted  ( _In_z_ PWSTR pList, _In_z_ PCWSTR pPattern, bool fCase = true){return const_cast<PWSTR>(StrListFindSorted  (const_cast<PCWSTR>(pList), pPattern, fCase));}

int     WINAPI   StrSubstituteChar  ( _Inout_z_ PWSTR psz, WCHAR chOld, WCHAR chNew); 
int     WINAPI   StrSubstituteCharA ( _Inout_z_ PSTR  psz, CHAR  chOld, CHAR  chNew); // skips double byte chars

//----------------------------------------------------------------
// !! WARNING!! If you call FindURL or IsProtocol, you must call FreeCachedURLResources()
#define NO_HITTEST -1
BOOL    WINAPI   FindURL (
    _In_count_(iLen) PCWSTR sz,                // IN buffer
    int iLen,                 // IN length of buffer
    int iAt,                  // IN index of point to intersect, or NO_HITTEST
    _Inout_ INT_PTR * piStart,        // IN/OUT starting index to begin scan (IN), start of URL (OUT)
    _Out_ INT_PTR * piEndProtocol,  // OUT index of end of protocol
    _Out_ INT_PTR * piEnd           // OUT index of end of URL
    );
BOOL    WINAPI   IsProtocol ( _In_z_ PCWSTR sz);
void    WINAPI   FreeCachedURLResources (void);

//----------------------------------------------------------------
void    WINAPI   SwapSegments    ( _Inout_ PWSTR x, _Inout_ PWSTR y, _Inout_ PWSTR z);
void    WINAPI   PivotSegments   ( _Inout_ PWSTR pA, _Inout_ PWSTR pB, _Inout_ PWSTR pC, _Inout_ PWSTR pD);

#define TRUNC_BEGIN    0 // remove text at the very beginning
#define TRUNC_LEFT     1 // remove text towards the beginning
#define TRUNC_CENTER   2 // remove text from the center
#define TRUNC_RIGHT    3 // remove text towards the end
#define TRUNC_END      4 // remove text from the very end

void    WINAPI   FitText( _In_z_ _Pre_count_(cchText) PCWSTR pszText, size_t cchText, _Out_cap_(cchDst) PWSTR pszDst, size_t cchDst, _In_z_ PCWSTR pszFill, DWORD flags);


// NormalizeFileSlashes
// Convert forward slashes to backslashes.
// Reduce multiple slashes to a single slash (leading double slash allowed).
//
// To normalize to forward slashes use StrSubstituteChar(psz, '\\', '/'); after NormalizeFileSlashes
//
// Returns a pointer to the 0 terminator of the transformed string
//
PWSTR   WINAPI   NormalizeFileSlashes  ( _Inout_z_ PWSTR szFile);
PSTR    WINAPI   NormalizeFileSlashesA ( _Inout_z_ PSTR  szFile);

void    WINAPI   TrimWhitespace  ( _Inout_opt_z_ PWSTR psz); // removes leading and trailing whitespace from psz

// Remove blanks adjacent to line ends within a buffer.
#define TLB_START      1
#define TLB_END        2
unsigned int  WINAPI   TrimLineBlanks  ( _Inout_ _Pre_count_(cch) _Post_count_(return) PWSTR pchBuf, unsigned int cch, DWORD dwFlags);

enum StripBlanksFlags{
//  --------------------    ------     -----------------------   -------------------------
//  Flag:                   Value:     On input:                 Return value:
//  --------------------    ------     -----------------------   -------------------------
    STB_NONE              = 0x0000, // Remove all blanks.        No blanks found.
    STB_SINGLE            = 0x0001, // Reduce to single blanks.  Only single blanks found.
    STB_MULTIPLE          = 0x0002, // (N/A)                     Multiple blanks found.
    STB_STRIP_LINEBREAKS  = 0x0010  // Include line breaks.      (N/A)
};

StripBlanksFlags  WINAPI  StripBlanks ( _Inout_z_cap_(*plen+1) _Prepost_count_(*plen) WCHAR * pchBuf, unsigned int * plen, int flags = STB_NONE);

// Determine whether a file path is local, UNC, or absolute 
//
// These examine the string only, and only the first part at that.  They don't
// attempt to decide whether the path is a valid filename or not.

BOOL WINAPI IsLocalAbsPath( _In_z_ PCWSTR sz);   // Path starts with <letter>:
BOOL WINAPI IsUNC( _In_z_ PCWSTR sz);            // Path starts with two backslashes
BOOL WINAPI IsAbsPath( _In_z_ PCWSTR sz);        // Either of the above

// Remove Bidi formatting characters from a string if the underlying system does not support them.
// They would be displayed as ? on Win9x systems that don't have appropriate codepage support.

void WINAPI StripUnsupportedBidiFormatChars( _Inout_z_ PWSTR sz);

//-----------------------------------------------------------------
/*
  BOOL FindURL (
    PCWSTR sz,                // IN buffer
    int iLen,                 // IN length of buffer
    int iAt,                  // IN index of point to intersect, or NO_HITTEST
    INT_PTR * piStart,        // IN/OUT starting index to begin scan (IN), start of URL (OUT)
    INT_PTR * piEndProtocol,  // OUT index of end of protocol
    INT_PTR * piEnd           // OUT index of end of URL
    );

  Find an URL in text, starting at *piStart index into wsz.
  iAt is NO_HITTEST to find the first URL in the text.
  To find an URL that intersects a point in the text, iAt is index from wsz of the point.
*/

//-----------------------------------------------------------------
//
// int StrLen (PCWSTR psz);
// Returns: count of chars in string 
//

//-----------------------------------------------------------------
//
// PSTR FindLastCharA (PCSTR psz, CHAR ch);
//
// MBCS-aware version of strrchr.
// Returns: pointer to right-most instance of ch in psz.
//

//-----------------------------------------------------------------
//
// PWSTR FindCharInSet ( _In_count_(cchBuffer) PCWSTR pchBuffer, int cchBuffer, _In_z_ PCWSTR set);
//
// Length-limited wide-char version of strpbrk.
// Returns: pointer to first char from set in buffer.
//

//-----------------------------------------------------------------
//
// PWSTR CopyCat (PWSTR dst, PCWSTR src)
//
// Wide char string copy for concatenation. Copy Src to Dst.
// Return: position of NUL in dst for further catenation.
//

//-----------------------------------------------------------------
//
// PWSTR CopyNCat (PWSTR dst, PCWSTR src, int cchz);
//
// NUL-limited char copy up to n chars for catentation
//
// Return: Position after char n in destination or position of 
//         copied NUL for further catenation.
//

//-----------------------------------------------------------------
//
// int StrListSize   (PCWSTR psz); 
//
// Return: count of chars to hold the list (includes terminator)

//-----------------------------------------------------------------
//
// int StrListSizeA  (PCSTR  psz); 
//
// Return: count of bytes to hold the list (includes terminator)

//-----------------------------------------------------------------
//
// int StrListCount  (PCWSTR psz); 
//
// Return: count of strings in the list (not including terminator)

//-----------------------------------------------------------------
//
// int StrListCountA (PCSTR  psz);
//
// Return: count of strings in the list (not including terminator)

//-----------------------------------------------------------------
//
// int StrListCounts (PCWSTR psz, int * pcStr = NULL); 
//
// pcStr    Receives count of strings in the list (not including terminator)
//
// Return: return count of chars to hold list
//

//-----------------------------------------------------------------
//
// PCWSTR * StrListCreateArray (PCWSTR pList, int * pcel);
//
// pcel     Receives a count of elements in the returned array,
//          not including the terminating NULL entry.
//
// Return: NULL-terminated array of PWSTRs pointing to the strings 
//         in pList, or NULL if out of memory.
//
// You must free the returned array using VSFree
//

//-----------------------------------------------------------------
//
// PCWSTR StrListNext (PCWSTR pList);
//
// Return: Pointer to the next non-empty string in the list, or 
//         NULL if no more strings
//

//-----------------------------------------------------------------
//
// HRESULT StrListSort (PWSTR pList, bool fCase = true);
//
// Sorts the string list
//
// fCase    true for case-sensitive, false for case-insensitive
//
// Return: success
//

//-----------------------------------------------------------------
//
// HRESULT StrListSortPairs (PWSTR pList, bool fCase = true);
//
// Sorts the paired-string (key/value) list. There must be an even 
// number of strings in the list. Keys and values must not contain L'\1'.
//
// fCase    true for case-sensitive, false for case-insensitive
//
// Return: success
//

//-----------------------------------------------------------------
//
// PCWSTR StrListFind (PCWSTR pList, PCWSTR pPattern, bool fCase = true);
//
// pList     Sorted list of strings to search
// pPattern  String to find in list
// fCase     true to match exact case, false to ignore case 
//
// Return: Matching string in list or NULL if not found
//

//-----------------------------------------------------------------
//
// PCWSTR StrListFindSorted (PCWSTR pList, PCWSTR pPattern, bool fCase = true);
//
// pList     Sorted list of strings to search
// pPattern  String to find in list
// fCase     true to match exact case, false to ignore case 
//           The list must be sorted in ascending order with the same fCase
//
// Return: Matching string in list or NULL if not found
//

//-----------------------------------------------------------------
//
// int StrSubstituteChar  (PWSTR psz, WCHAR chOld, WCHAR chNew); 
// int StrSubstituteCharA (PSTR  psz, CHAR  chOld, CHAR  chNew); // skips double byte chars
//
// Return: Count of chars replaced
//

//-----------------------------------------------------------------
//
// void TrimWhitespace (PWSTR psz);
//
// Remove whitespace from the start and end of the string
//

//-----------------------------------------------------------------
//
// unsigned int TrimLineBlanks (PWSTR pchBuf, unsigned int cch, DWORD dwFlags);
//
// Remove blanks adjacent to line ends within a buffer.
//
// Set TLB_START in dwFlags to also remove blanks at the start of the buffer.
// Set TLB_END in dwFlags to also remove blanks at the end of the buffer.
// 
// Returns trimmed length
// 

//-----------------------------------------------------------------
//
// void WINAPI SwapSegments(PWSTR x, PWSTR y, PWSTR z);
//
// Swap two segments of a string.
//
//  IN: xxxxxYYYz
// OUT: YYYxxxxxz
//
// Z can point to the zero terminator or past the end of the buffer.
// The swap is performed in-place.
//

//-----------------------------------------------------------------
// void PivotSegments (PWSTR pA, PWSTR pB, PWSTR pC, PWSTR pD);
//
// Pivot two segments of a string around a middle segment.
//
//  IN: aaaaaaaBBcccccD
// OUT: cccccBBaaaaaaaD
//
// D can point to the zero terminator or past the end of the buffer.
// The pivot is performed in-place.
//

//=================================================================
//===== Implementation ============================================
//=================================================================

inline int WINAPI StrLen ( _In_opt_z_ PCWSTR psz)
{
    if (!psz) return 0;

    // Win64Fix (MikhailA): wcslen returns size_t which is 64-bit long.
    // In this particular case I think int is sufficient and I won't need to fix all the calls to the StrLen
    return (int)wcslen(psz);
}

inline int WINAPI StrLenA ( _In_opt_z_ PCSTR psz) 
{ 
    if (!psz) return 0;

    // Win64Fix (MikhailA): strlen returns size_t which is 64-bit long.
    // In this particular case I think int is sufficient and I won't need to fix all the calls to the StrLenA
    return (int) strlen(psz);
}

inline PWSTR WINAPI CopyCat ( _Pre_notnull_ _Post_z_ PWSTR dst, _In_z_ PCWSTR src)
{
    while ((*dst++ = *src++))
        ;
    return --dst; 
}

// Ansi version
inline PSTR WINAPI CopyCatA ( _Pre_notnull_ _Post_z_ PSTR dst, _In_z_ PCSTR src)
{
    while ((*dst++ = *src++))
        ;
    return --dst; 
}

// Guarantees zero termination. Can write one more than cchz
inline PWSTR WINAPI CopyNCat ( _Out_z_cap_(cchz+1) PWSTR dst, _In_z_ PCWSTR src, int cchz)
{
    WCHAR ch = 0xFFFF; // UCH_NONCHAR
    while (cchz-- && (ch = *dst++ = *src++) != 0)
        ;
    if (ch)
    {
        *dst = 0;
        return dst;
    }
    else
        return --dst;
}


// Ansi version
inline PSTR WINAPI CopyNCatA ( _Out_z_cap_(cchz+1) PSTR dst, _In_z_ PCSTR src, int cchz)
{
    CHAR ch = 1;
    while (cchz-- && (ch = *dst++ = *src++))
        ;
    if (ch)
    {
        *dst = 0;
        return dst;
    }
    else
        return --dst;
}

inline PWSTR WINAPI CopyCatInt ( _Pre_notnull_ _Post_z_ PWSTR dst, int cchBufferSize, int n, int radix)
{
	_itow_s(n, dst, cchBufferSize, radix);

	while (*dst)
		dst++;
	return dst;
}

#ifndef FEATURE_PAL // (VSWhidbey 199217) - _itoa not defined for FEATURE_PAL
inline PSTR WINAPI CopyCatIntA ( _Pre_notnull_ _Post_z_ PSTR dst, int cchBufferSize, int n, int radix)
{
	_itoa_s(n, dst, cchBufferSize, radix);

	while (*dst)
		dst++;
	return dst;
}
#endif

#pragma deprecated(CopyCat)
#pragma deprecated(CopyCatA)
#pragma deprecated(CopyNCat)
#pragma deprecated(CopyNCatA)
#pragma deprecated(CopyCatInt)
#pragma deprecated(CopyCatIntA)

inline PCWSTR WINAPI FindChar ( _In_opt_z_ PCWSTR psz, WCHAR ch)
{
    if (!psz) 
        return NULL;
    WCHAR T;
    do
	{
        T = *psz;
        if (T == ch)
            return psz;
        psz++;
    } while (T);
    return NULL;
}

inline PCWSTR WINAPI FindSlash ( _In_z_ PCWSTR psz)
{
    if (!psz) 
        return NULL;
    WCHAR T;
    do
	{
        T = *psz;
        if ((T == L'\\') || (T == L'/'))
            return psz;
        psz++;
    } while (T);
    return NULL;
}

inline PCWSTR WINAPI FindCharN ( _In_opt_count_(cch) PCWSTR pchBuffer, int cch, WCHAR ch)
{
    if (!pchBuffer) 
        return NULL;
    while (cch--)
    {
        if (ch == *pchBuffer)
            return pchBuffer;
        ++pchBuffer;
    }
    return NULL;
}

inline PCWSTR WINAPI FindLastChar ( _In_z_ PCWSTR  psz, WCHAR ch)
{
    if (!psz) 
        return NULL;
    PCWSTR pch = NULL;
    WCHAR T;
    for (; (T = *psz); psz++)
    {
        if (T == ch)
            pch = psz;
    }
    return pch;
}

inline PCWSTR WINAPI FindLastSlash ( _In_z_ PCWSTR  psz, int iLength)
{
    if (!psz || !iLength) 
        return NULL;

    for (PCWSTR pch = psz + (iLength-1); pch >= psz; pch--)
    {
        if ((*pch == L'\\') || (*pch == L'/'))
            return pch;
    }
    return NULL;
}

inline BOOL WINAPI IsLocalAbsPath ( _In_z_ PCWSTR sz)
{
    return ((sz[0] >= L'A' && sz[0] <= L'Z') || (sz[0] >= L'a' && sz[0] <= L'z'))
        && sz[1] == L':';
}

inline BOOL WINAPI IsUNC ( _In_z_ PCWSTR sz)
{
    return sz[0] == L'\\' && sz[1] == L'\\';
}

inline BOOL WINAPI IsAbsPath (PCWSTR sz)
{
    return IsLocalAbsPath(sz) || IsUNC(sz);
}

#endif // __UNISTR_H__

