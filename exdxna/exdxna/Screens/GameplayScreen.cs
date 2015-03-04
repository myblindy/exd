using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using exd.World;
using exd.World.AI;
using exd.World.Helpers;
using exd.World.Resources;
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
        Texture2D GrassTexture;

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

                InitializeWorld();
                ScreenManager.Game.ResetElapsedTime();
            }
        }

        private void InitializeWorld()
        {
            GameWorld.Initialize(100, 100);

            var bounds = ScreenManager.GraphicsDevice.Viewport.Bounds;
            GameWorld.ScreenSize = new WorldDimension((long)(bounds.Width / ExdSettings.TileSizePx), (long)(bounds.Height / ExdSettings.TileSizePx));

            // add some trees
            GameWorld.Placeables.Add(Enumerable.Range(0, 10).SelectMany(i => Enumerable.Range(10, 3).Select(j =>
                new Tree(new WorldLocation(i, j)))));

            // and a few actors
            GameWorld.Placeables.Add(new[]{
                new Actor(new WorldLocation(2,0)){Name="Steve"},
                new Actor(new WorldLocation(5,0)){Name="Martha"}});
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
                GameWorld.Update(gametime.ElapsedGameTime.TotalMilliseconds);
        }

        public override void Draw(GameTime gametime)
        {
            var batch = ScreenManager.SpriteBatch;
            var bounds = ScreenManager.GraphicsDevice.Viewport.Bounds;

            batch.Begin();
            for (int x = 0; x < bounds.Width / ExdSettings.TileSizePx; ++x)
                for (int y = 0; y < bounds.Height / ExdSettings.TileSizePx; ++y)
                    batch.DrawRectangle(new Rectangle((int)(GameWorld.CameraLocation.X + x * ExdSettings.TileSizePx), (int)(GameWorld.CameraLocation.Y + y * ExdSettings.TileSizePx), 
                        (int)(ExdSettings.TileSizePx), (int)(ExdSettings.TileSizePx)), Color.Green);
            batch.End();
        }
    }
}
