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

### Calendario sem eventos reais

`wwwroot/js/modules/calendar.js` inicializa FullCalendar com `events: []`.

Impacto:

- A tela existe, mas ainda nao representa dados do banco.

### SweetAlert2 carregado mas nao usado

O layout carrega SweetAlert2, mas ainda nao existem confirmacoes implementadas.

Impacto:

- Operacoes sensiveis ainda acontecem direto no POST.

### Frequencia de habitos incompleta

O backend possui enum para frequencias, mas a UI ainda nao cobre tudo.

Pontos:

- `SpecificDays` depende de `DaysOfWeek`, mas nao ha seletor claro na view.
- `Custom` nao tem regra real.
- `Monthly` compara dia do mes com `CreatedAt.Day`, sem tratar meses mais curtos.

### Botao de pular nao aparece nas principais telas

`HabitsController.Skip` existe e `HabitService.RegisterLogAsync` aceita `Skipped`, mas a tela atual prioriza `Concluir` e `Falhei`.

Impacto:

- Funcionalidade existe no backend, mas fica pouco acessivel.

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

### Validacao ainda concentrada em DataAnnotations

ViewModels tem `Required`, `MaxLength` e `Range`, mas algumas regras de negocio ainda nao sao validadas no service.

Exemplos:

- Verificar se `CategoryId`, `IdentityId`, `GoalId` e `HabitId` pertencem ao usuario antes de vincular.
- Validar formato de `Icon`.
- Validar formato de `Color`.
- Validar `DaysOfWeek`.

### Vinculos opcionais podem aceitar IDs de outro usuario

Nos services de criacao/edicao, os IDs opcionais sao atribuidos diretamente.

Impacto potencial:

- Um usuario poderia tentar enviar manualmente um `GoalId`, `IdentityId`, `CategoryId` ou `HabitId` que nao pertence a ele.

Recomendacao:

- Antes de salvar, validar propriedade dos IDs opcionais.
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

