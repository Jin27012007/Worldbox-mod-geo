using System;
using System.Collections.Generic;
using NeoModLoader.api;

namespace ObservatorioDosReinos
{
    /// <summary>
    /// Primeiro sistema de regras do Observatório. Mantém estado político próprio
    /// do mod, separado da simulação original do WorldBox.
    /// </summary>
    public sealed class PoliticaDoReino
    {
        public string Id { get; private set; }
        public string Nome { get; private set; }
        public int Estabilidade { get; private set; }
        public int Legitimidade { get; private set; }
        public int Tesouro { get; private set; }

        public PoliticaDoReino(string id, string nome)
        {
            Id = string.IsNullOrEmpty(id) ? Guid.NewGuid().ToString("N") : id;
            Nome = string.IsNullOrEmpty(nome) ? "Reino sem nome" : nome;
            Estabilidade = 60;
            Legitimidade = 50;
            Tesouro = 100;
        }

        public void AjustarEstabilidade(int variacao)
        {
            Estabilidade = Limitar(Estabilidade + variacao, 0, 100);
        }

        public void AjustarLegitimidade(int variacao)
        {
            Legitimidade = Limitar(Legitimidade + variacao, 0, 100);
        }

        public void AjustarTesouro(int variacao)
        {
            long novoValor = (long)Tesouro + variacao;
            Tesouro = (int)Math.Max(0, Math.Min(1000000L, novoValor));
        }

        private static int Limitar(int valor, int minimo, int maximo)
        {
            return Math.Max(minimo, Math.Min(maximo, valor));
        }
    }

    public enum TipoRelacaoDiplomatica
    {
        Neutra = 0,
        Comercio = 1,
        Alianca = 2,
        Rivalidade = 3,
        Guerra = 4
    }

    /// <summary>
    /// Relacao politica do mod entre dois IDs estaveis. Nao altera relacoes vanilla.
    /// </summary>
    public sealed class RelacaoDiplomatica
    {
        public string ReinoA { get; private set; }
        public string ReinoB { get; private set; }
        public TipoRelacaoDiplomatica Tipo { get; private set; }
        public int Opiniao { get; private set; }

        public RelacaoDiplomatica(string reinoA, string reinoB)
        {
            if (string.IsNullOrEmpty(reinoA)) throw new ArgumentException("ReinoA precisa de um ID.");
            if (string.IsNullOrEmpty(reinoB)) throw new ArgumentException("ReinoB precisa de um ID.");
            if (reinoA == reinoB) throw new ArgumentException("Um reino nao pode ter relacao consigo mesmo.");

            ReinoA = reinoA;
            ReinoB = string.CompareOrdinal(reinoA, reinoB) < 0 ? reinoB : reinoA;
            if (string.CompareOrdinal(reinoA, reinoB) > 0)
            {
                ReinoA = reinoB;
                ReinoB = reinoA == reinoB ? reinoA : reinoA;
            }

            Tipo = TipoRelacaoDiplomatica.Neutra;
            Opiniao = 0;
        }

        public void DefinirTipo(TipoRelacaoDiplomatica tipo)
        {
            if (!Enum.IsDefined(typeof(TipoRelacaoDiplomatica), tipo))
                throw new ArgumentOutOfRangeException("tipo");
            Tipo = tipo;
        }

        public void AjustarOpiniao(int variacao)
        {
            Opiniao = Math.Max(-100, Math.Min(100, Opiniao + variacao));
        }

        public string Chave
        {
            get { return ReinoA + "::" + ReinoB; }
        }
    }

    /// <summary>
    /// Repositorio central das regras politicas extras. Nao toca objetos do jogo.
    /// </summary>
    public sealed class EstadoPolitico
    {
        private readonly Dictionary<string, PoliticaDoReino> reinos =
            new Dictionary<string, PoliticaDoReino>();
        private readonly Dictionary<string, RelacaoDiplomatica> relacoes =
            new Dictionary<string, RelacaoDiplomatica>();

        public int QuantidadeDeReinos { get { return reinos.Count; } }
        public int QuantidadeDeRelacoes { get { return relacoes.Count; } }

        public PoliticaDoReino RegistrarReino(string id, string nome)
        {
            if (string.IsNullOrEmpty(id)) throw new ArgumentException("O ID do reino e obrigatorio.");
            PoliticaDoReino existente;
            if (reinos.TryGetValue(id, out existente)) return existente;

            PoliticaDoReino novo = new PoliticaDoReino(id, nome);
            reinos.Add(id, novo);
            return novo;
        }

        public bool TentarObterReino(string id, out PoliticaDoReino reino)
        {
            return reinos.TryGetValue(id, out reino);
        }

        public RelacaoDiplomatica ObterOuCriarRelacao(string reinoA, string reinoB)
        {
            if (string.IsNullOrEmpty(reinoA) || string.IsNullOrEmpty(reinoB))
                throw new ArgumentException("Os IDs dos dois reinos sao obrigatorios.");
            if (reinoA == reinoB)
                throw new ArgumentException("Nao existe relacao diplomatica entre um reino e ele mesmo.");

            string primeiro = string.CompareOrdinal(reinoA, reinoB) < 0 ? reinoA : reinoB;
            string segundo = string.CompareOrdinal(reinoA, reinoB) < 0 ? reinoB : reinoA;
            string chave = primeiro + "::" + segundo;

            RelacaoDiplomatica relacao;
            if (relacoes.TryGetValue(chave, out relacao)) return relacao;

            relacao = new RelacaoDiplomatica(primeiro, segundo);
            relacoes.Add(chave, relacao);
            return relacao;
        }
    }

    public class Main : BasicMod<Main>
    {
        private static EstadoPolitico estadoPolitico;

        protected override void OnModLoad()
        {
            estadoPolitico = new EstadoPolitico();

            PoliticaDoReino testeA = estadoPolitico.RegistrarReino("diagnostico-a", "Reino de teste A");
            PoliticaDoReino testeB = estadoPolitico.RegistrarReino("diagnostico-b", "Reino de teste B");
            RelacaoDiplomatica relacao = estadoPolitico.ObterOuCriarRelacao(testeA.Id, testeB.Id);
            relacao.DefinirTipo(TipoRelacaoDiplomatica.Comercio);
            relacao.AjustarOpiniao(10);

            LogInfo("Observatorio dos Reinos carregado. Sistemas politicos locais inicializados.");
            LogInfo("Autoteste politico: reinos=" + estadoPolitico.QuantidadeDeReinos
                + ", relacoes=" + estadoPolitico.QuantidadeDeRelacoes
                + ", tipo=" + relacao.Tipo + ", opiniao=" + relacao.Opiniao + ".");
            LogInfo("Modo seguro: nenhuma cidade, unidade ou relacao original do WorldBox foi alterada.");
        }
    }
}
