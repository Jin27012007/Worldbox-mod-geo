# Observatório dos Reinos (WorldBox 0.51.4)

Projeto de mod 2D para Android usando NeoModLoader, com foco em geopolítica, diplomacia e interações de RPG, preservando a simulação normal do WorldBox.

## Estado atual

Este repositório está no estágio de estruturação. A compatibilidade exata entre WorldBox 0.51.4, NeoModLoader e o ambiente de compilação ainda precisa ser validada em execução no aparelho. Nenhuma build instalável foi validada até o momento.

## Princípios

- Começar por leitura de dados; não alterar o mundo na primeira versão.
- UI 2D simples e leve, sem recursos 3D.
- Não copiar código, modelos, imagens ou outros assets proprietários de Worldfall.
- Isolar chamadas específicas do jogo para reduzir conflitos e facilitar a depuração.
- Só declarar compatibilidade depois de compilar e testar no jogo.

## Primeiro marco: Observatório

1. Confirmar que o NeoModLoader carrega o mod.
2. Registrar versão/configuração e estado de inicialização no log.
3. Exibir um painel simples de diagnóstico.
4. Quando as APIs forem confirmadas, listar reinos e governantes sem modificar a simulação.

## Estrutura

- `docs/STATUS.md`: fatos confirmados, hipóteses e pendências.
- `docs/TEST_PROTOCOL.md`: protocolo de testes no Android.
- `src/ObservatorioMod.cs`: ponto de entrada inicial, sujeito à validação da API instalada.
- `mod.json`: manifesto provisório; validar formato e campos exigidos pela versão instalada do NeoModLoader.

## Compilação e instalação

Ainda não há instruções de compilação comprovadas para esta combinação específica de versões. Não instale uma DLL gerada por este scaffold até que o projeto compile contra as dependências reais do aparelho e passe pelos testes descritos em `docs/TEST_PROTOCOL.md`.
