namespace Shithead.Moves;

/// <summary>
/// Removes the selected <paramref name="CardIndex"/> from the player's hand
/// and places it on the revealed cards at <paramref name="TargetIndex"/> location.
/// </summary>
/// <param name="CardIndex">Card index in player deck.</param>
/// <param name="TargetIndex">The revealed card index to send the card to</param>
public sealed record RevealedCardSelection(
    int CardIndex,
    int TargetIndex) : Move;

/// <summary>
/// Removes the selected <paramref name="CardIndex"/> from the player's revealed cards
/// and places it back to the player's hand.
/// </summary>
/// <param name="CardIndex">The index of the revealed card to remove.</param>
public sealed record UnsetRevealedCard(int CardIndex) : Move;

/// <summary>
/// Accepts the revealed cards selection and wait for the others to select theirs.
/// </summary>
public sealed record AcceptSelectedRevealedCards : Move;

/// <summary>
/// Reject the previously accepted revealed cards, allowing the player to change their selection.
/// </summary>
public sealed record ReselectRevealedCards : Move;
