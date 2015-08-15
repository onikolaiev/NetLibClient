using System;
using System.Collections.Generic;
using System.Text;

namespace netLogic.Clients
{
    public class ActionBars
    {
        // Some #defines
        public static int MaxButtons = 120;
        public static int ButtonsPerBar = 12;
        public static int NumOfBars = MaxButtons / ButtonsPerBar;

        private ActionButton[] buttons;

        public ActionBars(ActionButton[] abl)
        {
            buttons = abl;
        }

        // Populate some junk onto the action bars. (called if the bars are empty or mostly empty)
        public void populateBars()
        {
        }

        // Add supplied spell to the bar (if there's room). Called when we learn a new spell at the trainer, etc.
        public void addSpellToBar()
        {
        }

        // Add supplied item to the bar (if there's room). Called when we pick up an important item (eg, mount?)
        public void addItemToBar()
        {
        }

        public override String ToString()
        {
            StringBuilder sb = new StringBuilder();

            sb.Append("ActionBars Class:\n");

            return sb.ToString();
        }
    }

    public struct ActionButton
    {
        public UInt16 action;
        public byte type;
        public byte misc;
    }
}

// Message to send to server for button placements
// CMSG_SET_ACTION_BUTTON = 296,  //(0x128)
