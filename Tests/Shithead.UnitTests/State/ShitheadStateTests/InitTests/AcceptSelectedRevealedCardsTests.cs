using System;

using AwesomeAssertions;

using Deck;
using Deck.Cards.FrenchSuited;

using Shithead.Moves;
using Shithead.State;

namespace Shithead.UnitTests.State.ShitheadStateTests.InitTests;

public class AcceptSelectedRevealedCardsTests : InitTestsBase
{
    [Test]
    public void WhenAllRevealedCardsAreSet()
    {
        var deck = CardsDeck.FullShuffledDeck();

        var testSubject = GetTestSubject(
            [
                new PlayerData(
                    DealHand(deck, 6),
                    new Dictionary<int, Card>
                    {
                        [0] = deck.Pop(),
                        [1] = deck.Pop(),
                        [2] = deck.Pop(),
                    },
                    DealUndercards(deck)),
                new PlayerData(
                    DealHand(deck, 6),
                    RevealedCards: [],
                    DealUndercards(deck))],
            deck);

        var player = testSubject.PlayerStates[0];
        var originalHand = player.Hand.ToArray();
        var originalRevealed = player.RevealedCards.ToDictionary();

        AcceptSelectedRevealedCards move = new();

        ValidateValidMove(
            testSubject,
            player,
            move,
            () =>
            {
                player.RevealedCardsAccepted.Should().BeTrue("revealed cards accepted");
                testSubject.GameState.Should().Be(GameState.Init, "game still initializing");
            },
            originalHand,
            originalRevealed);
    }

    [Test]
    public void WhenAllOtherPlayerHaveAcceptedTheirRevealedCards()
    {
        var deck = CardsDeck.FullShuffledDeck();

        var testSubject = GetTestSubject(
            [
                new PlayerData(
                    DealHand(deck, 6),
                    new Dictionary<int, Card>
                    {
                        [0] = deck.Pop(),
                        [1] = deck.Pop(),
                        [2] = deck.Pop(),
                    },
                    DealUndercards(deck)),
                new PlayerData(
                    DealHand(deck, 6),
                    new Dictionary<int, Card>
                    {
                        [0] = deck.Pop(),
                        [1] = deck.Pop(),
                        [2] = deck.Pop(),
                    },
                    DealUndercards(deck))],
            deck);
        testSubject.PlayerStates[1].RevealedCardsAccepted = true;
        var player = testSubject.PlayerStates[0];
        var originalHand = player.Hand.ToArray();
        var originalRevealed = player.RevealedCards.ToDictionary();

        AcceptSelectedRevealedCards move = new();

        ValidateValidMove(
            testSubject,
            player,
            move,
            () =>
            {
                player.RevealedCardsAccepted.Should().BeTrue("revealed cards accepted");
                testSubject.GameState.Should().Be(GameState.GameOn, "the gave have started");
            },
            originalHand,
            originalRevealed);
    }

    [Test]
    public void WhenNotAllRevealedCardsAreSet()
    {
        var deck = CardsDeck.FullShuffledDeck();

        var testSubject = GetTestSubject(
            [
                new PlayerData(
                    DealHand(deck, 6),
                    new Dictionary<int, Card>
                    {
                        [0] = deck.Pop(),
                        [2] = deck.Pop(),
                    },
                    DealUndercards(deck)),
                new PlayerData(
                    DealHand(deck, 6),
                    RevealedCards: [],
                    DealUndercards(deck))],
            deck);

        var player = testSubject.PlayerStates[0];
        var originalHand = player.Hand.ToArray();
        var originalRevealed = player.RevealedCards.ToDictionary();

        AcceptSelectedRevealedCards move = new();

        ValidateInvalidMove(
            testSubject,
            player,
            move,
            originalHand,
            originalRevealed);
    }

    [Test]
    public void WhenRevealedCardsHaveAlreadyAccepted()
    {
        var deck = CardsDeck.FullShuffledDeck();

        var testSubject = GetTestSubject(
            [
                new PlayerData(
                    DealHand(deck, 6),
                    new Dictionary<int, Card>
                    {
                        [0] = deck.Pop(),
                        [1] = deck.Pop(),
                        [2] = deck.Pop(),
                    },
                    DealUndercards(deck)),
                new PlayerData(
                    DealHand(deck, 6),
                    RevealedCards: [],
                    DealUndercards(deck))],
            deck);

        var player = testSubject.PlayerStates[0];
        player.RevealedCardsAccepted = true;
        var originalHand = player.Hand.ToArray();
        var originalRevealed = player.RevealedCards.ToDictionary();

        AcceptSelectedRevealedCards move = new();

        ValidateInvalidMove(
            testSubject,
            player,
            move,
            originalHand,
            originalRevealed);
    }
}
