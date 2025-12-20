using GameEngine;

namespace GameServer;

internal interface ITable<TGameState, TSharedState, TPlayerState, TGameMove>
{
    Table<TGameState, TSharedState, TPlayerState, TGameMove>.Player this[Guid connectionId] { get; }

    Table<TGameState, TSharedState, TPlayerState, TGameMove>.Player TableMaster { get; }
    string TableName { get; }
    Engine<TGameState, TSharedState, TPlayerState, TGameMove>? Game { get; }
    bool GameStarted { get; }

    event EventHandler? TableUpdated;
    event EventHandler<TableGameUpdateEventArgs<TGameState, TSharedState, TPlayerState, TGameMove>>? GameUpdated;

    Table<TGameState, TSharedState, TPlayerState, TGameMove>.Player AddPlayer(string name);
    Table AsTableDescriptor();
    void PlayMove(TGameMove move, int? playerId = null);
    void RemovePlayer(Guid connectionId);
    void SetGame(Engine<TGameState, TSharedState, TPlayerState, TGameMove> game);
    bool TrySetGame(Engine<TGameState, TSharedState, TPlayerState, TGameMove> game);
}
