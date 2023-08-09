using System;

namespace GameEngine
{
	partial class Engine<TGameState, TSharedState, TPlayerState, TGameMove>
	{
		private class Player : IPlayer<TSharedState, TPlayerState, TGameMove>
		{
			#region Fields

			private readonly Engine<TGameState, TSharedState, TPlayerState, TGameMove> engine;
			#endregion

			#region Events

			public event EventHandler? Updated
			{
				add => this.engine.state.Updated += value;
				remove => this.engine.state.Updated -= value;
			}
			#endregion

			#region Properties

			public int PlayerId { get; }
			
			public TSharedState SharedState => this.engine.state.SharedState;

			public TPlayerState State { get; }
			#endregion

			#region Ctors

			public Player(
				int playerId,
				Engine<TGameState, TSharedState, TPlayerState, TGameMove> engine,
				TPlayerState state
			)
			{
				if (state == null)
				{
					throw new ArgumentNullException(nameof(state));
				}

				this.engine = engine ?? throw new ArgumentNullException(nameof(engine));
				this.PlayerId = playerId;
				this.State = state;
			}
			#endregion

			#region Methods

			public void PlayMove(TGameMove move)
			{
				this.engine.PlayMove(this.PlayerId, move);
			}

			public bool IsValidMove(TGameMove move)
			{
				return this.engine.IsValidMove(this.PlayerId, move);
			}

			#endregion
		}
	}
}
