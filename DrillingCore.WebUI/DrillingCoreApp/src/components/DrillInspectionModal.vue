<template>
    <div class="modal-overlay" @click.self="closeModal">
      <div class="modal-content drill-inspection-modal">
        <div class="modal-header">
          <h3 class="modal-title">Drill Inspection</h3>
          <button type="button" class="btn-close" @click="closeModal">&times;</button>
        </div>
        <div class="modal-body">
          <!-- Поля для Crew Name и Unit # -->
          <div class="form-group">
            <label for="crewName">Name (e.g. Crew #1):</label>
            <input
              type="text"
              id="crewName"
              v-model="crewName"
              class="form-control"
              placeholder="Enter crew name"
            />
          </div>
          <div class="form-group">
            <label for="unitNumber">Unit # or Reg Number:</label>
            <input
              type="text"
              id="unitNumber"
              v-model="unitNumber"
              class="form-control"
              placeholder="Enter unit or reg number"
            />
          </div>
          <!-- Выбор участников с поиском -->
          <div class="form-group participants-section">
            <label>Participants:</label>
            <div class="participants-filter">
              <input
                type="text"
                class="form-control search-participant"
                placeholder="Search participants..."
                v-model="participantSearch"
                @input="filterParticipants"
              />
            </div>
            <div class="participants-list">
              <div
                v-for="p in filteredParticipantOptions"
                :key="p.id"
                class="participant-item"
              >
                <label>
                  <input
                    type="checkbox"
                    :value="p.id"
                    v-model="selectedParticipantIds"
                  />
                  {{ p.name }}
                </label>
              </div>
            </div>
          </div>
          <!-- Чек-лист: две группы -->
          <div class="checklist-section">
            <div class="checklist-group">
              <h5>Equipment & Storage</h5>
              <div class="checklist-item" v-for="item in equipmentStorageItems" :key="item.id">
                <label>
                  <input type="checkbox" :value="item.id" v-model="checkedItems" />
                  {{ item.label }}
                </label>
              </div>
            </div>
            <div class="checklist-group">
              <h5>Safety & Accessories</h5>
              <div class="checklist-item" v-for="item in safetyAccessoryItems" :key="item.id">
                <label>
                  <input type="checkbox" :value="item.id" v-model="checkedItems" />
                  {{ item.label }}
                </label>
              </div>
            </div>
          </div>
          <!-- Блок Other (Комментарии/Ремонт) -->
          <div class="form-group">
            <h5>Other (Comments/Repairs)</h5>
            <textarea
              class="form-control"
              rows="3"
              v-model="otherComments"
              placeholder="Describe any additional repairs or notes here..."
            ></textarea>
          </div>
          <!-- Загрузка фотографий -->
          <div class="form-group photos-section">
  <h5>Attach Photos</h5>
  <div class="custom-file-input">
    <label for="photo-upload" class="custom-file-label">Choose Files</label>
    <input type="file" id="photo-upload" multiple accept="image/*" @change="onPhotosSelected" />
  </div>
  <div class="photo-gallery">
    <div
      v-for="(photo, index) in photos"
      :key="index"
      class="photo-item"
    >
      <div class="photo-wrapper" @click="openPhotoViewer(index)">
        <img :src="photo.preview" alt="Photo" class="photo-preview" />
        <button class="delete-photo-btn" @click.stop="deletePhoto(index)">×</button>
      </div>
    </div>
  </div>
</div>
          <!-- Подписи участников -->
          <div class="signatures-section">
            <h5>Signatures</h5>
            <div
              v-for="participantId in selectedParticipantIds"
              :key="participantId"
              class="signature-block"
            >
              <div class="signature-info">
                <strong>{{ getParticipantName(participantId) }}</strong>
                <button class="btn sign-btn" @click="openSignatureModal(participantId)">
                  Sign
                </button>
                <img
                  v-if="signatures[participantId]"
                  :src="signatures[participantId]"
                  alt="Signature"
                  class="signature-preview"
                />
              </div>
            </div>
          </div>
        </div>
        <div class="modal-footer">
          <button class="btn btn-success" @click="saveInspection">Save Inspection</button>
          <button class="btn btn-secondary" @click="closeModal">Cancel</button>
        </div>
      </div>
      <!-- Оверлей для просмотра фото -->
      <div class="photo-viewer-overlay" v-if="photoViewerIndex !== null" @click.self="closePhotoViewer">
        <div class="photo-viewer-content">
          <img :src="photos[photoViewerIndex].preview" alt="Full Photo" class="photo-full" />
          <button class="btn btn-secondary close-photo-btn" @click="closePhotoViewer">Close</button>
        </div>
      </div>
      <!-- Модальное окно для подписи -->
      <SignatureModal
        v-if="showSignatureModal"
        :participantId="currentSignatureParticipantId"
        @close="showSignatureModal = false"
        @signature-saved="onSignatureSaved"
      />
    </div>
  </template>
  
  <script lang="ts">
  import { defineComponent } from 'vue';
  import SignatureModal from './SignatureModal.vue';
  
  export default defineComponent({
    name: 'DrillInspectionModal',
    components: { SignatureModal },
    data() {
      return {
        crewName: '',
        unitNumber: '',
        allParticipants: [
          { id: 1, name: 'Vasyl Siziov' },
          { id: 2, name: 'Frangozeb Hinduter' },
          { id: 3, name: 'John Driller' },
          { id: 4, name: 'Emily Safety' },
          { id: 5, name: 'Participant 5' },
          { id: 6, name: 'Participant 6' },
          { id: 7, name: 'Participant 7' },
          { id: 8, name: 'Participant 8' },
          { id: 9, name: 'Participant 9' },
          { id: 10, name: 'Participant 10' },
          { id: 11, name: 'Participant 11' },
          { id: 12, name: 'Participant 12' },
          { id: 13, name: 'Participant 13' },
          { id: 14, name: 'Participant 14' },
          { id: 15, name: 'Participant 15' },
          { id: 16, name: 'Participant 16' },
          { id: 17, name: 'Participant 17' },
          { id: 18, name: 'Participant 18' },
          { id: 19, name: 'Participant 19' },
          { id: 20, name: 'Participant 20' },
          { id: 21, name: 'Participant 21' },
          { id: 22, name: 'Participant 22' }
        ],
        participantSearch: '',
        selectedParticipantIds: [] as number[],
        equipmentStorageItems: [
          { id: 11, label: 'Cap Mag (away from powder box)' },
          { id: 12, label: 'Propane stored upright and away from mags' },
          { id: 13, label: 'Hoods Covering Mag Locks' },
          { id: 14, label: 'Drive Line "U" Joints (Tandem Only)' },
          { id: 15, label: 'Light (Front, Back, Mast & Deck)' },
          { id: 16, label: 'Seat Belt (3 inch for LIS)' },
          { id: 17, label: 'Leaks' },
          { id: 18, label: 'Kelly Hose' },
          { id: 19, label: 'Cleanliness' },
          { id: 20, label: 'Break Out Bowl Ram' },
          { id: 21, label: 'Pressure Gauges' },
          { id: 22, label: 'Mast Rams' }
        ],
        safetyAccessoryItems: [
          { id: 31, label: '2-way Radio with external speaker' },
          { id: 32, label: 'Escape Hutch (LIS Only)' },
          { id: 33, label: 'Windows' },
          { id: 34, label: 'Pull Down Ram, Cable or chains' },
          { id: 35, label: 'Myno Pump' },
          { id: 36, label: 'Hydraulic Hoses' },
          { id: 37, label: 'Emergency Shut Down Switch' },
          { id: 38, label: 'Galvo' },
          { id: 39, label: '4 Brass Knife' },
          { id: 40, label: 'E Hoper Stinger Point' },
          { id: 41, label: 'First Aid Kit (B.C basic level with book)' },
          { id: 42, label: 'Fire Extinguisher (2-20lb BC or 2x 10lb AB)' },
          { id: 43, label: 'Regulator on Propane Tank' },
          { id: 44, label: 'Explosive Placards, MSDS on board' },
          { id: 45, label: 'ERP' },
          { id: 46, label: 'Back-Up Alarm' }
        ],
        checkedItems: [] as number[],
        otherComments: '',
        photos: [] as any[],
        photoViewerIndex: null as number | null,
        showSignatureModal: false,
        currentSignatureParticipantId: null as number | null,
        signatures: {} as Record<number, string>
      };
    },
    computed: {
      filteredParticipantOptions() {
        const term = this.participantSearch.toLowerCase();
        const selected = this.allParticipants.filter(p => this.selectedParticipantIds.includes(p.id));
        const notSelected = this.allParticipants.filter(p => !this.selectedParticipantIds.includes(p.id));
        const filterFn = (p: any) => p.name.toLowerCase().includes(term);
        return [...selected.filter(filterFn), ...notSelected.filter(filterFn)];
      }
    },
    methods: {
      closeModal() {
        this.$emit('close');
      },
      filterParticipants() {
        // Логика фильтрации реализована в computed
      },
      getParticipantName(id: number) {
        const p = this.allParticipants.find(part => part.id === id);
        return p ? p.name : 'Unknown';
      },
      onPhotosSelected(event: any) {
        const files = event.target.files;
        if (!files) return;
        for (let i = 0; i < files.length; i++) {
          const file = files[i];
          const reader = new FileReader();
          reader.onload = (e: any) => {
            this.photos.push({
              name: file.name,
              preview: e.target.result
            });
          };
          reader.readAsDataURL(file);
        }
      },
      openPhotoViewer(index: number) {
        this.photoViewerIndex = index;
      },
      closePhotoViewer() {
        this.photoViewerIndex = null;
      },
      deletePhoto(index: number) {
        this.photos.splice(index, 1);
      },
      openSignatureModal(participantId: number) {
        this.currentSignatureParticipantId = participantId;
        this.showSignatureModal = true;
      },
      onSignatureSaved(payload: { participantId: number; signatureData: string }) {
        this.signatures[payload.participantId] = payload.signatureData;
      },
      saveInspection() {
        const payload = {
          crewName: this.crewName,
          unitNumber: this.unitNumber,
          participants: this.selectedParticipantIds,
          checklist: this.checkedItems,
          otherComments: this.otherComments,
          photos: this.photos.map(p => p.preview),
          signatures: this.signatures
        };
        console.log('Saving Drill Inspection:', payload);
        alert('Drill Inspection saved. Check console for payload.');
        this.closeModal();
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
    background-color: rgba(0,0,0,0.6);
    display: flex;
    justify-content: center;
    align-items: center;
    z-index: 1100;
    padding: 1rem;
  }
  .drill-inspection-modal {
    background-color: #fff;
    padding: 1rem;
    border-radius: 8px;
    width: 90%;
    max-width: 800px;
    max-height: 90vh;
    overflow-y: auto;
    position: relative;
  }
  .modal-header {
    display: flex;
    justify-content: space-between;
    align-items: center;
    border-bottom: 1px solid #ddd;
    margin-bottom: 1rem;
  }
  .modal-title {
    margin: 0;
    font-size: 1.5rem;
  }
  .btn-close {
    background: transparent;
    border: none;
    font-size: 1.5rem;
    cursor: pointer;
  }
  .modal-body {
    padding: 0.5rem;
  }
  .modal-footer {
    display: flex;
    justify-content: flex-end;
    gap: 0.5rem;
    border-top: 1px solid #ddd;
    padding-top: 1rem;
    margin-top: 1rem;
  }
  .form-group {
    margin-bottom: 1rem;
  }
  .form-group label {
    display: block;
    margin-bottom: 0.3rem;
    font-weight: 600;
  }
  .form-control {
    width: 100%;
    padding: 0.4rem;
    border: 1px solid #ccc;
    border-radius: 4px;
  }
  .participants-section {
    margin-bottom: 1rem;
  }
  .participants-filter {
    margin-bottom: 0.5rem;
  }
  .search-participant {
    width: 100%;
  }
  .participants-list {
    max-height: 120px;
    overflow-y: auto;
    border: 1px solid #ccc;
    border-radius: 4px;
    padding: 0.5rem;
  }
  .participant-item {
    margin-bottom: 0.3rem;
  }
  .checklist-section {
    margin-bottom: 1rem;
  }
  .checklist-group {
    margin-bottom: 1rem;
  }
  .checklist-group h5 {
    margin-bottom: 0.5rem;
    font-size: 1rem;
    font-weight: 600;
  }
  .checklist-item {
    margin-left: 1rem;
    margin-bottom: 0.3rem;
  }
  .photos-section {
    margin-bottom: 1rem;
  }
  .photo-gallery {
    display: flex;
    flex-wrap: wrap;
    gap: 0.5rem;
    margin-top: 0.5rem;
  }
  .photo-item {
    display: flex;
    flex-direction: column;
    align-items: center;
    cursor: pointer;
    border: 1px solid #ddd;
    padding: 0.3rem;
    border-radius: 4px;
    position: relative;
  }
  .photo-wrapper {
    position: relative;
  }
  .photo-preview {
    width: 80px;
    height: 80px;
    object-fit: cover;
    border-radius: 4px;
  }

  .custom-file-input {
  position: relative;
  display: inline-block;
  margin-bottom: 1rem;
}

.custom-file-input input[type="file"] {
  display: none;
}

.custom-file-label {
  background-color: #1976d2;
  color: #fff;
  padding: 0.5rem 1rem;
  border-radius: 4px;
  cursor: pointer;
  transition: background-color 0.3s ease;
  display: inline-block;
}

.custom-file-label:hover {
  background-color: #1565c0;
}
  .delete-photo-btn {
    position: absolute;
    top: 2px;
    right: 2px;
    background-color: rgba(220, 53, 69, 0.8);
    border: none;
    color: #fff;
    font-size: 1rem;
    line-height: 1;
    padding: 0 4px;
    border-radius: 50%;
    cursor: pointer;
  }
  .signatures-section {
    margin-top: 1rem;
  }
  .signature-block {
    border: 1px solid #ddd;
    border-radius: 4px;
    padding: 0.5rem;
    margin-bottom: 0.5rem;
    display: flex;
    align-items: center;
    justify-content: space-between;
  }
  .signature-info {
    display: flex;
    gap: 1rem;
    align-items: center;
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
    color: #fff;
    border: none;
    padding: 0.4rem 0.8rem;
    border-radius: 4px;
    font-size: 0.9rem;
    cursor: pointer;
    transition: background-color 0.3s ease;
  }
  .btn:hover {
    background-color: #1565c0;
  }
  .btn-success {
    background-color: #28a745;
  }
  .btn-success:hover {
    background-color: #218838;
  }
  .btn-secondary {
    background-color: #6c757d;
  }
  .btn-secondary:hover {
    background-color: #5a6268;
  }
  .sign-btn {
    background-color: #fd7e14;
  }
  .sign-btn:hover {
    background-color: #e36b0a;
  }
  .photo-viewer-overlay {
    position: fixed;
    top: 0;
    left: 0;
    right: 0;
    bottom: 0;
    background-color: rgba(0,0,0,0.8);
    display: flex;
    align-items: center;
    justify-content: center;
    z-index: 2000;
  }
  .photo-viewer-content {
    position: relative;
  }
  .photo-full {
    max-width: 90vw;
    max-height: 80vh;
    object-fit: contain;
    border-radius: 4px;
  }
  .close-photo-btn {
    position: absolute;
    top: 1rem;
    right: 1rem;
    background-color: #6c757d;
  }
  .fade-enter-active,
  .fade-leave-active {
    transition: opacity 0.3s;
  }
  .fade-enter-from,
  .fade-leave-to {
    opacity: 0;
  }
  @media (max-width: 768px) {
    .form-type-grid {
      flex-direction: column;
      align-items: center;
    }
  }
  </style>
  