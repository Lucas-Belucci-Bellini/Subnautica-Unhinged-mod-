using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;

// Le os atributos [HarmonyPatch] do assembly compilado (nao do fonte: no fonte
// `typeof` e `nameof` ainda sao texto) e tenta resolver cada alvo nas assemblies
// do jogo, do mesmo jeito que o HarmonyX faz ao carregar.
internal static class Program
{
    private static int Main(string[] args)
    {
        if (args.Length < 2)
        {
            Console.Error.WriteLine("uso: VerificarPatches <alvo.dll> <pasta-de-refs>...");
            return 2;
        }

        var alvo = args[0];
        var pastas = args.Skip(1).ToArray();

        var arquivos = new List<string> { alvo };
        foreach (var pasta in pastas)
            arquivos.AddRange(Directory.GetFiles(pasta, "*.dll", SearchOption.AllDirectories));

        // Um mesmo nome simples pode aparecer em varias pastas; a primeira vence.
        var porNome = new Dictionary<string, string>(StringComparer.OrdinalIgnoreCase);
        foreach (var f in arquivos)
        {
            var nome = Path.GetFileNameWithoutExtension(f);
            if (!porNome.ContainsKey(nome)) porNome[nome] = f;
        }

        var resolver = new PathAssemblyResolver(porNome.Values);
        using var ctx = new MetadataLoadContext(resolver, "mscorlib");

        var asm = ctx.LoadFromAssemblyPath(alvo);

        // Modo membro: confere alvos imperativos (AccessTools.Method/Field), que nao
        // tem atributo nenhum e por isso escapam da varredura de [HarmonyPatch].
        // Uso: MEMBROS="Tipo::Membro,Tipo::Membro" — resolve nas assemblies do jogo.
        var membros = Environment.GetEnvironmentVariable("MEMBROS");
        if (!string.IsNullOrWhiteSpace(membros))
        {
            int mau = 0;
            foreach (var par in membros.Split(',', StringSplitOptions.RemoveEmptyEntries))
            {
                var pedaco = par.Trim().Split("::");
                var t = ctx.LoadFromAssemblyName("Assembly-CSharp").GetType(pedaco[0]);
                if (t == null) { Console.WriteLine($"  ✗ tipo {pedaco[0]} NAO EXISTE"); mau++; continue; }
                var achou = t.GetMembers(TodosOsFlags).Where(x => x.Name == pedaco[1]).ToArray();
                if (achou.Length == 0) { Console.WriteLine($"  ✗ {pedaco[0]}::{pedaco[1]} NAO EXISTE"); mau++; }
                else foreach (var a in achou)
                {
                    var extra = a is FieldInfo fi ? (fi.IsStatic ? " (campo estatico)" : " (campo de instancia)")
                              : a is MethodInfo mi ? (mi.IsStatic ? " (metodo estatico)" : " (metodo de instancia)") : "";
                    Console.WriteLine($"  ✓ {pedaco[0]}::{pedaco[1]} existe — {a.MemberType}{extra}");
                }
            }
            return mau == 0 ? 0 : 1;
        }

        var achados = new List<Achado>();
        foreach (var tipo in TiposDe(asm))
        {
            var doTipo = LerPatch(tipo.GetCustomAttributesData());
            foreach (var m in tipo.GetMethods(BindingFlags.Public | BindingFlags.NonPublic
                                              | BindingFlags.Static | BindingFlags.Instance
                                              | BindingFlags.DeclaredOnly))
            {
                var doMetodo = LerPatch(m.GetCustomAttributesData());
                if (doTipo == null && doMetodo == null) continue;

                // Regra do proprio Harmony: dentro de uma classe com [HarmonyPatch],
                // so vira patch o metodo anotado com [HarmonyPrefix]/[HarmonyPostfix]/
                // [HarmonyTranspiler]/[HarmonyFinalizer] ou chamado assim. Metodo
                // auxiliar solto na mesma classe e ignorado — contar esses fazia o
                // verificador reportar 17 "indeterminados" que nunca foram patch.
                if (!EhPatch(m)) continue;

                // Harmony combina o atributo da classe com o do metodo.
                var combinado = Info.Combinar(doTipo, doMetodo);
                if (combinado?.TipoAlvo == null && combinado?.NomeMetodo == null) continue;

                achados.Add(new Achado(tipo.FullName, m.Name, combinado));
            }

            // Classe com [HarmonyPatch] mas sem metodo anotado: os nomes por convencao.
            if (doTipo?.TipoAlvo != null && !achados.Any(a => a.Patch == tipo.FullName))
            {
                foreach (var m in tipo.GetMethods(BindingFlags.Public | BindingFlags.NonPublic
                                                  | BindingFlags.Static | BindingFlags.DeclaredOnly))
                {
                    if (EhPatch(m)) achados.Add(new Achado(tipo.FullName, m.Name, doTipo));
                }
            }
        }

        int ok = 0;
        var quebrados = new List<string>();
        var indefinidos = new List<string>();

        foreach (var a in achados.DistinctBy(a => (a.Patch, a.Metodo)))
        {
            var t = a.Patch2.TipoAlvo;
            if (t == null) { indefinidos.Add($"{a.Patch}.{a.Metodo}: sem tipo alvo resolvivel"); continue; }

            var nome = a.Patch2.NomeMetodo;
            if (a.Patch2.EhConstrutor)
            {
                if (t.GetConstructors(TodosOsFlags).Any()) ok++;
                else quebrados.Add($"{a.Patch}.{a.Metodo}: {t.Name} nao tem construtor");
                continue;
            }
            if (nome == null) { indefinidos.Add($"{a.Patch}.{a.Metodo}: sem nome de metodo"); continue; }

            var candidatos = t.GetMembers(TodosOsFlags)
                              .Where(x => x.Name == nome
                                       || x.Name == "get_" + nome || x.Name == "set_" + nome)
                              .ToArray();

            if (candidatos.Length == 0)
                quebrados.Add($"{a.Patch}.{a.Metodo}  ->  {t.FullName}::{nome}  NAO EXISTE");
            else ok++;
        }

        // Contagem crua ANTES do Distinct: sobrecarga de mesmo nome (varios `Postfix`
        // na mesma classe) colapsa, e um total menor que o do fonte precisa ser
        // explicado, nao aceito.
        Console.WriteLine($"metodos de patch (cru) : {achados.Count}");
        Console.WriteLine($"tipos com [HarmonyPatch]: {TiposDe(asm).Count(t => t.GetCustomAttributesData().Any(x => x.AttributeType.Name == "HarmonyPatch"))}");
        Console.WriteLine($"patches examinados : {achados.DistinctBy(a => (a.Patch, a.Metodo)).Count()}");
        Console.WriteLine($"alvo resolvido     : {ok}");
        Console.WriteLine($"alvo INEXISTENTE   : {quebrados.Count}");
        Console.WriteLine($"indeterminado      : {indefinidos.Count}");

        if (Environment.GetEnvironmentVariable("LISTAR") == "1")
        {
            Console.WriteLine("\n=== CLASSES DE PATCH NO ASSEMBLY ===");
            foreach (var g in achados.GroupBy(a => a.Patch).OrderBy(g => g.Key))
                Console.WriteLine($"  {g.Key}  [{string.Join(", ", g.Select(x => x.Metodo))}]");
        }

        if (quebrados.Count > 0)
        {
            Console.WriteLine("\n=== ALVOS QUE NAO EXISTEM NO JOGO ATUAL ===");
            foreach (var q in quebrados.OrderBy(x => x)) Console.WriteLine("  " + q);
        }
        if (indefinidos.Count > 0)
        {
            Console.WriteLine("\n=== INDETERMINADOS (conferir a mao) ===");
            foreach (var q in indefinidos.OrderBy(x => x).Take(40)) Console.WriteLine("  " + q);
        }
        return quebrados.Count == 0 ? 0 : 1;
    }

    private const BindingFlags TodosOsFlags =
        BindingFlags.Public | BindingFlags.NonPublic | BindingFlags.Static |
        BindingFlags.Instance | BindingFlags.FlattenHierarchy;

    private static readonly string[] Anotacoes =
        { "HarmonyPrefix", "HarmonyPostfix", "HarmonyTranspiler", "HarmonyFinalizer",
          "HarmonyReversePatch" };

    private static bool EhPatch(MethodInfo m)
        => m.Name is "Prefix" or "Postfix" or "Transpiler" or "Finalizer"
        || m.GetCustomAttributesData().Any(a => Anotacoes.Contains(a.AttributeType.Name));

    private static IEnumerable<Type> TiposDe(Assembly a)
    {
        try { return a.GetTypes(); }
        catch (ReflectionTypeLoadException ex) { return ex.Types.Where(t => t != null); }
    }

    private sealed record Achado(string Patch, string Metodo, Info Patch2);

    private sealed class Info
    {
        public Type TipoAlvo;
        public string NomeMetodo;
        public bool EhConstrutor;

        public static Info Combinar(Info a, Info b)
        {
            if (a == null) return b;
            if (b == null) return a;
            return new Info
            {
                TipoAlvo = b.TipoAlvo ?? a.TipoAlvo,
                NomeMetodo = b.NomeMetodo ?? a.NomeMetodo,
                EhConstrutor = a.EhConstrutor || b.EhConstrutor,
            };
        }
    }

    private static Info LerPatch(IList<CustomAttributeData> attrs)
    {
        Info info = null;
        foreach (var at in attrs)
        {
            if (at.AttributeType.Name != "HarmonyPatch") continue;
            info ??= new Info();
            foreach (var arg in at.ConstructorArguments)
            {
                var tn = arg.ArgumentType.FullName;
                if (tn == "System.Type" && arg.Value is Type t) info.TipoAlvo ??= t;
                else if (tn == "System.String" && arg.Value is string s && s.Length > 0)
                    info.NomeMetodo ??= s;
                else if (arg.ArgumentType.Name == "MethodType")
                {
                    // MethodType.Constructor == 1, StaticConstructor == 2
                    var v = Convert.ToInt32(arg.Value);
                    if (v is 1 or 2) info.EhConstrutor = true;
                }
            }
        }
        return info;
    }
}
