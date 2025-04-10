<template>
  <div class="modal-overlay" @click.self="closeModal">
    <div class="modal-content drill-inspection-modal">
      <div class="modal-header">
        <h3 class="modal-title">Drill Inspection</h3>
        <button type="button" class="btn-close" @click="closeModal">&times;</button>
      </div>
      <div class="modal-body">
        <!-- Crew Name -->
        <div class="form-group">
          <label>Crew Name</label>
          <input type="text" v-model="crewName" class="form-control" disabled />
        </div>

        <!-- Date -->
        <div class="form-group">
          <label>Date Filled</label>
          <input type="date" v-model="dateFilled" class="form-control" />
        </div>

        <!-- Unit Number -->
        <div class="form-group">
          <label>Unit Number</label>
          <input type="text" v-model="unitNumber" class="form-control" />
        </div>

        <!-- Participants -->
        <div class="form-group">
          <label>Participants</label>
          <input type="text" class="form-control" v-model="participantSearch" placeholder="Search participants..." />
          <div class="participants-list">
            <div v-for="p in filteredParticipantOptions" :key="p.id" class="participant-item">
              <label>
                <input type="checkbox" :value="p.id" v-model="selectedParticipantIds" />
                {{ p.fullName }}
              </label>
            </div>
          </div>
        </div>

        <!-- Checklist Section -->
        <div class="checklist-section">
          <div class="checklist-group">
            <h5>🛠️ Equipment & Storage</h5>
            <div class="checklist-grid">
              <label v-for="item in equipmentStorageItems" :key="item.id" class="checklist-item">
                <input type="checkbox" :value="item.id" v-model="checkedItems" />
                {{ item.label }}
              </label>
            </div>
          </div>

          <div class="checklist-group">
            <h5>🦺 Safety & Accessories</h5>
            <div class="checklist-grid">
              <label v-for="item in safetyAccessoryItems" :key="item.id" class="checklist-item">
                <input type="checkbox" :value="item.id" v-model="checkedItems" />
                {{ item.label }}
              </label>
            </div>
          </div>
        </div>

        <!-- Comments -->
        <div class="form-group">
          <label>Other Comments</label>
          <textarea v-model="otherComments" rows="3" class="form-control" placeholder="Additional notes..."></textarea>
        </div>

        <!-- Photo Upload -->
        <div class="form-group">
          <label>Attach Photos</label>
          <input type="file" multiple @change="onPhotosSelected" accept="image/*" class="form-control" />
          <div class="photo-preview-list">
            <div v-for="(photo, idx) in photos" :key="idx" class="preview-wrapper">
              <img :src="photo.preview" class="preview-img" @click="viewPhoto(photo.preview)" />
              <button class="remove-btn" @click="removePhoto(idx)">&times;</button>
            </div>
          </div>
        </div>

        <!-- Signatures -->
        <div class="form-group">
          <label>Signatures</label>
          <div v-for="participantId in selectedParticipantIds" :key="participantId" class="signature-block">
            <strong>{{ getParticipantName(participantId) }}</strong>
            <button class="btn sign-btn" @click="openSignatureModal(participantId)">Sign</button>
            <img v-if="signatures[participantId]" :src="signatures[participantId]" class="signature-preview" />
          </div>
        </div>

        <!-- Photo Viewer -->
        <div v-if="photoToView" class="photo-modal" @click="photoToView = ''">
          <img :src="photoToView" />
        </div>

        <!-- Signature Modal -->
        <SignatureModal
          v-if="showSignatureModal && currentSignatureParticipantId !== null"
          :participantId="currentSignatureParticipantId"
          @close="showSignatureModal = false"
          @signature-saved="onSignatureSaved"
        />
      </div>

      <div class="modal-footer">
        <button class="btn btn-success" @click="saveInspection">Save</button>
        <button class="btn btn-secondary" @click="closeModal">Cancel</button>
      </div>
    </div>
  </div>
</template>

<script lang="ts">
import { defineComponent, watch } from 'vue';
import SignatureModal from './SignatureModal.vue';

export default defineComponent({
  name: 'DrillInspectionModal',
  components: { SignatureModal },
  props: {
    userId: { type: Number, required: true },
    formTypeId: { type: Number, default: 2 },
    formId: { type: Number, required: false }
  },
  data() {
    return {
      projectId: 0,
      crewName: '',
      unitNumber: '',
      dateFilled: new Date().toISOString().split('T')[0],
      participantSearch: '',
      allParticipants: [] as any[],
      selectedParticipantIds: [] as number[],
      equipmentStorageItems: [] as any[],
      safetyAccessoryItems: [] as any[],
      checkedItems: [] as number[],
      otherComments: '',
      photos: [] as { name: string; preview: string; file: File }[],
      signatures: {} as Record<number, string>,
      currentSignatureParticipantId: null as number | null,
      showSignatureModal: false,
      photoToView: ''
    };
  },
  computed: {
    filteredParticipantOptions() {
      const term = this.participantSearch.toLowerCase();
      return this.allParticipants.filter(p => p.fullName.toLowerCase().includes(term));
    }
  },
  watch: {
    formId: {
      immediate: true,
      async handler(newVal: number | null) {
        this.resetForm();
        await this.loadActiveProject();
        await this.loadParticipants();
        await this.loadChecklist();
        await this.loadEquipment();
        // TODO: for editing existing form (optional)
        if (newVal !== null) {
          console.log("Edit mode - load form details");
        }
      }
    }
  },
  methods: {
    resetForm() {
      this.crewName = '';
      this.unitNumber = '';
      this.dateFilled = new Date().toISOString().split('T')[0];
      this.participantSearch = '';
      this.selectedParticipantIds = [];
      this.photos = [];
      this.signatures = {};
      this.otherComments = '';
      this.checkedItems = [];
    },
    closeModal() {
      this.$emit('close');
    },
    getParticipantName(id: number) {
      const p = this.allParticipants.find(x => x.id === id);
      return p?.fullName || 'Unknown';
    },
    async loadActiveProject() {
      const res = await fetch(`/api/users/${this.userId}/active-project`, { credentials: 'include' });
      const data = await res.json();
      this.projectId = data.projectId ?? data.id;
    },
    async loadParticipants() {
      const res = await fetch(`/api/projects/${this.projectId}/groups`);
      const groups = await res.json();
      this.allParticipants = groups.flatMap((g: any) => g.participants);
      const group = groups.find((g: any) => g.participants.some((p: any) => p.userId === this.userId));
      if (group) {
        this.crewName = group.groupName;
        this.selectedParticipantIds = group.participants.map((p: any) => p.id);
      }
    },
    async loadChecklist() {
      const res = await fetch(`/api/checklist/by-form-type/${this.formTypeId}`);
      const items = await res.json();
      this.equipmentStorageItems = items.filter((i: any) => i.groupName === 'Equipment & Storage');
      this.safetyAccessoryItems = items.filter((i: any) => i.groupName === 'Safety & Accessories');
    },
    async loadEquipment() {
      if (!this.selectedParticipantIds.length) return;
      const res = await fetch(`/api/forms/equipment?formTypeId=${this.formTypeId}&participantId=${this.selectedParticipantIds[0]}&projectId=${this.projectId}`);
      const equipment = await res.json();
      this.unitNumber = equipment?.registrationNumber ?? '';
    },
    onPhotosSelected(event: any) {
      const files = event.target.files;
      for (let i = 0; i < files.length; i++) {
        const file = files[i];
        const reader = new FileReader();
        reader.onload = (e: any) => {
          this.photos.push({ name: file.name, preview: e.target.result, file });
        };
        reader.readAsDataURL(file);
      }
    },
    removePhoto(index: number) {
      this.photos.splice(index, 1);
    },
    viewPhoto(url: string) {
      this.photoToView = url;
    },
    openSignatureModal(participantId: number) {
      this.currentSignatureParticipantId = participantId;
      this.showSignatureModal = true;
    },
    onSignatureSaved(payload: { participantId: number; signatureData: string }) {
      this.signatures[payload.participantId] = payload.signatureData;
      this.showSignatureModal = false;
    },
    async saveInspection() {
      const payload = {
        projectId: this.projectId,
        formTypeId: this.formTypeId,
        creatorId: this.userId,
        crewName: this.crewName,
        unitNumber: this.unitNumber,
        dateFilled: this.dateFilled,
        participants: this.selectedParticipantIds.map(id => ({
    participantId: id,
    signature: this.signatures[id] || null
  })),
        checklistResponses: this.checkedItems.map(x => ({ checklistItemId: x, response: true })),
        otherComments: this.otherComments
      };

      const res = await fetch('/api/forms/drill-inspection', {
        method: 'POST',
        headers: { 'Content-Type': 'application/json' },
        body: JSON.stringify(payload)
      });

      const result = await res.json();
      const formId = result.formId;
      for (const photo of this.photos) {
        const formData = new FormData();
        formData.append('file', photo.file);
        await fetch(`/api/forms/${formId}/photos`, {
          method: 'POST',
          body: formData
        });
      }

      for (const [participantId, signature] of Object.entries(this.signatures)) {
        const blob = await fetch(signature).then(res => res.blob());
        const formData = new FormData();
        formData.append('participantId', participantId);
        formData.append('file', new File([blob], "signature.png", { type: 'image/png' }));
        await fetch(`/api/forms/${formId}/signatures`, {
          method: 'POST',
          body: formData
        });
      }

      alert('Drill Inspection saved!');
      this.closeModal();
    }
  }
});
</script>

<style scoped>
.modal-overlay {
  position: fixed;
  inset: 0;
  background: rgba(0, 0, 0, 0.6);
  display: flex;
  align-items: center;
  justify-content: center;
  z-index: 9999;
}
.modal-content {
  background: white;
  width: 90%;
  max-width: 800px;
  padding: 1.2rem;
  border-radius: 8px;
  max-height: 90vh;
  overflow-y: auto;
}
.modal-header {
  display: flex;
  justify-content: space-between;
  align-items: center;
}
.form-group {
  margin-bottom: 1rem;
}
.form-control {
  width: 100%;
  padding: 0.5rem;
  border: 1px solid #ccc;
  border-radius: 4px;
}
.participants-list {
  border: 1px solid #ccc;
  padding: 0.5rem;
  max-height: 120px;
  overflow-y: auto;
  border-radius: 4px;
}
.checklist-section {
  margin-top: 1rem;
}
.checklist-group {
  background: #f4f4f4;
  border: 1px solid #ccc;
  padding: 0.75rem;
  border-radius: 6px;
  margin-bottom: 1rem;
}
.checklist-grid {
  display: grid;
  grid-template-columns: repeat(auto-fill, minmax(250px, 1fr));
  gap: 0.5rem;
}
.photo-preview-list {
  display: flex;
  gap: 0.5rem;
  margin-top: 0.5rem;
  flex-wrap: wrap;
}
.preview-img {
  width: 80px;
  height: 80px;
  object-fit: cover;
  border-radius: 4px;
  border: 1px solid #ccc;
}
.signature-block {
  display: flex;
  align-items: center;
  justify-content: space-between;
  margin-top: 0.5rem;
  border: 1px solid #ccc;
  padding: 0.5rem;
  border-radius: 6px;
}
.signature-preview {
  width: 100px;
  height: 60px;
  object-fit: contain;
  border: 1px solid #ccc;
  border-radius: 4px;
}
.btn {
  background-color: #1976d2;
  color: white;
  padding: 0.4rem 1rem;
  border: none;
  border-radius: 4px;
}
.btn-success {
  background-color: #28a745;
}
.btn-secondary {
  background-color: #6c757d;
}
.sign-btn {
  background-color: #fd7e14;
}
.preview-wrapper {
  position: relative;
  display: inline-block;
}
.remove-btn {
  position: absolute;
  top: 0;
  right: 0;
  background: rgba(0, 0, 0, 0.7);
  color: white;
  border: none;
  border-radius: 0 0 0 4px;
  cursor: pointer;
}
.photo-modal {
  position: fixed;
  inset: 0;
  background: rgba(0, 0, 0, 0.9);
  display: flex;
  align-items: center;
  justify-content: center;
  z-index: 99999;
}
.photo-modal img {
  max-width: 90vw;
  max-height: 90vh;
  border-radius: 8px;
}
</style>
