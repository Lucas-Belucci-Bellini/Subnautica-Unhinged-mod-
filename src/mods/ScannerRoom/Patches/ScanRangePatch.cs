using HarmonyLib;
using UnityEngine;

namespace Unhinged.ScannerRoom.Patches
{
    /// <summary>
    /// Alcance do scanner: vanilla 300 m base, +50 m por chip, 500 m com os quatro.
    ///
    /// ⛔ <c>defaultRange</c> e <c>rangePerUpgrade</c> são <c>const</c>. O compilador as
    /// embute nos call sites, então **o Harmony não consegue patchá-las** — mexer nelas
    /// não faz nada. O que dá para mudar é o resultado: o campo <c>scanRange</c>, que a
    /// vanilla escreve em <see cref="MapRoomFunctionality.UpdateScanRangeAndInterval"/>.
    /// </summary>
    [HarmonyPatch(typeof(MapRoomFunctionality))]
    internal static class ScanRangePatch
    {
        /// <summary>Constantes vanilla, do metadata de <c>MapRoomFunctionality</c>.</summary>
        private const float VanillaBase = 300f;
        private const float VanillaPerUpgrade = 50f;
        private const int MaxUpgrades = 4;

        [HarmonyPostfix]
        [HarmonyPatch(nameof(MapRoomFunctionality.UpdateScanRangeAndInterval))]
        internal static void DepoisDeAtualizar(MapRoomFunctionality __instance)
        {
            var cfg = Plugin.Settings;
            if (cfg == null || !cfg.ScannerEnabled.Value || __instance == null) return;

            __instance.scanRange = Calcular(__instance.scanRange, cfg);
        }

        /// <summary>
        /// Também aqui, e não só no campo: se algum dia o <c>GetScanRange</c> passar a
        /// calcular em vez de ler o campo, a resposta continua sendo a mesma. Hoje as
        /// duas coincidem — o custo de manter as duas coerentes é zero.
        /// </summary>
        [HarmonyPostfix]
        [HarmonyPatch(nameof(MapRoomFunctionality.GetScanRange))]
        internal static void DepoisDeLer(MapRoomFunctionality __instance, ref float __result)
        {
            var cfg = Plugin.Settings;
            if (cfg == null || !cfg.ScannerEnabled.Value || __instance == null) return;

            __result = Calcular(__result, cfg);
        }

        /// <summary>
        /// Reescreve o alcance preservando a PROGRESSÃO da vanilla.
        ///
        /// Em vez de contar os chips por conta própria — o que exigiria mexer no
        /// <c>Equipment</c> e duplicar a regra de quais itens contam —, a contagem é
        /// **derivada do próprio resultado da vanilla**: ela já calculou
        /// <c>300 + n×50</c>, então <c>n = (alcance − 300) / 50</c>. Se o jogo mudar
        /// quais itens valem como upgrade, isto acompanha sozinho.
        /// </summary>
        private static float Calcular(float alcanceVanilla, ScannerRoomConfig cfg)
        {
            var chips = Mathf.Clamp(
                Mathf.RoundToInt((alcanceVanilla - VanillaBase) / VanillaPerUpgrade),
                0, MaxUpgrades);

            var baseR = cfg.ScannerBaseRange.Value;
            var maxR = cfg.ScannerMaxRange.Value;

            // Base acima do máximo é configuração incoerente; respeitar a base e não
            // devolver uma progressão decrescente.
            if (maxR <= baseR) return baseR;

            var porChip = (maxR - baseR) / MaxUpgrades;
            return baseR + chips * porChip;
        }
    }
}
