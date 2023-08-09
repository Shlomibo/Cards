﻿using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace GameEngine
{
	public interface IPlayer<TSharedState, TState, TGameMove>
	{
		event EventHandler Updated;

		TSharedState SharedState { get; }
		TState State { get; }
		int PlayerId { get; }

		//TGameMove Actions { get; }

		void PlayMove(TGameMove move);
		bool IsValidMove(TGameMove move);
	}
}
