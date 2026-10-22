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
    /// UI real de Canvas, nao IMGUI. O botao e inserido acima do botao nativo de
    /// informacoes quando encontrado; se a hierarquia variar, existe um botao reserva.
    /// </summary>
    public sealed class MenuAdministracaoOverlay : MonoBehaviour
    {
        private static MenuAdministracaoOverlay instancia;
        private const string IdBotao = "Observatorio_Admin_Button";
        private Canvas canvas;
        private RectTransform painel;
        private GameObject faixaAlternativa;
        private Text textoEstado;
        private InputField campoLei, campoTratado, campoReuniao, campoEvento;
        private GameObject[] paginas = new GameObject[4];
        private int aba;
        private int frames;
        private Button botaoNativoClonado;
        private Component donoBotaoNativo;
        private bool patchInstalado;
        private Sprite spriteJanela;
        private Font fonte;
        private Text textoDiagnostico;
        private Rect lastSafeArea;
        private int diagnosticoFrames;
        private bool tentouAvisarFalha;


        public static void CriarSeNecessario()
        {
            if (instancia != null) return;
            MenuAdministracaoOverlay encontrada = FindObjectOfType<MenuAdministracaoOverlay>();
            if (encontrada != null) { instancia=encontrada; return; }
            GameObject raiz = new GameObject("ObservatorioDosReinos_RuntimeUI");
            DontDestroyOnLoad(raiz);
            instancia=raiz.AddComponent<MenuAdministracaoOverlay>();
        }

        public static void Abrir()
        {
            CriarSeNecessario();
            if (instancia != null) instancia.DefinirAberto(true);
        }

        public static void Fechar()
        {
            if (instancia != null) instancia.DefinirAberto(false);
        }

        private void Awake()
        {
            if (instancia != null && instancia != this) { Destroy(gameObject); return; }
            instancia=this;
            DontDestroyOnLoad(gameObject);
            fonte=Resources.GetBuiltinResource<Font>("Arial.ttf");
            spriteJanela=BuscarSpriteJanela();
            ConstruirCanvas();
            InstalarProtecaoStatus();
        }

        private void Update()
        {
            frames++;
            if (frames % 25 == 0) TentarInserirBotaoNativo();
            if (frames % 45 == 0)
            {
                string safe = Screen.safeArea.width.ToString("0") + "x" + Screen.safeArea.height.ToString("0");
                if (textoDiagnostico != null)
                    textoDiagnostico.text = "OBSERVATORIO 0.5.1-dev | tela " + Screen.width + "x" + Screen.height
                        + " | area segura " + safe + " | Canvas ativo=" + (canvas != null && canvas.gameObject.activeInHierarchy)
                        + " | botao nativo=" + (botaoNativoClonado != null);
            }
        }

        private void ConstruirCanvas()
        {
            GameObject go = new GameObject("Observatorio_Admin_Canvas", typeof(RectTransform), typeof(Canvas), typeof(CanvasScaler), typeof(GraphicRaycaster));
            go.transform.SetParent(transform, false);
            canvas=go.GetComponent<Canvas>();
            canvas.renderMode=RenderMode.ScreenSpaceOverlay;
            canvas.sortingOrder=32000;
            CanvasScaler scaler=go.GetComponent<CanvasScaler>();
            scaler.uiScaleMode=CanvasScaler.ScaleMode.ScaleWithScreenSize;
            scaler.referenceResolution=new Vector2(720f,1280f);
            scaler.matchWidthOrHeight=0.5f;
            go.AddComponent<CanvasSafeAreaLayout>();

            GameObject fallback=NovoUI("Atalho reserva Governo",go.transform,typeof(Image),typeof(Button));
            RectTransform fr=fallback.GetComponent<RectTransform>();
            fr.anchorMin=new Vector2(0.02f,0.87f); fr.anchorMax=new Vector2(0.32f,0.93f);
            fr.offsetMin=Vector2.zero; fr.offsetMax=Vector2.zero;
            Image fi=fallback.GetComponent<Image>(); fi.color=new Color(0.23f,0.25f,0.21f,0.98f);
            Button fb=fallback.GetComponent<Button>(); fb.targetGraphic=fi; fb.onClick.AddListener(delegate { Abrir(); });
            AdicionarBordaDourada(fallback);
            TextoFilho(fallback.transform,"GOVERNO",15,FontStyle.Bold,new Color(1f,0.72f,0.28f));
            faixaAlternativa=fallback;

            GameObject diag=NovoUI("Diagnostico visivel",go.transform,typeof(Image));
            RectTransform dr=diag.GetComponent<RectTransform>();
            dr.anchorMin=new Vector2(0.02f,0.80f);dr.anchorMax=new Vector2(0.98f,0.86f);
            dr.offsetMin=Vector2.zero;dr.offsetMax=Vector2.zero;
            diag.GetComponent<Image>().color=new Color(0.12f,0.13f,0.11f,0.96f);
            textoDiagnostico=TextoFilho(diag.transform,"Observatorio carregado: inicializando interface...",12,FontStyle.Bold,new Color(1f,0.76f,0.30f));
            textoDiagnostico.alignment=TextAnchor.MiddleLeft;

            GameObject p=NovoUI("Painel de administracao",go.transform,typeof(Image));
            painel=p.GetComponent<RectTransform>();
            painel.anchorMin=new Vector2(0.045f,0.045f); painel.anchorMax=new Vector2(0.955f,0.955f);
            painel.offsetMin=Vector2.zero; painel.offsetMax=Vector2.zero; painel.pivot=new Vector2(0.5f,0.5f);
            Image pi=p.GetComponent<Image>(); pi.color=new Color(0.24f,0.26f,0.22f,0.99f);
            if(spriteJanela!=null){pi.sprite=spriteJanela;pi.type=spriteJanela.border.sqrMagnitude>0.01f?Image.Type.Sliced:Image.Type.Simple;pi.preserveAspect=false;}

            GameObject topo=NovoUI("Barra de titulo",p.transform,typeof(Image));
            RectTransform tr=topo.GetComponent<RectTransform>(); Anchors(tr,new Vector2(0f,0.91f),new Vector2(1f,1f),new Vector2(8f,-4f),new Vector2(-8f,-4f));
            topo.GetComponent<Image>().color=new Color(0.13f,0.15f,0.13f,0.95f);
            TextoFilho(topo.transform,"OBSERVATORIO DOS REINOS",18,FontStyle.Bold,new Color(1f,0.68f,0.23f)).alignment=TextAnchor.MiddleLeft;
            Button fechar=Botao(topo.transform,"X",new Color(0.58f,0.16f,0.12f,1f),42f);
            RectTransform fecharR=fechar.GetComponent<RectTransform>(); fecharR.anchorMin=new Vector2(1f,0.08f); fecharR.anchorMax=new Vector2(1f,0.92f); fecharR.pivot=new Vector2(1f,0.5f); fecharR.sizeDelta=new Vector2(44f,0f); fecharR.anchoredPosition=Vector2.zero;
            fechar.onClick.AddListener(delegate { DefinirAberto(false); });

            GameObject nav=NovoUI("Navegacao",p.transform,typeof(HorizontalLayoutGroup));
            RectTransform nr=nav.GetComponent<RectTransform>(); Anchors(nr,new Vector2(0.025f,0.815f),new Vector2(0.975f,0.90f),Vector2.zero,Vector2.zero);
            HorizontalLayoutGroup hg=nav.GetComponent<HorizontalLayoutGroup>(); hg.spacing=5f; hg.childControlWidth=true; hg.childForceExpandWidth=true; hg.childControlHeight=true; hg.childForceExpandHeight=true;
            string[] nomes={"Governo","Diplomacia","Reunioes","RPG"};
            for(int i=0;i<4;i++){int j=i;Button b=Botao(nav.transform,nomes[i],new Color(0.31f,0.33f,0.28f,1f),46f);b.onClick.AddListener(delegate{TrocarAba(j);});}

            GameObject estado=NovoUI("Estado da acao",p.transform,typeof(Image));
            RectTransform er=estado.GetComponent<RectTransform>(); Anchors(er,new Vector2(0.025f,0.055f),new Vector2(0.975f,0.13f),Vector2.zero,Vector2.zero);
            estado.GetComponent<Image>().color=new Color(0.13f,0.15f,0.13f,0.95f);
            textoEstado=TextoFilho(estado.transform,"Painel de teste pronto. As acoes ainda sao rascunhos locais.",13,FontStyle.Normal,new Color(0.94f,0.93f,0.82f));
            textoEstado.alignment=TextAnchor.MiddleLeft;

            for(int i=0;i<4;i++) paginas[i]=CriarPagina(p.transform,i);
            TrocarAba(0);
            DefinirAberto(false);
        }

        private GameObject CriarPagina(Transform pai,int indice)
        {
            GameObject pg=NovoUI("Pagina "+indice,pai.transform,typeof(ScrollRect),typeof(Image));
            RectTransform r=pg.GetComponent<RectTransform>(); Anchors(r,new Vector2(0.035f,0.145f),new Vector2(0.965f,0.80f),Vector2.zero,Vector2.zero);
            Image pi=pg.GetComponent<Image>();pi.color=new Color(0.17f,0.19f,0.16f,0.78f);
            ScrollRect scroll=pg.GetComponent<ScrollRect>();scroll.horizontal=false;scroll.vertical=true;
            GameObject vp=NovoUI("Viewport",pg.transform,typeof(RectTransform),typeof(Image),typeof(Mask));
            RectTransform vr=vp.GetComponent<RectTransform>();vr.anchorMin=Vector2.zero;vr.anchorMax=Vector2.one;vr.offsetMin=Vector2.zero;vr.offsetMax=Vector2.zero;
            vp.GetComponent<Image>().color=new Color(1f,1f,1f,0.015f);vp.GetComponent<Mask>().showMaskGraphic=false;scroll.viewport=vr;
            GameObject content=NovoUI("Conteudo",vp.transform,typeof(RectTransform),typeof(VerticalLayoutGroup),typeof(ContentSizeFitter));
            RectTransform cr=content.GetComponent<RectTransform>();cr.anchorMin=new Vector2(0f,1f);cr.anchorMax=new Vector2(1f,1f);cr.pivot=new Vector2(0.5f,1f);cr.anchoredPosition=Vector2.zero;cr.sizeDelta=new Vector2(0f,0f);
            VerticalLayoutGroup v=content.GetComponent<VerticalLayoutGroup>();v.padding=new RectOffset(12,12,12,12);v.spacing=8f;v.childControlWidth=true;v.childForceExpandWidth=true;v.childControlHeight=false;v.childForceExpandHeight=false;
            ContentSizeFitter fit=content.GetComponent<ContentSizeFitter>();fit.verticalFit=ContentSizeFitter.FitMode.PreferredSize;fit.horizontalFit=ContentSizeFitter.FitMode.Unconstrained;
            scroll.content=cr;
            if(indice==0)
            {
                Rotulo(content.transform,"POLITICAS DO REINO",16,true);
                Rotulo(content.transform,"Nome da proposta",13,false);
                campoLei=Campo(content.transform,"Protecao das aldeias");
                Alternancia(content.transform,"Proteger aldeias e civis",true);
                Alternancia(content.transform,"Incentivar o comercio",false);
                Alternancia(content.transform,"Expandir o recrutamento",false);
                Acao(content.transform,"Preparar proposta de lei", "Rascunho local criado. Nenhuma politica nativa foi alterada.");
                Rotulo(content.transform,"Administracao financeira, impostos e cargos serao conectados apos validar os dados reais do reino.",13,false);
            }
            else if(indice==1)
            {
                Rotulo(content.transform,"DIPLOMACIA ENTRE REINOS",16,true);
                Rotulo(content.transform,"Texto da proposta",13,false);
                campoTratado=Campo(content.transform,"Proposta de paz e comercio");
                Acao(content.transform,"Preparar tratado","Rascunho de tratado preparado; ainda nao enviado a outro reino.");
                Acao(content.transform,"Solicitar reuniao","Pedido preparado localmente; ainda nao ha reuniao nativa.");
                Acao(content.transform,"Declarar guerra (bloqueado)","Bloqueado ate confirmar a API nativa de guerra.");
            }
            else if(indice==2)
            {
                Rotulo(content.transform,"REUNIOES E CONTATO COM VILAS",16,true);
                Rotulo(content.transform,"Pauta do conselho",13,false);
                campoReuniao=Campo(content.transform,"Necessidades das aldeias, recursos e seguranca");
                Acao(content.transform,"Preparar reuniao do conselho","Pauta guardada nesta sessao; ninguem foi convocado.");
                Acao(content.transform,"Contatar vila (demonstracao)","A integracao real com cidades/vilas ainda esta pendente.");
            }
            else
            {
                Rotulo(content.transform,"EVENTOS E INTERACOES DE RPG",16,true);
                Rotulo(content.transform,"Descricao do evento",13,false);
                campoEvento=Campo(content.transform,"Um emissario chega ao castelo com noticias da fronteira.");
                Acao(content.transform,"Criar rascunho de evento","Rascunho narrativo criado; sem efeito no mundo.");
                Acao(content.transform,"Escolha: negociar","Escolha marcada como demonstracao.");
                Acao(content.transform,"Escolha: recusar","Escolha marcada como demonstracao.");
            }
            pg.SetActive(false);
            return pg;
        }

        private void TrocarAba(int n)
        {
            aba=Mathf.Clamp(n,0,3);
            if(paginas!=null)for(int i=0;i<paginas.Length;i++)if(paginas[i]!=null)paginas[i].SetActive(i==aba);
        }

        private void DefinirAberto(bool valor)
        {
            if(painel!=null)painel.gameObject.SetActive(valor);
            if(faixaAlternativa!=null)faixaAlternativa.SetActive(!valor && botaoNativoClonado==null);
        }

        private void Acao(Transform pai,string nome,string resposta)
        {
            Button b=Botao(pai,nome,new Color(0.34f,0.36f,0.29f,1f),46f);
            b.onClick.AddListener(delegate{if(textoEstado!=null)textoEstado.text=resposta;});
        }

        private void Rotulo(Transform pai,string frase,int tamanho,bool forte)
        {
            GameObject go=NovoUI("Rotulo",pai,typeof(LayoutElement));
            LayoutElement le=go.GetComponent<LayoutElement>();le.preferredHeight=forte?34f:27f;le.minHeight=le.preferredHeight;
            Text t=TextoFilho(go.transform,frase,tamanho,forte?FontStyle.Bold:FontStyle.Normal,forte?new Color(1f,0.70f,0.27f):new Color(0.92f,0.92f,0.82f));
            t.alignment=TextAnchor.MiddleLeft;
        }

        private InputField Campo(Transform pai,string placeholder)
        {
            GameObject go=NovoUI("Campo de texto",pai,typeof(Image),typeof(InputField),typeof(LayoutElement));
            Image img=go.GetComponent<Image>();img.color=new Color(0.10f,0.12f,0.10f,0.98f);
            LayoutElement le=go.GetComponent<LayoutElement>();le.preferredHeight=68f;le.minHeight=58f;
            InputField input=go.GetComponent<InputField>();input.targetGraphic=img;input.lineType=InputField.LineType.MultiLineNewline;
            GameObject tx=NovoUI("Texto",go.transform,typeof(Text));
            RectTransform tr=tx.GetComponent<RectTransform>();tr.anchorMin=Vector2.zero;tr.anchorMax=Vector2.one;tr.offsetMin=new Vector2(10f,7f);tr.offsetMax=new Vector2(-10f,-7f);
            Text t=tx.GetComponent<Text>();t.font=fonte;t.fontSize=14;t.color=new Color(0.96f,0.95f,0.88f);t.alignment=TextAnchor.UpperLeft;t.supportRichText=false;input.textComponent=t;
            GameObject ph=NovoUI("Placeholder",go.transform,typeof(Text));
            RectTransform pr=ph.GetComponent<RectTransform>();pr.anchorMin=Vector2.zero;pr.anchorMax=Vector2.one;pr.offsetMin=new Vector2(10f,7f);pr.offsetMax=new Vector2(-10f,-7f);
            Text pt=ph.GetComponent<Text>();pt.font=fonte;pt.fontSize=14;pt.fontStyle=FontStyle.Italic;pt.color=new Color(0.57f,0.60f,0.53f);pt.text=placeholder;pt.alignment=TextAnchor.UpperLeft;input.placeholder=pt;
            input.text=placeholder;
            return input;
        }

        private void Alternancia(Transform pai,string nome,bool ligado)
        {
            GameObject go=NovoUI("Opcao "+nome,pai,typeof(Toggle),typeof(Image),typeof(LayoutElement));
            LayoutElement le=go.GetComponent<LayoutElement>();le.preferredHeight=34f;le.minHeight=32f;
            Image fundo=go.GetComponent<Image>();fundo.color=new Color(0.20f,0.22f,0.18f,0.8f);
            Toggle tog=go.GetComponent<Toggle>();tog.isOn=ligado;tog.targetGraphic=fundo;
            GameObject texto=NovoUI("Texto",go.transform,typeof(Text));
            RectTransform r=texto.GetComponent<RectTransform>();r.anchorMin=Vector2.zero;r.anchorMax=Vector2.one;r.offsetMin=new Vector2(12f,0f);r.offsetMax=new Vector2(-8f,0f);
            Text t=texto.GetComponent<Text>();t.font=fonte;t.fontSize=14;t.color=Color.white;t.alignment=TextAnchor.MiddleLeft;t.text=(ligado?"[x] ":"[ ] ")+nome;
            tog.onValueChanged.AddListener(delegate(bool valor){t.text=(valor?"[x] ":"[ ] ")+nome;});
        }

        private Button Botao(Transform pai,string legenda,Color cor,float altura)
        {
            GameObject go=NovoUI("Botao "+legenda,pai,typeof(Image),typeof(Button),typeof(LayoutElement));
            Image img=go.GetComponent<Image>();img.color=cor;
            LayoutElement le=go.GetComponent<LayoutElement>();le.preferredHeight=altura;le.minHeight=altura;
            Button b=go.GetComponent<Button>();b.targetGraphic=img;b.transition=Selectable.Transition.ColorTint;
            ColorBlock cb=b.colors;cb.normalColor=cor;cb.highlightedColor=new Color(Mathf.Min(cor.r+0.12f,1f),Mathf.Min(cor.g+0.12f,1f),Mathf.Min(cor.b+0.12f,1f),1f);cb.pressedColor=new Color(0.72f,0.61f,0.36f,1f);b.colors=cb;
            Text t=TextoFilho(go.transform,legenda,14,FontStyle.Bold,Color.white);t.alignment=TextAnchor.MiddleCenter;
            return b;
        }

        private GameObject NovoUI(string nome,Transform pai,params Type[] comps)
        {
            bool temRect=false;
            for(int i=0;i<comps.Length;i++)if(comps[i]==typeof(RectTransform)){temRect=true;break;}
            Type[] tipos=comps;
            if(!temRect)
            {
                tipos=new Type[comps.Length+1];
                tipos[0]=typeof(RectTransform);
                Array.Copy(comps,0,tipos,1,comps.Length);
            }
            GameObject go=new GameObject(nome,tipos);go.transform.SetParent(pai,false);
            RectTransform r=go.GetComponent<RectTransform>();
            if(r!=null){r.localScale=Vector3.one;r.localRotation=Quaternion.identity;}
            return go;
        }

        private Text TextoFilho(Transform pai,string frase,int tamanho,FontStyle estilo,Color cor)
        {
            GameObject go=NovoUI("Texto "+frase,pai,typeof(Text));
            RectTransform r=go.GetComponent<RectTransform>();r.anchorMin=Vector2.zero;r.anchorMax=Vector2.one;r.offsetMin=new Vector2(8f,2f);r.offsetMax=new Vector2(-8f,-2f);
            Text t=go.GetComponent<Text>();t.font=fonte;t.fontSize=tamanho;t.fontStyle=estilo;t.color=cor;t.text=frase;t.supportRichText=false;t.horizontalOverflow=HorizontalWrapMode.Wrap;t.verticalOverflow=VerticalWrapMode.Overflow;
            return t;
        }

        private static void Anchors(RectTransform r,Vector2 min,Vector2 max,Vector2 offMin,Vector2 offMax)
        {
            r.anchorMin=min;r.anchorMax=max;r.offsetMin=offMin;r.offsetMax=offMax;
        }

        private void AdicionarBordaDourada(GameObject go)
        {
            Outline outline=go.AddComponent<Outline>();outline.effectColor=new Color(0.68f,0.47f,0.20f,0.95f);outline.effectDistance=new Vector2(2f,-2f);
        }

        private sealed class CanvasSafeAreaLayout : MonoBehaviour
        {
            private RectTransform rect;
            private Rect ultima;
            private void Awake(){rect=transform as RectTransform;Aplicar();}
            private void Update(){if(rect==null)rect=transform as RectTransform;if(Screen.safeArea!=ultima)Aplicar();}
            private void Aplicar()
            {
                if(rect==null)return;
                ultima=Screen.safeArea;
                Vector2 min=ultima.position;
                Vector2 max=ultima.position+ultima.size;
                min.x/=Mathf.Max(1f,Screen.width);min.y/=Mathf.Max(1f,Screen.height);
                max.x/=Mathf.Max(1f,Screen.width);max.y/=Mathf.Max(1f,Screen.height);
                rect.anchorMin=min;rect.anchorMax=max;rect.offsetMin=Vector2.zero;rect.offsetMax=Vector2.zero;
            }
        }

        private static Sprite BuscarSpriteJanela()
        {
            try
            {
                Type t=Type.GetType("ScrollWindow, Assembly-CSharp");
                if(t==null)return null;
                UnityEngine.Object[] objs=Resources.FindObjectsOfTypeAll(t);
                for(int i=0;i<objs.Length;i++)
                {
                    Component c=objs[i] as Component;if(c==null)continue;
                    FieldInfo f=t.GetField("_bg_rect",BindingFlags.Public|BindingFlags.NonPublic|BindingFlags.Instance);
                    if(f==null)continue;
                    RectTransform rect=f.GetValue(c) as RectTransform;
                    if(rect==null)continue;
                    Image image=rect.GetComponent<Image>();if(image==null)image=rect.GetComponentInChildren<Image>(true);
                    if(image!=null&&image.sprite!=null)return image.sprite;
                }
            }
            catch(Exception ex){Debug.Log("[Observatorio] Sprite nativo indisponivel: "+ex.GetType().Name);}
            return null;
        }

        private void TentarInserirBotaoNativo()
        {
            if(botaoNativoClonado!=null)return;
            string[] tipos={"SelectedPowerTabButtons","SelectedPowerTabTopButtons"};
            for(int t=0;t<tipos.Length;t++)
            {
                Type tipo=Type.GetType(tipos[t]+", Assembly-CSharp");if(tipo==null)continue;
                UnityEngine.Object[] objetos=Resources.FindObjectsOfTypeAll(tipo);
                for(int i=0;i<objetos.Length;i++)
                {
                    Component c=objetos[i] as Component;if(c==null||c.gameObject==null)continue;
                    Button info=LocalizarBotaoInfo(c,tipo);
                    if(info==null||info.transform==null||info.transform.parent==null)continue;
                    if(info.gameObject.scene.IsValid()==false)continue;
                    if(info.transform.parent.Find(IdBotao)!=null){botaoNativoClonado=info.transform.parent.Find(IdBotao).GetComponent<Button>();return;}
                    Button novo=CriarBotaoVizinho(info);
                    if(novo!=null){botaoNativoClonado=novo;donoBotaoNativo=c;if(faixaAlternativa!=null)faixaAlternativa.SetActive(false);Debug.Log("[Observatorio] Botao de governo inserido acima do botao nativo de informacoes.");return;}
                }
            }
        }

        private Button LocalizarBotaoInfo(Component c,Type tipo)
        {
            string[] preferidos={"_button_open_window","_button_open_main","button_open_main","buttonOpenMain","button_open_window","button_info","info_button"};
            for(int p=0;p<preferidos.Length;p++)
            {
                for(Type t=tipo;t!=null;t=t.BaseType)
                {
                    FieldInfo f=t.GetField(preferidos[p],BindingFlags.Public|BindingFlags.NonPublic|BindingFlags.Instance|BindingFlags.DeclaredOnly);
                    if(f==null)continue;
                    Button b=ComoBotao(f.GetValue(c));if(b!=null)return b;
                }
            }
            Button[] bs=c.GetComponentsInChildren<Button>(true);
            for(int i=0;i<bs.Length;i++)
            {
                string n=bs[i].gameObject.name.ToLowerInvariant();
                if(n.Contains("open")||n.Contains("main")||n.Contains("info")||n.Contains("window"))return bs[i];
            }
            return null;
        }

        private Button ComoBotao(object o)
        {
            Button b=o as Button;if(b!=null)return b;
            GameObject g=o as GameObject;if(g!=null)return g.GetComponent<Button>();
            Component c=o as Component;if(c!=null)return c.GetComponent<Button>();
            return null;
        }

        private Button CriarBotaoVizinho(Button original)
        {
            try
            {
                Transform pai=original.transform.parent;
                RectTransform velho=original.GetComponent<RectTransform>();
                GameObject go=NovoUI(IdBotao,pai,typeof(Image),typeof(Button),typeof(LayoutElement));
                Image img=go.GetComponent<Image>();
                Image source=original.targetGraphic as Image;if(source==null)source=original.GetComponent<Image>();
                if(source!=null){img.sprite=source.sprite;img.type=source.type;img.color=source.color;img.material=source.material;img.preserveAspect=source.preserveAspect;}
                else img.color=new Color(0.26f,0.29f,0.24f,1f);
                Button b=go.GetComponent<Button>();b.targetGraphic=img;
                LayoutElement le=go.GetComponent<LayoutElement>();
                le.preferredWidth=velho.rect.width;le.preferredHeight=velho.rect.height;le.minWidth=velho.rect.width;le.minHeight=velho.rect.height;
                RectTransform nr=go.GetComponent<RectTransform>();nr.sizeDelta=velho.sizeDelta;nr.anchorMin=velho.anchorMin;nr.anchorMax=velho.anchorMax;nr.pivot=velho.pivot;nr.localScale=velho.localScale;
                HorizontalLayoutGroup horizontal=pai.GetComponent<HorizontalLayoutGroup>();
                VerticalLayoutGroup vertical=pai.GetComponent<VerticalLayoutGroup>();
                if(vertical!=null){nr.SetSiblingIndex(original.transform.GetSiblingIndex());}
                else if(horizontal!=null)
                {
                    Transform grand=pai.parent;
                    if(grand!=null)
                    {
                        go.transform.SetParent(grand,false);
                        nr.anchorMin=new Vector2(0.5f,0.5f);nr.anchorMax=new Vector2(0.5f,0.5f);
                        nr.pivot=new Vector2(0.5f,0.5f);nr.sizeDelta=new Vector2(velho.rect.width,velho.rect.height);
                        nr.position=velho.position+new Vector3(0f,velho.rect.height+6f,0f);
                        nr.SetAsLastSibling();
                    }
                    else {nr.SetSiblingIndex(original.transform.GetSiblingIndex());}
                }
                else
                {
                    nr.SetSiblingIndex(original.transform.GetSiblingIndex());
                    nr.anchoredPosition=velho.anchoredPosition+new Vector2(0f,velho.rect.height+6f);
                }
                Text oldText=original.GetComponentInChildren<Text>(true);
                Text texto=TextoFilho(go.transform,"GOV",Mathf.Clamp(Mathf.RoundToInt(velho.rect.width/6f),9,13),FontStyle.Bold,new Color(1f,0.72f,0.28f));
                texto.alignment=TextAnchor.MiddleCenter;
                if(oldText!=null&&oldText.font!=null)texto.font=oldText.font;
                b.onClick.AddListener(delegate{Abrir();});
                return b;
            }
            catch(Exception ex){Debug.LogWarning("[Observatorio] Nao foi possivel inserir o botao na barra nativa: "+ex.GetType().Name+": "+ex.Message);return null;}
        }

        private void InstalarProtecaoStatus()
        {
            if(patchInstalado)return;
            try
            {
                Type harmonyType=Type.GetType("HarmonyLib.Harmony, 0Harmony");
                Type harmonyMethodType=Type.GetType("HarmonyLib.HarmonyMethod, 0Harmony");
                Type statsType=Type.GetType("StatsWindow, Assembly-CSharp");
                if(harmonyType==null||harmonyMethodType==null||statsType==null){Debug.LogWarning("[Observatorio] Protecao do status nativo indisponivel: API Harmony/StatsWindow ausente.");return;}
                MethodInfo original=statsType.GetMethod("OnEnable",BindingFlags.Public|BindingFlags.NonPublic|BindingFlags.Instance);
                MethodInfo finalizer=typeof(MenuAdministracaoOverlay).GetMethod("StatsWindowFinalizer",BindingFlags.Public|BindingFlags.Static);
                ConstructorInfo ctor=harmonyType.GetConstructor(new Type[]{typeof(string)});
                ConstructorInfo hmCtor=harmonyMethodType.GetConstructor(new Type[]{typeof(MethodInfo)});
                if(original==null||finalizer==null||ctor==null||hmCtor==null){Debug.LogWarning("[Observatorio] Protecao do status nativo nao encontrou os metodos requeridos.");return;}
                object harmony=ctor.Invoke(new object[]{"com.jin27012007.observatoriodosreinos.statusfallback"});
                object hm=hmCtor.Invoke(new object[]{finalizer});
                MethodInfo patch=null;
                MethodInfo[] ms=harmonyType.GetMethods(BindingFlags.Public|BindingFlags.Instance);
                for(int i=0;i<ms.Length;i++)
                {
                    if(ms[i].Name!="Patch")continue;
                    ParameterInfo[] ps=ms[i].GetParameters();
                    if(ps.Length==5&&ps[0].ParameterType==typeof(MethodBase)){patch=ms[i];break;}
                }
                if(patch==null){Debug.LogWarning("[Observatorio] Harmony.Patch compativel nao encontrado.");return;}
                patch.Invoke(harmony,new object[]{original,null,null,null,hm});
                patchInstalado=true;
                Debug.Log("[Observatorio] Protecao de fallback instalada para erro NullReference no status do reino.");
            }
            catch(Exception ex){Debug.LogWarning("[Observatorio] Protecao do status nativo falhou sem interromper o mod: "+ex.GetType().Name+": "+ex.Message);}
        }

        public static Exception StatsWindowFinalizer(Exception __exception, object __instance)
        {
            if(__exception==null)return null;
            if(!(__exception is NullReferenceException))return __exception;
            string trace=__exception.StackTrace??"";
            if(trace.IndexOf("showWhoIsSelected",StringComparison.OrdinalIgnoreCase)<0 &&
               trace.IndexOf("showWorldTipSelected",StringComparison.OrdinalIgnoreCase)<0)return __exception;
            try
            {
                Component c=__instance as Component;
                if(c!=null&&c.gameObject!=null)c.gameObject.SetActive(false);
                Debug.LogWarning("[Observatorio] FINALIZER ACIONADO: NullReference no status nativo. Stack="+trace.Replace("\\n"," | "));
                Abrir();
                if(instancia!=null && instancia.textoEstado!=null)
                    instancia.textoEstado.text="A janela nativa falhou ao abrir. O Observatorio interceptou essa falha e exibiu o painel alternativo.";
                return null;
            }
            catch(Exception ex){Debug.LogWarning("[Observatorio] Fallback visual falhou: "+ex.GetType().Name);return __exception;}
        }

        private void Start()
        {
            if(faixaAlternativa!=null)faixaAlternativa.SetActive(botaoNativoClonado==null);
            if(textoDiagnostico!=null)textoDiagnostico.text="OBSERVATORIO 0.5.1-dev | UI iniciada. Procure o botao GOV ou o atalho GOVERNO.";
            Debug.Log("[Observatorio] Canvas, botao reserva e painel admin prontos. Screen="+Screen.width+"x"+Screen.height+" safeArea="+Screen.safeArea);
        }

        private void OnDestroy(){if(instancia==this)instancia=null;}

        private void DefinirAbertoInterno(bool valor){DefinirAberto(valor);}
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
            try{MenuAdministracaoOverlay.CriarSeNecessario();LogInfo("UI administrativa via Canvas criada; botao nativo sera procurado automaticamente.");}
            catch(Exception ex){LogInfo("UI administrativa indisponivel nesta build: "+ex.GetType().Name+": "+ex.Message);}
            LogInfo("Modo seguro: nenhuma entidade nativa foi alterada pelas opcoes atuais.");
        }
    }
}
