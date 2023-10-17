export type PlayerDetailsProps =
	| NormalPlayerDetails
	| TableMasterDetails

export interface PlayerDetailsBase {
	table: string
	playerName: string
	isTableMaster: boolean
	onStartGame?: () => unknown
}

export interface NormalPlayerDetails extends PlayerDetailsBase {
	isTableMaster: false
	onStartGame?: never
}

export interface TableMasterDetails extends PlayerDetailsBase {
	isTableMaster: true
	onStartGame: () => unknown
}

export function PlayerDetails(props: PlayerDetailsProps): JSX.Element;
export function PlayerDetails({
	table,
	playerName,
	isTableMaster,
	onStartGame,
}: PlayerDetailsBase) {
	return (
		<div>
			Table: {table}
			Name: {playerName}

			{!isTableMaster ? (<></>) : (<><br />
				<button onClick={() => onStartGame!()}>Start Game</button></>)}
		</div>
	);
}
