# Status técnico — Observatório dos Reinos 0.6.2-dev

## Mudanças desta revisão

- Removido o botão administrativo no canto superior esquerdo; o objetivo agora é inserir `GOV` no mesmo grupo visual dos controles nativos de seleção/posse.
- A rotina procura `SelectedPowerTabButtons` e `SelectedPowerTabTopButtons`, com suporte a nomes de campos `possession/possess`, `spectate`, `open_window/open_main/info` e fallback por nome de objetos filhos.
- O novo botão usa como referência o layout e a imagem de um botão nativo e é inserido como irmão no mesmo contêiner.
- A visibilidade é sincronizada com os métodos refletidos `SelectedUnit.isSet()` e `SelectedObjects.isNanoObjectSet()`, usados pela UI nativa de seleção. Não deve ficar visível sem uma seleção controlável.
- A janela de governo continua sendo `AbstractWideWindow<ObservatorioJanelaNativa>`.
- O Observatório não instala patches Harmony para `StatsWindow`, `showWhoIsSelected` ou `showWorldTipSelected`. O mod não altera a abertura da janela nativa de status.

## Logs para verificar no dispositivo

- `NATIVE_CONTROL_BUTTON_CREATED`: o botão foi inserido; a mensagem indica classe, campo de referência e pai do botão.
- `PAINEL_NPC_AGUARDANDO`: as classes de controle ainda não foram encontradas na cena.
- `PAINEL_NPC_ENCONTRADO_MAS_BOTAO_NAO_INSERIDO`: o componente foi localizado mas não foi possível descobrir/inserir o botão.
- `NATIVE_CONTROL_BUTTON_FAILED`: ocorreu uma exceção ao construir o botão.
- `SELECAO_REFLECTION_FALHOU`: não foi possível consultar o estado da seleção.

## Validação

- GitHub Actions do fonte atualizado: https://github.com/Jin27012007/Worldbox-mod-geo/actions/runs/39858614380
- Build contra stubs: aprovado com 0 warnings e 0 errors.
- Teste automatizado: invariantes do modelo político local aprovados.
- Build contra as DLLs reais: não executado pelo runner público, pois as referências privadas não estão presentes.
- A nova posição do botão e sua interação ainda **não foram confirmadas no telefone**.

## Limitações funcionais

A janela tem páginas Governo, Diplomacia, Reuniões e RPG. Os botões registram preferências e propostas localmente na sessão; ainda não alteram o estado real dos reinos. A remoção dos patches do Observatório permite que a janela de status siga o fluxo original, mas não prova que a exceção relatada foi causada por esses patches nem que a causa do jogo tenha sido corrigida.

## Download de código-fonte

https://github.com/Jin27012007/Worldbox-mod-geo/archive/refs/heads/scaffold/observatorio-dos-reinos.zip
