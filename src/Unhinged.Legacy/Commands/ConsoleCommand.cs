using System;

namespace SMLHelper.V2.Commands
{
    /// <summary>
    /// Marca um método como comando de console, como no SMLHelper V2.
    ///
    /// ⚠️ Igual aos atributos de opções: hoje é APENAS DE DECLARAÇÃO — faz compilar, mas
    /// o comando ainda não é registrado no console. Registrar exige ligar ao
    /// <c>Nautilus.Handlers.ConsoleCommandsHandler</c>, ainda não feito.
    /// </summary>
    [AttributeUsage(AttributeTargets.Method)]
    public sealed class ConsoleCommandAttribute : Attribute
    {
        public string Command { get; }
        public ConsoleCommandAttribute(string command) => Command = command;
    }
}
