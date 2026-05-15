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
- Confirmar se os placeholders do onboarding mudam conforme a area de foco.
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

### 2. Validar e refinar UX basica de habitos

Tarefas:

- Validar visualmente o botao `Pular`.
- Validar visualmente horario sugerido e frequencia na listagem.
- Testar seletor de dias especificos.
- Ajustar mensagens de validacao se necessario.
- Garantir mensagens TempData consistentes.

Motivo:

- Habitos sao o centro do MVP.

### 3. Validar e refinar calendario real

Tarefas:

- Validar visualmente os eventos do FullCalendar com usuario demo.
- Conferir cores por status.
- Conferir comportamento de mes, semana e lista.
- Adicionar modal ou painel de detalhe para evento, se necessario.
- Planejar filtros por habito, identidade e status.

Motivo:

- A tela ja esta ligada ao banco, mas ainda precisa de acabamento de produto.

### 4. Refinar confirmacoes e feedbacks

Tarefas:

- Reusar `dialogs.js` em novas acoes sensiveis.
- Padronizar Notyf para success, warning, info e error.
- Validar textos e comportamento das confirmacoes em navegador.
- Adicionar confirmacao em futuras exclusoes.

Motivo:

- Reduz cliques acidentais e melhora percepcao de acabamento.

### 5. Manter seguranca de vinculos por usuario

Tarefas:

- Repetir o padrao de validacao em novos services.
- Revisar futuras APIs JSON antes de expor escrita.
- Adicionar testes automatizados para vinculos invalidos.

Motivo:

- Forms podem ser manipulados manualmente.
- Esse cuidado evita que um usuario relacione dados de outro usuario.

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
3. Testar `Pular`, horario sugerido e dias especificos em habitos.
4. Ajustar regra de frequencia mensal.
5. Validar calendario real com dados demo.
6. Adicionar filtros no calendario por habito, identidade e status.
7. Validar SweetAlert2 nas acoes sensiveis.
8. Adicionar testes para vinculos invalidos por usuario.
9. Melhorar empty states com CTAs.
10. Revisar responsividade das telas principais.
11. Adicionar testes para logs de habitos.
12. Adicionar testes para check-in diario.
13. Adicionar testes para dashboard.
14. Mover bibliotecas CDN para dependencias locais, se o projeto caminhar para producao.
15. Criar CRUD de categorias do usuario.
16. Planejar revisao semanal e mensal.

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
