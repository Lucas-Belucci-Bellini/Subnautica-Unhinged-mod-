using System.Collections.Generic;
using System.Reflection.Emit;
using HarmonyLib;
using UnityEngine;

namespace Unhinged.ScannerRoom.Patches
{
    /// <summary>
    /// Alcance do drone: até onde ele pode se afastar da sala antes de perder o controle.
    ///
    /// ⛔ <c>MapRoomScreen.maxCameraDistance</c> é <c>const</c> = 500, igual ao alcance do
    /// scanner. Ou seja: não há campo para escrever — o 500 está **embutido no IL** de
    /// <see cref="MapRoomCamera.CanBeControlled"/>.
    ///
    /// Por isso um transpiler, e não um postfix. Um postfix só veria <c>false</c> e não
    /// teria como saber se a recusa foi por distância ou por outra razão (drone morto,
    /// sem energia, ancorado) — devolver <c>true</c> ali atropelaria as outras checagens.
    /// O transpiler troca **só o número**, e deixa toda a lógica intacta.
    /// </summary>
    [HarmonyPatch(typeof(MapRoomCamera), nameof(MapRoomCamera.CanBeControlled))]
    internal static class DroneRangePatch
    {
        /// <summary>Valor vanilla de <c>MapRoomScreen.maxCameraDistance</c>.</summary>
        private const int VanillaMaxDistance = 500;

        /// <summary>
        /// Quantos literais foram trocados na última aplicação. O
        /// <see cref="Plugin"/> lê isto para dizer, no log, se o patch pegou —
        /// um transpiler que não casa com nada falha **em silêncio**, e silêncio aqui
        /// viraria "o mod não funciona e ninguém sabe por quê".
        /// </summary>
        internal static int SubstituicoesFeitas { get; private set; }

        [HarmonyTranspiler]
        internal static IEnumerable<CodeInstruction> Transpiler(IEnumerable<CodeInstruction> instructions)
        {
            var cfg = Plugin.Settings;
            var alvo = cfg?.DroneMaxDistance?.Value ?? VanillaMaxDistance;
            var ligado = cfg?.DroneEnabled?.Value ?? false;

            var trocas = 0;

            foreach (var instrucao in instructions)
            {
                if (ligado && EhLiteral500(instrucao))
                {
                    // O literal pode estar no IL como float ou como int, dependendo de
                    // como a comparação foi escrita. Devolver sempre no MESMO opcode que
                    // estava lá mantém a pilha com o tipo que o resto do método espera.
                    trocas++;
                    yield return instrucao.opcode == OpCodes.Ldc_R4
                        ? new CodeInstruction(OpCodes.Ldc_R4, alvo)
                        : new CodeInstruction(OpCodes.Ldc_I4, (int)alvo);
                    continue;
                }

                yield return instrucao;
            }

            SubstituicoesFeitas = trocas;
        }

        private static bool EhLiteral500(CodeInstruction instrucao)
        {
            if (instrucao.opcode == OpCodes.Ldc_R4 && instrucao.operand is float f)
                return Mathf.Approximately(f, VanillaMaxDistance);

            if (instrucao.opcode == OpCodes.Ldc_I4 && instrucao.operand is int i)
                return i == VanillaMaxDistance;

            return false;
        }
    }
}
