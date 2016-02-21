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
            AnyfinCanHappen = Database.GetCardFromId("LOE_026");
        }

        public static Card BluegillWarrior { get; }
        public static Card GrimscaleOracle { get; }
        public static Card MurlocWarleader { get; }
        public static Card OldMurkEye { get; }
        public static Card AnyfinCanHappen { get; }
        public static bool IsMurloc(this Card card) => card.Race == "Murloc";
        public static bool IsChargeMurloc(this Card card) => card.Id == OldMurkEye.Id || card.Id == BluegillWarrior.Id;
        public static bool IsBluegill(this Card card) => card.Id == BluegillWarrior.Id;
        public static bool IsGrimscale(this Card card) => card.Id == GrimscaleOracle.Id;
        public static bool IsWarleader(this Card card) => card.Id == MurlocWarleader.Id;
        public static bool IsOldMurkEye(this Card card) => card.Id == OldMurkEye.Id;
    }
}