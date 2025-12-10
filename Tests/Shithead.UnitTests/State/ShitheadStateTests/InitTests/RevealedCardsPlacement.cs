using System;

using AwesomeAssertions;

using Deck;
using Deck.Cards.FrenchSuited;

using Shithead.Moves;

namespace Shithead.UnitTests.State.ShitheadStateTests.InitTests;

internal class RevealedCardsPlacement : InitTestsBase
{
    [Test]
    public void WhenPlayerPutsARevealedCard()
    {
        var deck = CardsDeck.FullShuffledDeck();

        var testSubject = GetTestSubject(
            [
                new PlayerData(
                    DealHand(deck, 6),
                    RevealedCards: [],
                    DealUndercards(deck)),
                new PlayerData(
                    DealHand(deck, 6),
                    RevealedCards: [],
                    DealUndercards(deck))],
            deck);

        var player = testSubject.PlayerStates[0];
        var originalHand = player.Hand.ToArray();
        int cardIndex = Random.Next(originalHand.Length);
        int targetIndex = Random.Next(3);

        RevealedCardSelection move = new(cardIndex, targetIndex);

        testSubject.IsValidMove(move, player.Id).Should().BeTrue("the move is valid");
        player.Hand.Should().BeEquivalentTo(originalHand, "move not played yet");
        player.RevealedCards.Should().BeEmpty();

        testSubject.PlayMove(move, player.Id).Should().BeTrue("the played move is valid");
        player.Hand.Should().BeEquivalentTo(
            originalHand.Where((_, i) => i != cardIndex),
            "the card was removed from hand");
        player.RevealedCards.Should().ContainKey(targetIndex)
            .WhoseValue.Should().Be(originalHand[cardIndex], "card is placed in revealed cards");

        testSubject.LastMove.Should().BeEquivalentTo((move, player.Id));
        testSubject.LastPlayedMove.Should().BeEquivalentTo((move, player.Id));
    }

    [Test]
    public void WhenPlayerPutsARevealedCardOnTopOfOther()
    {
        const int HAND_SIZE = 5;

        var deck = CardsDeck.FullShuffledDeck();
        int cardIndex = Random.Next(HAND_SIZE);
        int targetIndex = Random.Next(3);

        var testSubject = GetTestSubject(
            [
                new PlayerData(
                    DealHand(deck, HAND_SIZE),
                    new Dictionary<int, Card>
                    {
                        [targetIndex] = deck.Pop(),
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

        RevealedCardSelection move = new(cardIndex, targetIndex);

        testSubject.IsValidMove(move, player.Id).Should().BeFalse("the move is invalid");
        player.Hand.Should().BeEquivalentTo(originalHand, "move not played yet");
        player.RevealedCards.Should().BeEquivalentTo(originalRevealed);

        testSubject.PlayMove(move, player.Id).Should().BeFalse("the played move is invalid");
        player.Hand.Should().BeEquivalentTo(originalHand, "move not valid to play");
        player.RevealedCards.Should().BeEquivalentTo(originalRevealed);

        testSubject.LastMove.Should().BeEquivalentTo((move, player.Id));
        testSubject.LastPlayedMove.Should().BeNull();
    }

    [Test]
    public void WhenPlayerPutsARevealedCardIn4thPosition()
    {
        var deck = CardsDeck.FullShuffledDeck();

        var testSubject = GetTestSubject(
            [
                new PlayerData(
                    DealHand(deck, 6),
                    RevealedCards: [],
                    DealUndercards(deck)),
                new PlayerData(
                    DealHand(deck, 6),
                    RevealedCards: [],
                    DealUndercards(deck))],
            deck);

        var player = testSubject.PlayerStates[0];
        var originalHand = player.Hand.ToArray();
        var originalRevealed = player.RevealedCards.ToDictionary();
        int cardIndex = Random.Next(originalHand.Length);
        int targetIndex = 4;

        RevealedCardSelection move = new(cardIndex, targetIndex);

        testSubject.IsValidMove(move, player.Id).Should().BeFalse("the move is invalid");
        player.Hand.Should().BeEquivalentTo(originalHand, "move not played yet");
        player.RevealedCards.Should().BeEmpty();

        testSubject.PlayMove(move, player.Id).Should().BeFalse("the played move is invalid");
        player.Hand.Should().BeEquivalentTo(originalHand, "move not valid to play");
        player.RevealedCards.Should().BeEquivalentTo(originalRevealed);

        testSubject.LastMove.Should().BeEquivalentTo((move, player.Id));
        testSubject.LastPlayedMove.Should().BeNull();
    }

    [Test]
    public void WhenPlayerPutsARevealedCardInNegativePosition()
    {
        var deck = CardsDeck.FullShuffledDeck();

        var testSubject = GetTestSubject(
            [
                new PlayerData(
                    DealHand(deck, 6),
                    RevealedCards: [],
                    DealUndercards(deck)),
                new PlayerData(
                    DealHand(deck, 6),
                    RevealedCards: [],
                    DealUndercards(deck))],
            deck);

        var player = testSubject.PlayerStates[0];
        var originalHand = player.Hand.ToArray();
        var originalRevealed = player.RevealedCards.ToDictionary();
        int cardIndex = Random.Next(originalHand.Length);
        int targetIndex = -1;

        RevealedCardSelection move = new(cardIndex, targetIndex);

        testSubject.IsValidMove(move, player.Id).Should().BeFalse("the move is invalid");
        player.Hand.Should().BeEquivalentTo(originalHand, "move not played yet");
        player.RevealedCards.Should().BeEmpty();

        testSubject.PlayMove(move, player.Id).Should().BeFalse("the played move is invalid");
        player.Hand.Should().BeEquivalentTo(originalHand, "move not valid to play");
        player.RevealedCards.Should().BeEquivalentTo(originalRevealed);

        testSubject.LastMove.Should().BeEquivalentTo((move, player.Id));
        testSubject.LastPlayedMove.Should().BeNull();
    }

    [Test]
    public void WhenPlayerPutsARevealedCardFromNegativeIndex()
    {
        var deck = CardsDeck.FullShuffledDeck();

        var testSubject = GetTestSubject(
            [
                new PlayerData(
                    DealHand(deck, 6),
                    RevealedCards: [],
                    DealUndercards(deck)),
                new PlayerData(
                    DealHand(deck, 6),
                    RevealedCards: [],
                    DealUndercards(deck))],
            deck);

        var player = testSubject.PlayerStates[0];
        var originalHand = player.Hand.ToArray();
        var originalRevealed = player.RevealedCards.ToDictionary();
        int cardIndex = -Random.Next(1, originalHand.Length);
        int targetIndex = Random.Next(3);

        RevealedCardSelection move = new(cardIndex, targetIndex);

        testSubject.IsValidMove(move, player.Id).Should().BeFalse("the move is invalid");
        player.Hand.Should().BeEquivalentTo(originalHand, "move not played yet");
        player.RevealedCards.Should().BeEmpty();

        testSubject.PlayMove(move, player.Id).Should().BeFalse("the played move is invalid");
        player.Hand.Should().BeEquivalentTo(originalHand, "move not valid to play");
        player.RevealedCards.Should().BeEquivalentTo(originalRevealed);

        testSubject.LastMove.Should().BeEquivalentTo((move, player.Id));
        testSubject.LastPlayedMove.Should().BeNull();
    }

    [Test]
    public void WhenPlayerPutsARevealedCardFromBeyondTheHand()
    {
        var deck = CardsDeck.FullShuffledDeck();

        var testSubject = GetTestSubject(
            [
                new PlayerData(
                    DealHand(deck, 6),
                    RevealedCards: [],
                    DealUndercards(deck)),
                new PlayerData(
                    DealHand(deck, 6),
                    RevealedCards: [],
                    DealUndercards(deck))],
            deck);

        var player = testSubject.PlayerStates[0];
        var originalHand = player.Hand.ToArray();
        var originalRevealed = player.RevealedCards.ToDictionary();
        int cardIndex = originalHand.Length;
        int targetIndex = Random.Next(3);

        RevealedCardSelection move = new(cardIndex, targetIndex);

        testSubject.IsValidMove(move, player.Id).Should().BeFalse("the move is invalid");
        player.Hand.Should().BeEquivalentTo(originalHand, "move not played yet");
        player.RevealedCards.Should().BeEmpty();

        testSubject.PlayMove(move, player.Id).Should().BeFalse("the played move is invalid");
        player.Hand.Should().BeEquivalentTo(originalHand, "move not valid to play");
        player.RevealedCards.Should().BeEquivalentTo(originalRevealed);

        testSubject.LastMove.Should().BeEquivalentTo((move, player.Id));
        testSubject.LastPlayedMove.Should().BeNull();
    }

    [Test]
    public void WhenPlayerPutsARevealedCardAfterAcceptingRevealedCards()
    {
        var deck = CardsDeck.FullShuffledDeck();

        var testSubject = GetTestSubject(
            [
                new PlayerData(
                    DealHand(deck, 6),
                    RevealedCards: [],
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
        int cardIndex = Random.Next(originalHand.Length);
        int targetIndex = Random.Next(3);

        RevealedCardSelection move = new(cardIndex, targetIndex);

        testSubject.IsValidMove(move, player.Id).Should().BeFalse("the move is invalid");
        player.Hand.Should().BeEquivalentTo(originalHand, "move not played yet");
        player.RevealedCards.Should().BeEmpty();

        testSubject.PlayMove(move, player.Id).Should().BeFalse("the played move is invalid");
        player.Hand.Should().BeEquivalentTo(originalHand, "move not valid to play");
        player.RevealedCards.Should().BeEquivalentTo(originalRevealed);

        testSubject.LastMove.Should().BeEquivalentTo((move, player.Id));
        testSubject.LastPlayedMove.Should().BeNull();
    }
}
