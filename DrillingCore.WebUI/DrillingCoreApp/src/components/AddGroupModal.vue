<template>
    <div class="modal-overlay" @click.self="closeModal">
      <div class="modal-dialog">
        <div class="modal-content">
          <div class="modal-header">
            <h5 class="modal-title">Add New Group</h5>
            <button type="button" class="btn-close" @click="closeModal">&times;</button>
          </div>
          <div class="modal-body">
            <div class="form-group">
              <label for="groupNameInput" class="form-label">Group Name</label>
              <input
                type="text"
                id="groupNameInput"
                class="form-control"
                v-model="groupName"
                placeholder="Enter group name"
              />
            </div>
            <div class="form-group templates">
              <label class="form-label">Templates</label>
              <div class="templates-buttons">
                <button
                  v-for="(template, index) in groupTemplates"
                  :key="index"
                  type="button"
                  class="btn btn-outline-secondary btn-template"
                  @click="applyTemplate(template)"
                >
                  {{ template }}
                </button>
              </div>
            </div>
            <div v-if="notificationMessage" class="alert alert-success">
              {{ notificationMessage }}
            </div>
            <div v-if="errorMessage" class="alert alert-danger">
              {{ errorMessage }}
            </div>
          </div>
          <div class="modal-footer">
            <button type="button" class="btn btn-primary" @click="createGroup" :disabled="isSaving">
              {{ isSaving ? 'Saving...' : 'Save Group' }}
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
    name: 'AddGroupModal',
    props: {
      projectId: {
        type: Number,
        required: true
      }
    },
    data() {
      return {
        groupName: '',
        groupTemplates: ['Drill Crew', 'Drill push', 'Water Truck Driver'],
        notificationMessage: '',
        errorMessage: '',
        isSaving: false
      };
    },
    methods: {
      closeModal() {
        this.$emit('close');
      },
      applyTemplate(template: string) {
        this.groupName = template;
      },
      async createGroup() {
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
            body: JSON.stringify({ projectId: this.projectId, groupName: this.groupName.trim() }),
            credentials: 'include'
            
          });
          if (response.ok) {
            this.notificationMessage = "Group created successfully!";
            this.groupName = '';
            this.$emit('group-created');
          } else {
            const errorText = await response.text();
            this.errorMessage = response.status === 409 
              ? "A group with the same name already exists for this project."
              : "Error creating group: " + errorText;
          }
        } catch (error: any) {
          console.error("Error creating group:", error);
          this.errorMessage = "Error creating group. See console for details.";
        } finally {
          this.isSaving = false;
        }
      }
    }
  });
  </script>
  
  <style scoped>
  /* Общий фон модального окна */
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
  
  /* Диалог модального окна */
  .modal-dialog {
    background: #fff;
    border-radius: 8px;
    width: 100%;
    max-width: 500px;
    box-shadow: 0 4px 15px rgba(0, 0, 0, 0.3);
    overflow: hidden;
  }
  
  /* Контейнер модального содержимого */
  .modal-content {
    display: flex;
    flex-direction: column;
  }
  
  /* Шапка модального окна */
  .modal-header {
  display: flex;
  justify-content: space-between;
  align-items: center;
  padding: 1rem;
  background-color: #fff; /* заменили синий на белый */
  color: #333;          /* нейтральный цвет текста */
  border-bottom: 1px solid #ddd; /* можно добавить разделительную линию */
}
  
  /* Заголовок модального окна */
  .modal-title {
    margin: 0;
    font-size: 1.25rem;
  }
  
  /* Кнопка закрытия */
  .btn-close {
    background: transparent;
    border: none;
    font-size: 1.5rem;
    color: #fff;
    cursor: pointer;
  }
  
  /* Основное содержимое */
  .modal-body {
    padding: 1rem;
  }
  
  /* Группа элементов формы */
  .form-group {
    margin-bottom: 1rem;
  }
  
  /* Поля ввода */
  .form-control {
    width: 100%;
    padding: 0.5rem;
    border: 1px solid #ddd;
    border-radius: 4px;
    font-size: 1rem;
  }
  
  /* Секция шаблонов */
  .templates {
    margin-top: 0.5rem;
  }
  
  .templates-buttons {
    display: flex;
    flex-wrap: wrap;
    gap: 0.5rem;
  }
  
  .btn-template {
    font-size: 0.9rem;
    padding: 0.3rem 0.6rem;
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
  
  /* Футер модального окна */
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
  