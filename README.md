# Observatório dos Reinos — WorldBox Android

Mod original 2D para WorldBox Android, com governo e diplomacia ligados à mecânica nativa de controlar personagens. O objetivo é que, ao controlar um rei ou uma rainha, o jogador possa administrar o reino e interagir com outros reinos sem perder os controles normais do personagem.

## Experiência desejada

- Detectar o rei/rainha que o jogador está controlando e identificar seu reino.
- Abrir um painel de governo para definir regras e administrar o reino.
- Convocar reuniões com vilas/cidades e personagens importantes.
- Solicitar reuniões diplomáticas com outros reinos.
- Negociar tratados, comércio, alianças, rivalidades e guerras.
- Criar eventos e diálogos de RPG com escolhas e consequências no mundo.

## Estado atual confirmado

O usuário confirmou que o NeoModLoader reconhece o mod e que a mensagem de carregamento aparece no log. O usuário também confirmou que o autoteste político de `0.2.0-dev` apareceu corretamente no log.

O código atual contém um protótipo de estado político (estabilidade, legitimidade, tesouro, relações e opinião), mas o teste usa dois reinos fictícios. **Ainda não há ligação com o personagem controlado, reinos/cidades reais ou painel jogável.** O teste confirmou inicialização, não a experiência final.

## Rota de desenvolvimento

1. Confirmar na assembly exata do WorldBox Android 0.51.4 as APIs para personagem controlado, rei/rainha, reino, cidade e UI.
2. Ler e registrar informações reais sem alterar a simulação.
3. Detectar quando o jogador controla um rei/rainha e mapear o reino correspondente.
4. Criar um painel de governo simples, inicialmente somente leitura.
5. Habilitar decisões internas reversíveis com teste em mundo descartável.
6. Integrar reuniões, tratados e diplomacia usando APIs nativas confirmadas.
7. Implementar diálogos/eventos de RPG e persistência sem corromper saves.

## Arquivos

- `mod.json`: manifesto do NeoModLoader.
- `src/ObservatorioMod.cs`: entrada e protótipo de regras políticas.
- `docs/STATUS.md`: progresso, requisitos e limitações confirmadas.
- `docs/TEST_PROTOCOL.md`: etapas de teste no Android.

## Compatibilidade e segurança

O código atualizado foi testado pelo usuário através da linha de autoteste no log, mas ainda não foi compilado de forma independente por uma toolchain externa. Referências públicas de APIs da versão 0.51.2 são apenas pistas para a 0.51.4. Nenhuma relação, guerra, cidade ou unidade nativa é alterada no estado atual. Não incluir DLLs do jogo/carregador nem reutilizar código ou assets do Worldfall.
