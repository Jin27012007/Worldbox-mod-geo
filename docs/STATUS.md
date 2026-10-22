# Status técnico

## Ambiente e testes
- WorldBox Android 0.51.4; NeoModLoader funciona no aparelho.
- O usuário confirmou que o carregamento, o autoteste político e a sonda de diagnóstico rodaram no aparelho.
- Ao controlar um rei, abrir o status nativo do reino causa uma janela parcialmente cortada na orientação vertical. O restante do jogo continua funcional.

## Requisito central
Aproveitar o controle nativo de personagem: ao controlar rei/rainha, o usuário deve receber ferramentas de governo do reino correspondente. Metas: políticas internas, administração, contato com vilas/cidades, reuniões, tratados, alianças, guerras e interações de RPG. Usar a identidade visual padrão do WorldBox sempre que possível.

## Implementado no ramo `scaffold/observatorio-dos-reinos`
- Entrada NeoModLoader e modelo político interno de protótipo.
- Sonda de reflexão somente leitura para buscar `MapBox.instance`, contagens de reinos/cidades/unidades e nomes de membros candidatos.
- Novo `MenuAdministracaoOverlay`: uma pequena aba/botão "Governo" abre uma janela ajustada às dimensões de tela, com quatro abas (Governo, Diplomacia, Reuniões e RPG), botões, caixas de texto, toggles e feedback de rascunho. Usa os estilos/tipografia da skin IMGUI fornecida pelo Unity e cores escuras/douradas aproximadas do WorldBox.
- O menu é uma estrutura visual inicial, não uma reprodução pixel-perfect da UI nativa. Não tenta corrigir diretamente a janela nativa de status do reino.
- Manifesto atualizado para `0.4.0-dev`.

## Limites que precisam ficar claros
- O overlay é criado no carregamento do mod e fica disponível como um botão próprio de teste; ainda não abre automaticamente ao controlar um rei.
- O rótulo personagem/reino deixa claro que a integração com a API real ainda está pendente.
- As opções são rascunhos visuais de sessão; não alteram políticas, tesouro, guerra, diplomacia, cidades ou unidades do jogo.
- A nova UI usa `UnityEngine`/IMGUI e ainda precisa ser compilada/carregada/testada no Android. Se falhar, o try/catch do `OnModLoad` registra o tipo de exceção, mas falhas de compilação do loader podem aparecer antes do `OnModLoad`.
- A origem exata do corte da janela nativa não foi localizada. A solução imediata é oferecer um painel responsivo alternativo; corrigir a UI nativa exige APIs/âncoras confirmadas para 0.51.4.

## Próximos passos
1. Testar se `0.4.0-dev` compila/carrega e se o botão Governo aparece em retrato e paisagem.
2. Ajustar área segura, escala e scroll conforme a captura real do usuário.
3. Com as linhas de diagnóstico completas, confirmar o ator controlado e o vínculo ao reino/cidade.
4. Fazer o painel mostrar informações reais em modo somente leitura.
5. Só depois ligar propostas administrativas, reuniões, tratados e escolhas de RPG a consequências testadas.

## Segurança
- A nova UI não chama APIs nativas de mutação.
- Nenhuma entidade nativa é alterada pelo protótipo atual.
- Não incluir DLLs do jogo ou NeoModLoader.
- Não reutilizar código ou assets de Worldfall.
- Documentação comunitária da versão 0.51.2 continua sendo pista, não prova de compatibilidade com 0.51.4.
