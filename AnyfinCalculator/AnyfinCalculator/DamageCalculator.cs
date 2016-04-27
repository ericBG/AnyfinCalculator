using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using HearthDb.Enums;
using Hearthstone_Deck_Tracker;
using Hearthstone_Deck_Tracker.Enums;
using Hearthstone_Deck_Tracker.Hearthstone;
using Hearthstone_Deck_Tracker.Hearthstone.Entities;
using Hearthstone_Deck_Tracker.Utility.Logging;

namespace AnyfinCalculator
{
    public class DamageCalculator
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
            List<Entity> board = Core.Game.Player.Board.ToList();
            List<Entity> opponent = Core.Game.Opponent.Board.ToList();
            int? min = null, max = null;
            foreach (IEnumerable<Card> combination in Combinations(murlocs, 7 - Core.Game.PlayerMinionCount))
            {
                int damage = CalculateDamageInternal(combination, board, opponent);
                if (damage > max || !max.HasValue) max = damage;
                if (damage < min || !min.HasValue) min = damage;
            }
            sw.Stop();
            Log.Debug($"Time to calculate the possibilities: {sw.Elapsed.ToString("ss\\:fff")}");
            return new Range<int> {Maximum = max.Value, Minimum = min.Value};
        }

        private static int CalculateDamageInternal(IEnumerable<Card> graveyard, IEnumerable<Entity> friendlyBoard,
            IEnumerable<Entity> opponentBoard)
        {
            List<Card> deadMurlocs = graveyard.ToList();
            List<Entity> aliveMurlocs = friendlyBoard.Where(c => c.Card.IsMurloc()).ToList();
            List<Entity> opponentMurlocs = opponentBoard.Where(c => c.Card.IsMurloc()).ToList();
            //compiles together into one big freaking list
            List<MurlocInfo> murlocs =
                deadMurlocs.Select(
                    c =>
                        new MurlocInfo
                        {
                            AreBuffsApplied = false,
                            Attack = c.Attack,
                            BoardState = MurlocInfo.State.Dead,
                            CanAttack = c.IsChargeMurloc(),
                            IsSilenced = false,
                            Murloc = c
                        })
                    .Concat(
                        aliveMurlocs.Select(
                            ent =>
                                new MurlocInfo
                                {
                                    AreBuffsApplied = true,
                                    Attack = ent.GetTag(GameTag.ATK),
                                    BoardState = MurlocInfo.State.OnBoard,
                                    CanAttack = CanAttack(ent),
                                    IsSilenced = IsSilenced(ent),
                                    Murloc = ent.Card
                                }))
                    .Concat(
                        opponentMurlocs.Select(
                            ent =>
                                new MurlocInfo
                                {
                                    AreBuffsApplied = false,
                                    Attack = ent.Card.Attack,
                                    BoardState = MurlocInfo.State.OnOpponentsBoard,
                                    CanAttack = false,
                                    IsSilenced = IsSilenced(ent),
                                    Murloc = ent.Card
                                })).ToList();
            int nonSilencedWarleaders =
                murlocs.Count(m => m.BoardState != MurlocInfo.State.Dead && m.Murloc.IsWarleader() && !m.IsSilenced);
            int nonSilencedGrimscales =
                murlocs.Count(m => m.BoardState != MurlocInfo.State.Dead && m.Murloc.IsGrimscale() && !m.IsSilenced);
            int murlocsToBeSummoned = murlocs.Count(m => m.BoardState == MurlocInfo.State.Dead);
            foreach (MurlocInfo murloc in murlocs.Where(t => t.AreBuffsApplied))
            {
                murloc.AreBuffsApplied = false;
                murloc.Attack -= nonSilencedGrimscales + (nonSilencedWarleaders*2);
                if (murloc.IsSilenced) continue;
                if (murloc.Murloc.IsGrimscale()) murloc.Attack += 1;
                if (murloc.Murloc.IsWarleader()) murloc.Attack += 2;
                if (murloc.Murloc.IsMurkEye()) murloc.Attack -= (murlocs.Count(m => m.BoardState != MurlocInfo.State.Dead) - 1);
            }
            nonSilencedWarleaders += murlocs.Count(m => m.BoardState == MurlocInfo.State.Dead && m.Murloc.IsWarleader());
            nonSilencedGrimscales += murlocs.Count(m => m.BoardState == MurlocInfo.State.Dead && m.Murloc.IsGrimscale());
            foreach (MurlocInfo murloc in murlocs)
            {
                murloc.AreBuffsApplied = true;
                murloc.Attack += nonSilencedGrimscales + (nonSilencedWarleaders*2);
                if (murloc.IsSilenced) continue;
                if (murloc.Murloc.IsWarleader()) murloc.Attack -= 2;
                if (murloc.Murloc.IsGrimscale()) murloc.Attack -= 1;
                if (murloc.Murloc.IsMurkEye()) murloc.Attack += (murlocs.Count - 1);
                if (murloc.Murloc.IsTidecaller()) murloc.Attack += murlocsToBeSummoned;
            }
            Log.Debug(murlocs.Aggregate("", (s, m) => s + $"{m.Murloc.Name}{(m.IsSilenced?" (Silenced)":"")}: {m.Attack} {(!m.CanAttack ? "(Can't Attack)" : "")}\n"));
            return murlocs.Sum(m => m.CanAttack ? m.Attack : 0);
        }

        private static bool IsSilenced(Entity entity) => entity.GetTag(GameTag.SILENCED) == 1;

        private static bool CanAttack(Entity entity)
        {
            if (entity.GetTag(GameTag.CANT_ATTACK) == 1 || entity.GetTag(GameTag.FROZEN) == 1)
                return false;
            if (entity.GetTag(GameTag.EXHAUSTED) == 1)
                //from reading the HDT source, it seems like internally Charge minions still have summoning sickness
                return entity.GetTag(GameTag.CHARGE) == 1 &&
                       entity.GetTag(GameTag.NUM_ATTACKS_THIS_TURN) < MaxAttacks(entity);
            return entity.GetTag(GameTag.NUM_ATTACKS_THIS_TURN) < MaxAttacks(entity);
        }

        private static int MaxAttacks(Entity entity)
        {
            // GVG_111t == V-07-TR-0N (MegaWindfury, 4x attack)
            if (entity.CardId == "GVG_111t") return 4;
            // if it has windfury it can attack twice, else it can only attack once
            return entity.GetTag(GameTag.WINDFURY) == 1 ? 2 : 1;
        }

        public static IEnumerable<IEnumerable<T>> Combinations<T>(IEnumerable<T> elements, int k)
        {
            return k == 0 ? new[] { new T[0] } :
              elements.SelectMany((e, i) =>
                Combinations(elements.Skip(i + 1), k - 1).Select(c => (new[] { e }).Concat(c)));
        }

    }
}