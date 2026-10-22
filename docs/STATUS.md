# Status técnico

## Ambiente e testes
- WorldBox Android 0.51.4; NeoModLoader funciona no aparelho.
- O usuário confirmou que o carregamento, o autoteste político e a sonda de diagnóstico rodaram em testes anteriores.
- Ao controlar um rei, abrir a janela nativa de status do reino deixa a interface cortada e dispara repetidamente `NullReferenceException`.
- Stack trace enviado em 2026-10-22: `PowerTabController.showWhoIsSelected(...)` -> `StatsWindow.OnEnable()` -> `WindowMetaGeneric<T>.OnEnable()` -> `GameObject.SetActive` -> `ScrollWindow.show(...)` / `PowerTabLibrary.defaultMainInfoClick(...)` -> `SelectedPowerTabButtons.clickOpenMain()` -> `EventSystem.Update()`. Isso aponta para a UI nativa do jogo, não para uma exceção originada diretamente no overlay do Observatório.
- A mensagem `Disabling Discord Integration` / `discord_game_sdk` aparece separadamente e parece ser um erro de integração Discord; não é a causa demonstrada do painel cortado.
- O usuário também não encontrou o menu próprio do Observatório, mesmo após uma alteração para abri-lo por padrão. Portanto, a UI IMGUI criada pelo protótipo ainda não está visivelmente validada; não presumir que o código mais recente esteja sendo executado/renderizado no aparelho.

## Requisito central
Aproveitar o controle nativo de personagem: ao controlar rei/rainha, o usuário deve receber ferramentas de governo do reino correspondente. Metas: políticas internas, administração, contato com vilas/cidades, reuniões, tratados, alianças, guerras e interações de RPG. Usar a identidade visual padrão do WorldBox sempre que possível.

## Implementado no ramo `scaffold/observatorio-dos-reinos`
- Entrada NeoModLoader e modelo político interno de protótipo.
- Sonda de reflexão somente leitura para buscar `MapBox.instance`, contagens de reinos/cidades/unidades e nomes de membros candidatos.
- Overlay visual IMGUI com botão `Governo`, quatro abas, campos de texto, toggles, rolagem e mensagens de rascunho. Na revisão `0.4.1-dev`, o painel está configurado para abrir por padrão.
- **O overlay ainda não foi confirmado visivelmente no Android.** A existência do código no ramo não comprova que foi compilado/carregado/renderizado.

## Diagnóstico atual e próximo passo
1. Não aplicar um patch especulativo que simplesmente suprime `StatsWindow.OnEnable`; isso pode esconder o erro sem restaurar a UI nativa e não identifica a referência nula.
2. Inspecionar os metadados da assembly exata do jogo para entender `StatsWindow`, `PowerTabController.showWhoIsSelected` e o campo/objeto nulo; só então decidir se é necessário patch Harmony compatível com esta versão.
3. Confirmar a versão/código do mod efetivamente carregado: procurar no log por `Menu visual administrativo criado` e `Observatorio dos Reinos carregado`. Se essas linhas não aparecerem, investigar compilação/carregamento antes de depurar o `OnGUI`.
4. Só depois testar o overlay em Android e orientação horizontal.
5. Obter as linhas `Diagnostico do mundo:` e `Diagnostico de API (somente leitura):` para mapear ator controlado/reino/cidade.

## Segurança
- O protótipo atual não deve alterar entidades nativas do mundo.
- Não incluir DLLs do jogo ou NeoModLoader no repositório.
- Não reutilizar código ou assets de Worldfall.
- Documentação comunitária de 0.51.2 é pista, não prova de compatibilidade com 0.51.4.
