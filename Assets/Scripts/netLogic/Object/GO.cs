using netLogic;
using netLogic.Shared;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace netLogic
{
    public struct GameobjectTemplate
    {
        public UInt32  entry;
        public UInt32 type;
        public UInt32 displayId;
        public string name;
        public string castBarCaption;
        public string unk1;
        public UInt32 faction;
        public UInt32 flags;
        public float size;
        public UInt32[] questItems;//4
        
        //0 GAMEOBJECT_TYPE_DOOR
        public struct door
        {
            UInt32 startOpen;                               //0 used client side to determine GO_ACTIVATED means open/closed
            UInt32 lockId;                                  //1 -> Lock.dbc
            UInt32 autoCloseTime;                           //2 secs till autoclose = autoCloseTime / 0x10000
            UInt32 noDamageImmune;                          //3 break opening whenever you recieve damage?
            UInt32 openTextID;                              //4 can be used to replace castBarCaption?
            UInt32 closeTextID;                             //5
            UInt32 ignoredByPathing;                        //6
        } ;
        //1 GAMEOBJECT_TYPE_BUTTON
        public struct button
        {
            UInt32 startOpen;                               //0
            UInt32 lockId;                                  //1 -> Lock.dbc
            UInt32 autoCloseTime;                           //2 secs till autoclose = autoCloseTime / 0x10000
            UInt32 linkedTrap;                              //3
            UInt32 noDamageImmune;                          //4 isBattlegroundObject
            UInt32 large;                                   //5
            UInt32 openTextID;                              //6 can be used to replace castBarCaption?
            UInt32 closeTextID;                             //7
            UInt32 losOK;                                   //8
        } ;
        //2 GAMEOBJECT_TYPE_QUESTGIVER
        public struct questgiver
        {
            UInt32 lockId;                                  //0 -> Lock.dbc
            UInt32 questList;                               //1
            UInt32 pageMaterial;                            //2
            UInt32 gossipID;                                //3
            UInt32 customAnim;                              //4
            UInt32 noDamageImmune;                          //5
            UInt32 openTextID;                              //6 can be used to replace castBarCaption?
            UInt32 losOK;                                   //7
            UInt32 allowMounted;                            //8
            UInt32 large;                                   //9
        } ;
        //3 GAMEOBJECT_TYPE_CHEST
        public struct chest
        {
            UInt32 lockId;                                  //0 -> Lock.dbc
            UInt32 lootId;                                  //1
            UInt32 chestRestockTime;                        //2
            UInt32 consumable;                              //3
            UInt32 minSuccessOpens;                         //4
            UInt32 maxSuccessOpens;                         //5
            UInt32 eventId;                                 //6 lootedEvent
            UInt32 linkedTrapId;                            //7
            UInt32 questId;                                 //8 not used currently but store quest required for GO activation for player
            UInt32 level;                                   //9
            UInt32 losOK;                                   //10
            UInt32 leaveLoot;                               //11
            UInt32 notInCombat;                             //12
            UInt32 logLoot;                                 //13
            UInt32 openTextID;                              //14 can be used to replace castBarCaption?
            UInt32 groupLootRules;                          //15
            UInt32 floatingTooltip;                         //16
        } ;
        //4 GAMEOBJECT_TYPE_BINDER - empty
        //5 GAMEOBJECT_TYPE_GENERIC
        public struct _generic
        {
            UInt32 floatingTooltip;                         //0
            UInt32 highlight;                               //1
            UInt32 serverOnly;                              //2
            UInt32 large;                                   //3
            UInt32 floatOnWater;                            //4
            UInt32 questID;                                 //5
        } ;
        //6 GAMEOBJECT_TYPE_TRAP
        public struct trap
        {
            UInt32 lockId;                                  //0 -> Lock.dbc
            UInt32 level;                                   //1
            UInt32 radius;                                  //2 radius for trap activation
            UInt32 spellId;                                 //3
            UInt32 charges;                                 //4 need respawn (if > 0)
            UInt32 cooldown;                                //5 time in secs
            UInt32 autoCloseTime;                           //6
            UInt32 startDelay;                              //7
            UInt32 serverOnly;                              //8
            UInt32 stealthed;                               //9
            UInt32 large;                                   //10
            UInt32 stealthAffected;                         //11
            UInt32 openTextID;                              //12 can be used to replace castBarCaption?
            UInt32 closeTextID;                             //13
            UInt32 ignoreTotems;                            //14
        };
        //7 GAMEOBJECT_TYPE_CHAIR
        public struct chair
        {
            UInt32 slots;                                   //0
            UInt32 height;                                  //1
            UInt32 onlyCreatorUse;                          //2
            UInt32 triggeredEvent;                          //3
        } ;
        //8 GAMEOBJECT_TYPE_SPELL_FOCUS
        public struct spellFocus
        {
            UInt32 focusId;                                 //0
            UInt32 dist;                                    //1
            UInt32 linkedTrapId;                            //2
            UInt32 serverOnly;                              //3
            UInt32 questID;                                 //4
            UInt32 large;                                   //5
            UInt32 floatingTooltip;                         //6
        } ;
        //9 GAMEOBJECT_TYPE_TEXT
        public struct text
        {
            UInt32 pageID;                                  //0
            UInt32 language;                                //1
            UInt32 pageMaterial;                            //2
            UInt32 allowMounted;                            //3
        } ;
        //10 GAMEOBJECT_TYPE_GOOBER
        public struct goober
        {
            UInt32 lockId;                                  //0 -> Lock.dbc
            UInt32 questId;                                 //1
            UInt32 eventId;                                 //2
            UInt32 autoCloseTime;                           //3
            UInt32 customAnim;                              //4
            UInt32 consumable;                              //5
            UInt32 cooldown;                                //6
            UInt32 pageId;                                  //7
            UInt32 language;                                //8
            UInt32 pageMaterial;                            //9
            UInt32 spellId;                                 //10
            UInt32 noDamageImmune;                          //11
            UInt32 linkedTrapId;                            //12
            UInt32 large;                                   //13
            UInt32 openTextID;                              //14 can be used to replace castBarCaption?
            UInt32 closeTextID;                             //15
            UInt32 losOK;                                   //16 isBattlegroundObject
            UInt32 allowMounted;                            //17
            UInt32 floatingTooltip;                         //18
            UInt32 gossipID;                                //19
            UInt32 WorldStateSetsState;                     //20
        } ;
        //11 GAMEOBJECT_TYPE_TRANSPORT
        public struct transport
        {
            UInt32 pause;                                   //0
            UInt32 startOpen;                               //1
            UInt32 autoCloseTime;                           //2 secs till autoclose = autoCloseTime / 0x10000
            UInt32 pause1EventID;                           //3
            UInt32 pause2EventID;                           //4
        } ;
        //12 GAMEOBJECT_TYPE_AREADAMAGE
        public struct areadamage
        {
            UInt32 lockId;                                  //0
            UInt32 radius;                                  //1
            UInt32 damageMin;                               //2
            UInt32 damageMax;                               //3
            UInt32 damageSchool;                            //4
            UInt32 autoCloseTime;                           //5 secs till autoclose = autoCloseTime / 0x10000
            UInt32 openTextID;                              //6
            UInt32 closeTextID;                             //7
        } ;
        //13 GAMEOBJECT_TYPE_CAMERA
        public struct camera
        {
            UInt32 lockId;                                  //0 -> Lock.dbc
            UInt32 cinematicId;                             //1
            UInt32 eventID;                                 //2
            UInt32 openTextID;                              //3 can be used to replace castBarCaption?
        } ;
        //14 GAMEOBJECT_TYPE_MAPOBJECT - empty
        //15 GAMEOBJECT_TYPE_MO_TRANSPORT
        public struct moTransport
        {
            UInt32 taxiPathId;                              //0
            UInt32 moveSpeed;                               //1
            UInt32 accelRate;                               //2
            UInt32 startEventID;                            //3
            UInt32 stopEventID;                             //4
            UInt32 transportPhysics;                        //5
            UInt32 mapID;                                   //6
            UInt32 worldState1;                             //7
        } ;
        //16 GAMEOBJECT_TYPE_DUELFLAG - empty
        //17 GAMEOBJECT_TYPE_FISHINGNODE - empty
        //18 GAMEOBJECT_TYPE_SUMMONING_RITUAL
        public struct summoningRitual
        {
            UInt32 reqParticipants;                         //0
            UInt32 spellId;                                 //1
            UInt32 animSpell;                               //2
            UInt32 ritualPersistent;                        //3
            UInt32 casterTargetSpell;                       //4
            UInt32 casterTargetSpellTargets;                //5
            UInt32 castersGrouped;                          //6
            UInt32 ritualNoTargetCheck;                     //7
        } ;
        //19 GAMEOBJECT_TYPE_MAILBOX - empty
        //20 GAMEOBJECT_TYPE_DONOTUSE - empty
        //21 GAMEOBJECT_TYPE_GUARDPOST
        public struct guardpost
        {
            UInt32 creatureID;                              //0
            UInt32 charges;                                 //1
        } ;
        //22 GAMEOBJECT_TYPE_SPELLCASTER
        public struct spellcaster
        {
            UInt32 spellId;                                 //0
            UInt32 charges;                                 //1
            UInt32 partyOnly;                               //2
            UInt32 allowMounted;                            //3
            UInt32 large;                                   //4
        } ;
        //23 GAMEOBJECT_TYPE_MEETINGSTONE
        public struct meetingstone
        {
            UInt32 minLevel;                                //0
            UInt32 maxLevel;                                //1
            UInt32 areaID;                                  //2
        } ;
        //24 GAMEOBJECT_TYPE_FLAGSTAND
        public struct flagstand
        {
            UInt32 lockId;                                  //0
            UInt32 pickupSpell;                             //1
            UInt32 radius;                                  //2
            UInt32 returnAura;                              //3
            UInt32 returnSpell;                             //4
            UInt32 noDamageImmune;                          //5
            UInt32 openTextID;                              //6
            UInt32 losOK;                                   //7
        } ;
        //25 GAMEOBJECT_TYPE_FISHINGHOLE                    // not implemented yet
        public struct fishinghole
        {
            UInt32 radius;                                  //0 how close bobber must land for sending loot
            UInt32 lootId;                                  //1
            UInt32 minSuccessOpens;                         //2
            UInt32 maxSuccessOpens;                         //3
            UInt32 lockId;                                  //4 -> Lock.dbc; possibly 1628 for all?
        } ;
        //26 GAMEOBJECT_TYPE_FLAGDROP
        public struct flagdrop
        {
            UInt32 lockId;                                  //0
            UInt32 eventID;                                 //1
            UInt32 pickupSpell;                             //2
            UInt32 noDamageImmune;                          //3
            UInt32 openTextID;                              //4
        } ;
        //27 GAMEOBJECT_TYPE_MINI_GAME
        public struct miniGame
        {
            UInt32 gameType;                                //0
        } ;
        //29 GAMEOBJECT_TYPE_CAPTURE_POINT
        public struct capturePoint
        {
            UInt32 radius;                                  //0
            UInt32 spell;                                   //1
            UInt32 worldState1;                             //2
            UInt32 worldstate2;                             //3
            UInt32 winEventID1;                             //4
            UInt32 winEventID2;                             //5
            UInt32 contestedEventID1;                       //6
            UInt32 contestedEventID2;                       //7
            UInt32 progressEventID1;                        //8
            UInt32 progressEventID2;                        //9
            UInt32 neutralEventID1;                         //10
            UInt32 neutralEventID2;                         //11
            UInt32 neutralPercent;                          //12
            UInt32 worldstate3;                             //13
            UInt32 minSuperiority;                          //14
            UInt32 maxSuperiority;                          //15
            UInt32 minTime;                                 //16
            UInt32 maxTime;                                 //17
            UInt32 large;                                   //18
            UInt32 highlight;                               //19
            UInt32 startingValue;                           //20
            UInt32 unidirectional;                          //21
        } ;
        //30 GAMEOBJECT_TYPE_AURA_GENERATOR
        public struct auraGenerator
        {
            UInt32 startOpen;                               //0
            UInt32 radius;                                  //1
            UInt32 auraID1;                                 //2
            UInt32 conditionID1;                            //3
            UInt32 auraID2;                                 //4
            UInt32 conditionID2;                            //5
            UInt32 serverOnly;                              //6
        } ;
        //31 GAMEOBJECT_TYPE_DUNGEON_DIFFICULTY
        public struct dungeonDifficulty
        {
            UInt32 mapID;                                   //0
            UInt32 difficulty;                              //1
        } ;
        //32 GAMEOBJECT_TYPE_BARBER_CHAIR
        public struct barberChair
        {
            UInt32 chairheight;                             //0
            UInt32 heightOffset;                            //1
        } ;
        //33 GAMEOBJECT_TYPE_DESTRUCTIBLE_BUILDING
        public struct destructibleBuilding
        {
            UInt32 intactNumHits;                           //0
            UInt32 creditProxyCreature;                     //1
            UInt32 empty1;                                  //2
            UInt32 intactEvent;                             //3
            UInt32 empty2;                                  //4
            UInt32 damagedNumHits;                          //5
            UInt32 empty3;                                  //6
            UInt32 empty4;                                  //7
            UInt32 empty5;                                  //8
            UInt32 damagedEvent;                            //9
            UInt32 empty6;                                  //10
            UInt32 empty7;                                  //11
            UInt32 empty8;                                  //12
            UInt32 empty9;                                  //13
            UInt32 destroyedEvent;                          //14
            UInt32 empty10;                                 //15
            UInt32 debuildingTimeSecs;                      //16
            UInt32 empty11;                                 //17
            UInt32 destructibleData;                        //18
            UInt32 rebuildingEvent;                         //19
            UInt32 empty12;                                 //20
            UInt32 empty13;                                 //21
            UInt32 damageEvent;                             //22
            UInt32 empty14;                                 //23
        } ;
        //34 GAMEOBJECT_TYPE_GUILDBANK - empty
        //35 GAMEOBJECT_TYPE_TRAPDOOR
        public struct trapDoor
        {
            UInt32 whenToPause;                             // 0
            UInt32 startOpen;                               // 1
            UInt32 autoClose;                               // 2
        } ;

        // not use for specific field access (only for output with loop by all filed), also this determinate max union size
        public struct raw
        {
            UInt32[] data;//24
        } ;
    };

    public class GO : WorldObject
    {
        public GO()
        {
            _type = OBJECT_TYPE.TYPE_GAMEOBJECT;
            _typeId = OBJECT_TYPE_ID.TYPEID_GAMEOBJECT;
        
        }


    }
}
