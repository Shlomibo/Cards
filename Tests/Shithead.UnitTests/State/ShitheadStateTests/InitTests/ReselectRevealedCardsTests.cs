using System;

using AwesomeAssertions;

using Deck;
using Deck.Cards.FrenchSuited;

using Shithead.Moves;
using Shithead.State;

namespace Shithead.UnitTests.State.ShitheadStateTests.InitTests;

public class ReselectRevealedCardsTests : InitTestsBase
{
    [Test]
    public void WhenRevealedCardsAreAccepted()
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

        ReselectRevealedCards move = new();

        ValidateValidMove(
            testSubject,
            player,
            move,
            () =>
            {
                player.RevealedCardsAccepted.Should().BeFalse("revealed cards are reselected");
            },
            originalHand,
            originalRevealed);
    }

    [Test]
    public void WhenTheGameHaveStarted()
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

        testSubject.GameState = GameState.GameOn;
        var player = testSubject.PlayerStates[0];
        player.RevealedCardsAccepted = true;
        var originalHand = player.Hand.ToArray();
        var originalRevealed = player.RevealedCards.ToDictionary();

        ReselectRevealedCards move = new();

        ValidateInvalidMove(
            testSubject,
            player,
            move,
            originalHand,
            originalRevealed);
    }

    [Test]
    public void WhenRevealedCardsHaveNotAlreadyAccepted()
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

        ReselectRevealedCards move = new();

        ValidateInvalidMove(
            testSubject,
            player,
            move,
            originalHand,
            originalRevealed);
    }
}
