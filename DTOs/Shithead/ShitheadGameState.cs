using System;

namespace DTOs.Shithead;

/// <summary>
/// The state of a Shithead game.
/// </summary>
public record ShitheadGameState : State<SharedState, PlayerState>;
