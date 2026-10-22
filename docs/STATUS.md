# Status técnico — Observatório dos Reinos 0.5.9-dev

## Build e testes automatizados confirmados
- GitHub Actions da revisão de código 0.5.9-dev: https://github.com/Jin27012007/Worldbox-mod-geo/actions/runs/39850928817
- `dotnet build tests/CompileStubs.csproj`: **Build succeeded, 0 warnings, 0 errors**.
- Smoke test passou: `PASS: prefix ativo somente durante o escopo de status; postfix/finalizer restauram o estado.`
- O build real foi pulado porque as assemblies privadas não estão no runner público. Não existe ainda uma DLL compilada contra a runtime real nem instalação validada.
- `tests/CompileWorldBoxRefs.csproj` usa as DLLs locais de `references/` e todos os módulos Unity disponíveis, excluindo a cópia não publicizada `Assembly-CSharp.dll` para evitar conflito com a identidade da assembly publicizada. Os binários ficam fora do repositório público.

## Evidência do dispositivo
- WorldBox Android 0.51.4, NeoModLoader instalado.
- O log do usuário mostra `NullReferenceException` em `PowerTabController.showWhoIsSelected(String, Boolean)` durante `StatsWindow.OnEnable()`, iniciado por `SelectedPowerTabButtons.clickOpenMain()`.
- A janela nativa fica parcialmente aberta e, em versões anteriores, os painéis desapareceram ao girar para horizontal.
- A versão 0.5.9-dev ainda não foi confirmada no telefone.

## Divergência de assembly
Consulte `docs/ASSEMBLY_INVESTIGATION.md` para hashes, MVIDs e análise do IL. As assemblies fornecidas mostram `StatsWindow.OnEnable()` chamando `PowerTabController.showWorldTipSelected(String, Boolean)` e a classe `SelectedPowerTabTopButtons`. A stack do aparelho usa `showWhoIsSelected` e `SelectedPowerTabButtons`, ausentes nessas referências. Isso impede identificar qual campo exato está nulo até comparar os MVIDs do jogo realmente carregado.

## Mudanças em 0.5.9-dev
- O Canvas é uma raiz persistente de tela cheia; `SafeAreaRoot` interno acompanha `Screen.safeArea` após rotação; o atalho reserva `GOVERNO` permanece visível.
- A procura de Harmony e HarmonyMethod agora tenta nome qualificado e varredura das assemblies carregadas.
- O fingerprint (FullName + MVID) de `StatsWindow`, `PowerTabController`, Harmony e HarmonyMethod é impresso **antes** das verificações que podem encerrar a instalação do patch. Mesmo que Harmony falhe, deve ficar evidência de quais assemblies foram resolvidas.
- O prefix ignora a rotina auxiliar de seleção/tooltip somente durante o escopo de `StatsWindow.OnEnable`. O postfix e o finalizer restauram o estado anterior.
- Smoke test confirma o escopo do prefix e a restauração por postfix/finalizer.
- O finalizer de segunda barreira tenta fechar o `ScrollWindow` externo inteiro se ocorrer a NullReference conhecida.

## Próximo gate
1. No aparelho, confirmar a faixa `OBSERVATORIO 0.5.9-dev` e o atalho `GOVERNO` em retrato/paisagem.
2. No log, verificar `Runtime assembly fingerprint`, `CANVAS_CREATED`, `STATUS_INFO_PREFIX_INSTALLED`, `STATUS_WINDOW_PREFIX_INSTALLED`, `STATUS_WINDOW_POSTFIX_INSTALLED` e `STATUS_WINDOW_FINALIZER_INSTALLED`.
3. Ao abrir o status, procurar `STATUS_INFO_PREFIX_SKIP`. Se a exceção persistir, comparar os MVIDs do runtime com `docs/ASSEMBLY_INVESTIGATION.md`.

## Limites
- A validação automática é contra stubs: não é DLL instalável e não prova compatibilidade completa com a API real.
- Nenhuma correção foi confirmada no dispositivo.
- As ações administrativas permanecem rascunhos e não alteram os reinos reais.
- As DLLs privadas do jogo/Unity/loader continuam fora do repositório público.