using System;
using System.Diagnostics;

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

    protected static Card RandomCard(Value? value = null, Suit? suit = null) =>
        new(value ?? Fixture.Create<Value>(), suit ?? Fixture.Create<Suit>());

    private protected static ShitheadState GetTestSubject(
        IReadOnlyCollection<PlayerData> playerData,
        GameState gameState,
        CardsDeck deck,
        IEnumerable<Card> discardPile)
    {
        ShitheadState result = new(
            deck,
            [.. playerData.Select((p, i) => p.CreateState(i))],
            gameState);

        result.DiscardPile.Push(discardPile);

        return result;
    }

    protected static Card[] Discard(CardsDeck deck, int count) =>
        [.. Enumerable.Repeat(ValueTuple.Create(), count)
                .Select(_ => deck.Pop())];

    protected static CardsDeck DealHand(CardsDeck source, int count = 3) =>
        [.. Enumerable.Repeat(ValueTuple.Create(), count)
            .Select(_ => source.Pop())];

    protected static Dictionary<int, Card> DealRevealedCards(CardsDeck source) =>
        DealHand(source)
            .Select((card, i) => KeyValuePair.Create(i, card))
            .ToDictionary();

    protected static Dictionary<int, CardFace<Card>> DealUndercards(CardsDeck source) =>
        DealRevealedCards(source)
            .Select(kv => KeyValuePair.Create(kv.Key, (CardFace<Card>)kv.Value))
            .ToDictionary();

    protected static void ValidateValidMove(
        ShitheadState testSubject,
        PlayerState player,
        Move move,
        Action afterPlayStateValidation,
        Card[]? originalHand = null,
        Dictionary<int, Card>? originalRevealed = null,
        Dictionary<int, CardFace<Card>>? originalUndercards = null,
        Card[]? originalDiscardPile = null)
    {
        int currentTurn = testSubject.TurnsManager.Current;
        testSubject.IsValidMove(move, player.Id).Should().BeTrue("the move is valid");
        testSubject.TurnsManager.Current.Should().Be(currentTurn, "move not played yet");

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

        if (originalDiscardPile != null)
        {
            testSubject.DiscardPile.Should().BeEquivalentTo(originalDiscardPile);
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
        Dictionary<int, CardFace<Card>>? originalUndercards = null,
        Card[]? originalDiscardPile = null)
    {
        int currentTurn = testSubject.TurnsManager.Current;
        testSubject.IsValidMove(move, player.Id).Should().BeFalse("the move is invalid");

        testSubject.TurnsManager.Current.Should().Be(currentTurn, "move not played yet");

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

        if (originalDiscardPile != null)
        {
            testSubject.DiscardPile.Should().BeEquivalentTo(originalDiscardPile);
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

        if (originalDiscardPile != null)
        {
            testSubject.DiscardPile.Should().BeEquivalentTo(originalDiscardPile);
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
        fixture.Register(() => Random.Next(8) switch
                {
                    0 => Value.Four,
                    1 => Value.Five,
                    2 => Value.Six,
                    3 => Value.Nine,
                    4 => Value.Jack,
                    5 => Value.Queen,
                    6 => Value.King,
                    7 => Value.Ace,
                    _ => throw new UnreachableException(),
                });

        return fixture;
    }
}
