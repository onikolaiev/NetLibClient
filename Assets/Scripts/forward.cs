using UnityEngine;
using System.Collections;
using netLogic;
using netLogic.Constants;
using netLogic.Network;
using System;
public class forward : MonoBehaviour {

	// Use this for initialization
	void Start () {
	
	}
	
	// Update is called once per frame
	void Update () {
	
	}

   
    public void BuildMovePacket()
    {
        PacketOut outpacket = new PacketOut(WorldServerOpCode.MSG_MOVE_JUMP);
       /// outpacket.Write(netLogicCore.ObjectMgr.GetPlayerObject().GUID.GetOldGuid());

        outpacket.Write((UInt32)0);
        outpacket.Write((byte)255);
        outpacket.Write((UInt32)0);

        outpacket.Write(0);
        outpacket.Write(0);
        outpacket.Write(0);
        outpacket.Write(0);

        outpacket.Write((UInt32)0);

        //netInstance.GetWSession().Send(outpacket);
    }


}
