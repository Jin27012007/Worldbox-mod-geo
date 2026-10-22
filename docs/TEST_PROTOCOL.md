# Teste Android — 0.5.1-dev

## Diagnóstico da UI
1. Faça backup da pasta do Observatório dos Reinos e instale o ZIP do branch `scaffold/observatorio-dos-reinos`.
2. Em retrato, procure a faixa `OBSERVATORIO 0.5.1-dev` e o atalho `GOVERNO` perto do topo, mas abaixo da barra de estado.
3. Toque em `GOVERNO`: a janela deve abrir com abas Governo, Diplomacia, Reuniões e RPG.
4. Gire para paisagem. A faixa e o atalho devem continuar visíveis; use o atalho para abrir/fechar de novo.
5. Toque no botão nativo de status do reino. Se o fallback rodar, o log deverá conter `FINALIZER ACIONADO` e o painel alternativo deve abrir.

## Mensagens úteis do log
- `Canvas, botao reserva e painel admin prontos`
- `UI administrativa via Canvas criada`
- `Botao de governo inserido acima do botao nativo de informacoes`
- `FINALIZER ACIONADO: NullReference no status nativo`

## Como interpretar
- Sem faixa visível e sem essas mensagens: fonte não compilado/carregado, ou erro antes da inicialização. Não tente corrigir posição de UI ainda.
- Faixa presente mas atalho ausente: erro de posicionamento/Hierarchy ou o elemento não está ativo; envie captura e as mensagens.
- Atalho presente mas tocar não abre: bug no callback da UI; envie captura e stack trace.
- O erro nativo continua sem `FINALIZER ACIONADO`: o patch Harmony não se instalou ou a exceção ocorre em método diferente. Envie stack completa; não vamos simplesmente esconder erros.
- Se o botão funciona em retrato, mas some em paisagem, a faixa de diagnóstico deverá mostrar resolução/área segura para a orientação nova; envie captura nos dois sentidos.

A UI continua em protótipo: botões demonstrativos não alteram entidades do mundo.
