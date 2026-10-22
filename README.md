# Observatório dos Reinos — WorldBox Android

Projeto original para adicionar governo e diplomacia ao controle nativo de rei/rainha. Alvo informado pelo usuário: WorldBox Android 0.51.4 + NeoModLoader.

## `0.5.2-dev`
A revisão mantém um Canvas Unity UI com quatro abas de protótipo e acrescenta diagnóstico visível do estado da UI: resolução, área segura, Canvas ativo, botão nativo localizado e fallback visível. Um finalizer Harmony tenta interceptar apenas a NullReferenceException cujo stack contém `showWhoIsSelected` ou `showWorldTipSelected` durante `StatsWindow.OnEnable`, fechar a janela nativa incompleta e abrir o painel alternativo.

## O que o log realmente comprovou
A captura do usuário mostra repetidamente `NullReferenceException` em `PowerTabController.showWhoIsSelected(String, Boolean)` chamado durante `StatsWindow.OnEnable()`, via `SelectedPowerTabButtons.clickOpenMain()`. As versões anteriores não fizeram o painel/fallback aparecer no dispositivo. A DLL publicizada fornecida tem nomes diferentes, então ainda não se sabe qual objeto runtime está nulo, nem se o patch Harmony foi instalado/acionado na revisão testada.

## Próximo teste — obrigatório antes de outro patch visual
1. Faça backup e instale o ZIP da branch `scaffold/observatorio-dos-reinos`.
2. Confirme se o banner `OBSERVATORIO 0.5.1-dev`/`0.5.2-dev` aparece.
3. Abra o log desde o carregamento e procure `Canvas, botao reserva e painel admin prontos`, `Protecao de fallback instalada`, `UI administrativa via Canvas criada`.
4. Ao abrir o status do reino, verifique `FINALIZER ACIONADO`.
5. Se as mensagens de carregamento faltarem, o código não está efetivamente executando; corrigir a cadeia de instalação/compilação antes de alterar layout. Se aparecem mas o finalizer nunca roda, conferir assinatura/método real do Harmony contra a assembly exata.

## Limitações
- Não afirmar que o painel está funcionando até ser visto no telefone.
- O campo de rei/reino real e as ações políticas continuam não integrados.
- Botões atuais não alteram as entidades do mundo.
- Não incluir DLLs do jogo ou loader; não copiar código/assets de Worldfall.
