using Deck.Cards.Regular;
using System;
using System.Collections.Generic;

namespace Shithead.State
{
	public sealed class Player
	{
		#region Fields

		private readonly int player;
		private readonly Game game;
		private readonly Shared shared;
		#endregion

		#region Properties

		public PlayerCards DownFacingCards { get; }
		public PlayerCards UpFacingCards => this.shared.UpFacingCards[this.player];
		public IReadOnlyList<Card> Deck => this.game.PlayersDecks[this.player];
		#endregion

		#region Ctors

		public Player(Game game, Shared shared, int player)
		{
			this.game = game ?? throw new ArgumentNullException(nameof(game));
			this.shared = shared ?? throw new ArgumentNullException(nameof(shared));
			this.player = player;
			this.DownFacingCards = new PlayerCards(
				player,
				game.DownFacingCards,
				card => game.DownFacingCardsVisibilty[player, card]
			);
		} 
		#endregion
	}
}
