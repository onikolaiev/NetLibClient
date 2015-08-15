using netLogic.Constants;
using netLogic.Shared;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace netLogic
{
    public struct PlayerEnum
    {
        public GameGuid GUID;
        public string Name;
        public byte Race;
        public byte Class;
        public byte Gender;
        public byte Skin;
        public byte Face;
        public byte HairStyle;
        public byte HairColor;
        public byte FacialHair;
        public byte Level;
        public UInt32 ZoneID;
        public UInt32 MapID;
        public float X;
        public float Y;
        public float Z;
        public UInt32 Guild;
        public UInt32 CharacterFlags;
        public UInt32 CustomizationFlags;
        public byte FirstLogin;
        public UInt32 PetInfoID;
        public UInt32 PetLevel;
        public UInt32 PetFamilyID;
        public PlayerEnumItem[] _items;
    }

    public struct PlayerEnumItem
    {
        public UInt32 displayId;
        public byte inventorytype;
        public UInt32 enchant;
    }

    public struct SpellBookEntry
    {
        public UInt32 id;
        public UInt16 slot;
    }

    public class Player : Unit
    {

        public Player()
        {
            _type = OBJECT_TYPE.TYPE_PLAYER;
            _typeId = OBJECT_TYPE_ID.TYPEID_PLAYER;
        }


        public uint GetGender() { return GetUInt32Value((int)UpdateFields.PLAYER_BYTES_3); }
        public uint GetSkinId() { return (GetUInt32Value((int)UpdateFields.PLAYER_BYTES) & 0x000000FF); }
        public uint GetFaceId() { return (GetUInt32Value((int)UpdateFields.PLAYER_BYTES) & 0x0000FF00) >> 8; }
        public uint GetHairStyleId() { return (GetUInt32Value((int)UpdateFields.PLAYER_BYTES) & 0x00FF0000) >> 16; }
        public uint GetHairColorId() { return (GetUInt32Value((int)UpdateFields.PLAYER_BYTES) & 0xFF000000) >> 24; }
        public uint GetFaceTraitsId() { return (GetUInt32Value((int)UpdateFields.PLAYER_BYTES_2) & 0x000000FF); }
        
    }

    public class MyCharacter : Player
    {
        List<SpellBookEntry> _spells = new List<SpellBookEntry>();
        UInt64 _target; // currently targeted object

        public UInt64 GetTarget() { return _target; }

        public void SetTarget(UInt64 guid) { _target = guid; } // should only be called by WorldSession::SendSetSelection() !!
        
        public void AddSpell(UInt32 _spellid, UInt16 _spellslot)
        {
	        SpellBookEntry _spell;
	        _spell.id = _spellid;
	        _spell.slot = _spellslot;
	        _spells.Add(_spell);
        }

        public void RemoveSpell(UInt32 _spellid)
        {
            foreach (SpellBookEntry se in _spells)
            {
                if (se.id == _spellid)
                {
                    _spells.Remove(se);
                }
            
            }
        }

        public void ClearSpells() { _spells.Clear(); }

        public bool HasSpell(UInt32 _spellid)
        {
            foreach (SpellBookEntry se in _spells)
            {
                if (se.id == _spellid)
                {
                    return true;
                }

            }
            return false;
        }

        public UInt32 GetSpellSlot(UInt32 _spellid)
        {
            if (HasSpell(_spellid))
            {
                foreach (SpellBookEntry se in _spells)
                {
                    if (se.id == _spellid)
                    {
                        return se.slot;
                    }

                }
            }
            return 0;
        }

        public void SetActionButtons()
        { 
        
        
        }
    }
}
