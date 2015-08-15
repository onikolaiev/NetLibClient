using System;
using System.Collections.Generic;
using System.Text;
using netLogic.Utilities.DBC;
namespace netLogic.Utilities
{
    /// <summary>Object to access AreaTable.dbc</summary>
   /* public class AreaTable : DBCFile
    {
        public AreaTable() : base(@"DBFilesClient\AreaTable.dbc")
        {
        }

        /// <summary>Returns Area Name for a given AreaID (eg, Northshire Valley, Sentinel Hill, Moonbrook, etc)</summary>
        public String getAreaName(uint areaid)
        {
            for (uint i = 0; i < wdbc_header.nRecords; i++)
            {
                uint id = getFieldAsUInt32(i, 0);

                if (id == areaid)
                    return getStringForField(i, 11);
            }

            throw new Exception("areaid wasn't found");
        }

        /// <summary>Returns MapID for a given AreaID (eg, Goldshire is on MapID=0, Astranaar is on MapID=1)</summary>
        public uint getMapId(uint areaid)
        {
            for (uint i = 0; i < wdbc_header.nRecords; i++)
            {
                uint id = getFieldAsUInt32(i, 0);

                if (id == areaid)
                    return getFieldAsUInt32(i, 1);
            }

            throw new Exception("areaid wasn't found");
        }

        /// <summary>Returns RegionID for a given AreaID (wtf is a RegionID though?)</summary>
        public uint getRegionID(uint areaid)
        {
            for (uint i = 0; i < wdbc_header.nRecords; i++)
            {
                uint id = getFieldAsUInt32(i, 0);

                if (id == areaid)
                    return getFieldAsUInt32(i, 2);
            }

            throw new Exception("areaid wasn't found");
        }
    }*/
}
