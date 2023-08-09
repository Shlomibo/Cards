using GameEngine;
using Shithead.ShitheadMove;
using System;
using System.Linq;
using TurnsManagement;
using Shithead.ShitheadMove;

namespace Shithead.State
{
	public sealed partial class ShitheadState: 
		IState<ShitheadState, ShitheadState.Shared, ShitheadState.Player, IShitheadMove>
	{
		#region Events

		public event EventHandler? Updated;
		#endregion

		#region Properties

		public int PlayersCount { get; private set; }

		public ShitheadState GameState => this;

		public Shared SharedState { get; }
		#endregion

		#region Constructors

		public ShitheadState()
		{
			this.SharedState = new Shared(this);
		}
		#endregion

		#region Methods

		public Player GetPlayerState(int playerId) =>
			new Player(playerId, this);

		public bool IsGameOver()
		{
			throw new NotImplementedException();
		}

		public bool IsValidMove(IShitheadMove move, int? player = null) =>
			GetMove(move, player) is not null;

		public void PlayMove(IShitheadMove move, int? player = null)
		{
			var moveAction = GetMove(move, player);
			moveAction();
		}

		private Action GetMove(IShitheadMove move, int? player = null)
		{
			throw new NotImplementedException();
		}
		#endregion
	}
}
