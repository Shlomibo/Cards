using System;

using AwesomeAssertions;

using Deck.Cards.FrenchSuited;

using Shithead.Moves;

namespace Shithead.UnitTests.State.ShitheadStateTests.GameOnTests;

public class LeaveGameTests : GameOnTestsBase
{
    [Test]
    public void WhenAnActivePlayerLeaves()
    {
        var deck = CardsDeck.FullShuffledDeck();
        var testSubject = GetTestSubject(
            [
                DealPlayer(deck),
                DealPlayer(deck),
                DealPlayer(deck),
                DealPlayer(deck)],
            [],
            Discard(deck, 5));

        int leavingPlayer = Random.Next(1, 4);
        var player = testSubject.PlayerStates[0];
        var leftPlayer = testSubject.PlayerStates[leavingPlayer];
        var originalHand = player.Hand.ToArray();
        var originalRevealed = player.RevealedCards.ToDictionary();
        var originalUndercards = player.Undercards.ToDictionary();

        LeaveGame move = new(leavingPlayer);

        ValidateValidMove(
            testSubject,
            player,
            move,
            () =>
            {
                testSubject.TurnsManager.Current.Should().Be(0);
                testSubject.PlayersCount.Should().Be(3);
                testSubject.TurnsManager.ActivePlayers.Should().NotContain(leavingPlayer);
                leftPlayer.DidLeaveGame.Should().BeTrue();
            },
            originalHand,
            originalRevealed,
            originalUndercards);
    }

    [Test]
    public void WhenCurrentPlayerLeaves()
    {
        var deck = CardsDeck.FullShuffledDeck();
        var testSubject = GetTestSubject(
            [
                DealPlayer(deck),
                DealPlayer(deck),
                DealPlayer(deck),
                DealPlayer(deck)],
            [],
            Discard(deck, 5));

        var player = testSubject.PlayerStates[0];
        var originalHand = player.Hand.ToArray();
        var originalRevealed = player.RevealedCards.ToDictionary();
        var originalUndercards = player.Undercards.ToDictionary();

        LeaveGame move = new(0);

        ValidateValidMove(
            testSubject,
            player,
            move,
            () =>
            {
                testSubject.TurnsManager.Current.Should().NotBe(0);
                testSubject.PlayersCount.Should().Be(3);
                testSubject.TurnsManager.ActivePlayers.Should().NotContain(0);
                player.DidLeaveGame.Should().BeTrue();
            },
            originalHand,
            originalRevealed,
            originalUndercards);
    }

    [Test]
    public void WhenAPlayerThatLeftLeaves()
    {
        var deck = CardsDeck.FullShuffledDeck();
        var testSubject = GetTestSubject(
            [
                DealPlayer(deck),
                DealPlayer(deck),
                DealPlayer(deck),
                DealPlayer(deck)],
            [],
            Discard(deck, 5));

        int leavingPlayer = Random.Next(1, 4);
        var player = testSubject.PlayerStates[0];
        var originalHand = player.Hand.ToArray();
        var originalRevealed = player.RevealedCards.ToDictionary();
        var originalUndercards = player.Undercards.ToDictionary();

        LeaveGame move = new(leavingPlayer);
        testSubject.PlayMove(move, leavingPlayer);
        testSubject.LastMove = testSubject.LastPlayedMove = null;

        ValidateInvalidMove(
            testSubject,
            player,
            move,
            originalHand,
            originalRevealed,
            originalUndercards);
    }

    [Test]
    public void WhenANonExistentPlayerLeaves()
    {
        var deck = CardsDeck.FullShuffledDeck();
        var testSubject = GetTestSubject(
            [
                DealPlayer(deck),
                DealPlayer(deck),
                DealPlayer(deck),
                DealPlayer(deck)],
            [],
            Discard(deck, 5));

        var player = testSubject.PlayerStates[0];
        var originalHand = player.Hand.ToArray();
        var originalRevealed = player.RevealedCards.ToDictionary();
        var originalUndercards = player.Undercards.ToDictionary();

        LeaveGame move = new(4);

        ValidateInvalidMove(
            testSubject,
            player,
            move,
            originalHand,
            originalRevealed,
            originalUndercards);
    }

    [Test]
    public void WhenANegativePlayerLeaves()
    {
        var deck = CardsDeck.FullShuffledDeck();
        var testSubject = GetTestSubject(
            [
                DealPlayer(deck),
                DealPlayer(deck),
                DealPlayer(deck),
                DealPlayer(deck)],
            [],
            Discard(deck, 5));

        var player = testSubject.PlayerStates[0];
        var originalHand = player.Hand.ToArray();
        var originalRevealed = player.RevealedCards.ToDictionary();
        var originalUndercards = player.Undercards.ToDictionary();

        LeaveGame move = new(-1);

        ValidateInvalidMove(
            testSubject,
            player,
            move,
            originalHand,
            originalRevealed,
            originalUndercards);
    }
}
