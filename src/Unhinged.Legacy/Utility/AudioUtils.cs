using FMOD;
using FMODUnity;
using UnityEngine;

namespace SMLHelper.V2.Utility
{
    /// <summary>
    /// Canal de som, como o SMLHelper V2 o expunha. Cada valor mapeia para um bus do FMOD.
    ///
    /// Os caminhos vêm de <c>Nautilus.Utility.AudioUtils.BusPaths</c> — que é uma classe
    /// <c>partial</c> com valores DIFERENTES entre Subnautica e Below Zero. Referenciar a
    /// constante, em vez de copiar a string, garante o caminho certo por jogo; copiar teria
    /// gerado som mudo em silêncio.
    /// </summary>
    public enum SoundChannel
    {
        Master,
        Music,
        Voice,
        Ambient,
    }

    /// <summary>
    /// Equivale ao <c>SMLHelper.V2.Utility.AudioUtils</c>. Membros conferidos contra o uso
    /// real: <c>CreateSound</c> (10 usos no FCS/S.O.C.K.) e <c>PlaySound</c> (3).
    /// </summary>
    public static class AudioUtils
    {
        /// <summary>Bus raiz do FMOD. Todos os demais caminhos descendem dele.</summary>
        private const string MasterBus = "bus:/master";

        public static Sound CreateSound(string path, MODE mode = MODE.DEFAULT)
            => Nautilus.Utility.AudioUtils.CreateSound(path, mode);

        public static Sound CreateSound(AudioClip clip, MODE mode = MODE.DEFAULT)
            => Nautilus.Utility.AudioUtils.CreateSound(clip, mode);

        /// <summary>
        /// Toca um som num canal. O Nautilus trocou para <c>TryPlaySound</c>, que devolve
        /// o <see cref="Channel"/> e informa falha em vez de lançar.
        /// </summary>
        public static Channel PlaySound(Sound sound, SoundChannel channel = SoundChannel.Master)
        {
            Nautilus.Utility.AudioUtils.TryPlaySound(sound, ResolveBus(channel), out var result);
            return result;
        }

        private static string ResolveBus(SoundChannel channel)
        {
            switch (channel)
            {
                case SoundChannel.Music:
                    return Nautilus.Utility.AudioUtils.BusPaths.Music;
                case SoundChannel.Voice:
                    return Nautilus.Utility.AudioUtils.BusPaths.VoiceOvers;
                case SoundChannel.Ambient:
                    return Nautilus.Utility.AudioUtils.BusPaths.UnderwaterAmbient;
                default:
                    return MasterBus;
            }
        }
    }
}
