# 02 - O Que Foi Implementado

## Autenticacao

Arquivos principais:

- `Controllers/AccountController.cs`
- `Models/Identity/ApplicationUser.cs`
- `ViewModels/Auth/LoginViewModel.cs`
- `ViewModels/Auth/RegisterViewModel.cs`
- `Views/Account/Login.cshtml`
- `Views/Account/Register.cshtml`
- `Views/Account/AccessDenied.cshtml`
- `Views/Shared/_LoginPartial.cshtml`

Implementado:

- Cadastro de usuario com nome, e-mail, senha e confirmacao de senha.
- Login por e-mail e senha.
- Logout via POST com antiforgery.
- Redirecionamento de usuario autenticado para dashboard.
- Redirecionamento de cadastro concluido para onboarding.
- Identity configurado sem confirmacao obrigatoria de e-mail.
- Regras de senha simplificadas para MVP: minimo 6 caracteres e sem obrigatoriedade de caractere nao alfanumerico.

## Onboarding

Arquivos principais:

- `Controllers/OnboardingController.cs`
- `Services/OnboardingService.cs`
- `ViewModels/Onboarding/OnboardingViewModel.cs`
- `Views/Onboarding/Index.cshtml`
- `wwwroot/js/modules/onboarding.js`

Implementado:

- Tela inicial obrigatoria apos cadastro.
- Formulario para criar uma area de foco, identidade, objetivo e primeiro habito.
- Campos do onboarding iniciam vazios; exemplos aparecem apenas como placeholders.
- Placeholders dinamicos por area de foco, com fallback generico quando a categoria nao possui exemplos especificos.
- Criacao conjunta de `UserIdentity`, `Goal` e `Habit`.
- Marcacao de `ApplicationUser.OnboardingCompletedAt`.
- Bloqueio do dashboard para usuarios sem onboarding completo.

## Dashboard

Arquivos principais:

- `Controllers/DashboardController.cs`
- `Services/DashboardService.cs`
- `ViewModels/Dashboard/DashboardViewModel.cs`
- `ViewModels/Dashboard/TodayHabitViewModel.cs`
- `ViewModels/Dashboard/WeeklyProgressPointViewModel.cs`
- `Views/Dashboard/Index.cshtml`
- `wwwroot/js/modules/dashboard.js`

Implementado:

- Dashboard protegido por login e onboarding concluido.
- Metricas do dia: habitos devidos, concluidos, falhas, pulados e taxa de conclusao.
- Metricas semanais: conclusoes dos ultimos 7 dias, taxa semanal e check-ins semanais.
- Sequencia atual e melhor sequencia calculadas com base nos logs de habitos.
- Indice 1% Better calculado a partir de habitos concluidos, check-ins, sequencia e identidades ativas.
- Identidade em destaque baseada na identidade ativa com mais habitos.
- Lista de habitos de hoje com a versao de 2 minutos, gatilho e acoes de concluir/falhar.
- Alertas simples quando um habito falhou duas vezes seguidas ou tem baixa taxa recente.
- Grafico semanal com ApexCharts usando barras empilhadas para concluidos, falhas e pulados.
- Endpoint JSON `GetWeeklyProgress`.

## Identidades

Arquivos principais:

- `Controllers/IdentitiesController.cs`
- `Services/IdentityService.cs`
- `Models/Entities/UserIdentity.cs`
- `ViewModels/Identities/IdentityFormViewModel.cs`
- `ViewModels/Identities/IdentityListItemViewModel.cs`
- `Views/Identities/Index.cshtml`
- `Views/Identities/Create.cshtml`
- `Views/Identities/Edit.cshtml`
- `Views/Identities/_Form.cshtml`

Implementado:

- Listagem de identidades do usuario.
- Criacao de identidade.
- Edicao de identidade.
- Relacionamento opcional com categoria.
- Status, cor e icone configuraveis.
- Contadores de objetivos e habitos relacionados.
- Validacao de categoria vinculada para garantir categoria global ou pertencente ao usuario.

## Objetivos

Arquivos principais:

- `Controllers/GoalsController.cs`
- `Services/GoalService.cs`
- `Models/Entities/Goal.cs`
- `ViewModels/Goals/GoalFormViewModel.cs`
- `ViewModels/Goals/GoalListItemViewModel.cs`
- `Views/Goals/Index.cshtml`
- `Views/Goals/Create.cshtml`
- `Views/Goals/Edit.cshtml`
- `Views/Goals/_Form.cshtml`

Implementado:

- Listagem de objetivos.
- Criacao e edicao.
- Vinculo opcional com identidade e categoria.
- Status e prioridade.
- Datas de inicio e alvo.
- Acoes de pausar e concluir.
- Contagem de habitos vinculados.
- Validacao de categoria e identidade vinculadas para garantir que pertencem ao usuario logado ou, no caso de categoria, que seja global.

## Habitos

Arquivos principais:

- `Controllers/HabitsController.cs`
- `Services/HabitService.cs`
- `Models/Entities/Habit.cs`
- `Models/Entities/HabitLog.cs`
- `ViewModels/Habits/HabitFormViewModel.cs`
- `ViewModels/Habits/HabitListItemViewModel.cs`
- `Views/Habits/Index.cshtml`
- `Views/Habits/Create.cshtml`
- `Views/Habits/Edit.cshtml`
- `Views/Habits/_Form.cshtml`

Implementado:

- Listagem de habitos.
- Criacao e edicao.
- Vinculo opcional com identidade, objetivo e categoria.
- Frequencia, dias especificos, horario sugerido e dificuldade no modelo.
- Formulario com selecao visual de dias da semana quando a frequencia e dias especificos.
- Versao de 2 minutos, gatilho e recompensa.
- Status, cor e icone.
- Registro diario de log como concluido, falhou ou pulado.
- Botao `Pular` visivel na listagem e no dashboard.
- Listagem com frequencia e horario sugerido.
- Upsert de log por usuario, habito e data atual.
- Validacao de categoria, identidade e objetivo vinculados para garantir que pertencem ao usuario logado ou, no caso de categoria, que seja global.

## Check-in diario

Arquivos principais:

- `Controllers/CheckInsController.cs`
- `Services/CheckInService.cs`
- `Models/Entities/DailyCheckIn.cs`
- `ViewModels/CheckIns/DailyCheckInViewModel.cs`
- `Views/CheckIns/Today.cshtml`

Implementado:

- Tela de check-in do dia.
- Humor, energia, produtividade e nota do dia.
- Pequena vitoria, principal dificuldade, ajuste para amanha e notas.
- Criacao ou atualizacao do check-in da data atual.
- Redirecionamento para dashboard apos salvar.

## Anotacoes

Arquivos principais:

- `Controllers/NotesController.cs`
- `Services/NoteService.cs`
- `Models/Entities/Note.cs`
- `ViewModels/Notes/NoteFormViewModel.cs`
- `ViewModels/Notes/NoteListItemViewModel.cs`
- `Views/Notes/Index.cshtml`
- `Views/Notes/Create.cshtml`
- `Views/Notes/Edit.cshtml`
- `Views/Notes/_Form.cshtml`

Implementado:

- Listagem de anotacoes por data.
- Criacao e edicao.
- Tipo de anotacao.
- Tags.
- Vinculo opcional com identidade, objetivo e habito.
- Preview de conteudo com limite de 180 caracteres.
- Validacao de identidade, objetivo e habito vinculados para garantir que pertencem ao usuario logado.

## Calendario

Arquivos principais:

- `Controllers/CalendarController.cs`
- `Services/CalendarService.cs`
- `ViewModels/Calendar/CalendarEventViewModel.cs`
- `ViewModels/Calendar/CalendarEventExtendedPropsViewModel.cs`
- `Views/Calendar/Index.cshtml`
- `wwwroot/js/modules/calendar.js`

Implementado:

- Tela de calendario com FullCalendar.
- FullCalendar carregado com locale `pt-br`.
- Views `dayGridMonth`, `timeGridWeek` e `listWeek`.
- Endpoint JSON `Calendar/Events`.
- Eventos reais baseados em `HabitLogs` do usuario logado.
- Filtro por periodo usando `start` e `end` enviados pelo FullCalendar.
- Eventos coloridos por status: concluido, falhou, pulado e parcial.
- Legenda visual de status na tela.
- Tooltip nativo com status e notas quando disponiveis.

## Categorias

Arquivos principais:

- `Models/Entities/Category.cs`
- `Services/CategoryService.cs`
- `Data/ApplicationDbContext.cs`

Implementado:

- Entidade de categoria.
- Categorias globais sem `UserId`.
- Possibilidade de categorias do usuario via `UserId`.
- Seed de 10 categorias base no `ApplicationDbContext`.
- Opcoes de categoria usadas em onboarding, identidades, objetivos e habitos.

## Frontend e layout

Arquivos principais:

- `Views/Shared/_Layout.cshtml`
- `Views/_ViewImports.cshtml`
- `wwwroot/css/input.css`
- `wwwroot/css/app.css`
- `wwwroot/js/app.js`
- `wwwroot/js/modules/notifications.js`
- `wwwroot/js/modules/dialogs.js`
- `wwwroot/js/modules/dashboard.js`
- `wwwroot/js/modules/calendar.js`
- `wwwroot/js/modules/habits-form.js`
- `tailwind.config.js`
- `package.json`
- `package-lock.json`

Implementado:

- Layout autenticado com sidebar desktop e navegacao mobile.
- Tema escuro padrao.
- Tailwind CSS local, sem CDN do Tailwind.
- Classes globais para botoes, cards, campos de formulario, validacoes e badges.
- Notificacoes globais por `TempData` usando Notyf.
- Confirmacoes globais com SweetAlert2 para forms marcados com `data-confirm`.
- Inicializacao global de Lucide Icons.
- JavaScript modular.
- Traducoes de enums para PT-BR via `EnumDisplayExtensions`.

## Banco de dados e migrations

Arquivos principais:

- `Data/ApplicationDbContext.cs`
- `Data/Migrations/00000000000000_CreateIdentitySchema.cs`
- `Data/Migrations/20260514162755_CreateCoreHabitSchema.cs`
- `Data/Migrations/ApplicationDbContextModelSnapshot.cs`

Implementado:

- Schema ASP.NET Core Identity.
- Tabelas principais do sistema: `Categories`, `Identities`, `Goals`, `Habits`, `HabitLogs`, `DailyCheckIns`, `Notes`.
- Indices por usuario/status/nome/data.
- Indice unico em `HabitLogs` para `UserId`, `HabitId` e `Date`.
- Indice unico em `DailyCheckIns` para `UserId` e `Date`.
- Delete behavior conservador: `Restrict` para usuario e `SetNull` para vinculos opcionais.

## Scripts SQL demo

Arquivos:

- `database/scripts/001_seed_demo_mvp.sql`
- `database/scripts/002_clear_demo_mvp.sql`
- `database/scripts/003_demo_summary_queries.sql`

Implementado:

- Seed idempotente de usuario demo.
- Dados demo com 3 identidades, 3 objetivos, 5 habitos, 14 dias de logs por habito, 10 check-ins e 4 anotacoes.
- Script para limpar apenas o usuario demo.
- Script de consultas para inspecionar os dados demo.

## Decisoes tecnicas importantes

- MVC server-side foi mantido para reduzir complexidade do MVP.
- Services concentram regras e composicao de ViewModels.
- Identity foi usado em vez de auth custom.
- SQL Server LocalDB foi usado para desenvolvimento local.
- Bootstrap foi removido da decisao de frontend.
- Tailwind local substituiu o CDN do Tailwind.
- Bibliotecas visuais externas ainda sao carregadas por CDN para acelerar MVP.
- Enums continuam persistidos como numeros, com traducao apenas na camada de exibicao.
- Logs de habitos usam upsert por data para evitar duplicidade.
- A frequencia `Custom` foi retirada do select de habitos por enquanto, pois ainda nao ha regra de negocio implementada para ela.
- Services principais validam vinculos opcionais antes de persistir IDs enviados por formularios.
