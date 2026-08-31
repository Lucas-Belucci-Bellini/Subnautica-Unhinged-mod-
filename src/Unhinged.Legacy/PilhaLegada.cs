using System.Collections.Generic;
using System.Linq;
using BepInEx.Bootstrap;

namespace Unhinged.Legacy
{
    /// <summary>
    /// Detecta se a pilha LEGADA (QModManager + SMLHelper) está carregada junto com esta.
    ///
    /// Isto importa porque a ponte reimplementa os namespaces <c>SMLHelper.V2.*</c> sobre
    /// o Nautilus. Com o SMLHelper de verdade também carregado, os **dois** frameworks
    /// existem no mesmo processo e patcham os mesmos métodos do jogo (<c>CraftData</c>,
    /// <c>KnownTech</c>, <c>uGUI</c>…). O resultado não é um erro limpo: é
    /// comportamento indefinido, do tipo que trava o carregamento sem dizer por quê.
    ///
    /// Um mod portado que se recusa a rodar com uma mensagem clara é **melhor** do que um
    /// que roda e corrompe a carga do jogo. Silêncio aqui vira "simplesmente não funciona".
    /// </summary>
    public static class PilhaLegada
    {
        /// <summary>GUIDs da pilha legada, os mesmos usados pelo relatório de ambiente.</summary>
        private static readonly string[] Guids =
        {
            "QModManager.QMods",
            "com.ahk1221.smlhelper",
            "com.snmodding.smlhelper",
        };

        /// <summary>Nomes legíveis do que foi encontrado. Vazio = pilha legada ausente.</summary>
        public static IReadOnlyList<string> Detectar()
        {
            var plugins = Chainloader.PluginInfos;
            if (plugins == null) return new string[0];

            return Guids
                .Where(plugins.ContainsKey)
                .Select(g =>
                {
                    var meta = plugins[g]?.Metadata;
                    return meta == null ? g : $"{meta.Name} {meta.Version} [{g}]";
                })
                .ToList();
        }

        public static bool EstaPresente() => Detectar().Count > 0;
    }
}
