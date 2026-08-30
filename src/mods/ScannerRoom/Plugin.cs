using System;
using BepInEx;
using HarmonyLib;
using Unhinged.ScannerRoom.Patches;

namespace Unhinged.ScannerRoom
{
    /// <summary>
    /// Sala de scanner: alcance de scanner e de drone acima do vanilla.
    ///
    /// Mod próprio, com versão e release próprios — não depende do Alterra Hub nem do
    /// Core, e mexe só em classes do jogo base.
    /// </summary>
    [BepInPlugin(Guid, "Subnautica Unhinged — Sala de Scanner", "0.1.0")]
    public sealed class Plugin : BaseUnityPlugin
    {
        internal const string Guid = "com.subnauticaunhinged.scannerroom";

        internal static ScannerRoomConfig Settings { get; private set; }

        private void Awake()
        {
            // Antes do PatchAll: o transpiler do drone lê a configuração no momento em
            // que é aplicado, não a cada chamada. Configurar depois seria tarde.
            Settings = new ScannerRoomConfig(Config);

            try
            {
                new Harmony(Guid).PatchAll(typeof(Plugin).Assembly);
            }
            catch (Exception ex)
            {
                Logger.LogError($"Falha ao aplicar os patches: {ex}");
                return;
            }

            RelatarEstado();
        }

        private void RelatarEstado()
        {
            if (Settings.ScannerEnabled.Value)
            {
                Logger.LogInfo(
                    $"Scanner: {Settings.ScannerBaseRange.Value:0} m sem chip → "
                    + $"{Settings.ScannerMaxRange.Value:0} m com os 4 (vanilla: 300 → 500).");
            }
            else
            {
                Logger.LogInfo("Scanner: desligado, alcance vanilla.");
            }

            if (!Settings.DroneEnabled.Value)
            {
                Logger.LogInfo("Drone: desligado, alcance vanilla.");
                return;
            }

            // Um transpiler que não casa com nada falha em SILÊNCIO. Sem esta linha, o
            // sintoma seria "o drone continua parando em 500 m" sem nada no log — o tipo
            // de defeito que custa horas para achar.
            var trocas = DroneRangePatch.SubstituicoesFeitas;
            if (trocas > 0)
            {
                Logger.LogInfo(
                    $"Drone: {Settings.DroneMaxDistance.Value:0} m (vanilla: 500). "
                    + $"{trocas} literal(is) substituído(s) em CanBeControlled.");
            }
            else
            {
                Logger.LogWarning(
                    "Drone: o patch de alcance NÃO pegou — nenhum literal 500 foi encontrado "
                    + "em MapRoomCamera.CanBeControlled. O drone segue com o alcance vanilla. "
                    + "Provável causa: o jogo mudou o método numa atualização. "
                    + "Reporte com este log.");
            }
        }
    }
}
