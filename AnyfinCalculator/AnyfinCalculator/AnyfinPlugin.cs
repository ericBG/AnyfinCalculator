using System;
using System.Windows.Controls;
using Hearthstone_Deck_Tracker.Plugins;

namespace AnyfinCalculator
{
    public class AnyfinPlugin : IPlugin
    {
        public void OnLoad()
        {
            throw new NotImplementedException();
        }

        public void OnUnload()
        {
            throw new NotImplementedException();
        }

        public void OnButtonPress()
        {
            throw new NotImplementedException();
        }

        public void OnUpdate()
        {
            throw new NotImplementedException();
        }

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
