using System;

using AwesomeAssertions;

using Deck;
using Deck.Cards.FrenchSuited;

using Shithead.Moves;

namespace Shithead.UnitTests.State.ShitheadStateTests.GameOnTests;

public class PlaceJokerTests : GameOnTestsBase
{
    [Test]
    public void WhenPlayerHasCardsInHandAndHasAJoker()
    {
        var joker = RandomCard(Value.Joker);

        var deck = CardsDeck.FullShuffledDeck(excludeJokers: true);
        var testSubject = GetTestSubject(
            [
                DealPlayer(deck,
                    [joker, RandomCard(), RandomCard()]),
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

        PlaceJoker move = new(otherPlayerId);

        ValidateValidMove(
            testSubject,
            player,
            move,
            () =>
            {
                player.Hand.Should().NotContain(joker).And.HaveCount(3);
                testSubject.DiscardPile.Should().BeEmpty();
                otherPlayer.Hand.Should().Contain(originalDiscard)
                    .And.Contain(originalOtherPlayerHand);
                testSubject.TurnsManager.Current.Should().Be(otherPlayerId);
            },
            originalHand,
            originalDiscardPile: originalDiscard);
    }

    [Test]
    public void WhenPlayerHasCardsInHandAndHasNoJoker()
    {
        var deck = CardsDeck.FullShuffledDeck(excludeJokers: true);
        var testSubject = GetTestSubject(
            [
                DealPlayer(deck,
                    [RandomCard(), RandomCard(), RandomCard()]),
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

        PlaceJoker move = new(otherPlayerId);

        ValidateInvalidMove(
            testSubject,
            player,
            move,
            originalHand,
            originalDiscardPile: originalDiscard);
    }

    [Test]
    public void WhenPlayerHasCardsInHandAndMoreThanOneJoker()
    {
        var joker1 = Card.GetJoker(Color.Black);
        var joker2 = Card.GetJoker(Color.Red);

        var deck = CardsDeck.FullShuffledDeck(excludeJokers: true);
        var testSubject = GetTestSubject(
            [
                DealPlayer(deck,
                    [joker1, RandomCard(), joker2]),
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

        PlaceJoker move = new(otherPlayerId);

        ValidateValidMove(
            testSubject,
            player,
            move,
            () =>
            {
                player.Hand.Should().NotContain(joker1)
                    .And.Contain(joker2).And.HaveCount(3);
                testSubject.DiscardPile.Should().BeEmpty();
                otherPlayer.Hand.Should().Contain(originalDiscard)
                    .And.Contain(originalOtherPlayerHand);
                testSubject.TurnsManager.Current.Should().Be(otherPlayerId);
            },
            originalHand,
            originalDiscardPile: originalDiscard);
    }

    [Test]
    public void WhenPlayerHasNoCardsInHandAndHasAJokerInRevealedCards()
    {
        var joker = RandomCard(Value.Joker);

        var deck = CardsDeck.FullShuffledDeck(excludeJokers: true);
        var testSubject = GetTestSubject(
            [
                DealPlayer(deck,
                    [],
                    new(){
                        [0] = joker,
                        [2] = RandomCard(),
                    }),
                DealPlayer(deck),
                DealPlayer(deck),
                DealPlayer(deck)],
            [],
            Discard(deck, 5));

        int otherPlayerId = Random.Next(1, 3);
        var player = testSubject.PlayerStates[0];
        var otherPlayer = testSubject.PlayerStates[otherPlayerId];
        var originalRevealed = player.RevealedCards.ToDictionary();
        var originalOtherPlayerHand = otherPlayer.Hand.ToArray();
        var originalDiscard = testSubject.DiscardPile.ToArray();

        PlaceJoker move = new(otherPlayerId);

        ValidateValidMove(
            testSubject,
            player,
            move,
            () =>
            {
                player.Hand.Should().BeEmpty();
                player.RevealedCards.Values.Should().ContainSingle()
                    .Which.Should().NotBe(joker);
                testSubject.DiscardPile.Should().BeEmpty();
                otherPlayer.Hand.Should().Contain(originalDiscard)
                    .And.Contain(originalOtherPlayerHand);
                testSubject.TurnsManager.Current.Should().Be(otherPlayerId);
            },
            [],
            originalRevealed,
            originalDiscardPile: originalDiscard);
    }

    [Test]
    public void WhenPlayerHasNoCardsInHandAndHasNoJokerInRevealedCards()
    {
        var deck = CardsDeck.FullShuffledDeck(excludeJokers: true);
        var testSubject = GetTestSubject(
            [
                DealPlayer(deck,
                    [],
                    new(){
                        [2] = RandomCard(),
                    }),
                DealPlayer(deck),
                DealPlayer(deck),
                DealPlayer(deck)],
            [],
            Discard(deck, 5));

        int otherPlayerId = Random.Next(1, 3);
        var player = testSubject.PlayerStates[0];
        var otherPlayer = testSubject.PlayerStates[otherPlayerId];
        var originalRevealed = player.RevealedCards.ToDictionary();
        var originalOtherPlayerHand = otherPlayer.Hand.ToArray();
        var originalDiscard = testSubject.DiscardPile.ToArray();

        PlaceJoker move = new(otherPlayerId);

        ValidateInvalidMove(
            testSubject,
            player,
            move,
            [],
            originalRevealed,
            originalDiscardPile: originalDiscard);
    }

    [Test]
    public void WhenPlayerHasNoCardsInHandAndMoreThanOneJokerInRevealedCards()
    {
        var joker1 = Card.GetJoker(Color.Black);
        var joker2 = Card.GetJoker(Color.Red);

        var deck = CardsDeck.FullShuffledDeck(excludeJokers: true);
        var testSubject = GetTestSubject(
            [
                DealPlayer(deck,
                    [],
                    new(){
                        [0] = joker1,
                        [1] = joker2,
                        [2] = RandomCard(),
                    }),
                DealPlayer(deck),
                DealPlayer(deck),
                DealPlayer(deck)],
            [],
            Discard(deck, 5));

        int otherPlayerId = Random.Next(1, 3);
        var player = testSubject.PlayerStates[0];
        var otherPlayer = testSubject.PlayerStates[otherPlayerId];
        var originalRevealed = player.RevealedCards.ToDictionary();
        var originalOtherPlayerHand = otherPlayer.Hand.ToArray();
        var originalDiscard = testSubject.DiscardPile.ToArray();

        PlaceJoker move = new(otherPlayerId);

        ValidateValidMove(
            testSubject,
            player,
            move,
            () =>
            {
                player.Hand.Should().BeEmpty();
                player.RevealedCards.Values.Should().HaveCount(2)
                    .And.Contain(joker2).And.NotContain(joker1);
                testSubject.DiscardPile.Should().BeEmpty();
                otherPlayer.Hand.Should().Contain(originalDiscard)
                    .And.Contain(originalOtherPlayerHand);
                testSubject.TurnsManager.Current.Should().Be(otherPlayerId);
            },
            [],
            originalRevealed,
            originalDiscardPile: originalDiscard);
    }

    [Test]
    public void WhenPlayerHasNoRevealedCardsAndHasRevealedJokerInUndercards()
    {
        var joker = RandomCard(Value.Joker);

        var deck = CardsDeck.FullShuffledDeck(excludeJokers: true);
        var testSubject = GetTestSubject(
            [
                DealPlayer(deck,
                    [],
                    [],
                    new(){
                        [0] = new CardFace<Card>(joker, isRevealed: true),
                        [2] = RandomCard(),
                    }),
                DealPlayer(deck),
                DealPlayer(deck),
                DealPlayer(deck)],
            [],
            Discard(deck, 5));

        int otherPlayerId = Random.Next(1, 3);
        var player = testSubject.PlayerStates[0];
        var otherPlayer = testSubject.PlayerStates[otherPlayerId];
        var originalUndercards = player.Undercards.ToDictionary();
        var originalOtherPlayerHand = otherPlayer.Hand.ToArray();
        var originalDiscard = testSubject.DiscardPile.ToArray();

        PlaceJoker move = new(otherPlayerId);

        ValidateValidMove(
            testSubject,
            player,
            move,
            () =>
            {
                player.Hand.Should().BeEmpty();
                player.RevealedCards.Should().BeEmpty();
                player.Undercards.Values.Should().ContainSingle()
                    .Which.Should().NotBeEquivalentTo(new CardFace<Card>(joker, isRevealed: true));
                testSubject.DiscardPile.Should().BeEmpty();
                otherPlayer.Hand.Should().Contain(originalDiscard)
                    .And.Contain(originalOtherPlayerHand);
                testSubject.TurnsManager.Current.Should().Be(otherPlayerId);
            },
            [],
            [],
            originalUndercards,
            originalDiscardPile: originalDiscard);
    }

    [Test]
    public void WhenPlayerHasNoRevealedCardsAndHasUnrevealedJokerInUndercards()
    {
        var joker = RandomCard(Value.Joker);

        var deck = CardsDeck.FullShuffledDeck(excludeJokers: true);
        var testSubject = GetTestSubject(
            [
                DealPlayer(deck,
                    [],
                    [],
                    new(){
                        [0] = new CardFace<Card>(joker, isRevealed: false),
                        [2] = RandomCard(),
                    }),
                DealPlayer(deck),
                DealPlayer(deck),
                DealPlayer(deck)],
            [],
            Discard(deck, 5));

        int otherPlayerId = Random.Next(1, 3);
        var player = testSubject.PlayerStates[0];
        var otherPlayer = testSubject.PlayerStates[otherPlayerId];
        var originalUndercards = player.Undercards.ToDictionary();
        var originalDiscard = testSubject.DiscardPile.ToArray();

        PlaceJoker move = new(otherPlayerId);

        ValidateInvalidMove(
            testSubject,
            player,
            move,
            [],
            [],
            originalUndercards,
            originalDiscardPile: originalDiscard);
    }

    [Test]
    public void WhenPlayerHasNoRevealedCardsAndHasNoJokerInUndercards()
    {
        var deck = CardsDeck.FullShuffledDeck(excludeJokers: true);
        var testSubject = GetTestSubject(
            [
                DealPlayer(deck,
                    [],
                    [],
                    new(){
                        [0] = new CardFace<Card>(RandomCard(), isRevealed: true),
                    }),
                DealPlayer(deck),
                DealPlayer(deck),
                DealPlayer(deck)],
            [],
            Discard(deck, 5));

        int otherPlayerId = Random.Next(1, 3);
        var player = testSubject.PlayerStates[0];
        var otherPlayer = testSubject.PlayerStates[otherPlayerId];
        var originalUndercards = player.Undercards.ToDictionary();
        var originalDiscard = testSubject.DiscardPile.ToArray();

        PlaceJoker move = new(otherPlayerId);

        ValidateInvalidMove(
            testSubject,
            player,
            move,
            [],
            [],
            originalUndercards,
            originalDiscardPile: originalDiscard);
    }

    [Test]
    public void WhenPlayerHasNoRevealedCardsAndHasNoJokerNorRevealedUndercards()
    {
        var deck = CardsDeck.FullShuffledDeck(excludeJokers: true);
        var testSubject = GetTestSubject(
            [
                DealPlayer(deck,
                    [],
                    [],
                    new(){
                        [0] = new CardFace<Card>(RandomCard(), isRevealed: false),
                    }),
                DealPlayer(deck),
                DealPlayer(deck),
                DealPlayer(deck)],
            [],
            Discard(deck, 5));

        int otherPlayerId = Random.Next(1, 3);
        var player = testSubject.PlayerStates[0];
        var otherPlayer = testSubject.PlayerStates[otherPlayerId];
        var originalUndercards = player.Undercards.ToDictionary();
        var originalDiscard = testSubject.DiscardPile.ToArray();

        PlaceJoker move = new(otherPlayerId);

        ValidateInvalidMove(
            testSubject,
            player,
            move,
            [],
            [],
            originalUndercards,
            originalDiscardPile: originalDiscard);
    }

    [Test]
    public void WhenOtherPlayerIsNegative()
    {
        var joker = RandomCard(Value.Joker);

        var deck = CardsDeck.FullShuffledDeck(excludeJokers: true);
        var testSubject = GetTestSubject(
            [
                DealPlayer(deck,
                    [joker, RandomCard(), RandomCard()]),
                DealPlayer(deck),
                DealPlayer(deck),
                DealPlayer(deck)],
            deck,
            Discard(deck, 5));

        var player = testSubject.PlayerStates[0];
        var originalHand = player.Hand.ToArray();
        var originalRevealed = player.RevealedCards.ToDictionary();
        var originalUndercards = player.Undercards.ToDictionary();
        var originalDiscard = testSubject.DiscardPile.ToArray();

        PlaceJoker move = new(-1);

        ValidateInvalidMove(
            testSubject,
            player,
            move,
            originalHand,
            originalRevealed,
            originalUndercards,
            originalDiscardPile: originalDiscard);
    }

    [Test]
    public void WhenOtherPlayerDoesNotExist()
    {
        var joker = RandomCard(Value.Joker);

        var deck = CardsDeck.FullShuffledDeck(excludeJokers: true);
        var testSubject = GetTestSubject(
            [
                DealPlayer(deck,
                    [joker, RandomCard(), RandomCard()]),
                DealPlayer(deck),
                DealPlayer(deck),
                DealPlayer(deck)],
            deck,
            Discard(deck, 5));

        var player = testSubject.PlayerStates[0];
        var originalHand = player.Hand.ToArray();
        var originalRevealed = player.RevealedCards.ToDictionary();
        var originalUndercards = player.Undercards.ToDictionary();
        var originalDiscard = testSubject.DiscardPile.ToArray();

        PlaceJoker move = new(5);

        ValidateInvalidMove(
            testSubject,
            player,
            move,
            originalHand,
            originalRevealed,
            originalUndercards,
            originalDiscardPile: originalDiscard);
    }

    [Test]
    public void WhenOtherPlayerLeftTheGame()
    {
        var joker = RandomCard(Value.Joker);

        var deck = CardsDeck.FullShuffledDeck(excludeJokers: true);
        var testSubject = GetTestSubject(
            [
                DealPlayer(deck,
                    [joker, RandomCard(), RandomCard()]),
                DealPlayer(deck),
                DealPlayer(deck),
                DealPlayer(deck)],
            deck,
            Discard(deck, 5));

        int otherPlayerId = Random.Next(1, 3);
        var player = testSubject.PlayerStates[0];
        var originalHand = player.Hand.ToArray();
        var originalRevealed = player.RevealedCards.ToDictionary();
        var originalUndercards = player.Undercards.ToDictionary();
        var originalDiscard = testSubject.DiscardPile.ToArray();

        testSubject.PlayMove(new LeaveGame(otherPlayerId), otherPlayerId);
        testSubject.LastMove = testSubject.LastPlayedMove = null;

        PlaceJoker move = new(otherPlayerId);

        ValidateInvalidMove(
            testSubject,
            player,
            move,
            originalHand,
            originalRevealed,
            originalUndercards,
            originalDiscardPile: originalDiscard);
    }
}
