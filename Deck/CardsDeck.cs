using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;

namespace Deck
{
	public class CardsDeck<TCard> : IDeck<TCard>
		where TCard : class
	{
		#region Fields

		private readonly List<TCard> cards;
		private readonly IEnumerable<TCard> cardsEnumerable;
		#endregion

		#region Properties

		public TCard this[int index]
		{
			get => this.cards[^index];
			set => this.cards[^index] = value;
		}

		public TCard? Top => this.Count == 0
			? null
			: this.cards[^1];

		public int Count => this.cards.Count;

		bool ICollection<TCard>.IsReadOnly => false;
		#endregion

		#region Ctors

		public CardsDeck()
		{
			this.cards = new List<TCard>();
			this.cardsEnumerable = this.cards.AsEnumerable().Reverse();
		}

		public CardsDeck(IEnumerable<TCard> cards)
		{
			this.cards = cards is CardsDeck<TCard> deck
				? new List<TCard>(deck.cards)
				: new List<TCard>(cards.Reverse());
			this.cardsEnumerable = this.cards.AsEnumerable().Reverse();
		}
		#endregion

		#region Methods

		public void Add(TCard card) =>
			this.Push(card);

		public void Clear() =>
			this.cards.Clear();

		public bool Contains(TCard card) =>
			this.cards.Contains(card);

		public void CopyTo(TCard[] array, int arrayIndex) =>
			this.cards.CopyTo(array, arrayIndex);

		public IEnumerator<TCard> GetEnumerator() =>
			this.cardsEnumerable.GetEnumerator();

		public int IndexOf(TCard card) =>
			ReversedIndex(this.cards.LastIndexOf(card));

		public void Insert(int index, TCard card) =>
			this.cards.Insert(ReversedIndex(index), card);

		public TCard Pop()
		{
			if (this.Count == 0)
			{
				throw new InvalidOperationException("Deck is empty");
			}

			var card = this.Top!;
			this.cards.RemoveAt(this.Count - 1);

			return card;
		}

		public void Push(TCard card)
		{
			this.cards.Add(card);
		}

		public bool Remove(TCard card)
		{
			int lastIndex = this.cards.LastIndexOf(card);

			if (lastIndex == -1)
			{
				return false;
			}

			this.cards.RemoveAt(lastIndex);
			return true;
		}

		public void RemoveAt(int index) =>
			this.cards.RemoveAt(ReversedIndex(index));

		public void Shuffle()
		{
			var rand = new Random();
			var tempList = new LinkedList<TCard>(this.cards);
			this.cards.Clear();
			var node = tempList.First;

			while (tempList.Count > 0)
			{
				int next = rand.Next(tempList.Count);

				for (int i = 0; i < next; i++)
				{
					node = node.Next ?? tempList.First;
				}

				this.Push(node.Value);
				node = node.Next ?? tempList.First;
				tempList.Remove(node.Previous ?? tempList.Last);
			}
		}

		IEnumerator IEnumerable.GetEnumerator() =>
			GetEnumerator();

		private int ReversedIndex(int index) =>
			this.Count - index - 1;
		#endregion
	}
}
