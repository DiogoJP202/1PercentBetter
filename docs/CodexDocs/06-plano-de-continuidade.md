# 06 - Plano de Continuidade

## Objetivo imediato

Transformar o MVP atual em uma versao navegavel, validada visualmente e coerente para demonstracao completa.

O foco deve ser estabilizar o que ja existe antes de criar modulos maiores como IA, analytics avancado ou revisoes semanais.

## Ordem recomendada

### 1. Validar o fluxo ponta a ponta

Prioridade maxima.

Tarefas:

- Rodar o app em terminal visivel ou IDE.
- Logar com usuario demo.
- Criar um usuario novo.
- Passar pelo onboarding.
- Criar/editar identidade.
- Criar/editar objetivo.
- Criar/editar habito.
- Registrar concluido, falha e pulado.
- Salvar check-in.
- Criar/editar anotacao.
- Abrir dashboard.
- Abrir calendario.

Resultado esperado:

- Lista objetiva de bugs visuais ou funcionais reais.

### 2. Corrigir UX basica de habitos

Tarefas:

- Adicionar botao `Pular`.
- Exibir horario sugerido.
- Adicionar seletor para dias especificos.
- Ocultar ou desabilitar campos que ainda nao funcionam.
- Garantir mensagens TempData consistentes.

Motivo:

- Habitos sao o centro do MVP.

### 3. Implementar calendario real

Tarefas:

- Criar ViewModel ou DTO para eventos.
- Adicionar action JSON em `CalendarController`, por exemplo `Events`.
- Consultar `HabitLogs` do usuario.
- Retornar eventos com titulo, data, status e cor.
- Atualizar `calendar.js` para buscar eventos.

Motivo:

- A tela ja existe, mas hoje e apenas visual.

### 4. Adicionar confirmacoes e feedbacks

Tarefas:

- Criar modulo JS `dialogs.js`.
- Integrar SweetAlert2 em formularios/acoes sensiveis.
- Padronizar Notyf para success, warning, info e error.
- Adicionar confirmacao para pausar/concluir objetivo e falhar/pular habito.

Motivo:

- Reduz cliques acidentais e melhora percepcao de acabamento.

### 5. Reforcar seguranca de vinculos por usuario

Tarefas:

- Validar `CategoryId`, `IdentityId`, `GoalId` e `HabitId` nos services.
- Garantir que IDs opcionais pertencem ao usuario ou sao categorias globais.
- Retornar erro de validacao ou ignorar vinculo invalido.

Motivo:

- Forms podem ser manipulados manualmente.
- Isso fecha uma brecha importante antes de evoluir.

### 6. Melhorar dashboard

Tarefas:

- Revisar formula do `BetterIndex`.
- Adicionar cards por identidade.
- Melhorar alertas.
- Criar graficos de humor/energia com base em check-ins.
- Permitir filtro simples por periodo.

Motivo:

- Dashboard e a principal tela de retorno do usuario.

### 7. Criar testes automatizados iniciais

Tarefas:

- Criar projeto de testes.
- Testar `HabitService.RegisterLogAsync`.
- Testar `CheckInService.CreateOrUpdateAsync`.
- Testar `DashboardService` para frequencia, sequencia e taxas.
- Testar `OnboardingService.CompleteAsync`.

Motivo:

- As regras de data e usuario ficam mais sensiveis conforme o app cresce.

## Backlog tecnico sugerido

1. Rodar validacao manual do MVP com usuario demo.
2. Registrar bugs reais encontrados na validacao.
3. Adicionar botao `Pular` no dashboard e em `Habits/Index`.
4. Criar UI para `DaysOfWeek` em habitos.
5. Ajustar regra de frequencia `Custom` ou remover da UI ate existir regra.
6. Implementar endpoint `Calendar/Events`.
7. Popular FullCalendar com logs reais.
8. Adicionar cores/status no calendario.
9. Criar modulo `wwwroot/js/modules/dialogs.js`.
10. Integrar SweetAlert2 nas acoes sensiveis.
11. Validar IDs opcionais por usuario nos services.
12. Adicionar mensagens de erro quando vinculo opcional for invalido.
13. Melhorar empty states com CTAs.
14. Revisar responsividade das telas principais.
15. Adicionar testes para logs de habitos.
16. Adicionar testes para check-in diario.
17. Adicionar testes para dashboard.
18. Mover bibliotecas CDN para dependencias locais, se o projeto caminhar para producao.
19. Criar CRUD de categorias do usuario.
20. Planejar revisao semanal e mensal.

## Fases futuras

### Fase 2 - Habitos e calendario completos

- Frequencias robustas.
- Calendario com logs reais.
- Melhor controle de historico.
- Filtros por identidade, objetivo e status.

### Fase 3 - Revisoes

- Revisao semanal.
- Revisao mensal.
- Resumos por identidade.
- Aprendizados e ajustes de sistema.

### Fase 4 - Analytics

- Tendencias.
- Correlacoes entre humor, energia e habitos.
- Ranking de consistencia.
- Indicadores por area de vida.

### Fase 5 - IA

- Sugestao de ajustes de habitos.
- Analise de check-ins.
- Resumos semanais automaticos.
- Recomendacoes personalizadas.

## Comandos uteis

Instalar dependencias frontend:

```powershell
npm install
```

Compilar CSS:

```powershell
npm run css:build
```

Build .NET:

```powershell
$env:DOTNET_CLI_HOME = (Join-Path (Get-Location) '.dotnet-home')
dotnet build --nologo --no-restore
```

Aplicar migrations:

```powershell
$env:DOTNET_CLI_HOME = (Join-Path (Get-Location) '.dotnet-home')
.\.tools\dotnet-ef.exe database update --project src\OnePercentBetter.Web --startup-project src\OnePercentBetter.Web
```

Rodar app em terminal visivel:

```powershell
cd src\OnePercentBetter.Web
dotnet run --urls http://127.0.0.1:5022
```

Observacao: evitar comandos em background para `dotnet run` neste ambiente.
