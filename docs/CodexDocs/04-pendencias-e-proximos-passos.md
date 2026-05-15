# 04 - Pendencias e Proximos Passos

## Prioridade alta

### Validacao visual completa do MVP

O build passa, mas a ultima tentativa de abrir servidor via comando em background travou. Falta validar visualmente no navegador:

- Login.
- Cadastro.
- Onboarding.
- Placeholders dinamicos do onboarding por area de foco.
- Dashboard.
- Criacao e edicao de identidade.
- Criacao e edicao de objetivo.
- Criacao e edicao de habito.
- Concluir/falhar/pular habito.
- Check-in diario.
- Anotacoes.
- Calendario.

Recomendacao:

- Subir o projeto manualmente pelo Visual Studio, Rider ou `dotnet run` em terminal visivel.
- Evitar comandos de background via `cmd /c start /b` ou `Start-Process` neste ambiente.

### Completar UX de habitos

Pendencias:

- Validar visualmente a nova UI de dias especificos no navegador.
- Melhorar regra de frequencia mensal para meses sem o mesmo dia.
- Implementar regra real para `Custom` antes de voltar a exibir essa opcao.
- Avaliar se frequencia semanal deve usar dia de criacao ou um dia escolhido pelo usuario.

### Refinar calendario real

O calendario ja consome `HabitLogs` reais via endpoint JSON. Ainda falta validar e refinar a experiencia.

Pendencias:

- Validar visualmente no navegador com dados demo.
- Melhorar tooltip ou modal de detalhe do evento.
- Filtros por habito, identidade ou status.
- Opcional: mostrar tambem habitos pendentes do dia, nao apenas logs registrados.

### Refinar confirmacoes com SweetAlert2

SweetAlert2 ja esta integrado de forma generica para forms com `data-confirm`.

Ja aplicado em:

- Concluir objetivo.
- Pausar objetivo.
- Falhar habito.
- Pular habito.

Pendencias:

- Futuras exclusoes.
- Revisar textos depois de testes manuais.
- Reaproveitar o padrao em novas acoes sensiveis.

### Testar e ajustar SQL LocalDB

Pontos para conferir:

- Se a instancia `(localdb)\OnePercentBetterLocalDb` existe.
- Se migrations foram aplicadas.
- Se o seed demo roda sem conflito.
- Se o usuario demo consegue logar apos novo seed.

## Prioridade media

### Melhorar formularios

Pendencias:

- Inputs de cor poderiam virar color picker ou swatches.
- Campo de icone Lucide poderia virar seletor simples.
- Formularios poderiam ter textos de ajuda curtos.
- Datas e horarios precisam de revisao de UX.
- Validacoes server-side podem ser reforcadas para formatos de cor, icone e regras especificas de frequencia.

### Melhorar empty states

Ja existem estados vazios simples, mas podem melhorar em:

- Identidades.
- Objetivos.
- Habitos.
- Anotacoes.
- Calendario.

Cada estado vazio deveria sugerir a proxima acao mais provavel.

### Melhorar dashboard

Possiveis melhorias:

- Separar cards em partials.
- Adicionar componente de progresso por identidade.
- Mostrar tendencias de humor e energia.
- Permitir filtro por periodo.
- Exibir top habitos consistentes e habitos em risco.

### Mover bibliotecas CDN para dependencias locais

Atualmente Tailwind ja e local, mas ainda usam CDN:

- Notyf.
- SweetAlert2.
- Lucide Icons.
- ApexCharts.
- FullCalendar.

Para producao, avaliar npm/local bundle ou pipeline de assets.

### Melhorar organizacao visual

Pendencias:

- Revisar responsividade em mobile real.
- Validar contraste de cards e badges.
- Padronizar tamanho de botoes em formularios e cards.
- Evitar textos longos quebrando layout.

## Prioridade baixa

### Perfis e preferencias

Ainda nao existe tela para:

- Editar nome.
- Trocar tema.
- Alterar cor principal.
- Alterar avatar.
- Trocar senha.

### CRUD de categorias do usuario

O modelo suporta categorias por usuario, mas nao existe tela para criar ou editar categorias customizadas.

### Exclusoes e arquivamento

Nao ha exclusao ou arquivamento completo para:

- Identidades.
- Objetivos.
- Habitos.
- Anotacoes.

Hoje o caminho principal e mudar status em alguns casos.

### Internacionalizacao

Os textos estao em PT-BR no codigo. Nao ha `resx`, localizacao formal ou suporte multi-idioma.

### Testes automatizados

Nao ha projeto de testes.

Sugestoes:

- Testes unitarios para services com banco em memoria ou SQLite.
- Testes de integracao para controllers principais.
- Testes de regra de frequencia e sequencia.

## Funcionalidades futuras do escopo original

Deixar para fases posteriores:

- Habitos ruins.
- Revisao semanal.
- Revisao mensal.
- Analytics avancado.
- IA e recomendacoes inteligentes.
- Metas compostas e progresso quantitativo.
- Relatorios exportaveis.
- Gamificacao mais profunda.
