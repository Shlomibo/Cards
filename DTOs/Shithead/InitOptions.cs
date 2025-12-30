using System.ComponentModel.DataAnnotations;

namespace DTOs.Shithead;

/// <summary>
/// Shithead table initialization options.
/// </summary>
public record InitOptions
{
    /// <summary> The minimum number of players allowed in a Shithead game.</summary>
    public const int MinPlayersCount = 2;

    /// <summary> The number of players in the game.</summary>
    [Range(MinPlayersCount, int.MaxValue)]
    public int PlayersCount { get; set; }
}
