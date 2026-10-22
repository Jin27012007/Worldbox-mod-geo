# Observatório dos Reinos — runtime proof baseline

Esta branch é uma base de diagnóstico isolada para WorldBox Android + NeoModLoader. Ela **não cria botão, janela ou mecânica de jogo**. O objetivo é comprovar que o NML compilou e executou o código-fonte e coletar a estrutura real da aba de controle de NPC antes de voltar a inserir UI.

## Por que esta branch existe

As compilações anteriores passaram contra stubs, mas isso não comprovou o carregamento nem a renderização no jogo. Além disso, as referências de assembly disponíveis aqui contêm `SelectedPowerTabTopButtons`, enquanto o log antigo menciona `SelectedPowerTabButtons` e `showWhoIsSelected`. Essas diferenças precisam ser resolvidas com dados do runtime que realmente está rodando no telefone.

## O que ela registra

Ao carregar, o mod registra:
- build e versões do WorldBox/Unity, plataforma e resolução;
- nome, versão e MVID das assemblies-chave;
- existência de `SelectedPowerTabButtons`, `SelectedPowerTabTopButtons`, `PowerTabController`, `SelectedUnit` e `SelectedObjects`;
- existência e visibilidade do método `Update()`, além dos nomes e tipos dos campos relacionados a botões;
- após o Unity começar a executar o mod, componentes de controle vivos na cena, hierarquia de pais, RectTransform, Canvas, referências dos botões e estado da seleção.

Os registros começam com `[OR-BASELINE]`. A presença de `STAGE=ON_MOD_LOAD`, `STAGE=ON_ENABLE`, `STAGE=START_OK` e `STAGE=UPDATE_OK` diferencia compilação/carregamento de execução do componente.

## Instalação para um único teste de base

1. Faça backup e remova a pasta antiga do Observatório de `Mods` antes de extrair esta branch, para não deixar duas pastas com o mesmo GUID.
2. Baixe o ZIP desta branch: https://github.com/Jin27012007/Worldbox-mod-geo/archive/refs/heads/foundation/runtime-proof.zip
3. Extraia na pasta `Mods` e confirme que `mod.json` está diretamente na raiz da pasta do mod.
4. Inicie o jogo e carregue um mundo. Selecione uma unidade/NPC e abra a aba de controle.
5. Guarde as linhas `[OR-BASELINE]` do log. Não precisa fazer várias tentativas nem alterar o código.

## Como interpretar

- **Nenhuma linha `[OR-BASELINE]`:** a classe do mod não chegou a executar. Verifique o erro de compilação/carregamento do NML, a pasta instalada e se há pastas duplicadas; não é um problema de posição do botão ainda.
- **`ON_MOD_LOAD`, mas sem `UPDATE_OK`:** o mod inicializou, mas o componente não entrou no ciclo de atualização esperado.
- **`TYPE_PROBE ... found=false`:** a classe indicada não existe nas assemblies realmente carregadas.
- **`TYPE_PROBE ... found=true`, mas `UI_SCAN ... NO_LIVE_SCENE_COMPONENT`:** o tipo existe, mas o objeto de controle não foi encontrado na cena naquele momento.
- **`UI_COMPONENT` e `UI_BUTTON`:** informam os nomes e as hierarquias reais a usar na integração.

## Testes automatizados

O GitHub Actions compila e executa testes contra doubles de Unity/NML para validar a sonda. Isso não é descrito como teste do jogo real. O NML do aparelho compila o source mod contra as assemblies locais; o log do runtime é a evidência necessária para validar a integração real.

Não publique DLLs do WorldBox/NeoModLoader neste repositório público. O projeto inclui um csproj opcional para compilação local contra referências privadas, que devem permanecer fora do controle de versão.
