<template>
  <div id="app">
    <main class="main-content">
      <GameSetup
        v-if="currentView === 'setup'"
        @game-started="currentView = 'playing'"
      />
      
      <GamePlay
        v-else-if="currentView === 'playing'"
        @game-completed="handleGameCompleted"
      />
      
      <GameResults
        v-else-if="currentView === 'results' && gameStore.state.results"
        :results="gameStore.state.results"
        @play-again="handlePlayAgain"
      />
      
      <div v-else-if="currentView === 'results' && !gameStore.state.results">
        <p>Loading results...</p>
      </div>
    </main>
  </div>
</template>

<script setup lang="ts">
import { ref } from 'vue'
import GameSetup from '@/components/GameSetup.vue'
import GamePlay from '@/components/GamePlay.vue'
import GameResults from '@/components/GameResults.vue'
import { useGameStore } from '@/stores/gameStore'

const gameStore = useGameStore()
const currentView = ref<'setup' | 'playing' | 'results'>('setup')

function handleGameCompleted() {
  currentView.value = 'results'
}

function handlePlayAgain() {
  gameStore.resetGame()
  currentView.value = 'setup'
}
</script>

<style>
* {
  margin: 0;
  padding: 0;
  box-sizing: border-box;
}

body {
  font-family: -apple-system, BlinkMacSystemFont, 'Segoe UI', 'Roboto', 'Oxygen',
    'Ubuntu', 'Cantarell', 'Fira Sans', 'Droid Sans', 'Helvetica Neue',
    sans-serif;
  -webkit-font-smoothing: antialiased;
  -moz-osx-font-smoothing: grayscale;
  background-color: #f5f5f5;
  color: #333;
}

#app {
  min-height: 100vh;
  display: flex;
  flex-direction: column;
}

.main-content {
  flex: 1;
  padding: 2rem 1rem;
}

h1, h2, h3, h4 {
  color: #333;
}

button {
  transition: all 0.2s ease;
}

button:focus {
  outline: 2px solid #007bff;
  outline-offset: 2px;
}

.sr-only {
  position: absolute;
  width: 1px;
  height: 1px;
  padding: 0;
  margin: -1px;
  overflow: hidden;
  clip: rect(0, 0, 0, 0);
  white-space: nowrap;
  border: 0;
}
</style>