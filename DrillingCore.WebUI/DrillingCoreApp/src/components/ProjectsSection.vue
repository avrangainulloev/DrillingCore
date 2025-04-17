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
              <button class="btn edit-btn" @click="openProjectModal(project.id)">Edit</button>
              <button class="btn participant-btn" @click="openParticipants(project.id)">Participants</button>
              <button class="btn forms-btn" @click="openFormsModal(project.id)">Forms</button>
              <button class="btn delivery-btn" @click="openDeliveryModal(project.id)">Delivery Settings</button>
            </td>
          </tr>
        </tbody>
      </table>
    </div>

    <!-- Modal for Forms -->
    <Modal v-if="showFormsModal" @close="closeFormsModal">
      <FormsSection
        v-if="selectedProjectId !== null"
        :user-id="userId"
        :project-id="selectedProjectId"
      />
    </Modal>

    <!-- Modal for Delivery Settings + Form -->
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

      // Delivery settings and rule creation/editing
      showDeliveryModal: false,
      deliveryProjectId: null as number | null,
      showFormDeliveryModal: false,
      editingRule: null as any | null
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

        const url = `https://localhost:7200/api/Projects?${params.toString()}`;
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
      if (!dateStr) return '';
      return new Date(dateStr).toLocaleDateString();
    }
  }
});
</script>


<style scoped>
.projects-section {
  padding: 1rem;
}
.projects-header {
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
}
.search-input,
.status-select {
  padding: 0.5rem;
  border: 1px solid #ccc;
  border-radius: 4px;
}
.search-btn {
  background-color: #1976d2;
  color: #ffffff;
  padding: 0.5rem 1rem;
  border: none;
  border-radius: 4px;
  cursor: pointer;
}
.search-btn:hover {
  background-color: #1565c0;
}
.projects-table {
  width: 100%;
  border-collapse: collapse;
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
.create-btn {
  background-color: #1976d2;
  color: white;
  padding: 0.5rem 1rem;
  border-radius: 4px;
}
.create-btn:hover {
  background-color: #1565c0;
}
.edit-btn {
  background-color: #115293;
  color: white;
  padding: 0.3rem 0.75rem;
  border-radius: 4px;
}
.edit-btn:hover {
  background-color: #0d3c73;
}
.participant-btn {
  background-color: #4caf50;
  color: white;
  padding: 0.3rem 0.75rem;
  margin-left: 0.3rem;
  border-radius: 4px;
}
.participant-btn:hover {
  background-color: #43a047;
}
.forms-btn {
  background-color: #ff9800;
  color: white;
  padding: 0.3rem 0.75rem;
  margin-left: 0.3rem;
  border-radius: 4px;
}
.forms-btn:hover {
  background-color: #fb8c00;
}
.delivery-btn {
  background-color: #f08d8d;
  color: white;
  padding: 0.3rem 0.75rem;
  margin-left: 0.3rem;
  border-radius: 4px;
}
.delivery-btn:hover {
  background-color: #7b1fa2;
}
.scrollable-projects {
  max-height: calc(100vh - 220px);
  overflow-y: auto;
  border: 1px solid #ccc;
  border-radius: 6px;
}
</style>
