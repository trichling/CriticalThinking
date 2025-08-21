import { describe, it, expect } from 'vitest'
import { mount } from '@vue/test-utils'
import GameResults from '../GameResults.vue'
import type { GameSubmitResponse } from '@/types/game'

describe('GameResults', () => {
  const mockResults: GameSubmitResponse = {
    score: 250,
    timeTakenSeconds: 180, // 3 minutes
    results: [
      {
        fallacyId: 1,
        fallacyName: 'Ad Hominem',
        fallacyKey: 'ad-hominem',
        resultType: 'correct',
        textReference: '...attacking the person rather than the argument...',
        position: 45
      },
      {
        fallacyId: 2,
        fallacyName: 'Strawman',
        fallacyKey: 'strawman',
        resultType: 'wrong'
      },
      {
        fallacyId: 3,
        fallacyName: 'Appeal to Authority',
        fallacyKey: 'appeal-to-authority',
        resultType: 'missed',
        textReference: '...Dr. Smith says this is true...',
        position: 120
      }
    ],
    stats: {
      correctCount: 1,
      wrongCount: 1,
      missedCount: 1,
      totalFallacies: 2,
      accuracy: 0.5
    }
  }

  it('renders results header correctly', () => {
    const wrapper = mount(GameResults, {
      props: { results: mockResults }
    })

    expect(wrapper.find('h2').text()).toBe('Game Complete!')
    expect(wrapper.find('.score').text()).toBe('Score: 250 points')
  })

  it('displays stats summary correctly', () => {
    const wrapper = mount(GameResults, {
      props: { results: mockResults }
    })

    const statCards = wrapper.findAll('.stat-card')
    expect(statCards).toHaveLength(5)

    expect(statCards[0].find('.stat-number').text()).toBe('1')
    expect(statCards[0].find('.stat-label').text()).toBe('Correct')

    expect(statCards[1].find('.stat-number').text()).toBe('1')
    expect(statCards[1].find('.stat-label').text()).toBe('Wrong')

    expect(statCards[2].find('.stat-number').text()).toBe('1')
    expect(statCards[2].find('.stat-label').text()).toBe('Missed')

    expect(statCards[3].find('.stat-number').text()).toBe('50%') // 0.5 * 100
    expect(statCards[3].find('.stat-label').text()).toBe('Accuracy')

    expect(statCards[4].find('.stat-number').text()).toBe('3:00') // 180 seconds
    expect(statCards[4].find('.stat-label').text()).toBe('Time')
  })

  it('displays correct answers section', () => {
    const wrapper = mount(GameResults, {
      props: { results: mockResults }
    })

    const correctSection = wrapper.find('.section-title.correct')
    expect(correctSection.text()).toBe('✓ Correctly Identified')

    const correctItems = wrapper.findAll('.result-item.correct')
    expect(correctItems).toHaveLength(1)
    expect(correctItems[0].find('.fallacy-name').text()).toBe('Ad Hominem')
    expect(correctItems[0].find('.text-reference').text()).toBe('...attacking the person rather than the argument...')
  })

  it('displays wrong answers section', () => {
    const wrapper = mount(GameResults, {
      props: { results: mockResults }
    })

    const wrongSection = wrapper.find('.section-title.wrong')
    expect(wrongSection.text()).toBe('✗ Incorrectly Selected')

    const wrongItems = wrapper.findAll('.result-item.wrong')
    expect(wrongItems).toHaveLength(1)
    expect(wrongItems[0].find('.fallacy-name').text()).toBe('Strawman')
    expect(wrongItems[0].find('.explanation').text()).toBe('This fallacy was not present in the text.')
  })

  it('displays missed answers section', () => {
    const wrapper = mount(GameResults, {
      props: { results: mockResults }
    })

    const missedSection = wrapper.find('.section-title.missed')
    expect(missedSection.text()).toBe('⚠ Missed Fallacies')

    const missedItems = wrapper.findAll('.result-item.missed')
    expect(missedItems).toHaveLength(1)
    expect(missedItems[0].find('.fallacy-name').text()).toBe('Appeal to Authority')
    expect(missedItems[0].find('.text-reference').text()).toBe('...Dr. Smith says this is true...')
  })

  it('emits playAgain event when button is clicked', async () => {
    const wrapper = mount(GameResults, {
      props: { results: mockResults }
    })

    await wrapper.find('.play-again-button').trigger('click')

    expect(wrapper.emitted('playAgain')).toHaveLength(1)
  })

  it('formats time correctly', () => {
    const wrapper = mount(GameResults, {
      props: { results: mockResults }
    })

    // Access the formatTime method through component instance
    const component = wrapper.vm as any

    expect(component.formatTime(0)).toBe('0:00')
    expect(component.formatTime(59)).toBe('0:59')
    expect(component.formatTime(60)).toBe('1:00')
    expect(component.formatTime(125)).toBe('2:05')
    expect(component.formatTime(180)).toBe('3:00')
  })

  it('handles results with no correct answers', () => {
    const noCorrectResults: GameSubmitResponse = {
      ...mockResults,
      results: [
        {
          fallacyId: 1,
          fallacyName: 'Ad Hominem',
          fallacyKey: 'ad-hominem',
          resultType: 'wrong'
        }
      ],
      stats: {
        correctCount: 0,
        wrongCount: 1,
        missedCount: 0,
        totalFallacies: 1,
        accuracy: 0
      }
    }

    const wrapper = mount(GameResults, {
      props: { results: noCorrectResults }
    })

    expect(wrapper.find('.section-title.correct').exists()).toBe(false)
    expect(wrapper.find('.section-title.wrong').exists()).toBe(true)
  })

  it('handles results with no wrong answers', () => {
    const noWrongResults: GameSubmitResponse = {
      ...mockResults,
      results: [
        {
          fallacyId: 1,
          fallacyName: 'Ad Hominem',
          fallacyKey: 'ad-hominem',
          resultType: 'correct',
          textReference: '...some text...'
        }
      ],
      stats: {
        correctCount: 1,
        wrongCount: 0,
        missedCount: 0,
        totalFallacies: 1,
        accuracy: 1.0
      }
    }

    const wrapper = mount(GameResults, {
      props: { results: noWrongResults }
    })

    expect(wrapper.find('.section-title.correct').exists()).toBe(true)
    expect(wrapper.find('.section-title.wrong').exists()).toBe(false)
    expect(wrapper.find('.section-title.missed').exists()).toBe(false)
  })

  it('handles perfect score display', () => {
    const perfectResults: GameSubmitResponse = {
      score: 500,
      timeTakenSeconds: 60,
      results: [
        {
          fallacyId: 1,
          fallacyName: 'Ad Hominem',
          fallacyKey: 'ad-hominem',
          resultType: 'correct',
          textReference: '...some text...'
        }
      ],
      stats: {
        correctCount: 1,
        wrongCount: 0,
        missedCount: 0,
        totalFallacies: 1,
        accuracy: 1.0
      }
    }

    const wrapper = mount(GameResults, {
      props: { results: perfectResults }
    })

    expect(wrapper.find('.score').text()).toBe('Score: 500 points')
    const accuracyStat = wrapper.findAll('.stat-card')[3]
    expect(accuracyStat.find('.stat-number').text()).toBe('100%')
  })
})