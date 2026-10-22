# Status técnico

## Ambiente informado
- WorldBox: 0.51.4.
- Plataforma: Android.
- NeoModLoader já funciona no aparelho do usuário; foi usado para executar o Worldfall.
- Foram fornecidos assemblies do jogo, NeoModLoader e Harmony.

## Progresso
- Branch de trabalho: `scaffold/observatorio-dos-reinos`.
- Manifesto atualizado para as chaves documentadas do NML (`GUID`, `Dependencies`, `OptionalDependencies`, `IncompatibleWith`, `UsePublicizedAssembly`).
- Entrada inicial implementada com `NeoModLoader.api.BasicMod<Main>` e `OnModLoad()`, seguindo documentação pública atual do NML.
- A entrada inicial registra mensagens no log e não modifica o mundo.
- PR #1 atualizada; continua draft e não deve ser tratada como lançamento.

## Descoberta preliminar da API do jogo
Documentação comunitária para WorldBox 0.51.2 descreve `MapBox.instance.kingdoms` como `KingdomManager` e `MapBox.instance.cities` como `CityManager`; o manager implementa enumeração de entidades vivas. Uma fonte de código de outra versão também mostra `KingdomManager : MetaSystemManager<Kingdom, KingdomData>`.

**Ressalva:** essa evidência não é da assembly 0.51.4 fornecida pelo usuário. Tratar apenas como pista, não como confirmação para compilar. Antes de usar os tipos diretamente, verificar seus metadados na DLL exata ou compilar um adapter que possa ser validado com log real.

## Pendências concretas
- Compilar o fonte via NML no dispositivo e obter log real.
- Confirmar que a versão instalada fornece o mesmo namespace e assinatura `BasicMod<T>.OnModLoad()`.
- Confirmar os tipos/membros de Kingdom/City/MapBox na assembly exata do 0.51.4.
- Só depois implementar painel 2D e listagem somente leitura.

## Não afirmar ainda
- Nenhuma DLL deste projeto foi compilada ou testada no jogo.
- Ainda não há integração de dados de reinos, diplomacia ou controle de personagens.

## Regras de estabilidade
1. Confirmar assinaturas antes de chamar APIs específicas.
2. Manter o protótipo inicial somente de diagnóstico/leitura.
3. Não alterar guerras, relações, território ou sucessão antes de validar leitura.
4. Não incluir DLLs do jogo ou do carregador na distribuição.
5. Erros de inicialização não podem tentar alterar o mundo.
