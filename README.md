# Observatório dos Reinos — WorldBox Android

Projeto original para acrescentar governo e diplomacia ao controle nativo de rei/rainha. Alvo informado: WorldBox Android 0.51.4 com NeoModLoader.

## Estado atual — `0.5.4-dev`
- UI Unity Canvas com abas de Governo, Diplomacia, Reuniões e RPG, campos, toggles e botões de demonstração.
- Botão `GOV` tenta aparecer próximo ao status nativo; um atalho `GOVERNO` serve de reserva.
- A UI tem diagnóstico de resolução/área segura, estado do Canvas e localização do botão; a instalação Harmony registra `STATUS_PATCH_INSTALLED` ou `STATUS_PATCH_NOT_INSTALLED`.
- A compilação independente do fonte contra stubs Unity/NML passou no GitHub Actions com **0 erros e 0 avisos**. Essa é uma verificação de sintaxe/tipos internos, não uma DLL instalável e não prova compatibilidade real com as assemblies do jogo.

## Falha nativa ainda em aberto
A captura do telefone mostra `NullReferenceException` em `PowerTabController.showWhoIsSelected(String, Boolean)` dentro de `StatsWindow.OnEnable()`. A janela deixa de renderizar corretamente e some em landscape. A proteção do mod não foi confirmada funcionando no aparelho.

## Incompatibilidade na assembly de referência
O pacote de referência enviado contém `Assembly-CSharp-Publicized.dll` e `Assembly-CSharp.dll` que, pela inspeção de identificadores, expõem `SelectedPowerTabTopButtons` e `showWorldTipSelected`, mas não os nomes `SelectedPowerTabButtons` / `showWhoIsSelected` da stack real. As assemblies fornecidas, portanto, não correspondem à versão/runtime que originou a captura, apesar de a informação de versão visível ser semelhante. O campo nulo exato segue desconhecido.

## O que falta para compilar de verdade
Uma DLL instalável precisa ser compilada contra as assemblies privadas exatas de WorldBox 0.51.4 + NML. Essas DLLs não serão copiadas para o repositório público. O código atual ainda não foi compilado contra elas nem validado no Android.

## Próximo teste útil
Após instalar o ZIP desta branch com backup, procure `OBSERVATORIO 0.5.4-dev` e `GOVERNO`. No log, confira `STATUS_PATCH_INSTALLED` ou `STATUS_PATCH_NOT_INSTALLED`; ao pressionar o status, procure `FINALIZER ACIONADO`. Sem a faixa e os logs, não devemos depurar layout nem assumir que o patch carregou.
