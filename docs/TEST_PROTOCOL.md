# Protocolo de teste Android — 0.5.5-dev

## Antes da instalação
- Faça backup da pasta do Observatório dos Reinos.
- Instale somente os arquivos do ZIP da branch `scaffold/observatorio-dos-reinos`.
- A verificação C# contra stubs não é uma DLL instalável nem valida todas as chamadas da API real.

## Confirmar que a nova versão carregou
1. Em retrato, procure a faixa `OBSERVATORIO 0.5.5-dev` e o atalho `GOVERNO`.
2. No log, procurar `CANVAS_CREATED` ou `CANVAS_NOT_CREATED`.
3. Procurar `STATUS_INFO_PREFIX_INSTALLED` e `STATUS_WINDOW_FINALIZER_INSTALLED` (ou os logs detalhados `NOT_INSTALLED/FAILED`).
4. Gire para paisagem: a faixa e o atalho `GOVERNO` devem continuar acessíveis; abrir e fechar o painel pelo atalho.

## Testar o status do reino
1. Possua um rei como nos testes anteriores.
2. Toque no botão nativo de informações/status do reino.
3. O prefix preventivo deve registrar `STATUS_INFO_PREFIX_SKIP` se o método de tooltip/seleção for chamado durante `StatsWindow.OnEnable`. A intenção é permitir que a janela conclua a inicialização sem executar essa rotina auxiliar nessa transição.
4. Se ainda ocorrer `NullReferenceException`, guardar o log completo: se prefix não instalou, usar a razão `STATUS_INFO_PREFIX_NOT_INSTALLED/FAILED`; se instalou mas não houve `SKIP`, a stack real pode usar outro método/nome ou o frame caller não foi reconhecido; se houve `SKIP` mas a exceção persiste, ela vem de outra referência dentro de `StatsWindow.OnEnable`.

Nenhuma das ações de administração altera dados do mundo nessa versão.