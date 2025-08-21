import axios from 'axios'
import type { 
  GameStartRequest, 
  GameStartResponse, 
  GameSubmitRequest, 
  GameSubmitResponse,
  FallacyOption,
  Difficulty 
} from '@/types/game'

const api = axios.create({
  baseURL: '/api',
  headers: {
    'Content-Type': 'application/json'
  }
})

export const gameApi = {
  async startGame(request: GameStartRequest): Promise<GameStartResponse> {
    const backendRequest = {
      playerName: request.playerName,
      difficulty: request.difficulty
    }
    const response = await api.post<GameStartResponse>('/game/start', backendRequest)
    return response.data
  },

  async submitGame(request: GameSubmitRequest): Promise<GameSubmitResponse> {
    const backendRequest = {
      sessionId: request.sessionId,
      selectedFallacyIds: request.selectedFallacyIds,
      completedAt: request.completedAt
    }
    const response = await api.post<GameSubmitResponse>('/game/submit', backendRequest)
    return response.data
  },

  async getFallaciesByDifficulty(difficulty: Difficulty): Promise<FallacyOption[]> {
    const response = await api.get<FallacyOption[]>(`/game/fallacies/${difficulty}`)
    return response.data
  }
}