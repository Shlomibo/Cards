using System;

namespace Deck.UnitTests.CardDeckTests;

public readonly record struct RandomCard
{
    public static Random Random { get; set; } = Random.Shared;
    public int Value { get; init; }

    public RandomCard()
    {
        Value = Random.Next();
    }
}
