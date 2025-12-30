using System;
using DTOs.Cards.FrenchSuited;

namespace DTOs.Shithead;

public class SharedState
{
    public GameState GameState { get; init; }
    public required OtherPlayersState[] Players { get; init; }
    public required int[] ActivePlayers { get; init; }
    public int DeckSize { get; init; }
    public required Card[] DiscardPile { get; init; }
    public int CurrentTurnPlayer { get; init; }
    public AttemptedMove? LastMove { get; init; }
    public AttemptedMove? LastPlayedMove { get; init; }
}
