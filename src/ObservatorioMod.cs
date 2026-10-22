using System;
using System.Collections;
using System.Collections.Generic;
using System.Reflection;
using NeoModLoader.api;
using UnityEngine;
using UnityEngine.UI;

namespace ObservatorioDosReinos
{
    internal static class LeitorSeguroDoMundo
    {
        private const BindingFlags B = BindingFlags.Public | BindingFlags.NonPublic | BindingFlags.Instance | BindingFlags.Static;
        internal sealed class Resumo { public bool MapaEncontrado; public int Reinos=-1, Cidades=-1, Unidades=-1; public string Diagnostico=""; }

        public static Resumo Inspecionar()
        {
            Resumo r = new Resumo();
            List<string> notas = new List<string>();
            try
            {
                Type tm = Type.GetType("MapBox, Assembly-CSharp");
                if (tm == null) { r.Diagnostico = "MapBox ausente."; return r; }
                FieldInfo fi = tm.GetField("instance", B);
                object mapa = fi == null ? null : fi.GetValue(null);
                r.MapaEncontrado = mapa != null;
                if (mapa == null) { r.Diagnostico = "MapBox.instance indisponivel."; return r; }
                r.Reinos = Contar(mapa, "kingdoms", notas);
                r.Cidades = Contar(mapa, "cities", notas);
                r.Unidades = Contar(mapa, "units", notas);
                string[] tipos = { "PlayerControl", "Actor", "Kingdom", "City", "MapBox" };
                string[] pistas = { "selected", "controlled", "player", "king", "kingdom", "city", "name", "actor", "current", "unit", "focus", "control", "instance", "target", "follow", "hero" };
                for (int i=0; i<tipos.Length; i++)
                {
                    Type t = Type.GetType(tipos[i] + ", Assembly-CSharp");
                    notas.Add(t == null ? "tipo " + tipos[i] + " ausente." : "tipo " + t.FullName + ": " + ListarMembros(t, pistas));
                }
                r.Diagnostico = string.Join(" | ", notas.ToArray());
            }
            catch (Exception ex) { r.Diagnostico = "Sonda interrompida: " + ex.GetType().Name + ": " + ex.Message; }
            return r;
        }

        private static int Contar(object mapa, string nome, List<string> notas)
        {
            try
            {
                FieldInfo f = AcharCampo(mapa.GetType(), nome);
                object manager = f == null ? null : f.GetValue(mapa);
                if (manager == null) { notas.Add("gerenciador " + nome + " indisponivel."); return -1; }
                PropertyInfo p = AcharPropriedade(manager.GetType(), "Count");
                if (p != null && p.PropertyType == typeof(int)) { object v=p.GetValue(manager,null); if (v is int) return (int)v; }
                IEnumerable e = manager as IEnumerable;
                if (e == null) { notas.Add("gerenciador " + nome + " sem IEnumerable."); return -1; }
                int n=0; foreach (object item in e) if (item!=null) n++; return n;
            }
            catch (Exception ex) { notas.Add("contagem " + nome + " falhou: " + ex.GetType().Name); return -1; }
        }

        private static string ListarMembros(Type t, string[] pistas)
        {
            List<string> a = new List<string>();
            for (Type c=t; c!=null; c=c.BaseType)
            {
                FieldInfo[] fs=c.GetFields(B|BindingFlags.DeclaredOnly);
                for (int i=0;i<fs.Length;i++) if (Tem(fs[i].Name,pistas)) a.Add("F:"+fs[i].Name);
                PropertyInfo[] ps=c.GetProperties(B|BindingFlags.DeclaredOnly);
                for (int i=0;i<ps.Length;i++) if (Tem(ps[i].Name,pistas)) a.Add("P:"+ps[i].Name);
                MethodInfo[] ms=c.GetMethods(B|BindingFlags.DeclaredOnly);
                for (int i=0;i<ms.Length;i++) if (Tem(ms[i].Name,pistas)) a.Add("M:"+ms[i].Name);
            }
            if (a.Count==0) return "(sem correspondencias)";
            if (a.Count>35) a.RemoveRange(35,a.Count-35);
            return string.Join(",",a.ToArray());
        }
        private static bool Tem(string n,string[] p) { for(int i=0;i<p.Length;i++) if(n.IndexOf(p[i],StringComparison.OrdinalIgnoreCase)>=0)return true; return false; }
        private static FieldInfo AcharCampo(Type t,string n) { for(Type c=t;c!=null;c=c.BaseType){FieldInfo f=c.GetField(n,B|BindingFlags.DeclaredOnly);if(f!=null)return f;}return null; }
        private static PropertyInfo AcharPropriedade(Type t,string n) { for(Type c=t;c!=null;c=c.BaseType){PropertyInfo p=c.GetProperty(n,B|BindingFlags.DeclaredOnly);if(p!=null)return p;}return null; }
    }

    /// <summary>
    /// Janela administrativa nativa do WorldBox, criada pelo ciclo AbstractWideWindow do NeoModLoader.
    /// Não intercepta StatsWindow nem instala patches Harmony.
    /// </summary>
    public sealed class ObservatorioJanelaNativa : AbstractWideWindow<ObservatorioJanelaNativa>
    {
        public const string IdJanela = "observatorio_dos_reinos";

        private Font fonte;
        private Text textoEstado;
        private readonly GameObject[] paginas = new GameObject[4];
        private InputField campoLei;
        private InputField campoTratado;
        private InputField campoReuniao;
        private InputField campoEvento;
        private bool protegerAldeias = true;
        private bool incentivarComercio;
        private bool ampliarRecrutamento;
        private readonly List<string> propostasLeis = new List<string>();
        private readonly List<string> propostasDiplomaticas = new List<string>();
        private string ultimaPauta = "";
        private string ultimoEvento = "";

        public static void Inicializar()
        {
            if (Instance == null)
            {
                CreateAndInit(IdJanela, new Vector2(600f, 420f));
                Debug.Log("[Observatorio] NATIVE_WINDOW_CREATED: janela registrada pelo NeoModLoader/AbstractWideWindow.");
            }
            ObservatorioAtalhoGov.TentarCriar();
        }

        public static void Abrir()
        {
            if (Instance == null) Inicializar();
            ScrollWindow.showWindow(IdJanela);
        }

        protected override void Init()
        {
            fonte = Resources.GetBuiltinResource<Font>("Arial.ttf");
            if (fonte == null) fonte = Resources.GetBuiltinResource<Font>("LegacyRuntime.ttf");

            if (ContentTransform == null) throw new InvalidOperationException("AbstractWideWindow não forneceu ContentTransform.");
            PrepararLayoutVertical(ContentTransform, 8f, new RectOffset(12, 12, 10, 10));

            AdicionarTexto(ContentTransform, "OBSERVATÓRIO DOS REINOS", 20, true, 36f);
            AdicionarTexto(ContentTransform,
                "Painel administrativo experimental. As propostas ficam nesta sessão e ainda não modificam reinos nativos.",
                13, false, 52f);

            GameObject navegacao = new GameObject("Observatorio_Navegacao",
                typeof(RectTransform), typeof(HorizontalLayoutGroup), typeof(LayoutElement));
            navegacao.transform.SetParent(ContentTransform, false);
            LayoutElement navLayout = navegacao.GetComponent<LayoutElement>();
            navLayout.preferredHeight = 42f;
            navLayout.minHeight = 42f;
            HorizontalLayoutGroup navGroup = navegacao.GetComponent<HorizontalLayoutGroup>();
            navGroup.spacing = 5f;
            navGroup.childControlWidth = true;
            navGroup.childForceExpandWidth = true;
            navGroup.childControlHeight = true;
            navGroup.childForceExpandHeight = true;

            string[] nomesAbas = { "Governo", "Diplomacia", "Reuniões", "RPG" };
            for (int i = 0; i < nomesAbas.Length; i++)
            {
                int indice = i;
                CriarBotao(navegacao.transform, nomesAbas[i], delegate { TrocarPagina(indice); });
            }

            paginas[0] = CriarPagina("Pagina_Governo");
            AdicionarTexto(paginas[0].transform, "GOVERNO E POLÍTICAS", 16, true, 32f);
            AdicionarTexto(paginas[0].transform, "As opções abaixo são guardadas pelo mod nesta sessão; não alteram a IA, os impostos ou as ações dos reinos do jogo.", 13, false, 58f);
            CriarBotao(paginas[0].transform, TextoProtecao(), delegate
            {
                protegerAldeias = !protegerAldeias;
                AtualizarTextoBotao("Proteção de aldeias", TextoProtecao());
                AtualizarStatus("Preferência local atualizada: " + (protegerAldeias ? "proteção de aldeias ativada" : "proteção de aldeias desativada") + ".");
            });
            CriarBotao(paginas[0].transform, TextoComercio(), delegate
            {
                incentivarComercio = !incentivarComercio;
                AtualizarTextoBotao("Incentivo ao comércio", TextoComercio());
                AtualizarStatus("Preferência local de comércio " + (incentivarComercio ? "ativada" : "desativada") + ".");
            });
            CriarBotao(paginas[0].transform, TextoRecrutamento(), delegate
            {
                ampliarRecrutamento = !ampliarRecrutamento;
                AtualizarTextoBotao("Recrutamento", TextoRecrutamento());
                AtualizarStatus("Preferência local de recrutamento " + (ampliarRecrutamento ? "ampliada" : "normal") + ".");
            });
            campoLei = CriarCampo(paginas[0].transform, "Ex.: Proteger aldeias de ataques");
            CriarBotao(paginas[0].transform, "Registrar proposta de lei", delegate
            {
                string proposta = TextoOuPadrao(campoLei, "Nova proposta de lei");
                propostasLeis.Add(proposta);
                AtualizarStatus("Proposta registrada nesta sessão (" + propostasLeis.Count + "): " + proposta);
            });

            paginas[1] = CriarPagina("Pagina_Diplomacia");
            AdicionarTexto(paginas[1].transform, "DIPLOMACIA ENTRE REINOS", 16, true, 32f);
            AdicionarTexto(paginas[1].transform, "Registre propostas de relação para planejamento. Elas ainda não são enviadas a outro reino.", 13, false, 52f);
            campoTratado = CriarCampo(paginas[1].transform, "Ex.: Tratado de paz e comércio");
            CriarBotao(paginas[1].transform, "Registrar proposta de paz", delegate { RegistrarDiplomacia("Paz", campoTratado); });
            CriarBotao(paginas[1].transform, "Registrar proposta comercial", delegate { RegistrarDiplomacia("Comércio", campoTratado); });
            CriarBotao(paginas[1].transform, "Registrar proposta de aliança", delegate { RegistrarDiplomacia("Aliança", campoTratado); });
            CriarBotao(paginas[1].transform, "Guerra (indisponível)", delegate
            {
                AtualizarStatus("Declaração de guerra desativada: a integração com a API nativa ainda não está implementada.");
            });

            paginas[2] = CriarPagina("Pagina_Reunioes");
            AdicionarTexto(paginas[2].transform, "REUNIÕES E CONSELHO", 16, true, 32f);
            AdicionarTexto(paginas[2].transform, "Guarde uma pauta para retomar durante a sessão de jogo.", 13, false, 34f);
            campoReuniao = CriarCampo(paginas[2].transform, "Ex.: Segurança das aldeias e escassez de alimentos");
            CriarBotao(paginas[2].transform, "Registrar pauta do conselho", delegate
            {
                ultimaPauta = TextoOuPadrao(campoReuniao, "Pauta do conselho");
                AtualizarStatus("Pauta registrada nesta sessão: " + ultimaPauta);
            });

            paginas[3] = CriarPagina("Pagina_RPG");
            AdicionarTexto(paginas[3].transform, "EVENTOS E INTERAÇÕES DE RPG", 16, true, 32f);
            AdicionarTexto(paginas[3].transform, "Crie um registro narrativo e escolha uma resposta. Isso não gera eventos nativos no mapa ainda.", 13, false, 52f);
            campoEvento = CriarCampo(paginas[3].transform, "Ex.: Um emissário chega com notícias da fronteira");
            CriarBotao(paginas[3].transform, "Registrar evento narrativo", delegate
            {
                ultimoEvento = TextoOuPadrao(campoEvento, "Evento narrativo");
                AtualizarStatus("Evento narrativo registrado nesta sessão: " + ultimoEvento);
            });
            CriarBotao(paginas[3].transform, "Escolha: negociar", delegate
            {
                ultimoEvento = TextoOuPadrao(campoEvento, "Evento narrativo") + " | resposta: negociar";
                AtualizarStatus("Resposta registrada localmente: negociar.");
            });
            CriarBotao(paginas[3].transform, "Escolha: recusar", delegate
            {
                ultimoEvento = TextoOuPadrao(campoEvento, "Evento narrativo") + " | resposta: recusar";
                AtualizarStatus("Resposta registrada localmente: recusar.");
            });

            textoEstado = AdicionarTexto(ContentTransform, "Estado: pronto.", 12, false, 42f);
            TrocarPagina(0);
        }

        public override void OnFirstEnable()
        {
            AtualizarStatus("Janela nativa aberta. Use as abas para navegar.");
        }

        public override void OnNormalEnable() { }

        public override void OnNormalDisable() { }

        private GameObject CriarPagina(string nome)
        {
            GameObject pagina = new GameObject(nome,
                typeof(RectTransform), typeof(VerticalLayoutGroup), typeof(ContentSizeFitter));
            pagina.transform.SetParent(ContentTransform, false);
            PrepararLayoutVertical(pagina.transform, 7f, new RectOffset(6, 6, 6, 6));
            return pagina;
        }

        private void TrocarPagina(int indice)
        {
            for (int i = 0; i < paginas.Length; i++)
            {
                if (paginas[i] != null) paginas[i].SetActive(i == indice);
            }
            AtualizarStatus("Aba aberta: " + new string[] { "Governo", "Diplomacia", "Reuniões", "RPG" }[indice] + ".");
        }

        private void RegistrarDiplomacia(string tipo, InputField campo)
        {
            string proposta = tipo + ": " + TextoOuPadrao(campo, "Proposta diplomática");
            propostasDiplomaticas.Add(proposta);
            AtualizarStatus("Proposta diplomática registrada nesta sessão (" + propostasDiplomaticas.Count + "): " + proposta);
        }

        private string TextoProtecao() { return "Proteção de aldeias: " + (protegerAldeias ? "ATIVA" : "INATIVA"); }
        private string TextoComercio() { return "Incentivo ao comércio: " + (incentivarComercio ? "ATIVO" : "INATIVO"); }
        private string TextoRecrutamento() { return "Recrutamento: " + (ampliarRecrutamento ? "AMPLIADO" : "NORMAL"); }

        private void AtualizarTextoBotao(string nome, string legenda)
        {
            Button[] botoes = ContentTransform.GetComponentsInChildren<Button>(true);
            for (int i = 0; i < botoes.Length; i++)
            {
                if (botoes[i].gameObject.name.StartsWith("Botao_" + nome, StringComparison.Ordinal))
                {
                    Text t = botoes[i].GetComponentInChildren<Text>(true);
                    if (t != null) t.text = legenda;
                    return;
                }
            }
        }

        private void AtualizarStatus(string mensagem)
        {
            if (textoEstado != null) textoEstado.text = "Estado: " + mensagem;
        }

        private static string TextoOuPadrao(InputField campo, string padrao)
        {
            if (campo == null || string.IsNullOrWhiteSpace(campo.text)) return padrao;
            return campo.text.Trim();
        }

        private Text AdicionarTexto(Transform parent, string frase, int tamanho, bool negrito, float altura)
        {
            GameObject go = new GameObject("Texto_" + frase, typeof(RectTransform), typeof(Text), typeof(LayoutElement));
            go.transform.SetParent(parent, false);
            Text texto = go.GetComponent<Text>();
            texto.font = fonte;
            texto.fontSize = tamanho;
            texto.fontStyle = negrito ? FontStyle.Bold : FontStyle.Normal;
            texto.color = negrito ? new Color(1f, 0.72f, 0.30f, 1f) : new Color(0.92f, 0.93f, 0.85f, 1f);
            texto.text = frase;
            texto.supportRichText = false;
            texto.alignment = TextAnchor.MiddleLeft;
            texto.horizontalOverflow = HorizontalWrapMode.Wrap;
            texto.verticalOverflow = VerticalWrapMode.Overflow;
            LayoutElement le = go.GetComponent<LayoutElement>();
            le.preferredHeight = altura;
            le.minHeight = altura;
            return texto;
        }

        private InputField CriarCampo(Transform parent, string placeholder)
        {
            GameObject go = new GameObject("Campo_" + placeholder,
                typeof(RectTransform), typeof(Image), typeof(InputField), typeof(LayoutElement));
            go.transform.SetParent(parent, false);
            Image image = go.GetComponent<Image>();
            image.color = new Color(0.10f, 0.12f, 0.10f, 0.98f);
            InputField input = go.GetComponent<InputField>();
            input.targetGraphic = image;
            input.lineType = InputField.LineType.MultiLineNewline;
            LayoutElement le = go.GetComponent<LayoutElement>();
            le.preferredHeight = 62f;
            le.minHeight = 56f;

            Text value = CriarTextoCampo(go.transform, "", new Color(0.96f, 0.95f, 0.88f, 1f), false);
            value.alignment = TextAnchor.UpperLeft;
            input.textComponent = value;

            Text hint = CriarTextoCampo(go.transform, placeholder, new Color(0.57f, 0.60f, 0.53f, 1f), true);
            hint.alignment = TextAnchor.UpperLeft;
            input.placeholder = hint;
            input.text = "";
            return input;
        }

        private Text CriarTextoCampo(Transform parent, string frase, Color cor, bool italico)
        {
            GameObject go = new GameObject("TextoCampo", typeof(RectTransform), typeof(Text));
            go.transform.SetParent(parent, false);
            RectTransform rect = go.GetComponent<RectTransform>();
            rect.anchorMin = Vector2.zero;
            rect.anchorMax = Vector2.one;
            rect.offsetMin = new Vector2(10f, 7f);
            rect.offsetMax = new Vector2(-10f, -7f);
            Text texto = go.GetComponent<Text>();
            texto.font = fonte;
            texto.fontSize = 14;
            texto.fontStyle = italico ? FontStyle.Italic : FontStyle.Normal;
            texto.color = cor;
            texto.text = frase;
            texto.supportRichText = false;
            texto.horizontalOverflow = HorizontalWrapMode.Wrap;
            texto.verticalOverflow = VerticalWrapMode.Overflow;
            return texto;
        }

        private Button CriarBotao(Transform parent, string legenda, Action acao)
        {
            GameObject go = new GameObject("Botao_" + legenda,
                typeof(RectTransform), typeof(Image), typeof(Button), typeof(LayoutElement));
            go.transform.SetParent(parent, false);
            Image image = go.GetComponent<Image>();
            image.color = new Color(0.30f, 0.33f, 0.25f, 1f);
            Button button = go.GetComponent<Button>();
            button.targetGraphic = image;
            button.onClick.AddListener(acao);
            LayoutElement le = go.GetComponent<LayoutElement>();
            le.preferredHeight = 42f;
            le.minHeight = 38f;
            Text texto = CriarTextoCampo(go.transform, legenda, Color.white, false);
            texto.fontStyle = FontStyle.Bold;
            texto.alignment = TextAnchor.MiddleCenter;
            return button;
        }

        private static void PrepararLayoutVertical(Transform target, float espacamento, RectOffset padding)
        {
            VerticalLayoutGroup layout = target.GetComponent<VerticalLayoutGroup>();
            if (layout == null) layout = target.gameObject.AddComponent<VerticalLayoutGroup>();
            layout.padding = padding;
            layout.spacing = espacamento;
            layout.childControlWidth = true;
            layout.childForceExpandWidth = true;
            layout.childControlHeight = false;
            layout.childForceExpandHeight = false;

            ContentSizeFitter fitter = target.GetComponent<ContentSizeFitter>();
            if (fitter == null) fitter = target.gameObject.AddComponent<ContentSizeFitter>();
            fitter.verticalFit = ContentSizeFitter.FitMode.PreferredSize;
            fitter.horizontalFit = ContentSizeFitter.FitMode.Unconstrained;
        }
    }

    /// <summary>Atalho no Canvas de interface do jogo; a janela em si é um ScrollWindow nativo.</summary>
    internal static class ObservatorioAtalhoGov
    {
        private static GameObject botaoRaiz;

        internal static bool TentarCriar()
        {
            if (botaoRaiz != null) return true;
            CanvasMain canvasMain = CanvasMain.instance;
            if (canvasMain == null || canvasMain.canvas_ui == null) return false;

            Transform parent = canvasMain.canvas_ui.transform;
            Transform anterior = parent.Find("ObservatorioDosReinos_BotaoGoverno");
            if (anterior != null)
            {
                botaoRaiz = anterior.gameObject;
                return true;
            }

            botaoRaiz = new GameObject("ObservatorioDosReinos_BotaoGoverno",
                typeof(RectTransform), typeof(Image), typeof(Button));
            botaoRaiz.transform.SetParent(parent, false);
            RectTransform rect = botaoRaiz.GetComponent<RectTransform>();
            rect.anchorMin = new Vector2(0f, 1f);
            rect.anchorMax = new Vector2(0f, 1f);
            rect.pivot = new Vector2(0f, 1f);
            rect.anchoredPosition = new Vector2(16f, -138f);
            rect.sizeDelta = new Vector2(132f, 42f);

            Image image = botaoRaiz.GetComponent<Image>();
            image.color = new Color(0.18f, 0.23f, 0.15f, 0.96f);
            Button button = botaoRaiz.GetComponent<Button>();
            button.targetGraphic = image;
            button.onClick.AddListener(ObservatorioJanelaNativa.Abrir);

            GameObject textoObj = new GameObject("Texto_GOVERNO", typeof(RectTransform), typeof(Text));
            textoObj.transform.SetParent(botaoRaiz.transform, false);
            RectTransform textoRect = textoObj.GetComponent<RectTransform>();
            textoRect.anchorMin = Vector2.zero;
            textoRect.anchorMax = Vector2.one;
            textoRect.offsetMin = new Vector2(4f, 2f);
            textoRect.offsetMax = new Vector2(-4f, -2f);
            Text texto = textoObj.GetComponent<Text>();
            texto.font = Resources.GetBuiltinResource<Font>("Arial.ttf");
            if (texto.font == null) texto.font = Resources.GetBuiltinResource<Font>("LegacyRuntime.ttf");
            texto.fontSize = 15;
            texto.fontStyle = FontStyle.Bold;
            texto.alignment = TextAnchor.MiddleCenter;
            texto.color = new Color(1f, 0.78f, 0.35f, 1f);
            texto.text = "GOVERNO";
            texto.supportRichText = false;

            botaoRaiz.transform.SetAsLastSibling();
            Debug.Log("[Observatorio] NATIVE_WINDOW_BUTTON_CREATED: botão GOVERNO no CanvasMain.canvas_ui.");
            return true;
        }
    }

    public sealed class PoliticaDoReino
    {
        public string Id { get; private set; }
        public string Nome { get; private set; }
        public int Estabilidade { get; private set; }
        public int Legitimidade { get; private set; }
        public int Tesouro { get; private set; }
        public PoliticaDoReino(string id,string nome){Id=string.IsNullOrEmpty(id)?Guid.NewGuid().ToString("N"):id;Nome=string.IsNullOrEmpty(nome)?"Reino sem nome":nome;Estabilidade=60;Legitimidade=50;Tesouro=100;}
        public void AjustarEstabilidade(int v){Estabilidade=Limitar((long)Estabilidade+v,0,100);}
        public void AjustarLegitimidade(int v){Legitimidade=Limitar((long)Legitimidade+v,0,100);}
        public void AjustarTesouro(int v){Tesouro=Limitar((long)Tesouro+v,0,1000000);}
        private static int Limitar(long v,int min,int max){return (int)Math.Max(min,Math.Min(max,v));}
    }
    public enum TipoRelacaoDiplomatica { Neutra=0, Comercio=1, Alianca=2, Rivalidade=3, Guerra=4 }
    public sealed class RelacaoDiplomatica
    {
        public string ReinoA { get; private set; }
        public string ReinoB { get; private set; }
        public TipoRelacaoDiplomatica Tipo { get; private set; }
        public int Opiniao { get; private set; }
        public RelacaoDiplomatica(string a,string b){if(string.IsNullOrEmpty(a)||string.IsNullOrEmpty(b))throw new ArgumentException("IDs obrigatorios.");if(a==b)throw new ArgumentException("Um reino nao pode ter relacao consigo mesmo.");if(string.CompareOrdinal(a,b)<0){ReinoA=a;ReinoB=b;}else{ReinoA=b;ReinoB=a;}Tipo=TipoRelacaoDiplomatica.Neutra;Opiniao=0;}
        public void DefinirTipo(TipoRelacaoDiplomatica t){if(!Enum.IsDefined(typeof(TipoRelacaoDiplomatica),t))throw new ArgumentOutOfRangeException("t");Tipo=t;}
        public void AjustarOpiniao(int v){Opiniao=(int)Math.Max(-100L,Math.Min(100L,(long)Opiniao+v));}
        public string Chave{get{return ReinoA+"::"+ReinoB;}}
    }
    public sealed class EstadoPolitico
    {
        private readonly Dictionary<string,PoliticaDoReino> reinos=new Dictionary<string,PoliticaDoReino>();
        private readonly Dictionary<string,RelacaoDiplomatica> relacoes=new Dictionary<string,RelacaoDiplomatica>();
        public int QuantidadeDeReinos{get{return reinos.Count;}}
        public int QuantidadeDeRelacoes{get{return relacoes.Count;}}
        public PoliticaDoReino RegistrarReino(string id,string nome){if(string.IsNullOrEmpty(id))throw new ArgumentException("ID obrigatorio.");PoliticaDoReino existente;if(reinos.TryGetValue(id,out existente))return existente;PoliticaDoReino novo=new PoliticaDoReino(id,nome);reinos.Add(id,novo);return novo;}
        public bool TentarObterReino(string id,out PoliticaDoReino reino){return reinos.TryGetValue(id,out reino);}
        public RelacaoDiplomatica ObterOuCriarRelacao(string a,string b){if(string.IsNullOrEmpty(a)||string.IsNullOrEmpty(b))throw new ArgumentException("IDs obrigatorios.");if(a==b)throw new ArgumentException("Relacao consigo mesmo invalida.");string x=string.CompareOrdinal(a,b)<0?a:b;string y=string.CompareOrdinal(a,b)<0?b:a;string k=x+"::"+y;RelacaoDiplomatica r;if(relacoes.TryGetValue(k,out r))return r;r=new RelacaoDiplomatica(x,y);relacoes.Add(k,r);return r;}
    }

    public class Main : BasicMod<Main>
    {
        private static EstadoPolitico estadoPolitico;
        private bool uiPendente;
        private bool janelaNativaPronta;
        private int quadrosDesdeCarga;

        private void Update()
        {
            if (!uiPendente) return;
            quadrosDesdeCarga++;
            if (quadrosDesdeCarga % 30 != 0) return;
            try
            {
                if (CanvasMain.instance == null || CanvasMain.instance.canvas_ui == null) return;
                if (!janelaNativaPronta)
                {
                    ObservatorioJanelaNativa.Inicializar();
                    janelaNativaPronta = ObservatorioAtalhoGov.TentarCriar();
                    if (janelaNativaPronta) LogInfo("JANELA_NATIVA_PRONTA: Observatório registrado como janela do WorldBox.");
                }
                else
                {
                    ObservatorioAtalhoGov.TentarCriar();
                }
            }
            catch (Exception ex)
            {
                if (quadrosDesdeCarga % 300 == 0)
                    LogInfo("JANELA_NATIVA_AGUARDANDO: " + ex.GetType().Name + ": " + ex.Message);
            }
        }

        protected override void OnModLoad()
        {
            estadoPolitico=new EstadoPolitico();
            PoliticaDoReino a=estadoPolitico.RegistrarReino("diagnostico-a","Reino de teste A");
            PoliticaDoReino b=estadoPolitico.RegistrarReino("diagnostico-b","Reino de teste B");
            RelacaoDiplomatica rel=estadoPolitico.ObterOuCriarRelacao(a.Id,b.Id);rel.DefinirTipo(TipoRelacaoDiplomatica.Comercio);rel.AjustarOpiniao(10);
            LogInfo("Observatorio dos Reinos carregado. Sistemas politicos locais inicializados.");
            LogInfo("Autoteste politico: reinos="+estadoPolitico.QuantidadeDeReinos+", relacoes="+estadoPolitico.QuantidadeDeRelacoes+", tipo="+rel.Tipo+", opiniao="+rel.Opiniao+".");
            LeitorSeguroDoMundo.Resumo mundo=LeitorSeguroDoMundo.Inspecionar();
            LogInfo("Diagnostico do mundo: mapa="+mundo.MapaEncontrado+", reinos="+mundo.Reinos+", cidades="+mundo.Cidades+", unidades="+mundo.Unidades+".");
            LogInfo("Diagnostico de API (somente leitura): "+mundo.Diagnostico);
            uiPendente = true;
            LogInfo("Inicialização da janela administrativa nativa agendada; nenhuma alteração será aplicada à janela nativa de status.");
            LogInfo("As ações políticas, diplomáticas e de RPG são registros locais da sessão; integração com os reinos nativos continua pendente.");
        }
    }
}
