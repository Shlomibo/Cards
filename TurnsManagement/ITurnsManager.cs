namespace TurnsManagement
{
	public interface ITurnsManager
	{
		int PlayersCount { get; }
		IReadOnlyList<int> ActivePlayers { get; }
		int Current { get; set; }
		TurnsDirection Direction { get; set; }
		
		int MoveNext();
		int Jump(int skippedTurns, TurnsDirection? direction = null);
		TurnsDirection SwitchDirection();
		void RemovePlayer(int playerId);
	}

	public enum TurnsDirection
	{
		Up,
		Down,
	}
}
