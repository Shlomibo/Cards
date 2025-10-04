namespace GameEngine;

public interface IState<TGameState, TSharedState, TPlayerState, TGameMove>
{
	int PlayersCount { get; }
	TGameState GameState { get; }
	TSharedState SharedState { get; }
	bool IsGameOver();

	TPlayerState GetPlayerState(int player);

	bool IsValidMove(TGameMove move, int? player = null);

	bool PlayMove(TGameMove move, int? player = null);
}
