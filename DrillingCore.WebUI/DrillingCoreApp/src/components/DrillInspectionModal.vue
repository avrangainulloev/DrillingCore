<template>
  <div>
    <div class="modal-overlay" @click.self="closeModal">
      <div class="modal-content drill-inspection-modal">
        <div class="modal-header">
          <h3 class="modal-title">{{ formTypeId === 1 ? 'Truck Inspection' : 'Drill Inspection' }}</h3>
          <button type="button" class="btn-close" @click="closeModal">&times;</button>
        </div>

        <div class="modal-body">
          <div class="form-group">
            <label>Crew Name</label>
            <input type="text" v-model="crewName" class="form-control" disabled />
          </div>

          <div class="form-group">
            <label>Date Filled</label>
            <input type="date" v-model="dateFilled" class="form-control" />
          </div>

          <div class="form-group">
            <label>Unit Number</label>
            <input type="text" v-model="unitNumber" class="form-control" />
          </div>

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

          <div class="checklist-section">
            <div v-for="(items, groupName) in groupedChecklist" :key="groupName" class="checklist-group">
              <h5>{{ groupName }}</h5>
              <div class="checklist-grid">
                <label v-for="item in items" :key="item.id" class="checklist-item">
                  <input type="checkbox" :value="item.id" v-model="checkedItems" />
                  {{ item.label }}
                </label>
              </div>
            </div>
          </div>

          <div class="form-group">
            <label>Other Comments</label>
            <textarea v-model="otherComments" rows="3" class="form-control" placeholder="Additional notes..."></textarea>
          </div>

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

          <div class="form-group">
            <label>Signatures</label>
            <div v-for="participantId in selectedParticipantIds" :key="participantId" class="signature-block">
              <strong>{{ getParticipantName(participantId) }}</strong>
              <button class="btn sign-btn" @click="openSignatureModal(participantId)">Sign</button>
              <img v-if="signatures[participantId]" :src="signatures[participantId]" class="signature-preview" />
            </div>
          </div>

          <div v-if="photoToView" class="photo-modal" @click="photoToView = ''">
            <img :src="photoToView" />
          </div>

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

    <div v-if="showSuccessToast" class="toast-success">
      {{ successMessage }}
    </div>
  </div>
</template>

<script lang="ts">
import { defineComponent } from 'vue';
import SignatureModal from './SignatureModal.vue';

export default defineComponent({
  name: 'DrillInspectionModal',
  emits: ['close'],
  components: { SignatureModal },
  props: {
    userId: { type: Number, required: true },
    formTypeId: { type: Number, required: true },
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
      checkedItems: [] as number[],
      otherComments: '',
      photos: [] as { name: string; preview: string; file: File | null }[],
      signatures: {} as Record<number, string>,
      currentSignatureParticipantId: null as number | null,
      showSignatureModal: false,
      photoToView: '',
      showSuccessToast: false,
      successMessage: '',
      checklistItems: [] as any[],
    };
  },
  computed: {
    filteredParticipantOptions() {
      const term = this.participantSearch.toLowerCase();
      return this.allParticipants.filter(p => p.fullName.toLowerCase().includes(term));
    },
    groupedChecklist() {
      const grouped: Record<string, any[]> = {};
      for (const item of this.checklistItems) {
        if (!grouped[item.groupName]) {
          grouped[item.groupName] = [];
        }
        grouped[item.groupName].push(item);
      }
      return grouped;
    }
  },
  watch: {
    formId: {
      immediate: true,
      async handler(newVal: number | null) {
        this.resetForm();
        if (newVal != null) {
          await this.loadFormData(newVal);
          await this.loadChecklist();
        } else {
          await this.loadActiveProject();
          await this.loadParticipants();
          await this.loadChecklist();
          await this.loadEquipment();
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
    showToast(message: string) {
      this.successMessage = message;
      this.showSuccessToast = true;
      setTimeout(() => this.showSuccessToast = false, 3000);
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
  const flatList = groups.flatMap((g: any) => g.participants);

  const nowRes = await fetch('/api/Common/server-date');
  const now = new Date((await nowRes.json()).now);

  const grouped = new Map<number, any[]>();
  for (const p of flatList) {
    if (!grouped.has(p.userId)) grouped.set(p.userId, []);
    grouped.get(p.userId)!.push(p);
  }

  this.allParticipants = [];

  for (const [userId, records] of grouped.entries()) {
    const valid = records.filter(r => !r.endDate || new Date(r.endDate) > now);
    if (valid.length > 0) {
      valid.sort((a, b) => {
        const aDate = a.endDate ? new Date(a.endDate) : new Date('9999-12-31');
        const bDate = b.endDate ? new Date(b.endDate) : new Date('9999-12-31');
        return aDate.getTime() - bDate.getTime();
      });
      this.allParticipants.push(valid[0]);
    }
  }

  const group = groups.find((g: any) => g.participants.some((p: any) => p.userId === this.userId));
  if (group && !this.formId) {
    this.crewName = group.groupName;
    this.selectedParticipantIds = group.participants.map((p: any) => p.id);
  }
},
    async loadChecklist() {
      const res = await fetch(`/api/checklist/by-form-type/${this.formTypeId}`);
       
      this.checklistItems = await res.json();
    },
    async loadEquipment() {
  if (!this.allParticipants.length) return;

  const currentParticipant = this.allParticipants.find(p => p.userId === this.userId)
                            ?? this.allParticipants[0];
  if (!currentParticipant) return;

  const participantId = currentParticipant.id;

  try {
    const res = await fetch(`/api/forms/equipment?formTypeId=${this.formTypeId}&participantId=${participantId}&projectId=${this.projectId}`);
    
    if (res.ok) {
      const contentType = res.headers.get("Content-Type");
      if (contentType?.includes("application/json")) {
        const equipment = await res.json();
        this.unitNumber = equipment?.registrationNumber ?? '';
      } else {
//        const text = await res.text(); // например: "No equipment found."
       
        this.unitNumber = '';
      }
    } else {
      console.warn("Failed to fetch equipment:", res.statusText);
      this.unitNumber = '';
    }
  } catch (err) {
    console.error("Error loading equipment:", err);
  }
},
    async loadFormData(formId: number) {
      const res = await fetch(`/api/forms/drill-inspection/${formId}`);
      const form = await res.json();

      const API_BASE = 'http://localhost:5152/';
      this.projectId = form.projectId;
      this.crewName = form.crewName;
      this.unitNumber = form.unitNumber;
      this.dateFilled = form.dateFilled.split("T")[0];
      this.otherComments = form.otherComments;

      this.checkedItems = form.checklistResponses
        .filter((c: any) => c.response)
        .map((c: any) => c.checklistItemId);

      this.selectedParticipantIds = form.participants.map((p: any) => p.participantId);

      this.signatures = {};
      for (const sig of form.signatures) {
        this.signatures[sig.participantId] = API_BASE + sig.signatureUrl;
      }

      this.photos = form.photoUrls.map((url: string) => ({
        name: '',
        preview: API_BASE + url,
        file: null
      }));

      await this.loadParticipants();
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
        crewName: this.crewName,
        unitNumber: this.unitNumber,
        dateFilled: this.dateFilled,
        otherComments: this.otherComments,
        participants: this.selectedParticipantIds.map(id => ({
          participantId: id,
          signature: this.signatures[id] || null
        })),
        checklistResponses: this.checkedItems.map(x => ({ checklistItemId: x, response: true }))
      };

      let formId = this.formId;

      if (this.formId) {
        await fetch('/api/forms/drill-inspection', {
          method: 'PUT',
          headers: { 'Content-Type': 'application/json' },
          body: JSON.stringify({ formId: this.formId, ...payload })
        });
      } else {
        
        const res = await fetch('/api/forms/drill-inspection', {
          method: 'POST',
          headers: { 'Content-Type': 'application/json' },
          body: JSON.stringify({ projectId: this.projectId, formTypeId: this.formTypeId, creatorId: this.userId, ...payload })
        });
        const result = await res.json();
        formId = result.formId;
      }

      for (const photo of this.photos) {
        if (!photo.file) continue;
        const formData = new FormData();
        formData.append('file', photo.file);
        await fetch(`/api/forms/${formId}/photos`, {
          method: 'POST',
          body: formData
        });
      }

      for (const [participantId, signature] of Object.entries(this.signatures)) {
        if (signature.startsWith('http')) continue;
        const blob = await fetch(signature).then(res => res.blob());
        const formData = new FormData();
        formData.append('participantId', participantId);
        formData.append('file', new File([blob], "signature.png", { type: 'image/png' }));
        await fetch(`/api/forms/${formId}/signatures`, {
          method: 'POST',
          body: formData
        });
      }

      this.showToast(this.formId ? '✅ Drill Inspection updated successfully!' : '✅ Drill Inspection created successfully!');
      setTimeout(() => this.closeModal(), 1500);
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

.toast-success {
  position: fixed;
  bottom: 20px;
  right: 20px;
  background-color: #28a745;
  color: white;
  padding: 0.75rem 1.25rem;
  border-radius: 6px;
  box-shadow: 0 0 10px rgba(0,0,0,0.2);
  font-weight: bold;
  z-index: 10000;
  animation: fadeInOut 3s ease-in-out;
}

@keyframes fadeInOut {
  0% { opacity: 0; transform: translateY(20px); }
  10%, 90% { opacity: 1; transform: translateY(0); }
  100% { opacity: 0; transform: translateY(20px); }
}
</style>
