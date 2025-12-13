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
    protected static Card Not(Card notValue, params IEnumerable<Card>? otherValues) =>
        Not(notValue.Value, otherValues?.Select(c => c.Value));

    protected static Card Not(Value notValue, params IEnumerable<Value>? otherValues)
    {
        HashSet<Value> notValues = [.. otherValues ?? [], notValue];
        int fuckedOutCount = 100;

        Card cardValue;
        do
        { cardValue = RandomCard(); }
        while (notValues.Contains(cardValue.Value) && 0 < --fuckedOutCount);

        return fuckedOutCount != 0
            ? cardValue
            : throw new InvalidOperationException("All cards are NOTed");
    }
}
