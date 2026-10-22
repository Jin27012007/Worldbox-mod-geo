# Protocolo de teste Android — 0.5.6-dev

1. Faça backup da pasta do Observatório dos Reinos antes de atualizar o código a partir do ZIP da branch `scaffold/observatorio-dos-reinos`.
2. Em retrato, procure a faixa `OBSERVATORIO 0.5.6-dev` e o atalho `GOVERNO`.
3. No log, confira `CANVAS_CREATED`, `STATUS_INFO_PREFIX_INSTALLED`, `STATUS_WINDOW_FINALIZER_INSTALLED` e a linha `Runtime assembly fingerprint`.
4. Gire para landscape; confirme que a faixa e o atalho continuam visíveis e que o painel pode ser aberto/fechado pelo atalho.
5. Toque no botão nativo de status do reino. Procure `STATUS_INFO_PREFIX_SKIP`; confirme se a janela abre por completo.
6. Se a exceção persistir, envie a stack completa e a linha `Runtime assembly fingerprint`. Compare MVIDs com `docs/ASSEMBLY_INVESTIGATION.md`; não faça novo patch até saber se a DLL em execução corresponde às referências analisadas.

A última compilação CI confirmou a fonte anterior com stubs. Isso não é uma DLL instalável nem valida o Harmony/runtime real; aguarde o resultado da revisão 0.5.6-dev e não considere o bug resolvido até o teste no dispositivo.