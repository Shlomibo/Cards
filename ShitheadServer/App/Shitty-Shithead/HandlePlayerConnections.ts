import { Player } from '../shithead/Player.js';
import { createTable } from '../shithead/index.js';
import { CLS_HIDDEN, asyncHandler } from './common.js';

const EL_TABLE_NAME = 'table-name';
const EL_PLAYER_NAME = 'player-name';

export function handlePlayerConnections(root: HTMLElement): EventTarget {
	const events = new EventTarget();

	const container = root.appendChild(document.createElement('div'));

	const errorsDiv = root.appendChild(document.createElement('div'));
	errorsDiv.classList.add('error');

	const tableNameInput = container.appendChild(document.createElement('input'));
	tableNameInput.id = EL_TABLE_NAME;
	const tableNameLabel = container.appendChild(document.createElement('label'));
	tableNameLabel.setAttribute('for', EL_TABLE_NAME);
	tableNameLabel.textContent = 'Table name';

	const playerNameInput = container.appendChild(document.createElement('input'));
	playerNameInput.id = EL_PLAYER_NAME;
	const playerNameLabel = container.appendChild(document.createElement('label'));
	playerNameLabel.setAttribute('for', EL_PLAYER_NAME);
	playerNameLabel.textContent = 'Player name';

	tableNameInput.oninput = validateInput;
	playerNameInput.oninput = validateInput;

	const createTableButton = container.appendChild(document.createElement('button'));
	createTableButton.textContent = 'Create table';
	createTableButton.disabled = true;
	createTableButton.onclick = asyncHandler(createShitheadTable, errorsDiv);

	const joinTableButton = container.appendChild(document.createElement('button'));
	joinTableButton.textContent = 'Join table';
	joinTableButton.disabled = true;
	joinTableButton.onclick = asyncHandler(joinShitheadTable, errorsDiv);

	return events;

	function validateInput() {
		const isDisabled = !tableNameInput.value || !playerNameInput.value;

		createTableButton.disabled = isDisabled;
	}

	async function createShitheadTable() {
		const tableName = tableNameInput.value;
		const playerName = playerNameInput.value;

		const connection = await createTable(tableName, playerName);
		handleNewConnection(connection);
	}

	async function joinShitheadTable() {
		const tableName = tableNameInput.value;
		const playerName = playerNameInput.value;

		const connection = await createTable(tableName, playerName);
		handleNewConnection(connection);
	}

	function handleNewConnection(connection: Player) {
		container.classList.add('CLS_HIDDEN');
		connection.addEventListener('close', ev => container.classList.remove(CLS_HIDDEN));
	}
}
