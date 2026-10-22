# Status técnico — Observatório dos Reinos 0.5.8-dev

## Build e testes automatizados confirmados
- GitHub Actions do commit `8cee81d2dcd4b17e229d995d67fa185d8a184962`: https://github.com/Jin27012007/Worldbox-mod-geo/actions/runs/39850586295
- `dotnet build tests/CompileStubs.csproj`: **Build succeeded, 0 warnings, 0 errors**.
- Smoke test executado com sucesso: `PASS: prefix ativo somente durante o escopo de status; postfix/finalizer restauram o estado.`
- O passo de compilação real detectou que as DLLs não estão presentes no runner público e foi corretamente marcado como ignorado. Portanto, ainda não foi gerada uma DLL instalável e não se afirma compatibilidade completa com a runtime real.
- `tests/CompileWorldBoxRefs.csproj` permite build contra as DLLs locais dentro de `references/`; o projeto agora inclui o conjunto de módulos Unity do diretório e exclui o `Assembly-CSharp.dll` não publicizado para evitar colisão de identidade. `.gitignore` mantém esses binários fora do repositório público.

## Evidência do dispositivo
- WorldBox Android 0.51.4, NeoModLoader presente.
- A stack trace do usuário mostra `NullReferenceException` em `PowerTabController.showWhoIsSelected(String, Boolean)` durante `StatsWindow.OnEnable()`, acionada por `SelectedPowerTabButtons.clickOpenMain()`.
- A janela nativa fica parcialmente aberta; versões anteriores do overlay desapareceram em orientação horizontal.
- A revisão 0.5.8-dev ainda não foi confirmada no telefone.

## Divergência de assembly
Consulte `docs/ASSEMBLY_INVESTIGATION.md` para os hashes, MVIDs e IL inspecionados. As assemblies fornecidas contêm `StatsWindow.OnEnable()` chamando `PowerTabController.showWorldTipSelected(String, Boolean)` e a classe `SelectedPowerTabTopButtons`; a stack trace do aparelho menciona `showWhoIsSelected` e `SelectedPowerTabButtons`, ausentes nessas referências. Assim, a causa exata da referência nula ainda não está identificada, e precisamos comparar o fingerprint das assemblies carregadas no dispositivo.

## Mudanças no código 0.5.8-dev
- Canvas persistente como raiz em tela cheia; `SafeAreaRoot` interno acompanha `Screen.safeArea` ao girar o aparelho.
- O atalho `GOVERNO` permanece disponível, mesmo quando o botão `GOV` nativo é encontrado.
- Harmony é tentado antes da construção visual.
- Prefix ativa um escopo de status via `__state`; a rotina auxiliar de seleção/tooltip é ignorada apenas enquanto `StatsWindow.OnEnable` está em execução. Chamadas de outros contextos continuam normais.
- Postfix e finalizer restauram o estado anterior; o smoke test automatizado comprova esse comportamento.
- Finalizer fica como segunda barreira: tenta interceptar a NullReference conhecida, fechar o `ScrollWindow` ancestral inteiro e abrir o painel alternativo.
- Logs mostram `CANVAS_CREATED`/`CANVAS_NOT_CREATED`, instalação real do prefix/postfix/finalizer e o FullName + MVID das assemblies carregadas.

## Próximo gate
1. No aparelho, confirmar a faixa `OBSERVATORIO 0.5.8-dev` e o atalho `GOVERNO` em retrato e paisagem.
2. No log, conferir `CANVAS_CREATED`, `STATUS_INFO_PREFIX_INSTALLED`, `STATUS_WINDOW_PREFIX_INSTALLED`, `STATUS_WINDOW_POSTFIX_INSTALLED`, `STATUS_WINDOW_FINALIZER_INSTALLED` e `Runtime assembly fingerprint`.
3. Ao abrir o status, procurar `STATUS_INFO_PREFIX_SKIP`. Se persistir a exceção, preservar a stack completa e comparar os MVIDs registrados com `docs/ASSEMBLY_INVESTIGATION.md`.

## Limites
- A compilação validada é contra stubs simulados; não é uma DLL instalável.
- Não há confirmação de runtime no aparelho para 0.5.8-dev.
- O menu de governo ainda contém rascunhos visuais e não modifica reino/diplomacia reais.
- DLLs proprietárias ficam fora do repositório público.