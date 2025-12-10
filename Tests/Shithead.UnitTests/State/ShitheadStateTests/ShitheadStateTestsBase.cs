using System;

using AutoFixture;

using Deck;
using Deck.Cards.FrenchSuited;

using NUnit.Framework.Internal;

using Shithead.State;

namespace Shithead.UnitTests.State.ShitheadStateTests;

public abstract class ShitheadStateTestsBase
{
    protected static TestContext TestContext => TestContext.CurrentContext;
    protected static Randomizer Random => TestContext.Random;
    protected static Fixture Fixture { get; } = CreateFixture();

    protected static Card RandomCard() =>
        new(Fixture.Create<Value>(), Fixture.Create<Suit>());

    private protected static ShitheadState GetTestSubject(
        IReadOnlyCollection<PlayerData> playerData,
        GameState gameState,
        CardsDeck deck)
        =>
        new(
            deck,
            [.. playerData.Select((p, i) => p.CreateState(i))],
            gameState);

    protected static CardsDeck DealHand(CardsDeck source, int count = 3) =>
        [.. Enumerable.Repeat(ValueTuple.Create(), count)
            .Select(_ => source.Pop())];

    protected static Dictionary<int, CardFace<Card>> DealUndercards(CardsDeck source) =>
        DealHand(source)
            .Select((card, i) => KeyValuePair.Create(i, new CardFace<Card>(card)))
            .ToDictionary();

    protected readonly record struct PlayerData(
        CardsDeck Hand,
        Dictionary<int, Card> RevealedCards,
        Dictionary<int, CardFace<Card>> Undercards)
    {
        public PlayerState CreateState(int id) => new(
            id,
            Hand,
            RevealedCards,
            Undercards);
    }

    private static Fixture CreateFixture()
    {
        Fixture fixture = new();
        fixture.Register(() => (Value)Random.Next(1, (int)Value.King + 1));

        return fixture;
    }
}
