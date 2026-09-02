using System.Collections.Generic;
using System.IO;
using System.Reflection;
using FCS_AlterraHub.Buildables;
using FCS_AlterraHub.Configuration;
using FCS_AlterraHub.Registration;
using FCSCommon.Utilities;
using SMLHelper.V2.Utility;
using UnityEngine;

namespace FCS_AlterraHub.API
{
    public interface IFcAssetBundlesService
    {
        AssetBundle GetAssetBundleByName(string bundleName);
        Sprite GetIconByName(string iconName);
        AssetBundle GetAssetBundleByName(string bundleName,string executingFolder);
        string GlobalBundleName { get;}
        Texture2D GetEncyclopediaTexture2D(string imageName, string globalBundle = "");
        GameObject GetPrefabByName(string item, string bundle, bool applyShaders = true);
        string GetBundleByModID(string modID);
    }

    public class FCSAssetBundlesService : IFcAssetBundlesService
    {

        public static IFcAssetBundlesService PublicAPI { get; } = new FCSAssetBundlesService();

        // PORTE — tem de casar com o `Mod.GetModDirectory()`, senao o bundle e procurado
        // num caminho e documentado em outro.
        private static string ExecutingFolder { get; } =
            UnhingedModPaths.ModuleFolder(Assembly.GetExecutingAssembly(), "FCS_AlterraHub");

        private static readonly Dictionary<string, AssetBundle> loadedAssetBundles = new();
        private static readonly Dictionary<string, Sprite> loadedIcons = new();
        private static readonly Dictionary<string, Texture2D> loadedImages = new();
        private static readonly Dictionary<string, GameObject> loadedPrefabs = new();
        public string GlobalBundleName => Mod.AssetBundleName;

        /// <summary>
        /// De qual módulo é um bundle. Necessário porque a sobrecarga que recebe
        /// `executingFolder` é chamada pelos outros seis módulos, e o localizador
        /// precisa saber qual subpasta (`FCS_EnergySolutions`, …) procurar.
        /// </summary>
        private static string ModuloDoBundle(string bundleName)
        {
            switch ((bundleName ?? string.Empty).ToLowerInvariant())
            {
                case "fcsalterrahubbundle":          return "FCS_AlterraHub";
                case "fcsenergysolutionsbundle":     return "FCS_EnergySolutions";
                case "fcshomesolutionsbundle":       return "FCS_HomeSolutions";
                case "fcslifesupportsolutionsbundle":return "FCS_LifeSupportSolutions";
                case "fcsproductionsolutionsbundle": return "FCS_ProductionSolutions";
                case "fcsstoragesolutionsbundle":    return "FCS_StorageSolutions";
                case "cyclopsupgradeconsolebundle":  return "FCS_CyclopsUpgradeConsole";
                default:                             return null;
            }
        }
        public Texture2D GetEncyclopediaTexture2D(string imageName, string bundleName = "")
        {
            QuickLogger.Debug($"Trying to find {imageName} in bundle {bundleName}");
            AssetBundle bundle = null;

            if (string.IsNullOrWhiteSpace(imageName)) return null;

            if (string.IsNullOrWhiteSpace(bundleName))
            {
                bundleName = GlobalBundleName;
            }

            if (loadedImages.ContainsKey(imageName)) return loadedImages[imageName];

            QuickLogger.Debug($"Image {imageName} not already loaded. Trying to locate in bundle {bundleName}");

            if (loadedAssetBundles.TryGetValue(bundleName, out AssetBundle preLoadedBundle))
            {
                bundle =  preLoadedBundle;
            }

            if (bundle == null)
            {
                QuickLogger.Debug("Bundle returned null. Getting Image failed");
                return null;
            }

            var image = bundle.LoadAsset<Texture2D>(imageName);
            if (image == null)
            {
                QuickLogger.DebugError($"Failed to find image {imageName} in bundle {bundleName}");
                return null;
            }

            loadedImages.Add(imageName, image);
            return loadedImages[imageName];
        }

        public GameObject GetPrefabByName(string item, string bundleName, bool applyShaders = true)
        {
            if (loadedPrefabs.ContainsKey(item))
            {
                return loadedPrefabs[item];
            }

            var bundle = GetAssetBundleByName(bundleName);
            if (bundle == null) return null;
            AlterraHub.LoadAsset(item, bundle, out var go);

            if (applyShaders)
            {
                AlterraHub.ReplaceShadersV2(go);
            }

            loadedPrefabs.Add(item,go);
            return go;
        }

        public string GetBundleByModID(string modID)
        {
            var modPackData = FCSAlterraHubService.InternalAPI.GetRegisteredModData(modID);
            return modPackData != null ? modPackData.ModBundleName : string.Empty;
        }

        private FCSAssetBundlesService()
        {
        }

        public AssetBundle GetAssetBundleByName(string bundleName)
        {
            if (loadedAssetBundles.TryGetValue(bundleName, out AssetBundle preLoadedBundle))
            {
                return preLoadedBundle;
            }

            var onDemandBundle = CarregarBundle(bundleName, ExecutingFolder, "FCS_AlterraHub");

            if (onDemandBundle != null)
            {
                loadedAssetBundles.Add(bundleName, onDemandBundle);
                return onDemandBundle;
            }

            return null;
        }

        /// <summary>
        /// Carrega um bundle procurando nos layouts de instalação possíveis, e registra
        /// o resultado no relatório de diagnóstico.
        /// </summary>
        /// <remarks>
        /// ⚠️ <b>Esta é a diferença entre "o item aparece" e "o item funciona".</b> Todo
        /// prefab do FCS sai de um destes sete bundles. Sem o arquivo, o TechType e a
        /// receita existem, o item aparece no PDA e no construtor — e não faz nada,
        /// porque não há modelo, nem componente, nem comportamento por trás dele.
        ///
        /// O original tentava UM caminho (<c>&lt;pasta&gt;/Assets/&lt;bundle&gt;</c>) e
        /// devolvia null calado. Agora tenta os layouts reais — inclusive a pasta
        /// <c>QMods/</c> do QModManager, que é onde os arquivos já estão em quem usava o
        /// FCS antes — e, quando falha, diz no relatório EXATAMENTE onde procurou.
        /// </remarks>
        internal static AssetBundle CarregarBundle(string bundleName, string pastaFallback, string nomeDoModulo)
        {
            var caminho = UnhingedModPaths.LocalizarBundle(
                Assembly.GetExecutingAssembly(), nomeDoModulo, bundleName);

            // Sem candidato conhecido, ainda vale tentar o caminho classico: o modulo
            // pode ter recebido uma pasta por outro meio.
            if (caminho == null && !string.IsNullOrEmpty(pastaFallback))
                caminho = Path.Combine(Path.Combine(pastaFallback, "Assets"), bundleName);

            AssetBundle bundle = null;
            if (caminho != null && File.Exists(caminho))
                bundle = AssetBundle.LoadFromFile(caminho);

            Unhinged.Legacy.Diagnostico.RegistroDeConteudo.AnotarBundle(
                nomeDoModulo, bundleName, bundle != null, caminho);

            return bundle;
        }

        public Sprite GetIconByName(string iconName)
        {
            if (loadedIcons.TryGetValue(iconName, out Sprite preLoadedBundle))
            {
                return preLoadedBundle;
            }
            
            Texture2D texture = ImageUtils.LoadTextureFromFile(Path.Combine(Mod.GetAssetPath(), $"{iconName}.png"));

            if (texture != null)
            {
                var icon = Sprite.Create(texture, new Rect(0.0f, 0.0f, texture.width, texture.height), new Vector2(0.5f, 0.5f));
                loadedIcons.Add(iconName, icon);
                return icon;
            }

            return null;
        }

        public AssetBundle GetAssetBundleByName(string bundleName, string executingFolder)
        {
            if (loadedAssetBundles.TryGetValue(bundleName, out AssetBundle preLoadedBundle))
            {
                return preLoadedBundle;
            }

            var onDemandBundle = CarregarBundle(bundleName, executingFolder, ModuloDoBundle(bundleName));

            if (onDemandBundle != null)
            {
                loadedAssetBundles.Add(bundleName, onDemandBundle);
                return onDemandBundle;
            }

            return null;
        }
    }
}
