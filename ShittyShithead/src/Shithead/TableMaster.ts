import { Player } from './Player';
import { LeaveGame } from './ShitheadMove';
import { ShitheadTableCreationOptions } from './common';

export class TableMaster extends Player {
	public constructor(
		tableName: string,
		public readonly tableMasterName: string,
		ws: WebSocket,
		options: ShitheadTableCreationOptions,
	) {
		super(tableName, tableMasterName, ws, options);
	}

	public async removePlayer(playerId: number): Promise<void> {
		await this._send({
			move: 'LeaveGame',
			playerId,
		} satisfies LeaveGame);
	}

	public async startGame(): Promise<void> {
		if (!this.lastState) {
			return;
		}

		const url = new URL(`https://${this._options.serverHost}`);
		url.pathname = `/shithead/${this.tableName}/start/${this.lastState.currentPlayer.connectionId}`;

		const response = await fetch(new Request(url, {
			method: 'POST',
			body: '',
		}));

		if (!response.ok) {
			throw Object.assign(new Error('Failed to start the game'), {
				tableName: this.tableName,
				url,
				response,
			});
		}
	}
}