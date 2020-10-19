using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace GameEngine
{
	public interface IPlayer<TActions, TSharedState, TState>
	{
		event EventHandler Updated;

		TSharedState SharedState { get; }
		TState State { get; }

		TActions Actions { get; }
	}
}
