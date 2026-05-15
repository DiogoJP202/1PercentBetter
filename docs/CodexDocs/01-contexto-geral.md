# 01 - Contexto Geral

## Objetivo do sistema

O 1% Better e um sistema web pessoal para transformar identidades desejadas em objetivos, objetivos em habitos diarios e habitos em progresso visivel. O MVP atual foca em dar ao usuario uma rotina minima de evolucao: criar conta, configurar o primeiro sistema, registrar habitos, fazer check-in diario e acompanhar um dashboard simples.

## Problema que resolve

O sistema resolve a falta de conexao entre intencoes pessoais e execucao diaria. Em vez de manter objetivos soltos, o app organiza:

- Quem o usuario quer se tornar.
- Quais objetivos sustentam essa identidade.
- Quais habitos pequenos executam esses objetivos.
- Como registrar progresso diario.
- Como visualizar consistencia semanal.

## Principais modulos

- Autenticacao e cadastro.
- Onboarding inicial.
- Identidades.
- Objetivos.
- Habitos.
- Logs diarios de habitos.
- Check-in diario.
- Dashboard inicial.
- Anotacoes.
- Calendario visual inicial.
- Categorias base.
- Seed SQL para dados demo.

## Tecnologias utilizadas

Backend:

- C#.
- ASP.NET Core MVC.
- ASP.NET Core Identity.
- Entity Framework Core.
- SQL Server LocalDB.

Frontend:

- Razor Views.
- Tailwind CSS compilado localmente.
- JavaScript modular.
- ApexCharts para grafico do dashboard.
- FullCalendar para tela de calendario.
- Notyf para notificacoes.
- SweetAlert2 para confirmacoes em acoes sensiveis.
- Lucide Icons para icones.
- jQuery apenas para validacao unobtrusive do ASP.NET Core.

Build e ferramentas:

- .NET SDK 8.
- EF Core Tools.
- npm.
- Tailwind CSS 3.

## Estrutura geral do projeto

```text
1better/
  database/
    scripts/
      001_seed_demo_mvp.sql
      002_clear_demo_mvp.sql
      003_demo_summary_queries.sql
  docs/
    frontend-stack.md
    CodexDocs/
  src/
    OnePercentBetter.Web/
      Controllers/
      Data/
        Migrations/
      Extensions/
      Models/
        Entities/
        Enums/
        Identity/
      Services/
      ViewModels/
      Views/
      wwwroot/
        css/
        js/
        lib/
      Program.cs
      appsettings.json
      package.json
      tailwind.config.js
```

## Decisao de arquitetura

A arquitetura atual e simples e propositalmente adequada para MVP:

- Controllers finos recebem requisicoes e delegam regras para services.
- Services concentram consultas, criacao, atualizacao e composicao de ViewModels.
- EF Core acessa SQL Server diretamente pelo `ApplicationDbContext`.
- Razor Views renderizam HTML server-side.
- JavaScript fica modularizado por recurso quando necessario.
- Tailwind fornece utilitarios e uma pequena camada de classes globais.

Nao ha Clean Architecture, CQRS, Mediator, API separada ou frontend SPA. Isso reduz custo de implementacao e facilita evolucao inicial.

## Configuracao de banco atual

Connection string em `src/OnePercentBetter.Web/appsettings.json`:

```json
"DefaultConnection": "Server=(localdb)\\OnePercentBetterLocalDb;Database=OnePercentBetter;Trusted_Connection=True;MultipleActiveResultSets=true;Encrypt=False;TrustServerCertificate=True"
```

Banco esperado para desenvolvimento local:

- Instancia: `(localdb)\OnePercentBetterLocalDb`
- Database: `OnePercentBetter`
