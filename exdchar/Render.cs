using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using exd.World;
using exd.World.Buildings;
using exd.World.Helpers;
using exd.World.Resources;

namespace exdchar
{
    static class Render
    {
        public static void RenderScreen()
        {
            SuperConsole.SetCursorPosition(0, 0);
            SuperConsole.Write(string.Format("Map @ {0:0.00}s, ({1}):",
                GameWorld.Now / 1000, GameWorld.CameraLocation));
            RenderMap();
        }

        public static void RenderMap()
        {
            SuperConsole.SetCursorPosition(0, 1);
            for (int y = 0; y < GameWorld.ScreenSize.Height; ++y)
            {
                for (int x = 0; x < GameWorld.ScreenSize.Width; ++x)
                {
                    var placeable = GameWorld.Placeables.GetPlaceables(new WorldLocation(
                        x + GameWorld.CameraLocation.X, y + GameWorld.CameraLocation.Y));

                    if (placeable is Tree)
                        SuperConsole.Write('T');
                    else if (placeable is Storage)
                        SuperConsole.Write('S');
                    else
                        SuperConsole.Write('.');
                }
                SuperConsole.WriteLine();
            }
        }
    }
}
