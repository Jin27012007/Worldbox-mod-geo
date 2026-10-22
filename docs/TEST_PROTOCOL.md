# Protocolo de teste Android — 0.5.8-dev

1. Faça backup da pasta do mod e instale a revisão da branch `scaffold/observatorio-dos-reinos`.
2. Em retrato, confirme a faixa `OBSERVATORIO 0.5.8-dev` e o atalho `GOVERNO`.
3. No log, procure `CANVAS_CREATED`, `STATUS_INFO_PREFIX_INSTALLED`, `STATUS_WINDOW_PREFIX_INSTALLED`, `STATUS_WINDOW_POSTFIX_INSTALLED`, `STATUS_WINDOW_FINALIZER_INSTALLED` e `Runtime assembly fingerprint`.
4. Gire para landscape; confirme que a faixa e o atalho continuam visíveis e abra/feche o menu pelo atalho.
5. Pressione o botão nativo de status do reino. Procure `STATUS_WINDOW_OPENING` e `STATUS_INFO_PREFIX_SKIP`; veja se a janela abre completamente.
6. Se a exceção persistir, guarde a stack completa e o fingerprint. Compare os MVIDs com `docs/ASSEMBLY_INVESTIGATION.md` antes de propor outro patch.

O CI da versão 0.5.8-dev passou contra stubs, mas ainda não há DLL compilada contra as assemblies reais nem teste no aparelho. Não considerar a correção confirmada até o teste de execução.