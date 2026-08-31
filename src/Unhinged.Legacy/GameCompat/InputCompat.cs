using System.Text;

// SEM `namespace`, como os outros compat: vale em todo arquivo, sem `using`.

/// <summary>
/// O nome legível da tecla ligada a uma ação — o que o mod mostra ao jogador em
/// dicas do tipo "Use Paint Can: R".
///
/// O legado chamava <c>GameInput.GetBindingName(Button, BindingSet)</c>. Esse método
/// **não existe mais**: confirmado no metadata, a string "GetBindingName" não aparece
/// em lugar nenhum do <c>Assembly-CSharp</c>. O jogo passou a expor
/// <c>GetBinding(Device, Button, BindingSet)</c> — que agora exige o **dispositivo** —
/// e <c>AppendDisplayText</c>, que monta o texto já formatado para a tela.
///
/// Não dá para acrescentar um estático a uma classe do jogo, então em vez de uma
/// extensão isto é um helper próprio, e os 11 sítios do FCS passaram a chamá-lo.
/// </summary>
public static class UnhingedInput
{
    /// <summary>
    /// Equivalente ao <c>GameInput.GetBindingName</c> antigo. Usa o dispositivo
    /// primário atual, que é o que o jogador está usando de fato — o método antigo
    /// não recebia dispositivo porque, na época, só havia um caminho de binding.
    /// </summary>
    public static string GetBindingName(GameInput.Button button, GameInput.BindingSet bindingSet)
    {
        // `GetBinding` devolve o binding cru (ex.: "R"); pode vir nulo se a ação não
        // estiver ligada a nada nesse conjunto — daí o fallback para o nome da ação,
        // que é melhor do que devolver vazio no meio de uma frase.
        var binding = GameInput.GetBinding(GameInput.PrimaryDevice, button, bindingSet);
        if (!string.IsNullOrEmpty(binding))
        {
            // `GetDisplayText` transforma o binding cru no rótulo que a UI mostra.
            var display = GameInput.GetDisplayText(binding, null);
            return string.IsNullOrEmpty(display) ? binding : display;
        }

        var sb = new StringBuilder();
        GameInput.AppendDisplayText(button, sb, false);
        return sb.Length > 0 ? sb.ToString() : GameInput.AsString(button);
    }

    /// <summary>Sobrecarga de conveniência: conjunto primário, o caso de 100% dos usos.</summary>
    public static string GetBindingName(GameInput.Button button)
        => GetBindingName(button, GameInput.BindingSet.Primary);
}
