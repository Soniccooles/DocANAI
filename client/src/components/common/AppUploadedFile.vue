<script setup lang="ts">
import { computed } from 'vue'
import { VALID_FILE_TYPES } from '../../utils/constants'
import { getFileType } from '../../utils/files'

import txtIcon from '../../assets/images/txt.png'
import mdIcon from '../../assets/images/md.svg'
import docxIcon from '../../assets/images/docx.png'
import pdfIcon from '../../assets/images/pdf.png'
import defaultIcon from '../../assets/images/default.svg'

const props = defineProps<{
  file: File
}>()

const emit = defineEmits<{
  (e: 'fileRemove', file: File): void
}>()

const ICONS: Record<string, string> = {
  [VALID_FILE_TYPES.TXT]: txtIcon,
  [VALID_FILE_TYPES.MD]: mdIcon,
  [VALID_FILE_TYPES.DOCX]: docxIcon,
  [VALID_FILE_TYPES.PDF]: pdfIcon,
}

const iconSrc = computed(() => ICONS[getFileType(props.file)] ?? defaultIcon)
</script>

<template>
  <div class="file">
    <img class="file__image" :src="iconSrc" alt="" width="24" height="24">
    <div class="file__name">{{ file.name }}</div>
    <button
      class="file__remove"
      type="button"
      @click="emit('fileRemove', file)"
    >
      ✕
    </button>
  </div>
</template>

<style scoped lang="scss">
@use '../../styles/helpers/' as *;

.file {
  @include fluid-text(16, 14);

  display: flex;
  align-items: center;
  column-gap: rem(12);

  &__image {
    @include square(24);
  }

  &__remove {
    @include square(24);
    @include flex-center;

    color: var(--color-light);
    background-color: var(--color-red);
    border: rem(1) solid var(--color-red);
    border-radius: 50%;
    transition-duration: var(--transition-duration);

    @include hover {
      background-color: transparent;
      color: var(--color-red);
    }
  }
}
</style>
