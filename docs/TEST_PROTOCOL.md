# Protocolo de testes — Android

## Antes de atualizar
- Faça uma cópia da pasta atual do Observatório dos Reinos e mantenha um mundo descartável para os testes.
- O código no ramo de trabalho agora contém regras políticas locais; a build anterior não deve ser tratada como se já contivesse essa atualização.

## Teste 1 — carregar o protótipo político
1. Baixe o ZIP da branch `scaffold/observatorio-dos-reinos`.
2. Atualize os arquivos do mod com a nova versão do ramo. Não substitua a pasta do Worldfall.
3. Abra o WorldBox e consulte o log do NeoModLoader.
4. Procure `Autoteste politico: reinos=2, relacoes=1, tipo=Comercio, opiniao=10.`
5. Confirme que o jogo abre e que não surgem exceções repetidas.

Aprovação: log com os dois reinos de teste e a relação de comércio, sem crash.

## Teste 2 — validação de leitura real (futura)
- A implementação só deve começar depois de confirmar os nomes e as assinaturas dos tipos na assembly exata do WorldBox 0.51.4.
- Usar um mundo descartável que contenha ao menos dois reinos.
- Comparar nomes/quantidades do painel com os dados nativos do jogo.
- Confirmar que abrir/fechar o painel não altera a simulação.

## Teste 3 — mecânicas integradas (futuro)
- Só testar ações que mudem o mundo depois de adicionar backup/salvaguardas.
- Confirmar separadamente efeitos de diplomacia, persistência e carregamento de saves.
- Se houver crash ou erro repetido, desative o mod e restaure a cópia.

## Diagnóstico de falha
- Envie a mensagem completa do log e informe se aparece antes ou depois da linha do autoteste.
- Não tentar adivinhar nomes de APIs ou ocultar exceções; corrigir usando o erro real.
