<template>
  <div class="forms-section">
    <transition name="fade" mode="out-in">
      <!-- Cards View -->
      <div v-if="viewMode === 'cards'" key="cards">
        <h3 class="section-title">Select Form Type</h3>
        <div class="form-type-grid">
          <div
            v-for="type in formTypes"
            :key="type.id"
            class="form-type-card"
            :style="{ backgroundColor: type.color }"
            @click="selectFormType(type)"
          >
            <i :class="type.icon"></i>
            <span>{{ type.name }}</span>
          </div>
        </div>
      </div>

      <!-- List View -->
      <div v-else key="list">
        <div class="list-header">
          <button class="btn back-btn" @click="backToCards">Back</button>
          <h3 class="section-title">{{ selectedType?.name || '' }} Forms</h3>
          <div>
            <button class="btn send-btn" @click="openSendModal">📤 Send Forms</button>
            <button class="btn add-new-btn" @click="addNewForm">+ Add New Form</button>
          </div>
        </div>

        <div class="forms-table-container scrollable-forms">
  <table class="table forms-table" v-if="forms.length > 0">
    <thead>
      <tr>
        <th>ID</th>
        <th v-if="isInspectionType">Form Type</th>
        <th v-if="isInspectionType">Crew</th>
        <th v-if="isInspectionType">Unit</th>
        <th>Date Filled</th>
        <th v-if="selectedType?.id === 3 || selectedType?.id === 5">Created By</th>
        <th v-if="selectedType?.id === 5">Total Wells</th>
        <th v-if="selectedType?.id === 5">Total Meters</th>
        <th>Comments</th>
        <th>Status</th>
        <th>Actions</th>
      </tr>
    </thead>
    <tbody>
      <tr v-for="form in filteredForms" :key="form.id">
        <td>{{ form.id }}</td>
        <td v-if="isInspectionType">{{ form.formTypeName }}</td>
        <td v-if="isInspectionType">{{ form.crewName }}</td>
        <td v-if="isInspectionType">{{ form.unitNumber }}</td>
        <td>{{ formatDate(form.dateFilled) }}</td>
        <td v-if="selectedType?.id === 3 || selectedType?.id === 5">{{ form.creatorName }}</td>
        <td v-if="selectedType?.id === 5">{{ form.totalWells }}</td>
        <td v-if="selectedType?.id === 5">{{ form.totalMeters }}</td>
        <td>
          <div>
            <span>
              {{ commentExpanded[form.id]
                ? form.otherComments
                : form.otherComments
                  ? form.otherComments.slice(0, 100) + (form.otherComments.length > 100 ? '...' : '')
                  : '' }}
            </span>
            <a
              v-if="form.otherComments && form.otherComments.length > 100"
              href="#"
              @click.prevent="toggleComment(form.id)"
              style="color: #1976d2; margin-left: 6px; font-size: 0.9rem"
            >
              {{ commentExpanded[form.id] ? 'Show less' : 'Show more' }}
            </a>
          </div>
        </td>
        <td>{{ form.status }}</td>
        <td>
          <button class="btn edit-btn" @click="editForm(form.id)">View/Edit</button>
        </td>
      </tr>
    </tbody>
  </table>

  <div v-else class="no-data">No forms available for this type.</div>
</div>
      </div>
    </transition>

    <!-- Modals -->
    <DrillInspectionModal v-if="showDrillInspectionModal" :user-id="userId" :projectId="projectId" :form-type-id="selectedType!.id" :form-id="editingFormId ?? undefined" @close="onModalClosed" />
    <FLHAModal v-if="showFLHAModal" :user-id="userId" :project-id="projectId" :form-id="editingFormId ?? undefined" @close="onModalClosed" />
    <DrillingFormModal v-if="showDrillingFormModal" :user-id="userId" :project-id="projectId" :form-id="editingFormId ?? undefined" @close="onModalClosed" />

    <!-- Send Forms Modal -->
    <Modal v-if="showSendModal" @close="closeSendModal">
      <div class="send-modal-content">
        <h4>Select Date to Send Forms</h4>
        <div class="date-picker-row">
          <i class="bi bi-calendar3"></i>
          <input type="date" v-model="selectedDate" class="form-control" />
        </div>
        <transition name="fade">
          <div class="modal-actions">
            <button class="btn btn-secondary" @click="closeSendModal">Cancel</button>
            <button class="btn btn-primary" :disabled="!selectedDate || sending || buttonLocked" @click="sendForms">
              {{ sending ? 'Sending...' : 'Send Now' }}
            </button>
          </div>
        </transition>
        <div v-if="sendSuccess" class="success-msg">✅ Forms sent successfully.</div>
        <div v-if="sendError" class="error-msg">❌ {{ sendError }}</div>
      </div>
    </Modal>
  </div>
</template>

<script lang="ts">
import { defineComponent } from 'vue';
import DrillInspectionModal from './DrillInspectionModal.vue';
import FLHAModal from './FLHAModal.vue';
import DrillingFormModal from './DrillingFormModal.vue';
import Modal from './Modal.vue';

interface FormDto {
  id: number;
  formTypeName?: string;
  crewName?: string;
  unitNumber?: string;
  dateFilled: string;
  otherComments?: string;
  creatorName?: string;
  status?: string;
  totalWells?: number;
  totalMeters?: number;
}

interface FormType {
  id: number;
  name: string;
  icon: string;
  color: string;
}

export default defineComponent({
  name: 'FormsSection',
  components: { DrillInspectionModal, FLHAModal, DrillingFormModal, Modal },
  props: {
    userId: { type: Number, required: true },
    projectId: { type: Number, required: true }
  },
  data() {
    return {
      viewMode: 'cards',
      formTypes: [
        { id: 1, name: 'Truck Inspection', icon: 'bi bi-truck', color: '#e3f2fd' },
        { id: 2, name: 'Drill Inspection', icon: 'bi bi-wrench-adjustable', color: '#fce4ec' },
        { id: 3, name: 'FLHA', icon: 'bi bi-shield-lock', color: '#e8f5e9' },
        { id: 4, name: 'ATV/UTV', icon: 'bi bi-check2-square', color: '#fff3e0' },
        { id: 5, name: 'Drilling', icon: 'bi bi-tools', color: '#f3e5f5' }
      ] as FormType[],
      forms: [] as FormDto[],
      selectedType: null as FormType | null,
      editingFormId: null as number | null,
      showDrillInspectionModal: false,
      showFLHAModal: false,
      showDrillingFormModal: false,
      commentExpanded: {} as Record<number, boolean>,
      showSendModal: false,
      selectedDate: '',
      sending: false,
      sendError: '',
      sendSuccess: false,
      buttonLocked: false
    };
  },
  computed: {
    filteredForms(): FormDto[] {
      return this.forms;
    },
    isInspectionType(): boolean {
      return this.selectedType?.id === 1 || this.selectedType?.id === 2 || this.selectedType?.id === 4;
    }
  },
  methods: {
    toggleComment(id: number) {
      this.commentExpanded[id] = !this.commentExpanded[id];
    },
    async selectFormType(type: FormType) {
      this.selectedType = type;
      this.viewMode = 'list';
      await this.loadFormsFromBackend();
    },
    backToCards() {
      this.viewMode = 'cards';
      this.selectedType = null;
      this.forms = [];
    },
    formatDate(dateStr: string) {
      const date = new Date(dateStr);
      return date.toLocaleDateString(undefined, {
        year: 'numeric',
        month: '2-digit',
        day: '2-digit',
        hour: '2-digit',
        minute: '2-digit',
        hour12: false
      });
    },
    addNewForm() {
      this.editingFormId = null;
      if (this.selectedType?.id === 3) {
        this.showFLHAModal = true;
      } else if (this.selectedType?.id === 5) {
        this.showDrillingFormModal = true;
      } else if (this.isInspectionType) {
        this.showDrillInspectionModal = true;
      }
    },
    editForm(formId: number) {
      this.editingFormId = formId;
      if (this.selectedType?.id === 3) {
        this.showFLHAModal = true;
      } else if (this.selectedType?.id === 5) {
        this.showDrillingFormModal = true;
      } else if (this.isInspectionType) {
        this.showDrillInspectionModal = true;
      }
    },
    onModalClosed() {
      this.showDrillInspectionModal = false;
      this.showFLHAModal = false;
      this.showDrillingFormModal = false;
      this.editingFormId = null;
      this.loadFormsFromBackend();
    },
    async loadFormsFromBackend() {
      if (!this.selectedType) return;
      try {
        const url =
          this.selectedType.id === 3
            ? `${import.meta.env.VITE_API_BASE_URL}/flha/project/${this.projectId}`
            : this.selectedType.id === 5
              ? `${import.meta.env.VITE_API_BASE_URL}/forms/project/${this.projectId}/drilling`
              : `${import.meta.env.VITE_API_BASE_URL}/forms/project/${this.projectId}/type/${this.selectedType.id}`;
        console.log(url);
        const res = await fetch(url, { credentials: 'include' });
        if (!res.ok) throw new Error('Failed to load forms');
        this.forms = await res.json();
      } catch (err) {
        console.error('Error loading forms:', err);
      }
    },
    openSendModal() {
      this.selectedDate = '';
      this.sendError = '';
      this.sendSuccess = false;
      this.showSendModal = true;
    },
    closeSendModal() {
      this.showSendModal = false;
    },
    async sendForms() {
      if (!this.selectedDate || !this.selectedType || !this.projectId) return;

      this.sending = true;
      this.sendError = '';
      this.sendSuccess = false;

      try {
        const res = await fetch(`${import.meta.env.VITE_API_BASE_URL}/FormDeliveryRules/send-manual`, {
          method: 'POST',
          headers: { 'Content-Type': 'application/json' },
          credentials: 'include',
          body: JSON.stringify({
            projectId: this.projectId,
            formTypeId: this.selectedType.id,
            dateFilled: this.selectedDate
          })
        });

        if (res.ok) {
          this.sendSuccess = true;
          this.buttonLocked = true;
          setTimeout(() => {
            this.buttonLocked = false;
          }, 3000);
        } else {
          const data = await res.json();
          this.sendError = data?.title || 'Failed to send forms';
        }
      } catch (err) {
        this.sendError = 'Error sending forms: ' + err;
      } finally {
        this.sending = false;
      }
    }
  }
});
</script>
<style scoped>
.forms-section {
  padding: 1rem;
  background-color: #fff;
  border-radius: 8px;
}
.section-title {
  text-align: center;
  font-size: 1.75rem;
  margin-bottom: 1rem;
}
.form-type-grid {
  display: flex;
  flex-wrap: wrap;
  gap: 1rem;
  justify-content: center;
  margin-bottom: 1rem;
}
.form-type-card {
  display: flex;
  flex-direction: column;
  align-items: center;
  justify-content: center;
  width: 140px;
  height: 140px;
  border: 1px solid #ddd;
  border-radius: 8px;
  cursor: pointer;
  transition: transform 0.3s ease, box-shadow 0.3s ease;
}
.form-type-card:hover {
  transform: scale(1.03);
}
.form-type-card i {
  font-size: 2rem;
  margin-bottom: 0.5rem;
  color: #1976d2;
}
.form-type-card span {
  font-size: 1rem;
  text-align: center;
}
.list-header {
  display: flex;
  justify-content: space-between;
  align-items: center;
  margin-bottom: 1rem;
  flex-wrap: wrap;
  gap: 0.5rem;
}
.btn.back-btn,
.btn.add-new-btn,
.btn.send-btn {
  background-color: #1976d2;
  color: #fff;
  padding: 0.5rem 1rem;
  border-radius: 4px;
  cursor: pointer;
  margin-left: 0.3rem;
}
.btn.send-btn {
  background-color: #43a047;
}
.btn.back-btn {
  background-color: #6c757d;
}
.btn.edit-btn {
  background-color: #115293;
  color: #fff;
}
.forms-table-container {
  overflow-x: auto;
  margin-top: 1rem;
}
.table {
  width: 100%;
  border-collapse: collapse;
}
.table th,
.table td {
  border: 1px solid #ddd;
  padding: 0.5rem;
  text-align: left;
}
.table th {
  background-color: #f7f7f7;
  font-weight: bold;
}
.fade-enter-active,
.fade-leave-active {
  transition: opacity 0.3s;
}
.fade-enter-from,
.fade-leave-to {
  opacity: 0;
}
.scrollable-forms {
  max-height: calc(100vh - 160px);
  overflow-y: auto;
  border: 1px solid #ccc;
  border-radius: 6px;
}
.forms-table thead th {
  position: sticky;
  top: 0;
  background-color: #f7f7f7;
  z-index: 2;
}
.send-modal-content {
  padding: 1rem;
}
.send-modal-content h4 {
  margin-bottom: 1rem;
}
.date-picker-row {
  display: flex;
  align-items: center;
  gap: 0.5rem;
  margin-bottom: 1rem;
}
.date-picker-row i {
  font-size: 1.25rem;
  color: #1976d2;
}
.modal-actions {
  display: flex;
  justify-content: flex-end;
  gap: 0.5rem;
  margin-top: 1rem;
}
.success-msg {
  color: green;
  margin-top: 0.5rem;
}
.error-msg {
  color: red;
  margin-top: 0.5rem;
}
</style>
