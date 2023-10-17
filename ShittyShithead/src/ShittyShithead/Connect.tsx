import { Input } from '../Components/Input';
import { createTable, joinTable } from '../Shithead';
import { GameConnectionEvent } from './common';
import { useState } from 'react';

export interface ConnectProperties {
	onConnect: (connection: GameConnectionEvent) => unknown
}

export function Connect({ onConnect }: ConnectProperties) {
	const [tableName, setTableName] = useState('');
	const [playerName, setPlayerName] = useState('');

	const onCreateTable = async () => {
		const connection = await createTable(tableName, playerName);
		await Promise.resolve(onConnect({
			event: 'create',
			table: tableName,
			playerName,
			game: connection,
		}))
	};
	const onJoinTable = async () => {
		const connection = await joinTable(tableName, playerName);
		await Promise.resolve(onConnect({
			event: 'join',
			table: tableName,
			playerName,
			game: connection,
		}))
	};

	return (
		<>
			<div>
				<Input id="table" label="Table name:" value={tableName} onChange={setTableName} />
			</div>

			<div>
				<Input id="player" label="Your name:" value={playerName} onChange={setPlayerName} />
			</div>

			<button onClick={onCreateTable}>Create game</button>
			<button onClick={onJoinTable}>Join game</button>
		</>
	);
}
