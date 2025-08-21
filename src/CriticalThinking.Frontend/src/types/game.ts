export enum Difficulty {
  Easy = 'Easy',
  Medium = 'Medium',
  Hard = 'Hard'
}

export interface FallacyOption {
  id: number
  name: string
  key: string
  description: string
}

export interface GameStartRequest {
  playerName: string
  difficulty: Difficulty
}

export interface GameStartResponse {
  sessionId: string
  text: string
  availableFallacies: FallacyOption[]
  startedAt: string
}

export interface GameSubmitRequest {
  sessionId: string
  selectedFallacyIds: number[]
  completedAt: string
}

export interface FallacyResult {
  fallacyId: number
  fallacyName: string
  fallacyKey: string
  resultType: 'correct' | 'wrong' | 'missed'
  textReference?: string
  position?: number
}

export interface GameStats {
  correctCount: number
  wrongCount: number
  missedCount: number
  totalFallacies: number
  accuracy: number
}

export interface GameSubmitResponse {
  score: number
  timeTakenSeconds: number
  results: FallacyResult[]
  stats: GameStats
}

export interface GameState {
  isPlaying: boolean
  sessionId: string | null
  playerName: string
  difficulty: Difficulty | null
  gameText: string
  availableFallacies: FallacyOption[]
  selectedFallacies: number[]
  startTime: Date | null
  endTime: Date | null
  results: GameSubmitResponse | null
}