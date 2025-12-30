using System;
using DTOs.Cards.FrenchSuited;

namespace DTOs.Shithead;

public abstract record PlayerStateBase
{
    public int PlayerId { get; init; }
    public bool Won { get; init; }
    public bool RevealedCardsAccepted { get; init; }
    public required Dictionary<int, Card> RevealedCards { get; init; }
    public required Dictionary<int, Card?> Undercards { get; init; }

}
