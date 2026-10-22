# Protocolo de testes — Android

## Teste da interface `0.4.0-dev`
1. Faça backup da pasta do Observatório dos Reinos e use um mundo descartável.
2. Baixe o ZIP da branch `scaffold/observatorio-dos-reinos` e atualize os arquivos desse mod; não toque na pasta do Worldfall.
3. Abra o jogo e verifique se aparece o botão `Governo` no canto superior esquerdo.
4. Toque em `Governo`. Teste as abas Governo, Diplomacia, Reuniões e RPG.
5. Digite nos campos de texto, marque/desmarque as opções e toque nos botões. Eles devem apenas alterar mensagens locais e não modificar o mundo.
6. Teste em orientação vertical e horizontal. Confirme se a janela inteira cabe na tela e se é possível alcançar os controles com o toque.
7. Envie uma captura do menu aberto e qualquer log de erro.

## Teste da janela nativa do status do reino
A janela nativa que aparece ao controlar um rei continua sendo um problema separado. Confirme se o overlay `Governo` cabe na tela mesmo quando a janela nativa fica cortada. Não afirmamos que a UI nativa foi corrigida nesta versão.

## Aprovação
- O mod carrega sem nova exceção.
- O botão abre e fecha a janela responsiva.
- As quatro abas alternam, os campos aceitam texto e a rolagem permite acessar o conteúdo.
- As ações mostram avisos de protótipo e não alteram entidades do jogo.

## Recuperação
Se o jogo travar ou o mod deixar de carregar, desative o Observatório dos Reinos e restaure o backup. Preserve o log desde o início da inicialização para diagnosticar a falha.
