# Contrato observado da UI — WorldBox 0.51.4

Data da inspeção: 2026-10-23  
Fonte: `Assembly-CSharp.dll` extraída do arquivo `com.mkarpenko.worldbox.zip` fornecido nesta conversa. O MVID da assembly, em formato sem separadores, é `c8e357a15932475b92df58e3db43b9da`, o mesmo identificador que aparece no stack trace da captura. Este documento registra metadados do assembly; não significa que o mod foi carregado ou testado em runtime.

## Membros confirmados nos metadados

### `SelectedPowerTabTopButtons`

Campos de instância relevantes:

- `_button_possession` — `UnityEngine.GameObject`
- `_button_spectate` — `UnityEngine.GameObject`
- `_button_open_window` — `UnityEngine.GameObject`
- `_button_favorite` — `UnityEngine.GameObject`

Métodos confirmados:

- `Update()` — instância, sem parâmetros.
- `clickOpenMain()` — instância, sem parâmetros.

Isso confirma que o tipo e o método `Update()` procurados pelo hook experimental existem na assembly examinada. Também confirma que os campos prioritários usados para localizar uma âncora são referências diretas a `GameObject`, não nomes hipotéticos.

### `PowerTabController`

Campos incluem `tab_main`, `tab_selected_unit`, `tab_multiple_units` e outros tabs nativos. Também há um campo estático chamado `instance`.

Métodos confirmados:

- `Update()` — instância, sem parâmetros.
- `showWorldTipSelected(String, Boolean)` — instância.

O segundo método aparece no stack trace da captura durante a abertura da janela de informações. Não é um ponto de entrada comprovadamente seguro para a janela do Observatório.

### Estado da seleção

- `SelectedUnit.isSet()` existe e retorna `Boolean`.
- `SelectedObjects.isNanoObjectSet()` existe e retorna `Boolean`.

Os dois métodos não resolvem por si sós o problema visual do botão. Ainda é necessário confirmar seu resultado na sessão em que o mod roda e observar os logs do próprio Observatório.

## Comparação com a implementação experimental

O código da branch `scaffold/observatorio-dos-reinos` procura por `SelectedPowerTabTopButtons.Update()`, tenta usar `_button_possession`, `_button_spectate` ou `_button_open_window` como referência e chama `TentarInserirNoPainel` após o Update. Esses nomes batem com os metadados examinados.

Portanto, a hipótese simples de que o nome da classe, o nome do método `Update()` ou os campos prioritários foram digitados incorretamente não é sustentada pela assembly fornecida.

O contrato estático, contudo, não informa se os campos estão preenchidos quando o hook executa, em qual pai/layout o botão deve ser inserido, se a referência existe na cena ativa, ou se o assembly do mod foi carregado naquela sessão. Esses pontos exigem logs de runtime; trocar novamente nomes de métodos ou lançar outro botão sem essa evidência seria especulativo.

## Próxima evidência que separa as hipóteses

Na build de diagnóstico, procurar no log:

- `[OR-BASELINE] STAGE=ON_MOD_LOAD` e `STAGE=UPDATE_OK`: confirmam os estágios do ciclo de vida do mod.
- `[OR-BASELINE] ASSEMBLY ... mvid=c8e357a15932475b92df58e3db43b9da`: confirma que o jogo carregou a assembly esperada.
- `[OR-BASELINE] TYPE_PROBE`, `METHOD_PROBE` e `FIELD_PROBE`: registram o contrato visto pelo runtime.
- `NPC_CONTROL_HOOK_SUMMARY`, `NATIVE_CONTROL_BUTTON_CREATED`, `NPC_CONTROL_PANEL_NOT_FOUND` ou `NATIVE_CONTROL_BUTTON_FAILED`: distinguem falha ao instalar o hook, não localizar o painel e falha ao criar o botão.

O CI de stubs não substitui esses dados do jogo real.
