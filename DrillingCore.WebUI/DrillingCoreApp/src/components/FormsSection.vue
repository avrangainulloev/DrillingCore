<template>
  <div class="forms-section">
    <transition name="fade" mode="out-in">
      <!-- Cards Mode -->
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

      <!-- List Mode -->
      <div v-else key="list">
        <div class="list-header">
          <button class="btn back-btn" @click="backToCards">Back</button>
          <h3 class="section-title">{{ selectedType?.name || '' }} Forms</h3>
          <button class="btn add-new-btn" @click="addNewForm">Add New Form</button>
        </div>

        <div class="forms-table-container">
          <table class="table forms-table">
            <thead>
              <tr>
                <th>ID</th>
                <th>Form Type</th>
                <th>Crew</th>
                <th>Unit</th>
                <th>Date Filled</th>
                <th>Comments</th>
                <th>Actions</th>
              </tr>
            </thead>
            <tbody>
              <tr v-for="form in filteredForms" :key="form.id">
                <td>{{ form.id }}</td>
                <td>{{ form.formTypeName }}</td>
                <td>{{ form.crewName }}</td>
                <td>{{ form.unitNumber }}</td>
                <td>{{ formatDate(form.dateFilled) }}</td>
                <td>{{ form.otherComments }}</td>
                <td>
                  <button class="btn edit-btn" @click="editForm(form.id)">View/Edit</button>
                </td>
              </tr>
              <tr v-if="filteredForms.length === 0">
                <td colspan="7" class="no-data">No forms found.</td>
              </tr>
            </tbody>
          </table>
        </div>
      </div>
    </transition>

    <!-- DrillInspectionModal -->
    <DrillInspectionModal
      v-if="showDrillInspectionModal"
      :user-id="userId"
      :form-type-id="selectedType?.id || 2"
      :form-id="editingFormId ?? undefined"
      @close="onModalClosed"
    />
  </div>
</template>

<script lang="ts">
import { defineComponent } from 'vue';
import DrillInspectionModal from './DrillInspectionModal.vue';

interface FormDto {
  id: number;
  formTypeName: string;
  crewName: string;
  unitNumber: string;
  dateFilled: string;
  otherComments: string;
}

interface FormType {
  id: number;
  name: string;
  icon: string;
  color: string;
}

export default defineComponent({
  name: 'FormsSection',
  components: { DrillInspectionModal },
  props: {
    userId: { type: Number, required: true }
  },
  data() {
    return {
      viewMode: 'cards',
      formTypes: [
        { id: 1, name: 'Truck Inspection', icon: 'bi bi-truck', color: '#e3f2fd' },
        { id: 2, name: 'Drill Inspection', icon: 'bi bi-wrench-adjustable', color: '#fce4ec' },
        { id: 3, name: 'FLHA', icon: 'bi bi-shield-lock', color: '#e8f5e9' },
        { id: 4, name: 'Safety Checklist', icon: 'bi bi-check2-square', color: '#fff3e0' },
        { id: 5, name: 'Well Servicing', icon: 'bi bi-tools', color: '#f3e5f5' }
      ] as FormType[],
      forms: [] as FormDto[],
      selectedType: null as FormType | null,
      showDrillInspectionModal: false,
      editingFormId: null as number | null,
      projectId: 0
    };
  },
  computed: {
    filteredForms(): FormDto[] {
      return this.forms;
    }
  },
  methods: {
    async selectFormType(type: FormType) {
      this.selectedType = type;
      this.viewMode = 'list';
      await this.loadProjectId();
      await this.loadFormsFromBackend();
    },
    backToCards() {
      this.viewMode = 'cards';
      this.selectedType = null;
      this.forms = [];
    },
    formatDate(dateStr: string) {
      return new Date(dateStr).toLocaleDateString();
    },
    addNewForm() {
      this.editingFormId = null;
      this.showDrillInspectionModal = true;
    },
    editForm(formId: number) {
      this.editingFormId = formId;
      this.showDrillInspectionModal = true;
    },
    onModalClosed() {
      this.showDrillInspectionModal = false;
      this.editingFormId = null;
      this.loadFormsFromBackend(); // refresh
    },
    async loadProjectId() {
      try {
        const res = await fetch(`/api/users/${this.userId}/active-project`, { credentials: 'include' });
        if (!res.ok) throw new Error("Failed to load project");
        const project = await res.json();
        this.projectId = project.projectId ?? project.id;
      } catch (err) {
        console.error("Error loading projectId:", err);
      }
    },
    async loadFormsFromBackend() {
      if (!this.selectedType) return;
      try {
        const res = await fetch(`/api/forms/project/${this.projectId}/type/${this.selectedType.id}`, { credentials: 'include' });
        if (!res.ok) throw new Error("Failed to load forms");
        this.forms = await res.json();
      } catch (err) {
        console.error("Error loading forms:", err);
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
}
.btn.back-btn {
  background-color: #6c757d;
  color: #fff;
  padding: 0.5rem 1rem;
  border-radius: 4px;
  cursor: pointer;
}
.btn.add-new-btn {
  background-color: #1976d2;
  color: #fff;
  padding: 0.5rem 1.5rem;
  border-radius: 4px;
  cursor: pointer;
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
.btn.edit-btn {
  background-color: #115293;
  color: #fff;
}
.no-data {
  text-align: center;
  font-style: italic;
  color: #999;
  padding: 1rem;
}
.fade-enter-active,
.fade-leave-active {
  transition: opacity 0.3s;
}
.fade-enter-from,
.fade-leave-to {
  opacity: 0;
}
</style>
