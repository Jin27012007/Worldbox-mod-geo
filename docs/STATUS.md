# Status técnico — 0.5.1-dev

## Evidência de runtime do usuário
- WorldBox Android 0.51.4, NeoModLoader ativo.
- Ao tocar no status nativo do reino, repete-se `NullReferenceException` na chamada `PowerTabController.showWhoIsSelected(String, Boolean)` dentro de `StatsWindow.OnEnable()`, originada por `SelectedPowerTabButtons.clickOpenMain()`.
- Sair da tela não repara a janela; fica cortada. Na orientação horizontal, painel e janela somem sem um atalho visível.
- A assembly publicizada fornecida diverge no nome de métodos da stack ao vivo. Não sabemos ainda qual campo real está nulo.
- O painel do Observatório das versões anteriores não foi encontrado pelo usuário, logo não pode ser considerado funcional mesmo existindo código-fonte.

## Mudanças em `0.5.1-dev`
- UI foi construída via Canvas/Unity UI, não OnGUI.
- Adicionado layout que restringe o Canvas à `Screen.safeArea`, atualizando após mudanças de orientação.
- O painel passa a usar margens quase completas da área segura.
- O atalho reserva `GOVERNO` fica numa posição mais baixa no topo, evitando a área de barra de status e permanecendo disponível quando o botão nativo não for descoberto.
- Adicionada faixa visível de diagnóstico: versão, resolução, área segura, estado ativo do Canvas e se o botão nativo foi localizado.
- Loga explicitamente a criação da UI e registra a stack capturada se o finalizer for acionado.
- A recuperação Harmony continua limitada à `NullReferenceException` cuja stack inclui `showWhoIsSelected` ou `showWorldTipSelected`; ela fecha o objeto `StatsWindow` e tenta abrir o painel alternativo. Não afirma corrigir o objeto nulo original.

## Resultado da revisão de código
- Inspeção estrutural simples de delimitadores feita; ainda não é uma compilação C#.
- Código não compilado por toolchain independente nem testado em runtime nesta versão.

## Teste obrigatório no Android
1. Instalar com backup e abrir em orientação vertical.
2. Confirmar se aparecem a faixa `OBSERVATORIO 0.5.1-dev`, o botão `GOVERNO` e o painel após clicar nele.
3. Girar para horizontal: a faixa de diagnóstico e o botão de reabertura devem continuar na área segura; testar abrir/fechar.
4. Tocar no status nativo do reino e confirmar se surge `FINALIZER ACIONADO` no log e se o painel alternativo abre.
5. Enviar captura e log se a faixa ou botão não aparecer. Se as mensagens de inicialização não aparecerem, a versão não foi carregada ou houve erro antes de `OnModLoad`.

## Segurança
- As opções do painel são ainda rascunhos visuais; nenhuma entidade nativa é alterada.
- Não incluir DLLs de jogo/loader nem código/assets do Worldfall.
