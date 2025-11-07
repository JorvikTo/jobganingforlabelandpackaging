import { createRouter, createWebHistory } from 'vue-router'
import Home from './views/Home.vue'
import DieLines from './views/DieLines.vue'
import Sheets from './views/Sheets.vue'
import Ganging from './views/Ganging.vue'

const routes = [
  { path: '/', component: Home },
  { path: '/die-lines', component: DieLines },
  { path: '/sheets', component: Sheets },
  { path: '/ganging', component: Ganging }
]

const router = createRouter({
  history: createWebHistory(),
  routes
})

export default router
