using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace exdxna.Screens
{
    class MainMenuScreen : MenuScreen
    {
        public MainMenuScreen()
            : base("Main Menu")
        {
            MenuEntries.Add(new MenuEntry("Play Game", PlayGameMenuEntrySelected));
            MenuEntries.Add(new MenuEntry("Options", OptionsMenuEntrySelected));
            MenuEntries.Add(new MenuEntry("Exit", OnCancel));
        }

        private void PlayGameMenuEntrySelected(object sender, PlayerIndexEventArgs e)
        {
            LoadingScreen.Load(ScreenManager, true, e.PlayerIndex, new GameplayScreen());
        }

        private void OptionsMenuEntrySelected(object sender, PlayerIndexEventArgs e)
        {
            // todo
        }

        protected override void OnCancel(Microsoft.Xna.Framework.PlayerIndex playerindex)
        {
            ScreenManager.Game.Exit();
        }
    }
}
