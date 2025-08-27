import { defineStore } from 'pinia'
import { ref, computed } from 'vue'
import type { 
  StartGameResponse, 
  SubmitAnswerResponse
} from '@/services/api'
import { apiService, Difficulty } from '@/services/api'
import { getCurrentLanguage } from '@/locales'

export const useGameStore = defineStore('game', () => {
  // State
  const currentGame = ref<StartGameResponse | null>(null)
  const gameResult = ref<SubmitAnswerResponse | null>(null)
  const isLoading = ref(false)
  const error = ref<string | null>(null)
  const playerName = ref('')
  const selectedFallacies = ref<number[]>([])
  const startTime = ref<Date | null>(null)

  // Computed
  const isGameActive = computed(() => currentGame.value !== null && gameResult.value === null)
  const gameText = computed(() => currentGame.value?.gameText?.content || '')
  const availableFallacies = computed(() => currentGame.value?.availableFallacies || [])

  // Actions
  async function startGame(name: string, difficulty: Difficulty) {
    isLoading.value = true
    error.value = null
    
    try {
      playerName.value = name
      currentGame.value = await apiService.startGame({
        playerName: name,
        difficulty,
        languageCode: getCurrentLanguage()
      })
      selectedFallacies.value = []
      gameResult.value = null
      startTime.value = new Date()
    } catch (err) {
      error.value = err instanceof Error ? err.message : 'Failed to start game'
    } finally {
      isLoading.value = false
    }
  }

  async function submitAnswer() {
    if (!currentGame.value) return

    isLoading.value = true
    error.value = null

    try {
      gameResult.value = await apiService.submitAnswer({
        sessionId: currentGame.value.sessionId,
        selectedFallacyIds: selectedFallacies.value,
        languageCode: getCurrentLanguage()
      })
    } catch (err) {
      error.value = err instanceof Error ? err.message : 'Failed to submit answer'
    } finally {
      isLoading.value = false
    }
  }

  function toggleFallacy(fallacyId: number) {
    const index = selectedFallacies.value.indexOf(fallacyId)
    if (index > -1) {
      selectedFallacies.value.splice(index, 1)
    } else {
      selectedFallacies.value.push(fallacyId)
    }
  }

  function resetGame() {
    currentGame.value = null
    gameResult.value = null
    selectedFallacies.value = []
    startTime.value = null
    error.value = null
  }

  function getDifficultyName(difficulty: Difficulty): string {
    switch (difficulty) {
      case Difficulty.Easy:
        return 'Easy'
      case Difficulty.Medium:
        return 'Medium'
      case Difficulty.Hard:
        return 'Hard'
      default:
        return 'Unknown'
    }
  }

  return {
    // State
    currentGame,
    gameResult,
    isLoading,
    error,
    playerName,
    selectedFallacies,
    startTime,
    
    // Computed
    isGameActive,
    gameText,
    availableFallacies,
    
    // Actions
    startGame,
    submitAnswer,
    toggleFallacy,
    resetGame,
    getDifficultyName
  }
})
