<script setup lang="ts">
defineProps<{
  title?: string
  text: string
}>()
</script>

<template>
  <div class="error-message" role="alert">
    <p v-if="title" class="error-message__title">{{ title }}</p>
    <p class="error-message__text">{{ text }}</p>
    <slot></slot>
  </div>
</template>

<style scoped lang="scss">
@use '../../styles/helpers/' as *;

.error-message {
  @include fluid-text(20, 16);

  display: flex;
  flex-direction: column;
  align-items: center;
  row-gap: rem(6);
  text-align: center;

  &__title {
    font-weight: 600;
  }

  &__text {
    color: var(--color-gray);
    font-weight: 500;
  }
}
</style>