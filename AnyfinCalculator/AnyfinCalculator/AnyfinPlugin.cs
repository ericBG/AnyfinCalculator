using System;
using System.Windows.Controls;
using Hearthstone_Deck_Tracker;
using Hearthstone_Deck_Tracker.API;
using Hearthstone_Deck_Tracker.Hearthstone;
using Hearthstone_Deck_Tracker.Plugins;

namespace AnyfinCalculator
{
    public class AnyfinPlugin : IPlugin
    {
        public void OnLoad()
        {
            _calculator = new DamageCalculator();
            GameEvents.OnPlayerHandMouseOver.Add(DisplayDamage);
        }

        private void DisplayDamage(Card card)
        {
            if (card.Id != "LOE_026") return;
            Range<int> damageDealt = _calculator.CalculateDamageDealt();
            Logger.WriteLine(damageDealt.Minimum == -1 ? "???" : damageDealt.ToString());
        }

        public void OnUnload()
        {
        }

        public void OnButtonPress()
        {
        }

        public void OnUpdate()
        {
        }

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
