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
    max-width: 90%;
    max-height: 90%;
    padding: 1.5rem;
    position: relative;
    overflow-y: auto;
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
  