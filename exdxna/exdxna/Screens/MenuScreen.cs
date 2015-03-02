using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using exdxna.GameStateManagement;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;

namespace exdxna.Screens
{
    abstract class MenuScreen : GameScreen
    {
        protected List<MenuEntry> MenuEntries = new List<MenuEntry>();
        int SelectedEntry = 0;
        string MenuTitle;

        InputAction MenuUp, MenuDown, MenuSelect, MenuCancel;

        public MenuScreen(string title)
        {
            MenuTitle = title;

            TransitionOnTime = TimeSpan.FromSeconds(.5);
            TransitionOffTime = TimeSpan.FromSeconds(.5);

            MenuUp = new InputAction(null, new[] { Keys.Up }, true);
            MenuDown = new InputAction(null, new[] { Keys.Down }, true);
            MenuSelect = new InputAction(null, new[] { Keys.Enter, Keys.Space }, true);
            MenuCancel = new InputAction(null, new[] { Keys.Escape }, true);
        }

        public override void HandleInput(GameTime gametime, InputState input)
        {
            PlayerIndex playerindex;

            if (MenuUp.Evaluate(input, ControllingPlayer, out playerindex))
                if (--SelectedEntry < 0)
                    SelectedEntry = MenuEntries.Count - 1;

            if (MenuDown.Evaluate(input, ControllingPlayer, out playerindex))
                if (++SelectedEntry >= MenuEntries.Count)
                    SelectedEntry = 0;

            if (MenuSelect.Evaluate(input, ControllingPlayer, out playerindex))
                OnSelectEntry(SelectedEntry, playerindex);

            if (MenuCancel.Evaluate(input, ControllingPlayer, out playerindex))
                OnCancel(playerindex);
        }

        protected virtual void OnCancel(PlayerIndex playerindex)
        {
            ExitScreen();
        }

        protected void OnCancel(object sender, PlayerIndexEventArgs e)
        {
            OnCancel(e.PlayerIndex);
        }

        protected virtual void OnSelectEntry(int selectedentry, PlayerIndex playerindex)
        {
            MenuEntries[selectedentry].OnSelectEntry(playerindex);
        }

        protected virtual void UpdateMenuEntryLocations()
        {
            // Make the menu slide into place during transitions, using a
            // power curve to make things look more interesting (this makes
            // the movement slow down as it nears the end).
            var transitionoffset = Math.Pow(TransitionPosition, 2);

            // start at Y = 175; each X value is generated per entry
            Vector2 position = new Vector2(0f, 175f);

            // update each menu entry's location in turn
            foreach (var entry in MenuEntries)
            {
                // each entry is to be centered horizontally
                position.X = (float)(ScreenManager.GraphicsDevice.Viewport.Width / 2 - entry.GetWidth(this) / 2);

                if (ScreenState == ScreenState.TransitionOn)
                    position.X -= (float)(transitionoffset * 256);
                else
                    position.X += (float)(transitionoffset * 512);

                // set the entry's position
                entry.Position = position;

                // move down for the next entry the size of this entry
                position.Y += entry.GetHeight(this);
            }
        }

        public override void Update(GameTime gametime, bool otherScreenHasFocus, bool coveredByOtherScreen)
        {
            base.Update(gametime, otherScreenHasFocus, coveredByOtherScreen);

            for (int i = 0; i < MenuEntries.Count; ++i)
            {
                var selected = IsActive && (i == SelectedEntry);
                MenuEntries[i].Update(this, selected, gametime);
            }
        }

        public override void Draw(GameTime gametime)
        {
            // make sure our entries are in the right place before we draw them
            UpdateMenuEntryLocations();

            var graphics = ScreenManager.GraphicsDevice;
            var spritebatch = ScreenManager.SpriteBatch;
            var font = ScreenManager.Font;

            spritebatch.Begin();

            // Draw each menu entry in turn.
            for (int i = 0; i < MenuEntries.Count; i++)
            {
                var menuEntry = MenuEntries[i];

                var selected = IsActive && (i == SelectedEntry);
                menuEntry.Draw(this, selected, gametime);
            }

            // Make the menu slide into place during transitions, using a
            // power curve to make things look more interesting (this makes
            // the movement slow down as it nears the end).
            var transitionoffset = Math.Pow(TransitionPosition, 2);

            // Draw the menu title centered on the screen
            var titleposition = new Vector2(graphics.Viewport.Width / 2, 80);
            var titleorigin = font.MeasureString(MenuTitle) / 2;
            var titlecolor = new Color(192, 192, 192) * TransitionAlpha;
            var titlescale = 1.25;

            titleposition.Y -= (float)(transitionoffset * 100);

            spritebatch.DrawString(font, MenuTitle, titleposition, titlecolor, 0,
                                   titleorigin, (float)titlescale, SpriteEffects.None, 0);

            spritebatch.End();
        }
    }
}
