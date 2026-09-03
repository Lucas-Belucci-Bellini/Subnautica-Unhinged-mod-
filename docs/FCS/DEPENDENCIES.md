# FCS — dependências

## Obrigatórias (sem elas o pacote não funciona)

| Dependência | Versão alvo | Redistribuída? | Nota |
| --- | --- | :---: | --- |
| **BepInEx** | 5.4.21 | ❌ | o jogador instala |
| **Nautilus** | 1.0.0-pre.53 | ❌ | o jogador instala |
| **HarmonyX** | 2.7 (via BepInEx) | ❌ | vem com o BepInEx |
| `Unhinged.Legacy.dll` | igual ao pacote | ✅ | ponte SMLHelper→Nautilus, **é nossa** |

## Obrigatórias e NÃO redistribuíveis — os 7 asset bundles

Sem eles os itens existem e não funcionam. Não estão sob a MIT do upstream (ver
[`ASSETS.md`](ASSETS.md)); vêm da instalação do próprio jogador.

```
fcsalterrahubbundle · fcsenergysolutionsbundle · fcshomesolutionsbundle
fcslifesupportsolutionsbundle · fcsproductionsolutionsbundle
fcsstoragesolutionsbundle · cyclopsupgradeconsolebundle
```

Desde a 1.4.0 o mod procura em 5 layouts, incluindo o `QMods/` do QModManager.

## Opcionais (o upstream detecta em runtime, e segue sem)

| Mod | Uso | Como o FCS trata a ausência |
| --- | --- | --- |
| `MoreCyclopsUpgrades` | integração do console do Cyclops | referência de compilação do upstream |
| `SubnauticaMap` | ícone de ping no mapa | `Type.GetType(...)` + `if (type != null)` |
| `DockedVehicleStorageAccess` | acesso a storage acoplado | flag `IsDockedVehicleStorageAccessInstalled` |
| `NAudio` | áudio no upstream | referência de compilação |

## Incompatíveis — não conviver

| GUID | Por quê |
| --- | --- |
| QModManager | pilha legada; dois frameworks patcheando os mesmos métodos |
| SMLHelper (o real) | a ponte reimplementa `SMLHelper.V2.*`; os dois juntos = indefinido |

O plugin **recusa carregar** nesse cenário desde a 1.0.4. Há a chave
`ForcarComPilhaLegada` para quem quiser tentar assim mesmo.

## ⚠️ Conflito medido, ainda não resolvido: AlterraDecor

`AlterraDecor 1.1.0` registra **89 TechTypes** e carrega **antes** do nosso
plugin. Medi **32 ClassIDs em comum** — `AlterraHubDepot`, `OreConsumer`,
`Seabreeze`, `Recycler`, `QuantumPowerBank`, `FCSShower`, `FCSJukebox`,
`Sofa1/2/3`, entre outros. Ele é um port de parte da mesma suíte FCS.

**Não sei o que o Nautilus faz com TechType duplicado.** Tentei ler a fonte e não
consegui; não vou afirmar. O teste que isola a variável é rodar uma vez com o
AlterraDecor desativado.

## Licença do conjunto

Código FCS **MIT** (Field Creator Studios) + ponte e empacotamento
**GPL-3.0-or-later** — a GPL é obrigatória pelo link com o Nautilus, e o
resultado combinado sai sob ela. Créditos preservados em `CREDITOS.md` e
`LICENSE-FCS.txt`, dentro de todo pacote.
