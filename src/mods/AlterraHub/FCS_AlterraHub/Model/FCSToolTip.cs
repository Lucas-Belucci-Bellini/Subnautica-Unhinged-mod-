using System;
using System.Collections.Generic;
using System.Text;
using UnityEngine;
using UnityEngine.UI;

namespace FCS_AlterraHub.Model
{
    public class FCSToolTip : MonoBehaviour, ITooltip
    {
        public string Tooltip;
        public Func<string> ToolTipStringDelegate;
        public TechType TechType { get; set; }
        public Func<bool> RequestPermission { get; set; }
        public bool Description { get; set; } = false;
        void Awake() => Destroy(GetComponent<LayoutElement>());

        // PORTE — o Subnautica atual convergiu para a `ITooltip` que era do Below Zero:
        // `GetTooltip(TooltipData)` + `showTooltipOnDrag`, no lugar do antigo
        // `GetTooltip(out string, List<TooltipIcon>)`. O `#if BELOWZERO` original ja
        // trazia a forma nova, mas so com o texto simples; a logica rica (permissao,
        // TechType, receita bloqueada) morava no ramo do Subnautica. Aqui as duas se
        // juntam, porque agora e um jogo so.
        public bool showTooltipOnDrag => true;

        public void GetTooltip(TooltipData tooltip)
        {
            // ⚠️ DIVERGENCIA DELIBERADA do original: la, `RequestPermission` filtrava
            // apenas o TEXTO — os icones de `BuildTech` eram escritos de qualquer jeito,
            // porque o `out string` e a lista de icones eram parametros separados. No
            // jogo atual `BuildTech` escreve texto E icones no mesmo `TooltipData`, e
            // separa-los exigiria montar um TooltipData descartavel a cada quadro.
            // Optei por respeitar a permissao inteira: sem permissao, tooltip vazio.
            // Isso trata o vazamento de icone como o descuido que aparenta ser — se em
            // jogo ficar claro que era intencional, e aqui que se desfaz.
            if (!(RequestPermission?.Invoke() ?? false)) return;

            if (ToolTipStringDelegate != null)
            {
                Tooltip = ToolTipStringDelegate.Invoke();
            }

            if (TechType != TechType.None)
            {
                if (Description)
                {
                    Tooltip = InventoryItemView(TechType);
                }
                else
                {
                    // Escreve titulo, descricao e icones direto no TooltipData.
                    bool locked = !CrafterLogic.IsCraftRecipeUnlocked(TechType);
                    TooltipFactory.BuildTech(TechType, locked, tooltip);
                    return;
                }
            }

            tooltip.prefix.Append(Tooltip);
        }

        public static string InventoryItemView(TechType techType)
        {
            TooltipFactory.Initialize();
            StringBuilder stringBuilder = new StringBuilder();
            TooltipFactory.WriteTitle(stringBuilder, Language.main.Get(techType));
            TooltipFactory.WriteDescription(stringBuilder, Language.main.Get(TooltipFactory.techTypeTooltipStrings.Get(techType)));
            return stringBuilder.ToString();
        }
    }
}
