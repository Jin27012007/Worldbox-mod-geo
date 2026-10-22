using NeoModLoader.api;

namespace ObservatorioDosReinos
{
    /// <summary>
    /// Primeiro ponto de entrada do mod. Nesta fase, validamos somente o ciclo
    /// de vida e o log do NeoModLoader; não modificamos a simulação.
    /// </summary>
    public class Main : BasicMod<Main>
    {
        protected override void OnModLoad()
        {
            LogInfo("Observatorio dos Reinos carregado. Modo inicial: somente diagnostico/leitura.");
            LogInfo("WorldBox alvo informado: 0.51.4. Nenhum estado do mundo foi alterado.");
        }
    }
}
