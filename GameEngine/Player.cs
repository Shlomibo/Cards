namespace GameEngine;

public partial class Engine<TGameState, TSharedState, TPlayerState, TGameMove>
{
	private class Player : IPlayer<TSharedState, TPlayerState, TGameMove>
	{
		#region Events

		public event EventHandler? Updated;
		#endregion

		#region Properties

		public int PlayerId { get; }

		public TSharedState SharedState => Engine.state.SharedState;

		public TPlayerState State { get; }

		public Engine<TGameState, TSharedState, TPlayerState, TGameMove> Engine { get; }
		#endregion

		#region Ctors

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
		#endregion

		#region Methods

		public void PlayMove(TGameMove move)
		{
			Engine.PlayMove(move, PlayerId);
		}

		public bool IsValidMove(TGameMove move)
		{
			return Engine.IsValidMove(move, PlayerId);
		}

		#endregion
	}
}
