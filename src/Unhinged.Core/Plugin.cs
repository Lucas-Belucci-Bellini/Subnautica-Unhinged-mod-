using System;
using BepInEx;
using BepInEx.Logging;
using HarmonyLib;
using Unhinged.Core.Configuration;
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

            // Inventário do que mais está carregado. Roda depois dos patches para que uma
            // falha aqui não impeça o resto de subir — e serve de linha de base para
            // qualquer diagnóstico posterior de conflito entre mods.
            try
            {
                ModRegistry.LogInventory(Log, Settings.VerboseLogging.Value);
            }
            catch (Exception ex)
            {
                Log.LogWarning($"Falha ao inventariar os mods carregados: {ex.Message}");
            }

            Log.LogInfo($"{UnhingedInfo.Name} carregado.");
        }

        private void OnDestroy()
        {
            HarmonyInstance.UnpatchSelf();
            Log?.LogInfo($"{UnhingedInfo.Name} descarregado.");
        }
    }
}
