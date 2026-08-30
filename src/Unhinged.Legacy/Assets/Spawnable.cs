using Nautilus.Assets;
using Nautilus.Assets.Gadgets;

namespace SMLHelper.V2.Assets
{
    /// <summary>
    /// Item que existe no mundo e no inventário. Base mais usada pelo código legado
    /// (13 das 20 classes derivadas no FCS).
    /// </summary>
    public abstract class Spawnable : ModPrefab
    {
        protected Spawnable(string classId, string friendlyName, string description)
            : base(classId, $"{classId}Prefab")
        {
            _friendlyName = friendlyName;
            _description = description;
        }

        private readonly string _friendlyName;
        private readonly string _description;

        internal override string FriendlyName => _friendlyName;
        internal override string Description => _description;

        /// <summary>Nome do arquivo de ícone dentro de <see cref="ModPrefab.AssetsFolder"/>.</summary>
        public virtual string IconFileName => $"{ClassID}.png";

        /// <summary>Tamanho ocupado no inventário.</summary>
        public virtual Vector2int SizeInInventory => new Vector2int(1, 1);

        /// <summary>Se a receita já nasce liberada. Sobrescrito por 6 classes do FCS.</summary>
        public virtual bool UnlockedAtStart => true;

        /// <summary>Dados de entidade de mundo, para spawn. Nulo = não spawna sozinho.</summary>
        public virtual UWE.WorldEntityInfo EntityInfo => null;

        /// <summary>Mensagem ao descobrir o item pela primeira vez.</summary>
        public virtual string DiscoverMessage => null;
    }

    /// <summary>
    /// Item fabricável: soma receita e posição no PDA ao <see cref="Spawnable"/>.
    /// </summary>
    public abstract class Craftable : Spawnable
    {
        protected Craftable(string classId, string friendlyName, string description)
            : base(classId, friendlyName, description) { }

        public abstract TechGroup GroupForPDA { get; }

        public abstract TechCategory CategoryForPDA { get; }

        /// <summary>Tecnologia que precisa estar liberada. <c>None</c> = já disponível.</summary>
        public virtual TechType RequiredForUnlock => TechType.None;

        /// <summary>Caminho de abas dentro do fabricador.</summary>
        public virtual string[] StepsToFabricatorTab => null;

        public virtual CraftTree.Type FabricatorType => CraftTree.Type.Fabricator;

        public virtual float CraftingTime => 1f;

        /// <summary>A receita. É o membro que toda subclasse fabricável implementa.</summary>
        // Qualificado por extenso: o jogo moderno tem um `TechData` ESTÁTICO no namespace
        // global, que ganha da resolução por `using`. Outra quebra que o código legado
        // encontra ao ser recompilado contra o jogo atual.
        protected abstract SMLHelper.V2.Crafting.TechData GetBlueprintRecipe();

        /// <summary>
        /// Traduz os membros acima para os gadgets do Nautilus. É o ponto onde a API
        /// por herança do SMLHelper vira a API por composição do Nautilus.
        /// </summary>
        protected override void ConfigurePrefab(CustomPrefab prefab)
        {
            base.ConfigurePrefab(prefab);

            var recipe = GetBlueprintRecipe();
            if (recipe != null)
            {
                var crafting = prefab.SetRecipe(recipe)
                    .WithCraftingTime(CraftingTime)
                    .WithFabricatorType(FabricatorType);

                if (StepsToFabricatorTab != null && StepsToFabricatorTab.Length > 0)
                    crafting.WithStepsToFabricatorTab(StepsToFabricatorTab);
            }

            prefab.SetPdaGroupCategory(GroupForPDA, CategoryForPDA);

            if (RequiredForUnlock != TechType.None)
                prefab.SetUnlock(RequiredForUnlock);
        }
    }

    /// <summary>
    /// Estrutura construível com a ferramenta de construção. No SMLHelper, <c>Buildable</c>
    /// derivava de <c>Craftable</c> — a diferença é a receita ir para a árvore de construção.
    /// </summary>
    public abstract class Buildable : Craftable
    {
        protected Buildable(string classId, string friendlyName, string description)
            : base(classId, friendlyName, description) { }

        public override CraftTree.Type FabricatorType => CraftTree.Type.Constructor;

        public override TechGroup GroupForPDA => TechGroup.Miscellaneous;

        public override TechCategory CategoryForPDA => TechCategory.Misc;
    }
}
