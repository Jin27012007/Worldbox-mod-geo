# Observatório dos Reinos — WorldBox Android

Projeto original para construir uma interface de governo e diplomacia para WorldBox Android 0.51.4 + NeoModLoader.

## 0.6.0-dev — janela nativa experimental

A interface foi reescrita para usar `AbstractWideWindow<T>` do NeoModLoader, que cria uma janela baseada no prefab nativo `windows/empty` e a registra no sistema de janelas. O botão `GOVERNO` é filho de `CanvasMain.instance.canvas_ui` e chama `ScrollWindow.showWindow("observatorio_dos_reinos")`.

A nova versão **não instala patches Harmony na janela nativa de status**. Isso devolve ao jogo o fluxo normal de abertura da janela, sem a interceptação de status que o mod anterior tentava fazer. Isso não garante que um bug originado no próprio jogo ou em outro mod tenha sido corrigido.

A janela contém páginas de Governo, Diplomacia, Reuniões e RPG. Os botões de políticas e de propostas atualizam registros locais durante a sessão; eles **ainda não alteram os reinos, relações diplomáticas, guerras ou cidades reais**.

## Verificação

O repositório contém testes de compilação contra stubs e testes de invariantes do modelo político local. Eles não validam a interface Unity em runtime. Não há ainda confirmação visual da janela no telefone nem uma DLL produzida aqui contra a instalação exata do jogo.

## Baixar e instalar como mod de código-fonte

O ZIP desta branch contém o código-fonte, não uma DLL pré-compilada. O NeoModLoader compila os arquivos `.cs` da pasta do mod. O arquivo `.gitignore` exclui `tests/` da compilação dentro do jogo, pois essa pasta contém stubs exclusivos do CI.

Baixe a branch: https://github.com/Jin27012007/Worldbox-mod-geo/archive/refs/heads/scaffold/observatorio-dos-reinos.zip

Extraia a pasta do projeto para o diretório de mods do NeoModLoader, da mesma forma que os demais mods de código-fonte que você já usa. Faça backup da pasta antiga antes de substituir.

Guia de exemplos fornecido na conversa: https://the-official-worldbox-wiki.fandom.com/wiki/Modding_Code_Examples

Referência adicional de janela nativa no código do NeoModLoader: https://github.com/WorldBoxOpenMods/ModLoader/blob/36a891b5dcd067be43aa12574338d849c2947001/api/AbstractWindow.cs e https://github.com/WorldBoxOpenMods/ModLoader/blob/36a891b5dcd067be43aa12574338d849c2947001/general/WindowCreator.cs

## Critério de conclusão

Não considere a mudança confirmada até compilar a versão com as referências reais do aparelho, instalá-la e observar no jogo: (1) o botão GOVERNO, (2) abertura/fechamento da janela nativa, (3) navegação nas quatro páginas e (4) abertura normal da janela de status sem patches do Observatório.
