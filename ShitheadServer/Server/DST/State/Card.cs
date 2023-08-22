using Deck.Cards.FrenchSuited;

namespace ShitheadServer.Server.DST.State
{
	public static class CardExtensions
	{
		private static readonly Dictionary<Value, string> cardValues = new(
			// Cards 2 through 10
			Enumerable.Range(2, 9).Select(value => new KeyValuePair<Value, string>((Value)value, value.ToString()))
		);

		public static object ToJsonObject(this Card card)
		{
			return card.Suit == null
				? new
				{
					value = CardValue(card.Value),
					color = card.Color.ToString(),
				}
				: new
				{
					value = CardValue(card.Value),
					suit = card.Suit.ToString(),
				};
		}

		private static string CardValue(Value value) =>
			cardValues.TryGetValue(value, out var name)
			? name
			: value.ToString();
	}
}
