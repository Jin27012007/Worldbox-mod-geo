using System;
using System.Collections;
using System.Collections.Generic;
using System.Reflection;
using NeoModLoader.api;

namespace ObservatorioDosReinos
{
    /// <summary>
    /// Leitura tolerante a mudanças de versão. Só procura metadados e valores;
    /// nunca chama métodos de alteração do jogo.
    /// </summary>
    internal static class LeitorSeguroDoMundo
    {
        private const BindingFlags Membros = BindingFlags.Public | BindingFlags.NonPublic
            | BindingFlags.Instance | BindingFlags.Static;

        internal sealed class Resumo
        {
            public bool MapaEncontrado;
            public int Reinos;
            public int Cidades;
            public int Unidades;
            public string Diagnostico;
        }

        public static Resumo Inspecionar()
        {
            Resumo resultado = new Resumo();
            List<string> notas = new List<string>();

            try
            {
                Type mapaTipo = Type.GetType("MapBox, Assembly-CSharp");
                if (mapaTipo == null)
                {
                    notas.Add("MapBox nao encontrado na Assembly-CSharp.");
                    resultado.Diagnostico = string.Join(" | ", notas.ToArray());
                    return resultado;
                }

                FieldInfo instanciaCampo = mapaTipo.GetField("instance", Membros);
                object mapa = instanciaCampo == null ? null : instanciaCampo.GetValue(null);
                resultado.MapaEncontrado = mapa != null;

                if (mapa == null)
                {
                    notas.Add("MapBox.instance indisponivel; mundo pode nao estar carregado.");
                    resultado.Diagnostico = string.Join(" | ", notas.ToArray());
                    return resultado;
                }

                resultado.Reinos = ContarGerenciador(mapa, "kingdoms", notas);
                resultado.Cidades = ContarGerenciador(mapa, "cities", notas);
                resultado.Unidades = ContarGerenciador(mapa, "units", notas);

                Type[] candidatos =
                {
                    Type.GetType("PlayerControl, Assembly-CSharp"),
                    Type.GetType("Actor, Assembly-CSharp"),
                    Type.GetType("Kingdom, Assembly-CSharp"),
                    Type.GetType("City, Assembly-CSharp"),
                    Type.GetType("MapBox, Assembly-CSharp")
                };

                for (int i = 0; i < candidatos.Length; i++)
                {
                    if (candidatos[i] == null) continue;
                    Type t = candidatos[i];
                    notas.Add("tipo " + t.FullName + ": " + ResumirMembros(t,
                        new string[] { "selected", "controlled", "player", "king", "kingdom",
                            "city", "name", "actor", "current", "unit", "focus", "control" }));
                }

                resultado.Diagnostico = string.Join(" | ", notas.ToArray());
                return resultado;
            }
            catch (Exception ex)
            {
                resultado.Diagnostico = "Inspecao interrompida com seguranca: "
                    + ex.GetType().Name + ": " + ex.Message;
                return resultado;
            }
        }

        private static int ContarGerenciador(object mapa, string nome, List<string> notas)
        {
            try
            {
                FieldInfo campo = AcharCampo(mapa.GetType(), nome);
                object gerenciador = campo == null ? null : campo.GetValue(mapa);
                if (gerenciador == null)
                {
                    notas.Add("gerenciador " + nome + " indisponivel.");
                    return -1;
                }

                PropertyInfo contagem = AcharPropriedade(gerenciador.GetType(), "Count");
                if (contagem != null && contagem.PropertyType == typeof(int))
                {
                    object valor = contagem.GetValue(gerenciador, null);
                    if (valor is int) return (int)valor;
                }

                IEnumerable enumeravel = gerenciador as IEnumerable;
                if (enumeravel == null)
                {
                    notas.Add("gerenciador " + nome + " nao implementa IEnumerable.");
                    return -1;
                }

                int total = 0;
                foreach (object item in enumeravel)
                    if (item != null) total++;
                return total;
            }
            catch (Exception ex)
            {
                notas.Add("contagem de " + nome + " falhou: " + ex.GetType().Name);
                return -1;
            }
        }

        private static string ResumirMembros(Type tipo, string[] pistas)
        {
            List<string> encontrados = new List<string>();
            for (Type atual = tipo; atual != null; atual = atual.BaseType)
            {
                FieldInfo[] campos = atual.GetFields(Membros | BindingFlags.DeclaredOnly);
                for (int i = 0; i < campos.Length; i++)
                    if (Combina(campos[i].Name, pistas)) encontrados.Add("F:" + campos[i].Name);

                PropertyInfo[] propriedades = atual.GetProperties(Membros | BindingFlags.DeclaredOnly);
                for (int i = 0; i < propriedades.Length; i++)
                    if (Combina(propriedades[i].Name, pistas)) encontrados.Add("P:" + propriedades[i].Name);

                MethodInfo[] metodos = atual.GetMethods(Membros | BindingFlags.DeclaredOnly);
                for (int i = 0; i < metodos.Length; i++)
                    if (Combina(metodos[i].Name, pistas)) encontrados.Add("M:" + metodos[i].Name);
            }

            if (encontrados.Count == 0) return "(nenhum membro compatível encontrado)";
            if (encontrados.Count > 24) encontrados.RemoveRange(24, encontrados.Count - 24);
            return string.Join(",", encontrados.ToArray());
        }

        private static bool Combina(string nome, string[] pistas)
        {
            for (int i = 0; i < pistas.Length; i++)
                if (nome.IndexOf(pistas[i], StringComparison.OrdinalIgnoreCase) >= 0)
                    return true;
            return false;
        }

        private static FieldInfo AcharCampo(Type tipo, string nome)
        {
            for (Type atual = tipo; atual != null; atual = atual.BaseType)
            {
                FieldInfo campo = atual.GetField(nome, Membros | BindingFlags.DeclaredOnly);
                if (campo != null) return campo;
            }
            return null;
        }

        private static PropertyInfo AcharPropriedade(Type tipo, string nome)
        {
            for (Type atual = tipo; atual != null; atual = atual.BaseType)
            {
                PropertyInfo prop = atual.GetProperty(nome, Membros | BindingFlags.DeclaredOnly);
                if (prop != null) return prop;
            }
            return null;
        }
    }

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

        public void AjustarEstabilidade(int variacao) { Estabilidade = Limitar((long)Estabilidade + variacao, 0, 100); }
        public void AjustarLegitimidade(int variacao) { Legitimidade = Limitar((long)Legitimidade + variacao, 0, 100); }
        public void AjustarTesouro(int variacao) { Tesouro = Limitar((long)Tesouro + variacao, 0, 1000000); }

        private static int Limitar(long valor, int minimo, int maximo)
        {
            return (int)Math.Max(minimo, Math.Min(maximo, valor));
        }
    }

    public enum TipoRelacaoDiplomatica { Neutra = 0, Comercio = 1, Alianca = 2, Rivalidade = 3, Guerra = 4 }

    public sealed class RelacaoDiplomatica
    {
        public string ReinoA { get; private set; }
        public string ReinoB { get; private set; }
        public TipoRelacaoDiplomatica Tipo { get; private set; }
        public int Opiniao { get; private set; }

        public RelacaoDiplomatica(string reinoA, string reinoB)
        {
            if (string.IsNullOrEmpty(reinoA) || string.IsNullOrEmpty(reinoB))
                throw new ArgumentException("Os IDs dos dois reinos sao obrigatorios.");
            if (reinoA == reinoB) throw new ArgumentException("Um reino nao pode ter relacao consigo mesmo.");
            if (string.CompareOrdinal(reinoA, reinoB) < 0) { ReinoA = reinoA; ReinoB = reinoB; }
            else { ReinoA = reinoB; ReinoB = reinoA; }
            Tipo = TipoRelacaoDiplomatica.Neutra;
            Opiniao = 0;
        }

        public void DefinirTipo(TipoRelacaoDiplomatica tipo)
        {
            if (!Enum.IsDefined(typeof(TipoRelacaoDiplomatica), tipo)) throw new ArgumentOutOfRangeException("tipo");
            Tipo = tipo;
        }
        public void AjustarOpiniao(int variacao) { Opiniao = (int)Math.Max(-100L, Math.Min(100L, (long)Opiniao + variacao)); }
        public string Chave { get { return ReinoA + "::" + ReinoB; } }
    }

    public sealed class EstadoPolitico
    {
        private readonly Dictionary<string, PoliticaDoReino> reinos = new Dictionary<string, PoliticaDoReino>();
        private readonly Dictionary<string, RelacaoDiplomatica> relacoes = new Dictionary<string, RelacaoDiplomatica>();

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

        public bool TentarObterReino(string id, out PoliticaDoReino reino) { return reinos.TryGetValue(id, out reino); }

        public RelacaoDiplomatica ObterOuCriarRelacao(string reinoA, string reinoB)
        {
            if (string.IsNullOrEmpty(reinoA) || string.IsNullOrEmpty(reinoB))
                throw new ArgumentException("Os IDs dos dois reinos sao obrigatorios.");
            if (reinoA == reinoB) throw new ArgumentException("Nao existe relacao diplomatica entre um reino e ele mesmo.");
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

            LeitorSeguroDoMundo.Resumo mundo = LeitorSeguroDoMundo.Inspecionar();
            LogInfo("Diagnostico do mundo: mapa=" + mundo.MapaEncontrado
                + ", reinos=" + mundo.Reinos + ", cidades=" + mundo.Cidades
                + ", unidades=" + mundo.Unidades + ".");
            LogInfo("Diagnostico de API (somente leitura): " + mundo.Diagnostico);
            LogInfo("Modo seguro: nenhuma cidade, unidade ou relacao original do WorldBox foi alterada.");
        }
    }
}
