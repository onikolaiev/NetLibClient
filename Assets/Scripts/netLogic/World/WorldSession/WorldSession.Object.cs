using System.Collections.Generic;
using System.Text;
using System.Timers;
using System.Collections;
using System;
using netLogic.Network;
using netLogic.Shared;
using netLogic.Constants;
using System.IO;
using UnityEngine;
using netLogic;
using System.Threading;



namespace netLogic
{
    public partial class WorldSession
    {
        #region Update object
        [PacketHandlerAtribute(WorldServerOpCode.SMSG_COMPRESSED_UPDATE_OBJECT)]
		public void HandleCompressedObjectUpdate(PacketIn packet)
		{
			try
			{
				Int32 size = packet.ReadInt32();
                byte[] decomped = netLogic.Shared.Compression.Decompress(size, packet.ReadRemaining());
				packet = new PacketIn(decomped, 1);
                HandleObjectUpdate(packet);
			}
			catch(Exception ex)
			{
				Log.WriteLine(netLogic.Shared.LogType.Error, "{1} \n {0}", ex.StackTrace, ex.Message);
			}
		}

		[PacketHandlerAtribute(WorldServerOpCode.SMSG_UPDATE_OBJECT)]
		public void HandleObjectUpdate(PacketIn packet)
		{
                UInt32 UpdateBlocks = packet.ReadUInt32();

                for(int allBlocks = 0; allBlocks < UpdateBlocks; allBlocks++)
                {
                    UpdateTypes type = (UpdateTypes)packet.ReadByte();
                    
                   
                    switch (type)
                    {
                        case UpdateTypes.UPDATETYPE_VALUES:
                            ParseValues(packet);
                            break;
                        case UpdateTypes.UPDATETYPE_MOVEMENT:
                            ParseMovement(packet);
                            break;
                        case UpdateTypes.UPDATETYPE_CREATE_OBJECT:
                        case UpdateTypes.UPDATETYPE_CREATE_OBJECT2:
                            ParseCreateObjects(packet);
                            break;
                        case UpdateTypes.UPDATETYPE_OUT_OF_RANGE_OBJECTS:
                            //ParseOutOfRangeObjects(packet);
                            break;
                        case UpdateTypes.UPDATETYPE_NEAR_OBJECTS:
                            //ParseNearObjects(packet);
                            break;
                        default:
                            Console.WriteLine("Unknown updatetype {0}", type);
                            break;
                    }

                    #region
                    /*switch (type)
                    {
                        case UpdateType.Values:
                            Object getObject;
                            var guidv = packet.ReadPackedGuid();
                            updateGuid = new WoWGuid(packet.ReadUInt64());
                            updateId = (ObjectTypes)packet.ReadByte();
                            //updategui = packet.ReadPackedGuid();

                            if (netLogicCore.ObjectMgr.objectExists(updateGuid))
                            {
                                getObject = netLogicCore.ObjectMgr.getObject(updateGuid);
                            }
                            else
                            {
                                getObject = new Object(updateGuid);
                                netLogicCore.ObjectMgr.newObject(getObject);
                            }

                            Log.WriteLine(LogType.Normal, "Handling Fields Update for object: {0}", getObject.GUID.ToString());

                            getObject.Fields = new UInt32[2000];

                            HandleUpdateObjectFieldBlock(packet, getObject);


                            netLogicCore.ObjectMgr.updateObject(getObject);
                            QueryName(getObject.GUID);
                            break;



                        case UpdateType.Movement:
                            ParseMovement(packet);
                            break;
                        case UpdateType.Create:
                        case UpdateType.CreateSelf:
                            //var guidc = packet.ReadPackedGuid();
                            updateGuid = new WoWGuid(packet.ReadUInt64());
                            updateId = (ObjectTypes)packet.ReadByte();

                            //fCount = GetUpdateFieldsCount(updateId);
                            //fCount = GetUpdateFieldsCount(updateId);

                            if (netLogicCore.ObjectMgr.objectExists(updateGuid))
                                netLogicCore.ObjectMgr.delObject(updateGuid);

                            Object newObject = new Object(updateGuid);

                            newObject.Fields = new UInt32[2000];

                            netLogicCore.ObjectMgr.newObject(newObject);

                            HandleUpdateMovementBlock(packet, newObject);

                            HandleUpdateObjectFieldBlock(packet, newObject);

                            netLogicCore.ObjectMgr.updateObject(newObject);
                            QueryName(newObject.GUID);

                            Log.WriteLine(LogType.Normal, "Handling Creation of object: {0}", newObject.GUID.ToString());
                            break;
                        
                        case UpdateType.OutOfRange:
                            fCount = packet.ReadByte();
                            for (int j = 0; j < fCount; j++)
                            {
                                WoWGuid guid = new WoWGuid(packet.ReadUInt64());
                                Log.WriteLine(LogType.Normal, "Handling delete for object: {0}", guid.ToString());
                                if (netLogicCore.ObjectMgr.objectExists(guid))
                                    netLogicCore.ObjectMgr.delObject(guid);
                            }
                            break;
                    }*/
                    #endregion

                }
              
        }
        #endregion

        #region Creature query
        public void CreatureQuery(GameGuid guid, UInt32 entry)
        {
            PacketOut packet = new PacketOut(WorldServerOpCode.CMSG_CREATURE_QUERY);
            packet.Write(entry);
            packet.Write(guid.GetOldGuid());
            Send(packet);
        }

        [PacketHandlerAtribute(WorldServerOpCode.SMSG_CREATURE_QUERY_RESPONSE)]
        public void Handle_CreatureQuery(PacketIn packet)
        {
            Entry entry = new Entry();
            entry.entry = packet.ReadUInt32();
            entry.name = packet.ReadString();
            entry.blarg = packet.ReadBytes(3);
            entry.subname = packet.ReadString();
            entry.flags = packet.ReadUInt32();
            entry.subtype = packet.ReadUInt32();
            entry.family = packet.ReadUInt32();
            entry.rank = packet.ReadUInt32();

        /*    foreach (MMOObject obj in netLogicCore.ObjectMgr.WorldObjects)
            {
                if (obj.Fields != null)
                {
                    if (obj.Fields[(int)UpdateFieldsLoader.GetUpdateField(ObjectTypes.TYPEID_UNIT,"OBJECT_FIELD_ENTRY").Value] == entry.entry)
                    {
                        obj.Name = entry.name;
                        obj.Rank = entry.rank;
                        obj.Family = entry.family;
                        obj.SubType = entry.subtype;
                        //netLogicCore.ObjectMgr.updateObject(obj);
                    }
                }
            }*/




        }
        #endregion

        #region Name query

        public void QueryName(GameGuid guid)
        {
            PacketOut packet = new PacketOut(WorldServerOpCode.CMSG_NAME_QUERY);
            packet.Write(guid.GetOldGuid());
            Send(packet);
        }

        public void QueryName(UInt64 guid)
        {
            PacketOut packet = new PacketOut(WorldServerOpCode.CMSG_NAME_QUERY);
            packet.Write(guid);
            Send(packet);
        }

        [PacketHandlerAtribute(WorldServerOpCode.SMSG_NAME_QUERY_RESPONSE)]
        public void Handle_NameQuery(PacketIn packet)
        {
            byte mask = packet.ReadByte();
            GameGuid guid = new GameGuid(mask, packet.ReadBytes(GameGuid.BitCount8(mask)));

            packet.ReadByte();
            string name = packet.ReadString();
            packet.ReadByte();


            //Races Race = (Races)packet.ReadByte();
            //Gender Gender = (Gender)packet.ReadByte();
            //Classes Class = (Classes)packet.ReadByte();


          /*  if (netLogicCore.ObjectMgr.GetObject(guid) != null)    // Update existing Object
            {
                obj = netLogicCore.ObjectMgr.GetObject(guid);
                obj.Name = name;
                
               // netLogicCore.ObjectMgr.updateObject(obj);
            }*/
         /*    else                // Create new Object        -- FIXME: Add to new 'names only' list?
             {
                 obj = new MMOObject(guid);
                 obj.Name = name;
                 netLogicCore.ObjectMgr.newObject(obj);

                //  Process chat message if we looked them up now 
                 for (int i = 0; i < ChatQueued.Count; i++)
                 {
                     ChatQueue message = (ChatQueue)ChatQueued[i];
                     if (message.GUID == guid)
                     {
                         Log.WriteLine(netLogic.Shared.LogType.Chat, "[{1}] {0}", message.Message, name);
                         ChatQueued.Remove(message);
                     }
                 }

             }*/
        }


        #endregion

        #region Logic
        

        private void ParseValues(BinaryReader gr)
        {
            byte mask = gr.ReadByte();

            GameGuid guid = new GameGuid(mask, gr.ReadBytes(GameGuid.BitCount8(mask)));

            var wowobj = GetInstance().ObjMgr().GetObj(guid);
            if(wowobj)
                wowobj.ReadUpd(gr);
        }

        private void ParseMovement(BinaryReader gr)
        {
            byte mask = gr.ReadByte();

            GameGuid guid = new GameGuid(mask, gr.ReadBytes(GameGuid.BitCount8(mask)));

            GetInstance().ObjMgr().GetObj(guid).ReadMov(gr);
        
        }

        private void ParseCreateObjects(BinaryReader gr)
        {
            byte mask = gr.ReadByte();

            GameGuid guid = new GameGuid(mask, gr.ReadBytes(GameGuid.BitCount8(mask)));

            OBJECT_TYPE_ID objectTypeId = (OBJECT_TYPE_ID)gr.ReadByte();

            switch (objectTypeId)
            {
                case OBJECT_TYPE_ID.TYPEID_OBJECT:
                    Log.WriteLine(netLogic.Shared.LogType.Normal, "UPDATETYPE_CREATE_OBJECT Unknown updatetype {0}", objectTypeId);
                    break;

                case OBJECT_TYPE_ID.TYPEID_ITEM:
                    GameObject _goui = GameObject.CreatePrimitive(PrimitiveType.Capsule);
                    _goui.AddComponent<Item>();
                    Item item = _goui.GetComponent<Item>();
                    item.Create(guid);
                    GetInstance().ObjMgr().Add(item);
                    break;

                case OBJECT_TYPE_ID.TYPEID_CONTAINER:
                    GameObject _goub = GameObject.CreatePrimitive(PrimitiveType.Quad);
                    _goub.AddComponent<Bag>();
                    Bag b = _goub.GetComponent<Bag>();
                    b.Create(guid);
                    GetInstance().ObjMgr().Add(b);
                    break;

                case OBJECT_TYPE_ID.TYPEID_UNIT:
                    GameObject _unit = GameObject.CreatePrimitive(PrimitiveType.Sphere);
                    _unit.AddComponent<Unit>();
                    Unit u = _unit.GetComponent<Unit>();
                    u.Create(guid);
                    GetInstance().ObjMgr().Add(u);
                    
                    break;

                case OBJECT_TYPE_ID.TYPEID_PLAYER:
                    if (guid.GetOldGuid() == _myGUID.GetOldGuid()) // objmgr.Add() would cause quite some trouble if we added ourself again
                    {
                        break;
                    }
                    else
                    {
                        GameObject _goup = GameObject.CreatePrimitive(PrimitiveType.Cube);

                        _goup.AddComponent<Player>();
                        //if (netInstance._PlayerInited) Instantiate(_goup); else netInstance._PlayerInited = true;

                        Player p = _goup.GetComponent<Player>();
                        p.Create(guid);
                        GetInstance().ObjMgr().Add(p);
                        QueryName(guid);
                        break;
                    }
                    break;
                case OBJECT_TYPE_ID.TYPEID_GAMEOBJECT:
                    
                    GameObject _goug = GameObject.CreatePrimitive(PrimitiveType.Capsule);
                    _goug.AddComponent<GO>();
                    GO go = _goug.GetComponent<GO>();
                    go.Create(guid);
                    GetInstance().ObjMgr().Add(go);
              //      ObjectQuery(go.GetGUID(), go.GetEntry());
                    break;

                case OBJECT_TYPE_ID.TYPEID_DYNAMICOBJECT:
                    DynamicObject d = new DynamicObject();
                    d.Create(guid);
                    GetInstance().ObjMgr().Add(d);
                    break;
                case OBJECT_TYPE_ID.TYPEID_CORPSE:
                    Corpse c = new Corpse();
                    c.Create(guid);
                    GetInstance().ObjMgr().Add(c);
                    break;

                default:
                    Log.WriteLine(netLogic.Shared.LogType.Normal, "Unknown updatetype {0}", objectTypeId);
                    break;
            }

            Object _o = GetInstance().ObjMgr().GetObj(guid);

            _o.ReadMov(gr);
            if (_o)
            {
                _o.ReadUpd(gr);
                if (_o.IsUnit())
                {
                    CreatureQuery(_o.GetGUID(), _o.GetEntry());
                }
                else if (_o.IsGameObject())
                {
                    ObjectQuery(_o.GetGUID(), _o.GetEntry());
                }

            }
            else
            {
                Log.WriteLine(netLogic.Shared.LogType.Debug, "Unknown updatetype {0}", objectTypeId);
            }






        }



      /*  private void ParseCreateObjects(BinaryReader gr)
        {
            byte mask = gr.ReadByte();
            GameGuid guid = new GameGuid(mask, gr.ReadBytes(GameGuid.BitCount8(mask)));

            OBJECT_TYPE_ID objectTypeId = (OBJECT_TYPE_ID)gr.ReadByte();
            GameObject _unit1 = new GameObject();
            _unit1.AddComponent<Unit>();
            Unit u1 = _unit1.GetComponent<Unit>();
            u1.Create(guid);
            u1.Read(gr);
            var mInfo = MovementInfo.Read(gr);
            u1.Read(gr);
            
            
            switch (objectTypeId)
            {
                case OBJECT_TYPE_ID.TYPEID_OBJECT:
                    Log.WriteLine(netLogic.Shared.LogType.Normal, "UPDATETYPE_CREATE_OBJECT Unknown updatetype {0}", objectTypeId);
                    break;

                case OBJECT_TYPE_ID.TYPEID_ITEM:
                        GameObject _goui = GameObject.CreatePrimitive(PrimitiveType.Capsule);
                        _goui.AddComponent<Item>();
                        Item item = _goui.GetComponent<Item>();
                        item.Create(guid);
                        item.Read(gr);
                        netLogicCore.objMgr.Add(item);
                    break;

                case OBJECT_TYPE_ID.TYPEID_CONTAINER:
                        GameObject _goub = GameObject.CreatePrimitive(PrimitiveType.Quad);
                        _goub.AddComponent<Bag>();
                        Bag b =  _goub.GetComponent<Bag>();
                        b.Create(guid);
                        b.Read(gr);
                        netLogicCore.objMgr.Add(b);
                    break;

                case OBJECT_TYPE_ID.TYPEID_UNIT:
                        GameObject _unit = new GameObject();
                        _unit.AddComponent<Unit>();
                        Unit u=_unit.GetComponent<Unit>();
                        u.Create(guid);
                        u.Read(gr);
                        netLogicCore.objMgr.Add(u);
                        CreatureQuery(u.GetGUID(), u.GetEntry());
                    break;

                case OBJECT_TYPE_ID.TYPEID_PLAYER:
                        GameObject _goup = GameObject.CreatePrimitive(PrimitiveType.Sphere);
                        _goup.AddComponent<Player>();
                        Player p = _goup.GetComponent<Player>();
                        p.Create(guid);
                        p.Read(gr);
                        netLogicCore.objMgr.Add(p);
                        QueryName(guid);
                    break;

                case OBJECT_TYPE_ID.TYPEID_GAMEOBJECT:
                        GameObject _goug = GameObject.CreatePrimitive(PrimitiveType.Capsule);
                        _goug.AddComponent<GO>();
                        GO go =  _goug.GetComponent<GO>();
                        go.Read(gr);
                        netLogicCore.objMgr.Add(go);
                        ObjectQuery(go.GetGUID(), go.GetEntry());
                    break;

                case OBJECT_TYPE_ID.TYPEID_DYNAMICOBJECT:
                        DynamicObject d = new DynamicObject();
                        d.Create(guid);
                        d.Read(gr);
                        netLogicCore.objMgr.Add(d);
                    break;
                case OBJECT_TYPE_ID.TYPEID_CORPSE:
                        Corpse c = new Corpse();
                        c.Create(guid);
                        c.Read(gr);
                        netLogicCore.objMgr.Add(c);
                    break;

                default:
                    Log.WriteLine(netLogic.Shared.LogType.Normal, "Unknown updatetype {0}", objectTypeId);
                    break;
            }

         //   MovementInfo.Read(objectTypeId, guid, gr);
           
            


 
          
        }
        







        */



        public void ObjectQuery(GameGuid guid, UInt32 entry)
        {
            PacketOut packet = new PacketOut(WorldServerOpCode.CMSG_GAMEOBJECT_QUERY);
            packet.Write(entry);
            packet.Write(guid.GetOldGuid());
            Send(packet);
        }

        public uint GetUpdateFieldsCount(OBJECT_TYPE_ID updateId)
        {
            switch (updateId)
            {
                case OBJECT_TYPE_ID.TYPEID_OBJECT:
                    return (uint)UpdateFieldsLoader.OBJECT_END;

                case OBJECT_TYPE_ID.TYPEID_GAMEOBJECT:
                    return (uint)UpdateFieldsLoader.GO_END;

                case OBJECT_TYPE_ID.TYPEID_UNIT:
                    return (uint)UpdateFieldsLoader.UNIT_END;

                case OBJECT_TYPE_ID.TYPEID_PLAYER:
                    return (uint)UpdateFieldsLoader.PLAYER_END;

                case OBJECT_TYPE_ID.TYPEID_ITEM:
                    return (uint)UpdateFieldsLoader.ITEM_END;

                case OBJECT_TYPE_ID.TYPEID_CONTAINER:
                    return (uint)UpdateFieldsLoader.CONTAINER_END;

                case OBJECT_TYPE_ID.TYPEID_DYNAMICOBJECT:
                    return (uint)UpdateFieldsLoader.DO_END;

                case OBJECT_TYPE_ID.TYPEID_CORPSE:
                    return (uint)UpdateFieldsLoader.CORPSE_END;
                default:

                    return 0;
            }
        }

        #endregion
        [PacketHandlerAtribute(WorldServerOpCode.SMSG_DESTROY_OBJECT)]
        public void DestroyObject(PacketIn packet)
        {
            //Log.WriteLine(netLogic.Shared.LogType.Network, "SMSG_DESTROY_OBJECT {0}", packet.PacketId.RawId);
            GameGuid guid = new GameGuid(packet.ReadUInt64());
            //Destroy(GameObject.Find(guid.ToString()));
            GetInstance().ObjMgr().Remove(guid);

        }




     /*   public void HandleUpdateMovementBlock(PacketIn packet, Object newObject)
        {
            UInt16 flags = packet.ReadUInt16();


            if((flags & 0x20) >= 1)
            {
                UInt32 flags2 = packet.ReadUInt32();
                UInt16 unk1 = packet.ReadUInt16();
                UInt32 unk2 = packet.ReadUInt32();
                newObject.Movement.Position = new Coordinate(packet.ReadFloat(), packet.ReadFloat(), packet.ReadFloat(), packet.ReadFloat());

                if ((flags2 & 0x200) >= 1)
                {
                    packet.ReadBytes(21); //transporter
                }

                if (((flags2 & 0x2200000) >= 1) || ((unk1 & 0x20) >= 1))
                {
                    packet.ReadBytes(4); // pitch
                }

                packet.ReadBytes(4); //lastfalltime

                if ((flags2 & 0x1000) >= 1)
                {
                    packet.ReadBytes(16); // skip 4 floats
                }

                if ((flags2 & 0x4000000) >= 1)
                {
                    packet.ReadBytes(4); // skip 1 float
                }

                packet.ReadBytes(32); // all of speeds

                if ((flags2 & 0x8000000) >= 1) //spline ;/
                {
                    UInt32 splineFlags = packet.ReadUInt32();

                    if ((splineFlags & 0x00020000) >= 1)
                    {
                        packet.ReadBytes(4); // skip 1 float
                    }
                    else
                    {
                        if ((splineFlags & 0x00010000) >= 1)
                        {
                            packet.ReadBytes(4); // skip 1 float
                        }
                        else if ((splineFlags & 0x00008000) >= 1)
                        {
                            packet.ReadBytes(12); // skip 3 float
                        }
                    }

                    packet.ReadBytes(28); // skip 8 float

                    UInt32 splineCount = packet.ReadUInt32();

                    for (UInt32 j = 0; j < splineCount; j++)
                    {
                        packet.ReadBytes(12); // skip 3 float
                    }

                    packet.ReadBytes(13);

                }
            }

            else if ((flags & 0x100) >= 1)
            {
                packet.ReadBytes(40);
            }
            else if ((flags & 0x40) >= 1)
            {
                newObject.Movement.Position = new Coordinate(packet.ReadFloat(), packet.ReadFloat(), packet.ReadFloat(), packet.ReadFloat());
            }

            if ((flags & 0x8) >= 1)
            {
                packet.ReadBytes(4);
            }

            if ((flags & 0x10) >= 1)
            {
                packet.ReadBytes(4);
            }

            if ((flags & 0x04) >= 1)
            {
                packet.ReadBytes(8);
            }

            if ((flags & 0x2) >= 1)
            {
                packet.ReadBytes(4);
            }

            if ((flags & 0x80) >= 1)
            {
                packet.ReadBytes(8);
            }

            if ((flags & 0x200) >= 1)
            {
                packet.ReadBytes(8);
            }
        }

        public void HandleUpdateObjectFieldBlock(PacketIn packet, MMOObject newObject)
        {
            uint lenght = packet.ReadByte();
            
            UpdateMask UpdateMask = new UpdateMask();
            UpdateMask.SetCount((ushort)(lenght));
            UpdateMask.SetMask(packet.ReadBytes((int)lenght * 4), (ushort)lenght);
            newObject.Fields = new UInt32[UpdateMask.GetCount()];
            try
            {
                for (int i = 0; i < UpdateMask.GetCount(); i++)
                {
                    if (!UpdateMask.GetBit((ushort)i))
                    {
                        UInt32 val = packet.ReadUInt32();
                        newObject.SetField(i, val);
                      //  Log.WriteLine(LogType.Normal, "Update Field: {0} = {1}", (UpdateFieldsLoader.UpdateFields)i, val);
                    }
                }
            }
            catch (Exception ex)
            { }
        }

        */
    }

}

