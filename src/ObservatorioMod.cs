using System;
using System.Collections;
using System.Collections.Generic;
using System.Reflection;
using NeoModLoader.api;
using UnityEngine;

namespace ObservatorioDosReinos
{
    internal static class LeitorSeguroDoMundo
    {
        private const BindingFlags B = BindingFlags.Public | BindingFlags.NonPublic | BindingFlags.Instance | BindingFlags.Static;
        internal sealed class Resumo
        {
            public bool MapaEncontrado;
            public int Reinos = -1, Cidades = -1, Unidades = -1;
            public string Diagnostico = "";
        }
        public static Resumo Inspecionar()
        {
            Resumo r = new Resumo();
            List<string> notas = new List<string>();
            try
            {
                Type tMapa = Type.GetType("MapBox, Assembly-CSharp");
                if (tMapa == null) { r.Diagnostico = "MapBox ausente."; return r; }
                FieldInfo fInstancia = tMapa.GetField("instance", B);
                object mapa = fInstancia == null ? null : fInstancia.GetValue(null);
                r.MapaEncontrado = mapa != null;
                if (mapa == null) { r.Diagnostico = "MapBox.instance indisponivel; mundo nao inicializado ou campo ausente."; return r; }
                r.Reinos = Contar(mapa, "kingdoms", notas);
                r.Cidades = Contar(mapa, "cities", notas);
                r.Unidades = Contar(mapa, "units", notas);
                string[] tipos = { "PlayerControl", "Actor", "Kingdom", "City", "MapBox" };
                string[] pistas = { "selected", "controlled", "player", "king", "kingdom", "city", "name", "actor", "current", "unit", "focus", "control", "instance", "target", "follow", "hero" };
                for (int i = 0; i < tipos.Length; i++)
                {
                    Type t = Type.GetType(tipos[i] + ", Assembly-CSharp");
                    notas.Add(t == null ? "tipo " + tipos[i] + " ausente." : "tipo " + t.FullName + ": " + ResumoMembros(t, pistas));
                }
                r.Diagnostico = string.Join(" | ", notas.ToArray());
            }
            catch (Exception ex) { r.Diagnostico = "Sonda interrompida com seguranca: " + ex.GetType().Name + ": " + ex.Message; }
            return r;
        }
        private static int Contar(object mapa, string nome, List<string> notas)
        {
            try
            {
                FieldInfo f = Campo(mapa.GetType(), nome);
                object manager = f == null ? null : f.GetValue(mapa);
                if (manager == null) { notas.Add("gerenciador " + nome + " indisponivel."); return -1; }
                PropertyInfo p = Propriedade(manager.GetType(), "Count");
                if (p != null && p.PropertyType == typeof(int)) { object v = p.GetValue(manager, null); if (v is int) return (int)v; }
                IEnumerable e = manager as IEnumerable;
                if (e == null) { notas.Add("gerenciador " + nome + " sem IEnumerable."); return -1; }
                int n = 0; foreach (object item in e) if (item != null) n++; return n;
            }
            catch (Exception ex) { notas.Add("contagem " + nome + " falhou: " + ex.GetType().Name); return -1; }
        }
        private static string ResumoMembros(Type tipo, string[] pistas)
        {
            List<string> itens = new List<string>();
            for (Type a = tipo; a != null; a = a.BaseType)
            {
                FieldInfo[] fs = a.GetFields(B | BindingFlags.DeclaredOnly);
                for (int i = 0; i < fs.Length; i++) if (Combina(fs[i].Name, pistas)) itens.Add("F:" + fs[i].Name);
                PropertyInfo[] ps = a.GetProperties(B | BindingFlags.DeclaredOnly);
                for (int i = 0; i < ps.Length; i++) if (Combina(ps[i].Name, pistas)) itens.Add("P:" + ps[i].Name);
                MethodInfo[] ms = a.GetMethods(B | BindingFlags.DeclaredOnly);
                for (int i = 0; i < ms.Length; i++) if (Combina(ms[i].Name, pistas)) itens.Add("M:" + ms[i].Name);
            }
            if (itens.Count == 0) return "(sem correspondencias)";
            if (itens.Count > 35) itens.RemoveRange(35, itens.Count - 35);
            return string.Join(",", itens.ToArray());
        }
        private static bool Combina(string n, string[] pistas)
        {
            for (int i = 0; i < pistas.Length; i++) if (n.IndexOf(pistas[i], StringComparison.OrdinalIgnoreCase) >= 0) return true;
            return false;
        }
        private static FieldInfo Campo(Type t, string nome)
        {
            for (Type a = t; a != null; a = a.BaseType) { FieldInfo f = a.GetField(nome, B | BindingFlags.DeclaredOnly); if (f != null) return f; }
            return null;
        }
        private static PropertyInfo Propriedade(Type t, string nome)
        {
            for (Type a = t; a != null; a = a.BaseType) { PropertyInfo p = a.GetProperty(nome, B | BindingFlags.DeclaredOnly); if (p != null) return p; }
            return null;
        }
    }

    /// <summary>Menu visual responsivo. As opções são rascunhos e não alteram o mundo real.</summary>
    public sealed class MenuAdministracaoOverlay : MonoBehaviour
    {
        private const int IdJanela = 490271;
        private static MenuAdministracaoOverlay instancia;
        private bool aberto;
        private int aba;
        private Rect janela;
        private Vector2 rolagem;
        private string nomePolitica = "Proteção das aldeias";
        private string textoReuniao = "Convocar conselho real para discutir as necessidades do reino.";
        private string textoDiplomacia = "Propor um tratado de paz e comércio.";
        private string textoEvento = "Um emissário chega ao castelo com notícias da fronteira.";
        private string retorno = "Protótipo visual: ações ainda não alteram o mundo.";
        private bool leiProtecao = true, leiComercio, leiRecrutamento;
        private GUIStyle titulo, secao, texto, botao, campo, pequeno, caixa;

        public static void CriarSeNecessario()
        {
            if (instancia != null) return;
            MenuAdministracaoOverlay e = FindObjectOfType<MenuAdministracaoOverlay>();
            if (e != null) { instancia = e; return; }
            GameObject go = new GameObject("ObservatorioDosReinos_MenuAdmin");
            DontDestroyOnLoad(go);
            instancia = go.AddComponent<MenuAdministracaoOverlay>();
        }
        private void OnGUI()
        {
            try
            {
                Estilos();
                float margem = Mathf.Clamp(Screen.width * 0.02f, 8f, 18f);
                float largAtalho = Mathf.Clamp(Screen.width * 0.22f, 112f, 184f);
                float altAtalho = Mathf.Clamp(Screen.height * 0.045f, 34f, 48f);
                Color anterior = GUI.color;
                GUI.color = Color.white;
                if (GUI.Button(new Rect(margem, margem, largAtalho, altAtalho), aberto ? "Fechar governo" : "Governo", botao)) aberto = !aberto;
                if (aberto)
                {
                    float largura = Mathf.Min(620f, Screen.width - margem * 2f);
                    float altura = Mathf.Min(Mathf.Clamp(Screen.height * 0.72f, 300f, 660f), Screen.height - margem * 2f);
                    janela = new Rect((Screen.width - largura) * 0.5f, (Screen.height - altura) * 0.5f, largura, altura);
                    janela = GUI.Window(IdJanela, janela, DesenharJanela, "OBSERVATÓRIO DOS REINOS", caixa);
                }
                GUI.color = anterior;
            }
            catch (Exception ex) { GUI.color = Color.white; Debug.LogWarning("[ObservatorioDosReinos] UI indisponivel: " + ex.GetType().Name); }
        }
        private void Estilos()
        {
            if (titulo != null) return;
            int fonte = Mathf.Clamp(Screen.width / 38, 12, 20);
            titulo = new GUIStyle(GUI.skin.label) { fontSize = fonte + 2, fontStyle = FontStyle.Bold, wordWrap = true };
            titulo.normal.textColor = new Color(1f, 0.68f, 0.24f);
            secao = new GUIStyle(GUI.skin.label) { fontSize = fonte, fontStyle = FontStyle.Bold, wordWrap = true };
            secao.normal.textColor = new Color(1f, 0.72f, 0.34f);
            texto = new GUIStyle(GUI.skin.label) { fontSize = fonte, wordWrap = true };
            texto.normal.textColor = new Color(0.93f, 0.92f, 0.82f);
            botao = new GUIStyle(GUI.skin.button) { fontSize = fonte, wordWrap = true, fixedHeight = Mathf.Clamp(fonte * 2.8f, 34f, 54f) };
            campo = new GUIStyle(GUI.skin.textField) { fontSize = fonte, fixedHeight = Mathf.Clamp(fonte * 2.5f, 30f, 46f) };
            pequeno = new GUIStyle(texto) { fontSize = Mathf.Max(10, fonte - 2) };
            caixa = new GUIStyle(GUI.skin.window) { fontSize = fonte, fontStyle = FontStyle.Bold };
            caixa.normal.textColor = new Color(1f, 0.68f, 0.24f);
        }
        private void DesenharJanela(int id)
        {
            GUILayout.BeginVertical();
            GUILayout.Label("Painel de administração • protótipo", titulo);
            GUILayout.Label("Personagem/reino: integração real ainda pendente.", pequeno);
            GUILayout.BeginHorizontal();
            string[] abas = { "Governo", "Diplomacia", "Reuniões", "RPG" };
            for (int i = 0; i < abas.Length; i++) if (GUILayout.Button(abas[i], botao)) aba = i;
            GUILayout.EndHorizontal();
            rolagem = GUILayout.BeginScrollView(rolagem, GUILayout.Height(Mathf.Max(110f, janela.height - 150f)));
            if (aba == 0) DesenharGoverno();
            else if (aba == 1) DesenharDiplomacia();
            else if (aba == 2) DesenharReunioes();
            else DesenharRpg();
            GUILayout.EndScrollView();
            GUILayout.Space(4);
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
            if (GUILayout.Button("Preparar proposta de lei", botao)) retorno = "Rascunho local preparado: " + nomePolitica + ". Não altera o reino.";
            GUILayout.Space(8);
            GUILayout.Label("ADMINISTRAÇÃO", secao);
            GUILayout.Label("Estas opções são visuais e ainda não aplicam efeitos ao jogo.", texto);
        }
        private void DesenharDiplomacia()
        {
            GUILayout.Label("DIPLOMACIA ENTRE REINOS", secao);
            GUILayout.Label("Texto da proposta", texto);
            textoDiplomacia = GUILayout.TextArea(textoDiplomacia, campo, GUILayout.MinHeight(70f));
            if (GUILayout.Button("Preparar tratado", botao)) retorno = "Rascunho de tratado preparado; nenhum reino foi alterado.";
            if (GUILayout.Button("Solicitar reunião", botao)) retorno = "Pedido de reunião preparado nesta sessão.";
            if (GUILayout.Button("Declarar guerra (bloqueado)", botao)) retorno = "Bloqueado até validar a API diplomática nativa.";
        }
        private void DesenharReunioes()
        {
            GUILayout.Label("REUNIÕES E VILAS", secao);
            GUILayout.Label("Assunto da reunião", texto);
            textoReuniao = GUILayout.TextArea(textoReuniao, campo, GUILayout.MinHeight(70f));
            if (GUILayout.Button("Preparar reunião do conselho", botao)) retorno = "Pauta preparada; ainda não convoca personagens reais.";
            if (GUILayout.Button("Contatar uma vila (visual)", botao)) retorno = "Contato visual preparado; integração de cidades pendente.";
            GUILayout.Label("Futuro: escolher cidade, representante e resultado.", pequeno);
        }
        private void DesenharRpg()
        {
            GUILayout.Label("EVENTOS E INTERAÇÕES DE RPG", secao);
            GUILayout.Label("Descrição do evento", texto);
            textoEvento = GUILayout.TextArea(textoEvento, campo, GUILayout.MinHeight(70f));
            if (GUILayout.Button("Criar rascunho de evento", botao)) retorno = "Rascunho narrativo criado nesta sessão, sem efeito no mundo.";
            if (GUILayout.Button("Escolha: negociar", botao)) retorno = "Escolha de exemplo selecionada; consequência não conectada.";
            if (GUILayout.Button("Escolha: recusar", botao)) retorno = "Escolha de exemplo selecionada; consequência não conectada.";
        }
        private void OnDestroy() { if (instancia == this) instancia = null; }
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
            Estabilidade = 60; Legitimidade = 50; Tesouro = 100;
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
        public RelacaoDiplomatica(string a, string b)
        {
            if (string.IsNullOrEmpty(a) || string.IsNullOrEmpty(b)) throw new ArgumentException("IDs obrigatórios.");
            if (a == b) throw new ArgumentException("Um reino não pode ter relação consigo mesmo.");
            if (string.CompareOrdinal(a, b) < 0) { ReinoA = a; ReinoB = b; } else { ReinoA = b; ReinoB = a; }
            Tipo = TipoRelacaoDiplomatica.Neutra; Opiniao = 0;
        }
        public void DefinirTipo(TipoRelacaoDiplomatica tipo) { if (!Enum.IsDefined(typeof(TipoRelacaoDiplomatica), tipo)) throw new ArgumentOutOfRangeException("tipo"); Tipo = tipo; }
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
            if (string.IsNullOrEmpty(id)) throw new ArgumentException("O ID é obrigatório.");
            PoliticaDoReino existente;
            if (reinos.TryGetValue(id, out existente)) return existente;
            PoliticaDoReino novo = new PoliticaDoReino(id, nome); reinos.Add(id, novo); return novo;
        }
        public bool TentarObterReino(string id, out PoliticaDoReino reino) { return reinos.TryGetValue(id, out reino); }
        public RelacaoDiplomatica ObterOuCriarRelacao(string a, string b)
        {
            if (string.IsNullOrEmpty(a) || string.IsNullOrEmpty(b)) throw new ArgumentException("IDs obrigatórios.");
            if (a == b) throw new ArgumentException("Um reino não pode ter relação consigo mesmo.");
            string primeiro = string.CompareOrdinal(a, b) < 0 ? a : b;
            string segundo = string.CompareOrdinal(a, b) < 0 ? b : a;
            string chave = primeiro + "::" + segundo;
            RelacaoDiplomatica relacao;
            if (relacoes.TryGetValue(chave, out relacao)) return relacao;
            relacao = new RelacaoDiplomatica(primeiro, segundo); relacoes.Add(chave, relacao); return relacao;
        }
    }

    public class Main : BasicMod<Main>
    {
        private static EstadoPolitico estadoPolitico;
        protected override void OnModLoad()
        {
            estadoPolitico = new EstadoPolitico();
            PoliticaDoReino a = estadoPolitico.RegistrarReino("diagnostico-a", "Reino de teste A");
            PoliticaDoReino b = estadoPolitico.RegistrarReino("diagnostico-b", "Reino de teste B");
            RelacaoDiplomatica relacao = estadoPolitico.ObterOuCriarRelacao(a.Id, b.Id);
            relacao.DefinirTipo(TipoRelacaoDiplomatica.Comercio);
            relacao.AjustarOpiniao(10);
            LogInfo("Observatorio dos Reinos carregado.");
            LogInfo("Autoteste politico: reinos=" + estadoPolitico.QuantidadeDeReinos + ", relacoes=" + estadoPolitico.QuantidadeDeRelacoes + ", tipo=" + relacao.Tipo + ", opiniao=" + relacao.Opiniao + ".");
            LeitorSeguroDoMundo.Resumo mundo = LeitorSeguroDoMundo.Inspecionar();
            LogInfo("Diagnostico do mundo: mapa=" + mundo.MapaEncontrado + ", reinos=" + mundo.Reinos + ", cidades=" + mundo.Cidades + ", unidades=" + mundo.Unidades + ".");
            LogInfo("Diagnostico de API (somente leitura): " + mundo.Diagnostico);
            try
            {
                MenuAdministracaoOverlay.CriarSeNecessario();
                LogInfo("Menu visual administrativo criado; ações ainda são protótipos.");
            }
            catch (Exception ex) { LogInfo("Menu visual indisponível nesta build: " + ex.GetType().Name + "."); }
            LogInfo("Modo seguro: nenhuma entidade nativa foi alterada.");
        }
    }
}
