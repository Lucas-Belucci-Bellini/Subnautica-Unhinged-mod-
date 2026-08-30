using System;
using System.Linq;
using System.Reflection;
using BepInEx.Logging;
using QModManager.API.ModLoading;

namespace Unhinged.Legacy
{
    /// <summary>
    /// Executa o ponto de entrada de um mod legado a partir de um plugin BepInEx.
    ///
    /// É a peça que os atributos sozinhos não resolvem: <c>[QModCore]</c> e
    /// <c>[QModPatch]</c> só marcam código — quem os invocava era o QModManager, que não
    /// existe no ramo moderno. Este carregador ocupa esse lugar, respeitando a ordem
    /// original: pré-patch → patch → pós-patch.
    ///
    /// Uso, a partir do <c>Awake</c> de um plugin BepInEx:
    /// <code>
    /// LegacyModLoader.Run(typeof(AlgumTipoDoMod).Assembly, Logger);
    /// </code>
    /// </summary>
    public static class LegacyModLoader
    {
        /// <summary>
        /// Roda as três fases nos tipos marcados com <c>[QModCore]</c> do assembly.
        /// Uma falha num mod é registrada e não interrompe os demais — um mod legado
        /// quebrado não deve derrubar o carregamento inteiro.
        /// </summary>
        /// <returns>Quantos métodos de entrada rodaram sem erro.</returns>
        public static int Run(Assembly assembly, ManualLogSource log = null)
        {
            if (assembly == null) throw new ArgumentNullException(nameof(assembly));

            var cores = SafeGetTypes(assembly, log)
                .Where(t => t.GetCustomAttribute<QModCoreAttribute>() != null)
                .ToList();

            if (cores.Count == 0)
            {
                log?.LogWarning($"{assembly.GetName().Name}: nenhum tipo com [QModCore].");
                return 0;
            }

            var executed = 0;
            executed += InvokePhase<QModPrePatchAttribute>(cores, log);
            executed += InvokePhase<QModPatchAttribute>(cores, log);
            executed += InvokePhase<QModPostPatchAttribute>(cores, log);
            return executed;
        }

        private static int InvokePhase<TAttribute>(System.Collections.Generic.IEnumerable<Type> cores, ManualLogSource log)
            where TAttribute : Attribute
        {
            var executed = 0;

            foreach (var type in cores)
            {
                const BindingFlags flags = BindingFlags.Public | BindingFlags.NonPublic
                                           | BindingFlags.Static | BindingFlags.Instance;

                foreach (var method in type.GetMethods(flags)
                             .Where(m => m.GetCustomAttribute<TAttribute>() != null))
                {
                    try
                    {
                        // Os pontos de entrada do QMod eram estáticos e sem parâmetros.
                        // Um método de instância aqui é código malformado, não um caso a suportar.
                        var target = method.IsStatic ? null : Activator.CreateInstance(type);
                        method.Invoke(target, null);
                        executed++;
                    }
                    catch (Exception ex)
                    {
                        // TargetInvocationException esconde a causa real; desembrulhar
                        // é o que torna o log utilizável.
                        var cause = (ex as TargetInvocationException)?.InnerException ?? ex;
                        log?.LogError($"{type.FullName}.{method.Name} falhou: {cause}");
                    }
                }
            }

            return executed;
        }

        private static Type[] SafeGetTypes(Assembly assembly, ManualLogSource log)
        {
            try
            {
                return assembly.GetTypes();
            }
            catch (ReflectionTypeLoadException ex)
            {
                // Acontece quando parte do assembly referencia algo que não existe mais —
                // exatamente o cenário de um mod legado meio portado. Os tipos que
                // carregaram ainda servem.
                log?.LogWarning($"{assembly.GetName().Name}: {ex.LoaderExceptions?.Length ?? 0} tipo(s) não carregaram; seguindo com os demais.");
                return ex.Types?.Where(t => t != null).ToArray() ?? Array.Empty<Type>();
            }
        }
    }
}
