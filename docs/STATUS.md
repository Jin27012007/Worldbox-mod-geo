# Status técnico — Observatório dos Reinos 0.6.3-dev

## Inserção do botão

- O atalho global foi removido.
- Na inicialização, o mod inspeciona as assemblies carregadas e tenta conectar um postfix Harmony ao método `Update()` de `SelectedPowerTabButtons`, `SelectedPowerTabTopButtons` ou classes `SelectedPowerTab*Buttons`.
- Depois de cada atualização do painel, o código procura os campos nativos de posse/espectador/informações e insere `GOV` como irmão do botão de referência. Se não encontrar campo, usa outro Button filho ou, como último fallback, a raiz do próprio componente.
- A varredura periódica da hierarquia de `CanvasMain.canvas_ui` continua como fallback se nenhum botão tiver sido registrado.
- O botão não depende da janela estar aberta para ser criado. O clique chama `ObservatorioJanelaNativa.Abrir()` e registra sucesso/falha no log.

## Evidências esperadas no log

- `NPC_CONTROL_HOOK_SUMMARY`: quantidade de classes encontradas e hooks conectados.
- `NPC_CONTROL_HOOK_INSTALLED`: nome completo do componente cujo `Update()` foi interceptado.
- `NATIVE_CONTROL_BUTTON_CREATED`: nome da classe de controle e contêiner no qual o botão foi inserido.
- `NPC_CONTROL_REFERENCE_FALLBACK`: nenhum campo de referência conhecido foi encontrado; inclui um resumo dos botões filhos.
- `NPC_CONTROL_PANEL_NOT_FOUND`: nem hook nem fallback localizaram a classe esperada.
- `NATIVE_CONTROL_BUTTON_FAILED`: exceção ao construir o botão.

## Validação e limites

- O código foi modificado para usar o ciclo de atualização real da aba como gatilho primário.
- O CI valida compilação contra stubs e invariantes do modelo político local, não a renderização no Unity.
- A nova revisão ainda precisa ser confirmada no WorldBox Android em execução; não existe confirmação visual automática neste ambiente.
- O Observatório não interfere na janela nativa de status.
- Os controles de governo, diplomacia e RPG continuam registrando dados locais da sessão; não mudam os reinos reais.

## Download

https://github.com/Jin27012007/Worldbox-mod-geo/archive/refs/heads/scaffold/observatorio-dos-reinos.zip
