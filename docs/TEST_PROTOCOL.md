# Protocolo de teste Android — 0.6.0-dev

ZIP de código-fonte: https://github.com/Jin27012007/Worldbox-mod-geo/archive/refs/heads/scaffold/observatorio-dos-reinos.zip. Extraia no diretório de mods do NeoModLoader e garanta que a pasta do projeto contenha o `mod.json` na raiz. O `tests/` é excluído pelo `.gitignore` para que os stubs não sejam compilados como parte do mod.

1. Faça backup da versão instalada do Observatório.
2. Instale/compile esta revisão da branch `scaffold/observatorio-dos-reinos` usando as referências adequadas ao WorldBox Android 0.51.4 + NeoModLoader.
3. No log, procure `NATIVE_WINDOW_CREATED`, `NATIVE_WINDOW_BUTTON_CREATED` e `JANELA_NATIVA_PRONTA`.
4. Confirme que o botão `GOVERNO` aparece na interface do jogo. Toque nele e verifique se abre uma janela no sistema nativo do WorldBox.
5. Teste as quatro abas; registre uma proposta de lei, uma proposta diplomática, uma pauta e um evento narrativo. Os textos de status devem responder a cada ação.
6. Feche a janela usando o controle nativo e, separadamente, abra a janela de status de uma unidade/rei. O Observatório não deve emitir logs de instalação de Harmony nem interceptar essa janela.
7. Repita em retrato e paisagem. Registre screenshots e o log se o botão/janela não aparecer ou se a janela de status continuar falhando.

## O que este teste comprova

- A interface só é aceita se estiver visível e interativa no jogo real.
- O teste de CI usa stubs e valida invariantes da lógica política local. Ele não demonstra compatibilidade runtime, não produz por si só uma DLL instalável e não substitui o teste do aparelho.
- As ações apresentadas ainda são registros locais de sessão. Nenhuma alteração do reino nativo deve ser esperada nesta revisão.
