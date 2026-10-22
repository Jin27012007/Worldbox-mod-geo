# Status técnico

## Ambiente informado
- WorldBox: 0.51.4.
- Plataforma: Android.
- NeoModLoader já funciona no aparelho do usuário; foi usado para executar o Worldfall.
- Assembly pública do jogo, NeoModLoader e Harmony foram fornecidos para análise.

## Progresso do projeto
- Branch de trabalho: `scaffold/observatorio-dos-reinos`.
- Manifesto atualizado para usar chaves documentadas do NML: `GUID`, `Dependencies`, `OptionalDependencies`, `IncompatibleWith` e `UsePublicizedAssembly`.
- Entrada inicial implementada com `NeoModLoader.api.BasicMod<Main>` e `OnModLoad()`, baseada na documentação pública de NML.
- A entrada apenas registra duas mensagens no log; não altera o mundo.

## Pendências concretas
- Compilar o fonte pelo próprio NML no dispositivo e obter log real.
- Confirmar que a versão específica do loader fornece o mesmo namespace e a mesma assinatura `BasicMod<T>.OnModLoad()`.
- Analisar os metadados de `Assembly-CSharp-Publicized.dll` para identificar os tipos reais envolvidos em reinos, cidades e governantes.
- Implementar o painel 2D somente após o smoke test funcionar.

## Não afirmar ainda
- Nenhuma DLL deste projeto foi compilada ou testada no jogo nesta etapa.
- Ainda não há integração de dados de reinos, diplomacia ou controle de personagens.

## Regras de segurança/estabilidade
1. Confirmar assinaturas antes de chamar APIs específicas do jogo.
2. Manter o protótipo inicial somente de diagnóstico/leitura.
3. Não alterar guerras, relações, território ou sucessão antes de haver leitura estável e testes.
4. Não incluir DLLs do jogo ou do carregador na distribuição do mod.
5. Tratar falha de inicialização sem tentar modificar o mundo.
