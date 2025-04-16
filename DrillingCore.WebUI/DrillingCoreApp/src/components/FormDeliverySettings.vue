<template>
    <div class="delivery-settings">
      <h4>Delivery Settings for Project #{{ projectId }}</h4>
  
      <div v-if="loading">Loading rules...</div>
  
      <div v-else>
        <div v-if="rules.length === 0" class="no-rules">No delivery rules yet.</div>
  
        <table v-else class="table table-bordered mt-3">
          <thead>
            <tr>
              <th>Form Type</th>
              <th>Condition</th>
              <th>Recipients</th>
              <th>Actions</th>
            </tr>
          </thead>
          <tbody>
            <tr v-for="rule in rules" :key="rule.id">
              <td>{{ rule.formTypeName }}</td>
              <td>{{ formatCondition(rule.condition) }}</td>
              <td>
                <ul>
                  <li v-for="r in rule.recipients" :key="r.email">{{ r.email }}</li>
                </ul>
              </td>
              <td>
                <button class="btn btn-sm btn-secondary me-2" @click="$emit('edit-rule', rule)">Edit</button>
                <button class="btn btn-sm btn-danger" @click="confirmDelete(rule.id)">Delete</button>
              </td>
            </tr>
          </tbody>
        </table>
  
        <button class="btn btn-primary mt-3" @click="$emit('add-new')">+ Add New Rule</button>
      </div>
  
      <Modal v-if="showDeleteModal" @close="cancelDelete">
        <div style="padding: 1rem">
          <h5>Confirm Delete</h5>
          <p>Are you sure you want to delete this rule?</p>
          <div class="d-flex justify-content-end mt-3">
            <button class="btn btn-danger me-2" @click="deleteRule">Delete</button>
            <button class="btn btn-secondary" @click="cancelDelete">Cancel</button>
          </div>
        </div>
      </Modal>
    </div>
  </template>
  
  <script lang="ts">
  import { defineComponent, onMounted, ref } from 'vue';
  import Modal from './Modal.vue';
  
  export default defineComponent({
    name: 'FormDeliverySettings',
    components: { Modal },
    props: {
      projectId: { type: Number, required: true }
    },
    emits: ['add-new', 'edit-rule'],
    setup(props) {
      const rules = ref<any[]>([]);
      const loading = ref(true);
      const showDeleteModal = ref(false);
      const ruleToDeleteId = ref<number | null>(null);
  
      const loadRules = async () => {
        loading.value = true;
        try {
          const res = await fetch(`/api/FormDeliveryRules/project/${props.projectId}`);
          rules.value = await res.json();
        } catch (err) {
          console.error(err);
          rules.value = [];
        } finally {
          loading.value = false;
        }
      };
  
      const formatCondition = (condition: string | number) => {
        switch (condition) {
          case 'AfterEachFormCompleted':
          case 2: return 'After Each Form is Completed';
          case 'AfterAllParticipantsSigned':
          case 1: return 'After All Participants Signed';
          case 'ScheduledDailyAt2000':
          case 3: return 'Scheduled (Daily at 20:00)';
          default: return 'Unknown';
        }
      };
  
      const confirmDelete = (id: number) => {
        ruleToDeleteId.value = id;
        showDeleteModal.value = true;
      };
  
      const cancelDelete = () => {
        showDeleteModal.value = false;
        ruleToDeleteId.value = null;
      };
  
      const deleteRule = async () => {
        try {
          await fetch(`/api/FormDeliveryRules/${ruleToDeleteId.value}`, { method: 'DELETE' });
          await loadRules();
        } catch (e) {
          console.error(e);
        } finally {
          cancelDelete();
        }
      };
  
      onMounted(loadRules);
  
      return {
        rules,
        loading,
        formatCondition,
        showDeleteModal,
        confirmDelete,
        cancelDelete,
        deleteRule
      };
    }
  });
  </script>
  <style scoped>
  .delivery-settings {
    padding: 1rem;
  }
  
  .delivery-table {
    width: 100%;
    border-collapse: collapse;
    margin-top: 1rem;
  }
  
  .delivery-table th,
  .delivery-table td {
    border: 1px solid #ccc;
    padding: 0.75rem;
    text-align: left;
    vertical-align: top;
  }
  
  .delivery-table th {
    background-color: #f0f0f0;
  }
  
  .recipient-list {
    margin: 0;
    padding-left: 1rem;
    list-style-type: disc;
    font-size: 0.9rem;
  }
  
  .no-rules {
    font-style: italic;
    color: #777;
    margin-top: 1rem;
  }
  
  .btn {
    margin-right: 0.3rem;
  }
  </style>
  