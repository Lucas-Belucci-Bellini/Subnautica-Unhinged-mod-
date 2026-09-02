using System;
using System.Collections;
using Nautilus.Assets;
using UnityEngine;

namespace SMLHelper.V2.Assets
{
    /// <summary>
    /// Base dos prefabs, como o SMLHelper V2 a expunha.
    ///
    /// Aqui mora a diferença de fundo entre as duas APIs: o SMLHelper era por **herança**
    /// (você deriva e sobrescreve <c>GetGameObject</c>, <c>ClassID</c>…), e o Nautilus é por
    /// **composição** (você monta um <see cref="CustomPrefab"/> e pendura gadgets nele).
    ///
    /// Esta classe faz a ponte: guarda um <see cref="CustomPrefab"/> por dentro e, no
    /// <see cref="Patch"/>, liga os membros sobrescritos aos gadgets equivalentes. A classe
    /// derivada continua escrita como sempre foi.
    /// </summary>
    public abstract class ModPrefab
    {
        protected ModPrefab(string classId, string prefabFileName, TechType techType = TechType.None)
        {
            ClassID = classId;
            PrefabFileName = prefabFileName;
            TechType = techType;
        }

        public string ClassID { get; protected set; }

        public string PrefabFileName { get; protected set; }

        public TechType TechType { get; protected set; }

        /// <summary>Pasta de assets do mod. O código legado sobrescreve isto.</summary>
        public virtual string AssetsFolder { get; } = string.Empty;

        /// <summary>Disparado antes do registro. Legado usa para preparar estado.</summary>
        public event Action OnStartedPatching;

        /// <summary>
        /// Disparado depois do registro — é aqui que o código legado costuma ajustar
        /// equipamento, quick slot e entradas de loja, já com o <see cref="TechType"/> válido.
        /// </summary>
        public event Action OnFinishedPatching;

        /// <summary>O <see cref="CustomPrefab"/> montado por <see cref="Patch"/>.</summary>
        protected CustomPrefab Prefab { get; private set; }

        /// <summary>Constrói o GameObject. É o que quase todo mod legado sobrescreve.</summary>
        public virtual GameObject GetGameObject() => null;

        /// <summary>Versão assíncrona; o legado sobrescreve uma OU outra.</summary>
        public virtual IEnumerator GetGameObjectAsync(IOut<GameObject> gameObject)
        {
            gameObject.Set(GetGameObject());
            yield break;
        }

        /// <summary>
        /// Registra o prefab no jogo. Equivale ao <c>Patch()</c> do SMLHelper.
        /// A ordem dos eventos reproduz a do legado: started → registro → finished.
        /// </summary>
        public virtual void Patch()
        {
            try
            {
                PatchInterno();
            }
            catch (Exception ex)
            {
                // ⚠️ Sem isto o relatorio MENTIA POR CONSTRUCAO: `AnotarFalha` existia e
                // nao era chamada de lugar nenhum, entao a linha "excecao no registro"
                // so podia sair 0 — nao porque nao houve excecao, mas porque ninguem
                // tinha como reportar uma. Contador que so sabe dizer zero e pior que
                // contador nenhum, porque alguem confia nele.
                Unhinged.Legacy.Diagnostico.RegistroDeConteudo.AnotarFalha(
                    ClassID, GetType().Namespace?.Split('.')[0], ex);

                // Relancar de proposito: o registro falhar NAO e cosmetico como o icone.
                // O codigo do mod costuma usar o TechType logo em seguida, e engolir a
                // falha aqui trocaria um estouro legivel por um efeito estranho adiante.
                throw;
            }
        }

        private void PatchInterno()
        {
            OnStartedPatching?.Invoke();

            // ⚠️ `unlockAtStart` do Nautilus tem padrao **false**; o do SMLHelper era
            // **true**. Chamar a sobrecarga curta registrava TODO item bloqueado — ele
            // existia como TechType mas nao aparecia nem no blueprint nem no construtor.
            // Passar explicitamente e o que reconcilia as duas convencoes.
            var info = PrefabInfo.WithTechType(
                ClassID, FriendlyName, Description, unlockAtStart: ResolverLiberadoNoInicio());
            if (!string.IsNullOrEmpty(PrefabFileName))
                info = info.WithFileName(PrefabFileName);

            // ⚠️ O ICONE NUNCA PODE DERRUBAR O ITEM. Foi exatamente isto que apagou 88
            // itens do FCS: `GetItemSprite()` do legado chama
            // `ImageUtils.LoadSpriteFromFile(<pasta de assets>/<ClassID>.png)`, e com o
            // arquivo ausente o Nautilus estoura NullReferenceException (ele le
            // `texture2D.width` de uma textura nula). A excecao subia daqui, pelo
            // `Patch()`, pelo `PatchSpawnables()` e saia pelo `[QModPatch]` do modulo —
            // matando o modulo inteiro no PRIMEIRO item com icone em arquivo.
            //
            // Um icone e cosmetico. Perde-lo custa uma imagem; deixa-lo propagar custa
            // a suite. O `LoadSpriteFromFile` da ponte ja contem o defeito do Nautilus,
            // e este try/catch cobre qualquer outra fonte de icone que falhe.
            Atlas.Sprite icon = null;
            string falhaIcone = null;
            try
            {
                icon = GetItemSprite();
            }
            catch (Exception ex)
            {
                falhaIcone = "icone: " + ex.GetType().Name + ": " + ex.Message;
            }
            if (icon != null)
                info = info.WithIcon(icon);

            TechType = info.TechType;

            Prefab = new CustomPrefab(info);
            Prefab.SetGameObject(GetGameObjectAsync);

            ConfigurePrefab(Prefab);

            Prefab.Register();

            // Anotar DEPOIS do Register, com o TechType que o Nautilus devolveu — e nao
            // com o que pretendiamos registrar. `TechType.None` aqui significa que o
            // registro falhou em silencio, e e a diferenca entre "o item nao aparece
            // porque esta trancado" e "o item nao existe".
            Unhinged.Legacy.Diagnostico.RegistroDeConteudo.Anotar(new Unhinged.Legacy.Diagnostico.RegistroDeConteudo.Entrada
            {
                ClassID = ClassID,
                Modulo = GetType().Namespace?.Split('.')[0],
                TechType = TechType.ToString(),
                TechTypeValor = (int)TechType,
                TemIcone = icon != null,
                LiberadoNoInicio = ResolverLiberadoNoInicio(),
                FalhaIcone = falhaIcone,
            });

            OnFinishedPatching?.Invoke();
        }

        /// <summary>
        /// Ponto de extensão para as subclasses pendurarem gadgets (receita, PDA, …)
        /// antes do registro. Vazio nesta base: um prefab puro não tem receita.
        /// </summary>
        protected virtual void ConfigurePrefab(CustomPrefab prefab) { }

        /// <summary>
        /// Se a receita já nasce liberada. <c>false</c> nesta base — um prefab puro não
        /// tem receita para liberar. O <see cref="SMLHelper.V2.Assets.Spawnable"/>
        /// sobrescreve para <c>true</c>, que era o padrão do SMLHelper.
        /// </summary>
        public virtual bool UnlockedAtStart => false;

        /// <summary>
        /// O valor que de fato vai para o Nautilus. Existe separado de
        /// <see cref="UnlockedAtStart"/> porque o <c>Craftable</c> precisa cruzar essa
        /// intenção com o <c>RequiredForUnlock</c>: liberado desde o início e exigir
        /// tecnologia para liberar são coisas contraditórias.
        /// </summary>
        protected virtual bool ResolverLiberadoNoInicio() => UnlockedAtStart;

        /// <summary>
        /// Ícone do item. <c>protected</c>, não <c>public</c>: é assim que o SMLHelper o
        /// declarava, e as 16 classes do FCS que o sobrescrevem usam
        /// <c>protected override</c> — declarar mais aberto aqui dá CS0507.
        /// </summary>
        protected virtual Atlas.Sprite GetItemSprite() => null;

        /// <summary>
        /// Público, não interno: o código legado lê estes dois de fora da hierarquia
        /// (o FCS faz isso em 2 lugares), e `internal` dava CS0122.
        /// </summary>
        public virtual string FriendlyName => ClassID;

        public virtual string Description => string.Empty;
    }
}
