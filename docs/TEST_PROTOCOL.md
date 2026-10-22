# Protocolo de teste Android — 0.5.4-dev

## Antes de instalar
- Faça backup da pasta do Observatório dos Reinos.
- Use somente o ZIP da branch scaffold/observatorio-dos-reinos e substitua apenas os arquivos desse mod.
- O build checker passa contra stubs; a DLL de mod ainda não foi compilada contra assemblies privadas do jogo.

## Conferir a inicialização
1. Abra o WorldBox em retrato e procure a faixa `OBSERVATORIO 0.5.4-dev` e o botão `GOVERNO`.
2. No log, localize `Canvas, botao reserva e painel admin prontos` e `UI administrativa via Canvas criada`.
3. Procure `STATUS_PATCH_INSTALLED` ou `STATUS_PATCH_NOT_INSTALLED`. Se falhou, preserve a mensagem e a lista de assinaturas Harmony/OnEnable.
4. Toque no botão `GOVERNO`; teste as quatro abas, os campos de texto, botões e rolagem.
5. Gire para landscape; confirme que a faixa e o atalho continuam visíveis.

## Testar status nativo
1. Possua um rei como nos testes anteriores.
2. Pressione o botão nativo de status.
3. Se o finalizer rodar, o log conterá `FINALIZER ACIONADO` e o painel alternativo deverá abrir. Se a exceção ocorrer sem esse registro, o patch não interceptou o método em execução.
4. Não considere o erro corrigido apenas porque a janela fechou; precisamos confirmar que o status funciona ou que o fallback é estável.

## Se a faixa/log não aparecer
Isso indica que a revisão correta não está a correr ou que a UI não foi criada; nesse caso não vale a pena ajustar posições visuais. Envie o log completo desde o arranque do mod.
