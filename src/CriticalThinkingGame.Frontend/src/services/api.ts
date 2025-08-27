// Types
export interface LogicalFallacy {
  id: number
  name: string
  description: string
  difficulty: Difficulty
}

export interface GameText {
  id: number
  content: string
  difficulty: Difficulty
  fallacies: TextFallacy[]
}

export interface TextFallacy {
  id: number
  logicalFallacyId: number
  fallacyName: string
  startIndex: number
  endIndex: number
}

export interface StartGameRequest {
  playerName: string
  difficulty: Difficulty
  languageCode: string
}

export interface StartGameResponse {
  sessionId: number
  gameText: GameText
  availableFallacies: LogicalFallacy[]
}

export interface SubmitAnswerRequest {
  sessionId: number
  selectedFallacyIds: number[]
  languageCode: string
}

export interface SubmitAnswerResponse {
  score: number
  timeTakenSeconds: number
  results: FallacyResult[]
}

export interface FallacyResult {
  fallacyId: number
  fallacyName: string
  startIndex: number
  endIndex: number
  resultType: FallacyResultType
}

export enum Difficulty {
  Easy = 1,
  Medium = 2,
  Hard = 3
}

export enum FallacyResultType {
  Correct = 0,
  Missed = 1,
  Incorrect = 2
}

// API Service
class ApiService {
  private readonly baseUrl = import.meta.env.VITE_API_URL || 'http://localhost:5000'

  async startGame(request: StartGameRequest): Promise<StartGameResponse> {
    const response = await fetch(`${this.baseUrl}/api/game/start`, {
      method: 'POST',
      headers: {
        'Content-Type': 'application/json',
      },
      body: JSON.stringify(request),
    })

    if (!response.ok) {
      throw new Error(`Failed to start game: ${response.statusText}`)
    }

    return response.json()
  }

  async submitAnswer(request: SubmitAnswerRequest): Promise<SubmitAnswerResponse> {
    const response = await fetch(`${this.baseUrl}/api/game/submit`, {
      method: 'POST',
      headers: {
        'Content-Type': 'application/json',
      },
      body: JSON.stringify(request),
    })

    if (!response.ok) {
      throw new Error(`Failed to submit answer: ${response.statusText}`)
    }

    return response.json()
  }

  async getLogicalFallacies(): Promise<LogicalFallacy[]> {
    const response = await fetch(`${this.baseUrl}/api/game/fallacies`)

    if (!response.ok) {
      throw new Error(`Failed to fetch fallacies: ${response.statusText}`)
    }

    return response.json()
  }
}

export const apiService = new ApiService()
