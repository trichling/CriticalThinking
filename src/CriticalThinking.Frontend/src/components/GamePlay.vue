<template>
  <div class="game-play">
    <div class="game-header">
      <div class="player-info">
        <h2>{{ gameStore.state.playerName }}</h2>
        <span class="difficulty">{{ gameStore.state.difficulty }} Mode</span>
      </div>
      <div class="game-stats">
        <div class="timer">
          Time: {{ formatTime(elapsedTime) }}
        </div>
        <div class="selection-count">
          Selected: {{ gameStore.state.selectedFallacies.length }}
        </div>
      </div>
    </div>

    <div class="game-content">
      <div class="text-section">
        <h3>Read the text and identify the logical fallacies:</h3>
        <div class="game-text">
          {{ gameStore.state.gameText }}
        </div>
      </div>

      <div class="fallacies-section">
        <h3>Select the fallacies you found:</h3>
        <div class="fallacies-grid">
          <label
            v-for="fallacy in gameStore.state.availableFallacies"
            :key="fallacy.id"
            class="fallacy-option"
            :class="{ selected: gameStore.state.selectedFallacies.includes(fallacy.id) }"
          >
            <input
              type="checkbox"
              :value="fallacy.id"
              :checked="gameStore.state.selectedFallacies.includes(fallacy.id)"
              @change="gameStore.toggleFallacy(fallacy.id)"
            >
            <div class="fallacy-card">
              <h4>{{ fallacy.name }}</h4>
              <p class="fallacy-description">{{ fallacy.description }}</p>
            </div>
          </label>
        </div>
      </div>

      <div class="actions">
        <button
          @click="handleSubmit"
          :disabled="submitting || gameStore.state.selectedFallacies.length === 0"
          class="submit-button"
        >
          {{ submitting ? 'Submitting...' : 'Submit Answer' }}
        </button>
      </div>
    </div>
  </div>
</template>

<script setup lang="ts">
import { ref, computed, onMounted, onUnmounted } from 'vue'
import { useGameStore } from '@/stores/gameStore'

const emit = defineEmits<{
  gameCompleted: []
}>()

const gameStore = useGameStore()
const submitting = ref(false)
const elapsedTime = ref(0)
let timer: ReturnType<typeof setInterval> | null = null

const formatTime = (seconds: number) => {
  const minutes = Math.floor(seconds / 60)
  const remainingSeconds = seconds % 60
  return `${minutes}:${remainingSeconds.toString().padStart(2, '0')}`
}

const updateTimer = () => {
  if (gameStore.state.startTime) {
    elapsedTime.value = Math.floor((new Date().getTime() - gameStore.state.startTime.getTime()) / 1000)
  }
}

async function handleSubmit() {
  if (gameStore.state.selectedFallacies.length === 0) {
    alert('Please select at least one fallacy before submitting.')
    return
  }

  submitting.value = true
  try {
    await gameStore.submitGame()
    emit('gameCompleted')
  } catch (error) {
    console.error('Failed to submit game:', error)
    alert('Failed to submit your answer. Please try again.')
  } finally {
    submitting.value = false
  }
}

onMounted(() => {
  updateTimer()
  timer = setInterval(updateTimer, 1000)
})

onUnmounted(() => {
  if (timer) {
    clearInterval(timer)
  }
})
</script>

<style scoped>
.game-play {
  max-width: 1000px;
  margin: 0 auto;
  padding: 1rem;
}

.game-header {
  display: flex;
  justify-content: space-between;
  align-items: center;
  padding: 1rem;
  background-color: #f8f9fa;
  border-radius: 8px;
  margin-bottom: 2rem;
}

.player-info h2 {
  margin: 0;
  color: #333;
}

.difficulty {
  background-color: #007bff;
  color: white;
  padding: 0.25rem 0.5rem;
  border-radius: 4px;
  font-size: 0.9rem;
  font-weight: bold;
}

.game-stats {
  display: flex;
  gap: 1rem;
  align-items: center;
}

.timer, .selection-count {
  padding: 0.5rem 1rem;
  background-color: white;
  border: 1px solid #ddd;
  border-radius: 4px;
  font-weight: bold;
}

.game-content {
  display: grid;
  gap: 2rem;
}

.text-section h3, .fallacies-section h3 {
  margin: 0 0 1rem 0;
  color: #333;
}

.game-text {
  padding: 1.5rem;
  background-color: #f8f9fa;
  border: 1px solid #ddd;
  border-radius: 8px;
  line-height: 1.6;
  font-size: 1.1rem;
}

.fallacies-grid {
  display: grid;
  grid-template-columns: repeat(auto-fit, minmax(300px, 1fr));
  gap: 1rem;
}

.fallacy-option {
  cursor: pointer;
}

.fallacy-option input[type="checkbox"] {
  display: none;
}

.fallacy-card {
  padding: 1rem;
  border: 2px solid #ddd;
  border-radius: 8px;
  transition: all 0.2s;
  background-color: white;
}

.fallacy-option.selected .fallacy-card {
  border-color: #28a745;
  background-color: #f8fff9;
}

.fallacy-card:hover {
  border-color: #007bff;
  transform: translateY(-1px);
}

.fallacy-card h4 {
  margin: 0 0 0.5rem 0;
  color: #333;
}

.fallacy-description {
  margin: 0;
  color: #666;
  font-size: 0.9rem;
}

.actions {
  text-align: center;
  margin-top: 2rem;
}

.submit-button {
  padding: 1rem 2rem;
  background-color: #28a745;
  color: white;
  border: none;
  border-radius: 4px;
  font-size: 1.1rem;
  font-weight: bold;
  cursor: pointer;
  transition: background-color 0.2s;
}

.submit-button:hover:not(:disabled) {
  background-color: #218838;
}

.submit-button:disabled {
  background-color: #ccc;
  cursor: not-allowed;
}
</style>