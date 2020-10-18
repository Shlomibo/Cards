using Deck;
using Deck.Cards.Regular;
using System;
using System.Linq;

namespace Checks
{
	class Program
	{
		static void Main(string[] args)
		{
			var d = new CardsDeck<Card>(Card.AllCards());

			d.Shuffle();

			foreach (var c in d)
			{
				Console.WriteLine(c);
			}
		}
	}
}
