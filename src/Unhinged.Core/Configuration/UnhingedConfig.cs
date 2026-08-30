using BepInEx.Configuration;

namespace Unhinged.Core.Configuration
{
    /// <summary>Perfis de balanceamento previstos no plano (Fase 3).</summary>
    public enum BalanceProfile
    {
        /// <summary>Próximo do vanilla; só correções.</summary>
        Normal,

        /// <summary>Alcance e limites maiores, ainda com custo controlado.</summary>
        Expandido,

        /// <summary>Deliberadamente fora do balanceamento vanilla.</summary>
        Unhinged,
    }

    /// <summary>
    /// Configuração do Unhinged via BepInEx. Usar <see cref="ConfigEntry{T}"/> (e não um
    /// JSON próprio) faz o ConfigurationManager — já instalado na máquina do operador —
    /// editar tudo isto dentro do jogo, sem reiniciar.
    ///
    /// Nada aqui aplica efeito ainda: esta versão só declara os valores. Os patches do
    /// scanner entram depois, e cada um deve poder ser desligado por estas chaves.
    /// </summary>
    public sealed class UnhingedConfig
    {
        private const string SectionGeneral = "1. Geral";
        private const string SectionScanner = "2. Sala de Scanner";
        private const string SectionDrones = "3. Drones do Scanner";

        public ConfigEntry<BalanceProfile> Profile { get; }
        public ConfigEntry<bool> VerboseLogging { get; }
        public ConfigEntry<bool> WriteEnvironmentReport { get; }

        public ConfigEntry<bool> ScannerEnabled { get; }
        public ConfigEntry<float> ScannerRangeMeters { get; }
        public ConfigEntry<int> ScannerMaxResults { get; }
        public ConfigEntry<float> ScannerRefreshSeconds { get; }

        public ConfigEntry<bool> DroneDegradationEnabled { get; }
        public ConfigEntry<float> DroneCleanRangeMeters { get; }
        public ConfigEntry<float> DroneMaxNoise { get; }

        public UnhingedConfig(ConfigFile config)
        {
            Profile = config.Bind(
                SectionGeneral, "Perfil", BalanceProfile.Normal,
                "Perfil de balanceamento. Normal = próximo do vanilla; Unhinged = sem freio.");

            VerboseLogging = config.Bind(
                SectionGeneral, "LogDetalhado", false,
                "Log detalhado. Útil para diagnóstico; deixa o log grande.");

            WriteEnvironmentReport = config.Bind(
                SectionGeneral, "EscreverRelatorio", true,
                "Escreve BepInEx/Unhinged-Relatorio.md a cada partida: mods carregados, "
                + "mods que falharam e quais pilhas de modding estão presentes. "
                + "É o arquivo a enviar ao relatar um problema. Desligue se não quiser o arquivo.");

            // Vanilla: defaultRange = 300 m, +50 m por upgrade, teto de 500 m
            // (valores lidos das constantes de MapRoomFunctionality em Assembly-CSharp).
            // 5000 m é 10x o teto vanilla — e 100x a ÁREA. Ver docs/SCANNER_API_NOTES.md.
            ScannerEnabled = config.Bind(
                SectionScanner, "Ativado", true,
                "Liga as mudanças do Unhinged na sala de scanner. Desligado = comportamento vanilla.");

            ScannerRangeMeters = config.Bind(
                SectionScanner, "AlcanceMetros", 500f,
                new ConfigDescription(
                    "Alcance máximo do scanner, em metros. Vanilla vai a 500 m. " +
                    "O custo cresce com o QUADRADO do alcance: 5000 m varre 100x a área de 500 m.",
                    new AcceptableValueRange<float>(100f, 5000f)));

            ScannerMaxResults = config.Bind(
                SectionScanner, "MaxResultados", 200,
                new ConfigDescription(
                    "Teto de alvos rastreados por varredura. Existe para proteger o frame rate.",
                    new AcceptableValueRange<int>(10, 2000)));

            ScannerRefreshSeconds = config.Bind(
                SectionScanner, "IntervaloAtualizacaoSegundos", 1.0f,
                new ConfigDescription(
                    "Intervalo entre varreduras escalonadas. Menor = mais responsivo e mais caro.",
                    new AcceptableValueRange<float>(0.1f, 10f)));

            DroneDegradationEnabled = config.Bind(
                SectionDrones, "DegradacaoAtivada", true,
                "Degrada a IMAGEM do drone com a distância, sem perder o rastreamento lógico.");

            DroneCleanRangeMeters = config.Bind(
                SectionDrones, "AlcanceLimpoMetros", 2000f,
                new ConfigDescription(
                    "Até esta distância a imagem do drone fica limpa. Depois dela o ruído cresce.",
                    new AcceptableValueRange<float>(100f, 5000f)));

            DroneMaxNoise = config.Bind(
                SectionDrones, "RuidoMaximo", 1.0f,
                new ConfigDescription(
                    "Ruído máximo aplicado no pior caso (mapeia para MapRoomCameraScreenFX.noiseFactor).",
                    new AcceptableValueRange<float>(0f, 1f)));
        }
    }
}
