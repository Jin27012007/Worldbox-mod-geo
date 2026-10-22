// Scaffold inicial. Não declaramos ainda uma classe de entrada do mod porque a
// interface exigida pela versão instalada do NeoModLoader precisa ser confirmada
// na assembly real antes da implementação do ciclo de vida.
//
// Próximo passo: inspecionar interfaces/atributos exportados pelo NeoModLoader.dll
// e então criar uma classe que compile contra as referências concretas.
namespace ObservatorioDosReinos
{
    internal static class ProjetoInfo
    {
        internal const string Nome = "Observatorio dos Reinos";
        internal const string Versao = "0.1.0-dev";
        internal const string WorldBoxAlvo = "0.51.4";
        internal const bool SomenteLeitura = true;
    }
}
