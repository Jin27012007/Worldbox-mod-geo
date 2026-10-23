# Observatório dos Reinos — WorldBox Android

Projeto experimental para WorldBox Android 0.51.4 + NeoModLoader. O código do atalho GOV ainda não foi comprovado no runtime Android; não trate o botão como corrigido.

## Desenvolvimento

O projeto real é ObservatorioDosReinos.csproj. Ele compila todos os arquivos C# sob src/ como biblioteca netstandard2.1 e usa referências locais do WorldBox/NeoModLoader. As DLLs do jogo não ficam no Git.

Leia:
- docs/BUILD_AND_RELEASE.md — preparar referências, compilar e empacotar.
- docs/ADDING_FEATURES.md — estrutura recomendada para novas funcionalidades.
- docs/STATUS.md — estado conhecido da integração experimental.
- docs/UI_CONTRACT_0.51.4.md, disponível na branch de investigação — membros nativos examinados.

## Build em duas camadas

A GitHub Action sempre valida os manifests, compila contra stubs e executa os testes de lógica. Isso verifica o código-fonte, não o jogo real.

Quando um conjunto completo de DLLs privadas existir no runner, a Action também compila o projeto de produção e cria um ZIP de mod. Sem referências, registra SKIP e não produz um ZIP falso. Referências incompletas ou erros na compilação real fazem o job falhar.

Para build local/Codespaces, prepare referências e execute:

    python3 scripts/prepare_references.py /path/to/com.mkarpenko.worldbox.zip
    python3 scripts/validate_repository.py
    dotnet build ObservatorioDosReinos.csproj --configuration Release
    python3 scripts/package_mod.py

O resultado fica em dist/. O pacote continua experimental até ser instalado e testado no WorldBox Android 0.51.4 com a versão correspondente do NeoModLoader.

## Estado funcional

Os registros de governo, diplomacia e RPG no código atual ainda são dados locais da sessão; não alteram reinos nativos. Uma compilação bem-sucedida tampouco comprova que o botão aparece, responde ao toque ou que a janela abre. Essas condições exigem logs e confirmação dentro do jogo.
