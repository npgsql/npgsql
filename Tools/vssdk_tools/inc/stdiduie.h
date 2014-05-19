//-----------------------------------------------------------------------------
// Microsoft Visual Studio
//
// Copyright 1995-1997 Microsoft Corporation.  All Rights Reserved.
//
// File: stdiduie.h
// Area: IOleComponentUIManager
//
// Contents:
//   Contains ids used for UIEvents used in StandardUIEventSet98.
//   StandardUIEventSet98 is defined by the following guid:
//
//   {54038220-FA22-11d0-8798-00A0C91E2A46}
//   DEFINE_GUID(StandardUIEventSet98, 
//   0x54038220, 0xfa22, 0x11d0, 0x87, 0x98, 0x0, 0xa0, 0xc9, 0x1e, 0x2a, 0x46);
//-----------------------------------------------------------------------------

#ifndef _STDIDUIE_H_                  
#define _STDIDUIE_H_                  

#ifndef __NOENUM__

// for specialized contracts
enum
  {
  UIE_TEXTSELMODE_STREAM   = 0,
  UIE_TEXTSELMODE_BOX      = 1
  };

enum
  {
  UIE_TEXTINSMODE_INSERT     = 0,
  UIE_TEXTINSMODE_OVERSTRIKE = 1
  };

#endif //__NOENUM__

// Events for GUID StandardUIEventSet98
// The following UIEvents all use:
//    dwUIEventStatus = OLEUIEVENTSTATUS_OCCURRED
//    dwEventFreq     = OLEUIEVENTFREQ_NULL
#define uieventidSetTextLinePos     1	// ARG: NULL, VT_I2, VT_I4, VT_BSTR, or VT_EMPTY
					//	If VT_I2 or VT_I4, number is formatted into "Ln ##"
					//	if VT_BSTR, text is displayed as is with no formatting
					//	if VT_EMPTY, value is NOT displayed at all
					//	if NULL pointer, value is left unchanged

#define uieventidSetTextColPos	    10	// ARG: NULL, VT_I2, VT_I4, VT_BSTR, or VT_EMPTY
					//	If VT_I2 or VT_I4, number is formatted into "Ln ##"
					//	if VT_BSTR, text is displayed as is with no formatting
					//	if VT_EMPTY, value is NOT displayed at all
					//	if NULL pointer, value is left unchanged

#define uieventidSetTextCharPos     2	// ARG: NULL, VT_I2, VT_I4, VT_BSTR, or VT_EMPTY
					//	If VT_I2 or VT_I4, number is formatted into "Ln ##"
					//	if VT_BSTR, text is displayed as is with no formatting
					//	if VT_EMPTY, value is NOT displayed at all
					//	if NULL pointer, value is left unchanged

#define uieventidSetTextSelMode     3	// ARG: VT_I4 UIE_TEXTSELMODE enum or VT_BSTR
#define uieventidSetTextInsMode     4	// ARG: VT_I4 UIE_TEXTINSMODE enum

#define uieventidSetGraphicXPos     5	// ARG: VT_I2, VT_I4, VT_R4, or VT_R8
#define uieventidSetGraphicYPos     6	// ARG: VT_I2, VT_I4, VT_R4, or VT_R8
#define uieventidSetGraphicXSize    7	// ARG: VT_I2, VT_I4, VT_R4, or VT_R8
#define uieventidSetGraphicYSize    8	// ARG: VT_I2, VT_I4, VT_R4, or VT_R8

// The following UIEvents all use:
//    dwUIEventStatus = OLEUIEVENTSTATUS_START / OLEUIEVENTSTATUS_STOP
//    dwEventFreq     = OLEUIEVENTFREQ_NULL
#define uieventidAnimateIcon	    9	// OLEUIEVENTSTATUS_START:  Starts animated icon
					// OLEUIEVENTSTATUS_STOP:   Stops animated icon
					// NYI: ARG: VT_I4, Index of predefined icons, or custom icon
#endif //_STDIDUIE_H_

