using System;

namespace ObservatorioDosReinos.Tests
{
    internal static class StubCompileSmokeTest
    {
        private static void Exigir(bool condicao, string mensagem)
        {
            if (!condicao) throw new InvalidOperationException("Teste falhou: " + mensagem);
        }

        public static int Main()
        {
            // Fora da abertura do status, o método original deve continuar executando.
            Exigir(
                MenuAdministracaoOverlay.PrefixSelectedInfoWhenOpeningStatus("teste", true),
                "a rotina auxiliar deve seguir fora de StatsWindow.OnEnable");

            // Durante OnEnable, o prefix preventivo deve ignorar somente a atualização auxiliar.
            bool estadoAnterior;
            MenuAdministracaoOverlay.StatsWindowPrefix(out estadoAnterior);
            Exigir(!estadoAnterior, "o primeiro escopo deve salvar o estado anterior false");
            Exigir(
                !MenuAdministracaoOverlay.PrefixSelectedInfoWhenOpeningStatus("teste", false),
                "a rotina auxiliar deve ser ignorada dentro do escopo de abertura do status");

            // Em execução bem-sucedida, o postfix restaura o valor anterior.
            MenuAdministracaoOverlay.StatsWindowPostfix(estadoAnterior);
            Exigir(
                MenuAdministracaoOverlay.PrefixSelectedInfoWhenOpeningStatus("teste", true),
                "o postfix deve restaurar o estado fora do escopo");

            // Se OnEnable lançar uma exceção, o finalizer também deve restaurar o estado,
            // mesmo quando não precisa interceptar a exceção.
            bool estadoFinalizer;
            MenuAdministracaoOverlay.StatsWindowPrefix(out estadoFinalizer);
            Exigir(!estadoFinalizer, "o segundo escopo deve salvar false");
            Exception retorno = MenuAdministracaoOverlay.StatsWindowFinalizer(null, null, estadoFinalizer);
            Exigir(retorno == null, "finalizer sem exceção deve retornar null");
            Exigir(
                MenuAdministracaoOverlay.PrefixSelectedInfoWhenOpeningStatus("teste", true),
                "o finalizer deve restaurar o estado mesmo sem exceção");

            Console.WriteLine("PASS: prefix ativo somente durante o escopo de status; postfix/finalizer restauram o estado.");
            return 0;
        }
    }
}
