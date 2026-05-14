# CodexDocs - 1% Better

Esta pasta documenta o estado atual do projeto web 1% Better para facilitar continuidade por outro desenvolvedor.

Esta documentacao considera o working tree atual, incluindo os ajustes recentes de Tailwind local, dashboard e traducao de enums.

## Indice

- [01 - Contexto geral](01-contexto-geral.md)
- [02 - O que foi implementado](02-o-que-foi-implementado.md)
- [03 - Como foi implementado](03-como-foi-implementado.md)
- [04 - Pendencias e proximos passos](04-pendencias-e-proximos-passos.md)
- [05 - Problemas conhecidos](05-problemas-conhecidos.md)
- [06 - Plano de continuidade](06-plano-de-continuidade.md)

## Estado rapido

O projeto ja possui um MVP web em ASP.NET Core MVC com autenticacao, onboarding, identidades, objetivos, habitos, logs diarios de habitos, check-in diario, dashboard inicial, anotacoes e calendario visual inicial.

A stack atual e:

- ASP.NET Core MVC em .NET 8.
- Razor Views.
- SQL Server LocalDB.
- Entity Framework Core.
- ASP.NET Core Identity.
- Tailwind CSS compilado localmente.
- JavaScript modular.
- ApexCharts, FullCalendar, Notyf, SweetAlert2 e Lucide Icons via CDN nas views/layout.

## Caminhos principais

- Solucao: `OnePercentBetter.sln`
- Projeto web: `src/OnePercentBetter.Web`
- DbContext: `src/OnePercentBetter.Web/Data/ApplicationDbContext.cs`
- Controllers: `src/OnePercentBetter.Web/Controllers`
- Services: `src/OnePercentBetter.Web/Services`
- Models: `src/OnePercentBetter.Web/Models`
- ViewModels: `src/OnePercentBetter.Web/ViewModels`
- Views: `src/OnePercentBetter.Web/Views`
- Frontend JS: `src/OnePercentBetter.Web/wwwroot/js`
- CSS fonte: `src/OnePercentBetter.Web/wwwroot/css/input.css`
- CSS compilado: `src/OnePercentBetter.Web/wwwroot/css/app.css`
- Scripts SQL demo: `database/scripts`

## Usuario demo

Existe script SQL para popular dados de demonstracao.

- Script: `database/scripts/001_seed_demo_mvp.sql`
- E-mail: `demo@1better.local`
- Senha: `Demo@123`

## Validacao recente

Ultima validacao tecnica feita apos os ajustes de frontend/dashboard:

```powershell
dotnet build --nologo --no-restore
```

Resultado: build concluido com sucesso, sem warnings e sem erros.
