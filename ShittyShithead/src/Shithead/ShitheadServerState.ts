import { Move, isMove } from './ShitheadMove';
import { isObject, isRecord } from './common';

export type Numeric = `${number}`;

export function isNumeric(value: unknown): value is Numeric {
	return typeof value === 'string' &&
		!isNaN(Number(value));
}

export interface ServerState {
	tableName: string
	currentPlayer: CurrentTablePlayer;
	table: Record<Numeric | number, TablePlayer>
	gameState: null | ShitheadState
}

export function isServerState(value: unknown): value is ServerState {
	return isObject(value) &&

		'tableName' in value &&
		typeof value.tableName === 'string' &&

		'currentPlayer' in value &&
		isCurrentTablePlayer(value.currentPlayer) &&

		'table' in value &&
		isRecord(value.table, isNumeric, isTablePlayer) &&

		'gameState' in value &&
		(value.gameState === null || isShitheadState(value.gameState));
}

export interface TablePlayer {
	id: number
	name: string
}

export function isTablePlayer(value: unknown): value is TablePlayer {
	return isObject(value) &&

		'id' in value &&
		typeof value.id === 'number' &&

		'name' in value &&
		typeof value.name === 'string';
}

export interface CurrentTablePlayer extends TablePlayer {
	connectionId: string
}

export function isCurrentTablePlayer(value: unknown): value is CurrentTablePlayer {
	return isTablePlayer(value) &&

		'connectionId' in value &&
		typeof value.connectionId === 'string';
}

export interface ShitheadState {
	sharedState: SharedShitheadState
	playerState: PlayerShitheadState
}

export function isShitheadState(value: unknown): value is ShitheadState {
	return isObject(value) &&

		'sharedState' in value &&
		isSharedShitheadState(value.sharedState) &&

		'playerState' in value &&
		isPlayerShitheadState(value.playerState);
}

export interface SharedShitheadState {
	players: SharedShitheadPlayerState[]
	activePlayers: number[]
	lastMove: LastMove | null
	deckSize: number
	discardPile: Card[]
	gameState: GameState
	currentTurnPlayer: number
}

export function isSharedShitheadState(value: unknown): value is SharedShitheadState {
	return isObject(value) &&

		'players' in value &&
		Array.isArray(value.players) &&
		value.players.every(isSharedShitheadPlayerState) &&

		'activePlayers' in value &&
		Array.isArray(value.activePlayers) &&
		value.activePlayers.every(player => typeof player === 'number') &&

		'lastMove' in value &&
		(value.lastMove === null || isLastMove(value.lastMove)) &&

		'deckSize' in value &&
		typeof value.deckSize === 'number' &&

		'discardPile' in value &&
		Array.isArray(value.discardPile) &&
		value.discardPile.every(isCard) &&

		'gameState' in value &&
		isGameState(value.gameState) &&

		'currentTurnPlayer' in value &&
		typeof value.currentTurnPlayer === 'number';
}

const gameStates = {
	Init: true,
	GameOn: true,
	GameOver: true,
} as const;

export type GameState = keyof typeof gameStates;

export function isGameState(value: unknown): value is GameState {
	return typeof value === 'string' &&
		value in gameStates;
}

export interface LastMove {
	playerId: null | number
	move: Move
}

export function isLastMove(value: unknown): value is LastMove {
	return isObject(value) &&

		'playerId' in value &&
		(value.playerId === null || typeof value.playerId === 'number') &&

		'move' in value &&
		isMove(value.move);
}

export interface SharedShitheadPlayerState {
	id: number
	won: boolean
	cardsCount: number
	revealedCardsAccepted: boolean
	revealedCards: Record<Numeric, Card>
	undercards: Record<Numeric, Card | null>
}

export function isSharedShitheadPlayerState(value: unknown): value is SharedShitheadPlayerState {
	return isObject(value) &&

		'id' in value &&
		typeof value.id === 'number' &&

		'won' in value &&
		typeof value.won === 'boolean' &&

		'cardsCount' in value &&
		typeof value.cardsCount === 'number' &&

		'revealedCardsAccepted' in value &&
		typeof value.revealedCardsAccepted === 'boolean' &&

		'revealedCards' in value &&
		isRecord(value.revealedCards, isNumeric, isCard) &&

		'undercards' in value &&
		isRecord(
			value.undercards,
			isNumeric,
			(card: unknown): card is (Card | null) => card === null || isCard(card)
		);
}

export interface PlayerShitheadState {
	gameState: GameState
	playerId: number
	hand: Card[]
	revealedCards: Record<Numeric, Card>
	undercards: Record<Numeric, Card | null>
	won: boolean
	revealedCardsAccepted: boolean
}

export function isPlayerShitheadState(value: unknown): value is PlayerShitheadState {
	return isObject(value) &&

		'gameState' in value &&
		isGameState(value.gameState) &&

		'playerId' in value &&
		typeof value.playerId === 'number' &&

		'hand' in value &&
		Array.isArray(value.hand) &&
		value.hand.every(isCard) &&

		'revealedCards' in value &&
		isRecord(value.revealedCards, isNumeric, isCard) &&

		'undercards' in value &&
		isRecord(
			value.undercards,
			isNumeric,
			(card: unknown): card is (Card | null) => card === null || isCard(card)
		) &&

		'won' in value &&
		typeof value.won === 'boolean' &&

		'revealedCardsAccepted' in value &&
		typeof value.revealedCardsAccepted === 'boolean';
}

export type Card =
	| Joker
	| NormalCard

export function isCard(value: unknown): value is Card {
	return !isCardBase(value) ? false :
		value.value === 'Joker' ? isJoker(value) :
			isNormalCard(value);
}

export interface CardBase {
	value: CardValue
}

function isCardBase(value: unknown): value is CardBase {
	return isObject(value) &&
		'value' in value &&
		isCardValue(value.value);
}

export interface Joker extends CardBase {
	value: 'Joker'
	color: CardColor
}

export function isJoker(value: unknown): value is Joker {
	return isCardBase(value) &&
		value.value === 'Joker' &&
		'color' in value &&
		isCardColor(value.color);
}

export interface NormalCard extends CardBase {
	value: Exclude<CardValue, 'Joker'>
	suit: CardSuit
}

export function isNormalCard(value: unknown): value is NormalCard {
	return isCardBase(value) &&
		value.value !== 'Joker' &&
		'suit' in value &&
		isCardSuit(value.suit);
}

const cardSuits = {
	Hearts: true,
	Diamonds: true,
	Clubs: true,
	Spades: true,
} as const;

export type CardSuit = keyof typeof cardSuits;

export function isCardSuit(value: unknown): value is CardSuit {
	return typeof value === 'string' &&
		value in cardSuits;
}

const cardColors = {
	Red: true,
	Black: true,
} as const;

export type CardColor = keyof typeof cardColors;

export function isCardColor(value: unknown): value is CardColor {
	return typeof value === 'string' &&
		value in cardColors;
}

const cardValues = {
	Joker: true,
	Ace: true,
	'2': true,
	'3': true,
	'4': true,
	'5': true,
	'6': true,
	'7': true,
	'8': true,
	'9': true,
	'10': true,
	Jack: true,
	Queen: true,
	King: true,
} as const;

export type CardValue = keyof typeof cardValues;


export function isCardValue(value: unknown): value is CardValue {
	return typeof value === 'string' &&
		value in cardValues;
}