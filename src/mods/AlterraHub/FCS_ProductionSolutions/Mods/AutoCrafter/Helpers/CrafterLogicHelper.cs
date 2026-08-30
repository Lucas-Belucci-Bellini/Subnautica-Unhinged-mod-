using System.Collections.Generic;
using QModManager.API;
using SMLHelper.V2.Crafting;
using SMLHelper.V2.Handlers;
#if SUBNAUTICA
using RecipeData = SMLHelper.V2.Crafting.TechData;
#endif
namespace FCS_ProductionSolutions.Mods.AutoCrafter.Helpers
{
    // O jogo moderno tem um `TechData` ESTATICO no namespace global, e membro de
    // namespace ganha de `using` de topo de arquivo — sem este alias, o tipo do
    // SMLHelper fica invisivel aqui (CS0722/CS0576). Tem de ficar DENTRO do
    // `namespace`. Ver docs/PORTE-LEGADO.md secao 2.
    using TechData = SMLHelper.V2.Crafting.TechData;

    public static class CrafterLogicHelper
    {
        public static List<TechType> BlackList = new List<TechType>() { TechType.Titanium, TechType.Copper };

        public static bool IsItemUnlocked(TechType techType, bool useDefault = false)
        {
#if DEBUG
            QuickLogger.Debug($"Checking if {Language.main.Get(techType)} is unlocked");
#endif
            if (useDefault)
            {
                return CrafterLogic.IsCraftRecipeUnlocked(techType);
            }


            if (GameModeUtils.RequiresBlueprints())
            {
                if (!QModServices.Main.ModPresent("UITweaks"))
                {
                    RecipeData data = GetData(techType);
                    int ingredientCount = data?.ingredientCount ?? 0;
                    for (int i = 0; i < ingredientCount; i++)
                    {
                        Ingredient ingredient = data.Ingredients[i];
                        if (!BlackList.Contains(techType) && !CrafterLogic.IsCraftRecipeUnlocked(ingredient.techType))
                        {
#if DEBUG
                            QuickLogger.Debug($"{Language.main.Get(ingredient.techType)} is locked");
#endif
                            return false;
                        }
                    }
                }
                else
                {
#if SUBNAUTICA
                    // PORTE — o jogo atual nao tem mais o dicionario `CraftData.techData` nem o tipo
                    // aninhado `CraftData.TechData`. A receita passou a ser consultada por
                    // handler, e o `CraftDataHandler.GetTechData` da ponte devolve o mesmo
                    // formato (ingredientCount / GetIngredient), entao o corpo abaixo nao muda.
                    var data = CraftDataHandler.GetTechData(techType);
                    if (data != null)
                    {
                        int ingredientCount = data?.ingredientCount ?? 0;
                        for (int i = 0; i < ingredientCount; i++)
                        {
                            // `var`, nao `IIngredient`: o `GetIngredient` da ponte devolve o `Ingredient` do
                            // jogo, que expoe os mesmos `techType`/`amount` mas nao implementa a
                            // interface antiga do SMLHelper. Nada abaixo depende da interface.
                            var ingredient = data.GetIngredient(i);
                            if (!BlackList.Contains(techType) &&
                                !CrafterLogic.IsCraftRecipeUnlocked(ingredient.techType))
                            {
#if DEBUG
                                QuickLogger.Debug($"{Language.main.Get(techType)} is locked");
#endif
                                return false;
                            }
                        }
                    }
#elif BELOWZERO
#endif
                }
            }

#if DEBUG
            QuickLogger.Debug($"{Language.main.Get(techType)} is unlocked");
#endif
            return true;
        }

        internal static RecipeData GetData(TechType techType)
        {
            return CraftDataHandler.GetTechData(techType);
        }

        public static void Inc<T>(this Dictionary<T, int> dictionary, T key, int value = 1)
        {
            int num;
            dictionary.TryGetValue(key, out num);
            dictionary[key] = num + value;
        }
    }
}
