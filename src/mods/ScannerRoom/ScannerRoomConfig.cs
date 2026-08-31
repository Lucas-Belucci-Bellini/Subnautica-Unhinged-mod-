using BepInEx.Configuration;

namespace Unhinged.ScannerRoom
{
    /// <summary>
    /// Configuração da sala de scanner, via BepInEx — o ConfigurationManager edita tudo
    /// isto em jogo, sem reiniciar.
    ///
    /// Os valores vanilla estão citados em cada entrada porque eles são a régua: sem
    /// eles, "1000" e "5000" são números soltos.
    /// </summary>
    internal sealed class ScannerRoomConfig
    {
        private const string SecScanner = "1. Alcance do scanner";
        private const string SecDrone = "2. Drone (câmera)";

        public ConfigEntry<bool> ScannerEnabled { get; }
        public ConfigEntry<float> ScannerBaseRange { get; }
        public ConfigEntry<float> ScannerMaxRange { get; }

        public ConfigEntry<bool> DroneEnabled { get; }
        public ConfigEntry<float> DroneMaxDistance { get; }

        public ConfigEntry<bool> DroneNoiseEnabled { get; }
        public ConfigEntry<float> DroneCleanDistance { get; }
        public ConfigEntry<float> DroneMaxNoise { get; }

        public ScannerRoomConfig(ConfigFile config)
        {
            ScannerEnabled = config.Bind(SecScanner, "Ativado", true,
                "Liga a mudança de alcance do scanner. Desligado = comportamento vanilla.");

            // Vanilla: defaultRange = 300 m, +50 m por chip, 4 chips = 500 m
            // (constantes lidas do metadata de MapRoomFunctionality).
            ScannerBaseRange = config.Bind(SecScanner, "AlcanceBase", 1000f,
                new ConfigDescription(
                    "Alcance sem nenhum chip, em metros. Vanilla: 300.",
                    new AcceptableValueRange<float>(100f, 10000f)));

            ScannerMaxRange = config.Bind(SecScanner, "AlcanceMaximo", 5000f,
                new ConfigDescription(
                    "Alcance com os 4 chips, em metros. Vanilla: 500. "
                    + "⚠️ 5000 m é 10x o alcance vanilla e 100x a ÁREA varrida.",
                    new AcceptableValueRange<float>(100f, 10000f)));

            DroneEnabled = config.Bind(SecDrone, "Ativado", true,
                "Liga a mudança de alcance do drone. Desligado = comportamento vanilla.");

            // Vanilla: MapRoomScreen.maxCameraDistance = 500 m (const).
            DroneMaxDistance = config.Bind(SecDrone, "DistanciaMaxima", 1000f,
                new ConfigDescription(
                    "Até onde o drone pode se afastar da sala, em metros. Vanilla: 500.",
                    new AcceptableValueRange<float>(100f, 10000f)));

            DroneNoiseEnabled = config.Bind(SecDrone, "DegradarImagem", true,
                "Chuvisco na tela conforme o drone se afasta. O rastreamento NÃO é "
                + "afetado — só a imagem. É o custo visual de pilotar longe.");

            DroneCleanDistance = config.Bind(SecDrone, "DistanciaImagemLimpa", 2000f,
                new ConfigDescription(
                    "Até esta distância a imagem fica limpa; além dela o chuvisco cresce "
                    + "até DistanciaMaxima.",
                    new AcceptableValueRange<float>(0f, 10000f)));

            DroneMaxNoise = config.Bind(SecDrone, "ChuviscoMaximo", 0.75f,
                new ConfigDescription(
                    "Chuvisco no limite do alcance. 0 = imagem limpa, 1 = ilegível.",
                    new AcceptableValueRange<float>(0f, 1f)));
        }
    }
}
