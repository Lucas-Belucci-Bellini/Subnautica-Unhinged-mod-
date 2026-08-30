// SEM NAMESPACE, de propósito.
//
// Métodos de extensão só entram em escopo se o namespace deles estiver importado, e não há
// como saber que `using` cada arquivo legado tem. No namespace global eles valem em todo
// lugar, sem tocar numa linha da fonte legada — que é o ponto desta ponte.
//
// O jogo moderno reescreveu o HandReticle: SetInteractText / SetInteractTextRaw /
// SetUseTextRaw sumiram, e o destino do texto virou um parâmetro TextType em SetText /
// SetTextRaw. Como o membro de instância não existe mais, o compilador aceita a extensão.

/// <summary>
/// Restaura a API antiga do <see cref="HandReticle"/> sobre a atual.
/// Cobre ~43 das 48 chamadas do FCS; as 5 que passam <c>HandReticle.Hand.None</c> ainda
/// exigem edição na fonte, porque o tipo <c>Hand</c> deixou de existir e o erro acontece
/// no argumento, antes da resolução de sobrecarga.
/// </summary>
public static class HandReticleLegacyExtensions
{
    /// <summary>Texto principal da mão, traduzido pela chave.</summary>
    public static void SetInteractText(this HandReticle reticle, string primaryKey)
        => reticle.SetText(HandReticle.TextType.Hand, primaryKey, true, GameInput.Button.None);

    /// <summary>
    /// Texto principal e secundário. No jogo atual o secundário é um destino próprio
    /// (<c>HandSubscript</c>), e não mais um segundo argumento da mesma chamada.
    /// </summary>
    public static void SetInteractText(this HandReticle reticle, string primaryKey, string secondaryKey)
    {
        reticle.SetText(HandReticle.TextType.Hand, primaryKey, true, GameInput.Button.None);
        reticle.SetText(HandReticle.TextType.HandSubscript, secondaryKey, true, GameInput.Button.None);
    }

    public static void SetInteractText(this HandReticle reticle, string primaryKey, bool translate, GameInput.Button button)
        => reticle.SetText(HandReticle.TextType.Hand, primaryKey, translate, button);

    public static void SetInteractText(this HandReticle reticle, string primaryKey, string secondaryKey,
        bool translatePrimary, bool translateSecondary, GameInput.Button button)
    {
        reticle.SetText(HandReticle.TextType.Hand, primaryKey, translatePrimary, button);
        reticle.SetText(HandReticle.TextType.HandSubscript, secondaryKey, translateSecondary, button);
    }

    /// <summary>Texto literal, sem passar pela tradução.</summary>
    /// <summary>
    /// Forma de 5 argumentos com a ultima <c>bool</c>: no SMLHelper ela era
    /// <c>addInstructions</c>, não um <c>GameInput.Button</c> — daí o CS1503 quando só
    /// existia a sobrecarga com Button. O jogo atual nao tem esse conceito separado,
    /// entao a flag e absorvida: sem instrucao, sem botao.
    /// </summary>
    public static void SetInteractText(this HandReticle reticle, string primaryKey, string secondaryKey,
        bool translate1, bool translate2, bool addInstructions)
        => reticle.SetInteractText(primaryKey, secondaryKey, translate1, translate2,
            addInstructions ? GameInput.Button.LeftHand : GameInput.Button.None);

    public static void SetInteractTextRaw(this HandReticle reticle, string primary)
        => reticle.SetTextRaw(HandReticle.TextType.Hand, primary);

    public static void SetInteractTextRaw(this HandReticle reticle, string primary, string secondary)
    {
        reticle.SetTextRaw(HandReticle.TextType.Hand, primary);
        reticle.SetTextRaw(HandReticle.TextType.HandSubscript, secondary);
    }

    public static void SetUseTextRaw(this HandReticle reticle, string primary)
        => reticle.SetTextRaw(HandReticle.TextType.Use, primary);

    public static void SetUseTextRaw(this HandReticle reticle, string primary, string secondary)
    {
        reticle.SetTextRaw(HandReticle.TextType.Use, primary);
        reticle.SetTextRaw(HandReticle.TextType.UseSubscript, secondary);
    }
}
