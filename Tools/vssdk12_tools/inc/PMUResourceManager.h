#pragma once
//--------------------------------------------------------------------------
// Microsoft Visual Studio Sampling Profiler Driver
//
// Copyright (c) 2005 Microsoft Corporation Inc.
// All rights reserved
//
// PMUResourceManager.h
//
// PMU resource management.
//
//---------------------------------------------------------------------------

#ifdef __cplusplus
extern "C" {
#endif

#include <ntddk.h>

/*---------------------------------------------------------------------------

Usage:
	
	Any driver wishing to politely share the PMU should use the Performance 
	Monitoring Unit Resource Manager (PmuRm) API in this module.

    The driver must initialize the PmuRm using PmuRmInit.  It must
    deinitialize using PmuRmUninit.  Typically this would be done in 
    DriverEntry and DriverUnload.
    
    Each entity in the driver that wishes to participate in the PmuRm 
    framework must call PmuRmParticipate.  Participation is resigned using
    PmuRmResign.  This could be done in a driver's AddDevice/RemoveDevice
    routines, for instance if multiple devices will participate independently
    in the management scheme.
    
    When a participant wishes to acquire PMU resources, it calls
    PmuRmAcquire, passing the affinity mask for the processors on which it
    will acquire resources, along with an optional timeout.  This call
    will block until the timeout expires or the resources are acquired.
    
    The participant must eventually release the resource using PmuRmRelease.
    
Example:

    // Assuming driver exposes a single device
    
    DEVICE_OBJECT MyDevice;
    UNICODE_STRING MyFriendlyName;
    
    PPMURM_PARTICIPANT Participant;
    
    NTSTATUS
    DriverEntry(PDRIVER_OBJECT DriverObject, PUNICODE_STRING RegPath)
    {
        NTSTATUS Status;
        
        // Initialize PmuRm.
        Status = PmuRmInit();
        
        if ( !NT_SUCCESS( Status ) )
            // bail...
        
        // Create the device
        IoCreateDevice(..., &DeviceObject);
        
        // Create a PmuRmParticipant
        RtlInitUnicodeString(&MyFriendlyName, L"My Excellent Driver");
        
        Status = PmuRmParticipate(
                    DriverObject,
                    DeviceObject,
                    &MyFriendlyName,
                    &Participant);
                    
        if ( !NT_SUCCESS( Status ) )
            // bail...
    
        ...
    }    

    NTSTATUS
    DriverUnload()
    {
        PmuRmResign(Participant);
        PmuRmUninit();
        [...]
    }
    
    NTSTATUS
    DeviceIoctl(DEVICE_OBJECT *	Device, IRP * Irp)
    {
        [...]
        
        case IOCTL_GET_PMU:

            // Wait 10 seconds
            LARGE_INTEGER   Timeout;
            Timeout.QuadPart = -(10 * 1000 * 1000 * 10);

            // Get PMUs for CPUs 0 and 1
            Status = PmuRmAcquire(Participant, 0x000000003, &Timeout, NULL);
            
            break;
            
        case IOCTL_RETURN_PMU:
        
            // Return CPUs 0 and 1
            PmuRmRelease(0x00000003);
            
            break;
            
        [...]
    }
    
    
---------------------------------------------------------------------------*/


typedef struct _PMURM_PARTICIPANT PMURM_PARTICIPANT, *PPMURM_PARTICIPANT;


NTSTATUS
NTAPI
PmuRmInit(
	VOID
	);
/*++

Routine Description:

    This function initializes the PMU resource manager.

Arguments:

Return Value:

    NTSTATUS.

Notes:

    Must be called at PASSIVE_LEVEL.
    
--*/


	
NTSTATUS
NTAPI
PmuRmUninit(
	VOID
	);
/*++

Routine Description:

    This function is used to release the PMU resource manager.

Arguments:

Return Value:

    NTSTATUS.

Notes:

    Must be called at PASSIVE_LEVEL.

--*/


	
NTSTATUS
NTAPI
PmuRmParticipate(
    IN  PDRIVER_OBJECT       DriverObject,
    IN  PDEVICE_OBJECT       DeviceObject OPTIONAL,
    IN  PUNICODE_STRING      FriendlyName,
    OUT PPMURM_PARTICIPANT * Participant
    );
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

    Must be called at PASSIVE_LEVEL.

--*/



NTSTATUS
NTAPI
PmuRmResign(
    IN  PPMURM_PARTICIPANT  Participant
    );
/*++

Routine Description:

    This function removes a participant in the resource management scheme.

Arguments:

    Participant - The original PMURM_PARTICIPANT pointer returned from 
        PmuRmParticipate.

Return Value:

    NTSTATUS.

Notes:

    Must be called at PASSIVE_LEVEL.

--*/



NTSTATUS
NTAPI
PmuRmAcquire(
    IN  PPMURM_PARTICIPANT  Participant,
	IN	KAFFINITY           CpuMask,
    IN  PLARGE_INTEGER      Timeout      OPTIONAL,
    OUT PUNICODE_STRING     NoVoteText   OPTIONAL
	);
/*++

Routine Description:

    This function attempts to acquire PMU resources for the given CPUs.

Arguments:

    Participant - The PMURM_PARTICIPANT pointer returned from PmuRmParticipate.

    CpuMask - A bit mask indicating which CPUs to acquire PMU resources for.
    
    Timeout - An optional timeout value.  The semantics of this value are the 
        same as the timeout parameter to KeWaitForSingleObject.  If omitted, 
        this function blocks until the PMU resources become available.  If set 
        to zero, the function returns success or failure immediately.  A 
        negative value implies a relative timeout, based from the current time,
        and a positive value describes an absolute time.  
                   
    NoVoteText - If the function fails because the resource is currently held,
        this string will hold the friendly name of the participant that holds 
        the resource.

Return Value:

    NTSTATUS.

Notes:

    Must be called at PASSIVE_LEVEL.

--*/



NTSTATUS
NTAPI
PmuRmRelease(
    IN  PPMURM_PARTICIPANT  Participant,
	IN  KAFFINITY	        CpuMask
	);
/*++

Routine Description:

    This function returns PMU resources for the given CPUs.

Arguments:

    Participant - The PMURM_PARTICIPANT pointer returned from PmuRmParticipate.

    CpuMask - A bit mask indicating which CPUs to return PMU resources for.

Return Value:

    NTSTATUS.

Notes:

    Must be called at PASSIVE_LEVEL.

--*/


#ifdef __cplusplus
}
#endif
