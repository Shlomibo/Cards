using System;

using Deck;
using Deck.Cards.FrenchSuited;

using Shithead.State;

namespace Shithead.UnitTests.State.ShitheadStateTests.GameOnTests;

public abstract class GameOnTestsBase : ShitheadStateTestsBase
{
    protected static ShitheadState GetTestSubject(
        IReadOnlyCollection<PlayerData> playerData,
        CardsDeck deck,
        IEnumerable<Card> discardPile)
        =>
        GetTestSubject(playerData, GameState.GameOn, deck, discardPile);

    protected static PlayerData DealPlayer(
        CardsDeck deck,
        IEnumerable<Card>? hand = null,
        Dictionary<int, Card>? revealedCards = null,
        Dictionary<int, CardFace<Card>>? undercards = null)
        =>
        new(
            hand == null
                ? DealHand(deck)
                : [.. hand],
            revealedCards ?? DealRevealedCards(deck),
            undercards ?? DealUndercards(deck));
}
