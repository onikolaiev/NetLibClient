using netLogic.Constants;
using netLogic.Network;
using netLogic.Shared;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace netLogic
{
    
    #region ItemEnums
    public enum InventoryChangeFailure
    {
        EQUIP_ERR_OK = 0,
        EQUIP_ERR_YOU_MUST_REACH_LEVEL_N = 1,
        EQUIP_ERR_SKILL_ISNT_HIGH_ENOUGH = 2,
        EQUIP_ERR_ITEM_DOESNT_GO_TO_SLOT = 3,
        EQUIP_ERR_BAG_FULL = 4,
        EQUIP_ERR_NONEMPTY_BAG_OVER_OTHER_BAG = 5,
        EQUIP_ERR_CANT_TRADE_EQUIP_BAGS = 6,
        EQUIP_ERR_ONLY_AMMO_CAN_GO_HERE = 7,
        EQUIP_ERR_NO_REQUIRED_PROFICIENCY = 8,
        EQUIP_ERR_NO_EQUIPMENT_SLOT_AVAILABLE = 9,
        EQUIP_ERR_YOU_CAN_NEVER_USE_THAT_ITEM = 10,
        EQUIP_ERR_YOU_CAN_NEVER_USE_THAT_ITEM2 = 11,
        EQUIP_ERR_NO_EQUIPMENT_SLOT_AVAILABLE2 = 12,
        EQUIP_ERR_CANT_EQUIP_WITH_TWOHANDED = 13,
        EQUIP_ERR_CANT_DUAL_WIELD = 14,
        EQUIP_ERR_ITEM_DOESNT_GO_INTO_BAG = 15,
        EQUIP_ERR_ITEM_DOESNT_GO_INTO_BAG2 = 16,
        EQUIP_ERR_CANT_CARRY_MORE_OF_THIS = 17,
        EQUIP_ERR_NO_EQUIPMENT_SLOT_AVAILABLE3 = 18,
        EQUIP_ERR_ITEM_CANT_STACK = 19,
        EQUIP_ERR_ITEM_CANT_BE_EQUIPPED = 20,
        EQUIP_ERR_ITEMS_CANT_BE_SWAPPED = 21,
        EQUIP_ERR_SLOT_IS_EMPTY = 22,
        EQUIP_ERR_ITEM_NOT_FOUND = 23,
        EQUIP_ERR_CANT_DROP_SOULBOUND = 24,
        EQUIP_ERR_OUT_OF_RANGE = 25,
        EQUIP_ERR_TRIED_TO_SPLIT_MORE_THAN_COUNT = 26,
        EQUIP_ERR_COULDNT_SPLIT_ITEMS = 27,
        EQUIP_ERR_MISSING_REAGENT = 28,
        EQUIP_ERR_NOT_ENOUGH_MONEY = 29,
        EQUIP_ERR_NOT_A_BAG = 30,
        EQUIP_ERR_CAN_ONLY_DO_WITH_EMPTY_BAGS = 31,
        EQUIP_ERR_DONT_OWN_THAT_ITEM = 32,
        EQUIP_ERR_CAN_EQUIP_ONLY1_QUIVER = 33,
        EQUIP_ERR_MUST_PURCHASE_THAT_BAG_SLOT = 34,
        EQUIP_ERR_TOO_FAR_AWAY_FROM_BANK = 35,
        EQUIP_ERR_ITEM_LOCKED = 36,
        EQUIP_ERR_YOU_ARE_STUNNED = 37,
        EQUIP_ERR_YOU_ARE_DEAD = 38,
        EQUIP_ERR_CANT_DO_RIGHT_NOW = 39,
        EQUIP_ERR_BAG_FULL2 = 40,
        EQUIP_ERR_CAN_EQUIP_ONLY1_QUIVER2 = 41,
        EQUIP_ERR_CAN_EQUIP_ONLY1_AMMOPOUCH = 42,
        EQUIP_ERR_STACKABLE_CANT_BE_WRAPPED = 43,
        EQUIP_ERR_EQUIPPED_CANT_BE_WRAPPED = 44,
        EQUIP_ERR_WRAPPED_CANT_BE_WRAPPED = 45,
        EQUIP_ERR_BOUND_CANT_BE_WRAPPED = 46,
        EQUIP_ERR_UNIQUE_CANT_BE_WRAPPED = 47,
        EQUIP_ERR_BAGS_CANT_BE_WRAPPED = 48,
        EQUIP_ERR_ALREADY_LOOTED = 49,
        EQUIP_ERR_INVENTORY_FULL = 50,
        EQUIP_ERR_BANK_FULL = 51,
        EQUIP_ERR_ITEM_IS_CURRENTLY_SOLD_OUT = 52,
        EQUIP_ERR_BAG_FULL3 = 53,
        EQUIP_ERR_ITEM_NOT_FOUND2 = 54,
        EQUIP_ERR_ITEM_CANT_STACK2 = 55,
        EQUIP_ERR_BAG_FULL4 = 56,
        EQUIP_ERR_ITEM_SOLD_OUT = 57,
        EQUIP_ERR_OBJECT_IS_BUSY = 58,
        EQUIP_ERR_NONE = 59,
        EQUIP_ERR_CANT_DO_IN_COMBAT = 60,
        EQUIP_CANT_DO_WHILE_DISARMED = 61,
        EQUIP_ERR_BAG_FULL6 = 62,
        EQUIP_ITEM_RANK_NOT_ENOUGH = 63,
        EQUIP_ITEM_REPUTATION_NOT_ENOUGH = 64,
        EQUIP_MORE_THAN1_SPECIAL_BAG = 65
    }

    public enum BuyFailure
    {
        BUY_ERR_CANT_FIND_ITEM = 0,
        BUY_ERR_ITEM_ALREADY_SOLD = 1,
        BUY_ERR_NOT_ENOUGHT_MONEY = 2,
        BUY_ERR_SELLER_DONT_LIKE_YOU = 4,
        BUY_ERR_DISTANCE_TOO_FAR = 5,
        BUY_ERR_CANT_CARRY_MORE = 8,
        BUY_ERR_LEVEL_REQUIRE = 11,
        BUY_ERR_REPUTATION_REQUIRE = 12
    }

    public enum SellFailure
    {
        SELL_ERR_CANT_FIND_ITEM = 1,
        SELL_ERR_CANT_SELL_ITEM = 2,
        SELL_ERR_CANT_FIND_VENDOR = 3
    }

    public enum ITEM_STAT_TYPE
    {
        ITEM_STAT_POWER = 0,
        ITEM_STAT_HEALTH = 1,
        ITEM_STAT_UNKNOWN = 2,
        ITEM_STAT_AGILITY = 3,
        ITEM_STAT_STRENGTH = 4,
        ITEM_STAT_INTELLECT = 5,
        ITEM_STAT_SPIRIT = 6,
        ITEM_STAT_STAMINA = 7
    }

    public enum ITEM_DAMAGE_TYPE
    {
        NORMAL_DAMAGE = 0,
        HOLY_DAMAGE = 1,
        FIRE_DAMAGE = 2,
        NATURE_DAMAGE = 3,
        FROST_DAMAGE = 4,
        SHADOW_DAMAGE = 5,
        ARCANE_DAMAGE = 6
    }

    public enum ITEM_SPELLTRIGGER_TYPE
    {
        USE = 0,
        ON_EQUIP = 1,
        CHANCE_ON_HIT = 2,
        SOULSTONE = 4
    }

    public enum ITEM_BONDING_TYPE
    {
        NO_BIND = 0,
        BIND_WHEN_PICKED_UP = 1,
        BIND_WHEN_EQUIPED = 2,
        BIND_WHEN_USE = 3,
        //TODO: Better name these
        QUEST_ITEM = 4,
        QUEST_ITEM1 = 5
    }

    // masks for ITEM_FIELD_FLAGS field
    public enum ITEM_FLAGS
    {
        ITEM_FLAGS_BINDED = 1
    }

    public enum BAG_FAMILY
    {
        BAG_FAMILY_NONE = 0,
        BAG_FAMILY_ARROWS = 1,
        BAG_FAMILY_BULLETS = 2,
        BAG_FAMILY_SOUL_SHARDS = 3,
        //BAG_FAMILY_UNK1                           = 4,
        //BAG_FAMILY_UNK1                           = 5,
        BAG_FAMILY_HERBS = 6,
        BAG_FAMILY_ENCHANTING_SUPP = 7,
        BAG_FAMILY_ENGINEERING_SUPP = 8,
        BAG_FAMILY_KEYS = 9,
        BAG_FAMILY_GEMS = 10,
        //BAG_FAMILY_UNK3                           = 11,
        BAG_FAMILY_MINING_SUPP = 12
    }

    public enum INVENTORY_TYPES
    {
        INVTYPE_NON_EQUIP = 0,
        INVTYPE_HEAD = 1,
        INVTYPE_NECK = 2,
        INVTYPE_SHOULDERS = 3,
        INVTYPE_BODY = 4,
        INVTYPE_CHEST = 5,
        INVTYPE_WAIST = 6,
        INVTYPE_LEGS = 7,
        INVTYPE_FEET = 8,
        INVTYPE_WRISTS = 9,
        INVTYPE_HANDS = 10,
        INVTYPE_FINGER = 11,
        INVTYPE_TRINKET = 12,
        INVTYPE_WEAPON = 13,
        INVTYPE_SHIELD = 14,
        INVTYPE_RANGED = 15,
        INVTYPE_CLOAK = 16,
        INVTYPE_2HWEAPON = 17,
        INVTYPE_BAG = 18,
        INVTYPE_TABARD = 19,
        INVTYPE_ROBE = 20,
        INVTYPE_WEAPONMAINHAND = 21,
        INVTYPE_WEAPONOFFHAND = 22,
        INVTYPE_HOLDABLE = 23,
        INVTYPE_AMMO = 24,
        INVTYPE_THROWN = 25,
        INVTYPE_RANGEDRIGHT = 26,
        INVTYPE_SLOT_ITEM = 27,
        INVTYPE_RELIC = 28,
        NUM_INVENTORY_TYPES = 29
    }

    public enum INVENTORY_CLASS
    {
        ITEM_CLASS_CONSUMABLE = 0,
        ITEM_CLASS_CONTAINER = 1,
        ITEM_CLASS_WEAPON = 2,
        ITEM_CLASS_JEWELRY = 3,
        ITEM_CLASS_ARMOR = 4,
        ITEM_CLASS_REAGENT = 5,
        ITEM_CLASS_PROJECTILE = 6,
        ITEM_CLASS_TRADE_GOODS = 7,
        ITEM_CLASS_GENERIC = 8,
        ITEM_CLASS_BOOK = 9,
        ITEM_CLASS_MONEY = 10,
        ITEM_CLASS_QUIVER = 11,
        ITEM_CLASS_QUEST = 12,
        ITEM_CLASS_KEY = 13,
        ITEM_CLASS_PERMANENT = 14,
        ITEM_CLASS_JUNK = 15,
        ITEM_CLASS_MISC = 15,
        ITEM_CLASS_GLYPH = 16
    }

    // Client understand only 0 subclass for ITEM_CLASS_CONSUMABLE
    // but this value used in code as implementation workaround
    public enum ITEM_SUBCLASS_CONSUMABLE
    {
        ITEM_SUBCLASS_POTION = 1,
        ITEM_SUBCLASS_ELIXIR = 2,
        ITEM_SUBCLASS_FLASK = 3,
        ITEM_SUBCLASS_SCROLL = 4,
        ITEM_SUBCLASS_FOOD = 5,
        ITEM_SUBCLASS_ITEM_ENHANCEMENT = 6,
        ITEM_SUBCLASS_BANDAGE = 7
    }

    public enum ITEM_SUBCLASS_CONTAINER
    {
        ITEM_SUBCLASS_CONTAINER = 0,
        ITEM_SUBCLASS_SOUL_CONTAINER = 1,
        ITEM_SUBCLASS_HERB_CONTAINER = 2,
        ITEM_SUBCLASS_ENCHANTING_CONTAINER = 3,
        ITEM_SUBCLASS_ENGINEERING_CONTAINER = 4,
        ITEM_SUBCLASS_GEM_CONTAINER = 5,
        ITEM_SUBCLASS_MINING_CONTAINER = 6,
        ITEM_SUBCLASS_LEATHERWORKING_CONTAINER = 7,
        ITEM_SUBCLASS_INSCRIPTION_CONTAINER = 8
    }

    public enum INVENTORY_SUBCLASS_WEAPON
    {
        ITEM_SUBCLASS_WEAPON_AXE = 0,
        ITEM_SUBCLASS_WEAPON_AXE2 = 1,
        ITEM_SUBCLASS_WEAPON_BOW = 2,
        ITEM_SUBCLASS_WEAPON_GUN = 3,
        ITEM_SUBCLASS_WEAPON_MACE = 4,
        ITEM_SUBCLASS_WEAPON_MACE2 = 5,
        ITEM_SUBCLASS_WEAPON_POLEARM = 6,
        ITEM_SUBCLASS_WEAPON_SWORD = 7,
        ITEM_SUBCLASS_WEAPON_SWORD2 = 8,
        ITEM_SUBCLASS_WEAPON_obsolete = 9,
        ITEM_SUBCLASS_WEAPON_STAFF = 10,
        ITEM_SUBCLASS_WEAPON_EXOTIC = 11,
        ITEM_SUBCLASS_WEAPON_EXOTIC2 = 12,
        ITEM_SUBCLASS_WEAPON_UNARMED = 13,
        ITEM_SUBCLASS_WEAPON_GENERIC = 14,
        ITEM_SUBCLASS_WEAPON_DAGGER = 15,
        ITEM_SUBCLASS_WEAPON_THROWN = 16,
        ITEM_SUBCLASS_WEAPON_SPEAR = 17,
        ITEM_SUBCLASS_WEAPON_CROSSBOW = 18,
        ITEM_SUBCLASS_WEAPON_WAND = 19,
        ITEM_SUBCLASS_WEAPON_FISHING_POLE = 20
    }

    public enum ITEM_SUBCLASS_ARMOR
    {
        ITEM_SUBCLASS_ARMOR_GENERIC = 0,
        ITEM_SUBCLASS_ARMOR_CLOTH = 1,
        ITEM_SUBCLASS_ARMOR_LEATHER = 2,
        ITEM_SUBCLASS_ARMOR_MAIL = 3,
        ITEM_SUBCLASS_ARMOR_PLATE = 4,
        ITEM_SUBCLASS_ARMOR_BUCKLER = 5,
        ITEM_SUBCLASS_ARMOR_SHIELD = 6,
        ITEM_SUBCLASS_ARMOR_LIBRAM = 7,
        ITEM_SUBCLASS_ARMOR_IDOL = 8,
        ITEM_SUBCLASS_ARMOR_TOTEM = 9,
        ITEM_SUBCLASS_ARMOR_SIGIL = 10
    }

    public enum ITEM_SUBCLASS_PROJECTILE
    {
        ITEM_SUBCLASS_ARROW = 2,
        ITEM_SUBCLASS_BULLET = 3
    }

    public enum ITEM_SUBCLASS_TRADE_GOODS
    {
        ITEM_SUBCLASS_TRADE_GOODS = 0,
        ITEM_SUBCLASS_PARTS = 1,
        ITEM_SUBCLASS_EXPLOSIVES = 2,
        ITEM_SUBCLASS_DEVICES = 3,
        ITEM_SUBCLASS_JEWELCRAFTING = 4,
        ITEM_SUBCLASS_CLOTH = 5,
        ITEM_SUBCLASS_LEATHER = 6,
        ITEM_SUBCLASS_METAL_STONE = 7,
        ITEM_SUBCLASS_MEAT = 8,
        ITEM_SUBCLASS_HERB = 9,
        ITEM_SUBCLASS_ELEMENTAZL = 10,
        ITEM_SUBCLASS_TRADE_GOODS_OTHER = 11,
        ITEM_SUBCLASS_ENCHANTING = 12,
        ITEM_SUBCLASS_MATERIAL = 13,
        ITEM_SUBCLASS_ARMOR_ENCHANTMENT = 14,
        ITEM_SUBCLASS_WEAPON_ENCHANTMENT = 15
    }

    public enum ITEM_SUBCLASS_BOOK
    {
        ITEM_SUBCLASS_BOOK = 0,
        ITEM_SUBCLASS_LEATHERWORKING_PATTERN = 1,
        ITEM_SUBCLASS_TAILORING_PATTERN = 2,
        ITEM_SUBCLASS_ENGINEERING_SCHEMATIC = 3,
        ITEM_SUBCLASS_COOKING_RECIPE = 5,
        ITEM_SUBCLASS_ALCHEMY_RECIPE = 6,
        ITEM_SUBCLASS_FIRST_AID_MANUAL = 7,
        ITEM_SUBCLASS_ENCHANTING_FORMULA = 8,
        ITEM_SUBCLASS_FISHING_MANUAL = 9
    }

    public enum ITEM_SUBCLASS_QUIVER
    {
        ITEM_SUBCLASS_QUIVER = 2,
        ITEM_SUBCLASS_AMMO_POUCH = 3
    }
    #endregion

    #region _Struct ItemProto
    public struct _ItemStat
{
        public UInt32 ItemStatType;
        public UInt32 ItemStatValue;

};

    public struct _ItemSpell
    {
        public UInt32 SpellId;
        public UInt32 SpellTrigger;
        public UInt32 SpellCharges;
        public UInt32 SpellCooldown;
        public UInt32 SpellCategory;
        public UInt32 SpellCategoryCooldown;

    };

    public struct _ItemSocket
    {
        public UInt32 Color;
        public UInt32 Content;
    };

    public struct _ItemDamage
    {
        public float DamageMin;
        public float DamageMax;
        public UInt32 DamageType;

    };

    public struct ItemProto
    {
        
        public UInt32 Id;
        public UInt32 Class;
        public UInt32 SubClass;
        public string Name;
        public UInt32 DisplayInfoID;
        public UInt32 Quality;
        public UInt32 Flags;
        public UInt32 Faction;
        public UInt32 BuyCount;
        public UInt32 BuyPrice;
        public UInt32 SellPrice;
        public UInt32 InventoryType;
        public UInt32 AllowableClass;
        public UInt32 AllowableRace;
        public UInt32 ItemLevel;
        public UInt32 RequiredLevel;
        public UInt32 RequiredSkill;
        public UInt32 RequiredSkillRank;
        public UInt32 RequiredSpell;
        public UInt32 RequiredHonorRank;
        public UInt32 RequiredCityRank;
        public UInt32 RequiredReputationFaction;
        public UInt32 RequiredReputationRank;
        public UInt32 MaxCount;
        public UInt32 Stackable;
        public UInt32 ContainerSlots;
        public UInt32 StatsCount;
        public _ItemStat[] ItemStat;
        public UInt32 ScalingStatDistribution;                         // id from ScalingStatDistribution.dbc
        public UInt32 ScalingStatValue;                                // mask for selecting column in ScalingStatValues.dbc
        public _ItemDamage[] Damage;
        public UInt32 Armor;
        public UInt32 HolyRes;
        public UInt32 FireRes;
        public UInt32 NatureRes;
        public UInt32 FrostRes;
        public UInt32 ShadowRes;
        public UInt32 ArcaneRes;
        public UInt32 Delay;
        public UInt32 Ammo_type;
        public float RangedModRange;
        public _ItemSpell[] Spells;
        public UInt32 Bonding;
        public string Description;
        public UInt32 PageText;
        public UInt32 LanguageID;
        public UInt32 PageMaterial;
        public UInt32 StartQuest;
        public UInt32 LockID;
        public UInt32 Material;
        public UInt32 Sheath;
        public UInt32 RandomProperty;
        public UInt32 RandomSuffix;
        public UInt32 Unk1;
        public UInt32 Block;
        public UInt32 ItemSet;
        public UInt32 MaxDurability;
        public UInt32 Area;
        public UInt32 Map;
        public UInt32 BagFamily;
        public UInt32 TotemCategory;
        public _ItemSocket[] Socket;
        public UInt32 socketBonus;
        public UInt32 GemProperties;
        public UInt32 RequiredArenaRank;
        public UInt32 RequiredDisenchantSkill;
        public float ArmorDamageModifier;
        public Int32 Duration;                                        // negative = realtime, positive = ingame time
        public UInt32 ItemLimitCategory;                               // id from ItemLimitCategory.dbc
        public UInt32 HolidayId;
    };
    #endregion


    public class Item : Object
    {

        private uint _slot;
        private Bag _bag;

        public Item()
        {
            
            _depleted = false;
            _type = OBJECT_TYPE.TYPE_ITEM;
            _typeId = OBJECT_TYPE_ID.TYPEID_ITEM;
            _slot = 0;
        }

        public uint GetSlot() { return _slot;}

        public void SetSlot(uint nr) { _slot = nr; }

        public UInt32 GetEntry() { return GetUInt32Value((int)UpdateFields.OBJECT_FIELD_ENTRY); }

        public UInt32 GetCount() { return GetUInt32Value((int)UpdateFields.ITEM_FIELD_STACK_COUNT); }

        public Bag GetBag() { return _bag; }

        public bool IsInBag() { return _bag != null; }



        #region ItemProto_Handler
        [PacketHandlerAtribute(WorldServerOpCode.SMSG_ITEM_QUERY_SINGLE_RESPONSE)]
        public void _ItemQuerySingleResponse(PacketIn packet)
        {
            UInt32 ItemID;
          //  UInt32 unk;
           // uint unk8;
          //  string s;

            ItemID = packet.ReadUInt32();

            if (!(ItemID == 0x80000000)) // invalid item flag?
            {
                ItemProto proto = new ItemProto();
                proto.Id = ItemID;
                proto.Class = packet.ReadUInt32();
                proto.SubClass = packet.ReadUInt32();
                packet.ReadUInt32();
                proto.Name = packet.ReadString();
                packet.ReadByte();
                packet.ReadByte();
                packet.ReadByte();
                proto.DisplayInfoID = packet.ReadUInt32();
                proto.Quality = packet.ReadUInt32();
                proto.Flags = packet.ReadUInt32();
                proto.Faction = packet.ReadUInt32();
                proto.BuyPrice = packet.ReadUInt32();
                proto.SellPrice = packet.ReadUInt32();
                proto.InventoryType = packet.ReadUInt32();
                proto.AllowableClass = packet.ReadUInt32();
                proto.AllowableRace = packet.ReadUInt32();
                proto.ItemLevel = packet.ReadUInt32();
                proto.RequiredLevel = packet.ReadUInt32();
                proto.RequiredSkill = packet.ReadUInt32();
                proto.RequiredSkillRank = packet.ReadUInt32();
                proto.RequiredSpell = packet.ReadUInt32();
                proto.RequiredHonorRank = packet.ReadUInt32();
                proto.RequiredCityRank = packet.ReadUInt32();
                proto.RequiredReputationFaction = packet.ReadUInt32();
                proto.RequiredReputationRank = packet.ReadUInt32();
                proto.MaxCount = packet.ReadUInt32();
                proto.Stackable = packet.ReadUInt32();
                proto.ContainerSlots = packet.ReadUInt32();
                proto.StatsCount = packet.ReadUInt32();
                for (UInt32 i = 0; i<proto.StatsCount; i++)
                {
                    proto.ItemStat = new _ItemStat[10];
                    proto.ItemStat[i].ItemStatType = packet.ReadUInt32();
                    proto.ItemStat[i].ItemStatValue = packet.ReadUInt32();
                }
                proto.ScalingStatDistribution = packet.ReadUInt32();
                proto.ScalingStatValue = packet.ReadUInt32();
                for (int i = 0; i < 2; i++)
                {
                    proto.Damage = new _ItemDamage[2];
                    proto.Damage[i].DamageMin = packet.ReadFloat();
                    proto.Damage[i].DamageMax = packet.ReadFloat();
                    proto.Damage[i].DamageType = packet.ReadUInt32();
                }
                proto.Armor = packet.ReadUInt32();
                proto.HolyRes = packet.ReadUInt32();
                proto.FireRes = packet.ReadUInt32();
                proto.NatureRes = packet.ReadUInt32();
                proto.FrostRes = packet.ReadUInt32();
                proto.ShadowRes = packet.ReadUInt32();
                proto.ArcaneRes = packet.ReadUInt32();
                proto.Delay = packet.ReadUInt32();
                proto.Ammo_type = packet.ReadUInt32();
                proto.RangedModRange = packet.ReadFloat();
                for (int i = 0; i < 5; i++)
                {
                    proto.Spells = new _ItemSpell[5];
                    proto.Spells[i].SpellId = packet.ReadUInt32();
                    proto.Spells[i].SpellTrigger = packet.ReadUInt32();
                    proto.Spells[i].SpellCharges = packet.ReadUInt32();
                    proto.Spells[i].SpellCooldown = packet.ReadUInt32();
                    proto.Spells[i].SpellCategory = packet.ReadUInt32();
                    proto.Spells[i].SpellCategoryCooldown = packet.ReadUInt32();
                }
                proto.Bonding = packet.ReadUInt32();
                proto.Description = packet.ReadString();
                proto.PageText = packet.ReadUInt32();
                proto.LanguageID = packet.ReadUInt32();
                proto.PageMaterial = packet.ReadUInt32();
                proto.StartQuest = packet.ReadUInt32();
                proto.LockID = packet.ReadUInt32();
                proto.Material = packet.ReadUInt32();
                proto.Sheath = packet.ReadUInt32();
                proto.RandomProperty = packet.ReadUInt32();
                proto.RandomSuffix = packet.ReadUInt32();
                proto.Block = packet.ReadUInt32();
                proto.ItemSet = packet.ReadUInt32();
                proto.MaxDurability = packet.ReadUInt32();
                proto.Area = packet.ReadUInt32();
                proto.Map = packet.ReadUInt32();
                proto.BagFamily = packet.ReadUInt32();
                proto.TotemCategory = packet.ReadUInt32();
                for (UInt32 i = 0; i < 3; i++)
                {
                    proto.Socket = new _ItemSocket[3];
                    proto.Socket[i].Color = packet.ReadUInt32();
                    proto.Socket[i].Content = packet.ReadUInt32();
                }
                proto.socketBonus = packet.ReadUInt32();
                proto.GemProperties = packet.ReadUInt32();
                proto.RequiredDisenchantSkill = packet.ReadUInt32();
                proto.ArmorDamageModifier = packet.ReadFloat();
                proto.Duration = packet.ReadInt32();
                proto.ItemLimitCategory = packet.ReadUInt32();
                proto.HolidayId = packet.ReadUInt32();

                //TODO
               /* objmgr.Add(proto);
                objmgr.AssignNameToObj(proto->Id, TYPEID_ITEM, proto->Name);
                objmgr.AssignNameToObj(proto->Id, TYPEID_CONTAINER, proto->Name);*/


            }


        }
        #endregion

    }
}
