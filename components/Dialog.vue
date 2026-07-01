<template>
  <Transition name="modal">
    <div v-if="model" class="modal-mask">
      <div class="modal-container" @click.self="closeModal">
        <div class="modal-content">
          <slot></slot>
        </div>
      </div>
    </div>
  </Transition>
</template>

<script setup>
const model = defineModel()

defineProps({
  isOpen: {
    type: Boolean,
    default: false,
  },
  title: {
    type: String,
    default: "Modal Title",
  },
  subtitle: {
    type: String,
    default: "Modal SubTitle",
  },
  showFooter: {
    type: Boolean,
    default: true,
  },
})

/**
 * @type {import('vue').EmitsOptions}
 */
const emit = defineEmits(["close", "confirm"])

const closeModal = () => {
  model.value = false
}

const confirmAction = () => {
  emit("confirm")
}
</script>

<style scoped lang="scss">
/* Modal Transition */
.modal-enter-active,
.modal-leave-active {
  transition: opacity 0.3s ease;
}
