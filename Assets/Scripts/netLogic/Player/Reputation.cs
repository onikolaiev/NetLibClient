using System;
using System.Collections.Generic;
using System.Text;

namespace netLogic.Clients
{
    public class Reputation
    {
        private ReputationItem[] reputationList;

        public Reputation(ReputationItem[] rl)
        {
            reputationList = rl;
        }

        public override String ToString()
        {
            StringBuilder sb = new StringBuilder();

            sb.Append("Reputation Class:\n");

            return sb.ToString();
        }

    }

    public struct ReputationItem
    {
        public Byte flag;
        public UInt32 standing;
    }
}