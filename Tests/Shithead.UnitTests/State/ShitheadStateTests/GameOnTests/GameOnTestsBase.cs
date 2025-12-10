using System;

using Deck.Cards.FrenchSuited;

using Shithead.State;

namespace Shithead.UnitTests.State.ShitheadStateTests.GameOnTests;

public abstract class GameOnTestsBase : ShitheadStateTestsBase
{
    protected static ShitheadState GetTestSubject(
        IReadOnlyCollection<PlayerData> playerData,
        CardsDeck deck)
        =>
        GetTestSubject(playerData, GameState.GameOn, deck);
}
