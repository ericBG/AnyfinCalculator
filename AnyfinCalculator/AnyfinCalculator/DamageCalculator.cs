using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using Hearthstone_Deck_Tracker;
using Hearthstone_Deck_Tracker.Enums;
using Hearthstone_Deck_Tracker.Hearthstone;
using Hearthstone_Deck_Tracker.Utility.Logging;

namespace AnyfinCalculator
{
    internal class DamageCalculator
    {
        private readonly GraveyardHelper _helper = new GraveyardHelper(c => c.Race == "Murloc");

        public Range<int> CalculateDamageDealt()
        {
            if (_helper.TrackedMinions.Count() + Core.Game.PlayerMinionCount <= 7)
            {
                var damage = CalculateDamageInternal(_helper.TrackedMinions, Core.Game.Player.Board,
                    Core.Game.Opponent.Board);
                return new Range<int> {Maximum = damage, Minimum = damage};
            }
            Stopwatch sw = Stopwatch.StartNew();
            List<Card> murlocs = _helper.TrackedMinions.ToList();
            List<CardEntity> board = Core.Game.Player.Board.ToList();
            List<CardEntity> opponent = Core.Game.Opponent.Board.ToList();
            int? min = null, max = null;
            foreach (IEnumerable<Card> combination in Combinations(murlocs, 7 - Core.Game.PlayerMinionCount))
            {
                int damage = CalculateDamageInternal(combination, board, opponent);
                if (damage > max || !max.HasValue) max = damage;
                if (damage < min || !min.HasValue) min = damage;
            }
            sw.Stop();
            Log.Debug($"Time to calculate the possibilities: {sw.Elapsed.ToString("ss:ffffff")}");
            return new Range<int> {Maximum = max.Value, Minimum = min.Value};
        }

        private static int CalculateDamageInternal(IEnumerable<Card> graveyard, IList<CardEntity> friendlyBoard,
            IEnumerable<CardEntity> opponentBoard)
        {
            List<Card> deadMurlocs = graveyard.ToList();
            List<Card> aliveMurlocs = friendlyBoard.Select(c => c.Entity.Card).Where(c => c.IsMurloc()).ToList();
            List<CardEntity> possibleAliveAttackers =
                friendlyBoard.Where(ce => ce.Entity.Card.IsMurloc() && CanAttack(ce)).ToList();
            List<Card> possibleAliveAttackerCards = possibleAliveAttackers.Select(ce => ce.Entity.Card).ToList();
            List<Card> allFriendlyMurlocs = deadMurlocs.Concat(aliveMurlocs).ToList();
            List<Card> opponentMurlocs = opponentBoard.Select(c => c.Entity.Card).Where(Murlocs.IsMurloc).ToList();
            List<Card> allMurlocs = allFriendlyMurlocs.Concat(opponentMurlocs).ToList();
            // murlocs on board that can attack + charge murlocs that will be summoned = murlocs that can attack
            int chargeMurlocsToBeSummoned = deadMurlocs.Count(Murlocs.IsChargeMurloc);
            int oldMurlocsThatCanAttack = possibleAliveAttackerCards.Count;
            int totalAttackableMurlocs = chargeMurlocsToBeSummoned + oldMurlocsThatCanAttack;
            //sum up the attack of all murlocs that can attack currently on the board
            int damage = possibleAliveAttackers.Sum(card => card.Entity.GetTag(GAME_TAG.ATK));
            //that felt bad, deleting an hour's work
            //let's do this counting algorhithm another way
            //get base attack and then reapply effect on all
            //every warleader gives 2 to every alive murloc except itself

            /////////////////////////////////////////////////////////////////////////////////////////////////
            //NOTE: For the purposes of this algorithm, Murk-Eye's base attack is 1 and it counts itself!!!//
            /////////////////////////////////////////////////////////////////////////////////////////////////

            damage -= aliveMurlocs.Count(Murlocs.IsWarleader)*2*(oldMurlocsThatCanAttack - 1);
            //same with grimscale, except it gives 1
            damage -= aliveMurlocs.Count(Murlocs.IsGrimscale)*(oldMurlocsThatCanAttack - 1);
            //each murk eye needs 1 off for each murloc
            damage -= possibleAliveAttackerCards.Count(Murlocs.IsOldMurkEye)*allMurlocs.Count;
            //also take into account opponent's warleaders and grimscales
            damage -= opponentMurlocs.Count(Murlocs.IsWarleader)*2*oldMurlocsThatCanAttack;
            damage -= opponentMurlocs.Count(Murlocs.IsGrimscale)*oldMurlocsThatCanAttack;

            //REMEMBER: FOR THIS ALGO MURK-EYE'S BASE IS 1
            damage += deadMurlocs.Count(Murlocs.IsOldMurkEye);
            damage += deadMurlocs.Count(Murlocs.IsBluegill)*2;
            //warleader and grimscale buff ALL murlocs
            damage += allMurlocs.Count(Murlocs.IsWarleader)*2*totalAttackableMurlocs;
            //but a warleader can't buff itself!
            damage -= possibleAliveAttackerCards.Count(Murlocs.IsWarleader)*2;
            //same shit for grimscale, at 1x instead of 2x
            damage += allMurlocs.Count(Murlocs.IsGrimscale)*totalAttackableMurlocs;
            damage -= possibleAliveAttackerCards.Count(Murlocs.IsGrimscale);
            //and then let murk eye get buffed
            damage += (allFriendlyMurlocs.Count + opponentMurlocs.Count)*
                      allFriendlyMurlocs.Count(Murlocs.IsOldMurkEye);
            //edgecase of tidecallers, too
            damage += possibleAliveAttackerCards.Count(Murlocs.IsTidecaller)*
                      Math.Min(deadMurlocs.Count, 7 - friendlyBoard.Count());
            return damage;
        }

        private static bool CanAttack(CardEntity cardEntity)
        {
            if (cardEntity.Entity.GetTag(GAME_TAG.CANT_ATTACK) == 1 || cardEntity.Entity.GetTag(GAME_TAG.FROZEN) == 1)
                return false;
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

        public static IEnumerable<IEnumerable<T>> Combinations<T>(IEnumerable<T> elements, int k)
        {
            return k == 0 ? new[] { new T[0] } :
              elements.SelectMany((e, i) =>
                Combinations(elements.Skip(i + 1), k - 1).Select(c => (new[] { e }).Concat(c)));
        }

    }
}