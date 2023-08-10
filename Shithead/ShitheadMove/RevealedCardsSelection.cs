namespace Shithead.ShitheadMove
{
	internal sealed class RevealedCardsSelection : IShitheadMove
	{
		/// <summary>
		/// Card index in player deck
		/// </summary>
		public int CardIndex { get; set; }

		/// <summary>
		/// The revealed card index to send the card to
		/// </summary>
		public int TargetIndex { get; set; }
	}

	internal sealed class UnsetRevealedCard : IShitheadMove
	{
		/// <summary>
		/// Card index in player revealed deck
		/// </summary>
		public int CardIndex { get; set; }
	}

	internal sealed class AcceptSelectedRevealedCards : IShitheadMove
	{
	}

	internal sealed class ReselectRevealedCards : IShitheadMove
	{
	}
}
