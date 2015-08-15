using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;


namespace netLogic.Utilities.DBC
{
    public static class DBCStores
    {
        public static DBCFile<MapEntry> Map { get; private set; }
        public static DBCFile<LoadingScreenEntry> LoadingScreen { get; private set; }
        public static DBCFile<AreaTableEntry> AreaTable { get; private set; }
        public static DBCFile<Light> Light { get; private set; }
        public static DBCFile<LightIntBand> LightIntBand { get; private set; }
        public static DBCFile<LightFloatBand> LightFloatBand { get; private set; }
        public static DBCFile<LightParams> LightParams { get; private set; }
        public static DBCFile<LightSkyBox> LightSkyBox { get; private set; }
        public static DBCFile<SkillLineAbility> SkillLineAbility { get; private set; }
        public static DBCFile<SpellEntry> Spell { get; private set; }

        public static void LoadFiles()
        {
            Spell = new DBCFile<SpellEntry>("DBFilesClient\\Spell.dbc");
            Spell.LoadData();
            SkillLineAbility = new DBCFile<DBC.SkillLineAbility>(@"DBFilesClient\SkillLineAbility.dbc");
            SkillLineAbility.LoadData();
            string str = "Non 0 entries:\n";
            var qry = from dbc in SkillLineAbility.Records
                      where
                          dbc.chrRaces != 0
                      select
                      dbc.ID;

            foreach (var entry in qry)
            {
                if (Spell.ContainsKey(entry))
                    str += Spell[entry].Name + ", ";
            }

            //System.Windows.Forms.MessageBox.Show(str);
            Map = new DBCFile<MapEntry>("DBFilesClient\\Map.dbc");
            LoadingScreen = new DBCFile<LoadingScreenEntry>("DBFilesClient\\LoadingScreens.dbc");
            AreaTable = new DBCFile<AreaTableEntry>("DBFilesClient\\AreaTable.dbc");
            Light = new DBCFile<DBC.Light>("DBFilesClient\\Light.dbc");
            LightIntBand = new DBCFile<DBC.LightIntBand>("DBFilesClient\\LightIntBand.dbc");
            LightFloatBand = new DBCFile<DBC.LightFloatBand>("DBFilesClient\\LightFloatBand.dbc");
            LightParams = new DBCFile<DBC.LightParams>("DBFilesClient\\LightParams.dbc");
            LightSkyBox = new DBCFile<DBC.LightSkyBox>("DBFilesClient\\LightSkybox.dbc");

            Map.LoadData();
            LoadingScreen.LoadData();

            AreaTable.LoadData();
            Light.LoadData();
            LightFloatBand.LoadData();
            LightIntBand.LoadData();

            LightParams.LoadData();
            LightSkyBox.LoadData();
        }
    }
}
