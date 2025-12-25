using System;

using GameEngine.UnitTests.MockedGameState;

namespace GameEngine.UnitTests;

public abstract class TestsBase
{

    protected State State { get; set; }
    protected IEngine<SharedState, PlayerState, Move> Engine { get; set; }

    [SetUp]
    public virtual void Setup()
    {
        State = new State();
        Engine = new Engine<GameState, SharedState, PlayerState, Move>(
            State
        );
    }
}
