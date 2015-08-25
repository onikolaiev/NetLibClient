using netLogic.Constants;
using netLogic.Network;
using netLogic.Shared;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UnityEngine;


namespace netLogic
{
    public class ObjMgr : MonoBehaviour
    {
        public readonly Dictionary<UInt32, ItemProto> ItemProtoMap;
        public readonly Dictionary<UInt32, CreatureTemplate> CreatureTemplateMap;
        public readonly Dictionary<UInt32, GameobjectTemplate> GOTemplateMap;
        public readonly Dictionary<GameGuid, Object> ObjectMap;

        List<UInt32> _noitem;
        List<UInt32> _reqpnames;
        List<UInt32> _nocreature;
        List<UInt32> _nogameobj;


        void Update()
        { 
             
   




        
        }



        public ObjMgr()
        { 
            ItemProtoMap = new Dictionary<UInt32, ItemProto>();
            CreatureTemplateMap = new Dictionary<UInt32, CreatureTemplate>();
            GOTemplateMap = new Dictionary<UInt32, GameobjectTemplate>();
            ObjectMap = new Dictionary<GameGuid, Object>();
            _noitem = new List<uint>();
            _reqpnames = new List<uint>();
            _nocreature = new List<uint>();
            _nogameobj = new List<uint>();
        
        }

        ~ObjMgr()
        {
            RemoveAll();
        }

        public void RemoveAll()
        {
            ItemProtoMap.Clear();
            CreatureTemplateMap.Clear();
            GOTemplateMap.Clear();
            ObjectMap.Clear();
        }

        public void Remove(GameGuid _guid)
        {
            foreach (KeyValuePair<GameGuid, Object> entry in ObjectMap)
            {
                if (entry.Key.GetOldGuid() == _guid.GetOldGuid())
                {
                    ObjectMap.Remove(entry.Key);
                    Destroy(GameObject.Find(entry.Value.transform.name));
                }


            }
                
        
        }

        #region -- Object part --

        public void Add(Object _o)
        {
            try
            {

                    ObjectMap.Add(_o.GetGUID(), _o);// ...assign new one...

            }
            catch
            {
                Log.WriteLine(netLogic.Shared.LogType.Normal, "Unknown updatetype {0}", _o.GetGUID());
                   
            }
        }

        public Object GetObj(GameGuid _guid)
        {

            foreach (KeyValuePair<GameGuid, Object> o in ObjectMap)
            {
                if (o.Key.GetOldGuid() == _guid.GetOldGuid())
                {
                    switch (o.Value.Get_TypeId())
                    {
                        case OBJECT_TYPE_ID.TYPEID_OBJECT:
                        //    Log.WriteLine(netLogic.Shared.LogType.Normal, "UPDATETYPE_CREATE_OBJECT Unknown updatetype {0}", objectTypeId);
                            break;

                        case OBJECT_TYPE_ID.TYPEID_ITEM:
                            return (Item)o.Value;
                            

                        case OBJECT_TYPE_ID.TYPEID_CONTAINER:
                            return (Bag)o.Value;
                            

                        case OBJECT_TYPE_ID.TYPEID_UNIT:
                            return (Unit)o.Value;
                           

                        case OBJECT_TYPE_ID.TYPEID_PLAYER:
                            return (Player)o.Value;
                            

                        case OBJECT_TYPE_ID.TYPEID_GAMEOBJECT:
                            return (GO)o.Value;
                            

                        case OBJECT_TYPE_ID.TYPEID_DYNAMICOBJECT:
                            return (DynamicObject)o.Value;
                            
                        case OBJECT_TYPE_ID.TYPEID_CORPSE:
                            return (Corpse)o.Value;
                            

                        default:
                            return new Object();
                            
                    }
                }
            }
            return null;
        }

        public int GetObjectCount() { return ObjectMap.Count(); }

        public UInt32 AssignNameToObj(UInt32 _entry, OBJECT_TYPE_ID _type, string _name)
        {
            UInt32 changed = 0;
            foreach (KeyValuePair<GameGuid, Object> o in ObjectMap)
            {
                if (o.Value.GetEntry() == _entry && o.Value.Get_TypeId()==_type)
                {
                    o.Value.SetName(name);
                    changed++;
                }
            }
            return changed;
        
        }

        public Dictionary<GameGuid, Object> GetObjectStorage() { return ObjectMap; }

        #endregion

        #region -- Item part --

        public void Add(ItemProto _proto)
        {
            ItemProtoMap.Add(_proto.Id, _proto);
        }

        public ItemProto GetItemProto(UInt32 _entry)
        {
            ItemProto _ip = new ItemProto();
            foreach (KeyValuePair<UInt32, ItemProto> o in ItemProtoMap)
            {
                if (o.Value.Id == _entry)
                {
                    return o.Value;

                }
            }
            return _ip;
        }

        public int GetItemProtoCount() { return ItemProtoMap.Count(); }

        public Dictionary<UInt32, ItemProto> GetItemProtoStorage() { return ItemProtoMap; }

        public void AddNonExistentItem(UInt32 _id)
        {
            _noitem.Add(_id);
        }

        public bool ItemNonExistent(UInt32 _id)
        {
          //  return _noitem.Find(x => x == _id) !=null;
            return true;
        }

        #endregion

        #region -- Creature part --

        public void Add(CreatureTemplate _cr)
        {
            CreatureTemplateMap.Add(_cr.entry, _cr);
        }

        public CreatureTemplate GetCreatureTemplate(UInt32 _entry)
        {
            CreatureTemplate _cr = new CreatureTemplate();
            foreach (KeyValuePair<UInt32, CreatureTemplate> o in CreatureTemplateMap)
            {
                if (o.Value.entry == _entry)
                {
                    return o.Value;

                }
            }
            return _cr;
        }

        public int GetCreatureTemplateCount() { return CreatureTemplateMap.Count(); }

        public Dictionary<UInt32, CreatureTemplate> GetCreatureTemplateStorage() { return CreatureTemplateMap; }

        public void AddNonExistentCreature(UInt32 _id)
        {
            _nocreature.Add(_id);
        }

        public bool CreatureNonExistent(UInt32 _id)
        {
           // return _nocreature.Find(x => x == _id) != null;
            return true;
        }

        #endregion

        #region -- Gameobject part --

        public void Add(GameobjectTemplate _cr)
        {
            GOTemplateMap.Add(_cr.entry, _cr);
        }

        public GameobjectTemplate GetGOTemplate(UInt32 _entry)
        {
            GameobjectTemplate _cr = new GameobjectTemplate();
            foreach (KeyValuePair<UInt32, GameobjectTemplate> o in GOTemplateMap)
            {
                if (o.Value.entry == _entry)
                {
                    return o.Value;

                }
            }
            return _cr;
        }

        public int GetGOTemplateCount() { return GOTemplateMap.Count(); }

        public Dictionary<UInt32, GameobjectTemplate> GetGOTemplateStorage() { return GOTemplateMap; }

        public void AddNonexistentGO(UInt32 _id)
        {
            _nogameobj.Add(_id);
        }

        public bool GONonExistent(UInt32 _id)
        {
            //return _nogameobj.Find(x => x == _id) != null;
            return true;
        }

        #endregion

        //--misc part--

        public void AddRequestedPlayerGUID(UInt32 _id)
        {
            _reqpnames.Add(_id);
        }

        public bool IsRequestedPlayerGUID(UInt32 _id)
        {
            //return _reqpnames.Find(x => x == _id) != null;
            return true;
        }




    }
}
