namespace SMLHelper.V2.Handlers
{
    /// <summary>
    /// Equivale ao <c>SMLHelper.V2.Handlers.TechTypeHandler</c>.
    ///
    /// O Nautilus removeu os handlers por tipo de enum e os unificou no
    /// <c>EnumHandler</c> genérico — é o que o guia oficial de migração descreve.
    /// Este shim reexpõe a forma antiga sobre a nova.
    /// </summary>
    public static class TechTypeHandler
    {
        public static ITechTypeHandler Main { get; } = new MainShim();

        public static TechType AddTechType(string internalName, string displayName, string tooltip)
        {
            var builder = Nautilus.Handlers.EnumHandler.AddEntry<TechType>(internalName);
            Nautilus.Handlers.LanguageHandler.SetTechTypeName(builder.Value, displayName, "English");
            Nautilus.Handlers.LanguageHandler.SetTechTypeTooltip(builder.Value, tooltip, "English");
            return builder.Value;
        }

        public static TechType AddTechType(string internalName, string displayName, string tooltip, bool unlockAtStart)
        {
            var techType = AddTechType(internalName, displayName, tooltip);
            if (unlockAtStart)
                Nautilus.Handlers.KnownTechHandler.UnlockOnStart(techType);
            return techType;
        }

        public static bool TryGetModdedTechType(string internalName, out TechType techType)
            => Nautilus.Handlers.EnumHandler.TryGetValue(internalName, out techType);

        /// <summary>
        /// O Nautilus não tem um <c>ModdedEnumExists</c> — confirmado lendo o
        /// <c>EnumHandler</c>. A pergunta é respondida pelo próprio <c>TryGetValue</c>.
        /// </summary>
        public static bool ModdedTechTypeExists(string internalName)
            => Nautilus.Handlers.EnumHandler.TryGetValue<TechType>(internalName, out _);

        private sealed class MainShim : ITechTypeHandler
        {
            public TechType AddTechType(string internalName, string displayName, string tooltip)
                => TechTypeHandler.AddTechType(internalName, displayName, tooltip);

            public TechType AddTechType(string internalName, string displayName, string tooltip, bool unlockAtStart)
                => TechTypeHandler.AddTechType(internalName, displayName, tooltip, unlockAtStart);

            public bool TryGetModdedTechType(string internalName, out TechType techType)
                => TechTypeHandler.TryGetModdedTechType(internalName, out techType);

            public bool ModdedTechTypeExists(string internalName)
                => TechTypeHandler.ModdedTechTypeExists(internalName);
        }
    }

    public interface ITechTypeHandler
    {
        TechType AddTechType(string internalName, string displayName, string tooltip);
        TechType AddTechType(string internalName, string displayName, string tooltip, bool unlockAtStart);
        bool TryGetModdedTechType(string internalName, out TechType techType);
        bool ModdedTechTypeExists(string internalName);
    }
}
