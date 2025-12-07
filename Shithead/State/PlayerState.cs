using System.Diagnostics;

using Deck;
using Deck.Cards.FrenchSuited;

namespace Shithead.State;

/// <summary>
/// The state of each Shithead game player.
/// </summary>
public sealed class PlayerState
{
    /// <summary>
    /// The number of undercards each player has.
    /// </summary>
    public const int UndercardsCount = 3;

    /// <summary>
    /// Gets the Id of the player.
    /// </summary>
    public int Id { get; }

    /// <summary>
    /// Gets the player's hand.
    /// </summary>
    public CardsDeck Hand { get; } = [];

    /// <summary>
    /// Gets the player's undercards.
    /// </summary>
    public Dictionary<int, CardFace<Card>> Undercards { get; }

    /// <summary>
    /// Gets or sets a value indicating whether the player has left the game.
    /// </summary>
    public bool DidLeaveGame { get; set; } = false;

    /// <summary>
    /// Gets the player's revealed cards.
    /// </summary>
    /// <value></value>
    public Dictionary<int, Card> RevealedCards { get; } = [];

    /// <summary>
    /// Gets a value indicating whether the player has won the game.
    /// </summary>
    public bool Won =>
        !DidLeaveGame
        && Hand is []
        && RevealedCards is { Count: 0 }
        && Undercards is { Count: 0 };

    /// <summary>
    /// Gets a value indicating whether the player has accepted their revealed cards selection.
    /// </summary>
    public bool RevealedCardsAccepted { get; set; } = false;

    /// <summary>
    /// Initializes a new instance of the <see cref="PlayerState"/> class.
    /// </summary>
    /// <param name="undercards">The player's undercards.</param>
    /// <param name="id">The player's id.</param>
    public PlayerState(ICollection<Card> undercards, int id)
    {
        ArgumentNullException.ThrowIfNull(undercards);
        if (undercards.Count != UndercardsCount)
        {
            throw new ArgumentException($"Undercards count is {UndercardsCount}", nameof(undercards));
        }

        Id = id;
        Undercards = undercards
            .Select((card, index) => KeyValuePair.Create(index, (CardFace<Card>)card))
            .ToDictionary();
    }

    internal PlayerState(
        ICollection<Card> undercards,
        int id,
        CardsDeck hand,
        Dictionary<int, Card> revealedCards)
        : this(undercards, id)
    {
        Hand = hand ?? throw new ArgumentNullException(nameof(hand));
        RevealedCards = revealedCards ?? throw new ArgumentNullException(nameof(revealedCards));
    }

    /// <summary>
    /// Determines whether the player can set a revealed card.
    /// </summary>
    /// <param name="cardIndex">The index of the card.</param>
    /// <param name="target">The revealed card location.</param>
    /// <returns>
    /// <see langword="true"/> if the player can set the revealed card;
    /// otherwise, <see langword="false"/>.
    /// </returns>
    public bool CanSetRevealedCard(int cardIndex, int target) =>
        !RevealedCardsAccepted
        && RevealedCards.Count < UndercardsCount
        && cardIndex >= 0
        && cardIndex < Hand.Count
        && !RevealedCards.ContainsKey(target);

    /// <summary>
    /// Determines whether the player can unset a revealed card.
    /// </summary>
    /// <param name="cardIndex">The index of the revealed card to return to player's hand.</param>
    /// <returns>
    /// <see langword="true"/> if the player can unset the revealed card;
    /// otherwise, <see langword="false"/>.
    /// </returns>
    public bool CanUnsetRevealedCard(int cardIndex) =>
        !RevealedCardsAccepted
        && RevealedCards.Count > 0
        && cardIndex >= 0
        && RevealedCards.ContainsKey(cardIndex);

    /// <summary>
    /// Determines whether the player can accept their revealed cards selection.
    /// </summary>
    /// <returns>
    /// <see langword="true"/> if the player can accept the revealed cards selection;
    /// otherwise, <see langword="false"/>.
    /// </returns>
    public bool CanAcceptSelectedRevealedCards() =>
        !RevealedCardsAccepted
        && RevealedCards.Count == UndercardsCount;

    /// <summary>
    /// Determines whether the player can reselect their revealed cards during <see cref="GameState.Init"/>.
    /// </summary>
    /// <returns>
    /// </returns>
    public bool CanReselectRevealedCards() => RevealedCardsAccepted;

    /// <summary>
    /// Determines whether the player can place a joker card.
    /// </summary>
    /// <returns>
    /// <see langword="true"/> if the player can place a joker;
    /// otherwise, <see langword="false"/>.
    /// </returns>
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

    /// <summary>
    /// Determines whether the player can place the specified cards.
    /// </summary>
    /// <param name="cardIndices">The indices of the cards to check.</param>
    /// <returns>
    /// <see langword="true"/> if the player can place the specified cards;
    /// otherwise, <see langword="false"/>.
    /// </returns>
    public bool CanPlaceCard(IEnumerable<int> cardIndices)
    {
        return cardIndices.Any() && CanPlaceCardFromHand();

        bool CanPlaceCardFromHand()
        {
            if (cardIndices.Any(i => i < 0 || i >= Hand.Count))
            {
                return false;
            }

            var value = Hand[cardIndices.First()].Value;

            return cardIndices.Skip(1).All(i => Hand[i].Value == value);
        }
    }

    /// <summary>
    /// Determines if the player can reveal an undercard.
    /// </summary>
    /// <param name="cardIndex">The index of the card to reveal.</param>
    /// <returns>
    /// <see langword="true"/> if the player can reveal the undercard at <paramref name="cardIndex"/>;
    /// otherwise <see langword="false"/>.
    /// </returns>
    public bool CanRevealUndercard(int cardIndex) =>
        // Has no other cards
        this is
        {
            Hand.Count: 0,
            RevealedCards.Count: 0
        }
        // The undercard exists
        && Undercards.ContainsKey(cardIndex)
        // No other card is revealed
        && Undercards.Values.All(card => !card.IsRevealed);

    /// <summary>
    ///
    /// </summary>
    /// <param name="cardIndices"></param>
    /// <returns></returns>
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

    /// <summary>
    /// Gets the selected cards from the players hand.
    /// </summary>
    /// <param name="cardIndex">The index of the card to get.</param>
    /// <returns>The card at the <paramref name="cardIndex"/> location.</returns>
    public Card GetCard(int cardIndex) => Hand[cardIndex];

    /// <summary>
    ///Removes the card at the specified index from the player's hand.
    /// </summary>
    /// <param name="cardIndex">The index of the card to remove.</param>
    public void RemoveCard(int cardIndex) => Hand.RemoveAt(cardIndex);

    /// <summary>
    /// Removes a joker from the player's hand.
    /// </summary>
    public void RemoveJoker()
    {
        switch (this)
        {
            case { Hand.Count: > 0 }:
                RemoveFromList(
                    Hand,
                    GetJokerIndex(Hand, card => card.Value));

                break;
            case { RevealedCards.Count: > 0 }:
                RemoveFromDict(
                    RevealedCards,
                    GetJokerKey(RevealedCards, card => card.Value));

                break;
            default:
                RemoveFromDict(
                    Undercards,
                    GetJokerKey(
                        Undercards.Where(kv => kv.Value.IsRevealed),
                        card => card.Card.Value));
                break;
        }

        void RemoveFromDict<T>(IDictionary<int, T> list, int index)
        {
            if (index != -1)
            {
                list.Remove(index);
            }
        }

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
            Func<T, bool>? filter = null)
            =>
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
