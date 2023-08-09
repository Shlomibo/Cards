using System.Collections.Generic;

namespace Deck
{
	public interface IDeck<TCard> : IList<TCard>, IReadOnlyList<TCard>
		where TCard: struct
	{
		TCard? Top { get; }
		new int Count { get; }

		void Push(TCard card);
		TCard Pop();
		void Shuffle();
	}
}
