using Deck;
using Deck.Cards.FrenchSuited;
using System.Collections;
using System.Diagnostics.CodeAnalysis;

namespace Shithead.State
{
	public sealed partial class ShitheadState
	{
		public sealed class SharedState
		{
			private readonly ShitheadState state;

			public IReadOnlyList<SharedPlayerState> Players { get; }
			public int DeckSize => this.state.Deck.Count;
			public IReadonlyDeck<Card> DiscardPile { get; }
			public GameState GameState => this.state.GameState;
			public int CurrentTurnPlayer => this.state.turnsManager.Current;

			public SharedState(ShitheadState state)
			{
				this.state = state;
				this.Players = new PlayersView(this);
				this.DiscardPile = state.DiscardPile.AsReadonly();
			}

			private sealed class PlayersView : IReadOnlyList<SharedPlayerState>
			{
				private readonly SharedState sharedState;
				private readonly Lazy<SharedPlayerState[]> sharedPlayers;

				private SharedPlayerState[] Players => this.sharedPlayers.Value;

				public SharedPlayerState this[int index] => this.Players[index];

				public int Count => this.sharedState.state.players.Length;


				public PlayersView(SharedState sharedState)
				{
					this.sharedState = sharedState;
					this.sharedPlayers = new Lazy<SharedPlayerState[]>(() =>
					this.sharedState.state.players
						.Select(player => new SharedPlayerState(this.sharedState.state, player.Id))
						.ToArray());
				}

				public IEnumerator<SharedPlayerState> GetEnumerator() => (from player in this.Players
																		  select player).GetEnumerator();

				IEnumerator IEnumerable.GetEnumerator() => GetEnumerator();
			}
		}

		public sealed class SharedPlayerState
		{
			private readonly ShitheadState gameState;

			private PlayerState Player => this.gameState.players[this.Id];

			public bool Won => this.Player.Won;

			public IReadOnlyDictionary<int, Card> RevealedCards => this.Player.RevealedCards;

			public IReadOnlyDictionary<int, Card?> Undercards { get; }

			public int Id { get; }

			public int CardsCount => this.Player.Hand.Count;

			public SharedPlayerState(ShitheadState state, int id)
			{
				this.gameState = state;
				this.Id = id;
				this.Undercards = new UndercardsView(this);
			}

			private sealed class UndercardsView : IReadOnlyDictionary<int, Card?>
			{
				private readonly SharedPlayerState playerState;

				private Dictionary<int, CardFace<Card>> Undercards =>
					this.playerState.Player.Undercards;

				public Card? this[int key] => UndercardValue(this.Undercards[key]);

				public IEnumerable<int> Keys => this.Undercards.Keys;

				public IEnumerable<Card?> Values => this.Undercards.Values.Select(UndercardValue);

				public int Count => this.Undercards.Count;

				public UndercardsView(SharedPlayerState playerState)
				{
					this.playerState = playerState;
				}

				public bool ContainsKey(int key) => this.Undercards.ContainsKey(key);

				public IEnumerator<KeyValuePair<int, Card?>> GetEnumerator()
				{
					foreach (var kv in this.Undercards)
					{
						yield return new KeyValuePair<int, Card?>(kv.Key, UndercardValue(kv.Value));
					}
				}

				public bool TryGetValue(int key, [MaybeNullWhen(false)] out Card? value)
				{
					value = default;
					bool gotValue = false;

					if (this.Undercards.TryGetValue(key, out var card))
					{
						value = UndercardValue(card);
						gotValue = true;
					}

					return gotValue;
				}

				IEnumerator IEnumerable.GetEnumerator() => this.GetEnumerator();

				private static Card? UndercardValue(CardFace<Card> undercard) =>
					undercard.IsRevealed
						? undercard.Card
						: null;
			}
		}
	}
}
