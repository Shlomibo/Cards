namespace Shithead.ShitheadMove;

public sealed class RevealedCardSelection : IShitheadMove
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

public sealed class UnsetRevealedCard : IShitheadMove
{
	/// <summary>
	/// Card index in player revealed deck
	/// </summary>
	public int CardIndex { get; set; }
}

public sealed class AcceptSelectedRevealedCards : IShitheadMove
{
}

public sealed class ReselectRevealedCards : IShitheadMove
{
}
