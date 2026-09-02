using System;
using System.Collections.Generic;
using System.IO;
using System.Reflection;

// SEM `namespace`, como os outros compat: vale em todo arquivo, sem `using`.

/// <summary>
/// Onde um mod legado acha os arquivos dele (Assets, bundles, configuração).
///
/// Dois problemas que o porte cria, e que este helper resolve:
///
/// **1. Fundir módulos num DLL só faz todos apontarem para a mesma pasta.** O código
/// legado usa <c>Path.GetDirectoryName(Assembly.GetExecutingAssembly().Location)</c>.
/// Quando cada módulo era um DLL numa pasta própria, isso dava a pasta daquele módulo.
/// Fundidos num assembly, os sete passam a devolver o MESMO caminho — e as sete pastas
/// <c>Assets</c> teriam de ser mescladas à mão, com colisão silenciosa de nome de
/// arquivo entre módulos.
///
/// **2. Caminho fixo de QMods.** Um dos módulos monta
/// <c>&lt;CWD&gt;/QMods/&lt;mod&gt;/Assets</c> — o layout do QModManager. Numa instalação
/// só de BepInEx essa pasta não existe, e o módulo fica sem asset nenhum sem dizer nada.
/// </summary>
public static class UnhingedModPaths
{
    /// <summary>
    /// Toda tentativa de localizar pasta ou bundle, na ordem. Serve ao diagnóstico:
    /// "não achei" sem dizer ONDE procurou é um beco sem saída para quem instala.
    /// </summary>
    public static readonly List<string> Tentativas = new List<string>();

    private static void Nota(string linha)
    {
        lock (Tentativas)
            if (Tentativas.Count < 200) Tentativas.Add(linha);
    }

    /// <summary>
    /// Pasta do módulo. Procura, em ordem, os layouts que uma instalação real pode ter.
    /// </summary>
    /// <remarks>
    /// ⚠️ A ordem NÃO é arbitrária, e a última entrada é a que mais importa na prática:
    /// quem usava o FCS antes o tinha sob <c>QMods/</c>, pelo QModManager. Esses
    /// arquivos continuam no disco depois de migrar para o BepInEx — então procurar lá
    /// faz o mod funcionar sem que ninguém copie nada. É leitura da instalação do
    /// próprio operador, não redistribuição.
    /// </remarks>
    public static string ModuleFolder(Assembly assembly, string nomeDoModulo)
    {
        var raiz = AssemblyFolder(assembly);
        if (raiz == null || string.IsNullOrEmpty(nomeDoModulo)) return raiz;

        foreach (var candidato in CandidatosDePasta(raiz, nomeDoModulo))
        {
            if (Directory.Exists(Path.Combine(candidato, "Assets")))
            {
                Nota("pasta " + nomeDoModulo + " -> " + candidato + "  [Assets/ existe]");
                return candidato;
            }
        }

        // Nenhum candidato tem Assets/. Devolver a pasta do DLL mantem o comportamento
        // antigo (e as mensagens de erro apontam para um caminho que faz sentido).
        Nota("pasta " + nomeDoModulo + " -> NAO ACHEI Assets/ em nenhum candidato; usando " + raiz);
        return raiz;
    }

    /// <summary>
    /// Caminho de um asset bundle, procurado em todos os layouts. <c>null</c> se não
    /// existir em lugar nenhum — e nesse caso as tentativas ficam em <see cref="Tentativas"/>.
    /// </summary>
    public static string LocalizarBundle(Assembly assembly, string nomeDoModulo, string nomeDoBundle)
    {
        var raiz = AssemblyFolder(assembly);
        if (raiz == null || string.IsNullOrEmpty(nomeDoBundle)) return null;

        foreach (var pasta in CandidatosDePasta(raiz, nomeDoModulo))
        {
            // Com e sem a subpasta Assets: ha empacotamentos dos dois jeitos.
            var comAssets = Path.Combine(Path.Combine(pasta, "Assets"), nomeDoBundle);
            if (File.Exists(comAssets)) { Nota("bundle " + nomeDoBundle + " -> " + comAssets); return comAssets; }

            var direto = Path.Combine(pasta, nomeDoBundle);
            if (File.Exists(direto)) { Nota("bundle " + nomeDoBundle + " -> " + direto); return direto; }
        }

        Nota("bundle " + nomeDoBundle + " -> NAO ENCONTRADO. Procurei em: "
             + string.Join(" | ", CandidatosDePasta(raiz, nomeDoModulo).ToArray()));
        return null;
    }

    /// <summary>Pastas onde um módulo pode estar, da mais específica para a mais genérica.</summary>
    private static List<string> CandidatosDePasta(string raiz, string nomeDoModulo)
    {
        var lista = new List<string>();

        if (!string.IsNullOrEmpty(nomeDoModulo))
            lista.Add(Path.Combine(raiz, nomeDoModulo));   // <dll>/FCS_AlterraHub
        lista.Add(raiz);                                    // <dll>            (layout achatado)

        // O layout ORIGINAL do QModManager, na instalacao do proprio operador.
        var jogo = RaizDoJogo(raiz);
        if (jogo != null && !string.IsNullOrEmpty(nomeDoModulo))
        {
            lista.Add(Path.Combine(Path.Combine(jogo, "QMods"), nomeDoModulo));
            // O Vortex/QMM as vezes usa o nome sem o prefixo FCS_.
            if (nomeDoModulo.StartsWith("FCS_", StringComparison.Ordinal))
                lista.Add(Path.Combine(Path.Combine(jogo, "QMods"), nomeDoModulo.Substring(4)));
        }

        return lista;
    }

    /// <summary>
    /// Sobe a partir da pasta do DLL até achar a raiz do jogo (a que contém `QMods`
    /// ou `BepInEx`). Sem depender do BepInEx.Paths, para o helper seguir testável fora
    /// do jogo.
    /// </summary>
    private static string RaizDoJogo(string partida)
    {
        try
        {
            var dir = new DirectoryInfo(partida);
            for (var i = 0; i < 6 && dir != null; i++, dir = dir.Parent)
            {
                if (Directory.Exists(Path.Combine(dir.FullName, "QMods")))
                    return dir.FullName;
            }
        }
        catch (Exception) { }
        return null;
    }

    /// <summary>Pasta onde o assembly está. <c>null</c> se não der para determinar.</summary>
    public static string AssemblyFolder(Assembly assembly)
    {
        try
        {
            var local = assembly?.Location;
            return string.IsNullOrEmpty(local) ? null : Path.GetDirectoryName(local);
        }
        catch
        {
            // Assembly carregado da memória não tem Location. Nada a fazer aqui além de
            // não derrubar o mod por causa de um caminho.
            return null;
        }
    }
}
