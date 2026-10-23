# Investigação do stack trace do WorldBox 0.51.4

Data: 2026-10-23  
Base: captura de log enviada pelo usuário (arquivo `1000149719.jpg`).

## Evidência visível na captura

A tela informa:

- Game Version: `0.51.4 (725@a593f)`.
- Android 13 / API 33, dispositivo SM-A325M, ARM64.
- `Modded: True`.
- Antes do erro de UI, aparece uma `System.DllNotFoundException` para `discord_game_sdk`, no caminho de inicialização da integração do Discord.
- A exceção de UI é `NullReferenceException: Object reference not set to an instance of an object`.
- O frame da exceção visível é `PowerTabController.showWorldTipSelected(String pPowerTabId, Boolean pBar)`.
- A chamada passa por `StatsWindow.OnEnable()`, `WindowMetaGeneric<TMetaObject,TData>.OnEnable()`, `ScrollWindow.showWindow(...)`, `PowerTabLibrary.defaultMainInfoClick(PowerTabAsset)` e `SelectedPowerTabTopButtons.clickOpenMain()`.
- O log diz que os erros anteriores se repetiram oito vezes.
- O identificador de assembly mostrado no stack (`c8e357a15932475b92df58e3db43b9da`) corresponde ao MVID da `Assembly-CSharp.dll` do pacote analisado.

## O que isso demonstra — e o que não demonstra

A captura confirma que `SelectedPowerTabTopButtons.clickOpenMain()` participou do caminho que terminou na exceção e que a falha ocorre quando a janela de informações é aberta. A DLL da mesma versão contém os tipos e métodos citados no stack, então há correspondência entre a evidência e a assembly examinada.

A captura **não** identifica qual objeto está nulo, não prova que o Observatório foi carregado e não prova que o mod causou a exceção. O erro de `discord_game_sdk` aparece em outro caminho e não deve ser tratado como causa da falha de UI sem evidência adicional.

## Consequências para o próximo passo

1. Não usar `PowerTabController.showWorldTipSelected` como método para abrir a janela do Observatório. A captura mostra uma exceção nesse caminho, e a assinatura não revela qual referência estava nula.
2. Inspecionar os membros reais de `SelectedPowerTabTopButtons` e `PowerTabController` na assembly 0.51.4; ver `docs/UI_CONTRACT_0.51.4.md`.
3. Separar os estágios de diagnóstico: confirmar que o mod inicia; confirmar que o hook de `SelectedPowerTabTopButtons.Update()` foi instalado; confirmar que o objeto do painel e os botões âncora existem; confirmar que o novo botão foi criado, ativo e em posição visível; só então testar o clique.
4. Não declarar sucesso com base apenas no CI de stubs ou no fato de o jogo estar marcado como modificado.

## Limite atual

A captura é evidência de uma exceção durante a abertura do painel de informações, não um log de inicialização do Observatório. Ainda é necessário correlacionar a sessão com os logs do NeoModLoader, especialmente linhas `[OR-BASELINE]` e `[Observatorio]`. Até isso ser feito, não há base para atribuir a exceção ao mod nem para afirmar que a integração do botão está corrigida.
