namespace SMLHelper.V2.Handlers
{
    /// <summary>
    /// Equivale ao <c>SMLHelper.V2.Handlers.CraftDataHandler</c>.
    /// Só os membros que o código legado realmente usa — medido no FCS e no S.O.C.K. Tank:
    /// <c>GetTechData</c> (16 usos), <c>SetEquipmentType</c> (6), <c>SetQuickSlotType</c> (2).
    /// </summary>
    public static class CraftDataHandler
    {
        /// <summary>Compatibilidade com o padrão <c>CraftDataHandler.Main.X(...)</c>.</summary>
        public static ICraftDataHandler Main { get; } = new MainShim();

        /// <summary>
        /// A receita de um item. O Nautilus renomeou para <c>GetRecipeData</c> e mudou o
        /// tipo de retorno; a conversão implícita de <see cref="SMLHelper.V2.Crafting.TechData"/> cobre a volta.
        /// </summary>
        public static SMLHelper.V2.Crafting.TechData GetTechData(TechType techType)
            => Nautilus.Handlers.CraftDataHandler.GetRecipeData(techType);

        public static void SetTechData(TechType techType, SMLHelper.V2.Crafting.TechData techData)
            => Nautilus.Handlers.CraftDataHandler.SetRecipeData(techType, techData);

        public static void SetEquipmentType(TechType techType, EquipmentType equipmentType)
            => Nautilus.Handlers.CraftDataHandler.SetEquipmentType(techType, equipmentType);

        public static void SetQuickSlotType(TechType techType, QuickSlotType slotType)
            => Nautilus.Handlers.CraftDataHandler.SetQuickSlotType(techType, slotType);

        public static void SetItemSize(TechType techType, int width, int height)
            => Nautilus.Handlers.CraftDataHandler.SetItemSize(techType, width, height);

        public static void SetCraftingTime(TechType techType, float time)
            => Nautilus.Handlers.CraftDataHandler.SetCraftingTime(techType, time);

        private sealed class MainShim : ICraftDataHandler
        {
            public SMLHelper.V2.Crafting.TechData GetTechData(TechType techType) => CraftDataHandler.GetTechData(techType);

            public void SetTechData(TechType techType, SMLHelper.V2.Crafting.TechData techData)
                => CraftDataHandler.SetTechData(techType, techData);

            public void SetEquipmentType(TechType techType, EquipmentType equipmentType)
                => CraftDataHandler.SetEquipmentType(techType, equipmentType);

            public void SetQuickSlotType(TechType techType, QuickSlotType slotType)
                => CraftDataHandler.SetQuickSlotType(techType, slotType);
        }
    }

    public interface ICraftDataHandler
    {
        SMLHelper.V2.Crafting.TechData GetTechData(TechType techType);
        void SetTechData(TechType techType, SMLHelper.V2.Crafting.TechData techData);
        void SetEquipmentType(TechType techType, EquipmentType equipmentType);
        void SetQuickSlotType(TechType techType, QuickSlotType slotType);
    }
}
