<template>
  <div class="modal-overlay" @click.self="closeModal">
    <div class="modal-content">
      <h3>{{ projectId ? 'Edit Project' : 'Create Project' }}</h3>
      <form @submit.prevent="submitForm">
        <div class="form-group">
          <label for="projectName">Name</label>
          <input v-model="project.name" type="text" id="projectName" required />
        </div>
        <div class="form-group">
          <label for="location">Location</label>
          <input v-model="project.location" type="text" id="location" required />
        </div>
        <div class="form-group">
          <label for="client">Client</label>
          <input v-model="project.client" type="text" id="client" required />
        </div>
        <div class="form-group">
          <label for="startDate">Start Date</label>
          <input v-model="project.startDate" type="date" id="startDate" required />
        </div>
        <div class="form-group">
          <label for="endDate">End Date</label>
          <input v-model="project.endDate" type="date" id="endDate" />
        </div>
        <div class="form-group">
          <label for="status">Status</label>
          <select v-model="project.statusId" id="status" required>
            <option value="1">Active</option>
            <option value="2">Inactive</option>
            <option value="3">Suspended</option>
            <option value="4">Completed</option>
          </select>
        </div>
        <div class="modal-actions">
          <button type="submit" class="btn save-btn">Save</button>
          <button type="button" class="btn cancel-btn" @click="closeModal">Cancel</button>
        </div>
      </form>
    </div>
  </div>
</template>

<script lang="ts">
import { defineComponent } from 'vue';

export default defineComponent({
  name: 'ProjectModal',
  props: {
    projectId: {
      type: Number,
      default: null
    }
  },
  data() {
    return {
      project: {
        id: null as number | null,
        name: '',
        location: '',
        client: '',
        startDate: '',
        endDate: '',
        statusId: null,
        hasCampOrHotel: false
      }
    }
  },
  mounted() {
    if (this.projectId) {
      this.loadProjectData(this.projectId);
    }
  },
  methods: {
    loadProjectData(projectId: number) {
      fetch(`${import.meta.env.VITE_API_BASE_URL}/Projects/${projectId}`, { credentials: 'include' })
        .then(response => response.json())
        .then(data => {
          console.log("Loaded project data:", data);
          this.project = {
            id: data.id,
            name: data.name,
            location: data.location,
            client: data.client,
            startDate: data.startDate ? data.startDate.split('T')[0] : '',
            endDate: data.endDate ? data.endDate.split('T')[0] : '',
            // Если data.statusId присутствует – используем его, иначе преобразуем строковое значение status
            statusId: data.statusId,
            hasCampOrHotel: data.hasCampOrHotel ?? false
          };
        })
        .catch(err => console.error("Error loading project data:", err));
    },
    getStatusIdFromName(statusName: string): number {
      switch (statusName) {
        case 'Active': return 1;
        case 'Inactive': return 2;
        case 'Suspended': return 3;
        case 'Completed': return 4;
        default: return 1;
      }
    },
    submitForm() {
      const payload = {
        id: this.project.id ?? 0,
        name: this.project.name,
        location: this.project.location,
        client: this.project.client,
        startDate: this.project.startDate,
        endDate: this.project.endDate || null,
        statusId: this.project.statusId,
        hasCampOrHotel: this.project.hasCampOrHotel
      };

      const method = this.project.id ? 'PUT' : 'POST';
      const url = this.project.id 
        ? `${import.meta.env.VITE_API_BASE_URL}/Projects/${this.project.id}` 
        : `${import.meta.env.VITE_API_BASE_URL}/Projects`;
      
      console.log("Submitting project payload:", payload, "Method:", method, "URL:", url);
      
      fetch(url, {
        method,
        headers: { 'Content-Type': 'application/json' },
        credentials: 'include',
        body: JSON.stringify(payload)
      })
      .then(response => {
        if (response.ok) {
          console.log("Project saved successfully");
          this.$emit('project-saved');
          this.closeModal();
        } else {
          response.text().then(errorText => {
            console.error('Error saving project:', errorText);
          });
        }
      })
      .catch(err => console.error("Error saving project:", err));
    },
    closeModal() {
      console.log("Closing modal");
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
  background-color: rgba(0,0,0,0.5);
  display: flex;
  justify-content: center;
  align-items: center;
  z-index: 1000;
}

.modal-content {
  background-color: #fff;
  padding: 1.5rem;
  border-radius: 8px;
  width: 90%;
  max-width: 500px;
  box-shadow: 0 4px 12px rgba(0,0,0,0.2);
  position: relative;
}

.modal-content h3 {
  margin-top: 0;
}

.form-group {
  margin-bottom: 1rem;
}

.form-group label {
  display: block;
  margin-bottom: 0.25rem;
}

.form-group input,
.form-group select {
  width: 100%;
  padding: 0.5rem;
  border: 1px solid #ccc;
  border-radius: 4px;
}

.modal-actions {
  display: flex;
  justify-content: flex-end;
  gap: 0.5rem;
}

.save-btn {
  background-color: #1976d2;
  color: #fff;
  border: none;
  padding: 0.5rem 1rem;
  border-radius: 4px;
  cursor: pointer;
  transition: background-color 0.3s ease;
}

.save-btn:hover {
  background-color: #1565c0;
}

.cancel-btn {
  background-color: #ccc;
  color: #333;
  border: none;
  padding: 0.5rem 1rem;
  border-radius: 4px;
  cursor: pointer;
  transition: background-color 0.3s ease;
}

.cancel-btn:hover {
  background-color: #bbb;
}
</style>
