using System;
using System.Collections.Generic;
using System.Text;

namespace Deck.Cards.FrenchSuited
{
	public struct Card: IEquatable<Card>
	{
		#region Consts

		public const int CARDS_COUNT = NO_JOKERS_CARDS_COUNT + 2;
		public const int NO_JOKERS_CARDS_COUNT = 52;
		#endregion

		#region Fields

		private readonly Suit suit;
		#endregion

		#region Properties

		public Value Value { get; }
		public readonly Suit? Suit => this.Value != Value.Joker
			? this.suit
			: null;

		public readonly Color Color => ColorBySuit(this.suit);
		#endregion

		#region Ctors

		public Card(Value value, Suit suit)
		{
			this.Value = value;
			this.suit = suit;
		}
		#endregion

		#region Methods

		public readonly Card With(Value? value = null, Suit? suit = null) =>
			GetCard(value ?? this.Value, suit ?? this.suit);

		public override readonly string ToString()
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

		public static Card GetCard(Value value, Suit suit)
		{
			return new Card(value, suit);
		}

		public static Card GetJoker(Color color) =>
			GetCard(Value.Joker, DefaultColorSuit(color));

		public static IEnumerable<Card> AllCards(bool excludeJokers = false)
		{
			for (Suit suit = default; suit <= FrenchSuited.Suit.Spades; suit++)
			{
				foreach (var card in FullSuit(suit))
				{
					yield return card;
				}
			}

			if (!excludeJokers)
			{
				yield return GetJoker(Color.Black);
				yield return GetJoker(Color.Red);
			}

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

		// override object.Equals
		public override readonly bool Equals(object? obj) => 
			obj is Card other && Equals(other);

		// override object.GetHashCode
		public override readonly int GetHashCode()
		{
			return this.Value != Value.Joker
				? (this.Value, this.Suit).GetHashCode()
				: (this.Value, this.Color).GetHashCode();
		}

		public readonly bool Equals(Card other)
		{
			if (this.Value != other.Value)
			{
				return false;
			}

			return this.Value != Value.Joker
				? this.Suit == other.Suit
				: this.Color == other.Color;
		}
		#endregion

		#region Operators

		public static bool operator ==(Card left, Card right)
		{
			return left.Equals(right);
		}

		public static bool operator !=(Card left, Card right)
		{
			return !(left == right);
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
