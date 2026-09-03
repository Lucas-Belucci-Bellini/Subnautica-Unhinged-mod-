# FCS — snapshot do upstream

Clone **completo** (não raso) de `https://github.com/ccgould/FCStudios_SubnauticaMods`,
medido em 2026-09-03.

| | |
| --- | --- |
| commits | **632** |
| primeiro | `b4b7909b` · 2018-09-05 · *Add .gitignore and .gitattributes* |
| **HEAD** | **`4275d847`** · 2022-08-19 · *AlterraHub Mod Suite V1.0.2* |
| branches | **1** — `master` |
| tags | **0** |
| licença | **MIT**, Copyright (c) 2020 Field Creator Studios |
| assets de arte | **0** — ver [`ASSETS.md`](ASSETS.md) |

> ⚠️ O clone que este projeto usava era **raso** (1 commit). Toda a análise
> histórica abaixo só ficou possível depois do `git fetch --unshallow --tags`.
> Um clone raso responde "0 tags" e "1 commit" sem avisar que não sabe — e as
> duas respostas parecem fato.

## O upstream está morto

Último commit em **agosto de 2022**. Zero tags, um branch. Não há release
marcada, então "a versão do upstream" é o texto da mensagem do commit: `V1.0.2`.

## ⚠️ Upstream ≠ build atual (§3 do briefing)

O upstream é a fonte do **código, comportamento, receitas e intenção**. Ele
**não** é referência para a API do Subnautica atual: foi escrito para
QModManager + SMLHelper, e a build alvo é BepInEx + Nautilus. Ver
[`PORTING.md`](PORTING.md).
