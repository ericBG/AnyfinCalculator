using System;
using System.Diagnostics;
using System.Windows;
using System.Windows.Controls;
using Hearthstone_Deck_Tracker;
using Hearthstone_Deck_Tracker.API;
using Hearthstone_Deck_Tracker.Hearthstone;
using Hearthstone_Deck_Tracker.Plugins;
using Hearthstone_Deck_Tracker.Utility.Logging;
using APICore = Hearthstone_Deck_Tracker.API.Core;

namespace AnyfinCalculator
{
    public class AnyfinPlugin : IPlugin
    {
        private HearthstoneTextBlock _displayBlock;
        private Point _centreOfCanvas;
        public void OnLoad()
        {
            _calculator = new DamageCalculator();
            _displayBlock = new HearthstoneTextBlock {FontSize = 18, Visibility = Visibility.Collapsed};
            GameEvents.OnPlayerHandMouseOver.Add(OnMouseOver);
            GameEvents.OnMouseOverOff.Add(OnMouseOff);
            APICore.OverlayCanvas.SizeChanged += (sender, args) => RecalculateCentre();
        }

        private void OnMouseOff()
        {
            APICore.OverlayCanvas.Children.Remove(_displayBlock);
            _displayBlock.Visibility = Visibility.Collapsed;
        }

        private void RecalculateCentre() => _centreOfCanvas = new Point(APICore.OverlayCanvas.Width/2, APICore.OverlayCanvas.Width/2);

        private void PlaceTextboxWithText(string text)
        {
            _displayBlock.Text = text;
            Point size = new Point(_displayBlock.ActualWidth, _displayBlock.ActualHeight);
            Canvas.SetTop(_displayBlock, _centreOfCanvas.X - (size.X/2));
            Canvas.SetLeft(_displayBlock, _centreOfCanvas.Y - (size.Y / 2));
            _displayBlock.Visibility = Visibility.Visible;
            if (!APICore.OverlayCanvas.Children.Contains(_displayBlock))
                APICore.OverlayCanvas.Children.Add(_displayBlock);
            Log.Debug("Textbox has been 'set up'");
        }

        private void OnMouseOver(Card card)
        {
            if (!card.IsAnyfin()) return;
            Log.Debug("Anyfin hover detected");
            Range<int> damageDealt = _calculator.CalculateDamageDealt();
            PlaceTextboxWithText(damageDealt.Minimum == -1 ? "???" : damageDealt.ToString());
        }

        public void OnUnload() { }
        public void OnButtonPress() { }
        public void OnUpdate() { }

        private DamageCalculator _calculator;

        public string Name => "Anyfin Can Happen Calculator 1.0.0";

        public string Description
            => "Anyfin Can Happen Calculator is a plugin for Hearthstone Deck Tracker which allows you to quickly and easily figure out the damage "
                + "(or damage range) that playing Anyfin Can Happen will have on your current board. \n For any questions or issues look at github.com/ericBG/AnyfinCalculator";

        public string ButtonText => "";
        public string Author => "ericBG";
        public Version Version => new Version(1, 0, 0);
        public MenuItem MenuItem => null;
    }
}
