# Observatório dos Reinos — WorldBox Android

Projeto original para acrescentar governo/diplomacia ao controle nativo de rei/rainha. Alvo informado: WorldBox Android 0.51.4 + NeoModLoader.

## 0.5.5-dev — mudanças principais
- Corrigida a arquitetura do Canvas: o Canvas persistente é a raiz e `SafeAreaRoot` é um filho dimensionado por `Screen.safeArea`. A versão anterior aplicava âncoras de safe area diretamente no objeto Canvas, o que poderia provocar sumiço/escala errada ao girar a tela.
- A proteção Harmony agora é tentada antes de construir a UI, então falha visual não deve impedir a proteção de status.
- Prefix preventivo tenta ignorar somente `showWhoIsSelected`/`showWorldTipSelected` chamado na cadeia de `StatsWindow.OnEnable`, permitindo que a janela termine de inicializar sem atualizar a dica auxiliar naquele momento. Fora desse contexto, deixa a rotina original funcionar.
- Finalizer de `StatsWindow.OnEnable` permanece como segunda barreira; logs distinguem prefix/finalizer instalados e acionados.
- O atalho `GOVERNO` fica visível mesmo que o botão `GOV` seja inserido na barra nativa.

## Inspeção real da assembly enviada
O IL da `Assembly-CSharp.dll` do pacote de referência contém `StatsWindow.OnEnable` e `PowerTabController.showWorldTipSelected(String, Boolean)`. A abertura do status chama a rotina de seleção/tooltip e esta consulta o objeto selecionado, biblioteca de power tabs e callback de tooltip. Isso torna plausível evitar essa atualização auxiliar apenas durante a inicialização do status. Porém o log real do aparelho menciona `showWhoIsSelected`, que não existe na assembly entregue, então a causa nula exata ainda não está identificada.

## Estado de compilação
- A verificação anterior contra stubs C# passou com 0 erros e 0 avisos.
- A revisão 0.5.5-dev precisa de sua própria execução do CI.
- Isso não é uma DLL instalável nem uma compilação contra a API exata de WorldBox/NML. As DLLs proprietárias não são incluídas no repositório público.

## Teste dirigido
1. Faça backup e instale o ZIP desta branch.
2. Em retrato e paisagem, confirmar a faixa `OBSERVATORIO 0.5.5-dev` e o atalho `GOVERNO`.
3. Verificar os logs `STATUS_INFO_PREFIX_INSTALLED`, `STATUS_WINDOW_FINALIZER_INSTALLED` e `CANVAS_CREATED`.
4. Ao abrir status do reino, procurar `STATUS_INFO_PREFIX_SKIP`. A janela deve terminar de inicializar. Se der erro, preservar o log completo para distinguir falha de patch e exceção nativa.

## Limites
As opções de administração são ainda rascunhos, sem alterações reais de reinos ou diplomacia. A correção precisa ser confirmada no aparelho. Não incluir DLLs do jogo/loader nem copiar código/assets de Worldfall.