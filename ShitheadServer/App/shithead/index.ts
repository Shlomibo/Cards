import { Player } from './Player.js';
import { TableMaster } from './TableMaster.js';
import { ShitheadOptions, ShitheadTableCreationOptions } from './common.js';

export {
	ShitheadOptions,
	ShitheadTableCreationOptions,
}

const defaultShitheadOptions: ShitheadOptions = {
	serverHost: location.host,
};

export async function joinTable(
	tableName: string,
	playerName: string,
	options?: Partial<ShitheadOptions>,
): Promise<Player> {
	const fullOptions: ShitheadOptions = {
		...defaultShitheadOptions,
		...options,
	};

	const { serverHost } = fullOptions;
	const url = new URL(`wss://${serverHost}`);
	url.pathname = `/shithead/${tableName}/join/${playerName}`;

	const ws = await new Promise<WebSocket>((res, rej) => {
		const ws = new WebSocket(url);

		ws.addEventListener('error', rej);
		ws.addEventListener('open', () => res(ws));
	});

	return new Player(tableName, playerName, ws, fullOptions);
}

const defaultShitheadTableCreationOptions: ShitheadTableCreationOptions = {
	...defaultShitheadOptions,
};

export async function createTable(
	tableName: string,
	playerName: string,
	options: Partial<ShitheadTableCreationOptions> = {},
): Promise<TableMaster> {
	const fullOptions: ShitheadTableCreationOptions = {
		...defaultShitheadTableCreationOptions,
		...options,
	};

	const { serverHost } = fullOptions;
	const url = new URL(`wss://${serverHost}`);
	url.pathname = `/shithead/${tableName}/create/${playerName}`;

	const ws = await new Promise<WebSocket>((res, rej) => {
		const ws = new WebSocket(url);

		ws.addEventListener('error', rej);
		ws.addEventListener('open', () => res(ws));
	});

	return new TableMaster(tableName, playerName, ws, fullOptions);
}
