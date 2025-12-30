using System;

namespace DTOs.Responses;

public record CanJoinTableResponse
{
    public bool CanJoin { get; init; }
}
