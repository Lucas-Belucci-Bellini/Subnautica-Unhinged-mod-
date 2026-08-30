using Nautilus.Assets;
using Nautilus.Assets.Gadgets;

namespace SMLHelper.V2.Assets
{
    /// <summary>
    /// Item fabricável que ocupa um slot de equipamento — traje, tanque, ferramenta de mão.
    ///
    /// No SMLHelper o slot era declarado por herança (<c>override EquipmentType</c>); no
    /// Nautilus é um gadget (<c>SetEquipment</c>). Esta classe traduz um no outro, então a
    /// classe legada continua escrita como sempre foi.
    /// </summary>
    public abstract class Equipable : Craftable
    {
        protected Equipable(string classId, string friendlyName, string description)
            : base(classId, friendlyName, description) { }

        /// <summary>Em que slot o item entra. Abstrato porque no SMLHelper também era.</summary>
        public abstract EquipmentType EquipmentType { get; }

        /// <summary>
        /// Como o item se comporta na barra rápida. <c>None</c> = não vai para a barra —
        /// é o padrão do SMLHelper, e o que a maioria dos trajes usa.
        /// </summary>
        public virtual QuickSlotType QuickSlotType => QuickSlotType.None;

        protected override void ConfigurePrefab(CustomPrefab prefab)
        {
            base.ConfigurePrefab(prefab);

            prefab.SetEquipment(EquipmentType)
                  .WithQuickSlotType(QuickSlotType);
        }
    }
}
