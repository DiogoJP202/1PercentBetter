# 05 - Problemas Conhecidos e Pontos de Atencao

## Problemas conhecidos

### Comandos de servidor em background podem travar o terminal

Durante a validacao visual, comandos usando `Start-Process` ou `cmd.exe /c start /b` para executar `dotnet run` ficaram presos por muito tempo no ambiente atual.

Impacto:

- Pode parecer que o processo esta travado.
- Pode deixar processos `dotnet` abertos.
- Atrasa validacao visual.

Recomendacao:

- Evitar subir servidor em background por script neste ambiente.
- Usar terminal visivel com `dotnet run --urls http://127.0.0.1:5022`.
- Encerrar manualmente com `Ctrl+C`.
- Como alternativa, usar Visual Studio/Rider.

### Calendario precisa validacao visual

O calendario agora busca eventos reais em `Calendar/Events`, mas ainda nao foi validado visualmente no navegador nesta rodada.

Impacto:

- Pode haver ajustes finos de layout, tooltip ou cores depois do teste manual.

### Confirmacoes ainda precisam ser reaproveitadas em fluxos futuros

SweetAlert2 ja foi integrado para falhar/pular habitos e pausar/concluir objetivos. Novas acoes sensiveis devem usar o mesmo padrao de `data-confirm`.

Impacto:

- Futuras exclusoes ou edicoes destrutivas podem ficar sem confirmacao se o padrao nao for reaplicado.

### Placeholders do onboarding dependem do nome da categoria

O mapeamento de exemplos dinamicos usa o texto da categoria selecionada, normalizado sem acentos.

Impacto:

- Categorias novas ou renomeadas usam fallback generico ate receberem exemplos especificos em `wwwroot/js/modules/onboarding.js`.

### Frequencia de habitos parcialmente incompleta

O backend possui enum para frequencias, mas a UI ainda nao cobre tudo.

Pontos:

- `SpecificDays` ja possui checkboxes no formulario de habitos.
- `Custom` nao tem regra real e foi removido do select do MVP.
- `Monthly` compara dia do mes com `CreatedAt.Day`, sem tratar meses mais curtos.

### Dashboard usa regras simples de score

`BetterIndex` e calculado com formula simples:

```text
completedToday * 12
+ completedLast7Days * 4
+ checkInsLast7Days * 5
+ streak * 7
+ activeIdentities * 3
```

Impacto:

- E util para MVP, mas ainda nao e uma metrica validada de produto.
- Pode precisar de ajuste quando analytics crescer.

### Dashboard carrega logs dos ultimos 90 dias junto com habitos

`DashboardService` usa filtered include para carregar logs recentes.

Impacto:

- Funciona para MVP.
- Pode ficar pesado com muitos habitos/logs.
- Futuramente pode ser melhor usar queries agregadas.

### Notas nao possuem exclusao

Anotacoes podem ser criadas e editadas, mas nao removidas.

### Sem controle de concorrencia explicito no dominio

O Identity tem `ConcurrencyStamp`, mas entidades do dominio nao usam row version.

Impacto:

- Edicoes simultaneas podem sobrescrever dados sem aviso.

## Riscos tecnicos

### Validacao ainda precisa evoluir alem dos vinculos principais

ViewModels tem `Required`, `MaxLength` e `Range`. Os vinculos opcionais principais ja sao validados nos services de identidades, objetivos, habitos e anotacoes, mas ainda ha regras de negocio que podem melhorar.

Exemplos:

- Validar formato de `Icon`.
- Validar formato de `Color`.
- Validar `DaysOfWeek`.

### Vinculos opcionais devem ser reavaliados em novos modulos

Os modulos atuais ja receberam validacao de vinculos. O risco volta a existir se novos fluxos forem criados sem repetir esse padrao.

Impacto potencial:

- Um usuario poderia tentar enviar manualmente um `GoalId`, `IdentityId`, `CategoryId` ou `HabitId` que nao pertence a ele.

Recomendacao:

- Antes de salvar em novos fluxos, validar propriedade dos IDs opcionais.
- Categorias globais devem permitir `UserId == null`; categorias privadas devem exigir `UserId == usuario`.

### Dependencia de CDN

Bibliotecas de UI ainda dependem de internet:

- ApexCharts.
- FullCalendar.
- Notyf.
- SweetAlert2.
- Lucide Icons.

Impacto:

- Em ambiente offline ou com bloqueio de CDN, funcionalidades visuais podem falhar.

### Sem testes automatizados

Atualmente a seguranca vem de build manual e teste manual.

Areas que merecem teste primeiro:

- Onboarding.
- Registro de logs de habitos.
- Check-in upsert.
- Calculo de dashboard.
- Validacao de propriedade por usuario.

## Partes sensiveis do codigo

### `DashboardService`

Concentra varias regras:

- Frequencia.
- Sequencia.
- Taxas.
- Alertas.
- Indice 1% Better.

Alteracoes aqui podem mudar numeros visiveis no dashboard.

### `HabitService.RegisterLogAsync`

Responsavel por manter apenas um log por habito/dia. Deve continuar alinhado ao indice unico do banco.

### `OnboardingService.CompleteAsync`

Cria multiplas entidades relacionadas e marca onboarding concluido. Uma falha aqui pode deixar usuario parcialmente configurado se a transacao implicita do `SaveChangesAsync` nao for suficiente para futuros passos mais complexos.

### `ApplicationDbContext.OnModelCreating`

Define tabelas, indices, relacionamentos, delete behavior e seeds. Mudancas podem exigir nova migration.

### Scripts SQL demo

O seed demo contem hash de senha e IDs fixos. Deve ser tratado apenas como dado de desenvolvimento.

## Pontos que precisam de teste cuidadoso

- Cadastro e login.
- Redirecionamento para onboarding quando necessario.
- Usuario com onboarding completo indo direto ao dashboard.
- Criacao de identidade, objetivo e habito.
- Edicao de entidades com vinculos opcionais.
- POST duplicado de concluir/falhar habito no mesmo dia.
- Check-in salvo mais de uma vez no mesmo dia.
- Dashboard sem dados.
- Dashboard com dados demo.
- Dashboard em dias sem habitos devidos.
- Frequencia semanal, mensal e dias especificos.
- Calendario depois que receber eventos reais.
- Responsividade em mobile.
