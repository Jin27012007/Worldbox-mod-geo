# Status técnico

## Ambiente confirmado
- WorldBox Android 0.51.4; NeoModLoader funciona no aparelho.
- O usuário confirmou que tanto o carregamento inicial quanto o autoteste político `reinos=2, relacoes=1, tipo=Comercio, opiniao=10` apareceram nos logs.
- Na orientação vertical, apenas a janela do menu de mods fica visualmente cortada; o jogo em si está normal.

## Requisito central do projeto
O mod deve aproveitar o controle nativo de personagem: quando o jogador controlar um rei/rainha, terá acesso ao governo do reino daquele personagem. O objetivo é administrar o reino, definir regras, contatar vilas/cidades, convocar reuniões e negociar tratados, alianças e guerras com outros reinos. Reuniões/diálogos de RPG devem produzir escolhas e consequências. A identidade visual deve aproveitar o padrão do jogo; não criar um framework de UI de aparência alheia sem necessidade.

## Implementação atual no ramo de trabalho
- Manifesto NML e entrada `BasicMod<Main>`.
- Modelos políticos locais de protótipo: estabilidade, legitimidade, tesouro, tipos de relação e opinião.
- Diagnóstico por reflexão recém-adicionado: procura `MapBox.instance`, tenta contar `kingdoms`, `cities` e `units`, e registra nomes de membros relacionados a seleção/controle/jogador/rei/reino/cidade em tipos candidatos. A inspeção apenas lê metadados e coleções; não chama APIs de mutação do jogo.

## Importante sobre a atualização mais recente
O código de diagnóstico foi enviado ao ramo, mas ainda não foi testado no telefone. Precisamos garantir que ele compila/carrega no NML Android. O log esperado inclui `Diagnostico do mundo: mapa=...` e `Diagnostico de API (somente leitura): ...`. Se a compilação falhar, usar a exceção real para corrigir.

## Ainda não implementado
- Detectar com certeza o personagem que o jogador está controlando.
- Identificar rei/rainha e seus vínculos reais de reino/cidade.
- Painel de governo ou botões de UI.
- Regras que alterem a simulação, reuniões, tratados, guerra, administração ou persistência.
- O modelo político ainda não é conectado a reinos reais; `diagnostico-a/b` são exemplos fictícios apenas para smoke test.

## Próximas fases
1. Validar no aparelho o novo diagnóstico de reflexão.
2. Com base nos nomes reais de membros exibidos no log, implementar uma leitura de dados reais e detecção do personagem controlado sem supor nomes de API.
3. Adicionar uma interface simples aproveitando componentes/estilo nativo do WorldBox, começando em modo somente leitura.
4. Implementar decisões internas pequenas e reversíveis em mundo descartável.
5. Habilitar reunião, diplomacia e eventos de RPG quando as APIs nativas correspondentes tiverem sido identificadas e testadas.

## Segurança e distribuição
- Nenhuma cidade, unidade, guerra ou relação nativa é alterada pela versão atual.
- Não embutir DLLs do jogo ou do NeoModLoader.
- Não reutilizar código ou assets de Worldfall.
- Referências públicas a APIs da versão 0.51.2 são apenas pistas, não prova de compatibilidade com a 0.51.4.
