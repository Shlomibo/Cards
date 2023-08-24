using GameServer.DTO;
using Shithead.ShitheadMove;

namespace ShitheadServer.Server.DTO
{
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
			return this.Move switch
			{
				nameof(RevealedCardSelection) => new RevealedCardSelection
				{
					CardIndex = this.CardIndex,
					TargetIndex = this.TargetIndex,
				},

				nameof(UnsetRevealedCard) => new UnsetRevealedCard
				{
					CardIndex = this.CardIndex,
				},

				nameof(AcceptSelectedRevealedCards) => new AcceptSelectedRevealedCards(),
				nameof(ReselectRevealedCards) => new ReselectRevealedCards(),

				nameof(PlaceCard) => new PlaceCard
				{
					CardIndices = this.CardIndices,
				},

				nameof(PlaceJoker) => new PlaceJoker
				{
					PlayerId = this.PlayerId,
				},

				nameof(AcceptDiscardPile) => new AcceptDiscardPile(),

				nameof(RevealUndercard) => new RevealUndercard
				{
					CardIndex = this.CardIndex,
				},

				nameof(TakeUndercards) => new TakeUndercards
				{
					CardIndices = this.CardIndices,
				},

				nameof(LeaveGame) => new LeaveGame
				{
					PlayerId = this.PlayerId,
				},

				_ => throw new InvalidOperationException($"Unknown game move: {this.Move}"),
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
			return this.Move switch
			{
				nameof(RevealedCardSelection) => new
				{
					move = this.Move,
					cardIndex = this.CardIndex,
					targetIndex = this.TargetIndex,
				},

				nameof(UnsetRevealedCard) or
				nameof(RevealUndercard) => new
				{
					move = this.Move,
					cardIndex = this.CardIndex,
				},

				nameof(PlaceCard) or
				nameof(TakeUndercards) => new
				{
					move = this.Move,
					cardIndices = this.CardIndices,
				},

				nameof(PlaceJoker) or
				nameof(LeaveGame) => new
				{
					move = this.Move,
					playerId = this.PlayerId,
				},

				_ when noPropertyMovements.Contains(this.Move) => new
				{
					move = this.Move,
				},

				_ => throw new InvalidOperationException($"Unknown game move: {this.Move}"),

			};
		}
	}
}
