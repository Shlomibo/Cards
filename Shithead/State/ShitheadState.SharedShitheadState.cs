using Deck;
using Deck.Cards.FrenchSuited;
using Shithead.ShitheadMove;
using System.Collections;
using System.Diagnostics.CodeAnalysis;

namespace Shithead.State;

public sealed partial class ShitheadState
{
	public sealed class SharedShitheadState
	{
		private readonly ShitheadState state;

		public IReadOnlyList<SharedPlayerState> Players { get; }
		public IReadOnlyList<int> ActivePlayers => state.turnsManager.ActivePlayers;
		public int DeckSize => state.Deck.Count;
		public IReadonlyDeck<Card> DiscardPile { get; }
		public GameState GameState => state.GameState;
		public int CurrentTurnPlayer => state.turnsManager.Current;
		public (IShitheadMove move, int? playerId)? LastMove => state.lastMove;

		public SharedShitheadState(ShitheadState state)
		{
			this.state = state;
			Players = new PlayersView(this);
			DiscardPile = state.DiscardPile.AsReadonly();
		}

		private sealed class PlayersView : IReadOnlyList<SharedPlayerState>
		{
			private readonly SharedShitheadState sharedState;
			private readonly Lazy<SharedPlayerState[]> sharedPlayers;

			private SharedPlayerState[] Players => sharedPlayers.Value;

			public SharedPlayerState this[int index] => Players[index];

			public int Count => sharedState.state.players.Length;

			public PlayersView(SharedShitheadState sharedState)
			{
				this.sharedState = sharedState;
				sharedPlayers = new Lazy<SharedPlayerState[]>(() =>
				this.sharedState.state.players
					.Select(player => new SharedPlayerState(this.sharedState.state, player.Id))
					.ToArray());
			}

			public IEnumerator<SharedPlayerState> GetEnumerator() => (from player in Players
																	  select player).GetEnumerator();

			IEnumerator IEnumerable.GetEnumerator() => GetEnumerator();
		}
	}

	public sealed class SharedPlayerState
	{
		private readonly ShitheadState gameState;

		private PlayerState Player => gameState.players[Id];

		public bool Won => Player.Won;

		public IReadOnlyDictionary<int, Card> RevealedCards => Player.RevealedCards;

		public IReadOnlyDictionary<int, Card?> Undercards { get; }

		public int Id { get; }

		public int CardsCount => Player.Hand.Count;
		public bool RevealedCardsAccepted => Player.RevealedCardsAccepted;

		public SharedPlayerState(ShitheadState state, int id)
		{
			gameState = state;
			Id = id;
			Undercards = new UndercardsView(this);
		}

		private sealed class UndercardsView : IReadOnlyDictionary<int, Card?>
		{
			private readonly SharedPlayerState playerState;

			private Dictionary<int, CardFace<Card>> Undercards =>
				playerState.Player.Undercards;

			public Card? this[int key] => UndercardValue(Undercards[key]);

			public IEnumerable<int> Keys => Undercards.Keys;

			public IEnumerable<Card?> Values => Undercards.Values.Select(UndercardValue);

			public int Count => Undercards.Count;

			public UndercardsView(SharedPlayerState playerState)
			{
				this.playerState = playerState;
			}

			public bool ContainsKey(int key) => Undercards.ContainsKey(key);

			public IEnumerator<KeyValuePair<int, Card?>> GetEnumerator()
			{
				foreach (var kv in Undercards)
				{
					yield return new KeyValuePair<int, Card?>(kv.Key, UndercardValue(kv.Value));
				}
			}

			public bool TryGetValue(int key, [MaybeNullWhen(false)] out Card? value)
			{
				value = default;
				bool gotValue = false;

				if (Undercards.TryGetValue(key, out var card))
				{
					value = UndercardValue(card);
					gotValue = true;
				}

				return gotValue;
			}

			IEnumerator IEnumerable.GetEnumerator() => GetEnumerator();

			private static Card? UndercardValue(CardFace<Card> undercard) =>
				undercard.IsRevealed
					? undercard.Card
					: null;
		}
	}
}
