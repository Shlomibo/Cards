export interface InputPropertes {
	id: string
	label: string
	value?: string
	onChange?: (value: string) => unknown
}

export function Input({
	id,
	label,
	value = '',
	onChange,
}: InputPropertes) {

	return (
		<>
			<label htmlFor={id}>{label}</label>
			<input id={id} name={id} type="text" value={value} onChange={ev => onChange?.(ev.target.value)} />
		</>
	);
}
