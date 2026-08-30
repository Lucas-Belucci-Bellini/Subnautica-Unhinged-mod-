using BepInEx.Logging;

namespace Unhinged.Legacy
{
    /// <summary>
    /// Log da própria ponte. Existe porque o shim precisa reportar problemas sem depender
    /// de um plugin específico — ele é carregado por qualquer mod portado, não por um só.
    /// </summary>
    public static class LegacyLog
    {
        private static readonly ManualLogSource Source =
            Logger.CreateLogSource("Unhinged.Legacy");

        public static void Info(string message) => Source.LogInfo(message);
        public static void Warn(string message) => Source.LogWarning(message);
        public static void Error(string message) => Source.LogError(message);
    }
}
