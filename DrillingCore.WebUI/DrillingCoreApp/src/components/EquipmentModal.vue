<template>
    <div class="modal-overlay" @click.self="closeModal">
      <div class="modal-dialog modal-lg">
        <div class="modal-content">
          <div class="modal-header">
            <h5 class="modal-title">{{ equipmentId ? 'Edit Equipment' : 'Add Equipment' }}</h5>
            <button type="button" class="btn-close" @click="closeModal">&times;</button>
          </div>
          <div class="modal-body modal-scroll">
            <form @submit.prevent="submitEquipment" id="equipmentForm">
              <div class="mb-3">
                <label class="form-label">Name</label>
                <input type="text" class="form-control" v-model="equipment.name" required />
              </div>
              <div class="mb-3">
                <label class="form-label">Type</label>
                <select class="form-select" v-model="equipment.typeId" required>
                  <option value="">Select Type</option>
                  <option v-for="type in equipmentTypes" :key="type.id" :value="type.id">
                    {{ type.typeName }}
                  </option>
                </select>
              </div>
              <div class="mb-3">
                <label class="form-label">Registration Number</label>
                <input type="text" class="form-control" v-model="equipment.registrationNumber" required />
              </div>
              <!-- Можно добавить другие поля, если нужно -->
            </form>
            <div v-if="notificationMessage" class="alert alert-success">{{ notificationMessage }}</div>
            <div v-if="errorMessage" class="alert alert-danger">{{ errorMessage }}</div>
          </div>
          <div class="modal-footer">
            <button class="btn btn-success" @click="submitEquipment">{{ equipmentId ? 'Save Changes' : 'Save Equipment' }}</button>
            <button class="btn btn-secondary" @click="closeModal">Cancel</button>
          </div>
        </div>
      </div>
    </div>
  </template>
  
  <script lang="ts">
  import { defineComponent } from 'vue';
  
  export default defineComponent({
    name: 'EquipmentModal',
    props: {
      equipmentId: {
        type: Number,
        default: null
      }
    },
    data() {
      return {
        equipment: {
          id: 0,
          name: '',
          typeId: null as number | null,
          registrationNumber: ''
        },
        equipmentTypes: [] as any[],
        notificationMessage: '',
        errorMessage: ''
      };
    },
    mounted() {
      this.loadEquipmentTypes();
      if (this.equipmentId) {
        this.loadEquipmentData(this.equipmentId);
      }
    },
    methods: {
      async loadEquipmentTypes() {
        try {
          const response = await fetch(`${import.meta.env.VITE_API_BASE_URL}/EquipmentType`, { credentials: 'include' });
          if (response.ok) {
            this.equipmentTypes = await response.json();
          } else {
            console.error('Error loading equipment types:', response.statusText);
          }
        } catch (error) {
          console.error('Error loading equipment types:', error);
        }
      },
      async loadEquipmentData(id: number) {
        try {
          const response = await fetch(`${import.meta.env.VITE_API_BASE_URL}/Equipment/${id}`, { credentials: 'include' });
          if (response.ok) {
            const data = await response.json();
            this.equipment = {
              id: data.id,
              name: data.name,
              typeId: data.typeId,
              registrationNumber: data.registrationNumber
            };
          } else {
            console.error('Error loading equipment data:', response.statusText);
          }
        } catch (error) {
          console.error('Error loading equipment data:', error);
        }
      },
      async submitEquipment() {
        this.notificationMessage = '';
        this.errorMessage = '';
        try {
          const url = this.equipmentId
            ? `${import.meta.env.VITE_API_BASE_URL}/Equipment/${this.equipmentId}`
            : `${import.meta.env.VITE_API_BASE_URL}/Equipment`;
          const method = this.equipmentId ? 'PUT' : 'POST';
          const response = await fetch(url, {
            method,
            headers: { 'Content-Type': 'application/json' },
            body: JSON.stringify(this.equipment),
            credentials: 'include'
          });
          if (response.ok) {
            this.notificationMessage = "Equipment saved successfully!";
            this.$emit('equipment-saved');
            setTimeout(() => this.closeModal(), 1000);
          } else {
            const errorText = await response.text();
            this.errorMessage = "Error saving equipment: " + errorText;
          }
        } catch (error) {
          console.error("Error saving equipment:", error);
          this.errorMessage = "Error saving equipment. See console for details.";
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
    background: rgba(0, 0, 0, 0.6);
    display: flex;
    align-items: center;
    justify-content: center;
    z-index: 1100;
    padding: 1rem;
  }
  .modal-dialog {
    background: #fff;
    border-radius: 8px;
    width: 100%;
    max-width: 600px;
    box-shadow: 0 4px 12px rgba(0,0,0,0.2);
    overflow: hidden;
  }
  .modal-content {
    display: flex;
    flex-direction: column;
    max-height: 80vh;
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
    overflow-y: auto;
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
  