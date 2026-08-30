namespace SMLHelper.V2.Options
{
    /// <summary>
    /// Base do painel de opções, como no SMLHelper V2. Herda da do Nautilus, então o
    /// código legado que escreve <c>class MinhasOpcoes : ModOptions</c> segue igual.
    /// </summary>
    public abstract class ModOptions : Nautilus.Options.ModOptions
    {
        protected ModOptions(string name) : base(name) { }
    }
}

namespace SMLHelper.V2.Handlers
{
    /// <summary>
    /// Registro do painel de opções. Encaminha para o
    /// <c>Nautilus.Handlers.OptionsPanelHandler</c>.
    ///
    /// ⚠️ Isto cobre o caminho <c>RegisterModOptions(ModOptions)</c>, que é o que o FCS
    /// usa (8 chamadas). O caminho **por atributos** (`[Toggle]`, `[Slider]`…) continua
    /// apenas declarativo — ver Options/Attributes.cs.
    /// </summary>
    public static class OptionsPanelHandler
    {
        public static IOptionsPanelHandler Main { get; } = new MainShim();

        public static void RegisterModOptions(Nautilus.Options.ModOptions options)
            => Nautilus.Handlers.OptionsPanelHandler.RegisterModOptions(options);

        /// <summary>Forma genérica: o Nautilus constrói e registra a instância.</summary>
        public static T RegisterModOptions<T>() where T : Nautilus.Json.ConfigFile, new()
            => Nautilus.Handlers.OptionsPanelHandler.RegisterModOptions<T>();

        private sealed class MainShim : IOptionsPanelHandler
        {
            public void RegisterModOptions(Nautilus.Options.ModOptions options)
                => OptionsPanelHandler.RegisterModOptions(options);

            public T RegisterModOptions<T>() where T : Nautilus.Json.ConfigFile, new()
                => OptionsPanelHandler.RegisterModOptions<T>();
        }
    }

    public interface IOptionsPanelHandler
    {
        void RegisterModOptions(Nautilus.Options.ModOptions options);
        T RegisterModOptions<T>() where T : Nautilus.Json.ConfigFile, new();
    }
}
