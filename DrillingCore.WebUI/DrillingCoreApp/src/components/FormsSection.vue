<template>
  <div class="forms-section">
    <transition name="fade" mode="out-in">
      <!-- Режим карточек для выбора типа формы -->
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
      <!-- Режим списка заполненных форм -->
      <div v-else key="list">
        <div class="list-header">
          <button class="btn back-btn" @click="backToCards">Back</button>
          <h3 class="section-title">{{ selectedType?.name || '' }} Forms</h3>
          <button class="btn add-new-btn" @click="addNewForm">Add New Form</button>
        </div>
        <!-- Поле поиска по формам -->
        <div class="search-container">
          <input
            type="text"
            class="form-control search-input"
            v-model="searchTerm"
            placeholder="Search forms..."
          />
        </div>
        <!-- Таблица заполненных форм -->
        <div class="forms-table-container">
          <table class="table forms-table">
            <thead>
              <tr>
                <th>ID</th>
                <th>Title</th>
                <th>Date Filled</th>
                <th>Status</th>
                <th>Actions</th>
              </tr>
            </thead>
            <tbody>
              <tr v-for="form in filteredForms" :key="form.id">
                <td>{{ form.id }}</td>
                <td>{{ form.title }}</td>
                <td>{{ formatDate(form.dateFilled) }}</td>
                <td>{{ form.status }}</td>
                <td>
                  <button class="btn edit-btn" @click="editForm(form.id)">View/Edit</button>
                  <button class="btn copy-btn" @click="copyForm(form.id)">Copy</button>
                </td>
              </tr>
              <tr v-if="filteredForms.length === 0">
                <td colspan="5" class="no-data">No forms found.</td>
              </tr>
            </tbody>
          </table>
        </div>
      </div>
    </transition>

    <!-- Модальное окно для Drill Inspection (появляется, если выбран тип с id === 2) -->
    <DrillInspectionModal
      v-if="showDrillInspectionModal"
      @close="closeDrillInspectionModal"
    />
  </div>
</template>

<script lang="ts">
import { defineComponent } from 'vue';
import DrillInspectionModal from './DrillInspectionModal.vue';

export default defineComponent({
  name: 'FormsSection',
  components: { DrillInspectionModal },
  data() {
    return {
      // Режим отображения: 'cards' или 'list'
      viewMode: 'cards',
      // Тестовые типы форм с иконками и цветами
      formTypes: [
        { id: 1, name: 'Truck Inspection', icon: 'bi bi-truck', color: '#e3f2fd' },
        { id: 2, name: 'Drill Inspection', icon: 'bi bi-wrench-adjustable', color: '#fce4ec' },
        { id: 3, name: 'FLHA', icon: 'bi bi-shield-lock', color: '#e8f5e9' },
        { id: 4, name: 'Safety Checklist', icon: 'bi bi-check2-square', color: '#fff3e0' },
        { id: 5, name: 'Well Servicing', icon: 'bi bi-tools', color: '#f3e5f5' }
      ],
      // Тестовые данные заполненных форм
      forms: [
        { id: 101, title: 'Truck Inspection Form #101', formTypeId: 1, dateFilled: '2023-03-01', status: 'Completed' },
        { id: 102, title: 'Drill Inspection Form #102', formTypeId: 2, dateFilled: '2023-03-05', status: 'In Progress' },
        { id: 103, title: 'FLHA Form #103', formTypeId: 3, dateFilled: '2023-03-07', status: 'Completed' },
        { id: 104, title: 'Safety Checklist Form #104', formTypeId: 4, dateFilled: '2023-03-10', status: 'Completed' },
        { id: 105, title: 'Truck Inspection Form #105', formTypeId: 1, dateFilled: '2023-03-12', status: 'Draft' },
        { id: 106, title: 'Well Servicing Form #106', formTypeId: 5, dateFilled: '2023-03-15', status: 'Completed' },
        { id: 107, title: 'Drill Inspection Form #107', formTypeId: 2, dateFilled: '2023-03-18', status: 'In Progress' }
      ],
      // Выбранный тип формы – null, если не выбран
      selectedType: null as { id: number; name: string; icon: string; color: string } | null,
      // Поле поиска по формам
      searchTerm: '',
      // Флаг для открытия модального окна для Drill Inspection
      showDrillInspectionModal: false
    };
  },
  computed: {
    filteredForms() {
      let result = this.forms;
      if (this.selectedType !== null) {
        result = result.filter(f => f.formTypeId === this.selectedType!.id);
      }
      if (this.searchTerm.trim() !== '') {
        const term = this.searchTerm.toLowerCase();
        result = result.filter(f => f.title.toLowerCase().includes(term));
      }
      return result;
    }
  },
  methods: {
    selectFormType(type: any) {
      this.selectedType = type;
      this.viewMode = 'list';
    },
    backToCards() {
      this.viewMode = 'cards';
      this.selectedType = null;
      this.searchTerm = '';
    },
    formatDate(dateStr: string) {
      if (!dateStr) return '';
      return new Date(dateStr).toLocaleDateString();
    },
    editForm(formId: number) {
      alert(`View/Edit form ${formId}`);
    },
    copyForm(formId: number) {
      alert(`Copy form ${formId}`);
    },
    addNewForm() {
      if (!this.selectedType) {
        alert("Please select a form type.");
        return;
      }
      // Если выбран тип Drill Inspection (id === 2), открываем модальное окно DrillInspectionModal
      if (this.selectedType.id === 2) {
        this.showDrillInspectionModal = true;
      } else {
        alert(`Add new form for type ${this.selectedType.name}`);
      }
    },
    closeDrillInspectionModal() {
      this.showDrillInspectionModal = false;
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
.form-type-card.selected {
  border-color: #1976d2;
  box-shadow: 0 2px 8px rgba(25, 118, 210, 0.5);
  transform: scale(1.05);
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
  transition: background-color 0.3s ease;
}
.btn.back-btn:hover {
  background-color: #5a6268;
}
.btn.add-new-btn {
  background-color: #1976d2;
  color: #fff;
  padding: 0.5rem 1.5rem;
  border-radius: 4px;
  font-size: 1rem;
  cursor: pointer;
  transition: background-color 0.3s ease;
}
.btn.add-new-btn:hover {
  background-color: #1565c0;
}
.search-container {
  margin-bottom: 1rem;
  text-align: center;
}
.search-input {
  width: 80%;
  max-width: 400px;
  padding: 0.5rem;
  border: 1px solid #ccc;
  border-radius: 4px;
  font-size: 1rem;
}
.forms-table-container {
  overflow-x: auto;
  margin-top: 1rem;
}
.table {
  width: 100%;
  border-collapse: collapse;
  margin-top: 1rem;
}
.table th,
.table td {
  border: 1px solid #ddd;
  padding: 0.5rem;
  font-size: 0.9rem;
  text-align: left;
}
.table th {
  background-color: #f7f7f7;
  font-weight: bold;
}
.btn.edit-btn {
  background-color: #115293;
  color: #fff;
  margin-right: 0.5rem;
}
.btn.copy-btn {
  background-color: #4caf50;
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
@media (max-width: 768px) {
  .form-type-grid {
    flex-direction: column;
    align-items: center;
  }
}
</style>
