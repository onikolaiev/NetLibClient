using netLogic.Constants;
using System;
using System.Collections.Generic;
using System.Text;

namespace netLogic.Clients
{
    public class Equipped
    {
        public Equipped(Player po)
        {
        }

        public override String ToString()
        {
            StringBuilder sb = new StringBuilder();

            sb.Append("Equipped Class:\n");

            return sb.ToString();
        }
    }
}
