# Status técnico — Observatório dos Reinos 0.6.0-dev

## Alterações na fonte

- Removida a implementação antiga do painel em Canvas de tela cheia e os patches Harmony que interceptavam `StatsWindow.OnEnable()` e rotinas de tooltip/seleção.
- A interface principal agora herda de `AbstractWideWindow<ObservatorioJanelaNativa>`, usa `CreateAndInit("observatorio_dos_reinos", ...)` e registra uma janela nativa através do NeoModLoader.
- O botão `GOVERNO` é criado como filho de `CanvasMain.instance.canvas_ui` e abre a janela com `ScrollWindow.showWindow(IdJanela)`.
- Foram adicionados arquivos de localização para o título e o teste automatizado passou a validar invariantes do modelo político local, não o Harmony.
- O carregamento da UI aguarda `CanvasMain` estar disponível antes de inicializar a janela.

## Limitações conhecidas

- A janela nativa e o botão ainda **não foram confirmados no WorldBox rodando no telefone**.
- A validação via stubs apenas checa compilação/estrutura do código e lógica de modelo. Não substitui compilação contra as DLLs do WorldBox 0.51.4/NeoModLoader nem teste de interação real.
- O botão adiciona um controle visível ao Canvas do jogo, mas pode precisar de ajuste de posição dependendo da escala/orientação do aparelho.
- As ações de governo, diplomacia, reunião e RPG ainda registram preferências/propostas dentro da sessão. Elas não alteram estados internos reais de reinos/cidades nem ordenam guerra ou alianças.
- O erro original da janela de status não é mais interceptado pelo Observatório; se continuar acontecendo sem os patches do mod, a causa fica fora desta implementação e precisa ser investigada separadamente.

## Protocolo de aceitação

1. Instalar apenas a nova versão do Observatório.
2. Confirmar no log `NATIVE_WINDOW_CREATED`, `NATIVE_WINDOW_BUTTON_CREATED` e `JANELA_NATIVA_PRONTA`.
3. Tocar em `GOVERNO`; a janela deve abrir no espaço nativo do WorldBox, com controles de fechar/ESC do jogo.
4. Alternar entre Governo, Diplomacia, Reuniões e RPG; verificar textos, campos e botões.
5. Fechar a janela e abrir o status nativo para confirmar que o Observatório não instalou patches de Harmony nele.
6. Repetir em retrato e paisagem; se a janela ou botão sumir, guardar o log completo.

**Estado honesto:** mudança de arquitetura escrita no repositório; teste real no aparelho pendente.
