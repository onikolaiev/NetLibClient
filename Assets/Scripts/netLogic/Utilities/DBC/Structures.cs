using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;


namespace netLogic.Utilities.DBC
{
    public class SkillLineAbility
    {
        public uint ID;
        public uint refSkillLine;
        public uint refSpellDBC;
        public uint chrRaces;
        public uint chrClasses;
        public uint chrRacesEx;
        public uint chrClassesEx;
        public uint reqSkill;
        public uint refSpellParent;
        public uint aquireMethod;
        public uint skillGreyLevel;
        public uint skillGreenLevel;
        public uint chrPoints1, chrPoints2;
    }

    public class SpellEntry
    {
        public uint ID;
        [Array(135)]
        public uint[] unused;
        [Localized]
        public string Name;
        [Array(81)]
        public uint[] unused2;
    }

    public class MapEntry
    {
        public uint ID;
        public string InternalName;
        public uint MapType;
        public uint AreaTable;
        public uint IsBattleground;        
        [Localized]
        public string Name;
        [Localized]
        public string DescAlliance;
        [Localized]
        public string DescHorde;
        public uint TimeOfDay;
        public uint LoadScreen;
        public float minimapScale;
        public float EntranceX;
        public float EntranceY;       
        public uint Expansion;
        public uint ParentArea;
        public uint Unk;
        public uint NumberOfPlayers;
        public uint Unk2;
    }

    public class MapEntry_4
    {
        public uint ID;
        public string InternalName;
        public uint MapType;
        public uint AreaTable;
        public uint Flags;
        public uint PvP;
        public string Name;
        public uint Unk2;
        public string DescAlliance;
        public string DescHorde;
        public uint LoadScreen;
        public float minimapScale;
        public uint corpseID;
        public float corpseX;
        public float corpseY;
        public uint TimeOfDay;
        public uint Expansion;
        public uint NumberOfPlayers;
        public uint ParentMap;
        public uint Unk1;
    }

    public class MapEntry_5
    {
        public uint ID;
        public string InternalName;
        public uint MapType;
        public uint AreaTable;
        public uint Flags;
        public string Name;
        public uint Unk2;
        public string DescAlliance;
        public string DescHorde;
        public uint LoadScreen;
        public float minimapScale;
        public uint corpseID;
        public float corpseX;
        public float corpseY;
        public uint TimeOfDay;
        public uint Expansion;
        public uint NumberOfPlayers;
        public uint ParentMap;
        public uint Unk1;
    }

    public class LoadingScreenEntry
    {
        public uint ID;
        public string Name;
        public string Path;
        public uint wscreen;
    }

    public class AreaTableEntry
    {
        public uint ID;
        public uint mapid;
        public uint parentId;
        public uint exploreFlag;
        public uint flags;
        public uint refSoundPref;
        public uint refSoundPrefUWater;
        public uint refSoundAmbi;
        public uint refZoneMusic;
        public uint refZoneIntro;
        public int area_level;
        [Localized]
        public string AreaName;
        public uint refFactionGroup;
        public uint liquidType1, liquidType2, liquidType3, liquidType4;
        public float minElevation;
        public float ambientMultiplier;
        public uint lightid;
    }

    public class AreaTableEntry_4
    {
        public uint ID;
        public uint mapid;
        public uint parentId;
        public uint exploreFlag;
        public uint flags;
        public uint refSoundPref;
        public uint refSoundPrefUWater;
        public uint refSoundAmbi;
        public uint refZoneMusic;
        public uint refZoneIntro;
        public int area_level;
        public string AreaName;
        public uint refFactionGroup;
        public uint liquidType1, liquidType2, liquidType3, liquidType4;
        public float minElevation;
        public float ambientMultiplier;
        public uint lightid;
        public uint unk1, unk2, unk3, unk4, unk5, unk6;
    }

    public class AreaTableEntry_5
    {
        public uint ID;
        public uint mapid;
        public uint parentId;
        public uint exploreFlag;
        public uint flags;
        public uint refSoundPref;
        public uint refSoundPrefUWater;
        public uint refSoundAmbi;
        public uint refZoneMusic;
        public uint refZoneIntro;
        public int area_level;
        public int unkMOP;
        public int unk7;
        public string AreaName;
        public uint refFactionGroup;
        public uint liquidType1, liquidType2, liquidType3, liquidType4;
        public float minElevation;
        public float ambientMultiplier;
        public uint lightid;
        public uint unk1, unk2, unk3, unk4, unk5, unk6;
    }

    public class Light
    {
        public uint ID;
        public uint MapID;
        public float x;
        public float y;
        public float z;
        public float falloff;
        public float falloffEnd;
        public uint skyParam;
        public uint waterParam;
        public uint sunsetParam;
        public uint otherParam;
        public uint deathParam;
        public uint unk1, unk2, unk3;
    }

    public class LightData
    {
        public uint ID;
        public uint NumValues;
        [Array(11)]
        public uint[] Times;
        [Array(11)]
        public uint[] Values;
    }

    public class LightIntBand
    {
        public uint ID;
        public uint NumEntries;
        [Array(16)]
        public uint[] Times;
        [Array(16)]
        public uint[] Values;
    }

    public class LightFloatBand
    {
        public uint ID;
        public uint NumEntries;
        [Array(16)]
        public uint[] Times;
        [Array(16)]
        public float[] Values;
    }

    public class LightParams
    {
        public uint ID;
        public uint HighlightSky;
        public uint skyboxID;
        public uint cloudID;
        public float glow;
        public float waterShallowAlpha;
        public float waterDeepAlpha;
        public float oceanShallowAlpha;
        public float oceanDeepAlpha;
    }

    public class LightParams_4
    {
        public uint ID;
        public uint HighlightSky;
        public uint skyboxID;
        public uint cloudID;
        public float glow;
        public float waterShallowAlpha;
        public float waterDeepAlpha;
        public float oceanShallowAlpha;
        public float oceanDeepAlpha;
        public float unk1;
    }

    public class LightSkyBox
    {
        public uint ID;
        public string Path;
        public uint Flags;
    }
}
