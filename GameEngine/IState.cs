using System;
using TurnsManagement;

namespace GameEngine
{
    public interface IState<TGameState, TSharedState, TPlayerState, TGameMove>
    {
        event EventHandler? Updated;
		int PlayersCount { get; }
		//ITurnsManager Turns { get; }
		TGameState GameState { get; }
        TSharedState SharedState { get; }
        bool IsGameOver();

        TPlayerState GetPlayerState(int player);

        bool IsValidMove(int player, TGameMove move);

		void PlayMove(int player, TGameMove move);
	}
}
