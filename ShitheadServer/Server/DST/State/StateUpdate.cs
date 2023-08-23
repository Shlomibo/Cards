using GameServer.DST;
using Shithead.State;

namespace ShitheadServer.Server.DST.State
{
	public sealed class StateUpdate : IState<SharedState, PlayerState>
	{
		public SharedState SharedState { get; }

		public PlayerState PlayerState { get; }

		public StateUpdate(
			ShitheadState.SharedShitheadState sharedState,
			ShitheadState.ShitheadPlayerState playerState)
		{
			this.SharedState = new SharedState(sharedState);
			this.PlayerState = new PlayerState(playerState);
		}
	}
}
