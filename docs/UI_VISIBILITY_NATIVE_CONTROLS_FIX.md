# Correção experimental da visibilidade do botão GOV

Data: 2026-10-23

## Problema identificado no código

A implementação anterior usava `SelectedUnit.isSet()` e `SelectedObjects.isNanoObjectSet()` como condição obrigatória para mostrar o botão. Se pelo menos um desses métodos fosse encontrado, mas ambos retornassem falso, o botão era ocultado mesmo que os controles nativos do painel estivessem ativos.

Esse vínculo é frágil: os flags de seleção lógica e o estado visual real dos controles da interface não precisam representar exatamente o mesmo contexto.

## Alteração nesta branch

- A visibilidade do botão agora segue os controles nativos do painel: `_button_possession`, `_button_spectate`, `_button_open_window` e `_button_favorite`.
- O botão é considerado elegível quando pelo menos um controle nativo correspondente está ativo na hierarquia.
- Se os campos não estiverem disponíveis ou não fornecerem objetos, o código tenta identificar controles filhos por nome; se ainda não houver referência, usa o estado ativo do próprio painel.
- O log `NATIVE_CONTROL_BUTTON_CREATED` passa a registrar `visibilidade_origem`.
- A rotina compara `activeSelf` com o estado desejado, em vez de comparar `activeInHierarchy`, evitando depender do estado de um pai desativado para decidir se o próprio botão precisa ser ligado ou desligado.

## Base da hipótese

A documentação `docs/UI_CONTRACT_0.51.4.md` registra esses quatro campos de instância na `Assembly-CSharp.dll` que corresponde ao identificador de assembly visto na captura do WorldBox 0.51.4.

## Verificação e limites

Foi feita uma verificação estática dos fragmentos editados e das referências ao novo helper. Isso não equivale a compilar contra as DLLs reais do NeoModLoader/WorldBox nem a executar o mod no Android.

A mudança mira uma causa plausível para um botão criado mas oculto; não confirma que o mod carrega, que o hook Harmony instala ou que a janela administrativa abre corretamente. O stack trace da janela de informações nativa continua sendo um problema separado.
