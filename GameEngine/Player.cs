namespace GameEngine
{
	partial class Engine<TGameState, TSharedState, TPlayerState, TGameMove>
	{
		private class Player : IPlayer<TSharedState, TPlayerState, TGameMove>
		{
			#region Events

			public event EventHandler? Updated;
			#endregion

			#region Properties

			public int PlayerId { get; }

			public TSharedState SharedState => this.Engine.state.SharedState;

			public TPlayerState State { get; }

			public Engine<TGameState, TSharedState, TPlayerState, TGameMove> Engine { get; }
			#endregion

			#region Ctors

			public Player(
				int playerId,
				Engine<TGameState, TSharedState, TPlayerState, TGameMove> engine,
				TPlayerState state)
			{
				this.Engine = engine ?? throw new ArgumentNullException(nameof(engine));
				this.PlayerId = playerId;
				this.State = state ?? throw new ArgumentNullException(nameof(state));

				this.Engine.Updated += (_, args) => this.Updated?.Invoke(this, args);
			}
			#endregion

			#region Methods

			public void PlayMove(TGameMove move)
			{
				this.Engine.PlayMove(move, this.PlayerId);
			}

			public bool IsValidMove(TGameMove move)
			{
				return this.Engine.IsValidMove(move, this.PlayerId);
			}

			#endregion
		}
	}
}
