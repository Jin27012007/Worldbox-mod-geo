# Teste Android — `0.5.0-dev`

## Antes de atualizar
- Faça backup da pasta atual do Observatório dos Reinos e mantenha um mundo descartável.
- Baixe o ZIP do ramo `scaffold/observatorio-dos-reinos`. Não altere a pasta do Worldfall.

## UI do Observatório
1. Abra o WorldBox e carregue um mundo.
2. Procure um pequeno botão `GOV` imediatamente acima do botão nativo de informações. Se a barra nativa não tiver sido localizada, procure o botão reserva `GOVERNO` no canto superior esquerdo.
3. Toque nele. A janela deve aparecer por cima do jogo, adaptada à tela, com abas Governo, Diplomacia, Reuniões e RPG.
4. Teste a rolagem, os campos de texto, toggles, botões e a alternância de abas.
5. Repita em retrato e paisagem.

## Fallback da janela nativa de status
1. Possua/controle um rei como nos testes anteriores.
2. Toque no botão nativo de informações/status do reino.
3. Se a falha conhecida reaparecer como `NullReferenceException` dentro do caminho `showWhoIsSelected` ou `showWorldTipSelected` durante `StatsWindow.OnEnable`, o patch tenta fechar a janela nativa incompleta e abrir o painel alternativo, sem propagar essa exceção conhecida.
4. Outros tipos de exceção não são suprimidos.

## Logs úteis
- `UI administrativa via Canvas criada`
- `Protecao de fallback instalada para erro NullReference no status do reino`
- `Botao de governo inserido acima do botao nativo de informacoes`
- `Interceptado NullReference na UI nativa de status`

## Interpretação
- Se o mod não carregar ou se as mensagens do Observatório não aparecerem, a revisão atual não foi compilada/executada ou houve erro anterior ao `OnModLoad`.
- Se o fallback do status não aparecer, enviar a stack trace completa: os nomes do assembly público que recebemos não coincidem exatamente com os da stack atual do aparelho.
- O menu é visual; nenhuma das opções altera reinos, cidades, unidades, guerra ou diplomacia nesta fase.
