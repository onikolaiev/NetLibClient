using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace netLogic.Constants
{

    [Flags]
    public enum MovementFlags : ulong
    {
        None = 0x0,
        Forward = 0x1,
        Backward = 0x2,
        StrafeLeft = 0x4,
        StrafeRight = 0x8,
        TurnLeft = 0x10,
        TurnRight = 0x20,
        PitchUp = 0x40,
        PitchDown = 0x80,
        WalkMode = 0x100,
        OnTransport = 0x200,
        DisableGravity = 0x400,
        Root = 0x800,
        Falling = 0x1000,
        FallingFar = 0x2000,
        PendingStop = 0x4000,
        PendingStrafeStop = 0x8000,
        PendingForward = 0x10000,
        PendingBackward = 0x20000,
        PendingStrafeLeft = 0x40000,
        PendingStrafeRight = 0x80000,
        PendingRoot = 0x100000,
        Swimming = 0x200000,
        Ascending = 0x400000,
        Descending = 0x800000,
        CanFly = 0x1000000,
        Flying = 0x2000000,
        /// <summary>
        /// Used For Flight Paths, also called WalkableRedirection?
        /// </summary>
        SplineElevation = 0x4000000,
        SplineEnabled = 0x8000000,
        Waterwalking = 0x10000000,
        CanSafeFall = 0x20000000,
        Hover = 0x40000000,
        /// <summary>
        /// Client has changes that haven't been sent back to server. Server should never see this flag
        /// </summary>
        LocalDirty = 0x80000000,

        MaskDirections = Forward | Backward | StrafeLeft | StrafeRight | TurnLeft | TurnRight | PitchUp | PitchDown,
        MaskMoving = Forward | Backward | StrafeLeft | StrafeRight | TurnLeft | TurnRight | Falling | FallingFar | Ascending | Descending,


        // GameObject Specific Flags - Never seen or influenced directly by the server, client-side only.
        AwaitingLoad = 0x08000000,
    }

    [Flags]
    public enum MovementFlags2 : ushort
    {
        None = 0x0000,
        PreventStrafe = 0x0001,	// TODO: Recheck preventing AscendDescend. Prevent strafe confirmed
        PreventJumping = 0x0002,
        DisableCollision = 0x0004,
        FullSpeedTurning = 0x0008,
        FullSpeedPitching = 0x0010,
        AlwaysAllowPitching = 0x0020,
        IsVehicleExitVoluntary = 0x0040,
        IsJumpSplineInAir = 0x0080,
        IsAnimTierInTrans = 0x0100,
        PreventChangePitch = 0x0200,
        InterpolateMove = 0x0400,	// Interpolation is player only
        InterpolateTurning = 0x0800,
        InterpolatePitching = 0x1000,
        VehiclePassengerIsTransitionAllowed = 0x2000,
        CanTransitionBetweenSwimAndFly = 0x4000,
        Flag_0x8000 = 0x8000,	// Used in CGUnit_C::UpdateFlightStatus, prevent flight and swimming? or prevent falling?

        // The client also reuses some flags to indicate other states when
        // sending movement packets back to the server 
        Status_400 = 0x400,	//	This flag means that the "TransportTime2" field of the CMovementStatus should be used
        Status_800 = 0x800,	//	This flag means the client is currently falling


        InterpMask = InterpolateMove | InterpolateTurning | InterpolatePitching
    }



}
