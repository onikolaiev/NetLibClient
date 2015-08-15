using System;
using System.Collections.Generic;
using System.Text;

namespace netLogic.Clients
{
    public class Spells
    {
        private SpellItem[] spellList;

        public Spells(SpellItem[] sl)
        {
            spellList = sl;
        }

        public override String ToString()
        {
            StringBuilder sb = new StringBuilder();

            sb.Append("Spell ID's:");
            foreach (SpellItem si in spellList)
                sb.Append(" " + si.spellID);
            sb.Append("\n");

            return sb.ToString();
        }

    }

    public struct SpellItem
    {
        public UInt16 spellID;
        public UInt16 slot;
    }
}

