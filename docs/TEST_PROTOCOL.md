# Protocolo de teste Android — 0.5.8-dev

1. Faça backup da pasta do mod e instale os arquivos da branch `scaffold/observatorio-dos-reinos`.
2. Em retrato, procure a faixa `OBSERVATORIO 0.5.8-dev` e o atalho `GOVERNO`.
3. No log, procure `CANVAS_CREATED`, `STATUS_INFO_PREFIX_INSTALLED`, `STATUS_WINDOW_PREFIX_INSTALLED`, `STATUS_WINDOW_POSTFIX_INSTALLED`, `STATUS_WINDOW_FINALIZER_INSTALLED` e `Runtime assembly fingerprint`.
4. Gire para paisagem; confirme que a faixa e o atalho continuam acessíveis e abra/feche o painel.
5. Possua um rei e pressione o botão nativo de status. Procure `STATUS_INFO_PREFIX_SKIP` e confirme se o painel de status termina de abrir sem erro.
6. Se o erro persistir, preserve a stack completa e o fingerprint (MVID) de `StatsWindow` e `PowerTabController`; compare com `docs/ASSEMBLY_INVESTIGATION.md`.

## Build automatizado
O GitHub Actions passou com 0 erros/avisos e o smoke test confirmou o escopo do prefix e a restauração por postfix/finalizer: https://github.com/Jin27012007/Worldbox-mod-geo/actions/runs/39850586295. O build é contra stubs, não uma DLL instalada. A compilação com referências reais é pulada em runners públicos porque as DLLs privadas ficam locais.

As ações de administração ainda são protótipos e não modificam o mundo.