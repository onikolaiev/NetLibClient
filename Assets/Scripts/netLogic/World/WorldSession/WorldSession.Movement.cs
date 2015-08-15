using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Timers;

using System.Runtime.InteropServices;
using System.Resources;

using netLogic.Shared;
using netLogic.Network;
using netLogic.Constants;

namespace netLogic
{
    public partial class WorldSession
    {

        [PacketHandlerAtribute(WorldServerOpCode.MSG_MOVE_START_FORWARD)]
        [PacketHandlerAtribute(WorldServerOpCode.MSG_MOVE_START_BACKWARD)]
        [PacketHandlerAtribute(WorldServerOpCode.MSG_MOVE_STOP)]
        [PacketHandlerAtribute(WorldServerOpCode.MSG_MOVE_START_STRAFE_LEFT)]
        [PacketHandlerAtribute(WorldServerOpCode.MSG_MOVE_START_STRAFE_RIGHT)]
        [PacketHandlerAtribute(WorldServerOpCode.MSG_MOVE_STOP_STRAFE)]
        [PacketHandlerAtribute(WorldServerOpCode.MSG_MOVE_JUMP)]
        [PacketHandlerAtribute(WorldServerOpCode.MSG_MOVE_START_TURN_LEFT)]
        [PacketHandlerAtribute(WorldServerOpCode.MSG_MOVE_START_TURN_RIGHT)]
        [PacketHandlerAtribute(WorldServerOpCode.MSG_MOVE_STOP_TURN)]
        [PacketHandlerAtribute(WorldServerOpCode.MSG_MOVE_START_PITCH_UP)]
        [PacketHandlerAtribute(WorldServerOpCode.MSG_MOVE_START_PITCH_DOWN)]
        [PacketHandlerAtribute(WorldServerOpCode.MSG_MOVE_STOP_PITCH)]
        [PacketHandlerAtribute(WorldServerOpCode.MSG_MOVE_SET_RUN_MODE)]
        [PacketHandlerAtribute(WorldServerOpCode.MSG_MOVE_SET_WALK_MODE)]
        [PacketHandlerAtribute(WorldServerOpCode.MSG_MOVE_TOGGLE_LOGGING)]
        [PacketHandlerAtribute(WorldServerOpCode.MSG_MOVE_TELEPORT)]
        [PacketHandlerAtribute(WorldServerOpCode.MSG_MOVE_TELEPORT_CHEAT)]
        [PacketHandlerAtribute(WorldServerOpCode.MSG_MOVE_TELEPORT_ACK)]
        [PacketHandlerAtribute(WorldServerOpCode.MSG_MOVE_TOGGLE_FALL_LOGGING)]
        [PacketHandlerAtribute(WorldServerOpCode.MSG_MOVE_FALL_LAND)]
        [PacketHandlerAtribute(WorldServerOpCode.MSG_MOVE_START_SWIM)]
        [PacketHandlerAtribute(WorldServerOpCode.MSG_MOVE_STOP_SWIM)]
        [PacketHandlerAtribute(WorldServerOpCode.MSG_MOVE_HEARTBEAT)]
        public void HandleAnyMove(PacketIn packet)
        {
            /*ulong guid = packet.ReadPackedGuid();
            MMOObject obj = netLogicCore.ObjectMgr.GetMMOObjectByGuid(guid);
            if (obj != null)
            {
                var movement = MovementInfo.Read(packet);
                obj.SetNewMovement(movement);
            }*/
            byte mask = packet.ReadByte();

            GameGuid guid = new GameGuid(mask, packet.ReadBytes(GameGuid.BitCount8(mask)));
            GetInstance().ObjMgr().GetObj(guid);
            //netLogicCore.ObjectMgr.GetObject(guid).SetNewMovement(MovementInfo.Read(packet));
          /*  if (obj != null)
            {
                obj.coord = new Coordinate(packet.ReadFloat(), packet.ReadFloat(), packet.ReadFloat());
            }*/
        }
       
        [PacketHandlerAtribute(WorldServerOpCode.SMSG_MONSTER_MOVE)]
        public void HandleMonsterMove(PacketIn packet)
        {
           /* ulong guid = packet.ReadPackedGuid();
            MMOObject obj = netLogicCore.ObjectMgr.GetMMOObjectByGuid(guid);
            if (obj != null)
            {
                var movement = MovementInfo.Read(packet);
                obj.SetNewMovement(movement);
            }*/
            
         //   byte mask = packet.ReadByte();

          //  WoWGuid guid = new WoWGuid(mask, packet.ReadBytes(WoWGuid.BitCount8(mask)));
            byte mask = packet.ReadByte();

            GameGuid guid = new GameGuid(mask, packet.ReadBytes(GameGuid.BitCount8(mask)));
            
            Unit u = (Unit)GetInstance().ObjMgr().GetObj(guid);
           // u.SetCoordinates(MovementInfo.Read(packet).Position);



            //netLogicCore.ObjectMgr.GetObject(guid).SetNewMovement(MovementInfo.Read(packet));
            /*
            MMOObject obj = netLogicCore.ObjectMgr.GetMMOObjectByGuid(guid);
            if (obj != null)
            {
                obj.coord = new Coordinate(packet.ReadFloat(), packet.ReadFloat(), packet.ReadFloat());
            }*/
        }






































        void Heartbeat(object source, ElapsedEventArgs e)
        {
            Loom.DispatchToMainThread(() =>
            {
                MyCharacter _ch = GetMyChar();
                PacketOut packet = new PacketOut(WorldServerOpCode.MSG_MOVE_HEARTBEAT);
                packet.Write(_ch.HighGuid);
                packet.Write((UInt64)_ch.Flags);
                packet.Write((byte)0);
                packet.Write((UInt32)MM_GetTime());
                packet.Write(_ch.transform.position.x);
                packet.Write(_ch.transform.position.z);
                packet.Write(_ch.transform.position.y);
                packet.Write(_ch.transform.rotation.y);
                packet.Write((UInt32)0);
                Send(packet);
            });
        }

        


        //////////////////////////////////////////////////////////
        //////////////////////////////////////////////////////////
        //////////////////////////////////////////////////////////


        /*






        public enum MovementFlags : ulong
        {
            // Byte 1 (Resets on Movement Key Press)
            MOVEFLAG_MOVE_STOP = 0x00,			//verified
            MOVEFLAG_MOVE_FORWARD = 0x01,			//verified
            MOVEFLAG_MOVE_BACKWARD = 0x02,			//verified
            MOVEFLAG_STRAFE_LEFT = 0x04,			//verified
            MOVEFLAG_STRAFE_RIGHT = 0x08,			//verified
            MOVEFLAG_TURN_LEFT = 0x10,			//verified
            MOVEFLAG_TURN_RIGHT = 0x20,			//verified
            MOVEFLAG_PITCH_DOWN = 0x40,			//Unconfirmed
            MOVEFLAG_PITCH_UP = 0x80,			//Unconfirmed

            // Byte 2 (Resets on Situation Change)
            MOVEFLAG_WALK = 0x100,		//verified
            MOVEFLAG_TAXI = 0x200,
            MOVEFLAG_NO_COLLISION = 0x400,
            MOVEFLAG_FLYING = 0x800,		//verified
            MOVEFLAG_REDIRECTED = 0x1000,		//Unconfirmed
            MOVEFLAG_FALLING = 0x2000,       //verified
            MOVEFLAG_FALLING_FAR = 0x4000,		//verified
            MOVEFLAG_FREE_FALLING = 0x8000,		//half verified

            // Byte 3 (Set by server. TB = Third Byte. Completely unconfirmed.)
            MOVEFLAG_TB_PENDING_STOP = 0x10000,		// (MOVEFLAG_PENDING_STOP)
            MOVEFLAG_TB_PENDING_UNSTRAFE = 0x20000,		// (MOVEFLAG_PENDING_UNSTRAFE)
            MOVEFLAG_TB_PENDING_FALL = 0x40000,		// (MOVEFLAG_PENDING_FALL)
            MOVEFLAG_TB_PENDING_FORWARD = 0x80000,		// (MOVEFLAG_PENDING_FORWARD)
            MOVEFLAG_TB_PENDING_BACKWARD = 0x100000,		// (MOVEFLAG_PENDING_BACKWARD)
            MOVEFLAG_SWIMMING = 0x200000,		//  verified
            MOVEFLAG_FLYING_PITCH_UP = 0x400000,		// (half confirmed)(MOVEFLAG_PENDING_STR_RGHT)
            MOVEFLAG_TB_MOVED = 0x800000,		// (half confirmed) gets called when landing (MOVEFLAG_MOVED)

            // Byte 4 (Script Based Flags. Never reset, only turned on or off.)
            MOVEFLAG_AIR_SUSPENSION = 0x1000000,	// confirmed allow body air suspension(good name? lol).
            MOVEFLAG_AIR_SWIMMING = 0x2000000,	// confirmed while flying.
            MOVEFLAG_SPLINE_MOVER = 0x4000000,	// Unconfirmed
            MOVEFLAG_IMMOBILIZED = 0x8000000,
            MOVEFLAG_WATER_WALK = 0x10000000,
            MOVEFLAG_FEATHER_FALL = 0x20000000,	// Does not negate fall damage.
            MOVEFLAG_LEVITATE = 0x40000000,
            MOVEFLAG_LOCAL = 0x80000000,	// This flag defaults to on. (Assumption)

            // Masks
            MOVEFLAG_MOVING_MASK = 0x03,
            MOVEFLAG_STRAFING_MASK = 0x0C,
            MOVEFLAG_TURNING_MASK = 0x30,
            MOVEFLAG_FALLING_MASK = 0x6000,
            MOVEFLAG_MOTION_MASK = 0xE00F,		// Forwards, Backwards, Strafing, Falling
            MOVEFLAG_PENDING_MASK = 0x7F0000,
            MOVEFLAG_PENDING_STRAFE_MASK = 0x600000,
            MOVEFLAG_PENDING_MOVE_MASK = 0x180000,
            MOVEFLAG_FULL_FALLING_MASK = 0xE000,
        };


        public UInt16 MoveMask = 0;
        private ulong MoveFlags = 0;

        private Object TrackObj = null;

        public void TrackObject(Object o)
        {
            if (o == null)
                return;

            TrackObj = o;
        }

        public void StopTrack()
        {
            TrackObj = null;
        }

        public Object GetTrackedObject()
        {
            if (TrackObj == null)
                return null;

            return TrackObj;
        }

        private void TeleportHandler(PacketIn pi)
        {
            float x, y, z, orient;
            byte mask = pi.ReadByte();

            WoWGuid guid = new WoWGuid(mask, pi.ReadBytes(WoWGuid.BitCount8(mask)));

            pi.ReadUInt32(); // flags

            pi.ReadUInt32(); // time?
            pi.ReadByte(); // unk 2.3.0

            pi.ReadSingle(); // unk2
            x = pi.ReadSingle();
            y = pi.ReadSingle();
            z = pi.ReadSingle();
            orient = pi.ReadSingle();
            pi.ReadUInt16(); // unk3
            pi.ReadByte(); // unk4
            Log(LogType.Debug, "Got teleport to: {0} {1} {2} {3}", x, y, z, orient);

            objectMgr.getPlayerObject().SetCoordinates(new Coordinate(x, y, z, orient));

            
           
             ///WoWWriter ww = new WoWWriter(ServerMessage.MSG_MOVE_TELEPORT_ACK);
             // ww.Write(BoogieCore.world.getPlayerObject().GUID.GetOldGuid());
             // Send(ww.ToArray());
             // SendMoveHeartBeat(BoogieCore.world.getPlayerObject().GetCoordinates());
        
        }
    
        public void SendMoveHeartBeat()
        {
            SendMoveHeartBeat(BoogieCore.World.getPlayerObject().GetCoordinates());
        }


        public void SendMoveHeartBeat(Coordinate c)
        {
            SendMoveHeartBeat(c.X, c.Y, c.Z, c.O);
        }

        public void SendMoveHeartBeat(float x, float y, float z, float o)
        {
            // BuildMovePacket(ServerMessage.MSG_MOVE_HEARTBEAT, x, y, z, o);
        }
        public void SetMoveFlag(MovementFlags flag)
        {
            MoveFlags |= (ulong)flag;
        }
        public void UnSetMoveFlag(MovementFlags flag)
        {
            MoveFlags &= ~(ulong)flag;
        }
        public bool IsMoveFlagSet(MovementFlags flag)
        {
            return ((MoveFlags & (ulong)flag) >= 1) ? true : false;
        }
        public void StartMoveForward()
        {
            SetMoveFlag(MovementFlags.MOVEFLAG_MOVE_FORWARD);
            //   BuildMovePacket(ServerMessage.MSG_MOVE_START_FORWARD, BoogieCore.World.getPlayerObject().GetCoordinates());
        }

        public void StopMoveForward()
        {
            SetMoveFlag(MovementFlags.MOVEFLAG_MOVE_STOP);
            // BuildMovePacket(ServerMessage.MSG_MOVE_STOP, BoogieCore.World.getPlayerObject().GetCoordinates());
            MoveFlags = 0;
        }

        public void MoveJump()
        {
            //  BuildMovePacket(ServerMessage.MSG_MOVE_JUMP, BoogieCore.world.getPlayerObject().GetCoordinates());
        }
        
        public void BuildMovePacket(OpCode op)
        {
            BuildMovePacket(op, BoogieCore.World.getPlayerObject().GetCoordinates());
        }
        public void BuildMovePacket(OpCode op, Coordinate c)
        {
            BuildMovePacket(op, c.X, c.Y, c.Z, c.O);
        }

        private void BuildMovePacket(OpCode op, float x, float y, float z, float o)
        {
            WoWWriter ww = new WoWWriter(op);
            ww.Write((UInt32)MoveFlags);
            ww.Write((byte)255);
            ww.Write((UInt32)MM_GetTime());

            ww.Write(x);
            ww.Write(y);
            ww.Write(z);
            ww.Write(o);

            ww.Write((UInt32)0);

            Send(ww.ToArray());
        }

        private void MovementHandler(WoWReader wr)
        {
            WoWGuid guid;
            byte mask = wr.ReadByte();

            if (mask == 0x00)
                return;

            guid = new WoWGuid(mask, wr.ReadBytes(WoWGuid.BitCount8(mask)));

            MovementInfo mi = new MovementInfo(wr);

            if (BoogieCore.world.getObject(guid) != null)
            {
                //BoogieCore.Log(LogType.Error, "Updating coordinates for object {0}, x={1} y={2} z={3}", BoogieCore.world.getObject(guid).Name, mi.x, mi.y, mi.z);
                BoogieCore.world.getObject(guid).SetCoordinates(mi.GetCoordinates());
            }
        }


        public void UpdatePosition(UInt32 diff)
        {

            if (TrackObj != null)
            {
                Object player = BoogieCore.world.getPlayerObject();
                player.SetOrientation(
                        player.CalculateAngle(
                            TrackObj.GetPositionX(), TrackObj.GetPositionY()
                        )
                    );

            }

            if (MoveFlags == 0)
                return; // no need to predict coordinates if we aint movin', yo

            BoogieCore.Log(LogType.System, "UpdatePos diff: {0}", diff);
            float predictedDX = 0;
            float predictedDY = 0;

            if (oldLocation == null)
                oldLocation = BoogieCore.world.getPlayerObject().GetCoordinates();

            // update predicted location
            double h; double speed;
            h = BoogieCore.world.getPlayerObject().GetOrientation();
            speed = 7.0;//BoogieCore.world.getPlayerObject().runSpeed;

            float dt = (float)diff / 1000f;
            float dx = (float)Math.Cos(h) * (float)speed * dt;
            float dy = (float)Math.Sin(h) * (float)speed * dt;

            BoogieCore.Log(LogType.System, "speed: {0} dt: {1} dx: {2} dy : {3}", speed, dt, dx, dy);

            predictedDX = dx;
            predictedDY = dy;

            Coordinate loc = BoogieCore.world.getPlayerObject().GetCoordinates();
            float realDX = loc.X - oldLocation.X;
            float realDY = loc.Y - oldLocation.Y;
            BoogieCore.Log(LogType.System, " dx: " + predictedDX + " dy : " + predictedDY + " Real dx: " + realDX + " dy : " + realDY);

            float predictDist = (float)Math.Sqrt(predictedDX * predictedDX + predictedDY * predictedDY);
            float realDist = (float)Math.Sqrt(realDX * realDX + realDY * realDY);

            BoogieCore.Log(LogType.System, "predict dist: {0} real dist: {1}", predictDist, realDist);
            if (predictDist > 0.0)
            {
                Coordinate expected = new Coordinate(loc.X + predictedDX, loc.Y + predictedDY, BoogieCore.world.getPlayerObject().GetPositionZ(), BoogieCore.world.getPlayerObject().GetOrientation());

                BoogieCore.Log(LogType.System, "new loc x {0}, y {1}, z {2}", expected.X, expected.Y, expected.Z);
                BoogieCore.world.getPlayerObject().SetCoordinates(expected);
            }

            oldLocation = loc;
        }
    }

    public class MovementInfo
    {
        public UInt32 time;

        public UInt32 unk8, unk9, unk10, unk11, unk12;
        public UInt32 unklast;
        public float unk6;
        public float x, y, z, orientation;
        public UInt32 flags;
        public UInt32 FallTime;
        public UInt64 transGuid;
        public float transX, transY, transZ, transO;

        public Coordinate GetCoordinates()
        {
            return new Coordinate(x, y, z, orientation);
        }

        public MovementInfo(WoWReader wr)
        {
            transGuid = 0;
            flags = wr.ReadUInt32();
            wr.ReadByte();
            time = wr.ReadUInt32();

            x = wr.ReadFloat();
            y = wr.ReadFloat();
            z = wr.ReadFloat();
            orientation = wr.ReadFloat();

            if ((flags & 0x2000000) >= 1) // Transport
            {
                transGuid = wr.ReadUInt64();

                transX = wr.ReadFloat();
                transY = wr.ReadFloat();
                transZ = wr.ReadFloat();
                transO = wr.ReadFloat();
            }

            if ((flags & 0x200000) >= 1) // Swimming
            {
                unk6 = wr.ReadFloat();
            }

            if ((flags & 0x2000) >= 1) // Falling
            {
                FallTime = wr.ReadUInt32();
                unk8 = wr.ReadUInt32();
                unk9 = wr.ReadUInt32();
                unk10 = wr.ReadUInt32();
            }

            if ((flags & 0x4000000) >= 1)
            {
                unk12 = wr.ReadUInt32();
            }

            //if (wr.Remaining >= 4) unklast = wr.ReadUInt32();

        }



        */



    }
}

