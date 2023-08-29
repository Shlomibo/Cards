import { handleGame } from './Game.js';
import { handlePlayerConnections } from './HandlePlayerConnections.js';

export function createGame(root: HTMLElement): void {
	const connectionEvents = handlePlayerConnections(root);
	handleGame(root, connectionEvents);
}
