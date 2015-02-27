using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace exd.World.Helpers
{
    public static class Logger
    {
        public static void Log(string action)
        {
            Log(action, "");
        }

        public static void Log(string action, string text)
        {
            Debug.WriteLine("[{0:0.00s} {1}] {2}", GameWorld.Now / 1000, action, text);
        }

        public static void Log(string action, string format, params object[] args)
        {
            Log(action, string.Format(format, args));
        }
    }
}
