using Deck.Cards.FrenchSuited;
using System;
using System.Collections.Generic;

namespace Shithead.State
{
	public sealed partial class ShitheadState
	{
		public sealed class Player
		{
			private readonly int playerId;
			private readonly ShitheadState state;

			public Player(int playerId, ShitheadState state)
            {
				this.playerId = playerId;
				this.state = state;
			}
        }
	}
}
