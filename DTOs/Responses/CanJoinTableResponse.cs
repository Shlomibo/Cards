using System;

namespace DTOs.Responses;

/// <summary>
/// The response for checking if the player can join a table.
/// </summary>
public record CanJoinTableResponse
{
    /// <summary>
    /// Indicates whether the player can join the table.
    /// </summary>
    public bool CanJoin { get; init; }
}
