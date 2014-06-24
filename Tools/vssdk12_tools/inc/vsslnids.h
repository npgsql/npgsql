// SolnMnId.h

#ifndef _VSSLNIDS_H_
#define _VSSLNIDS_H_

#ifndef NOGUIDS

#ifdef DEFINE_GUID
  DEFINE_GUID (guidSolnPkg,
    0x282BD676, 0x8B5B, 0x11D0, 0x8A, 0x34, 0x00, 0xA0, 0xC9, 0x1E, 0x2A, 0xCD);

  DEFINE_GUID (guidSolnBuilder,
    0xd0b027ce, 0x8c1f, 0x11d0, 0x8a, 0x34, 0x00, 0xa0, 0xc9, 0x1e, 0x2a, 0xcd);

  DEFINE_GUID (guidSolnLDM,
    0xa42b2bf0, 0x516e, 0x11d1, 0xa1, 0xfa, 0x00, 0x00, 0xf8, 0x02, 0x6f, 0x55);

#else

  #define guidSolnPkg	    {0x282BD676,0x8B5B,0x11D0,{0x8A,0x34,0x00,0xA0,0xC9,0x1E,0x2A,0xCD}}
  #define guidSolnBuilder   {0xd0b027ce,0x8c1f,0x11d0,{0x8a,0x34,0x00,0xa0,0xc9,0x1e,0x2a,0xcd}}
  #define guidSolnLDM	    {0xa42b2bf0,0x516e,0x11d1,{0xa1,0xfa,0x00,0x00,0xf8,0x02,0x6f,0x55}}

#endif //DEFINE_GUID
#endif //NOGUIDS

// Top level "Build" menu.
#define IDM_SLN_BUILDMENU	1
#define IDM_SLN_BUILDTOOLBAR 4
#define IDM_SLN_BUILD_CONFIG 5

// Simple project context menu
#define IDM_SPROJ_CTXT		2
#define IDM_LOCALDEPLOY_MAPTGTCONTEXT		3

// Build menu groups.
#define IDG_SLN_BUILD		11
#define IDG_SLN_REBUILD         12
#define IDG_SLN_BATCHBUILD	13
#define IDG_SLN_GO		14
#define IDG_SLN_CLEAN		15
#define IDG_SLN_CANCEL		16
#define IDG_SPROJ_CTXT_OPTIONS  17
#define IDG_SLN_DEPLOY          18

#define IDG_BUILD_TOOLBAR	20
#define IDG_CFG_TOOLBAR		21

#define icmdBuildSolution	110
#define icmdRebuildSolution	120
#define icmdRebuildSelection	125
#define icmdCleanSolution	130
#define icmdDeploySolution	140
#define icmdDeploySelection     145

#define icmdBuildSelection	150
#define icmdCleanSelection	160
#define icmdBuildCancel		170
#define icmdBatchBuild          175

#define icmdProjectBuildSettings	180

#define itbrCfg				190
#define cmdidSlnCfgCombo	191
#define cmdidSlnCfgGetList	192


// Build menu button icons
#define iconCompile			1
#define iconBuildSelection	2
#define iconBuildSolution	3
#define iconCancelBuild		4

#endif //_VSSLNIDS_H_