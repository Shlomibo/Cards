using System.Collections.Generic;

namespace Deck
{
	public interface IDeck<TCard> : IList<TCard>
		where TCard : class
	{
		TCard? Top { get; }

		void Push(TCard card);
		TCard Pop();
		void Shuffle();
	}
}
