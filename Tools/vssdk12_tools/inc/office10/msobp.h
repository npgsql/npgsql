/*------------------------------------------------------------------------------------------

	%%File: msobp.h
	%%Contact: EricFox

	MSO routines for bullet-proofing a document that might be corrupt.  Sniffs out
	corruption and tries to fix it.

------------------------------------------------------------------------------------------*/

#pragma once

#ifndef MSOBP_H
#define MSOBP_H


typedef enum // number of stack levels for Resetting the BPL Error watcher
	{
	bprNil = -1,
	bprMax = 4
	} BPR;


typedef enum	// BulletProofing Success Code
	{
	bpscNone = 0,	// no problem
	bpscFixed,
	bpscNotFixed,
	bpscOOM
	} BPSC;


typedef int MSOBPLEK;

#define msobplekNil               0
#define msobplekPv                1
#define msobplekH                 2
#define msobplekPl                3
#define msobplekPlc               4
#define msobplekSttb              5
#define msobplekPcd               6
#define msobplekBte               7
#define msobplekChp               8
#define msobplekPap               9
#define msobplekTableProps        10
#define msobplekTableChars        11
#define msobplekFieldChars        12
#define msobplekLists             13
#define msobplekStyles            14
#define msobplekOleFld            15
#define msobplekOleOcx            16
#define msobplekOleObjd           17
#define msobplekNumberedStyles    18
#define msobplekSections          19
#define msobplekFootnotes         20
#define msobplekEndnotes          21
#define msobplekComments          22
#define msobplekDrawnObjects      23
#define msobplekTextbox           24
#define msobplekFsr               25
#define msobplekRsl               26
#define msobplekUim               27
#define msobplekPictures          28
#define msobplekDivs              29
#define msobplekBookmarks         30
#define msobplekDataRecoveryOpen  31
#define msobplekEscher            32
#define msobplekPx                33
#define msobplekSumInfo           34
#define msobplekLinkedList        35
#define msobplekString            36
#define msobplekHyperlink         37

#define msobplekMax               38


typedef BPSC (*PFNBP)(struct _BPL *, void *, struct _SVI *);
typedef struct _MSOBPCB		// MSO BulletProofing CallBack Block
	{
	// main BP logging routine
	void			(CALLBACK *pfnLogFixBple)(BPSC *pbsc, struct _BPL *pbpl, MSOBPLEK msobplek);

	// BPR helper routines
	BPR				(CALLBACK *pfnBprResetBpl)(void *pbpl);
	int				(CALLBACK *pfnFSawErrorBpr)(void *pbpl, BPR bpr);
	void			(CALLBACK *pfnSawErrorBpl)(void *pbpl);

	// actual error log
	struct _BPL		*pbpl;
	} MSOBPCB;


// BPSC handling
MSOAPI_(void) MsoUpdateBpsc(BPSC *pbpsc, BPSC bpscNew);

// generic pointer routines
MSOAPI_(void) MsoSetStackLimits(void);		//  REVIEW:  PETERO:  Is this a real routine?
MSOAPI_(BOOL) MsoFTestCbPv(void *pv, int cb);
MSOAPI_(BOOL) MsoFTestCbPvRO(const void *pv, int cb);
MSOAPI_(int) MsoCbActualFromPv(void *pv, int cb);
MSOAPI_(BOOL) MsoFEnsureCbPv(void **ppv, int cb);
MSOAPI_(BOOL) MsoFTestH(void **ppv);
MSOAPI_(BOOL) MsoFTestWz(WCHAR *wz, int cwchMaxPossible);
MSOAPI_(BOOL) MsoFTestSz(char *sz, int cchMaxPossible);
MSOAPI_(BOOL) MsoFTestWt(WCHAR *wt);
MSOAPI_(BOOL) MsoFTestSt(char *st);
MSOAPI_(BOOL) MsoFTestWtz(WCHAR *wtz);
MSOAPI_(BOOL) MsoFTestStz(char *stz);
MSOAPI_(BPSC) MsoBpscBulletProofPx(void *pvPx, void *pmsobpcb, int cb);
MSOAPI_(BPSC) MsoBpscBulletProofLinkedList(MSOBPCB *pmsobpcb, BYTE **ppbHead, int cb, int bpNext, PFNBP pfnbp, struct _SVI *psvi);

#endif // !MSOBP_H

