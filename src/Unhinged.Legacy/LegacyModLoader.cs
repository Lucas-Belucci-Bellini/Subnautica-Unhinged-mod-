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
        /// <param name="filtro">
        /// Decide se um <c>[QModCore]</c> deve rodar. Devolver <c>false</c> pula aquele
        /// módulo inteiro — é o que permite desligar um dos sete módulos do pacote sem
        /// recompilar. <c>null</c> roda todos.
        /// </param>
        /// <returns>Quantos métodos de entrada rodaram sem erro.</returns>
        public static int Run(Assembly assembly, ManualLogSource log = null,
            Func<Type, int> prioridade = null, Func<Type, bool> filtro = null)
        {
            if (assembly == null) throw new ArgumentNullException(nameof(assembly));

            var cores = SafeGetTypes(assembly, log)
                .Where(t => t.GetCustomAttribute<QModCoreAttribute>() != null)
                .ToList();

            // Filtrar ANTES de ordenar e de invocar: um modulo desligado nao deve nem
            // aparecer na contagem de "pontos de entrada executados", senao o numero no
            // log mente sobre o que realmente rodou.
            if (filtro != null)
            {
                var antes = cores.Count;
                cores = cores.Where(filtro).ToList();
                if (cores.Count != antes)
                    log?.LogInfo($"{antes - cores.Count} modulo(s) desligado(s) na configuracao.");
            }

            // A ordem importa e o `GetTypes()` NAO a garante. Quando varios mods legados
            // moram no mesmo assembly (e o caso do pacote Alterra Hub, com 7 modulos), um
            // deles costuma registrar os servicos que os outros consomem. Sem ordenar, o
            // resultado varia por build — e falha de forma dificil de reproduzir.
            if (prioridade != null)
                cores = cores.OrderBy(prioridade).ThenBy(t => t.FullName, StringComparer.Ordinal).ToList();

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
                    // O nome do modulo e o primeiro segmento do namespace — o mesmo
                    // criterio usado pelo ModPrefab, para as duas metades do relatorio
                    // falarem do mesmo "modulo".
                    var modulo = type.Namespace?.Split('.')[0] ?? type.Name;
                    var fase = typeof(TAttribute).Name.Replace("QMod", "").Replace("Attribute", "");

                    Diagnostico.RegistroDeConteudo.AnotarModulo(modulo, fase, "entrou");
                    try
                    {
                        // Os pontos de entrada do QMod eram estáticos e sem parâmetros.
                        // Um método de instância aqui é código malformado, não um caso a suportar.
                        var target = method.IsStatic ? null : Activator.CreateInstance(type);
                        method.Invoke(target, null);
                        executed++;
                        Diagnostico.RegistroDeConteudo.AnotarModulo(modulo, fase, "concluiu");
                    }
                    catch (Exception ex)
                    {
                        // TargetInvocationException esconde a causa real; desembrulhar
                        // é o que torna o log utilizável.
                        var cause = (ex as TargetInvocationException)?.InnerException ?? ex;
                        log?.LogError($"{type.FullName}.{method.Name} falhou: {cause}");

                        // ⚠️ E TAMBEM no arquivo de registro. Este catch era o unico
                        // lugar que sabia por que um modulo inteiro sumiu, e mandava a
                        // resposta so para o LogOutput.log — dezenas de milhares de
                        // linhas de todos os mods, que ninguem garimpa. O relatorio do
                        // operador mostrou 1 item de 7 modulos sem uma palavra sobre os
                        // outros seis, porque a explicacao estava do outro lado.
                        Diagnostico.RegistroDeConteudo.AnotarModulo(modulo, fase, "ABORTOU", cause);
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
