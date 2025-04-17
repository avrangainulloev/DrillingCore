<template>
  <div class="modal-overlay" @click.self="close">
    <div class="modal-dialog">
      <div class="modal-content">
        <div class="modal-header">
          <h5 class="modal-title">Timesheet for {{ userFullName }}</h5>
          <button class="btn-close" @click="close">&times;</button>
        </div>

        <div class="modal-body">
          <div class="form-section">
            <div class="form-group">
              <label>From:</label>
              <input type="date" v-model="fromDate" class="form-control" />
            </div>
            <div class="form-group">
              <label>To:</label>
              <input type="date" v-model="toDate" class="form-control" />
            </div>
            <div class="form-group">
              <button class="btn btn-primary" @click="generatePreview" :disabled="!fromDate || !toDate">
                Preview
              </button>
            </div>
          </div>

          <div class="table-scroll-wrapper">
            <table v-if="preview.length">
              <thead>
                <tr>
                  <th>Date</th>
                  <th>Day Rate</th>
                  <th>Meters</th>
                  <th>Total (Meters × Rate)</th>
                  <th>Narrative</th>
                </tr>
              </thead>
              <tbody>
                <tr v-for="day in preview" :key="day.date">
                  <td>{{ day.date }}</td>
                  <td>{{ day.dayRate.toFixed(2) }}</td>
                  <td>{{ day.meters.toFixed(2) }}</td>
                  <td>{{ (day.meters * day.meterRate).toFixed(2) }}</td>
                  <td>{{ day.narrative }}</td>
                </tr>
              </tbody>
              <tfoot>
                <tr>
                  <th>Total</th>
                  <th>{{ totalDayRate.toFixed(2) }}</th>
                  <th>{{ totalMeters.toFixed(2) }}</th>
                  <th>{{ totalMeterAmount.toFixed(2) }}</th>
                  <th></th>
                </tr>
              </tfoot>
            </table>
            <div v-else-if="previewRequested" class="no-data">
              No data found for this period.
            </div>
          </div>
        </div>

        <div class="modal-footer">
          <button class="btn btn-success" @click="exportToExcel" :disabled="!preview.length">
            Export to Excel
          </button>
        </div>
      </div>
    </div>
  </div>
</template>

<script setup lang="ts">
import { ref, computed } from 'vue';

const props = defineProps<{ userId: number; userFullName: string }>();
const emit = defineEmits(['close']);

const fromDate = ref('');
const toDate = ref('');
const preview = ref<any[]>([]);
const previewRequested = ref(false);

const close = () => emit('close');

const generatePreview = async () => {
  preview.value = [];
  previewRequested.value = true;

  const res = await fetch(`https://localhost:7200/api/Report/timesheet/preview`, {
    method: 'POST',
    headers: { 'Content-Type': 'application/json' },
    body: JSON.stringify({
      userId: props.userId,
      fromDate: fromDate.value,
      toDate: toDate.value
    })
  });

  if (res.ok) preview.value = await res.json();
  else alert('Failed to load preview');
};

const exportToExcel = async () => {
  const res = await fetch(`https://localhost:7200/api/Report/timesheet/excel`, {
    method: 'POST',
    headers: { 'Content-Type': 'application/json' },
    body: JSON.stringify({
      userId: props.userId,
      fromDate: fromDate.value,
      toDate: toDate.value
    })
  });

  if (res.ok) {
    const blob = await res.blob();
    const url = window.URL.createObjectURL(blob);
    const link = document.createElement('a');
    link.href = url;
    link.download = `Timesheet_${props.userFullName}_${fromDate.value}_to_${toDate.value}.xlsx`;
    link.click();
  } else {
    alert('Failed to export to Excel');
  }
};

const totalDayRate = computed(() => preview.value.reduce((sum, d) => sum + d.dayRate, 0));
const totalMeters = computed(() => preview.value.reduce((sum, d) => sum + d.meters, 0));
const totalMeterAmount = computed(() => preview.value.reduce((sum, d) => sum + (d.meters * d.meterRate), 0));
</script>

<style scoped>
.modal-overlay {
  position: fixed;
  inset: 0;
  background: rgba(0, 0, 0, 0.6);
  display: flex;
  justify-content: center;
  align-items: center;
  z-index: 9999;
  padding: 1rem;
}

.modal-dialog {
  width: 100%;
  max-width: 1100px;
  background: #fff;
  border-radius: 10px;
  max-height: 95vh;
  display: flex;
  flex-direction: column;
  overflow: hidden;
}

.modal-content {
  display: flex;
  flex-direction: column;
  height: 100%;
}

.modal-header {
  padding: 1rem;
  background: #f5f5f5;
  display: flex;
  justify-content: space-between;
  align-items: center;
  border-bottom: 1px solid #ddd;
}

.modal-body {
  flex-grow: 1;
  overflow-y: auto;
  padding: 1rem;
}

.modal-footer {
  padding: 1rem;
  background: #f9f9f9;
  border-top: 1px solid #ddd;
  text-align: right;
}

.btn {
  padding: 0.5rem 1rem;
  font-size: 0.95rem;
  border-radius: 4px;
  border: none;
  cursor: pointer;
}

.btn-success {
  background-color: #2e7d32;
  color: white;
}

.btn-primary {
  background-color: #1976d2;
  color: white;
}

.btn:disabled {
  background-color: #ccc;
  cursor: not-allowed;
}

.btn-close {
  background: none;
  border: none;
  font-size: 1.5rem;
  cursor: pointer;
}

.form-section {
  display: flex;
  flex-wrap: wrap;
  gap: 1rem;
  margin-bottom: 1rem;
}

.form-group {
  display: flex;
  flex-direction: column;
}

.table-scroll-wrapper {
  overflow-x: auto;
  max-height: 55vh;
  border: 1px solid #ccc;
  border-radius: 6px;
}

table {
  width: 100%;
  border-collapse: collapse;
}

th, td {
  padding: 0.6rem 0.75rem;
  border: 1px solid #ddd;
  font-size: 0.9rem;
}

th {
  background: #f0f0f0;
  font-weight: bold;
}

tfoot {
  background: #fafafa;
  font-weight: bold;
}

.no-data {
  padding: 1rem;
  text-align: center;
  color: #d32f2f;
}
</style>
