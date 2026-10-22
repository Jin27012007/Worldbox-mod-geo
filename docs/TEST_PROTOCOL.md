# Protocolo de teste Android — 0.6.1-dev

ZIP de código-fonte: https://github.com/Jin27012007/Worldbox-mod-geo/archive/refs/heads/scaffold/observatorio-dos-reinos.zip. Faça backup da versão instalada antes de substituir; mantenha `mod.json` na raiz do diretório do mod. A pasta `tests/` fica excluída do compilador de mods por `.gitignore`.

1. Inicie o WorldBox e carregue um mundo.
2. No log, procure `NATIVE_WINDOW_BUTTON_CREATED`. O botão esperado fica no canto superior esquerdo da UI, com fundo verde e texto dourado `GOVERNO`.
3. Se o botão não aparecer, procure `CANVAS_MAIN_NULL`, `CANVAS_UI_NULL`, `CANVAS_UI_INACTIVE` ou `NATIVE_WINDOW_BUTTON_FAILED`. Eles distinguem a espera pelo Canvas do erro na criação do botão.
4. Toque no botão. A janela deve abrir dentro do sistema nativo de janelas do WorldBox. Se a construção falhar, o botão deve continuar visível e exibir `GOVERNO !`; o log deve conter `JANELA_NATIVA_FALHOU` ou `NATIVE_WINDOW_OPEN_FAILED`.
5. Teste as quatro abas, registre uma proposta, uma pauta e um evento narrativo.
6. Abra a janela de status do rei/unidade separadamente. O Observatório não instala Harmony nela nesta revisão. Se a exceção continuar, salve o log atual contendo a stack completa.
7. Repita em retrato e paisagem.

## Limite do CI

O CI compila contra stubs e valida o modelo político local, não a visibilidade da UI na Unity em runtime. A compilação com as DLLs reais e a aceitação no dispositivo continuam pendentes.
