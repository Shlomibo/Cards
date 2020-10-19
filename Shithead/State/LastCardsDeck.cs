using Deck.Cards.Regular;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;

namespace Shithead.State
{
	using static Consts;

	public sealed class LastCardsDeck : IReadOnlyList<PlayerCards>
	{
		#region Fields

		private readonly Card[,] cards;
		private readonly PlayerCards[] cardsByPlayer;
		#endregion

		#region Properties

		public PlayerCards this[int player] => this.cardsByPlayer[player];

		public int Count => this.cardsByPlayer.Length;
		#endregion

		#region Ctors

		public LastCardsDeck(Card[,] lastCards)
		{
			this.cards = lastCards ?? throw new ArgumentNullException(nameof(lastCards));
			this.cardsByPlayer = Enumerable.Range(0, lastCards.GetLength(0))
				.Select(player => new PlayerCards(player, this.cards, _ => true))
				.ToArray();
		}
		#endregion

		#region Methods

		public IEnumerator<PlayerCards> GetEnumerator() =>
			this.cardsByPlayer
			.Cast<PlayerCards>()
			.GetEnumerator();

		IEnumerator IEnumerable.GetEnumerator() =>
			GetEnumerator(); 
		#endregion
	}

	public sealed class PlayerCards : IReadOnlyList<Card?>
	{
		#region Fields

		private readonly int player;
		private readonly Card[,] cards;
		private readonly Func<int, bool> isCardVisible;
		#endregion

		#region Properties

		public Card this[int index] => this.cards[player, index];

		public int Count => LAST_CARDS_COUNT;
		#endregion

		#region Ctors

		public PlayerCards(int player, Card[,] cards, Func<int, bool> isCardVisible)
		{
			this.player = player;
			this.cards = cards;
			this.isCardVisible = isCardVisible;
		}
		#endregion

		#region Methods

		public IEnumerator<Card> GetEnumerator()
		{
			for (int i = 0; i < LAST_CARDS_COUNT; i++)
			{
				yield return this[i];
			}
		}

		IEnumerator IEnumerable.GetEnumerator() =>
			GetEnumerator(); 
		#endregion
	}
}
