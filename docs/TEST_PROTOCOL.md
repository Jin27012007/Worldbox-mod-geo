# Protocolo de teste Android — 0.5.9-dev

1. Faça backup da pasta do Observatório e instale somente os arquivos do ZIP da branch `scaffold/observatorio-dos-reinos`.
2. Em retrato, procure a faixa `OBSERVATORIO 0.5.9-dev` e o atalho `GOVERNO`.
3. No log, confirme `Runtime assembly fingerprint` mesmo que Harmony não possa ser resolvido. Depois procure `CANVAS_CREATED`, `STATUS_INFO_PREFIX_INSTALLED`, `STATUS_WINDOW_PREFIX_INSTALLED`, `STATUS_WINDOW_POSTFIX_INSTALLED` e `STATUS_WINDOW_FINALIZER_INSTALLED`.
4. Gire para paisagem: confirme que a faixa e o atalho permanecem visíveis e que o painel pode abrir/fechar.
5. Possua um rei e toque no status nativo. Procure `STATUS_INFO_PREFIX_SKIP` e confirme se a janela termina de inicializar.
6. Se a exceção persistir, preserve a stack completa e o fingerprint de `StatsWindow`, `PowerTabController` e Harmony para comparar com `docs/ASSEMBLY_INVESTIGATION.md`.

## Build automatizado
https://github.com/Jin27012007/Worldbox-mod-geo/actions/runs/39850928817 confirma fonte com 0 warnings/0 errors contra stubs e o smoke test do escopo Harmony. Não é uma DLL instalável; o build real é pulado no runner público por falta das DLLs privadas.

As ações de administração continuam rascunhos, sem alterações no mundo.