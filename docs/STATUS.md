# Status técnico — Observatório dos Reinos 0.5.8-dev

## Build confirmado
- GitHub Actions executou `dotnet build tests/CompileStubs.csproj` na revisão com banner 0.5.8-dev e restauração Harmony via `__state`: **Build succeeded, 0 warnings, 0 errors**. Execução: https://github.com/Jin27012007/Worldbox-mod-geo/actions/runs/39846610447
- O teste compila o fonte contra stubs Unity/NeoModLoader; não gera uma DLL pronta nem valida a API real por completo.
- O passo de compilação real é pulado porque as assemblies locais em `references/` não foram incluídas no runner público. Portanto, nenhuma DLL instalável compilada contra as referências reais do jogo foi produzida nesta etapa.

## Erro reproduzido pelo usuário
O log do telefone mostra `NullReferenceException` em `PowerTabController.showWhoIsSelected(String, Boolean)` durante `StatsWindow.OnEnable()`, acionada por `SelectedPowerTabButtons.clickOpenMain()`. A janela nativa fica parcialmente aberta; interfaces anteriores também desapareceram na orientação horizontal. A build 0.5.8-dev ainda não foi confirmada no aparelho.

## Divergência das assemblies
`docs/ASSEMBLY_INVESTIGATION.md` contém os hashes, MVIDs e IL inspecionados. O pacote fornecido tem `StatsWindow.OnEnable()` chamando `PowerTabController.showWorldTipSelected(String, Boolean)` e a classe `SelectedPowerTabTopButtons`; a stack do telefone tem `showWhoIsSelected` e `SelectedPowerTabButtons`, ausentes nas referências fornecidas. Assim, a causa exata da referência nula ainda não pode ser fixada com confiança até identificar o binário realmente carregado no aparelho.

## Mudanças 0.5.8-dev
- Prefix Harmony em `StatsWindow.OnEnable` ativa um escopo de proteção explícito, guardando o estado anterior por `__state`.
- Prefix do método auxiliar (`showWhoIsSelected`/`showWorldTipSelected`, assinatura `(string,bool)`) ignora a atualização auxiliar apenas enquanto o status está sendo inicializado; chamadas fora desse escopo continuam originais.
- Postfix e finalizer restauram o estado de proteção anterior, inclusive em chamadas aninhadas. O finalizer continua como segunda barreira para a NullReference conhecida e tenta fechar o `ScrollWindow` ancestral completo antes de abrir o painel alternativo.
- Canvas persistente é raiz em tela cheia; `SafeAreaRoot` filho é atualizado na rotação pelo controlador principal; atalho `GOVERNO` permanece visível quando o painel está fechado.
- Logs incluem a criação do Canvas, instalação dos hooks e FullName/MVID de `StatsWindow`, `PowerTabController` e Harmony.

## Próxima validação necessária
1. No Android, confirmar `OBSERVATORIO 0.5.8-dev`, `CANVAS_CREATED`, `STATUS_INFO_PREFIX_INSTALLED`, `STATUS_WINDOW_PREFIX_INSTALLED`, `STATUS_WINDOW_POSTFIX_INSTALLED` e `STATUS_WINDOW_FINALIZER_INSTALLED`.
2. Abrir o status do reino e verificar `STATUS_INFO_PREFIX_SKIP` e se a janela conclui a inicialização.
3. Preservar a linha `Runtime assembly fingerprint` e compará-la com `docs/ASSEMBLY_INVESTIGATION.md` se o erro persistir.

## Limites
- Ainda não há DLL instalável validada contra as assemblies reais do WorldBox/NML, nem teste no aparelho da 0.5.8-dev.
- A interface de administração ainda é protótipo; suas ações não alteram reinos reais.
- As DLLs proprietárias do jogo, Unity e loader ficam fora do repositório público.