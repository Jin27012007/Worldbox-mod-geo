# Observatório dos Reinos — WorldBox Android

Mod original 2D para expandir o controle nativo de personagens com governo e diplomacia. Alvo informado pelo usuário: WorldBox Android 0.51.4 com NeoModLoader.

## `0.5.0-dev` — UI e recuperação de status
- Troca o protótipo `OnGUI` por uma interface de Canvas/Unity UI (deve ser mais confiável em touch e em retrato).
- Painel com Governo, Diplomacia, Reuniões e RPG, campos de texto, toggles, rolagem e botões de rascunho.
- Tenta inserir um botão `GOV` imediatamente acima do botão nativo que abre as informações do personagem/reino, reutilizando a imagem do botão original. Se não localizar essa hierarquia, há um botão reserva `GOVERNO` no alto da tela.
- Tenta reutilizar o sprite de fundo da janela nativa para manter a aparência do jogo.
- Tenta interceptar somente a `NullReferenceException` conhecida no `StatsWindow.OnEnable` quando a stack passa por `showWhoIsSelected`/`showWorldTipSelected`; fecha a janela parcial e abre o painel alternativo.

## Diagnóstico do erro conhecido
A captura do usuário mostrou `NullReferenceException` durante `PowerTabController.showWhoIsSelected(String, Boolean)` chamado na abertura da `StatsWindow` pelo botão de informações. Isso localiza a falha no caminho da UI de status nativa. A assembly pública extraída do ZIP que recebemos não contém esses mesmos nomes (ela usa `showWorldTipSelected` e `SelectedPowerTabTopButtons`), então o objeto exato nulo na build executada ainda precisa ser confirmado. O patch de `0.5.0-dev` é uma recuperação direcionada da UI, não prova de correção da causa interna.

## Como testar
1. Faça backup da pasta atual do mod.
2. Baixe o ZIP do ramo `scaffold/observatorio-dos-reinos` e substitua apenas os arquivos do Observatório dos Reinos.
3. Abra um mundo, espere surgir o botão `GOV` acima do botão de informações do personagem/reino. Se não for possível localizar a barra nativa, procure o botão reserva `GOVERNO` no canto superior esquerdo.
4. Toque no botão, teste as quatro abas, campos de texto, toggles e botões em orientação vertical e horizontal.
5. Teste o botão nativo de status do reino novamente. Se a exceção for interceptada, a janela parcial deve fechar e o painel do Observatório deve abrir no lugar.
6. Se nada aparecer, envie as linhas de log após `Observatorio dos Reinos carregado`. Se o erro persistir, envie a stack trace completa.

## Limites honestos
- O código desta versão ainda precisa ser compilado/carregado e validado no Android.
- As opções ainda são demonstrações locais; não alteram o mundo.
- A detecção do governante possuído e a exibição de dados reais de reino/cidade continuam pendentes.
- Não incluir DLLs do jogo/loader nem reutilizar código ou assets de Worldfall.
