using GameEngine;
using Shithead.ShitheadMove;
using Shithead.State;

namespace Shithead;

public static class ShitheadGame
{
	public static Engine<
		ShitheadState,
		ShitheadState.SharedShitheadState,
		ShitheadState.ShitheadPlayerState,
		IShitheadMove> CreateGame(int playersCount)
	{
		return new Engine<
			ShitheadState,
			ShitheadState.SharedShitheadState,
			ShitheadState.ShitheadPlayerState,
			IShitheadMove>(new ShitheadState(playersCount));
	}
}
