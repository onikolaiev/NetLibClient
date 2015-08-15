using netLogic.Utilities.DBC;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace netLogic.Utilities.DBC
{


    internal class MapConverter : DBC.IDBCRowConverter<MapEntry>
    {
        public MapEntry Convert(object value)
        {
            if (value is MapEntry_4)
            {
                MapEntry_4 me = value as MapEntry_4;
                MapEntry ret = new MapEntry()
                {
                    ID = me.ID,
                    AreaTable = me.AreaTable,
                    InternalName = me.InternalName,
                    Name = me.Name
                };

                return ret;
            }
            else if (value is MapEntry_5)
            {
                MapEntry_5 me = value as MapEntry_5;
                MapEntry ret = new MapEntry();
                foreach (var field in ret.GetType().GetFields())
                {
                    var field5 = me.GetType().GetField(field.Name);
                    if (field5 == null)
                        continue;

                    field.SetValue(ret, me.GetType().GetField(field.Name).GetValue(me));
                }
                return ret;
            }

            throw new InvalidOperationException();
        }

        public static Type GetRawType() { return false ? typeof(MapEntry_5) : typeof(MapEntry_4); }
    }

    internal class AreaTableConverter : DBC.IDBCRowConverter<AreaTableEntry>
    {
        public AreaTableEntry Convert(object value)
        {
            object ae = value as AreaTableEntry_4;
            if (ae == null)
                ae = value as AreaTableEntry_5;

            AreaTableEntry atbl = new AreaTableEntry();
            foreach (var field in atbl.GetType().GetFields())
            {
                field.SetValue(atbl, ae.GetType().GetField(field.Name).GetValue(ae));
            }
            return atbl;
        }

        public static Type GetRawType() { return false ? typeof(AreaTableEntry_5) : typeof(AreaTableEntry_4); }
    }

    internal class LightParamsConverter : netLogic.Utilities.DBC.IDBCRowConverter<LightParams>
    {
        public LightParams Convert(object value)
        {
            LightParams ret = new LightParams();
            foreach (var field in ret.GetType().GetFields())
            {
                field.SetValue(ret, value.GetType().GetField(field.Name).GetValue(value));
            }

            return ret;
        }
    }
}
