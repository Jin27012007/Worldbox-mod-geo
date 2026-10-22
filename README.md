# Observatório dos Reinos — WorldBox Android

Projeto experimental de interface de governo e diplomacia para WorldBox Android 0.51.4 + NeoModLoader.

## 0.6.2-dev — botão no painel de seleção/controle de NPC

A janela administrativa usa `NeoModLoader.api.AbstractWideWindow<T>` e o registro nativo de janelas do WorldBox. O botão global no canto superior esquerdo foi removido.

O mod agora procura, por reflexão, a área de controles de `SelectedPowerTabButtons` ou `SelectedPowerTabTopButtons`. Quando encontra a referência do botão de posse, espectador ou abertura de janela, cria um botão `GOV` como irmão no mesmo contêiner de UI. O botão só fica visível enquanto `SelectedUnit.isSet()` ou `SelectedObjects.isNanoObjectSet()` reporta uma seleção, acompanhando o contexto em que os botões de seleção nativos são usados.

O Observatório **não instala patches Harmony na janela de status**. A remoção desses patches devolve o fluxo de abertura ao jogo, mas não prova que a `NullReferenceException` original foi corrigida.

## A janela administrativa

O botão `GOV` abre uma janela nativa com páginas Governo, Diplomacia, Reuniões e RPG. Os controles de proposta, pauta e evento registram dados locais durante a sessão; eles ainda **não alteram reinos, cidades, diplomacia ou guerras reais**.

## Download

ZIP da branch de desenvolvimento: https://github.com/Jin27012007/Worldbox-mod-geo/archive/refs/heads/scaffold/observatorio-dos-reinos.zip

É um pacote de código-fonte, não uma DLL pré-compilada. Faça backup da versão anterior antes de substituir os arquivos e mantenha `mod.json` na raiz do diretório do mod. O `.gitignore` exclui `tests/` para que os stubs do CI não sejam compilados como parte do mod.

## Verificação e limites

O CI mais recente compila a fonte contra stubs e testa invariantes do modelo político local. Isso verifica a compilação/estrutura do código, mas não comprova que o botão foi encontrado e renderizado dentro do Unity em execução. A compilação com as DLLs reais ainda não ocorreu neste runner, que não possui as referências privadas do aparelho.

Na primeira execução, procure no log `NATIVE_CONTROL_BUTTON_CREATED`. Se o botão não aparecer, procure `PAINEL_NPC_AGUARDANDO`, `PAINEL_NPC_ENCONTRADO_MAS_BOTAO_NAO_INSERIDO` ou `NATIVE_CONTROL_BUTTON_FAILED`.

Guia de exemplos: https://the-official-worldbox-wiki.fandom.com/wiki/Modding_Code_Examples

Referências de janela nativa:
- https://github.com/WorldBoxOpenMods/ModLoader/blob/36a891b5dcd067be43aa12574338d849c2947001/api/AbstractWindow.cs
- https://github.com/WorldBoxOpenMods/ModLoader/blob/36a891b5dcd067be43aa12574338d849c2947001/general/WindowCreator.cs
