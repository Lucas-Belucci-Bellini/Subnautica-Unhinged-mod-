using System;
using System.Reflection;
using BepInEx.Configuration;
using HarmonyLib;

namespace Unhinged.Core.Interop
{
    /// <summary>
    /// Alcança o interior de outro mod em tempo de execução, sem referência de compilação.
    ///
    /// É isto que permite "mexer sem medo": um mod que trava um valor na config pode ter
    /// esse valor reescrito daqui; um mod que trava o valor em código pode ter o método
    /// patchado pelo Harmony. Em nenhum dos casos o código dele é copiado ou redistribuído.
    ///
    /// Toda operação é tolerante a falha e retorna <c>false</c> em vez de lançar: o alvo
    /// pode não estar instalado, pode ter mudado de versão, pode ter renomeado o campo.
    /// Derrubar o jogo porque um mod opcional mudou de forma seria o pior resultado.
    /// </summary>
    public static class ModBridge
    {
        /// <summary>
        /// Lê uma entrada de configuração de outro mod. O <paramref name="section"/> e a
        /// <paramref name="key"/> são os mesmos que aparecem no arquivo .cfg dele.
        /// </summary>
        public static bool TryGetConfig<T>(string guid, string section, string key, out ConfigEntry<T> entry)
        {
            entry = null;
            try
            {
                if (!ModRegistry.TryGet(guid, out var info)) return false;

                var config = info.Instance?.Config;
                if (config == null) return false;

                return config.TryGetEntry(section, key, out entry);
            }
            catch (Exception ex)
            {
                Plugin.Log?.LogWarning($"ModBridge: falha ao ler config {guid}/{section}/{key}: {ex.Message}");
                return false;
            }
        }

        /// <summary>
        /// Reescreve uma entrada de configuração de outro mod — inclusive além do limite
        /// que a UI dele aceitaria, já que o <c>AcceptableValueRange</c> só é aplicado na
        /// borda de entrada, não na atribuição.
        ///
        /// Um mod que lê a config a cada uso adota o valor na hora; um que a lê só no
        /// carregamento pode exigir patch no lugar. Por isso o retorno informa apenas que
        /// a escrita ocorreu — não que o mod passou a obedecê-la.
        /// </summary>
        public static bool TrySetConfig<T>(string guid, string section, string key, T value)
        {
            if (!TryGetConfig<T>(guid, section, key, out var entry)) return false;

            try
            {
                var previous = entry.Value;
                entry.Value = value;
                Plugin.Log?.LogInfo($"ModBridge: {guid}/{section}/{key}: {previous} → {value}");
                return true;
            }
            catch (Exception ex)
            {
                Plugin.Log?.LogWarning($"ModBridge: falha ao escrever {guid}/{section}/{key}: {ex.Message}");
                return false;
            }
        }

        /// <summary>
        /// Resolve um tipo pelo nome, em qualquer assembly carregada — a via de acesso ao
        /// código de um mod sem referenciá-lo em tempo de compilação.
        /// Aceita tanto <c>Namespace.Tipo</c> quanto o nome qualificado por assembly.
        /// </summary>
        public static Type FindType(string typeName)
        {
            if (string.IsNullOrEmpty(typeName)) return null;

            try
            {
                return AccessTools.TypeByName(typeName);
            }
            catch (Exception ex)
            {
                Plugin.Log?.LogWarning($"ModBridge: falha ao resolver tipo '{typeName}': {ex.Message}");
                return null;
            }
        }

        /// <summary>
        /// Localiza um método de outro mod para servir de alvo de patch do Harmony.
        /// <paramref name="parameterTypes"/> só é necessário para desambiguar sobrecargas.
        /// </summary>
        public static MethodInfo FindMethod(string typeName, string methodName, Type[] parameterTypes = null)
        {
            var type = FindType(typeName);
            if (type == null) return null;

            try
            {
                var method = AccessTools.Method(type, methodName, parameterTypes);
                if (method == null)
                    Plugin.Log?.LogWarning($"ModBridge: '{typeName}' existe, mas não tem o método '{methodName}'.");
                return method;
            }
            catch (Exception ex)
            {
                Plugin.Log?.LogWarning($"ModBridge: falha ao resolver {typeName}.{methodName}: {ex.Message}");
                return null;
            }
        }
    }
}
