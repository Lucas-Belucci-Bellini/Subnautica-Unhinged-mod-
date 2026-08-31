using System;
using System.IO;
using System.Reflection;
using UnityEngine;

namespace CyclopsUpgradeConsole.Utilities
{
    internal static class CUCUtils
    {
        // PORTE — idem: subpasta do modulo, nao a pasta do DLL.
        public static string ModDirLocation =>
            UnhingedModPaths.ModuleFolder(Assembly.GetExecutingAssembly(), "FCS_CyclopsUpgradeConsole");
        public static AssetBundle Asset(string modBundleName)
        {
            if (string.IsNullOrWhiteSpace(ModDirLocation) && string.IsNullOrWhiteSpace(modBundleName))
            {
                throw new ArgumentException($"Both {nameof(ModDirLocation)} and {nameof(modBundleName)} are empty");
            }

            if (string.IsNullOrWhiteSpace(ModDirLocation) || string.IsNullOrWhiteSpace(modBundleName))
            {
                var result = string.IsNullOrWhiteSpace(ModDirLocation) ? nameof(ModDirLocation) : nameof(modBundleName);
                throw new ArgumentException($"{result} is empty");
            }

            return AssetBundle.LoadFromFile(Path.Combine(ModDirLocation, "Assets", modBundleName));
        }
    }
}
