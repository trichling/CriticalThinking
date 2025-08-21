<template>
  <div class="game-setup">
    <h1>Critical Thinking Training</h1>
    <p class="description">
      Train your critical thinking skills by identifying logical fallacies in text passages.
    </p>

    <form @submit.prevent="handleSubmit" class="setup-form">
      <div class="form-group">
        <label for="playerName">Your Name:</label>
        <input
          id="playerName"
          v-model="playerName"
          type="text"
          required
          minlength="1"
          maxlength="100"
          placeholder="Enter your name"
          class="form-input"
        >
      </div>

      <div class="form-group">
        <label>Difficulty Level:</label>
        <div class="difficulty-options">
          <label class="difficulty-option">
            <input
              v-model="selectedDifficulty"
              type="radio"
              name="difficulty"
              :value="Difficulty.Easy"
            >
            <div class="difficulty-card">
              <h3>Easy</h3>
              <p>3 common fallacies</p>
              <small>Perfect for beginners</small>
            </div>
          </label>

          <label class="difficulty-option">
            <input
              v-model="selectedDifficulty"
              type="radio"
              name="difficulty"
              :value="Difficulty.Medium"
            >
            <div class="difficulty-card">
              <h3>Medium</h3>
              <p>6 varied fallacies</p>
              <small>For intermediate learners</small>
            </div>
          </label>

          <label class="difficulty-option">
            <input
              v-model="selectedDifficulty"
              type="radio"
              name="difficulty"
              :value="Difficulty.Hard"
            >
            <div class="difficulty-card">
              <h3>Hard</h3>
              <p>9 complex fallacies</p>
              <small>Challenge for experts</small>
            </div>
          </label>
        </div>
      </div>

      <button type="submit" :disabled="!isFormValid || loading" class="start-button">
        {{ loading ? 'Starting...' : 'Start Game' }}
      </button>
    </form>
  </div>
</template>

<script setup lang="ts">
import { ref, computed } from 'vue'
import { Difficulty } from '@/types/game'
import { useGameStore } from '@/stores/gameStore'

const emit = defineEmits<{
  gameStarted: []
}>()

const gameStore = useGameStore()

const playerName = ref('')
const selectedDifficulty = ref<Difficulty | null>(null)
const loading = ref(false)

const isFormValid = computed(() => 
  playerName.value.trim().length > 0 && selectedDifficulty.value !== null
)

async function handleSubmit() {
  if (!isFormValid.value) return

  loading.value = true
  try {
    await gameStore.startGame(playerName.value.trim(), selectedDifficulty.value!)
    emit('gameStarted')
  } catch (error) {
    console.error('Failed to start game:', error)
    alert('Failed to start the game. Please try again.')
  } finally {
    loading.value = false
  }
}
</script>

<style scoped>
.game-setup {
  max-width: 600px;
  margin: 0 auto;
  padding: 2rem;
  text-align: center;
}

.description {
  font-size: 1.1rem;
  color: #666;
  margin-bottom: 2rem;
}

.setup-form {
  display: flex;
  flex-direction: column;
  gap: 2rem;
}

.form-group {
  text-align: left;
}

.form-group label {
  display: block;
  font-weight: bold;
  margin-bottom: 0.5rem;
}

.form-input {
  width: 100%;
  padding: 0.75rem;
  border: 2px solid #ddd;
  border-radius: 4px;
  font-size: 1rem;
}

.form-input:focus {
  outline: none;
  border-color: #007bff;
}

.difficulty-options {
  display: grid;
  grid-template-columns: repeat(auto-fit, minmax(150px, 1fr));
  gap: 1rem;
  margin-top: 0.5rem;
}

.difficulty-option {
  cursor: pointer;
}

.difficulty-option input[type="radio"] {
  display: none;
}

.difficulty-card {
  padding: 1rem;
  border: 2px solid #ddd;
  border-radius: 8px;
  transition: all 0.2s;
  text-align: center;
}

.difficulty-option input:checked + .difficulty-card {
  border-color: #007bff;
  background-color: #f0f8ff;
}

.difficulty-card:hover {
  border-color: #007bff;
  transform: translateY(-2px);
}

.difficulty-card h3 {
  margin: 0 0 0.5rem 0;
  color: #333;
}

.difficulty-card p {
  margin: 0 0 0.25rem 0;
  font-weight: 500;
}

.difficulty-card small {
  color: #666;
}

.start-button {
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

.start-button:hover:not(:disabled) {
  background-color: #0056b3;
}

.start-button:disabled {
  background-color: #ccc;
  cursor: not-allowed;
}
</style>