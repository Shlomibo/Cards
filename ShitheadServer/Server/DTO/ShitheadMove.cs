using GameServer.DTO;
using Shithead.ShitheadMove;

namespace ShitheadServer.Server.DTO;

public sealed class ShitheadMove : IMove
{
	private const string SHITHEAD_MOVES_NAMESPACE = "Shithead.ShitheadMove.";

	private static readonly IReadOnlySet<string> noPropertyMovements = new HashSet<string>
	{
		nameof(AcceptSelectedRevealedCards),
		nameof(ReselectRevealedCards),
		nameof(AcceptDiscardPile),
	};

	public string Move { get; set; } = "";
	public int CardIndex { get; set; }
	public int TargetIndex { get; set; }
	public int[] CardIndices { get; set; } = Array.Empty<int>();
	public int PlayerId { get; set; }

	public IShitheadMove ToGameMove()
	{
		return Move switch
		{
			nameof(RevealedCardSelection) => new RevealedCardSelection
			{
				CardIndex = CardIndex,
				TargetIndex = TargetIndex,
			},

			nameof(UnsetRevealedCard) => new UnsetRevealedCard
			{
				CardIndex = CardIndex,
			},

			nameof(AcceptSelectedRevealedCards) => new AcceptSelectedRevealedCards(),
			nameof(ReselectRevealedCards) => new ReselectRevealedCards(),

			nameof(PlaceCard) => new PlaceCard
			{
				CardIndices = CardIndices,
			},

			nameof(PlaceJoker) => new PlaceJoker
			{
				PlayerId = PlayerId,
			},

			nameof(AcceptDiscardPile) => new AcceptDiscardPile(),

			nameof(RevealUndercard) => new RevealUndercard
			{
				CardIndex = CardIndex,
			},

			nameof(TakeUndercards) => new TakeUndercards
			{
				CardIndices = CardIndices,
			},

			nameof(LeaveGame) => new LeaveGame
			{
				PlayerId = PlayerId,
			},

			_ => throw new InvalidOperationException($"Unknown game move: {Move}"),
		};
	}

	public static ShitheadMove FromGameMove(IShitheadMove move)
	{
		return move switch
		{
			RevealedCardSelection
			{
				CardIndex: int cardIndex,
				TargetIndex: int targetIndex,
			} => new ShitheadMove
			{
				Move = nameof(RevealedCardSelection),
				CardIndex = cardIndex,
				TargetIndex = targetIndex,
			},

			UnsetRevealedCard
			{
				CardIndex: int cardIndex,
			} => new ShitheadMove
			{
				Move = nameof(UnsetRevealedCard),
				CardIndex = cardIndex,
			},

			PlaceCard
			{
				CardIndices: var cardIndices,
			} => new ShitheadMove
			{
				Move = nameof(PlaceCard),
				CardIndices = cardIndices,
			},

			PlaceJoker
			{
				PlayerId: int playerId,
			} => new ShitheadMove
			{
				Move = nameof(PlaceJoker),
				PlayerId = playerId,
			},

			RevealUndercard
			{
				CardIndex: int cardIndex
			} => new ShitheadMove
			{
				Move = nameof(RevealUndercard),
				CardIndex = cardIndex,
			},

			TakeUndercards
			{
				CardIndices: var cardIndices,
			} => new ShitheadMove
			{
				Move = nameof(TakeUndercards),
				CardIndices = cardIndices,
			},

			LeaveGame
			{
				PlayerId: int playerId,
			} => new ShitheadMove
			{
				Move = nameof(LeaveGame),
				PlayerId = playerId,
			},

			_ when noPropertyMovements.Contains(move.GetType().Name) => new ShitheadMove
			{
				Move = move.GetType().Name,
			},

			_ => throw new InvalidOperationException($"Unknown game move: {move.GetType().FullName}"),
		};
	}

	public object ToJsonObject()
	{
		return Move switch
		{
			nameof(RevealedCardSelection) => new
			{
				move = Move,
				cardIndex = CardIndex,
				targetIndex = TargetIndex,
			},

			nameof(UnsetRevealedCard) or
			nameof(RevealUndercard) => new
			{
				move = Move,
				cardIndex = CardIndex,
			},

			nameof(PlaceCard) or
			nameof(TakeUndercards) => new
			{
				move = Move,
				cardIndices = CardIndices,
			},

			nameof(PlaceJoker) or
			nameof(LeaveGame) => new
			{
				move = Move,
				playerId = PlayerId,
			},

			_ when noPropertyMovements.Contains(Move) => new
			{
				move = Move,
			},

			_ => throw new InvalidOperationException($"Unknown game move: {Move}"),

		};
	}
}
