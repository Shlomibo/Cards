using System;

using AutoFixture;

using AwesomeAssertions;

using Deck;
using Deck.Cards.FrenchSuited;

using NUnit.Framework.Internal;

using Shithead.Moves;
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

    protected static void ValidateValidMove(
        ShitheadState testSubject,
        PlayerState player,
        Move move,
        Action afterPlayStateValidation,
        Card[]? originalHand = null,
        Dictionary<int, Card>? originalRevealed = null,
        Dictionary<int, CardFace<Card>>? originalUndercards = null)
    {
        testSubject.IsValidMove(move, player.Id).Should().BeTrue("the move is valid");

        if (originalHand != null)
        {
            player.Hand.Should().BeEquivalentTo(originalHand, "move not played yet");
        }

        if (originalRevealed != null)
        {
            player.RevealedCards.Should().BeEquivalentTo(originalRevealed);
        }

        if (originalUndercards != null)
        {
            player.Undercards.Should().BeEquivalentTo(originalUndercards);
        }

        testSubject.PlayMove(move, player.Id).Should().BeTrue("the played move is valid");
        afterPlayStateValidation();

        testSubject.LastMove.Should().BeEquivalentTo((move, player.Id));
        testSubject.LastPlayedMove.Should().BeEquivalentTo((move, player.Id));
    }

    protected static void ValidateInvalidMove(
        ShitheadState testSubject,
        PlayerState player,
        Move move,
        Card[]? originalHand = null,
        Dictionary<int, Card>? originalRevealed = null,
        Dictionary<int, CardFace<Card>>? originalUndercards = null)
    {
        testSubject.IsValidMove(move, player.Id).Should().BeFalse("the move is invalid");

        if (originalHand != null)
        {
            player.Hand.Should().BeEquivalentTo(originalHand, "move not played yet");
        }

        if (originalRevealed != null)
        {
            player.RevealedCards.Should().BeEquivalentTo(originalRevealed);
        }

        if (originalUndercards != null)
        {
            player.Undercards.Should().BeEquivalentTo(originalUndercards);
        }

        testSubject.PlayMove(move, player.Id).Should().BeFalse("the played move is invalid");

        if (originalHand != null)
        {
            player.Hand.Should().BeEquivalentTo(originalHand, "move not played yet");
        }

        if (originalRevealed != null)
        {
            player.RevealedCards.Should().BeEquivalentTo(originalRevealed);
        }

        if (originalUndercards != null)
        {
            player.Undercards.Should().BeEquivalentTo(originalUndercards);
        }

        testSubject.LastMove.Should().BeEquivalentTo((move, player.Id));
        testSubject.LastPlayedMove.Should().BeNull();
    }

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
