using System;
using System.Collections.Generic;
using System.Linq;
using BepInEx;
using BepInEx.Bootstrap;
using BepInEx.Logging;

namespace Unhinged.Core.Interop
{
    /// <summary>
    /// Inventário, em tempo de execução, de tudo que o BepInEx carregou.
    ///
    /// É a base do Unhinged como camada de override: em vez de juntar os mods num
    /// binário só — o que exigiria redistribuir código de terceiros e congelaria cada
    /// mod na versão copiada — o Unhinged descobre os mods JÁ INSTALADOS e trabalha
    /// em cima deles. Os mods continuam atualizáveis; a camada é que se adapta.
    /// </summary>
    public static class ModRegistry
    {
        /// <summary>Todo plugin carregado, indexado por GUID pelo próprio BepInEx.</summary>
        public static IReadOnlyDictionary<string, PluginInfo> All => Chainloader.PluginInfos;

        /// <summary>
        /// Mods que o BepInEx tentou carregar e falhou (dependência faltando, versão
        /// incompatível). É a primeira coisa a olhar quando "um mod sumiu".
        /// </summary>
        public static IReadOnlyList<string> LoadFailures => Chainloader.DependencyErrors;

        public static bool IsLoaded(string guid) =>
            !string.IsNullOrEmpty(guid) && Chainloader.PluginInfos.ContainsKey(guid);

        public static bool TryGet(string guid, out PluginInfo info)
        {
            info = null;
            return !string.IsNullOrEmpty(guid) && Chainloader.PluginInfos.TryGetValue(guid, out info);
        }

        /// <summary>Versão declarada por um mod, ou <c>null</c> se ele não está carregado.</summary>
        public static Version GetVersion(string guid) =>
            TryGet(guid, out var info) ? info.Metadata?.Version : null;

        /// <summary>
        /// Procura um plugin pelo nome legível quando o GUID não é conhecido de antemão.
        /// Vários mods do Nexus não publicam o GUID em lugar nenhum — descobrir pelo nome
        /// é o caminho prático, e o resultado deve virar constante depois de confirmado.
        /// </summary>
        public static IEnumerable<PluginInfo> FindByName(string fragment)
        {
            if (string.IsNullOrEmpty(fragment)) yield break;

            foreach (var info in Chainloader.PluginInfos.Values)
            {
                var name = info?.Metadata?.Name;
                if (name != null && name.IndexOf(fragment, StringComparison.OrdinalIgnoreCase) >= 0)
                    yield return info;
            }
        }

        /// <summary>
        /// Despeja o inventário no log. As falhas de carga saem SEMPRE — são o sintoma
        /// mais barato de diagnosticar e o mais caro de perceber tarde. A lista completa
        /// dos mods fica atrás do log detalhado, porque são dezenas de linhas.
        /// </summary>
        public static void LogInventory(ManualLogSource log, bool verbose)
        {
            if (log == null) return;

            var plugins = Chainloader.PluginInfos;
            log.LogInfo($"Mods carregados: {plugins.Count}");

            var failures = Chainloader.DependencyErrors;
            if (failures != null && failures.Count > 0)
            {
                // Nível Warning de propósito: isto explica mods que "sumiram".
                log.LogWarning($"{failures.Count} mod(s) NÃO carregaram:");
                foreach (var failure in failures)
                    log.LogWarning($"  · {failure}");
            }

            if (!verbose) return;

            foreach (var info in plugins.Values
                         .Where(p => p?.Metadata != null)
                         .OrderBy(p => p.Metadata.Name, StringComparer.OrdinalIgnoreCase))
            {
                log.LogInfo($"  · {info.Metadata.Name} {info.Metadata.Version} [{info.Metadata.GUID}]");
            }
        }
    }
}
