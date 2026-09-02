# FCS × PrototypeSub — compatibilidade

## Estado do PrototypeSub analisado

```text
Repositório:  https://github.com/Indigocoder1/PrototypeSub
Branch:       main
Commit:       3b3465f  —  "Fix incorrect variable use", 29/07/2026
Licença:      MIT
Tamanho:      638 arquivos .cs
```

## Dependências que ele declara

`[BepInDependency]`, extraídos do código:

| dependência | também usada pelo FCS? |
| --- | --- |
| `com.snmodding.nautilus` | ✅ **sim** — é a única compartilhada |
| `com.indigocoder.sublibrary` (Sub Library) | ❌ não |
| `Indigocoder.SuitLib` (Suit Library) | ❌ não |
| `Esper89.TerrainPatcher` | ❌ não |
| `ArchitectsLibrary` | ❌ não |
| `com.mikjaw.subnautica.vehicleframework.mod` | ❌ não |
| `com.aci.thesilence` · `com.danithedani.deepercreatures` · `com.digaoness.CyclopsModules` · `com.lee23.epicweather` · `com.lee23.theredplague` | ❌ não |

**Nenhuma delas é empacotada por nós.** O pacote FCS declara dependência e não
duplica biblioteca — nem Nautilus, nem Sub Library, nem Suit Library, nem
TerrainPatcher, nem Epic Structure Loader. O `empacotar.sh` recusa o build se
qualquer DLL que não seja `Unhinged.*` aparecer dentro do pacote.

⚠️ Não encontrei referência a **Epic Structure Loader** no código do PrototypeSub
neste commit. Se ele for dependência, é por outro caminho que a leitura do código
não mostra — não vou afirmar que é nem que não é.

## Conteúdo: colisão zero, medida nos dois lados

| | |
| --- | ---: |
| TechTypes declarados pelo FCS | 88 |
| TechTypes declarados pelo PrototypeSub | 63 |
| **colisões** | **0** |

⚠️ A primeira medição deu "0 colisões" por acidente: o PrototypeSub reportava
**zero** ClassIDs porque ele usa `PrefabInfo.WithTechType("...")`, e eu procurava
`ClassID = "..."`. Zero contra zero não é evidência de nada. Refeito extraindo os
63 literais do padrão que ele realmente usa, e aí sim o zero significa alguma
coisa.

## Harmony: 8 tipos em comum

De 71 tipos patcheados pelo PrototypeSub e 25 pelo FCS:

| tipo do jogo | risco |
| --- | --- |
| `Equipment` | ⚠️ transpiler do lado do PrototypeSub |
| `Player` | ⚠️ transpiler do lado do PrototypeSub |
| `SubRoot` | ⚠️ transpiler do lado do PrototypeSub |
| `uGUI_Equipment` | ⚠️ transpiler do lado do PrototypeSub |
| `IngameMenu` | prefixo/postfixo dos dois lados |
| `PDAEncyclopedia` | prefixo/postfixo dos dois lados |
| `PDAScanner` | prefixo/postfixo dos dois lados |
| `VehicleDockingBay` | prefixo/postfixo dos dois lados |

### O que isso significa, e o que não significa

**O FCS não tem nenhum transpiler.** Isso importa: o caso que corrompe IL é
**dois transpilers no mesmo método**, e ele não acontece aqui. Transpiler de um
lado com prefixo/postfixo do outro é o arranjo normal — o Harmony aplica o
transpiler ao IL e envolve o resultado com os prefixos e postfixos.

⚠️ **Limite desta medição:** ela é por **arquivo**, não por método. Sei que o
arquivo que patcheia `Equipment` no PrototypeSub também contém um transpiler; não
sei, por leitura estática, se o transpiler está no *mesmo método* que o FCS
patcheia. Chamar isso de "compatível" seria ir além do que eu medi.

## Veredito

**`NOT TESTED`.** Não há execução nenhuma por trás disto — o ambiente de build é
Linux sem Subnautica.

O que a análise estática diz é mais modesto: **não encontrei colisão de conteúdo
nem o padrão de conflito que corrompe IL.** Isso reduz a lista do que procurar em
jogo; não substitui procurar.

Os 8 tipos em comum são exatamente onde olhar primeiro no Cenário B, e os 4 com
transpiler antes dos outros.
