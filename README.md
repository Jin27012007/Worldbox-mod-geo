# Observatório dos Reinos — WorldBox Android

Projeto experimental de interface de governo e diplomacia para WorldBox Android 0.51.4 + NeoModLoader.

## 0.6.1-dev — diagnóstico visível do botão

A interface principal usa `AbstractWideWindow<T>` do NeoModLoader e o registro de janela nativo. A revisão 0.6.1-dev separa a criação do botão `GOVERNO` da criação da janela: mesmo se a janela nativa falhar ao inicializar, o mod deve deixar o atalho visível para permitir diagnóstico. O botão recebe um Canvas próprio com `overrideSorting = true` e `sortingOrder = 32000`, e tenta exibir `GOVERNO !` quando a janela falha.

O Observatório não instala patches Harmony em `StatsWindow` nem intercepta as rotinas da janela nativa de status. Portanto, remover a intervenção do mod não prova que a exceção original do status foi corrigida.

As páginas Governo, Diplomacia, Reuniões e RPG guardam preferências/propostas localmente durante a sessão. Não alteram ainda os reinos, cidades, diplomacia ou guerras reais.

## Download

ZIP do código-fonte para o NeoModLoader: https://github.com/Jin27012007/Worldbox-mod-geo/archive/refs/heads/scaffold/observatorio-dos-reinos.zip

O NeoModLoader compila os arquivos `.cs` no diretório do mod. O `.gitignore` exclui `tests/` para não incluir stubs de CI na compilação do mod. Faça backup da versão anterior antes de substituir os arquivos e mantenha `mod.json` na raiz da pasta do mod.

## Verificação

O CI compila a fonte contra stubs e testa invariantes do modelo político. Isso não verifica UI no Unity em execução, não substitui a compilação contra as referências reais do WorldBox/NeoModLoader e não confirma que o botão aparece no aparelho.

A aceitação real exige ver o botão no jogo, abrir a janela, navegar pelas abas e verificar separadamente o erro da janela de status.

Guia fornecido na conversa: https://the-official-worldbox-wiki.fandom.com/wiki/Modding_Code_Examples

Referências de janela nativa:
- https://github.com/WorldBoxOpenMods/ModLoader/blob/36a891b5dcd067be43aa12574338d849c2947001/api/AbstractWindow.cs
- https://github.com/WorldBoxOpenMods/ModLoader/blob/36a891b5dcd067be43aa12574338d849c2947001/general/WindowCreator.cs
