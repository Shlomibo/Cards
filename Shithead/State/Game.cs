using Deck;
using Deck.Cards.Regular;
using System.Collections.Generic;
using TurnsManagement;

namespace Shithead.State
{
	using static Consts;

	public sealed class Game
	{
		#region Properties

		public ITurnsManager TurnsManager { get; }
		public IDeck<Card> MainDeck { get; } = new CardsDeck<Card>(Card.AllCards());
		public Card[,] DownFacingCards { get; }
		public Card[,] UpFacingCards { get; }
		public IDeck<Card>[] PlayersDecks { get; }
		public bool[,] DownFacingCardsVisibilty { get; }
		public IList<Card> Pile { get; } = new List<Card>();
		#endregion

		#region Ctors

		public Game(int playersCount)
		{
			this.TurnsManager = new TurnsManager(playersCount);
			this.MainDeck.Shuffle();

			this.DownFacingCardsVisibilty = new bool[playersCount, LAST_CARDS_COUNT];
			this.DownFacingCards = new Card[playersCount, LAST_CARDS_COUNT];
			this.UpFacingCards = new Card[playersCount, LAST_CARDS_COUNT];

			InitLastCardsDeck(playersCount, this.DownFacingCards, this.MainDeck);

			this.PlayersDecks = new IDeck<Card>[playersCount];
			DrawPlayersCards(this.MainDeck, this.PlayersDecks);

			static void InitLastCardsDeck(int playersCount, Card[,] cards, IDeck<Card> deck)
			{
				for (int cardIndex = 0; cardIndex < LAST_CARDS_COUNT; cardIndex++)
				{
					for (int player = 0; player < playersCount; player++)
					{
						cards[player, cardIndex] = deck.Pop();
					}
				}
			}

			static void DrawPlayersCards(IDeck<Card> mainDeck, IDeck<Card>[] playersDecks)
			{
				for (int cardNumber = 0; cardNumber < INITIAL_PLAYER_CARDS; cardNumber++)
				{
					for (int player = 0; player < playersDecks.Length; player++)
					{
						playersDecks[player].Push(mainDeck.Pop());
					}
				}
			}
		} 
		#endregion
	}
}
