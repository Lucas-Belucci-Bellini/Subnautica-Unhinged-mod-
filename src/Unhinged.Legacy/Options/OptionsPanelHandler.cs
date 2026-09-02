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

        // Uma instancia por TIPO de config. O Nautilus indexa por assembly; aqui o
        // indice e o tipo, que e o que realmente distingue os sete modulos.
        private static readonly System.Collections.Generic.Dictionary<System.Type, object> _porTipo
            = new System.Collections.Generic.Dictionary<System.Type, object>();
        private static readonly object _trava = new object();

        /// <summary>
        /// ⚠️ <b>SEGUNDO defeito do assembly fundido, e o que matou seis dos sete
        /// modulos do operador.</b>
        /// </summary>
        /// <remarks>
        /// O Nautilus guarda os paineis de opcoes numa <c>SortedList</c> indexada pelo
        /// <b>nome do assembly</b> de quem registra. Enquanto cada modulo do FCS era um
        /// DLL proprio, as sete chaves eram distintas. Fundidos num assembly so, as sete
        /// viraram <c>Unhinged.AlterraHub</c> — a primeira entra, e a segunda estoura:
        /// <code>
        /// ArgumentException: An item with the same key has already been added.
        ///                    Key: Unhinged.AlterraHub
        ///   at SortedList`2.Add(TKey, TValue)
        ///   at Nautilus.Handlers.OptionsPanelHandler.RegisterModOptions(...)
        ///   at CyclopsUpgradeConsole.QPatch..cctor()
        /// </code>
        /// E o lugar onde isso acontece e o pior possivel: o <b>construtor estatico</b>
        /// do <c>QPatch</c> de cada modulo. Um <c>TypeInitializationException</c> fica
        /// memorizado pelo runtime — o tipo nao volta a funcionar naquela sessao, entao
        /// o modulo inteiro morre antes de registrar o primeiro item. Foi exatamente o
        /// que o log mostrou, seis vezes seguidas.
        ///
        /// A entrada duplicada no MENU e cosmetica (o painel so consegue mostrar uma
        /// entrada por assembly de qualquer jeito). Perder o menu e aceitavel; perder o
        /// modulo nao e. Entao a duplicata deixa de ser fatal, e o modulo recebe uma
        /// config carregada do disco do mesmo jeito.
        /// </remarks>
        public static void RegisterModOptions(Nautilus.Options.ModOptions options)
        {
            try
            {
                Nautilus.Handlers.OptionsPanelHandler.RegisterModOptions(options);
            }
            catch (System.ArgumentException)
            {
                // Ja ha um painel registrado para este assembly. Ver o remarks acima.
            }
        }

        /// <summary>Forma genérica: o Nautilus constrói e registra a instância.</summary>
        public static T RegisterModOptions<T>() where T : Nautilus.Json.ConfigFile, new()
        {
            lock (_trava)
            {
                if (_porTipo.TryGetValue(typeof(T), out var pronto))
                    return (T)pronto;
            }

            T config;
            try
            {
                config = Nautilus.Handlers.OptionsPanelHandler.RegisterModOptions<T>();
            }
            catch (System.ArgumentException)
            {
                // O painel ja foi tomado por outro modulo do mesmo assembly. A config em
                // si continua valendo: e um arquivo JSON proprio, e o modulo depende do
                // VALOR, nao da entrada no menu. Carregar direto preserva o que importa.
                config = new T();
                try { config.Load(); } catch (System.Exception) { }
            }

            lock (_trava) _porTipo[typeof(T)] = config;
            return config;
        }

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
