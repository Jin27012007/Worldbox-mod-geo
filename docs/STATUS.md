# Status técnico

## Ambiente confirmado pelo usuário
- WorldBox Android: 0.51.4.
- NeoModLoader funciona no aparelho do usuário.
- O mod foi reconhecido e a mensagem de diagnóstico apareceu no log.
- O protótipo político `0.2.0-dev` também inicializou o autoteste `reinos=2, relacoes=1, tipo=Comercio, opiniao=10` no aparelho, confirmado pelo usuário.
- A janela do menu de mods fica visualmente cortada na orientação vertical; o resto do jogo permanece normal.

## Direção do produto — requisito principal
O usuário quer aproveitar a mecânica nativa de controlar personagens. Quando o jogador controlar um rei ou rainha, o mod deverá oferecer recursos de governo para o reino desse personagem. Não deve ser um simulador político paralelo com reinos fictícios.

### Experiência desejada
- Identificar o personagem atualmente controlado e verificar se é rei/rainha.
- Identificar o reino e a cidade/vila associados a esse personagem.
- Painel de governo para ditar regras e administrar o reino.
- Reuniões com vilas/cidades e representantes locais.
- Interação com outros reinos: solicitar reuniões, negociar e firmar tratados, alianças, rivalidades e guerras.
- Reuniões e diálogos de RPG com escolhas e consequências no mundo.
- Preservar os controles nativos de personagem e a simulação base; integrar ações de forma gradual.

## Implementado no ramo de trabalho
- Manifesto NML básico.
- Entrada `BasicMod<Main>` com diagnóstico.
- Modelo interno de protótipo: cadastro de entidade política, estabilidade, legitimidade, tesouro, tipos de relação diplomática e opinião.
- Autoteste com dois reinos fictícios, usado somente para validar inicialização das regras.

## Limites atuais
- `diagnostico-a` e `diagnostico-b` são dados fictícios de teste, não reinos reais. Eles são temporários e não devem ser tratados como parte da experiência final.
- A camada política ainda não detecta o personagem controlado nem lê reinos/cidades do mundo.
- Não existe painel de governo, reuniões, tratados funcionais, decisões administrativas ou salvamento persistente ainda.
- Nenhuma relação, guerra, cidade ou unidade nativa é alterada pelo protótipo atual.
- O código atualizado foi testado pelo usuário por meio do log de autoteste, mas ainda não foi compilado de forma independente por uma toolchain externa.
- Referências comunitárias preliminares de APIs de WorldBox 0.51.2 são pistas, não garantia para a assembly 0.51.4.

## Próxima sequência técnica
1. Inspecionar os metadados da assembly exata 0.51.4 para localizar personagem controlado, cargo/identidade de rei, reino, cidade e API de UI.
2. Criar uma ponte de leitura somente leitura para dados reais e confirmar os valores no log.
3. Detectar o personagem controlado e mostrar contexto do reino a que pertence.
4. Criar um painel de governo inicial que não mude o mundo.
5. Adicionar decisões internas pequenas e reversíveis, com logs e testes em mundo descartável.
6. Implementar reuniões, tratados e diplomacia, conectando cada ação à API nativa confirmada.
7. Projetar persistência segura e compatibilidade com saves.

## Princípios de segurança
- Não modificar a simulação nativa antes de confirmar APIs e fluxo de erro.
- Testar mudanças que alterem o mundo em um mundo descartável com backup.
- Não incluir DLLs do jogo ou do carregador na distribuição.
- Não reutilizar código ou assets do Worldfall.
