import { describe, it, expect, beforeEach, vi } from 'vitest'
import { apiService, Difficulty } from '../api'

// Mock fetch
global.fetch = vi.fn()

describe('API Service', () => {
  beforeEach(() => {
    vi.resetAllMocks()
  })

  describe('startGame', () => {
    it('should make a POST request to start game endpoint', async () => {
      const mockResponse = {
        sessionId: 1,
        gameText: {
          id: 1,
          content: 'Test content',
          difficulty: Difficulty.Easy,
          fallacies: []
        },
        availableFallacies: []
      }

      ;(fetch as any).mockResolvedValueOnce({
        ok: true,
        json: async () => mockResponse
      })

      const request = {
        playerName: 'Test Player',
        difficulty: Difficulty.Easy
      }

      const result = await apiService.startGame(request)

      expect(fetch).toHaveBeenCalledWith(
        'http://localhost:5000/api/game/start',
        {
          method: 'POST',
          headers: {
            'Content-Type': 'application/json'
          },
          body: JSON.stringify(request)
        }
      )
      expect(result).toEqual(mockResponse)
    })

    it('should throw error when response is not ok', async () => {
      ;(fetch as any).mockResolvedValueOnce({
        ok: false,
        statusText: 'Bad Request'
      })

      const request = {
        playerName: 'Test Player',
        difficulty: Difficulty.Easy
      }

      await expect(apiService.startGame(request)).rejects.toThrow(
        'Failed to start game: Bad Request'
      )
    })
  })

  describe('submitAnswer', () => {
    it('should make a POST request to submit answer endpoint', async () => {
      const mockResponse = {
        score: 10,
        timeTakenSeconds: 60,
        results: []
      }

      ;(fetch as any).mockResolvedValueOnce({
        ok: true,
        json: async () => mockResponse
      })

      const request = {
        sessionId: 1,
        selectedFallacyIds: [1, 2]
      }

      const result = await apiService.submitAnswer(request)

      expect(fetch).toHaveBeenCalledWith(
        'http://localhost:5000/api/game/submit',
        {
          method: 'POST',
          headers: {
            'Content-Type': 'application/json'
          },
          body: JSON.stringify(request)
        }
      )
      expect(result).toEqual(mockResponse)
    })
  })

  describe('getLogicalFallacies', () => {
    it('should make a GET request to fallacies endpoint', async () => {
      const mockResponse = [
        {
          id: 1,
          name: 'Ad Hominem',
          description: 'Attacking the person',
          difficulty: Difficulty.Easy
        }
      ]

      ;(fetch as any).mockResolvedValueOnce({
        ok: true,
        json: async () => mockResponse
      })

      const result = await apiService.getLogicalFallacies()

      expect(fetch).toHaveBeenCalledWith('http://localhost:5000/api/game/fallacies')
      expect(result).toEqual(mockResponse)
    })
  })
})
