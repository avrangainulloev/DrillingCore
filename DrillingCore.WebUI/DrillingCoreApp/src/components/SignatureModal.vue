<template>
  <div class="signature-modal-overlay" @click.self="close">
    <div class="signature-modal-content">
      <div class="modal-header">
        <h3 class="modal-title">Add Signature</h3>
        <button class="btn-close" @click="close">&times;</button>
      </div>

      <div class="modal-body">
        <canvas ref="canvas" class="signature-canvas" @mousedown="startDrawing" @mousemove="draw"
                @mouseup="stopDrawing" @mouseleave="stopDrawing"
                @touchstart.prevent="startTouch" @touchmove.prevent="moveTouch" @touchend="stopDrawing">
        </canvas>
        <button class="btn btn-secondary mt-2" @click="clear">Clear</button>
      </div>

      <div class="modal-footer">
        <button class="btn btn-success" @click="save">Save Signature</button>
        <button class="btn btn-secondary" @click="close">Cancel</button>
      </div>
    </div>
  </div>
</template>

<script lang="ts">
import { defineComponent, onMounted, ref } from 'vue';

export default defineComponent({
  name: 'SignatureModal',
  props: {
    participantId: { type: Number, required: true }
  },
  emits: ['close', 'signature-saved'],
  setup(props, { emit }) {
    const canvas = ref<HTMLCanvasElement | null>(null);
    const ctx = ref<CanvasRenderingContext2D | null>(null);
    const drawing = ref(false);

    onMounted(() => {
      if (canvas.value) {
        const canvasEl = canvas.value;
        canvasEl.width = 500;
        canvasEl.height = 150;
        ctx.value = canvasEl.getContext('2d');
        if (ctx.value) {
          ctx.value.lineWidth = 2;
          ctx.value.lineCap = 'round';
          ctx.value.strokeStyle = '#000';
        }
      }
    });

    const startDrawing = (e: MouseEvent) => {
      if (!ctx.value) return;
      drawing.value = true;
      ctx.value.beginPath();
      ctx.value.moveTo(e.offsetX, e.offsetY);
    };

    const draw = (e: MouseEvent) => {
      if (!drawing.value || !ctx.value) return;
      ctx.value.lineTo(e.offsetX, e.offsetY);
      ctx.value.stroke();
    };

    const startTouch = (e: TouchEvent) => {
      if (!ctx.value || !canvas.value) return;
      const rect = canvas.value.getBoundingClientRect();
      const touch = e.touches[0];
      drawing.value = true;
      ctx.value.beginPath();
      ctx.value.moveTo(touch.clientX - rect.left, touch.clientY - rect.top);
    };

    const moveTouch = (e: TouchEvent) => {
      if (!ctx.value || !canvas.value || !drawing.value) return;
      const rect = canvas.value.getBoundingClientRect();
      const touch = e.touches[0];
      ctx.value.lineTo(touch.clientX - rect.left, touch.clientY - rect.top);
      ctx.value.stroke();
    };

    const stopDrawing = () => {
      drawing.value = false;
    };

    const clear = () => {
      if (canvas.value && ctx.value) {
        ctx.value.clearRect(0, 0, canvas.value.width, canvas.value.height);
      }
    };

    const save = () => {
      if (canvas.value) {
        const dataUrl = canvas.value.toDataURL('image/png');
        emit('signature-saved', {
          participantId: props.participantId,
          signatureData: dataUrl
        });
      }
    };

    const close = () => emit('close');

    return {
      canvas,
      startDrawing,
      draw,
      stopDrawing,
      clear,
      save,
      close,
      startTouch,
      moveTouch
    };
  }
});
</script>

<style scoped>
.signature-modal-overlay {
  position: fixed;
  inset: 0;
  background: rgba(0, 0, 0, 0.6);
  z-index: 10000;
  display: flex;
  justify-content: center;
  align-items: center;
}
.signature-modal-content {
  background: white;
  padding: 1rem;
  border-radius: 8px;
  width: 520px;
  max-width: 90%;
  box-shadow: 0 0 10px rgba(0, 0, 0, 0.3);
}
.signature-canvas {
  border: 1px solid #ccc;
  width: 100%;
  height: 150px;
  touch-action: none;
}
.modal-header {
  display: flex;
  justify-content: space-between;
  align-items: center;
}
.modal-body {
  margin-top: 0.5rem;
}
.modal-footer {
  display: flex;
  justify-content: flex-end;
  margin-top: 1rem;
  gap: 0.5rem;
}
.btn {
  background-color: #1976d2;
  color: white;
  padding: 0.4rem 0.8rem;
  border: none;
  border-radius: 4px;
  cursor: pointer;
}
.btn-success {
  background-color: #28a745;
}
.btn-secondary {
  background-color: #6c757d;
}
.btn-close {
  background: transparent;
  font-size: 1.5rem;
  border: none;
}
</style>
