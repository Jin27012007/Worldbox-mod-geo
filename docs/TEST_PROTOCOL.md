# Protocolo de testes — Android

## Preparação
- Fazer backup do diretório de mods e dos mundos importantes.
- Registrar versões exatas do WorldBox, NeoModLoader e runtime de compilação.
- Testar separadamente dos mods funcionais existentes.

## Teste 1 — carregamento
1. Instalar uma build compilada contra as referências do aparelho.
2. Abrir o jogo e iniciar um mundo de teste descartável.
3. Confirmar no log uma única inicialização do mod.
4. Verificar se o jogo inicia sem novas exceções.

Aprovação: inicialização única, sem crash, exceção não tratada ou loop de logs.

## Teste 2 — painel de diagnóstico
1. Abrir/fechar o painel repetidamente.
2. Confirmar que não altera pausa, velocidade, unidades, cidades ou relações.
3. Testar orientação de tela quando suportada.

Aprovação: UI abre e fecha sem travar a simulação nem gerar exceções.

## Teste 3 — leitura de reinos
1. Usar mundo descartável com pelo menos dois reinos.
2. Comparar os nomes do painel com o jogo.
3. Deixar a simulação rodar e abrir/fechar o painel várias vezes.
4. Se a API suportar eventos de criação/destruição de reinos, verificar esses cenários.

Aprovação: dados somente de leitura corretos; nenhum estado alterado.

## Recuperação
- Em caso de crash ou erro repetitivo, desativar o mod e restaurar backup.
- Guardar log, versões, etapa do teste e sequência de ações.
- Não declarar compatibilidade apenas porque compila; executar no aparelho.
