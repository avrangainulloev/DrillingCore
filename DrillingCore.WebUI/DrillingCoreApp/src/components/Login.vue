<template>
  <div class="login-wrapper">
    <div class="login-card">
      <h1 class="brand-title">DrillingCore</h1>
      <h2 class="subtitle">Welcome back</h2>
      <form @submit.prevent="login">
        <div class="mb-3">
          <label for="username" class="form-label">Username</label>
          <input v-model="username" type="text" class="form-control" id="username" placeholder="Enter your username" required>
        </div>
        <div class="mb-3">
          <label for="password" class="form-label">Password</label>
          <input v-model="password" type="password" class="form-control" id="password" placeholder="Enter your password" required>
        </div>
        <div class="d-flex justify-content-between align-items-center mb-3">
          <a href="#" class="forgot-link">Forgot password?</a>
        </div>
        <button type="submit" class="btn btn-primary w-100">Log In</button>
      </form>
      <div class="footer-text">© DrillingCore 2025</div>
    </div>
  </div>
</template>

<script lang="ts">
import { defineComponent } from 'vue'
import { useRouter } from 'vue-router'
import { useUserStore } from '../stores/userStore'
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
    const response = await fetch(`${import.meta.env.VITE_API_BASE_URL}/Auth/login`, {
      method: 'POST',
      headers: { 'Content-Type': 'application/json' },
      credentials: 'include',
      body: JSON.stringify({
        username: this.username,
        password: this.password
      })
    });

    if (!response.ok) {
      const errorData = await response.json();
      alert(errorData.message || "Ошибка авторизации");
      return;
    }

    const data = await response.json();
    
    const userStore = useUserStore();
    userStore.setUser({
      userId: data.userId,
      fullName: data.fullname,
      role: data.role,
      roleId: data.roleId
    });
   
    this.$router.push('/dashboard');
  } catch (error) {
    console.error("Ошибка при запросе:", error);
    alert("Ошибка соединения");
  }
}}
});

</script>
<style scoped>
.login-wrapper {
  height: 100vh;
  background: linear-gradient(135deg, #667eea 0%, #764ba2 100%);
  display: flex;
  align-items: center;
  justify-content: center;
  padding: 1rem;
}

.login-card {
  background: #fff;
  padding: 2.5rem;
  border-radius: 1rem;
  box-shadow: 0 20px 40px rgba(0, 0, 0, 0.15);
  width: 100%;
  max-width: 420px;
  animation: fadeIn 0.5s ease-in-out;
  position: relative;
}

.brand-title {
  font-size: 2.2rem;
  font-weight: 900;
  color: #4b6cb7;
  text-align: center;
  margin-bottom: 0.25rem;
}

.subtitle {
  font-size: 1.1rem;
  color: #555;
  text-align: center;
  margin-bottom: 1.5rem;
}

.form-control {
  border-radius: 0.5rem;
  padding: 0.75rem;
  font-size: 1rem;
  border: 1px solid #ccc;
  transition: border-color 0.3s;
}

.form-control:focus {
  border-color: #4b6cb7;
  box-shadow: 0 0 0 0.1rem rgba(75, 108, 183, 0.2);
  outline: none;
}

.forgot-link {
  font-size: 0.9rem;
  color: #4b6cb7;
  text-decoration: none;
  transition: color 0.3s;
}

.forgot-link:hover {
  color: #364d91;
  text-decoration: underline;
}

.btn-primary {
  background: linear-gradient(to right, #4b6cb7, #182848);
  border: none;
  padding: 0.75rem;
  font-weight: 600;
  font-size: 1rem;
  border-radius: 0.5rem;
  transition: background 0.3s;
}

.btn-primary:hover {
  background: linear-gradient(to right, #3a5191, #101d35);
}

.footer-text {
  text-align: center;
  font-size: 0.85rem;
  color: #999;
  margin-top: 2rem;
}

@keyframes fadeIn {
  from {
    opacity: 0;
    transform: translateY(20px);
  }
  to {
    opacity: 1;
    transform: translateY(0);
  }
}

@media (max-width: 480px) {
  .login-card {
    padding: 2rem 1.5rem;
  }

  .brand-title {
    font-size: 1.8rem;
  }

  .subtitle {
    font-size: 1rem;
  }
}

</style>
