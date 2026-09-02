using System.IO;
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
    ///
    /// ⚠️ <b>E não é só encaminhamento.</b> O <c>Nautilus.Utility.ImageUtils</c> tem um
    /// defeito que derruba um mod inteiro quando falta um PNG — ver
    /// <see cref="LoadSpriteFromFile"/>. Esta classe é o lugar certo para contê-lo,
    /// porque vale para qualquer mod portado pela ponte, não só o FCS.
    /// </summary>
    public static class ImageUtils
    {
        public static Texture2D LoadTextureFromFile(string filePathToImage, TextureFormat format = TextureFormat.BC7)
        {
            // O Nautilus ja trata arquivo ausente aqui (loga e devolve null) — mas so
            // aqui. Repetir o File.Exists evita a linha de erro no log para o caso
            // esperado de "o operador ainda nao copiou os assets".
            if (string.IsNullOrEmpty(filePathToImage) || !File.Exists(filePathToImage))
                return null;

            return Nautilus.Utility.ImageUtils.LoadTextureFromFile(filePathToImage, format);
        }

        /// <summary>
        /// Carrega um sprite de arquivo, devolvendo <c>null</c> quando o arquivo não
        /// existe — que é o que o Nautilus documenta e <b>não</b> é o que ele faz.
        /// </summary>
        /// <remarks>
        /// ⚠️ <b>Este método existe por causa de um NullReferenceException no Nautilus,
        /// e ele custou 88 itens do FCS ao operador.</b>
        ///
        /// <c>Nautilus.Utility.ImageUtils.LoadSpriteFromFile</c> faz:
        /// <code>
        /// Texture2D texture2D = LoadTextureFromFile(path);   // devolve NULL se nao existe
        /// return LoadSpriteFromTexture(texture2D);           // →
        ///     Sprite.Create(texture2D, new Rect(0, 0, texture2D.width, ...))
        ///                                                          ^^^^^ NRE
        /// </code>
        /// O XMLdoc dele promete <i>"Will return a new Sprite instance if the file
        /// exists; Otherwise returns null"</i> — mas o código desreferencia o null
        /// antes de conseguir devolver. Arquivo ausente vira <b>exceção</b>, não null.
        ///
        /// Por que isso e catastrofico e nao cosmetico: o FCS chama isto de dentro do
        /// <c>GetItemSprite()</c>, que roda durante o <c>Patch()</c> do item. A excecao
        /// sobe pelo registro do item, pelo <c>PatchSpawnables()</c>, e sai pelo ponto
        /// de entrada <c>[QModPatch]</c> do modulo — abortando o modulo inteiro no
        /// primeiro item que tenha icone em arquivo. Um PNG que falta apaga 89 itens.
        /// </remarks>
        public static Atlas.Sprite LoadSpriteFromFile(string filePathToImage, TextureFormat format = TextureFormat.BC7)
        {
            var textura = LoadTextureFromFile(filePathToImage, format);
            if (textura == null) return null;

            return Nautilus.Utility.ImageUtils.LoadSpriteFromTexture(textura);
        }

        public static Atlas.Sprite LoadSpriteFromTexture(Texture2D texture2D)
        {
            // Mesmo motivo: o Nautilus le texture2D.width sem conferir.
            if (texture2D == null) return null;

            return Nautilus.Utility.ImageUtils.LoadSpriteFromTexture(texture2D);
        }
    }
}
