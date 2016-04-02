using System;
using System.Windows;
using System.Windows.Controls;
using Hearthstone_Deck_Tracker;
using Hearthstone_Deck_Tracker.API;
using Hearthstone_Deck_Tracker.Hearthstone;
using Hearthstone_Deck_Tracker.Plugins;
using Hearthstone_Deck_Tracker.Utility.Logging;
using MahApps.Metro.Controls.Dialogs;
using APICore = Hearthstone_Deck_Tracker.API.Core;

namespace AnyfinCalculator
{
    public class AnyfinPlugin : IPlugin
    {
        private HearthstoneTextBlock _displayBlock;
        private DamageCalculator _calculator;

        public void OnLoad()
        {
            _displayBlock = new HearthstoneTextBlock {FontSize = 36, Visibility = Visibility.Collapsed};
            _calculator = new DamageCalculator();
            GameEvents.OnPlayerHandMouseOver.Add(OnMouseOver);
            GameEvents.OnMouseOverOff.Add(OnMouseOff);
        }

        private void OnMouseOff()
        {
            _displayBlock.Visibility = Visibility.Collapsed;
            APICore.OverlayCanvas.Children.Remove(_displayBlock);
        }

        private void PlaceTextboxWithText(string text)
        {
            _displayBlock.Text = text;
            _displayBlock.Visibility = Visibility.Visible;
            if (!APICore.OverlayCanvas.Children.Contains(_displayBlock))
                APICore.OverlayCanvas.Children.Add(_displayBlock);
            Log.Debug($"Textbox has been placed to display:\n '{text}'.");
        }

        private void OnMouseOver(Card card)
        {
            if (!card.IsAnyfin()) return;
            Log.Debug("Anyfin hover detected");
            Range<int> damageDealt = _calculator.CalculateDamageDealt();
            string friendlyText = damageDealt.Minimum == damageDealt.Maximum ? "" : "between ";
            PlaceTextboxWithText($"Anyfin can deal {friendlyText}{damageDealt}");
        }

        public async void OnButtonPress() => await APICore.MainWindow.ShowMessageAsync("Warning", "There is currently no options for this plugin.");

        public void OnUnload() { }
        public void OnUpdate() { }

        public string Name => "Anyfin Can Happen Calculator";

        public string Description
            => "Anyfin Can Happen Calculator is a plugin for Hearthstone Deck Tracker which allows you to quickly and easily figure out the damage "
                + "(or damage range) that playing Anyfin Can Happen will have on your current board. \n For any questions or issues look at github.com/ericBG/AnyfinCalculator";

        public string ButtonText => "Options";
        public string Author => "ericBG";
        public Version Version => new Version(1, 0, 3);
        public MenuItem MenuItem => null;
    }
}