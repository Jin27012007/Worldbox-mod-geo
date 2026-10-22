# Status técnico

## Ambiente-alvo informado
- WorldBox: 0.51.4 (versão informada pelo usuário).
- Plataforma: Android.
- Loader: NeoModLoader; foi fornecido um arquivo chamado `NeoModLoader.dll`.
- Assembleia pública do jogo: o ZIP `com.mkarpenko.worldbox.zip` foi informado como contendo `Assembly-CSharp-Publicized.dll` e `0Harmony.dll`.

## Confirmado nesta etapa
- Repositório: `Jin27012007/Worldbox-mod-geo`, branch padrão `main`.
- Foi criada a branch isolada `scaffold/observatorio-dos-reinos`.
- Objetivo: mod Android-first com UI 2D e foco em geopolítica; iniciar com diagnóstico/leitura, sem modificar a simulação.

## Ainda não confirmado
- Interface de entrada exigida pela versão instalada do NeoModLoader.
- Target .NET/Mono e referências necessárias para compilar no Android.
- Compatibilidade exata entre loader e WorldBox 0.51.4.
- APIs para ler reinos, cidades e governantes.
- Nenhuma DLL foi compilada ou testada no jogo nesta etapa.

## Regras
1. Não inventar nomes de classes, métodos de ciclo de vida ou campos de configuração do loader.
2. Confirmar assinaturas com metadados/reflection estática antes de implementá-las.
3. Encapsular acesso ao jogo em adaptadores pequenos e defensivos.
4. Não modificar guerras, relações, território ou sucessão antes de uma leitura estável.
5. Erros de inicialização devem ir ao log e não interromper a simulação.
