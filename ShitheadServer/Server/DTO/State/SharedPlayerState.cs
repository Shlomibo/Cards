using Shithead.State;

namespace ShitheadServer.Server.DTO.State;

public sealed class SharedPlayerState
{
	private readonly ShitheadState.SharedPlayerState state;

	public int Id => state.Id;
	public bool Won => state.Won;
	public int CardsCount => state.CardsCount;
	public bool RevealedCardsAccepted => state.RevealedCardsAccepted;
	public IReadOnlyDictionary<int, object> RevealedCards { get; }
	public IReadOnlyDictionary<int, object?> Undercards { get; }

	public SharedPlayerState(ShitheadState.SharedPlayerState state)
	{
		this.state = state;

		RevealedCards = new Dictionary<int, object>(
			state.RevealedCards
				.Select(kv => new KeyValuePair<int, object>(
					kv.Key,
					kv.Value.ToJsonObject())));
		Undercards = new Dictionary<int, object?>(
			state.Undercards
				.Select(kv => new KeyValuePair<int, object?>(
					kv.Key,
					kv.Value?.ToJsonObject())));
	}
}
