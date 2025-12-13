using System;

using AwesomeAssertions;

using Deck.Cards.FrenchSuited;

using Shithead.Moves;

namespace Shithead.UnitTests.State.ShitheadStateTests.GameOnTests;

public class PlaceCardTests : GameOnTestsBase
{
    [Test]
    public void WhenPlacingZeroCards()
    {
        var deck = CardsDeck.FullShuffledDeck();
        var testSubject = GetTestSubject(
            [
                DealPlayer(deck),
                DealPlayer(deck),
                DealPlayer(deck),
                DealPlayer(deck)],
            deck,
            Discard(deck, 5));

        var player = testSubject.PlayerStates[0];
        var originalHand = player.Hand.ToArray();
        var originalDiscard = testSubject.DiscardPile.ToArray();

        PlaceCard move = new([]);

        ValidateInvalidMove(
            testSubject,
            player,
            move,
            originalHand,
            originalDiscardPile: originalDiscard);
    }

    [TestCase(true)]
    [TestCase(false)]
    public void WhenPlacingCardsOnNothing(bool shadowedBy3)
    {
        var cardValue = RandomCard();
        var deck = CardsDeck.FullShuffledDeck();
        var testSubject = GetTestSubject(
            [
                DealPlayer(
                    deck,
                    [cardValue, RandomCard(), RandomCard(cardValue.Value)]),
                DealPlayer(deck),
                DealPlayer(deck),
                DealPlayer(deck)],
            deck,
            CreateDiscard(shadowedBy3));

        var player = testSubject.PlayerStates[0];
        var originalHand = player.Hand.ToArray();
        var originalDiscard = testSubject.DiscardPile.ToArray();

        PlaceCard move = new([0, 2]);

        ValidateValidMove(
            testSubject,
            player,
            move,
            () =>
            {
                player.Hand.Should().Contain(originalHand[1]).And.HaveCount(3);
                testSubject.DiscardPile.Should()
                    .BeEquivalentTo([.. originalDiscard, originalHand[0], originalHand[2]]);
                testSubject.TurnsManager.Current.Should().Be(player.Id + 1);
            },
            originalHand,
            originalDiscardPile: originalDiscard);
    }

    [TestCase(true)]
    [TestCase(false)]
    public void WhenPlacingCardsOnSmallerValue(bool shadowedBy3)
    {
        Card cardValue = Not(Value.Four);

        var deck = CardsDeck.FullShuffledDeck();
        var testSubject = GetTestSubject(
            [
                DealPlayer(
                    deck,
                    [cardValue, RandomCard(), RandomCard(cardValue.Value)]),
                DealPlayer(deck),
                DealPlayer(deck),
                DealPlayer(deck)],
            deck,
            CreateDiscard(shadowedBy3, RandomCard(Value.Four)));

        var player = testSubject.PlayerStates[0];
        var originalHand = player.Hand.ToArray();
        var originalDiscard = testSubject.DiscardPile.ToArray();

        PlaceCard move = new([0, 2]);

        ValidateValidMove(
            testSubject,
            player,
            move,
            () =>
            {
                player.Hand.Should().Contain(originalHand[1]).And.HaveCount(3);
                testSubject.DiscardPile.Should()
                    .BeEquivalentTo([.. originalDiscard, originalHand[0], originalHand[2]]);
                testSubject.TurnsManager.Current.Should().Be(player.Id + 1);
            },
            originalHand,
            originalDiscardPile: originalDiscard);
    }

    [TestCase(true)]
    [TestCase(false)]
    public void WhenPlacingCardsOnSameValue(bool shadowedBy3)
    {
        Card cardValue = RandomCard();

        var deck = CardsDeck.FullShuffledDeck();
        var testSubject = GetTestSubject(
            [
                DealPlayer(
                    deck,
                    [cardValue, RandomCard(), RandomCard(cardValue.Value)]),
                DealPlayer(deck),
                DealPlayer(deck),
                DealPlayer(deck)],
            deck,
            CreateDiscard(shadowedBy3, RandomCard(cardValue.Value)));

        var player = testSubject.PlayerStates[0];
        var originalHand = player.Hand.ToArray();
        var originalDiscard = testSubject.DiscardPile.ToArray();

        PlaceCard move = new([0, 2]);

        ValidateValidMove(
            testSubject,
            player,
            move,
            () =>
            {
                player.Hand.Should().Contain(originalHand[1]).And.HaveCount(3);
                testSubject.DiscardPile.Should()
                    .BeEquivalentTo([.. originalDiscard, originalHand[0], originalHand[2]]);
                testSubject.TurnsManager.Current.Should().Be(player.Id + 1);
            },
            originalHand,
            originalDiscardPile: originalDiscard);
    }

    [TestCase(true)]
    [TestCase(false)]
    public void WhenPlacingCardsOfDifferentValuesOnSmallerValue(bool shadowedBy3)
    {
        Card card1 = Not(Value.Four);
        Card card3 = Not(card1.Value, Value.Four);

        var deck = CardsDeck.FullShuffledDeck();
        var testSubject = GetTestSubject(
            [
                DealPlayer(
                    deck,
                    [card1, RandomCard(), card3]),
                DealPlayer(deck),
                DealPlayer(deck),
                DealPlayer(deck)],
            deck,
            CreateDiscard(shadowedBy3, RandomCard(Value.Four)));

        var player = testSubject.PlayerStates[0];
        var originalHand = player.Hand.ToArray();
        var originalDiscard = testSubject.DiscardPile.ToArray();

        PlaceCard move = new([0, 2]);

        ValidateInvalidMove(
            testSubject,
            player,
            move,
            originalHand,
            originalDiscardPile: originalDiscard);
    }

    [TestCase(true)]
    [TestCase(false)]
    public void WhenPlacingCardsOnHigherValue(bool shadowedBy3)
    {
        Card cardValue = Not(Value.Ace);

        var deck = CardsDeck.FullShuffledDeck();
        var testSubject = GetTestSubject(
            [
                DealPlayer(
                    deck,
                    [cardValue, RandomCard(), RandomCard(cardValue.Value)]),
                DealPlayer(deck),
                DealPlayer(deck),
                DealPlayer(deck)],
            deck,
            CreateDiscard(shadowedBy3, RandomCard(Value.Ace)));

        var player = testSubject.PlayerStates[0];
        var originalHand = player.Hand.ToArray();
        var originalDiscard = testSubject.DiscardPile.ToArray();

        PlaceCard move = new([0, 2]);

        ValidateInvalidMove(
            testSubject,
            player,
            move,
            originalHand,
            originalDiscardPile: originalDiscard);
    }

    [TestCase(true)]
    [TestCase(false)]
    public void WhenPlacingCardsOnHigherSeven(bool shadowedBy3)
    {
        Card cardValue = Not(
            Value.Seven,
            Value.Nine,
            Value.Jack,
            Value.Queen,
            Value.King,
            Value.Ace);

        var deck = CardsDeck.FullShuffledDeck();
        var testSubject = GetTestSubject(
            [
                DealPlayer(
                    deck,
                    [cardValue, RandomCard(), RandomCard(cardValue.Value)]),
                DealPlayer(deck),
                DealPlayer(deck),
                DealPlayer(deck)],
            deck,
            CreateDiscard(shadowedBy3, RandomCard(Value.Seven)));

        var player = testSubject.PlayerStates[0];
        var originalHand = player.Hand.ToArray();
        var originalDiscard = testSubject.DiscardPile.ToArray();

        PlaceCard move = new([0, 2]);

        ValidateValidMove(
            testSubject,
            player,
            move,
            () =>
            {
                player.Hand.Should().Contain(originalHand[1]).And.HaveCount(3);
                testSubject.DiscardPile.Should()
                    .BeEquivalentTo([.. originalDiscard, originalHand[0], originalHand[2]]);
                testSubject.TurnsManager.Current.Should().Be(player.Id + 1);
            },
            originalHand,
            originalDiscardPile: originalDiscard);
    }

    [TestCase(true)]
    [TestCase(false)]
    public void WhenPlacingCardsOnLowerSeven(bool shadowedBy3)
    {
        Card cardValue = Not(Value.Four, Value.Five, Value.Six, Value.Seven);

        var deck = CardsDeck.FullShuffledDeck();
        var testSubject = GetTestSubject(
            [
                DealPlayer(
                    deck,
                    [cardValue, RandomCard(), RandomCard(cardValue.Value)]),
                DealPlayer(deck),
                DealPlayer(deck),
                DealPlayer(deck)],
            deck,
            CreateDiscard(shadowedBy3, RandomCard(Value.Seven)));

        var player = testSubject.PlayerStates[0];
        var originalHand = player.Hand.ToArray();
        var originalDiscard = testSubject.DiscardPile.ToArray();

        PlaceCard move = new([0, 2]);

        ValidateInvalidMove(
            testSubject,
            player,
            move,
            originalHand,
            originalDiscardPile: originalDiscard);
    }

    [TestCase(true)]
    [TestCase(false)]
    public void WhenPlacingSevenOnSeven(bool shadowedBy3)
    {
        Card cardValue = RandomCard(Value.Seven);

        var deck = CardsDeck.FullShuffledDeck();
        var testSubject = GetTestSubject(
            [
                DealPlayer(
                    deck,
                    [cardValue, RandomCard(), RandomCard(cardValue.Value)]),
                DealPlayer(deck),
                DealPlayer(deck),
                DealPlayer(deck)],
            deck,
            CreateDiscard(shadowedBy3, RandomCard(Value.Seven)));

        var player = testSubject.PlayerStates[0];
        var originalHand = player.Hand.ToArray();
        var originalDiscard = testSubject.DiscardPile.ToArray();

        PlaceCard move = new([0, 2]);

        ValidateValidMove(
            testSubject,
            player,
            move,
            () =>
            {
                player.Hand.Should().Contain(originalHand[1]).And.HaveCount(3);
                testSubject.DiscardPile.Should()
                    .BeEquivalentTo([.. originalDiscard, originalHand[0], originalHand[2]]);
                testSubject.TurnsManager.Current.Should().Be(player.Id + 1);
            },
            originalHand,
            originalDiscardPile: originalDiscard);
    }

    [TestCase(true)]
    [TestCase(false)]
    public void WhenCompletingASet(bool outOfOrder)
    {
        Card cardValue = RandomCard();

        var deck = CardsDeck.FullShuffledDeck();
        var testSubject = GetTestSubject(
            [
                DealPlayer(
                    deck,
                    [cardValue, RandomCard(), RandomCard(cardValue.Value)]),
                DealPlayer(deck),
                DealPlayer(
                    deck,
                    [cardValue, RandomCard(), RandomCard(cardValue.Value)]),
                DealPlayer(deck)],
            deck,
            [RandomCard(cardValue.Value), RandomCard(cardValue.Value)]);

        int playerId = outOfOrder ? 2 : 0;
        var player = testSubject.PlayerStates[playerId];
        var originalHand = player.Hand.ToArray();
        var originalDiscard = testSubject.DiscardPile.ToArray();

        PlaceCard move = new([0, 2]);

        ValidateValidMove(
            testSubject,
            player,
            move,
            () =>
            {
                player.Hand.Should().Contain(originalHand[1]).And.HaveCount(3);
                testSubject.DiscardPile.Should()
                    .BeEmpty();
                testSubject.TurnsManager.Current.Should().Be(playerId);
            },
            originalHand,
            originalDiscardPile: originalDiscard);
    }

    [TestCase(true)]
    [TestCase(false)]
    public void WhenCompletingASetWith3InBetween(bool outOfOrder)
    {
        Card cardValue = RandomCard();

        var deck = CardsDeck.FullShuffledDeck();
        var testSubject = GetTestSubject(
            [
                DealPlayer(
                    deck,
                    [cardValue, RandomCard(), RandomCard(cardValue.Value)]),
                DealPlayer(deck),
                DealPlayer(
                    deck,
                    [cardValue, RandomCard(), RandomCard(cardValue.Value)]),
                DealPlayer(deck)],
            deck,
            [
                RandomCard(cardValue.Value),
                RandomCard(Value.Three),
                RandomCard(cardValue.Value)]);

        int playerId = outOfOrder ? 2 : 0;
        var player = testSubject.PlayerStates[playerId];
        var originalHand = player.Hand.ToArray();
        var originalDiscard = testSubject.DiscardPile.ToArray();

        PlaceCard move = new([0, 2]);

        if (outOfOrder)
        {
            ValidateInvalidMove(
                testSubject,
                player,
                move,
                originalHand,
                originalDiscardPile: originalDiscard);
        }
        else
        {
            ValidateValidMove(
                testSubject,
                player,
                move,
                () =>
                {
                    player.Hand.Should().Contain(originalHand[1]).And.HaveCount(3);
                    testSubject.DiscardPile.Should()
                        .BeEquivalentTo([.. originalDiscard, originalHand[0], originalHand[2]]);
                    testSubject.TurnsManager.Current.Should().Be(playerId + 1);
                },
                originalHand,
                originalDiscardPile: originalDiscard);
        }
    }

    [TestCase(true)]
    [TestCase(false)]
    public void WhenPlacingAJoker(bool outOfOrder)
    {
        Card cardValue = RandomCard(Value.Joker);

        var deck = CardsDeck.FullShuffledDeck();
        var testSubject = GetTestSubject(
            [
                DealPlayer(
                    deck,
                    [cardValue, RandomCard(), RandomCard(cardValue.Value)]),
                DealPlayer(deck),
                DealPlayer(
                    deck,
                    [cardValue, RandomCard(), RandomCard(cardValue.Value)]),
                DealPlayer(deck)],
            deck,
            [
                RandomCard(cardValue.Value),
                RandomCard(Value.Three),
                RandomCard(cardValue.Value)]);

        int playerId = outOfOrder ? 2 : 0;
        var player = testSubject.PlayerStates[playerId];
        var originalHand = player.Hand.ToArray();
        var originalDiscard = testSubject.DiscardPile.ToArray();

        PlaceCard move = new([0, 2]);

        ValidateInvalidMove(
            testSubject,
            player,
            move,
            originalHand,
            originalDiscardPile: originalDiscard);
    }

    [TestCase(true)]
    [TestCase(false)]
    public void WhenPlacing2s(bool shadowedBy3)
    {
        Card cardValue = RandomCard(Value.Two);

        var deck = CardsDeck.FullShuffledDeck();
        var testSubject = GetTestSubject(
            [
                DealPlayer(
                    deck,
                    [cardValue, RandomCard(), RandomCard(cardValue.Value)]),
                DealPlayer(deck),
                DealPlayer(deck),
                DealPlayer(deck)],
            deck,
            CreateDiscard(shadowedBy3, RandomCard()));

        var player = testSubject.PlayerStates[0];
        var originalHand = player.Hand.ToArray();
        var originalDiscard = testSubject.DiscardPile.ToArray();

        PlaceCard move = new([0, 2]);

        ValidateValidMove(
            testSubject,
            player,
            move,
            () =>
            {
                player.Hand.Should().Contain(originalHand[1]).And.HaveCount(3);
                testSubject.DiscardPile.Should()
                    .BeEquivalentTo([.. originalDiscard, originalHand[0], originalHand[2]]);
                testSubject.TurnsManager.Current.Should().Be(player.Id + 1);
            },
            originalHand,
            originalDiscardPile: originalDiscard);
    }

    [Test]
    public void WhenPlacing3s()
    {
        Card cardValue = RandomCard(Value.Three);

        var deck = CardsDeck.FullShuffledDeck();
        var testSubject = GetTestSubject(
            [
                DealPlayer(
                    deck,
                    [cardValue, RandomCard(), RandomCard(cardValue.Value)]),
                DealPlayer(deck),
                DealPlayer(deck),
                DealPlayer(deck)],
            deck,
            [RandomCard()]);

        var player = testSubject.PlayerStates[0];
        var originalHand = player.Hand.ToArray();
        var originalDiscard = testSubject.DiscardPile.ToArray();

        PlaceCard move = new([0, 2]);

        ValidateValidMove(
            testSubject,
            player,
            move,
            () =>
            {
                player.Hand.Should().Contain(originalHand[1]).And.HaveCount(3);
                testSubject.DiscardPile.Should()
                    .BeEquivalentTo([.. originalDiscard, originalHand[0], originalHand[2]]);
                testSubject.TurnsManager.Current.Should().Be(player.Id + 1);
            },
            originalHand,
            originalDiscardPile: originalDiscard);
    }

    [Test]
    public void WhenCompletingASetOf3s()
    {
        Card cardValue = RandomCard(Value.Three);

        var deck = CardsDeck.FullShuffledDeck();
        var testSubject = GetTestSubject(
            [
                DealPlayer(
                    deck,
                    [cardValue, RandomCard(), RandomCard(cardValue.Value)]),
                DealPlayer(deck),
                DealPlayer(deck),
                DealPlayer(deck)],
            deck,
            [RandomCard(Value.Three), RandomCard(Value.Three)]);

        var player = testSubject.PlayerStates[0];
        var originalHand = player.Hand.ToArray();
        var originalDiscard = testSubject.DiscardPile.ToArray();

        PlaceCard move = new([0, 2]);

        ValidateValidMove(
            testSubject,
            player,
            move,
            () =>
            {
                player.Hand.Should().Contain(originalHand[1]).And.HaveCount(3);
                testSubject.DiscardPile.Should()
                    .BeEmpty();
                testSubject.TurnsManager.Current.Should().Be(player.Id);
            },
            originalHand,
            originalDiscardPile: originalDiscard);
    }

    [TestCase(true)]
    [TestCase(false)]
    public void WhenPlacingAn8(bool shadowedBy3)
    {
        Card cardValue = RandomCard(Value.Eight);

        var deck = CardsDeck.FullShuffledDeck();
        var testSubject = GetTestSubject(
            [
                DealPlayer(
                    deck,
                    [RandomCard(), cardValue, RandomCard(cardValue.Value)]),
                DealPlayer(deck),
                DealPlayer(deck),
                DealPlayer(deck)],
            deck,
            []);

        var player = testSubject.PlayerStates[0];
        var originalHand = player.Hand.ToArray();
        var originalDiscard = testSubject.DiscardPile.ToArray();

        PlaceCard move = new([1]);

        ValidateValidMove(
            testSubject,
            player,
            move,
            () =>
            {
                player.Hand.Should().Contain(originalHand[0])
                    .And.Contain(originalHand[2]).And.HaveCount(3);
                testSubject.DiscardPile.Should()
                    .BeEquivalentTo([.. originalDiscard, originalHand[1]]);
                testSubject.TurnsManager.Current.Should().Be(player.Id + 2);
            },
            originalHand,
            originalDiscardPile: originalDiscard);
    }

    [TestCase(true)]
    [TestCase(false)]
    public void WhenPlacingAn8OnAHigherValue(bool shadowedBy3)
    {
        Card cardValue = RandomCard(Value.Eight);

        var deck = CardsDeck.FullShuffledDeck();
        var testSubject = GetTestSubject(
            [
                DealPlayer(
                    deck,
                    [RandomCard(), cardValue, RandomCard(cardValue.Value)]),
                DealPlayer(deck),
                DealPlayer(deck),
                DealPlayer(deck)],
            deck,
            [Not(Value.Six, Value.Five, Value.Four)]);

        var player = testSubject.PlayerStates[0];
        var originalHand = player.Hand.ToArray();
        var originalDiscard = testSubject.DiscardPile.ToArray();

        PlaceCard move = new([1]);

        ValidateInvalidMove(
            testSubject,
            player,
            move,
            originalHand,
            originalDiscardPile: originalDiscard);
    }

    [Test]
    public void WhenPlacingMany8s()
    {
        Card cardValue = RandomCard(Value.Eight);

        var deck = CardsDeck.FullShuffledDeck();
        var testSubject = GetTestSubject(
            [
                DealPlayer(
                    deck,
                    [cardValue, RandomCard(), RandomCard(cardValue.Value)]),
                DealPlayer(deck),
                DealPlayer(deck),
                DealPlayer(deck)],
            deck,
            []);

        var player = testSubject.PlayerStates[0];
        var originalHand = player.Hand.ToArray();
        var originalDiscard = testSubject.DiscardPile.ToArray();

        PlaceCard move = new([0, 2]);

        ValidateValidMove(
            testSubject,
            player,
            move,
            () =>
            {
                player.Hand.Should().Contain(originalHand[1]).And.HaveCount(3);
                testSubject.DiscardPile.Should()
                    .BeEquivalentTo([.. originalDiscard, originalHand[0], originalHand[2]]);
                testSubject.TurnsManager.Current.Should().Be(player.Id + 3);
            },
            originalHand,
            originalDiscardPile: originalDiscard);
    }

    [Test]
    public void WhenPlacingAsEightsAsPlayers()
    {
        Card cardValue = RandomCard(Value.Eight);

        var deck = CardsDeck.FullShuffledDeck();
        var testSubject = GetTestSubject(
            [
                DealPlayer(
                    deck,
                    [cardValue, RandomCard(cardValue.Value), RandomCard(cardValue.Value)]),
                DealPlayer(deck),
                DealPlayer(deck)],
            deck,
            []);

        var player = testSubject.PlayerStates[0];
        var originalHand = player.Hand.ToArray();
        var originalDiscard = testSubject.DiscardPile.ToArray();

        PlaceCard move = new([0, 1, 2]);

        ValidateValidMove(
            testSubject,
            player,
            move,
            () =>
            {
                player.Hand.Should().HaveCount(3);
                testSubject.DiscardPile.Should()
                    .BeEquivalentTo([
                        .. originalDiscard,
                        originalHand[0],
                        originalHand[1],
                        originalHand[2]]);
                testSubject.TurnsManager.Current.Should().Be(player.Id + 2);
            },
            originalHand,
            originalDiscardPile: originalDiscard);
    }

    [Test]
    public void WhenCompletingASetOf8s()
    {
        Card cardValue = RandomCard(Value.Eight);

        var deck = CardsDeck.FullShuffledDeck();
        var testSubject = GetTestSubject(
            [
                DealPlayer(
                    deck,
                    [cardValue, RandomCard(cardValue.Value), RandomCard(cardValue.Value)]),
                DealPlayer(deck),
                DealPlayer(deck)],
            deck,
            [RandomCard(cardValue.Value)]);

        var player = testSubject.PlayerStates[0];
        var originalHand = player.Hand.ToArray();
        var originalDiscard = testSubject.DiscardPile.ToArray();

        PlaceCard move = new([0, 1, 2]);

        ValidateValidMove(
            testSubject,
            player,
            move,
            () =>
            {
                player.Hand.Should().HaveCount(3);
                testSubject.DiscardPile.Should().BeEmpty();
                testSubject.TurnsManager.Current.Should().Be(player.Id);
            },
            originalHand,
            originalDiscardPile: originalDiscard);
    }

    [Test]
    public void WhenPlacingMoreEightsThanPlayers()
    {
        Card cardValue = RandomCard(Value.Eight);

        var deck = CardsDeck.FullShuffledDeck();
        var testSubject = GetTestSubject(
            [
                DealPlayer(
                    deck,
                    [cardValue, RandomCard(cardValue.Value), RandomCard(cardValue.Value)]),
                DealPlayer(deck)],
            deck,
            []);

        var player = testSubject.PlayerStates[0];
        var originalHand = player.Hand.ToArray();
        var originalDiscard = testSubject.DiscardPile.ToArray();

        PlaceCard move = new([0, 1, 2]);

        ValidateValidMove(
            testSubject,
            player,
            move,
            () =>
            {
                player.Hand.Should().HaveCount(3);
                testSubject.DiscardPile.Should()
                    .BeEquivalentTo([.. originalDiscard, originalHand[0], originalHand[1], originalHand[2]]);
                testSubject.TurnsManager.Current.Should().Be(player.Id);
            },
            originalHand,
            originalDiscardPile: originalDiscard);
    }

    [Test]
    public void WhenPlacing10s()
    {
        Card cardValue = RandomCard(Value.Ten);

        var deck = CardsDeck.FullShuffledDeck();
        var testSubject = GetTestSubject(
            [
                DealPlayer(
                    deck,
                    [cardValue, RandomCard(), RandomCard(cardValue.Value)]),
                DealPlayer(deck),
                DealPlayer(deck),
                DealPlayer(deck)],
            deck,
            Discard(deck, 5));

        var player = testSubject.PlayerStates[0];
        var originalHand = player.Hand.ToArray();
        var originalDiscard = testSubject.DiscardPile.ToArray();

        PlaceCard move = new([0, 2]);

        ValidateValidMove(
            testSubject,
            player,
            move,
            () =>
            {
                player.Hand.Should().Contain(originalHand[1]).And.HaveCount(3);
                testSubject.DiscardPile.Should()
                    .BeEmpty();
                testSubject.TurnsManager.Current.Should().Be(player.Id);
            },
            originalHand,
            originalDiscardPile: originalDiscard);
    }

    [Test]
    public void WhenPlayersHandIsEmpty()
    {
        var deck = CardsDeck.FullShuffledDeck();
        var testSubject = GetTestSubject(
            [
                DealPlayer(
                    deck,
                    []),
                DealPlayer(deck),
                DealPlayer(deck),
                DealPlayer(deck)],
            [],
            [RandomCard(Value.Four)]);

        var player = testSubject.PlayerStates[0];
        var originalHand = player.Hand.ToArray();
        var originalRevealed = player.RevealedCards.ToDictionary();
        var originalDiscard = testSubject.DiscardPile.ToArray();

        PlaceCard move = new([1]);

        ValidateInvalidMove(
            testSubject,
            player,
            move,
            originalHand,
            originalRevealed,
            originalDiscardPile: originalDiscard);
    }

    [Test]
    public void WhenPlacingNegativeIndexCard()
    {
        var deck = CardsDeck.FullShuffledDeck();
        var testSubject = GetTestSubject(
            [
                DealPlayer(deck),
                DealPlayer(deck),
                DealPlayer(deck),
                DealPlayer(deck)],
            deck,
            []);

        var player = testSubject.PlayerStates[0];
        var originalHand = player.Hand.ToArray();
        var originalDiscard = testSubject.DiscardPile.ToArray();

        PlaceCard move = new([-1]);

        ValidateInvalidMove(
            testSubject,
            player,
            move,
            originalHand,
            originalDiscardPile: originalDiscard);
    }

    [Test]
    public void WhenPlacingCardNotInHand()
    {
        var deck = CardsDeck.FullShuffledDeck();
        var testSubject = GetTestSubject(
            [
                DealPlayer(deck),
                DealPlayer(deck),
                DealPlayer(deck),
                DealPlayer(deck)],
            deck,
            []);

        var player = testSubject.PlayerStates[0];
        var originalHand = player.Hand.ToArray();
        var originalDiscard = testSubject.DiscardPile.ToArray();

        PlaceCard move = new([-1]);

        ValidateInvalidMove(
            testSubject,
            player,
            move,
            originalHand,
            originalDiscardPile: originalDiscard);
    }

    [Test]
    public void WhenPlacingCardAndHandIsFull()
    {
        var deck = CardsDeck.FullShuffledDeck();
        var testSubject = GetTestSubject(
            [
                DealPlayer(
                    deck,
                    DealHand(deck, 5)),
                DealPlayer(deck),
                DealPlayer(deck),
                DealPlayer(deck)],
            deck,
            []);

        var player = testSubject.PlayerStates[0];
        var originalHand = player.Hand.ToArray();
        var originalDiscard = testSubject.DiscardPile.ToArray();

        PlaceCard move = new([0]);

        ValidateValidMove(
            testSubject,
            player,
            move,
            () =>
            {
                player.Hand.Should().HaveCount(4);
            },
            originalHand,
            originalDiscardPile: originalDiscard);
    }

    [Test]
    public void WhenPlacingCardAndHandIsNotFull()
    {
        var deck = CardsDeck.FullShuffledDeck();
        var testSubject = GetTestSubject(
            [
                DealPlayer(
                    deck,
                    DealHand(deck)),
                DealPlayer(deck),
                DealPlayer(deck),
                DealPlayer(deck)],
            deck,
            []);

        var player = testSubject.PlayerStates[0];
        var originalHand = player.Hand.ToArray();
        var originalDiscard = testSubject.DiscardPile.ToArray();

        PlaceCard move = new([0]);

        ValidateValidMove(
            testSubject,
            player,
            move,
            () =>
            {
                player.Hand.Should().HaveCount(3);
            },
            originalHand,
            originalDiscardPile: originalDiscard);
    }

    [Test]
    public void WhenCompletingASetNotInTurn()
    {
        var cardValue = RandomCard();
        var deck = CardsDeck.FullShuffledDeck();
        var testSubject = GetTestSubject(
            [
                DealPlayer(deck),
                DealPlayer(deck),
                DealPlayer(
                    deck,
                    [
                        cardValue,
                        RandomCard(cardValue.Value),
                        RandomCard(cardValue.Value),
                        RandomCard(cardValue.Value)]),
                DealPlayer(deck)],
            deck,
            [RandomCard(Value.Four)]);

        var player = testSubject.PlayerStates[2];
        var originalHand = player.Hand.ToArray();
        var originalDiscard = testSubject.DiscardPile.ToArray();

        PlaceCard move = new([0, 1, 2, 3]);

        ValidateValidMove(
            testSubject,
            player,
            move,
            () =>
            {
                testSubject.DiscardPile.Should().BeEmpty();
                testSubject.TurnsManager.Current.Should().Be(player.Id);
            },
            originalHand,
            originalDiscardPile: originalDiscard);
    }

    [Test]
    public void WhenTryingToCompletingASetNotInTurnOnTopOfHigherValue()
    {
        var cardValue = RandomCard();
        var deck = CardsDeck.FullShuffledDeck();
        var testSubject = GetTestSubject(
            [
                DealPlayer(deck),
                DealPlayer(deck),
                DealPlayer(
                    deck,
                    [
                        cardValue,
                        RandomCard(cardValue.Value),
                        RandomCard(cardValue.Value),
                        RandomCard(cardValue.Value)]),
                DealPlayer(deck)],
            deck,
            [RandomCard(Value.Four)]);

        var player = testSubject.PlayerStates[2];
        var originalHand = player.Hand.ToArray();
        var originalDiscard = testSubject.DiscardPile.ToArray();

        PlaceCard move = new([0, 1, 3, 4]);

        ValidateInvalidMove(
            testSubject,
            player,
            move,
            originalHand,
            originalDiscardPile: originalDiscard);
    }

    private static IEnumerable<Card> CreateDiscard(bool shadowedBy3, params IEnumerable<Card>? cards) =>
        (shadowedBy3, cards) switch
        {
            (false, _) => cards ?? [],
            (true, null) => [RandomCard(Value.Three)],
            _ => [.. cards, RandomCard(Value.Three)]
        };
}
