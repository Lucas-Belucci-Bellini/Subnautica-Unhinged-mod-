using System;
using System.IO;
using System.Reflection;

namespace CyclopsUpgradeConsole.Configuration
{
    internal static class Mod
    {
        internal const string BundleName = "cyclopsupgradeconsolebundle";
        internal const string ModTabID = "CUC";
        internal const string ModFriendlyName = "Cyclops Upgrade Console";
        internal const string ModName = "CyclopsUpgradeConsole";
        internal static string CyclopsUpgradeConsoleKitClassID => $"{ModName}_Kit";
        internal static string ModClassName => ModName;
        internal static string ModPrefabName => ModName;
        internal static string ModFolderName => $"FCS_{ModName}";
        
        internal const string ModDescription = "A wall mountable upgrade console to connect a greater number of upgrades to your Cyclops.";


        // PORTE — o original montava `<CWD>/QMods/FCS_CyclopsUpgradeConsole/Assets`,
        // caminho fixo do QModManager. Numa instalacao so de BepInEx essa pasta NAO
        // EXISTE, e o modulo ficava sem asset nenhum — sem erro, sem log, so sem
        // modelo. Agora resolve ao lado do DLL, como os outros seis.
        private static string GetModPath()
        {
            return UnhingedModPaths.ModuleFolder(Assembly.GetExecutingAssembly(), ModFolderName);
        }

        internal static string GetAssetFolder()
        {
            return Path.Combine(GetModPath(), "Assets");
        }
    }
}