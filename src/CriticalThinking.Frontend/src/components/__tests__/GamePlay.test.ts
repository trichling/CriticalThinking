import { describe, it, expect, beforeEach, vi } from 'vitest'
import { mount } from '@vue/test-utils'
import { createPinia, setActivePinia } from 'pinia'
import GamePlay from '../GamePlay.vue'
import { useGameStore } from '@/stores/gameStore'
import { Difficulty } from '@/types/game'

// Mock the game store
vi.mock('@/stores/gameStore')
const mockUseGameStore = vi.mocked(useGameStore)

describe('GamePlay', () => {
  let mockGameStore: any
  
  beforeEach(() => {
    setActivePinia(createPinia())
    
    mockGameStore = {
      state: {
        playerName: 'Test Player',
        difficulty: Difficulty.Easy,
        gameText: 'This is a test game text with logical fallacies.',
        availableFallacies: [
          { id: 1, name: 'Ad Hominem', key: 'ad-hominem', description: 'Attacking the person making the argument' },
          { id: 2, name: 'Strawman', key: 'strawman', description: 'Misrepresenting someone\'s argument' }
        ],
        selectedFallacies: [],
        startTime: new Date('2023-01-01T00:00:00Z')
      },
      toggleFallacy: vi.fn(),
      submitGame: vi.fn()
    }
    mockUseGameStore.mockReturnValue(mockGameStore)
  })

  it('renders game information correctly', () => {
    const wrapper = mount(GamePlay)
    
    expect(wrapper.find('h2').text()).toBe('Test Player')
    expect(wrapper.find('.difficulty').text()).toBe('Easy Mode')
    expect(wrapper.find('.game-text').text()).toBe('This is a test game text with logical fallacies.')
  })

  it('displays available fallacies correctly', () => {
    const wrapper = mount(GamePlay)
    
    const fallacyCards = wrapper.findAll('.fallacy-card')
    expect(fallacyCards).toHaveLength(2)
    
    expect(fallacyCards[0].find('h4').text()).toBe('Ad Hominem')
    expect(fallacyCards[0].find('.fallacy-description').text()).toBe('Attacking the person making the argument')
    
    expect(fallacyCards[1].find('h4').text()).toBe('Strawman')
    expect(fallacyCards[1].find('.fallacy-description').text()).toBe('Misrepresenting someone\'s argument')
  })

  it('handles fallacy selection', async () => {
    const wrapper = mount(GamePlay)
    
    const checkboxes = wrapper.findAll('input[type="checkbox"]')
    
    await checkboxes[0].trigger('change')()
    expect(mockGameStore.toggleFallacy).toHaveBeenCalledWith(1)
    
    await checkboxes[1].trigger('change')()
    expect(mockGameStore.toggleFallacy).toHaveBeenCalledWith(2)
  })

  it('shows selected fallacies visually', async () => {
    mockGameStore.state.selectedFallacies = [1]
    
    const wrapper = mount(GamePlay)
    
    const fallacyOptions = wrapper.findAll('.fallacy-option')
    expect(fallacyOptions[0].classes()).toContain('selected')
    expect(fallacyOptions[1].classes()).not.toContain('selected')
  })

  it('updates timer correctly', async () => {
    vi.useFakeTimers()
    vi.setSystemTime(new Date('2023-01-01T00:01:30Z')) // 1 minute 30 seconds after start
    
    const wrapper = mount(GamePlay)
    
    expect(wrapper.find('.timer').text()).toBe('Time: 1:30')
    
    vi.useRealTimers()
  })

  it('updates selection count correctly', () => {
    mockGameStore.state.selectedFallacies = [1, 2]
    
    const wrapper = mount(GamePlay)
    
    expect(wrapper.find('.selection-count').text()).toBe('Selected: 2')
  })

  it('disables submit button when no fallacies selected', () => {
    mockGameStore.state.selectedFallacies = []
    
    const wrapper = mount(GamePlay)
    
    expect(wrapper.find('.submit-button').attributes('disabled')).toBeDefined()
  })

  it('enables submit button when fallacies are selected', () => {
    mockGameStore.state.selectedFallacies = [1]
    
    const wrapper = mount(GamePlay)
    
    expect(wrapper.find('.submit-button').attributes('disabled')).toBeUndefined()
  })

  it('handles game submission successfully', async () => {
    mockGameStore.state.selectedFallacies = [1, 2]
    mockGameStore.submitGame.mockResolvedValue({})
    
    const wrapper = mount(GamePlay)
    
    await wrapper.find('.submit-button').trigger('click')
    
    expect(mockGameStore.submitGame).toHaveBeenCalled()
    expect(wrapper.emitted('gameCompleted')).toHaveLength(1)
  })

  it('handles game submission error', async () => {
    mockGameStore.state.selectedFallacies = [1]
    mockGameStore.submitGame.mockRejectedValue(new Error('Submission failed'))
    
    // Mock window.alert
    const mockAlert = vi.fn()
    vi.stubGlobal('alert', mockAlert)
    
    const wrapper = mount(GamePlay)
    
    await wrapper.find('.submit-button').trigger('click')
    
    expect(mockGameStore.submitGame).toHaveBeenCalled()
    expect(mockAlert).toHaveBeenCalledWith('Failed to submit your answer. Please try again.')
    expect(wrapper.emitted('gameCompleted')).toBeUndefined()
    
    vi.unstubAllGlobals()
  })

  it('prevents submission with empty selection and shows alert', async () => {
    mockGameStore.state.selectedFallacies = []
    
    // Mock window.alert
    const mockAlert = vi.fn()
    vi.stubGlobal('alert', mockAlert)
    
    const wrapper = mount(GamePlay)
    
    // Force click the button (even though it should be disabled)
    await wrapper.find('.submit-button').trigger('click')
    
    expect(mockAlert).toHaveBeenCalledWith('Please select at least one fallacy before submitting.')
    expect(mockGameStore.submitGame).not.toHaveBeenCalled()
    
    vi.unstubAllGlobals()
  })

  it('shows loading state during submission', async () => {
    mockGameStore.state.selectedFallacies = [1]
    
    let resolvePromise: () => void
    const promise = new Promise<void>((resolve) => {
      resolvePromise = resolve
    })
    mockGameStore.submitGame.mockReturnValue(promise)
    
    const wrapper = mount(GamePlay)
    
    // Start submission
    wrapper.find('.submit-button').trigger('click')
    await wrapper.vm.$nextTick()
    
    expect(wrapper.find('.submit-button').text()).toBe('Submitting...')
    expect(wrapper.find('.submit-button').attributes('disabled')).toBeDefined()
    
    // Resolve the promise
    resolvePromise!()
    await promise
    await wrapper.vm.$nextTick()
    
    expect(wrapper.find('.submit-button').text()).toBe('Submit Answer')
  })

  it('formats time correctly', () => {
    const wrapper = mount(GamePlay)
    
    // Access the formatTime method through component instance
    const component = wrapper.vm as any
    
    expect(component.formatTime(0)).toBe('0:00')
    expect(component.formatTime(59)).toBe('0:59')
    expect(component.formatTime(60)).toBe('1:00')
    expect(component.formatTime(125)).toBe('2:05')
    expect(component.formatTime(3661)).toBe('61:01')
  })
})