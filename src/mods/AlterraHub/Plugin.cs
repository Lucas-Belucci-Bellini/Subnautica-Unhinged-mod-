using System;
using System.Linq;
using BepInEx;
using Unhinged.Legacy;

namespace Unhinged.AlterraHub
{
    /// <summary>
    /// Ponto de entrada do pacote Alterra Hub.
    ///
    /// A fonte do FCS marca seus pontos de entrada com <c>[QModCore]</c>/<c>[QModPatch]</c>,
    /// atributos que **só marcam** código — quem os executava era o QModManager, que não
    /// existe no ramo moderno. E o BepInEx só carrega assemblies com
    /// <c>[BepInPlugin]</c>. Sem esta classe, o DLL compilaria, seria ignorado no
    /// carregamento e o mod simplesmente não existiria em jogo — sem erro nenhum.
    /// </summary>
    [BepInPlugin(Guid, "Subnautica Unhinged — Alterra Hub (FCStudios)", "1.1.0")]
    // Hard, não Soft: ao contrário do Core, este pacote realmente chama a API do Nautilus
    // em toda receita e todo prefab. Carregar sem ele seria falhar mais tarde e pior.
    [BepInDependency(NautilusGuid, BepInDependency.DependencyFlags.HardDependency)]
    public sealed class Plugin : BaseUnityPlugin
    {
        internal const string Guid = "com.subnauticaunhinged.alterrahub";
        internal const string NautilusGuid = "com.snmodding.nautilus";

        /// <summary>
        /// Namespace do módulo que precisa registrar antes de todos os outros. Os outros
        /// seis módulos FCS consomem os serviços que ele publica (registro de dispositivos,
        /// moeda, loja), então rodá-los antes dá erro ou registro silenciosamente vazio.
        /// </summary>
        private const string ModuloBase = "FCS_AlterraHub";

        private BepInEx.Configuration.ConfigEntry<bool> _forcarComPilhaLegada;
        private BepInEx.Configuration.ConfigEntry<bool> _habilitarFcs;

        /// <summary>
        /// Namespace de cada modulo -> a chave que o liga/desliga.
        ///
        /// UM mecanismo de configuracao para os sete modulos, e nao sete mecanismos
        /// incompativeis: tudo passa pelo `Config.Bind` do BepInEx e vai parar no mesmo
        /// arquivo .cfg, editavel sem recompilar nada.
        /// </summary>
        private static readonly (string Namespace, string Chave, string Descricao)[] Modulos =
        {
            ("FCS_AlterraHub",           "EnableAlterraHub",  "Alterra Hub — a BASE. Os outros seis registram nos servicos dele (dispositivos, moeda, loja); desligar este desliga todos."),
            ("FCS_EnergySolutions",      "EnableEnergy",      "Energy Solutions — gerador, cluster solar, armazenamento de energia, pilone telepower."),
            ("FCS_HomeSolutions",        "EnableHome",        "Home Solutions — mobiliario, paredes, JukeBox, SeaBreeze."),
            ("FCS_LifeSupportSolutions", "EnableLifeSupport", "Life Support Solutions — suporte de vida."),
            ("FCS_ProductionSolutions", "EnableProduction",  "Production Solutions — Deep Driller, Hydroponic Harvester, Replicator, Matter Analyzer."),
            ("FCS_StorageSolutions",     "EnableStorage",     "Storage Solutions — Alterra Storage e docking."),
            // ⚠️ O namespace deste NAO tem o prefixo `FCS_` — a PASTA se chama
            // `FCS_CyclopsUpgradeConsole`, mas o `namespace` e `CyclopsUpgradeConsole`.
            // Usar o nome da pasta aqui faz o interruptor nunca casar: ele aparece no
            // .cfg, o jogador desliga, e o modulo carrega assim mesmo.
            ("CyclopsUpgradeConsole",    "EnableCyclops",     "Cyclops Upgrade Console — exige o mod MoreCyclopsUpgrades instalado."),
        };

        private readonly System.Collections.Generic.Dictionary<string, BepInEx.Configuration.ConfigEntry<bool>>
            _porModulo = new System.Collections.Generic.Dictionary<string, BepInEx.Configuration.ConfigEntry<bool>>(StringComparer.Ordinal);

        private void Awake()
        {
            // A ponte reimplementa `SMLHelper.V2.*` sobre o Nautilus. Com o SMLHelper de
            // verdade tambem carregado, OS DOIS frameworks patcham os mesmos metodos do
            // jogo — e o resultado nao e um erro limpo, e comportamento indefinido que
            // pode travar a carga sem dizer por que. Recusar com mensagem clara e melhor
            // do que rodar e corromper.
            _forcarComPilhaLegada = Config.Bind(
                "1. Compatibilidade", "ForcarComPilhaLegada", false,
                "Carrega mesmo com QModManager/SMLHelper ativos. Padrao false: as duas "
                + "pilhas patcham os mesmos metodos do jogo, e rodar as duas juntas e o "
                + "cenario onde o jogo trava sem explicacao.");

            _habilitarFcs = Config.Bind(
                "2. Modulos", "EnableFCS", true,
                "Chave mestra da suite FCS. Em false, nenhum modulo carrega — o plugin "
                + "continua sendo carregado pelo BepInEx, mas nao registra nada.");

            foreach (var (ns, chave, descricao) in Modulos)
                _porModulo[ns] = Config.Bind("2. Modulos", chave, true, descricao);

            var legada = PilhaLegada.Detectar();
            if (legada.Count > 0 && !_forcarComPilhaLegada.Value)
            {
                Logger.LogError(
                    "Alterra Hub NAO foi carregado: a pilha LEGADA de modding esta ativa junto "
                    + "com a moderna, e as duas patcham os mesmos metodos do jogo.");
                foreach (var item in legada) Logger.LogError($"  · {item}");
                Logger.LogError(
                    "Escolha uma: desative o QModManager/SMLHelper e use os mods portados, "
                    + "ou desinstale este pacote e siga com os originais. Para tentar mesmo "
                    + "assim, ligue 'ForcarComPilhaLegada' na configuracao — por sua conta, "
                    + "e o cenario onde o jogo trava sem explicacao.");
                return;
            }

            if (!_habilitarFcs.Value)
            {
                Logger.LogInfo("EnableFCS=false: nenhum modulo da suite FCS foi carregado.");
                return;
            }

            // ⚠️ Dependencia real, nao preferencia: os outros seis modulos registram nos
            // servicos que o FCS_AlterraHub publica. Rodar um deles com a base desligada
            // nao da erro limpo — da registro vazio, item sem receita e NRE mais adiante.
            // Entao ou a base esta ligada, ou nada roda; e dito em voz alta.
            if (!_porModulo[ModuloBase].Value)
            {
                var dependentes = Modulos
                    .Where(m => m.Namespace != ModuloBase && _porModulo[m.Namespace].Value)
                    .Select(m => m.Chave)
                    .ToArray();

                if (dependentes.Length > 0)
                {
                    Logger.LogError(
                        $"EnableAlterraHub=false, mas {dependentes.Length} modulo(s) que dependem dele "
                        + "continuam ligados: " + string.Join(", ", dependentes) + ".");
                    Logger.LogError(
                        "Eles registram nos servicos do Alterra Hub (dispositivos, moeda, loja). "
                        + "Sem a base, o resultado nao e um erro limpo — e item sem receita e falha "
                        + "mais adiante. Nada foi carregado. Ligue EnableAlterraHub ou desligue os demais.");
                    return;
                }

                Logger.LogInfo("EnableAlterraHub=false e nenhum dependente ligado: nada a carregar.");
                return;
            }

            try
            {
                var executados = LegacyModLoader.Run(
                    typeof(Plugin).Assembly,
                    Logger,
                    tipo => tipo.FullName != null && tipo.FullName.StartsWith(ModuloBase, StringComparison.Ordinal) ? 0 : 1,
                    ModuloEstaLigado);

                Logger.LogInfo($"Alterra Hub: {executados} ponto(s) de entrada executado(s).");

                if (executados == 0)
                {
                    Logger.LogWarning(
                        "Nenhum ponto de entrada rodou. O pacote foi carregado mas não registrou nada — "
                        + "confira se o Unhinged.Legacy.dll está na mesma pasta.");
                }
            }
            catch (Exception ex)
            {
                // Um pacote que falha inteiro não pode levar o resto do jogo junto.
                Logger.LogError($"Falha ao carregar o Alterra Hub: {ex}");
            }
        }

        /// <summary>
        /// Um <c>[QModCore]</c> so roda se o modulo dele estiver ligado. Tipo cujo
        /// namespace nao casa com nenhum modulo conhecido roda por padrao — melhor
        /// carregar algo nao mapeado do que engolir codigo em silencio.
        /// </summary>
        private bool ModuloEstaLigado(Type tipo)
        {
            var nome = tipo?.FullName;
            if (nome == null) return true;

            foreach (var (ns, _, _) in Modulos)
            {
                if (!nome.StartsWith(ns, StringComparison.Ordinal)) continue;
                var ligado = _porModulo[ns].Value;
                if (!ligado) Logger.LogInfo($"  · {ns} desligado na configuracao.");
                return ligado;
            }
            return true;
        }
    }
}
