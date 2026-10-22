# Observatório dos Reinos — WorldBox Android

Projeto original para acrescentar governo e diplomacia ao controle nativo de rei/rainha. Alvo informado: WorldBox Android 0.51.4 + NeoModLoader.

## 0.5.9-dev
- Canvas em tela cheia com `SafeAreaRoot` interno e atalho reserva `GOVERNO` que deve permanecer acessível na rotação.
- Prefix Harmony tenta bloquear a atualização auxiliar de seleção/tooltip apenas durante a inicialização do status nativo. Postfix e finalizer restauram o escopo usando `__state`.
- A procura de Harmony agora é tolerante a diferenças no carregamento de assemblies e registra FullName/MVID de todos os tipos relevantes **antes de decidir se consegue aplicar patches**.
- O finalizer procura fechar a janela externa `ScrollWindow`, não apenas o componente interno.

## Build confirmado
Execução GitHub Actions: https://github.com/Jin27012007/Worldbox-mod-geo/actions/runs/39850928817
- Fonte compilado contra stubs: 0 warnings / 0 errors.
- Smoke test: prefix só bloqueia dentro do escopo de status e postfix/finalizer restauram o estado.
- Isso não gera DLL instalável e não valida a API real. A build com DLLs reais está preparada em `tests/CompileWorldBoxRefs.csproj`, mas é pulada em runners públicos porque as referências locais privadas não estão em `references/`.

## Erro nativo ainda em aberto
O log do aparelho usa `showWhoIsSelected`/`SelectedPowerTabButtons`, enquanto as assemblies fornecidas usam `showWorldTipSelected`/`SelectedPowerTabTopButtons`. Hashes, MVIDs e IL estão em [docs/ASSEMBLY_INVESTIGATION.md](docs/ASSEMBLY_INVESTIGATION.md). A causa exata da referência nula ainda não foi provada e a versão 0.5.9-dev não foi confirmada no telefone.

## Limites
As ações do painel continuam demonstrativas e não alteram reinos reais. Não considere o erro corrigido até testar a versão no dispositivo e verificar a linha `Runtime assembly fingerprint`.