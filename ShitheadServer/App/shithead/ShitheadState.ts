import { Move } from './ShitheadMove.js';
import {
	ServerState,
	SharedShitheadState as SharedShitheadServerState,
	PlayerShitheadState as PlayerShitheadServerState,
	SharedShitheadPlayerState as SharedShitheadPlayerServerState,
    TablePlayer,
} from './ShitheadServerState.js';

export {
	ServerState,
	SharedShitheadServerState,
	PlayerShitheadServerState,
	SharedShitheadPlayerServerState,
	TablePlayer,
};

export type State =
	& Omit<ServerState, 'gameState'>
	& { gameState: null | ShitheadState };

export interface ShitheadState {
	sharedState: SharedShitheadState
	playerState: PlayerShitheadState
}

export type SharedShitheadState =
	& Omit<SharedShitheadServerState,
		| 'players'
		| 'activePlayers'
		| 'lastMove'
		| 'currentTurnPlayer'>
	& {
		players: SharedShitheadPlayerState[]
		activePlayers: TablePlayer[]
		lastMove: null | LastMove
		currentTurnPlayer: TablePlayer
	}

export interface SharedShitheadPlayerState extends SharedShitheadPlayerServerState {
	name: string
}

export interface LastMove {
	player: null | TablePlayer
	move: Move
}

export interface PlayerShitheadState extends PlayerShitheadServerState {
	name: string
}
