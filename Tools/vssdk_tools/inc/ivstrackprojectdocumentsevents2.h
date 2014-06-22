

/* this ALWAYS GENERATED file contains the definitions for the interfaces */


 /* File created by MIDL compiler version 7.00.0499 */
/* Compiler settings for IVsTrackProjectDocumentsEvents2.idl:
    Oicf, W0, Zp8, env=Win32 (32b run)
    protocol : dce , ms_ext, c_ext, robust
    error checks: allocation ref bounds_check enum stub_data 
    VC __declspec() decoration level: 
         __declspec(uuid()), __declspec(selectany), __declspec(novtable)
         DECLSPEC_UUID(), MIDL_INTERFACE()
*/
//@@MIDL_FILE_HEADING(  )

#pragma warning( disable: 4049 )  /* more than 64k source lines */


/* verify that the <rpcndr.h> version is high enough to compile this file*/
#ifndef __REQUIRED_RPCNDR_H_VERSION__
#define __REQUIRED_RPCNDR_H_VERSION__ 475
#endif

/* verify that the <rpcsal.h> version is high enough to compile this file*/
#ifndef __REQUIRED_RPCSAL_H_VERSION__
#define __REQUIRED_RPCSAL_H_VERSION__ 100
#endif

#include "rpc.h"
#include "rpcndr.h"

#ifndef __RPCNDR_H_VERSION__
#error this stub requires an updated version of <rpcndr.h>
#endif // __RPCNDR_H_VERSION__

#ifndef COM_NO_WINDOWS_H
#include "windows.h"
#include "ole2.h"
#endif /*COM_NO_WINDOWS_H*/

#ifndef __IVsTrackProjectDocumentsEvents2_h__
#define __IVsTrackProjectDocumentsEvents2_h__

#if defined(_MSC_VER) && (_MSC_VER >= 1020)
#pragma once
#endif

/* Forward Declarations */ 

#ifndef __IVsTrackProjectDocumentsEvents2_FWD_DEFINED__
#define __IVsTrackProjectDocumentsEvents2_FWD_DEFINED__
typedef interface IVsTrackProjectDocumentsEvents2 IVsTrackProjectDocumentsEvents2;
#endif 	/* __IVsTrackProjectDocumentsEvents2_FWD_DEFINED__ */


/* header files for imported files */
#include "oaidl.h"
#include "vsshell.h"

#ifdef __cplusplus
extern "C"{
#endif 


/* interface __MIDL_itf_IVsTrackProjectDocumentsEvents2_0000_0000 */
/* [local] */ 

#pragma once
#pragma once
typedef 
enum tagVSQUERYADDFILEFLAGS
    {	VSQUERYADDFILEFLAGS_NoFlags	= 0,
	VSQUERYADDFILEFLAGS_IsSpecialFile	= 1,
	VSQUERYADDFILEFLAGS_IsNestedProjectFile	= 2
    } 	VSQUERYADDFILEFLAGS;

typedef 
enum tagVSQUERYADDFILERESULTS
    {	VSQUERYADDFILERESULTS_AddOK	= 0,
	VSQUERYADDFILERESULTS_AddNotOK	= 1
    } 	VSQUERYADDFILERESULTS;

typedef 
enum tagVSADDFILEFLAGS
    {	VSADDFILEFLAGS_NoFlags	= 0,
	VSADDFILEFLAGS_AddToSourceControlDoneExternally	= 1,
	VSADDFILEFLAGS_IsSpecialFile	= 2,
	VSADDFILEFLAGS_IsNestedProjectFile	= 4
    } 	VSADDFILEFLAGS;

typedef 
enum tagVSQUERYADDDIRECTORYFLAGS
    {	VSQUERYADDDIRECTORYFLAGS_padding	= 0
    } 	VSQUERYADDDIRECTORYFLAGS;

typedef 
enum tagVSQUERYADDDIRECTORYRESULTS
    {	VSQUERYADDDIRECTORYRESULTS_AddOK	= 0,
	VSQUERYADDDIRECTORYRESULTS_AddNotOK	= 1
    } 	VSQUERYADDDIRECTORYRESULTS;

typedef 
enum tagVSADDDIRECTORYFLAGS
    {	VSADDDIRECTORYFLAGS_NoFlags	= 0,
	VSADDDIRECTORYFLAGS_AddToSourceControlDoneExternally	= 1
    } 	VSADDDIRECTORYFLAGS;

typedef 
enum VSQUERYREMOVEFILEFLAGS
    {	VSQUERYREMOVEFILEFLAGS_NoFlags	= 0,
	VSQUERYREMOVEFILEFLAGS_IsSpecialFile	= 1,
	VSQUERYREMOVEFILEFLAGS_IsNestedProjectFile	= 2
    } 	VSQUERYREMOVEFILEFLAGS;

typedef 
enum tagVSQUERYREMOVEFILERESULTS
    {	VSQUERYREMOVEFILERESULTS_RemoveOK	= 0,
	VSQUERYREMOVEFILERESULTS_RemoveNotOK	= 1
    } 	VSQUERYREMOVEFILERESULTS;

typedef 
enum tagVSREMOVEFILEFLAGS
    {	VSREMOVEFILEFLAGS_NoFlags	= 0,
	VSREMOVEFILEFLAGS_IsDirectoryBased	= 1,
	VSREMOVEFILEFLAGS_RemoveFromSourceControlDoneExternally	= 2,
	VSREMOVEFILEFLAGS_IsSpecialFile	= 4,
	VSREMOVEFILEFLAGS_IsNestedProjectFile	= 8
    } 	VSREMOVEFILEFLAGS;

typedef 
enum tagVSQUERYREMOVEDIRECTORYFLAGS
    {	VSQUERYREMOVEDIRECTORYFLAGS_padding	= 0
    } 	VSQUERYREMOVEDIRECTORYFLAGS;

typedef 
enum tagVSQUERYREMOVEDIRECTORYRESULTS
    {	VSQUERYREMOVEDIRECTORYRESULTS_RemoveOK	= 0,
	VSQUERYREMOVEDIRECTORYRESULTS_RemoveNotOK	= 1
    } 	VSQUERYREMOVEDIRECTORYRESULTS;

typedef 
enum VSREMOVEDIRECTORYFLAGS
    {	VSREMOVEDIRECTORYFLAGS_NoFlags	= 0,
	VSREMOVEDIRECTORYFLAGS_IsDirectoryBased	= 1,
	VSREMOVEDIRECTORYFLAGS_RemoveFromSourceControlDoneExternally	= 2
    } 	VSREMOVEDIRECTORYFLAGS;

typedef 
enum VSQUERYRENAMEFILEFLAGS
    {	VSQUERYRENAMEFILEFLAGS_NoFlags	= 0,
	VSQUERYRENAMEFILEFLAGS_IsSpecialFile	= 1,
	VSQUERYRENAMEFILEFLAGS_IsNestedProjectFile	= 2,
	VSQUERYRENAMEFILEFLAGS_Directory	= 0x20
    } 	VSQUERYRENAMEFILEFLAGS;

typedef 
enum tagVSQUERYRENAMEFILERESULTS
    {	VSQUERYRENAMEFILERESULTS_RenameOK	= 0,
	VSQUERYRENAMEFILERESULTS_RenameNotOK	= 1
    } 	VSQUERYRENAMEFILERESULTS;

typedef 
enum tagVSRENAMEFILEFLAGS
    {	VSRENAMEFILEFLAGS_NoFlags	= 0,
	VSRENAMEFILEFLAGS_FromShellCommand	= 0x1,
	VSRENAMEFILEFLAGS_FromScc	= 0x2,
	VSRENAMEFILEFLAGS_FromFileChange	= 0x4,
	VSRENAMEFILEFLAGS_WasQueried	= 0x8,
	VSRENAMEFILEFLAGS_AlreadyOnDisk	= 0x10,
	VSRENAMEFILEFLAGS_Directory	= 0x20,
	VSRENAMEFILEFLAGS_RenameInSourceControlDoneExternally	= 0x40,
	VSRENAMEFILEFLAGS_IsSpecialFile	= 0x80,
	VSRENAMEFILEFLAGS_IsNestedProjectFile	= 0x100,
	VSRENAMEFILEFLAGS_ALL	= 0x1ff,
	VSRENAMEFILEFLAGS_INVALID	= 0xfffffe00
    } 	VSRENAMEFILEFLAGS;

typedef 
enum tagVSQUERYRENAMEDIRECTORYFLAGS
    {	VSQUERYRENAMEDIRECTORYFLAGS_padding	= 0
    } 	VSQUERYRENAMEDIRECTORYFLAGS;

typedef 
enum tagVSQUERYRENAMEDIRECTORYRESULTS
    {	VSQUERYRENAMEDIRECTORYRESULTS_RenameOK	= 0,
	VSQUERYRENAMEDIRECTORYRESULTS_RenameNotOK	= 1
    } 	VSQUERYRENAMEDIRECTORYRESULTS;

typedef 
enum tagVSRENAMEDIRECTORYFLAGS
    {	VSRENAMEDIRECTORYFLAGS_NoFlags	= 0,
	VSRENAMEDIRECTORYFLAGS_RenameInSourceControlDoneExternally	= 1
    } 	VSRENAMEDIRECTORYFLAGS;



extern RPC_IF_HANDLE __MIDL_itf_IVsTrackProjectDocumentsEvents2_0000_0000_v0_0_c_ifspec;
extern RPC_IF_HANDLE __MIDL_itf_IVsTrackProjectDocumentsEvents2_0000_0000_v0_0_s_ifspec;

#ifndef __IVsTrackProjectDocumentsEvents2_INTERFACE_DEFINED__
#define __IVsTrackProjectDocumentsEvents2_INTERFACE_DEFINED__

/* interface IVsTrackProjectDocumentsEvents2 */
/* [object][uuid] */ 


EXTERN_C const IID IID_IVsTrackProjectDocumentsEvents2;

#if defined(__cplusplus) && !defined(CINTERFACE)
    
    MIDL_INTERFACE("53544c4d-663d-11d3-a60d-005004775ab1")
    IVsTrackProjectDocumentsEvents2 : public IUnknown
    {
    public:
        virtual HRESULT STDMETHODCALLTYPE OnQueryAddFiles( 
            /* [in] */ __RPC__in_opt IVsProject *pProject,
            /* [in] */ int cFiles,
            /* [size_is][in] */ __RPC__in_ecount_full(cFiles) const LPCOLESTR rgpszMkDocuments[  ],
            /* [size_is][in] */ __RPC__in_ecount_full(cFiles) const VSQUERYADDFILEFLAGS rgFlags[  ],
            /* [out] */ __RPC__out VSQUERYADDFILERESULTS *pSummaryResult,
            /* [size_is][out] */ __RPC__out_ecount_full(cFiles) VSQUERYADDFILERESULTS rgResults[  ]) = 0;
        
        virtual HRESULT STDMETHODCALLTYPE OnAfterAddFilesEx( 
            /* [in] */ int cProjects,
            /* [in] */ int cFiles,
            /* [size_is][in] */ __RPC__in_ecount_full(cProjects) IVsProject *rgpProjects[  ],
            /* [size_is][in] */ __RPC__in_ecount_full(cProjects) const int rgFirstIndices[  ],
            /* [size_is][in] */ __RPC__in_ecount_full(cFiles) const LPCOLESTR rgpszMkDocuments[  ],
            /* [size_is][in] */ __RPC__in_ecount_full(cFiles) const VSADDFILEFLAGS rgFlags[  ]) = 0;
        
        virtual HRESULT STDMETHODCALLTYPE OnAfterAddDirectoriesEx( 
            /* [in] */ int cProjects,
            /* [in] */ int cDirectories,
            /* [size_is][in] */ __RPC__in_ecount_full(cProjects) IVsProject *rgpProjects[  ],
            /* [size_is][in] */ __RPC__in_ecount_full(cProjects) const int rgFirstIndices[  ],
            /* [size_is][in] */ __RPC__in_ecount_full(cDirectories) const LPCOLESTR rgpszMkDocuments[  ],
            /* [size_is][in] */ __RPC__in_ecount_full(cDirectories) const VSADDDIRECTORYFLAGS rgFlags[  ]) = 0;
        
        virtual HRESULT STDMETHODCALLTYPE OnAfterRemoveFiles( 
            /* [in] */ int cProjects,
            /* [in] */ int cFiles,
            /* [size_is][in] */ __RPC__in_ecount_full(cProjects) IVsProject *rgpProjects[  ],
            /* [size_is][in] */ __RPC__in_ecount_full(cProjects) const int rgFirstIndices[  ],
            /* [size_is][in] */ __RPC__in_ecount_full(cFiles) const LPCOLESTR rgpszMkDocuments[  ],
            /* [size_is][in] */ __RPC__in_ecount_full(cFiles) const VSREMOVEFILEFLAGS rgFlags[  ]) = 0;
        
        virtual HRESULT STDMETHODCALLTYPE OnAfterRemoveDirectories( 
            /* [in] */ int cProjects,
            /* [in] */ int cDirectories,
            /* [size_is][in] */ __RPC__in_ecount_full(cProjects) IVsProject *rgpProjects[  ],
            /* [size_is][in] */ __RPC__in_ecount_full(cProjects) const int rgFirstIndices[  ],
            /* [size_is][in] */ __RPC__in_ecount_full(cDirectories) const LPCOLESTR rgpszMkDocuments[  ],
            /* [size_is][in] */ __RPC__in_ecount_full(cDirectories) const VSREMOVEDIRECTORYFLAGS rgFlags[  ]) = 0;
        
        virtual HRESULT STDMETHODCALLTYPE OnQueryRenameFiles( 
            /* [in] */ __RPC__in_opt IVsProject *pProject,
            /* [in] */ int cFiles,
            /* [size_is][in] */ __RPC__in_ecount_full(cFiles) const LPCOLESTR rgszMkOldNames[  ],
            /* [size_is][in] */ __RPC__in_ecount_full(cFiles) const LPCOLESTR rgszMkNewNames[  ],
            /* [size_is][in] */ __RPC__in_ecount_full(cFiles) const VSQUERYRENAMEFILEFLAGS rgflags[  ],
            /* [out] */ __RPC__out VSQUERYRENAMEFILERESULTS *pSummaryResult,
            /* [size_is][out] */ __RPC__out_ecount_full(cFiles) VSQUERYRENAMEFILERESULTS rgResults[  ]) = 0;
        
        virtual HRESULT STDMETHODCALLTYPE OnAfterRenameFiles( 
            /* [in] */ int cProjects,
            /* [in] */ int cFiles,
            /* [size_is][in] */ __RPC__in_ecount_full(cProjects) IVsProject *rgpProjects[  ],
            /* [size_is][in] */ __RPC__in_ecount_full(cProjects) const int rgFirstIndices[  ],
            /* [size_is][in] */ __RPC__in_ecount_full(cFiles) const LPCOLESTR rgszMkOldNames[  ],
            /* [size_is][in] */ __RPC__in_ecount_full(cFiles) const LPCOLESTR rgszMkNewNames[  ],
            /* [size_is][in] */ __RPC__in_ecount_full(cFiles) const VSRENAMEFILEFLAGS rgflags[  ]) = 0;
        
        virtual HRESULT STDMETHODCALLTYPE OnQueryRenameDirectories( 
            /* [in] */ __RPC__in_opt IVsProject *pProject,
            /* [in] */ int cDirs,
            /* [size_is][in] */ __RPC__in_ecount_full(cDirs) const LPCOLESTR rgszMkOldNames[  ],
            /* [size_is][in] */ __RPC__in_ecount_full(cDirs) const LPCOLESTR rgszMkNewNames[  ],
            /* [size_is][in] */ __RPC__in_ecount_full(cDirs) const VSQUERYRENAMEDIRECTORYFLAGS rgflags[  ],
            /* [out] */ __RPC__out VSQUERYRENAMEDIRECTORYRESULTS *pSummaryResult,
            /* [size_is][out] */ __RPC__out_ecount_full(cDirs) VSQUERYRENAMEDIRECTORYRESULTS rgResults[  ]) = 0;
        
        virtual HRESULT STDMETHODCALLTYPE OnAfterRenameDirectories( 
            /* [in] */ int cProjects,
            /* [in] */ int cDirs,
            /* [size_is][in] */ __RPC__in_ecount_full(cProjects) IVsProject *rgpProjects[  ],
            /* [size_is][in] */ __RPC__in_ecount_full(cProjects) const int rgFirstIndices[  ],
            /* [size_is][in] */ __RPC__in_ecount_full(cDirs) const LPCOLESTR rgszMkOldNames[  ],
            /* [size_is][in] */ __RPC__in_ecount_full(cDirs) const LPCOLESTR rgszMkNewNames[  ],
            /* [size_is][in] */ __RPC__in_ecount_full(cDirs) const VSRENAMEDIRECTORYFLAGS rgflags[  ]) = 0;
        
        virtual HRESULT STDMETHODCALLTYPE OnQueryAddDirectories( 
            /* [in] */ __RPC__in_opt IVsProject *pProject,
            /* [in] */ int cDirectories,
            /* [size_is][in] */ __RPC__in_ecount_full(cDirectories) const LPCOLESTR rgpszMkDocuments[  ],
            /* [size_is][in] */ __RPC__in_ecount_full(cDirectories) const VSQUERYADDDIRECTORYFLAGS rgFlags[  ],
            /* [out] */ __RPC__out VSQUERYADDDIRECTORYRESULTS *pSummaryResult,
            /* [size_is][out] */ __RPC__out_ecount_full(cDirectories) VSQUERYADDDIRECTORYRESULTS rgResults[  ]) = 0;
        
        virtual HRESULT STDMETHODCALLTYPE OnQueryRemoveFiles( 
            /* [in] */ __RPC__in_opt IVsProject *pProject,
            /* [in] */ int cFiles,
            /* [size_is][in] */ __RPC__in_ecount_full(cFiles) const LPCOLESTR rgpszMkDocuments[  ],
            /* [size_is][in] */ __RPC__in_ecount_full(cFiles) const VSQUERYREMOVEFILEFLAGS rgFlags[  ],
            /* [out] */ __RPC__out VSQUERYREMOVEFILERESULTS *pSummaryResult,
            /* [size_is][out] */ __RPC__out_ecount_full(cFiles) VSQUERYREMOVEFILERESULTS rgResults[  ]) = 0;
        
        virtual HRESULT STDMETHODCALLTYPE OnQueryRemoveDirectories( 
            /* [in] */ __RPC__in_opt IVsProject *pProject,
            /* [in] */ int cDirectories,
            /* [size_is][in] */ __RPC__in_ecount_full(cDirectories) const LPCOLESTR rgpszMkDocuments[  ],
            /* [size_is][in] */ __RPC__in_ecount_full(cDirectories) const VSQUERYREMOVEDIRECTORYFLAGS rgFlags[  ],
            /* [out] */ __RPC__out VSQUERYREMOVEDIRECTORYRESULTS *pSummaryResult,
            /* [size_is][out] */ __RPC__out_ecount_full(cDirectories) VSQUERYREMOVEDIRECTORYRESULTS rgResults[  ]) = 0;
        
        virtual HRESULT STDMETHODCALLTYPE OnAfterSccStatusChanged( 
            /* [in] */ int cProjects,
            /* [in] */ int cFiles,
            /* [size_is][in] */ __RPC__in_ecount_full(cProjects) IVsProject *rgpProjects[  ],
            /* [size_is][in] */ __RPC__in_ecount_full(cProjects) const int rgFirstIndices[  ],
            /* [size_is][in] */ __RPC__in_ecount_full(cFiles) const LPCOLESTR rgpszMkDocuments[  ],
            /* [size_is][in] */ __RPC__in_ecount_full(cFiles) const DWORD rgdwSccStatus[  ]) = 0;
        
    };
    
#else 	/* C style interface */

    typedef struct IVsTrackProjectDocumentsEvents2Vtbl
    {
        BEGIN_INTERFACE
        
        HRESULT ( STDMETHODCALLTYPE *QueryInterface )( 
            IVsTrackProjectDocumentsEvents2 * This,
            /* [in] */ __RPC__in REFIID riid,
            /* [iid_is][out] */ 
            __RPC__deref_out  void **ppvObject);
        
        ULONG ( STDMETHODCALLTYPE *AddRef )( 
            IVsTrackProjectDocumentsEvents2 * This);
        
        ULONG ( STDMETHODCALLTYPE *Release )( 
            IVsTrackProjectDocumentsEvents2 * This);
        
        HRESULT ( STDMETHODCALLTYPE *OnQueryAddFiles )( 
            IVsTrackProjectDocumentsEvents2 * This,
            /* [in] */ __RPC__in_opt IVsProject *pProject,
            /* [in] */ int cFiles,
            /* [size_is][in] */ __RPC__in_ecount_full(cFiles) const LPCOLESTR rgpszMkDocuments[  ],
            /* [size_is][in] */ __RPC__in_ecount_full(cFiles) const VSQUERYADDFILEFLAGS rgFlags[  ],
            /* [out] */ __RPC__out VSQUERYADDFILERESULTS *pSummaryResult,
            /* [size_is][out] */ __RPC__out_ecount_full(cFiles) VSQUERYADDFILERESULTS rgResults[  ]);
        
        HRESULT ( STDMETHODCALLTYPE *OnAfterAddFilesEx )( 
            IVsTrackProjectDocumentsEvents2 * This,
            /* [in] */ int cProjects,
            /* [in] */ int cFiles,
            /* [size_is][in] */ __RPC__in_ecount_full(cProjects) IVsProject *rgpProjects[  ],
            /* [size_is][in] */ __RPC__in_ecount_full(cProjects) const int rgFirstIndices[  ],
            /* [size_is][in] */ __RPC__in_ecount_full(cFiles) const LPCOLESTR rgpszMkDocuments[  ],
            /* [size_is][in] */ __RPC__in_ecount_full(cFiles) const VSADDFILEFLAGS rgFlags[  ]);
        
        HRESULT ( STDMETHODCALLTYPE *OnAfterAddDirectoriesEx )( 
            IVsTrackProjectDocumentsEvents2 * This,
            /* [in] */ int cProjects,
            /* [in] */ int cDirectories,
            /* [size_is][in] */ __RPC__in_ecount_full(cProjects) IVsProject *rgpProjects[  ],
            /* [size_is][in] */ __RPC__in_ecount_full(cProjects) const int rgFirstIndices[  ],
            /* [size_is][in] */ __RPC__in_ecount_full(cDirectories) const LPCOLESTR rgpszMkDocuments[  ],
            /* [size_is][in] */ __RPC__in_ecount_full(cDirectories) const VSADDDIRECTORYFLAGS rgFlags[  ]);
        
        HRESULT ( STDMETHODCALLTYPE *OnAfterRemoveFiles )( 
            IVsTrackProjectDocumentsEvents2 * This,
            /* [in] */ int cProjects,
            /* [in] */ int cFiles,
            /* [size_is][in] */ __RPC__in_ecount_full(cProjects) IVsProject *rgpProjects[  ],
            /* [size_is][in] */ __RPC__in_ecount_full(cProjects) const int rgFirstIndices[  ],
            /* [size_is][in] */ __RPC__in_ecount_full(cFiles) const LPCOLESTR rgpszMkDocuments[  ],
            /* [size_is][in] */ __RPC__in_ecount_full(cFiles) const VSREMOVEFILEFLAGS rgFlags[  ]);
        
        HRESULT ( STDMETHODCALLTYPE *OnAfterRemoveDirectories )( 
            IVsTrackProjectDocumentsEvents2 * This,
            /* [in] */ int cProjects,
            /* [in] */ int cDirectories,
            /* [size_is][in] */ __RPC__in_ecount_full(cProjects) IVsProject *rgpProjects[  ],
            /* [size_is][in] */ __RPC__in_ecount_full(cProjects) const int rgFirstIndices[  ],
            /* [size_is][in] */ __RPC__in_ecount_full(cDirectories) const LPCOLESTR rgpszMkDocuments[  ],
            /* [size_is][in] */ __RPC__in_ecount_full(cDirectories) const VSREMOVEDIRECTORYFLAGS rgFlags[  ]);
        
        HRESULT ( STDMETHODCALLTYPE *OnQueryRenameFiles )( 
            IVsTrackProjectDocumentsEvents2 * This,
            /* [in] */ __RPC__in_opt IVsProject *pProject,
            /* [in] */ int cFiles,
            /* [size_is][in] */ __RPC__in_ecount_full(cFiles) const LPCOLESTR rgszMkOldNames[  ],
            /* [size_is][in] */ __RPC__in_ecount_full(cFiles) const LPCOLESTR rgszMkNewNames[  ],
            /* [size_is][in] */ __RPC__in_ecount_full(cFiles) const VSQUERYRENAMEFILEFLAGS rgflags[  ],
            /* [out] */ __RPC__out VSQUERYRENAMEFILERESULTS *pSummaryResult,
            /* [size_is][out] */ __RPC__out_ecount_full(cFiles) VSQUERYRENAMEFILERESULTS rgResults[  ]);
        
        HRESULT ( STDMETHODCALLTYPE *OnAfterRenameFiles )( 
            IVsTrackProjectDocumentsEvents2 * This,
            /* [in] */ int cProjects,
            /* [in] */ int cFiles,
            /* [size_is][in] */ __RPC__in_ecount_full(cProjects) IVsProject *rgpProjects[  ],
            /* [size_is][in] */ __RPC__in_ecount_full(cProjects) const int rgFirstIndices[  ],
            /* [size_is][in] */ __RPC__in_ecount_full(cFiles) const LPCOLESTR rgszMkOldNames[  ],
            /* [size_is][in] */ __RPC__in_ecount_full(cFiles) const LPCOLESTR rgszMkNewNames[  ],
            /* [size_is][in] */ __RPC__in_ecount_full(cFiles) const VSRENAMEFILEFLAGS rgflags[  ]);
        
        HRESULT ( STDMETHODCALLTYPE *OnQueryRenameDirectories )( 
            IVsTrackProjectDocumentsEvents2 * This,
            /* [in] */ __RPC__in_opt IVsProject *pProject,
            /* [in] */ int cDirs,
            /* [size_is][in] */ __RPC__in_ecount_full(cDirs) const LPCOLESTR rgszMkOldNames[  ],
            /* [size_is][in] */ __RPC__in_ecount_full(cDirs) const LPCOLESTR rgszMkNewNames[  ],
            /* [size_is][in] */ __RPC__in_ecount_full(cDirs) const VSQUERYRENAMEDIRECTORYFLAGS rgflags[  ],
            /* [out] */ __RPC__out VSQUERYRENAMEDIRECTORYRESULTS *pSummaryResult,
            /* [size_is][out] */ __RPC__out_ecount_full(cDirs) VSQUERYRENAMEDIRECTORYRESULTS rgResults[  ]);
        
        HRESULT ( STDMETHODCALLTYPE *OnAfterRenameDirectories )( 
            IVsTrackProjectDocumentsEvents2 * This,
            /* [in] */ int cProjects,
            /* [in] */ int cDirs,
            /* [size_is][in] */ __RPC__in_ecount_full(cProjects) IVsProject *rgpProjects[  ],
            /* [size_is][in] */ __RPC__in_ecount_full(cProjects) const int rgFirstIndices[  ],
            /* [size_is][in] */ __RPC__in_ecount_full(cDirs) const LPCOLESTR rgszMkOldNames[  ],
            /* [size_is][in] */ __RPC__in_ecount_full(cDirs) const LPCOLESTR rgszMkNewNames[  ],
            /* [size_is][in] */ __RPC__in_ecount_full(cDirs) const VSRENAMEDIRECTORYFLAGS rgflags[  ]);
        
        HRESULT ( STDMETHODCALLTYPE *OnQueryAddDirectories )( 
            IVsTrackProjectDocumentsEvents2 * This,
            /* [in] */ __RPC__in_opt IVsProject *pProject,
            /* [in] */ int cDirectories,
            /* [size_is][in] */ __RPC__in_ecount_full(cDirectories) const LPCOLESTR rgpszMkDocuments[  ],
            /* [size_is][in] */ __RPC__in_ecount_full(cDirectories) const VSQUERYADDDIRECTORYFLAGS rgFlags[  ],
            /* [out] */ __RPC__out VSQUERYADDDIRECTORYRESULTS *pSummaryResult,
            /* [size_is][out] */ __RPC__out_ecount_full(cDirectories) VSQUERYADDDIRECTORYRESULTS rgResults[  ]);
        
        HRESULT ( STDMETHODCALLTYPE *OnQueryRemoveFiles )( 
            IVsTrackProjectDocumentsEvents2 * This,
            /* [in] */ __RPC__in_opt IVsProject *pProject,
            /* [in] */ int cFiles,
            /* [size_is][in] */ __RPC__in_ecount_full(cFiles) const LPCOLESTR rgpszMkDocuments[  ],
            /* [size_is][in] */ __RPC__in_ecount_full(cFiles) const VSQUERYREMOVEFILEFLAGS rgFlags[  ],
            /* [out] */ __RPC__out VSQUERYREMOVEFILERESULTS *pSummaryResult,
            /* [size_is][out] */ __RPC__out_ecount_full(cFiles) VSQUERYREMOVEFILERESULTS rgResults[  ]);
        
        HRESULT ( STDMETHODCALLTYPE *OnQueryRemoveDirectories )( 
            IVsTrackProjectDocumentsEvents2 * This,
            /* [in] */ __RPC__in_opt IVsProject *pProject,
            /* [in] */ int cDirectories,
            /* [size_is][in] */ __RPC__in_ecount_full(cDirectories) const LPCOLESTR rgpszMkDocuments[  ],
            /* [size_is][in] */ __RPC__in_ecount_full(cDirectories) const VSQUERYREMOVEDIRECTORYFLAGS rgFlags[  ],
            /* [out] */ __RPC__out VSQUERYREMOVEDIRECTORYRESULTS *pSummaryResult,
            /* [size_is][out] */ __RPC__out_ecount_full(cDirectories) VSQUERYREMOVEDIRECTORYRESULTS rgResults[  ]);
        
        HRESULT ( STDMETHODCALLTYPE *OnAfterSccStatusChanged )( 
            IVsTrackProjectDocumentsEvents2 * This,
            /* [in] */ int cProjects,
            /* [in] */ int cFiles,
            /* [size_is][in] */ __RPC__in_ecount_full(cProjects) IVsProject *rgpProjects[  ],
            /* [size_is][in] */ __RPC__in_ecount_full(cProjects) const int rgFirstIndices[  ],
            /* [size_is][in] */ __RPC__in_ecount_full(cFiles) const LPCOLESTR rgpszMkDocuments[  ],
            /* [size_is][in] */ __RPC__in_ecount_full(cFiles) const DWORD rgdwSccStatus[  ]);
        
        END_INTERFACE
    } IVsTrackProjectDocumentsEvents2Vtbl;

    interface IVsTrackProjectDocumentsEvents2
    {
        CONST_VTBL struct IVsTrackProjectDocumentsEvents2Vtbl *lpVtbl;
    };

    

#ifdef COBJMACROS


#define IVsTrackProjectDocumentsEvents2_QueryInterface(This,riid,ppvObject)	\
    ( (This)->lpVtbl -> QueryInterface(This,riid,ppvObject) ) 

#define IVsTrackProjectDocumentsEvents2_AddRef(This)	\
    ( (This)->lpVtbl -> AddRef(This) ) 

#define IVsTrackProjectDocumentsEvents2_Release(This)	\
    ( (This)->lpVtbl -> Release(This) ) 


#define IVsTrackProjectDocumentsEvents2_OnQueryAddFiles(This,pProject,cFiles,rgpszMkDocuments,rgFlags,pSummaryResult,rgResults)	\
    ( (This)->lpVtbl -> OnQueryAddFiles(This,pProject,cFiles,rgpszMkDocuments,rgFlags,pSummaryResult,rgResults) ) 

#define IVsTrackProjectDocumentsEvents2_OnAfterAddFilesEx(This,cProjects,cFiles,rgpProjects,rgFirstIndices,rgpszMkDocuments,rgFlags)	\
    ( (This)->lpVtbl -> OnAfterAddFilesEx(This,cProjects,cFiles,rgpProjects,rgFirstIndices,rgpszMkDocuments,rgFlags) ) 

#define IVsTrackProjectDocumentsEvents2_OnAfterAddDirectoriesEx(This,cProjects,cDirectories,rgpProjects,rgFirstIndices,rgpszMkDocuments,rgFlags)	\
    ( (This)->lpVtbl -> OnAfterAddDirectoriesEx(This,cProjects,cDirectories,rgpProjects,rgFirstIndices,rgpszMkDocuments,rgFlags) ) 

#define IVsTrackProjectDocumentsEvents2_OnAfterRemoveFiles(This,cProjects,cFiles,rgpProjects,rgFirstIndices,rgpszMkDocuments,rgFlags)	\
    ( (This)->lpVtbl -> OnAfterRemoveFiles(This,cProjects,cFiles,rgpProjects,rgFirstIndices,rgpszMkDocuments,rgFlags) ) 

#define IVsTrackProjectDocumentsEvents2_OnAfterRemoveDirectories(This,cProjects,cDirectories,rgpProjects,rgFirstIndices,rgpszMkDocuments,rgFlags)	\
    ( (This)->lpVtbl -> OnAfterRemoveDirectories(This,cProjects,cDirectories,rgpProjects,rgFirstIndices,rgpszMkDocuments,rgFlags) ) 

#define IVsTrackProjectDocumentsEvents2_OnQueryRenameFiles(This,pProject,cFiles,rgszMkOldNames,rgszMkNewNames,rgflags,pSummaryResult,rgResults)	\
    ( (This)->lpVtbl -> OnQueryRenameFiles(This,pProject,cFiles,rgszMkOldNames,rgszMkNewNames,rgflags,pSummaryResult,rgResults) ) 

#define IVsTrackProjectDocumentsEvents2_OnAfterRenameFiles(This,cProjects,cFiles,rgpProjects,rgFirstIndices,rgszMkOldNames,rgszMkNewNames,rgflags)	\
    ( (This)->lpVtbl -> OnAfterRenameFiles(This,cProjects,cFiles,rgpProjects,rgFirstIndices,rgszMkOldNames,rgszMkNewNames,rgflags) ) 

#define IVsTrackProjectDocumentsEvents2_OnQueryRenameDirectories(This,pProject,cDirs,rgszMkOldNames,rgszMkNewNames,rgflags,pSummaryResult,rgResults)	\
    ( (This)->lpVtbl -> OnQueryRenameDirectories(This,pProject,cDirs,rgszMkOldNames,rgszMkNewNames,rgflags,pSummaryResult,rgResults) ) 

#define IVsTrackProjectDocumentsEvents2_OnAfterRenameDirectories(This,cProjects,cDirs,rgpProjects,rgFirstIndices,rgszMkOldNames,rgszMkNewNames,rgflags)	\
    ( (This)->lpVtbl -> OnAfterRenameDirectories(This,cProjects,cDirs,rgpProjects,rgFirstIndices,rgszMkOldNames,rgszMkNewNames,rgflags) ) 

#define IVsTrackProjectDocumentsEvents2_OnQueryAddDirectories(This,pProject,cDirectories,rgpszMkDocuments,rgFlags,pSummaryResult,rgResults)	\
    ( (This)->lpVtbl -> OnQueryAddDirectories(This,pProject,cDirectories,rgpszMkDocuments,rgFlags,pSummaryResult,rgResults) ) 

#define IVsTrackProjectDocumentsEvents2_OnQueryRemoveFiles(This,pProject,cFiles,rgpszMkDocuments,rgFlags,pSummaryResult,rgResults)	\
    ( (This)->lpVtbl -> OnQueryRemoveFiles(This,pProject,cFiles,rgpszMkDocuments,rgFlags,pSummaryResult,rgResults) ) 

#define IVsTrackProjectDocumentsEvents2_OnQueryRemoveDirectories(This,pProject,cDirectories,rgpszMkDocuments,rgFlags,pSummaryResult,rgResults)	\
    ( (This)->lpVtbl -> OnQueryRemoveDirectories(This,pProject,cDirectories,rgpszMkDocuments,rgFlags,pSummaryResult,rgResults) ) 

#define IVsTrackProjectDocumentsEvents2_OnAfterSccStatusChanged(This,cProjects,cFiles,rgpProjects,rgFirstIndices,rgpszMkDocuments,rgdwSccStatus)	\
    ( (This)->lpVtbl -> OnAfterSccStatusChanged(This,cProjects,cFiles,rgpProjects,rgFirstIndices,rgpszMkDocuments,rgdwSccStatus) ) 

#endif /* COBJMACROS */


#endif 	/* C style interface */




#endif 	/* __IVsTrackProjectDocumentsEvents2_INTERFACE_DEFINED__ */


/* Additional Prototypes for ALL interfaces */

/* end of Additional Prototypes */

#ifdef __cplusplus
}
#endif

#endif


