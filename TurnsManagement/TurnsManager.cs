using System.Diagnostics;

namespace TurnsManagement;

public sealed class TurnsManager : ITurnsManager
{
    #region Fields

    private int _currentPlayerIndex;
    private readonly List<int> _activePlayers;
    #endregion

    #region Properties

    public int PlayersCount { get; }
    public IReadOnlyList<int> ActivePlayers => _activePlayers;

    public int Current
    {
        get => GetPlayer(_currentPlayerIndex);
        set => _currentPlayerIndex = _activePlayers.IndexOf(value) switch
        {
            -1 => throw new ArgumentException($"The player {value} does not exist or was removed", nameof(Current)),
            int index => index,
        };
    }

    public int Previous =>
        Direction == TurnsDirection.Up
        ? GetPlayer(_currentPlayerIndex - 1)
        : GetPlayer(_currentPlayerIndex + 1);

    public int Next =>
        Direction == TurnsDirection.Up
        ? GetPlayer(_currentPlayerIndex + 1)
        : GetPlayer(_currentPlayerIndex - 1);

    public TurnsDirection Direction { get; set; }
    #endregion

    #region Ctors

    public TurnsManager(int playersCount, TurnsDirection? direction = null)
    {
        if (playersCount <= 0)
        {
            throw new ArgumentException("Players count must be greater than 0", nameof(playersCount));
        }

        PlayersCount = playersCount;
        _activePlayers = Enumerable.Range(0, playersCount).ToList();

        if (direction.HasValue)
        {
            Direction = direction.Value;
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
            Direction = newDir;
        }

        if (skippedTurns != 0)
        {
            if (Direction == TurnsDirection.Down)
            {
                skippedTurns *= -1;
            }

            _currentPlayerIndex = (_currentPlayerIndex + skippedTurns) % _activePlayers.Count;

            // If this.currentPlayerIndex is negative, its value is between -1 and -(this.activePlayers.Count - 1)
            // We want to push it to be positive between 1 and (this.activePlayers.Count - 1), but simply negating it
            // will make it look as if we changed direction.
            // So to make it right - we just need to add this.activePlayers.Count to it...
            if (_currentPlayerIndex < 0)
            {
                _currentPlayerIndex += _activePlayers.Count;
            }

            Debug.Assert(_currentPlayerIndex >= 0, "Current player is negative number!");
            Debug.Assert(_currentPlayerIndex < _activePlayers.Count, "Current is greater than players count!");
        }

        return Current;
    }

    public int MoveNext() => Jump(1);

    public TurnsDirection SwitchDirection() =>
        Direction = Direction switch
        {
            TurnsDirection.Up => TurnsDirection.Down,
            TurnsDirection.Down => TurnsDirection.Up,
            _ => throw new InvalidOperationException("Invalid direction"),
        };

    public void RemovePlayer(int playerId)
    {
        _activePlayers.Remove(playerId);

        if (Direction == TurnsDirection.Up)
        {
            if (_currentPlayerIndex >= _activePlayers.Count)
            {
                _currentPlayerIndex = 0;
            }
        }
        else
        {
            _currentPlayerIndex--;

            if (_currentPlayerIndex < 0)
            {
                _currentPlayerIndex = _activePlayers.Count == 0
                    ? 0
                    : _activePlayers.Count - 1;
            }
        }
    }

    private int GetPlayer(int index)
    {
        if (_activePlayers.Count == 0)
        {
            throw new InvalidOperationException("There are no players to choose from");
        }

        index %= _activePlayers.Count;

        if (index < 0)
        {
            index += _activePlayers.Count;
        }

        return _activePlayers[index];
    }
    #endregion
}
