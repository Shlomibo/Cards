using System;

using Deck.Cards.FrenchSuited;

using Shithead.State;

namespace Shithead.UnitTests.State.ShitheadStateTests.InitTests;

public abstract class InitTestsBase : ShitheadStateTestsBase
{
    protected static ShitheadState GetTestSubject(
        IReadOnlyCollection<PlayerData> playerData,
        CardsDeck deck)
        =>
        GetTestSubject(playerData, GameState.Init, deck, []);
}
