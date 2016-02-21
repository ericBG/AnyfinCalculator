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
            if (deadMurlocs.Count + Core.Game.Player.Board.Count <= 7)
            {
                List<Card> aliveMurlocs = Core.Game.Player.Board.Select(c => c.Entity.Card).Where(c => c.IsMurloc()).ToList();
                List<CardEntity> attackableAliveMurlocs = Core.Game.Player.Board.Where(CanAttack).Where(ce => ce.Entity.Card.IsMurloc()).ToList();
                List<Card> attackableAliveMurlocCards = attackableAliveMurlocs.Select(ce => ce.Entity.Card).ToList();
                List<Card> allMurlocs = deadMurlocs.Concat(aliveMurlocs).ToList();
                // murlocs on board that can attack + charge murlocs that will be summoned = murlocs that can attack
                int chargeMurlocsToBeSummoned = deadMurlocs.Count(Murlocs.IsChargeMurloc);
                int oldMurlocsThatCanAttack = attackableAliveMurlocCards.Count;
                int totalAttackableMurlocs = chargeMurlocsToBeSummoned + oldMurlocsThatCanAttack;

                //sum up the attack of all murlocs that can attack currently on the board
                int damage = attackableAliveMurlocs.Sum(card => card.Entity.GetTag(GAME_TAG.ATK));
                //that felt bad, deleting an hour's work
                //let's do this counting algorhithm another way
                //get base attack and then reapply effect on all
                //every warleader gives 2 to every alive murloc except itself
                /////////////////////////////////////////////////////////////////////////////////////////////////
                //NOTE: For the purposes of this algorithm, Murk-Eye's base attack is 1 and it counts itself!!!//
                /////////////////////////////////////////////////////////////////////////////////////////////////
                damage -= aliveMurlocs.Count(Murlocs.IsWarleader)*2*(aliveMurlocs.Count - 1);
                //same with grimscale, except it gives 1
                damage -= aliveMurlocs.Count(Murlocs.IsGrimscale)*(aliveMurlocs.Count - 1);
                //each murk eye needs 1 off for each murloc
                damage -= attackableAliveMurlocCards.Count(Murlocs.IsOldMurkEye)*aliveMurlocs.Count;

                //REMEMBER: FOR THIS ALGO MURK-EYE'S BASE IS 1
                damage += deadMurlocs.Count(Murlocs.IsOldMurkEye);
                damage += deadMurlocs.Count(Murlocs.IsBluegill)*2;
                damage += allMurlocs.Count(Murlocs.IsWarleader)*2*totalAttackableMurlocs;
                //but a warleader can't buff itself!
                damage -= attackableAliveMurlocCards.Count(Murlocs.IsWarleader) * 2;
                //same shit for grimscale, at 1x instead of 2x
                damage += allMurlocs.Count(Murlocs.IsGrimscale)*totalAttackableMurlocs;
                damage -= attackableAliveMurlocCards.Count(Murlocs.IsGrimscale);
                //and then let murk eye get buffed
                damage += (allMurlocs.Count + Core.Game.Opponent.Board.Count(ce => ce.Entity.Card.IsMurloc()))*
                          allMurlocs.Count(Murlocs.IsOldMurkEye);
                //boom
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