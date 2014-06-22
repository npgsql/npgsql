//--------------------------------------------------------------------------
// Microsoft Visual Studio Sampling Profiler Driver
//
// Copyright (c) 2005 Microsoft Corporation Inc.
// All rights reserved
//
// PMUResourceManager.c
//
// PMU resource management.
//
//---------------------------------------------------------------------------
#include "PMUResourceManager.h"

/*
    Implementation Details:


    The following is an implementation of a cooperative, masterless resource
    management scheme.  In this case, the resource is the collection of 
    performance monitoring units (PMUs) on an SMP system.
    
    Resources are assigned to "Participants".  Participants may own one, 
    several, or all of the PMUs in a system.  Paricipants track their 
    own PMU usage.  When requesting PMU resources, a Participant queries all
    other Participants for their current usage.  If no other Participant 
    is using the resource, then the requestor takes ownership.
    
    Obviously, the above requires synchronization around acquisition and 
    relinquishment of the resources.  It also requires a guaranteed 
    communication mechanism that all participants can use to request and 
    release resources.  We use named callback objects for both.
    
    Drivers must initialize the Resource Manager by calling PmuRmInit.  This
    creates the callback that Participants will use to communicate.  When
    finished, the driver must call PmuRmUninit.
    
    To register a Participant, the driver must call PmuRmParticipate.  On
    success, this function returns an opaque pointer to a PMURM_PARTICIPANT
    structure.  The driver uses this pointer in subsequent calls to the 
    Resource Manager.  When a Participant no longer needs access to the 
    Resource Manager, it calls PmuRmResign to free the structure.  The
    structure keeps information that associates the Participant with a 
    DRIVER_OBJECT, and optional DEVICE_OBJECT, and a unicode string that is
    to be used as a friendly name.
    
    When a participant requires PMU resources, it calls PmuRmAcquire, passing
    it the affinity mask for the CPUs on which it requests the PMU.  On 
    success, the PMURM_PARTICIPANT structure is updated internally with a 
    copy of this affinity mask.  On failure, the PMURM_PARTICIPANT structure's
    internal affinity mask is cleared.  If desired, the call also returns the
    friendly name of the Participant that denied the request.
    
    If PmuRmAcquire's callback fails to obtain the resources, then it will 
    block on an event.  The timeout for this event is specified in a parameter
    to PmuRmAcquire.  The event is set any time a Participant releases PMU
    resources.  PmuRmAcquire will then retry the request.  This continues 
    until the request is successful, or the timeout expires.
*/

//
// Our named callback.
//
static const WCHAR CallbackName_buffer[] = L"\\Callback\\VSPerf_PmuManager";
static UNICODE_STRING CallbackName =
{
    sizeof(CallbackName_buffer) - sizeof(WCHAR),   \
    sizeof(CallbackName_buffer),                   \
    (PWSTR)(CallbackName_buffer)                   \
};    

PCALLBACK_OBJECT CallbackObject = NULL;


//
// Our pool tag, in case anyone asks
//
ULONG PmuRmTag = 'mrmp';


//
// Each participant supplies a valid structure for its ownership data.
// This data will be passed in the callback.
//
typedef struct _PMURM_PARTICIPANT
{
    // 
    // Current ownership mask
    //
    KAFFINITY       CpuMask;
    
    //
    // State of vote (only used during acquisition callbacks)
    //
    BOOLEAN         Vote;

    //
    // Driver and (optional) Device objects for this participant.
    //
    PDRIVER_OBJECT  DriverObject;
    PDEVICE_OBJECT  DeviceObject;

    //
    // Callback handle for this participant
    //
    PVOID           CallbackHandle;

    //
    // Signal event for waits
    //
    KEVENT          WaitEvent;

    //
    // Friendly name of participant (for error messages).
    //
    UNICODE_STRING  FriendlyName;

    //
    // Buffer for FriendlyName immediately follows this structure.
    //
    
} PMURM_PARTICIPANT, * PPMURM_PARTICIPANT;


//
// Parameters to the callback notification function
//
typedef struct _PMURM_CB_IN_PARMS
{
    // 
    // TRUE if this is an acquisition request, FALSE if it is a release.
    //
    BOOLEAN             Acquiring;

    //
    // The desired CPU mask (for an acquisition), or the released CPU
    // mask (for a release).
    //
    KAFFINITY           CpuMask;
    
    //
    // A pointer to the participant requesting the PMU.
    //
    PPMURM_PARTICIPANT  Participant;
    
} PMURM_CB_IN_PARMS, * PPMURM_CB_IN_PARMS;


//
// For a failed acquisiton, the last 'NO' vote will populate the output
// parms with its info, which may be used in an error message.
//
typedef struct _PMURM_CB_OUT_PARMS
{
    //
    // Driver and (optional) Device objects for declining voter.
    //
    PDRIVER_OBJECT  DriverObject;
    PDEVICE_OBJECT  DeviceObject;
    
    //
    // A friendly name for debugging or issuing an error message to user mode
    // profiling components.
    //
    PUNICODE_STRING FriendlyName;
    
} PMURM_CB_OUT_PARMS, * PPMURM_CB_OUT_PARMS;


/*
    As part of the PmuRm scheme, we must request notifications so that we
    can vote on requests.
*/
VOID
ParticipantCallback(
    IN PVOID CallbackContext,
    IN PVOID Argument1,
    IN PVOID Argument2
    )
{
    // Context is the participant (us)
    // Argument1 is the input parms
    // Argument2 is the output parms
    PPMURM_PARTICIPANT  Participant = (PPMURM_PARTICIPANT) CallbackContext;
    PPMURM_CB_IN_PARMS  InParms     = (PPMURM_CB_IN_PARMS) Argument1;
    PPMURM_CB_OUT_PARMS OutParms    = (PPMURM_CB_OUT_PARMS)Argument2;

    if ( InParms->Acquiring )
    {
        // Someone is requesting ownership of the PMU.
        // Only need to check when no one else has rejected the request
        if ( InParms->Participant->Vote )
        {
            // Always vote yes for ourselves.  Otherwise, verify that our
            // current mask doesn't include any bits in the request.
            if ( (InParms->Participant == Participant) || 
                 (InParms->CpuMask & Participant->CpuMask) == 0 )
            {
                // Vote YES
                InParms->Participant->CpuMask |= InParms->CpuMask;
            }
            else
            {
                // Vote NO
                InParms->Participant->Vote    = FALSE;
                InParms->Participant->CpuMask &= ~(InParms->CpuMask);
                
                // Populate the output parameters with information on who
                // voted NO.
                OutParms->DriverObject = Participant->DriverObject;
                OutParms->DeviceObject = Participant->DeviceObject;
                if ( OutParms->FriendlyName != NULL )
                {
                    RtlCopyUnicodeString(
                        OutParms->FriendlyName, 
                        &Participant->FriendlyName
                        );
                }
            }
        }
    }
    else
    {
        // Someone is releasing their PMU ownership.  Set an event 
        // to wake me up if I am waiting.
        KeSetEvent(&Participant->WaitEvent, 0, FALSE);
    }
}


/*------------------------------------------------------------------------
    Public facing APIs
------------------------------------------------------------------------*/

NTSTATUS
NTAPI
PmuRmInit(
	VOID
	)
/*++

Routine Description:

    This function initializes the PMU resource manager.

Arguments:

Return Value:

    NTSTATUS.

Notes:

    Always called at PASSIVE_LEVEL.
    
--*/
{
    //
    // Create our callback object for driver-driver communication.
    //
    OBJECT_ATTRIBUTES   ObjectAttributes;
    NTSTATUS Status;
    
    InitializeObjectAttributes(
        &ObjectAttributes,
        &CallbackName,
        OBJ_CASE_INSENSITIVE | OBJ_PERMANENT,
        NULL,
        NULL
        );
        
    Status = ExCreateCallback(
                &CallbackObject,
                &ObjectAttributes,
                TRUE,
                TRUE);
                            
    if ( !NT_SUCCESS(Status) )
        CallbackObject = NULL;
        
    return Status;
}


NTSTATUS
NTAPI
PmuRmUninit(
	VOID
	)
/*++

Routine Description:

    This function is used to release the PMU resource manager.

Arguments:

Return Value:

    NTSTATUS.

Notes:

    Always called at PASSIVE_LEVEL.

--*/
{
    // 
    // Release callback
    //
    if ( CallbackObject != NULL )
    {
        ObDereferenceObject(CallbackObject);
        CallbackObject = NULL;
    }

    return STATUS_SUCCESS;
}


NTSTATUS
NTAPI
PmuRmParticipate(
    IN  PDRIVER_OBJECT       DriverObject,
    IN  PDEVICE_OBJECT       DeviceObject OPTIONAL,
    IN  PUNICODE_STRING      FriendlyName,
    OUT PPMURM_PARTICIPANT * Participant
    )
/*++

Routine Description:

    This function registers a participant in the resource management scheme.

Arguments:

    DriverObject - Specifies the associated driver for the participant.
    
    DeviceObject - Specifies the associated device for the participant.  This
        parameter can be NULL if there is no associated device.
                   
    FriendlyName - Specifies a friendly name for the participant.  This string
        will be used in warning messages from other participants.
        
    Participant - PPMURM_PARTICIPANT pointer that will receive an initialized
        structure to be used in other PMURM calls.

Return Value:

    NTSTATUS.

Notes:

    Always called at PASSIVE_LEVEL.

--*/
{
    PPMURM_PARTICIPANT  TempParticipant;
    SIZE_T              StructSize;
    
    *Participant = NULL;

    //
    // The callback has to have been created
    //
    if ( CallbackObject == NULL )
        return STATUS_UNSUCCESSFUL;
    
    // 
    // Need room for the participant struct and the friendly name
    //
    StructSize = sizeof(PMURM_PARTICIPANT) + 
                 FriendlyName->Length + sizeof(WCHAR);
    
    // 
    // Callbacks will be done at DISPATCH_LEVEL, so our objects must come
    // from the non-paged pool.
    //
    TempParticipant = ExAllocatePoolWithTag(
                        NonPagedPool,
                        StructSize,
                        PmuRmTag);
                        
    if ( TempParticipant == NULL )
        return STATUS_NO_MEMORY;

    //
    // Initialize participant struct
    //
    TempParticipant->CpuMask      = 0;
    TempParticipant->Vote         = FALSE;
    TempParticipant->DriverObject = DriverObject;
    TempParticipant->DeviceObject = DeviceObject;
    
    //
    // Copy the friendly name to the end of the participant struct.
    //
    TempParticipant->FriendlyName.MaximumLength = 
        FriendlyName->Length + sizeof(WCHAR);

    TempParticipant->FriendlyName.Buffer = 
        (PWSTR)(TempParticipant + 1);

    RtlCopyUnicodeString(
        &TempParticipant->FriendlyName, 
        FriendlyName
        );
    
	//
    // Initialize wait event structure.
    //
    KeInitializeEvent(
        &TempParticipant->WaitEvent,
        NotificationEvent,
        FALSE
        );

	//
    // Register this participant for a callback.
    //
    TempParticipant->CallbackHandle =
        ExRegisterCallback(
            CallbackObject,
            ParticipantCallback,
            TempParticipant
            );
            
    if ( TempParticipant->CallbackHandle == NULL )
    {
        ExFreePool(TempParticipant);
        return STATUS_UNSUCCESSFUL;
    }

    *Participant = TempParticipant;
    return STATUS_SUCCESS;
}


NTSTATUS
NTAPI
PmuRmResign(
    IN  PPMURM_PARTICIPANT  Participant
    )
/*++

Routine Description:

    This function removes a participant in the resource management scheme.

Arguments:

    Participant - The original PMURM_PARTICIPANT pointer returned from 
        PmuRmParticipate.

Return Value:

    NTSTATUS.

Notes:

    Always called at PASSIVE_LEVEL.

--*/
{
    if ( Participant == NULL ||
         Participant->CallbackHandle == NULL )
    {
        return STATUS_INVALID_PARAMETER;
    }
    
    //
    // If we own resources, wake up anyone who might be waiting.
    //
    if ( Participant->CpuMask != 0 )
        PmuRmRelease(Participant, Participant->CpuMask);
    
    // 
    // Unregister the participant's callback.
    //
    ExUnregisterCallback(Participant->CallbackHandle);
    
    //
    // Free participant structure.
    //
    ExFreePool(Participant);
    
    return STATUS_SUCCESS;
}


NTSTATUS
NTAPI
PmuRmAcquire(
    IN  PPMURM_PARTICIPANT  Participant,
	IN	KAFFINITY           CpuMask,
    IN  PLARGE_INTEGER      Timeout      OPTIONAL,
    OUT PUNICODE_STRING     NoVoteText   OPTIONAL
	)
/*++

Routine Description:

    This function attempts to acquire PMU resources for the given CPUs.

Arguments:

    Participant - The PMURM_PARTICIPANT pointer returned from PmuRmParticipate.

    CpuMask - A bit mask indicating which CPUs to acquire PMU resources for.
    
    Timeout - An optional timeout value.  If omitted, this function blocks 
        until the PMU resources become available.  If set to zero, the function
        returns success or failure immediately.
                   
    NoVoteText - If the function fails because the resource is currently held,
        this string will hold the friendly name of the participant that holds 
        the resource.

Return Value:

    NTSTATUS.

Notes:

    Always called at PASSIVE_LEVEL.

--*/
{
    KIRQL         OldIrql;
    NTSTATUS      Status = STATUS_UNSUCCESSFUL;
    LARGE_INTEGER CurrentTimeout = {0};
    LARGE_INTEGER StartTime;
    LARGE_INTEGER ElapsedTime;
    
    PMURM_CB_IN_PARMS InParms;
    PMURM_CB_OUT_PARMS OutParms;

    // 
    // You cannot acquire resources you already own.
    // 
    if ( (Participant->CpuMask & CpuMask) != 0 )
        return STATUS_INVALID_PARAMETER;

    //
    // The callback has to have been created
    //
    if ( CallbackObject == NULL )
        return STATUS_UNSUCCESSFUL;

    // Copy timeout value (if present)
    if ( Timeout != NULL )
    {
        CurrentTimeout = *Timeout;
        // Reassign pointer so we don't trash the caller's data
        Timeout = &CurrentTimeout;
    }
    
    // 
    // Set up in and out parms
    //
    InParms.Acquiring = TRUE;
    InParms.CpuMask = CpuMask;
    InParms.Participant = Participant;

    OutParms.FriendlyName = NoVoteText;
    
    //
    // We will continue attempting to get the resource until either we 
    // succeed or we time out.
    //
    while ( TRUE )
    {
        //
        // Get base for elapsed time
        //
        KeQuerySystemTime(&StartTime);
        
        // 
        // This callback *must* happen synchronously.  
        //
		IoAcquireCancelSpinLock(&OldIrql);
        {
            Participant->Vote = TRUE;
            KeClearEvent(&Participant->WaitEvent);
            ExNotifyCallback(CallbackObject, &InParms, &OutParms);
        }    
        IoReleaseCancelSpinLock(OldIrql);

        //
        // We are done if our CpuMask was updated.
        //
        if ( (Participant->CpuMask & CpuMask) == CpuMask )
             return STATUS_SUCCESS;
    
        //
        // We lose!  Wait until someone releases or we time out.
        // 
        Status = KeWaitForSingleObject(
                    &Participant->WaitEvent,
                    Executive,
                    KernelMode,
                    FALSE,
                    Timeout
                    );

        //
        // Calculate time spent waiting.
        //
        KeQuerySystemTime(&ElapsedTime);
        ElapsedTime.QuadPart -= StartTime.QuadPart;
    
        if ( Status == STATUS_TIMEOUT )
            return STATUS_TIMEOUT;
        
        //
        // We've been woken up, let's adjust our timeout and retry.  If the 
        // timeout is negative, it's relative to the start time, and we adjust 
        // for elapsed time above.  Otherwise, the timeout is NULL (infinite) 
        // or an absolute time, and no adjustment is necessary.
        //
        if ( Timeout != NULL && Timeout->QuadPart < 0 )
        {
            Timeout->QuadPart += ElapsedTime.QuadPart;
            if ( Timeout->QuadPart >= 0 )
                return STATUS_TIMEOUT;
        }
    }

    // Should never get here.
    return Status;
}
	

NTSTATUS
NTAPI
PmuRmRelease(
    IN  PPMURM_PARTICIPANT  Participant,
	IN  KAFFINITY	        CpuMask
	)
/*++

Routine Description:

    This function returns PMU resources for the given CPUs.

Arguments:

    Participant - The PMURM_PARTICIPANT pointer returned from PmuRmParticipate.

    CpuMask - A bit mask indicating which CPUs to return PMU resources for.

Return Value:

    NTSTATUS.

Notes:

    Always called at PASSIVE_LEVEL.

--*/
{
    PMURM_CB_IN_PARMS InParms;
    KIRQL    OldIrql;

    //
    // The callback has to have been created
    //
    if ( CallbackObject == NULL )
        return STATUS_UNSUCCESSFUL;

    //
    // Don't bother releasing what we don't have.
    //
    if ( (Participant->CpuMask & CpuMask) == 0 )
        return STATUS_UNSUCCESSFUL;

    // Set up in parms
    InParms.Acquiring   = FALSE;
    InParms.CpuMask     = CpuMask;
    InParms.Participant = Participant;
    
    // 
    // This callback *must* happen synchronously.  
    //
	IoAcquireCancelSpinLock(&OldIrql);
    {
        //
        // Slam our CpuMask.
        //
        Participant->CpuMask &= ~CpuMask;
        
        //
        // Notify everyone that PMU resources have become available.
        //
        ExNotifyCallback(CallbackObject, &InParms, NULL);
    }    
    IoReleaseCancelSpinLock(OldIrql);

    return STATUS_SUCCESS;
}
