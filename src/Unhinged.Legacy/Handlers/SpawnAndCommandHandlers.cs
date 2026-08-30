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

        public static void RegisterConsoleCommands(Type type)
            => Nautilus.Handlers.ConsoleCommandsHandler.RegisterConsoleCommands(type);

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
