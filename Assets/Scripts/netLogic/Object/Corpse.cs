using netLogic.Shared;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace netLogic
{
    public class Corpse : WorldObject
    {
        public Corpse()
        {
            _type = OBJECT_TYPE.TYPE_CORPSE;
            _typeId = OBJECT_TYPE_ID.TYPEID_CORPSE;
        
        }

    }
}
