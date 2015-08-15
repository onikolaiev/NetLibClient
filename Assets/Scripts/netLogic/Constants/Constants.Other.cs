using netLogic.Shared;
using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using UnityEngine;

namespace netLogic.Constants
{


    /// <summary>
    /// MMO Object Class
    /// </summary>
/*    public class MMOObject : MMOObjectUpdate 
    {
        private readonly List<MMOObjectUpdate> updates = new List<MMOObjectUpdate>();

        public string Name;
        public string SubName;
        public GameGuid GUID;
        public OBJECT_TYPE Type;
        public UInt32 EntryId;
        public UInt32 SubType;
        public UInt32 Rank;
        public UInt32 Family;
        public UInt32 Race;
        public UInt32 Gender;
        public UInt32 Level;
        public UInt32[] Fields;
        
        

        public IEnumerable<MMOObjectUpdate> Updates { get { return updates; } }
        public OBJECT_TYPE_ID TypeId { get; private set; }
        public MovementInfo Movement { get; private set; }

        public MMOObject(GameGuid guid)
        {
            this.GUID = guid;
        }

        public void SetCoordinates(Coordinate l)
        {
            Movement.Position = l;
        }

        public Coordinate GetCoordinates()
        {
            return Movement.Position;
        }

        public float GetPositionX()
        {
            return Movement.Position.X;
        }

        public void SetPositionX(float x)
        {
            Movement.Position.X = x;
        }

        public float GetPositionY()
        {
            return Movement.Position.Y;
        }

        public void SetPositionY(float y)
        {
            Movement.Position.Y = y;
        }

        public float GetPositionZ()
        {
            return Movement.Position.Z;
        }

        public void SetPositionZ(float z)
        {
            Movement.Position.Z = z;
        }

        public float GetOrientation()
        {
            return Movement.Position.O;
        }

        public void SetOrientation(float o)
        {
            Movement.Position.O = o;
        }

        public float CalculateAngle(float x1, float y1)
        {
            return CalculateAngle(this.Movement.Position.X, this.Movement.Position.Y, x1, y1);
        }
        // Credit to ascent - I'm lazy :P
        public float CalculateAngle(float x1, float y1, float x2, float y2)
        {
            float dx = x2 - x1;
            float dy = y2 - y1;
            double angle = 0.0f;

            // Calculate angle
            if (dx == 0.0)
            {
                if (dy == 0.0)
                    angle = 0.0;
                else if (dy > 0.0)
                    angle = Math.PI * 0.5;
                else
                    angle = Math.PI * 3.0 * 0.5;
            }
            else if (dy == 0.0)
            {
                if (dx > 0.0)
                    angle = 0.0;
                else
                    angle = Math.PI;
            }
            else
            {
                if (dx < 0.0)
                    angle = Math.Atan(dy / dx) + Math.PI;
                else if (dy < 0.0)
                    angle = Math.Atan(dy / dx) + (2 * Math.PI);
                else
                    angle = Math.Atan(dy / dx);
            }

            return (float)angle;
        }

        public bool IsDead
        {
            get { return false; }// fixme
        }

        public int Reaction
        {
            get { return 0; } //fixme
        }

        public bool IsValid
        {
            get { return true; } // fixme
        }

        public Coordinate Location
        {
            get { return Movement.Position; }
        }

        public void SetField(int x, UInt32 value)
        {
            Fields[x] = value;
        }

        public MMOObject()
        {
          //  gameObject.AddComponent<MMOObject>();
        }

        public MMOObject(OBJECT_TYPE_ID typeId, MovementInfo movement, IDictionary<int, uint> data, GameGuid guid) : base(data)
        {
           
            GUID = guid;
            TypeId = typeId;
            Movement = movement;
        }

        public MMOObject(OBJECT_TYPE_ID typeId, MovementInfo movement, IDictionary<int, uint> data) : base(data)
        {
            TypeId = typeId;
            Movement = movement;
        }

        public void Init(OBJECT_TYPE_ID typeId, MovementInfo movement, IDictionary<int, uint> data)
        {
            
            base.Data = data;
            TypeId = typeId;
            Movement = movement;
            SetNewMovement(movement);


        }

        public void SetNewMovement(MovementInfo movement)
        {
            Loom.DispatchToMainThread(() =>
            {
                base.transform.position = new Vector3(movement.Position.X, movement.Position.Z, movement.Position.Y);
                //  GameObject temp = GameObject.CreatePrimitive(PrimitiveType.Cylinder);
                //  temp.transform.parent = o.transform;

            });


            Movement = movement;
        }

        public void AddUpdate(MMOObjectUpdate update)
        {
            updates.Add(update);
        }

        public uint GetGUIDHigh()
        {
            return GetUInt32Value(1); // OBJECT_FIELD_GUID (high)
        }

        public new OBJECT_TYPE_MASK GetType()
        {
            return (OBJECT_TYPE_MASK)GetUInt32Value(2); // OBJECT_FIELD_TYPE
        }

        private uint GetUInt32Value(int index)
        {
            uint result;
            Data.TryGetValue(index, out result);
            return result;
        }


    }*/


    public class MMOObjectUpdate
    {
        public IDictionary<int, uint> Data { get; set; }

        protected MMOObjectUpdate(IDictionary<int, uint> data)
        {
            Data = data;
        }

        protected MMOObjectUpdate()
        {
            
        }

        public static MMOObjectUpdate Read(BinaryReader gr)
        {
            byte blocksCount = gr.ReadByte();
            var updatemask = new int[blocksCount];

            for (int i = 0; i < updatemask.Length; ++i)
                updatemask[i] = gr.ReadInt32();

            var mask = new BitArray(updatemask);

            var values = new Dictionary<int, uint>();

            for (int i = 0; i < mask.Count; ++i)
                if (mask[i])
                    values[i] = gr.ReadUInt32();

            return new MMOObjectUpdate(values);
        }
    }

    public struct WoWVersion
    {
        public WoWVersion(byte a, byte b, byte c, ushort d)
        {
            major = a; minor = b; update = c; build = d;
        }

        public WoWVersion(String versionString)
        {
            String[] versionParts = versionString.Split(new char[] { '.' });
            Byte.TryParse(versionParts[0], out major);
            Byte.TryParse(versionParts[1], out minor);
            Byte.TryParse(versionParts[2], out update);
            UInt16.TryParse(versionParts[3], out build);
        }

        public byte major;
        public byte minor;
        public byte update;
        public UInt16 build;
    }

    public enum ServiceType
    {
        None = 0,
        Logon = 1,
        World = 2,
        Count
    }








}
