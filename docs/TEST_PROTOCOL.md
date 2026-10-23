# Protocolo de teste Android — 0.6.3-dev

ZIP do código-fonte: https://github.com/Jin27012007/Worldbox-mod-geo/archive/refs/heads/scaffold/observatorio-dos-reinos.zip

1. Faça backup da versão instalada e substitua a pasta. Confirme `mod.json` na raiz.
2. Abra o WorldBox e carregue um mundo. Verifique no log que o mod informa `Build=0.6.3-dev`.
3. Selecione um NPC/unidade para que a área de controles apareça. Procure `GOV` junto dos botões nativos.
4. Se não aparecer, o log agora permite identificar o estágio: `NPC_CONTROL_HOOK_SUMMARY` mostra se a classe foi encontrada; `NPC_CONTROL_HOOK_INSTALLED` confirma o gancho no `Update()`; `NATIVE_CONTROL_BUTTON_CREATED` confirma a criação. `NPC_CONTROL_HOOK_FAILED`, `NPC_CONTROL_PANEL_NOT_FOUND` e `NATIVE_CONTROL_BUTTON_FAILED` reportam falhas diferentes.
5. Toque em `GOV`. O log deve mostrar `NPC_CONTROL_BUTTON_CLICKED`; em caso de falha de abertura, procure `NPC_CONTROL_BUTTON_OPEN_FAILED`.
6. A janela nativa de status deve ser testada separadamente; o Observatório não instala patches nessa janela.

## Limite dos testes automatizados

O CI compila contra stubs de Unity/NeoModLoader/Harmony e testa invariantes dos dados políticos. Não executa o jogo nem confirma que o botão é desenhado no runtime do Android.
