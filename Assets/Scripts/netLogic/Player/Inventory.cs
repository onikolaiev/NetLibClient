using netLogic.Constants;
using System;
using System.Collections.Generic;
using System.Text;

namespace netLogic.Clients
{
    public class Inventory
    {
        public Inventory(Player po)
        {
        }

        public override String ToString()
        {
            StringBuilder sb = new StringBuilder();

            sb.Append("Inventory Class:\n");

            return sb.ToString();
        }
    }
}
