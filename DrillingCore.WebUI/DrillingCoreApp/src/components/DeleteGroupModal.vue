<template>
    <div class="modal-overlay" @click.self="closeModal">
      <div class="modal-dialog">
        <div class="modal-content">
          <!-- Заголовок -->
          <div class="modal-header">
            <h5 class="modal-title">Delete Empty Groups</h5>
            <button type="button" class="btn-close" @click="closeModal">&times;</button>
          </div>
          <!-- Основное содержимое -->
          <div class="modal-body">
            <!-- Если группы ещё не загрузились -->
            <div v-if="loading">Loading groups...</div>
            <!-- Если группы загружены, но список пуст -->
            <div v-else-if="emptyGroups.length === 0">No empty groups found.</div>
            <!-- Список пустых групп с чекбоксами -->
            <div v-else>
              <div v-for="group in emptyGroups" :key="group.groupName" class="form-check">
                <input
                  type="checkbox"
                  class="form-check-input"
                  :id="`deleteGroup_${group.groupName}`"
                  :value="group.groupName"
                  v-model="selectedGroups"
                />
                <label class="form-check-label" :for="`deleteGroup_${group.groupName}`">
                  {{ group.groupName }}
                </label>
              </div>
            </div>
            <!-- Уведомления -->
            <div v-if="notificationMessage" class="alert alert-success">
              {{ notificationMessage }}
            </div>
            <div v-if="errorMessage" class="alert alert-danger">
              {{ errorMessage }}
            </div>
          </div>
          <!-- Футер -->
          <div class="modal-footer">
            <button type="button" class="btn btn-primary" @click="deleteGroups" :disabled="isDeleting || selectedGroups.length === 0">
              {{ isDeleting ? 'Deleting...' : 'Delete Selected' }}
            </button>
            <button type="button" class="btn btn-secondary" @click="closeModal">Close</button>
          </div>
        </div>
      </div>
    </div>
  </template>
  
  <script lang="ts">
  import { defineComponent } from 'vue';
  
  export default defineComponent({
    name: 'DeleteGroupModal',
    props: {
      projectId: {
        type: Number,
        required: true
      }
    },
    data() {
      return {
        emptyGroups: [] as any[],
        selectedGroups: [] as string[],
        notificationMessage: '',
        errorMessage: '',
        isDeleting: false,
        loading: false
      };
    },
    mounted() {
      this.loadEmptyGroups();
    },
    methods: {
      closeModal() {
        this.$emit('close');
      },
      async loadEmptyGroups() {
        this.loading = true;
        try {
          const response = await fetch(`${import.meta.env.VITE_API_BASE_URL}/Projects/${this.projectId}/Groups/emptygroups`, { credentials: 'include' });
          if (response.ok) {
            const data = await response.json();
            this.emptyGroups = data;
          } else {
            this.errorMessage = "Error loading empty groups: " + response.statusText;
          }
        } catch (error) {
          console.error("Error loading empty groups:", error);
          this.errorMessage = "Error loading empty groups. See console for details.";
        } finally {
          this.loading = false;
        }
      },
      async deleteGroups() {
        // Очистка предыдущих уведомлений
        this.notificationMessage = '';
        this.errorMessage = '';
  
        if (this.selectedGroups.length === 0) {
          this.errorMessage = "Please select at least one group to delete.";
          return;
        }
        this.isDeleting = true;
        try {
          const response = await fetch(`${import.meta.env.VITE_API_BASE_URL}/Projects/${this.projectId}/Groups/emptygroups`, {
            method: 'DELETE',
            headers: { 'Content-Type': 'application/json' },
            body: JSON.stringify({ projectId: this.projectId, groups: this.selectedGroups }),
            credentials: 'include'
          });
          if (response.ok) {
            this.notificationMessage = "Selected group(s) deleted successfully!";
            // После успешного удаления обновляем список
            this.loadEmptyGroups();
            // Очищаем выбранные группы
            this.selectedGroups = [];
            // Можно также эмитить событие для обновления родительского компонента
            this.$emit('groups-deleted');
          } else {
            const errorText = await response.text();
            this.errorMessage = "Error deleting groups: " + errorText;
          }
        } catch (error: any) {
          console.error("Error deleting groups:", error);
          this.errorMessage = "Error deleting groups. See console for details.";
        } finally {
          this.isDeleting = false;
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
    max-width: 500px;
    box-shadow: 0 4px 15px rgba(0, 0, 0, 0.3);
    overflow: hidden;
  }
  
  .modal-content {
    display: flex;
    flex-direction: column;
  }
  
  /* Заголовок */
  .modal-header {
    display: flex;
    justify-content: space-between;
    align-items: center;
    padding: 1rem;
    background-color: #fff; /* нейтральный фон */
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
  
  /* Основное содержимое */
  .modal-body {
    padding: 1rem;
  }
  
  /* Стили для чекбоксов */
  .form-check {
    margin-bottom: 0.5rem;
  }
  
  .form-check-input {
    margin-right: 0.5rem;
  }
  
  /* Уведомления */
  .alert {
    padding: 0.5rem 0.75rem;
    border-radius: 4px;
    margin-top: 0.5rem;
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
  
  /* Футер */
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
  </style>
  