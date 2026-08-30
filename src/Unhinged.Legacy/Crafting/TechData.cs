using System.Collections.Generic;
using System.Linq;

// Namespace do SMLHelper V2 de propósito: é o que faz a fonte legada compilar sem
// tocar nos `using`. Ver src/Unhinged.Legacy/README.md.
namespace SMLHelper.V2.Crafting
{
    /// <summary>
    /// Ingrediente de receita, como o SMLHelper V2 o expunha.
    ///
    /// O jogo moderno já tem <see cref="global::Ingredient"/> com o mesmo formato
    /// (<c>techType</c> + <c>amount</c>), e o <c>Nautilus.Crafting.RecipeData</c> trabalha
    /// com ele. Herdar em vez de duplicar faz esta classe ser aceita direto onde o
    /// Nautilus espera o tipo do jogo — sem conversão no meio.
    /// </summary>
    public class Ingredient : global::Ingredient
    {
        public Ingredient(TechType techType, int amount) : base(techType, amount) { }
    }

    /// <summary>
    /// Receita, como o SMLHelper V2 a expunha. Equivale ao
    /// <c>Nautilus.Crafting.RecipeData</c>, com os mesmos campos e nomes.
    ///
    /// Mantida como tipo próprio (e não um alias) porque o código legado escreve
    /// <c>new TechData { craftAmount = 1, Ingredients = { ... } }</c>, e essa forma de
    /// inicialização exige que a lista já exista.
    /// </summary>
    public class TechData
    {
        public int craftAmount { get; set; } = 1;

        public List<global::Ingredient> Ingredients { get; set; } = new List<global::Ingredient>();

        public List<TechType> LinkedItems { get; set; } = new List<TechType>();

        public int ingredientCount => Ingredients?.Count ?? 0;

        public int linkedItemCount => LinkedItems?.Count ?? 0;

        public TechData() { }

        public TechData(params global::Ingredient[] ingredients)
        {
            Ingredients = ingredients?.ToList() ?? new List<global::Ingredient>();
        }

        public TechData(List<global::Ingredient> ingredients)
        {
            Ingredients = ingredients ?? new List<global::Ingredient>();
        }

        public global::Ingredient GetIngredient(int index) => Ingredients[index];

        public TechType GetLinkedItem(int index) => LinkedItems[index];

        /// <summary>Converte para o tipo que o Nautilus consome.</summary>
        public Nautilus.Crafting.RecipeData ToRecipeData() => new Nautilus.Crafting.RecipeData
        {
            craftAmount = craftAmount,
            Ingredients = Ingredients ?? new List<global::Ingredient>(),
            LinkedItems = LinkedItems ?? new List<TechType>(),
        };

        /// <summary>
        /// Conversão implícita: deixa passar um <c>TechData</c> legado direto para
        /// qualquer API do Nautilus que peça <c>RecipeData</c>.
        /// </summary>
        public static implicit operator Nautilus.Crafting.RecipeData(TechData data)
            => data?.ToRecipeData();

        /// <summary>
        /// Sentido inverso: o <c>CraftDataHandler.GetTechData</c> legado devolvia
        /// <c>TechData</c>, e o Nautilus devolve <c>RecipeData</c>.
        /// </summary>
        public static implicit operator TechData(Nautilus.Crafting.RecipeData data)
            => data == null ? null : new TechData
            {
                craftAmount = data.craftAmount,
                Ingredients = data.Ingredients ?? new List<global::Ingredient>(),
                LinkedItems = data.LinkedItems ?? new List<TechType>(),
            };
    }
}
