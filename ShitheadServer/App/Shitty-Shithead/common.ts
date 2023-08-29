export function asyncHandler<T extends (...args: any[]) => Promise<any>>(
	asyncFn: T,
	errorsDiv: HTMLElement
): (...args: Parameters<T>) => void {
	return (...args) => void asyncFn(...args)
		.then(
			() => errorsDiv.innerText = '',
			error => errorsDiv.innerText = error instanceof Error
				? error.message
				: String(error)
		);
}

export const CLS_HIDDEN = 'hidden';
