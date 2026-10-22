# Observatório dos Reinos (WorldBox 0.51.4)

Mod original para WorldBox no Android, usando NeoModLoader, com foco em geopolítica, diplomacia e interações de RPG. A proposta é manter a simulação original e adicionar um painel 2D leve — sem modo 3D.

## Primeiro marco: smoke test do carregador

A classe de entrada agora segue o padrão documentado do NML:

```csharp
using NeoModLoader.api;

public class Main : BasicMod<Main>
{
    protected override void OnModLoad()
    {
        LogInfo("Observatorio dos Reinos carregado.");
    }
}
```

Este código serve para verificar o ciclo de vida do mod e escrever no log. **Ainda não foi compilado nem executado no aparelho.** A interface de entrada corresponde à documentação pública consultada para NeoModLoader, mas deve ser confirmada contra a DLL e a versão específica instalada.

## Estrutura

- `mod.json`: manifesto usando as chaves documentadas do NML (`GUID`, `Dependencies`, `UsePublicizedAssembly` etc.).
- `src/ObservatorioMod.cs`: entrada do mod, atualmente só registra carregamento.
- `docs/STATUS.md`: fatos e pendências técnicas.
- `docs/TEST_PROTOCOL.md`: plano de validação no Android.

## Próximos passos

1. Fazer o NML compilar o fonte no aparelho e verificar o log de carregamento.
2. Resolver quaisquer erros de compilação com o log exato, sem adivinhar APIs.
3. Adicionar um painel 2D simples.
4. Inspecionar as APIs do jogo para exibir reinos/cidades/governantes em modo somente leitura.
5. Só depois avançar para diplomacia e interação de RPG.

## Princípios

- Não mudar a simulação na etapa inicial.
- Não incluir cópias das DLLs do jogo ou do carregador no pacote do mod.
- Não reutilizar código, modelos ou assets do Worldfall.
- Não afirmar compatibilidade até testar no WorldBox 0.51.4 no aparelho.
