using Newtonsoft.Json;

namespace SMLHelper.V2.Json.ExtensionMethods
{
    /// <summary>
    /// Equivale ao <c>SMLHelper.V2.Json.ExtensionMethods.JsonFileExtensions</c>.
    ///
    /// São métodos de extensão, então não dá para repassar por herança como no
    /// <see cref="SMLHelper.V2.Json.ConfigFile"/>: o jeito é declarar de novo e encaminhar
    /// para o Nautilus. A assinatura é a mesma dos dois lados.
    ///
    /// ⚠️ Não coloque <c>using Nautilus.Json.ExtensionMethods;</c> num arquivo legado que já
    /// usa este namespace — os dois trazem <c>SaveJson</c>/<c>LoadJson</c> e o compilador
    /// para em CS0121 (ambiguidade). É a mesma armadilha do <c>CoordinatedSpawnsHandler</c>.
    /// </summary>
    public static class JsonFileExtensions
    {
        /// <summary>Grava o objeto como JSON no caminho dado.</summary>
        public static void SaveJson<T>(this T jsonObject, string path, params JsonConverter[] jsonConverters)
            where T : class
            => Nautilus.Json.ExtensionMethods.JsonExtensions.SaveJson(jsonObject, path, jsonConverters);

        /// <summary>
        /// Lê o JSON do caminho dado para dentro do objeto. <paramref name="createFileIfNotExist"/>
        /// grava os valores atuais quando o arquivo ainda não existe — é como o legado
        /// cria o config na primeira execução.
        /// </summary>
        public static void LoadJson<T>(this T jsonObject, string path, bool createFileIfNotExist = true, params JsonConverter[] jsonConverters)
            where T : class
            => Nautilus.Json.ExtensionMethods.JsonExtensions.LoadJson(jsonObject, path, createFileIfNotExist, jsonConverters);
    }
}
