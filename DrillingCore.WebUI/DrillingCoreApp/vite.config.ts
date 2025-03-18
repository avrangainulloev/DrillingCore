import { defineConfig } from 'vite'
import vue from '@vitejs/plugin-vue'

export default defineConfig({
  plugins: [vue()],
  build: {
    outDir: '../../DrillingCore.WebAPI/wwwroot', // или путь к вашей папке wwwroot
    emptyOutDir: true
  },
  server: {
    proxy: {
      // Если нужно проксировать запросы к /api на ваш .NET-бэкенд (порт 7200)
      '/api': 'https://localhost:7200'
    }
  }
})
