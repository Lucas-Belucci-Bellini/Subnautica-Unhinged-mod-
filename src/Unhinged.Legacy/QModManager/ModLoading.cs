using System;

namespace QModManager.API.ModLoading
{
    /// <summary>
    /// Marca a classe de entrada de um mod legado. No QModManager isto era lido pelo
    /// carregador; aqui serve para o código compilar e para o
    /// <see cref="Unhinged.Legacy.LegacyModLoader"/> encontrar a classe por reflexão.
    /// </summary>
    [AttributeUsage(AttributeTargets.Class, Inherited = false)]
    public sealed class QModCoreAttribute : Attribute { }

    /// <summary>Método chamado na fase de patch. Equivale ao <c>Awake</c> do BepInEx.</summary>
    [AttributeUsage(AttributeTargets.Method, Inherited = false)]
    public sealed class QModPatchAttribute : Attribute { }

    /// <summary>Método chamado antes do patch.</summary>
    [AttributeUsage(AttributeTargets.Method, Inherited = false)]
    public sealed class QModPrePatchAttribute : Attribute { }

    /// <summary>Método chamado depois do patch.</summary>
    [AttributeUsage(AttributeTargets.Method, Inherited = false)]
    public sealed class QModPostPatchAttribute : Attribute { }
}
