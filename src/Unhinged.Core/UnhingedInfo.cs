namespace Unhinged.Core
{
    /// <summary>
    /// Identidade deste plugin, num lugar só, para que o GUID usado no
    /// <c>BepInPlugin</c>, na instância do Harmony e nos logs nunca divirjam.
    ///
    /// O nome é propositalmente distinto de <c>BepInEx.PluginInfo</c>: a camada de
    /// interop usa aquele tipo o tempo todo, e dois <c>PluginInfo</c> visíveis no mesmo
    /// namespace fazem o compilador escolher o nosso em silêncio.
    /// </summary>
    internal static class UnhingedInfo
    {
        internal const string Guid = "com.subnauticaunhinged.core";
        internal const string Name = "Subnautica Unhinged — Core";
        internal const string Version = "0.1.0";

        /// <summary>GUID do Nautilus, confirmado no código do Nautilus e de mods publicados.</summary>
        internal const string NautilusGuid = "com.snmodding.nautilus";
    }
}
