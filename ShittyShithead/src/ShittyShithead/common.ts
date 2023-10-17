import type { Player } from '../Shithead/Player'
import type { TableMaster } from '../Shithead/TableMaster'

export type GameConnectionEvent =
	| JoinTableEvent
	| CreateTableEevent

export interface GameConnectionEventBase {
	event: string
	table: string
	playerName: string
	game: Player
}

export interface JoinTableEvent extends GameConnectionEventBase {
	event: 'join'
}

export interface CreateTableEevent extends GameConnectionEventBase {
	event: 'create'
	game: TableMaster
}
