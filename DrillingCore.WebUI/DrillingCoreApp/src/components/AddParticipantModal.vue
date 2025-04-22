<template>
  <div class="modal-overlay" @click.self="closeModal">
    <div class="modal-dialog modal-lg">
      <div class="modal-content">

        <!-- Заголовок -->
        <div class="modal-header">
          <h5 class="modal-title">Add Participant</h5>
          <button type="button" class="btn-close" @click="closeModal">&times;</button>
        </div>

        <!-- Тело с прокруткой -->
        <div class="modal-body-scrollable">
          <div class="modal-body-inner">

            <!-- Select Group -->
            <div class="mb-3">
              <label class="form-label">Select Group</label>
              <select class="form-select" v-model="selectedGroupId">
                <option value="">-- No Group (just project) --</option>
                <option v-for="group in groups" :key="group.id" :value="group.id">
                  {{ group.groupName }}
                </option>
              </select>
            </div>

            <!-- Start Date -->
            <div class="mb-3">
              <label class="form-label">Start Date</label>
              <input type="date" class="form-control" v-model="startDate" />
            </div>

            <!-- Salary Inputs -->
            <div class="row mb-3">
              <div class="col-md-6">
                <label class="form-label">Daily Rate</label>
                <input type="number" class="form-control" v-model.number="dailyRate" />
              </div>
              <div class="col-md-6">
                <label class="form-label">Meter Rate</label>
                <input type="number" class="form-control" v-model.number="meterRate" />
              </div>
            </div>

            <!-- Filters -->
            <div class="row mb-3">
              <div class="col-md-6">
                <input
                  type="text"
                  class="form-control"
                  placeholder="Search available users..."
                  v-model="searchTerm"
                  @input="applyFilters"
                />
              </div>
              <div class="col-md-6">
                <select
                  class="form-select"
                  v-model="selectedRole"
                  @change="applyFilters"
                >
                  <option value="all">All Roles</option>
                  <option v-for="role in roles" :key="role.id" :value="role.id">
                    {{ role.name }}
                  </option>
                </select>
              </div>
            </div>

            <!-- Table -->
            <div class="scrollable-table">
              <table class="table table-bordered">
                <thead>
                  <tr>
                    <th>Select</th>
                    <th>Full Name</th>
                    <th>Mobile</th>
                    <th>Email</th>
                    <th>Role</th>
                  </tr>
                </thead>
                <tbody>
                  <tr v-if="filteredUsers.length === 0">
                    <td colspan="5">No users found.</td>
                  </tr>
                  <tr v-for="user in filteredUsers" :key="user.id">
                    <td>
                      <input type="checkbox" :value="user.id" v-model="selectedUserIds" />
                    </td>
                    <td>{{ user.fullName }}</td>
                    <td>{{ user.mobile }}</td>
                    <td>{{ user.email }}</td>
                    <td>{{ user.roleName }}</td>
                  </tr>
                </tbody>
              </table>
            </div>

            <!-- Notifications -->
            <div v-if="notificationMessage" class="alert alert-success">
              {{ notificationMessage }}
            </div>
            <div v-if="errorMessage" class="alert alert-danger">
              {{ errorMessage }}
            </div>

          </div>
        </div>

        <!-- Футер -->
        <div class="modal-footer">
          <button type="button" class="btn btn-primary" @click="addSelectedParticipants" :disabled="isAdding">
            {{ isAdding ? 'Adding...' : 'Add Selected Participants' }}
          </button>
          <button type="button" class="btn btn-secondary" @click="closeModal">Cancel</button>
        </div>

      </div>
    </div>
  </div>
</template>
  
  <script lang="ts">
  import { defineComponent } from 'vue';
  
  export default defineComponent({
    name: 'AddParticipantModal',
    props: {
      projectId: {
        type: Number,
        required: true
      }
    },
    data() {
      return {
        availableUsers: [] as any[],
        filteredUsers: [] as any[],
        groups: [] as any[],
        roles: [] as any[],
        searchTerm: '',
        selectedRole: 'all',
        selectedGroupId: '' as string | number,
        // Новое поле для Start Date (формат "YYYY-MM-DD")
        startDate: '',
        dailyRate: null as number | null,
        meterRate: null as number | null,
        selectedUserIds: [] as number[],
        notificationMessage: '',
        errorMessage: '',
        isAdding: false
      };
    },
    mounted() {
      this.initModal();
    },
    methods: {
      closeModal() {
        this.$emit('close');
      },
      async initModal() {
        // Сброс выбранных пользователей и фильтров
        this.selectedUserIds = [];
        this.searchTerm = '';
        this.selectedRole = 'all';
        // Загружаем данные
        await Promise.all([
          this.loadAvailableUsers(),
          this.loadParticipantRoles(),
          this.loadProjectGroups()
        ]);
        // Применяем фильтр после загрузки данных
        this.applyFilters();
      },
      async loadAvailableUsers() {
        try {
          const url = `${import.meta.env.VITE_API_BASE_URL}/Users/available?t=${new Date().getTime()}`;
          const response = await fetch(url, { cache: 'no-cache' });
          if (response.ok) {
            this.availableUsers = await response.json();
          } else {
            console.error("Error loading available users:", response.statusText);
          }
        } catch (error) {
          console.error("Error loading available users:", error);
        }
      },
      async loadParticipantRoles() {
        try {
          const response = await fetch(`${import.meta.env.VITE_API_BASE_URL}/Roles?t=${new Date().getTime()}`, { cache: 'no-cache' });
          if (response.ok) {
            const roles = await response.json();
            this.roles = roles;
          } else {
            console.error("Error loading roles:", response.statusText);
          }
        } catch (error) {
          console.error("Error loading roles:", error);
        }
      },
      async loadProjectGroups() {
        try {
          const response = await fetch(`${import.meta.env.VITE_API_BASE_URL}/Projects/${this.projectId}/Groups?t=${new Date().getTime()}`, { cache: 'no-cache' });
          if (response.ok) {
            const groups = await response.json();
            this.groups = groups;
          } else {
            console.error("Error loading project groups:", response.statusText);
          }
        } catch (error) {
          console.error("Error loading project groups:", error);
        }
      },
      applyFilters() {
        const term = this.searchTerm.trim().toLowerCase();
        let filtered = this.availableUsers.filter(user => {
          const fullName = user.fullName ? user.fullName.toLowerCase() : "";
          const email = user.email ? user.email.toLowerCase() : "";
          const mobile = user.mobile ? user.mobile.toLowerCase() : "";
          return fullName.includes(term) || email.includes(term) || mobile.includes(term);
        });
        if (this.selectedRole !== "all") {
          filtered = filtered.filter(user => String(user.roleId) === String(this.selectedRole));
        }
        this.filteredUsers = filtered;
      },
      async addSelectedParticipants() {
        // Сброс уведомлений
        this.notificationMessage = '';
        this.errorMessage = '';
  
        if (this.selectedUserIds.length === 0) {
          this.errorMessage = "Please select at least one user to add.";
          return;
        }
        if (!this.startDate) {
          this.errorMessage = "Please select a start date.";
          return;
        }
        this.isAdding = true;
        const requestBody = {
          projectId: this.projectId,
          groupId: this.selectedGroupId ? parseInt(this.selectedGroupId as string) : null,
          UserIds: this.selectedUserIds,
          // Передаем startDate как строку "YYYY-MM-DD"
          StartDate: this.startDate,
          dailyRate: this.dailyRate,
          meterRate: this.meterRate
        };
        try {
          const response = await fetch(`${import.meta.env.VITE_API_BASE_URL}/Projects/${this.projectId}/Participants`, {
            method: 'POST',
            headers: { 'Content-Type': 'application/json' },
            body: JSON.stringify(requestBody)
          });
          if (response.ok) {
            this.notificationMessage = "Participants added successfully!";
            this.$emit('participants-added');
            // Обновляем список доступных пользователей
            await this.loadAvailableUsers();
            this.applyFilters();
            // Сброс выбранных пользователей
            this.selectedUserIds = [];
          } else {
            const errorText = await response.text();
            this.errorMessage = "Error adding participants: " + errorText;
          }
        } catch (error) {
          console.error("Error adding participants:", error);
          this.errorMessage = "Error adding participants. See console for details.";
        } finally {
          this.isAdding = false;
        }
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
    background: rgba(0, 0, 0, 0.6);
    display: flex;
    align-items: center;
    justify-content: center;
    z-index: 1050;
    padding: 1rem;
    
  }
  .modal-dialog {
    background: #fff;
    border-radius: 8px;
    width: 100%;    
    max-width: 900px;
    box-shadow: 0 4px 15px rgba(0, 0, 0, 0.3);
    overflow: hidden;
    display: flex;
  flex-direction: column;
  max-height: 95vh;
  }
  .modal-content {
    display: flex;
    flex-direction: column;
    display: flex;
  flex-direction: column;
  max-height: 95vh;
  overflow: hidden;
  }
  .modal-header {
    display: flex;
    justify-content: space-between;
    align-items: center;
    padding: 1rem;
    background-color: #fff;
    color: #333;
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
    color: #333;
    cursor: pointer;
  }
  .modal-body {
    padding: 1rem;
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
  .table {
    width: 100%;
    margin-bottom: 1rem;
    border-collapse: collapse;
  }
  .table th,
  .table td {
    border: 1px solid #ddd;
    padding: 0.75rem;
    text-align: left;
  }
  .table th {
    background-color: #f7f7f7;
  }
  .modal-footer {
    display: flex;
    justify-content: flex-end;
    padding: 1rem;
    gap: 0.5rem;
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
  .scrollable-table {
  max-height: 250px;
  overflow-y: auto;
  border: 1px solid #ddd;
  border-radius: 4px;  
}
.modal-body-scrollable {
  overflow-y: auto;
  flex-grow: 1;
  padding: 1rem;
}
  </style>
  