<template>
  <div class="projects-section">
    <div class="projects-header">
      <h2>Projects</h2>
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
          </td>
        </tr>
      </tbody>
    </table>
  </div>
</template>

<script lang="ts">
import { defineComponent } from 'vue';

export default defineComponent({
  name: 'ProjectsSection',
  data() {
    return {
      projects: [] as any[],
      searchTerm: '',
      selectedStatus: ''
    }
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
      console.log("ProjectsSection: Emitting open-project-modal with projectId:", projectId);
      this.$emit('open-project-modal', projectId);
    },
    openParticipants(projectId: number) {
      console.log("ProjectsSection: Emitting open-participant-modal for projectId:", projectId);
      this.$emit('open-participant-modal', projectId);
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

/* Фильтры */
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

/* Таблица */
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

/* Кнопки */
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
</style>
