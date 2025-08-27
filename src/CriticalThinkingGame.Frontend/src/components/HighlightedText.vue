<template>
  <span>
    <template v-for="(segment, index) in textSegments" :key="index">
      <span 
        v-if="segment.isFallacy"
        :class="getFallacyClasses(segment.fallacy!)"
        :data-fallacy-id="segment.fallacy!.fallacyId"
      >{{ segment.text }}</span>
      <span v-else>{{ segment.text }}</span>
    </template>
  </span>
</template>

<script setup lang="ts">
import { computed } from 'vue'
import type { FallacyResult } from '@/services/api'
import { FallacyResultType } from '@/services/api'

interface TextSegment {
  text: string
  isFallacy: boolean
  fallacy?: FallacyResult
}

interface Props {
  text: string
  fallacies: FallacyResult[]
  hoveredFallacyId: number | null
}

const props = defineProps<Props>()

const textSegments = computed(() => {
  if (!props.text || !props.fallacies.length) {
    return [{ text: props.text || '', isFallacy: false }] as TextSegment[]
  }

  // Filter fallacies that have valid positions
  const validFallacies = props.fallacies.filter(f => f.startIndex >= 0 && f.endIndex > f.startIndex)
  
  if (!validFallacies.length) {
    return [{ text: props.text, isFallacy: false }] as TextSegment[]
  }

  // Sort fallacies by start position
  const sortedFallacies = [...validFallacies].sort((a, b) => a.startIndex - b.startIndex)

  const segments: TextSegment[] = []
  let currentIndex = 0

  for (const fallacy of sortedFallacies) {
    // Add text before fallacy if any
    if (currentIndex < fallacy.startIndex) {
      segments.push({
        text: props.text.substring(currentIndex, fallacy.startIndex),
        isFallacy: false
      })
    }

    // Add the fallacy segment
    segments.push({
      text: props.text.substring(fallacy.startIndex, fallacy.endIndex),
      isFallacy: true,
      fallacy: fallacy
    })

    currentIndex = fallacy.endIndex
  }

  // Add remaining text after last fallacy
  if (currentIndex < props.text.length) {
    segments.push({
      text: props.text.substring(currentIndex),
      isFallacy: false
    })
  }

  return segments
})

function getFallacyClasses(fallacy: FallacyResult): string {
  const classes = ['fallacy-base']
  
  if (fallacy.resultType === FallacyResultType.Correct) {
    classes.push('fallacy-correct')
  } else if (fallacy.resultType === FallacyResultType.Missed) {
    classes.push('fallacy-missed')
  }
  
  if (props.hoveredFallacyId === fallacy.fallacyId) {
    classes.push('fallacy-hovered')
  }
  
  return classes.join(' ')
}
</script>

<style scoped>
.fallacy-base {
  padding: 2px 4px;
  border-radius: 3px;
  transition: all 0.3s ease;
  border: 1px solid transparent;
}

.fallacy-correct {
  background-color: rgba(40, 167, 69, 0.3);
  border-color: rgba(40, 167, 69, 0.5);
}

.fallacy-missed {
  background-color: rgba(255, 193, 7, 0.3);
  border-color: rgba(255, 193, 7, 0.5);
}

.fallacy-hovered {
  background-color: rgba(255, 0, 0, 0.4) !important;
  border: 2px solid #ff0000 !important;
  padding: 1px 3px !important;
  box-shadow: 0 0 8px rgba(255, 0, 0, 0.3);
  transform: scale(1.02);
}
</style>
