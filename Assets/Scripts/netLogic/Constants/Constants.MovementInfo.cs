using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.IO;
using netLogic.Shared;
using UnityEngine;

namespace netLogic.Constants
{

}
/*

public static MovementInfo Read(OBJECT_TYPE_ID objtypeid , GameGuid uguid, BinaryReader gr)
        {
            var movement = new MovementInfo();
           
            Object obj = (Object)netLogicCore.ObjMgr.GetObj(uguid);

            Unit u = null;
            if (obj)
            {
                if (obj.IsUnit())
                {
                    u = (Unit)obj;

                }
                else return null;
            }


            movement.UpdateFlags = (UpdateFlags)gr.ReadUInt16();

            if (movement.UpdateFlags.HasFlag(UpdateFlags.UPDATEFLAG_LIVING))
            {
                movement.Flags = (MovementFlags)gr.ReadUInt32();
                movement.Flags2 = (MovementFlags2)gr.ReadUInt16();
                movement.TimeStamp = gr.ReadUInt32();

                movement.Position = gr.ReadCoords3();
                movement.Facing = gr.ReadSingle();

                if (obj && obj.IsWorldObject())
                    ((WorldObject)obj).SetCoordinates(movement.Position);

                if (movement.Flags.HasFlag(MovementFlags.ONTRANSPORT))
                {
                    movement.Transport = TransportInfo.Read(gr, movement.Flags2);
                }

                if (movement.Flags.HasFlag(MovementFlags.SWIMMING) || movement.Flags.HasFlag(MovementFlags.FLYING) ||
                    movement.Flags2.HasFlag(MovementFlags2.AlwaysAllowPitching))
                {
                    movement.Pitch = gr.ReadSingle();
                }

                movement.FallTime = gr.ReadUInt32();

                if (movement.Flags.HasFlag(MovementFlags.FALLING))
                {
                    movement.FallVelocity = gr.ReadSingle();
                    movement.FallCosAngle = gr.ReadSingle();
                    movement.FallSinAngle = gr.ReadSingle();
                    movement.FallSpeed = gr.ReadSingle();
                }

                if (movement.Flags.HasFlag(MovementFlags.SPLINEELEVATION))
                {
                    movement.SplineElevation = gr.ReadSingle();
                }


                for (byte i = 0; i < movement.speeds.Length; ++i)
                    movement.speeds[i] = gr.ReadSingle();
                
                    movement.speedWalk = gr.ReadSingle();
                    movement.speedRun = gr.ReadSingle();
                    movement.speedSwimBack = gr.ReadSingle();
                    movement.speedSwim = gr.ReadSingle();
                    movement.speedWalkBack = gr.ReadSingle();
                    movement.speedFly = gr.ReadSingle();
                    movement.speedFlyBack = gr.ReadSingle();
                    movement.speedTurn = gr.ReadSingle();
                    movement.speedPitchRate = gr.ReadSingle();
                    
                
                    u.SetCoordinates(movement.Position);
                    u.SetSpeed(UnitMoveType.MOVE_WALK, movement.speedWalk);
                    u.SetSpeed(UnitMoveType.MOVE_RUN, movement.speedRun);
                    u.SetSpeed(UnitMoveType.MOVE_SWIMBACK, movement.speedSwimBack);
                    u.SetSpeed(UnitMoveType.MOVE_SWIM, movement.speedSwim);
                    u.SetSpeed(UnitMoveType.MOVE_WALKBACK, movement.speedWalkBack);
                    u.SetSpeed(UnitMoveType.MOVE_TURN, movement.speedTurn);
                    u.SetSpeed(UnitMoveType.MOVE_FLY, movement.speedFly);
                    u.SetSpeed(UnitMoveType.MOVE_FLYBACK, movement.speedFlyBack);
                    u.SetSpeed(UnitMoveType.MOVE_PITCH_RATE, movement.speedPitchRate);


                if (movement.Flags.HasFlag(MovementFlags.SPLINEENABLED))
                {
                    movement.Spline = SplineInfo.Read(gr);
                }
            }
            else
            {
                if (movement.UpdateFlags.HasFlag(UpdateFlags.UPDATEFLAG_GO_POSITION))
                {
                    movement.Transport.Guid = gr.ReadPackedGuid();
                    movement.Position = gr.ReadCoords3();
                    movement.Transport.Position = gr.ReadCoords3();
                    movement.Facing = gr.ReadSingle();
                    movement.Transport.Facing = gr.ReadSingle();
                    if (obj && obj.IsWorldObject())
                        ((WorldObject)obj).SetCoordinates(movement.Position);
                }
                else if (movement.UpdateFlags.HasFlag(UpdateFlags.UPDATEFLAG_HAS_POSITION))
                {
                    movement.Position = gr.ReadCoords3();
                    movement.Facing = gr.ReadSingle();
                    if (obj && obj.IsWorldObject())
                        ((WorldObject)obj).SetCoordinates(movement.Position);
                }
            }

            if (movement.UpdateFlags.HasFlag(UpdateFlags.UPDATEFLAG_LOWGUID))
            {
                movement.LowGuid = gr.ReadUInt32();
            }

            if (movement.UpdateFlags.HasFlag(UpdateFlags.UPDATEFLAG_HIGHGUID))
            {
                movement.HighGuid = gr.ReadUInt32();
            }

            if (movement.UpdateFlags.HasFlag(UpdateFlags.UPDATEFLAG_TARGET_GUID))
            {
                movement.AttackingTarget = gr.ReadPackedGuid();
            }

            if (movement.UpdateFlags.HasFlag(UpdateFlags.UPDATEFLAG_TRANSPORT))
            {
                movement.TransportTime = gr.ReadUInt32();
            }

            // WotLK
            if (movement.UpdateFlags.HasFlag(UpdateFlags.UPDATEFLAG_VEHICLE))
            {
                movement.VehicleId = gr.ReadUInt32();
                movement.VehicleAimAdjustement = gr.ReadSingle();
            }

            // 3.1
            if (movement.UpdateFlags.HasFlag(UpdateFlags.UPDATEFLAG_GO_ROTATION))
            {
                movement.GoRotationULong = gr.ReadUInt64(); // fixme
            }
            return movement;
        }

*/