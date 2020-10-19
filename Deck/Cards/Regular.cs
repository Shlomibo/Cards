using System.Collections.Generic;

namespace Deck.Cards.Regular
{
	public sealed class Card 
	{
		public const int CARDS_COUNT =NO_JOKERS_CARDS_COUNT + 2;
		public const int NO_JOKERS_CARDS_COUNT = 52;

		private const int SHAPES_COUNT = 4;
		#region Fields

		private readonly Shape shape;
		private static readonly Card?[] allCards = new Card[54];
		#endregion

		#region Properties

		public Value Value { get; }
		public Shape? Shape => this.Value != Value.Joker
			? this.shape
			: (Shape?)null;

		public Color Color => ColorByShape(this.shape);
		#endregion

		#region Ctors

		private Card(Value value, Shape shape)
		{
			this.Value = value;
			this.shape = shape;
		}
		#endregion

		#region Methods

		public Card With(Value? value = null, Shape? shape = null) =>
			GetCard(value ?? this.Value, shape ?? this.shape);

		public override string ToString()
		{
			if (this is { Value: Value.Joker})
			{
				return $"{this.Color} Joker";
			}
			else if (this is {Value: Value.Ace } || 
				(this.Value >= Value.Jack && this.Value <= Value.King))
			{
				return $"{this.Value} of {this.Shape}";
			}
			else
			{
				return $"{this.Value} {this.Shape}";
			}
		}

		public static Color ColorByShape(Shape shape) =>
			shape < Regular.Shape.Diamonds
			? Color.Black
			: Color.Red;

		public static Shape DefaultColorShape(Color color) =>
			color == Color.Black
				? Regular.Shape.Clubs
				: Regular.Shape.Diamonds;

		private static int CardIndex(Value value, Shape shape)
		{
			if (value == Value.Joker)
			{
				return NO_JOKERS_CARDS_COUNT + (int)shape / 2;
			}
			else
			{
				return ((int)value - 1) * SHAPES_COUNT + (int)shape;
			}
		}

		public static Card GetCard(Value value, Shape shape)
		{
			int index = CardIndex(value, shape);

			if (allCards[index] == null)
			{
				allCards[index] = new Card(value, shape);
			}

			return allCards[index]!;
		}

		public static Card GetJoker(Color color) =>
			GetCard(Value.Joker, DefaultColorShape(color));

		public static IEnumerable<Card> AllCards()
		{
			for (Value value = Value.Two; value <= Value.King; value++)
			{
				foreach (var card in AllShapes(value))
				{
					yield return card;
				}
			}

			foreach (var card in AllShapes(Value.Ace))
			{
				yield return card;
			}

			yield return GetJoker(Color.Black);
			yield return GetJoker(Color.Red);

			static IEnumerable<Card> AllShapes(Value value)
			{
				yield return GetCard(value, Regular.Shape.Clubs);
				yield return GetCard(value, Regular.Shape.Spades);
				yield return GetCard(value, Regular.Shape.Diamonds);
				yield return GetCard(value, Regular.Shape.Hearts);
			}
		}
		#endregion
	}

	public enum Shape
	{
		Clubs,
		Spades,
		Diamonds,
		Hearts,
	}

	public enum Color
	{
		Black,
		Red,
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
