# Protocolo de teste Android — 0.6.2-dev

ZIP de código-fonte: https://github.com/Jin27012007/Worldbox-mod-geo/archive/refs/heads/scaffold/observatorio-dos-reinos.zip

1. Faça backup da pasta do Observatório instalada e substitua-a pelos arquivos deste ZIP, mantendo `mod.json` na raiz.
2. Inicie um mundo e selecione um NPC/unidade para que a interface nativa de controles apareça.
3. Procure um botão `GOV` no mesmo grupo visual dos botões nativos de posse/espectador/abertura de informações. O botão global no canto superior esquerdo foi removido.
4. O botão deve aparecer enquanto `SelectedUnit.isSet()` ou `SelectedObjects.isNanoObjectSet()` estiver ativo e desaparecer quando não houver seleção controlável.
5. Toque em `GOV`. A janela nativa Observatório dos Reinos deve abrir; navegue por Governo, Diplomacia, Reuniões e RPG.
6. Se não aparecer, copie do log a linha mais recente entre `NATIVE_CONTROL_BUTTON_CREATED`, `PAINEL_NPC_AGUARDANDO`, `PAINEL_NPC_ENCONTRADO_MAS_BOTAO_NAO_INSERIDO`, `NATIVE_CONTROL_BUTTON_FAILED` e `SELECAO_REFLECTION_FALHOU`.
7. Abra a janela de status nativa separadamente. O Observatório não deve instalar patches Harmony nela. Se a mesma exceção continuar, guarde a stack trace atual; a retirada da intervenção do mod não prova que o erro do jogo foi corrigido.
8. Repita em retrato e paisagem.

## O que o CI confirma

O CI compila a fonte contra stubs e testa invariantes do modelo político local. Isso não valida a UI em Unity em runtime nem a compilação contra as DLLs reais do WorldBox Android 0.51.4.
