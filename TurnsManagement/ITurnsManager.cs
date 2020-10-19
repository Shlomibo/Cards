using System;
using System.Collections.Generic;
using System.Text;

namespace TurnsManager
{
	public interface ITurnsManager
	{
		int PlayersCount { get; }
		int Current { get; }
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
