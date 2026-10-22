# Observatório dos Reinos — WorldBox Android

Projeto original para acrescentar governo e diplomacia ao controle nativo de rei/rainha. Alvo informado: WorldBox Android 0.51.4 + NeoModLoader.

## 0.5.7-dev
- Canvas persistente como raiz de tela cheia; `SafeAreaRoot` filho é reajustado pelo controlador principal após rotação.
- Atalho `GOVERNO` permanece disponível quando o painel está fechado, mesmo se o botão `GOV` for localizado perto das informações nativas.
- Prefix Harmony preventivo tenta pular a atualização auxiliar de seleção/tooltip apenas durante `StatsWindow.OnEnable` / `WindowMetaGeneric.OnEnable`; callers normais seguem originais.
- Finalizer é a segunda barreira: quando a NullReference conhecida ocorre, procura fechar o `ScrollWindow` externo inteiro e abrir o menu alternativo.
- Logs mostram estado real de instalação dos patches e FullName/MVID das assemblies em runtime.

## Compilação
O CI anterior passou contra stubs Unity/NML com 0 erros/avisos. Uma refatoração posterior foi corretamente rejeitada pelo CI porque o helper `ConstruirHarmonyMethod` havia sumido durante a edição; ele foi restaurado no commit `dd553b5cf329ee0e7e84cc58c390e3ca9bd3ed61`. Aguarde o CI desse commit antes de considerar `0.5.7-dev` validado. Esse build é uma verificação de fonte, não uma DLL instalável. O build real usa agora `netstandard2.1` e referências locais, mas não pode rodar no runner público sem as DLLs privadas.

## Investigação da janela
A DLL de referência fornecida contém `StatsWindow.OnEnable()` chamando `PowerTabController.showWorldTipSelected(String, Boolean)`, enquanto a stack do telefone mostra `showWhoIsSelected` e `SelectedPowerTabButtons`. Consulte [`docs/ASSEMBLY_INVESTIGATION.md`](docs/ASSEMBLY_INVESTIGATION.md) para hashes, MVIDs e IL. A nova linha `Runtime assembly fingerprint` deve identificar as DLLs exatas carregadas pelo telefone.

## Limitações
A revisão 0.5.7-dev não foi testada no Android e nenhuma DLL final foi validada contra o runtime real. As opções administrativas continuam demonstrativas e não alteram reinos, cidades, alianças ou guerras.