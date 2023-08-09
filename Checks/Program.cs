using Deck;
using Deck.Cards.FrenchSuite;
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
			Console.WriteLine(d.Count);

			var d1 = d.AsEnumerable().ToArray();
			d.Shuffle();
			var d2 = d.ToArray();

			for (int i = 0; i <d.Count; i++)
			{
				Console.WriteLine($"{d1[i]} <-> {d2[i]} <-> {d[i]}");
			}
		}
	}
}
