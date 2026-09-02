using System;
using System.Collections.Generic;
using Nautilus.Handlers;

namespace SMLHelper.V2.Handlers
{
    /// <summary>
    /// Spawns por coordenada. O tipo <c>SpawnInfo</c> passou a viver no Nautilus
    /// (<c>Nautilus.Handlers.SpawnInfo</c>); este shim mantém o nome antigo em escopo para
    /// o código legado, encaminhando as chamadas.
    /// </summary>
    public static class CoordinatedSpawnsHandler
    {
        public static void RegisterCoordinatedSpawn(SpawnInfo spawnInfo)
            => Nautilus.Handlers.CoordinatedSpawnsHandler.RegisterCoordinatedSpawn(spawnInfo);

        public static void RegisterCoordinatedSpawns(List<SpawnInfo> spawnInfos)
            => Nautilus.Handlers.CoordinatedSpawnsHandler.RegisterCoordinatedSpawns(spawnInfos);

        public static void RegisterCoordinatedSpawnsForOneTechType(TechType techType, params Nautilus.Assets.SpawnLocation[] locations)
            => Nautilus.Handlers.CoordinatedSpawnsHandler.RegisterCoordinatedSpawnsForOneTechType(techType, locations);
    }

    /// <summary>
    /// Comandos de console.
    ///
    /// ⚠️ Isto é o registro por <b>tipo</b> (<c>RegisterConsoleCommands(Type)</c>), que é o
    /// que o FCS usa — e ele funciona de verdade. O atributo
    /// <c>[ConsoleCommand]</c> continua apenas declarativo: quem varre os métodos marcados
    /// é o Nautilus, ao receber o tipo aqui.
    /// </summary>
    public static class ConsoleCommandsHandler
    {
        public static IConsoleCommandsHandler Main { get; } = new MainShim();

        /// <summary>
        /// Registra os comandos de um tipo. Uma falha aqui NÃO derruba o módulo.
        /// </summary>
        /// <remarks>
        /// ⚠️ Precaução, não conserto de defeito medido: os nomes de comando dos sete
        /// módulos foram conferidos e nenhum se repete. Mas este é o mesmo padrão
        /// "registro por mod" que, no painel de opções, matou seis módulos quando os
        /// sete DLLs viraram um só — e um comando de debug jamais vale o módulo que o
        /// declara. Se colidir um dia, sai no log e o resto segue.
        /// </remarks>
        public static void RegisterConsoleCommands(Type type)
        {
            try
            {
                Nautilus.Handlers.ConsoleCommandsHandler.RegisterConsoleCommands(type);
            }
            catch (Exception ex)
            {
                Unhinged.Legacy.Diagnostico.RegistroDeConteudo.AnotarModulo(
                    type?.Namespace?.Split('.')[0] ?? "?", "ConsoleCommands", "ignorado", ex);
            }
        }

        public static void RegisterConsoleCommand(string command, Type declaringType, string methodName)
            => Nautilus.Handlers.ConsoleCommandsHandler.RegisterConsoleCommand(command, declaringType, methodName, null);

        private sealed class MainShim : IConsoleCommandsHandler
        {
            public void RegisterConsoleCommands(Type type)
                => ConsoleCommandsHandler.RegisterConsoleCommands(type);
        }
    }

    public interface IConsoleCommandsHandler
    {
        void RegisterConsoleCommands(Type type);
    }
}

namespace SMLHelper.V2.Handlers
{
    /// <summary>
    /// Interface de ingrediente que o SMLHelper V2 expunha. O jogo moderno usa a classe
    /// concreta <c>Ingredient</c>; a interface some, mas código legado ainda a declara.
    /// </summary>
    public interface IIngredient
    {
        TechType techType { get; }
        int amount { get; }
    }
}
