﻿using System;
using System.Collections;
using System.Linq;
using System.Text;
using System.Net;
using System.Net.Sockets;
using System.Text.RegularExpressions;

using netLogic.Shared;
using netLogic.Network;
using netLogic.Constants;

namespace netLogic
{
    public partial class WorldSession
    {
        private ArrayList ChatQueued = new ArrayList();

        [PacketHandlerAtribute(WorldServerOpCode.SMSG_CHANNEL_NOTIFY)]
        public void HandleChannelNotify(PacketIn packet)
        {
            Log.WriteLine(LogType.Success, "Dostalem takie gowno: {0}", packet.ReadByte());
        }

        [PacketHandlerAtribute(WorldServerOpCode.SMSG_MESSAGECHAT)]
        public void HandleChat(PacketIn packet)
        {
            try
            {
                
                string channel = null;
                UInt64 guid = 0;
                GameGuid fguid = null, fguid2 = null;
                string username = null;

                byte Type = packet.ReadByte();
                UInt32 Language = packet.ReadUInt32();

                guid = packet.ReadUInt64();
                fguid = new GameGuid(guid);
                packet.ReadInt32();

                if ((ChatMsg)Type == ChatMsg.CHAT_MSG_CHANNEL)
                {
                    channel = packet.ReadString();
                }

                if (Type == 47)
                    return;
                fguid2 = new GameGuid(packet.ReadUInt64());

                UInt32 Length = packet.ReadUInt32();
                string Message = Encoding.Default.GetString(packet.ReadBytes((int)Length));
                
                //Message = Regex.Replace(Message, @"\|H[a-zA-z0-9:].|h", ""); // Why do i should need spells and quest linked? ;>
                Message = Regex.Replace(Message, @"\|[rc]{1}[a-zA-z0-9]{0,8}", ""); // Colorfull chat message also isn't the most important thing.

                byte afk = 0;
           
                if (fguid.GetOldGuid() == 0)
                {
                    username = "System";
                }
                else
                {
                    //if (netLogicCore.ObjectMgr.objectExists(fguid))
                        //username = netLogicCore.ObjectMgr.GetObject(fguid).Name;
                }

                if (username == null)
                {
                    
                    ChatQueue que = new ChatQueue();
                    que.GUID = fguid;
                    que.Type = Type;
                    que.Language = Language;
                    if ((ChatMsg)Type == ChatMsg.CHAT_MSG_CHANNEL)
                        que.Channel = channel;
                    que.Length = Length;
                    que.Message = Message;
                    que.AFK = afk;
                    ChatQueued.Add(que);
                    QueryName(guid);
                    return;
                }

                object[] param = new object[] { (ChatMsg)Type, channel, username, Message };
                netInstance.Event(new Event(EventType.EVENT_CHAT_MSG, "0", param)); 
                //Log.WriteLine(LogType.Chat, "[{1}] {0}", Message, username);
            }
            catch (Exception ex)
            {
                Log.WriteLine(LogType.Error, "Exception Occured");
                Log.WriteLine(LogType.Error, "Message: {0}", ex.Message);
                Log.WriteLine(LogType.Error, "Stacktrace: {0}", ex.StackTrace);
            }
        }

        public void SendChatMsg(ChatMsg Type, Languages Language, string Message)
        {
            if (Type != ChatMsg.CHAT_MSG_WHISPER || Type != ChatMsg.CHAT_MSG_CHANNEL)
                SendChatMsg(Type, Language, Message, "");
        }

        public void SendChatMsg(ChatMsg Type, Languages Language, string Message, string To)
        {
            PacketOut packet = new PacketOut(WorldServerOpCode.CMSG_MESSAGECHAT);
            packet.Write((UInt32)Type);
            packet.Write((UInt32)Language);
            if ((Type == ChatMsg.CHAT_MSG_WHISPER || Type == ChatMsg.CHAT_MSG_CHANNEL) && To != "")
                packet.Write(To);
            packet.Write(Message);
            Send(packet);
        }

        public void SendEmoteMsg(ChatMsg Type, Languages Language, string Message, string To)
        {
            PacketOut packet = new PacketOut(WorldServerOpCode.CMSG_TEXT_EMOTE);
            packet.Write((UInt32)Type);
            packet.Write((UInt32)Language);
            packet.Write(Message);
            Send(packet);
        }

        public void JoinChannel(string channel, string password)
        {
            PacketOut packet = new PacketOut(WorldServerOpCode.CMSG_JOIN_CHANNEL);
            packet.Write((UInt32)0);
            packet.Write((UInt16)0);
            packet.Write(channel);
            packet.Write("");
            Send(packet);
        }
    }

    
   

    public struct ChatQueue
    {
        public GameGuid GUID;
        public byte Type;
        public UInt32 Language;
        public string Channel;
        public UInt32 Length;
        public string Message;
        public byte AFK;

    };
}
