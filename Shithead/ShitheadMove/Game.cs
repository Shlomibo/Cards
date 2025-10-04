namespace Shithead.ShitheadMove;

public sealed class PlaceCard : IShitheadMove
{
	/// <summary>
	/// The index of the card in the player's hand
	/// </summary>
	public int[] CardIndices { get; set; } = Array.Empty<int>();
}

public sealed class PlaceJoker : IShitheadMove
{
	/// <summary>
	/// The Id of the player that accepts the discard pile
	/// </summary>
	public int PlayerId { get; set; }
}

public sealed class AcceptDiscardPile : IShitheadMove
{
}

public sealed class RevealUndercard : IShitheadMove
{
	/// <summary>
	/// The index of the card in the player's hand
	/// </summary>
	public int CardIndex { get; set; }
}

public sealed class TakeUndercards : IShitheadMove
{
	/// <summary>
	/// The index of the card in the player's revealed or undercards lists
	/// </summary>
	public int[] CardIndices { get; set; } = Array.Empty<int>();
}

public sealed class LeaveGame : IShitheadMove
{
	public int PlayerId { get; set; }
}
