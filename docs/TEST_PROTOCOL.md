# Protocolo de teste Android — 0.5.7-dev

## Primeiro, confirmar a revisão
1. Faça backup da pasta atual e instale os arquivos da branch `scaffold/observatorio-dos-reinos`.
2. Em retrato, confirme a faixa `OBSERVATORIO 0.5.7-dev` e o atalho `GOVERNO`.
3. No log, confira `CANVAS_CREATED`, `STATUS_INFO_PREFIX_INSTALLED`, `STATUS_WINDOW_PREFIX_INSTALLED`, `STATUS_WINDOW_POSTFIX_INSTALLED` e `STATUS_WINDOW_FINALIZER_INSTALLED`.
4. Gire para landscape; confirme que faixa e atalho continuam visíveis e que o menu pode ser aberto/fechado.

## Janela nativa de status
1. Possua um rei como nos testes anteriores e pressione o botão nativo de status.
2. O log deve mostrar `STATUS_WINDOW_OPENING`, e se a atualização auxiliar for chamada, `STATUS_INFO_PREFIX_SKIP`.
3. Confirme se o painel de status abre completo. Se houver erro, copie toda a stack trace e a linha `Runtime assembly fingerprint` com os MVIDs de StatsWindow, PowerTabController e Harmony.
4. Se `STATUS_WINDOW_PREFIX_INSTALLED` estiver ausente, preserve a razão `NOT_INSTALLED`/`FAILED`; se `STATUS_INFO_PREFIX_SKIP` ocorrer mas a janela continuar quebrada, a exceção vem de outro ponto de `StatsWindow.OnEnable` e deverá ser rastreada separadamente.

## Compilação
A última refatoração teve uma falha de código detectada pelo CI e o helper foi restaurado em `dd553b5cf329ee0e7e84cc58c390e3ca9bd3ed61`. A próxima execução precisa passar antes de testar. O build de stubs não é uma DLL instalável; a compilação com referências reais requer as DLLs locais em `references/` que não ficam no repositório público.