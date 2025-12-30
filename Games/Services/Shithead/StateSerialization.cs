using System;
using DTOs.Cards.FrenchSuited;
using Deck.Cards.FrenchSuited;
using DTOs.Shithead;
using Riok.Mapperly.Abstractions;
using Shithead.State;
using PlayerState = DTOs.Shithead.PlayerState;
using CardDto = DTOs.Cards.FrenchSuited.Card;
using Card = Deck.Cards.FrenchSuited.Card;
using Shithead.Moves;
using DTOs.Shithead.Moves;

namespace Games.Services.Shithead;

[Mapper(EnumMappingStrategy = EnumMappingStrategy.ByName)]
internal static partial class StateSerialization
{
    public static ShitheadGameState Map(
        ShitheadState.SharedShitheadState sharedState,
        ShitheadState.ShitheadPlayerState playerState)
        =>
        new()
        {
            SharedState = Map(sharedState),
            PlayerState = Map(playerState),
        };

    private static partial SharedState Map(ShitheadState.SharedShitheadState sharedState);

    [MapProperty(nameof(playerState.Id), nameof(OtherPlayersState.PlayerId))]
    private static partial OtherPlayersState Map(ShitheadState.SharedPlayerState playerState);

    [MapperIgnoreSource(nameof(playerState.GameState))]
    private static partial PlayerState Map(ShitheadState.ShitheadPlayerState playerState);
    private static CardDto? Map(Card? card) => card.HasValue
        ? Map(card.Value)
        : null;
    private static partial CardDto Map(Card card);

    private static AttemptedMove Map((Move move, int? playerId) m) => new()
    {
        Move = Map(m.move),
        PlayerId = m.playerId,
    };

#pragma warning disable RMG066 // No members are mapped in an object mapping
    [MapDerivedType<global::Shithead.Moves.PlaceCard, DTOs.Shithead.Moves.PlaceCard>]
    [MapDerivedType<global::Shithead.Moves.PlaceJoker, DTOs.Shithead.Moves.PlaceJoker>]
    [MapDerivedType<global::Shithead.Moves.AcceptDiscardPile, DTOs.Shithead.Moves.AcceptDiscardPile>]
    [MapDerivedType<global::Shithead.Moves.RevealUndercard, DTOs.Shithead.Moves.RevealUndercard>]
    [MapDerivedType<global::Shithead.Moves.TakeRevealedCards, DTOs.Shithead.Moves.TakeRevealedCards>]
    [MapDerivedType<global::Shithead.Moves.TakeUndercard, DTOs.Shithead.Moves.TakeUndercard>]
    [MapDerivedType<global::Shithead.Moves.LeaveGame, DTOs.Shithead.Moves.LeaveGame>]
    [MapDerivedType<global::Shithead.Moves.SetRevealedCard, DTOs.Shithead.Moves.SetRevealedCard>]
    [MapDerivedType<global::Shithead.Moves.UnsetRevealedCard, DTOs.Shithead.Moves.UnsetRevealedCard>]
    [MapDerivedType<global::Shithead.Moves.AcceptSelectedRevealedCards, DTOs.Shithead.Moves.AcceptSelectedRevealedCards>]
    [MapDerivedType<global::Shithead.Moves.ReselectRevealedCards, DTOs.Shithead.Moves.ReselectRevealedCards>]
    private static partial ShitheadMove Map(Move move);
#pragma warning restore RMG066 // No members are mapped in an object mapping
}
