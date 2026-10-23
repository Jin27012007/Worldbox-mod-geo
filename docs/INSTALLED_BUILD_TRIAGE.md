# Triagem do pacote observado nas capturas Android

Data: 2026-10-23

## Evidência visível nas capturas

Na tela de Mods, o item instalado aparece como `Observatorio dos Reinos 0.1.0-dev`, com a descrição:

> Teste experimental de carregamento para WorldBox Android; somente diagnóstico.

Outra captura mostra o estado `Current Enabled, won't load`. Isso indica que não devemos assumir que esse pacote chegou a executar no jogo. A captura do jogo também mostra `Modded: True`, mas esse rótulo não identifica quais mods efetivamente carregaram.

## Comparação com os arquivos do repositório

| Origem | Versão declarada | O que promete |
|---|---|---|
| Captura do aparelho | `0.1.0-dev` | Teste de carregamento; somente diagnóstico |
| Branch `foundation/runtime-proof` | `0.1.0-proof` | Diagnóstico de carregamento e UI; não adiciona botões |
| Branch `scaffold/observatorio-dos-reinos` | `0.6.4-dev` | Inclui a tentativa experimental de integrar o botão `GOV` ao painel de NPC |

Os números de versão e as descrições não coincidem. Com a evidência disponível, não é possível concluir que o código `0.6.4-dev` tenha sido compilado, instalado ou executado no aparelho.

## Interpretação correta do stack trace

A exceção `NullReferenceException` passa por `SelectedPowerTabTopButtons.clickOpenMain()` e `PowerTabController.showWorldTipSelected(...)` ao abrir a janela nativa de informações. Ela não demonstra que o Observatório carregou, nem que o botão `GOV` foi criado ou clicado. A exceção separada `DllNotFoundException: discord_game_sdk` também não comprova uma falha do Observatório.

## Próximo ponto de verificação

Antes de mudar a geometria ou a lógica de visibilidade do botão, é necessário confirmar que a sessão testa a build que contém o botão. O registro decisivo é a presença dos logs do próprio mod:

- `[OR-BASELINE] BUILD=0.6.4-dev; STAGE=ON_MOD_LOAD`
- `[OR-BASELINE] BUILD=0.6.4-dev; STAGE=UPDATE_OK`
- `NPC_CONTROL_HOOK_SUMMARY`
- `NPC_CONTROL_HOOK_INSTALLED`
- `NATIVE_CONTROL_BUTTON_CREATED`, ou a mensagem de falha correspondente

Se esses registros não existem, o teste ainda não alcançou o código do botão. Se existem, eles permitem localizar a etapa que falhou. Os testes de CI com stubs não substituem essa confirmação no WorldBox real.

**Conclusão:** as capturas atuais são evidência de uma discrepância de build e de um erro nativo ao abrir informações, não evidência suficiente para declarar defeito ou correção no botão da build `0.6.4-dev`.