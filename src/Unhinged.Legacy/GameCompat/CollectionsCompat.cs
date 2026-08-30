using System.Collections.Generic;

// SEM `namespace`, de propósito — mesmo truque de HandReticleCompat.cs: extensão em
// namespace global vale em todo arquivo compilado, sem `using` nenhum, e portanto sem
// tocar na fonte de terceiro. Ver docs/PORTE-LEGADO.md §3.55.

/// <summary>
/// Métodos de coleção que o código legado usava e que não existem mais — uns porque o
/// jogo os removeu, outros porque nunca existiram no <c>net472</c>.
/// </summary>
public static class UnhingedCollectionsCompat
{
    /// <summary>
    /// O jogo expunha isto como extensão de <see cref="HashSet{T}"/>. O próprio
    /// <see cref="HashSet{T}.Add"/> já é "adiciona se não houver" e devolve se
    /// adicionou, então encaminhar é equivalente exato — não uma aproximação.
    /// </summary>
    public static bool AddIfNotPresent<T>(this HashSet<T> set, T item)
        => set != null && set.Add(item);

    /// <summary>
    /// <c>Queue&lt;T&gt;.TryDequeue</c> só chegou no .NET Core 2.0; o jogo roda em
    /// Mono/net472, onde <see cref="Queue{T}.Dequeue"/> lança se a fila está vazia.
    /// </summary>
    public static bool TryDequeue<T>(this Queue<T> queue, out T result)
    {
        if (queue == null || queue.Count == 0)
        {
            result = default(T);
            return false;
        }

        result = queue.Dequeue();
        return true;
    }
}
