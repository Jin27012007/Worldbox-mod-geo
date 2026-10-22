# Status técnico — Observatório dos Reinos 0.5.5-dev

## Build atual
- GitHub Actions executou `dotnet build tests/CompileStubs.csproj` contra stubs Unity/NML no SHA `179a62596388e2bb5ccc664ed563bdef6f78ffbc`: **Build succeeded, 0 warnings, 0 errors**.
- O passo de build contra DLLs reais foi corretamente marcado como ignorado porque `references/NeoModLoader.dll`, `Assembly-CSharp-Publicized.dll`, `UnityEngine.dll`, `UnityEngine.CoreModule.dll`, `UnityEngine.UI.dll` e `0Harmony.dll` não existem no runner. Eles não são commitados no repositório público.
- Portanto não existe ainda uma DLL final instalável ou uma compilação contra referências exatas do jogo. A checagem com stubs valida sintaxe/consistência interna, não a API real nem o runtime do aparelho.

## Log do aparelho e causa em investigação
O log do usuário mostra `NullReferenceException` em `PowerTabController.showWhoIsSelected(String, Boolean)` durante `StatsWindow.OnEnable()`, iniciado por `SelectedPowerTabButtons.clickOpenMain()`. A janela nativa fica parcialmente aberta; os painéis anteriormente testados também sumiram em landscape. O usuário ainda não testou a revisão atual 0.5.5-dev.

A inspeção dos metadados/IL do pacote de referência está em `docs/ASSEMBLY_INVESTIGATION.md`. Em síntese, as DLLs fornecidas contêm `StatsWindow.OnEnable()` e `PowerTabController.showWorldTipSelected(String, Boolean)`, mas não os nomes `showWhoIsSelected` e `SelectedPowerTabButtons` da stack do aparelho. É uma divergência comprovada, logo o campo nulo exato continua desconhecido.

## Mudanças 0.5.5-dev
- Instalação da proteção Harmony tentada antes de construir UI.
- Prefix procura ambos os nomes do helper com assinatura `(string,bool)` e tenta ignorar a atualização auxiliar somente quando a chamada ocorre dentro da inicialização do status (`StatsWindow.OnEnable` / `WindowMetaGeneric.OnEnable`). Os outros callers continuam executando o método original.
- Finalizer é segunda barreira para o `NullReferenceException` conhecido; tenta fechar o `ScrollWindow` ancestral completo em vez de desligar apenas o componente interno.
- Busca dos tipos tenta `Type.GetType` e depois percorre as assemblies carregadas.
- Canvas agora é objeto raiz persistente e o `SafeAreaRoot` filho respeita `Screen.safeArea`; o controlador principal atualiza âncoras na rotação.
- O atalho `GOVERNO` segue visível mesmo se o botão nativo for localizado.
- Log inclui assembly FullName + MVID de `StatsWindow`, `PowerTabController` e Harmony; isso permitirá comparar runtime real com o pacote de referência.

## Próximo gate
1. Executar a compilação stub novamente no último SHA `1606de358330400f4c1d426d2424496cebdb4b37` (docs apenas após último build).
2. No aparelho, confirmar faixa `OBSERVATORIO 0.5.5-dev`, `CANVAS_CREATED`, `STATUS_INFO_PREFIX_INSTALLED`, `STATUS_WINDOW_FINALIZER_INSTALLED` e linha `Runtime assembly fingerprint`.
3. Abrir status e procurar `STATUS_INFO_PREFIX_SKIP`. Se persistir erro, registrar MVID e stack completa para identificar a assembly real e mirar o método certo.

## Limites
- Nenhuma DLL final instalável foi produzida nesta rodada.
- Nenhuma mecânica política altera reinos reais ainda; botões atuais são rascunhos.
- As DLLs do jogo, Unity e loader não são distribuídas no repositório público.