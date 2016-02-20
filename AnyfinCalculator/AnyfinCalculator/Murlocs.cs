
using Hearthstone_Deck_Tracker.Hearthstone;

namespace AnyfinCalculator
{
    //MRGLGLG
    public static class Murlocs
    {
        public static Card BluegillWarrior { get; }
        public static Card GrimscaleOracle { get; }
        public static Card MurlocWarleader { get; }
        public static Card OldMurkEye { get; }
        public static Card AnyfinCanHappen { get; }
        static Murlocs()
        {
            BluegillWarrior = Database.GetCardFromId("CS2_173");
            GrimscaleOracle = Database.GetCardFromId("EX1_508");
            MurlocWarleader = Database.GetCardFromId("EX1_507");
            OldMurkEye = Database.GetCardFromId("EX1_062");
            AnyfinCanHappen = Database.GetCardFromId("LOE_026");
        }
        public static bool IsMurloc(Card card)
        {
            return card.Race == "Murloc";
        }
    }
}