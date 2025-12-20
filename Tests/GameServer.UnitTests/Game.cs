using System;
using System.Diagnostics.CodeAnalysis;

using DTOs;

using GameEngine;

namespace GameServer.UnitTests;

public sealed class Game : Engine<
GameState,
GameState,
GameState,
GameMove>
{
    public Game(GameState state) : base(state)
    {
    }
}

public record GameState : IState<GameState, GameState, GameState, GameMove>
{
    public bool DidMove { get; set; }
    public bool GameOver { get; set; }

    public int PlayersCount => 1;

    public GameState SharedState => this;

    GameState IState<GameState, GameState, GameState, GameMove>.GameState => this;

    public GameState GetPlayerState(int player) => this;

    public bool IsGameOver() => GameOver;

    public bool IsValidMove(GameMove move, int? player = null) => move is ValidMove;

    public bool PlayMove(GameMove move, int? player = null)
    {
        switch ((move, DidMove, GameOver))
        {
            case (ValidMove, false, false):
                DidMove = true;
                return true;
            case (ValidMove, _, false):
                GameOver = true;
                return true;
            default:
                return false;
        }
    }

    public Serialized Serialize() =>
        Serialize(this);

    public static Serialized Serialize(GameState state) =>
        new(state.DidMove, state.GameOver);

    public record Serialized : State<(bool DidMove, bool GameOver), (bool DidMove, bool GameOver)>
    {
        [SetsRequiredMembers]
        public Serialized(bool didMove, bool gameOver)
        {
            SharedState = (didMove, gameOver);
            PlayerState = (didMove, gameOver);
        }
    }

}

public record GameMove
{
    public abstract record Serialized
    {
        public GameMove Deserialize() =>
            GameMove.Deserialize(this);
    }

    public static GameMove Deserialize(Serialized move) =>
        move switch
        {
            ValidMove.Serialized { Value: var v } => new ValidMove(v),
            InvalidMove.Serialized { Value: var v } => new InvalidMove(v),
            _ => throw new NotSupportedException(),
        };
}

public record ValidMove(int Value) : GameMove
{
    public new record Serialized(int Value) : GameMove.Serialized;
}

public record InvalidMove(string Value) : GameMove
{
    public new record Serialized(string Value) : GameMove.Serialized;
}
