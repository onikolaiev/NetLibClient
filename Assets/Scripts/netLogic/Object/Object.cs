using Frankfort.Threading;
using netLogic.Constants;
using netLogic.Network;
using netLogic.Shared;
using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using UnityEngine;

namespace netLogic
{
    #region ObjectTypes
    /// <summary>
    /// Game object types.
    /// </summary>
    public enum OBJECT_TYPE:uint
    {
        TYPE_OBJECT = 1,
        TYPE_ITEM = 2,
        TYPE_CONTAINER = 6, // a container is ALWAYS an item!
        TYPE_UNIT = 8,
        TYPE_PLAYER = 16,
        TYPE_GAMEOBJECT = 32,
        TYPE_DYNAMICOBJECT = 64,
        TYPE_CORPSE = 128,
        TYPE_AIGROUP = 256,
        TYPE_AREATRIGGER = 512
    }

    public enum OBJECT_TYPE_ID:uint
    {
        /// <summary>
        /// An object.
        /// </summary>
        TYPEID_OBJECT = 0,
        /// <summary>
        /// Item.
        /// </summary>
        TYPEID_ITEM = 1,
        /// <summary>
        /// Container (item).
        /// </summary>
        TYPEID_CONTAINER = 2,
        /// <summary>
        /// Unit.
        /// </summary>
        TYPEID_UNIT = 3,
        /// <summary>
        /// Player (unit).
        /// </summary>
        TYPEID_PLAYER = 4,
        /// <summary>
        /// Game object.
        /// </summary>
        TYPEID_GAMEOBJECT = 5,
        /// <summary>
        /// Dynamic object.
        /// </summary>
        TYPEID_DYNAMICOBJECT = 6,
        /// <summary>
        /// Player corpse (not used for units?).
        /// </summary>
        TYPEID_CORPSE = 7,
        TYPEID_AIGROUP = 8,
        TYPEID_AREATRIGGER = 9
    }

    [Flags]
    public enum OBJECT_TYPE_MASK
    {
        TYPEMASK_NONE = 0x0000,
        TYPEMASK_OBJECT = 0x0001,
        TYPEMASK_ITEM = 0x0002,
        TYPEMASK_CONTAINER = 0x0004,
        TYPEMASK_UNIT = 0x0008,
        TYPEMASK_PLAYER = 0x0010,
        TYPEMASK_GAMEOBJECT = 0x0020,
        TYPEMASK_DYNAMICOBJECT = 0x0040,
        TYPEMASK_CORPSE = 0x0080,
        TYPEMASK_AIGROUP = 0x0100,
        TYPMASKE_AREATRIGGER = 0x0200
    };
    #endregion

    interface a
    { 
    
    }

    #region Object Update
    public class ObjectUpdate :  MonoBehaviour , IThreadWorkerObject
    {
        public IDictionary<int, uint> Data { get; set; }

        public Transform gObject;
        protected ObjectUpdate(IDictionary<int, uint> data)
        {
            Data = data;
        }


        protected ObjectUpdate()
        {
            

        }

        public void ReadUpd(BinaryReader gr)
        {
            
            byte blocksCount = gr.ReadByte();
            var updatemask = new int[blocksCount];

            for (int i = 0; i < updatemask.Length; ++i)
                updatemask[i] = gr.ReadInt32();

            var mask = new BitArray(updatemask);

            //var values = new Dictionary<int, uint>();

            for (int i = 0; i < mask.Count; ++i)
                if (mask[i])
                    Data[i] = gr.ReadUInt32();

          //  Data = values;
        }
    }
    #endregion

    public class Object : ObjectUpdate
    {
        #region Private var
        
        private GameGuid GUID;
        private UInt32 EntryId;
        protected OBJECT_TYPE _type;
        protected OBJECT_TYPE_ID _typeId;
        public string _name;
        
        
        #endregion

        #region Public vars
        public UpdateFlags UpdateFlags { get; private set; }
        public MovementFlags Flags { get;  set; }
        public MovementFlags2 Flags2 { get;  set; }
        public uint TimeStamp { get; set; }
        public Vector3 Position { get; set; }
        public float Facing { get; private set; }
        public TransportInfo Transport { get; private set; }
        public float Pitch { get; private set; }
        public uint FallTime { get; private set; }
        public float FallSinAngle { get; private set; }
        public float FallCosAngle { get; private set; }
        public float FallVelocity { get; private set; }
        public float FallSpeed { get; private set; }
        public float SplineElevation { get; private set; }
        public readonly float[] speeds = new float[9];
        public SplineInfo Spline { get; private set; }
        public uint LowGuid { get; private set; }
        public uint HighGuid { get; private set; }
        public ulong AttackingTarget { get; private set; }
        public uint TransportTime { get; private set; }
        public uint VehicleId { get; private set; }
        public float VehicleAimAdjustement { get; private set; }
        public ulong GoRotationULong { get; private set; }
        public float speedWalk { get; private set; }
        public float speedRun { get; private set; }
        public float speedSwimBack { get; private set; }
        public float speedSwim { get; private set; }
        public float speedWalkBack { get; private set; }
        public float speedFly { get; private set; }
        public float speedFlyBack { get; private set; }
        public float speedTurn { get; private set; }
        public float speedPitchRate { get; private set; }
        private ulong MoveFlags = 0;



        #endregion


        #region Public methods
        public bool _depleted; // true if the object was deleted from the objmgr, but not from memory
        public GameGuid GetGUID()                 { return GUID; }
        public UInt32 GetGUIDLow()              { return GetUInt32Value(0); }
        public OBJECT_TYPE Get_Type()            { return _type; }
        public OBJECT_TYPE_ID Get_TypeId()       { return _typeId; }
        public UInt32 GetGUIDHigh()             { return GetUInt32Value(1); }
        public UInt32 GetEntry()                { return GetUInt32Value((int)UpdateFields.OBJECT_FIELD_ENTRY); }
        public bool _IsDepleted()               { return _depleted; }
        public void _SetDepleted()              { _depleted = true; }

        public new OBJECT_TYPE_MASK GetType()   { return (OBJECT_TYPE_MASK)GetUInt32Value(2);}
        public bool IsPlayer()                  { return _typeId == OBJECT_TYPE_ID.TYPEID_PLAYER; }             // specific
        public bool IsUnit()                    { return _type.HasFlag(OBJECT_TYPE.TYPE_UNIT); }                // generic (unit = creature or player)
        public bool IsCreature()                { return IsUnit() && !IsPlayer(); }                             // specific
        public bool IsItem()                    { return _type.HasFlag(OBJECT_TYPE.TYPE_ITEM); }                // generic (item or container)
        public bool IsContainer()               { return _typeId == OBJECT_TYPE_ID.TYPEID_CONTAINER; }          // specific
        public bool IsCorpse()                  { return _typeId == OBJECT_TYPE_ID.TYPEID_CORPSE; }             // specific
        public bool IsDynObject()               { return _typeId == OBJECT_TYPE_ID.TYPEID_DYNAMICOBJECT; }      // specific
        public bool IsGameObject()              { return _typeId == OBJECT_TYPE_ID.TYPEID_GAMEOBJECT; }         // specific
        //public bool IsWorldObject()             { return _type.HasFlag(OBJECT_TYPE.TYPE_PLAYER | OBJECT_TYPE.TYPE_UNIT | OBJECT_TYPE.TYPE_CORPSE | OBJECT_TYPE.TYPE_DYNAMICOBJECT | OBJECT_TYPE.TYPE_GAMEOBJECT); }
        public bool IsWorldObject() { return IsPlayer() && IsUnit() && IsCorpse() && IsDynObject() && IsGameObject(); }


        public void ReadMov(BinaryReader gr)
        {
            Object obj = this;

            Unit u = new Unit();
            if (obj)
            {
                if (obj.IsUnit() || obj.IsPlayer())
                {
                    u = (Unit)obj;

                }
                //else return null;
            }

            UpdateFlags = (UpdateFlags)gr.ReadUInt16();

            if (UpdateFlags.HasFlag(UpdateFlags.UPDATEFLAG_LIVING))
            {
                Flags = (MovementFlags)gr.ReadUInt32();
                Flags2 = (MovementFlags2)gr.ReadUInt16();
                TimeStamp = gr.ReadUInt32();

                Position = gr.ReadCoords3();
                Facing = gr.ReadSingle()*Mathf.Rad2Deg;

                if (obj.IsUnit() || obj.IsPlayer() || obj)
                    ((WorldObject)obj).SetCoordinates(Position);

                if (Flags.HasFlag(MovementFlags.OnTransport))
                {
                    Transport = TransportInfo.Read(gr, Flags2);
                }

                if (Flags.HasFlag(MovementFlags.Swimming) || Flags.HasFlag(MovementFlags.Flying) ||
                    Flags2.HasFlag(MovementFlags2.AlwaysAllowPitching))
                {
                    Pitch = gr.ReadSingle();
                }

                FallTime = gr.ReadUInt32();

                if (Flags.HasFlag(MovementFlags.Falling))
                {
                    FallVelocity = gr.ReadSingle();
                    FallCosAngle = gr.ReadSingle();
                    FallSinAngle = gr.ReadSingle();
                    FallSpeed = gr.ReadSingle();
                }

                if (Flags.HasFlag(MovementFlags.SplineElevation))
                {
                    SplineElevation = gr.ReadSingle();
                }


                /*  for (byte i = 0; i < movement.speeds.Length; ++i)
                      movement.speeds[i] = gr.ReadSingle();*/

                speedWalk = gr.ReadSingle();
                speedRun = gr.ReadSingle();
                speedSwimBack = gr.ReadSingle();
                speedSwim = gr.ReadSingle();
                speedWalkBack = gr.ReadSingle();
                speedFly = gr.ReadSingle();
                speedFlyBack = gr.ReadSingle();
                speedTurn = gr.ReadSingle();
                speedPitchRate = gr.ReadSingle();


                u.SetCoordinates(Position);
                u.SetSpeed(UnitMoveType.MOVE_WALK, speedWalk);
                u.SetSpeed(UnitMoveType.MOVE_RUN, speedRun);
                u.SetSpeed(UnitMoveType.MOVE_SWIMBACK, speedSwimBack);
                u.SetSpeed(UnitMoveType.MOVE_SWIM, speedSwim);
                u.SetSpeed(UnitMoveType.MOVE_WALKBACK, speedWalkBack);
                u.SetSpeed(UnitMoveType.MOVE_TURN, speedTurn);
                u.SetSpeed(UnitMoveType.MOVE_FLY, speedFly);
                u.SetSpeed(UnitMoveType.MOVE_FLYBACK, speedFlyBack);
                u.SetSpeed(UnitMoveType.MOVE_PITCH_RATE, speedPitchRate);


                if (Flags.HasFlag(MovementFlags.SplineEnabled))
                {
                    Spline = SplineInfo.Read(gr);
                }
            }
            else
            {
                if (UpdateFlags.HasFlag(UpdateFlags.UPDATEFLAG_GO_POSITION))
                {
                    Transport = new TransportInfo();
                    Transport.Guid = gr.ReadPackedGuid();
                    Position = gr.ReadCoords3();
                    Transport.Position = gr.ReadCoords3();
                    Facing = gr.ReadSingle() * Mathf.Rad2Deg;
                    Transport.Facing = gr.ReadSingle() * Mathf.Rad2Deg;
                    if (obj && obj.IsGameObject())
                        ((GO)obj).SetCoordinates(Position);
                }
                else if (UpdateFlags.HasFlag(UpdateFlags.UPDATEFLAG_HAS_POSITION))
                {
                    Position = gr.ReadCoords3();
                    Facing = gr.ReadSingle() * Mathf.Rad2Deg;
                    if (obj && obj.IsWorldObject())
                        ((WorldObject)obj).SetCoordinates(Position);
                }
            }

            if (UpdateFlags.HasFlag(UpdateFlags.UPDATEFLAG_LOWGUID))
            {
                LowGuid = gr.ReadUInt32();
            }

            if (UpdateFlags.HasFlag(UpdateFlags.UPDATEFLAG_HIGHGUID))
            {
                HighGuid = gr.ReadUInt32();
            }

            if (UpdateFlags.HasFlag(UpdateFlags.UPDATEFLAG_TARGET_GUID))
            {
                AttackingTarget = gr.ReadPackedGuid();
            }

            if (UpdateFlags.HasFlag(UpdateFlags.UPDATEFLAG_TRANSPORT))
            {
                TransportTime = gr.ReadUInt32();
            }

            // WotLK
            if (UpdateFlags.HasFlag(UpdateFlags.UPDATEFLAG_VEHICLE))
            {
                VehicleId = gr.ReadUInt32();
                VehicleAimAdjustement = gr.ReadSingle();
            }

            // 3.1
            if (UpdateFlags.HasFlag(UpdateFlags.UPDATEFLAG_GO_ROTATION))
            {
                GoRotationULong = gr.ReadUInt64(); // fixme
            }
        }




        #endregion

        #region Name methods
        public void SetName(string name) { _name = name; }
        public string GetName() { return _name; }
        #endregion

        #region Public methods

        public Object()
        {
            
            _depleted = false;
            _type = OBJECT_TYPE.TYPE_OBJECT;
            _typeId = OBJECT_TYPE_ID.TYPEID_OBJECT;
            Data = new Dictionary<int, uint>();
            
        }

        public void Create(GameGuid _guid)
        {
            
            GUID = _guid;
            SetNameToObj(_guid.GetOldGuid().ToString());
            SetUInt64Value((ushort)UpdateFields.OBJECT_FIELD_GUID, _guid.GetOldGuid());
            SetUInt32Value((ushort)UpdateFields.OBJECT_FIELD_TYPE, (UInt32)_type);
         
        }

        public void SetNameToObj(String _str)
        {
            this.transform.name =_str;
        
        }
        public uint GetUInt32Value(int index)
        {
            uint result;
            Data.TryGetValue(index, out result);
            return result;
        }

        public void SetUInt32Value( UInt16 index, UInt32 value )
        {
            uint _b;
            Data.TryGetValue(index, out _b);
            if (_b > 0) Data[index] = (UInt32)value;
            else Data.Add(index, (UInt32)value);
        }
        public void SetUInt64Value( UInt16 index, UInt64 value)
        {
            uint _b;
            Data.TryGetValue(index,out _b);
            if (_b > 0) Data[index] = (UInt32)value;
            else Data.Add(index, (UInt32)value);
        }



        public void SetMoveFlag(MovementFlags flag)
        {
            Flags |= flag;
        }
        public void UnSetMoveFlag(MovementFlags flag)
        {
            Flags &= ~flag;
        }
        public bool IsMoveFlagSet(MovementFlags flag)
        {
            return Flags.HasFlag(flag) ? true : false;
        }
        #endregion


    }
        
}
