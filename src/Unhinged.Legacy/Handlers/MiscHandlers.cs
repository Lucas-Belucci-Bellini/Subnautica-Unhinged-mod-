using UnityEngine;

namespace SMLHelper.V2.Handlers
{
    /// <summary>Registro de sprites. Encaminha 1:1 para o Nautilus.</summary>
    public static class SpriteHandler
    {
        public static void RegisterSprite(TechType techType, Sprite sprite)
            => Nautilus.Handlers.SpriteHandler.RegisterSprite(techType, sprite);

        public static void RegisterSprite(TechType techType, string filePathToImage)
            => Nautilus.Handlers.SpriteHandler.RegisterSprite(techType, filePathToImage);

        public static void RegisterSprite(SpriteManager.Group group, string id, Sprite sprite)
            => Nautilus.Handlers.SpriteHandler.RegisterSprite(group, id, sprite);

        public static void RegisterSprite(SpriteManager.Group group, string id, string filePathToImage)
            => Nautilus.Handlers.SpriteHandler.RegisterSprite(group, id, filePathToImage);
    }

    /// <summary>Entradas do PDA. Encaminha para o Nautilus.</summary>
    public static class PDAHandler
    {
        public static void AddCustomScannerEntry(PDAScanner.EntryData entryData)
            => Nautilus.Handlers.PDAHandler.AddCustomScannerEntry(entryData);

        public static void EditFragmentsToScan(TechType techType, int fragmentCount)
            => Nautilus.Handlers.PDAHandler.EditFragmentsToScan(techType, fragmentCount);

        public static void EditFragmentScanTime(TechType techType, float scanTime)
            => Nautilus.Handlers.PDAHandler.EditFragmentScanTime(techType, scanTime);
    }

    /// <summary>Sons personalizados.</summary>
    public static class CustomSoundHandler
    {
        public static ICustomSoundHandler Main { get; } = new MainShim();

        public static FMOD.Sound RegisterCustomSound(string id, string filePath, string busPath,
            FMOD.MODE mode = FMOD.MODE.DEFAULT)
            => Nautilus.Handlers.CustomSoundHandler.RegisterCustomSound(id, filePath, busPath, mode);

        public static FMOD.Sound RegisterCustomSound(string id, AudioClip clip, string busPath,
            FMOD.MODE mode = FMOD.MODE.DEFAULT)
            => Nautilus.Handlers.CustomSoundHandler.RegisterCustomSound(id, clip, busPath, mode);

        private sealed class MainShim : ICustomSoundHandler
        {
            public FMOD.Sound RegisterCustomSound(string id, string filePath, string busPath)
                => CustomSoundHandler.RegisterCustomSound(id, filePath, busPath);

            public FMOD.Sound RegisterCustomSound(string id, string filePath)
                => CustomSoundHandler.RegisterCustomSound(id, filePath, "bus:/master");
        }
    }

    public interface ICustomSoundHandler
    {
        FMOD.Sound RegisterCustomSound(string id, string filePath, string busPath);

        /// <summary>Sem bus explícito: cai no bus raiz, como o SMLHelper fazia.</summary>
        FMOD.Sound RegisterCustomSound(string id, string filePath);
    }

    /// <summary>
    /// Tipos de ping personalizados.
    ///
    /// O Nautilus **removeu** o <c>PingTypeHandler</c> — não há tipo <c>Ping*</c> nele.
    /// Conforme o guia oficial, os handlers por enum viraram o <c>EnumHandler</c>
    /// genérico. O sprite, que o handler antigo registrava junto, passa a ser registrado
    /// à parte no grupo <c>Pings</c>.
    /// </summary>
    public static class PingHandler
    {
        public static PingType RegisterNewPingType(string pingName, Sprite pingSprite)
        {
            var pingType = Nautilus.Handlers.EnumHandler.AddEntry<PingType>(pingName).Value;

            if (pingSprite != null)
                Nautilus.Handlers.SpriteHandler.RegisterSprite(SpriteManager.Group.Pings, pingName, pingSprite);

            return pingType;
        }
    }
}

namespace SMLHelper.V2.Utility
{
    /// <summary>
    /// Utilidades de save. O Nautilus moveu isto de <c>InGameMenuHandler</c> para
    /// <c>Nautilus.Utility.SaveUtils</c> — mudança descrita no guia oficial.
    /// </summary>
    public static class SaveUtils
    {
        public static string GetCurrentSaveDataDir()
            => Nautilus.Utility.SaveUtils.GetCurrentSaveDataDir();

        public static void RegisterOnSaveEvent(System.Action onSave)
            => Nautilus.Utility.SaveUtils.RegisterOnSaveEvent(onSave);

        /// <summary>
        /// O Nautilus marcou o alvo como obsoleto em favor de
        /// <c>WaitScreenHandler.RegisterLateLoadTask()</c>, que é baseado em task. Este
        /// shim existe para expor a API ANTIGA, cuja assinatura é um <see cref="System.Action"/>,
        /// então o encaminhamento é mantido de propósito — trocar aqui mudaria o contrato
        /// que o código legado espera. Quem escrever código novo deve usar o Nautilus direto.
        /// </summary>
#pragma warning disable CS0618 // encaminhamento intencional para a API legada
        public static void RegisterOnFinishLoadingEvent(System.Action onLoad)
            => Nautilus.Utility.SaveUtils.RegisterOnFinishLoadingEvent(onLoad);
#pragma warning restore CS0618
    }
}
