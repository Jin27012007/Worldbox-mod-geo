# Investigação de assemblies — Observatório dos Reinos

## Referências examinadas localmente
Os binários abaixo vieram do pacote de referência enviado pelo usuário. Eles foram analisados fora do repositório público e não foram commitados.

| Assembly | SHA-256 | MVID |
|---|---|---|
| `Assembly-CSharp-Publicized.dll` | `8c54a0b93a3b929834fb426fce8ce66344444e7709484956ddf04267de718234` | `eaf20368-35e7-4bb6-b082-5477d61019a6` |
| `Assembly-CSharp.dll` | `dfe3697f8929e4ecfc3bec154167e3227199b5c6fcd1baf353f379937407bffe` | `c8e357a1-5932-475b-92df-58e3db43b9da` |
| `NeoModLoader.dll` | `adeccd1dfab7adafc6c0a283af728dd5ef16d9c899e8471ff78ccf9e38756d88` | `21090d08-defb-4c27-add6-b64c7d320d1d` |
| `0Harmony.dll` | `dfbd444ea824f2c2d2c67a69ffbe13695fa740981e08a3ea04845582f901b0f2` | `26c4a01a-d38b-42c3-bb84-784da4694f8e` |
| `UnityEngine.CoreModule.dll` | `3957af3fcf9603486acdcfdf13d58013f0e27f5e3ec743e3f5908d8f265d9f0d` | `748b2836-7761-4773-8cc4-bc54ba1a15d9` |
| `UnityEngine.UI.dll` | `1967429fc071f5376c4b09bae05eb184fc486fc3ce97df1d290e8fb1c4aae746` | `e41b1462-7f78-4a1d-a390-d02f335c32f3` |

## Metadados e IL inspecionados
- `StatsWindow.OnEnable()` está presente em `Assembly-CSharp.dll`, token `0x0600478C`, sem parâmetros.
- `WindowMetaGeneric<T>.OnEnable()` chama `StatsWindow.OnEnable()` e depois `startShowingWindow`.
- `PowerTabController.showWorldTipSelected(String, Boolean)` existe com token `0x06003B5B`.
- `SelectedPowerTabTopButtons.clickOpenMain()` aciona o delegate `PowerTabLibrary.defaultMainInfoClick` pelo campo `on_main_info_click`.

O IL de `StatsWindow.OnEnable()` obtém `power_tab_id` do meta/asset, chama o delegate `get_selected`, define o `NanoObject` selecionado e chama `PowerTabController.showWorldTipSelected(...)`. Esse método lê o objeto selecionado e seu tipo, compara `prev_selected_meta_id`, consulta `power_tab_library`, obtém `get_localized_worldtip`, invoca o delegate e atualiza toolbar.

## Comparação corrigida com o log do aparelho (2026-10-23)
A captura enviada pelo usuário mostra WorldBox `0.51.4 (725@a593f)`, a chamada `SelectedPowerTabTopButtons.clickOpenMain()` e a falha em `PowerTabController.showWorldTipSelected(String, Boolean)`. A DLL `Assembly-CSharp.dll` extraída do pacote fornecido contém esses mesmos nomes, e o MVID exibido no log (`c8e357a15932475b92df58e3db43b9da`) corresponde ao MVID registrado para essa DLL.

**Correção:** a afirmação anterior de que a stack trace usava `showWhoIsSelected` e `SelectedPowerTabButtons` estava errada. Não há divergência demonstrada entre esses nomes/essa DLL. A evidência indica uma `NullReferenceException` dentro de `showWorldTipSelected` durante `StatsWindow.OnEnable`; apenas esta captura e a DLL não identificam por si só o campo nulo. A revisão 0.6.4-dev registra novamente as assemblies do runtime para verificar o ambiente exato no qual a tentativa atual está sendo executada.

## Estratégia histórica de uma versão anterior (não é a implementação atual)
- Harmony é tentado antes da construção da interface.
- Prefix mira `showWhoIsSelected` e `showWorldTipSelected` com assinatura `(string,bool)` e tenta ignorar a atualização auxiliar apenas durante `StatsWindow.OnEnable`/`WindowMetaGeneric.OnEnable`. Fora dessa chamada, deixa o método original seguir.
- Finalizer funciona como segunda barreira para a `NullReferenceException` conhecida e tenta fechar o `ScrollWindow` ancestral completo antes de abrir o painel alternativo.
- A UI usa Canvas persistente como raiz; `SafeAreaRoot` interno é reajustado pelo controlador principal após rotação; o atalho `GOVERNO` continua visível quando o painel está fechado.

## Build
GitHub Actions confirma que o fonte compila contra stubs isolados Unity/NeoModLoader com 0 erros e 0 avisos no SHA `179a62596388e2bb5ccc664ed563bdef6f78ffbc`. Isso é uma verificação de sintaxe e consistência interna, não uma DLL instalável. O projeto `tests/CompileWorldBoxRefs.csproj` permite build contra DLLs reais em `references/`, mas o runner público não tem acesso aos arquivos locais. As referências permanecem excluídas pelo `.gitignore`; uma compilação real precisa ocorrer num ambiente privado/local.


## Verificação adicional do APK (2026-10-22)
O arquivo `worldbox_il2cpp2mono (1).apk` não tem central directory ZIP válido e está truncado perto do final; por isso ferramentas ZIP comuns rejeitam o arquivo. Ainda assim, foi possível percorrer os cabeçalhos ZIP locais, recuperar o conteúdo completo de `Assembly-CSharp.dll` da entrada `assets/Mono/Managed.zip` (o fluxo interno dessa entrada estava disponível até esse ponto) e validar o CRC32 da DLL extraída. O SHA-256 recuperado foi:

`dfe3697f8929e4ecfc3bec154167e3227199b5c6fcd1baf353f379937407bffe`

Esse hash é idêntico ao da `Assembly-CSharp.dll` do pacote `com.mkarpenko.worldbox.zip` já analisado. Portanto, os dois materiais enviados contêm a mesma assembly principal. O nome `showWhoIsSelected` da stack trace do dispositivo continua sem aparecer nessa DLL; a diferença não se explica por esse APK conter uma outra build de `Assembly-CSharp.dll`. O restante do pacote Managed.zip do APK está truncado e não forneceu uma assembly alternativa completa.

## Verificação de APIs Harmony
A metadata de `0Harmony.dll` confirma:
- `HarmonyLib.Harmony` tem construtor com uma string.
- Existe overload `Patch(MethodBase, HarmonyMethod, HarmonyMethod, HarmonyMethod, HarmonyMethod)` (cinco parâmetros).
- `HarmonyLib.HarmonyMethod` tem construtor que recebe `MethodInfo`.

Assim, a estratégia reflexiva usada no código corresponde às assinaturas encontradas nessa referência de Harmony. Isso confirma a compatibilidade com a DLL enviada, não com uma versão diferente que possa estar carregada no celular.


## Decisão posterior à investigação — 0.6.0-dev

A interface atual não usa mais as rotinas de Harmony investigadas acima. Os prefix/postfix/finalizer do Observatório foram removidos para que o mod não altere a abertura de `StatsWindow` ou as rotinas auxiliares de tooltip. O projeto usa agora `NeoModLoader.api.AbstractWideWindow<T>` e o registro nativo realizado pelo `WindowCreator.CreateEmptyWindow`.

Isso é uma retirada da intervenção do mod, não uma prova de que a NullReference original do jogo foi corrigida. Se o erro ocorrer com esta versão, a nova janela e o erro de status devem ser diagnosticados como dois problemas independentes.


## Revisão 0.6.1-dev: botão e janela desacoplados

O código não instala mais prefix/postfix/finalizer Harmony no status. A criação do atalho foi movida para antes da tentativa de inicialização da janela. O atalho usa Canvas próprio com ordenação prioritária e logs de falha por condição. Assim, uma exceção durante `AbstractWideWindow.CreateAndInit` não deve impedir a criação do botão.

A exceção nativa de status ainda não está corrigida. A ausência de patches significa que esta revisão não intercepta o erro; a stack trace atual do aparelho será necessária para decidir se o problema é causado pelo próprio jogo ou por outro mod.


## Decisão de arquitetura — 0.6.2-dev

A estratégia de interceptar `StatsWindow` e métodos auxiliares de tooltip/seleção foi removida da fonte. O mod não instala mais Prefix/Postfix/Finalizer nesses métodos; a janela nativa de status segue o fluxo do jogo sem intervenção do Observatório. Isso não afirma que a `NullReferenceException` do jogo foi corrigida.

O botão administrativo global em `CanvasMain.canvas_ui` também foi removido. A versão 0.6.2-dev procura a área de botões de `SelectedPowerTabButtons` ou `SelectedPowerTabTopButtons` e adiciona um botão `GOV` ao mesmo grupo de UI do controle/posse de NPC. A visibilidade é condicionada aos métodos de seleção `SelectedUnit.isSet()` ou `SelectedObjects.isNanoObjectSet()`. O runtime deve confirmar o nome real do componente/campo utilizado pelo telefone.


## Integração com controles nativos — 0.6.3-dev

A referência Assembly-CSharp examinada contém a classe `SelectedPowerTabTopButtons`, com campos `_button_possession`, `_button_spectate`, `_button_open_window` e o método `Update()`. A nova implementação usa esse ciclo de atualização como ponto de inserção e procura a mesma classe por reflexão no runtime. Também procura `SelectedPowerTabButtons` para cobrir a divergência de nome observada na stack trace fornecida anteriormente.

O hook tenta criar `GOV` como irmão do controle nativo. As mensagens `NPC_CONTROL_HOOK_SUMMARY`, `NPC_CONTROL_HOOK_INSTALLED` e `NATIVE_CONTROL_BUTTON_CREATED` diferenciam encontrar o tipo, instalar o hook e inserir o botão — etapas que o teste de compilação, sozinho, não confirma.
