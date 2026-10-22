# Status técnico

## Ambiente confirmado pelo usuário
- WorldBox Android: 0.51.4.
- NeoModLoader funciona no aparelho do usuário.
- O menu de mods reconhece o Observatório dos Reinos e o usuário confirmou que a mensagem de carregamento apareceu no log.
- A janela do menu de mod fica visualmente cortada na orientação vertical; o restante do jogo permanece normal.

## Implementado no ramo de trabalho
- Manifesto NML básico.
- Entrada `BasicMod<Main>` com mensagens de diagnóstico.
- Primeira camada de regras próprias do mod em `src/ObservatorioMod.cs`:
  - registro de reino por ID e nome;
  - estabilidade, legitimidade e tesouro com limites;
  - relações diplomáticas locais: neutra, comércio, aliança, rivalidade e guerra;
  - opinião diplomática de -100 a +100;
  - armazenamento das relações sem duplicar par A-B e B-A;
  - autoteste de duas entidades políticas de exemplo, registrado no log.

## Limites fundamentais desta versão
- Os reinos de teste são dados internos do mod. Ainda não estão ligados aos reinos reais que existem no mundo.
- Relações, opinião, estabilidade, legitimidade e tesouro são apenas estado do mod: não modificam guerras, comércio, cidades, população ou IA nativa do WorldBox.
- Ainda não existe janela/painel próprio de gestão, salvamento persistente, eventos políticos ou interação RPG.
- O código atualizado ainda precisa ser recompilado/carregado pelo NML no aparelho e o novo autoteste confirmado no log.
- A investigação preliminar de APIs de reino/cidade encontrada publicamente é referente a WorldBox 0.51.2, não à assembly exata 0.51.4 do usuário. Usá-la como pista, não como garantia de compatibilidade.

## Próxima etapa técnica
1. Instalar esta atualização de fonte mantendo a build anterior como backup.
2. Confirmar no log `reinos=2, relacoes=1, tipo=Comercio, opiniao=10`.
3. Obter os erros exatos caso a compilação falhe.
4. Verificar na assembly 0.51.4 os tipos e propriedades de reinos/cidades e só então adicionar leitura real do mundo.
5. Só depois criar UI e ligar as regras diplomáticas ao mundo, com testes em mundo descartável.

## Segurança
- Não altera entidades do jogo nessa etapa.
- Não inclui DLLs do jogo ou do carregador.
- Não reutiliza código ou assets do Worldfall.
