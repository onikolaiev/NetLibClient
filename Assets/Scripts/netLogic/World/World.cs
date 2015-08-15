using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using netLogic;

namespace netLogic
{
    public class World
    {
        private WorldSession _session;
        private  MapMgr _mapmgr = null;
        private  UInt32 _mapId;
        private  float _x,_y;
        private  float _lastx,_lasty;
        private  MovementMgr _movemgr;


        public UInt32 GetMapId() { return _mapId; }
        public WorldSession GetWSession() { return _session; }
        public MapMgr GetMapMgr() { return _mapmgr; }
        public MovementMgr GetMoveMgr() { return _movemgr; }


        public World(WorldSession _s)
        {
            _session = _s;
            _mapId = 0;
            _mapmgr = null;
            _movemgr = null;
            _mapmgr = new MapMgr();
            
        }

        ~World()
        {
            Clear();
            if(_mapmgr!=null)
                _mapmgr = null;
        }

        // called on SMSG_NEW_WORLD
        void Clear()
        {
            if(_mapmgr!=null)
            {
              //  _mapmgr.Flush();
            }
            // TODO: clear WorldStates (-> SMSG_INIT_WORLD_STATES ?) and everything else thats required
        }

        void Update()
        {
            if(_mapId == 0) // to prevent unexpected behaviour
                return;

            if(_mapmgr!=null)
            {
                //_mapmgr.Update(_x,_y,_mapId);
            }
            if(_movemgr!=null)
            {
              //  _movemgr.Update(false);
            }

            // some debug code for testing...
            /*if(_mapmgr && _x != _lastx || _y != _lasty)
            {
                logdetail("WORLD: relocation, to x=%f y=%f, calculated z=%f",_x,_y,this->GetPosZ(_x,_y));
                _lastx = _x;
                _lasty = _y;
            }*/

        }

        void UpdatePos(float x, float y, UInt32 m)
        {
            _mapId = m;
            UpdatePos(x,y);
        }

        void UpdatePos(float x, float y)
        {
            _x = x;
            _y = y;
            Update();
        }

    /*    float GetPosZ(float x, float y)
        {
            if(_mapmgr!=null)
                return _mapmgr.GetZ(x,y);

         //   logdebug("WORLD: GetPosZ() called, but no MapMgr exists (do you really use maps?)");
            return 0;
        }*/

        // must be called after MyCharacter is created
        void CreateMoveMgr()
        {
        //    _movemgr = new MovementMgr();
        //    _movemgr.SetInstance(_session.GetInstance());
        }
    }
}
