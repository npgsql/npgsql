//-----------------------------------------------------------------------------
//
//    Microsoft Visual Web Developer
//
//    Microsoft Confidential
//    Copyright 1997-2005 Microsoft Corporation.    All Rights Reserved.
//
//    File:    VenusCmdDef.h
//    Area:    Visual Web Developer Commands
//
//-----------------------------------------------------------------------------

#ifndef _VENUSCMDDEF_H_
#define _VENUSCMDDEF_H_

// Internal use mostly
#define icmdCheckout                        15000
#define icmdPartialParse                    15001
#define icmdFullParse                       15002
#define icmdDoIdle                          15003
#define icmdShowValidationErrors            15004

#define icmdGenerateServerCode              15008
#define icmdEnsureCodeGenerationPending     15009
#define icmdEnsureServerCodeReady           15010
#define icmdIsServerCodeReady               15011

#define icmdEnsureReferencesReady           15012

#define icmdGenerateControlSchemas          15013
#define icmdEnsureControlSchemasReady       15014

#define icmdGenerateServerEventHandler      15015
#define icmdGenerateClientEventHandler      15016
#define icmdGeneratePageEventHandler        15017
#define icmdGenerateDocumentEventHandler    15018

#define icmdSyncViews                       15023
#define icmdSyncFromView                    15024

#define icmdIsControlSchemasReady           15025
#ifdef DEFINE_GUID
// guid for HTM Editor group IDs   {D7E8C5E1-BDB8-11d0-9C88-0000F8040A53}
DEFINE_GUID(CMDSETID_HtmEdGrp,
0xd7e8c5e1, 0xbdb8, 0x11d0, 0x9c, 0x88, 0x0, 0x0, 0xf8, 0x4, 0xa, 0x53);

// GUID for Server Form Designer
DEFINE_GUID(CMDSETID_WebForms,
            0x29aab322, 0x2365, 0x11d3, 0x82, 0x2f, 0x0, 0x60, 0x97, 0xde, 0x50, 0xf5);
#endif // DEFINE_GUID

#endif // _VENUSCMDDEF_H_