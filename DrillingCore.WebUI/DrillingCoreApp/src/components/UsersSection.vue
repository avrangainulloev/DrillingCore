<template>
  <div class="users-section">
   
    <!-- Панель фильтрации и поиска -->
    <div class="row mb-3">
      <div class="col-md-4">
        <input
          type="text"
          class="form-control"
          placeholder="Search by name, username, or email"
          v-model="searchTerm"
          @input="loadUsers"
        />
      </div>
      <div class="col-md-4">
        <select class="form-select" v-model="selectedRole" @change="loadUsers">
          <option value="">All Roles</option>
          <option v-for="role in roles" :key="role.id" :value="role.id">
            {{ role.name }}
          </option>
        </select>
      </div>
      <div class="col-md-4 text-end">
        <button class="btn btn-primary" @click="openUserModal()">Add User</button>
      </div>
    </div>

    <!-- Таблица пользователей -->
    <div class="scrollable-table-container">
    <table class="table table-striped">
      <thead>
        <tr>
          <th>ID</th>
          <th>Username</th>
          <th>Full Name</th>
          <th>Email</th>
          <th>Mobile</th>
          <th>Role</th>
          <th>Status</th>
          <th>Actions</th>
        </tr>
      </thead>
      <tbody>
        <tr v-for="user in users" :key="user.id">
          <td>{{ user.id }}</td>
          <td>{{ user.username }}</td>
          <td>{{ user.fullName }}</td>
          <td>{{ user.email }}</td>
          <td>{{ user.mobile }}</td>
          <td>{{ user.roleName || 'Unknown' }}</td>
          <td :style="{ color: user.isActive ? 'green' : 'red' }">
            {{ user.isActive ? 'Active' : 'Inactive' }}
          </td>
          <td>
            <div class="d-flex flex-wrap align-items-center">
              <button class="btn btn-sm btn-primary me-2" @click="openUserModal(user.id)">Edit</button>
              <button class="btn btn-sm btn-success" @click="openTimesheetModal(user.id, user.fullName)">Timesheet</button>
            </div>
          </td>
        </tr>
        <tr v-if="users.length === 0">
          <td colspan="8">No users found.</td>
        </tr>
      </tbody>
    </table>
    </div>

    <UserModal 
  v-if="showUserModal"
  :editingUserId="editingUserId ?? undefined"
  @close="closeUserModal" 
  @user-saved="handleUserSaved" 
/>

    <TimesheetModal 
      v-if="showTimesheetModal"
      :user-id="selectedUserId"
      :user-full-name="selectedUserFullName"
      @close="showTimesheetModal = false"
    />
  </div>
</template>

<script lang="ts">
import { defineComponent } from 'vue';
import UserModal from './UserModal.vue';
import TimesheetModal from './TimesheetModal.vue';

export default defineComponent({
  name: 'UsersSection',
  components: { UserModal, TimesheetModal },
  data() {
    return {
      users: [] as any[],
      roles: [] as any[],
      searchTerm: '',
      selectedRole: '',
      showUserModal: false,
      editingUserId: null as number | null,
      showTimesheetModal: false,
      selectedUserId: null as number | null,
      selectedUserFullName: ''
    };
  },
  async mounted() {
    await this.loadRoles();
    await this.loadUsers();
  },
  methods: {
    async loadRoles() {
      try {
        const response = await fetch('https://localhost:7200/api/Roles');
        if (response.ok) {
          this.roles = await response.json();
        }
      } catch (error) {
        console.error('Error loading roles:', error);
      }
    },
    async loadUsers() {
      let url = `https://localhost:7200/api/Users?searchTerm=${encodeURIComponent(this.searchTerm)}`;
      if (this.selectedRole) {
        url += `&roleId=${this.selectedRole}`;
      }
      try {
        const response = await fetch(url);
        if (response.ok) {
          this.users = await response.json();
        } else {
          console.error("Error loading users:", response.statusText);
        }
      } catch (error) {
        console.error("Error loading users:", error);
      }
    },
    openUserModal(userId?: number) {
      this.editingUserId = userId || null;
      this.showUserModal = true;
    },
    closeUserModal() {
      this.showUserModal = false;
    },
    handleUserSaved() {
      this.loadUsers();
      this.closeUserModal();
    },
    openTimesheetModal(userId: number, fullName: string) {
      this.selectedUserId = userId;
      this.selectedUserFullName = fullName;
      this.showTimesheetModal = true;
    }
  }
});
</script>

<style scoped>
.users-section {
  padding: 1rem;
}

.scrollable-table-container {
  max-height: calc(100vh - 220px);
  overflow-y: auto;
  border: 1px solid #ddd;
  border-radius: 6px;
}

.table thead th {
  position: sticky;
  top: 0;
  background-color: #f7f7f7;
  z-index: 1;
}
action-buttons {
  display: flex;
  gap: 0.5rem;
  justify-content: flex-start;
  align-items: center;
}

 
</style>
