using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;

namespace Unhinged.Legacy.Diagnostico
{
    /// <summary>
    /// Coleta, item a item, o que realmente aconteceu no registro.
    ///
    /// Existe por causa de um sintoma que leitura de código não resolve: "o item não
    /// aparece". Essa frase é compatível com sete causas diferentes — o plugin não
    /// carregou, o ponto de entrada não rodou, o <c>Patch()</c> não foi chamado, o
    /// TechType não nasceu, o prefab não resolveu, a receita não registrou, o unlock não
    /// valeu. Cada uma tem conserto diferente, e adivinhar entre elas já custou versões
    /// a este projeto.
    ///
    /// A coleta é FATO, não intenção: cada linha é gravada dentro do próprio
    /// <c>Patch()</c>, depois de o Nautilus devolver o TechType.
    /// </summary>
    public static class RegistroDeConteudo
    {
        public sealed class Entrada
        {
            public string ClassID;
            public string Modulo;
            public string TechType;
            public int TechTypeValor;
            public bool TemIcone;
            public bool LiberadoNoInicio;
            public string Falha;
        }

        private static readonly List<Entrada> _entradas = new List<Entrada>();
        private static readonly object _trava = new object();
        private static StreamWriter _fluxo;
        private static string _caminho;

        /// <summary>Desligado, `Anotar` não faz nada e não custa nada.</summary>
        public static bool Ligado { get; set; }

        /// <summary>
        /// Abre o arquivo AGORA e escreve o cabeçalho, antes de qualquer registro.
        ///
        /// Escrever tudo no fim parece mais limpo e é pior: se o jogo fechar no meio do
        /// registro — que é justamente quando o diagnóstico importa —, não sobra arquivo
        /// nenhum. Aqui cada item é gravado e descarregado no disco na hora, então um
        /// fechamento no item 37 deixa um arquivo com 37 itens, e onde ele parou já é
        /// metade da resposta.
        ///
        /// ⚠️ `FileShare.ReadWrite` não é detalhe: sem ele o Windows **impede** que o
        /// arquivo seja copiado enquanto o jogo está aberto, e a única hora em que dá
        /// para copiar seria depois de fechar — exatamente o que se quer evitar.
        /// </summary>
        public static void AbrirArquivo(string caminho)
        {
            if (!Ligado) return;
            try
            {
                _caminho = caminho;
                var fs = new FileStream(caminho, FileMode.Create, FileAccess.Write,
                                        FileShare.ReadWrite | FileShare.Delete);
                _fluxo = new StreamWriter(fs) { AutoFlush = true };
                _fluxo.WriteLine("# Registro de conteudo — Alterra Hub (FCS)");
                _fluxo.WriteLine();
                _fluxo.WriteLine("Aberto em " + DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss") + ".");
                _fluxo.WriteLine();
                _fluxo.WriteLine("> Este arquivo e escrito AO VIVO, um item por vez. Se o jogo fechar");
                _fluxo.WriteLine("> no meio, o que estiver aqui ja vale — e onde ele para e a pista.");
                _fluxo.WriteLine("> Pode copiar com o jogo aberto.");
                _fluxo.WriteLine();
                _fluxo.WriteLine("| # | ClassID | modulo | TechType | valor | icone | liberado |");
                _fluxo.WriteLine("| ---: | --- | --- | --- | ---: | :---: | :---: |");
            }
            catch (Exception)
            {
                // Sem arquivo, a coleta em memoria continua e o resumo final ainda sai.
                _fluxo = null;
            }
        }

        public static void Anotar(Entrada e)
        {
            if (!Ligado || e == null) return;
            lock (_trava)
            {
                _entradas.Add(e);
                EscreverLinha(_entradas.Count, e);
            }
        }

        private static void EscreverLinha(int n, Entrada e)
        {
            if (_fluxo == null) return;
            try
            {
                if (e.Falha != null)
                    _fluxo.WriteLine("| " + n + " | `" + e.ClassID + "` | " + e.Modulo
                        + " | ❌ " + e.Falha + " | — | — | — |");
                else
                    _fluxo.WriteLine("| " + n + " | `" + e.ClassID + "` | " + e.Modulo
                        + " | " + (e.TechType ?? "—") + " | " + e.TechTypeValor
                        + " | " + (e.TemIcone ? "sim" : "—")
                        + " | " + (e.LiberadoNoInicio ? "sim" : "BLOQ") + " |");
            }
            catch (Exception) { _fluxo = null; }
        }

        /// <summary>
        /// Fecha o arquivo escrevendo o resumo. Chamada de um `finally`: um registro que
        /// estoura no meio é o caso em que o diagnóstico mais vale, e seria o exato caso
        /// em que ele não sairia se dependesse do caminho feliz.
        /// </summary>
        public static void FecharArquivo(string motivo = null)
        {
            if (_fluxo == null) return;
            try
            {
                _fluxo.WriteLine();
                if (motivo != null)
                {
                    _fluxo.WriteLine("## ⚠️ Interrompido");
                    _fluxo.WriteLine();
                    _fluxo.WriteLine(motivo);
                    _fluxo.WriteLine();
                }
                _fluxo.Write(Resumo());
                _fluxo.Flush();
                _fluxo.Dispose();
            }
            catch (Exception) { }
            finally { _fluxo = null; }
        }

        /// <summary>Onde o arquivo está, para o log poder dizer.</summary>
        public static string Caminho => _caminho;

        public static void AnotarFalha(string classId, string modulo, Exception ex)
        {
            if (!Ligado) return;
            lock (_trava)
                _entradas.Add(new Entrada
                {
                    ClassID = classId,
                    Modulo = modulo,
                    Falha = (ex?.GetType().Name ?? "?") + ": " + (ex?.Message ?? ""),
                });
        }

        public static IReadOnlyList<Entrada> Entradas
        {
            get { lock (_trava) return _entradas.ToList(); }
        }

        /// <summary>
        /// Relatório em Markdown, para ARQUIVO — não para o log. O `LogOutput.log` tem
        /// dezenas de milhares de linhas de todos os mods, e pedir para alguém garimpar
        /// aquilo é pedir para o diagnóstico não acontecer.
        /// </summary>
        public static string Relatorio()
        {
            var sb = new StringBuilder();
            sb.AppendLine("# Registro de conteudo — Alterra Hub (FCS)");
            sb.AppendLine();
            sb.AppendLine("Gerado em " + DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss") + ".");
            sb.AppendLine();
            sb.Append(Resumo());
            return sb.ToString();
        }

        /// <summary>
        /// O diagnóstico em si, sem cabeçalho. Serve tanto ao relatório de uma vez só
        /// quanto ao rodapé do arquivo escrito ao vivo — uma implementação, dois usos,
        /// e nenhuma chance de os dois divergirem.
        /// </summary>
        public static string Resumo()
        {
            var itens = Entradas;
            var sb = new StringBuilder();

            if (itens.Count == 0)
            {
                sb.AppendLine("## ⚠️ NENHUM item tentou se registrar");
                sb.AppendLine();
                sb.AppendLine("Isso NAO e problema de receita, de PDA nem de unlock — e a");
                sb.AppendLine("montante dos tres. Verifique nesta ordem:");
                sb.AppendLine();
                sb.AppendLine("1. O `LogOutput.log` tem `Loading [Subnautica Unhinged — Alterra Hub`?");
                sb.AppendLine("   Se nao: o BepInEx nao achou o DLL. E instalacao, nao codigo.");
                sb.AppendLine("2. Tem `Alterra Hub: N ponto(s) de entrada executado(s)`?");
                sb.AppendLine("   Se N = 0: o carregador nao achou os `[QModCore]`.");
                sb.AppendLine("3. `EnableFCS` esta `true` no `.cfg`?");
                return sb.ToString();
            }

            var comFalha = itens.Where(x => x.Falha != null).ToList();
            var ok = itens.Where(x => x.Falha == null).ToList();
            var semTechType = ok.Where(x => x.TechTypeValor == 0).ToList();
            var trancados = ok.Where(x => !x.LiberadoNoInicio).ToList();
            var semIcone = ok.Where(x => !x.TemIcone).ToList();

            sb.AppendLine("| | |");
            sb.AppendLine("| --- | ---: |");
            sb.AppendLine("| tentaram registrar | " + itens.Count + " |");
            sb.AppendLine("| **TechType criado** | **" + (ok.Count - semTechType.Count) + "** |");
            sb.AppendLine("| TechType = None (falhou) | " + semTechType.Count + " |");
            sb.AppendLine("| excecao no registro | " + comFalha.Count + " |");
            sb.AppendLine("| nascem BLOQUEADOS | " + trancados.Count + " |");
            sb.AppendLine("| sem icone | " + semIcone.Count + " |");
            sb.AppendLine();

            sb.AppendLine("## Por modulo");
            sb.AppendLine();
            sb.AppendLine("| modulo | itens | com TechType | falhas |");
            sb.AppendLine("| --- | ---: | ---: | ---: |");
            foreach (var g in itens.GroupBy(x => x.Modulo ?? "?").OrderBy(g => g.Key))
                sb.AppendLine("| " + g.Key + " | " + g.Count() + " | "
                    + g.Count(x => x.Falha == null && x.TechTypeValor != 0) + " | "
                    + g.Count(x => x.Falha != null) + " |");
            sb.AppendLine();

            if (comFalha.Count > 0)
            {
                sb.AppendLine("## ❌ Excecoes no registro");
                sb.AppendLine();
                foreach (var x in comFalha)
                    sb.AppendLine("- `" + x.ClassID + "` (" + x.Modulo + ") — " + x.Falha);
                sb.AppendLine();
            }

            if (semTechType.Count > 0)
            {
                sb.AppendLine("## ⚠️ TechType nao criado");
                sb.AppendLine();
                sb.AppendLine("O item chamou `Patch()` e saiu com `TechType.None`. Esses NAO");
                sb.AppendLine("aparecem em lugar nenhum, e `unlock all` tambem nao os alcanca —");
                sb.AppendLine("o comando percorre TechTypes que existem.");
                sb.AppendLine();
                foreach (var x in semTechType)
                    sb.AppendLine("- `" + x.ClassID + "` (" + x.Modulo + ")");
                sb.AppendLine();
            }

            if (trancados.Count > 0)
            {
                sb.AppendLine("## 🔒 Nascem bloqueados");
                sb.AppendLine();
                sb.AppendLine("Existem como TechType e NAO aparecem no construtor ate serem");
                sb.AppendLine("liberados. Se for a maioria, e o sintoma do `unlockAtStart`");
                sb.AppendLine("invertido — corrigido na 1.0.6, entao confira a versao.");
                sb.AppendLine();
                foreach (var x in trancados.Take(40))
                    sb.AppendLine("- `" + x.ClassID + "` (" + x.Modulo + ")");
                if (trancados.Count > 40)
                    sb.AppendLine("- … e mais " + (trancados.Count - 40));
                sb.AppendLine();
            }

            return sb.ToString();
        }
    }
}
