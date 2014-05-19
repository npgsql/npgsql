#pragma once

/****************************************************************************
	MsoCI.h

	Owner: ClarG
 	Copyright (c) 1995 Microsoft Corporation

	This file contains the exported interfaces and declarations for
	Office Component Integration.
****************************************************************************/

#ifndef MSOCI_H
#define MSOCI_H

#if !defined(MSOSTD_H)
#include <msostd.h>
#endif

#if !defined(MSOUSER_H)
#include <msouser.h>
#endif

#if !defined(MSODEBUG_H)
#include <msodebug.h>
#endif

/****************************************************************************
	Component integration structures and constants
****************************************************************************/

// Component registration flags
enum
	{
	msocrfNeedIdleTime         = 1,  // needs idle time
	msocrfNeedPeriodicIdleTime = 2,  // needs idle time every N milliseconds
	msocrfPreTranslateKeys     = 4,  // must process keyboard msgs
	                                 // before translation
	msocrfPreTranslateAll      = 8,  // must process all msgs
	                                 // before translation
	msocrfNeedSpecActiveNotifs = 16, // needs to be notified for special
	                                 // activation changes (currently, this will
	                                 // notify comp if ExclusiveBorderSpace
	                                 // or ExclusiveActivation mode changes)
	                                 // Top-level comps should reg this flag.
	msocrfNeedTopActiveNotifs  = msocrfNeedSpecActiveNotifs, // old name
	msocrfNeedAllActiveNotifs  = 32, // needs to be notified for every
	                                 // change in activation state
	msocrfExclusiveBorderSpace = 64, // needs exclusive border space when 
	                                 // active (normally only used by TopLevel
	                                 // Mac components)
	msocrfExclusiveActivation = 128, // comp becomes exclusively active 
	                                 // when activated
	msocrfNeedAllMacEvents	= 256,	 // needs all mac events
	msocrfMaster			= 512,   // comp is always active and gets first shot at evrything
	};

// Component registration advise flags (see msocstate enumeration)
enum
	{
	msocadvfModal              = 1,  // needs modal state change notification
	                                 //  (must be registered by components
	                                 //   managing a toplevel window)												
	msocadvfRedrawOff          = 2,  // needs redrawOff state change notif
	msocadvfWarningsOff        = 4,  // needs warningsOff state change notif
	msocadvfRecording          = 8,  // needs Recording state change notif
	};

// Component registration information
typedef struct _MSOCRINFO
	{
	ULONG cbSize;             // size of MSOCRINFO structure in bytes.
	ULONG uIdleTimeInterval;  // If msocrfNeedPeriodicIdleTime is registered
	                          // in grfcrf, component needs to perform
	                          // periodic idle time tasks during an idle phase
	                          // every uIdleTimeInterval milliseconds.
	DWORD grfcrf;             // bit flags taken from msocrf values (above)
	DWORD grfcadvf;           // bit flags taken from msocadvf values (above)
	} MSOCRINFO;


// Component Host flags
enum
	{
	msochostfExclusiveBorderSpace = 1,  // needs exclusive border space when 
	                                    // active (normally only used by 
	                                    // TopLevel Mac hosts)
	};

// Component Host information
typedef struct _MSOCHOSTINFO
	{
	ULONG cbSize;             // size of MSOCHOSTINFO structure in bytes.
	DWORD grfchostf;          // bit flags taken from msochostf values (above)
	} MSOCHOSTINFO;


// idle flags, passed to IMsoComponent::FDoIdle and 
// IMsoStdComponentMgr::FDoIdle.
typedef enum
	{
	msoidlefPeriodic    = 1,  // periodic idle tasks
	msoidlefNonPeriodic = 2,  // any nonperiodic idle task
	msoidlefPriority    = 4,  // high priority, nonperiodic idle tasks
	msoidlefAll         = -1  // all idle tasks
	} MSOIDLE;


// Reasons for pushing a message loop, passed to 
// IMsoComponentManager::FPushMessageLoop and 
// IMsoComponentHost::FPushMessageLoop.  The host should remain in message
// loop until IMsoComponent::FContinueMessageLoop 
// (or IMsoStdComponentMgr::FContinueMessageLoop) returns FALSE.
enum
	{
	msoloopFocusWait = 1,  // component is activating host 
	msoloopDoEvents  = 2,  // component is asking host to process messages
	msoloopDebug     = 3,  // component has entered debug mode
	msoloopModalForm = 4,   // component is displaying a modal form  
	msoloopModalAlert = 5  // Different from ModalForm in the intention that
							// this should act as much like a blocking call as
				 			// as possible- app should do no idling in this case
				 			// if alerts might come up in badly defined states

	};


/* msocstate values: state IDs passed to 
	IMsoComponent::OnEnterState, 
	IMsoComponentManager::OnComponentEnterState/FOnComponentExitState/FInState,
	IMsoComponentHost::OnComponentEnterState,
	IMsoStdComponentMgr::OnHostEnterState/FOnHostExitState/FInState.
	When the host or a component is notified through one of these methods that 
	another entity (component or host) is entering or exiting a state 
	identified by one of these state IDs, the host/component should take
	appropriate action:
		msocstateModal (modal state):
			If app is entering modal state, host/component should disable
			its toplevel windows, and reenable them when app exits this
			state.  Also, when this state is entered or exited, host/component
			should notify approprate inplace objects via 
			IOleInPlaceActiveObject::EnableModeless.
		msocstateRedrawOff (redrawOff state):
			If app is entering redrawOff state, host/component should disable
			repainting of its windows, and reenable repainting when app exits
			this state.
		msocstateWarningsOff (warningsOff state):
			If app is entering warningsOff state, host/component should disable
			the presentation of any user warnings, and reenable this when
			app exits this state.
		msocstateRecording (Recording state):
			Used to notify host/component when Recording is turned on or off. */
enum
	{
	msocstateModal       = 1, // modal state; disable toplevel windows
	msocstateRedrawOff   = 2, // redrawOff state; disable window repainting
	msocstateWarningsOff = 3, // warningsOff state; disable user warnings
	msocstateRecording   = 4, // Recording state
	};


/*             ** Comments on State Contexts **
	IMsoComponentManager::FCreateSubComponentManager allows one to create a 
	hierarchical tree of component managers.  This tree is used to maintain 
	multiple contexts with regard to msocstateXXX states.  These contexts are 
	referred to as 'state contexts'.
	Each component manager in the tree defines a state context.  The
	components registered with a particular component manager or any of its
	descendents live within that component manager's state context.  Calls
	to IMsoComponentManager::OnComponentEnterState/FOnComponentExitState
	can be used to	affect all components, only components within the component
	manager's state context, or only those components that are outside of the
	component manager's state context.  IMsoComponentManager::FInState is used
	to query the state of the component manager's state context at its root.

   msoccontext values: context indicators passed to 
	IMsoComponentManager::OnComponentEnterState/FOnComponentExitState.
	These values indicate the state context that is to be affected by the
	state change. 
	In IMsoComponentManager::OnComponentEnterState/FOnComponentExitState,
	the comp mgr informs only those components/host that are within the
	specified state context. */
enum
	{
	msoccontextAll    = 0, // all state contexts in state context tree
	msoccontextMine   = 1, // component manager's state context
	msoccontextOthers = 2, // all other state contexts outside of comp mgr's
	};


/*     ** WM_MOUSEACTIVATE Note (for top level compoenents and host) **
	If the active (or tracking) comp's reg info indicates that it
	wants mouse messages, then no MA_xxxANDEAT value should be returned 
	from WM_MOUSEACTIVATE, so that the active (or tracking) comp will be able
	to process the resulting mouse message.  If one does not want to examine
	the reg info, no MA_xxxANDEAT value should be returned from 
	WM_MOUSEACTIVATE if any comp is active (or tracking).
	One can query the reg info of the active (or tracking) component at any
	time via IMsoComponentManager::FGetActiveComponent. */

/* msogac values: values passed to 
	IMsoComponentManager::FGetActiveComponent. */ 
enum
	{
	msogacActive    = 0, // retrieve true active component
	msogacTracking   = 1, // retrieve tracking component
	msogacTrackingOrActive = 2, // retrieve tracking component if one exists,
	                            // otherwise retrieve true active component
	};


/* msocWindow values: values passed to IMsoComponent::HwndGetWindow. */ 
enum
	{
	msocWindowFrameToplevel = 0,
		/* MDI Apps should return the MDI frame (not MDI client) or App frame
			window, and SDI Apps should return the frame window which hosts the
			component. Basically it should be the topmost window which owns the
			component. For a toolbar set this will be the toplevel owner of
			TBS::m_hwnd. */

	msocWindowFrameOwner = 1,
		/* This is the window which owns the component. It could be same as
			the window obtained by msocWindowFrameTopLevel or be an owned window
			of that window. For a toolbar set this will be TBS::m_hwnd. */

	msocWindowComponent = 2,
		/* This is the "main" window of the component (if it has one). */

	msocWindowDlgOwner = 3,
		/* Caller wishes to display a dialog to be parented by the component.
			Component should return a window suitable for use as the dialog's
			owner window. */  
	};

/* Values passed to IMsoComponentManager::FSetTrackingComponent. */ 
enum
	{
	msostcBeginTracking = 1,  // begin tracking normally
	msostcNonBlocking = 2,    // don't report to the app that the component is
	                          // a tracking component.  used for workpanes and
	                          // other pane-like toolbars.
	};


/****************************************************************************
	Defines the IMsoComponent interface

	Any component that needs idle time, the ability to process
	messages before they are translated 
	(for example, to call TranslateAccelerator or IsDialogMessage),
	notification about modal states,
	or the ability push message loops 
	must implement this interface and register with the Component Manager.
****************************************************************************/
#undef  INTERFACE
#define INTERFACE  IMsoComponent

DECLARE_INTERFACE_(IMsoComponent, IUnknown)
	{
	BEGIN_MSOINTERFACE
	// *** IUnknown methods ***
	MSOMETHOD(QueryInterface) (THIS_ REFIID riid, void **ppvObj) PURE;
	MSOMETHOD_(ULONG, AddRef) (THIS) PURE;
	MSOMETHOD_(ULONG, Release) (THIS) PURE;

	/* Standard FDebugMessage method.
	   Since IMsoComponent is a reference counted interface, 
	   MsoDWGetChkMemCounter should be used when processing the 
	   msodmWriteBe message. */
	MSODEBUGMETHOD

	/* Give component a chance to process the message pMsg before it is
		translated and dispatched. Component can do TranslateAccelerator,
		do IsDialogMessage, modify pMsg, or take some other action.
		Return TRUE if the message is consumed, FALSE otherwise. */
	MSOMETHOD_(BOOL, FPreTranslateMessage) (THIS_ MSG *pMsg) PURE;

	/* Notify component when app enters or exits (as indicated by fEnter)
		the state identified by uStateID (a value from msocstate enumeration).
		Component should take action depending on value of uStateID
		(see msocstate comments, above).
		
		Note: If n calls are made with TRUE fEnter, component should consider 
		the state to be in effect until n calls are made with FALSE fEnter.
		
		Note: Components should be aware that it is possible for this method to
		be called with FALSE fEnter more	times than it was called with TRUE 
		fEnter (so, for example, if component is maintaining a state counter
		(incremented when this method is called with TRUE fEnter, decremented
		when called with FALSE fEnter), the counter should not be decremented
		for FALSE fEnter if it is already at zero.)  */
	MSOMETHOD_(void, OnEnterState) (THIS_ ULONG uStateID, BOOL fEnter) PURE;

	/* Notify component when the host application gains or loses activation.
		If fActive is TRUE, the host app is being activated and dwOtherThreadID
		is the ID of the thread owning the window being deactivated.
		If fActive is FALSE, the host app is being deactivated and 
		dwOtherThreadID is the ID of the thread owning the window being 
		activated.
		Note: this method is not called when both the window being activated
		and the one being deactivated belong to the host app. */
	MSOMETHOD_(void, OnAppActivate) (THIS_ 
		BOOL fActive, DWORD dwOtherThreadID) PURE;
	
	/* Notify the active component that it has lost its active status because
		the host or another component has become active. */
	MSOMETHOD_(void, OnLoseActivation) (THIS) PURE;

	/* Notify component when a new object is being activated.
		If pic is non-NULL, then it is the component that is being activated.
		In this case, fSameComponent is TRUE if pic is the same component as
		the callee of this method, and pcrinfo is the reg info of pic.
		If pic is NULL and fHostIsActivating is TRUE, then the host is the
		object being activated, and pchostinfo is its host info.
		If pic is NULL and fHostIsActivating is FALSE, then there is no current
		active object.

		If pic is being activated and pcrinfo->grf has the 
		msocrfExclusiveBorderSpace bit set, component should hide its border
		space tools (toolbars, status bars, etc.);
		component should also do this if host is activating and 
		pchostinfo->grfchostf has the msochostfExclusiveBorderSpace bit set.
		In either of these cases, component should unhide its border space
		tools the next time it is activated.

		If pic is being activated and pcrinfo->grf has the
		msocrfExclusiveActivation bit is set, then pic is being activated in
		"ExclusiveActive" mode.  
		Component should retrieve the top frame window that is hosting pic
		(via pic->HwndGetWindow(msocWindowFrameToplevel, 0)).  
		If this window is different from component's own top frame window, 
			component should disable its windows and do other things it would do
			when receiving OnEnterState(msocstateModal, TRUE) notification. 
		Otherwise, if component is top-level, 
			it should refuse to have its window activated by appropriately
			processing WM_MOUSEACTIVATE (but see WM_MOUSEACTIVATE NOTE, above).
		Component should remain in one of these states until the 
		ExclusiveActive mode ends, indicated by a future call to 
		OnActivationChange with ExclusiveActivation bit not set or with NULL
		pcrinfo. */
	MSOMETHOD_(void, OnActivationChange) (THIS_ 
		IMsoComponent *pic, 
		BOOL fSameComponent,
		const MSOCRINFO *pcrinfo,
		BOOL fHostIsActivating,
		const MSOCHOSTINFO *pchostinfo, 
		DWORD dwReserved) PURE;

	/* Give component a chance to do idle time tasks.  grfidlef is a group of
		bit flags taken from the enumeration of msoidlef values (above),
		indicating the type of idle tasks to perform.  
		Component may periodically call IMsoComponentManager::FContinueIdle; 
		if this method returns FALSE, component should terminate its idle 
		time processing and return.  
		Return TRUE if more time is needed to perform the idle time tasks, 
		FALSE otherwise.
		Note: If a component reaches a point where it has no idle tasks
		and does not need FDoIdle calls, it should remove its idle task
		registration via IMsoComponentManager::FUpdateComponentRegistration.
		Note: If this method is called on while component is performing a 
		tracking operation, component should only perform idle time tasks that
		it deems are appropriate to perform during tracking. */
	MSOMETHOD_(BOOL, FDoIdle) (THIS_ DWORD grfidlef) PURE;
	
	/* Called during each iteration of a message loop that the component
		pushed. uReason and pvLoopData are the reason and the component private 
		data that were passed to IMsoComponentManager::FPushMessageLoop.
		This method is called after peeking the next message in the queue
		(via PeekMessage) but before the message is removed from the queue.
		The peeked message is passed in the pMsgPeeked param (NULL if no
		message is in the queue).  This method may be additionally called when
		the next message has already been removed from the queue, in which case
		pMsgPeeked is passed as NULL.
		Return TRUE if the message loop should continue, FALSE otherwise.
		If FALSE is returned, the component manager terminates the loop without
		removing pMsgPeeked from the queue. */
	MSOMETHOD_(BOOL, FContinueMessageLoop) (THIS_ 
		ULONG uReason, void *pvLoopData, MSG *pMsgPeeked) PURE;

	/* Called when component manager wishes to know if the component is in a
		state in which it can terminate.  If fPromptUser is FALSE, component
		should simply return TRUE if it can terminate, FALSE otherwise.
		If fPromptUser is TRUE, component should return TRUE if it can
		terminate without prompting the user; otherwise it should prompt the
		user, either 1.) asking user if it can terminate and returning TRUE
		or FALSE appropriately, or 2.) giving an indication as to why it
		cannot terminate and returning FALSE. */
	MSOMETHOD_(BOOL, FQueryTerminate) (THIS_ BOOL fPromptUser) PURE;
	
	/* Called when component manager wishes to terminate the component's
		registration.  Component should revoke its registration with component
		manager, release references to component manager and perform any
		necessary cleanup. */
	MSOMETHOD_(void, Terminate) (THIS) PURE;

	/* Called to retrieve a window associated with the component, as specified
		by dwWhich, a msocWindowXXX value (see msocWindow, above).
		dwReserved is reserved for future use and should be zero.
		Component should return the desired window or NULL if no such window
		exists. */
	MSOMETHOD_(HWND, HwndGetWindow) (THIS_ 
		DWORD dwWhich, DWORD dwReserved) PURE;
	};


/****************************************************************************
	Defines the IMsoComponentManager interface

	A component manager is an object implementing the IMsoComponentManager
	interface.  The component manager coordinates components with its message
	loop for proper distribution of idle time and pre-translation message
	processing.	
	It also coordinates modalities and the pushing of message loops.
	The host application can implement its own component manager and register
	it via MsoFSetComponentManager or it can make use of the office supplied
	component manager via MsoFCreateStdComponentManager.
****************************************************************************/
#undef  INTERFACE
#define INTERFACE  IMsoComponentManager

DECLARE_INTERFACE_(IMsoComponentManager, IUnknown)
	{
	BEGIN_MSOINTERFACE
	// *** IUnknown methods ***
	MSOMETHOD(QueryInterface) (THIS_ REFIID riid, void **ppvObj) PURE;
	MSOMETHOD_(ULONG, AddRef) (THIS) PURE;
	MSOMETHOD_(ULONG, Release) (THIS) PURE;

	/* Return in *ppvObj an implementation of interface iid for service
		guidService (same as IServiceProvider::QueryService).
		Return NOERROR if the requested service is supported, otherwise return
		NULL in *ppvObj and an appropriate error (eg E_FAIL, E_NOINTERFACE). */
	MSOMETHOD(QueryService) (THIS_
		REFGUID guidService, REFIID iid, void **ppvObj) PURE;

	/* Standard FDebugMessage method.
	   Since IMsoComponentManager is a reference counted interface, 
	   MsoDWGetChkMemCounter should be used when processing the 
	   msodmWriteBe message. */
	MSODEBUGMETHOD

	/* Register component piComponent and its registration info pcrinfo with
		this component manager.  Return in *pdwComponentID a cookie which will
		identify the component when it calls other IMsoComponentManager
		methods.
		Return TRUE if successful, FALSE otherwise. */
	MSOMETHOD_(BOOL, FRegisterComponent) (THIS_
		IMsoComponent *piComponent, const MSOCRINFO *pcrinfo, 
		DWORD_PTR *pdwComponentID) PURE;
	
	/* Undo the registration of the component identified by dwComponentID
		(the cookie returned from the FRegisterComponent method).
		Return TRUE if successful, FALSE otherwise. */
	MSOMETHOD_(BOOL, FRevokeComponent) (THIS_ DWORD_PTR dwComponentID) PURE;
	
	/* Update the registration info of the component identified by
		dwComponentID (the cookie returned from FRegisterComponent) with the
		new registration information pcrinfo.
		Typically this is used to update the idle time registration data, but
		can be used to update other registration data as well.
		Return TRUE if successful, FALSE otherwise. */
	MSOMETHOD_(BOOL, FUpdateComponentRegistration) (THIS_ 
		DWORD_PTR dwComponentID, const MSOCRINFO *pcrinfo) PURE;
	
	/* Notify component manager that component identified by dwComponentID
		(cookie returned from FRegisterComponent) has been activated.
		The active component gets the	chance to process messages before they
		are dispatched (via IMsoComponent::FPreTranslateMessage) and typically
		gets first shot at idle time after the host.
		This method fails if another component is already Exclusively Active.
		In this case, FALSE is returned and SetLastError is set to 
		msoerrACompIsXActive (comp usually need not take any special action
		in this case).
		Return TRUE if successful. */
	MSOMETHOD_(BOOL, FOnComponentActivate) (THIS_ DWORD_PTR dwComponentID) PURE;
	
	/* Called to inform component manager that  component identified by 
		dwComponentID (cookie returned from FRegisterComponent) wishes
		to perform a tracking operation (such as mouse tracking).
		The component calls this method with fTrack == TRUE to begin the
		tracking operation and with fTrack == FALSE to end the operation.
		During the tracking operation the component manager routes messages
		to the tracking component (via IMsoComponent::FPreTranslateMessage)
		rather than to the active component.  When the tracking operation ends,
		the component manager should resume routing messages to the active
		component.  
		Note: component manager should perform no idle time processing during a
		      tracking operation other than give the tracking component idle
		      time via IMsoComponent::FDoIdle.
		Note: there can only be one tracking component at a time.
		Return TRUE if successful, FALSE otherwise.  */
	MSOMETHOD_(BOOL, FSetTrackingComponent) (THIS_ 
		DWORD dwComponentID, ULONG grftrack) PURE;

	/* Notify component manager that component identified by dwComponentID
		(cookie returned from FRegisterComponent) is entering the state
		identified by uStateID (msocstateXXX value).  (For convenience when
		dealing with sub CompMgrs, the host can call this method passing 0 for
		dwComponentID.)  
		Component manager should notify all other interested components within
		the state context indicated by uContext (a msoccontextXXX value),
		excluding those within the state context of a CompMgr in rgpicmExclude,
		via IMsoComponent::OnEnterState (see "Comments on State Contexts", 
		above).
		Component Manager should also take appropriate action depending on the 
		value of uStateID (see msocstate comments, above).
		dwReserved is reserved for future use and should be zero.

		rgpicmExclude (can be NULL) is an array of cpicmExclude CompMgrs (can
		include root CompMgr and/or sub CompMgrs); components within the state
		context of a CompMgr appearing in this	array should NOT be notified of 
		the state change (note: if uContext	is msoccontextMine, the only 
		CompMgrs in rgpicmExclude that are checked for exclusion are those that 
		are sub CompMgrs of this Component Manager, since all other CompMgrs 
		are outside of this Component Manager's state context anyway.)

		Note: Calls to this method are symmetric with calls to 
		FOnComponentExitState. 
		That is, if n OnComponentEnterState calls are made, the component is
		considered to be in the state until n FOnComponentExitState calls are
		made.  Before revoking its registration a component must make a 
		sufficient number of FOnComponentExitState calls to offset any
		outstanding OnComponentEnterState calls it has made.

		Note: inplace objects should not call this method with
		uStateID == msocstateModal when entering modal state. Such objects
		should call IOleInPlaceFrame::EnableModeless instead. */
	MSOMETHOD_(void, OnComponentEnterState) (THIS_ 
		DWORD_PTR dwComponentID, 
		ULONG uStateID, 
		ULONG uContext,
		ULONG cpicmExclude,
		IMsoComponentManager **rgpicmExclude, 
		DWORD dwReserved) PURE;
	
	/* Notify component manager that component identified by dwComponentID
		(cookie returned from FRegisterComponent) is exiting the state
		identified by uStateID (a msocstateXXX value).  (For convenience when
		dealing with sub CompMgrs, the host can call this method passing 0 for
		dwComponentID.)
		uContext, cpicmExclude, and rgpicmExclude are as they are in 
		OnComponentEnterState.
		Component manager	should notify all appropriate interested components
		(taking into account uContext, cpicmExclude, rgpicmExclude) via
		IMsoComponent::OnEnterState (see "Comments on State Contexts", above). 
		Component Manager should also take appropriate action depending on
		the value of uStateID (see msocstate comments, above).
		Return TRUE if, at the end of this call, the state is still in effect
		at the root of this component manager's state context
		(because the host or some other component is still in the state),
		otherwise return FALSE (ie. return what FInState would return).
		Caller can normally ignore the return value.
		
		Note: n calls to this method are symmetric with n calls to 
		OnComponentEnterState (see OnComponentEnterState comments, above). */
	MSOMETHOD_(BOOL, FOnComponentExitState) (THIS_ 
		DWORD_PTR dwComponentID, 
		ULONG uStateID, 
		ULONG uContext,
		ULONG cpicmExclude,
		IMsoComponentManager **rgpicmExclude) PURE;

	/* Return TRUE if the state identified by uStateID (a msocstateXXX value)
		is in effect at the root of this component manager's state context, 
		FALSE otherwise (see "Comments on State Contexts", above).
		pvoid is reserved for future use and should be NULL. */
	MSOMETHOD_(BOOL, FInState) (THIS_ ULONG uStateID, void *pvoid) PURE;
	
	/* Called periodically by a component during IMsoComponent::FDoIdle.
		Return TRUE if component can continue its idle time processing, 
		FALSE if not (in which case component returns from FDoIdle.) */
	MSOMETHOD_(BOOL, FContinueIdle) (THIS) PURE;

	/* Component identified by dwComponentID (cookie returned from 
		FRegisterComponent) wishes to push a message loop for reason uReason.
		uReason is one the values from the msoloop enumeration (above).
		pvLoopData is data private to the component.
		The component manager should push its message loop, 
		calling IMsoComponent::FContinueMessageLoop(uReason, pvLoopData)
		during each loop iteration (see IMsoComponent::FContinueMessageLoop
		comments).  When IMsoComponent::FContinueMessageLoop returns FALSE, the
		component manager terminates the loop.
		Returns TRUE if component manager terminates loop because component
		told it to (by returning FALSE from IMsoComponent::FContinueMessageLoop),
		FALSE if it had to terminate the loop for some other reason.  In the 
		latter case, component should perform any necessary action (such as 
		cleanup). */
	MSOMETHOD_(BOOL, FPushMessageLoop) (THIS_ 
		DWORD_PTR dwComponentID, ULONG uReason, void *pvLoopData) PURE;

	/* Cause the component manager to create a "sub" component manager, which
		will be one of its children in the hierarchical tree of component
		managers used to maintiain state contexts (see "Comments on State
		Contexts", above).
		piunkOuter is the controlling unknown (can be NULL), riid is the
		desired IID, and *ppvObj returns	the created sub component manager.
		piunkServProv (can be NULL) is a ptr to an object supporting
		IServiceProvider interface to which the created sub component manager
		will delegate its IMsoComponentManager::QueryService calls. 
		(see objext.h or docobj.h for definition of IServiceProvider).
		Returns TRUE if successful. */
	MSOMETHOD_(BOOL, FCreateSubComponentManager) (THIS_ 
		IUnknown *piunkOuter, 
		IUnknown *piunkServProv,
		REFIID riid, 
		void **ppvObj) PURE;

	/* Return in *ppicm an AddRef'ed ptr to this component manager's parent
		in the hierarchical tree of component managers used to maintain state
		contexts (see "Comments on State	Contexts", above).
		Returns TRUE if the parent is returned, FALSE if no parent exists or
		some error occurred. */
	MSOMETHOD_(BOOL, FGetParentComponentManager) (THIS_ 
		IMsoComponentManager **ppicm) PURE;

	/* Return in *ppic an AddRef'ed ptr to the current active or tracking
		component (as indicated by dwgac (a msogacXXX value)), and
		its registration information in *pcrinfo.  ppic and/or pcrinfo can be
		NULL if caller is not interested these values.  If pcrinfo is not NULL,
		caller should set pcrinfo->cbSize before calling this method.
		Returns TRUE if the component indicated by dwgac exists, FALSE if no 
		such component exists or some error occurred.
		dwReserved is reserved for future use and should be zero. */
	MSOMETHOD_(BOOL, FGetActiveComponent) (THIS_ 
		DWORD dwgac, 
		IMsoComponent **ppic, 
		MSOCRINFO *pcrinfo,
		DWORD dwReserved) PURE;
	};


/****************************************************************************
	Defines the IMsoStdComponentMgr interface

	IMsoStdComponentMgr is an interface exposed by the office supplied
	standard component manager, created by MsoFCreateStdComponentMgr.
	The host application uses this interface to communicate directly with
	the standard component manager and indirectly with registered components.
	By making appropriate calls to this interface and implementing
	IMsoComponentHost the host can avoid implementing its own 
	IMsoComponentManager interface.
****************************************************************************/
#undef  INTERFACE
#define INTERFACE  IMsoStdComponentMgr

DECLARE_INTERFACE_(IMsoStdComponentMgr, IUnknown)
	{
	BEGIN_MSOINTERFACE
	// *** IUnknown methods ***
	MSOMETHOD(QueryInterface) (THIS_ REFIID riid, void **ppvObj) PURE;
	MSOMETHOD_(ULONG, AddRef) (THIS) PURE;
	MSOMETHOD_(ULONG, Release) (THIS) PURE;

	/* Standard FDebugMessage method.
	   Since IMsoStdComponentMgr is a reference counted interface, 
	   MsoDWGetChkMemCounter is used when processing the 
	   msodmWriteBe message. */
	MSODEBUGMETHOD

	/* Set *pchostinfo as the host info.  Can be called multiple times.
		Returns TRUE if successful. */
	MSOMETHOD_(BOOL, FSetHostInfo) (THIS_ 
		const MSOCHOSTINFO *pchostinfo) PURE;

	/* Host calls this method to give the active component a chance to
		process messages before they are translated and dispatched.
		The host need not call this method if no component is active.
		When this method is called on message pMsg, StdComponentMgr in turn 
		calls IMsoComponent::FPreTranslateMessage on the active component if
		its registration info indicates that it is interested.  
		Returns TRUE if message is consumed, in which case the host should
		perform no further processing on the message.  
		Returns FALSE otherwise. */
	MSOMETHOD_(BOOL, FPreTranslateMessage) (THIS_ MSG *pMsg) PURE;

	/* Called by host to notify StdComponentMgr that one of the host's windows
		has been activated.  This causes the current active component to lose
		its active status.  However, host should not assume that the component
		is no longer active until 
		IMsoComponentHost::OnComponentActivate(NULL) is called.
		This method fails if a component is already Exclusively Active.
		In this case, FALSE is returned and SetLastError is set to 
		msoerrACompIsXActive (host usually need not take any special action
		in this case).
		Returns TRUE if successful. */
	MSOMETHOD_(BOOL, FOnHostActivate) (THIS) PURE;

	/* Called by host to notify StdComponentMgr that host is entering the 
		state identified by uStateID (a msocstateXXX value).
		StdComponentMgr in turn notifies all interested components excluding
		those within the context of a CompMgr appearing in rgpicmExclude (an
		array (can be NULL) of cpicmExclude CompMgrs (can include root CompMgr 
		and/or sub CompMgrs)).
		dwReserved is reserved for future use and should be zero.
		Note: Calls to this method are symmetric with FOnHostExitState calls.
		That is, if n OnHostEnterState calls are made, the host is
		considered to be in the state until n FOnHostExitState calls are
		made. */
	MSOMETHOD_(void, OnHostEnterState) (THIS_ 
		ULONG uStateID, 
		ULONG cpicmExclude,
		IMsoComponentManager **rgpicmExclude, 
		DWORD dwReserved) PURE;

	/* Called by host to notify StdComponentMgr that host is exiting the state 
		identified by uStateID (a msocstateXXX value).  
		StdComponentMgr in turn notifies  all interested components excluding
		those within the context of a CompMgr appearing in rgpicmExclude (an
		array (can be NULL) of cpicmExclude CompMgrs (can include root CompMgr 
		and/or sub CompMgrs)).
		Returns TRUE if the state is still in effect at the end of this call
		(because some component is still in the state), otherwise returns 
		FALSE (ie. returns what FInState would return).  Caller can normally
		ignore the return value.
		Note: n calls to this method are symmetric with n calls to 
		OnHostEnterState (see OnHostEnterState comments, above). */
	MSOMETHOD_(BOOL, FOnHostExitState) (THIS_ 
		ULONG uStateID,
		ULONG cpicmExclude,
		IMsoComponentManager **rgpicmExclude) PURE;

	/* Returns TRUE if state identified by uStateID (a value from msocstate
		enumeration) is in effect, FALSE otherwise.
		pvoid is reserved for future use and should be NULL. */
	MSOMETHOD_(BOOL, FInState) (THIS_ ULONG uStateID, void *pvoid) PURE;
	
	/* Called by host to give registered components the chance to perform idle
		time tasks of the type indicated by grfidlef, a group of bit flags
		taken from the enumeration of msoidlef values (above).
		During a component tracking operation, StdComponentMgr only gives the
		tracking component idle time.
		Returns TRUE if any component needs more time to perform the idle time
		tasks, FALSE otherwise. */
	MSOMETHOD_(BOOL, FDoIdle) (THIS_ MSOIDLE grfidlef) PURE;

	/* Called by host just before it enters the 'wait mode' resulting from a
		call to WaitMessage, GetMessage, or MsgWaitForMultipleObjects.  Such a
		'wait mode' would prevent any components from receiving periodic idle 
		time.  If any registered components need periodic idle time, StdCompMgr
		starts an appropriate timer.  The resulting WM_TIMER message will cause
		the host	to exit the 'wait mode', allowing the processing of periodic 
		idle time tasks when the host calls IMsoStdComponentMgr::FDoIdle. */
	MSOMETHOD_(void, OnWaitForMessage) (THIS) PURE;

	/* Called by host during each iteration of a message loop that a component 
		pushed.
		This method is to be called after peeking the next message in the queue
		(via PeekMessage) but before the message is removed from the queue.
		The peeked message is passed in the pMsgPeeked param (NULL if no 
		message is in the queue).  If this method is additionally called when
		the next message has already been removed from the queue, pMsgPeeked 
		should be passed as NULL.
		StdComponentMgr in turn calls IMsoComponent::FContinueMessageLoop
		on the component, and returns the value returned by that call. 
		Returns TRUE if the message loop should continue, FALSE otherwise.
		If FALSE is returned, the loop should be terminated without removing
		pMsgPeeked from the queue. */
	MSOMETHOD_(BOOL, FContinueMessageLoop) (THIS_ MSG *pMsgPeeked) PURE;

	/* Called by host to determine if all registered components can terminate.
		StdComponentMgr asks each component whether it can terminate via
		IMsoComponent::FQueryTerminate(fPromptUser).  If all components
		return TRUE, then StdComponentMgr returns TRUE.  If any one of the
		components returns FALSE, then StdComponentMgr immediately returns
		FALSE without asking any more components. */
	MSOMETHOD_(BOOL, FQueryTerminate) (THIS_ BOOL fPromptUser) PURE;

	/* Called by host to terminate the StdComponentMgr.
		If fRevoke is TRUE and StdComponentMgr is registered as the current
		thread's component manager, this registration is revoked.
		Then, for each registered component, StdComponentMgr calls 
		IMsoComponent::Terminate and revokes its registration. */
	MSOMETHOD_(void, Terminate) (THIS_ BOOL fRevoke) PURE;
	};


/****************************************************************************
	Defines the IMsoComponentHost interface

	IMsoComponentHost is a host implemented interface that the standard
	component manager uses to communicate with the host.  By implementing
	this interface and making use of the standard component manager, the host
	can avoid having to implement its own component manager. 
****************************************************************************/
#undef  INTERFACE
#define INTERFACE  IMsoComponentHost

DECLARE_INTERFACE_(IMsoComponentHost, IUnknown)
	{
	BEGIN_MSOINTERFACE
	// *** IUnknown methods ***
	MSOMETHOD(QueryInterface) (THIS_ REFIID riid, void **ppvObj) PURE;
	MSOMETHOD_(ULONG, AddRef) (THIS) PURE;
	MSOMETHOD_(ULONG, Release) (THIS) PURE;

	/* StdComponentMgr delegates calls to IMsoComponentManager::QueryService
		to the host by calling this method.
		Return in *ppvObj an implementation of interface iid for service
		guidService (same as IServiceProvider::QueryService).
		Return NOERROR if the requested service is supported, otherwise return
		NULL in *ppvObj and an appropriate error (eg E_FAIL, E_NOINTERFACE). */
	MSOMETHOD(QueryService) (THIS_
		REFGUID guidService, REFIID iid, void **ppvObj) PURE;

	/* Standard FDebugMessage method.
	   Since IMsoComponentHost is a reference counted interface, 
	   MsoDWGetChkMemCounter should be used when processing the 
	   msodmWriteBe message. */
	MSODEBUGMETHOD

	/* Called when component pic is activated (or should be treated as active
		by virtue of beginning a "tracking" operation 
		(see IMsoComponentManager::FSetTrackingComponent)).  
		pcrinfo contains component's registration information.  
		fTracking indicates whether the component is in tracking mode or not.
		If pic is NULL (in which case pcrinfo will be NULL), then this 
		indicates that no component is active.  
		When a component is active and its registration info indicates that it
		needs to process untranslated messages, the host must call 
		IMsoStdComponentMgr::FPreTranslateMessage for each appropriate message
		retrieved from the queue before processing it, so that the active 
		component gets a chance to process the message appropriately.
		(A simple host could avoid examining the component's registration info,
		 and simply call IMsoStdComponentMgr::FPreTranslateMessage for all
		 retrieved messages.)
		If the component is in tracking mode, as indicated by fTracking, then
		the host app should perform no idle time processing other than to give
		the tracking component idle time via IMsoStdComponentMgr::FDoIdle,
		until the tracking operation is completed (communicated to host via a
		subsequent call to OnComponentActivate with fTracking == FALSE).

		Additionally, if pic is nonNULL and fTracking is FALSE, then host 
		should check if pcrinfo->grfcrf has the ExclusiveActive or 
		ExclusiveBorderSpace bit set.  
		
		If ExclusiveBorderSpace bit is set, then host should hide its border
		space tools (toolbars, status bars, etc.) and not show them again until
		the host is reactivated.
		
		If ExclusiveActivation bit is set, then pic is being activated in
		"ExclusiveActive" mode.
		Host should retrieve the top frame window that is hosting pic 
		(via pic->HwndGetWindow(msocWindowFrameToplevel, 0)).  
		If this window is different from host's own top frame window, 
			host should disable its windows and do other things it would do
			when receiving OnComponentEnterState(msocstateModal, TRUE)
			notification. 
		Otherwise, 
			host should refuse to have its window activated by appropriately
			processing WM_MOUSEACTIVATE (but see WM_MOUSEACTIVATE NOTE, above).
		Host should remain in this state until the ExclusiveActive mode ends, 
		indicated by a future call to OnComponentActivate (with FALSE 
		fTracking) with ExclusiveActivation bit not set or NULL pcrinfo. */
	MSOMETHOD_(void, OnComponentActivate) (THIS_
		IMsoComponent *pic, const MSOCRINFO *pcrinfo, BOOL fTracking) PURE;
	
	/* Notify host that a component is entering or exiting (indicated by 
		fEnter) the state identified by uStateID (a msocstateXXX value).  
		Host should take action depending on value of uStateID (see msocstate
		comments, above). 
		
		Note: If n calls are made with TRUE fEnter, the host should consider
		the state to be in effect until n calls are made with FALSE fEnter.

		Note: Hosts should be aware that it is possible for this method to
		be called with FALSE fEnter more	times than it was called with TRUE 
		fEnter (so, for example, if host is maintaining a state counter
		(incremented when this method is called with TRUE fEnter, decremented
		when called with FALSE fEnter), the counter should not be decremented
		for FALSE fEnter if it is already at zero.)  */
	MSOMETHOD_(void, OnComponentEnterState) (THIS_
		ULONG uStateID, BOOL fEnter) PURE;
	
	/* Called periodically by StdComponentMgr during 
		IMsoStdComponentMgr::FDoIdle.
		Return TRUE if idle time processing can continue, FALSE if not. */
	MSOMETHOD_(BOOL, FContinueIdle) (THIS) PURE;

	/* Called by StdComponentMgr when a component wishes to push a message
		loop for reason uReason.
		uReason is one the values from the msoloop enumeration (above).
		The host should push its message loop, calling
		IMsoStdComponentMgr::FContinueMessageLoop during each loop iteration 
		(see IMsoStdComponentMgr::FContinueMessageLoop comments).
		When IMsoStdComponentMgr::FContinueMessageLoop returns FALSE, the host
		should terminate the loop.
		If host terminates the loop because StdComponentMgr told it to
		(by returning FALSE from IMsoStdComponentMgr::FContinueMessageLoop),
		host should return TRUE from this method.  If host had to terminate
		the loop for some other reason, it should return FALSE. */
	MSOMETHOD_(BOOL, FPushMessageLoop) (THIS_ ULONG uReason) PURE;
	};



/****************************************************************************
	Structures and constants for simple recording.
****************************************************************************/

// Simple recorder context values passed to IMsoSimpleRecorder::FGetContext
enum
	{
	msosrctxCommandBars = 0,			// pv will be the IMsoToolbarSet object
	msosrctxEscher = 1,
	msosrctxDrawing = 2,
	msosrctxDrawingSelection = 3,
	msosrctxDrawingSchemeColor = 4,
	msosrctxDrawingDefault = 5,
	};

/****************************************************************************
	Defines the IMsoSimpleRecorder interface
****************************************************************************/
#undef  INTERFACE
#define INTERFACE  IMsoSimpleRecorder

DECLARE_INTERFACE_(IMsoSimpleRecorder, IUnknown)
{
	BEGIN_MSOINTERFACE
	// *** IUnknown methods ***
	MSOMETHOD(QueryInterface) (THIS_ REFIID riid, void **ppvObj) PURE;
	MSOMETHOD_(ULONG, AddRef) (THIS) PURE;
	MSOMETHOD_(ULONG, Release) (THIS) PURE;

	/* Standard FDebugMessage method.
	   Since IMsoSimpleRecorder is a reference counted interface, 
	   MsoDWGetChkMemCounter is used when processing the 
	   msodmWriteBe message. */
	MSODEBUGMETHOD

	/* Returns a string specifying the position of the requested context
	   in the host's object model.  The context is specified by msosrctx, 
	   and the additional value pv if needed to disambiguate different 
	   objects of that type.  The string buffer should have room for
	   255 characters.  Returns TRUE if successful, FALSE otherwise. */
	MSOMETHOD_(BOOL, FGetContext)(THIS_ int msosrctx, void *pv, WCHAR *wz) PURE;

	/* Asks the host to record a line of text at the current position
	   in the recorder stream.  Returns TRUE if the line was successfully
	   recorded, FALSE otherwise. */
	MSOMETHOD_(BOOL, FRecordLine)(THIS_ WCHAR *wz) PURE;

	/* Returns TRUE if the host currently has recording turned on, and
	   FALSE otherwise. */
	MSOMETHOD_(BOOL, FRecording)(THIS) PURE;
};



/****************************************************************************
	Global DLL API's
****************************************************************************/

/* Called by host application to register picm as the component manager for
	the calling thread.  Standard reference counting rules apply.  
	picm can be NULL, indicating that the currently	registered component 
	manager should be revoked.
	If there is a previously registered component manager it is Released.
	Returns TRUE if successful. */
MSOAPI_(BOOL) MsoFSetComponentManager(IMsoComponentManager *picm);

/* Return in *ppicm an AddRef'ed pointer to the currently registered
	component manager for the calling thread.
	Returns TRUE if successful. */
MSOAPI_(BOOL) MsoFGetComponentManager(IMsoComponentManager **ppicm);

/* Create an object which has the Office supplied standard implementation
	of IMsoStdComponentMgr and IMsoComponentManager, and return it in *ppvObj.
	piUnkOuter is the controlling unknown for aggregation (can be NULL).
	pich (cannot be NULL) is the pointer to the IMsoComponentHost
	that will use the object.
	riid is the id of the desired interface.
	If fRegister is TRUE, the created object is registered as the calling
	thread's component manager.
	Returns TRUE if successful. */
MSOAPI_(BOOL) MsoFCreateStdComponentManager(
	IUnknown *piUnkOuter,
	IMsoComponentHost *pich,
	BOOL fRegister,
	REFIID riid,
	void **ppvObj);

#endif // MSOCI_H
