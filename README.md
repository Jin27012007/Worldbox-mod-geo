# Observatório dos Reinos — WorldBox Android

Projeto original para acrescentar governo e diplomacia ao controle nativo de rei/rainha. Alvo informado: WorldBox Android 0.51.4 + NeoModLoader.

## Revisão 0.5.8-dev
- Canvas persistente em tela cheia; o contêiner filho `SafeAreaRoot` se reajusta após rotação.
- O atalho `GOVERNO` permanece visível como reserva, mesmo quando o botão nativo `GOV` é localizado.
- Prefix Harmony ativa o escopo de status via `__state`; ignora a rotina auxiliar de seleção/tooltip somente durante a inicialização da janela nativa. O postfix e o finalizer restauram o estado anterior.
- A recuperação tenta fechar o `ScrollWindow` externo inteiro se surgir a NullReference conhecida.
- Logs de diagnóstico registram se o Canvas e patches foram instalados e o FullName/MVID das assemblies efetivamente carregadas.

## Build automatizado
O GitHub Actions da revisão atual passou com **0 erros e 0 avisos** e também executou um smoke test que confirmou que a rotina auxiliar só é ignorada dentro do escopo de status e que postfix/finalizer restauram o estado: https://github.com/Jin27012007/Worldbox-mod-geo/actions/runs/39850586295

Esse build compila contra stubs Unity/NeoModLoader; **não produz uma DLL instalável e não valida todas as APIs reais**. O build com referências reais está preparado em `tests/CompileWorldBoxRefs.csproj`, mas só roda quando as DLLs privadas estiverem localmente disponíveis sob `references/`. Elas não são publicadas no repositório público.

## Erro nativo ainda em aberto
O log do aparelho menciona `showWhoIsSelected`/`SelectedPowerTabButtons`; as assemblies fornecidas contêm `showWorldTipSelected`/`SelectedPowerTabTopButtons`. Hashes, MVIDs e IL estão em [docs/ASSEMBLY_INVESTIGATION.md](docs/ASSEMBLY_INVESTIGATION.md). A versão 0.5.8-dev ainda não foi confirmada no dispositivo, e o erro não deve ser considerado resolvido até essa validação.

## Próximo teste
Faça backup antes de atualizar o mod. Confirme `OBSERVATORIO 0.5.8-dev`, `GOVERNO`, `CANVAS_CREATED`, os três status de instalação Harmony e `Runtime assembly fingerprint`. Ao abrir o status, procure `STATUS_INFO_PREFIX_SKIP`.