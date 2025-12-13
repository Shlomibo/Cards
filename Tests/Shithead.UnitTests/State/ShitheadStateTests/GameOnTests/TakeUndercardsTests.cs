using System;

using AwesomeAssertions;

using Deck;
using Deck.Cards.FrenchSuited;

using Shithead.Moves;

namespace Shithead.UnitTests.State.ShitheadStateTests.GameOnTests;

public class TakeUndercardsTests : GameOnTestsBase
{
    [Test]
    public void WhenThereAreCardsInDeck()
    {
        var card = RandomCard();
        var deck = CardsDeck.FullShuffledDeck();
        var testSubject = GetTestSubject(
            [
                DealPlayer(deck,
                    [card, RandomCard(), RandomCard()],
                    new()
                    {
                        [0] = RandomCard(card.Value),
                        [1] = RandomCard(),
                        [2] = RandomCard(),
                    }),
                DealPlayer(deck),
                DealPlayer(deck),
                DealPlayer(deck)],
            deck,
            Discard(deck, 5));

        var player = testSubject.PlayerStates[0];
        var originalHand = player.Hand.ToArray();
        var originalRevealed = player.RevealedCards.ToDictionary();
        var originalUndercards = player.Undercards.ToDictionary();

        TakeUndercards move = new([0]);

        ValidateInvalidMove(
            testSubject,
            player,
            move,
            originalHand,
            originalRevealed,
            originalUndercards);
    }

    [Test]
    public void WhenThereDifferentCardsInHand()
    {
        var card = RandomCard();
        var deck = CardsDeck.FullShuffledDeck();
        var testSubject = GetTestSubject(
            [
                DealPlayer(deck,
                    [Not(card), Not(card), Not(card)],
                    new()
                    {
                        [0] = RandomCard(card.Value),
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

        TakeUndercards move = new([0]);

        ValidateInvalidMove(
            testSubject,
            player,
            move,
            originalHand,
            originalRevealed,
            originalUndercards);
    }

    [Test]
    public void WhenThereAreCardsInHandAndAllCardsHaveTheSameValueWhichHandContains()
    {
        var card = RandomCard();
        var deck = CardsDeck.FullShuffledDeck();
        var testSubject = GetTestSubject(
            [
                DealPlayer(deck,
                    [RandomCard(card.Value), RandomCard(), RandomCard()],
                    new()
                    {
                        [0] = RandomCard(card.Value),
                        [1] = RandomCard(),
                        [2] = RandomCard(card.Value),
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

        TakeUndercards move = new([0, 2]);

        ValidateInvalidMove(
            testSubject,
            player,
            move,
            originalHand,
            originalRevealed,
            originalUndercards);
    }

    [Test]
    public void WhenThereAreRevealedCardAndAllCardsHaveTheSameValue()
    {
        var card = RandomCard();
        var deck = CardsDeck.FullShuffledDeck();
        var testSubject = GetTestSubject(
            [
                DealPlayer(deck,
                    [],
                    new()
                    {
                        [0] = RandomCard(card.Value),
                        [1] = RandomCard(),
                        [2] = RandomCard(card.Value),
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

        TakeUndercards move = new([0, 2]);

        ValidateValidMove(
            testSubject,
            player,
            move,
            () =>
            {
                testSubject.TurnsManager.Current.Should().Be(0);
                player.Hand.Should().Contain([originalRevealed[0], originalRevealed[2]]);
                player.RevealedCards.Should().ContainSingle().And.ContainKey(1)
                    .WhoseValue.Should().Be(originalRevealed[1]);
                player.Undercards.Should().BeEquivalentTo(originalUndercards);
            },
            originalHand,
            originalRevealed,
            originalUndercards);
    }

    [Test]
    public void WhenThereAreRevealedCardAndAllCardsHaveTheSameValueOutOfTurn()
    {
        var card = RandomCard();
        var deck = CardsDeck.FullShuffledDeck();
        var testSubject = GetTestSubject(
            [
                DealPlayer(deck),
                DealPlayer(deck,
                    [],
                    new()
                    {
                        [0] = RandomCard(card.Value),
                        [1] = RandomCard(),
                        [2] = RandomCard(card.Value),
                    }),
                DealPlayer(deck),
                DealPlayer(deck)],
            [],
            Discard(deck, 5));

        var player = testSubject.PlayerStates[1];
        var originalHand = player.Hand.ToArray();
        var originalRevealed = player.RevealedCards.ToDictionary();
        var originalUndercards = player.Undercards.ToDictionary();

        TakeUndercards move = new([0, 2]);

        ValidateValidMove(
            testSubject,
            player,
            move,
            () =>
            {
                testSubject.TurnsManager.Current.Should().Be(0);
                player.Hand.Should().Contain([originalRevealed[0], originalRevealed[2]]);
                player.RevealedCards.Should().ContainSingle().And.ContainKey(1)
                    .WhoseValue.Should().Be(originalRevealed[1]);
                player.Undercards.Should().BeEquivalentTo(originalUndercards);
            },
            originalHand,
            originalRevealed,
            originalUndercards);
    }

    [Test]
    public void WhenThereAreRevealedCardAndCardsHaveDifferentValue()
    {
        var card = RandomCard();
        var deck = CardsDeck.FullShuffledDeck();
        var testSubject = GetTestSubject(
            [
                DealPlayer(deck,
                    [],
                    new()
                    {
                        [0] = RandomCard(card.Value),
                        [1] = RandomCard(),
                        [2] = Not(card),
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

        TakeUndercards move = new([0, 2]);

        ValidateInvalidMove(
            testSubject,
            player,
            move,
            originalHand,
            originalRevealed,
            originalUndercards);
    }

    [Test]
    public void WhenThereAreRevealedCardAndTakingNothing()
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

        TakeUndercards move = new([]);

        ValidateInvalidMove(
            testSubject,
            player,
            move,
            originalHand,
            originalRevealed,
            originalUndercards);
    }

    [Test]
    public void WhenThereAreRevealedCardAndTakingNonExistingCard()
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

        TakeUndercards move = new([3]);

        ValidateInvalidMove(
            testSubject,
            player,
            move,
            originalHand,
            originalRevealed,
            originalUndercards);
    }

    [Test]
    public void WhenThereAreRevealedCardAndTakingNegativeCard()
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

        TakeUndercards move = new([-1]);

        ValidateInvalidMove(
            testSubject,
            player,
            move,
            originalHand,
            originalRevealed,
            originalUndercards);
    }

    [Test]
    public void WhenThereAreNoRevealedCardAndTakingMoreThanOneCard()
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

        TakeUndercards move = new([0, 1]);

        ValidateInvalidMove(
            testSubject,
            player,
            move,
            originalHand,
            originalRevealed,
            originalUndercards);
    }

    [Test]
    public void WhenThereAreNoRevealedUndercardsCards()
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

        TakeUndercards move = new([0]);

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

        TakeUndercards move = new([0]);

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

        TakeUndercards move = new([0]);

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
