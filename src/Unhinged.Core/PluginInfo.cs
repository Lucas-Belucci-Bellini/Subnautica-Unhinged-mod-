namespace Unhinged.Core
{
    /// <summary>
    /// Identidade do plugin. Mantida em um único lugar para que o GUID usado no
    /// <c>BepInPlugin</c>, na instância do Harmony e nos logs nunca divirjam.
    /// </summary>
    internal static class PluginInfo
    {
        internal const string Guid = "com.subnauticaunhinged.core";
        internal const string Name = "Subnautica Unhinged — Core";
        internal const string Version = "0.1.0";

        /// <summary>GUID do Nautilus, confirmado no código do Nautilus e de mods publicados.</summary>
        internal const string NautilusGuid = "com.snmodding.nautilus";
    }
}
