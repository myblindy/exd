using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using exdxna.GameStateManagement;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.Graphics;

namespace exdxna.Screens
{
    /// <summary>
    /// The background screen sits behind all the other menu screens.
    /// It draws a background image that remains fixed in place regardless
    /// of whatever transitions the screens on top of it may be doing.
    /// </summary>
    class BackgroundScreen : GameScreen
    {
        ContentManager Content;
        //Texture2D BackgroundTexture;

        public BackgroundScreen()
        {
            TransitionOnTime = TimeSpan.FromSeconds(0.5);
            TransitionOffTime = TimeSpan.FromSeconds(0.5);
        }

        /// <summary>
        /// Loads graphics content for this screen. The background texture is quite
        /// big, so we use our own local ContentManager to load it. This allows us
        /// to unload before going from the menus into the game itself, wheras if we
        /// used the shared ContentManager provided by the Game class, the content
        /// would remain loaded forever.
        /// </summary>
        public override void Activate(bool instancepreserved)
        {
            if (!instancepreserved)
            {
                if (Content == null)
                    Content = new ContentManager(ScreenManager.Game.Services, "Content");

                //BackgroundTexture = Content.Load<Texture2D>("background");
            }
        }

        public override void Unload()
        {
            Content.Unload();
        }

        /// <summary>
        /// Updates the background screen. Unlike most screens, this should not
        /// transition off even if it has been covered by another screen: it is
        /// supposed to be covered, after all! This overload forces the
        /// coveredByOtherScreen parameter to false in order to stop the base
        /// Update method wanting to transition off.
        /// </summary>
        public override void Update(GameTime gametime, bool otherscreenhasfocus, bool coveredbyotherscreen)
        {
            base.Update(gametime, otherscreenhasfocus, false);
        }

        public override void Draw(GameTime gameTime)
        {
            //var spritebatch = ScreenManager.SpriteBatch;
            //var viewport = ScreenManager.GraphicsDevice.Viewport;
            //var fullscreen = new Rectangle(0, 0, viewport.Width, viewport.Height);

            //spritebatch.Begin();
            //spritebatch.Draw(BackgroundTexture, fullscreen, new Color(TransitionAlpha, TransitionAlpha, TransitionAlpha));
            //spritebatch.End();
        }
    }
}
