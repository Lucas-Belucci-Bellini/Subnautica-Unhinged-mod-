using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using HarmonyLib;

// SEM `namespace`, como os outros compat.

/// <summary>
/// Aplica só os patches de UM módulo, quando vários módulos moram no mesmo assembly.
///
/// ⚠️ Este é o defeito mais caro que a fusão dos módulos criou. Cada módulo do FCS
/// chamava <c>harmony.PatchAll(Assembly.GetExecutingAssembly())</c>. Quando cada um era
/// um assembly próprio, isso aplicava só os patches dele. Fundidos num assembly só,
/// <c>PatchAll</c> passa a varrer o assembly INTEIRO — então os 129 patches do pacote
/// eram aplicados **uma vez por módulo**, sete vezes, sob sete instâncias de Harmony
/// diferentes.
///
/// Não é só desperdício: são 36 prefixos e 37 postfixes rodando sete vezes cada em
/// métodos do jogo. Um prefixo que devolve <c>false</c> para pular o original passa a
/// ser avaliado sete vezes; um postfix que registra algo, registra sete.
///
/// A correção é patchear por namespace: cada módulo aplica só o que é dele.
/// </summary>
public static class UnhingedHarmony
{
    /// <summary>
    /// Aplica os patches cujo tipo está sob <paramref name="namespaceDoModulo"/>.
    /// </summary>
    /// <returns>Quantas classes de patch foram aplicadas.</returns>
    public static int PatchModule(this Harmony harmony, Assembly assembly, string namespaceDoModulo)
    {
        if (harmony == null || assembly == null || string.IsNullOrEmpty(namespaceDoModulo)) return 0;

        var aplicadas = 0;
        foreach (var tipo in TiposDe(assembly))
        {
            var ns = tipo.Namespace;
            if (ns == null) continue;

            // Prefixo exato ou seguido de ponto: "FCS_Home" não deve casar
            // "FCS_HomeSolutions", e vice-versa.
            if (!(ns.Equals(namespaceDoModulo, StringComparison.Ordinal) ||
                  ns.StartsWith(namespaceDoModulo + ".", StringComparison.Ordinal)))
                continue;

            // Num tipo sem atributo de patch isto é no-op — não precisa filtrar antes.
            var processados = harmony.CreateClassProcessor(tipo).Patch();
            if (processados != null && processados.Count > 0) aplicadas++;
        }

        return aplicadas;
    }

    /// <summary>
    /// Tipos do assembly, tolerando referência que não resolve. O pacote referencia
    /// DLLs que podem não estar instaladas (MoreCyclopsUpgrades, NAudio), e nesse caso
    /// <c>GetTypes()</c> lança — mas traz os tipos que carregaram junto com a exceção.
    /// Aproveitá-los é melhor do que desistir do módulo inteiro.
    /// </summary>
    private static IEnumerable<Type> TiposDe(Assembly assembly)
    {
        try
        {
            return assembly.GetTypes();
        }
        catch (ReflectionTypeLoadException ex)
        {
            return ex.Types?.Where(t => t != null) ?? Enumerable.Empty<Type>();
        }
    }
}
