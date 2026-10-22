using System;
using System.Collections;
using System.Collections.Generic;
using System.Reflection;
using NeoModLoader.api;
using UnityEngine;

namespace ObservatorioDosReinos
{
    /// <summary>
    /// Sonda somente leitura para descobrir APIs sem assumir que todas as versões
    /// do WorldBox expõem os mesmos membros.
    /// </summary>
    internal static class LeitorSeguroDoMundo
    {
        private const BindingFlags Membros = BindingFlags.Public | BindingFlags.NonPublic
            | BindingFlags.Instance | BindingFlags.Static;

        internal sealed class Resumo
        {
            public bool MapaEncontrado;
            public int Reinos = -1;
            public int Cidades = -1;
            public int Unidades = -1;
            public string Diagnostico = "";
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
                    notas.Add("MapBox nao encontrado em Assembly-CSharp.");
                    resultado.Diagnostico = string.Join(" | ", notas.ToArray());
                    return resultado;
                }

                FieldInfo instanciaCampo = mapaTipo.GetField("instance", Membros);
                object mapa = instanciaCampo == null ? null : instanciaCampo.GetValue(null);
                resultado.MapaEncontrado = mapa != null;
                if (mapa == null)
                {
                    notas.Add("MapBox.instance indisponivel; mundo nao inicializado ou campo ausente.");
                    resultado.Diagnostico = string.Join(" | ", notas.ToArray());
                    return resultado;
                }

                resultado.Reinos = ContarGerenciador(mapa, "kingdoms", notas);
                resultado.Cidades = ContarGerenciador(mapa, "cities", notas);
                resultado.Unidades = ContarGerenciador(mapa, "units", notas);

                string[] nomesTipos = { "PlayerControl", "Actor", "Kingdom", "City", "MapBox" };
                string[] pistas = { "selected", "controlled", "player", "king", "kingdom",
                    "city", "name", "actor", "current", "unit", "focus", "control",
                    "instance", "target", "follow", "hero" };
                for (int i = 0; i < nomesTipos.Length; i++)
                {
                    Type t = Type.GetType(nomesTipos[i] + ", Assembly-CSharp");
                    if (t == null)
                    {
                        notas.Add("tipo " + nomesTipos[i] + " ausente.");
                        continue;
                    }
                    notas.Add("tipo " + t.FullName + ": " + ResumirMembros(t, pistas));
                }

                resultado.Diagnostico = string.Join(" | ", notas.ToArray());
                return resultado;
            }
            catch (Exception ex)
            {
                resultado.Diagnostico = "Inspecao segura falhou: " + ex.GetType().Name + ": " + ex.Message;
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
                    notas.Add("gerenciador " + nome + " sem IEnumerable.");
                    return -1;
                }
                int total = 0;
                foreach (object item in enumeravel) if (item != null) total++;
                return total;
            }
            catch (Exception ex)
            {
                notas.Add("contagem " + nome + " falhou: " + ex.GetType().Name);
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
            if (encontrados.Count == 0) return "(sem correspondencias pelas pistas)";
            if (encontrados.Count > 35) encontrados.RemoveRange(35, encontrados.Count - 35);
            return string.Join(",", encontrados.ToArray());
        }

        private static bool Combina(string nome, string[] pistas)
        {
            for (int i = 0; i < pistas.Length; i++)
                if (nome.IndexOf(pistas[i], StringComparison.OrdinalIgnoreCase) >= 0) return true;
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

    /// <summary>Protótipo visual responsivo. Os botões de política ainda são demonstrativos.</summary>
    public sealed class MenuAdministracaoOverlay : MonoBehaviour
    {
        private const int IdJanela = 490271;
        private static MenuAdministracaoOverlay instancia;
        private bool aberto;
        private int aba;
        private Rect janela;
        private Vector2 rolagem;
        private string nomePolitica = "Protecao das aldeias";
        private string textoReuniao = "Convocar conselho real para discutir as necessidades do reino.";
        private string textoDiplomacia = "Propor um tratado de paz e comercio.";
        private string textoEvento = "Um emissario chega ao castelo com noticias da fronteira.";
        private string retorno = "Protótipo visual: ações ainda não alteram o mundo.";
        private bool leiProtecao = true;
        private bool leiComercio;
        private bool leiRecrutamento;
        private GUIStyle titulo;
        private GUIStyle secao;
        private GUIStyle texto;
        private GUIStyle botao;
        private GUIStyle campo;
        private GUIStyle pequeno;

        public static void CriarSeNecessario()
        {
            if (instancia != null) return;
            MenuAdministracaoOverlay existente = FindObjectOfType<MenuAdministracaoOverlay>();
            if (existente != null) { instancia = existente; return; }
            GameObject objeto = new GameObject("ObservatorioDosReinos_MenuAdmin");
            DontDestroyOnLoad(objeto);
            instancia = objeto.AddComponent<MenuAdministracaoOverlay>();
        }

        private void OnGUI()
        {
            try
            {
                PrepararEstilos();
                float margem = Mathf.Clamp(Screen.width * 0.02f, 8f, 18f);
                float larguraAtalho = Mathf.Clamp(Screen.width * 0.22f, 112f, 184f);
                float alturaAtalho = Mathf.Clamp(Screen.height * 0.045f, 34f, 48f);
                GUI.color = new Color(1f, 1f, 1f, 0.98f);
                if (GUI.Button(new Rect(margem, margem, larguraAtalho, alturaAtalho),
                    aberto ? "Fechar governo" : "Governo", botao))
                    aberto = !aberto;

                if (aberto)
                {
                    float largura = Mathf.Clamp(Screen.width - margem * 2f, 270f, 620f);
                    largura = Mathf.Min(largura, Screen.width - margem * 2f);
                    float altura = Mathf.Clamp(Screen.height * 0.72f, 300f, 660f);
                    altura = Mathf.Min(altura, Screen.height - margem * 2f);
                    float x = (Screen.width - largura) * 0.5f;
                    float y = Mathf.Max(margem, (Screen.height - altura) * 0.5f);
                    janela = new Rect(x, y, largura, altura);
                    janela = GUI.Window(IdJanela, janela, DesenharJanela, "OBSERVATÓRIO DOS REINOS");
                }
                GUI.color = Color.white;
            }
            catch (Exception ex)
            {
                GUI.color = Color.white;
                Debug.LogWarning("[ObservatorioDosReinos] UI segura interrompida: " + ex.GetType().Name);
            }
        }

        private void PrepararEstilos()
        {
            if (titulo != null) return;
            int fonte = Mathf.Clamp(Screen.width / 38, 12, 20);
            titulo = new GUIStyle(GUI.skin.label);
            titulo.fontSize = fonte + 2;
            titulo.fontStyle = FontStyle.Bold;
            titulo.normal.textColor = new Color(1f, 0.68f, 0.24f);
            titulo.wordWrap = true;

            secao = new GUIStyle(GUI.skin.label);
            secao.fontSize = fonte;
            secao.fontStyle = FontStyle.Bold;
            secao.normal.textColor = new Color(1f, 0.72f, 0.34f);
            secao.wordWrap = true;

            texto = new GUIStyle(GUI.skin.label);
            texto.fontSize = fonte;
            texto.normal.textColor = new Color(0.93f, 0.92f, 0.82f);
            texto.wordWrap = true;

            botao = new GUIStyle(GUI.skin.button);
            botao.fontSize = fonte;
            botao.wordWrap = true;
            botao.fixedHeight = Mathf.Clamp(fonte * 2.8f, 34f, 54f);
            botao.normal.textColor = Color.white;

            campo = new GUIStyle(GUI.skin.textField);
            campo.fontSize = fonte;
            campo.fixedHeight = Mathf.Clamp(fonte * 2.5f, 30f, 46f);

            pequeno = new GUIStyle(texto);
            pequeno.fontSize = Mathf.Max(10, fonte - 2);
        }

        private void DesenharJanela(int id)
        {
            GUILayout.BeginVertical();
            GUILayout.Label("Painel de administração • protótipo", titulo);
            GUILayout.Label("Personagem/reino: aguardando ligação à API real do WorldBox.", pequeno);
            GUILayout.Space(4);

            GUILayout.BeginHorizontal();
            string[] abas = { "Governo", "Diplomacia", "Reuniões", "RPG" };
            for (int i = 0; i < abas.Length; i++)
                if (GUILayout.Button(abas[i], botao)) aba = i;
            GUILayout.EndHorizontal();
            GUILayout.Space(8);

            rolagem = GUILayout.BeginScrollView(rolagem, GUILayout.Height(Mathf.Max(110f, janela.height - 150f)));
            if (aba == 0) DesenharGoverno();
            else if (aba == 1) DesenharDiplomacia();
            else if (aba == 2) DesenharReunioes();
            else DesenharRpg();
            GUILayout.EndScrollView();

            GUILayout.Space(5);
            GUILayout.Label(retorno, pequeno);
            GUILayout.EndVertical();
            GUI.DragWindow(new Rect(0, 0, 10000, 26));
        }

        private void DesenharGoverno()
        {
            GUILayout.Label("POLÍTICAS DO REINO", secao);
            GUILayout.Label("Nome da proposta", texto);
            nomePolitica = GUILayout.TextField(nomePolitica, campo);
            leiProtecao = GUILayout.Toggle(leiProtecao, "Proteger aldeias e civis");
            leiComercio = GUILayout.Toggle(leiComercio, "Incentivar o comércio");
            leiRecrutamento = GUILayout.Toggle(leiRecrutamento, "Expandir o recrutamento");
            if (GUILayout.Button("Preparar proposta de lei", botao))
                retorno = "Rascunho local guardado na sessão: " + nomePolitica + ". Ainda não altera o reino.";
            GUILayout.Space(8);
            GUILayout.Label("ADMINISTRAÇÃO", secao);
            GUILayout.Label("Os campos e opções acima são protótipos de interface; ainda não aplicam efeitos ao jogo.", texto);
        }

        private void DesenharDiplomacia()
        {
            GUILayout.Label("DIPLOMACIA ENTRE REINOS", secao);
            GUILayout.Label("Texto da proposta", texto);
            textoDiplomacia = GUILayout.TextArea(textoDiplomacia, campo, GUILayout.MinHeight(70f));
            if (GUILayout.Button("Preparar tratado", botao))
                retorno = "Proposta diplomática preparada como rascunho. Nenhum reino foi alterado.";
            if (GUILayout.Button("Solicitar reunião", botao))
                retorno = "Pedido de reunião registrado apenas nesta sessão de interface.";
            if (GUILayout.Button("Declarar guerra (bloqueado)", botao))
                retorno = "Ação bloqueada: integração diplomática nativa ainda não validada.";
        }

        private void DesenharReunioes()
        {
            GUILayout.Label("REUNIÕES E VILAS", secao);
            GUILayout.Label("Assunto da reunião", texto);
            textoReuniao = GUILayout.TextArea(textoReuniao, campo, GUILayout.MinHeight(70f));
            if (GUILayout.Button("Preparar reunião do conselho", botao))
                retorno = "Pauta preparada localmente. Ainda não convocou personagens reais.";
            if (GUILayout.Button("Contatar uma vila (visual)", botao))
                retorno = "Interface de contato preparada; falta ligar ao sistema real de cidades.";
            GUILayout.Label("Etapa futura: escolher cidade, representante e resultado da reunião.", pequeno);
        }

        private void DesenharRpg()
        {
            GUILayout.Label("EVENTOS E INTERAÇÕES DE RPG", secao);
            GUILayout.Label("Descrição do evento", texto);
            textoEvento = GUILayout.TextArea(textoEvento, campo, GUILayout.MinHeight(70f));
            if (GUILayout.Button("Criar rascunho de evento", botao))
                retorno = "Rascunho narrativo criado nesta sessão; sem consequência na simulação.";
            if (GUILayout.Button("Escolha: negociar", botao))
                retorno = "Escolha de exemplo selecionada. A consequência ainda não está conectada.";
            if (GUILayout.Button("Escolha: recusar", botao))
                retorno = "Escolha de exemplo selecionada. A consequência ainda não está conectada.";
        }

        private void OnDestroy()
        {
            if (instancia == this) instancia = null;
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
        private static int Limitar(long valor, int minimo, int maximo) { return (int)Math.Max(minimo, Math.Min(maximo, valor)); }
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
