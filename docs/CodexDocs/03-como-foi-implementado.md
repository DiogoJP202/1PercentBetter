# 03 - Como Foi Implementado

## Fluxo geral MVC

O fluxo padrao do projeto e:

1. O usuario acessa uma rota MVC.
2. O controller valida autenticacao e obtem o `UserId` com `User.GetRequiredUserId()`.
3. O controller chama um service especifico.
4. O service consulta ou altera dados pelo `ApplicationDbContext`.
5. O service retorna uma ViewModel.
6. A Razor View renderiza HTML com Tailwind.
7. JavaScript modular adiciona comportamento visual quando necessario.

Exemplo no dashboard:

```text
DashboardController.Index
  -> OnboardingService.IsCompletedAsync
  -> DashboardService.GetDashboardAsync
  -> ApplicationDbContext
  -> DashboardViewModel
  -> Views/Dashboard/Index.cshtml
  -> wwwroot/js/modules/dashboard.js
```

## Inicializacao da aplicacao

`Program.cs` configura:

- Logging em console e debug.
- `ApplicationDbContext` com SQL Server.
- ASP.NET Core Identity com `ApplicationUser`.
- Cookie paths customizados para login, logout e access denied.
- Services como scoped.
- MVC controllers com views.
- Razor Pages para suporte ao Identity.

Rota default:

```text
{controller=Home}/{action=Index}/{id?}
```

## Autenticacao e usuario atual

`ApplicationUser` herda de `IdentityUser` e adiciona:

- `DisplayName`
- `AvatarUrl`
- `MainColor`
- `ThemePreference`
- `OnboardingCompletedAt`
- `CreatedAt`
- `UpdatedAt`
- Colecoes de categorias, identidades, objetivos, habitos e check-ins.

`ClaimsPrincipalExtensions.GetRequiredUserId()` centraliza a leitura do id do usuario autenticado:

```csharp
user.FindFirstValue(ClaimTypes.NameIdentifier)
```

Isso evita repetir logica de claims nos controllers.

## Onboarding

O onboarding cria a primeira estrutura minima do usuario.

Fluxo:

1. Cadastro redireciona para `OnboardingController.Index`.
2. `OnboardingService.CreateFormAsync` carrega categorias disponiveis sem selecionar uma categoria automaticamente.
3. `Views/Onboarding/Index.cshtml` renderiza campos vazios e placeholders genericos iniciais.
4. `wwwroot/js/modules/onboarding.js` troca os placeholders conforme a area de foco selecionada.
5. Usuario preenche identidade, objetivo, habito, gatilho, versao de 2 minutos e recompensa.
6. `OnboardingService.ValidateFormAsync` valida se a categoria informada pertence ao usuario ou e global.
7. `OnboardingService.CompleteAsync` cria:
   - `UserIdentity`
   - `Goal`
   - `Habit`
8. O mesmo metodo atualiza `ApplicationUser.OnboardingCompletedAt`.
9. Usuario vai para o dashboard.

Detalhe importante:

- `OnboardingViewModel.CategoryId` e nullable com `[Required]`, evitando selecao automatica no GET.
- `Goal` recebe a `Identity` criada em memoria.
- `Habit` recebe a `Identity` e o `Goal` criados em memoria.
- `Reward` continua opcional e e salvo quando preenchido.
- EF Core salva todo o grafo no mesmo `SaveChangesAsync`.
- Nao houve nova migration para os placeholders dinamicos, pois a alteracao e de fluxo, ViewModel e frontend.

## Identidades, objetivos, habitos e anotacoes

Esses modulos seguem o mesmo padrao:

```text
Controller
  -> Service
  -> Entity Framework Core
  -> ViewModel
  -> Razor View
```

Operacoes implementadas:

- Listagem.
- Formulario de criacao.
- Criacao.
- Formulario de edicao.
- Atualizacao.

Os formularios usam ViewModels proprias e carregam listas auxiliares com `SelectOptionViewModel`.

Exemplo em habitos:

- `HabitFormViewModel.Categories`
- `HabitFormViewModel.Identities`
- `HabitFormViewModel.Goals`

Essas listas sao preenchidas em `HabitService.FillOptionsAsync`.

## Registro de habitos

O registro diario de habito usa `HabitService.RegisterLogAsync`.

Fluxo:

1. Controller recebe POST para `Complete`, `Fail` ou `Skip`.
2. Service verifica se o habito pertence ao usuario.
3. Service busca log existente para `UserId`, `HabitId` e `Date == DateTime.Today`.
4. Se nao existir, cria um novo `HabitLog`.
5. Atualiza `Status`.
6. Define `CompletedAt` apenas quando o status e `Completed`.
7. Salva no banco.

O banco reforca isso com indice unico:

```text
HabitLogs(UserId, HabitId, Date)
```

## Check-in diario

`CheckInService.CreateOrUpdateAsync` tambem usa comportamento de upsert.

Fluxo:

1. A tela `CheckIns/Today` carrega o check-in da data atual.
2. Se existir, os dados sao preenchidos.
3. Se nao existir, a ViewModel vem com valores padrao.
4. No POST, o service procura por `UserId` e `Date`.
5. Cria ou atualiza `DailyCheckIn`.

O banco reforca uma linha por usuario e data:

```text
DailyCheckIns(UserId, Date)
```

## Dashboard

`DashboardService.GetDashboardAsync` monta todos os dados do dashboard em uma unica ViewModel.

Logica atual:

- `today`: data atual local do servidor.
- `weekStart`: 6 dias antes de hoje.
- `historyStart`: 89 dias antes de hoje.
- Busca habitos ativos do usuario com logs entre `historyStart` e hoje.
- Filtra habitos devidos hoje com `IsDueOn`.
- Calcula:
  - `DueToday`
  - `CompletedToday`
  - `FailedToday`
  - `SkippedToday`
  - `TodayCompletionRate`
  - `WeeklyCompletionRate`
  - `CompletedLast7Days`
  - `CheckInsLast7Days`
  - `CurrentStreak`
  - `BestStreak`
  - `BetterIndex`
  - `Alerts`

### Frequencia de habitos

Metodo: `IsDueOn`.

Regras atuais:

- Se a data e anterior a `Habit.CreatedAt`, o habito nao e devido.
- `Daily`: devido todos os dias.
- `Weekly`: devido no mesmo dia da semana em que o habito foi criado.
- `Monthly`: devido no mesmo dia do mes em que o habito foi criado.
- `SpecificDays`: compara `DaysOfWeek` com o dia da semana.
- Outros tipos caem como devido.

Formulario:

- O formulario de habitos exibe checkboxes de segunda a domingo quando `FrequencyType` e `SpecificDays`.
- Os checkboxes preenchem `HabitFormViewModel.SelectedDaysOfWeek`.
- `HabitService` converte `SelectedDaysOfWeek` para a string persistida em `Habit.DaysOfWeek`.
- Quando a frequencia nao e `SpecificDays`, `DaysOfWeek` e salvo como `null`.
- `Custom` foi removido do select do MVP, porque ainda nao possui regra real.

Ponto de atencao:

- `Monthly` compara o dia do mes com `CreatedAt.Day`, sem tratamento especial para meses mais curtos.

### Sequencias

`CalculateCurrentStreak` percorre de hoje para tras por ate 60 dias. Um dia conta para a sequencia quando todos os habitos devidos naquele dia possuem log `Completed`.

`CalculateBestStreak` percorre dos ultimos 90 dias ate hoje e guarda a maior sequencia encontrada.

### Grafico semanal

`BuildWeeklyProgress` cria 7 pontos, um por dia, com:

- `Label`
- `Completed`
- `Failed`
- `Skipped`

A View serializa `Model.WeeklyProgress` em `data-points`. O modulo `dashboard.js` le esse JSON e cria um ApexCharts de barras empilhadas.

## Calendario

O calendario usa FullCalendar no frontend e `CalendarService` no backend.

Fluxo:

1. `Views/Calendar/Index.cshtml` renderiza o container com `data-events-url`.
2. `wwwroot/js/modules/calendar.js` inicializa FullCalendar.
3. FullCalendar chama `CalendarController.Events` com query string `start` e `end`.
4. `CalendarController` obtem o usuario logado e chama `CalendarService.GetHabitLogEventsAsync`.
5. `CalendarService` consulta `HabitLogs` do usuario no periodo.
6. Cada log vira um `CalendarEventViewModel`.
7. A resposta JSON alimenta o calendario.

Cada evento contem:

- `id`
- `title`
- `start`
- `allDay`
- `backgroundColor`
- `borderColor`
- `textColor`
- `extendedProps`

As cores seguem o status:

- `Completed`: verde.
- `Failed`: rosa/vermelho.
- `Skipped`: amarelo.
- `Partial`: azul.

## Frontend

### Layout

`Views/Shared/_Layout.cshtml` define:

- HTML em `pt-BR`.
- Tema escuro via classe `dark`.
- CSS local `~/css/app.css`.
- Notyf CSS via CDN.
- Layout autenticado com sidebar.
- Header mobile.
- Scripts globais:
  - Notyf.
  - SweetAlert2.
  - Lucide.
  - `~/js/app.js`.

O layout tambem transforma `TempData["Success"]`, `TempData["Warning"]` e `TempData["Info"]` em `window.__flashMessages`.

### Notificacoes

`wwwroot/js/modules/notifications.js`:

- Recebe array de mensagens.
- Instancia Notyf.
- Exibe success, warning ou info.

`wwwroot/js/app.js`:

- Chama `showFlashMessages`.
- Chama `bindConfirmDialogs`.
- Inicializa `window.lucide.createIcons()`.

### Confirmacoes

`wwwroot/js/modules/dialogs.js`:

- Procura forms com `data-confirm`.
- Intercepta o submit.
- Exibe SweetAlert2 com textos definidos em `data-confirm-title`, `data-confirm-text` e `data-confirm-button`.
- Se o usuario confirma, reenvia o form.

Usado atualmente em:

- Falhar habito.
- Pular habito.
- Pausar objetivo.
- Concluir objetivo.

### CSS

`wwwroot/css/input.css` contem:

- Diretivas Tailwind.
- Classes utilitarias globais:
  - `.nav-link`
  - `.mobile-nav-link`
  - `.btn-primary`
  - `.btn-secondary`
  - `.btn-danger`
  - `.app-card`
  - `.form-label`
  - `.form-input`
  - `.form-select`
  - `.form-textarea`
  - `.status-pill`

`wwwroot/css/app.css` e gerado por:

```powershell
npm run css:build
```

### Tailwind

`tailwind.config.js` varre:

- `./Views/**/*.cshtml`
- `./Areas/**/*.cshtml`
- `./wwwroot/js/**/*.js`

Configura:

- `darkMode: 'class'`
- Cores estendidas: `surface`, `panel`, `line`

## Traducoes de enums

`Extensions/EnumDisplayExtensions.cs` centraliza textos em PT-BR para:

- `ItemStatus`
- `GoalPriority`
- `HabitDifficulty`
- `HabitFrequencyType`
- `HabitLogStatus`
- `MoodLevel`
- `NoteType`

Tambem fornece:

```csharp
EnumDisplayExtensions.ToSelectList<TEnum>()
```

As views usam esse helper para selects e badges, evitando exibir nomes internos como `Completed`, `VeryEasy` ou `DailyReflection`.

## Banco de dados

`ApplicationDbContext` herda de `IdentityDbContext<ApplicationUser>`.

DbSets:

- `Categories`
- `UserIdentities`
- `Goals`
- `Habits`
- `HabitLogs`
- `DailyCheckIns`
- `Notes`

Relacionamentos principais:

- Usuario possui categorias, identidades, objetivos, habitos e check-ins.
- Identidade pode ter objetivos, habitos e anotacoes.
- Objetivo pode ter habitos e anotacoes.
- Habito pode ter logs e anotacoes.
- Categoria pode agrupar identidades, objetivos e habitos.

Delete behavior:

- Usuario usa `Restrict` para evitar delecoes em cascata amplas.
- Varios vinculos opcionais usam `SetNull`.
- `HabitLog` e apagado em cascata quando o habito e apagado.

## Scripts SQL

### Seed demo

`001_seed_demo_mvp.sql`:

- Cria ou atualiza usuario `demo@1better.local`.
- Remove dados antigos desse usuario.
- Recria identidades, objetivos, habitos, logs, check-ins e notas.
- Usa senha demo `Demo@123`.

### Limpeza demo

`002_clear_demo_mvp.sql`:

- Remove apenas o usuario demo e seus dados relacionados.

### Consultas demo

`003_demo_summary_queries.sql`:

- Lista usuario, identidades, objetivos, habitos, logs, check-ins e notas do usuario demo.
