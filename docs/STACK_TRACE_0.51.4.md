# Investigação do stack trace do WorldBox 0.51.4

Data: 2026-10-23  
Base: captura de log enviada pelo usuário (arquivo `1000149719.jpg`).

## Evidência visível na captura

A tela informa:

- Game Version: `0.51.4 (725@a593f)`
- Sistema: Android 13 / API 33, dispositivo SM-A325M, ARM64.
- O jogo está marcado como `Modded: True`.
- A exceção mostrada é `NullReferenceException: Object reference not set to an instance of an object`.
- O frame mais alto visível no stack é `PowerTabController.showWhoIsSelected(String pPowerTabId, Boolean pBar)`.
- A chamada passa por `StatsWindow.OnEnable()`, `WindowMetaGeneric<TMetaObject,TData>.OnEnable()`, `ScrollWindow.showWindow(...)`, `PowerTabLibrary.defaultMainInfoClick(PowerTabAsset)` e `SelectedPowerTabTopButtons.clickOpenMain()`.
- A captura mostra repetição do erro oito vezes.

## O que isso demonstra — e o que não demonstra

A captura confirma que, nessa sessão, o jogo carregou e o componente chamado `SelectedPowerTabTopButtons` participou do caminho que terminou na exceção. Isso dá um alvo concreto para inspeção em vez de procurar indiscriminadamente qualquer botão de NPC.

A captura **não** identifica a variável nula, não prova que o Observatório foi carregado e não prova que o erro foi causado pelo mod. O stack aponta para a abertura da janela principal de informações do jogo. A relação causal com o Observatório permanece desconhecida.

## Consequências para o próximo passo

1. Não usar `showWhoIsSelected` como método para abrir a janela do Observatório. Ele está no caminho de uma exceção e sua assinatura, isoladamente, não revela o significado dos parâmetros nem o objeto nulo.
2. Inspecionar os metadados reais de `SelectedPowerTabTopButtons` e `PowerTabController` da mesma instalação 0.51.4, em especial os campos de referência aos botões, o método `clickOpenMain` e os membros usados por `showWhoIsSelected`.
3. Manter a criação do botão separada da abertura da janela: primeiro confirmar que o mod inicia; depois confirmar que o componente alvo é encontrado; em seguida confirmar que o botão foi criado, está ativo e tem posição visível; por último testar o clique.
4. Não declarar sucesso com base apenas no CI de stubs ou no fato de o jogo estar marcado como modificado.

## Limite atual

A captura é evidência de uma exceção durante a abertura do painel de informações, não um log de inicialização do Observatório. Ainda é necessário correlacionar o horário/stack com os logs do NML e com os assemblies exatos usados pelo jogo. Até isso ser feito, não há base para atribuir a exceção ao mod nem para afirmar que a integração do botão está corrigida.
