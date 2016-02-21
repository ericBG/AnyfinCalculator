using System;
using System.Collections.Generic;
using System.Linq;
using Hearthstone_Deck_Tracker.Hearthstone;
using Hearthstone_Deck_Tracker.API;

namespace AnyfinCalculator
{
    public class GraveyardHelper
    {
        private readonly Predicate<Card> _shouldBeTracked;
        private List<Card> _checkedMinions = new List<Card>();

        public GraveyardHelper(Predicate<Card> shouldBeTracked = null)
        {
            _shouldBeTracked = shouldBeTracked;
        }

        public IEnumerable<Card> TrackedMinions
            => (Core.Game.Player.Graveyard ?? Enumerable.Empty<CardEntity>()).Select(ce => ce.Entity.Card)
                .Concat((Core.Game.Opponent.Graveyard ?? Enumerable.Empty<CardEntity>()).Select(ce => ce.Entity.Card))
                .Where(thisShouldBeAPredicate => _shouldBeTracked(thisShouldBeAPredicate));
    }
}