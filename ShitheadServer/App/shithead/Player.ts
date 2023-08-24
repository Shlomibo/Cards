import {
	AcceptDiscardPile,
	AcceptSelectedRevealedCards,
	PlaceCard,
	PlaceJoker,
	ReselectRevealedCards,
	RevealUndercard,
	RevealedCardSelection,
	TakeUndercards,
	UnsetRevealedCard,
} from './ShitheadMove.js';
import { ServerState, isServerState } from './ShitheadServerState.js';
import { State } from './ShitheadState.js';
import { ShitheadOptions } from './common.js';

export class Player extends EventTarget {
	#lastState: null | State = null;

	public constructor(
		public readonly tableName: string,
		public readonly playerName: string,
		protected readonly _ws: WebSocket,
		protected readonly _options: ShitheadOptions
	) {
		super();

		_ws.addEventListener('message', ev => {
			if (!isServerState(ev.data)) {
				console.error('Invalid server state data!', { data: ev.data });
				return;
			}

			this.#lastState = ev.data;
			this.dispatchEvent(new ShitheadStateEvent(gameStateFromServerState(ev.data)));
		});
	}

	public get lastState(): State | null {
		return this.#lastState;
	}

	public revealedCardSelection(cardIndex: number, targetIndex: number): void {
		this._send({
			move: 'RevealedCardSelection',
			cardIndex,
			targetIndex,
		} satisfies RevealedCardSelection);
	}

	public unsetRevealedCard(cardIndex: number): void {
		this._send({
			move: 'UnsetRevealedCard',
			cardIndex,
		} satisfies UnsetRevealedCard);
	}

	public revealUndercard(cardIndex: number): void {
		this._send({
			move: 'RevealUndercard',
			cardIndex,
		} satisfies RevealUndercard);
	}

	public placeCard(cardIndex:number, ...cardIndices: number[]): void
	public placeCard(...cardIndices: [number, ...number[]]): void {
		this._send({
			move: 'PlaceCard',
			cardIndices,
		} satisfies PlaceCard);
	}

	public takeUndercards(cardIndex: number, ...cardIndices: number[]): void
	public takeUndercards(...cardIndices: [number, ...number[]]): void {
		this._send({
			move: 'TakeUndercards',
			cardIndices,
		} satisfies TakeUndercards);
	}

	public placeJoker(playerId:number): void {
		this._send({
			move: 'PlaceJoker',
			playerId,
		} satisfies PlaceJoker);
	}

	public acceptSelectedRevealedCards(): void {
		this._send({
			move: 'AcceptSelectedRevealedCards',
		} satisfies AcceptSelectedRevealedCards);
	}

	public reselectRevealedCards(): void {
		this._send({
			move: 'ReselectRevealedCards',
		} satisfies ReselectRevealedCards);
	}

	public acceptDiscardPile(): void {
		this._send({
			move: 'AcceptDiscardPile',
		} satisfies AcceptDiscardPile);
	}

	protected _send(data: unknown): void {
		return this._ws.send(JSON.stringify(data));
	}
}

export class ShitheadStateEvent extends Event {
	public constructor(public readonly state: State) {
		super('state-update');
	}
}

function gameStateFromServerState(state: ServerState): State {
	return state;
}