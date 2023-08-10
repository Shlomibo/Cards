namespace TurnsManagement
{
	public interface ITurnsManager
	{
		int PlayersCount { get; }
		int Current { get; set; }
		TurnsDirection Direction { get; set; }
		
		int MoveNext(int? next = null);
		int Jump(int skippedTurns, TurnsDirection? direction = null);
		TurnsDirection SwitchDirection();
	}

	public enum TurnsDirection
	{
		Up,
		Down,
	}
}
