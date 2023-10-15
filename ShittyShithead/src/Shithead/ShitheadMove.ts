import { isObject } from './common';

export interface MoveBase {
	move: string
}

function isMoveBase(value: unknown): value is MoveBase {
	return isObject(value) &&
		'move' in value &&
		typeof value.move === 'string';
}

export interface RevealedCardSelection extends MoveBase {
	move: 'RevealedCardSelection'
	cardIndex: number
	targetIndex: number
}

function isCardIndexMove(value: unknown): value is (MoveBase & { cardIndex: number }) {
	return isMoveBase(value) &&
		'cardIndex' in value &&
		typeof value.cardIndex === 'number';
}

export function isRevealedCardSelection(value: unknown): value is RevealedCardSelection {
	return isCardIndexMove(value) &&
		value.move === 'RevealedCardSelection' &&
		'targetIndex' in value &&
		typeof value.targetIndex === 'number';
}

export interface UnsetRevealedCard extends MoveBase {
	move: 'UnsetRevealedCard'
	cardIndex: number
}

export function isUnsetRevealedCard(value: unknown): value is UnsetRevealedCard {
	return isCardIndexMove(value) &&
		value.move === 'UnsetRevealedCard';
}

export interface RevealUndercard extends MoveBase {
	move: 'RevealUndercard'
	cardIndex: number
}

export function isRevealUndercard(value: unknown): value is RevealUndercard {
	return isCardIndexMove(value) &&
		value.move === 'RevealUndercard';
}

export interface PlaceCard extends MoveBase {
	move: 'PlaceCard'
	cardIndices: number[]
}

function isCardIndicesMove(value: unknown): value is (MoveBase & { cardIndices: number[] }) {
	return isMoveBase(value) &&
		'cardIndices' in value &&
		Array.isArray(value.cardIndices) &&
		value.cardIndices.every(element => typeof element === 'number');
}

export function isPlaceCard(value: unknown): value is PlaceCard {
	return isCardIndicesMove(value) &&
		value.move === 'PlaceCard';
}

export interface TakeUndercards extends MoveBase {
	move: 'TakeUndercards'
	cardIndices: number[]
}

export function isTakeUndercards(value: unknown): value is TakeUndercards {
	return isCardIndicesMove(value) &&
		value.move === 'TakeUndercards';
}

export interface PlaceJoker extends MoveBase {
	move: 'PlaceJoker'
	playerId: number
}

function isPlayerIdMove(value: unknown): value is (MoveBase & { playerId: number }) {
	return isMoveBase(value) &&
		'playerId' in value &&
		typeof value.playerId === 'number';
}

export function isPlaceJoker(value: unknown): value is PlaceJoker {
	return isPlayerIdMove(value) &&
		value.move === 'PlaceJoker';
}

export interface LeaveGame extends MoveBase {
	move: 'LeaveGame'
	playerId: number
}

export function isLeaveGame(value: unknown): value is LeaveGame {
	return isPlayerIdMove(value) &&
		value.move === 'LeaveGame';
}

export interface AcceptSelectedRevealedCards extends MoveBase {
	move: 'AcceptSelectedRevealedCards'
}

export function isAcceptSelectedRevealedCards(value: unknown): value is AcceptSelectedRevealedCards {
	return isMoveBase(value) &&
		value.move === 'AcceptSelectedRevealedCards';
}

export interface ReselectRevealedCards extends MoveBase {
	move: 'ReselectRevealedCards'
}

export function isReselectRevealedCards(value: unknown): value is ReselectRevealedCards {
	return isMoveBase(value) &&
		value.move === 'ReselectRevealedCards';
}

export interface AcceptDiscardPile extends MoveBase {
	move: 'AcceptDiscardPile'
}

export function isAcceptDiscardPile(value: unknown): value is AcceptDiscardPile {
	return isMoveBase(value) &&
		value.move === 'AcceptDiscardPile';
}

export type Move =
	| RevealedCardSelection
	| UnsetRevealedCard
	| RevealUndercard
	| PlaceCard
	| TakeUndercards
	| PlaceJoker
	| LeaveGame
	| AcceptSelectedRevealedCards
	| ReselectRevealedCards
	| AcceptDiscardPile;

export function isMove(value: unknown): value is Move {
	if (!isMoveBase(value)) {
		return false;
	}

	switch (value.move) {
		case 'RevealedCardSelection': return isRevealedCardSelection(value);
		case 'UnsetRevealedCard': return isUnsetRevealedCard(value);
		case 'RevealUndercard': return isRevealUndercard(value);
		case 'PlaceCard': return isPlaceCard(value);
		case 'TakeUndercards': return isTakeUndercards(value);
		case 'PlaceJoker': return isPlaceJoker(value);
		case 'LeaveGame': return isLeaveGame(value);
		case 'AcceptSelectedRevealedCards': return isAcceptSelectedRevealedCards(value);
		case 'ReselectRevealedCards': return isReselectRevealedCards(value);
		case 'AcceptDiscardPile': return isAcceptDiscardPile(value);
		default: return false;
	}
}