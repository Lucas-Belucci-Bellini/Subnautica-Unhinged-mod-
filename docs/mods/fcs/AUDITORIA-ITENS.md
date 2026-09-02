# FCS — auditoria item a item

**Nenhuma linha "Funciona" pode ser marcada sem evidência do jogo.** Esta tabela
existe para impedir exatamente o estado que o briefing proíbe: `APARECE = OK` com
`FUNCIONA = NÃO` passando por porte concluído.

## Legenda

| | |
| --- | --- |
| ✅ | verificado |
| ⬜ | **não verificado** — não é "não", é "ninguém olhou" |
| ⛔ | verificado e quebrado |

## O que já dá para afirmar, e por quê

- **TechType ✅** — o jogo confirmou: os itens aparecem no PDA e no construtor
  desde a 1.3.0.
- **Receita ✅** — 53 blocos `RecipeData` existem no código e não foram alterados
  (ver [`FCS_RECIPE_BALANCE.md`](../../FCS_RECIPE_BALANCE.md)).
- **Modelo / Textura / Ícone ⬜** — dependem dos 7 asset bundles. A 1.4.0 passou a
  procurá-los em 5 layouts, mas **nenhuma execução confirmou que carregaram**. O
  `Unhinged-RegistroFCS.md` agora traz uma linha `📦` por bundle: é ela que
  transforma essas três colunas em ✅ ou ⛔.
- **Prefab / Funciona / Save-Load ⬜** — todos a jusante do bundle. Sem modelo não
  há o que testar.

## Tabela

| Mod | Item | TechType | Prefab | Modelo | Textura | Ícone | Receita | Fabricador | Funciona | Save/Load |
| --- | --- | :---: | :---: | :---: | :---: | :---: | :---: | :---: | :---: | :---: |
| FCS_AlterraHub | AlterraHubDepot | ✅ | ⬜ | ⬜ | ⬜ | ⬜ | ✅ | ⬜ | ⬜ | ⬜ |
| FCS_AlterraHub | AlterraHubFabricatorBuilding | ✅ | ⬜ | ⬜ | ⬜ | ⬜ | ✅ | ⬜ | ⬜ | ⬜ |
| FCS_AlterraHub | FCSDataBox | ✅ | ⬜ | ⬜ | ⬜ | ⬜ | ✅ | ⬜ | ⬜ | ⬜ |
| FCS_AlterraHub | FCSPDA | ✅ | ⬜ | ⬜ | ⬜ | ⬜ | ✅ | ⬜ | ⬜ | ⬜ |
| FCS_AlterraHub | Global | ✅ | ⬜ | ⬜ | ⬜ | ⬜ | ✅ | ⬜ | ⬜ | ⬜ |
| FCS_AlterraHub | OreConsumer | ✅ | ⬜ | ⬜ | ⬜ | ⬜ | ✅ | ⬜ | ⬜ | ⬜ |
| FCS_AlterraHub | PatreonStatue | ✅ | ⬜ | ⬜ | ⬜ | ⬜ | ✅ | ⬜ | ⬜ | ⬜ |
| FCS_EnergySolutions | AlterraGen | ✅ | ⬜ | ⬜ | ⬜ | ⬜ | ✅ | ⬜ | ⬜ | ⬜ |
| FCS_EnergySolutions | AlterraSolarCluster | ✅ | ⬜ | ⬜ | ⬜ | ⬜ | ✅ | ⬜ | ⬜ | ⬜ |
| FCS_EnergySolutions | JetStreamT242 | ✅ | ⬜ | ⬜ | ⬜ | ⬜ | ✅ | ⬜ | ⬜ | ⬜ |
| FCS_EnergySolutions | PowerStorage | ✅ | ⬜ | ⬜ | ⬜ | ⬜ | ✅ | ⬜ | ⬜ | ⬜ |
| FCS_EnergySolutions | Spawnables | ✅ | ⬜ | ⬜ | ⬜ | ⬜ | ✅ | ⬜ | ⬜ | ⬜ |
| FCS_EnergySolutions | TelepowerPylon | ✅ | ⬜ | ⬜ | ⬜ | ⬜ | ✅ | ⬜ | ⬜ | ⬜ |
| FCS_EnergySolutions | UniversalCharger | ✅ | ⬜ | ⬜ | ⬜ | ⬜ | ✅ | ⬜ | ⬜ | ⬜ |
| FCS_EnergySolutions | WindSurfer | ✅ | ⬜ | ⬜ | ⬜ | ⬜ | ✅ | ⬜ | ⬜ | ⬜ |
| FCS_HomeSolutions | AlienChef | ✅ | ⬜ | ⬜ | ⬜ | ⬜ | ✅ | ⬜ | ⬜ | ⬜ |
| FCS_HomeSolutions | BunkBed | ✅ | ⬜ | ⬜ | ⬜ | ⬜ | ✅ | ⬜ | ⬜ | ⬜ |
| FCS_HomeSolutions | Cabinets | ✅ | ⬜ | ⬜ | ⬜ | ⬜ | ✅ | ⬜ | ⬜ | ⬜ |
| FCS_HomeSolutions | CrewLocker | ✅ | ⬜ | ⬜ | ⬜ | ⬜ | ✅ | ⬜ | ⬜ | ⬜ |
| FCS_HomeSolutions | Curtains | ✅ | ⬜ | ⬜ | ⬜ | ⬜ | ✅ | ⬜ | ⬜ | ⬜ |
| FCS_HomeSolutions | DisplayBoard | ✅ | ⬜ | ⬜ | ⬜ | ⬜ | ✅ | ⬜ | ⬜ | ⬜ |
| FCS_HomeSolutions | Elevator | ✅ | ⬜ | ⬜ | ⬜ | ⬜ | ✅ | ⬜ | ⬜ | ⬜ |
| FCS_HomeSolutions | FireExtinguisherRefueler | ✅ | ⬜ | ⬜ | ⬜ | ⬜ | ✅ | ⬜ | ⬜ | ⬜ |
| FCS_HomeSolutions | HologramPoster | ✅ | ⬜ | ⬜ | ⬜ | ⬜ | ✅ | ⬜ | ⬜ | ⬜ |
| FCS_HomeSolutions | JukeBox | ✅ | ⬜ | ⬜ | ⬜ | ⬜ | ✅ | ⬜ | ⬜ | ⬜ |
| FCS_HomeSolutions | LedLights | ✅ | ⬜ | ⬜ | ⬜ | ⬜ | ✅ | ⬜ | ⬜ | ⬜ |
| FCS_HomeSolutions | Microwave | ✅ | ⬜ | ⬜ | ⬜ | ⬜ | ✅ | ⬜ | ⬜ | ⬜ |
| FCS_HomeSolutions | MiniFountainFilter | ✅ | ⬜ | ⬜ | ⬜ | ⬜ | ✅ | ⬜ | ⬜ | ⬜ |
| FCS_HomeSolutions | NeonPlanter | ✅ | ⬜ | ⬜ | ⬜ | ⬜ | ✅ | ⬜ | ⬜ | ⬜ |
| FCS_HomeSolutions | PaintTool | ✅ | ⬜ | ⬜ | ⬜ | ⬜ | ✅ | ⬜ | ⬜ | ⬜ |
| FCS_HomeSolutions | PartitionWalls | ✅ | ⬜ | ⬜ | ⬜ | ⬜ | ✅ | ⬜ | ⬜ | ⬜ |
| FCS_HomeSolutions | PeeperLoungeBar | ✅ | ⬜ | ⬜ | ⬜ | ⬜ | ✅ | ⬜ | ⬜ | ⬜ |
| FCS_HomeSolutions | QuantumTeleporter | ✅ | ⬜ | ⬜ | ⬜ | ⬜ | ✅ | ⬜ | ⬜ | ⬜ |
| FCS_HomeSolutions | Rug | ✅ | ⬜ | ⬜ | ⬜ | ⬜ | ✅ | ⬜ | ⬜ | ⬜ |
| FCS_HomeSolutions | SeaBreeze | ✅ | ⬜ | ⬜ | ⬜ | ⬜ | ✅ | ⬜ | ⬜ | ⬜ |
| FCS_HomeSolutions | Shower | ✅ | ⬜ | ⬜ | ⬜ | ⬜ | ✅ | ⬜ | ⬜ | ⬜ |
| FCS_HomeSolutions | Sink | ✅ | ⬜ | ⬜ | ⬜ | ⬜ | ✅ | ⬜ | ⬜ | ⬜ |
| FCS_HomeSolutions | Sofas | ✅ | ⬜ | ⬜ | ⬜ | ⬜ | ✅ | ⬜ | ⬜ | ⬜ |
| FCS_HomeSolutions | Stairs | ✅ | ⬜ | ⬜ | ⬜ | ⬜ | ✅ | ⬜ | ⬜ | ⬜ |
| FCS_HomeSolutions | Stove | ✅ | ⬜ | ⬜ | ⬜ | ⬜ | ✅ | ⬜ | ⬜ | ⬜ |
| FCS_HomeSolutions | TV | ✅ | ⬜ | ⬜ | ⬜ | ⬜ | ✅ | ⬜ | ⬜ | ⬜ |
| FCS_HomeSolutions | TVStand | ✅ | ⬜ | ⬜ | ⬜ | ⬜ | ✅ | ⬜ | ⬜ | ⬜ |
| FCS_HomeSolutions | Toilet | ✅ | ⬜ | ⬜ | ⬜ | ⬜ | ✅ | ⬜ | ⬜ | ⬜ |
| FCS_HomeSolutions | TrashReceptacle | ✅ | ⬜ | ⬜ | ⬜ | ⬜ | ✅ | ⬜ | ⬜ | ⬜ |
| FCS_HomeSolutions | TrashRecycler | ✅ | ⬜ | ⬜ | ⬜ | ⬜ | ✅ | ⬜ | ⬜ | ⬜ |
| FCS_LifeSupportSolutions | BaseUtilityUnit | ✅ | ⬜ | ⬜ | ⬜ | ⬜ | ✅ | ⬜ | ⬜ | ⬜ |
| FCS_LifeSupportSolutions | EnergyPillVendingMachine | ✅ | ⬜ | ⬜ | ⬜ | ⬜ | ✅ | ⬜ | ⬜ | ⬜ |
| FCS_LifeSupportSolutions | MiniMedBay | ✅ | ⬜ | ⬜ | ⬜ | ⬜ | ✅ | ⬜ | ⬜ | ⬜ |
| FCS_LifeSupportSolutions | OxygenTank | ✅ | ⬜ | ⬜ | ⬜ | ⬜ | ✅ | ⬜ | ⬜ | ⬜ |
| FCS_ProductionSolutions | AutoCrafter | ✅ | ⬜ | ⬜ | ⬜ | ⬜ | ✅ | ⬜ | ⬜ | ⬜ |
| FCS_ProductionSolutions | DeepDriller | ✅ | ⬜ | ⬜ | ⬜ | ⬜ | ✅ | ⬜ | ⬜ | ⬜ |
| FCS_ProductionSolutions | HydroponicHarvester | ✅ | ⬜ | ⬜ | ⬜ | ⬜ | ✅ | ⬜ | ⬜ | ⬜ |
| FCS_ProductionSolutions | MatterAnalyzer | ✅ | ⬜ | ⬜ | ⬜ | ⬜ | ✅ | ⬜ | ⬜ | ⬜ |
| FCS_ProductionSolutions | Replicator | ✅ | ⬜ | ⬜ | ⬜ | ⬜ | ✅ | ⬜ | ⬜ | ⬜ |
| FCS_StorageSolutions | AlterraStorage | ✅ | ⬜ | ⬜ | ⬜ | ⬜ | ✅ | ⬜ | ⬜ | ⬜ |
| FCS_StorageSolutions | DataStorageSolutions | ✅ | ⬜ | ⬜ | ⬜ | ⬜ | ✅ | ⬜ | ⬜ | ⬜ |

## Contagem

| | |
| --- | ---: |
| itens (pastas `Mods/<Nome>/`) | **56** |
| com TechType confirmado em jogo | 56 |
| com bundle confirmado | **0** |
| com comportamento confirmado | **0** |
| sobreviveram a save/load | **0** |

## Como preencher

1. Instale a 1.4.0 e rode o jogo uma vez.
2. Abra `BepInEx/Unhinged-RegistroFCS.md` e olhe as linhas `📦`.
3. Bundle que carregou → Modelo/Textura/Ícone viram ✅ para os itens daquele módulo.
4. Bundle que não carregou → o próprio relatório lista os caminhos tentados.
5. Só então testar comportamento e save/load, item a item.
