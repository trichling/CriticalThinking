import { describe, it, expect, beforeEach, vi } from 'vitest'
import axios from 'axios'
import { gameApi } from '../gameApi'
import { Difficulty } from '@/types/game'

// Mock axios
vi.mock('axios')
const mockAxios = vi.mocked(axios)

describe('GameApi', () => {
  const mockCreate = vi.fn()
  
  beforeEach(() => {
    vi.clearAllMocks()
    mockAxios.create = mockCreate.mockReturnValue({
      post: vi.fn(),
      get: vi.fn()
    } as any)
  })

  it('should create axios instance with correct config', () => {
    expect(mockAxios.create).toHaveBeenCalledWith({
      baseURL: '/api',
      headers: {
        'Content-Type': 'application/json'
      }
    })
  })

  it('should start game successfully', async () => {
    const mockApi = {
      post: vi.fn().mockResolvedValue({
        data: {
          sessionId: 'test-session',
          text: 'Test text',
          availableFallacies: [],
          startedAt: '2023-01-01T00:00:00Z'
        }
      })
    }
    mockCreate.mockReturnValue(mockApi as any)

    const request = {
      playerName: 'Test Player',
      difficulty: Difficulty.Easy
    }

    const result = await gameApi.startGame(request)

    expect(mockApi.post).toHaveBeenCalledWith('/game/start', request)
    expect(result.sessionId).toBe('test-session')
    expect(result.text).toBe('Test text')
  })

  it('should submit game successfully', async () => {
    const mockApi = {
      post: vi.fn().mockResolvedValue({
        data: {
          score: 150,
          timeTakenSeconds: 120,
          results: [],
          stats: {
            correctCount: 1,
            wrongCount: 0,
            missedCount: 0,
            totalFallacies: 1,
            accuracy: 1.0
          }
        }
      })
    }
    mockCreate.mockReturnValue(mockApi as any)

    const request = {
      sessionId: 'test-session',
      selectedFallacyIds: [1, 2],
      completedAt: '2023-01-01T00:02:00Z'
    }

    const result = await gameApi.submitGame(request)

    expect(mockApi.post).toHaveBeenCalledWith('/game/submit', request)
    expect(result.score).toBe(150)
    expect(result.timeTakenSeconds).toBe(120)
  })

  it('should get fallacies by difficulty successfully', async () => {
    const mockApi = {
      get: vi.fn().mockResolvedValue({
        data: [
          { id: 1, name: 'Ad Hominem', key: 'ad-hominem', description: 'Test description' },
          { id: 2, name: 'Strawman', key: 'strawman', description: 'Test description 2' }
        ]
      })
    }
    mockCreate.mockReturnValue(mockApi as any)

    const result = await gameApi.getFallaciesByDifficulty(Difficulty.Easy)

    expect(mockApi.get).toHaveBeenCalledWith('/game/fallacies/Easy')
    expect(result).toHaveLength(2)
    expect(result[0].name).toBe('Ad Hominem')
  })

  it('should handle API errors', async () => {
    const mockApi = {
      post: vi.fn().mockRejectedValue(new Error('Network error'))
    }
    mockCreate.mockReturnValue(mockApi as any)

    const request = {
      playerName: 'Test Player',
      difficulty: Difficulty.Easy
    }

    await expect(gameApi.startGame(request)).rejects.toThrow('Network error')
  })
})