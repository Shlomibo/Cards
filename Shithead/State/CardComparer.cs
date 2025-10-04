using Deck.Cards.FrenchSuited;

namespace Shithead.State;

internal sealed class CardComparer : IComparer<Card>, IComparer<Value>
{
	public static IReadOnlySet<Value> WildCards { get; } = new HashSet<Value>(new[]
	{
		Value.Joker,
		Value.Two,
		Value.Three,
		Value.Ten,
	});

	public static Dictionary<Value, int> CardValueRank { get; } = new()
	{
		[Value.Joker] = -1,
		[Value.Two] = 100,
		[Value.Three] = 100,
		[Value.Four] = 1,
		[Value.Five] = 2,
		[Value.Six] = 3,
		[Value.Seven] = 4,
		[Value.Eight] = 5,
		[Value.Nine] = 6,
		[Value.Ten] = 100,
		[Value.Jack] = 7,
		[Value.Queen] = 8,
		[Value.King] = 9,
		[Value.Ace] = 10,
	};

	public int Compare(Card cardX, Card cardY)
	{
		var x = cardX.Value;
		var y = cardY.Value;

		return Compare(x, y);
	}

	public int Compare(Value x, Value y)
	{
		if (!CardValueRank.ContainsKey(y))
		{
			return !CardValueRank.ContainsKey(x)
				? 0
				: 1;
		}
		else if (!CardValueRank.ContainsKey(x))
		{
			return -1;
		}
		else
		{
			return CardValueRank[x] - CardValueRank[y];
		}
	}
}
