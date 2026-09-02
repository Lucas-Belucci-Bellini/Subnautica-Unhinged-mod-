# FC Studios — matriz de funcionalidades

**56 features** em pasta própria, nos 7 módulos. Contagem por `Mods/<Nome>/` no
código, que é como o FCS organiza cada item construível.

⚠️ **Nenhuma linha da coluna "Testada" está marcada.** O ambiente de build é
Linux sem Subnautica.

| Módulo | features | Original | Modernizada | Testada |
| --- | ---: | :---: | :---: | :---: |
| **FCS_HomeSolutions** | 30 | ✅ | ✅ compila | ⬜ |
| **FCS_EnergySolutions** | 8 | ✅ | ✅ compila | ⬜ |
| **FCS_AlterraHub** (base) | 7 | ✅ | ✅ compila | ⬜ |
| **FCS_ProductionSolutions** | 5 | ✅ | ✅ compila | ⬜ |
| **FCS_LifeSupportSolutions** | 4 | ✅ | ✅ compila | ⬜ |
| **FCS_StorageSolutions** | 2 | ✅ | ✅ compila | ⬜ |
| **FCS_CyclopsUpgradeConsole** | — | ✅ | ✅ compila | ⬜ |

## Por módulo

**FCS_AlterraHub** (7) — AlterraHubDepot · AlterraHubFabricatorBuilding ·
FCSDataBox · FCSPDA · Global · OreConsumer · PatreonStatue

**FCS_EnergySolutions** (8) — AlterraGen · AlterraSolarCluster · JetStreamT242 ·
PowerStorage · Spawnables · TelepowerPylon · UniversalCharger · WindSurfer

**FCS_HomeSolutions** (30) — AlienChef · BunkBed · Cabinets · CrewLocker ·
Curtains · DisplayBoard · Elevator · FireExtinguisherRefueler · e mais 22

**FCS_LifeSupportSolutions** (4) — BaseUtilityUnit · EnergyPillVendingMachine ·
MiniMedBay · OxygenTank

**FCS_ProductionSolutions** (5) — AutoCrafter · DeepDriller ·
HydroponicHarvester · MatterAnalyzer · Replicator

**FCS_StorageSolutions** (2) — AlterraStorage · DataStorageSolutions

**FCS_CyclopsUpgradeConsole** — item único (`AuxiliaryUpgradeConsole`), sem
pasta `Mods/`. Depende do mod **MoreCyclopsUpgrades**.

## O que "Modernizada ✅ compila" garante, e o que não

**Garante:** o código do item está no assembly, compila contra a build 82304, e
todo alvo de patch/reflexão que ele usa foi resolvido por metadata.

**Não garante:** que o item aparece no jogo, que a receita registra, que o prefab
carrega, que o modelo existe. Sem os **asset bundles** — que não são nossos e
não estão no upstream — cada item aparece sem modelo e sem ícone.

## Como a contagem foi feita, e onde ela erra

```bash
find <modulo>/Mods -maxdepth 1 -mindepth 1 -type d | wc -l
```

⚠️ A primeira tentativa contou `ClassID = "..."` literal no fonte e deu números
absurdos (Energy com 1, Cyclops com 0). O `ClassID` vem de constantes em
`Mod.cs`, não de string no arquivo do item — a busca literal não enxerga isso.
A contagem por pasta bate com os nomes de módulo que o código procura em disco.

Uma pasta ≠ exatamente um item construível: `Spawnables` e `Global` agrupam
vários, e `Cabinets` tem quatro variantes. **56 é o número de features, não de
TechTypes.** TechTypes distintos são 88 (`ClassID` únicos no pacote inteiro).
