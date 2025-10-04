using static Shithead.State.ShitheadState;

namespace ShitheadServer.Server.DTO.State;

public sealed class PlayerState
{
	private readonly ShitheadPlayerState state;

	public string GameState => state.GameState.ToString();
	public int PlayerId => state.PlayerId;
	public object[] Hand { get; }
	public IReadOnlyDictionary<int, object> RevealedCards { get; }
	public IReadOnlyDictionary<int, object?> Undercards { get; }
	public bool Won => state.Won;
	public bool RevealedCardsAccepted => state.RevealedCardsAccepted;

	public PlayerState(ShitheadPlayerState state)
	{
		this.state = state;

		Hand = state.Hand
			.Select(card => card.ToJsonObject())
			.ToArray();
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
