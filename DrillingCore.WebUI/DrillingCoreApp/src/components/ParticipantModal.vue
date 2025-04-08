<template>
  <div class="modal-overlay" @click.self="closeModal">
    <div class="modal-content participant-modal">
      <h3>Participants</h3>
      <!-- Верхняя панель действий -->
      <div class="actions">
        <button class="btn" @click="openAddGroupModal">Add Group</button>
        <button class="btn" @click="deleteSelectedGroups">Delete Group</button>
        <button class="btn" @click="openAddParticipantModal">Add Participant</button>
        <button class="btn" @click="deleteSelectedParticipants">Delete Selected Participants</button>
      </div>
      
      <!-- Таблица групп и участников -->
      <table class="participants-table">
        <thead>
          <tr>
            <th class="col-group">Group</th>
            <th class="col-name">Full Name</th>
            <th class="col-mobile">Mobile</th>
            <th class="col-date">Date Added</th>
            <th class="col-date">End Date</th>
            <th class="col-rate">Daily Rate</th>
            <th class="col-rate">Meter Rate</th>
            <th class="col-actions">Actions</th>
            <th class="col-select">Select</th>
          </tr>
        </thead>
        <tbody>
          <template v-for="group in participantGroups" :key="group.groupName">
            <tr class="group-header">
              <td colspan="9">{{ group.groupName }}</td>
            </tr>
            <tr v-for="participant in group.participants" :key="participant.id">
              <td class="small-cell col-group">{{ group.groupName }}</td>
              <td class="small-cell col-name">{{ participant.fullName || '-' }}</td>
              <td class="small-cell col-mobile">{{ participant.mobile || '-' }}</td>
              <!-- Используем одну и ту же функцию форматирования для обоих полей -->
              <td class="small-cell col-date">{{ formatDate(participant.startDate) }}</td>
              <td class="small-cell col-date">{{ formatDate(participant.endDate) }}</td>
              <td class="small-cell col-rate">
                {{ participant.dailyRate !== null && participant.dailyRate !== undefined ? participant.dailyRate : '-' }}
              </td>
              <td class="small-cell col-rate">
                {{ participant.meterRate !== null && participant.meterRate !== undefined ? participant.meterRate : '-' }}
              </td>
              <td class="small-cell col-actions">
                <button class="btn finish-btn" @click="openFinishModal(participant.id, participant.fullName)">Finish</button>
                <button class="btn equipment-btn" @click="openEquipmentModal(group.groupName, participant.id)">Equipment</button>
              </td>
              <td class="small-cell col-select">
                <input type="checkbox" v-model="selectedParticipants" :value="participant.id" />
              </td>
            </tr>
            <tr v-if="!group.participants || group.participants.length === 0">
              <td colspan="9" class="no-participants">No participants in this group.</td>
            </tr>
          </template>
        </tbody>
      </table>
      <button class="btn close-btn" @click="closeModal">Close</button>
    </div>

    <!-- Подключение модальных окон -->
    <AddGroupModal
      v-if="showAddGroupModal"
      :projectId="projectId"
      @close="showAddGroupModal = false"
      @group-created="loadParticipantGroups"
    />
    <DeleteGroupModal
      v-if="showDeleteGroupModal"
      :projectId="projectId"
      @close="showDeleteGroupModal = false"
      @groups-deleted="loadParticipantGroups"
    />
    <AddParticipantModal
      v-if="showAddParticipantModal"
      :projectId="projectId"
      @close="showAddParticipantModal = false"
      @participants-added="loadParticipantGroups"
    />
    <FinishParticipantModal
      v-if="showFinishModal"
      :projectId="projectId"
      :participantId="finishModalData.participantId"
      :participantName="finishModalData.participantName"
      @close="showFinishModal = false"
      @finished="onParticipantFinished"
    />
    <AttachEquipmentModal
  v-if="showEquipmentModal"
  :participantId="currentEquipmentParticipantId"
  :projectId="projectId"
  @close="showEquipmentModal = false"
  @equipment-attached="loadParticipantGroups"
/>
  </div>
</template>

<script lang="ts">
import { defineComponent } from 'vue';
import AddGroupModal from './AddGroupModal.vue';
import DeleteGroupModal from './DeleteGroupModal.vue';
import AddParticipantModal from './AddParticipantModal.vue';
import FinishParticipantModal from './FinishParticipantModal.vue';
import AttachEquipmentModal from './AttachEquipmentModal.vue';
export default defineComponent({
  name: 'ParticipantModal',
  components: {
    AddGroupModal,
    DeleteGroupModal,
    AddParticipantModal,
    FinishParticipantModal,
    AttachEquipmentModal
  },
  props: {
    projectId: {
      type: Number,
      required: true
    }
  },
  data() {
    return {
      participantGroups: [] as any[],
      selectedParticipants: [] as number[],
      showAddGroupModal: false,
      showDeleteGroupModal: false,
      showAddParticipantModal: false,
      showFinishModal: false,
      showEquipmentModal: false,
    currentEquipmentParticipantId: 0,
      finishModalData: {
        participantId: 0,
        participantName: ''
      }
    };
  },
  mounted() {
    this.loadParticipantGroups();
  },
  methods: {
    loadParticipantGroups() {
      const projectId = this.projectId;
      fetch(`https://localhost:7200/api/Projects/${projectId}/Groups`, { credentials: 'include' })
        .then(response => response.json())
        .then(data => {
          console.log("Loaded participant groups:", data);
          this.participantGroups = data;
        })
        .catch(err => console.error("Error loading participant groups:", err));
    },
    // Форматирует дату в формате "M/D/YYYY", например "3/13/2025"
    // Работает со строками вида "YYYY-MM-DD" или "YYYY-MM-DDT..." – берется только дата.
    formatDate(dateStr: string) {
      if (!dateStr) return '';
      // Если есть временная часть, берем только дату
      const datePart = dateStr.split('T')[0];
      const parts = datePart.split('-');
      if (parts.length !== 3) return dateStr;
      const year = parts[0];
      const month = parseInt(parts[1], 10); // убираем ведущий 0, если есть
      const day = parseInt(parts[2], 10);
      return `${month}/${day}/${year}`;
    },
    openAddGroupModal() {
      console.log("Open Add Group modal");
      this.showAddGroupModal = true;
    },
    deleteSelectedGroups() {
      console.log("Open Delete Group modal");
      this.showDeleteGroupModal = true;
    },
    openAddParticipantModal() {
      console.log("Open Add Participant modal");
      this.showAddParticipantModal = true;
    },
    deleteSelectedParticipants() {
      console.log("Delete selected participants:", this.selectedParticipants);
      // Реализуйте логику удаления участников по выбранным id
    },
    openFinishModal(participantId: number, participantName: string) {
      console.log(`Open Finish modal for participant ${participantId}`);
      this.finishModalData = { participantId, participantName };
      this.showFinishModal = true;
    },
    openEquipmentModal(groupName: string, participantId: number) {
  console.log(`Open equipment modal for participant ${participantId} in group ${groupName}`);
  this.currentEquipmentParticipantId = participantId;
  this.showEquipmentModal = true;
    },
    onParticipantFinished() {
      // Обновляем список участников после завершения работы участника
      this.loadParticipantGroups();
      this.showFinishModal = false;
    },
    closeModal() {
      console.log("Closing Participant Modal");
      this.$emit('close');
    }
  }
});
</script>

<style scoped>
/* Единый фон и выравнивание для всех модальных окон */
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

/* Основной контейнер модального окна - увеличена максимальная ширина */
.modal-content.participant-modal {
  background-color: #fff;
  padding: 1.5rem;
  border-radius: 8px;
  width: 90%;
  max-width: 950px;
  box-shadow: 0 4px 12px rgba(0,0,0,0.2);
  max-height: 90vh;
  overflow-y: auto;
  position: relative;
}

/* Заголовок окна */
.modal-content h3 {
  margin-top: 0;
  text-align: center;
  font-size: 1.5rem;
  font-weight: bold;
  margin-bottom: 1rem;
}

/* Верхняя панель действий */
.actions {
  display: flex;
  gap: 0.5rem;
  margin-bottom: 1rem;
  flex-wrap: wrap;
  justify-content: center;
}

/* Базовый стиль кнопок */
.btn {
  background-color: #1976d2;
  color: #fff;
  border: none;
  padding: 0.5rem 1rem;
  border-radius: 4px;
  cursor: pointer;
  transition: background-color 0.3s ease;
  font-size: 0.9rem;
}
.btn:hover {
  background-color: #1565c0;
}

/* Специальные стили для кнопок Finish и Equipment */
.finish-btn {
  background-color: #28a745;
  margin-right: 0.3rem;
  padding: 0.3rem 0.6rem;
  font-size: 0.8rem;
}
.finish-btn:hover {
  background-color: #218838;
}
.equipment-btn {
  background-color: #fd7e14;
  padding: 0.3rem 0.6rem;
  font-size: 0.8rem;
}
.equipment-btn:hover {
  background-color: #e36b0a;
}

/* Таблица участников */
.participants-table {
  width: 100%;
  border-collapse: collapse;
  margin-bottom: 1rem;
}
.participants-table th,
.participants-table td {
  border: 1px solid #ddd;
  padding: 0.2rem;
  text-align: left;
  font-size: 0.8rem;
  line-height: 1;
  max-width: 140px;
  word-wrap: break-word;
}
.participants-table th {
  background-color: #f7f7f7;
  font-weight: bold;
}

/* Индивидуальные стили колонок */
.col-group { max-width: 90px; }
.col-name { max-width: 140px; }
.col-mobile { max-width: 90px; }
.col-date { max-width: 80px; }
.col-rate { max-width: 90px; }
.col-actions { max-width: 110px; }
.col-select { max-width: 50px; }

/* Стили для заголовка группы */
.group-header {
  background-color: #e0e0e0;
  font-weight: bold;
  font-size: 0.9rem;
}

/* Дополнительный класс для уменьшенных ячеек */
.small-cell {
  padding: 0.2rem;
}

/* Стиль для сообщений об отсутствии участников */
.no-participants {
  font-style: italic;
  color: #777;
  padding: 0.2rem;
}

/* Стиль для кнопки закрытия */
.close-btn {
  background-color: #ccc;
  color: #333;
  border: none;
  padding: 0.5rem 1rem;
  border-radius: 4px;
  cursor: pointer;
  transition: background-color 0.3s ease;
  display: block;
  margin: 0 auto;
}
.close-btn:hover {
  background-color: #bbb;
}
</style>
