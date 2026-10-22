# Status técnico — Observatório dos Reinos 0.5.6-dev

## Estado do build
- O GitHub Actions compilou o fonte da revisão anterior contra stubs Unity/NeoModLoader: `Build succeeded`, 0 warnings, 0 errors. Execução: https://github.com/Jin27012007/Worldbox-mod-geo/actions/runs/39844184218
- O passo de build com referências reais foi ignorado corretamente, pois o runner público não tem os DLLs listados em `references/`. O código **ainda não é uma DLL instalável compilada contra o runtime real**.
- O fonte mudou para 0.5.6-dev depois daquela execução; um novo CI será acionado para validar as mudanças atuais.

## Evidência do dispositivo
- A stack trace do usuário mostra `NullReferenceException` em `PowerTabController.showWhoIsSelected(String, Boolean)` durante `StatsWindow.OnEnable()`, a partir de `SelectedPowerTabButtons.clickOpenMain()`.
- A janela oficial do status fica parcialmente aberta; em landscape, interfaces anteriores desapareceram.
- A versão atual ainda não foi testada no telefone.

## Divergência comprovada nas assemblies
Consulte `docs/ASSEMBLY_INVESTIGATION.md` para hashes, MVIDs e análise do IL. O pacote enviado contém `StatsWindow.OnEnable()` e `PowerTabController.showWorldTipSelected(String, Boolean)`, mas os identificadores `showWhoIsSelected` e `SelectedPowerTabButtons` do log do aparelho não aparecem em nenhuma dessas assemblies. Portanto, o pacote de referência não corresponde exatamente à implementação/runtime que originou a stack trace; não é possível identificar o campo nulo exato até comparar as assemblies que o jogo realmente carregou.

## Mudanças em 0.5.6-dev
- Prefix Harmony preventivo para `showWhoIsSelected`/`showWorldTipSelected` com assinatura `(string,bool)`, tentando ignorar a atualização auxiliar somente quando acionada durante `StatsWindow.OnEnable`/`WindowMetaGeneric.OnEnable`. Chamadas de outras origens seguem normalmente.
- Finalizer de segunda barreira para a exceção conhecida; procura fechar o `ScrollWindow` ancestral completo antes de abrir o painel alternativo.
- Proteção Harmony tentada antes da criação da interface.
- Canvas agora é raiz persistente de tela cheia; `SafeAreaRoot` filho é ajustado por `Screen.safeArea` pelo controlador principal quando há rotação; botão reserva `GOVERNO` permanece disponível.
- Logs reportam `CANVAS_CREATED`/`CANVAS_NOT_CREATED`, estado de instalação dos patches e `Runtime assembly fingerprint` (FullName + MVID) de `StatsWindow`, `PowerTabController` e Harmony.
- A versão do banner de diagnóstico foi sincronizada com `0.5.6-dev`.

## Próximo gate de validação
1. Consultar CI do SHA atual; o último resultado anterior não valida 0.5.6-dev.
2. No telefone, confirmar a faixa `OBSERVATORIO 0.5.6-dev`, o atalho `GOVERNO`, `CANVAS_CREATED`, `STATUS_INFO_PREFIX_INSTALLED` e `STATUS_WINDOW_FINALIZER_INSTALLED`.
3. Ao abrir status, procurar `STATUS_INFO_PREFIX_SKIP`; se falhar, guardar o log completo e a linha `Runtime assembly fingerprint`. Comparar os MVIDs do aparelho com `docs/ASSEMBLY_INVESTIGATION.md` para saber qual DLL precisa ser analisada.

## Limites explícitos
- Não há uma DLL final instalável validada nesta rodada.
- O painel político ainda contém ações demonstrativas e não modifica reinos reais.
- DLLs do jogo/Unity/loader ficam fora do repositório público e são ignoradas por Git.