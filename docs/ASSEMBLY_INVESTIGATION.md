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

## Divergência com o log do aparelho
O log enviado pelo usuário mostra `PowerTabController.showWhoIsSelected(String, Boolean)` e `SelectedPowerTabButtons.clickOpenMain()`. Nenhuma das assemblies de referência listadas contém os identificadores `showWhoIsSelected` ou `SelectedPowerTabButtons`; elas contêm `showWorldTipSelected` e `SelectedPowerTabTopButtons`.

Portanto, o pacote de referência enviado não corresponde exatamente ao runtime que gerou a stack trace. Não é possível apontar o campo exato da `NullReferenceException` apenas com essas assemblies. A versão `0.5.5-dev` agora registra o nome completo e o MVID das assemblies carregadas para que o log do dispositivo possa confirmar quais binários estão realmente em execução.

## Alterações preventivas no mod
- Harmony é tentado antes da construção da interface.
- Prefix mira `showWhoIsSelected` e `showWorldTipSelected` com assinatura `(string,bool)` e tenta ignorar a atualização auxiliar apenas durante `StatsWindow.OnEnable`/`WindowMetaGeneric.OnEnable`. Fora dessa chamada, deixa o método original seguir.
- Finalizer funciona como segunda barreira para a `NullReferenceException` conhecida e tenta fechar o `ScrollWindow` ancestral completo antes de abrir o painel alternativo.
- A UI usa Canvas persistente como raiz; `SafeAreaRoot` interno é reajustado pelo controlador principal após rotação; o atalho `GOVERNO` continua visível quando o painel está fechado.

## Build
GitHub Actions confirma que o fonte compila contra stubs isolados Unity/NeoModLoader com 0 erros e 0 avisos no SHA `179a62596388e2bb5ccc664ed563bdef6f78ffbc`. Isso é uma verificação de sintaxe e consistência interna, não uma DLL instalável. O projeto `tests/CompileWorldBoxRefs.csproj` permite build contra DLLs reais em `references/`, mas o runner público não tem acesso aos arquivos locais. As referências permanecem excluídas pelo `.gitignore`; uma compilação real precisa ocorrer num ambiente privado/local.
