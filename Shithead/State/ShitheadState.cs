using GameEngine;
using System;
using System.Linq;
using TurnsManagement;

namespace Shithead.State
{
	public sealed class ShitheadState : IState<Game, Shared, Player>
	{
		#region Fields

		private readonly Player[] playerStates;
		#endregion

		#region Events

		public event EventHandler? Updated;
		#endregion

		#region Properties

		public ITurnsManager Turns => this.GameState.TurnsManager;

		public Game GameState { get; }

		public Shared SharedState { get; }
		#endregion

		#region Ctors

		public ShitheadState(int playersCount)
		{
			this.GameState = new Game(playersCount);
			this.SharedState = new Shared(this.GameState);

			this.playerStates = Enumerable.Range(0, playersCount)
				.Select(player => new Player(this.GameState, this.SharedState, player))
				.ToArray();
		}
		#endregion

		#region Methods

		public Player GetPlayerState(int player) =>
			this.playerStates[player]; 
		#endregion
	}
}
