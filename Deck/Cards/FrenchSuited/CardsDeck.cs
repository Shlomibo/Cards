using System.Collections;
using System.Collections.Generic;

namespace Deck.Cards.FrenchSuited
{
	public sealed class CardsDeck : IDeck<Card>
	{
		private readonly CardsDeck<Card> deck;

		public CardsDeck()
		{
			this.deck = new CardsDeck<Card>();
		}

		public CardsDeck(IEnumerable<Card> cards)
		{
			this.deck = new CardsDeck<Card>(cards);
		}

		#region Properties 

		public Card this[int index]
		{
			get => this.deck[index];
			set => this.deck[index] = value;
		}

		Card IReadOnlyList<Card>.this[int index] => this.deck[index];

		public Card? Top => this.deck.Top;

		public int Count => this.deck.Count;

		bool ICollection<Card>.IsReadOnly => ((ICollection<Card>)this.deck).IsReadOnly;
		#endregion

		#region Methods

		public static CardsDeck FullDeck(bool excludeJokers = false) => 
			new(Card.AllCards(excludeJokers));

		#region IDeck<Card> Methods
		public void Add(Card item)
		{
			this.deck.Add(item);
		}

		public void Clear()
		{
			this.deck.Clear();
		}

		public bool Contains(Card item)
		{
			return this.deck.Contains(item);
		}

		public void CopyTo(Card[] array, int arrayIndex)
		{
			this.deck.CopyTo(array, arrayIndex);
		}

		public IEnumerator<Card> GetEnumerator()
		{
			return this.deck.GetEnumerator();
		}

		public int IndexOf(Card item)
		{
			return this.deck.IndexOf(item);
		}

		public void Insert(int index, Card item)
		{
			this.deck.Insert(index, item);
		}

		public Card Pop()
		{
			return this.deck.Pop();
		}

		public void Push(Card card)
		{
			this.deck.Push(card);
		}

		public void Push(params Card[]? cards)
		{
			if (cards != null)
			{
				Push(cards.AsEnumerable());
			}
		}

		public void Push(IEnumerable<Card> cards)
		{
			foreach (var card in cards)
			{
				Push(card);
			}
		}

		public bool Remove(Card item)
		{
			return this.deck.Remove(item);
		}

		public void RemoveAt(int index)
		{
			this.deck.RemoveAt(index);
		}

		public void Shuffle()
		{
			this.deck.Shuffle();
		}

		IEnumerator IEnumerable.GetEnumerator()
		{
			return ((IEnumerable)this.deck).GetEnumerator();
		}
		#endregion
		#endregion
	}
}
