<template>
  <div class="game-page">
    <div v-if="!gameStore.isGameActive" class="loading">
      <p>Loading game...</p>
    </div>

    <div v-else class="game-container">
      <div class="game-header">
        <h2>Find the Logical Fallacies</h2>
        <div class="game-info">
          <span class="player-name">Player: {{ gameStore.playerName }}</span>
          <span class="timer">Time: {{ formatTime(elapsedTime) }}</span>
        </div>
      </div>

      <div class="game-content">
        <!-- Main Content Area -->
        <div class="main-content">
          <!-- Text Section -->
          <div class="text-section">
            <div class="text-container">
              <h3>Read the text and identify the logical fallacies:</h3>
              <div class="game-text">
                {{ gameStore.gameText }}
              </div>
            </div>
          </div>

          <!-- Fallacies Sidebar -->
          <div class="fallacies-sidebar">
            <div class="fallacies-container">
              <h3>Select fallacies ({{ gameStore.selectedFallacies.length }} selected):</h3>
              <div class="fallacies-list">
                <div
                  v-for="fallacy in gameStore.availableFallacies"
                  :key="fallacy.id"
                  class="fallacy-card"
                  :class="{ selected: gameStore.selectedFallacies.includes(fallacy.id) }"
                  @click="gameStore.toggleFallacy(fallacy.id)"
                >
                  <div class="fallacy-header">
                    <h4>{{ fallacy.name }}</h4>
                    <span class="difficulty-badge" :class="`difficulty-${fallacy.difficulty}`">
                      {{ gameStore.getDifficultyName(fallacy.difficulty) }}
                    </span>
                  </div>
                  <p>{{ fallacy.description }}</p>
                </div>
              </div>
            </div>
          </div>
        </div>

        <div class="game-actions">
          <button 
            @click="submitAnswer" 
            :disabled="gameStore.isLoading || gameStore.selectedFallacies.length === 0"
            class="submit-button"
          >
            <span v-if="gameStore.isLoading">Submitting...</span>
            <span v-else>Submit Answer ({{ gameStore.selectedFallacies.length }} fallacies)</span>
          </button>
        </div>
      </div>

      <div v-if="gameStore.error" class="error-message">
        {{ gameStore.error }}
      </div>
    </div>
  </div>
</template>

<script setup lang="ts">
import { ref, onMounted, onUnmounted } from 'vue'
import { useRouter } from 'vue-router'
import { useGameStore } from '@/stores/game'

const router = useRouter()
const gameStore = useGameStore()

const elapsedTime = ref(0)
let intervalId: number | null = null

onMounted(() => {
  if (!gameStore.isGameActive) {
    router.push('/')
    return
  }

  // Start timer
  intervalId = setInterval(() => {
    if (gameStore.startTime) {
      elapsedTime.value = Math.floor((Date.now() - gameStore.startTime.getTime()) / 1000)
    }
  }, 1000)
})

onUnmounted(() => {
  if (intervalId) {
    clearInterval(intervalId)
  }
})

async function submitAnswer() {
  await gameStore.submitAnswer()
  if (!gameStore.error) {
    router.push('/results')
  }
}

function formatTime(seconds: number): string {
  const minutes = Math.floor(seconds / 60)
  const remainingSeconds = seconds % 60
  return `${minutes}:${remainingSeconds.toString().padStart(2, '0')}`
}
</script>

<style scoped>
.game-page {
  max-width: 1400px;
  margin: 0 auto;
  padding: 1rem;
}

.loading {
  text-align: center;
  padding: 2rem;
  font-size: 1.2rem;
}

.game-container {
  background: rgba(255, 255, 255, 0.95);
  color: #333;
  border-radius: 1rem;
  padding: 2rem;
  box-shadow: 0 8px 32px rgba(0, 0, 0, 0.1);
}

.game-header {
  display: flex;
  justify-content: space-between;
  align-items: center;
  margin-bottom: 1.5rem;
  padding-bottom: 1rem;
  border-bottom: 2px solid #e1e8ed;
}

.game-header h2 {
  margin: 0;
  color: #2c3e50;
  font-size: 1.5rem;
}

.game-info {
  display: flex;
  gap: 2rem;
  font-weight: bold;
}

.player-name {
  color: #667eea;
}

.timer {
  color: #e74c3c;
}

.game-content {
  display: flex;
  flex-direction: column;
  gap: 1.5rem;
}

.main-content {
  display: flex;
  gap: 2rem;
}

.text-section {
  flex: 2;
  min-width: 0;
}

.fallacies-sidebar {
  flex: 1;
  min-width: 300px;
  max-width: 400px;
}

.text-container h3,
.fallacies-container h3 {
  margin-bottom: 1rem;
  color: #2c3e50;
}

.game-text {
  background: #f8f9fa;
  padding: 1.5rem;
  border-radius: 0.5rem;
  border-left: 4px solid #667eea;
  line-height: 1.8;
  font-size: 1.1rem;
  white-space: pre-wrap;
  border: 1px solid #e9ecef;
}

.fallacies-list {
  max-height: 600px;
  overflow-y: auto;
  padding-right: 0.5rem;
}

/* Scrollbar styling */
.fallacies-list::-webkit-scrollbar {
  width: 8px;
}

.fallacies-list::-webkit-scrollbar-track {
  background: #f1f1f1;
  border-radius: 4px;
}

.fallacies-list::-webkit-scrollbar-thumb {
  background: #c1c1c1;
  border-radius: 4px;
}

.fallacies-list::-webkit-scrollbar-thumb:hover {
  background: #a8a8a8;
}

.fallacy-card {
  background: white;
  border: 2px solid #e1e8ed;
  border-radius: 0.5rem;
  padding: 1rem;
  cursor: pointer;
  transition: all 0.3s;
  margin-bottom: 1rem;
}

.fallacy-card:hover {
  border-color: #667eea;
  transform: translateY(-2px);
  box-shadow: 0 4px 12px rgba(0, 0, 0, 0.1);
}

.fallacy-card.selected {
  border-color: #27ae60;
  background: #e8f5e8;
}

.fallacy-header {
  display: flex;
  justify-content: space-between;
  align-items: flex-start;
  margin-bottom: 0.5rem;
}

.fallacy-card h4 {
  margin: 0;
  color: #2c3e50;
  font-size: 1rem;
  flex: 1;
}

.fallacy-card p {
  margin: 0;
  color: #666;
  font-size: 0.9rem;
  line-height: 1.4;
}

.difficulty-badge {
  padding: 0.25rem 0.5rem;
  border-radius: 0.25rem;
  font-size: 0.7rem;
  font-weight: bold;
  text-transform: uppercase;
  white-space: nowrap;
  margin-left: 0.5rem;
}

.difficulty-1 {
  background: #d4edda;
  color: #155724;
}

.difficulty-2 {
  background: #fff3cd;
  color: #856404;
}

.difficulty-3 {
  background: #f8d7da;
  color: #721c24;
}

.game-actions {
  display: flex;
  justify-content: center;
  margin-top: 2rem;
}

.submit-button {
  padding: 1rem 2rem;
  background: linear-gradient(135deg, #27ae60 0%, #2ecc71 100%);
  color: white;
  border: none;
  border-radius: 0.5rem;
  font-size: 1.1rem;
  font-weight: bold;
  cursor: pointer;
  transition: all 0.3s;
}

.submit-button:hover:not(:disabled) {
  transform: translateY(-2px);
  box-shadow: 0 4px 12px rgba(39, 174, 96, 0.3);
}

.submit-button:disabled {
  opacity: 0.6;
  cursor: not-allowed;
}

.error-message {
  padding: 1rem;
  background: #fee;
  color: #c33;
  border-radius: 0.5rem;
  text-align: center;
  margin-top: 1rem;
}

@media (max-width: 768px) {
  .main-content {
    flex-direction: column;
    gap: 1.5rem;
  }
  
  .text-section,
  .fallacies-sidebar {
    flex: none;
    min-width: unset;
    max-width: none;
  }
  
  .fallacies-list {
    max-height: 400px;
  }
  
  .game-header {
    flex-direction: column;
    gap: 1rem;
    text-align: center;
  }

  .game-info {
    justify-content: center;
  }
}

@media (max-width: 480px) {
  .page-header h2 {
    font-size: 1.5rem;
  }
  
  .game-text {
    padding: 1rem;
    font-size: 1rem;
  }
  
  .fallacy-card {
    padding: 0.75rem;
  }
  
  .fallacies-list {
    max-height: 300px;
  }
}
</style>
