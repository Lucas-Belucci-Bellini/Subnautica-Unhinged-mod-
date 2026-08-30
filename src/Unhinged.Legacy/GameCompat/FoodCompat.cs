using System;
using System.Collections.Generic;

// SEM `namespace`, como os outros compat.

/// <summary>
/// A tabela peixe cru → peixe cozido, que o jogo apagou.
///
/// O legado lia <c>CraftData.cookedCreatureList</c>. Conferido no metadata: nem o campo
/// nem a string "cookedCreatureList" existem mais em lugar nenhum do
/// <c>Assembly-CSharp</c>. O que **continua existindo** são os próprios TechTypes
/// (<c>CookedPeeper</c>, <c>CookedReginald</c>, …).
///
/// Por isso a tabela é **derivada do enum do jogo**, não digitada aqui. Escrever a lista
/// à mão significaria inventar dado de jogo — exatamente o que este projeto não faz — e
/// ainda ficaria desatualizada na primeira atualização que mexesse em peixe. Derivando,
/// ela acompanha o jogo sozinha, e um peixe novo que siga a convenção entra de graça.
/// </summary>
public static class UnhingedFood
{
    private static Dictionary<TechType, TechType> _cooked;

    /// <summary>
    /// Cru → cozido. Montada uma vez, na primeira leitura.
    ///
    /// A convenção do jogo é <c>Cooked&lt;Nome&gt;</c> (Peeper → CookedPeeper), e ela
    /// vale inclusive nos compostos (LavaBoomerang → CookedLavaBoomerang). Só entra o
    /// par em que **os dois lados existem** no enum, então um "Cooked" órfão não vira
    /// entrada inventada.
    /// </summary>
    public static Dictionary<TechType, TechType> CookedCreatureList
    {
        get
        {
            if (_cooked != null) return _cooked;

            _cooked = new Dictionary<TechType, TechType>();
            foreach (TechType cru in Enum.GetValues(typeof(TechType)))
            {
                var nome = cru.ToString();

                // Um "CookedX" é o destino de alguém, não a origem de nada.
                if (nome.StartsWith("Cooked", StringComparison.Ordinal)) continue;

                if (Enum.TryParse("Cooked" + nome, out TechType cozido) && cozido != TechType.None)
                    _cooked[cru] = cozido;
            }

            return _cooked;
        }
    }

    /// <summary>Reconstrói a tabela. Útil depois de um mod registrar TechTypes novos.</summary>
    public static void Invalidate() => _cooked = null;
}
