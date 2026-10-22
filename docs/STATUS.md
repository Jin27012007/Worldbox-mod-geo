# Status técnico — 0.5.3-dev

## Evidência de runtime
- WorldBox Android 0.51.4; NeoModLoader ativo.
- O usuário relata que a exceção persiste ao abrir o status do reino: `NullReferenceException` em `PowerTabController.showWhoIsSelected(String, Boolean)` durante `StatsWindow.OnEnable()`, via `SelectedPowerTabButtons.clickOpenMain()`.
- A janela fica cortada mesmo depois de fechar; em landscape, a UI some. O usuário não viu o painel do mod nem o fallback nas tentativas anteriores.
- A assembly publicizada enviada tem nomes divergentes e não permite provar qual referência runtime é nula. A recuperação Harmony anterior ainda não foi vista funcionando.

## 0.5.3-dev — engenharia de diagnóstico e patch Harmony
- Canvas Unity UI limitado à `Screen.safeArea`, abas e campos de teste.
- Banner visível de diagnóstico expandido: versão, resolução, área segura, Canvas ativo, botão nativo encontrado e botão reserva visível.
- Log de erro único se Canvas/texto diagnóstico estiverem inativos.
- Finalizer Harmony é estreito: só deve interceptar `NullReferenceException` quando stack contém `showWhoIsSelected` ou `showWorldTipSelected`; o objetivo é fechar a janela de status incompleta e abrir o painel alternativo.
- Refeito o registro de instalação do Harmony: enumera overloads de `StatsWindow.OnEnable`, procura o construtor do `HarmonyMethod` de modo tolerante, lista assinaturas de `Harmony.Patch` quando não encontra uma compatível e imprime `STATUS_PATCH_INSTALLED` ou `STATUS_PATCH_NOT_INSTALLED`. Isso elimina a ambiguidade sobre o patch ter sido aplicado, mas ainda requer teste no aparelho.

## Compilação independente
- Foi executado um job de compilação Mono no GitHub Actions. A primeira execução falhou com 39 erros de tipos não resolvidos, incluindo `UnityEngine`, `NeoModLoader` e `BasicMod`, porque não foram encontradas as assemblies de referência. Esse resultado não valida a compilação do mod; ele prova que faltam dependências de build.
- O workflow foi ajustado para localizar DLLs antes de compilar, mas o repositório não deve carregar as DLLs privadas do jogo/loader. Uma compilação real exige que as assemblies da versão 0.51.4 sejam disponibilizadas ao processo de build de forma privada/local.

## Diagnóstico obrigatório no próximo teste
O fato de a stack trace persistir **não demonstra sozinho** se a revisão do mod não carregou, se Harmony não foi instalado, ou se o método alvo não coincide. Verificar no log:
- `[Observatorio] Canvas, botao reserva e painel admin prontos...`
- `[Observatorio] Protecao de fallback instalada...` (ou a razão pela qual não foi instalada)
- `[Observatorio] FINALIZER ACIONADO...` depois de abrir status
- `UI administrativa via Canvas criada...`

Se as mensagens de criação estiverem ausentes, o mod não está executando essa revisão: parar de modificar layout e revisar compilação/carga/estrutura da pasta. Se Canvas está criado mas o banner não aparece, investigar ordem de sorting, Canvas lifecycle e safe area. Se o erro acontece sem log de finalizer, inspecionar o método e assinatura reais da assembly em execução antes de tentar patching adicional.

## Estado real do painel
Ainda não há confirmação visual de que a UI Canvas aparece; não chamar isso de funcional. Os botões são rascunhos e não alteram a simulação. Detecção do rei/reino real permanece pendente.

## Segurança
Não suprimir outras exceções. Não alterar mundos/saves. Não incluir DLLs do jogo/loader ou assets/código de Worldfall.