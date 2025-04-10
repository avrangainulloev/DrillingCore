<template>
    <div class="modal-overlay" @click.self="closeModal">
      <div class="modal-dialog small-dialog">
        <div class="modal-content attach-date-modal">
          <div class="modal-header">
            <h5 class="modal-title">{{ modalTitle }}</h5>
            <button type="button" class="btn-close" @click="closeModal">&times;</button>
          </div>
          <div class="modal-body">
            <label for="date-input">{{ dateLabel }}</label>
            <input type="date" id="date-input" v-model="dateInput" class="form-control" />
          </div>
          <div class="modal-footer">
            <button class="btn btn-success" @click="submitDate">{{ submitButtonText }}</button>
            <button class="btn btn-secondary" @click="closeModal">Cancel</button>
          </div>
        </div>
      </div>
    </div>
  </template>
    
  <script lang="ts">
  import { defineComponent } from 'vue';
  
  export default defineComponent({
    name: 'AttachDateModal',
    props: {
      mode: {
        type: String,
        required: true // "attach" или "detach"
      },
      projectId: {
        type: Number,
        required: true
      },
      participantId: {
        type: Number,
        required: true
      },
      // Для режима "attach" требуется equipmentId
      equipmentId: {
        type: Number,
        required: false,
        default: null
      },
      // Для режима "detach" требуется assignmentId
      assignmentId: {
        type: Number,
        required: false,
        default: null
      }
    },
    data() {
      return {
        dateInput: new Date().toISOString().substring(0, 10)
      };
    },
    computed: {
      isAttachMode(): boolean {
        return this.mode === 'attach';
      },
      modalTitle(): string {
        return this.isAttachMode ? 'Enter Attach Date' : 'Enter Detach Date';
      },
      dateLabel(): string {
        return this.isAttachMode ? 'Attach Date:' : 'Detach Date:';
      },
      submitButtonText(): string {
        return this.isAttachMode ? 'Attach' : 'Detach';
      }
    },
    methods: {
      submitDate() {
        console.log(`EquipmentID ${this.assignmentId}` )
        const payload = this.isAttachMode 
          ? { projectId: this.projectId, participantId: this.participantId, equipmentId: this.equipmentId, startDate: this.dateInput }
          : { id: this.assignmentId, endDate: this.dateInput };
        this.$emit('submitted', payload);
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
    background-color: rgba(0,0,0,0.6);
    display: flex;
    justify-content: center;
    align-items: center;
    z-index: 1100;
    padding: 1rem;
  }
  .small-dialog {
    max-width: 400px;
  }
  .attach-date-modal {
    background-color: #fff;
    padding: 1rem;
    border-radius: 8px;
    width: 100%;
    box-shadow: 0 4px 12px rgba(0,0,0,0.2);
  }
  .modal-header {
    display: flex;
    justify-content: space-between;
    align-items: center;
    border-bottom: 1px solid #ddd;
    padding-bottom: 0.5rem;
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
    padding: 1rem 0;
  }
  .modal-footer {
    display: flex;
    justify-content: flex-end;
    gap: 0.5rem;
    border-top: 1px solid #ddd;
    padding-top: 0.5rem;
  }
  .btn {
    padding: 0.4rem 0.8rem;
    border: none;
    border-radius: 4px;
    font-size: 0.9rem;
    cursor: pointer;
    transition: background-color 0.3s ease;
  }
  .btn:hover {
    background-color: #1565c0;
  }
  .btn-success {
    background-color: #1976d2;
    color: #fff;
  }
  .btn-success:hover {
    background-color: #1565c0;
  }
  .btn-secondary {
    background-color: #6c757d;
    color: #fff;
  }
  .btn-secondary:hover {
    background-color: #5a6268;
  }
  .form-control {
    width: 100%;
    padding: 0.4rem;
    border: 1px solid #ccc;
    border-radius: 4px;
    font-size: 0.9rem;
  }
  </style>
  