import { defineConfig } from 'vite'
import react from '@vitejs/plugin-react'

// Build en "library mode" para controlar nombres y salida en wwwroot/react
export default defineConfig({
    plugins: [react()],
    build: {
        outDir: '../wwwroot/react',   // carpeta de salida dentro de Blazor
        emptyOutDir: true,
        assetsDir: 'assets',          // subcarpeta para assets (imgs, chunks, etc.)
        lib: {
            entry: './src/main.jsx',    // punto de entrada de tu bundle
            formats: ['es'],            // ESM (module) para cargar con <script type="module">
            fileName: () => 'main.js'   // nombre fijo del JS principal
        },
        rollupOptions: {
            output: {
                // Fuerza nombre fijo del CSS extraído por Vite
                assetFileNames: (assetInfo) => {
                    const n = String(assetInfo.name || '')
                    if (n.endsWith('.css')) return 'main.css'
                    return 'assets/[name][extname]'
                },
                // Nombres de chunks secundarios
                chunkFileNames: 'assets/[name].js'
            }
        }
    },
    server: { port: 5173 }
})
