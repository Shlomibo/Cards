using System;
using System.Collections.Generic;
using System.Text;

namespace GameEngine
{
	partial class Engine<TGameState, TSharedState, TPlayerState, TActions>
	{
		private class Player : IPlayer<TActions, TSharedState, TPlayerState>
		{
			#region Fields

			private readonly Engine<TGameState, TSharedState, TPlayerState, TActions> engine;
			#endregion

			#region Events

			public event EventHandler Updated
			{
				add => this.engine.state.Updated += value;
				remove => this.engine.state.Updated -= value;
			}
			#endregion

			#region Properties

			public TSharedState SharedState => this.engine.state.SharedState;

			public TPlayerState State { get; }

			public TActions Actions { get; }
			#endregion

			#region Ctors

			public Player(Engine<TGameState, TSharedState, TPlayerState, TActions> engine,
				 TPlayerState state,
				 TActions actions)
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
				this.State = state;
				this.Actions = actions;
			} 
			#endregion
		}
	}
}
