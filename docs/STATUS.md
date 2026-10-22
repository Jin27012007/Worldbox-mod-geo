# Status técnico — Observatório dos Reinos 0.5.7-dev

## Build
- O CI de `0.5.5-dev`/`0.5.6-dev` passou contra stubs isolados Unity/NML com 0 erros e 0 avisos, mas os primeiros builds da refatoração de escopo de Harmony falharam porque `ConstruirHarmonyMethod` havia sido removido acidentalmente durante uma edição. Isso foi corrigido no commit `dd553b5cf329ee0e7e84cc58c390e3ca9bd3ed61`; é necessário aguardar a execução do CI desse SHA antes de marcar 0.5.7-dev como validado.
- A checagem real contra DLLs agora usa `netstandard2.1` e referências Unity UI/texto explícitas; o runner público não tem os arquivos locais em `references/`, portanto esse passo continua sendo pulado. Nenhuma DLL instalável foi produzida contra o runtime real.

## Problema relatado no telefone
A captura mostra `NullReferenceException` em `PowerTabController.showWhoIsSelected(String, Boolean)` durante `StatsWindow.OnEnable()`, chamada a partir de `SelectedPowerTabButtons.clickOpenMain()`. A janela nativa fica incompleta; em landscape, o menu/atalho de versões anteriores desaparecia. A versão 0.5.7-dev ainda não foi confirmada no aparelho.

## Assemblies de referência versus runtime
Consulte `docs/ASSEMBLY_INVESTIGATION.md` para hashes, MVIDs e IL. A assembly entregue contém `StatsWindow.OnEnable()` chamando `PowerTabController.showWorldTipSelected(String, Boolean)` e `SelectedPowerTabTopButtons.clickOpenMain()`. O log do telefone usa `showWhoIsSelected` e `SelectedPowerTabButtons`, que não aparecem no pacote de referência. O MVID/runtime fingerprint adicionado ao mod deve confirmar qual assembly o telefone realmente carrega. Sem isso, o campo nulo específico não está provado.

## Mudanças em 0.5.7-dev
- Harmony é tentado antes da construção visual; a falha do Canvas não impede a instalação da proteção.
- O prefix de `StatsWindow.OnEnable` ativa um escopo explícito (`abrindoStatus`). O prefix do método auxiliar `showWhoIsSelected`/`showWorldTipSelected` com assinatura `(string,bool)` pula a rotina apenas enquanto esse escopo está ativo, evitando depender de `StackTrace` para adivinhar o chamador.
- Um postfix limpa o escopo após o retorno normal de `StatsWindow.OnEnable`; o finalizer limpa o escopo tanto em sucesso quanto em exceção e serve de segunda barreira para a `NullReferenceException` conhecida.
- O fallback tenta fechar o `ScrollWindow` ancestral completo, não apenas o componente interno, antes de abrir o menu alternativo.
- O Canvas persistente é a raiz de tela cheia; `SafeAreaRoot` filho acompanha `Screen.safeArea` na rotação; o atalho `GOVERNO` permanece disponível com o painel fechado.
- Logs têm `CANVAS_CREATED`/`CANVAS_NOT_CREATED`, prefix/postfix/finalizer instalados/acionados e fingerprint (assembly FullName + MVID) de `StatsWindow`, `PowerTabController` e Harmony.
- `tests/CompileWorldBoxRefs.csproj` foi ajustado para `netstandard2.1`, compatível com a configuração comum de mods WorldBox/NML, e inclui módulos Unity de UI/texto sem referenciar simultaneamente a DLL normal e a publicizada da `Assembly-CSharp`.

## Próxima validação
1. Confirmar CI do commit `dd553b5cf329ee0e7e84cc58c390e3ca9bd3ed61` ou posterior.
2. No Android, confirmar a faixa `OBSERVATORIO 0.5.7-dev`, botão `GOVERNO`, `CANVAS_CREATED`, `STATUS_WINDOW_PREFIX_INSTALLED`, `STATUS_WINDOW_POSTFIX_INSTALLED`, `STATUS_WINDOW_FINALIZER_INSTALLED` e `STATUS_INFO_PREFIX_INSTALLED`.
3. Abrir status; procurar `STATUS_INFO_PREFIX_SKIP` e confirmar se a janela abre inteira. Se falhar, guardar a linha `Runtime assembly fingerprint` e a stack inteira.

## Limites
- O build de fonte com stubs não equivale a compilar uma DLL instalável contra as DLLs reais.
- A versão atual ainda não foi testada no telefone.
- Botões de administração ainda são rascunhos e não alteram reinos reais.
- As DLLs do jogo/Unity/loader não são publicadas no repositório público.