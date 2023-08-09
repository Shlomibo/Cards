using Deck.Cards.FrenchSuited;
using System;
using System.Collections.Generic;

namespace Shithead.State
{
	public sealed partial class ShitheadState
	{
		public sealed class Shared
		{
			private readonly ShitheadState state;

			public Shared(ShitheadState state)
			{
				this.state = state;
			}
		}
	}
}
