# Status técnico

## Ambiente confirmado
- WorldBox Android 0.51.4; NeoModLoader funciona no aparelho.
- O usuário confirmou que o carregador, o autoteste político e o diagnóstico de reflexão aparecem/funcionam no aparelho.
- Apenas o menu de mods apresenta corte visual na orientação vertical; o jogo funciona normalmente.

## Requisito central
Aproveitar o controle nativo de personagem: ao controlar um rei/rainha, o jogador deve receber ferramentas de governo para o reino daquele personagem. Funcionalidades desejadas: políticas internas, administração, contato com vilas/cidades, reuniões, tratados, alianças, guerras e interações de RPG. UI deve usar a identidade visual padrão do jogo.

## Implementado no ramo
- Entrada NML e modelo político interno de protótipo.
- Diagnóstico por reflexão somente leitura que tenta ler `MapBox.instance`, contar gerenciadores `kingdoms`, `cities`, `units` e localizar membros cujos nomes sugerem seleção, controle, personagem, rei/rainha, reino e cidade em tipos candidatos.
- Probe expandido com pistas adicionais e até 35 correspondências por tipo para facilitar a descoberta de APIs na build exata.

## Próximo teste
O usuário relatou que encontrou o diagnóstico e que funcionou, mas não colou os valores das linhas no chat. A versão mais recente ampliou o diagnóstico; ainda deve ser validada no aparelho. Procurar no log:
- `Diagnostico do mundo:`
- `Diagnostico de API (somente leitura):`

O código não deve ser considerado uma implementação de detecção de rei até que a API/valores explícitos sejam identificados. Também não há painel de governo jogável ou ações diplomáticas integradas neste momento.

## Próximas fases
1. Validar o probe expandido, obter os membros reais disponíveis.
2. Implementar leitor real de personagem/reino/cidade de modo fail-soft e somente leitura.
3. Detectar o personagem que está sendo controlado; confirmar o papel de rei/rainha.
4. Criar um painel simples com estilo do WorldBox, inicialmente somente leitura.
5. Adicionar políticas internas reversíveis e testar em mundo descartável.
6. Implementar reuniões, tratados, alianças, guerras e eventos de RPG usando APIs confirmadas.

## Limites e segurança
- O protótipo político atual não altera o mundo; os reinos de diagnóstico são fictícios.
- A sonda de reflexão não chama métodos que alterem o jogo.
- APIs encontradas em documentação comunitária de WorldBox 0.51.2 são pistas, não confirmação da versão 0.51.4.
- Nenhuma DLL do jogo/loader nem código/assets de Worldfall incluídos.
