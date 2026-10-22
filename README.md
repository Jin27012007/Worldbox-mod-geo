# Observatório dos Reinos — WorldBox Android

Projeto experimental para WorldBox Android 0.51.4 + NeoModLoader.

## 0.6.3-dev — integração pelo ciclo do painel de NPC

O botão global de canto de tela foi removido. A versão 0.6.3-dev tenta instalar um postfix Harmony no método `Update()` das classes carregadas `SelectedPowerTabButtons` e `SelectedPowerTabTopButtons` (e outras classes chamadas `SelectedPowerTab*Buttons`). A inserção do botão acontece a partir do componente real que atualiza a aba de controles, em vez de depender apenas de uma busca global por objetos.

O código procura primeiro os campos nativos de posse/espectador/abertura de informações; se esses nomes foram alterados, tenta usar outro botão que exista dentro do mesmo componente. Se a classe existir mas não tiver nenhum botão-filho, o código cria `GOV` diretamente sob a raiz desse painel. O botão aparece com a seleção de NPC/unidade quando os métodos de seleção conhecidos são encontrados; se a revisão do jogo não expuser esses métodos, usa o estado ativo do próprio painel.

O clique no botão abre `ObservatorioJanelaNativa`. O mod não aplica patches na janela de status do jogo.

## Diagnóstico incorporado

Após iniciar o jogo, procure no log por:
- `NPC_CONTROL_HOOK_SUMMARY`: classes de controle encontradas e quantidade de hooks conectados.
- `NPC_CONTROL_HOOK_INSTALLED`: o postfix foi instalado em `Update()`.
- `NATIVE_CONTROL_BUTTON_CREATED`: o botão foi criado, com a classe de painel, referência e contêiner.
- `NPC_CONTROL_REFERENCE_FALLBACK`: os nomes dos campos mudaram, então foi usada outra referência ou a raiz do painel.
- `NPC_CONTROL_PANEL_NOT_FOUND`, `NPC_CONTROL_HOOK_FAILED` ou `NATIVE_CONTROL_BUTTON_FAILED`: o ponto exato que falhou.

Essas mensagens são importantes: compilação não garante que o componente exista no runtime correto.

## Download

ZIP do código-fonte: https://github.com/Jin27012007/Worldbox-mod-geo/archive/refs/heads/scaffold/observatorio-dos-reinos.zip

É pacote-fonte, não uma DLL pré-compilada. Faça backup da pasta atual antes de substituir os arquivos, extraia com `mod.json` na raiz e confirme que a versão carregada no log é `0.6.3-dev`.

## Limites atuais

O CI compila contra stubs, verifica o modelo político local e **não simula a interface do Unity em execução**. A compilação local contra as referências privadas e a confirmação visual no Android ainda são necessárias. As ações de governo/diplomacia permanecem dados locais de sessão; não alteram os reinos reais.
