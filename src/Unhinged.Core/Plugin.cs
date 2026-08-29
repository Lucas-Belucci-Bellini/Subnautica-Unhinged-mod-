using System;
using BepInEx;
using BepInEx.Logging;
using HarmonyLib;
using Unhinged.Core.Configuration;

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
    [BepInPlugin(PluginInfo.Guid, PluginInfo.Name, PluginInfo.Version)]
    [BepInDependency(PluginInfo.NautilusGuid, BepInDependency.DependencyFlags.HardDependency)]
    public sealed class Plugin : BaseUnityPlugin
    {
        /// <summary>Log do plugin, para uso das outras classes do assembly.</summary>
        internal static ManualLogSource Log { get; private set; }

        /// <summary>Configuração carregada, disponível depois do <see cref="Awake"/>.</summary>
        internal static UnhingedConfig Settings { get; private set; }

        private static readonly Harmony HarmonyInstance = new Harmony(PluginInfo.Guid);

        private void Awake()
        {
            Log = Logger;
            Settings = new UnhingedConfig(Config);

            Log.LogInfo($"{PluginInfo.Name} {PluginInfo.Version} carregando…");
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

            Log.LogInfo($"{PluginInfo.Name} carregado.");
        }

        private void OnDestroy()
        {
            HarmonyInstance.UnpatchSelf();
            Log?.LogInfo($"{PluginInfo.Name} descarregado.");
        }
    }
}
