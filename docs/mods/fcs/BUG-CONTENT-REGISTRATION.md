# P0 — CONTENT REGISTRATION FAILURE (FCS)

**Estado: causa encontrada e corrigida no código. Correção NÃO verificada em jogo.**

Diagnosticado a partir de evidência do jogo do operador — `Unhinged-RegistroFCS.md`
e `LogOutput.log` da execução da **1.2.2**, em 2026-09-02.

## Sintoma

Os itens do FCS não aparecem no save, **nem depois de `unlock all`**.

## O que a evidência mostrou

O registro escreveu **1 item**, de 7 módulos:

```
| 1 | FCSDataBox | FCS_AlterraHub | FCSDataBox | 10886 | — | sim |

tentaram registrar      1
TechType criado         1
nascem BLOQUEADOS       0
```

E o log fechou a conta:

```
Alterra Hub: 0 ponto(s) de entrada executado(s).
```

**Zero pontos de entrada concluíram.** Os sete abortaram — por **duas causas
diferentes e independentes**.

---

## Causa A — o ícone que faltava derrubava o módulo (1 módulo)

```
FCS_AlterraHub.QPatch.Patch falhou: System.NullReferenceException
  at Nautilus.Utility.ImageUtils.LoadSpriteFromTexture (Texture2D texture2D)
  at Nautilus.Utility.ImageUtils.LoadSpriteFromFile (String filePathToImage, ...)
  at SMLHelper.V2.Utility.ImageUtils.LoadSpriteFromFile (...)
  at FCS_AlterraHub.Mods.Global.Spawnables.DebitCardSpawnable.GetItemSprite ()
  at SMLHelper.V2.Assets.ModPrefab.Patch ()
  at FCS_AlterraHub.QPatch.PatchSpawnables ()
```

O `Nautilus.Utility.ImageUtils.LoadSpriteFromFile` tem um defeito:

```csharp
Texture2D texture2D = LoadTextureFromFile(path);   // devolve NULL se o arquivo nao existe
return LoadSpriteFromTexture(texture2D);           // →
    Sprite.Create(texture2D, new Rect(0, 0, texture2D.width, ...))
                                                          ^^^^^ NullReferenceException
```

O XMLdoc dele promete *"Otherwise returns null"*, mas o código desreferencia o null
antes de conseguir devolver. **Arquivo de ícone ausente vira exceção, não null.**

Por que isso apaga 89 itens e não 1: o `GetItemSprite()` roda **dentro** do
`Patch()` do item. A exceção sobe pelo item, pelo `PatchSpawnables()` e sai pelo
`[QModPatch]` — abortando o módulo no **primeiro** item que tenha ícone em arquivo.

`PatchSpawnables()` registra nesta ordem:

| # | item | ícone de arquivo? | resultado |
| ---: | --- | :---: | --- |
| 1 | `FCSDataBox` | **não** (não sobrescreve `GetItemSprite`) | ✅ registrou — é o único do relatório |
| 2 | `DebitCard` | sim | ❌ NRE, módulo abortado |
| 3+ | … | sim | nunca chamados |

São **89 chamadas** de `LoadSpriteFromFile` no pacote. O `FCSDataBox` sobreviveu por
ser um dos poucos sem ícone próprio — e é por isso que o relatório mostrava
`icone: —` justamente no único item vivo. A pista estava na própria coluna.

### Correção A

`src/Unhinged.Legacy/Utility/ImageUtils.cs` — `File.Exists` antes, e guarda de null
antes do `LoadSpriteFromTexture`. Vale para **qualquer** mod portado pela ponte.

`src/Unhinged.Legacy/Assets/ModPrefab.cs` — `GetItemSprite()` passa a rodar em
`try/catch`. Um ícone que falha vira `FalhaIcone` no relatório e **o item registra
mesmo assim**. Ícone é cosmético; item não é.

---

## Causa B — o assembly fundido colidia no painel de opções (6 módulos)

```
CyclopsUpgradeConsole.QPatch.Patch falhou: System.TypeInitializationException
  ---> System.ArgumentException: An item with the same key has already been added.
                                 Key: Unhinged.AlterraHub
  at System.Collections.Generic.SortedList`2.Add (TKey key, TValue value)
  at Nautilus.Handlers.OptionsPanelHandler.RegisterModOptions (ModOptions options)
  at CyclopsUpgradeConsole.QPatch..cctor ()
```

Idêntico nos seis: `FCS_EnergySolutions`, `FCS_HomeSolutions`,
`FCS_LifeSupportSolutions`, `FCS_ProductionSolutions`, `FCS_StorageSolutions` e
`CyclopsUpgradeConsole`.

Os sete módulos declaram, em **inicializador estático**:

```csharp
internal static Config Configuration { get; } = OptionsPanelHandler.Main.RegisterModOptions<Config>();
```

O Nautilus guarda os painéis numa `SortedList` indexada pelo **nome do assembly**.
Enquanto cada módulo era um DLL próprio, as sete chaves eram distintas. Fundidos num
assembly só, as sete viraram `Unhinged.AlterraHub`: a primeira entra, a segunda
estoura.

**E o lugar é o pior possível.** É o construtor estático. Um
`TypeInitializationException` fica memorizado pelo runtime — o tipo não volta a
funcionar naquela sessão. O módulo morre antes do primeiro item.

> Esta é a **terceira** vez que o assembly fundido morde, e sempre pelo mesmo
> mecanismo: código que assumia "um DLL = um mod". As anteriores foram o
> `PatchAll` aplicando 129 patches 7× (v1.0.5) e os caminhos de asset por módulo
> (v1.0.3/1.0.7). Sempre que o legado usar `Assembly.GetExecutingAssembly()` como
> **identidade**, e não como conteúdo, o mesmo defeito reaparece.

### Correção B

`src/Unhinged.Legacy/Options/OptionsPanelHandler.cs` — a duplicata deixa de ser
fatal. O painel só mostra uma entrada por assembly de qualquer forma; perder a
entrada do menu é aceitável, perder o módulo não. A config em si continua valendo
(é um JSON próprio), então é carregada direto do disco e indexada **por tipo** — que
é o que realmente distingue os sete.

---

## ⚠️ O que a correção NÃO resolve: os assets não estão instalados

Do mesmo log, antes de tudo:

```
Unable to open archive file: .../BepInEx/plugins/AlterraHub/Assets/fcsalterrahubbundle
Failed to read data for the AssetBundle '...fcsalterrahubbundle'.
```

Esse erro o FCS **captura e engole** (`QuickLogger.Error`), então ele não aborta nada
— mas significa que os bundles do FCStudios não foram copiados.

| depois das correções A e B | |
| --- | --- |
| TechTypes criados | ✅ sim |
| receitas registradas | ✅ sim |
| aparece no PDA / construtor | ✅ sim |
| **modelo e ícone** | ❌ **não, sem os assets** |

Ou seja: as correções fazem o **conteúdo existir** (que é o P0). Deixá-lo
**utilizável** ainda depende de copiar as 7 pastas do FCS — ver `INSTALL.md`.

---

## Como o diagnóstico falhou em me dizer isso, e o que mudou

Três defeitos no próprio instrumento, todos corrigidos:

1. **`AnotarFalha` não era chamada de lugar nenhum.** A linha
   `excecao no registro | 0` do relatório do operador não era informação: o contador
   era estruturalmente incapaz de ser diferente de zero.
2. **`AnotarFalha` não escrevia no disco** — só somava na lista em memória. Numa
   carga que aborta, a lista morre com o processo.
3. **Não havia nível de MÓDULO.** O relatório não conseguia distinguir "o módulo
   rodou e não registrou nada" de "o módulo estourou no `..cctor`" — e a resposta
   estava só no `LogOutput.log`, que é o arquivo que ninguém garimpa.

Agora o registro traz uma **linha do tempo da carga**: qual módulo entrou, qual
concluiu, qual abortou e com que exceção — com as 8 primeiras linhas da pilha, no
próprio arquivo.

---

## Regressão a evitar

Qualquer coisa **cosmética ou opcional** (ícone, painel de opções, comando de
console, som) que estoure durante o registro **não pode** derrubar o item nem o
módulo. Toda vez que este projeto quebrou, foi por isso.

## Verificação

| | |
| --- | --- |
| compila | ✅ |
| portões de patch/módulo | ✅ |
| diagnóstico testado (tabela + linha do tempo + falhas) | ✅ |
| **os 7 módulos registram em jogo** | ⬜ **não verificado** |
| itens aparecem no PDA/construtor | ⬜ não verificado |
| itens com modelo (requer assets) | ⬜ não verificado |
