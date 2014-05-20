/*------------------------------------------------------------------------*
 * msosproc.h (prevously known as sdmproc.h): SDM PUBLIC include file     *
 *  -- SDM function prototypes.                                           *
 *                                                                        *
 * Please do not modify or check in this file without contacting MsoSdmQs.*
 *------------------------------------------------------------------------*/

#pragma once
#ifndef NO_SDM

//============================
// who's not defined?
#ifdef IME_OLD
#error
#endif
#ifdef BACKGROUND_PATTERN
#error
#endif
#ifdef IME32
// REVIEW KirkG: This is not defined for Mso.  It is defined for Word
//  (in word.h), but I don't think anything in Word's build uses it.
//#error
#endif
#ifdef DBCS
// REVIEW KirkG: This is not defined for Mso.  It is defined for Word
//  (in word.h), but I don't think anything in Word's build uses it.
//#error
#endif
#ifdef EXTCHAR
// REVIEW KirkG: This is not defined for Mso.  It is defined for Word
//  (in word.h), but I don't think anything in Word's build uses it.
//#error
#endif
//============================

typedef int (*MSO_PFN)(); // Pointer to function
 
///////////////////////////////////////////////////////////////////////////////
// Common calls.

// Initialization and main control.

MSOAPI_(BOOL_SDM)	FValidTmc(TMC);	//new 96: executes for ship and debug versions

#ifdef	VSMSODEBUG
MSOAPIX_(VOID)	EnableReports(BOOL_SDM);
#else
#define			EnableReports(f)	
#endif

MSOAPI_(FTME)	FtmeIsSdmMessage(LPMSG);

MSOAPI_(VOID)	ChangeColors(void);
MSOAPIX_(VOID)	SetDlgColor(HDLG, DWORD);
MSOAPIX_(DWORD)	ClrGetDlgColor(HDLG);

MSOAPI_(BOOL_SDM)	FInitSdm(SDI *);
MSOAPI_(VOID)	EndSdm(void);
MSOAPI_(BOOL) MsoFSetHMsoinstOfSdm(HMSOINST hinst);
MSOAPI_(BOOL) MsoFSetSdmNoModalComponent(BOOL fNoModalComp);
MSOAPI_(BOOL) MsoFSetSdmDontShowInvisibleWindow(BOOL fDontShow);
MSOAPI_(HMSOINST) HMsoinstGetFromSdm(void);
MSOAPI_(void)	MsoSetBidiRtlSdm(BOOL fRTL);	

// Help Support.
MSOAPI_(DWORD)	ContextPosOfDlg(HDLG);
MSOAPIXX_(UINT_SDM)	ContextHidOfDlg(HDLG);
MSOAPI_(UINT_SDM)	HidOfDlg(HDLG);
MSOAPIX_(UINT_SDM)	BaseHidOfDlg(HDLG);
MSOAPI_(BOOL_SDM)	FSetDlgHid(HDLG, UINT_SDM);
MSOAPI_(BOOL_SDM)	FIsOfficeDlg(HDLG);

// CAB Control.
MSOAPI_(HCAB)	HcabAlloc(UINT_SDM, SB_SDM);
MSOAPI_(VOID)	InitCab(HCAB, UINT_SDM);
MSOAPI_(VOID)	InitCabWords(HCAB, UINT_SDM);
MSOAPI_(VOID)	FreeCab(HCAB, SB_SDM);
MSOAPI_(VOID)	MsoFreeCabData(HCAB, SB_SDM);

__inline PCAB PcabLockCab(HCAB hcab, SB_SDM sb) { return (*(hcab)); }
#define UnlockCab(hcab, sb)

#ifdef VSMSODEBUG
// Use this one to mark dynamically allocated CAB's (most apps)
MSOAPIXX_(BOOL_SDM) MsoFWriteCabBe(HCAB hcab, LPARAM lParam);
// Use this one to mark statically allocated CAB's (special case)
MSOAPIXX_(BOOL_SDM) MsoFWriteCabHandlesBe(HCAB hcab, LPARAM lParam);
#endif
MSOAPI_(BOOL_SDM)	FSetCabWt(HCAB, CONST_WTZ_CAB, UINT_SDM, SB_SDM);
MSOAPI_(VOID)	GetCabWtz(HCAB, WTZ_CAB, UINT_SDM, UINT_SDM);
MSOAPI_(BOOL_SDM)	FSetCabWz(HCAB, CONST_WZ_CAB, UINT_SDM, SB_SDM);
MSOAPI_(UINT_SDM)	CchOfCabWz(HCAB, UINT_SDM);
MSOAPI_(VOID)	GetCabWz(HCAB, WZ_CAB, UINT_SDM, UINT_SDM);
MSOAPI_(BOOL_SDM)	FSetCabRgb(HCAB, CONST_RGB_CAB, UINT_SDM, UINT_SDM, SB_SDM);
MSOAPI_(VOID)	GetCabWt(HCAB, WTZ_CAB, UINT_SDM, UINT_SDM);
MSOAPI_(BOOL_SDM)	GetCabRgb(HCAB, RGB_CAB, UINT_SDM, UINT_SDM, SB_SDM);
#define FGetCabRgb GetCabRgb

MSOAPI_(BOOL_SDM) MsoFSetCabHandleWt(PCABH, CONST_WTZ_CAB, SB_SDM);
MSOAPI_(BOOL_SDM) MsoFSetCabHandleWz(PCABH, CONST_WZ_CAB, SB_SDM);
MSOAPI_(BOOL_SDM) MsoFSetCabHandleRgb(PCABH, CONST_RGB_CAB, UINT_SDM, SB_SDM);
MSOAPI_(VOID)	  MsoGetCabHandleWtz(PCABH, WTZ_CAB, UINT_SDM);
MSOAPI_(VOID)	  MsoGetCabHandleWz(PCABH, WZ_CAB, UINT_SDM);
MSOAPI_(VOID)	  MsoGetCabHandleWt(PCABH, WT_CAB, UINT_SDM);
MSOAPI_(BOOL_SDM) MsoFGetCabHandleRgb(PCABH, RGB_CAB, UINT_SDM, SB_SDM);

// Generic Dialog Control
MSOAPI_(void) 	MsoInitDli(DLI *, HWND, FDLG, UINT_SDM);
MSOAPI_(short) 	MsoCbOfDlt(long, HINSTANCE);
MSOAPI_(BOOL) 	MsoFBltDlt(long, void *, HINSTANCE);
MSOAPI_(TMC)	MsoTmcStartDlt(long, HCAB, UINT_SDM, FDLG, HWND, HINSTANCE, const MSO_PFN *, UINT_SDM);
//OAPI_(void) 	MsoStartModelessDlt(long, HCAB, UINT_SDM, FDLG, HWND, MLDComp *, HINSTANCE, PFN*, UINT_SDM);
MSOAPI_(void)	MsoFixDltPfns(void *, const MSO_PFN *);
MSOAPI_(void)	MsoConvertRecToRc(REC *, RECT *);
MSOAPIX_(BOOL) 	MsoFSetupDlgResizing(long dlt, XY_SDM dx, XY_SDM dy, PFN_RESIZE pfnResize, HINSTANCE hinst);
MSOAPI_(BOOL)   MsoFSetupDalResizing(XY_SDM dx, XY_SDM dy, BOOL fResizeBorder);
MSOAPI_(BOOL)   MsoFResetDalResizing();
MSOAPI_(BOOL)   MsoFFixResizedRect(HWND hwnd, TMC tmc); // Resizes an SDM picture's
                                                        // window if necessary.  This
                                                        // should be called on tmmPaint
                                                        // messages.
MSOAPI_(BOOL) MsoFFixResizedIndentedRect(HWND hwnd, TMC tmc, RECT rectIndents);

MSOAPI_(LPVOID) MsoLoadPres(HINSTANCE hinst, int resType, int resId);

MSOAPI_(VOID) MsoSetDefaultButtonIndication(HDLG hdlg, BOOL fUseDefault);

// Resize handles (grippies)
MSOAPI_(VOID) MsoSetGrippiesDlg(HDLG hdlg, BOOL fUseGrippies);
MSOAPI_(BOOL) MsoFGetGrippiesDlg(HDLG hdlg);
#define msoGrippieSize 11

// Define some macros
#define MsoTmcPerformDlg(Dlt, hcab, fdlg, hParent, hinst, rgpfn, wRef) \
	MsoTmcStartDlt((long)dlt##Dlt, (HCAB)(hcab), ctmDlt##Dlt, (fdlg), hParent, hinst, rgpfn, wRef)
#define MsoPerformModelessDlg(Dlt, hcab, fdlg, hParent, pmdlc, hinst, rgpfn, wRef) \
	MsoStartModelessDlt((long)dlt##Dlt, (HCAB)(hcab), ctmDlt##Dlt, (fdlg), hParent, pmdlc, hinst, rgpfn, wRef, fFalse)

// Dialog Control.
MSOAPI_(UINT_SDM)	IdDoMsgBox(const WCHAR *, const WCHAR *, UINT_SDM);
MSOAPI_(BOOL_SDM)	FSetDlgSab(SAB_SDM);

// Flags for bringing up a dialog.
#define msogrfHdlgStartDefault        0x00000000 // Default settings.
#define msogrfHdlgStartWorkPane       0x00000001 // Start a WorkPane dialog.

#define msogrfHdlgStartRepositionLeft 0x00000010 // For AutoLayout dialogs only:  The
#define msogrfHdlgStartRepositionTop  0x00000100 // given position of the dialog is
                                                 // actually the center point of the
                                                 // dialog, so the dialog will need
                                                 // to be repositioned around that
                                                 // point once the size of the dialog
                                                 // is known.  Not pretty, I know, but
                                                 // it's the best we could come up
                                                 // with to allow for sticky points
                                                 // in DAL dialogs.  NB:  If the
                                                 // bdrAutoPosX border style is set
                                                 // for the dialog, it doesn't make
                                                 // sense to make use of
                                                 // msogrfHdlgStartRepositionLeft,
                                                 // and if the bdrAutoPosY
                                                 // border style is set for the
                                                 // dialog, it doesn't make sense
                                                 // to make use of
                                                 // msogrfHdlgStartRepositionTop.
                                                 

MSOAPI_(TMC) TmcDoDlgDliEx(struct _dlt **, HCAB hcab, DLI *pdli, DWORD grfHdlgStart);
MSOAPIX_(TMC)	TmcDoDlg(struct _dlt * *, HCAB, BYTE *);
MSOAPI_(TMC)	TmcDoDlgDli(struct _dlt * *, HCAB, DLI *);
MSOAPI_(HDLG)	HdlgStartDlg(struct _dlt * *, HCAB, DLI *);
MSOAPI_(HDLG) HdlgStartDlgEx(struct _dlt * *, HCAB, DLI *, DWORD);

#ifdef VSMSODEBUG
MSOAPIXX_(BOOL_SDM) MsoFWriteDlgBe(HDLG hdlg, LPARAM lParam);
#endif
MSOAPI_(VOID)	EndDlg(TMC);
MSOAPIX_(TMC)	TmcEndedDlg(VOID);
MSOAPI_(BOOL_SDM)	FFreeDlg(void);
MSOAPI_(SAB_SDM)	SabGetDlg(void);
MSOAPI_(SAB_SDM)	MsoSabGetDlgEx(HDLG hdlg);
MSOAPI_(UINT_SDM) MsoItmBaseGetDlgTmc(HDLG hdlg, TMC tmc);

MSOAPI_(VOID)	SetTmcVal(TMC, UINT_SDM);
MSOAPI_(UINT_SDM)	ValGetTmc(TMC);
MSOAPI_(VOID)	GetTmcLargeVal(TMC, VOID *, UINT_SDM);
MSOAPI_(BOOL_SDM)	FSetTmcLargeVal(TMC, VOID *);
MSOAPI_(VOID)	SetTmcText(TMC, const WCHAR *);
MSOAPI_(VOID)	SetTmcTextNoRedraw(TMC, const WCHAR *);
MSOAPI_(VOID)	GetTmcText(TMC, WCHAR *, UINT_SDM);
MSOAPI_(UINT_SDM)	CchGetTmcText(TMC, WCHAR *, UINT_SDM);
MSOAPI_(VOID) SetTmcTextFromDw(TMC tmc, DWORD dw);
MSOAPI_(DWORD) DwFromTmcText(TMC tmc);

MSOAPI_(BOOL_SDM)	SetFocusTmc(TMC);
#define FSetFocusTmc(tmc) SetFocusTmc(tmc)

MSOAPI_(BOOL_SDM)	SetFocusTmcEx(TMC tmc, DWORD msogrfSFT);

// Nothing
#define msogrfSFTNone 		0

// Use when you want to move focus because of a mouse click. Will stop scrolling
// the control into view if in the workpane.
#define msogrfSFTClick		1
#define msogrfSFTNoScroll	msogrfSFTClick 
#define msogrfSFTAll		msogrfSFTNone | msogrfSFTClick | msogrfSFTNoScroll

MSOAPI_(TMC)	TmcGetFocus(VOID);
MSOAPI_(TMC)	TmcGetAccelerator(UINT_SDM);

MSOAPIX_(VOID)	SpinTmc(TMC, UINT_SDM, INT_SDM, INT_SDM, INT_SDM);
MSOAPI_(VOID)	SetReadOnlyTmc(TMC, BOOL_SDM);

MSOAPIMX_(BOOL_SDM)	FIsMultiLineEditTmc(TMC);
MSOAPI_(VOID)	SetTmcTxs(TMC, TXS);
MSOAPIMX_(VOID) SetTmcTxsNoFocus(TMC tmc, TXS txs);
MSOAPI_(TXS)	TxsGetTmc(TMC);
MSOAPI_(VOID)	RedisplayTmc(TMC);
MSOAPIMX_(VOID) MsoInvalidateTmc(TMC tmc, BOOL fErase);

MSOAPI_(BOOL_SDM)	FEnabledTmc(TMC);
MSOAPI_(VOID)	EnableTmc(TMC, BOOL_SDM);
MSOAPIX_(VOID)	AdminEnableTmc(TMC, BOOL_SDM);
MSOAPIX_(VOID)	EnableNoninteractiveTmc(TMC, BOOL_SDM);

MSOAPI_(U32_SDM)	LUserFromTmc(TMC);
MSOAPI_(VOID)	SetTmcLUser(TMC, U32_SDM);

#define SetScrollRangeTmc(tmc, iMin, iMax) \
		SetScrollRange(WindowOfTmc(tmc), SB_CTL, iMin, iMax, fTrue)
#define GetScrollRangeTmc(tmc, piMin, piMax) \
		GetScrollRange(WindowOfTmc(tmc), SB_CTL, piMin, piMax)
#define SetScrollPosTmc(tmc, iPos) \
		SetScrollPos(WindowOfTmc(tmc), SB_CTL, iPos)
MSOAPI_(VOID)	SetScrollPageTmc(INT_SDM nPage, TMC tmc);
MSOAPI_(HWND) WindowOfDlgCur(void);

MSOAPI_(VOID) CaptureMouseTmc(TMC, BOOL_SDM);

MSOAPIMX_(VOID) SetLightFontTmc(TMC, BOOL_SDM);

MSOAPIX_(PFN_FILTERMSG) PfnSetPfnFilterMsg(PFN_FILTERMSG);
MSOAPI_(PFN_DLGCREATED) MsoPfnSetDlgCreated(PFN_DLGCREATED);

MSOAPIX_(VOID)	SetModeBiasTmc(TMC, BOOL_SDM);
MSOAPIX_(VOID)	SetTmcSpecialTab(TMC tmc, BOOL fFlag);

///////////////////////////////////////////////////////////////////////////////
// Rare Calls.

// CAB Control.
MSOAPI_(HCAB)	HcabFromDlg(BOOL_SDM);
MSOAPI_(VOID)	NinchCab(HCAB);
MSOAPI_(VOID)	MsoZeroCab(HCAB);

// Dialog Control.
MSOAPI_(HDLG)	HdlgSetCurDlg(HDLG);
MSOAPI_(VOID)	ShowDlg(BOOL_SDM);
MSOAPI_(BOOL_SDM)	FVisibleDlg(void);
MSOAPI_(VOID)	ResizeDlg(XY_SDM, XY_SDM);
MSOAPI_(VOID)	ResizeDlgEx(XY_SDM, XY_SDM, BOOL_SDM);

MSOAPI_(VOID)	MoveDlg(XY_SDM, XY_SDM);
MSOAPI_(VOID)	SdmScaleRec(REC *);
MSOAPI_(HDLG)	HdlgSetFocusDlg(HDLG);
MSOAPI_(VOID)	SetTabOrder(TMC *, UINT_SDM);
MSOAPI_(int) 	SwapTabOrder(TMC tmc1, TMC tmc2);
MSOAPI_(VOID)	UpdateWindowDlg(HDLG);

MSOAPI_(VOID)	GetListBoxEntry(TMC, ILBE_SDM, WCHAR *, UINT_SDM);
MSOAPI_(UINT_SDM)	CchGetListBoxEntry(TMC, ILBE_SDM, WCHAR *, UINT_SDM);

MSOAPI_(VOID)	AddListBoxEntry(TMC, const WCHAR *);
MSOAPI_(VOID)	InsertListBoxEntry(TMC, const WCHAR *, ILBE_SDM);
MSOAPI_(VOID)	DeleteListBoxEntry(TMC, ILBE_SDM);
MSOAPI_(ILBE_SDM)	CentryListBoxTmc(TMC);
MSOAPI_(VOID)	StartListBoxUpdate(TMC);
MSOAPI_(VOID)	BeginListBoxUpdate(TMC, BOOL_SDM);
MSOAPI_(VOID)	EndListBoxUpdate(TMC);
MSOAPI_(ILBE_SDM)	IEntryFindListBox(TMC, const WCHAR *, UINT_SDM *);
MSOAPI_(ILBE_SDM)	IEntryListBoxCursorTmc(TMC);
MSOAPI_(BOOL) MsoFSetGalleryColumnWidth(HLBX hlbx, XY_SDM dxPreferred);
MSOAPI_(ILBE_SDM)	IselListBoxTmc(TMC);
MSOAPI_(BOOL) MsoFSetLastMruEntry(HLBX hlbx, ILBE_SDM ilbeMRU);
MSOAPIX_(BOOL) MsoFRemoveMruLine(HLBX hlbx, ILBE_SDM ilbeMRU);

// Set the height of a combo or droplist control's dropdown.
MSOAPI_(void) MsoSDMSetDropdownHeightLines(TMC tmc, int nLines);

MSOAPIMX_(BOOL_SDM) FExtendedTmmSet(BOOL_SDM);
MSOAPI_(HDLG) HdlgQueryCur(void);
MSOAPI_(HCAB) HcabQueryCur(void);
MSOAPI_(UINT_SDM) WRefQueryCur(void);
MSOAPI_(VOID)	EnsureVisibleDlgRec(REC *);
MSOAPIX_(TMC) TmcSetTmcContext(TMC tmc);
MSOAPI_(void) MsoSdmNotifyWinEvent(DWORD dwEvent, TMC tmc, BOOL fChildOnly);

// Command Bar Visuals ('new look')
MSOAPI_(int) MsoIHyperlinkIconMarginGet();
MSOAPIX_(VOID) MsoSdmCtrlCrGet(FTMS ftms, COLORREF *pcrBackground, COLORREF *pcrBorder, COLORREF *pcrText);
MSOAPIX_(VOID) MsoSdmCtrlCbvCrGet(FTMS ftms, int *pmsocbvcrBackground, int *pmsocbvcrBorder, int *pmsocbvcrText);

///////////////////////////////////////////////////////////////////////////////
// Very Rare.

MSOAPI_(BOOL) BidiDropsToLeft(TMC tmc);	// Test Wizard called helper API
MSOAPIX_(BOOL_SDM) MsoFExecuteTmc(TMC, UINT_SDM);
MSOAPI_(BOOL_SDM)	FSendDlm(DLM, TMC, UCBK_SDM, UCBK_SDM, UCBK_SDM);

MSOAPI_(BOOL_SDM)	FIsDialogWindow(HWND);

// CAB Control.
MSOAPI_(HCAB)	HcabDupeCab(HCAB, SB_SDM);

// Dialog Control.
MSOAPI_(BOOL_SDM)	FKillDlgFocus(void);
MSOAPI_(BOOL_SDM)	FModalDlg(HDLG);
MSOAPI_(BOOL_SDM)	FIsDlgDying(VOID);
MSOAPIX_(VOID)	ClearListError(HDLG);

MSOAPI_(ILBE_SDM)	CselListBoxTmc(TMC);
MSOAPIX_(TMV)	TmvGetTmc(TMC);
MSOAPI_(TMT)	TmtGetTmc(TMC);
MSOAPI_(BOOL_SDM)	FReturnDlgControl(TMC, BOOL_SDM);
MSOAPI_(VOID)	SetDefaultTmc(TMC);
MSOAPI_(TMC)	TmcGetDefault(BOOL_SDM);

MSOAPI_(TMC)	TmcGetDropped(VOID);
MSOAPI_(VOID)	GrowDropTmc(TMC);
#if SDMMSAA_TROBWIS_UPDATE
MSOAPIX_(VOID) ShrinkDropTmc(TMC tmc, BARG_SDM fSel);
#else
MSOAPIX_(VOID) ShrinkDropTmc(TMC tmc);
#endif

MSOAPI_(VOID)	SetSecretEditTmc(TMC);

MSOAPI_(VOID)	CompleteComboTmc(TMC);
MSOAPI_(VOID)	LimitTextTmc(TMC, UINT_SDM);
MSOAPI_(VOID)	SetVisibleTmc(TMC, BOOL_SDM);
MSOAPI_(BOOL_SDM)	FIsVisibleTmc(TMC);
MSOAPI_(VOID)	GetTmcRec(TMC, REC *);
MSOAPI_(VOID)	SetTmcRec(TMC, REC *);
MSOAPI_(int)    MsoSDMGetListboxBorder(TMC tmc);


MSOAPI_(HWND)	WindowOfTmc(TMC);

MSOAPIX_(BOOL_SDM)	FIsDlgInteractive(VOID);
MSOAPI_(VOID)	SetDlgCaption(CONST_WZ);

MSOAPI_(HLBX)	HlbxFromTmc(TMC);
MSOAPIX_(BOOL_SDM)	SetTmcFlbx(TMC, UINT_SDM);
MSOAPIX_(BOOL_SDM) GetTmcFlbx(TMC tmc, UINT_SDM  *pflbx);

MSOAPI_(HDLG)	HdlgFromWindow(HWND);
MSOAPI_(HWND)	WindowSwapSdmParent(HWND);
MSOAPI_(HWND)	WindowFromDlg(HDLG);

MSOAPI_(UINT_SDM)	CchGetTmc(TMC);
MSOAPIX_(VOID)	SetFdlgOfHdlgCur(FDLG);

MSOAPIX_(VOID)	SetEditTmcHandle(TMC, HANDLE);
MSOAPIX_(HANDLE)	HGetEditTmc(TMC);
MSOAPIX_(FARPROC)	LpfnSetEditFilter(FARPROC);

MSOAPI_(BOOL_SDM) FShrinkRefEdit(TMC);
MSOAPI_(BOOL_SDM) FDlgShrunk(VOID);
MSOAPI_(BOOL_SDM) MsoFSetSdmNotifyOnActivation(BOOL fNotifyOnActivation);

MSOAPI_(void) MsoSetDialogPositionViaFrame(BOOL fDialogPositionViaFrame);

///////////////////////////////////////////////////////////////////////////////
// Restore state.

MSOAPI_(BOOL_SDM)	FRestoreDlg(BOOL_SDM);
MSOAPI_(BOOL_SDM)	FRestoreTmc(TMC, BOOL_SDM);


///////////////////////////////////////////////////////////////////////////////
// EB/EL Support.

MSOAPIX_(BOOL_SDM)	FSetNoninteractive(UINT_SDM, TMC);

#endif //!NO_SDM


///////////////////////////////////////////////////////////////////////////////
// Common interface to button drawing code.

MSOAPIX_(VOID)	SdmBeginPaint(VOID);
MSOAPIX_(VOID)	SdmEndPaint(VOID);

// Draw a control's focus rect in SDM's own special way.
MSOAPI_(VOID) DrawSDMFocusRect(HDC hdc, RECT *pRect, COLORREF crFore, COLORREF crBkg);
MSOAPI_(VOID)	InvertCaret(HWND, HDC, XY_SDM, XY_SDM, XY_SDM, XY_SDM);
MSOAPI_(HANDLE) MsoHdibFromHbitmap(HBITMAP hbmp, HDC hdc, HPALETTE hpal);

// Draw the assistant button face
UCBK_SDM SDM_CALLBACK MsoDrawAsstBtn(TMM tmm, RDS *prds, FTMS ftmsNew,
									 FTMS ftmsOld, TMC tmc, UCBK_SDM wParam);

UCBK_SDM SDM_CALLBACK	WRenderPush(TMM, RDS *, FTMS, FTMS, TMC, UCBK_SDM);
UCBK_SDM SDM_CALLBACK	WRenderWorkPanePush(TMM tmm, RDS *prds, FTMS ftmsNew, FTMS ftmsOld, TMC tmc, UCBK_SDM wParam);
UCBK_SDM SDM_CALLBACK	WRenderStaticText(TMM, RDS *, FTMS, FTMS, TMC, UCBK_SDM);

MSOAPI_(void) MsoMeasureSDMWorkPanePush(TMC tmc, int *pWidth, int *pHeight);

// t-tomker: these functions are necessary for the Dialog Editor, but evidently
// nothing else, so they should only be exported in the internal (debug) library
#ifdef VSMSODEBUG
	MSOAPI_(UCBK_SDM) SDM_CALLBACK	WRenderRadio(TMM, RDS *, FTMS, FTMS, TMC, UCBK_SDM);
	MSOAPI_(UCBK_SDM) SDM_CALLBACK	WRenderCheck(TMM, RDS *, FTMS, FTMS, TMC, UCBK_SDM);
	MSOAPI_(UCBK_SDM) SDM_CALLBACK	WRenderGroup(TMM, RDS *, FTMS, FTMS, TMC, UCBK_SDM);
#else
	UCBK_SDM SDM_CALLBACK	WRenderRadio(TMM, RDS *, FTMS, FTMS, TMC, UCBK_SDM);
	UCBK_SDM SDM_CALLBACK	WRenderCheck(TMM, RDS *, FTMS, FTMS, TMC, UCBK_SDM);
	UCBK_SDM SDM_CALLBACK	WRenderGroup(TMM, RDS *, FTMS, FTMS, TMC, UCBK_SDM);
#endif

MSOAPIDBG_(UCBK_SDM) SDM_CALLBACK	WRenderDropIcon(TMM, RDS *, FTMS, FTMS, TMC, UCBK_SDM);
MSOAPIMX_(VOID) CalcDropIconRecGccRec( REC *oprecIcon, REC *oprecGcc );
MSOAPIXX_(VOID) SetPictureDropped(BOOL fDropped);

MSOAPI_(HFONT) HSdmDlgFontNormal(); //in dmfont.cpp
MSOAPI_(HFONT) GetHFontSdm( void );
MSOAPI_(HFONT) GetHFontLightSdm( void );
MSOAPI_(HFONT) GetHFontBoldSdm( void );


// Sdm Toolbar-like Button flags - see MsoWRenderAsTbButton()
#define msogrfstbNil				0x00000000
#define msofstbDrawDropdown	0x00000001 // Should we draw a dropdown arrow?
#define msofstbSplitDepressed	0x00000002 // Is the dropdown dropped?
#define msofstbNoBorder			0x00000004 // Draw no border (hyperlinks)
#define msofstbHyperlink		0x00000008 // Draw as hyperlink - underlined blue
#define msofstbNoCenter			0x00000010 // Left justify instead of centering
#define msofstbWPIconTextGap	0x00000020 // Uses ICON_TEXT_GAP spacing in drawing icons with text

MSOAPI_(UCBK_SDM) SDM_CALLBACK MsoWRenderAsTbButton(
	TMM			tmm,				// Passed by SDM
	RDS			*prds,				// Passed by SDM
	FTMS		ftmsNew,			// Passed by SDM
	FTMS		ftmsOld,			// Passed by SDM
	TMC			tmc,				// Passed by SDM
	UCBK_SDM	jc,					// Passed by SDM
	int			tcid,				// TCID of icon: msotcidNil for none
	HANDLE  	hdibCustom,			// handle to bitmap icon (HICON, BITMAPINFO, etc)
	HANDLE  	hmaskCustom,		// handle to bitmap icon mask (HICON, BITMAPINFO, etc)
	WCHAR		*wtzLabel,			// Button text
	DWORD		grfstb);			// see msofstb flags

// Sets the icon for the given pushbutton control
MSOAPI_(BOOL) MsoFSetTmcIcon(TMC tmc, int msotcid, BOOL fNoTooltipSet);

#ifdef IME32
MSOAPIX_(VOID) SdmRichEditCtrlIME(TMC, HWND);
MSOAPIX_(VOID) SdmSetImeHwnd(HWND);
MSOAPIX_(VOID) SetImeOption(BOOL_SDM, BOOL_SDM);
#endif //IME32
