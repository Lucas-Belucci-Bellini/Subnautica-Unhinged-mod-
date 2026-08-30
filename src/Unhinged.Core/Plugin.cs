using System;
using BepInEx;
using BepInEx.Logging;
using HarmonyLib;
using Unhinged.Core.Configuration;
using Unhinged.Core.Diagnostics;
using Unhinged.Core.Interop;

namespace Unhinged.Core
{
    /// <summary>
    /// Ponto de entrada do Subnautica Unhinged.
    ///
    /// Esta versão é deliberadamente um esqueleto: sobe logging e configuração e
    /// NÃO aplica nenhum patch de jogo. A regra do projeto é "preparar != implementar" —
    /// os patches do scanner só entram depois da nota de API estar fechada
    /// (docs/SCANNER_API_NOTES.md) e revisada.
    /// </summary>
    [BepInPlugin(UnhingedInfo.Guid, UnhingedInfo.Name, UnhingedInfo.Version)]
    // SoftDependency, não Hard: garante que o Unhinged carregue DEPOIS do Nautilus
    // (é o que importa para uma camada de override) sem se recusar a carregar caso o
    // Nautilus falhe ou esteja numa versão inesperada. Como camada que existe para
    // consertar a convivência entre mods, ser a primeira a desistir seria contraditório.
    // Vira HardDependency no dia em que este assembly realmente chamar uma API do Nautilus.
    [BepInDependency(UnhingedInfo.NautilusGuid, BepInDependency.DependencyFlags.SoftDependency)]
    public sealed class Plugin : BaseUnityPlugin
    {
        /// <summary>Log do plugin, para uso das outras classes do assembly.</summary>
        internal static ManualLogSource Log { get; private set; }

        /// <summary>Configuração carregada, disponível depois do <see cref="Awake"/>.</summary>
        internal static UnhingedConfig Settings { get; private set; }

        private static readonly Harmony HarmonyInstance = new Harmony(UnhingedInfo.Guid);

        private void Awake()
        {
            Log = Logger;
            Settings = new UnhingedConfig(Config);

            Log.LogInfo($"{UnhingedInfo.Name} {UnhingedInfo.Version} carregando…");
            Log.LogInfo($"Perfil: {Settings.Profile.Value}");

            try
            {
                // Sem patches ainda — a chamada existe para que a infraestrutura do
                // Harmony esteja provada desde o primeiro build. PatchAll() sobre um
                // assembly sem [HarmonyPatch] é um no-op seguro.
                HarmonyInstance.PatchAll(typeof(Plugin).Assembly);
            }
            catch (Exception ex)
            {
                // Uma exceção aqui derrubaria o carregamento do plugin inteiro.
                // Logar e seguir deixa o resto do jogo utilizável e o erro visível.
                Log.LogError($"Falha ao aplicar patches do Harmony: {ex}");
                return;
            }

            Log.LogInfo($"{UnhingedInfo.Name} carregado.");
        }

        /// <summary>
        /// Inventário e relatório. Mora aqui, e **não** no <see cref="Awake"/>, por um
        /// motivo que muda o resultado: o BepInEx instancia os plugins UM A UM durante o
        /// chainload, e cada instanciação dispara o `Awake` daquele plugin na hora. Ou
        /// seja, no nosso `Awake` o `Chainloader.PluginInfos` só tem os plugins carregados
        /// ATÉ AQUI — quem vem depois de nós na ordem ainda não existe.
        ///
        /// Um inventário parcial não seria só incompleto, seria enganoso: mostraria
        /// dezenas de mods "ausentes" que na verdade carregam normalmente.
        ///
        /// O `Start` do Unity roda só no primeiro quadro depois que os componentes foram
        /// adicionados, e o chainload inteiro acontece dentro de um quadro. Então aqui a
        /// lista está completa — é a diferença entre um diagnóstico e um boato.
        /// </summary>
        private void Start()
        {
            // Awake sempre roda antes de Start, mas ele pode ter saido cedo por falha do
            // Harmony. Sem os dois, nao ha o que inventariar nem onde registrar.
            if (Log == null || Settings == null) return;

            try
            {
                ModRegistry.LogInventory(Log, Settings.VerboseLogging.Value);
            }
            catch (Exception ex)
            {
                Log.LogWarning($"Falha ao inventariar os mods carregados: {ex.Message}");
            }

            // O mesmo inventário, em arquivo curto e legível. O log do BepInEx tem
            // dezenas de milhares de linhas de todos os mods; pedir para alguém
            // garimpar aquilo é pedir para o diagnóstico não acontecer.
            if (Settings.WriteEnvironmentReport.Value)
                RelatorioDeAmbiente.Escrever(Log);
        }

        private void OnDestroy()
        {
            HarmonyInstance.UnpatchSelf();
            Log?.LogInfo($"{UnhingedInfo.Name} descarregado.");
        }
    }
}
