using System;
using System.Collections;
using System.Collections.Generic;
using System.Reflection;
using HarmonyLib;
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
            if (Instance != null) return;
            CreateAndInit(IdJanela, new Vector2(600f, 420f));
            if (Instance == null) throw new InvalidOperationException("AbstractWideWindow não retornou a instância da janela.");
            Debug.Log("[Observatorio] NATIVE_WINDOW_CREATED: " + IdJanela + " registrada via AbstractWideWindow.");
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
            button.onClick.AddListener(delegate { acao(); });
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
    /// <summary>
    /// Insere o botão de governo no grupo de controles da interface que mostra a seleção/posse de NPCs.
    /// Não cria um botão genérico no canto da tela e não altera a janela de status do jogo.
    /// </summary>
    internal static class ObservatorioBotaoPainelNPC
    {
        private const string IdBotao = "Observatorio_Botao_Governo_NPC";
        private const string IdHarmony = "com.jin27012007.observatoriodosreinos.npc-control-button";
        private const BindingFlags Instancia = BindingFlags.Instance | BindingFlags.Public | BindingFlags.NonPublic;
        private const BindingFlags Estatico = BindingFlags.Static | BindingFlags.Public | BindingFlags.NonPublic;

        private sealed class Registro
        {
            public GameObject Botao;
            public GameObject Pai;
            public Component Painel;
        }

        private static readonly List<Registro> registros = new List<Registro>();
        private static readonly HashSet<MethodBase> metodosComHook = new HashSet<MethodBase>();
        private static readonly HashSet<string> avisosJaEmitidos = new HashSet<string>();
        private static Harmony harmony;
        private static bool inicializacaoTentada;
        private static bool estadoSelecaoConhecido;
        private static bool selecaoAtiva;
        private static int quadros;
        private static int hooksInstalados;

        internal static void Inicializar()
        {
            if (inicializacaoTentada) return;
            inicializacaoTentada = true;

            try
            {
                harmony = new Harmony(IdHarmony);
                int tiposEncontrados = 0;
                int novosHooks = 0;
                Assembly[] assemblies = AppDomain.CurrentDomain.GetAssemblies();

                for (int a = 0; a < assemblies.Length; a++)
                {
                    Type[] tipos = ObterTiposCarregados(assemblies[a]);
                    for (int t = 0; t < tipos.Length; t++)
                    {
                        Type tipo = tipos[t];
                        if (tipo == null || !EhTipoDeControleNPC(tipo.Name)) continue;
                        tiposEncontrados++;

                        MethodInfo update;
                        try
                        {
                            update = tipo.GetMethod("Update", Instancia, null, Type.EmptyTypes, null);
                        }
                        catch (Exception ex)
                        {
                            RegistrarAviso("NPC_CONTROL_UPDATE_LOOKUP_FAILED: " + tipo.FullName + " | " + ex.GetType().Name);
                            continue;
                        }

                        if (update == null)
                        {
                            RegistrarAviso("NPC_CONTROL_UPDATE_NOT_FOUND: " + tipo.FullName);
                            continue;
                        }

                        if (metodosComHook.Contains(update)) continue;
                        try
                        {
                            MethodInfo postfix = typeof(ObservatorioBotaoPainelNPC).GetMethod(
                                "DepoisDoUpdateDoPainel", Estatico);
                            harmony.Patch(update, null, new HarmonyMethod(postfix));
                            metodosComHook.Add(update);
                            novosHooks++;
                            hooksInstalados++;
                            Debug.LogWarning("[Observatorio] NPC_CONTROL_HOOK_INSTALLED: " + tipo.FullName + ".Update()");
                        }
                        catch (Exception ex)
                        {
                            RegistrarAviso("NPC_CONTROL_HOOK_FAILED: " + tipo.FullName + " | " + ex.GetType().Name + ": " + ex.Message);
                        }
                    }
                }

                Debug.LogWarning("[Observatorio] NPC_CONTROL_HOOK_SUMMARY: assemblies=" + assemblies.Length
                    + " | tipos=" + tiposEncontrados + " | novos_hooks=" + novosHooks
                    + " | total_hooks=" + hooksInstalados + " | build=0.6.4-dev");

                if (tiposEncontrados == 0)
                    RegistrarAviso("NPC_CONTROL_TYPE_NOT_FOUND: nenhuma classe SelectedPowerTab*Buttons foi encontrada nas assemblies carregadas.");
                else if (hooksInstalados == 0)
                    RegistrarAviso("NPC_CONTROL_NO_UPDATE_HOOK: as classes de controle existem, mas nenhum Update foi conectado.");
            }
            catch (Exception ex)
            {
                RegistrarAviso("NPC_CONTROL_INIT_FAILED: " + ex.GetType().Name + ": " + ex.Message);
            }
        }

        private static Type[] ObterTiposCarregados(Assembly assembly)
        {
            try
            {
                return assembly.GetTypes();
            }
            catch (ReflectionTypeLoadException ex)
            {
                List<Type> recuperados = new List<Type>();
                Type[] tipos = ex.Types;
                for (int i = 0; i < tipos.Length; i++)
                    if (tipos[i] != null) recuperados.Add(tipos[i]);
                return recuperados.ToArray();
            }
            catch
            {
                return new Type[0];
            }
        }

        private static bool EhTipoDeControleNPC(string nome)
        {
            if (string.IsNullOrEmpty(nome)) return false;
            if (string.Equals(nome, "SelectedPowerTabButtons", StringComparison.Ordinal)) return true;
            if (string.Equals(nome, "SelectedPowerTabTopButtons", StringComparison.Ordinal)) return true;
            return nome.StartsWith("SelectedPowerTab", StringComparison.Ordinal)
                && nome.EndsWith("Buttons", StringComparison.OrdinalIgnoreCase);
        }

        // Chamado depois do Update do painel real, não por um Canvas genérico da tela.
        private static void DepoisDoUpdateDoPainel(object __instance)
        {
            try
            {
                Component painel = __instance as Component;
                if (painel == null || painel.gameObject == null) return;

                AtualizarEstadoSelecao();
                TentarInserirNoPainel(painel);
            }
            catch (Exception ex)
            {
                RegistrarAviso("NPC_CONTROL_POSTFIX_FAILED: " + ex.GetType().Name + ": " + ex.Message);
            }
        }

        internal static void Atualizar()
        {
            quadros++;
            if (!inicializacaoTentada || (hooksInstalados == 0 && quadros % 120 == 0))
            {
                inicializacaoTentada = false;
                Inicializar();
            }

            if (quadros % 4 == 0)
            {
                AtualizarEstadoSelecao();
                AtualizarVisibilidade();
            }

            // Fallback caso a versão do jogo não chame o método esperado pelo hook.
            if (quadros % 90 == 0 && !ExisteBotaoRegistrado())
                ProcurarPainelNaCena();
        }

        private static void AtualizarEstadoSelecao()
        {
            bool unidadeOk;
            bool unidade = LerFlagEstatica("SelectedUnit", "isSet", out unidadeOk);
            bool objetoOk;
            bool objeto = LerFlagEstatica("SelectedObjects", "isNanoObjectSet", out objetoOk);
            estadoSelecaoConhecido = unidadeOk || objetoOk;
            selecaoAtiva = (unidadeOk && unidade) || (objetoOk && objeto);
        }

        private static bool LerFlagEstatica(string nomeTipo, string nomeMetodo, out bool conhecido)
        {
            conhecido = false;
            try
            {
                Type tipo = ResolverTipo(nomeTipo);
                if (tipo == null) return false;
                MethodInfo metodo = tipo.GetMethod(nomeMetodo, Estatico, null, Type.EmptyTypes, null);
                if (metodo == null) return false;

                object valor = metodo.Invoke(null, null);
                if (!(valor is bool)) return false;
                conhecido = true;
                return (bool)valor;
            }
            catch (Exception ex)
            {
                RegistrarAviso("NPC_SELECTION_READ_FAILED: " + nomeTipo + "." + nomeMetodo + " | " + ex.GetType().Name);
                return false;
            }
        }

        private static Type ResolverTipo(string nomeTipo)
        {
            Assembly[] assemblies = AppDomain.CurrentDomain.GetAssemblies();
            for (int i = 0; i < assemblies.Length; i++)
            {
                try
                {
                    Type tipo = assemblies[i].GetType(nomeTipo, false);
                    if (tipo != null) return tipo;
                }
                catch { }
            }
            return null;
        }

        private static bool ExisteBotaoRegistrado()
        {
            for (int i = registros.Count - 1; i >= 0; i--)
            {
                if (registros[i] != null && registros[i].Botao != null && registros[i].Pai != null)
                    return true;
                registros.RemoveAt(i);
            }
            return false;
        }

        private static void ProcurarPainelNaCena()
        {
            int encontrados = 0;
            CanvasMain main = CanvasMain.instance;

            if (main != null && main.canvas_ui != null)
            {
                try
                {
                    MonoBehaviour[] componentes = main.canvas_ui.GetComponentsInChildren<MonoBehaviour>(true);
                    for (int i = 0; i < componentes.Length; i++)
                    {
                        Component componente = componentes[i] as Component;
                        if (componente == null || !EhTipoDeControleNPC(componente.GetType().Name)) continue;
                        encontrados++;
                        TentarInserirNoPainel(componente);
                    }
                }
                catch (Exception ex)
                {
                    RegistrarAviso("NPC_CONTROL_CANVAS_SCAN_FAILED: " + ex.GetType().Name + ": " + ex.Message);
                }
            }

            string[] nomes = { "SelectedPowerTabButtons", "SelectedPowerTabTopButtons" };
            for (int n = 0; n < nomes.Length; n++)
            {
                Type tipo = ResolverTipo(nomes[n]);
                if (tipo == null) continue;
                UnityEngine.Object[] objetos;
                try { objetos = Resources.FindObjectsOfTypeAll(tipo); }
                catch (Exception ex)
                {
                    RegistrarAviso("NPC_CONTROL_RESOURCE_SCAN_FAILED: " + ex.GetType().Name);
                    continue;
                }

                for (int i = 0; i < objetos.Length; i++)
                {
                    Component componente = objetos[i] as Component;
                    if (componente == null || componente.gameObject == null) continue;
                    try
                    {
                        // Não alterar prefabs de recursos: só objetos ligados a uma cena carregada.
                        if (!componente.gameObject.scene.IsValid()) continue;
                    }
                    catch { continue; }
                    encontrados++;
                    TentarInserirNoPainel(componente);
                }
            }

            if (encontrados == 0)
            {
                RegistrarAviso("NPC_CONTROL_PANEL_NOT_FOUND: nenhum componente de controle foi localizado; CanvasMain="
                    + (main != null) + " | canvas_ui=" + (main != null && main.canvas_ui != null)
                    + " | hooks=" + hooksInstalados);
            }
        }

        private static bool TentarInserirNoPainel(Component painel)
        {
            if (painel == null || painel.gameObject == null || !EhTipoDeControleNPC(painel.GetType().Name))
                return false;

            GameObject referencia;
            string nomeCampo;
            LocalizarBotaoReferencia(painel, out referencia, out nomeCampo);

            Transform pai = referencia != null && referencia.transform.parent != null
                ? referencia.transform.parent
                : painel.transform;
            if (pai == null) return false;

            Transform existente = pai.Find(IdBotao);
            if (existente != null)
            {
                RegistrarRegistro(existente.gameObject, pai.gameObject, painel);
                return true;
            }

            // Mesmo se a versão renomear os campos dos botões, GOV fica dentro do componente
            // específico dos controles de NPC, com um fallback visível na raiz desse painel.
            if (referencia == null)
            {
                nomeCampo = "fallback:raiz-do-painel";
                RegistrarAviso("NPC_CONTROL_REFERENCE_FALLBACK: " + painel.GetType().FullName
                    + " | " + ResumirBotoes(painel));
            }

            GameObject novo = null;
            try
            {
                RectTransform modelo = referencia == null ? null : referencia.GetComponent<RectTransform>();
                float largura = modelo == null ? 66f : modelo.rect.width;
                float altura = modelo == null ? 40f : modelo.rect.height;
                string estrategiaPosicao = modelo == null ? "fallback-raiz" : "offset-calculado";
                if (largura < 24f) largura = modelo == null ? 66f : modelo.sizeDelta.x;
                if (altura < 24f) altura = modelo == null ? 40f : modelo.sizeDelta.y;
                if (largura < 24f) largura = 66f;
                if (altura < 24f) altura = 40f;

                novo = new GameObject(IdBotao,
                    typeof(RectTransform), typeof(Image), typeof(Button), typeof(LayoutElement));
                novo.transform.SetParent(pai, false);

                RectTransform rect = novo.GetComponent<RectTransform>();
                if (modelo != null)
                {
                    rect.anchorMin = modelo.anchorMin;
                    rect.anchorMax = modelo.anchorMax;
                    rect.pivot = modelo.pivot;
                    rect.localScale = modelo.localScale;
                    rect.sizeDelta = modelo.sizeDelta;
                }
                else
                {
                    rect.anchorMin = new Vector2(1f, 0.5f);
                    rect.anchorMax = new Vector2(1f, 0.5f);
                    rect.pivot = new Vector2(1f, 0.5f);
                    rect.anchoredPosition = new Vector2(-18f, 0f);
                }
                rect.sizeDelta = new Vector2(Math.Max(largura, 60f), altura);

                Image imagem = novo.GetComponent<Image>();
                Image imagemModelo = referencia == null ? null : referencia.GetComponent<Image>();
                if (imagemModelo == null && referencia != null)
                    imagemModelo = referencia.GetComponentInChildren<Image>(true);
                if (imagemModelo != null)
                {
                    imagem.sprite = imagemModelo.sprite;
                    imagem.type = imagemModelo.type;
                    imagem.material = imagemModelo.material;
                    imagem.preserveAspect = imagemModelo.preserveAspect;
                    imagem.color = imagemModelo.color;
                }
                else
                {
                    imagem.color = new Color(0.13f, 0.30f, 0.17f, 1f);
                }

                Button button = novo.GetComponent<Button>();
                button.targetGraphic = imagem;
                button.transition = Selectable.Transition.ColorTint;
                ColorBlock cores = button.colors;
                cores.normalColor = imagem.color;
                cores.highlightedColor = new Color(0.32f, 0.48f, 0.31f, 1f);
                cores.pressedColor = new Color(0.68f, 0.58f, 0.32f, 1f);
                button.colors = cores;
                button.onClick.AddListener(AbrirObservatorio);

                GameObject textoObj = new GameObject("Texto_GOV", typeof(RectTransform), typeof(Text));
                textoObj.transform.SetParent(novo.transform, false);
                RectTransform textoRect = textoObj.GetComponent<RectTransform>();
                textoRect.anchorMin = Vector2.zero;
                textoRect.anchorMax = Vector2.one;
                textoRect.offsetMin = new Vector2(2f, 1f);
                textoRect.offsetMax = new Vector2(-2f, -1f);

                Text texto = textoObj.GetComponent<Text>();
                Text textoModelo = referencia == null ? null : referencia.GetComponentInChildren<Text>(true);
                try
                {
                    texto.font = textoModelo != null
                        ? textoModelo.font
                        : Resources.GetBuiltinResource<Font>("Arial.ttf");
                    if (texto.font == null)
                        texto.font = Resources.GetBuiltinResource<Font>("LegacyRuntime.ttf");
                }
                catch (Exception fontEx)
                {
                    RegistrarAviso("NPC_CONTROL_FONT_FALLBACK: " + fontEx.GetType().Name);
                }
                texto.fontSize = textoModelo == null ? 12 : Math.Max(10, textoModelo.fontSize - 2);
                texto.fontStyle = FontStyle.Bold;
                texto.alignment = TextAnchor.MiddleCenter;
                texto.color = new Color(1f, 0.91f, 0.53f, 1f);
                texto.text = "GOV";
                texto.supportRichText = false;

                HorizontalLayoutGroup horizontal = pai.GetComponent<HorizontalLayoutGroup>();
                VerticalLayoutGroup vertical = pai.GetComponent<VerticalLayoutGroup>();
                if (horizontal != null || vertical != null)
                {
                    LayoutElement layout = novo.GetComponent<LayoutElement>();
                    layout.minWidth = Math.Max(largura, 60f);
                    layout.preferredWidth = Math.Max(largura, 60f);
                    layout.minHeight = altura;
                    layout.preferredHeight = altura;
                    if (referencia != null)
                        novo.transform.SetSiblingIndex(referencia.transform.GetSiblingIndex() + 1);
                    else
                        novo.transform.SetAsLastSibling();

                    estrategiaPosicao = horizontal != null && vertical != null
                        ? "layout-horizontal+vertical"
                        : horizontal != null ? "layout-horizontal" : "layout-vertical";
                }
                else if (modelo != null)
                {
                    // Em ancoragem à direita, deslocar para a direita pode tirar o botão da tela.
                    // Coloque o novo botão para dentro do painel; em âncoras à esquerda/centro, para a direita.
                    bool ancoraDireita = modelo.anchorMin.x >= 0.5f && modelo.anchorMax.x >= 0.5f;
                    float direcaoX = ancoraDireita ? -1f : 1f;
                    rect.anchoredPosition = modelo.anchoredPosition
                        + new Vector2(direcaoX * (largura + 6f), 0f);
                    novo.transform.SetAsLastSibling();
                    estrategiaPosicao = ancoraDireita
                        ? "offset-esquerda-ancora-direita"
                        : "offset-direita-ancora-esquerda-centro";
                }
                else
                {
                    novo.transform.SetAsLastSibling();
                    estrategiaPosicao = "fallback-raiz-ancora-direita";
                }

                string origemVisibilidade;
                bool contextoAtivo = ContextoDosControlesAtivo(painel, out origemVisibilidade);
                novo.SetActive(contextoAtivo && painel.gameObject.activeInHierarchy && pai.gameObject.activeInHierarchy);
                RegistrarRegistro(novo, pai.gameObject, painel);

                RectTransform paiRect = pai as RectTransform;
                Debug.LogWarning("[Observatorio] NATIVE_CONTROL_BUTTON_CREATED: GOV | painel=" + painel.GetType().FullName
                    + " | referência=" + nomeCampo + " | pai=" + pai.gameObject.name
                    + " | estratégia=" + estrategiaPosicao
                    + " | anchorMin=" + rect.anchorMin + " | anchorMax=" + rect.anchorMax
                    + " | pivot=" + rect.pivot + " | anchoredPosition=" + rect.anchoredPosition
                    + " | sizeDelta=" + rect.sizeDelta + " | parentSize=" + (paiRect == null ? "(not-rect)" : paiRect.rect.size.ToString())
                    + " | seleção_conhecida=" + estadoSelecaoConhecido + " | seleção_ativa=" + selecaoAtiva
                    + " | visibilidade_origem=" + origemVisibilidade
                    + " | activeSelf=" + novo.activeSelf + " | activeInHierarchy=" + novo.activeInHierarchy
                    + " | parent_active=" + pai.gameObject.activeInHierarchy);
                return true;
            }
            catch (Exception ex)
            {
                if (novo != null) UnityEngine.Object.Destroy(novo);
                RegistrarAviso("NATIVE_CONTROL_BUTTON_FAILED: " + painel.GetType().FullName
                    + " | " + ex.GetType().Name + ": " + ex.Message + " | " + ResumirBotoes(painel));
                return false;
            }
        }

        private static void LocalizarBotaoReferencia(Component painel, out GameObject referencia, out string nomeCampo)
        {
            referencia = null;
            nomeCampo = null;
            Type tipo = painel.GetType();
            string[] prioritarios =
            {
                "possession", "possess", "spectate", "open_window", "openwindow",
                "open_main", "openmain", "main_info", "button_info", "favorite"
            };

            for (int p = 0; p < prioritarios.Length; p++)
            {
                for (Type atual = tipo; atual != null; atual = atual.BaseType)
                {
                    FieldInfo[] campos;
                    try { campos = atual.GetFields(Instancia | BindingFlags.DeclaredOnly); }
                    catch { continue; }

                    for (int i = 0; i < campos.Length; i++)
                    {
                        if (campos[i].Name.IndexOf(prioritarios[p], StringComparison.OrdinalIgnoreCase) < 0)
                            continue;
                        try
                        {
                            GameObject go = ConverterParaGameObject(campos[i].GetValue(painel));
                            if (go == null || go == painel.gameObject) continue;
                            referencia = go;
                            nomeCampo = campos[i].Name;
                            return;
                        }
                        catch { }
                    }
                }
            }

            Button[] botoes;
            try { botoes = painel.GetComponentsInChildren<Button>(true); }
            catch { botoes = new Button[0]; }

            for (int i = 0; i < botoes.Length; i++)
            {
                if (botoes[i] == null || botoes[i].gameObject == null) continue;
                string nome = botoes[i].gameObject.name.ToLowerInvariant();
                if (nome.Contains("possession") || nome.Contains("possess") ||
                    nome.Contains("spectate") || nome.Contains("open_window") ||
                    nome.Contains("open_main") || nome.Contains("info"))
                {
                    referencia = botoes[i].gameObject;
                    nomeCampo = "child:" + botoes[i].gameObject.name;
                    return;
                }
            }

            // Se os campos foram renomeados, use outra ação clicável do mesmo painel como âncora.
            for (int i = 0; i < botoes.Length; i++)
            {
                if (botoes[i] == null || botoes[i].gameObject == null) continue;
                if (botoes[i].gameObject.name == IdBotao) continue;
                referencia = botoes[i].gameObject;
                nomeCampo = "fallback:first-button:" + botoes[i].gameObject.name;
                return;
            }
        }

        private static GameObject ConverterParaGameObject(object valor)
        {
            GameObject go = valor as GameObject;
            if (go != null) return go;
            Component componente = valor as Component;
            return componente == null ? null : componente.gameObject;
        }

        // A seleção lógica pode divergir dos flags SelectedUnit/SelectedObjects.
        // Para visibilidade, use o estado dos controles nativos que já compõem este painel.
        private static bool ContextoDosControlesAtivo(Component painel, out string origem)
        {
            if (painel == null || painel.gameObject == null)
            {
                origem = "painel-indisponivel";
                return false;
            }

            bool controlesConhecidos;
            bool controleAtivo = AlgumControleNativoAtivo(painel, out controlesConhecidos);
            if (controlesConhecidos)
            {
                origem = "controles-nativos";
                return controleAtivo;
            }

            origem = "fallback-painel";
            return painel.gameObject.activeInHierarchy;
        }

        private static bool AlgumControleNativoAtivo(Component painel, out bool controlesConhecidos)
        {
            controlesConhecidos = false;
            if (painel == null) return false;

            string[] camposNativos =
            {
                "_button_possession", "_button_spectate", "_button_open_window", "_button_favorite"
            };

            for (Type atual = painel.GetType(); atual != null; atual = atual.BaseType)
            {
                for (int i = 0; i < camposNativos.Length; i++)
                {
                    FieldInfo campo = null;
                    try
                    {
                        campo = atual.GetField(camposNativos[i], Instancia | BindingFlags.DeclaredOnly);
                    }
                    catch { }

                    if (campo == null) continue;
                    try
                    {
                        GameObject go = ConverterParaGameObject(campo.GetValue(painel));
                        if (go == null || go == painel.gameObject) continue;
                        controlesConhecidos = true;
                        if (go.activeInHierarchy) return true;
                    }
                    catch { }
                }
            }

            // Fallback pelos nomes dos botões filhos, se os campos privados mudarem.
            Button[] botoes;
            try { botoes = painel.GetComponentsInChildren<Button>(true); }
            catch { botoes = new Button[0]; }

            for (int i = 0; i < botoes.Length; i++)
            {
                if (botoes[i] == null || botoes[i].gameObject == null) continue;
                string nome = botoes[i].gameObject.name.ToLowerInvariant();
                bool nativo = nome.Contains("possession") || nome.Contains("possess")
                    || nome.Contains("spectate") || nome.Contains("open_window")
                    || nome.Contains("open_main") || nome.Contains("main_info")
                    || nome.Contains("favorite") || nome.Contains("info");
                if (!nativo) continue;

                controlesConhecidos = true;
                if (botoes[i].gameObject.activeInHierarchy) return true;
            }

            return false;
        }

        private static void RegistrarRegistro(GameObject botao, GameObject pai, Component painel)
        {
            for (int i = 0; i < registros.Count; i++)
            {
                if (registros[i] != null && registros[i].Botao == botao)
                {
                    registros[i].Pai = pai;
                    registros[i].Painel = painel;
                    return;
                }
            }
            registros.Add(new Registro { Botao = botao, Pai = pai, Painel = painel });
        }

        private static void AtualizarVisibilidade()
        {
            for (int i = registros.Count - 1; i >= 0; i--)
            {
                Registro item = registros[i];
                if (item == null || item.Botao == null || item.Pai == null || item.Painel == null)
                {
                    registros.RemoveAt(i);
                    continue;
                }

                string origemVisibilidade;
                bool contextoAtivo = ContextoDosControlesAtivo(item.Painel, out origemVisibilidade);
                bool ativo = contextoAtivo
                    && item.Painel.gameObject.activeInHierarchy
                    && item.Pai.activeInHierarchy;
                if (item.Botao.activeSelf != ativo)
                    item.Botao.SetActive(ativo);
            }
        }

        private static string ResumirBotoes(Component painel)
        {
            try
            {
                Button[] botoes = painel.GetComponentsInChildren<Button>(true);
                string resumo = "buttons=" + botoes.Length;
                int limite = Math.Min(botoes.Length, 8);
                for (int i = 0; i < limite; i++)
                {
                    if (botoes[i] == null || botoes[i].gameObject == null) continue;
                    resumo += " | " + botoes[i].gameObject.name;
                }
                if (botoes.Length > limite) resumo += " | +" + (botoes.Length - limite);
                return resumo;
            }
            catch (Exception ex)
            {
                return "hierarchy_error=" + ex.GetType().Name;
            }
        }

        private static void AbrirObservatorio()
        {
            try
            {
                Debug.LogWarning("[Observatorio] NPC_CONTROL_BUTTON_CLICKED: abrindo janela administrativa.");
                ObservatorioJanelaNativa.Abrir();
            }
            catch (Exception ex)
            {
                Debug.LogError("[Observatorio] NPC_CONTROL_BUTTON_OPEN_FAILED: " + ex.GetType().Name + ": " + ex.Message);
            }
        }

        private static void RegistrarAviso(string mensagem)
        {
            if (string.IsNullOrEmpty(mensagem) || !avisosJaEmitidos.Add(mensagem)) return;
            Debug.LogWarning("[Observatorio] " + mensagem);
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
        private const string BuildVersion = "0.6.4-dev";
        private const BindingFlags BaselineFlags = BindingFlags.Public | BindingFlags.NonPublic | BindingFlags.Instance | BindingFlags.Static;
        private static EstadoPolitico estadoPolitico;
        private bool uiPendente;
        private bool janelaNativaPronta;
        private bool tentouInicializarJanela;
        private bool baselineUpdateReported;
        private int quadrosDesdeCarga;

        private void Update()
        {
            if (!baselineUpdateReported)
            {
                baselineUpdateReported = true;
                LogInfo("[OR-BASELINE] BUILD=" + BuildVersion + "; STAGE=UPDATE_OK");
            }

            ObservatorioBotaoPainelNPC.Atualizar();

            if (!uiPendente) return;
            quadrosDesdeCarga++;
            if (quadrosDesdeCarga % 30 != 0) return;
            if (janelaNativaPronta) return;
            if (tentouInicializarJanela && quadrosDesdeCarga % 300 != 0) return;

            if (CanvasMain.instance == null || CanvasMain.instance.canvas_ui == null)
            {
                if (quadrosDesdeCarga % 300 == 0)
                    LogInfo("JANELA_NATIVA_AGUARDANDO: CanvasMain/canvas_ui ainda não está pronto.");
                return;
            }

            tentouInicializarJanela = true;
            try
            {
                if (ObservatorioJanelaNativa.Instance == null)
                    ObservatorioJanelaNativa.Inicializar();

                janelaNativaPronta = ObservatorioJanelaNativa.Instance != null;
                if (janelaNativaPronta)
                {
                    LogInfo("JANELA_NATIVA_PRONTA: janela administrativa registrada. Build=0.6.4-dev.");
                }
                else
                {
                    LogInfo("JANELA_NATIVA_FALHOU: CreateAndInit retornou sem uma instância.");
                }
            }
            catch (Exception ex)
            {
                LogInfo("JANELA_NATIVA_FALHOU: " + ex.GetType().Name + ": " + ex.Message);
            }
        }

        private void LogModDeclarationBaseline()
        {
            try
            {
                var declaration = GetDeclaration();
                if (declaration == null)
                {
                    LogInfo("[OR-BASELINE] DECLARATION=null");
                    return;
                }

                LogInfo("[OR-BASELINE] DECLARATION name=" + declaration.Name
                    + "; uid=" + declaration.UID
                    + "; folder=" + declaration.FolderPath);
            }
            catch (Exception ex)
            {
                LogInfo("[OR-BASELINE] DECLARATION_ERROR=" + ex.GetType().Name + ": " + ex.Message);
            }
        }

        private void LogAssemblyBaseline()
        {
            string[] expected = { "Assembly-CSharp", "NeoModLoader", "UnityEngine.CoreModule", "UnityEngine.UI", "0Harmony" };
            Assembly[] loaded = AppDomain.CurrentDomain.GetAssemblies();
            for (int n = 0; n < expected.Length; n++)
            {
                bool found = false;
                for (int i = 0; i < loaded.Length; i++)
                {
                    try
                    {
                        AssemblyName identity = loaded[i].GetName();
                        if (!string.Equals(identity.Name, expected[n], StringComparison.OrdinalIgnoreCase)
                            && !(expected[n] == "0Harmony" && identity.Name.IndexOf("Harmony", StringComparison.OrdinalIgnoreCase) >= 0))
                            continue;

                        found = true;
                        string mvid = "(unavailable)";
                        try { mvid = loaded[i].ManifestModule.ModuleVersionId.ToString("N"); } catch { }
                        LogInfo("[OR-BASELINE] ASSEMBLY name=" + identity.Name
                            + "; version=" + (identity.Version == null ? "(null)" : identity.Version.ToString())
                            + "; mvid=" + mvid);
                        break;
                    }
                    catch { }
                }
                if (!found) LogInfo("[OR-BASELINE] ASSEMBLY name=" + expected[n] + "; found=false");
            }
        }

        private static Type ResolveBaselineType(string name)
        {
            Assembly[] loaded = AppDomain.CurrentDomain.GetAssemblies();
            for (int i = 0; i < loaded.Length; i++)
            {
                try
                {
                    Type found = loaded[i].GetType(name, false);
                    if (found != null) return found;
                }
                catch { }
            }
            return null;
        }

        private void LogNpcUiContractBaseline()
        {
            Type controls = ResolveBaselineType("SelectedPowerTabTopButtons");
            LogInfo("[OR-BASELINE] TYPE_PROBE name=SelectedPowerTabTopButtons; found=" + (controls != null));
            if (controls != null)
            {
                MethodInfo update = null;
                try { update = controls.GetMethod("Update", BaselineFlags, null, Type.EmptyTypes, null); } catch { }
                LogInfo("[OR-BASELINE] METHOD_PROBE target=SelectedPowerTabTopButtons.Update; found=" + (update != null)
                    + (update == null ? "" : "; visibility=" + (update.IsPublic ? "public" : update.IsFamily ? "protected" : update.IsPrivate ? "private" : "other")));

                string[] interestingFields = { "_button_possession", "_button_spectate", "_button_open_window" };
                for (int f = 0; f < interestingFields.Length; f++)
                {
                    FieldInfo field = null;
                    for (Type current = controls; current != null && field == null; current = current.BaseType)
                    {
                        try { field = current.GetField(interestingFields[f], BaselineFlags | BindingFlags.DeclaredOnly); } catch { }
                    }
                    LogInfo("[OR-BASELINE] FIELD_PROBE name=" + interestingFields[f]
                        + "; found=" + (field != null)
                        + (field == null ? "" : "; type=" + field.FieldType.FullName));
                }
            }

            Type controller = ResolveBaselineType("PowerTabController");
            LogInfo("[OR-BASELINE] TYPE_PROBE name=PowerTabController; found=" + (controller != null));
            if (controller != null)
            {
                MethodInfo method = null;
                try { method = controller.GetMethod("showWorldTipSelected", BaselineFlags, null, new Type[] { typeof(string), typeof(bool) }, null); } catch { }
                LogInfo("[OR-BASELINE] METHOD_PROBE target=PowerTabController.showWorldTipSelected(String, Boolean); found=" + (method != null));
            }
        }

        protected override void OnModLoad()
        {
            LogInfo("[OR-BASELINE] BUILD=" + BuildVersion + "; STAGE=ON_MOD_LOAD");
            LogModDeclarationBaseline();
            LogAssemblyBaseline();
            LogNpcUiContractBaseline();

            estadoPolitico=new EstadoPolitico();
            PoliticaDoReino a=estadoPolitico.RegistrarReino("diagnostico-a","Reino de teste A");
            PoliticaDoReino b=estadoPolitico.RegistrarReino("diagnostico-b","Reino de teste B");
            RelacaoDiplomatica rel=estadoPolitico.ObterOuCriarRelacao(a.Id,b.Id);rel.DefinirTipo(TipoRelacaoDiplomatica.Comercio);rel.AjustarOpiniao(10);
            LogInfo("Observatorio dos Reinos carregado. Build=0.6.4-dev.");
            ObservatorioBotaoPainelNPC.Inicializar();
            LogInfo("Integração com controles de NPC iniciada; consulte NPC_CONTROL_HOOK_SUMMARY.");
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
