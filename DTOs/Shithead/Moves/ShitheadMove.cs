using System;
using System.Text.Json.Serialization;

namespace DTOs.Shithead.Moves;

/// <summary>
/// A move in the Shithead game.
/// </summary>
[JsonPolymorphic]
[JsonDerivedType(typeof(PlaceCard), nameof(PlaceCard))]
[JsonDerivedType(typeof(PlaceJoker), nameof(PlaceJoker))]
[JsonDerivedType(typeof(AcceptDiscardPile), nameof(AcceptDiscardPile))]
[JsonDerivedType(typeof(RevealUndercard), nameof(RevealUndercard))]
[JsonDerivedType(typeof(TakeRevealedCards), nameof(TakeRevealedCards))]
[JsonDerivedType(typeof(TakeUndercard), nameof(TakeUndercard))]
[JsonDerivedType(typeof(LeaveGame), nameof(LeaveGame))]
[JsonDerivedType(typeof(SetRevealedCard), nameof(SetRevealedCard))]
[JsonDerivedType(typeof(UnsetRevealedCard), nameof(UnsetRevealedCard))]
[JsonDerivedType(typeof(AcceptSelectedRevealedCards), nameof(AcceptSelectedRevealedCards))]
[JsonDerivedType(typeof(ReselectRevealedCards), nameof(ReselectRevealedCards))]
public abstract record ShitheadMove;
