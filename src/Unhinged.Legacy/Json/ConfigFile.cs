namespace SMLHelper.V2.Json
{
    /// <summary>
    /// Equivale ao <c>SMLHelper.V2.Json.ConfigFile</c>.
    ///
    /// A API do Nautilus é praticamente a mesma (<c>Load</c>, <c>Save</c>,
    /// <c>JsonFilePath</c>), então herdar repassa tudo sem intermediário — e o código
    /// legado, que escreve <c>class Config : ConfigFile</c>, continua igual.
    /// </summary>
    public abstract class ConfigFile : Nautilus.Json.ConfigFile
    {
    }
}
