using System;

using AwesomeAssertions;

using Deck;
using Deck.Cards.FrenchSuited;

using Shithead.Moves;

namespace Shithead.UnitTests.State.ShitheadStateTests.GameOnTests;

public class AcceptDiscardPileTests : GameOnTestsBase
{
    [Test]
    public void WhenTheresADiscardPile()
    {
        var deck = CardsDeck.FullShuffledDeck(excludeJokers: true);
        var testSubject = GetTestSubject(
            [
                DealPlayer(deck),
                DealPlayer(deck),
                DealPlayer(deck),
                DealPlayer(deck)],
            deck,
            Discard(deck, 5));

        int otherPlayerId = Random.Next(1, 3);
        var player = testSubject.PlayerStates[0];
        var otherPlayer = testSubject.PlayerStates[otherPlayerId];
        var originalHand = player.Hand.ToArray();
        var originalOtherPlayerHand = otherPlayer.Hand.ToArray();
        var originalDiscard = testSubject.DiscardPile.ToArray();

        AcceptDiscardPile move = new();

        ValidateValidMove(
            testSubject,
            player,
            move,
            () =>
            {
                player.Hand.Should().Contain(originalHand).And.Contain(originalDiscard);
                testSubject.DiscardPile.Should().BeEmpty();
                testSubject.TurnsManager.Current.Should().Be(player.Id + 1);
            },
            originalHand,
            originalDiscardPile: originalDiscard);
    }

    [Test]
    public void WhenDiscardPileIsEmpty()
    {
        var deck = CardsDeck.FullShuffledDeck(excludeJokers: true);
        var testSubject = GetTestSubject(
            [
                DealPlayer(deck),
                DealPlayer(deck),
                DealPlayer(deck),
                DealPlayer(deck)],
            deck,
            []);

        int otherPlayerId = Random.Next(1, 3);
        var player = testSubject.PlayerStates[0];
        var otherPlayer = testSubject.PlayerStates[otherPlayerId];
        var originalHand = player.Hand.ToArray();
        var originalOtherPlayerHand = otherPlayer.Hand.ToArray();
        var originalDiscard = testSubject.DiscardPile.ToArray();

        AcceptDiscardPile move = new();

        ValidateValidMove(
            testSubject,
            player,
            move,
            () =>
            {
                player.Hand.Should().Contain(originalHand);
                testSubject.DiscardPile.Should().BeEmpty();
                testSubject.TurnsManager.Current.Should().Be(player.Id + 1);
            },
            originalHand,
            originalDiscardPile: originalDiscard);
    }

    [Test]
    public void WhenAcceptingPileNotInTurn()
    {
        var deck = CardsDeck.FullShuffledDeck(excludeJokers: true);
        var testSubject = GetTestSubject(
            [
                DealPlayer(deck),
                DealPlayer(deck),
                DealPlayer(deck),
                DealPlayer(deck)],
            deck,
            Discard(deck, 5));

        int otherPlayerId = Random.Next(1, 3);
        var player = testSubject.PlayerStates[1];
        var otherPlayer = testSubject.PlayerStates[otherPlayerId];
        var originalHand = player.Hand.ToArray();
        var originalOtherPlayerHand = otherPlayer.Hand.ToArray();
        var originalDiscard = testSubject.DiscardPile.ToArray();

        AcceptDiscardPile move = new();

        ValidateInvalidMove(
            testSubject,
            player,
            move,
            originalHand,
            originalDiscardPile: originalDiscard);
    }
}
