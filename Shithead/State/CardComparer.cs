using Deck.Cards.FrenchSuited;

namespace Shithead.State;

internal sealed class CardComparer : IComparer<Card>, IComparer<Value>
{
    public static CardComparer Instance { get; } = new();

    public static IReadOnlySet<Value> WildCards { get; } = new HashSet<Value>
    {
        Value.Joker,
        Value.Two,
        Value.Three,
        Value.Ten,
    };

    public static Dictionary<Value, int> CardValueRank { get; } = new()
    {
        [Value.Joker] = -1,
        [Value.Two] = 100,
        [Value.Three] = 100,
        [Value.Four] = 4,
        [Value.Five] = 5,
        [Value.Six] = 6,
        [Value.Seven] = 7,
        [Value.Eight] = 8,
        [Value.Nine] = 9,
        [Value.Ten] = 100,
        [Value.Jack] = 10,
        [Value.Queen] = 11,
        [Value.King] = 12,
        [Value.Ace] = 13,
    };

    public int Compare(Card cardX, Card cardY)
    {
        var x = cardX.Value;
        var y = cardY.Value;

        return Compare(x, y);
    }

    public int Compare(Value x, Value y)
    {
#pragma warning disable IDE0046 // Convert to conditional expression
        if (!CardValueRank.TryGetValue(y, out int yValue))
        {
            return !CardValueRank.ContainsKey(x)
                ? 0
                : 1;
        }
        else if (!CardValueRank.TryGetValue(x, out int xValue))
        {
            return -1;
        }
        else
        {
            return xValue - yValue;
        }
#pragma warning restore IDE0046 // Convert to conditional expression
    }
}
