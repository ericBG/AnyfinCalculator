using System.CodeDom;
using System.Collections.Generic;
using System.Linq;
using Hearthstone_Deck_Tracker;
using Hearthstone_Deck_Tracker.Enums;
using Hearthstone_Deck_Tracker.Hearthstone;

namespace AnyfinCalculator
{
    internal class DamageCalculator
    {
        private readonly GraveyardHelper _helper = new GraveyardHelper(c => c.Race == "Murloc");
        public Range<int> CalculateDamageDealt()
        {
            List<Card> deadMurlocs = _helper.TrackedMinions.ToList();
            List<Card> aliveMurlocs = Core.Game.Player.Board.Select(c => c.Entity.Card).Where(c => c.IsMurloc()).ToList();
            List<Card> allMurlocs = deadMurlocs.Concat(aliveMurlocs).ToList();
            if (deadMurlocs.Count + Core.Game.Player.Board.Count <= 7)
            {
                // murlocs on board that can attack + charge murlocs that will be summoned = murlocs that can attack
                int chargeMurlocsToBeSummoned = deadMurlocs.Count(Murlocs.IsChargeMurloc);
                int oldMurlocsThatCanAttack = Core.Game.Player.Board.Where(c => c.Entity.Card.IsMurloc()).Count(CanAttack);

                //sum up the attack of all murlocs currently on the board
                int damage = Core.Game.Player.Board.Where(c => c.Entity.Card.IsMurloc()).Sum(card => card.Entity.GetTag(GAME_TAG.ATK));
                //add attack for each new charge murloc
                //murk eye attack is set to 1 because I'm making it count itself in the damage increase per murloc
                damage += deadMurlocs.Count(Murlocs.IsOldMurkEye) + (2*deadMurlocs.Count(Murlocs.IsBluegill));
                //for each murk eye, damage is added according to the total amount of murlocs, post summon
                damage += (allMurlocs.Count(Murlocs.IsOldMurkEye))*
                          (allMurlocs.Count + Core.Game.Opponent.Board.Count(c => c.Entity.Card.IsMurloc()));
                //apply the warleader effect on the dead murlocs
                damage += chargeMurlocsToBeSummoned*2*allMurlocs.Count(Murlocs.IsWarleader);
                //and apply the new warleader effect on the attacking murlocs
                damage += oldMurlocsThatCanAttack*2*deadMurlocs.Count(Murlocs.IsWarleader);
                //do the same shit for grimscale with a x1 multiplier
                damage += chargeMurlocsToBeSummoned * allMurlocs.Count(Murlocs.IsGrimscale);
                damage += oldMurlocsThatCanAttack * deadMurlocs.Count(Murlocs.IsGrimscale);
                //and that should be it!

                return new Range<int> {Maximum = damage, Minimum = damage};
            }
            else
            {
                //fuck
                return new Range<int> {Maximum = -1, Minimum = -1};
            }
        }

        private static bool CanAttack(CardEntity cardEntity)
        {
            if (cardEntity.Entity.GetTag(GAME_TAG.CANT_ATTACK) == 1 || cardEntity.Entity.GetTag(GAME_TAG.FROZEN) == 1) return false;
            if (cardEntity.Entity.GetTag(GAME_TAG.EXHAUSTED) == 1)
                //from reading the HDT source, it seems like internally Charge minions still have summoning sickness
                return cardEntity.Entity.GetTag(GAME_TAG.CHARGE) == 1 &&
                       cardEntity.Entity.GetTag(GAME_TAG.NUM_ATTACKS_THIS_TURN) < MaxAttacks(cardEntity);
            return true;
        }

        private static int MaxAttacks(CardEntity cardEntity)
        {
            // GVG_111t == V-07-TR-0N (MegaWindfury, 4x attack)
            if (cardEntity.CardId == "GVG_111t") return 4;
            // if it has windfury it can attack twice, else it can only attack once
            return cardEntity.Entity.GetTag(GAME_TAG.WINDFURY) == 1 ? 2 : 1;
        }
    }
}