// codepage.h - codepage identifiers
//-----------------------------------------------------------------
// Microsoft Confidential
// Copyright 1998 Microsoft Corporation.  All Rights Reserved.
//
// June 1, 1998 [paulde]
//
//---------------------------------------------------------------

#pragma once
#ifndef __CODEPAGE_H__
#define __CODEPAGE_H__

UINT WINAPI CodePageFromLCID (LCID lcid);
UINT WINAPI CodepageFromCharset (BYTE cs);
BOOL WINAPI AnsiCodePageSupportsLCID (UINT cp, LCID lcid);

#ifndef CP_INVALID
#define CP_INVALID        0xFFFF // useful invalid codepage value
#endif

//---------------------------------------------------------------
// Windows Code Pages
//
#define CP_THAI              874 // Thai
#define CP_JPN               932 // Japanese
#define CP_CHS               936 // Simplified Chinese (PRC, Singapore)
#define CP_KOR               949 // Korean
#define CP_CHT               950 // Traditional Chinese (Taiwan, Hong Kong)

#ifndef CP_UNICODE
#define CP_UNICODE          1200 // Unicode
#endif

#define CP_UNICODESWAP      1201 // Unicode Big-Endian

#define CP_EASTEUROPE       1250 // Windows 3.1 Eastern European
#define CP_CYRILLIC         1251 // Windows 3.1 Cyrillic
#define CP_LATIN1           1252 // Windows 3.1 Latin 1 (US, Western Europe)
#define CP_GREEK            1253 // Windows 3.1 Greek
#define CP_TURKISH          1254 // Windows 3.1 Turkish
#define CP_HEBREW           1255 // Hebrew
#define CP_ARABIC           1256 // Arabic
#define CP_BALTIC           1257 // Baltic

// winnls.h
//#define CP_ACP                    0           // default to ANSI code page
//#define CP_OEMCP                  1           // default to OEM  code page
//#define CP_MACCP                  2           // default to MAC  code page
//#define CP_THREAD_ACP             3           // current thread's ANSI code page
#ifndef CP_SYMBOL
#define CP_SYMBOL          42          // SYMBOL encoding (SYMBOL font encoding)
#endif
#ifndef CP_UTF7
#define CP_UTF7            65000 // UTF-7
#endif
#ifndef CP_UTF8
#define CP_UTF8            65001 // UTF-8
#endif

//---------------------------------------------------------------
// OEM Code Pages
//
#define CP_OEM_US                437 // MS-DOS United States
#define CP_OEM_ARABICASMO708     708 // Arabic (ASMO 708) 
#define CP_OEM_ARABICASMO449     709 // Arabic (ASMO 449+, BCON V4) 
#define CP_OEM_ARABICTRANSP      710 // Arabic (Transparent Arabic) 
#define CP_OEM_ARABICASMOTRANS   720 // Arabic (Transparent ASMO) 
#define CP_OEM_GREEK             737 // Greek (formerly 437G) 
#define CP_OEM_BALTIC            775 // Baltic 
#define CP_OEM_LATIN1            850 // MS-DOS Multilingual (Latin I) 
#define CP_OEM_SLAVIC            852 // MS-DOS Slavic (Latin II) 
#define CP_OEM_CYRILLIC          855 // IBM Cyrillic (primarily Russian) 
#define CP_OEM_TURKISH           857 // IBM Turkish 
#define CP_OEM_PORTUGESE         860 // MS-DOS Portuguese 
#define CP_OEM_ICELANDIC         861 // MS-DOS Icelandic 
#define CP_OEM_HEBREW            862 // Hebrew 
#define CP_OEM_CANADIANFRENCH    863 // MS-DOS Canadian-French 
#define CP_OEM_ARABIC            864 // Arabic 
#define CP_OEM_NORDIC            865 // MS-DOS Nordic 
#define CP_OEM_RUSSIAN           866 // MS-DOS Russian (former USSR) 
#define CP_OEM_MODERNGREEK       869 // IBM Modern Greek 
#define CP_OEM_THAI              874 // Thai 
#define CP_OEM_JPN               932 // Japan 
#define CP_OEM_CHS               936 // Chinese (PRC, Singapore) 
#define CP_OEM_KOR               949 // Korean 
#define CP_OEM_CHT               950 // Chinese (Taiwan, Hong Kong) 
#define CP_OEM_JOHAB            1361 // Korean (Johab) 

#endif // __CODEPAGE_H__

