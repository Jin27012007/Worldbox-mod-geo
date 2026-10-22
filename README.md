# Observatório dos Reinos — WorldBox Android

Mod original 2D para WorldBox Android com foco em geopolítica, diplomacia e interações de RPG. O objetivo é acrescentar sistemas próprios sem substituir a simulação original do jogo.

## Estado atual

A classe de entrada do NeoModLoader foi confirmada pelo usuário no jogo: o mod foi reconhecido e a mensagem de diagnóstico apareceu no log. A versão mais recente do código acrescenta um protótipo de regras políticas isoladas, mas **essa atualização ainda precisa ser recompilada/carregada no aparelho**.

## Primeira camada de sistemas

- Cadastro interno de entidades políticas por ID e nome.
- Estabilidade e legitimidade (0–100) e tesouro (0–1.000.000).
- Relações diplomáticas do mod: neutra, comércio, aliança, rivalidade e guerra.
- Opinião entre reinos (-100 a +100).
- Proteção contra relações duplicadas quando os IDs são informados em ordem invertida.
- Autoteste reportado no log: dois reinos simulados e uma relação de comércio.

**Importante:** são dados internos de protótipo; ainda não leem nem alteram os reinos reais do mundo. Nenhuma guerra, cidade, unidade ou relação original do WorldBox é modificada nesta etapa.

## Arquivos

- `mod.json`: manifesto do NeoModLoader.
- `src/ObservatorioMod.cs`: entrada e regras políticas iniciais.
- `docs/STATUS.md`: estado confirmado, limitações e próximos passos.
- `docs/TEST_PROTOCOL.md`: protocolo para testar no Android.

## Próximos passos

1. Reinstalar/atualizar a pasta do mod com esta versão do fonte e confirmar a nova linha de autoteste.
2. Investigar os tipos e membros reais de reinos e cidades na assembly exata da versão 0.51.4 instalada.
3. Adicionar leitura real do mundo, inicialmente somente leitura.
4. Criar painel 2D próprio e testar em orientação horizontal e vertical.
5. Construir eventos diplomáticos, comércio, crises políticas e interações RPG apenas depois de confirmar APIs e persistência.

## Compatibilidade e origem

O protótipo usa a estrutura pública documentada do NeoModLoader. O código atualizado não foi compilado de forma independente nem validado em runtime no aparelho. Referências comunitárias de APIs de WorldBox 0.51.2 são apenas pistas para a 0.51.4, não prova de compatibilidade. Não inclui DLLs do jogo/carregador nem reutiliza código ou assets do Worldfall.
