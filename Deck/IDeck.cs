namespace Deck;

public interface IReadonlyDeck<TCard> : IReadOnlyList<TCard>
	where TCard : struct
{
	TCard? Top { get; }
}

public interface IDeck<TCard> : IList<TCard>, IReadonlyDeck<TCard>
	where TCard : struct
{
	new int Count { get; }
	new TCard this[int index] { get; set; }

	void Push(TCard card);
	void Push(params TCard[ ]? cards);
	void Push(IEnumerable<TCard> cards);
	void Add(params TCard[ ]? cards);
	void Add(IEnumerable<TCard> cards);
	TCard Pop();
	void Shuffle();
	IReadonlyDeck<TCard> AsReadonly();
}
