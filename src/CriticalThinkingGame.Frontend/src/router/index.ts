import { createRouter, createWebHistory } from 'vue-router'
import StartView from '../views/StartView.vue'

const router = createRouter({
  history: createWebHistory(import.meta.env.BASE_URL),
  routes: [
    {
      path: '/',
      name: 'start',
      component: StartView,
    },
    {
      path: '/game',
      name: 'game',
      component: () => import('../views/GameView.vue'),
    },
    {
      path: '/results',
      name: 'results',
      component: () => import('../views/ResultsView.vue'),
    },
  ],
})

export default router
