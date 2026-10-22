# Observatório dos Reinos — WorldBox Android

Mod original 2D que amplia o controle nativo de personagens com governo e diplomacia. Ao controlar um rei ou uma rainha, a meta é liberar ferramentas para governar seu reino sem substituir os controles normais do WorldBox. A interface deve aproveitar a identidade visual padrão do jogo.

## Experiência desejada
- Identificar o personagem controlado e confirmar se ele é rei/rainha.
- Vincular o personagem ao reino e à cidade corretos.
- Painel de governo com políticas e administração interna.
- Contato com vilas/cidades, reuniões com representantes e conselheiros.
- Pedidos de reunião com outros reinos, tratados, alianças, rivalidades e guerras.
- Diálogos/eventos de RPG com escolhas e efeitos no mundo.

## Status testado pelo usuário
O usuário confirmou que o carregador reconhece o mod e que o autoteste político da versão anterior aparece no log. A atualização mais recente adicionou uma inspeção por reflexão somente de leitura, mas ainda não foi testada no aparelho.

## Próxima tarefa de teste
Atualize os arquivos da branch `scaffold/observatorio-dos-reinos`, abra um mundo no WorldBox e procure no log as linhas `Diagnostico do mundo:` e `Diagnostico de API (somente leitura):`. Envie o conteúdo dessas linhas; elas vão indicar quais tipos e membros existem na build 0.51.4 instalada. Se aparecer erro de compilação, envie a mensagem completa.

## Limites atuais
- Ainda não detecta com certeza o personagem controlado nem seu cargo de rei/rainha.
- Ainda não tem painel jogável, botões de governo, reuniões, tratados, guerra integrada nem salvamento persistente.
- As regras políticas atuais são um modelo interno de protótipo; os dois reinos do autoteste são fictícios.
- A nova inspeção por reflexão procura tipos/campos e conta os gerenciadores de mundo, mas não altera objetos nativos.

## Plano
1. Validar o diagnóstico da assembly 0.51.4.
2. Ligar os dados reais de personagem, reino e cidade com leitura somente.
3. Construir um painel usando a identidade visual padrão do jogo.
4. Implementar decisões administrativas seguras.
5. Integrar reuniões, diplomacia e RPG depois de validar cada API.

Não incluir DLLs de jogo/loader nem reutilizar código ou assets de Worldfall. Referências públicas de WorldBox 0.51.2 são apenas pistas para a 0.51.4.
