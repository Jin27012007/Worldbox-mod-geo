# Teste Android — `0.5.2-dev`

O objetivo agora é descobrir por que o painel e o patch não aparecem antes de tentar outra correção cosmética.

1. Faça backup da pasta atual do Observatório dos Reinos.
2. Baixe e instale o ZIP do branch `scaffold/observatorio-dos-reinos`.
3. Em retrato, procure o banner `OBSERVATORIO 0.5.2-dev` e o botão `GOVERNO`.
4. Abra o log a partir do início do carregamento e procure:
   - `[Observatorio] Canvas, botao reserva e painel admin prontos`
   - `[Observatorio] Protecao de fallback instalada` ou a mensagem explicando por que não foi possível instalar
   - `UI administrativa via Canvas criada`
5. Abra o status nativo do reino e procure `FINALIZER ACIONADO`.
6. Gire para landscape; anote se o banner e o botão ainda existem.

## Decisão com base no resultado
- **Não aparecem as mensagens de carga:** mod não compilado/carregado nesta revisão; não ajustar mais layout até resolver isso.
- **Mensagens aparecem, mas não há banner:** revisar `Canvas`, sorting order, lifecycle e `Screen.safeArea`.
- **Banner e botão aparecem, mas clique não funciona:** investigar `GraphicRaycaster`, `EventSystem`, raycast targets e hierarquia.
- **Status dá erro, mas não há `FINALIZER ACIONADO`:** patch Harmony não foi instalado ou o método/assinatura real é diferente; obter metadata da assembly 0.51.4 antes de mudar o patch.
- **Finalizer é acionado, mas painel não abre:** investigar sequência de `OnEnable`/`SetActive`/Canvas, sem suprimir outras exceções.

Botões atuais são apenas rascunhos. Não testar mudanças de gameplay nesta etapa. Guarde o log completo em vez de enviar apenas a parte da exceção.