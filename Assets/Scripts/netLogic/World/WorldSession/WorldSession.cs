using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Net;
using System.Net.Sockets;
using System.Timers;
using System.IO;
using System.Threading;
using System.Runtime.InteropServices;
using System.Resources;

using netLogic.Shared;
using netLogic.Network;
using netLogic.Crypt;
using netLogic.Constants;
using netLogic.Terrain;
using System.Collections;
using UnityEngine;

namespace netLogic
{
    public struct CharacterListExt
{
    public PlayerEnum p;
    public string zone;
    public string class_;
    public string race;
    public string map_;
};

    public  struct WhoListEntry
{
       public string name;
       public string gname;
       public UInt32 level;
       public UInt32 classId;
       public UInt32 raceId;
       public UInt32 zoneId;
};



    public partial class WorldSession : MonoBehaviour
    {
        private UInt32 ServerSeed;
        private UInt32 ClientSeed;
        private System.Random random = new System.Random();
        public Socket mSocket = null;
        private Socket mWardenSocket;
        private static string WardenHost = "127.0.01";
        private static int WardenPort = 4141;



        [DllImport("winmm.dll", EntryPoint = "timeGetTime")]
        public static extern uint MM_GetTime();

        private System.Timers.Timer aTimer = new System.Timers.Timer();
        private System.Timers.Timer uTimer = new System.Timers.Timer();
        private UInt32 Ping_Seq;
        private UInt32 Ping_Req_Time;
        private UInt32 Ping_Res_Time;
        public UInt32 Latency;

        // Connection Info
        readonly string mUsername;
        private byte[] mKey;
        public bool Connected;
        public Realm realm;

        //Packet Handling
        private PacketHandler pHandler;
        private PacketLoop pLoop = null;
        public  PacketCrypt mCrypt;


        private List<WhoListEntry> _whoList = new List<WhoListEntry>();
        private List<CharacterListExt> _charList = new List<CharacterListExt>();



        private bool _logged, _mustdie; // world status
        public bool _chSelected; // world status
     //   Channel _channels;
        private GameGuid _myGUID;
        private World _world;
        private UInt32 _lag_ms;
        private netInstance _instance;
        private PlayerEnum _myPlayerEnum { get; set; }



        public UInt64 GetTarget()                   { return GetMyChar() ? GetMyChar().GetTarget() : 0; }
        public GameGuid GetMyGuid()                 { return _myGUID; }
        public void SetMyGuid() { GameGuid _myGUID; }
        public MyCharacter GetMyChar()              { if (_myGUID.GetOldGuid() > 0) return (MyCharacter)GetInstance().ObjMgr().GetObj(_myGUID); else return null; }
        public netInstance GetInstance()            { return _instance; }
        public UInt32 GetCharsCount()               { return (uint)_charList.Count; }
        public List<CharacterListExt> GetCharList() { return _charList; }
        public List<WhoListEntry> GetWhoList()      { return _whoList; }
        public World GetWorld()                     { return _world; }
        public bool InWorld()                       { return _logged; }
        public void SetMyPlayerEnum(PlayerEnum _ch) { _myPlayerEnum = _ch; _chSelected = true; _myGUID = _ch.GUID; }
        public PlayerEnum GetPlayerEnum()
        {
            return _myPlayerEnum;
        }
        public PlayerEnum GetPlayerEnum(string _name)
        {
            List<CharacterListExt> _list = GetCharList();
            foreach (CharacterListExt _ch in _list)
            { 
                if(_ch.p.Name == _name)
                {
                    return _ch.p;
                }
            
            }
            return new PlayerEnum();
        }
        



        public void _OnEnterWorld() { if (!InWorld()) { _logged = true; } }
        public void _OnLeaveWorld() { if (InWorld()) { _logged = false; } }



        public WorldSession(Realm rl, string username, byte[] key, netInstance _ins)
        {
            mUsername = username.ToUpper();
            realm = rl;
            mKey = key;
            _instance = _ins;
            _logged = false;
            _world = new World(this);
            //objmgr.SetInstance(in);
            _lag_ms = 0;
        }

        public void Connect()
        {
            string[] address = realm.Address.Split(':');
            byte[] test = new byte[1];
            test[0] = 10;
            mCrypt = new PacketCrypt(test);
            IPAddress WSAddr = Dns.GetHostAddresses(address[0])[0];
            IPAddress wWSAddr = Dns.GetHostAddresses(WardenHost)[0];
            int WSPort = Int32.Parse(address[1]);
            IPEndPoint ep = new IPEndPoint(WSAddr, WSPort);
            IPEndPoint wep = new IPEndPoint(wWSAddr, WardenPort);
            
            try
            {
                mSocket = new Socket(AddressFamily.InterNetwork, SocketType.Stream, ProtocolType.IP);
                mSocket.Connect(ep);
                Log.WriteLine(netLogic.Shared.LogType.Success, "Successfully connected to WorldServer at: {0}!", realm.Address);

            }
            catch (SocketException ex)
            {
                Log.WriteLine(netLogic.Shared.LogType.Error, "Failed to connect to realm: {0}", ex.Message);
                Disconnect();
                return;
            }

           /* try
            {
                mWardenSocket = new Socket(AddressFamily.InterNetwork, SocketType.Stream, ProtocolType.IP);
                mWardenSocket.Connect(wep);
            }
            catch (SocketException ex)
            {
                Log.WriteLine(LogType.Error, "Failed to connect to WardenServer: {0}", ex.Message);
                return;
            }*/

            byte[] nullA = new byte[24];
            mCrypt = new PacketCrypt(nullA);
            Connected = true;
            pHandler = new PacketHandler(this);
            pHandler.Initialize();

            

            pLoop = new PacketLoop(this, mSocket);
            pLoop.Start();
            Thread.Sleep(1000);
            
        }

        void PingLoop()
        {
            aTimer.Elapsed += new ElapsedEventHandler(Ping);
            aTimer.Interval = 10000000;
            aTimer.Enabled = true;

            Ping_Seq = 10;
            Latency = 10;
        }

        void Ping(object source, ElapsedEventArgs e)
        {
            while(!mSocket.Connected)
            {
                aTimer.Enabled = false;
                aTimer.Stop();
                return;
            }

            Ping_Req_Time = MM_GetTime();

            PacketOut ping = new PacketOut(WorldServerOpCode.CMSG_PING);
            ping.Write(Ping_Seq);
            ping.Write(Latency);
            Send(ping);
        }

        public void Send(PacketOut packet)
        {
            try
            {
                if (!Connected)
                    return;
                Log.WriteLine(netLogic.Shared.LogType.Network, "Sending packet: {0}", packet.packetId);
                if (!Connected)
                    return;
                Byte[] Data = packet.ToArray();

                int Length = Data.Length;
                byte[] Packet = new byte[2 + Length];
                Packet[0] = (byte)(Length >> 8);
                Packet[1] = (byte)(Length & 0xff);
                Data.CopyTo(Packet, 2);
                mCrypt.Encrypt(Packet, 0, 6);
                //While writing this part of code I had a strange feeling of Deja-Vu or whatever it's called :>

               // Log.WriteLine(LogType.Packet,"{0}", packet.ToHex());
                mSocket.Send(Packet);
            }
            catch (Exception ex)
            {
                Log.WriteLine(netLogic.Shared.LogType.Error, "Exception Occured");
                Log.WriteLine(netLogic.Shared.LogType.Error, "Message: {0}", ex.Message);
                Log.WriteLine(netLogic.Shared.LogType.Error, "Stacktrace: {0}", ex.StackTrace);
            }
            
        }

        public void StartHeartbeat()
        {
            aTimer.Elapsed += new ElapsedEventHandler(Heartbeat);
            aTimer.Interval = 3000;
            aTimer.Enabled = true;
        }

        public void HandlePacket(PacketIn packet)
        {
            //Log.WriteLine(LogType.Packet, "{0}", packet.ToHex());
            pHandler.HandlePacket(packet);
           
        }

        public void StopThread(bool wait)
        {
            Connected = false;

            if (mSocket.Connected)
                mSocket.Disconnect(false);

           // if (wait)   // Wait till this thread shuts down.
            //    WorldThread.Join();
        }

        public void Disconnect()
        {
            Event e = new Event(EventType.EVENT_DISCONNECT, "", null);
            netInstance.Event(e);
        }

        public void HardDisconnect()
        {
            if (mSocket != null && mSocket.Connected)
                mSocket.Close();

            if (GetInstance().MovementMgr()!=null)
                GetInstance().MovementMgr().Stop();
            if (GetInstance().CombatMgr() != null)
                GetInstance().CombatMgr().Stop();
            if (pLoop != null)
                pLoop.Stop();
            Connected = false;
        }

        ~WorldSession()
        {
            HardDisconnect();
        }
    }
}
