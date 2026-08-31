# Como portar um mod legado (QModManager/SMLHelper) para o jogo atual

Procedimento derivado do `Unhinged.Legacy`. A ideia é que o porte vire **mecânico**: a
ponte já traduz a API, então sobra trocar referências e corrigir o que o jogo mudou.

## 1. Trocar as referências no `.csproj`

| Sai | Entra |
| --- | --- |
| `SMLHelper.dll` | `Unhinged.Legacy.dll` |
| `QModManager.*.dll` | `Unhinged.Legacy.dll` (traz os atributos `[QMod*]`) |
| `Assembly-CSharp.dll` local | pacote `Subnautica.GameLibs` |
| — | `BepInEx.Core` 5.4.21 + `Nautilus.dll` |

Os `using SMLHelper.V2.*` **não mudam**: a ponte reexpõe os mesmos namespaces.

## 2. As três quebras do jogo moderno

Estas o compilador aponta, e são as únicas edições realmente necessárias na fonte:

### `Atlas.Sprite` não existe mais
O namespace `Atlas` sumiu das duas assemblies do jogo. **Já resolvido pela ponte** —
`Compat/AtlasSprite.cs` devolve o tipo com conversão implícita nos dois sentidos.
Nenhuma edição na fonte legada (só no FCS seriam 85 arquivos).

### ⚠️ `TechData` colide com um tipo estático do jogo
O jogo moderno tem um `TechData` **estático no namespace global**. Pela regra de resolução
de nomes do C#, membros do namespace ganham dos `using` em escopo de arquivo — então
`TechData` resolve para o do jogo e o build morre com **`CS0722`/`CS0721`**.

A ponte **não consegue** resolver isto por você: é resolução de nome na fonte legada.
A correção é uma linha por arquivo afetado:

```csharp
using TechData = SMLHelper.V2.Crafting.TechData;
```

Aplicável em lote nos arquivos que o compilador apontar.

### `PlaySound` mudou de forma
Virou `TryPlaySound`, com o canal expresso por caminho de bus do FMOD. A ponte mantém
`PlaySound(sound, SoundChannel)` e traduz. **Importante:** os caminhos de bus **diferem
entre Subnautica e Below Zero** — a ponte referencia
`Nautilus.Utility.AudioUtils.BusPaths`, que é `partial` por jogo. Copiar a string em vez
de referenciar a constante dá som mudo, em silêncio.

## 3. Ponto de entrada

`[QModCore]` e `[QModPatch]` só **marcam** código — quem os executava era o QModManager,
que não existe aqui. A ponte fornece os atributos (para compilar) e o executor:

```csharp
[BepInPlugin("com.exemplo.modlegado", "Mod Legado", "1.0.0")]
[BepInDependency("com.snmodding.nautilus")]
public class Plugin : BaseUnityPlugin
{
    private void Awake() => LegacyModLoader.Run(typeof(ClasseDoMod).Assembly, Logger);
}
```

`LegacyModLoader` respeita a ordem original (pré-patch → patch → pós-patch), desembrulha
`TargetInvocationException` para o log ficar legível, e **isola falhas**: um mod quebrado
não derruba os outros.

## 3.5 Resultado medido: Alterra Hub compilado contra a ponte

Compilado de verdade — 225 arquivos do `FCS_AlterraHub` mais o shared project `FCSCommon`,
contra `Unhinged.Legacy` + `Subnautica.GameLibs` + Nautilus 1.0.0-pre.53.

### ⚠️ Duas armadilhas de medição, ambas encontradas na prática

**1. Case-sensitivity engana.** O `.csproj` diz `Mono\`, o disco tem `mono/`. No Windows
dá na mesma; no Linux, 27 arquivos "somem" e parece que a fonte publicada está incompleta.
**Não está** — os 225 resolvem, com comparação case-insensitive. Uma conclusão errada aqui
teria mandado o porte reimplementar um subsistema inteiro que já existe.

**2. Contagem de erro baixa pode ser o compilador desistindo.** Com um `CS0576` no
`Mod.cs`, o total parecia **6**. Corrigido esse erro, o compilador foi adiante e o número
real apareceu: **164**. Erro fatal cedo mascara o resto — comemorar contagem baixa sem
antes eliminar os fatais é enganar a si mesmo.

**3. Grepar pelo namespace não mede cobertura da ponte.** Nenhum dos 164 erros escreve
"SMLHelper", mas vários nomes ausentes — `SpriteHandler`, `PDAHandler`,
`OptionsPanelHandler`, `CustomSoundHandler`, `PingHandler`, `SaveUtils`, `QModServices` —
**são** API legada que falta na ponte. O erro diz só o nome do tipo, não de onde ele vinha.

### O estado real

| Categoria | Erros | Natureza |
| --- | ---: | --- |
| Handlers legados ainda ausentes na ponte | ~26 | `SpriteHandler`, `PDAHandler`, `OptionsPanelHandler`, `CustomSoundHandler`, `PingHandler`, `SaveUtils`, `IIngredient`, `QModServices` |
| API do jogo mudou | ~90 | `CraftData.GetItemSize`, `CraftData.techData`, `CraftData.cookedCreatureList` e outros membros removidos |
| Unity/UI mudou | ~10 | `Text` → `TMPro.TextMeshProUGUI` |
| Spawn passou para o Nautilus | ~16 | `CoordinatedSpawnsHandler`, `SpawnInfo` — existem no Nautilus, precisam do `using` certo |
| Resto | ~22 | construtores, acessibilidade, conversões |

➡️ **A ponte encurtou o trabalho, não o eliminou.** As bases de asset, crafting, idioma,
áudio e o carregador funcionam; falta uma segunda leva de handlers, e há migração de API do
jogo que **nenhum shim absorve** — é edição de fonte, arquivo a arquivo.

### Regra de porte que este teste revelou

**Onde o FCS tem `#if BELOWZERO`, o Subnautica moderno costuma precisar do ramo do Below
Zero.** O `ITooltip` é o caso exemplar: o FCS já implementava `showTooltipOnDrag` e
`GetTooltip(TooltipData)`, mas só sob `#if BELOWZERO` — porque o Subnautica de então tinha
a API antiga. Os dois jogos convergiram. Antes de escrever implementação nova, **procure se
o ramo BZ já a tem.**

### Correção à orientação do §2

O alias do `TechData` **precisa ficar dentro do `namespace`**, não no topo do arquivo:

```csharp
namespace MeuMod
{
    using TechData = SMLHelper.V2.Crafting.TechData;   // ✅ aqui
```

Em escopo de arquivo ele perde para o tipo global do jogo e dá **`CS0576`** — verificado
compilando, depois de eu ter documentado errado a primeira vez.

### Correções que o teste provocou na ponte

`GetItemSprite` tinha de ser `protected` (16 classes do FCS o sobrescrevem assim);
faltavam `UnlockedAtStart`, `EntityInfo` (`UWE.WorldEntityInfo`) e `DiscoverMessage` nas
bases; e faltava a propriedade **`Order`** nos atributos de opções — usada pelo FCS para
ordenar o painel, e invisível em qualquer leitura que não fosse compilar.

## 3.55 Truque que evitou 46 edições: extensão em namespace global

O `HandReticle` sozinho era **48 dos 152 erros**. A saída não foi editar 48 lugares na
fonte do FCS — foi devolver a API antiga como **métodos de extensão declarados sem
namespace**.

Funciona por duas razões que se somam:

1. **O membro de instância não existe mais**, então o compilador aceita a extensão. (Se
   ainda existisse, o método de instância venceria e a extensão seria ignorada.)
2. **Sem `namespace`, a extensão vale em todo arquivo**, sem `using`. Isso importa porque
   não há como saber quais `using` cada arquivo legado tem — e o objetivo é não tocar neles.

```csharp
// sem namespace, de propósito
public static class HandReticleLegacyExtensions
{
    public static void SetInteractText(this HandReticle r, string key)
        => r.SetText(HandReticle.TextType.Hand, key, true, GameInput.Button.None);
}
```

**Resultado: 152 → 106 erros, com zero edição na fonte de terceiro.**

⚠️ **Onde o truque NÃO alcança**, e é bom saber a fronteira:

- **Tipo de argumento que sumiu.** 5 chamadas passam `HandReticle.Hand.None`; o tipo `Hand`
  não existe mais, e o erro acontece no argumento, **antes** da resolução de sobrecarga.
  Extensão não ajuda — é edição na fonte (`GameInput.Button.None`).
- **Membro estático removido de classe estática.** `CraftData.GetItemSize` migrou para
  `TechData.GetItemSize` (mesma assinatura, devolve `Vector2int`). Não dá para estender uma
  chamada estática: é `sed` de `CraftData.GetItemSize` → `TechData.GetItemSize`.

## 3.6 Progressão medida do Alterra Hub

| Passo | Erros |
| --- | ---: |
| Primeira tentativa | **586** |
| `FCSCommon` (shared project) + `Newtonsoft.Json` | 216 |
| Primeira leva da ponte (Options, Commands, Json, bases) | 152 |
| `HandReticle` por extensão em namespace global | 106 |
| `EndCreditsManager` → Postfix + seds | 70 |
| `CoordinatedSpawnsHandler` + `ConsoleCommandsHandler` | 60 |
| Acessibilidade, ctors e sobrecargas que faltavam na ponte | 44 |
| `SpawnInfo`, `Ocean.GetDepthOf`, `Subtitles.Add` (seds) | **30** |

### ⚠️ Armadilha do padrão de reexportação: `CS0104`

Ao adicionar `using Nautilus.Handlers;` num arquivo legado para achar o `SpawnInfo`,
o build passou a acusar **ambiguidade**: o shim reexporta `CoordinatedSpawnsHandler`
com o mesmo nome do Nautilus, e os dois namespaces importados juntos empatam.

**Regra:** para alcançar um tipo do Nautilus a partir de código legado, use **alias do
tipo**, nunca `using` do namespace inteiro:

```csharp
using SpawnInfo = Nautilus.Handlers.SpawnInfo;   // ✅
using Nautilus.Handlers;                          // ⛔ CS0104 com o shim
```

Isso vale sempre que o shim e o Nautilus expõem o mesmo nome — que é a maioria dos
handlers, por construção.

`SpawnInfo` precisa de alias porque é **`sealed`** no Nautilus: não dá para reexportá-lo
por herança como foi feito com `ConfigFile` e `ModOptions`.

### Os 30 restantes: 12 problemas em 9 arquivos

Nenhum é da ponte. São membros removidos ou trocados pelo jogo/Unity, e **nenhum é
mecânico** — cada um exige entender o que o código pretendia:

| Arquivo | Problema |
| --- | --- |
| `WorldHelpers.cs` | `CraftData.techData` e o tipo `CraftData.TechData` sumiram |
| `BaseManager.cs` | `CraftData.cookedCreatureList` sumiu |
| `FCSPDAController.cs` | `PDA.screen` sumiu |
| `PlayerPatch.cs` | `Player.pdaSpawn` sumiu; e um argumento virou `Transform` |
| `EncyclopediaTabController.cs`, `FCSAlterraHubService.cs` | `EntryData.timeCapsule` sumiu |
| `AlterraHubModelPrefab.cs` | `Text` → `TMPro.TextMeshProUGUI` |
| `FCSGrowingPlant.cs` | prefab virou `AssetReferenceGameObject` (Addressables) |
| `SearchField.cs` | `InputField.SetText` sumiu |
| `PowercellSlot.cs` | argumento virou `GameInput.Button` |

Os dois do `CraftData` são os mais profundos: eram a base de receitas do vanilla lida
direto. Não existem em `CraftData`, `TechData` nem `CraftDataUtils` — ler receita vanilla
agora passa pelo Nautilus, o que muda o **desenho** daquele código, não o nome.

## 3.7 Medição da suíte FCS inteira: 81 erros em 47 dos 636 arquivos

Os sete módulos FCS compilados **juntos**, contra a ponte, com o `Subnautica.GameLibs`
82304 e o Nautilus 1.0.0-pre.53. É a resposta para "quanto falta".

| | |
| --- | --- |
| arquivos compilados | **636** (a lista vem dos `.csproj` dos autores) |
| arquivos com erro | **47** (7,4%) |
| erros únicos | **81** |
| sintomas distintos | **33** |

### ⚠️ Três armadilhas de medição, todas encontradas neste teste

Antes destes números eu produzi 16, depois 10, depois 3 — **os três estavam errados**,
cada um por um motivo diferente. Vale mais registrar os motivos do que os números.

1. **Mascaramento por fase.** `CS0246`/`CS0115` são erros de **declaração**; o Roslyn
   aborta antes de ligar corpos de método, e todo erro tipo `CS1061` some do relatório.
   Um número baixo enquanto ainda houver erro de declaração **não quer dizer nada**.
   Foi assim que 3 erros viraram 81 ao completar um stub de três membros.
2. **Corpus incompleto.** Minha lista de compilação vinha dos `.csproj` dos autores, mas
   com casamento de caminho sensível a maiúsculas: o `.csproj` diz `Mono\`, o disco tem
   `mono/`. No Windows deles dá na mesma; aqui, 21 arquivos sumiram em silêncio — entre
   eles os dois que este mesmo teste deveria exercitar. **Compare sempre a contagem de
   `<Compile>` com a do disco.**
3. **Código morto no repositório.** Outros 13 arquivos existem no disco mas **não estão
   em nenhum `.csproj` dos autores** (`FCS_AlterraHub/Mono/AlterraHub/*`, os
   `Mods/Stairs/Patchers/*`). São fonte antiga que eles deixaram na árvore. Incluí-los
   inventa 23 erros que não existem. **A lista de compilação é a dos autores, não a do
   `find`.**

Os `AssemblyInfo.cs` continuam fora: sete módulos viram **um** assembly nesta medição, e
os atributos duplicados dão `CS0579`. Isso é artefato da medição, não do porte.

Dois stubs de medição (`MoreCyclopsUpgrades`, `NAudio`) vivem só no scratchpad e **nunca
entram no repositório** — existem para o compilador passar da fase de declaração.

### Os 81, em três baldes

| balde | erros | o que é |
| --- | --- | --- |
| **A. Mecânico** | ~38 | Uma linha na ponte, ou uma troca de nome de chamada, resolve vários sítios de uma vez. Sem decisão de projeto. |
| **B. A API sumiu do jogo** | ~29 | O símbolo **não existe mais** no `Assembly-CSharp` — não migrou, foi apagado. Precisa de um caminho novo, e isso é decisão de comportamento. |
| **C. O jogo trocou de tecnologia** | ~13 | `UnityEngine.UI.Text` → `TMPro.TextMeshProUGUI`, e prefab direto → `Addressables`. Edição sítio a sítio. |

#### Balde A — mecânico

| sintoma | sítios | conserto |
| --- | --- | --- |
| `TechData.GetItemSize` | 5 | O alias `using TechData = SMLHelper...` esconde o `TechData` **estático global** do jogo, que é onde o método passou a morar. Somar os estáticos à classe da ponte, encaminhando, resolve os dois sentidos no mesmo arquivo. |
| `CraftData.GetItemSize` / `GetEquipmentType` / `GetCraftTime` / `craftingTimes` | 10 | Migraram do `CraftData` para o `TechData` global. Troca de nome de chamada. |
| `CraftData.techData` / `CraftData.TechData` | 4 | Idem. |
| `Ocean.GetDepthOf`, `Subtitles.Add` | 5 | Viraram **estáticos**; basta qualificar pelo tipo. |
| `HandReticle.Hand` | 4 | Mais uma sobrecarga na extensão que já existe em `GameCompat/`. |
| `HashSet.AddIfNotPresent`, `Queue.TryDequeue` | 4 | Extensões triviais (o net472 não tem `TryDequeue`). |
| `CraftTreeHandler`, `ModCraftTreeRoot`, `KnownTechHandler`, `BioReactorHandler` | 7 | Lacunas da ponte. Os quatro **existem no Nautilus** — é só reexportar, com o cuidado do `CS0104` do §3.6. |

#### Balde B — o símbolo foi apagado do jogo

Verificado no `Assembly-CSharp` 82304: `cookedCreatureList`, `craftingTimes`,
`GetPickupSound` e `GetBindingName` **não aparecem em lugar nenhum** do assembly. Não é
um `using` faltando; o jogo removeu.

| sintoma | sítios | o que decidir |
| --- | --- | --- |
| `GameInput.GetBindingName` | 11 | Como mostrar o nome da tecla ao jogador. O `GameInputLegacy` expõe `GetBindingInternal`, mas é outra forma. |
| `CraftData.cookedCreatureList` | 6 | Reconstruir a tabela de peixe cozido. |
| `PDA.screen` | 3 | Mudou de forma. |
| `Player.pdaSpawn`, `uGUI.isLoading`, `Inventory.PickupAsync`, `EntryData.timeCapsule` | 8 | Idem, um a um. |
| `CraftData.GetPickupSound` | 1 | Idem. |

**Este balde é o que separa "compila" de "funciona".** Nenhum deles tem resposta óbvia, e
inventar chamada aqui é exatamente o que o `PROJECT_CONTEXT.md` proíbe.

### Atualização: a medição agora sai do repositório, não do scratchpad

O §3.7 acima foi medido numa cópia fora do repositório. Com a fonte do FCS importada
para `src/mods/AlterraHub/` e um `AlterraHub.csproj` que compila os 7 módulos num DLL
só, o número passou a sair do build do próprio repositório — que é o que vale.

| momento | erros | arquivos |
| --- | --- | --- |
| fonte pristina, sem alias de `TechData` | 19 | 6 |
| ⚠️ **mas** eram todos de declaração, mascarando o resto | | |
| depois do alias + `ITooltip` portado (mascaramento removido) | **101** | 52 |
| depois do balde mecânico | **59** | **29** |

Ou seja: **42 erros e 23 arquivos resolvidos**, sem nenhuma decisão de comportamento.

#### O que resolveu, e quanto cada coisa rendeu

| conserto | erros |
| --- | --- |
| `CraftData.GetItemSize/GetEquipmentType/GetCraftTime` → `TechData.*` (migraram de classe) | 14 |
| `Ocean.GetDepthOf` e `Subtitles.Add` viraram estáticos — tirar o `.main` | 7 |
| `HandReticle.Hand.X` → `GameInput.Button.X` (o enum aninhado sumiu) | 4 |
| `SpawnInfo`: alias, porque o tipo do Nautilus é `sealed` | 4 |
| `CraftTreeHandler`, `KnownTechHandler`, `BioReactorHandler` na ponte | 4 |
| `CraftData.techData` → `CraftDataHandler.GetTechData` | 4 |
| `HashSet.AddIfNotPresent` e `Queue.TryDequeue` (extensões) | 4 |
| `ITooltip` portado para a forma nova | 2 |

#### Duas descobertas que mudam como se lê o resto

**1. Os dois jogos convergiram — mas só em parte.** A fonte do FCS tem 210 arquivos com
`#if SUBNAUTICA / #elif BELOWZERO`, e em vários casos a API do Subnautica **atual** é a
que estava no ramo BELOWZERO: `ITooltip.GetTooltip(TooltipData)`, `Ocean.GetDepthOf`
estático. Isso sugeriria compilar tudo com `BELOWZERO`.

**Medi antes de acreditar: dá 160 erros contra 101.** A convergência é parcial, e o
conserto continua sítio a sítio. A hipótese era boa e estava errada — o barato foi
testá-la.

**2. `BioReactorHandler` não existe no Nautilus.** Conferido no metadata: zero tipos com
"BioReactor" no nome. O que existe é a tabela do próprio jogo,
`BaseBioReactor.charge`, um dicionário estático público — que é onde o SMLHelper
escrevia. A ponte escreve nela também; é a mesma coisa, não uma aproximação.

### O que este número não diz

Ele mede **só a suíte FCS** (7 dos mods instalados) e mede **só compilação**. Nada disso
foi aberto em jogo — este ambiente é Linux, sem Subnautica. Asset, registro de prefab e
dado de save continuam sem verificação nenhuma.

## 3.8 A quarta armadilha de medição, e a mais cara: o artefato

As três do §3.7 eram erros de *contagem*. Esta é de outra família e custou quatro
versões: **eu verifiquei a intenção, nunca o artefato entregue.**

O operador reportou "instala e não aparece nada". Diagnostiquei três vezes, sempre
lendo código, sempre encontrando um defeito **real**:

| versão | defeito encontrado | era real? | era a causa? |
| --- | --- | --- | --- |
| 1.0.3 | caminho de asset achatado no merge | sim | não |
| 1.0.5 | cada patch do FCS aplicado 7× | sim | não |
| 1.0.6 | `unlockAtStart` invertido entre Nautilus e SMLHelper | sim | não |
| 1.0.7 | **o ZIP tinha uma pasta de topo** | sim | **sim** |

Os três primeiros são defeitos legítimos e continuam corrigidos — só que nenhum podia
ser a causa, porque **o assembly nunca foi carregado**. O empacotador fazia:

```bash
( cd dist && zip -r "$nome-v$versao.zip" "$nome-v$versao" )   # ERRADO
```

Compactar a pasta *como argumento* a coloca na raiz do arquivo. O certo é compactar
**de dentro** dela, para a raiz ser `BepInEx/`:

```bash
( cd "$pkg" && zip -r "$RAIZ/$pkg.zip" . )                    # certo
```

### Por que isso escapou de tudo que eu conferia

O ZIP tinha o conteúdo certo, as DLLs certas, o `sha256sum` batia, e o guard contra
DLL de terceiro passava. Todas as verificações olhavam **o que havia dentro** do
pacote. Nenhuma olhava **onde**. Um caminho errado por um nível de pasta é invisível
para quem confere lista de arquivos e não confere a raiz.

### O que o log provou, e nenhuma leitura de código provaria

O sintoma decisivo não era um erro — era a **ausência** dele. O `LogOutput.log` não
tinha uma linha do mod, nem mesmo o `Loading [...]` que o chainloader do BepInEx
escreve **antes** de qualquer código nosso rodar. Isso separa três mundos que a
leitura de código não separa:

- há `Loading` e há exceção → o código rodou e quebrou;
- há `Loading` e não há exceção → carregou e o defeito é de lógica (era aí que eu
  estava procurando);
- **não há `Loading`** → o carregador nunca viu o assembly. É um problema de
  *instalação*, e nenhuma quantidade de leitura de código chega nele.

### A regra

**Ausência de log é evidência, e é a primeira a ser lida.** Antes de diagnosticar
comportamento, confirmar que o mod carregou. E: **abrir o próprio artefato antes de
entregá-lo** — o `empacotar.sh` agora exige `BepInEx/plugins/` na raiz do ZIP e
recusa publicar se a pasta de topo voltar.

## 3.9 Os alvos de Harmony contra o jogo atual: 68 conferidos, 0 quebrados

Compilar não prova que um patch funciona. O Harmony resolve o alvo **por nome, em
runtime** — `[HarmonyPatch(typeof(Builder), "UpdateAllowed")]` é string e reflexão, não
chamada. Método renomeado desde 2022 compila liso e derruba o módulo inteiro no
carregamento. Como o FCS é de **agosto de 2022** (upstream morto: `4275d84` é o topo do
`master`, confirmado por `ls-remote`) e o jogo está na build **82304**, essa era a
falha de runtime mais provável.

`tools/VerificarPatches` mede isso: lê os atributos do **assembly compilado** (no fonte,
`typeof` e `nameof` ainda são texto) e resolve cada alvo nas assemblies reais do jogo,
via `MetadataLoadContext` — sem executar nada.

| | |
| --- | --- |
| patches por atributo | **66**, todos resolvidos |
| alvos imperativos no jogo | **2**, ambos existem |
| alvo imperativo de outro mod | 1 (`SubnauticaMap.PingMapIcon.Refresh`) |
| **alvos inexistentes** | **0** |

Os dois imperativos escapam da varredura por atributo porque não têm atributo nenhum —
são `AccessTools.Method(typeof(ConstructorInput), "OnCraftingBegin")` e
`AccessTools.Field(typeof(BaseBioReactor), "charge")`. Ambos falhariam **silenciosamente
para o compilador** e ruidosamente em jogo: `AccessTools` devolve `null`, e daí
`harmony.Patch(null, …)` estoura. Conferidos pelo modo `MEMBROS=` do verificador —
`OnCraftingBegin` existe (método de instância) e `charge` existe (**campo estático**, o
que valida o `GetValue(tipo)` que o autor escreveu).

O terceiro aponta para o mod **SubnauticaMap**, não para o jogo, e é resolvido por
`Type.GetType(…, throwOnError: false)` dentro de um `if (type != null)`. Sem aquele mod
instalado, o trecho é pulado — não é defeito.

### ⚠️ Quarta armadilha de medição, agora do lado do verificador

O verificador **errou duas vezes antes de acertar**, e as duas foram de contagem:

1. **Contou demais (107).** Somava métodos auxiliares soltos dentro de classes com
   `[HarmonyPatch]` no tipo. O Harmony só patcheia o método anotado com
   `[HarmonyPrefix]`/`[HarmonyPostfix]`/… ou chamado `Prefix`/`Postfix`/… — auxiliar na
   mesma classe é ignorado. Aplicada a regra do próprio Harmony, caiu para 66.
2. **A conciliação contou de menos.** Comparei **nome de arquivo** com **nome de
   classe** (`EquipmentPatcher.cs` declara `Equipment_GetSlotType_Patch`) e depois
   quebrei nome de tipo aninhado com `sed 's/.*\.//'` — o formato é `Externa+Interna`,
   sem ponto. As duas coisas fabricaram "26 classes ausentes do DLL" que estavam todas
   lá.

A regra que fica é a mesma do §3.7: **quando o número surpreender, suspeite primeiro do
medidor.** Aqui, as duas conferências que resolveram foram baratas — `strings` no DLL
procurando o nome exato da classe, e olhar a saída crua em vez do `comm`.

### Roda sozinho

O workflow de release roda o verificador antes de empacotar o Alterra Hub. Se uma
atualização do jogo renomear um alvo, o build falha — em vez de o jogador descobrir.

```bash
tools/VerificarPatches/rodar.sh                                    # por atributo
MEMBROS="Tipo::Membro" tools/VerificarPatches/rodar.sh             # imperativos
```

### O que isto ainda não diz

Diz que **todo patch encontra o alvo**. Não diz que o patch faz a coisa certa: assinatura
compatível, campo com o mesmo significado, ordem de execução. E continua sem verificação
em jogo — este ambiente é Linux, sem Subnautica.

## 4. O que a ponte ainda não cobre

| Pendente | Por quê |
| --- | --- |
| `ModUtils.Save` / `LoadSaveData` | Mexe em **dados de save**. Errar aqui corrompe o save do jogador, então merece verificação própria antes de ser escrito — não vale deduzir. |
| `OptionsPanelHandler` / `ConsoleCommandsHandler` | ⚠️ Os **atributos** existem e fazem compilar, mas são **apenas de declaração**: o painel de opções e os comandos de console **ainda não são registrados em jogo**. Um mod portado agora terá as opções ignoradas — sem erro visível. Ligar ao `Nautilus.Options` e ao `ConsoleCommandsHandler` é o próximo passo. |
| `SpriteHandler`, `KnownTechHandler`, `CraftTreeHandler`, `PDAHandler`, `BioReactorHandler` | ≤6 usos cada. Medido no §3.7: 7 sítios no FCS inteiro, e os quatro últimos **existem no Nautilus** — é reexportação, não implementação. |
| Dependências externas: `MoreCyclopsUpgrades`, `NAudio` | Não são API legada, são **outros mods/bibliotecas**. O `MoreCyclopsUpgrades` o operador já tem instalado; o `NAudio` é MIT e está no NuGet. Enquanto não forem referenciados de verdade, ficam como stub **de medição, fora do repositório**. |

**Já cobertos** (era pendência, deixou de ser):

| Coberto | Como |
| --- | --- |
| `SMLHelper.V2.Assets.Equipable` | `Assets/Equipable.cs` — herda de `Craftable` e traduz `EquipmentType`/`QuickSlotType` para o gadget `SetEquipment` do Nautilus. |
| `SMLHelper.V2.Json.ExtensionMethods` | `Json/ExtensionMethods.cs` — `SaveJson`/`LoadJson` encaminhando para o `Nautilus.Json.ExtensionMethods.JsonExtensions`. Método de extensão não se repassa por herança, então aqui é redeclaração. ⚠️ Não misture os dois `using` no mesmo arquivo: dá `CS0121`. |

## 5. Ordem recomendada

1. **Alterra Hub** (FCS, MIT) — é dependência de todos os outros módulos FCS.
2. Um módulo FCS pequeno, para validar prefabs, receitas, PDA e assets de ponta a ponta.
3. Os demais módulos FCS, um a um.
4. **S.O.C.K. Tank** — usa um subconjunto do que o FCS já exige, então vem quase de graça
   *do ponto de vista técnico*. Continua bloqueado por licença: ver
   [`PORTE-SOCK-TANK.md`](PORTE-SOCK-TANK.md).

Cada módulo é testado **isolado** antes de entrar no pacote integrado, conforme o `PLAN.md`.
