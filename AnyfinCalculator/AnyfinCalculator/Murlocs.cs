using System.Runtime.CompilerServices;
using Hearthstone_Deck_Tracker.Hearthstone;

namespace AnyfinCalculator
{
    //MRGLGLG
    public static class Murlocs
    {
        static Murlocs()
        {
            BluegillWarrior = Database.GetCardFromId("CS2_173");
            GrimscaleOracle = Database.GetCardFromId("EX1_508");
            MurlocWarleader = Database.GetCardFromId("EX1_507");
            OldMurkEye = Database.GetCardFromId("EX1_062");
            MurlocTidecaller = Database.GetCardFromId("EX1_509");
            AnyfinCanHappen = Database.GetCardFromId("LOE_026");
        }

        public static Card BluegillWarrior { get; }
        public static Card GrimscaleOracle { get; }
        public static Card MurlocWarleader { get; }
        public static Card OldMurkEye { get; }
        public static Card MurlocTidecaller { get; }
        public static Card AnyfinCanHappen { get; }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static bool IsMurloc(this Card card) => card.Race == "Murloc";

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static bool IsChargeMurloc(this Card card) => card.Id == OldMurkEye.Id || card.Id == BluegillWarrior.Id;

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static bool IsBluegill(this Card card) => card.Id == BluegillWarrior.Id;

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static bool IsGrimscale(this Card card) => card.Id == GrimscaleOracle.Id;

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static bool IsWarleader(this Card card) => card.Id == MurlocWarleader.Id;

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static bool IsOldMurkEye(this Card card) => card.Id == OldMurkEye.Id;

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static bool IsTidecaller(this Card card) => card.Id == MurlocTidecaller.Id;

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static bool IsAnyfin(this Card card) => card.Id == AnyfinCanHappen.Id;
    }
}