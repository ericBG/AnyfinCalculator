using System;
using System.Collections.Generic;
using System.Linq;
using Hearthstone_Deck_Tracker.API;
using Hearthstone_Deck_Tracker.Hearthstone;
using Hearthstone_Deck_Tracker.Hearthstone.Entities;

namespace AnyfinCalculator
{
	public class GraveyardHelper
	{
		private readonly Predicate<Card> _shouldBeTracked;

		public GraveyardHelper(Predicate<Card> shouldBeTracked = null)
		{
			_shouldBeTracked = shouldBeTracked ?? (c => true);
		}

		public IEnumerable<Card> TrackedMinions
			=> (Core.Game.Player.Graveyard ?? Enumerable.Empty<Entity>()).Select(ce => ce.Card)
				.Concat((Core.Game.Opponent.Graveyard ?? Enumerable.Empty<Entity>()).Select(ce => ce.Card))
				.Where(thisShouldBeAPredicate => _shouldBeTracked(thisShouldBeAPredicate));
	}
}