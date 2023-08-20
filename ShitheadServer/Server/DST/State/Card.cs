using Deck.Cards.FrenchSuited;

namespace ShitheadServer.Server.DST.State
{
	public static class CardExtensions
	{
		public static object ToJsonObject(this Card card)
		{
			return card.Suit == null
				? new
				{
					value = card.Value,
					color = card.Color,
				}
				: new
				{
					value = card.Value,
					suit = card.Suit,
				};
		}
	}
}
