using System.Collections.Generic;

namespace SMLHelper.V2.Handlers
{
    /// <summary>
    /// Árvore de fabricação. Encaminha para o <c>Nautilus.Handlers.CraftTreeHandler</c>.
    ///
    /// A única diferença de assinatura é o ícone: o legado passa <c>Atlas.Sprite</c>, que
    /// não existe mais no jogo. A conversão implícita do nosso <see cref="Atlas.Sprite"/>
    /// resolve na borda, então a fonte legada não muda.
    /// </summary>
    public static class CraftTreeHandler
    {
        public static void AddTabNode(CraftTree.Type craftTree, string name, string displayName, Atlas.Sprite sprite)
            => Nautilus.Handlers.CraftTreeHandler.AddTabNode(craftTree, name, displayName, sprite);

        public static void AddTabNode(CraftTree.Type craftTree, string name, string displayName, Atlas.Sprite sprite, params string[] stepsToTab)
            => Nautilus.Handlers.CraftTreeHandler.AddTabNode(craftTree, name, displayName, sprite, stepsToTab);

        public static void AddCraftingNode(CraftTree.Type craftTree, TechType techType)
            => Nautilus.Handlers.CraftTreeHandler.AddCraftingNode(craftTree, techType);

        public static void AddCraftingNode(CraftTree.Type craftTree, TechType techType, params string[] stepsToTab)
            => Nautilus.Handlers.CraftTreeHandler.AddCraftingNode(craftTree, techType, stepsToTab);

        public static void RemoveNode(CraftTree.Type craftTree, params string[] stepsToNode)
            => Nautilus.Handlers.CraftTreeHandler.RemoveNode(craftTree, stepsToNode);
    }

    /// <summary>Desbloqueios e entradas de análise. Encaminha para o Nautilus.</summary>
    public static class KnownTechHandler
    {
        public static void UnlockOnStart(TechType techType)
            => Nautilus.Handlers.KnownTechHandler.UnlockOnStart(techType);

        public static void SetAnalysisTechEntry(TechType techTypeToBeAnalysed, IEnumerable<TechType> techTypesToUnlock)
            => Nautilus.Handlers.KnownTechHandler.SetAnalysisTechEntry(techTypeToBeAnalysed, techTypesToUnlock);

        public static void SetAnalysisTechEntry(TechType techTypeToBeAnalysed, IEnumerable<TechType> techTypesToUnlock, string unlockMessage)
            => Nautilus.Handlers.KnownTechHandler.SetAnalysisTechEntry(techTypeToBeAnalysed, techTypesToUnlock, unlockMessage);

        public static void AddRequirementForUnlock(TechType techType, TechType requirement)
            => Nautilus.Handlers.KnownTechHandler.AddRequirementForUnlock(techType, requirement);
    }

    /// <summary>
    /// Quanta energia um item rende no bio-reator.
    ///
    /// ⚠️ Este é o único handler aqui que **não** tem equivalente no Nautilus — conferido
    /// no metadata: não existe nenhum tipo com "BioReactor" no nome lá. O que existe é a
    /// tabela do próprio jogo, <c>BaseBioReactor.charge</c>, que é um dicionário estático
    /// público. O SMLHelper escrevia nela; nós escrevemos também, que é a mesma coisa
    /// que ele fazia — não uma aproximação.
    /// </summary>
    public static class BioReactorHandler
    {
        public static void SetBioReactorCharge(TechType techType, float charge)
        {
            if (BaseBioReactor.charge == null) return;
            BaseBioReactor.charge[techType] = charge;
        }
    }
}
