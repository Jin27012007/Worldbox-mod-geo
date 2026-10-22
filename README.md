# Observatório dos Reinos — WorldBox Android

Projeto original para acrescentar governo e diplomacia ao controle nativo de rei/rainha. Alvo informado: WorldBox Android 0.51.4 + NeoModLoader.

## `0.5.5-dev` — mudanças de engenharia
- Canvas persistente como raiz em tela cheia; `SafeAreaRoot` filho é dimensionado por `Screen.safeArea` e reajustado quando a orientação muda.
- O atalho `GOVERNO` fica disponível mesmo quando o botão `GOV` é localizado na barra nativa.
- A proteção Harmony é tentada antes da criação visual. Um prefix tenta evitar a rotina auxiliar de seleção/tooltip somente durante `StatsWindow.OnEnable` / `WindowMetaGeneric.OnEnable`; chamadas fora dessa transição continuam originais.
- Um finalizer fica como segunda barreira e tenta fechar a janela nativa `ScrollWindow` ancestral inteira se ocorrer a NullReference conhecida.
- Os logs reportam se cada patch foi instalado, se o Canvas foi criado e o nome/MVID das assemblies em runtime.

## Resultado de compilação
GitHub Actions executou `dotnet build tests/CompileStubs.csproj` contra stubs isolados de Unity/NeoModLoader e concluiu com **0 erros, 0 avisos** no SHA `179a62596388e2bb5ccc664ed563bdef6f78ffbc`. Isso valida sintaxe e consistência interna do código, mas **não produz uma DLL instalável** e não confirma a compatibilidade com as assemblies reais. O build real está configurado em `tests/CompileWorldBoxRefs.csproj`, mas depende de DLLs privadas que não são commitadas no repositório público.

## Diagnóstico da janela nativa
O log do aparelho menciona `PowerTabController.showWhoIsSelected(String, Boolean)` e `SelectedPowerTabButtons.clickOpenMain()`. O pacote de referência disponível tem `showWorldTipSelected` e `SelectedPowerTabTopButtons`, sem os nomes reportados na stack real. Os hashes, MVIDs e a leitura do IL estão em [`docs/ASSEMBLY_INVESTIGATION.md`](docs/ASSEMBLY_INVESTIGATION.md). A causa do campo nulo exato continua em aberto porque as assemblies não coincidem com o runtime do log.

## Estado honesto
A versão 0.5.5-dev ainda não foi confirmada no telefone e nenhuma DLL instalável foi produzida nesta rodada. As ações de governo são rascunhos; não modificam os reinos reais. Faça backup antes de substituir arquivos e use os logs `OBSERVATORIO 0.5.5-dev`, `CANVAS_CREATED`, `STATUS_INFO_PREFIX_INSTALLED`, `STATUS_WINDOW_FINALIZER_INSTALLED` e `Runtime assembly fingerprint` para confirmar a execução.