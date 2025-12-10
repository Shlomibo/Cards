using System;

using AwesomeAssertions;
using AwesomeAssertions.Execution;

using Deck;
using Deck.Cards.FrenchSuited;

using NUnit.Framework.Internal;

using Shithead.State;

namespace Shithead.UnitTests.State.ShitheadStateTests;

public class StateCreationTests
{
    private const int STARTING_CARDS_PER_PLAYER = 3 + 3 + 3;
    protected static TestContext TestContext => TestContext.CurrentContext;
    protected static Randomizer Random => TestContext.Random;

    [Test]
    public void WhenCreatingAGameStateWithInsufficientCards()
    {
        int playersCount = Random.Next(3, 6);
        int cardsCount = (playersCount * STARTING_CARDS_PER_PLAYER) - 1;
        CardsDeck deck = [.. Card.AllCards().Take(cardsCount)];

        var creation = () => new ShitheadState(playersCount, deck);

        creation.Should().Throw<ArgumentOutOfRangeException>("players count was out of range");
    }

    [Test]
    public void WhenCreatingAGameStateWithJustEnoughCards()
    {
        int playersCount = Random.Next(3, 6);
        int cardsCount = playersCount * STARTING_CARDS_PER_PLAYER;
        CardsDeck deck = [.. Card.AllCards().Take(cardsCount)];

        ShitheadState state = new(playersCount, deck);

        state.Should().BeEquivalentTo(
            new
            {
                PlayersCount = playersCount,
                GameState = GameState.Init,
            },
            "state properties are correct");

        state.DiscardPile.Should().BeEmpty("no cards have been discarded yet");
        state.Deck.Should().BeEmpty("all cards have been dealt");
        state.PlayerStates.Should().HaveCount(playersCount, "all players have states");

        HashSet<int> usedIds = [];

        foreach (var playerState in state.PlayerStates)
        {
            usedIds.Add(playerState.Id).Should().BeTrue("the player id is unique");
            playerState.Should().BeEquivalentTo(
                new
                {
                    DidLeaveGame = false,
                    RevealedCards = new Dictionary<int, CardFace<Card>>(),
                    Won = false,
                    RevealedCardsAccepted = false,
                },
                "player state properties are set correctly");

            playerState.Hand.Should().HaveCount(6, $"player {playerState.Id} received enough cards");
            playerState.Undercards.Should().HaveCount(3, "player undercards have dealt");
            playerState.Undercards.Values.Should().OnlyContain(card => !card.IsRevealed, "none of the under cards is revealed");
        }
    }

    [Test]
    public void WhenCreatingAGameState()
    {
        int playersCount = Random.Next(3, 6);
        var deck = CardsDeck.FullDeck();

        ShitheadState state = new(playersCount, deck);

        state.Should().BeEquivalentTo(
            new
            {
                PlayersCount = playersCount,
                GameState = GameState.Init,
            },
            "state properties are correct");

        state.DiscardPile.Should().BeEmpty("no cards have been discarded yet");
        state.Deck.Should().NotBeEmpty("there are cards left in the deck");
        state.Deck.Should().NotBeInAscendingOrder(CardComparer.Instance);
        state.Deck.Should().NotBeInDescendingOrder(CardComparer.Instance);

        state.PlayerStates.Should().HaveCount(playersCount, "all players have states");

        HashSet<int> usedIds = [];

        foreach (var playerState in state.PlayerStates)
        {
            usedIds.Add(playerState.Id).Should().BeTrue("the player id is unique");
            playerState.Should().BeEquivalentTo(
                new
                {
                    DidLeaveGame = false,
                    RevealedCards = new Dictionary<int, CardFace<Card>>(),
                    Won = false,
                    RevealedCardsAccepted = false,
                },
                "player state properties are set correctly");

            playerState.Hand.Should().HaveCount(6, $"player {playerState.Id} received enough cards");
            playerState.Undercards.Should().HaveCount(3, "player undercards have dealt");
            playerState.Undercards.Values.Should().OnlyContain(card => !card.IsRevealed, "none of the under cards is revealed");
        }
    }
}
