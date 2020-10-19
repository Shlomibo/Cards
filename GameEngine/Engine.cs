using System;
using System.Collections.Generic;
using System.Linq;
using TurnsManager;

namespace GameEngine
{
	public sealed partial class Engine<TGameState, TSharedState, TPlayerState, TActions>
	{
		#region Fields

		private readonly Player[] players;
		private readonly IState<TGameState, TSharedState, TPlayerState> state;
		#endregion

		#region Properties

		public IReadOnlyList<IPlayer<TActions, TSharedState, TPlayerState>> Players => this.players;
		#endregion

		#region Ctors

		public Engine(IState<TGameState, TSharedState, TPlayerState> state, Func<int, TActions> actionsFactory)
		{
			if (actionsFactory is null)
			{
				throw new ArgumentNullException(nameof(actionsFactory));
			}

			this.state = state ?? throw new ArgumentNullException(nameof(state));
			this.players = Enumerable.Range(0, this.state.Turns.PlayersCount)
				.Select(player => new Player(this, state.GetPlayerState(player), actionsFactory(player)))
				.ToArray();
		} 
		#endregion
	}
}
