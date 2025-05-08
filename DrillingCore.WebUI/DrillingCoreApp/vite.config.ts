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
   
    emptyOutDir: true
  },
  server: {
    host: '0.0.0.0', // нужно, чтобы открывалось и с других устройств (например, телефона)
    port: 8080,      // нужный тебе порт
    proxy: {
      '/api': {
        target: 'http://10.0.0.80:5000',
        changeOrigin: true,
        secure: false
      }
    }
  }
})
