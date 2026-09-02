# FCS — registro de conteúdo

**TechTypes/ClassIDs distintos declarados no código: 88.**

> ⚠️ Isto é o que o código **declara**. Não é o que o jogo **registrou** —
> essa é outra medição, e ela sai do `BepInEx/Unhinged-RegistroFCS.md` que o
> plugin escreve a cada partida. Um ClassID aqui e ausente lá é exatamente o
> defeito que estamos caçando.

## Colisões

| contra | colisões |
| --- | ---: |
| entre os próprios módulos FCS | **0** |
| **PrototypeSub** (63 TechTypes literais) | **0** |

Medido comparando os conjuntos dos dois códigos. Zero em ambos.

## Por módulo

### FCS_AlterraHub — 9

`AlterraHub`, `AlterraHubDepot`, `AlterraHubStation`, `DebitCard`, `DronePortPad`, `DronePortPad_Kit`, `FCSKit`, `OreConsumer`, `PatreonStatue`

### FCS_EnergySolutions — 1

`FCSBioFuel`

### FCS_HomeSolutions — 75

`CabinetMediumTall`, `CabinetTVStand`, `CabinetTall`, `CabinetWide`, `DisplayBoard`, `DisplayBoard_Kit`, `Elevator`, `Elevator_Kit`, `EmptyObservationTank`, `FCSCrewLocker`, `FCSJukeBoxSubWoofer`, `FCSJukebox`, `FCSJukeboxSpeaker`, `FCSShower`, `FCSSink`, `FCSStairs`, `FCSStove`, `FCSToilet`, `FireExtinguisherRefueler`, `FireExtinguisherRefueler_Kit`, `HologramPoster`, `JukeBoxSubWoofer_Kit`, `JukeboxSpeaker_Kit`, `Jukebox_Kit`, `MiniFountainFilter`, `NeonBarStool`, `NeonBarStool_Kit`, `NeonPlanter`, `PaintCan`, `PaintTool`, `PaintTool_Kit`, `QuantumPowerBankCharger`, `QuantumTeleporter`, `QuantumTeleporterVehiclePad`, `Recycler`, `Recycler_Kit`, `Seabreeze`, `Sofa1`, `Sofa1_Kit`, `Sofa2`, `Sofa2_Kit`, `Sofa3`, `Sofa3_Kit`, `Stairs_Kit`, `Toilet_Kit`, `TrashReceptacle`, `TrashReceptacle_Kit`, `ahsleftcornerrailing_kit`, `ahsleftcornerwGlassrailing_kit`, `ahsrailing_kit`, `ahsrailingglass_kit`, `ahsrightcornerrailing_kit`, `ahsrightcornerwGlassrailing_kit`, `curingCabinet_kit`, `floorShelf01_kit`, `floorShelf02_kit`, `floorShelf03_kit`, `floorShelf04_kit`, `floorShelf05_kit`, `floorShelf06_kit`, `floorShelf07_kit`, `microwave_kit`, `mountSmartTV_kit`, `neonShelf01_kit`, `neonShelf02_kit`, `neonShelf03_kit`, `neonTable01_kit`, `neonTable02_kit`, `outsideSign_kit`, `partitionWall_kit`, `pccpu_kit`, `pcmonitor_kit`, `rug_kit`, `tableSmartTV_kit`, `wallSign_kit`

### FCS_LifeSupportSolutions — 1

`BaseOxygenTank`

### FCS_ProductionSolutions — 1

`Sand_DD`

### FCS_StorageSolutions — 1

`ItemTransferUnit_Kit`

## Como conferir contra o runtime

```text
BepInEx/Unhinged-RegistroFCS.md   ← o que REGISTROU
docs/mods/fcs/FCS-CONTENT-REGISTRY.md  ← o que o codigo DECLARA
```

A diferença entre os dois é a resposta do P0. Se o primeiro disser
"NENHUM item tentou se registrar", o problema é de carregamento, e nem
receita nem `unlock all` têm a ver com isso.
