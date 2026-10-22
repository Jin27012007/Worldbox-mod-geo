# Status técnico — Observatório dos Reinos 0.6.1-dev

## Mudanças na revisão 0.6.1-dev

- O botão `GOVERNO` é criado independentemente da janela `ScrollWindow`; se a janela falhar ao ser construída, o botão não deve desaparecer por causa dessa falha.
- O botão usa um Canvas próprio com `overrideSorting = true`, ordem de renderização 32000, fundo verde de alto contraste e posição relativa ao canto superior esquerdo de `CanvasMain.canvas_ui`.
- O log registra explicitamente as condições `CANVAS_MAIN_NULL`, `CANVAS_UI_NULL`, `CANVAS_UI_INACTIVE`, `NATIVE_WINDOW_BUTTON_CREATED` ou `NATIVE_WINDOW_BUTTON_FAILED`.
- A janela é inicializada depois que o botão já existe. Se a criação falhar, o log emite `JANELA_NATIVA_FALHOU` e o texto do atalho muda para `GOVERNO !`. Ao tocar no botão, uma falha de abertura é registrada como `NATIVE_WINDOW_OPEN_FAILED`.
- Não há patches Harmony no Observatório. A janela nativa de status volta a seguir o comportamento do jogo sem intervenção deste mod; isso não constitui correção da NullReference original.

## Limitações conhecidas

- O usuário informou que na 0.6.0-dev o botão não apareceu e o erro do status continuou.
- Esta revisão tenta eliminar uma falha de ordem de inicialização existente: antes, a criação da janela era chamada antes do atalho, então uma exceção na janela impedia o botão de ser construído.
- A janela e o botão 0.6.1-dev ainda não foram confirmados no telefone.
- O CI usa stubs para verificar a estrutura do código; não valida a UI em runtime.
- A compilação com as DLLs reais do aparelho não foi executada neste runner, pois não contém as referências privadas.
- As ações de Governo/Diplomacia/Reuniões/RPG continuam armazenando escolhas locais de sessão; não modificam entidades/relações/guerras nativas.

## Como diagnosticar no aparelho

Após iniciar o mundo, procure `NATIVE_WINDOW_BUTTON_CREATED`. Se houver `CANVAS_MAIN_NULL`, `CANVAS_UI_NULL` ou `CANVAS_UI_INACTIVE`, o log informa por que o botão não foi inserido. Ao tocar no botão, qualquer exceção de abertura aparece em `NATIVE_WINDOW_OPEN_FAILED`; falha de construção aparece em `JANELA_NATIVA_FALHOU`.

Para a exceção do status, a fonte desta versão já não a intercepta. É necessário o log atualizado que contenha a stack trace do erro após reproduzi-lo nesta versão; o antigo pacote de referência não é suficiente para provar a causa em execução.

## Download

ZIP de código-fonte: https://github.com/Jin27012007/Worldbox-mod-geo/archive/refs/heads/scaffold/observatorio-dos-reinos.zip
