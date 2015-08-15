using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using netLogic.Shared;
using netLogic.Network;
using netLogic.Crypt;
using netLogic.Constants;

namespace netLogic
{
    public partial class WorldSession
    {
       /* public void Attack(Player target)
        {
            PacketOut packet = new PacketOut(WorldServerOpCode.CMSG_SET_SELECTION);
            if (netLogicCore.ObjectMgr.GetPlayerObject() != null)
            {
                packet.Write(target.GetGUID().GetNewGuid());
            }
            Send(packet);

            packet = new PacketOut(WorldServerOpCode.CMSG_ATTACKSWING);
            if (netLogicCore.ObjectMgr.GetPlayerObject() != null)
            {
                packet.Write(target.GetGUID().GetNewGuid());
            }
            Send(packet);
        }*/


    }
}
