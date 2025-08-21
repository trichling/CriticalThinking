import { describe, it, expect, beforeEach, vi } from 'vitest'
import { mount } from '@vue/test-utils'
import { createPinia, setActivePinia } from 'pinia'
import GameSetup from '../GameSetup.vue'
import { useGameStore } from '@/stores/gameStore'
import { Difficulty } from '@/types/game'

// Mock the game store
vi.mock('@/stores/gameStore')
const mockUseGameStore = vi.mocked(useGameStore)

describe('GameSetup', () => {
  let mockGameStore: any
  
  beforeEach(() => {
    setActivePinia(createPinia())
    
    mockGameStore = {
      startGame: vi.fn()
    }
    mockUseGameStore.mockReturnValue(mockGameStore)
  })

  it('renders correctly', () => {
    const wrapper = mount(GameSetup)
    
    expect(wrapper.find('h1').text()).toBe('Critical Thinking Training')
    expect(wrapper.find('.description').text()).toContain('Train your critical thinking skills')
    expect(wrapper.find('input[type="text"]').exists()).toBe(true)
    expect(wrapper.findAll('input[type="radio"]')).toHaveLength(3)
  })

  it('validates form correctly', async () => {
    const wrapper = mount(GameSetup)
    
    // Initially, form should be invalid
    expect(wrapper.find('.start-button').attributes('disabled')).toBeDefined()
    
    // Enter player name
    await wrapper.find('input[type="text"]').setValue('Test Player')
    expect(wrapper.find('.start-button').attributes('disabled')).toBeDefined()
    
    // Select difficulty
    const easyRadio = wrapper.find('input[value="Easy"]')
    await easyRadio.trigger('change')
    expect(wrapper.find('.start-button').attributes('disabled')).toBeUndefined()
  })

  it('displays difficulty options correctly', () => {
    const wrapper = mount(GameSetup)
    
    const difficultyCards = wrapper.findAll('.difficulty-card')
    expect(difficultyCards).toHaveLength(3)
    
    expect(difficultyCards[0].find('h3').text()).toBe('Easy')
    expect(difficultyCards[0].text()).toContain('3 common fallacies')
    
    expect(difficultyCards[1].find('h3').text()).toBe('Medium')
    expect(difficultyCards[1].text()).toContain('6 varied fallacies')
    
    expect(difficultyCards[2].find('h3').text()).toBe('Hard')
    expect(difficultyCards[2].text()).toContain('9 complex fallacies')
  })

  it('handles form submission successfully', async () => {
    mockGameStore.startGame.mockResolvedValue({})
    
    const wrapper = mount(GameSetup)
    
    await wrapper.find('input[type="text"]').setValue('Test Player')
    await wrapper.find('input[value="Medium"]').trigger('change')
    
    await wrapper.find('form').trigger('submit.prevent')
    
    expect(mockGameStore.startGame).toHaveBeenCalledWith('Test Player', Difficulty.Medium)
    expect(wrapper.emitted('gameStarted')).toHaveLength(1)
  })

  it('handles form submission error', async () => {
    mockGameStore.startGame.mockRejectedValue(new Error('API Error'))
    
    // Mock window.alert
    const mockAlert = vi.fn()
    vi.stubGlobal('alert', mockAlert)
    
    const wrapper = mount(GameSetup)
    
    await wrapper.find('input[type="text"]').setValue('Test Player')
    await wrapper.find('input[value="Easy"]').trigger('change')
    
    await wrapper.find('form').trigger('submit.prevent')
    
    expect(mockGameStore.startGame).toHaveBeenCalled()
    expect(mockAlert).toHaveBeenCalledWith('Failed to start the game. Please try again.')
    expect(wrapper.emitted('gameStarted')).toBeUndefined()
    
    vi.unstubAllGlobals()
  })

  it('shows loading state during submission', async () => {
    let resolvePromise: () => void
    const promise = new Promise<void>((resolve) => {
      resolvePromise = resolve
    })
    mockGameStore.startGame.mockReturnValue(promise)
    
    const wrapper = mount(GameSetup)
    
    await wrapper.find('input[type="text"]').setValue('Test Player')
    await wrapper.find('input[value="Easy"]').trigger('change')
    
    // Start submission
    wrapper.find('form').trigger('submit.prevent')
    await wrapper.vm.$nextTick()
    
    expect(wrapper.find('.start-button').text()).toBe('Starting...')
    expect(wrapper.find('.start-button').attributes('disabled')).toBeDefined()
    
    // Resolve the promise
    resolvePromise!()
    await promise
    await wrapper.vm.$nextTick()
    
    expect(wrapper.find('.start-button').text()).toBe('Start Game')
  })

  it('prevents submission with empty player name', async () => {
    const wrapper = mount(GameSetup)
    
    await wrapper.find('input[value="Easy"]').trigger('change')
    
    // Button should still be disabled with empty name
    expect(wrapper.find('.start-button').attributes('disabled')).toBeDefined()
    
    // Set empty string (trimmed)
    await wrapper.find('input[type="text"]').setValue('   ')
    expect(wrapper.find('.start-button').attributes('disabled')).toBeDefined()
  })

  it('updates difficulty selection correctly', async () => {
    const wrapper = mount(GameSetup)
    
    // Select Easy
    await wrapper.find('input[value="Easy"]').trigger('change')
    expect(wrapper.find('input[value="Easy"]').element.checked).toBe(true)
    
    // Switch to Hard
    await wrapper.find('input[value="Hard"]').trigger('change')
    expect(wrapper.find('input[value="Hard"]').element.checked).toBe(true)
    expect(wrapper.find('input[value="Easy"]').element.checked).toBe(false)
  })
})