# Status técnico

## Ambiente confirmado
- WorldBox Android 0.51.4; NeoModLoader funciona no aparelho.
- O usuário confirmou que o carregador, o autoteste político e o diagnóstico de reflexão aparecem/funcionam no aparelho.
- O usuário conseguiu controlar um personagem rei usando o controle nativo do jogo.
- Na captura mais recente, uma janela/painel do jogo fica cortada na lateral direita em orientação vertical; o restante da jogabilidade parece funcionar. Ainda não está confirmado se esse painel é uma janela nativa do jogo ou uma UI do mod.

## Requisito central
Aproveitar o controle nativo de personagem: ao controlar um rei/rainha, o jogador deve receber ferramentas de governo para o reino daquele personagem. Funcionalidades desejadas: políticas internas, administração, contato com vilas/cidades, reuniões, tratados, alianças, guerras e interações de RPG. UI deve usar a identidade visual padrão do jogo.

## Implementado no ramo
- Entrada NML e modelo político interno de protótipo.
- Diagnóstico por reflexão somente leitura que tenta ler `MapBox.instance`, contar gerenciadores `kingdoms`, `cities`, `units` e localizar membros cujos nomes sugerem seleção, controle, personagem, rei/rainha, reino e cidade em tipos candidatos.
- Probe expandido com pistas adicionais e até 35 correspondências por tipo para facilitar a descoberta de APIs na build exata.

## Testes e limitações atuais
- O usuário confirmou que o diagnóstico anterior rodou, mas não compartilhou os valores completos das linhas no chat; portanto, a detecção do rei pelo mod ainda não está comprovada.
- O controle nativo do rei funciona no aparelho. A janela visível na captura está cortada em orientação vertical; precisamos identificar se é UI vanilla ou do mod antes de propor uma correção.
- Não há painel de governo próprio, decisões jogáveis ou ações diplomáticas integradas.

## Próximas fases
1. Obter os valores completos de `Diagnostico do mundo:` e `Diagnostico de API (somente leitura):` para validar os membros da build 0.51.4.
2. Identificar a janela cortada e inspecionar APIs/layout nativos para adaptar ao espaço seguro da tela vertical.
3. Implementar leitor real de personagem/reino/cidade de modo fail-soft e somente leitura.
4. Confirmar o personagem controlado e seu papel de rei/rainha.
5. Criar um painel de governo simples com estilo do WorldBox, inicialmente somente leitura.
6. Adicionar políticas internas reversíveis e testar em mundo descartável.
7. Implementar reuniões, tratados, alianças, guerras e eventos de RPG usando APIs confirmadas.

## Limites e segurança
- O protótipo político atual não altera o mundo; os reinos de diagnóstico são fictícios.
- A sonda de reflexão não chama métodos que alterem o jogo.
- APIs encontradas em documentação comunitária de WorldBox 0.51.2 são pistas, não confirmação da versão 0.51.4.
- Nenhuma DLL do jogo/loader nem código/assets de Worldfall incluídos.
