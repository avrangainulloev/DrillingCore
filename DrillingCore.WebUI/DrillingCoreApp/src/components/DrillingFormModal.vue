<template>
    <div class="modal-overlay" @click.self="closeModal">
      <div class="modal-content drill-inspection-modal">
        <div class="modal-header">
          <h3 class="modal-title">Drilling Form</h3>
          <button type="button" class="btn-close" @click="closeModal">&times;</button>
        </div>
  
        <div class="modal-body">
          <div class="form-group">
            <label>Date Filled</label>
            <input type="date" v-model="dateFilled" class="form-control" />
          </div>
  
          <div class="form-group">
            <label>Total Wells</label>
            <input type="number" v-model="totalWells" class="form-control" />
          </div>
  
          <div class="form-group">
            <label>Total Meters</label>
            <input type="number" v-model="totalMeters" class="form-control" />
          </div>
  
          <div class="form-group">
            <label>Participants</label>
            <input type="text" class="form-control" v-model="participantSearch" placeholder="Search participants..." />
            <div class="participants-list">
              <div v-for="p in filteredParticipants" :key="p.id" class="participant-item">
                <label>
                  <input type="checkbox" :value="p.id" v-model="selectedParticipantIds" />
                  {{ p.fullName }}
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
          <button class="btn btn-success" @click="saveForm">Save</button>
          <button class="btn btn-secondary" @click="closeModal">Cancel</button>
        </div>
      </div>
    </div>
  </template>
<script lang="ts">
import { defineComponent } from 'vue';
import SignatureModal from './SignatureModal.vue';

export default defineComponent({
  name: 'DrillingFormModal',
  emits: ['close'],
  components: { SignatureModal },
  props: {
    userId: { type: Number, required: true },
    projectId: { type: Number, required: true },
    formId: { type: Number, required: false }
  },
  data() {
    return {
      dateFilled: new Date().toISOString().split('T')[0],
      totalWells: 0,
      totalMeters: 0,
      otherComments: '',
      participantSearch: '',
      allParticipants: [] as any[],
      selectedParticipantIds: [] as number[],
      signatures: {} as Record<number, string>,
      currentSignatureParticipantId: null as number | null,
      showSignatureModal: false,
      photos: [] as { name: string; preview: string; file: File | null }[],
      photoToView: '',
      apiBase: import.meta.env.VITE_API_BASE_URL
    };
  },
  computed: {
    filteredParticipants() {
      const term = this.participantSearch.toLowerCase();
      return this.allParticipants.filter(p => p.fullName.toLowerCase().includes(term));
    }
  },
  async mounted() {
    await this.loadParticipants();
    if (this.formId) {
      await this.loadFormData(this.formId);
    }
  },
  methods: {
    closeModal() {
      this.$emit('close');
    },
    async loadParticipants() {
      const res = await fetch(`${import.meta.env.VITE_API_BASE_URL}/projects/${this.projectId}/groups`);
      const groups = await res.json();
      const flat = groups.flatMap((g: any) => g.participants);
      this.allParticipants = flat;
    },
    getParticipantName(id: number) {
      const p = this.allParticipants.find(x => x.id === id);
      return p?.fullName || 'Unknown';
    },
    openSignatureModal(participantId: number) {
      this.currentSignatureParticipantId = participantId;
      this.showSignatureModal = true;
    },
    onSignatureSaved(payload: { participantId: number; signatureData: string }) {
      this.signatures[payload.participantId] = payload.signatureData;
      this.showSignatureModal = false;
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
    async loadFormData(formId: number) {
      const res = await fetch(`${import.meta.env.VITE_API_BASE_URL}/forms/drilling/${formId}`);
      const form = await res.json();

      this.dateFilled = form.dateFilled;
      this.totalWells = form.totalWells;
      this.totalMeters = form.totalMeters;
      this.otherComments = form.otherComments;

      this.selectedParticipantIds = form.participants.map((p: any) => p.participantId);
      this.signatures = {};
      for (const sig of form.signatures) {
        this.signatures[sig.participantId] = `${import.meta.env.VITE_FILE_BASE_URL}` + sig.signatureUrl;
      }

      this.photos = form.photoUrls.map((url: string) => ({
        name: '',
        preview: `${import.meta.env.VITE_FILE_BASE_URL}` + url,
        file: null
      }));
    },
    async saveForm() {
      const payload = {
        projectId: this.projectId,
        creatorId: this.userId,
        dateFilled: this.dateFilled,
        totalWells: this.totalWells,
        totalMeters: this.totalMeters,
        otherComments: this.otherComments,
        participants: this.selectedParticipantIds.map(id => ({
          participantId: id,
          signature: this.signatures[id] || null
        }))
      };

      let formId = this.formId;

      if (this.formId) {
        await fetch(`${import.meta.env.VITE_API_BASE_URL}/forms/drilling`, {
          method: 'PUT',
          headers: { 'Content-Type': 'application/json' },
          body: JSON.stringify({ formId: this.formId, ...payload })
        });

        
      } else {
        const res = await fetch(`${import.meta.env.VITE_API_BASE_URL}/forms/drilling`, {
          method: 'POST',
          headers: { 'Content-Type': 'application/json' },
          body: JSON.stringify(payload)
        });
        const result = await res.json();
        formId = result.formId;
      }

      for (const photo of this.photos) {
        if (!photo.file) continue;
        const formData = new FormData();
        formData.append('file', photo.file);
        await fetch(`${import.meta.env.VITE_API_BASE_URL}/forms/${formId}/photos`, {
          method: 'POST',
          body: formData
        });
      }

      for (const [participantId, signature] of Object.entries(this.signatures)) {
        if (signature.startsWith('http')) continue;
        const blob = await fetch(signature).then(res => res.blob());
        const formData = new FormData();
        formData.append('participantId', participantId);
        formData.append('file', new File([blob], 'signature.png', { type: 'image/png' }));
        await fetch(`${import.meta.env.VITE_API_BASE_URL}/forms/${formId}/signatures`, {
          method: 'POST',
          body: formData
        });
      }

      this.closeModal();
    }
  }
});
</script>

  <style scoped>
  /* ❗ Используется тот же стиль, что и у DrillInspectionModal.vue */
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
  