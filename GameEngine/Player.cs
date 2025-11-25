namespace GameEngine;

#pragma warning disable IDE0040 // Add accessibility modifiers
partial class Engine<TGameState, TSharedState, TPlayerState, TGameMove>
#pragma warning restore IDE0040 // Add accessibility modifiers
{
    private class Player : IPlayer<TSharedState, TPlayerState, TGameMove>
    {
        public event EventHandler? Updated;

        public int PlayerId { get; }

        public TSharedState SharedState => Engine._state.SharedState;

        public TPlayerState State { get; }

        public Engine<TGameState, TSharedState, TPlayerState, TGameMove> Engine { get; }

        public Player(
            int playerId,
            Engine<TGameState, TSharedState, TPlayerState, TGameMove> engine,
            TPlayerState state)
        {
            Engine = engine ?? throw new ArgumentNullException(nameof(engine));
            PlayerId = playerId;
            State = state ?? throw new ArgumentNullException(nameof(state));

            Engine.Updated += (_, args) => Updated?.Invoke(this, args);
        }

        public void PlayMove(TGameMove move) => Engine.PlayMove(move, PlayerId);

        public bool IsValidMove(TGameMove move) => Engine.IsValidMove(move, PlayerId);
    }
}
