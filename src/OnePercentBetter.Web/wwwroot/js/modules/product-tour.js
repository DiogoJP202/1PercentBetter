const TOUR_QUERY_KEY = 'tour'
const TOUR_ENABLED_VALUE = '1'
const STEP_QUERY_KEY = 'step'
const REPLAY_QUERY_KEY = 'replay'

const TOUR_STEPS = [
  {
    route: '/Onboarding/Tour',
    anchor: 'tour-what-is',
    title: 'O que é o 1% Better',
    description: 'Aqui você constrói evolução diária. A ideia é melhorar um pouco por vez, sem depender de motivação extrema.'
  },
  {
    route: '/Onboarding/Tour',
    anchor: 'tour-systems',
    title: 'Sistemas primeiro, metas depois',
    description: 'Meta mostra direção. Sistema define o que você faz hoje para chegar lá com constância.'
  },
  {
    route: '/Identities',
    anchor: 'identities-overview',
    title: 'Identidades',
    description: 'Identidade define quem você quer se tornar. Ela guia suas decisões quando o dia fica difícil.'
  },
  {
    route: '/Goals',
    anchor: 'goals-overview',
    title: 'Objetivos',
    description: 'Objetivos traduzem a identidade em uma direção concreta. São o destino do seu sistema.'
  },
  {
    route: '/Habits',
    anchor: 'habits-good',
    title: 'Hábitos bons',
    description: 'Hábitos bons são a menor ação repetível que fortalece sua identidade todos os dias.'
  },
  {
    route: '/Habits',
    anchor: 'habits-bad',
    title: 'Hábitos ruins',
    description: 'Hábitos ruins também entram no sistema: você identifica gatilhos e reduz repetição com ajustes práticos.'
  },
  {
    route: '/CheckIns',
    query: { period: 'month' },
    anchor: 'checkins-overview',
    title: 'Check-in diário',
    description: 'O check-in registra humor, energia e produtividade para você enxergar padrões reais.'
  },
  {
    route: '/Notes',
    anchor: 'notes-overview',
    title: 'Anotações',
    description: 'Anotações guardam aprendizados, vitórias e dificuldades para você não perder sinais importantes.'
  },
  {
    route: '/Dashboard',
    anchor: 'dashboard-progress',
    title: 'Dashboard e gráficos',
    description: 'No dashboard, os dados viram clareza: consistência, ritmo e pontos de ajuste do seu sistema.'
  },
  {
    route: '/Calendar',
    anchor: 'calendar-overview',
    title: 'Calendário de consistência',
    description: 'O calendário mostra continuidade no tempo e ajuda a visualizar se você está sustentando o processo.'
  },
  {
    route: '/Notes',
    anchor: 'notes-reviews',
    title: 'Revisões semanais e mensais',
    description: 'Revisar semanalmente e mensalmente evita piloto automático e melhora seu sistema com base em dados.'
  },
  {
    route: '/Onboarding/Tour',
    anchor: 'tour-connection',
    title: 'Como tudo se conecta',
    description: 'Identidade -> Objetivo -> Hábito -> Check-in -> Dashboard -> Ajuste. Agora você monta seu primeiro sistema.'
  }
]

let tourOverlay = null
let tourPanel = null
let titleElement = null
let descriptionElement = null
let progressElement = null
let backButton = null
let nextButton = null
let skipButton = null
let highlightedAnchor = null
let currentStepIndex = -1
let replayMode = false

function normalizePath(pathname) {
  if (!pathname) {
    return '/'
  }

  return pathname.endsWith('/') && pathname !== '/'
    ? pathname.slice(0, -1)
    : pathname
}

function getTourState() {
  const url = new URL(window.location.href)
  const params = url.searchParams
  const isTourActive = params.get(TOUR_QUERY_KEY) === TOUR_ENABLED_VALUE
  replayMode = ['1', 'true', 'True', 'TRUE'].includes(params.get(REPLAY_QUERY_KEY) ?? '')

  if (!isTourActive) {
    return null
  }

  const rawStep = params.get(STEP_QUERY_KEY)
  const parsedStep = rawStep ? Number.parseInt(rawStep, 10) : 1

  if (!Number.isInteger(parsedStep) || parsedStep < 1 || parsedStep > TOUR_STEPS.length) {
    return null
  }

  return {
    stepNumber: parsedStep,
    stepIndex: parsedStep - 1
  }
}

function buildStepUrl(stepIndex) {
  const step = TOUR_STEPS[stepIndex]
  const url = new URL(step.route, window.location.origin)

  url.searchParams.set(TOUR_QUERY_KEY, TOUR_ENABLED_VALUE)
  url.searchParams.set(STEP_QUERY_KEY, String(stepIndex + 1))

  const stepQuery = step.query ?? {}
  for (const [key, value] of Object.entries(stepQuery)) {
    url.searchParams.set(key, value)
  }

  if (replayMode) {
    url.searchParams.set(REPLAY_QUERY_KEY, 'true')
  }

  return `${url.pathname}${url.search}`
}

function isCurrentRouteForStep(stepIndex) {
  const step = TOUR_STEPS[stepIndex]
  const currentUrl = new URL(window.location.href)
  const samePath = normalizePath(currentUrl.pathname) === normalizePath(step.route)

  if (!samePath) {
    return false
  }

  if (!step.query) {
    return true
  }

  return Object.entries(step.query).every(([key, value]) => currentUrl.searchParams.get(key) === value)
}

function navigateToStep(stepIndex) {
  window.location.assign(buildStepUrl(stepIndex))
}

function ensureTourUi() {
  if (tourOverlay && tourPanel) {
    return
  }

  tourOverlay = document.createElement('div')
  tourOverlay.className = 'tour-overlay'
  tourOverlay.setAttribute('aria-hidden', 'true')

  tourPanel = document.createElement('aside')
  tourPanel.className = 'tour-panel'
  tourPanel.setAttribute('role', 'dialog')
  tourPanel.setAttribute('aria-modal', 'true')
  tourPanel.innerHTML = `
    <div class="tour-panel-top">
      <p class="tour-panel-kicker">Tour guiado</p>
      <button type="button" class="tour-skip-link" data-tour-skip-link>Pular tour</button>
    </div>
    <h2 class="tour-panel-title" data-tour-title></h2>
    <p class="tour-panel-description" data-tour-description></p>
    <div class="tour-panel-footer">
      <span class="tour-panel-progress" data-tour-progress></span>
      <div class="tour-panel-actions">
        <button type="button" class="btn-secondary" data-tour-back>Voltar</button>
        <button type="button" class="btn-primary" data-tour-next>Próximo</button>
      </div>
    </div>
  `

  document.body.append(tourOverlay, tourPanel)

  titleElement = tourPanel.querySelector('[data-tour-title]')
  descriptionElement = tourPanel.querySelector('[data-tour-description]')
  progressElement = tourPanel.querySelector('[data-tour-progress]')
  backButton = tourPanel.querySelector('[data-tour-back]')
  nextButton = tourPanel.querySelector('[data-tour-next]')
  skipButton = tourPanel.querySelector('[data-tour-skip-link]')

  backButton?.addEventListener('click', handleBack)
  nextButton?.addEventListener('click', handleNext)
  skipButton?.addEventListener('click', handleSkip)

  window.addEventListener('resize', positionTourPanel, { passive: true })
  window.addEventListener('scroll', positionTourPanel, { passive: true, capture: true })
}

function getAnchorForStep(stepIndex) {
  const step = TOUR_STEPS[stepIndex]
  const anchor = document.querySelector(`[data-tour-anchor="${step.anchor}"]`)
  return anchor ?? document.querySelector('main') ?? document.body
}

function clearHighlightedAnchor() {
  if (!highlightedAnchor) {
    return
  }

  highlightedAnchor.classList.remove('tour-anchor-active')
  highlightedAnchor = null
}

function highlightAnchor(anchor) {
  clearHighlightedAnchor()
  highlightedAnchor = anchor
  highlightedAnchor.classList.add('tour-anchor-active')
}

function positionTourPanel() {
  if (!tourPanel || !highlightedAnchor) {
    return
  }

  const anchorRect = highlightedAnchor.getBoundingClientRect()
  const panelRect = tourPanel.getBoundingClientRect()
  const safeMargin = 16
  const gap = 14

  let top = anchorRect.bottom + gap
  if (top + panelRect.height > window.innerHeight - safeMargin) {
    top = anchorRect.top - panelRect.height - gap
  }
  if (top < safeMargin) {
    top = safeMargin
  }

  let left = anchorRect.left + (anchorRect.width / 2) - (panelRect.width / 2)
  if (left + panelRect.width > window.innerWidth - safeMargin) {
    left = window.innerWidth - panelRect.width - safeMargin
  }
  if (left < safeMargin) {
    left = safeMargin
  }

  tourPanel.style.top = `${Math.round(top)}px`
  tourPanel.style.left = `${Math.round(left)}px`
}

function renderStep(stepIndex) {
  const step = TOUR_STEPS[stepIndex]
  const anchor = getAnchorForStep(stepIndex)
  currentStepIndex = stepIndex

  highlightAnchor(anchor)
  anchor.scrollIntoView({ behavior: 'smooth', block: 'center', inline: 'nearest' })

  if (titleElement) {
    titleElement.textContent = step.title
  }

  if (descriptionElement) {
    descriptionElement.textContent = step.description
  }

  if (progressElement) {
    progressElement.textContent = `${stepIndex + 1} de ${TOUR_STEPS.length}`
  }

  if (backButton) {
    backButton.disabled = stepIndex === 0
  }

  if (nextButton) {
    nextButton.textContent = stepIndex === TOUR_STEPS.length - 1 ? 'Finalizar tour' : 'Próximo'
  }

  window.setTimeout(positionTourPanel, 130)
}

function getRequestVerificationToken() {
  const tokenElement = document.querySelector('meta[name="request-verification-token"]')
  const token = tokenElement?.getAttribute('content')
  return token ?? ''
}

function submitTourAction(actionUrl) {
  const form = document.createElement('form')
  form.method = 'post'
  form.action = actionUrl

  const requestVerificationToken = getRequestVerificationToken()
  if (requestVerificationToken) {
    const input = document.createElement('input')
    input.type = 'hidden'
    input.name = '__RequestVerificationToken'
    input.value = requestVerificationToken
    form.appendChild(input)
  }

  document.body.appendChild(form)
  form.submit()
}

function handleBack() {
  if (currentStepIndex <= 0) {
    return
  }

  navigateToStep(currentStepIndex - 1)
}

function handleNext() {
  if (currentStepIndex >= TOUR_STEPS.length - 1) {
    submitTourAction('/Onboarding/CompleteTour')
    return
  }

  navigateToStep(currentStepIndex + 1)
}

function handleSkip() {
  submitTourAction('/Onboarding/SkipTour')
}

export function initProductTour() {
  const state = getTourState()
  if (!state) {
    return
  }

  if (!isCurrentRouteForStep(state.stepIndex)) {
    navigateToStep(state.stepIndex)
    return
  }

  ensureTourUi()
  renderStep(state.stepIndex)
}
