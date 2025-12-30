using System;
using Riok.Mapperly.Abstractions;
using Shithead.Moves;

namespace Games.Services.Shithead;

[Mapper]
internal static partial class MoveDeserialization
{
#pragma warning disable RMG066 // No members are mapped in an object mapping
    [MapDerivedType<DTOs.Shithead.Moves.PlaceCard, PlaceCard>]
    [MapDerivedType<DTOs.Shithead.Moves.PlaceJoker, PlaceJoker>]
    [MapDerivedType<DTOs.Shithead.Moves.AcceptDiscardPile, AcceptDiscardPile>]
    [MapDerivedType<DTOs.Shithead.Moves.RevealUndercard, RevealUndercard>]
    [MapDerivedType<DTOs.Shithead.Moves.TakeRevealedCards, TakeRevealedCards>]
    [MapDerivedType<DTOs.Shithead.Moves.TakeUndercard, TakeUndercard>]
    [MapDerivedType<DTOs.Shithead.Moves.LeaveGame, LeaveGame>]
    [MapDerivedType<DTOs.Shithead.Moves.SetRevealedCard, SetRevealedCard>]
    [MapDerivedType<DTOs.Shithead.Moves.UnsetRevealedCard, UnsetRevealedCard>]
    [MapDerivedType<DTOs.Shithead.Moves.AcceptSelectedRevealedCards, AcceptSelectedRevealedCards>]
    [MapDerivedType<DTOs.Shithead.Moves.ReselectRevealedCards, ReselectRevealedCards>]
    public static partial Move Map(DTOs.Shithead.Moves.ShitheadMove move);
#pragma warning restore RMG066 // No members are mapped in an object mapping
}
