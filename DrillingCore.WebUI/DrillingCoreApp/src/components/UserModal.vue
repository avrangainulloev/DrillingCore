<template>
  <div class="modal-overlay" @click.self="closeModal">
    <div class="modal-dialog">
      <div class="modal-content user-modal">
        <div class="modal-header">
          <h5 class="modal-title">{{ editingUserId ? 'Edit User' : 'Create User' }}</h5>
          <button type="button" class="btn-close" @click="closeModal">&times;</button>
        </div>
        <div class="modal-body">
          <form @submit.prevent="submitUser" id="userForm">
            <input type="hidden" v-model="user.id" />
            <div class="mb-3">
              <label class="form-label">Username</label>
              <input type="text" class="form-control" v-model="user.username" required />
            </div>
            <div class="mb-3">
              <label class="form-label">Password</label>
              <input type="password" class="form-control" v-model="user.password" :required="!editingUserId" />
            </div>
            <div class="mb-3">
              <label class="form-label">Full Name</label>
              <input type="text" class="form-control" v-model="user.fullName" required />
            </div>
            <div class="mb-3">
              <label class="form-label">Email</label>
              <input type="email" class="form-control" v-model="user.email" required />
            </div>
            <div class="mb-3">
              <label class="form-label">Mobile</label>
              <input type="text" class="form-control" v-model="user.mobile" />
            </div>
            <div class="mb-3">
              <label class="form-label">Role</label>
              <select class="form-select" v-model="user.roleId" required>
                <option v-for="role in roles" :key="role.id" :value="role.id">
                  {{ role.name }}
                </option>
              </select>
            </div>
            <div class="mb-3">
              <label class="form-label">Status</label>
              <select class="form-select" v-model="user.isActive" required>
                <option :value="true">Active</option>
                <option :value="false">Inactive</option>
              </select>
            </div>
          </form>
          <div v-if="notificationMessage" class="alert alert-success">
            {{ notificationMessage }}
          </div>
          <div v-if="errorMessage" class="alert alert-danger">
            {{ errorMessage }}
          </div>
        </div>
        <div class="modal-footer">
          <button class="btn btn-success" @click="submitUser">{{ editingUserId ? 'Save Changes' : 'Save User' }}</button>
          <button class="btn btn-secondary" @click="closeModal">Cancel</button>
        </div>
      </div>
    </div>
  </div>
</template>

<script lang="ts">
import { defineComponent } from 'vue';

export default defineComponent({
  name: 'UserModal',
  props: {
    editingUserId: {
      type: Number,
      default: undefined
    }
  },
  emits: ['close', 'user-saved'],
  data() {
    return {
      user: {
        id: 0,
        username: '',
        password: '',
        fullName: '',
        email: '',
        mobile: '',
        roleId: null,
        isActive: true
      },
      roles: [] as { id: number; name: string }[],
      notificationMessage: '',
      errorMessage: ''
    };
  },
  async mounted() {
    await this.loadRoles();
    if (this.editingUserId) {
      this.loadUserData(this.editingUserId);
    }
  },
  methods: {
    async loadRoles() {
      try {
        const res = await fetch(`${import.meta.env.VITE_API_BASE_URL}/Roles`);
        this.roles = await res.json();
      } catch (error) {
        this.errorMessage = 'Failed to load roles';
      }
    },
    async loadUserData(userId: number) {
      try {
        const res = await fetch(`${import.meta.env.VITE_API_BASE_URL}/Users/${userId}`);
        const data = await res.json();
        this.user = { ...data, password: '' };
      } catch (error) {
        this.errorMessage = 'Failed to load user';
      }
    },
    async submitUser() {
      this.notificationMessage = '';
      this.errorMessage = '';
      const method = this.editingUserId ? 'PUT' : 'POST';
      let url = `${import.meta.env.VITE_API_BASE_URL}/Users`;
      if (this.editingUserId) url += `/${this.editingUserId}`;

      try {
        const res = await fetch(url, {
          method,
          headers: { 'Content-Type': 'application/json' },
          body: JSON.stringify(this.user)
        });
        if (res.ok) {
          this.notificationMessage = 'User saved successfully';
          this.$emit('user-saved');
          setTimeout(() => this.closeModal(), 1000);
        } else {
          this.errorMessage = await res.text();
        }
      } catch (err) {
        this.errorMessage = 'Error saving user';
      }
    },
    closeModal() {
      this.$emit('close');
    }
  }
});
</script>

<style scoped>
.modal-overlay {
  position: fixed;
  top: 0;
  left: 0;
  right: 0;
  bottom: 0;
  background: rgba(0,0,0,0.6);
  display: flex;
  justify-content: center;
  align-items: center;
  z-index: 1100;
  padding: 1rem;
}
.modal-dialog {
  background: #fff;
  border-radius: 8px;
  width: 100%;
  max-width: 500px;
  box-shadow: 0 4px 12px rgba(0,0,0,0.2);
  overflow: hidden;
}
.modal-content.user-modal {
  display: flex;
  flex-direction: column;
  max-height: 80vh; /* Ограничиваем максимальную высоту */
}
.modal-header {
  display: flex;
  justify-content: space-between;
  align-items: center;
  padding: 1rem;
  border-bottom: 1px solid #ddd;
}
.modal-title {
  margin: 0;
  font-size: 1.25rem;
}
.btn-close {
  background: transparent;
  border: none;
  font-size: 1.5rem;
  cursor: pointer;
}
.modal-body {
  padding: 1rem;
  overflow-y: auto; /* Добавляем вертикальный скролл */
}
.mb-3 {
  margin-bottom: 1rem;
}
.form-label {
  margin-bottom: 0.5rem;
  font-weight: bold;
}
.form-control, .form-select {
  width: 100%;
  padding: 0.5rem;
  border: 1px solid #ddd;
  border-radius: 4px;
  font-size: 1rem;
}
.modal-footer {
  display: flex;
  justify-content: flex-end;
  gap: 0.5rem;
  padding: 1rem;
  background-color: #f1f1f1;
}
.btn {
  padding: 0.5rem 1rem;
  border: none;
  border-radius: 4px;
  cursor: pointer;
  font-size: 1rem;
}
.btn-primary {
  background-color: #1976d2;
  color: #fff;
}
.btn-primary:hover {
  background-color: #1565c0;
}
.btn-secondary {
  background-color: #6c757d;
  color: #fff;
}
.btn-secondary:hover {
  background-color: #5a6268;
}
.alert {
  margin-top: 0.5rem;
  padding: 0.5rem 0.75rem;
  border-radius: 4px;
  font-size: 0.9rem;
}
.alert-success {
  background-color: #d4edda;
  color: #155724;
}
.alert-danger {
  background-color: #f8d7da;
  color: #721c24;
}
</style>
