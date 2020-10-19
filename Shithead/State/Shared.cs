using Deck.Cards.Regular;
using System;
using System.Collections.Generic;

namespace Shithead.State
{
	public sealed class Shared
	{
		#region Fields

		private readonly Game gameState;
		#endregion

		#region Properties

		public LastCardsDeck UpFacingCards { get; }
		public int DeckSize => this.gameState.MainDeck.Count;
		public IReadOnlyList<Card> Pile => (IReadOnlyList<Card>)this.gameState.Pile;
		#endregion

		#region Ctors

		public Shared(Game gameState)
		{
			this.gameState = gameState ?? throw new ArgumentNullException(nameof(gameState));
			this.UpFacingCards = new LastCardsDeck(gameState.UpFacingCards);
		} 
		#endregion
	}
}
