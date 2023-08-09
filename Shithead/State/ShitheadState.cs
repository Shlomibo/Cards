using Deck.Cards.FrenchSuited;
using GameEngine;
using Shithead.ShitheadMove;
using System;
using System.Linq;
using TurnsManagement;

namespace Shithead.State
{
	public sealed partial class ShitheadState :
		IState<ShitheadState, ShitheadState.Shared, ShitheadState.Player, IShitheadMove>
	{
		private const int MIN_PLAYERS_COUNT = 3;
		#region Fields

		private PlayerState[] players;
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
			this.players = Enumerable
				.Range(0, playersCount)
				.Select(i => new PlayerState(
					Enumerable.Range(0, PlayerState.UNDERCARDS_COUNT)
					.Select(_ => this.Deck.Pop())
					.ToArray()
				))
				.ToArray();
		}
		#endregion

		#region Methods

		public Player GetPlayerState(int playerId) =>
			new Player(playerId, this);

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
					RevealedCardsSelection { CardIndex: var index } when player.CanSetRevealedCard(index) => () =>
					{
						var card = player.Hand[index];
						player.Hand.RemoveAt(index);

						player.RevealedCards.Add(card);
					}
					,
					UnsetRevealedCard { CardIndex: var index } when player.CanUnsetRevealedCard(index) => () =>
					{
						var card = player.RevealedCards[index];
						player.RevealedCards.RemoveAt(index);

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

				_ => throw new Exception("Invalid game state"),
			};
		}
		#endregion
	}
}
