using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Text;
using System.Net;
using System.Net.Sockets;
using System.Text.RegularExpressions;
using System.IO;
using System.Xml;
using Microsoft.Win32;
using netLogic.Clients;
using netLogic.Shared;
using netLogic.Terrain;
using netLogic.Constants;
using netLogic.Common;
using UnityEngine;





namespace netLogic
{
    public static class Global
    {
        public static netInstance GetInstance()
        {
            netInstance ni = (netInstance)Loom.DispatchToMainThreadReturn(()=>{ return GameObject.Find("_start").GetComponent<netInstance>();});


            return ni;
        }

        public static void WriteInfo(string _str)
        {
            Loom.DispatchToMainThread(() =>
            {
                GameObject.Find("InfoLable").GetComponent<UILabel>().text = _str;
            });
        
        }
    
    }

    public partial class netInstance : MonoBehaviour
    {
        #region Objects accessible to anyone
        public static IniReader configFile;            // For reading the bots .ini file
        private static ObjMgr objMgr;                   // For accessing and modifying World Objects, including the player
        private static CombatMgr combatMgr;                     // For accessing and modifying World Objects, including the player
        private static MovementMgr movementMgr;                     // For accessing and modifying World Objects, including the player
        private static TerrainMgr terrainMgr;
        //public static AreaTable areaTable;             // For accessing areatable.dbc
        //public static MapTable mapTable;               // For accessing map.dbc
        //public static CallBackLog Log;                 // For sending logtext to the UI
        public static CallBackEvent Event;             // For sending events to the UI
        public static String wowPath;                  // Obvious
        public static WoWVersion wowVersion;           // WoW.exe / WoW.app version information
        public static WoWType wowType;                 // WoWtype. (win32, OSX - ppc or x86, etc)
        #endregion

        #region Internal stuff
        private static Boolean inited = false;
        public static Boolean _PlayerInited = false;
        private static LogonSession logonSession;      // My Pvt handle to the RealmListClient
        private static WorldSession worldSession;  // My Pvt handle to the WorldServerClient
        private static Player player;                   // For accessing anything Player related.
        private static String BoogieBotConfigFileName = "netLogic.ini";
        #endregion



        #region Unity logic

        public GameObject pass;
        public GameObject login;
        public GameObject scene;
        public GameObject _instance;
        public string levelName = "";
        public string username;
        public string password;
       
        UIInput passwd;

        void Awake()
        {
            Loom.CreateThreadPoolScheduler();
            InitCore(EventHandler);
            DontDestroyOnLoad(this);
            DontDestroyOnLoad(GameObject.Find("InfoLabel"));
        }

        // Use this for initialization
        void Start()
        {
            
            
        }

        // Update is called once per frame
        void Update()
        {
            
            if (logonSession==null)
            {
                username = login.GetComponent<UILabel>().text;
                password = pass.GetComponent<UIInput>().value;
            }
            
        }



        public delegate void myEvent(Realm[] rlist);

        static void HandleChatMsg(ChatMsg msg, string v1, string v2, string v3)
        {
            //  Debug.Log.WriteLine(LogType.Log, "[{0}][{1}][{2}]{3}", msg, v1, v2, v3);
        }

        delegate void EventInvoke(netLogic.Event e);
        // Event Handler
        public static void EventHandler(netLogic.Event e)
        {
            EventHandle(e);

        }

        public static void EventHandle(netLogic.Event e)
        {
            switch (e.eventType)
            {
                case EventType.EVENT_REALMLIST:


                    LevelManager.Load("RealmServers");
                    //  UnityThreadHelper.Dispatcher.Dispatch(() => { LevelManager.Load("RealmServers"); });

                    break;
                case EventType.EVENT_CHAR_LIST:

                    // HandleCharlist((List<Character>)e.eventArgs[0]);
                    break;
                case EventType.EVENT_LOG:
                    UnityEngine.Debug.Log((String)e.eventArgs[0] + "\n\r");
                    //  Console.ForegroundColor = ConsoleColor.Gray;
                    break;
                case EventType.EVENT_CHAT_MSG:
                    HandleChatMsg((ChatMsg)e.eventArgs[0], (string)e.eventArgs[1], (string)e.eventArgs[2], (string)e.eventArgs[3]);
                    break;
                case EventType.EVENT_ERROR:
                    //MessageBox.Show((string)e.eventArgs[0], "Error!");
                    break;
                case netLogic.EventType.EVENT_AUTH_FALSE:
                    Global.WriteInfo((string)e.eventArgs[0]);
                    break;
                case netLogic.EventType.EVENT_OK:
                    Global.WriteInfo((string)e.eventArgs[0]);
                    break;
                case EventType.EVENT_DISCONNECT:
                    //HandleDisconnect();
                    break;
            }
        }

        void OnApplicationQuit()
        {
            Global.GetInstance().GetLSession().HardDisconnect();
            Global.GetInstance().GetWSession().HardDisconnect();
        }

        #endregion

        #region Initialize the Core :D
        public void InitCore(CallBackEvent e)
        {
            // Can't run this more than once
            if (inited) return;

            // We need to be able to Log stuff
            //if (l == null) return;
            if (e == null) return;

            //Log = l;
            Event = e;

            Log.WriteLine(netLogic.Shared.LogType.Debug, "netLogic.dll Initializing...");

            // Initialize everything
            try
            {
                if (!File.Exists(Environment.CurrentDirectory + "/" + BoogieBotConfigFileName))
                    throw new FileNotFoundException("Configuration file not found.", BoogieBotConfigFileName);

                configFile = new IniReader(Environment.CurrentDirectory + "/netLogic.ini");

                // NOTE: Set any OS specific variables so things can be done differently later, ie. Windows or Linux, etc.
                OperatingSystem os = Environment.OSVersion;
                switch (os.Platform)
                {
                    case PlatformID.Win32Windows:
                        Log.WriteLine(netLogic.Shared.LogType.Debug, "> Operating System: Windows");
                        break;
                    case PlatformID.Win32NT:
                        Log.WriteLine(netLogic.Shared.LogType.Debug, "> Operating System: Windows NT");
                        break;
                    case PlatformID.Unix:
                        Log.WriteLine(netLogic.Shared.LogType.Debug, "> Operating System: Unix");
                        break;
                }
                Log.WriteLine(netLogic.Shared.LogType.Debug, "> OS Version: {0}.{1}  (Build: {2})  ({3})", os.Version.Major, os.Version.Minor, os.Version.Build, os.ServicePack);

                // Find WoW's Folder
                wowPath = getWowPath();
                Log.WriteLine(netLogic.Shared.LogType.Debug, "> WowPath: {0}", wowPath);

                // Find WoW's Version
                wowVersion = getWoWVersion();
                Log.WriteLine(netLogic.Shared.LogType.Debug, "> WoW Version: World of Warcraft {0}.{1}.{2}.{3} ({4}) Found!  Emulating this version.", wowVersion.major, wowVersion.minor, wowVersion.update, wowVersion.build, netInstance.WowTypeString);


                //objectMgr = new ObjectMgr();
                GameObject _objects = new GameObject("objMgr");
                _objects.AddComponent<ObjMgr>();
                objMgr = _objects.GetComponent<ObjMgr>();
                DontDestroyOnLoad(objMgr);

                //movementMgr = new MovementMgr(netLogicCore.ObjectMgr);
                //combatMgr = new CombatMgr(ObjectMgr,MovementMgr);
                //terrainMgr = new TerrainMgr();
//              
                UpdateFieldsLoader.LoadUpdateFields(wowVersion.build);

                //areaTable = new AreaTable();
                //mapTable = new MapTable();
            }
            catch (Exception ex)
            {
                // Bot Start up Failed. Log why, and rethrow the exception.
                Log.WriteLine(netLogic.Shared.LogType.Debug, ex.Message);
                Log.WriteLine(netLogic.Shared.LogType.Debug, ex.StackTrace);

                throw new Exception("BoogieBot.dll Init Failure.", ex);
            }

            inited = true;
            Log.WriteLine(netLogic.Shared.LogType.Debug, "BoogieBot.dll Initialized.");
        }

        public Boolean ConnectToLogonServer(string username,string password)
        {
            if (!inited)
                throw new Exception("Run BoogieCore.Init() first.");

            if (logonSession != null)
                throw new Exception("Already connected?");

            IPAddress RLAddr;

            string Address = configFile.ReadString("Connection", "Host", "us.logon.worldofwarcraft.com");
            int Port = configFile.ReadInteger("Connection", "Port", 3724);

            Regex DnsMatch = new Regex("[a-zA-Z]");

            if (DnsMatch.IsMatch(Address))
                RLAddr = Dns.GetHostEntry(Address).AddressList[0];
            else
                RLAddr = System.Net.IPAddress.Parse(Address);

            IPEndPoint RLDest = new IPEndPoint(RLAddr, Port);

            Log.WriteLine(netLogic.Shared.LogType.Debug, "Attempting connection to Realm List Server at {0}.", Address);

            try
            {
                logonSession = new LogonSession(username, password);

                if (!logonSession.Connect(RLDest))
                {
                    logonSession = null;
                    return false;
                }

                if (!logonSession.Authenticate())
                {
                    logonSession = null;
                    return false;
                }
            }
            catch (Exception ex)
            {
                Log.WriteLine(netLogic.Shared.LogType.Debug, "Failed to maintain connection with realm list server. Details below:\n{0}", ex.Message);
                logonSession = null;
                return false;
            }

            return true;
        }

        public void ConnectToWorldServer()
        {
            if (!inited)
                throw new Exception("Run BoogieCore.Init() first.");

            if (worldSession != null)
                throw new Exception("Already connected?");

            if (logonSession.mUsername.Length > 2 && logonSession.mKey.Length >= 16)
            {
                worldSession = new WorldSession(GetLSession().Realmlist[0], logonSession.mUsername, logonSession.mKey, this);
                //worldSession.Connect();
            }
            else
                Log.WriteLine(netLogic.Shared.LogType.Error, "Unable to login to Game server! Unknown error occured. Check above!");
        }

        public void Disconnect()
        {
            if (!inited)
                throw new Exception("Run BoogieCore.Init() first.");

            if (logonSession != null)
            {
                // Stop the WS thread and wait till it ends.
                logonSession.HardDisconnect(); // causes deadlocks atm.
                logonSession = null;
            }

            if (worldSession != null)
            {
                // Stop the WS thread and wait till it ends.
                worldSession.StopThread(false); // causes deadlocks atm.
                worldSession = null;
            }
        }
        #endregion

        #region Properties
        public LogonSession GetLSession()            {  return logonSession; } 
        public WorldSession GetWSession()            {  return worldSession;  }
        public ObjMgr ObjMgr()                       {  return objMgr; } 
        public MovementMgr MovementMgr()             {  return movementMgr; } 
        public TerrainMgr TerrainMgr()               {  return terrainMgr; } 
        public CombatMgr CombatMgr()                 {  return combatMgr; } 
        public Player Player()                       {  return player; } 
        #endregion


        public static String WowTypeString
        {
            get
            {
                switch (netInstance.wowType)
                {
                    case WoWType.OSXppc:
                        return "OSX/ppc";
                    case WoWType.OSXx86:
                        return "OSX/x86";
                    case WoWType.Win32:
                        return "WIN32/x86";
                    default:
                        return "Unknown";
                }
            }
        }

        // Determine the Wow path. Will try to read it from the INI, or from the Registry. INI overrides Registry.
        // (ie. you may have multiple wow installations and want to use a specific one)
        private static String getWowPath()
        {
            String wowPath = configFile.ReadString("WoW", "Wowpath");

            //if(BoogieCore.wowType != WoWType.Win32) return;  // If its not windows wow, it won't be in the registry ;p
            // Okay maybe i'll keep that commented out; I just place my WoW.app into my windows wow folder LOL.

            if (wowPath.Equals(""))
            {
                RegistryKey rootKey = RegistryKey.OpenRemoteBaseKey(Microsoft.Win32.RegistryHive.LocalMachine, "");
                RegistryKey blizKey = rootKey.OpenSubKey(@"SOFTWARE\Blizzard Entertainment\World of Warcraft");

                if (blizKey != null)
                    wowPath = (String)blizKey.GetValue("InstallPath");

                if (wowPath == null || wowPath == "")
                    throw new Exception("Couldn't determine wowpath! Please set it in your boogiebot.ini file.");
            }

            return wowPath;
        }

        // Determine what version of WoW we're running.
        private static WoWVersion getWoWVersion()
        {
            WoWType wowType = (WoWType)configFile.ReadInteger("WoW", "WowType");

            switch (wowType)
            {
                case WoWType.OSXppc:
                    netInstance.wowType = wowType;
                    return getWoWVersion_OSX();
                case WoWType.OSXx86:
                    netInstance.wowType = wowType;
                    return getWoWVersion_OSX();
                case WoWType.Win32:
                    netInstance.wowType = wowType;
                    return getWoWVersion_Win32();
                default:
                    throw new Exception("Please fix wowtype in your config file.");
            }
        }

        // Find Windows WoW Version using FileVersionInfo
        private static WoWVersion getWoWVersion_Win32()
        {
            try
            {
                FileVersionInfo wowExeInfo = FileVersionInfo.GetVersionInfo(wowPath + @"\WoW.exe");
                return new WoWVersion((byte)wowExeInfo.FileMajorPart, (byte)wowExeInfo.FileMinorPart, (byte)wowExeInfo.FileBuildPart, (ushort)wowExeInfo.FilePrivatePart);
            }
            catch (Exception)
            {
                throw new Exception("Couldn't open wow.exe. Check that it exists, and wowpath is set correctly.");
            }
        }

        // Find OSX WoW Version, by reading in World of Warcraft.app's Info.plist. Fuck I hate XML though.
        private static WoWVersion getWoWVersion_OSX()
        {
            XmlTextReader reader;

            try
            {
                reader = new XmlTextReader(netInstance.wowPath + @"\World of Warcraft.app\Contents\Info.plist");
            }
            catch
            {
                throw new Exception("Couldn't open WoW.app. Check that it exists, and wowpath is set correctly.");
            }

            String previousText = "";

            while (reader.Read())
            {
                switch (reader.NodeType)
                {
                    case XmlNodeType.Element:
                        break;

                    case XmlNodeType.Text:
                        if (previousText.Equals("BlizzardFileVersion"))
                            return new WoWVersion(reader.Value);
                        previousText = reader.Value;
                        break;
                }
            }

            throw new Exception("Couldn't figure out World of Warcraft.app's version.");
        }
    }
    // End of BoogieCore class!!


    // Delegates - Used to make calls to the UI from this .dll
    public delegate void CallBackEvent(Event e);
    public delegate void CallBackLog(netLogic.Shared.LogType logType, string format, params object[] parameters);

    // Different Types of WoW (platform/OS combiantions)
    public enum WoWType
    {
        Win32 = 0,
        OSXppc = 1,
        OSXx86 = 2
    }

    // Log types. Feel free to add more!
    /*public enum LogType
    {
        Error,              // Error Message. (This should be saved for fatal messages?)
        System,             // System Message
        SystemDebug,        // System Debug Message
        NeworkComms,        // Network Communications
        FileDebug           // File reading debug info
    }*/

    // Event types. You'll definitely be adding more.
    public enum EventType
    {
        EVENT_REALMLIST,
        EVENT_CHAR_LIST,
        EVENT_CHAR_CREATE_RESULT,
        EVENT_CHAT,
        EVENT_CHANNEL_JOINED,
        EVENT_CHANNEL_LEFT,
        EVENT_SELF_MOVED,
        EVENT_LOCATION_UPDATE,
        EVENT_FRIENDS_LIST,
        EVENT_IGNORE_LIST,
        EVENT_FRIEND_STATUS,
        EVENT_NAMEQUERY_RESPONSE,
        EVENT_ADD_OBJECT,
        EVENT_DEL_OBJECT,
        EVENT_UDT_OBJECT,
        EVENT_LOG,
        EVENT_CHAT_MSG,
        EVENT_ERROR,
        EVENT_DISCONNECT,
        EVENT_AUTH_FALSE,
        EVENT_OK
    }

    public class Event
    {
        public EventType eventType;
        public string eventTime;
        public object[] eventArgs;

        public Event(EventType type, string time, params object[] parms)
        {
            eventType = type;
            eventTime = time;
            eventArgs = parms;
        }
    }

    public struct WoWVersion
    {
        public WoWVersion(byte a, byte b, byte c, ushort d)
        {
            major = a; minor = b; update = c; build = d;
        }

        public WoWVersion(String versionString)
        {
            String[] versionParts = versionString.Split(new char[] { '.' });
            Byte.TryParse(versionParts[0], out major);
            Byte.TryParse(versionParts[1], out minor);
            Byte.TryParse(versionParts[2], out update);
            UInt16.TryParse(versionParts[3], out build);
        }

        // DON'T CHANGE THE TYPE ON THESE FIELDS
        // (otherwise u need to fix RealmListClient.Auth.cs)
        public byte major;
        public byte minor;
        public byte update;
        public ushort build;
    }
}
