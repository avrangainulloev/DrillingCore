<template>
    <div class="modal-overlay" @click.self="closeModal">
      <div class="modal-dialog">
        <div class="modal-content attach-equipment-modal">
          <!-- Заголовок, который всегда виден -->
          <div class="modal-header">
            <h3 class="modal-title">Attach Equipment</h3>
            <button type="button" class="btn-close" @click="closeModal">&times;</button>
          </div>
          <!-- Модальное тело с прокруткой, но без заголовка и футера -->
          <div class="modal-body modal-scroll">
            <!-- Секция: уже привязанная техника -->
            <div class="attached-section">
              <h6>Already Attached Equipment</h6>
              <table class="table attached-table">
                <thead>
                  <tr>
                    <th>Name</th>
                    <th>Type</th>
                    <th>Reg. No.</th>
                    <th>Start Date</th>
                    <th>End Date</th>
                    <th>Action</th>
                  </tr>
                </thead>
                <tbody>
                  <tr 
                    v-for="item in sortedAttachedEquipment" 
                    :key="item.id"
                    :class="{'detached-row': item.endDate}"
                  >
                    <td>{{ item.equipmentName }}</td>
                    <td>{{ item.equipmentTypeName || 'Unknown' }}</td>
                    <td>{{ item.registrationNumber }}</td>
                    <td>{{ formatDate(item.startDate) }}</td>
                    <td>
                      <span v-if="item.endDate">Detached ({{ formatDate(item.endDate) }})</span>
                      <span v-else>Active</span>
                    </td>
                    <td>
                      <button
                        v-if="!item.endDate"
                        class="btn detach-btn"
                        @click="openDetachDateModal(item.id)"
                      >
                        Detach
                      </button>
                      <span v-else class="detached-label">N/A</span>
                    </td>
                  </tr>
                  <tr v-if="sortedAttachedEquipment.length === 0">
                    <td :colspan="6" class="no-data">No equipment attached.</td>
                  </tr>
                </tbody>
              </table>
            </div>
    
            <hr />
    
            <!-- Секция: свободная техника -->
            <div class="free-section">
              <div class="free-header">
                <h6>Free Equipment</h6>
                <div class="free-filters">
                  <input
                    type="text"
                    class="form-control free-search-input"
                    v-model="freeSearchTerm"
                    placeholder="Search..."
                    @input="onFreeFilterChanged"
                  />
                  <select class="form-select free-type-select" v-model="freeSelectedTypeId" @change="onFreeFilterChanged">
                    <option value="">All Types</option>
                    <option v-for="type in equipmentTypes" :key="type.id" :value="type.id">
                      {{ type.typeName }}
                    </option>
                  </select>
                </div>
              </div>
              <div class="table-container">
                <table class="table free-table">
                  <thead>
                    <tr>
                      <th>Name</th>
                      <th>Type</th>
                      <th>Reg. No.</th>
                      <th>Created Date</th>
                      <th>Action</th>
                    </tr>
                  </thead>
                  <tbody>
                    <tr v-for="eq in paginatedFreeEquipment" :key="eq.id">
                      <td>{{ eq.name }}</td>
                      <td>{{ getEquipmentTypeName(eq) }}</td>
                      <td>{{ eq.registrationNumber }}</td>
                      <td>{{ formatDate(eq.createdDate) }}</td>
                      <td>
                        <button class="btn attach-btn" @click="openAttachDateModal(eq.id)">Attach</button>
                      </td>
                    </tr>
                    <tr v-if="paginatedFreeEquipment.length === 0">
                      <td colspan="5" class="no-data">No free equipment found.</td>
                    </tr>
                  </tbody>
                </table>
              </div>
              <div class="pagination" v-if="freeTotalPages > 1">
                <button class="btn page-btn" @click="prevFreePage" :disabled="freeCurrentPage === 1">Previous</button>
                <span>Page {{ freeCurrentPage }} of {{ freeTotalPages }}</span>
                <button class="btn page-btn" @click="nextFreePage" :disabled="freeCurrentPage === freeTotalPages">Next</button>
              </div>
            </div>
          </div>
          <!-- Фиксированный футер модального окна (вне области скролла) -->
          <div class="modal-footer">
            <button class="btn close-btn" @click="closeModal">Close</button>
          </div>
        </div>
      </div>
    
      <!-- Подмодальное окно для ввода даты Attach -->
      <AttachDateModal
        v-if="showAttachDateModal"
        :mode="'attach'"
        :projectId="projectId"
        :participantId="participantId"
        :equipmentId="selectedEquipmentId"
        @close="closeAttachDateModal"
        @submitted="onEquipmentAttached"
      />
    
      <!-- Подмодальное окно для ввода даты Detach -->
      <AttachDateModal
        v-if="showDetachDateModal"
        :mode="'detach'"
        :projectId="projectId"
        :participantId="participantId"
        :assignmentId="selectedAssignmentId"
        @close="closeDetachDateModal"
        @submitted="onEquipmentDetached"
      />
    </div>
  </template>
  
  <script lang="ts">
  import { defineComponent } from 'vue';
  import AttachDateModal from './AttachDateModal.vue';
  
  export default defineComponent({
    name: 'AttachEquipmentModal',
    components: { AttachDateModal },
    props: {
      participantId: {
        type: Number,
        required: true
      },
      projectId: {
        type: Number,
        required: true
      }
    },
    data() {
      return {
        attachedEquipment: [] as any[],
        freeEquipmentList: [] as any[],
        equipmentTypes: [] as any[],
        freeSearchTerm: '',
        freeSelectedTypeId: '',
        freeCurrentPage: 1,
        freePageSize: 10,
        selectedEquipmentId: null as number | null,
        selectedAssignmentId: null as number | null,
        showAttachDateModal: false,
        showDetachDateModal: false
      };
    },
    computed: {
      sortedAttachedEquipment(): any[] {
        return this.attachedEquipment.slice().sort((a, b) => b.id - a.id);
      },
      hasDetachAction(): boolean {
        return this.attachedEquipment.some(item => !item.endDate);
      },
      freeTotalPages(): number {
        return Math.ceil(this.filteredFreeEquipment.length / this.freePageSize);
      },
      paginatedFreeEquipment(): any[] {
        const start = (this.freeCurrentPage - 1) * this.freePageSize;
        return this.filteredFreeEquipment.slice(start, start + this.freePageSize);
      },
      filteredFreeEquipment(): any[] {
        return this.freeEquipmentList.filter(eq => {
          const searchMatch = eq.name.toLowerCase().includes(this.freeSearchTerm.toLowerCase());
          const typeMatch = this.freeSelectedTypeId ? eq.typeId == this.freeSelectedTypeId : true;
          return searchMatch && typeMatch;
        });
      }
    },
    mounted() {
      this.loadEquipmentTypes();
      this.loadAttachedEquipment();
      this.loadFreeEquipment();
    },
    methods: {
      async loadEquipmentTypes() {
        try {
          const response = await fetch('https://localhost:7200/api/EquipmentType', { credentials: 'include' });
          if (response.ok) {
            this.equipmentTypes = await response.json();
          } else {
            console.error('Error loading equipment types:', response.statusText);
          }
        } catch (error) {
          console.error('Error loading equipment types:', error);
        }
      },
      async loadAttachedEquipment() {
        try {
          const url = `https://localhost:7200/api/ParticipantEquipment/${this.participantId}`;
          const response = await fetch(url, { credentials: 'include' });
          if (response.ok) {
            this.attachedEquipment = await response.json();
          } else {
            console.error('Error loading attached equipment:', response.statusText);
          }
        } catch (error) {
          console.error('Error loading attached equipment:', error);
        }
      },
      async loadFreeEquipment() {
        try {
          const params = new URLSearchParams();
          if (this.freeSelectedTypeId) {
            params.append('equipmentTypeId', this.freeSelectedTypeId);
          }
          const url = `https://localhost:7200/api/Equipment/free?${params.toString()}`;
          const response = await fetch(url, { credentials: 'include' });
          if (response.ok) {
            this.freeEquipmentList = await response.json();
            this.freeCurrentPage = 1;
          } else {
            console.error('Error loading free equipment:', response.statusText);
          }
        } catch (error) {
          console.error('Error loading free equipment:', error);
        }
      },
      onFreeFilterChanged() {
        this.freeCurrentPage = 1;
      },
      formatDate(dateStr: string) {
        if (!dateStr) return '';
        return new Date(dateStr).toLocaleDateString();
      },
      getEquipmentTypeName(equipment: any): string {
        return equipment.typeName || 'Unknown';
      },
      prevFreePage() {
        if (this.freeCurrentPage > 1) this.freeCurrentPage--;
      },
      nextFreePage() {
        if (this.freeCurrentPage < this.freeTotalPages) this.freeCurrentPage++;
      },
      openAttachDateModal(equipmentId: number) {
        this.selectedEquipmentId = equipmentId;
        this.showAttachDateModal = true;
      },
      closeAttachDateModal() {
        this.showAttachDateModal = false;
      },
      async onEquipmentAttached(payload: any) {
        try {
          const response = await fetch(`https://localhost:7200/api/ParticipantEquipment/attach`, {
            method: 'POST',
            headers: { 'Content-Type': 'application/json' },
            body: JSON.stringify(payload)
          });
          if (response.ok) {
            alert("Equipment attached successfully!");
            this.loadAttachedEquipment();
            this.loadFreeEquipment();
            this.closeAttachDateModal();
          } else {
            const errorText = await response.text();
            alert("Error attaching equipment: " + errorText);
          }
        } catch (error) {
          console.error("Error attaching equipment:", error);
          alert("Error attaching equipment. See console for details.");
        }
      },
      openDetachDateModal(assignmentId: number) {
        this.selectedAssignmentId = assignmentId;
        this.showDetachDateModal = true;
      },
      closeDetachDateModal() {
        this.showDetachDateModal = false;
      },
      async onEquipmentDetached(payload: any) {
        try {
          const response = await fetch(`https://localhost:7200/api/ParticipantEquipment/detach/${payload.id}`, {
            method: 'PUT',
            headers: { 'Content-Type': 'application/json' },
            body: JSON.stringify(payload)
          });
          if (response.ok) {
            alert("Equipment detached successfully!");
            this.loadAttachedEquipment();
            this.loadFreeEquipment();
            this.closeDetachDateModal();
          } else {
            const errorText = await response.text();
            alert("Error detaching equipment: " + errorText);
          }
        } catch (error) {
          console.error("Error detaching equipment:", error);
          alert("Error detaching equipment. See console for details.");
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
    background-color: rgba(0, 0, 0, 0.6);
    display: flex;
    justify-content: center;
    align-items: center;
    z-index: 1000;
    padding: 1rem;
  }
  .modal-dialog {
    max-width: 1200px !important;
  }
  .attach-equipment-modal {
    background-color: #fff;
    padding: 1.5rem;
    border-radius: 8px;
    width: 120% !important;
    box-shadow: 0 4px 12px rgba(0,0,0,0.2);
    max-height: 90vh;
    overflow-y: auto;
    position: relative;
  }
  .attach-equipment-modal h3 {
    margin-top: 0;
    text-align: center;
    font-size: 1.5rem;
    font-weight: bold;
    margin-bottom: 1rem;
  }
  .modal-header {
    position: relative;
    /* фиксированный блок, который не скроллится */
    flex-shrink: 0;
    padding: 1rem;
    border-bottom: 1px solid #ddd;
  }
  .modal-header .modal-title {
    margin: 0;
    font-size: 1.5rem;
  }
  .btn-close {
    position: absolute;
    top: 1rem;
    right: 1rem;
    background: transparent;
    border: none;
    font-size: 1.5rem;
    cursor: pointer;
  }
  .modal-body {
    padding: 1rem;
    /* Фиксированная высота, чтобы футер был всегда виден */
    max-height: calc(90vh - 180px);
    overflow-y: auto;
  }
  .table-container {
    max-height: 300px;
    overflow-y: auto;
    margin-bottom: 1rem;
  }
  .table {
    width: 100%;
    border-collapse: collapse;
  }
  .table th,
  .table td {
    border: 1px solid #ddd;
    padding: 0.2rem 0.4rem;
    text-align: left;
    font-size: 0.8rem;
  }
  .table th {
    background-color: #f7f7f7;
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
  .attach-btn {
    background-color: #1976d2;
    color: #fff;
    padding: 0.3rem 0.75rem;
    border-radius: 4px;
    font-size: 0.8rem;
    cursor: pointer;
  }
  .attach-btn:hover {
    background-color: #1565c0;
  }
  .detach-btn {
    background-color: #dc3545;
    color: #fff;
    padding: 0.3rem 0.75rem;
    border-radius: 4px;
    font-size: 0.8rem;
    cursor: pointer;
  }
  .detach-btn:hover {
    background-color: #c82333;
  }
  .pagination {
    margin-top: 0.5rem;
    display: flex;
    justify-content: center;
    align-items: center;
    gap: 0.5rem;
  }
  .page-btn {
    background-color: #1976d2;
    color: #fff;
    border: none;
    padding: 0.2rem 0.4rem;
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
  .free-filters {
    display: flex;
    gap: 0.5rem;
    align-items: center;
    margin-bottom: 0.5rem;
  }
  .free-search-input {
    width: 100px;
    padding: 0.3rem;
    border: 1px solid #ccc;
    border-radius: 4px;
    font-size: 0.9rem;
  }
  .free-type-select {
    width: 100px;
    padding: 0.3rem;
    border: 1px solid #ccc;
    border-radius: 4px;
    font-size: 0.9rem;
  }
  .modal-footer {
    display: flex;
    justify-content: flex-end;
    padding-top: 1rem;
    margin-top: 1rem;
    border-top: 1px solid #ddd;
    flex-shrink: 0;
  }
  .close-btn {
    background-color: #ccc;
    color: #333;
    padding: 0.5rem 1rem;
    border-radius: 4px;
    cursor: pointer;
    transition: background-color 0.3s ease;
  }
  .close-btn:hover {
    background-color: #bbb;
  }
  .detached-row {
    background-color: #f1f1f1;
    color: #999;
  }
  </style>
  