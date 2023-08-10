using System.Diagnostics;

namespace TurnsManagement
{
	public sealed class TurnsManager : ITurnsManager
	{
		#region Fields

		private int currentPlayerIndex;
		private readonly List<int> activePlayers;
		#endregion

		#region Properties

		public int PlayersCount { get; }
		public IReadOnlyList<int> ActivePlayers => this.activePlayers;

		public int Current
		{
			get => GetPlayer(currentPlayerIndex);
			set => currentPlayerIndex = this.activePlayers.IndexOf(value) switch
			{
				-1 => throw new ArgumentException($"The player {value} does not exist or was removed", nameof(Current)),
				int index => index,
			};
		}

		public int Previous =>
			this.Direction == TurnsDirection.Up
			? GetPlayer(currentPlayerIndex - 1)
			: GetPlayer(currentPlayerIndex + 1);

		public int Next =>
			this.Direction == TurnsDirection.Up
			? GetPlayer(currentPlayerIndex + 1)
			: GetPlayer(currentPlayerIndex - 1);

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
			this.activePlayers = Enumerable.Range(0, playersCount).ToList();

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

				this.currentPlayerIndex = (this.currentPlayerIndex + skippedTurns) % this.activePlayers.Count;

				// If this.currentPlayerIndex is negative, its value is between -1 and -(this.activePlayers.Count - 1)
				// We want to push it to be positive between 1 and (this.activePlayers.Count - 1), but simply negating it
				// will make it look as if we changed direction.
				// So to make it right - we just need to add this.activePlayers.Count to it...
				if (this.currentPlayerIndex < 0)
				{
					this.currentPlayerIndex += this.activePlayers.Count;
				}

				Debug.Assert(this.currentPlayerIndex >= 0, "Current player is negative number!");
				Debug.Assert(this.currentPlayerIndex < this.activePlayers.Count, "Current is greater than players count!");
			}

			return this.Current;
		}

		public int MoveNext() => Jump(1);

		public TurnsDirection SwitchDirection() =>
			this.Direction = this.Direction switch
			{
				TurnsDirection.Up => TurnsDirection.Down,
				TurnsDirection.Down => TurnsDirection.Up,
				_ => throw new InvalidOperationException("Invalid direction"),
			};

		public void RemovePlayer(int playerId)
		{
			this.activePlayers.Remove(playerId);

			if (this.Direction == TurnsDirection.Up)
			{
				if (this.currentPlayerIndex >= this.activePlayers.Count)
				{
					this.currentPlayerIndex = 0;
				}
			}
			else
			{
				this.currentPlayerIndex--;

				if (this.currentPlayerIndex < 0)
				{
					this.currentPlayerIndex = this.activePlayers.Count == 0
						? 0
						: this.activePlayers.Count - 1;
				}
			}
		}

		private int GetPlayer(int index)
		{
			if (this.activePlayers.Count == 0)
			{
				throw new InvalidOperationException("There are no players to choose from");
			}

			index %= this.activePlayers.Count;

			if (index < 0)
			{
				index += this.activePlayers.Count;
			}

			return this.activePlayers[index];
		}
		#endregion
	}
}
