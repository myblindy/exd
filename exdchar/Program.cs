using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using exd.World;
using exd.World.Helpers;
using exd.World.Resources;

namespace exdchar
{
    class Program
    {
        static void Main(string[] args)
        {
            // initialize
            SuperConsole.Initialize();
            Input.Initialize();
            GameWorld.Initialize(50, 50);
            GameWorld.ScreenSize = new WorldDimension(5, 5);

            GameWorld.Placeables.Add(new Tree(new WorldLocation(2, 2)));
            GameWorld.Placeables.Add(new Tree(new WorldLocation(1, 3)));

            // main loop
            var lasttick = DateTime.Now;
            while (true)
            {
                var now = DateTime.Now;

                if ((now - lasttick).TotalMilliseconds >= 20)
                {
                    Input.Process();
                    GameWorld.Update((now - lasttick).TotalMilliseconds);
                    Render.RenderScreen();
                    SuperConsole.FinishFrame();
                    lasttick = now;
                }
            }
        }
    }
}
