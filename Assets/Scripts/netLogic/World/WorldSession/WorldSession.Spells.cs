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
  /*      public void CastSpell(Player target, UInt32 SpellId)
        {
            SpellTargetFlags flags = 0;

            if (target == netLogicCore.ObjectMgr.GetPlayerObject())
                flags = SpellTargetFlags.Self;
            else
            {
                flags = SpellTargetFlags.Unit;
                //Target(target as Unit);
            }

            PacketOut packet = new PacketOut(WorldServerOpCode.CMSG_CAST_SPELL);
            packet.Write(SpellId);
            packet.Write((byte)0); // unk flags in WCell

            packet.Write((UInt32)flags);

            // 0x18A02
            if (flags.Has(SpellTargetFlags.SpellTargetFlag_Dynamic_0x10000 | SpellTargetFlags.Corpse | SpellTargetFlags.Object |
                SpellTargetFlags.PvPCorpse | SpellTargetFlags.Unit))
            {
                //packet.Write(netLogicCore.ObjectMgr.GetMMOGuidByObject(target));
            }

            // 0x1010
            if (flags.Has(SpellTargetFlags.TradeItem | SpellTargetFlags.Item))
            {
                //packet.Write(netLogicCore.ObjectMgr.GetMMOGuidByObject(target));
            }

            // 0x20
            if (flags.Has(SpellTargetFlags.SourceLocation))
            {
                packet.Write(netLogicCore.ObjectMgr.GetPlayerObject().Movement.Position.X);
                packet.Write(netLogicCore.ObjectMgr.GetPlayerObject().Movement.Position.Y);
                packet.Write(netLogicCore.ObjectMgr.GetPlayerObject().Movement.Position.Z);
            }

            // 0x40
            if (flags.Has(SpellTargetFlags.DestinationLocation))
            {
                //packet.Write(netLogicCore.ObjectMgr.GetMMOGuidByObject(target));

                packet.Write(target.Movement.Position.X);
                packet.Write(target.Movement.Position.Y);
                packet.Write(target.Movement.Position.Z);

            }

            Send(packet);
        }*/

     }
}
