# Observatório dos Reinos — WorldBox Android

Projeto original para acrescentar governo e diplomacia ao controle nativo de rei/rainha. Alvo informado: WorldBox Android 0.51.4 + NeoModLoader.

## 0.5.6-dev
- Canvas persistente como raiz de tela cheia; `SafeAreaRoot` filho é reajustado pelo controlador principal após rotação.
- Atalho `GOVERNO` permanece visível quando o painel está fechado, mesmo se o botão `GOV` for localizado perto das informações nativas.
- Prefix Harmony preventivo tenta pular a atualização auxiliar de seleção/tooltip apenas durante `StatsWindow.OnEnable` / `WindowMetaGeneric.OnEnable`; callers normais seguem originais.
- Finalizer é a segunda barreira: quando a NullReference conhecida ocorre, procura fechar o `ScrollWindow` externo inteiro e abrir o menu alternativo.
- Logs mostram estado real de instalação dos patches e FullName/MVID das assemblies em runtime.

## Compilação verificada
GitHub Actions passou a checagem independente contra stubs com 0 erros/0 avisos na revisão 0.5.5-dev. Isso é uma compilação do fonte contra API simulada — não uma DLL instalável. O passo de referências reais foi corretamente pulado porque as DLLs locais privadas não estão no runner público. 0.5.6-dev ainda precisa de seu próprio resultado do CI, e nenhuma DLL final foi validada contra o runtime do jogo.

## Investigação da exceção
O log do aparelho reporta `showWhoIsSelected`/`SelectedPowerTabButtons`, enquanto a assembly fornecida contém `showWorldTipSelected`/`SelectedPowerTabTopButtons`. O diagnóstico de IL, hashes e MVIDs está em [`docs/ASSEMBLY_INVESTIGATION.md`](docs/ASSEMBLY_INVESTIGATION.md). Para estabelecer a causa real, precisamos comparar o MVID impresso pelo mod em execução no aparelho com esses binários de referência.

## Limitações
A interface 0.5.6-dev ainda não foi confirmada no telefone. Ações administrativas são rascunhos e não alteram reinos reais. Não copiar DLLs proprietárias do jogo/loader ao repositório público.