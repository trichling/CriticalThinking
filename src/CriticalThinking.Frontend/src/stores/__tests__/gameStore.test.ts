import { setActivePinia, createPinia } from 'pinia'
import { describe, it, expect, beforeEach, vi } from 'vitest'
import { useGameStore } from '../gameStore'
import { gameApi } from '@/services/gameApi'
import { Difficulty } from '@/types/game'

// Mock the gameApi
vi.mock('@/services/gameApi')
const mockGameApi = vi.mocked(gameApi)

describe('GameStore', () => {
  beforeEach(() => {
    setActivePinia(createPinia())
    vi.clearAllMocks()
  })

  it('should initialize with correct default state', () => {
    const store = useGameStore()
    
    expect(store.state.isPlaying).toBe(false)
    expect(store.state.sessionId).toBeNull()
    expect(store.state.playerName).toBe('')
    expect(store.state.difficulty).toBeNull()
    expect(store.state.gameText).toBe('')
    expect(store.state.availableFallacies).toEqual([])
    expect(store.state.selectedFallacies).toEqual([])
    expect(store.state.startTime).toBeNull()
    expect(store.state.endTime).toBeNull()
    expect(store.state.results).toBeNull()
  })

  it('should start game successfully', async () => {
    const store = useGameStore()
    const mockResponse = {
      sessionId: 'test-session-id',
      text: 'Test game text',
      availableFallacies: [
        { id: 1, name: 'Ad Hominem', key: 'ad-hominem', description: 'Test description' }
      ],
      startedAt: '2023-01-01T00:00:00Z'
    }

    mockGameApi.startGame.mockResolvedValue(mockResponse)

    await store.startGame('Test Player', Difficulty.Easy)

    expect(store.state.isPlaying).toBe(true)
    expect(store.state.sessionId).toBe('test-session-id')
    expect(store.state.playerName).toBe('Test Player')
    expect(store.state.difficulty).toBe(Difficulty.Easy)
    expect(store.state.gameText).toBe('Test game text')
    expect(store.state.availableFallacies).toHaveLength(1)
    expect(store.state.selectedFallacies).toEqual([])
    expect(store.state.startTime).toBeInstanceOf(Date)
  })

  it('should handle startGame failure', async () => {
    const store = useGameStore()
    const error = new Error('API Error')
    
    mockGameApi.startGame.mockRejectedValue(error)

    await expect(store.startGame('Test Player', Difficulty.Easy)).rejects.toThrow('API Error')
    expect(store.state.isPlaying).toBe(false)
  })

  it('should submit game successfully', async () => {
    const store = useGameStore()
    
    // Set up initial state as if game was started
    store.state.sessionId = 'test-session-id'
    store.state.startTime = new Date('2023-01-01T00:00:00Z')
    store.state.selectedFallacies = [1, 2]

    const mockResponse = {
      score: 150,
      timeTakenSeconds: 120,
      results: [
        { fallacyId: 1, fallacyName: 'Ad Hominem', fallacyKey: 'ad-hominem', resultType: 'correct' as const }
      ],
      stats: {
        correctCount: 1,
        wrongCount: 0,
        missedCount: 1,
        totalFallacies: 2,
        accuracy: 0.5
      }
    }

    mockGameApi.submitGame.mockResolvedValue(mockResponse)

    const result = await store.submitGame()

    expect(store.state.isPlaying).toBe(false)
    expect(store.state.results).toEqual(mockResponse)
    expect(store.state.endTime).toBeInstanceOf(Date)
    expect(result).toEqual(mockResponse)
  })

  it('should handle submitGame without active session', async () => {
    const store = useGameStore()
    
    await expect(store.submitGame()).rejects.toThrow('No active game session')
  })

  it('should toggle fallacy selection correctly', () => {
    const store = useGameStore()
    
    // Select fallacy
    store.toggleFallacy(1)
    expect(store.state.selectedFallacies).toEqual([1])
    
    // Select another fallacy
    store.toggleFallacy(2)
    expect(store.state.selectedFallacies).toEqual([1, 2])
    
    // Deselect first fallacy
    store.toggleFallacy(1)
    expect(store.state.selectedFallacies).toEqual([2])
    
    // Deselect last fallacy
    store.toggleFallacy(2)
    expect(store.state.selectedFallacies).toEqual([])
  })

  it('should calculate time taken correctly', () => {
    const store = useGameStore()
    
    expect(store.timeTaken).toBe(0)
    
    const startTime = new Date('2023-01-01T00:00:00Z')
    const endTime = new Date('2023-01-01T00:02:30Z') // 2 minutes 30 seconds later
    
    store.state.startTime = startTime
    store.state.endTime = endTime
    
    expect(store.timeTaken).toBe(150) // 150 seconds
  })

  it('should check if game is active correctly', () => {
    const store = useGameStore()
    
    expect(store.isGameActive).toBe(false)
    
    store.state.isPlaying = true
    expect(store.isGameActive).toBe(false) // Still false because no sessionId
    
    store.state.sessionId = 'test-session'
    expect(store.isGameActive).toBe(true)
    
    store.state.isPlaying = false
    expect(store.isGameActive).toBe(false)
  })

  it('should reset game state correctly', () => {
    const store = useGameStore()
    
    // Set some state
    store.state.isPlaying = true
    store.state.sessionId = 'test-session'
    store.state.playerName = 'Test Player'
    store.state.selectedFallacies = [1, 2]
    
    store.resetGame()
    
    expect(store.state.isPlaying).toBe(false)
    expect(store.state.sessionId).toBeNull()
    expect(store.state.playerName).toBe('')
    expect(store.state.selectedFallacies).toEqual([])
  })
})