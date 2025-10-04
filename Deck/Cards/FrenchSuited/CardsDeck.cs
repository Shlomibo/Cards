using System.Collections;

namespace Deck.Cards.FrenchSuited;

public sealed class CardsDeck : IDeck<Card>
{
	private readonly CardsDeck<Card> deck;

	public CardsDeck()
	{
		deck = new CardsDeck<Card>();
	}

	public CardsDeck(IEnumerable<Card> cards)
	{
		deck = new CardsDeck<Card>(cards);
	}

	#region Properties 

	public Card this[int index]
	{
		get => deck[index];
		set => deck[index] = value;
	}

	Card IReadOnlyList<Card>.this[int index] => deck[index];

	public Card? Top => deck.Top;

	public int Count => deck.Count;

	bool ICollection<Card>.IsReadOnly => ((ICollection<Card>)deck).IsReadOnly;
	#endregion

	#region Methods

	public static CardsDeck FullDeck(bool excludeJokers = false) =>
		new(Card.AllCards(excludeJokers));

	#region IDeck<Card> Methods
	public void Add(Card item)
	{
		deck.Add(item);
	}

	public void Add(params Card[]? cards)
	{
		deck.Add(cards);
	}

	public void Add(IEnumerable<Card> cards)
	{
		deck.Add(cards);
	}

	public void Clear()
	{
		deck.Clear();
	}

	public bool Contains(Card item)
	{
		return deck.Contains(item);
	}

	public void CopyTo(Card[] array, int arrayIndex)
	{
		deck.CopyTo(array, arrayIndex);
	}

	public IEnumerator<Card> GetEnumerator()
	{
		return deck.GetEnumerator();
	}

	public int IndexOf(Card item)
	{
		return deck.IndexOf(item);
	}

	public void Insert(int index, Card item)
	{
		deck.Insert(index, item);
	}

	public Card Pop()
	{
		return deck.Pop();
	}

	public void Push(Card card)
	{
		deck.Push(card);
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
		return deck.Remove(item);
	}

	public void RemoveAt(int index)
	{
		deck.RemoveAt(index);
	}

	public void Shuffle()
	{
		deck.Shuffle();
	}

	IEnumerator IEnumerable.GetEnumerator()
	{
		return ((IEnumerable)deck).GetEnumerator();
	}
	public IReadonlyDeck<Card> AsReadonly() => new ReadonlyDeck<Card>(this);
	#endregion
	#endregion
}
