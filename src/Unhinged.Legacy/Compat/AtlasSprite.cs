using UnityEngine;

// O jogo moderno NÃO tem mais `Atlas.Sprite`: ele foi substituído por UnityEngine.Sprite.
// Confirmado lendo Assembly-CSharp e Assembly-CSharp-firstpass — o namespace Atlas não
// existe em nenhuma das duas. Como 85 arquivos só do FCS declaram
// `using Sprite = Atlas.Sprite;`, devolver o tipo aqui evita editar os 85.
namespace Atlas
{
    /// <summary>
    /// Invólucro de compatibilidade sobre <see cref="UnityEngine.Sprite"/>.
    ///
    /// Deliberadamente mínimo: só o que se confirmou necessário. Membros novos entram
    /// quando o porte de fato os exigir — inventar a superfície antiga de cor seria
    /// justamente o erro que este projeto evita.
    /// </summary>
    public class Sprite
    {
        /// <summary>Sprite da Unity por trás. Pode ser nulo se a textura não carregou.</summary>
        public UnityEngine.Sprite UnitySprite { get; }

        public Sprite(UnityEngine.Sprite sprite) => UnitySprite = sprite;

        public Sprite(Texture2D texture) => UnitySprite = texture == null
            ? null
            : UnityEngine.Sprite.Create(
                texture,
                new Rect(0f, 0f, texture.width, texture.height),
                new Vector2(0.5f, 0.5f));

        public Texture2D texture => UnitySprite == null ? null : UnitySprite.texture;

        public Vector2 size => UnitySprite == null ? Vector2.zero : UnitySprite.rect.size;

        /// <summary>Nome que o <c>Atlas.Sprite</c> legado expunha.</summary>
        public UnityEngine.Sprite AsUnitySprite() => UnitySprite;

        // As conversões implícitas são o que faz o código legado atravessar sem edição:
        // ele passa um Atlas.Sprite onde o Nautilus e o jogo moderno querem UnityEngine.Sprite.
        public static implicit operator UnityEngine.Sprite(Sprite sprite) => sprite?.UnitySprite;

        public static implicit operator Sprite(UnityEngine.Sprite sprite)
            => sprite == null ? null : new Sprite(sprite);
    }
}
