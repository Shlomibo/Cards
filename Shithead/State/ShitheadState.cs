using Deck.Cards.FrenchSuited;
using GameEngine;
using Shithead.ShitheadMove;
using TurnsManagement;

namespace Shithead.State
{
	public sealed partial class ShitheadState :
		IState<ShitheadState, ShitheadState.Shared, ShitheadState.Player, IShitheadMove>
	{
		private const int MIN_PLAYERS_COUNT = 3;
		private const int MIN_HAND_CARDS = 3;
		#region Fields

		private readonly PlayerState[] players;
		private readonly TurnsManager turnsManager;
		private static readonly CardComparer cardComparer = new();
		#endregion

		#region Events

		public event EventHandler? Updated;
		#endregion

		#region Properties

		public int PlayersCount { get; private set; }

		ShitheadState IState<ShitheadState, Shared, Player, IShitheadMove>.GameState => this;
		public GameState GameState { get; private set; } = GameState.Init;

		public Shared SharedState { get; }
		public CardsDeck Deck { get; } = CardsDeck.FullDeck();
		public CardsDeck DiscardPile { get; } = new();
		#endregion

		#region Constructors

		public ShitheadState(int playersCount)
		{
			if (playersCount < MIN_PLAYERS_COUNT)
			{
				throw new ArgumentException($"The minimum players count is {MIN_PLAYERS_COUNT}", nameof(playersCount));
			}

			this.PlayersCount = playersCount;

			this.Deck.Shuffle();
			this.SharedState = new Shared(this);
			this.turnsManager = new TurnsManager(playersCount);
			this.players = Enumerable
				.Range(0, playersCount)
				.Select(id => new PlayerState(
					Enumerable.Range(0, PlayerState.UNDERCARDS_COUNT)
					.Select(_ => this.Deck.Pop())
					.ToArray(),
					id
				))
				.ToArray();
		}
		#endregion

		#region Methods

		public Player GetPlayerState(int playerId) =>
			new(playerId, this);

		public bool IsGameOver() =>
			this.GameState == GameState.GameOver;

		public bool IsValidMove(IShitheadMove move, int? player = null) =>
			GetMove(move, player) is not null;

		public void PlayMove(IShitheadMove move, int? player = null)
		{
			var moveAction = GetMove(move, player);
			moveAction?.Invoke();
		}

		private Action? GetMove(IShitheadMove move, int? playerId = null)
		{
			if (playerId is null)
			{
				return null;
			}

			var player = this.players[playerId.Value];

			return this.GameState switch
			{
				GameState.Init => move switch
				{
					RevealedCardsSelection
					{
						CardIndex: var index,
						TargetIndex: var target,
					} when player.CanSetRevealedCard(index) => () =>
					{
						var card = player.Hand[index];
						player.Hand.RemoveAt(index);

						player.RevealedCards.Add(target, card);
					}
					,
					UnsetRevealedCard
					{
						CardIndex: var index,
					} when player.CanUnsetRevealedCard(index) => () =>
					{
						var card = player.RevealedCards[index];
						player.RevealedCards.Remove(index);

						player.Hand.Add(card);
					}
					,
					AcceptSelectedRevealedCards when player.CanAcceptSelectedRevealedCards() => () =>
					{
						player.RevealedCardsAccepted = true;

						if (this.players.All(player => player.RevealedCardsAccepted))
						{
							this.GameState = GameState.GameOn;
						}
					}
					,
					ReselectRevealedCards when player.CanReselectRevealedCards() => () =>
					{
						player.RevealedCardsAccepted = false;
					}
					,
					_ => null,
				},

				GameState.GameOn => move switch
				{
					PlaceJoker
					{
						PlayerId: var targetPlayerId,
					} when player.CanPlaceJoker() &&
						this.turnsManager.ActivePlayers.Contains(targetPlayerId) => () =>
						{
							player.RemoveJoker();
							var targetPlayer = this.players[targetPlayerId];

							targetPlayer.Hand.Push(this.DiscardPile);
							this.DiscardPile.Clear();

							HandlePlayerWin(player);

							this.turnsManager.Current = targetPlayerId;
							this.turnsManager.MoveNext();
						}
					,

					PlaceCard
					{
						CardIndices: var indices,
					} when this.turnsManager.Current == playerId &&
						player.CanPlaceCard(indices) &&
						CanPlaceCard(player.GetCard(indices.First())) => () =>
						{
							var value = PlayHand(player, indices);
							HandlePlayerWin(player);

							if (ShouldDiscardPile(value))
							{
								this.DiscardPile.Clear();
							}
							// If the player won, it was removed and the turn belongs to the next player
							else if (!player.Won)
							{
								this.turnsManager.MoveNext();
							}
						}
					,
					// When Player tries to add cards of the same value they got from deck, after finishing
					// their turn
					PlaceCard { CardIndices: var indices } when this.turnsManager.Previous == playerId &&
						player.CanPlaceCard(indices) &&
						player.GetCard(indices.First()).Value == TopCard()?.Value => () =>
						{
							var value = PlayHand(player, indices);
							HandlePlayerWin(player);

							if (ShouldDiscardPile(value))
							{
								this.DiscardPile.Clear();
								this.turnsManager.Current = playerId.Value;
							}
						}
					,

					RevealUndercard
					{
						CardIndex: var cardIndex,
					} when this.turnsManager.Current == playerId &&
						player.CanRevealUndercard(cardIndex) => () =>
						{
							player.Undercards[cardIndex].IsRevealed = true;
						}
					,

					_ => null
				},

				GameState.GameOver => null,

				_ => throw new Exception("Invalid game state"),
			}; ;
		}

		private void HandlePlayerWin(PlayerState player)
		{
			if (player.Won)
			{
				this.turnsManager.RemovePlayer(player.Id);

				if (this.turnsManager.ActivePlayers.Count == 1)
				{
					this.GameState = GameState.GameOver;
				}
			}
		}

		private bool ShouldDiscardPile(Value cardValue)
		{
			var topFour = this.DiscardPile.Take(Enum.GetNames(typeof(Suit)).Length);
			return cardValue == Value.Ten || topFour.All(discard => discard.Value == cardValue);
		}

		private Value PlayHand(PlayerState player, int[] indices)
		{
			// TODO: change `player.Hand[i]` to a method on `PlayerState` that get the correct card
			var cards = indices.Select(i => player.GetCard(i)).ToArray();

			foreach (var i in indices.OrderByDescending(i => i))
			{
				player.RemoveCard(i);
			}

			this.DiscardPile.Push(cards);

			var value = cards.First().Value;
			ReplenishPlayerHand(player);

			return value;
		}

		private void ReplenishPlayerHand(PlayerState player)
		{
			while (player.Hand.Count < MIN_HAND_CARDS && this.Deck.Count > 0)
			{
				var card = this.Deck.Pop();
				player.Hand.Push(card);
			}
		}

		private bool CanPlaceCard(Card card)
		{
			var cardValue = card.Value;

			if (!Enum.IsDefined(cardValue))
			{
				return false;
			}

			var top = TopCard();


			if (top is null)
			{
				return true;
			}

			return cardValue switch
			{
				Value.Two => true,
				Value.Three => true,
				Value.Ten => true,
				var value when top.Value.Value == Value.Seven => cardComparer.Compare(value, Value.Seven) <= 0,
				var value => cardComparer.Compare(value, top.Value.Value) >= 0,
			};
		}

		private Card? TopCard()
		{
			foreach (var card in this.DiscardPile)
			{
				if (card.Value != Value.Three)
				{
					return card;
				}
			}

			return null;
		}
		#endregion
	}
}
