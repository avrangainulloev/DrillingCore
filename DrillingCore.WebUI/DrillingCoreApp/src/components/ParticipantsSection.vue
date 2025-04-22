<template>
    <div>
      <!-- Модальное окно со списком групп участников -->
      <div class="modal-overlay" v-if="show">
        <div class="modal-dialog modal-lg">
          <div class="modal-content">
            <div class="modal-header d-flex justify-content-between align-items-center">
              <h5 class="modal-title">Participant Groups</h5>
              <div>
                <button class="btn btn-primary me-2" @click="openAddGroupModal">Add Group</button>
                <button type="button" class="btn-close" @click="closeMainModal"></button>
              </div>
            </div>
            <div class="modal-body">
              <table class="table table-bordered">
                <thead>
                  <tr>
                    <th>Group</th>
                    <th>Participants</th>
                  </tr>
                </thead>
                <tbody>
                  <tr v-if="groups.length === 0">
                    <td colspan="2">No participant groups found.</td>
                  </tr>
                  <tr v-for="group in groups" :key="group.id">
                    <td>{{ group.groupName }}</td>
                    <td>
                      <ul v-if="group.participants && group.participants.length">
                        <li v-for="participant in group.participants" :key="participant.id">
                          {{ participant.fullName }} ({{ participant.roleName }})
                        </li>
                      </ul>
                      <span v-else>No participants</span>
                    </td>
                  </tr>
                </tbody>
              </table>
            </div>
            <div class="modal-footer">
              <button type="button" class="btn btn-secondary" @click="closeMainModal">Close</button>
            </div>
          </div>
        </div>
      </div>
  
      <!-- Модальное окно для добавления группы -->
      <div class="modal-overlay" v-if="showAddGroup">
        <div class="modal-dialog">
          <div class="modal-content">
            <div class="modal-header">
              <h5 class="modal-title">Add New Group</h5>
              <button type="button" class="btn-close" @click="closeAddGroupModal"></button>
            </div>
            <div class="modal-body">
              <!-- Форма ввода названия группы -->
              <div class="mb-3">
                <label for="groupNameInput" class="form-label">Group Name</label>
                <input
                  type="text"
                  id="groupNameInput"
                  class="form-control"
                  v-model="groupName"
                  placeholder="Enter group name"
                />
              </div>
              <!-- Шаблоны названий групп -->
              <div class="mb-3">
                <label class="form-label">Templates</label>
                <div>
                  <button
                    v-for="(template, index) in groupTemplates"
                    :key="index"
                    type="button"
                    class="btn btn-outline-secondary btn-sm me-2 mb-2"
                    @click="applyTemplate(template)"
                  >
                    {{ template }}
                  </button>
                </div>
              </div>
              <!-- Уведомление об успешном создании группы -->
              <div v-if="notificationMessage" class="alert alert-success">
                {{ notificationMessage }}
              </div>
              <!-- Вывод ошибки, если создание группы завершилось неудачно -->
              <div v-if="errorMessage" class="alert alert-danger">
                {{ errorMessage }}
              </div>
            </div>
            <div class="modal-footer">
              <!-- Кнопка создания группы -->
              <button type="button" class="btn btn-primary" @click="createGroup" :disabled="isSaving">
                {{ isSaving ? 'Saving...' : 'Save Group' }}
              </button>
              <!-- Кнопка закрытия формы -->
              <button type="button" class="btn btn-secondary" @click="closeAddGroupModal">
                Close
              </button>
            </div>
          </div>
        </div>
      </div>
    </div>
  </template>
  
  <script lang="ts">
  import { defineComponent } from 'vue';
  
  export default defineComponent({
    name: 'ParticipantSection',
    props: {
      show: {
        type: Boolean,
        required: true
      },
      projectId: {
        type: [Number, String],
        required: true
      }
    },
    data() {
      return {
        groups: [] as any[],
        showAddGroup: false,
        groupName: '',
        groupTemplates: ['Team Alpha', 'Team Beta', 'Team Gamma'],
        notificationMessage: '',
        errorMessage: '',
        isSaving: false
      };
    },
    methods: {
      closeMainModal() {
        this.$emit('close');
      },
      openAddGroupModal() {
        // Сброс значений для формы
        this.groupName = '';
        this.notificationMessage = '';
        this.errorMessage = '';
        this.showAddGroup = true;
      },
      closeAddGroupModal() {
        this.showAddGroup = false;
      },
      applyTemplate(template: string) {
        this.groupName = template;
      },
      async loadGroups() {
        if (!this.projectId) {
          console.error("Project ID is undefined");
          return;
        }
        try {
          console.log(`Fetching groups for project ID: ${this.projectId}`);
          const response = await fetch(`${import.meta.env.VITE_API_BASE_URL}/Projects/${this.projectId}/Groups`, {
            headers: { 'Authorization': 'Bearer ' + localStorage.getItem('jwtToken') }
          });
          if (!response.ok) {
            console.error("Error loading participant groups:", response.statusText);
            return;
          }
          this.groups = await response.json();
          console.log("Loaded groups:", this.groups);
        } catch (error) {
          console.error("Error fetching participant groups:", error);
        }
      },
      async createGroup() {
        // Очистка предыдущих уведомлений
        this.notificationMessage = '';
        this.errorMessage = '';
  
        if (!this.groupName.trim()) {
          this.errorMessage = "Please enter a group name.";
          return;
        }
        if (!this.projectId) {
          this.errorMessage = "Project ID not found.";
          return;
        }
        this.isSaving = true;
        try {
          const response = await fetch(`${import.meta.env.VITE_API_BASE_URL}/Projects/${this.projectId}/Groups`, {
            method: 'POST',
            headers: { 'Content-Type': 'application/json' },
            body: JSON.stringify({ projectId: parseInt(this.projectId as string), groupName: this.groupName.trim() })
          });
          if (response.ok) {
            this.notificationMessage = "Group created successfully!";
            // Перезагружаем список групп, но форма остается открытой
            this.loadGroups();
          } else {
            const errorText = await response.text();
            if (response.status === 409) {
              this.errorMessage = "A group with the same name already exists for this project.";
            } else {
              this.errorMessage = "Error creating group: " + errorText;
            }
          }
        } catch (error: any) {
          console.error("Error creating group:", error);
          this.errorMessage = "Error creating group. See console for details.";
        } finally {
          this.isSaving = false;
        }
      }
    },
    watch: {
      show(newVal) {
        if (newVal) {
          this.loadGroups();
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
    width: 100%;
    height: 100%;
    background: rgba(0, 0, 0, 0.5);
    display: flex;
    align-items: center;
    justify-content: center;
    z-index: 1050;
  }
  
  .modal-dialog {
    background: #fff;
    border-radius: 5px;
    width: 90%;
    max-width: 800px;
  }
  
  .btn-close {
    background: none;
    border: none;
    font-size: 1.5rem;
  }
  
  .alert {
    margin-top: 1rem;
  }
  </style>
  