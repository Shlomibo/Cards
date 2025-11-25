using System.Diagnostics;

using Deck;
using Deck.Cards.FrenchSuited;

namespace Shithead.State;

public sealed class PlayerState
{
    public const int UndercardsCount = 3;

    public int Id { get; }
    public CardsDeck Hand { get; } = [];

    public Dictionary<int, CardFace<Card>> Undercards { get; }
    public bool DidLeaveGame { get; set; } = false;
    public Dictionary<int, Card> RevealedCards { get; } = [];

    public bool Won =>
        !DidLeaveGame
        && Hand is []
        && RevealedCards is { Count: 0 }
        && Undercards is { Count: 0 };
    public bool RevealedCardsAccepted { get; set; } = false;

    public PlayerState(ICollection<Card> undercards, int id)
    {
        if (undercards.Count != UndercardsCount)
        {
            throw new ArgumentException($"Undercards count is {UndercardsCount}", nameof(undercards));
        }

        Id = id;
        Undercards = undercards
            .Select((card, index) => KeyValuePair.Create(index, new CardFace<Card>(card)))
            .ToDictionary();
    }

    public bool CanSetRevealedCard(int cardIndex, int target) =>
        !RevealedCardsAccepted
        && RevealedCards.Count < UndercardsCount
        && cardIndex >= 0
        && cardIndex < Hand.Count
        && !RevealedCards.ContainsKey(target);

    public bool CanUnsetRevealedCard(int cardIndex) =>
        !RevealedCardsAccepted
        && RevealedCards.Count > 0
        && cardIndex >= 0
        && RevealedCards.ContainsKey(cardIndex);

    public bool CanAcceptSelectedRevealedCards() =>
        !RevealedCardsAccepted
        && RevealedCards.Count == UndercardsCount;

    public bool CanReselectRevealedCards() => RevealedCardsAccepted;

    public bool CanPlaceJoker()
    {
        return this switch
        {
            { Hand.Count: > 0 } => Hand.Any(IsJoker),
            { RevealedCards.Count: > 0 } => RevealedCards.Values.Any(IsJoker),
            _ => Undercards.Values
                .Where(undercard => undercard.IsRevealed)
                .Select(undercard => undercard.Card)
                .Any(IsJoker),
        };

        static bool IsJoker(Card card) => card.Value == Value.Joker;
    }

    public bool CanPlaceCard(IEnumerable<int> cardIndices)
    {
        return cardIndices.Any() && CanPlaceCardFromList(Hand);

        bool CanPlaceCardFromList(IReadOnlyList<Card> cards)
        {
            if (cardIndices.Any(i => i < 0 || i >= cards.Count))
            {
                return false;
            }

            var value = cards[cardIndices.First()].Value;

            return cardIndices.Skip(1).All(i => cards[i].Value == value);
        }
    }

    public bool CanRevealUndercard(int cardIndex) =>
        this is
        {
            Hand.Count: 0,
            RevealedCards.Count: 0
        }
        && Undercards.ContainsKey(cardIndex)
        && Undercards.Values.All(card => !card.IsRevealed);

    public bool CanTakeUndercards(int[] cardIndices) =>
        (this, cardIndices) switch
        {
            ({ Hand.Count: > 0 }, _) or (_, { Length: 0 }) => false,
            ({ RevealedCards.Count: > 0 }, _) when !cardIndices.All(i => RevealedCards.ContainsKey(i)) => false,
            ({ RevealedCards.Count: > 0 }, _) => cardIndices
                .Skip(1)
                .Select(i => RevealedCards[i].Value)
                .Scan(
                    (v: RevealedCards[cardIndices[0]].Value, eq: true),
                    (state, currentCardValue) => (v: state.v, eq: state.v == currentCardValue))
                    .All(state => state.eq),
            (_, [int index]) when !Undercards.ContainsKey(index) => false,
            (_, [int index]) => Undercards[index].IsRevealed,
            _ => throw new UnreachableException(),
        };

    public Card GetCard(int cardIndex) => Hand[cardIndex];


    public void RemoveCard(int cardIndex) => Hand.RemoveAt(cardIndex);

    public void RemoveJoker()
    {
        RemoveFromList(Hand, GetJokerIndex(Hand, card => card.Value));

        void RemoveFromList<T>(IList<T> list, int index)
        {
            if (index != -1)
            {
                list.RemoveAt(index);
            }
        }

        int GetJokerIndex<T>(
            IEnumerable<T> list,
            Func<T, Value> cardValueSelection,
            Func<T, bool>? filter = null) =>
            GetJokerKey(
                list.Select((item, index) => KeyValuePair.Create(index, item)),
                cardValueSelection,
                filter);

        int GetJokerKey<T>(
            IEnumerable<KeyValuePair<int, T>> list,
            Func<T, Value> cardValueSelection,
            Func<T, bool>? filter = null)
        {
            if (filter != null)
            {
                list = list.Where(item => filter(item.Value));
            }

            return list
                .Where(item => cardValueSelection(item.Value) == Value.Joker)
                .Select(item => (int?)item.Key)
                .FirstOrDefault() ?? -1;
        }
    }
}
