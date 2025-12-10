using Deck.Cards.FrenchSuited;

using Shithead;
using Shithead.Moves;
using Shithead.State;

if (args.Length == 0 || !int.TryParse(args[0], out int playersCount))
{
    string playersCountStr;

    do
    {
        Console.WriteLine("Please enter how many players are playing:");
        playersCountStr = Console.ReadLine()!;
    }
    while (!int.TryParse(playersCountStr, out playersCount) || playersCount < 3);
}

var engine = ShitheadGame.CreateGame(playersCount);
engine.Updated += (_, _) => Console.WriteLine("Game updated!");
var state = engine.State;

while (state.GameState == GameState.Init)
{
    int selectedPlayer;

    do
    {
        Console.WriteLine($"Please select a player (0 - {playersCount - 1}):");
    }
    while (!int.TryParse(Console.ReadLine(), out selectedPlayer) ||
        selectedPlayer < 0 ||
        selectedPlayer >= playersCount);

    var player = engine.Players[selectedPlayer];

    Console.WriteLine($"Player {selectedPlayer}: " +
        PrintPlayer(player.State));

    if (player.State.RevealedCardsAccepted)
    {
        Console.WriteLine("Do you want to change your selection? (y/N)");

        if (Console.ReadLine()!.ToLowerInvariant() == "y")
        {
            player.PlayMove(new ReselectRevealedCards());
        }
    }

    bool selectRevealedCard = true;

    if (selectRevealedCard)
    {
        string action;

        if (player.State.RevealedCards.Count == 0)
        {
            action = "a";
        }
        else if (player.State.RevealedCards.Count == 3)
        {
            action = "r";
        }
        else
        {
            do
            {
                Console.WriteLine("Please choose (a) to add a card to the revealed cards or (r) to remove a card");
                action = Console.ReadLine()!;
            }
            while (action.ToLowerInvariant() != "a" && action.ToLowerInvariant() != "r");
        }

        if (action == "a")
        {
            while (player.State.RevealedCards.Count < 3)
            {
                int selectedCard;

                do
                {
                    Console.WriteLine($"Select card (0 - {player.State.Hand.Count - 1}):");
                } while (!int.TryParse(Console.ReadLine(), out selectedCard) ||
                    selectedCard < 0 ||
                    selectedCard >= player.State.Hand.Count);

                int selectedTarget;

                do
                {
                    Console.WriteLine($"Select a spot (0 - 2):");
                }
                while (!int.TryParse(Console.ReadLine(), out selectedTarget) ||
                    selectedTarget < 0 ||
                    selectedTarget > 2);

                player.PlayMove(new SetRevealedCard(selectedCard, selectedTarget));

                Console.WriteLine(PrintPlayer(player.State));
            }

            Console.WriteLine("Do you want to accept your revealed cards? (y/N)");

            if (Console.ReadLine()!.ToLowerInvariant() == "y")
            {
                selectRevealedCard = false;
                player.PlayMove(new AcceptSelectedRevealedCards());
            }
        }
        else
        {
            if (player.State.RevealedCardsAccepted)
            {
                Console.WriteLine("Do you want to change your revealed cards? y/N");

                if (Console.ReadLine() == "y")
                {
                    player.PlayMove(new ReselectRevealedCards());
                }
            }

            while (!player.State.RevealedCardsAccepted && player.State.RevealedCards.Count > 0)
            {
                int selectedTarget;

                do
                {
                    Console.WriteLine($"Select a spot (0 - 2):");
                }
                while (!int.TryParse(Console.ReadLine(), out selectedTarget) ||
                    selectedTarget < 0 ||
                    selectedTarget > 2);

                player.PlayMove(new UnsetRevealedCard(selectedTarget));

                Console.WriteLine(PrintPlayer(player.State));
            }
        }
    }
}

while (state.GameState != GameState.GameOver)
{
    Console.WriteLine($"This is {state.CurrentTurnPlayer} turn...");
    var player = engine.Players[state.CurrentTurnPlayer];

    Console.WriteLine(PrintBoard());
    Console.WriteLine(PrintPlayer(player.State));
    Console.WriteLine();

    string action;

    do
    {
        Console.WriteLine("Please select a list of cards to put on the pile -\n" +
            "or 'fuck me' to take the pile");
        action = Console.ReadLine()!;
    }
    while (action.ToLowerInvariant() != "fuck me" &&
        (action.Length == 0 || action.Split(' ').Any(val =>
            !int.TryParse(val, out int card) ||
            card < 0 ||
            (player.State.Hand.Count > 0 && card >= player.State.Hand.Count) ||
            (player.State.Hand.Count == 0 &&
                player.State.RevealedCards.Count > 0 &&
                !player.State.RevealedCards.ContainsKey(card)) ||
            (player.State.Hand.Count == 0 &&
                player.State.RevealedCards.Count == 0 &&
                player.State.Undercards.Count > 0 &&
                !player.State.Undercards.ContainsKey(card)))));

    if (action.ToLowerInvariant() == "fuck me")
    {
        player.PlayMove(new AcceptDiscardPile());
    }
    else
    {
        var selected = action.Split(' ')
            .Select(val => int.Parse(val))
            .ToArray();

        if (player.State.Hand.Count == 0)
        {
            if (player.State.RevealedCards.Count == 0 &&
            selected.Length == 1)
            {
                Console.WriteLine("Revealing card...");
                player.PlayMove(new RevealUndercard(selected[0]));

                Console.WriteLine(PrintPlayer(player.State));
            }

            Console.WriteLine("Taking selected cards to hand...");
            player.PlayMove(new TakeUndercards(selected));
        }
        else if (selected.Length != 1 || player.State.GetCard(selected[0]).Value != Value.Joker)
        {
            player.PlayMove(new PlaceCard(selected));
        }
        else
        {
            int selectedTarget;

            do
            {
                Console.WriteLine("Please select a target from the list to send them the pile " +
                    $"({string.Join(' ', state.ActivePlayers)}):");
            }
            while (!int.TryParse(Console.ReadLine(), out selectedTarget) ||
                !state.ActivePlayers.Contains(selectedTarget));

            player.PlayMove(new PlaceJoker(selectedTarget));
        }
    }

    if (player.State.Won)
    {
        Console.WriteLine($"Player {player.PlayerId} won!");
    }
}

Console.WriteLine("Game over");
var shithead = engine.Players.Single(player => !player.State.Won);
Console.WriteLine($"The player {shithead.PlayerId} is shithead!");

#if DEBUG
Console.ReadLine();
#endif

string PrintBoard() =>
    $"Cards in deck: {state!.DeckSize}\n" +
    $"Pile ({state.DiscardPile.Count}): {PrintPile()}\n" +
    $"Players:\n\t" +
    string.Join("\n\t", state.Players.Select(PrintSharedPlayer));

string PrintPile() =>
    state!.DiscardPile.Count == 0
    ? "X"
    : $"{state.DiscardPile.Top} ({state.DiscardPile.TopCardValue()})";

string PrintSharedPlayer(ShitheadState.SharedPlayerState player) =>
    $"{player.Id} (hand: {player.CardsCount}): {string.Join(' ', EnumerateDict(player.RevealedCards))} " +
    PrintUndercards(player.Undercards);

IEnumerable<T> EnumerateDict<T>(IReadOnlyDictionary<int, T> dict) =>
    from kv in dict
    orderby kv.Key
    select kv.Value;

string PrintPlayer(ShitheadState.ShitheadPlayerState player)
{
    string hand = player.Hand.Count == 0
    ? "[ ]"
    : string.Join(' ', player.Hand.Select((card, i) => $"{i}:{card}"));

    string revealed = "\t" + string.Join(' ',
        from i in Enumerable.Range(0, 3)
        select player.RevealedCards.ContainsKey(i)
            ? player.RevealedCards[i].ToString()
            : "[ ]");

    string undercards = "\t" + PrintUndercards(player.Undercards);

    return string.Join('\n', hand, revealed, undercards);
}

string PrintUndercards(IReadOnlyDictionary<int, Card?> undercards) =>
    string.Join(' ',
        from i in Enumerable.Range(0, 3)
        select !undercards.ContainsKey(i)
        ? "[ ]"
        : undercards[i]?.ToString() ?? "[○]");
