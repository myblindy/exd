using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using exd.World;
using exd.World.Helpers;

namespace exdchar
{
    static class Input
    {
        [System.Runtime.InteropServices.DllImport("user32.dll")]
        private static extern short GetAsyncKeyState(int key);

        private const int VK_LEFT = 0x25;
        private const int VK_UP = 0x26;
        private const int VK_RIGHT = 0x27;
        private const int VK_DOWN = 0x28;

        private static bool IsKeyDown(int key)
        {
            return (GetAsyncKeyState(key) & 0x8000) != 0;
        }

        public static void Process()
        {
            if (IsKeyDown(VK_RIGHT))
                GameWorld.CameraLocation = new WorldLocation(GameWorld.CameraLocation.X + 1, GameWorld.CameraLocation.Y);
            else if (IsKeyDown(VK_LEFT))
                GameWorld.CameraLocation = new WorldLocation(Math.Max(0, GameWorld.CameraLocation.X - 1), GameWorld.CameraLocation.Y);
        }

        public static void Initialize()
        {

        }
    }
}
