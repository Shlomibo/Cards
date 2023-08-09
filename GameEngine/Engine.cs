using System;
using System.Collections.Generic;
using System.Linq;

namespace GameEngine
{
	public sealed partial class Engine<TGameState, TSharedState, TPlayerState, TGameMove>
	{
		#region Fields

		private readonly Player[] players;
		private readonly IState<TGameState, TSharedState, TPlayerState, TGameMove> state;
		#endregion

		#region Properties

		public IReadOnlyList<IPlayer<TSharedState, TPlayerState, TGameMove>> Players => this.players;
		#endregion

		#region Ctors

		public Engine(IState<TGameState, TSharedState, TPlayerState, TGameMove> state)
		{
			this.state = state ?? throw new ArgumentNullException(nameof(state));
			this.players = Enumerable.Range(0, this.state.PlayersCount)
				.Select((player, id) => new Player(
					id,
					this,
					state.GetPlayerState(player)
				))
				.ToArray();
		}
		#endregion

		#region Methods
		public bool IsValidMove(int player, TGameMove move)
		{
			return this.state.IsValidMove(player, move);
		}

		public void PlayMove(int playerId, TGameMove move)
		{
			this.state.PlayMove(playerId, move);
		}
		#endregion
	}
}
