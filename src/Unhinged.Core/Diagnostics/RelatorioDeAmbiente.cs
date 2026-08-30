using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using BepInEx;
using BepInEx.Bootstrap;
using BepInEx.Logging;
using UnityEngine;

namespace Unhinged.Core.Diagnostics
{
    /// <summary>
    /// Escreve um relatório do ambiente em <c>BepInEx/Unhinged-Relatorio.md</c>.
    ///
    /// Existe por um motivo prático: o `LogOutput.log` do BepInEx tem dezenas de
    /// milhares de linhas de todos os mods, e pedir para alguém garimpar aquilo é
    /// pedir para o teste não acontecer. Este arquivo é curto, tem só o que decide
    /// diagnóstico, e pode ser enviado inteiro.
    ///
    /// Regra de ouro: **nunca lançar**. Um relatório é diagnóstico; derrubar o
    /// carregamento do plugin por causa dele inverteria a finalidade.
    /// </summary>
    internal static class RelatorioDeAmbiente
    {
        internal const string NomeDoArquivo = "Unhinged-Relatorio.md";

        /// <summary>
        /// GUIDs da pilha moderna e da pilha legada. A convivência das duas é a
        /// causa-raiz suspeita nº 1 na instalação do operador, então o relatório
        /// responde isso na primeira tela em vez de deixar deduzir.
        /// </summary>
        private static readonly (string Guid, string Rotulo)[] Marcadores =
        {
            ("com.snmodding.nautilus",      "Nautilus (pilha moderna)"),
            ("QModManager.QMods",           "QModManager (pilha LEGADA)"),
            ("com.ahk1221.smlhelper",       "SMLHelper (pilha LEGADA)"),
            ("com.snmodding.smlhelper",     "SMLHelper (pilha LEGADA, GUID alt.)"),
            ("com.bepis.bepinex.configurationmanager", "ConfigurationManager"),
        };

        internal static string Escrever(ManualLogSource log)
        {
            try
            {
                var destino = Path.Combine(Paths.BepInExRootPath, NomeDoArquivo);
                File.WriteAllText(destino, Montar(), new UTF8Encoding(false));
                log?.LogInfo($"Relatório de ambiente escrito em: {destino}");
                return destino;
            }
            catch (Exception ex)
            {
                log?.LogWarning($"Não consegui escrever o relatório de ambiente: {ex.Message}");
                return null;
            }
        }

        private static string Montar()
        {
            var sb = new StringBuilder();
            var plugins = Chainloader.PluginInfos ?? new Dictionary<string, PluginInfo>();
            var falhas = Chainloader.DependencyErrors ?? new List<string>();

            sb.AppendLine("# Subnautica Unhinged — relatório de ambiente");
            sb.AppendLine();
            sb.AppendLine($"Gerado em {DateTime.Now:yyyy-MM-dd HH:mm:ss} (hora local).");
            sb.AppendLine();
            sb.AppendLine("> Arquivo de diagnóstico. Pode apagar à vontade — é reescrito a cada partida.");
            sb.AppendLine();

            sb.AppendLine("## Resumo");
            sb.AppendLine();
            sb.AppendLine("| | |");
            sb.AppendLine("| --- | --- |");
            sb.AppendLine($"| Unhinged | {UnhingedInfo.Version} |");
            sb.AppendLine($"| Mods carregados | **{plugins.Count}** |");
            sb.AppendLine($"| Mods que FALHARAM | **{falhas.Count}** |");
            Linha(sb, "Versão do jogo", Seguro(() => Application.version));
            Linha(sb, "Unity", Seguro(() => Application.unityVersion));
            Linha(sb, "BepInEx", Seguro(() => typeof(Chainloader).Assembly.GetName().Version.ToString()));
            Linha(sb, "Plataforma", Seguro(() => Application.platform.ToString()));
            sb.AppendLine();

            sb.AppendLine("## Pilhas de modding presentes");
            sb.AppendLine();
            sb.AppendLine("A pilha moderna (Nautilus) e a legada (QModManager/SMLHelper) **não foram feitas");
            sb.AppendLine("para conviver**. Se as duas aparecerem abaixo, é o primeiro suspeito de qualquer");
            sb.AppendLine("erro estranho — inclusive do `TypeLoadException` de `Oculus.Newtonsoft.Json`.");
            sb.AppendLine();
            sb.AppendLine("| Componente | Presente | Versão |");
            sb.AppendLine("| --- | --- | --- |");
            foreach (var (guid, rotulo) in Marcadores)
            {
                var presente = plugins.TryGetValue(guid, out var info);
                var versao = presente ? info?.Metadata?.Version?.ToString() ?? "?" : "—";
                sb.AppendLine($"| {rotulo} | {(presente ? "**sim**" : "não")} | {versao} |");
            }
            sb.AppendLine();

            sb.AppendLine("### A ponte legada do Unhinged está carregada?");
            sb.AppendLine();
            var ponte = Seguro(() => AppDomain.CurrentDomain.GetAssemblies()
                .FirstOrDefault(a => a.GetName().Name == "Unhinged.Legacy")?.GetName().Version?.ToString());
            sb.AppendLine(ponte == null
                ? "**Não.** O `Unhinged.Legacy.dll` não foi carregado. Nesta versão isso é *esperado*: "
                  + "a ponte é uma biblioteca, e só é carregada quando algum mod portado a usa. "
                  + "Não é erro."
                : $"Sim — versão {ponte}.");
            sb.AppendLine();

            if (falhas.Count > 0)
            {
                sb.AppendLine("## ⚠️ Mods que NÃO carregaram");
                sb.AppendLine();
                sb.AppendLine("É aqui que mora a explicação para \"o mod sumiu do jogo\".");
                sb.AppendLine();
                foreach (var f in falhas) sb.AppendLine($"- {f}");
                sb.AppendLine();
            }

            sb.AppendLine("## Mods carregados");
            sb.AppendLine();
            sb.AppendLine("| Nome | Versão | GUID |");
            sb.AppendLine("| --- | --- | --- |");
            foreach (var info in plugins.Values
                         .Where(p => p?.Metadata != null)
                         .OrderBy(p => p.Metadata.Name, StringComparer.OrdinalIgnoreCase))
            {
                sb.AppendLine($"| {info.Metadata.Name} | {info.Metadata.Version} | `{info.Metadata.GUID}` |");
            }
            sb.AppendLine();

            sb.AppendLine("## Caminhos");
            sb.AppendLine();
            Linha(sb, "Jogo", Seguro(() => Paths.GameRootPath));
            Linha(sb, "BepInEx", Seguro(() => Paths.BepInExRootPath));
            Linha(sb, "Plugins", Seguro(() => Paths.PluginPath));
            sb.AppendLine();

            sb.AppendLine("---");
            sb.AppendLine();
            sb.AppendLine("Envie este arquivo inteiro ao abrir um problema — ele responde a maior parte");
            sb.AppendLine("das perguntas de diagnóstico sem precisar do `LogOutput.log` completo.");

            return sb.ToString();
        }

        private static void Linha(StringBuilder sb, string chave, string valor)
        {
            if (!string.IsNullOrEmpty(valor)) sb.AppendLine($"| {chave} | {valor} |");
        }

        /// <summary>
        /// Cada sonda é isolada: uma API que mudou de forma entre versões do jogo
        /// deixa aquela linha de fora, e não leva o relatório junto.
        /// </summary>
        private static string Seguro(Func<string> sonda)
        {
            try { return sonda(); }
            catch { return null; }
        }
    }
}
