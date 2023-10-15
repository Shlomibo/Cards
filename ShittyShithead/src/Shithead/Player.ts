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
} from './ShitheadMove';
import { ServerState, isServerState } from './ShitheadServerState';
import {
	PlayerShitheadServerState,
	PlayerShitheadState,
	SharedShitheadPlayerServerState,
	SharedShitheadPlayerState,
	SharedShitheadServerState,
	SharedShitheadState,
	ShitheadState,
	State,
} from './ShitheadState';
import { ShitheadOptions } from './common';

const HIGH_WATERMARK = 7 * 1024;

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

			const lastState = stateFromServerState(ev.data)
			this.#lastState = lastState;
			this.dispatchEvent(new ShitheadStateEvent(lastState));
		});
		_ws.addEventListener('close', ev => this.dispatchEvent(ev));
	}

	public get lastState(): State | null {
		return this.#lastState;
	}

	public async revealedCardSelection(cardIndex: number, targetIndex: number): Promise<void> {
		await this._send({
			move: 'RevealedCardSelection',
			cardIndex,
			targetIndex,
		} satisfies RevealedCardSelection);
	}

	public async unsetRevealedCard(cardIndex: number): Promise<void> {
		await this._send({
			move: 'UnsetRevealedCard',
			cardIndex,
		} satisfies UnsetRevealedCard);
	}

	public async revealUndercard(cardIndex: number): Promise<void> {
		await this._send({
			move: 'RevealUndercard',
			cardIndex,
		} satisfies RevealUndercard);
	}

	public placeCard(cardIndex: number, ...cardIndices: number[]): Promise<void>;
	public async placeCard(...cardIndices: [number, ...number[]]): Promise<void> {
		await this._send({
			move: 'PlaceCard',
			cardIndices,
		} satisfies PlaceCard);
	}

	public takeUndercards(cardIndex: number, ...cardIndices: number[]): Promise<void>
	public async takeUndercards(...cardIndices: [number, ...number[]]): Promise<void> {
		await this._send({
			move: 'TakeUndercards',
			cardIndices,
		} satisfies TakeUndercards);
	}

	public async placeJoker(playerId:number): Promise<void> {
		await this._send({
			move: 'PlaceJoker',
			playerId,
		} satisfies PlaceJoker);
	}

	public async acceptSelectedRevealedCards(): Promise<void> {
		await this._send({
			move: 'AcceptSelectedRevealedCards',
		} satisfies AcceptSelectedRevealedCards);
	}

	public async reselectRevealedCards(): Promise<void> {
		await this._send({
			move: 'ReselectRevealedCards',
		} satisfies ReselectRevealedCards);
	}

	public async acceptDiscardPile(): Promise<void> {
		await this._send({
			move: 'AcceptDiscardPile',
		} satisfies AcceptDiscardPile);
	}

	protected async _send(data: unknown): Promise<void> {
		this._ws.send(JSON.stringify(data));

		if (this._ws.bufferedAmount >= HIGH_WATERMARK) {
			await new Promise<void>(async (res, rej) => {
				while (this._ws.bufferedAmount > 0) {
					if (this._ws.readyState !== WebSocket.OPEN) {
						return rej(new Error('Socket was closed'));
					}

					await new Promise(res => setTimeout(res, 50));
				}

				res();
			});
		}
		this.addEventListener
	}

	public addEventListener(
		type: 'close',
		callback: StupidDOMEventListener<CloseEvent> | null,
		options?: AddEventListenerOptions | boolean
	): void;
	public addEventListener(
		type: 'state-update',
		callback: ShitheadStateEventListener | null,
		options?: AddEventListenerOptions | boolean
	): void;
	public addEventListener(
		type: string,
		callback: EventListenerOrEventListenerObject | null,
		options?: AddEventListenerOptions | boolean
	): void;
	public addEventListener(
		type: string,
		callback: EventListenerOrEventListenerObject | null,
		options?: AddEventListenerOptions | boolean
	): void {
		super.addEventListener(type, callback, options);
	}
}

type StupidDOMEventListener<TEvent extends Event> =
	| ((ev: TEvent) => void)
	| { handleEvent: (ev: TEvent) => void };
export type ShitheadStateEventListener = StupidDOMEventListener<ShitheadStateEvent>

export class ShitheadStateEvent extends Event {
	public constructor(public readonly state: State) {
		super('state-update');
	}
}

function stateFromServerState({
	gameState,
	...state
}: ServerState): State {
	const tablePlayers = state.table;

	return Object.assign(state, {
		gameState: gameState && gameStateFromServerState(gameState),
	});

	function gameStateFromServerState({
		sharedState,
		playerState,
	}: ServerState['gameState'] & NonNullable<unknown>): ShitheadState {
		return {
			sharedState: shareStateFromServerState(sharedState),
			playerState: playerStateFromServerState(playerState),
		};

		function shareStateFromServerState({
			players,
			activePlayers,
			lastMove,
			currentTurnPlayer,
			...sharedState
		}: SharedShitheadServerState): SharedShitheadState {
			return Object.assign(sharedState, {
				players: players.map(sharedPlayerStateFromServerState),
				activePlayers: activePlayers.map(id => tablePlayers[id]),
				lastMove: lastMove && {
					...lastMove,
					player: lastMove.playerId !== null
						? tablePlayers[lastMove.playerId]
						: null,
				},
				currentTurnPlayer: tablePlayers[currentTurnPlayer],
			});

			function sharedPlayerStateFromServerState(
				player: SharedShitheadPlayerServerState
			): SharedShitheadPlayerState {
				return {
					...player,
					name: tablePlayers[player.id].name,
				};
			}
		}

		function playerStateFromServerState(playerState: PlayerShitheadServerState): PlayerShitheadState {
			return {
				...playerState,
				name: tablePlayers[playerState.playerId].name,
			}
		}
	}
}