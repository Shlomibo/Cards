using System;
using DTOs.Shithead.Moves;

namespace DTOs.Shithead;

/// <summary>
/// An attempted move in the Shithead game.
/// </summary>
public record AttemptedMove
{
    /// <summary>
    /// The move being attempted.
    /// </summary>ary>
    public required ShitheadMove Move { get; init; }

    /// <summary>
    /// The ID of the player attempting the move, or <see langword="null"/>.
    /// </summary>
    public int? PlayerId { get; init; }
}
