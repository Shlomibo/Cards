export interface ShitheadOptions {
	serverHost: string;
}

export interface ShitheadTableCreationOptions extends ShitheadOptions {
}

export function isObject(value: unknown): value is object {
	return value === Object(value);
}

// eslint-disable-next-line @typescript-eslint/no-explicit-any
export type Key = keyof any;

export function isRecord<TValue>(
	value: unknown,
	propValueValidation: (x: unknown) => x is TValue,
): value is Record<Key, TValue>;
export function isRecord<TKey extends Key, TValue>(
	value: unknown,
	propKeyValidation: (x: unknown) => x is TKey,
	propValueValidation: (x: unknown) => x is TValue,
): value is Record<TKey, TValue>;
export function isRecord(
	value: unknown,
	valueOrKeyValidation: (x: unknown) => boolean,
	maybeValueValidation?: (x: unknown) => boolean,
): boolean {
	if (!isObject(value)) {
		return false;
	}

	const [propValueValidation, propKeyValidation] = maybeValueValidation
		? [maybeValueValidation, valueOrKeyValidation]
		: [valueOrKeyValidation, null];

	const entries = Object.entries(value);

	return propKeyValidation
		? entries.every(([key, value]) => propKeyValidation!(key) && propValueValidation(value))
		: entries.every(([, value]) => propValueValidation(value));
}
