using System;

using AwesomeAssertions;

using Deck;
using Deck.Cards.FrenchSuited;

using Shithead.Moves;

namespace Shithead.UnitTests.State.ShitheadStateTests.GameOnTests;

public class RevealUndercardTests : GameOnTestsBase
{
    [Test]
    public void WhenThereAreCardsInHand()
    {
        var deck = CardsDeck.FullShuffledDeck();
        var testSubject = GetTestSubject(
            [
                DealPlayer(deck,
                    DealHand(deck),
                    []),
                DealPlayer(deck),
                DealPlayer(deck),
                DealPlayer(deck)],
            [],
            Discard(deck, 5));

        var player = testSubject.PlayerStates[0];
        var originalHand = player.Hand.ToArray();
        var originalUndercards = player.Undercards.ToDictionary();

        RevealUndercard move = new(1);

        ValidateInvalidMove(
            testSubject,
            player,
            move,
            originalHand,
            originalUndercards: originalUndercards);
    }

    [Test]
    public void WhenThereAreRevealedCard()
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
        var originalUndercards = player.Undercards.ToDictionary();

        RevealUndercard move = new(1);

        ValidateInvalidMove(
            testSubject,
            player,
            move,
            originalHand,
            originalUndercards: originalUndercards);
    }

    [Test]
    public void WhenThereAreOtherRevealedUndercards()
    {
        var deck = CardsDeck.FullShuffledDeck();
        var testSubject = GetTestSubject(
            [
                DealPlayer(deck,
                    [],
                    [],
                    new(){
                        [0] = new CardFace<Card>(RandomCard(), isRevealed: true),
                        [1] = new CardFace<Card>(RandomCard(), isRevealed: false),
                    }),
                DealPlayer(deck),
                DealPlayer(deck),
                DealPlayer(deck)],
            [],
            Discard(deck, 5));

        var player = testSubject.PlayerStates[0];
        var originalHand = player.Hand.ToArray();
        var originalUndercards = player.Undercards.ToDictionary();

        RevealUndercard move = new(1);

        ValidateInvalidMove(
            testSubject,
            player,
            move,
            originalHand,
            originalUndercards: originalUndercards);
    }

    [Test]
    public void WhenTheCardIsAlreadyRevealed()
    {
        var deck = CardsDeck.FullShuffledDeck();
        var testSubject = GetTestSubject(
            [
                DealPlayer(deck,
                    [],
                    [],
                    new(){
                        [0] = new CardFace<Card>(RandomCard(), isRevealed: false),
                        [1] = new CardFace<Card>(RandomCard(), isRevealed: true),
                    }),
                DealPlayer(deck),
                DealPlayer(deck),
                DealPlayer(deck)],
            [],
            Discard(deck, 5));

        var player = testSubject.PlayerStates[0];
        var originalHand = player.Hand.ToArray();
        var originalUndercards = player.Undercards.ToDictionary();

        RevealUndercard move = new(1);

        ValidateInvalidMove(
            testSubject,
            player,
            move,
            originalHand,
            originalUndercards: originalUndercards);
    }

    [Test]
    public void WhenTheCardIsNotRevealed()
    {
        var deck = CardsDeck.FullShuffledDeck();
        var testSubject = GetTestSubject(
            [
                DealPlayer(deck,
                    [],
                    [],
                    new(){
                        [0] = new CardFace<Card>(RandomCard(), isRevealed: false),
                        [1] = new CardFace<Card>(RandomCard(), isRevealed: false),
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

        RevealUndercard move = new(1);

        ValidateValidMove(
            testSubject,
            player,
            move,
            () =>
            {
                testSubject.TurnsManager.Current.Should().Be(0);
                player.Hand.Should().BeEmpty();
                player.RevealedCards.Should().BeEmpty();
                player.Undercards.Should().HaveCount(2)
                    .And.ContainKey(1).WhoseValue.Should()
                        .BeEquivalentTo(new CardFace<Card>(originalUndercards[1].Card, true));
            },
            originalHand,
            originalRevealed,
            originalUndercards: originalUndercards);
    }

    [Test]
    public void WhenTheCardIsNotRevealedButNotInTurn()
    {
        var deck = CardsDeck.FullShuffledDeck();
        var testSubject = GetTestSubject(
            [
                DealPlayer(deck,
                    [],
                    [],
                    new(){
                        [0] = new CardFace<Card>(RandomCard(), isRevealed: false),
                        [1] = new CardFace<Card>(RandomCard(), isRevealed: false),
                    }),
                DealPlayer(deck),
                DealPlayer(deck),
                DealPlayer(deck)],
            [],
            Discard(deck, 5));

        var player = testSubject.PlayerStates[1];
        var originalHand = player.Hand.ToArray();
        var originalRevealed = player.RevealedCards.ToDictionary();
        var originalUndercards = player.Undercards.ToDictionary();

        RevealUndercard move = new(1);

        ValidateInvalidMove(
            testSubject,
            player,
            move,
            originalHand,
            originalRevealed,
            originalUndercards: originalUndercards);
    }

    [Test]
    public void WhenThereIsNoCardAtIndex()
    {
        var deck = CardsDeck.FullShuffledDeck();
        var testSubject = GetTestSubject(
            [
                DealPlayer(deck,
                    [],
                    [],
                    new(){
                        [0] = new CardFace<Card>(RandomCard(), isRevealed: false),
                        [1] = new CardFace<Card>(RandomCard(), isRevealed: false),
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

        RevealUndercard move = new(2);

        ValidateInvalidMove(
            testSubject,
            player,
            move,
            originalHand,
            originalRevealed,
            originalUndercards: originalUndercards);
    }

    [Test]
    public void WhenTheIndexIsNegative()
    {
        var deck = CardsDeck.FullShuffledDeck();
        var testSubject = GetTestSubject(
            [
                DealPlayer(deck,
                    [],
                    [],
                    new(){
                        [0] = new CardFace<Card>(RandomCard(), isRevealed: false),
                        [1] = new CardFace<Card>(RandomCard(), isRevealed: false),
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

        RevealUndercard move = new(-1);

        ValidateInvalidMove(
            testSubject,
            player,
            move,
            originalHand,
            originalRevealed,
            originalUndercards: originalUndercards);
    }
}
