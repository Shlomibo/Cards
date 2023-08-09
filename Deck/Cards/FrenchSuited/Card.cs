using System;
using System.Collections.Generic;
using System.Text;

namespace Deck.Cards.FrenchSuited
{
	public sealed class Card
	{
		#region Consts

		public const int CARDS_COUNT = NO_JOKERS_CARDS_COUNT + 2;
		public const int NO_JOKERS_CARDS_COUNT = 52;

		private const int SUITS_COUNT = 4;
		#endregion

		#region Fields

		private readonly Suit suit;
		private static readonly Card?[] allCards = new Card[CARDS_COUNT];
		#endregion

		#region Properties

		public Value Value { get; }
		public Suit? Suit => this.Value != Value.Joker
			? this.suit
			: (Suit?)null;

		public Color Color => ColorBySuit(this.suit);
		#endregion

		#region Ctors

		private Card(Value value, Suit suit)
		{
			this.Value = value;
			this.suit = suit;
		}
		#endregion

		#region Methods

		public Card With(Value? value = null, Suit? suit = null) =>
			GetCard(value ?? this.Value, suit ?? this.suit);

		public override string ToString()
		{
			if (this is { Value: Value.Joker })
			{
				return $"{this.Color} Joker";
			}
			else if (this is { Value: Value.Ace } ||
				(this.Value >= Value.Jack && this.Value <= Value.King))
			{
				return $"{this.Value} of {this.Suit}";
			}
			else
			{
				return $"{this.Value} {this.Suit}";
			}
		}

		public static Color ColorBySuit(Suit suit) =>
			(Color)((int)suit & 0x2);

		public static Suit DefaultColorSuit(Color color) =>
			(Suit)(int)color;

		private static int CardIndex(Value value, Suit suit)
		{
			if (value == Value.Joker)
			{
				return NO_JOKERS_CARDS_COUNT + (int)suit / 2;
			}
			else
			{
				return ((int)value - 1) * SUITS_COUNT + (int)suit;
			}
		}

		public static Card GetCard(Value value, Suit suit)
		{
			int index = CardIndex(value, suit);

			if (allCards[index] is null)
			{
				allCards[index] = new Card(value, suit);
			}

			return allCards[index]!;
		}

		public static Card GetJoker(Color color) =>
			GetCard(Value.Joker, DefaultColorSuit(color));

		//public static IEnumerable<Card> FullSuit(Suit)

		public static IEnumerable<Card> AllCards()
		{
			for (Suit suit = default; suit <= FrenchSuited.Suit.Spades; suit++)
			{
                foreach (var card in FullSuit(suit))
                {
					yield return card;
                }
            }

			yield return GetJoker(Color.Black);
			yield return GetJoker(Color.Red);

		}

		public static IEnumerable<Card> FullSuit(Suit suit)
		{
			for (var value = Value.Ace; value <= Value.King; value++)
			{
				yield return GetCard(value, suit);
			}
		}

		public static IEnumerable<Card> AllSuits(Value value)
		{
			yield return GetCard(value, FrenchSuited.Suit.Clubs);
			yield return GetCard(value, FrenchSuited.Suit.Spades);
			yield return GetCard(value, FrenchSuited.Suit.Diamonds);
			yield return GetCard(value, FrenchSuited.Suit.Hearts);
		}
		#endregion
	}

	public enum Suit
	{
		Hearts,
		Diamonds,
		Clubs,
		Spades,
	}

	public enum Color
	{
		Red,
		// So we can bit-mask from suit to color
		Black = 0x2,
	}

	public enum Value
	{
		Joker,
		Ace,
		Two,
		Three,
		Four,
		Five,
		Six,
		Sevem,
		Eight,
		Nine,
		Ten,
		Jack,
		Queen,
		King,
	}
}
