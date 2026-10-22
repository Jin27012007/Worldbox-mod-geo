# Protocolo de testes — Android

## Teste atual: sonda ampliada de APIs
1. Faça cópia da pasta do mod atual e mantenha um mundo descartável.
2. Baixe o ZIP da branch `scaffold/observatorio-dos-reinos` e atualize a pasta do Observatório dos Reinos, sem tocar na pasta do Worldfall.
3. Abra o WorldBox e carregue um mundo.
4. Consulte o log do NeoModLoader e copie as linhas completas `Diagnostico do mundo:` e `Diagnostico de API (somente leitura):`.
5. Envie também qualquer erro de compilação/carregamento que surgir.

Aprovação: o mod é carregado normalmente e o diagnóstico registra o estado da instância do mapa, contagens disponíveis e membros candidatos da API.

## Notas
- O diagnóstico é executado em `OnModLoad`. Caso esse ponto ocorra antes de carregar o mundo, as contagens podem estar indisponíveis; os resultados são pistas e não prova de falha do mundo.
- Campos/membros nomeados como `controlled`, `player`, `king`, etc. são apenas candidatos por nome. Nenhum deles deve ser usado como referência definitiva sem confirmar tipo, valor e comportamento na versão 0.51.4.
- A sonda não altera dados de jogo, guerras, unidades, reinos ou cidades.

## Próximas fases de teste
- Confirmar o personagem sob controle com comportamento observável no jogo.
- Confirmar o reino/cidade vinculados ao personagem.
- Testar o painel somente leitura na orientação horizontal e vertical.
- Testar alterações administrativas isoladamente num mundo descartável antes de introduzir ações diplomáticas.

## Recuperação
Em caso de crash, desative o mod e restaure a cópia. Guarde o log desde o início do carregamento.
