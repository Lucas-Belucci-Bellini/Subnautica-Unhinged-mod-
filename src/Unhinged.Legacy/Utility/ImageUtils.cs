using UnityEngine;

namespace SMLHelper.V2.Utility
{
    /// <summary>
    /// Equivale ao <c>SMLHelper.V2.Utility.ImageUtils</c>, encaminhando para o
    /// <c>Nautilus.Utility.ImageUtils</c>.
    ///
    /// A diferença de tipo é absorvida aqui: o Nautilus devolve
    /// <see cref="UnityEngine.Sprite"/>, e o código legado espera <c>Atlas.Sprite</c>.
    /// A conversão implícita de <see cref="Atlas.Sprite"/> cobre os dois sentidos.
    /// </summary>
    public static class ImageUtils
    {
        public static Texture2D LoadTextureFromFile(string filePathToImage, TextureFormat format = TextureFormat.BC7)
            => Nautilus.Utility.ImageUtils.LoadTextureFromFile(filePathToImage, format);

        public static Atlas.Sprite LoadSpriteFromFile(string filePathToImage, TextureFormat format = TextureFormat.BC7)
            => Nautilus.Utility.ImageUtils.LoadSpriteFromFile(filePathToImage, format);

        public static Atlas.Sprite LoadSpriteFromTexture(Texture2D texture2D)
            => Nautilus.Utility.ImageUtils.LoadSpriteFromTexture(texture2D);
    }
}
