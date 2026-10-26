import { describe, it, expect } from 'vitest'
import { mount } from '@vue/test-utils'
import StatusTimeline from '../../components/StatusTimeline.vue'

const entries = [
  { id: 1, oldStatus: null, newStatus: 'new', changedByName: 'أحمد محمد', changedAt: '2026-09-01T10:00:00.000Z' },
  { id: 2, oldStatus: 'new', newStatus: 'shortlisted', changedByName: null, changedAt: '2026-09-02T10:00:00.000Z' },
]

describe('StatusTimeline meta layout', () => {
  it('renders name above the date in separate blocks', () => {
    const wrapper = mount(StatusTimeline as any, { props: { history: entries } })
    const metas = wrapper.findAll('.timeline-meta')
    expect(metas).toHaveLength(2)
    // name and date are stacked divs, not one inline row
    expect(metas[0].find('.timeline-meta-name').exists()).toBe(true)
    expect(metas[0].find('.timeline-meta-name').text()).toContain('أحمد محمد')
    expect(metas[0].find('.timeline-meta-date').exists()).toBe(true)
  })

  it('renders date only when there is no actor name', () => {
    const wrapper = mount(StatusTimeline as any, { props: { history: entries } })
    const metas = wrapper.findAll('.timeline-meta')
    expect(metas[1].find('.timeline-meta-name').exists()).toBe(false)
    expect(metas[1].find('.timeline-meta-date').text().length).toBeGreaterThan(0)
  })

  it('shows the empty state when history is empty', () => {
    const wrapper = mount(StatusTimeline as any, { props: { history: [] } })
    expect(wrapper.text()).toContain('لا يوجد سجل حالات بعد')
  })
})
