<template>
    <div class="modal-overlay" @click.self="$emit('close')">
      <div class="modal-window">
        <button class="close-btn" @click="$emit('close')">&times;</button>
        <slot></slot>
      </div>
    </div>
  </template>
  
  <script lang="ts">
  import { defineComponent } from 'vue';
  
  export default defineComponent({
    name: 'Modal',
    emits: ['close']
  });
  </script>
  
  <style scoped>
  .modal-overlay {
    position: fixed;
    inset: 0;
    background: rgba(0, 0, 0, 0.5);
    display: flex;
    align-items: center;
    justify-content: center;
    z-index: 10000;
  }
  
  .modal-window {
  background: white;
  border-radius: 8px;
  width: 900px;              /* фиксированная ширина */
  max-width: 95vw;
  max-height: 90vh;
  padding: 1.5rem;
  position: relative;
  overflow-y: auto;
  box-shadow: 0 5px 25px rgba(0, 0, 0, 0.2);
}
  
  .close-btn {
    position: absolute;
    top: 8px;
    right: 12px;
    background: none;
    border: none;
    font-size: 1.5rem;
    font-weight: bold;
    cursor: pointer;
    color: #555;
  }
  </style>
  