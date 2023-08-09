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

			public TGameMove Actions { get; }
			#endregion

			#region Ctors

			public Player(
				int playerId,
				Engine<TGameState, TSharedState, TPlayerState, TGameMove> engine,
				TPlayerState state,
				TGameMove actions
			)
			{
				if (actions == null)
				{
					throw new ArgumentNullException(nameof(actions));
				}
				if (state == null)
				{
					throw new ArgumentNullException(nameof(state));
				}

				this.engine = engine ?? throw new ArgumentNullException(nameof(engine));
				this.PlayerId = playerId;
				this.State = state;
				this.Actions = actions;
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
