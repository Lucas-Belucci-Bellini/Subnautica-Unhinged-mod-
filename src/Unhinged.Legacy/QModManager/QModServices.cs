using System.Linq;
using BepInEx.Bootstrap;

namespace QModManager.API
{
    /// <summary>Descrição mínima de um mod, como o QModManager a expunha.</summary>
    public interface IQMod
    {
        string Id { get; }
        string DisplayName { get; }
        System.Version ParsedVersion { get; }
    }

    /// <summary>
    /// Serviços do QModManager, reimplementados sobre o <c>Chainloader</c> do BepInEx.
    ///
    /// O QModManager não existe no ramo moderno, mas as perguntas que o código legado lhe
    /// fazia — "este mod está presente?", "qual é o meu mod?" — o BepInEx responde melhor,
    /// porque o <c>Chainloader</c> é a fonte real do que carregou.
    /// </summary>
    public static class QModServices
    {
        public static IQModServices Main { get; } = new Services();

        private sealed class Services : IQModServices
        {
            public bool ModPresent(string id) =>
                !string.IsNullOrEmpty(id) && Chainloader.PluginInfos.ContainsKey(id);

            public IQMod FindModById(string id) =>
                !string.IsNullOrEmpty(id) && Chainloader.PluginInfos.TryGetValue(id, out var info)
                    ? new PluginMod(info)
                    : null;

            /// <summary>
            /// O mod que chamou. Resolvido pelo assembly do chamador — é o mais próximo
            /// que dá para chegar sem o registro do QModManager.
            /// </summary>
            public IQMod GetMyMod()
            {
                var caller = System.Reflection.Assembly.GetCallingAssembly().Location;
                var info = Chainloader.PluginInfos.Values.FirstOrDefault(
                    p => !string.IsNullOrEmpty(p?.Location) &&
                         string.Equals(p.Location, caller, System.StringComparison.OrdinalIgnoreCase));
                return info == null ? null : new PluginMod(info);
            }

            /// <summary>
            /// Mensagem crítica ao jogador. O QModManager tinha UI própria; aqui vai para
            /// o <c>ErrorMessage</c> do jogo, que é o canal equivalente.
            /// </summary>
            public void AddCriticalMessage(string message)
            {
                Unhinged.Legacy.LegacyLog.Error(message);
                ErrorMessage.AddError(message);
            }
        }

        private sealed class PluginMod : IQMod
        {
            private readonly BepInEx.PluginInfo _info;
            public PluginMod(BepInEx.PluginInfo info) => _info = info;
            public string Id => _info.Metadata?.GUID;
            public string DisplayName => _info.Metadata?.Name;
            public System.Version ParsedVersion => _info.Metadata?.Version;
        }
    }

    public interface IQModServices
    {
        bool ModPresent(string id);
        IQMod FindModById(string id);
        IQMod GetMyMod();
        void AddCriticalMessage(string message);
    }
}
