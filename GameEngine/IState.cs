using System;
using System.Collections.Generic;
using System.Text;
using TurnsManager;

namespace GameEngine
{
	public interface IState<TGameState, TSharedState, TPlayerState>
	{
		event EventHandler Updated;

		ITurnsManager Turns { get; }
		TGameState GameState { get; }
		TSharedState SharedState { get; }

		TPlayerState GetPlayerState(int player);
	}
}
