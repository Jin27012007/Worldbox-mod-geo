# Protocolo de teste Android — 0.5.5-dev

1. Faça backup da pasta atual e instale somente os arquivos do ZIP da branch `scaffold/observatorio-dos-reinos`.
2. Em retrato, confirme a faixa `OBSERVATORIO 0.5.5-dev` e o atalho `GOVERNO`.
3. No log, procure `CANVAS_CREATED`, `STATUS_INFO_PREFIX_INSTALLED` e `STATUS_WINDOW_FINALIZER_INSTALLED`. Se algum estiver ausente, preserve a linha `*_NOT_INSTALLED`/`*_FAILED`.
4. Gire para paisagem; confirme que a faixa e o atalho continuam visíveis e abra/feche o painel pelo atalho.
5. Pressione o botão nativo de status do reino. Procure `STATUS_INFO_PREFIX_SKIP`; a janela deve terminar de inicializar sem a rotina auxiliar de tooltip naquela transição.
6. Se o erro persistir, preserve a stack completa e a linha `Runtime assembly fingerprint` (StatsWindow, PowerTabController e Harmony). Compare os MVIDs com `docs/ASSEMBLY_INVESTIGATION.md` antes de outro patch.

## O que o build comprova
A verificação independente contra stubs compilou o fonte com zero erros/avisos. Isso não é uma DLL instalável e não verifica as APIs reais. O build contra assemblies reais está preparado, mas as DLLs privadas não são fornecidas ao runner público.

As opções administrativas ainda são demonstrações e não alteram reino, cidades, guerra ou diplomacia reais.