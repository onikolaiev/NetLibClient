﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.IO;
using UnityEngine;



namespace netLogic.Constants
{
    public static class EnumExt
    {
        /// <summary>
        /// Check to see if a flags enumeration has a specific flag set.
        /// </summary>
        /// <param name="variable">Flags enumeration to check</param>
        /// <param name="value">Flag to check for</param>
        /// <returns></returns>
        public static bool HasFlag(this Enum variable, Enum value)
        {
            if (variable == null)
                return false;

            if (value == null)
                throw new ArgumentNullException("value");

            // Not as good as the .NET 4 version of this function, but should be good enough
            if (!Enum.IsDefined(variable.GetType(), value))
            {
                throw new ArgumentException(string.Format(
                    "Enumeration type mismatch.  The flag is of type '{0}', was expecting '{1}'.",
                    value.GetType(), variable.GetType()));
            }

            ulong num = Convert.ToUInt64(value);
            return ((Convert.ToUInt64(variable) & num) == num);

        }

    }
    #region UpdateTypes
    /// <summary>
    /// WoW Update Types.
    /// </summary>
    public enum UpdateTypes
    {
        /// <summary>
        /// Update type that update only object field values.
        /// </summary>
        UPDATETYPE_VALUES = 0,
        /// <summary>
        /// Update type that update only object movement.
        /// </summary>
        UPDATETYPE_MOVEMENT = 1,
        /// <summary>
        /// Update type that create an object (full update).
        /// </summary>
        UPDATETYPE_CREATE_OBJECT = 2,
        /// <summary>
        /// Update type that create an object (gull update, self use).
        /// </summary>
        UPDATETYPE_CREATE_OBJECT2 = 3,
        /// <summary>
        /// Update type that update only objects out of range.
        /// </summary>
        UPDATETYPE_OUT_OF_RANGE_OBJECTS = 4,
        /// <summary>
        /// Update type that update only near objects.
        /// </summary>
        UPDATETYPE_NEAR_OBJECTS = 5,
    }
    #endregion

    #region UpdateFlags
    /// <summary>
    /// WoW Update Flags
    /// </summary>
    [Flags]
    public enum UpdateFlags : ushort
    {
        /// <summary>
        /// No flag set.
        /// </summary>
        UPDATEFLAG_NONE = 0x00,
        /// <summary>
        /// Update flag for self.
        /// </summary>
        UPDATEFLAG_SELFTARGET = 0x01,
        /// <summary>
        /// Update flag for transport object.
        /// </summary>
        UPDATEFLAG_TRANSPORT = 0x02,
        /// <summary>
        /// Update flag with target guid.
        /// </summary>
        UPDATEFLAG_TARGET_GUID = 0x04,
        /// <summary>
        /// Update flag unknown...
        /// </summary>
        UPDATEFLAG_LOWGUID = 0x08,
        /// <summary>
        /// Common update flag.
        /// </summary>
        UPDATEFLAG_HIGHGUID = 0x10,
        /// <summary>
        /// Update flag for living objects.
        /// </summary>
        UPDATEFLAG_LIVING = 0x20,
        /// <summary>
        /// Update flag for world objects (players, units, go, do, corpses).
        /// </summary>
        UPDATEFLAG_HAS_POSITION = 0x40,
        /// <summary>
        /// Unknown, added in WotLK Beta
        /// </summary>
        UPDATEFLAG_VEHICLE = 0x80,
        /// <summary>
        /// Unknown, added in 3.1
        /// </summary>
        UPDATEFLAG_GO_POSITION = 0x100,
        /// <summary>
        /// Unknown, added in 3.1
        /// </summary>
        UPDATEFLAG_GO_ROTATION = 0x200,
        /// <summary>
        /// Unknown, added in 3.1+
        /// </summary>
        UPDATEFLAG_UNK1 = 0x400,
    }
    #endregion


    #region Updatefield struct
    public struct UpdateField
    {
        public int Identifier;
        public string Name;
        public uint Type;
        public uint Format;
        public uint Value;

        public UpdateField(int id, string name, uint type, uint format, uint value)
        {
            Identifier = id;
            Name = name;
            Type = type;
            Format = format;
            Value = value;
        }
    }
    #endregion

    enum FieldType
    {
        FIELD_TYPE_NONE = 0,
        FIELD_TYPE_END = 1,
        FIELD_TYPE_ITEM = 2,
        FIELD_TYPE_UNIT = 3,
        FIELD_TYPE_GO = 4,
        FIELD_TYPE_DO = 5,
        FIELD_TYPE_CORPSE = 6
    };

    public class UpdateFieldsLoader
    {
        /// <summary>
        /// Object update fields end.
        /// </summary>
        public static uint OBJECT_END;
        /// <summary>
        /// Item update fields end.
        /// </summary>
        public static uint ITEM_END;
        /// <summary>
        /// Container update fields end.
        /// </summary>
        public static uint CONTAINER_END;
        /// <summary>
        /// Unit update fields end.
        /// </summary>
        public static uint UNIT_END;
        /// <summary>
        /// Player update fields end.
        /// </summary>
        public static uint PLAYER_END;
        /// <summary>
        /// Game object update fields end.
        /// </summary>
        public static uint GO_END;
        /// <summary>
        /// Dynamic object fields end.
        /// </summary>
        public static uint DO_END;
        /// <summary>
        /// Corpse fields end.
        /// </summary>
        public static uint CORPSE_END;

        public static Dictionary<int, UpdateField> item_uf = new Dictionary<int, UpdateField>(); // item + container
        public static Dictionary<int, UpdateField> unit_uf = new Dictionary<int, UpdateField>(); // unit + player
        public static Dictionary<int, UpdateField> go_uf = new Dictionary<int, UpdateField>();
        public static Dictionary<int, UpdateField> do_uf = new Dictionary<int, UpdateField>();
        public static Dictionary<int, UpdateField> corpse_uf = new Dictionary<int, UpdateField>();

        public static UpdateField GetUpdateField(OBJECT_TYPE_ID type, UpdateFields index)
        {
            int indx = (int)index;
            // index parameter must be checked first
            switch (type)
            {
                case OBJECT_TYPE_ID.TYPEID_ITEM:
                case OBJECT_TYPE_ID.TYPEID_CONTAINER:
                    return item_uf[indx];
                case OBJECT_TYPE_ID.TYPEID_UNIT:
                case OBJECT_TYPE_ID.TYPEID_PLAYER:
                    return unit_uf[indx];
                case OBJECT_TYPE_ID.TYPEID_GAMEOBJECT:
                    return go_uf[indx];
                case OBJECT_TYPE_ID.TYPEID_DYNAMICOBJECT:
                    return do_uf[indx];
                case OBJECT_TYPE_ID.TYPEID_CORPSE:
                    return corpse_uf[indx];
                default:
                    return unit_uf[indx];
            }
        }

        public static UpdateField GetUpdateField(OBJECT_TYPE_ID type, string _name)
        {
            int val = 0;
            // index parameter must be checked first
            switch (type)
            {
                case OBJECT_TYPE_ID.TYPEID_ITEM:
                    foreach (var name in item_uf)
                    {
                        if (name.Value.Name == _name)
                        {
                            val = name.Key;
                        }

                    }
                    return item_uf[val];
                case OBJECT_TYPE_ID.TYPEID_CONTAINER:
                    foreach (var name in item_uf)
                    {
                        if (name.Value.Name == _name)
                        {
                            val = name.Key;
                        }

                    }
                    return item_uf[val];
                case OBJECT_TYPE_ID.TYPEID_UNIT:
                    foreach (var name in unit_uf)
                    {
                        if (name.Value.Name == _name)
                        {
                            val = name.Key;
                        }

                    }
                    return unit_uf[val];
                case OBJECT_TYPE_ID.TYPEID_PLAYER:
                    foreach (var name in unit_uf)
                    {
                        if (name.Value.Name == _name)
                        {
                            val = name.Key;
                        }

                    }
                    return unit_uf[val];
                case OBJECT_TYPE_ID.TYPEID_GAMEOBJECT:
                    foreach (var name in go_uf)
                    {
                        if (name.Value.Name == _name)
                        {
                            val = name.Key;
                        }

                    }
                   
                    return go_uf[val];
                case OBJECT_TYPE_ID.TYPEID_DYNAMICOBJECT:
                    foreach (var name in do_uf)
                    {
                        if (name.Value.Name == _name)
                        {
                            val = name.Key;
                        }

                    }
                    
                    return do_uf[val];
                case OBJECT_TYPE_ID.TYPEID_CORPSE:
                    foreach (var name in corpse_uf)
                    {
                        if (name.Value.Name == _name)
                        {
                            val = name.Key;
                        }

                    }
                    
                    return corpse_uf[val];
                default:
                    return unit_uf[val];
            }
        }

        public static void LoadUpdateFields(uint build)
        {
            ClearUpdateFields();
            byte[] file;
            try
            {
                //var file = String.Format(System.Windows.Forms.Application.StartupPath + "\\" + "updatefields\\{0}.dat", build);
                file = File.ReadAllBytes(Application.dataPath + "\\Scripts\\netLogic\\Resources\\12340.dat");
            }
            catch
            { 
                file = File.ReadAllBytes(Application.dataPath + "\\Resources\\12340.dat");
            }

             
            
            
            var type = FieldType.FIELD_TYPE_NONE;
            var sr = new System.IO.StreamReader(new MemoryStream(file));
            while (sr.Peek() >= 0)
            {
                if (type == FieldType.FIELD_TYPE_END)
                {
                    OBJECT_END = Convert.ToUInt32(sr.ReadLine());
                    ITEM_END = Convert.ToUInt32(sr.ReadLine());
                    CONTAINER_END = Convert.ToUInt32(sr.ReadLine());
                    UNIT_END = Convert.ToUInt32(sr.ReadLine());
                    PLAYER_END = Convert.ToUInt32(sr.ReadLine());
                    GO_END = Convert.ToUInt32(sr.ReadLine());
                    DO_END = Convert.ToUInt32(sr.ReadLine());
                    CORPSE_END = Convert.ToUInt32(sr.ReadLine());
                    type = FieldType.FIELD_TYPE_NONE;
                    continue;
                }

                var curline = sr.ReadLine();

                if (curline.StartsWith("#") || curline.StartsWith("/")) // skip commentary lines
                    continue;

                if (curline.Length == 0)    // empty line
                    continue;

                if (curline.StartsWith(":"))    // label lines
                {
                    if (curline.Contains("ends"))
                        type = FieldType.FIELD_TYPE_END;
                    if (curline.Contains("item"))
                        type = FieldType.FIELD_TYPE_ITEM;
                    if (curline.Contains("unit+player"))
                        type = FieldType.FIELD_TYPE_UNIT;
                    else if (curline.Contains("gameobject"))
                        type = FieldType.FIELD_TYPE_GO;
                    else if (curline.Contains("dynamicobject"))
                        type = FieldType.FIELD_TYPE_DO;
                    else if (curline.Contains("corpse"))
                        type = FieldType.FIELD_TYPE_CORPSE;

                    continue;
                }

                var arr = curline.Split('	');

                if (arr.Length < 3)
                    continue;

                var id = Convert.ToInt32(arr[0]);
                var name = arr[1];
                var type1 = Convert.ToUInt32(arr[2]);
                //uint format = Convert.ToUInt32(arr[3]);
                const uint format = 0;

                var uf = new UpdateField(id, name, type1, format, 0);
                switch (type)
                {
                    case FieldType.FIELD_TYPE_END:
                        break;
                    case FieldType.FIELD_TYPE_ITEM:
                        item_uf.Add(id, uf);
                        break;
                    case FieldType.FIELD_TYPE_UNIT:
                        unit_uf.Add(id, uf);
                        break;
                    case FieldType.FIELD_TYPE_GO:
                        go_uf.Add(id, uf);
                        break;
                    case FieldType.FIELD_TYPE_DO:
                        do_uf.Add(id, uf);
                        break;
                    case FieldType.FIELD_TYPE_CORPSE:
                        corpse_uf.Add(id, uf);
                        break;
                }
            }

            CheckIntegrity();
        }

        public static void ClearUpdateFields()
        {
            item_uf.Clear();
            unit_uf.Clear();
            go_uf.Clear();
            do_uf.Clear();
            corpse_uf.Clear();
        }

        /// <summary>
        /// Check updatefields.dat integrity
        /// </summary>
        public static void CheckIntegrity()
        {
            // program will crash there if updatefields.dat contains errors
            for (var i = 0; i < item_uf.Count; i++)
            {
                var uf = item_uf[i];
            }

            for (var i = 0; i < unit_uf.Count; i++)
            {
                var uf = unit_uf[i];
            }

            for (var i = 0; i < go_uf.Count; i++)
            {
                var uf = go_uf[i];
            }

            for (var i = 0; i < do_uf.Count; i++)
            {
                var uf = do_uf[i];
            }

            for (var i = 0; i < corpse_uf.Count; i++)
            {
                var uf = corpse_uf[i];
            }
            
        }
        
       
    }
    
    public enum UpdateFields
    {
        OBJECT_FIELD_GUID = 0x000,	//  Size: 2, Type: GUID, Flags: 1
        OBJECT_FIELD_GUID_01 = 0x001,	//  Size: 2, Type: GUID, Flags: 1
        OBJECT_FIELD_TYPE = 0x002,    //  Size: 1, Type: Int32, Flags: 1
        OBJECT_FIELD_ENTRY = 0x003,	//  Size: 1, Type: Int32, Flags: 1
        OBJECT_FIELD_SCALE_X = 0x004,	//  Size: 1, Type: Float, Flags: 1
        OBJECT_FIELD_PADDING = 0x005,	//  Size: 1, Type: Int32, Flags: 0
        OBJECT_END = 0x006,

        //CONTAINER:
        CONTAINER_FIELD_NUM_SLOTS = ITEM_END + 0x000,	// Size: 1, Type: Int32, Flags: 1
        CONTAINER_ALIGN_PAD = ITEM_END + 0x001,	// Size: 1, Type: Bytes, Flags: 0
        CONTAINER_FIELD_SLOT_1 = ITEM_END + 0x002,	// Size: 72, Type: GUID, Flags: 1
        CONTAINER_FIELD_SLOT_01 = ITEM_END + 0x003,	// Size: 72, Type: GUID, Flags: 1
        CONTAINER_FIELD_SLOT_02 = ITEM_END + 0x004,	// Size: 72, Type: GUID, Flags: 1
        CONTAINER_FIELD_SLOT_03 = ITEM_END + 0x005,	// Size: 72, Type: GUID, Flags: 1
        CONTAINER_FIELD_SLOT_04 = ITEM_END + 0x006,	// Size: 72, Type: GUID, Flags: 1
        CONTAINER_FIELD_SLOT_05 = ITEM_END + 0x007,	// Size: 72, Type: GUID, Flags: 1
        CONTAINER_FIELD_SLOT_06 = ITEM_END + 0x008,	// Size: 72, Type: GUID, Flags: 1
        CONTAINER_FIELD_SLOT_07 = ITEM_END + 0x009,	// Size: 72, Type: GUID, Flags: 1
        CONTAINER_FIELD_SLOT_08 = ITEM_END + 0x00A,	// Size: 72, Type: GUID, Flags: 1
        CONTAINER_FIELD_SLOT_09 = ITEM_END + 0x00B,	// Size: 72, Type: GUID, Flags: 1
        CONTAINER_FIELD_SLOT_10 = ITEM_END + 0x00C,	// Size: 72, Type: GUID, Flags: 1
        CONTAINER_FIELD_SLOT_11 = ITEM_END + 0x00D,	// Size: 72, Type: GUID, Flags: 1
        CONTAINER_FIELD_SLOT_12 = ITEM_END + 0x00E,	// Size: 72, Type: GUID, Flags: 1
        CONTAINER_FIELD_SLOT_13 = ITEM_END + 0x00F,	// Size: 72, Type: GUID, Flags: 1
        CONTAINER_FIELD_SLOT_14 = ITEM_END + 0x010,	// Size: 72, Type: GUID, Flags: 1
        CONTAINER_FIELD_SLOT_15 = ITEM_END + 0x011,	// Size: 72, Type: GUID, Flags: 1
        CONTAINER_FIELD_SLOT_16 = ITEM_END + 0x012,	// Size: 72, Type: GUID, Flags: 1
        CONTAINER_FIELD_SLOT_17 = ITEM_END + 0x013,	// Size: 72, Type: GUID, Flags: 1
        CONTAINER_FIELD_SLOT_18 = ITEM_END + 0x014,	// Size: 72, Type: GUID, Flags: 1
        CONTAINER_FIELD_SLOT_19 = ITEM_END + 0x015,	// Size: 72, Type: GUID, Flags: 1
        CONTAINER_FIELD_SLOT_20 = ITEM_END + 0x016,	// Size: 72, Type: GUID, Flags: 1
        CONTAINER_FIELD_SLOT_21 = ITEM_END + 0x017,	// Size: 72, Type: GUID, Flags: 1
        CONTAINER_FIELD_SLOT_22 = ITEM_END + 0x018,	// Size: 72, Type: GUID, Flags: 1
        CONTAINER_FIELD_SLOT_23 = ITEM_END + 0x019,	// Size: 72, Type: GUID, Flags: 1
        CONTAINER_FIELD_SLOT_24 = ITEM_END + 0x01A,	// Size: 72, Type: GUID, Flags: 1
        CONTAINER_FIELD_SLOT_25 = ITEM_END + 0x01B,	// Size: 72, Type: GUID, Flags: 1
        CONTAINER_FIELD_SLOT_26 = ITEM_END + 0x01C,	// Size: 72, Type: GUID, Flags: 1
        CONTAINER_FIELD_SLOT_27 = ITEM_END + 0x01D,	// Size: 72, Type: GUID, Flags: 1
        CONTAINER_FIELD_SLOT_28 = ITEM_END + 0x01E,	// Size: 72, Type: GUID, Flags: 1
        CONTAINER_FIELD_SLOT_29 = ITEM_END + 0x01F,	// Size: 72, Type: GUID, Flags: 1
        CONTAINER_FIELD_SLOT_30 = ITEM_END + 0x020,	// Size: 72, Type: GUID, Flags: 1
        CONTAINER_FIELD_SLOT_31 = ITEM_END + 0x021,	// Size: 72, Type: GUID, Flags: 1
        CONTAINER_FIELD_SLOT_32 = ITEM_END + 0x022,	// Size: 72, Type: GUID, Flags: 1
        CONTAINER_FIELD_SLOT_33 = ITEM_END + 0x023,	// Size: 72, Type: GUID, Flags: 1
        CONTAINER_FIELD_SLOT_34 = ITEM_END + 0x024,	// Size: 72, Type: GUID, Flags: 1
        CONTAINER_FIELD_SLOT_35 = ITEM_END + 0x025,	// Size: 72, Type: GUID, Flags: 1
        CONTAINER_FIELD_SLOT_36 = ITEM_END + 0x026,	// Size: 72, Type: GUID, Flags: 1
        CONTAINER_FIELD_SLOT_37 = ITEM_END + 0x027,	// Size: 72, Type: GUID, Flags: 1
        CONTAINER_FIELD_SLOT_38 = ITEM_END + 0x028,	// Size: 72, Type: GUID, Flags: 1
        CONTAINER_FIELD_SLOT_39 = ITEM_END + 0x029,	// Size: 72, Type: GUID, Flags: 1
        CONTAINER_FIELD_SLOT_40 = ITEM_END + 0x02A,	// Size: 72, Type: GUID, Flags: 1
        CONTAINER_FIELD_SLOT_41 = ITEM_END + 0x02B,	// Size: 72, Type: GUID, Flags: 1
        CONTAINER_FIELD_SLOT_42 = ITEM_END + 0x02C,	// Size: 72, Type: GUID, Flags: 1
        CONTAINER_FIELD_SLOT_43 = ITEM_END + 0x02D,	// Size: 72, Type: GUID, Flags: 1
        CONTAINER_FIELD_SLOT_44 = ITEM_END + 0x02E,	// Size: 72, Type: GUID, Flags: 1
        CONTAINER_FIELD_SLOT_45 = ITEM_END + 0x02F,	// Size: 72, Type: GUID, Flags: 1
        CONTAINER_FIELD_SLOT_46 = ITEM_END + 0x030,	// Size: 72, Type: GUID, Flags: 1
        CONTAINER_FIELD_SLOT_47 = ITEM_END + 0x031,	// Size: 72, Type: GUID, Flags: 1
        CONTAINER_FIELD_SLOT_48 = ITEM_END + 0x032,	// Size: 72, Type: GUID, Flags: 1
        CONTAINER_FIELD_SLOT_49 = ITEM_END + 0x033,	// Size: 72, Type: GUID, Flags: 1
        CONTAINER_FIELD_SLOT_50 = ITEM_END + 0x034,	// Size: 72, Type: GUID, Flags: 1
        CONTAINER_FIELD_SLOT_51 = ITEM_END + 0x035,	// Size: 72, Type: GUID, Flags: 1
        CONTAINER_FIELD_SLOT_52 = ITEM_END + 0x036,	// Size: 72, Type: GUID, Flags: 1
        CONTAINER_FIELD_SLOT_53 = ITEM_END + 0x037,	// Size: 72, Type: GUID, Flags: 1
        CONTAINER_FIELD_SLOT_54 = ITEM_END + 0x038,	// Size: 72, Type: GUID, Flags: 1
        CONTAINER_FIELD_SLOT_55 = ITEM_END + 0x039,	// Size: 72, Type: GUID, Flags: 1
        CONTAINER_FIELD_SLOT_56 = ITEM_END + 0x03A,	// Size: 72, Type: GUID, Flags: 1
        CONTAINER_FIELD_SLOT_57 = ITEM_END + 0x03B,	// Size: 72, Type: GUID, Flags: 1
        CONTAINER_FIELD_SLOT_58 = ITEM_END + 0x03C,	// Size: 72, Type: GUID, Flags: 1
        CONTAINER_FIELD_SLOT_59 = ITEM_END + 0x03D,	// Size: 72, Type: GUID, Flags: 1
        CONTAINER_FIELD_SLOT_60 = ITEM_END + 0x03E,	// Size: 72, Type: GUID, Flags: 1
        CONTAINER_FIELD_SLOT_61 = ITEM_END + 0x03F,	// Size: 72, Type: GUID, Flags: 1
        CONTAINER_FIELD_SLOT_62 = ITEM_END + 0x040,	// Size: 72, Type: GUID, Flags: 1
        CONTAINER_FIELD_SLOT_63 = ITEM_END + 0x041,	// Size: 72, Type: GUID, Flags: 1
        CONTAINER_FIELD_SLOT_64 = ITEM_END + 0x042,	// Size: 72, Type: GUID, Flags: 1
        CONTAINER_FIELD_SLOT_65 = ITEM_END + 0x043,	// Size: 72, Type: GUID, Flags: 1
        CONTAINER_FIELD_SLOT_66 = ITEM_END + 0x044,	// Size: 72, Type: GUID, Flags: 1
        CONTAINER_FIELD_SLOT_67 = ITEM_END + 0x045,	// Size: 72, Type: GUID, Flags: 1
        CONTAINER_FIELD_SLOT_68 = ITEM_END + 0x046,	// Size: 72, Type: GUID, Flags: 1
        CONTAINER_FIELD_SLOT_69 = ITEM_END + 0x047,	// Size: 72, Type: GUID, Flags: 1
        CONTAINER_FIELD_SLOT_70 = ITEM_END + 0x048,	// Size: 72, Type: GUID, Flags: 1
        CONTAINER_FIELD_SLOT_71 = ITEM_END + 0x049,	// Size: 72, Type: GUID, Flags: 1
        CONTAINER_END = ITEM_END + 0x04A,	// Size: 1, Type: Int32, Flags: 0

        //ITEM:
        ITEM_FIELD_OWNER = OBJECT_END + 0x000,	//  Size: 2, Type: GUID, Flags: 1
        ITEM_FIELD_CONTAINED = OBJECT_END + 0x002,	//  Size: 2, Type: GUID, Flags: 1
        ITEM_FIELD_CREATOR = OBJECT_END + 0x004,	//  Size: 2, Type: GUID, Flags: 1
        ITEM_FIELD_GIFTCREATOR = OBJECT_END + 0x006,	//  Size: 2, Type: GUID, Flags: 1
        ITEM_FIELD_STACK_COUNT = OBJECT_END + 0x008,	//  Size: 1, Type: Int32, Flags: 20
        ITEM_FIELD_DURATION = OBJECT_END + 0x009,	//  Size: 1, Type: Int32, Flags: 20
        ITEM_FIELD_SPELL_CHARGES = OBJECT_END + 0x00A,	//  Size: 5, Type: Int32, Flags: 20
        ITEM_FIELD_SPELL_CHARGES_01 = OBJECT_END + 0x00B,	//  Size: 5, Type: Int32, Flags: 20
        ITEM_FIELD_SPELL_CHARGES_02 = OBJECT_END + 0x00C,	//  Size: 5, Type: Int32, Flags: 20
        ITEM_FIELD_SPELL_CHARGES_03 = OBJECT_END + 0x00D,	//  Size: 5, Type: Int32, Flags: 20
        ITEM_FIELD_SPELL_CHARGES_04 = OBJECT_END + 0x00E,	//  Size: 5, Type: Int32, Flags: 20
        ITEM_FIELD_FLAGS = OBJECT_END + 0x00F,	//  Size: 1, Type: Int32, Flags: 1
        ITEM_FIELD_ENCHANTMENT = OBJECT_END + 0x010,	//  Size: 33, Type: Int32, Flags: 1
        ITEM_FIELD_ENCHANTMENT_01 = OBJECT_END + 0x011,	//  Size: 33, Type: Int32, Flags: 1
        ITEM_FIELD_ENCHANTMENT_02 = OBJECT_END + 0x012,	//  Size: 33, Type: Int32, Flags: 1
        ITEM_FIELD_ENCHANTMENT_03 = OBJECT_END + 0x013,	//  Size: 33, Type: Int32, Flags: 1
        ITEM_FIELD_ENCHANTMENT_04 = OBJECT_END + 0x014,	//  Size: 33, Type: Int32, Flags: 1
        ITEM_FIELD_ENCHANTMENT_05 = OBJECT_END + 0x015,	//  Size: 33, Type: Int32, Flags: 1
        ITEM_FIELD_ENCHANTMENT_06 = OBJECT_END + 0x016,	//  Size: 33, Type: Int32, Flags: 1
        ITEM_FIELD_ENCHANTMENT_07 = OBJECT_END + 0x017,	//  Size: 33, Type: Int32, Flags: 1
        ITEM_FIELD_ENCHANTMENT_08 = OBJECT_END + 0x018,	//  Size: 33, Type: Int32, Flags: 1
        ITEM_FIELD_ENCHANTMENT_09 = OBJECT_END + 0x019,	//  Size: 33, Type: Int32, Flags: 1
        ITEM_FIELD_ENCHANTMENT_10 = OBJECT_END + 0x01A,	//  Size: 33, Type: Int32, Flags: 1
        ITEM_FIELD_ENCHANTMENT_11 = OBJECT_END + 0x01B,	//  Size: 33, Type: Int32, Flags: 1
        ITEM_FIELD_ENCHANTMENT_12 = OBJECT_END + 0x01C,	//  Size: 33, Type: Int32, Flags: 1
        ITEM_FIELD_ENCHANTMENT_13 = OBJECT_END + 0x01D,	//  Size: 33, Type: Int32, Flags: 1
        ITEM_FIELD_ENCHANTMENT_14 = OBJECT_END + 0x01E,	//  Size: 33, Type: Int32, Flags: 1
        ITEM_FIELD_ENCHANTMENT_15 = OBJECT_END + 0x01F,	//  Size: 33, Type: Int32, Flags: 1
        ITEM_FIELD_ENCHANTMENT_16 = OBJECT_END + 0x020,	//  Size: 33, Type: Int32, Flags: 1
        ITEM_FIELD_ENCHANTMENT_17 = OBJECT_END + 0x021,	//  Size: 33, Type: Int32, Flags: 1
        ITEM_FIELD_ENCHANTMENT_18 = OBJECT_END + 0x022,	//  Size: 33, Type: Int32, Flags: 1
        ITEM_FIELD_ENCHANTMENT_19 = OBJECT_END + 0x023,	//  Size: 33, Type: Int32, Flags: 1
        ITEM_FIELD_ENCHANTMENT_20 = OBJECT_END + 0x024,	//  Size: 33, Type: Int32, Flags: 1
        ITEM_FIELD_ENCHANTMENT_21 = OBJECT_END + 0x025,	//  Size: 33, Type: Int32, Flags: 1
        ITEM_FIELD_ENCHANTMENT_22 = OBJECT_END + 0x026,	//  Size: 33, Type: Int32, Flags: 1
        ITEM_FIELD_ENCHANTMENT_23 = OBJECT_END + 0x027,	//  Size: 33, Type: Int32, Flags: 1
        ITEM_FIELD_ENCHANTMENT_24 = OBJECT_END + 0x028,	//  Size: 33, Type: Int32, Flags: 1
        ITEM_FIELD_ENCHANTMENT_25 = OBJECT_END + 0x029,	//  Size: 33, Type: Int32, Flags: 1
        ITEM_FIELD_ENCHANTMENT_26 = OBJECT_END + 0x02A,	//  Size: 33, Type: Int32, Flags: 1
        ITEM_FIELD_ENCHANTMENT_27 = OBJECT_END + 0x02B,	//  Size: 33, Type: Int32, Flags: 1
        ITEM_FIELD_ENCHANTMENT_28 = OBJECT_END + 0x02C,	//  Size: 33, Type: Int32, Flags: 1
        ITEM_FIELD_ENCHANTMENT_29 = OBJECT_END + 0x02D,	//  Size: 33, Type: Int32, Flags: 1
        ITEM_FIELD_ENCHANTMENT_30 = OBJECT_END + 0x02E,	//  Size: 33, Type: Int32, Flags: 1
        ITEM_FIELD_ENCHANTMENT_31 = OBJECT_END + 0x02F,	//  Size: 33, Type: Int32, Flags: 1
        ITEM_FIELD_ENCHANTMENT_32 = OBJECT_END + 0x030,	//  Size: 33, Type: Int32, Flags: 1
        ITEM_FIELD_PROPERTY_SEED = OBJECT_END + 0x031,	//  Size: 1, Type: Int32, Flags: 1
        ITEM_FIELD_RANDOM_PROPERTIES_ID = OBJECT_END + 0x032,	//  Size: 1, Type: Int32, Flags: 1
        ITEM_FIELD_ITEM_TEXT_ID = OBJECT_END + 0x033,	//  Size: 1, Type: Int32, Flags: 4
        ITEM_FIELD_DURABILITY = OBJECT_END + 0x034,	//  Size: 1, Type: Int32, Flags: 20
        ITEM_FIELD_MAXDURABILITY = OBJECT_END + 0x035,	//  Size: 1, Type: Int32, Flags: 20
        ITEM_END = OBJECT_END + 0x036,

        //UNIT:
        UNIT_FIELD_CHARM = OBJECT_END + 0x000,	//  Size: 2, Type: GUID, Flags: 1
        UNIT_FIELD_SUMMON = OBJECT_END + 0x002,	//  Size: 2, Type: GUID, Flags: 1
        UNIT_FIELD_CHARMEDBY = OBJECT_END + 0x004,	//  Size: 2, Type: GUID, Flags: 1
        UNIT_FIELD_SUMMONEDBY = OBJECT_END + 0x006,	//  Size: 2, Type: GUID, Flags: 1
        UNIT_FIELD_CREATEDBY = OBJECT_END + 0x008,	//  Size: 2, Type: GUID, Flags: 1
        UNIT_FIELD_TARGET = OBJECT_END + 0x00A,	//  Size: 2, Type: GUID, Flags: 1
        UNIT_FIELD_PERSUADED = OBJECT_END + 0x00C,	//  Size: 2, Type: GUID, Flags: 1
        UNIT_FIELD_CHANNEL_OBJECT = OBJECT_END + 0x00E,	//  Size: 2, Type: GUID, Flags: 1
        UNIT_FIELD_HEALTH = OBJECT_END + 0x010,	//  Size: 1, Type: Int32, Flags: 256
        UNIT_FIELD_POWER1 = OBJECT_END + 0x011,	//  Size: 1, Type: Int32, Flags: 1
        UNIT_FIELD_POWER2 = OBJECT_END + 0x012,	//  Size: 1, Type: Int32, Flags: 1
        UNIT_FIELD_POWER3 = OBJECT_END + 0x013,	//  Size: 1, Type: Int32, Flags: 1
        UNIT_FIELD_POWER4 = OBJECT_END + 0x014,	//  Size: 1, Type: Int32, Flags: 1
        UNIT_FIELD_POWER5 = OBJECT_END + 0x015,	//  Size: 1, Type: Int32, Flags: 1
        UNIT_FIELD_MAXHEALTH = OBJECT_END + 0x016,	//  Size: 1, Type: Int32, Flags: 256
        UNIT_FIELD_MAXPOWER1 = OBJECT_END + 0x017,	//  Size: 1, Type: Int32, Flags: 1
        UNIT_FIELD_MAXPOWER2 = OBJECT_END + 0x018,	//  Size: 1, Type: Int32, Flags: 1
        UNIT_FIELD_MAXPOWER3 = OBJECT_END + 0x019,	//  Size: 1, Type: Int32, Flags: 1
        UNIT_FIELD_MAXPOWER4 = OBJECT_END + 0x01A,	//  Size: 1, Type: Int32, Flags: 1
        UNIT_FIELD_MAXPOWER5 = OBJECT_END + 0x01B,	//  Size: 1, Type: Int32, Flags: 1
        UNIT_FIELD_LEVEL = OBJECT_END + 0x01C,	//  Size: 1, Type: Int32, Flags: 1
        UNIT_FIELD_FACTIONTEMPLATE = OBJECT_END + 0x01D,	//  Size: 1, Type: Int32, Flags: 1
        UNIT_FIELD_BYTES_0 = OBJECT_END + 0x01E,	//  Size: 1, Type: Bytes, Flags: 1
        UNIT_VIRTUAL_ITEM_SLOT_DISPLAY = OBJECT_END + 0x01F,	//  Size: 3, Type: Int32, Flags: 1
        UNIT_VIRTUAL_ITEM_SLOT_DISPLAY_01 = OBJECT_END + 0x020,	//  Size: 3, Type: Int32, Flags: 1
        UNIT_VIRTUAL_ITEM_SLOT_DISPLAY_02 = OBJECT_END + 0x021,	//  Size: 3, Type: Int32, Flags: 1
        UNIT_VIRTUAL_ITEM_INFO = OBJECT_END + 0x022,	//  Size: 6, Type: Bytes, Flags: 1
        UNIT_VIRTUAL_ITEM_INFO_01 = OBJECT_END + 0x023,	//  Size: 6, Type: Bytes, Flags: 1
        UNIT_VIRTUAL_ITEM_INFO_02 = OBJECT_END + 0x024,	//  Size: 6, Type: Bytes, Flags: 1
        UNIT_VIRTUAL_ITEM_INFO_03 = OBJECT_END + 0x025,	//  Size: 6, Type: Bytes, Flags: 1
        UNIT_VIRTUAL_ITEM_INFO_04 = OBJECT_END + 0x026,	//  Size: 6, Type: Bytes, Flags: 1
        UNIT_VIRTUAL_ITEM_INFO_05 = OBJECT_END + 0x027,	//  Size: 6, Type: Bytes, Flags: 1
        UNIT_FIELD_FLAGS = OBJECT_END + 0x028,	//  Size: 1, Type: Int32, Flags: 1
        UNIT_FIELD_FLAGS_2 = OBJECT_END + 0x029,	//  Size: 1, Type: Int32, Flags: 1
        UNIT_FIELD_AURA = OBJECT_END + 0x02A,	//  Size: 56, Type: Int32, Flags: 1
        UNIT_FIELD_AURA_01 = OBJECT_END + 0x02B,	//  Size: 56, Type: Int32, Flags: 1
        UNIT_FIELD_AURA_02 = OBJECT_END + 0x02C,	//  Size: 56, Type: Int32, Flags: 1
        UNIT_FIELD_AURA_03 = OBJECT_END + 0x02D,	//  Size: 56, Type: Int32, Flags: 1
        UNIT_FIELD_AURA_04 = OBJECT_END + 0x02E,	//  Size: 56, Type: Int32, Flags: 1
        UNIT_FIELD_AURA_05 = OBJECT_END + 0x02F,	//  Size: 56, Type: Int32, Flags: 1
        UNIT_FIELD_AURA_06 = OBJECT_END + 0x030,	//  Size: 56, Type: Int32, Flags: 1
        UNIT_FIELD_AURA_07 = OBJECT_END + 0x031,	//  Size: 56, Type: Int32, Flags: 1
        UNIT_FIELD_AURA_08 = OBJECT_END + 0x032,	//  Size: 56, Type: Int32, Flags: 1
        UNIT_FIELD_AURA_09 = OBJECT_END + 0x033,	//  Size: 56, Type: Int32, Flags: 1
        UNIT_FIELD_AURA_10 = OBJECT_END + 0x034,	//  Size: 56, Type: Int32, Flags: 1
        UNIT_FIELD_AURA_11 = OBJECT_END + 0x035,	//  Size: 56, Type: Int32, Flags: 1
        UNIT_FIELD_AURA_12 = OBJECT_END + 0x036,	//  Size: 56, Type: Int32, Flags: 1
        UNIT_FIELD_AURA_13 = OBJECT_END + 0x037,	//  Size: 56, Type: Int32, Flags: 1
        UNIT_FIELD_AURA_14 = OBJECT_END + 0x038,	//  Size: 56, Type: Int32, Flags: 1
        UNIT_FIELD_AURA_15 = OBJECT_END + 0x039,	//  Size: 56, Type: Int32, Flags: 1
        UNIT_FIELD_AURA_16 = OBJECT_END + 0x03A,	//  Size: 56, Type: Int32, Flags: 1
        UNIT_FIELD_AURA_17 = OBJECT_END + 0x03B,	//  Size: 56, Type: Int32, Flags: 1
        UNIT_FIELD_AURA_18 = OBJECT_END + 0x03C,	//  Size: 56, Type: Int32, Flags: 1
        UNIT_FIELD_AURA_19 = OBJECT_END + 0x03D,	//  Size: 56, Type: Int32, Flags: 1
        UNIT_FIELD_AURA_20 = OBJECT_END + 0x03E,	//  Size: 56, Type: Int32, Flags: 1
        UNIT_FIELD_AURA_21 = OBJECT_END + 0x03F,	//  Size: 56, Type: Int32, Flags: 1
        UNIT_FIELD_AURA_22 = OBJECT_END + 0x040,	//  Size: 56, Type: Int32, Flags: 1
        UNIT_FIELD_AURA_23 = OBJECT_END + 0x041,	//  Size: 56, Type: Int32, Flags: 1
        UNIT_FIELD_AURA_24 = OBJECT_END + 0x042,	//  Size: 56, Type: Int32, Flags: 1
        UNIT_FIELD_AURA_25 = OBJECT_END + 0x043,	//  Size: 56, Type: Int32, Flags: 1
        UNIT_FIELD_AURA_26 = OBJECT_END + 0x044,	//  Size: 56, Type: Int32, Flags: 1
        UNIT_FIELD_AURA_27 = OBJECT_END + 0x045,	//  Size: 56, Type: Int32, Flags: 1
        UNIT_FIELD_AURA_28 = OBJECT_END + 0x046,	//  Size: 56, Type: Int32, Flags: 1
        UNIT_FIELD_AURA_29 = OBJECT_END + 0x047,	//  Size: 56, Type: Int32, Flags: 1
        UNIT_FIELD_AURA_30 = OBJECT_END + 0x048,	//  Size: 56, Type: Int32, Flags: 1
        UNIT_FIELD_AURA_31 = OBJECT_END + 0x049,	//  Size: 56, Type: Int32, Flags: 1
        UNIT_FIELD_AURA_32 = OBJECT_END + 0x04A,	//  Size: 56, Type: Int32, Flags: 1
        UNIT_FIELD_AURA_33 = OBJECT_END + 0x04B,	//  Size: 56, Type: Int32, Flags: 1
        UNIT_FIELD_AURA_34 = OBJECT_END + 0x04C,	//  Size: 56, Type: Int32, Flags: 1
        UNIT_FIELD_AURA_35 = OBJECT_END + 0x04D,	//  Size: 56, Type: Int32, Flags: 1
        UNIT_FIELD_AURA_36 = OBJECT_END + 0x04E,	//  Size: 56, Type: Int32, Flags: 1
        UNIT_FIELD_AURA_37 = OBJECT_END + 0x04F,	//  Size: 56, Type: Int32, Flags: 1
        UNIT_FIELD_AURA_38 = OBJECT_END + 0x050,	//  Size: 56, Type: Int32, Flags: 1
        UNIT_FIELD_AURA_39 = OBJECT_END + 0x051,	//  Size: 56, Type: Int32, Flags: 1
        UNIT_FIELD_AURA_40 = OBJECT_END + 0x052,	//  Size: 56, Type: Int32, Flags: 1
        UNIT_FIELD_AURA_41 = OBJECT_END + 0x053,	//  Size: 56, Type: Int32, Flags: 1
        UNIT_FIELD_AURA_42 = OBJECT_END + 0x054,	//  Size: 56, Type: Int32, Flags: 1
        UNIT_FIELD_AURA_43 = OBJECT_END + 0x055,	//  Size: 56, Type: Int32, Flags: 1
        UNIT_FIELD_AURA_44 = OBJECT_END + 0x056,	//  Size: 56, Type: Int32, Flags: 1
        UNIT_FIELD_AURA_45 = OBJECT_END + 0x057,	//  Size: 56, Type: Int32, Flags: 1
        UNIT_FIELD_AURA_46 = OBJECT_END + 0x058,	//  Size: 56, Type: Int32, Flags: 1
        UNIT_FIELD_AURA_47 = OBJECT_END + 0x059,	//  Size: 56, Type: Int32, Flags: 1
        UNIT_FIELD_AURA_48 = OBJECT_END + 0x05A,	//  Size: 56, Type: Int32, Flags: 1
        UNIT_FIELD_AURA_49 = OBJECT_END + 0x05B,	//  Size: 56, Type: Int32, Flags: 1
        UNIT_FIELD_AURA_50 = OBJECT_END + 0x05C,	//  Size: 56, Type: Int32, Flags: 1
        UNIT_FIELD_AURA_51 = OBJECT_END + 0x05D,	//  Size: 56, Type: Int32, Flags: 1
        UNIT_FIELD_AURA_52 = OBJECT_END + 0x05E,	//  Size: 56, Type: Int32, Flags: 1
        UNIT_FIELD_AURA_53 = OBJECT_END + 0x05F,	//  Size: 56, Type: Int32, Flags: 1
        UNIT_FIELD_AURA_54 = OBJECT_END + 0x060,	//  Size: 56, Type: Int32, Flags: 1
        UNIT_FIELD_AURA_55 = OBJECT_END + 0x061,	//  Size: 56, Type: Int32, Flags: 1
        UNIT_FIELD_AURAFLAGS = OBJECT_END + 0x062,	//  Size: 14, Type: Bytes, Flags: 1
        UNIT_FIELD_AURAFLAGS_01 = OBJECT_END + 0x063,	//  Size: 14, Type: Bytes, Flags: 1
        UNIT_FIELD_AURAFLAGS_02 = OBJECT_END + 0x064,	//  Size: 14, Type: Bytes, Flags: 1
        UNIT_FIELD_AURAFLAGS_03 = OBJECT_END + 0x065,	//  Size: 14, Type: Bytes, Flags: 1
        UNIT_FIELD_AURAFLAGS_04 = OBJECT_END + 0x066,	//  Size: 14, Type: Bytes, Flags: 1
        UNIT_FIELD_AURAFLAGS_05 = OBJECT_END + 0x067,	//  Size: 14, Type: Bytes, Flags: 1
        UNIT_FIELD_AURAFLAGS_06 = OBJECT_END + 0x068,	//  Size: 14, Type: Bytes, Flags: 1
        UNIT_FIELD_AURAFLAGS_07 = OBJECT_END + 0x069,	//  Size: 14, Type: Bytes, Flags: 1
        UNIT_FIELD_AURAFLAGS_08 = OBJECT_END + 0x06A,	//  Size: 14, Type: Bytes, Flags: 1
        UNIT_FIELD_AURAFLAGS_09 = OBJECT_END + 0x06B,	//  Size: 14, Type: Bytes, Flags: 1
        UNIT_FIELD_AURAFLAGS_10 = OBJECT_END + 0x06C,	//  Size: 14, Type: Bytes, Flags: 1
        UNIT_FIELD_AURAFLAGS_11 = OBJECT_END + 0x06D,	//  Size: 14, Type: Bytes, Flags: 1
        UNIT_FIELD_AURAFLAGS_12 = OBJECT_END + 0x06E,	//  Size: 14, Type: Bytes, Flags: 1
        UNIT_FIELD_AURAFLAGS_13 = OBJECT_END + 0x06F,	//  Size: 14, Type: Bytes, Flags: 1
        UNIT_FIELD_AURALEVELS = OBJECT_END + 0x070,	//  Size: 14, Type: Bytes, Flags: 1
        UNIT_FIELD_AURALEVELS_01 = OBJECT_END + 0x071,	//  Size: 14, Type: Bytes, Flags: 1
        UNIT_FIELD_AURALEVELS_02 = OBJECT_END + 0x072,	//  Size: 14, Type: Bytes, Flags: 1
        UNIT_FIELD_AURALEVELS_03 = OBJECT_END + 0x073,	//  Size: 14, Type: Bytes, Flags: 1
        UNIT_FIELD_AURALEVELS_04 = OBJECT_END + 0x074,	//  Size: 14, Type: Bytes, Flags: 1
        UNIT_FIELD_AURALEVELS_05 = OBJECT_END + 0x075,	//  Size: 14, Type: Bytes, Flags: 1
        UNIT_FIELD_AURALEVELS_06 = OBJECT_END + 0x076,	//  Size: 14, Type: Bytes, Flags: 1
        UNIT_FIELD_AURALEVELS_07 = OBJECT_END + 0x077,	//  Size: 14, Type: Bytes, Flags: 1
        UNIT_FIELD_AURALEVELS_08 = OBJECT_END + 0x078,	//  Size: 14, Type: Bytes, Flags: 1
        UNIT_FIELD_AURALEVELS_09 = OBJECT_END + 0x079,	//  Size: 14, Type: Bytes, Flags: 1
        UNIT_FIELD_AURALEVELS_10 = OBJECT_END + 0x07A,	//  Size: 14, Type: Bytes, Flags: 1
        UNIT_FIELD_AURALEVELS_11 = OBJECT_END + 0x07B,	//  Size: 14, Type: Bytes, Flags: 1
        UNIT_FIELD_AURALEVELS_12 = OBJECT_END + 0x07C,	//  Size: 14, Type: Bytes, Flags: 1
        UNIT_FIELD_AURALEVELS_13 = OBJECT_END + 0x07D,	//  Size: 14, Type: Bytes, Flags: 1
        UNIT_FIELD_AURAAPPLICATIONS = OBJECT_END + 0x07E,	//  Size: 14, Type: Bytes, Flags: 1
        UNIT_FIELD_AURAAPPLICATIONS_01 = OBJECT_END + 0x07F,	//  Size: 14, Type: Bytes, Flags: 1
        UNIT_FIELD_AURAAPPLICATIONS_02 = OBJECT_END + 0x080,	//  Size: 14, Type: Bytes, Flags: 1
        UNIT_FIELD_AURAAPPLICATIONS_03 = OBJECT_END + 0x081,	//  Size: 14, Type: Bytes, Flags: 1
        UNIT_FIELD_AURAAPPLICATIONS_04 = OBJECT_END + 0x082,	//  Size: 14, Type: Bytes, Flags: 1
        UNIT_FIELD_AURAAPPLICATIONS_05 = OBJECT_END + 0x083,	//  Size: 14, Type: Bytes, Flags: 1
        UNIT_FIELD_AURAAPPLICATIONS_06 = OBJECT_END + 0x084,	//  Size: 14, Type: Bytes, Flags: 1
        UNIT_FIELD_AURAAPPLICATIONS_07 = OBJECT_END + 0x085,	//  Size: 14, Type: Bytes, Flags: 1
        UNIT_FIELD_AURAAPPLICATIONS_08 = OBJECT_END + 0x086,	//  Size: 14, Type: Bytes, Flags: 1
        UNIT_FIELD_AURAAPPLICATIONS_09 = OBJECT_END + 0x087,	//  Size: 14, Type: Bytes, Flags: 1
        UNIT_FIELD_AURAAPPLICATIONS_10 = OBJECT_END + 0x088,	//  Size: 14, Type: Bytes, Flags: 1
        UNIT_FIELD_AURAAPPLICATIONS_11 = OBJECT_END + 0x089,	//  Size: 14, Type: Bytes, Flags: 1
        UNIT_FIELD_AURAAPPLICATIONS_12 = OBJECT_END + 0x08A,	//  Size: 14, Type: Bytes, Flags: 1
        UNIT_FIELD_AURAAPPLICATIONS_13 = OBJECT_END + 0x08B,	//  Size: 14, Type: Bytes, Flags: 1
        UNIT_FIELD_AURASTATE = OBJECT_END + 0x08C,	//  Size: 1, Type: Int32, Flags: 1
        UNIT_FIELD_BASEATTACKTIME = OBJECT_END + 0x08D,	//  Size: 2, Type: Int32, Flags: 1
        UNIT_FIELD_BASEATTACKTIME_01 = OBJECT_END + 0x08E,	//  Size: 2, Type: Int32, Flags: 1
        UNIT_FIELD_RANGEDATTACKTIME = OBJECT_END + 0x08F,	//  Size: 1, Type: Int32, Flags: 2
        UNIT_FIELD_BOUNDINGRADIUS = OBJECT_END + 0x090,	//  Size: 1, Type: Float, Flags: 1
        UNIT_FIELD_COMBATREACH = OBJECT_END + 0x091,	//  Size: 1, Type: Float, Flags: 1
        UNIT_FIELD_DISPLAYID = OBJECT_END + 0x092,	//  Size: 1, Type: Int32, Flags: 1
        UNIT_FIELD_NATIVEDISPLAYID = OBJECT_END + 0x093,	//  Size: 1, Type: Int32, Flags: 1
        UNIT_FIELD_MOUNTDISPLAYID = OBJECT_END + 0x094,	//  Size: 1, Type: Int32, Flags: 1
        UNIT_FIELD_MINDAMAGE = OBJECT_END + 0x095,	//  Size: 1, Type: Float, Flags: 38
        UNIT_FIELD_MAXDAMAGE = OBJECT_END + 0x096,	//  Size: 1, Type: Float, Flags: 38
        UNIT_FIELD_MINOFFHANDDAMAGE = OBJECT_END + 0x097,	//  Size: 1, Type: Float, Flags: 38
        UNIT_FIELD_MAXOFFHANDDAMAGE = OBJECT_END + 0x098,	//  Size: 1, Type: Float, Flags: 38
        UNIT_FIELD_BYTES_1 = OBJECT_END + 0x099,	//  Size: 1, Type: Bytes, Flags: 1
        UNIT_FIELD_PETNUMBER = OBJECT_END + 0x09A,	//  Size: 1, Type: Int32, Flags: 1
        UNIT_FIELD_PET_NAME_TIMESTAMP = OBJECT_END + 0x09B,	//  Size: 1, Type: Int32, Flags: 1
        UNIT_FIELD_PETEXPERIENCE = OBJECT_END + 0x09C,	//  Size: 1, Type: Int32, Flags: 4
        UNIT_FIELD_PETNEXTLEVELEXP = OBJECT_END + 0x09D,	//  Size: 1, Type: Int32, Flags: 4
        UNIT_DYNAMIC_FLAGS = OBJECT_END + 0x09E,	//  Size: 1, Type: Int32, Flags: 256
        UNIT_CHANNEL_SPELL = OBJECT_END + 0x09F,	//  Size: 1, Type: Int32, Flags: 1
        UNIT_MOD_CAST_SPEED = OBJECT_END + 0x0A0,	//  Size: 1, Type: Float, Flags: 1
        UNIT_CREATED_BY_SPELL = OBJECT_END + 0x0A1,	//  Size: 1, Type: Int32, Flags: 1
        UNIT_NPC_FLAGS = OBJECT_END + 0x0A2,	//  Size: 1, Type: Int32, Flags: 256
        UNIT_NPC_EMOTESTATE = OBJECT_END + 0x0A3,	//  Size: 1, Type: Int32, Flags: 1
        UNIT_TRAINING_POINTS = OBJECT_END + 0x0A4,	//  Size: 1, Type: Chars?, Flags: 4
        UNIT_FIELD_STAT0 = OBJECT_END + 0x0A5,	//  Size: 1, Type: Int32, Flags: 6
        UNIT_FIELD_STAT1 = OBJECT_END + 0x0A6,	//  Size: 1, Type: Int32, Flags: 6
        UNIT_FIELD_STAT2 = OBJECT_END + 0x0A7,	//  Size: 1, Type: Int32, Flags: 6
        UNIT_FIELD_STAT3 = OBJECT_END + 0x0A8,	//  Size: 1, Type: Int32, Flags: 6
        UNIT_FIELD_STAT4 = OBJECT_END + 0x0A9,	//  Size: 1, Type: Int32, Flags: 6
        UNIT_FIELD_POSSTAT0 = OBJECT_END + 0x0AA,	//  Size: 1, Type: Int32, Flags: 6
        UNIT_FIELD_POSSTAT1 = OBJECT_END + 0x0AB,	//  Size: 1, Type: Int32, Flags: 6
        UNIT_FIELD_POSSTAT2 = OBJECT_END + 0x0AC,	//  Size: 1, Type: Int32, Flags: 6
        UNIT_FIELD_POSSTAT3 = OBJECT_END + 0x0AD,	//  Size: 1, Type: Int32, Flags: 6
        UNIT_FIELD_POSSTAT4 = OBJECT_END + 0x0AE,	//  Size: 1, Type: Int32, Flags: 6
        UNIT_FIELD_NEGSTAT0 = OBJECT_END + 0x0AF,	//  Size: 1, Type: Int32, Flags: 6
        UNIT_FIELD_NEGSTAT1 = OBJECT_END + 0x0B0,	//  Size: 1, Type: Int32, Flags: 6
        UNIT_FIELD_NEGSTAT2 = OBJECT_END + 0x0B1,	//  Size: 1, Type: Int32, Flags: 6
        UNIT_FIELD_NEGSTAT3 = OBJECT_END + 0x0B2,	//  Size: 1, Type: Int32, Flags: 6
        UNIT_FIELD_NEGSTAT4 = OBJECT_END + 0x0B3,	//  Size: 1, Type: Int32, Flags: 6
        UNIT_FIELD_RESISTANCES = OBJECT_END + 0x0B4,	//  Size: 7, Type: Int32, Flags: 38
        UNIT_FIELD_RESISTANCES_01 = OBJECT_END + 0x0B5,	//  Size: 7, Type: Int32, Flags: 38
        UNIT_FIELD_RESISTANCES_02 = OBJECT_END + 0x0B6,	//  Size: 7, Type: Int32, Flags: 38
        UNIT_FIELD_RESISTANCES_03 = OBJECT_END + 0x0B7,	//  Size: 7, Type: Int32, Flags: 38
        UNIT_FIELD_RESISTANCES_04 = OBJECT_END + 0x0B8,	//  Size: 7, Type: Int32, Flags: 38
        UNIT_FIELD_RESISTANCES_05 = OBJECT_END + 0x0B9,	//  Size: 7, Type: Int32, Flags: 38
        UNIT_FIELD_RESISTANCES_06 = OBJECT_END + 0x0BA,	//  Size: 7, Type: Int32, Flags: 38
        UNIT_FIELD_RESISTANCEBUFFMODSPOSITIVE = OBJECT_END + 0x0BB,	//  Size: 7, Type: Int32, Flags: 6
        UNIT_FIELD_RESISTANCEBUFFMODSPOSITIVE_01 = OBJECT_END + 0x0BC,	//  Size: 7, Type: Int32, Flags: 6
        UNIT_FIELD_RESISTANCEBUFFMODSPOSITIVE_02 = OBJECT_END + 0x0BD,	//  Size: 7, Type: Int32, Flags: 6
        UNIT_FIELD_RESISTANCEBUFFMODSPOSITIVE_03 = OBJECT_END + 0x0BE,	//  Size: 7, Type: Int32, Flags: 6
        UNIT_FIELD_RESISTANCEBUFFMODSPOSITIVE_04 = OBJECT_END + 0x0BF,	//  Size: 7, Type: Int32, Flags: 6
        UNIT_FIELD_RESISTANCEBUFFMODSPOSITIVE_05 = OBJECT_END + 0x0C0,	//  Size: 7, Type: Int32, Flags: 6
        UNIT_FIELD_RESISTANCEBUFFMODSPOSITIVE_06 = OBJECT_END + 0x0C1,	//  Size: 7, Type: Int32, Flags: 6
        UNIT_FIELD_RESISTANCEBUFFMODSNEGATIVE = OBJECT_END + 0x0C2,	//  Size: 7, Type: Int32, Flags: 6
        UNIT_FIELD_RESISTANCEBUFFMODSNEGATIVE_01 = OBJECT_END + 0x0C3,	//  Size: 7, Type: Int32, Flags: 6
        UNIT_FIELD_RESISTANCEBUFFMODSNEGATIVE_02 = OBJECT_END + 0x0C4,	//  Size: 7, Type: Int32, Flags: 6
        UNIT_FIELD_RESISTANCEBUFFMODSNEGATIVE_03 = OBJECT_END + 0x0C5,	//  Size: 7, Type: Int32, Flags: 6
        UNIT_FIELD_RESISTANCEBUFFMODSNEGATIVE_04 = OBJECT_END + 0x0C6,	//  Size: 7, Type: Int32, Flags: 6
        UNIT_FIELD_RESISTANCEBUFFMODSNEGATIVE_05 = OBJECT_END + 0x0C7,	//  Size: 7, Type: Int32, Flags: 6
        UNIT_FIELD_RESISTANCEBUFFMODSNEGATIVE_06 = OBJECT_END + 0x0C8,	//  Size: 7, Type: Int32, Flags: 6
        UNIT_FIELD_BASE_MANA = OBJECT_END + 0x0C9,	//  Size: 1, Type: Int32, Flags: 6
        UNIT_FIELD_BASE_HEALTH = OBJECT_END + 0x0CA,	//  Size: 1, Type: Int32, Flags: 6
        UNIT_FIELD_BYTES_2 = OBJECT_END + 0x0CB,	//  Size: 1, Type: Bytes, Flags: 1
        UNIT_FIELD_ATTACK_POWER = OBJECT_END + 0x0CC,	//  Size: 1, Type: Int32, Flags: 6
        UNIT_FIELD_ATTACK_POWER_MODS = OBJECT_END + 0x0CD,	//  Size: 1, Type: Chars?, Flags: 6
        UNIT_FIELD_ATTACK_POWER_MULTIPLIER = OBJECT_END + 0x0CE,	//  Size: 1, Type: Float, Flags: 6
        UNIT_FIELD_RANGED_ATTACK_POWER = OBJECT_END + 0x0CF,	//  Size: 1, Type: Int32, Flags: 6
        UNIT_FIELD_RANGED_ATTACK_POWER_MODS = OBJECT_END + 0x0D0,	//  Size: 1, Type: Chars?, Flags: 6
        UNIT_FIELD_RANGED_ATTACK_POWER_MULTIPLIER = OBJECT_END + 0x0D1,	//  Size: 1, Type: Float, Flags: 6
        UNIT_FIELD_MINRANGEDDAMAGE = OBJECT_END + 0x0D2,	//  Size: 1, Type: Float, Flags: 6
        UNIT_FIELD_MAXRANGEDDAMAGE = OBJECT_END + 0x0D3,	//  Size: 1, Type: Float, Flags: 6
        UNIT_FIELD_POWER_COST_MODIFIER = OBJECT_END + 0x0D4,	//  Size: 7, Type: Int32, Flags: 6
        UNIT_FIELD_POWER_COST_MODIFIER_01 = OBJECT_END + 0x0D5,	//  Size: 7, Type: Int32, Flags: 6
        UNIT_FIELD_POWER_COST_MODIFIER_02 = OBJECT_END + 0x0D6,	//  Size: 7, Type: Int32, Flags: 6
        UNIT_FIELD_POWER_COST_MODIFIER_03 = OBJECT_END + 0x0D7,	//  Size: 7, Type: Int32, Flags: 6
        UNIT_FIELD_POWER_COST_MODIFIER_04 = OBJECT_END + 0x0D8,	//  Size: 7, Type: Int32, Flags: 6
        UNIT_FIELD_POWER_COST_MODIFIER_05 = OBJECT_END + 0x0D9,	//  Size: 7, Type: Int32, Flags: 6
        UNIT_FIELD_POWER_COST_MODIFIER_06 = OBJECT_END + 0x0DA,	//  Size: 7, Type: Int32, Flags: 6
        UNIT_FIELD_POWER_COST_MULTIPLIER = OBJECT_END + 0x0DB,	//  Size: 7, Type: Float, Flags: 6
        UNIT_FIELD_POWER_COST_MULTIPLIER_01 = OBJECT_END + 0x0DC,	//  Size: 7, Type: Float, Flags: 6
        UNIT_FIELD_POWER_COST_MULTIPLIER_02 = OBJECT_END + 0x0DD,	//  Size: 7, Type: Float, Flags: 6
        UNIT_FIELD_POWER_COST_MULTIPLIER_03 = OBJECT_END + 0x0DE,	//  Size: 7, Type: Float, Flags: 6
        UNIT_FIELD_POWER_COST_MULTIPLIER_04 = OBJECT_END + 0x0DF,	//  Size: 7, Type: Float, Flags: 6
        UNIT_FIELD_POWER_COST_MULTIPLIER_05 = OBJECT_END + 0x0E0,	//  Size: 7, Type: Float, Flags: 6
        UNIT_FIELD_POWER_COST_MULTIPLIER_06 = OBJECT_END + 0x0E1,	//  Size: 7, Type: Float, Flags: 6
        UNIT_FIELD_MAXHEALTHMODIFIER = OBJECT_END + 0x0E2,	//  Size: 1, Type: Float, Flags: 6
        UNIT_FIELD_PADDING = OBJECT_END + 0x0E3,	//  Size: 1, Type: Int32, Flags: 0
        UNIT_END = OBJECT_END + 0x0E4,

        //PLAYER:
        PLAYER_DUEL_ARBITER = UNIT_END + 0x000,	//  Size: 2, Type: GUID, Flags: 1
        PLAYER_FLAGS = UNIT_END + 0x002,	//  Size: 1, Type: Int32, Flags: 1
        PLAYER_GUILDID = UNIT_END + 0x003,	//  Size: 1, Type: Int32, Flags: 1
        PLAYER_GUILDRANK = UNIT_END + 0x004,	//  Size: 1, Type: Int32, Flags: 1
        PLAYER_BYTES = UNIT_END + 0x005,	//  Size: 1, Type: Bytes, Flags: 1
        PLAYER_BYTES_2 = UNIT_END + 0x006,	//  Size: 1, Type: Bytes, Flags: 1
        PLAYER_BYTES_3 = UNIT_END + 0x007,	//  Size: 1, Type: Bytes, Flags: 1
        PLAYER_DUEL_TEAM = UNIT_END + 0x008,	//  Size: 1, Type: Int32, Flags: 1
        PLAYER_GUILD_TIMESTAMP = UNIT_END + 0x009,	//  Size: 1, Type: Int32, Flags: 1
        PLAYER_QUEST_LOG_1_1 = UNIT_END + 0x00A,	//  Size: 1, Type: Int32, Flags: 64
        PLAYER_QUEST_LOG_1_2 = UNIT_END + 0x00B,	//  Size: 1, Type: Int32, Flags: 2
        PLAYER_QUEST_LOG_1_3 = UNIT_END + 0x00C,	//  Size: 1, Type: Bytes, Flags: 2
        PLAYER_QUEST_LOG_1_4 = UNIT_END + 0x00D,	//  Size: 1, Type: Int32, Flags: 2
        PLAYER_QUEST_LOG_2_1 = UNIT_END + 0x00E,	//  Size: 1, Type: Int32, Flags: 64
        PLAYER_QUEST_LOG_2_2 = UNIT_END + 0x00F,	//  Size: 1, Type: Int32, Flags: 2
        PLAYER_QUEST_LOG_2_3 = UNIT_END + 0x010,	//  Size: 1, Type: Bytes, Flags: 2
        PLAYER_QUEST_LOG_2_4 = UNIT_END + 0x011,	//  Size: 1, Type: Int32, Flags: 2
        PLAYER_QUEST_LOG_3_1 = UNIT_END + 0x012,	//  Size: 1, Type: Int32, Flags: 64
        PLAYER_QUEST_LOG_3_2 = UNIT_END + 0x013,	//  Size: 1, Type: Int32, Flags: 2
        PLAYER_QUEST_LOG_3_3 = UNIT_END + 0x014,	//  Size: 1, Type: Bytes, Flags: 2
        PLAYER_QUEST_LOG_3_4 = UNIT_END + 0x015,	//  Size: 1, Type: Int32, Flags: 2
        PLAYER_QUEST_LOG_4_1 = UNIT_END + 0x016,	//  Size: 1, Type: Int32, Flags: 64
        PLAYER_QUEST_LOG_4_2 = UNIT_END + 0x017,	//  Size: 1, Type: Int32, Flags: 2
        PLAYER_QUEST_LOG_4_3 = UNIT_END + 0x018,	//  Size: 1, Type: Bytes, Flags: 2
        PLAYER_QUEST_LOG_4_4 = UNIT_END + 0x019,	//  Size: 1, Type: Int32, Flags: 2
        PLAYER_QUEST_LOG_5_1 = UNIT_END + 0x01A,	//  Size: 1, Type: Int32, Flags: 64
        PLAYER_QUEST_LOG_5_2 = UNIT_END + 0x01B,	//  Size: 1, Type: Int32, Flags: 2
        PLAYER_QUEST_LOG_5_3 = UNIT_END + 0x01C,	//  Size: 1, Type: Bytes, Flags: 2
        PLAYER_QUEST_LOG_5_4 = UNIT_END + 0x01D,	//  Size: 1, Type: Int32, Flags: 2
        PLAYER_QUEST_LOG_6_1 = UNIT_END + 0x01E,	//  Size: 1, Type: Int32, Flags: 64
        PLAYER_QUEST_LOG_6_2 = UNIT_END + 0x01F,	//  Size: 1, Type: Int32, Flags: 2
        PLAYER_QUEST_LOG_6_3 = UNIT_END + 0x020,	//  Size: 1, Type: Bytes, Flags: 2
        PLAYER_QUEST_LOG_6_4 = UNIT_END + 0x021,	//  Size: 1, Type: Int32, Flags: 2
        PLAYER_QUEST_LOG_7_1 = UNIT_END + 0x022,	//  Size: 1, Type: Int32, Flags: 64
        PLAYER_QUEST_LOG_7_2 = UNIT_END + 0x023,	//  Size: 1, Type: Int32, Flags: 2
        PLAYER_QUEST_LOG_7_3 = UNIT_END + 0x024,	//  Size: 1, Type: Bytes, Flags: 2
        PLAYER_QUEST_LOG_7_4 = UNIT_END + 0x025,	//  Size: 1, Type: Int32, Flags: 2
        PLAYER_QUEST_LOG_8_1 = UNIT_END + 0x026,	//  Size: 1, Type: Int32, Flags: 64
        PLAYER_QUEST_LOG_8_2 = UNIT_END + 0x027,	//  Size: 1, Type: Int32, Flags: 2
        PLAYER_QUEST_LOG_8_3 = UNIT_END + 0x028,	//  Size: 1, Type: Bytes, Flags: 2
        PLAYER_QUEST_LOG_8_4 = UNIT_END + 0x029,	//  Size: 1, Type: Int32, Flags: 2
        PLAYER_QUEST_LOG_9_1 = UNIT_END + 0x02A,	//  Size: 1, Type: Int32, Flags: 64
        PLAYER_QUEST_LOG_9_2 = UNIT_END + 0x02B,	//  Size: 1, Type: Int32, Flags: 2
        PLAYER_QUEST_LOG_9_3 = UNIT_END + 0x02C,	//  Size: 1, Type: Bytes, Flags: 2
        PLAYER_QUEST_LOG_9_4 = UNIT_END + 0x02D,	//  Size: 1, Type: Int32, Flags: 2
        PLAYER_QUEST_LOG_10_1 = UNIT_END + 0x02E,	//  Size: 1, Type: Int32, Flags: 64
        PLAYER_QUEST_LOG_10_2 = UNIT_END + 0x02F,	//  Size: 1, Type: Int32, Flags: 2
        PLAYER_QUEST_LOG_10_3 = UNIT_END + 0x030,	//  Size: 1, Type: Bytes, Flags: 2
        PLAYER_QUEST_LOG_10_4 = UNIT_END + 0x031,	//  Size: 1, Type: Int32, Flags: 2
        PLAYER_QUEST_LOG_11_1 = UNIT_END + 0x032,	//  Size: 1, Type: Int32, Flags: 64
        PLAYER_QUEST_LOG_11_2 = UNIT_END + 0x033,	//  Size: 1, Type: Int32, Flags: 2
        PLAYER_QUEST_LOG_11_3 = UNIT_END + 0x034,	//  Size: 1, Type: Bytes, Flags: 2
        PLAYER_QUEST_LOG_11_4 = UNIT_END + 0x035,	//  Size: 1, Type: Int32, Flags: 2
        PLAYER_QUEST_LOG_12_1 = UNIT_END + 0x036,	//  Size: 1, Type: Int32, Flags: 64
        PLAYER_QUEST_LOG_12_2 = UNIT_END + 0x037,	//  Size: 1, Type: Int32, Flags: 2
        PLAYER_QUEST_LOG_12_3 = UNIT_END + 0x038,	//  Size: 1, Type: Bytes, Flags: 2
        PLAYER_QUEST_LOG_12_4 = UNIT_END + 0x039,	//  Size: 1, Type: Int32, Flags: 2
        PLAYER_QUEST_LOG_13_1 = UNIT_END + 0x03A,	//  Size: 1, Type: Int32, Flags: 64
        PLAYER_QUEST_LOG_13_2 = UNIT_END + 0x03B,	//  Size: 1, Type: Int32, Flags: 2
        PLAYER_QUEST_LOG_13_3 = UNIT_END + 0x03C,	//  Size: 1, Type: Bytes, Flags: 2
        PLAYER_QUEST_LOG_13_4 = UNIT_END + 0x03D,	//  Size: 1, Type: Int32, Flags: 2
        PLAYER_QUEST_LOG_14_1 = UNIT_END + 0x03E,	//  Size: 1, Type: Int32, Flags: 64
        PLAYER_QUEST_LOG_14_2 = UNIT_END + 0x03F,	//  Size: 1, Type: Int32, Flags: 2
        PLAYER_QUEST_LOG_14_3 = UNIT_END + 0x040,	//  Size: 1, Type: Bytes, Flags: 2
        PLAYER_QUEST_LOG_14_4 = UNIT_END + 0x041,	//  Size: 1, Type: Int32, Flags: 2
        PLAYER_QUEST_LOG_15_1 = UNIT_END + 0x042,	//  Size: 1, Type: Int32, Flags: 64
        PLAYER_QUEST_LOG_15_2 = UNIT_END + 0x043,	//  Size: 1, Type: Int32, Flags: 2
        PLAYER_QUEST_LOG_15_3 = UNIT_END + 0x044,	//  Size: 1, Type: Bytes, Flags: 2
        PLAYER_QUEST_LOG_15_4 = UNIT_END + 0x045,	//  Size: 1, Type: Int32, Flags: 2
        PLAYER_QUEST_LOG_16_1 = UNIT_END + 0x046,	//  Size: 1, Type: Int32, Flags: 64
        PLAYER_QUEST_LOG_16_2 = UNIT_END + 0x047,	//  Size: 1, Type: Int32, Flags: 2
        PLAYER_QUEST_LOG_16_3 = UNIT_END + 0x048,	//  Size: 1, Type: Bytes, Flags: 2
        PLAYER_QUEST_LOG_16_4 = UNIT_END + 0x049,	//  Size: 1, Type: Int32, Flags: 2
        PLAYER_QUEST_LOG_17_1 = UNIT_END + 0x04A,	//  Size: 1, Type: Int32, Flags: 64
        PLAYER_QUEST_LOG_17_2 = UNIT_END + 0x04B,	//  Size: 1, Type: Int32, Flags: 2
        PLAYER_QUEST_LOG_17_3 = UNIT_END + 0x04C,	//  Size: 1, Type: Bytes, Flags: 2
        PLAYER_QUEST_LOG_17_4 = UNIT_END + 0x04D,	//  Size: 1, Type: Int32, Flags: 2
        PLAYER_QUEST_LOG_18_1 = UNIT_END + 0x04E,	//  Size: 1, Type: Int32, Flags: 64
        PLAYER_QUEST_LOG_18_2 = UNIT_END + 0x04F,	//  Size: 1, Type: Int32, Flags: 2
        PLAYER_QUEST_LOG_18_3 = UNIT_END + 0x050,	//  Size: 1, Type: Bytes, Flags: 2
        PLAYER_QUEST_LOG_18_4 = UNIT_END + 0x051,	//  Size: 1, Type: Int32, Flags: 2
        PLAYER_QUEST_LOG_19_1 = UNIT_END + 0x052,	//  Size: 1, Type: Int32, Flags: 64
        PLAYER_QUEST_LOG_19_2 = UNIT_END + 0x053,	//  Size: 1, Type: Int32, Flags: 2
        PLAYER_QUEST_LOG_19_3 = UNIT_END + 0x054,	//  Size: 1, Type: Bytes, Flags: 2
        PLAYER_QUEST_LOG_19_4 = UNIT_END + 0x055,	//  Size: 1, Type: Int32, Flags: 2
        PLAYER_QUEST_LOG_20_1 = UNIT_END + 0x056,	//  Size: 1, Type: Int32, Flags: 64
        PLAYER_QUEST_LOG_20_2 = UNIT_END + 0x057,	//  Size: 1, Type: Int32, Flags: 2
        PLAYER_QUEST_LOG_20_3 = UNIT_END + 0x058,	//  Size: 1, Type: Bytes, Flags: 2
        PLAYER_QUEST_LOG_20_4 = UNIT_END + 0x059,	//  Size: 1, Type: Int32, Flags: 2
        PLAYER_QUEST_LOG_21_1 = UNIT_END + 0x05A,	//  Size: 1, Type: Int32, Flags: 64
        PLAYER_QUEST_LOG_21_2 = UNIT_END + 0x05B,	//  Size: 1, Type: Int32, Flags: 2
        PLAYER_QUEST_LOG_21_3 = UNIT_END + 0x05C,	//  Size: 1, Type: Bytes, Flags: 2
        PLAYER_QUEST_LOG_21_4 = UNIT_END + 0x05D,	//  Size: 1, Type: Int32, Flags: 2
        PLAYER_QUEST_LOG_22_1 = UNIT_END + 0x05E,	//  Size: 1, Type: Int32, Flags: 64
        PLAYER_QUEST_LOG_22_2 = UNIT_END + 0x05F,	//  Size: 1, Type: Int32, Flags: 2
        PLAYER_QUEST_LOG_22_3 = UNIT_END + 0x060,	//  Size: 1, Type: Bytes, Flags: 2
        PLAYER_QUEST_LOG_22_4 = UNIT_END + 0x061,	//  Size: 1, Type: Int32, Flags: 2
        PLAYER_QUEST_LOG_23_1 = UNIT_END + 0x062,	//  Size: 1, Type: Int32, Flags: 64
        PLAYER_QUEST_LOG_23_2 = UNIT_END + 0x063,	//  Size: 1, Type: Int32, Flags: 2
        PLAYER_QUEST_LOG_23_3 = UNIT_END + 0x064,	//  Size: 1, Type: Bytes, Flags: 2
        PLAYER_QUEST_LOG_23_4 = UNIT_END + 0x065,	//  Size: 1, Type: Int32, Flags: 2
        PLAYER_QUEST_LOG_24_1 = UNIT_END + 0x066,	//  Size: 1, Type: Int32, Flags: 64
        PLAYER_QUEST_LOG_24_2 = UNIT_END + 0x067,	//  Size: 1, Type: Int32, Flags: 2
        PLAYER_QUEST_LOG_24_3 = UNIT_END + 0x068,	//  Size: 1, Type: Bytes, Flags: 2
        PLAYER_QUEST_LOG_24_4 = UNIT_END + 0x069,	//  Size: 1, Type: Int32, Flags: 2
        PLAYER_QUEST_LOG_25_1 = UNIT_END + 0x06A,	//  Size: 1, Type: Int32, Flags: 64
        PLAYER_QUEST_LOG_25_2 = UNIT_END + 0x06B,	//  Size: 1, Type: Int32, Flags: 2
        PLAYER_QUEST_LOG_25_3 = UNIT_END + 0x06C,	//  Size: 1, Type: Bytes, Flags: 2
        PLAYER_QUEST_LOG_25_4 = UNIT_END + 0x06D,	//  Size: 1, Type: Int32, Flags: 2
        PLAYER_VISIBLE_ITEM_1_CREATOR = UNIT_END + 0x06E,	//  Size: 2, Type: GUID, Flags: 1
        PLAYER_VISIBLE_ITEM_1_0 = UNIT_END + 0x070,	//  Size: 12, Type: Int32, Flags: 1
        PLAYER_VISIBLE_ITEM_1_01 = UNIT_END + 0x071,	//  Size: 12, Type: Int32, Flags: 1
        PLAYER_VISIBLE_ITEM_1_02 = UNIT_END + 0x072,	//  Size: 12, Type: Int32, Flags: 1
        PLAYER_VISIBLE_ITEM_1_03 = UNIT_END + 0x073,	//  Size: 12, Type: Int32, Flags: 1
        PLAYER_VISIBLE_ITEM_1_04 = UNIT_END + 0x074,	//  Size: 12, Type: Int32, Flags: 1
        PLAYER_VISIBLE_ITEM_1_05 = UNIT_END + 0x075,	//  Size: 12, Type: Int32, Flags: 1
        PLAYER_VISIBLE_ITEM_1_06 = UNIT_END + 0x076,	//  Size: 12, Type: Int32, Flags: 1
        PLAYER_VISIBLE_ITEM_1_07 = UNIT_END + 0x077,	//  Size: 12, Type: Int32, Flags: 1
        PLAYER_VISIBLE_ITEM_1_08 = UNIT_END + 0x078,	//  Size: 12, Type: Int32, Flags: 1
        PLAYER_VISIBLE_ITEM_1_09 = UNIT_END + 0x079,	//  Size: 12, Type: Int32, Flags: 1
        PLAYER_VISIBLE_ITEM_1_10 = UNIT_END + 0x07A,	//  Size: 12, Type: Int32, Flags: 1
        PLAYER_VISIBLE_ITEM_1_11 = UNIT_END + 0x07B,	//  Size: 12, Type: Int32, Flags: 1
        PLAYER_VISIBLE_ITEM_1_PROPERTIES = UNIT_END + 0x07C,	//  Size: 1, Type: Chars?, Flags: 1
        PLAYER_VISIBLE_ITEM_1_PAD = UNIT_END + 0x07D,	//  Size: 1, Type: Int32, Flags: 1
        PLAYER_VISIBLE_ITEM_2_CREATOR = UNIT_END + 0x07E,	//  Size: 2, Type: GUID, Flags: 1
        PLAYER_VISIBLE_ITEM_2_0 = UNIT_END + 0x080,	//  Size: 12, Type: Int32, Flags: 1
        PLAYER_VISIBLE_ITEM_2_01 = UNIT_END + 0x081,	//  Size: 12, Type: Int32, Flags: 1
        PLAYER_VISIBLE_ITEM_2_02 = UNIT_END + 0x082,	//  Size: 12, Type: Int32, Flags: 1
        PLAYER_VISIBLE_ITEM_2_03 = UNIT_END + 0x083,	//  Size: 12, Type: Int32, Flags: 1
        PLAYER_VISIBLE_ITEM_2_04 = UNIT_END + 0x084,	//  Size: 12, Type: Int32, Flags: 1
        PLAYER_VISIBLE_ITEM_2_05 = UNIT_END + 0x085,	//  Size: 12, Type: Int32, Flags: 1
        PLAYER_VISIBLE_ITEM_2_06 = UNIT_END + 0x086,	//  Size: 12, Type: Int32, Flags: 1
        PLAYER_VISIBLE_ITEM_2_07 = UNIT_END + 0x087,	//  Size: 12, Type: Int32, Flags: 1
        PLAYER_VISIBLE_ITEM_2_08 = UNIT_END + 0x088,	//  Size: 12, Type: Int32, Flags: 1
        PLAYER_VISIBLE_ITEM_2_09 = UNIT_END + 0x089,	//  Size: 12, Type: Int32, Flags: 1
        PLAYER_VISIBLE_ITEM_2_10 = UNIT_END + 0x08A,	//  Size: 12, Type: Int32, Flags: 1
        PLAYER_VISIBLE_ITEM_2_11 = UNIT_END + 0x08B,	//  Size: 12, Type: Int32, Flags: 1
        PLAYER_VISIBLE_ITEM_2_PROPERTIES = UNIT_END + 0x08C,	//  Size: 1, Type: Chars?, Flags: 1
        PLAYER_VISIBLE_ITEM_2_PAD = UNIT_END + 0x08D,	//  Size: 1, Type: Int32, Flags: 1
        PLAYER_VISIBLE_ITEM_3_CREATOR = UNIT_END + 0x08E,	//  Size: 2, Type: GUID, Flags: 1
        PLAYER_VISIBLE_ITEM_3_0 = UNIT_END + 0x090,	//  Size: 12, Type: Int32, Flags: 1
        PLAYER_VISIBLE_ITEM_3_01 = UNIT_END + 0x091,	//  Size: 12, Type: Int32, Flags: 1
        PLAYER_VISIBLE_ITEM_3_02 = UNIT_END + 0x092,	//  Size: 12, Type: Int32, Flags: 1
        PLAYER_VISIBLE_ITEM_3_03 = UNIT_END + 0x093,	//  Size: 12, Type: Int32, Flags: 1
        PLAYER_VISIBLE_ITEM_3_04 = UNIT_END + 0x094,	//  Size: 12, Type: Int32, Flags: 1
        PLAYER_VISIBLE_ITEM_3_05 = UNIT_END + 0x095,	//  Size: 12, Type: Int32, Flags: 1
        PLAYER_VISIBLE_ITEM_3_06 = UNIT_END + 0x096,	//  Size: 12, Type: Int32, Flags: 1
        PLAYER_VISIBLE_ITEM_3_07 = UNIT_END + 0x097,	//  Size: 12, Type: Int32, Flags: 1
        PLAYER_VISIBLE_ITEM_3_08 = UNIT_END + 0x098,	//  Size: 12, Type: Int32, Flags: 1
        PLAYER_VISIBLE_ITEM_3_09 = UNIT_END + 0x099,	//  Size: 12, Type: Int32, Flags: 1
        PLAYER_VISIBLE_ITEM_3_10 = UNIT_END + 0x09A,	//  Size: 12, Type: Int32, Flags: 1
        PLAYER_VISIBLE_ITEM_3_11 = UNIT_END + 0x09B,	//  Size: 12, Type: Int32, Flags: 1
        PLAYER_VISIBLE_ITEM_3_PROPERTIES = UNIT_END + 0x09C,	//  Size: 1, Type: Chars?, Flags: 1
        PLAYER_VISIBLE_ITEM_3_PAD = UNIT_END + 0x09D,	//  Size: 1, Type: Int32, Flags: 1
        PLAYER_VISIBLE_ITEM_4_CREATOR = UNIT_END + 0x09E,	//  Size: 2, Type: GUID, Flags: 1
        PLAYER_VISIBLE_ITEM_4_0 = UNIT_END + 0x0A0,	//  Size: 12, Type: Int32, Flags: 1
        PLAYER_VISIBLE_ITEM_4_01 = UNIT_END + 0x0A1,	//  Size: 12, Type: Int32, Flags: 1
        PLAYER_VISIBLE_ITEM_4_02 = UNIT_END + 0x0A2,	//  Size: 12, Type: Int32, Flags: 1
        PLAYER_VISIBLE_ITEM_4_03 = UNIT_END + 0x0A3,	//  Size: 12, Type: Int32, Flags: 1
        PLAYER_VISIBLE_ITEM_4_04 = UNIT_END + 0x0A4,	//  Size: 12, Type: Int32, Flags: 1
        PLAYER_VISIBLE_ITEM_4_05 = UNIT_END + 0x0A5,	//  Size: 12, Type: Int32, Flags: 1
        PLAYER_VISIBLE_ITEM_4_06 = UNIT_END + 0x0A6,	//  Size: 12, Type: Int32, Flags: 1
        PLAYER_VISIBLE_ITEM_4_07 = UNIT_END + 0x0A7,	//  Size: 12, Type: Int32, Flags: 1
        PLAYER_VISIBLE_ITEM_4_08 = UNIT_END + 0x0A8,	//  Size: 12, Type: Int32, Flags: 1
        PLAYER_VISIBLE_ITEM_4_09 = UNIT_END + 0x0A9,	//  Size: 12, Type: Int32, Flags: 1
        PLAYER_VISIBLE_ITEM_4_10 = UNIT_END + 0x0AA,	//  Size: 12, Type: Int32, Flags: 1
        PLAYER_VISIBLE_ITEM_4_11 = UNIT_END + 0x0AB,	//  Size: 12, Type: Int32, Flags: 1
        PLAYER_VISIBLE_ITEM_4_PROPERTIES = UNIT_END + 0x0AC,	//  Size: 1, Type: Chars?, Flags: 1
        PLAYER_VISIBLE_ITEM_4_PAD = UNIT_END + 0x0AD,	//  Size: 1, Type: Int32, Flags: 1
        PLAYER_VISIBLE_ITEM_5_CREATOR = UNIT_END + 0x0AE,	//  Size: 2, Type: GUID, Flags: 1
        PLAYER_VISIBLE_ITEM_5_0 = UNIT_END + 0x0B0,	//  Size: 12, Type: Int32, Flags: 1
        PLAYER_VISIBLE_ITEM_5_01 = UNIT_END + 0x0B1,	//  Size: 12, Type: Int32, Flags: 1
        PLAYER_VISIBLE_ITEM_5_02 = UNIT_END + 0x0B2,	//  Size: 12, Type: Int32, Flags: 1
        PLAYER_VISIBLE_ITEM_5_03 = UNIT_END + 0x0B3,	//  Size: 12, Type: Int32, Flags: 1
        PLAYER_VISIBLE_ITEM_5_04 = UNIT_END + 0x0B4,	//  Size: 12, Type: Int32, Flags: 1
        PLAYER_VISIBLE_ITEM_5_05 = UNIT_END + 0x0B5,	//  Size: 12, Type: Int32, Flags: 1
        PLAYER_VISIBLE_ITEM_5_06 = UNIT_END + 0x0B6,	//  Size: 12, Type: Int32, Flags: 1
        PLAYER_VISIBLE_ITEM_5_07 = UNIT_END + 0x0B7,	//  Size: 12, Type: Int32, Flags: 1
        PLAYER_VISIBLE_ITEM_5_08 = UNIT_END + 0x0B8,	//  Size: 12, Type: Int32, Flags: 1
        PLAYER_VISIBLE_ITEM_5_09 = UNIT_END + 0x0B9,	//  Size: 12, Type: Int32, Flags: 1
        PLAYER_VISIBLE_ITEM_5_10 = UNIT_END + 0x0BA,	//  Size: 12, Type: Int32, Flags: 1
        PLAYER_VISIBLE_ITEM_5_11 = UNIT_END + 0x0BB,	//  Size: 12, Type: Int32, Flags: 1
        PLAYER_VISIBLE_ITEM_5_PROPERTIES = UNIT_END + 0x0BC,	//  Size: 1, Type: Chars?, Flags: 1
        PLAYER_VISIBLE_ITEM_5_PAD = UNIT_END + 0x0BD,	//  Size: 1, Type: Int32, Flags: 1
        PLAYER_VISIBLE_ITEM_6_CREATOR = UNIT_END + 0x0BE,	//  Size: 2, Type: GUID, Flags: 1
        PLAYER_VISIBLE_ITEM_6_0 = UNIT_END + 0x0C0,	//  Size: 12, Type: Int32, Flags: 1
        PLAYER_VISIBLE_ITEM_6_01 = UNIT_END + 0x0C1,	//  Size: 12, Type: Int32, Flags: 1
        PLAYER_VISIBLE_ITEM_6_02 = UNIT_END + 0x0C2,	//  Size: 12, Type: Int32, Flags: 1
        PLAYER_VISIBLE_ITEM_6_03 = UNIT_END + 0x0C3,	//  Size: 12, Type: Int32, Flags: 1
        PLAYER_VISIBLE_ITEM_6_04 = UNIT_END + 0x0C4,	//  Size: 12, Type: Int32, Flags: 1
        PLAYER_VISIBLE_ITEM_6_05 = UNIT_END + 0x0C5,	//  Size: 12, Type: Int32, Flags: 1
        PLAYER_VISIBLE_ITEM_6_06 = UNIT_END + 0x0C6,	//  Size: 12, Type: Int32, Flags: 1
        PLAYER_VISIBLE_ITEM_6_07 = UNIT_END + 0x0C7,	//  Size: 12, Type: Int32, Flags: 1
        PLAYER_VISIBLE_ITEM_6_08 = UNIT_END + 0x0C8,	//  Size: 12, Type: Int32, Flags: 1
        PLAYER_VISIBLE_ITEM_6_09 = UNIT_END + 0x0C9,	//  Size: 12, Type: Int32, Flags: 1
        PLAYER_VISIBLE_ITEM_6_10 = UNIT_END + 0x0CA,	//  Size: 12, Type: Int32, Flags: 1
        PLAYER_VISIBLE_ITEM_6_11 = UNIT_END + 0x0CB,	//  Size: 12, Type: Int32, Flags: 1
        PLAYER_VISIBLE_ITEM_6_PROPERTIES = UNIT_END + 0x0CC,	//  Size: 1, Type: Chars?, Flags: 1
        PLAYER_VISIBLE_ITEM_6_PAD = UNIT_END + 0x0CD,	//  Size: 1, Type: Int32, Flags: 1
        PLAYER_VISIBLE_ITEM_7_CREATOR = UNIT_END + 0x0CE,	//  Size: 2, Type: GUID, Flags: 1
        PLAYER_VISIBLE_ITEM_7_0 = UNIT_END + 0x0D0,	//  Size: 12, Type: Int32, Flags: 1
        PLAYER_VISIBLE_ITEM_7_01 = UNIT_END + 0x0D1,	//  Size: 12, Type: Int32, Flags: 1
        PLAYER_VISIBLE_ITEM_7_02 = UNIT_END + 0x0D2,	//  Size: 12, Type: Int32, Flags: 1
        PLAYER_VISIBLE_ITEM_7_03 = UNIT_END + 0x0D3,	//  Size: 12, Type: Int32, Flags: 1
        PLAYER_VISIBLE_ITEM_7_04 = UNIT_END + 0x0D4,	//  Size: 12, Type: Int32, Flags: 1
        PLAYER_VISIBLE_ITEM_7_05 = UNIT_END + 0x0D5,	//  Size: 12, Type: Int32, Flags: 1
        PLAYER_VISIBLE_ITEM_7_06 = UNIT_END + 0x0D6,	//  Size: 12, Type: Int32, Flags: 1
        PLAYER_VISIBLE_ITEM_7_07 = UNIT_END + 0x0D7,	//  Size: 12, Type: Int32, Flags: 1
        PLAYER_VISIBLE_ITEM_7_08 = UNIT_END + 0x0D8,	//  Size: 12, Type: Int32, Flags: 1
        PLAYER_VISIBLE_ITEM_7_09 = UNIT_END + 0x0D9,	//  Size: 12, Type: Int32, Flags: 1
        PLAYER_VISIBLE_ITEM_7_10 = UNIT_END + 0x0DA,	//  Size: 12, Type: Int32, Flags: 1
        PLAYER_VISIBLE_ITEM_7_11 = UNIT_END + 0x0DB,	//  Size: 12, Type: Int32, Flags: 1
        PLAYER_VISIBLE_ITEM_7_PROPERTIES = UNIT_END + 0x0DC,	//  Size: 1, Type: Chars?, Flags: 1
        PLAYER_VISIBLE_ITEM_7_PAD = UNIT_END + 0x0DD,	//  Size: 1, Type: Int32, Flags: 1
        PLAYER_VISIBLE_ITEM_8_CREATOR = UNIT_END + 0x0DE,	//  Size: 2, Type: GUID, Flags: 1
        PLAYER_VISIBLE_ITEM_8_0 = UNIT_END + 0x0E0,	//  Size: 12, Type: Int32, Flags: 1
        PLAYER_VISIBLE_ITEM_8_01 = UNIT_END + 0x0E1,	//  Size: 12, Type: Int32, Flags: 1
        PLAYER_VISIBLE_ITEM_8_02 = UNIT_END + 0x0E2,	//  Size: 12, Type: Int32, Flags: 1
        PLAYER_VISIBLE_ITEM_8_03 = UNIT_END + 0x0E3,	//  Size: 12, Type: Int32, Flags: 1
        PLAYER_VISIBLE_ITEM_8_04 = UNIT_END + 0x0E4,	//  Size: 12, Type: Int32, Flags: 1
        PLAYER_VISIBLE_ITEM_8_05 = UNIT_END + 0x0E5,	//  Size: 12, Type: Int32, Flags: 1
        PLAYER_VISIBLE_ITEM_8_06 = UNIT_END + 0x0E6,	//  Size: 12, Type: Int32, Flags: 1
        PLAYER_VISIBLE_ITEM_8_07 = UNIT_END + 0x0E7,	//  Size: 12, Type: Int32, Flags: 1
        PLAYER_VISIBLE_ITEM_8_08 = UNIT_END + 0x0E8,	//  Size: 12, Type: Int32, Flags: 1
        PLAYER_VISIBLE_ITEM_8_09 = UNIT_END + 0x0E9,	//  Size: 12, Type: Int32, Flags: 1
        PLAYER_VISIBLE_ITEM_8_10 = UNIT_END + 0x0EA,	//  Size: 12, Type: Int32, Flags: 1
        PLAYER_VISIBLE_ITEM_8_11 = UNIT_END + 0x0EB,	//  Size: 12, Type: Int32, Flags: 1
        PLAYER_VISIBLE_ITEM_8_PROPERTIES = UNIT_END + 0x0EC,	//  Size: 1, Type: Chars?, Flags: 1
        PLAYER_VISIBLE_ITEM_8_PAD = UNIT_END + 0x0ED,	//  Size: 1, Type: Int32, Flags: 1
        PLAYER_VISIBLE_ITEM_9_CREATOR = UNIT_END + 0x0EE,	//  Size: 2, Type: GUID, Flags: 1
        PLAYER_VISIBLE_ITEM_9_0 = UNIT_END + 0x0F0,	//  Size: 12, Type: Int32, Flags: 1
        PLAYER_VISIBLE_ITEM_9_01 = UNIT_END + 0x0F1,	//  Size: 12, Type: Int32, Flags: 1
        PLAYER_VISIBLE_ITEM_9_02 = UNIT_END + 0x0F2,	//  Size: 12, Type: Int32, Flags: 1
        PLAYER_VISIBLE_ITEM_9_03 = UNIT_END + 0x0F3,	//  Size: 12, Type: Int32, Flags: 1
        PLAYER_VISIBLE_ITEM_9_04 = UNIT_END + 0x0F4,	//  Size: 12, Type: Int32, Flags: 1
        PLAYER_VISIBLE_ITEM_9_05 = UNIT_END + 0x0F5,	//  Size: 12, Type: Int32, Flags: 1
        PLAYER_VISIBLE_ITEM_9_06 = UNIT_END + 0x0F6,	//  Size: 12, Type: Int32, Flags: 1
        PLAYER_VISIBLE_ITEM_9_07 = UNIT_END + 0x0F7,	//  Size: 12, Type: Int32, Flags: 1
        PLAYER_VISIBLE_ITEM_9_08 = UNIT_END + 0x0F8,	//  Size: 12, Type: Int32, Flags: 1
        PLAYER_VISIBLE_ITEM_9_09 = UNIT_END + 0x0F9,	//  Size: 12, Type: Int32, Flags: 1
        PLAYER_VISIBLE_ITEM_9_10 = UNIT_END + 0x0FA,	//  Size: 12, Type: Int32, Flags: 1
        PLAYER_VISIBLE_ITEM_9_11 = UNIT_END + 0x0FB,	//  Size: 12, Type: Int32, Flags: 1
        PLAYER_VISIBLE_ITEM_9_PROPERTIES = UNIT_END + 0x0FC,	//  Size: 1, Type: Chars?, Flags: 1
        PLAYER_VISIBLE_ITEM_9_PAD = UNIT_END + 0x0FD,	//  Size: 1, Type: Int32, Flags: 1
        PLAYER_VISIBLE_ITEM_10_CREATOR = UNIT_END + 0x0FE,	//  Size: 2, Type: GUID, Flags: 1
        PLAYER_VISIBLE_ITEM_10_0 = UNIT_END + 0x100,	//  Size: 12, Type: Int32, Flags: 1
        PLAYER_VISIBLE_ITEM_10_01 = UNIT_END + 0x101,	//  Size: 12, Type: Int32, Flags: 1
        PLAYER_VISIBLE_ITEM_10_02 = UNIT_END + 0x102,	//  Size: 12, Type: Int32, Flags: 1
        PLAYER_VISIBLE_ITEM_10_03 = UNIT_END + 0x103,	//  Size: 12, Type: Int32, Flags: 1
        PLAYER_VISIBLE_ITEM_10_04 = UNIT_END + 0x104,	//  Size: 12, Type: Int32, Flags: 1
        PLAYER_VISIBLE_ITEM_10_05 = UNIT_END + 0x105,	//  Size: 12, Type: Int32, Flags: 1
        PLAYER_VISIBLE_ITEM_10_06 = UNIT_END + 0x106,	//  Size: 12, Type: Int32, Flags: 1
        PLAYER_VISIBLE_ITEM_10_07 = UNIT_END + 0x107,	//  Size: 12, Type: Int32, Flags: 1
        PLAYER_VISIBLE_ITEM_10_08 = UNIT_END + 0x108,	//  Size: 12, Type: Int32, Flags: 1
        PLAYER_VISIBLE_ITEM_10_09 = UNIT_END + 0x109,	//  Size: 12, Type: Int32, Flags: 1
        PLAYER_VISIBLE_ITEM_10_10 = UNIT_END + 0x10A,	//  Size: 12, Type: Int32, Flags: 1
        PLAYER_VISIBLE_ITEM_10_11 = UNIT_END + 0x10B,	//  Size: 12, Type: Int32, Flags: 1
        PLAYER_VISIBLE_ITEM_10_PROPERTIES = UNIT_END + 0x10C,	//  Size: 1, Type: Chars?, Flags: 1
        PLAYER_VISIBLE_ITEM_10_PAD = UNIT_END + 0x10D,	//  Size: 1, Type: Int32, Flags: 1
        PLAYER_VISIBLE_ITEM_11_CREATOR = UNIT_END + 0x10E,	//  Size: 2, Type: GUID, Flags: 1
        PLAYER_VISIBLE_ITEM_11_0 = UNIT_END + 0x110,	//  Size: 12, Type: Int32, Flags: 1
        PLAYER_VISIBLE_ITEM_11_01 = UNIT_END + 0x111,	//  Size: 12, Type: Int32, Flags: 1
        PLAYER_VISIBLE_ITEM_11_02 = UNIT_END + 0x112,	//  Size: 12, Type: Int32, Flags: 1
        PLAYER_VISIBLE_ITEM_11_03 = UNIT_END + 0x113,	//  Size: 12, Type: Int32, Flags: 1
        PLAYER_VISIBLE_ITEM_11_04 = UNIT_END + 0x114,	//  Size: 12, Type: Int32, Flags: 1
        PLAYER_VISIBLE_ITEM_11_05 = UNIT_END + 0x115,	//  Size: 12, Type: Int32, Flags: 1
        PLAYER_VISIBLE_ITEM_11_06 = UNIT_END + 0x116,	//  Size: 12, Type: Int32, Flags: 1
        PLAYER_VISIBLE_ITEM_11_07 = UNIT_END + 0x117,	//  Size: 12, Type: Int32, Flags: 1
        PLAYER_VISIBLE_ITEM_11_08 = UNIT_END + 0x118,	//  Size: 12, Type: Int32, Flags: 1
        PLAYER_VISIBLE_ITEM_11_09 = UNIT_END + 0x119,	//  Size: 12, Type: Int32, Flags: 1
        PLAYER_VISIBLE_ITEM_11_10 = UNIT_END + 0x11A,	//  Size: 12, Type: Int32, Flags: 1
        PLAYER_VISIBLE_ITEM_11_11 = UNIT_END + 0x11B,	//  Size: 12, Type: Int32, Flags: 1
        PLAYER_VISIBLE_ITEM_11_PROPERTIES = UNIT_END + 0x11C,	//  Size: 1, Type: Chars?, Flags: 1
        PLAYER_VISIBLE_ITEM_11_PAD = UNIT_END + 0x11D,	//  Size: 1, Type: Int32, Flags: 1
        PLAYER_VISIBLE_ITEM_12_CREATOR = UNIT_END + 0x11E,	//  Size: 2, Type: GUID, Flags: 1
        PLAYER_VISIBLE_ITEM_12_0 = UNIT_END + 0x120,	//  Size: 12, Type: Int32, Flags: 1
        PLAYER_VISIBLE_ITEM_12_01 = UNIT_END + 0x121,	//  Size: 12, Type: Int32, Flags: 1
        PLAYER_VISIBLE_ITEM_12_02 = UNIT_END + 0x122,	//  Size: 12, Type: Int32, Flags: 1
        PLAYER_VISIBLE_ITEM_12_03 = UNIT_END + 0x123,	//  Size: 12, Type: Int32, Flags: 1
        PLAYER_VISIBLE_ITEM_12_04 = UNIT_END + 0x124,	//  Size: 12, Type: Int32, Flags: 1
        PLAYER_VISIBLE_ITEM_12_05 = UNIT_END + 0x125,	//  Size: 12, Type: Int32, Flags: 1
        PLAYER_VISIBLE_ITEM_12_06 = UNIT_END + 0x126,	//  Size: 12, Type: Int32, Flags: 1
        PLAYER_VISIBLE_ITEM_12_07 = UNIT_END + 0x127,	//  Size: 12, Type: Int32, Flags: 1
        PLAYER_VISIBLE_ITEM_12_08 = UNIT_END + 0x128,	//  Size: 12, Type: Int32, Flags: 1
        PLAYER_VISIBLE_ITEM_12_09 = UNIT_END + 0x129,	//  Size: 12, Type: Int32, Flags: 1
        PLAYER_VISIBLE_ITEM_12_10 = UNIT_END + 0x12A,	//  Size: 12, Type: Int32, Flags: 1
        PLAYER_VISIBLE_ITEM_12_11 = UNIT_END + 0x12B,	//  Size: 12, Type: Int32, Flags: 1
        PLAYER_VISIBLE_ITEM_12_PROPERTIES = UNIT_END + 0x12C,	//  Size: 1, Type: Chars?, Flags: 1
        PLAYER_VISIBLE_ITEM_12_PAD = UNIT_END + 0x12D,	//  Size: 1, Type: Int32, Flags: 1
        PLAYER_VISIBLE_ITEM_13_CREATOR = UNIT_END + 0x12E,	//  Size: 2, Type: GUID, Flags: 1
        PLAYER_VISIBLE_ITEM_13_0 = UNIT_END + 0x130,	//  Size: 12, Type: Int32, Flags: 1
        PLAYER_VISIBLE_ITEM_13_01 = UNIT_END + 0x131,	//  Size: 12, Type: Int32, Flags: 1
        PLAYER_VISIBLE_ITEM_13_02 = UNIT_END + 0x132,	//  Size: 12, Type: Int32, Flags: 1
        PLAYER_VISIBLE_ITEM_13_03 = UNIT_END + 0x133,	//  Size: 12, Type: Int32, Flags: 1
        PLAYER_VISIBLE_ITEM_13_04 = UNIT_END + 0x134,	//  Size: 12, Type: Int32, Flags: 1
        PLAYER_VISIBLE_ITEM_13_05 = UNIT_END + 0x135,	//  Size: 12, Type: Int32, Flags: 1
        PLAYER_VISIBLE_ITEM_13_06 = UNIT_END + 0x136,	//  Size: 12, Type: Int32, Flags: 1
        PLAYER_VISIBLE_ITEM_13_07 = UNIT_END + 0x137,	//  Size: 12, Type: Int32, Flags: 1
        PLAYER_VISIBLE_ITEM_13_08 = UNIT_END + 0x138,	//  Size: 12, Type: Int32, Flags: 1
        PLAYER_VISIBLE_ITEM_13_09 = UNIT_END + 0x139,	//  Size: 12, Type: Int32, Flags: 1
        PLAYER_VISIBLE_ITEM_13_10 = UNIT_END + 0x13A,	//  Size: 12, Type: Int32, Flags: 1
        PLAYER_VISIBLE_ITEM_13_11 = UNIT_END + 0x13B,	//  Size: 12, Type: Int32, Flags: 1
        PLAYER_VISIBLE_ITEM_13_PROPERTIES = UNIT_END + 0x13C,	//  Size: 1, Type: Chars?, Flags: 1
        PLAYER_VISIBLE_ITEM_13_PAD = UNIT_END + 0x13D,	//  Size: 1, Type: Int32, Flags: 1
        PLAYER_VISIBLE_ITEM_14_CREATOR = UNIT_END + 0x13E,	//  Size: 2, Type: GUID, Flags: 1
        PLAYER_VISIBLE_ITEM_14_0 = UNIT_END + 0x140,	//  Size: 12, Type: Int32, Flags: 1
        PLAYER_VISIBLE_ITEM_14_01 = UNIT_END + 0x141,	//  Size: 12, Type: Int32, Flags: 1
        PLAYER_VISIBLE_ITEM_14_02 = UNIT_END + 0x142,	//  Size: 12, Type: Int32, Flags: 1
        PLAYER_VISIBLE_ITEM_14_03 = UNIT_END + 0x143,	//  Size: 12, Type: Int32, Flags: 1
        PLAYER_VISIBLE_ITEM_14_04 = UNIT_END + 0x144,	//  Size: 12, Type: Int32, Flags: 1
        PLAYER_VISIBLE_ITEM_14_05 = UNIT_END + 0x145,	//  Size: 12, Type: Int32, Flags: 1
        PLAYER_VISIBLE_ITEM_14_06 = UNIT_END + 0x146,	//  Size: 12, Type: Int32, Flags: 1
        PLAYER_VISIBLE_ITEM_14_07 = UNIT_END + 0x147,	//  Size: 12, Type: Int32, Flags: 1
        PLAYER_VISIBLE_ITEM_14_08 = UNIT_END + 0x148,	//  Size: 12, Type: Int32, Flags: 1
        PLAYER_VISIBLE_ITEM_14_09 = UNIT_END + 0x149,	//  Size: 12, Type: Int32, Flags: 1
        PLAYER_VISIBLE_ITEM_14_10 = UNIT_END + 0x14A,	//  Size: 12, Type: Int32, Flags: 1
        PLAYER_VISIBLE_ITEM_14_11 = UNIT_END + 0x14B,	//  Size: 12, Type: Int32, Flags: 1
        PLAYER_VISIBLE_ITEM_14_PROPERTIES = UNIT_END + 0x14C,	//  Size: 1, Type: Chars?, Flags: 1
        PLAYER_VISIBLE_ITEM_14_PAD = UNIT_END + 0x14D,	//  Size: 1, Type: Int32, Flags: 1
        PLAYER_VISIBLE_ITEM_15_CREATOR = UNIT_END + 0x14E,	//  Size: 2, Type: GUID, Flags: 1
        PLAYER_VISIBLE_ITEM_15_0 = UNIT_END + 0x150,	//  Size: 12, Type: Int32, Flags: 1
        PLAYER_VISIBLE_ITEM_15_01 = UNIT_END + 0x151,	//  Size: 12, Type: Int32, Flags: 1
        PLAYER_VISIBLE_ITEM_15_02 = UNIT_END + 0x152,	//  Size: 12, Type: Int32, Flags: 1
        PLAYER_VISIBLE_ITEM_15_03 = UNIT_END + 0x153,	//  Size: 12, Type: Int32, Flags: 1
        PLAYER_VISIBLE_ITEM_15_04 = UNIT_END + 0x154,	//  Size: 12, Type: Int32, Flags: 1
        PLAYER_VISIBLE_ITEM_15_05 = UNIT_END + 0x155,	//  Size: 12, Type: Int32, Flags: 1
        PLAYER_VISIBLE_ITEM_15_06 = UNIT_END + 0x156,	//  Size: 12, Type: Int32, Flags: 1
        PLAYER_VISIBLE_ITEM_15_07 = UNIT_END + 0x157,	//  Size: 12, Type: Int32, Flags: 1
        PLAYER_VISIBLE_ITEM_15_08 = UNIT_END + 0x158,	//  Size: 12, Type: Int32, Flags: 1
        PLAYER_VISIBLE_ITEM_15_09 = UNIT_END + 0x159,	//  Size: 12, Type: Int32, Flags: 1
        PLAYER_VISIBLE_ITEM_15_10 = UNIT_END + 0x15A,	//  Size: 12, Type: Int32, Flags: 1
        PLAYER_VISIBLE_ITEM_15_11 = UNIT_END + 0x15B,	//  Size: 12, Type: Int32, Flags: 1
        PLAYER_VISIBLE_ITEM_15_PROPERTIES = UNIT_END + 0x15C,	//  Size: 1, Type: Chars?, Flags: 1
        PLAYER_VISIBLE_ITEM_15_PAD = UNIT_END + 0x15D,	//  Size: 1, Type: Int32, Flags: 1
        PLAYER_VISIBLE_ITEM_16_CREATOR = UNIT_END + 0x15E,	//  Size: 2, Type: GUID, Flags: 1
        PLAYER_VISIBLE_ITEM_16_0 = UNIT_END + 0x160,	//  Size: 12, Type: Int32, Flags: 1
        PLAYER_VISIBLE_ITEM_16_01 = UNIT_END + 0x161,	//  Size: 12, Type: Int32, Flags: 1
        PLAYER_VISIBLE_ITEM_16_02 = UNIT_END + 0x162,	//  Size: 12, Type: Int32, Flags: 1
        PLAYER_VISIBLE_ITEM_16_03 = UNIT_END + 0x163,	//  Size: 12, Type: Int32, Flags: 1
        PLAYER_VISIBLE_ITEM_16_04 = UNIT_END + 0x164,	//  Size: 12, Type: Int32, Flags: 1
        PLAYER_VISIBLE_ITEM_16_05 = UNIT_END + 0x165,	//  Size: 12, Type: Int32, Flags: 1
        PLAYER_VISIBLE_ITEM_16_06 = UNIT_END + 0x166,	//  Size: 12, Type: Int32, Flags: 1
        PLAYER_VISIBLE_ITEM_16_07 = UNIT_END + 0x167,	//  Size: 12, Type: Int32, Flags: 1
        PLAYER_VISIBLE_ITEM_16_08 = UNIT_END + 0x168,	//  Size: 12, Type: Int32, Flags: 1
        PLAYER_VISIBLE_ITEM_16_09 = UNIT_END + 0x169,	//  Size: 12, Type: Int32, Flags: 1
        PLAYER_VISIBLE_ITEM_16_10 = UNIT_END + 0x16A,	//  Size: 12, Type: Int32, Flags: 1
        PLAYER_VISIBLE_ITEM_16_11 = UNIT_END + 0x16B,	//  Size: 12, Type: Int32, Flags: 1
        PLAYER_VISIBLE_ITEM_16_PROPERTIES = UNIT_END + 0x16C,	//  Size: 1, Type: Chars?, Flags: 1
        PLAYER_VISIBLE_ITEM_16_PAD = UNIT_END + 0x16D,	//  Size: 1, Type: Int32, Flags: 1
        PLAYER_VISIBLE_ITEM_17_CREATOR = UNIT_END + 0x16E,	//  Size: 2, Type: GUID, Flags: 1
        PLAYER_VISIBLE_ITEM_17_0 = UNIT_END + 0x170,	//  Size: 12, Type: Int32, Flags: 1
        PLAYER_VISIBLE_ITEM_17_01 = UNIT_END + 0x171,	//  Size: 12, Type: Int32, Flags: 1
        PLAYER_VISIBLE_ITEM_17_02 = UNIT_END + 0x172,	//  Size: 12, Type: Int32, Flags: 1
        PLAYER_VISIBLE_ITEM_17_03 = UNIT_END + 0x173,	//  Size: 12, Type: Int32, Flags: 1
        PLAYER_VISIBLE_ITEM_17_04 = UNIT_END + 0x174,	//  Size: 12, Type: Int32, Flags: 1
        PLAYER_VISIBLE_ITEM_17_05 = UNIT_END + 0x175,	//  Size: 12, Type: Int32, Flags: 1
        PLAYER_VISIBLE_ITEM_17_06 = UNIT_END + 0x176,	//  Size: 12, Type: Int32, Flags: 1
        PLAYER_VISIBLE_ITEM_17_07 = UNIT_END + 0x177,	//  Size: 12, Type: Int32, Flags: 1
        PLAYER_VISIBLE_ITEM_17_08 = UNIT_END + 0x178,	//  Size: 12, Type: Int32, Flags: 1
        PLAYER_VISIBLE_ITEM_17_09 = UNIT_END + 0x179,	//  Size: 12, Type: Int32, Flags: 1
        PLAYER_VISIBLE_ITEM_17_10 = UNIT_END + 0x17A,	//  Size: 12, Type: Int32, Flags: 1
        PLAYER_VISIBLE_ITEM_17_11 = UNIT_END + 0x17B,	//  Size: 12, Type: Int32, Flags: 1
        PLAYER_VISIBLE_ITEM_17_PROPERTIES = UNIT_END + 0x17C,	//  Size: 1, Type: Chars?, Flags: 1
        PLAYER_VISIBLE_ITEM_17_PAD = UNIT_END + 0x17D,	//  Size: 1, Type: Int32, Flags: 1
        PLAYER_VISIBLE_ITEM_18_CREATOR = UNIT_END + 0x17E,	//  Size: 2, Type: GUID, Flags: 1
        PLAYER_VISIBLE_ITEM_18_0 = UNIT_END + 0x180,	//  Size: 12, Type: Int32, Flags: 1
        PLAYER_VISIBLE_ITEM_18_01 = UNIT_END + 0x181,	//  Size: 12, Type: Int32, Flags: 1
        PLAYER_VISIBLE_ITEM_18_02 = UNIT_END + 0x182,	//  Size: 12, Type: Int32, Flags: 1
        PLAYER_VISIBLE_ITEM_18_03 = UNIT_END + 0x183,	//  Size: 12, Type: Int32, Flags: 1
        PLAYER_VISIBLE_ITEM_18_04 = UNIT_END + 0x184,	//  Size: 12, Type: Int32, Flags: 1
        PLAYER_VISIBLE_ITEM_18_05 = UNIT_END + 0x185,	//  Size: 12, Type: Int32, Flags: 1
        PLAYER_VISIBLE_ITEM_18_06 = UNIT_END + 0x186,	//  Size: 12, Type: Int32, Flags: 1
        PLAYER_VISIBLE_ITEM_18_07 = UNIT_END + 0x187,	//  Size: 12, Type: Int32, Flags: 1
        PLAYER_VISIBLE_ITEM_18_08 = UNIT_END + 0x188,	//  Size: 12, Type: Int32, Flags: 1
        PLAYER_VISIBLE_ITEM_18_09 = UNIT_END + 0x189,	//  Size: 12, Type: Int32, Flags: 1
        PLAYER_VISIBLE_ITEM_18_10 = UNIT_END + 0x18A,	//  Size: 12, Type: Int32, Flags: 1
        PLAYER_VISIBLE_ITEM_18_11 = UNIT_END + 0x18B,	//  Size: 12, Type: Int32, Flags: 1
        PLAYER_VISIBLE_ITEM_18_PROPERTIES = UNIT_END + 0x18C,	//  Size: 1, Type: Chars?, Flags: 1
        PLAYER_VISIBLE_ITEM_18_PAD = UNIT_END + 0x18D,	//  Size: 1, Type: Int32, Flags: 1
        PLAYER_VISIBLE_ITEM_19_CREATOR = UNIT_END + 0x18E,	//  Size: 2, Type: GUID, Flags: 1
        PLAYER_VISIBLE_ITEM_19_0 = UNIT_END + 0x190,	//  Size: 12, Type: Int32, Flags: 1
        PLAYER_VISIBLE_ITEM_19_01 = UNIT_END + 0x191,	//  Size: 12, Type: Int32, Flags: 1
        PLAYER_VISIBLE_ITEM_19_02 = UNIT_END + 0x192,	//  Size: 12, Type: Int32, Flags: 1
        PLAYER_VISIBLE_ITEM_19_03 = UNIT_END + 0x193,	//  Size: 12, Type: Int32, Flags: 1
        PLAYER_VISIBLE_ITEM_19_04 = UNIT_END + 0x194,	//  Size: 12, Type: Int32, Flags: 1
        PLAYER_VISIBLE_ITEM_19_05 = UNIT_END + 0x195,	//  Size: 12, Type: Int32, Flags: 1
        PLAYER_VISIBLE_ITEM_19_06 = UNIT_END + 0x196,	//  Size: 12, Type: Int32, Flags: 1
        PLAYER_VISIBLE_ITEM_19_07 = UNIT_END + 0x197,	//  Size: 12, Type: Int32, Flags: 1
        PLAYER_VISIBLE_ITEM_19_08 = UNIT_END + 0x198,	//  Size: 12, Type: Int32, Flags: 1
        PLAYER_VISIBLE_ITEM_19_09 = UNIT_END + 0x199,	//  Size: 12, Type: Int32, Flags: 1
        PLAYER_VISIBLE_ITEM_19_10 = UNIT_END + 0x19A,	//  Size: 12, Type: Int32, Flags: 1
        PLAYER_VISIBLE_ITEM_19_11 = UNIT_END + 0x19B,	//  Size: 12, Type: Int32, Flags: 1
        PLAYER_VISIBLE_ITEM_19_PROPERTIES = UNIT_END + 0x19C,	//  Size: 1, Type: Chars?, Flags: 1
        PLAYER_VISIBLE_ITEM_19_PAD = UNIT_END + 0x19D,	//  Size: 1, Type: Int32, Flags: 1
        PLAYER_CHOSEN_TITLE = UNIT_END + 0x19E,	//  Size: 1, Type: Int32, Flags: 1
        PLAYER_FIELD_PAD_0 = UNIT_END + 0x19F,	//  Size: 1, Type: Int32, Flags: 0
        PLAYER_FIELD_INV_SLOT_HEAD = UNIT_END + 0x1A0,	//  Size: 46, Type: GUID, Flags: 2
        PLAYER_FIELD_PACK_SLOT_1 = UNIT_END + 0x1CE,	//  Size: 32, Type: GUID, Flags: 2
        PLAYER_FIELD_BANK_SLOT_1 = UNIT_END + 0x1EE,	//  Size: 56, Type: GUID, Flags: 2
        PLAYER_FIELD_BANKBAG_SLOT_1 = UNIT_END + 0x226,	//  Size: 14, Type: GUID, Flags: 2
        PLAYER_FIELD_VENDORBUYBACK_SLOT_1 = UNIT_END + 0x234,	//  Size: 24, Type: GUID, Flags: 2
        PLAYER_FIELD_KEYRING_SLOT_1 = UNIT_END + 0x24C,	//  Size: 64, Type: GUID, Flags: 2
        PLAYER_FIELD_VANITYPET_SLOT_1 = UNIT_END + 0x28C,	//  Size: 36, Type: GUID, Flags: 2
        PLAYER_FARSIGHT = UNIT_END + 0x2B0,	//  Size: 2, Type: GUID, Flags: 2
        PLAYER__FIELD_KNOWN_TITLES = UNIT_END + 0x2B2,	//  Size: 2, Type: GUID, Flags: 2
        PLAYER_XP = UNIT_END + 0x2B4,	//  Size: 1, Type: Int32, Flags: 2
        PLAYER_NEXT_LEVEL_XP = UNIT_END + 0x2B5,	//  Size: 1, Type: Int32, Flags: 2
        PLAYER_SKILL_INFO_1_1 = UNIT_END + 0x2B6,	//  Size: 384, Type: Chars?, Flags: 2
        PLAYER_SKILL_INFO_1_01 = UNIT_END + 0x2B7,	//  Size: 384, Type: Chars?, Flags: 2
        PLAYER_SKILL_INFO_1_02 = UNIT_END + 0x2B8,	//  Size: 384, Type: Chars?, Flags: 2
        PLAYER_SKILL_INFO_1_03 = UNIT_END + 0x2B9,	//  Size: 384, Type: Chars?, Flags: 2
        PLAYER_SKILL_INFO_1_04 = UNIT_END + 0x2BA,	//  Size: 384, Type: Chars?, Flags: 2
        PLAYER_SKILL_INFO_1_05 = UNIT_END + 0x2BB,	//  Size: 384, Type: Chars?, Flags: 2
        PLAYER_SKILL_INFO_1_06 = UNIT_END + 0x2BC,	//  Size: 384, Type: Chars?, Flags: 2
        PLAYER_SKILL_INFO_1_07 = UNIT_END + 0x2BD,	//  Size: 384, Type: Chars?, Flags: 2
        PLAYER_SKILL_INFO_1_08 = UNIT_END + 0x2BE,	//  Size: 384, Type: Chars?, Flags: 2
        PLAYER_SKILL_INFO_1_09 = UNIT_END + 0x2BF,	//  Size: 384, Type: Chars?, Flags: 2
        PLAYER_SKILL_INFO_1_10 = UNIT_END + 0x2C0,	//  Size: 384, Type: Chars?, Flags: 2
        PLAYER_SKILL_INFO_1_11 = UNIT_END + 0x2C1,	//  Size: 384, Type: Chars?, Flags: 2
        PLAYER_SKILL_INFO_1_12 = UNIT_END + 0x2C2,	//  Size: 384, Type: Chars?, Flags: 2
        PLAYER_SKILL_INFO_1_13 = UNIT_END + 0x2C3,	//  Size: 384, Type: Chars?, Flags: 2
        PLAYER_SKILL_INFO_1_14 = UNIT_END + 0x2C4,	//  Size: 384, Type: Chars?, Flags: 2
        PLAYER_SKILL_INFO_1_15 = UNIT_END + 0x2C5,	//  Size: 384, Type: Chars?, Flags: 2
        PLAYER_SKILL_INFO_1_16 = UNIT_END + 0x2C6,	//  Size: 384, Type: Chars?, Flags: 2
        PLAYER_SKILL_INFO_1_17 = UNIT_END + 0x2C7,	//  Size: 384, Type: Chars?, Flags: 2
        PLAYER_SKILL_INFO_1_18 = UNIT_END + 0x2C8,	//  Size: 384, Type: Chars?, Flags: 2
        PLAYER_SKILL_INFO_1_19 = UNIT_END + 0x2C9,	//  Size: 384, Type: Chars?, Flags: 2
        PLAYER_SKILL_INFO_1_20 = UNIT_END + 0x2CA,	//  Size: 384, Type: Chars?, Flags: 2
        PLAYER_SKILL_INFO_1_21 = UNIT_END + 0x2CB,	//  Size: 384, Type: Chars?, Flags: 2
        PLAYER_SKILL_INFO_1_22 = UNIT_END + 0x2CC,	//  Size: 384, Type: Chars?, Flags: 2
        PLAYER_SKILL_INFO_1_23 = UNIT_END + 0x2CD,	//  Size: 384, Type: Chars?, Flags: 2
        PLAYER_SKILL_INFO_1_24 = UNIT_END + 0x2CE,	//  Size: 384, Type: Chars?, Flags: 2
        PLAYER_SKILL_INFO_1_25 = UNIT_END + 0x2CF,	//  Size: 384, Type: Chars?, Flags: 2
        PLAYER_SKILL_INFO_1_26 = UNIT_END + 0x2D0,	//  Size: 384, Type: Chars?, Flags: 2
        PLAYER_SKILL_INFO_1_27 = UNIT_END + 0x2D1,	//  Size: 384, Type: Chars?, Flags: 2
        PLAYER_SKILL_INFO_1_28 = UNIT_END + 0x2D2,	//  Size: 384, Type: Chars?, Flags: 2
        PLAYER_SKILL_INFO_1_29 = UNIT_END + 0x2D3,	//  Size: 384, Type: Chars?, Flags: 2
        PLAYER_SKILL_INFO_1_30 = UNIT_END + 0x2D4,	//  Size: 384, Type: Chars?, Flags: 2
        PLAYER_SKILL_INFO_1_31 = UNIT_END + 0x2D5,	//  Size: 384, Type: Chars?, Flags: 2
        PLAYER_SKILL_INFO_1_32 = UNIT_END + 0x2D6,	//  Size: 384, Type: Chars?, Flags: 2
        PLAYER_SKILL_INFO_1_33 = UNIT_END + 0x2D7,	//  Size: 384, Type: Chars?, Flags: 2
        PLAYER_SKILL_INFO_1_34 = UNIT_END + 0x2D8,	//  Size: 384, Type: Chars?, Flags: 2
        PLAYER_SKILL_INFO_1_35 = UNIT_END + 0x2D9,	//  Size: 384, Type: Chars?, Flags: 2
        PLAYER_SKILL_INFO_1_36 = UNIT_END + 0x2DA,	//  Size: 384, Type: Chars?, Flags: 2
        PLAYER_SKILL_INFO_1_37 = UNIT_END + 0x2DB,	//  Size: 384, Type: Chars?, Flags: 2
        PLAYER_SKILL_INFO_1_38 = UNIT_END + 0x2DC,	//  Size: 384, Type: Chars?, Flags: 2
        PLAYER_SKILL_INFO_1_39 = UNIT_END + 0x2DD,	//  Size: 384, Type: Chars?, Flags: 2
        PLAYER_SKILL_INFO_1_40 = UNIT_END + 0x2DE,	//  Size: 384, Type: Chars?, Flags: 2
        PLAYER_SKILL_INFO_1_41 = UNIT_END + 0x2DF,	//  Size: 384, Type: Chars?, Flags: 2
        PLAYER_SKILL_INFO_1_42 = UNIT_END + 0x2E0,	//  Size: 384, Type: Chars?, Flags: 2
        PLAYER_SKILL_INFO_1_43 = UNIT_END + 0x2E1,	//  Size: 384, Type: Chars?, Flags: 2
        PLAYER_SKILL_INFO_1_44 = UNIT_END + 0x2E2,	//  Size: 384, Type: Chars?, Flags: 2
        PLAYER_SKILL_INFO_1_45 = UNIT_END + 0x2E3,	//  Size: 384, Type: Chars?, Flags: 2
        PLAYER_SKILL_INFO_1_46 = UNIT_END + 0x2E4,	//  Size: 384, Type: Chars?, Flags: 2
        PLAYER_SKILL_INFO_1_47 = UNIT_END + 0x2E5,	//  Size: 384, Type: Chars?, Flags: 2
        PLAYER_SKILL_INFO_1_48 = UNIT_END + 0x2E6,	//  Size: 384, Type: Chars?, Flags: 2
        PLAYER_SKILL_INFO_1_49 = UNIT_END + 0x2E7,	//  Size: 384, Type: Chars?, Flags: 2
        PLAYER_SKILL_INFO_1_50 = UNIT_END + 0x2E8,	//  Size: 384, Type: Chars?, Flags: 2
        PLAYER_SKILL_INFO_1_51 = UNIT_END + 0x2E9,	//  Size: 384, Type: Chars?, Flags: 2
        PLAYER_SKILL_INFO_1_52 = UNIT_END + 0x2EA,	//  Size: 384, Type: Chars?, Flags: 2
        PLAYER_SKILL_INFO_1_53 = UNIT_END + 0x2EB,	//  Size: 384, Type: Chars?, Flags: 2
        PLAYER_SKILL_INFO_1_54 = UNIT_END + 0x2EC,	//  Size: 384, Type: Chars?, Flags: 2
        PLAYER_SKILL_INFO_1_55 = UNIT_END + 0x2ED,	//  Size: 384, Type: Chars?, Flags: 2
        PLAYER_SKILL_INFO_1_56 = UNIT_END + 0x2EE,	//  Size: 384, Type: Chars?, Flags: 2
        PLAYER_SKILL_INFO_1_57 = UNIT_END + 0x2EF,	//  Size: 384, Type: Chars?, Flags: 2
        PLAYER_SKILL_INFO_1_58 = UNIT_END + 0x2F0,	//  Size: 384, Type: Chars?, Flags: 2
        PLAYER_SKILL_INFO_1_59 = UNIT_END + 0x2F1,	//  Size: 384, Type: Chars?, Flags: 2
        PLAYER_SKILL_INFO_1_60 = UNIT_END + 0x2F2,	//  Size: 384, Type: Chars?, Flags: 2
        PLAYER_SKILL_INFO_1_61 = UNIT_END + 0x2F3,	//  Size: 384, Type: Chars?, Flags: 2
        PLAYER_SKILL_INFO_1_62 = UNIT_END + 0x2F4,	//  Size: 384, Type: Chars?, Flags: 2
        PLAYER_SKILL_INFO_1_63 = UNIT_END + 0x2F5,	//  Size: 384, Type: Chars?, Flags: 2
        PLAYER_SKILL_INFO_1_64 = UNIT_END + 0x2F6,	//  Size: 384, Type: Chars?, Flags: 2
        PLAYER_SKILL_INFO_1_65 = UNIT_END + 0x2F7,	//  Size: 384, Type: Chars?, Flags: 2
        PLAYER_SKILL_INFO_1_66 = UNIT_END + 0x2F8,	//  Size: 384, Type: Chars?, Flags: 2
        PLAYER_SKILL_INFO_1_67 = UNIT_END + 0x2F9,	//  Size: 384, Type: Chars?, Flags: 2
        PLAYER_SKILL_INFO_1_68 = UNIT_END + 0x2FA,	//  Size: 384, Type: Chars?, Flags: 2
        PLAYER_SKILL_INFO_1_69 = UNIT_END + 0x2FB,	//  Size: 384, Type: Chars?, Flags: 2
        PLAYER_SKILL_INFO_1_70 = UNIT_END + 0x2FC,	//  Size: 384, Type: Chars?, Flags: 2
        PLAYER_SKILL_INFO_1_71 = UNIT_END + 0x2FD,	//  Size: 384, Type: Chars?, Flags: 2
        PLAYER_SKILL_INFO_1_72 = UNIT_END + 0x2FE,	//  Size: 384, Type: Chars?, Flags: 2
        PLAYER_SKILL_INFO_1_73 = UNIT_END + 0x2FF,	//  Size: 384, Type: Chars?, Flags: 2
        PLAYER_SKILL_INFO_1_74 = UNIT_END + 0x300,	//  Size: 384, Type: Chars?, Flags: 2
        PLAYER_SKILL_INFO_1_75 = UNIT_END + 0x301,	//  Size: 384, Type: Chars?, Flags: 2
        PLAYER_SKILL_INFO_1_76 = UNIT_END + 0x302,	//  Size: 384, Type: Chars?, Flags: 2
        PLAYER_SKILL_INFO_1_77 = UNIT_END + 0x303,	//  Size: 384, Type: Chars?, Flags: 2
        PLAYER_SKILL_INFO_1_78 = UNIT_END + 0x304,	//  Size: 384, Type: Chars?, Flags: 2
        PLAYER_SKILL_INFO_1_79 = UNIT_END + 0x305,	//  Size: 384, Type: Chars?, Flags: 2
        PLAYER_SKILL_INFO_1_80 = UNIT_END + 0x306,	//  Size: 384, Type: Chars?, Flags: 2
        PLAYER_SKILL_INFO_1_81 = UNIT_END + 0x307,	//  Size: 384, Type: Chars?, Flags: 2
        PLAYER_SKILL_INFO_1_82 = UNIT_END + 0x308,	//  Size: 384, Type: Chars?, Flags: 2
        PLAYER_SKILL_INFO_1_83 = UNIT_END + 0x309,	//  Size: 384, Type: Chars?, Flags: 2
        PLAYER_SKILL_INFO_1_84 = UNIT_END + 0x30A,	//  Size: 384, Type: Chars?, Flags: 2
        PLAYER_SKILL_INFO_1_85 = UNIT_END + 0x30B,	//  Size: 384, Type: Chars?, Flags: 2
        PLAYER_SKILL_INFO_1_86 = UNIT_END + 0x30C,	//  Size: 384, Type: Chars?, Flags: 2
        PLAYER_SKILL_INFO_1_87 = UNIT_END + 0x30D,	//  Size: 384, Type: Chars?, Flags: 2
        PLAYER_SKILL_INFO_1_88 = UNIT_END + 0x30E,	//  Size: 384, Type: Chars?, Flags: 2
        PLAYER_SKILL_INFO_1_89 = UNIT_END + 0x30F,	//  Size: 384, Type: Chars?, Flags: 2
        PLAYER_SKILL_INFO_1_90 = UNIT_END + 0x310,	//  Size: 384, Type: Chars?, Flags: 2
        PLAYER_SKILL_INFO_1_91 = UNIT_END + 0x311,	//  Size: 384, Type: Chars?, Flags: 2
        PLAYER_SKILL_INFO_1_92 = UNIT_END + 0x312,	//  Size: 384, Type: Chars?, Flags: 2
        PLAYER_SKILL_INFO_1_93 = UNIT_END + 0x313,	//  Size: 384, Type: Chars?, Flags: 2
        PLAYER_SKILL_INFO_1_94 = UNIT_END + 0x314,	//  Size: 384, Type: Chars?, Flags: 2
        PLAYER_SKILL_INFO_1_95 = UNIT_END + 0x315,	//  Size: 384, Type: Chars?, Flags: 2
        PLAYER_SKILL_INFO_1_96 = UNIT_END + 0x316,	//  Size: 384, Type: Chars?, Flags: 2
        PLAYER_SKILL_INFO_1_97 = UNIT_END + 0x317,	//  Size: 384, Type: Chars?, Flags: 2
        PLAYER_SKILL_INFO_1_98 = UNIT_END + 0x318,	//  Size: 384, Type: Chars?, Flags: 2
        PLAYER_SKILL_INFO_1_99 = UNIT_END + 0x319,	//  Size: 384, Type: Chars?, Flags: 2
        PLAYER_SKILL_INFO_1_100 = UNIT_END + 0x31A,	//  Size: 384, Type: Chars?, Flags: 2
        PLAYER_SKILL_INFO_1_101 = UNIT_END + 0x31B,	//  Size: 384, Type: Chars?, Flags: 2
        PLAYER_SKILL_INFO_1_102 = UNIT_END + 0x31C,	//  Size: 384, Type: Chars?, Flags: 2
        PLAYER_SKILL_INFO_1_103 = UNIT_END + 0x31D,	//  Size: 384, Type: Chars?, Flags: 2
        PLAYER_SKILL_INFO_1_104 = UNIT_END + 0x31E,	//  Size: 384, Type: Chars?, Flags: 2
        PLAYER_SKILL_INFO_1_105 = UNIT_END + 0x31F,	//  Size: 384, Type: Chars?, Flags: 2
        PLAYER_SKILL_INFO_1_106 = UNIT_END + 0x320,	//  Size: 384, Type: Chars?, Flags: 2
        PLAYER_SKILL_INFO_1_107 = UNIT_END + 0x321,	//  Size: 384, Type: Chars?, Flags: 2
        PLAYER_SKILL_INFO_1_108 = UNIT_END + 0x322,	//  Size: 384, Type: Chars?, Flags: 2
        PLAYER_SKILL_INFO_1_109 = UNIT_END + 0x323,	//  Size: 384, Type: Chars?, Flags: 2
        PLAYER_SKILL_INFO_1_110 = UNIT_END + 0x324,	//  Size: 384, Type: Chars?, Flags: 2
        PLAYER_SKILL_INFO_1_111 = UNIT_END + 0x325,	//  Size: 384, Type: Chars?, Flags: 2
        PLAYER_SKILL_INFO_1_112 = UNIT_END + 0x326,	//  Size: 384, Type: Chars?, Flags: 2
        PLAYER_SKILL_INFO_1_113 = UNIT_END + 0x327,	//  Size: 384, Type: Chars?, Flags: 2
        PLAYER_SKILL_INFO_1_114 = UNIT_END + 0x328,	//  Size: 384, Type: Chars?, Flags: 2
        PLAYER_SKILL_INFO_1_115 = UNIT_END + 0x329,	//  Size: 384, Type: Chars?, Flags: 2
        PLAYER_SKILL_INFO_1_116 = UNIT_END + 0x32A,	//  Size: 384, Type: Chars?, Flags: 2
        PLAYER_SKILL_INFO_1_117 = UNIT_END + 0x32B,	//  Size: 384, Type: Chars?, Flags: 2
        PLAYER_SKILL_INFO_1_118 = UNIT_END + 0x32C,	//  Size: 384, Type: Chars?, Flags: 2
        PLAYER_SKILL_INFO_1_119 = UNIT_END + 0x32D,	//  Size: 384, Type: Chars?, Flags: 2
        PLAYER_SKILL_INFO_1_120 = UNIT_END + 0x32E,	//  Size: 384, Type: Chars?, Flags: 2
        PLAYER_SKILL_INFO_1_121 = UNIT_END + 0x32F,	//  Size: 384, Type: Chars?, Flags: 2
        PLAYER_SKILL_INFO_1_122 = UNIT_END + 0x330,	//  Size: 384, Type: Chars?, Flags: 2
        PLAYER_SKILL_INFO_1_123 = UNIT_END + 0x331,	//  Size: 384, Type: Chars?, Flags: 2
        PLAYER_SKILL_INFO_1_124 = UNIT_END + 0x332,	//  Size: 384, Type: Chars?, Flags: 2
        PLAYER_SKILL_INFO_1_125 = UNIT_END + 0x333,	//  Size: 384, Type: Chars?, Flags: 2
        PLAYER_SKILL_INFO_1_126 = UNIT_END + 0x334,	//  Size: 384, Type: Chars?, Flags: 2
        PLAYER_SKILL_INFO_1_127 = UNIT_END + 0x335,	//  Size: 384, Type: Chars?, Flags: 2
        PLAYER_SKILL_INFO_1_128 = UNIT_END + 0x336,	//  Size: 384, Type: Chars?, Flags: 2
        PLAYER_SKILL_INFO_1_129 = UNIT_END + 0x337,	//  Size: 384, Type: Chars?, Flags: 2
        PLAYER_SKILL_INFO_1_130 = UNIT_END + 0x338,	//  Size: 384, Type: Chars?, Flags: 2
        PLAYER_SKILL_INFO_1_131 = UNIT_END + 0x339,	//  Size: 384, Type: Chars?, Flags: 2
        PLAYER_SKILL_INFO_1_132 = UNIT_END + 0x33A,	//  Size: 384, Type: Chars?, Flags: 2
        PLAYER_SKILL_INFO_1_133 = UNIT_END + 0x33B,	//  Size: 384, Type: Chars?, Flags: 2
        PLAYER_SKILL_INFO_1_134 = UNIT_END + 0x33C,	//  Size: 384, Type: Chars?, Flags: 2
        PLAYER_SKILL_INFO_1_135 = UNIT_END + 0x33D,	//  Size: 384, Type: Chars?, Flags: 2
        PLAYER_SKILL_INFO_1_136 = UNIT_END + 0x33E,	//  Size: 384, Type: Chars?, Flags: 2
        PLAYER_SKILL_INFO_1_137 = UNIT_END + 0x33F,	//  Size: 384, Type: Chars?, Flags: 2
        PLAYER_SKILL_INFO_1_138 = UNIT_END + 0x340,	//  Size: 384, Type: Chars?, Flags: 2
        PLAYER_SKILL_INFO_1_139 = UNIT_END + 0x341,	//  Size: 384, Type: Chars?, Flags: 2
        PLAYER_SKILL_INFO_1_140 = UNIT_END + 0x342,	//  Size: 384, Type: Chars?, Flags: 2
        PLAYER_SKILL_INFO_1_141 = UNIT_END + 0x343,	//  Size: 384, Type: Chars?, Flags: 2
        PLAYER_SKILL_INFO_1_142 = UNIT_END + 0x344,	//  Size: 384, Type: Chars?, Flags: 2
        PLAYER_SKILL_INFO_1_143 = UNIT_END + 0x345,	//  Size: 384, Type: Chars?, Flags: 2
        PLAYER_SKILL_INFO_1_144 = UNIT_END + 0x346,	//  Size: 384, Type: Chars?, Flags: 2
        PLAYER_SKILL_INFO_1_145 = UNIT_END + 0x347,	//  Size: 384, Type: Chars?, Flags: 2
        PLAYER_SKILL_INFO_1_146 = UNIT_END + 0x348,	//  Size: 384, Type: Chars?, Flags: 2
        PLAYER_SKILL_INFO_1_147 = UNIT_END + 0x349,	//  Size: 384, Type: Chars?, Flags: 2
        PLAYER_SKILL_INFO_1_148 = UNIT_END + 0x34A,	//  Size: 384, Type: Chars?, Flags: 2
        PLAYER_SKILL_INFO_1_149 = UNIT_END + 0x34B,	//  Size: 384, Type: Chars?, Flags: 2
        PLAYER_SKILL_INFO_1_150 = UNIT_END + 0x34C,	//  Size: 384, Type: Chars?, Flags: 2
        PLAYER_SKILL_INFO_1_151 = UNIT_END + 0x34D,	//  Size: 384, Type: Chars?, Flags: 2
        PLAYER_SKILL_INFO_1_152 = UNIT_END + 0x34E,	//  Size: 384, Type: Chars?, Flags: 2
        PLAYER_SKILL_INFO_1_153 = UNIT_END + 0x34F,	//  Size: 384, Type: Chars?, Flags: 2
        PLAYER_SKILL_INFO_1_154 = UNIT_END + 0x350,	//  Size: 384, Type: Chars?, Flags: 2
        PLAYER_SKILL_INFO_1_155 = UNIT_END + 0x351,	//  Size: 384, Type: Chars?, Flags: 2
        PLAYER_SKILL_INFO_1_156 = UNIT_END + 0x352,	//  Size: 384, Type: Chars?, Flags: 2
        PLAYER_SKILL_INFO_1_157 = UNIT_END + 0x353,	//  Size: 384, Type: Chars?, Flags: 2
        PLAYER_SKILL_INFO_1_158 = UNIT_END + 0x354,	//  Size: 384, Type: Chars?, Flags: 2
        PLAYER_SKILL_INFO_1_159 = UNIT_END + 0x355,	//  Size: 384, Type: Chars?, Flags: 2
        PLAYER_SKILL_INFO_1_160 = UNIT_END + 0x356,	//  Size: 384, Type: Chars?, Flags: 2
        PLAYER_SKILL_INFO_1_161 = UNIT_END + 0x357,	//  Size: 384, Type: Chars?, Flags: 2
        PLAYER_SKILL_INFO_1_162 = UNIT_END + 0x358,	//  Size: 384, Type: Chars?, Flags: 2
        PLAYER_SKILL_INFO_1_163 = UNIT_END + 0x359,	//  Size: 384, Type: Chars?, Flags: 2
        PLAYER_SKILL_INFO_1_164 = UNIT_END + 0x35A,	//  Size: 384, Type: Chars?, Flags: 2
        PLAYER_SKILL_INFO_1_165 = UNIT_END + 0x35B,	//  Size: 384, Type: Chars?, Flags: 2
        PLAYER_SKILL_INFO_1_166 = UNIT_END + 0x35C,	//  Size: 384, Type: Chars?, Flags: 2
        PLAYER_SKILL_INFO_1_167 = UNIT_END + 0x35D,	//  Size: 384, Type: Chars?, Flags: 2
        PLAYER_SKILL_INFO_1_168 = UNIT_END + 0x35E,	//  Size: 384, Type: Chars?, Flags: 2
        PLAYER_SKILL_INFO_1_169 = UNIT_END + 0x35F,	//  Size: 384, Type: Chars?, Flags: 2
        PLAYER_SKILL_INFO_1_170 = UNIT_END + 0x360,	//  Size: 384, Type: Chars?, Flags: 2
        PLAYER_SKILL_INFO_1_171 = UNIT_END + 0x361,	//  Size: 384, Type: Chars?, Flags: 2
        PLAYER_SKILL_INFO_1_172 = UNIT_END + 0x362,	//  Size: 384, Type: Chars?, Flags: 2
        PLAYER_SKILL_INFO_1_173 = UNIT_END + 0x363,	//  Size: 384, Type: Chars?, Flags: 2
        PLAYER_SKILL_INFO_1_174 = UNIT_END + 0x364,	//  Size: 384, Type: Chars?, Flags: 2
        PLAYER_SKILL_INFO_1_175 = UNIT_END + 0x365,	//  Size: 384, Type: Chars?, Flags: 2
        PLAYER_SKILL_INFO_1_176 = UNIT_END + 0x366,	//  Size: 384, Type: Chars?, Flags: 2
        PLAYER_SKILL_INFO_1_177 = UNIT_END + 0x367,	//  Size: 384, Type: Chars?, Flags: 2
        PLAYER_SKILL_INFO_1_178 = UNIT_END + 0x368,	//  Size: 384, Type: Chars?, Flags: 2
        PLAYER_SKILL_INFO_1_179 = UNIT_END + 0x369,	//  Size: 384, Type: Chars?, Flags: 2
        PLAYER_SKILL_INFO_1_180 = UNIT_END + 0x36A,	//  Size: 384, Type: Chars?, Flags: 2
        PLAYER_SKILL_INFO_1_181 = UNIT_END + 0x36B,	//  Size: 384, Type: Chars?, Flags: 2
        PLAYER_SKILL_INFO_1_182 = UNIT_END + 0x36C,	//  Size: 384, Type: Chars?, Flags: 2
        PLAYER_SKILL_INFO_1_183 = UNIT_END + 0x36D,	//  Size: 384, Type: Chars?, Flags: 2
        PLAYER_SKILL_INFO_1_184 = UNIT_END + 0x36E,	//  Size: 384, Type: Chars?, Flags: 2
        PLAYER_SKILL_INFO_1_185 = UNIT_END + 0x36F,	//  Size: 384, Type: Chars?, Flags: 2
        PLAYER_SKILL_INFO_1_186 = UNIT_END + 0x370,	//  Size: 384, Type: Chars?, Flags: 2
        PLAYER_SKILL_INFO_1_187 = UNIT_END + 0x371,	//  Size: 384, Type: Chars?, Flags: 2
        PLAYER_SKILL_INFO_1_188 = UNIT_END + 0x372,	//  Size: 384, Type: Chars?, Flags: 2
        PLAYER_SKILL_INFO_1_189 = UNIT_END + 0x373,	//  Size: 384, Type: Chars?, Flags: 2
        PLAYER_SKILL_INFO_1_190 = UNIT_END + 0x374,	//  Size: 384, Type: Chars?, Flags: 2
        PLAYER_SKILL_INFO_1_191 = UNIT_END + 0x375,	//  Size: 384, Type: Chars?, Flags: 2
        PLAYER_SKILL_INFO_1_192 = UNIT_END + 0x376,	//  Size: 384, Type: Chars?, Flags: 2
        PLAYER_SKILL_INFO_1_193 = UNIT_END + 0x377,	//  Size: 384, Type: Chars?, Flags: 2
        PLAYER_SKILL_INFO_1_194 = UNIT_END + 0x378,	//  Size: 384, Type: Chars?, Flags: 2
        PLAYER_SKILL_INFO_1_195 = UNIT_END + 0x379,	//  Size: 384, Type: Chars?, Flags: 2
        PLAYER_SKILL_INFO_1_196 = UNIT_END + 0x37A,	//  Size: 384, Type: Chars?, Flags: 2
        PLAYER_SKILL_INFO_1_197 = UNIT_END + 0x37B,	//  Size: 384, Type: Chars?, Flags: 2
        PLAYER_SKILL_INFO_1_198 = UNIT_END + 0x37C,	//  Size: 384, Type: Chars?, Flags: 2
        PLAYER_SKILL_INFO_1_199 = UNIT_END + 0x37D,	//  Size: 384, Type: Chars?, Flags: 2
        PLAYER_SKILL_INFO_1_200 = UNIT_END + 0x37E,	//  Size: 384, Type: Chars?, Flags: 2
        PLAYER_SKILL_INFO_1_201 = UNIT_END + 0x37F,	//  Size: 384, Type: Chars?, Flags: 2
        PLAYER_SKILL_INFO_1_202 = UNIT_END + 0x380,	//  Size: 384, Type: Chars?, Flags: 2
        PLAYER_SKILL_INFO_1_203 = UNIT_END + 0x381,	//  Size: 384, Type: Chars?, Flags: 2
        PLAYER_SKILL_INFO_1_204 = UNIT_END + 0x382,	//  Size: 384, Type: Chars?, Flags: 2
        PLAYER_SKILL_INFO_1_205 = UNIT_END + 0x383,	//  Size: 384, Type: Chars?, Flags: 2
        PLAYER_SKILL_INFO_1_206 = UNIT_END + 0x384,	//  Size: 384, Type: Chars?, Flags: 2
        PLAYER_SKILL_INFO_1_207 = UNIT_END + 0x385,	//  Size: 384, Type: Chars?, Flags: 2
        PLAYER_SKILL_INFO_1_208 = UNIT_END + 0x386,	//  Size: 384, Type: Chars?, Flags: 2
        PLAYER_SKILL_INFO_1_209 = UNIT_END + 0x387,	//  Size: 384, Type: Chars?, Flags: 2
        PLAYER_SKILL_INFO_1_210 = UNIT_END + 0x388,	//  Size: 384, Type: Chars?, Flags: 2
        PLAYER_SKILL_INFO_1_211 = UNIT_END + 0x389,	//  Size: 384, Type: Chars?, Flags: 2
        PLAYER_SKILL_INFO_1_212 = UNIT_END + 0x38A,	//  Size: 384, Type: Chars?, Flags: 2
        PLAYER_SKILL_INFO_1_213 = UNIT_END + 0x38B,	//  Size: 384, Type: Chars?, Flags: 2
        PLAYER_SKILL_INFO_1_214 = UNIT_END + 0x38C,	//  Size: 384, Type: Chars?, Flags: 2
        PLAYER_SKILL_INFO_1_215 = UNIT_END + 0x38D,	//  Size: 384, Type: Chars?, Flags: 2
        PLAYER_SKILL_INFO_1_216 = UNIT_END + 0x38E,	//  Size: 384, Type: Chars?, Flags: 2
        PLAYER_SKILL_INFO_1_217 = UNIT_END + 0x38F,	//  Size: 384, Type: Chars?, Flags: 2
        PLAYER_SKILL_INFO_1_218 = UNIT_END + 0x390,	//  Size: 384, Type: Chars?, Flags: 2
        PLAYER_SKILL_INFO_1_219 = UNIT_END + 0x391,	//  Size: 384, Type: Chars?, Flags: 2
        PLAYER_SKILL_INFO_1_220 = UNIT_END + 0x392,	//  Size: 384, Type: Chars?, Flags: 2
        PLAYER_SKILL_INFO_1_221 = UNIT_END + 0x393,	//  Size: 384, Type: Chars?, Flags: 2
        PLAYER_SKILL_INFO_1_222 = UNIT_END + 0x394,	//  Size: 384, Type: Chars?, Flags: 2
        PLAYER_SKILL_INFO_1_223 = UNIT_END + 0x395,	//  Size: 384, Type: Chars?, Flags: 2
        PLAYER_SKILL_INFO_1_224 = UNIT_END + 0x396,	//  Size: 384, Type: Chars?, Flags: 2
        PLAYER_SKILL_INFO_1_225 = UNIT_END + 0x397,	//  Size: 384, Type: Chars?, Flags: 2
        PLAYER_SKILL_INFO_1_226 = UNIT_END + 0x398,	//  Size: 384, Type: Chars?, Flags: 2
        PLAYER_SKILL_INFO_1_227 = UNIT_END + 0x399,	//  Size: 384, Type: Chars?, Flags: 2
        PLAYER_SKILL_INFO_1_228 = UNIT_END + 0x39A,	//  Size: 384, Type: Chars?, Flags: 2
        PLAYER_SKILL_INFO_1_229 = UNIT_END + 0x39B,	//  Size: 384, Type: Chars?, Flags: 2
        PLAYER_SKILL_INFO_1_230 = UNIT_END + 0x39C,	//  Size: 384, Type: Chars?, Flags: 2
        PLAYER_SKILL_INFO_1_231 = UNIT_END + 0x39D,	//  Size: 384, Type: Chars?, Flags: 2
        PLAYER_SKILL_INFO_1_232 = UNIT_END + 0x39E,	//  Size: 384, Type: Chars?, Flags: 2
        PLAYER_SKILL_INFO_1_233 = UNIT_END + 0x39F,	//  Size: 384, Type: Chars?, Flags: 2
        PLAYER_SKILL_INFO_1_234 = UNIT_END + 0x3A0,	//  Size: 384, Type: Chars?, Flags: 2
        PLAYER_SKILL_INFO_1_235 = UNIT_END + 0x3A1,	//  Size: 384, Type: Chars?, Flags: 2
        PLAYER_SKILL_INFO_1_236 = UNIT_END + 0x3A2,	//  Size: 384, Type: Chars?, Flags: 2
        PLAYER_SKILL_INFO_1_237 = UNIT_END + 0x3A3,	//  Size: 384, Type: Chars?, Flags: 2
        PLAYER_SKILL_INFO_1_238 = UNIT_END + 0x3A4,	//  Size: 384, Type: Chars?, Flags: 2
        PLAYER_SKILL_INFO_1_239 = UNIT_END + 0x3A5,	//  Size: 384, Type: Chars?, Flags: 2
        PLAYER_SKILL_INFO_1_240 = UNIT_END + 0x3A6,	//  Size: 384, Type: Chars?, Flags: 2
        PLAYER_SKILL_INFO_1_241 = UNIT_END + 0x3A7,	//  Size: 384, Type: Chars?, Flags: 2
        PLAYER_SKILL_INFO_1_242 = UNIT_END + 0x3A8,	//  Size: 384, Type: Chars?, Flags: 2
        PLAYER_SKILL_INFO_1_243 = UNIT_END + 0x3A9,	//  Size: 384, Type: Chars?, Flags: 2
        PLAYER_SKILL_INFO_1_244 = UNIT_END + 0x3AA,	//  Size: 384, Type: Chars?, Flags: 2
        PLAYER_SKILL_INFO_1_245 = UNIT_END + 0x3AB,	//  Size: 384, Type: Chars?, Flags: 2
        PLAYER_SKILL_INFO_1_246 = UNIT_END + 0x3AC,	//  Size: 384, Type: Chars?, Flags: 2
        PLAYER_SKILL_INFO_1_247 = UNIT_END + 0x3AD,	//  Size: 384, Type: Chars?, Flags: 2
        PLAYER_SKILL_INFO_1_248 = UNIT_END + 0x3AE,	//  Size: 384, Type: Chars?, Flags: 2
        PLAYER_SKILL_INFO_1_249 = UNIT_END + 0x3AF,	//  Size: 384, Type: Chars?, Flags: 2
        PLAYER_SKILL_INFO_1_250 = UNIT_END + 0x3B0,	//  Size: 384, Type: Chars?, Flags: 2
        PLAYER_SKILL_INFO_1_251 = UNIT_END + 0x3B1,	//  Size: 384, Type: Chars?, Flags: 2
        PLAYER_SKILL_INFO_1_252 = UNIT_END + 0x3B2,	//  Size: 384, Type: Chars?, Flags: 2
        PLAYER_SKILL_INFO_1_253 = UNIT_END + 0x3B3,	//  Size: 384, Type: Chars?, Flags: 2
        PLAYER_SKILL_INFO_1_254 = UNIT_END + 0x3B4,	//  Size: 384, Type: Chars?, Flags: 2
        PLAYER_SKILL_INFO_1_255 = UNIT_END + 0x3B5,	//  Size: 384, Type: Chars?, Flags: 2
        PLAYER_SKILL_INFO_1_256 = UNIT_END + 0x3B6,	//  Size: 384, Type: Chars?, Flags: 2
        PLAYER_SKILL_INFO_1_257 = UNIT_END + 0x3B7,	//  Size: 384, Type: Chars?, Flags: 2
        PLAYER_SKILL_INFO_1_258 = UNIT_END + 0x3B8,	//  Size: 384, Type: Chars?, Flags: 2
        PLAYER_SKILL_INFO_1_259 = UNIT_END + 0x3B9,	//  Size: 384, Type: Chars?, Flags: 2
        PLAYER_SKILL_INFO_1_260 = UNIT_END + 0x3BA,	//  Size: 384, Type: Chars?, Flags: 2
        PLAYER_SKILL_INFO_1_261 = UNIT_END + 0x3BB,	//  Size: 384, Type: Chars?, Flags: 2
        PLAYER_SKILL_INFO_1_262 = UNIT_END + 0x3BC,	//  Size: 384, Type: Chars?, Flags: 2
        PLAYER_SKILL_INFO_1_263 = UNIT_END + 0x3BD,	//  Size: 384, Type: Chars?, Flags: 2
        PLAYER_SKILL_INFO_1_264 = UNIT_END + 0x3BE,	//  Size: 384, Type: Chars?, Flags: 2
        PLAYER_SKILL_INFO_1_265 = UNIT_END + 0x3BF,	//  Size: 384, Type: Chars?, Flags: 2
        PLAYER_SKILL_INFO_1_266 = UNIT_END + 0x3C0,	//  Size: 384, Type: Chars?, Flags: 2
        PLAYER_SKILL_INFO_1_267 = UNIT_END + 0x3C1,	//  Size: 384, Type: Chars?, Flags: 2
        PLAYER_SKILL_INFO_1_268 = UNIT_END + 0x3C2,	//  Size: 384, Type: Chars?, Flags: 2
        PLAYER_SKILL_INFO_1_269 = UNIT_END + 0x3C3,	//  Size: 384, Type: Chars?, Flags: 2
        PLAYER_SKILL_INFO_1_270 = UNIT_END + 0x3C4,	//  Size: 384, Type: Chars?, Flags: 2
        PLAYER_SKILL_INFO_1_271 = UNIT_END + 0x3C5,	//  Size: 384, Type: Chars?, Flags: 2
        PLAYER_SKILL_INFO_1_272 = UNIT_END + 0x3C6,	//  Size: 384, Type: Chars?, Flags: 2
        PLAYER_SKILL_INFO_1_273 = UNIT_END + 0x3C7,	//  Size: 384, Type: Chars?, Flags: 2
        PLAYER_SKILL_INFO_1_274 = UNIT_END + 0x3C8,	//  Size: 384, Type: Chars?, Flags: 2
        PLAYER_SKILL_INFO_1_275 = UNIT_END + 0x3C9,	//  Size: 384, Type: Chars?, Flags: 2
        PLAYER_SKILL_INFO_1_276 = UNIT_END + 0x3CA,	//  Size: 384, Type: Chars?, Flags: 2
        PLAYER_SKILL_INFO_1_277 = UNIT_END + 0x3CB,	//  Size: 384, Type: Chars?, Flags: 2
        PLAYER_SKILL_INFO_1_278 = UNIT_END + 0x3CC,	//  Size: 384, Type: Chars?, Flags: 2
        PLAYER_SKILL_INFO_1_279 = UNIT_END + 0x3CD,	//  Size: 384, Type: Chars?, Flags: 2
        PLAYER_SKILL_INFO_1_280 = UNIT_END + 0x3CE,	//  Size: 384, Type: Chars?, Flags: 2
        PLAYER_SKILL_INFO_1_281 = UNIT_END + 0x3CF,	//  Size: 384, Type: Chars?, Flags: 2
        PLAYER_SKILL_INFO_1_282 = UNIT_END + 0x3D0,	//  Size: 384, Type: Chars?, Flags: 2
        PLAYER_SKILL_INFO_1_283 = UNIT_END + 0x3D1,	//  Size: 384, Type: Chars?, Flags: 2
        PLAYER_SKILL_INFO_1_284 = UNIT_END + 0x3D2,	//  Size: 384, Type: Chars?, Flags: 2
        PLAYER_SKILL_INFO_1_285 = UNIT_END + 0x3D3,	//  Size: 384, Type: Chars?, Flags: 2
        PLAYER_SKILL_INFO_1_286 = UNIT_END + 0x3D4,	//  Size: 384, Type: Chars?, Flags: 2
        PLAYER_SKILL_INFO_1_287 = UNIT_END + 0x3D5,	//  Size: 384, Type: Chars?, Flags: 2
        PLAYER_SKILL_INFO_1_288 = UNIT_END + 0x3D6,	//  Size: 384, Type: Chars?, Flags: 2
        PLAYER_SKILL_INFO_1_289 = UNIT_END + 0x3D7,	//  Size: 384, Type: Chars?, Flags: 2
        PLAYER_SKILL_INFO_1_290 = UNIT_END + 0x3D8,	//  Size: 384, Type: Chars?, Flags: 2
        PLAYER_SKILL_INFO_1_291 = UNIT_END + 0x3D9,	//  Size: 384, Type: Chars?, Flags: 2
        PLAYER_SKILL_INFO_1_292 = UNIT_END + 0x3DA,	//  Size: 384, Type: Chars?, Flags: 2
        PLAYER_SKILL_INFO_1_293 = UNIT_END + 0x3DB,	//  Size: 384, Type: Chars?, Flags: 2
        PLAYER_SKILL_INFO_1_294 = UNIT_END + 0x3DC,	//  Size: 384, Type: Chars?, Flags: 2
        PLAYER_SKILL_INFO_1_295 = UNIT_END + 0x3DD,	//  Size: 384, Type: Chars?, Flags: 2
        PLAYER_SKILL_INFO_1_296 = UNIT_END + 0x3DE,	//  Size: 384, Type: Chars?, Flags: 2
        PLAYER_SKILL_INFO_1_297 = UNIT_END + 0x3DF,	//  Size: 384, Type: Chars?, Flags: 2
        PLAYER_SKILL_INFO_1_298 = UNIT_END + 0x3E0,	//  Size: 384, Type: Chars?, Flags: 2
        PLAYER_SKILL_INFO_1_299 = UNIT_END + 0x3E1,	//  Size: 384, Type: Chars?, Flags: 2
        PLAYER_SKILL_INFO_1_300 = UNIT_END + 0x3E2,	//  Size: 384, Type: Chars?, Flags: 2
        PLAYER_SKILL_INFO_1_301 = UNIT_END + 0x3E3,	//  Size: 384, Type: Chars?, Flags: 2
        PLAYER_SKILL_INFO_1_302 = UNIT_END + 0x3E4,	//  Size: 384, Type: Chars?, Flags: 2
        PLAYER_SKILL_INFO_1_303 = UNIT_END + 0x3E5,	//  Size: 384, Type: Chars?, Flags: 2
        PLAYER_SKILL_INFO_1_304 = UNIT_END + 0x3E6,	//  Size: 384, Type: Chars?, Flags: 2
        PLAYER_SKILL_INFO_1_305 = UNIT_END + 0x3E7,	//  Size: 384, Type: Chars?, Flags: 2
        PLAYER_SKILL_INFO_1_306 = UNIT_END + 0x3E8,	//  Size: 384, Type: Chars?, Flags: 2
        PLAYER_SKILL_INFO_1_307 = UNIT_END + 0x3E9,	//  Size: 384, Type: Chars?, Flags: 2
        PLAYER_SKILL_INFO_1_308 = UNIT_END + 0x3EA,	//  Size: 384, Type: Chars?, Flags: 2
        PLAYER_SKILL_INFO_1_309 = UNIT_END + 0x3EB,	//  Size: 384, Type: Chars?, Flags: 2
        PLAYER_SKILL_INFO_1_310 = UNIT_END + 0x3EC,	//  Size: 384, Type: Chars?, Flags: 2
        PLAYER_SKILL_INFO_1_311 = UNIT_END + 0x3ED,	//  Size: 384, Type: Chars?, Flags: 2
        PLAYER_SKILL_INFO_1_312 = UNIT_END + 0x3EE,	//  Size: 384, Type: Chars?, Flags: 2
        PLAYER_SKILL_INFO_1_313 = UNIT_END + 0x3EF,	//  Size: 384, Type: Chars?, Flags: 2
        PLAYER_SKILL_INFO_1_314 = UNIT_END + 0x3F0,	//  Size: 384, Type: Chars?, Flags: 2
        PLAYER_SKILL_INFO_1_315 = UNIT_END + 0x3F1,	//  Size: 384, Type: Chars?, Flags: 2
        PLAYER_SKILL_INFO_1_316 = UNIT_END + 0x3F2,	//  Size: 384, Type: Chars?, Flags: 2
        PLAYER_SKILL_INFO_1_317 = UNIT_END + 0x3F3,	//  Size: 384, Type: Chars?, Flags: 2
        PLAYER_SKILL_INFO_1_318 = UNIT_END + 0x3F4,	//  Size: 384, Type: Chars?, Flags: 2
        PLAYER_SKILL_INFO_1_319 = UNIT_END + 0x3F5,	//  Size: 384, Type: Chars?, Flags: 2
        PLAYER_SKILL_INFO_1_320 = UNIT_END + 0x3F6,	//  Size: 384, Type: Chars?, Flags: 2
        PLAYER_SKILL_INFO_1_321 = UNIT_END + 0x3F7,	//  Size: 384, Type: Chars?, Flags: 2
        PLAYER_SKILL_INFO_1_322 = UNIT_END + 0x3F8,	//  Size: 384, Type: Chars?, Flags: 2
        PLAYER_SKILL_INFO_1_323 = UNIT_END + 0x3F9,	//  Size: 384, Type: Chars?, Flags: 2
        PLAYER_SKILL_INFO_1_324 = UNIT_END + 0x3FA,	//  Size: 384, Type: Chars?, Flags: 2
        PLAYER_SKILL_INFO_1_325 = UNIT_END + 0x3FB,	//  Size: 384, Type: Chars?, Flags: 2
        PLAYER_SKILL_INFO_1_326 = UNIT_END + 0x3FC,	//  Size: 384, Type: Chars?, Flags: 2
        PLAYER_SKILL_INFO_1_327 = UNIT_END + 0x3FD,	//  Size: 384, Type: Chars?, Flags: 2
        PLAYER_SKILL_INFO_1_328 = UNIT_END + 0x3FE,	//  Size: 384, Type: Chars?, Flags: 2
        PLAYER_SKILL_INFO_1_329 = UNIT_END + 0x3FF,	//  Size: 384, Type: Chars?, Flags: 2
        PLAYER_SKILL_INFO_1_330 = UNIT_END + 0x400,	//  Size: 384, Type: Chars?, Flags: 2
        PLAYER_SKILL_INFO_1_331 = UNIT_END + 0x401,	//  Size: 384, Type: Chars?, Flags: 2
        PLAYER_SKILL_INFO_1_332 = UNIT_END + 0x402,	//  Size: 384, Type: Chars?, Flags: 2
        PLAYER_SKILL_INFO_1_333 = UNIT_END + 0x403,	//  Size: 384, Type: Chars?, Flags: 2
        PLAYER_SKILL_INFO_1_334 = UNIT_END + 0x404,	//  Size: 384, Type: Chars?, Flags: 2
        PLAYER_SKILL_INFO_1_335 = UNIT_END + 0x405,	//  Size: 384, Type: Chars?, Flags: 2
        PLAYER_SKILL_INFO_1_336 = UNIT_END + 0x406,	//  Size: 384, Type: Chars?, Flags: 2
        PLAYER_SKILL_INFO_1_337 = UNIT_END + 0x407,	//  Size: 384, Type: Chars?, Flags: 2
        PLAYER_SKILL_INFO_1_338 = UNIT_END + 0x408,	//  Size: 384, Type: Chars?, Flags: 2
        PLAYER_SKILL_INFO_1_339 = UNIT_END + 0x409,	//  Size: 384, Type: Chars?, Flags: 2
        PLAYER_SKILL_INFO_1_340 = UNIT_END + 0x40A,	//  Size: 384, Type: Chars?, Flags: 2
        PLAYER_SKILL_INFO_1_341 = UNIT_END + 0x40B,	//  Size: 384, Type: Chars?, Flags: 2
        PLAYER_SKILL_INFO_1_342 = UNIT_END + 0x40C,	//  Size: 384, Type: Chars?, Flags: 2
        PLAYER_SKILL_INFO_1_343 = UNIT_END + 0x40D,	//  Size: 384, Type: Chars?, Flags: 2
        PLAYER_SKILL_INFO_1_344 = UNIT_END + 0x40E,	//  Size: 384, Type: Chars?, Flags: 2
        PLAYER_SKILL_INFO_1_345 = UNIT_END + 0x40F,	//  Size: 384, Type: Chars?, Flags: 2
        PLAYER_SKILL_INFO_1_346 = UNIT_END + 0x410,	//  Size: 384, Type: Chars?, Flags: 2
        PLAYER_SKILL_INFO_1_347 = UNIT_END + 0x411,	//  Size: 384, Type: Chars?, Flags: 2
        PLAYER_SKILL_INFO_1_348 = UNIT_END + 0x412,	//  Size: 384, Type: Chars?, Flags: 2
        PLAYER_SKILL_INFO_1_349 = UNIT_END + 0x413,	//  Size: 384, Type: Chars?, Flags: 2
        PLAYER_SKILL_INFO_1_350 = UNIT_END + 0x414,	//  Size: 384, Type: Chars?, Flags: 2
        PLAYER_SKILL_INFO_1_351 = UNIT_END + 0x415,	//  Size: 384, Type: Chars?, Flags: 2
        PLAYER_SKILL_INFO_1_352 = UNIT_END + 0x416,	//  Size: 384, Type: Chars?, Flags: 2
        PLAYER_SKILL_INFO_1_353 = UNIT_END + 0x417,	//  Size: 384, Type: Chars?, Flags: 2
        PLAYER_SKILL_INFO_1_354 = UNIT_END + 0x418,	//  Size: 384, Type: Chars?, Flags: 2
        PLAYER_SKILL_INFO_1_355 = UNIT_END + 0x419,	//  Size: 384, Type: Chars?, Flags: 2
        PLAYER_SKILL_INFO_1_356 = UNIT_END + 0x41A,	//  Size: 384, Type: Chars?, Flags: 2
        PLAYER_SKILL_INFO_1_357 = UNIT_END + 0x41B,	//  Size: 384, Type: Chars?, Flags: 2
        PLAYER_SKILL_INFO_1_358 = UNIT_END + 0x41C,	//  Size: 384, Type: Chars?, Flags: 2
        PLAYER_SKILL_INFO_1_359 = UNIT_END + 0x41D,	//  Size: 384, Type: Chars?, Flags: 2
        PLAYER_SKILL_INFO_1_360 = UNIT_END + 0x41E,	//  Size: 384, Type: Chars?, Flags: 2
        PLAYER_SKILL_INFO_1_361 = UNIT_END + 0x41F,	//  Size: 384, Type: Chars?, Flags: 2
        PLAYER_SKILL_INFO_1_362 = UNIT_END + 0x420,	//  Size: 384, Type: Chars?, Flags: 2
        PLAYER_SKILL_INFO_1_363 = UNIT_END + 0x421,	//  Size: 384, Type: Chars?, Flags: 2
        PLAYER_SKILL_INFO_1_364 = UNIT_END + 0x422,	//  Size: 384, Type: Chars?, Flags: 2
        PLAYER_SKILL_INFO_1_365 = UNIT_END + 0x423,	//  Size: 384, Type: Chars?, Flags: 2
        PLAYER_SKILL_INFO_1_366 = UNIT_END + 0x424,	//  Size: 384, Type: Chars?, Flags: 2
        PLAYER_SKILL_INFO_1_367 = UNIT_END + 0x425,	//  Size: 384, Type: Chars?, Flags: 2
        PLAYER_SKILL_INFO_1_368 = UNIT_END + 0x426,	//  Size: 384, Type: Chars?, Flags: 2
        PLAYER_SKILL_INFO_1_369 = UNIT_END + 0x427,	//  Size: 384, Type: Chars?, Flags: 2
        PLAYER_SKILL_INFO_1_370 = UNIT_END + 0x428,	//  Size: 384, Type: Chars?, Flags: 2
        PLAYER_SKILL_INFO_1_371 = UNIT_END + 0x429,	//  Size: 384, Type: Chars?, Flags: 2
        PLAYER_SKILL_INFO_1_372 = UNIT_END + 0x42A,	//  Size: 384, Type: Chars?, Flags: 2
        PLAYER_SKILL_INFO_1_373 = UNIT_END + 0x42B,	//  Size: 384, Type: Chars?, Flags: 2
        PLAYER_SKILL_INFO_1_374 = UNIT_END + 0x42C,	//  Size: 384, Type: Chars?, Flags: 2
        PLAYER_SKILL_INFO_1_375 = UNIT_END + 0x42D,	//  Size: 384, Type: Chars?, Flags: 2
        PLAYER_SKILL_INFO_1_376 = UNIT_END + 0x42E,	//  Size: 384, Type: Chars?, Flags: 2
        PLAYER_SKILL_INFO_1_377 = UNIT_END + 0x42F,	//  Size: 384, Type: Chars?, Flags: 2
        PLAYER_SKILL_INFO_1_378 = UNIT_END + 0x430,	//  Size: 384, Type: Chars?, Flags: 2
        PLAYER_SKILL_INFO_1_379 = UNIT_END + 0x431,	//  Size: 384, Type: Chars?, Flags: 2
        PLAYER_SKILL_INFO_1_380 = UNIT_END + 0x432,	//  Size: 384, Type: Chars?, Flags: 2
        PLAYER_SKILL_INFO_1_381 = UNIT_END + 0x433,	//  Size: 384, Type: Chars?, Flags: 2
        PLAYER_SKILL_INFO_1_382 = UNIT_END + 0x434,	//  Size: 384, Type: Chars?, Flags: 2
        PLAYER_SKILL_INFO_1_383 = UNIT_END + 0x435,	//  Size: 384, Type: Chars?, Flags: 2
        PLAYER_CHARACTER_POINTS1 = UNIT_END + 0x436,	//  Size: 1, Type: Int32, Flags: 2
        PLAYER_CHARACTER_POINTS2 = UNIT_END + 0x437,	//  Size: 1, Type: Int32, Flags: 2
        PLAYER_TRACK_CREATURES = UNIT_END + 0x438,	//  Size: 1, Type: Int32, Flags: 2
        PLAYER_TRACK_RESOURCES = UNIT_END + 0x439,	//  Size: 1, Type: Int32, Flags: 2
        PLAYER_BLOCK_PERCENTAGE = UNIT_END + 0x43A,	//  Size: 1, Type: Float, Flags: 2
        PLAYER_DODGE_PERCENTAGE = UNIT_END + 0x43B,	//  Size: 1, Type: Float, Flags: 2
        PLAYER_PARRY_PERCENTAGE = UNIT_END + 0x43C,	//  Size: 1, Type: Float, Flags: 2
        PLAYER_EXPERTISE = UNIT_END + 0x43D,	//  Size: 1, Type: Int32, Flags: 2
        PLAYER_OFFHAND_EXPERTISE = UNIT_END + 0x43E,	//  Size: 1, Type: Int32, Flags: 2
        PLAYER_CRIT_PERCENTAGE = UNIT_END + 0x43F,	//  Size: 1, Type: Float, Flags: 2
        PLAYER_RANGED_CRIT_PERCENTAGE = UNIT_END + 0x440,	//  Size: 1, Type: Float, Flags: 2
        PLAYER_OFFHAND_CRIT_PERCENTAGE = UNIT_END + 0x441,	//  Size: 1, Type: Float, Flags: 2
        PLAYER_SPELL_CRIT_PERCENTAGE1 = UNIT_END + 0x442,	//  Size: 7, Type: Float, Flags: 2
        PLAYER_SPELL_CRIT_PERCENTAGE01 = UNIT_END + 0x443,	//  Size: 7, Type: Float, Flags: 2
        PLAYER_SPELL_CRIT_PERCENTAGE02 = UNIT_END + 0x444,	//  Size: 7, Type: Float, Flags: 2
        PLAYER_SPELL_CRIT_PERCENTAGE03 = UNIT_END + 0x445,	//  Size: 7, Type: Float, Flags: 2
        PLAYER_SPELL_CRIT_PERCENTAGE04 = UNIT_END + 0x446,	//  Size: 7, Type: Float, Flags: 2
        PLAYER_SPELL_CRIT_PERCENTAGE05 = UNIT_END + 0x447,	//  Size: 7, Type: Float, Flags: 2
        PLAYER_SPELL_CRIT_PERCENTAGE06 = UNIT_END + 0x448,	//  Size: 7, Type: Float, Flags: 2
        PLAYER_SHIELD_BLOCK = UNIT_END + 0x449,	//  Size: 1, Type: Int32, Flags: 2
        PLAYER_EXPLORED_ZONES_1 = UNIT_END + 0x44A,	//  Size: 64, Type: Bytes, Flags: 2
        PLAYER_EXPLORED_ZONES_01 = UNIT_END + 0x44B,	//  Size: 64, Type: Bytes, Flags: 2
        PLAYER_EXPLORED_ZONES_02 = UNIT_END + 0x44C,	//  Size: 64, Type: Bytes, Flags: 2
        PLAYER_EXPLORED_ZONES_03 = UNIT_END + 0x44D,	//  Size: 64, Type: Bytes, Flags: 2
        PLAYER_EXPLORED_ZONES_04 = UNIT_END + 0x44E,	//  Size: 64, Type: Bytes, Flags: 2
        PLAYER_EXPLORED_ZONES_05 = UNIT_END + 0x44F,	//  Size: 64, Type: Bytes, Flags: 2
        PLAYER_EXPLORED_ZONES_06 = UNIT_END + 0x450,	//  Size: 64, Type: Bytes, Flags: 2
        PLAYER_EXPLORED_ZONES_07 = UNIT_END + 0x451,	//  Size: 64, Type: Bytes, Flags: 2
        PLAYER_EXPLORED_ZONES_08 = UNIT_END + 0x452,	//  Size: 64, Type: Bytes, Flags: 2
        PLAYER_EXPLORED_ZONES_09 = UNIT_END + 0x453,	//  Size: 64, Type: Bytes, Flags: 2
        PLAYER_EXPLORED_ZONES_10 = UNIT_END + 0x454,	//  Size: 64, Type: Bytes, Flags: 2
        PLAYER_EXPLORED_ZONES_11 = UNIT_END + 0x455,	//  Size: 64, Type: Bytes, Flags: 2
        PLAYER_EXPLORED_ZONES_12 = UNIT_END + 0x456,	//  Size: 64, Type: Bytes, Flags: 2
        PLAYER_EXPLORED_ZONES_13 = UNIT_END + 0x457,	//  Size: 64, Type: Bytes, Flags: 2
        PLAYER_EXPLORED_ZONES_14 = UNIT_END + 0x458,	//  Size: 64, Type: Bytes, Flags: 2
        PLAYER_EXPLORED_ZONES_15 = UNIT_END + 0x459,	//  Size: 64, Type: Bytes, Flags: 2
        PLAYER_EXPLORED_ZONES_16 = UNIT_END + 0x45A,	//  Size: 64, Type: Bytes, Flags: 2
        PLAYER_EXPLORED_ZONES_17 = UNIT_END + 0x45B,	//  Size: 64, Type: Bytes, Flags: 2
        PLAYER_EXPLORED_ZONES_18 = UNIT_END + 0x45C,	//  Size: 64, Type: Bytes, Flags: 2
        PLAYER_EXPLORED_ZONES_19 = UNIT_END + 0x45D,	//  Size: 64, Type: Bytes, Flags: 2
        PLAYER_EXPLORED_ZONES_20 = UNIT_END + 0x45E,	//  Size: 64, Type: Bytes, Flags: 2
        PLAYER_EXPLORED_ZONES_21 = UNIT_END + 0x45F,	//  Size: 64, Type: Bytes, Flags: 2
        PLAYER_EXPLORED_ZONES_22 = UNIT_END + 0x460,	//  Size: 64, Type: Bytes, Flags: 2
        PLAYER_EXPLORED_ZONES_23 = UNIT_END + 0x461,	//  Size: 64, Type: Bytes, Flags: 2
        PLAYER_EXPLORED_ZONES_24 = UNIT_END + 0x462,	//  Size: 64, Type: Bytes, Flags: 2
        PLAYER_EXPLORED_ZONES_25 = UNIT_END + 0x463,	//  Size: 64, Type: Bytes, Flags: 2
        PLAYER_EXPLORED_ZONES_26 = UNIT_END + 0x464,	//  Size: 64, Type: Bytes, Flags: 2
        PLAYER_EXPLORED_ZONES_27 = UNIT_END + 0x465,	//  Size: 64, Type: Bytes, Flags: 2
        PLAYER_EXPLORED_ZONES_28 = UNIT_END + 0x466,	//  Size: 64, Type: Bytes, Flags: 2
        PLAYER_EXPLORED_ZONES_29 = UNIT_END + 0x467,	//  Size: 64, Type: Bytes, Flags: 2
        PLAYER_EXPLORED_ZONES_30 = UNIT_END + 0x468,	//  Size: 64, Type: Bytes, Flags: 2
        PLAYER_EXPLORED_ZONES_31 = UNIT_END + 0x469,	//  Size: 64, Type: Bytes, Flags: 2
        PLAYER_EXPLORED_ZONES_32 = UNIT_END + 0x46A,	//  Size: 64, Type: Bytes, Flags: 2
        PLAYER_EXPLORED_ZONES_33 = UNIT_END + 0x46B,	//  Size: 64, Type: Bytes, Flags: 2
        PLAYER_EXPLORED_ZONES_34 = UNIT_END + 0x46C,	//  Size: 64, Type: Bytes, Flags: 2
        PLAYER_EXPLORED_ZONES_35 = UNIT_END + 0x46D,	//  Size: 64, Type: Bytes, Flags: 2
        PLAYER_EXPLORED_ZONES_36 = UNIT_END + 0x46E,	//  Size: 64, Type: Bytes, Flags: 2
        PLAYER_EXPLORED_ZONES_37 = UNIT_END + 0x46F,	//  Size: 64, Type: Bytes, Flags: 2
        PLAYER_EXPLORED_ZONES_38 = UNIT_END + 0x470,	//  Size: 64, Type: Bytes, Flags: 2
        PLAYER_EXPLORED_ZONES_39 = UNIT_END + 0x471,	//  Size: 64, Type: Bytes, Flags: 2
        PLAYER_EXPLORED_ZONES_40 = UNIT_END + 0x472,	//  Size: 64, Type: Bytes, Flags: 2
        PLAYER_EXPLORED_ZONES_41 = UNIT_END + 0x473,	//  Size: 64, Type: Bytes, Flags: 2
        PLAYER_EXPLORED_ZONES_42 = UNIT_END + 0x474,	//  Size: 64, Type: Bytes, Flags: 2
        PLAYER_EXPLORED_ZONES_43 = UNIT_END + 0x475,	//  Size: 64, Type: Bytes, Flags: 2
        PLAYER_EXPLORED_ZONES_44 = UNIT_END + 0x476,	//  Size: 64, Type: Bytes, Flags: 2
        PLAYER_EXPLORED_ZONES_45 = UNIT_END + 0x477,	//  Size: 64, Type: Bytes, Flags: 2
        PLAYER_EXPLORED_ZONES_46 = UNIT_END + 0x478,	//  Size: 64, Type: Bytes, Flags: 2
        PLAYER_EXPLORED_ZONES_47 = UNIT_END + 0x479,	//  Size: 64, Type: Bytes, Flags: 2
        PLAYER_EXPLORED_ZONES_48 = UNIT_END + 0x47A,	//  Size: 64, Type: Bytes, Flags: 2
        PLAYER_EXPLORED_ZONES_49 = UNIT_END + 0x47B,	//  Size: 64, Type: Bytes, Flags: 2
        PLAYER_EXPLORED_ZONES_50 = UNIT_END + 0x47C,	//  Size: 64, Type: Bytes, Flags: 2
        PLAYER_EXPLORED_ZONES_51 = UNIT_END + 0x47D,	//  Size: 64, Type: Bytes, Flags: 2
        PLAYER_EXPLORED_ZONES_52 = UNIT_END + 0x47E,	//  Size: 64, Type: Bytes, Flags: 2
        PLAYER_EXPLORED_ZONES_53 = UNIT_END + 0x47F,	//  Size: 64, Type: Bytes, Flags: 2
        PLAYER_EXPLORED_ZONES_54 = UNIT_END + 0x480,	//  Size: 64, Type: Bytes, Flags: 2
        PLAYER_EXPLORED_ZONES_55 = UNIT_END + 0x481,	//  Size: 64, Type: Bytes, Flags: 2
        PLAYER_EXPLORED_ZONES_56 = UNIT_END + 0x482,	//  Size: 64, Type: Bytes, Flags: 2
        PLAYER_EXPLORED_ZONES_57 = UNIT_END + 0x483,	//  Size: 64, Type: Bytes, Flags: 2
        PLAYER_EXPLORED_ZONES_58 = UNIT_END + 0x484,	//  Size: 64, Type: Bytes, Flags: 2
        PLAYER_EXPLORED_ZONES_59 = UNIT_END + 0x485,	//  Size: 64, Type: Bytes, Flags: 2
        PLAYER_EXPLORED_ZONES_60 = UNIT_END + 0x486,	//  Size: 64, Type: Bytes, Flags: 2
        PLAYER_EXPLORED_ZONES_61 = UNIT_END + 0x487,	//  Size: 64, Type: Bytes, Flags: 2
        PLAYER_EXPLORED_ZONES_62 = UNIT_END + 0x488,	//  Size: 64, Type: Bytes, Flags: 2
        PLAYER_EXPLORED_ZONES_63 = UNIT_END + 0x489,	//  Size: 64, Type: Bytes, Flags: 2
        PLAYER_REST_STATE_EXPERIENCE = UNIT_END + 0x48A,	//  Size: 1, Type: Int32, Flags: 2
        PLAYER_FIELD_COINAGE = UNIT_END + 0x48B,	//  Size: 1, Type: Int32, Flags: 2
        PLAYER_FIELD_MOD_DAMAGE_DONE_POS = UNIT_END + 0x48C,	//  Size: 7, Type: Int32, Flags: 2
        PLAYER_FIELD_MOD_DAMAGE_DONE_POS_01 = UNIT_END + 0x48D,	//  Size: 7, Type: Int32, Flags: 2
        PLAYER_FIELD_MOD_DAMAGE_DONE_POS_02 = UNIT_END + 0x48E,	//  Size: 7, Type: Int32, Flags: 2
        PLAYER_FIELD_MOD_DAMAGE_DONE_POS_03 = UNIT_END + 0x48F,	//  Size: 7, Type: Int32, Flags: 2
        PLAYER_FIELD_MOD_DAMAGE_DONE_POS_04 = UNIT_END + 0x490,	//  Size: 7, Type: Int32, Flags: 2
        PLAYER_FIELD_MOD_DAMAGE_DONE_POS_05 = UNIT_END + 0x491,	//  Size: 7, Type: Int32, Flags: 2
        PLAYER_FIELD_MOD_DAMAGE_DONE_POS_06 = UNIT_END + 0x492,	//  Size: 7, Type: Int32, Flags: 2
        PLAYER_FIELD_MOD_DAMAGE_DONE_NEG = UNIT_END + 0x493,	//  Size: 7, Type: Int32, Flags: 2
        PLAYER_FIELD_MOD_DAMAGE_DONE_NEG_01 = UNIT_END + 0x494,	//  Size: 7, Type: Int32, Flags: 2
        PLAYER_FIELD_MOD_DAMAGE_DONE_NEG_02 = UNIT_END + 0x495,	//  Size: 7, Type: Int32, Flags: 2
        PLAYER_FIELD_MOD_DAMAGE_DONE_NEG_03 = UNIT_END + 0x496,	//  Size: 7, Type: Int32, Flags: 2
        PLAYER_FIELD_MOD_DAMAGE_DONE_NEG_04 = UNIT_END + 0x497,	//  Size: 7, Type: Int32, Flags: 2
        PLAYER_FIELD_MOD_DAMAGE_DONE_NEG_05 = UNIT_END + 0x498,	//  Size: 7, Type: Int32, Flags: 2
        PLAYER_FIELD_MOD_DAMAGE_DONE_NEG_06 = UNIT_END + 0x499,	//  Size: 7, Type: Int32, Flags: 2
        PLAYER_FIELD_MOD_DAMAGE_DONE_PCT = UNIT_END + 0x49A,	//  Size: 7, Type: Int32, Flags: 2
        PLAYER_FIELD_MOD_DAMAGE_DONE_PCT_01 = UNIT_END + 0x49B,	//  Size: 7, Type: Int32, Flags: 2
        PLAYER_FIELD_MOD_DAMAGE_DONE_PCT_02 = UNIT_END + 0x49C,	//  Size: 7, Type: Int32, Flags: 2
        PLAYER_FIELD_MOD_DAMAGE_DONE_PCT_03 = UNIT_END + 0x49D,	//  Size: 7, Type: Int32, Flags: 2
        PLAYER_FIELD_MOD_DAMAGE_DONE_PCT_04 = UNIT_END + 0x49E,	//  Size: 7, Type: Int32, Flags: 2
        PLAYER_FIELD_MOD_DAMAGE_DONE_PCT_05 = UNIT_END + 0x49F,	//  Size: 7, Type: Int32, Flags: 2
        PLAYER_FIELD_MOD_DAMAGE_DONE_PCT_06 = UNIT_END + 0x4A0,	//  Size: 7, Type: Int32, Flags: 2
        PLAYER_FIELD_MOD_HEALING_DONE_POS = UNIT_END + 0x4A1,	//  Size: 1, Type: Int32, Flags: 2
        PLAYER_FIELD_MOD_TARGET_RESISTANCE = UNIT_END + 0x4A2,	//  Size: 1, Type: Int32, Flags: 2
        PLAYER_FIELD_MOD_TARGET_PHYSICAL_RESISTANCE = UNIT_END + 0x4A3,	//  Size: 1, Type: Int32, Flags: 2
        PLAYER_FIELD_BYTES = UNIT_END + 0x4A4,	//  Size: 1, Type: Bytes, Flags: 2
        PLAYER_AMMO_ID = UNIT_END + 0x4A5,	//  Size: 1, Type: Int32, Flags: 2
        PLAYER_SELF_RES_SPELL = UNIT_END + 0x4A6,	//  Size: 1, Type: Int32, Flags: 2
        PLAYER_FIELD_PVP_MEDALS = UNIT_END + 0x4A7,	//  Size: 1, Type: Int32, Flags: 2
        PLAYER_FIELD_BUYBACK_PRICE_1 = UNIT_END + 0x4A8,	//  Size: 12, Type: Int32, Flags: 2
        PLAYER_FIELD_BUYBACK_PRICE_01 = UNIT_END + 0x4A9,	//  Size: 12, Type: Int32, Flags: 2
        PLAYER_FIELD_BUYBACK_PRICE_02 = UNIT_END + 0x4AA,	//  Size: 12, Type: Int32, Flags: 2
        PLAYER_FIELD_BUYBACK_PRICE_03 = UNIT_END + 0x4AB,	//  Size: 12, Type: Int32, Flags: 2
        PLAYER_FIELD_BUYBACK_PRICE_04 = UNIT_END + 0x4AC,	//  Size: 12, Type: Int32, Flags: 2
        PLAYER_FIELD_BUYBACK_PRICE_05 = UNIT_END + 0x4AD,	//  Size: 12, Type: Int32, Flags: 2
        PLAYER_FIELD_BUYBACK_PRICE_06 = UNIT_END + 0x4AE,	//  Size: 12, Type: Int32, Flags: 2
        PLAYER_FIELD_BUYBACK_PRICE_07 = UNIT_END + 0x4AF,	//  Size: 12, Type: Int32, Flags: 2
        PLAYER_FIELD_BUYBACK_PRICE_08 = UNIT_END + 0x4B0,	//  Size: 12, Type: Int32, Flags: 2
        PLAYER_FIELD_BUYBACK_PRICE_09 = UNIT_END + 0x4B1,	//  Size: 12, Type: Int32, Flags: 2
        PLAYER_FIELD_BUYBACK_PRICE_10 = UNIT_END + 0x4B2,	//  Size: 12, Type: Int32, Flags: 2
        PLAYER_FIELD_BUYBACK_PRICE_11 = UNIT_END + 0x4B3,	//  Size: 12, Type: Int32, Flags: 2
        PLAYER_FIELD_BUYBACK_TIMESTAMP_1 = UNIT_END + 0x4B4,	//  Size: 12, Type: Int32, Flags: 2
        PLAYER_FIELD_BUYBACK_TIMESTAMP_01 = UNIT_END + 0x4B5,	//  Size: 12, Type: Int32, Flags: 2
        PLAYER_FIELD_BUYBACK_TIMESTAMP_02 = UNIT_END + 0x4B6,	//  Size: 12, Type: Int32, Flags: 2
        PLAYER_FIELD_BUYBACK_TIMESTAMP_03 = UNIT_END + 0x4B7,	//  Size: 12, Type: Int32, Flags: 2
        PLAYER_FIELD_BUYBACK_TIMESTAMP_04 = UNIT_END + 0x4B8,	//  Size: 12, Type: Int32, Flags: 2
        PLAYER_FIELD_BUYBACK_TIMESTAMP_05 = UNIT_END + 0x4B9,	//  Size: 12, Type: Int32, Flags: 2
        PLAYER_FIELD_BUYBACK_TIMESTAMP_06 = UNIT_END + 0x4BA,	//  Size: 12, Type: Int32, Flags: 2
        PLAYER_FIELD_BUYBACK_TIMESTAMP_07 = UNIT_END + 0x4BB,	//  Size: 12, Type: Int32, Flags: 2
        PLAYER_FIELD_BUYBACK_TIMESTAMP_08 = UNIT_END + 0x4BC,	//  Size: 12, Type: Int32, Flags: 2
        PLAYER_FIELD_BUYBACK_TIMESTAMP_09 = UNIT_END + 0x4BD,	//  Size: 12, Type: Int32, Flags: 2
        PLAYER_FIELD_BUYBACK_TIMESTAMP_10 = UNIT_END + 0x4BE,	//  Size: 12, Type: Int32, Flags: 2
        PLAYER_FIELD_BUYBACK_TIMESTAMP_11 = UNIT_END + 0x4BF,	//  Size: 12, Type: Int32, Flags: 2
        PLAYER_FIELD_KILLS = UNIT_END + 0x4C0,	//  Size: 1, Type: Chars?, Flags: 2
        PLAYER_FIELD_TODAY_CONTRIBUTION = UNIT_END + 0x4C1,	//  Size: 1, Type: Int32, Flags: 2
        PLAYER_FIELD_YESTERDAY_CONTRIBUTION = UNIT_END + 0x4C2,	//  Size: 1, Type: Int32, Flags: 2
        PLAYER_FIELD_LIFETIME_HONORBALE_KILLS = UNIT_END + 0x4C3,	//  Size: 1, Type: Int32, Flags: 2
        PLAYER_FIELD_BYTES2 = UNIT_END + 0x4C4,	//  Size: 1, Type: Bytes, Flags: 2
        PLAYER_FIELD_WATCHED_FACTION_INDEX = UNIT_END + 0x4C5,	//  Size: 1, Type: Int32, Flags: 2
        PLAYER_FIELD_COMBAT_RATING_1 = UNIT_END + 0x4C6,	//  Size: 24, Type: Int32, Flags: 2
        PLAYER_FIELD_COMBAT_RATING_01 = UNIT_END + 0x4C7,	//  Size: 24, Type: Int32, Flags: 2
        PLAYER_FIELD_COMBAT_RATING_02 = UNIT_END + 0x4C8,	//  Size: 24, Type: Int32, Flags: 2
        PLAYER_FIELD_COMBAT_RATING_03 = UNIT_END + 0x4C9,	//  Size: 24, Type: Int32, Flags: 2
        PLAYER_FIELD_COMBAT_RATING_04 = UNIT_END + 0x4CA,	//  Size: 24, Type: Int32, Flags: 2
        PLAYER_FIELD_COMBAT_RATING_05 = UNIT_END + 0x4CB,	//  Size: 24, Type: Int32, Flags: 2
        PLAYER_FIELD_COMBAT_RATING_06 = UNIT_END + 0x4CC,	//  Size: 24, Type: Int32, Flags: 2
        PLAYER_FIELD_COMBAT_RATING_07 = UNIT_END + 0x4CD,	//  Size: 24, Type: Int32, Flags: 2
        PLAYER_FIELD_COMBAT_RATING_08 = UNIT_END + 0x4CE,	//  Size: 24, Type: Int32, Flags: 2
        PLAYER_FIELD_COMBAT_RATING_09 = UNIT_END + 0x4CF,	//  Size: 24, Type: Int32, Flags: 2
        PLAYER_FIELD_COMBAT_RATING_10 = UNIT_END + 0x4D0,	//  Size: 24, Type: Int32, Flags: 2
        PLAYER_FIELD_COMBAT_RATING_11 = UNIT_END + 0x4D1,	//  Size: 24, Type: Int32, Flags: 2
        PLAYER_FIELD_COMBAT_RATING_12 = UNIT_END + 0x4D2,	//  Size: 24, Type: Int32, Flags: 2
        PLAYER_FIELD_COMBAT_RATING_13 = UNIT_END + 0x4D3,	//  Size: 24, Type: Int32, Flags: 2
        PLAYER_FIELD_COMBAT_RATING_14 = UNIT_END + 0x4D4,	//  Size: 24, Type: Int32, Flags: 2
        PLAYER_FIELD_COMBAT_RATING_15 = UNIT_END + 0x4D5,	//  Size: 24, Type: Int32, Flags: 2
        PLAYER_FIELD_COMBAT_RATING_16 = UNIT_END + 0x4D6,	//  Size: 24, Type: Int32, Flags: 2
        PLAYER_FIELD_COMBAT_RATING_17 = UNIT_END + 0x4D7,	//  Size: 24, Type: Int32, Flags: 2
        PLAYER_FIELD_COMBAT_RATING_18 = UNIT_END + 0x4D8,	//  Size: 24, Type: Int32, Flags: 2
        PLAYER_FIELD_COMBAT_RATING_19 = UNIT_END + 0x4D9,	//  Size: 24, Type: Int32, Flags: 2
        PLAYER_FIELD_COMBAT_RATING_20 = UNIT_END + 0x4DA,	//  Size: 24, Type: Int32, Flags: 2
        PLAYER_FIELD_COMBAT_RATING_21 = UNIT_END + 0x4DB,	//  Size: 24, Type: Int32, Flags: 2
        PLAYER_FIELD_COMBAT_RATING_22 = UNIT_END + 0x4DC,	//  Size: 24, Type: Int32, Flags: 2
        PLAYER_FIELD_COMBAT_RATING_23 = UNIT_END + 0x4DD,	//  Size: 24, Type: Int32, Flags: 2
        PLAYER_FIELD_ARENA_TEAM_INFO_1_1 = UNIT_END + 0x4DE,	//  Size: 18, Type: Int32, Flags: 2
        PLAYER_FIELD_ARENA_TEAM_INFO_1_01 = UNIT_END + 0x4DF,	//  Size: 18, Type: Int32, Flags: 2
        PLAYER_FIELD_ARENA_TEAM_INFO_1_02 = UNIT_END + 0x4E0,	//  Size: 18, Type: Int32, Flags: 2
        PLAYER_FIELD_ARENA_TEAM_INFO_1_03 = UNIT_END + 0x4E1,	//  Size: 18, Type: Int32, Flags: 2
        PLAYER_FIELD_ARENA_TEAM_INFO_1_04 = UNIT_END + 0x4E2,	//  Size: 18, Type: Int32, Flags: 2
        PLAYER_FIELD_ARENA_TEAM_INFO_1_05 = UNIT_END + 0x4E3,	//  Size: 18, Type: Int32, Flags: 2
        PLAYER_FIELD_ARENA_TEAM_INFO_1_06 = UNIT_END + 0x4E4,	//  Size: 18, Type: Int32, Flags: 2
        PLAYER_FIELD_ARENA_TEAM_INFO_1_07 = UNIT_END + 0x4E5,	//  Size: 18, Type: Int32, Flags: 2
        PLAYER_FIELD_ARENA_TEAM_INFO_1_08 = UNIT_END + 0x4E6,	//  Size: 18, Type: Int32, Flags: 2
        PLAYER_FIELD_ARENA_TEAM_INFO_1_09 = UNIT_END + 0x4E7,	//  Size: 18, Type: Int32, Flags: 2
        PLAYER_FIELD_ARENA_TEAM_INFO_1_10 = UNIT_END + 0x4E8,	//  Size: 18, Type: Int32, Flags: 2
        PLAYER_FIELD_ARENA_TEAM_INFO_1_11 = UNIT_END + 0x4E9,	//  Size: 18, Type: Int32, Flags: 2
        PLAYER_FIELD_ARENA_TEAM_INFO_1_12 = UNIT_END + 0x4EA,	//  Size: 18, Type: Int32, Flags: 2
        PLAYER_FIELD_ARENA_TEAM_INFO_1_13 = UNIT_END + 0x4EB,	//  Size: 18, Type: Int32, Flags: 2
        PLAYER_FIELD_ARENA_TEAM_INFO_1_14 = UNIT_END + 0x4EC,	//  Size: 18, Type: Int32, Flags: 2
        PLAYER_FIELD_ARENA_TEAM_INFO_1_15 = UNIT_END + 0x4ED,	//  Size: 18, Type: Int32, Flags: 2
        PLAYER_FIELD_ARENA_TEAM_INFO_1_16 = UNIT_END + 0x4EE,	//  Size: 18, Type: Int32, Flags: 2
        PLAYER_FIELD_ARENA_TEAM_INFO_1_17 = UNIT_END + 0x4EF,	//  Size: 18, Type: Int32, Flags: 2
        PLAYER_FIELD_HONOR_CURRENCY = UNIT_END + 0x4F0,	//  Size: 1, Type: Int32, Flags: 2
        PLAYER_FIELD_ARENA_CURRENCY = UNIT_END + 0x4F1,	//  Size: 1, Type: Int32, Flags: 2
        PLAYER_FIELD_MOD_MANA_REGEN = UNIT_END + 0x4F2,	//  Size: 1, Type: Float, Flags: 2
        PLAYER_FIELD_MOD_MANA_REGEN_INTERRUPT = UNIT_END + 0x4F3,	//  Size: 1, Type: Float, Flags: 2
        PLAYER_FIELD_MAX_LEVEL = UNIT_END + 0x4F4,	//  Size: 1, Type: Int32, Flags: 2
        PLAYER_FIELD_DAILY_QUESTS_1 = UNIT_END + 0x4F5,	//  Size: 25, Type: Int32, Flags: 2
        PLAYER_FIELD_DAILY_QUESTS_01 = UNIT_END + 0x4F6,	//  Size: 25, Type: Int32, Flags: 2
        PLAYER_FIELD_DAILY_QUESTS_02 = UNIT_END + 0x4F7,	//  Size: 25, Type: Int32, Flags: 2
        PLAYER_FIELD_DAILY_QUESTS_03 = UNIT_END + 0x4F8,	//  Size: 25, Type: Int32, Flags: 2
        PLAYER_FIELD_DAILY_QUESTS_04 = UNIT_END + 0x4F9,	//  Size: 25, Type: Int32, Flags: 2
        PLAYER_FIELD_DAILY_QUESTS_05 = UNIT_END + 0x4FA,	//  Size: 25, Type: Int32, Flags: 2
        PLAYER_FIELD_DAILY_QUESTS_06 = UNIT_END + 0x4FB,	//  Size: 25, Type: Int32, Flags: 2
        PLAYER_FIELD_DAILY_QUESTS_07 = UNIT_END + 0x4FC,	//  Size: 25, Type: Int32, Flags: 2
        PLAYER_FIELD_DAILY_QUESTS_08 = UNIT_END + 0x4FD,	//  Size: 25, Type: Int32, Flags: 2
        PLAYER_FIELD_DAILY_QUESTS_09 = UNIT_END + 0x4FE,	//  Size: 25, Type: Int32, Flags: 2
        PLAYER_FIELD_DAILY_QUESTS_10 = UNIT_END + 0x4FF,	//  Size: 25, Type: Int32, Flags: 2
        PLAYER_FIELD_DAILY_QUESTS_11 = UNIT_END + 0x500,	//  Size: 25, Type: Int32, Flags: 2
        PLAYER_FIELD_DAILY_QUESTS_12 = UNIT_END + 0x501,	//  Size: 25, Type: Int32, Flags: 2
        PLAYER_FIELD_DAILY_QUESTS_13 = UNIT_END + 0x502,	//  Size: 25, Type: Int32, Flags: 2
        PLAYER_FIELD_DAILY_QUESTS_14 = UNIT_END + 0x503,	//  Size: 25, Type: Int32, Flags: 2
        PLAYER_FIELD_DAILY_QUESTS_15 = UNIT_END + 0x504,	//  Size: 25, Type: Int32, Flags: 2
        PLAYER_FIELD_DAILY_QUESTS_16 = UNIT_END + 0x505,	//  Size: 25, Type: Int32, Flags: 2
        PLAYER_FIELD_DAILY_QUESTS_17 = UNIT_END + 0x506,	//  Size: 25, Type: Int32, Flags: 2
        PLAYER_FIELD_DAILY_QUESTS_18 = UNIT_END + 0x507,	//  Size: 25, Type: Int32, Flags: 2
        PLAYER_FIELD_DAILY_QUESTS_19 = UNIT_END + 0x508,	//  Size: 25, Type: Int32, Flags: 2
        PLAYER_FIELD_DAILY_QUESTS_20 = UNIT_END + 0x509,	//  Size: 25, Type: Int32, Flags: 2
        PLAYER_FIELD_DAILY_QUESTS_21 = UNIT_END + 0x50A,	//  Size: 25, Type: Int32, Flags: 2
        PLAYER_FIELD_DAILY_QUESTS_22 = UNIT_END + 0x50B,	//  Size: 25, Type: Int32, Flags: 2
        PLAYER_FIELD_DAILY_QUESTS_23 = UNIT_END + 0x50C,	//  Size: 25, Type: Int32, Flags: 2
        PLAYER_FIELD_DAILY_QUESTS_24 = UNIT_END + 0x50D,	//  Size: 25, Type: Int32, Flags: 2
        PLAYER_END = UNIT_END + 0x50E,

        //OBJECT:
        OBJECT_FIELD_CREATED_BY = OBJECT_END + 0x000,	//  Size: 2, Type: GUID, Flags: 1

        //GAMEOBJECT:
        GAMEOBJECT_DISPLAYID = OBJECT_END + 0x002,	//  Size: 1, Type: Int32, Flags: 1
        GAMEOBJECT_FLAGS = OBJECT_END + 0x003,	//  Size: 1, Type: Int32, Flags: 1
        GAMEOBJECT_ROTATION = OBJECT_END + 0x004,	//  Size: 4, Type: Float, Flags: 1
        GAMEOBJECT_ROTATION_01 = OBJECT_END + 0x005,	//  Size: 4, Type: Float, Flags: 1
        GAMEOBJECT_ROTATION_02 = OBJECT_END + 0x006,	//  Size: 4, Type: Float, Flags: 1
        GAMEOBJECT_ROTATION_03 = OBJECT_END + 0x007,	//  Size: 4, Type: Float, Flags: 1
        GAMEOBJECT_STATE = OBJECT_END + 0x008,	//  Size: 1, Type: Int32, Flags: 1
        GAMEOBJECT_POS_X = OBJECT_END + 0x009,	//  Size: 1, Type: Float, Flags: 1
        GAMEOBJECT_POS_Y = OBJECT_END + 0x00A,	//  Size: 1, Type: Float, Flags: 1
        GAMEOBJECT_POS_Z = OBJECT_END + 0x00B,	//  Size: 1, Type: Float, Flags: 1
        GAMEOBJECT_FACING = OBJECT_END + 0x00C,	//  Size: 1, Type: Float, Flags: 1
        GAMEOBJECT_DYN_FLAGS = OBJECT_END + 0x00D,	//  Size: 1, Type: Int32, Flags: 256
        GAMEOBJECT_FACTION = OBJECT_END + 0x00E,	//  Size: 1, Type: Int32, Flags: 1
        GAMEOBJECT_TYPE_ID = OBJECT_END + 0x00F,	//  Size: 1, Type: Int32, Flags: 1
        GAMEOBJECT_LEVEL = OBJECT_END + 0x010,	//  Size: 1, Type: Int32, Flags: 1
        GAMEOBJECT_ARTKIT = OBJECT_END + 0x011,	//  Size: 1, Type: Int32, Flags: 1
        GAMEOBJECT_ANIMPROGRESS = OBJECT_END + 0x012,	//  Size: 1, Type: Int32, Flags: 256
        GAMEOBJECT_PADDING = OBJECT_END + 0x013,	//  Size: 1, Type: Int32, Flags: 0
        GAMEOBJECT_END = OBJECT_END + 0x014,

        //DYNAMICOBJECT:
        DYNAMICOBJECT_CASTER = OBJECT_END + 0x000,	//  Size: 2, Type: GUID, Flags: 1
        DYNAMICOBJECT_BYTES = OBJECT_END + 0x002,	//  Size: 1, Type: Bytes, Flags: 1
        DYNAMICOBJECT_SPELLID = OBJECT_END + 0x003,	//  Size: 1, Type: Int32, Flags: 1
        DYNAMICOBJECT_RADIUS = OBJECT_END + 0x004,	//  Size: 1, Type: Float, Flags: 1
        DYNAMICOBJECT_POS_X = OBJECT_END + 0x005,	//  Size: 1, Type: Float, Flags: 1
        DYNAMICOBJECT_POS_Y = OBJECT_END + 0x006,	//  Size: 1, Type: Float, Flags: 1
        DYNAMICOBJECT_POS_Z = OBJECT_END + 0x007,	//  Size: 1, Type: Float, Flags: 1
        DYNAMICOBJECT_FACING = OBJECT_END + 0x008,	//  Size: 1, Type: Float, Flags: 1
        DYNAMICOBJECT_CASTTIME = OBJECT_END + 0x009,	//  Size: 1, Type: Int32, Flags: 1
        DYNAMICOBJECT_END = OBJECT_END + 0x00A,

        //CORPSE:
        CORPSE_FIELD_OWNER = OBJECT_END + 0x000,	//  Size: 2, Type: GUID, Flags: 1
        CORPSE_FIELD_FACING = OBJECT_END + 0x002,	//  Size: 1, Type: Float, Flags: 1
        CORPSE_FIELD_POS_X = OBJECT_END + 0x003,	//  Size: 1, Type: Float, Flags: 1
        CORPSE_FIELD_POS_Y = OBJECT_END + 0x004,	//  Size: 1, Type: Float, Flags: 1
        CORPSE_FIELD_POS_Z = OBJECT_END + 0x005,	//  Size: 1, Type: Float, Flags: 1
        CORPSE_FIELD_DISPLAY_ID = OBJECT_END + 0x006,	//  Size: 1, Type: Int32, Flags: 1
        CORPSE_FIELD_ITEM = OBJECT_END + 0x007,	//  Size: 19, Type: Int32, Flags: 1
        CORPSE_FIELD_ITEM_01 = OBJECT_END + 0x008,	//  Size: 19, Type: Int32, Flags: 1
        CORPSE_FIELD_ITEM_02 = OBJECT_END + 0x009,	//  Size: 19, Type: Int32, Flags: 1
        CORPSE_FIELD_ITEM_03 = OBJECT_END + 0x00A,	//  Size: 19, Type: Int32, Flags: 1
        CORPSE_FIELD_ITEM_04 = OBJECT_END + 0x00B,	//  Size: 19, Type: Int32, Flags: 1
        CORPSE_FIELD_ITEM_05 = OBJECT_END + 0x00C,	//  Size: 19, Type: Int32, Flags: 1
        CORPSE_FIELD_ITEM_06 = OBJECT_END + 0x00D,	//  Size: 19, Type: Int32, Flags: 1
        CORPSE_FIELD_ITEM_07 = OBJECT_END + 0x00E,	//  Size: 19, Type: Int32, Flags: 1
        CORPSE_FIELD_ITEM_08 = OBJECT_END + 0x00F,	//  Size: 19, Type: Int32, Flags: 1
        CORPSE_FIELD_ITEM_09 = OBJECT_END + 0x010,	//  Size: 19, Type: Int32, Flags: 1
        CORPSE_FIELD_ITEM_10 = OBJECT_END + 0x011,	//  Size: 19, Type: Int32, Flags: 1
        CORPSE_FIELD_ITEM_11 = OBJECT_END + 0x012,	//  Size: 19, Type: Int32, Flags: 1
        CORPSE_FIELD_ITEM_12 = OBJECT_END + 0x013,	//  Size: 19, Type: Int32, Flags: 1
        CORPSE_FIELD_ITEM_13 = OBJECT_END + 0x014,	//  Size: 19, Type: Int32, Flags: 1
        CORPSE_FIELD_ITEM_14 = OBJECT_END + 0x015,	//  Size: 19, Type: Int32, Flags: 1
        CORPSE_FIELD_ITEM_15 = OBJECT_END + 0x016,	//  Size: 19, Type: Int32, Flags: 1
        CORPSE_FIELD_ITEM_16 = OBJECT_END + 0x017,	//  Size: 19, Type: Int32, Flags: 1
        CORPSE_FIELD_ITEM_17 = OBJECT_END + 0x018,	//  Size: 19, Type: Int32, Flags: 1
        CORPSE_FIELD_ITEM_18 = OBJECT_END + 0x019,	//  Size: 19, Type: Int32, Flags: 1
        CORPSE_FIELD_BYTES_1 = OBJECT_END + 0x01A,	//  Size: 1, Type: Bytes, Flags: 1
        CORPSE_FIELD_BYTES_2 = OBJECT_END + 0x01B,	//  Size: 1, Type: Bytes, Flags: 1
        CORPSE_FIELD_GUILD = OBJECT_END + 0x01C,	//  Size: 1, Type: Int32, Flags: 1
        CORPSE_FIELD_FLAGS = OBJECT_END + 0x01D,	//  Size: 1, Type: Int32, Flags: 1
        CORPSE_FIELD_DYNAMIC_FLAGS = OBJECT_END + 0x01E,	//  Size: 1, Type: Int32, Flags: 256
        CORPSE_END = OBJECT_END + 0x01F,
        FIELDS_MAX
    };
}
