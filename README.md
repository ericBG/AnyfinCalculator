# AnyfinCalculator
A calculator for Anyfin Can Happen, inbuilt to [Hearthstone Deck Tracker](https://github.com/Epix37/Hearthstone-Deck-Tracker/). ***NOTE:*** This currently only works with Hearthstone Deck Tracker version 0.13.12+ (more specifically commit [ed3aa02](https://github.com/Epix37/Hearthstone-Deck-Tracker/commit/ed3aa025a59faee1f46e3f9e6bc14a3229e09363) and later), due to a new API method being used in it, so currently to use it you'll have to compile it by hand.

# So what exactly does this do?
Not much, yet. I'm still writing it. The idea is though, that using Hearthstone Deck Tracker's abilities, whenever the card "Anyfin Can Happen" is hovered over in the player's hand, it will display a quick overview of how much damage it will/could inflict onto the opponent (just counting murloc damage). This would include murlocs on the board, but not other minions.

#Sweet, that sounds awesome. How can I use this?
Wherever you have Hearthstone Deck Tracker installed, drop the `AnyfinCalculator.dll` (available in the releases folder) into the 'Plugins' folder. Then run Hearthstone Deck Tracker and enable this plugin in the plugins menu, and ta-da! *If the plugin does not show up in that list, right click the '[PLUGINNAME].dll', go into Properties and click "Unblock" at the bottom.*
