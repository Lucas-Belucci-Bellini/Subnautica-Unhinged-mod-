using System;
using System.Collections.Generic;
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

        /// <summary>Desligado, `Anotar` não faz nada e não custa nada.</summary>
        public static bool Ligado { get; set; }

        public static void Anotar(Entrada e)
        {
            if (!Ligado || e == null) return;
            lock (_trava) _entradas.Add(e);
        }

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
            var itens = Entradas;
            var sb = new StringBuilder();
            sb.AppendLine("# Registro de conteudo — Alterra Hub (FCS)");
            sb.AppendLine();
            sb.AppendLine("Gerado em " + DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss") + ".");
            sb.AppendLine();

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

            sb.AppendLine("## Todos os itens");
            sb.AppendLine();
            sb.AppendLine("| ClassID | modulo | TechType | valor | icone | liberado |");
            sb.AppendLine("| --- | --- | --- | ---: | :---: | :---: |");
            foreach (var x in itens.OrderBy(x => x.Modulo).ThenBy(x => x.ClassID))
                sb.AppendLine("| `" + x.ClassID + "` | " + x.Modulo + " | "
                    + (x.TechType ?? "—") + " | " + x.TechTypeValor + " | "
                    + (x.TemIcone ? "sim" : "—") + " | "
                    + (x.LiberadoNoInicio ? "sim" : "BLOQ") + " |");

            return sb.ToString();
        }
    }
}
