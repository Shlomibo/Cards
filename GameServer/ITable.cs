using GameEngine;

namespace GameServer;

internal interface ITable<TGameState, TSharedState, TPlayerState, TGameMove>
{
    Table<TGameState, TSharedState, TPlayerState, TGameMove>.Player this[Guid connectionId] { get; }

    Table<TGameState, TSharedState, TPlayerState, TGameMove>.Player TableMaster { get; }
    string TableName { get; }
    IEngine<TSharedState, TPlayerState, TGameMove>? Game { get; }
    bool GameStarted { get; }

    event EventHandler? TableUpdated;
    event EventHandler<TableGameUpdateEventArgs<TGameState, TSharedState, TPlayerState, TGameMove>>? GameUpdated;
    IReadOnlyList<Table<TGameState, TSharedState, TPlayerState, TGameMove>.Player> GetPlayers();

    Table<TGameState, TSharedState, TPlayerState, TGameMove>.Player AddPlayer(string name);
    bool CanAddPlayer(string name);
    Table AsTableDescriptor();
    void PlayMove(TGameMove move, int? playerId = null);
    void RemovePlayer(Guid connectionId);
    void SetGame(IEngine<TSharedState, TPlayerState, TGameMove>? game);
}
