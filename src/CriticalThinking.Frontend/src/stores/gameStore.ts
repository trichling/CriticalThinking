import { defineStore } from 'pinia'
import { ref, computed } from 'vue'
import type { GameState, Difficulty, FallacyOption, GameSubmitResponse } from '@/types/game'
import { gameApi } from '@/services/gameApi'

export const useGameStore = defineStore('game', () => {
  const state = ref<GameState>({
    isPlaying: false,
    sessionId: null,
    playerName: '',
    difficulty: null,
    gameText: '',
    availableFallacies: [],
    selectedFallacies: [],
    startTime: null,
    endTime: null,
    results: null
  })

  const isGameActive = computed(() => state.value.isPlaying && state.value.sessionId)
  const timeTaken = computed(() => {
    if (!state.value.startTime || !state.value.endTime) return 0
    return Math.floor((state.value.endTime.getTime() - state.value.startTime.getTime()) / 1000)
  })

  async function startGame(playerName: string, difficulty: Difficulty) {
    try {
      state.value.playerName = playerName
      state.value.difficulty = difficulty
      
      const response = await gameApi.startGame({ playerName, difficulty })
      
      state.value.sessionId = response.sessionId
      state.value.gameText = response.text
      state.value.availableFallacies = response.availableFallacies
      state.value.selectedFallacies = []
      state.value.startTime = new Date(response.startedAt)
      state.value.endTime = null
      state.value.isPlaying = true
      state.value.results = null
      
      return response
    } catch (error) {
      console.error('Failed to start game:', error)
      throw error
    }
  }

  async function submitGame() {
    if (!state.value.sessionId || !state.value.startTime) {
      throw new Error('No active game session')
    }

    try {
      state.value.endTime = new Date()
      
      const response = await gameApi.submitGame({
        sessionId: state.value.sessionId,
        selectedFallacyIds: state.value.selectedFallacies,
        completedAt: state.value.endTime.toISOString()
      })
      
      state.value.results = response
      state.value.isPlaying = false
      
      return response
    } catch (error) {
      console.error('Failed to submit game:', error)
      throw error
    }
  }

  function toggleFallacy(fallacyId: number) {
    const index = state.value.selectedFallacies.indexOf(fallacyId)
    if (index === -1) {
      state.value.selectedFallacies.push(fallacyId)
    } else {
      state.value.selectedFallacies.splice(index, 1)
    }
  }

  function resetGame() {
    state.value = {
      isPlaying: false,
      sessionId: null,
      playerName: '',
      difficulty: null,
      gameText: '',
      availableFallacies: [],
      selectedFallacies: [],
      startTime: null,
      endTime: null,
      results: null
    }
  }

  return {
    state,
    isGameActive,
    timeTaken,
    startGame,
    submitGame,
    toggleFallacy,
    resetGame
  }
})