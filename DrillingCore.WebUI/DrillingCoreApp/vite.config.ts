import { defineConfig } from 'vite'
import vue from '@vitejs/plugin-vue'

export default defineConfig({
  plugins: [vue()],
  build: {
    outDir: '../../DrillingCore.WebAPI/wwwroot', // или путь к вашей папке wwwroot
    emptyOutDir: true
  },
  server: {
    https: false, // сам Vite не будет https
    proxy: {
      '/api': {
        target: 'https://localhost:7200',
        changeOrigin: true,
        secure: false // <--- Важно! отключает проверку SSL
      }
    }
  }
})
