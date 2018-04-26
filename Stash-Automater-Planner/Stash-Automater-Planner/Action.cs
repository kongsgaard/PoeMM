using System;
using System.Collections.Generic;
using System.Text;

namespace Stash_Automater_Planner
{
    public class Action
    {
        public int mouseX = 0;
        public int mouseY = 0;

        public int delayMS = 0;

        public string action = "";

        public Action(int mX, int mY, int dMS, string A)
        {
            mouseX = mX;
            mouseY = mY;
            delayMS = dMS;
            action = A;
        }
    }
}
