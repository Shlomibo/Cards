using System.Collections;

namespace Deck;

public sealed class CardsDeck<TCard> : IDeck<TCard>
	where TCard : struct
{
	#region Fields

	private readonly List<TCard> cards;

	private readonly Random rand = new();
	#endregion

	#region Properties

	public TCard this[int index]
	{
		get => cards[ReversedIndex(index)];
		set => cards[ReversedIndex(index)] = value;
	}

	public TCard? Top => Count == 0
		? null
		: cards[^1];

	public int Count => cards.Count;

	bool ICollection<TCard>.IsReadOnly => false;
	#endregion

	#region Ctors

	public CardsDeck()
	{
		cards = new List<TCard>();
	}

	public CardsDeck(IEnumerable<TCard> cards)
	{
		this.cards = cards is CardsDeck<TCard> deck
			? new List<TCard>(deck.cards)
			: new List<TCard>(cards.Reverse());
	}
	#endregion

	#region Methods

	public void Add(TCard card) =>
		cards.Insert(0, card);

	public void Add(params TCard[]? cards)
	{
		if (cards != null)
		{
			Add(cards.AsEnumerable());
		}
	}

	public void Add(IEnumerable<TCard> cards)
	{
		this.cards.InsertRange(0, cards);
	}

	public void Clear() =>
		cards.Clear();

	public bool Contains(TCard card) =>
		cards.Contains(card);

	public void CopyTo(TCard[] array, int arrayIndex)
	{
		if (array is null)
		{
			throw new ArgumentNullException(nameof(array));
		}

		if (arrayIndex < 0)
		{
			throw new ArgumentOutOfRangeException(nameof(arrayIndex));
		}

		if (array.Length - arrayIndex < Count)
		{
			throw new ArgumentException(
				"Destination array was not long enough. " +
					"Check the destination index, length, and the array's lower bounds.",
				nameof(array)
			);
		}

		for (int i = 0; i < Count; i++)
		{
			array[i + arrayIndex] = this[i];
		}
	}

	public IEnumerator<TCard> GetEnumerator()
	{
		for (int i = 0; i < Count; i++)
		{
			yield return this[i];
		}
	}

	public int IndexOf(TCard card) =>
		ReversedIndex(cards.LastIndexOf(card));

	public void Insert(int index, TCard card) =>
		cards.Insert(ReversedIndex(index), card);

	public TCard Pop()
	{
		if (Count == 0)
		{
			throw new InvalidOperationException("Deck is empty");
		}

		var card = Top!;
		cards.RemoveAt(Count - 1);

		return card.Value;
	}

	// We are listing the cards in reverse, so pushing "adds" and adding "pushes"
	public void Push(TCard card)
	{
		cards.Add(card);
	}

	public void Push(params TCard[]? cards)
	{
		if (cards != null)
		{
			Push(cards.AsEnumerable());
		}
	}

	public void Push(IEnumerable<TCard> cards)
	{
		foreach (var card in cards)
		{
			Push(card);
		}
	}

	public bool Remove(TCard card)
	{
		int lastIndex = cards.LastIndexOf(card);

		if (lastIndex == -1)
		{
			return false;
		}

		cards.RemoveAt(lastIndex);
		return true;
	}

	public void RemoveAt(int index) =>
		cards.RemoveAt(ReversedIndex(index));

	public void Shuffle()
	{
		if (cards.Count == 0)
		{
			return;
		}

		var tempList = new LinkedList<TCard>(cards);
		cards.Clear();
		var node = tempList.First!;

		while (tempList.Count > 0)
		{
			int next = rand.Next(tempList.Count);

			for (int i = 0; i < next; i++)
			{
				node = node.Next ?? tempList.First!;
			}

			cards.Add(node.Value);
			node = node.Next ?? tempList.First!;

			tempList.Remove(node.Previous ?? tempList.Last!);
		}
	}

	IEnumerator IEnumerable.GetEnumerator() =>
		GetEnumerator();

	public IReadonlyDeck<TCard> AsReadonly() => new ReadonlyDeck<TCard>(this);

	private int ReversedIndex(int index) =>
		Math.Max(Count - index - 1, 0);
	#endregion
}

public sealed class ReadonlyDeck<TCard> : IReadonlyDeck<TCard>
	where TCard : struct
{
	private readonly IDeck<TCard> deck;

	public ReadonlyDeck(IDeck<TCard> deck)
	{
		this.deck = deck;
	}

	public TCard this[int index] => deck[index];

	public TCard? Top => deck.Top;

	public int Count => deck.Count;

	public IEnumerator<TCard> GetEnumerator() => deck.GetEnumerator();

	IEnumerator IEnumerable.GetEnumerator() => GetEnumerator();
}
