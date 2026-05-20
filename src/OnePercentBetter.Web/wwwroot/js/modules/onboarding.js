const categorySelect = document.querySelector('[data-onboarding-category]')
const fields = [...document.querySelectorAll('[data-onboarding-field]')]

const generic = {
  identityName: 'Pessoa consistente em uma área importante',
  goalTitle: 'Construir uma rotina simples e sustentável',
  identityStatement: 'Eu sou uma pessoa que melhora um pouco todos os dias',
  habitTitle: 'Praticar por 15 minutos em um horário definido',
  trigger: 'Quando a situação X surgir, executarei a resposta Y',
  twoMinuteVersion: 'Fazer a menor versão possível por 2 minutos',
  reward: 'Sentir progresso real no fim do dia'
}

const placeholders = {
  tecnologia: {
    identityName: 'Desenvolvedor .NET Júnior',
    goalTitle: 'Construir e entender uma aplicação .NET completa',
    identityStatement: 'Eu sou uma pessoa que evolui tecnicamente todos os dias',
    habitTitle: 'Praticar C# por 30 minutos depois do trabalho',
    trigger: 'Quando eu ligar o computador, vou abrir o projeto por 15 minutos',
    twoMinuteVersion: 'Abrir o projeto e corrigir uma pequena melhoria',
    reward: 'Ser capaz de construir e entender uma aplicação .NET completa'
  },
  idiomas: {
    identityName: 'Fluente em japonês',
    goalTitle: 'Entender e me comunicar em japonês',
    identityStatement: 'Eu sou uma pessoa que fala fluentemente japonês e lê livros em japonês',
    habitTitle: 'Estudar japonês às 15h no meu quarto',
    trigger: 'Quando eu terminar o almoço, vou revisar minhas anotações',
    twoMinuteVersion: 'Ler uma página de um livro em japonês',
    reward: 'Ser capaz de falar japonês'
  },
  saude: {
    identityName: 'Pessoa saudável e enérgica',
    goalTitle: 'Dormir melhor e ter mais energia',
    identityStatement: 'Eu sou uma pessoa que cuida da minha energia antes de cobrar intensidade',
    habitTitle: 'Caminhar 15 minutos depois do trabalho',
    trigger: 'Quando eu encerrar o trabalho, vou calçar o tênis',
    twoMinuteVersion: 'Caminhar por 2 minutos na rua',
    reward: 'Sentir meu corpo com mais energia'
  },
  estudos: {
    identityName: 'Estudante consistente',
    goalTitle: 'Aprender um tema importante para minha carreira',
    identityStatement: 'Eu sou uma pessoa que aprende com constância e revisa o que estuda',
    habitTitle: 'Estudar por 25 minutos antes de dormir',
    trigger: 'Quando eu sentar na mesa, vou abrir meu material de estudo',
    twoMinuteVersion: 'Ler uma página e anotar uma ideia',
    reward: 'Sentir que estou dominando um assunto novo'
  },
  trabalho: {
    identityName: 'Barista',
    goalTitle: 'Trabalhar em uma cafeteria',
    identityStatement: 'Sou uma pessoa que conhece várias técnicas e métodos de cafés',
    habitTitle: 'Preparar um café usando V60 pela manhã',
    trigger: 'Quando eu terminar o café da manhã, vou anotar uma técnica',
    twoMinuteVersion: 'Abrir um vídeo de receita e tentar fazer',
    reward: 'Conseguir preparar cafés melhores'
  },
  projetos: {
    identityName: 'Criador de projetos consistentes',
    goalTitle: 'Publicar uma versão utilizável de um projeto pessoal',
    identityStatement: 'Eu sou uma pessoa que transforma ideias em entregas pequenas e reais',
    habitTitle: 'Trabalhar 20 minutos no projeto depois do jantar',
    trigger: 'Quando eu abrir o notebook, vou escolher uma tarefa pequena',
    twoMinuteVersion: 'Abrir o backlog e mover uma tarefa simples',
    reward: 'Ver meu projeto ficando mais completo'
  },
  financas: {
    identityName: 'Pessoa organizada financeiramente',
    goalTitle: 'Entender para onde meu dinheiro está indo',
    identityStatement: 'Eu sou uma pessoa que cuida do dinheiro com clareza e calma',
    habitTitle: 'Registrar gastos por 5 minutos no fim do dia',
    trigger: 'Quando eu deitar para dormir, vou abrir minha planilha de gastos',
    twoMinuteVersion: 'Registrar apenas um gasto do dia',
    reward: 'Sentir mais controle sobre minhas escolhas financeiras'
  },
  casa: {
    identityName: 'Pessoa organizada',
    goalTitle: 'Deixar meu quarto limpo',
    identityStatement: 'Eu sou uma pessoa que cuida do meu ambiente todos os dias',
    habitTitle: 'Organizar meu quarto por 10 minutos depois do banho',
    trigger: 'Quando eu chegar em casa, vou guardar 3 coisas fora do lugar',
    twoMinuteVersion: 'Arrumar uma superfície do quarto',
    reward: 'Sentir meu ambiente mais limpo e organizado'
  },
  social: {
    identityName: 'Pessoa presente nos relacionamentos',
    goalTitle: 'Fortalecer contato com pessoas importantes',
    identityStatement: 'Eu sou uma pessoa que cultiva relações com presença e cuidado',
    habitTitle: 'Enviar uma mensagem sincera para alguém todo sábado',
    trigger: 'Quando eu terminar o almoço, vou mandar uma mensagem para alguém',
    twoMinuteVersion: 'Responder uma mensagem pendente',
    reward: 'Sentir minhas relações mais próximas'
  },
  mental: {
    identityName: 'Pessoa calma e focada',
    goalTitle: 'Ter mais clareza durante a semana',
    identityStatement: 'Eu sou uma pessoa que observa meus pensamentos sem se perder neles',
    habitTitle: 'Meditar por 5 minutos ao acordar',
    trigger: 'Quando eu sentar na cama, vou respirar fundo por 2 minutos',
    twoMinuteVersion: 'Fazer 3 respirações lentas',
    reward: 'Sentir minha mente mais leve'
  }
}

function normalizeCategory(value) {
  return value
    .normalize('NFD')
    .replace(/[\u0300-\u036f]/g, '')
    .toLowerCase()
    .trim()
}

function getSelectedCategoryKey() {
  const selectedOption = categorySelect?.selectedOptions?.[0]
  return normalizeCategory(selectedOption?.textContent ?? '')
}

function syncPlaceholders() {
  const selectedPlaceholders = placeholders[getSelectedCategoryKey()] ?? generic

  fields.forEach((field) => {
    field.placeholder = selectedPlaceholders[field.dataset.onboardingField] ?? generic[field.dataset.onboardingField] ?? ''
  })
}

if (categorySelect && fields.length > 0) {
  categorySelect.addEventListener('change', syncPlaceholders)
  syncPlaceholders()
}
