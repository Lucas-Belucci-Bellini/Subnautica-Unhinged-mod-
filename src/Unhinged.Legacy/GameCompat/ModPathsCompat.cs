using System;
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
///
/// A resposta é dar a cada módulo uma **subpasta ao lado do DLL**, com o mesmo nome que
/// a pasta dele tinha no QMods. O layout de instalação passa a espelhar o original, e
/// as pastas não se misturam.
/// </summary>
public static class UnhingedModPaths
{
    /// <summary>
    /// Pasta do módulo, ao lado do assembly: <c>&lt;pasta do DLL&gt;/&lt;nomeDoModulo&gt;</c>.
    ///
    /// Se essa subpasta não existir, devolve a pasta do próprio DLL. Esse retorno não é
    /// desistência: é o layout "achatado", em que alguém juntou tudo numa pasta só.
    /// Aceitar os dois evita quebrar quem já instalou de um jeito.
    /// </summary>
    public static string ModuleFolder(Assembly assembly, string nomeDoModulo)
    {
        var raiz = AssemblyFolder(assembly);
        if (raiz == null || string.IsNullOrEmpty(nomeDoModulo)) return raiz;

        var subpasta = Path.Combine(raiz, nomeDoModulo);
        return Directory.Exists(subpasta) ? subpasta : raiz;
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
