# Decisao tecnica: Frontend do 1% Better

## Stack frontend adotada

O projeto 1% Better nao usara Bootstrap 5.

A stack frontend oficial passa a ser:

- Razor Views
- Tailwind CSS
- JavaScript modular
- ApexCharts ou ECharts
- FullCalendar
- SweetAlert2
- Notyf
- Lucide Icons ou Tabler Icons

## Direcao pratica para o MVP

Para o MVP, a recomendacao e usar:

- Tailwind CSS como base visual e sistema de utilitarios.
- JavaScript modular separado por tela ou modulo.
- ApexCharts para os primeiros graficos do dashboard.
- FullCalendar para a tela de calendario de habitos.
- SweetAlert2 para confirmacoes importantes.
- Notyf para feedbacks rapidos de sucesso, erro e aviso.
- Lucide Icons como primeira opcao de icones, por ser leve e consistente.

## Impactos na implementacao

### Layout

O arquivo de layout principal deve ser construido com classes Tailwind, sem dependencias de componentes Bootstrap.

Prioridades:

- Tema escuro como padrao.
- Sidebar ou navegacao lateral para desktop.
- Navegacao compacta para mobile.
- Cards, formularios, botoes e badges feitos com classes Tailwind.
- Padroes visuais reaproveitaveis em partial views quando fizer sentido.

### Formularios

Os formularios Razor devem continuar usando tag helpers do ASP.NET Core MVC, mas a estilizacao deve ser feita com Tailwind.

Exemplos de areas prioritarias:

- Login e cadastro.
- Onboarding.
- Criacao de identidade.
- Criacao de objetivo.
- Criacao de habito.
- Check-in diario.

### JavaScript

Evitar scripts soltos grandes dentro das views.

Organizacao sugerida:

```text
wwwroot/
└── js/
    ├── app.js
    ├── modules/
    │   ├── dashboard.js
    │   ├── habits.js
    │   ├── checkins.js
    │   ├── calendar.js
    │   └── notifications.js
    └── shared/
        ├── api.js
        ├── charts.js
        └── dialogs.js
```

### CSS

Organizacao sugerida:

```text
wwwroot/
└── css/
    ├── input.css
    └── app.css
```

`input.css` deve conter as diretivas do Tailwind e estilos globais minimos.

`app.css` deve ser o arquivo compilado.

## Ajuste no plano original

Onde o plano anterior mencionava Bootstrap 5, substituir por Tailwind CSS.

Controllers, models, services, migrations, entidades e banco de dados nao mudam por causa dessa decisao.

A mudanca afeta principalmente:

- Views Razor.
- Layout compartilhado.
- Partial views.
- Scripts por tela.
- Build frontend.
- Padrao visual dos componentes.

## Cuidados tecnicos

- Nao misturar Bootstrap com Tailwind.
- Definir tokens visuais basicos no `tailwind.config.js`.
- Criar convencoes simples para botoes, cards, inputs e badges.
- Evitar componentes gigantes com dezenas de classes repetidas sem criterio.
- Extrair partials para componentes visuais repetidos.
- Manter JavaScript modular e especifico por tela.
- Evitar jQuery no MVP, exceto se alguma biblioteca exigir.
- Garantir que graficos e calendario respeitem o tema escuro.

## Backlog tecnico atualizado do frontend

1. Instalar e configurar Tailwind CSS no projeto ASP.NET Core MVC.
2. Criar `tailwind.config.js`.
3. Criar `wwwroot/css/input.css`.
4. Configurar build para gerar `wwwroot/css/app.css`.
5. Criar layout Razor base com Tailwind.
6. Criar componentes visuais base: botoes, inputs, cards, badges e alerts.
7. Criar estrutura de JavaScript modular em `wwwroot/js`.
8. Integrar Notyf para notificacoes globais.
9. Integrar SweetAlert2 para confirmacoes.
10. Integrar Lucide Icons ou Tabler Icons.
11. Criar dashboard inicial usando ApexCharts ou ECharts.
12. Criar calendario simples usando FullCalendar.
13. Validar responsividade das telas principais.
14. Remover qualquer referencia futura a Bootstrap 5 do plano tecnico.
