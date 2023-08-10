namespace Shithead.ShitheadMove
{
	internal sealed class PlaceCard : IShitheadMove
	{
		/// <summary>
		/// The index of the card in the player's hand
		/// </summary>
		public int[] CardIndices { get; set; } = Array.Empty<int>();
	}

	internal sealed class PlaceJoker : IShitheadMove
	{
		/// <summary>
		/// The Id of the player that accepts the discard pile
		/// </summary>
		public int PlayerId { get; set; }
	}

	internal sealed class AcceptDiscardPile : IShitheadMove
	{
	}

	internal sealed class RevealUndercard : IShitheadMove
	{
		/// <summary>
		/// The index of the card in the player's hand
		/// </summary>
		public int CardIndex { get; set; }
	}
}
