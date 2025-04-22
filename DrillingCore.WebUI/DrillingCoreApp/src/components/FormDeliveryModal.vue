<template>
    <div class="modal-overlay" @click.self="close">
      <div class="modal-content">
        <div class="modal-header">
          <h3>Form Delivery Settings</h3>
          <button class="btn-close" @click="close">&times;</button>
        </div>
  
        <div class="modal-body">
          <!-- Тип формы -->
          <div class="form-group">
            <label>Form Type</label>
            <select v-model="formTypeId" class="form-control">
              <option disabled value="">-- Select --</option>
              <option v-for="t in formTypes" :key="t.id" :value="t.id">
                {{ t.name }}
              </option>
            </select>
          </div>
  
          <!-- Условие -->
          <div class="form-group">
            <label>Condition</label>
            <select v-model="condition" class="form-control">
              <option :value="1">After All Participants Signed</option>
              <option :value="2">After Each Form Completed</option>
              <option :value="3">Scheduled (Daily at 20:00)</option>
            </select>
          </div>
  
          <!-- Получатели -->
          <div class="recipients-block">
            <h5>Recipients</h5>
            <div
              v-for="(recipient, index) in recipients"
              :key="index"
              class="recipient-entry"
            >
              <input v-model="recipient.fullName" placeholder="Full Name" class="form-control" />
              <input v-model="recipient.company" placeholder="Company" class="form-control" />
              <input v-model="recipient.email" placeholder="Email" class="form-control" />
              <button class="btn btn-danger" @click="removeRecipient(index)">Remove</button>
            </div>
            <button class="btn btn-secondary mt-2" @click="addRecipient">+ Add Recipient</button>
          </div>
  
          <!-- Ошибки -->
          <div v-if="errorMessage" class="alert alert-danger">
            {{ errorMessage }}
          </div>
          <div v-if="successMessage" class="alert alert-success">
            {{ successMessage }}
          </div>
        </div>
  
        <div class="modal-footer">
          <button class="btn btn-success" @click="saveSettings">Save</button>
          <button class="btn btn-secondary" @click="close">Cancel</button>
        </div>
      </div>
    </div>
  </template>
  
  <script lang="ts">
  import { defineComponent, onMounted, ref, watch } from 'vue';
  
  export default defineComponent({
    name: 'FormDeliveryModal',
    props: {
      projectId: { type: Number, required: true },
      rule: { type: Object as () => any, required: false }
    },
    emits: ['close'],
    setup(props, { emit }) {
      const formTypeId = ref<number | null>(null);
      const condition = ref<number>(1);
      const recipients = ref([{ fullName: '', company: '', email: '' }]);
      const formTypes = ref<{ id: number; name: string }[]>([]);
      const errorMessage = ref('');
      const successMessage = ref('');
  
      const close = () => emit('close');
  
      const mapConditionToNumber = (conditionStr: string): number => {
        switch (conditionStr) {
          case 'AfterAllParticipantsSigned': return 1;
          case 'AfterEachFormCompleted': return 2;
          case 'ScheduledDailyAt2000': return 3;
          default: return 1;
        }
      };
  
      const loadFormTypes = async () => {
        try {
          const res = await fetch(`${import.meta.env.VITE_API_BASE_URL}/forms/form-types`);
          if (!res.ok) throw new Error('Failed to load form types');
          formTypes.value = await res.json();
        } catch (err) {
          console.error(err);
          errorMessage.value = 'Error loading form types';
        }
      };
  
      const initializeFromRule = () => {
        if (props.rule) {
          formTypeId.value = props.rule.formTypeId ?? null;
          condition.value = mapConditionToNumber(props.rule.condition);
          recipients.value = props.rule.recipients.map((r: any) => ({
            fullName: r.fullName,
            company: r.company,
            email: r.email
          }));
        }
      };
  
      const addRecipient = () => {
        recipients.value.push({ fullName: '', company: '', email: '' });
      };
  
      const removeRecipient = (index: number) => {
        recipients.value.splice(index, 1);
      };
  
      const saveSettings = async () => {
        errorMessage.value = '';
        successMessage.value = '';
  
        if (!formTypeId.value || recipients.value.length === 0) {
          errorMessage.value = 'Please fill out all required fields.';
          return;
        }
  
        if (!recipients.value.every(r => r.fullName && r.company && r.email)) {
          errorMessage.value = 'All recipient fields must be filled.';
          return;
        }
  
        const payload = {
          projectId: props.projectId,
          formTypeId: formTypeId.value,
          condition: condition.value,
          recipients: recipients.value
        };
  
        try {
          const url = props.rule?.id
            ? `${import.meta.env.VITE_API_BASE_URL}/FormDeliveryRules/${props.rule.id}`
            : `${import.meta.env.VITE_API_BASE_URL}/FormDeliveryRules`;
          const method = props.rule?.id ? 'PUT' : 'POST';
  
          const res = await fetch(url, {
            method,
            headers: { 'Content-Type': 'application/json' },
            body: JSON.stringify({ ...payload, id: props.rule?.id })
          });
  
          if (!res.ok) throw new Error('Failed to save settings');
          successMessage.value = 'Settings saved successfully.';
          setTimeout(() => close(), 1000);
        } catch (err) {
          errorMessage.value = 'Error saving settings.';
          console.error(err);
        }
      };
  
      onMounted(async () => {
        await loadFormTypes();
        initializeFromRule();
      });
  
      watch(() => props.rule, initializeFromRule, { immediate: true });
  
      return {
        formTypeId,
        condition,
        recipients,
        formTypes,
        addRecipient,
        removeRecipient,
        saveSettings,
        close,
        errorMessage,
        successMessage
      };
    }
  });
  </script>
  
  <style scoped>
  .modal-overlay {
    position: fixed;
    inset: 0;
    background: rgba(0, 0, 0, 0.6);
    display: flex;
    align-items: center;
    justify-content: center;
    z-index: 9999;
  }
  .modal-content {
    background: white;
    width: 95%;
    max-width: 700px;
    padding: 1rem;
    border-radius: 8px;
    max-height: 90vh;
    overflow-y: auto;
  }
  .modal-header {
    display: flex;
    justify-content: space-between;
    align-items: center;
  }
  .form-group {
    margin-bottom: 1rem;
  }
  .form-control {
    width: 100%;
    padding: 0.5rem;
    margin-top: 0.3rem;
    margin-bottom: 0.5rem;
    border: 1px solid #ccc;
    border-radius: 4px;
  }
  .recipient-entry {
    display: flex;
    gap: 0.5rem;
    margin-bottom: 0.5rem;
  }
  .btn {
    padding: 0.4rem 1rem;
    border: none;
    border-radius: 4px;
    cursor: pointer;
  }
  .btn-success {
    background-color: #28a745;
    color: white;
  }
  .btn-secondary {
    background-color: #6c757d;
    color: white;
  }
  .btn-danger {
    background-color: #dc3545;
    color: white;
  }
  .btn-close {
    font-size: 1.5rem;
    background: none;
    border: none;
  }
  .alert {
    padding: 0.5rem;
    border-radius: 4px;
    margin-top: 0.5rem;
  }
  .alert-danger {
    background-color: #f8d7da;
    color: #721c24;
  }
  .alert-success {
    background-color: #d4edda;
    color: #155724;
  }
  </style>
  