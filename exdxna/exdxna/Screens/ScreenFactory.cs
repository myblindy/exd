using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace exdxna.GameStateManagement.Screens
{
    /// <summary>
    /// Our game's implementation of IScreenFactory which can handle creating the screens
    /// when resuming from being tombstoned.
    /// </summary>
    public class ScreenFactory : IScreenFactory
    {
        public GameScreen CreateScreen(Type screenType)
        {
            // All of our screens have empty constructors so we can just use Activator
            return Activator.CreateInstance(screenType) as GameScreen;
        }
    }
}
