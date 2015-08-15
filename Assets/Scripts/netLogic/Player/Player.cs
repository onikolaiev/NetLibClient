using System;
using System.Text;

using netLogic.Shared;
using netLogic.Constants;        // for WoWGuid and UpdateFields

namespace netLogic.Clients
{
   /* // Represents the 'Player' being played by the bot.
    public class Player : MMOObject
    {
        public Character character;
      //  public MMOObject obj;

        private uint level;
        private uint exp;
        private uint nextlevelexp;

        private uint hp;
        private uint mana = 0;
        private uint energy = 0;
        private uint rage = 0;

        private Boolean inited = false;

        private ActionBars actionBars;                  // Players Action Bars
        private FriendsListItem[] friendsList;          // Friends List
        private IgnoreListItem[] ignoreList;            // Ignore List
        private Talents talents;                        // Talents (Class Build)
        private Inventory inventory;                    // Items in players Inventory. (all bags, combined, treated as one large inventory)
        private Bank bank;                              // Items in players Bank. (bank, and bags, all combined)
        private Spells spells;                          // Spells the player currently knows. Spell cooldowns kept in here.
        private Reputation reputation;                  // Players reputation with various factions
        private Equipped equipped;                      // Items currently equipped. Durability also stored here.
        private Buffs buffs;                            // Players Buffs
        private Debuffs debuffs;                        // Players Debuffs
        private QuestLog questLog;                      // Players Quest Log

        public Player()
        {
           
        }

        // Initialize Player, with Character Object
        public void setPlayer(Character c)
        {
            character = c;
        }

        // Initialize Player, with Player Object Update Fields :D
        public void setPlayer(MMOObject po)
        {
            
            
            Log.WriteLine(LogType.Debug, "Player Class Initialized!");

            level = po.Data[(int)UpdateFieldsLoader.GetUpdateField(OBJECT_TYPE_ID.TYPEID_PLAYER, "UNIT_FIELD_LEVEL").Identifier];
            //exp = po.Data[(int)UpdateFieldsLoader.GetUpdateField(ObjectTypes.TYPEID_PLAYER, "PLAYER_XP").Identifier];
           // nextlevelexp = po.Data[(int)UpdateFieldsLoader.GetUpdateField(ObjectTypes.TYPEID_PLAYER, "PLAYER_NEXT_LEVEL_XP").Identifier];
            hp = po.Data[(int)UpdateFieldsLoader.GetUpdateField(OBJECT_TYPE_ID.TYPEID_PLAYER, "UNIT_FIELD_HEALTH").Identifier];      // probably wrong?

            // Create contained Objects
            questLog = new QuestLog(po);
            inventory = new Inventory(po);
            bank = new Bank(po);
            equipped = new Equipped(po);
            talents = new Talents(po);
            buffs = new Buffs(po);
            debuffs = new Debuffs(po);

            inited = true;
        }


        public void setPlayer(MMOObject po,bool nameonly)
        {
            //obj = netLogicCore.ObjectMgr.GetObject(po.GUID);
        }

        // Update Player, with Player Object Update Fields :D
        public void updatePlayer(MMOObject po)
        {
            if (!inited)
                return;

          //  obj = po;

            Log.WriteLine(LogType.Debug, "Player Class Updated!");

            level = po.Data[UpdateFieldsLoader.GetUpdateField(OBJECT_TYPE_ID.TYPEID_PLAYER, "UNIT_FIELD_LEVEL").Identifier];
            exp = po.Data[UpdateFieldsLoader.GetUpdateField(OBJECT_TYPE_ID.TYPEID_PLAYER, "PLAYER_XP").Identifier];
            nextlevelexp = po.Data[UpdateFieldsLoader.GetUpdateField(OBJECT_TYPE_ID.TYPEID_PLAYER, "PLAYER_NEXT_LEVEL_XP").Identifier];
            hp = po.Data[UpdateFieldsLoader.GetUpdateField(OBJECT_TYPE_ID.TYPEID_PLAYER, "UNIT_FIELD_HEALTH").Identifier];      // probably wrong?
        }

        // Initialize Friends List, from the list recieved from the WorldServer
        public void setFriendList(FriendsListItem[] fl)
        {
            friendsList = fl;
        }

        // Initialize Ignore List, from the list recieved from the WorldServer
        public void setIgnoreList(IgnoreListItem[] il)
        {
            ignoreList = il;
        }

        // Update status of a Friend on our Friends List
        public void friendStatusUpdate(FriendsListItem f)
        {
            // Find friend in friends list, and update online status.
            for (int i = 0; i < friendsList.Length; i++)
            {
                if (friendsList[i].guid.GetOldGuid() == f.guid.GetOldGuid())
                {
                    friendsList[i].online = f.online;
                    return;
                }
            }
        }

        // Create new Spells Object, initialized by the list recieved from the WorldServer
        public void setSpells(SpellItem[] sl)
        {
            spells = new Spells(sl);
            
        }

        // Create new Reputation Object, initialized by the list recieved from the WorldServer
        public void setReputation(ReputationItem[] rl)
        {
            reputation = new Reputation(rl);
        }

        // Create a new Action Buttons Object, initialized by the list recieved from the WorldServer
        public void setActionBars(ActionButton[] abl)
        {
            actionBars = new ActionBars(abl);
        }

        // Do Levelup Stuff
        public void levelUp()
        {
            level++;
        }

        // Properties
        public Boolean      Inited      { get { return inited;      } }
        public Coordinate   Location      { get { return Movement.Position;   } }
        public float        Heading     { get { return Movement.Facing; } }
        public Character    Character   { get { return character;   } }
        public byte         Class       { get { return character.Class; } }
        public uint         Level       { get { return level;       } }
        public uint         Health      { get { return hp;          } }
        public Spells       Spells      { get { return spells;      } }
        public Reputation   Reputation  { get { return reputation;  } }
        public ActionBars   ActionBars  { get { return actionBars;  } }
        public Inventory    Inventory   { get { return inventory;   } }
        public Bank         Bank        { get { return bank;        } }
        public Equipped     Equipped    { get { return equipped;    } }
        public Talents      Talents     { get { return talents;     } }
        public Buffs        Buffs       { get { return buffs;       } }
        public Debuffs      Debuffs     { get { return debuffs;     } }
        public QuestLog     QuestLog    { get { return questLog;    } }
        public bool         IsDead      { get { return false;       } } // todo
        public bool         IsInCombat  { get { return false;       } } // todo


       public float DistanceTo(Coordinate l)
        {
            return obj.GetCoordinates().DistanceTo(l);
        }

        


    }

    // This is retrieved from the worldserver. May as well hold onto it in Player.
    public struct Character
    {
        public WoWGuid GUID;
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
        public CharEquipment[] Equipment;
    }
    */
    public struct FriendsListItem
    {
        public GameGuid guid;
        public bool online;
    }

    public struct IgnoreListItem
    {
        public GameGuid guid;
    }
}