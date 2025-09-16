import { defineConfig } from 'vite'
import react from '@vitejs/plugin-react'

// Vite 7 – library mode para controlar nombres y outDir
export default defineConfig({
    plugins: [react()],
    build: {
        outDir: '../wwwroot/react',
        emptyOutDir: true,
        // Entramos en "library mode" para controlar fileName
        lib: {
            entry: './src/main.jsx',
            formats: ['es'],
            fileName: () => 'main.js',
        },
        rollupOptions: {
            output: {
                // CSS extraído por Vite -> renómbralo a main.css
                assetFileNames: (assetInfo) => {
                    const n = (assetInfo.name || '').toString()
                    if (n.endsWith('.css')) return 'main.css'
                    return 'assets/[name][extname]'
                },
                chunkFileNames: 'assets/[name].js'
            }
        }
    },
    server: { port: 5173 }
})
