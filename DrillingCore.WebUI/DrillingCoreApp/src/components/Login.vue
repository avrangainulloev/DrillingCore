<template>
  <div class="login-card">
    <h2 class="text-center mb-4">DrillingCore Login</h2>
    <form @submit.prevent="login">
      <div class="mb-3">
        <label for="username" class="form-label">Username</label>
        <input v-model="username" type="text" class="form-control" id="username" placeholder="Enter your username" required>
      </div>
      <div class="mb-3">
        <label for="password" class="form-label">Password</label>
        <input v-model="password" type="password" class="form-control" id="password" placeholder="Enter your password" required>
      </div>
      <button type="submit" class="btn btn-primary w-100">Log In</button>
    </form>
  </div>
</template>

<script lang="ts">
import { defineComponent } from 'vue'
import { useRouter } from 'vue-router'

export default defineComponent({
  name: 'Login',
  data() {
    return {
      // По умолчанию логин и пароль установлены как "admin" для разработки
      username: 'admin',
      password: 'admin'
    }
  },
  setup() {
    const router = useRouter();
    return { router };
  },
  methods: {
    async login() {
  try {
    const response = await fetch('https://localhost:7200/api/Auth/login', {
      method: 'POST',
      headers: { 'Content-Type': 'application/json' },
      credentials: 'include', // добавлено для отправки/приема куки
      body: JSON.stringify({ username: this.username, password: this.password })
    });
    if (!response.ok) {
      const errorData = await response.json();
      alert(errorData.message || "Ошибка авторизации");
      return;
    }
    const data = await response.json();
    // Токен устанавливается сервером в HttpOnly куку, поэтому здесь он не возвращается
    this.$router.push('/dashboard');
  } catch (error) {
    console.error("Ошибка при запросе:", error);
    alert("Ошибка соединения");
  }
    }}
});
</script>

<style scoped>
.login-card {
  background-color: #fff;
  padding: 2rem;
  border-radius: 10px;
  box-shadow: 0 4px 6px rgba(0,0,0,0.1);
  width: 100%;
  max-width: 400px;
  margin: auto;
}

body {
  background: linear-gradient(135deg, #667eea 0%, #764ba2 100%);
  height: 100vh;
  display: flex;
  align-items: center;
  justify-content: center;
  margin: 0;
}
</style>
