using System;

using AwesomeAssertions;

using Deck;
using Deck.Cards.FrenchSuited;

using Shithead.Moves;

namespace Shithead.UnitTests.State.ShitheadStateTests.InitTests;

public class UnsetRevealedCardTests : InitTestsBase
{
    [Test]
    public void WhenPlayerUnsetARevealedCard()
    {
        const int HAND_SIZE = 4;
        var deck = CardsDeck.FullShuffledDeck();
        int cardIndex = Random.Next(3);
        int otherRevealedCard = cardIndex switch
        {
            3 => 0,
            _ => cardIndex + 1,
        };

        var testSubject = GetTestSubject(
            [
                new PlayerData(
                    DealHand(deck, HAND_SIZE),
                    new Dictionary<int, Card>
                    {
                        [cardIndex] = deck.Pop(),
                        [otherRevealedCard] = deck.Pop(),
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

        UnsetRevealedCard move = new(cardIndex);

        ValidateValidMove(
            testSubject,
            player,
            move,
            () =>
            {
                player.Hand.Should().BeEquivalentTo(
                    originalHand.Append(originalRevealed[cardIndex]),
                    "the card was added to hand");
                player.RevealedCards.Should().ContainSingle("only one revealed card is left");
                player.RevealedCards.Should().ContainKey(otherRevealedCard)
                    .WhoseValue.Should().Be(
                        originalRevealed[otherRevealedCard],
                        "only the other card remains");
            },
            originalHand,
            originalRevealed);
    }

    [Test]
    public void WhenPlayerUnsetAnEmptyRevealedCard()
    {
        const int HAND_SIZE = 5;

        var deck = CardsDeck.FullShuffledDeck();
        int targetIndex = Random.Next(3);
        int otherIndex = targetIndex switch
        {
            3 => 0,
            _ => targetIndex + 1,
        };

        var testSubject = GetTestSubject(
            [
                new PlayerData(
                    DealHand(deck, HAND_SIZE),
                    new Dictionary<int, Card>
                    {
                        [otherIndex] = deck.Pop(),
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

        UnsetRevealedCard move = new(targetIndex);

        ValidateInvalidMove(testSubject, player, move, originalHand, originalRevealed);
    }

    [Test]
    public void WhenPlayerUnsetARevealedCardIn4thPosition()
    {
        const int HAND_SIZE = 5;

        var deck = CardsDeck.FullShuffledDeck();
        int targetIndex = 4;
        int otherIndex = Random.Next(3);

        var testSubject = GetTestSubject(
            [
                new PlayerData(
                    DealHand(deck, HAND_SIZE),
                    new Dictionary<int, Card>
                    {
                        [otherIndex] = deck.Pop(),
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

        UnsetRevealedCard move = new(targetIndex);

        ValidateInvalidMove(testSubject, player, move, originalHand, originalRevealed);
    }

    [Test]
    public void WhenPlayerUnsetARevealedCardInNegativePosition()
    {
        const int HAND_SIZE = 5;

        var deck = CardsDeck.FullShuffledDeck();
        int targetIndex = -Random.Next(1, 3);
        int otherIndex = Random.Next(3);

        var testSubject = GetTestSubject(
            [
                new PlayerData(
                    DealHand(deck, HAND_SIZE),
                    new Dictionary<int, Card>
                    {
                        [otherIndex] = deck.Pop(),
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

        UnsetRevealedCard move = new(targetIndex);

        ValidateInvalidMove(testSubject, player, move, originalHand, originalRevealed);
    }

    [Test]
    public void WhenPlayerUnsetARevealedCardAfterAcceptingRevealedCards()
    {
        const int HAND_SIZE = 4;
        var deck = CardsDeck.FullShuffledDeck();
        int cardIndex = Random.Next(3);
        int otherRevealedCard = cardIndex switch
        {
            3 => 0,
            _ => cardIndex + 1,
        };

        var testSubject = GetTestSubject(
            [
                new PlayerData(
                    DealHand(deck, HAND_SIZE),
                    new Dictionary<int, Card>
                    {
                        [cardIndex] = deck.Pop(),
                        [otherRevealedCard] = deck.Pop(),
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

        UnsetRevealedCard move = new(cardIndex);

        ValidateInvalidMove(testSubject, player, move, originalHand, originalRevealed);
    }
}
