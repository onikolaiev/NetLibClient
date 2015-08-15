using netLogic.Shared;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace netLogic
{
    public class Bag : Item
    {
        public Bag()
        {
            _type = OBJECT_TYPE.TYPE_CONTAINER;
            _typeId = OBJECT_TYPE_ID.TYPEID_CONTAINER;
        
        }


    }
}
