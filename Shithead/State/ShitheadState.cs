using Deck.Cards.FrenchSuited;
using GameEngine;
using Shithead.ShitheadMove;
using TurnsManagement;

namespace Shithead.State
{
	public sealed partial class ShitheadState :
		IState<ShitheadState, ShitheadState.SharedShitheadState, ShitheadState.ShitheadPlayerState, IShitheadMove>
	{
		private const int MIN_PLAYERS_COUNT = 3;
		private const int MIN_HAND_CARDS = 3;
		private const int DEALT_CARDS = 6;
		private static readonly int suitSize = Enum.GetNames(typeof(Suit)).Length;

		#region Fields

		private readonly PlayerState[] players;
		private readonly TurnsManager turnsManager;
		private static readonly CardComparer cardComparer = new();
		private readonly object stateLock = new object();
		#endregion

		#region Properties

		public int PlayersCount { get; private set; }

		ShitheadState IState<ShitheadState, SharedShitheadState, ShitheadPlayerState, IShitheadMove>.GameState => this;
		public GameState GameState { get; private set; } = GameState.Init;

		public SharedShitheadState SharedState { get; }
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

			int maxPlayersCount = this.Deck.Count / (DEALT_CARDS + PlayerState.UNDERCARDS_COUNT);

			if (maxPlayersCount < playersCount)
			{
				throw new ArgumentException($"The maximum players count is {maxPlayersCount}", nameof(playersCount));
			}

			this.PlayersCount = playersCount;

			this.Deck.Shuffle();
			this.SharedState = new SharedShitheadState(this);
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

			Deal();
		}
		#endregion

		#region Methods

		public ShitheadPlayerState GetPlayerState(int playerId) =>
			new(playerId, this);

		public bool IsGameOver() =>
			this.GameState == GameState.GameOver;

		public bool IsValidMove(IShitheadMove move, int? player = null)
		{
			lock (this.stateLock)
			{
				return GetMove(move, player) is not null;
			}
		}

		public bool PlayMove(IShitheadMove move, int? player = null)
		{
			lock (this.stateLock)
			{
				var moveAction = GetMove(move, player);
				moveAction?.Invoke();

				return moveAction != null;
			}
		}

		private void Deal()
		{
			for (int i = 0; i < DEALT_CARDS; i++)
			{
				foreach (var player in this.players)
				{
					player.Hand.Push(this.Deck.Pop());
				}
			}
		}

		private int SelectStartingPlayer()
		{
			return (from player in this.players
					let lowestCard = (from card in player.Hand
									  where !CardComparer.WildCards.Contains(card.Value)
									  orderby card.Value ascending
									  select (Value?)card.Value).FirstOrDefault()
					where lowestCard.HasValue
					orderby CardComparer.CardValueRank[lowestCard.Value]
					select (int?)player.Id).FirstOrDefault() ?? 0;
		}

		private Action? GetMove(IShitheadMove move, int? playerId = null)
		{
			if (playerId is null)
			{
				return null;
			}

			var player = this.players[playerId.Value];

			return (this.GameState, move) switch
			{
				// GameState.Init
				(GameState.Init, RevealedCardSelection { CardIndex: var index, TargetIndex: var target })
					when player.CanSetRevealedCard(index, target) => () =>
					{
						var card = player.Hand[index];
						player.Hand.RemoveAt(index);

						player.RevealedCards.Add(target, card);
					}
				,
				(GameState.Init, UnsetRevealedCard { CardIndex: var index })
					when player.CanUnsetRevealedCard(index) => () =>
					{
						var card = player.RevealedCards[index];
						player.RevealedCards.Remove(index);

						player.Hand.Add(card);
					}
				,
				(GameState.Init, AcceptSelectedRevealedCards)
					when player.CanAcceptSelectedRevealedCards() => () =>
					{
						player.RevealedCardsAccepted = true;

						if (this.players.All(player => player.RevealedCardsAccepted))
						{
							this.turnsManager.Current = SelectStartingPlayer();
							this.GameState = GameState.GameOn;
						}
					}
				,
				(GameState.Init, ReselectRevealedCards)
					when player.CanReselectRevealedCards() => () =>
					{
						player.RevealedCardsAccepted = false;
					}
				,
				(GameState.Init, _) => null,

				// GameState.GameOn
				(GameState.GameOn, PlaceJoker { PlayerId: var targetPlayerId })
					when player.CanPlaceJoker() &&
						this.turnsManager.ActivePlayers.Contains(targetPlayerId) =>
					() =>
					{
						player.RemoveJoker();
						var targetPlayer = this.players[targetPlayerId];

						targetPlayer.Hand.Push(this.DiscardPile);
						this.DiscardPile.Clear();

						ReplenishPlayerHand(player);
						HandlePlayerWin(player);

						this.turnsManager.Current = targetPlayerId;
					}
				,
				(GameState.GameOn, PlaceCard { CardIndices: var indices })
					when this.turnsManager.Current == playerId &&
						player.CanPlaceCard(indices) &&
						CanPlaceCard(player.GetCard(indices.First())) =>
					() =>
					{
						var value = PlayHand(player, indices);
						HandlePlayerWin(player);
						bool pileDiscarded = ShouldDiscardPile(value);

						if (pileDiscarded)
						{
							this.DiscardPile.Clear();
						}
						// If the player won, it was removed and the turn belongs to the next player
						else if (!player.Won)
						{
							this.turnsManager.MoveNext();
						}

						if (value == Value.Eight && !pileDiscarded)
						{
							int otherPlayersCount = this.turnsManager.ActivePlayers.Count - 1;
							// We jump the count of eights, plus 1 as the turn should have passed anyway
							int turnsToJump = indices.Length % otherPlayersCount;

							if (turnsToJump == 0)
							{
								this.turnsManager.Current = player.Id;
							}
							else
							{
								this.turnsManager.Jump(turnsToJump);
							}
						}
					}
				,
				// When Player tries to add cards of the same value they got from deck, after finishing
				// their turn
				(GameState.GameOn, PlaceCard { CardIndices: var indices })
					when this.turnsManager.Previous == playerId &&
						player.CanPlaceCard(indices) &&
						player.GetCard(indices.First()).Value == TopCard()?.Value =>
					() =>
					{
						var value = PlayHand(player, indices);
						HandlePlayerWin(player);

						if (ShouldDiscardPile(value))
						{
							this.DiscardPile.Clear();
							this.turnsManager.Current = playerId.Value;
						}
						else if (value == Value.Eight)
						{
							this.turnsManager.Jump(indices.Length);
						}
					}
				,
				(GameState.GameOn, PlaceCard { CardIndices: var indices })
					when player.CanPlaceCard(indices) &&
						ShouldDiscardPileIfHadThese(indices.Select(i => player.GetCard(i))) =>
					() =>
					{
						PlayHand(player, indices);
						this.DiscardPile.Clear();
						this.turnsManager.Current = player.Id;
					}
				,
				(GameState.GameOn, AcceptDiscardPile)
					when this.turnsManager.Current == playerId &&
						player.Hand.Count > 0 =>
					() =>
					{
						player.Hand.Push(this.DiscardPile);
						this.DiscardPile.Clear();
						this.turnsManager.MoveNext();
					}
				,
				(GameState.GameOn, RevealUndercard { CardIndex: var cardIndex })
					when player.CanRevealUndercard(cardIndex) => () =>
					{
						player.Undercards[cardIndex].IsRevealed = true;
					}
				,
				(GameState.GameOn, TakeUndercards { CardIndices: var cardIndices })
					when this.turnsManager.Current == playerId &&
						player.CanTakeUndercards(cardIndices) =>
					() =>
					{
						if (player.RevealedCards.Count == 0)
						{
							int i = cardIndices[0];

							player.Hand.Push(player.Undercards[i].Card);
							player.Undercards.Remove(i);
						}
						else
						{
							foreach (int i in cardIndices)
							{
								player.Hand.Push(player.RevealedCards[i]);
								player.RevealedCards.Remove(i);
							}
						}
					}
				,

				(GameState.GameOn, LeaveGame { PlayerId: var leavingPlayerId })
					when this.turnsManager.ActivePlayers.Contains(leavingPlayerId) => () =>
					{
						RemovePlayer(leavingPlayerId);
					}
				,
				(GameState.GameOn, _) => null,

				// GameState.GameOver
				(GameState.GameOver, _) => null,

				_ => throw new Exception("Invalid game state"),
			};
		}

		private void RemovePlayer(int leavingPlayerId)
		{
			this.turnsManager.RemovePlayer(leavingPlayerId);
			var leavingPlayer = this.players[leavingPlayerId];

			leavingPlayer.DidLeaveGame = true;
			leavingPlayer.Hand.Clear();
			leavingPlayer.RevealedCards.Clear();

			this.Deck.Add(
				from undercard in leavingPlayer.Undercards.Values
				where !undercard.IsRevealed
				select undercard.Card
			);
			leavingPlayer.Undercards.Clear();
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
			var top = this.DiscardPile.Take(suitSize).ToArray();

			return cardValue == Value.Ten || (top.Length == suitSize &&
				top.All(discard => discard.Value == cardValue));
		}

		private bool ShouldDiscardPileIfHadThese(IEnumerable<Card> cards)
		{
			var top = cards
				.Concat(this.DiscardPile)
				.Take(suitSize)
				.ToArray();

			if (top.Length < suitSize)
			{
				return false;
			}

			var topValue = top.First().Value;

			return top.Skip(1).All(card => card.Value == topValue);
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

			return cardValue switch
			{
				_ when top is null => true,
				Value.Two => true,
				Value.Three => true,
				Value.Ten => true,
				_ when top.Value.Value == Value.Two => true,
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
