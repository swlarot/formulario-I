export default function EntitySelector() {
    const items = [
        { key: 'ph', label: 'Propiedad Horizontal' },
        { key: 'sa', label: 'Sociedad Anónima' },
        { key: 'off', label: 'Offshore' },
        { key: 'pn', label: 'Persona Natural' },
        { key: 'otra', label: 'Otra' },
    ];

    const onSelect = (key) => {
        // Por ahora navegamos a la página actual con un querystring.
        window.location.href = `/conocimiento-cliente?entidad=${key}`;
    };

    return (
        <div className="p-3">
            <h5 className="mb-3">Elige el tipo de entidad</h5>
            <div className="d-grid gap-2">
                {items.map(x => (
                    <button key={x.key}
                        className="btn btn-outline-primary btn-lg"
                        onClick={() => onSelect(x.key)}>
                        {x.label}
                    </button>
                ))}
            </div>
        </div>
    );
}
