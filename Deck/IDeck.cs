using System.Collections.Generic;

namespace Deck
{
	public interface IDeck<TCard> : IList<TCard>, IReadOnlyList<TCard>
		where TCard: struct
	{
		TCard? Top { get; }
		new int Count { get; }

		void Push(TCard card);
		void Push(params TCard[]? cards);
		void Push(IEnumerable<TCard> cards);
		TCard Pop();
		void Shuffle();
	}
}
