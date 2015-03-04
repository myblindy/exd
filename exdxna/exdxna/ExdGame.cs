using System;
using System.Collections.Generic;
using System.Linq;
using exdxna.GameStateManagement;
using exdxna.GameStateManagement.Screens;
using exdxna.Helpers;
using exdxna.Screens;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Audio;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.GamerServices;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using Microsoft.Xna.Framework.Media;

namespace exdxna
{
    /// <summary>
    /// This is the main type for your game
    /// </summary>
    public class ExdGame : Microsoft.Xna.Framework.Game
    {
        GraphicsDeviceManager Graphics;
        ScreenManager ScreenManager;
        ScreenFactory ScreenFactory;

        public ExdGame()
        {
            Graphics = new GraphicsDeviceManager(this);
            Content.RootDirectory = "Content";
            IsMouseVisible = true;

            ScreenFactory = new ScreenFactory();
            Services.AddService(typeof(IScreenFactory), ScreenFactory);

            ScreenManager = new ScreenManager(this);
            Components.Add(ScreenManager);

            AddInitialScreens();
        }

        private void AddInitialScreens()
        {
            ScreenManager.AddScreen(new BackgroundScreen(), null);
            ScreenManager.AddScreen(new MainMenuScreen(), null);
        }

        protected override void LoadContent()
        {
            // TODO: use this.Content to load your game content here
        }

        protected override void UnloadContent()
        {
            // TODO: Unload any non ContentManager content here
        }

        protected override void Draw(GameTime gameTime)
        {
            GraphicsDevice.Clear(Color.Black);

            base.Draw(gameTime);
        }
    }
}
