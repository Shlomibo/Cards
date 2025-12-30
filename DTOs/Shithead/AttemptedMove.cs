using System;
using DTOs.Shithead.Moves;

namespace DTOs.Shithead;

public record AttemptedMove
{
    public required ShitheadMove Move { get; init; }
    public int? PlayerId { get; init; }
}
