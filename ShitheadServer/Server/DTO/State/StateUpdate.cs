using GameServer.DTO;
using Shithead.State;

namespace ShitheadServer.Server.DTO.State;

public sealed class StateUpdate : IState<SharedState, PlayerState>
{
	public SharedState SharedState { get; }

	public PlayerState PlayerState { get; }

	public StateUpdate(
		ShitheadState.SharedShitheadState sharedState,
		ShitheadState.ShitheadPlayerState playerState)
	{
		SharedState = new SharedState(sharedState);
		PlayerState = new PlayerState(playerState);
	}
}
