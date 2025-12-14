using System.Diagnostics;

namespace TurnsManagement;

/// <inheritdoc cref="ITurnsManager"/>
public sealed class TurnsManager : ITurnsManager
{
    private int _currentPlayerIndex;
    private readonly List<int> _activePlayers;

    /// <inheritdoc/>
    public int InitialPlayersCount { get; }
    /// <inheritdoc/>
    public IReadOnlyList<int> ActivePlayers => _activePlayers;

    /// <inheritdoc/>
    public int Current
    {
        get => GetPlayer(_currentPlayerIndex);
        set
        {
            ArgumentOutOfRangeException.ThrowIfLessThan(Current, 0);
            ArgumentOutOfRangeException.ThrowIfGreaterThanOrEqual(Current, InitialPlayersCount);

            _currentPlayerIndex = _activePlayers.IndexOf(value) switch
            {
                -1 => throw new InvalidOperationException(
                    $"Cannot set {nameof(Current)} player to {value}: " +
                    $"The player {value} does not exist or was removed"),
                int index => index,
            };
        }
    }

    /// <inheritdoc/>
    public int Previous =>
        Direction == TurnsDirection.Up
        ? GetPlayer(_currentPlayerIndex - 1)
        : GetPlayer(_currentPlayerIndex + 1);

    /// <inheritdoc/>
    public int Next =>
        Direction == TurnsDirection.Up
        ? GetPlayer(_currentPlayerIndex + 1)
        : GetPlayer(_currentPlayerIndex - 1);

    /// <inheritdoc/>
    public TurnsDirection Direction
    {
        get;
        set => field = Enum.IsDefined(value)
            ? value
            : throw new ArgumentOutOfRangeException($"Invalid '{nameof(Direction)}' value: {value}");
    }

    /// <summary>
    /// Initializes a new instance of the <see cref="TurnsManager"/> class.
    /// </summary>
    /// <param name="playersCount">The initial number of playing players.</param>
    /// <param name="direction">The initial playing direction.</param>
    public TurnsManager(int playersCount, TurnsDirection? direction = null)
    {
        ArgumentOutOfRangeException.ThrowIfLessThanOrEqual(playersCount, 0);

        if (direction.HasValue && !Enum.IsDefined(direction.Value))
        {
            throw new ArgumentException($"Invalid direction: {direction.Value}", nameof(direction));
        }

        InitialPlayersCount = playersCount;
        _activePlayers = [.. Enumerable.Range(0, playersCount)];

        if (direction.HasValue)
        {
            Direction = direction.Value;
        }
    }

    /// <inheritdoc/>
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

    /// <inheritdoc/>
    public int MoveNext() => Jump(1);

    /// <inheritdoc/>
    public TurnsDirection SwitchDirection() =>
        Direction = Direction switch
        {
            TurnsDirection.Up => TurnsDirection.Down,
            TurnsDirection.Down => TurnsDirection.Up,
            _ => throw new InvalidOperationException("Invalid direction"),
        };

    /// <inheritdoc/>
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
}
