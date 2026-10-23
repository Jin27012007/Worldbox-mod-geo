# Experimento: regra de visibilidade do atalho NPC

Status: experimental; sem validação dentro do WorldBox real.
Base: `scaffold/observatorio-dos-reinos`.
Branch: `experiment/visible-npc-button`.

## Hipótese testada

A implementação anterior condicionava o `GameObject` do botão à leitura refletida de `SelectedUnit.isSet()` ou `SelectedObjects.isNanoObjectSet()`. Se pelo menos uma dessas leituras funcionasse mas retornasse `false`, o estado de seleção era considerado conhecido e o botão ficava inativo, mesmo que o painel nativo de controles estivesse ativo.

A captura de erro que temos não informa os valores dessas flags e não prova que esse fosse o motivo do botão ausente. A mudança, portanto, é um experimento delimitado, não uma correção confirmada.

## Alteração

A criação e a atualização periódica do botão passam a seguir apenas a hierarquia de UI:

- o painel de controles precisa estar ativo na hierarquia;
- o objeto pai do botão precisa estar ativo na hierarquia;
- a leitura de seleção permanece nos logs como diagnóstico, mas não bloqueia a visibilidade.

O registro de criação inclui `regra_visibilidade=painel+pai`, para ser possível identificar a regra em uso. Nenhum método da janela de informações nativa é chamado como atalho para abrir o Observatório.

## Como interpretar um teste futuro

- Se houver `NATIVE_CONTROL_BUTTON_CREATED` e `regra_visibilidade=painel+pai`, mas o botão não estiver visível, a hipótese da flag de seleção fica menos provável; o próximo foco passa a ser hierarquia/layout/posição/clipping.
- Se houver `NPC_CONTROL_HOOK_SUMMARY` sem `NATIVE_CONTROL_BUTTON_CREATED`, a falha ocorre antes ou durante a descoberta/inserção do botão.
- Se não houver `[Observatorio]` nem `[OR-BASELINE]`, ainda não há prova de que esse source foi carregado.
- Se o botão aparecer em contextos não desejados, esta regra é ampla demais e deve ser refinada com o estado real do painel, não com uma flag de seleção especulativa.

## Limites

A CI compila contra doubles de Unity/NML e executa testes do modelo local. Isso não prova compatibilidade binária real, renderização, posição ou clique em WorldBox Android 0.51.4. Não mesclar como correção final sem evidência do jogo.
