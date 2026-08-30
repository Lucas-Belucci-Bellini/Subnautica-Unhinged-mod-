using System;
using BepInEx;
using Unhinged.Legacy;

namespace Unhinged.AlterraHub
{
    /// <summary>
    /// Ponto de entrada do pacote Alterra Hub.
    ///
    /// A fonte do FCS marca seus pontos de entrada com <c>[QModCore]</c>/<c>[QModPatch]</c>,
    /// atributos que **só marcam** código — quem os executava era o QModManager, que não
    /// existe no ramo moderno. E o BepInEx só carrega assemblies com
    /// <c>[BepInPlugin]</c>. Sem esta classe, o DLL compilaria, seria ignorado no
    /// carregamento e o mod simplesmente não existiria em jogo — sem erro nenhum.
    /// </summary>
    [BepInPlugin(Guid, "Subnautica Unhinged — Alterra Hub (FCStudios)", "1.0.2")]
    // Hard, não Soft: ao contrário do Core, este pacote realmente chama a API do Nautilus
    // em toda receita e todo prefab. Carregar sem ele seria falhar mais tarde e pior.
    [BepInDependency(NautilusGuid, BepInDependency.DependencyFlags.HardDependency)]
    public sealed class Plugin : BaseUnityPlugin
    {
        internal const string Guid = "com.subnauticaunhinged.alterrahub";
        internal const string NautilusGuid = "com.snmodding.nautilus";

        /// <summary>
        /// Namespace do módulo que precisa registrar antes de todos os outros. Os outros
        /// seis módulos FCS consomem os serviços que ele publica (registro de dispositivos,
        /// moeda, loja), então rodá-los antes dá erro ou registro silenciosamente vazio.
        /// </summary>
        private const string ModuloBase = "FCS_AlterraHub";

        private void Awake()
        {
            try
            {
                var executados = LegacyModLoader.Run(
                    typeof(Plugin).Assembly,
                    Logger,
                    tipo => tipo.FullName != null && tipo.FullName.StartsWith(ModuloBase, StringComparison.Ordinal) ? 0 : 1);

                Logger.LogInfo($"Alterra Hub: {executados} ponto(s) de entrada executado(s).");

                if (executados == 0)
                {
                    Logger.LogWarning(
                        "Nenhum ponto de entrada rodou. O pacote foi carregado mas não registrou nada — "
                        + "confira se o Unhinged.Legacy.dll está na mesma pasta.");
                }
            }
            catch (Exception ex)
            {
                // Um pacote que falha inteiro não pode levar o resto do jogo junto.
                Logger.LogError($"Falha ao carregar o Alterra Hub: {ex}");
            }
        }
    }
}
