namespace GameServer.DTO;

public abstract record State
{
    public object SharedState => GetSharedState();
    public object PlayerState => GetPlayerState();


    private protected abstract object GetPlayerState();
    private protected abstract object GetSharedState();
}

public record State<TShared, TPlayer> : State
    where TShared : notnull
    where TPlayer : notnull
{
    public new required TShared SharedState { get; init; }
    public new required TPlayer PlayerState { get; init; }

    private protected sealed override object GetPlayerState() => PlayerState;

    private protected sealed override object GetSharedState() => SharedState;
}
