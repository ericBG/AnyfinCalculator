using Hearthstone_Deck_Tracker.Hearthstone;

namespace AnyfinCalculator
{
	public class MurlocInfo
	{
		public enum State
		{
			Dead,
			OnBoard,
			OnOpponentsBoard
		}

		public State BoardState { get; set; }
		public bool CanAttack { get; set; }
		public bool IsSilenced { get; set; }
		public bool AreBuffsApplied { get; set; }
		public int Attack { get; set; }
		public Card Murloc { get; set; }
	}
}