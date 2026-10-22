# Observatório dos Reinos — WorldBox Android

Mod 2D original que pretende acrescentar governo/diplomacia ao controlar um rei/rainha. Alvo informado: WorldBox Android 0.51.4 com NeoModLoader.

## 0.5.1-dev
- Menu Unity Canvas com abas Governo, Diplomacia, Reuniões e RPG.
- Tenta localizar o botão nativo de status e inserir `GOV` acima dele; o atalho reserva `GOVERNO` permanece disponível quando o botão nativo não é identificado.
- Usa `Screen.safeArea` para manter Canvas e controles dentro da área visível ao girar o aparelho.
- Mostra uma faixa de diagnóstico (`OBSERVATORIO 0.5.1-dev`, resolução, área segura, Canvas ativo e botão nativo encontrado) e registra quando o fallback de `StatsWindow` é chamado.
- O fallback intercepta somente a `NullReferenceException` com stack relacionada a `showWhoIsSelected`/`showWorldTipSelected` e abre o painel alternativo. Isso é recuperação da interface, não correção comprovada do campo nulo do jogo.

## Evidência atual
A captura de log do usuário confirma a falha na UI nativa do status do reino: `PowerTabController.showWhoIsSelected` durante `StatsWindow.OnEnable`, aberto por `SelectedPowerTabButtons.clickOpenMain`. A assembly publicizada enviada não bate exatamente com os nomes da stack ao vivo, por isso a causa interna exata ainda é incerta. A UI das versões anteriores não apareceu no aparelho; esta revisão tem diagnósticos visíveis para saber se o código está realmente sendo executado.

## Teste
1. Faça backup da pasta atual do mod e instale o ZIP deste branch.
2. Em retrato, procure a faixa `OBSERVATORIO 0.5.1-dev` e o botão `GOVERNO`. Abra o painel.
3. Gire para paisagem e confirme que a faixa/atalho ainda aparecem e que o painel pode ser aberto.
4. Toque no botão nativo de status do reino. Se a proteção executar, o log deverá conter `FINALIZER ACIONADO` e o painel alternativo deverá abrir.
5. Envie capturas nos dois sentidos e as mensagens do log caso falhe.

## Limitações honestas
O código foi revisado estruturalmente, mas não foi compilado independentemente nem validado nesta versão no dispositivo. Os campos e botões ainda criam rascunhos locais e não alteram dados reais do mundo. A detecção do rei/reino ainda precisa ser ligada à API correta. Não incluir DLLs do jogo/loader nem código/assets do Worldfall.
