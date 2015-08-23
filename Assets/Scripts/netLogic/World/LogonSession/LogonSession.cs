﻿using System;
using System.Text;
using System.Net;
using System.Net.Sockets;
using System.Text.RegularExpressions;
using System.IO;

using netLogic.Shared;
using netLogic.Network;
using netLogic.Crypt;
using netLogic.Constants;

namespace netLogic
{
    public class LogonSession
    {
        // Connection Info
        readonly string mHost;
        readonly int mPort;
        public string mUsername;
        private string mPassword;
        public bool Connected;

        //Packet Handling
        private PacketHandler pHandler;
        private PacketLoop pLoop = null;
        public WoWCrypt mCrypt;

        // Client stuff
        public Realm[] Realmlist = new Realm[0];
        public Char[] Charlist;
        
        // Argh, huge code of math variables here, just copy-pasted from boogiebot.
        private Srp6 srp;       // http://srp.stanford.edu/design.html  <- SRP6 information
        private BigInteger A;   // My public key?
        private BigInteger B;   // Server's public key
        private BigInteger a;   // my random number, used to initalize A from g and N.
        private byte[] I;       // Hash of "username:password"
        private BigInteger M;   // Combination of... everything!
        private byte[] M2;      // M2 is the combination of the server's everything to proof with ours (which we don't actually do, cause we trust blizzard, right?)
        private byte[] N;       // Modulus for A and B
        private byte[] g;       // base for A and B
        public byte[] mKey{get;set;}   // Yay! The key!
        
        private BigInteger Salt;    // Server provided salt
        private byte[] crcsalt;     // Server provided crcsalt for file crc's.

        public Socket mSocket = null;
        public TextWriter tw;

        //public byte[] K;        // Our combined key used for encryption of all traffic

        #region Constructors

        /// <summary>
        /// Creates LogonClient with specified data.
        /// </summary>
        /// <param name="host">Host to LogonServer</param>
        /// <param name="port">Port on which LogonServer listens for connection</param>
        /// <param name="username">Username</param>
        /// <param name="password">Password</param>
        public LogonSession(string host, int port, string username, string password)
        {
            Time.GetTime();
            mHost = host;
            mPort = port;
            mUsername = username.ToUpper();
            mPassword = password.ToUpper();
        }



        /// <summary>
        /// Creates LogonClient with specified data on default port.
        /// </summary>
        /// <param name="host">Host to LogonServer</param>
        /// <param name="username">Username</param>
        /// <param name="password">Password</param>
        public LogonSession(string host, string username, string password)
        {
            mHost = host;
            mPort = 3724;
            mUsername = username.ToUpper();
            mPassword = password.ToUpper();
        }

        public LogonSession()
        {
            mHost = "127.0.0.1";
            mPort = 3724;
            mUsername = "admin".ToUpper();
            mPassword = "admin".ToUpper();
        }


        /// <summary>
        /// Creates LogonClient with specified data, and sets host to localhost on default port.
        /// </summary>
        /// <param name="username">Username</param>
        /// <param name="password">Password</param>
        public LogonSession(string username, string password)
        {
            mHost = "localhost";
            mPort = 3724;
            mUsername = username.ToUpper();
            mPassword = password.ToUpper();
        }

        #endregion

        #region Send Methods

        public bool Connect(IPEndPoint ep)
        {
            
            try
            {
                mSocket = new Socket(AddressFamily.InterNetwork, SocketType.Stream, ProtocolType.IP);

                Log.WriteLine(LogType.Normal, "Attempting to connect to Logon Server at {0}:{1}", ep.Address, ep.Port);
                mSocket.Connect(ep);
            }
            catch (Exception ex)
            {
                Log.WriteLine(LogType.Error, "Exception Occured");
                Log.WriteLine(LogType.Error, "Message: {0}", ex.Message);
                Log.WriteLine(LogType.Error, "Stacktrace: {0}", ex.StackTrace);
                Disconnect();
                return false;
            }

            Log.WriteLine(LogType.Success, "Succesfully connected to Logon Server at {0}:{1}", ep.Address, ep.Port);

            Connected = true;
            pHandler = new PacketHandler(this);
            pLoop = new PacketLoop(this, mSocket);
            pHandler.Initialize();
            return true;
            //Authenticate();
        }

        public bool Authenticate()
        {

            PacketOut packet = new PacketOut(LogonServerOpCode.AUTH_LOGON_CHALLENGE);

            packet.Write((byte)3);
            packet.Write((UInt16)(30 + mUsername.Length));
            packet.Write((byte)'W'); packet.Write((byte)'o'); packet.Write((byte)'W'); packet.Write((byte)'\0');        // WoW

            packet.Write((byte)Config.Version.major);
            packet.Write((byte)Config.Version.minor);
            packet.Write((byte)Config.Version.update);
            packet.Write((UInt16)Config.Version.build);

            packet.Write((byte)'6'); packet.Write((byte)'8'); packet.Write((byte)'x'); packet.Write((byte)'\0');     // 68x
            packet.Write((byte)'n'); packet.Write((byte)'i'); packet.Write((byte)'W'); packet.Write((byte)'\0');     // niW

            packet.Write((byte)'B'); packet.Write((byte)'G'); packet.Write((byte)'n'); packet.Write((byte)'e');  // SUne

            packet.Write(1);

            packet.Write((byte)127); packet.Write((byte)0); packet.Write((byte)0); packet.Write((byte)1);       // Interestingly, mac sends IPs in reverse order.

            packet.Write((byte)mUsername.Length);
            packet.Write(Encoding.Default.GetBytes(mUsername)); // Name - NOT null terminated
            Send(packet);

            pLoop.Start();
            return true;

        }

        public void RequestRealmlist()
        {
            PacketOut packet = new PacketOut(LogonServerOpCode.REALM_LIST);
            packet.Write(0x00);
            Send(packet);
            
            
            // Most tricky code ever. It's so because retail server sends a lot of data in this packet... 
            byte[] temp = pLoop.OnReceive(3);
            PacketIn p1 = new PacketIn(temp, true);
            byte[] temp2 = pLoop.OnReceive(p1.ReadUInt16());
            byte[] temp3 = new byte[temp.Length + temp2.Length];
            temp.CopyTo(temp3, 0);
            temp2.CopyTo(temp3, temp.Length);

            p1 = new PacketIn(temp3, false);
            HandlePacket(p1);

        }

        public void Send(PacketOut packet)
        {
            if (Connected)
            {
                Log.WriteLine(LogType.Network, "Sending packet {0}. Length: {1}", packet.packetId.ToString(), packet.Lenght());
               // Log.WriteLine(LogType.Packet, "{0}", packet.ToHex());
                Byte[] Data = packet.ToArray();
                mSocket.Send(Data);
            }
        }

        #endregion

        #region Handlers

        [PacketHandlerAtribute(LogonServerOpCode.AUTH_LOGON_CHALLENGE)]
        public void AuthChallangeRequest(PacketIn packetIn)
        {

            packetIn.ReadByte();
            byte error = packetIn.ReadByte();
            if (error != 0x00)
            {
                Log.WriteLine(LogType.Error, "Authentication error: {0}", (AccountStatus)error);
                Disconnect();
                return;
            }

            B = new BigInteger(packetIn.ReadBytes(32));               // Varies
            byte glen = packetIn.ReadByte();                          // Length = 1
            g = packetIn.ReadBytes(glen);                             // g = 7
            byte Nlen = packetIn.ReadByte();                          // Length = 32
            N = packetIn.ReadBytes(Nlen);                             // N = B79B3E2A87823CAB8F5EBFBF8EB10108535006298B5BADBD5B53E1895E644B89
            Salt = new BigInteger(packetIn.ReadBytes(32));            // Salt = 3516482AC96291B3C84B4FC204E65B623EFC2563C8B4E42AA454D93FCD1B56BA
            crcsalt = packetIn.ReadBytes(16);                         // Varies

            

            BigInteger S;
            srp = new Srp6(new BigInteger(N), new BigInteger(g));

            do
            {
                a = BigInteger.Random(19 * 8);
                A = srp.GetA(a);

                I = Srp6.GetLogonHash(mUsername, mPassword);

                BigInteger x = Srp6.Getx(Salt, I);
                BigInteger u = Srp6.Getu(A, B);
                S = srp.ClientGetS(a, B, x, u);
            }
            while (S < 0);

            mKey = Srp6.ShaInterleave(S);
            M = srp.GetM(mUsername, Salt, A, B, new BigInteger(mKey));

            packetIn.ReadByte();

            Sha1Hash sha;
            byte[] files_crc;

            // Generate CRC/hashes of the Game Files
            if (Config.Retail)
                files_crc = GenerateCrc(crcsalt);
            else
                files_crc = new byte[20];

            // get crc_hash from files_crc
            sha = new Sha1Hash();
            sha.Update(A);
            sha.Update(files_crc);
            byte[] crc_hash = sha.Final();

            PacketOut packet = new PacketOut(LogonServerOpCode.AUTH_LOGON_PROOF);
            packet.Write(A); // 32 bytes
            packet.Write(M); // 20 bytes
            packet.Write(crc_hash); // 20 bytes
            packet.Write((byte)0); // number of keys
            packet.Write((byte)0); // unk (1.11.x)
            Send(packet);
        }

        [PacketHandlerAtribute(LogonServerOpCode.AUTH_LOGON_PROOF)]
        public void HandleLogonProof(PacketIn packetIn)
        {
            if (packetIn.ReadByte() == 0x00)
            {
                Log.WriteLine(LogType.Success, "Authenitcation successed. Requesting RealmList");
                netInstance.Event(new Event(EventType.EVENT_OK, "0", new object[] { "Succesfally connected" }));

                //Retail server sends a loooooot of informations there!
                RequestRealmlist();

                pLoop.Stop();

            }
            else {
                //Log.WriteLine(LogType.Error, "Authenitcation filed. Wrong information");
                netInstance.Event(new Event(EventType.EVENT_AUTH_FALSE, "0", new object[] { "Authenitcation filed. Wrong information" }));
                Global.GetInstance().Disconnect();
            }

        }

        [PacketHandlerAtribute(LogonServerOpCode.REALM_LIST)]
        public void HandleRealmlist(PacketIn packetIn)
        {
            //packetIn.ReadByte();
            UInt16 Length = packetIn.ReadUInt16();
            UInt32 Request = packetIn.ReadUInt32();
            int realmscount = packetIn.ReadInt16();

            //Console.Write(packetIn.ToHex());

            Log.WriteLine(LogType.Success, "Got information about {0} realms.", realmscount);
            Realm[] realms = new Realm[realmscount];
            try
            {
                for (int i = 0; i < realmscount; i++)
                {
                    realms[i].Type = packetIn.ReadByte();
                    realms[i].Color = packetIn.ReadByte();
                    packetIn.ReadByte(); // unk
                    realms[i].Name = packetIn.ReadString();
                    realms[i].Address = packetIn.ReadString();
                    realms[i].Population = packetIn.ReadFloat();
                    realms[i].NumChars = packetIn.ReadByte();
                    realms[i].Language = packetIn.ReadByte();
                    packetIn.ReadByte();
               }

                Realmlist = realms;

                LevelManager.Load("RealmServers");
                //netInstance.Event(new Event(EventType.EVENT_REALMLIST, "", new object[] { Realmlist }));
                

            }
            catch (Exception ex)
            {
                Log.WriteLine(LogType.Error, "Exception Occured");
                Log.WriteLine(LogType.Error, "Message: {0}", ex.Message);
                Log.WriteLine(LogType.Error, "Stacktrace: {0}", ex.StackTrace);
                Disconnect();
            }
        }

        #endregion


        private byte[] GenerateCrc(byte[] crcsalt)
        {
            Sha1Hash sha;
            string[] files = { "WoW.bin", "DivxDecoder.bin", "Unicows.bin" };

            byte[] buffer1 = new byte[0x40];
            byte[] buffer2 = new byte[0x40];

            for (int i = 0; i < 0x40; ++i)
            {
                buffer1[i] = 0x36;
                buffer2[i] = 0x5c;
            }

            for (int i = 0; i < crcsalt.Length; ++i)
            {
                buffer1[i] ^= crcsalt[i];
                buffer2[i] ^= crcsalt[i];
            }

            sha = new Sha1Hash();
            sha.Update(buffer1);
            
            foreach (string filename in files)
            {
                if (!File.Exists(@"crc\" + filename))
                {
                    Log.WriteLine(LogType.Error, "CRC File {0} doesn't exist!", filename);
                }

                FileStream fs = new FileStream(@"crc\" + filename, FileMode.Open, FileAccess.Read);
                byte[] Buffer = new byte[fs.Length];
                fs.Read(Buffer, 0, (int)fs.Length);
                sha.Update(Buffer);
            }
            byte[] hash1 = sha.Final();

            sha = new Sha1Hash();
            sha.Update(buffer2);
            sha.Update(hash1);
            return sha.Final();


        }


        public void HandlePacket(PacketIn packet)
        {
            //Log.WriteLine(LogType.Packet, "{0}", packet.ToHex());
            pHandler.HandlePacket(packet);
        }

        public void Disconnect()
        {
            Event e = new Event(EventType.EVENT_DISCONNECT, "", null);
            netInstance.Event(e);
        }

        public void HardDisconnect()
        {
            Connected = false;

            if (pLoop != null)
                pLoop.Stop();
            
            if (mSocket != null)
                mSocket.Close();
        }

        ~LogonSession()
        {
            HardDisconnect();
        }
        
    }
}