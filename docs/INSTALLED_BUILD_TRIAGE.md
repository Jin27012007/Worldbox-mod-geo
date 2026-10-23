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

## Estado do artefato no GitHub

Na branch `scaffold/observatorio-dos-reinos`, a listagem da raiz contém `src/ObservatorioMod.cs`, `mod.json`, documentação e testes, mas não contém uma DLL compilada do mod. O workflow `build-source.yml` primeiro compila contra stubs isolados e só tenta a compilação com referências reais se as DLLs privadas estiverem em `references/`; se elas faltarem, a etapa é pulada.

Também foi consultado o run de CI `39872165432`: o job concluído com sucesso se chama `Probe logic tests (not a game-runtime test)`, as etapas compilam e executam testes doubles e não há artefato publicado nessa execução. Logo, o resultado verde não fornece um instalador do mod nem prova compatibilidade no aparelho.

Esse é um bloqueio de empacotamento, separado do defeito visual do botão. Para uma correção funcional testável, primeiro precisamos de uma build compilada com as referências exatas do WorldBox 0.51.4 e do NeoModLoader instalado. Essas DLLs não devem ser publicadas no repositório público; a compilação deve ocorrer num ambiente privado/local que já as possua.
