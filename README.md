# Observatório dos Reinos — WorldBox Android

Projeto original para acrescentar governo e diplomacia ao controle nativo de rei/rainha. Alvo informado: WorldBox Android 0.51.4 + NeoModLoader.

## Revisão 0.5.8-dev
- Canvas persistente em tela cheia com `SafeAreaRoot` filho, recalculado pela área segura após rotação. O botão reserva `GOVERNO` permanece acessível.
- Prefix Harmony estabelece um escopo de abertura em `StatsWindow.OnEnable`. O helper de seleção/tooltip é ignorado apenas durante esse escopo; postfix e finalizer restauram o valor anterior por `__state`, incluindo chamadas aninhadas.
- Finalizer é segunda barreira para a NullReference conhecida e tenta fechar a janela externa `ScrollWindow` antes de abrir o painel alternativo.
- Logs registram instalação real dos hooks e FullName/MVID das assemblies carregadas.

## Resultado de compilação
A revisão 0.5.8-dev passou no GitHub Actions contra stubs Unity/NeoModLoader com **0 erros e 0 avisos**: https://github.com/Jin27012007/Worldbox-mod-geo/actions/runs/39846610447. Isso valida sintaxe e tipos internos contra API simulada, mas não é uma DLL instalável nem uma validação da API real. A compilação real foi ignorada porque as DLLs privadas não estão no runner público.

## Investigação
A assembly enviada chama `PowerTabController.showWorldTipSelected`, mas o log do telefone menciona `showWhoIsSelected`; também há divergência nos nomes da classe de botões. Consulte [`docs/ASSEMBLY_INVESTIGATION.md`](docs/ASSEMBLY_INVESTIGATION.md) para hashes, MVIDs e análise de IL. O fingerprint adicionado ao log do mod permite identificar o runtime real.

## Limitações
A revisão 0.5.8-dev ainda não foi testada no telefone. A causa exata da NullReference continua não comprovada; as ações políticas permanecem demonstrações e não alteram reinos reais.