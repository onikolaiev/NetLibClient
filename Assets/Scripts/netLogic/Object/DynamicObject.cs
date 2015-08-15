using netLogic.Shared;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace netLogic
{
    public class DynamicObject : WorldObject
    {
        public DynamicObject()
        {
            _type = OBJECT_TYPE.TYPE_DYNAMICOBJECT;
            _typeId = OBJECT_TYPE_ID.TYPEID_DYNAMICOBJECT;
        
        }

    }
}
