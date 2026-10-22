# Status técnico — 0.5.4-dev

## Resumo objetivo
- O fonte agora compila em verificação independente de C# contra stubs isolados de Unity/NeoModLoader: GitHub Actions `dotnet build tests/CompileStubs.csproj` concluiu com **0 erros e 0 avisos** no commit de verificação anterior.
- Essa verificação valida sintaxe e tipos internos do fonte contra uma API simulada. **Não é a DLL instalável do mod e não valida a assinatura real de Harmony nem de WorldBox 0.51.4.**
- A compilação direta inicialmente tentada com Mono ficou bloqueada porque o repositório não possui assemblies de referência privadas. O workflow foi substituído por .NET preinstalado, para compilar o fonte contra stubs sem travar em apt/Mono.
- Mod ainda não foi visto funcionando na build 0.5.4-dev no aparelho.

## Evidência do dispositivo
- WorldBox Android 0.51.4, NML instalado.
- Ao abrir a janela nativa de status do reino, o log mostra `NullReferenceException` em `PowerTabController.showWhoIsSelected(String, Boolean)` durante `StatsWindow.OnEnable()`, chamado pelo botão de informações.
- A janela nativa permanece parcialmente desenhada; ao mudar para paisagem, a janela/painel podem desaparecer.
- As tentativas anteriores de Canvas/patch não foram confirmadas como carregadas ou executadas.

## Descoberta sobre as assemblies fornecidas
Inspecionei strings das duas assemblies extraídas de `com.mkarpenko.worldbox.zip`:
- `Assembly-CSharp-Publicized.dll` SHA-256: `8c54a0b93a3b929834fb426fce8ce66344444e7709484956ddf04267de718234`
- `Assembly-CSharp.dll` SHA-256: `dfe3697f8929e4ecfc3bec154167e3227199b5c6fcd1baf353f379937407bffe`
- Ambas contêm os identificadores `StatsWindow`, `SelectedPowerTabTopButtons` e `showWorldTipSelected`.
- Nenhuma contém os nomes `showWhoIsSelected` ou `SelectedPowerTabButtons` que aparecem na stack trace do aparelho.

Isso estabelece uma divergência entre as assemblies do arquivo fornecido e o runtime reportado pelo jogo. Não dá para localizar o campo exato da NullReference com esse pacote. É necessário obter a assembly que corresponde realmente ao processo que produziu a stack trace.

## Estado do código 0.5.4-dev
- UI via Unity Canvas/Unity UI; tenta obedecer `Screen.safeArea`, mostra build tag, resolução, Canvas ativo, botão nativo localizado e visibilidade do fallback.
- Tenta acrescentar `GOV` perto do botão nativo do status; mantém botão-reserva.
- Patch Harmony agora enumera candidatos de `StatsWindow.OnEnable`, explora as assinaturas e registra explicitamente `STATUS_PATCH_INSTALLED` / `STATUS_PATCH_NOT_INSTALLED`.
- Se patch for instalado, o finalizer só tenta interceptar a NullReference conhecida quando a stack contém `showWhoIsSelected` ou `showWorldTipSelected`; isso é um fallback, não uma correção provada da referência nula original.
- Versão exibida na faixa sincronizada com `mod.json` (0.5.4-dev); removidos dois campos não usados.

## Gate de build
1. O teste de fontes com stubs passou.
2. A compilação real requer as DLLs de referência do jogo/NML usadas no dispositivo. Elas não devem ser commitadas no repositório público.
3. Antes de qualquer claim de correção, o aparelho precisa mostrar a faixa 0.5.4-dev e o log de inicialização da UI.
4. Depois, pressionar status e confirmar `STATUS_PATCH_INSTALLED` na inicialização e `FINALIZER ACIONADO` após o erro, ou capturar `STATUS_PATCH_NOT_INSTALLED` e a razão.

## Limites
- Nenhuma DLL final instalada no dispositivo foi produzida nesta rodada.
- O painel ainda é protótipo; opções não alteram reinos/cidades/guerra/diplomacia reais.
- Não incluir DLLs do jogo/NML no repositório público nem reutilizar código/assets de Worldfall.
