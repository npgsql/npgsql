/*-----------------------------------------------------------------------------
 filename: msoiv.h
 summary : This file contains the #defines used for the Office 9 Instrumented
           version.
 last mod: July 9, 1998
------------------------------------------------------------------- t-benyu -*/
#ifndef MSOIV
#define MSOIV

#define PROFILE_ASSISTANT_QUERY 10
#define PROFILE_ALERT_MESSAGE	22
/*
   Format of Logging Callback (PFCNIVLoggingCallback)
   This typedef is also defined in Profiler.h of the Test Wizard project.
   Any changes made here must also be made in profiler.h
*/
typedef void (__cdecl *PFCNIVLoggingCallback)(DWORD dwTime, WORD wType, WORD wDataLength, void *pData);

#endif /* MSOIV */
