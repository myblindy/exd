using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace exdxna.Screens
{
    class MenuEntry
    {
        public string Text { get; set; }
        public double SelectionFade { get; set; }
        public Vector2 Position { get; set; }

        public event EventHandler<PlayerIndexEventArgs> Selected;
        protected internal virtual void OnSelectEntry(PlayerIndex index)
        {
            if (Selected != null)
                Selected(this, new PlayerIndexEventArgs(index));
        }

        public MenuEntry(string text)
            : this(text, null)
        {
        }

        public MenuEntry(string text, EventHandler<PlayerIndexEventArgs> handler)
        {
            Text = text;

            if (handler != null)
                Selected += handler;
        }

        public virtual void Update(MenuScreen screen, bool isselected, GameTime gametime)
        {
            var fadespeed = gametime.ElapsedGameTime.TotalSeconds * 4;

            if (isselected)
                SelectionFade = Math.Min(SelectionFade + fadespeed, 1);
            else
                SelectionFade = Math.Max(SelectionFade - fadespeed, 0);
        }

        public virtual void Draw(MenuScreen screen, bool isselected, GameTime gametime)
        {
            var color = isselected ? Color.Yellow : Color.White;
            var time = gametime.TotalGameTime.TotalSeconds;

            var pulsate = Math.Sin(time * 6) + 1;
            var scale = 1 + pulsate * 0.05 * SelectionFade;
            color *= screen.TransitionAlpha;

            // draw the text
            var manager = screen.ScreenManager;
            var spritebatch = manager.SpriteBatch;
            var font = manager.Font;

            var origin = new Vector2(0, font.LineSpacing / 2);

            spritebatch.DrawString(font, Text, Position, color, 0, origin, (float)scale, SpriteEffects.None, 0);
        }

        public virtual int GetHeight(MenuScreen screen)
        {
            return screen.ScreenManager.Font.LineSpacing;
        }

        public virtual double GetWidth(MenuScreen screen)
        {
            return screen.ScreenManager.Font.MeasureString(Text).X;
        }
    }

    class PlayerIndexEventArgs : EventArgs
    {
        public PlayerIndexEventArgs(PlayerIndex playerIndex)
        {
            PlayerIndex = playerIndex;
        }

        public PlayerIndex PlayerIndex { get; private set; }
    }
}
