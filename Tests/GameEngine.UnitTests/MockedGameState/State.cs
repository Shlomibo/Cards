using System;

using AutoFixture;

namespace GameEngine.UnitTests.MockedGameState;

public class State : IState<GameState, SharedState, PlayerState, Move>
{
    public const int DefPlayersCount = 4;
    private PlayerState[] _playerStates = [.. Fixture.CreateMany<PlayerState>(DefPlayersCount)];

    public static Fixture Fixture { get; } = new();
    public int? LastPlayedPlayer { get; private set; }
    public int PlayersCount
    {
        get;
        set
        {
            ArgumentOutOfRangeException.ThrowIfLessThanOrEqual(value, 0);
            field = value;
            _playerStates = [.. Fixture.CreateMany<PlayerState>(value)];
        }
    } = DefPlayersCount;

    public GameState GameState { get; set; } = Fixture.Create<GameState>();

    public SharedState SharedState { get; } = Fixture.Create<SharedState>();

    public PlayerState GetPlayerState(int player) => _playerStates[player];

    public bool GameOverValue { get; set; }
    public bool IsNextMoveValid { get; set; } = true;

    public bool IsGameOver() => GameOverValue;

    public bool IsValidMove(Move move, int? player = null)
    {
        LastPlayedPlayer = player;
        return IsNextMoveValid;
    }

    public bool PlayMove(Move move, int? player = null)
    {
        LastPlayedPlayer = player;
        return IsNextMoveValid;
    }
}

public record GameState
{
    public int Value { get; set; }
}
public record UpdateGameState : GameState;
public record SharedState
{
    public int Value { get; set; }
}
public record UpdateSharedState : SharedState;
public record PlayerState
{
    public int Value { get; set; }
}
public record UpdatePlayerState : PlayerState;
public record Move
{
    public int Value { get; set; }
}
public record AnotherMove : Move;
