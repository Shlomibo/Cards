namespace DTOs;

/// <summary>
/// A DTO of any game's state
/// </summary>
public abstract record State
{
    /// <summary>
    /// Gets the shared part of the game state
    /// </summary>
    public object SharedState => GetSharedState();

    /// <summary>
    /// Gets the player-specific part of the game state
    /// </summary>
    public object PlayerState => GetPlayerState();


    private protected abstract object GetPlayerState();
    private protected abstract object GetSharedState();
}

/// <inheritdoc/>
/// <typeparam name="TShared">The type of shared state.</typeparam>
/// <typeparam name="TPlayer">The type of player-specific state.</typeparam>
public record State<TShared, TPlayer> : State
    where TShared : notnull
    where TPlayer : notnull
{
    /// <inheritdoc cref="State.SharedState"/>
    public new required TShared SharedState { get; init; }

    /// <inheritdoc cref="State.PlayerState"/>
    public new required TPlayer PlayerState { get; init; }

    private protected sealed override object GetPlayerState() => PlayerState;

    private protected sealed override object GetSharedState() => SharedState;
}
