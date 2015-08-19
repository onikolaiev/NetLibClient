using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace netLogic.Constants
{
    [Flags]
    public enum SplineFlags : uint
    {
        None = 0x00000000,

        // 0x00 - 0x7 is the AnimationTier

        Flag_0x8 = 0x00000008,
        SplineSafeFall = 0x00000010,
        Flag_0x20 = 0x00000020,
        Flag_0x40 = 0x00000040,
        Flag_0x80 = 0x00000080,
        Done = 0x00000100,
        Falling = 0x00000200,	// Affects elevation computation, can't be combined with Parabolic flag
        NotSplineMover = 0x00000400,
        Parabolic = 0x00000800,	// Affects elevation computation, can't be combined with Falling flag. Alternate name is Jumping
        Walkmode = 0x00001000,
        Flying = 0x00002000,
        Knockback = 0x00004000,	// Model orientation fixed
        FinalFacePoint = 0x00008000,
        FinalFaceTarget = 0x00010000,
        FinalFaceAngle = 0x00020000,
        CatmullRom = 0x00040000,
        Cyclic = 0x00080000,	// Movement by cycled spline
        EnterCycle = 0x00100000,	// Everytimes appears with cyclic flag in monster move packet, erases first spline vertex after first cycle done
        AnimationTier = 0x00200000,	// Plays animation after some time passed
        Frozen = 0x00400000,	// Will never arrive
        Unknown5 = 0x00800000,	// Transport?
        Unknown6 = 0x01000000,	// ExitVehicle?
        Unknown7 = 0x02000000,
        Unknown8 = 0x04000000,
        Backward = 0x08000000,
        UsePathSmoothing = 0x10000000,	// SplineType 2
        SplineCanSwim = 0x20000000,
        UncompressedPath = 0x40000000,	// Read pathpoints uncompressed in monster move
        Unknown13 = 0x80000000
    }

    public enum SplineMode : byte
    {
        Linear = 0,
        CatmullRom = 1,
        Bezier3 = 2
    }

    public enum SplineType : byte
    {
        Normal = 0,
        Stop = 1,
        FacingSpot = 2,
        FacingTarget = 3,
        FacingAngle = 4
    }
}
