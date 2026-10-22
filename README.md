# Observatório dos Reinos — WorldBox Android

Mod original 2D para estender o controle nativo de personagens: controlar um rei/rainha deverá habilitar ferramentas de governo para seu reino. O projeto busca usar a identidade visual padrão do WorldBox.

## Versão de desenvolvimento

`0.4.0-dev` acrescenta um protótipo visual de administração, com botão `Governo` e quatro abas: Governo, Diplomacia, Reuniões e RPG. Inclui caixas de texto, toggles e botões que deixam rascunhos/avisos locais. A janela calcula sua posição e tamanho usando `Screen.width` e `Screen.height`, com rolagem para telas menores.

## Estado atual

- O usuário confirmou que o mod e os diagnósticos anteriores carregam no Android.
- O status nativo de reino fica parcialmente cortado quando acionado durante o controle de um rei em orientação vertical. A causa exata ainda não foi isolada.
- O novo overlay é uma alternativa responsiva de interface para testar; não modifica diretamente a UI nativa do status do reino.
- A UI usa Unity IMGUI e a skin de Unity com cores aproximadas da paleta escura/dourada do WorldBox. Ainda não foi compilada ou testada em runtime no aparelho.
- As opções são demonstrativas e não alteram dados de reinos, cidades, relações, guerras ou unidades.
- A detecção do rei controlado e a leitura de informações reais do reino continuam pendentes.

## Teste desta versão

1. Faça backup da pasta atual do mod.
2. Baixe o ZIP da branch `scaffold/observatorio-dos-reinos` e atualize os arquivos do Observatório dos Reinos.
3. Abra o WorldBox e veja se aparece o botão `Governo`.
4. Toque nele, alterne entre as quatro abas, digite nos campos e role o conteúdo.
5. Teste em orientação vertical e horizontal. O painel deve caber na tela; o botão deve abrir/fechar a janela.
6. Se o mod não carregar, envie a mensagem completa do NML. Se carregar, envie uma captura da janela aberta e diga se o toque nos controles funciona.

## Próximas etapas
1. Corrigir o layout do overlay com base no teste Android.
2. Identificar a API de personagem controlado e sua relação com rei/rainha, reino e cidade na build 0.51.4.
3. Exibir dados reais no painel somente leitura.
4. Conectar ações administrativas e diplomáticas em pequenas etapas, com validação em mundo descartável.

Não incluir DLLs do jogo/loader nem reutilizar código ou assets de Worldfall. Informações públicas de 0.51.2 não são garantia de compatibilidade com 0.51.4.
