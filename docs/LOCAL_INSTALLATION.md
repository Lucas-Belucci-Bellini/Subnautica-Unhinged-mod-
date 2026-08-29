# Inventário local — Subnautica e Vortex

Snapshot feito em 29/08/2026 para orientar o desenvolvimento do Subnautica Unhinged.

## Regra de segurança

Este arquivo registra apenas metadados e nomes de componentes. Não copiar para este
repositório: `Subnautica.exe`, DLLs do jogo, assemblies de Unity, assets distribuídos
com o jogo, saves, cache do Vortex, arquivos `.msgpack` ou DLLs de mods de terceiros.
Esses arquivos podem ser usados localmente como referências e durante os testes.

## Caminhos locais

- Jogo: `C:\Program Files (x86)\Steam\steamapps\common\Subnautica`
- Assemblies para referência local: `Subnautica_Data\Managed\Assembly-CSharp.dll`
- Plugins modernos: `BepInEx\plugins`
- Configurações: `BepInEx\config`
- Mods legados: `QMods`
- Mods gerenciados pelo Vortex: `C:\Users\Usuario\AppData\Roaming\Vortex\subnautica\mods`
- Perfis do Vortex: `C:\Users\Usuario\AppData\Roaming\Vortex\subnautica\profiles`

## Camada moderna detectada em `BepInEx\plugins`

`Nautilus`, `ECCLibrary`, `VehicleFramework`, `SubLibrary`, `SuitLib`,
`TerrainPatcher`, `Scan for Anything`, `Echelon`, `PrototypeSubMod`,
`AIOFabricator`, `AIS`, `AlienRifle`, `AlterraDecor`, `AlterraWeaponry`,
`Archon`, `BaseIntegrityCustomizer`, `Beluga`, `BlossomSubmarine`,
`BuilderModule`, `CameraStalkerGuard`, `Comforts`, `CompositeBuildables`,
`ConfigurationManager`, `CustomBatteries`, `CustomBedsSN`, `CustomCraft3`,
`CustomPosters`, `CyclopsDockingMod`, `DisableOptionsTabs`,
`DockedVehicleStorageAccess`, `EpicStructureLoader`, `FabricatorLocker`,
`FindMyUpdates`, `FloatingFoundations`, `Gargantuan Leviathan Beds`, `Hydra`,
`IonDefenseCapacitor`, `Kallie'sPropPack`, `MoreCyclopsUpgrades`,
`NanoTanksMod`, `NanoWeaveBarrier`, `OtherStorageIncrease v1.1.0`,
`PdaUpgradeChips`, `PowerBank`, `RadialTabs`, `RamunesWorkbench`,
`ResourceMonitor`, `Rm_PowerModifier`, `SeaglideUpgrades`, `SeamothAIUpgrade`,
`SeaVoyager`, `ShowUnlockRequirements`, `SolidTerrain`, `SonarModule`,
`SubFurniturica`, `SubHelperX`, `Titanfall2 bed`, `VanillaExpandedLoreFriendly`.

## Camada legada detectada em `QMods`

- AchievementUnlocker
- AutosortLockersSML
- ConsoleImproved
- HabitatPlatform
- Modding Helper

QMods, SMLHelper e QModManager devem ser tratados como legado. O alvo do projeto
é BepInEx/Nautilus; não combinar as duas camadas automaticamente dentro da mesma
DLL.

## Pacotes encontrados no Vortex

O Vortex contém, entre outros, pacotes de AchievementUnlocker, AIUpgrade, Alien
Rifle, All-in-One Fabricator, Alterra Industrial Scrapper, Alterra Weaponry,
AlterraDecor, Archon, Autosort Lockers SML, Base Hull Integrity Customizer,
Beluga, BlossomSubmarine, BuilderModule, CameraStalkerGuard, Comforts,
Composite Buildables, Configuration Manager, ConsoleImproved, Custom Posters,
CustomBatteries, CustomBedsSN, CustomCraft3, Cyclops Docking Mod, Cyclops Vehicle
Upgrades, DemonSlayer Posters, Disable Options Tabs, Docked Vehicle Storage
Access, ECC Library 2.0, Echelon, Epic Structure Loader, Fabricator Lockers,
Find My Updates, FloatingFoundations, FutaMommyBeds, Gargantuan Leviathan Beds,
HabitatPlatform, Hydra, Ion Defense Capacitor, Kallie's Prop Pack, More Cyclops
Upgrades, More Storage Mod, Nano-Weave Barrier, NanoTanksMod, Nautilus, PDA
Upgrade Chips, Phazon Batteries, posters adicionais, PowerBank, Prototype Sub,
QModManager, Radial Tabs, Ramune's Workbench, Resource Monitor, Rm_PowerModifier,
Scan for Anything, Scanner Speed Multiplier, Sea Voyager, Seaglide Upgrades,
Show Locked Items, SMLHelper, Solid Terrain, Sonar Module, Sub Library,
SubFurniturica, SubHelperX, SuitLib, TerrainPatcher, Tobey's BepInEx Pack,
Vanilla Expanded, Vehicle Framework e VisibleLockerInterior.

A lista acima é um inventário do estado local, não uma autorização para redistribuir
esses pacotes. Cada mod precisa manter seu autor, licença e permissões originais.

## Decisão de engenharia

É recomendado usar o jogo instalado apenas como ambiente local de referência e
teste. O projeto deve referenciar as assemblies localmente, gerar código próprio
em `src/` e registrar dependências por documentação/configuração. O primeiro
protótipo recomendado continua sendo o scanner expandido, com limite configurável,
detecção de leviatãs e atualização escalonada para controlar desempenho.
