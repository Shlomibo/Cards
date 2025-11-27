namespace GameServer.DTO;

public sealed record StateUpdate<TState>(
    string TableName,
    CurrentPlayer CurrentPlayer,
    IReadOnlyDictionary<int, Player> Table,
    TState? State = null)
    where TState : State
{
    public StateUpdate(
        string tableName,
        CurrentPlayer currentPlayer,
        IEnumerable<Player> players,
        TState? state = null)
        : this(
            tableName,
            currentPlayer,
            players.ToDictionary(player => player.PlayerId),
            state)
    {
    }
}

public sealed record Player(int PlayerId, string PlayerName);
public sealed record CurrentPlayer(int PlayerId, string PlayerName, Guid ConnectionId);
