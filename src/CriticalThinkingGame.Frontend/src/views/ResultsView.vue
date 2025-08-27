<template>
  <div class="results-page">
    <div v-if="!gameStore.gameResult" class="loading">
      <p>Loading results...</p>
    </div>

    <div v-else class="results-container">
      <!-- Game Results Header -->
      <div class="results-header">
        <h2>Game Results</h2>
        <div class="score-display">
          <div class="score-circle">
            <span class="score-value">{{ gameStore.gameResult.score }}</span>
            <span class="score-label">points</span>
          </div>
        </div>
      </div>

      <div class="summary-stats">
        <div class="stat-card">
          <h3>Time Taken</h3>
          <p class="stat-value">{{ formatTime(gameStore.gameResult.timeTakenSeconds) }}</p>
        </div>
        <div class="stat-card">
          <h3>Correct</h3>
          <p class="stat-value correct">{{ correctCount }}</p>
        </div>
        <div class="stat-card">
          <h3>Missed</h3>
          <p class="stat-value missed">{{ missedCount }}</p>
        </div>
        <div class="stat-card">
          <h3>Incorrect</h3>
          <p class="stat-value incorrect">{{ incorrectCount }}</p>
        </div>
      </div>

      <!-- Main Content Area -->
      <div class="main-content">
        <!-- Text Analysis Section -->
        <div class="text-section">
          <div class="game-text-review" v-if="gameStore.currentGame">
            <h3>Text Analysis</h3>
            <div class="text-with-highlights">
              <div class="review-text">
                <HighlightedText 
                  :text="gameStore.gameText" 
                  :fallacies="gameStore.gameResult?.results || []"
                  :hoveredFallacyId="hoveredFallacyId"
                />
              </div>
            </div>
          </div>
        </div>

        <!-- Fallacy Results Sidebar -->
        <div class="fallacy-sidebar">
          <div class="results-details">
            <h3>Detailed Results</h3>
            <div class="fallacy-results">
              <div
                v-for="result in gameStore.gameResult.results"
                :key="result.fallacyId"
                class="result-item"
                :class="getResultClass(result.resultType)"
                @mouseenter="highlightFallacy(result.fallacyId)"
                @mouseleave="removeHighlight()"
              >
                <div class="result-icon">
                  <span v-if="result.resultType === FallacyResultType.Correct">✓</span>
                  <span v-else-if="result.resultType === FallacyResultType.Missed">○</span>
                  <span v-else>✗</span>
                </div>
                <div class="result-content">
                  <h4>{{ result.fallacyName }}</h4>
                </div>
              </div>
            </div>
          </div>
        </div>
      </div>

      <div class="action-buttons">
        <button @click="playAgain" class="play-again-button">
          Play Again
        </button>
        <button @click="goHome" class="home-button">
          Back to Home
        </button>
      </div>
    </div>
  </div>
</template>

<script setup lang="ts">
import { computed, ref } from 'vue'
import { useRouter } from 'vue-router'
import { useGameStore } from '@/stores/game'
import { FallacyResultType } from '@/services/api'
import HighlightedText from '@/components/HighlightedText.vue'

const router = useRouter()
const gameStore = useGameStore()

// Reactive state for hover highlighting
const hoveredFallacyId = ref<number | null>(null)

// Computed properties
const correctCount = computed(() => 
  gameStore.gameResult?.results.filter(r => r.resultType === FallacyResultType.Correct).length || 0
)

const missedCount = computed(() => 
  gameStore.gameResult?.results.filter(r => r.resultType === FallacyResultType.Missed).length || 0
)

const incorrectCount = computed(() => 
  gameStore.gameResult?.results.filter(r => r.resultType === FallacyResultType.Incorrect).length || 0
)

const highlightedText = computed(() => {
  if (!gameStore.currentGame || !gameStore.gameResult) {
    return gameStore.gameText
  }

  const text = gameStore.gameText
  const correctFallacies = gameStore.gameResult.results.filter(
    r => r.resultType === FallacyResultType.Correct && r.startIndex > 0
  )

  // Sort by start index descending to avoid index shifting when inserting highlights
  correctFallacies.sort((a, b) => b.startIndex - a.startIndex)

  let highlightedText = text
  for (const fallacy of correctFallacies) {
    const before = highlightedText.substring(0, fallacy.startIndex)
    const highlighted = highlightedText.substring(fallacy.startIndex, fallacy.endIndex)
    const after = highlightedText.substring(fallacy.endIndex)
    highlightedText = `${before}[${highlighted}]${after}`
  }

  return highlightedText
})

// Methods
function highlightFallacy(fallacyId: number) {
  console.log('Highlighting fallacy:', fallacyId)
  hoveredFallacyId.value = fallacyId
}

function removeHighlight() {
  console.log('Removing highlight')
  hoveredFallacyId.value = null
}

function formatTime(seconds: number): string {
  const minutes = Math.floor(seconds / 60)
  const remainingSeconds = seconds % 60
  return `${minutes}:${remainingSeconds.toString().padStart(2, '0')}`
}

function getResultClass(resultType: FallacyResultType): string {
  switch (resultType) {
    case FallacyResultType.Correct:
      return 'result-correct'
    case FallacyResultType.Missed:
      return 'result-missed'
    case FallacyResultType.Incorrect:
      return 'result-incorrect'
    default:
      return ''
  }
}

function playAgain() {
  gameStore.resetGame()
  router.push('/')
}

function goHome() {
  gameStore.resetGame()
  router.push('/')
}

// Redirect if no game result
if (!gameStore.gameResult) {
  router.push('/')
}
</script>

<style scoped>
.results-page {
  max-width: 1400px;
  margin: 0 auto;
  padding: 1rem;
}

.loading {
  text-align: center;
  padding: 2rem;
  font-size: 1.2rem;
}

.results-container {
  background: rgba(255, 255, 255, 0.95);
  color: #333;
  border-radius: 1rem;
  padding: 2rem;
  box-shadow: 0 8px 32px rgba(0, 0, 0, 0.1);
}

.results-header {
  text-align: center;
  margin-bottom: 1rem;
}

.results-header h2 {
  font-size: 1.5rem;
  margin-bottom: 0.5rem;
}

.main-content {
  display: flex;
  gap: 2rem;
  margin-bottom: 2rem;
}

.text-section {
  flex: 2;
  min-width: 0; /* Allows flex item to shrink below its content size */
}

.fallacy-sidebar {
  flex: 1;
  min-width: 300px;
  max-width: 400px;
}

.fallacy-results {
  max-height: 600px;
  overflow-y: auto;
  padding-right: 0.5rem;
}

/* Scrollbar styling */
.fallacy-results::-webkit-scrollbar {
  width: 8px;
}

.fallacy-results::-webkit-scrollbar-track {
  background: #f1f1f1;
  border-radius: 4px;
}

.fallacy-results::-webkit-scrollbar-thumb {
  background: #c1c1c1;
  border-radius: 4px;
}

.fallacy-results::-webkit-scrollbar-thumb:hover {
  background: #a8a8a8;
}

.score-display {
  display: flex;
  justify-content: center;
}

.score-circle {
  display: flex;
  flex-direction: column;
  align-items: center;
  justify-content: center;
  width: 80px;
  height: 80px;
  border-radius: 50%;
  background: linear-gradient(135deg, #667eea 0%, #764ba2 100%);
  color: white;
  font-weight: bold;
}

.score-value {
  font-size: 1.5rem;
  line-height: 1;
}

.score-label {
  font-size: 0.7rem;
  opacity: 0.9;
}

.summary-stats {
  display: grid;
  grid-template-columns: repeat(4, 1fr);
  gap: 1rem;
  margin-bottom: 1rem;
}

.stat-card {
  background: white;
  padding: 1rem;
  border-radius: 0.5rem;
  text-align: center;
  min-width: 0; /* Allows content to shrink */
  box-shadow: 0 2px 8px rgba(0, 0, 0, 0.1);
}

.stat-card h3 {
  margin: 0 0 0.5rem 0;
  color: #2c3e50;
  font-size: 1rem;
}

.stat-value {
  font-size: 1.8rem;
  font-weight: bold;
  margin: 0;
}

.stat-value.correct {
  color: #27ae60;
}

.stat-value.missed {
  color: #f39c12;
}

.stat-value.incorrect {
  color: #e74c3c;
}

.results-details h3,
.game-text-review h3 {
  margin-bottom: 1rem;
  color: #2c3e50;
}

.result-item {
  display: flex;
  align-items: flex-start;
  gap: 0.75rem;
  padding: 0.75rem;
  border-radius: 0.5rem;
  border-left: 4px solid;
  transition: all 0.3s ease;
  cursor: pointer;
}

.result-item:hover {
  transform: translateY(-2px);
  box-shadow: 0 4px 12px rgba(0, 0, 0, 0.15);
}

.result-correct {
  background: #e8f5e8;
  border-color: #27ae60;
}

.result-missed {
  background: #fff8e1;
  border-color: #f39c12;
}

.result-incorrect {
  background: #fee;
  border-color: #e74c3c;
}

.result-icon {
  display: flex;
  align-items: center;
  justify-content: center;
  width: 24px;
  height: 24px;
  border-radius: 50%;
  font-weight: bold;
  font-size: 1rem;
}

.result-correct .result-icon {
  background: #27ae60;
  color: white;
}

.result-missed .result-icon {
  background: #f39c12;
  color: white;
}

.result-incorrect .result-icon {
  background: #e74c3c;
  color: white;
}

.result-content h4 {
  margin: 0 0 0.25rem 0;
  color: #2c3e50;
}

.result-status {
  margin: 0 0 0.25rem 0;
  font-weight: bold;
}

.result-location {
  margin: 0;
  font-size: 0.9rem;
  color: #666;
}

.text-with-highlights {
  background: #f8f9fa;
  padding: 1.5rem;
  border-radius: 0.5rem;
  border-left: 4px solid #667eea;
  margin-bottom: 2rem;
  border: 1px solid #e9ecef;
}

.game-text-review h2 {
  color: #2c3e50;
  margin-bottom: 1rem;
  text-align: center;
}

.review-text {
  line-height: 1.8;
  font-size: 1.1rem;
  white-space: pre-wrap;
  margin: 0;
  color: #333;
}

.action-buttons {
  display: flex;
  gap: 1rem;
  justify-content: center;
}

.play-again-button,
.home-button {
  padding: 1rem 2rem;
  border: none;
  border-radius: 0.5rem;
  font-size: 1.1rem;
  font-weight: bold;
  cursor: pointer;
  transition: transform 0.2s;
}

.play-again-button {
  background: linear-gradient(135deg, #667eea 0%, #764ba2 100%);
  color: white;
}

.home-button {
  background: #6c757d;
  color: white;
}

.play-again-button:hover,
.home-button:hover {
  transform: translateY(-2px);
}

@media (max-width: 1024px) {
  .main-content {
    flex-direction: column;
  }
  
  .fallacy-sidebar {
    min-width: auto;
    max-width: none;
  }
  
  .fallacy-results {
    max-height: 400px;
  }
}

@media (max-width: 768px) {
  .summary-stats {
    grid-template-columns: repeat(2, 1fr);
    gap: 0.5rem;
  }

  .stat-card {
    padding: 0.75rem;
  }

  .stat-card h3 {
    font-size: 0.9rem;
  }

  .action-buttons {
    flex-direction: column;
  }
  
  .results-page {
    padding: 0.5rem;
  }
  
  .results-container {
    padding: 1rem;
  }
}

@media (max-width: 480px) {
  .summary-stats {
    grid-template-columns: repeat(2, 1fr);
    gap: 0.25rem;
  }

  .stat-card {
    padding: 0.5rem;
  }

  .stat-card h3 {
    font-size: 0.8rem;
    margin-bottom: 0.25rem;
  }

  .stat-value {
    font-size: 1.2rem;
  }
}

/* Fallacy highlighting styles */
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
