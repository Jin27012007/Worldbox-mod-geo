# Protocolo de testes — Android

## Teste atual: diagnóstico seguro da API
1. Faça backup da pasta do mod e mantenha um mundo descartável para os testes.
2. Baixe o ZIP da branch `scaffold/observatorio-dos-reinos` e atualize os arquivos do mod, sem substituir a pasta do Worldfall.
3. Abra um mundo já gerado no WorldBox 0.51.4 e abra o log do NeoModLoader.
4. Procure `Diagnostico do mundo:` e `Diagnostico de API (somente leitura):`.
5. Envie as linhas completas. Elas indicam se `MapBox.instance` está disponível, quantos reinos/cidades/unidades foram enumerados e quais membros candidatos existem.
6. Se o código não compilar/carregar, envie a exceção ou erro completo; não remova arquivos às cegas.

Aprovação: o mod segue carregando, o log apresenta um resultado diagnóstico e o jogo permanece estável. Contagens `-1` indicam que um membro não foi localizado ou não pôde ser contado, não que o mundo não tenha entidades.

## Importante
- Este probe é somente leitura: não invoca métodos de alteração do mundo.
- O diagnóstico acontece em `OnModLoad`; se o NML chamar esse método antes de carregar um mundo, `MapBox.instance` pode estar nulo. Nesse caso o resultado é inconclusivo para contagens, e precisamos mudar a fase da inspeção antes de adicionar mais código.
- Não declare personagem controlado/rei detectado sem evidência explícita no log.
- Teste UI e ações que mudam o mundo só depois de identificar e validar as APIs da versão exata.

## Recuperação
Em caso de crash, desative o mod e restaure a cópia. Guarde o log desde o início do carregamento.
