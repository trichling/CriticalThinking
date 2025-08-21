<template>
  <div class="game-results">
    <div class="results-header">
      <h2>Game Complete!</h2>
      <div class="score">
        Score: {{ props.results.score }} points
      </div>
    </div>

    <div class="stats-summary">
      <div class="stat-card">
        <div class="stat-number">{{ props.results.stats.correctCount }}</div>
        <div class="stat-label">Correct</div>
      </div>
      <div class="stat-card">
        <div class="stat-number">{{ props.results.stats.wrongCount }}</div>
        <div class="stat-label">Wrong</div>
      </div>
      <div class="stat-card">
        <div class="stat-number">{{ props.results.stats.missedCount }}</div>
        <div class="stat-label">Missed</div>
      </div>
      <div class="stat-card">
        <div class="stat-number">{{ Math.round(props.results.stats.accuracy * 100) }}%</div>
        <div class="stat-label">Accuracy</div>
      </div>
      <div class="stat-card">
        <div class="stat-number">{{ formatTime(props.results.timeTakenSeconds) }}</div>
        <div class="stat-label">Time</div>
      </div>
    </div>

    <div class="results-details">
      <h3>Detailed Results</h3>
      
      <div v-if="correctResults.length > 0" class="result-section">
        <h4 class="section-title correct">✓ Correctly Identified</h4>
        <div class="result-items">
          <div
            v-for="result in correctResults"
            :key="result.fallacyId"
            class="result-item correct"
          >
            <div class="fallacy-name">{{ result.fallacyName }}</div>
            <div v-if="result.textReference" class="text-reference">
              {{ result.textReference }}
            </div>
          </div>
        </div>
      </div>

      <div v-if="wrongResults.length > 0" class="result-section">
        <h4 class="section-title wrong">✗ Incorrectly Selected</h4>
        <div class="result-items">
          <div
            v-for="result in wrongResults"
            :key="result.fallacyId"
            class="result-item wrong"
          >
            <div class="fallacy-name">{{ result.fallacyName }}</div>
            <div class="explanation">This fallacy was not present in the text.</div>
          </div>
        </div>
      </div>

      <div v-if="missedResults.length > 0" class="result-section">
        <h4 class="section-title missed">⚠ Missed Fallacies</h4>
        <div class="result-items">
          <div
            v-for="result in missedResults"
            :key="result.fallacyId"
            class="result-item missed"
          >
            <div class="fallacy-name">{{ result.fallacyName }}</div>
            <div v-if="result.textReference" class="text-reference">
              {{ result.textReference }}
            </div>
          </div>
        </div>
      </div>
    </div>

    <div class="actions">
      <button @click="$emit('playAgain')" class="play-again-button">
        Play Again
      </button>
    </div>
  </div>
</template>

<script setup lang="ts">
import { computed } from 'vue'
import type { GameSubmitResponse } from '@/types/game'

const props = defineProps<{
  results: GameSubmitResponse
}>()

defineEmits<{
  playAgain: []
}>()

const correctResults = computed(() => 
  props.results.results.filter(r => r.resultType === 'correct')
)

const wrongResults = computed(() => 
  props.results.results.filter(r => r.resultType === 'wrong')
)

const missedResults = computed(() => 
  props.results.results.filter(r => r.resultType === 'missed')
)

const formatTime = (seconds: number) => {
  const minutes = Math.floor(seconds / 60)
  const remainingSeconds = seconds % 60
  return `${minutes}:${remainingSeconds.toString().padStart(2, '0')}`
}
</script>

<style scoped>
.game-results {
  max-width: 800px;
  margin: 0 auto;
  padding: 2rem;
}

.results-header {
  text-align: center;
  margin-bottom: 2rem;
}

.results-header h2 {
  margin: 0 0 1rem 0;
  color: #333;
}

.score {
  font-size: 2rem;
  font-weight: bold;
  color: #007bff;
}

.stats-summary {
  display: grid;
  grid-template-columns: repeat(auto-fit, minmax(120px, 1fr));
  gap: 1rem;
  margin-bottom: 2rem;
}

.stat-card {
  text-align: center;
  padding: 1rem;
  background-color: #f8f9fa;
  border-radius: 8px;
  border: 1px solid #ddd;
}

.stat-number {
  font-size: 1.5rem;
  font-weight: bold;
  color: #333;
}

.stat-label {
  font-size: 0.9rem;
  color: #666;
  margin-top: 0.25rem;
}

.results-details {
  margin-bottom: 2rem;
}

.results-details h3 {
  margin: 0 0 1rem 0;
  color: #333;
}

.result-section {
  margin-bottom: 1.5rem;
}

.section-title {
  margin: 0 0 0.5rem 0;
  padding: 0.5rem;
  border-radius: 4px;
  font-weight: bold;
}

.section-title.correct {
  background-color: #d4edda;
  color: #155724;
}

.section-title.wrong {
  background-color: #f8d7da;
  color: #721c24;
}

.section-title.missed {
  background-color: #fff3cd;
  color: #856404;
}

.result-items {
  display: grid;
  gap: 0.5rem;
}

.result-item {
  padding: 1rem;
  border-radius: 4px;
  border-left: 4px solid;
}

.result-item.correct {
  background-color: #f8fff9;
  border-left-color: #28a745;
}

.result-item.wrong {
  background-color: #fff8f8;
  border-left-color: #dc3545;
}

.result-item.missed {
  background-color: #fffef8;
  border-left-color: #ffc107;
}

.fallacy-name {
  font-weight: bold;
  margin-bottom: 0.25rem;
}

.text-reference, .explanation {
  font-size: 0.9rem;
  color: #666;
  font-style: italic;
}

.actions {
  text-align: center;
}

.play-again-button {
  padding: 1rem 2rem;
  background-color: #007bff;
  color: white;
  border: none;
  border-radius: 4px;
  font-size: 1.1rem;
  font-weight: bold;
  cursor: pointer;
  transition: background-color 0.2s;
}

.play-again-button:hover {
  background-color: #0056b3;
}
</style>