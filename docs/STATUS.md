# Status técnico — 0.5.5-dev

## Resumo objetivo
- Verificação independente do código-fonte contra stubs Unity/NeoModLoader passa no GitHub Actions (`dotnet build tests/CompileStubs.csproj`, 0 erros/0 avisos na revisão 0.5.4-dev). A revisão 0.5.5-dev contém novas alterações; precisa de um novo job de CI.
- Essa verificação não é uma DLL instalável e não valida a API real do runtime.
- As DLLs de referência foram analisadas localmente, mas não serão publicadas neste repositório público, porque pertencem ao jogo/loader. O ambiente atual não dispõe de compilador C# local nem permite instalação por rede; a compilação real contra referências exatas ainda fica bloqueada.

## Evidência do dispositivo
- WorldBox Android 0.51.4, NeoModLoader presente.
- A stack trace do aparelho mostra `NullReferenceException` em `PowerTabController.showWhoIsSelected(String, Boolean)` durante `StatsWindow.OnEnable()` ao abrir status do reino.
- A janela nativa fica parcialmente renderizada e, em landscape, janelas/atalhos anteriores desapareceram.
- O usuário não testou ainda a revisão 0.5.4-dev; então não devemos dizer que o código novo foi executado no dispositivo.

## Inspeção do IL da assembly de referência fornecida
- `Assembly-CSharp.dll` contém `StatsWindow.OnEnable()` (token `0x0600478C` no arquivo analisado) e `PowerTabController.showWorldTipSelected(String, Boolean)` (token `0x06003B5B`).
- O IL de `StatsWindow.OnEnable()` chama `PowerTabController.showWorldTipSelected(...)` durante a abertura da janela.
- O IL de `showWorldTipSelected` obtém o objeto selecionado e o tipo, compara `prev_selected_meta_id`, consulta `power_tab_library`, invoca `get_localized_worldtip` e atualiza a toolbar. Uma das referências dessa cadeia pode estar nula quando a janela é aberta nessa situação; o arquivo disponível não possui o método `showWhoIsSelected` citado pelo dispositivo, portanto não é possível afirmar qual objeto exato é nulo na execução real.
- A mesma assembly tem um método `OnEnable` em `StatsWindow` com assinatura sem parâmetros. A API Harmony em `0Harmony.dll` possui overloads `Patch` e construtor de `HarmonyMethod(MethodInfo)` compatíveis com a estratégia reflexiva de prefix/finalizer.

## Mudanças em 0.5.5-dev
- A configuração preventiva do Harmony é tentada **antes** de construir a interface, para uma falha do Canvas não impedir a proteção do status.
- Descobre e tenta interceptar por prefix os métodos com nome `showWhoIsSelected` ou `showWorldTipSelected` e assinatura `(string, bool)`. O prefix só ignora a chamada quando a stack de chamada está dentro de `StatsWindow.OnEnable`/`WindowMetaGeneric.OnEnable`; nas demais situações deixa o método original rodar. Isso tenta permitir a conclusão da inicialização do status sem chamar a rotina auxiliar de tooltip nessa transição.
- Mantém finalizer de `StatsWindow.OnEnable` como fallback para a `NullReferenceException` conhecida. Mensagens distintas reportam prefix instalado, finalizer instalado, falhas ou callback acionado.
- O Canvas agora é o objeto raiz persistente; o `SafeAreaRoot` é um filho que recebe `Screen.safeArea`. Antes a safe area era aplicada diretamente ao objeto Canvas, hipótese concreta para o desaparecimento/escala errada em orientação horizontal.
- O atalho reserva `GOVERNO` permanece ativo quando o painel está fechado mesmo se o botão `GOV` for encontrado na barra nativa.
- O `Awake` tenta a proteção Harmony antes da UI, e regista `CANVAS_CREATED` ou `CANVAS_NOT_CREATED` em vez de deixar a criação visual silenciosa.
- A versão exibida na faixa é `0.5.5-dev`.

## O que ainda não está demonstrado
- A compilação real contra as assemblies privadas exatas do aparelho.
- Que o prefix consegue ser instalado na runtime exata e captura o método `showWhoIsSelected`.
- Que o painel aparece no dispositivo e a janela nativa agora termina a inicialização.
- As ações de governo ainda são rascunhos visuais e não alteram reinos reais.

## Portão de validação
1. CI stub-based deve passar na revisão 0.5.5-dev.
2. No aparelho, confirmar faixa `OBSERVATORIO 0.5.5-dev`, `STATUS_INFO_PREFIX_INSTALLED`, `STATUS_WINDOW_FINALIZER_INSTALLED` e `CANVAS_CREATED`.
3. Ao tocar status do reino, procurar `STATUS_INFO_PREFIX_SKIP`. Se ocorrer, verificar se a janela abriu por completo; se não houver skip, verificar a razão do prefix não ser instalado ou de não encontrar caller `OnEnable`.
4. Confirmar atalho `GOVERNO` em retrato e paisagem.
5. Só então decidir se é necessária uma correção mais específica baseada na assembly exata do processo que gera o log.

## Segurança
Não commitar as DLLs proprietárias do jogo/Unity no repositório público. Nenhuma entidade de gameplay é alterada por estas opções de demonstração. Não copiar código/assets de Worldfall.