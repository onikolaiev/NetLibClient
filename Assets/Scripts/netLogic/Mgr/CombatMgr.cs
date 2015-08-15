using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Timers;

using System.Runtime.InteropServices;
using System.Resources;
using netLogic.Network;
using netLogic.Shared;
using netLogic.Constants;
using netLogic.Terrain;

namespace netLogic
{
    public class CombatMgr
    {
        [DllImport("winmm.dll", EntryPoint = "timeGetTime")]
        public static extern uint MM_GetTime();
        bool loopStartStop;
        Thread loop;
        public List<Player> Targets = new List<Player>();
        UInt32 lastUpdateTime;

        WorldSession client;
        ObjMgr objectMgr;
        MovementMgr movementMgr;
        Boolean isFighting = false;


        public CombatMgr(ObjMgr _objectMgr, MovementMgr _movementMgr,WorldSession _session)
        {
            objectMgr = _objectMgr;
            movementMgr = _movementMgr;
            client = _session;
        }

        public void Start()
        {
            try
            {
                lastUpdateTime = MM_GetTime();

                loop = new Thread(Loop);
                loop.IsBackground = true;
                loop.Start();
                loopStartStop = true;
            }
            catch (Exception ex)
            {
                Log.WriteLine(LogType.Error, "Exception Occured");
                Log.WriteLine(LogType.Error, "Message: {0}", ex.Message);
                Log.WriteLine(LogType.Error, "Stacktrace: {0}", ex.StackTrace);
            }
        }

        public void Stop()
        {
            if (loop != null)
            {
                loop.Abort();
                Targets = null;
                loopStartStop = false;
            }
        }

        void Loop()
        {
        /*    while (loopStartStop)
            {
                try
                {
                    if (Targets.Count > 0)
                    {
                        MMOObject target = Targets.First();
                        float dist = TerrainMgr.CalculateDistance(netLogicCore.ObjectMgr.GetPlayerObject().Movement.Position, target.Movement.Position);
                        if (dist > 1)
                        {
                            movementMgr.Waypoints.Add(target.Movement.Position);
                        }
                        else if (dist < 1 && !isFighting)
                        {
                            client.Attack(target);
                            isFighting = true;
                        }
                        /*else if (isFighting && target.Health < 0)
                        {
                            isFighting = false;
                            Targets.Remove(target);
                        }
                        else if (isFighting && target.Health > 0)
                        {
                            Console.WriteLine(target.Health);
                        }*/
                    /*}
                }
                catch (Exception ex)
                {
                    Log.WriteLine(LogType.Error, "Exception Occured");
                    Log.WriteLine(LogType.Error, "Message: {0}", ex.Message);
                    Log.WriteLine(LogType.Error, "Stacktrace: {0}", ex.StackTrace);
                }
            }*/
        }
    }
}
