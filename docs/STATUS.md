# Status técnico — 0.5.0-dev

## Evidência recebida do Android (WorldBox 0.51.4)
- O usuário consegue possuir/controlar um rei.
- Ao abrir a janela nativa de status do reino, a janela fica cortada e o log repete `NullReferenceException`.
- Stack trace fornecido: `PowerTabController.showWhoIsSelected(String, Boolean)` -> `StatsWindow.OnEnable()` -> `WindowMetaGeneric<T>.OnEnable()` -> `GameObject.SetActive` -> `ScrollWindow.show(...)` / `PowerTabLibrary.defaultMainInfoClick(...)` -> `SelectedPowerTabButtons.clickOpenMain()` -> `EventSystem.Update()`.
- O erro aparece no caminho de abertura da janela de status, associado à referência nula no método de seleção/status. O aviso de `discord_game_sdk` é separado.
- O arquivo `Assembly-CSharp-Publicized.dll` no pacote de jogo que foi possível inspecionar não corresponde exatamente à pilha mostrada pelo aparelho: ele contém `showWorldTipSelected` e `SelectedPowerTabTopButtons`, enquanto o log atual mostra `showWhoIsSelected` e `SelectedPowerTabButtons`. Portanto, a causa imediata (qual campo está nulo na runtime exata) ainda não está provada; não marcar como correção definitiva.

## Mudanças em 0.5.0-dev
- Substituí o overlay baseado em `OnGUI` por uma UI real de Unity Canvas/Unity UI, porque a versão anterior nunca apareceu visivelmente para o usuário.
- O painel é Screen Space Overlay com ordem alta, escala ajustável a retrato/paisagem, quatro abas (Governo, Diplomacia, Reuniões, RPG), `InputField`, `Toggle`, botões e mensagens de rascunho.
- Tenta reutilizar sprites de botão e painel das janelas nativas do jogo, quando encontradas.
- Tenta localizar `SelectedPowerTabButtons` ou `SelectedPowerTabTopButtons` e inserir um botão `GOV` imediatamente antes/acima do botão nativo de abrir informações. Quando não acha a hierarquia nativa, oferece um botão reserva `GOVERNO` no alto da tela.
- Adiciona via reflexão uma finalizer Harmony para `StatsWindow.OnEnable`. Ela só intercepta `NullReferenceException` quando a stack contém `showWhoIsSelected` ou `showWorldTipSelected`; fecha a janela nativa parcialmente aberta e mostra o painel de administração alternativo. Essa é uma recuperação segura da interface, não prova de que a referência nula original foi corrigida.

## O que ainda precisa de teste
- A revisão 0.5.0-dev ainda não foi compilada externamente nem validada no aparelho.
- Confirmar no log `UI administrativa via Canvas criada`, `Botao de governo inserido acima do botao nativo...` e `Protecao de fallback instalada...`.
- Confirmar que aparece o botão GOV junto ao botão de informações. Se não encontrar a hierarquia nativa, usar o botão reserva no alto da tela.
- Ao acionar o status nativo, confirmar que a exceção conhecida não se repete e o painel alternativo abre. Se o problema persistir, a runtime real continua diferente da assembly fornecida ou a assinatura do método Harmony não foi localizada.
- Testar todas as abas, campos e toque em retrato e paisagem.

## Limites do protótipo
- As opções são apenas rascunhos locais; ainda não alteram políticas, tesouro, unidades, cidades, alianças ou guerras.
- O menu ainda não detecta de forma confirmada o rei possuído nem mostra dados reais do reino.
- Ainda não distribuímos assets do jogo; os sprites nativos são lidos do próprio jogo instalado.
- Não incluir DLLs do jogo ou loader nem reutilizar código/asset de Worldfall.
