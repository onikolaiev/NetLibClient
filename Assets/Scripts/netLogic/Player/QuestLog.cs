using netLogic.Constants;
using System;
using System.Collections.Generic;
using System.Text;

namespace netLogic.Clients
{
    public class QuestLog
    {
        //private QuestItem[] questItemList;

        public QuestLog(Player po)
        {
        }

        public override String ToString()
        {
            StringBuilder sb = new StringBuilder();

            sb.Append("QuestLog Class:\n");

            return sb.ToString();
        }
    }

    public struct QuestItem
    {
        public UInt32 unk1;
        public UInt32 unk2;
        public UInt32 unk3;
    }
}