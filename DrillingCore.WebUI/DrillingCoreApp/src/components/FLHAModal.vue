<template>
    <div>
      <div class="modal-overlay" @click.self="closeModal">
        <div class="modal-content">
          <div class="modal-header">
            <h3>Field Level Hazard Assessment</h3>
            <button class="btn-close" @click="closeModal">&times;</button>
          </div>
  
          <div class="modal-body">
            <!-- Task and Date -->
            <div class="form-group">
              <label>Task Description</label>
              <input v-model="taskDescription" class="form-control" />
            </div>
            <div class="form-group">
              <label>Date</label>
              <input type="date" v-model="dateFilled" class="form-control" />
            </div>
  
            <!-- Participants -->
            <div class="form-section">
              <h5>Participants</h5>
              <input v-model="participantSearch" class="form-control" placeholder="Search participants..." />
              <div class="participants-list">
                <div v-for="p in filteredParticipants" :key="p.id" class="participant-item">
                  <label><input type="checkbox" :value="p.id" v-model="selectedParticipantIds" /> {{ p.fullName }}</label>
                </div>
              </div>
            </div>
  
            <!-- Hazards -->
            <div class="form-section">
              <h5>Identified Hazards</h5>
              <div v-for="hazard in hazardTemplates" :key="hazard.id" class="hazard-item">
                <label>
                  <input type="checkbox" :value="hazard.id" v-model="selectedHazardIds" />
                  {{ hazard.label }}
                </label>
                <textarea
                  v-if="selectedHazardIds.includes(hazard.id)"
                  class="form-control mt-1"
                  v-model="controls[hazard.id]"
                  placeholder="Control Measures..."
                />
              </div>
  
              <!-- Custom Hazards -->
              <div class="custom-hazards">
                <h6>Custom Hazards</h6>
                <div v-for="(hazard, index) in customHazards" :key="index" class="custom-hazard-entry">
                  <input v-model="hazard.label" class="form-control" placeholder="Hazard" />
                  <textarea v-model="hazard.control" class="form-control mt-1" placeholder="Control Measures"></textarea>
                  <button class="btn btn-sm btn-danger mt-1" @click="removeCustomHazard(index)">Remove</button>
                </div>
                <button class="btn btn-sm btn-secondary mt-2" @click="addCustomHazard">+ Add Hazard</button>
              </div>
            </div>
  
            <!-- Other Comments -->
            <div class="form-group">
              <label>Other Comments</label>
              <textarea v-model="otherComments" rows="3" class="form-control"></textarea>
            </div>
  
            <!-- Photo Upload -->
            <div class="form-section">
              <h5>Attach Photos</h5>
              <input type="file" multiple @change="onPhotosSelected" accept="image/*" class="form-control" />
              <div class="photo-preview-list">
                <div v-for="(photo, idx) in photos" :key="idx" class="preview-wrapper">
                  <img :src="photo.preview" class="preview-img" @click="viewPhoto(photo.preview)" />
                  <button class="remove-btn" @click="removePhoto(idx)">&times;</button>
                </div>
              </div>
            </div>
  
            <!-- Signatures -->
            <div class="form-section">
              <h5>Signatures</h5>
              <div v-for="participantId in selectedParticipantIds" :key="participantId" class="signature-block">
                <strong>{{ getParticipantName(participantId) }}</strong>
                <button class="btn sign-btn" @click="openSignatureModal(participantId)">Sign</button>
                <img v-if="signatures[participantId]" :src="signatures[participantId]" class="signature-preview" />
              </div>
            </div>
  
            <!-- Photo modal -->
            <div v-if="photoToView" class="photo-modal" @click="photoToView = ''">
              <img :src="photoToView" />
            </div>
  
            <!-- Signature modal -->
            <SignatureModal
              v-if="showSignatureModal && currentSignatureParticipantId !== null"
              :participantId="currentSignatureParticipantId"
              @close="showSignatureModal = false"
              @signature-saved="onSignatureSaved"
            />
          </div>
  
          <div class="modal-footer">
            <button class="btn btn-success" @click="saveFLHAForm">Save</button>
            <button class="btn btn-secondary" @click="closeModal">Cancel</button>
          </div>
        </div>
      </div>
    </div>
  </template>
  
  <script lang="ts">
  import { defineComponent } from 'vue';
  import SignatureModal from './SignatureModal.vue';
  
  export default defineComponent({
    name: 'FLHAModal',
    components: { SignatureModal },
    emits: ['close'],
    props: {
      userId: { type: Number, required: true },
      projectId: { type: Number, required: true },
      formId: { type: Number, required: false }
    },
    data() {
      return {
        taskDescription: '',
        dateFilled: new Date().toISOString().split('T')[0],
        participantSearch: '',
        allParticipants: [] as any[],
        selectedParticipantIds: [] as number[],
        hazardTemplates: [] as any[],
        selectedHazardIds: [] as number[],
        controls: {} as Record<number, string>,
        customHazards: [] as { label: string; control: string }[],
        otherComments: '',
        photos: [] as { preview: string; file: File | null }[],
        signatures: {} as Record<number, string>,
        showSignatureModal: false,
        currentSignatureParticipantId: null as number | null,
        photoToView: ''
      };
    },
    computed: {
      filteredParticipants() {
        return this.allParticipants.filter(p => p.fullName.toLowerCase().includes(this.participantSearch.toLowerCase()));
      }
    },
    watch: {
  selectedHazardIds(newVal: number[], oldVal: number[]) {
    const added = newVal.filter(id => !oldVal.includes(id));
    const removed = oldVal.filter(id => !newVal.includes(id));

    for (const id of added) {
      const hazard = this.hazardTemplates.find(h => h.id === id);
      if (hazard) this.controls[id] = hazard.controlSuggestion || '';
    }

    for (const id of removed) {
      delete this.controls[id];
    }
  }
},
    mounted() {
      this.loadParticipants();
      this.loadHazards();
    },
    methods: {
      closeModal() {
        this.$emit('close');
      },
      getParticipantName(id: number) {
        const p = this.allParticipants.find(x => x.id === id);
        return p?.fullName || 'Unknown';
      },
      async loadParticipants() {
        const res = await fetch(`/api/projects/${this.projectId}/groups`);
        const groups = await res.json();
        this.allParticipants = groups.flatMap((g: any) => g.participants);
      },
      async loadHazards() {
        const res = await fetch(`/api/flha/hazards/2`);
        this.hazardTemplates = await res.json();
      },
      addCustomHazard() {
        this.customHazards.push({ label: '', control: '' });
      },
      removeCustomHazard(index: number) {
        this.customHazards.splice(index, 1);
      },
      onPhotosSelected(event: any) {
        const files = event.target.files;
        for (let file of files) {
          const reader = new FileReader();
          reader.onload = e => {
            this.photos.push({ preview: e.target?.result as string, file });
          };
          reader.readAsDataURL(file);
        }
      },
      removePhoto(idx: number) {
        this.photos.splice(idx, 1);
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
      async saveFLHAForm() {
        const payload = {
          projectId: this.projectId,
          creatorId: this.userId,
          taskDescription: this.taskDescription,
          dateFilled: this.dateFilled,
          otherComments: this.otherComments,
          participants: this.selectedParticipantIds.map(id => ({
            participantId: id,
            signature: this.signatures[id] || null
          })),
          hazards: [
            ...this.selectedHazardIds.map(id => ({
              hazardTemplateId: id,
              hazardText: this.hazardTemplates.find(h => h.id === id)?.label || '',
              controlMeasures: this.controls[id]
            })),
            ...this.customHazards.map(h => ({
              hazardText: h.label,
              controlMeasures: h.control
            }))
          ]
        };
  
        const res = await fetch('/api/forms/flha', {
          method: 'POST',
          headers: { 'Content-Type': 'application/json' },
          body: JSON.stringify(payload)
        });
        const result = await res.json();
        const formId = result.formId;
  
        // Upload photos
        for (const photo of this.photos) {
          if (!photo.file) continue;
          const formData = new FormData();
          formData.append('file', photo.file);
          await fetch(`/api/forms/${formId}/photos`, { method: 'POST', body: formData });
        }
  
        // Upload signatures
        for (const [participantId, signature] of Object.entries(this.signatures)) {
          if (signature.startsWith('http')) continue;
          const blob = await fetch(signature).then(res => res.blob());
          const formData = new FormData();
          formData.append('participantId', participantId);
          formData.append('file', new File([blob], 'signature.png', { type: 'image/png' }));
          await fetch(`/api/forms/${formId}/signatures`, { method: 'POST', body: formData });
        }
  
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
    z-index: 1000;
  }
  .modal-content {
    background: white;
    padding: 1rem;
    width: 95%;
    max-width: 850px;
    border-radius: 10px;
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
    border-radius: 6px;
    border: 1px solid #ccc;
  }
  .form-section {
    margin-top: 1rem;
    margin-bottom: 1rem;
  }
  .participants-list {
    max-height: 150px;
    overflow-y: auto;
    border: 1px solid #ccc;
    padding: 0.5rem;
    border-radius: 6px;
  }
  .hazard-item {
    background: #f9f9f9;
    border: 1px solid #ddd;
    padding: 0.5rem;
    margin-bottom: 0.75rem;
    border-radius: 6px;
  }
  .custom-hazard-entry {
    margin-top: 0.5rem;
    background: #eef;
    border: 1px solid #aad;
    padding: 0.5rem;
    border-radius: 6px;
  }
  .photo-preview-list {
    display: flex;
    flex-wrap: wrap;
    gap: 0.5rem;
    margin-top: 0.5rem;
  }
  .preview-img {
    width: 80px;
    height: 80px;
    object-fit: cover;
    border-radius: 4px;
  }
  .remove-btn {
    position: absolute;
    top: 2px;
    right: 2px;
    background: rgba(0, 0, 0, 0.6);
    color: white;
    border: none;
    border-radius: 4px;
    cursor: pointer;
  }
  .signature-block {
    display: flex;
    justify-content: space-between;
    align-items: center;
    border: 1px dashed #ccc;
    padding: 0.5rem;
    margin-top: 0.5rem;
    border-radius: 6px;
  }
  .signature-preview {
    width: 100px;
    height: 60px;
    object-fit: contain;
    border-radius: 4px;
    border: 1px solid #ccc;
  }
  .photo-modal {
    position: fixed;
    inset: 0;
    background: rgba(0, 0, 0, 0.9);
    display: flex;
    justify-content: center;
    align-items: center;
    z-index: 9999;
  }
  .photo-modal img {
    max-width: 90vw;
    max-height: 90vh;
  }
  .btn {
    padding: 0.4rem 1rem;
    border: none;
    border-radius: 4px;
  }
  .btn-success {
    background-color: #28a745;
    color: white;
  }
  .btn-secondary {
    background-color: #6c757d;
    color: white;
  }
  .sign-btn {
    background-color: #ff9800;
    color: white;
  }
  </style>
  