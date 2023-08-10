using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Shithead.ShitheadMove
{
	internal sealed class PlaceCard : IShitheadMove
	{
		/// <summary>
		/// The index of the card in the player's hand
		/// </summary>
		public int[] CardIndices { get; set; } = Array.Empty<int>();
	}

	internal sealed class PlaceJoker : IShitheadMove
	{
		/// <summary>
		/// The Id of the player that accepts the discard pile
		/// </summary>
		public int PlayerId { get; set; }
	}

	internal sealed class AcceptDiscardPile : IShitheadMove
	{
	}
}
