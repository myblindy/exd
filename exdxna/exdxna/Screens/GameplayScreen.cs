using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using exdxna.GameStateManagement;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;

namespace exdxna.Screens
{
    class GameplayScreen : GameScreen
    {
        ContentManager Content;
        SpriteFont GameFont;

        double PauseAlpha;

        InputAction PauseAction;

        public GameplayScreen()
        {
            TransitionOnTime = TimeSpan.FromSeconds(1.5);
            TransitionOffTime = TimeSpan.FromSeconds(.5);

            PauseAction = new InputAction(null, new[] { Keys.Escape }, true);
        }

        public override void Activate(bool instancepreserved)
        {
            if (!instancepreserved)
            {
                if (Content == null)
                    Content = new ContentManager(ScreenManager.Game.Services, "Content");

                GameFont = Content.Load<SpriteFont>("gamefont");

                // TODO remove this
                Thread.Sleep(1000);

                ScreenManager.Game.ResetElapsedTime();
            }
        }

        public override void Unload()
        {
            Content.Unload();
        }

        public override void Update(GameTime gametime, bool otherscreenhasfocus, bool coveredbyotherscreen)
        {
            base.Update(gametime, otherscreenhasfocus, coveredbyotherscreen);

            // Gradually fade in or out depending on whether we are covered by the pause screen.
            if (coveredbyotherscreen)
                PauseAlpha = Math.Min(PauseAlpha + 1.0 / 32, 1);
            else
                PauseAlpha = Math.Max(PauseAlpha - 1.0 / 32, 0);

            if (IsActive)
            {
                // TODO game
            }
        }
    }
}
