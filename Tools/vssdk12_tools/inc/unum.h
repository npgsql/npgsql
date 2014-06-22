// uNum.h - Unicode number handling
//-----------------------------------------------------------------
// Microsoft Confidential
// Copyright 1999 Microsoft Corporation.  All Rights Reserved.
//
// June 1, 1999 [paulde]
//
// Written to a draft Unicode 3.0 standard
//
//-----------------------------------------------------------------
#ifdef _MSC_VER
#pragma once
#endif

#ifndef __UNUM_H__
#define __UNUM_H__

/*
    WStrToLong, WStrToULong - Convert a Unicode string to an integer.

    SHLWAPI defines some the names that we would prefer to use,
    so we use 'WStr' instead of 'Str'

    These are very similar to wcstol (the code is derived from them), 
    but handle full-width digits and letters and script-specific decimal 
    digits such as Arabic digits.

    Unlike the CRT routines, these do not set errno. Instead, you can pass 
    a pointer to an overflow flag.

    The parameters have been rearranged and given default arguments for common usage.

    NOT HANDLED: ETHIOPIC, TAMIL, SUPERSCRIPT, SUBSCRIPT
    ETHIOPIC and TAMIL do not use conventional positional notation.
    We don't expect users to input superscript/subscript numbers.

    RETURNS:    On error   : 0
                On overflow: LONG_MAX or LONG_MIN (WStrToLong)
                             LONG_MAX (WStrToULong)
                Else       : number

    ARGUMENTS:

    nptr        String to parse an integer from

    endptr      Optional address of pointer that will point to the character 
                where parsing stopped.
                Same as nptr when the string cannot be interpreted as a number.

    ibase       Numeric base: default 10.

                0       Interpret base according to the text:
                            Leading '0' : base 8
                            Leading '0x': base 16
                            Otherwise   : base 10
                        0 can be a national digit zero.
                        x is case and width-insensitive.

                2 - 36  For bases above 10, the letters a-z (case/width insensitive) are 
                        interpreted as digits.

    pfOverflow  Optional pointer to overflow flag.
    
*/

long WINAPI WStrToLong (
    _In_z_ const WCHAR *   nptr,               // text to parse a number from
    _In_opt_ _Deref_out_z_ const WCHAR **  endptr = NULL,      // optional address of pointer to end of number, or start if not a number
    int             ibase = 10,         // optional number base
    _Out_opt_ bool *          pfOverflow = NULL   // optional overflow flag
    );

unsigned long WINAPI WStrToULong (
    _In_z_ const WCHAR *   nptr,
    _In_opt_ _Deref_out_z_ const WCHAR **  endptr = NULL,
    int             ibase = 10,
    _Out_opt_ bool *          pfOverflow = NULL
    );

//-----------------------------------------------------------------
// FoldNumericsToASCII - Convert numerics in a string to their ASCII equivalents
//
// This converts a wider range of things to ASCII than are built into WStrToLong.
// This can be used to prepare a buffer for use with CRT routines to convert
// floating point numbers or perform similar processing.
//
// Script decimal digits
// Script FULL STOP converted to '.'
// Script +/- converted to '+'/'-'
// All Full-width ASCII equivalents converted to ASCII
// Miscellaneous stops converted to '.'
// Superscript digits and +/- converted to ASCII
//
// Does NOT convert script commas or other mathematical operators (e.g. '/' '\' division)
//
// RETURNS: TRUE on success, FALSE if arguments are bad (e.g. NULL ptrs)
//
// cchSrc  -1 if null-terminated, else count of chars to convert
// pchDst  Assumed to be at least the same size as input string
//
bool WINAPI FoldNumericsToASCII (int cchSrc, /* __in_xcount(cchSrc) */ PCWSTR pchSrc, __out_z PWSTR pchDst);

//-----------------------------------------------------------------
// FoldFullWidthLatinToASCII
//
// Convert FULLWIDTH LATIN characters in a string to their LATIN counterparts.
//
// RETURNS: TRUE on success, FALSE if arguments are bad (e.g. NULL ptrs)
//
// cchSrc   -1 if null-terminated, else count of chars to convert
// pchDst   Assumed to be at least the same size as input string
//
bool WINAPI FoldFullWidthLatinToASCII (int cchSrc, /* __in_xcount(cchSrc)*/ PCWSTR pchSrc, __out_z PWSTR pchDst);

//-----------------------------------------------------------------
// IsDigitValue - Determines if a char is a decimal digit and 
//                gets it's numeric value.
//
// Returns: TRUE    char is a decimal digit
//          FALSE   char is not a decimal digit
//
// *puVal gets numeric value of the digit, or 0 if not a digit
//
bool WINAPI IsDigitValue (WCHAR ch, unsigned * puVal = NULL);

//-----------------------------------------------------------------
// IsAZDigitValue -  Determines if char is a-zA-Z or fullwidth equivalent
//                   and returns it's numeric value.
//
// Returns: TRUE    char is a-z
//          FALSE   char is not a-z
//
// *puVal gets numeric value if char in a-z (a == 10), else 0
//
bool WINAPI IsAZDigitValue (WCHAR ch, unsigned * puVal = NULL);

//-----------------------------------------------------------------
// SignChar - Get sign signifier of a char
//
// Returns:
//       1 if char is a plus sign or fullwidth equivalent
//      -1 if char is a minus sign or fullwidth equivalent
//       0 otherwise
//
// 002B;PLUS SIGN;Sm;0;ET;;;;;N;;;;;
// FF0B;FULLWIDTH PLUS SIGN;Sm;0;ET;<wide> 002B;;;;N;;;;;
// 002D;HYPHEN-MINUS;Pd;0;ET;;;;;N;;;;;
// FF0D;FULLWIDTH HYPHEN-MINUS;Pd;0;ET;<wide> 002D;;;;N;;;;;
//
// Note that there are other +/- equivalents in Unicode.
//
int  WINAPI SignChar (WCHAR ch);



#endif // __UNUM_H__
