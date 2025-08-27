<template>
  <div class="start-page">
    <div class="card">
      <h2>Welcome to the Critical Thinking Game!</h2>
      <p class="description">
        Test your ability to identify logical fallacies in argumentative texts. 
        Choose your difficulty level and start training your critical thinking skills.
      </p>

      <form @submit.prevent="handleStartGame" class="start-form">
        <div class="form-group">
          <label for="playerName">Your Name:</label>
          <input
            id="playerName"
            v-model="playerName"
            type="text"
            required
            maxlength="100"
            placeholder="Enter your name"
            class="form-input"
          />
        </div>

        <div class="form-group">
          <label>Difficulty Level:</label>
          <div class="difficulty-options">
            <label class="difficulty-option">
              <input
                v-model="selectedDifficulty"
                type="radio"
                :value="Difficulty.Easy"
                name="difficulty"
              />
              <span class="difficulty-label">
                <strong>Easy</strong>
                <small>3 fallacies from basic set</small>
              </span>
            </label>

            <label class="difficulty-option">
              <input
                v-model="selectedDifficulty"
                type="radio"
                :value="Difficulty.Medium"
                name="difficulty"
              />
              <span class="difficulty-label">
                <strong>Medium</strong>
                <small>6 fallacies from easy & medium sets</small>
              </span>
            </label>

            <label class="difficulty-option">
              <input
                v-model="selectedDifficulty"
                type="radio"
                :value="Difficulty.Hard"
                name="difficulty"
              />
              <span class="difficulty-label">
                <strong>Hard</strong>
                <small>9 fallacies from all sets</small>
              </span>
            </label>
          </div>
        </div>

        <button type="submit" :disabled="isLoading" class="start-button">
          <span v-if="isLoading">Starting Game...</span>
          <span v-else>Start Game</span>
        </button>
      </form>

      <div v-if="error" class="error-message">
        {{ error }}
      </div>
    </div>
  </div>
</template>

<script setup lang="ts">
import { ref } from 'vue'
import { useRouter } from 'vue-router'
import { useGameStore } from '@/stores/game'
import { Difficulty } from '@/services/api'

const router = useRouter()
const gameStore = useGameStore()

const playerName = ref('')
const selectedDifficulty = ref(Difficulty.Easy)

const { isLoading, error } = gameStore

async function handleStartGame() {
  await gameStore.startGame(playerName.value, selectedDifficulty.value)
  if (!gameStore.error) {
    router.push('/game')
  }
}
</script>

<style scoped>
.start-page {
  display: flex;
  justify-content: center;
  align-items: center;
  min-height: 60vh;
}

.card {
  background: rgba(255, 255, 255, 0.95);
  color: #333;
  padding: 2rem;
  border-radius: 1rem;
  box-shadow: 0 8px 32px rgba(0, 0, 0, 0.1);
  max-width: 500px;
  width: 100%;
}

h2 {
  text-align: center;
  margin-bottom: 1rem;
  color: #2c3e50;
}

.description {
  text-align: center;
  margin-bottom: 2rem;
  color: #666;
  line-height: 1.6;
}

.start-form {
  display: flex;
  flex-direction: column;
  gap: 1.5rem;
}

.form-group {
  display: flex;
  flex-direction: column;
  gap: 0.5rem;
}

label {
  font-weight: bold;
  color: #2c3e50;
}

.form-input {
  padding: 0.75rem;
  border: 2px solid #e1e8ed;
  border-radius: 0.5rem;
  font-size: 1rem;
  transition: border-color 0.3s;
}

.form-input:focus {
  outline: none;
  border-color: #667eea;
}

.difficulty-options {
  display: flex;
  flex-direction: column;
  gap: 0.75rem;
}

.difficulty-option {
  display: flex;
  align-items: center;
  gap: 0.75rem;
  padding: 0.75rem;
  border: 2px solid #e1e8ed;
  border-radius: 0.5rem;
  cursor: pointer;
  transition: all 0.3s;
}

.difficulty-option:hover {
  border-color: #667eea;
  background: rgba(102, 126, 234, 0.05);
}

.difficulty-option input[type="radio"] {
  margin: 0;
}

.difficulty-label {
  display: flex;
  flex-direction: column;
  gap: 0.25rem;
}

.difficulty-label strong {
  color: #2c3e50;
}

.difficulty-label small {
  color: #666;
  font-size: 0.9rem;
}

.start-button {
  padding: 1rem 2rem;
  background: linear-gradient(135deg, #667eea 0%, #764ba2 100%);
  color: white;
  border: none;
  border-radius: 0.5rem;
  font-size: 1.1rem;
  font-weight: bold;
  cursor: pointer;
  transition: transform 0.2s;
}

.start-button:hover:not(:disabled) {
  transform: translateY(-2px);
}

.start-button:disabled {
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
</style>
