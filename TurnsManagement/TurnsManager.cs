using System;
using System.Diagnostics;

namespace TurnsManagement
{
	public sealed class TurnsManager : ITurnsManager
	{
		#region Properties

		public int PlayersCount { get; }

		public int Current { get; private set; }

		public TurnsDirection Direction { get; set; }
		#endregion

		#region Ctors

		public TurnsManager(int playersCount, TurnsDirection? direction = null)
		{
			if (playersCount <= 0)
			{
				throw new ArgumentException("Players count must be greater than 0", nameof(playersCount));
			}

			this.PlayersCount = playersCount;

			if (direction.HasValue)
			{
				this.Direction = direction.Value;
			}
		}
		#endregion

		#region Methods

		public int Jump(int skippedTurns, TurnsDirection? direction = null)
		{
			if (skippedTurns < 0)
			{
				throw new ArgumentException("SkippedTurns must not be negative", nameof(skippedTurns));
			}

			if (direction is TurnsDirection newDir)
			{
				this.Direction = newDir;
			}

			if (skippedTurns != 0)
			{
				if (this.Direction == TurnsDirection.Down)
				{
					skippedTurns *= -1;
				}

				this.Current = (this.Current + skippedTurns) % this.PlayersCount;

				if (this.Current < 0)
				{
					this.Current += this.PlayersCount;
				}

				Debug.Assert(this.Current >= 0, "Current player is negative number!");
				Debug.Assert(this.Current < this.PlayersCount, "Current is greater than players count!");
			}

			return this.Current;
		}

		public int MoveNext(int? next = null) =>
			next switch
			{
				int value => this.Current = value,
				_ => Jump(1),
			};

		public TurnsDirection SwitchDirection() =>
			this.Direction = this.Direction switch
			{
				TurnsDirection.Up => TurnsDirection.Down,
				TurnsDirection.Down => TurnsDirection.Up,
				_ => throw new InvalidOperationException("Invalid direction"),
			}; 
		#endregion
	}
}
