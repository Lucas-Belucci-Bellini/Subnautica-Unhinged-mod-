using HarmonyLib;
using UnityEngine;

namespace Unhinged.ScannerRoom.Patches
{
    /// <summary>
    /// Chuvisco na imagem conforme o drone se afasta.
    ///
    /// É o contrapeso do alcance ampliado: pilotar a 5 km deixa de ser gratuito, mas
    /// **só a imagem degrada** — o rastreamento e os blips continuam exatos. Degradar o
    /// dado seria tirar a utilidade; degradar a imagem cobra um preço sem mentir.
    /// </summary>
    [HarmonyPatch(typeof(MapRoomCamera))]
    internal static class DroneNoisePatch
    {
        // Só uma câmera é controlada por vez, então um par de campos basta — e não vaza
        // referência como um dicionário por câmera vazaria.
        private static MapRoomCamera _cameraAtual;
        private static MapRoomScreen _telaAtual;

        // `screenEffectModel` e um GameObject, nao o componente de efeito — o compilador
        // corrigiu essa suposicao. Buscar o componente a cada quadro num Update seria
        // desperdicio, entao ele e resolvido uma vez, ao assumir o controle.
        private static MapRoomCameraScreenFX _fxAtual;

        [HarmonyPostfix]
        [HarmonyPatch(nameof(MapRoomCamera.ControlCamera))]
        // `__0` e nao `screen`: casar pelo NOME do parametro amarra o patch a um detalhe
        // que nao da para confirmar no metadata (nomes de parametro nao aparecem la), e
        // um nome errado faz o Harmony estourar na hora de aplicar. Posicional nao erra.
        internal static void AoAssumirControle(MapRoomCamera __instance, MapRoomScreen __0)
        {
            _cameraAtual = __instance;
            _telaAtual = __0;
            _fxAtual = ResolverFx(__instance);
        }

        [HarmonyPostfix]
        [HarmonyPatch(nameof(MapRoomCamera.FreeCamera))]
        internal static void AoSoltarControle(MapRoomCamera __instance)
        {
            if (!ReferenceEquals(_cameraAtual, __instance)) return;

            // Devolve a imagem limpa: sair do controle não pode deixar chuvisco preso.
            AplicarRuido(0f);
            _cameraAtual = null;
            _telaAtual = null;
            _fxAtual = null;
        }

        [HarmonyPostfix]
        [HarmonyPatch(nameof(MapRoomCamera.Update))]
        internal static void DepoisDoUpdate(MapRoomCamera __instance)
        {
            var cfg = Plugin.Settings;
            if (cfg == null || !cfg.DroneNoiseEnabled.Value) return;
            if (!ReferenceEquals(_cameraAtual, __instance) || _telaAtual == null) return;

            AplicarRuido(CalcularRuido(__instance.GetScreenDistance(_telaAtual), cfg));
        }

        /// <summary>
        /// Limpo até <c>DistanciaImagemLimpa</c>; daí em diante cresce linearmente até
        /// <c>ChuviscoMaximo</c> no limite do alcance.
        /// </summary>
        private static float CalcularRuido(float distancia, ScannerRoomConfig cfg)
        {
            var limpo = cfg.DroneCleanDistance.Value;
            var maxDist = cfg.DroneMaxDistance.Value;
            var maxRuido = cfg.DroneMaxNoise.Value;

            if (distancia <= limpo) return 0f;

            // Faixa de degradação inexistente (config com limpo >= max): sem meio-termo,
            // é limpo até o limite. Evita divisão por zero e um salto brusco.
            if (maxDist <= limpo) return 0f;

            var t = Mathf.Clamp01((distancia - limpo) / (maxDist - limpo));
            return t * maxRuido;
        }

        private static void AplicarRuido(float ruido)
        {
            if (_fxAtual != null) _fxAtual.noiseFactor = ruido;
        }

        /// <summary>
        /// O efeito pode estar no proprio objeto ou num filho dele, dependendo de como o
        /// prefab foi montado — `GetComponentInChildren` cobre os dois casos.
        /// </summary>
        private static MapRoomCameraScreenFX ResolverFx(MapRoomCamera camera)
        {
            var modelo = camera != null ? camera.screenEffectModel : null;
            return modelo != null ? modelo.GetComponentInChildren<MapRoomCameraScreenFX>(true) : null;
        }
    }
}
