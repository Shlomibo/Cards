using System;

using AwesomeAssertions;

using Deck;
using Deck.Cards.FrenchSuited;

using Shithead.Moves;

namespace Shithead.UnitTests.State.ShitheadStateTests.GameOnTests;

public class TakeUndercardTests : GameOnTestsBase
{
    [Test]
    public void WhenThereAreRevealedCards()
    {
        var deck = CardsDeck.FullShuffledDeck();
        var testSubject = GetTestSubject(
            [
                DealPlayer(deck,
                    []),
                DealPlayer(deck),
                DealPlayer(deck),
                DealPlayer(deck)],
            [],
            Discard(deck, 5));

        var player = testSubject.PlayerStates[0];
        var originalHand = player.Hand.ToArray();
        var originalRevealed = player.RevealedCards.ToDictionary();
        var originalUndercards = player.Undercards.ToDictionary();

        TakeUndercard move = new(0);

        ValidateInvalidMove(
            testSubject,
            player,
            move,
            originalHand,
            originalRevealed,
            originalUndercards);
    }

    [Test]
    public void WhenThereAreNoUndercardsCards()
    {
        var deck = CardsDeck.FullShuffledDeck();
        var testSubject = GetTestSubject(
            [
                DealPlayer(deck,
                    [],
                    []),
                DealPlayer(deck),
                DealPlayer(deck),
                DealPlayer(deck)],
            [],
            Discard(deck, 5));

        var player = testSubject.PlayerStates[0];
        var originalHand = player.Hand.ToArray();
        var originalRevealed = player.RevealedCards.ToDictionary();
        var originalUndercards = player.Undercards.ToDictionary();

        TakeUndercard move = new(0);

        ValidateInvalidMove(
            testSubject,
            player,
            move,
            originalHand,
            originalRevealed,
            originalUndercards);
    }

    [Test]
    public void WhenTheUndercardIsRevealed()
    {
        var deck = CardsDeck.FullShuffledDeck();
        var testSubject = GetTestSubject(
            [
                DealPlayer(deck,
                    [],
                    [],
                    new()
                    {
                        [0] = new CardFace<Card>(RandomCard(), isRevealed: true),
                        [1] = RandomCard(),
                        [2] = RandomCard(),
                    }),
                DealPlayer(deck),
                DealPlayer(deck),
                DealPlayer(deck)],
            [],
            Discard(deck, 5));

        var player = testSubject.PlayerStates[0];
        var originalHand = player.Hand.ToArray();
        var originalRevealed = player.RevealedCards.ToDictionary();
        var originalUndercards = player.Undercards.ToDictionary();

        TakeUndercard move = new(0);

        ValidateValidMove(
            testSubject,
            player,
            move,
            () =>
            {
                testSubject.TurnsManager.Current.Should().Be(0);
                player.Hand.Should().ContainSingle()
                    .Which.Should().Be(originalUndercards[0].Card);
                player.RevealedCards.Should().BeEmpty();
                player.Undercards.Should().HaveCount(2)
                    .And.NotContainKey(0);
            },
            originalHand,
            originalRevealed,
            originalUndercards);
    }

    [Test]
    public void WhenTheUndercardIsNotRevealed()
    {
        var deck = CardsDeck.FullShuffledDeck();
        var testSubject = GetTestSubject(
            [
                DealPlayer(deck),
                DealPlayer(deck,
                    [],
                    []),
                DealPlayer(deck),
                DealPlayer(deck)],
            [],
            Discard(deck, 5));

        var player = testSubject.PlayerStates[1];
        var originalHand = player.Hand.ToArray();
        var originalRevealed = player.RevealedCards.ToDictionary();
        var originalUndercards = player.Undercards.ToDictionary();

        TakeUndercard move = new(0);

        ValidateInvalidMove(
            testSubject,
            player,
            move,
            originalHand,
            originalRevealed,
            originalUndercards);
    }

    [Test]
    public void WhenTheUndercardIsRevealedNotOnTurn()
    {
        var deck = CardsDeck.FullShuffledDeck();
        var testSubject = GetTestSubject(
            [
                DealPlayer(deck),
                DealPlayer(deck,
                    [],
                    [],
                    new()
                    {
                        [0] = new CardFace<Card>(RandomCard(), isRevealed: true),
                        [1] = RandomCard(),
                        [2] = RandomCard(),
                    }),
                DealPlayer(deck),
                DealPlayer(deck)],
            [],
            Discard(deck, 5));

        var player = testSubject.PlayerStates[1];
        var originalHand = player.Hand.ToArray();
        var originalRevealed = player.RevealedCards.ToDictionary();
        var originalUndercards = player.Undercards.ToDictionary();

        TakeUndercard move = new(0);

        ValidateValidMove(
            testSubject,
            player,
            move,
            () =>
            {
                testSubject.TurnsManager.Current.Should().Be(0);
                player.Hand.Should().ContainSingle()
                    .Which.Should().Be(originalUndercards[0].Card);
                player.RevealedCards.Should().BeEmpty();
                player.Undercards.Should().HaveCount(2)
                    .And.NotContainKey(0);
            },
            originalHand,
            originalRevealed,
            originalUndercards);
    }
}
