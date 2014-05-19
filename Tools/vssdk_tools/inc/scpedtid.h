//////////////////////////////////////////////////////////////////////////////
//
//Microsoft Confidential
//Copyright 1996-1997 Microsoft Corporation.  All Rights Reserved.
//
//File: scpedtid.h
//
//Contents:
//
//////////////////////////////////////////////////////////////////////////////


#ifndef _SCPEDTID_H_
#define _SCPEDTID_H_

#ifndef NOGUIDS

#ifdef DEFINE_GUID

  // Guids for the script editor package.
  DEFINE_GUID(guidSCPGrpId, 
  0xd842f011, 0xdd01, 0x11d0, 0xa7, 0x68, 0x0, 0xa0, 0xc9, 0x11, 0x10, 0xc3);

  DEFINE_GUID(guidSCPCmdId, 
  0xd842f010, 0xdd01, 0x11d0, 0xa7, 0x68, 0x0, 0xa0, 0xc9, 0x11, 0x10, 0xc3);

#else

  // Guids for the script editor
  #define guidSCPCmdId { 0xd842f010, 0xdd01, 0x11d0, { 0xa7, 0x68, 0x0, 0xa0, 0xc9, 0x11, 0x10, 0xc3 } }
  #define guidSCPGrpId { 0xd842f011, 0xdd01, 0x11d0, { 0xa7, 0x68, 0x0, 0xa0, 0xc9, 0x11, 0x10, 0xc3 } }
#endif

#endif  // NOGUIDS



// Script Editor Group
#define IDG_SCP_MainMenu        0x0900

// Script Editor Commands.
#define cmdSCPSynchTree            0x0300
#define cmdSCPGotoDefinition       0x0400
#define cmdSCPSortCascade          0x0700
#define cmdSCPToggleUnimplEvents   0x0800


#endif
