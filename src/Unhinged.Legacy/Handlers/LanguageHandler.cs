using System.Collections.Generic;

namespace SMLHelper.V2.Handlers
{
    /// <summary>
    /// Equivale ao <c>SMLHelper.V2.Handlers.LanguageHandler</c>, encaminhando para o
    /// <c>Nautilus.Handlers.LanguageHandler</c>.
    ///
    /// Duas diferenças que o guia oficial de migração aponta e que esta classe absorve:
    /// o handler do Nautilus é estático (o legado usava a propriedade <c>Main</c>), e os
    /// métodos dele pedem o idioma explicitamente, enquanto o legado assumia inglês.
    /// </summary>
    public static class LanguageHandler
    {
        private const string DefaultLanguage = "English";

        /// <summary>
        /// Compatibilidade com o padrão <c>LanguageHandler.Main.SetLanguageLine(...)</c>
        /// do SMLHelper V2. Aponta para a própria classe estática.
        /// </summary>
        public static ILanguageHandler Main { get; } = new MainShim();

        public static void SetLanguageLine(string lineId, string text, string language = DefaultLanguage)
            => Nautilus.Handlers.LanguageHandler.SetLanguageLine(lineId, text, language);

        public static void SetTechTypeName(TechType techType, string text, string language = DefaultLanguage)
            => Nautilus.Handlers.LanguageHandler.SetTechTypeName(techType, text, language);

        public static void SetTechTypeTooltip(TechType techType, string text, string language = DefaultLanguage)
            => Nautilus.Handlers.LanguageHandler.SetTechTypeTooltip(techType, text, language);

        public static void RegisterLocalization(string language, Dictionary<string, string> lines)
            => Nautilus.Handlers.LanguageHandler.RegisterLocalization(language, lines);

        public static void RegisterLocalizationFolder(string folder)
            => Nautilus.Handlers.LanguageHandler.RegisterLocalizationFolder(folder);

        private sealed class MainShim : ILanguageHandler
        {
            public void SetLanguageLine(string lineId, string text)
                => LanguageHandler.SetLanguageLine(lineId, text);
        }
    }

    /// <summary>Interface que o SMLHelper V2 expunha via <c>LanguageHandler.Main</c>.</summary>
    public interface ILanguageHandler
    {
        void SetLanguageLine(string lineId, string text);
    }
}
