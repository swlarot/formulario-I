import React from 'react'
import ReactDOM from 'react-dom/client'
import EntitySelector from './EntitySelector.jsx'
import './index.css'

// Solo montamos si existe el contenedor en el DOM de Blazor:
const rootEl = document.getElementById('entity-react-root');
if (rootEl) {
    ReactDOM.createRoot(rootEl).render(
        <React.StrictMode>
            <EntitySelector />
        </React.StrictMode>
    );
}

// Evita que Vite trate de hacer HMR sobre index.html de esta app embebida
export { };
