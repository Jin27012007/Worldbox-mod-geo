# Revisão do Neo Mod Maker para o Observatório dos Reinos

Data da revisão: 2026-10-23.

## Fontes examinadas

- Projeto: https://github.com/AgricheX/Neo-Mod-Maker
- README: https://github.com/AgricheX/Neo-Mod-Maker/blob/main/README.md
- Gerador C#: https://github.com/AgricheX/Neo-Mod-Maker/blob/main/js/codegen.js
- Gerador de tipos e código personalizado: https://github.com/AgricheX/Neo-Mod-Maker/blob/main/js/kinddefs.js
- Template de inicialização: https://github.com/AgricheX/Neo-Mod-Maker/blob/main/js/finishtpl.js
- Binários de referência enviados para esta investigação pelo usuário. Não foram publicados no repositório.

## O que foi confirmado no código-fonte

O aplicativo roda como página HTML/JavaScript e gera um ZIP contendo, entre outros itens, `mod.json`, `default_config.json`, `SPRITES.txt`, traduções e arquivos C# dentro de `code/`.

O ponto de entrada gerado usa `Main : BasicMod<Main>` e chama inicializadores a partir de `OnModLoad()`. Isso é uma referência útil para o formato de um mod NML.

A seção avançada de código personalizado permite três modos: bloco C# dentro de um método `Init()`, patch Harmony gerado por atributos, ou arquivo C# completo. O próprio gerador declara que não verifica esse código; erros de compilação são descobertos no log do jogo. Portanto, o recurso não é um compilador nem uma garantia de que o mod funcionará.

## A diferença entre os botões

O botão padrão da seção de poderes não é um botão arbitrário de interface. O gerador cria um `GodPower` com o template `$template_spawn_actor$`, atribui `actor_asset_id` e adiciona o controle a uma aba por meio de `PowerButtonCreator.CreateGodPowerButton` e `PowerButtonCreator.AddButtonToTab`. Esse fluxo é voltado a poderes do jogo, como invocar uma criatura.

O Observatório precisa de um atalho junto aos controles do NPC selecionado e de uma janela própria. Isso exige código personalizado que encontre ou observe o painel nativo e crie o controle no contexto correto. Configurar apenas o botão padrão do criador não implementa esse comportamento.

## Identificador do mod: risco de exportação

O gerador transforma o campo de ID em namespace C# removendo todos os caracteres que não sejam letras, números ou sublinhados. Depois usa esse namespace transformado também como `GUID` no `mod.json`.

Assim, inserir diretamente o GUID atual `com.jin27012007.observatoriodosreinos` não preserva o GUID: os pontos são removidos. Uma exportação usada como substituição do mod existente exigiria corrigir e verificar o `GUID` do manifesto antes da instalação. Não se deve instalar a exportação como se fosse automaticamente a mesma identidade do mod.

## Compatibilidade com os arquivos de referência

O README do criador diz que ele mira a versão mais recente do WorldBox. Isso não confirma compatibilidade com WorldBox 0.51.4 nem com a cópia específica do NeoModLoader instalada no aparelho.

A DLL `NeoModLoader.dll` enviada pelo usuário tem SHA-256 `adeccd1dfab7adafc6c0a283af728dd5ef16d9c899e8471ff78ccf9e38756d88`. Os nomes `TabManager`, `CreateTab`, `PowerButtonCreator`, `AddButtonToTab` e `CreateGodPowerButton` aparecem nessa DLL. Isso confirma a presença desses nomes no binário de referência, mas a busca por strings, isoladamente, não prova assinaturas corretas, compilação ou execução em jogo.

A `Assembly-CSharp.dll` enviada também contém os nomes `SelectedPowerTabTopButtons` e `PowerTabController`. A investigação de assemblies já documentada neste repositório registrou uma divergência entre esses nomes e os nomes citados no log do aparelho. Essa divergência continua exigindo confirmação com o runtime real; o criador de mods não a resolve por si só.

## Decisão para o projeto

Usar o Neo Mod Maker como referência para a estrutura de exportação, o ponto de entrada NML e o formato dos arquivos/recursos é razoável. Não substituir o código do Observatório pelo botão padrão do criador e não tratar uma exportação como compatível até que seja conferida contra os assemblies exatos.

A próxima alteração funcional deve permanecer pequena e baseada em evidência: verificar a identidade do mod e a etapa real de carregamento primeiro; depois, validar separadamente a descoberta do painel do NPC, a criação visível do botão e a abertura da janela. Os testes de CI com stubs não equivalem a uma compilação com as DLLs do aparelho nem a um teste dentro do WorldBox.
