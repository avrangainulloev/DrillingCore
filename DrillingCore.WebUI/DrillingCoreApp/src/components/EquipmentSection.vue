<template>
  <div class="equipment-section">
    <div class="header-controls">
      <div class="filters">
        <input
          type="text"
          class="form-control search-input"
          v-model="searchTerm"
          placeholder="Search equipment..."
          @input="onFilterChanged"
        />
        <select class="form-select type-select" v-model="selectedTypeId" @change="onFilterChanged">
          <option value="">All Types</option>
          <option v-for="type in equipmentTypes" :key="type.id" :value="type.id">
            {{ type.typeName }}
          </option>
        </select>
        <button class="btn search-btn" @click="loadEquipment">Search</button>
      </div>
      <div class="add-equipment-container">
        <button class="btn add-btn" @click="openAddEquipmentModal">Add Equipment</button>
      </div>
    </div>

    <div class="table-container">
      <table class="table equipment-table">
        <thead>
          <tr>
            <th>Name</th>
            <th>Type</th>
            <th>Reg. No.</th>
            <th>Created Date</th>
            <th>Actions</th>
          </tr>
        </thead>
        <tbody>
          <tr v-for="equipment in paginatedEquipment" :key="equipment.id">
            <td>{{ equipment.name }}</td>
            <td>{{ getEquipmentTypeName(equipment) }}</td>
            <td>{{ equipment.registrationNumber }}</td>
            <td>{{ formatDate(equipment.createdDate) }}</td>
            <td>
              <button class="btn edit-btn" @click="openEditEquipmentModal(equipment.id)">Edit</button>
            </td>
          </tr>
          <tr v-if="paginatedEquipment.length === 0">
            <td colspan="5" class="no-data">No equipment found.</td>
          </tr>
        </tbody>
      </table>
    </div>

    <div class="pagination">
      <button class="btn page-btn" @click="prevPage" :disabled="currentPage === 1">Previous</button>
      <span>Page {{ currentPage }} of {{ totalPages }}</span>
      <button class="btn page-btn" @click="nextPage" :disabled="currentPage === totalPages">Next</button>
    </div>

    <!-- Equipment Modal -->
    <EquipmentModal
      v-if="showEquipmentModal"
      :equipmentId="editingEquipmentId ?? undefined" 
      @close="closeEquipmentModal"
      @equipment-saved="handleEquipmentSaved"
    />
  </div>
</template>

<script lang="ts">
import { defineComponent } from 'vue';
import EquipmentModal from './EquipmentModal.vue';

export default defineComponent({
  name: 'EquipmentSection',
  components: { EquipmentModal },
  data() {
    return {
      equipmentList: [] as any[],
      equipmentTypes: [] as any[],
      searchTerm: '',
      selectedTypeId: '',
      currentPage: 1,
      pageSize: 10,
      showEquipmentModal: false,
      editingEquipmentId: null as number | null
    };
  },
  computed: {
    totalPages(): number {
      return Math.ceil(this.equipmentList.length / this.pageSize);
    },
    paginatedEquipment(): any[] {
      const start = (this.currentPage - 1) * this.pageSize;
      return this.equipmentList.slice(start, start + this.pageSize);
    },
    equipmentTypeMap(): Record<number, string> {
      const map: Record<number, string> = {};
      this.equipmentTypes.forEach(type => {
        map[type.id] = type.typeName;
      });
      return map;
    }
  },
  mounted() {
    this.loadEquipmentTypes();
    this.loadEquipment();
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
    async loadEquipment() {
      try {
        const params = new URLSearchParams();
        if (this.searchTerm) {
          params.append('searchTerm', this.searchTerm);
        }
        if (this.selectedTypeId) {
          params.append('equipmentTypeId', this.selectedTypeId);
        }
        params.append('limit', '1000');
        const url = `${import.meta.env.VITE_API_BASE_URL}/Equipment?${params.toString()}`;
        const response = await fetch(url, { credentials: 'include' });
        if (response.ok) {
          this.equipmentList = await response.json();
          this.currentPage = 1;
        } else {
          console.error('Error loading equipment:', response.statusText);
        }
      } catch (error) {
        console.error('Error loading equipment:', error);
      }
    },
    onFilterChanged() {
      this.loadEquipment();
    },
    formatDate(dateStr: string) {
      if (!dateStr) return '';
      return new Date(dateStr).toLocaleDateString();
    },
    getEquipmentTypeName(equipment: any): string {
      if (equipment.typeName) return equipment.typeName;
      return this.equipmentTypeMap[equipment.typeId] || 'Unknown';
    },
    prevPage() {
      if (this.currentPage > 1) this.currentPage--;
    },
    nextPage() {
      if (this.currentPage < this.totalPages) this.currentPage++;
    },
    openAddEquipmentModal() {
      this.editingEquipmentId = null;
      this.showEquipmentModal = true;
    },
    openEditEquipmentModal(equipmentId: number) {
      this.editingEquipmentId = equipmentId;
      this.showEquipmentModal = true;
    },
    closeEquipmentModal() {
      this.showEquipmentModal = false;
    },
    handleEquipmentSaved() {
      this.loadEquipment();
      this.closeEquipmentModal();
    }
  }
});
</script>

<style scoped>
.equipment-section {
  padding: 1rem;
}

/* Разделение фильтров и кнопки Add Equipment */
.header-controls {
  display: flex;
  flex-wrap: wrap;
  justify-content: space-between;
  align-items: center;
  margin-bottom: 1rem;
}
.filters {
  display: flex;
  gap: 0.5rem;
  align-items: center;
  flex: 1;
  min-width: 300px;
}
.search-input,
.type-select {
  width: 120px;
  padding: 0.4rem;
  border: 1px solid #ccc;
  border-radius: 4px;
  font-size: 0.9rem;
}
.search-btn {
  background-color: #1976d2;
  color: #fff;
  border: none;
  padding: 0.4rem 0.8rem;
  border-radius: 4px;
  font-size: 0.9rem;
  cursor: pointer;
  transition: background-color 0.3s ease;
}
.search-btn:hover {
  background-color: #1565c0;
}
.add-equipment-container {
  margin-left: 1rem;
}
.add-btn {
  background-color: #1976d2;
  color: #fff;
  border: none;
  padding: 0.5rem 1rem;
  border-radius: 4px;
  font-size: 0.9rem;
  cursor: pointer;
  transition: background-color 0.3s ease;
}
.add-btn:hover {
  background-color: #1565c0;
}

/* Таблица с уменьшёнными строками */
.table-container {
  max-height: 400px;
  overflow-y: auto;
  margin-bottom: 1rem;
}
.equipment-table {
  width: 100%;
  border-collapse: collapse;
}
.equipment-table th,
.equipment-table td {
  border: 1px solid #333;
  padding: 0.2rem 0.4rem; /* уменьшены отступы */
  text-align: left;
  font-size: 0.8rem;
}
.equipment-table th {
  background-color: #f7f7f7;
}

/* Pagination – оставляем оригинальный стиль */
.pagination {
  margin-top: 1rem;
  display: flex;
  justify-content: center;
  align-items: center;
  gap: 1rem;
}
.page-btn {
  background-color: #1976d2;
  color: #fff;
  border: none;
  padding: 0.3rem 0.6rem;
  border-radius: 4px;
  font-size: 0.8rem;
  cursor: pointer;
}
.page-btn:disabled {
  opacity: 0.5;
  cursor: not-allowed;
}
.no-data {
  text-align: center;
  font-style: italic;
  padding: 0.5rem;
}

/* Стили для кнопки Edit – возвращаем оригинальный дизайн, но можно немного уменьшить размеры */
.edit-btn {
  background-color: #115293;
  color: #fff;
  border: none;
  padding: 0.3rem 0.75rem;
  border-radius: 4px;
  cursor: pointer;
  font-size: 0.8rem;
  transition: background-color 0.3s ease;
}
.edit-btn:hover {
  background-color: #0d3c73;
}
</style>
