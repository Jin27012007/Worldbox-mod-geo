# Observatório dos Reinos — WorldBox Android

Mod original 2D que amplia o controle nativo de personagens com governo e diplomacia. Ao controlar um rei ou rainha, a meta é abrir ferramentas para governar o reino daquele personagem sem substituir os controles normais do WorldBox. A interface deve aproveitar a identidade visual padrão do jogo.

## Funcionalidades desejadas
- Detectar o personagem controlado e confirmar se é rei/rainha.
- Identificar reino e cidade correspondentes.
- Painel de governo: políticas e administração interna.
- Contato com vilas/cidades, reuniões com representantes e conselheiros.
- Pedidos de reunião com outros reinos, tratados, alianças, rivalidades e guerras.
- Diálogos/eventos de RPG com escolhas e consequências no mundo.

## Progresso confirmado
O usuário testou com sucesso o carregamento do mod, o autoteste político e o diagnóstico de reflexão no NeoModLoader Android. O código atual ampliou a sonda de API para registrar mais membros candidatos de `PlayerControl`, `Actor`, `Kingdom`, `City` e `MapBox`.

## Próximo teste
Baixe o ZIP da branch `scaffold/observatorio-dos-reinos`, atualize a pasta do mod preservando um backup e procure as linhas `Diagnostico do mundo:` e `Diagnostico de API (somente leitura):` no log. O conteúdo completo das linhas ainda é necessário para confirmar as APIs da build 0.51.4.

## Estado e limitações
- A leitura de API é somente de metadados e coleções; não chama métodos de alteração do jogo.
- Ainda não foi confirmada a detecção do personagem controlado nem do cargo de rei/rainha.
- Ainda não existe painel jogável ou ações integradas de governo/diplomacia.
- Os reinos usados no autoteste são fictícios e servem apenas para validar a inicialização do protótipo.
- Documentação comunitária de 0.51.2 serve somente como pista para 0.51.4.

## Plano de execução
1. Validar o probe expandido na versão Android 0.51.4.
2. Implementar leitura fail-soft dos dados reais de personagem/reino/cidade.
3. Confirmar o personagem controlado e o papel de rei/rainha.
4. Construir um painel em estilo nativo do WorldBox, inicialmente somente leitura.
5. Implementar políticas administrativas e testar em mundo descartável.
6. Conectar reuniões, tratados, guerras e eventos de RPG após validar suas APIs.

Não embutir DLLs do jogo/loader nem reutilizar código ou assets do Worldfall.
