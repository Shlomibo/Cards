namespace Deck
{
	public sealed class CardFace<TCard>
		where TCard : new()
	{
		#region Fields

		private readonly TCard card;
		#endregion

		#region Properties

		public TCard Card => this.IsRevealed
			? this.card
			: new TCard();
		public bool IsRevealed { get; set; }
		#endregion

		#region Ctors
		public CardFace(TCard card, bool isRevealed = false)
		{
			this.card = card;
			this.IsRevealed = isRevealed;
		}
		#endregion
	}
}
