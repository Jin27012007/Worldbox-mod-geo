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
            EstadoPolitico estado = new EstadoPolitico();
            PoliticaDoReino reinoA = estado.RegistrarReino("reino-a", "Reino A");
            PoliticaDoReino reinoB = estado.RegistrarReino("reino-b", "Reino B");
            Exigir(estado.QuantidadeDeReinos == 2, "dois reinos devem ser registrados");

            RelacaoDiplomatica relacao = estado.ObterOuCriarRelacao(reinoA.Id, reinoB.Id);
            RelacaoDiplomatica relacaoInvertida = estado.ObterOuCriarRelacao(reinoB.Id, reinoA.Id);
            Exigir(object.ReferenceEquals(relacao, relacaoInvertida), "a relação deve ser única independentemente da ordem dos reinos");

            relacao.DefinirTipo(TipoRelacaoDiplomatica.Alianca);
            relacao.AjustarOpiniao(150);
            Exigir(relacao.Tipo == TipoRelacaoDiplomatica.Alianca, "tipo de relação deve ser persistido no modelo");
            Exigir(relacao.Opiniao == 100, "opinião deve ser limitada a 100");
            relacao.AjustarOpiniao(-250);
            Exigir(relacao.Opiniao == -100, "opinião deve ser limitada a -100");

            reinoA.AjustarEstabilidade(-500);
            reinoA.AjustarLegitimidade(500);
            reinoA.AjustarTesouro(-200);
            Exigir(reinoA.Estabilidade == 0, "estabilidade deve ter limite inferior 0");
            Exigir(reinoA.Legitimidade == 100, "legitimidade deve ter limite superior 100");
            Exigir(reinoA.Tesouro == 0, "tesouro não pode ser negativo");

            Console.WriteLine("PASS: invariantes do modelo político local; isso não valida a interface em runtime.");
            return 0;
        }
    }
}
