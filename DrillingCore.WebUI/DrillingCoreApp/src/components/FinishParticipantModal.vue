<template>
    <div class="modal-overlay" @click.self="closeModal">
      <div class="modal-dialog">
        <div class="modal-content">
          <!-- Заголовок модального окна -->
          <div class="modal-header">
            <h5 class="modal-title">Finish Participant</h5>
            <button type="button" class="btn-close" @click="closeModal">&times;</button>
          </div>
          <!-- Тело модального окна -->
          <div class="modal-body">
            <p>
              Specify the finish date for 
              <strong>{{ participantName }}</strong>
            </p>
            <div class="mb-3">
              <label for="finishDate" class="form-label">Finish Date</label>
              <input
                type="date"
                id="finishDate"
                class="form-control"
                v-model="finishDate"
              />
            </div>
            <!-- Уведомления -->
            <div v-if="notificationMessage" class="alert alert-success">
              {{ notificationMessage }}
            </div>
            <div v-if="errorMessage" class="alert alert-danger">
              {{ errorMessage }}
            </div>
          </div>
          <!-- Футер модального окна -->
          <div class="modal-footer">
            <button class="btn btn-primary" @click="saveFinish" :disabled="isSaving">
              {{ isSaving ? 'Saving...' : 'Save' }}
            </button>
            <button class="btn btn-secondary" @click="closeModal">Cancel</button>
          </div>
        </div>
      </div>
    </div>
  </template>
  
  <script lang="ts">
  import { defineComponent } from 'vue';
  
  export default defineComponent({
    name: 'FinishParticipantModal',
    props: {
      projectId: {
        type: Number,
        required: true
      },
      participantId: {
        type: Number,
        required: true
      },
      participantName: {
        type: String,
        required: true
      }
    },
    data() {
      return {
        finishDate: '',
        notificationMessage: '',
        errorMessage: '',
        isSaving: false
      };
    },
    methods: {
      closeModal() {
        this.$emit('close');
      },
      async saveFinish() {
        if (!this.finishDate) {
          this.errorMessage = "Please select a finish date.";
          return;
        }
        this.isSaving = true;
        this.notificationMessage = '';
        this.errorMessage = '';
        try {
          // Формируем команду для завершения участника
          const command = {
            ProjectId: this.projectId,
            ParticipantId: this.participantId,
            FinishDate: this.finishDate
          };
          const response = await fetch(
            `${import.meta.env.VITE_API_BASE_URL}/Projects/${this.projectId}/Participants/${this.participantId}/Finish`,
            {
              method: 'PUT',
              headers: { 'Content-Type': 'application/json' },
              body: JSON.stringify(command),
              credentials: 'include'
            }
          );
          if (response.ok) {
            this.notificationMessage = "Participant finished successfully!";
            // Эмитим событие для обновления списка участников
            this.$emit('finished');
          } else {
            const errorText = await response.text();
const maxLength = 1000;
const shortErrorText = errorText.length > maxLength ? errorText.slice(0, maxLength) + '...' : errorText;
this.errorMessage = "Error finishing participant: " + shortErrorText;
      
          }
        } catch (error: any) {
          console.error("Error finishing participant:", error);
          this.errorMessage = "Error finishing participant. See console for details.";
        } finally {
          this.isSaving = false;
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
    justify-content: center;
    align-items: center;
    z-index: 1100;
    padding: 1rem;
  }
  
  .modal-dialog {
    background: #fff;
    border-radius: 8px;
    width: 90%;
    max-width: 400px;
    box-shadow: 0 4px 12px rgba(0,0,0,0.2);
    overflow: hidden;
  }
  
  .modal-content {
    display: flex;
    flex-direction: column;
  }
  
  .modal-header {
    display: flex;
    justify-content: space-between;
    align-items: center;
    padding: 1rem;
    background-color: #fff;
    border-bottom: 1px solid #ddd;
  }
  
  .modal-title {
    margin: 0;
    font-size: 1.25rem;
    color: #333;
  }
  
  .btn-close {
    background: transparent;
    border: none;
    font-size: 1.5rem;
    cursor: pointer;
  }
  
  .modal-body {
    padding: 1rem;
  }
  
  .form-control {
    width: 100%;
    padding: 0.5rem;
    border: 1px solid #ddd;
    border-radius: 4px;
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
    font-size: 0.9rem;
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
  