# Status técnico — Observatório dos Reinos 0.6.4-dev

## Interface e ponto de entrada

- A integração do botão GOV continua experimental; não existe confirmação visual/runtime no WorldBox Android ainda.
- O mod procura conectar um postfix Harmony ao Update de classes SelectedPowerTab*Buttons, priorizando SelectedPowerTabButtons e SelectedPowerTabTopButtons.
- O código tenta inserir GOV como irmão de um controle nativo. O caminho de janela é separado: o clique chama ObservatorioJanelaNativa.Abrir().
- A captura do aparelho mostra uma NullReferenceException no caminho nativo de abertura da janela de informações. Isso não prova que o Observatório causou o erro. Ver docs/STACK_TRACE_0.51.4.md e docs/UI_CONTRACT_0.51.4.md.
- As capturas também indicam discrepância entre a build instalada (0.1.0-dev, marcada como “won't load”) e o código-fonte experimental (0.6.4-dev). Ver docs/INSTALLED_BUILD_TRIAGE.md.

## Fundação de build desta branch

- ObservatorioDosReinos.csproj é o projeto de produção, target netstandard2.1 e inclui automaticamente todos os arquivos C# em src/**.
- scripts/prepare_references.py prepara referências privadas a partir do ZIP Android fornecido; references/ é ignorado pelo Git e não deve ser publicado.
- scripts/validate_repository.py verifica manifesto, projeto, versão e idiomas.
- scripts/package_mod.py só cria um ZIP quando uma DLL de mod já compilada existe; o ZIP não copia assemblies de Unity/WorldBox/NML/Harmony.
- A GitHub Action executa validação estática, testes de pipeline e compilação/testes contra stubs. Ela compila e empacota a DLL real apenas se todas as referências privadas estiverem presentes no runner. Se faltarem, registra SKIP e não cria um pacote falso; referências parciais ou erro na compilação real fazem o job falhar.
- O fluxo de build real e instalação via Codespaces está em docs/BUILD_AND_RELEASE.md. As regras para adicionar fontes modulares estão em docs/ADDING_FEATURES.md.

## Validação atual e limites

A revisão da pipeline passou em validação estática e nos testes automatizados de stubs/pipeline na GitHub Action. Isso confirma a infraestrutura desses testes, não a compilação contra as DLLs privadas nem a execução em Unity/WorldBox Android. Ainda não há base para declarar o botão GOV funcional.

As opções de governo, diplomacia e RPG existentes continuam sendo registros locais da sessão; ainda não alteram os reinos reais do jogo.
