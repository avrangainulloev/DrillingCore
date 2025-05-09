<template>
  <div class="projects-section">
    <div class="projects-header">
      <div class="filters">
        <input
          type="text"
          class="search-input"
          v-model="searchTerm"
          placeholder="Search projects..."
        />
        <select class="status-select" v-model="selectedStatus">
          <option value="">All Statuses</option>
          <option value="Active">Active</option>
          <option value="Inactive">Inactive</option>
          <option value="Suspended">Suspended</option>
          <option value="Completed">Completed</option>
        </select>
        <button class="btn search-btn" @click="loadProjects">Search</button>
      </div>
      <button class="btn create-btn" @click="openProjectModal()">Create Project</button>
    </div>

    <div class="scrollable-projects">
      <table class="projects-table">
        <thead>
          <tr>
            <th>ID</th>
            <th>Name</th>
            <th>Client</th>
            <th>Start Date</th>
            <th>End Date</th>
            <th>Status</th>
            <th>Actions</th>
          </tr>
        </thead>
        <tbody>
          <tr v-for="project in projects" :key="project.id">
            <td>{{ project.id }}</td>
            <td>{{ project.name }}</td>
            <td>{{ project.client }}</td>
            <td>{{ formatDate(project.startDate) }}</td>
            <td>{{ formatDate(project.endDate) }}</td>
            <td>{{ project.status }}</td>
            <td>
              <div class="desktop-buttons">
                <button class="btn edit-btn" @click="openProjectModal(project.id)">Edit</button>
                <button class="btn participant-btn" @click="openParticipants(project.id)">Participants</button>
                <button class="btn forms-btn" @click="openFormsModal(project.id)">Forms</button>
                <button class="btn delivery-btn" @click="openDeliveryModal(project.id)">Delivery</button>
              </div>
            </td>
          </tr>
        </tbody>
      </table>
    </div>

    <Modal v-if="showFormsModal" @close="closeFormsModal">
      <FormsSection
        v-if="selectedProjectId !== null"
        :user-id="userId"
        :project-id="selectedProjectId"
      />
    </Modal>

    <Modal v-if="showDeliveryModal" @close="closeDeliveryModal">
      <FormDeliverySettings
        v-if="!showFormDeliveryModal"
        :project-id="deliveryProjectId!"
        @add-new="onAddNewRule"
        @edit-rule="onEditRule"
      />
      <FormDeliveryModal
        v-if="showFormDeliveryModal"
        :project-id="deliveryProjectId!"
        :rule="editingRule"
        @close="closeFormDeliveryModal"
      />
    </Modal>
  </div>
</template>

<script lang="ts">
import { defineComponent } from 'vue';
import Modal from './Modal.vue';
import FormsSection from './FormsSection.vue';
import FormDeliveryModal from './FormDeliveryModal.vue';
import FormDeliverySettings from './FormDeliverySettings.vue';

const API_BASE_URL = import.meta.env.VITE_API_BASE_URL;

export default defineComponent({
  name: 'ProjectsSection',
  components: {
    Modal,
    FormsSection,
    FormDeliveryModal,
    FormDeliverySettings,
  },
  data() {
    return {
      projects: [] as any[],
      searchTerm: '',
      selectedStatus: '',
      showFormsModal: false,
      selectedProjectId: null as number | null,
      userId: 1,
      showDeliveryModal: false,
      deliveryProjectId: null as number | null,
      showFormDeliveryModal: false,
      editingRule: null as any | null,
      activeDropdownId: null as number | null,
    };
  },
  mounted() {
    this.loadProjects();
  },
  methods: {
    async loadProjects() {
      try {
        const params = new URLSearchParams();
        params.append('limit', '30');
        if (this.searchTerm) params.append('searchTerm', this.searchTerm);
        if (this.selectedStatus) params.append('status', this.selectedStatus);

        const url = `${API_BASE_URL}/Projects?${params.toString()}`;
        const response = await fetch(url, { credentials: 'include' });

        if (response.ok) {
          this.projects = (await response.json()).sort((a: any, b: any) => b.id - a.id);
        } else {
          console.error('Error loading projects:', response.statusText);
        }
      } catch (err) {
        console.error('Error loading projects:', err);
      }
    },
    openProjectModal(projectId?: number) {
      this.$emit('open-project-modal', projectId);
    },
    openParticipants(projectId: number) {
      this.$emit('open-participant-modal', projectId);
    },
    openFormsModal(projectId: number) {
      this.selectedProjectId = projectId;
      this.showFormsModal = true;
    },
    closeFormsModal() {
      this.showFormsModal = false;
    },
    openDeliveryModal(projectId: number) {
      this.deliveryProjectId = projectId;
      this.showDeliveryModal = true;
      this.showFormDeliveryModal = false;
      this.editingRule = null;
    },
    onAddNewRule() {
      this.showFormDeliveryModal = true;
      this.editingRule = null;
    },
    onEditRule(rule: any) {
      this.showFormDeliveryModal = true;
      this.editingRule = rule;
    },
    closeFormDeliveryModal() {
      this.showFormDeliveryModal = false;
      this.editingRule = null;
    },
    closeDeliveryModal() {
      this.showDeliveryModal = false;
      this.deliveryProjectId = null;
      this.editingRule = null;
      this.showFormDeliveryModal = false;
    },
    formatDate(dateStr: string) {
      return dateStr ? new Date(dateStr).toLocaleDateString() : '';
    },
  },
});
</script>

<style scoped>
.projects-section {
  padding: 1rem;
  background-color: var(--content-bg);
  color: var(--menu-text-light);
}

.dark-theme .projects-section {
  background-color: var(--content-bg);
  color: var(--menu-text-dark);
}

.projects-header {
  display: flex;
  flex-wrap: wrap;
  justify-content: space-between;
  align-items: center;
  margin-bottom: 1rem;
  gap: 0.5rem;
}

.filters {
  display: flex;
  flex-wrap: wrap;
  gap: 0.5rem;
  align-items: center;
  flex-grow: 1;
}

.search-input,
.status-select {
  padding: 0.5rem;
  border: 1px solid #ccc;
  border-radius: 4px;
  min-width: 140px;
  background-color: #fff;
  color: #333;
}

.dark-theme .search-input,
.dark-theme .status-select {
  background-color: #2c2c2c;
  color: #e0e0e0;
  border-color: #444;
}

.search-btn,
.create-btn {
  background-color: #1976d2;
  color: #ffffff;
  padding: 0.5rem 1rem;
  border: none;
  border-radius: 4px;
  cursor: pointer;
}

.search-btn:hover,
.create-btn:hover {
  background-color: #1565c0;
}

.scrollable-projects {
  max-height: calc(100vh - 220px);
  overflow-y: auto;
  border: 1px solid #ccc;
  border-radius: 6px;
  overflow-x: auto;
}

.projects-table {
  width: 100%;
  border-collapse: collapse;
  min-width: 768px;
}

.projects-table th,
.projects-table td {
  border: 1px solid #333;
  padding: 0.75rem;
  text-align: left;
}

.projects-table thead th {
  position: sticky;
  top: 0;
  background-color: #f7f7f7;
  z-index: 2;
}

.dark-theme .projects-table th,
.dark-theme .projects-table td {
  background-color: #1f1f1f;
  color: #e0e0e0;
  border-color: #444;
}

.edit-btn,
.participant-btn,
.forms-btn,
.delivery-btn {
  padding: 0.3rem 0.75rem;
  border-radius: 4px;
  font-size: 0.85rem;
  color: white;
  cursor: pointer;
  margin-bottom: 0.25rem;
  white-space: nowrap;
}

.edit-btn {
  background-color: #115293;
}
.edit-btn:hover {
  background-color: #0d3c73;
}

.participant-btn {
  background-color: #4caf50;
}
.participant-btn:hover {
  background-color: #43a047;
}

.forms-btn {
  background-color: #ff9800;
}
.forms-btn:hover {
  background-color: #fb8c00;
}

.delivery-btn {
  background-color: #f08d8d;
}
.delivery-btn:hover {
  background-color: #7b1fa2;
}

.desktop-buttons {
  display: flex;
  flex-direction: row; /* по горизонтали */
  gap: 0.5rem; /* отступы между кнопками */
  flex-wrap: wrap; /* если места мало, перенос на новую строку */
}
/* Responsive styles */
@media (max-width: 768px) {
  .filters {
    flex-direction: column;
    align-items: stretch;
  }

  .projects-header {
    flex-direction: column;
    align-items: stretch;
  }

  .create-btn {
    align-self: flex-end;
    width: 100%;
  }

  .scrollable-projects {
    overflow-x: auto;
    -webkit-overflow-scrolling: touch;
  }

  .projects-table {
    font-size: 0.85rem;
    min-width: 700px;
  }

  .projects-table td button {
    display: block;
    margin-bottom: 0.25rem;
  }



  .mobile-actions {
    display: none;
  }
}
</style>

