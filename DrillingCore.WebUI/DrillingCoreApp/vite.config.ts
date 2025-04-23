import { defineConfig } from 'vite'
import vue from '@vitejs/plugin-vue'
import path from 'path' // ⬅️ Добавить!

export default defineConfig({
  plugins: [vue()],
  resolve: {
    alias: {
      '@': path.resolve(__dirname, './src') // ⬅️ Добавить!
    }
  },
  build: {
    outDir: '../../DrillingCore.WebAPI/wwwroot',
    emptyOutDir: true
  },
  server: {
  
    proxy: {
      '/api': {
        target: 'http://localhost:5000',
        changeOrigin: true,
        secure: false
      }
    }
  }
})
