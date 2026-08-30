using FCS_AlterraHub.Extensions;
using FCS_HomeSolutions.Buildables;
using SMLHelper.V2.Crafting;
using UnityEngine;
#if SUBNAUTICA
using RecipeData = SMLHelper.V2.Crafting.TechData;
#endif

namespace FCS_HomeSolutions.Mods.LedLights.Buildable
{
    // O jogo moderno tem um `TechData` ESTATICO no namespace global, e membro de
    // namespace ganha de `using` de topo de arquivo — sem este alias, o tipo do
    // SMLHelper fica invisivel aqui (CS0722/CS0576). Tem de ficar DENTRO do
    // `namespace`. Ver docs/PORTE-LEGADO.md secao 2.
    using TechData = SMLHelper.V2.Crafting.TechData;

    internal class FloodLEDLight : LedLightPatch
    {
        public FloodLEDLight() : base(new LedLightData
        {
            classId = "FloodLEDLight",
            description = "A Flood Light for wide area illumination, suitable for exterior use. (Change the color with the Paint Tool)",
            friendlyName = "LED Flood Light",
            allowedInBase = false,
            allowedInSub = false,
            allowedOnGround = true,
            allowedOnWall = false,
            allowedOutside = true,
            categoryForPDA = TechCategory.ExteriorModule,
            groupForPda = TechGroup.ExteriorModules,
            size = Vector3.zero,
            center = Vector3.zero,
            prefab = ModelPrefab.GetPrefabFromGlobal("FCS_FloodLight")
        })
        {
        }
    }
}
